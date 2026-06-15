using System;

namespace MorgueManager.API.Models;

public class AutopsyReport
{
    public string PathologistName { get; set; } = "";
    public string ConcludingCause { get; set; } = "";
    public string ToxicologyResult { get; set; } = "";
    public string InternalExamDetails { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
