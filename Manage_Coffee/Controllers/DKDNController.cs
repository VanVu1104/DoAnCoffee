using Manage_Coffee.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Manage_Coffee.Controllers
{
    public class DKDNController : Controller
    {
        private readonly Cf2Context _context;

        public DKDNController(Cf2Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> Register(DKSDT model)
        {
            if (ModelState.IsValid)
            {
                var existingCustomer = await _context.KhachHangs.FirstOrDefaultAsync(c => c.Sdt == model.Sdt);
                if (existingCustomer != null)
                {
                    ModelState.AddModelError(string.Empty, "Số điện thoại đã tồn tại.");
                    return View(model);
                }

                // Kiểm tra mật khẩu nhập lại
                if (model.Matkhau != model.MatkhauNhapLai)
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu và mật khẩu nhập lại không khớp.");
                    return View(model);
                }

                var khachHang = new KhachHang
                {
                    MaKh = model.Sdt.ToString(),
                    Sdt = model.Sdt,
                    Matkhau = model.Matkhau, // Không băm mật khẩu
                    Ten = model.Ten,
                    Diachi = model.Diachi,
                    Xu=0,
                    Role = "User"
                };

                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            // Ghi lại lỗi nếu ModelState không hợp lệ
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
            Console.WriteLine(string.Join(", ", errors));

            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        //public async Task<IActionResult> Login(DNSDT model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(c => c.Sdt == model.Sdt);
        //        if (khachHang != null && khachHang.Matkhau == model.Matkhau)
        //        {
        //            var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, khachHang.Ten),
        //        new Claim("Sdt", khachHang.Sdt.ToString())
        //    };

        //            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        //            // Đăng nhập và thiết lập cookie
        //            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        //            // Ghi log hoặc thông báo người dùng

        //            return RedirectToAction("Index", "Home");
        //        }
        //        ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
        //    }
        //    return View(model);
        //}
        public async Task<IActionResult> Login(DNSDT model)
        {
            if (ModelState.IsValid)
            {
                var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(c => c.Sdt == model.Sdt);
                if (khachHang != null && khachHang.Matkhau == model.Matkhau)
                {
                    // Lưu thông tin người dùng vào Session
                    HttpContext.Session.SetString("UserName", khachHang.Ten);
                    HttpContext.Session.SetString("UserPhone", khachHang.Sdt.ToString());

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
            }
            return View(model);
        }





        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa tất cả thông tin trong Session
            return RedirectToAction("Login", "DKDN");
        }

    }
}
