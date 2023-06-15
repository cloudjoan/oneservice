using Microsoft.AspNetCore.Mvc;
using OneService.Models;

namespace OneService.Controllers
{
	public class QrCodeController : Controller
	{

		APP_DATAContext appDB = new APP_DATAContext();
		public IActionResult Index()
		{
			return View();
		}


		public IActionResult Accessable()
		{
			ViewBag.beans = appDB.TbAccessables;
			return View();
		}

		public IActionResult SaveAccessable(TbAccessable bean)
		{
			appDB.TbAccessables.Add(bean);
			appDB.SaveChanges();

			return View();
		}

		public IActionResult ShowAccessHistory()
		{
			ViewBag.beans = appDB.TbAccessHistories.OrderByDescending(x => x.Id).ToList();
			return View();
		}

	}
}
