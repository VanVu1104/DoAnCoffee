using Manage_Coffee.Helpers;
using Manage_Coffee.Models;
using Manage_Coffee.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Logging;
using Newtonsoft.Json;

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


		public IActionResult Cart()
		{

			List<CartItem> Cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key) ?? new List<CartItem>();
			CartItemViewModel cart1 = new()
			{
				CartItems = Cart,
				GrandTotal = Cart.Sum(X => X.Soluong *
				(X.Dongia + X.TriGia)),
				Discounts = _context.DanhMucKms.ToList()
			};
			return View(cart1);
		}
		[HttpPost]
		public IActionResult ApplyDiscount(string discountCode)
		{
			List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);
			if (cart == null || !cart.Any())
			{
				return BadRequest("Giỏ hàng trống.");
			}

			var discount = _context.DanhMucKms.SingleOrDefault(d => d.MaKm == discountCode);
			if (discount == null || discount.Soluong <= 0 || discount.Ngayhethan < DateTime.Now)
			{
				return BadRequest("Mã giảm giá không hợp lệ.");
			}

			// Tính toán và áp dụng giảm giá
			int discountAmount = discount.GiaTri; // Giả sử là giá trị giảm giá
			int grandTotal = cart.Sum(item => (item.Dongia + item.TriGia) * item.Soluong) * (1 - discountAmount / 100);

			// Cập nhật lại tổng tiền trong ViewBag hoặc Model
			ViewBag.DiscountApplied = discountAmount;
			ViewBag.NewTotal = grandTotal;

			// Quay lại View giỏ hàng
			return RedirectToAction("Cart");
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
			if (cart == null || cart.Count == 0)
			{
				return Redirect("/");
			}
			ViewBag.PaypalClientdId = _paypalClient.ClientId;
			var viewModel = new CartItemViewModel
			{
				CartItems = cart, // Gán danh sách cart vào thuộc tính CartItems
				Discounts = new List<DanhMucKm>()
			};
			viewModel.Discounts = GetDiscounts();
			return View(viewModel);
		}
		private List<DanhMucKm> GetDiscounts()
		{
			// Giả sử bạn có một dịch vụ hoặc repository để lấy giảm giá
			// Ví dụ, nếu bạn sử dụng Entity Framework
			using (var context = new Cf2Context())
			{
				return context.DanhMucKms.ToList(); // Lấy danh sách giảm giá từ cơ sở dữ liệu
			}
		}

		[HttpPost]
        public IActionResult Checkout(CheckoutKH checkoutKH, string macn, string MaKm, decimal totalValue)
        {
            var makh = "";
            int tongtien = 0;
		
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

            // Tính tổng chi tiêu của khách hàng
            var tongChiTieu = _context.Phieudhonls
                .Where(hd => hd.MaKh == makh)
                .Sum(hd => hd.TongTien);

            // Xác định vai trò của khách hàng dựa trên tổng chi tiêu
            if (tongChiTieu > 3000000)
            {
                khachhang.Role = "Vang"; // Nâng cấp lên Vàng
            }
            else if (tongChiTieu > 1000000)
            {
                khachhang.Role = "Bac"; 
            }
            else
            {
                khachhang.Role = "Dong"; 
            }

           
            _context.Update(khachhang);
            _context.SaveChanges();

            Console.WriteLine($"Khách hàng: {khachhang.Ten}, Role: {khachhang.Role}");

            var hoadon = new Phieudhonl
            {
                MaPhieuonl = makh + DateTime.Now.ToString("yyyyMMddHHmmss"),
                MaKh = makh,
                DiaChi = checkoutKH.DiaChi ?? khachhang.Diachi,
                Ngaygiodat = DateTime.Now,
                Ptnh = "GRAB",
                Pttt = "COD",
                TrangThai = false,
                MaKm = MaKm,
                MaCn = macn,
                TongTien = 0,
                TienShip = 20
            };

            // Bắt đầu giao dịch
            using (var transaction = _context.Database.BeginTransaction())
            {
                List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);
                try
                {
                    tongtien = 0;
                    var ctsp = new List<Ctsponl>();

                    foreach (var item in cart)
                    {
                        tongtien =0;
                        ctsp.Add(new Ctsponl
                        {
                            MaKh = makh,
                            MaPhieuonl = hoadon.MaPhieuonl,
                            Soluong = item.Soluong,
                            MaSp = item.ProductID,
                            MaSize = item.SizeID,
                            Gia = item.Dongia,
                            Ghichu = item.Ghichu,
                            Tongtien = (item.Dongia + item.TriGia) * item.Soluong,
                        });
                    }

                   
                    hoadon.TongTien = (int)totalValue + hoadon.TienShip;


                   // được để thông báo về này dùm t sd để bên javascript
                    int giamGia = 0; 

                    if (khachhang.Role == "Bac")
                    {
                        giamGia = (int)(hoadon.TongTien * 0.05);
                      
                    }
                
                    else if (khachhang.Role == "Vang")
                    {
                        giamGia = (int)(hoadon.TongTien * 0.10);
                        
                    }

                    hoadon.TongTien -= giamGia;

                    _context.Add(hoadon);
                    _context.SaveChanges();
                    _context.AddRange(ctsp);
                    _context.SaveChanges();

                    transaction.Commit();
                    HttpContext.Session.Set<List<CartItem>>(MySetting.Cart_key, new List<CartItem>());
               

                    return View("Success");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
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
			var tongTien = cart.Sum(p => p.Total).ToString();
			var donViTienTe = "USD";
			var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();
			
			try
			{ 
				var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);
				return Ok(response);
			}
			catch (Exception ex)
			{
                var error = JsonConvert.SerializeObject(new { ex.GetBaseException().Message });
                return BadRequest(error);

            }
        }
		[Authorize]
		[HttpPost("/Cart/capture-paypal-order")]
		public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken,CheckoutKH checkoutKH, string MaKm, decimal totalValue)
		{
            Console.WriteLine("CapturePaypalOrder");
			try
			{
				// Gọi PayPal API để capture order
				var response = await _paypalClient.CaptureOrder(orderID);
				List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);

				if (cart == null || cart.Count == 0)
				{
					return BadRequest("Giỏ hàng trống");
				}

                // Lấy thông tin khách hàng từ Claims hoặc Session
                var makh = " ";
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
					return BadRequest("ma khong ton tai");
				}

				var khachhang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == makh);
				if (khachhang == null)
				{
					return BadRequest($"khong tim thay: {makh}");
				}

				// Tạo hóa đơn
				var hoadon = new Phieudhonl
				{
					MaPhieuonl = makh + DateTime.Now,
					MaKh = makh,
					DiaChi = khachhang.Diachi,
					Ngaygiodat = DateTime.Now,
					Ptnh = "GRAB",
					Pttt = "PayPal",
					TrangThai = true,
					MaKm = MaKm,
					MaCn = "CN001",
					//TongTien = cart.Sum(item => (item.Dongia + item.TriGia) * item.Soluong),
					TongTien = (int)totalValue,
					TienShip = 20 // Hoặc tính toán phí ship từ trước
				};
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
				using (var transaction = _context.Database.BeginTransaction())
				{
					try
					{
						_context.Add(hoadon);
						_context.SaveChanges();
						_context.AddRange(ctsp);
						_context.SaveChanges();
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

                return new JsonResult(response);
                // Trả về phản hồi thành công
            }
            catch (Exception ex)
			{
                var error = JsonConvert.SerializeObject(new { ex.GetBaseException().Message });
                return BadRequest("error");

            }

        }

		#endregion
	}
}

