using Manage_Coffee.Areas.Admin.Models;
using Manage_Coffee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manage_Coffee.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountAdminController : Controller
    {
        private readonly Cf2Context _context;
        public AccountAdminController(Cf2Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        // GET: Hiển thị form đăng ký
        [Route("Register-admin")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        // POST: Xử lý đăng ký
        [Route("Register-admin")]
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra số điện thoại đã tồn tại chưa
                var existingNhanVien = _context.NhanViens.FirstOrDefault(nv => nv.Sdt == model.Sdt);
                if (existingNhanVien != null)
                {
                    ViewBag.Error = "Số điện thoại đã tồn tại!";
                    return View(model);
                }

                // Tạo nhân viên mới
                var newNhanVien = new NhanVien
                {
                    MaNv = Guid.NewGuid().ToString().Substring(0, 5), // Chỉ lấy 5 ký tự đầu tiên
                    Ten = model.Ten,
                    Sdt = model.Sdt,
                    Mkhau = model.Mkhau,
                    Chucvu = model.Chucvu,
                    Diachi = model.Diachi,
                    Ngaysinh = DateTime.Now // Tạm thời sử dụng ngày hiện tại cho ngày sinh
                };

                _context.NhanViens.Add(newNhanVien);
                _context.SaveChanges();

                // Điều hướng tới trang login sau khi đăng ký thành công
                return RedirectToAction("LoginAdmin", "AccountAdmin", new { area = "Admin" });
            }

            return View(model);
        }
        [Route("Login-admin")]
        [HttpGet]
        public IActionResult LoginAdmin()
        {
            return View();
        }
        [Route("Login-admin")]
        [HttpPost]
        public IActionResult LoginAdmin(int sdt, string password)
        {
            // Tìm nhân viên dựa trên số điện thoại và mật khẩu
            var nhanVien = _context.NhanViens
                .FirstOrDefault(nv => nv.Sdt == sdt && nv.Mkhau == password);

            if (nhanVien != null)
            {
                // Lưu thông tin vào session
                HttpContext.Session.SetString("NhanVienSdt", nhanVien.Sdt.ToString());
                HttpContext.Session.SetString("NhanVienChucVu", nhanVien.Chucvu);
                HttpContext.Session.SetString("Ten", nhanVien.Ten);
                HttpContext.Session.SetString("Manv", nhanVien.MaNv);
                if(nhanVien.MaCn == null)
                {
                    nhanVien.MaCn = "CN001";
                }
                HttpContext.Session.SetString("Macn", nhanVien.MaCn);

                if (nhanVien.Chucvu == "Phục vụ")
                {
                    // Điều hướng đến PhucVuController (nếu trong Admin Area)
                    return RedirectToAction("Index", "PhucVu", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "QuanLy", new { area = "Admin" });
                }
            }

            ViewBag.Error = "Số điện thoại hoặc mật khẩu không chính xác";
            return View();
        }
        
    }
}
