using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneService.Models;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Security.Principal;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

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

        #region -----↓↓↓↓↓一般服務請求 ↓↓↓↓↓-----        

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
        /// <returns></returns>
        public List<SelectListItem> findSRTeamIDList()
        {
            List<string> tTempList = new List<string>();

            string tKEY = string.Empty;
            string tNAME = string.Empty;

            var beans = dbOne.TbOneSrteamMappings.OrderBy(x => x.CTeamOldId).Where(x => x.Disabled == 0);

            var tList = new List<SelectListItem>();
            tList.Add(new SelectListItem { Text = "請選擇", Value = "" });

            foreach (var bean in beans)
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

        #endregion -----↑↑↑↑↑一般服務請求 ↑↑↑↑↑-----

        #region -----↓↓↓↓↓共用方法 ↓↓↓↓↓-----

        #region 取得登入者資訊
        public EmployeeBean findEmployeeInfo(string keyword)
        {
            EmployeeBean empBean = new EmployeeBean();

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

                var beanD = dbEIP.Departments.FirstOrDefault(x => x.Id == beanE.DeptId);

                if (beanD != null)
                {
                    empBean.DepartmentNO = beanD.Id.Trim();
                    empBean.DepartmentName = beanD.Name2.Trim();
                }
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
            /// <summary>人員Email</summary>
            public string EmployeeEmail { get; set; }
            /// <summary>人員ID(Person資料表ID)</summary>
            public string EmployeePersonID { get; set; }
        }
        #endregion

        #endregion

        #region 查詢客戶資料By公司別
        /// <summary>
        /// 查詢客戶資料By公司別
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="compcde">公司別</param>
        /// <returns></returns>
        public IQueryable<ViewCustomer2> findCustByKeywordAndComp(string keyword, string compcde)
        {
            return dbProxy.ViewCustomer2s.Where(x => x.KnvvVkorg == compcde &&
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
            var qPjRec = dbProxy.CustomerContacts.OrderByDescending(x => x.ModifiedDate).
                                               Where(x => (x.Disabled == null || x.Disabled != 1) && x.ContactName != "" && x.ContactCity != "" &&
                                                          x.ContactAddress != "" && x.ContactPhone != "" &&
                                                          x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == CustomerID).ToList();

            List<string> tTempList = new List<string>();

            string tTempValue = string.Empty;

            List<PCustomerContact> liPCContact = new List<PCustomerContact>();
            if (qPjRec != null && qPjRec.Count() > 0)
            {
                foreach (var prBean in qPjRec)
                {
                    tTempValue = prBean.Kna1Kunnr.Trim().Replace(" ", "") + "|" + cBUKRS + "|" + prBean.ContactName.Trim().Replace(" ", "");

                    if (!tTempList.Contains(tTempValue)) //判斷客戶ID、公司別、聯絡人名姓名不重覆才要顯示
                    {
                        tTempList.Add(tTempValue);

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

            /// <summary>來源表單</summary>
            public string BPMNo { get; set; }
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

            var beans = psipDb.TbOneSysParameters.OrderBy(x => x.COperationId).OrderBy(x => x.CFunctionId).OrderBy(x => x.CCompanyId).OrderBy(x => x.CNo).
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

        #region 寫log 
        /// <summary>
        /// 寫log
        /// </summary>
        /// <param name="tContent">內容</param>
        /// <param name="tMethodName">方法目錄</param>
        public void writeToLog(string tContent, string tMethodName)
        {
            string filePath = "C://ONEServiceLogs//" + tMethodName + "//";
            if (!(Directory.Exists(filePath)))
            {
                Directory.CreateDirectory(filePath);
            }
            // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + DateTime.Now.ToString("yyyyMMddHH") + ".txt", true);
            file.WriteLine(tContent);
            file.Close();
        }
        #endregion

        #endregion -----↑↑↑↑↑共用方法 ↑↑↑↑↑-----
    }
}
