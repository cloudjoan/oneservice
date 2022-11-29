using Microsoft.AspNetCore.Mvc;
using OneService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Linq;
using NuGet.Packaging.Signing;

namespace OneService.Controllers
{
    public class WorkingHoursController : Controller
    {
        BIContext biDB = new BIContext();
        PSIPContext psipDB = new PSIPContext();
        TAIFContext bpmDB = new TAIFContext();
        MCSWorkflowContext eipDB = new MCSWorkflowContext();

        public IActionResult Index()
        {
            var userAccount = User.Identity.Name;
            System.Diagnostics.Debug.WriteLine(userAccount);

            var client = new RestClient("http://tsti-sapapp01.etatung.com.tw/api/ticc");
           
            var request = new RestRequest();
            request.Method = RestSharp.Method.Post;

            Dictionary<Object, Object> parameters = new Dictionary<Object, Object>();
            parameters.Add("SAP_FUNCTION_NAME", "ZFM_TICC_SERIAL_SEARCH");
            parameters.Add("IV_SERIAL", "2CE3231K6X");

            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", parameters, ParameterType.RequestBody);
            
            RestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);

            var data   = (JObject)JsonConvert.DeserializeObject(response.Content);

            System.Diagnostics.Debug.WriteLine(data["ET_REQUEST"]["SyncRoot"][0]["cNAMEField"]);

            return View();
        }

        public IActionResult GetSRLabor(string erpId)
        {
            var viewWHBeans = psipDB.ViewWorkingHours.Where(x => x.UserErpId == erpId);
            ViewBag.viewWHBeans = viewWHBeans;
            return View();
        }

        public IActionResult CreateWH()
        {
            ViewBag.now = string.Format("{0:yyyy-MM-dd}", DateTime.Now);

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
                bean.StartTime = formCollection["tbx_StartDate"].ToString() + " " + formCollection["hid_StartTime"].ToString();
                bean.EndTime = formCollection["tbx_EndDate"].ToString() + " " + formCollection["hid_EndTime"].ToString();
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
                bean.StartTime = formCollection["tbx_StartDate"].ToString() + " " + formCollection["hid_StartTime"].ToString();
                bean.EndTime = formCollection["tbx_EndDate"].ToString() + " " + formCollection["hid_EndTime"].ToString();
                bean.InsertTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

                //計算工時分鐘數
                DateTime startTime = Convert.ToDateTime(bean.StartTime);
                DateTime endTime = Convert.ToDateTime(bean.EndTime);
                TimeSpan ts = endTime - startTime;
                bean.Labor = Convert.ToInt32(ts.TotalMinutes);

                psipDB.TbWorkingHoursMains.Add(bean);
            }

