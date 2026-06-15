using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MorgueManager.API.Models;
using MorgueManager.API.Exceptions;
using MorgueManager.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MorgueManager.API.Controllers;

[ApiController]
[Route("api/corpses")]
[Authorize]
public class CorpsesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CorpsesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public IActionResult GetAll()
    {
        var corpses = _context.Corpses
            .Include(c => c.NextOfKin)
            .Include(c => c.Belongings)
            .Include(c => c.History)
            .Include(c => c.Documents)
            .ToList();
        return Ok(corpses);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public IActionResult GetById(int id)
    {
        var corpse = _context.Corpses
            .Include(c => c.NextOfKin)
            .Include(c => c.Belongings)
            .Include(c => c.History)
            .Include(c => c.Documents)
            .FirstOrDefault(c => c.Id == id);
            
        if (corpse == null)
        {
            throw new ResourceNotFoundException($"Không tìm thấy thi thể có ID = {id} trong hệ thống.");
        }
        return Ok(corpse);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Create([FromBody] Corpse corpse)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(corpse.Name))
        {
            errors.Add("name", new[] { "Họ và tên không được để trống." });
        }
        if (string.IsNullOrWhiteSpace(corpse.Cccd))
        {
            errors.Add("cccd", new[] { "Số CCCD/CMND không được để trống." });
        }

        if (errors.Any())
        {
            throw new AppValidationException("Thông tin thi thể gửi lên không hợp lệ.", errors);
        }

        // Generate Case ID
        int nextId = 1;
        if (_context.Corpses.Any())
        {
            nextId = _context.Corpses.Max(c => c.Id) + 1;
        }
        corpse.CaseId = $"MC-2026-{nextId:D4}";
        corpse.DateAdmitted = DateTime.Now.ToString("yyyy-MM-dd");
        corpse.DaysStored = 1;
        corpse.Priority = "NORMAL";

        _context.Corpses.Add(corpse);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = corpse.Id }, corpse);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Update(int id, [FromBody] Corpse updatedCorpse)
    {
        var corpse = _context.Corpses
            .Include(c => c.NextOfKin)
            .Include(c => c.Belongings)
            .Include(c => c.History)
            .Include(c => c.Documents)
            .FirstOrDefault(c => c.Id == id);

        if (corpse == null)
        {
            throw new ResourceNotFoundException($"Không tìm thấy thi thể có ID = {id} để cập nhật.");
        }

        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(updatedCorpse.Name))
        {
            errors.Add("name", new[] { "Họ và tên không được để trống." });
        }

        if (errors.Any())
        {
            throw new AppValidationException("Thông tin thi thể gửi lên không hợp lệ.", errors);
        }

        corpse.Name = updatedCorpse.Name;
        corpse.Cccd = updatedCorpse.Cccd;
        corpse.Gender = updatedCorpse.Gender;
        corpse.BirthDate = updatedCorpse.BirthDate;
        corpse.Age = updatedCorpse.Age;
        corpse.CauseOfDeath = updatedCorpse.CauseOfDeath;
        corpse.DateOfDeath = updatedCorpse.DateOfDeath;
        corpse.Status = updatedCorpse.Status;
        corpse.StorageUnit = updatedCorpse.StorageUnit;
        corpse.StorageSlot = updatedCorpse.StorageSlot;
        corpse.Temp = updatedCorpse.Temp;
        corpse.Notes = updatedCorpse.Notes;
        if (updatedCorpse.NextOfKin != null)
        {
            corpse.NextOfKin = updatedCorpse.NextOfKin;
        }

        // Replace belongings list
        if (updatedCorpse.Belongings != null)
        {
            corpse.Belongings.Clear();
            corpse.Belongings.AddRange(updatedCorpse.Belongings);
        }

        // Replace history list
        if (updatedCorpse.History != null)
        {
            corpse.History.Clear();
            corpse.History.AddRange(updatedCorpse.History);
        }

        // Replace documents list
        if (updatedCorpse.Documents != null)
        {
            corpse.Documents.Clear();
            corpse.Documents.AddRange(updatedCorpse.Documents);
        }

        _context.SaveChanges();
        return Ok(corpse);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var corpse = _context.Corpses.FirstOrDefault(c => c.Id == id);
        if (corpse == null)
        {
            throw new ResourceNotFoundException($"Không tìm thấy thi thể có ID = {id} để xóa.");
        }

        _context.Corpses.Remove(corpse);
        _context.SaveChanges();

        return Ok(new { Message = $"Đã xóa thi thể ID = {id} thành công!" });
    }

    [HttpGet("test-error")]
    [AllowAnonymous]
    public IActionResult TestError()
    {
        throw new DivideByZeroException("Lỗi thử nghiệm hệ thống: Chia cho 0.");
    }
}
