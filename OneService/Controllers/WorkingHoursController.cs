using Microsoft.AspNetCore.Mvc;
using OneService.Models;

namespace OneService.Controllers
{
    public class WorkingHoursController : Controller
    {
        BIContext biDB = new BIContext();
        public IActionResult Index()
        {
            var SRLaborBeans =  biDB.MartAnalyseServiceRequestLabors.Take(50);
            ViewBag.SRLaborBeans = SRLaborBeans;
            return View();
        }

        public IActionResult CreateWH()
        {
            return View();
        }
    }
}
