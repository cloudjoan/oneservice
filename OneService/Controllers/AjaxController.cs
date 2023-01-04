using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OneService.Models;
using OneService.Utils;

namespace OneService.Controllers
{
	public class AjaxController : Controller
	{
        private readonly IWebHostEnvironment _HostEnvironment;

        TAIFContext bpmDB = new TAIFContext();
        MCSWorkflowContext dbEIP = new MCSWorkflowContext();
        ERP_PROXY_DBContext proxyDB = new ERP_PROXY_DBContext();
        PSIPContext psipDB = new PSIPContext();
        public IActionResult Index()
		{
			return View();
		}

        public ActionResult AjaxHandler(string functionName, string keyword)
        {
            object contentObj = null;

            switch (functionName)
            {
                #region -- findEmployeeByKeyword --
                case "findEmployeeByKeyword":
                    contentObj = dbEIP.ViewEmpInfoWithoutLeaves.Where(x => x.EmpName.Contains(keyword) || x.Account.Contains(keyword)).Take(5);
                    break;
                #endregion

                #region -- findEmployeeIncludeLeaveByKeyword --
                case "findEmployeeIncludeLeaveByKeyword":
                    contentObj = bpmDB.TblEmployees.Where(x => (x.CEmployeeAccount.Contains(keyword) || x.CEmployeeCName.Contains(keyword))).Take(5);
                    break;
                #endregion

                case "findEmployeeByKeywordAndPrivilege":

                    //只能找到自己or下屬的資料
                    var erpId = HttpContext.Session.GetString(SessionKey.USER_ERP_ID);
                    //erpId = "10000479";
                    var deptMgrBean = dbEIP.ViewDeptMgrs.FirstOrDefault(x => x.ErpId == erpId);
                    if (deptMgrBean != null)
                    {
                        var deptCodeList = GetAllChildDept(deptMgrBean.DeptCode);
                        contentObj = dbEIP.ViewEmpInfoWithoutLeaves.Where(x => deptCodeList.Contains(x.DeptId) && (x.EmpName.Contains(keyword) || x.Account.Contains(keyword))).Take(5);
                    }
                    else
                    {
                        contentObj = dbEIP.ViewEmpInfoWithoutLeaves.Where(x => x.ErpId == erpId);
                    }
                    break;

                //#region -- findEmployeeInfo --
                //case "findEmployeeInfo":
                //    TblEmployee empBean = new TblEmployee();

                //    var beanE = dbEIP.People.FirstOrDefault(x => x.Account.ToLower() == keyword.ToLower() && (x.LeaveDate == null && x.LeaveReason == null));

                //    if (beanE != null)
                //    {
                //        empBean.CEmployeeNo = beanE.Account.Trim();
                //        empBean.CEmployeeErpid = beanE.ErpId.Trim();
                //        empBean.CEmployeeCName = beanE.Name2.Trim();
                //        empBean.CEmployeeEname = beanE.Name.Trim();
                //        empBean.CEmployeeWorkPlace = beanE.WorkPlace.Trim();
                //        empBean.CEmployeeCompanyPhoneExt = beanE.Extension.Trim();
                //        empBean.CEmployeeCompanyCode = beanE.CompCde.Trim();

                //        var beanD = dbEIP.Departments.FirstOrDefault(x => x.Id == beanE.DeptId);

                //        //if (beanD != null)
                //        //{
                //        //    empBean.DepartmentNO = beanD.ID.Trim();
                //        //    empBean.D = beanD.Name2.Trim();
                //        //}
                //    }

                //    contentObj = empBean;
                //    break;
                //#endregion

                //#region -- findEmployeeIncludeLeaveInfo --
                //case "findEmployeeIncludeLeaveInfo":
                //    EmployeeBean empBean2 = new EmployeeBean();

                //    var beanE2 = dbEIP.Person.OrderByDescending(x => x.ID).FirstOrDefault(x => x.Account.ToLower() == keyword.ToLower());

                //    if (beanE2 != null)
                //    {
                //        empBean2.EmployeeNO = beanE2.Account.Trim();
                //        empBean2.EmployeeERPID = beanE2.ERP_ID.Trim();
                //        empBean2.EmployeeCName = beanE2.Name2.Trim();
                //        empBean2.EmployeeEName = beanE2.Name.Trim();
                //        empBean2.WorkPlace = beanE2.Work_Place.Trim();
                //        empBean2.PhoneExt = beanE2.Extension.Trim();
                //        empBean2.CompanyCode = beanE2.Comp_Cde.Trim();

                //        var beanD = dbEIP.Department.FirstOrDefault(x => x.ID == beanE2.DeptID);

                //        if (beanD != null)
                //        {
                //            empBean2.DepartmentNO = beanD.ID.Trim();
                //            empBean2.DepartmentName = beanD.Name2.Trim();
                //        }
                //    }

                //    contentObj = empBean2;
                //    break;
                //#endregion

                case "findCrmOppByOppNo":
                    contentObj = proxyDB.TbCrmOppHeads.FirstOrDefault(x => x.CrmOppNo == OppNoFormat(keyword));
                    break;
                default:
                    break;
            }

            string json = JsonConvert.SerializeObject(contentObj);
            return Content(json, "application/json");
        }

