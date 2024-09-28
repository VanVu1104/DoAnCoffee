using System;
using System.Collections.Generic;

namespace GoogleAuthDemo.Models;

public partial class Ctphieu
{
    public int Soluong { get; set; }

    public string MaNvl { get; set; } = null!;

    public string MaPhieu { get; set; } = null!;

    public virtual NguyenVatLieu MaNvlNavigation { get; set; } = null!;

    public virtual PhieuNhapXuat MaPhieuNavigation { get; set; } = null!;
}
