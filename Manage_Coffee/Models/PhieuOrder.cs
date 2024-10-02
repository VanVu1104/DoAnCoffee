using System;
using System.Collections.Generic;

namespace Manage_Coffee.Models;

public partial class PhieuOrder
{
    public string MaOrder { get; set; } = null!;

    public DateTime Ngaygiodat { get; set; }

    public int Soban { get; set; }

    public int Tongtien { get; set; }

    public bool Trangthai { get; set; }

    public string? Pttt { get; set; }

    public string? Ptnh { get; set; }

    public string MaKh { get; set; } = null!;

    public string MaCn { get; set; } = null!;

    public string? MaNv { get; set; }

    public string? MaKm { get; set; }

    public virtual ICollection<CtsanPham> CtsanPhams { get; set; } = new List<CtsanPham>();

    public virtual ChiNhanh MaCnNavigation { get; set; } = null!;

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual DanhMucKm? MaKmNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }
}
