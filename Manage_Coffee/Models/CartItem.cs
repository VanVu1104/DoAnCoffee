using System.Drawing;

namespace Manage_Coffee.Models
{
    public class CartItem
    {
		
		public string ProductID { get; set; } 

        public string Ten { get; set; } 

        public int Dongia { get; set; }
        public string Anh { get; set; }

        public int Soluong { get; set; }

		public IEnumerable<DanhMucKm> Discounts { get; set; }

		// Thêm Size vào CartItem
		public string SizeID { get; set; }
	    public string SizeName { get; set; }
	    public int TriGia { get; set; }
        public string Ghichu { get; set; }
        public List<Kit> Kits { get; set; } = new List<Kit>();// Thêm danh sách kit
        public int Total { get { return Soluong *(Dongia+TriGia); } }
        public CartItem()
        {
        
        }
        public CartItem(SanPham sanPham,Size size, string ghiChu = "")
        {
            ProductID = sanPham.MaSp;
            Ten = sanPham.Ten;
            Dongia = sanPham.Dongia;
            Soluong = 1;
            Anh = sanPham.Anh;
            SizeID = size.MaSize;  
	    	SizeName = size.Ten;
            TriGia = size.TriGia;
            Ghichu = ghiChu;
            Kits = new List<Kit>();
        }
    }
}
