using System.Collections.Generic;

namespace MorgueManager.Web.Pages.Admin;

public enum BiohazardLevel { None, Infectious, HighRisk }

public class NextOfKinDto
{
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Relationship { get; set; } = "";
}

public class CorpseApiDto
{
    public int Id { get; set; }
    public string CaseId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Cccd { get; set; } = "";
    public string Gender { get; set; } = "Nam";
    public string BirthDate { get; set; } = "";
    public int Age { get; set; }
    public string CauseOfDeath { get; set; } = "";
    public string DateOfDeath { get; set; } = "";
    public string DateAdmitted { get; set; } = "";
    public string Status { get; set; } = "Tiếp nhận";
    public string? StorageUnit { get; set; }
    public string? StorageSlot { get; set; }
    public double? Temp { get; set; }
    public int DaysStored { get; set; }
    public string Priority { get; set; } = "NORMAL";
    public BiohazardLevel Biohazard { get; set; } = BiohazardLevel.None;
    public NextOfKinDto NextOfKin { get; set; } = new();
    public string Notes { get; set; } = "";
    public List<MorgueManager.Web.Models.Belonging> Belongings { get; set; } = new();
    public List<MorgueManager.Web.Models.HistoryEntry> History { get; set; } = new();
    public List<MorgueManager.Web.Models.Document> Documents { get; set; } = new();
}

public class CreateCorpseDto
{
    public string Name { get; set; } = "";
    public string Cccd { get; set; } = "";
    public string Gender { get; set; } = "Nam";
    public string BirthDate { get; set; } = "";
    public int Age { get; set; }
    public string CauseOfDeath { get; set; } = "";
    public string DateOfDeath { get; set; } = "";
    public string Status { get; set; } = "Tiếp nhận";
    public string? StorageUnit { get; set; }
    public string? StorageSlot { get; set; }
    public double? Temp { get; set; }
    public string Notes { get; set; } = "";
    public BiohazardLevel Biohazard { get; set; } = BiohazardLevel.None;
    public NextOfKinDto NextOfKin { get; set; } = new();
}

public class UpdateCorpseDto
{
    public string Name { get; set; } = "";
    public string Cccd { get; set; } = "";
    public string Gender { get; set; } = "Nam";
    public string BirthDate { get; set; } = "";
    public int Age { get; set; }
    public string CauseOfDeath { get; set; } = "";
    public string DateOfDeath { get; set; } = "";
    public string Status { get; set; } = "Tiếp nhận";
    public string? StorageUnit { get; set; }
    public string? StorageSlot { get; set; }
    public double? Temp { get; set; }
    public string Notes { get; set; } = "";
    public BiohazardLevel Biohazard { get; set; } = BiohazardLevel.None;
    public NextOfKinDto NextOfKin { get; set; } = new();
}
