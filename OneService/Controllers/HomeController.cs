using Microsoft.AspNetCore.Mvc;
using OneService.Models;
using System.Diagnostics;

namespace OneService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Demo()
        {
            PSIPContext psipDB = new PSIPContext();
            var beans = psipDB.TbBulletinItems;
            ViewBag.beans = beans;
            return View();
        }
    }
}