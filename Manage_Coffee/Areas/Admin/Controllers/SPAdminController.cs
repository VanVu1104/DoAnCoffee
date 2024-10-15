using Manage_Coffee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Manage_Coffee.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class SPAdminController : Controller
    {
        private readonly Cf2Context _context;
        public SPAdminController(Cf2Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var cf2Context = _context.SanPhams.Include(s => s.MaToppingNavigation).Include(s => s.MaloaiNavigation);
            return View(await cf2Context.ToListAsync());
        }
        public IActionResult Detail(string id)
        {
            // Lấy sản phẩm theo mã ID từ database
            var sanPham = _context.SanPhams.Find(id);

            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["LoaiList"] = _context.Loais.ToList();
            ViewData["ToppingList"] = _context.SanPhams
                .Where(sp => sp.Maloai == "L0003")
                .ToList() ?? new List<SanPham>();
            return View(sanPham);
        }
        [HttpPost]
        public IActionResult DeleteConfirmed(string id)
        {
            var sanPham = _context.SanPhams.Find(id);
            if (sanPham == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            // Xóa sản phẩm khỏi cơ sở dữ liệu
            _context.SanPhams.Remove(sanPham);
            _context.SaveChanges();

            return Json(new { success = true, message = "Xóa sản phẩm thành công." });
        }
        [HttpPost]
        public IActionResult ThayDoiTrangThaiAjax(string maSp)
        {
            var sanPham = _context.SanPhams.FirstOrDefault(sp => sp.MaSp == maSp);

            if (sanPham == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            // Đảo ngược trạng thái
            sanPham.TrangThai = !sanPham.TrangThai;

            // Lưu thay đổi
            _context.SaveChanges();

            // Trả về kết quả thành công và trạng thái mới
            return Json(new { success = true, trangThai = sanPham.TrangThai });
        }
        // GET: Hiển thị form thêm sản phẩm
        [HttpGet]
        public IActionResult Create1()
        {
            ViewData["LoaiList"] = _context.Loais.ToList();
            ViewData["ToppingList"] = _context.SanPhams
                .Where(sp => sp.Maloai == "L0003")
                .ToList() ?? new List<SanPham>();

            return View();
        }

        // POST: Xử lý thêm sản phẩm mới
        [HttpPost]
        public IActionResult Create1(string ten, int dongia, string dvt, string mota,
            string? anh, bool trangThai, string? maloai, string? maTopping)
        {
            // 1. Tạo mã sản phẩm ngẫu nhiên
            var maSp = $"SP{new Random().Next(100, 1000)}";

            // 2. Tạo đối tượng SanPham mới
            var sanPham = new SanPham
            {
                MaSp = maSp,
                Ten = ten,
                Dongia = dongia,
                Dvt = dvt,
                Mota = mota,
                Anh = anh,
                TrangThai = trangThai,
                Maloai = maloai,
                MaTopping = maTopping
            };

            // 3. Thêm sản phẩm vào database
            _context.SanPhams.Add(sanPham);
            _context.SaveChanges();

            // 4. Chuyển hướng về trang danh sách sản phẩm
            return RedirectToAction("Index");
        }
        // GET: Hiển thị form với thông tin sản phẩm cần chỉnh sửa
        [HttpGet]
        public IActionResult Edit(string id)
        {
            // Lấy sản phẩm theo mã ID từ database
            var sanPham = _context.SanPhams.Find(id);

            if (sanPham == null)
            {
                return NotFound();
            }

            // Load dữ liệu danh mục và topping
            ViewData["LoaiList"] = _context.Loais.ToList();
            ViewData["ToppingList"] = _context.SanPhams
                .Where(sp => sp.Maloai == "L0003")
                .ToList() ?? new List<SanPham>();

            return View(sanPham); // Truyền sản phẩm vào View để hiển thị
        }

        // POST: Cập nhật thông tin sản phẩm
        [HttpPost]
        public IActionResult Edit(string maSp, string ten, int dongia, string dvt,
                          string mota, string? anh, bool trangThai,
                          string? maloai, string? maTopping)
        {
            var sanPham = _context.SanPhams.Find(maSp);
            if (sanPham == null) return NotFound();

            // Cập nhật thông tin sản phẩm
            sanPham.Ten = ten;
            sanPham.Dongia = dongia;
            sanPham.Dvt = dvt;
            sanPham.Mota = mota;
            sanPham.Anh = anh;
            sanPham.TrangThai = trangThai;
            sanPham.Maloai = maloai;
            sanPham.MaTopping = maTopping;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
