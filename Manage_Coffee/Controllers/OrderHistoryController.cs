using Manage_Coffee.Helpers;
using Manage_Coffee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Manage_Coffee.Controllers
{
    public class OrderHistoryController : Controller
    {
        private readonly Cf2Context _context;
        public OrderHistoryController(Cf2Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var makh = "";
            if (HttpContext.Session.GetString("UserName") == null)
            {
                var makhClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "MAKH");
                makh = makhClaim?.Value;
            }
            else
            {
                makh = HttpContext.Session.GetString("UserPhone");
            }
            var khachHang = _context.Phieudhonls
                  .Where(o => o.MaKh == makh)
           .OrderByDescending(o => o.Ngaygiodat)
                .ToList();
            // Lấy danh sách đơn hàng của khách hàng dựa vào mã khách hàng
           // Sắp xếp theo ngày đặt
     
            return View(khachHang);
        }
        public IActionResult Detail(string maPhieuonl)
        {
           
            // Lấy chi tiết đơn hàng
            var orderDetail = _context.Ctsponls
                 .Include(d => d.MaSpNavigation) 
                 .Include(d => d.MaSizeNavigation)
                .Where(d => d.MaPhieuonl == maPhieuonl)
                .ToList();
            if (orderDetail == null || !orderDetail.Any())
            {
                return NotFound();
            }
            ViewBag.MaPhieuonl = maPhieuonl;
            // Ghi log thông tin đơn hàng
            foreach (var item in orderDetail)
            {
                if (item.MaSpNavigation == null)
                {
                    Console.WriteLine($"Sản phẩm với mã {item.MaSp} không tìm thấy.");
                }
            }
            return View(orderDetail);
        }
        [HttpPost]
        public async Task<IActionResult> Reorder(string maPhieuonl)
        {
            // Lấy chi tiết đơn hàng dựa trên mã phiếu đơn lẻ
            var orderDetails = await _context.Ctsponls
                .Include(d => d.MaSpNavigation) // Bao gồm thông tin sản phẩm
                .Include(d => d.MaSizeNavigation) // Bao gồm thông tin kích thước
                .Where(d => d.MaPhieuonl == maPhieuonl)
                .ToListAsync();

            if (orderDetails == null || !orderDetails.Any())
            {
                return NotFound(); // Trả về NotFound nếu không có đơn hàng
            }

            List<CartItem> Cart = HttpContext.Session.Get<List<CartItem>>(MySetting.Cart_key) ?? new List<CartItem>();

            foreach (var item in orderDetails)
            {
                // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
                var cartItem = Cart.FirstOrDefault(c => c.ProductID == item.MaSp && c.SizeID == item.MaSize);
                if (cartItem == null)
                {
                    // Tạo mới cartItem và thêm vào giỏ hàng
                    cartItem = new CartItem(item.MaSpNavigation, item.MaSizeNavigation, item.Ghichu)
                    {
                        Soluong = item.Soluong // Số lượng từ đơn hàng
                    };
                    Cart.Add(cartItem);
                }
                else
                {
                    // Nếu sản phẩm đã có, tăng số lượng
                    cartItem.Soluong += item.Soluong;
                    cartItem.Ghichu = item.Ghichu; // Cập nhật ghi chú
                }
            }

            // Lưu giỏ hàng vào session
            HttpContext.Session.Set(MySetting.Cart_key, Cart);

            return RedirectToAction("Cart", "Cart"); // Chuyển hướng đến trang giỏ hàng
        }

    }
}
