using Microsoft.AspNetCore.Mvc;
using OneService.Models;
using OneService.Utils;

namespace OneService.Controllers
{
    public class ContractController : Controller
    {
        /// <summary>
        /// 登入者帳號
        /// </summary>
        string pLoginAccount = string.Empty;

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
        /// 登入者是否為客服主管(true.是 false.否)
        /// </summary>
        bool pIsCSManager = false;

        /// <summary>
        /// 登入者是否為客服人員(true.是 false.否)
        /// </summary>
        bool pIsCS = false;

        /// <summary>
        /// 登入者是否為管理者(true.是 false.否)
        /// </summary>
        bool pIsManager = false;

        /// <summary>
        /// 登入者是否可編輯服務案件
        /// </summary>
        bool pIsCanEditSR = false;

        /// <summary>
        /// 服務ID
        /// </summary>
        string pSRID = string.Empty;

        /// <summary>
        /// 程式作業編號檔系統ID(ALL，固定的GUID)
        /// </summary>
        string pSysOperationID = "F8EFC55F-FA77-4731-BB45-2F2147244A2D";

        /// <summary>
        /// 程式作業編號檔系統ID(合約主數據查詢/維護作業)
        /// </summary>
        static string pOperationID_MaintainSR = "5B80D6AB-9143-4916-9273-ADFAEA9A61ED";

        /// <summary>
        /// 公司別(T012、T016、C069、T022)
        /// </summary>
        static string pCompanyCode = string.Empty;

        CommonFunction CMF = new CommonFunction();

        PSIPContext psipDb = new PSIPContext();
        TSTIONEContext dbOne = new TSTIONEContext();
        TAIFContext bpmDB = new TAIFContext();
        ERP_PROXY_DBContext dbProxy = new ERP_PROXY_DBContext();
        MCSWorkflowContext dbEIP = new MCSWorkflowContext();

        public IActionResult Index()
        {
            return View();
        }

        #region 取得登入帳號權限
        /// <summary>
        /// 取得登入帳號權限
        /// </summary>
        public void getLoginAccount()
        {
            #region 測試用            
            //pLoginAccount = @"etatung\Allen.Chen";      //陳勁嘉(主管)
            //pLoginAccount = @"etatung\Boyen.Chen";      //陳建良(主管)
            //pLoginAccount = @"etatung\Aniki.Huang";     //黃志豪(主管)
            //pLoginAccount = @"etatung\jack.hung";       //洪佑典(主管)
            //pLoginAccount = @"etatung\Allen.Tang";      //湯文華(業務主管)
            //pLoginAccount = @"etatung\Sam.Lee";         //李思霖(業務)
            //pLoginAccount = @"etatung\Julia.Hsu";       //徐瑄辰(祕書)
            //pLoginAccount = @"etatung\Steve.Guo";       //郭翔元         
            //pLoginAccount = @"etatung\Wenjui.Chan";     //詹文瑞        
            //pLoginAccount = @"etatung\Jordan.Chang";    //張景堯
            #endregion

            pLoginAccount = HttpContext.Session.GetString(SessionKey.USER_ACCOUNT); //正式用

            #region One Service相關帳號
            pIsMIS = CMF.getIsMIS(pLoginAccount, pSysOperationID);
            pIsCSManager = CMF.getIsCustomerServiceManager(pLoginAccount, pSysOperationID);
            pIsCS = CMF.getIsCustomerService(pLoginAccount, pSysOperationID);

            ViewBag.pIsMIS = pIsMIS;
            ViewBag.pIsCSManager = pIsCSManager;
            ViewBag.pIsCS = pIsCS;
            #endregion            
        }
        #endregion

        #region -----↓↓↓↓↓合約主數據查詢/維護 ↓↓↓↓↓-----       

        #region 合約主數據查詢
        public IActionResult QueryContractMain()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

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
            ViewBag.cLoginUser_ProfitCenterID = EmpBean.ProfitCenterID;
            ViewBag.cLoginUser_CostCenterID = EmpBean.CostCenterID;
            ViewBag.cLoginUser_CompCode = EmpBean.CompanyCode;
            ViewBag.cLoginUser_BUKRS = EmpBean.BUKRS;
            ViewBag.pIsManager = EmpBean.IsManager;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");

            pCompanyCode = EmpBean.BUKRS;
            pIsManager = EmpBean.IsManager;
            #endregion            

            return View();
        }
        #endregion

        #region 合約主數據查詢結果
        public IActionResult QueryContractMainResult()
        {
            return View();
        }
        #endregion

        #region 合約主數據維護
        public IActionResult ContractMain()
        {
            return View();
        }
        #endregion

        #endregion -----↑↑↑↑↑合約主數據查詢/維護 ↑↑↑↑↑-----
    }
}
