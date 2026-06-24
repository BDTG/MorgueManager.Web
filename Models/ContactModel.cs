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
}
