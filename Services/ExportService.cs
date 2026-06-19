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

    public async Task ExportStorageSlotsToExcelAsync(List<StorageSlot> slots)
    {
        await EnsureScriptsLoadedAsync();

        var exportData = slots.Select(s => new
        {
            Id = s.Id,
            Zone = s.Zone,
            Status = s.Status.ToString() == "Occupied" ? "Đang lưu" : s.Status.ToString() == "Cleaning" ? "Chờ vệ sinh" : s.Status.ToString() == "Maintenance" ? "Bảo trì" : "Trống",
            Temp = $"{s.Temp}°C",
            CorpseName = s.CorpseName ?? "—",
            DaysStored = s.DaysStored,
            LastUpdate = s.LastUpdate
        }).ToList();

        string jsonData = JsonSerializer.Serialize(exportData);
        await _jsRuntime.InvokeVoidAsync("window.ExportHelper.exportStorageSlotsToExcel", jsonData);
    }

    public async Task ExportStorageSlotsToPdfAsync(List<StorageSlot> slots)
    {
        await EnsureScriptsLoadedAsync();

        var exportData = slots.Select(s => new
        {
            Id = s.Id,
            Zone = s.Zone,
            Status = s.Status.ToString() == "Occupied" ? "Đang lưu" : s.Status.ToString() == "Cleaning" ? "Chờ vệ sinh" : s.Status.ToString() == "Maintenance" ? "Bảo trì" : "Trống",
            Temp = $"{s.Temp}°C",
            CorpseName = s.CorpseName ?? "—",
            DaysStored = $"{s.DaysStored} ngày",
            LastUpdate = s.LastUpdate
        }).ToList();

        string jsonData = JsonSerializer.Serialize(exportData);
        await _jsRuntime.InvokeVoidAsync("window.ExportHelper.exportStorageSlotsToPdf", jsonData);
    }

    public async Task ExportShiftsToExcelAsync(List<Shift> shifts)
    {
        await EnsureScriptsLoadedAsync();

        var exportData = shifts.Select(s => new
        {
            Date = s.Date,
            Type = s.Type.ToString() == "Morning" ? "Sáng" : s.Type.ToString() == "Afternoon" ? "Chiều" : "Đêm",
            Staff = string.Join(", ", s.Staff.Select(st => $"{st.Name} ({st.Role})")),
            Phone = string.Join(", ", s.Staff.Select(st => st.Phone)),
            Status = s.Status.ToString() == "Confirmed" ? "Đã xác nhận" : s.Status.ToString() == "Pending" ? "Chờ xác nhận" : "Vắng mặt",
            Notes = s.Notes
        }).ToList();

        string jsonData = JsonSerializer.Serialize(exportData);
        await _jsRuntime.InvokeVoidAsync("window.ExportHelper.exportShiftsToExcel", jsonData);
    }

    public async Task ExportShiftsToPdfAsync(List<Shift> shifts)
    {
        await EnsureScriptsLoadedAsync();

        var exportData = shifts.Select(s => new
        {
            Date = s.Date,
            Type = s.Type.ToString() == "Morning" ? "Ca Sáng" : s.Type.ToString() == "Afternoon" ? "Ca Chiều" : "Ca Đêm",
            Staff = string.Join(", ", s.Staff.Select(st => $"{st.Name} ({st.Role})")),
            Phone = string.Join(", ", s.Staff.Select(st => st.Phone)),
            Status = s.Status.ToString() == "Confirmed" ? "Đã xác nhận" : s.Status.ToString() == "Pending" ? "Chờ xác nhận" : "Vắng mặt",
            Notes = s.Notes
        }).ToList();

        string jsonData = JsonSerializer.Serialize(exportData);
        await _jsRuntime.InvokeVoidAsync("window.ExportHelper.exportShiftsToPdf", jsonData);
    }

    public async Task ExportSummaryToExcelAsync(List<Corpse> corpses, List<StorageSlot> slots, List<Shift> shifts)
    {
        await EnsureScriptsLoadedAsync();

        var corpseData = corpses.Select(c => new
        {
            CaseId = c.CaseId, Name = c.Name, Gender = c.Gender, Age = c.Age,
            CauseOfDeath = c.CauseOfDeath, Status = c.Status, Storage = c.StorageSlot ?? "—", Temp = c.Temp.HasValue ? $"{c.Temp.Value}°C" : "—",
            DateAdmitted = c.DateAdmitted
        }).ToList();

        var slotData = slots.Select(s => new
        {
            Id = s.Id, Zone = s.Zone, Status = s.Status.ToString(), Temp = $"{s.Temp}°C",
            CorpseName = s.CorpseName ?? "—", DaysStored = s.DaysStored
        }).ToList();

        var shiftData = shifts.Select(s => new
        {
            Date = s.Date, Type = s.Type.ToString(),
            Staff = string.Join(", ", s.Staff.Select(st => st.Name)),
            Status = s.Status.ToString(), Notes = s.Notes
        }).ToList();

        var summaryPayload = new
        {
            Corpses = corpseData,
            Slots = slotData,
            Shifts = shiftData,
            Stats = new
            {
                TotalCorpses = corpses.Count,
                ActiveCorpses = corpses.Count(c => c.Status != "Bàn giao"),
                OccupiedSlots = slots.Count(s => s.Status == SlotStatus.Occupied),
                TotalSlots = slots.Count,
                OccupancyRate = slots.Count > 0 ? Math.Round((double)slots.Count(s => s.Status == SlotStatus.Occupied) / slots.Count * 100, 1) : 0
            }
        };

        string jsonData = JsonSerializer.Serialize(summaryPayload);
        await _jsRuntime.InvokeVoidAsync("window.ExportHelper.exportSummaryToExcel", jsonData);
    }

    public async Task ExportSummaryToPdfAsync(List<Corpse> corpses, List<StorageSlot> slots, List<Shift> shifts)
    {
        await EnsureScriptsLoadedAsync();

        var corpseData = corpses.Select(c => new
        {
            CaseId = c.CaseId, Name = c.Name, Status = c.Status, Storage = c.StorageSlot ?? "—", DateAdmitted = c.DateAdmitted
        }).Take(15).ToList();

        var slotStats = new
        {
            Total = slots.Count,
            Occupied = slots.Count(s => s.Status == SlotStatus.Occupied),
            Cleaning = slots.Count(s => s.Status == SlotStatus.Cleaning),
            Maint = slots.Count(s => s.Status == SlotStatus.Maintenance),
            Empty = slots.Count(s => s.Status == SlotStatus.Empty),
            OccupancyRate = slots.Count > 0 ? Math.Round((double)slots.Count(s => s.Status == SlotStatus.Occupied) / slots.Count * 100, 1) : 0
        };

        var payload = new
        {
            Corpses = corpseData,
            SlotStats = slotStats,
            TotalCorpses = corpses.Count,
            ActiveCorpses = corpses.Count(c => c.Status != "Bàn giao"),
            TotalShifts = shifts.Count
        };

        string jsonData = JsonSerializer.Serialize(payload);
        await _jsRuntime.InvokeVoidAsync("window.ExportHelper.exportSummaryToPdf", jsonData);
    }
}
