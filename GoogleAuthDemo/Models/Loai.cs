using System;
using System.Collections.Generic;

namespace GoogleAuthDemo.Models;

public partial class Loai
{
    public string Maloai { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
