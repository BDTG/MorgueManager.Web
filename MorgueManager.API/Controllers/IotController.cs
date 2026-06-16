using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MorgueManager.API.Data;
using MorgueManager.API.Models;
using System;
using System.Threading.Tasks;

namespace MorgueManager.API.Controllers;

[ApiController]
[Route("api/iot")]
public class IotController : ControllerBase
{
    private readonly AppDbContext _context;

    public IotController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("weight-log")]
    public async Task<IActionResult> LogWeight([FromBody] WeightLogDto dto)
    {
        var slot = await _context.StorageSlots.FirstOrDefaultAsync(s => s.SlotNumber == dto.SlotNumber);
        if (slot == null)
        {
            return NotFound(new { Message = $"Không tìm thấy hộc lạnh có mã {dto.SlotNumber}." });
        }

        // Find the corpse currently stored in this slot
        var corpse = await _context.Corpses.FirstOrDefaultAsync(c => c.StorageSlotId == slot.Id);

        if (corpse != null)
        {
            // Security Anomaly: Weight falls below 5.0 kg for an occupied slot, but status is not "Bàn giao"
            if (dto.Weight < 5.0 && corpse.Status != "Bàn giao")
            {
                // Verify if notification already exists to avoid spamming
                bool alreadyAlerted = await _context.Notifications.AnyAsync(n => 
                    n.Content.Contains($"mất trọng lượng tại hộc lạnh {slot.SlotNumber}") && !n.IsRead);

                if (!alreadyAlerted)
                {
                    var notification = new Notification
                    {
                        Title = "BÁO ĐỘNG ĐỎ: DI DỜI THI HÀI TRÁI PHÉP",
                        Content = $"Báo động: Phát hiện mất trọng lượng tại hộc lạnh {slot.SlotNumber} (thi hài [{corpse.Name}]) khi chưa có lệnh bàn giao!",
                        IsRead = false,
                        Timestamp = DateTime.Now
                    };
                    _context.Notifications.Add(notification);

                    var auditLog = new AuditLog
                    {
                        UserEmail = "system@hospital.vn",
                        Action = "CRITICAL",
                        EntityName = "StorageSlot",
                        EntityId = slot.Id.ToString(),
                        Details = $"Cảnh báo bảo mật: Thi hài {corpse.Name} ({corpse.CaseId}) trong hộc tủ {slot.SlotNumber} bị di dời không hợp lệ. Trọng lượng hiện tại: {dto.Weight}kg.",
                        Timestamp = DateTime.Now
                    };
                    _context.AuditLogs.Add(auditLog);

                    await _context.SaveChangesAsync();
                }
            }
        }

        return Ok(new { Message = "Đã ghi nhận dữ liệu cảm biến cân nặng." });
    }
}

public class WeightLogDto
{
    public string SlotNumber { get; set; } = "";
    public double Weight { get; set; }
}
