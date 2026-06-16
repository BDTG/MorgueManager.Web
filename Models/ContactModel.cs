using System.ComponentModel.DataAnnotations;

namespace MorgueManager.Web.Models;

public class ContactModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập chủ đề.")]
    [StringLength(150, ErrorMessage = "Chủ đề không được vượt quá 150 ký tự.")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập nội dung tin nhắn.")]
    [StringLength(2000, ErrorMessage = "Nội dung tin nhắn không được vượt quá 2000 ký tự.")]
    public string Message { get; set; } = string.Empty;
}
