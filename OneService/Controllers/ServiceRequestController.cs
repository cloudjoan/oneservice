using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneService.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;


namespace OneService.Controllers
{
    public class ServiceRequestController : Controller
    {
        /// <summary>
        /// 登入者帳號
        /// </summary>
        //string pLoginAccount = string.Empty;
        string pLoginAccount = @"etatung\elvis.chang";  //工程師
        //string pLoginAccount = @"etatung\Jordan.Chang"; //備品主管(電腦)
        //string pLoginAccount = @"etatung\danny.hu";     //備品主管(系統)
        //string pLoginAccount = @"etatung\Dale.Wu";      //台北1區管理員        
        //string pLoginAccount = @"etatung\along.chou";   //台北2區管理員
        //string pLoginAccount = @"etatung\Debby.Liu";    //台北1、2區檢測員
        //string pLoginAccount = @"etatung\Angel.Fang";   //台中備品管理員
        //string pLoginAccount = @"etatung\Aniki.Huang";  //台中備品檢測員

        //string pLoginAccount = @"etatung\Vic.Chen";  //群輝工程師
        //string pLoginAccount = @"etatung\Lun.Hsu";     //群輝高雄備品檢測員(管理員)

        /// <summary>全域變數</summary>
        string pMsg = "";

        /// <summary>
        /// 登入者姓名
        /// </summary>
        string pLoginName = string.Empty;

        /// <summary>
        /// 登入者ERPID
        /// </summary>
        string pLoginERPID = string.Empty;

        /// <summary>
        /// 登入者是否為MIS(true.是 false.否)
        /// </summary>
        bool pIsMIS = false;

        /// <summary>
        /// 程式作業編號檔系統ID(ALL，固定的GUID)
        /// </summary>
        string pSysOperationID = "F8EFC55F-FA77-4731-BB45-2F2147244A2D";

        /// <summary>
        /// 程式作業編號檔系統ID(一般服務請求)
        /// </summary>
        string pOperationID_GenerallySR = "869FC989-1049-4266-ABDE-69A9B07BCD0A";
        //edit by elvis 2022/10/19 End

        CommonFunction CMF = new CommonFunction();
        PSIPContext psipDb = new PSIPContext();
        TAIFContext bpmDB = new TAIFContext();
        ERP_PROXY_DBContext dbProxy = new ERP_PROXY_DBContext();
        MCSWorkflowContext dbEIP = new MCSWorkflowContext();

        public IActionResult GenerallySR()
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.cLoginUser_EmployeeNO = EmpBean.EmployeeNO;
            ViewBag.cLoginUser_ERPID = EmpBean.EmployeeERPID;
            ViewBag.cLoginUser_WorkPlace = EmpBean.WorkPlace;
            ViewBag.cLoginUser_DepartmentName = EmpBean.DepartmentName;
            ViewBag.cLoginUser_DepartmentNO = EmpBean.DepartmentNO;
            ViewBag.cLoginUser_CompCode = EmpBean.CompanyCode;
            ViewBag.cLoginUser_BUKRS = EmpBean.BUKRS;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            ViewBag.pOperationID = pOperationID_GenerallySR;
            ViewBag.pStatus = "E0001";
            return View();
        }

        #region -----↓↓↓↓↓共用方法 ↓↓↓↓↓-----

        #region 取得登入帳號權限
        /// <summary>
        /// 取得登入帳號權限
        /// </summary>
        public void getLoginAccount()
        {
            //pLoginAccount = User.Identity.Name;

            #region One Service相關帳號
            pIsMIS = CMF.getIsMIS(pLoginAccount, pSysOperationID);

            ViewBag.pIsMIS = pIsMIS;
            #endregion            
        }
        #endregion

