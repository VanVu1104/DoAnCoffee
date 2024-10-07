namespace Manage_Coffee.Models.ViewModels
{
    public class CheckoutKH
    {
        public bool GiongKhachHang { get; set; }
        public string? HoTen { get; set; }
        public string? DiaChi { get; set; }
        public string? DienThoai { get; set; }
        public string? GhiChu { get; set; }
        public string? MaCn { get; set; } 

        public string? TenCN { get; set; } 

        public string? Diachi { get; set; }
        public string? DiaChiNha { get; set; }

        public CheckoutKH() { }
        public CheckoutKH(ChiNhanh chi, KhachHang kh)
        {
            MaCn = chi.MaCn;
            TenCN = chi.Ten;
            Diachi = chi.Diachi;
            DiaChi = kh.Diachi;
        }
    }
}
