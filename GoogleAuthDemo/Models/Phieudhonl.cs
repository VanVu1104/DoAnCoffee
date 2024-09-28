using System;
using System.Collections.Generic;

namespace GoogleAuthDemo.Models;

public partial class Phieudhonl
{
    public string MaPhieuonl { get; set; } = null!;

    public DateTime Ngaygiodat { get; set; }

    public int? TongTien { get; set; }

    public string DiaChi { get; set; } = null!;

    public bool TrangThai { get; set; }

    public string? Pttt { get; set; }

    public int? TienShip { get; set; }

    public string? Ptnh { get; set; }

    public string MaKh { get; set; } = null!;

    public string MaCn { get; set; } = null!;

    public string? MaKm { get; set; }

    public virtual ICollection<Ctsponl> Ctsponls { get; set; } = new List<Ctsponl>();

    public virtual ChiNhanh MaCnNavigation { get; set; } = null!;

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual DanhMucKm? MaKmNavigation { get; set; }
}
