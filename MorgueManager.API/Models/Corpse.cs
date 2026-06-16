using System.Collections.Generic;

namespace MorgueManager.API.Models;

public enum BiohazardLevel { None, Infectious, HighRisk }

public class Corpse
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
    public int? StorageSlotId { get; set; }
    public double? Temp { get; set; }
    public int DaysStored { get; set; }
    public string Priority { get; set; } = "NORMAL";
    public BiohazardLevel Biohazard { get; set; } = BiohazardLevel.None;
    public NextOfKinInfo NextOfKin { get; set; } = new();
    public string Notes { get; set; } = "";
    public AutopsyReport? AutopsyReport { get; set; }
    public EmbalmingInfo EmbalmingInfo { get; set; } = new();
    public List<Belonging> Belongings { get; set; } = new();
    public List<HistoryEntry> History { get; set; } = new();
    public List<Document> Documents { get; set; } = new();
}

public class NextOfKinInfo
{
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Relationship { get; set; } = "";
}

public class EmbalmingInfo
{
    public bool IsEmbalmed { get; set; } = false;
    public string EmbalmerName { get; set; } = "";
    public string ChemicalsUsed { get; set; } = "";
    public string DateEmbalmed { get; set; } = "";
}

public class Belonging
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Desc { get; set; } = "";
    public string Condition { get; set; } = "";
    public bool HasImage { get; set; }
}

public class HistoryEntry
{
    public string Status { get; set; } = "";
    public string Timestamp { get; set; } = "";
    public string By { get; set; } = "";
    public string Detail { get; set; } = "";
}

public class Document
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Size { get; set; } = "";
    public string Type { get; set; } = "";
}
