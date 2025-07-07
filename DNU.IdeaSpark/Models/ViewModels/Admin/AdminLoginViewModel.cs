namespace DNU.IdeaSpark.Models.ViewModels.Admin
{
    public class AdminLoginViewModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string? ErrorMessage { get; set; }
    }
}