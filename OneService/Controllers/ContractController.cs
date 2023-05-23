using Microsoft.AspNetCore.Mvc;

namespace OneService.Controllers
{
    public class ContractController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ContractMain()
        {
            return View();
        }
    }
}
