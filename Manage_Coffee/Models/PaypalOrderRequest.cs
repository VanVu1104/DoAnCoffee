namespace Manage_Coffee.Models
{
	public class PaypalOrderRequest
	{
		public string? MaKm { get; set; } // Mã giảm giá
		public decimal? TotalValue { get; set; }
		public int? SoXu { get; set; }
		public string? MaCn { get; set; }
		public string DiaChi { get; set; }
	}
}
