using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OneService.Models;

namespace OneService.Controllers
{
	public class AjaxController : Controller
	{
        private readonly IWebHostEnvironment _HostEnvironment;

        TAIFContext bpmDB = new TAIFContext();
        MCSWorkflowContext dbEIP = new MCSWorkflowContext();
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
                    contentObj = bpmDB.TblEmployees.Where(x => (x.CEmployeeAccount.Contains(keyword) || x.CEmployeeCName.Contains(keyword)) && (x.CEmployeeLeaveReason == null && x.CEmployeeLeaveDay == null)).Take(5);
                    break;
                #endregion

                #region -- findEmployeeIncludeLeaveByKeyword --
                case "findEmployeeIncludeLeaveByKeyword":
                    contentObj = bpmDB.TblEmployees.Where(x => (x.CEmployeeAccount.Contains(keyword) || x.CEmployeeCName.Contains(keyword))).Take(5);
                    break;
                #endregion

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
