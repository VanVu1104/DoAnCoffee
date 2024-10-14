namespace Manage_Coffee.Models.ViewModels
{
    public class ChiTietSanPhamViewModel
    {
        public string MaSp { get; set; }
        public string MaSize { get; set; } // Mã size
        public string TenSize { get; set; } // Tên size
        public int SoLuong { get; set; }
        public int Gia { get; set; }
        public int TongTien { get; set; }
        public string? Ghichu { get; set; } // Ghi chú
        public string? TenSP { get; set; } //
    }
}
