using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneService.Models;
using OneService.Utils;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Policy;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Diagnostics.Contracts;
using NuGet.Packaging.Core;
using System.Runtime.ConstrainedExecution;
using System.Net;

namespace OneService.Controllers
{
    public class ServiceRequestController : Controller
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
        /// 程式作業編號檔系統ID(一般服務)
        /// </summary>
        static string pOperationID_GenerallySR = "869FC989-1049-4266-ABDE-69A9B07BCD0A";

        /// <summary>
        /// 程式作業編號檔系統ID(裝機服務)
        /// </summary>
        static string pOperationID_InstallSR = "3B6FF77B-DAF4-4C2D-957A-6C28CE054D75";

        /// <summary>
        /// 程式作業編號檔系統ID(定維服務)
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

        #region -----↓↓↓↓↓服務總表 ↓↓↓↓↓-----
        /// <summary>
        /// 個人客戶設定作業
        /// </summary>
        /// <returns></returns>
        public IActionResult SRReport()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_CompCode = EmpBean.CompanyCode;
            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.cLoginUser_BUKRS = EmpBean.BUKRS;

            pCompanyCode = EmpBean.BUKRS;
            #endregion

            #region 服務案件類型
            var SRTypeList = new List<SelectListItem>()
            {
                new SelectListItem {Text="ZSR1_一般服務", Value="ZSR1" },
                new SelectListItem {Text="ZSR3_裝機服務", Value="ZSR3" },
                new SelectListItem {Text="ZSR5_定維服務", Value="ZSR5" },                
            };

            ViewBag.ddl_SRType = SRTypeList;
            #endregion

            #region 服務團隊
            var SRTeamIDList = CMF.findSRTeamIDList(pCompanyCode, false);
            ViewBag.ddl_TeamID = SRTeamIDList;
            #endregion

            #region 狀態
            List<SelectListItem> ListStatus = CMF.findSRStatus(pOperationID_GenerallySR, pOperationID_InstallSR, pOperationID_MaintainSR, pCompanyCode);
            ViewBag.ddl_Status = ListStatus;
            #endregion

            #region 報修類別
            //大類
            var SRTypeOneList = CMF.findFirstKINDList();

            //中類
            var SRTypeSecList = new List<SelectListItem>();
            SRTypeSecList.Add(new SelectListItem { Text = " ", Value = "" });

            //小類
            var SRTypeThrList = new List<SelectListItem>();
            SRTypeThrList.Add(new SelectListItem { Text = " ", Value = "" });

            ViewBag.SRTypeOneList = SRTypeOneList;
            ViewBag.SRTypeSecList = SRTypeSecList;
            ViewBag.SRTypeThrList = SRTypeThrList;
            #endregion

