using System;
using System.Collections.Generic;

namespace Manage_Coffee.Models;

public partial class DanhMucCa
{
    public string Calam { get; set; } = null!;

    public DateTime Ngay { get; set; }

    public string MaNv { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
