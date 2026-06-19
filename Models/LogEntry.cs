namespace MorgueManager.Web.Models;

public class LogEntry
{
    public int Id { get; set; }
    public string Time { get; set; } = "";
    public string User { get; set; } = "";
    public string Action { get; set; } = "";
    public string Detail { get; set; } = "";
    public string BadgeBg { get; set; } = "";
    public string BadgeText { get; set; } = "";
    public string Link { get; set; } = "";
    public string LinkText { get; set; } = "";
}
