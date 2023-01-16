using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneService.Models;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Policy;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;


namespace OneService.Controllers
{
    public class ServiceRequestController : Controller
    {
        /// <summary>
        /// 登入者帳號
        /// </summary>
        //string pLoginAccount = string.Empty;
        //string pLoginAccount = @"etatung\elvis.chang";  //MIS
        //string pLoginAccount = @"etatung\Allen.Chen";    //陳勁嘉(主管)
        string pLoginAccount = @"etatung\Boyen.Chen";    //陳建良(主管)
        //string pLoginAccount = @"etatung\Aniki.Huang";    //黃志豪(主管)
        //string pLoginAccount = @"etatung\jack.hung";      //洪佑典(主管)
        //string pLoginAccount = @"etatung\Wenjui.Chan";    //詹文瑞

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
        /// 登入者是否為管理者(true.是 false.否)
        /// </summary>
        bool pIsManager = false;

        /// <summary>
        /// 登入者是否為負責工程師(含主要和協助)
        /// </summary>
        bool pIsExeEngineer = false;

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
        /// 公司別(T012、T016、C069、T022)
        /// </summary>
        static string pCompanyCode = string.Empty;

        CommonFunction CMF = new CommonFunction();

        PSIPContext psipDb = new PSIPContext();
        TSTIONEContext dbOne = new TSTIONEContext();
        TAIFContext bpmDB = new TAIFContext();
        ERP_PROXY_DBContext dbProxy = new ERP_PROXY_DBContext();
        MCSWorkflowContext dbEIP = new MCSWorkflowContext();

        #region -----↓↓↓↓↓待辦清單 ↓↓↓↓↓-----
        public IActionResult ToDoList()
        {
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
                List<string[]> SRIDList_GenerallySR = CMF.findSRIDList(pOperationID_GenerallySR, pCompanyCode, pIsManager, EmpBean.EmployeeERPID, tSRTeamList, "61");

                ViewBag.SRIDList_GenerallySR = SRIDList_GenerallySR;
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
            var SRTeamIDList = CMF.findSRTeamIDList(pCompanyCode);           
            #endregion

            #region 取得SRID
            var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pSRID);

            if (beanM != null)
            {
                //記錄目前GUID，用來判斷更新的先後順序
                ViewBag.pGUID = beanM.CSystemGuid.ToString();

                #region 報修資訊
                ViewBag.cSRID = beanM.CSrid;
                ViewBag.cCustomerID = beanM.CCustomerId;
                ViewBag.cCustomerName = beanM.CCustomerName;
                ViewBag.cRepairName = beanM.CRepairName;
                ViewBag.cDesc = beanM.CDesc;
                ViewBag.cNotes = beanM.CNotes;
                ViewBag.cSRPathWay = beanM.CSrpathWay;
                ViewBag.cMAServiceType = beanM.CMaserviceType;
                ViewBag.cSRProcessWay = beanM.CSrprocessWay;
                ViewBag.cIsSecondFix = beanM.CIsSecondFix;

                ViewBag.pStatus = beanM.CStatus;
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

                #region 客戶聯絡窗口資訊
                ViewBag.cContacterName = beanM.CContacterName;
                ViewBag.cContactAddress = beanM.CContactAddress;
                ViewBag.cContactPhone = beanM.CContactPhone;
                ViewBag.cContactEmail = beanM.CContactEmail;
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
                #endregion
                
                ViewBag.CreatedDate = Convert.ToDateTime(beanM.CreatedDate).ToString("yyyy-MM-dd");

                #region 取得產品序號資訊(明細)
                var beansProduct = dbOne.TbOneSrdetailProducts.OrderBy(x => x.CSerialId).Where(x => x.Disabled == 0 && x.CSrid == pSRID);
                
                ViewBag.Detailbean_Product = beansProduct;
                ViewBag.trProductNo = beansProduct.Count();
                #endregion

                #region 取得工時紀錄檔資訊(明細)
                var beansRecord = dbOne.TbOneSrdetailRecords.OrderBy(x => x.CEngineerId).ThenByDescending(x => x.CFinishTime).Where(x => x.Disabled == 0 && x.CSrid == pSRID);
                
                ViewBag.Detailbean_Record = beansRecord;
                ViewBag.trRecordNo = beansRecord.Count();
                #endregion

                #region 取得零件更換資訊(明細)
                var beansParts = dbOne.TbOneSrdetailPartsReplaces.OrderBy(x => x.CMaterialId).Where(x => x.Disabled == 0 && x.CSrid == pSRID);
                
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
                ViewBag.cIsSecondFix = "";     //請選擇
            }
            #endregion

