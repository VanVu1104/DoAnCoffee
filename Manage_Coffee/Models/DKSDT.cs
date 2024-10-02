using System.ComponentModel.DataAnnotations;
namespace Manage_Coffee.Models
{
    public class DKSDT
    {
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        public int Sdt { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.", MinimumLength = 6)]
        public string Matkhau { get; set; }

        [Required(ErrorMessage = "Mật khẩu nhập lại là bắt buộc.")]
        [Compare("Matkhau", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
        public string MatkhauNhapLai { get; set; }

        public string? Ten { get; set; }
        public string? Diachi { get; set; }
    }
}
