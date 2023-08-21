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
using Org.BouncyCastle.Bcpg.OpenPgp;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;
using MathNet.Numerics;
using NPOI.OpenXmlFormats.Wordprocessing;
using MathNet.Numerics.Optimization;
using Org.BouncyCastle.Crypto.Prng;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula.PTG;
using Org.BouncyCastle.Utilities.Net;
using NPOI.HPSF;
using Microsoft.Extensions.Hosting;
using NPOI.XSSF.UserModel;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

namespace OneService.Controllers
{
    public class ServiceRequestController : Controller
    {
		private readonly IWebHostEnvironment _HostEnvironment;

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
        /// 登入者是否為備品管理員或檢測員(true.是 false.否)
        /// </summary>
        bool pIsSpareManager = false;

        /// <summary>
        /// 登入者是否為管理者(true.是 false.否)
        /// </summary>
        bool pIsManager = false;

        /// <summary>
        /// 登入者是否為批次上傳裝機備料服務通知單、合約書文件人員(true.是 false.否)
        /// </summary>
        bool pIsBatchUploadSecretary = false;

        /// <summary>
        /// 登入者是否為批次上傳裝機派工人員(true.是 false.否)
        /// </summary>
        bool pIsExePerson = false;

        /// <summary>
        /// 登入者是否為批次上傳定維派工人員(true.是 false.否)
        /// </summary>
        bool pIsExeMaintainPerson = false;

        /// <summary>
        /// 登入者是否可編輯服務案件
        /// </summary>
        bool pIsCanEditSR = false;

        /// <summary>
        /// 服務ID
        /// </summary>
        string pSRID = string.Empty;

        /// <summary>
        /// 複製的來源服務ID
        /// </summary>
        string pCopySRID = string.Empty;

        /// <summary>
        /// 批次上傳類型(裝機備料服務通知單/合約書文件)
        /// </summary>
        string pBatchUploadType = string.Empty;

        /// <summary>
        /// 批次上傳類型說明
        /// </summary>
        string pBatchUploadTypeNote = string.Empty;

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
        /// 程式作業編號檔系統ID(批次上傳裝機備料服務通知單、合約書文件)
        /// </summary>
        static string pOperationID_BatchUploadStockNo = "BB3DD376-969A-4518-B9C2-8BFF431148BE";

        /// <summary>
        /// 程式作業編號檔系統ID(批次上傳裝機派工作業)
        /// </summary>
        static string pOperationID_QueryBatchInstall = "3BF8ED29-3639-49D2-8D4A-19F9C1FF7934";

        /// <summary>
        /// 程式作業編號檔系統ID(批次上傳定維派工作業)
        /// </summary>
        static string pOperationID_QueryBatchMaintain = "7F07161D-D086-4004-AB25-B292469C979C";

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

		public ServiceRequestController(IWebHostEnvironment hostingEnvironment)
		{
			_HostEnvironment = hostingEnvironment;
		}

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
            getEmployeeInfo();

            var model = new ViewModelSRReport();

            #region 服務案件類型
            var SRTypeList = new List<SelectListItem>()
            {
                new SelectListItem {Text="ZSR1_一般服務", Value="ZSR1" },
                new SelectListItem {Text="ZSR3_裝機服務", Value="ZSR3" },
                new SelectListItem {Text="ZSR5_定維服務", Value="ZSR5" },                
            };

            ViewBag.ddl_SRType = SRTypeList;
            #endregion

            #region 服務團隊(新)
            var SRTeamIDList = CMF.findSRTeamIDList("ALL", true);
            ViewBag.SRTeamIDList = SRTeamIDList;
            #endregion

            #region 服務團隊(舊)
            var SRTeamOldIDList = CMF.findSRTeamOldIDList(pOperationID_GenerallySR, "ALL", false);
            ViewBag.ddl_TeamOldID = SRTeamOldIDList;
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

            #region 建立日期
            ViewBag.cStartCreatedDate = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-01");
            ViewBag.cEndCreatedDate = DateTime.Now.ToString("yyyy-MM-dd");
            #endregion

