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
        // Đăng ký tài khoản nhân viên của quản lý
        [Route("Register-admin")]
        [HttpGet]
        public IActionResult Register()
        {
            var position = HttpContext.Session.GetString("NhanVienChucVu");
            var maCn = HttpContext.Session.GetString("MaCn");

            ViewBag.ShowManagerOption = position != null && position == "Quản lý tổng";
            ViewBag.BranchCode = maCn; // Lấy mã chi nhánh nếu là quản lý chi nhánh

            return View();
        }
        // POST: Xử lý cấp tài khoản
        [Route("Register-admin")]
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            var position = HttpContext.Session.GetString("NhanVienChucVu");
            var maCn = HttpContext.Session.GetString("MaCn") ?? throw new Exception("Mã chi nhánh không tồn tại");
            // Xử lý mã chi nhánh tùy theo loại quản lý
            string finalMaCn = (position == "Quản lý tổng" ? model.inputMaCn : maCn) ?? throw new Exception("Mã chi nhánh không tồn tại");
            if (ModelState.IsValid)
            {
                // Kiểm tra số điện thoại đã tồn tại chưa
                var existingNhanVien = _context.NhanViens.FirstOrDefault(nv => nv.Sdt == model.Sdt);
                if (existingNhanVien != null)
                {
                    ViewBag.Error = "Số điện thoại đã tồn tại!";
                    return View(model); // Hiển thị lại form với lỗi
                }

                // Tạo nhân viên mới
                var newNhanVien = new NhanVien
                {
                    MaNv = Guid.NewGuid().ToString().Substring(0, 5), // Lấy 5 ký tự đầu
                    Ten = model.Ten,
                    Sdt = model.Sdt,
                    Mkhau = model.Mkhau,
                    Chucvu = model.Chucvu,
                    Diachi = model.Diachi,
                    Ngaysinh = model.NgaySinh,
                    GioiTinh = model.GioiTinh,
                    MaCn = finalMaCn,
                };

                _context.NhanViens.Add(newNhanVien);
                _context.SaveChanges();

                // Gửi thông báo thành công qua TempData
                TempData["Success"] = "Tạo tài khoản thành công!";

                // Giữ nguyên trang và làm trống model để người dùng tạo thêm tài khoản mới
                ModelState.Clear();
                return View();
            }

            return View(model); // Nếu có lỗi thì hiển thị lại form với dữ liệu cũ
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
                HttpContext.Session.SetString("MaCn", nhanVien.MaCn);
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
