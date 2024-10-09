using Manage_Coffee.Areas.Admin.Models;
using Manage_Coffee.Helpers;
using Manage_Coffee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Manage_Coffee.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhucVuController : Controller
    {

        private readonly Cf2Context _context;

        public PhucVuController(Cf2Context context)
        {
            _context = context;
        }
        public IActionResult Index(string keyword)
        {
            // Lấy thông tin nhân viên từ Session
            var TenNhanVien = HttpContext.Session.GetString("Ten");
            var MaNhanVien = HttpContext.Session.GetString("Manv");
            if (string.IsNullOrEmpty(TenNhanVien))
            {
                return RedirectToAction("Login", "AccountAdmin", new { area = "Admin" });
            }
            // Load danh sách sản phẩm khi truy cập vào trang Index
            var products = LoadProducts(keyword);

            ViewBag.TenNhanVien = TenNhanVien;
            return View(products);
        }
        // Phương thức load sản phẩm từ cơ sở dữ liệu
        private List<SanPham> LoadProducts(string keyword)
        {
            var products = string.IsNullOrEmpty(keyword)
                 ? _context.SanPhams.ToList()
                 : _context.SanPhams.Where(p => p.Ten.ToLower().Contains(keyword.ToLower())).ToList();
            // Kiểm tra nếu không tìm thấy sản phẩm nào
            if (products == null || products.Count == 0)
            {
                ViewBag.Message = "Không tìm thấy sản phẩm nào.";
            }
            else
            {
                ViewBag.Message = null; // Reset thông báo nếu có sản phẩm
            }
            return products;
        }
        // Phương thức để thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public IActionResult AddToCart(string productId, string idsize)
        {
            var product = _context.SanPhams.FirstOrDefault(p => p.MaSp == productId);
            Size size = _context.Sizes.Find(idsize);
            if (product == null || size == null)
            {
                return NotFound();
            }

            // Lấy giỏ hàng từ Session, nếu không có thì tạo mới
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
            var cartItem = cart.Where(c => c.ProductID == productId && c.SizeID == idsize).FirstOrDefault();
            if (cartItem == null)
            {
                cart.Add(new CartItem
                {
                    ProductID = productId,
                    Ten = product.Ten,
                    Dongia = product.Dongia,
                    SizeID = idsize,
                    TriGia = size.TriGia,
                    Soluong = 1
                });
            }
            else
            {
                cartItem.Soluong++;
            }

            // Lưu lại giỏ hàng vào Session
            HttpContext.Session.Set("Cart", cart);

            // Trả về partial view của giỏ hàng để cập nhật
            return PartialView("_CartPartial", cart);
        }
        [Route("Logout-admin")]
        public IActionResult LogoutAdmin()
        {
            // Xóa tất cả session của người dùng
            HttpContext.Session.Clear();

            // Điều hướng về trang login hoặc trang chính sau khi logout
            return RedirectToAction("LoginAdmin", "AccountAdmin", new { area = "Admin" });
        }
        // Thêm hành động Checkout để hiển thị trang Checkout
        [Route("Checkout")]
        public IActionResult Checkout()
        {
            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");

            // Kiểm tra nếu giỏ hàng trống
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index");
            }

            // Lấy thông tin nhân viên từ Session
            var maNv = HttpContext.Session.GetString("Manv");
            var tenNhanVien = HttpContext.Session.GetString("Ten");

            // Tạo ViewModel để truyền dữ liệu đến view
            var viewModel = new CheckoutViewModel
            {
                CartItems = cart,
                TenNhanVien = tenNhanVien,
                MaNv = maNv,
                TongTien = cart.Sum(c => c.Dongia * c.Soluong + c.TriGia)
            };

            return View(viewModel);
        }
        [HttpPost]
        [Route("Checkout")]
        public IActionResult ConfirmCheckout(int soban, string pttt, string tenKhachHang, int sdt)
        {
            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");

            // Kiểm tra nếu giỏ hàng trống
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index");
            }

            // Lấy thông tin nhân viên từ Session
            var maNv = HttpContext.Session.GetString("Manv");
            var maCn = HttpContext.Session.GetString("Macn");

            // Tạo đối tượng PhieuOrder
            var phieuOrder = new PhieuOrder
            {
                MaOrder = Guid.NewGuid().ToString().Substring(0, 5),
                Ngaygiodat = DateTime.Now,
                Soban = soban,  // Số bàn từ người dùng
                Tongtien = cart.Sum(c => (c.Dongia + c.TriGia) * c.Soluong ),
                Trangthai = true,  // Thanh toán thành công
                Pttt = pttt,  
                MaNv = maNv,  
                MaCn = maCn,  
                MaKm = null,  
                Ten = tenKhachHang,  
                Sdt = sdt,
            };
            foreach (var item in cart)
            {
                var ctsanPham = new CtsanPham
                {
                    MaOrder = phieuOrder.MaOrder,  // Liên kết với mã đơn hàng
                    MaSp = item.ProductID,  // Mã sản phẩm
                    Soluong = item.Soluong,  // Số lượng
                    Gia = item.Dongia,  // Đơn giá sản phẩm
                    MaKh = maNv,
                    MaSize = item.SizeID,
                    TongTien = item.Soluong * (item.Dongia + item.TriGia),

                };
                _context.CtsanPhams.Add(ctsanPham);
            }
            // Thêm đơn hàng vào database
            _context.PhieuOrders.Add(phieuOrder);

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            // Xóa giỏ hàng sau khi thanh toán thành công
            HttpContext.Session.Remove("Cart");

            // Chuyển hướng đến trang thành công
            return RedirectToAction("CheckoutSuccess");
        }
        public IActionResult CheckoutSuccess()
        {
            return View();
        }
       

    }

}
