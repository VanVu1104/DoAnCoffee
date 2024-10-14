using Manage_Coffee.Helpers;
using Manage_Coffee.Models;
using Manage_Coffee.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

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
                Discounts = _context.DanhMucKms.ToList() // Lấy danh sách thẻ giảm giá từ cơ sở dữ liệu
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
        //[HttpPost]
        //public async Task<IActionResult> Add(string id, string idsize, string ghiChu)
        //{
        //    SanPham sanPham = await _context.SanPhams.FindAsync(id);
        //    Size size = await _context.Sizes.FindAsync(idsize);

        //    if (sanPham == null || size == null)
        //    {
        //        // Xử lý lỗi nếu sản phẩm hoặc kích thước không tồn tại
        //        return NotFound();
        //    }

        //    List<CartItem> Cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key) ?? new List<CartItem>();
        //    CartItem cartItem = Cart.Where(c => c.ProductID == id && c.SizeID == idsize).FirstOrDefault();

        //    if (cartItem == null)
        //    {
        //        cartItem = new CartItem(sanPham, size, ghiChu);

        //        Cart.Add(cartItem);
        //    }
        //    else
        //    {
        //        cartItem.Soluong += 1;
        //        cartItem.Ghichu = ghiChu;

        //    }

        //    HttpContext.Session.Set(MySetting.Cart_key, Cart);

        //    return Redirect(Request.Headers["Referer"].ToString());
        //}
        [HttpPost]
        public async Task<IActionResult> Add(string id, string idsize, string ghiChu, List<string> kits) // Thêm tham số kit
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
                cartItem = new CartItem(sanPham, size, ghiChu);
                
                Cart.Add(cartItem);
            }
            else
            {
                cartItem.Soluong += 1;
                cartItem.Ghichu = ghiChu;
            }

            //// Thêm các kit vào cartItem
            //if (kits != null)
            //{
            //    foreach (var kitId in kits)
            //    {
            //        // Giả sử bạn cần một giá trị MaSp hoặc một cách nào đó để xác định nó
            //        // Bạn có thể cần xác định giá trị MaSp phù hợp cho kitId
            //        var kit = await _context.Kits.FirstOrDefaultAsync(k => k.MaKit == kitId && k.MaSp == id); // Thay id bằng MaSp nếu cần

            //        if (kit != null && !cartItem.Kits.Any(k => k.MaKit == kit.MaKit)) // Kiểm tra để tránh thêm trùng
            //        {
            //            cartItem.Kits.Add(kit); // Thêm kit vào cartItem
            //        }
            //    }
            //}


            HttpContext.Session.Set(MySetting.Cart_key, Cart);

            return Redirect(Request.Headers["Referer"].ToString());
        }


		public async Task<IActionResult> Decrease(string id, string idsize)
		{
			List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key) ?? new List<CartItem>();
			CartItem cartItem = cart.Where(c => c.ProductID == id && c.SizeID == idsize ).FirstOrDefault();
			if (cartItem.Soluong > 1)
			{
				--cartItem.Soluong;
			}
			else
			{
				cart.Remove(cartItem);
			}
			HttpContext.Session.Set(MySetting.Cart_key, cart);
			
			return RedirectToAction("Cart");
		}
		public async Task<IActionResult> Increase(string id, string idsize)
		{
			List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);
			CartItem cartItem = cart.Where(c => c.ProductID == id && c.SizeID == idsize).FirstOrDefault();
			if (cartItem.Soluong >= 1)
			{
				++cartItem.Soluong;
			}
			else
			{
				cart.RemoveAll(p => p.Soluong >0);
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
		public async Task<IActionResult> Remove(string id,string idsize)
        {
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);
            cart.RemoveAll(c => c.ProductID == id && c.SizeID == idsize );

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
        [SessionOrAuthorize]
        [HttpGet]

        public IActionResult Checkout()
        {
            var makh = "";


			var kits = _context.Kits.ToList();
            if (HttpContext.Session.GetString("UserName") == null)
            {
                var makhClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "MAKH");
                makh = makhClaim?.Value;
            }
            else
            {
                makh = HttpContext.Session.GetString("UserPhone");
            }
            var khachHang = _context.KhachHangs
                .Where(r => r.MaKh == makh)
                .FirstOrDefault();
            // Gán số xu vào ViewBag
            if (khachHang != null)
            {
                ViewBag.SoXu = khachHang.Xu;
            }
            else
            {
                ViewBag.SoXu = 0; // Trường hợp khách hàng chưa đăng nhập hoặc không có số xu
            }
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);
            if (cart == null || cart.Count == 0)
            {
                return Redirect("/");
            }

            ViewBag.PaypalClientdId = _paypalClient.ClientId;

            var viewModel = new CartItemViewModel
            {
                KitsAsKit = kits,
                CartItems = cart, // Gán danh sách cart vào thuộc tính CartItems
                Discounts = new List<DanhMucKm>() // Khởi tạo danh sách Discounts
            };

            viewModel.Discounts = GetDiscounts(); // Ví dụ, hãy đảm bảo phương thức này trả về danh sách giảm giá

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


        //[Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutKH checkoutKH, string macn, string MaKm, decimal totalValue, string MaCn, int SoXu)
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
            var tongChiTieu = _context.Phieudhonls
                .Where(hd => hd.MaKh == makh)
                .Sum(hd => hd.TongTien);
            if (tongChiTieu > 3000000)
            {
                khachhang.Role = "Vàng";
            }
            else if (tongChiTieu > 1000000)
            {
                khachhang.Role = "Bạc";
            }
            else
            {
                khachhang.Role = "Đồng";
            }
            _context.Update(khachhang);
            _context.SaveChanges();

            Console.WriteLine($"Khách hàng: {khachhang.Ten}, Role: {khachhang.Role}");
            ViewBag.Role = khachhang.Role;
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
                MaCn = MaCn,
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
                        tongtien = 0;
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
                    if (totalValue == 0)
                    {
                        hoadon.TongTien = cart.Sum(p => (p.Dongia + p.TriGia) * p.Soluong) + hoadon.TienShip;
                    }
                    else
                    {
                        hoadon.TongTien = (int)totalValue + hoadon.TienShip;
                    }

                    int giamGia = 0;
                    if (khachhang.Role == "Bạc")
                    {
                        giamGia = (int)(hoadon.TongTien * 0.05);

                    }

                    else if (khachhang.Role == "Vàng")
                    {
                        giamGia = (int)(hoadon.TongTien * 0.10);

                    }
                    hoadon.TongTien -= giamGia;

                    _context.Add(hoadon);
                    _context.SaveChanges();
                    _context.AddRange(ctsp);
                    _context.SaveChanges();
                    var khachHang = _context.KhachHangs
                    .Where(r => r.MaKh == makh)
                    .FirstOrDefault();
                    if (khachHang != null && khachHang.Xu >= SoXu)
                    {
                        khachHang.Xu -= SoXu; // Giảm xu
                        _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    }
                    var km = _context.DanhMucKms
                    .Where(r => r.MaKm == MaKm)
                    .FirstOrDefault();
                    if (km != null)
                    {
                        km.Soluong--; // Giảm xu
                        _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    }
                    transaction.Commit();
                    HttpContext.Session.Set<List<CartItem>>(MySetting.Cart_key, new List<CartItem>());
                  

					return RedirectToAction("XemHoaDon", new { maPhieuonl = hoadon.MaPhieuonl, soxu= SoXu  });

                    return View("Success");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public IActionResult XemHoaDon(string maPhieuonl, int soxu)
        {
            var phieuDh = _context.Phieudhonls
          .Include(p => p.Ctsponls)
              .ThenInclude(ct => ct.MaSizeNavigation) // Bao gồm thông tin từ bảng Size
          .Include(p => p.Ctsponls) // Bao gồm thông tin từ bảng Ctsponl
              .ThenInclude(ct => ct.MaSpNavigation) // Bao gồm thông tin từ bảng SanPham
          .Include(p => p.MaCnNavigation)
          .Include(p => p.MaKhNavigation)
          .Include(p => p.MaKmNavigation)
          .FirstOrDefault(p => p.MaPhieuonl == maPhieuonl);

            if (phieuDh == null)
            {
                return NotFound("Hóa đơn không tồn tại.");
            }
            var chiTietSanPham = phieuDh.Ctsponls.Select(ct => new ChiTietSanPhamViewModel
            {
                MaSp = ct.MaSp,
                MaSize = ct.MaSize, // Mã size
                TenSize = ct.MaSizeNavigation?.Ten ?? "Không xác định", // Lấy tên size
                SoLuong = ct.Soluong,
                Gia = ct.Gia,
                TongTien = ct.Tongtien,
                Ghichu = ct.Ghichu, // Ghi chú
                /*TenSP = ct.Ten */// Tên sản phẩm nếu cần
            }).ToList();
            var hoaDon = new HoaDonViewModel
            {
                MaPhieuonl = phieuDh.MaPhieuonl,
                Ngaygiodat = phieuDh.Ngaygiodat,
                TongTien = phieuDh.TongTien,
                DiaChi = phieuDh.DiaChi,
                TrangThai = phieuDh.TrangThai,
                Pttt = phieuDh.Pttt,
                TienShip = phieuDh.TienShip,
                SoXu= soxu,
                Ptnh = phieuDh.Ptnh,
                MaKh = phieuDh.MaKh,
                MaCn = phieuDh.MaCn,
                MaKm = phieuDh.MaKm,
                ChiTietSanPham = phieuDh.Ctsponls.ToList(),
                TenKhachHang = phieuDh.MaKhNavigation?.Ten ?? "Không xác định",
                EmailKhachHang = phieuDh.MaKhNavigation?.Email ?? "Không xác định",
                TenChiNhanh = phieuDh.MaCnNavigation?.Ten ?? "Không xác định",
                TenKhuyenMai = phieuDh.MaKmNavigation != null ? phieuDh.MaKmNavigation.Ten : "Không có khuyến mãi",
                Role = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == phieuDh.MaKh)?.Role ?? "Không xác định",

                GiaTrikm = phieuDh.MaKmNavigation?.GiaTri
                // Thêm các thông tin khác nếu cần
            };

            // Nếu ViewModel cần thêm thông tin từ các navigation properties

            return View(hoaDon); // Trả về view hiển thị hóa đơn
        }

        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }



        #region Paypal payment
        //[Authorize]
        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder([FromBody] PaypalOrderRequest request, CancellationToken cancellationToken)
        {
            //var maKm = "";
            var totalValue = "";


            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);
            if (/*request != null &&*/ request.TotalValue != 0)
            {
                totalValue = request.TotalValue.ToString();
            }
            else
            {
                totalValue = cart.Sum(q => q.Total).ToString();
            }
            if (cart == null || cart.Count == 0)
            {
                return BadRequest("Giỏ hàng trống.");
            }

            // Thông tin đơn hàng gửi qua Paypal
            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                // Tạo đơn hàng PayPal với tổng giá trị sau giảm
                var response = await _paypalClient.CreateOrder(totalValue, donViTienTe, maDonHangThamChieu);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }



        //[Authorize]
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, [FromBody] PaypalOrderRequest request, CancellationToken cancellationToken)
        {
            var totalValue = 0;
            var maCn = request.MaCn.ToString();
            var maKm = "";
            try
            {
                // Gọi PayPal API để capture order
                var response = await _paypalClient.CaptureOrder(orderID);

                // Tiến hành lưu hóa đơn với thông tin mã giảm giá
                List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key);
                if (request.TotalValue != 0)
                {
                    totalValue = (int)request.TotalValue;
                }
                else
                {
                    totalValue = (int)cart.Sum(q => q.Total);
                }
                if (cart == null || cart.Count == 0)
                {
                    return BadRequest("Giỏ hàng trống");
                }
                if (request.MaKm != "")
                {
                    maKm = request.MaKm;
                }
                else
                {
                    maKm = null;
                }


                // Lấy thông tin khách hàng từ Claims hoặc Session
                var makh = ""; //HttpContext.Session.GetString("UserPhone") ?? HttpContext.User.Claims.FirstOrDefault(c => c.Type == "MAKH")?.Value;
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
                    DiaChi = request.DiaChi ?? khachhang.Diachi,
                    Ngaygiodat = DateTime.Now,
                    Ptnh = "GRAB",
                    Pttt = "PayPal",
                    TrangThai = true,
                    MaKm = maKm, // Gán mã giảm giá vào hóa đơn
                    MaCn = maCn,
                    TongTien = (int)totalValue, // Gán tổng giá trị sau giảm
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
                        await _context.SaveChangesAsync();

                        // Lưu chi tiết hóa đơn
                        _context.AddRange(ctsp);
                        await _context.SaveChangesAsync();

                        decimal totalOrderValue = (decimal)totalValue; // Tổng giá trị hóa đơn đã tính phí ship
                        decimal bonusPoints = 0;

                        // Tùy vào role mà cộng phần trăm tương ứng
                        if (khachhang.Role == "Đồng")
                        {
                            bonusPoints = totalOrderValue * 0.01m;

                        }

                        else if (khachhang.Role == "Bạc")
                        {
                            bonusPoints = totalOrderValue * 0.02m;
                        }
                        else if (khachhang.Role == "Vàng")
                        {
                            bonusPoints = totalOrderValue * 0.03m;
                        }
                        // Cộng xu vào tài khoản khách hàng
                        if (khachhang.Xu == null)
                        {
                            khachhang.Xu = (int)bonusPoints;
                        }
                        else
                        {
                            khachhang.Xu += (int)bonusPoints;
                        }
                        _context.KhachHangs.Update(khachhang);
                        await _context.SaveChangesAsync();

                        int SoXu = (int)request.SoXu;
                        var khachHang = _context.KhachHangs
                        .Where(r => r.MaKh == makh)
                        .FirstOrDefault();
                        if (khachHang != null)
                        {
                            khachHang.Xu -= SoXu; // Giảm xu
                            _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                        }
                        var km = _context.DanhMucKms
                        .Where(r => r.MaKm == maKm)
                        .FirstOrDefault();
                        if (km != null)
                        {
                            km.Soluong--; // Giảm xu
                            _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                        }
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