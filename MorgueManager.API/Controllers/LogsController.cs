using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MorgueManager.API.Controllers;

[ApiController]
[Route("api/logs")]
[AllowAnonymous] // Anyone can view logs during local testing
public class LogsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetLogs()
    {
        try
        {
            var logDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logDir))
            {
                return Ok(new { logs = "[INFO] Hệ thống mới khởi động, chưa ghi nhận nhật ký nào.\n" });
            }

            var logFile = Directory.GetFiles(logDir, "app-log*.txt")
                                   .OrderByDescending(System.IO.File.GetLastWriteTime)
                                   .FirstOrDefault();

            if (logFile == null)
            {
                return Ok(new { logs = "[INFO] Chưa tìm thấy tệp nhật ký nào được ghi.\n" });
            }

            using var fs = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(fs);
            var lines = new List<string>();
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }

            // Return the last 100 lines
            return Ok(new { logs = string.Join("\n", lines.TakeLast(100)) });
        }
        catch (Exception ex)
        {
            return Problem($"Lỗi khi đọc nhật ký hệ thống: {ex.Message}");
        }
    }
}
