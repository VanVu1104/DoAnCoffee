namespace Manage_Coffee.Models.ViewModels
{
    public class HoaDonViewModel
    {
        public string MaPhieuonl { get; set; }
        public DateTime Ngaygiodat { get; set; }
        public int? TongTien { get; set; }
        public string DiaChi { get; set; }
        public bool TrangThai { get; set; }
        public string? Pttt { get; set; }
        public int? TienShip { get; set; }
		public int? SoXu { get; set; }
		public string? Ptnh { get; set; }
        public string MaKh { get; set; }
        public string MaCn { get; set; }
        public string? MaKm { get; set; }
        public string TenKhachHang { get; set; }
        public string EmailKhachHang { get; set; }
        public string TenChiNhanh { get; set; }
        public string TenKhuyenMai { get; set; }
        public int? GiaTrikm { get; set; }
        public string Role { get; set; }
        public List<Ctsponl> ChiTietSanPham { get; set; }
    }
}
