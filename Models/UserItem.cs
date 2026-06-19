namespace MorgueManager.Web.Models;

public class UserItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Role { get; set; } = "";
    public string RoleBg { get; set; } = "";
    public string RoleText { get; set; } = "";
    public bool IsActive { get; set; }
    public string JoinDate { get; set; } = "";
}
