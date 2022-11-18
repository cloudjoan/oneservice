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
        TSTIONEContext dbOne = new TSTIONEContext();
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

            #region 報修類別
            var SRTypeOneList = CMF.findFirstKINDList();

            var SRTypeSecList = new List<SelectListItem>();
            SRTypeSecList.Add(new SelectListItem { Text = " ", Value = "" });

            var SRTypeThrList = new List<SelectListItem>();
            SRTypeThrList.Add(new SelectListItem { Text = " ", Value = "" });          

            ViewBag.SRTypeOneList = SRTypeOneList;
            ViewBag.SRTypeSecList = SRTypeSecList;
            ViewBag.SRTypeThrList = SRTypeThrList;
            #endregion

            #region 取得SRID
            var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == "6100000001");

            if (beanM != null)
            {
                //記錄目前GUID，用來判斷更新的先後順序
                ViewBag.pGUID = beanM.CSystemGuid.ToString();

                ViewBag.cSRID = beanM.CSrid;
                ViewBag.cCustomerID = beanM.CCustomerId;
                ViewBag.cCustomerName = beanM.CCustomerName;
                ViewBag.cDesc = beanM.CDesc;
                ViewBag.cNotes = beanM.CNotes;
                ViewBag.cSRPathWay = beanM.CSrpathWay;
                ViewBag.pStatus = "E0001";
            }
            #endregion

            #region 設定狀態
            var model = new StatusViewModel();
            model.ddl_cStatus = "E0001";
            #endregion

            ViewBag.pStatus = "E0001";
            ViewBag.pOperationID = pOperationID_GenerallySR;           

            return View(model);
        }

        public IActionResult SaveGenerallySR()
        {            
            getLoginAccount();

            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            pLoginName = EmpBean.EmployeeCName;

            return RedirectToAction("finishForm");
        }

        #region -----↓↓↓↓↓共用方法 ↓↓↓↓↓-----

        /// <summary>
        /// 提交表單後開啟該完成表單，並顯示即將關閉後再關閉此頁
        /// </summary>
        /// <returns></returns>
        public IActionResult finishForm()
        {
            return View();
        }

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

        #region Ajax儲存客戶聯絡人
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

            var bean = dbProxy.CustomerContacts.FirstOrDefault(x => x.BpmNo == tBpmNo && x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == cCustomerID && x.ContactName == cAddContactName);

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
                bean1.ContactType = "0";
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

        #region Ajax傳入第一階(大類)並取得第二階(中類)清單
        /// <summary>
        /// Ajax傳入第一階(大類)並取得第二階(中類)清單
        /// </summary>
        /// <param name="keyword">第一階(大類)代碼</param>
        /// <returns></returns>
        public ActionResult findSRTypeSecList(string keyword)
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
        public ActionResult findSRTypeThrList(string keyword)
        {
            var tList = new List<SelectListItem>();

            tList = CMF.findSRTypeThrList(keyword);

            ViewBag.SRTypeThrList = tList;
            return Json(tList);
        }
        #endregion

        #endregion -----↑↑↑↑↑共用方法 ↑↑↑↑↑-----    

        #region -----↓↓↓↓↓自定義Class ↓↓↓↓↓-----

        #region 狀態
        /// <summary>
        /// 狀態
        /// </summary>
        public class StatusViewModel
        {
            public string ddl_cStatus { get; set; }
            public List<SelectListItem> ListStatus { get; } = new List<SelectListItem>
            {
                new SelectListItem { Value = "E0001", Text = "新建" },
                new SelectListItem { Value = "E0002", Text = "L2處理中" },
                new SelectListItem { Value = "E0003", Text = "報價中" },
                new SelectListItem { Value = "E0004", Text = "3rd Party處理中" },
                new SelectListItem { Value = "E0005", Text = "L3處理中" },
                new SelectListItem { Value = "E0006", Text = "完修" },
                new SelectListItem { Value = "E0012", Text = "HPGCSN 申請" },
                new SelectListItem { Value = "E0013", Text = "HPGCSN 完成" },
                new SelectListItem { Value = "E0014", Text = "駁回" },
                new SelectListItem { Value = "E0015", Text = "取消" },                
            };
        }
        #endregion

        #endregion -----↑↑↑↑↑共用方法 ↑↑↑↑↑-----  
    }
}