            //如果有商機跟專案管理的話，就需加入專案管理的工時計算
            if (!string.IsNullOrEmpty(bean.CrmOppNo))
            {
                //取得MileStone
                string ms = GetMileStone(bean.CrmOppNo, bean.StartTime, bean.EndTime);

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


        #region -- 儲存專案執行紀錄 --
        /// <summary>儲存專案執行紀錄</summary>
        public ActionResult SavePjRecord(int? prId, string oppNo, string bundleMs, string bundleTask, string impBy, string ImplementersCount, string Attendees, string place, string startDate, string endDate, string workHours, string withPpl, string withPplPhone, string desc, string attach)
        {
            #region -- 取得登入者資訊 --

            string userAccount = @"etatung\leon.huang";
            //取得登入者資訊            
            Person empBean = eipDB.People.FirstOrDefault(x => x.Account == userAccount &&string.IsNullOrEmpty(x.LeaveReason));
            ViewEmpInfo empInfoBean = eipDB.ViewEmpInfos.FirstOrDefault(x => x.Account == userAccount);
            
            #endregion

            try
            {
                int result = 0;
                if (prId == null) //新增
                {
                    #region -- 儲存專案執行紀錄 --
                    TbProPjRecord prBean = new TbProPjRecord();
                    prBean.CrmOppNo = OppNoFormat(oppNo);
                    prBean.BundleMs = bundleMs;
                    prBean.BundleTask = !string.IsNullOrEmpty(bundleTask) ? bundleTask : "";
                    prBean.Implementers = impBy;
                    prBean.ImplementersCount = Convert.ToInt32(ImplementersCount);
                    prBean.Attendees = Attendees;   //edit by elvis 2022/07/18
                    prBean.Place = place;
                    prBean.StartDatetime = startDate; //Convert.ToDateTime(startDate).ToString("yyyy-MM-dd HH:mm:ss")
                    prBean.EndDatetime = endDate;
                    prBean.WorkHours = Convert.ToInt32(workHours);
                    prBean.TotalWorkHours = prBean.ImplementersCount * prBean.WorkHours;
                    prBean.WithPpl = withPpl;
                    prBean.WithPplPhone = withPplPhone;
                    prBean.Description = desc;
                    prBean.Attachment = attach;

                    prBean.CrErpId = empBean.ErpId;
                    prBean.CrAccount = empBean.Account.ToLower();
                    prBean.CrName = empBean.Name2;
                    prBean.CrEmail = empBean.Email;
                    prBean.CrCompCode = empBean.CompCde;
                    prBean.CrDeptId = empBean.DeptId;
                    prBean.CrDeptName = empInfoBean.DeptName;
                    prBean.Disabled = 0; // 0: false; 1: true(disabled)
                    prBean.InsertTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    prBean.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    psipDB.TbProPjRecords.Add(prBean);
                    result = psipDB.SaveChanges();

                    //有設定任務編號，則要將執行紀錄的合計總工時，更新到任務統計的實際工時
                    if (!string.IsNullOrEmpty(prBean.BundleTask))
                    {
                        var qSameTask = psipDB.TbProPjRecords.Where(x => x.BundleMs == prBean.BundleMs && x.BundleTask == prBean.BundleTask && x.Disabled == 0).ToList();
                        if (qSameTask != null && qSameTask.Count > 0)
                        {
                            int sumSameTaskWorkHour = 0;
                            foreach (var pRecord in qSameTask)
                            {
                                sumSameTaskWorkHour += Convert.ToInt32(pRecord.TotalWorkHours);
                            }

                            int stair1No = Convert.ToInt32(prBean.BundleTask.Split('.')[0]);
                            int stair2No = prBean.BundleTask.Contains(".") ? Convert.ToInt32(prBean.BundleTask.Split('.')[1]) : 0;
                            int stair3No = 0;
                            if (prBean.BundleTask.Contains("."))
                            {
                                int tLen = prBean.BundleTask.Split('.').Length;

                                if (tLen >= 3)
                                {
                                    stair3No = Convert.ToInt32(prBean.BundleTask.Split('.')[2]);
                                }
                            }

                            var qTask = psipDB.TbProTasks.FirstOrDefault(x => x.Milestone == prBean.BundleMs && x.Stair1No == stair1No && x.Stair2No == stair2No && x.Stair3No == stair3No && x.Disabled == 0);
                            if (qTask != null)
                            {
                                qTask.WorkHours = sumSameTaskWorkHour;
                                psipDB.SaveChanges();
                            }
                        }
                    }
                    #endregion
                }
                else //編輯
                {
                    #region -- 編輯專案執行紀錄 --
                    var prBean = psipDB.TbProPjRecords.FirstOrDefault(x => x.Id == prId);
                    if (prBean != null)
                    {
                        prBean.CrmOppNo = OppNoFormat(oppNo);
                        prBean.BundleMs = bundleMs;
                        prBean.BundleTask = !string.IsNullOrEmpty(bundleTask) ? bundleTask : "";
                        prBean.Implementers = impBy;
                        prBean.ImplementersCount = Convert.ToInt32(ImplementersCount);
                        prBean.Attendees = Attendees;   //edit by elvis 2022/07/18
                        prBean.Place = place;
                        prBean.StartDatetime = startDate; //Convert.ToDateTime(startDate).ToString("yyyy-MM-dd HH:mm:ss")
                        prBean.EndDatetime = endDate;
                        prBean.WorkHours = Convert.ToInt32(workHours);
                        prBean.TotalWorkHours = prBean.ImplementersCount * prBean.WorkHours;
                        prBean.WithPpl = withPpl;
                        prBean.WithPplPhone = withPplPhone;
                        prBean.Description = desc;
                        prBean.Attachment = attach;

                        prBean.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        result = psipDB.SaveChanges();

                        //有設定任務編號，則要將執行紀錄的合計總工時，更新到任務統計的實際工時
                        if (!string.IsNullOrEmpty(prBean.BundleTask))
                        {
                            var qSameTask = psipDB.TbProPjRecords.Where(x => x.BundleMs == prBean.BundleMs && x.BundleTask == prBean.BundleTask && x.Disabled == 0).ToList();
                            if (qSameTask != null && qSameTask.Count > 0)
                            {
                                int sumSameTaskWorkHour = 0;
                                foreach (var pRecord in qSameTask)
                                {
                                    sumSameTaskWorkHour += Convert.ToInt32(pRecord.TotalWorkHours);
                                }

                                int stair1No = Convert.ToInt32(prBean.BundleTask.Split('.')[0]);
                                int stair2No = prBean.BundleTask.Contains(".") ? Convert.ToInt32(prBean.BundleTask.Split('.')[1]) : 0;
                                int stair3No = 0;
                                if (prBean.BundleTask.Contains("."))
                                {
                                    int tLen = prBean.BundleTask.Split('.').Length;

                                    if (tLen >= 3)
                                    {
                                        stair3No = Convert.ToInt32(prBean.BundleTask.Split('.')[2]);
                                    }
                                }

                                var qTask = psipDB.TbProTasks.FirstOrDefault(x => x.Milestone == prBean.BundleMs && x.Stair1No == stair1No && x.Stair2No == stair2No && x.Stair3No == stair3No);
                                if (qTask != null)
                                {
                                    qTask.WorkHours = sumSameTaskWorkHour;
                                    psipDB.SaveChanges();
                                }
                            }
                        }
                    }
                    #endregion
                }
                return Json(true);
            }
            catch (Exception e)
            {
                //SendMailByAPI("PMO儲存專案執行紀錄", null, "Elvis.Chang@etatung.com", "", "", "PMO儲存專案執行紀錄_錯誤", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "<br>prId: " + prId + "<br>" + e.ToString(), null, null);
                return Json(false);
            }
        }
        #endregion

        #region -- 商機編號設定為10位數 --
        public string OppNoFormat(string oppNo)
        {
            var _oppNo = "0000000000" + oppNo;
            var oppNoFormat = _oppNo.Substring(_oppNo.Length - 10, 10);
            return oppNoFormat;
        }
        #endregion

        #region 依起訖時間，取得里程碑
        public string GetMileStone(string oppNo, string startDate, string endDate)
        {
            var beans = psipDB.TbProMilestones.Where(x => x.CrmOppNo == OppNoFormat(oppNo));
            var _startDate = Convert.ToDateTime(startDate);
            var _endDate = Convert.ToDateTime(startDate);

            foreach (var bean in beans)
            {
                if (!string.IsNullOrEmpty(bean.EstimatedDate))
                {
                    var eDate = Convert.ToDateTime(bean.EstimatedDate);

                    if (eDate.Year == _startDate.Year && eDate.Month == _startDate.Month && eDate.Day == _startDate.Day) return bean.MilestoneNo; 

                }else if(!string.IsNullOrEmpty(bean.EstimatedDate) && !string.IsNullOrEmpty(bean.FinishedDate))
                {
                    var eDate = Convert.ToDateTime(bean.EstimatedDate);
                    var fDate = Convert.ToDateTime(bean.FinishedDate);
                    if (_startDate >= eDate && _startDate <= fDate) return bean.MilestoneNo;
                    if (_endDate >= fDate && _endDate <= fDate) return bean.MilestoneNo;
                }
            }
            return "";
        }

        #endregion
    }

}
