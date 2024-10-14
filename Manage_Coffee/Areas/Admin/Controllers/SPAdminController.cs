using Manage_Coffee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult Detail()
        {

            return View();
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
        // GET: SanPhams/Create
        [HttpGet]
        public IActionResult Create1()
        {
            ViewData["MaTopping"] = new SelectList(_context.SanPhams, "MaSp", "MaSp");
            ViewData["Maloai"] = new SelectList(_context.Loais, "Maloai", "Maloai");
            return View();
        }
        [HttpPost]
        public IActionResult Create1(SanPham model)
        {
            if (ModelState.IsValid)
            {
                _context.SanPhams.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

    }
}
