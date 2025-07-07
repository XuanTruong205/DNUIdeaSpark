using System.ComponentModel.DataAnnotations;

namespace DNU.IdeaSpark.Models.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Nhập email trường")]
        [EmailAddress]
        public string Email { get; set; }
    }
}