using Manage_Coffee.Areas.Admin.Models;
using Manage_Coffee.Helpers;
using Manage_Coffee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Font;
using iText.Kernel.Font;
using static iText.Kernel.Font.PdfFontFactory;
using System.Globalization;


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
                return RedirectToAction("LoginAdmin", "AccountAdmin", new { area = "Admin" });
            }
            // Lấy giỏ hàng từ Session và truyền vào ViewBag
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            ViewBag.Cart = cart;
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
        public IActionResult Checkout(int soban, string pttt, string tenKhachHang, int sdt)
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
                Tongtien = cart.Sum(c => (c.Dongia + c.TriGia) * c.Soluong),
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

            // Lưu mã đơn hàng vào TempData để dùng ở trang CheckoutSuccess
            TempData["MaOrder"] = phieuOrder.MaOrder;

            // Chuyển hướng đến trang thành công
            return RedirectToAction("CheckoutSuccess");
        }
        public IActionResult CheckoutSuccess()
        {
            var maOrder = TempData["MaOrder"] as string; // Lấy mã đơn hàng từ TempData
            ViewBag.MaOrder = maOrder; // Truyền mã đơn hàng vào ViewBag
            return View();
        }
        [HttpPost]
        public IActionResult IncreaseQuantity(string productId, string idsize)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.ProductID == productId && c.SizeID == idsize);

            if (cartItem != null)
            {
                cartItem.Soluong++;
            }

            // Lưu lại giỏ hàng vào Session
            HttpContext.Session.Set("Cart", cart);
            return PartialView("_CartPartial", cart);
        }

        // Phương thức để giảm số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public IActionResult DecreaseQuantity(string productId, string idsize)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.ProductID == productId && c.SizeID == idsize);

            if (cartItem != null)
            {
                if (cartItem.Soluong > 1) // Không cho giảm xuống dưới 1
                {
                    cartItem.Soluong--;
                }
                else // Nếu số lượng là 1 thì xóa sản phẩm
                {
                    cart.Remove(cartItem);
                }
            }

            // Lưu lại giỏ hàng vào Session
            HttpContext.Session.Set("Cart", cart);
            return PartialView("_CartPartial", cart);
        }

        // Phương thức để xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public IActionResult RemoveFromCart(string productId, string idsize)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.ProductID == productId && c.SizeID == idsize);

            if (cartItem != null)
            {
                cart.Remove(cartItem); // Xóa sản phẩm khỏi giỏ hàng
            }

            // Lưu lại giỏ hàng vào Session
            HttpContext.Session.Set("Cart", cart);
            return PartialView("_CartPartial", cart);
        }

        // Hàm Download File PDF
        public IActionResult DownloadPdf(string maOrder)
        {
            if (string.IsNullOrEmpty(maOrder))
            {
                return BadRequest("Mã đơn hàng không hợp lệ.");
            }

            var phieuOrder = _context.PhieuOrders
                .Include(po => po.CtsanPhams)
                .FirstOrDefault(po => po.MaOrder == maOrder);

            if (phieuOrder == null)
            {
                return NotFound("Không tìm thấy đơn hàng.");
            }

            byte[] pdfBytes;

            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);
                // Khai báo font hỗ trợ Unicode

                PdfFont font = PdfFontFactory.CreateFont("C:/WINDOWS/Fonts/ARIAL.TTF", PdfEncodings.IDENTITY_H, EmbeddingStrategy.PREFER_EMBEDDED);

                // Sử dụng font cho tài liệu PDF
                document.SetFont(font);
                document.Add(new Paragraph("HÓA ĐƠN MUA HÀNG").SetFontSize(18).SetBold());
                document.Add(new Paragraph($"Mã đơn hàng: {phieuOrder.MaOrder}"));
                document.Add(new Paragraph($"Ngày đặt: {phieuOrder.Ngaygiodat}"));
                document.Add(new Paragraph($"Số bàn: {phieuOrder.Soban}"));
                document.Add(new Paragraph($"Tổng tiền: {phieuOrder.Tongtien.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))} VNĐ"));

                document.Add(new Paragraph("Chi tiết sản phẩm:").SetBold());
                foreach (var item in phieuOrder.CtsanPhams)
                {
                    document.Add(new Paragraph(
                        $"- Sản phẩm: {item.MaSp}, Số lượng: {item.Soluong}, Giá: {item.Gia.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))} VNĐ")
                    );
                }

                document.Close();
                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", $"HoaDon_{maOrder}.pdf");
        }


    }
}
