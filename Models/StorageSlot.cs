namespace MorgueManager.Web.Models;

public enum SlotStatus { Empty, Occupied, Cleaning, Maintenance }

public class StorageSlot
{
    public string Id { get; set; } = "";
    public string Zone { get; set; } = "";
    public SlotStatus Status { get; set; }
    public double Temp { get; set; }
    public string? CorpseName { get; set; }
    public int DaysStored { get; set; }
    public string LastUpdate { get; set; } = "";
}
