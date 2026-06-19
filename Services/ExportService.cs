using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MorgueManager.Web.Models;

namespace MorgueManager.Web.Services;

public class ExportService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _scriptsLoaded = false;

    public ExportService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async Task EnsureScriptsLoadedAsync()
    {
        if (_scriptsLoaded) return;

        bool loaded = await _jsRuntime.InvokeAsync<bool>("window.ExportHelper.initExportLibraries");
        if (loaded)
        {
            _scriptsLoaded = true;
        }
        else
        {
            throw new Exception("Không thể tải các thư viện xuất dữ liệu từ CDN.");
        }
    }

    public async Task ExportToExcelAsync(List<Corpse> corpses)
    {
        await EnsureScriptsLoadedAsync();

        var exportData = corpses.Select(c => new
        {
            CaseId = c.CaseId,
            Name = c.Name,
            Cccd = c.Cccd,
            Gender = c.Gender,
            Age = c.Age,
            CauseOfDeath = c.CauseOfDeath,
            Status = c.Status,
            StorageUnit = c.StorageUnit ?? "—",
            StorageSlot = c.StorageSlot ?? "—",
            Temp = c.Temp.HasValue ? $"{c.Temp.Value}°C" : "—",
            DaysStored = c.DaysStored,
            DateAdmitted = c.DateAdmitted,
            DateOfDeath = c.DateOfDeath,
            NokName = c.NextOfKin?.Name ?? "—",
            NokPhone = c.NextOfKin?.Phone ?? "—",
            NokRelationship = c.NextOfKin?.Relationship ?? "—",
            Notes = c.Notes ?? ""
        }).ToList();

        string jsonData = JsonSerializer.Serialize(exportData);
        await _jsRuntime.InvokeVoidAsync("window.ExportHelper.exportToExcel", jsonData);
    }

    public async Task ExportToPdfAsync(List<Corpse> corpses)
    {
        await EnsureScriptsLoadedAsync();

        var exportData = corpses.Select(c => new
        {
            CaseId = c.CaseId,
            Name = c.Name,
            Gender = c.Gender,
            Age = c.Age,
            CauseOfDeath = c.CauseOfDeath,
            Status = c.Status,
            Storage = c.StorageSlot != null ? $"{c.StorageUnit} / {c.StorageSlot}" : "—",
            Temp = c.Temp.HasValue ? $"{c.Temp.Value}°C" : "—",
            DateAdmitted = c.DateAdmitted
        }).ToList();

        string jsonData = JsonSerializer.Serialize(exportData);
        await _jsRuntime.InvokeVoidAsync("window.ExportHelper.exportToPdf", jsonData);
    }
}
