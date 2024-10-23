namespace Manage_Coffee.Areas.Admin.Models
{
    public class RegisterViewModel
    {
        public string Ten { get; set; } = null!;
        public int Sdt { get; set; }
        public string Mkhau { get; set; } = null!;
        public string Chucvu { get; set; } = null!; // "Quản lý" hoặc "Phục vụ"
        public string Diachi { get; set; } = null!;
        public string? MaCn { get; set; } = null!;
        public string inputMaCn { get; set; } = null!;
        public bool? GioiTinh { get; set; } 
        public DateTime NgaySinh { get; set; } 
    }
}
