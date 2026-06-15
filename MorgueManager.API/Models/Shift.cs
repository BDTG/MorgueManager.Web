using System;

namespace MorgueManager.API.Models;

public class Shift
{
    public int Id { get; set; }
    public string StaffEmail { get; set; } = "";
    public DateTime Date { get; set; }
    public string ShiftType { get; set; } = "Morning"; // Morning, Afternoon, Night
    public string Notes { get; set; } = "";
}
