using Manage_Coffee.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace Manage_Coffee.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ListOrderController : Controller
    {
        private readonly Cf2Context _context;
        private readonly IAntiforgery _antiforgery;

        public ListOrderController(IAntiforgery antiforgery, Cf2Context context)
        {
            _antiforgery = antiforgery;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("List-order")]
        public IActionResult Orders()
        {
            var TenNhanVien = HttpContext.Session.GetString("Ten");
            if (string.IsNullOrEmpty(TenNhanVien))
            {
                return RedirectToAction("LoginAdmin", "AccountAdmin", new { area = "Admin" });
            }
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            ViewBag.AntiForgeryToken = tokens.RequestToken;
            var orders = LoadOrders();

            if (orders == null || !orders.Any()) // Kiểm tra nếu không có đơn hàng
            {
                ViewBag.Message = "Không có đơn đặt hàng nào.";
            }

            return View(orders);
        }

        public List<Phieudhonl> LoadOrders()
        {
            var listOrders = _context.Phieudhonls.ToList();
            return listOrders; // Trả về danh sách đơn hàng
        }

        [HttpPost]
        public IActionResult ConfirmOrder([FromBody] string orderId)
        {
            var order = _context.Phieudhonls.FirstOrDefault(o => o.MaPhieuonl == orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Đơn hàng không tồn tại." });
            }

            // Cập nhật trạng thái đơn hàng từ false thành true
            order.TrangThai = true;
            try
            {
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi cập nhật đơn hàng: " + ex.Message });
            }
        }
        [HttpPost]
        public IActionResult CancelOrder([FromBody] string orderId)
        {
            var order = _context.Phieudhonls.FirstOrDefault(o => o.MaPhieuonl == orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Đơn hàng không tồn tại." });
            }

            // Cập nhật trạng thái đơn hàng thành null hoặc trạng thái "Đã hủy"
            order.TrangThai = null;
            try
            {
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi cập nhật đơn hàng: " + ex.Message });
            }
        }

    }
}
