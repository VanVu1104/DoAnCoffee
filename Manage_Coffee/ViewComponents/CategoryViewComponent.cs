using Manage_Coffee.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Manage_Coffee.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly Cf2Context _context;

        public CategoryViewComponent(Cf2Context context)
        {
            _context = context;
        }

    
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loais = await Task.Run(() => _context.Loais.ToList());
            return View(loais);
        }
    }
}
