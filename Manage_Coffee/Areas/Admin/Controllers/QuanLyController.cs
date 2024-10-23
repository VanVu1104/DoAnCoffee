using Manage_Coffee.Areas.Admin.Models;
using Manage_Coffee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace Manage_Coffee.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class QuanLyController : Controller
	{

		private readonly Cf2Context _context;

		public QuanLyController(Cf2Context context)
		{
			_context = context;
		}
		public IActionResult Index()
		{

			var manv = HttpContext.Session.GetString("NhanVienPhucVu");
			if(manv == null)
			{
				return RedirectToAction("LoginAdmin", "AccountAdmin", new { area = "Admin" });
			}
			var nvl = LoadNvl();
			return View(nvl);
		}
		public List<NguyenVatLieu> LoadNvl()
		{
			var nvl = _context.NguyenVatLieus.ToList();
			return nvl;
		}
		// Phương thức GET để hiển thị form thêm nguyên vật liệu
		public IActionResult Create()
		{
			var nvl = new NguyenVatLieu
			{
				MaNvl = Guid.NewGuid().ToString().Substring(0, 5),
			}; // Khởi tạo đối tượng mới
			return View(nvl); // Trả về view với model mới
		}
		//Hàm load nhân viên
		public IActionResult LoadEmployee()
		{
			var employee = Employee();
			return View(employee);
		}
		public List<NhanVien> Employee()
		{
			var MaCn = HttpContext.Session.GetString("MaCn");
			if(MaCn == null)
			{
				MaCn = "CN001";
			}
            var listEmployee = _context.NhanViens
				.Where(cv => cv.Chucvu == "Phục vụ" && cv.MaCn == MaCn)
				.ToList();
            if (listEmployee == null || !listEmployee.Any())
            {
                throw new Exception("Danh sách nhân viên không tồn tại!");
            }

            return listEmployee;
        }
		// Phương thức POST để xử lý form khi người dùng submit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(NguyenVatLieu model)
		{
			if (ModelState.IsValid)
			{
				// Kiểm tra xem mã nguyên vật liệu đã tồn tại chưa
				var existingNVL = _context.NguyenVatLieus.FirstOrDefault(nvl => nvl.MaNvl == model.MaNvl);
				if (existingNVL != null)
				{
					ModelState.AddModelError("MaNvl", "Mã nguyên vật liệu đã tồn tại.");
					return View(model);
				}

				// Tạo mới nguyên vật liệu
				var nguyenVatLieu = new NguyenVatLieu
				{
					MaNvl = model.MaNvl,
					Ten = model.Ten,
					Dongia = model.Dongia,
					Dvt = model.Dvt,
					Mota = model.Mota,
					Anh = model.Anh
				};

				// Lưu nguyên vật liệu vào cơ sở dữ liệu
				_context.NguyenVatLieus.Add(nguyenVatLieu);
				_context.SaveChanges();

				// Điều hướng về trang danh sách nguyên vật liệu (hoặc trang khác)
				return RedirectToAction("Index");
			}

			// Nếu dữ liệu không hợp lệ, trả về lại view với lỗi
			return View(model);
		}
		public IActionResult CreateMaterial()
		{
			ViewBag.NguyenVatLieu = _context.NguyenVatLieus.ToList(); // Load danh sách NVL
			ViewBag.NhaCungCapList = _context.Nhacungcaps.ToList(); // Load danh sách NVL
			return View();
		}

		// POST: Xử lý tạo phiếu nhập/xuất
		[HttpPost]
		public IActionResult CreateMaterial(
		string loai,
		List<string> maNvlList,
		List<int> soLuongList,
		string diaChi,
		 string? maCcap)
		{
			// Lấy mã nhân viên từ session
			var maNv = HttpContext.Session.GetString("Manv");
			if (string.IsNullOrEmpty(maNv))
			{
				ModelState.AddModelError("", "Không tìm thấy mã nhân viên. Vui lòng đăng nhập lại.");
				return View();
			}

			// 1. Tạo phiếu mới
			var phieu = new PhieuNhapXuat
			{
				MaPhieu = GenerateMaPhieu(),  // Sinh mã phiếu từ hàm GenerateMaPhieu
				NgayLap = DateTime.Now,
				Loai = loai,
				Diachi = diaChi,
				MaNv = maNv,
				MaCcap = maCcap
			};
			_context.PhieuNhapXuats.Add(phieu);
			_context.SaveChanges();

			// 2. Lưu chi tiết phiếu cho từng sản phẩm
			for (int i = 0; i < maNvlList.Count; i++)
			{
				var ctPhieu = new Ctphieu
				{
					MaPhieu = phieu.MaPhieu,
					MaNvl = maNvlList[i],
					Soluong = soLuongList[i]
				};
				_context.Ctphieus.Add(ctPhieu);

				// 3. Cập nhật số lượng tồn kho của từng nguyên vật liệu
				var nvl = _context.NguyenVatLieus.Find(maNvlList[i]);
				if (nvl != null)
				{
					if (loai.ToLower() == "nhap")
					{
						nvl.SoLuong += soLuongList[i];
					}
					else if (loai.ToLower() == "xuat")
					{
						if (nvl.SoLuong >= soLuongList[i])
						{
							nvl.SoLuong -= soLuongList[i];
						}
						else
						{
							ModelState.AddModelError("",
								$"Số lượng tồn kho của {nvl.Ten} không đủ để xuất.");
							return View();
						}
					}
				}
			}

			_context.SaveChanges();  // Lưu tất cả thay đổi vào DB
			return RedirectToAction("Index");
		}
		// Hàm load phiếu nhập xuất
        public IActionResult LoadPhieuNhapXuat()
        {
            var phieuList = GetPhieuNhapXuat();
            if (phieuList == null || !phieuList.Any())
            {
                throw new Exception("Không có phiếu nhập xuất.");
            }

            return View(phieuList); // Truyền danh sách phiếu vào View
        }

        public List<PhieuNhapXuat> GetPhieuNhapXuat()
        {
            var phieuList = _context.PhieuNhapXuats
			.Select(phieu => new PhieuNhapXuat
        {
            MaPhieu = phieu.MaPhieu,
            NgayLap = phieu.NgayLap,
            Loai = phieu.Loai,
                Diachi = phieu.Diachi,
            MaNv = _context.NhanViens
                .Where(nv => nv.MaNv == phieu.MaNv)
                .Select(nv => nv.Ten)
                .FirstOrDefault() ?? "Không rõ", 

            MaCcap = _context.Nhacungcaps
                .Where(ncc => ncc.MaCcap == phieu.MaCcap)
                .Select(ncc => ncc.Ten)
                .FirstOrDefault() ?? "Không rõ"  
			}).ToList();
            if (!phieuList.Any() || phieuList == null)
            {
                throw new Exception("Không có phiếu nhập xuất.");
            }
            return phieuList;
        }
        // Hàm tạo mã phiếu PNX + 2 số ngẫu nhiên
        private string GenerateMaPhieu()
		{
			Random random = new Random();
			int randomNumber = random.Next(10, 99); // Sinh số ngẫu nhiên từ 10 đến 99
			return $"PNX{randomNumber}";
		}
		
	}
}
