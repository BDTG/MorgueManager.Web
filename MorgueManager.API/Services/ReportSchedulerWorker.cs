using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MorgueManager.API.Data;
using MorgueManager.API.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MorgueManager.API.Services;

public class ReportSchedulerWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReportSchedulerWorker> _logger;

    public ReportSchedulerWorker(IServiceProvider serviceProvider, ILogger<ReportSchedulerWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Monthly Report Scheduler Background Worker is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Generating monthly activity report statistics...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    
                    var totalCorpses = context.Corpses.Count();
                    var activeCorpses = context.Corpses.Count(c => c.Status != "Bàn giao");
                    var releasedCorpses = context.Corpses.Count(c => c.Status == "Bàn giao");
                    
                    var totalSlots = context.StorageSlots.Count();
                    var occupiedSlots = context.StorageSlots.Count(s => s.Status == SlotStatus.Occupied);
                    double occupancyRate = totalSlots > 0 ? Math.Round((double)occupiedSlots / totalSlots * 100, 2) : 0;

                    var reportContent = $@"=========================================
BÁO CÁO THỐNG KÊ HOẠT ĐỘNG NHÀ XÁC
Thời gian lập: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
=========================================
1. Thống kê thi hài:
   - Tổng số thi hài đã tiếp nhận (lịch sử): {totalCorpses}
   - Số thi hài đang lưu trữ hiện tại: {activeCorpses}
   - Số thi hài đã bàn giao / xuất viện: {releasedCorpses}

2. Hiệu suất hộc lạnh:
   - Tổng số hộc tủ: {totalSlots}
   - Số hộc tủ đang sử dụng (Occupied): {occupiedSlots}
   - Tỷ lệ lấp đầy hộc tủ: {occupancyRate}%

=========================================";

                    // Write to Logs/monthly-report.txt
                    var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                    if (!Directory.Exists(logDir))
                    {
                        Directory.CreateDirectory(logDir);
                    }
                    var reportPath = Path.Combine(logDir, "monthly-report.txt");
                    await File.WriteAllTextAsync(reportPath, reportContent, stoppingToken);

                    _logger.LogInformation("Monthly report statistics successfully written to {Path}.", reportPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Report Scheduler Background Worker.");
            }

            // Run once every 24 hours (simulates cron scheduler)
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }

        _logger.LogInformation("Monthly Report Scheduler Background Worker is stopping.");
    }
}
