using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneService.Models;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Security.Principal;
using RestSharp;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static OneService.Controllers.ServiceRequestController;
using System.Diagnostics.Metrics;
using System.Security.Policy;
using static System.Net.WebRequestMethods;
using System.Net.NetworkInformation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text;
using NuGet.Packaging.Signing;
using System.Numerics;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace OneService.Controllers
{
    public class CommonFunction
    {
        PSIPContext psipDb = new PSIPContext();
        TSTIONEContext dbOne = new TSTIONEContext();
        TAIFContext bpmDB = new TAIFContext();
        ERP_PROXY_DBContext dbProxy = new ERP_PROXY_DBContext();
        MCSWorkflowContext dbEIP = new MCSWorkflowContext();       

        /// <summary>
        /// 呼叫SAPERP正式區或測試區(true.正式區 false.測試區)
        /// </summary>
        bool pIsFormal = false;

        public CommonFunction()
        {
            
        }

        #region -----↓↓↓↓↓待辦清單 ↓↓↓↓↓-----

        #region 取得登入人員所有要負責的SRID
        /// <summary>
        /// 取得登入人員所有要負責的SRID
        /// </summary>
        /// <param name="cOperationID_GenerallySR">程式作業編號檔系統ID(一般)</param>
        /// <param name="cOperationID_InstallSR">程式作業編號檔系統ID(裝機)</param>
        /// <param name="cOperationID_MaintainSR">程式作業編號檔系統ID(定維)</param>
        /// <param name="cCompanyID">公司別</param>
        /// <param name="IsManager">true.管理員 false.非管理員</param>
        /// <param name="tERPID">登入人員ERPID</param>
        /// <param name="tTeamList">可觀看服務團隊清單</param>        
        /// <returns></returns>
        public List<string[]> findSRIDList(string cOperationID_GenerallySR, string cOperationID_InstallSR, string cOperationID_MaintainSR, 
                                         string cCompanyID, bool IsManager, string tERPID, List<string> tTeamList)
        {            

            List<string[]> SRIDUserToList = new List<string[]>();   //組SRID清單

            SRIDUserToList = getSRIDToDoList(cOperationID_GenerallySR, cOperationID_InstallSR, cOperationID_MaintainSR, cCompanyID, IsManager, tERPID, tTeamList);

            return SRIDUserToList;
        }
        #endregion

        #region 取得一般服務SRID負責清單
        /// <summary>
        /// 取得一般服務SRID負責清單
        /// </summary>
        /// <param name="cOperationID_GenerallySR">程式作業編號檔系統ID(一般)</param>
        /// <param name="cOperationID_InstallSR">程式作業編號檔系統ID(裝機)</param>
        /// <param name="cOperationID_MaintainSR">程式作業編號檔系統ID(定維)</param>
        /// <param name="cCompanyID">公司別</param>
        /// <param name="IsManager">true.管理員 false.非管理員</param>
        /// <param name="tERPID">登入人員ERPID</param>
        /// <param name="tTeamList">可觀看服務團隊清單</param>
        /// <returns></returns>
        private List<string[]> getSRIDToDoList(string cOperationID_GenerallySR, string cOperationID_InstallSR, string cOperationID_MaintainSR, 
                                             string cCompanyID, bool IsManager, string tERPID, List<string> tTeamList)
        {
            List<string[]> SRIDUserToList = new List<string[]>();   //組SRID清單

            string tSRContactName = string.Empty;       //客戶聯絡人
            string tSRPathWay = string.Empty;           //報修管理
            string tSRType = string.Empty;              //報修類別
            string tMainEngineerID = string.Empty;      //主要工程師ERPID
            string tMainEngineerName = string.Empty;    //主要工程師姓名            
            string tAssEngineerName = string.Empty;     //協助工程師姓名
            string tTechManagerID = string.Empty;       //技術主管ERPID            
            string tTechManagerName = string.Empty;     //技術主管姓名
            string tSalesID = string.Empty;             //業務人員ERPID
            string tSecretaryID = string.Empty;         //業務祕書ERPID
            string tModifiedDate = string.Empty;        //修改日期

            List<string> tListAssAndTech = new List<string>();                          //記錄所有協助工程師和所有技術主管的ERPID
            Dictionary<string, string> tDicAssAndTech = new Dictionary<string, string>();  //記錄所有協助工程師和所有技術主管的<ERPID,中、英文姓名>

            var tSRContact_List = findSRDetailContactList();

            List<TbOneSrmain> beans = new List<TbOneSrmain>();
            List<TbOneSysParameter> tSRPathWay_List = findSysParameterALLDescription(cOperationID_GenerallySR, "OTHER", cCompanyID, "SRPATH");

            if (IsManager)
            {
                string tWhere = TrnasTeamListToWhere(tTeamList);

                string tSQL = @"select * from TB_ONE_SRMain
                                   where 
                                   (cStatus <> 'E0015' and cStatus <> 'E0006' and cStatus <> 'E0010') and 
                                   (
                                        (
                                            (CMainEngineerId = '{0}') or (cSalesID = '{0}') or (cSecretaryID = '{0}') or (cTechManagerID like '%{0}%')
                                        )
                                        {1}
                                   )";

                tSQL = string.Format(tSQL, tERPID, tWhere);

                DataTable dt = getDataTableByDb(tSQL, "dbOne");

                #region 先取得所有協助工程師和技術主管的ERPID
                foreach (DataRow dr in dt.Rows)
                {
                    #region 協助工程師
                    findListAssAndTech(ref tListAssAndTech, dr["cAssEngineerID"].ToString());
                    #endregion

                    #region 技術主管
                    findListAssAndTech(ref tListAssAndTech, dr["cTechManagerID"].ToString());
                    #endregion
                }
                #endregion

                #region 再取得所有協助工程師和技術主管的中文姓名
                tDicAssAndTech = findListEmployeeInfo(tListAssAndTech);
                #endregion

                foreach (DataRow dr in dt.Rows)
                {
                    tSRContactName = TransSRDetailContactName(tSRContact_List, dr["cSRID"].ToString());
                    tSRPathWay = TransSysParameterByList(tSRPathWay_List, dr["cSRPathWay"].ToString());
                    tSRType = TransSRType(dr["cSRTypeOne"].ToString(), dr["cSRTypeSec"].ToString(), dr["cSRTypeThr"].ToString());
                    tMainEngineerID = dr["cMainEngineerID"].ToString();
                    tMainEngineerName = dr["cMainEngineerName"].ToString();
                    tAssEngineerName = TransEmployeeName(tDicAssAndTech, dr["cAssEngineerID"].ToString());
                    tTechManagerName = TransEmployeeName(tDicAssAndTech, dr["cTechManagerID"].ToString());
                    tTechManagerID = dr["cTechManagerID"].ToString();
                    tSalesID = dr["cSalesID"].ToString();
                    tSecretaryID = dr["cSecretaryID"].ToString();
                    tModifiedDate = dr["ModifiedDate"].ToString() != "" ? Convert.ToDateTime(dr["ModifiedDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss") : "";

                    #region 組待處理服務
                    string[] ProcessInfo = new string[16];

                    ProcessInfo[0] = dr["cSRID"].ToString();             //SRID
                    ProcessInfo[1] = dr["cCustomerName"].ToString();      //客戶
                    ProcessInfo[2] = dr["cRepairName"].ToString();        //客戶報修人
                    ProcessInfo[3] = tSRContactName;                    //客戶聯絡人
                    ProcessInfo[4] = dr["cDesc"].ToString();             //說明
                    ProcessInfo[5] = tSRPathWay;                        //報修管道
                    ProcessInfo[6] = tSRType;                           //報修類別
                    ProcessInfo[7] = tMainEngineerID;                   //主要工程師ERPID
                    ProcessInfo[8] = tMainEngineerName;                 //主要工程師姓名
                    ProcessInfo[9] = tAssEngineerName;                  //協助工程師姓名
                    ProcessInfo[10] = tTechManagerID;                   //技術主管ERPID
                    ProcessInfo[11] = tTechManagerName;                 //技術主管姓名
                    ProcessInfo[12] = tSalesID;                         //業務人員ERPID
                    ProcessInfo[13] = tSecretaryID;                     //業務祕書ERPID
                    ProcessInfo[14] = tModifiedDate;                    //最後編輯日期
                    ProcessInfo[15] = dr["cStatus"].ToString();           //狀態                    

                    SRIDUserToList.Add(ProcessInfo);
                    #endregion
                }
            }
            else
            {
                beans = dbOne.TbOneSrmains.Where(x => (x.CStatus != "E0015" && x.CStatus != "E0006" && x.CStatus != "E0010") && 
                                                    (x.CMainEngineerId == tERPID || x.CSalesId == tERPID || x.CSecretaryId == tERPID || x.CTechManagerId.Contains(tERPID) || x.CAssEngineerId.Contains(tERPID))
                                              ).ToList();

                #region 先取得所有協助工程師和技術主管的ERPID
                foreach (var bean in beans)
                {
                    #region 協助工程師
                    findListAssAndTech(ref tListAssAndTech, bean.CAssEngineerId);
                    #endregion

                    #region 技術主管
                    findListAssAndTech(ref tListAssAndTech, bean.CTechManagerId);
                    #endregion
                }
                #endregion

                #region 再取得所有協助工程師和技術主管的中文姓名
                tDicAssAndTech = findListEmployeeInfo(tListAssAndTech);
                #endregion

                foreach (var bean in beans)
                {
                    tSRContactName = TransSRDetailContactName(tSRContact_List, bean.CSrid);
                    tSRPathWay = TransSysParameterByList(tSRPathWay_List, bean.CSrpathWay);                    
                    tSRType = TransSRType(bean.CSrtypeOne, bean.CSrtypeSec, bean.CSrtypeThr);
                    tMainEngineerID = string.IsNullOrEmpty(bean.CMainEngineerId) ? "" : bean.CMainEngineerId;
                    tMainEngineerName = string.IsNullOrEmpty(bean.CMainEngineerName) ? "" : bean.CMainEngineerName;                    
                    tAssEngineerName = TransEmployeeName(tDicAssAndTech, bean.CAssEngineerId);
                    tTechManagerName = TransEmployeeName(tDicAssAndTech, bean.CTechManagerId);
                    tTechManagerID = string.IsNullOrEmpty(bean.CTechManagerId) ? "" : bean.CTechManagerId;
                    tSalesID = string.IsNullOrEmpty(bean.CSalesId) ? "" : bean.CSalesId;
                    tSecretaryID = string.IsNullOrEmpty(bean.CSecretaryId) ? "" : bean.CSecretaryId;
                    tModifiedDate = bean.ModifiedDate == null ? "" : Convert.ToDateTime(bean.ModifiedDate).ToString("yyyy-MM-dd HH:mm:ss");

                    #region 組待處理服務
                    string[] ProcessInfo = new string[16];

                    ProcessInfo[0] = bean.CSrid;            //SRID
                    ProcessInfo[1] = bean.CCustomerName;     //客戶
                    ProcessInfo[2] = bean.CRepairName;       //客戶報修人
                    ProcessInfo[3] = tSRContactName;       //客戶聯絡人
                    ProcessInfo[4] = bean.CDesc;            //說明
                    ProcessInfo[5] = tSRPathWay;           //報修管道
                    ProcessInfo[6] = tSRType;              //報修類別
                    ProcessInfo[7] = tMainEngineerID;      //主要工程師ERPID
                    ProcessInfo[8] = tMainEngineerName;    //主要工程師姓名
                    ProcessInfo[9] = tAssEngineerName;     //協助工程師姓名
                    ProcessInfo[10] = tTechManagerID;      //技術主管ERPID
                    ProcessInfo[11] = tTechManagerName;    //技術主管姓名
                    ProcessInfo[12] = tSalesID;            //業務人員ERPID
                    ProcessInfo[13] = tSecretaryID;        //業務祕書ERPID
                    ProcessInfo[14] = tModifiedDate;       //最後編輯日期
                    ProcessInfo[15] = bean.CStatus;         //狀態                    

                    SRIDUserToList.Add(ProcessInfo);
                    #endregion
                }
            }            

            return SRIDUserToList;
        }
        #endregion

        #region 將服務團隊清單轉成where條件
        private string TrnasTeamListToWhere(List<string> tTeamList)
        {
            string reValue = string.Empty;

            int count = tTeamList.Count;
            int i = 0;

            foreach (var tTeam in tTeamList)
            {
                if (i == count - 1)
                {
                    reValue += "cTeamID like '%" + tTeam + "%'";
                }
                else
                {
                    reValue += "cTeamID like '%" + tTeam + "%' or ";
                }

                i++;
            }

            if (reValue != "")
            {
                reValue = " or (" + reValue + ")";
            }

            return reValue;
        }
        #endregion

        #region 取得登入人員所負責的服務團隊
        /// <summary>
        /// 取得登入人員所負責的服務團隊
        /// </summary>
        /// <param name="tCostCenterID">登入人員部門成本中心ID</param>
        /// <param name="tDeptID">登入人員部門ID</param>
        /// <returns></returns>
        public List<string> findSRTeamMappingList(string tCostCenterID, string tDeptID)
        {
            List<string> tList = new List<string>();

            var beans = dbOne.TbOneSrteamMappings.Where(x => x.Disabled == 0 && (x.CTeamNewId == tCostCenterID || x.CTeamNewId == tDeptID));

            foreach (var beansItem in beans)
            {
                if (!tList.Contains(beansItem.CTeamOldId))
                {
                    tList.Add(beansItem.CTeamOldId);
                }
            }

            return tList;
        }
        #endregion

        #region 取得報修管道參數值說明
        /// <summary>
        /// 取得報修管道參數值說明
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cCompanyID">公司別</param>
        /// <param name="cSRPathWay">報修管道ID</param>
        /// <returns></returns>
        public string TransSRPATH(string cOperationID, string cCompanyID, string cSRPathWay)
        {
            string tValue = findSysParameterDescription(cOperationID, "OTHER", cCompanyID, "SRPATH", cSRPathWay);

            return tValue;
        }
        #endregion

        #region 取得參數值說明By List
        /// <summary>
        /// 取得參數值說明By List
        /// </summary>
        /// <param name="tList">報修管道參數值清單</param>
        /// <param name="tValue">參數值</param>
        /// <returns></returns>
        public string TransSysParameterByList(List<TbOneSysParameter> tList, string tValue)
        {
            string reValue = string.Empty;

            foreach (var bean in tList)
            {
                if (bean.CValue == tValue.Trim())
                {
                    reValue = bean.CDescription;
                }
            }

            return reValue;
        }
        #endregion

        #region 取得服務案件狀態值說明
        /// <summary>
        /// 取得服務案件狀態值說明
        /// </summary>
        /// <param name="ListStatus">狀態清單</param>
        /// <param name="cSTATUS">狀態</param>
        /// <returns></returns>
        public string TransSRSTATUS(List<SelectListItem> ListStatus, string cSTATUS)
        {
            string tValue = string.Empty;

            var result = ListStatus.SingleOrDefault(x => x.Value == cSTATUS);

            if (result != null)
            {
                tValue = result.Text;
            }

            return tValue;
        }
        #endregion       

        #region 取得報修類別說明
        /// <summary>
        /// 取得報修類別說明
        /// </summary>
        /// <param name="cSRTypeOne">大類</param>
        /// <param name="cSRTypeSec">中類</param>
        /// <param name="cSRTypeThr">小類</param>
        /// <returns></returns>
        public string TransSRType(string cSRTypeOne, string cSRTypeSec, string cSRTypeThr)
        {
            string reValue = string.Empty;

            if (!string.IsNullOrEmpty(cSRTypeOne))
            {
                reValue += findSRRepairTypeName(cSRTypeOne) + "<br/>";
            }

            if (!string.IsNullOrEmpty(cSRTypeSec))
            {
                reValue += findSRRepairTypeName(cSRTypeSec) + "<br/>";
            }

            if (!string.IsNullOrEmpty(cSRTypeThr))
            {
                reValue += findSRRepairTypeName(cSRTypeThr);
            }

            return reValue;
        }
        #endregion

        #region 取得產品序號資訊說明
        /// <summary>
        /// 取得報修類別說明
        /// </summary>
        /// <param name="Products">產品序號＃＃產品機器型號＃＃Product Number</param>        
        /// <returns></returns>
        public string TransProductSerial(string Products)
        {
            string reValue = string.Empty;
            string cSerialID = string.Empty;        //產品序號
            string cMaterialName = string.Empty;    //產品機器型號
            string cProductNumber = string.Empty;   //Product Number

            string[] tAry = Products.Split("＃＃");
            cSerialID = tAry.Length >= 1 ? tAry[0] : "";
            cMaterialName = tAry.Length >= 2 ? tAry[1] : "";
            cProductNumber = tAry.Length >= 3 ? tAry[2] : "";

            if (!string.IsNullOrEmpty(cSerialID))
            {
                reValue += cSerialID + "<br/>";
            }

            if (!string.IsNullOrEmpty(cMaterialName))
            {
                reValue += cMaterialName + "<br/>";
            }

            if (!string.IsNullOrEmpty(cProductNumber))
            {
                reValue += cProductNumber;
            }

            return reValue;
        }
        #endregion

        #region 取得所有協助工程師和技術主管的ERPID清單
        /// <summary>
        /// 取得所有協助工程師和技術主管的ERPID清單
        /// </summary>
        /// <param name="tList">ERPID清單</param>
        /// <param name="tOriERPID">傳入的ERPID</param>
        public void findListAssAndTech(ref List<string> tList, string tOriERPID)
        {
            tOriERPID = string.IsNullOrEmpty(tOriERPID) ? "" : tOriERPID.Trim();

            if (tOriERPID != "")
            {
                string[] tAryAssERPID = tOriERPID.ToString().Split(';');

                foreach (string tERPID in tAryAssERPID)
                {
                    if (tERPID != "")
                    {
                        if (!tList.Contains(tERPID))
                        {
                            tList.Add(tERPID);
                        }
                    }
                }
            }
        }
        #endregion

        #region 取得所有傳入員工ERPID清單，並回傳ERPID和中、英文姓名清單
        /// <summary>
        /// 取得所有傳入員工ERPID清單，並回傳ERPID和中、英文姓名清單
        /// </summary>
        /// <param name="tERPID_List">員工ERPID清單</param>
        /// <returns></returns>
        public Dictionary<string, string> findListEmployeeInfo(List<string> tERPID_List)
        {
            Dictionary<string, string> reDic = new Dictionary<string, string>();

            var beans = dbEIP.People.Where(x => tERPID_List.Contains(x.ErpId));

            foreach (var bean in beans)
            {
                reDic.Add(bean.ErpId, bean.Name2 + " " + bean.Name);
            }

            return reDic;
        }
        #endregion

        #region 取得人員中、英文姓名
        /// <summary>
        /// 取得人員中、英文姓名
        /// </summary>
        /// <param name="Dic">ERPID,中、英文姓名清單</param>
        /// <param name="tOriERPID">ERPID(多人，用分號隔開)</param>
        /// <returns></returns>
        public string TransEmployeeName(Dictionary<string, string> Dic, string tOriERPID)
        {
            string reValue = string.Empty;

            tOriERPID = string.IsNullOrEmpty(tOriERPID) ? "" : tOriERPID.Trim();

            if (tOriERPID != "")
            {
                string[] tAryERPID = tOriERPID.Split(';');

                foreach(string tERPID in tAryERPID)
                {
                    var tName = Dic.FirstOrDefault(x => x.Key == tERPID).Value;

                    reValue += tName + "<br/>";
                }
            }

            return reValue;
        }
        #endregion        

        #endregion -----↑↑↑↑↑待辦清單 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓一般服務 ↓↓↓↓↓-----        

        #region 取得所有第一階List清單(報修類別)
        /// <summary>
        /// 取得所有第一階List清單(報修類別)
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> findFirstKINDList()
        {
            List<string> tTempList = new List<string>();

            string tKIND_KEY = string.Empty;
            string tKIND_NAME = string.Empty;

            var beansKIND = dbOne.TbOneSrrepairTypes.OrderBy(x => x.CKindKey).Where(x => x.Disabled == 0 && x.CUpKindKey == "0");

            var tList = new List<SelectListItem>();
            tList.Add(new SelectListItem { Text = " ", Value = "" });

            foreach (var bean in beansKIND)
            {
                if (!tTempList.Contains(bean.CKindKey))
                {
                    tKIND_NAME = bean.CKindKey + "_" + bean.CKindName;

                    tList.Add(new SelectListItem { Text = tKIND_NAME, Value = bean.CKindKey});
                    tTempList.Add(bean.CKindKey);
                }
            }

            return tList;
        }
        #endregion

        #region 傳入第一階(大類)並取得第二階(中類)清單
        /// <summary>
        /// 傳入第一階(大類)並取得第二階(中類)清單
        /// </summary>
        /// <param name="keyword">第一階(大類)代碼</param>
        /// <returns></returns>
        public List<SelectListItem> findSRTypeSecList(string keyword)
        {
            List<string> tTempList = new List<string>();

            string tKIND_KEY = string.Empty;
            string tKIND_NAME = string.Empty;

            var beansKIND = dbOne.TbOneSrrepairTypes.OrderBy(x => x.CKindKey).Where(x => x.Disabled == 0 && x.CKindLevel == 2 && x.CUpKindKey == keyword);

            var tList = new List<SelectListItem>();
            tList.Add(new SelectListItem { Text = " ", Value = "" });

            foreach (var bean in beansKIND)
            {
                if (!tTempList.Contains(bean.CKindKey))
                {                    
                    tKIND_NAME = bean.CKindKey + "_" + bean.CKindName;

                    tList.Add(new SelectListItem { Text = tKIND_NAME, Value = bean.CKindKey });
                    tTempList.Add(bean.CKindKey);
                }
            }

            return tList;
        }
        #endregion

        #region 傳入第二階(中類)並取得第三階(小類)清單
        /// <summary>
        /// 傳入第二階(中類)並取得第三階(小類)清單
        /// </summary>
        /// <param name="keyword">第二階(中類)代碼</param>
        /// <returns></returns>
        public List<SelectListItem> findSRTypeThrList(string keyword)
        {
            List<string> tTempList = new List<string>();

            string tKIND_KEY = string.Empty;
            string tKIND_NAME = string.Empty;

            var beansKIND = dbOne.TbOneSrrepairTypes.OrderBy(x => x.CKindKey).Where(x => x.Disabled == 0 && x.CKindLevel == 3 && x.CUpKindKey == keyword);

            var tList = new List<SelectListItem>();
            tList.Add(new SelectListItem { Text = " ", Value = "" });

            foreach (var bean in beansKIND)
            {
                if (!tTempList.Contains(bean.CKindKey))
                {
                    tKIND_NAME = bean.CKindKey + "_" + bean.CKindName;

                    tList.Add(new SelectListItem { Text = tKIND_NAME, Value = bean.CKindKey });
                    tTempList.Add(bean.CKindKey);
                }
            }

            return tList;
        }
        #endregion       

        #region 取得服務團隊清單
        /// <summary>
        /// 取得服務團隊清單
        /// </summary>
        /// <param name="pCompanyCode">公司別(T012、T016、C069、T022)</param>
        /// <param name="cEmptyOption">是否要產生「請選擇」選項(True.要 false.不要)</param>        
        /// <returns></returns>
        public List<SelectListItem> findSRTeamIDList(string pCompanyCode, bool cEmptyOption)
        {
            List<string> tTempList = new List<string>();

            string tKEY = string.Empty;
            string tNAME = string.Empty;
            string tSRVID = "SRV." + pCompanyCode.Substring(2, 2);

            List<TbOneSrteamMapping> TeamList = new List<TbOneSrteamMapping>();          

            TeamList = dbOne.TbOneSrteamMappings.OrderBy(x => x.CTeamOldId).Where(x => x.Disabled == 0 && (x.CTeamOldId.Contains(tSRVID))).ToList();

            var tList = new List<SelectListItem>();

            if (cEmptyOption)
            {
                tList.Add(new SelectListItem { Text = "請選擇", Value = "" });
            }

            foreach (var bean in TeamList)
            {
                if (!tTempList.Contains(bean.CTeamOldId))
                {
                    tNAME = bean.CTeamOldId + "_" + bean.CTeamOldName;

                    tList.Add(new SelectListItem { Text = tNAME, Value = bean.CTeamOldId });
                    tTempList.Add(bean.CTeamOldId);
                }
            }

            return tList;
        }
        #endregion

        #region 取得服務團隊清單(舊組織)
        /// <summary>
        /// 取得服務團隊清單(舊組織)
        /// </summary>
        /// <param name="pCompanyCode">公司別(T012、T016、C069、T022)</param>
        /// <param name="cEmptyOption">是否要產生「請選擇」選項(True.要 false.不要)</param>        
        /// <returns></returns>
        public List<SelectListItem> findSRTeamOldIDList(string cOperationID_GenerallySR, string pCompanyCode, bool cEmptyOption)
        {
            var tList = findSysParameterListItem(cOperationID_GenerallySR, "OTHER", pCompanyCode, "OLDTEAM", cEmptyOption);           

            return tList;
        }
        #endregion

        #region 取得服務團隊說明By List
        /// <summary>
        /// 取得服務團隊說明By List
        /// </summary>
        /// <param name="tList">服務團隊清單</param>
        /// <param name="tValue">參數值</param>
        /// <returns></returns>
        public string TransSRTeam(List<SelectListItem> tList, string tValue)
        {
            string reValue = string.Empty;
            string[] tAryValue = tValue.Split(';');

            foreach(string tStr in tAryValue)
            {
                foreach (var bean in tList)
                {
                    if (bean.Value == tStr)
                    {
                        reValue += bean.Text + "<br/>";
                        break;
                    }
                }
            }            

            return reValue;
        }
        #endregion

        #region 取得呼叫SAPERP參數是正式區或測試區(true.正式區 false.測試區)
        /// <summary>
        /// 取得呼叫SAPERP參數是正式區或測試區(true.正式區 false.測試區)
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <returns></returns>
        public bool getCallSAPERPPara(string cOperationID)
        {
            bool reValue = false;

            string tValue =  findSysParameterValue(cOperationID, "OTHER", "ALL", "SAPERP");

            reValue = Convert.ToBoolean(tValue);

            return reValue;
        }
        #endregion

        #region 取得系統位址參數相關資訊
        /// <summary>
        /// 取得系統位址參數相關資訊
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>        
        /// <returns></returns>
        public SRSYSPARAINFO findSRSYSPARAINFO(string cOperationID)
        {
            SRSYSPARAINFO OUTBean = new SRSYSPARAINFO();

            bool tIsFormal = getCallSAPERPPara(cOperationID); //取得呼叫SAPERP參數是正式區或測試區(true.正式區 false.測試區)          

            OUTBean.IsFormal = tIsFormal;

            if (tIsFormal)
            {
                #region 正式區                
                OUTBean.BPMURLName = "tsti-bpm01.etatung.com.tw";
                OUTBean.PSIPURLName = "psip-prd-ap";
                OUTBean.AttachURLName = "tsticrmmbgw.etatung.com:8082";
                OUTBean.APIURLName = @"https://api-qas.etatung.com";
                #endregion
            }
            else
            {
                #region 測試區                
                OUTBean.BPMURLName = "tsti-bpm01.etatung.com.tw";
                OUTBean.PSIPURLName = "psip-prd-ap";
                OUTBean.AttachURLName = "tsticrmmbgw.etatung.com:8082";
                OUTBean.APIURLName = @"https://api-qas.etatung.com";
                #endregion
            }

            return OUTBean;
        }
        #endregion

        #region 取得附件/服務報告書URL(多筆以;號隔開)
        /// <summary>
        /// 取得附件/服務報告書URL(多筆以;號隔開)
        /// </summary>
        /// <param name="tAttach">附件GUID(多筆以,號隔開)</param>
        /// <param name="tAttachURLName">附件URL站台名稱</param>
        /// <returns></returns>
        public string findAttachUrl(string tAttach, string tAttachURLName)
        {
            string reValue = string.Empty;

            List<SRATTACHINFO> SR_List = findSRATTACHINFO(tAttach, tAttachURLName);

            foreach (var bean in SR_List)
            {
                reValue += bean.FILE_URL + ";";
            }

            reValue = reValue.TrimEnd(';').Replace(";", "<br/>");            

            return reValue;
        }
        #endregion

        #region 取得附件相關資訊
        /// <summary>
        /// 取得附件相關資訊
        /// </summary>
        /// <param name="tAttach">附件GUID(多筆以,號隔開)</param>
        /// <param name="tAttachURLName">附件URL站台名稱</param>
        /// <returns></returns>
        public List<SRATTACHINFO> findSRATTACHINFO(string tAttach, string tAttachURLName)
        {
            #region 範例Url
            //http://tsticrmmbgw.etatung.com:8082/CSreport/a7f12260-0168-4cf8-a321-c2d410ac3536.txt
            #endregion

            tAttach = string.IsNullOrEmpty(tAttach) ? "" : tAttach;

            List<SRATTACHINFO> tList = new List<SRATTACHINFO>();

            string tURL = string.Empty;
            string[] tAryAttach = tAttach.TrimEnd(',').Split(',');

            foreach (string tKey in tAryAttach)
            {
                var bean = dbOne.TbOneDocuments.FirstOrDefault(x => x.Id.ToString() == tKey);

                if (bean != null)
                {
                    SRATTACHINFO beanSR = new SRATTACHINFO();

                    tURL = "http://" + tAttachURLName + "/CSreport/" + bean.FileName;

                    beanSR.ID = tKey;
                    beanSR.FILE_ORG_NAME = bean.FileOrgName;
                    beanSR.FILE_NAME = bean.FileName;
                    beanSR.FILE_EXT = bean.FileExt;
                    beanSR.FILE_URL = tURL;
                    beanSR.INSERT_TIME = bean.InsertTime;

                    tList.Add(beanSR);
                }
            }

            return tList;
        }
        #endregion

        #region 呼叫BPM保固申請單並取得計費業務、發票號碼、發票日期
        /// <summary>
        /// 呼叫BPM保固申請單並取得計費業務、發票號碼、發票日期
        /// </summary>
        /// <param name="BPMNO">保固表單編號</param>
        /// <returns></returns>
        public string[] findBPMWarrantyInfo(string BPMNO)
        {
            string[] reValue = new string[3];

            var bean = bpmDB.TblFormGuaranteePops.FirstOrDefault(x => x.CFormNo == BPMNO);

            if (bean != null)
            {
                reValue[0] = bean.CApplyName;   //計費業務
                reValue[1] = bean.CReceiptNo;   //發票號碼
                reValue[2] = bean.CReceiptDate; //發票日期
            }

            return reValue;
        }
        #endregion       

        #region 呼叫RFC並回傳SLA Table清單
        /// <summary>
        /// 呼叫RFC並回傳SLA Table清單
        /// </summary>        
        /// <param name="ArySERIAL">序號Array</param>
        /// <param name="NowCount">目前的項次</param>
        /// <param name="tBPMURLName">BPM站台名稱</param>
        /// <param name="tPSIPURLName">PSIP站台名稱</param>
        /// <param name="tAPIURLName">API站台名稱</param>
        /// <returns></returns>
        public List<SRWarranty> ZFM_TICC_SERIAL_SEARCHWTYList(string[] ArySERIAL, ref int NowCount, string tBPMURLName, string tPSIPURLName, string tAPIURLName)
        {
            List<SRWarranty> QueryToList = new List<SRWarranty>();

            string cWTYID = string.Empty;
            string cWTYName = string.Empty;            
            string cWTYSDATE = string.Empty;
            string cWTYEDATE = string.Empty;
            string cSLARESP = string.Empty;
            string cSLASRV = string.Empty;
            string cContractID = string.Empty;
            string cContractIDURL = string.Empty;
            string cSUB_CONTRACTID = string.Empty;
            string tBPMNO = string.Empty;            
            string tAdvice = string.Empty;
            string tURL = string.Empty;
            string tBGColor = "table-success";

            int tLength = 0;
            int pCount = 0;

            try
            {
                var client = new RestClient("http://tsti-sapapp01.etatung.com.tw/api/ticc");                

                foreach (string IV_SERIAL in ArySERIAL)
                {
                    if (pCount % 2 == 0 )
                    {
                        tBGColor = "";
                    }
                    else
                    {
                        tBGColor = "table-success";
                    }

                    if (IV_SERIAL != null)
                    {
                        if (IV_SERIAL.ToUpper() != "NA")
                        {
                            var request = new RestRequest();
                            request.Method = RestSharp.Method.Post;

                            Dictionary<Object, Object> parameters = new Dictionary<Object, Object>();
                            parameters.Add("SAP_FUNCTION_NAME", "ZFM_TICC_SERIAL_SEARCH");
                            parameters.Add("IV_SERIAL", IV_SERIAL);

                            request.AddHeader("Content-Type", "application/json");
                            request.AddHeader("X-MBX-APIKEY", "6xdTlREsMbFd0dBT28jhb5W3BNukgLOos");
                            request.AddParameter("application/json", parameters, ParameterType.RequestBody);

                            RestResponse response = client.Execute(request);

                            var data = (JObject)JsonConvert.DeserializeObject(response.Content);

                            tLength = int.Parse(data["ET_WARRANTY"]["Length"].ToString());                          //保固共有幾筆

                            for (int i = 0; i < tLength; i++)
                            {
                                NowCount++;

                                cContractIDURL = "";
                                tBPMNO = "";
                                tURL = "";

                                cWTYID = data["ET_WARRANTY"]["SyncRoot"][i]["wTYCODEField"].ToString().Trim();       //保固
                                cWTYName = data["ET_WARRANTY"]["SyncRoot"][i]["wTYCODEField"].ToString().Trim();     //保固說明
                                cWTYSDATE = data["ET_WARRANTY"]["SyncRoot"][i]["wTYSTARTField"].ToString().Trim();   //保固開始日期
                                cWTYEDATE = data["ET_WARRANTY"]["SyncRoot"][i]["wTYENDField"].ToString().Trim();     //保固結束日期                                                          
                                cSLARESP = data["ET_WARRANTY"]["SyncRoot"][i]["sLASRVField"].ToString().Trim();      //回應條件
                                cSLASRV = data["ET_WARRANTY"]["SyncRoot"][i]["sLARESPField"].ToString().Trim();      //服務條件
                                cContractID = data["ET_WARRANTY"]["SyncRoot"][i]["cONTRACTField"].ToString().Trim(); //合約編號
                                tBPMNO = data["ET_WARRANTY"]["SyncRoot"][i]["bPM_NOField"].ToString().Trim();        //BPM表單編號
                                tAdvice = data["ET_WARRANTY"]["SyncRoot"][i]["aDVICEField"].ToString().Trim();       //客服主管建議

                                #region 取得BPM Url
                                if (cContractID != "")
                                {
                                    tBPMNO = findContractSealsFormNo(cContractID);

                                    cSUB_CONTRACTID = findContractSealsOBJSubNo(tAPIURLName, cContractID);

                                    try
                                    {
                                        Int32 ContractID = Int32.Parse(cContractID);

                                        if (ContractID >= 10506151 && ContractID != 10506152 && ContractID != 10506157) //新的用印申請單
                                        {
                                            tURL = "http://" + tBPMURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Rwd/ContractSeals/ContractSealsForm.aspx?FormNo=" + tBPMNO + " target=_blank";
                                        }
                                        else //舊的用印申請單
                                        {
                                            tURL = "http://" + tBPMURLName + "/ContractSeals/_layouts/FormServer.aspx?XmlLocation=%2fContractSeals%2fBPMContractSealsForm%2f" + tBPMNO + ".xml&ClientInstalled=true&DefaultItemOpen=1&source=/_layouts/TSTI.SharePoint.BPM/CloseWindow.aspx target=_blank";
                                        }

                                        cContractIDURL = "http://" + tPSIPURLName + "/Spare/QueryContractInfo?CONTRACTID=" + cContractID + " target=_blank"; //合約編號URL
                                    }
                                    catch (Exception ex)
                                    {
                                        cContractIDURL = "";
                                        tBPMNO = "";
                                        tURL = "";
                                    }
                                }
                                else
                                {
                                    if (tBPMNO.IndexOf("WTY") != -1)
                                    {
                                        tURL = "http://" + tBPMURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Rwd/Warranty/WarrantyForm.aspx?FormNo=" + tBPMNO + " target=_blank";
                                    }
                                    else
                                    {
                                        tURL = "http://" + tBPMURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Form/Guarantee/GuaranteeForm.aspx?FormNo=" + tBPMNO + " target=_blank";
                                    }
                                }
                                #endregion

                                #region 取得清單
                                SRWarranty QueryInfo = new SRWarranty();

                                QueryInfo.cID = NowCount.ToString();            //系統ID
                                QueryInfo.cSerialID = IV_SERIAL;                //序號                        
                                QueryInfo.cWTYID = cWTYID;                      //保固
                                QueryInfo.cWTYName = cWTYName;                  //保固說明
                                QueryInfo.cWTYSDATE = cWTYSDATE;                //保固開始日期
                                QueryInfo.cWTYEDATE = cWTYEDATE;                //保固結束日期                                                          
                                QueryInfo.cSLARESP = cSLARESP;                  //回應條件
                                QueryInfo.cSLASRV = cSLASRV;                   //服務條件
                                QueryInfo.cContractID = cContractID;           //合約編號
                                QueryInfo.cContractIDUrl = cContractIDURL;     //合約編號Url
                                QueryInfo.cSUB_CONTRACTID = cSUB_CONTRACTID;   //下包文件編號
                                QueryInfo.cBPMFormNo = tBPMNO;                 //BPM表單編號                        
                                QueryInfo.cBPMFormNoUrl = tURL;                //BPM URL
                                QueryInfo.cAdvice = tAdvice;                  //客服主管建議                            
                                QueryInfo.cUsed = "N";
                                QueryInfo.cBGColor = tBGColor;                //tr背景顏色Class

                                QueryToList.Add(QueryInfo);
                                #endregion
                            }
                        }
                    }

                    pCount++;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return QueryToList;
        }
        #endregion

        #region 取得SLA Table清單
        /// <summary>
        /// 取得SLA Table清單
        /// </summary>        
        /// <param name="cSRID">SRID</param>
        /// <param name="NowCount">目前的項次</param>
        /// <param name="tURLName">BPM站台名稱</param>
        /// <param name="tSeverName">PSIP站台名稱</param>
        /// <returns></returns>
        public List<SRWarranty> SEARCHWTYList(string cSRID, ref int NowCount, string tURLName, string tSeverName)
        {
            List<SRWarranty> QueryToList = new List<SRWarranty>();

            string cSERIAL = string.Empty;
            string cWTYID = string.Empty;
            string cWTYName = string.Empty;
            string cWTYSDATE = string.Empty;
            string cWTYEDATE = string.Empty;
            string cSLARESP = string.Empty;
            string cSLASRV = string.Empty;
            string cContractID = string.Empty;
            string cContractIDURL = string.Empty;
            string cSUB_CONTRACTID = string.Empty;
            string tBPMNO = string.Empty;
            string tURL = string.Empty;
            string tAdvice = string.Empty;
            string cUsed = string.Empty;
            string tBGColor = "table-success";
            string TempSERIAL = string.Empty;

            int pCount = 0;

            try
            {
                var beans = dbOne.TbOneSrdetailWarranties.OrderBy(x => x.CSerialId).OrderByDescending(x => x.CWtyedate).Where(x => x.CSrid == cSRID);

                foreach (var bean in beans)
                {
                    #region 判斷背景顏色class
                    if (TempSERIAL != "" && TempSERIAL != bean.CSerialId)
                    {
                        pCount++;
                    }

                    TempSERIAL = bean.CSerialId;

                    if (pCount % 2 == 0)
                    {
                        tBGColor = "";
                    }
                    else
                    {
                        tBGColor = "table-success";
                    }
                    #endregion

                    NowCount++;

                    cContractIDURL = "";
                    tBPMNO = "";
                    tURL = "";

                    cSERIAL = bean.CSerialId;                                           //序號
                    cWTYID = bean.CWtyid;                                               //保固
                    cWTYName = bean.CWtyname;                                           //保固說明
                    cWTYSDATE = Convert.ToDateTime(bean.CWtysdate).ToString("yyyy-MM-dd");  //保固開始日期
                    cWTYEDATE = Convert.ToDateTime(bean.CWtyedate).ToString("yyyy-MM-dd");  //保固結束日期                                                          
                    cSLARESP = bean.CSlaresp;                                           //回應條件
                    cSLASRV = bean.CSlasrv;                                             //服務條件
                    cContractID = bean.CContractId;                                     //合約編號
                    cSUB_CONTRACTID = bean.CSubContractId;                              //下包文件編號
                    tBPMNO = bean.CBpmformNo;                                           //BPM表單編號
                    tAdvice = bean.CAdvice;                                             //客服主管建議
                    cUsed = bean.CUsed;                                                 //本次使用

                    #region 取得BPM Url
                    if (cContractID != "")
                    {
                        tBPMNO = findContractSealsFormNo(cContractID);

                        try
                        {
                            Int32 ContractID = Int32.Parse(cContractID);

                            if (ContractID >= 10506151 && ContractID != 10506152 && ContractID != 10506157) //新的用印申請單
                            {
                                tURL = "http://" + tURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Rwd/ContractSeals/ContractSealsForm.aspx?FormNo=" + tBPMNO + " target=_blank";
                            }
                            else //舊的用印申請單
                            {
                                tURL = "http://" + tURLName + "/ContractSeals/_layouts/FormServer.aspx?XmlLocation=%2fContractSeals%2fBPMContractSealsForm%2f" + tBPMNO + ".xml&ClientInstalled=true&DefaultItemOpen=1&source=/_layouts/TSTI.SharePoint.BPM/CloseWindow.aspx target=_blank";
                            }

                            cContractIDURL = "http://" + tSeverName + "/Spare/QueryContractInfo?CONTRACTID=" + cContractID + " target=_blank"; //合約編號URL
                        }
                        catch (Exception ex)
                        {
                            cContractIDURL = "";
                            tBPMNO = "";
                            tURL = "";
                        }
                    }
                    else
                    {
                        if (tBPMNO.IndexOf("WTY") != -1)
                        {
                            tURL = "http://" + tURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Rwd/Warranty/WarrantyForm.aspx?FormNo=" + tBPMNO + " target=_blank";
                        }
                        else
                        {
                            tURL = "http://" + tURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Form/Guarantee/GuaranteeForm.aspx?FormNo=" + tBPMNO + " target=_blank";
                        }
                    }
                    #endregion

                    #region 取得清單
                    SRWarranty QueryInfo = new SRWarranty();

                    QueryInfo.cID = NowCount.ToString();            //系統ID
                    QueryInfo.cSerialID = cSERIAL;                  //序號                        
                    QueryInfo.cWTYID = cWTYID;                      //保固
                    QueryInfo.cWTYName = cWTYName;                  //保固說明
                    QueryInfo.cWTYSDATE = cWTYSDATE;                //保固開始日期
                    QueryInfo.cWTYEDATE = cWTYEDATE;                //保固結束日期                                                          
                    QueryInfo.cSLARESP = cSLARESP;                  //回應條件
                    QueryInfo.cSLASRV = cSLASRV;                    //服務條件
                    QueryInfo.cContractID = cContractID;            //合約編號
                    QueryInfo.cContractIDUrl = cContractIDURL;      //合約編號Url
                    QueryInfo.cSUB_CONTRACTID = cSUB_CONTRACTID;    //下包文件編號
                    QueryInfo.cBPMFormNo = tBPMNO;                  //BPM表單編號                        
                    QueryInfo.cBPMFormNoUrl = tURL;                 //BPM URL
                    QueryInfo.cAdvice = tAdvice;                    //客服主管建議                                           
                    QueryInfo.cUsed = cUsed;                        //本次使用
                    QueryInfo.cBGColor = tBGColor;                  //tr背景顏色Class

                    QueryToList.Add(QueryInfo);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return QueryToList;
        }
        #endregion

        #region 傳入合約編號並取得BPM用印申請單表單編號
        /// <summary>
        /// 傳入合約編號並取得BPM用印申請單表單編號
        /// </summary>
        /// <param name="NO">合約編號</param>
        /// <returns></returns>
        public string findContractSealsFormNo(string NO)
        {
            string reValue = string.Empty;

            var bean = dbProxy.F4501s.FirstOrDefault(x => x.No == NO);

            if (bean != null)
            {
                if (bean.Bpmno != null)
                {
                    reValue = bean.Bpmno.Trim();
                }
            }

            return reValue;
        }
        #endregion

        #region 取得保固說明
        /// <summary>
        /// 取得保固說明
        /// </summary>
        /// <returns></returns>
        public string findWTYName(string cWTYID)
        {
            string reValue = string.Empty;

            var bean = dbProxy.F0005s.FirstOrDefault(x => x.Modt == "SD" && x.Alias == "WARTY" && x.Dsc2 == cWTYID);

            if (bean != null)
            {
                reValue = bean.Dsc1.Trim();
            }

            return reValue;
        }
        #endregion

        #region 傳入合約編號並取得CRM合約標的的下包文件編號
        /// <summary>
        /// 傳入合約編號並取得CRM合約標的的下包文件編號
        /// </summary>
        /// <param name="tAPIURLName">API站台名稱</param>
        /// <param name="NO">合約編號</param>
        /// <returns></returns>
        public string findContractSealsOBJSubNo(string tAPIURLName, string NO)
        {
            string reValue = string.Empty;
            string SUB_CONTRACTID = string.Empty;
            string tURL = tAPIURLName + "/API/API_CONTRACTOBJINFO_GET";

            var client = new RestClient(tURL);

            if (NO != null)
            {
                CONTRACTOBJINFO_OUTPUT OUTBean = new CONTRACTOBJINFO_OUTPUT();

                var request = new RestRequest();
                request.Method = RestSharp.Method.Post;

                Dictionary<Object, Object> parameters = new Dictionary<Object, Object>();
                parameters.Add("IV_CONTRACTID", NO);

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-MBX-APIKEY", "6xdTlREsMbFd0dBT28jhb5W3BNukgLOos");
                request.AddParameter("application/json", parameters, ParameterType.RequestBody);

                RestResponse response = client.Execute(request);

                #region 取得回傳訊息(成功或失敗)
                if (response.Content != null)
                {
                    var data = (JObject)JsonConvert.DeserializeObject(response.Content);

                    OUTBean.EV_MSGT = data["EV_MSGT"].ToString().Trim();
                    OUTBean.EV_MSG = data["EV_MSG"].ToString().Trim();
                    #endregion

                    if (OUTBean.EV_MSGT == "Y")
                    {
                        #region 取得合約標的資料List
                        var tList = (JArray)JsonConvert.DeserializeObject(data["CONTRACTOBJINFO_LIST"].ToString().Trim());

                        if (tList != null)
                        {
                            foreach (JObject bean in tList)
                            {
                                SUB_CONTRACTID = bean["SUB_CONTRACTID"].ToString().Trim();

                                if (SUB_CONTRACTID != "")
                                {
                                    break;
                                }
                            }

                            reValue = SUB_CONTRACTID;
                        }
                        #endregion
                    }
                }                
            }

            return reValue;
        }
        #endregion

        #region 查詢合約標的資料OUTPUT資訊
        /// <summary>查詢合約標的資料OUTPUT資訊</summary>
        public struct CONTRACTOBJINFO_OUTPUT
        {
            /// <summary>消息類型(E.處理失敗 Y.處理成功)</summary>
            public string EV_MSGT { get; set; }
            /// <summary>消息內容</summary>
            public string EV_MSG { get; set; }

            /// <summary>合約標的資料清單</summary>
            public List<CONTRACTOBJINFO_LIST> CONTRACTOBJINFO_LIST { get; set; }
        }

        public struct CONTRACTOBJINFO_LIST
        {
            /// <summary>主約文件編號</summary>
            public string CONTRACTID;
            /// <summary>HostName</summary>
            public string HOSTNAME;
            /// <summary>序號</summary>
            public string SN;
            /// <summary>廠牌</summary>
            public string BRANDS;
            /// <summary>ProductModel</summary>
            public string MODEL;
            /// <summary>Location</summary>
            public string LOCATION;
            /// <summary>地點</summary>
            public string PLACE;
            /// <summary>區域</summary>
            public string AREA;
            /// <summary>回應條件</summary>
            public string RESPONSE_LEVEL;
            /// <summary>服務條件</summary>
            public string SERVICE_LEVEL;
            /// <summary>備註</summary>
            public string NOTES;
            /// <summary>下包文件編號</summary>
            public string SUB_CONTRACTID;
        }
        #endregion    

        #region 取得一般服務(報修類別說明)
        /// <summary>
        /// 取得一般服務(報修類別說明)
        /// </summary>
        /// <param name="cKindKey">報修類別ID</param>
        /// <returns></returns>
        public string findSRRepairTypeName(string cKindKey)
        {
            string reValue = string.Empty;

            var bean = dbOne.TbOneSrrepairTypes.FirstOrDefault(x => x.CKindKey == cKindKey);

            if (bean != null)
            {
                reValue = bean.CKindName;
            }

            return reValue;
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
        public string findSRRepairTypeKindKey(string SRTypeZero, string SRTypeOne, string SRTypeSec)
        {
            string reValue = string.Empty;
            string tTempKey = string.Empty;

            int tCount = 0;

            SRTypeOne = string.IsNullOrEmpty(SRTypeOne) ? "" : SRTypeOne.Trim();
            SRTypeSec = string.IsNullOrEmpty(SRTypeSec) ? "" : SRTypeSec.Trim();

            TbOneSrrepairType bean = new TbOneSrrepairType();

            if (SRTypeSec != "")
            {
                bean = dbOne.TbOneSrrepairTypes.OrderByDescending(x => x.CKindKey).FirstOrDefault(x => x.Disabled == 0 && x.CKindLevel == 3 && x.CUpKindKey == SRTypeSec);

                if (bean != null)
                {
                    //ex.ZC010101
                    tTempKey = bean.CKindKey.Replace("ZC", "");
                    tCount = int.Parse(tTempKey) + 1;

                    reValue = "ZC" + tCount.ToString().PadLeft(6, '0');
                }
                else
                {
                    tTempKey = SRTypeSec.Replace("ZB", "");
                    tCount = 1;

                    reValue = "ZC" + tTempKey + tCount.ToString().PadLeft(2, '0');
                }
            }
            else if (SRTypeOne != "")
            {
                bean = dbOne.TbOneSrrepairTypes.OrderByDescending(x => x.CKindKey).FirstOrDefault(x => x.Disabled == 0 && x.CKindLevel == 2 && x.CUpKindKey == SRTypeOne);

                if (bean != null)
                {
                    //ex.ZB0101
                    tTempKey = bean.CKindKey.Replace("ZB", "");
                    tCount = int.Parse(tTempKey) + 1;

                    reValue = "ZB" + tCount.ToString().PadLeft(4, '0');
                }
            }
            else
            {
                bean = dbOne.TbOneSrrepairTypes.OrderByDescending(x => x.CKindKey).FirstOrDefault(x => x.Disabled == 0 && x.CKindLevel == 1 && x.CUpKindKey == SRTypeZero);

                if (bean != null)
                {
                    //ex.ZA01
                    tTempKey = bean.CKindKey.Replace("ZA", "");
                    tCount = int.Parse(tTempKey) + 1;

                    reValue = "ZA" + tCount.ToString().PadLeft(2, '0');
                }
            }

            return reValue;
        }
        #endregion

        #region call ONE SERVICE 服務案件(一般/裝機/定維)狀態更新接口
        /// <summary>
        /// call ONE SERVICE 服務案件(一般/裝機/定維)狀態更新接口
        /// </summary>
        /// <param name="beanIN"></param>
        public SRMain_SRSTATUS_OUTPUT GetAPI_SRSTATUS_Update(SRMain_SRSTATUS_INPUT beanIN)
        {
            SRMain_SRSTATUS_OUTPUT OUTBean = new SRMain_SRSTATUS_OUTPUT();

            string pMsg = string.Empty;

            try
            {
                string tURL = beanIN.IV_APIURLName + "/API/API_SRSTATUS_UPDATE";

                var client = new RestClient(tURL);

                var request = new RestRequest();
                request.Method = RestSharp.Method.Post;

                Dictionary<Object, Object> parameters = new Dictionary<Object, Object>();
                parameters.Add("IV_LOGINEMPNO", beanIN.IV_LOGINEMPNO);
                parameters.Add("IV_SRID", beanIN.IV_SRID);
                parameters.Add("IV_STATUS", beanIN.IV_STATUS);                

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-MBX-APIKEY", "6xdTlREsMbFd0dBT28jhb5W3BNukgLOos");
                request.AddParameter("application/json", parameters, ParameterType.RequestBody);

                RestResponse response = client.Execute(request);

                #region 取得回傳訊息(成功或失敗)
                var data = (JObject)JsonConvert.DeserializeObject(response.Content);

                OUTBean.EV_MSGT = data["EV_MSGT"].ToString().Trim();
                OUTBean.EV_MSG = data["EV_MSG"].ToString().Trim();
                #endregion

                if (OUTBean.EV_MSGT == "Y")
                {
                    pMsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE 服務案件(一般/裝機/定維)狀態更新接口成功";                    
                }
                else
                {
                    pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE 服務案件(一般/裝機/定維)狀態更新接口失敗，原因:" + OUTBean.EV_MSG;                    
                }               
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE 服務案件(一般/裝機/定維)狀態更新接口失敗，原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();                
            }

            writeToLog(beanIN.IV_SRID, "GetAPI_SRSTATUS_Update", pMsg, beanIN.IV_LOGINEMPNAME);

            return OUTBean;
        }
        #endregion

        #endregion -----↑↑↑↑↑一般服務 ↑↑↑↑↑-----

        #region -----↓↓↓↓↓裝機服務 ↓↓↓↓↓-----

        #region call ONE SERVICE 更新裝機現況資訊接口 
        /// <summary>
        /// call ONE SERVICE 更新裝機現況資訊接口
        /// </summary>
        /// <param name="beanIN"></param>
        public CURRENTINSTALLINFO_OUTPUT GetAPI_CURRENTINSTALLINFO_Update(CURRENTINSTALLINFO_INPUT beanIN)
        {
            CURRENTINSTALLINFO_OUTPUT OUTBean = new CURRENTINSTALLINFO_OUTPUT();

            string pMsg = string.Empty;
            string cID = "0";

            try
            {
                #region 取得該SRID是否有存在APP_INSTALL
                //var beanAPP = dbEIP.TbServicesAppInstalls.FirstOrDefault(x => x.Srid == beanIN.IV_SRID);
                var beanAPP = dbEIP.TbServicesAppInstalltemps.FirstOrDefault(x => x.Srid == beanIN.IV_SRID);

                if (beanAPP != null)
                {
                    cID = beanAPP.Id.ToString();
                }
                #endregion

                string tURL = beanIN.IV_APIURLName + "/API/API_CURRENTINSTALLINFO_UPDATE";

                var client = new RestClient(tURL);

                var request = new RestRequest();
                request.Method = RestSharp.Method.Post;

                Dictionary<Object, Object> parameters = new Dictionary<Object, Object>();
                parameters.Add("IV_LOGINEMPNO", beanIN.IV_LOGINEMPNO);
                parameters.Add("IV_SRID", beanIN.IV_SRID);
                parameters.Add("IV_CID", cID);
                parameters.Add("IV_InstallDate", beanIN.IV_InstallDate);
                parameters.Add("IV_ExpectedDate", beanIN.IV_ExpectedDate);
                parameters.Add("IV_TotalQuantity", beanIN.IV_TotalQuantity);
                parameters.Add("IV_InstallQuantity", beanIN.IV_InstallQuantity);
                parameters.Add("IV_IsFromAPP", beanIN.IV_IsFromAPP);                

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-MBX-APIKEY", "6xdTlREsMbFd0dBT28jhb5W3BNukgLOos");
                request.AddParameter("application/json", parameters, ParameterType.RequestBody);

                RestResponse response = client.Execute(request);

                #region 取得回傳訊息(成功或失敗)
                var data = (JObject)JsonConvert.DeserializeObject(response.Content);

                OUTBean.EV_MSGT = data["EV_MSGT"].ToString().Trim();
                OUTBean.EV_MSG = data["EV_MSG"].ToString().Trim();
                #endregion

                if (OUTBean.EV_MSGT == "Y")
                {
                    pMsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE 更新裝機現況資訊接口成功";
                }
                else
                {
                    pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE 更新裝機現況資訊接口失敗，原因:" + OUTBean.EV_MSG;
                }
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE 更新裝機現況資訊接口失敗，原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();
            }

            writeToLog(beanIN.IV_SRID, "GetAPI_CURRENTINSTALLINFO_Update", pMsg, beanIN.IV_LOGINEMPNAME);

            return OUTBean;
        }
        #endregion

        #endregion -----↑↑↑↑↑裝機服務 ↑↑↑↑↑-----

        #region -----↓↓↓↓↓合約管理 ↓↓↓↓↓-----

        #region call ONE SERVICE 查詢是否可以讀取合約書PDF權限接口
        /// <summary>
        /// call ONE SERVICE 查詢是否可以讀取合約書PDF權限接口
        /// </summary>
        /// <param name="beanIN"></param>
        public VIEWCONTRACTSMEMBERSINFO_OUTPUT GetAPI_VIEWCONTRACTSMEMBERSINFO(VIEWCONTRACTSMEMBERSINFO_INPUT beanIN)
        {
            VIEWCONTRACTSMEMBERSINFO_OUTPUT OUTBean = new VIEWCONTRACTSMEMBERSINFO_OUTPUT();

            string pMsg = string.Empty;

            try
            {
                string tURL = beanIN.IV_APIURLName + "/API/API_VIEWCONTRACTSMEMBERSINFO_GET";

                var client = new RestClient(tURL);

                var request = new RestRequest();
                request.Method = RestSharp.Method.Post;

                Dictionary<Object, Object> parameters = new Dictionary<Object, Object>();
                parameters.Add("IV_LOGINEMPNO", beanIN.IV_LOGINEMPNO);
                parameters.Add("IV_CONTRACTID", beanIN.IV_CONTRACTID);
                parameters.Add("IV_SRTEAM", beanIN.IV_SRTEAM);

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-MBX-APIKEY", "6xdTlREsMbFd0dBT28jhb5W3BNukgLOos");
                request.AddParameter("application/json", parameters, ParameterType.RequestBody);

                RestResponse response = client.Execute(request);

                #region 取得回傳訊息(成功或失敗)
                var data = (JObject)JsonConvert.DeserializeObject(response.Content);

                OUTBean.EV_MSGT = data["EV_MSGT"].ToString().Trim();
                OUTBean.EV_MSG = data["EV_MSG"].ToString().Trim();
                OUTBean.EV_IsCanRead = data["EV_IsCanRead"].ToString().Trim();
                #endregion

                if (OUTBean.EV_MSGT == "Y")
                {
                    pMsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE 查詢是否可以讀取合約書PDF權限接口接口成功";
                }
                else
                {
                    pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE 查詢是否可以讀取合約書PDF權限接口接口失敗，原因:" + OUTBean.EV_MSG;
                }
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE 服務案件(一般/裝機/定維)狀態更新接口失敗，原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();
            }

            writeToLog(beanIN.IV_CONTRACTID, "GetAPI_VIEWCONTRACTSMEMBERSINFO_GET", pMsg, beanIN.IV_LOGINEMPNAME);

            return OUTBean;
        }
        #endregion

        #region call ONE SERVICE 合約主數據資料新增/異動時發送Mail通知接口
        /// <summary>
        /// call ONE SERVICE 合約主數據資料新增/異動時發送Mail通知接口
        /// </summary>
        /// <param name="beanIN"></param>
        public CONTRACTCHANGE_OUTPUT GetAPI_CONTRACTCHANGE_SENDMAIL(CONTRACTCHANGE_INPUT beanIN)
        {
            CONTRACTCHANGE_OUTPUT OUTBean = new CONTRACTCHANGE_OUTPUT();

            string pMsg = string.Empty;

            try
            {
                string tURL = beanIN.IV_APIURLName + "/API/API_CONTRACTCHANGE_SENDMAIL";

                var client = new RestClient(tURL);

                var request = new RestRequest();
                request.Method = RestSharp.Method.Post;

                Dictionary<Object, Object> parameters = new Dictionary<Object, Object>();
                parameters.Add("IV_LOGINEMPNO", beanIN.IV_LOGINEMPNO);
                parameters.Add("IV_CONTRACTID", beanIN.IV_CONTRACTID);
                parameters.Add("IV_LOG", beanIN.IV_LOG);

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-MBX-APIKEY", "6xdTlREsMbFd0dBT28jhb5W3BNukgLOos");
                request.AddParameter("application/json", parameters, ParameterType.RequestBody);

                RestResponse response = client.Execute(request);

                #region 取得回傳訊息(成功或失敗)
                var data = (JObject)JsonConvert.DeserializeObject(response.Content);

                OUTBean.EV_MSGT = data["EV_MSGT"].ToString().Trim();
                OUTBean.EV_MSG = data["EV_MSG"].ToString().Trim();
                #endregion

                if (OUTBean.EV_MSGT == "Y")
                {
                    pMsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE 合約主數據資料新增/異動時發送Mail通知接口成功";
                }
                else
                {
                    pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE 合約主數據資料新增/異動時發送Mail通知接口失敗，原因:" + OUTBean.EV_MSG;
                }
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE 合約主數據資料新增/異動時發送Mail通知接口失敗，原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();
            }

            writeToLog(beanIN.IV_CONTRACTID, "GetAPI_CONTRACTCHANGE_SENDMAIL", pMsg, beanIN.IV_LOGINEMPNAME);

            return OUTBean;
        }
        #endregion

        #region 取得合約主數據相關人員資訊
        /// <summary>
        /// 取得合約主數據相關資訊
        /// </summary>        
        /// <param name="pOperationID_Contract">程式作業編號檔系統ID(合約主數據查詢/維護)</param>
        /// <param name="tLoginERPID">登入者ERPID</param>
        /// <param name="tLoginAccout">登入者AD帳號</param>
        /// <param name="tBUKRS">登入者公司別(T012、T016、C069、T022)</param>
        /// <param name="tCostCenterID">登入者成本中心ID</param>
        /// <param name="tDeptID">登人者部門ID</param>
        /// <param name="tIsMIS">登入者是否為MIS</param>
        /// <param name="tIsCSManager">登入者是否為客服主管</param>
        /// <param name="tIsDCC">登入者是否為文管中心人員</param>
        /// <param name="cContractID">文件編號</param>
        /// <param name="tType">MAIN.主數據(含下包) ENG.工程師明細 OBJ.合約標的</param>
        /// <returns></returns>
        public bool checkIsCanEditContracInfo(string pOperationID_Contract, string tLoginERPID, string tLoginAccout, string tBUKRS, string tCostCenterID, string tDeptID, bool tIsMIS, bool tIsCSManager, bool tIsDCC, string cContractID, string tType)
        {
            bool reValue = checkIsCanEditContracInfo(pOperationID_Contract, tLoginERPID, tLoginAccout, tBUKRS, tCostCenterID, tDeptID, tIsMIS, tIsCSManager, tIsDCC, cContractID, tType, null);

            return reValue;
        }
        #endregion

        #region 取得合約主數據相關人員資訊
        /// <summary>
        /// 取得合約主數據相關資訊
        /// </summary>        
        /// <param name="pOperationID_Contract">程式作業編號檔系統ID(合約主數據查詢/維護)</param>
        /// <param name="tLoginERPID">登入者ERPID</param>
        /// <param name="tLoginAccout">登入者AD帳號</param>
        /// <param name="tBUKRS">登入者公司別(T012、T016、C069、T022)</param>
        /// <param name="tCostCenterID">登入者成本中心ID</param>
        /// <param name="tDeptID">登人者部門ID</param>
        /// <param name="tIsMIS">登入者是否為MIS</param>
        /// <param name="tIsCSManager">登入者是否為客服主管</param>
        /// <param name="tIsDCC">登入者是否為文管中心人員</param>
        /// <param name="cContractID">文件編號</param>
        /// <param name="tType">MAIN.主數據(含下包) ENG.工程師明細 OBJ.合約標的</param>
        /// <param name="tMainList">合約主檔清單</param>
        /// <returns></returns>
        public bool checkIsCanEditContracInfo(string pOperationID_Contract, string tLoginERPID, string tLoginAccout, string tBUKRS, string tCostCenterID, string tDeptID, bool tIsMIS, bool tIsCSManager, bool tIsDCC, string cContractID, string tType, List<TbOneContractMain> tMainList)
        {
            bool reValue = false;
            bool tIsOldContractID = checkIsOldContractID(pOperationID_Contract, cContractID.Trim()); //判斷是否為舊文件編號(true.舊組織 false.新組織)

            TbOneContractMain beanM = new TbOneContractMain();

            if (tIsMIS || tIsCSManager || tIsDCC)
            {
                reValue = true;
            }
            else
            {
                if (tMainList == null)
                {
                     beanM = dbOne.TbOneContractMains.FirstOrDefault(x => x.Disabled == 0 && x.CContractId == cContractID.Trim());
                }
                else
                {
                    beanM = tMainList.FirstOrDefault(x => x.Disabled == 0 && x.CContractId == cContractID.Trim());
                }

                if (beanM != null)
                {
                    switch (tType)
                    {
                        case "MAIN":
                            #region 主數據(含下包)
                            Dictionary<string, string> DicORG = new Dictionary<string, string>(); //記錄合約相關人員

                            SetDtORGPeople(beanM.CSoSales, beanM.CSoSalesName, ref DicORG);
                            SetDtORGPeople(beanM.CSoSalesAss, beanM.CSoSalesAssname, ref DicORG);
                            SetDtORGPeople(beanM.CMasales, beanM.CMasalesName, ref DicORG);                            

                            if (DicORG.Keys.Contains(tLoginERPID))
                            {
                                reValue = true;
                            }

                            if (!reValue)
                            {
                                //先判斷是否為服務團隊主管
                                if (tIsOldContractID)
                                {
                                    reValue = checkEmpIsExistSRTeamMapping_OLD(pOperationID_Contract, tBUKRS, tLoginAccout); //舊組織
                                }
                                else
                                {
                                    reValue = checkIsSRTeamMappingManager(tLoginERPID, beanM.CTeamId); //新組織
                                }

                                if (!reValue)
                                {
                                    //再判斷是否為主要工程師
                                    reValue = checkIsContractEngineer(tLoginERPID, cContractID, "Y");
                                }
                            }
                            #endregion

                            break;

                        case "ENG":
                            #region 工程師明細
                            //先判斷是否為服務團隊主管
                            if (tIsOldContractID)
                            {
                                reValue = checkEmpIsExistSRTeamMapping_OLD(pOperationID_Contract, tBUKRS, tLoginAccout); //舊組織
                            }
                            else
                            {
                                reValue = checkIsSRTeamMappingManager(tLoginERPID, beanM.CTeamId); //新組織
                            }

                            if (!reValue)
                            {
                                //再判斷是否為主要工程師
                                reValue = checkIsContractEngineer(tLoginERPID, cContractID, "Y");
                            }
                            #endregion

                            break;

                        case "OBJ":
                            #region 合約標的
                            //先判斷是否為服務團隊主管
                            if (tIsOldContractID)
                            {
                                reValue = checkEmpIsExistSRTeamMapping_OLD(pOperationID_Contract, tBUKRS, tLoginAccout); //舊組織
                            }
                            else
                            {
                                reValue = checkIsSRTeamMappingManager(tLoginERPID, beanM.CTeamId); //新組織
                            }

                            if (!reValue)
                            {
                                //再判斷是否為主要工程師或協助工程師
                                reValue = checkIsContractEngineer(tLoginERPID, cContractID, "");
                            }
                            #endregion

                            break;
                    }
                }
            }

            return reValue;
        }
        #endregion

        #region 判斷登入人員是否有在傳入的服務團隊裡(true.有 false.否)，抓舊組織
        /// <summary>
        /// 判斷登入人員是否有在傳入的服務團隊裡(true.有 false.否)，抓舊組織
        /// </summary>
        /// <param name="pOperationID_Contract">程式作業編號檔系統ID(合約主數據查詢/維護)</param>
        /// <param name="tBUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="tAccountNo">AD帳號</param>
        /// <returns></returns>
        public bool checkEmpIsExistSRTeamMapping_OLD(string pOperationID_Contract, string tBUKRS, string tAccountNo)
        {
            bool reValue = false;

            var bean = psipDb.TbOneRoleParameters.FirstOrDefault(x => x.Disabled == 0 && x.COperationId.ToString() == pOperationID_Contract &&
                                                                    x.CFunctionId == "PERSON" && x.CCompanyId == tBUKRS &&
                                                                    x.CNo == "OLDORG" && x.CValue.ToLower() == tAccountNo.ToLower());

            if (bean != null)
            {
                reValue = true;
            }

            return reValue;
        }
        #endregion

        #region 判斷登入人員是否為該服務團隊主管
        /// <summary>
        /// 判斷登入人員是否為該服務團隊主管
        /// </summary>
        /// <param name="tLoginERPID">登入者ERPID</param>
        /// <param name="tTeamOldID">服務團隊ID</param>
        /// <returns></returns>      
        public bool checkIsSRTeamMappingManager(string tLoginERPID, string tTeamOldID)
        {
            bool reValue = false;

            List<string> tList = findALLDeptIDListbyTeamID(tTeamOldID);
            
            string tMGRERPID = string.Empty;            

            foreach (var tValue in tList)
            {
                tMGRERPID = findDeptMGRERPID(tValue);

                if (tMGRERPID == tLoginERPID)
                {
                    reValue = true;
                    break;
                }
            }

            return reValue;
        }
        #endregion       

        #region 判斷登入人員是否為該文件編號的主要工程師、協助工程師
        /// <summary>
        /// 判斷登入人員是否為該文件編號的主要工程師、協助工程師
        /// </summary>
        /// <param name="tLoginERPID">登入者ERPID</param>
        /// <param name="cContractID">文件編號</param>
        /// <param name="cIsMainEngineer">是否為主要工程師(Y or 空白)</param>
        /// <returns></returns>      
        public bool checkIsContractEngineer(string tLoginERPID, string cContractID, string cIsMainEngineer)
        {
            bool reValue = false;

            var bean = dbOne.TbOneContractDetailEngs.FirstOrDefault(x => x.Disabled == 0 && x.CContractId == cContractID && x.CEngineerId == tLoginERPID &&
                                                                     (string.IsNullOrEmpty(cIsMainEngineer) ? true : x.CIsMainEngineer == cIsMainEngineer));

            if (bean != null)
            {
                reValue = true;
            }

            return reValue;
        }
        #endregion

        #region 判斷是否為舊文件編號(true.舊組織 false.新組織)
        /// <summary>
        /// 判斷是否為舊文件編號(true.舊組織 false.新組織)
        /// </summary>
        /// <param name="pOperationID_Contract">程式作業編號檔系統ID(合約主數據查詢/維護)</param>
        /// <param name="cContractID">文件編號</param>
        /// <returns></returns>
        public bool checkIsOldContractID(string pOperationID_Contract, string cContractID)
        {
            bool reValue = false;

            string ContractIDLimit = findSysParameterValue(pOperationID_Contract, "OTHER", "T012", "ContractIDLimit");

            if (int.Parse(cContractID.Trim()) < int.Parse(ContractIDLimit))
            {
                reValue = true;
            }

            return reValue;
        }
        #endregion       

        #region 判斷傳入的工程師，是否已存在該文件編號裡的工程師明細內容(true.已存在 false.未存在)
        /// <summary>
        /// 判斷傳入的工程師，是否已存在該文件編號裡的工程師明細內容(true.已存在 false.未存在)
        /// </summary>
        /// <param name="cContractID">文件編號</param>        
        /// <param name="cID">系統ID</param>
        /// <param name="cIsMainEngineer">是否為主要工程師(Y or 空白)</param>
        /// <param name="cEngineerID">工程師ERPID</param>
        /// <returns></returns>
        public bool checkIsExitsEngineer(string cContractID, string cID, string cIsMainEngineer, string cEngineerID)
        {
            bool reValue = false;

            var beanM = dbOne.TbOneContractDetailEngs.FirstOrDefault(x => x.Disabled == 0 && x.CContractId == cContractID && 
                                                                      (string.IsNullOrEmpty(cIsMainEngineer) ? true :  x.CIsMainEngineer == cIsMainEngineer) &&
                                                                      (string.IsNullOrEmpty(cEngineerID) ? true : x.CEngineerId == cEngineerID) &&
                                                                      (string.IsNullOrEmpty(cID) ? true : x.CId != int.Parse(cID)));

            if (beanM != null)
            {
                reValue = true;
            }

            return reValue;
        }
        #endregion

        #region 取得下包合約供應商代號
        /// <summary>
        /// 取得下包合約供應商代號
        /// </summary>
        /// <param name="cSubContractID">下包文件編號</param>
        /// <returns></returns>
        public string findSubSupplierID(string cSubContractID)
        {
            string reValue = string.Empty;

            var beanSub = dbOne.TbOneContractDetailSubs.FirstOrDefault(x => x.Disabled == 0 && x.CSubContractId == cSubContractID);

            if (beanSub != null)
            {
                reValue = beanSub.CSubSupplierId;
            }

            return reValue;
        }
        #endregion

        #region 判斷傳入的序號是否已存在合約標的明細內容(true.已存在 false.未存在)
        /// <summary>
        /// 判斷傳入的序號是否已存在合約標的明細內容(true.已存在 false.未存在)
        /// </summary>
        /// <param name="cContractID">文件編號</param>
        /// <param name="cID">系統ID</param>
        /// <param name="cSerialID">序號</param>
        /// <returns></returns>
        public bool checkIsExitsContractDetailObj(string cContractID, string cID, string cSerialID)
        {
            bool reValue = false;

            var beanM = dbOne.TbOneContractDetailObjs.FirstOrDefault(x => x.Disabled == 0 && x.CContractId == cContractID &&
                                                                      (string.IsNullOrEmpty(cSerialID) ? true : x.CSerialId == cSerialID) &&                                                                      
                                                                      (string.IsNullOrEmpty(cID) ? true : x.CId != int.Parse(cID)));

            if (beanM != null)
            {
                reValue = true;
            }

            return reValue;
        }
        #endregion

        #endregion -----↑↑↑↑↑合約管理 ↑↑↑↑↑-----

        #region -----↓↓↓↓↓共用方法 ↓↓↓↓↓-----

        #region 取得登入者資訊
        public EmployeeBean findEmployeeInfo(string keyword)
        {
            EmployeeBean empBean = new EmployeeBean();

            bool tIsManager = false;

            var beanE = dbEIP.People.FirstOrDefault(x => x.Account.ToLower() == keyword.ToLower() && (x.LeaveDate == null && x.LeaveReason == null));

            if (beanE != null)
            {
                empBean.EmployeeNO = beanE.Account.Trim();
                empBean.EmployeeERPID = beanE.ErpId.Trim();
                empBean.EmployeeCName = beanE.Name2.Trim();
                empBean.EmployeeEName = beanE.Name.Trim();
                empBean.WorkPlace = beanE.WorkPlace.Trim();
                empBean.PhoneExt = beanE.Extension.Trim();
                empBean.CompanyCode = beanE.CompCde.Trim();
                empBean.BUKRS = getBUKRS(beanE.CompCde.Trim());
                empBean.EmployeeEmail = beanE.Email.Trim();
                empBean.EmployeePersonID = beanE.Id.Trim();

                #region 取得部門資訊
                var beanD = dbEIP.Departments.FirstOrDefault(x => x.Id == beanE.DeptId);

                if (beanD != null)
                {
                    empBean.DepartmentNO = beanD.Id.Trim();
                    empBean.DepartmentName = beanD.Name2.Trim();
                    empBean.ProfitCenterID = beanD.ProfitCenterId.Trim();
                    empBean.CostCenterID = beanD.CostCenterId.Trim();
                }
                #endregion

                #region 取得是否為主管
                var beansManager = dbEIP.Departments.Where(x => x.ManagerId == beanE.Id && x.Status == 0);

                if (beansManager.Count() > 0)
                {
                    tIsManager = true;
                }

                empBean.IsManager = tIsManager;
                #endregion
            }

            return empBean;
        }

        #region 取得SAP的公司別
        /// <summary>
        /// 取得SAP的公司別(T012、T016、C069、T022)
        /// </summary>
        /// <param name="tCompCode">公司別(Comp-1、Comp-2、Comp-3、Comp-4)</param>
        /// <returns></returns>
        public string getBUKRS(string tCompCode)
        {
            string reValue = "T012";

            switch (tCompCode.Trim())
            {
                case "Comp-1":
                    reValue = "T012";
                    break;
                case "Comp-2":
                    reValue = "T016";
                    break;
                case "Comp-3":
                    reValue = "C069";
                    break;
                case "Comp-4":
                    reValue = "T022";
                    break;
            }

            return reValue;
        }
        #endregion

        #region 人員資訊相關
        public struct EmployeeBean
        {
            /// <summary>人員帳號</summary>
            public string EmployeeNO { get; set; }
            /// <summary>ERPID</summary>
            public string EmployeeERPID { get; set; }
            /// <summary>中文姓名</summary>
            public string EmployeeCName { get; set; }
            /// <summary>英文姓名</summary>
            public string EmployeeEName { get; set; }
            /// <summary>工作地點</summary>
            public string WorkPlace { get; set; }
            /// <summary>分機</summary>
            public string PhoneExt { get; set; }
            /// <summary>公司別(Comp-1、Comp-2、Comp-3、Comp-4)</summary>
            public string CompanyCode { get; set; }
            /// <summary>工廠別(T012、T016、C069、T022_</summary>
            public string BUKRS { get; set; }
            /// <summary>部門代號</summary>
            public string DepartmentNO { get; set; }
            /// <summary>部門名稱</summary>
            public string DepartmentName { get; set; }
            /// <summary>利潤中心</summary>
            public string ProfitCenterID { get; set; }
            /// <summary>成本中心</summary>
            public string CostCenterID { get; set; }
            /// <summary>人員Email</summary>
            public string EmployeeEmail { get; set; }
            /// <summary>人員ID(Person資料表ID)</summary>
            public string EmployeePersonID { get; set; }
            /// <summary>是否為主管(true.是 false.否)</summary>
            public bool IsManager { get; set; }
        }
        #endregion

        #endregion

        #region 取得員工姓名
        /// <summary>
        /// 取得員工姓名
        /// </summary>
        /// <param name="tERPID">員工ERPID</param>
        /// <returns></returns>
        public string findEmployeeName(string tERPID)
        {
            string reValue = string.Empty;

           var bean = dbEIP.People.FirstOrDefault(x => x.ErpId == tERPID);

            if (bean != null)
            {
                reValue = bean.Name2;
            }

            return reValue;
        }
        #endregion       

        #region 查詢客戶資料By公司別(含法人和個人客戶)
        /// <summary>
        /// 查詢客戶資料By公司別(含法人和個人客戶)
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="compcde">公司別</param>
        /// <returns></returns>
        public IQueryable<ViewCustomerandpersonal> findCustByKeywordAndComp(string keyword, string compcde)
        {
            return dbProxy.ViewCustomerandpersonals.Where(x => x.KnvvVkorg == compcde &&
                                                          (x.Kna1Name1.Contains(keyword) || x.Kna1Kunnr.Contains(keyword))).Take(30);
        }
        #endregion

        #region 取得客戶聯絡人
        /// <summary>
        /// 取得客戶聯絡人
        /// </summary>
        /// <param name="cBUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="CustomerID">客戶代號</param>        
        /// <returns></returns>
        public List<PCustomerContact> GetContactInfo(string cBUKRS, string CustomerID)
        {
            List<string> tTempList = new List<string>();
            List<PCustomerContact> liPCContact = new List<PCustomerContact>();

            string tTempValue = string.Empty;
            string ContactMobile = string.Empty;
            string ContactStore = string.Empty;

            if (CustomerID.Substring(0, 1) == "P")
            {
                var qPjRec = dbProxy.PersonalContacts.OrderByDescending(x => x.ModifiedDate).
                                                   Where(x => x.Disabled == 0 && x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == CustomerID).ToList();

                if (qPjRec != null && qPjRec.Count() > 0)
                {
                    foreach (var prBean in qPjRec)
                    {
                        ContactMobile = string.IsNullOrEmpty(prBean.ContactMobile) ? "" : prBean.ContactMobile.Trim().Replace(" ", "");

                        PCustomerContact prDocBean = new PCustomerContact();

                        prDocBean.ContactID = prBean.ContactId.ToString();
                        prDocBean.CustomerID = prBean.Kna1Kunnr.Trim().Replace(" ", "");
                        prDocBean.CustomerName = prBean.Kna1Name1.Trim().Replace(" ", "");
                        prDocBean.BUKRS = cBUKRS;
                        prDocBean.Name = prBean.ContactName.Trim().Replace(" ", "");
                        prDocBean.City = prBean.ContactCity.Trim().Replace(" ", "");
                        prDocBean.Address = prBean.ContactAddress.Trim().Replace(" ", "");
                        prDocBean.Email = prBean.ContactEmail.Trim().Replace(" ", "");
                        prDocBean.Phone = prBean.ContactPhone.Trim().Replace(" ", "");
                        prDocBean.Mobile = ContactMobile;
                        prDocBean.BPMNo = "GenerallySR";

                        liPCContact.Add(prDocBean);
                    }
                }
            }
            else
            {
                var qPjRec = dbProxy.CustomerContacts.OrderByDescending(x => x.ModifiedDate).
                                                   Where(x => (x.Disabled == null || x.Disabled != 1) && 
                                                              x.ContactName != "" && x.ContactCity != "" && 
                                                              x.ContactAddress != "" && (x.ContactPhone != "" || (x.ContactMobile != "" && x.ContactMobile != null)) &&                                                              
                                                              x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == CustomerID).ToList();

                if (qPjRec != null && qPjRec.Count() > 0)
                {
                    foreach (var prBean in qPjRec)
                    {
                        tTempValue = prBean.Kna1Kunnr.Trim().Replace(" ", "") + "|" + cBUKRS + "|" + prBean.ContactName.Trim().Replace(" ", "") + "|" + prBean.ContactStore.ToString();

                        if (!tTempList.Contains(tTempValue)) //判斷客戶ID、公司別、聯絡人姓名、聯絡門市不重覆才要顯示
                        {
                            tTempList.Add(tTempValue);

                            ContactMobile = string.IsNullOrEmpty(prBean.ContactMobile) ? "" : prBean.ContactMobile.Trim().Replace(" ", "");
                            ContactStore = string.IsNullOrEmpty(prBean.ContactStore.ToString()) ? "" : prBean.ContactStore.ToString();

                            PCustomerContact prDocBean = new PCustomerContact();

                            prDocBean.ContactID = prBean.ContactId.ToString();
                            prDocBean.CustomerID = prBean.Kna1Kunnr.Trim().Replace(" ", "");
                            prDocBean.CustomerName = prBean.Kna1Name1.Trim().Replace(" ", "");
                            prDocBean.BUKRS = cBUKRS;
                            prDocBean.Name = prBean.ContactName.Trim().Replace(" ", "");
                            prDocBean.City = prBean.ContactCity.Trim().Replace(" ", "");
                            prDocBean.Address = prBean.ContactAddress.Trim().Replace(" ", "");
                            prDocBean.Email = prBean.ContactEmail.Trim().Replace(" ", "");
                            prDocBean.Phone = prBean.ContactPhone.Trim().Replace(" ", "");
                            prDocBean.Mobile = ContactMobile;
                            prDocBean.Store = ContactStore;
                            prDocBean.BPMNo = prBean.BpmNo.Trim().Replace(" ", "");

                            liPCContact.Add(prDocBean);
                        }
                    }
                }
            }            

            return liPCContact;
        }

        /// <summary>
        /// 取得客戶聯絡人資訊(模糊查詢)
        /// </summary>
        /// <param name="cBUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="CustomerID">客戶代號</param>    
        /// <param name="ContactName">聯絡人姓名</param>
        /// <returns></returns>
        public List<PCustomerContact> findContactInfoByKeywordAndComp(string cBUKRS, string CustomerID, string ContactName)
        {
            var qPjRec = dbProxy.CustomerContacts.OrderByDescending(x => x.ModifiedDate).
                                               Where(x => (x.Disabled == null || x.Disabled != 1) && x.ContactName != "" && x.ContactCity != "" &&
                                                          x.ContactAddress != "" && (x.ContactPhone != "" || (x.ContactMobile != "" && x.ContactMobile != null)) &&
                                                          x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == CustomerID && x.ContactName.Contains(ContactName)).ToList();

            List<string> tTempList = new List<string>();

            string tTempValue = string.Empty;
            string ContactMobile = string.Empty;
            string ContactStore = string.Empty;

            List<PCustomerContact> liPCContact = new List<PCustomerContact>();
            if (qPjRec != null && qPjRec.Count() > 0)
            {
                foreach (var prBean in qPjRec)
                {
                    tTempValue = prBean.Kna1Kunnr.Trim().Replace(" ", "") + "|" + cBUKRS + "|" + prBean.ContactName.Trim().Replace(" ", "") + "|" + prBean.ContactStore.ToString();

                    if (!tTempList.Contains(tTempValue)) //判斷客戶ID、公司別、聯絡人姓名、聯絡人門市不重覆才要顯示
                    {
                        tTempList.Add(tTempValue);

                        ContactMobile = string.IsNullOrEmpty(prBean.ContactMobile) ? "" : prBean.ContactMobile.Trim().Replace(" ", "");
                        ContactStore = string.IsNullOrEmpty(prBean.ContactStore.ToString()) ? "" : prBean.ContactStore.ToString();

                        PCustomerContact prDocBean = new PCustomerContact();

                        prDocBean.ContactID = prBean.ContactId.ToString();
                        prDocBean.CustomerID = prBean.Kna1Kunnr.Trim().Replace(" ", "");
                        prDocBean.CustomerName = prBean.Kna1Name1.Trim().Replace(" ", "");
                        prDocBean.BUKRS = cBUKRS;
                        prDocBean.Name = prBean.ContactName.Trim().Replace(" ", "");
                        prDocBean.City = prBean.ContactCity.Trim().Replace(" ", "");
                        prDocBean.Address = prBean.ContactAddress.Trim().Replace(" ", "");
                        prDocBean.Email = prBean.ContactEmail.Trim().Replace(" ", "");
                        prDocBean.Phone = prBean.ContactPhone.Trim().Replace(" ", "");
                        prDocBean.Mobile = ContactMobile;
                        prDocBean.Store = ContactStore;
                        prDocBean.BPMNo = prBean.BpmNo.Trim().Replace(" ", "");                        

                        liPCContact.Add(prDocBean);
                    }
                }
            }

            return liPCContact;
        }        
        #endregion

        #region 取得客戶聯絡資訊檔清單
        /// <summary>
        /// 取得客戶聯絡資訊檔清單
        /// </summary>
        /// <returns></returns>
        public List<TbOneSrdetailContact> findSRDetailContactList()
        {
            var beans = dbOne.TbOneSrdetailContacts.Where(x => x.Disabled == 0).ToList();

            return beans;
        }
        #endregion

        #region 取得客戶聯絡資訊檔的聯絡人名稱By List
        /// <summary>
        /// 取得客戶聯絡資訊檔的聯絡人名稱By List
        /// </summary>
        /// <param name="tList">服務團隊清單</param>
        /// <param name="tSRID">SRID</param>
        /// <returns></returns>
        public string TransSRDetailContactName(List<TbOneSrdetailContact> tList, string tSRID)
        {
            string reValue = string.Empty;

            var beans = tList.Where(x => x.CSrid == tSRID);

            foreach (var bean in beans)
            {
                reValue += bean.CContactName + "<br/>";
            }

            return reValue;
        }
        #endregion

        #region 判斷客戶聯絡人是否有重覆(true.重覆 false.無重覆)
        /// <summary>
        /// 判斷客戶聯絡人是否有重覆(true.重覆 false.無重覆)
        /// </summary>
        /// <param name="cID">系統ID(若為新增時則傳空白)</param>
        /// <param name="cBUKRS">工廠別(T012、T016、C069、T022)</param>
        /// <param name="cCustomerID">客戶代號</param>
        /// <param name="cContactName">聯絡人姓名</param>
        /// <param name="cContactStore">聯絡人門市</param>
        /// <returns></returns>
        public bool CheckContactsIsDouble(string cID, string cBUKRS, string cCustomerID, string cContactName, string cContactStore)
        {
            bool reValue = false;            

            if (cCustomerID.Substring(0, 1) == "P")
            {
                #region 個人客戶
                var beanT = dbProxy.PersonalContacts.FirstOrDefault(x => x.Disabled == 0 && x.Knb1Bukrs == cBUKRS &&
                                                                     x.Kna1Kunnr == cCustomerID && x.ContactName == cContactName &&
                                                                     (string.IsNullOrEmpty(cID) ? true : x.ContactId.ToString() != cID));
                if (beanT != null)
                {
                    reValue = true;
                }
                #endregion
            }
            else
            {
                #region 法人客戶
                var beanT = dbProxy.CustomerContacts.FirstOrDefault(x => (x.Disabled == null || x.Disabled != 1) && x.Knb1Bukrs == cBUKRS &&
                                                                     x.Kna1Kunnr == cCustomerID && x.ContactName == cContactName &&
                                                                     (string.IsNullOrEmpty(cContactStore) ? true : x.ContactStore.ToString() == cContactStore) &&
                                                                     (string.IsNullOrEmpty(cID) ? true : x.ContactId.ToString() != cID));

                if (beanT != null)
                {
                    reValue = true;
                }
                #endregion
            }

            return reValue;
        }
        #endregion

        #region 取得客戶名稱
        /// <summary>
        /// 取得客戶名稱
        /// </summary>
        /// <param name="tCustomerID">客戶代號</param>
        /// <returns></returns>
        public string findCustomerName(string tCustomerID)
        {
            string reValue = string.Empty;

            var bean = dbProxy.ViewCustomer2s.FirstOrDefault(x => x.Kna1Kunnr == tCustomerID);

            if (bean != null)
            {
                reValue = bean.Kna1Name1;
            }

            return reValue;               
        }
        #endregion

        #region 判斷登入者是否為MIS
        /// <summary>
        /// 判斷登入者是否為MIS
        /// </summary>
        /// <param name="LoginAccount">登入者帳號</param>
        /// <param name="tSysOperationID">程式作業編號檔系統ID(ALL，固定的GUID)</param>
        /// <returns></returns>
        public bool getIsMIS(string LoginAccount, string tSysOperationID)
        {
            bool reValue = false;

            Guid tcID = new Guid(tSysOperationID); //全模組

            var beans = psipDb.TbOneSysParameters.Where(x => x.Disabled == 0 && x.COperationId == tcID && x.CFunctionId == "ACCOUNT" && x.CCompanyId == "ALL" && x.CNo == "MIS");

            foreach (var bean in beans)
            {
                if (bean.CValue.ToLower() == LoginAccount.ToLower())
                {
                    reValue = true;
                    break;
                }
            }

            return reValue;
        }
        #endregion

        #region 判斷登入者是否為客服人員
        /// <summary>
        /// 判斷登入者是否為客服人員
        /// </summary>
        /// <param name="LoginAccount">登入者帳號</param>
        /// <param name="tSysOperationID">程式作業編號檔系統ID(ALL，固定的GUID)</param>
        /// <returns></returns>
        public bool getIsCustomerService(string LoginAccount, string tSysOperationID)
        {
            bool reValue = false;

            Guid tcID = new Guid(tSysOperationID); //全模組

            var beans = psipDb.TbOneSysParameters.Where(x => x.Disabled == 0 && x.COperationId == tcID && x.CFunctionId == "ACCOUNT" && x.CCompanyId == "ALL" && x.CNo == "CUSTOMERSERVICE");

            foreach (var bean in beans)
            {
                if (bean.CValue.ToLower() == LoginAccount.ToLower())
                {
                    reValue = true;
                    break;
                }
            }

            return reValue;
        }
        #endregion

        #region 判斷登入者是否為客服主管
        /// <summary>
        /// 判斷登入者是否為客服主管
        /// </summary>
        /// <param name="LoginAccount">登入者帳號</param>
        /// <param name="tSysOperationID">程式作業編號檔系統ID(ALL，固定的GUID)</param>
        /// <returns></returns>
        public bool getIsCustomerServiceManager(string LoginAccount, string tSysOperationID)
        {
            bool reValue = false;

            Guid tcID = new Guid(tSysOperationID); //全模組

            var beans = psipDb.TbOneSysParameters.Where(x => x.Disabled == 0 && x.COperationId == tcID && x.CFunctionId == "ACCOUNT" && x.CCompanyId == "ALL" && x.CNo == "CUSTOMERSERVICEManager");

            foreach(var bean in beans)
            {
                if (bean.CValue.ToLower() == LoginAccount.ToLower())
                {
                    reValue = true;
                    break;
                }
            }

            return reValue;
        }
        #endregion

        #region 判斷登入者是否為文管人員
        /// <summary>
        /// 判斷登入者是否為文管人員
        /// </summary>
        /// <param name="LoginAccount">登入者帳號</param>
        /// <param name="tSysOperationID">程式作業編號檔系統ID(ALL，固定的GUID)</param>
        /// <returns></returns>
        public bool getIsDocumentCenter(string LoginAccount, string tSysOperationID)
        {
            bool reValue = false;

            Guid tcID = new Guid(tSysOperationID); //全模組

            var beans = psipDb.TbOneSysParameters.Where(x => x.Disabled == 0 && x.COperationId == tcID && x.CFunctionId == "ACCOUNT" && x.CCompanyId == "ALL" && x.CNo == "DOCUMENTCENTER");

            foreach (var bean in beans)
            {
                if (bean.CValue.ToLower() == LoginAccount.ToLower())
                {
                    reValue = true;
                    break;
                }
            }

            return reValue;
        }
        #endregion

        #region 判斷登入者是否可以編輯服務案件
        /// <summary>
        /// 判斷登入者是否可以編輯服務案件
        /// </summary>
        /// <param name="tSRID">SRID</param>
        /// <param name="tLoginERPID">登入者ERPID</param>
        /// <param name="tIsMIS">登入者是否為MIS</param>
        /// <param name="tIsCS">登入者是否為客服人員</param>
        /// <returns></returns>
        public bool checkIsCanEditSR(string tSRID, string tLoginERPID, bool tIsMIS, bool tIsCS)
        {
            bool reValue = false;

            //服務團隊主管、主要工程師、協助工程師、技術主管
            var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == tSRID);

            if (beanM != null)
            {
                if (beanM.CStatus == "E0006" || beanM.CStatus == "E0015") //完修或取修不可編輯
                {
                    return false;
                }

                if (tIsMIS || tIsCS) //MIS or 客服人員都可編輯
                {
                    reValue = true;
                }
                else
                {
                    #region 主要工程師可編輯
                    if (beanM.CMainEngineerId != null)
                    {
                        if (beanM.CMainEngineerId == tLoginERPID)
                        {
                            reValue = true;
                        }
                    }
                    #endregion

                    #region 服務團隊主管可編輯
                    if (!reValue)
                    {
                        string tMGRERPID = string.Empty;
                        string cTeamOldID = beanM.CTeamId;

                        var beansT = dbOne.TbOneSrteamMappings.Where(x => x.Disabled == 0 && cTeamOldID.Contains(x.CTeamOldId));

                        foreach(var beanT in beansT)
                        {
                            tMGRERPID = findDeptMGRERPID(beanT.CTeamNewId);

                            if (tLoginERPID == tMGRERPID)
                            {
                                reValue = true;
                                break;
                            }
                        }
                    }
                    #endregion

                    #region 【裝機服務】業務人員/業務祕書新建狀態才可編輯
                    if (!reValue)
                    {
                        if (beanM.CSrid.Substring(0, 2) == "63") //裝機服務
                        {
                            if (beanM.CStatus == "E0001") //新建
                            {
                                #region 業務人員
                                if (beanM.CSalesId != null)
                                {
                                    if (beanM.CSalesId == tLoginERPID)
                                    {
                                        reValue = true;
                                    }
                                }
                                #endregion

                                #region 業務祕書
                                if (beanM.CSecretaryId != null)
                                {
                                    if (beanM.CSecretaryId == tLoginERPID)
                                    {
                                        reValue = true;
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                }

                if (!reValue)
                {
                    if (beanM.CStatus == "E0007") //技術支援升級(技術主管可編輯)
                    {
                        if (beanM.CTechManagerId != null)
                        {
                            if (beanM.CTechManagerId.Contains(tLoginERPID))
                            {
                                reValue = true;
                            }
                        }
                    }                    
                    else //非L2處理中狀態(協助工程師都可以編輯)
                    {
                        #region 協助工程師
                        if (beanM.CStatus != "E0002")
                        {
                            if (beanM.CAssEngineerId != null)
                            {
                                if (beanM.CAssEngineerId.Contains(tLoginERPID))
                                {
                                    reValue = true;
                                }
                            }
                        }
                        #endregion                       
                    }
                }
            }

            return reValue;
        }
        #endregion

        #region 取得該部門主管的ERPID
        /// <summary>
        /// 取得該部門主管的ERPID
        /// </summary>
        /// <param name="DEPTID">部門ID</param>        
        /// <returns></returns>
        public string findDeptMGRERPID(string DEPTID)
        {
            string reValue = string.Empty;
            string tManagerID = string.Empty;

            var beanDept = dbEIP.Departments.FirstOrDefault(x => x.Id == DEPTID);

            if (beanDept != null)
            {
                tManagerID = beanDept.ManagerId;

                if (tManagerID != "")
                {
                    var beanP = dbEIP.People.FirstOrDefault(x => x.Id == tManagerID);

                    if (beanP != null)
                    {
                        reValue = beanP.ErpId;
                    }
                }
            }

            return reValue;
        }
        #endregion

        #region 取得所有組織的DataTable
        /// <summary>
        /// 取得所有組織的DataTable
        /// </summary>
        /// <returns></returns>
        protected DataTable GetOrgDt()
        {
            DataTable dt = new DataTable();

            string cmmStr = @"select ID,ParentID,Name,Level from Department where ((Status='0' and Level <> '0') or (Status='0' and ParentID IS NULL)) AND NOT(ID LIKE '9%' or ID like '12GH%') ";

            dt = getDataTableByDb(cmmStr, "dbEIP");

            return dt;
        }
        #endregion

        #region 傳入最上層部門ID，並取得底下所有子部門ID
        /// <summary>
        /// 傳入最上層部門ID，並取得底下所有子部門ID
        /// </summary>
        /// <param name="tParentID">上層部門ID</param>
        /// <returns></returns>
        public List<string> GetALLSubDeptID(string tParentID)
        {
            List<string> tList = new List<string>();

            string reValue = string.Empty;
            string tmpNodeID = string.Empty;
            string tAllDept = string.Empty;

            DataTable dt = GetOrgDt(); //取得所有組織的DataTable
            DataRow[] rows = dt.Select("ID = '" + tParentID + "'");

            bool rc;

            foreach (DataRow row in rows)
            {
                tmpNodeID = row["ID"].ToString();

                tAllDept = tmpNodeID + ",";

                rc = AddNodes_Dept(tmpNodeID, ref dt, ref tAllDept);
            }

            reValue = tAllDept.TrimEnd(',');

            tList = reValue.Split(',').ToList();

            return tList;
        }
        #endregion

        #region 取得子節點，遞廻(部門代號)
        private bool AddNodes_Dept(string PID, ref DataTable dt, ref string tAllDept)
        {
            try
            {
                string tmpNodeID;

                DataRow[] rows = dt.Select("ParentID = '" + PID + "'");

                if (rows.GetUpperBound(0) >= 0)
                {
                    bool rc;

                    foreach (DataRow row in rows)
                    {
                        tmpNodeID = row["ID"].ToString();

                        tAllDept += tmpNodeID + ",";

                        rc = AddNodes_Dept(tmpNodeID, ref dt, ref tAllDept);
                    }
                }

                rows = null;

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion 

        #region 判斷Email格式是否正確
        /// <summary>
        /// 判斷Email格式是否正確
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsEmailValid(string email)
        {
            var valid = true;

            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }

            return valid;
        }
        #endregion

        #region 取號(SRID)
        /// <summary>
        /// 取號(SRID)
        /// </summary>
        /// <param name="cTilte">SRID開頭</param>
        /// <param name="cSRID">SRID</param>
        /// <returns></returns>
        public string GetSRID(string cTilte, string cSRID)
        {
            string strCNO = "";
            string tYear = DateTime.Now.Year.ToString().Substring(2, 2);
            string tMonth = DateTime.Now.Month.ToString().PadLeft(2, '0');
            string tDay = DateTime.Now.Day.ToString().PadLeft(2, '0');

            //若原本就有值就不須再取號
            if (string.IsNullOrEmpty(cSRID))
            {
                var bean = dbOne.TbOneSridformats.FirstOrDefault(x => x.CTitle == cTilte && x.CYear == tYear && x.CMonth == tMonth && x.CDay == tDay);

                if (bean == null) //若沒有資料，則新增一筆當月的資料
                {
                    TbOneSridformat FormNoTable = new TbOneSridformat();

                    FormNoTable.CTitle = cTilte;
                    FormNoTable.CYear = tYear;
                    FormNoTable.CMonth = tMonth;
                    FormNoTable.CDay = tDay;
                    FormNoTable.CNo = "0000";

                    dbOne.TbOneSridformats.Add(FormNoTable);
                    dbOne.SaveChanges();
                }

                bean = dbOne.TbOneSridformats.FirstOrDefault(x => x.CTitle == cTilte && x.CYear == tYear && x.CMonth == tMonth && x.CDay == tDay);

                if (bean != null)
                {
                    strCNO = cTilte + (bean.CYear + bean.CMonth + bean.CDay) + (int.Parse(bean.CNo) + 1).ToString().PadLeft(4, '0');
                    bean.CNo = (int.Parse(bean.CNo) + 1).ToString().PadLeft(4, '0');

                    dbOne.SaveChanges();
                }
            }
            else
            {
                strCNO = cSRID.Trim();
            }

            return strCNO;
        }
        #endregion

        #region 取SQ人員流水號
        /// <summary>
        /// 取SQ人員流水號
        /// </summary>
        /// <param name="cFirstKey">第1碼代號</param>
        /// <param name="cSecondKEY">區域代號(第2碼代號)</param>
        /// <param name="cThirdKEY">類別代號(第3碼代號)</param>
        /// <returns></returns>
        public string GetSRSQNo(string cFirstKey, string cSecondKEY, string cThirdKEY)
        {
            string strCNO = "01";         

            var bean = dbOne.TbOneSrsqpeople.OrderByDescending(x => x.CId).FirstOrDefault(x => x.CFirstKey == cFirstKey && x.CSecondKey == cSecondKEY && x.CThirdKey == cThirdKEY);

            if (bean != null)
            {
                strCNO = (int.Parse(bean.CNo) + 1).ToString().PadLeft(2, '0');
            }

            return strCNO;
        }
        #endregion

        #region 取SQ人員代號和說明
        /// <summary>
        /// 取SQ人員流水號
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>        
        /// <param name="cFirstKey">第1碼代號</param>
        /// <param name="cSecondKEY">區域代號(第2碼代號)</param>
        /// <param name="cThirdKEY">類別代號(第3碼代號)</param>
        /// <param name="cNo">流水號</param>
        /// <param name="cContent">證照編號</param>
        /// <param name="cEngineerName">工程師姓名</param>
        /// <returns></returns>
        public string[] GetSRSQFullInfo(string cOperationID, string cFirstKey, string cSecondKEY, string cThirdKEY, string cNo, string cContent, string cEngineerName)
        {
            string[] reValue = new string[2];
            string tKey = string.Empty;
            string tName = string.Empty;

            string tSecName = findSysParameterDescription(cOperationID, "OTHER", "T012", "SQSECKEY", cSecondKEY);
            string tThiName = findSysParameterDescription(cOperationID, "OTHER", "T012", "SQTHIKEY", cThirdKEY);

            //ex.ZA101
            tKey = cFirstKey + cSecondKEY + cThirdKEY + cNo;

            //ex.ISS-台北-PL70708859-陳勁嘉
            tName = tThiName + "-" + tSecName + "-" + cContent + "-" + cEngineerName;

            reValue[0] = tKey;
            reValue[1] = tName;

            return reValue;
        }
        #endregion

        #region 判斷系統目前GUID是否已被異動
        /// <summary>
        /// 判斷系統目前GUID是否已被異動(回傳CHANGE代表被動異動；回傳表單編號代表沒被異動)
        /// </summary>
        /// <param name="cSRID">SRID</param>
        /// <param name="tGUID">傳入的GUID</param>
        /// <returns></returns>
        public string checkSRIDIsChang(string cSRID, string tGUID)
        {
            string reValue = cSRID;            

            var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == cSRID);

            if (beanM != null)
            {
                if (tGUID != beanM.CSystemGuid.ToString())
                {
                    reValue = "CHANGE";
                }
            }

            return reValue;
        }
        #endregion        

        #region 取得製造商零件號碼
        /// <summary>
        /// 取得製造商零件號碼
        /// </summary>        
        /// <param name="IV_MATERIAL">物料代號</param>
        /// <returns></returns>
        public string findMFRPNumber(string IV_MATERIAL)
        {
            string reValue = string.Empty;            

            #region 取得製造商零件號碼
            var beanM = dbProxy.Materials.FirstOrDefault(x => x.MaraMatnr == IV_MATERIAL.Trim());

            if (beanM != null)
            {
                reValue = beanM.MaraMfrpn;                
            }
            #endregion           

            return reValue;
        }
        #endregion

        #region 取得物料廠牌
        /// <summary>
        /// 取得物料廠牌
        /// </summary>
        /// <param name="tMaterialID">物料代號</param>        
        /// <param name="tMVKE_PRODH">產品階層</param>
        public string findMATERIALBRAND(string tMaterialID, string tMVKE_PRODH)
        {
            string reValue = String.Empty;                        

            #region 取得廠牌
            if (tMVKE_PRODH != "")
            {
                var beanF = dbProxy.F0005s.FirstOrDefault(x => x.Modt == "MM" && x.Alias == "ProHierarchy" &&
                                                            x.Codet == "3" && x.Codets == tMVKE_PRODH.Substring(3, 3));

                if (beanF != null)
                {
                    reValue = beanF.Dsc1;
                }
                else
                {
                    reValue = "Other";
                }
            }
            else
            {
                reValue = "Other";
            }
            #endregion

            return reValue;
        }
        #endregion

        #region 取得裝機號碼(83 or 63)
        /// <summary>
        /// 取得裝機號碼(83 or 63)
        /// </summary>        
        /// <param name="IV_SERIAL">序號</param>
        /// <returns></returns>
        public string findInstallNumber(string IV_SERIAL)
        {
            string reValue = string.Empty;

            #region 取得製造商零件號碼
            var beanM = psipDb.TbPisInstallmaterials.FirstOrDefault(x => x.Srserial == IV_SERIAL.Trim());

            if (beanM != null)
            {
                reValue = beanM.Srid;
            }
            #endregion           

            return reValue;
        }
        #endregion

        #region 取得物料BOM表
        /// <summary>
        /// 取得物料BOM表
        /// </summary>        
        /// <param name="tMARA_MATNR">料號</param>
        /// <returns></returns>
        public string findMaterialBOM(string tMARA_MATNR)
        {
            string reValue = string.Empty;

            var bean = dbProxy.Materials.FirstOrDefault(x => x.MaraMatnr == tMARA_MATNR && x.BasicContent != "");

            if (bean != null)
            {
                if (bean.BasicContent != null)
                {
                    reValue = bean.BasicContent;
                }
            }

            return reValue;
        }
        #endregion

        #region 依關鍵字查詢物料資訊
        /// <summary>
        /// 依關鍵字查詢物料資訊
        /// </summary>    
        /// <param name="cBUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="keyword">料號/料號說明關鍵字</param>
        /// <returns></returns>
        public Object findMaterialByKeyWords(string cBUKRS, string keyword)
        {
            string cMARD_WERKS = string.Empty;

            switch(cBUKRS)
            {
                case "T012":
                    cMARD_WERKS = "12G9";
                    break;
                case "T016":
                    cMARD_WERKS = "16G9";
                    break;
            }

            Object contentObj = dbProxy.ViewMaterialByComps.Where(x => (x.MaraMatnr.Contains(keyword) || x.MaktTxza1Zf.Contains(keyword)) &&
                                                                   (string.IsNullOrEmpty(cMARD_WERKS) ? true : x.MardWerks == cMARD_WERKS)
                                                             ).Take(8);

            return contentObj;
        }
        #endregion

        #region 取得服務團隊名稱
        /// <summary>
        /// 取得服務團隊名稱
        /// </summary>
        /// <param name="cTeamOldId">服務團隊ID</param>
        /// <returns></returns>
        public string findTeamName(string cTeamOldId)
        {
            string reValue = string.Empty;

            var bean = dbOne.TbOneSrteamMappings.FirstOrDefault(x => x.CTeamOldId == cTeamOldId);

            if (bean != null)
            {
                reValue = bean.CTeamOldName;
            }

            return reValue;
        }
        #endregion

        #region 取得服務團隊ID所對應的部門代號清單
        /// <summary>
        /// 取得服務團隊ID所對應的部門代號清單
        /// </summary>
        /// <param name="cTeamOldId">服務團隊ID</param>
        /// <returns></returns>
        public List<string> findDeptIDListbyTeamID(string cTeamOldId)
        {
            List<string> tList = new List<string>();
            List<string> tTeamList = cTeamOldId.Split(';').ToList();

            string reValue = string.Empty;

            var beans = dbOne.TbOneSrteamMappings.Where(x => x.Disabled == 0  && tTeamList.Contains(x.CTeamOldId));

            foreach(var bean in beans)
            {
                if (!tList.Contains(bean.CTeamNewId))
                {
                    tList.Add(bean.CTeamNewId);
                }
            }

            return tList;
        }
        #endregion

        #region 取得服務團隊ID下所對應的部門(含子部門)
        /// <summary>
        /// 取得服務團隊ID下所對應的部門(含子部門)
        /// </summary>
        /// <param name="cTeamOldId">服務團隊ID</param>
        /// <returns></returns>
        public List<string> findALLDeptIDListbyTeamID(string cTeamOldId)
        {
            List<string> tAllDeptIDList = new List<string>();
            List<string> tDeptIDList = findDeptIDListbyTeamID(cTeamOldId);

            foreach (string tValue in tDeptIDList)
            {
                List<string> tLIst = GetALLSubDeptID(tValue);

                tAllDeptIDList.AddRange(tLIst);
            }

            return tAllDeptIDList;
        }
        #endregion

        #region 取得OneService服務案件URL
        /// <summary>
        /// 取得OneService服務案件URL
        /// </summary>
        /// <param name="cSRID">SRID</param>
        /// <returns></returns>
        public string findSRIDUrl(string cSRID)
        {
            string reValue = string.Empty;

            switch (cSRID.Substring(0, 2))
            {
                case "61":  //一般服務
                    reValue = "../ServiceRequest/GenerallySR?SRID=" + cSRID;
                    break;

                case "63":  //裝機服務
                    reValue = "../ServiceRequest/InstallSR?SRID=" + cSRID;
                    break;

                case "65":  //定維服務
                    reValue = "../ServiceRequest/MaintainSR?SRID=" + cSRID;
                    break;
            }

            return reValue;
        }
        #endregion

        #region 取得客戶故障狀況分類資訊(L2處理中)說明
        /// <summary>
        /// 取得客戶故障狀況分類資訊(L2處理中)說明
        /// </summary>
        /// <param name="tInValue">傳入值</param>
        /// <param name="tType">Group.客戶故障狀況分類、State.客戶故障狀況、Spec.故障零件規格料號</param>
        /// <returns></returns>
        public string TransL2Fault(string tInValue, string tType)
        {
            string reValue = string.Empty;

            if (tInValue != "")
            {
                string[] tAryValue = tInValue.TrimEnd(';').Split(';');

                if (tType == "Group")
                {
                    foreach (string tValue in tAryValue)
                    {
                        switch (tValue)
                        {
                            case "Z01":
                                reValue += "硬體;";
                                break;
                            case "Z02":
                                reValue += "系統;";
                                break;
                            case "Z03":
                                reValue += "服務;";
                                break;
                            case "Z04":
                                reValue += "網路;";
                                break;
                            case "Z99":
                                reValue += "其他;";
                                break;
                        }
                    }
                }
                else if (tType == "State")
                {
                    foreach (string tValue in tAryValue)
                    {
                        switch (tValue)
                        {
                            case "Z01":
                                reValue += "面板燈號;";
                                break;
                            case "Z02":
                                reValue += "管理介面(iLO、IMM、iDRAC);";
                                break;
                            case "Z99":
                                reValue += "其他;";
                                break;
                        }
                    }
                }
                else if (tType == "Spec")
                {
                    foreach (string tValue in tAryValue)
                    {
                        switch (tValue)
                        {
                            case "Z01":
                                reValue += "零件規格;";
                                break;
                            case "Z02":
                                reValue += "零件料號;";
                                break;
                            case "Z99":
                                reValue += "其他;";
                                break;
                        }
                    }
                }
            }

            return reValue.TrimEnd(';');
        }
        #endregion

        #region 新增時取得個人客戶流水號ID
        /// <summary>
        /// 新增時取得個人客戶流水號ID
        /// </summary>
        /// <returns></returns>
        public string findPERSONALISerialID()
        {
            string reValue = string.Empty;

            int tSerialID = 1;

            var bean = dbProxy.PersonalContacts.OrderByDescending(x => x.Kna1Kunnr).FirstOrDefault();

            if (bean != null)
            {
                tSerialID = int.Parse(bean.Kna1Kunnr.Replace("P", "")) + 1;
                reValue = "P" + tSerialID.ToString().PadLeft(8, '0');
            }
            else
            {
                reValue = "P00000001";
            }

            return reValue;
        }
        #endregion

        #region 取得服務狀態清單
        /// <summary>
        /// 取得服務狀態清單
        /// </summary>
        /// <param name="cOperationID_GenerallySR">程式作業編號檔系統ID(一般)</param>
        /// <param name="cOperationID_InstallSR">程式作業編號檔系統ID(裝機)</param>
        /// <param name="cOperationID_MaintainSR">程式作業編號檔系統ID(定維)</param>
        /// <param name="cCompanyID">公司別</param>
        /// <returns></returns>
        public List<SelectListItem> findSRStatus(string cOperationID_GenerallySR, string cOperationID_InstallSR, string cOperationID_MaintainSR, string cCompanyID)
        {
            List<SelectListItem> ListTempStatus = new List<SelectListItem>();

            List<SelectListItem> ListStatus_Gen = findSysParameterListItem(cOperationID_GenerallySR, "OTHER", cCompanyID, "SRSTATUS", false);   //一般服務
            List<SelectListItem> ListStatus_Ins = findSysParameterListItem(cOperationID_InstallSR, "OTHER", cCompanyID, "SRSTATUS", false);     //裝機服務
            List<SelectListItem> ListStatus_Man = findSysParameterListItem(cOperationID_MaintainSR, "OTHER", cCompanyID, "SRSTATUS", false);    //定維服務

            ListTempStatus = findSRStatus(ListTempStatus, ListStatus_Gen);
            ListTempStatus = findSRStatus(ListTempStatus, ListStatus_Ins);
            ListTempStatus = findSRStatus(ListTempStatus, ListStatus_Man);
            ListTempStatus = ListTempStatus.OrderBy(x => x.Value).ToList();

            return ListTempStatus;
        }
        #endregion

        #region 取得服務狀態清單
        /// <summary>
        /// 取得服務狀態清單
        /// </summary>
        /// <param name="ListOriStatus">來源的清單</param>
        /// <param name="ListInputStatus">傳入卻比對的清單</param>
        public List<SelectListItem> findSRStatus(List<SelectListItem> ListOriStatus, List<SelectListItem> ListInputStatus)
        {
            List<SelectListItem> ListTempStatus = new List<SelectListItem>();
            ListTempStatus.AddRange(ListOriStatus);

            if (ListTempStatus.Count == 0)
            {
                foreach (var beanG in ListInputStatus)
                {
                    ListTempStatus.Add(new SelectListItem { Text = beanG.Text, Value = beanG.Value });
                }
            }
            else
            {
                foreach (var beanG in ListInputStatus)
                {
                    bool tIsMatch = false;

                    foreach (var bean in ListOriStatus)
                    {
                        if (beanG.Value == bean.Value)
                        {
                            tIsMatch = true;
                            break;
                        }
                    }

                    if (!tIsMatch) //不符合才新增
                    {
                        ListTempStatus.Add(new SelectListItem { Text = beanG.Text, Value = beanG.Value });
                    }
                }
            }

            return ListTempStatus;
        }
        #endregion       

        #region 取得【資訊系統參數設定檔】的參數值清單(回傳SelectListItem)
        /// <summary>
        /// 取得【資訊系統參數設定檔】的參數值清單(回傳SelectListItem)
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cFunctionID">功能別(OTHER.其他自定義)</param>
        /// <param name="cCompanyID">公司別(ALL.全集團、T012.大世科、T016.群輝、C069.大世科技上海、T022.協志科)</param>
        /// <param name="cNo">參數No</param>
        /// <param name="cEmptyOption">是否要產生「請選擇」選項(True.要 false.不要)</param>
        /// <returns></returns>
        public List<SelectListItem> findSysParameterListItem(string cOperationID, string cFunctionID, string cCompanyID, string cNo, bool cEmptyOption)
        {
            var tList = new List<SelectListItem>();
            List<string> tTempList = new List<string>();

            string tKEY = string.Empty;
            string tNAME = string.Empty;

            var beans = psipDb.TbOneSysParameters.OrderBy(x => x.COperationId).ThenBy(x => x.CFunctionId).ThenBy(x => x.CCompanyId).ThenBy(x => x.CNo).ThenBy(x => x.CValue).
                                               Where(x => x.Disabled == 0 &&
                                                          x.COperationId.ToString() == cOperationID &&
                                                          x.CFunctionId == cFunctionID.Trim() &&
                                                          x.CCompanyId == cCompanyID.Trim() &&
                                                          x.CNo == cNo.Trim());

            if (cEmptyOption)
            {
                tList.Add(new SelectListItem { Text = "請選擇", Value = "" });
            }

            foreach (var bean in beans)
            {
                if (!tTempList.Contains(bean.CValue))
                {
                    tNAME = bean.CValue + "_" + bean.CDescription;

                    tList.Add(new SelectListItem { Text = tNAME, Value = bean.CValue });
                    tTempList.Add(bean.CValue);
                }
            }

            return tList;
        }
        #endregion

        #region 取得【資訊系統參數設定檔】的參數值
        /// <summary>
        /// 取得【資訊系統參數設定檔】的參數值
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cFunctionID">功能別(SENDMAIL.寄送Mail、ACCOUNT.取得人員帳號、OTHER.其他自定義)</param>
        /// <param name="cCompanyID">公司別(ALL.全集團、T012.大世科、T016.群輝、C069.大世科技上海、T022.協志科)</param>
        /// <param name="cNo">參數No</param>
        /// <returns></returns>
        public string findSysParameterValue(string cOperationID, string cFunctionID, string cCompanyID, string cNo)
        {
            string reValue = string.Empty;

            var bean = psipDb.TbOneSysParameters.FirstOrDefault(x => x.Disabled == 0 &&
                                                                 x.COperationId.ToString() == cOperationID &&
                                                                 x.CFunctionId == cFunctionID.Trim() &&
                                                                 x.CCompanyId == cCompanyID.Trim() &&
                                                                 x.CNo == cNo.Trim());

            if (bean != null)
            {
                reValue = bean.CValue;
            }

            return reValue;
        }
        #endregion

        #region 取得【資訊系統參數設定檔】的參數值說明
        /// <summary>
        /// 取得【資訊系統參數設定檔】的參數值
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cFunctionID">功能別(SENDMAIL.寄送Mail、ACCOUNT.取得人員帳號、OTHER.其他自定義)</param>
        /// <param name="cCompanyID">公司別(ALL.全集團、T012.大世科、T016.群輝、C069.大世科技上海、T022.協志科)</param>
        /// <param name="cNo">參數No</param>
        /// <param name="cValue">參數值</param>
        /// <returns></returns>
        public string findSysParameterDescription(string cOperationID, string cFunctionID, string cCompanyID, string cNo, string cValue)
        {
            string reValue = string.Empty;

            var bean = psipDb.TbOneSysParameters.FirstOrDefault(x => x.Disabled == 0 &&
                                                                 x.COperationId.ToString() == cOperationID &&
                                                                 x.CFunctionId == cFunctionID.Trim() &&
                                                                 x.CCompanyId == cCompanyID.Trim() &&
                                                                 x.CNo == cNo.Trim() &&
                                                                 x.CValue == cValue.Trim());

            if (bean != null)
            {
                reValue = bean.CDescription;
            }

            return reValue;
        }
        #endregion        

        #region 取得【資訊系統參數設定檔】的所有參數值說明
        /// <summary>
        /// 取得【資訊系統參數設定檔】的參數值
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cFunctionID">功能別(SENDMAIL.寄送Mail、ACCOUNT.取得人員帳號、OTHER.其他自定義)</param>
        /// <param name="cCompanyID">公司別(ALL.全集團、T012.大世科、T016.群輝、C069.大世科技上海、T022.協志科)</param>
        /// <param name="cNo">參數No</param>
        /// <param name="cValue">參數值</param>
        /// <returns></returns>
        public List<TbOneSysParameter> findSysParameterALLDescription(string cOperationID, string cFunctionID, string cCompanyID, string cNo)
        {
            var bean = psipDb.TbOneSysParameters.Where(x => x.Disabled == 0 &&
                                                                 x.COperationId.ToString() == cOperationID &&
                                                                 x.CFunctionId == cFunctionID.Trim() &&
                                                                 x.CCompanyId == cCompanyID.Trim() &&
                                                                 x.CNo == cNo.Trim()).ToList();           

            return bean;
        }
        #endregion

        #region 服務進度查詢Distinct SRID
        /// <summary>
        /// 服務進度查詢Distinct SRID
        /// </summary>
        /// <param name="dtSource">傳入的DataTable</param>
        /// <returns></returns>
        public DataTable DistinctTable(DataTable dtSource)
        {
            DataTable dt = dtSource.Clone();
            DataTable dtDistinct = dtSource.DefaultView.ToTable(true, new string[] { "cSRID" }); //取得distinct SRID

            int count = dtDistinct.Rows.Count;

            for (int i = 0; i < count; i++)
            {                
                string tFiler = "cSRID = '" + dtDistinct.Rows[i][0].ToString() + "'";

                DataRow[] drs = dtSource.Select(tFiler);

                foreach (DataRow dr in drs)
                {
                    #region 只要塞入第一筆DataTable
                    dt.Rows.Add(dr.ItemArray);
                    #endregion

                    break;
                }
            }

            return dt;
        }
        #endregion

        #region 傳入ERPID並過濾重覆人員
        /// <summary>
        /// 傳入ERPID並過濾重覆人員
        /// </summary>
        /// <param name="tERPID">ERPID</param>
        /// <param name="tNAME">姓名</param>
        /// <param name="DicORG">傳入的Dic</param>
        public void SetDtORGPeople(string tERPID, string tNAME, ref Dictionary<string, string> DicORG)
        {
            if (!DicORG.Keys.Contains(tERPID))
            {
                DicORG.Add(tERPID, tNAME);
            }
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
        public string callAjaxSaveStockOUT(string pLoginName, string pAPIURLName, string[] AryOriSERIAL, string[] ArySERIAL, string[] AryCID, string[] AryCIDName,
                                         string[] ArySONO, string[] AryMATERIAL, string[] AryDesc)
        {
            string returnMsg = "";
            string tOriSERIAL = string.Empty;
            string tSERIAL = string.Empty;
            string tCID = string.Empty;
            string tCIDName = string.Empty;
            string tSONO = string.Empty;
            string tMATERIAL = string.Empty;
            string tDesc = string.Empty;            
            string tURL = pAPIURLName + "/API/API_STOCKOUTINFO_UPDATE";

            var client = new RestClient(tURL);

            int tCount = AryOriSERIAL.Length;

            for (int i = 0; i < tCount; i++)
            {
                try
                {
                    STOCKITEMINFO_OUTPUT OUTBean = new STOCKITEMINFO_OUTPUT();

                    tOriSERIAL = AryOriSERIAL[i];
                    tSERIAL = string.IsNullOrEmpty(ArySERIAL[i]) ? "" : ArySERIAL[i];
                    tCID = AryCID[i];
                    tCIDName = AryCIDName[i];
                    tSONO = ArySONO[i];
                    tMATERIAL = AryMATERIAL[i];
                    tDesc = AryDesc[i];

                    if (tSERIAL != "") //新序號不為空才執行
                    {
                        var request = new RestRequest();
                        request.Method = RestSharp.Method.Post;

                        Dictionary<Object, Object> parameters = new Dictionary<Object, Object>();
                        parameters.Add("IV_LOGINEMPNAME", pLoginName);
                        parameters.Add("IV_SN", tOriSERIAL);
                        parameters.Add("IV_SNNEW", tSERIAL);
                        parameters.Add("IV_MATERIALNO", tMATERIAL);
                        parameters.Add("IV_PID", tDesc);
                        parameters.Add("IV_SO", tSONO);
                        parameters.Add("IV_CUSTOMERID", tCID);
                        parameters.Add("IV_IO", "O");

                        request.AddHeader("Content-Type", "application/json");
                        request.AddHeader("X-MBX-APIKEY", "6xdTlREsMbFd0dBT28jhb5W3BNukgLOos");
                        request.AddParameter("application/json", parameters, ParameterType.RequestBody);

                        RestResponse response = client.Execute(request);

                        #region 取得回傳訊息(成功或失敗)
                        if (response.Content != null)
                        {
                            var data = (JObject)JsonConvert.DeserializeObject(response.Content);

                            OUTBean.EV_MSGT = data["EV_MSGT"].ToString().Trim();
                            OUTBean.EV_MSG = data["EV_MSG"].ToString().Trim();
                            #endregion

                            if (OUTBean.EV_MSGT == "E")
                            {
                                returnMsg += OUTBean.EV_MSG;
                            }
                            else
                            {
                                #region 新增至log
                                TbOneLog logBean = new TbOneLog
                                {
                                    CSrid = tOriSERIAL,
                                    EventName = "callAjaxSaveStockOUT",
                                    Log = "序號異動設定作業_原序號: " + tOriSERIAL + "; 新序號: " + tSERIAL,
                                    CreatedUserName = pLoginName,
                                    CreatedDate = DateTime.Now
                                };

                                dbOne.TbOneLogs.Add(logBean);
                                dbOne.SaveChanges();
                                #endregion
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    returnMsg += "出貨資料檔儲存失敗！原序號【" + tOriSERIAL + "】原因：" + ex.Message + Environment.NewLine;
                }
            }

            if (returnMsg == "")
            {
                returnMsg = "SUCCESS";
            }

            return returnMsg;
        }
        #endregion

        #region 更新進出貨資料OUTPUT資訊
        /// <summary>更新進出貨資料OUTPUT資訊</summary>
        public struct STOCKITEMINFO_OUTPUT
        {
            /// <summary>消息類型(E.處理失敗 Y.處理成功)</summary>
            public string EV_MSGT { get; set; }
            /// <summary>消息內容</summary>
            public string EV_MSG { get; set; }
        }
        #endregion

        #region 傳入語法回傳DataTable(根據資料庫名稱)
        /// <summary>
        /// 傳入語法回傳DataTable(根據資料庫名稱)
        /// </summary>
        /// <param name="tSQL">SQL語法</param>
        /// <param name="dbName">資料庫名稱(db(備品); dbBPM; dbEIP; dbProxy)</param>
        /// <returns></returns>
        public DataTable getDataTableByDb(string tSQL, string dbName)
        {
            DataTable dt = new DataTable();

            string tConnectionString = string.Empty;

            SqlConnection con = null;

            switch (dbName)
            {               
                case "dbBPM":
                    tConnectionString = bpmDB.Database.GetConnectionString();
                    break;
                case "dbEIP":                    
                    tConnectionString = dbEIP.Database.GetConnectionString();
                    break;
                case "dbProxy":                    
                    tConnectionString = dbProxy.Database.GetConnectionString();
                    break;
                case "dbPSIP":                    
                    tConnectionString = psipDb.Database.GetConnectionString();
                    break;
                case "dbOne":                    
                    tConnectionString = dbOne.Database.GetConnectionString();
                    break;
            }

            SqlCommand cmd = new SqlCommand(tSQL);
            con = new SqlConnection(tConnectionString);
            cmd.Connection = con;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.SelectCommand.CommandTimeout = 600; //設定timeout為600秒
            sda.Fill(dt);

            return dt;
        }
        #endregion

        #region log紀錄(新、舊值對照)
        /// <summary>
        /// log紀錄(新、舊值對照)
        /// </summary>
        /// <param name="tFieldName">欄位名稱</param>
        /// <param name="tOldValue">舊值</param>
        /// <param name="tNewValue">新值</param>
        /// <returns></returns>
        public string getNewAndOldLog(string tFieldName, string tOldValue , string tNewValue)
        {
            string tLog = string.Empty;

            tOldValue = string.IsNullOrEmpty(tOldValue) ? "" : tOldValue;
            tNewValue = string.IsNullOrEmpty(tNewValue) ? "" : tNewValue;

            if (tOldValue != tNewValue)
            {
                tLog = tFieldName + "_舊值【 " + tOldValue + "】 新值【 " + tNewValue + "】" + Environment.NewLine;
            }

            return tLog;
        }
        #endregion

        #region 寫log 
        /// <summary>
        /// 寫log
        /// </summary>
        /// <param name="pSRID">目前SRID</param>
        /// <param name="tMethodName">方法目錄</param>
        /// <param name="tContent">內容</param>
        /// <param name="LoginUser_Name">登入人員姓名</param>
        public void writeToLog(string? pSRID,  string tMethodName, string tContent, string LoginUser_Name)
        {
            #region 紀錄log
            TbOneLog logBean = new TbOneLog
            {
                CSrid = pSRID,
                EventName = tMethodName,
                Log = tContent,
                CreatedUserName = LoginUser_Name,
                CreatedDate = DateTime.Now
            };

            dbOne.TbOneLogs.Add(logBean);
            dbOne.SaveChanges();
            #endregion
        }
        #endregion

        #region 呼叫寄送Mail
        /// <summary>
        /// 呼叫寄送Mail
        /// </summary>
        /// <param name="tMailToTemp">收件者</param>
        /// <param name="tMailCcTemp">副本</param>
        /// <param name="tMailBCcTemp">密件副本</param>
        /// <param name="cFormNo">借用單號</param>
        /// <param name="cApplicationType">申請類型(REPAIR.備品維修 INTERNAL.內部借用)</param>
        /// <param name="cApplicationNote">申請說明</param>
        /// <param name="tNextStage">下個階段</param>
        /// <param name="tStageName">狀態說明</param>
        /// <param name="tStageName2">狀態說明2</param>
        /// <param name="cApplyUser_Name">申請人姓名</param>
        /// <param name="cFillUser_Name">填表人姓名</param>
        /// <param name="cSRCustName">客戶名稱</param>
        /// <param name="cSRNote">服務請求說明</param>
        /// <param name="cSRID">SRID</param>
        /// <param name="cSRInfo">主機訊息</param>
        /// <param name="cStartDate">內部借用(起)</param>
        /// <param name="cEndDate">內部借用(迄)</param>
        /// <param name="CreatedDate">建立時間</param>
        /// <param name="tComment">備註說明</param>
        /// <param name="tIsFinished">是否結案(Y.結案 N.未結案)</param>
        private void WSSpareSendMail(string tMailToTemp, string tMailCcTemp, string tMailBCcTemp, string cFormNo, string cApplicationType, string cApplicationNote,
                                   string tNextStage, string tStageName, string tStageName2, string cApplyUser_Name, string cFillUser_Name,
                                   string cSRCustName, string cSRNote, string cSRID, string cSRInfo, string cStartDate, string cEndDate,
                                   string CreatedDate, string tComment, string tIsFinished)
        {
            List<string> tMailToList = new List<string>();
            List<string> tMailCcList = new List<string>();
            List<string> tMailBCcList = new List<string>();

            string tMailTo = string.Empty;          //收件者            
            string tMailCc = string.Empty;          //副本            
            string tMailBCc = string.Empty;         //密件副本
            string tHypeLink = string.Empty;        //超連結
            string tSeverName = string.Empty;       //主機名稱

            //bool tIsFormal = getCallSAPERPPara(); //是否為正式區(true.是 false.不是)

            //if (tIsFormal)
            //{
            //    tSeverName = "psip-prd-ap";
            //}
            //else
            //{
            //    tSeverName = "psip-qas";
            //}

            #region 取得收件者
            if (tMailToTemp != "")
            {
                foreach (string tValue in tMailToTemp.TrimEnd(';').Split(';'))
                {
                    if (!tMailToList.Contains(tValue))
                    {
                        tMailToList.Add(tValue);

                        tMailTo += tValue + ";";
                    }
                }

                tMailTo = tMailTo.TrimEnd(';');
            }
            #endregion

            #region 取得副本
            if (tMailCcTemp != "")
            {
                foreach (string tValue in tMailCcTemp.TrimEnd(';').Split(';'))
                {
                    if (!tMailCcList.Contains(tValue))
                    {
                        tMailCcList.Add(tValue);

                        tMailCc += tValue + ";";
                    }
                }

                tMailCc = tMailCc.TrimEnd(';');
            }
            #endregion

            #region 取得密件副本
            if (tMailBCcTemp != "")
            {
                foreach (string tValue in tMailBCcTemp.TrimEnd(';').Split(';'))
                {
                    if (!tMailBCcList.Contains(tValue))
                    {
                        tMailBCcList.Add(tValue);

                        tMailBCc += tValue + ";";
                    }
                }

                tMailBCc = tMailBCc.TrimEnd(';');
            }
            #endregion

            #region 是否為測試區
            string strTest = string.Empty;

            if (!pIsFormal)
            {
                strTest = "【*測試*】";
            }
            #endregion

            #region 郵件主旨
            //備品維修
            //(待發料)備品維修_陳大明_台灣大哥大股份有限公司_8100002643
            //((狀態)借用類型_申請人_客戶_SRID)

            //內部借用
            //(待備品主管判斷備品周轉)內部借用_陳大明_20201001～20201031
            //((狀態)借用類型_申請人_借用起訖)

            string tMailSubject = string.Empty;

            if (cApplicationType == "備品維修")
            {
                tMailSubject = strTest + "(" + tStageName + ")" + cApplicationType + "_" + cApplyUser_Name + "_" + cSRCustName + "_" + cSRID;
            }
            else if (cApplicationType == "內部借用")
            {
                tMailSubject = strTest + "(" + tStageName + ")" + cApplicationType + "_" + cApplyUser_Name + "_" + cStartDate + "～" + cEndDate;
            }
            #endregion

            #region 郵件內容

            #region 內容格式參考(備品維修)                
            //備品借用單SP-20200701-0010請協助發料，謝謝。
            //[服務案件明細]
            //服務案件ID: 8100002643
            //借用人:田巧如
            //填表人:吳若華
            //建立時間: 2020/10/08 12:58:05
            //客戶名稱: 台灣大哥大股份有限公司
            //需求說明: 【網路報修】加盟店-電腦維修無法連結印表機
            //主機訊息(序號，主機P/N，主機規格/說明): SGH747T67N，OOO，DL360

            //[備品待辦清單]
            //查看待辦清單 =>超連結(http://psip-qas/Spare/Index?FormNo=SP-20200701-0010&SRID=8100002643&NowStage=3)

            //-------此信件由系統管理員發出，請勿回覆此信件-------
            #endregion

            #region 內容格式參考(內部借用)                
            //備品借用單SP-20200701-0010請協助判斷備品周轉，謝謝。
            //[服務案件明細]
            //借用人:田巧如
            //填表人:吳若華
            //建立時間: 2020/10/08 12:58:05
            //借用起訖: 20201001~20201031
            //申請說明: POC電腦維修無法連結印表機

            //[備品待辦清單]
            //查看待辦清單 =>超連結(http://psip-qas/Spare/Index?FormNo=SP-20200701-0010&NowStage=2)

            //-------此信件由系統管理員發出，請勿回覆此信件-------
            #endregion

            string tMailBody = string.Empty;

            if (tNextStage == "2" || tNextStage == "13" || tNextStage == "14") //周轉確認、借A還B、除帳
            {
                tHypeLink = "http://" + tSeverName + "/Spare/Index?FormNo=" + cFormNo + "&SRID=" + cSRID + "&NowStage=" + tNextStage;
            }
            else
            {
                if (tIsFinished == "Y")
                {
                    tHypeLink = "http://" + tSeverName + "/Spare/Index?FormNo=" + cFormNo + "&SRID=" + cSRID + "&NowStage=" + tNextStage + "&ActionType=History";
                }
                else
                {
                    tHypeLink = "http://" + tSeverName + "/Spare/ToDoList";
                }
            }

            if (cApplicationType == "備品維修")
            {
                tMailBody = GetMailBody("WSSpareREPAIR_MAIL");

                tMailBody = tMailBody.Replace("【<cFormNo>】", cFormNo).Replace("【<tStageName2>】", tStageName2).Replace("【<cSRID>】", cSRID);
                tMailBody = tMailBody.Replace("【<cApplyUser_Name>】", cApplyUser_Name).Replace("【<cFillUser_Name>】", cFillUser_Name);
                tMailBody = tMailBody.Replace("【<CreatedDate>】", CreatedDate).Replace("【<cSRCustName>】", cSRCustName).Replace("【<cSRNote>】", cSRNote);
                tMailBody = tMailBody.Replace("【<cSRInfo>】", cSRInfo).Replace("【<tNextStage>】", tNextStage).Replace("【<tComment>】", tComment).Replace("【<tHypeLink>】", tHypeLink);
            }
            else if (cApplicationType == "內部借用")
            {
                tMailBody = GetMailBody("WSSpareINTERNAL_MAIL");

                tMailBody = tMailBody.Replace("【<cFormNo>】", cFormNo).Replace("【<tStageName2>】", tStageName2);
                tMailBody = tMailBody.Replace("【<cApplyUser_Name>】", cApplyUser_Name).Replace("【<cFillUser_Name>】", cFillUser_Name);
                tMailBody = tMailBody.Replace("【<CreatedDate>】", CreatedDate).Replace("【<cStartDate>】", cStartDate).Replace("【<cEndDate>】", cEndDate);
                tMailBody = tMailBody.Replace("【<cApplicationNote>】", cApplicationNote).Replace("【<cSRInfo>】", cSRInfo).Replace("【<tNextStage>】", tNextStage);
                tMailBody = tMailBody.Replace("【<tComment>】", tComment).Replace("【<tHypeLink>】", tHypeLink);
            }
            #endregion

            //呼叫寄送Mail
            SendMailByAPI("WSSpareSend", null, tMailTo, tMailCc, tMailBCc, tMailSubject, tMailBody, "", "");

        }
        #endregion

        #region 取得Mail Body
        /// <summary>
        /// 取得Mail Body
        /// </summary>
        /// <param name="tMAIL_TYPE">MAIL TYPE</param>
        /// <returns></returns>
        public string GetMailBody(string tMAIL_TYPE)
        {
            string reValue = string.Empty;

            var bean = dbProxy.TbMailContents.FirstOrDefault(x => x.MailType == tMAIL_TYPE);            

            if (bean != null)
            {
                reValue = bean.MailContent;
            }

            return reValue;
        }
        #endregion

        #region 以訊息中心發送Mail(新版)
        /// <summary>
        /// Email寄送 API
        /// </summary>
        /// <param name="eventName">事件名稱 </param>
        /// <param name="sender">設定寄件者：如為空或 null，則預設用 IC@etatung.com為寄件者 </param>
        /// <param name="recipients">收件者：用 ;分隔 </param>
        /// <param name="ccs">副本：用 ;分隔。如果沒有，就給空值或 null</param>
        /// <param name="bccs">密碼副本：用 ;分隔。如果沒有，就給空值或 null</param>
        /// <param name="subject">標題 </param>
        /// <param name="content">內容 </param>
        /// <param name="attachFileNames">附檔檔名：用 ;分隔 (※項目必需跟附檔路徑匹配 )。如果沒有，就給空值或 null</param>
        /// <param name="attachFilePaths">附檔路徑：用 ;分隔 (※項目必需跟附檔檔名匹配 )。如果沒有，就給空值或 null</param>
        public void SendMailByAPI(string eventName, string sender, string recipients, string ccs, string bccs, string subject, string content, string attachFileNames, string attachFilePaths)
        {
            WebRequest browser = WebRequest.Create("http://psip-prd-ap:8080/Ajax/SendMailAPI");
            browser.Method = "POST";
            browser.ContentType = "application/x-www-form-urlencoded";

            //附檔轉換成附檔轉換成base64
            List<string> attachFileBase64s = new List<string>();
            if (!string.IsNullOrEmpty(attachFilePaths))
            {
                var _attachFilePaths = attachFilePaths.Split(';');
                foreach (var attachFilePath in _attachFilePaths)
                {
                    attachFileBase64s.Add(Convert.ToBase64String(System.IO.File.ReadAllBytes(attachFilePath)));
                }
            }

            NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
            postParams.Add("eventName", eventName);
            postParams.Add("sender", sender);
            postParams.Add("recipients", recipients);
            postParams.Add("ccs", ccs);
            postParams.Add("bccs", bccs);
            postParams.Add("subject", subject);
            postParams.Add("content", content);
            postParams.Add("attachFileNames", attachFileNames);
            postParams.Add("attachFileBase64s", string.Join(";", attachFileBase64s));

            //要發送的字串轉為要發送的字串轉為byte[]
            byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());
            using (Stream reqStream = browser.GetRequestStream())
            {
                reqStream.Write(byteArray, 0, byteArray.Length);
            }//end using

            //API回傳的字串回傳的字串
            string responseStr = "";

            //發出發出Request
            using (WebResponse response = browser.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = sr.ReadToEnd();
                }//end using
            }

            System.Diagnostics.Debug.WriteLine(responseStr);
        }
        #endregion

        #endregion -----↑↑↑↑↑共用方法 ↑↑↑↑↑-----

        #region -----↓↓↓↓↓匯入Excel ↓↓↓↓↓-----

        #region 匯入Excel並轉換成DataTable
        /// <summary>
        /// 匯入Excel並轉換成DataTable
        /// </summary>
        /// <param name="postedFile">傳入的Excel</param>
        /// <param name="tSheetName">傳入的頁籤名稱</param>
        /// <returns>回傳的result若為空白代表執行成功，若非空白代表執行失敗並回傳錯誤訊息</returns>
        public Dictionary<string, DataTable> ImportExcelToDataTable(IFormFile postedFile, string tSheetName)
        {
            Dictionary<string, DataTable> Dic = new Dictionary<string, DataTable>();

            string result = string.Empty;

            DataTable dt = new DataTable();
           
            if (postedFile.Length > 0)
            {
                try
                {
                    XSSFWorkbook workbook;

                    using (var ms = new MemoryStream())
                    {
                        postedFile.CopyTo(ms);

                        Stream stream = new MemoryStream(ms.ToArray());
                        workbook = new XSSFWorkbook(stream);
                    }

                    #region 讀取內容
                    // 0表示：第一個 worksheet工作表
                    XSSFSheet u_sheet = (XSSFSheet)workbook.GetSheetAt(0);

                    //-- Excel 表頭列
                    XSSFRow headerRow = (XSSFRow)u_sheet.GetRow(0);

                    if (u_sheet.SheetName == tSheetName)
                    {
                        // 表頭列，共有幾個 "欄位"?（取得最後一欄的數字）                                                                     
                        for (int k = headerRow.FirstCellNum; k < headerRow.LastCellNum; k++)
                        {   // 把上傳的 Excel「表頭列」，每一欄位抓到
                            if (headerRow.GetCell(k) != null)
                            {
                                dt.Columns.Add(new DataColumn(headerRow.GetCell(k).StringCellValue));
                            }
                        }

                        // for迴圈的「啟始值」要加一，表示不包含 Excel表頭列
                        for (int i = (u_sheet.FirstRowNum + 1); i <= u_sheet.LastRowNum; i++)
                        {   // 每一列做迴圈
                            XSSFRow row = (XSSFRow)u_sheet.GetRow(i);  //不包含 Excel表頭列的 "其他資料列"

                            if (row == null) continue;

                            DataRow dataRow = dt.NewRow();

                            #region 跑明細Loop
                            for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                            {
                                ICell cell = row.GetCell(j);

                                //-- 每一個欄位（行）做迴圈
                                if (cell != null)
                                {
                                    switch (cell.CellType)
                                    {
                                        case CellType.Numeric:
                                            dataRow[j] = cell.NumericCellValue;
                                            break;

                                        default:
                                            dataRow[j] = cell.StringCellValue.Trim();
                                            break;
                                    }
                                }
                            }

                            dt.Rows.Add(dataRow);
                            #endregion
                        }
                    }
                    else
                    {
                        result = "請確認傳入的Excel頁籤是否為【" + tSheetName + "】？";
                    }
                }
                catch(Exception ex)
                {
                    result = ex.Message;
                }
                finally
                {
                    Dic.Add(result, dt);
                }
            }
            #endregion

            return Dic;
        }
        #endregion

        #endregion -----↑↑↑↑↑匯入Excel ↑↑↑↑↑-----
    }
}
