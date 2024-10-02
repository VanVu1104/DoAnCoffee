using System;
using System.Collections.Generic;

namespace Manage_Coffee.Models;
public partial class NguyenVatLieu
{
    public string MaNvl { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public int Dongia { get; set; }

    public string Dvt { get; set; } = null!;

    public string Mota { get; set; } = null!;

    public string Anh { get; set; } = null!;

    public virtual ICollection<Bom> Boms { get; set; } = new List<Bom>();

    public virtual ICollection<Ctphieu> Ctphieus { get; set; } = new List<Ctphieu>();
}