        [HttpPost]
        public ActionResult AjaxFileUpload(IFormFile upload, bool needResize, int? width, int? height)
        {
            try
            {
                CkFileBean bean = new CkFileBean();
                if (upload != null)
                {
                    string webRootPath = _HostEnvironment.WebRootPath + "/upload";

                    string fileId = Guid.NewGuid().ToString();
                    string fileOrgName = upload.FileName;
                    string fileName = fileId + Path.GetExtension(upload.FileName);
                    string path = Path.Combine(webRootPath, fileName);
                    bean.fileId = fileId;
                    bean.fileName = fileName;
                    bean.fileOrgName = fileOrgName;
                    bean.url = "https://www.etatung.com/upload/" + fileName;
                    bean.uploaded = 1;
                    using (Stream fileStream = new FileStream(path, FileMode.Create))
                    {
                        upload.CopyToAsync(fileStream);
                    }
                    //縮圖
                    //if (needResize)
                    //{
                    //    Image img = resizeImageFromFile(path, (int)width, (int)height, true, true);
                    //    img.Save(path);
                    //    GetPicThumbnail(path, path, 80);
                    //}

                }
                ViewBag.Message = "File Uploaded Successfully!!";
                return Json(bean);
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }

        public string OppNoFormat(string oppNo)
        {
            var _oppNo = "0000000000" + oppNo;
            var oppNoFormat = _oppNo.Substring(_oppNo.Length - 10);
            return oppNoFormat;
        }

        public IActionResult SaveWH(IFormCollection formCollection)
        {
            TbWorkingHoursMain bean;
            int prId = 0;
            if (!string.IsNullOrEmpty(formCollection["Id"]))
            {
                bean = psipDB.TbWorkingHoursMains.Find(int.Parse(formCollection["Id"]));
                bean.Whtype = formCollection["ddl_WHType"].ToString();
                bean.ActType = formCollection["ddl_ActType"].ToString();
                bean.CrmOppNo = formCollection["ddl_CrmOppNo"].ToString();
                bean.CrmOppName = formCollection["hid_CrmOppName"].ToString();
                bean.WhDescript = formCollection["tbx_WhDescript"].ToString();
                bean.StartTime = formCollection["tbx_StartDate"].ToString() + " " + formCollection["hid_StartTime"].ToString();
                bean.EndTime = formCollection["tbx_EndDate"].ToString() + " " + formCollection["hid_EndTime"].ToString();
                bean.UpdateTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                prId = bean.PrId ?? 0;

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
                bean.CrmOppNo = formCollection["ddl_CrmOppNo"].ToString();
                bean.CrmOppName = formCollection["hid_CrmOppName"].ToString();
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
                var pjInfoBean = psipDB.TbProPjinfos.FirstOrDefault(x => x.CrmOppNo == bean.CrmOppNo);

                WorkingHoursController workingHoursController = new WorkingHoursController();

                //取得MileStone
                string ms = workingHoursController.GetMileStone(bean.CrmOppNo, bean.StartTime, bean.EndTime);

                var workHours = Math.Ceiling((decimal)bean.Labor / 60);

                //int? prId, string oppNo, string bundleMs, string bundleTask, string impBy, string ImplementersCount, string Attendees, string place, string startDate, string endDate, string workHours, string withPpl, string withPplPhone, string desc, string attach)
                var _prId = workingHoursController.SavePjRecord(prId, bean.CrmOppNo, ms, "", "", "1", "", "", bean.StartTime, bean.EndTime, workHours.ToString(), "", "", bean.WhDescript, "");

                bean.PrId = _prId;
            }



            psipDB.SaveChanges();

            return RedirectToAction("CreateWH");
        }

        /// <summary>
        /// 取得所有的子單位代碼清單
        /// </summary>
        /// <param name="parentDeptId"></param>
        /// <returns></returns>
        public List<string> GetAllChildDept(string parentDeptId)
        {
            List<string> deptIds = new List<string>();
            Dictionary<string, string> parentDict = new Dictionary<string, string>();

            parentDict.Add(parentDeptId, parentDeptId);

            bool noneChildAnyMore = true;

            while (noneChildAnyMore)
            {
                if (parentDict.Keys.Count() == 0)
                {
                    noneChildAnyMore = false;
                    break;
                }
                else
                {
                    var parentIds = new List<string>(parentDict.Keys);

                    foreach (var parentId in parentIds)
                    {

                        var deptBeans = dbEIP.Departments.Where(x => x.ParentId == parentId);

                        //加入單位id後，就從parentDict移除
                        deptIds.Add(parentId);
                        parentDict.Remove(parentId);

                        if (deptBeans.Count() > 0)
                        {
                            foreach (var deptBean in deptBeans)
                            {
                                parentDict.Add(deptBean.Id, deptBean.Id);
                            }
                        }
                    }

                }

            }

            return deptIds;
        }


        public struct CkFileBean
        {
            public int uploaded;
            public string fileName;
            public string fileOrgName;
            public string url;
            public string fileId;
        }
    }
}
