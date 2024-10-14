using System;
using System.Collections.Generic;

namespace Manage_Coffee.Models;
public partial class ChitietDanhGium
{
    public int SoSao { get; set; }

    public string MaSp { get; set; } = null!;

    public string MaKh { get; set; } = null!;
    public string? NhanXet { get; set; }
	public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
