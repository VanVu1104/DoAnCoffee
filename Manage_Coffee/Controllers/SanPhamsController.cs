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

using Manage_Coffee.Models;
using Manage_Coffee.ViewModels;
using Manage_Coffee.Helpers;


namespace Manage_Coffee.Controllers
{
    public class SanPhamsController : Controller
    {
        private readonly Cf2Context _context;

        public SanPhamsController(Cf2Context context)
        {
            _context = context;
        }
		//public async Task<IActionResult> Sanpham1(string category, int? page)
		//{
		//    int pageSize = 3;
		//    int pageNumber = page == null || page <= 0 ? 1 : page.Value;
		//    var lst = _context.SanPhams.AsNoTracking();
		//    if (!string.IsNullOrEmpty(category))
		//    {
		//        lst = lst.Where(x => x.Maloai == category);
		//    }
		//    ViewBag.Query = new Dictionary<string, string> { { "category", category } };
		//    var pagedList = lst.OrderBy(x => x.Ten)
		//               .ToPagedList(pageNumber, pageSize);
		//    if (pagedList == null)
		//    {
		//        return NotFound();
		//    }
		//    return View(pagedList);
		//}
		public async Task<IActionResult> Sanpham1(string category, int? page)
		{
			int pageSize = 8;
			int pageNumber = page == null || page <= 0 ? 1 : page.Value;
			var lst = _context.SanPhams.AsNoTracking();
			if (!string.IsNullOrEmpty(category))
			{
				lst = lst.Where(x => x.Maloai == category);
			}
			var productWithRatings = from sp in lst
									 join rating in _context.ChitietDanhGia
									 on sp.MaSp equals rating.MaSp into temp
									 from ratingGroup in temp.DefaultIfEmpty()
									 group ratingGroup by sp into g
									 select new
									 {
										 SanPham = g.Key,
										 AverageRating = g.Average(r => r.SoSao) // Tính trung bình số sao
									 };
			ViewBag.Query = new Dictionary<string, string> { { "category", category } };
			var sortedProducts = productWithRatings
						 .OrderByDescending(x => x.AverageRating)
						 .Select(x => x.SanPham);
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
            var sanPham = await _context.SanPhams
                  .Include(s => s.KitsAsKit)  // Bao gồm các kit mà sản phẩm là sản phẩm chính
                  .ThenInclude(k => k.MaSpNavigation) // Bao gồm sản phẩm đi kèm trong combo
                  .Include(s => s.KitsAsSp)   // Bao gồm các kit mà sản phẩm là sản phẩm đi kèm
                  .ThenInclude(k => k.MaKitNavigation) // Bao gồm sản phẩm chính trong combo
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
				Toppings = toppings,
                KitsAsKit = sanPham.KitsAsKit.ToList(),
                KitsAsSp = sanPham.KitsAsSp.ToList()
            };
			// Lấy sản phẩm từ database
			var product = _context.SanPhams
				.FirstOrDefault(p => p.MaSp == id);

			if (product == null)
			{
				return NotFound();
			}
			// Tính toán số sao trung bình và số lượt đánh giá
			var ratings = _context.ChitietDanhGia
				.Where(r => r.MaSp == id)
				.ToList();
			double averageRating = 0;
			int ratingCount = 0;
			if (ratings != null)
			{
				 averageRating = ratings.Any() ? Math.Round(ratings.Average(r => r.SoSao), 2) : 0;
				 ratingCount = ratings.Count;
			}	
			

