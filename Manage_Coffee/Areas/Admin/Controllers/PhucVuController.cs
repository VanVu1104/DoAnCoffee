using Microsoft.AspNetCore.Mvc;

namespace Manage_Coffee.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhucVuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
