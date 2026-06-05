namespace MorgueManager.Web.Models;

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
    public double? Temp { get; set; }
    public int DaysStored { get; set; }
    public string Priority { get; set; } = "NORMAL";
    public NextOfKinInfo NextOfKin { get; set; } = new();
    public string Notes { get; set; } = "";
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
