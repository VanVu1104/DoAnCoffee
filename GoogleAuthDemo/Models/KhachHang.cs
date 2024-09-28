using System;
using System.Collections.Generic;

namespace GoogleAuthDemo.Models;

public partial class KhachHang
{
    public string Role { get; set; } = null!;

    public string MaKh { get; set; } = null!;

    public string? Ten { get; set; }

    public string? Diachi { get; set; }

    public string Matkhau { get; set; } = null!;

    public int? Sdt { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<ChitietDanhGium> ChitietDanhGia { get; set; } = new List<ChitietDanhGium>();

    public virtual ICollection<PhieuOrder> PhieuOrders { get; set; } = new List<PhieuOrder>();

    public virtual ICollection<Phieudhonl> Phieudhonls { get; set; } = new List<Phieudhonl>();
}
