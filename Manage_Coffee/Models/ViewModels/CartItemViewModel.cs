namespace Manage_Coffee.Models.ViewModels
{
    public class CartItemViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal GrandTotal { get; set; }
		public List<DanhMucKm> Discounts { get; set; }

        public KhachHang KhachHang { get; set; }
		public List<Kit> KitsAsKit { get; set; }
		public List<Kit> KitsAsSp { get; set; }
		public List<string> SelectedKits { get; set; }
	}
}
