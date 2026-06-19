using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MorgueManager.API.Data;
using MorgueManager.API.Models;

namespace MorgueManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShiftsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetShifts([FromQuery] int month, [FromQuery] int year)
    {
        if (month == 0 || year == 0)
        {
            month = DateTime.Now.Month;
            year = DateTime.Now.Year;
        }

        var shifts = await _context.Shifts
            .Where(s => s.Date.Month == month && s.Date.Year == year)
            .Select(s => new {
                s.Id,
                s.Date,
                ShiftType = s.ShiftType.ToLower(),
                StaffName = s.StaffEmail.Split('@')[0],
                s.StaffEmail,
                Phone = s.Notes // Map phone to notes for now
            })
            .ToListAsync();
            
        return Ok(shifts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateShift([FromBody] ShiftDto dto)
    {
        var shift = new Shift
        {
            Date = dto.Date,
            ShiftType = dto.ShiftType,
            StaffEmail = dto.StaffName + "@hospital.vn", // fake email
            Notes = dto.Phone ?? "+84 999 888 777"
        };

        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetShifts), new { id = shift.Id }, new {
            shift.Id,
            shift.Date,
            ShiftType = shift.ShiftType.ToLower(),
            StaffName = shift.StaffEmail.Split('@')[0],
            shift.StaffEmail,
            Phone = shift.Notes
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateShift(int id, [FromBody] ShiftDto dto)
    {
        var existing = await _context.Shifts.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Date = dto.Date;
        existing.ShiftType = dto.ShiftType;
        existing.StaffEmail = dto.StaffName + "@hospital.vn";
        existing.Notes = dto.Phone ?? existing.Notes;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShift(int id)
    {
        var existing = await _context.Shifts.FindAsync(id);
        if (existing == null) return NotFound();

        _context.Shifts.Remove(existing);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class ShiftDto
{
    public DateTime Date { get; set; }
    public string ShiftType { get; set; } = "";
    public string StaffName { get; set; } = "";
    public string Phone { get; set; } = "";
}
