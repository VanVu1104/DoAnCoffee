using System;
using System.Collections.Generic;

namespace GoogleAuthDemo.Models;

public partial class Bom
{
    public double SoLuong { get; set; }

    public string MaSp { get; set; } = null!;

    public string MaNvl { get; set; } = null!;

    public virtual NguyenVatLieu MaNvlNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
