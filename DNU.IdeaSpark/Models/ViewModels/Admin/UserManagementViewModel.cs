using DNU.IdeaSpark.Models.Entities;

namespace DNU.IdeaSpark.Models.ViewModels.Admin;

public class UserManagementViewModel
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Department { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public int Points { get; set; }
}