namespace MorgueManager.API.Models;

public enum SlotStatus
{
    Empty,
    Occupied,
    Cleaning,
    Maintenance
}

public class StorageSlot
{
    public int Id { get; set; }
    public string SlotNumber { get; set; } = ""; // e.g. A-01, A-02, B-01
    public string UnitName { get; set; } = ""; // e.g. Cold Room A, Cold Room B
    public SlotStatus Status { get; set; } = SlotStatus.Empty;
    public double CurrentTemperature { get; set; } = 4.0;
}
