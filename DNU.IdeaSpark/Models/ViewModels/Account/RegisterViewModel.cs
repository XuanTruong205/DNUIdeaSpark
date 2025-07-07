using System.ComponentModel.DataAnnotations;

namespace DNU.IdeaSpark.Models.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email trường")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@dnu\.edu\.vn$", ErrorMessage = "Chỉ dùng email trường (@dnu.edu.vn)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn khoa/đơn vị")]
        [Display(Name = "Khoa/Đơn vị")]
        public string Department { get; set; }


        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }
    }
}