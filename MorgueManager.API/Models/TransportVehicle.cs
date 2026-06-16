namespace MorgueManager.API.Models;

public enum VehicleStatus
{
    Available,
    InTransit,
    Maintenance
}

public class TransportVehicle
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = "";
    public string Model { get; set; } = "";
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;
}
