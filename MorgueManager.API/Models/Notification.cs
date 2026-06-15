using System;

namespace MorgueManager.API.Models;

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public bool IsRead { get; set; } = false;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
