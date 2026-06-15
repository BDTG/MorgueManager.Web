using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MorgueManager.API.Data;
using MorgueManager.API.Models;
using MorgueManager.API.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MorgueManager.API.Controllers;

[ApiController]
[Route("api/corpses/{id:int}/documents")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public FilesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UploadDocument(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { Message = "Không có tệp tin nào được gửi lên." });
        }

        var corpse = _context.Corpses
            .Include(c => c.Documents)
            .FirstOrDefault(c => c.Id == id);

        if (corpse == null)
        {
            throw new ResourceNotFoundException($"Không tìm thấy thi thể có ID = {id} để đính kèm tài liệu.");
        }

        try
        {
            // Ensure uploads directory exists
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file to disk
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Record document in database
            var document = new Document
            {
                Name = file.FileName,
                Size = $"{(file.Length / 1024.0):F1} KB",
                Type = file.ContentType
            };

            corpse.Documents.Add(document);
            await _context.SaveChangesAsync();

            var fileUrl = $"/uploads/{uniqueFileName}";

            return Ok(new
            {
                Message = "Tải lên tài liệu thành công!",
                Document = document,
                Url = fileUrl
            });
        }
        catch (Exception ex)
        {
            return Problem($"Lỗi máy chủ khi tải lên tệp tin: {ex.Message}");
        }
    }
}
