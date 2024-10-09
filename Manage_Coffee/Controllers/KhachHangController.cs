using Microsoft.AspNetCore.Mvc;
using Manage_Coffee.Models;
using Manage_Coffee.Models.ViewModels;
using System.Linq;
using Manage_Coffee.Service;

namespace Manage_Coffee.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly KhachHangService _khachHangService;

        public KhachHangController(KhachHangService khachHangService)
        {
            _khachHangService = khachHangService;
        }

        public async Task<IActionResult> ThongTinKhachHang()
        {
            var maKh = "";
            if (HttpContext.Session.GetString("UserName") == null)
            {
                var makhClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "MAKH");
                maKh = makhClaim?.Value;
            }
            else
            {
                maKh = HttpContext.Session.GetString("UserPhone");
            }
            // maKh = User.FindFirst("maKh").Value;  // Lấy MaKh từ Claims khi người dùng đăng nhập
            var khachHang = await _khachHangService.GetKhachHangByIdAsync(maKh);

            return View(khachHang);
        }
        [HttpGet]
        public async Task<IActionResult> ChinhSuaThongTin(string id)
        {
            var khachHang = await _khachHangService.GetKhachHangByIdAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return View(khachHang);
        }

        // Xử lý khi người dùng cập nhật thông tin
        [HttpPost]
        public async Task<IActionResult> ChinhSuaThongTin(KhachHang khachHang)
        {
            //if (ModelState.IsValid)
            // {
            // Gọi dịch vụ để cập nhật khách hàng
            await _khachHangService.UpdateKhachHangAsync(khachHang);
            return RedirectToAction("ThongTinKhachHang", new { id = khachHang.MaKh });
            // }
            // return View("Index", "Home"); // Nếu có lỗi, trả lại view với model
        }

    }
}