            return View(model);
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
        /// <param name="cDesc">說明</param>
        /// <param name="CustomerID">客戶代號</param>
        /// <param name="RepairName">報修人</param>
        /// <param name="RepairAddress">報修人地址</param>        
        /// <param name="SerialID">序號</param>
        /// <param name="PID">機器型號</param>
        /// <param name="TeamID">服務團隊(新)</param>
        /// <param name="TeamOldID">服務團隊(舊)</param>
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
        /// <param name="cIsSecondFix">是否二修</param>
        /// <returns></returns>
        public IActionResult SRReportResult(string StartCreatedDate, string EndCreatedDate, string StartFinishTime, string EndFinishTime, string SRID, string cDesc,
                                          string CustomerID, string RepairName, string RepairAddress, string SerialID, string PID, string TeamID, string TeamOldID,
                                          string Status, string SRType, string EngineerID, string ContractID, string SO, string XCHP, string HPCT, 
                                          string MaterialID, string MaterialName, string OLDCT, string NEWCT, string cSRTypeOne, string cSRTypeSec, string cSRTypeThr, string cIsSecondFix)
        {
            StringBuilder tSQL = new StringBuilder();

            bool tIsFormal = false;
            string tTop = string.Empty;                 //是否有輸入說明，若有就要限制前1,000筆
            string ttWhere = string.Empty;
            string ttStrItem = string.Empty;
            string tONEURLName = string.Empty;
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
            string cIsShowLink = string.Empty;
            string UrlFront = string.Empty;

            getLoginAccount();
            getEmployeeInfo();

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tONEURLName = ParaBean.ONEURLName;
            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;
            #endregion

            UrlFront = tONEURLName + "/files/";

            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            #region 建立日期
            if (!string.IsNullOrEmpty(StartCreatedDate))
            {
                ttWhere += "AND CreatedDate >= N'" + StartCreatedDate.Replace("/", "-") + " 00:00:00' ";
            }

            if (!string.IsNullOrEmpty(EndCreatedDate))
            {
                ttWhere += "AND CreatedDate <= N'" + EndCreatedDate.Replace("/", "-") + " 23:59:59' ";
            }
            #endregion

            #region 完成時間
            if (!string.IsNullOrEmpty(StartFinishTime))
            {
                ttWhere += "AND cFinishTime >= N'" + StartFinishTime.Replace("/", "-") + " 00:00:00' ";
            }

            if (!string.IsNullOrEmpty(EndFinishTime))
            {
                ttWhere += "AND cFinishTime <= N'" + EndFinishTime.Replace("/", "-") + " 23:59:59' ";
            }
            #endregion

            #region SRID
            if (!string.IsNullOrEmpty(SRID))
            {
                ttWhere += "AND cSRID LIKE N'%" + SRID + "%' " + Environment.NewLine;
            }
            #endregion

            #region 說明
            if (!string.IsNullOrEmpty(cDesc))
            {
                tTop = " TOP 1000 ";
                ttWhere += "AND cDesc LIKE N'%" + cDesc.Trim() + "%' " + Environment.NewLine;
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

            #region 服務團隊(新)
            if (!string.IsNullOrEmpty(TeamID))
            {
                ttStrItem = "";
                string[] tAryTeam = TeamID.TrimEnd(';').Split(';');

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

            #region 服務團隊(舊)
            if (!string.IsNullOrEmpty(TeamOldID))
            {
                ttStrItem = "";
                string[] tAryTeam = TeamOldID.TrimEnd(',').Split(',');

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
                string[] AryAss = EngineerID.Split(';');

                ttStrItem = "";

                foreach (string Ass in AryAss)
                {
                    ttStrItem += "N'" + Ass + "',";
                }

                ttWhere += "AND cEngineerID IN (" + ttStrItem.TrimEnd(',') + ") " + Environment.NewLine;
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

            #region 是否為二修
            if (!string.IsNullOrEmpty(cIsSecondFix))
            {
                if (cIsSecondFix == "N")
                {
                    ttWhere += "AND (cIsSecondFix = '" + cIsSecondFix.Trim() + "' or cIsSecondFix = '') " + Environment.NewLine;
                }
                else
                {
                    ttWhere += "AND cIsSecondFix = '" + cIsSecondFix.Trim() + "' " + Environment.NewLine;
                }
            }
            #endregion

            #region 組待查詢清單

            #region SQL語法
            tSQL.AppendLine(" Select " + tTop + " *");
            tSQL.AppendLine(" From VIEW_ONE_SRREPORT ");            
            tSQL.AppendLine(" Where 1=1 " + ttWhere);
            #endregion

            DataTable dt = CMF.getDataTableByDb(tSQL.ToString(), "dbOne");

            var tSRTeam_List = CMF.findSRTeamIDList("ALL", false);

            foreach (DataRow dr in dt.Rows)
            {
                string[] QueryInfo = new string[59];

                CreatedDate = string.IsNullOrEmpty(dr["CreatedDate"].ToString()) ? "" : Convert.ToDateTime(dr["CreatedDate"].ToString()).ToString("yyyy-MM-dd");
                tSRIDUrl = CMF.findSRIDUrl(dr["cSRID"].ToString());

                if (dr["cTeamID"].ToString().Length >= 14)
                {
                    tSRTeam = dr["cTeamID"].ToString();
                }
                else
                {
                    tSRTeam = CMF.TransSRTeam(tSRTeam_List, dr["cTeamID"].ToString());
                }

                if (dr["cSRID"].ToString().Substring(0,1) == "6") //新版SR才要顯示連結
                {
                    cIsShowLink = "Y";
                }
                else
                {
                    cIsShowLink = "N";
                }

                cNotes = dr["cNotes"].ToString().Replace("\r\n", "<br/>");
                cReceiveTime = Convert.ToDateTime(dr["cReceiveTime"].ToString()) == Convert.ToDateTime("1900-01-01") ? "" : Convert.ToDateTime(dr["cReceiveTime"].ToString()).ToString("yyyy-MM-dd HH:mm");
                cStartTime = Convert.ToDateTime(dr["cStartTime"].ToString()) == Convert.ToDateTime("1900-01-01") ? "" : Convert.ToDateTime(dr["cStartTime"].ToString()).ToString("yyyy-MM-dd HH:mm");
                cArriveTime = Convert.ToDateTime(dr["cArriveTime"].ToString()) == Convert.ToDateTime("1900-01-01") ? "" : Convert.ToDateTime(dr["cArriveTime"].ToString()).ToString("yyyy-MM-dd HH:mm");
                cFinishTime = Convert.ToDateTime(dr["cFinishTime"].ToString()) == Convert.ToDateTime("1900-01-01") ? "" : Convert.ToDateTime(dr["cFinishTime"].ToString()).ToString("yyyy-MM-dd HH:mm");
                cWorkHours = dr["cWorkHours"].ToString() == "0" ? "" : dr["cWorkHours"].ToString();
                
                cSRReportURL = CMF.findAttachUrl(dr["cSRReport"].ToString(), tAttachURLName);
                cSRReportURL = cSRReportURL.Replace("http://tsticrmmbgw.etatung.com:8081/CSreport/", UrlFront).Replace("http://tsticrmmbgw.etatung.com:8082/CSreport/", UrlFront);

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
                QueryInfo[58] = cIsShowLink;                    //是否要顯示Link

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #region 傳入下載的檔案名稱並轉換成原檔檔名
        /// <summary>
        /// 傳入下載的檔案名稱並轉換成原檔檔名
        /// </summary>
        /// <param name="fileName">檔案名稱</param>
        /// <returns></returns>
        public string getSRReportName(string fileName)
        {
            string reValue = string.Empty;
            string GuidKey = string.Empty;

            if (!string.IsNullOrEmpty(fileName))
            {
                #region 取得系統位址參數相關資訊
                SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);
                string tAttachURLName = ParaBean.AttachURLName;
                #endregion                

                string[] AryName = fileName.Split('.');
                GuidKey = AryName[0];

                if (!string.IsNullOrEmpty(GuidKey))
                {
                    var beanSR = CMF.findSingleSRATTACHINFO(GuidKey, tAttachURLName);

                    if (!string.IsNullOrEmpty(beanSR.FILE_ORG_NAME))
                    {
                        reValue = beanSR.FILE_ORG_NAME;
                    }
                }

                if (string.IsNullOrEmpty(reValue)) //若沒有抓到檔名，代表是舊的檔案，就取原檔名
                {
                    reValue = fileName;
                }
            }

            return reValue;
        }

        [HttpGet]
        public async Task<IActionResult> findSRReportName(string filePath)
        {
            var memory = new MemoryStream();
            string webRootPath = _HostEnvironment.WebRootPath;

            string TempfilePath = filePath.Replace("https://oneservice.etatung.com", "").Replace("http://172.31.7.56:32200", "");
            string fileName = filePath.Replace("https://oneservice.etatung.com/files/", "").Replace("http://172.31.7.56:32200/files/", "");

            var uploads = Path.Combine(webRootPath + TempfilePath);
            using (var stream = new FileStream(uploads, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            string tReportName = getSRReportName(fileName);

            return File(memory, "application/octet-stream", tReportName);
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
            getEmployeeInfo();

            var model = new ViewModelQuerySRProgress();

            #region 服務案件種類
            var ListSRCaseType = findSRIDTypeList(false);
            ViewBag.ddl_cSRCaseType = ListSRCaseType;
            #endregion

            #region 狀態
            List<SelectListItem> ListStatus = new List<SelectListItem>();

            ListStatus.Add(new SelectListItem("UNDONE_未完成", "UNDONE"));
            ListStatus.Add(new SelectListItem("ALLDONE_已完成", "ALLDONE"));

            List<SelectListItem> ListTempStatus = CMF.findSRStatus(pOperationID_GenerallySR, pOperationID_InstallSR, pOperationID_MaintainSR, pCompanyCode);

            foreach(var tListItem in ListTempStatus)
            {
                ListStatus.Add(tListItem);
            }
            
            ViewBag.ddl_cStatus = ListStatus;
            #endregion

            #region 報修管道
            var ListSRPathWay = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "SRPATH", false);
            ViewBag.ddl_cSRPathWay = ListSRPathWay;
            #endregion

            #region 取得服務團隊清單
            var SRTeamIDList = CMF.findSRTeamIDList("ALL", true);
            ViewBag.SRTeamIDList = SRTeamIDList;
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
        /// <param name="cDesc">說明</param>
        /// <param name="cIsSecondFix">是否為二修</param>
        /// <param name="CreatedUserName">派單人員</param>
        /// <param name="cRepairName">報修人姓名</param>
        /// <param name="cSRPathWay">報修管道</param>      
        /// <param name="cAssEngineerID">工程師ERPID</param>
        /// <param name="cTechManagerID">技術主管ERPID</param>
        /// <param name="cTeamID">服務團隊</param>
        /// <param name="cContractID">合約文件編號</param>
        /// <param name="cSRTypeOne">報修類別-大類</param>
        /// <param name="cSRTypeSec">報修類別-中類</param>
        /// <param name="cSRTypeThr">報修類別-小類</param>
        /// <param name="cSerialID">報修/裝機序號</param>
        /// <param name="cMaterialName">報修機器型號/裝機料號說明</param>
        /// <param name="cProductNumber">報修Product Number/裝機料號</param>
        /// <returns></returns>
        public IActionResult QuerySRProgressResult(string cCompanyID, string cSRCaseType, string cStatus, string cStartCreatedDate, string cEndCreatedDate,
                                                 string cCustomerID, string cCustomerName, string cSRID, string cDesc, string cIsSecondFix, string CreatedUserName, string cRepairName, string cSRPathWay,
                                                 string cAssEngineerID, string cTechManagerID, string cTeamID, string cContractID, string cSRTypeOne, string cSRTypeSec, string cSRTypeThr,
                                                 string cSerialID, string cMaterialName, string cProductNumber)
        {            
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            DataTable dt = null;
            DataTable dtProgress = null;

            StringBuilder tSQL = new StringBuilder();            

            string tTop = string.Empty;                 //是否有輸入說明，若有就要限制前1,000筆
            string ttWhere = string.Empty;
            string ttWhere2 = string.Empty;
            string ttJoin = string.Empty;
            string ttJoin2 = string.Empty;
            string ttUnion = string.Empty;
            string ttStrItem = string.Empty;
            string tTempERPID = string.Empty;            
            string tSRIDUrl = string.Empty;             //服務案件URL
            string tSRContactName = string.Empty;       //客戶聯絡人
            string tSRPathWayNote = string.Empty;       //報修管道
            string tSTATUSDESC = string.Empty;          //狀態
            string tSRType = string.Empty;              //報修類別
            string tSRProductSerial = string.Empty;     //報修產品序號資訊
            string tSRTeam = string.Empty;              //服務團隊
            string tMainEngineerID = string.Empty;      //主要工程師ERPID
            string tMainEngineerName = string.Empty;    //主要工程師姓名
            string tAssEngineerName = string.Empty;     //協助工程師姓名
            string tTechManagerName = string.Empty;     //技術主管姓名
            string tCreatedUserName = string.Empty;     //派單人員
            string tCreatedDate = string.Empty;         //派單日期
            string tModifiedDate = string.Empty;        //最後編輯日期                          

            List<TbOneSrdetailSerialFeedback> tListSerialFeedback = new List<TbOneSrdetailSerialFeedback>();     //記錄SerialFeedbacks(服務明細-序號回報檔)清單
            List<string> tListAssAndTech = new List<string>();                                              //記錄所有協助工程師和所有技術主管的ERPID
            Dictionary<string, string> tDicAssAndTech = new Dictionary<string, string>();                      //記錄所有協助工程師和所有技術主管的<ERPID,中、英文姓名>

            var tSRTeam_List = CMF.findSRTeamIDList("ALL", false);
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
                    ttStrItem += " M.cSRID like N'" + tSRCaseType + "%' or";
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
                    if (ttStrItem.IndexOf("UNDONE") != -1) //未完成
                    {
                        ttWhere += "AND M.cStatus NOT IN ('E0006','E0010','E0017','E0015') " + Environment.NewLine;
                    }

                    if (ttStrItem.IndexOf("ALLDONE") != -1) //已完成
                    {
                        ttWhere += "AND M.cStatus IN ('E0006','E0010','E0017') " + Environment.NewLine;
                    }

                    if (ttStrItem.IndexOf("UNDONE") == -1 && ttStrItem.IndexOf("ALLDONE") == -1)
                    {
                        ttWhere += "AND M.cStatus IN (" + ttStrItem + ") " + Environment.NewLine;
                    }
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

            #region 說明
            if (!string.IsNullOrEmpty(cDesc))
            {
                tTop = " TOP 1000 ";
                ttWhere += "AND M.cDesc LIKE N'%" + cDesc.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 是否為二修
            if (!string.IsNullOrEmpty(cIsSecondFix))
            {
                if (cIsSecondFix == "N")
                {
                    ttWhere += "AND (M.cIsSecondFix = '" + cIsSecondFix.Trim() + "' or  M.cIsSecondFix = '') " + Environment.NewLine;
                }
                else
                {
                    ttWhere += "AND M.cIsSecondFix = '" + cIsSecondFix.Trim() + "' " + Environment.NewLine;
                }
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

            #region 主要/協助工程師ERPID
            if (!string.IsNullOrEmpty(cAssEngineerID))
            {
                string[] AryAss = cAssEngineerID.Split(';');                

                #region 先組主要工程師
                ttStrItem = "";              

                foreach (string Ass in AryAss)
                {
                    ttStrItem += "N'" + Ass + "',";
                }

                ttWhere += "AND (M.cMainEngineerID IN (" + ttStrItem.TrimEnd(',') + ") or ";
                #endregion

                #region 再組協助工程師
                char[] tchar = new char[2];
                tchar[0] = 'o';
                tchar[1] = 'r';

                string AssWhere = "(";

                foreach (string Ass in AryAss)
                {
                    AssWhere += " M.cAssEngineerID LIKE N'%" + Ass + "%' or";
                }

                AssWhere = AssWhere.TrimEnd(tchar) + ")) ";
                ttWhere += AssWhere + Environment.NewLine;
                #endregion
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
                string[] tAryTeam = cTeamID.TrimEnd(';').Split(';');

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

            #region 合約文件編號
            if (!string.IsNullOrEmpty(cContractID))
            {
                ttWhere += "AND M.cContractID LIKE N'%" + cContractID.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 將ttWhere複製到ttWhere2，要給裝機join用
            if (ttWhere != "")
            {
                ttWhere2 = ttWhere;
            }
            #endregion

            #region 報修/裝機序號
            if (!string.IsNullOrEmpty(cSerialID))
            {
                ttWhere += "AND (P.cSerialID LIKE N'%" + cSerialID.Trim() + "%' or P.cNewSerialID LIKE N'%" + cSerialID.Trim() + "%') " + Environment.NewLine;
                ttWhere2 += "AND P.cSerialID LIKE N'%" + cSerialID.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 報修機器型號/裝機料號說明
            if (!string.IsNullOrEmpty(cMaterialName))
            {
                ttWhere += "AND P.cMaterialName LIKE N'%" + cMaterialName.Trim() + "%' " + Environment.NewLine;
                ttWhere2 += "AND P.cMaterialName LIKE N'%" + cMaterialName.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 報修Product Number/裝機料號
            if (!string.IsNullOrEmpty(cProductNumber))
            {
                ttWhere += "AND P.cProductNumber LIKE N'%" + cProductNumber.Trim() + "%' " + Environment.NewLine;
                ttWhere2 += "AND P.cMaterialID LIKE N'%" + cProductNumber.Trim() + "%' " + Environment.NewLine;
            }
            #endregion

            #region 若【報修/裝機序號】、【報修機器型號/裝機料號說明】、【報修Product Number/裝機料號】其中有一個，就要執行Join語法和Union的語法
            if (!string.IsNullOrEmpty(cSerialID) || !string.IsNullOrEmpty(cMaterialName) || !string.IsNullOrEmpty(cProductNumber))
            {
                ttJoin = " left join TB_ONE_SRDetail_Product P on M.cSRID = P.cSRID";
                ttWhere = "AND P.disabled = 0 " + ttWhere;

                ttJoin2 = " left join TB_ONE_SRDetail_SerialFeedback P on M.cSRID = P.cSRID";
                ttWhere2 = "AND P.disabled = 0 " + ttWhere2;

                #region 組UNION ALL SQL語法(join裝機)
                tSQL.AppendLine(" UNION ALL");
                tSQL.AppendLine(" Select " + tTop + " M.*, '' as Feedbacks");                
                tSQL.AppendLine(" From TB_ONE_SRMain M");
                tSQL.AppendLine(ttJoin2);
                tSQL.AppendLine(" Where 1=1 " + ttWhere2);

                ttUnion = tSQL.ToString();
                #endregion
            }
            #endregion

            #region 組待查詢清單

            #region SQL語法
            tSQL = new StringBuilder();
            tSQL.AppendLine(" Select " + tTop + " M.*,");
            tSQL.AppendLine("        (Select top 1");
            tSQL.AppendLine("            case when sp.cNewSerialID <> '' then sp.cSerialID + '(更換後序號：' + sp.cNewSerialID + ')' + '＃＃' + sp.cMaterialName + '＃＃' + sp.cProductNumber");
            tSQL.AppendLine("            else sp.cSerialID + '＃＃' + sp.cMaterialName + '＃＃' + sp.cProductNumber end");            
            tSQL.AppendLine("         From TB_ONE_SRDetail_Product sp where M.cSRID = sp.cSRID AND sp.disabled = 0");
            tSQL.AppendLine("        ) as Products");
            tSQL.AppendLine(" From TB_ONE_SRMain M");
            tSQL.AppendLine(ttJoin);
            tSQL.AppendLine(" Where 1=1 " + ttWhere);
            tSQL.AppendLine(ttUnion);
            #endregion

            dt = CMF.getDataTableByDb(tSQL.ToString(), "dbOne");            
            dtProgress = CMF.DistinctTable(dt);

            #region 取得所有SRID清單裡的序號回報檔清單
            List<string> SridList = new List<string>();
            foreach (DataRow dr in dtProgress.Rows)
            {
                SridList.Add(dr["cSRID"].ToString());
            }

            tListSerialFeedback = CMF.findSRSerialFeedbackList(SridList);
            #endregion

            #region 取得所有協助工程師和技術主管的ERPID
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
                tSRProductSerial = CMF.TransProductSerial(dr["cSRID"].ToString(), dr["Products"].ToString(), tListSerialFeedback);
                tMainEngineerID = string.IsNullOrEmpty(dr["cMainEngineerID"].ToString()) ? "" : dr["cMainEngineerID"].ToString();
                tMainEngineerName = string.IsNullOrEmpty(dr["cMainEngineerName"].ToString()) ? "" : dr["cMainEngineerName"].ToString();
                tAssEngineerName = CMF.TransEmployeeName(tDicAssAndTech, dr["cAssEngineerID"].ToString());
                tTechManagerName = CMF.TransEmployeeName(tDicAssAndTech, dr["cTechManagerID"].ToString());
                tCreatedUserName = string.IsNullOrEmpty(dr["CreatedUserName"].ToString()) ? "" : dr["CreatedUserName"].ToString();
                tCreatedDate = string.IsNullOrEmpty(dr["CreatedDate"].ToString()) ? "" : Convert.ToDateTime(dr["CreatedDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                tModifiedDate = string.IsNullOrEmpty(dr["ModifiedDate"].ToString()) ? "" : Convert.ToDateTime(dr["ModifiedDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");

                string[] QueryInfo = new string[20];                

                QueryInfo[0] = dr["cSRID"].ToString();          //SRID
                QueryInfo[1] = tSRIDUrl;                       //服務案件URL
                QueryInfo[2] = dr["cCustomerName"].ToString();   //客戶
                QueryInfo[3] = dr["cRepairName"].ToString();     //客戶報修人
                QueryInfo[4] = tSRContactName;                 //客戶聯絡人
                QueryInfo[5] = dr["cDesc"].ToString();          //說明
                QueryInfo[6] = dr["cDelayReason"].ToString();    //延遲結案原因
                QueryInfo[7] = tSRProductSerial;               //報修/裝機序號資訊
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
                QueryInfo[19] = dr["cContractID"].ToString();    //合約文件編號

                QueryToList.Add(QueryInfo);
            }

            QueryToList = QueryToList.OrderByDescending(x => x[16]).ToList();
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
                getEmployeeInfo();

                #region 一般服務

                //取得登入人員所負責的服務團隊
                List<string> tSRTeamList = CMF.findSRTeamMappingList(ViewBag.cLoginUser_CostCenterID, ViewBag.cLoginUser_DepartmentNO);

                //取得登入人員所有要負責的SRID                
                List<string[]> SRIDToDoList = CMF.findSRIDList(pOperationID_GenerallySR, pOperationID_InstallSR, pOperationID_MaintainSR, pCompanyCode, pIsManager, ViewBag.cLoginUser_ERPID, tSRTeamList);

                ViewBag.SRIDToDoList = SRIDToDoList;
                #endregion
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "失敗原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();

                CMF.writeToLog(pSRID, "ToDoList", pMsg, ViewBag.empEngName);
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
            getEmployeeInfo();

            var model = new ViewModel();

            #region Request參數            
            if (HttpContext.Request.Query["SRID"].FirstOrDefault() != null)
            {
                pSRID = HttpContext.Request.Query["SRID"].FirstOrDefault();
            }

            ViewBag.CopySRID = "";

            if (HttpContext.Request.Query["CopySRID"].FirstOrDefault() != null)
            {
                pCopySRID = HttpContext.Request.Query["CopySRID"].FirstOrDefault();
                
                ViewBag.CopySRID = pCopySRID;
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
            var SRTeamIDList = CMF.findSRTeamIDList("ALL", true);           
            #endregion

            if (!string.IsNullOrEmpty(pCopySRID))
            {
                #region 取得複製來源的SRID
                var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pCopySRID);

                if (beanM != null)
                {
                    #region 報修資訊                    
                    ViewBag.cSRID = "";
                    ViewBag.cCustomerID = beanM.CCustomerId;
                    ViewBag.cCustomerName = beanM.CCustomerName;
                    ViewBag.cDesc = beanM.CDesc;
                    ViewBag.cNotes = beanM.CNotes;
                    ViewBag.cAttachement = "";
                    ViewBag.cSRPathWay = "Z05";                         //手動建立
                    ViewBag.cMAServiceType = beanM.CMaserviceType;
                    ViewBag.cSRProcessWay = "";
                    ViewBag.cSRRepairLevel = "Z03";                     //三級(一般叫修)
                    ViewBag.cDelayReason = "";
                    ViewBag.cIsSecondFix = "";
                    ViewBag.cIsInternalWork = "N";
                    ViewBag.pStatus = "E0001";                          //新建
                    ViewBag.CreatedUserName = "";

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

                    #region 客戶故障狀況分類資訊(L2處理中)

                    #region 客戶故障狀況分類
                    ViewBag.cFaultGroup = "";
                    ViewBag.cFaultGroup1 = "";
                    ViewBag.cFaultGroup2 = "";
                    ViewBag.cFaultGroup3 = "";
                    ViewBag.cFaultGroup4 = "";
                    ViewBag.cFaultGroupOther = "";

                    ViewBag.cFaultGroupNote1 = "";
                    ViewBag.cFaultGroupNote2 = "";
                    ViewBag.cFaultGroupNote3 = "";
                    ViewBag.cFaultGroupNote4 = "";
                    ViewBag.cFaultGroupNoteOther = "";                    
                    #endregion

                    #region 客戶故障狀況
                    ViewBag.cFaultState = "";
                    ViewBag.cFaultState1 = "";
                    ViewBag.cFaultState2 = "";
                    ViewBag.cFaultStateOther = "";

                    ViewBag.cFaultStateNote1 = "";
                    ViewBag.cFaultStateNote2 = "";
                    ViewBag.cFaultStateNoteOther = "";                    
                    #endregion

                    #region 故障零件規格料號
                    ViewBag.cFaultSpec = "";
                    ViewBag.cFaultSpec1 = "";
                    ViewBag.cFaultSpec2 = "";
                    ViewBag.cFaultSpecOther = "";

                    ViewBag.cFaultSpecNote1 = "";
                    ViewBag.cFaultSpecNote2 = "";
                    ViewBag.cFaultSpecNoteOther = "";                    
                    #endregion

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

                    #region 取得客戶聯絡人資訊(明細)
                    var beansContact = dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pCopySRID);

                    ViewBag.Detailbean_Contact = beansContact;
                    ViewBag.trContactNo = beansContact.Count();
                    #endregion

                    #region 取得產品序號資訊(明細)
                    var beansProduct = dbOne.TbOneSrdetailProducts.OrderBy(x => x.CSerialId).Where(x => x.Disabled == 0 && x.CSrid == pCopySRID);

                    ViewBag.Detailbean_Product = beansProduct;
                    ViewBag.trProductNo = beansProduct.Count();
                    #endregion                   
                }                
                #endregion
            }
            else
            {
                #region 取得SRID
                var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pSRID);

                if (beanM != null)
                {
                    //記錄目前GUID，用來判斷更新的先後順序
                    ViewBag.pGUID = beanM.CSystemGuid.ToString();

                    //判斷登入者是否可以編輯服務案件                
                    pIsCanEditSR = CMF.checkIsCanEditSR(beanM.CSrid, ViewBag.cLoginUser_ERPID, ViewBag.cLoginUser_CostCenterID, ViewBag.cLoginUser_DepartmentNO, pIsMIS, pIsCS, pIsSpareManager);

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
                    ViewBag.cSRRepairLevel = beanM.CSrrepairLevel;
                    ViewBag.cDelayReason = beanM.CDelayReason;
                    ViewBag.cIsSecondFix = beanM.CIsSecondFix;
                    ViewBag.cIsInternalWork = beanM.CIsInternalWork;
                    ViewBag.pStatus = beanM.CStatus;
                    ViewBag.CreatedUserName = beanM.CreatedUserName;

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

                    #region 客戶故障狀況分類資訊(L2處理中)

                    #region 客戶故障狀況分類
                    ViewBag.cFaultGroup = "";
                    ViewBag.cFaultGroup1 = "";
                    ViewBag.cFaultGroup2 = "";
                    ViewBag.cFaultGroup3 = "";
                    ViewBag.cFaultGroup4 = "";
                    ViewBag.cFaultGroupOther = "";

                    ViewBag.cFaultGroupNote1 = "";
                    ViewBag.cFaultGroupNote2 = "";
                    ViewBag.cFaultGroupNote3 = "";
                    ViewBag.cFaultGroupNote4 = "";
                    ViewBag.cFaultGroupNoteOther = "";

                    if (!string.IsNullOrEmpty(beanM.CFaultGroup))
                    {
                        ViewBag.cFaultGroup = beanM.CFaultGroup;

                        string[] tAryGroup = beanM.CFaultGroup.Split(';');

                        foreach (string tValue in tAryGroup)
                        {
                            switch (tValue)
                            {
                                case "Z01": //硬體
                                    ViewBag.cFaultGroup1 = "Y";
                                    ViewBag.cFaultGroupNote1 = beanM.CFaultGroupNote1;
                                    break;
                                case "Z02": //系統
                                    ViewBag.cFaultGroup2 = "Y";
                                    ViewBag.cFaultGroupNote2 = beanM.CFaultGroupNote2;
                                    break;
                                case "Z03": //服務
                                    ViewBag.cFaultGroup3 = "Y";
                                    ViewBag.cFaultGroupNote3 = beanM.CFaultGroupNote3;
                                    break;
                                case "Z04": //網路
                                    ViewBag.cFaultGroup4 = "Y";
                                    ViewBag.cFaultGroupNote4 = beanM.CFaultGroupNote4;
                                    break;
                                case "Z99": //其他
                                    ViewBag.cFaultGroupOther = "Y";
                                    ViewBag.cFaultGroupNoteOther = beanM.CFaultGroupNoteOther;
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region 客戶故障狀況
                    ViewBag.cFaultState = "";
                    ViewBag.cFaultState1 = "";
                    ViewBag.cFaultState2 = "";
                    ViewBag.cFaultStateOther = "";

                    ViewBag.cFaultStateNote1 = "";
                    ViewBag.cFaultStateNote2 = "";
                    ViewBag.cFaultStateNoteOther = "";

                    if (!string.IsNullOrEmpty(beanM.CFaultState))
                    {
                        ViewBag.cFaultState = beanM.CFaultState;

                        string[] tAryState = beanM.CFaultState.Split(';');

                        foreach (string tValue in tAryState)
                        {
                            switch (tValue)
                            {
                                case "Z01": //面板燈號
                                    ViewBag.cFaultState1 = "Y";
                                    ViewBag.cFaultStateNote1 = beanM.CFaultStateNote1;
                                    break;
                                case "Z02": //管理介面(iLO、IMM、iDRAC)
                                    ViewBag.cFaultState2 = "Y";
                                    ViewBag.cFaultStateNote2 = beanM.CFaultStateNote2;
                                    break;
                                case "Z99": //其他
                                    ViewBag.cFaultStateOther = "Y";
                                    ViewBag.cFaultStateNoteOther = beanM.CFaultStateNoteOther;
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region 故障零件規格料號
                    ViewBag.cFaultSpec = "";
                    ViewBag.cFaultSpec1 = "";
                    ViewBag.cFaultSpec2 = "";
                    ViewBag.cFaultSpecOther = "";

                    ViewBag.cFaultSpecNote1 = "";
                    ViewBag.cFaultSpecNote2 = "";
                    ViewBag.cFaultSpecNoteOther = "";

                    if (!string.IsNullOrEmpty(beanM.CFaultSpec))
                    {
                        ViewBag.cFaultSpec = beanM.CFaultSpec;

                        string[] tArySpec = beanM.CFaultSpec.Split(';');

                        foreach (string tValue in tArySpec)
                        {
                            switch (tValue)
                            {
                                case "Z01": //零件規格
                                    ViewBag.cFaultSpec1 = "Y";
                                    ViewBag.cFaultSpecNote1 = beanM.CFaultSpecNote1;
                                    break;
                                case "Z02": //零件料號
                                    ViewBag.cFaultSpec2 = "Y";
                                    ViewBag.cFaultSpecNote2 = beanM.CFaultSpecNote2;
                                    break;
                                case "Z99": //其他
                                    ViewBag.cFaultSpecOther = "Y";
                                    ViewBag.cFaultSpecNoteOther = beanM.CFaultSpecNoteOther;
                                    break;
                            }
                        }
                    }
                    #endregion

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

                    ViewBag.CreatedDate = Convert.ToDateTime(beanM.CreatedDate).ToString("yyyy-MM-dd HH:mm");

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
                    ViewBag.cSRRepairLevel = "Z03"; //三級(一般叫修)
                    ViewBag.cDelayReason = "";      //空值
                    ViewBag.cIsSecondFix = "";     //請選擇
                    ViewBag.cIsInternalWork = "N";
                    ViewBag.CreatedUserName = "";
                }
                #endregion
            }

            #region 指派Option值
            model.ddl_cStatus = ViewBag.pStatus;                //設定狀態
            model.ddl_cSRPathWay = ViewBag.cSRPathWay;          //設定報修管道
            model.ddl_cMAServiceType = ViewBag.cMAServiceType;   //設定維護服務種類
            model.ddl_cSRProcessWay = ViewBag.cSRProcessWay;    //設定處理方式
            model.ddl_cSRRepairLevel = ViewBag.cSRRepairLevel;  //設定故障報修等級
            model.ddl_cIsSecondFix = ViewBag.cIsSecondFix;      //是否為二修
            model.ddl_cCustomerType = ViewBag.cCustomerType;    //客戶類型(P.個人 C.法人)
            model.ddl_cIsInternalWork = ViewBag.cIsInternalWork; //是否為內部作業
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
            getEmployeeInfo();           

            pSRID = formCollection["hid_cSRID"].FirstOrDefault();

            bool tIsFormal = false;
            string tONEURLName = string.Empty;
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
            string OldCSrrepairLevel = string.Empty;
            string OldCDelayReason = string.Empty;
            string OldCIsSecondFix = string.Empty;
            string OldCIsInternalWork = string.Empty;
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

            string OldCFaultGroup = string.Empty;
            string OldCFaultGroup1 = string.Empty;
            string OldCFaultGroup2 = string.Empty;
            string OldCFaultGroup3 = string.Empty;
            string OldCFaultGroup4 = string.Empty;
            string OldCFaultGroupOther = string.Empty;
            string OldCFaultState = string.Empty;
            string OldCFaultState1 = string.Empty;
            string OldCFaultState2 = string.Empty;
            string OldCFaultStateOther = string.Empty;
            string OldCFaultSpec = string.Empty;
            string OldCFaultSpec1 = string.Empty;
            string OldCFaultSpec2 = string.Empty;
            string OldCFaultSpecOther = string.Empty;

            string OldCFaultGroupNote1 = string.Empty;
            string OldCFaultGroupNote2 = string.Empty;
            string OldCFaultGroupNote3 = string.Empty;
            string OldCFaultGroupNote4 = string.Empty;
            string OldCFaultGroupNoteOther = string.Empty;
            string OldCFaultStateNote1 = string.Empty;
            string OldCFaultStateNote2 = string.Empty;
            string OldCFaultStateNoteOther = string.Empty;
            string OldCFaultSpecNote1 = string.Empty;
            string OldCFaultSpecNote2 = string.Empty;
            string OldCFaultSpecNoteOther = string.Empty;

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
            string CSrrepairLevel = formCollection["ddl_cSRRepairLevel"].FirstOrDefault();
            string CRepairEmail = formCollection["tbx_cRepairEmail"].FirstOrDefault();
            string CIsSecondFix = formCollection["ddl_cIsSecondFix"].FirstOrDefault();
            string CIsInternalWork = formCollection["ddl_cIsInternalWork"].FirstOrDefault();
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
            //string LoginUser_Name = formCollection["hid_cLoginUser_Name"].FirstOrDefault();

            string CFaultGroup = formCollection["hid_cFaultGroup"].FirstOrDefault();            
            string CFaultState = formCollection["hid_cFaultState"].FirstOrDefault();            
            string CFaultSpec = formCollection["hid_cFaultSpec"].FirstOrDefault();            
            string CFaultGroupNote1 = formCollection["tbx_cFaultGroupNote1"].FirstOrDefault();
            string CFaultGroupNote2 = formCollection["tbx_cFaultGroupNote2"].FirstOrDefault();
            string CFaultGroupNote3 = formCollection["tbx_cFaultGroupNote3"].FirstOrDefault();
            string CFaultGroupNote4 = formCollection["tbx_cFaultGroupNote4"].FirstOrDefault();
            string CFaultGroupNoteOther = formCollection["tbx_cFaultGroupNoteOther"].FirstOrDefault();
            string CFaultStateNote1 = formCollection["tbx_cFaultStateNote1"].FirstOrDefault();
            string CFaultStateNote2 = formCollection["tbx_cFaultStateNote2"].FirstOrDefault();
            string CFaultStateNoteOther = formCollection["tbx_cFaultStateNoteOther"].FirstOrDefault();
            string CFaultSpecNote1 = formCollection["tbx_cFaultSpecNote1"].FirstOrDefault();
            string CFaultSpecNote2 = formCollection["tbx_cFaultSpecNote2"].FirstOrDefault();
            string CFaultSpecNoteOther = formCollection["tbx_cFaultSpecNoteOther"].FirstOrDefault();

            SRCondition srCon = new SRCondition();
            SRMain_SRSTATUS_INPUT beanIN = new SRMain_SRSTATUS_INPUT();

            try
            {
                #region 取得系統位址參數相關資訊
                SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

                tIsFormal = ParaBean.IsFormal;

                tONEURLName = ParaBean.ONEURLName;
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
                    beanM.CSrrepairLevel = CSrrepairLevel;
                    beanM.CDelayReason = CDelayReason;
                    beanM.CIsSecondFix = CIsSecondFix;
                    beanM.CIsInternalWork = CIsInternalWork;
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
                    beanM.CreatedUserName = ViewBag.empEngName;

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
                            beanD.CreatedUserName = ViewBag.empEngName;

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
                            beanD.CreatedUserName = ViewBag.empEngName;

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
                        beanD.CreatedUserName = ViewBag.empEngName;

                        dbOne.TbOneSrdetailWarranties.Add(beanD);
                    }
                    #endregion                    

                    int result = dbOne.SaveChanges();

                    if (result <= 0)
                    {
                        pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "提交失敗(新建)" + Environment.NewLine;
                        CMF.writeToLog(pSRID, "SaveGenerallySR", pMsg, ViewBag.empEngName);
                    }
                    else
                    {
                        #region 紀錄修改log
                        TbOneLog logBean = new TbOneLog
                        {
                            CSrid = pSRID,
                            EventName = "SaveGenerallySR",
                            Log = "新增成功！",
                            CreatedUserName = ViewBag.empEngName,
                            CreatedDate = DateTime.Now
                        };

                        dbOne.TbOneLogs.Add(logBean);
                        dbOne.SaveChanges();
                        #endregion

                        #region call ONE SERVICE 服務案件(一般/裝機/定維)狀態更新接口來寄送Mail
                        beanIN.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
                        beanIN.IV_LOGINEMPNAME = ViewBag.empEngName;
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
                    srCon = SRCondition.SAVE;

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

                    OldCSrrepairLevel = beanNowM.CSrrepairLevel;
                    tLog += CMF.getNewAndOldLog("故障報修等級", OldCSrrepairLevel, CSrrepairLevel);

                    OldCDelayReason = beanNowM.CDelayReason;
                    tLog += CMF.getNewAndOldLog("延遲結案原因", OldCDelayReason, CDelayReason);

                    OldCIsSecondFix = beanNowM.CIsSecondFix;
                    tLog += CMF.getNewAndOldLog("是否為二修", OldCIsSecondFix, CIsSecondFix);

                    OldCIsInternalWork = beanNowM.CIsInternalWork;
                    tLog += CMF.getNewAndOldLog("是否為內部作業", OldCIsInternalWork, CIsInternalWork);

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

                    #region 客戶故障狀況分類
                    OldCFaultGroup = string.IsNullOrEmpty(beanNowM.CFaultGroup) ? "" : beanNowM.CFaultGroup;
                    OldCFaultGroup = CMF.TransL2Fault(OldCFaultGroup, "Group");
                    string CFaultGroupTemp = CMF.TransL2Fault(CFaultGroup, "Group");
                    tLog += CMF.getNewAndOldLog("客戶故障狀況分類", OldCFaultGroup, CFaultGroupTemp);

                    OldCFaultGroupNote1 = string.IsNullOrEmpty(beanNowM.CFaultGroupNote1) ? "" : beanNowM.CFaultGroupNote1;
                    tLog += CMF.getNewAndOldLog("客戶故障狀況分類-硬體說明", OldCFaultGroupNote1, CFaultGroupNote1);

                    OldCFaultGroupNote2 = string.IsNullOrEmpty(beanNowM.CFaultGroupNote2) ? "" : beanNowM.CFaultGroupNote2;
                    tLog += CMF.getNewAndOldLog("客戶故障狀況分類-系統說明", OldCFaultGroupNote2, CFaultGroupNote2);

                    OldCFaultGroupNote3 = string.IsNullOrEmpty(beanNowM.CFaultGroupNote3) ? "" : beanNowM.CFaultGroupNote3;
                    tLog += CMF.getNewAndOldLog("客戶故障狀況分類-服務說明", OldCFaultGroupNote3, CFaultGroupNote3);

                    OldCFaultGroupNote4 = string.IsNullOrEmpty(beanNowM.CFaultGroupNote4) ? "" : beanNowM.CFaultGroupNote4;
                    tLog += CMF.getNewAndOldLog("客戶故障狀況分類-網路說明", OldCFaultGroupNote4, CFaultGroupNote4);

                    OldCFaultGroupNoteOther = string.IsNullOrEmpty(beanNowM.CFaultGroupNoteOther) ? "" : beanNowM.CFaultGroupNoteOther;
                    tLog += CMF.getNewAndOldLog("客戶故障狀況分類-其他說明", OldCFaultGroupNoteOther, CFaultGroupNoteOther);
                    #endregion

                    #region 客戶故障狀況
                    OldCFaultState = string.IsNullOrEmpty(beanNowM.CFaultState) ? "" : beanNowM.CFaultState;
                    OldCFaultState = CMF.TransL2Fault(OldCFaultState, "State");
                    string CFaultStateTemp = CMF.TransL2Fault(CFaultState, "State");
                    tLog += CMF.getNewAndOldLog("客戶故障狀況", OldCFaultState, CFaultStateTemp);

                    OldCFaultStateNote1 = string.IsNullOrEmpty(beanNowM.CFaultStateNote1) ? "" : beanNowM.CFaultStateNote1;
                    tLog += CMF.getNewAndOldLog("客戶故障狀況-面板燈號說明", OldCFaultStateNote1, CFaultStateNote1);

                    OldCFaultStateNote2 = string.IsNullOrEmpty(beanNowM.CFaultStateNote2) ? "" : beanNowM.CFaultStateNote2;
                    tLog += CMF.getNewAndOldLog("客戶故障狀況-管理介面(iLO、IMM、iDRAC)說明", OldCFaultStateNote2, CFaultStateNote2);                    

                    OldCFaultStateNoteOther = string.IsNullOrEmpty(beanNowM.CFaultStateNoteOther) ? "" : beanNowM.CFaultStateNoteOther;
                    tLog += CMF.getNewAndOldLog("客戶故障狀況-其他說明", OldCFaultStateNoteOther, CFaultStateNoteOther);
                    #endregion

                    #region 故障零件規格料號
                    OldCFaultSpec = string.IsNullOrEmpty(beanNowM.CFaultSpec) ? "" : beanNowM.CFaultSpec;
                    OldCFaultSpec = CMF.TransL2Fault(OldCFaultSpec, "Spec");
                    string CFaultSpecTemp = CMF.TransL2Fault(CFaultSpec, "Spec");
                    tLog += CMF.getNewAndOldLog("故障零件規格料號", OldCFaultSpec, CFaultSpecTemp);

                    OldCFaultSpecNote1 = string.IsNullOrEmpty(beanNowM.CFaultSpecNote1) ? "" : beanNowM.CFaultSpecNote1;
                    tLog += CMF.getNewAndOldLog("故障零件規格料號-零件規格說明", OldCFaultSpecNote1, CFaultSpecNote1);

                    OldCFaultSpecNote2 = string.IsNullOrEmpty(beanNowM.CFaultSpecNote2) ? "" : beanNowM.CFaultSpecNote2;
                    tLog += CMF.getNewAndOldLog("故障零件規格料號-零件料號說明", OldCFaultSpecNote2, CFaultSpecNote2);

                    OldCFaultSpecNoteOther = string.IsNullOrEmpty(beanNowM.CFaultSpecNoteOther) ? "" : beanNowM.CFaultSpecNoteOther;
                    tLog += CMF.getNewAndOldLog("故障零件規格料號-其他說明", OldCFaultSpecNoteOther, CFaultSpecNoteOther);
                    #endregion

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
                    beanNowM.CSrrepairLevel = CSrrepairLevel;
                    beanNowM.CDelayReason = CDelayReason;
                    beanNowM.CIsSecondFix = CIsSecondFix;
                    beanNowM.CIsInternalWork = CIsInternalWork;
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

                    #region 客戶故障狀況分類
                    beanNowM.CFaultGroup = string.IsNullOrEmpty(CFaultGroup) ? "" : CFaultGroup.TrimEnd(';');
                    beanNowM.CFaultGroupNote1 = string.IsNullOrEmpty(CFaultGroupNote1) ? "" : CFaultGroupNote1;
                    beanNowM.CFaultGroupNote2 = string.IsNullOrEmpty(CFaultGroupNote2) ? "" : CFaultGroupNote2;
                    beanNowM.CFaultGroupNote3 = string.IsNullOrEmpty(CFaultGroupNote3) ? "" : CFaultGroupNote3;
                    beanNowM.CFaultGroupNote4 = string.IsNullOrEmpty(CFaultGroupNote4) ? "" : CFaultGroupNote4;
                    beanNowM.CFaultGroupNoteOther = string.IsNullOrEmpty(CFaultGroupNoteOther) ? "" : CFaultGroupNoteOther;
                    #endregion

                    #region 客戶故障狀況
                    beanNowM.CFaultState = string.IsNullOrEmpty(CFaultState) ? "" : CFaultState.TrimEnd(';');
                    beanNowM.CFaultStateNote1 = string.IsNullOrEmpty(CFaultStateNote1) ? "" : CFaultStateNote1;
                    beanNowM.CFaultStateNote2 = string.IsNullOrEmpty(CFaultStateNote2) ? "" : CFaultStateNote2;                    
                    beanNowM.CFaultStateNoteOther = string.IsNullOrEmpty(CFaultStateNoteOther) ? "" : CFaultStateNoteOther;
                    #endregion

                    #region 故障零件規格料號
                    beanNowM.CFaultSpec = string.IsNullOrEmpty(CFaultSpec) ? "" : CFaultSpec.TrimEnd(';');
                    beanNowM.CFaultSpecNote1 = string.IsNullOrEmpty(CFaultSpecNote1) ? "" : CFaultSpecNote1;
                    beanNowM.CFaultSpecNote2 = string.IsNullOrEmpty(CFaultSpecNote2) ? "" : CFaultSpecNote2;
                    beanNowM.CFaultSpecNoteOther = string.IsNullOrEmpty(CFaultSpecNoteOther) ? "" : CFaultSpecNoteOther;
                    #endregion

                    if (CStatus == "E0006") //完修
                    {
                        beanNowM.CIsAppclose = "N";
                    }

                    beanNowM.ModifiedDate = DateTime.Now;
                    beanNowM.ModifiedUserName = ViewBag.empEngName;
                    #endregion

                    #region -----↓↓↓↓↓客戶聯絡窗口資訊↓↓↓↓↓-----

                    #region 刪除明細                    
                    //dbOne.TbOneSrdetailContacts.RemoveRange(dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pSRID));

                    var beansDCon = dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    foreach(var beanDCon in beansDCon)
                    {
                        beanDCon.Disabled = 1;
                        beanDCon.ModifiedDate = DateTime.Now;
                        beanDCon.ModifiedUserName = ViewBag.empEngName;
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
                            beanD.CreatedUserName = ViewBag.empEngName;

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
                        beanDPro.ModifiedUserName = ViewBag.empEngName;
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
                            beanD.CreatedUserName = ViewBag.empEngName;

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
                        beanD.CreatedUserName = ViewBag.empEngName;

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
                        beanDRec.ModifiedUserName = ViewBag.empEngName;
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
                            beanD.CreatedUserName = ViewBag.empEngName;

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
                        beanDPar.ModifiedUserName = ViewBag.empEngName;
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
                            beanD.CreatedUserName = ViewBag.empEngName;

                            dbOne.TbOneSrdetailPartsReplaces.Add(beanD);
                        }
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑零件更換資訊 ↑↑↑↑↑-----

                    int result = dbOne.SaveChanges();

                    if (result <= 0)
                    {
                        pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "提交失敗(編輯)" + Environment.NewLine;
                        CMF.writeToLog(pSRID, "SaveGenerallySR", pMsg, ViewBag.empEngName);
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
                                CreatedUserName = ViewBag.empEngName,
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

                        if (OldCMainEngineerId != "")
                        {
                            if (CStatus != "E0001" && OldCStatus != CStatus)
                            {
                                TempStatus = CStatus + "|" + OldCStatus; //記錄新舊狀態(用來判斷是從網頁結案)
                            }
                        }

                        beanIN.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
                        beanIN.IV_LOGINEMPNAME = ViewBag.empEngName;
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
                
                CMF.writeToLog(pSRID, "SaveGenerallySR", pMsg, ViewBag.empEngName);
            }

            if(srCon == SRCondition.ADD)
            {
                return RedirectToAction("GenerallySR", new { SRID = pSRID });
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
            getEmployeeInfo();          

            bool tIsFormal = false;
            string BPMNO = string.Empty;
            string DNDATE = string.Empty;
            string SDATE = string.Empty;
            string EDATE = string.Empty;
            string tURL = string.Empty;
            string tONEURLName = string.Empty;
            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;
            string tInvoiceNo = string.Empty;
            string tInvoiceItem = string.Empty;

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tONEURLName = ParaBean.ONEURLName;
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
                QueryToList = CMF.ZFM_TICC_SERIAL_SEARCHWTYList(ArySERIAL, ref NowCount, tBPMURLName, tONEURLName, tAPIURLName);
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

                CMF.writeToLog("", "QuerySRDetail_Warranty", pMsg, ViewBag.empEngName);
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
            getEmployeeInfo();           

            bool tIsFormal = false;
            string tONEURLName = string.Empty;
            string tBPMURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tAttachURLName = string.Empty;

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tONEURLName = ParaBean.ONEURLName;
            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;
            #endregion           

            int NowCount = 0;

            List<SRWarranty> QueryToList = new List<SRWarranty>();    //查詢出來的清單             

            try
            {                
                QueryToList = CMF.SEARCHWTYList(cSRID, ref NowCount, tBPMURLName, tONEURLName);
                QueryToList = QueryToList.OrderBy(x => x.cSerialID).ThenByDescending(x => x.cWTYEDATE).ToList();
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "失敗原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();
                
                CMF.writeToLog(cSRID, "getSRDetail_Warranty", pMsg, ViewBag.empEngName);
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
            getEmployeeInfo();
           
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

        #region 儲存常用服務團隊
        /// <summary>
        /// 儲存常用服務團隊
        /// </summary>
        /// <param name="cTeamID">目前的服務團隊ID(;號隔開)</param>        
        /// <returns></returns>
        public IActionResult savepjPersonallyTeam(string cTeamID)
        {
            getLoginAccount();
            getEmployeeInfo();

            string reValue = "SUCCESS";
            string cERPID = ViewBag.cLoginUser_ERPID;

            try
            {
                if (!string.IsNullOrEmpty(cTeamID))
                {
                    var bean = dbOne.TbOneSroftenUsedData.FirstOrDefault(x => x.Disabled == 0 && x.CFunctionId == "PERSONAL" &&
                                                                      x.CCompanyId == pCompanyCode && x.CNo == "OftenTeam" &&
                                                                      x.CreatedErpid == cERPID);

                    if (bean != null)
                    {
                        bean.CValue = cTeamID.TrimEnd(';');
                        bean.ModifiedDate = DateTime.Now;
                        bean.ModifiedUserName = ViewBag.empEngName;
                    }
                    else
                    {
                        TbOneSroftenUsedDatum SRO = new TbOneSroftenUsedDatum();

                        SRO.CFunctionId = "PERSONAL";                //固定為「PERSONAL.個人」
                        SRO.CCompanyId = pCompanyCode;
                        SRO.CNo = "OftenTeam";                      //固定為「OftenTeam」
                        SRO.CValue = cTeamID.TrimEnd(';');
                        SRO.CDescription = "個人常用服務團隊清單";    //固定為「個人常用服務團隊清單」
                        SRO.Disabled = 0;

                        SRO.CreatedErpid = ViewBag.cLoginUser_ERPID;
                        SRO.CreatedDate = DateTime.Now;
                        SRO.CreatedUserName = ViewBag.empEngName;

                        dbOne.TbOneSroftenUsedData.Add(SRO);

                    }

                    int result = dbOne.SaveChanges();

                    if (result <= 0)
                    {
                        reValue = "儲存常用服務團隊失敗！";
                    }
                }
            }
            catch (Exception e)
            {
                return Json("儲存常用服務團隊失敗，原因：" + e.Message);
            }

            return Json(reValue);
        }
        #endregion

        #region 帶入常用服務團隊
        /// <summary>
        /// 儲存常用服務團隊
        /// </summary>        
        /// <returns></returns>
        public IActionResult getpjPersonallyTeam()
        {
            getLoginAccount();
            getEmployeeInfo();

            string reValue = string.Empty;
            string cERPID = ViewBag.cLoginUser_ERPID;

            try
            {
                var bean = dbOne.TbOneSroftenUsedData.FirstOrDefault(x => x.Disabled == 0 && x.CFunctionId == "PERSONAL" &&
                                                                      x.CCompanyId == pCompanyCode && x.CNo == "OftenTeam" &&
                                                                      x.CreatedErpid == cERPID);

                if (bean != null)
                {
                    reValue = bean.CValue.Trim();
                }
            }
            catch (Exception e)
            {

            }

            return Json(reValue);
        }
        #endregion

        #region 修改服務團隊
        /// <summary>
        /// 修改服務團隊
        /// </summary>
        /// <param name="cTeamID">目前的服務團隊的ID(;號隔開)</param>
        /// <param name="cTeamAcc">欲修改的服務團隊ERPID</param>
        /// <returns></returns>
        public IActionResult SavepjTeam(string cTeamID, string cTeamAcc)
        {
            getLoginAccount();
            getEmployeeInfo();           

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
        /// <param name="cTeamID">服務團隊的ID(;號隔開)</param>
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

                    //若是舊的服務團隊，至少塞服務團隊的ID
                    if (tEmpName == "")
                    {
                        tEmpName = AssAcc;
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
        /// <param name="cTeamID">目前的服務團隊的ID(;號隔開)</param>
        /// <param name="cTeamAcc">欲刪除的服務團隊ERPID</param>
        /// <returns></returns>
        public IActionResult DeletepjTeam(string cTeamID, string cTeamAcc)
        {
            getLoginAccount();
            getEmployeeInfo();           

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

        #region 儲存常用工程師
        /// <summary>
        /// 儲存常用工程師
        /// </summary>
        /// <param name="cAssEngineerID">目前的工程師ERPID(;號隔開)</param>        
        /// <returns></returns>
        public IActionResult savepjPersonallyEngineer(string cAssEngineerID)
        {
            getLoginAccount();
            getEmployeeInfo();

            string reValue = "SUCCESS";
            string cERPID = ViewBag.cLoginUser_ERPID;

            try
            {
                if (!string.IsNullOrEmpty(cAssEngineerID))
                {
                    var bean = dbOne.TbOneSroftenUsedData.FirstOrDefault(x => x.Disabled == 0 && x.CFunctionId == "PERSONAL" &&
                                                                      x.CCompanyId == pCompanyCode && x.CNo == "OftenEngineer" &&
                                                                      x.CreatedErpid == cERPID);

                    if (bean != null)
                    {
                        bean.CValue = cAssEngineerID.TrimEnd(';');
                        bean.ModifiedDate = DateTime.Now;
                        bean.ModifiedUserName = ViewBag.empEngName;
                    }
                    else
                    {
                        TbOneSroftenUsedDatum SRO = new TbOneSroftenUsedDatum();

                        SRO.CFunctionId = "PERSONAL";               //固定為「PERSONAL.個人」
                        SRO.CCompanyId = pCompanyCode;
                        SRO.CNo = "OftenEngineer";                  //固定為「OftenEngineer」
                        SRO.CValue = cAssEngineerID.TrimEnd(';');
                        SRO.CDescription = "個人常用工程師清單";     //固定為「個人常用工程師清單」
                        SRO.Disabled = 0;

                        SRO.CreatedErpid = ViewBag.cLoginUser_ERPID;
                        SRO.CreatedDate = DateTime.Now;
                        SRO.CreatedUserName = ViewBag.empEngName;

                        dbOne.TbOneSroftenUsedData.Add(SRO);

                    }

                    int result = dbOne.SaveChanges();

                    if (result <= 0 )
                    {
                        reValue = "儲存常用工程師失敗！";
                    }
                }               
            }
            catch (Exception e)
            {
                return Json("儲存常用工程師失敗，原因：" + e.Message);
            }

            return Json(reValue);
        }
        #endregion

        #region 帶入常用工程師
        /// <summary>
        /// 儲存常用工程師
        /// </summary>        
        /// <returns></returns>
        public IActionResult getpjPersonallyEngineer()
        {
            getLoginAccount();
            getEmployeeInfo();

            string reValue = string.Empty;
            string cERPID = ViewBag.cLoginUser_ERPID;

            try
            {
                var bean = dbOne.TbOneSroftenUsedData.FirstOrDefault(x => x.Disabled == 0 && x.CFunctionId == "PERSONAL" &&
                                                                      x.CCompanyId == pCompanyCode && x.CNo == "OftenEngineer" &&
                                                                      x.CreatedErpid == cERPID);

                if (bean != null)
                {
                    reValue = bean.CValue.Trim();
                }
            }
            catch (Exception e)
            {
                
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
            getEmployeeInfo();           

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
            getEmployeeInfo();           

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
            getEmployeeInfo();

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
            getEmployeeInfo();            

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
            getEmployeeInfo();            

            try
            {
                #region 刪除聯絡人
                var bean = dbProxy.CustomerContacts.FirstOrDefault(x => x.ContactId.ToString() == cContactID);

                if (bean != null)
                {
                    bean.Disabled = 1;
                    bean.MainModifiedDate = DateTime.Now;
                    bean.MainModifiedUserName = ViewBag.empEngName;

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

        #region 產生快遞單
        /// <summary>
        /// 產生快遞單
        /// </summary>        
        /// <param name="cSRID">cSRID</param>
        /// <returns></returns>
        public IActionResult DownloadDeliveryInfo(string cSRID)
        {
            var bean        = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == cSRID);
            var Productbean = dbOne.TbOneSrdetailProducts.FirstOrDefault(x => x.CSrid == cSRID);

            if (bean != null)
            {
                string webRootPath  = _HostEnvironment.WebRootPath + "/seed";
                string FilePath     = Path.Combine(webRootPath, "DeliveryForm.xlsx");
                string SerialId     = Productbean != null ? Productbean.CSerialId : "";

                XSSFWorkbook WorkBook;
                using (FileStream FS = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    WorkBook = new XSSFWorkbook(FS);
                }

                XSSFSheet Sheet = (XSSFSheet)WorkBook.GetSheetAt(0);

                Sheet.GetRow(4).GetCell(1).SetCellValue("JobID: " + cSRID);     // JobID
                Sheet.GetRow(5).GetCell(2).SetCellValue(bean.CCustomerName);    // 收件人公司名稱
                Sheet.GetRow(6).GetCell(2).SetCellValue(bean.CRepairAddress);   // 收件人公司地址
                Sheet.GetRow(7).GetCell(2).SetCellValue(bean.CRepairName);      // 收件人姓名
                Sheet.GetRow(7).GetCell(4).SetCellValue(bean.CRepairPhone);     // 收件人電話
                Sheet.GetRow(9).GetCell(4).SetCellValue(SerialId);              // 序號					

                // 將工作簿寫入 MemoryStream
                MemoryStream stream = new MemoryStream();
                WorkBook.Write(stream, true);
                stream.Flush();
                stream.Position = 0;

                // 回傳 Excel 檔案
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "JobID_" + cSRID + "_快遞單.xlsx");
            }
            else
            { return Json("Fail"); }
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
            getEmployeeInfo();           

            var model = new ViewModel_Install();

            #region Request參數            
            if (HttpContext.Request.Query["SRID"].FirstOrDefault() != null)
            {
                pSRID = HttpContext.Request.Query["SRID"].FirstOrDefault();
            }

            ViewBag.CopySRID = "";

            if (HttpContext.Request.Query["CopySRID"].FirstOrDefault() != null)
            {
                pCopySRID = HttpContext.Request.Query["CopySRID"].FirstOrDefault();

                ViewBag.CopySRID = pCopySRID;
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
            var SRTeamIDList = CMF.findSRTeamIDList("ALL", true);
            #endregion

            if (!string.IsNullOrEmpty(pCopySRID))
            {
                #region 取得複製來源的SRID
                var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pCopySRID);

                if (beanM != null)
                {
                    #region 裝機資訊                    
                    ViewBag.cSRID = "";
                    ViewBag.cCustomerID = beanM.CCustomerId;
                    ViewBag.cCustomerName = beanM.CCustomerName;
                    ViewBag.cDesc = beanM.CDesc;
                    ViewBag.cNotes = beanM.CNotes;
                    ViewBag.cAttachement = "";
                    ViewBag.cAttachementStockNo = "";
                    ViewBag.cDelayReason = "";
                    ViewBag.cSalesNo = beanM.CSalesNo;
                    ViewBag.cShipmentNo = beanM.CShipmentNo;
                    ViewBag.pStatus = "E0001";
                    ViewBag.CreatedUserName = "";

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

                    #region 取得客戶聯絡人資訊(明細)
                    var beansContact = dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pCopySRID);

                    ViewBag.Detailbean_Contact = beansContact;
                    ViewBag.trContactNo = beansContact.Count();
                    #endregion

                    #region 取得物料訊息檔資訊(明細)
                    var beansMaterial = dbOne.TbOneSrdetailMaterialInfos.OrderBy(x => x.CId).Where(x => x.Disabled == 0 && x.CSrid == pCopySRID);

                    ViewBag.Detailbean_Material = beansMaterial;
                    ViewBag.trMaterialNo = beansMaterial.Count();
                    #endregion                  
                }               
                #endregion
            }
            else
            {
                #region 取得SRID
                var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pSRID);

                if (beanM != null)
                {
                    //記錄目前GUID，用來判斷更新的先後順序
                    ViewBag.pGUID = beanM.CSystemGuid.ToString();

                    //判斷登入者是否可以編輯服務案件                
                    pIsCanEditSR = CMF.checkIsCanEditSR(beanM.CSrid, ViewBag.cLoginUser_ERPID, ViewBag.cLoginUser_CostCenterID, ViewBag.cLoginUser_DepartmentNO, pIsMIS, pIsCS, pIsSpareManager);

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
                    ViewBag.CreatedUserName = beanM.CreatedUserName;

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

                    ViewBag.CreatedDate = Convert.ToDateTime(beanM.CreatedDate).ToString("yyyy-MM-dd HH:mm");

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
                    ViewBag.CreatedUserName = "";
                }
                #endregion
            }

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
            getEmployeeInfo();           

            pSRID = formCollection["hid_cSRID"].FirstOrDefault();

            bool tIsFormal = false;

            int pTotalQuantity = 0;  //總安裝數量

            string tONEURLName = string.Empty;
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
            //string LoginUser_Name = formCollection["hid_cLoginUser_Name"].FirstOrDefault();

            SRCondition srCon = new SRCondition();
            SRMain_SRSTATUS_INPUT beanIN = new SRMain_SRSTATUS_INPUT();
            CURRENTINSTALLINFO_INPUT beanInstall = new CURRENTINSTALLINFO_INPUT();

            try
            {
                #region 取得系統位址參數相關資訊
                SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

                tIsFormal = ParaBean.IsFormal;

                tONEURLName = ParaBean.ONEURLName;
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
                    beanM.CreatedUserName = ViewBag.empEngName;

                    #region 未用到的欄位
                    beanM.CRepairName = "";
                    beanM.CRepairAddress = "";
                    beanM.CRepairPhone = "";
                    beanM.CRepairMobile = "";
                    beanM.CRepairEmail = "";
                    beanM.CMaserviceType = "";
                    beanM.CSrpathWay = "";
                    beanM.CSrprocessWay = "";
                    beanM.CSrrepairLevel = "";
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
                            beanD.CreatedUserName = ViewBag.empEngName;

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

                            pTotalQuantity += int.Parse(MIcQuantity[i]);

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
                            beanD.CreatedUserName = ViewBag.empEngName;

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
                            beanD.CreatedUserName = ViewBag.empEngName;

                            dbOne.TbOneSrdetailSerialFeedbacks.Add(beanD);
                        }
                    }
                    #endregion                    

                    int result = dbOne.SaveChanges();

                    if (result <= 0)
                    {
                        pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "提交失敗(新建)" + Environment.NewLine;
                        CMF.writeToLog(pSRID, "SaveInstallSR", pMsg, ViewBag.empEngName);
                    }
                    else
                    {
                        #region 紀錄修改log
                        TbOneLog logBean = new TbOneLog
                        {
                            CSrid = pSRID,
                            EventName = "SaveInstallSR",
                            Log = "新增成功！",
                            CreatedUserName = ViewBag.empEngName,
                            CreatedDate = DateTime.Now
                        };

                        dbOne.TbOneLogs.Add(logBean);
                        dbOne.SaveChanges();
                        #endregion

                        #region call ONE SERVICE更新裝機現況資訊接口   
                        beanInstall.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
                        beanInstall.IV_LOGINEMPNAME = ViewBag.empEngName;
                        beanInstall.IV_SRID = pSRID;
                        beanInstall.IV_InstallDate = "";
                        beanInstall.IV_ExpectedDate = "";
                        beanInstall.IV_TotalQuantity = pTotalQuantity.ToString("N0");
                        beanInstall.IV_InstallQuantity = "0";
                        beanInstall.IV_IsFromAPP = "N";
                        beanInstall.IV_APIURLName = tAPIURLName;

                        CMF.GetAPI_CURRENTINSTALLINFO_Update(beanInstall);
                        #endregion

                        #region call ONE SERVICE 服務案件(一般/裝機/定維)狀態更新接口來寄送Mail
                        beanIN.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
                        beanIN.IV_LOGINEMPNAME = ViewBag.empEngName;
                        beanIN.IV_SRID = pSRID;

                        if (CMainEngineerId != "")
                        {
                            beanIN.IV_STATUS = "E0008|ADD"; //新建但狀態是裝機中
                        }
                        else
                        {
                            beanIN.IV_STATUS = "E0001|ADD"; //新建
                        }

                        beanIN.IV_APIURLName = tAPIURLName;

                        CMF.GetAPI_SRSTATUS_Update(beanIN);
                        #endregion
                    }
                }
                else
                {
                    #region 修改主檔
                    srCon = SRCondition.SAVE;

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
                    beanNowM.ModifiedUserName = ViewBag.empEngName;
                    #endregion

                    #region -----↓↓↓↓↓客戶聯絡窗口資訊↓↓↓↓↓-----

                    #region 刪除明細
                    var beansDCon = dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    foreach (var beanDCon in beansDCon)
                    {
                        beanDCon.Disabled = 1;
                        beanDCon.ModifiedDate = DateTime.Now;
                        beanDCon.ModifiedUserName = ViewBag.empEngName;
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
                            beanD.CreatedUserName = ViewBag.empEngName;

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
                        beanDMI.ModifiedUserName = ViewBag.empEngName;
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

                            pTotalQuantity += int.Parse(MIcQuantity[i]);

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
                            beanD.CreatedUserName = ViewBag.empEngName;

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
                        beanDSF.ModifiedUserName = ViewBag.empEngName;
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
                            beanD.CreatedUserName = ViewBag.empEngName;

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
                        beanDRec.ModifiedUserName = ViewBag.empEngName;
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
                            beanD.CreatedUserName = ViewBag.empEngName;

                            dbOne.TbOneSrdetailRecords.Add(beanD);
                        }
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑處理與工時紀錄 ↑↑↑↑↑-----                    

                    int result = dbOne.SaveChanges();

                    if (result <= 0)
                    {
                        pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "提交失敗(編輯)" + Environment.NewLine;
                        CMF.writeToLog(pSRID, "SaveInstallSR", pMsg, ViewBag.empEngName);
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
                                CreatedUserName = ViewBag.empEngName,
                                CreatedDate = DateTime.Now
                            };

                            dbOne.TbOneLogs.Add(logBean);
                            dbOne.SaveChanges();
                            #endregion
                        }

                        #region call ONE SERVICE更新裝機現況資訊接口   
                        beanInstall.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
                        beanInstall.IV_LOGINEMPNAME = ViewBag.empEngName;
                        beanInstall.IV_SRID = pSRID;
                        beanInstall.IV_InstallDate = "";
                        beanInstall.IV_ExpectedDate = "";
                        beanInstall.IV_TotalQuantity = pTotalQuantity.ToString("N0");
                        beanInstall.IV_InstallQuantity = "0";
                        beanInstall.IV_IsFromAPP = "N";
                        beanInstall.IV_APIURLName = tAPIURLName;

                        CMF.GetAPI_CURRENTINSTALLINFO_Update(beanInstall);
                        #endregion

                        #region call ONE SERVICE（裝機服務案件）狀態更新接口來寄送Mail
                        string TempStatus = CStatus;

                        if (OldCMainEngineerId != CMainEngineerId)
                        {
                            TempStatus = CStatus + "|TRANS"; //轉單
                        }

                        if (OldCMainEngineerId != "")
                        {
                            if (CStatus != "E0001" && OldCStatus != CStatus)
                            {
                                TempStatus = CStatus + "|" + OldCStatus; //記錄新舊狀態(用來判斷是從網頁結案)
                            }
                        }

                        beanIN.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
                        beanIN.IV_LOGINEMPNAME = ViewBag.empEngName;
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

                CMF.writeToLog(pSRID, "SaveInstallSR", pMsg, ViewBag.empEngName);
            }

            if (srCon == SRCondition.ADD)
            {
                return RedirectToAction("InstallSR", new { SRID = pSRID });
            }

            return RedirectToAction("finishForm");
        }
        #endregion

        #endregion -----↑↑↑↑↑裝機服務 ↑↑↑↑↑----- 

        #region -----↓↓↓↓↓定維服務 ↓↓↓↓↓-----

        #region 定維服務index
        public IActionResult MaintainSR()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();
            getEmployeeInfo();           

            var model = new ViewModel_Maintain();

            #region Request參數            
            if (HttpContext.Request.Query["SRID"].FirstOrDefault() != null)
            {
                pSRID = HttpContext.Request.Query["SRID"].FirstOrDefault();
            }

            ViewBag.CopySRID = "";

            if (HttpContext.Request.Query["CopySRID"].FirstOrDefault() != null)
            {
                pCopySRID = HttpContext.Request.Query["CopySRID"].FirstOrDefault();

                ViewBag.CopySRID = pCopySRID;
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
            var SRTeamIDList = CMF.findSRTeamIDList("ALL", true);
            #endregion

            if (!string.IsNullOrEmpty(pCopySRID))
            {
                #region 取得複製來源的SRID
                var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pCopySRID);

                if (beanM != null)
                {
                    #region 定維資訊
                    ViewBag.cSRID = "";
                    ViewBag.cCustomerID = beanM.CCustomerId;
                    ViewBag.cCustomerName = beanM.CCustomerName;
                    ViewBag.cDesc = beanM.CDesc;
                    ViewBag.cNotes = beanM.CNotes;
                    ViewBag.cAttachement = "";                    
                    ViewBag.cDelayReason = "";
                    ViewBag.cContractID = beanM.CContractId;
                    ViewBag.pStatus = "E0001";
                    ViewBag.CreatedUserName = "";

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

                    #region 取得客戶聯絡人資訊(明細)
                    var beansContact = dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pCopySRID);

                    ViewBag.Detailbean_Contact = beansContact;
                    ViewBag.trContactNo = beansContact.Count();
                    #endregion                   
                }               
                #endregion
            }
            else
            {
                #region 取得SRID
                var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pSRID);

                if (beanM != null)
                {
                    //記錄目前GUID，用來判斷更新的先後順序
                    ViewBag.pGUID = beanM.CSystemGuid.ToString();

                    //判斷登入者是否可以編輯服務案件
                    pIsCanEditSR = CMF.checkIsCanEditSR(beanM.CSrid, ViewBag.cLoginUser_ERPID, ViewBag.cLoginUser_CostCenterID, ViewBag.cLoginUser_DepartmentNO, pIsMIS, pIsCS, pIsSpareManager);

                    #region 定維資訊
                    ViewBag.cSRID = beanM.CSrid;
                    ViewBag.cCustomerID = beanM.CCustomerId;
                    ViewBag.cCustomerName = beanM.CCustomerName;
                    ViewBag.cDesc = beanM.CDesc;
                    ViewBag.cNotes = beanM.CNotes;
                    ViewBag.cAttachement = beanM.CAttachement;                    
                    ViewBag.cDelayReason = beanM.CDelayReason;
                    ViewBag.cContractID = beanM.CContractId;
                    ViewBag.pStatus = beanM.CStatus;
                    ViewBag.CreatedUserName = beanM.CreatedUserName;

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

                    ViewBag.CreatedDate = Convert.ToDateTime(beanM.CreatedDate).ToString("yyyy-MM-dd HH:mm");

                    #region 取得客戶聯絡人資訊(明細)
                    var beansContact = dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    ViewBag.Detailbean_Contact = beansContact;
                    ViewBag.trContactNo = beansContact.Count();
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
                    ViewBag.cContractID = "";       //空值
                    ViewBag.CreatedUserName = "";
                }
                #endregion
            }

            #region 指派Option值
            model.ddl_cStatus = ViewBag.pStatus;                //設定狀態            
            model.ddl_cCustomerType = ViewBag.cCustomerType;     //客戶類型(P.個人 C.法人)
            #endregion

            ViewBag.SRTypeOneList = SRTypeOneList;
            ViewBag.SRTypeSecList = SRTypeSecList;
            ViewBag.SRTypeThrList = SRTypeThrList;
            ViewBag.SRTeamIDList = SRTeamIDList;

            ViewBag.pOperationID = pOperationID_MaintainSR;
            ViewBag.pIsCanEditSR = pIsCanEditSR.ToString();  //登入者是否可以編輯服務案件

            return View(model);
        }
        #endregion

        #region 儲存定維服務
        /// <summary>
        /// 儲存定維服務
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveMaintainSR(IFormCollection formCollection)
        {
            getLoginAccount();
            getEmployeeInfo();           

            pSRID = formCollection["hid_cSRID"].FirstOrDefault();

            bool tIsFormal = false;

            int pTotalQuantity = 0;  //總安裝數量

            string tONEURLName = string.Empty;
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
            string OldCSrtypeOne = string.Empty;
            string OldCSrtypeSec = string.Empty;
            string OldCSrtypeThr = string.Empty;
            string OldCContractId = string.Empty;            
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
            string CSrtypeOne = formCollection["ddl_cSRTypeOne"].FirstOrDefault();
            string CSrtypeSec = formCollection["ddl_cSRTypeSec"].FirstOrDefault();
            string CSrtypeThr = formCollection["ddl_cSRTypeThr"].FirstOrDefault();
            string CContractId = formCollection["tbx_cContractID"].FirstOrDefault();            
            string CDelayReason = formCollection["tbx_cDelayReason"].FirstOrDefault();

            string CTeamId = formCollection["hid_cTeamID"].FirstOrDefault();
            string CMainEngineerName = formCollection["tbx_cMainEngineerName"].FirstOrDefault();
            string CMainEngineerId = formCollection["hid_cMainEngineerID"].FirstOrDefault();
            string CAssEngineerId = formCollection["hid_cAssEngineerID"].FirstOrDefault();
            string CSalesName = formCollection["tbx_cSalesName"].FirstOrDefault();
            string CSalesId = formCollection["hid_cSalesID"].FirstOrDefault();
            string CSecretaryName = formCollection["tbx_cSecretaryName"].FirstOrDefault();
            string CSecretaryId = formCollection["hid_cSecretaryID"].FirstOrDefault();
            //string LoginUser_Name = formCollection["hid_cLoginUser_Name"].FirstOrDefault();

            SRCondition srCon = new SRCondition();
            SRMain_SRSTATUS_INPUT beanIN = new SRMain_SRSTATUS_INPUT();
            CURRENTINSTALLINFO_INPUT beanInstall = new CURRENTINSTALLINFO_INPUT();

            try
            {
                #region 取得系統位址參數相關資訊
                SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

                tIsFormal = ParaBean.IsFormal;

                tONEURLName = ParaBean.ONEURLName;
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
                    beanM.CSrtypeOne = CSrtypeOne;
                    beanM.CSrtypeSec = CSrtypeSec;
                    beanM.CSrtypeThr = CSrtypeThr;
                    beanM.CContractId = CContractId;                    
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
                    beanM.CreatedUserName = ViewBag.empEngName;

                    #region 未用到的欄位
                    beanM.CRepairName = "";
                    beanM.CRepairAddress = "";
                    beanM.CRepairPhone = "";
                    beanM.CRepairMobile = "";
                    beanM.CRepairEmail = "";
                    beanM.CMaserviceType = "";
                    beanM.CSrpathWay = "";
                    beanM.CSrprocessWay = "";
                    beanM.CSrrepairLevel = "";
                    beanM.CIsSecondFix = "";
                    beanM.CTechManagerId = "";
                    beanM.CSqpersonId = "";
                    beanM.CSqpersonName = "";
                    beanM.CIsAppclose = "";
                    beanM.CSalesNo = "";
                    beanM.CShipmentNo = "";                    
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
                            beanD.CreatedUserName = ViewBag.empEngName;

                            dbOne.TbOneSrdetailContacts.Add(beanD);
                        }
                    }
                    #endregion                                  

                    int result = dbOne.SaveChanges();

                    if (result <= 0)
                    {
                        pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "提交失敗(新建)" + Environment.NewLine;
                        CMF.writeToLog(pSRID, "SaveMaintainSR", pMsg, ViewBag.empEngName);
                    }
                    else
                    {
                        #region 紀錄修改log
                        TbOneLog logBean = new TbOneLog
                        {
                            CSrid = pSRID,
                            EventName = "SaveMaintainSR",
                            Log = "新增成功！",
                            CreatedUserName = ViewBag.empEngName,
                            CreatedDate = DateTime.Now
                        };

                        dbOne.TbOneLogs.Add(logBean);
                        dbOne.SaveChanges();
                        #endregion                      

                        #region call ONE SERVICE 服務案件(一般/裝機/定維)狀態更新接口來寄送Mail
                        beanIN.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
                        beanIN.IV_LOGINEMPNAME = ViewBag.empEngName;
                        beanIN.IV_SRID = pSRID;

                        if (CMainEngineerId != "")
                        {
                            beanIN.IV_STATUS = "E0016|ADD"; //新建但狀態是定保處理中
                        }
                        else
                        {
                            beanIN.IV_STATUS = "E0001|ADD"; //新建
                        }

                        beanIN.IV_APIURLName = tAPIURLName;

                        CMF.GetAPI_SRSTATUS_Update(beanIN);
                        #endregion
                    }
                }
                else
                {
                    #region 修改主檔
                    srCon = SRCondition.SAVE;

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

                    OldCSrtypeOne = beanNowM.CSrtypeOne;
                    tLog += CMF.getNewAndOldLog("報修類別(大類)", OldCSrtypeOne, CSrtypeOne);

                    OldCSrtypeSec = beanNowM.CSrtypeSec;
                    tLog += CMF.getNewAndOldLog("報修類別(中類)", OldCSrtypeSec, CSrtypeSec);

                    OldCSrtypeThr = beanNowM.CSrtypeThr;
                    tLog += CMF.getNewAndOldLog("報修類別(中類)", OldCSrtypeThr, CSrtypeThr);

                    OldCContractId = beanNowM.CContractId;
                    tLog += CMF.getNewAndOldLog("合約文件編號", OldCContractId, CContractId);                   

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
                    beanNowM.CSrtypeOne = CSrtypeOne;
                    beanNowM.CSrtypeSec = CSrtypeSec;
                    beanNowM.CSrtypeThr = CSrtypeThr;
                    beanNowM.CContractId = CContractId;                    
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

                    if (CStatus == "E0017") //定保完成
                    {
                        beanNowM.CIsAppclose = "N";
                    }

                    beanNowM.ModifiedDate = DateTime.Now;
                    beanNowM.ModifiedUserName = ViewBag.empEngName;
                    #endregion

                    #region -----↓↓↓↓↓客戶聯絡窗口資訊↓↓↓↓↓-----

                    #region 刪除明細
                    var beansDCon = dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    foreach (var beanDCon in beansDCon)
                    {
                        beanDCon.Disabled = 1;
                        beanDCon.ModifiedDate = DateTime.Now;
                        beanDCon.ModifiedUserName = ViewBag.empEngName;
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
                            beanD.CreatedUserName = ViewBag.empEngName;

                            dbOne.TbOneSrdetailContacts.Add(beanD);
                        }
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑客戶聯絡窗口資訊 ↑↑↑↑↑-----                                    

                    #region -----↓↓↓↓↓處理與工時紀錄↓↓↓↓↓-----

                    #region 刪除明細
                    var beansDRec = dbOne.TbOneSrdetailRecords.Where(x => x.Disabled == 0 && x.CSrid == pSRID);

                    foreach (var beanDRec in beansDRec)
                    {
                        beanDRec.Disabled = 1;
                        beanDRec.ModifiedDate = DateTime.Now;
                        beanDRec.ModifiedUserName = ViewBag.empEngName;
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
                            beanD.CreatedUserName = ViewBag.empEngName;

                            dbOne.TbOneSrdetailRecords.Add(beanD);
                        }
                    }
                    #endregion

                    #endregion -----↑↑↑↑↑處理與工時紀錄 ↑↑↑↑↑-----                    

                    int result = dbOne.SaveChanges();

                    if (result <= 0)
                    {
                        pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "提交失敗(編輯)" + Environment.NewLine;
                        CMF.writeToLog(pSRID, "SaveMaintainSR", pMsg, ViewBag.empEngName);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(tLog))
                        {
                            #region 紀錄修改log
                            TbOneLog logBean = new TbOneLog
                            {
                                CSrid = pSRID,
                                EventName = "SaveMaintainSR",
                                Log = tLog,
                                CreatedUserName = ViewBag.empEngName,
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

                        if (OldCMainEngineerId != "")
                        {
                            if (CStatus != "E0001" && OldCStatus != CStatus)
                            {
                                TempStatus = CStatus + "|" + OldCStatus; //記錄新舊狀態(用來判斷是從網頁結案)
                            }
                        }

                        beanIN.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
                        beanIN.IV_LOGINEMPNAME = ViewBag.empEngName;
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

                CMF.writeToLog(pSRID, "SaveMaintainSR", pMsg, ViewBag.empEngName);
            }

            if (srCon == SRCondition.ADD)
            {
                return RedirectToAction("MaintainSR", new { SRID = pSRID });
            }

            return RedirectToAction("finishForm");
        }
        #endregion

        #endregion -----↑↑↑↑↑定維服務 ↑↑↑↑↑-----

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
            getEmployeeInfo();           

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
            var beans = dbOne.TbOneSrteamMappings.OrderBy(x => x.CTeamOldId).Where(x => x.Disabled == 0 &&
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
            getEmployeeInfo();           

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
            getEmployeeInfo();           

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
            getEmployeeInfo();           

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
            getEmployeeInfo();           

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
            getEmployeeInfo();          

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
            getEmployeeInfo();           

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
            getEmployeeInfo();           

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
            getEmployeeInfo();         

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
            getEmployeeInfo();              

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
            getEmployeeInfo();           

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
                    prBean1.Knb1Bukrs = pCompanyCode;
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
            getEmployeeInfo();           

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
            getEmployeeInfo();           

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
            getEmployeeInfo();           

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
            getEmployeeInfo();           

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
            getEmployeeInfo();          

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

        #region -----↓↓↓↓↓批次上傳裝機備料服務通知單 ↓↓↓↓↓-----

        #region 批次上傳裝機備料服務通知單index
        public IActionResult BatchUploadStockNo()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();

            #region Request參數            
            if (HttpContext.Request.Query["BatchUploadType"].FirstOrDefault() != null)
            {
                pBatchUploadType = HttpContext.Request.Query["BatchUploadType"].FirstOrDefault();                
            }

            ViewBag.pBatchUploadType = pBatchUploadType;
            #endregion

            #region 取得批次上傳類型說明
            pBatchUploadTypeNote = "批次上傳(裝機備料服務通知單/合約書文件)";

            if (pBatchUploadType != "")
            {
                switch(pBatchUploadType)
                {
                    case "INSTALL":
                        pBatchUploadTypeNote = "批次上傳裝機備料服務通知單";
                        break;

                    case "CONTRACT":
                        pBatchUploadTypeNote = "批次上傳合約書文件";
                        break;
                }
            }

            ViewBag.pBatchUploadTypeNote = pBatchUploadTypeNote;
            #endregion

            #region 批次上傳類型            
            bool tRepType = false;
            bool tIntType = false;

            switch (pBatchUploadType)
            {
                case "INSTALL":
                    tRepType = true;
                    break;
                case "CONTRACT":
                    tIntType = true;
                    break;
            }

            var selectList = new List<SelectListItem>()
            {
                new SelectListItem {Text="批次上傳裝機備料服務通知單", Value="INSTALL", Selected =tRepType },
                new SelectListItem {Text="批次上傳合約書文件", Value="CONTRACT", Selected = tIntType },
            };            

            ViewBag.SelectList = selectList;
            #endregion

            return View();
        }
        #endregion

        #region 儲存批次上傳裝機備料服務通知單
        /// <summary>
        /// 儲存批次上傳裝機備料服務通知單
        /// </summary>
        /// <param name="filezone">檔案GUID</param>
        /// <param name="BatchUploadType">批次上傳類型(INSTALL.裝機備料服務通知單 CONTRACT.合約書文件)</param>
        /// <returns></returns>
        public IActionResult SaveBatchUploadStockNo(string filezone, string BatchUploadType)
        {
            bool tIsFormal = false;

            string[] reValue = new string[2];
            reValue[0] = "Y";   //回傳的結果(Y.成功 N.失敗)
            reValue[1] = "";    //回傳的訊息

            string reMsg = string.Empty;
            string tONEURLName = string.Empty;
            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;
            string cSRID = string.Empty;            //SRID
            string cContractID = string.Empty;      //文件編號
            string cFile_Ext = string.Empty;        //副檔名

            getLoginAccount();
            getEmployeeInfo();           

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tONEURLName = ParaBean.ONEURLName;
            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;
            #endregion

            List<SRATTACHINFO> tList = CMF.findSRATTACHINFO(filezone, tAttachURLName);

            switch (BatchUploadType)
            {
                case "INSTALL":
                    #region 裝機備料服務通知單
                    foreach (var bean in tList)
                    {
                        try
                        {
                            cSRID = bean.FILE_ORG_NAME.Split('_')[0].Trim(); //抓SRID
                            cFile_Ext = bean.FILE_EXT.Trim();

                            if (cFile_Ext != ".pdf")
                            {
                                reValue[0] = "E";
                                reMsg += "檔名【" + bean.FILE_ORG_NAME + "】有誤，原因：格式不為PDF！ </br>";
                            }
                            else if (cSRID.Substring(0, 2) != "63")
                            {
                                reValue[0] = "E";
                                reMsg += "檔名【" + bean.FILE_ORG_NAME + "】有誤，原因：SRID不為63開頭！ </br>";
                            }
                            else
                            {
                                var beanSRM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == cSRID);

                                if (beanSRM != null)
                                {
                                    beanSRM.CAttachementStockNo = bean.ID + ",";

                                    beanSRM.ModifiedDate = DateTime.Now;
                                    beanSRM.ModifiedUserName = ViewBag.empEngName;

                                    int result = dbOne.SaveChanges();

                                    if (result <= 0)
                                    {
                                        reValue[0] = "E";
                                        reMsg += "檔名【" + bean.FILE_ORG_NAME + "】有誤，原因：儲存批次上傳裝機備料服務通知單失敗！ </br>";
                                    }
                                    else
                                    {
                                        reMsg += "檔名【" + bean.FILE_ORG_NAME + "】上傳成功！ </br>";
                                    }
                                }
                                else
                                {
                                    reValue[0] = "E";
                                    reMsg += "檔名【" + bean.FILE_ORG_NAME + "】有誤，原因：SRID【" + cSRID + "】無相關主檔資訊！ </br>";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            reValue[0] = "E";
                            reMsg += "檔名【" + bean.FILE_ORG_NAME + "】有誤，原因：" + ex.Message + " </br>";
                        }
                    }
                    #endregion

                    break;
                case "CONTRACT":
                    #region 合約書文件
                    foreach (var bean in tList)
                    {
                        try
                        {
                            cContractID = bean.FILE_ORG_NAME.Replace(bean.FILE_EXT.Trim(), "").Trim(); //去除副檔名
                            cFile_Ext = bean.FILE_EXT.Trim();

                            if (cFile_Ext != ".pdf")
                            {
                                reValue[0] = "E";
                                reMsg += "檔名【" + bean.FILE_ORG_NAME + "】有誤，原因：格式不為PDF！ </br>";
                            }
                            else
                            {
                                var beanCOM = dbOne.TbOneContractMains.FirstOrDefault(x => x.Disabled == 0 && x.CContractId == cContractID);

                                if (beanCOM != null)
                                {
                                    beanCOM.CContractReport = bean.ID + ",";

                                    beanCOM.ModifiedDate = DateTime.Now;
                                    beanCOM.ModifiedUserName = ViewBag.empEngName;

                                    int result = dbOne.SaveChanges();

                                    if (result <= 0)
                                    {
                                        reValue[0] = "E";
                                        reMsg += "檔名【" + bean.FILE_ORG_NAME + "】有誤，原因：儲存批次上傳合約書文件失敗！ </br>";
                                    }
                                    else
                                    {
                                        reMsg += "檔名【" + bean.FILE_ORG_NAME + "】上傳成功！ </br>";
                                    }
                                }
                                else
                                {
                                    reValue[0] = "E";
                                    reMsg += "檔名【" + bean.FILE_ORG_NAME + "】有誤，原因：文件編號【" + cContractID + "】無相關主數據資訊！ </br>";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            reValue[0] = "E";
                            reMsg += "檔名【" + bean.FILE_ORG_NAME + "】有誤，原因：" + ex.Message + " </br>";
                        }
                    }
                    #endregion

                    break;
            }

            reValue[1] = reMsg;

            return Json(reValue);
        }
        #endregion

        #endregion -----↑↑↑↑↑批次上傳裝機備料服務通知單 ↑↑↑↑↑-----

        #region -----↓↓↓↓↓批次上傳裝機派工作業 ↓↓↓↓↓----- 

        #region 匯入批次上傳裝機派工清單Excel
        [HttpPost]
        public IActionResult ImportBatchInstallExcel(IFormCollection formCollection, IFormFile postedFile)
        {   
            string[] AryValue = new string[2];

            bool tIsOK = true;
            bool tIsFormal = false;

            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;
            string tUrl = string.Empty;

            string cID = string.Empty;
            string tLog = string.Empty;
            string tErrorMsg = string.Empty;
            string strItem = string.Empty;  //記錄Excel第幾列
            string cSRID = string.Empty;
            string cCustomerID = string.Empty;
            string cCustomerName = string.Empty;
            string cSalesNo = string.Empty;
            string cShipmentNo = string.Empty;
            string cTeamID = string.Empty;
            string cContactName = string.Empty;
            string cContactAddress = string.Empty;
            string cContactPhone = string.Empty;
            string cContactMobile = string.Empty;
            string cContactEmail = string.Empty;
            string cSalesID = string.Empty;
            string cSecretaryID = string.Empty;
            string cMainEngineerID = string.Empty;
            string cSerialID = string.Empty;

            Dictionary<string, DataTable> DicM = new Dictionary<string, DataTable>();
            Dictionary<string, DataTable> DicD = new Dictionary<string, DataTable>();
            DataTable dtM = new DataTable();
            DataTable dtD = new DataTable();

            getLoginAccount();
            getEmployeeInfo();

            Guid cGUID = Guid.NewGuid(); //本次系統GUID

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;            
            #endregion

            try
            {

                #region 取得匯入Excel主檔相關(批次裝機派工)
                DicM = CMF.ImportExcelToDataTable(postedFile, "批次裝機派工", 0);

                foreach (KeyValuePair<string, DataTable> item in DicM)
                {
                    #region 回傳結果訊息
                    tErrorMsg = item.Key;
                    #endregion

                    #region 回傳的DataTable
                    dtM = item.Value.Clone();

                    foreach (DataRow dr in item.Value.Rows)
                    {
                        dtM.Rows.Add(dr.ItemArray);
                    }
                    #endregion

                    break;
                }
                #endregion

                #region 取得匯入Excel明細相關(物料訊息明細)
                DicD = CMF.ImportExcelToDataTable(postedFile, "物料訊息明細", 1);

                foreach (KeyValuePair<string, DataTable> item in DicD)
                {
                    #region 回傳結果訊息
                    tErrorMsg += item.Key;
                    #endregion

                    #region 回傳的DataTable
                    dtD = item.Value.Clone();

                    foreach (DataRow dr in item.Value.Rows)
                    {
                        dtD.Rows.Add(dr.ItemArray);
                    }
                    #endregion

                    break;
                }
                #endregion       

                if (tErrorMsg == "")
                {
                    #region 寫入DataTable到主檔資料庫  
                    if (dtM.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtM.Rows)
                        {
                            try
                            {
                                cSRID = "";
                                tIsOK = true;
                                strItem = dr[0].ToString().Trim();
                                cCustomerID = dr[1].ToString().Trim();
                                cCustomerName = dr[2].ToString().Trim();
                                cSalesNo = dr[3].ToString().Trim();
                                cShipmentNo = dr[4].ToString().Trim();
                                cTeamID = dr[5].ToString().Trim();
                                cContactName = dr[6].ToString().Trim();
                                cContactAddress = dr[7].ToString().Trim();
                                cContactPhone = dr[8].ToString().Trim();
                                cContactMobile = dr[9].ToString().Trim();
                                cContactEmail = dr[10].ToString().Trim();
                                cSalesID = dr[11].ToString().Trim();
                                cSecretaryID = dr[12].ToString().Trim();
                                cMainEngineerID = dr[13].ToString().Trim();
                                cSerialID = dr[14].ToString().Trim();

                                #region Empty欄位檢查
                                AryValue = CMF.ValidationImportExcelField(cCustomerID, "客戶代號", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cCustomerName, "客戶名稱", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cSalesNo, "SO訂單號碼", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cShipmentNo, "DN出貨單號碼", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cTeamID, "服務團隊ID", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }
                                else
                                {
                                    #region 檢查服務團隊ID是否存在
                                    string tTeamName = CMF.findTeamName(cTeamID);

                                    if (tTeamName == "")
                                    {
                                        tErrorMsg += "項次【" + strItem + "】服務團隊ID不存在 </br>";
                                        tIsOK = false;
                                    }
                                    #endregion
                                }

                                AryValue = CMF.ValidationImportExcelField(cContactName, "聯絡人姓名", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cContactAddress, "聯絡人地址", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cContactPhone + cContactMobile, "聯絡人電話或聯絡人手機", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cSalesID, "業務員工ERPID", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cSecretaryID, "業務祕書ERPID", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cMainEngineerID, "指派主要工程師ERPID", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }
                                #endregion

                                #region 呼叫建立ONE SERVICE報修SR（裝機服務）接口
                                if (tIsOK)
                                {
                                    SRMain_INSTALLSR_INPUT beanIN = new SRMain_INSTALLSR_INPUT();

                                    cCustomerName = CMF.findCustomerName(cCustomerID);
                                    string InternalExp = CMF.findInternalExpBySoNo(cSalesNo);
                                    string tNote = "【" + cShipmentNo + "】" + cCustomerName + " " + InternalExp + "_OneService系統批次裝機派單";

                                    #region 設定主檔                                    
                                    beanIN.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
                                    beanIN.IV_LOGINEMPNAME = ViewBag.empEngName;
                                    beanIN.IV_CUSTOMER = cCustomerID;
                                    beanIN.IV_SALESNO = cSalesNo;
                                    beanIN.IV_SHIPMENTNO = cShipmentNo;
                                    beanIN.IV_SALESEMPNO = cSalesID;
                                    beanIN.IV_SECRETARYEMPNO = cSecretaryID;
                                    beanIN.IV_DESC = tNote;
                                    beanIN.IV_LTXT = tNote;
                                    beanIN.IV_SRTEAM = cTeamID;
                                    beanIN.IV_EMPNO = cMainEngineerID;
                                    beanIN.IV_APIURLName = tAPIURLName;
                                    #endregion

                                    #region 取得聯絡人清單
                                    List<CREATECONTACTINFO> tListCON = new List<CREATECONTACTINFO>();
                                    
                                    CREATECONTACTINFO Con = new CREATECONTACTINFO();
                                    Con.CONTNAME = cContactName;
                                    Con.CONTADDR = cContactAddress;
                                    Con.CONTTEL = cContactPhone;
                                    Con.CONTMOBILE = cContactMobile;
                                    Con.CONTEMAIL = cContactEmail;
                                    
                                    tListCON.Add(Con);

                                    beanIN.CREATECONTACT_LIST = tListCON;
                                    #endregion

                                    #region 取得物料訊息明細清單
                                    List<CREATEMATERIAL> tListMA = new List<CREATEMATERIAL>();

                                    foreach (DataRow drD in dtD.Rows)
                                    {
                                        if (drD[0].ToString() == cShipmentNo)
                                        {
                                            CREATEMATERIAL MA = new CREATEMATERIAL();
                                            MA.MATERIALID = drD[1].ToString();
                                            MA.MATERIALNAME = string.IsNullOrEmpty(drD[2].ToString()) ? CMF.findMATERIALName(drD[1].ToString()) : drD[2].ToString();
                                            MA.QTY = drD[3].ToString();

                                            tListMA.Add(MA);                                            
                                        }
                                    }

                                    beanIN.CREATEMATERIAL_LIST = tListMA;
                                    #endregion

                                    #region 取得序號回報明細清單
                                    List<CREATEFEEDBACK> tListFB = new List<CREATEFEEDBACK>();

                                    if (cSerialID != "")
                                    {
                                        CREATEFEEDBACK FB = new CREATEFEEDBACK();
                                        FB.SERIALID = cSerialID;
                                        
                                        tListFB.Add(FB);
                                    }

                                    beanIN.CREATEFEEDBACK_LIST = tListFB;
                                    #endregion


                                    cSRID = CMF.callAPI_INSTALLSR_CREATE(beanIN);

                                    if (cSRID != "")
                                    {                                        
                                        #region 回寫批次上傳裝機派工紀錄(主檔)
                                        TbOneSrbatchInstallRecord BInsM = new TbOneSrbatchInstallRecord();

                                        BInsM.CGuid = cGUID;
                                        BInsM.CSrid = cSRID;
                                        BInsM.CCustomerId = cCustomerID;
                                        BInsM.CCustomerName = cCustomerName;
                                        BInsM.CTeamId = CMF.findSRTeamIDandName(cTeamID);
                                        BInsM.CContactName = cContactName;
                                        BInsM.CContactAddress = cContactAddress;
                                        BInsM.CContactPhone = cContactPhone;
                                        BInsM.CContactMobile = cContactMobile;
                                        BInsM.CContactEmail = cContactEmail;
                                        BInsM.CMainEngineerId = cMainEngineerID;
                                        BInsM.CMainEngineerName = CMF.findEmployeeName(cMainEngineerID);
                                        BInsM.CSalesId = cSalesID;
                                        BInsM.CSalesName = CMF.findEmployeeName(cSalesID);
                                        BInsM.CSecretaryId = cSecretaryID;
                                        BInsM.CSecretaryName = CMF.findEmployeeName(cSecretaryID);
                                        BInsM.CSalesNo = cSalesNo;
                                        BInsM.CShipmentNo = cShipmentNo;
                                        BInsM.CSerialId = cSerialID;

                                        BInsM.CreatedDate = DateTime.Now;
                                        BInsM.CreatedUserName = ViewBag.empEngName;

                                        dbOne.TbOneSrbatchInstallRecords.Add(BInsM);
                                        #endregion

                                        #region 回寫批次上傳裝機派工紀錄(明細檔)
                                        if (tListMA.Count > 0)
                                        {
                                            foreach (var MA in tListMA)
                                            {
                                                TbOneSrbatchInstallRecordDetail BInsD = new TbOneSrbatchInstallRecordDetail();

                                                BInsD.CSrid = cSRID;
                                                BInsD.CMaterialId = MA.MATERIALID;
                                                BInsD.CMaterialName = MA.MATERIALNAME;
                                                BInsD.CQuantity = int.Parse(MA.QTY);

                                                BInsD.CreatedDate = DateTime.Now;
                                                BInsD.CreatedUserName = ViewBag.empEngName;

                                                dbOne.TbOneSrbatchInstallRecordDetails.Add(BInsD);
                                            }
                                        }
                                        #endregion

                                        int result = dbOne.SaveChanges();

                                        if (result <= 0)
                                        {
                                            pMsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "回寫批次上傳裝機派工紀錄檔失敗" + Environment.NewLine;
                                            CMF.writeToLog(cSRID, "ImportBatchInstallExcel", pMsg, ViewBag.empEngName);
                                        }                                        
                                    }
                                }
                                #endregion
                            }
                            catch (Exception e)
                            {
                                tErrorMsg += "項次【" + strItem + "】，產生裝機服務失敗！原因：" + e.Message + "</br>";
                            }
                        }
                    }
                    else
                    {
                        tErrorMsg += "您未輸入任何批次上傳裝機派工清單！</br>";
                    }
                    #endregion
                }

                if (tErrorMsg == "")
                {
                    ViewBag.Message = "匯入成功！";
                }
                else
                {
                    ViewBag.Message = "匯入失敗！</br>" + tErrorMsg;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "匯入失敗！原因：" + ex.Message;
            }

            return RedirectToAction("QueryBatchInstall", new { cGUID = cGUID, tMessage = ViewBag.Message });
        }
        #endregion

        #region 批次上傳裝機派工清單查詢
        /// <summary>
        /// 批次上傳裝機派工清單查詢
        /// </summary>        
        /// <returns></returns>
        public IActionResult QueryBatchInstall()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            bool tIsFormal = false;

            string tONEURLName = string.Empty;
            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;
            string tUrl = string.Empty;            

            getLoginAccount();
            getEmployeeInfo();

            Guid cGUID = Guid.NewGuid();

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tONEURLName = ParaBean.ONEURLName;
            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;

            if (tIsFormal)
            {
                ViewBag.DownloadURL = tONEURLName + "/files/批次裝機派工.XLSX";
            }
            else
            {
                ViewBag.DownloadURL = "http://" + tAttachURLName + "/CSreport/批次裝機派工.XLSX";
            }
            #endregion

            #region Request參數            
            if (HttpContext.Request.Query["cGUID"].FirstOrDefault() != null)
            {
                cGUID = new Guid(HttpContext.Request.Query["cGUID"].FirstOrDefault());
            }            

            if (HttpContext.Request.Query["tMessage"].FirstOrDefault() != null)
            {
                //記錄匯入Excel成功或失敗訊息！
                ViewBag.Message = HttpContext.Request.Query["tMessage"].FirstOrDefault();
            }
            #endregion

            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            #region 組待查詢清單
            var beans = dbOne.TbOneSrbatchInstallRecords.OrderBy(x => x.CSrid).Where(x => x.CGuid == cGUID);

            foreach (var bean in beans)
            {
                string[] QueryInfo = new string[19];

                tUrl = "../ServiceRequest/InstallSR?SRID=" + bean.CSrid;

                QueryInfo[0] = bean.CSrid;               //SRID
                QueryInfo[1] = tUrl;                    //SRID URL                 
                QueryInfo[2] = bean.CCustomerId;         //客戶代號
                QueryInfo[3] = bean.CCustomerName;       //客戶名稱
                QueryInfo[4] = bean.CSalesNo;           //SO訂單號碼
                QueryInfo[5] = bean.CShipmentNo;         //DN出貨單號碼
                QueryInfo[6] = bean.CTeamId;            //服務團隊ID
                QueryInfo[7] = bean.CContactName;        //聯絡人姓名
                QueryInfo[8] = bean.CContactAddress;     //聯絡人地址
                QueryInfo[9] = bean.CContactPhone;       //聯絡人電話
                QueryInfo[10] = bean.CContactMobile;     //聯絡人手機
                QueryInfo[11] = bean.CContactEmail;      //聯絡人信箱
                QueryInfo[12] = bean.CSalesId;          //業務員工ERPID
                QueryInfo[13] = bean.CSalesName;         //業務員工姓名
                QueryInfo[14] = bean.CSecretaryId;       //業務祕書ERPID
                QueryInfo[15] = bean.CSecretaryName;     //業務祕書姓名
                QueryInfo[16] = bean.CMainEngineerId;    //指派主要工程師ERPID
                QueryInfo[17] = bean.CMainEngineerName;  //指派主要工程師姓名
                QueryInfo[18] = bean.CSerialId;         //序號      

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #endregion -----↑↑↑↑↑批次上傳裝機派工作業 ↑↑↑↑↑-----

        #region -----↓↓↓↓↓批次上傳定維派工作業 ↓↓↓↓↓----- 

        #region 匯入批次上傳定維派工清單Excel
        [HttpPost]
        public IActionResult ImportBatchMaintainExcel(IFormCollection formCollection, IFormFile postedFile)
        {
            string[] AryValue = new string[2];

            bool tIsOK = true;           
            bool tIsFormal = false;

            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;
            string tUrl = string.Empty;

            string cID = string.Empty;
            string tLog = string.Empty;
            string tErrorMsg = string.Empty;
            string strItem = string.Empty;  //記錄Excel第幾列
            string cContractID = string.Empty;
            string cBUKRS = string.Empty;
            string cCustomerID = string.Empty;
            string cCustomerName = string.Empty;
            string cContactStoreName = string.Empty;            
            string cContactName = string.Empty;
            string cContactAddress = string.Empty;
            string cContactPhone = string.Empty;
            string cContactMobile = string.Empty;
            string cContactEmail = string.Empty;
            string cContactCity = string.Empty;
            string cMainEngineerID = string.Empty;
            string cMainEngineerName = string.Empty;
            string cMACycle = string.Empty;
            string checkMACycle = string.Empty;

            int ImportCount = 0;    //記錄匯入有幾筆
            int EmptyCount = 0;     //記錄自訂週期Empty有幾筆

            Dictionary<string, DataTable> DicM = new Dictionary<string, DataTable>();          
            DataTable dtM = new DataTable();
           
            getLoginAccount();
            getEmployeeInfo();            

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;
            #endregion

            try
            {

                #region 取得匯入Excel主檔相關(批次定維派工)
                DicM = CMF.ImportExcelToDataTable(postedFile, "批次定維", 0);

                foreach (KeyValuePair<string, DataTable> item in DicM)
                {
                    #region 回傳結果訊息
                    tErrorMsg = item.Key;
                    #endregion

                    #region 回傳的DataTable
                    dtM = item.Value.Clone();

                    foreach (DataRow dr in item.Value.Rows)
                    {
                        dtM.Rows.Add(dr.ItemArray);
                    }
                    #endregion

                    break;
                }
                #endregion                

                if (tErrorMsg == "")
                {
                    #region 檢查匯入的資料
                    if (dtM.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtM.Rows)
                        {
                            try
                            {
                                ImportCount++;

                                tIsOK = true;
                                strItem = dr[0].ToString().Trim();
                                cContractID = dr[1].ToString().Trim();
                                cBUKRS = dr[2].ToString().Trim();
                                cCustomerID = dr[3].ToString().Trim();
                                cCustomerName = dr[4].ToString().Trim();
                                cContactStoreName = dr[5].ToString().Trim();
                                cContactName = dr[6].ToString().Trim();
                                cContactAddress = dr[7].ToString().Trim();
                                cContactPhone = dr[8].ToString().Trim();
                                cContactMobile = dr[9].ToString().Trim();
                                cContactEmail = dr[10].ToString().Trim();                                
                                cMainEngineerID = dr[11].ToString().Trim();
                                cMainEngineerName = dr[12].ToString().Trim();
                                cMACycle = dr[13].ToString().Trim();

                                #region Empty欄位檢查
                                AryValue = CMF.ValidationImportExcelField(cContractID, "文件編號", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cBUKRS, "公司別", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cCustomerID, "客戶代號", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cCustomerName, "客戶名稱", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cContactStoreName, "門市名稱", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cContactName, "聯絡人姓名", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cContactAddress, "聯絡人地址", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cContactPhone + cContactMobile, "聯絡人電話或聯絡人手機", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }                               

                                AryValue = CMF.ValidationImportExcelField(cMainEngineerID, "指派工程師ERPID", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                AryValue = CMF.ValidationImportExcelField(cMainEngineerName, "指派工程師姓名", strItem);

                                if (AryValue[0] == "N")
                                {
                                    tErrorMsg += AryValue[1];
                                    tIsOK = false;
                                }

                                if (cMACycle == "")
                                {
                                    EmptyCount++;
                                }
                                else
                                {
                                    checkMACycle = CMF.checkCycle(cMACycle).Replace("\n", "</br>");

                                    if (checkMACycle != "")
                                    {
                                        tErrorMsg += "項次【" + strItem + "】" + checkMACycle;
                                    }
                                }
                                #endregion                               
                            }
                            catch (Exception e)
                            {
                                tErrorMsg += "項次【" + strItem + "】，產生定維服務失敗！原因：" + e.Message + "</br>";
                            }
                        }
                    }
                    else
                    {
                        tErrorMsg += "您未輸入任何批次上傳定維派工清單！</br>";
                    }
                    #endregion

                    if (tErrorMsg == "")
                    {
                        if (EmptyCount != 0 && ImportCount != EmptyCount)
                        {
                            tErrorMsg += "自訂維護週期不一致，必須皆為【全空白或全輸入】！</br>";
                        }                       
                    }

                    #region 寫入DataTable到主檔資料庫  
                    if (dtM.Rows.Count > 0 && tErrorMsg == "")
                    {
                        foreach (DataRow dr in dtM.Rows)
                        {
                            try
                            {                               
                                strItem = dr[0].ToString().Trim();
                                cContractID = dr[1].ToString().Trim();
                                cBUKRS = dr[2].ToString().Trim();
                                cCustomerID = dr[3].ToString().Trim();
                                cCustomerName = CMF.findCustomerName(cCustomerID);
                                cContactStoreName = dr[5].ToString().Trim();
                                cContactName = dr[6].ToString().Trim();
                                cContactAddress = dr[7].ToString().Trim();
                                cContactPhone = dr[8].ToString().Trim();
                                cContactMobile = dr[9].ToString().Trim();
                                cContactEmail = dr[10].ToString().Trim();
                                cMainEngineerID = dr[11].ToString().Trim();
                                cMainEngineerName = dr[12].ToString().Trim();
                                cMACycle = dr[13].ToString().Trim().TrimEnd('_');

                                #region 新增/更新批次上傳定維派工紀錄主檔
                                int result = CMF.ChangeTB_ONE_SRBatchMaintainRecord(cContractID, cBUKRS, cCustomerID, cCustomerName, cContactStoreName,
                                                                                   cContactName, cContactAddress, cContactPhone, cContactMobile, cContactEmail,
                                                                                   cMainEngineerID, cMainEngineerName, cMACycle, ViewBag.empEngName);
                                if (result <= 0)
                                {
                                    pMsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "回寫批次上傳定維派工紀錄檔失敗" + Environment.NewLine;
                                    CMF.writeToLog(cContractID, "ImportBatchMaintainExcel", pMsg, ViewBag.empEngName);
                                }
                                #endregion

                                #region 更新回客戶聯絡人主檔(沒有重覆才要新增)
                                bool tIsExits = CMF.CheckContactsIsDouble("", cBUKRS, cCustomerID, cContactName, "");

                                if (!tIsExits)
                                {
                                    try
                                    {                                        
                                        cContactCity = cContactAddress.Substring(0, 3);
                                        cContactAddress = cContactAddress.Substring(3, cContactAddress.Length - 3);

                                        CONTACTCREATE_INPUT beanIN = new CONTACTCREATE_INPUT();
                                        beanIN.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
                                        beanIN.IV_LOGINEMPNAME = ViewBag.empEngName;
                                        beanIN.IV_CONTRACTID = cContractID;
                                        beanIN.IV_CUSTOMEID = cCustomerID;
                                        beanIN.IV_CONTACTNAME = cContactName;
                                        beanIN.IV_CONTACTCITY = cContactCity;
                                        beanIN.IV_CONTACTADDRESS = cContactAddress;
                                        beanIN.IV_CONTACTTEL = cContactPhone;
                                        beanIN.IV_CONTACTMOBILE = cContactMobile;
                                        beanIN.IV_CONTACTEMAIL = cContactEmail;
                                        beanIN.IV_ISDELETE = "N";
                                        beanIN.IV_APIURLName = tAPIURLName;

                                        CMF.GetAPI_CONTACT_UPDATE(beanIN);
                                    }
                                    catch(Exception ex)
                                    {
                                        //若有錯誤就不要塞
                                    }
                                }
                                #endregion
                            }
                            catch (Exception e)
                            {
                                tErrorMsg += "項次【" + strItem + "】，產生定維服務失敗！原因：" + e.Message + "</br>";                                
                                continue;
                            }
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
                    ViewBag.Message = "匯入失敗！</br>" + tErrorMsg;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "匯入失敗！原因：" + ex.Message;
            }

            return RedirectToAction("QueryBatchMaintain", new { cContractID = cContractID, tMessage = ViewBag.Message });
        }
        #endregion

        #region 批次上傳定維派工清單查詢
        /// <summary>
        /// 批次上傳定維派工清單查詢
        /// </summary>        
        /// <returns></returns>
        public IActionResult QueryBatchMaintain(string cContractID)
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            bool tIsFormal = false;

            string tONEURLName = string.Empty;
            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;

            getLoginAccount();
            getEmployeeInfo();

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tONEURLName = ParaBean.ONEURLName;
            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;           

            if (tIsFormal)
            {
                ViewBag.DownloadURL = tONEURLName + "/files/批次定維派工.XLSX";
            }
            else
            {
                ViewBag.DownloadURL = "http://" + tAttachURLName + "/CSreport/批次定維派工.XLSX";
            }
            #endregion

            #region Request參數
            if (HttpContext.Request.Query["tMessage"].FirstOrDefault() != null)
            {
                //記錄匯入Excel成功或失敗訊息！
                ViewBag.Message = HttpContext.Request.Query["tMessage"].FirstOrDefault();
            }
            #endregion

            #region 公司別            
            var BUKRSList = findSysParameterList(pSysOperationID, "OTHER", "ALL", "BUKRS", true);
            ViewBag.ddl_cBUKRS = BUKRSList;
            #endregion

            if (!string.IsNullOrEmpty(cContractID))
            {
                callQueryBatchMaintain(cContractID, "", "", "", "");
            }

            return View();
        }
        #endregion

        #region 批次上傳定維派工明細查詢結果
        /// <summary>
        /// 批次上傳定維派工明細查詢結果
        /// </summary>
        /// <param name="cContractID">文件編號</param>
        /// <param name="cCustomerID">客戶代號</param>
        /// <param name="cContactStoreName">門市名稱</param>
        /// <param name="cContactName">聯絡人姓名</param>        
        /// <param name="cMainEngineerID">指派工程師ERPID</param>        
        /// <returns></returns>
        public IActionResult QueryBatchMaintainResult(string cContractID, string cCustomerID, string cContactStoreName, string cContactName, string cMainEngineerID)
        {
            getLoginAccount();
            getEmployeeInfo();
            callQueryBatchMaintain(cContractID, cCustomerID, cContactStoreName, cContactName, cMainEngineerID);

            return View();
        }
        #endregion

        #region 批次上傳定維派工明細查詢共用方法
        /// <summary>
        /// 批次上傳定維派工明細查詢共用方法
        /// </summary>
        /// <param name="cContractID">文件編號</param>
        /// <param name="cCustomerID">客戶代號</param>
        /// <param name="cContactStoreName">門市名稱</param>
        /// <param name="cContactName">聯絡人姓名</param>        
        /// <param name="cMainEngineerID">指派工程師ERPID</param> 
        public void callQueryBatchMaintain(string cContractID, string cCustomerID, string cContactStoreName, string cContactName, string cMainEngineerID)
        { 
            string tUrl = string.Empty;

            List<string[]> QueryToList = new List<string[]>();  //查詢出來的清單          

            var beans = dbOne.TbOneSrbatchMaintainRecords.Where(x => x.CDisabled == 0 &&
                                                                (string.IsNullOrEmpty(cContractID) ? true : x.CContractId.Contains(cContractID)) &&
                                                                (string.IsNullOrEmpty(cCustomerID) ? true : x.CCustomerId == cCustomerID) &&
                                                                (string.IsNullOrEmpty(cContactStoreName) ? true : x.CContactStoreName.Contains(cContactStoreName)) &&
                                                                (string.IsNullOrEmpty(cContactName) ? true : x.CContactName.Contains(cContactName)) &&
                                                                (string.IsNullOrEmpty(cMainEngineerID) ? true : x.CMainEngineerId == cMainEngineerID));

            #region 組待查詢清單
            foreach (var bean in beans)
            {
                tUrl = "../Contract/ContractMain?ContractID=" + bean.CContractId;

                string[] QueryInfo = new string[15];
              
                QueryInfo[0] = bean.CId.ToString();           //系統ID
                QueryInfo[1] = bean.CContractId;              //文件編號
                QueryInfo[2] = tUrl;                         //URL
                QueryInfo[3] = bean.CBukrs;                   //公司別
                QueryInfo[4] = bean.CCustomerId;              //客戶代號
                QueryInfo[5] = bean.CCustomerName;            //客戶名稱
                QueryInfo[6] = bean.CContactStoreName;         //門市名稱
                QueryInfo[7] = bean.CContactName;             //聯絡人姓名
                QueryInfo[8] = bean.CContactAddress;          //聯絡人地址                
                QueryInfo[9] = bean.CContactPhone;            //聯絡人電話
                QueryInfo[10] = bean.CContactMobile;           //聯絡人手機                
                QueryInfo[11] = bean.CContactEmail;            //聯絡人信箱                
                QueryInfo[12] = bean.CMainEngineerId;         //指派工程師ERPID                
                QueryInfo[13] = bean.CMainEngineerName;       //指派工程師姓名                
                QueryInfo[14] = bean.CMacycle;               //自訂維護週期

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion
        }
        #endregion

        #region 儲存批次定維派工的明細內容
        /// <summary>
        /// 儲存批次定維派工的明細內容
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <param name="cBUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="cCustomerID">客戶代號</param>
        /// <param name="cCustomerName">客戶名稱</param>
        /// <param name="cContactStoreName">門市名稱</param>
        /// <param name="cContactName">聯絡人姓名</param>
        /// <param name="cContactAddress">聯絡人地址</param>
        /// <param name="cContactPhone">聯絡人電話</param>
        /// <param name="cContactMobile">聯絡人手機</param>
        /// <param name="cContactEmail">聯絡人信箱</param>
        /// <param name="cMainEngineerID">指派工程師ERPID</param>
        /// <param name="cMainEngineerName">指派工程師姓名</param>
        /// <param name="cMACycle">自訂維護週期</param>
        /// <returns></returns>
        public IActionResult saveBatchMaintain(string cID, string cContractID, string cBUKRS, string cCustomerID, string cCustomerName, 
                                             string cContactStoreName, string cContactName, string cContactAddress, string cContactPhone,
                                             string cContactMobile, string cContactEmail, string cMainEngineerID, string cMainEngineerName, string cMACycle)

        {
            int result = 0;                

            bool tIsDouble = false; //判斷是否有重覆
            bool tIsFormal = false;

            string reValue = "SUCCESS";
            string tLog = string.Empty;
            string tBPMURLName = string.Empty;
            string tAPIURLName = string.Empty;
            string tPSIPURLName = string.Empty;
            string tAttachURLName = string.Empty;
            string cContactCity = string.Empty;
            string OldcMainEngineerID = string.Empty;
            string OldcMainEngineerName = string.Empty;
            string OldcMACycle = string.Empty;

            cContactPhone = string.IsNullOrEmpty(cContactPhone) ? "" : cContactPhone.Trim();
            cContactMobile = string.IsNullOrEmpty(cContactMobile) ? "" : cContactMobile.Trim();
            cContactEmail = string.IsNullOrEmpty(cContactEmail) ? "" : cContactEmail.Trim();
            cMainEngineerID = string.IsNullOrEmpty(cMainEngineerID) ? "" : cMainEngineerID.Trim();
            cMainEngineerName = string.IsNullOrEmpty(cMainEngineerName) ? "" : cMainEngineerName.Trim();
            cMACycle = string.IsNullOrEmpty(cMACycle) ? "" : cMACycle.Trim();

            getLoginAccount();
            getEmployeeInfo();

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tBPMURLName = ParaBean.BPMURLName;
            tPSIPURLName = ParaBean.PSIPURLName;
            tAPIURLName = ParaBean.APIURLName;
            tAttachURLName = ParaBean.AttachURLName;            
            #endregion

            if (!string.IsNullOrEmpty(cID))
            {
                #region 修改
                var bean = dbOne.TbOneSrbatchMaintainRecords.FirstOrDefault(x => x.CId == int.Parse(cID));

                if (bean != null)
                {
                    #region 紀錄新舊值                       
                    OldcMainEngineerID = bean.CMainEngineerId;
                    tLog += CMF.getNewAndOldLog("指派工程師ERPID", OldcMainEngineerID, cMainEngineerID);

                    OldcMainEngineerName = bean.CMainEngineerName;
                    tLog += CMF.getNewAndOldLog("指派工程師姓名", OldcMainEngineerName, cMainEngineerName);

                    OldcMACycle = bean.CMacycle;
                    tLog += CMF.getNewAndOldLog("自訂維護週期", OldcMACycle, cMACycle);
                    #endregion

                    bean.CMainEngineerId = cMainEngineerID;
                    bean.CMainEngineerName = cMainEngineerName;
                    bean.CMacycle = cMACycle;

                    bean.ModifiedDate = DateTime.Now;
                    bean.ModifiedUserName = ViewBag.empEngName;

                    result = dbOne.SaveChanges();
                }
                #endregion
            }
            else //新增
            {
                //判斷批次上傳定維派工紀錄主檔是否已存在(true.已存在 false.未存在)
                tIsDouble = CMF.CheckBatchMaintainIsDouble(cContractID, cBUKRS, cCustomerID, cContactStoreName, cContactName);

                if (!tIsDouble)
                {
                    result = CMF.ChangeTB_ONE_SRBatchMaintainRecord(cContractID, cBUKRS, cCustomerID, cCustomerName, cContactStoreName,
                                                                  cContactName, cContactAddress, cContactPhone, cContactMobile, cContactEmail,
                                                                  cMainEngineerID, cMainEngineerName, cMACycle, ViewBag.empEngName);

                    #region 更新回客戶聯絡人主檔(沒有重覆才要新增)
                    bool tIsExits = CMF.CheckContactsIsDouble("", cBUKRS, cCustomerID, cContactName, "");

                    if (!tIsExits)
                    {
                        try
                        {
                            cContactCity = cContactAddress.Substring(0, 3);
                            cContactAddress = cContactAddress.Substring(3, cContactAddress.Length - 3);

                            CONTACTCREATE_INPUT beanIN = new CONTACTCREATE_INPUT();
                            beanIN.IV_LOGINEMPNO = ViewBag.cLoginUser_ERPID;
                            beanIN.IV_LOGINEMPNAME = ViewBag.empEngName;
                            beanIN.IV_CONTRACTID = cContractID;
                            beanIN.IV_CUSTOMEID = cCustomerID;
                            beanIN.IV_CONTACTNAME = cContactName;
                            beanIN.IV_CONTACTCITY = cContactCity;
                            beanIN.IV_CONTACTADDRESS = cContactAddress;
                            beanIN.IV_CONTACTTEL = cContactPhone;
                            beanIN.IV_CONTACTMOBILE = cContactMobile;
                            beanIN.IV_CONTACTEMAIL = cContactEmail;
                            beanIN.IV_ISDELETE = "N";
                            beanIN.IV_APIURLName = tAPIURLName;

                            CMF.GetAPI_CONTACT_UPDATE(beanIN);
                        }
                        catch (Exception ex)
                        {
                            //若有錯誤就不要塞
                        }
                    }
                    #endregion
                }
                else
                {
                    result = 1;
                    reValue = "【文件編號 + 客戶代號 + 門市名稱 + 聯絡人姓名】已存在，請重新再確認！";
                }
            }

            if (result <= 0)
            {
                pMsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "儲存失敗" + Environment.NewLine;
                CMF.writeToLog(cContractID, "saveBatchMaintain", pMsg, ViewBag.empEngName);
                reValue = pMsg;
            }
            else
            {
                if (!string.IsNullOrEmpty(tLog))
                {
                    CMF.writeToLog(cContractID, "saveBatchMaintain", tLog, ViewBag.empEngName);
                }
            }

            return Json(reValue);
        }
        #endregion

        #region 刪除批次定維派工的明細內容
        /// <summary>
        /// 刪除批次定維派工的明細內容
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <returns></returns>
        public ActionResult DeleteBatchMaintain(int cID)
        {
            int result = 0;

            getLoginAccount();
            getEmployeeInfo();

            var bean = dbOne.TbOneSrbatchMaintainRecords.FirstOrDefault(x => x.CId == cID);
            if (bean != null)
            {
                bean.CDisabled = 1;
                bean.ModifiedDate = DateTime.Now;
                bean.ModifiedUserName = ViewBag.empEngName;

                result = dbOne.SaveChanges();
            }

            return Json(result);
        }
        #endregion

        #region 檢查維護週期格式是否正確
        /// <summary>
        /// 檢查維護週期格式是否正確
        /// </summary>
        /// <param name="cMACycle">維護週期</param>
        /// <returns></returns>
        public IActionResult checkMACycle(string cMACycle)
        {
            string reValue = string.Empty;

            if (!string.IsNullOrEmpty(cMACycle))
            {
                reValue = CMF.checkCycle(cMACycle);
            }

            return Json(reValue);
        }
        #endregion

        #endregion -----↑↑↑↑↑批次上傳定維派工作業 ↑↑↑↑↑-----

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
            pIsSpareManager = CMF.getIsSpareManager(pLoginAccount);
            pIsBatchUploadSecretary = CMF.getIsBatchUploadSecretary(pLoginAccount, pOperationID_BatchUploadStockNo);
            pIsExePerson = CMF.getIsExePerson(pLoginAccount, pOperationID_QueryBatchInstall);
            pIsExeMaintainPerson = CMF.getIsExeMaintainPerson(pLoginAccount, pOperationID_QueryBatchMaintain);

            ViewBag.pIsMIS = pIsMIS;
            ViewBag.pIsCSManager = pIsCSManager;
            ViewBag.pIsCS = pIsCS;            
            ViewBag.pIsSpareManager = pIsSpareManager;
            ViewBag.pIsBatchUploadSecretary = pIsBatchUploadSecretary;
            ViewBag.pIsExePerson = pIsExePerson;
            ViewBag.pIsExeMaintainPerson = pIsExeMaintainPerson;
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
            ViewBag.empEngName = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName;
            
            pCompanyCode = EmpBean.BUKRS;
            pIsManager = EmpBean.IsManager;
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
        /// <param name="keyword">縣市</param>
        /// <param name="keyword2">鄉鎮市區</param>
        /// <param name="keyword3">路段(名)</param>
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

        #region Ajax路段(名)關鍵字查詢
        /// <summary>
        /// Ajax路段(名)關鍵字查詢
        /// </summary>
        /// <param name="keyword">縣市</param>
        /// <param name="keyword2">鄉鎮市區</param>
        /// <param name="keyword3">路段(名)關鍵字</param>
        /// <returns></returns>
        public IActionResult findPostalRoadsInfo(string keyword, string keyword2, string keyword3)
        {
            List<string> reLists = new List<string>();

            var result = (from p in dbProxy.PostalaAddressAndCodes
                                where p.City == keyword.Trim() && p.Township == keyword2.Trim() && p.Road.Contains(keyword3.Trim())
                                select p.Road).Distinct();

            reLists = result.ToList();

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
        /// <param name="tIDName">傳入的欄位Name</param>
        /// <param name="cBUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="CustomerID">客戶代號</param>        
        /// <returns></returns>
        public IActionResult FindContactInfo(string tIDName, string cBUKRS, string CustomerID)
        {
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            List<PCustomerContact> beans = CMF.GetContactInfo(cBUKRS, CustomerID);

            #region 組待查詢清單
            foreach (var bean in beans)
            {
                string[] QueryInfo = new string[8];

                QueryInfo[0] = bean.ContactID;
                QueryInfo[1] = tIDName;
                QueryInfo[2] = bean.Name;
                QueryInfo[3] = bean.City;
                QueryInfo[4] = bean.Address;
                QueryInfo[5] = bean.Email;
                QueryInfo[6] = bean.Phone;
                QueryInfo[7] = bean.Mobile;

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
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
        /// <param name="cEditContactStore">客戶聯絡人門市</param>
        /// <param name="ModifiedUserName">修改人姓名</param>        
        /// <returns></returns>
        public IActionResult SaveEditContactInfo(string cEditContactID, string cBUKRS, string cCustomerID, string cCustomerName, string cEditContactName,
                                               string cEditContactCity, string cEditContactAddress, string cEditContactPhone, string cEditContactMobile, 
                                               string cEditContactEmail, string cEditContactStore, string ModifiedUserName)
        {
            bool tIsDouble = false;

            string reValue = "SUCCESS";
            string tLog = string.Empty;
            string OldContactCity = string.Empty;
            string OldContactAddress = string.Empty;
            string OldContactPhone = string.Empty;
            string OldContactMobile = string.Empty;
            string OldContactEmail = string.Empty;
            string OldContactStore = string.Empty;

            getLoginAccount();
            getEmployeeInfo();

            cEditContactCity = string.IsNullOrEmpty(cEditContactCity) ? "" : cEditContactCity.Trim();
            cEditContactAddress = string.IsNullOrEmpty(cEditContactAddress) ? "" : cEditContactAddress.Trim();
            cEditContactPhone = string.IsNullOrEmpty(cEditContactPhone) ? "" : cEditContactPhone.Trim();
            cEditContactMobile = string.IsNullOrEmpty(cEditContactMobile) ? "" : cEditContactMobile.Trim();
            cEditContactEmail = string.IsNullOrEmpty(cEditContactEmail) ? "" : cEditContactEmail.Trim();
            cEditContactStore = string.IsNullOrEmpty(cEditContactStore) ? "" : cEditContactStore.Trim();

            try
            {
                #region 註解
                //tIsDouble = CMF.CheckContactsIsDoubleEmail(cEditContactID, cBUKRS, cCustomerID, cEditContactEmail);

                //if (tIsDouble)
                //{
                //    reValue = "聯絡人Email重覆，請重新確認！";                    
                //}
                //else
                //{
                //    if (cCustomerID.Substring(0, 1) == "P")
                //    {
                //        var bean = dbProxy.PersonalContacts.FirstOrDefault(x => x.ContactId.ToString() == cEditContactID);

                //        if (bean != null) //修改
                //        {
                //            #region 紀錄新舊值
                //            OldContactCity = bean.ContactCity;
                //            tLog += CMF.getNewAndOldLog("城市", OldContactCity, cEditContactCity);

                //            OldContactAddress = bean.ContactAddress;
                //            tLog += CMF.getNewAndOldLog("詳細地址", OldContactAddress, cEditContactAddress);

                //            OldContactPhone = bean.ContactPhone;
                //            tLog += CMF.getNewAndOldLog("電話", OldContactPhone, cEditContactPhone);

                //            OldContactMobile = bean.ContactMobile;
                //            tLog += CMF.getNewAndOldLog("手機", OldContactMobile, cEditContactMobile);

                //            OldContactEmail = bean.ContactEmail;
                //            tLog += CMF.getNewAndOldLog("Email", OldContactEmail, cEditContactEmail);
                //            #endregion

                //            bean.ContactCity = cEditContactCity;
                //            bean.ContactAddress = cEditContactAddress;
                //            bean.ContactPhone = cEditContactPhone;
                //            bean.ContactMobile = cEditContactMobile;
                //            bean.ContactEmail = cEditContactEmail;

                //            bean.ModifiedUserName = ModifiedUserName;
                //            bean.ModifiedDate = DateTime.Now;                            
                //        }
                //    }
                //    else
                //    {
                //        var bean = dbProxy.CustomerContacts.FirstOrDefault(x => x.ContactId.ToString() == cEditContactID);

                //        if (bean != null) //修改
                //        {
                //            #region 紀錄新舊值
                //            OldContactCity = bean.ContactCity;
                //            tLog += CMF.getNewAndOldLog("城市", OldContactCity, cEditContactCity);

                //            OldContactAddress = bean.ContactAddress;
                //            tLog += CMF.getNewAndOldLog("詳細地址", OldContactAddress, cEditContactAddress);

                //            OldContactPhone = bean.ContactPhone;
                //            tLog += CMF.getNewAndOldLog("電話", OldContactPhone, cEditContactPhone);

                //            OldContactMobile = bean.ContactMobile;
                //            tLog += CMF.getNewAndOldLog("手機", OldContactMobile, cEditContactMobile);

                //            OldContactEmail = bean.ContactEmail;
                //            tLog += CMF.getNewAndOldLog("Email", OldContactEmail, cEditContactEmail);
                //            #endregion

                //            bean.ContactCity = cEditContactCity;
                //            bean.ContactAddress = cEditContactAddress;
                //            bean.ContactPhone = cEditContactPhone;
                //            bean.ContactMobile = cEditContactMobile;
                //            bean.ContactEmail = cEditContactEmail;

                //            bean.ModifiedUserName = ModifiedUserName;
                //            bean.ModifiedDate = DateTime.Now;                            
                //        }
                //    }

                //    int result = dbProxy.SaveChanges();

                //    if (result <= 0)
                //    {
                //        reValue = "提交失敗(編輯)";
                //    }                    
                //}
                #endregion

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

                        bean.ModifiedUserName = ViewBag.empEngName;
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

                        OldContactStore = bean.ContactStore.ToString();
                        tLog += CMF.getNewAndOldLog("門市", OldContactStore, cEditContactStore);
                        #endregion

                        bean.ContactCity = cEditContactCity;
                        bean.ContactAddress = cEditContactAddress;
                        bean.ContactPhone = cEditContactPhone;
                        bean.ContactMobile = cEditContactMobile;
                        bean.ContactEmail = cEditContactEmail;

                        if (cEditContactStore != "")
                        {
                            bean.ContactStore = Guid.Parse(cEditContactStore);
                        }                        

                        bean.ModifiedUserName = ViewBag.empEngName;
                        bean.ModifiedDate = DateTime.Now;
                    }
                }

                int result = dbProxy.SaveChanges();

                if (result <= 0)
                {
                    reValue = "提交失敗(編輯)";
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
        /// <param name="cAddContactStore">客戶聯絡人門市</param>
        /// <param name="ModifiedUserName">修改人姓名</param>        
        /// <returns></returns>
        public IActionResult SaveContactInfo(string cBUKRS, string cCustomerID, string cCustomerName, string cAddContactName,
                                           string cAddContactCity, string cAddContactAddress, string cAddContactPhone, string cAddContactMobile, 
                                           string cAddContactEmail, string cAddContactStore, string ModifiedUserName)
        {
            bool tIsDouble = false;

            string tBpmNo = "GenerallySR";
            string reValue = "SUCCESS";
            string tLog = string.Empty;

            getLoginAccount();
            getEmployeeInfo();

            cAddContactName = string.IsNullOrEmpty(cAddContactName) ? "" : cAddContactName.Trim();
            cAddContactCity = string.IsNullOrEmpty(cAddContactCity) ? "" : cAddContactCity.Trim();
            cAddContactAddress = string.IsNullOrEmpty(cAddContactAddress) ? "" : cAddContactAddress.Trim();
            cAddContactPhone = string.IsNullOrEmpty(cAddContactPhone) ? "" : cAddContactPhone.Trim();
            cAddContactMobile = string.IsNullOrEmpty(cAddContactMobile) ? "" : cAddContactMobile.Trim();
            cAddContactEmail = string.IsNullOrEmpty(cAddContactEmail) ? "" : cAddContactEmail.Trim();
            cAddContactStore = string.IsNullOrEmpty(cAddContactStore) ? "" : cAddContactStore.Trim();

            try
            {
                tIsDouble = CMF.CheckContactsIsDouble("", cBUKRS, cCustomerID, cAddContactName, cAddContactStore);

                if (tIsDouble)
                {
                    reValue = "聯絡人重覆，請重新確認！";
                }
                else
                {
                    if (cCustomerID.Substring(0, 1) == "P")
                    {
                        #region 個人客戶
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

                        bean1.CreatedUserName = ViewBag.empEngName;
                        bean1.CreatedDate = DateTime.Now;

                        dbProxy.PersonalContacts.Add(bean1);
                        #endregion
                    }
                    else
                    {
                        #region 法人客戶
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

                        if (cAddContactStore != "")
                        {
                            bean1.ContactStore = Guid.Parse(cAddContactStore);
                        }

                        bean1.BpmNo = tBpmNo;
                        bean1.Disabled = 0;

                        bean1.ModifiedUserName = ViewBag.empEngName;
                        bean1.ModifiedDate = DateTime.Now;

                        dbProxy.CustomerContacts.Add(bean1);
                        #endregion
                    }

                    int result = dbProxy.SaveChanges();

                    if (result <= 0)
                    {
                        reValue = "提交失敗(新增)";
                    }
                }
            }
            catch(Exception e)
            {
                reValue = "SaveContactInfo Error：" + e.Message;
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

        #region Ajax依關鍵字查詢物料資訊(for備品)
        /// <summary>
        /// Ajax依關鍵字查詢物料資訊(for備品)
        /// </summary>
        /// <param name="cBUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="keyword">關鍵字</param>        
        /// <returns></returns>
        public IActionResult findMaterialSpare(string cBUKRS, string keyword)
        {
            Object contentObj = CMF.findMaterialSpareByKeyWords(cBUKRS, keyword);

            return Json(contentObj);
        }
        #endregion

        #region Ajax用中文或英文姓名查詢人員(在職人員)
        /// <summary>
        /// Ajax用中文或英文姓名查詢人員(在職人員)
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

        #region Ajax用中文或英文姓名查詢人員(含離職人員)
        /// <summary>
        /// Ajax用中文或英文姓名查詢人員(含離職人員)
        /// </summary>
        /// <param name="keyword">中文/英文姓名</param>        
        /// <returns></returns>
        public IActionResult AjaxfindEmployeeInCludeLeaveByKeyword(string keyword)
        {
            List<TblEmployee> tList = new List<TblEmployee>();

            var beans = bpmDB.TblEmployees.Where(x => (x.CEmployeeAccount.Contains(keyword) || x.CEmployeeCName.Contains(keyword))).Take(5).ToList();

            foreach(var bean in beans)
            {
                if (bean.CEmployeeLeaveDay != null || bean.CEmployeeLeaveReason != null)
                {
                    bean.CEmployeeEname += "(離職)";
                }
            }

            tList = beans.ToList();

            string json = JsonConvert.SerializeObject(tList);
            return Content(json, "application/json");
        }
        #endregion

        #region Ajax用中文或英文姓名查詢人員(by新的服務團隊)
        /// <summary>
        /// Ajax用中文或英文姓名查詢人員(by新的服務團隊)
        /// </summary>
        /// <param name="cTeamID">服務團隊ID</param>
        /// <param name="keyword">中文/英文姓名</param>        
        /// <returns></returns>
        public IActionResult AjaxfindTeamEmployeeByKeyword(string cTeamID, string keyword)
        {
            object contentObj = null;
            
            List<string> tList = CMF.findALLDeptIDListbyTeamID(cTeamID);            

            contentObj = dbEIP.People.Where(x => tList.Contains(x.DeptId) &&
                                               (x.Account.Contains(keyword) || x.Name2.Contains(keyword)) &&
                                               (x.LeaveReason == null && x.LeaveDate == null)).Take(5);            

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

            if (cCustomerID.Substring(0, 1) == "P") //個人客戶
            {
                contentObj = dbProxy.PersonalContacts.Where(x => x.Disabled == 0 && x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == cCustomerID && x.ContactName.Contains(keyword));
            }
            else //法人客戶
            {
                contentObj = dbProxy.CustomerContacts.Where(x => (x.Disabled == null || x.Disabled != 1) &&
                                                              x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == cCustomerID && x.ContactName.Contains(keyword));
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

        #region Ajax用合約文件編號查詢業務人員和祕書
        /// <summary>
        ///  Ajax用合約文件編號查詢業務人員和祕書
        /// </summary>
        /// <param name="cContractID">合約文件編號</param>        
        /// <returns></returns>
        public IActionResult AjaxfindContractIDInfo(string cContractID)
        {
            string[] AryValue = new string[6];
            AryValue[0] = "";   //回傳是否正確(Y.正確 N.有錯誤)
            AryValue[1] = "";   //有錯誤訊息就回傳，沒有就是空白
            AryValue[2] = "";   //業務員ERPID
            AryValue[3] = "";   //業務員姓名
            AryValue[4] = "";   //業務祕書ERPID
            AryValue[5] = "";   //業務祕書姓名

            cContractID = string.IsNullOrEmpty(cContractID) ? "" : cContractID.Trim();

            var bean = dbOne.TbOneContractMains.FirstOrDefault(x => x.Disabled == 0 && x.CContractId == cContractID);

            if (bean != null)
            {
                AryValue[0] = "Y";
                AryValue[1] = "";
                AryValue[2] = bean.CSoSales;
                AryValue[3] = bean.CSoSalesName;
                AryValue[4] = bean.CSoSalesAss;
                AryValue[5] = bean.CSoSalesAssname;
            }
            else
            {
                AryValue[0] = "N";
                AryValue[1] = "您輸入的合約文件編號不存在或已作廢，請重新確認！";
                AryValue[2] = "";
                AryValue[3] = "";
                AryValue[4] = "";
                AryValue[5] = "";
            }

            return Json(AryValue);
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

        #region 檢查【姓名和ERPID】是否不一致
        /// <summary>
        /// 檢查【姓名和ERPID】是否不一致
        /// </summary>
        /// <param name="cERPID">ERPID</param>
        /// <param name="cEName">中英文姓名</param>
        /// <param name="tType">M.主要工程師 S.業務人員 ASS.祕書人員</param>
        /// <returns></returns>
        public IActionResult checkPersonIsMatch(string cERPID, string cEName, string tType)
        {
            string reValue = string.Empty;
            string title = string.Empty;

            switch(tType)
            {
                case "M":
                    title = "主要工程師";
                    break;
                case "S":
                    title = "業務";
                    break;
                case "ASS":
                    title = "祕書";
                    break;
            }

            cERPID = string.IsNullOrEmpty(cERPID) ? "" : cERPID;

            if (cERPID != "")
            {
                string tName = CMF.findEmployeeCNameAndEName(cERPID);

                if (tName.ToLower() != cEName.Trim().ToLower())
                {
                    reValue = title + "【" + cERPID + "】和姓名【" + cEName + "】不一致，請輸入關鍵字後再重新選取！\n";
                }
            }

            return Json(reValue);
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

            #region 故障報修等級
            public string ddl_cSRRepairLevel { get; set; }
            public List<SelectListItem> ListSRRepairLevel = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "SRREPAIRLEVEL", false);
            #endregion

            #region 是否為內部作業
            public string ddl_cIsInternalWork { get; set; }
            public List<SelectListItem> ListIsInternalWork = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "ISINTERNALWORK", false);
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

        #region DropDownList選項Class(定維服務)
        /// <summary>
        /// DropDownList選項Class(定維服務)
        /// </summary>
        public class ViewModel_Maintain
        {
            #region 狀態
            public string ddl_cStatus { get; set; }

            //不抓DB參數的設定
            //public List<SelectListItem> ListStatus { get; } = new List<SelectListItem>
            //{                
            //    new SelectListItem { Value = "E0001", Text = "新建" },
            //    new SelectListItem { Value = "E0016", Text = "定保處理中" },
            //    new SelectListItem { Value = "E0017", Text = "定保完成" },            
            //    new SelectListItem { Value = "E0014", Text = "駁回" },
            //    new SelectListItem { Value = "E0015", Text = "取消" },                
            //};

            public List<SelectListItem> ListStatus = findSysParameterList(pOperationID_MaintainSR, "OTHER", pCompanyCode, "SRSTATUS", false);
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
            public List<SelectListItem> ListTeamID = findSRTeamMappingListItem("ALL", true);
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

        #region DropDownList選項Class(服務進度查詢)
        /// <summary>
        /// DropDownList選項Class(服務進度查詢)
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

            #region 是否為二修
            public string ddl_cIsSecondFix { get; set; }
            public List<SelectListItem> ListIsSecondFix = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "ISSECONDFIX", true);
            #endregion
        }
        #endregion

        #region DropDownList選項Class(服務總表)
        /// <summary>
        /// DropDownList選項Class(服務總表)
        /// </summary>
        public class ViewModelSRReport
        {
            #region 是否為二修
            public string ddl_cIsSecondFix { get; set; }
            public List<SelectListItem> ListIsSecondFix = findSysParameterList(pOperationID_GenerallySR, "OTHER", pCompanyCode, "ISSECONDFIX", true);
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

    #region 客戶聯絡人定義
    /// <summary>客戶聯絡人</summary>
    public class PCustomerContact
    {
        /// <summary>GUID</summary>
        public string ContactID { get; set; }

        /// <summary>客戶ID</summary>
        public string CustomerID { get; set; }

        /// <summary>客戶名稱</summary>
        public string CustomerName { get; set; }

        /// <summary>公司別</summary>
        public string BUKRS { get; set; }

        /// <summary>聯絡人姓名</summary>
        public string Name { get; set; }

        /// <summary>聯絡人居住城市</summary>
        public string City { get; set; }

        /// <summary>聯絡人地址</summary>
        public string Address { get; set; }

        /// <summary>聯絡人Email</summary>
        public string Email { get; set; }

        /// <summary>聯絡人電話</summary>
        public string Phone { get; set; }

        /// <summary>聯絡人手機</summary>
        public string Mobile { get; set; }

        /// <summary>聯絡人門市</summary>
        public string Store { get; set; }

        /// <summary>來源表單</summary>
        public string BPMNo { get; set; }
    }
    #endregion

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
        /// <summary>OneService URL</summary>
        public string ONEURLName { get; set; }        
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

    #region 更新裝機現況資訊相關INPUT資訊
    /// <summary>更新裝機現況資訊相關INPUT資訊</summary>
    public struct CURRENTINSTALLINFO_INPUT
    {
        /// <summary>登入者員工編號</summary>
        public string IV_LOGINEMPNO { get; set; }
        /// <summary>修改者員工姓名(中文+英文)</summary>
        public string IV_LOGINEMPNAME { get; set; }
        /// <summary>服務案件ID</summary>
        public string IV_SRID { get; set; }
        /// <summary>系統ID</summary>
        public string IV_CID { get; set; }
        /// <summary>裝機起始日期</summary>
        public string IV_InstallDate { get; set; }
        /// <summary>裝機完成日期</summary>
        public string IV_ExpectedDate { get; set; }
        /// <summary>總安裝數量</summary>
        public string IV_TotalQuantity { get; set; }
        /// <summary>已安裝數量</summary>
        public string IV_InstallQuantity { get; set; }
        /// <summary>是否來自APP更新(Y.是 N.否)</summary>
        public string IV_IsFromAPP { get; set; }
        /// <summary>APIURL開頭網址</summary>
        public string IV_APIURLName { get; set; }
    }
    #endregion

    #region 更新裝機現況資訊相關OUTPUT資訊
        /// <summary>更新裝機現況資訊相關OUTPUT資訊</summary>
        public struct CURRENTINSTALLINFO_OUTPUT
        {
            /// <summary>消息類型(E.處理失敗 Y.處理成功)</summary>
            public string EV_MSGT { get; set; }
            /// <summary>消息內容</summary>
            public string EV_MSG { get; set; }            
        }
        #endregion

    #region 查詢是否可以讀取合約書PDF權限資料INPUT資訊
    /// <summary>查詢是否可以讀取合約書PDF權限資料INPUT資訊</summary>
    public struct VIEWCONTRACTSMEMBERSINFO_INPUT
    {
        /// <summary>登入者員工編號ERPID</summary>
        public string IV_LOGINEMPNO { get; set; }
        /// <summary>修改者員工姓名(中文+英文)</summary>
        public string IV_LOGINEMPNAME { get; set; }
        /// <summary>文件編號</summary>
        public string IV_CONTRACTID { get; set; }
        /// <summary>服務團隊代碼</summary>
        public string IV_SRTEAM { get; set; }
        /// <summary>APIURL開頭網址</summary>
        public string IV_APIURLName { get; set; }
    }
    #endregion

    #region 查詢是否可以讀取合約書PDF權限資料OUTPUT資訊
    /// <summary>查詢是否可以讀取合約書PDF權限資料OUTPUT資訊</summary>
    public struct VIEWCONTRACTSMEMBERSINFO_OUTPUT
    {
        /// <summary>消息類型(E.處理失敗 Y.處理成功)</summary>
        public string EV_MSGT { get; set; }
        /// <summary>消息內容</summary>
        public string EV_MSG { get; set; }
        /// <summary>是否可以讀取合約書PDF(Y.是 N.否)</summary>
        public string EV_IsCanRead { get; set; }
    }
    #endregion

    #region 裝機服務案件主檔INPUT資訊
    /// <summary>裝機服務案件主檔INPUT資訊</summary>
    public struct SRMain_INSTALLSR_INPUT
    {
        /// <summary>建立者員工編號ERPID</summary>
        public string IV_LOGINEMPNO { get; set; }
        /// <summary>建立者員工姓名</summary>
        public string IV_LOGINEMPNAME { get; set; }
        /// <summary>客戶ID</summary>
        public string IV_CUSTOMER { get; set; }
        /// <summary>服務團隊ID</summary>
        public string IV_SRTEAM { get; set; }
        /// <summary>銷售訂單號</summary>
        public string IV_SALESNO { get; set; }
        /// <summary>出貨單號</summary>
        public string IV_SHIPMENTNO { get; set; }
        /// <summary>服務案件說明</summary>
        public string IV_DESC { get; set; }
        /// <summary>詳細描述</summary>
        public string IV_LTXT { get; set; }        
        /// <summary>主要工程師員工編號</summary>
        public string IV_EMPNO { get; set; }
        /// <summary>業務人員員工編號</summary>
        public string IV_SALESEMPNO { get; set; }
        /// <summary>業務祕書員工編號</summary>
        public string IV_SECRETARYEMPNO { get; set; }
        /// <summary>APIURLName</summary>
        public string IV_APIURLName { get; set; }

        /// <summary>服務案件客戶聯絡人資訊</summary>
        public List<CREATECONTACTINFO> CREATECONTACT_LIST { get; set; }
        /// <summary>服務案件物料訊息資訊</summary>
        public List<CREATEMATERIAL> CREATEMATERIAL_LIST { get; set; }
        /// <summary>服務案件序號回報資訊</summary>
        public List<CREATEFEEDBACK> CREATEFEEDBACK_LIST { get; set; }
    }
    #endregion

    #region 裝機服務主檔OUTPUT資訊
    /// <summary>裝機服務主檔OUTPUT資訊</summary>
    public struct SRMain_INSTALLSR_OUTPUT
    {
        /// <summary>SRID</summary>
        public string EV_SRID { get; set; }
        /// <summary>消息類型(E.處理失敗 Y.處理成功)</summary>
        public string EV_MSGT { get; set; }
        /// <summary>消息內容</summary>
        public string EV_MSG { get; set; }
    }
    #endregion

    #region 建立客戶聯絡人資訊
    /// <summary>建立客戶聯絡人資訊</summary>
    public class CREATECONTACTINFO
    {
        /// <summary>聯絡人姓名</summary>
        public string CONTNAME { get; set; }
        /// <summary>聯絡人地址</summary>
        public string CONTADDR { get; set; }
        /// <summary>聯絡人電話</summary>
        public string CONTTEL { get; set; }
        /// <summary>聯絡人手機</summary>
        public string CONTMOBILE { get; set; }
        /// <summary>聯絡人信箱</summary>
        public string CONTEMAIL { get; set; }

    }
    #endregion

    #region 建立物料訊息資訊
    /// <summary>建立物料訊息資訊</summary>
    public class CREATEMATERIAL
    {
        /// <summary>物料編號</summary>
        public string MATERIALID { get; set; }
        /// <summary>物料說明</summary>
        public string MATERIALNAME { get; set; }
        /// <summary>物料數量</summary>
        public string QTY { get; set; }
    }
    #endregion

    #region 建立序號回報資訊
    /// <summary>建立序號回報資訊</summary>
    public class CREATEFEEDBACK
    {
        /// <summary>序號</summary>
        public string SERIALID { get; set; }
    }
    #endregion

    #region 法人客戶聯絡人資料新增INPUT資訊
    /// <summary>法人客戶聯絡人資料新增資料INPUT資訊</summary>
    public struct CONTACTCREATE_INPUT
    {
        /// <summary>建立者員工編號ERPID</summary>
        public string IV_LOGINEMPNO { get; set; }
        /// <summary>建立者員工姓名</summary>
        public string IV_LOGINEMPNAME { get; set; }
        /// <summary>文件編號</summary>
        public string IV_CONTRACTID { get; set; }
        /// <summary>法人客戶代號</summary>
        public string IV_CUSTOMEID { get; set; }        
        /// <summary>聯絡人姓名</summary>
        public string IV_CONTACTNAME { get; set; }
        /// <summary>聯絡人城市</summary>
        public string IV_CONTACTCITY { get; set; }
        /// <summary>聯絡人地址</summary>
        public string IV_CONTACTADDRESS { get; set; }
        /// <summary>聯絡人電話</summary>
        public string IV_CONTACTTEL { get; set; }
        /// <summary>聯絡人手機</summary>
        public string IV_CONTACTMOBILE { get; set; }
        /// <summary>聯絡人Email</summary>
        public string IV_CONTACTEMAIL { get; set; }        
        /// <summary>是否要刪除</summary>
        public string IV_ISDELETE { get; set; }
        /// <summary>APIURLName</summary>
        public string IV_APIURLName { get; set; }
    }
    #endregion

    #region 法人客戶聯絡人資料新增OUTPUT資訊
    /// <summary>法人客戶聯絡人資料新增OUTPUT資訊</summary>
    public struct CONTACTCREATE_OUTPUT
    {
        /// <summary>消息類型(E.處理失敗 Y.處理成功)</summary>
        public string EV_MSGT { get; set; }
        /// <summary>消息內容</summary>
        public string EV_MSG { get; set; }
    }
    #endregion

}
