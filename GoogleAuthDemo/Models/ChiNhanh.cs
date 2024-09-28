using System;
using System.Collections.Generic;

namespace GoogleAuthDemo.Models;

public partial class ChiNhanh
{
    public string MaCn { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public string Diachi { get; set; } = null!;

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();

    public virtual ICollection<PhieuOrder> PhieuOrders { get; set; } = new List<PhieuOrder>();

    public virtual ICollection<Phieudhonl> Phieudhonls { get; set; } = new List<Phieudhonl>();
}