            return View();
        }

        #region 服務總表查詢結果
        /// <summary>
        /// 個人客戶設定作業查詢結果
        /// </summary>   
        /// <param name="StartCreatedDate">建立日期(起)</param>
        /// <param name="EndCreatedDate">建立日期(迄)</param>
        /// <param name="StartFinishTime">完成時間(起)</param>
        /// <param name="EndFinishTime">完成時間(迄)</param>
        /// <param name="SRID">SRID</param>
        /// <param name="CustomerID">客戶代號</param>
        /// <param name="RepairName">報修人</param>
        /// <param name="RepairAddress">報修人地址</param>        
        /// <param name="SerialID">序號</param>
        /// <param name="PID">機器型號</param>
        /// <param name="TeamID">服務團隊</param>
        /// <param name="Status">狀態</param>
        /// <param name="SRType">服務類型</param>
        /// <param name="EngineerID">工程師ERPID</param>
        /// <param name="ContractID">合約編號</param>
        /// <param name="SO">銷售單號</param>
        /// <param name="XCHP">XCHP</param>
        /// <param name="HPCT">HPCT</param>
        /// <param name="MaterialID">更換零件料號ID</param>
        /// <param name="MaterialName">更換零件料號說明</param>        
        /// <param name="OLDCT">OLDCT</param>
        /// <param name="NEWCT">NEWCT</param>
        /// <param name="cSRTypeOne">報修類別-大類</param>
        /// <param name="cSRTypeSec">報修類別-中類</param>
        /// <param name="cSRTypeThr">報修類別-小類</param>
        /// <returns></returns>
        public IActionResult SRReportResult(string StartCreatedDate, string EndCreatedDate, string StartFinishTime, string EndFinishTime, string SRID, 
                                          string CustomerID, string RepairName, string RepairAddress, string SerialID, string PID, string TeamID, 
                                          string Status, string SRType, string EngineerID, string ContractID, string SO, string XCHP, string HPCT, 
                                          string MaterialID, string MaterialName, string OLDCT, string NEWCT, string cSRTypeOne, string cSRTypeSec, string cSRTypeThr)
        {
            StringBuilder tSQL = new StringBuilder();

            bool tIsFormal = false;
            string ttWhere = string.Empty;
            string ttStrItem = string.Empty;
            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;
            string CreatedDate = string.Empty;
            string tSRIDUrl = string.Empty;
            string tSRTeam = string.Empty;
            string cNotes = string.Empty;
            string cReceiveTime = string.Empty;
            string cStartTime = string.Empty;
            string cArriveTime = string.Empty;
            string cFinishTime = string.Empty;
            string cWorkHours = string.Empty;
            string cSRReportURL = string.Empty;
            string cUsed = string.Empty;
            string cWTYSDATE = string.Empty;
            string cWTYEDATE = string.Empty;

            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_CompCode = EmpBean.CompanyCode;
            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.cLoginUser_BUKRS = EmpBean.BUKRS;

            pCompanyCode = EmpBean.BUKRS;
            #endregion       

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;
            #endregion

            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            #region 建立日期
            if (!string.IsNullOrEmpty(StartCreatedDate))
            {
                ttWhere += "AND CreatedDate >= N'" + StartCreatedDate.Replace("/", "-") + "' ";
            }

            if (!string.IsNullOrEmpty(EndCreatedDate))
            {
                ttWhere += "AND CreatedDate <= N'" + EndCreatedDate.Replace("/", "-") + "' ";
            }
            #endregion

            #region 完成時間
            if (!string.IsNullOrEmpty(StartFinishTime))
            {
                ttWhere += "AND cFinishTime >= N'" + StartFinishTime.Replace("/", "-") + "' ";
            }

            if (!string.IsNullOrEmpty(EndFinishTime))
            {
                ttWhere += "AND cFinishTime <= N'" + EndFinishTime.Replace("/", "-") + "' ";
            }
            #endregion

            #region SRID
            if (!string.IsNullOrEmpty(SRID))
            {
                ttWhere += "AND cSRID LIKE N'%" + SRID + "%' " + Environment.NewLine;
            }
            #endregion

            #region 客戶代號
            if (!string.IsNullOrEmpty(CustomerID))
            {
                ttWhere += "AND cCustomerID = '" + CustomerID + "' " + Environment.NewLine;
            }
            #endregion

            #region 報修人姓名
            if (!string.IsNullOrEmpty(RepairName))
            {
                ttWhere += "AND cRepairName LIKE N'%" + RepairName.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 報修人地址
            if (!string.IsNullOrEmpty(RepairAddress))
            {
                ttWhere += "AND cRepairAddress LIKE N'%" + RepairAddress + "%' " + Environment.NewLine;
            }
            #endregion

            #region 序號
            if (!string.IsNullOrEmpty(SerialID))
            {
                ttWhere += "AND cSerialID LIKE N'%" + SerialID + "%' " + Environment.NewLine;
            }
            #endregion

            #region 機器型號
            if (!string.IsNullOrEmpty(PID))
            {
                ttWhere += "AND PID LIKE N'%" + PID.Trim() + "%' " + Environment.NewLine;
            }
            #endregion           

            #region 服務團隊
            if (!string.IsNullOrEmpty(TeamID))
            {
                ttStrItem = "";
                string[] tAryTeam = TeamID.TrimEnd(',').Split(',');

                if (tAryTeam.Length >= 0)
                {
                    ttStrItem = "AND (";
                }

                foreach (string tTeam in tAryTeam)
                {
                    ttStrItem += " cTeamID like N'%" + tTeam + "%' or";
                }

                if (tAryTeam.Length >= 0)
                {
                    if (ttStrItem.EndsWith("or"))
                    {
                        ttStrItem = ttStrItem.Substring(0, ttStrItem.Length - 2); //去除最後一個or  
                    }

                    ttStrItem += ") ";
                }

                ttWhere += ttStrItem + Environment.NewLine;
            }
            #endregion

            #region 狀態
            if (!string.IsNullOrEmpty(Status))
            {
                ttStrItem = "";
                string[] tAryStatus = Status.TrimEnd(',').Split(',');

                foreach (string tStatus in tAryStatus)
                {
                    ttStrItem += "N'" + tStatus + "',";
                }

                ttStrItem = ttStrItem.TrimEnd(',');

                if (ttStrItem != "")
                {
                    ttWhere += "AND cStatus IN (" + ttStrItem + ") " + Environment.NewLine;
                }
            }
            #endregion

            #region 服務類型
            if (!string.IsNullOrEmpty(SRType))
            {
                ttStrItem = "";
                string[] tArySRType = SRType.TrimEnd(',').Split(',');

                foreach (string tSRType in tArySRType)
                {
                    ttStrItem += "N'" + tSRType + "',";
                }

                ttStrItem = ttStrItem.TrimEnd(',');

                if (ttStrItem != "")
                {
                    ttWhere += "AND cSRType IN (" + ttStrItem + ") " + Environment.NewLine;
                }
            }
            #endregion

            #region 工程師ERPID
            if (!string.IsNullOrEmpty(EngineerID))
            {
                ttWhere += "AND cEngineerID LIKE N'%" + EngineerID.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 合約編號
            if (!string.IsNullOrEmpty(ContractID))
            {
                ttWhere += "AND cContractID LIKE N'%" + ContractID + "%' " + Environment.NewLine;
            }
            #endregion

            #region 銷售單號
            if (!string.IsNullOrEmpty(SO))
            {
                ttWhere += "AND SO LIKE N'%" + SO.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region XCHP
            if (!string.IsNullOrEmpty(XCHP))
            {
                ttWhere += "AND cXCHP LIKE N'%" + XCHP.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region XCHP
            if (!string.IsNullOrEmpty(HPCT))
            {
                ttWhere += "AND cHPCT LIKE N'%" + HPCT.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 更換零件料號ID
            if (!string.IsNullOrEmpty(MaterialID))
            {
                ttWhere += "AND cMaterialID LIKE N'%" + MaterialID.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 更換零件料號說明
            if (!string.IsNullOrEmpty(MaterialName))
            {
                ttWhere += "AND cMaterialName LIKE N'%" + MaterialName.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region OLDCT
            if (!string.IsNullOrEmpty(OLDCT))
            {
                ttWhere += "AND cOldCT LIKE N'%" + OLDCT.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region NEWCT
            if (!string.IsNullOrEmpty(NEWCT))
            {
                ttWhere += "AND cNewCT LIKE N'%" + NEWCT.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 報修類別-大類
            if (!string.IsNullOrEmpty(cSRTypeOne))
            {
                ttWhere += "AND cSRTypeOne = '" + cSRTypeOne + "' ";
            }
            #endregion

            #region 報修類別-中類
            if (!string.IsNullOrEmpty(cSRTypeSec))
            {
                ttWhere += "AND cSRTypeSec = '" + cSRTypeSec + "' ";
            }
            #endregion

            #region 報修類別-小類
            if (!string.IsNullOrEmpty(cSRTypeThr))
            {
                ttWhere += "AND cSRTypeThr = '" + cSRTypeThr + "' ";
            }
            #endregion

            #region 組待查詢清單

            #region SQL語法
            tSQL.AppendLine(" Select *");            
            tSQL.AppendLine(" From VIEW_ONE_SRREPORT ");            
            tSQL.AppendLine(" Where 1=1 " + ttWhere);
            #endregion

            DataTable dt = CMF.getDataTableByDb(tSQL.ToString(), "dbOne");

            var tSRTeam_List = CMF.findSRTeamIDList(pCompanyCode, false);

            foreach (DataRow dr in dt.Rows)
            {
                string[] QueryInfo = new string[58];

                CreatedDate = string.IsNullOrEmpty(dr["CreatedDate"].ToString()) ? "" : Convert.ToDateTime(dr["CreatedDate"].ToString()).ToString("yyyy-MM-dd");
                tSRIDUrl = CMF.findSRIDUrl(dr["cSRID"].ToString());
                tSRTeam = CMF.TransSRTeam(tSRTeam_List, dr["cTeamID"].ToString());
                cNotes = dr["cNotes"].ToString().Replace("\r\n", "<br/>");
                cReceiveTime = Convert.ToDateTime(dr["cReceiveTime"].ToString()) == Convert.ToDateTime("1900-01-01") ? "" : Convert.ToDateTime(dr["cReceiveTime"].ToString()).ToString("yyyy-MM-dd HH:mm");
                cStartTime = Convert.ToDateTime(dr["cStartTime"].ToString()) == Convert.ToDateTime("1900-01-01") ? "" : Convert.ToDateTime(dr["cStartTime"].ToString()).ToString("yyyy-MM-dd HH:mm");
                cArriveTime = Convert.ToDateTime(dr["cArriveTime"].ToString()) == Convert.ToDateTime("1900-01-01") ? "" : Convert.ToDateTime(dr["cArriveTime"].ToString()).ToString("yyyy-MM-dd HH:mm");
                cFinishTime = Convert.ToDateTime(dr["cFinishTime"].ToString()) == Convert.ToDateTime("1900-01-01") ? "" : Convert.ToDateTime(dr["cFinishTime"].ToString()).ToString("yyyy-MM-dd HH:mm");
                cWorkHours = dr["cWorkHours"].ToString() == "0" ? "" : dr["cWorkHours"].ToString();
                cSRReportURL = CMF.findAttachUrl(dr["cSRReport"].ToString(), tAttachURLName);
                cUsed = (dr["cWTYID"].ToString() != "" || dr["cWTYName"].ToString() != "") ? "Y" : "N";
                cWTYSDATE = Convert.ToDateTime(dr["cWTYSDATE"].ToString()) == Convert.ToDateTime("1900-01-01") ? "" : Convert.ToDateTime(dr["cWTYSDATE"].ToString()).ToString("yyyy-MM-dd");
                cWTYEDATE = Convert.ToDateTime(dr["cWTYEDATE"].ToString()) == Convert.ToDateTime("1900-01-01") ? "" : Convert.ToDateTime(dr["cWTYEDATE"].ToString()).ToString("yyyy-MM-dd");

                QueryInfo[0] = dr["cSRID"].ToString();              //SR_ID
                QueryInfo[1] = tSRIDUrl;                           //服務案件URL
                QueryInfo[2] = dr["cDesc"].ToString();              //說明
                QueryInfo[3] = cNotes;                             //詳細描述
                QueryInfo[4] = dr["cSRType"].ToString();            //類型
                QueryInfo[5] = dr["cSRTypeNote"].ToString();        //類型說明
                QueryInfo[6] = dr["cStatusNote"].ToString();        //狀態說明
                QueryInfo[7] = dr["cStatus"].ToString();            //狀態
                QueryInfo[8] = dr["cCustomerID"].ToString();        //客戶ID
                QueryInfo[9] = dr["cCustomerName"].ToString();      //客戶名稱
                QueryInfo[10] = dr["cRepairName"].ToString();       //報修人姓名
                QueryInfo[11] = dr["cRepairAddress"].ToString();    //報修人地址
                QueryInfo[12] = dr["cRepairPhone"].ToString();      //報修人電話
                QueryInfo[13] = dr["cRepairMobile"].ToString();     //報修人手機
                QueryInfo[14] = dr["cRepairEmail"].ToString();      //報修人Email
                QueryInfo[15] = CreatedDate;                      //建立日期
                QueryInfo[16] = dr["cSRProcessWay"].ToString();     //處理方式
                QueryInfo[17] = dr["cMAServiceType"].ToString();    //維護服務種類
                QueryInfo[18] = dr["cSRTypeOneNote"].ToString();    //報修大類
                QueryInfo[19] = dr["cSRTypeSecNote"].ToString();    //報修中類
                QueryInfo[20] = dr["cSRTypeThrNote"].ToString();    //報修小類
                QueryInfo[21] = tSRTeam;                          //服務團隊
                QueryInfo[22] = dr["cSQPersonName"].ToString();     //SQ人員
                QueryInfo[23] = dr["cIsSecondFix"].ToString();      //是否為二修
                QueryInfo[24] = dr["cIsAPPClose"].ToString();       //是否為APP結案
                QueryInfo[25] = dr["cDelayReason"].ToString();      //延遲結案原因
                QueryInfo[26] = dr["PID"].ToString();              //機器型號
                QueryInfo[27] = dr["PN"].ToString();               //Product Number
                QueryInfo[28] = dr["cSerialID"].ToString();         //序號
                QueryInfo[29] = cReceiveTime;                     //接單時間
                QueryInfo[30] = cStartTime;                       //出發時間
                QueryInfo[31] = cArriveTime;                      //到場時間
                QueryInfo[32] = cFinishTime;                      //完成時間
                QueryInfo[33] = cWorkHours;                       //工時(分鐘)
                QueryInfo[34] = dr["cEngineerID"].ToString();       //工程師ID
                QueryInfo[35] = dr["cEngineerName"].ToString();     //工程師姓名
                QueryInfo[36] = dr["cDesc_R"].ToString();          //處理紀錄
                QueryInfo[37] = cSRReportURL;                     //服務報告書
                QueryInfo[38] = cUsed;                            //本次使用保固
                QueryInfo[39] = dr["cWTYID"].ToString();           //保固代號
                QueryInfo[40] = dr["cWTYName"].ToString();         //保固說明
                QueryInfo[41] = cWTYSDATE;                       //保固開始
                QueryInfo[42] = cWTYEDATE;                       //保固結束
                QueryInfo[43] = dr["cSLARESP"].ToString();         //回應條件
                QueryInfo[44] = dr["cSLASRV"].ToString();          //服務條件
                QueryInfo[45] = dr["cContractID"].ToString();      //合約編號
                QueryInfo[46] = dr["cMaterialID"].ToString();      //更換零件料號ID
                QueryInfo[47] = dr["cMaterialName"].ToString();    //料號說明
                QueryInfo[48] = dr["cXCHP"].ToString();           //XC HP申請零件
                QueryInfo[49] = dr["cOldCT"].ToString();          //OLDCT
                QueryInfo[50] = dr["cNewCT"].ToString();          //NEWCT
                QueryInfo[51] = dr["cHPCT"].ToString();           //HPCT
                QueryInfo[52] = dr["cPersonalDamage"].ToString();  //是否有人損
                QueryInfo[53] = dr["cNote_PR"].ToString();        //零件更換備註
                QueryInfo[54] = dr["CountIN"].ToString();         //計數器(IN)
                QueryInfo[55] = dr["CountOUT"].ToString();        //計數器(OUT)
                QueryInfo[56] = dr["SO"].ToString();             //銷售單號
                QueryInfo[57] = dr["DN"].ToString();             //出貨單號

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion       

        #endregion -----↑↑↑↑↑服務總表 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓服務進度查詢 ↓↓↓↓↓-----
        public IActionResult QuerySRProgress()
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

            var model = new ViewModelQuerySRProgress();

            #region 服務案件種類
            var ListSRCaseType = findSRIDTypeList(false);
            ViewBag.ddl_cSRCaseType = ListSRCaseType;
            #endregion

            #region 狀態
            List<SelectListItem> ListStatus = CMF.findSRStatus(pOperationID_GenerallySR, pOperationID_InstallSR, pOperationID_MaintainSR, pCompanyCode);
            ViewBag.ddl_cStatus = ListStatus;
            #endregion

            #region 報修管道
            var ListSRPathWay = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "SRPATH", false);
            ViewBag.ddl_cSRPathWay = ListSRPathWay;
            #endregion

            #region 服務團隊
            var selectTeamList = CMF.findSRTeamIDList(pCompanyCode, false);
            ViewBag.ddl_cTeamID = selectTeamList;
            #endregion

            #region 報修類別
            //大類
            var SRTypeOneList = CMF.findFirstKINDList();

            //中類
            var SRTypeSecList = new List<SelectListItem>();
            SRTypeSecList.Add(new SelectListItem { Text = " ", Value = "" });

            //小類
            var SRTypeThrList = new List<SelectListItem>();
            SRTypeThrList.Add(new SelectListItem { Text = " ", Value = "" });

            ViewBag.SRTypeOneList = SRTypeOneList;
            ViewBag.SRTypeSecList = SRTypeSecList;
            ViewBag.SRTypeThrList = SRTypeThrList;
            #endregion

            return View(model);
        }

        #region 服務進度查詢結果
        /// <summary>
        /// 服務進度查詢結果
        /// </summary>
        /// <param name="cCompanyID">公司別(T012、T016、C069、T022)</param>
        /// <param name="cSRCaseType">服務案件種類</param>
        /// <param name="cStatus">狀態</param>
        /// <param name="cStartCreatedDate">派單起始日期</param>
        /// <param name="cEndCreatedDate">派單結束日期</param>
        /// <param name="cCustomerID">客戶代號</param>
        /// <param name="cCustomerName">客戶名稱</param>
        /// <param name="cSRID">SRID</param>
        /// <param name="CreatedUserName">派單人員</param>
        /// <param name="cRepairName">報修人姓名</param>
        /// <param name="cSRPathWay">報修管道</param>
        /// <param name="cMainEngineerID">主要工程師ERPID</param>
        /// <param name="cAssEngineerID">協助工程師ERPID</param>
        /// <param name="cTechManagerID">技術主管ERPID</param>
        /// <param name="cTeamID">服務團隊</param>
        /// <param name="cSRTypeOne">報修類別-大類</param>
        /// <param name="cSRTypeSec">報修類別-中類</param>
        /// <param name="cSRTypeThr">報修類別-小類</param>
        /// <param name="cSerialID">產品序號</param>
        /// <param name="cMaterialName">產品機器型號</param>
        /// <param name="cProductNumber">Product Number</param>
        /// <returns></returns>
        public IActionResult QuerySRProgressResult(string cCompanyID, string cSRCaseType, string cStatus, string cStartCreatedDate, string cEndCreatedDate,
                                                 string cCustomerID, string cCustomerName, string cSRID, string CreatedUserName, string cRepairName, string cSRPathWay, string cMainEngineerID, 
                                                 string cAssEngineerID, string cTechManagerID, string cTeamID, string cSRTypeOne, string cSRTypeSec, string cSRTypeThr,
                                                 string cSerialID, string cMaterialName, string cProductNumber)
        {            
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            DataTable dt = null;
            DataTable dtProgress = null;

            StringBuilder tSQL = new StringBuilder();
            
            string ttWhere = string.Empty;
            string ttJoin = string.Empty;
            string ttStrItem = string.Empty;
            string tTempERPID = string.Empty;            
            string tSRIDUrl = string.Empty;             //服務案件URL
            string tSRContactName = string.Empty;       //客戶聯絡人
            string tSRPathWayNote = string.Empty;       //報修管道
            string tSTATUSDESC = string.Empty;          //狀態
            string tSRType = string.Empty;              //報修類別
            string tSRProductSerial = string.Empty;     //產品序號資訊
            string tSRTeam = string.Empty;              //服務團隊
            string tMainEngineerID = string.Empty;      //主要工程師ERPID
            string tMainEngineerName = string.Empty;    //主要工程師姓名
            string tAssEngineerName = string.Empty;     //協助工程師姓名
            string tTechManagerName = string.Empty;     //技術主管姓名
            string tCreatedUserName = string.Empty;     //派單人員
            string tCreatedDate = string.Empty;         //派單日期
            string tModifiedDate = string.Empty;        //最後編輯日期                          

            List<string> tListAssAndTech = new List<string>();                          //記錄所有協助工程師和所有技術主管的ERPID
            Dictionary<string, string> tDicAssAndTech = new Dictionary<string, string>();  //記錄所有協助工程師和所有技術主管的<ERPID,中、英文姓名>

            var tSRTeam_List = CMF.findSRTeamIDList(cCompanyID, false);
            var tSRContact_List = CMF.findSRDetailContactList();            
            List<TbOneSysParameter> tSRPathWay_List = CMF.findSysParameterALLDescription(pOperationID_GenerallySR, "OTHER", cCompanyID, "SRPATH");
            List<SelectListItem> ListStatus = CMF.findSRStatus(pOperationID_GenerallySR, pOperationID_InstallSR, pOperationID_MaintainSR, cCompanyID);

            #region 服務案件種類
            if (!string.IsNullOrEmpty(cSRCaseType))
            {
                ttStrItem = "";
                string[] tArySRCaseType = cSRCaseType.TrimEnd(',').Split(',');

                if (tArySRCaseType.Length >= 0)
                {
                    ttStrItem = "AND (";
                }

                foreach (string tSRCaseType in tArySRCaseType)
                {
                    ttStrItem += " M.cSRID like N'%" + tSRCaseType + "%' or";
                }

                if (tArySRCaseType.Length >= 0)
                {
                    if (ttStrItem.EndsWith("or"))
                    {
                        ttStrItem = ttStrItem.Substring(0, ttStrItem.Length - 2); //去除最後一個or  
                    }

                    ttStrItem += ") ";
                }

                ttWhere += ttStrItem + Environment.NewLine;
            }
            #endregion

            #region 狀態
            if (!string.IsNullOrEmpty(cStatus))
            {
                ttStrItem = "";
                string[] tAryStatus = cStatus.TrimEnd(',').Split(',');

                foreach (string tStatus in tAryStatus)
                {
                    ttStrItem += "N'" + tStatus + "',";
                }

                ttStrItem = ttStrItem.TrimEnd(',');

                if (ttStrItem != "")
                {
                    ttWhere += "AND M.cStatus IN (" + ttStrItem + ") " + Environment.NewLine;
                }
            }
            #endregion

            #region 申請日期
            if (!string.IsNullOrEmpty(cStartCreatedDate))
            {
                ttWhere += "AND Convert(varchar(10),M.CreatedDate,111) >= N'" + cStartCreatedDate.Replace("-", "/") + "' ";
            }

            if (!string.IsNullOrEmpty(cEndCreatedDate))
            {
                ttWhere += "AND Convert(varchar(10),M.CreatedDate,111) <= N'" + cEndCreatedDate.Replace("-", "/") + "' ";
            }
            #endregion 申請日期

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

            #region SRID
            if (!string.IsNullOrEmpty(cSRID))
            {
                ttWhere += "AND M.cSRID LIKE N'%" + cSRID.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 派單人員
            if (!string.IsNullOrEmpty(CreatedUserName))
            {
                ttWhere += "AND M.CreatedUserName LIKE N'%" + CreatedUserName.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 報修人姓名
            if (!string.IsNullOrEmpty(cRepairName))
            {
                ttWhere += "AND M.cRepairName LIKE N'%" + cRepairName.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 報修管道
            if (!string.IsNullOrEmpty(cSRPathWay))
            {
                ttStrItem = "";
                string[] tArySRPathWay = cSRPathWay.TrimEnd(',').Split(',');

                foreach (string tSRPathWay in tArySRPathWay)
                {
                    ttStrItem += "N'" + tSRPathWay + "',";
                }

                ttStrItem = ttStrItem.TrimEnd(',');

                if (ttStrItem != "")
                {
                    ttWhere += "AND M.cSRPathWay IN (" + ttStrItem + ") " + Environment.NewLine;
                }
            }
            #endregion          

            #region 主要工程師ERPID
            if (!string.IsNullOrEmpty(cMainEngineerID))
            {
                ttWhere += "AND M.cMainEngineerID = '" + cMainEngineerID + "' " + Environment.NewLine;
            }
            #endregion

            #region 協助工程師ERPID
            if (!string.IsNullOrEmpty(cAssEngineerID))
            {
                ttWhere += "AND M.cAssEngineerID LIKE N'%" + cAssEngineerID.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 技術主管ERPID
            if (!string.IsNullOrEmpty(cTechManagerID))
            {
                ttWhere += "AND M.cTechManagerID LIKE N'%" + cTechManagerID.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 報修類別-大類
            if (!string.IsNullOrEmpty(cSRTypeOne))
            {
                ttWhere += "AND M.cSRTypeOne = '" + cSRTypeOne + "' " + Environment.NewLine;
            }
            #endregion

            #region 報修類別-中類
            if (!string.IsNullOrEmpty(cSRTypeSec))
            {
                ttWhere += "AND M.cSRTypeSec = '" + cSRTypeSec + "' " + Environment.NewLine;
            }
            #endregion

            #region 報修類別-小類
            if (!string.IsNullOrEmpty(cSRTypeThr))
            {
                ttWhere += "AND M.cSRTypeThr = '" + cSRTypeThr + "' " + Environment.NewLine;
            }
            #endregion

            #region 服務團隊
            if (!string.IsNullOrEmpty(cTeamID))
            {
                ttStrItem = "";
                string[] tAryTeam = cTeamID.TrimEnd(',').Split(',');

                if(tAryTeam.Length >= 0)
                {
                    ttStrItem = "AND (";
                }

                foreach (string tTeam in tAryTeam)
                {
                    ttStrItem += " M.cTeamID like N'%" + tTeam + "%' or";
                }                

                if (tAryTeam.Length >= 0)
                {
                    if (ttStrItem.EndsWith("or"))
                    {
                        ttStrItem = ttStrItem.Substring(0, ttStrItem.Length - 2); //去除最後一個or  
                    }                    

                    ttStrItem += ") ";
                }

                ttWhere += ttStrItem + Environment.NewLine;
            }
            #endregion

            #region 產品序號
            if (!string.IsNullOrEmpty(cSerialID))
            {
                ttWhere += "AND (P.cSerialID LIKE N'%" + cSerialID.Trim() + "%' or P.cNewSerialID LIKE N'%" + cSerialID.Trim() + "%') " + Environment.NewLine;
            }
            #endregion

            #region 產品機器型號
            if (!string.IsNullOrEmpty(cMaterialName))
            {
                ttWhere += "AND P.cMaterialName LIKE N'%" + cMaterialName.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region Product Number
            if (!string.IsNullOrEmpty(cProductNumber))
            {
                ttWhere += "AND P.cProductNumber LIKE N'%" + cProductNumber.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 若【產品序號】、【產品機器型號】、【Product Number】其中有一個，就要執行Join語法
            if (!string.IsNullOrEmpty(cSerialID) || !string.IsNullOrEmpty(cMaterialName) || !string.IsNullOrEmpty(cProductNumber))
            {
                ttJoin = " left join TB_ONE_SRDetail_Product P on M.cSRID = P.cSRID";
                ttWhere = "AND P.disabled = 0 " + ttWhere;
            }
            #endregion

            #region 組待查詢清單

            #region SQL語法
            tSQL.AppendLine(" Select M.*,");
            tSQL.AppendLine("        (Select top 1");
            tSQL.AppendLine("            case when sp.cNewSerialID <> '' then sp.cSerialID + '(更換後序號：' + sp.cNewSerialID + ')' + '＃＃' + sp.cMaterialName + '＃＃' + sp.cProductNumber");
            tSQL.AppendLine("            else sp.cSerialID + '＃＃' + sp.cMaterialName + '＃＃' + sp.cProductNumber end");            
            tSQL.AppendLine("         From TB_ONE_SRDetail_Product sp where M.cSRID = sp.cSRID AND sp.disabled = 0");
            tSQL.AppendLine("        ) as Products");
            tSQL.AppendLine(" From TB_ONE_SRMain M");
            tSQL.AppendLine(ttJoin);
            tSQL.AppendLine(" Where 1=1 " + ttWhere);
            #endregion

            dt = CMF.getDataTableByDb(tSQL.ToString(), "dbOne");
            dtProgress = CMF.DistinctTable(dt);

            #region 先取得所有協助工程師和技術主管的ERPID
            foreach (DataRow dr in dtProgress.Rows)
            {
                #region 協助工程師
                CMF.findListAssAndTech(ref tListAssAndTech, dr["cAssEngineerID"].ToString());
                #endregion

                #region 技術主管
                CMF.findListAssAndTech(ref tListAssAndTech, dr["cTechManagerID"].ToString());                
                #endregion
            }
            #endregion

            #region 再取得所有協助工程師和技術主管的中文姓名
            tDicAssAndTech = CMF.findListEmployeeInfo(tListAssAndTech);
            #endregion

            foreach (DataRow dr in dtProgress.Rows)
            {
                tSRIDUrl = CMF.findSRIDUrl(dr["cSRID"].ToString());
                tSRContactName = CMF.TransSRDetailContactName(tSRContact_List, dr["cSRID"].ToString());
                tSRPathWayNote = CMF.TransSysParameterByList(tSRPathWay_List, dr["cSRPathWay"].ToString());
                tSTATUSDESC = CMF.TransSRSTATUS(ListStatus, dr["cStatus"].ToString());
                tSRTeam = CMF.TransSRTeam(tSRTeam_List, dr["cTeamID"].ToString());
                tSRType = CMF.TransSRType(dr["cSRTypeOne"].ToString(), dr["cSRTypeSec"].ToString(), dr["cSRTypeThr"].ToString());
                tSRProductSerial = CMF.TransProductSerial(dr["Products"].ToString());
                tMainEngineerID = string.IsNullOrEmpty(dr["cMainEngineerID"].ToString()) ? "" : dr["cMainEngineerID"].ToString();
                tMainEngineerName = string.IsNullOrEmpty(dr["cMainEngineerName"].ToString()) ? "" : dr["cMainEngineerName"].ToString();
                tAssEngineerName = CMF.TransEmployeeName(tDicAssAndTech, dr["cAssEngineerID"].ToString());
                tTechManagerName = CMF.TransEmployeeName(tDicAssAndTech, dr["cTechManagerID"].ToString());
                tCreatedUserName = string.IsNullOrEmpty(dr["CreatedUserName"].ToString()) ? "" : dr["CreatedUserName"].ToString();
                tCreatedDate = string.IsNullOrEmpty(dr["CreatedDate"].ToString()) ? "" : Convert.ToDateTime(dr["CreatedDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                tModifiedDate = string.IsNullOrEmpty(dr["ModifiedDate"].ToString()) ? "" : Convert.ToDateTime(dr["ModifiedDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");

                string[] QueryInfo = new string[19];                

                QueryInfo[0] = dr["cSRID"].ToString();          //SRID
                QueryInfo[1] = tSRIDUrl;                       //服務案件URL
                QueryInfo[2] = dr["cCustomerName"].ToString();   //客戶
                QueryInfo[3] = dr["cRepairName"].ToString();     //客戶報修人
                QueryInfo[4] = tSRContactName;                 //客戶聯絡人
                QueryInfo[5] = dr["cDesc"].ToString();          //說明
                QueryInfo[6] = dr["cDelayReason"].ToString();    //延遲結案原因
                QueryInfo[7] = tSRProductSerial;               //產品序號資訊
                QueryInfo[8] = tSRTeam;                        //服務團隊
                QueryInfo[9] = tSRPathWayNote;                 //報修管道
                QueryInfo[10] = tSRType;                       //報修類別                
                QueryInfo[11] = tMainEngineerID;               //主要工程師ERPID
                QueryInfo[12] = tMainEngineerName;             //主要工程師姓名
                QueryInfo[13] = tAssEngineerName;              //協助工程師姓名
                QueryInfo[14] = tTechManagerName;              //技術主管姓名
                QueryInfo[15] = tCreatedUserName;              //派單人員
                QueryInfo[16] = tCreatedDate;                  //派單日期
                QueryInfo[17] = tModifiedDate;                 //最後編輯日期
                QueryInfo[18] = tSTATUSDESC;                   //狀態                

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #endregion -----↑↑↑↑↑服務進度查詢 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓待辦清單 ↓↓↓↓↓-----
        public IActionResult ToDoList()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            try
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
                ViewBag.cLoginUser_ProfitCenterID = EmpBean.ProfitCenterID;
                ViewBag.cLoginUser_CostCenterID = EmpBean.CostCenterID;
                ViewBag.cLoginUser_CompCode = EmpBean.CompanyCode;
                ViewBag.cLoginUser_BUKRS = EmpBean.BUKRS;
                ViewBag.pIsManager = EmpBean.IsManager;
                ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");

                pCompanyCode = EmpBean.BUKRS;
                pIsManager = EmpBean.IsManager;
                #endregion

                #region 一般服務

                //取得登入人員所負責的服務團隊
                List<string> tSRTeamList = CMF.findSRTeamMappingList(EmpBean.CostCenterID, EmpBean.DepartmentNO);

                //取得登入人員所有要負責的SRID                
                List<string[]> SRIDToDoList = CMF.findSRIDList(pOperationID_GenerallySR, pOperationID_InstallSR, pOperationID_MaintainSR, pCompanyCode, pIsManager, EmpBean.EmployeeERPID, tSRTeamList);

                ViewBag.SRIDToDoList = SRIDToDoList;
                #endregion
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "失敗原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();

                CMF.writeToLog(pSRID, "ToDoList", pMsg, ViewBag.cLoginUser_Name);
            }

            return View();
        }
        #endregion -----↑↑↑↑↑待辦清單 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓一般服務 ↓↓↓↓↓-----

        #region 一般服務index
        public IActionResult GenerallySR()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            ViewBag.cLoginUser_EmployeeNO = EmpBean.EmployeeNO;
            ViewBag.cLoginUser_ERPID = EmpBean.EmployeeERPID;
            ViewBag.cLoginUser_WorkPlace = EmpBean.WorkPlace;
            ViewBag.cLoginUser_DepartmentName = EmpBean.DepartmentName;
            ViewBag.cLoginUser_DepartmentNO = EmpBean.DepartmentNO;
            ViewBag.cLoginUser_ProfitCenterID = EmpBean.ProfitCenterID;
            ViewBag.cLoginUser_CostCenterID = EmpBean.CostCenterID;
            ViewBag.cLoginUser_CompCode = EmpBean.CompanyCode;
            ViewBag.cLoginUser_BUKRS = EmpBean.BUKRS;
            ViewBag.cLoginUser_IsManager = EmpBean.IsManager;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");

            pCompanyCode = EmpBean.BUKRS;
            #endregion

            var model = new ViewModel();

            #region Request參數            
            if (HttpContext.Request.Query["SRID"].FirstOrDefault() != null)
            {
                pSRID = HttpContext.Request.Query["SRID"].FirstOrDefault();
            }
            #endregion

            #region 報修類別
            //大類
            var SRTypeOneList = CMF.findFirstKINDList();

            //中類
            var SRTypeSecList = new List<SelectListItem>();
            SRTypeSecList.Add(new SelectListItem { Text = " ", Value = "" });

            //小類
            var SRTypeThrList = new List<SelectListItem>();
            SRTypeThrList.Add(new SelectListItem { Text = " ", Value = "" });           
            #endregion

            #region 取得服務團隊清單
            var SRTeamIDList = CMF.findSRTeamIDList(pCompanyCode, true);           
            #endregion

            #region 取得SRID
            var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pSRID);

            if (beanM != null)
            {
                //記錄目前GUID，用來判斷更新的先後順序
                ViewBag.pGUID = beanM.CSystemGuid.ToString();
                
                //判斷登入者是否可以編輯服務案件
                pIsCanEditSR = CMF.checkIsCanEditSR(beanM.CSrid, EmpBean.EmployeeERPID, pIsMIS, pIsCS);

                #region 報修資訊
                ViewBag.cSRID = beanM.CSrid;
                ViewBag.cCustomerID = beanM.CCustomerId;
                ViewBag.cCustomerName = beanM.CCustomerName;                
                ViewBag.cDesc = beanM.CDesc;
                ViewBag.cNotes = beanM.CNotes;
                ViewBag.cAttachement = beanM.CAttachement;
                ViewBag.cSRPathWay = beanM.CSrpathWay;
                ViewBag.cMAServiceType = beanM.CMaserviceType;
                ViewBag.cSRProcessWay = beanM.CSrprocessWay;
                ViewBag.cDelayReason = beanM.CDelayReason;
                ViewBag.cIsSecondFix = beanM.CIsSecondFix;
                ViewBag.pStatus = beanM.CStatus;

                ViewBag.cCustomerType = beanM.CCustomerId.Substring(0, 1) == "P" ? "P" : "C";
                #endregion

                #region 報修類別
                if (!string.IsNullOrEmpty(beanM.CSrtypeOne))    
                {                    
                    SRTypeOneList.Where(q => q.Value == beanM.CSrtypeOne).First().Selected = true;
                }

                if (!string.IsNullOrEmpty(beanM.CSrtypeSec))
                {
                    SRTypeSecList = CMF.findSRTypeSecList(beanM.CSrtypeOne);
                    SRTypeSecList.Where(q => q.Value == beanM.CSrtypeSec).First().Selected = true;
                }

                if (!string.IsNullOrEmpty(beanM.CSrtypeThr))
                {
                    SRTypeThrList = CMF.findSRTypeThrList(beanM.CSrtypeSec);
                    SRTypeThrList.Where(q => q.Value == beanM.CSrtypeThr).First().Selected = true;
                }
                #endregion               

                #region 客戶報修窗口資訊
                ViewBag.cRepairName = beanM.CRepairName;
                ViewBag.cRepairAddress = beanM.CRepairAddress;
                ViewBag.cRepairPhone = beanM.CRepairPhone;
                ViewBag.cRepairMobile = beanM.CRepairMobile;
                ViewBag.cRepairEmail = beanM.CRepairEmail;               
                #endregion

                #region 服務團隊
                ViewBag.cTeamID = beanM.CTeamId;
                ViewBag.cMainEngineerID = beanM.CMainEngineerId;
                ViewBag.cMainEngineerName = beanM.CMainEngineerName;
                ViewBag.cSQPersonID = beanM.CSqpersonId;
                ViewBag.cSQPersonName = beanM.CSqpersonName;
                ViewBag.cSalesID = beanM.CSalesId;
                ViewBag.cSalesName = beanM.CSalesName;
                ViewBag.cAssEngineerID = beanM.CAssEngineerId;
                ViewBag.cTechManagerID = beanM.CTechManagerId;
                #endregion

                ViewBag.CreatedDate = Convert.ToDateTime(beanM.CreatedDate).ToString("yyyy-MM-dd");

                #region 取得客戶聯絡人資訊(明細)
                var beansContact = dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                ViewBag.Detailbean_Contact = beansContact;
                ViewBag.trContactNo = beansContact.Count();
                #endregion

                #region 取得產品序號資訊(明細)
                var beansProduct = dbOne.TbOneSrdetailProducts.OrderBy(x => x.CSerialId).Where(x => x.Disabled == 0 && x.CSrid == pSRID);
                
                ViewBag.Detailbean_Product = beansProduct;
                ViewBag.trProductNo = beansProduct.Count();
                #endregion

                #region 取得工時紀錄檔資訊(明細)
                var beansRecord = dbOne.TbOneSrdetailRecords.OrderBy(x => x.CId).ThenByDescending(x => x.CFinishTime).Where(x => x.Disabled == 0 && x.CSrid == pSRID);
                
                ViewBag.Detailbean_Record = beansRecord;
                ViewBag.trRecordNo = beansRecord.Count();
                #endregion

                #region 取得零件更換資訊(明細)
                var beansParts = dbOne.TbOneSrdetailPartsReplaces.OrderBy(x => x.CId).Where(x => x.Disabled == 0 && x.CSrid == pSRID);
                
                ViewBag.Detailbean_Parts = beansParts;
                ViewBag.trPartsNo = beansParts.Count();
                #endregion
            }
            else
            {
                ViewBag.cSRID = "";
                ViewBag.cSRPathWay = "Z05";     //手動建立
                ViewBag.pStatus = "E0001";      //新建
                ViewBag.cMAServiceType = "";    //請選擇
                ViewBag.cSRProcessWay = "";     //請選擇
                ViewBag.cDelayReason = "";      //空值
                ViewBag.cIsSecondFix = "";     //請選擇
            }
            #endregion

            #region 指派Option值
            model.ddl_cStatus = ViewBag.pStatus;                //設定狀態
            model.ddl_cSRPathWay = ViewBag.cSRPathWay;          //設定報修管道
            model.ddl_cMAServiceType = ViewBag.cMAServiceType;   //設定維護服務種類
            model.ddl_cSRProcessWay = ViewBag.cSRProcessWay;    //設定處理方式
            model.ddl_cIsSecondFix = ViewBag.cIsSecondFix;      //是否為二修
            model.ddl_cCustomerType = ViewBag.cCustomerType;    //客戶類型(P.個人 C.法人)
            #endregion

            ViewBag.SRTypeOneList = SRTypeOneList;
            ViewBag.SRTypeSecList = SRTypeSecList;
            ViewBag.SRTypeThrList = SRTypeThrList;            
            ViewBag.SRTeamIDList = SRTeamIDList;

            ViewBag.pOperationID = pOperationID_GenerallySR;
            ViewBag.pIsCanEditSR = pIsCanEditSR.ToString();  //登入者是否可以編輯服務案件

            return View(model);
        }
        #endregion

        #region 儲存一般服務
        /// <summary>
        /// 儲存一般服務
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveGenerallySR(IFormCollection formCollection)
        {            
            getLoginAccount();

            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            pLoginName = EmpBean.EmployeeCName;

            pSRID = formCollection["hid_cSRID"].FirstOrDefault();

            bool tIsFormal = false;
            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;
            string tLog = string.Empty;

            string OldCStatus = string.Empty;
            string OldCCustomerName = string.Empty;            
            string OldCCustomerId = string.Empty;
            string OldCDesc = string.Empty;
            string OldCNotes = string.Empty;
            string OldCAttachement = string.Empty;
            string OldCMaserviceType = string.Empty;
            string OldCSrtypeOne = string.Empty;
            string OldCSrtypeSec = string.Empty;
            string OldCSrtypeThr = string.Empty;
            string OldCSrpathWay = string.Empty;
            string OldCSrprocessWay = string.Empty;
            string OldCDelayReason = string.Empty;
            string OldCIsSecondFix = string.Empty;
            string OldCRepairName = string.Empty;
            string OldCRepairAddress = string.Empty;
            string OldCRepairPhone = string.Empty;
            string OldCRepairMobile = string.Empty;
            string OldCRepairEmail = string.Empty;
            string OldCTeamId = string.Empty;
            string OldCSqpersonId = string.Empty;
            string OldCSqpersonName = string.Empty;
            string OldCSalesName = string.Empty;
            string OldCSalesId = string.Empty;
            string OldCMainEngineerName = string.Empty;
            string OldCMainEngineerId = string.Empty;
            string OldCAssEngineerId = string.Empty;
            string OldCTechManagerId = string.Empty;

            string CStatus = formCollection["ddl_cStatus"].FirstOrDefault();
            string CCustomerName = formCollection["tbx_cCustomerName"].FirstOrDefault();
            string CCustomerId = formCollection["hid_cCustomerID"].FirstOrDefault();            
            string CDesc = formCollection["tbx_cDesc"].FirstOrDefault();
            string CNotes = formCollection["tbx_cNotes"].FirstOrDefault();
            string CAttach = formCollection["hid_filezone_1"].FirstOrDefault();
            string CMaserviceType = formCollection["ddl_cMAServiceType"].FirstOrDefault();
            string CSrtypeOne = formCollection["ddl_cSRTypeOne"].FirstOrDefault();
            string CSrtypeSec = formCollection["ddl_cSRTypeSec"].FirstOrDefault();
            string CSrtypeThr = formCollection["ddl_cSRTypeThr"].FirstOrDefault();
            string CSrpathWay = formCollection["ddl_cSRPathWay"].FirstOrDefault();
            string CSrprocessWay = formCollection["ddl_cSRProcessWay"].FirstOrDefault();
            string CRepairEmail = formCollection["tbx_cRepairEmail"].FirstOrDefault();
            string CIsSecondFix = formCollection["ddl_cIsSecondFix"].FirstOrDefault();
            string CRepairName = formCollection["tbx_cRepairName"].FirstOrDefault();
            string CRepairAddress = formCollection["tbx_cRepairAddress"].FirstOrDefault();
            string CRepairPhone = formCollection["tbx_cRepairPhone"].FirstOrDefault();
            string CRepairMobile = formCollection["tbx_cRepairMobile"].FirstOrDefault();
            string CDelayReason = formCollection["tbx_cDelayReason"].FirstOrDefault();            
            string CTeamId = formCollection["hid_cTeamID"].FirstOrDefault();
            string CSqpersonId = formCollection["hid_cSQPersonID"].FirstOrDefault();
            string CSqpersonName = formCollection["tbx_cSQPersonName"].FirstOrDefault();
            string CSalesName = formCollection["tbx_cSalesName"].FirstOrDefault();
            string CSalesId = formCollection["hid_cSalesID"].FirstOrDefault();
            string CMainEngineerName = formCollection["tbx_cMainEngineerName"].FirstOrDefault();
            string CMainEngineerId = formCollection["hid_cMainEngineerID"].FirstOrDefault();
            string CAssEngineerId = formCollection["hid_cAssEngineerID"].FirstOrDefault();
            string CTechManagerId = formCollection["hid_cTechManagerID"].FirstOrDefault();
            string LoginUser_Name = formCollection["hid_cLoginUser_Name"].FirstOrDefault();

            SRCondition srCon = new SRCondition();
            SRMain_SRSTATUS_INPUT beanIN = new SRMain_SRSTATUS_INPUT();

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

                var beanNowM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pSRID);

                if (beanNowM == null)
                {
                    #region 新增主檔
                    TbOneSrmain beanM = new TbOneSrmain();

                    srCon = SRCondition.ADD;

                    //主表資料
                    beanM.CSrid = pSRID;
                    beanM.CStatus = "E0005";    //新增時先預設為L3.處理中
                    beanM.CCustomerName = CCustomerName;
                    beanM.CCustomerId = CCustomerId;                    
                    beanM.CDesc = CDesc;
                    beanM.CNotes = CNotes;
                    beanM.CAttachement = CAttach;
                    beanM.CMaserviceType = CMaserviceType;
                    beanM.CSrtypeOne = CSrtypeOne;
                    beanM.CSrtypeSec = CSrtypeSec;
                    beanM.CSrtypeThr = CSrtypeThr;
                    beanM.CSrpathWay = CSrpathWay;
                    beanM.CSrprocessWay = CSrprocessWay;
                    beanM.CDelayReason = CDelayReason;
                    beanM.CIsSecondFix = CIsSecondFix;
                    beanM.CRepairName = CRepairName;
                    beanM.CRepairAddress = CRepairAddress;
                    beanM.CRepairPhone = CRepairPhone;
                    beanM.CRepairMobile = CRepairMobile;
                    beanM.CRepairEmail = CRepairEmail;                    
                    beanM.CTeamId = CTeamId;
                    beanM.CSqpersonId = CSqpersonId;
                    beanM.CSqpersonName = CSqpersonName;
                    beanM.CSalesName = CSalesName;
                    beanM.CSalesId = CSalesId;
                    beanM.CMainEngineerName = CMainEngineerName;
                    beanM.CMainEngineerId = CMainEngineerId;
                    beanM.CAssEngineerId = CAssEngineerId;
                    beanM.CTechManagerId = CTechManagerId;
                    beanM.CSystemGuid = Guid.NewGuid();
                    beanM.CIsAppclose = "";

                    beanM.CreatedDate = DateTime.Now;
                    beanM.CreatedUserName = LoginUser_Name;

                    dbOne.TbOneSrmains.Add(beanM);
                    #endregion

                    #region 新增【客戶聯絡窗口資訊】明細
                    string[] COcContactName = formCollection["tbx_COcContactName"];
                    string[] COcContactAddress = formCollection["tbx_COcContactAddress"];
                    string[] COcContactPhone = formCollection["tbx_COcContactPhone"];
                    string[] COcContactMobile = formCollection["tbx_COcContactMobile"];
                    string[] COcContactEmail = formCollection["tbx_COcContactEmail"];
                    string[] COcDisabled = formCollection["hid_COcDisabled"];

                    int countCO = COcContactName.Length;

                    for (int i = 0; i < countCO; i++)
                    {
                        if (COcDisabled[i] == "0")
                        {
                            TbOneSrdetailContact beanD = new TbOneSrdetailContact();

                            beanD.CSrid = pSRID;
                            beanD.CContactName = COcContactName[i];
                            beanD.CContactAddress = COcContactAddress[i];
                            beanD.CContactPhone = COcContactPhone[i];
                            beanD.CContactMobile = COcContactMobile[i];
                            beanD.CContactEmail = COcContactEmail[i];
                            beanD.Disabled = 0;

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailContacts.Add(beanD);
                        }
                    }
                    #endregion

                    #region 新增【產品序號資訊】明細
                    string[] PRcSerialID = formCollection["tbx_PRcSerialID"];
                    string[] PRcMaterialID = formCollection["hid_PRcMaterialID"];
                    string[] PRcMaterialName = formCollection["tbx_PRcMaterialName"];
                    string[] PRcProductNumber = formCollection["tbx_PRcProductNumber"];
                    string[] PRcNewSerialID = formCollection["tbx_PRcNewSerialID"];
                    string[] PRcInstallID = formCollection["tbx_PRcInstallID"];
                    string[] PRcDisabled = formCollection["hid_PRcDisabled"];

                    int countPR = PRcSerialID.Length;

                    for (int i = 0; i < countPR; i++)
                    {
                        if (PRcDisabled[i] == "0")
                        {
                            TbOneSrdetailProduct beanD = new TbOneSrdetailProduct();

                            beanD.CSrid = pSRID;
                            beanD.CSerialId = PRcSerialID[i];
                            beanD.CMaterialId = PRcMaterialID[i];
                            beanD.CMaterialName = PRcMaterialName[i];
                            beanD.CProductNumber = PRcProductNumber[i];
                            beanD.CNewSerialId = PRcNewSerialID[i];
                            beanD.CInstallId = PRcInstallID[i];
                            beanD.Disabled = 0;

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailProducts.Add(beanD);
                        }
                    }
                    #endregion

                    #region 新增【保固SLA資訊】明細
                    string[] WAcSerialID = formCollection["hidcSerialID"];
                    string[] WAcWTYID = formCollection["hidcWTYID"];
                    string[] WAcWTYName = formCollection["hidcWTYName"];
                    string[] WAcWTYSDATE = formCollection["hidcWTYSDATE"];
                    string[] WAcWTYEDATE = formCollection["hidcWTYEDATE"];
                    string[] WAcSLARESP = formCollection["hidcSLARESP"];
                    string[] WAcSLASRV = formCollection["hidcSLASRV"];
                    string[] WAcContractID = formCollection["hidcContractID"];
                    string[] WAcSUB_CONTRACTID = formCollection["hidcSUB_CONTRACTID"];
                    string[] WAcBPMFormNo = formCollection["hidcBPMFormNo"];
                    string[] WAcAdvice = formCollection["hidcAdvice"];
                    string[] WACheckUsed = formCollection["hid_CheckUsed"];

                    int countWA = WAcSerialID.Length;

                    for (int i = 0; i < countWA; i++)
                    {
                        TbOneSrdetailWarranty beanD = new TbOneSrdetailWarranty();

                        beanD.CSrid = pSRID;
                        beanD.CSerialId = WAcSerialID[i];
                        beanD.CWtyid = WAcWTYID[i];
                        beanD.CWtyname = WAcWTYName[i];

                        if (WAcWTYSDATE[i] != "")
                        {
                            beanD.CWtysdate = Convert.ToDateTime(WAcWTYSDATE[i]);
                        }

                        if (WAcWTYEDATE[i] != "")
                        {
                            beanD.CWtyedate = Convert.ToDateTime(WAcWTYEDATE[i]);
                        }

                        beanD.CSlaresp = WAcSLARESP[i];
                        beanD.CSlasrv = WAcSLASRV[i];
                        beanD.CContractId = WAcContractID[i];
                        beanD.CSubContractId = WAcSUB_CONTRACTID[i];
                        beanD.CBpmformNo = WAcBPMFormNo[i];
                        beanD.CAdvice = WAcAdvice[i];
                        beanD.CUsed = WACheckUsed[i];

                        beanD.CreatedDate = DateTime.Now;
                        beanD.CreatedUserName = LoginUser_Name;

                        dbOne.TbOneSrdetailWarranties.Add(beanD);
                    }
                    #endregion                    

                    int result = dbOne.SaveChanges();

                    if (result <= 0)
                    {
                        pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "提交失敗(新建)" + Environment.NewLine;
                        CMF.writeToLog(pSRID, "SaveGenerallySR", pMsg, LoginUser_Name);
                    }
                    else
                    {
                        #region 紀錄修改log
                        TbOneLog logBean = new TbOneLog
                        {
                            CSrid = pSRID,
                            EventName = "SaveGenerallySR",
                            Log = "新增成功！",
                            CreatedUserName = LoginUser_Name,
                            CreatedDate = DateTime.Now
                        };

                        dbOne.TbOneLogs.Add(logBean);
                        dbOne.SaveChanges();
                        #endregion

                        #region call ONE SERVICE 服務案件(一般/裝機/定維)狀態更新接口來寄送Mail
                        beanIN.IV_LOGINEMPNO = EmpBean.EmployeeERPID;
                        beanIN.IV_LOGINEMPNAME = LoginUser_Name;
                        beanIN.IV_SRID = pSRID;
                        beanIN.IV_STATUS = "E0005|ADD"; //新建但狀態是L3處理中
                        beanIN.IV_APIURLName = tAPIURLName;

                        CMF.GetAPI_SRSTATUS_Update(beanIN);
                        #endregion
                    }
                }
                else
                {
                    #region 修改主檔

                    #region 紀錄新舊值
                    OldCStatus = beanNowM.CStatus;
                    tLog += CMF.getNewAndOldLog("狀態", OldCStatus, CStatus);

                    OldCCustomerName = beanNowM.CCustomerName;
                    tLog += CMF.getNewAndOldLog("客戶名稱", OldCCustomerName, CCustomerName);

                    OldCCustomerId = beanNowM.CCustomerId;
                    tLog += CMF.getNewAndOldLog("客戶ID", OldCCustomerId, CCustomerId);

                    OldCDesc = beanNowM.CDesc;
                    tLog += CMF.getNewAndOldLog("說明", OldCDesc, CDesc);

                    OldCNotes = beanNowM.CNotes;
                    tLog += CMF.getNewAndOldLog("詳細描述", OldCNotes, CNotes);

                    OldCAttachement = beanNowM.CAttachement;
                    tLog += CMF.getNewAndOldLog("檢附文件", OldCAttachement, CAttach);

                    OldCMaserviceType = beanNowM.CMaserviceType;
                    tLog += CMF.getNewAndOldLog("維護服務種類", OldCMaserviceType, CMaserviceType);

                    OldCSrtypeOne = beanNowM.CSrtypeOne;
                    tLog += CMF.getNewAndOldLog("報修類別(大類)", OldCSrtypeOne, CSrtypeOne);

                    OldCSrtypeSec = beanNowM.CSrtypeSec;
                    tLog += CMF.getNewAndOldLog("報修類別(中類)", OldCSrtypeSec, CSrtypeSec);

                    OldCSrtypeThr = beanNowM.CSrtypeThr;
                    tLog += CMF.getNewAndOldLog("報修類別(中類)", OldCSrtypeThr, CSrtypeThr);

                    OldCSrpathWay = beanNowM.CSrpathWay;
                    tLog += CMF.getNewAndOldLog("報修管道", OldCSrpathWay, CSrpathWay);

                    OldCSrprocessWay = beanNowM.CSrprocessWay;
                    tLog += CMF.getNewAndOldLog("處理方式", OldCSrprocessWay, CSrprocessWay);

                    OldCDelayReason = beanNowM.CDelayReason;
                    tLog += CMF.getNewAndOldLog("延遲結案原因", OldCDelayReason, CDelayReason);

                    OldCIsSecondFix = beanNowM.CIsSecondFix;
                    tLog += CMF.getNewAndOldLog("是否為二修", OldCIsSecondFix, CIsSecondFix);

                    OldCRepairName = beanNowM.CRepairName;
                    tLog += CMF.getNewAndOldLog("姓名(報修人)", OldCRepairName, CRepairName);

                    OldCRepairAddress = beanNowM.CRepairAddress;
                    tLog += CMF.getNewAndOldLog("地址(報修人)", OldCRepairAddress, CRepairAddress);

                    OldCRepairPhone = beanNowM.CRepairPhone;
                    tLog += CMF.getNewAndOldLog("電話(報修人)", OldCRepairPhone, CRepairPhone);

                    OldCRepairMobile = beanNowM.CRepairMobile;
                    tLog += CMF.getNewAndOldLog("手機(報修人)", OldCRepairMobile, CRepairMobile);

                    OldCRepairEmail = beanNowM.CRepairEmail;
                    tLog += CMF.getNewAndOldLog("Email(報修人)", OldCRepairEmail, CRepairEmail);

                    OldCTeamId = beanNowM.CTeamId;
                    tLog += CMF.getNewAndOldLog("服務團隊", OldCTeamId, CTeamId);

                    OldCSqpersonId = beanNowM.CSqpersonId;
                    tLog += CMF.getNewAndOldLog("SQ人員ID", OldCSqpersonId, CSqpersonId);

                    OldCSqpersonName = beanNowM.CSqpersonName;
                    tLog += CMF.getNewAndOldLog("SQ人員名稱", OldCSqpersonName, CSqpersonName);

                    OldCSalesName = beanNowM.CSalesName;
                    tLog += CMF.getNewAndOldLog("計費業務姓名", OldCSalesName, CSalesName);

                    OldCSalesId = beanNowM.CSalesId;
                    tLog += CMF.getNewAndOldLog("計費業務ERPID", OldCSalesId, CSalesId);

                    OldCMainEngineerName = beanNowM.CMainEngineerName;
                    tLog += CMF.getNewAndOldLog("主要工程師姓名", OldCMainEngineerName, CMainEngineerName);

                    OldCMainEngineerId = beanNowM.CMainEngineerId;
                    tLog += CMF.getNewAndOldLog("主要工程師ERPID", OldCMainEngineerId, CMainEngineerId);

                    OldCAssEngineerId = beanNowM.CAssEngineerId;
                    tLog += CMF.getNewAndOldLog("協助工程師ERPID", OldCAssEngineerId, CAssEngineerId);

                    OldCTechManagerId = beanNowM.CTechManagerId;
                    tLog += CMF.getNewAndOldLog("技術主管ERPID", OldCTechManagerId, CTechManagerId);
                    #endregion

                    //主表資料
                    beanNowM.CStatus = CStatus;
                    beanNowM.CCustomerName = CCustomerName;
                    beanNowM.CCustomerId = CCustomerId;                    
                    beanNowM.CDesc = CDesc;
                    beanNowM.CNotes = CNotes;
                    beanNowM.CAttachement = CAttach;
                    beanNowM.CMaserviceType = CMaserviceType;
                    beanNowM.CSrtypeOne = CSrtypeOne;
                    beanNowM.CSrtypeSec = CSrtypeSec;
                    beanNowM.CSrtypeThr = CSrtypeThr;
                    beanNowM.CSrpathWay = CSrpathWay;
                    beanNowM.CSrprocessWay = CSrprocessWay;
                    beanNowM.CDelayReason = CDelayReason;
                    beanNowM.CIsSecondFix = CIsSecondFix;
                    beanNowM.CRepairName = CRepairName;
                    beanNowM.CRepairAddress = CRepairAddress;
                    beanNowM.CRepairPhone = CRepairPhone;
                    beanNowM.CRepairMobile = CRepairMobile;
                    beanNowM.CRepairEmail = CRepairEmail;                    
                    beanNowM.CTeamId = CTeamId;
                    beanNowM.CSqpersonId = CSqpersonId;
                    beanNowM.CSqpersonName = CSqpersonName;
                    beanNowM.CSalesName = CSalesName;
                    beanNowM.CSalesId = CSalesId;
                    beanNowM.CMainEngineerName = CMainEngineerName;
                    beanNowM.CMainEngineerId = CMainEngineerId;
                    beanNowM.CAssEngineerId = CAssEngineerId;
                    beanNowM.CTechManagerId = CTechManagerId;
                    beanNowM.CSystemGuid = Guid.NewGuid();

                    if (CStatus == "E0006") //完修
                    {
                        beanNowM.CIsAppclose = "N";
                    }

                    beanNowM.ModifiedDate = DateTime.Now;
                    beanNowM.ModifiedUserName = LoginUser_Name;
                    #endregion

                    #region -----↓↓↓↓↓客戶聯絡窗口資訊↓↓↓↓↓-----

                    #region 刪除明細                    
                    //dbOne.TbOneSrdetailContacts.RemoveRange(dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pSRID));

                    var beansDCon = dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    foreach(var beanDCon in beansDCon)
                    {
                        beanDCon.Disabled = 1;
                        beanDCon.ModifiedDate = DateTime.Now;
                        beanDCon.ModifiedUserName = LoginUser_Name;
                    }
                    #endregion

                    #region 新增【客戶聯絡窗口資訊】明細
                    string[] COcContactName = formCollection["tbx_COcContactName"];
                    string[] COcContactAddress = formCollection["tbx_COcContactAddress"];
                    string[] COcContactPhone = formCollection["tbx_COcContactPhone"];
                    string[] COcContactMobile = formCollection["tbx_COcContactMobile"];
                    string[] COcContactEmail = formCollection["tbx_COcContactEmail"];
                    string[] COcDisabled = formCollection["hid_COcDisabled"];

                    int countCO = COcContactName.Length;

                    for (int i = 0; i < countCO; i++)
                    {
                        if (COcDisabled[i] == "0")
                        {
                            TbOneSrdetailContact beanD = new TbOneSrdetailContact();

                            beanD.CSrid = pSRID;
                            beanD.CContactName = COcContactName[i];
                            beanD.CContactAddress = COcContactAddress[i];
                            beanD.CContactPhone = COcContactPhone[i];
                            beanD.CContactMobile = COcContactMobile[i];
                            beanD.CContactEmail = COcContactEmail[i];
                            beanD.Disabled = int.Parse(COcDisabled[i]);

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailContacts.Add(beanD);
                        }
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑客戶聯絡窗口資訊 ↑↑↑↑↑-----

                    #region -----↓↓↓↓↓產品序號資訊↓↓↓↓↓-----

                    #region 刪除明細                    
                    //dbOne.TbOneSrdetailProducts.RemoveRange(dbOne.TbOneSrdetailProducts.Where(x => x.Disabled == 0 && x.CSrid == pSRID));

                    var beansDPro = dbOne.TbOneSrdetailProducts.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    foreach (var beanDPro in beansDPro)
                    {
                        beanDPro.Disabled = 1;
                        beanDPro.ModifiedDate = DateTime.Now;
                        beanDPro.ModifiedUserName = LoginUser_Name;
                    }
                    #endregion

                    #region 新增明細
                    string[] PRcSerialID = formCollection["tbx_PRcSerialID"];
                    string[] PRcMaterialID = formCollection["hid_PRcMaterialID"];
                    string[] PRcMaterialName = formCollection["tbx_PRcMaterialName"];
                    string[] PRcProductNumber = formCollection["tbx_PRcProductNumber"];
                    string[] PRcNewSerialID = formCollection["tbx_PRcNewSerialID"];
                    string[] PRcInstallID = formCollection["tbx_PRcInstallID"];
                    string[] PRcDisabled = formCollection["hid_PRcDisabled"];

                    int countPR = PRcSerialID.Length;

                    for (int i = 0; i < countPR; i++)
                    {
                        if (PRcDisabled[i] == "0")
                        {
                            TbOneSrdetailProduct beanD = new TbOneSrdetailProduct();

                            beanD.CSrid = pSRID;
                            beanD.CSerialId = PRcSerialID[i];
                            beanD.CMaterialId = PRcMaterialID[i];
                            beanD.CMaterialName = PRcMaterialName[i];
                            beanD.CProductNumber = PRcProductNumber[i];
                            beanD.CNewSerialId = PRcNewSerialID[i];
                            beanD.CInstallId = PRcInstallID[i];
                            beanD.Disabled = int.Parse(PRcDisabled[i]);

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailProducts.Add(beanD);
                        }
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑產品序號資訊 ↑↑↑↑↑-----

                    #region -----↓↓↓↓↓保固SLA資訊↓↓↓↓↓-----

                    #region 刪除明細
                    dbOne.TbOneSrdetailWarranties.RemoveRange(dbOne.TbOneSrdetailWarranties.Where(x => x.CSrid == pSRID));                    
                    #endregion

                    #region 新增明細
                    string[] WAcSerialID = formCollection["hidcSerialID"];
                    string[] WAcWTYID = formCollection["hidcWTYID"];
                    string[] WAcWTYName = formCollection["hidcWTYName"];
                    string[] WAcWTYSDATE = formCollection["hidcWTYSDATE"];
                    string[] WAcWTYEDATE = formCollection["hidcWTYEDATE"];
                    string[] WAcSLARESP = formCollection["hidcSLARESP"];
                    string[] WAcSLASRV = formCollection["hidcSLASRV"];
                    string[] WAcContractID = formCollection["hidcContractID"];
                    string[] WAcSUB_CONTRACTID = formCollection["hidcSUB_CONTRACTID"];
                    string[] WAcBPMFormNo = formCollection["hidcBPMFormNo"];
                    string[] WAcAdvice = formCollection["hidcAdvice"];
                    string[] WACheckUsed = formCollection["hid_CheckUsed"];

                    int countWA = WAcSerialID.Length;

                    for (int i = 0; i < countWA; i++)
                    {
                        TbOneSrdetailWarranty beanD = new TbOneSrdetailWarranty();

                        beanD.CSrid = pSRID;
                        beanD.CSerialId = WAcSerialID[i];
                        beanD.CWtyid = WAcWTYID[i];
                        beanD.CWtyname = WAcWTYName[i];

                        if (WAcWTYSDATE[i] != "")
                        {
                            beanD.CWtysdate = Convert.ToDateTime(WAcWTYSDATE[i]);
                        }

                        if (WAcWTYEDATE[i] != "")
                        {
                            beanD.CWtyedate = Convert.ToDateTime(WAcWTYEDATE[i]);
                        }

                        beanD.CSlaresp = WAcSLARESP[i];
                        beanD.CSlasrv = WAcSLASRV[i];
                        beanD.CContractId = WAcContractID[i];
                        beanD.CSubContractId = WAcSUB_CONTRACTID[i];
                        beanD.CBpmformNo = WAcBPMFormNo[i];
                        beanD.CAdvice = WAcAdvice[i];
                        beanD.CUsed = WACheckUsed[i];

                        beanD.CreatedDate = DateTime.Now;
                        beanD.CreatedUserName = LoginUser_Name;

                        dbOne.TbOneSrdetailWarranties.Add(beanD);
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑保固SLA資訊 ↑↑↑↑↑-----

                    #region -----↓↓↓↓↓處理與工時紀錄↓↓↓↓↓-----

                    #region 刪除明細
                    //dbOne.TbOneSrdetailRecords.RemoveRange(dbOne.TbOneSrdetailRecords.Where(x => x.Disabled == 0 && x.CSrid == pSRID));

                    var beansDRec = dbOne.TbOneSrdetailRecords.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    foreach (var beanDRec in beansDRec)
                    {
                        beanDRec.Disabled = 1;
                        beanDRec.ModifiedDate = DateTime.Now;
                        beanDRec.ModifiedUserName = LoginUser_Name;
                    }
                    #endregion

                    #region 新增明細
                    string[] REcEngineerName = formCollection["tbx_REcEngineerName"];
                    string[] REcEngineerID = formCollection["hid_REcEngineerID"];
                    string[] REcReceiveTime = formCollection["tbx_REcReceiveTime"];
                    string[] REcStartTime = formCollection["tbx_REcStartTime"];
                    string[] REcArriveTime = formCollection["tbx_REcArriveTime"];
                    string[] REcFinishTime = formCollection["tbx_REcFinishTime"];
                    string[] REcWorkHours = formCollection["tbx_REcWorkHours"];
                    string[] REcDesc = formCollection["tbx_REcDesc"];
                    string[] REcSRReport = formCollection["hid_filezoneRE"];                    
                    string[] REcDisabled = formCollection["hid_REcDisabled"];

                    int countRE = REcEngineerName.Length;

                    for (int i = 0; i < countRE; i++)
                    {
                        if (REcDisabled[i] == "0")
                        {
                            TbOneSrdetailRecord beanD = new TbOneSrdetailRecord();

                            beanD.CSrid = pSRID;
                            beanD.CEngineerName = REcEngineerName[i];
                            beanD.CEngineerId = REcEngineerID[i];

                            if (REcReceiveTime[i] != "")
                            {
                                beanD.CReceiveTime = Convert.ToDateTime(REcReceiveTime[i]);
                            }

                            if (REcStartTime[i] != "")
                            {
                                beanD.CStartTime = Convert.ToDateTime(REcStartTime[i]);
                            }

                            if (REcArriveTime[i] != "")
                            {
                                beanD.CArriveTime = Convert.ToDateTime(REcArriveTime[i]);
                            }

                            if (REcFinishTime[i] != "")
                            {
                                beanD.CFinishTime = Convert.ToDateTime(REcFinishTime[i]);
                            }

                            beanD.CWorkHours = decimal.Parse(REcWorkHours[i]);
                            beanD.CDesc = REcDesc[i];
                            beanD.CSrreport = REcSRReport[i];
                            beanD.Disabled = int.Parse(REcDisabled[i]);

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailRecords.Add(beanD);
                        }
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑處理與工時紀錄 ↑↑↑↑↑-----

                    #region -----↓↓↓↓↓零件更換資訊↓↓↓↓↓-----

                    #region 刪除明細                   
                    //dbOne.TbOneSrdetailPartsReplaces.RemoveRange(dbOne.TbOneSrdetailPartsReplaces.Where(x => x.Disabled == 0 && x.CSrid == pSRID));

                    var beansDPar = dbOne.TbOneSrdetailPartsReplaces.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    foreach (var beanDPar in beansDPar)
                    {
                        beanDPar.Disabled = 1;
                        beanDPar.ModifiedDate = DateTime.Now;
                        beanDPar.ModifiedUserName = LoginUser_Name;
                    }
                    #endregion

                    #region 新增明細
                    string[] PAcXCHP = formCollection["tbx_PAcXCHP"];
                    string[] PAcMaterialID = formCollection["tbx_PAcMaterialID"];
                    string[] PAcMaterialName = formCollection["tbx_PAcMaterialName"];
                    string[] PAcOldCT = formCollection["tbx_PAcOldCT"];
                    string[] PAcNewCT = formCollection["tbx_PAcNewCT"];
                    string[] PAcHPCT = formCollection["tbx_PAcHPCT"];
                    string[] PAcNewUEFI = formCollection["tbx_PAcNewUEFI"];
                    string[] PAcStandbySerialID = formCollection["tbx_PAcStandbySerialID"];
                    string[] PAcHPCaseID = formCollection["tbx_PAcHPCaseID"];
                    string[] PAcArriveDate = formCollection["tbx_PAcArriveDate"];
                    string[] PAcReturnDate = formCollection["tbx_PAcReturnDate"];
                    string[] PAcPersonalDamage = formCollection["ddl_PAcPersonalDamage"];
                    string[] PAcNote = formCollection["tbx_PAcNote"];
                    string[] PAcDisabled = formCollection["hid_PAcDisabled"];

                    int countPA = PAcMaterialID.Length;

                    for (int i = 0; i < countPA; i++)
                    {
                        if (PAcDisabled[i] == "0")
                        {
                            TbOneSrdetailPartsReplace beanD = new TbOneSrdetailPartsReplace();

                            beanD.CSrid = pSRID;
                            beanD.CXchp = PAcXCHP[i];
                            beanD.CMaterialId = PAcMaterialID[i];
                            beanD.CMaterialName = PAcMaterialName[i];
                            beanD.COldCt = PAcOldCT[i];
                            beanD.CNewCt = PAcNewCT[i];
                            beanD.CHpct = PAcHPCT[i];
                            beanD.CNewUefi = PAcNewUEFI[i];
                            beanD.CStandbySerialId = PAcStandbySerialID[i];
                            beanD.CHpcaseId = PAcHPCaseID[i];

                            if (PAcArriveDate[i] != "")
                            {
                                beanD.CArriveDate = Convert.ToDateTime(PAcArriveDate[i]);
                            }

                            if (PAcReturnDate[i] != "")
                            {
                                beanD.CReturnDate = Convert.ToDateTime(PAcReturnDate[i]);
                            }

                            beanD.CPersonalDamage = PAcPersonalDamage[i];
                            beanD.CNote = PAcNote[i];
                            beanD.Disabled = int.Parse(PAcDisabled[i]);

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailPartsReplaces.Add(beanD);
                        }
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑零件更換資訊 ↑↑↑↑↑-----

                    int result = dbOne.SaveChanges();

                    if (result <= 0)
                    {
                        pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "提交失敗(編輯)" + Environment.NewLine;
                        CMF.writeToLog(pSRID, "SaveGenerallySR", pMsg, LoginUser_Name);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(tLog))
                        {
                            #region 紀錄修改log
                            TbOneLog logBean = new TbOneLog
                            {
                                CSrid = pSRID,
                                EventName = "SaveGenerallySR",
                                Log = tLog,
                                CreatedUserName = LoginUser_Name,
                                CreatedDate = DateTime.Now
                            };

                            dbOne.TbOneLogs.Add(logBean);
                            dbOne.SaveChanges();
                            #endregion
                        }

                        #region call ONE SERVICE 服務案件(一般/裝機/定維)狀態更新接口來寄送Mail
                        string TempStatus = CStatus;

                        if (OldCMainEngineerId != CMainEngineerId)
                        {
                            TempStatus = CStatus + "|TRANS"; //轉單
                        }

                        beanIN.IV_LOGINEMPNO = EmpBean.EmployeeERPID;
                        beanIN.IV_LOGINEMPNAME = LoginUser_Name;
                        beanIN.IV_SRID = pSRID;
                        beanIN.IV_STATUS = TempStatus;
                        beanIN.IV_APIURLName = tAPIURLName;

                        CMF.GetAPI_SRSTATUS_Update(beanIN);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "失敗原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();
                
                CMF.writeToLog(pSRID, "SaveGenerallySR", pMsg, LoginUser_Name);
            }

            return RedirectToAction("finishForm");
        }
        #endregion

        #region 保固SLA資訊查詢結果
        /// <summary>
        /// 保固SLA資訊查詢結果
        /// </summary>
        /// <param name="ArySERIAL">序號(可多筆)</param>
        /// <returns></returns>
        public IActionResult QuerySRDetail_Warranty(string[] ArySERIAL)
        {
            getLoginAccount();

            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");

            bool tIsFormal = false;
            string BPMNO = string.Empty;
            string DNDATE = string.Empty;
            string SDATE = string.Empty;
            string EDATE = string.Empty;
            string tURL = string.Empty;
            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;
            string tInvoiceNo = string.Empty;
            string tInvoiceItem = string.Empty;

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;
            #endregion           

            DataTable dtWTY = null; //RFC保固Table

            int NowCount = 0;

            List<SRWarranty> QueryToList = new List<SRWarranty>();    //查詢出來的清單             

            try
            {
                #region 呼叫RFC並回傳保固SLA Table清單
                QueryToList = CMF.ZFM_TICC_SERIAL_SEARCHWTYList(ArySERIAL, ref NowCount, tBPMURLName, tPSIPURLName, tAPIURLName);
                #endregion

                #region 保固，因RFC已經有回傳所有清單，這邊暫時先不用
                //foreach (string IV_SERIAL in ArySERIAL)
                //{
                //    if (IV_SERIAL != null)
                //    {
                //        var beans = dbProxy.Stockwties.OrderByDescending(x => x.IvEdate).ThenByDescending(x => x.BpmNo).Where(x => x.IvSerial == IV_SERIAL.Trim());

                //        foreach (var bean in beans)
                //        {
                //            NowCount++;

                //            #region 組待查詢清單
                //            SRWarranty QueryInfo = new SRWarranty();

                //            //string[] tBPMList = CMF.findBPMWarrantyInfo(bean.BpmNo);

                //            DNDATE = bean.IvDndate == null ? "" : Convert.ToDateTime(bean.IvDndate).ToString("yyyy-MM-dd");
                //            SDATE = bean.IvSdate == null ? "" : Convert.ToDateTime(bean.IvSdate).ToString("yyyy-MM-dd");
                //            EDATE = bean.IvEdate == null ? "" : Convert.ToDateTime(bean.IvEdate).ToString("yyyy-MM-dd");

                //            #region 取得BPM Url
                //            tURL = "";

                //            if (bean.BpmNo != null)
                //            {
                //                if (bean.BpmNo.IndexOf("WTY") != -1)
                //                {
                //                    tURL = "http://" + tBPMURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Rwd/Warranty/WarrantyForm.aspx?FormNo=" + bean.BpmNo + " target=_blank";
                //                }
                //                else
                //                {
                //                    tURL = "http://" + tBPMURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Form/Guarantee/GuaranteeForm.aspx?FormNo=" + bean.BpmNo + " target=_blank";
                //                }
                //            }
                //            #endregion

                //            QueryInfo.cID = NowCount.ToString();                                        //系統ID
                //            QueryInfo.cSerialID = bean.IvSerial;                                         //序號                        
                //            QueryInfo.cWTYID = bean.IvWtyid;                                             //保固
                //            QueryInfo.cWTYName = bean.IvWtydesc;                                         //保固說明
                //            QueryInfo.cWTYSDATE = SDATE;                                                //保固開始日期
                //            QueryInfo.cWTYEDATE = EDATE;                                                //保固結束日期                                                          
                //            QueryInfo.cSLARESP = bean.IvSlaresp;                                         //回應條件
                //            QueryInfo.cSLASRV = bean.IvSlasrv;                                          //服務條件
                //            QueryInfo.cContractID = "";                                                 //合約編號                        
                //            QueryInfo.cBPMFormNo = string.IsNullOrEmpty(bean.BpmNo) ? "" : bean.BpmNo;      //BPM表單編號                        
                //            QueryInfo.cBPMFormNoUrl = tURL;                                             //BPM URL                    
                //            QueryInfo.cUsed = "N";                                                     //本次使用

                //            QueryToList.Add(QueryInfo);
                //            #endregion
                //        }
                //    }
                //}
                #endregion

                QueryToList = QueryToList.OrderBy(x => x.cSerialID).ThenByDescending(x => x.cWTYEDATE).ToList();
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "失敗原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();

                CMF.writeToLog("", "QuerySRDetail_Warranty", pMsg, EmpBean.EmployeeCName);
            }

            return Json(QueryToList);
        }
        #endregion

        #region 取得保固SLA明細Table(非新建或駁回狀態)
        /// <summary>
        /// 取得保固SLA明細Table(非新建或駁回狀態)
        /// </summary>        
        /// <param name="cSRID">SRID</param>
        /// <returns></returns>
        public IActionResult getSRDetail_Warranty(string cSRID)
        {
            getLoginAccount();

            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");

            bool tIsFormal = false;
            string tBPMURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tAttachURLName = string.Empty;

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;
            #endregion           

            int NowCount = 0;

            List<SRWarranty> QueryToList = new List<SRWarranty>();    //查詢出來的清單             

            try
            {                
                QueryToList = CMF.SEARCHWTYList(cSRID, ref NowCount, tBPMURLName, tPSIPURLName);
                QueryToList = QueryToList.OrderBy(x => x.cSerialID).ThenByDescending(x => x.cWTYEDATE).ToList();
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "失敗原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();
                
                CMF.writeToLog(cSRID, "getSRDetail_Warranty", pMsg, EmpBean.EmployeeCName);
            }

            return Json(QueryToList);
        }
        #endregion

        #region 儲存處理與工時紀錄明細
        /// <summary>
        /// 儲存處理與工時紀錄明細
        /// </summary>
        /// <param name="prId">系統ID</param>
        /// <param name="cSRID">SRID</param>
        /// <param name="cEngineerID">服務工程師ERPID</param>
        /// <param name="cEngineerName">服務工程師姓名</param>
        /// <param name="cStartTime">出發時間</param>
        /// <param name="cReceiveTime">接單時間</param>
        /// <param name="cArriveTime">到場時間</param>
        /// <param name="cFinishTime">完成時間</param>
        /// <param name="cDesc">處理紀錄</param>
        /// <param name="cReport">服務報告書</param>
        /// <returns></returns>
        public ActionResult SavepjRecord(int? prId, string cSRID, string cEngineerID, string cEngineerName, string cStartTime, string cReceiveTime, string cArriveTime, string cFinishTime, string cDesc, string cReport)
        {
            getLoginAccount();

            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);
            
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");

            bool reValue = false;

            try
            {
                int result = 0;
                if (prId == null) //新增
                {
                    #region -- 儲存處理與工時紀錄明細 --
                    TbOneSrdetailRecord prBean = new TbOneSrdetailRecord();

                    prBean.CSrid = cSRID;
                    prBean.CEngineerId = cEngineerID;
                    prBean.CEngineerName = cEngineerName;
                    prBean.CStartTime = Convert.ToDateTime(cStartTime);
                    prBean.CReceiveTime = Convert.ToDateTime(cReceiveTime);
                    prBean.CArriveTime = Convert.ToDateTime(cArriveTime);
                    prBean.CFinishTime = Convert.ToDateTime(cFinishTime);
                    prBean.CDesc = cDesc;
                    prBean.CSrreport = string.IsNullOrEmpty(cReport) ? "" : cReport;
                    prBean.Disabled = 0;
                    prBean.CreatedUserName = ViewBag.empEngName;
                    prBean.CreatedDate = DateTime.Now;

                    dbOne.TbOneSrdetailRecords.Add(prBean);
                    result = dbOne.SaveChanges();
                    #endregion
                }
                else //編輯
                {
                    #region -- 編輯處理與工時紀錄明細 --
                    var prBean = dbOne.TbOneSrdetailRecords.FirstOrDefault(x => x.CId == prId);
                    if (prBean != null)
                    {                        
                        prBean.CStartTime = Convert.ToDateTime(cStartTime);
                        prBean.CReceiveTime = Convert.ToDateTime(cReceiveTime);
                        prBean.CArriveTime = Convert.ToDateTime(cArriveTime);
                        prBean.CFinishTime = Convert.ToDateTime(cFinishTime);
                        prBean.CDesc = cDesc;
                        prBean.CSrreport = string.IsNullOrEmpty(cReport) ? "" : cReport;
                        prBean.Disabled = 1;

                        prBean.ModifiedUserName = ViewBag.empEngName;
                        prBean.ModifiedDate = DateTime.Now;
                        
                        result = dbOne.SaveChanges();                        
                    }
                    #endregion
                }

                if (result > 0)
                {
                    reValue = true;
                }

                return Json(reValue);
            }
            catch (Exception e)
            {
                //SendMailByAPI("ONESERVICE處理與工時紀錄明細", null, "Elvis.Chang@etatung.com", "", "", "ONESERVICE處理與工時紀錄明細_錯誤", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "<br>prId: " + prId + "<br>" + e.ToString(), null, null);
                return Json(false);
            }
        }
        #endregion

        #region 取得系統參數清單
        /// <summary>
        /// 取得系統參數清單
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cFunctionID">功能別(OTHER.其他自定義)</param>
        /// <param name="cCompanyID">公司別</param>
        /// <param name="cNo">參數No</param>
        /// <param name="cEmptyOption">是否要產生「請選擇」選項(True.要 false.不要)</param>
        /// <returns></returns>
        static public List<SelectListItem> findSysParameterList(string cOperationID, string cFunctionID, string cCompanyID, string cNo, bool cEmptyOption)
        {
            CommonFunction CMF = new CommonFunction();

            var tList = CMF.findSysParameterListItem(cOperationID, cFunctionID, cCompanyID, cNo, cEmptyOption);

            return tList;
        }
        #endregion

        #region 取得系統參數清單(第一項為空白)
        /// <summary>
        /// 取得系統參數清單(第一項為空白)
        /// </summary>        
        /// <returns></returns>
        static public List<SelectListItem> findSysParameterList_WithEmpty(string cOperationID, string cFunctionID, string cCompanyID, string cNo, bool cEmptyOption)
        {
            CommonFunction CMF = new CommonFunction();

            var tList = new List<SelectListItem>();
            tList.Add(new SelectListItem { Text = "", Value = "" });

            var tListS = CMF.findSysParameterListItem(cOperationID, cFunctionID, cCompanyID, cNo, cEmptyOption);
            tList.AddRange(tListS);

            return tList;
        }
        #endregion

        #region 取得客戶類型
        /// <summary>
        /// 取得客戶類型
        /// </summary>        
        static public List<SelectListItem> findCustomerTypeList()
        {
            var tList = new List<SelectListItem>();

            tList.Add(new SelectListItem { Text = "", Value = "" });
            tList.Add(new SelectListItem { Text = "法人", Value = "C" });
            tList.Add(new SelectListItem { Text = "個人", Value = "P" });

            return tList;
        }
        #endregion

        #region 取得服務案件種類說明
        /// <summary>
        /// 取得服務案件種類說明
        /// </summary>        
        /// <param name="cEmptyOption">是否要產生「空白」選項(True.要 false.不要)</param>
        /// <returns></returns>
        static public List<SelectListItem> findSRIDTypeList(bool cEmptyOption)
        {
            var tList = new List<SelectListItem>();

            if (cEmptyOption)
            {
                tList.Add(new SelectListItem { Text = "", Value = "" });
            }

            tList.Add(new SelectListItem { Text = "ZSR1_一般服務", Value = "61" });
            tList.Add(new SelectListItem { Text = "ZSR3_裝機服務", Value = "63" });
            tList.Add(new SelectListItem { Text = "ZSR5_定維服務", Value = "65" });

            return tList;           
        }
        #endregion       

        #region Ajax傳入第一階(大類)並取得第二階(中類)清單
        /// <summary>
        /// Ajax傳入第一階(大類)並取得第二階(中類)清單
        /// </summary>
        /// <param name="keyword">第一階(大類)代碼</param>
        /// <returns></returns>
        public IActionResult findSRTypeSecList(string keyword)
        {
            var tList = new List<SelectListItem>();

            tList = CMF.findSRTypeSecList(keyword);

            ViewBag.SRTypeSecList = tList;
            return Json(tList);
        }
        #endregion

        #region Ajax傳入第二階(中類)並取得第三階(小類)清單
        /// <summary>
        /// Ajax傳入第二階(中類)並取得第三階(小類)清單
        /// </summary>
        /// <param name="keyword">第二階(中類)代碼</param>
        /// <returns></returns>
        public IActionResult findSRTypeThrList(string keyword)
        {
            var tList = new List<SelectListItem>();

            tList = CMF.findSRTypeThrList(keyword);

            ViewBag.SRTypeThrList = tList;
            return Json(tList);
        }
        #endregion

        #region 修改服務團隊
        /// <summary>
        /// 修改服務團隊
        /// </summary>
        /// <param name="cTeamID">目前的服務團隊ERPID(;號隔開)</param>
        /// <param name="cTeamAcc">欲修改的服務團隊ERPID</param>
        /// <returns></returns>
        public IActionResult SavepjTeam(string cTeamID, string cTeamAcc)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            pCompanyCode = EmpBean.BUKRS;
            #endregion

            string reValue = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(cTeamID))
                {
                    var oldTeamAcc = cTeamID;

                    if (oldTeamAcc.Contains(cTeamAcc))
                    {
                        reValue = "Error：服務團隊已存在，請重新輸入！";
                    }
                    else
                    {
                        reValue = oldTeamAcc + ";" + cTeamAcc;

                        #region 紀錄修改log
                        TbOneLog logBean = new TbOneLog
                        {
                            CSrid = string.IsNullOrEmpty(ViewBag.cSRID) ? "" : ViewBag.cSRID,
                            EventName = "SavepjTeam",
                            Log = "修改服務團隊_舊值: " + oldTeamAcc + "; 新值: " + reValue,
                            CreatedUserName = ViewBag.empEngName,
                            CreatedDate = DateTime.Now
                        };

                        dbOne.TbOneLogs.Add(logBean);
                        dbOne.SaveChanges();
                        #endregion
                    }
                }
                else
                {
                    reValue = cTeamAcc;
                }
            }
            catch (Exception e)
            {
                return Json("SavepjTeam Error：" + e.Message);
            }

            return Json(reValue);
        }
        #endregion

        #region 取得服務團隊
        /// <summary>
        /// 取得服務團隊
        /// </summary>
        /// <param name="cTeamID">服務團隊ERPID(;號隔開)</param>
        /// <returns></returns>
        public IActionResult GetpjTeam(string cTeamID)
        {
            List<SRTeamInfo> liSRTeamInfo = new List<SRTeamInfo>();

            string tEmpName = string.Empty;     //服務團隊姓名(中文姓名+英文姓名)            
            string tDeptID = string.Empty;      //對應該部門ID
            string tDeptName = string.Empty;    //對應該部門名稱

            if (!string.IsNullOrEmpty(cTeamID))
            {
                List<string> liAssAcc = cTeamID.Split(';').ToList();
                int pmId = 0;
                foreach (var AssAcc in liAssAcc)
                {
                    tEmpName = "";
                    tDeptID = "";
                    tDeptName = "";

                    pmId++;
                    if (string.IsNullOrEmpty(AssAcc)) continue;

                    var qPms = dbOne.TbOneSrteamMappings.OrderBy(x => x.CTeamNewId).Where(x => x.Disabled == 0 && x.CTeamOldId == AssAcc);
                    foreach(var qPm in qPms)
                    {
                        tEmpName = qPm.CTeamOldId + " " + qPm.CTeamOldName; //因為是同一個服務團隊，所以只取一個就行了
                        tDeptID += qPm.CTeamNewId + "</br>";
                        tDeptName += qPm.CTeamNewName + "</br>";
                    }

                    SRTeamInfo pmBean = new SRTeamInfo(pmId, AssAcc, tEmpName, tDeptID, tDeptName);
                    liSRTeamInfo.Add(pmBean);
                }
            }

            return Json(liSRTeamInfo);
        }
        #endregion

        #region 刪除服務團隊
        /// <summary>
        /// 刪除服務團隊
        /// </summary>
        /// <param name="cTeamID">目前的服務團隊ERPID(;號隔開)</param>
        /// <param name="cTeamAcc">欲刪除的服務團隊ERPID</param>
        /// <returns></returns>
        public IActionResult DeletepjTeam(string cTeamID, string cTeamAcc)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            pCompanyCode = EmpBean.BUKRS;
            #endregion

            string reValue = cTeamID;

            try
            {
                if (!string.IsNullOrEmpty(cTeamID))
                {
                    #region 刪除服務團隊，並回傳最新的服務團隊
                    var oldPMAcc = cTeamID;

                    List<string> liPmAcc = cTeamID.Split(';').ToList();
                    List<string> liPmAccNew = new List<string>();

                    foreach (string tValue in liPmAcc)
                    {
                        if (tValue.ToLower() != cTeamAcc.ToLower())
                        {
                            liPmAccNew.Add(tValue);
                        }
                    }

                    reValue = string.Join(";", liPmAccNew);
                    #endregion

                    #region 紀錄刪除log
                    TbOneLog logBean = new TbOneLog
                    {
                        CSrid = string.IsNullOrEmpty(ViewBag.cSRID) ? "" : ViewBag.cSRID,
                        EventName = "DeletepjTeam",
                        Log = "刪除服務團隊_舊值: " + cTeamID + "; 新值: " + reValue,
                        CreatedUserName = ViewBag.empEngName,
                        CreatedDate = DateTime.Now
                    };

                    dbOne.TbOneLogs.Add(logBean);
                    dbOne.SaveChanges();
                    #endregion
                }
            }
            catch (Exception e)
            {
                return Json("DeletepjTeam Error：" + e.Message);
            }

            return Json(reValue);
        }
        #endregion

        #region 修改協助工程師
        /// <summary>
        /// 修改協助工程師
        /// </summary>
        /// <param name="cAssEngineerID">目前的協助工程師ERPID(;號隔開)</param>
        /// <param name="cAssEngineerAcc">欲修改的協助工程師ERPID</param>
        /// <returns></returns>
        public IActionResult SavepjAssEngineer(string cAssEngineerID, string cAssEngineerAcc)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            pCompanyCode = EmpBean.BUKRS;
            #endregion

            string reValue = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(cAssEngineerID))
                {
                    var oldAssEngineerAcc = cAssEngineerID;

                    if (oldAssEngineerAcc.Contains(cAssEngineerAcc))
                    {
                        reValue = "Error：協助工程師已存在，請重新輸入！";
                    }
                    else
                    {
                        reValue = oldAssEngineerAcc + ";" + cAssEngineerAcc;

                        #region 紀錄修改log
                        TbOneLog logBean = new TbOneLog
                        {
                            CSrid = string.IsNullOrEmpty(ViewBag.cSRID) ? "": ViewBag.cSRID,
                            EventName = "SavepjAssEngineer",
                            Log = "修改協助工程師_舊值: " + oldAssEngineerAcc + "; 新值: " + reValue,
                            CreatedUserName = ViewBag.empEngName,
                            CreatedDate = DateTime.Now
                        };

                        dbOne.TbOneLogs.Add(logBean);
                        dbOne.SaveChanges();
                        #endregion
                    }
                }
                else
                {
                    reValue = cAssEngineerAcc;
                }
            }
            catch (Exception e)
            {
                return Json("SavepjAssEngineer Error：" + e.Message);
            }

            return Json(reValue);
        }
        #endregion

        #region 取得協助工程師
        /// <summary>
        /// 取得協助工程師
        /// </summary>
        /// <param name="cAssEngineerID">協助工程師ERPID(;號隔開)</param>
        /// <returns></returns>
        public IActionResult GetpjAssEngineer(string cAssEngineerID)
        {            
            List<AssEngineerInfo> liAssEngineerInfo = new List<AssEngineerInfo>();

            string tEmpName = string.Empty; //協助工程師姓名(中文姓名+英文姓名)

            if (!string.IsNullOrEmpty(cAssEngineerID))
            {
                List<string> liAssAcc = cAssEngineerID.Split(';').ToList();
                int pmId = 0;
                foreach (var AssAcc in liAssAcc)
                {
                    pmId++;
                    if (string.IsNullOrEmpty(AssAcc)) continue;

                    var qPm = dbEIP.People.FirstOrDefault(x => x.ErpId == AssAcc);
                    if (qPm != null)
                    {
                        tEmpName = qPm.Name2 + " " + qPm.Name;

                        var qPmDept = dbEIP.Departments.FirstOrDefault(x => x.Id == qPm.DeptId && x.Status == 0);
                        if (qPmDept == null)
                        {
                            AssEngineerInfo pmBean = new AssEngineerInfo(pmId, AssAcc, tEmpName, qPm.Extension, qPm.Mobile, qPm.Email, "", "");
                            liAssEngineerInfo.Add(pmBean);
                        }
                        else
                        {
                            AssEngineerInfo pmBean = new AssEngineerInfo(pmId, AssAcc, tEmpName, qPm.Extension, qPm.Mobile, qPm.Email, qPmDept.Id, qPmDept.Name);
                            liAssEngineerInfo.Add(pmBean);
                        }
                    }
                }
            }

            return Json(liAssEngineerInfo);
        }
        #endregion

        #region 刪除協助工程師
        /// <summary>
        /// 刪除協助工程師
        /// </summary>
        /// <param name="cAssEngineerID">目前的協助工程師ERPID(;號隔開)</param>
        /// <param name="cAssEngineerAcc">欲刪除的協助工程師ERPID</param>
        /// <returns></returns>
        public IActionResult DeletepjAssEngineer(string cAssEngineerID, string cAssEngineerAcc)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);
          
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            pCompanyCode = EmpBean.BUKRS;
            #endregion

            string reValue = cAssEngineerID;                

            try
            {                
                if (!string.IsNullOrEmpty(cAssEngineerID))
                {
                    #region 刪除工程師，並回傳最新的工程師
                    var oldPMAcc = cAssEngineerID;

                    List<string> liPmAcc = cAssEngineerID.Split(';').ToList();
                    List<string> liPmAccNew = new List<string>();

                    foreach (string tValue in liPmAcc)
                    {
                        if (tValue.ToLower() != cAssEngineerAcc)
                        {
                            liPmAccNew.Add(tValue);
                        }
                    }

                    reValue = string.Join(";", liPmAccNew);
                    #endregion

                    #region 紀錄刪除log
                    TbOneLog logBean = new TbOneLog
                    {
                        CSrid = string.IsNullOrEmpty(ViewBag.cSRID) ? "" : ViewBag.cSRID,
                        EventName = "DeletepjAssEngineer",
                        Log = "刪除協助工程師_舊值: " + cAssEngineerID + "; 新值: " + reValue,
                        CreatedUserName = ViewBag.empEngName,
                        CreatedDate = DateTime.Now
                    };

                    dbOne.TbOneLogs.Add(logBean);
                    dbOne.SaveChanges();
                    #endregion
                }
            }
            catch (Exception e)
            {                
                return Json("DeletepjAssEngineer Error：" + e.Message);
            }

            return Json(reValue);
        }
        #endregion

        #region 修改技術主管
        /// <summary>
        /// 修改技術主管
        /// </summary>
        /// <param name="cTechManagerID">目前的技術主管ERPID(;號隔開)</param>
        /// <param name="cTechManagerAcc">欲修改的技術主管ERPID</param>
        /// <returns></returns>
        public IActionResult SavepjTechManager(string cTechManagerID, string cTechManagerAcc)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            pCompanyCode = EmpBean.BUKRS;
            #endregion

            string reValue = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(cTechManagerID))
                {
                    var oldTechManagerAcc = cTechManagerID;

                    if (oldTechManagerAcc.Contains(cTechManagerAcc))
                    {
                        reValue = "Error：技術主管已存在，請重新輸入！";
                    }
                    else
                    {
                        reValue = oldTechManagerAcc + ";" + cTechManagerAcc;

                        #region 紀錄修改log
                        TbOneLog logBean = new TbOneLog
                        {
                            CSrid = string.IsNullOrEmpty(ViewBag.cSRID) ? "" : ViewBag.cSRID,
                            EventName = "SavepjTechManager",
                            Log = "修改技術主管_舊值: " + oldTechManagerAcc + "; 新值: " + reValue,
                            CreatedUserName = ViewBag.empEngName,
                            CreatedDate = DateTime.Now
                        };

                        dbOne.TbOneLogs.Add(logBean);
                        dbOne.SaveChanges();
                        #endregion
                    }
                }
                else
                {
                    reValue = cTechManagerAcc;
                }
            }
            catch (Exception e)
            {
                return Json("SavepjTechManager Error：" + e.Message);
            }

            return Json(reValue);
        }
        #endregion

        #region 取得技術主管
        /// <summary>
        /// 取得技術主管
        /// </summary>
        /// <param name="cTechManagerID">技術主管ERPID(;號隔開)</param>
        /// <returns></returns>
        public IActionResult GetpjTechManager(string cTechManagerID)
        {
            List<TechManagerInfo> liTechManagerInfo = new List<TechManagerInfo>();

            string tEmpName = string.Empty; //技術主管姓名(中文姓名+英文姓名)

            if (!string.IsNullOrEmpty(cTechManagerID))
            {
                List<string> liAssAcc = cTechManagerID.Split(';').ToList();
                int pmId = 0;
                foreach (var AssAcc in liAssAcc)
                {
                    pmId++;
                    if (string.IsNullOrEmpty(AssAcc)) continue;

                    var qPm = dbEIP.People.FirstOrDefault(x => x.ErpId == AssAcc);
                    if (qPm != null)
                    {
                        tEmpName = qPm.Name2 + " " + qPm.Name;

                        var qPmDept = dbEIP.Departments.FirstOrDefault(x => x.Id == qPm.DeptId && x.Status == 0);
                        if (qPmDept == null)
                        {
                            TechManagerInfo pmBean = new TechManagerInfo(pmId, AssAcc, tEmpName, qPm.Extension, qPm.Mobile, qPm.Email, "", "");
                            liTechManagerInfo.Add(pmBean);
                        }
                        else
                        {
                            TechManagerInfo pmBean = new TechManagerInfo(pmId, AssAcc, tEmpName, qPm.Extension, qPm.Mobile, qPm.Email, qPmDept.Id, qPmDept.Name);
                            liTechManagerInfo.Add(pmBean);
                        }
                    }
                }
            }

            return Json(liTechManagerInfo);
        }
        #endregion

        #region 刪除技術主管
        /// <summary>
        /// 刪除技術主管
        /// </summary>
        /// <param name="cTechManagerID">目前的技術主管ERPID(;號隔開)</param>
        /// <param name="cTechManagerAcc">欲刪除的技術主管ERPID</param>
        /// <returns></returns>
        public IActionResult DeletepjTechManager(string cTechManagerID, string cTechManagerAcc)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            pCompanyCode = EmpBean.BUKRS;
            #endregion

            string reValue = cTechManagerID;

            try
            {
                if (!string.IsNullOrEmpty(cTechManagerID))
                {
                    #region 刪除工程師，並回傳最新的工程師
                    var oldPMAcc = cTechManagerID;

                    List<string> liPmAcc = cTechManagerID.Split(';').ToList();
                    List<string> liPmAccNew = new List<string>();

                    foreach (string tValue in liPmAcc)
                    {
                        if (tValue.ToLower() != cTechManagerAcc)
                        {
                            liPmAccNew.Add(tValue);
                        }
                    }

                    reValue = string.Join(";", liPmAccNew);
                    #endregion

                    #region 紀錄刪除log
                    TbOneLog logBean = new TbOneLog
                    {
                        CSrid = string.IsNullOrEmpty(ViewBag.cSRID) ? "" : ViewBag.cSRID,
                        EventName = "DeletepjTechManager",
                        Log = "刪除技術主管_舊值: " + cTechManagerID + "; 新值: " + reValue,
                        CreatedUserName = ViewBag.empEngName,
                        CreatedDate = DateTime.Now
                    };

                    dbOne.TbOneLogs.Add(logBean);
                    dbOne.SaveChanges();
                    #endregion
                }
            }
            catch (Exception e)
            {
                return Json("DeletepjTechManager Error：" + e.Message);
            }

            return Json(reValue);
        }
        #endregion

        #region 刪除聯絡人
        /// <summary>
        /// 刪除聯絡人
        /// </summary>        
        /// <param name="cContactID">客戶聯絡人GUID</param>
        /// <returns></returns>
        public IActionResult DeletepjContact(string cContactID)
        {
            string reValue = string.Empty;
                
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            pCompanyCode = EmpBean.BUKRS;
            #endregion            

            try
            {
                #region 刪除聯絡人
                var bean = dbProxy.CustomerContacts.FirstOrDefault(x => x.ContactId.ToString() == cContactID);

                if (bean != null)
                {
                    bean.Disabled = 1;
                    dbProxy.SaveChanges();
                }
                #endregion

                #region 紀錄刪除log
                TbOneLog logBean = new TbOneLog
                {
                    CSrid = string.IsNullOrEmpty(ViewBag.cSRID) ? "" : ViewBag.cSRID,
                    EventName = "DeletepjContact",
                    Log = "刪除聯絡人_舊值: " + cContactID + "; 新值: " + cContactID,
                    CreatedUserName = ViewBag.empEngName,
                    CreatedDate = DateTime.Now
                };

                dbOne.TbOneLogs.Add(logBean);
                dbOne.SaveChanges();
                #endregion
            }
            catch (Exception e)
            {
                return Json("DeletepjContact Error：" + e.Message);
            }

            return Json(reValue);
        }
        #endregion

        #endregion -----↑↑↑↑↑一般服務 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓裝機服務 ↓↓↓↓↓-----

        #region 裝機服務index
        public IActionResult InstallSR()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            ViewBag.cLoginUser_EmployeeNO = EmpBean.EmployeeNO;
            ViewBag.cLoginUser_ERPID = EmpBean.EmployeeERPID;
            ViewBag.cLoginUser_WorkPlace = EmpBean.WorkPlace;
            ViewBag.cLoginUser_DepartmentName = EmpBean.DepartmentName;
            ViewBag.cLoginUser_DepartmentNO = EmpBean.DepartmentNO;
            ViewBag.cLoginUser_ProfitCenterID = EmpBean.ProfitCenterID;
            ViewBag.cLoginUser_CostCenterID = EmpBean.CostCenterID;
            ViewBag.cLoginUser_CompCode = EmpBean.CompanyCode;
            ViewBag.cLoginUser_BUKRS = EmpBean.BUKRS;
            ViewBag.cLoginUser_IsManager = EmpBean.IsManager;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");

            pCompanyCode = EmpBean.BUKRS;
            #endregion

            var model = new ViewModel_Install();

            #region Request參數            
            if (HttpContext.Request.Query["SRID"].FirstOrDefault() != null)
            {
                pSRID = HttpContext.Request.Query["SRID"].FirstOrDefault();
            }
            #endregion

            #region 報修類別
            //大類
            var SRTypeOneList = CMF.findFirstKINDList();

            //中類
            var SRTypeSecList = new List<SelectListItem>();
            SRTypeSecList.Add(new SelectListItem { Text = " ", Value = "" });

            //小類
            var SRTypeThrList = new List<SelectListItem>();
            SRTypeThrList.Add(new SelectListItem { Text = " ", Value = "" });
            #endregion

            #region 取得服務團隊清單
            var SRTeamIDList = CMF.findSRTeamIDList(pCompanyCode, true);
            #endregion

            #region 取得SRID
            var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pSRID);

            if (beanM != null)
            {
                //記錄目前GUID，用來判斷更新的先後順序
                ViewBag.pGUID = beanM.CSystemGuid.ToString();

                //判斷登入者是否可以編輯服務案件
                pIsCanEditSR = CMF.checkIsCanEditSR(beanM.CSrid, EmpBean.EmployeeERPID, pIsMIS, pIsCS);

                #region 裝機資訊
                ViewBag.cSRID = beanM.CSrid;
                ViewBag.cCustomerID = beanM.CCustomerId;
                ViewBag.cCustomerName = beanM.CCustomerName;
                ViewBag.cDesc = beanM.CDesc;
                ViewBag.cNotes = beanM.CNotes;
                ViewBag.cAttachement = beanM.CAttachement;
                ViewBag.cAttachementStockNo = beanM.CAttachementStockNo;                
                ViewBag.cDelayReason = beanM.CDelayReason;
                ViewBag.cSalesNo = beanM.CSalesNo;
                ViewBag.cShipmentNo = beanM.CShipmentNo;
                ViewBag.pStatus = beanM.CStatus;

                ViewBag.cCustomerType = beanM.CCustomerId.Substring(0, 1) == "P" ? "P" : "C";
                #endregion                

                #region 報修類別
                if (!string.IsNullOrEmpty(beanM.CSrtypeOne))
                {
                    SRTypeOneList.Where(q => q.Value == beanM.CSrtypeOne).First().Selected = true;
                }

                if (!string.IsNullOrEmpty(beanM.CSrtypeSec))
                {
                    SRTypeSecList = CMF.findSRTypeSecList(beanM.CSrtypeOne);
                    SRTypeSecList.Where(q => q.Value == beanM.CSrtypeSec).First().Selected = true;
                }

                if (!string.IsNullOrEmpty(beanM.CSrtypeThr))
                {
                    SRTypeThrList = CMF.findSRTypeThrList(beanM.CSrtypeSec);
                    SRTypeThrList.Where(q => q.Value == beanM.CSrtypeThr).First().Selected = true;
                }
                #endregion              

                #region 服務團隊
                ViewBag.cTeamID = beanM.CTeamId;
                ViewBag.cMainEngineerID = beanM.CMainEngineerId;
                ViewBag.cMainEngineerName = beanM.CMainEngineerName;
                ViewBag.cAssEngineerID = beanM.CAssEngineerId;
                ViewBag.cSalesID = beanM.CSalesId;
                ViewBag.cSalesName = beanM.CSalesName;
                ViewBag.cSecretaryID = beanM.CSecretaryId;
                ViewBag.cSecretaryName = beanM.CSecretaryName;                
                #endregion

                ViewBag.CreatedDate = Convert.ToDateTime(beanM.CreatedDate).ToString("yyyy-MM-dd");

                #region 取得客戶聯絡人資訊(明細)
                var beansContact = dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                ViewBag.Detailbean_Contact = beansContact;
                ViewBag.trContactNo = beansContact.Count();
                #endregion

                #region 取得物料訊息檔資訊(明細)
                var beansMaterial = dbOne.TbOneSrdetailMaterialInfos.OrderBy(x => x.CId).Where(x => x.Disabled == 0 && x.CSrid == pSRID);                

                ViewBag.Detailbean_Material = beansMaterial;
                ViewBag.trMaterialNo = beansMaterial.Count();
                #endregion

                #region 取得序號回報檔資訊(明細)
                var beansSerial = dbOne.TbOneSrdetailSerialFeedbacks.OrderBy(x => x.CId).Where(x => x.Disabled == 0 && x.CSrid == pSRID);                

                ViewBag.Detailbean_Serial = beansSerial;
                ViewBag.trSerialNo = beansSerial.Count();
                #endregion

                #region 取得工時紀錄檔資訊(明細)
                var beansRecord = dbOne.TbOneSrdetailRecords.OrderBy(x => x.CId).ThenByDescending(x => x.CFinishTime).Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                ViewBag.Detailbean_Record = beansRecord;
                ViewBag.trRecordNo = beansRecord.Count();
                #endregion               
            }
            else
            {
                ViewBag.cSRID = "";                
                ViewBag.pStatus = "E0001";      //新建
                ViewBag.cDelayReason = "";      //空值
                ViewBag.cSalesNo = "";          //空值
                ViewBag.cShipmentNo = "";       //空值
            }
            #endregion

            #region 指派Option值
            model.ddl_cStatus = ViewBag.pStatus;                //設定狀態            
            model.ddl_cCustomerType = ViewBag.cCustomerType;     //客戶類型(P.個人 C.法人)
            #endregion

            ViewBag.SRTypeOneList = SRTypeOneList;
            ViewBag.SRTypeSecList = SRTypeSecList;
            ViewBag.SRTypeThrList = SRTypeThrList;
            ViewBag.SRTeamIDList = SRTeamIDList;

            ViewBag.pOperationID = pOperationID_InstallSR;
            ViewBag.pIsCanEditSR = pIsCanEditSR.ToString();  //登入者是否可以編輯服務案件

            return View(model);
        }
        #endregion

        #region 儲存裝機服務
        /// <summary>
        /// 儲存裝機服務
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveInstallSR(IFormCollection formCollection)
        {
            getLoginAccount();

            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            pLoginName = EmpBean.EmployeeCName;

            pSRID = formCollection["hid_cSRID"].FirstOrDefault();

            bool tIsFormal = false;
            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;
            string tLog = string.Empty;

            string OldCStatus = string.Empty;
            string OldCCustomerName = string.Empty;
            string OldCCustomerId = string.Empty;
            string OldCDesc = string.Empty;
            string OldCNotes = string.Empty;
            string OldCAttachement = string.Empty;
            string OldCAttachementStockNo = string.Empty;            
            string OldCSrtypeOne = string.Empty;
            string OldCSrtypeSec = string.Empty;
            string OldCSrtypeThr = string.Empty;
            string OldCSalesNo = string.Empty;
            string OldCShipmentNo = string.Empty;
            string OldCDelayReason = string.Empty;            
            string OldCTeamId = string.Empty;
            string OldCMainEngineerName = string.Empty;
            string OldCMainEngineerId = string.Empty;
            string OldCAssEngineerId = string.Empty;
            string OldCSalesName = string.Empty;
            string OldCSalesId = string.Empty;
            string OldCSecretaryName = string.Empty;
            string OldCSecretaryId = string.Empty;                     

            string CStatus = formCollection["ddl_cStatus"].FirstOrDefault();
            string CCustomerName = formCollection["tbx_cCustomerName"].FirstOrDefault();
            string CCustomerId = formCollection["hid_cCustomerID"].FirstOrDefault();
            string CDesc = formCollection["tbx_cDesc"].FirstOrDefault();
            string CNotes = formCollection["tbx_cNotes"].FirstOrDefault();
            string CAttach = formCollection["hid_filezone_1"].FirstOrDefault();
            string CAttachStockNo = formCollection["hid_filezone_2"].FirstOrDefault();            
            string CSrtypeOne = formCollection["ddl_cSRTypeOne"].FirstOrDefault();
            string CSrtypeSec = formCollection["ddl_cSRTypeSec"].FirstOrDefault();
            string CSrtypeThr = formCollection["ddl_cSRTypeThr"].FirstOrDefault();
            string CSalesNo = formCollection["tbx_cSalesNo"].FirstOrDefault();
            string CShipmentNo = formCollection["tbx_cShipmentNo"].FirstOrDefault();
            string CDelayReason = formCollection["tbx_cDelayReason"].FirstOrDefault();
            
            string CTeamId = formCollection["hid_cTeamID"].FirstOrDefault();
            string CMainEngineerName = formCollection["tbx_cMainEngineerName"].FirstOrDefault();
            string CMainEngineerId = formCollection["hid_cMainEngineerID"].FirstOrDefault();
            string CAssEngineerId = formCollection["hid_cAssEngineerID"].FirstOrDefault();
            string CSalesName = formCollection["tbx_cSalesName"].FirstOrDefault();
            string CSalesId = formCollection["hid_cSalesID"].FirstOrDefault();
            string CSecretaryName = formCollection["tbx_cSecretaryName"].FirstOrDefault();
            string CSecretaryId = formCollection["hid_cSecretaryID"].FirstOrDefault();
            string LoginUser_Name = formCollection["hid_cLoginUser_Name"].FirstOrDefault();

            SRCondition srCon = new SRCondition();
            SRMain_SRSTATUS_INPUT beanIN = new SRMain_SRSTATUS_INPUT();

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

                var beanNowM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pSRID);

                if (beanNowM == null)
                {
                    #region 新增主檔
                    TbOneSrmain beanM = new TbOneSrmain();

                    srCon = SRCondition.ADD;

                    //主表資料
                    beanM.CSrid = pSRID;
                    beanM.CStatus = CStatus;
                    beanM.CCustomerName = CCustomerName;
                    beanM.CCustomerId = CCustomerId;
                    beanM.CDesc = CDesc;
                    beanM.CNotes = CNotes;
                    beanM.CAttachement = CAttach;
                    beanM.CAttachementStockNo = CAttachStockNo;
                    beanM.CSrtypeOne = CSrtypeOne;
                    beanM.CSrtypeSec = CSrtypeSec;
                    beanM.CSrtypeThr = CSrtypeThr;
                    beanM.CSalesNo = CSalesNo;
                    beanM.CShipmentNo = CShipmentNo;
                    beanM.CDelayReason = CDelayReason;                    
                    
                    beanM.CTeamId = CTeamId;
                    beanM.CMainEngineerName = CMainEngineerName;
                    beanM.CMainEngineerId = CMainEngineerId;
                    beanM.CAssEngineerId = CAssEngineerId;                    
                    beanM.CSalesName = CSalesName;
                    beanM.CSalesId = CSalesId;
                    beanM.CSecretaryName = CSecretaryName;
                    beanM.CSecretaryId = CSecretaryId;

                    beanM.CSystemGuid = Guid.NewGuid();
                    beanM.CIsAppclose = "";
                    beanM.CreatedDate = DateTime.Now;
                    beanM.CreatedUserName = LoginUser_Name;

                    #region 未用到的欄位
                    beanM.CRepairName = "";
                    beanM.CRepairAddress = "";
                    beanM.CRepairPhone = "";
                    beanM.CRepairMobile = "";
                    beanM.CRepairEmail = "";
                    beanM.CMaserviceType = "";
                    beanM.CSrpathWay = "";
                    beanM.CSrprocessWay = "";
                    beanM.CIsSecondFix = "";
                    beanM.CTechManagerId = "";
                    beanM.CSqpersonId = "";
                    beanM.CSqpersonName = "";
                    beanM.CIsAppclose = "";
                    #endregion

                    dbOne.TbOneSrmains.Add(beanM);
                    #endregion

                    #region 新增【客戶聯絡窗口資訊】明細
                    string[] COcContactName = formCollection["tbx_COcContactName"];
                    string[] COcContactAddress = formCollection["tbx_COcContactAddress"];
                    string[] COcContactPhone = formCollection["tbx_COcContactPhone"];
                    string[] COcContactMobile = formCollection["tbx_COcContactMobile"];
                    string[] COcContactEmail = formCollection["tbx_COcContactEmail"];
                    string[] COcDisabled = formCollection["hid_COcDisabled"];

                    int countCO = COcContactName.Length;

                    for (int i = 0; i < countCO; i++)
                    {
                        if (COcDisabled[i] == "0")
                        {
                            TbOneSrdetailContact beanD = new TbOneSrdetailContact();

                            beanD.CSrid = pSRID;
                            beanD.CContactName = COcContactName[i];
                            beanD.CContactAddress = COcContactAddress[i];
                            beanD.CContactPhone = COcContactPhone[i];
                            beanD.CContactMobile = COcContactMobile[i];
                            beanD.CContactEmail = COcContactEmail[i];
                            beanD.Disabled = 0;

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailContacts.Add(beanD);
                        }
                    }
                    #endregion

                    #region 新增【物料訊息資訊】明細                    
                    string[] MIcMaterialID = formCollection["tbx_MIcMaterialID"];
                    string[] MIcMaterialName = formCollection["tbx_MIcMaterialName"];
                    string[] MIcQuantity = formCollection["tbx_MIcQuantity"];
                    string[] MIcBasicContent = formCollection["tbx_MIcBasicContent"];
                    string[] MIcMFPNumber = formCollection["tbx_MIcMFPNumber"];
                    string[] MIcBrand = formCollection["tbx_MIcBrand"];
                    string[] MIcProductHierarchy = formCollection["tbx_MIcProductHierarchy"];
                    string[] MIcDisabled = formCollection["hid_MIcDisabled"];

                    int countMI = MIcMaterialID.Length;

                    for (int i = 0; i < countMI; i++)
                    {
                        if (MIcDisabled[i] == "0")
                        {
                            TbOneSrdetailMaterialInfo beanD = new TbOneSrdetailMaterialInfo();

                            beanD.CSrid = pSRID;
                            beanD.CMaterialId = MIcMaterialID[i];
                            beanD.CMaterialName = MIcMaterialName[i];
                            beanD.CQuantity = int.Parse(MIcQuantity[i]);
                            beanD.CBasicContent = MIcBasicContent[i];
                            beanD.CMfpnumber = MIcMFPNumber[i];
                            beanD.CBrand = MIcBrand[i];
                            beanD.CProductHierarchy = MIcProductHierarchy[i];
                            beanD.Disabled = 0;

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailMaterialInfos.Add(beanD);
                        }
                    }
                    #endregion

                    #region 新增【序號回報資訊】明細
                    string[] SFcSerialID = formCollection["tbx_SFcSerialID"];
                    string[] SFcMaterialID = formCollection["tbx_SFcMaterialID"];
                    string[] SFcMaterialName = formCollection["tbx_SFcMaterialName"];
                    string[] SFcConfigReport = formCollection["hid_filezoneSF"];
                    string[] SFcDisabled = formCollection["hid_SFcDisabled"];

                    int countSF = SFcSerialID.Length;

                    for (int i = 0; i < countSF; i++)
                    {
                        if (SFcDisabled[i] == "0")
                        {
                            TbOneSrdetailSerialFeedback beanD = new TbOneSrdetailSerialFeedback();

                            beanD.CSrid = pSRID;
                            beanD.CSerialId = SFcSerialID[i];
                            beanD.CMaterialId = SFcMaterialID[i];
                            beanD.CMaterialName = SFcMaterialName[i];
                            beanD.CConfigReport = SFcConfigReport[i];
                            beanD.Disabled = 0;

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailSerialFeedbacks.Add(beanD);
                        }
                    }
                    #endregion                    

                    int result = dbOne.SaveChanges();

                    if (result <= 0)
                    {
                        pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "提交失敗(新建)" + Environment.NewLine;
                        CMF.writeToLog(pSRID, "SaveInstallSR", pMsg, LoginUser_Name);
                    }
                    else
                    {
                        #region 紀錄修改log
                        TbOneLog logBean = new TbOneLog
                        {
                            CSrid = pSRID,
                            EventName = "SaveInstallSR",
                            Log = "新增成功！",
                            CreatedUserName = LoginUser_Name,
                            CreatedDate = DateTime.Now
                        };

                        dbOne.TbOneLogs.Add(logBean);
                        dbOne.SaveChanges();
                        #endregion

                        #region call ONE SERVICE 服務案件(一般/裝機/定維)狀態更新接口來寄送Mail
                        beanIN.IV_LOGINEMPNO = EmpBean.EmployeeERPID;
                        beanIN.IV_LOGINEMPNAME = LoginUser_Name;
                        beanIN.IV_SRID = pSRID;
                        beanIN.IV_STATUS = "E0001"; //新建
                        beanIN.IV_APIURLName = tAPIURLName;

                        CMF.GetAPI_SRSTATUS_Update(beanIN);
                        #endregion
                    }
                }
                else
                {
                    #region 修改主檔

                    #region 紀錄舊值
                    OldCStatus = beanNowM.CStatus;
                    tLog += CMF.getNewAndOldLog("狀態", OldCStatus, CStatus);

                    OldCCustomerName = beanNowM.CCustomerName;
                    tLog += CMF.getNewAndOldLog("客戶名稱", OldCCustomerName, CCustomerName);

                    OldCCustomerId = beanNowM.CCustomerId;
                    tLog += CMF.getNewAndOldLog("客戶ID", OldCCustomerId, CCustomerId);

                    OldCDesc = beanNowM.CDesc;
                    tLog += CMF.getNewAndOldLog("說明", OldCDesc, CDesc);

                    OldCNotes = beanNowM.CNotes;
                    tLog += CMF.getNewAndOldLog("詳細描述", OldCNotes, CNotes);

                    OldCAttachement = beanNowM.CAttachement;
                    tLog += CMF.getNewAndOldLog("檢附文件", OldCAttachement, CAttach);

                    OldCAttachementStockNo = beanNowM.CAttachementStockNo;
                    tLog += CMF.getNewAndOldLog("備料服務通知單文件", OldCAttachementStockNo, CAttachStockNo);

                    OldCSrtypeOne = beanNowM.CSrtypeOne;
                    tLog += CMF.getNewAndOldLog("報修類別(大類)", OldCSrtypeOne, CSrtypeOne);

                    OldCSrtypeSec = beanNowM.CSrtypeSec;
                    tLog += CMF.getNewAndOldLog("報修類別(中類)", OldCSrtypeSec, CSrtypeSec);

                    OldCSrtypeThr = beanNowM.CSrtypeThr;
                    tLog += CMF.getNewAndOldLog("報修類別(中類)", OldCSrtypeThr, CSrtypeThr);

                    OldCSalesNo = beanNowM.CSalesNo;
                    tLog += CMF.getNewAndOldLog("銷售訂單號", OldCSalesNo, CSalesNo);

                    OldCShipmentNo = beanNowM.CShipmentNo;
                    tLog += CMF.getNewAndOldLog("出貨單號", OldCShipmentNo, CShipmentNo);

                    OldCDelayReason = beanNowM.CDelayReason;
                    tLog += CMF.getNewAndOldLog("延遲結案原因", OldCDelayReason, CDelayReason);                    

                    OldCTeamId = beanNowM.CTeamId;
                    tLog += CMF.getNewAndOldLog("服務團隊", OldCTeamId, CTeamId);

                    OldCMainEngineerName = beanNowM.CMainEngineerName;
                    tLog += CMF.getNewAndOldLog("主要工程師姓名", OldCMainEngineerName, CMainEngineerName);

                    OldCMainEngineerId = beanNowM.CMainEngineerId;
                    tLog += CMF.getNewAndOldLog("主要工程師ERPID", OldCMainEngineerId, CMainEngineerId);

                    OldCAssEngineerId = beanNowM.CAssEngineerId;
                    tLog += CMF.getNewAndOldLog("協助工程師ERPID", OldCAssEngineerId, CAssEngineerId);

                    OldCSalesName = beanNowM.CSalesName;
                    tLog += CMF.getNewAndOldLog("業務人員", OldCSalesName, CSalesName);

                    OldCSalesId = beanNowM.CSalesId;
                    tLog += CMF.getNewAndOldLog("業務人員ERPID", OldCSalesId, CSalesId);

                    OldCSalesName = beanNowM.CSalesName;
                    tLog += CMF.getNewAndOldLog("業務祕書", OldCSalesName, CSalesName);

                    OldCSalesId = beanNowM.CSalesId;
                    tLog += CMF.getNewAndOldLog("業務祕書ERPID", OldCSalesId, CSalesId);

                    #endregion

                    //主表資料
                    beanNowM.CStatus = CStatus;
                    beanNowM.CCustomerName = CCustomerName;
                    beanNowM.CCustomerId = CCustomerId;
                    beanNowM.CDesc = CDesc;
                    beanNowM.CNotes = CNotes;
                    beanNowM.CAttachement = CAttach;
                    beanNowM.CAttachementStockNo = CAttachStockNo;
                    beanNowM.CSrtypeOne = CSrtypeOne;
                    beanNowM.CSrtypeSec = CSrtypeSec;
                    beanNowM.CSrtypeThr = CSrtypeThr;
                    beanNowM.CSalesNo = CSalesNo;
                    beanNowM.CShipmentNo = CShipmentNo;
                    beanNowM.CDelayReason = CDelayReason;
                    
                    beanNowM.CTeamId = CTeamId;
                    beanNowM.CMainEngineerName = CMainEngineerName;
                    beanNowM.CMainEngineerId = CMainEngineerId;
                    beanNowM.CAssEngineerId = CAssEngineerId;
                    beanNowM.CSalesName = CSalesName;
                    beanNowM.CSalesId = CSalesId;
                    beanNowM.CSecretaryName = CSecretaryName;
                    beanNowM.CSecretaryId = CSecretaryId;
                    beanNowM.CSystemGuid = Guid.NewGuid();

                    if (CStatus == "E0010") //裝機完成
                    {
                        beanNowM.CIsAppclose = "N";
                    }

                    beanNowM.ModifiedDate = DateTime.Now;
                    beanNowM.ModifiedUserName = LoginUser_Name;
                    #endregion

                    #region -----↓↓↓↓↓客戶聯絡窗口資訊↓↓↓↓↓-----

                    #region 刪除明細
                    var beansDCon = dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    foreach (var beanDCon in beansDCon)
                    {
                        beanDCon.Disabled = 1;
                        beanDCon.ModifiedDate = DateTime.Now;
                        beanDCon.ModifiedUserName = LoginUser_Name;
                    }
                    #endregion

                    #region 新增【客戶聯絡窗口資訊】明細
                    string[] COcContactName = formCollection["tbx_COcContactName"];
                    string[] COcContactAddress = formCollection["tbx_COcContactAddress"];
                    string[] COcContactPhone = formCollection["tbx_COcContactPhone"];
                    string[] COcContactMobile = formCollection["tbx_COcContactMobile"];
                    string[] COcContactEmail = formCollection["tbx_COcContactEmail"];
                    string[] COcDisabled = formCollection["hid_COcDisabled"];

                    int countCO = COcContactName.Length;

                    for (int i = 0; i < countCO; i++)
                    {
                        if (COcDisabled[i] == "0")
                        {
                            TbOneSrdetailContact beanD = new TbOneSrdetailContact();

                            beanD.CSrid = pSRID;
                            beanD.CContactName = COcContactName[i];
                            beanD.CContactAddress = COcContactAddress[i];
                            beanD.CContactPhone = COcContactPhone[i];
                            beanD.CContactMobile = COcContactMobile[i];
                            beanD.CContactEmail = COcContactEmail[i];
                            beanD.Disabled = int.Parse(COcDisabled[i]);

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailContacts.Add(beanD);
                        }
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑客戶聯絡窗口資訊 ↑↑↑↑↑-----

                    #region -----↓↓↓↓↓物料訊息資訊↓↓↓↓↓-----

                    #region 刪除明細
                    var beansDMI = dbOne.TbOneSrdetailMaterialInfos.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    foreach (var beanDMI in beansDMI)
                    {
                        beanDMI.Disabled = 1;
                        beanDMI.ModifiedDate = DateTime.Now;
                        beanDMI.ModifiedUserName = LoginUser_Name;
                    }
                    #endregion

                    #region 新增明細
                    string[] MIcMaterialID = formCollection["tbx_MIcMaterialID"];
                    string[] MIcMaterialName = formCollection["tbx_MIcMaterialName"];
                    string[] MIcQuantity = formCollection["tbx_MIcQuantity"];
                    string[] MIcBasicContent = formCollection["tbx_MIcBasicContent"];
                    string[] MIcMFPNumber = formCollection["tbx_MIcMFPNumber"];
                    string[] MIcBrand = formCollection["tbx_MIcBrand"];
                    string[] MIcProductHierarchy = formCollection["tbx_MIcProductHierarchy"];
                    string[] MIcDisabled = formCollection["hid_MIcDisabled"];

                    int countMI = MIcMaterialID.Length;

                    for (int i = 0; i < countMI; i++)
                    {
                        if (MIcDisabled[i] == "0")
                        {
                            TbOneSrdetailMaterialInfo beanD = new TbOneSrdetailMaterialInfo();

                            beanD.CSrid = pSRID;
                            beanD.CMaterialId = MIcMaterialID[i];
                            beanD.CMaterialName = MIcMaterialName[i];
                            beanD.CQuantity = int.Parse(MIcQuantity[i]);
                            beanD.CBasicContent = MIcBasicContent[i];
                            beanD.CMfpnumber = MIcMFPNumber[i];
                            beanD.CBrand = MIcBrand[i];
                            beanD.CProductHierarchy = MIcProductHierarchy[i];
                            beanD.Disabled = int.Parse(MIcDisabled[i]);

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailMaterialInfos.Add(beanD);
                        }
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑物料訊息資訊 ↑↑↑↑↑-----

                    #region -----↓↓↓↓↓序號回報資訊↓↓↓↓↓-----

                    #region 刪除明細
                    var beansDSF = dbOne.TbOneSrdetailSerialFeedbacks.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    foreach (var beanDSF in beansDSF)
                    {
                        beanDSF.Disabled = 1;
                        beanDSF.ModifiedDate = DateTime.Now;
                        beanDSF.ModifiedUserName = LoginUser_Name;
                    }
                    #endregion

                    #region 新增明細
                    string[] SFcSerialID = formCollection["tbx_SFcSerialID"];
                    string[] SFcMaterialID = formCollection["tbx_SFcMaterialID"];
                    string[] SFcMaterialName = formCollection["tbx_SFcMaterialName"];
                    string[] SFcConfigReport = formCollection["hid_filezoneSF"];
                    string[] SFcDisabled = formCollection["hid_SFcDisabled"];

                    int countSF = SFcSerialID.Length;

                    for (int i = 0; i < countSF; i++)
                    {
                        if (SFcDisabled[i] == "0")
                        {
                            TbOneSrdetailSerialFeedback beanD = new TbOneSrdetailSerialFeedback();

                            beanD.CSrid = pSRID;
                            beanD.CSerialId = SFcSerialID[i];
                            beanD.CMaterialId = SFcMaterialID[i];
                            beanD.CMaterialName = SFcMaterialName[i];
                            beanD.CConfigReport = SFcConfigReport[i];
                            beanD.Disabled = int.Parse(SFcDisabled[i]);

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailSerialFeedbacks.Add(beanD);
                        }
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑序號回報資訊 ↑↑↑↑↑-----                   

                    #region -----↓↓↓↓↓處理與工時紀錄↓↓↓↓↓-----

                    #region 刪除明細
                    var beansDRec = dbOne.TbOneSrdetailRecords.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    foreach (var beanDRec in beansDRec)
                    {
                        beanDRec.Disabled = 1;
                        beanDRec.ModifiedDate = DateTime.Now;
                        beanDRec.ModifiedUserName = LoginUser_Name;
                    }
                    #endregion

                    #region 新增明細
                    string[] REcEngineerName = formCollection["tbx_REcEngineerName"];
                    string[] REcEngineerID = formCollection["hid_REcEngineerID"];
                    string[] REcReceiveTime = formCollection["tbx_REcReceiveTime"];
                    string[] REcStartTime = formCollection["tbx_REcStartTime"];
                    string[] REcArriveTime = formCollection["tbx_REcArriveTime"];
                    string[] REcFinishTime = formCollection["tbx_REcFinishTime"];
                    string[] REcWorkHours = formCollection["tbx_REcWorkHours"];
                    string[] REcDesc = formCollection["tbx_REcDesc"];
                    string[] REcSRReport = formCollection["hid_filezoneRE"];
                    string[] REcDisabled = formCollection["hid_REcDisabled"];

                    int countRE = REcEngineerName.Length;

                    for (int i = 0; i < countRE; i++)
                    {
                        if (REcDisabled[i] == "0")
                        {
                            TbOneSrdetailRecord beanD = new TbOneSrdetailRecord();

                            beanD.CSrid = pSRID;
                            beanD.CEngineerName = REcEngineerName[i];
                            beanD.CEngineerId = REcEngineerID[i];

                            if (REcReceiveTime[i] != "")
                            {
                                beanD.CReceiveTime = Convert.ToDateTime(REcReceiveTime[i]);
                            }

                            if (REcStartTime[i] != "")
                            {
                                beanD.CStartTime = Convert.ToDateTime(REcStartTime[i]);
                            }

                            if (REcArriveTime[i] != "")
                            {
                                beanD.CArriveTime = Convert.ToDateTime(REcArriveTime[i]);
                            }

                            if (REcFinishTime[i] != "")
                            {
                                beanD.CFinishTime = Convert.ToDateTime(REcFinishTime[i]);
                            }

                            beanD.CWorkHours = decimal.Parse(REcWorkHours[i]);
                            beanD.CDesc = REcDesc[i];
                            beanD.CSrreport = REcSRReport[i];
                            beanD.Disabled = int.Parse(REcDisabled[i]);

                            beanD.CreatedDate = DateTime.Now;
                            beanD.CreatedUserName = LoginUser_Name;

                            dbOne.TbOneSrdetailRecords.Add(beanD);
                        }
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑處理與工時紀錄 ↑↑↑↑↑-----                    

                    int result = dbOne.SaveChanges();

                    if (result <= 0)
                    {
                        pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "提交失敗(編輯)" + Environment.NewLine;
                        CMF.writeToLog(pSRID, "SaveInstallSR", pMsg, LoginUser_Name);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(tLog))
                        {
                            #region 紀錄修改log
                            TbOneLog logBean = new TbOneLog
                            {
                                CSrid = pSRID,
                                EventName = "SaveInstallSR",
                                Log = tLog,
                                CreatedUserName = LoginUser_Name,
                                CreatedDate = DateTime.Now
                            };

                            dbOne.TbOneLogs.Add(logBean);
                            dbOne.SaveChanges();
                            #endregion
                        }

                        #region call ONE SERVICE（裝機服務案件）狀態更新接口來寄送Mail
                        string TempStatus = CStatus;

                        if (OldCMainEngineerId != CMainEngineerId)
                        {
                            TempStatus = CStatus + "|TRANS"; //轉單
                        }

                        beanIN.IV_LOGINEMPNO = EmpBean.EmployeeERPID;
                        beanIN.IV_LOGINEMPNAME = LoginUser_Name;
                        beanIN.IV_SRID = pSRID;
                        beanIN.IV_STATUS = TempStatus;
                        beanIN.IV_APIURLName = tAPIURLName;

                        CMF.GetAPI_SRSTATUS_Update(beanIN);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "失敗原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();

                CMF.writeToLog(pSRID, "SaveInstallSR", pMsg, LoginUser_Name);
            }

            return RedirectToAction("finishForm");
        }
        #endregion

        #endregion -----↑↑↑↑↑裝機服務 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓服務團隊對照組織設定作業 ↓↓↓↓↓-----
        /// <summary>
        /// 服務團隊對照組織設定作業
        /// </summary>
        /// <returns></returns>
        public IActionResult SRTeamMapping()
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
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            return View();
        }

        #region 服務團隊對照組織設定作業查詢結果
        /// <summary>
        /// 服務團隊對照組織設定作業查詢結果
        /// </summary>        
        /// <param name="cTeamNew">服務團隊ID</param>
        /// <param name="cTeamOld">對應部門ID</param>
        /// <returns></returns>
        public IActionResult SRTeamMappingResult(string cTeamNew, string cTeamOld)
        {
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            #region 組待查詢清單
            var beans = dbOne.TbOneSrteamMappings.Where(x => x.Disabled == 0 &&
                                                         (string.IsNullOrEmpty(cTeamNew) ? true : x.CTeamNewId == cTeamNew) &&
                                                         (string.IsNullOrEmpty(cTeamOld) ? true : x.CTeamOldId == cTeamOld));

            foreach (var bean in beans)
            {
                string[] QueryInfo = new string[5];

                QueryInfo[0] = bean.CId.ToString(); //系統ID
                QueryInfo[1] = bean.CTeamOldId;     //服務團隊ID
                QueryInfo[2] = bean.CTeamOldName;   //服務團隊名稱               
                QueryInfo[3] = bean.CTeamNewId;     //對應部門ID
                QueryInfo[4] = bean.CTeamNewName;   //對應部門名稱                

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #region 儲存服務團隊對照組織設定檔
        /// <summary>
        /// 儲存服務團隊對照組織設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <param name="cTeamOldID">服務團隊ID</param>
        /// <param name="cTeamOldName">服務團隊名稱 </param>
        /// <param name="cTeamNewID">對應部門ID</param>
        /// <param name="cTeamNewName">對應部門名稱</param>        
        /// <returns></returns>
        public ActionResult saveTeam(string cID, string cTeamOldID, string cTeamOldName, string cTeamNewID, string cTeamNewName)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            string tMsg = string.Empty;

            try
            {
                int result = 0;
                if (cID == null)
                {
                    #region -- 新增 --
                    var prBean = dbOne.TbOneSrteamMappings.FirstOrDefault(x => x.Disabled == 0 &&
                                                                           x.CTeamOldId == cTeamOldID &&
                                                                           x.CTeamNewId == cTeamNewID);
                    if (prBean == null)
                    {
                        TbOneSrteamMapping prBean1 = new TbOneSrteamMapping();
                        
                        prBean1.CTeamOldId = cTeamOldID.Trim();
                        prBean1.CTeamOldName = cTeamOldName.Trim();
                        prBean1.CTeamNewId = cTeamNewID.Trim();
                        prBean1.CTeamNewName = cTeamNewName.Trim();                        
                        prBean1.Disabled = 0;

                        prBean1.CreatedUserName = ViewBag.empEngName;
                        prBean1.CreatedDate = DateTime.Now;

                        dbOne.TbOneSrteamMappings.Add(prBean1);
                        result = dbOne.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此服務團隊已存在，請重新輸入！";
                    }
                    #endregion                
                }
                else
                {
                    #region -- 編輯 --
                    var prBean = dbOne.TbOneSrteamMappings.FirstOrDefault(x => x.Disabled == 0 &&
                                                                            x.CId.ToString() != cID &&
                                                                            x.CTeamOldId == cTeamOldID &&
                                                                            x.CTeamNewId == cTeamNewID);
                    if (prBean == null)
                    {
                        var prBean1 = dbOne.TbOneSrteamMappings.FirstOrDefault(x => x.CId.ToString() == cID);
                        prBean1.CTeamNewId = cTeamNewID.Trim();
                        prBean1.CTeamNewName = cTeamNewName.Trim();

                        prBean1.ModifiedUserName = ViewBag.empEngName;
                        prBean1.ModifiedDate = DateTime.Now;
                        result = dbOne.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此服務團隊已存在，請重新輸入！";
                    }
                    #endregion
                }
                return Json(tMsg);
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }
        #endregion

        #region 刪除服務團隊對照組織設定檔
        /// <summary>
        /// 刪除服務團隊對照組織設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <returns></returns>
        public ActionResult DeleteSRTeamMapping(string cID)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            var ctBean = dbOne.TbOneSrteamMappings.FirstOrDefault(x => x.CId.ToString() == cID);
            ctBean.Disabled = 1;
            ctBean.ModifiedUserName = ViewBag.empEngName;
            ctBean.ModifiedDate = DateTime.Now;

            var result = dbOne.SaveChanges();

            return Json(result);
        }
        #endregion

        #endregion -----↑↑↑↑↑服務團隊對照組織設定作業 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓客戶Email對照設定作業 ↓↓↓↓↓-----
        /// <summary>
        /// 客戶Email對照設定作業
        /// </summary>
        /// <returns></returns>
        public IActionResult SRCustomerEmailMapping()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_CompCode = EmpBean.CompanyCode;
            ViewBag.cLoginUser_BUKRS = EmpBean.BUKRS;
            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;

            pCompanyCode = EmpBean.BUKRS;            
            #endregion

            var model = new ViewModelTeam();

            return View(model);
        }

        #region 客戶Email對照設定作業查詢結果
        /// <summary>
        /// 客戶Email對照設定作業查詢結果
        /// </summary>        
        /// <param name="cCustomerID">客戶代號</param>
        /// <param name="cTeamID">服務團隊ID</param>
        /// <returns></returns>
        public IActionResult SRCustomerEmailMappingResult(string cCustomerID, string cTeamID)
        {
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            #region 組待查詢清單
            var beans = dbOne.TbOneSrcustomerEmailMappings.Where(x => x.Disabled == 0 &&
                                                                (string.IsNullOrEmpty(cCustomerID) ? true : x.CCustomerId == cCustomerID) &&
                                                                (string.IsNullOrEmpty(cTeamID) ? true : x.CTeamId == cTeamID));

            foreach (var bean in beans)
            {
                string[] QueryInfo = new string[10];

                string CCustomerName = CMF.findCustomerName(bean.CCustomerId);
                string CCTeamName = CMF.findTeamName(bean.CTeamId);

                QueryInfo[0] = bean.CId.ToString(); //系統ID
                QueryInfo[1] = bean.CCustomerId;     //客戶代號
                QueryInfo[2] = CCustomerName;       //客戶名稱         
                QueryInfo[3] = bean.CTeamId;        //服務團隊ID
                QueryInfo[4] = CCTeamName;         //服務團隊名稱
                QueryInfo[5] = bean.CEmailId;       //Email網域名稱               
                QueryInfo[6] = bean.CContactName;   //客戶聯絡人姓名
                QueryInfo[7] = bean.CContactPhone;  //客戶聯絡人電話
                QueryInfo[8] = bean.CContactMobile; //客戶聯絡人手機
                QueryInfo[9] = bean.CContactEmail;  //客戶聯絡人E-Mail

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #region 儲存客戶Email對照設定檔
        /// <summary>
        /// 儲存客戶Email對照設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <param name="cCustomerID">客戶代號</param>
        /// <param name="cCustomerName">客戶名稱</param>
        /// <param name="cTeamID">服務團隊ID</param>
        /// <param name="cEmailID">Email網域名稱</param>   
        /// <param name="cContactName">客戶聯絡人姓名</param>
        /// <param name="cContactPhone">客戶聯絡人電話</param>
        /// <param name="cContactMobile">客戶聯絡人手機</param>
        /// <param name="cContactEmail">客戶聯絡人E-Mail</param>
        /// <returns></returns>
        public ActionResult saveCustomerEmail(string cID, string cCustomerID, string cCustomerName, string cTeamID, string cEmailID, string cContactName, 
                                            string cContactPhone, string cContactMobile, string cContactEmail)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            string tMsg = string.Empty;

            cContactMobile = String.IsNullOrEmpty(cContactMobile) ? "" : cContactMobile;

            try
            {
                int result = 0;
                if (cID == null)
                {
                    #region -- 新增 --
                    var prBean = dbOne.TbOneSrcustomerEmailMappings.FirstOrDefault(x => x.Disabled == 0 && x.CEmailId == cEmailID.Trim());
                    if (prBean == null)
                    {
                        TbOneSrcustomerEmailMapping  prBean1 = new TbOneSrcustomerEmailMapping();

                        prBean1.CCustomerId = cCustomerID.Trim();
                        prBean1.CCustomerName = cCustomerName.Trim();
                        prBean1.CTeamId = cTeamID.Trim();
                        prBean1.CEmailId = cEmailID.Trim();
                        prBean1.CContactName = cContactName.Trim();
                        prBean1.CContactPhone = cContactPhone.Trim();
                        prBean1.CContactMobile = cContactMobile.Trim();
                        prBean1.CContactEmail = cContactEmail.Trim();
                        prBean1.Disabled = 0;

                        prBean1.CreatedUserName = ViewBag.empEngName;
                        prBean1.CreatedDate = DateTime.Now;

                        dbOne.TbOneSrcustomerEmailMappings.Add(prBean1);
                        result = dbOne.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此Email網域名稱已存在，請重新輸入！";
                    }
                    #endregion                
                }
                else
                {
                    #region -- 編輯 --
                    var prBean = dbOne.TbOneSrcustomerEmailMappings.FirstOrDefault(x => x.Disabled == 0 &&
                                                                            x.CId.ToString() != cID &&                                                                             
                                                                            x.CEmailId == cEmailID.Trim());
                    if (prBean == null)
                    {
                        var prBean1 = dbOne.TbOneSrcustomerEmailMappings.FirstOrDefault(x => x.CId.ToString() == cID);
                        prBean1.CCustomerId = cCustomerID.Trim();
                        prBean1.CCustomerName = cCustomerName.Trim();
                        prBean1.CEmailId = cEmailID.Trim();
                        prBean1.CContactName = cContactName.Trim();
                        prBean1.CContactPhone = cContactPhone.Trim();
                        prBean1.CContactMobile = cContactMobile.Trim();
                        prBean1.CContactEmail = cContactEmail.Trim();

                        prBean1.ModifiedUserName = ViewBag.empEngName;
                        prBean1.ModifiedDate = DateTime.Now;
                        result = dbOne.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此Email網域名稱已存在，請重新輸入！";
                    }
                    #endregion
                }
                return Json(tMsg);
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }
        #endregion

        #region 刪除客戶Email對照設定檔
        /// <summary>
        /// 刪除客戶Email對照設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <returns></returns>
        public ActionResult DeleteSRCustomerEmailMapping(string cID)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            var ctBean = dbOne.TbOneSrcustomerEmailMappings.FirstOrDefault(x => x.CId.ToString() == cID);
            ctBean.Disabled = 1;
            ctBean.ModifiedUserName = ViewBag.empEngName;
            ctBean.ModifiedDate = DateTime.Now;

            var result = dbOne.SaveChanges();

            return Json(result);
        }
        #endregion       

        #endregion -----↑↑↑↑↑客戶Email對照設定作業 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓SQ人員設定作業 ↓↓↓↓↓-----
        /// <summary>
        /// SQ人員設定作業
        /// </summary>
        /// <returns></returns>
        public IActionResult SRSQPerson()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_CompCode = EmpBean.CompanyCode;
            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;

            pCompanyCode = EmpBean.BUKRS;
            #endregion

            var model = new ViewModelSQ();

            return View(model);
        }

        #region SQ人員設定作業查詢結果
        /// <summary>
        /// SQ人員設定作業查詢結果
        /// </summary>        
        /// <param name="cEngineerID">工程師ERPID</param>
        /// <param name="cFullNAME">SQ人員說明</param>
        /// <returns></returns>
        public IActionResult SRSQPersonResult(string cEngineerID, string cFullNAME)
        {
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            #region 組待查詢清單
            var beans = dbOne.TbOneSrsqpeople.Where(x => x.Disabled == 0 &&
                                                    (string.IsNullOrEmpty(cEngineerID) ? true : x.CEngineerId == cEngineerID) &&
                                                    (string.IsNullOrEmpty(cFullNAME) ? true : x.CFullName.Contains(cFullNAME)));

            foreach (var bean in beans)
            {
                string[] QueryInfo = new string[6];                

                QueryInfo[0] = bean.CId.ToString(); //系統ID
                QueryInfo[1] = bean.CEngineerId;     //工程師ERPID
                QueryInfo[2] = bean.CEngineerName;   //工程師姓名   
                QueryInfo[3] = bean.CContent;       //證照編號
                QueryInfo[4] = bean.CFullKey;       //SQ人員代號
                QueryInfo[5] = bean.CFullName;      //SQ人員說明               

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #region 儲存SQ人員設定檔
        /// <summary>
        /// 儲存SQ人員設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <param name="cEngineerID">工程師ERPID</param>
        /// <param name="cEngineerName">工程師姓名</param>
        /// <param name="cSecondKEY">區域代號</param>
        /// <param name="cThirdKEY">類別代號</param>        
        /// <param name="cContent">證照編號</param>
        /// <returns></returns>
        public ActionResult saveSRSQPerson(string cID, string cEngineerID, string cEngineerName, string cSecondKEY, string cThirdKEY, string cContent)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            string tMsg = string.Empty;

            try
            {
                int result = 0;
                if (cID == null)
                {
                    #region -- 新增 --
                    var prBean = dbOne.TbOneSrsqpeople.FirstOrDefault(x => x.Disabled == 0 && 
                                                                        x.CFirstKey == "Z" &&                                                                        
                                                                        x.CSecondKey == cSecondKEY &&
                                                                        x.CThirdKey == cThirdKEY &&
                                                                        x.CEngineerId == cEngineerID &&
                                                                        x.CContent == cContent);
                    if (prBean == null)
                    {
                        TbOneSrsqperson prBean1 = new TbOneSrsqperson();

                        string CNO = CMF.GetSRSQNo("Z", cSecondKEY, cThirdKEY);
                        string CEngineerName = CMF.findEmployeeName(cEngineerID);
                        string[] CFullInfo = CMF.GetSRSQFullInfo(pOperationID_GenerallySR, "Z", cSecondKEY, cThirdKEY, CNO, cContent, CEngineerName);

                        prBean1.CFirstKey = "Z";
                        prBean1.CSecondKey = cSecondKEY;
                        prBean1.CThirdKey = cThirdKEY;
                        prBean1.CNo = CNO;
                        prBean1.CEngineerId = cEngineerID;
                        prBean1.CEngineerName = CEngineerName;
                        prBean1.CContent = cContent;
                        prBean1.CFullKey = CFullInfo[0];
                        prBean1.CFullName = CFullInfo[1];
                        prBean1.Disabled = 0;

                        prBean1.CreatedUserName = ViewBag.empEngName;
                        prBean1.CreatedDate = DateTime.Now;

                        dbOne.TbOneSrsqpeople.Add(prBean1);
                        result = dbOne.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此SQ人員代號已存在，請重新輸入！";
                    }
                    #endregion                
                }
                else
                {
                    #region -- 編輯 --
                    var prBean = dbOne.TbOneSrsqpeople.FirstOrDefault(x => x.Disabled == 0 &&
                                                                            x.CId.ToString() != cID &&
                                                                            x.CFirstKey == "Z" &&
                                                                            x.CSecondKey == cSecondKEY &&
                                                                            x.CThirdKey == cThirdKEY &&
                                                                            x.CEngineerId == cEngineerID &&
                                                                            x.CContent == cContent);
                    if (prBean == null)
                    {
                        var prBean1 = dbOne.TbOneSrsqpeople.FirstOrDefault(x => x.CId.ToString() == cID);

                        string[] CFullInfo = CMF.GetSRSQFullInfo(pOperationID_GenerallySR, "Z", prBean1.CSecondKey, prBean1.CThirdKey, prBean1.CNo, cContent, prBean1.CEngineerName);

                        prBean1.CContent = cContent;
                        prBean1.CFullName = CFullInfo[1];

                        prBean1.ModifiedUserName = ViewBag.empEngName;
                        prBean1.ModifiedDate = DateTime.Now;
                        result = dbOne.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此SQ人員代號已存在，請重新輸入！";
                    }
                    #endregion
                }
                return Json(tMsg);
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }
        #endregion

        #region 刪除SQ人員設定檔
        /// <summary>
        /// 刪除SQ人員設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <returns></returns>
        public ActionResult DeleteSRSQPerson(string cID)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            var ctBean = dbOne.TbOneSrsqpeople.FirstOrDefault(x => x.CId.ToString() == cID);
            ctBean.Disabled = 1;
            ctBean.ModifiedUserName = ViewBag.empEngName;
            ctBean.ModifiedDate = DateTime.Now;

            var result = dbOne.SaveChanges();

            return Json(result);
        }
        #endregion       

        #endregion -----↑↑↑↑↑SQ人員設定作業 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓個人客戶設定作業 ↓↓↓↓↓-----
        /// <summary>
        /// 個人客戶設定作業
        /// </summary>
        /// <returns></returns>
        public IActionResult SRPersonCustomer()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_CompCode = EmpBean.CompanyCode;
            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;

            pCompanyCode = EmpBean.BUKRS;
            #endregion            

            return View();
        }

        #region 個人客戶設定作業查詢結果
        /// <summary>
        /// 個人客戶設定作業查詢結果
        /// </summary>        
        /// <param name="KNA1_KUNNR">個人客戶代號/名稱</param>
        /// <param name="ContactName">姓名</param>
        /// <returns></returns>
        public IActionResult SRPersonCustomerResult(string KNA1_KUNNR, string ContactName)
        {
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            #region 組待查詢清單
            var beans = dbProxy.PersonalContacts.OrderBy(x => x.Kna1Kunnr).Where(x => x.Disabled == 0 &&
                                                                            (string.IsNullOrEmpty(KNA1_KUNNR) ? true : (x.Kna1Kunnr.Contains(KNA1_KUNNR) || x.Kna1Name1.Contains(KNA1_KUNNR))) &&
                                                                            (string.IsNullOrEmpty(ContactName) ? true : x.ContactName.Contains(ContactName)));

            foreach (var bean in beans)
            {
                string[] QueryInfo = new string[10];

                QueryInfo[0] = bean.ContactId.ToString();   //系統ID
                QueryInfo[1] = bean.Kna1Kunnr;              //個人客戶代號
                QueryInfo[2] = bean.Kna1Name1;              //個人客戶名稱   
                QueryInfo[3] = pCompanyCode;                //公司別
                QueryInfo[4] = bean.ContactName;            //姓名
                QueryInfo[5] = bean.ContactPhone;           //電話
                QueryInfo[6] = bean.ContactMobile;          //手機
                QueryInfo[7] = bean.ContactEmail;           //Email
                QueryInfo[8] = bean.ContactCity;            //城市
                QueryInfo[9] = bean.ContactAddress;         //詳細地址

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #region 儲存個人客戶設定檔
        /// <summary>
        /// 儲存個人客戶設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <param name="KNB1_BUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="KNA1_KUNNR">個人客戶代號</param>
        /// <param name="KNA1_NAME1">個人客戶名稱</param>
        /// <param name="ContactName">姓名</param>
        /// <param name="ContactPhone">電話</param>
        /// <param name="ContactCity">城市</param>
        /// <param name="ContactMobile">手機</param>
        /// <param name="ContactAddress">詳細地址</param>
        /// <param name="ContactEmail">Email</param>
        /// <returns></returns>
        public ActionResult saveSRPersonCustomer(string cID, string KNB1_BUKRS, string KNA1_KUNNR, string KNA1_NAME1, string ContactName, 
                                               string ContactPhone, string ContactCity, string ContactMobile, string ContactAddress, string ContactEmail)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            string tMsg = string.Empty;

            KNB1_BUKRS = string.IsNullOrEmpty(KNB1_BUKRS) ? "" : KNB1_BUKRS;
            KNA1_KUNNR = string.IsNullOrEmpty(KNA1_KUNNR) ? "" : KNA1_KUNNR;
            KNA1_NAME1 = string.IsNullOrEmpty(KNA1_NAME1) ? "" : KNA1_NAME1;
            ContactName = string.IsNullOrEmpty(ContactName) ? "" : ContactName;
            ContactPhone = string.IsNullOrEmpty(ContactPhone) ? "" : ContactPhone;
            ContactCity = string.IsNullOrEmpty(ContactCity) ? "" : ContactCity;
            ContactMobile = string.IsNullOrEmpty(ContactMobile) ? "" : ContactMobile;
            ContactAddress = string.IsNullOrEmpty(ContactAddress) ? "" : ContactAddress;
            ContactEmail = string.IsNullOrEmpty(ContactEmail) ? "" : ContactEmail;

            try
            {
                int result = 0;
                if (cID == null)
                {
                    #region -- 新增 --                    
                    PersonalContact prBean1 = new PersonalContact();

                    string CNO = CMF.findPERSONALISerialID();

                    prBean1.ContactId = Guid.NewGuid();                    
                    prBean1.Kna1Kunnr = CNO;
                    prBean1.Kna1Name1 = KNA1_NAME1;
                    prBean1.Knb1Bukrs = EmpBean.BUKRS;
                    prBean1.ContactName = ContactName;
                    prBean1.ContactPhone = ContactPhone;
                    prBean1.ContactCity = ContactCity;
                    prBean1.ContactMobile = ContactMobile;
                    prBean1.ContactAddress = ContactAddress;
                    prBean1.ContactEmail = ContactEmail;
                    prBean1.Disabled = 0;

                    prBean1.CreatedUserName = ViewBag.empEngName;
                    prBean1.CreatedDate = DateTime.Now;

                    dbProxy.PersonalContacts.Add(prBean1);
                    result = dbProxy.SaveChanges();
                    #endregion
                }
                else
                {
                    #region -- 編輯 --
                    var prBean = dbProxy.PersonalContacts.FirstOrDefault(x => x.Disabled == 0 && x.ContactId.ToString() != cID &&
                                                                          (string.IsNullOrEmpty(ContactEmail) ? true : x.ContactEmail == ContactEmail));
                    if (prBean == null)
                    {
                        var prBean1 = dbProxy.PersonalContacts.FirstOrDefault(x => x.ContactId.ToString() == cID);

                        prBean1.Kna1Name1 = KNA1_NAME1;                        
                        prBean1.ContactName = ContactName;
                        prBean1.ContactPhone = ContactPhone;
                        prBean1.ContactCity = ContactCity;
                        prBean1.ContactMobile = ContactMobile;
                        prBean1.ContactAddress = ContactAddress;
                        prBean1.ContactEmail = ContactEmail;

                        prBean1.ModifiedUserName = ViewBag.empEngName;
                        prBean1.ModifiedDate = DateTime.Now;
                        result = dbProxy.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此個人客戶Email已存在，請重新輸入！";
                    }
                    #endregion
                }
                return Json(tMsg);
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }
        #endregion

        #region 刪除個人客戶設定檔
        /// <summary>
        /// 刪除個人客戶設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <returns></returns>
        public ActionResult DeleteSRPersonCustomer(string cID)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            var ctBean = dbProxy.PersonalContacts.FirstOrDefault(x => x.ContactId.ToString() == cID);
            ctBean.Disabled = 1;
            ctBean.ModifiedUserName = ViewBag.empEngName;
            ctBean.ModifiedDate = DateTime.Now;

            var result = dbProxy.SaveChanges();

            return Json(result);
        }
        #endregion       

        #endregion -----↑↑↑↑↑個人客戶設定作業 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓報修類別設定作業 ↓↓↓↓↓-----
        /// <summary>
        /// 報修類別設定作業
        /// </summary>
        /// <returns></returns>
        public IActionResult SRRepairType()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_CompCode = EmpBean.CompanyCode;
            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;

            pCompanyCode = EmpBean.BUKRS;
            #endregion

            #region 報修類別
            //大類
            var SRTypeOneList = CMF.findFirstKINDList();

            //中類
            var SRTypeSecList = new List<SelectListItem>();
            SRTypeSecList.Add(new SelectListItem { Text = " ", Value = "" });

            //小類
            var SRTypeThrList = new List<SelectListItem>();
            SRTypeThrList.Add(new SelectListItem { Text = " ", Value = "" });

            ViewBag.QuerySRTypeOneList = SRTypeOneList;
            ViewBag.QuerySRTypeSecList = SRTypeSecList;
            ViewBag.QuerySRTypeThrList = SRTypeThrList;

            ViewBag.SRTypeOneList = SRTypeOneList;
            ViewBag.SRTypeSecList = SRTypeSecList;
            ViewBag.SRTypeThrList = SRTypeThrList;
            #endregion

            var model = new ViewModelRepairType();

            return View(model);
        }

        #region 報修類別設定作業查詢結果
        /// <summary>
        /// 報修類別設定作業查詢結果
        /// </summary>
        /// <param name="SRTypeOne">報修類別(大類)</param>
        /// <param name="SRTypeSec">報修類別(中類)</param>
        /// <param name="SRTypeThr">報修類別(小類)</param>
        /// <param name="KIND_NAME">類別說明</param>
        /// <returns></returns>
        public IActionResult SRRepairTypeResult(string SRTypeOne, string SRTypeSec, string SRTypeThr, string KIND_NAME)
        {
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            SRTypeOne = string.IsNullOrEmpty(SRTypeOne) ? "" : SRTypeOne;
            SRTypeSec = string.IsNullOrEmpty(SRTypeSec) ? "" : SRTypeSec;
            SRTypeThr = string.IsNullOrEmpty(SRTypeThr) ? "" : SRTypeThr;
            KIND_NAME = string.IsNullOrEmpty(KIND_NAME) ? "" : KIND_NAME.Trim();

            #region 組待查詢清單
            List<TbOneSrrepairType> tList = new List<TbOneSrrepairType>();

            if (SRTypeOne == "" && SRTypeSec == "" && SRTypeThr == "")
            {
                tList = dbOne.TbOneSrrepairTypes.OrderBy(x => x.CKindKey).Where(x => x.Disabled == 0 &&
                                                               (string.IsNullOrEmpty(KIND_NAME) ? true : x.CKindName.Contains(KIND_NAME))).ToList();
            }
            else
            {
                if (SRTypeThr != "")
                {
                    tList = dbOne.TbOneSrrepairTypes.OrderBy(x => x.CKindKey).Where(x => x.Disabled == 0 && x.CKindKey == SRTypeThr &&
                                                               (string.IsNullOrEmpty(KIND_NAME) ? true : x.CKindName.Contains(KIND_NAME))).ToList();
                }
                else if (SRTypeSec != "")
                {
                    tList = dbOne.TbOneSrrepairTypes.OrderBy(x => x.CKindKey).Where(x => x.Disabled == 0 && x.CKindLevel == 3 && x.CUpKindKey == SRTypeSec &&
                                                               (string.IsNullOrEmpty(KIND_NAME) ? true : x.CKindName.Contains(KIND_NAME))).ToList();
                }
                else if (SRTypeOne != "")
                {
                    tList = dbOne.TbOneSrrepairTypes.OrderBy(x => x.CKindKey).Where(x => x.Disabled == 0 && x.CKindLevel == 2 && x.CUpKindKey == SRTypeOne &&
                                                               (string.IsNullOrEmpty(KIND_NAME) ? true : x.CKindName.Contains(KIND_NAME))).ToList();
                }
            }

            foreach (var bean in tList)
            {
                string[] QueryInfo = new string[6];

                QueryInfo[0] = bean.CId.ToString();         //系統ID
                QueryInfo[1] = bean.CKindKey;               //類別代號
                QueryInfo[2] = bean.CKindName;              //類別說明
                QueryInfo[3] = bean.CKindLevel.ToString();  //類別階層
                QueryInfo[4] = bean.CUpKindKey;             //父階代號                

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #region 儲存報修類別設定檔
        /// <summary>
        /// 儲存報修類別設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <param name="cKIND_KEY">類別代碼</param>
        /// <param name="cKIND_NAME">類別說明</param>
        /// <param name="cKIND_LEVEL">類別階層(1.大類 2.中類 3.小類)</param>
        /// <param name="cUP_KIND_KEY">父階代號(0.父類 1.大類 2.中類)</param>
        /// <returns></returns>
        public ActionResult saveSRRepairType(string cID, string cKIND_KEY, string cKIND_NAME, string cKIND_LEVEL, string cUP_KIND_KEY)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            string tMsg = string.Empty;

            try
            {
                int result = 0;
                if (cID == null)
                {
                    #region -- 新增 --
                    var prBean = dbOne.TbOneSrrepairTypes.FirstOrDefault(x => x.Disabled == 0 && x.CKindKey == cKIND_KEY);

                    if (prBean == null)
                    {
                        TbOneSrrepairType prBean1 = new TbOneSrrepairType();                        

                        prBean1.CKindKey = cKIND_KEY;
                        prBean1.CKindName = cKIND_NAME;
                        prBean1.CKindNameEnUs = "";
                        prBean1.CKindLevel = int.Parse(cKIND_LEVEL);
                        prBean1.CUpKindKey = cUP_KIND_KEY;
                        prBean1.Disabled = 0;

                        prBean1.CreatedUserName = ViewBag.empEngName;
                        prBean1.CreatedDate = DateTime.Now;

                        dbOne.TbOneSrrepairTypes.Add(prBean1);
                        result = dbOne.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此報修類別代號已存在，請重新輸入！";
                    }
                    #endregion                
                }
                else
                {
                    #region -- 編輯 --
                    var prBean = dbOne.TbOneSrrepairTypes.FirstOrDefault(x => x.Disabled == 0 &&
                                                                           x.CId.ToString() != cID &&
                                                                           x.CKindKey == cKIND_KEY);
                    if (prBean == null)
                    {
                        var prBean1 = dbOne.TbOneSrrepairTypes.FirstOrDefault(x => x.CId.ToString() == cID);

                        prBean1.CKindName = cKIND_NAME;

                        prBean1.ModifiedUserName = ViewBag.empEngName;
                        prBean1.ModifiedDate = DateTime.Now;
                        result = dbOne.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此報修類別代號已存在，請重新輸入！";
                    }
                    #endregion
                }
                return Json(tMsg);
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }
        #endregion

        #region 刪除報修類別設定檔
        /// <summary>
        /// 刪除報修類別設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <returns></returns>
        public ActionResult DeleteSRRepairType(string cID)
        {
            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            string reValue = string.Empty;

            var ctBean = dbOne.TbOneSrrepairTypes.FirstOrDefault(x => x.CId.ToString() == cID);

            if (ctBean != null)
            {
                string cKIND_KEY = ctBean.CKindKey;

                var bean = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrtypeOne == cKIND_KEY || x.CSrtypeSec == cKIND_KEY || x.CSrtypeThr == cKIND_KEY);

                if (bean == null)
                {
                    ctBean.Disabled = 1;
                    ctBean.ModifiedUserName = ViewBag.empEngName;
                    ctBean.ModifiedDate = DateTime.Now;

                    var result = dbOne.SaveChanges();

                    if (result <= 0 )
                    {
                        reValue = "刪除失敗，請MIS確認資料是否有異常！";
                    }
                }
                else
                {
                    reValue = "因該類別代號【" + cKIND_KEY + "】已存在服務主檔，不准許刪除！";
                }
            }            

            return Json(reValue);
        }
        #endregion 

        #region 傳入報修類別檔的0.父類、1.大類、2.中類，並取得最新的類別代號
        /// <summary>
        /// 傳入報修類別檔的0.父類、1.大類、2.中類，並取得最新的類別代號
        /// </summary>
        /// <param name="SRTypeZero">父類</param>
        /// <param name="SRTypeOne">大類</param>
        /// <param name="SRTypeSec">中類</param>
        /// <returns></returns>
        public IActionResult AjaxfindSRRepairTypeKindKey(string SRTypeZero, string SRTypeOne, string SRTypeSec)
        {
            string reValue = string.Empty;           

            reValue = CMF.findSRRepairTypeKindKey(SRTypeZero, SRTypeOne, SRTypeSec);

            return Json(reValue);
        }
        #endregion

        #endregion -----↑↑↑↑↑報修類別設定作業 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓序號異動設定作業 ↓↓↓↓↓-----
        /// <summary>
        /// 序號異動設定作業
        /// </summary>
        /// <returns></returns>
        public IActionResult SRSerialChang()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();

            #region 取得登入人員資訊
            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            ViewBag.cLoginUser_Name = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
            #endregion

            #region 取得API URL
            bool tIsFormal = CMF.getCallSAPERPPara(pOperationID_GenerallySR); //取得呼叫SAPERP參數是正式區或測試區(true.正式區 false.測試區)
            string tAPIURLName = string.Empty;

            if (tIsFormal)
            {                
                tAPIURLName = @"https://api-qas.etatung.com";
            }
            else
            {             
                tAPIURLName = @"https://api-qas.etatung.com";
            }

            ViewBag.cAPIURLName = tAPIURLName;
            #endregion

            var model = new ViewModelTeam();

            return View(model);
        }

        #region 序號異動設定作業查詢結果
        /// <summary>
        /// 序號異動設定作業查詢結果
        /// </summary>        
        /// <param name="IV_SERIAL">序號</param>
        /// <param name="cTeamID">服務團隊ID</param>
        /// <returns></returns>
        public IActionResult SRSerialChangResult(string IV_SERIAL)
        {
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            string IV_DNDATE = string.Empty;
            string IV_VNAME = string.Empty;

            #region 組待查詢清單
            var beans = dbProxy.Stockalls.Where(x => x.IvSerial.Contains(IV_SERIAL));

            foreach (var bean in beans)
            {
                #region 組待查詢清單
                string[] QueryInfo = new string[7];

                if (bean.IvDndate != null)
                {
                    IV_DNDATE = Convert.ToDateTime(bean.IvDndate).ToString("yyyy-MM-dd");
                }                

                QueryInfo[0] = bean.IvSerial;       //序號
                QueryInfo[1] = bean.CustomerId;     //客戶代號
                QueryInfo[2] = bean.CustomerName;   //客戶名稱           
                QueryInfo[3] = bean.SoNo;          //銷售訂單號
                QueryInfo[4] = IV_DNDATE;         //出貨日期
                QueryInfo[5] = bean.ProdId;        //物料編號
                QueryInfo[6] = bean.Product;       //品名/規格                

                QueryToList.Add(QueryInfo);
                #endregion
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #region 呼叫Ajax儲存出貨資料檔
        /// <summary>
        /// 呼叫Ajax儲存出貨資料檔
        /// </summary>               
        /// <param name="pLoginName">登入者姓名</param>        
        /// <param name="pAPIURLName">API站台名稱</param>
        /// <param name="AryOriSERIAL">Array 原序號</param>
        /// <param name="ArySERIAL">Array 新序號</param>
        /// <param name="AryCID">Array 客戶ID</param>
        /// <param name="AryCIDName">Array 客戶名稱</param>
        /// <param name="ArySONO">Array 銷售訂單號</param>
        /// <param name="AryMATERIAL">Array 產品編號</param>
        /// <param name="AryDesc">Array 品名/規格</param>        
        /// <returns></returns>
        public IActionResult callAjaxSaveStockOUT(string pLoginName, string pAPIURLName, string[] AryOriSERIAL, string[] ArySERIAL,
                                                string[] AryCID, string[] AryCIDName, string[] ArySONO, string[] AryMATERIAL, string[] AryDesc)
        {
            string result = "";

            result = CMF.callAjaxSaveStockOUT(pLoginName, pAPIURLName, AryOriSERIAL, ArySERIAL, AryCID, AryCIDName, ArySONO, AryMATERIAL, AryDesc);

            return Json(result);
        }
        #endregion      

        #endregion -----↑↑↑↑↑序號異動設定作業 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓共用方法 ↓↓↓↓↓-----

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

        #region 產生SRID
        /// <summary>
        /// 產生SRID
        /// </summary>
        /// <param name="cTitle">SRID開頭編號</param>
        /// <param name="cSRID">SRID</param>
        /// <param name="cGUID">系統GUID</param>
        /// <returns></returns>
        public IActionResult getSRID(string cTitle, string cSRID, string cGUID)
        {
            cSRID = string.IsNullOrEmpty(cSRID) ? "" : cSRID;
            cGUID = string.IsNullOrEmpty(cGUID) ? "" : cGUID;

            string reValue = CMF.GetSRID(cTitle, cSRID);

            #region 判斷系統目前GUID是否已被異動
            if (cGUID != "")
            {
                if (reValue != "")
                {
                    reValue = CMF.checkSRIDIsChang(reValue, cGUID);
                }
            }
            #endregion

            return Json(reValue);
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

        #region Ajax取得鄉鎮市區、路段(名)、門牌號碼
        /// <summary>
        /// Ajax取得鄉鎮市區、路段(名)、門牌號碼
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="keyword2"></param>
        /// <param name="keyword3"></param>
        /// <returns></returns>
        public IActionResult findPostalaAddressInfo(string keyword, string keyword2, string keyword3)
        {
            List<string> reLists = new List<string>();

            if (string.IsNullOrEmpty(keyword)) //縣市名稱
            {
                var result = (from p in dbProxy.PostalaAddressAndCodes
                              select new { p.City, p.Code }).Distinct().OrderBy(x => x.Code);

                foreach (var bean in result)
                {
                    if (!reLists.Contains(bean.City))
                    {
                        reLists.Add(bean.City);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(keyword3)) //門牌號碼
                {
                    var result = (from p in dbProxy.PostalaAddressAndCodes
                                  where p.City == keyword.Trim() && p.Township == keyword2.Trim() && p.Road == keyword3.Trim()
                                  select p.No).Distinct();

                    reLists = result.ToList();
                }
                else if (!string.IsNullOrEmpty(keyword2)) //路段(名)
                {
                    var result = (from p in dbProxy.PostalaAddressAndCodes
                                  where p.City == keyword.Trim() && p.Township == keyword2.Trim()
                                  select p.Road).Distinct();

                    reLists = result.ToList();
                }
                else if (!string.IsNullOrEmpty(keyword)) //鄉鎮市區
                {
                    var result = (from p in dbProxy.PostalaAddressAndCodes
                                  where p.City == keyword.Trim()
                                  select p.Township).Distinct();

                    reLists = result.ToList();
                }
            }

            return Json(reLists);
        }
        #endregion

        #region Ajax取得郵遞區號和地址
        /// <summary>
        /// Ajax取得郵遞區號和地址
        /// </summary>
        /// <param name="keyword">縣市名稱</param>
        /// <param name="keyword2">鄉鎮市區</param>
        /// <param name="keyword3">路段(名)</param>
        /// <param name="keyword4">門牌號碼</param>
        /// <returns></returns>
        public IActionResult findPostalaAddressAndCode(string keyword, string keyword2, string keyword3, string keyword4)
        {
            object contentObj = null;

            if (!string.IsNullOrEmpty(keyword4)) //門牌號碼
            {
                contentObj = dbProxy.PostalaAddressAndCodes.Where(x => x.City == keyword.Trim() && x.Township == keyword2.Trim() && x.Road == keyword3.Trim() && x.No.Contains(keyword4.Trim()));
            }
            else if (!string.IsNullOrEmpty(keyword3)) //路段(名)
            {
                contentObj = dbProxy.PostalaAddressAndCodes.Where(x => x.City == keyword.Trim() && x.Township == keyword2.Trim() && x.Road == keyword3.Trim());
            }
            else if (!string.IsNullOrEmpty(keyword2)) //鄉鎮市區
            {
                contentObj = dbProxy.PostalaAddressAndCodes.Where(x => x.City == keyword.Trim() && x.Township == keyword2.Trim());
            }
            else if (!string.IsNullOrEmpty(keyword)) //縣市名稱
            {
                contentObj = dbProxy.PostalaAddressAndCodes.Where(x => x.City == keyword.Trim());
            }

            return Json(contentObj);
        }
        #endregion

        #region Ajax判斷Email格式是否正確
        /// <summary>
        /// Ajax判斷Email格式是否正確
        /// </summary>        
        /// <param name="email">email信箱</param>        
        /// <returns></returns>
        public IActionResult CheckEmailValid(string email)
        {
            bool contentObj = CMF.IsEmailValid(email);

            return Json(contentObj);
        }
        #endregion

        #region Ajax取得客戶聯絡人
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

        #region Ajax取得客戶聯絡人資訊(模糊查詢)
        /// <summary>
        /// 取得客戶聯絡人
        /// </summary>
        /// <param name="cBUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="CustomerID">客戶代號</param>    
        /// <param name="ContactName">聯絡人姓名</param>
        /// <returns></returns>
        public IActionResult findContactInfoByKeywordAndComp(string cBUKRS, string CustomerID, string ContactName)
        {
            object contentObj = CMF.findContactInfoByKeywordAndComp(cBUKRS, CustomerID, ContactName);            

            return Json(contentObj);
        }
        #endregion

        #region Ajax儲存客戶聯絡人(for編輯)
        /// <summary>
        /// /Ajax儲存客戶聯絡人
        /// </summary>        
        /// <param name="cEditContactID">程式作業編號檔系統ID</param>
        /// <param name="cBUKRS">工廠別(T012、T016、C069、T022)</param>
        /// <param name="cCustomerID">客戶代號</param>
        /// <param name="cCustomerName">客戶名稱</param>
        /// <param name="cEditContactName">客戶聯絡人姓名</param>
        /// <param name="cEditContactCity">客戶聯絡人城市</param>
        /// <param name="cEditContactAddress">客戶聯絡人地址</param>
        /// <param name="cEditContactPhone">客戶聯絡人電話</param>
        /// <param name="cEditContactMobile">客戶聯絡人手機</param>
        /// <param name="cEditContactEmail">客戶聯絡人Email</param>
        /// <param name="ModifiedUserName">修改人姓名</param>        
        /// <returns></returns>
        public IActionResult SaveEditContactInfo(string cEditContactID, string cBUKRS, string cCustomerID, string cCustomerName, string cEditContactName,
                                               string cEditContactCity, string cEditContactAddress, string cEditContactPhone, string cEditContactMobile, 
                                               string cEditContactEmail, string ModifiedUserName)
        {
            bool tIsDouble = false;

            string reValue = "SUCCESS";
            string tLog = string.Empty;
            string OldContactCity = string.Empty;
            string OldContactAddress = string.Empty;
            string OldContactPhone = string.Empty;
            string OldContactMobile = string.Empty;
            string OldContactEmail = string.Empty;

            cEditContactCity = string.IsNullOrEmpty(cEditContactCity) ? "" : cEditContactCity.Trim();
            cEditContactAddress = string.IsNullOrEmpty(cEditContactAddress) ? "" : cEditContactAddress.Trim();
            cEditContactPhone = string.IsNullOrEmpty(cEditContactPhone) ? "" : cEditContactPhone.Trim();
            cEditContactMobile = string.IsNullOrEmpty(cEditContactMobile) ? "" : cEditContactMobile.Trim();
            cEditContactEmail = string.IsNullOrEmpty(cEditContactEmail) ? "" : cEditContactEmail.Trim();

            try
            {
                tIsDouble = CMF.CheckContactsIsDoubleEmail(cEditContactID, cBUKRS, cCustomerID, cEditContactEmail);

                if (tIsDouble)
                {
                    reValue = "聯絡人Email重覆，請重新確認！";                    
                }
                else
                {
                    if (cCustomerID.Substring(0, 1) == "P")
                    {
                        var bean = dbProxy.PersonalContacts.FirstOrDefault(x => x.ContactId.ToString() == cEditContactID);

                        if (bean != null) //修改
                        {
                            #region 紀錄新舊值
                            OldContactCity = bean.ContactCity;
                            tLog += CMF.getNewAndOldLog("城市", OldContactCity, cEditContactCity);

                            OldContactAddress = bean.ContactAddress;
                            tLog += CMF.getNewAndOldLog("詳細地址", OldContactAddress, cEditContactAddress);

                            OldContactPhone = bean.ContactPhone;
                            tLog += CMF.getNewAndOldLog("電話", OldContactPhone, cEditContactPhone);

                            OldContactMobile = bean.ContactMobile;
                            tLog += CMF.getNewAndOldLog("手機", OldContactMobile, cEditContactMobile);

                            OldContactEmail = bean.ContactEmail;
                            tLog += CMF.getNewAndOldLog("Email", OldContactEmail, cEditContactEmail);
                            #endregion

                            bean.ContactCity = cEditContactCity;
                            bean.ContactAddress = cEditContactAddress;
                            bean.ContactPhone = cEditContactPhone;
                            bean.ContactMobile = cEditContactMobile;
                            bean.ContactEmail = cEditContactEmail;

                            bean.ModifiedUserName = ModifiedUserName;
                            bean.ModifiedDate = DateTime.Now;                            
                        }
                    }
                    else
                    {
                        var bean = dbProxy.CustomerContacts.FirstOrDefault(x => x.ContactId.ToString() == cEditContactID);

                        if (bean != null) //修改
                        {
                            #region 紀錄新舊值
                            OldContactCity = bean.ContactCity;
                            tLog += CMF.getNewAndOldLog("城市", OldContactCity, cEditContactCity);

                            OldContactAddress = bean.ContactAddress;
                            tLog += CMF.getNewAndOldLog("詳細地址", OldContactAddress, cEditContactAddress);

                            OldContactPhone = bean.ContactPhone;
                            tLog += CMF.getNewAndOldLog("電話", OldContactPhone, cEditContactPhone);

                            OldContactMobile = bean.ContactMobile;
                            tLog += CMF.getNewAndOldLog("手機", OldContactMobile, cEditContactMobile);

                            OldContactEmail = bean.ContactEmail;
                            tLog += CMF.getNewAndOldLog("Email", OldContactEmail, cEditContactEmail);
                            #endregion

                            bean.ContactCity = cEditContactCity;
                            bean.ContactAddress = cEditContactAddress;
                            bean.ContactPhone = cEditContactPhone;
                            bean.ContactMobile = cEditContactMobile;
                            bean.ContactEmail = cEditContactEmail;

                            bean.ModifiedUserName = ModifiedUserName;
                            bean.ModifiedDate = DateTime.Now;                            
                        }
                    }

                    int result = dbProxy.SaveChanges();

                    if (result <= 0)
                    {
                        reValue = "提交失敗(編輯)";
                    }                    
                }

                if (tLog != "")
                {
                    CMF.writeToLog(cCustomerID, "SaveEditContactInfo", tLog, ModifiedUserName);
                }
            }
            catch (Exception e)
            {
                reValue = "SaveEditContactInfo Error：" + e.Message;                
            }

            return Json(reValue);
        }
        #endregion

        #region Ajax儲存客戶聯絡人(for新增)
        /// <summary>
        /// /Ajax儲存客戶聯絡人
        /// </summary>
        /// <param name="cBUKRS">工廠別(T012、T016、C069、T022)</param>
        /// <param name="cCustomerID">客戶代號</param>
        /// <param name="cCustomerName">客戶名稱</param>
        /// <param name="cAddContactName">客戶聯絡人姓名</param>
        /// <param name="cAddContactCity">客戶聯絡人城市</param>
        /// <param name="cAddContactAddress">客戶聯絡人地址</param>
        /// <param name="cAddContactPhone">客戶聯絡人電話</param>
        /// <param name="cAddContactMobile">客戶聯絡人手機</param>
        /// <param name="cAddContactEmail">客戶聯絡人Email</param>
        /// <param name="ModifiedUserName">修改人姓名</param>        
        /// <returns></returns>
        public IActionResult SaveContactInfo(string cBUKRS, string cCustomerID, string cCustomerName, string cAddContactName,
                                           string cAddContactCity, string cAddContactAddress, string cAddContactPhone, string cAddContactMobile, 
                                           string cAddContactEmail, string ModifiedUserName)
        {
            bool tIsDouble = false;

            string tBpmNo = "GenerallySR";
            string reValue = "SUCCESS";
            string tLog = string.Empty;
            
            cAddContactName = string.IsNullOrEmpty(cAddContactName) ? "" : cAddContactName.Trim();
            cAddContactCity = string.IsNullOrEmpty(cAddContactCity) ? "" : cAddContactCity.Trim();
            cAddContactAddress = string.IsNullOrEmpty(cAddContactAddress) ? "" : cAddContactAddress.Trim();
            cAddContactPhone = string.IsNullOrEmpty(cAddContactPhone) ? "" : cAddContactPhone.Trim();
            cAddContactMobile = string.IsNullOrEmpty(cAddContactMobile) ? "" : cAddContactMobile.Trim();
            cAddContactEmail = string.IsNullOrEmpty(cAddContactEmail) ? "" : cAddContactEmail.Trim();

            tIsDouble = CMF.CheckContactsIsDoubleEmail("", cBUKRS, cCustomerID, cAddContactEmail);

            if (tIsDouble)
            {
                reValue = "聯絡人Email重覆，請重新確認！";
            }
            else
            {
                if (cCustomerID.Substring(0, 1) == "P")
                {
                    #region 個人客戶

                    var bean = dbProxy.PersonalContacts.FirstOrDefault(x => x.Disabled == 0 && x.Knb1Bukrs == cBUKRS &&
                                                                         x.Kna1Kunnr == cCustomerID && x.ContactName == cAddContactName);

                    if (bean != null) //修改
                    {
                        bean.ContactCity = cAddContactCity;
                        bean.ContactAddress = cAddContactAddress;
                        bean.ContactPhone = cAddContactPhone;
                        bean.ContactMobile = cAddContactMobile;
                        bean.ContactEmail = cAddContactEmail;

                        bean.ModifiedUserName = ModifiedUserName;
                        bean.ModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        PersonalContact bean1 = new PersonalContact();

                        bean1.ContactId = Guid.NewGuid();
                        bean1.Kna1Kunnr = cCustomerID;
                        bean1.Kna1Name1 = cCustomerName;
                        bean1.Knb1Bukrs = cBUKRS;
                        bean1.ContactName = cAddContactName;
                        bean1.ContactCity = cAddContactCity;
                        bean1.ContactAddress = cAddContactAddress;
                        bean1.ContactPhone = cAddContactPhone;
                        bean1.ContactMobile = cAddContactMobile;
                        bean1.ContactEmail = cAddContactEmail;
                        bean1.Disabled = 0;

                        bean1.CreatedUserName = pLoginName;
                        bean1.CreatedDate = DateTime.Now;

                        dbProxy.PersonalContacts.Add(bean1);
                    }
                    #endregion
                }
                else
                {
                    #region 法人客戶
                    var bean = dbProxy.CustomerContacts.FirstOrDefault(x => (x.Disabled == null || x.Disabled != 1) &&
                                                                         x.BpmNo == tBpmNo && x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == cCustomerID && x.ContactName == cAddContactName);

                    cAddContactMobile = String.IsNullOrEmpty(cAddContactMobile) ? "" : cAddContactMobile;

                    if (bean != null) //修改
                    {
                        bean.ContactCity = cAddContactCity;
                        bean.ContactAddress = cAddContactAddress;
                        bean.ContactPhone = cAddContactPhone;
                        bean.ContactMobile = cAddContactMobile;
                        bean.ContactEmail = cAddContactEmail;

                        bean.ModifiedUserName = ModifiedUserName;
                        bean.ModifiedDate = DateTime.Now;
                    }
                    else //新增
                    {
                        CustomerContact bean1 = new CustomerContact();

                        bean1.ContactId = Guid.NewGuid();
                        bean1.Kna1Kunnr = cCustomerID;
                        bean1.Kna1Name1 = cCustomerName;
                        bean1.Knb1Bukrs = cBUKRS;
                        bean1.ContactType = "5"; //One Service
                        bean1.ContactName = cAddContactName;
                        bean1.ContactCity = cAddContactCity;
                        bean1.ContactAddress = cAddContactAddress;
                        bean1.ContactPhone = cAddContactPhone;
                        bean1.ContactMobile = cAddContactMobile;
                        bean1.ContactEmail = cAddContactEmail;
                        bean1.BpmNo = tBpmNo;
                        bean1.Disabled = 0;

                        bean1.ModifiedUserName = ModifiedUserName;
                        bean1.ModifiedDate = DateTime.Now;

                        dbProxy.CustomerContacts.Add(bean1);
                    }
                    #endregion
                }

                int result = dbProxy.SaveChanges();

                if (result <= 0)
                {
                    reValue = "提交失敗(新增)";
                }
            }            

            return Json(reValue);
        }
        #endregion

        #region Ajax產品序號資訊查詢
        /// <summary>
        /// Ajax產品序號資訊查詢
        /// </summary>
        /// <param name="IV_SERIAL">序號</param>
        /// <returns></returns>
        public IActionResult findMaterialBySerial(string IV_SERIAL)
        {
            var beans = dbProxy.Stockalls.Where(x => x.IvSerial.Contains(IV_SERIAL.Trim())).Take(30);

            List<SerialMaterialInfo> tList = new List<SerialMaterialInfo>();

            foreach (var bean in beans)
            {
                SerialMaterialInfo ProBean = new SerialMaterialInfo();

                ProBean.IV_SERIAL = bean.IvSerial;
                ProBean.ProdID = bean.ProdId;
                ProBean.Product = bean.Product;                

                tList.Add(ProBean);
            }

            return Json(tList);
        }
        #endregion

        #region Ajax取得製造商零件號碼和裝機號碼
        /// <summary>
        /// Ajax取得製造商零件號碼和裝機號碼
        /// </summary>
        /// <param name="ProdID">料號</param>
        /// <param name="IV_SERIAL">序號</param>
        /// <returns></returns>
        public IActionResult findMFRPandInstallNumber(string ProdID, string IV_SERIAL)
        {
            string[] tAry = new string[2];

            tAry[0] = CMF.findMFRPNumber(ProdID);
            tAry[1] = CMF.findInstallNumber(IV_SERIAL);

            return Json(tAry);
        }
        #endregion

        #region Ajax取得物料相關資訊
        /// <summary>
        /// Ajax取得物料相關資訊
        /// </summary>
        /// <param name="ProdID">料號</param>        
        /// <returns></returns>
        public IActionResult findMaterialInfo(string ProdID)
        {
            MaterialInfo ProBean = new MaterialInfo();

            var bean = dbProxy.Materials.FirstOrDefault(x => x.MaraMatnr.Contains(ProdID.Trim()));

            if (bean != null)
            {
                ProBean.MaterialID = bean.MaraMatnr;
                ProBean.MaterialName = bean.MaktTxza1Zf;
                ProBean.MFPNumber = bean.MaraMfrpn;
                ProBean.BasicContent = string.IsNullOrEmpty(bean.BasicContent) ? "" : bean.BasicContent;
                ProBean.ProductHierarchy = bean.MvkeProdh;
                ProBean.Brand = CMF.findMATERIALBRAND(bean.MaraMatnr, bean.MvkeProdh);
            }

            return Json(ProBean);
        }
        #endregion

        #region Ajax取得BOM表查詢結果
        /// <summary>
        /// BOM表查詢結果
        /// </summary>
        /// <param name="ProdID">物料編號</param>        
        /// <returns></returns>
        public IActionResult SpareBOM(string ProdID)
        {
            string reValue = string.Empty;

            reValue = CMF.findMaterialBOM(ProdID);

            if (reValue == "")
            {
                reValue = "無BOM表資訊！";
            }

            ViewBag.BasicContent = reValue.Replace("\r\n", "<br/>");

            return View();
        }
        #endregion

        #region Ajax依關鍵字查詢物料資訊
        /// <summary>
        /// Ajax依關鍵字查詢物料資訊
        /// </summary>
        /// <param name="cBUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="keyword">關鍵字</param>        
        /// <returns></returns>
        public IActionResult findMaterial(string cBUKRS, string keyword)
        {
            Object contentObj = CMF.findMaterialByKeyWords(cBUKRS, keyword);

            return Json(contentObj);
        }
        #endregion

        #region Ajax用中文或英文姓名查詢人員
        /// <summary>
        /// Ajax用中文或英文姓名查詢人員
        /// </summary>
        /// <param name="keyword">中文/英文姓名</param>        
        /// <returns></returns>
        public IActionResult AjaxfindEmployeeByKeyword(string keyword)
        {           

            object contentObj = null;

            contentObj = bpmDB.TblEmployees.Where(x => (x.CEmployeeAccount.Contains(keyword) || x.CEmployeeCName.Contains(keyword)) &&
                                                    (x.CEmployeeLeaveReason == null && x.CEmployeeLeaveDay == null)).Take(5);

            string json = JsonConvert.SerializeObject(contentObj);
            return Content(json, "application/json");
        }
        #endregion       

        #region Ajax用關鍵字查詢SQ人員
        /// <summary>
        /// Ajax用關鍵字查詢SQ人員
        /// </summary>
        /// <param name="keyword">人名或類別關鍵字</param>        
        /// <returns></returns>
        public IActionResult AjaxfindSQPersonByKeyword(string keyword)
        {
            object contentObj = null;

            contentObj = dbOne.TbOneSrsqpeople.Where(x => x.Disabled == 0 & (x.CFullKey.Contains(keyword.Trim()) || x.CFullName.Contains(keyword.Trim()))).Take(10);
            
            return Json(contentObj);
        }
        #endregion

        #region Ajax用關鍵字查詢聯絡人資訊
        /// <summary>
        /// Ajax用關鍵字查詢聯絡人資訊
        /// </summary>
        /// <param name="cBUKRS">工廠別(T012、T016、C069、T022)</param>
        /// <param name="cCustomerID">客戶代號</param>
        /// <param name="keyword">姓名關鍵字</param>        
        /// <returns></returns>
        public IActionResult AjaxfindContactByKeyword(string cBUKRS, string cCustomerID,  string keyword)
        {
            object contentObj = null;

            string tBpmNo = "GenerallySR";

            if (cCustomerID.Substring(0, 1) == "P") //個人客戶
            {
                contentObj = dbProxy.PersonalContacts.Where(x => x.Disabled == 0 && x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == cCustomerID && x.ContactName.Contains(keyword));
            }
            else //法人客戶
            {
                contentObj = dbProxy.CustomerContacts.Where(x => (x.Disabled == null || x.Disabled != 1) &&
                                                               x.BpmNo == tBpmNo && x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == cCustomerID && x.ContactName.Contains(keyword));
            }

            return Json(contentObj);
        }
        #endregion

        #region Ajax用關鍵字查詢服務團隊部門(新)
        /// <summary>
        /// Ajax用關鍵字查詢服務團隊部門(新)
        /// </summary>
        /// <param name="keyword">部門ID或部門名稱</param>        
        /// <returns></returns>
        public IActionResult AjaxfindTeamNewByKeyword(string keyword)
        {
            int count = 0;
            var beans = dbOne.TbOneSrteamMappings.Where(x => x.Disabled == 0 & (x.CTeamNewId.Contains(keyword.Trim()) || x.CTeamNewName.Contains(keyword.Trim())))
                                              .Select(o => new { CTeamNewId = o.CTeamNewId, CTeamNewName = o.CTeamNewName }).Distinct();

            List<TbOneSrteamMapping> ProList = new List<TbOneSrteamMapping>();

            foreach (var bean in beans)
            {
                if (count < 10)
                {
                    TbOneSrteamMapping ProBean = new TbOneSrteamMapping();

                    ProBean.CTeamNewId = bean.CTeamNewId;
                    ProBean.CTeamNewName = bean.CTeamNewName;

                    count++;
                    ProList.Add(ProBean);
                }
            }

            return Json(ProList);
        }
        #endregion

        #region Ajax用關鍵字查詢服務團隊部門(舊)
        /// <summary>
        /// Ajax用關鍵字查詢服務團隊部門(舊)
        /// </summary>
        /// <param name="keyword">團隊ID或團隊名稱</param>        
        /// <returns></returns>
        public IActionResult AjaxfindTeamOldByKeyword(string keyword)
        {
            int count = 0;
            var beans = dbOne.TbOneSrteamMappings.Where(x => x.Disabled == 0 & (x.CTeamOldId.Contains(keyword.Trim()) || x.CTeamOldName.Contains(keyword.Trim())))
                                              .Select(o => new { CTeamOldId = o.CTeamOldId, CTeamOldName = o.CTeamOldName }).Distinct();

            List<TbOneSrteamMapping> ProList = new List<TbOneSrteamMapping>();

            foreach (var bean in beans)
            {
                if (count < 10)
                {
                    TbOneSrteamMapping ProBean = new TbOneSrteamMapping();

                    ProBean.CTeamOldId = bean.CTeamOldId;
                    ProBean.CTeamOldName = bean.CTeamOldName;

                    count++;
                    ProList.Add(ProBean);
                }
            }

            return Json(ProList);
        }
        #endregion

        #region Ajax用關鍵字查詢部門相關資訊
        /// <summary>
        /// Ajax用關鍵字查詢部門相關資訊
        /// </summary>        
        /// <param name="keyword">部門ID/部門名稱關鍵字</param>        
        /// <returns></returns>
        public IActionResult AjaxfindDeptInfoByKeyword(string keyword)
        {
            object contentObj = null;

            contentObj = dbEIP.Departments.Where(x => x.Status == 0 && (x.Id.Contains(keyword.Trim()) || x.Name2.Contains(keyword.Trim()))).Take(10);

            return Json(contentObj);
        }
        #endregion

        #region Ajax用關鍵字查詢客戶Email對照的客戶代號
        /// <summary>
        /// Ajax用關鍵字查詢客戶Email對照的客戶代號
        /// </summary>
        /// <param name="keyword">客戶ID或名稱</param>        
        /// <returns></returns>
        public IActionResult AjaxfindSRCustomerEmailByKeyword(string keyword)
        {
            object contentObj = null;

            contentObj = dbOne.TbOneSrcustomerEmailMappings.Where(x => x.Disabled == 0 && (x.CCustomerId.Contains(keyword.Trim()) || x.CCustomerName.Contains(keyword.Trim()))).Take(10);

            return Json(contentObj);
        }
        #endregion

        #region 取得服務團隊清單
        /// <summary>
        /// 取得服務團隊清單
        /// </summary>        
        /// <param name="pCompanyCode">公司別(T012、T016、C069、T022)</param>
        /// <param name="cEmptyOption">是否要產生「請選擇」選項(True.要 false.不要)</param>
        /// <returns></returns>
        static public List<SelectListItem> findSRTeamMappingListItem(string pCompanyCode, bool cEmptyOption)
        {
            CommonFunction CMF = new CommonFunction();

            var tList = CMF.findSRTeamIDList(pCompanyCode, cEmptyOption);

            return tList;
        }
        #endregion

        #region 查詢更改歷史記錄
        /// <summary>
        /// 查詢更改歷史記錄
        /// </summary>
        /// <param name="cSRID">SRID</param>
        /// <param name="cCustomerID">客戶代號</param>
        /// <returns></returns>
        public IActionResult GetHistoryLog(string cSRID, string cCustomerID)
        {
            OneLogInfo beanOne = new OneLogInfo();

            string EventName = string.Empty;
            string EventName_ContactChang = "SaveEditContactInfo";

            switch (cSRID.Substring(0,2))
            {
                case "61":
                    EventName = "SaveGenerallySR";                    
                    break;

                case "63":
                    EventName = "SaveInstallSR";
                    break;

                case "65":
                    EventName = "SaveMaintainSR";
                    break;
            }

            //歷史記錄資訊(共用)
            var SROneLog = dbOne.TbOneLogs.OrderByDescending(x => x.CreatedDate).Where(x => x.CSrid == cSRID && x.EventName == EventName).ToList();

            //客戶聯絡人異動資訊(共用)
            var SRContactChangLog = dbOne.TbOneLogs.OrderByDescending(x => x.CreatedDate).Where(x => x.CSrid == cCustomerID && x.EventName == EventName_ContactChang).ToList();

            //客戶聯絡人資訊(共用)
            var SRDetailContact = dbOne.TbOneSrdetailContacts.OrderByDescending(x => x.ModifiedDate).Where(x => x.CSrid == cSRID && x.Disabled == 1).ToList();

            //處理與工程紀錄資訊(共用)
            var SRDetailRecord = dbOne.TbOneSrdetailRecords.OrderByDescending(x => x.ModifiedDate).Where(x => x.CSrid == cSRID && x.Disabled == 1).ToList();

            //產品與序號資訊(一般)
            var SRDetailProduct = dbOne.TbOneSrdetailProducts.OrderByDescending(x => x.ModifiedDate).Where(x => x.CSrid == cSRID && x.Disabled == 1).ToList();

            //零件更換資訊(一般)
            var SRDetailPartsReplace = dbOne.TbOneSrdetailPartsReplaces.OrderByDescending(x => x.ModifiedDate).Where(x => x.CSrid == cSRID && x.Disabled == 1).ToList();

            //物料訊息資訊(裝機)
            var SRDetailMaterialInfo = dbOne.TbOneSrdetailMaterialInfos.OrderByDescending(x => x.ModifiedDate).Where(x => x.CSrid == cSRID && x.Disabled == 1).ToList();

            //序號回報資訊(裝機)
            var SRDetailSerialFeedback = dbOne.TbOneSrdetailSerialFeedbacks.OrderByDescending(x => x.ModifiedDate).Where(x => x.CSrid == cSRID && x.Disabled == 1).ToList();

            #region 新增
            beanOne.SROneLog = SROneLog;
            beanOne.SRContactChangLog = SRContactChangLog;
            beanOne.SRDetailContact = SRDetailContact;
            beanOne.SRDetailRecord = SRDetailRecord;
            beanOne.SRDetailProduct = SRDetailProduct;
            beanOne.SRDetailPartsReplace = SRDetailPartsReplace;
            beanOne.SRDetailMaterialInfo = SRDetailMaterialInfo;
            beanOne.SRDetailSerialFeedback = SRDetailSerialFeedback;
            #endregion

            return Json(beanOne);
        }
        #endregion

        #endregion -----↑↑↑↑↑共用方法 ↑↑↑↑↑-----    

        #region -----↓↓↓↓↓自定義Class ↓↓↓↓↓-----     

        #region DropDownList選項Class(一般服務)
        /// <summary>
        /// DropDownList選項Class(一般服務)
        /// </summary>
        public class ViewModel
        {
            #region 狀態
            public string ddl_cStatus { get; set; }

            //不抓DB參數的設定
            //public List<SelectListItem> ListStatus { get; } = new List<SelectListItem>
            //{                
            //    new SelectListItem { Value = "E0001", Text = "新建" },
            //    new SelectListItem { Value = "E0002", Text = "L2處理中" },
            //    new SelectListItem { Value = "E0003", Text = "報價中" },
            //    new SelectListItem { Value = "E0004", Text = "3rd Party處理中" },
            //    new SelectListItem { Value = "E0005", Text = "L3處理中" },
            //    new SelectListItem { Value = "E0006", Text = "完修" },
            //    new SelectListItem { Value = "E0012", Text = "HPGCSN 申請" },
            //    new SelectListItem { Value = "E0013", Text = "HPGCSN 完成" },
            //    new SelectListItem { Value = "E0014", Text = "駁回" },
            //    new SelectListItem { Value = "E0015", Text = "取消" },                
            //};

            public List<SelectListItem> ListStatus = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "SRSTATUS", false);
            #endregion

            #region 報修管道
            public string ddl_cSRPathWay { get; set; }
            public List<SelectListItem> ListSRPathWay = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "SRPATH", false);
            #endregion

            #region 維護服務種類
            public string ddl_cMAServiceType { get; set; }
            public List<SelectListItem> ListMAServiceType = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "SRMATYPE", true);
            #endregion

            #region 處理方式
            public string ddl_cSRProcessWay { get; set; }            
            public List<SelectListItem> ListSRProcessWay = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "SRPROCESS", true);
            #endregion

            #region 是否為二修
            public string ddl_cIsSecondFix { get; set; }
            public List<SelectListItem> ListIsSecondFix = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "ISSECONDFIX", true);
            #endregion

            #region 是否有人損
            public string ddl_PAcPersonalDamage { get; set; }
            public List<SelectListItem> ListPersonalDamage = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "PERSONALDAMAGE", false);
            #endregion

            #region 客戶類型
            public string ddl_cCustomerType { get; set; }
            public List<SelectListItem> ListCustomerType = findCustomerTypeList();
            #endregion
        }
        #endregion

        #region DropDownList選項Class(裝機服務)
        /// <summary>
        /// DropDownList選項Class(裝機服務)
        /// </summary>
        public class ViewModel_Install
        {
            #region 狀態
            public string ddl_cStatus { get; set; }

            //不抓DB參數的設定
            //public List<SelectListItem> ListStatus { get; } = new List<SelectListItem>
            //{                
            //    new SelectListItem { Value = "E0001", Text = "新建" },
            //    new SelectListItem { Value = "E0008", Text = "裝機中" },
            //    new SelectListItem { Value = "E0009", Text = "維修/DOA" },
            //    new SelectListItem { Value = "E0010", Text = "裝機完成" },            
            //    new SelectListItem { Value = "E0014", Text = "駁回" },
            //    new SelectListItem { Value = "E0015", Text = "取消" },                
            //};

            public List<SelectListItem> ListStatus = findSysParameterList(pOperationID_InstallSR, "OTHER", pCompanyCode, "SRSTATUS", false);
            #endregion           

            #region 客戶類型
            public string ddl_cCustomerType { get; set; }
            public List<SelectListItem> ListCustomerType = findCustomerTypeList();
            #endregion
        }
        #endregion

        #region DropDownList選項Class(客戶Email對照設定作業)
        /// <summary>
        /// DropDownList選項Class(客戶Email對照設定作業)
        /// </summary>
        public class ViewModelTeam
        {
            #region 服務團隊ID
            public string ddl_cQueryTeamID { get; set; }
            public string ddl_cTeamID { get; set; }
            public List<SelectListItem> ListTeamID = findSRTeamMappingListItem(pCompanyCode, true);
            #endregion          
        }
        #endregion

        #region DropDownList選項Class(SQ人員設定作業)
        /// <summary>
        /// DropDownList選項Class(SQ人員設定作業)
        /// </summary>
        public class ViewModelSQ
        {
            #region SQ人員區域代號
            public string ddl_cSecondKEY { get; set; }
            public List<SelectListItem> ListSecondKEY = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "SQSECKEY", true);
            #endregion

            #region SQ人員類別代號
            public string ddl_cThirdKEY { get; set; }
            public List<SelectListItem> ListThirdKEY = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "SQTHIKEY", true);
            #endregion     
        }
        #endregion

        #region DropDownList選項Class(報修類別設定作業)
        /// <summary>
        /// DropDownList選項Class(報修類別設定作業)
        /// </summary>
        public class ViewModelRepairType
        {
            #region 報修類別代號
            public string ddl_QuerycKIND_KEY { get; set; }
            public List<SelectListItem> ListKIND_KEY = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "SQSECKEY", true);
            #endregion           
        }
        #endregion

        #region DropDownList選項Class(服務案件種類)
        /// <summary>
        /// DropDownList選項Class(服務案件種類)
        /// </summary>
        public class ViewModelQuerySRProgress
        {
            #region 狀態
            public string ddl_cStatus { get; set; }
            public List<SelectListItem> ListStatus = findSysParameterList_WithEmpty(pOperationID_GenerallySR, "OTHER", pCompanyCode, "SRSTATUS", false);
            #endregion

            #region 服務案件種類代號
            public string ddl_cSRCaseType { get; set; }
            public List<SelectListItem> ListSRCaseType = findSRIDTypeList(true);
            #endregion

            #region 報修管道
            public string ddl_cSRPathWay { get; set; }
            public List<SelectListItem> ListSRPathWay = findSysParameterList_WithEmpty(pOperationID_GenerallySR, "OTHER", pCompanyCode, "SRPATH", false);
            #endregion
        }
        #endregion

        #region 關鍵字產品序號物料資訊
        /// <summary>關鍵字產品序號物料資訊</summary>
        public struct SerialMaterialInfo
        {
            /// <summary>序號</summary>
            public string IV_SERIAL { get; set; }
            /// <summary>料號</summary>
            public string ProdID { get; set; }
            /// <summary>料號說明</summary>
            public string Product { get; set; }            
            /// <summary>裝機號碼</summary>
            public string InstallNo { get; set; }
        }
        #endregion

        #region 保固SLA資訊
        /// <summary>保固SLA資訊</summary>
        public struct SRWarranty
        {
            /// <summary>系統ID</summary>
            public string cID { get; set; }
            /// <summary>序號</summary>
            public string cSerialID { get; set; }
            /// <summary>保固代號</summary>
            public string cWTYID { get; set; }
            /// <summary>保固說明</summary>
            public string cWTYName { get; set; }
            /// <summary>保固開始日期</summary>
            public string cWTYSDATE { get; set; }
            /// <summary>保固結束日期</summary>
            public string cWTYEDATE { get; set; }
            /// <summary>回應條件</summary>
            public string cSLARESP { get; set; }
            /// <summary>服務條件</summary>
            public string cSLASRV { get; set; }
            /// <summary>合約編號</summary>
            public string cContractID { get; set; }
            /// <summary>合約編號Url</summary>
            public string cContractIDUrl { get; set; }
            /// <summary>保固申請(BPM表單編號)</summary>
            public string cBPMFormNo { get; set; }
            /// <summary>保固申請Url(BPM表單編號Url)</summary>
            public string cBPMFormNoUrl { get; set; }
            /// <summary>客服主管建議</summary>
            public string cAdvice { get; set; }
            /// <summary>下包文件編號(CRM標的)</summary>
            public string cSUB_CONTRACTID { get; set; }
            /// <summary>本次使用</summary>
            public string cUsed { get; set; }
            /// <summary>tr背景顏色Class</summary>
            public string cBGColor { get; set; }
        }
        #endregion

        #region 服務團隊資訊
        /// <summary>服務團隊資訊</summary>
        public class SRTeamInfo
        {
            public int ID { get; private set; }
            public string TeamID { get; private set; }            
            public string TeamName { get; private set; }            
            public string DeptId { get; private set; }
            public string DeptName { get; private set; }

            public SRTeamInfo(int id, string teamid, string teamname, string deptId, string deptName)
            {
                ID = id;
                TeamID = teamid;
                TeamName = teamname;             
                DeptId = deptId;
                DeptName = deptName;
            }
        }
        #endregion

        #region 協助工程師資訊
        /// <summary>協助工程師資訊</summary>
        public class AssEngineerInfo
        {
            public int ID { get; private set; }
            public string Acc { get; private set; }
            public string Name { get; private set; }
            public string Ext { get; private set; }
            public string Mobile { get; private set; }
            public string Email { get; private set; }
            public string DeptId { get; private set; }
            public string DeptName { get; private set; }

            public AssEngineerInfo(int id, string acc, string name, string ext, string mobile, string email, string deptId, string deptName)
            {
                ID = id;
                Acc = acc;
                Name = name;
                Ext = ext;
                Mobile = mobile;
                Email = email;
                DeptId = deptId;
                DeptName = deptName;
            }
        }
        #endregion

        #region 技術主管資訊
        /// <summary>技術主管資訊</summary>
        public class TechManagerInfo
        {
            public int ID { get; private set; }
            public string Acc { get; private set; }
            public string Name { get; private set; }
            public string Ext { get; private set; }
            public string Mobile { get; private set; }
            public string Email { get; private set; }
            public string DeptId { get; private set; }
            public string DeptName { get; private set; }

            public TechManagerInfo(int id, string acc, string name, string ext, string mobile, string email, string deptId, string deptName)
            {
                ID = id;
                Acc = acc;
                Name = name;
                Ext = ext;
                Mobile = mobile;
                Email = email;
                DeptId = deptId;
                DeptName = deptName;
            }
        }
        #endregion

        #region 處理與工時紀錄資訊
        /// <summary>處理與工時紀錄資訊</summary>
        public class PjRecord
        {
            /// <summary>工時紀錄檔</summary>
            public TbOneSrdetailRecord Pr { get; set; }
            /// <summary>是否可以編輯</summary>
            public bool IsCrUser { get; set; }
        }
        #endregion

        #region 服務報告書內容(檔案)
        /// <summary>服務報告書內容(檔案)</summary>
        public struct PjRecordReport
        {
            /// <summary>附件ID(GUID)</summary>
            public string Id { get; set; }
            /// <summary>原始檔名</summary>
            public string OrgName { get; set; }
            /// <summary>GUID檔名</summary>
            public string GuidName { get; set; }
            /// <summary>副檔名</summary>
            public string Ext { get; set; }
            /// <summary>Url連結</summary>
            public string Url { get; set; }
            /// <summary>上傳時間</summary>
            public string InsertTime { get; set; }
            /// <summary>SRID</summary>
            public string SRID { get; set; }
            /// <summary>工時紀錄檔系統ID</summary>
            public string PjRecordId { get; set; }
            /// <summary>建立人姓名</summary>
            public string CrName { get; set; }            
        }
        #endregion        

        #endregion -----↑↑↑↑↑共用方法 ↑↑↑↑↑-----  
    }

    #region 更改歷史記錄相關資訊
    public class OneLogInfo
    {
        /// <summary>歷史記錄資訊(共用)</summary>
        public List<TbOneLog> SROneLog { get; set; }
        /// <summary>客戶聯絡人異動資訊(共用)</summary>
        public List<TbOneLog> SRContactChangLog { get; set; }

        /// <summary>客戶聯絡人資訊(共用)</summary>
        public List<TbOneSrdetailContact> SRDetailContact { get; set; }
        /// <summary>處理與工程紀錄資訊(共用)</summary>
        public List<TbOneSrdetailRecord> SRDetailRecord { get; set; }

        /// <summary>產品與序號資訊(一般)</summary>
        public List<TbOneSrdetailProduct> SRDetailProduct { get; set; }
        /// <summary>零件更換資訊(一般)</summary>
        public List<TbOneSrdetailPartsReplace> SRDetailPartsReplace { get; set; }

        /// <summary>物料訊息資訊(裝機)</summary>
        public List<TbOneSrdetailMaterialInfo> SRDetailMaterialInfo { get; set; }
        /// <summary>序號回報資訊(裝機)</summary>
        public List<TbOneSrdetailSerialFeedback> SRDetailSerialFeedback { get; set; }
    }
    #endregion

    #region 物料相關資訊
    /// <summary>物料相關資訊</summary>
    public struct MaterialInfo
    {        
        /// <summary>料號</summary>
        public string MaterialID { get; set; }
        /// <summary>料號說明</summary>
        public string MaterialName { get; set; }
        /// <summary>製造商零件號碼</summary>
        public string MFPNumber { get; set; }
        /// <summary>基本內文</summary>
        public string BasicContent { get; set; }
        /// <summary>廠牌</summary>
        public string Brand { get; set; }
        /// <summary>廠品階層</summary>
        public string ProductHierarchy { get; set; }        
    }
    #endregion

    #region 取得系統位址參數相關資訊
    public class SRSYSPARAINFO
    {
        /// <summary>呼叫SAPERP參數是正式區或測試區(true.正式區 false.測試區)</summary>
        public bool IsFormal { get; set; }        
        /// <summary>BPM URL</summary>
        public string BPMURLName { get; set; }
        /// <summary>PSIP URL</summary>
        public string PSIPURLName { get; set; }
        /// <summary>API URL</summary>
        public string APIURLName { get; set; }
        /// <summary>附件URL</summary>
        public string AttachURLName { get; set; }
    }
    #endregion

    #region 取得附件相關資訊
    public class SRATTACHINFO
    {
        /// <summary>附件GUID</summary>
        public string ID { get; set; }
        /// <summary>附件原始檔名(含副檔名)</summary>
        public string FILE_ORG_NAME { get; set; }
        /// <summary>附件檔名(GUID，含副檔名)</summary>
        public string FILE_NAME { get; set; }
        /// <summary>副檔名</summary>
        public string FILE_EXT { get; set; }
        /// <summary>附件檔案路徑URL</summary>
        public string FILE_URL { get; set; }
        /// <summary>新增日期</summary>
        public string INSERT_TIME { get; set; }
    }
    #endregion   

    #region 服務案件執行條件
    /// <summary>
    /// 服務案件執行條件
    /// </summary>
    public enum SRCondition
    {
        /// <summary>
        /// 新建
        /// </summary>
        ADD,

        /// <summary>
        /// 轉派主要工程師
        /// </summary>
        TRANS,

        /// <summary>
        /// 駁回
        /// </summary>
        REJECT,

        /// <summary>
        /// HPGCSN申請
        /// </summary>
        HPGCSN,

        /// <summary>
        /// HPGCSN完成
        /// </summary>
        HPGCSNDONE,

        /// <summary>
        /// 二修
        /// </summary>
        SECFIX,

        /// <summary>
        /// 保存
        /// </summary>
        SAVE,

        /// <summary>
        /// 技術支援升級
        /// </summary>
        SUPPORT,

        /// <summary>
        /// 3 Party
        /// </summary>
        THRPARTY,

        /// <summary>
        /// 取消
        /// </summary>
        CANCEL,

        /// <summary>
        /// 完修
        /// </summary>
        DONE
    }
    #endregion

    #region 服務案件(一般/裝機/定維)狀態更新INPUT資訊
    /// <summary>服務案件(一般/裝機/定維)狀態更新INPUT資訊</summary>
    public struct SRMain_SRSTATUS_INPUT
    {
        /// <summary>修改者員工編號ERPID</summary>
        public string IV_LOGINEMPNO { get; set; }
        /// <summary>修改者員工姓名(中文+英文)</summary>
        public string IV_LOGINEMPNAME { get; set; }
        /// <summary>服務案件ID</summary>
        public string IV_SRID { get; set; }
        /// <summary>服務狀態ID</summary>
        public string IV_STATUS { get; set; }
        /// <summary>APIURL開頭網址</summary>
        public string IV_APIURLName { get; set; }
    }
    #endregion

    #region 服務案件(一般/裝機/定維)狀態更新OUTPUT資訊
    /// <summary>服務案件(一般/裝機/定維)狀態更新OUTPUT資訊</summary>
    public struct SRMain_SRSTATUS_OUTPUT
    {
        /// <summary>消息類型(E.處理失敗 Y.處理成功)</summary>
        public string EV_MSGT { get; set; }
        /// <summary>消息內容</summary>
        public string EV_MSG { get; set; }
    }
    #endregion
}
