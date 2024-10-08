﻿using System;
using System.Collections.Generic;

namespace Manage_Coffee.Models;

public partial class SanPham
{
    public string MaSp { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public int Dongia { get; set; }

    public string Dvt { get; set; } = null!;

    public string Mota { get; set; } = null!;

    public string Anh { get; set; }

    public string? Maloai { get; set; }

    public string? MaTopping { get; set; }

    public virtual ICollection<Bom> Boms { get; set; } = new List<Bom>();

    public virtual ICollection<ChitietDanhGium> ChitietDanhGia { get; set; } = new List<ChitietDanhGium>();

    public virtual ICollection<CtsanPham> CtsanPhams { get; set; } = new List<CtsanPham>();

    public virtual ICollection<Ctsponl> Ctsponls { get; set; } = new List<Ctsponl>();

    public virtual ICollection<SanPham> InverseMaToppingNavigation { get; set; } = new List<SanPham>();

    public virtual SanPham? MaToppingNavigation { get; set; }

    public virtual Loai? MaloaiNavigation { get; set; }
}
