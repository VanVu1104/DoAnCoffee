using Manage_Coffee.Helpers;
using Manage_Coffee.Models;
using Manage_Coffee.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoogleAuthDemo.Controllers
{
    public class CartController : Controller
    {
        private readonly Cf2Context _context;

        public CartController(Cf2Context context)
        {
            _context = context;
        }
      

        public IActionResult Cart()
        {
           
      List<CartItem> Cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartItemViewModel cart1 = new()
            {
                CartItems = Cart,
                GrandTotal = Cart.Sum(X => X.Soluong * 
                (X.Dongia + X.TriGia)),
            };
            return View(cart1);
        }
        public async Task<IActionResult> Add(string id, string idsize, string ghiChu)
        {
            SanPham sanPham = await _context.SanPhams.FindAsync(id);
            Size size = await _context.Sizes.FindAsync(idsize);

            if (sanPham == null || size == null)
            {
                // Xử lý lỗi nếu sản phẩm hoặc kích thước không tồn tại
                return NotFound();
            }

            List<CartItem> Cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartItem cartItem = Cart.Where(c => c.ProductID == id && c.SizeID == idsize).FirstOrDefault();

            if (cartItem == null)
            {
                Cart.Add(new CartItem(sanPham, size, ghiChu));
            }
            else
            {
                cartItem.Soluong += 1;
				cartItem.Ghichu = ghiChu;
			}

            HttpContext.Session.Set("Cart", Cart);

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Decrease(string id,string idsize)
        {
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            CartItem cartItem = cart.Where(c =>c.ProductID == id ).FirstOrDefault();
            if (cartItem.Soluong >1 )
            {
                --cartItem.Soluong;
            }
            else
            {
                cart.RemoveAll(p => p.Soluong > 0);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.Set("Cart", cart);
            }
            return RedirectToAction("Cart");
        }
		public async Task<IActionResult> Increase(string id, string idsize)
		{
			List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("Cart");
			CartItem cartItem = cart.Where(c => c.ProductID == id  ).FirstOrDefault();
			if (cartItem.Soluong > 1)
			{
				++cartItem.Soluong;
			}
			else
			{
				cart.RemoveAll(p => p.Soluong > 0);
			}
			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.Set("Cart", cart);
			}
			return RedirectToAction("Cart");
		}
		public async Task<IActionResult> Remove(string id)
		{
			List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            cart.RemoveAll(c => c.ProductID == id);
		
			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.Set("Cart", cart);
			}
			return RedirectToAction("Cart");
		}
        public async Task<IActionResult> Clear(string id)
        {
			HttpContext.Session.Remove("Cart");
			return RedirectToAction("Sanpham1");
		}
	}
}
