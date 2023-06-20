using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using OneService.Models;
using OneService.Utils;
using Org.BouncyCastle.Asn1.X509;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using static OneService.Controllers.ServiceRequestController;
using NPOI.XSSF.UserModel;
using System.Diagnostics.Contracts;
using Microsoft.CodeAnalysis.Operations;
using NPOI.SS.UserModel;

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
        /// 登入者是否可編輯合約主數據相關內容
        /// </summary>
        bool pIsCanEdit = false;

        /// <summary>
        /// 登入者是否可閱讀合約書link
        /// </summary>
        bool pIsCanRead = false;

        /// <summary>
        /// 主約文件編號
        /// </summary>
        string pContractID = string.Empty;

        /// <summary>
        /// 下包文件編號
        /// </summary>
        string pSubContractID = string.Empty;

        /// <summary>
        /// 程式作業編號檔系統ID(ALL，固定的GUID)
        /// </summary>
        string pSysOperationID = "F8EFC55F-FA77-4731-BB45-2F2147244A2D";

        /// <summary>
        /// 程式作業編號檔系統ID(一般服務)
        /// </summary>
        static string pOperationID_GenerallySR = "869FC989-1049-4266-ABDE-69A9B07BCD0A";     

        /// <summary>
        /// 程式作業編號檔系統ID(合約主數據查詢/維護)
        /// </summary>
        string pOperationID_Contract = "A9556C2C-E5DE-4745-A76B-5F2E1F69A3A9";

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
        /// <param name="cIsSubContract">合約類型(N.客戶 Y.供應商)</param>
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
                ttWhere += "AND (M.cIsSubContract = 'N' or M.cIsSubContract is null) " + Environment.NewLine;
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

            #region 合約到期日
            if (!string.IsNullOrEmpty(cStartDate))
            {
                ttWhere += "AND Convert(varchar(10),M.cStartDate,111) >= N'" + cStartDate.Replace("-", "/") + "' ";
            }

            if (!string.IsNullOrEmpty(cEndDate))
            {
                ttWhere += "AND Convert(varchar(10),M.cEndDate,111) <= N'" + cEndDate.Replace("-", "/") + "' ";
            }
            #endregion

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
                
                tIsSubContract = dr["cIsSubContract"].ToString() == "Y" ? "供應商" : "客戶";
                tDesc = dr["cDesc"].ToString().Replace("\r\n", "<br/>");
                tContractNotes = dr["cContractNotes"].ToString().Replace("\r\n", "<br/>");
                tStartDate = Convert.ToDateTime(dr["cStartDate"].ToString()).ToString("yyyy-MM-dd");
                tEndDate = Convert.ToDateTime(dr["cEndDate"].ToString()).ToString("yyyy-MM-dd");

                if (dr["cIsSubContract"].ToString() == "Y") //下包
                {
                    tUrl = "../Contract/ContractDetailSub?SubContractID=" + dr["cContractID"].ToString();
                }
                else //非下包
                {
                    tUrl = "../Contract/ContractMain?ContractID=" + dr["cContractID"].ToString();
                }

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
            
            bool tIsFormal = false;

            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;
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

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;

            ViewBag.APIURLName = tAPIURLName;
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
                ViewBag.cTeamID = beanM.CTeamId;
                ViewBag.cBillCycle = beanM.CBillCycle;
                ViewBag.cBillNotes = beanM.CBillNotes;
                ViewBag.cContractReport = beanM.CContractReport;

                #region 取得服務團隊清單
                var SRTeamIDList = CMF.findSRTeamIDList(pCompanyCode, true);
                ViewBag.SRTeamIDList = SRTeamIDList;
                #endregion

                #region 是否可編輯合約主數據相關內容                
                pIsCanEdit = CMF.checkIsCanEditContracInfo(pOperationID_Contract, ViewBag.cLoginUser_ERPID, ViewBag.cLoginUser_EmployeeNO, ViewBag.cLoginUser_BUKRS, ViewBag.cLoginUser_CostCenterID, ViewBag.cLoginUser_DepartmentNO, pIsMIS, pIsCSManager, beanM.CContractId, "MAIN");

                if (pIsCanEdit)
                {
                    ViewBag.IsCanEdit = "Y";
                }
                else
                {
                    ViewBag.IsCanEdit = "N";
                }
                #endregion

                #region 是否可顯示合約書link
                pIsCanRead = checkIsCanReadContractReport(beanM.CContractId, beanM.CTeamId, tAPIURLName);

                if (pIsCanRead)
                {
                    ViewBag.IsCanRead = "Y";
                }
                else
                {
                    ViewBag.IsCanRead = "N";
                }
                #endregion
            }
            #endregion

            #region 取得合約主要工程師
            var beanEng = dbOne.TbOneContractDetailEngs.FirstOrDefault(x => x.Disabled == 0 && x.CIsMainEngineer == "Y" && x.CContractId == pContractID);

            if (beanEng != null)
            {
                ViewBag.cMainEngineerName = beanEng.CEngineerName;
            }
            #endregion

            #region 取得下包約主檔
            var beansSub = dbOne.TbOneContractDetailSubs.Where(x => x.Disabled == 0 && x.CContractId == pContractID);

            foreach (var beanSub in beansSub)
            {
                string[] QueryInfo = new string[6];

                tSubUrl = "../Contract/ContractDetailSub?SubContractID=" + beanSub.CSubContractId;
                tObjUrl = "../Contract/QueryContractDetailObj?ContractID=" + beanSub.CContractId + "&SubContractID=" + beanSub.CSubContractId;
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

        #region 判斷是否可顯示合約書link(Y.可顯示 N.不可顯示)
        /// <summary>
        /// 判斷是否可顯示合約書link(Y.可顯示 N.不可顯示)
        /// </summary>        
        /// <param name="cContractID">文件編號</param>
        /// <param name="cTeamID">服務團隊</param>
        /// <param name="cAPIURLName">API URLName</param>
        /// <returns></returns>
        public bool checkIsCanReadContractReport(string cContractID, string cTeamID, string cAPIURLName)
        {
            bool reValue = false;

            getLoginAccount();
            getEmployeeInfo();

            #region call ONE SERVICE 查詢是否可以讀取合約書PDF權限接口
            VIEWCONTRACTSMEMBERSINFO_INPUT beanIN = new VIEWCONTRACTSMEMBERSINFO_INPUT();

            beanIN.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
            beanIN.IV_LOGINEMPNAME = ViewBag.empEngName;
            beanIN.IV_CONTRACTID = cContractID;
            beanIN.IV_SRTEAM = cTeamID;
            beanIN.IV_APIURLName = cAPIURLName;

            VIEWCONTRACTSMEMBERSINFO_OUTPUT beanOUT = CMF.GetAPI_VIEWCONTRACTSMEMBERSINFO(beanIN);

            if (beanOUT.EV_MSGT == "Y")
            {
                if (pIsMIS || pIsCSManager)
                {
                    reValue = true;
                }
                else
                {
                    if (beanOUT.EV_IsCanRead == "Y")
                    {
                        reValue = true;
                    }
                }
            }
            #endregion

            return reValue;
        }
        #endregion

        #region 儲存合約主數據內容
        [HttpPost]
        public IActionResult saveContractMain(IFormCollection formCollection)
        {
            getLoginAccount();
            getEmployeeInfo();

            bool tIsFormal = false;
            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;
            string tLog = string.Empty;            
            string OldCTeamId = string.Empty;
            string OldCMACycle = string.Empty;
            string OldCMANotes = string.Empty;
            string OldCMAAddress = string.Empty;
            string OldCContractNotes = string.Empty;
            string OldCBillNotes = string.Empty;

            pContractID = formCollection["hid_cContractID"].FirstOrDefault();            
            string CTeamId = formCollection["hid_cTeamID"].FirstOrDefault();
            string CMACycle = formCollection["tbx_cMACycle"].FirstOrDefault();
            string CMANotes = formCollection["tbx_cMANotes"].FirstOrDefault();
            string CMAAddress = formCollection["tbx_cMAAddress"].FirstOrDefault();
            string CContractNotes = formCollection["tbx_cContractNotes"].FirstOrDefault();
            string CBillNotes = formCollection["tbx_cBillNotes"].FirstOrDefault();

            CONTRACTCHANGE_INPUT beanIN = new CONTRACTCHANGE_INPUT();

            try
            {
                #region 取得系統位址參數相關資訊
                SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

                tIsFormal = ParaBean.IsFormal;

                tBPMURLName = ParaBean.BPMURLName;
                tPSIPURLName = ParaBean.PSIPURLName;
                tAPIURLName = ParaBean.APIURLName;
                tAttachURLName = ParaBean.AttachURLName;
                #endregion

                var beanM = dbOne.TbOneContractMains.FirstOrDefault(x => x.Disabled == 0 && x.CContractId == pContractID);

                if (beanM != null)
                {
                    #region 紀錄新舊值
                    OldCTeamId = beanM.CTeamId;
                    tLog += CMF.getNewAndOldLog("服務團隊", OldCTeamId, CTeamId);

                    OldCMACycle = beanM.CMacycle;
                    tLog += CMF.getNewAndOldLog("維護週期", OldCMACycle, CMACycle);

                    OldCMANotes = beanM.CManotes;
                    tLog += CMF.getNewAndOldLog("維護備註", OldCMANotes, CMANotes);

                    OldCMAAddress = beanM.CMaaddress;
                    tLog += CMF.getNewAndOldLog("維護地址", OldCMAAddress, CMAAddress);

                    OldCContractNotes = beanM.CContractNotes;
                    tLog += CMF.getNewAndOldLog("合約備註", OldCContractNotes, CContractNotes);

                    OldCBillNotes = beanM.CBillNotes;
                    tLog += CMF.getNewAndOldLog("請款備註", OldCBillNotes, CBillNotes);
                    #endregion

                    #region 主資料表                    
                    beanM.CTeamId = CTeamId;
                    beanM.CMacycle = CMACycle;
                    beanM.CManotes = CMANotes;
                    beanM.CMaaddress = CMAAddress;
                    beanM.CContractNotes = CContractNotes;
                    beanM.CBillNotes = CBillNotes;

                    beanM.ModifiedDate = DateTime.Now;
                    beanM.ModifiedUserName = ViewBag.empEngName;
                    #endregion

                    int result = dbOne.SaveChanges();

                    if (result <= 0)
                    {
                        pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "儲存失敗" + Environment.NewLine;
                        CMF.writeToLog(pContractID, "saveContractMain", pMsg, ViewBag.empEngName);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(tLog))
                        {
                            CMF.writeToLog(pContractID, "saveContractMain", tLog, ViewBag.empEngName);

                            #region call ONE SERVICE 合約主數據資料新增/異動時發送Mail通知接口
                            beanIN.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
                            beanIN.IV_LOGINEMPNAME = ViewBag.empEngName;
                            beanIN.IV_CONTRACTID = pContractID;
                            beanIN.IV_LOG = tLog;
                            beanIN.IV_APIURLName = tAPIURLName;

                            CMF.GetAPI_CONTRACTCHANGE_SENDMAIL(beanIN);
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "失敗原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();

                CMF.writeToLog(pContractID, "saveContractMain", pMsg, ViewBag.empEngName);
            }

            return RedirectToAction("finishForm");
        }
        #endregion

        #endregion -----↑↑↑↑↑合約主數據查詢/維護 ↑↑↑↑↑-----

        #region -----↓↓↓↓↓下包合約明細查詢 ↓↓↓↓↓----- 

        #region 下包合約明細顯示
        public IActionResult ContractDetailSub()
        {
            string SubIsCanRead = string.Empty;
            string SubIsCanEdit = string.Empty;            
            string IsFromQueryContractMain = string.Empty;
            string tAPIURLName = string.Empty;

            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();
            getEmployeeInfo();

            #region Request參數            
            if (HttpContext.Request.Query["SubContractID"].FirstOrDefault() != null)
            {
                pSubContractID = HttpContext.Request.Query["SubContractID"].FirstOrDefault();               
            }
            #endregion
           
            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tAPIURLName = ParaBean.APIURLName;
            #endregion

            #region 取得合約主數據主檔
            var beanM = dbOne.TbOneContractMains.FirstOrDefault(x => x.Disabled == 0 && x.CContractId == pSubContractID);

            if (beanM != null)
            {
                #region 是否可編輯合約主數據相關內容
                pIsCanEdit = CMF.checkIsCanEditContracInfo(pOperationID_Contract, ViewBag.cLoginUser_ERPID, ViewBag.cLoginUser_EmployeeNO, ViewBag.cLoginUser_BUKRS, ViewBag.cLoginUser_CostCenterID, ViewBag.cLoginUser_DepartmentNO, pIsMIS, pIsCSManager, pSubContractID, "MAIN");

                if (pIsCanEdit)
                {
                    ViewBag.IsCanEdit = "Y";
                }
                else
                {
                    ViewBag.IsCanEdit = "N";
                }
                #endregion

                #region 是否可顯示合約書link
                pIsCanRead = checkIsCanReadContractReport(beanM.CContractId, beanM.CTeamId, tAPIURLName);

                if (pIsCanRead)
                {
                    ViewBag.IsCanRead = "Y";
                }
                else
                {
                    ViewBag.IsCanRead = "N";
                }
                #endregion
            }
            #endregion        

            var beansSub = dbOne.TbOneContractDetailSubs.FirstOrDefault(x => x.Disabled == 0 && x.CSubContractId == pSubContractID);

            if (beansSub != null)
            {
                ViewBag.cID = beansSub.CId.ToString();
                ViewBag.cContractID = beansSub.CContractId;
                ViewBag.cSubContractID = beansSub.CSubContractId;
                ViewBag.cSubSupplierID = beansSub.CSubSupplierId;
                ViewBag.cSubSupplierName = beansSub.CSubSupplierName;
                ViewBag.cSubNotes = beansSub.CSubNotes;

                #region 取得下包約合約書link
                if (pIsCanRead)
                {
                    ViewBag.cSubContractReport = beanM.CContractReport;
                }
                #endregion
            }

            return View();
        }
        #endregion

        #region 儲存下包合約明細內容
        /// <summary>
        /// 儲存下包合約明細內容
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <param name="cSubNotes">下包備註</param>
        /// <returns></returns>
        public IActionResult saveDetailSub(string cID, string cSubNotes)
        {
            string reValue = "SUCCESS";
            string tLog = string.Empty;
            string tSubContractID = string.Empty;
            string CSubNotes = cSubNotes.Trim();
            string OldCSubNotes = string.Empty;

            getLoginAccount();
            getEmployeeInfo();

            var beanSub = dbOne.TbOneContractDetailSubs.FirstOrDefault(x => x.CId == int.Parse(cID));

            if (beanSub != null)
            {
                tSubContractID = beanSub.CSubContractId;

                #region 紀錄新舊值
                OldCSubNotes = beanSub.CSubNotes;
                tLog += CMF.getNewAndOldLog("狀態", OldCSubNotes, CSubNotes);
                #endregion

                #region 更新明細的下包備註
                beanSub.CSubNotes = CSubNotes;

                beanSub.ModifiedDate = DateTime.Now;
                beanSub.ModifiedUserName = ViewBag.empEngName;
                #endregion

                #region 更新主檔的維護備註
                var beanM = dbOne.TbOneContractMains.FirstOrDefault(x => x.CContractId == tSubContractID);

                if (beanM != null)
                {
                    beanM.CManotes = CSubNotes;

                    beanM.ModifiedDate = DateTime.Now;
                    beanM.ModifiedUserName = ViewBag.empEngName;
                }
                #endregion
            }

            int result = dbOne.SaveChanges();

            if (result <= 0)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "儲存失敗" + Environment.NewLine;
                CMF.writeToLog(tSubContractID, "saveDetailSub", pMsg, ViewBag.empEngName);
                reValue = pMsg;
            }
            else
            {
                if (!string.IsNullOrEmpty(tLog))
                {
                    CMF.writeToLog(tSubContractID, "saveDetailSub", tLog, ViewBag.empEngName);
                }                
            }

            return Json(reValue);
        }
        #endregion

        #endregion -----↑↑↑↑↑下包合約明細查詢 ↑↑↑↑↑-----

        #region -----↓↓↓↓↓工程師明細查詢/維謢 ↓↓↓↓↓----- 

        #region 工程師明細查詢
        public IActionResult QueryContractDetailEng()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();
            getEmployeeInfo();

            ViewBag.cContractID = "";
            ViewBag.cMainTeamID = "";
            ViewBag.cMainIsOldContractID = "N";

            #region Request參數            
            if (HttpContext.Request.Query["ContractID"].FirstOrDefault() != null)
            {
                pContractID = HttpContext.Request.Query["ContractID"].FirstOrDefault();
                ViewBag.cContractID = pContractID;
            }
            #endregion            

            if (pContractID != "")
            {
                callQueryContractDetailEng(pContractID, "", "");               
            }

            return View();
        }
        #endregion

        #region 工程師明細查詢結果
        /// <summary>
        /// 工程師明細查詢結果
        /// </summary>
        /// <param name="cContractID">文件編號</param>
        /// <param name="cEngineerID">工程師ERPID</param>
        /// <param name="cIsMainEngineer">主要工程師(Y、N)</param>
        /// <returns></returns>
        public IActionResult QueryContractDetailEngResult(string cContractID, string cEngineerID, string cIsMainEngineer)
        {
            getLoginAccount();
            getEmployeeInfo();
            callQueryContractDetailEng(cContractID, cEngineerID, cIsMainEngineer);

            return View();
        }
        #endregion        

        #region 工程師明細查詢共用方法
        public void callQueryContractDetailEng(string cContractID, string cEngineerID, string cIsMainEngineer)
        {
            string IsCanEdit = string.Empty;
            string cMainCustomerID = string.Empty;          //客戶ID
            string cMainTeamID = string.Empty;              //服務團隊ID
            string cMainIsOldContractID = string.Empty;     //是否為舊文件編號
            string tUrl = string.Empty;

            List<string[]> QueryToList = new List<string[]>();  //查詢出來的清單
            List<string> tContractIDList = new List<string>();   //文件編號清單
            List<TbOneContractMain> tMainList = new List<TbOneContractMain>();

            var beans = dbOne.TbOneContractDetailEngs.OrderBy(x => x.CContractId).Where(x => x.Disabled == 0 &&
                                                         (string.IsNullOrEmpty(cContractID) ? true : x.CContractId.Contains(cContractID.Trim())) &&
                                                         (string.IsNullOrEmpty(cEngineerID) ? true : x.CEngineerId == cEngineerID.Trim()) &&
                                                         (string.IsNullOrEmpty(cIsMainEngineer) ? true : x.CIsMainEngineer == cIsMainEngineer));

            #region 取得查詢所有出來的文件編號之合約主檔
            foreach (var bean in beans)
            {
                if (!tContractIDList.Contains(bean.CContractId))
                {
                    tContractIDList.Add(bean.CContractId);
                }
            }

            tMainList = dbOne.TbOneContractMains.Where(x => x.Disabled == 0  && tContractIDList.Contains(x.CContractId)).ToList();
            #endregion

            if (pContractID != "") //若有文件編號只要查一次(從主約過來)
            {
                string[] AryInfo = findExtraContractMainInfo(tMainList, pContractID);

                ViewBag.cMainCustomerID = AryInfo[0];        
                ViewBag.cMainTeamID = AryInfo[1];           
                ViewBag.cMainIsOldContractID = AryInfo[2];

                cMainCustomerID = AryInfo[0];
                cMainTeamID = AryInfo[1];
                cMainIsOldContractID = AryInfo[2];

                #region 是否可編輯合約主數據相關內容
                pIsCanEdit = CMF.checkIsCanEditContracInfo(pOperationID_Contract, ViewBag.cLoginUser_ERPID, ViewBag.cLoginUser_EmployeeNO, ViewBag.cLoginUser_BUKRS, ViewBag.cLoginUser_CostCenterID, ViewBag.cLoginUser_DepartmentNO, pIsMIS, pIsCSManager, pContractID, "ENG", tMainList);
                
                if (pIsCanEdit)
                {
                    ViewBag.IsCanEdit = "Y";
                }
                else
                {
                    ViewBag.IsCanEdit = "N";
                }
                #endregion
            }

            foreach (var bean in beans)
            {
                if (pContractID == "") //非從主約過來                
                {
                    pIsCanEdit = CMF.checkIsCanEditContracInfo(pOperationID_Contract, ViewBag.cLoginUser_ERPID, ViewBag.cLoginUser_EmployeeNO, ViewBag.cLoginUser_BUKRS, ViewBag.cLoginUser_CostCenterID, ViewBag.cLoginUser_DepartmentNO, pIsMIS, pIsCSManager, bean.CContractId, "ENG", tMainList);                                      

                    string[] AryInfo = findExtraContractMainInfo(tMainList, bean.CContractId);

                    cMainCustomerID = AryInfo[0];
                    cMainTeamID = AryInfo[1];
                    cMainIsOldContractID = AryInfo[2];
                }

                IsCanEdit = pIsCanEdit ? "Y" : "N";

                tUrl = "../Contract/ContractMain?ContractID=" + bean.CContractId;

                string[] QueryInfo = new string[12];

                QueryInfo[0] = IsCanEdit;                          //是否可以編輯
                QueryInfo[1] = bean.CId.ToString();                 //系統ID
                QueryInfo[2] = cMainCustomerID;                    //客戶ID
                QueryInfo[3] = cMainTeamID;                        //服務團隊ID
                QueryInfo[4] = cMainIsOldContractID;               //是否為舊文件編號
                QueryInfo[5] = bean.CContractId;                    //文件編號
                QueryInfo[6] = bean.CEngineerId;                    //工程師ERPID                
                QueryInfo[7] = bean.CEngineerName;                  //工程師姓名
                QueryInfo[8] = bean.CIsMainEngineer;                //是否為主要工程師                
                QueryInfo[9] = bean.CContactStoreId.ToString();     //門市代號                
                QueryInfo[10] = bean.CContactStoreName;             //門市名稱                
                QueryInfo[11] = tUrl;                             //URL

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
        }
        #endregion

        #region 儲存工程師明細內容
        /// <summary>
        /// 儲存工程師明細內容
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <param name="cContractID">文件編號</param>
        /// <param name="cEngineerID">工程師ERPID</param>
        /// <param name="cEngineerName">工程師姓名</param>
        /// <param name="cIsMainEngineer">是否為主要工程師</param>
        /// <param name="cContactStoreID">門市代號</param>
        /// <param name="cContactStoreName">門市名稱</param>
        /// <returns></returns>
        public IActionResult saveDetailENG(string cID, string cContractID, string cEngineerID, string cEngineerName, string cIsMainEngineer, string cContactStoreID, string cContactStoreName)
        {
            string reValue = "SUCCESS";
            string tLog = string.Empty;

            string OldcEngineerID = string.Empty;
            string OldcEngineerName = string.Empty;
            string OldcIsMainEngineer = string.Empty;
            string OldcContactStoreID = string.Empty;
            string OldcContactStoreName = string.Empty;

            bool tIsDouble = false; //判斷是否有重覆

            getLoginAccount();
            getEmployeeInfo();

            if (cIsMainEngineer == "Y")
            {
                //判斷該文件編號是否已有主要工程師(true.已存在 false.未存在)
                tIsDouble = CMF.checkIsExitsEngineer(cContractID, cID, "Y", "");
            }
            else
            {
                //判斷傳入的工程師，是否已存在該文件編號裡的工程師明細內容(true.已存在 false.未存在)
                tIsDouble = CMF.checkIsExitsEngineer(cContractID, cID, "", cEngineerID);
            }

            if (!tIsDouble)
            {
                if (!string.IsNullOrEmpty(cID)) //修改
                {
                    var bean = dbOne.TbOneContractDetailEngs.FirstOrDefault(x => x.CId == int.Parse(cID));

                    if (bean != null)
                    {
                        #region 紀錄新舊值
                        OldcEngineerID = bean.CEngineerId;
                        tLog += CMF.getNewAndOldLog("工程師ERPID", OldcEngineerID, cEngineerID);

                        OldcEngineerName = bean.CEngineerName;
                        tLog += CMF.getNewAndOldLog("工程師姓名", OldcEngineerName, cEngineerName);

                        OldcIsMainEngineer = bean.CIsMainEngineer;
                        tLog += CMF.getNewAndOldLog("是否為主要工程師", OldcIsMainEngineer, cIsMainEngineer);

                        OldcContactStoreID = bean.CContactStoreId.ToString();
                        tLog += CMF.getNewAndOldLog("門市代號", OldcContactStoreID, cContactStoreID);

                        OldcContactStoreName = bean.CContactStoreName;
                        tLog += CMF.getNewAndOldLog("門市名稱", OldcContactStoreName, cContactStoreName);
                        #endregion

                        bean.CEngineerId = cEngineerID;
                        bean.CEngineerName = cEngineerName;
                        bean.CIsMainEngineer = cIsMainEngineer;

                        if (!string.IsNullOrEmpty(cContactStoreID))
                        {
                            bean.CContactStoreId = new Guid(cContactStoreID);
                        }

                        if (!string.IsNullOrEmpty(cContactStoreName))
                        {
                            bean.CContactStoreName = cContactStoreName;
                        }

                        bean.ModifiedDate = DateTime.Now;
                        bean.ModifiedUserName = ViewBag.empEngName;
                    }
                }
                else //新增
                {
                    TbOneContractDetailEng beanENG = new TbOneContractDetailEng();

                    beanENG.CContractId = cContractID;
                    beanENG.CEngineerId = cEngineerID;
                    beanENG.CEngineerName = cEngineerName;
                    beanENG.CIsMainEngineer = cIsMainEngineer;

                    if (!string.IsNullOrEmpty(cContactStoreID))
                    {
                        beanENG.CContactStoreId = new Guid(cContactStoreID);
                    }

                    if (!string.IsNullOrEmpty(cContactStoreName))
                    {
                        beanENG.CContactStoreName = cContactStoreName;
                    }

                    beanENG.Disabled = 0;
                    beanENG.CreatedDate = DateTime.Now;
                    beanENG.CreatedUserName = ViewBag.empEngName;

                    dbOne.TbOneContractDetailEngs.Add(beanENG);
                }

                int result = dbOne.SaveChanges();

                if (result <= 0)
                {
                    pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "儲存失敗" + Environment.NewLine;
                    CMF.writeToLog(cContractID, "saveDetailENG", pMsg, ViewBag.empEngName);
                    reValue = pMsg;
                }
                else
                {
                    if (!string.IsNullOrEmpty(tLog))
                    {
                        CMF.writeToLog(cContractID, "saveDetailENG", tLog, ViewBag.empEngName);
                    }
                }
            }
            else
            {
                if (cIsMainEngineer == "Y")
                {
                    reValue = "文件編號【" + cContractID + "】已經有主要工程師，請重新再確認！";
                }
                else
                {
                    reValue = "【" + cEngineerName + "】工程師已存在，請重新再確認！";
                }
            }

            return Json(reValue);
        }
        #endregion

        #region 刪除工程師明細內容
        /// <summary>
        /// 刪除保固資訊
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <returns></returns>
        public ActionResult deleteDetailENG(int cID)
        {
            int result = 0;

            getLoginAccount();
            getEmployeeInfo();

            var bean = dbOne.TbOneContractDetailEngs.FirstOrDefault(x => x.CId == cID);
            if (bean != null)
            {
                bean.Disabled = 1;
                bean.ModifiedDate = DateTime.Now;
                bean.ModifiedUserName = ViewBag.empEngName;

                result = dbOne.SaveChanges();                 
            }

            return Json(result);
        }
        #endregion

        #endregion -----↑↑↑↑↑工程師明細查詢/維謢 ↑↑↑↑↑-----

        #region -----↓↓↓↓↓合約標的明細查詢/上傳/維護 ↓↓↓↓↓----- 

        #region 匯入合約標的明細Excel
        [HttpPost]
        public IActionResult ImportContractOBJExcel(IFormCollection formCollection, IFormFile postedFile)
        {            
            string tLog = string.Empty;
            string tErrorMsg = string.Empty;
            string cMainContractID = string.Empty; //主約文件編號，用來判斷Excel裡的文件編號都要同一個

            string cContractID = string.Empty;
            string cHostName = string.Empty;
            string cSerialID = string.Empty;
            string cPID = string.Empty;
            string cBrands= string.Empty; 
            string cModel= string.Empty; 
            string cLocation= string.Empty; 
            string cAddress= string.Empty; 
            string cArea= string.Empty;                                         
            string cSLARESP= string.Empty; 
            string cSLASRV= string.Empty; 
            string cNotes= string.Empty;
            string cSubContractID = string.Empty;

            string OldcHostName = string.Empty;
            string OldcSerialID = string.Empty;
            string OldcPID = string.Empty;
            string OldcBrands = string.Empty;
            string OldcModel = string.Empty;
            string OldcLocation = string.Empty;
            string OldcAddress = string.Empty;
            string OldcArea = string.Empty;
            string OldcSLARESP = string.Empty;
            string OldcSLASRV = string.Empty;
            string OldcNotes = string.Empty;
            string OldcSubContractID = string.Empty;

            bool tIsNew = true;               //判斷是否為新建

            Dictionary<string, DataTable> Dic = new Dictionary<string, DataTable>();            
            DataTable dt = new DataTable();

            getLoginAccount();
            getEmployeeInfo();

            try
            {
                cMainContractID = formCollection["hid_cContractID"].FirstOrDefault();

                #region 取得匯入Excel相關
                Dic = CMF.ImportExcelToDataTable(postedFile, "合約標的");

                foreach (KeyValuePair<string, DataTable> item in Dic)
                {
                    #region 回傳結果訊息
                    tErrorMsg = item.Key;
                    #endregion

                    #region 回傳的DataTable
                    dt = item.Value.Clone();

                    foreach (DataRow dr in item.Value.Rows)
                    {
                        dt.Rows.Add(dr.ItemArray);
                    }
                    #endregion

                    break;
                }
                #endregion

                #region 判斷Excel裡的文件編號是否都跟主約文件編號一樣
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[0].ToString() != cMainContractID)
                    {
                        tErrorMsg = "Excel的文件編號【" + dr[0].ToString() + "】與主約文件編號【"+ cMainContractID + "】不一致，請重新確認後再匯入！";
                        break;
                    }
                }
                #endregion

                if (tErrorMsg == "")
                {
                    #region 寫入DataTable到資料庫                    
                    foreach(DataRow dr in dt.Rows)
                    {
                        try
                        {
                            tIsNew = true;

                            cContractID = dr[0].ToString();
                            cHostName = dr[1].ToString();
                            cSerialID = dr[2].ToString();
                            cPID = dr[3].ToString();
                            cBrands = dr[4].ToString();
                            cModel = dr[5].ToString();
                            cLocation = dr[6].ToString();
                            cAddress = dr[7].ToString();
                            cArea = dr[8].ToString();
                            cSLARESP = dr[9].ToString();
                            cSLASRV = dr[10].ToString();
                            cNotes = dr[11].ToString();
                            cSubContractID = dr[12].ToString();

                            if (cSerialID.ToUpper() != "N/A")                          
                            {
                                var bean = dbOne.TbOneContractDetailObjs.FirstOrDefault(x => x.Disabled == 0 && x.CContractId == cContractID && x.CSerialId == cSerialID);

                                if (bean != null)
                                {
                                    tIsNew = false;

                                    #region 紀錄新舊值
                                    OldcHostName = bean.CHostName;
                                    tLog += CMF.getNewAndOldLog("HostName", OldcHostName, cHostName);

                                    OldcSerialID = bean.CSerialId;
                                    tLog += CMF.getNewAndOldLog("序號", OldcSerialID, cSerialID);

                                    OldcPID = bean.CPid;
                                    tLog += CMF.getNewAndOldLog("ProductID", OldcPID, cPID);

                                    OldcBrands = bean.CBrands;
                                    tLog += CMF.getNewAndOldLog("廠牌", OldcBrands, cBrands);

                                    OldcModel = bean.CModel;
                                    tLog += CMF.getNewAndOldLog("ProductModel", OldcModel, cModel);

                                    OldcLocation = bean.CLocation;
                                    tLog += CMF.getNewAndOldLog("Location", OldcLocation, cLocation);

                                    OldcAddress = bean.CAddress;
                                    tLog += CMF.getNewAndOldLog("地址", OldcAddress, cAddress);

                                    OldcArea = bean.CArea;
                                    tLog += CMF.getNewAndOldLog("區域", OldcArea, cArea);

                                    OldcSLARESP = bean.CSlaresp;
                                    tLog += CMF.getNewAndOldLog("回應條件", OldcSLARESP, cSLARESP);

                                    OldcSLASRV = bean.CSlasrv;
                                    tLog += CMF.getNewAndOldLog("服務條件", OldcSLASRV, cSLASRV);

                                    OldcNotes = bean.CNotes;
                                    tLog += CMF.getNewAndOldLog("備註", OldcNotes, cNotes);

                                    OldcSubContractID = bean.CSubContractId;
                                    tLog += CMF.getNewAndOldLog("下包文件編號", OldcSubContractID, cSubContractID);
                                    #endregion

                                    bean.CHostName = cHostName;
                                    bean.CSerialId = cSerialID;
                                    bean.CPid = cPID;
                                    bean.CBrands = cBrands;
                                    bean.CModel = cModel;
                                    bean.CLocation = cLocation;
                                    bean.CAddress = cAddress;
                                    bean.CArea = cArea;
                                    bean.CSlaresp = cSLARESP;
                                    bean.CSlasrv = cSLASRV;
                                    bean.CNotes = cNotes;
                                    bean.CSubContractId = cSubContractID;

                                    bean.ModifiedDate = DateTime.Now;
                                    bean.ModifiedUserName = ViewBag.empEngName;
                                }
                            }

                            if (tIsNew) //為新建才進來執行
                            {
                                TbOneContractDetailObj beanOBJ = new TbOneContractDetailObj();

                                beanOBJ.CContractId = cContractID;
                                beanOBJ.CHostName = cHostName;
                                beanOBJ.CSerialId = cSerialID;
                                beanOBJ.CPid = cPID;
                                beanOBJ.CBrands = cBrands;
                                beanOBJ.CModel = cModel;
                                beanOBJ.CLocation = cLocation;
                                beanOBJ.CAddress = cAddress;
                                beanOBJ.CArea = cArea;
                                beanOBJ.CSlaresp = cSLARESP;
                                beanOBJ.CSlasrv = cSLASRV;
                                beanOBJ.CNotes = cNotes;
                                beanOBJ.CSubContractId = cSubContractID;

                                beanOBJ.Disabled = 0;
                                beanOBJ.CreatedDate = DateTime.Now;
                                beanOBJ.CreatedUserName = ViewBag.empEngName;

                                dbOne.TbOneContractDetailObjs.Add(beanOBJ);
                            }

                            int result = dbOne.SaveChanges();

                            if (result <= 0)
                            {
                                tErrorMsg += "序號為【" + cSerialID + "】，寫入合約標的資料庫失敗！" + Environment.NewLine;
                                CMF.writeToLog(cContractID, "saveDetailOBJ", tErrorMsg, ViewBag.empEngName);                                
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(tLog))
                                {
                                    CMF.writeToLog(cContractID, "saveDetailOBJ", tLog, ViewBag.empEngName);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            tErrorMsg += "序號為【" + cSerialID + "】，寫入合約標的資料庫失敗！" + e.Message + Environment.NewLine;
                        }
                    }
                    #endregion
                }               

                if (tErrorMsg == "")
                {
                    ViewBag.Message = "匯入成功！";
                }
                else
                {
                    ViewBag.Message = "匯入失敗！原因：" + Environment.NewLine + tErrorMsg;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "匯入失敗！原因：" + ex.Message;
            }

            return RedirectToAction("QueryContractDetailObj", new { ContractID = cMainContractID, tMessage = ViewBag.Message });
        }
        #endregion

        #region 合約標的明細查詢
        public IActionResult QueryContractDetailObj()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            bool tIsFormal = false;

            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;

            getLoginAccount();
            getEmployeeInfo();

            
            ViewBag.cContractID = "";
            ViewBag.cSubContractID = "";
            ViewBag.cMainTeamID = "";
            ViewBag.cMainIsOldContractID = "N";

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;
            
            ViewBag.DownloadURL = "http://" + tAttachURLName + "/CSreport/ZCONTRACT_OBJ.XLSX";
            #endregion

            #region Request參數            
            if (HttpContext.Request.Query["ContractID"].FirstOrDefault() != null)
            {
                pContractID = HttpContext.Request.Query["ContractID"].FirstOrDefault();                
                ViewBag.cContractID = pContractID;
            }

            if (HttpContext.Request.Query["SubContractID"].FirstOrDefault() != null)
            {
                pSubContractID = HttpContext.Request.Query["SubContractID"].FirstOrDefault();
                pContractID = pSubContractID; //將下包約指定給主約
                ViewBag.cSubContractID = pSubContractID;
            }

            if (HttpContext.Request.Query["tMessage"].FirstOrDefault() != null)
            {
                //記錄匯入Excel成功或失敗訊息！
                ViewBag.Message = HttpContext.Request.Query["tMessage"].FirstOrDefault();                
            }
            #endregion            

            if (pContractID != "")
            {
                callQueryContractDetailObj(pContractID, "", "", "", "", "", "");
            }

            #region 回應條件
            List<SelectListItem> SLARESPList = CMF.findSysParameterListItem(pOperationID_Contract, "OTHER", ViewBag.cLoginUser_BUKRS, "SLARESP", true);
            ViewBag.SLARESPList = SLARESPList;
            #endregion

            #region 服務條件
            List<SelectListItem> SLASRVList = CMF.findSysParameterListItem(pOperationID_Contract, "OTHER", ViewBag.cLoginUser_BUKRS, "SLASRV", true);
            ViewBag.SLASRVList = SLASRVList;
            #endregion

            return View();
        }
        #endregion

        #region 合約標的明細查詢結果
        /// <summary>
        /// 合約標的明細查詢結果
        /// </summary>
        /// <param name="cContractID">文件編號</param>
        /// <param name="cHostName">HostName</param>
        /// <param name="cSerialID">序號</param>
        /// <param name="cModel">Product Model</param>        
        /// <param name="cArea">區域</param>
        /// <param name="cStartDate">合約到期日(起)</param>
        /// <param name="cEndDate">合約到期日(迄)</param>
        /// <returns></returns>
        public IActionResult QueryContractDetailObjResult(string cContractID, string cHostName, string cSerialID, string cModel, string cArea, string cStartDate, string cEndDate)
        {
            getLoginAccount();
            getEmployeeInfo();
            callQueryContractDetailObj(cContractID, cHostName, cSerialID, cModel, cArea, cStartDate, cEndDate);

            return View();
        }
        #endregion

        #region 合約標的明細查詢共用方法
        public void callQueryContractDetailObj(string cContractID, string cHostName, string cSerialID, string cModel, string cArea, string cStartDate, string cEndDate)
        {
            DataTable dt = null;
            StringBuilder tSQL = new StringBuilder();

            string IsCanEdit = string.Empty;
            string ttWhere = string.Empty;
            string tLefJoin = string.Empty;
            string tNotes = string.Empty;
            string tSubContractID = string.Empty;
            string tUrl = string.Empty;

            List<string[]> QueryToList = new List<string[]>();  //查詢出來的清單
            List<string> tContractIDList = new List<string>();   //文件編號清單
            List<TbOneContractMain> tMainList = new List<TbOneContractMain>();           

            #region 文件編號
            if (!string.IsNullOrEmpty(cContractID))
            {
                ttWhere += "AND O.cContractID LIKE N'%" + cContractID.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region HostName
            if (!string.IsNullOrEmpty(cHostName))
            {
                ttWhere += "AND O.cHostName LIKE N'%" + cHostName.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 序號
            if (!string.IsNullOrEmpty(cSerialID))
            {
                ttWhere += "AND O.cSerialID LIKE N'%" + cSerialID.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region Product Model
            if (!string.IsNullOrEmpty(cModel))
            {
                ttWhere += "AND O.cModel LIKE N'%" + cModel.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 區域
            if (!string.IsNullOrEmpty(cArea))
            {
                ttWhere += "AND O.cArea LIKE N'%" + cArea.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 合約到期日
            if (!string.IsNullOrEmpty(cStartDate))
            {
                ttWhere += "AND Convert(varchar(10),M.cStartDate,111) >= N'" + cStartDate.Replace("-", "/") + "' ";
                tLefJoin = " left join TB_ONE_ContractMain M on M.cContractID = O.cContractID";
            }

            if (!string.IsNullOrEmpty(cEndDate))
            {
                ttWhere += "AND Convert(varchar(10),M.cEndDate,111) <= N'" + cEndDate.Replace("-", "/") + "' ";
                tLefJoin = " left join TB_ONE_ContractMain M on M.cContractID = O.cContractID";
            }
            #endregion

            #region SQL語法
            tSQL.AppendLine(" Select O.* ");
            tSQL.AppendLine(" From TB_ONE_ContractDetail_OBJ O");
            tSQL.AppendLine(tLefJoin);
            tSQL.AppendLine(" Where 1=1 AND O.disabled = 0 " + ttWhere);
            #endregion

            dt = CMF.getDataTableByDb(tSQL.ToString(), "dbOne");

            #region 取得查詢所有出來的文件編號之合約主檔
            foreach (DataRow dr in dt.Rows)
            {
                if (!tContractIDList.Contains(dr["cContractID"].ToString()))
                {
                    tContractIDList.Add(dr["cContractID"].ToString());
                }
            }

            tMainList = dbOne.TbOneContractMains.Where(x => x.Disabled == 0 && tContractIDList.Contains(x.CContractId)).ToList();
            #endregion

            if (pContractID != "") //若有文件編號只要查一次(從主約過來)
            {
                #region 是否可編輯合約主數據相關內容
                pIsCanEdit = CMF.checkIsCanEditContracInfo(pOperationID_Contract, ViewBag.cLoginUser_ERPID, ViewBag.cLoginUser_EmployeeNO, ViewBag.cLoginUser_BUKRS, ViewBag.cLoginUser_CostCenterID, ViewBag.cLoginUser_DepartmentNO, pIsMIS, pIsCSManager, pContractID, "OBJ", tMainList);

                if (pIsCanEdit)
                {
                    ViewBag.IsCanEdit = "Y";
                }
                else
                {
                    ViewBag.IsCanEdit = "N";
                }
                #endregion
            }

            #region 組待查詢清單
            foreach (DataRow dr in dt.Rows)
            {
                if (pContractID == "") //非從主約過來                
                {
                    pIsCanEdit = CMF.checkIsCanEditContracInfo(pOperationID_Contract, ViewBag.cLoginUser_ERPID, ViewBag.cLoginUser_EmployeeNO, ViewBag.cLoginUser_BUKRS, ViewBag.cLoginUser_CostCenterID, ViewBag.cLoginUser_DepartmentNO, pIsMIS, pIsCSManager, dr["cContractID"].ToString(), "OBJ", tMainList);                   
                }

                IsCanEdit = pIsCanEdit ? "Y" : "N";

                tUrl = "../Contract/ContractMain?ContractID=" + dr["cContractID"].ToString();

                string[] QueryInfo = new string[16];

                tNotes = dr["cNotes"].ToString().Replace("\r\n", "<br/>");
                tSubContractID = dr["cSubContractID"].ToString().Replace("\r\n", "<br/>");

                QueryInfo[0] = IsCanEdit;                       //是否可以編輯
                QueryInfo[1] = dr["cID"].ToString();             //系統ID
                QueryInfo[2] = dr["cContractID"].ToString();      //文件編號
                QueryInfo[3] = dr["cHostName"].ToString();        //HostName
                QueryInfo[4] = dr["cSerialID"].ToString();        //序號
                QueryInfo[5] = dr["cPID"].ToString();            //ProductID
                QueryInfo[6] = dr["cBrands"].ToString();         //廠牌                
                QueryInfo[7] = dr["cModel"].ToString();          //ProductModel
                QueryInfo[8] = dr["cLocation"].ToString();       //Location                
                QueryInfo[9] = dr["cAddress"].ToString();        //地址                
                QueryInfo[10] = dr["cArea"].ToString();          //區域                
                QueryInfo[11] = dr["cSLARESP"].ToString();       //回應條件                
                QueryInfo[12] = dr["cSLASRV"].ToString();        //服務條件                
                QueryInfo[13] = tNotes;                        //備註                
                QueryInfo[14] = tSubContractID;                //下包文件編號                
                QueryInfo[15] = tUrl;                          //URL

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion
        }
        #endregion

        #region 儲存合約標的明細內容
        /// <summary>
        /// 儲存合約標的明細內容
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <param name="cContractID">文件編號</param>
        /// <param name="cHostName">HostName</param>
        /// <param name="cSerialID">序號</param>
        /// <param name="cPID">ProductID</param>
        /// <param name="cBrands">廠牌</param>
        /// <param name="cModel">ProductModel</param>
        /// <param name="cLocation">Location</param>
        /// <param name="cAddress">地址</param>
        /// <param name="cArea">區域</param>
        /// <param name="cSLARESP">回應條件</param>
        /// <param name="cSLASRV">服務條件</param>
        /// <param name="cNotes">備註</param>
        /// <param name="cSubContractID">下包文件編號</param>
        /// <returns></returns>
        public IActionResult saveDetailOBJ(string cID, string cContractID, string cHostName, string cSerialID, string cPID, 
                                         string cBrands, string cModel, string cLocation, string cAddress, string cArea, 
                                         string cSLARESP, string cSLASRV, string cNotes, string cSubContractID)

        {
            string reValue = "SUCCESS";
            string tLog = string.Empty;

            string OldcHostName = string.Empty;
            string OldcSerialID = string.Empty;
            string OldcPID = string.Empty;
            string OldcBrands = string.Empty;
            string OldcModel = string.Empty;
            string OldcLocation = string.Empty;
            string OldcAddress = string.Empty;
            string OldcArea = string.Empty;
            string OldcSLARESP = string.Empty;
            string OldcSLASRV = string.Empty;
            string OldcNotes = string.Empty;
            string OldcSubContractID = string.Empty;

            bool tIsDouble = false; //判斷是否有重覆

            getLoginAccount();
            getEmployeeInfo();

            //判斷傳入的序號是否已存在合約標的明細內容(true.已存在 false.未存在)
            tIsDouble = CMF.checkIsExitsContractDetailObj(cContractID, cID, cSerialID);

            if (!tIsDouble)
            {
                if (!string.IsNullOrEmpty(cID)) //修改
                {
                    var bean = dbOne.TbOneContractDetailObjs.FirstOrDefault(x => x.CId == int.Parse(cID));

                    if (bean != null)
                    {
                        #region 紀錄新舊值
                        OldcHostName = bean.CHostName;
                        tLog += CMF.getNewAndOldLog("HostName", OldcHostName, cHostName);

                        OldcSerialID = bean.CSerialId;
                        tLog += CMF.getNewAndOldLog("序號", OldcSerialID, cSerialID);

                        OldcPID = bean.CPid;
                        tLog += CMF.getNewAndOldLog("ProductID", OldcPID, cPID);

                        OldcBrands = bean.CBrands;
                        tLog += CMF.getNewAndOldLog("廠牌", OldcBrands, cBrands);

                        OldcModel = bean.CModel;
                        tLog += CMF.getNewAndOldLog("ProductModel", OldcModel, cModel);

                        OldcLocation = bean.CLocation;
                        tLog += CMF.getNewAndOldLog("Location", OldcLocation, cLocation);

                        OldcAddress = bean.CAddress;
                        tLog += CMF.getNewAndOldLog("地址", OldcAddress, cAddress);

                        OldcArea = bean.CArea;
                        tLog += CMF.getNewAndOldLog("區域", OldcArea, cArea);

                        OldcSLARESP = bean.CSlaresp;
                        tLog += CMF.getNewAndOldLog("回應條件", OldcSLARESP, cSLARESP);

                        OldcSLASRV = bean.CSlasrv;
                        tLog += CMF.getNewAndOldLog("服務條件", OldcSLASRV, cSLASRV);

                        OldcNotes = bean.CNotes;
                        tLog += CMF.getNewAndOldLog("備註", OldcNotes, cNotes);

                        OldcSubContractID = bean.CSubContractId;
                        tLog += CMF.getNewAndOldLog("下包文件編號", OldcSubContractID, cSubContractID);
                        #endregion

                        bean.CHostName = string.IsNullOrEmpty(cHostName) ? "" : cHostName;
                        bean.CSerialId = string.IsNullOrEmpty(cSerialID) ? "" : cSerialID;
                        bean.CPid = string.IsNullOrEmpty(cPID) ? "" : cPID;
                        bean.CBrands = string.IsNullOrEmpty(cBrands) ? "" : cBrands;
                        bean.CModel = string.IsNullOrEmpty(cModel) ? "" : cModel;
                        bean.CLocation = string.IsNullOrEmpty(cLocation) ? "" : cLocation;
                        bean.CAddress = string.IsNullOrEmpty(cAddress) ? "" : cAddress;
                        bean.CArea = string.IsNullOrEmpty(cArea) ? "" : cArea;
                        bean.CSlaresp = string.IsNullOrEmpty(cSLARESP) ? "" : cSLARESP;
                        bean.CSlasrv = string.IsNullOrEmpty(cSLASRV) ? "" : cSLASRV;
                        bean.CNotes = string.IsNullOrEmpty(cNotes) ? "" : cNotes;
                        bean.CSubContractId = string.IsNullOrEmpty(cSubContractID) ? "" : cSubContractID;

                        bean.ModifiedDate = DateTime.Now;
                        bean.ModifiedUserName = ViewBag.empEngName;
                    }
                }
                else //新增
                {
                    TbOneContractDetailObj beanOBJ = new TbOneContractDetailObj();

                    beanOBJ.CContractId = string.IsNullOrEmpty(cContractID) ? "" : cContractID;
                    beanOBJ.CHostName = string.IsNullOrEmpty(cHostName) ? "" : cHostName;
                    beanOBJ.CSerialId = string.IsNullOrEmpty(cSerialID) ? "" : cSerialID;
                    beanOBJ.CPid = string.IsNullOrEmpty(cPID) ? "" : cPID;
                    beanOBJ.CBrands = string.IsNullOrEmpty(cBrands) ? "" : cBrands;
                    beanOBJ.CModel = string.IsNullOrEmpty(cModel) ? "" : cModel;
                    beanOBJ.CLocation = string.IsNullOrEmpty(cLocation) ? "" : cLocation;
                    beanOBJ.CAddress = string.IsNullOrEmpty(cAddress) ? "" : cAddress;
                    beanOBJ.CArea = string.IsNullOrEmpty(cArea) ? "" : cArea;
                    beanOBJ.CSlaresp = string.IsNullOrEmpty(cSLARESP) ? "" : cSLARESP;
                    beanOBJ.CSlasrv = string.IsNullOrEmpty(cSLASRV) ? "" : cSLASRV;
                    beanOBJ.CNotes = string.IsNullOrEmpty(cNotes) ? "" : cNotes;
                    beanOBJ.CSubContractId = string.IsNullOrEmpty(cSubContractID) ? "" : cSubContractID;

                    beanOBJ.Disabled = 0;
                    beanOBJ.CreatedDate = DateTime.Now;
                    beanOBJ.CreatedUserName = ViewBag.empEngName;

                    dbOne.TbOneContractDetailObjs.Add(beanOBJ);
                }

                int result = dbOne.SaveChanges();

                if (result <= 0)
                {
                    pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "儲存失敗" + Environment.NewLine;
                    CMF.writeToLog(cContractID, "saveDetailOBJ", pMsg, ViewBag.empEngName);
                    reValue = pMsg;
                }
                else
                {
                    if (!string.IsNullOrEmpty(tLog))
                    {
                        CMF.writeToLog(cContractID, "saveDetailOBJ", tLog, ViewBag.empEngName);
                    }
                }
            }
            else
            {
                reValue = "序號【" + cSerialID + "】已存在，請重新再確認！";
            }

            return Json(reValue);
        }
        #endregion

        #region 刪除合約標的明細內容
        /// <summary>
        /// 刪除合約標的明細內容
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <returns></returns>
        public ActionResult deleteDetailOBJ(int cID)
        {
            int result = 0;

            getLoginAccount();
            getEmployeeInfo();

            var bean = dbOne.TbOneContractDetailObjs.FirstOrDefault(x => x.CId == cID);
            if (bean != null)
            {
                bean.Disabled = 1;
                bean.ModifiedDate = DateTime.Now;
                bean.ModifiedUserName = ViewBag.empEngName;

                result = dbOne.SaveChanges();
            }

            return Json(result);
        }
        #endregion

        #endregion -----↑↑↑↑↑合約標的明細查詢/上傳/維護 ↑↑↑↑↑-----

        #region -----↓↓↓↓↓共用方法 ↓↓↓↓↓-----

        #region 取得服務團隊ID和是否為舊文件編號
        /// <summary>
        /// 取得服務團隊ID和是否為舊文件編號
        /// </summary>
        /// <param name="tMainList">合約主檔清單</param>
        /// <param name="tContractID">文件編號</param>
        /// <returns></returns>
        public string[] findExtraContractMainInfo(List<TbOneContractMain> tMainList, string tContractID)
        {
            string[] AryValue = new string[3];
            AryValue[0] = "";   //客戶ID
            AryValue[1] = "";   //服務團隊ID
            AryValue[2] = "";   //是否為舊文件編號

            if (tMainList.Count > 0)
            {
                var beanM = tMainList.FirstOrDefault(x => x.CContractId == tContractID);

                if (beanM != null)
                {
                    #region 若無客戶ID，代表是供應商，則取供應商ID
                    if (beanM.CCustomerId == "")
                    {
                        AryValue[0] = CMF.findSubSupplierID(tContractID);
                    }
                    else
                    {
                        AryValue[0] = beanM.CCustomerId;
                    }
                    #endregion

                    AryValue[1] = beanM.CTeamId;

                    bool tIsOldContractID = CMF.checkIsOldContractID(pOperationID_Contract, tContractID); //判斷是否為舊文件編號(true.舊組織 false.新組織)
                    AryValue[2] = tIsOldContractID ? "Y" : "N";
                }
            }

            return AryValue;
        }
        #endregion

        #region Ajax用中文或英文姓名查詢人員(by服務團隊-含新舊)
        /// <summary>
        /// Ajax用中文或英文姓名查詢人員(by服務團隊-含新舊)
        /// </summary>
        /// <param name="cBUKRS">公司別(T012、T016、C069、T022)</param>         
        /// <param name="cTeamID">服務團隊ID</param>        
        /// <param name="cIsOldContractID">是否為舊文件編號(Y.是 N.否)</param>
        /// <param name="keyword">中文/英文姓名</param>        
        /// <returns></returns>
        public IActionResult AjaxfindContractTeamEmployeeByKeyword(string cBUKRS, string cTeamID, string cIsOldContractID, string keyword)
        {
            object contentObj = null;

            List<string> tList = new List<string>();

            if (cIsOldContractID == "Y")
            {
                #region 先取得所有舊CRM合約組織人員AD帳號
                var beans = psipDb.TbOneRoleParameters.Where(x => x.Disabled == 0 && x.COperationId.ToString() == pOperationID_Contract &&
                                                              x.CFunctionId == "PERSON" && x.CCompanyId == cBUKRS && x.CNo == "OLDORG");

                foreach (var bean in beans)
                {
                    if (!tList.Contains(bean.CValue))
                    {
                        tList.Add(bean.CValue);
                    }
                }
                #endregion

                contentObj = dbEIP.People.Where(x => tList.Contains(x.Account) &&
                                                   (x.LeaveReason == null && x.LeaveDate == null) &&
                                                   (x.Account.Contains(keyword) || x.Name2.Contains(keyword))).Take(5);
            }
            else
            {
                tList = CMF.findALLDeptIDListbyTeamID(cTeamID);

                contentObj = dbEIP.People.Where(x => tList.Contains(x.DeptId) &&
                                                   (x.Account.Contains(keyword) || x.Name2.Contains(keyword)) &&
                                                   (x.LeaveReason == null && x.LeaveDate == null)).Take(5);
            }

            string json = JsonConvert.SerializeObject(contentObj);
            return Content(json, "application/json");
        }
        #endregion

        #region Ajax用門市名稱查詢門市代號相關資訊
        /// <summary>
        /// Ajax用門市名稱查詢門市代號相關資訊
        /// </summary>
        /// <param name="cBUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="cCustomerID">客戶ID</param>
        /// <param name="keyword">門市名稱</param>
        /// <returns></returns>
        public IActionResult findContactStoreInfoByKeyword(string cBUKRS, string cCustomerID, string keyword)
        {
            object contentObj = null;

            contentObj = dbProxy.CustomerContactStores.Where(x => x.Disabled == 0 && x.ContactStoreName.Contains(keyword));

            string json = JsonConvert.SerializeObject(contentObj);
            return Content(json, "application/json");
        }
        #endregion

        #region Ajax判斷維護週期格式是否正確
        /// <summary>
        /// Ajax判斷維護週期格式是否正確
        /// </summary>
        /// <param name="cMACycle">維護週期</param>
        /// <returns></returns>
        public IActionResult AjaxcheckCycle(string cMACycle)
        {
            string reValue = string.Empty;
            string tTempCycle = string.Empty;

            if (cMACycle.Trim().ToUpper() != "NA")
            {
                string[] AryCycle = cMACycle.Trim().Split('_');

                foreach (string tCycle in AryCycle)
                {
                    try
                    {
                        if (tCycle.Length == 6)
                        {
                            tTempCycle = tCycle.Substring(0, 4) + "-" + tCycle.Substring(4, 2);
                            DateTime dt = Convert.ToDateTime(tTempCycle);
                        }
                        else
                        {
                            reValue += "【" + tCycle + "】維護週期格式不正確！\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        reValue += "【" + tCycle + "】維護週期格式不正確！\n";
                    }
                }               
            }

            return Json(reValue);
        }
        #endregion

        #region 提交表單後開啟該完成表單，並顯示即將關閉後再關閉此頁
        /// <summary>
        /// 提交表單後開啟該完成表單，並顯示即將關閉後再關閉此頁
        /// </summary>
        /// <returns></returns>
        public IActionResult finishForm()
        {
            return View();
        }
        #endregion

        #region 查詢更改歷史記錄
        /// <summary>
        /// 查詢更改歷史記錄
        /// </summary>
        /// <param name="cContractID">文件編號</param>
        /// <param name="cIsSubContract">是否為下包合約(Y.是 N.否)</param>
        /// <returns></returns>
        public IActionResult GetHistoryLog(string cContractID, string cIsSubContract)
        {
            OneLogInfo beanOne = new OneLogInfo();

            string EventName = string.Empty;

            if (cIsSubContract == "Y")
            {
                EventName = "saveDetailSub";
            }
            else
            {
                EventName = "saveContractMain";
            }            

            //歷史記錄資訊(共用)
            var SROneLog = dbOne.TbOneLogs.OrderByDescending(x => x.CreatedDate).Where(x => x.CSrid == cContractID && x.EventName == EventName).ToList();

            #region 新增
            beanOne.SROneLog = SROneLog;
            #endregion

            return Json(beanOne);
        }
        #endregion

        #endregion -----↑↑↑↑↑共用方法 ↑↑↑↑↑-----   
    }
}
