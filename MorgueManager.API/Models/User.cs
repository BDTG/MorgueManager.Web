using System;

namespace MorgueManager.API.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Role { get; set; } = "Staff"; // Admin, Manager, Staff
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
