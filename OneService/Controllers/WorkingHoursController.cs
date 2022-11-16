using Microsoft.AspNetCore.Mvc;
using OneService.Models;

namespace OneService.Controllers
{
    public class WorkingHoursController : Controller
    {
        BIContext biDB = new BIContext();
        PSIPContext psipDB = new PSIPContext();
        public IActionResult Index()
        {
            var SRLaborBeans =  biDB.MartAnalyseServiceRequestLabors.Take(50);
            ViewBag.SRLaborBeans = SRLaborBeans;
            return View();
        }


        public IActionResult CreateWH()
        {
            ViewBag.now = string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.Now);

            var beans = psipDB.TbWorkingHoursMains.OrderByDescending(x => x.Id);
            return View();
        }

        public IActionResult SaveWH(IFormCollection formCollection)
        {
            TbWorkingHoursMain bean = new TbWorkingHoursMain();
            bean.UserName = formCollection["tbx_UserName"].ToString();
            bean.UserErpId = formCollection["hid_UserErpId"].ToString();
            bean.Whtype = formCollection["ddl_WHType"].ToString();
            bean.ActType = formCollection["ddl_ActType"].ToString();
            bean.CrmOppNo = formCollection["tbx_CrmOppNo"].ToString();
            bean.WhDescript = formCollection["tbx_WhDescript"].ToString();
            bean.StartTime =  formCollection["tbx_StartTime"].ToString();
            bean.EndTime = formCollection["tbx_EndTime"].ToString();
            bean.InsertTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            psipDB.TbWorkingHoursMains.Add(bean);
            psipDB.SaveChanges();

            return RedirectToAction("CreateWH");
        }
    }
}
