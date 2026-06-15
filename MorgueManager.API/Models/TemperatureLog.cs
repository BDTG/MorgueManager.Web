using System;

namespace MorgueManager.API.Models;

public class TemperatureLog
{
    public int Id { get; set; }
    public int StorageSlotId { get; set; }
    public double Temperature { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
