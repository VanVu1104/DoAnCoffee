using System.ComponentModel.DataAnnotations;

namespace Manage_Coffee.Models
{
    public class RegisterAdmin
    {
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        public int Phone { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mật khẩu nhập lại là bắt buộc.")]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
        public string ConfirmPassword { get; set; }

        public string? Name { get; set; }
        public string? Address { get; set; }
    }
}
