using System;
using System.Collections.Generic;

namespace Manage_Coffee.Models;

public partial class KhachHang
{
    public string Role { get; set; } = "Dong";

    public string MaKh { get; set; } = null!;

    public string? Ten { get; set; }

    public string? Diachi { get; set; }

    public string Matkhau { get; set; } = null!;

    public int? Sdt { get; set; }

    public string? Email { get; set; }
    public bool? GioiTinh { get; set; }

    public virtual ICollection<ChitietDanhGium> ChitietDanhGia { get; set; } = new List<ChitietDanhGium>();

    public virtual ICollection<Phieudhonl> Phieudhonls { get; set; } = new List<Phieudhonl>();

}
