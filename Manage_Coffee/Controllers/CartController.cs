using Manage_Coffee.Helpers;
using Manage_Coffee.Models;
using Manage_Coffee.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manage_Coffee.Controllers
{
	public class CartController : Controller
	{
		private readonly Cf2Context _context;
		private readonly PaypalClient _paypalClient;

		public CartController(Cf2Context context, PaypalClient paypalClient)
		{
			_paypalClient = paypalClient;
			_context = context;
		}
		// Comment test

		public IActionResult Cart()
		{

			List<CartItem> Cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key) ?? new List<CartItem>();
			CartItemViewModel cart1 = new()
			{
				CartItems = Cart,
				GrandTotal = Cart.Sum(X => X.Soluong *
				(X.Dongia + X.TriGia)),
			};
			return View(cart1);
		}
		public async Task<IActionResult> Add(string id, string idsize, string ghiChu)
		{
			SanPham sanPham = await _context.SanPhams.FindAsync(id);
			Size size = await _context.Sizes.FindAsync(idsize);

			if (sanPham == null || size == null)
			{
				// Xử lý lỗi nếu sản phẩm hoặc kích thước không tồn tại
				return NotFound();
			}

			List<CartItem> Cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key) ?? new List<CartItem>();
			CartItem cartItem = Cart.Where(c => c.ProductID == id && c.SizeID == idsize).FirstOrDefault();

			if (cartItem == null)
			{
				Cart.Add(new CartItem(sanPham, size, ghiChu));
			}
			else
			{
				cartItem.Soluong += 1;
				cartItem.Ghichu = ghiChu;
			}

			HttpContext.Session.Set(MySetting.Cart_key, Cart);

			return Redirect(Request.Headers["Referer"].ToString());
		}

		public async Task<IActionResult> Decrease(string id, string idsize)
		{
			List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);
			CartItem cartItem = cart.Where(c => c.ProductID == id).FirstOrDefault();
			if (cartItem.Soluong > 1)
			{
				--cartItem.Soluong;
			}
			else
			{
				cart.RemoveAll(p => p.Soluong > 0);
			}
			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.Set(MySetting.Cart_key, cart);
			}
			return RedirectToAction("Cart");
		}
		public async Task<IActionResult> Increase(string id, string idsize)
		{
			List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);
			CartItem cartItem = cart.Where(c => c.ProductID == id).FirstOrDefault();
			if (cartItem.Soluong > 1)
			{
				++cartItem.Soluong;
			}
			else
			{
				cart.RemoveAll(p => p.Soluong > 0);
			}
			if (cart.Count == 0)
			{
				HttpContext.Session.Remove(MySetting.Cart_key);
			}
			else
			{
				HttpContext.Session.Set(MySetting.Cart_key, cart);
			}
			return RedirectToAction("Cart");
		}
		public async Task<IActionResult> Remove(string id)
		{
			List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);
			cart.RemoveAll(c => c.ProductID == id);

			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.Set(MySetting.Cart_key, cart);
			}
			return RedirectToAction("Cart");
		}
		public async Task<IActionResult> Clear(string id)
		{
			HttpContext.Session.Remove(MySetting.Cart_key);
			return RedirectToAction("Sanpham1");
		}
		//[Authorize]
		[HttpGet]
		public IActionResult Checkout()
		{
			List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);
			if (cart.Count == 0)
			{
				return Redirect("/");
			}
			ViewBag.PaypalClientdId = _paypalClient.ClientId;
			return View(cart);
		}
		//[Authorize]
		[HttpPost]
		public IActionResult Checkout(CheckoutKH checkoutKH)
		{
			var makh = "";
			int tongtien = 0;
			// Lấy mã khách hàng từ Claims
			if (HttpContext.Session.GetString("UserName") == null)
			{
				var makhClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "MAKH");
				makh = makhClaim?.Value;
			}
			else
			{
				makh = HttpContext.Session.GetString("UserPhone");
			}


			if (string.IsNullOrEmpty(makh))
			{
				throw new Exception("Mã khách hàng không tồn tại trong Claims!");
			}

			var khachhang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == makh);
			if (khachhang == null)
			{
				throw new Exception($"Không tìm thấy khách hàng với mã: {makh}");
			}

			Console.WriteLine($"Khách hàng: {khachhang.Ten}");

			var hoadon = new Phieudhonl
			{
				MaPhieuonl = makh + DateTime.Now,
				MaKh = makh,
				DiaChi = checkoutKH.DiaChi ?? khachhang.Diachi,
				Ngaygiodat = DateTime.Now,
				Ptnh = "GRAB",
				Pttt = "COD",
				TrangThai = false,
				MaKm = "KM001",
				MaCn = "CN001",
				TongTien = 0, // Initialize with 0, will update later
				TienShip = 20
			};

			// Begin transaction
			using (var transaction = _context.Database.BeginTransaction())
			{
				List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);
				try
				{
					// Calculate total before saving
					tongtien = 0;
					var ctsp = new List<Ctsponl>();

					foreach (var item in cart)
					{
						tongtien += (item.Dongia + item.TriGia) * item.Soluong;
						ctsp.Add(new Ctsponl
						{
							MaKh = makh,
							MaPhieuonl = hoadon.MaPhieuonl,
							Soluong = item.Soluong,
							MaSp = item.ProductID,
							MaSize = item.SizeID,
							Gia = item.Dongia,
							Ghichu = item.Ghichu,
							Tongtien = tongtien,
						});
					}

					// Update the invoice total
					hoadon.TongTien = tongtien + hoadon.TienShip; // Include shipping if needed

					// Save invoice first
					_context.Add(hoadon);
					_context.SaveChanges();

					// Save cart items
					_context.AddRange(ctsp);
					_context.SaveChanges();

					// Commit the transaction if everything succeeded
					transaction.Commit();



					// Xóa giỏ hàng
					HttpContext.Session.Set<List<CartItem>>(MySetting.Cart_key, new List<CartItem>());
					return View("Success");
				}
				catch (Exception ex)
				{
					// Rollback giao dịch nếu có lỗi xảy ra
					transaction.Rollback();
					// Bạn có thể ghi log lỗi ở đây nếu cần
					throw; // hoặc trả về thông báo lỗi phù hợp
				}
			}
		}
		public IActionResult PaymentSuccess()
		{
			return View("Success");
		}

		

		#region Paypal payment
		[Authorize]
		[HttpPost("/Cart/create-paypal-order")]
		public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
		{
		
			List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);

		
			if (cart == null || cart.Count == 0)
			{
				return BadRequest("Giỏ hàng trống.");
			}

			// Thông tin đơn hàng gửi qua Paypal
			var tongTien = cart.Sum(p => p.Total).ToString();
			var donViTienTe = "USD";
			var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();
			
			try
			{
				// Tạo đơn hàng PayPal
				var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);
				return Ok(response);
			}
			catch (Exception ex)
			{
				var error = new { ex.GetBaseException().Message };
				return BadRequest(error);
			}
		}


		[Authorize]
		[HttpPost("/Cart/capture-paypal-order")]
		public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken,CheckoutKH checkoutKH)
		{
			try
			{
				// Gọi PayPal API để capture order
				var response = await _paypalClient.CaptureOrder(orderID);


				// Thanh toán thành công, tiến hành lưu hóa đơn

				// Lấy giỏ hàng từ session
				List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);

				if (cart == null || cart.Count == 0)
				{
					return BadRequest("Giỏ hàng trống");
				}

				// Lấy thông tin khách hàng từ Claims hoặc Session
				var makh = HttpContext.Session.GetString("UserPhone");
				if (string.IsNullOrEmpty(makh))
				{
					var makhClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "MAKH");
					makh = makhClaim?.Value;
				}

				if (string.IsNullOrEmpty(makh))
				{
					return BadRequest("Mã khách hàng không tồn tại");
				}

				var khachhang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == makh);
				if (khachhang == null)
				{
					return BadRequest($"Không tìm thấy khách hàng với mã: {makh}");
				}

				// Tạo hóa đơn
				var hoadon = new Phieudhonl
				{
					MaPhieuonl = makh + DateTime.Now,
					MaKh = makh,
					DiaChi = checkoutKH.DiaChi ?? khachhang.Diachi,
					Ngaygiodat = DateTime.Now,
					Ptnh = "GRAB", // Hoặc thay đổi theo phương thức giao hàng bạn muốn
					Pttt = "PayPal", // Thanh toán bằng PayPal
					TrangThai = true, // Đơn hàng đã thanh toán thành công
					MaKm = "KM001", // Có thể lấy mã khuyến mãi nếu có
					MaCn = "CN001", // Mã chi nhánh
					TongTien = cart.Sum(item => (item.Dongia + item.TriGia) * item.Soluong),
					TienShip = 20 // Hoặc tính toán phí ship từ trước
				};

				// Tạo danh sách chi tiết sản phẩm
				var ctsp = new List<Ctsponl>();
				foreach (var item in cart)
				{
					ctsp.Add(new Ctsponl
					{
						MaKh = makh,
						MaPhieuonl = hoadon.MaPhieuonl,
						Soluong = item.Soluong,
						MaSp = item.ProductID,
						MaSize = item.SizeID,
						Gia = item.Dongia,
						Ghichu = item.Ghichu,
						Tongtien = (item.Dongia + item.TriGia) * item.Soluong
					});
				}

				// Bắt đầu giao dịch
				using (var transaction = _context.Database.BeginTransaction())
				{
					try
					{
						// Lưu hóa đơn
						_context.Add(hoadon);
						_context.SaveChanges();

						// Lưu chi tiết hóa đơn
						_context.AddRange(ctsp);
						_context.SaveChanges();

						// Commit giao dịch
						transaction.Commit();
					}
					catch (Exception ex)
					{
						// Rollback giao dịch nếu có lỗi
						transaction.Rollback();
						throw;
					}
				}

				// Xóa giỏ hàng sau khi thanh toán thành công
				HttpContext.Session.Set<List<CartItem>>(MySetting.Cart_key, new List<CartItem>());

				return Ok(response); // Trả về phản hồi thành công
			}
			catch (Exception ex)
			{
				var error = new { ex.GetBaseException().Message };
				return BadRequest(error);
			}
		}
		#endregion
	}
}

