using System;
using System.ComponentModel.DataAnnotations;
using Postgrest.Attributes;
using Postgrest.Models;

namespace MorgueManager.Web.Models;

[Table("contact_requests")]
public class ContactModel : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
    [Column("phone")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập chủ đề.")]
    [StringLength(150, ErrorMessage = "Chủ đề không được vượt quá 150 ký tự.")]
    [Column("subject")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập nội dung tin nhắn.")]
    [StringLength(2000, ErrorMessage = "Nội dung tin nhắn không được vượt quá 2000 ký tự.")]
    [Column("message")]
    public string Message { get; set; } = string.Empty;

    [Column("status")]
    public string Status { get; set; } = "Chưa xử lý"; // Trạng thái: Chưa xử lý, Đang liên hệ, Đã hoàn thành

    [Column("storage_slot")]
    public string? StorageSlot { get; set; }

    [Column("package_name")]
    public string? PackageName { get; set; }

    [Column("price")]
    public long? Price { get; set; }
}

public class ContactModelDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "Chưa xử lý";
    public string? StorageSlot { get; set; }
    public string? PackageName { get; set; }
    public long? Price { get; set; }

    public static ContactModelDto FromModel(ContactModel m) => new()
    {
        Id = m.Id,
        CreatedAt = m.CreatedAt,
        Name = m.Name,
        Email = m.Email,
        Phone = m.Phone,
        Subject = m.Subject,
        Message = m.Message,
        Status = m.Status,
        StorageSlot = m.StorageSlot,
        PackageName = m.PackageName,
        Price = m.Price
    };

    public ContactModel ToModel() => new()
    {
        Id = this.Id,
        CreatedAt = this.CreatedAt,
        Name = this.Name,
        Email = this.Email,
        Phone = this.Phone,
        Subject = this.Subject,
        Message = this.Message,
        Status = this.Status,
        StorageSlot = this.StorageSlot,
        PackageName = this.PackageName,
        Price = this.Price
    };
}
