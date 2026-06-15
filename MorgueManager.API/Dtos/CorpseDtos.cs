using System;

namespace MorgueManager.API.Dtos;

public class NextOfKinDto
{
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Relationship { get; set; } = "";
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
    public NextOfKinDto NextOfKin { get; set; } = new();
}
