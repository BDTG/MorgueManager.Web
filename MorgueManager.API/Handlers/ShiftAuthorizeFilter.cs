using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MorgueManager.API.Data;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MorgueManager.API.Handlers;

public class ShiftAuthorizeFilter : IAsyncActionFilter
{
    private readonly AppDbContext _context;

    public ShiftAuthorizeFilter(AppDbContext context)
    {
        _context = context;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        
        // Admins are bypass authorized
        if (user.IsInRole("Admin"))
        {
            await next();
            return;
        }

        var userEmail = user.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            context.Result = new UnauthorizedObjectResult(new { Message = "Yêu cầu đăng nhập để thực hiện thao tác này." });
            return;
        }

        var shiftInfo = GetCurrentShiftInfo();
        
        // Check if the user is scheduled for the current active shift
        var isOnShift = await _context.Shifts.AnyAsync(s => 
            s.StaffEmail == userEmail && 
            s.Date == shiftInfo.Date && 
            s.ShiftType == shiftInfo.ShiftType);

        if (!isOnShift)
        {
            context.Result = new ObjectResult(new { Message = $"Từ chối truy cập: Bạn không được phân công trực trong ca hiện tại ({shiftInfo.ShiftType} ngày {shiftInfo.Date:dd/MM/yyyy})." })
            {
                StatusCode = 403
            };
            return;
        }

        await next();
    }

    private (DateTime Date, string ShiftType) GetCurrentShiftInfo()
    {
        var now = DateTime.Now;
        var date = now.Date;
        string shiftType;

        if (now.Hour >= 6 && now.Hour < 14)
        {
            shiftType = "Morning";
        }
        else if (now.Hour >= 14 && now.Hour < 22)
        {
            shiftType = "Afternoon";
        }
        else
        {
            shiftType = "Night";
            if (now.Hour < 6)
            {
                date = date.AddDays(-1);
            }
        }
        // Force UTC kind to match seeded shifts
        date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        return (date, shiftType);
    }
}
