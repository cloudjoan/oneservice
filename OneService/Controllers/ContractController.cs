using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Extensions.Primitives;
using NPOI.SS.Formula.Functions;
using OneService.Models;
using OneService.Utils;
using System.Data;
using System.Security.Policy;
using System.Text;

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
        /// 文件編號
        /// </summary>
        string pContractID = string.Empty;

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

        #region 取得人員相關資料
        public void getEmployeeInfo()
        {
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
            getEmployeeInfo();

            return View();
        }
        #endregion

        #region 合約主數據查詢結果
        /// <summary>
        /// 合約主數據查詢結果
        /// </summary>
        /// <param name="cIsSubContract">合約類型(空白.客戶 Y.供應商)</param>
        /// <param name="cContractID">文件編號</param>
        /// <param name="cCustomerID">客戶代號</param>
        /// <param name="cCustomerName">客戶名稱</param>
        /// <param name="cSoSales">業務員ERPID</param>
        /// <param name="cMASales">維護業務員ERPID</param>
        /// <param name="cMainEngineerID">主要工程師ERPID</param>
        /// <param name="cStartDate">合約到期日(起)</param>
        /// <param name="cEndDate">合約到期日(迄)</param>
        /// <param name="cAssignMainEngineer">未指派主要工程師(Y.未指派)</param>
        /// <returns></returns>
        public IActionResult QueryContractMainResult(string cIsSubContract, string cContractID, string cCustomerID, string cCustomerName, 
                                                   string cSoSales, string cMASales, string cMainEngineerID, string cStartDate, string cEndDate, string cAssignMainEngineer)
        {
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            DataTable dt = null;

            StringBuilder tSQL = new StringBuilder();

            string ttWhere = string.Empty;
            string ttSelect = string.Empty;
            string tUrl = string.Empty;
            string tIsSubContract = string.Empty;
            string tStartDate = string.Empty;
            string tEndDate = string.Empty;
            string tDesc = string.Empty;
            string tContractNotes = string.Empty;

            ttSelect = "ISNULL((select D.cEngineerID from TB_ONE_ContractDetail_ENG D where  M.cContractID = D.cContractID and D.cIsMainEngineer = 'Y'),'') as cEngineerID,";
            ttSelect += "ISNULL((select D.cEngineerName from TB_ONE_ContractDetail_ENG D where  M.cContractID = D.cContractID and D.cIsMainEngineer = 'Y'),'') as cEngineerName";

            #region 合約類型
            if (!string.IsNullOrEmpty(cIsSubContract))
            {
                ttWhere += "AND M.cIsSubContract = '" + cIsSubContract + "' " + Environment.NewLine;
            }
            else
            {
                ttWhere += "AND (M.cIsSubContract = '' or M.cIsSubContract is null) " + Environment.NewLine;
            }
            #endregion

            #region 文件編號
            if (!string.IsNullOrEmpty(cContractID))
            {
                ttWhere += "AND M.cContractID LIKE N'%" + cContractID.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 客戶代號
            if (!string.IsNullOrEmpty(cCustomerID))
            {
                ttWhere += "AND M.cCustomerID = '" + cCustomerID + "' " + Environment.NewLine;
            }
            #endregion

            #region 客戶名稱
            if (!string.IsNullOrEmpty(cCustomerName))
            {
                ttWhere += "AND M.cCustomerName LIKE N'%" + cCustomerName.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 業務員ERPID
            if (!string.IsNullOrEmpty(cSoSales))
            {
                ttWhere += "AND M.cSoSales = '" + cSoSales + "' " + Environment.NewLine;
            }
            #endregion

            #region 維護業務員ERPID
            if (!string.IsNullOrEmpty(cMASales))
            {
                ttWhere += "AND M.cSoSales = '" + cMASales + "' " + Environment.NewLine;
            }
            #endregion           

            #region 申請日期
            if (!string.IsNullOrEmpty(cStartDate))
            {
                ttWhere += "AND Convert(varchar(10),M.cStartDate,111) >= N'" + cStartDate.Replace("-", "/") + "' ";
            }

            if (!string.IsNullOrEmpty(cEndDate))
            {
                ttWhere += "AND Convert(varchar(10),M.cEndDate,111) <= N'" + cEndDate.Replace("-", "/") + "' ";
            }
            #endregion 申請日期

            #region 主要工程師ERPID
            if (!string.IsNullOrEmpty(cMainEngineerID))
            {
                ttWhere += "AND D.disabled = 0 AND D.cIsMainEngineer='Y' AND D.cEngineerID = '" + cMainEngineerID + "' " + Environment.NewLine;                
            }
            #endregion

            #region 未指派主要工程師
            if (!string.IsNullOrEmpty(cAssignMainEngineer))
            {
                ttSelect = "'' as cEngineerID,'' as cEngineerName";
                ttWhere += "AND (select count(*) from TB_ONE_ContractDetail_ENG D where M.cContractID = D.cContractID) = 0 " + Environment.NewLine;
            }
            #endregion

            #region 組待查詢清單

            #region SQL語法
            tSQL.AppendLine(" Select distinct M.*, " + ttSelect);            
            tSQL.AppendLine(" From TB_ONE_ContractMain M");
            tSQL.AppendLine(" left join TB_ONE_ContractDetail_ENG D on M.cContractID = D.cContractID");
            tSQL.AppendLine(" Where 1=1 AND M.disabled = 0 " + ttWhere);
            #endregion

            dt = CMF.getDataTableByDb(tSQL.ToString(), "dbOne");            

            foreach (DataRow dr in dt.Rows)
            {
                string[] QueryInfo = new string[18];
                
                tUrl = "../Contract/ContractMain?ContractID=" + dr["cContractID"].ToString();
                tIsSubContract = dr["cIsSubContract"].ToString() == "Y" ? "供應商" : "客戶";
                tDesc = dr["cDesc"].ToString().Replace("\r\n", "<br/>");
                tContractNotes = dr["cContractNotes"].ToString().Replace("\r\n", "<br/>");
                tStartDate = Convert.ToDateTime(dr["cStartDate"].ToString()).ToString("yyyy-MM-dd");
                tEndDate = Convert.ToDateTime(dr["cEndDate"].ToString()).ToString("yyyy-MM-dd");

                QueryInfo[0] = dr["cContractID"].ToString();      //文件編號
                QueryInfo[1] = tUrl;                            //合約主數據主檔URL
                QueryInfo[2] = tIsSubContract;                  //合約類型                
                QueryInfo[3] = dr["cSoNo"].ToString();           //銷售單號
                QueryInfo[4] = dr["cSoSales"].ToString();        //業務ERPID
                QueryInfo[5] = dr["cSoSalesName"].ToString();    //業務姓名
                QueryInfo[6] = dr["cSoSalesASS"].ToString();     //業務祕書ERPID
                QueryInfo[7] = dr["cSoSalesASSName"].ToString();  //業務祕書姓名
                QueryInfo[8] = dr["cMASales"].ToString();        //維護業務ERPID
                QueryInfo[9] = dr["cMASalesName"].ToString();    //維護業務姓名
                QueryInfo[10] = dr["cEngineerID"].ToString();     //主要工程師ERPID
                QueryInfo[11] = dr["cEngineerName"].ToString();  //主要工程師姓名
                QueryInfo[12] = dr["cCustomerID"].ToString();    //客戶代號
                QueryInfo[13] = dr["cCustomerName"].ToString();  //客戶名稱
                QueryInfo[14] = tDesc;                         //訂單說明
                QueryInfo[15] = tStartDate;                    //維護開始
                QueryInfo[16] = tEndDate;                      //維護結束
                QueryInfo[17] = tContractNotes;                //合約備註

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #region 合約主數據維護
        public IActionResult ContractMain()
        {
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            string tSubUrl = string.Empty;
            string tObjUrl = string.Empty;
            string tSubNotes = string.Empty;

            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();
            getEmployeeInfo();

            #region Request參數            
            if (HttpContext.Request.Query["ContractID"].FirstOrDefault() != null)
            {
                pContractID = HttpContext.Request.Query["ContractID"].FirstOrDefault();
            }
            #endregion

            #region 取得合約主數據主檔
            var beanM = dbOne.TbOneContractMains.FirstOrDefault(x => x.Disabled == 0 && x.CContractId == pContractID);

            if (beanM != null)
            {
                ViewBag.cContractID = beanM.CContractId;
                ViewBag.cSoNo = beanM.CSoNo;
                ViewBag.cCustomerID = beanM.CCustomerId;
                ViewBag.cCustomerName = beanM.CCustomerName;
                ViewBag.cSoSales = beanM.CSoSales;
                ViewBag.cSoSalesName = beanM.CSoSalesName;
                ViewBag.cSoSalesASS = beanM.CSoSalesAss;
                ViewBag.cSoSalesASSName = beanM.CSoSalesAssname;
                ViewBag.cMASales = beanM.CMasales;
                ViewBag.cMASalesName = beanM.CMasalesName;
                ViewBag.cDesc = beanM.CDesc;
                ViewBag.cStartDate = Convert.ToDateTime(beanM.CStartDate).ToString("yyyy-MM-dd");
                ViewBag.cEndDate = Convert.ToDateTime(beanM.CEndDate).ToString("yyyy-MM-dd");
                ViewBag.cMACycle = beanM.CMacycle;
                ViewBag.cMANotes = beanM.CManotes;

                ViewBag.cMAAddress = beanM.CMaaddress;
                ViewBag.cContractNotes = beanM.CContractNotes;
                ViewBag.cSLARESP = beanM.CSlaresp;
                ViewBag.cSLASRV = beanM.CSlasrv;
                ViewBag.cMANotes = beanM.CManotes;
                ViewBag.cContractReport = beanM.CContractReport;
            }
            #endregion

            #region 取得合約主要工程師
            var beanEng = dbOne.TbOneContractDetailEngs.FirstOrDefault(x => x.Disabled == 0 && x.CIsMainEngineer == "Y" && x.CContractId == pContractID);

            if (beanEng != null)
            {
                ViewBag.cMainEngineerID = beanEng.CEngineerName;
            }
            #endregion

            #region 取得下包約主檔
            var beansSub = dbOne.TbOneContractDetailSubs.Where(x => x.Disabled == 0 && x.CContractId == pContractID);

            foreach (var beanSub in beansSub)
            {
                string[] QueryInfo = new string[6];

                tSubUrl = "javascript:void(0);";
                tObjUrl = "javascript:void(0);";
                tSubNotes = beanSub.CSubNotes.Replace("\n", "<br/>");                

                QueryInfo[0] = beanSub.CSubContractId;      //下包文件編號
                QueryInfo[1] = tSubUrl;                   //下包文件編號URL
                QueryInfo[2] = beanSub.CSubSupplierId;      //下包商統一編號
                QueryInfo[3] = beanSub.CSubSupplierName;    //下包商名稱                
                QueryInfo[4] = tSubNotes;                 //下包備註
                QueryInfo[5] = tObjUrl;                   //標的URL                

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #endregion -----↑↑↑↑↑合約主數據查詢/維護 ↑↑↑↑↑-----
    }
}
