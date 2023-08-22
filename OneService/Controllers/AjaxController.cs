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
        TSTIONEContext oneDB = new TSTIONEContext();

        public AjaxController(IWebHostEnvironment hostingEnvironment)
        {
            _HostEnvironment = hostingEnvironment;
        }
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
                    //erpId = "10012338";
                    var deptMgrBean = dbEIP.ViewDeptMgrs.FirstOrDefault(x => x.ErpId == erpId && x.Disabled != -1);
                    if (deptMgrBean != null)
                    {
                        var deptCodeList = GetAllChildDept(deptMgrBean.DeptCode);
                        contentObj = dbEIP.ViewEmpInfoWithoutLeaves.Where(x => deptCodeList.Contains(x.DeptId) && (x.EmpName.Contains(keyword) || x.EmpEname.Contains(keyword))).Take(5);
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

				case "findCrmOppByKeyword":
					contentObj = psipDB.ViewProPjOppInfos.Where(x => x.CrmOppNo.Contains(keyword) || x.OppDescription.Contains(keyword));
					break;
				default:
                    break;
            }

            string json = JsonConvert.SerializeObject(contentObj);
            return Content(json, "application/json");
        }




		[RequestSizeLimit(100 * 1024 * 1024)]
		[HttpPost]
        public async Task<ActionResult> AjaxFileUpload()
        {
            TbOneDocument docBean = new TbOneDocument();
            string webRootPath = _HostEnvironment.WebRootPath + "/files";

            try
            {
                foreach (var file in Request.Form.Files)
                {

                    System.Diagnostics.Debug.WriteLine(file.FileName);

                    string fileId = Guid.NewGuid().ToString();
                    string fileOrgName = file.FileName;
                    string fileName = fileId + Path.GetExtension(file.FileName);
                    string path = Path.Combine(webRootPath, fileName);

                    docBean.Id = new Guid(fileId);
                    docBean.FileName = fileName;
                    docBean.FileOrgName = fileOrgName;
                    docBean.FileExt = Path.GetExtension(file.FileName);
                    docBean.InsertTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                    oneDB.TbOneDocuments.Add(docBean);
                    oneDB.SaveChanges();

                    using (Stream fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            catch
            {

            }

            return Json(docBean);
        }


        public ActionResult GetFileData(string fileId)
        {
            if (!string.IsNullOrEmpty(fileId))
            {
                var fileBean = oneDB.TbOneDocuments.FirstOrDefault(x => x.Id == new Guid(fileId));
                return Json(fileBean);
            }
            return Json(null);
        }

        public ActionResult GetWhTypeByUpTypeCode(string upTypeCode)
        {
            var beans = psipDB.TbWhTypes.Where(x => x.UpTypeCode == upTypeCode).OrderBy(x => x.Sort);
            return Json(beans);
        }

        public string OppNoFormat(string oppNo)
        {
            var _oppNo = "0000000000" + oppNo;
            var oppNoFormat = _oppNo.Substring(_oppNo.Length - 10);
            return oppNoFormat;
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
