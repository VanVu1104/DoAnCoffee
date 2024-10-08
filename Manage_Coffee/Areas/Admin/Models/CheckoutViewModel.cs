using Manage_Coffee.Models;

namespace Manage_Coffee.Areas.Admin.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public string TenNhanVien { get; set; }
        public string MaNv { get; set; }
        public decimal TongTien { get; set; }
    }
}
