using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MorgueManager.API.Data;
using MorgueManager.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MorgueManager.API.Controllers;

[ApiController]
[Route("api/billing")]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly AppDbContext _context;

    public BillingController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public async Task<IActionResult> GetAll()
    {
        var bills = await _context.BillingRecords.ToListAsync();
        foreach (var bill in bills)
        {
            await UpdateBillAmountAsync(bill);
        }
        return Ok(bills);
    }

    [HttpGet("corpse/{corpseId:int}")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public async Task<IActionResult> GetByCorpseId(int corpseId)
    {
        var bill = await _context.BillingRecords.FirstOrDefaultAsync(b => b.CorpseId == corpseId);
        if (bill == null)
        {
            return NotFound(new { Message = "Không tìm thấy hóa đơn cho thi hài này." });
        }
        await UpdateBillAmountAsync(bill);
        return Ok(bill);
    }

    [HttpPost("{id:int}/pay")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> PayBill(int id)
    {
        var bill = await _context.BillingRecords.FindAsync(id);
        if (bill == null)
        {
            return NotFound(new { Message = "Không tìm thấy hóa đơn này." });
        }

        await UpdateBillAmountAsync(bill);
        bill.IsPaid = true;
        await _context.SaveChangesAsync();

        return Ok(bill);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateBill(int id, [FromBody] UpdateBillDto dto)
    {
        var bill = await _context.BillingRecords.FindAsync(id);
        if (bill == null)
        {
            return NotFound(new { Message = "Không tìm thấy hóa đơn này." });
        }

        bill.ServiceFee = dto.ServiceFee;
        bill.StorageFeePerDay = dto.StorageFeePerDay;
        await UpdateBillAmountAsync(bill);
        await _context.SaveChangesAsync();

        return Ok(bill);
    }

    private async Task UpdateBillAmountAsync(BillingRecord bill)
    {
        var corpse = await _context.Corpses.FindAsync(bill.CorpseId);
        if (corpse != null)
        {
            int days = corpse.DaysStored;
            if (DateTime.TryParse(corpse.DateAdmitted, out var admittedDate))
            {
                var duration = (int)(DateTime.Today - admittedDate.Date).TotalDays;
                days = duration > 0 ? duration : 1;
                corpse.DaysStored = days; // Sync back to corpse as well
            }
            bill.TotalAmount = (bill.StorageFeePerDay * days) + bill.ServiceFee;
        }
    }
}

public class UpdateBillDto
{
    public double StorageFeePerDay { get; set; }
    public double ServiceFee { get; set; }
}
