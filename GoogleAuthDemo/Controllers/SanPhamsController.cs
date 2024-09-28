using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GoogleAuthDemo.Models;

namespace GoogleAuthDemo.Controllers
{
    public class SanPhamsController : Controller
    {
        private readonly Cf2Context _context;

        public SanPhamsController(Cf2Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Sanpham1(string category)
        {
            
            IQueryable<SanPham> products = _context.SanPhams
                .Include(p => p.MaToppingNavigation)
                .Include(p => p.MaloaiNavigation);

           
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Maloai == category)
                                   .OrderByDescending(p => p.Maloai);
            }
            else
            {
          
                products = products.OrderByDescending(p => p.Ten);
            }

       
            return View(await products.ToListAsync());
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

            var sanPham = await _context.SanPhams
                .Include(s => s.MaToppingNavigation)
                .Include(s => s.MaloaiNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: SanPhams/Create
        public IActionResult Create()
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
        public async Task<IActionResult> Create([Bind("MaSp,Ten,Dongia,Dvt,Mota,Anh,Maloai,MaTopping")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTopping"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", sanPham.MaTopping);
            ViewData["Maloai"] = new SelectList(_context.Loais, "Maloai", "Maloai", sanPham.Maloai);
            return View(sanPham);
        }

        // GET: SanPhams/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.SanPhams == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["MaTopping"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", sanPham.MaTopping);
            ViewData["Maloai"] = new SelectList(_context.Loais, "Maloai", "Maloai", sanPham.Maloai);
            return View(sanPham);
        }

        // POST: SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSp,Ten,Dongia,Dvt,Mota,Anh,Maloai,MaTopping")] SanPham sanPham)
        {
            if (id != sanPham.MaSp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSp))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTopping"] = new SelectList(_context.SanPhams, "MaSp", "MaSp", sanPham.MaTopping);
            ViewData["Maloai"] = new SelectList(_context.Loais, "Maloai", "Maloai", sanPham.Maloai);
            return View(sanPham);
        }

        // GET: SanPhams/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.SanPhams == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaToppingNavigation)
                .Include(s => s.MaloaiNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.SanPhams == null)
            {
                return Problem("Entity set 'Cf2Context.SanPhams'  is null.");
            }
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(string id)
        {
          return (_context.SanPhams?.Any(e => e.MaSp == id)).GetValueOrDefault();
        }

    }
}