        #region Ajax SAP客戶代號/客戶名稱查詢
        /// <summary>
        /// Ajax SAP客戶代號/客戶名稱查詢
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="compcde">公司別</param>
        /// <returns></returns>
        public IActionResult findCustByKeywordAndComp(string keyword, string compcde)
        {
            object contentObj = null;

            switch (compcde)
            {
                case "Comp-1": //大世科
                    compcde = "12G0";
                    break;

                case "Comp-2": //群輝
                    compcde = "16G0";
                    break;

                case "Comp-3": //大世科技上海
                    compcde = "69G0";
                    break;

                case "Comp-4": //協科
                    compcde = "22G0";
                    break;
            }

            contentObj = CMF.findCustByKeywordAndComp(keyword, compcde);

            string json = JsonConvert.SerializeObject(contentObj);
            return Content(json, "application/json");
        }
        #endregion

        #region -- 取得客戶聯絡人 --
        /// <summary>
        /// 取得客戶聯絡人
        /// </summary>
        /// <param name="cBUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="CustomerID">客戶代號</param>        
        /// <returns></returns>
        public IActionResult GetContactInfo(string cBUKRS, string CustomerID)
        {
            object contentObj = CMF.GetContactInfo(cBUKRS, CustomerID);            

            return Json(contentObj);
        }
        #endregion

        #region -- Ajax儲存客戶聯絡人 --
        /// <summary>
        /// /Ajax儲存客戶聯絡人
        /// </summary>        
        /// <param name="cAddContactID">程式作業編號檔系統ID</param>
        /// <param name="cBUKRS">工廠別(T012、T016、C069、T022)</param>
        /// <param name="cCustomerID">客戶代號</param>
        /// <param name="cCustomerName">客戶名稱</param>
        /// <param name="cAddContactName">客戶聯絡人姓名</param>
        /// <param name="cAddContactCity">客戶聯絡人城市</param>
        /// <param name="cAddContactAddress">客戶聯絡人地址</param>
        /// <param name="cAddContactPhone">客戶聯絡人電話</param>
        /// <param name="cAddContactEmail">客戶聯絡人Email</param>
        /// <param name="ModifiedUserName">修改人姓名</param>
        /// <returns></returns>
        public IActionResult SaveContactInfo(string cAddContactID, string cBUKRS, string cCustomerID, string cCustomerName, string cAddContactName,
                                           string cAddContactCity, string cAddContactAddress, string cAddContactPhone, string cAddContactEmail, string ModifiedUserName)
        {
            string tBpmNo = "GenerallySR";

            var bean = dbProxy.CustomerContacts.FirstOrDefault(x => x.ContactId.ToString() == cAddContactID && x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == cCustomerID && x.ContactName == cAddContactName);

            if (bean != null) //修改
            {
                bean.ContactCity = cAddContactCity;
                bean.ContactAddress = cAddContactAddress;
                bean.ContactPhone = cAddContactPhone;
                bean.ContactEmail = cAddContactEmail;

                bean.ModifiedUserName = ModifiedUserName;
                bean.ModifiedDate = DateTime.Now;
            }
            else //新增
            {
                CustomerContact bean1 = new CustomerContact();

                bean1.ContactId = new Guid(cAddContactID);
                bean1.Kna1Kunnr = cCustomerID;
                bean1.Kna1Name1 = cCustomerName;
                bean1.Knb1Bukrs = cBUKRS;
                bean1.ContactType = "0";
                bean1.ContactName = cAddContactName;
                bean1.ContactCity = cAddContactCity;
                bean1.ContactAddress = cAddContactAddress;
                bean1.ContactPhone = cAddContactPhone;
                bean1.ContactEmail = cAddContactEmail;
                bean1.BpmNo = tBpmNo;

                bean1.ModifiedUserName = ModifiedUserName;
                bean1.ModifiedDate = DateTime.Now;

                dbProxy.CustomerContacts.Add(bean1);
            }

            var result = dbProxy.SaveChanges();

            return Json(result);
        }
        #endregion

        #endregion -----↑↑↑↑↑共用方法 ↑↑↑↑↑-----
    }
}
