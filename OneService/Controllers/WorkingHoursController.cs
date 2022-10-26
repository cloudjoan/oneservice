using Microsoft.AspNetCore.Mvc;

namespace OneService.Controllers
{
    public class WorkingHoursController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateWH()
        {
            return View();
        }
    }
}
