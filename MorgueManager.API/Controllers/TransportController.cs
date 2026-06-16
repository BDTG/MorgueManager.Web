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
[Route("api/transport")]
[Authorize]
public class TransportController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransportController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("vehicles")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public async Task<IActionResult> GetVehicles()
    {
        var vehicles = await _context.TransportVehicles.ToListAsync();
        return Ok(vehicles);
    }

    [HttpGet("trips")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public async Task<IActionResult> GetTrips()
    {
        var trips = await _context.TransportTrips.ToListAsync();
        return Ok(trips);
    }

    [HttpPost("trips")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateTrip([FromBody] CreateTripDto dto)
    {
        var vehicle = await _context.TransportVehicles.FindAsync(dto.VehicleId);
        if (vehicle == null)
        {
            return NotFound(new { Message = "Không tìm thấy xe vận chuyển." });
        }

        if (vehicle.Status != VehicleStatus.Available)
        {
            return BadRequest(new { Message = "Xe vận chuyển này hiện đang bận hoặc đang bảo trì." });
        }

        var corpse = await _context.Corpses.FindAsync(dto.CorpseId);
        if (corpse == null)
        {
            return NotFound(new { Message = "Không tìm thấy hồ sơ thi hài." });
        }

        var trip = new TransportTrip
        {
            CorpseId = dto.CorpseId,
            VehicleId = dto.VehicleId,
            DriverName = dto.DriverName,
            Destination = dto.Destination,
            DepartureTime = DateTime.Now,
            Status = TripStatus.Ongoing
        };

        vehicle.Status = VehicleStatus.InTransit;
        _context.TransportTrips.Add(trip);
        await _context.SaveChangesAsync();

        return Ok(trip);
    }

    [HttpPost("trips/{id:int}/complete")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CompleteTrip(int id)
    {
        var trip = await _context.TransportTrips.FindAsync(id);
        if (trip == null)
        {
            return NotFound(new { Message = "Không tìm thấy chuyến đi này." });
        }

        if (trip.Status == TripStatus.Completed)
        {
            return BadRequest(new { Message = "Chuyến đi này đã được hoàn thành trước đó." });
        }

        var vehicle = await _context.TransportVehicles.FindAsync(trip.VehicleId);
        if (vehicle != null)
        {
            vehicle.Status = VehicleStatus.Available;
        }

        trip.ArrivalTime = DateTime.Now;
        trip.Status = TripStatus.Completed;
        await _context.SaveChangesAsync();

        return Ok(trip);
    }
}

public class CreateTripDto
{
    public int CorpseId { get; set; }
    public int VehicleId { get; set; }
    public string DriverName { get; set; } = "";
    public string Destination { get; set; } = "";
}
