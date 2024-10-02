using System;
using System.Collections.Generic;

namespace Manage_Coffee.Models;

public partial class Nhacungcap
{
    public string MaCcap { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public string Diachi { get; set; } = null!;

    public int Sdt { get; set; }

    public virtual ICollection<PhieuNhapXuat> PhieuNhapXuats { get; set; } = new List<PhieuNhapXuat>();
}
