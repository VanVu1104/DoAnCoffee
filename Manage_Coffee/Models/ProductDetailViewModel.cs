using Manage_Coffee.Models;
using System.Collections.Generic;

namespace Manage_Coffee.ViewModels
{
    public class ProductDetailViewModel
    {
        public SanPham Product { get; set; }
        public List<SanPham> Toppings { get; set; } = new List<SanPham>();
        public List<Kit> KitsAsKit { get; set; } = new List<Kit>();

        // Danh sách các kit mà sản phẩm này là sản phẩm đi kèm
        public List<Kit> KitsAsSp { get; set; } = new List<Kit>();
    }
}