			// Sử dụng ViewBag để truyền dữ liệu đến view
			ViewBag.AverageRating = averageRating;
			ViewBag.RatingCount = ratingCount;
			ViewBag.MaSp = id;

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
		[SessionOrAuthorize]
		public IActionResult DanhGia(string ten, decimal dongia, string masp)
		{
			// Truyền dữ liệu đến View để hiển thị
			ViewBag.TenSanPham = ten;
			ViewBag.DonGia = dongia;
			ViewBag.MaSp = masp;

			// Lấy mã khách hàng từ session hoặc từ claims
			var maKh = HttpContext.Session.GetString("UserPhone") ?? HttpContext.User.Claims.FirstOrDefault(c => c.Type == "MAKH")?.Value;

			if (string.IsNullOrEmpty(maKh))
			{
				return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang đăng nhập nếu không có mã khách hàng
			}
			var nhanxet = _context.ChitietDanhGia
				.Where(r => r.MaKh == maKh && r.MaSp == masp)
				.Select(r => r.NhanXet)
				.FirstOrDefault();
			// Lấy đánh giá của khách hàng từ cơ sở dữ liệu
			var sosao = _context.ChitietDanhGia
				.Where(r => r.MaKh == maKh && r.MaSp == masp)
				.Select(r => r.SoSao)
				.FirstOrDefault();



			// Kiểm tra xem khách hàng đã mua sản phẩm chưa
			var hasPurchased = _context.Ctsponls
				.Any(c => c.MaSp == masp && c.MaKh == maKh);
			if (hasPurchased == false)
			{
				sosao = 0;
			}
			ViewBag.NhanXet = nhanxet ?? "";
			ViewBag.SoSao = sosao;
			ViewBag.HasPurchased = hasPurchased;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SubmitRating(int soSao, string maSp, string nhanXet)
		{
			if (soSao < 1 || soSao > 5)
			{
				return BadRequest("Số sao không hợp lệ.");
			}

			var maKh = HttpContext.Session.GetString("UserPhone") ??
						HttpContext.User.Claims.FirstOrDefault(c => c.Type == "MAKH")?.Value;

			if (string.IsNullOrEmpty(maKh))
			{
				return RedirectToAction("Login", "Account");
			}

			// Kiểm tra xem khách hàng đã mua sản phẩm chưa
			var hasPurchased = _context.Ctsponls.Any(c => c.MaSp == maSp && c.MaKh == maKh);
			if (!hasPurchased)
			{
				// Sử dụng JavaScript để hiển thị alert
				return Content("<script>alert('\\u0042\\u1ea1\\u006e \\u0063\\u0068\\u01b0\\u1ea3 \\u006d\\u0075\\u0061 \\u0073\\u1ea3\\u006e \\u0070\\u0068\\u1ea9\\u006d \\u006e\\u00e0\\u0079 \\u006e\\u00ean \\u006b\\u0068\\u00f4\\u006e\\u0067 \\u0074\\u1ec3 \\u0111\\u00e1nh \\u0067\\u0069\\u00e1.'); window.history.back();</script>", "text/html");
			}

			var existingRating = _context.ChitietDanhGia
				.FirstOrDefault(r => r.MaSp == maSp && r.MaKh == maKh);

			if (existingRating != null)
			{
				// Cập nhật đánh giá
				existingRating.SoSao = soSao;
				existingRating.NhanXet = nhanXet;
				_context.ChitietDanhGia.Update(existingRating);
			}
			else
			{
				// Tạo mới đánh giá
				var rating = new ChitietDanhGium
				{
					SoSao = soSao,
					MaSp = maSp,
					MaKh = maKh,
					NhanXet = nhanXet

				};

				_context.ChitietDanhGia.Add(rating);
			}

			_context.SaveChanges();

			return RedirectToAction("Details", new { id = maSp });
		}
		public IActionResult DanhSachDanhGia(string maSp)
		{
			// Lấy danh sách đánh giá của sản phẩm
			var danhSachDanhGia = _context.ChitietDanhGia
				.Where(dg => dg.MaSp == maSp)
				.Select(dg => new {
					TenKhachHang = dg.MaKhNavigation.Ten, // Lấy tên khách hàng từ bảng KhachHang
					SoSao = dg.SoSao,
					NhanXet = dg.NhanXet
				}).ToList();
			// Truyền danh sách đánh giá vào ViewBag hoặc ViewModel
			ViewBag.DanhSachDanhGia = danhSachDanhGia;
			ViewBag.MaSp = maSp;

			return View();
		}





		private bool SanPhamExists(string id)
        {
            return (_context.SanPhams?.Any(e => e.MaSp == id)).GetValueOrDefault();
        }

    }
}
