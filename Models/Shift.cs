namespace MorgueManager.Web.Models;

public enum ShiftType { Morning, Afternoon, Night }
public enum ShiftStatus { Confirmed, Pending, Absent }

public class Shift
{
    public int Id { get; set; }
    public string Date { get; set; } = "";
    public ShiftType Type { get; set; }
    public List<StaffInfo> Staff { get; set; } = new();
    public ShiftStatus Status { get; set; }
    public string Notes { get; set; } = "";
}

public class StaffInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Avatar { get; set; }
}
