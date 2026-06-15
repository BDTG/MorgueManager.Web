using System;

namespace MorgueManager.API.Models;

public class AuditLog
{
    public int Id { get; set; }
    public string UserEmail { get; set; } = "";
    public string Action { get; set; } = ""; // CREATE, UPDATE, DELETE
    public string EntityName { get; set; } = "";
    public string EntityId { get; set; } = "";
    public string Details { get; set; } = ""; // details of changed properties
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