            #region 指派Option值
            model.ddl_cStatus = ViewBag.pStatus;                //設定狀態
            model.ddl_cSRPathWay = ViewBag.cSRPathWay;          //設定報修管道
            model.ddl_cMAServiceType = ViewBag.cMAServiceType;   //設定維護服務種類
            model.ddl_cSRProcessWay = ViewBag.cSRProcessWay;    //設定處理方式
            model.ddl_cIsSecondFix = ViewBag.cIsSecondFix;      //是否為二修
            #endregion

            ViewBag.SRTypeOneList = SRTypeOneList;
            ViewBag.SRTypeSecList = SRTypeSecList;
            ViewBag.SRTypeThrList = SRTypeThrList;            
            ViewBag.SRTeamIDList = SRTeamIDList;

            ViewBag.pOperationID = pOperationID_GenerallySR;

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

            string OldCStatus = string.Empty; 
            string CStatus = formCollection["ddl_cStatus"].FirstOrDefault();
            string CCustomerName = formCollection["tbx_cCustomerName"].FirstOrDefault();
            string CCustomerId = formCollection["hid_cCustomerID"].FirstOrDefault();
            string CRepairName = formCollection["tbx_cRepairName"].FirstOrDefault();
            string CDesc = formCollection["tbx_cDesc"].FirstOrDefault();
            string CNotes = formCollection["tbx_cNotes"].FirstOrDefault();
            string CMaserviceType = formCollection["ddl_cMAServiceType"].FirstOrDefault();
            string CSrtypeOne = formCollection["ddl_cSRTypeOne"].FirstOrDefault();
            string CSrtypeSec = formCollection["ddl_cSRTypeSec"].FirstOrDefault();
            string CSrtypeThr = formCollection["ddl_cSRTypeThr"].FirstOrDefault();
            string CSrpathWay = formCollection["ddl_cSRPathWay"].FirstOrDefault();
            string CSrprocessWay = formCollection["ddl_cSRProcessWay"].FirstOrDefault();
            string CIsSecondFix = formCollection["ddl_cIsSecondFix"].FirstOrDefault();
            string CContacterName = formCollection["tbx_cContacterName"].FirstOrDefault();
            string CContactAddress = formCollection["tbx_cContactAddress"].FirstOrDefault();
            string CContactPhone = formCollection["tbx_cContactPhone"].FirstOrDefault();
            string CContactEmail = formCollection["tbx_cContactEmail"].FirstOrDefault();
            string CTeamId = formCollection["hid_cTeamID"].FirstOrDefault();
            string CSqpersonId = formCollection["hid_cSQPersonID"].FirstOrDefault();
            string CSqpersonName = formCollection["tbx_cSQPersonName"].FirstOrDefault();
            string CSalesName = formCollection["tbx_cSalesName"].FirstOrDefault();
            string CSalesId = formCollection["hid_cSalesID"].FirstOrDefault();
            string CMainEngineerName = formCollection["tbx_cMainEngineerName"].FirstOrDefault();
            string CMainEngineerId = formCollection["hid_cMainEngineerID"].FirstOrDefault();
            string CAssEngineerId = formCollection["hid_cAssEngineerID"].FirstOrDefault();            
            string LoginUser_Name = formCollection["hid_cLoginUser_Name"].FirstOrDefault();

            try
            {
                var beanNowM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pSRID);

