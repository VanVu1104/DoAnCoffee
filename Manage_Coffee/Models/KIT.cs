using System;
using System.Collections.Generic;

namespace Manage_Coffee.Models;

public partial class Kit
{
	public int SoLuong { get; set; }

	public string MaKit { get; set; } = null!;

	public string MaSp { get; set; } = null!;
	public decimal GiaKit { get; set; }
	public virtual SanPham MaKitNavigation { get; set; } = null!;

	public virtual SanPham MaSpNavigation { get; set; } = null!;
}
