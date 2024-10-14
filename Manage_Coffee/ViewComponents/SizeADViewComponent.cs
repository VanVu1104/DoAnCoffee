using Manage_Coffee.Models;
using Microsoft.AspNetCore.Mvc;

namespace Manage_Coffee.ViewComponents
{
    public class SizeADViewComponent : ViewComponent
    {
        private readonly Cf2Context _context;

        public SizeADViewComponent(Cf2Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var sizes = await Task.Run(() => _context.Sizes.ToList());
            return View(sizes);
        }
    }
}