                if (beanNowM == null)
                {
                    #region 新增主檔
                    TbOneSrmain beanM = new TbOneSrmain();

                    //主表資料
                    beanM.CSrid = pSRID;
                    beanM.CStatus = "E0005";    //新增時先預設為L3.處理中
                    beanM.CCustomerName = CCustomerName;
                    beanM.CCustomerId = CCustomerId;
                    beanM.CRepairName = CRepairName;
                    beanM.CDesc = CDesc;
                    beanM.CNotes = CNotes;
                    beanM.CMaserviceType = CMaserviceType;
                    beanM.CSrtypeOne = CSrtypeOne;
                    beanM.CSrtypeSec = CSrtypeSec;
                    beanM.CSrtypeThr = CSrtypeThr;
                    beanM.CSrpathWay = CSrpathWay;
                    beanM.CSrprocessWay = CSrprocessWay;
                    beanM.CIsSecondFix = CIsSecondFix;
                    beanM.CContacterName = CContacterName;
                    beanM.CContactAddress = CContactAddress;
                    beanM.CContactPhone = CContactPhone;
                    beanM.CContactEmail = CContactEmail;
                    beanM.CTeamId = CTeamId;
                    beanM.CSqpersonId = CSqpersonId;
                    beanM.CSqpersonName = CSqpersonName;
                    beanM.CSalesName = CSalesName;
                    beanM.CSalesId = CSalesId;
                    beanM.CMainEngineerName = CMainEngineerName;
                    beanM.CMainEngineerId = CMainEngineerId;
                    beanM.CAssEngineerId = CAssEngineerId;
                    beanM.CSystemGuid = Guid.NewGuid();

                    beanM.CreatedDate = DateTime.Now;
                    beanM.CreatedUserName = LoginUser_Name;

                    dbOne.TbOneSrmains.Add(beanM);
                    #endregion

                    #region 新增【產品序號資訊】明細
                    string[] PRcSerialID = formCollection["tbx_PRcSerialID"];
                    string[] PRcMaterialID = formCollection["tbx_PRcMaterialID"];
                    string[] PRcMaterialName = formCollection["tbx_PRcMaterialName"];
                    string[] PRcProductNumber = formCollection["tbx_PRcProductNumber"];
                    string[] PRcInstallID = formCollection["tbx_PRcInstallID"];

                    int countPR = PRcSerialID.Length;

                    for (int i = 0; i < countPR; i++)
                    {
                        TbOneSrdetailProduct beanD = new TbOneSrdetailProduct();

                        beanD.CSrid = pSRID;
                        beanD.CSerialId = PRcSerialID[i];
                        beanD.CMaterialId = PRcMaterialID[i];
                        beanD.CMaterialName = PRcMaterialName[i];
                        beanD.CProductNumber = PRcProductNumber[i];
                        beanD.CInstallId = PRcInstallID[i];
                        beanD.Disabled = 0;

                        beanD.CreatedDate = DateTime.Now;
                        beanD.CreatedUserName = LoginUser_Name;

                        dbOne.TbOneSrdetailProducts.Add(beanD);
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
                            Log = "SR狀態_舊值: " + OldCStatus + "; 新值: " + CStatus,
                            CreatedUserName = LoginUser_Name,
                            CreatedDate = DateTime.Now
                        };

                        dbOne.TbOneLogs.Add(logBean);
                        dbOne.SaveChanges();
                        #endregion
                    }
                }
                else
                {
                    #region 修改主檔                    
                    //主表資料
                    OldCStatus = beanNowM.CStatus;

                    beanNowM.CStatus = CStatus;
                    beanNowM.CCustomerName = CCustomerName;
                    beanNowM.CCustomerId = CCustomerId;
                    beanNowM.CRepairName = CRepairName;
                    beanNowM.CDesc = CDesc;
                    beanNowM.CNotes = CNotes;
                    beanNowM.CMaserviceType = CMaserviceType;
                    beanNowM.CSrtypeOne = CSrtypeOne;
                    beanNowM.CSrtypeSec = CSrtypeSec;
                    beanNowM.CSrtypeThr = CSrtypeThr;
                    beanNowM.CSrpathWay = CSrpathWay;
                    beanNowM.CSrprocessWay = CSrprocessWay;
                    beanNowM.CIsSecondFix = CIsSecondFix;
                    beanNowM.CContacterName = CContacterName;
                    beanNowM.CContactAddress = CContactAddress;
                    beanNowM.CContactPhone = CContactPhone;
                    beanNowM.CContactEmail = CContactEmail;
                    beanNowM.CTeamId = CTeamId;
                    beanNowM.CSqpersonId = CSqpersonId;
                    beanNowM.CSqpersonName = CSqpersonName;
                    beanNowM.CSalesName = CSalesName;
                    beanNowM.CSalesId = CSalesId;
                    beanNowM.CMainEngineerName = CMainEngineerName;
                    beanNowM.CMainEngineerId = CMainEngineerId;
                    beanNowM.CAssEngineerId = CAssEngineerId;
                    beanNowM.CSystemGuid = Guid.NewGuid();

                    beanNowM.ModifiedDate = DateTime.Now;
                    beanNowM.ModifiedUserName = LoginUser_Name;
                    #endregion

                    #region -----↓↓↓↓↓產品序號資訊↓↓↓↓↓-----

                    #region 刪除明細                    
                    dbOne.TbOneSrdetailProducts.RemoveRange(dbOne.TbOneSrdetailProducts.Where(x => x.Disabled == 0 && x.CSrid == pSRID));
                    #endregion

                    #region 新增明細
                    string[] PRcSerialID = formCollection["tbx_PRcSerialID"];
                    string[] PRcMaterialID = formCollection["tbx_PRcMaterialID"];
                    string[] PRcMaterialName = formCollection["tbx_PRcMaterialName"];
                    string[] PRcProductNumber = formCollection["tbx_PRcProductNumber"];
                    string[] PRcInstallID = formCollection["tbx_PRcInstallID"];
                    string[] PRcDisabled = formCollection["hid_PRcDisabled"];

                    int countPR = PRcSerialID.Length;

                    for (int i = 0; i < countPR; i++)
                    {
                        TbOneSrdetailProduct beanD = new TbOneSrdetailProduct();

                        beanD.CSrid = pSRID;
                        beanD.CSerialId = PRcSerialID[i];
                        beanD.CMaterialId = PRcMaterialID[i];
                        beanD.CMaterialName = PRcMaterialName[i];
                        beanD.CProductNumber = PRcProductNumber[i];
                        beanD.CInstallId = PRcInstallID[i];
                        beanD.Disabled = int.Parse(PRcDisabled[i]);

                        beanD.CreatedDate = DateTime.Now;
                        beanD.CreatedUserName = LoginUser_Name;

                        dbOne.TbOneSrdetailProducts.Add(beanD);
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
                    dbOne.TbOneSrdetailRecords.RemoveRange(dbOne.TbOneSrdetailRecords.Where(x => x.Disabled == 0 && x.CSrid == pSRID));
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
                    #endregion

                    #endregion -----↑↑↑↑↑處理與工時紀錄 ↑↑↑↑↑-----

                    #region -----↓↓↓↓↓零件更換資訊↓↓↓↓↓-----

                    #region 刪除明細                   
                    dbOne.TbOneSrdetailPartsReplaces.RemoveRange(dbOne.TbOneSrdetailPartsReplaces.Where(x => x.Disabled == 0 && x.CSrid == pSRID));
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
                    string[] PAcMaterialItem = formCollection["tbx_PAcMaterialItem"];
                    string[] PAcNote = formCollection["tbx_PAcNote"];
                    string[] PAcDisabled = formCollection["hid_PAcDisabled"];

                    int countPA = PAcMaterialID.Length;

                    for (int i = 0; i < countPA; i++)
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

                        beanD.CMaterialItem = PAcMaterialItem[i];
                        beanD.CNote = PAcNote[i];
                        beanD.Disabled = int.Parse(PAcDisabled[i]);

                        beanD.CreatedDate = DateTime.Now;
                        beanD.CreatedUserName = LoginUser_Name;

                        dbOne.TbOneSrdetailPartsReplaces.Add(beanD);
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
                        #region 紀錄修改log
                        TbOneLog logBean = new TbOneLog
                        {
                            CSrid = pSRID,
                            EventName = "SaveGenerallySR",
                            Log = "SR狀態_舊值: " + OldCStatus + "; 新值: " + CStatus,
                            CreatedUserName = LoginUser_Name,
                            CreatedDate = DateTime.Now
                        };

                        dbOne.TbOneLogs.Add(logBean);
                        dbOne.SaveChanges();
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

            string BPMNO = string.Empty;
            string DNDATE = string.Empty;
            string SDATE = string.Empty;
            string EDATE = string.Empty;
            string tURL = string.Empty;
            string tURLName = string.Empty;
            string tSeverName = string.Empty;
            string tInvoiceNo = string.Empty;
            string tInvoiceItem = string.Empty;

            bool tIsFormal = CMF.getCallSAPERPPara(pOperationID_GenerallySR); //取得呼叫SAPERP參數是正式區或測試區(true.正式區 false.測試區)

            if (tIsFormal)
            {
                tURLName = "tsti-bpm01.etatung.com.tw";
                tSeverName = "psip-prd-ap";
            }
            else
            {
                tURLName = "bpm-qas";
                tSeverName = "psip-qas";
            }

            DataTable dtWTY = null; //RFC保固Table

            int NowCount = 0;

            List<SRWarranty> QueryToList = new List<SRWarranty>();    //查詢出來的清單             

            try
            {
                #region 呼叫RFC並回傳保固SLA Table清單
                QueryToList = CMF.ZFM_TICC_SERIAL_SEARCHWTYList(ArySERIAL, ref NowCount, tURLName, tSeverName);
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
                //                    tURL = "http://" + tURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Rwd/Warranty/WarrantyForm.aspx?FormNo=" + bean.BpmNo + " target=_blank";
                //                }
                //                else
                //                {
                //                    tURL = "http://" + tURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Form/Guarantee/GuaranteeForm.aspx?FormNo=" + bean.BpmNo + " target=_blank";
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

            string tURLName = string.Empty;
            string tSeverName = string.Empty;

            bool tIsFormal = CMF.getCallSAPERPPara(pOperationID_GenerallySR); //取得呼叫SAPERP參數是正式區或測試區(true.正式區 false.測試區)

            if (tIsFormal)
            {
                tURLName = "tsti-bpm01.etatung.com.tw";
                tSeverName = "psip-prd-ap";
            }
            else
            {
                tURLName = "bpm-qas";
                tSeverName = "psip-qas";
            }            

            int NowCount = 0;

            List<SRWarranty> QueryToList = new List<SRWarranty>();    //查詢出來的清單             

            try
            {                
                QueryToList = CMF.SEARCHWTYList(cSRID, ref NowCount, tURLName, tSeverName);
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
                    prBean.CreatedUserName = EmpBean.EmployeeCName;
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

                        prBean.ModifiedUserName = EmpBean.EmployeeCName;
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
        /// <param name="cAssEngineerID">目前的服務團隊ERPID(;號隔開)</param>
        /// <param name="cAssEngineerAcc">欲修改的服務團隊ERPID</param>
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
                            CreatedUserName = ViewBag.cLoginUser_Name,
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

            string tEmpName = string.Empty; //服務團隊姓名(中文姓名+英文姓名)

            if (!string.IsNullOrEmpty(cTeamID))
            {
                List<string> liAssAcc = cTeamID.Split(';').ToList();
                int pmId = 0;
                foreach (var AssAcc in liAssAcc)
                {
                    pmId++;
                    if (string.IsNullOrEmpty(AssAcc)) continue;

                    var qPm = dbOne.TbOneSrteamMappings.FirstOrDefault(x => x.Disabled == 0 && x.CTeamOldId == AssAcc);
                    if (qPm != null)
                    {
                        tEmpName = qPm.CTeamOldId + " " + qPm.CTeamOldName;

                        SRTeamInfo pmBean = new SRTeamInfo(pmId, AssAcc, tEmpName, qPm.CTeamNewId, qPm.CTeamNewName);
                        liSRTeamInfo.Add(pmBean);
                    }
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
                    #region 刪除工程師，並回傳最新的工程師
                    var oldPMAcc = cTeamID;

                    List<string> liPmAcc = cTeamID.Split(';').ToList();
                    List<string> liPmAccNew = new List<string>();

                    foreach (string tValue in liPmAcc)
                    {
                        if (tValue.ToLower() != cTeamAcc)
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
                        CreatedUserName = ViewBag.cLoginUser_Name,
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
                            CreatedUserName = ViewBag.cLoginUser_Name,
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
                        CreatedUserName = ViewBag.cLoginUser_Name,
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
                    CreatedUserName = ViewBag.cLoginUser_Name,
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

        #region -----↓↓↓↓↓服務團隊對照設定作業 ↓↓↓↓↓-----
        /// <summary>
        /// 服務團隊對照設定作業
        /// </summary>
        /// <returns></returns>
        public IActionResult SRTeamMapping()
        {
            try
            {
                getLoginAccount();

                #region 取得登入人員資訊
                CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
                EmpBean = CMF.findEmployeeInfo(pLoginAccount);

                ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;               
                #endregion
              
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "失敗原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();

                CMF.writeToLog(pSRID, "SRTeamMapping", pMsg, ViewBag.cLoginUser_Name);
            }

            return View();
        }

        #region 服務團隊對照設定作業查詢結果
        /// <summary>
        /// 服務團隊對照設定作業查詢結果
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
                string[] QueryInfo = new string[8];

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

        #region 儲存服務團隊對照設定檔
        /// <summary>
        /// 儲存服務團隊對照設定檔
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

                        prBean1.CreatedUserName = EmpBean.EmployeeCName;
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

                        prBean1.ModifiedUserName = EmpBean.EmployeeCName;
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

        #region 刪除服務團隊對照設定檔
        /// <summary>
        /// 刪除服務團隊對照設定檔
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
            #endregion

            var ctBean = dbOne.TbOneSrteamMappings.FirstOrDefault(x => x.CId.ToString() == cID);
            ctBean.Disabled = 1;
            ctBean.ModifiedUserName = EmpBean.EmployeeCName;
            ctBean.ModifiedDate = DateTime.Now;

            var result = dbOne.SaveChanges();

            return Json(result);
        }
        #endregion

        #endregion -----↑↑↑↑↑服務團隊對照設定作業 ↑↑↑↑↑-----   

        #region -----↓↓↓↓↓客戶Email對照設定作業 ↓↓↓↓↓-----
        /// <summary>
        /// 客戶Email對照設定作業
        /// </summary>
        /// <returns></returns>
        public IActionResult SRCustomerEmailMapping()
        {
            var model = new ViewModel();

            try
            {
                getLoginAccount();

                #region 取得登入人員資訊
                CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
                EmpBean = CMF.findEmployeeInfo(pLoginAccount);

                ViewBag.cLoginUser_CompCode = EmpBean.CompanyCode;
                ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
                #endregion

            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "失敗原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();

                CMF.writeToLog(pSRID, "SRCustomerEmailMapping", pMsg, ViewBag.cLoginUser_Name);
            }

            return View(model);
        }

        #region 客戶Email對照設定作業查詢結果
        /// <summary>
        /// 客戶Email對照設定作業查詢結果
        /// </summary>        
        /// <param name="cTeamNew">服務團隊部門ID(新)</param>
        /// <param name="cTeamOld">服務團隊ID(舊)</param>
        /// <returns></returns>
        public IActionResult SRCustomerEmailMappingResult(string cTeamNew, string cTeamOld)
        {
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            #region 組待查詢清單
            //var beans = dbOne.tb.TbOneSRCustomerEmailMappings.Where(x => x.Disabled == 0 &&
            //                                             (string.IsNullOrEmpty(cTeamNew) ? true : x.CTeamNewId == cTeamNew) &&
            //                                             (string.IsNullOrEmpty(cTeamOld) ? true : x.CTeamOldId == cTeamOld));

            //foreach (var bean in beans)
            //{
            //    string[] QueryInfo = new string[8];

            //    QueryInfo[0] = bean.CId.ToString(); //系統ID
            //    QueryInfo[1] = bean.CTeamOldId;     //服務團隊ID
            //    QueryInfo[2] = bean.CTeamOldName;   //服務團隊名稱               
            //    QueryInfo[3] = bean.CTeamNewId;     //服務團隊部門ID(新)
            //    QueryInfo[4] = bean.CTeamNewName;   //對應部門名稱                

            //    QueryToList.Add(QueryInfo);
            //}

            //ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #region 儲存客戶Email對照設定檔
        ///// <summary>
        ///// 儲存客戶Email對照設定檔
        ///// </summary>
        ///// <param name="cID">系統ID</param>
        ///// <param name="cTeamOldID">程式作業編號檔系統ID</param>
        ///// <param name="cTeamOldName">功能別</param>
        ///// <param name="cTeamNewID">公司別</param>
        ///// <param name="cTeamNewName">參數No</param>        
        ///// <returns></returns>
        //public ActionResult saveCustomerEmail(string cID, string cTeamOldID, string cTeamOldName, string cTeamNewID, string cTeamNewName)
        //{
        //    getLoginAccount();

        //    #region 取得登入人員資訊
        //    CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
        //    EmpBean = CMF.findEmployeeInfo(pLoginAccount);

        //    ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
        //    #endregion

        //    string tMsg = string.Empty;

        //    try
        //    {
        //        int result = 0;
        //        if (cID == null)
        //        {
        //            #region -- 新增 --
        //            var prBean = dbOne.TbOneSRCustomerEmailMappings.FirstOrDefault(x => x.Disabled == 0 &&
        //                                                                   x.CTeamOldId == cTeamOldID &&
        //                                                                   x.CTeamNewId == cTeamNewID);
        //            if (prBean == null)
        //            {
        //                TbOneSRCustomerEmailMapping prBean1 = new TbOneSRCustomerEmailMapping();

        //                prBean1.CTeamOldId = cTeamOldID.Trim();
        //                prBean1.CTeamOldName = cTeamOldName.Trim();
        //                prBean1.CTeamNewId = cTeamNewID.Trim();
        //                prBean1.CTeamNewName = cTeamNewName.Trim();
        //                prBean1.Disabled = 0;

        //                prBean1.CreatedUserName = EmpBean.EmployeeCName;
        //                prBean1.CreatedDate = DateTime.Now;

        //                dbOne.TbOneSRCustomerEmailMappings.Add(prBean1);
        //                result = dbOne.SaveChanges();
        //            }
        //            else
        //            {
        //                tMsg = "此服務團隊已存在，請重新輸入！";
        //            }
        //            #endregion                
        //        }
        //        else
        //        {
        //            #region -- 編輯 --
        //            var prBean = dbOne.TbOneSRCustomerEmailMappings.FirstOrDefault(x => x.Disabled == 0 &&
        //                                                                    x.CId.ToString() != cID &&
        //                                                                    x.CTeamOldId == cTeamOldID &&
        //                                                                    x.CTeamNewId == cTeamNewID);
        //            if (prBean == null)
        //            {
        //                var prBean1 = dbOne.TbOneSRCustomerEmailMappings.FirstOrDefault(x => x.CId.ToString() == cID);
        //                prBean1.CTeamNewId = cTeamNewID.Trim();
        //                prBean1.CTeamNewName = cTeamNewName.Trim();

        //                prBean1.ModifiedUserName = EmpBean.EmployeeCName;
        //                prBean1.ModifiedDate = DateTime.Now;
        //                result = dbOne.SaveChanges();
        //            }
        //            else
        //            {
        //                tMsg = "此服務團隊已存在，請重新輸入！";
        //            }
        //            #endregion
        //        }
        //        return Json(tMsg);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(e.Message);
        //    }
        //}
        #endregion

        #region 刪除客戶Email對照設定檔
        ///// <summary>
        ///// 刪除客戶Email對照設定檔
        ///// </summary>
        ///// <param name="cID">系統ID</param>
        ///// <returns></returns>
        //public ActionResult DeleteSRCustomerEmailMapping(string cID)
        //{
        //    getLoginAccount();

        //    #region 取得登入人員資訊
        //    CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
        //    EmpBean = CMF.findEmployeeInfo(pLoginAccount);

        //    ViewBag.cLoginUser_Name = EmpBean.EmployeeCName;
        //    #endregion

        //    var ctBean = dbOne.TbOneSRCustomerEmailMappings.FirstOrDefault(x => x.CId.ToString() == cID);
        //    ctBean.Disabled = 1;
        //    ctBean.ModifiedUserName = EmpBean.EmployeeCName;
        //    ctBean.ModifiedDate = DateTime.Now;

        //    var result = dbOne.SaveChanges();

        //    return Json(result);
        //}
        #endregion       

        #endregion -----↑↑↑↑↑客戶Email對照設定作業 ↑↑↑↑↑-----   

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
            //pLoginAccount = User.Identity.Name;

            #region One Service相關帳號
            pIsMIS = CMF.getIsMIS(pLoginAccount, pSysOperationID);

            ViewBag.pIsMIS = pIsMIS;
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
        /// <param name="cEditContactEmail">客戶聯絡人Email</param>
        /// <param name="ModifiedUserName">修改人姓名</param>        
        /// <returns></returns>
        public IActionResult SaveEditContactInfo(string cEditContactID, string cBUKRS, string cCustomerID, string cCustomerName, string cEditContactName,
                                               string cEditContactCity, string cEditContactAddress, string cEditContactPhone, string cEditContactEmail, string ModifiedUserName)
        {
            var bean = dbProxy.CustomerContacts.FirstOrDefault(x => x.ContactId.ToString() == cEditContactID);

            if (bean != null) //修改
            {
                bean.ContactCity = cEditContactCity;
                bean.ContactAddress = cEditContactAddress;
                bean.ContactPhone = cEditContactPhone;
                bean.ContactEmail = cEditContactEmail;

                bean.ModifiedUserName = ModifiedUserName;
                bean.ModifiedDate = DateTime.Now;
            }           

            var result = dbProxy.SaveChanges();

            return Json(result);
        }
        #endregion

        #region Ajax儲存客戶聯絡人(for新增)
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

            var bean = dbProxy.CustomerContacts.FirstOrDefault(x => (x.Disabled == null || x.Disabled != 1) && 
                                                                 x.BpmNo == tBpmNo && x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == cCustomerID && x.ContactName == cAddContactName);

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

                bean1.ContactId = Guid.NewGuid();
                bean1.Kna1Kunnr = cCustomerID;
                bean1.Kna1Name1 = cCustomerName;
                bean1.Knb1Bukrs = cBUKRS;
                bean1.ContactType = "4";
                bean1.ContactName = cAddContactName;
                bean1.ContactCity = cAddContactCity;
                bean1.ContactAddress = cAddContactAddress;
                bean1.ContactPhone = cAddContactPhone;
                bean1.ContactEmail = cAddContactEmail;
                bean1.BpmNo = tBpmNo;
                bean1.Disabled = 0;

                bean1.ModifiedUserName = ModifiedUserName;                
                bean1.ModifiedDate = DateTime.Now;

                dbProxy.CustomerContacts.Add(bean1);
            }

            var result = dbProxy.SaveChanges();

            return Json(result);
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
            var beans = dbProxy.Stockalls.Where(x => x.IvSerial.Contains(IV_SERIAL.Trim()));

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
        /// <param name="keyword">關鍵字</param>        
        /// <returns></returns>
        public IActionResult findMaterial(string keyword)
        {
            Object contentObj = CMF.findMaterialByKeyWords(keyword);

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

            contentObj = dbProxy.CustomerContacts.Where(x => (x.Disabled == null || x.Disabled != 1) &&
                                                           x.BpmNo == tBpmNo && x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == cCustomerID && x.ContactName.Contains(keyword));

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
        /// <param name="cEmptyOption">是否要產生「請選擇」選項(True.要 false.不要)</param>
        /// <returns></returns>
        static public List<SelectListItem> findSRTeamMappingListItem(bool cEmptyOption)
        {
            CommonFunction CMF = new CommonFunction();

            var tList = CMF.findSRTeamMappingListItem(cEmptyOption);

            return tList;
        }
        #endregion

        #endregion -----↑↑↑↑↑共用方法 ↑↑↑↑↑-----    

        #region -----↓↓↓↓↓自定義Class ↓↓↓↓↓-----     

        #region DropDownList選項Class
        /// <summary>
        /// DropDownList選項Class
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

            #region 服務團隊ID
            public string ddl_cQueryTeamID { get; set; }
            public List<SelectListItem> ListTeamID = findSRTeamMappingListItem(true);
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
            /// <summary>製造商零件號碼</summary>
            public string MFRPN { get; set; }
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
}
