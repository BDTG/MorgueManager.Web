using Newtonsoft.Json;
using Postgrest.Attributes;
using Postgrest.Models;
using System.Collections.Generic;

namespace MorgueManager.Web.Models;

[Table("corpses")]
public class Corpse : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("case_id")]
    public string CaseId { get; set; } = "";

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("cccd")]
    public string Cccd { get; set; } = "";

    [Column("gender")]
    public string Gender { get; set; } = "Nam";

    [Column("birth_date")]
    public string BirthDate { get; set; } = "";

    [Column("age")]
    public int Age { get; set; }

    [Column("cause_of_death")]
    public string CauseOfDeath { get; set; } = "";

    [Column("date_of_death")]
    public string DateOfDeath { get; set; } = "";

    [Column("date_admitted")]
    public string DateAdmitted { get; set; } = "";

    [Column("status")]
    public string Status { get; set; } = "Tiếp nhận";

    [Column("storage_unit")]
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public string? StorageUnit { get; set; }

    [Column("storage_slot")]
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public string? StorageSlot { get; set; }

    [Column("temp")]
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public double? Temp { get; set; }

    [Column("days_stored")]
    public int DaysStored { get; set; }

    [Column("priority")]
    public string Priority { get; set; } = "NORMAL";

    [Column("next_of_kin")]
    public NextOfKinInfo NextOfKin { get; set; } = new();

    [Column("notes")]
    public string Notes { get; set; } = "";

    [Column("is_unidentified")]
    public bool IsUnidentified { get; set; }

    [Column("is_infectious")]
    public bool IsInfectious { get; set; }

    [Column("infectious_group")]
    public string InfectiousGroup { get; set; } = "";

    [Column("death_certificate_id")]
    public string DeathCertificateId { get; set; } = "";

    [Column("requires_deep_freeze")]
    public bool RequiresDeepFreeze { get; set; }

    [Column("belongings")]
    public List<Belonging> Belongings { get; set; } = new();

    [Column("history")]
    public List<HistoryEntry> History { get; set; } = new();

    [Column("documents")]
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
