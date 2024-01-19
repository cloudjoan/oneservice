using Microsoft.AspNetCore.Mvc;
using NPOI.OpenXmlFormats.Dml;

namespace OneService.Controllers
{
    public class CarRentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ShowCarList()
        {
            return View();
        }
    }
}
