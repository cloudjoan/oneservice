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
			bean.InsertTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
			appDB.TbAccessables.Add(bean);
			appDB.SaveChanges();

			return RedirectToAction("Accessable");
		}

		public IActionResult DelAccessableById(int id)
		{
			appDB.TbAccessables.Remove(appDB.TbAccessables.FirstOrDefault(x => x.Id == id));
			appDB.SaveChanges();
			return Json(true);
		}

		public IActionResult ShowAccessHistory()
		{
			ViewBag.beans = appDB.TbAccessHistories.OrderByDescending(x => x.Id).ToList();
			return View();
		}

	}
}
