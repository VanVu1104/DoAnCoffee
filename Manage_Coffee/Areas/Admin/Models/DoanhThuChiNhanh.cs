namespace Manage_Coffee.Areas.Admin.Models
{
    public class DoanhThuChiNhanh
    {
        public string MaCn { get; set; } = null!;
        public decimal TongDoanhThuOnline { get; set; }
        public decimal TongDoanhThuOffline { get; set; }
        public decimal TongDoanhThu => TongDoanhThuOnline + TongDoanhThuOffline; // Tổng doanh thu
    }
}
