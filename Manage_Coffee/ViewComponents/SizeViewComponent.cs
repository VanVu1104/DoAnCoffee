using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Manage_Coffee.Models;
using System.Threading.Tasks;


namespace SizeViewComponent.Components
{
    public class SizeViewComponent : ViewComponent
    {
        private readonly Cf2Context _context;

        public SizeViewComponent(Cf2Context context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string sanPhamId)
        {

            var sizes = await _context.Sizes.ToListAsync();
            return View(sizes);
        }
    }
}
