using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MorgueManager.API.Data;
using MorgueManager.API.Models;
using System;
using System.Linq;

namespace MorgueManager.API.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AnalyticsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public IActionResult GetDashboardStats()
    {
        try
        {
            var corpses = _context.Corpses.ToList();
            var slots = _context.StorageSlots.ToList();
            var notifications = _context.Notifications.OrderByDescending(n => n.Timestamp).Take(10).ToList();

            var activeCorpses = corpses.Where(c => c.Status != "Bàn giao").ToList();

            var totalSlots = slots.Count;
            var occupiedSlots = slots.Count(s => s.Status == SlotStatus.Occupied);
            var emptySlots = slots.Count(s => s.Status == SlotStatus.Empty);
            var maintenanceSlots = slots.Count(s => s.Status == SlotStatus.Maintenance);

            double occupancyRate = totalSlots > 0 ? Math.Round((double)occupiedSlots * 100.0 / totalSlots, 1) : 0.0;

            var causeOfDeathDist = activeCorpses
                .GroupBy(c => string.IsNullOrWhiteSpace(c.CauseOfDeath) ? "Không rõ" : c.CauseOfDeath)
                .Select(g => new { Cause = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var monthlyTrends = corpses
                .Where(c => !string.IsNullOrEmpty(c.DateAdmitted))
                .GroupBy(c => {
                    if (DateTime.TryParse(c.DateAdmitted, out var date))
                    {
                        return date.ToString("yyyy-MM");
                    }
                    return "Không xác định";
                })
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .OrderBy(x => x.Month)
                .ToList();

            return Ok(new
            {
                TotalCorpses = activeCorpses.Count,
                TotalSlots = totalSlots,
                OccupiedSlots = occupiedSlots,
                EmptySlots = emptySlots,
                MaintenanceSlots = maintenanceSlots,
                OccupancyRate = occupancyRate,
                CauseOfDeathDistribution = causeOfDeathDist,
                MonthlyTrends = monthlyTrends,
                RecentNotifications = notifications
            });
        }
        catch (Exception ex)
        {
            return Problem($"Lỗi khi tải dữ liệu thống kê: {ex.Message}");
        }
    }
}
