using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Manage_Coffee.Models;

namespace Manage_Coffee.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamsAdminController : Controller
    {
        private readonly Cf2Context _context;

        public SanPhamsAdminController(Cf2Context context)
        {
            _context = context;
        }

        // GET: Admin/SanPhamsAdmin
        public async Task<IActionResult> Index()
        {
            var cf2Context = _context.SanPhams.Include(s => s.MaToppingNavigation).Include(s => s.MaloaiNavigation);
            return View(await cf2Context.ToListAsync());
        }

        // GET: Admin/SanPhamsAdmin/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
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

        // GET: Admin/SanPhamsAdmin/Create
        public IActionResult Create()
        {
            ViewData["MaTopping"] = new SelectList(_context.SanPhams, "MaSp", "MaSp");
            ViewData["Maloai"] = new SelectList(_context.Loais, "Maloai", "Maloai");
            return View();
        }

        // POST: Admin/SanPhamsAdmin/Create
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

        // GET: Admin/SanPhamsAdmin/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
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

        // POST: Admin/SanPhamsAdmin/Edit/5
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

        // GET: Admin/SanPhamsAdmin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
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

        // POST: Admin/SanPhamsAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
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
            return _context.SanPhams.Any(e => e.MaSp == id);
        }
    }
}
