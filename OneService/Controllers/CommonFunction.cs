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
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cCompanyID">公司別</param>
        /// <param name="IsManager">true.管理員 false.非管理員</param>
        /// <param name="tERPID">登入人員ERPID</param>
        /// <param name="tTeamList">可觀看服務團隊清單</param>
        /// <param name="tType">61.一般服務 63.裝機服務...</param>
        /// <returns></returns>
        public List<string[]> findSRIDList(string cOperationID, string cCompanyID, bool IsManager, string tERPID, List<string> tTeamList, string tType)
        {            

            List<string[]> SRIDUserToList = new List<string[]>();   //組SRID清單

            switch(tType)
            {
                case "61":  //一般服務
                    SRIDUserToList = getSRIDLis_Generally(cOperationID, cCompanyID, IsManager, tERPID, tTeamList);
                    break;

                case "63":  //裝機服務

                    break;

                case "65":  //定維服務

                    break;
            }

            return SRIDUserToList;
        }
        #endregion

        #region 取得一般服務SRID負責清單
        /// <summary>
        /// 取得一般服務SRID負責清單
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cCompanyID">公司別</param>
        /// <param name="IsManager">true.管理員 false.非管理員</param>
        /// <param name="tERPID">登入人員ERPID</param>
        /// <param name="tTeamList">可觀看服務團隊清單</param>
        /// <returns></returns>
        private List<string[]> getSRIDLis_Generally(string cOperationID, string cCompanyID, bool IsManager, string tERPID, List<string> tTeamList)
        {
            List<string[]> SRIDUserToList = new List<string[]>();   //組SRID清單

            string tSRPathWay = string.Empty;           //報修管理
            string tSRType = string.Empty;              //報修類別
            string tMainEngineerID = string.Empty;      //L2工程師ERPID
            string tMainEngineerName = string.Empty;    //L2工程師姓名            
            string cTechManagerID = string.Empty;      //技術主管ERPID            
            string tModifiedDate = string.Empty;        //修改日期

            List<TbOneSrmain> beans = new List<TbOneSrmain>();
            List<TbOneSysParameter> tSRPathWay_List = findSysParameterALLDescription(cOperationID, "OTHER", cCompanyID, "SRPATH");            

            if (IsManager)
            {
                string tWhere = TrnasTeamListToWhere(tTeamList);

                string tSQL = @"select * from TB_ONE_SRMain
                                   where 
                                   (cStatus <> 'E0015' and cStatus <> 'E0006') and 
                                   (
                                        (
                                            (CMainEngineerId = '{0}') or (cTechManagerID like '%{0}%')
                                        )
                                        {1}
                                   )";

                tSQL = string.Format(tSQL, tERPID, tWhere);

                DataTable dt = getDataTableByDb(tSQL, "dbOne");

                foreach (DataRow dr in dt.Rows)
                {
                    tSRPathWay = TransSysParameterByList(tSRPathWay_List, dr["cSRPathWay"].ToString());
                    tSRType = TransSRType(dr["cSRTypeOne"].ToString(), dr["cSRTypeSec"].ToString(), dr["cSRTypeThr"].ToString());
                    tMainEngineerID = dr["cMainEngineerID"].ToString();
                    tMainEngineerName = dr["cMainEngineerName"].ToString();
                    cTechManagerID = dr["cTechManagerID"].ToString();                    
                    tModifiedDate = dr["ModifiedDate"].ToString() != "" ? Convert.ToDateTime(dr["ModifiedDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss") : "";

                    #region 組待處理服務
                    string[] ProcessInfo = new string[11];

                    ProcessInfo[0] = dr["cSRID"].ToString();             //SRID
                    ProcessInfo[1] = dr["cCustomerName"].ToString();      //客戶
                    ProcessInfo[2] = dr["cRepairName"].ToString();        //客戶報修人
                    ProcessInfo[3] = dr["cDesc"].ToString();             //說明
                    ProcessInfo[4] = tSRPathWay;                        //報修管道
                    ProcessInfo[5] = tSRType;                           //報修類別
                    ProcessInfo[6] = tMainEngineerID;                   //L2工程師ERPID
                    ProcessInfo[7] = tMainEngineerName;                 //L2工程師姓名
                    ProcessInfo[8] = cTechManagerID;                    //技術主管ERPID                    
                    ProcessInfo[9] = tModifiedDate;                     //最後編輯日期
                    ProcessInfo[10] = dr["cStatus"].ToString();           //狀態

                    SRIDUserToList.Add(ProcessInfo);
                    #endregion
                }
            }
            else
            {
                beans = dbOne.TbOneSrmains.Where(x => (x.CStatus != "E0015" && x.CStatus != "E0006") && (x.CMainEngineerId == tERPID || x.CTechManagerId.Contains(tERPID) || x.CAssEngineerId.Contains(tERPID))).ToList();

                foreach (var bean in beans)
                {
                    tSRPathWay = TransSysParameterByList(tSRPathWay_List, bean.CSrpathWay);                    
                    tSRType = TransSRType(bean.CSrtypeOne, bean.CSrtypeSec, bean.CSrtypeThr);
                    tMainEngineerID = string.IsNullOrEmpty(bean.CMainEngineerId) ? "" : bean.CMainEngineerId;
                    tMainEngineerName = string.IsNullOrEmpty(bean.CMainEngineerName) ? "" : bean.CMainEngineerName;
                    cTechManagerID = string.IsNullOrEmpty(bean.CTechManagerId) ? "" : bean.CTechManagerId;                    
                    tModifiedDate = bean.ModifiedDate == null ? "" : Convert.ToDateTime(bean.ModifiedDate).ToString("yyyy-MM-dd HH:mm:ss");

                    #region 組待處理服務
                    string[] ProcessInfo = new string[11];

                    ProcessInfo[0] = bean.CSrid;            //SRID
                    ProcessInfo[1] = bean.CCustomerName;     //客戶
                    ProcessInfo[2] = bean.CRepairName;       //客戶報修人
                    ProcessInfo[3] = bean.CDesc;            //說明
                    ProcessInfo[4] = tSRPathWay;           //報修管道
                    ProcessInfo[5] = tSRType;              //報修類別
                    ProcessInfo[6] = tMainEngineerID;      //L2工程師ERPID
                    ProcessInfo[7] = tMainEngineerName;    //L2工程師姓名
                    ProcessInfo[8] = cTechManagerID;       //技術主管ERPID                    
                    ProcessInfo[9] = tModifiedDate;        //最後編輯日期
                    ProcessInfo[10] = bean.CStatus;         //狀態

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

        #region 取得狀態值說明
        /// <summary>
        /// 取得報修管道參數值說明
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cCompanyID">公司別</param>
        /// <param name="cSTATUS">狀態ID</param>
        /// <returns></returns>
        public string TransSRSTATUS(string cOperationID, string cCompanyID, string cSTATUS)
        {
            string tValue = findSysParameterDescription(cOperationID, "OTHER", cCompanyID, "SRSTATUS", cSTATUS);

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

            List<TbOneSrteamMapping> TeamList = new List<TbOneSrteamMapping>();
            
            if (pCompanyCode == "T016")
            {
                TeamList = dbOne.TbOneSrteamMappings.OrderBy(x => x.CTeamOldId).Where(x => x.Disabled == 0 && (x.CTeamOldId.Contains("SRV.1217") || x.CTeamOldId.Contains("SRV.1227") || x.CTeamOldId.Contains("SRV.1237"))).ToList();
            }
            else
            {
                TeamList = dbOne.TbOneSrteamMappings.OrderBy(x => x.CTeamOldId).Where(x => x.Disabled == 0 && !(x.CTeamOldId.Contains("SRV.1217") || x.CTeamOldId.Contains("SRV.1227") || x.CTeamOldId.Contains("SRV.1237"))).ToList();
            }

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
                        var request = new RestRequest();
                        request.Method = RestSharp.Method.Post;

                        Dictionary<Object, Object> parameters = new Dictionary<Object, Object>();
                        parameters.Add("SAP_FUNCTION_NAME", "ZFM_TICC_SERIAL_SEARCH");
                        parameters.Add("IV_SERIAL", IV_SERIAL);

                        request.AddHeader("Content-Type", "application/json");
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

        #region call ONE SERVICE（一般服務案件）狀態更新接口
        /// <summary>
        /// call ONE SERVICE（一般服務案件）狀態更新接口
        /// </summary>
        /// <param name="beanIN"></param>
        public SRMain_GENERALSRSTATUS_OUTPUT GetAPI_GenerallySRSTATUS_Update(SRMain_GENERALSRSTATUS_INPUT beanIN)
        {
            SRMain_GENERALSRSTATUS_OUTPUT OUTBean = new SRMain_GENERALSRSTATUS_OUTPUT();

            string pMsg = string.Empty;

            try
            {
                string tURL = beanIN.IV_APIURLName + "/API/API_GENERALSRSTATUS_UPDATE";

                var client = new RestClient(tURL);

                var request = new RestRequest();
                request.Method = RestSharp.Method.Post;

                Dictionary<Object, Object> parameters = new Dictionary<Object, Object>();
                parameters.Add("IV_LOGINEMPNO", beanIN.IV_LOGINEMPNO);
                parameters.Add("IV_SRID", beanIN.IV_SRID);
                parameters.Add("IV_STATUS", beanIN.IV_STATUS);                

                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", parameters, ParameterType.RequestBody);

                RestResponse response = client.Execute(request);

                #region 取得回傳訊息(成功或失敗)
                var data = (JObject)JsonConvert.DeserializeObject(response.Content);

                OUTBean.EV_MSGT = data["EV_MSGT"].ToString().Trim();
                OUTBean.EV_MSG = data["EV_MSG"].ToString().Trim();
                #endregion

                if (OUTBean.EV_MSGT == "Y")
                {
                    pMsg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE（一般服務案件）狀態更新接口成功";                    
                }
                else
                {
                    pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE（一般服務案件）狀態更新接口失敗，原因:" + OUTBean.EV_MSG;                    
                }               
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "呼叫ONE SERVICE（一般服務案件）狀態更新接口失敗，原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();                
            }

            writeToLog(beanIN.IV_SRID, "GetAPI_GenerallySRSTATUS_Update", pMsg, beanIN.IV_LOGINEMPNAME);

            return OUTBean;
        }
        #endregion

        #endregion -----↑↑↑↑↑一般服務 ↑↑↑↑↑-----

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
                                                              x.ContactAddress != "" && x.ContactEmail != "" &&                                                              
                                                              x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == CustomerID).ToList();

                if (qPjRec != null && qPjRec.Count() > 0)
                {
                    foreach (var prBean in qPjRec)
                    {
                        tTempValue = prBean.Kna1Kunnr.Trim().Replace(" ", "") + "|" + cBUKRS + "|" + prBean.ContactEmail.Trim().Replace(" ", "");

                        if (!tTempList.Contains(tTempValue)) //判斷客戶ID、公司別、聯絡人名姓名不重覆才要顯示
                        {
                            tTempList.Add(tTempValue);

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
                                                          x.ContactAddress != "" && x.ContactPhone != "" && x.ContactEmail != "" &&
                                                          x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == CustomerID && x.ContactName.Contains(ContactName)).ToList();

            List<string> tTempList = new List<string>();

            string tTempValue = string.Empty;
            string ContactMobile = string.Empty;

            List<PCustomerContact> liPCContact = new List<PCustomerContact>();
            if (qPjRec != null && qPjRec.Count() > 0)
            {
                foreach (var prBean in qPjRec)
                {
                    tTempValue = prBean.Kna1Kunnr.Trim().Replace(" ", "") + "|" + cBUKRS + "|" + prBean.ContactEmail.Trim().Replace(" ", "");

                    if (!tTempList.Contains(tTempValue)) //判斷客戶ID、公司別、聯絡人名姓名不重覆才要顯示
                    {
                        tTempList.Add(tTempValue);

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
                        prDocBean.BPMNo = prBean.BpmNo.Trim().Replace(" ", "");

                        liPCContact.Add(prDocBean);
                    }
                }
            }

            return liPCContact;
        }

        /// <summary>客戶聯絡人</summary>
        public struct PCustomerContact
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
            public string Mobile{ get; set; }

        /// <summary>來源表單</summary>
        public string BPMNo { get; set; }
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

            var bean = psipDb.TbOneSysParameters.FirstOrDefault(x => x.COperationId == tcID && x.CFunctionId == "ACCOUNT" && x.CCompanyId == "ALL" && x.CNo == "MIS");

            if (bean != null)
            {
                if ( bean.CValue.ToLower() == LoginAccount.ToLower())
                {
                    reValue = true;                    
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

            var bean = psipDb.TbOneSysParameters.FirstOrDefault(x => x.COperationId == tcID && x.CFunctionId == "ACCOUNT" && x.CCompanyId == "ALL" && x.CNo == "CUSTOMERSERVICE");

            if (bean != null)
            {
                if (bean.CValue.ToLower() == LoginAccount.ToLower())
                {
                    reValue = true;
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

            //服務團隊主管、L2工程師、指派工程師、技術主管
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
                    #region L2工程師可編輯
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
                    else //非L2處理中狀態(指派工程師都可以編輯)
                    {
                        #region 指派工程師
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
        /// <param name="keyword">料號/料號說明關鍵字</param>
        /// <returns></returns>
        public Object findMaterialByKeyWords(string keyword)
        {
            Object contentObj = dbProxy.ViewMaterialByComps.Where(x => x.MaraMatnr.Contains(keyword) || x.MaktTxza1Zf.Contains(keyword)).Take(8);

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

                    if (tOriSERIAL != "") //原序號不為空才執行
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
    }
}
