using System;

namespace MorgueManager.API.Models;

public enum TripStatus
{
    Scheduled,
    Ongoing,
    Completed
}

public class TransportTrip
{
    public int Id { get; set; }
    public int CorpseId { get; set; }
    public int VehicleId { get; set; }
    public string DriverName { get; set; } = "";
    public string Destination { get; set; } = "";
    public DateTime DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public TripStatus Status { get; set; } = TripStatus.Scheduled;
}
