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

public class OverdueScannerWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OverdueScannerWorker> _logger;

    public OverdueScannerWorker(IServiceProvider serviceProvider, ILogger<OverdueScannerWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Overdue Storage Scanner Worker is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Running daily overdue storage scan...");
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var activeCorpses = context.Corpses
                        .Where(c => c.Status != "Bàn giao")
                        .ToList();

                    var today = DateTime.Today;
                    int scanCount = 0;
                    int alertCount = 0;

                    foreach (var corpse in activeCorpses)
                    {
                        scanCount++;
                        // Update DaysStored based on DateAdmitted
                        if (DateTime.TryParse(corpse.DateAdmitted, out var admittedDate))
                        {
                            var days = (int)(DateTime.Today - admittedDate.Date).TotalDays;
                            corpse.DaysStored = days > 0 ? days : 1;
                        }

                        if (corpse.DaysStored >= 14)
                        {
                            alertCount++;
                            // Check if already notified/audited today to prevent duplicates
                            bool alreadyNotified = context.Notifications.Any(n => 
                                n.Content.Contains(corpse.CaseId) && n.Timestamp >= today);

                            if (!alreadyNotified)
                            {
                                var notification = new Notification
                                {
                                    Title = "Cảnh báo lưu trữ quá hạn",
                                    Content = $"Thi thể {corpse.Name} (Mã HS: {corpse.CaseId}) đã được lưu trữ {corpse.DaysStored} ngày.",
                                    IsRead = false,
                                    Timestamp = DateTime.Now
                                };
                                context.Notifications.Add(notification);

                                var auditLog = new AuditLog
                                {
                                    UserEmail = "system@hospital.vn",
                                    Action = "WARNING",
                                    EntityName = "Corpse",
                                    EntityId = corpse.Id.ToString(),
                                    Details = $"Cảnh báo tự động: Thi thể {corpse.Name} ({corpse.CaseId}) đã lưu trữ quá hạn {corpse.DaysStored} ngày.",
                                    Timestamp = DateTime.Now
                                };
                                context.AuditLogs.Add(auditLog);
                            }
                        }
                    }

                    if (context.ChangeTracker.HasChanges())
                    {
                        await context.SaveChangesAsync(stoppingToken);
                    }

                    _logger.LogInformation("Scan finished. Processed {ScanCount} corpses. Generated alerts for {AlertCount} overdue corpses.", scanCount, alertCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Overdue Storage Scanner Worker.");
            }

            // Run once a day (24 hours)
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }

        _logger.LogInformation("Overdue Storage Scanner Worker is stopping.");
    }
}
