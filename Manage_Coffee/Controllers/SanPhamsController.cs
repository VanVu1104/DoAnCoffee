using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Manage_Coffee.Models;
using System.ComponentModel;
using Manage_Coffee.ViewModels;
using X.PagedList;

namespace Manage_Coffee.Controllers
{
    public class SanPhamsController : Controller
    {
        private readonly Cf2Context _context;

        public SanPhamsController(Cf2Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Sanpham1(string category, int? page)
        {
            int pageSize = 3;
            int pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var lst = _context.SanPhams.AsNoTracking();
            if (!string.IsNullOrEmpty(category))
            {
                lst = lst.Where(x => x.Maloai == category);
            }
            ViewBag.Query = new Dictionary<string, string> { { "category", category } };
            var pagedList = lst.OrderBy(x => x.Ten)
                       .ToPagedList(pageNumber, pageSize);
            if (pagedList == null)
            {
                return NotFound();
            }
            return View(pagedList);
        }



        public async Task<IActionResult> Index()
        {
            var cf2Context = _context.SanPhams.Include(s => s.MaToppingNavigation).Include(s => s.MaloaiNavigation);
            return View(await cf2Context.ToListAsync());
        }

        // GET: SanPhams/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.SanPhams == null)
            {
                return NotFound();
            }

            // Lấy thông tin sản phẩm
            var sanPham = await _context.SanPhams
                .Include(s => s.MaToppingNavigation)
                .Include(s => s.MaloaiNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            var toppings = await _context.SanPhams
           .Where(l => l.Maloai == "L0003")
           .ToListAsync();

            // Tạo ViewModel
            var viewModel = new ProductDetailViewModel
            {
                Product = sanPham,
                Toppings = toppings
            };

            return View(viewModel);
        }

        // GET: SanPhams/Create
        public IActionResult Create1()
        {
            ViewData["MaTopping"] = new SelectList(_context.SanPhams, "MaSp", "MaSp");
            ViewData["Maloai"] = new SelectList(_context.Loais, "Maloai", "Maloai");
            return View();
        }

        // POST: SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create1([Bind("MaSp,Ten,Dongia,Dvt,Mota,Anh,Maloai,MaTopping")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                // Nếu MaTopping không có giá trị, hãy đặt nó thành null
                if (string.IsNullOrEmpty(sanPham.MaTopping))
                {
                    sanPham.MaTopping = null; // Không cần thiết, nhưng có thể thêm để rõ ràng
                }

                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, tái tạo danh sách chọn cho MaTopping và Maloai
            ViewData["MaTopping"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", sanPham.MaTopping);
            ViewData["Maloai"] = new SelectList(_context.Loais, "Maloai", "Maloai", sanPham.Maloai);
            return View(sanPham);
        }





        private bool SanPhamExists(string id)
        {
            return (_context.SanPhams?.Any(e => e.MaSp == id)).GetValueOrDefault();
        }

    }
}
