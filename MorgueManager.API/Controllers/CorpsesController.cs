using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MorgueManager.API.Models;
using MorgueManager.API.Exceptions;
using MorgueManager.API.Data;
using MorgueManager.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MorgueManager.API.Controllers;

[ApiController]
[Route("api/corpses")]
[Authorize]
public class CorpsesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CorpsesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public IActionResult GetAll()
    {
        var corpses = _context.Corpses
            .Include(c => c.NextOfKin)
            .Include(c => c.Belongings)
            .Include(c => c.History)
            .Include(c => c.Documents)
            .ToList();
        return Ok(corpses);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public IActionResult GetById(int id)
    {
        var corpse = _context.Corpses
            .Include(c => c.NextOfKin)
            .Include(c => c.Belongings)
            .Include(c => c.History)
            .Include(c => c.Documents)
            .FirstOrDefault(c => c.Id == id);
            
        if (corpse == null)
        {
            throw new ResourceNotFoundException($"Không tìm thấy thi thể có ID = {id} trong hệ thống.");
        }
        return Ok(corpse);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Create([FromBody] CreateCorpseDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            errors.Add("name", new[] { "Họ và tên không được để trống." });
        }
        if (string.IsNullOrWhiteSpace(dto.Cccd))
        {
            errors.Add("cccd", new[] { "Số CCCD/CMND không được để trống." });
        }

        // Check and reserve slot
        int? storageSlotId = null;
        if (!string.IsNullOrEmpty(dto.StorageSlot))
        {
            var slot = _context.StorageSlots.FirstOrDefault(s => s.SlotNumber == dto.StorageSlot);
            if (slot != null)
            {
                if (slot.Status != SlotStatus.Empty)
                {
                    errors.Add("storageSlot", new[] { $"Ngăn tủ {dto.StorageSlot} đã có người sử dụng hoặc đang bảo trì." });
                }
                else
                {
                    slot.Status = SlotStatus.Occupied;
                    storageSlotId = slot.Id;
                }
            }
        }

        if (errors.Any())
        {
            throw new AppValidationException("Thông tin thi thể gửi lên không hợp lệ.", errors);
        }

        // Generate Case ID
        int nextId = 1;
        if (_context.Corpses.Any())
        {
            nextId = _context.Corpses.Max(c => c.Id) + 1;
        }

        var corpse = new Corpse
        {
            Name = dto.Name,
            Cccd = dto.Cccd,
            Gender = dto.Gender,
            BirthDate = dto.BirthDate,
            Age = dto.Age,
            CauseOfDeath = dto.CauseOfDeath,
            DateOfDeath = dto.DateOfDeath,
            Status = dto.Status,
            StorageUnit = dto.StorageUnit,
            StorageSlot = dto.StorageSlot,
            StorageSlotId = storageSlotId,
            Temp = dto.Temp,
            Notes = dto.Notes,
            NextOfKin = new NextOfKinInfo
            {
                Name = dto.NextOfKin?.Name ?? "",
                Phone = dto.NextOfKin?.Phone ?? "",
                Relationship = dto.NextOfKin?.Relationship ?? ""
            },
            CaseId = $"MC-2026-{nextId:D4}",
            DateAdmitted = DateTime.Now.ToString("yyyy-MM-dd"),
            DaysStored = 1,
            Priority = "NORMAL"
        };

        var userEmail = User.Identity?.Name ?? "system@hospital.vn";
        var shiftInfo = GetCurrentShiftInfo();
        var currentShift = _context.Shifts.FirstOrDefault(s => s.Date == shiftInfo.Date && s.ShiftType == shiftInfo.ShiftType);
        var shiftStaff = currentShift?.StaffEmail ?? "Chưa phân ca";

        corpse.History.Add(new HistoryEntry
        {
            Status = corpse.Status,
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            By = $"{userEmail} (Trực ca: {shiftStaff})",
            Detail = $"Tiếp nhận thi thể mới: {corpse.Name} (Mã HS: {corpse.CaseId})."
        });

        _context.Corpses.Add(corpse);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = corpse.Id }, corpse);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Update(int id, [FromBody] UpdateCorpseDto dto)
    {
        var corpse = _context.Corpses
            .Include(c => c.NextOfKin)
            .Include(c => c.History)
            .Include(c => c.Documents)
            .Include(c => c.Belongings)
            .FirstOrDefault(c => c.Id == id);

        if (corpse == null)
        {
            throw new ResourceNotFoundException($"Không tìm thấy thi thể có ID = {id} để cập nhật.");
        }

        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            errors.Add("name", new[] { "Họ và tên không được để trống." });
        }

        // Validate release conditions if status changes to "Bàn giao"
        if (dto.Status == "Bàn giao" && corpse.Status != "Bàn giao")
        {
            var hasDeathCert = corpse.Documents.Any(d => 
                d.Type.Equals("Death Certificate", StringComparison.OrdinalIgnoreCase) || 
                d.Name.Contains("giay_chung_tu", StringComparison.OrdinalIgnoreCase));
                
            if (!hasDeathCert)
            {
                errors.Add("status", new[] { "Không thể bàn giao: Thiếu tệp tin Giấy chứng tử đính kèm." });
            }

            if (string.IsNullOrWhiteSpace(corpse.NextOfKin?.Name) || string.IsNullOrWhiteSpace(corpse.NextOfKin?.Phone))
            {
                errors.Add("nextOfKin", new[] { "Không thể bàn giao: Thông tin thân nhân nhận bàn giao chưa đầy đủ." });
            }
        }

        // Handle slot change logic
        if (corpse.StorageSlot != dto.StorageSlot && !errors.Any())
        {
            // Free the old slot
            if (corpse.StorageSlotId != null)
            {
                var oldSlot = _context.StorageSlots.Find(corpse.StorageSlotId);
                if (oldSlot != null)
                {
                    oldSlot.Status = SlotStatus.Empty;
                }
            }

            // Reserve the new slot
            if (!string.IsNullOrEmpty(dto.StorageSlot))
            {
                var newSlot = _context.StorageSlots.FirstOrDefault(s => s.SlotNumber == dto.StorageSlot);
                if (newSlot != null)
                {
                    if (newSlot.Status != SlotStatus.Empty)
                    {
                        errors.Add("storageSlot", new[] { $"Ngăn tủ {dto.StorageSlot} đã có người sử dụng hoặc đang bảo trì." });
                    }
                    else
                    {
                        newSlot.Status = SlotStatus.Occupied;
                        corpse.StorageSlotId = newSlot.Id;
                        corpse.StorageUnit = newSlot.UnitName;
                        corpse.StorageSlot = newSlot.SlotNumber;
                        corpse.Temp = newSlot.CurrentTemperature;
                    }
                }
                else
                {
                    corpse.StorageSlotId = null;
                    corpse.StorageUnit = dto.StorageUnit;
                    corpse.StorageSlot = dto.StorageSlot;
                    corpse.Temp = dto.Temp;
                }
            }
            else
            {
                corpse.StorageSlotId = null;
                corpse.StorageUnit = null;
                corpse.StorageSlot = null;
                corpse.Temp = null;
            }
        }

        // If status changes to Bàn giao (Handed over), release the slot and set to Cleaning status
        if (dto.Status == "Bàn giao" && corpse.Status != "Bàn giao" && !errors.Any())
        {
            if (corpse.StorageSlotId != null)
            {
                var slot = _context.StorageSlots.Find(corpse.StorageSlotId);
                if (slot != null)
                {
                    slot.Status = SlotStatus.Cleaning;
                    slot.CurrentTemperature = 15.0; // Temp rises during cleaning
                }
            }
            corpse.StorageSlotId = null;
            corpse.StorageUnit = null;
            corpse.StorageSlot = null;
            corpse.Temp = null;
        }

        if (errors.Any())
        {
            throw new AppValidationException("Thông tin thi thể gửi lên không hợp lệ.", errors);
        }

        corpse.Name = dto.Name;
        corpse.Cccd = dto.Cccd;
        corpse.Gender = dto.Gender;
        corpse.BirthDate = dto.BirthDate;
        corpse.Age = dto.Age;
        corpse.CauseOfDeath = dto.CauseOfDeath;
        corpse.DateOfDeath = dto.DateOfDeath;
        corpse.Status = dto.Status;
        if (dto.Status != "Bàn giao")
        {
            corpse.StorageUnit = dto.StorageUnit;
            corpse.StorageSlot = dto.StorageSlot;
            corpse.Temp = dto.Temp;
        }
        corpse.Notes = dto.Notes;
        if (dto.NextOfKin != null)
        {
            corpse.NextOfKin ??= new NextOfKinInfo();
            corpse.NextOfKin.Name = dto.NextOfKin.Name;
            corpse.NextOfKin.Phone = dto.NextOfKin.Phone;
            corpse.NextOfKin.Relationship = dto.NextOfKin.Relationship;
        }

        var userEmail = User.Identity?.Name ?? "system@hospital.vn";
        var shiftInfo = GetCurrentShiftInfo();
        var currentShift = _context.Shifts.FirstOrDefault(s => s.Date == shiftInfo.Date && s.ShiftType == shiftInfo.ShiftType);
        var shiftStaff = currentShift?.StaffEmail ?? "Chưa phân ca";

        corpse.History.Add(new HistoryEntry
        {
            Status = corpse.Status,
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            By = $"{userEmail} (Trực ca: {shiftStaff})",
            Detail = $"Cập nhật thông tin thi thể. Trạng thái: {corpse.Status}. Ngăn tủ: {corpse.StorageSlot ?? "Không có"}."
        });

        _context.SaveChanges();
        return Ok(corpse);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var corpse = _context.Corpses.FirstOrDefault(c => c.Id == id);
        if (corpse == null)
        {
            throw new ResourceNotFoundException($"Không tìm thấy thi thể có ID = {id} để xóa.");
        }

        // Free slot if occupied
        if (corpse.StorageSlotId != null)
        {
            var slot = _context.StorageSlots.Find(corpse.StorageSlotId);
            if (slot != null)
            {
                slot.Status = SlotStatus.Empty;
            }
        }

        _context.Corpses.Remove(corpse);
        _context.SaveChanges();

        return Ok(new { Message = $"Đã xóa thi thể ID = {id} thành công!" });
    }

    [HttpGet("{id:int}/qrcode")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public IActionResult GetQrCode(int id)
    {
        var corpse = _context.Corpses.FirstOrDefault(c => c.Id == id);
        if (corpse == null)
        {
            throw new ResourceNotFoundException($"Không tìm thấy thi thể có ID = {id} để tạo mã QR.");
        }

        var qrText = $"Mã HS: {corpse.CaseId}\nHọ tên: {corpse.Name}\nNgày sinh: {corpse.BirthDate}\nNgăn tủ: {corpse.StorageSlot ?? "Chưa xếp"}";
        
        using var qrGenerator = new QRCoder.QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(qrText, QRCoder.QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new QRCoder.PngByteQRCode(qrCodeData);
        byte[] qrCodeImage = qrCode.GetGraphic(20);

        return File(qrCodeImage, "image/png");
    }

    [HttpPost("{id:int}/autopsy")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult SubmitAutopsy(int id, [FromBody] AutopsyReportDto dto)
    {
        var corpse = _context.Corpses
            .Include(c => c.History)
            .FirstOrDefault(c => c.Id == id);
            
        if (corpse == null)
        {
            throw new ResourceNotFoundException($"Không tìm thấy thi thể có ID = {id} để lập báo cáo khám nghiệm.");
        }

        corpse.AutopsyReport = new AutopsyReport
        {
            PathologistName = dto.PathologistName,
            ConcludingCause = dto.ConcludingCause,
            ToxicologyResult = dto.ToxicologyResult,
            InternalExamDetails = dto.InternalExamDetails,
            Timestamp = DateTime.Now
        };

        var userEmail = User.Identity?.Name ?? "system@hospital.vn";
        var shiftInfo = GetCurrentShiftInfo();
        var currentShift = _context.Shifts.FirstOrDefault(s => s.Date == shiftInfo.Date && s.ShiftType == shiftInfo.ShiftType);
        var shiftStaff = currentShift?.StaffEmail ?? "Chưa phân ca";

        corpse.History.Add(new HistoryEntry
        {
            Status = corpse.Status,
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            By = $"{userEmail} (Trực ca: {shiftStaff})",
            Detail = $"Cập nhật báo cáo khám nghiệm bởi BS {dto.PathologistName}. Kết luận: {dto.ConcludingCause}."
        });

        _context.SaveChanges();
        return Ok(corpse);
    }

    [HttpGet("{id:int}/autopsy/pdf")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public IActionResult GetAutopsyPdf(int id)
    {
        var corpse = _context.Corpses.FirstOrDefault(c => c.Id == id);
        if (corpse == null)
        {
            throw new ResourceNotFoundException($"Không tìm thấy thi thể có ID = {id}.");
        }

        if (corpse.AutopsyReport == null)
        {
            return BadRequest(new { Message = "Thi thể này chưa có báo cáo khám nghiệm tử thi." });
        }

        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(QuestPDF.Helpers.PageSizes.A4);
                page.Margin(1.5f, QuestPDF.Infrastructure.Unit.Centimetre);
                page.PageColor(QuestPDF.Helpers.Colors.White);
                page.DefaultTextStyle(x => x.FontFamily(QuestPDF.Helpers.Fonts.Arial).FontSize(11));

                page.Header().Column(header =>
                {
                    header.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("BỆNH VIỆN ĐA KHOA TRUNG ƯƠNG").Bold().FontSize(12).FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);
                            col.Item().Text("KHOA GIẢI PHẪU BỆNH & PHÁP Y").Bold().FontSize(10).FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                        });
                        row.ConstantItem(100).AlignRight().Text("Mã HS: " + corpse.CaseId).Bold().FontSize(11);
                    });
                    header.Item().PaddingTop(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Blue.Darken3);
                    header.Item().PaddingTop(15).AlignCenter().Text("BÁO CÁO KHÁM NGHIỆM TỬ THI").Bold().FontSize(18).FontColor(QuestPDF.Helpers.Colors.Blue.Darken4);
                    header.Item().AlignCenter().Text("FORENSIC AUTOPSY REPORT").Italic().FontSize(12).FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);
                });

                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Item().Text("I. THÔNG TIN HÀNH CHÍNH (ADMINISTRATIVE INFORMATION)").Bold().FontSize(12).FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);
                    
                    col.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(120);
                            columns.RelativeColumn();
                            columns.ConstantColumn(120);
                            columns.RelativeColumn();
                        });

                        table.Cell().Text("Họ và tên:").Bold();
                        table.Cell().Text(corpse.Name);
                        table.Cell().Text("Số CCCD:").Bold();
                        table.Cell().Text(corpse.Cccd);

                        table.Cell().Text("Giới tính:").Bold();
                        table.Cell().Text(corpse.Gender);
                        table.Cell().Text("Ngày sinh / Tuổi:").Bold();
                        table.Cell().Text($"{corpse.BirthDate} ({corpse.Age} tuổi)");

                        table.Cell().Text("Ngày nhập viện:").Bold();
                        table.Cell().Text(corpse.DateAdmitted);
                        table.Cell().Text("Ngày tử vong:").Bold();
                        table.Cell().Text(corpse.DateOfDeath);
                    });

                    col.Item().PaddingVertical(10).LineHorizontal(0.5f).LineColor(QuestPDF.Helpers.Colors.Grey.Lighten1);

                    col.Item().Text("II. THÔNG TIN KHÁM NGHIỆM PHÁP Y (AUTOPSY FINDINGS)").Bold().FontSize(12).FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);
                    
                    col.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(150);
                            columns.RelativeColumn();
                        });

                        table.Cell().Text("Bác sĩ pháp y:").Bold();
                        table.Cell().Text(corpse.AutopsyReport.PathologistName);

                        table.Cell().Text("Thời gian lập báo cáo:").Bold();
                        table.Cell().Text(corpse.AutopsyReport.Timestamp.ToString("dd/MM/yyyy HH:mm"));
                    });

                    col.Item().PaddingTop(10).Text("Khám nghiệm nội tạng (Internal Examination):").Bold();
                    col.Item().PaddingTop(2).Text(corpse.AutopsyReport.InternalExamDetails);

                    col.Item().PaddingTop(10).Text("Xét nghiệm độc chất (Toxicology Results):").Bold();
                    col.Item().PaddingTop(2).Text(corpse.AutopsyReport.ToxicologyResult);

                    col.Item().PaddingVertical(10).LineHorizontal(0.5f).LineColor(QuestPDF.Helpers.Colors.Grey.Lighten1);

                    col.Item().Text("III. KẾT LUẬN (CONCLUSION)").Bold().FontSize(12).FontColor(QuestPDF.Helpers.Colors.Blue.Darken3);
                    col.Item().PaddingTop(5).Background(QuestPDF.Helpers.Colors.Grey.Lighten4).Padding(10).Column(concl =>
                    {
                        concl.Item().Text("Nguyên nhân tử vong chính (Concluding Cause of Death):").Bold().FontColor(QuestPDF.Helpers.Colors.Red.Darken2);
                        concl.Item().PaddingTop(2).Text(corpse.AutopsyReport.ConcludingCause).Bold();
                    });

                    col.Item().PaddingTop(40).Row(row =>
                    {
                        row.RelativeItem().AlignCenter().Column(sig =>
                        {
                            sig.Item().Text("BÁC SĨ KHÁM NGHIỆM").Bold();
                            sig.Item().Text("(Ký và ghi rõ họ tên)").Italic().FontSize(9);
                            sig.Item().PaddingTop(40).Text(corpse.AutopsyReport.PathologistName).Bold();
                        });
                        row.RelativeItem().AlignCenter().Column(sig =>
                        {
                            sig.Item().Text("GIÁM ĐỐC BỆNH VIỆN").Bold();
                            sig.Item().Text("(Ký tên, đóng dấu)").Italic().FontSize(9);
                            sig.Item().PaddingTop(40).Text("PGS. TS. Nguyễn Văn A").Bold();
                        });
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Trang ").FontSize(9);
                    x.CurrentPageNumber().FontSize(9);
                    x.Span(" / ").FontSize(9);
                    x.TotalPages().FontSize(9);
                });
            });
        });

        byte[] pdfBytes = document.GeneratePdf();
        return File(pdfBytes, "application/pdf", $"BaoCaoTuThi_{corpse.CaseId}.pdf");
    }

    [HttpGet("test-error")]
    [AllowAnonymous]
    public IActionResult TestError()
    {
        throw new DivideByZeroException("Lỗi thử nghiệm hệ thống: Chia cho 0.");
    }

    private (DateTime Date, string ShiftType) GetCurrentShiftInfo()
    {
        var now = DateTime.Now;
        var date = now.Date;
        string shiftType;

        if (now.Hour >= 6 && now.Hour < 14)
        {
            shiftType = "Morning";
        }
        else if (now.Hour >= 14 && now.Hour < 22)
        {
            shiftType = "Afternoon";
        }
        else
        {
            shiftType = "Night";
            if (now.Hour < 6)
            {
                date = date.AddDays(-1);
            }
        }
        return (date, shiftType);
    }
}
