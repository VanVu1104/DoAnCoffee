namespace Manage_Coffee.Models.ViewModels
{
    public class CartItemViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal GrandTotal { get; set; }
		public List<DanhMucKm> Discounts { get; set; }
	}
}
