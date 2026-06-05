namespace MorgueManager.Web.Models;

public class DashboardStats
{
    public int TotalCorpses { get; set; }
    public int InStorage { get; set; }
    public int HandedOver { get; set; }
    public int Overdue { get; set; }
    public int Warning { get; set; }
    public double AvgTemp { get; set; }
    public int ActiveShifts { get; set; }
}
