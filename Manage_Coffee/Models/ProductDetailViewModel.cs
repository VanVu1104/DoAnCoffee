using Manage_Coffee.Models;
using Manage_Coffee.Models;
using System.Collections.Generic;

namespace Manage_Coffee.ViewModels
{
    public class ProductDetailViewModel
    {
        public SanPham Product { get; set; }
        public List<SanPham> Toppings { get; set; } = new List<SanPham>();
    }
}
