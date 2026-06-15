using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MorgueManager.API.Models;
using MorgueManager.API.Exceptions;
using MorgueManager.API.Data;
using MorgueManager.API.Dtos;
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
    public IActionResult Create([FromBody] CreateCorpseDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            errors.Add("name", new[] { "Họ và tên không được để trống." });
        }
        if (string.IsNullOrWhiteSpace(dto.Cccd))
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

        var corpse = new Corpse
        {
            Name = dto.Name,
            Cccd = dto.Cccd,
            Gender = dto.Gender,
            BirthDate = dto.BirthDate,
            Age = dto.Age,
            CauseOfDeath = dto.CauseOfDeath,
            DateOfDeath = dto.DateOfDeath,
            Status = dto.Status,
            StorageUnit = dto.StorageUnit,
            StorageSlot = dto.StorageSlot,
            Temp = dto.Temp,
            Notes = dto.Notes,
            NextOfKin = new NextOfKinInfo
            {
                Name = dto.NextOfKin?.Name ?? "",
                Phone = dto.NextOfKin?.Phone ?? "",
                Relationship = dto.NextOfKin?.Relationship ?? ""
            },
            CaseId = $"MC-2026-{nextId:D4}",
            DateAdmitted = DateTime.Now.ToString("yyyy-MM-dd"),
            DaysStored = 1,
            Priority = "NORMAL"
        };

        _context.Corpses.Add(corpse);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = corpse.Id }, corpse);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Update(int id, [FromBody] UpdateCorpseDto dto)
    {
        var corpse = _context.Corpses
            .Include(c => c.NextOfKin)
            .FirstOrDefault(c => c.Id == id);

        if (corpse == null)
        {
            throw new ResourceNotFoundException($"Không tìm thấy thi thể có ID = {id} để cập nhật.");
        }

        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            errors.Add("name", new[] { "Họ và tên không được để trống." });
        }

        if (errors.Any())
        {
            throw new AppValidationException("Thông tin thi thể gửi lên không hợp lệ.", errors);
        }

        corpse.Name = dto.Name;
        corpse.Cccd = dto.Cccd;
        corpse.Gender = dto.Gender;
        corpse.BirthDate = dto.BirthDate;
        corpse.Age = dto.Age;
        corpse.CauseOfDeath = dto.CauseOfDeath;
        corpse.DateOfDeath = dto.DateOfDeath;
        corpse.Status = dto.Status;
        corpse.StorageUnit = dto.StorageUnit;
        corpse.StorageSlot = dto.StorageSlot;
        corpse.Temp = dto.Temp;
        corpse.Notes = dto.Notes;
        if (dto.NextOfKin != null)
        {
            corpse.NextOfKin.Name = dto.NextOfKin.Name;
            corpse.NextOfKin.Phone = dto.NextOfKin.Phone;
            corpse.NextOfKin.Relationship = dto.NextOfKin.Relationship;
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
