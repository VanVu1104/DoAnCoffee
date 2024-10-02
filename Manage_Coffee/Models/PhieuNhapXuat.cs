using System;
using System.Collections.Generic;

namespace Manage_Coffee.Models;

public partial class PhieuNhapXuat
{
    public string MaPhieu { get; set; } = null!;

    public DateTime NgayLap { get; set; }

    public string Loai { get; set; } = null!;

    public string Diachi { get; set; } = null!;

    public string MaNv { get; set; } = null!;

    public string? MaCcap { get; set; }

    public virtual ICollection<Ctphieu> Ctphieus { get; set; } = new List<Ctphieu>();

    public virtual Nhacungcap? MaCcapNavigation { get; set; }

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
