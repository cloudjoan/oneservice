using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneService.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
        /// 服務請求ID
        /// </summary>
        string pSRID = string.Empty;

        /// <summary>
        /// 程式作業編號檔系統ID(ALL，固定的GUID)
        /// </summary>
        string pSysOperationID = "F8EFC55F-FA77-4731-BB45-2F2147244A2D";

        /// <summary>
        /// 程式作業編號檔系統ID(一般服務請求)
        /// </summary>
        static string pOperationID_GenerallySR = "869FC989-1049-4266-ABDE-69A9B07BCD0A";

        /// <summary>
        /// 公司別(T012、T016、C069、T022)
        /// </summary>
        static string pCompanyCode = string.Empty;

        static CommonFunction CMF = new CommonFunction();

        PSIPContext psipDb = new PSIPContext();
        TSTIONEContext dbOne = new TSTIONEContext();
        TAIFContext bpmDB = new TAIFContext();
        ERP_PROXY_DBContext dbProxy = new ERP_PROXY_DBContext();
        MCSWorkflowContext dbEIP = new MCSWorkflowContext();

        #region -----↓↓↓↓↓一般服務請求 ↓↓↓↓↓-----

        #region 一般服務請求index
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
            var SRTypeOneList = CMF.findFirstKINDList();

            var SRTypeSecList = new List<SelectListItem>();
            SRTypeSecList.Add(new SelectListItem { Text = " ", Value = "" });

            var SRTypeThrList = new List<SelectListItem>();
            SRTypeThrList.Add(new SelectListItem { Text = " ", Value = "" });          

            ViewBag.SRTypeOneList = SRTypeOneList;
            ViewBag.SRTypeSecList = SRTypeSecList;
            ViewBag.SRTypeThrList = SRTypeThrList;
            #endregion

            #region 取得服務團隊清單
            var SRTeamIDList = CMF.findSRTeamIDList();           
            #endregion

            #region 取得SRID
            var beanM = dbOne.TbOneSrmains.FirstOrDefault(x => x.CSrid == pSRID);

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
                ViewBag.cMAServiceType = beanM.CMaserviceType;
                ViewBag.pStatus = "E0001";
                ViewBag.CreatedDate = Convert.ToDateTime(beanM.CreatedDate).ToString("yyyy-MM-dd");

                //指定要顯示的服務團隊清單
                SRTeamIDList.Where(q => q.Value == beanM.CTeamId).First().Selected = true;
            }
            else
            {
                ViewBag.cSRPathWay = "Z05";     //手動建立
                ViewBag.pStatus = "E0001";      //新建
                ViewBag.cMAServiceType = "";    //請選擇
                ViewBag.cSRProcessWay = "";     //請選擇
            }
            #endregion

            #region 指派Option值
            model.ddl_cStatus = ViewBag.pStatus;                //設定狀態
            model.ddl_cSRPathWay = ViewBag.cSRPathWay;          //設定報修管道
            model.ddl_cMAServiceType = ViewBag.cMAServiceType;   //設定維護服務種類
            model.ddl_cSRProcessWay = ViewBag.cSRProcessWay;    //設定處理方式
            #endregion

            ViewBag.pOperationID = pOperationID_GenerallySR;
            ViewBag.SRTeamIDList = SRTeamIDList;

            return View(model);
        }
        #endregion

        #region 儲存一般服務請求
        /// <summary>
        /// 儲存一般服務請求
        /// </summary>
        /// <returns></returns>
        public IActionResult SaveGenerallySR()
        {            
            getLoginAccount();

            CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
            EmpBean = CMF.findEmployeeInfo(pLoginAccount);

            pLoginName = EmpBean.EmployeeCName;

            return RedirectToAction("finishForm");
        }
        #endregion

        #region 保固SLA資訊查詢結果
        /// <summary>
        /// 保固SLA資訊查詢結果
        /// </summary>
        /// <param name="ArySERIAL">序號(可多筆)</param>
        /// <returns></returns>
        public ActionResult QuerySRDetail_Warranty(string[] ArySERIAL)
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
            string tInvoiceNo = string.Empty;
            string tInvoiceItem = string.Empty;

            bool tIsFormal = CMF.getCallSAPERPPara(pOperationID_GenerallySR); //取得呼叫SAPERP參數是正式區或測試區(true.正式區 false.測試區)

            if (tIsFormal)
            {
                tURLName = "tsti-bpm01.etatung.com.tw";
            }
            else
            {
                tURLName = "bpm-qas";
            }

            DataTable dtWTY = null; //RFC保固Table

            int count = 0;

            List<SRWarranty> QueryToList = new List<SRWarranty>();    //查詢出來的清單             

            try
            {
                foreach(string IV_SERIAL in ArySERIAL)
                {
                    //#region 先塞RFC保固Table
                    //dtWTY = CMF.ZFM_TICC_SERIAL_SEARCHWTYList(IV_SERIAL);

                    //foreach (DataRow dr in dtWTY.Rows)
                    //{
                    //    if (dr["WTYCODE"].ToString().Trim() == "") //空白代表才有合約編號才執行
                    //    {
                    //        #region 組待查詢清單
                    //        string[] QueryInfo = new string[16];

                    //        SDATE = dr["WTYSTART"].ToString() == "" ? "" : Convert.ToDateTime(dr["WTYSTART"].ToString()).ToString("yyyy-MM-dd");
                    //        EDATE = dr["WTYEND"].ToString() == "" ? "" : Convert.ToDateTime(dr["WTYEND"].ToString()).ToString("yyyy-MM-dd");

                    //        #region 取得BPM Url
                    //        BPMNO = "";
                    //        tURL = "";

                    //        if (dr["CONTRACT"].ToString() != "")
                    //        {
                    //            BPMNO = CMF.findContractSealsFormNo(dr["CONTRACT"].ToString().Trim());

                    //            try
                    //            {
                    //                if (dr["CONTRACT"].ToString().Trim() != "")
                    //                {
                    //                    Int32 ContractID = Int32.Parse(dr["CONTRACT"].ToString().Trim());

                    //                    if (ContractID >= 10506151 && ContractID != 10506152 && ContractID != 10506157) //新的用印申請單
                    //                    {
                    //                        tURL = "http://" + tURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Rwd/ContractSeals/ContractSealsForm.aspx?FormNo=" + BPMNO + " target=_blank";
                    //                    }
                    //                    else //舊的用印申請單
                    //                    {
                    //                        tURL = "http://" + tURLName + "/ContractSeals/_layouts/FormServer.aspx?XmlLocation=%2fContractSeals%2fBPMContractSealsForm%2f" + BPMNO + ".xml&ClientInstalled=true&DefaultItemOpen=1&source=/_layouts/TSTI.SharePoint.BPM/CloseWindow.aspx target=_blank";
                    //                    }
                    //                }
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                BPMNO = "";
                    //                tURL = "";
                    //            }
                    //        }
                    //        #endregion

                    //        QueryInfo[0] = IV_SERIAL;                  //序號
                    //        QueryInfo[1] = "";                         //銷售訂單號
                    //        QueryInfo[2] = "";                         //出貨日期
                    //        QueryInfo[3] = "";                         //保固                                                               
                    //        QueryInfo[4] = "";                         //保固說明
                    //        QueryInfo[5] = SDATE;                      //保固開始日期
                    //        QueryInfo[6] = EDATE;                      //保固結束日期                                                          
                    //        QueryInfo[7] = dr["SLARESP"].ToString();    //回應條件
                    //        QueryInfo[8] = dr["SLASRV"].ToString();     //服務條件
                    //        QueryInfo[9] = dr["CONTRACT"].ToString();   //合約編號
                    //        QueryInfo[10] = "";                        //計費業務
                    //        QueryInfo[11] = "";                        //委外廠商HP CARE PACK
                    //        QueryInfo[12] = "";                        //備註
                    //        QueryInfo[13] = BPMNO;                     //BPM表單編號
                    //        QueryInfo[14] = "";                        //客服主管建議                        
                    //        QueryInfo[15] = tURL;                      //BPM URL                        

                    //        QueryToList.Add(QueryInfo);
                    //        #endregion
                    //    }
                    //}
                    //#endregion

                    #region 保固
                    var beans = dbProxy.Stockwties.OrderByDescending(x => x.IvEdate).ThenByDescending(x => x.BpmNo).Where(x => x.IvSerial == IV_SERIAL.Trim());

                    foreach (var bean in beans)
                    {
                        count++;

                        #region 組待查詢清單
                        SRWarranty QueryInfo = new SRWarranty();

                        //string[] tBPMList = CMF.findBPMWarrantyInfo(bean.BpmNo);

                        DNDATE = bean.IvDndate== null ? "" : Convert.ToDateTime(bean.IvDndate).ToString("yyyy-MM-dd");
                        SDATE = bean.IvSdate == null ? "" : Convert.ToDateTime(bean.IvSdate).ToString("yyyy-MM-dd");
                        EDATE = bean.IvEdate == null ? "" : Convert.ToDateTime(bean.IvEdate).ToString("yyyy-MM-dd");

                        #region 取得BPM Url
                        tURL = "";

                        if (bean.BpmNo != null)
                        {
                            if (bean.BpmNo.IndexOf("WTY") != -1)
                            {
                                tURL = "http://" + tURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Rwd/Warranty/WarrantyForm.aspx?FormNo=" + bean.BpmNo + " target=_blank";
                            }
                            else
                            {
                                tURL = "http://" + tURLName + "/sites/bpm/_layouts/Taif/BPM/Page/Form/Guarantee/GuaranteeForm.aspx?FormNo=" + bean.BpmNo + " target=_blank";
                            }
                        }
                        #endregion

                        QueryInfo.cID = count.ToString();                                           //系統ID
                        QueryInfo.cSerialID = bean.IvSerial;                                         //序號                        
                        QueryInfo.cWTYID = bean.IvWtyid;                                             //保固
                        QueryInfo.cWTYName = bean.IvWtydesc;                                         //保固說明
                        QueryInfo.cWTYSDATE = SDATE;                                                //保固開始日期
                        QueryInfo.cWTYEDATE = EDATE;                                                //保固結束日期                                                          
                        QueryInfo.cSLARESP = bean.IvSlaresp;                                         //回應條件
                        QueryInfo.cSLASRV = bean.IvSlasrv;                                          //服務條件
                        QueryInfo.cContractID = "";                                                 //合約編號                        
                        QueryInfo.cBPMFormNo = string.IsNullOrEmpty(bean.BpmNo) ? "" : bean.BpmNo;      //BPM表單編號                        
                        QueryInfo.cBPMFormNoUrl = tURL;                                             //BPM URL                    
                        QueryInfo.cUsed = "N";                                                     //本次使用

                        QueryToList.Add(QueryInfo);
                        #endregion
                    }
                    #endregion
                }                
            }
            catch (Exception ex)
            {
                pMsg += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "失敗原因:" + ex.Message + Environment.NewLine;
                pMsg += " 失敗行數：" + ex.ToString();

                //CMF.writeToLog("QueryStockALLWTYResult:" + Environment.NewLine + pMsg, "QueryStockALLWTYResult");
            }

            return Json(QueryToList);
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

        #endregion -----↑↑↑↑↑一般服務請求 ↑↑↑↑↑-----    

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
        public ActionResult SpareBOM(string ProdID)
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
        public ActionResult findMaterial(string keyword)
        {
            Object contentObj = CMF.findMaterialByKeyWords(keyword);

            return Json(contentObj);
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
            /// <summary>保固申請(BPM表單編號)</summary>
            public string cBPMFormNo { get; set; }
            /// <summary>保固申請Url(BPM表單編號Url)</summary>
            public string cBPMFormNoUrl { get; set; }
            /// <summary>本次使用</summary>
            public string cUsed { get; set; }
        }
        #endregion

        #endregion -----↑↑↑↑↑共用方法 ↑↑↑↑↑-----  
    }
}
