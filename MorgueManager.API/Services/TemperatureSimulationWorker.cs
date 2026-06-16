using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MorgueManager.API.Data;
using MorgueManager.API.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MorgueManager.API.Services;

public class TemperatureSimulationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TemperatureSimulationWorker> _logger;
    private readonly Random _random;

    public TemperatureSimulationWorker(IServiceProvider serviceProvider, ILogger<TemperatureSimulationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _random = new Random();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Temperature Simulation Background Worker is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var slots = context.StorageSlots.ToList();

                    if (slots.Any())
                    {
                        var timestamp = DateTime.Now;
                        var logs = slots.Select(slot =>
                        {
                            double targetTemp;
                            
                            switch (slot.Status)
                            {
                                case SlotStatus.Occupied:
                                    // Regulated cold temperature: 2.0 to 4.0, with a 5% chance of temp anomaly (malfunction simulation)
                                    if (_random.Next(1, 101) <= 5)
                                    {
                                        targetTemp = Math.Round(5.0 + (_random.NextDouble() * 2.0), 1);
                                    }
                                    else
                                    {
                                        targetTemp = Math.Round(2.0 + (_random.NextDouble() * 2.0), 1);
                                    }
                                    break;
                                case SlotStatus.Cleaning:
                                    // Room temperature during sanitation: 14.0 to 18.0
                                    targetTemp = Math.Round(14.0 + (_random.NextDouble() * 4.0), 1);
                                    break;
                                case SlotStatus.Maintenance:
                                    // Warmer: 10.0 to 15.0
                                    targetTemp = Math.Round(10.0 + (_random.NextDouble() * 5.0), 1);
                                    break;
                                case SlotStatus.Empty:
                                default:
                                    // Fluctuates around 3.5 to 5.5
                                    targetTemp = Math.Round(3.5 + (_random.NextDouble() * 2.0), 1);
                                    break;
                            }

                            slot.CurrentTemperature = targetTemp;

                            return new TemperatureLog
                            {
                                StorageSlotId = slot.Id,
                                Temperature = targetTemp,
                                Timestamp = timestamp
                            };
                        }).ToList();

                        context.TemperatureLogs.AddRange(logs);
                        await context.SaveChangesAsync(stoppingToken);

                        // Scan for overheating occupied slots and write warning notifications
                        var overheatingSlots = slots.Where(s => s.Status == SlotStatus.Occupied && s.CurrentTemperature > 4.5).ToList();
                        var webhookDispatcher = scope.ServiceProvider.GetService<WebhookDispatcher>();
                        foreach (var slot in overheatingSlots)
                        {
                            var corpseName = context.Corpses.FirstOrDefault(c => c.StorageSlotId == slot.Id)?.Name ?? "Chưa rõ";
                            bool alreadyAlerted = context.Notifications.Any(n => 
                                n.Content.Contains($"ngăn tủ {slot.SlotNumber}") && !n.IsRead);

                            if (!alreadyAlerted)
                            {
                                var alert = new Notification
                                {
                                    Title = "Cảnh báo quá nhiệt hộc lạnh",
                                    Content = $"Ngăn tủ {slot.SlotNumber} (đang lưu trữ thi hài [{corpseName}]) có nhiệt độ cao bất thường: {slot.CurrentTemperature}°C!",
                                    IsRead = false,
                                    Timestamp = DateTime.Now
                                };
                                context.Notifications.Add(alert);
                                _logger.LogWarning("ALERT: Slot {SlotNumber} containing corpse [{CorpseName}] is overheating at {Temp}°C!", slot.SlotNumber, corpseName, slot.CurrentTemperature);
                                
                                if (webhookDispatcher != null)
                                {
                                    await webhookDispatcher.SendWebhookAlertAsync("OVERHEATING", $"Cảnh báo quá nhiệt: Ngăn tủ {slot.SlotNumber} (đang lưu trữ thi hài [{corpseName}]) có nhiệt độ cao bất thường: {slot.CurrentTemperature}°C!");
                                }
                            }
                        }

                        if (context.ChangeTracker.HasChanges())
                        {
                            await context.SaveChangesAsync(stoppingToken);
                        }

                        _logger.LogInformation("Successfully updated temperature and logged for {Count} storage slots.", slots.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Temperature Simulation Worker.");
            }

            // Simulate every 10 seconds
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        _logger.LogInformation("Temperature Simulation Background Worker is stopping.");
    }
}
