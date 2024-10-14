using Manage_Coffee.Areas.Admin.Models;
using Manage_Coffee.Helpers;
using Manage_Coffee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Manage_Coffee.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongKeTongController : Controller
    {
        private readonly Cf2Context _context;

        public ThongKeTongController(Cf2Context context)
        {
            _context = context;
        }

        public IActionResult Index(int thang = 0, int nam = 0, string loaiBanHang = "all", string loaiHienThi = "doanhthu", string maCN = null)
        {
            var danhSachChiNhanh = _context.ChiNhanhs.ToList();
            ViewBag.DanhSachChiNhanh = new SelectList(danhSachChiNhanh, "MaCn", "Ten", maCN);

            if (thang == 0) thang = DateTime.Now.Month;
            if (nam == 0) nam = DateTime.Now.Year;

            IQueryable<ThongKeSanPham> thongKeSanPhamQuery;

            // Lọc theo loại bán hàng
            if (loaiBanHang == "onl")
            {
                thongKeSanPhamQuery = _context.Ctsponls
                    .Join(_context.Phieudhonls, ct => ct.MaPhieuonl, po => po.MaPhieuonl, (ct, po) => new { ct, po })
                    .Where(x => x.po.MaCn == maCN && x.po.Ngaygiodat.Month == thang && x.po.Ngaygiodat.Year == nam)
                    .GroupBy(x => x.ct.MaSp)
                    .Select(g => new ThongKeSanPham
                    {
                        MaSp = g.Key,
                        TenSanPham = _context.SanPhams.Where(sp => sp.MaSp == g.Key).Select(sp => sp.Ten).FirstOrDefault(),
                        TongSoluong = g.Sum(x => x.ct.Soluong),
                        TongDoanhThu = g.Sum(x => x.ct.Tongtien),
                        Thang = thang,
                        Nam = nam
                    });
            }
            else if (loaiBanHang == "off")
            {
                thongKeSanPhamQuery = _context.CtsanPhams
                    .Join(_context.PhieuOrders, ct => ct.MaOrder, po => po.MaOrder, (ct, po) => new { ct, po })
                    .Where(x => x.po.MaCn == maCN && x.po.Ngaygiodat.Month == thang && x.po.Ngaygiodat.Year == nam)
                    .GroupBy(x => x.ct.MaSp)
                    .Select(g => new ThongKeSanPham
                    {
                        MaSp = g.Key,
                        TenSanPham = _context.SanPhams.Where(sp => sp.MaSp == g.Key).Select(sp => sp.Ten).FirstOrDefault(),
                        TongSoluong = g.Sum(x => x.ct.Soluong),
                        TongDoanhThu = g.Sum(x => x.ct.TongTien),
                        Thang = thang,
                        Nam = nam
                    });
            }
            else
            {
                thongKeSanPhamQuery = _context.CtsanPhams
                    .Join(_context.PhieuOrders, ct => ct.MaOrder, po => po.MaOrder, (ct, po) => new { ct, po })
                    .Where(x => x.po.MaCn == maCN && x.po.Ngaygiodat.Month == thang && x.po.Ngaygiodat.Year == nam)
                    .GroupBy(x => x.ct.MaSp)
                    .Select(g => new ThongKeSanPham
                    {
                        MaSp = g.Key,
                        TenSanPham = _context.SanPhams.Where(sp => sp.MaSp == g.Key).Select(sp => sp.Ten).FirstOrDefault(),
                        TongSoluong = g.Sum(x => x.ct.Soluong),
                        TongDoanhThu = g.Sum(x => x.ct.TongTien),
                        Thang = thang,
                        Nam = nam
                    })
                    .Union(
                        _context.Ctsponls
                        .Join(_context.Phieudhonls, ct => ct.MaPhieuonl, po => po.MaPhieuonl, (ct, po) => new { ct, po })
                        .Where(x => x.po.MaCn == maCN && x.po.Ngaygiodat.Month == thang && x.po.Ngaygiodat.Year == nam)
                        .GroupBy(x => x.ct.MaSp)
                        .Select(g => new ThongKeSanPham
                        {
                            MaSp = g.Key,
                            TenSanPham = _context.SanPhams.Where(sp => sp.MaSp == g.Key).Select(sp => sp.Ten).FirstOrDefault(),
                            TongSoluong = g.Sum(x => x.ct.Soluong),
                            TongDoanhThu = g.Sum(x => x.ct.Tongtien),
                            Thang = thang,
                            Nam = nam
                        })
                    );
            }

            var tongDoanhThuOnl = _context.Ctsponls
               .Join(_context.Phieudhonls, ct => ct.MaPhieuonl, po => po.MaPhieuonl, (ct, po) => new { ct, po })
               .Where(x => x.po.MaCn == maCN && x.po.Ngaygiodat.Month == thang && x.po.Ngaygiodat.Year == nam)
               .Sum(x => x.ct.Tongtien);

            var tongDoanhThuOff = _context.CtsanPhams
                .Join(_context.PhieuOrders, ct => ct.MaOrder, po => po.MaOrder, (ct, po) => new { ct, po })
                .Where(x => x.po.MaCn == maCN && x.po.Ngaygiodat.Month == thang && x.po.Ngaygiodat.Year == nam)
                .Sum(x => x.ct.TongTien);

            ViewBag.TongDoanhThuOnl = tongDoanhThuOnl;
            ViewBag.TongDoanhThuOff = tongDoanhThuOff;

            var danhSachThongKe = thongKeSanPhamQuery.ToList(); // Lấy dữ liệu từ truy vấn

            // Tính toán tổng doanh thu và tổng số lượng
            var tongDoanhThu = danhSachThongKe.Sum(x => x.TongDoanhThu);
            var tongSoLuong = danhSachThongKe.Sum(x => x.TongSoluong);

            ViewBag.Thang = thang;
            ViewBag.Nam = nam;
            ViewBag.LoaiHienThi = loaiHienThi; // Thiết lập loại hiển thị để sử dụng trong view
            ViewBag.TongDoanhThu = tongDoanhThu; // Thêm tổng doanh thu
            ViewBag.TongSoLuong = tongSoLuong; // Thêm tổng số lượng
            ViewBag.LoaiBanHang = loaiBanHang; // Thêm loại bán hàng để sử dụng trong view

            // Gán doanh thu vào ViewBag để sử dụng trong view


            return View(danhSachThongKe); // Trả về danh sách thống kê
        }
        [HttpGet]
        public IActionResult DoanhThuTatCaChiNhanh(int thang, int nam)
        {
            if (thang == 0) thang = DateTime.Now.Month;
            if (nam == 0) nam = DateTime.Now.Year;

            var doanhThuTatCaChiNhanh = _context.Phieudhonls
                .Where(x => x.Ngaygiodat.Month == thang && x.Ngaygiodat.Year == nam)
                .GroupBy(x => x.MaCn)
                .Select(g => new DoanhThuChiNhanh
                {
                    MaCn = g.Key,
                    TongDoanhThuOnline = g.Sum(x => x.TongTien) ?? 0 // Tính tổng doanh thu online
                }).ToList();

            var doanhThuOffline = _context.PhieuOrders
                .Where(x => x.Ngaygiodat.Month == thang && x.Ngaygiodat.Year == nam)
                .GroupBy(x => x.MaCn)
                .Select(g => new
                {
                    MaCn = g.Key,
                    TongDoanhThuOffline = g.Sum(x => x.Tongtien) // Tính tổng doanh thu offline
                }).ToList();

            // Gộp dữ liệu doanh thu online và offline
            foreach (var offline in doanhThuOffline)
            {
                var onlineEntry = doanhThuTatCaChiNhanh.FirstOrDefault(x => x.MaCn == offline.MaCn);
                if (onlineEntry != null)
                {
                    onlineEntry.TongDoanhThuOffline = offline.TongDoanhThuOffline; // Cập nhật doanh thu offline
                }
                else
                {
                    doanhThuTatCaChiNhanh.Add(new DoanhThuChiNhanh
                    {
                        MaCn = offline.MaCn,
                        TongDoanhThuOnline = 0,
                        TongDoanhThuOffline = offline.TongDoanhThuOffline
                    });
                }
            }

            return View(doanhThuTatCaChiNhanh);
        }


    }
}
