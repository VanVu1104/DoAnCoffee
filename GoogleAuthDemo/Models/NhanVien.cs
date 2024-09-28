using System;
using System.Collections.Generic;

namespace GoogleAuthDemo.Models;

public partial class NhanVien
{
    public string MaNv { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public int Sdt { get; set; }

    public string Mkhau { get; set; } = null!;

    public string Chucvu { get; set; } = null!;

    public string Diachi { get; set; } = null!;

    public DateTime Ngaysinh { get; set; }

    public string? MaCn { get; set; }

    public virtual ICollection<DanhMucCa> DanhMucCas { get; set; } = new List<DanhMucCa>();

    public virtual ChiNhanh? MaCnNavigation { get; set; }

    public virtual ICollection<PhieuNhapXuat> PhieuNhapXuats { get; set; } = new List<PhieuNhapXuat>();

    public virtual ICollection<PhieuOrder> PhieuOrders { get; set; } = new List<PhieuOrder>();
}
