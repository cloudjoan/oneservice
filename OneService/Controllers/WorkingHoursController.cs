using Microsoft.AspNetCore.Mvc;
using OneService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace OneService.Controllers
{
    public class WorkingHoursController : Controller
    {
        BIContext biDB = new BIContext();
        PSIPContext psipDB = new PSIPContext();
        public IActionResult Index()
        {
            var SRLaborBeans =  biDB.MartAnalyseServiceRequestLabors.Where(x => x.EngineerId == "10010640");
            ViewBag.SRLaborBeans = SRLaborBeans;
            return View();
        }

        public IActionResult GetSRLabor(string erpId)
        {
            var SRLaborBeans = biDB.MartAnalyseServiceRequestLabors.Where(x => x.EngineerId == erpId);
            ViewBag.SRLaborBeans = SRLaborBeans;
            return View();
        }


        public IActionResult CreateWH()
        {
            ViewBag.now = string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.Now);

            var beans = psipDB.TbWorkingHoursMains.Where(x => x.Disabled != 1).OrderByDescending(x => x.Id);

            List<TbWorkingHoursMain> list2 = new List<TbWorkingHoursMain>();

            
            ViewBag.beans = beans;

            return View();
        }

        public IActionResult SaveWH(IFormCollection formCollection)
        {
            TbWorkingHoursMain bean;
            if (!string.IsNullOrEmpty(formCollection["Id"]))
            {
                bean = psipDB.TbWorkingHoursMains.Find(int.Parse(formCollection["Id"]));
                bean.Whtype = formCollection["ddl_WHType"].ToString();
                bean.ActType = formCollection["ddl_ActType"].ToString();
                bean.CrmOppNo = formCollection["tbx_CrmOppNo"].ToString();
                bean.WhDescript = formCollection["tbx_WhDescript"].ToString();
                bean.StartTime = formCollection["tbx_StartTime"].ToString();
                bean.EndTime = formCollection["tbx_EndTime"].ToString();
                bean.UpdateTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

                //計算工時分鐘數
                DateTime startTime = Convert.ToDateTime(bean.StartTime);
                DateTime endTime = Convert.ToDateTime(bean.EndTime);
                TimeSpan ts = endTime - startTime;
                bean.Labor = Convert.ToInt32(ts.TotalMinutes);
            }
            else
            {
                bean = new TbWorkingHoursMain();
                bean.UserName = formCollection["tbx_UserName"].ToString();
                bean.UserErpId = formCollection["hid_UserErpId"].ToString();
                bean.Whtype = formCollection["ddl_WHType"].ToString();
                bean.ActType = formCollection["ddl_ActType"].ToString();
                bean.CrmOppNo = formCollection["tbx_CrmOppNo"].ToString();
                bean.WhDescript = formCollection["tbx_WhDescript"].ToString();
                bean.StartTime = formCollection["tbx_StartTime"].ToString();
                bean.EndTime = formCollection["tbx_EndTime"].ToString();
                bean.InsertTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

                //計算工時分鐘數
                DateTime startTime = Convert.ToDateTime(bean.StartTime);
                DateTime endTime = Convert.ToDateTime(bean.EndTime);
                TimeSpan ts = endTime - startTime;
                bean.Labor = Convert.ToInt32(ts.TotalMinutes);

                psipDB.TbWorkingHoursMains.Add(bean);
            }

            psipDB.SaveChanges();

            return RedirectToAction("CreateWH");
        }

        public IActionResult GetWHById(int id)
        {
            var bean = psipDB.TbWorkingHoursMains.Find(id);
            return Json(bean);
        }

        public IActionResult DeleteWHById(int id)
        {
            var bean = psipDB.TbWorkingHoursMains.Find(id);
            bean.Disabled = 1;

            psipDB.SaveChanges();

            return Json("OK");
        }
    }
}
