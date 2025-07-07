using System.ComponentModel.DataAnnotations;

namespace DNU.IdeaSpark.Models.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Nhập email trường")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? ErrorMessage { get; set; }

    }
}