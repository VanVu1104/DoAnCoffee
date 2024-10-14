using Manage_Coffee.Models;
using Manage_Coffee.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Manage_Coffee.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, IUserService userService, IEmailService emailService)
        {
            _logger = logger;
            _userService = userService;
            _emailService = emailService;
        }
        public async Task<ViewResult> Index() //?ây là trang ch? nha
        {
            var userId = _userService.GetUserId();
            var isLoggedIn = _userService.IsAuthenticated();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult LienHe()
        {
            return View();
        }
        public IActionResult CuaHang()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
