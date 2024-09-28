using System;
using System.Collections.Generic;

namespace GoogleAuthDemo.Models;

public partial class CtsanPham
{
    public int Soluong { get; set; }

    public int Gia { get; set; }

    public int TongTien { get; set; }

    public string? Ghichu { get; set; }

    public string MaOrder { get; set; } = null!;

    public string MaSp { get; set; } = null!;

    public string MaKh { get; set; } = null!;

    public string MaSize { get; set; } = null!;

    public virtual PhieuOrder MaOrderNavigation { get; set; } = null!;

    public virtual Size MaSizeNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
