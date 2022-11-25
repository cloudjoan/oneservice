using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneService.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OneService.Controllers
{
	public class SysParameterController : Controller
	{        
        PSIPContext psipDb = new PSIPContext();
        TAIFContext bpmDB = new TAIFContext();

        #region -----資訊系統作業設定 Start-----
        /// <summary>
        /// 資訊系統作業設定
        /// </summary>
        /// <returns></returns>
        public IActionResult OperationParameter()
		{
			return View();
		}

        #region 資訊系統作業設定查詢結果
        /// <summary>
        /// 資訊系統作業設定查詢結果
        /// </summary>
        /// <param name="cModuleID">模組別</param>
        /// <param name="cOperationID">程式作業編號</param>
        /// <param name="cOperationName">程式作業編號</param>
        /// <returns></returns>        
        public IActionResult OperationParameterResult(string? cModuleID, string? cOperationID, string? cOperationName)
		{
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            #region 組待查詢清單
            var beans = psipDb.TbOneOperationParameters.OrderBy(x => x.CModuleId).OrderBy(x => x.COperationId).Where(x => x.Disabled == 0 &&
                                                           (string.IsNullOrEmpty(cModuleID) ? true : x.CModuleId == cModuleID) &&
                                                           (string.IsNullOrEmpty(cOperationID) ? true : x.COperationId.Contains(cOperationID.Trim())) &&
                                                           (string.IsNullOrEmpty(cOperationName) ? true : x.COperationName.Contains(cOperationName.Trim())));

            foreach (var bean in beans)
            {
                string[] QueryInfo = new string[6];

                QueryInfo[0] = bean.CId.ToString();             //系統ID
                QueryInfo[1] = bean.CModuleId;                  //模組別
                QueryInfo[2] = TransModuleID(bean.CModuleId);   //模組名稱
                QueryInfo[3] = bean.COperationId;               //程式作業編號
                QueryInfo[4] = bean.COperationName;             //程式作業名稱
                QueryInfo[5] = bean.COperationUrl;              //程式作業網址          

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion        

        #region 儲存資訊系統作業設定主檔
        /// <summary>
        /// 儲存資訊系統作業設定主檔
        /// </summary>
        /// <param name="cID">GUID系統編號</param>
        /// <param name="cModuleID">模組別</param>
        /// <param name="cOperationID">程式作業編號</param>
        /// <param name="cOperationName">程式作業名稱</param>
        /// <param name="cOperationURL">程式作業網址</param>
        /// <returns></returns>
        public ActionResult SaveOperation(string cID, string cModuleID, string cOperationID, string cOperationName, string cOperationURL)
        {
            string tMsg = string.Empty;            

            try
            {
                int result = 0;
                if (cID == null)
                {
                    #region -- 新增 --
                    var prBean = psipDb.TbOneOperationParameters.FirstOrDefault(x => x.Disabled == 0 && x.CModuleId == cModuleID && x.COperationId == cOperationID);
                    if (prBean == null)
                    {
                        TbOneOperationParameter prBean1 = new TbOneOperationParameter();
                        prBean1.CId = Guid.NewGuid();
                        prBean1.CModuleId = cModuleID;
                        prBean1.COperationId = cOperationID.Trim();
                        prBean1.COperationName = cOperationName.Trim();
                        prBean1.COperationUrl = cOperationURL == null ? "" : cOperationURL.Trim();
                        prBean1.Disabled = 0;

                        prBean1.CreatedUserName = "SYS";
                        prBean1.CreatedDate = DateTime.Now;

                        psipDb.TbOneOperationParameters.Add(prBean1);
                        result = psipDb.SaveChanges();
                    }
                    else
                    {
                        tMsg = "【模組別 + 程式作業編號】已存在，請重新輸入！";
                    }
                    #endregion                
                }
                else
                {
                    #region -- 編輯 --
                    var prBean = psipDb.TbOneOperationParameters.FirstOrDefault(x => x.CId.ToString() == cID);
                    prBean.COperationName = cOperationName.Trim();
                    prBean.COperationUrl = cOperationURL == null ? "": cOperationURL.Trim();

                    prBean.ModifiedUserName = "SYS";
                    prBean.ModifiedDate = DateTime.Now;
                    result = psipDb.SaveChanges();
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

        #region 刪除資訊系統作業設定主檔
        /// <summary>
        /// 刪除資訊系統作業設定主檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <returns></returns>
        public ActionResult DeleteOperationParameter(string cID)
        {
            string tMsg = string.Empty;

            var prBean = psipDb.TbOneSysParameters.FirstOrDefault(x => x.Disabled == 0 && x.COperationId.ToString() == cID);
            var prBean1 = psipDb.TbOneRoleParameters.FirstOrDefault(x => x.Disabled == 0 && x.COperationId.ToString() == cID);

            if (prBean != null)
            {
                tMsg = "此作業編號已在【資訊系統參數設定檔】被使用，不准許刪除！";
            }
            else if (prBean1 != null)
            {
                tMsg = "此作業編號已在【資訊系統角色權限設定檔】被使用，不准許刪除！";
            }
            else
            {
                var ctBean = psipDb.TbOneOperationParameters.FirstOrDefault(x => x.CId.ToString() == cID);
                ctBean.Disabled = 1;
                ctBean.ModifiedUserName = "SYS"; //EmpBean.EmployeeCName;
                ctBean.ModifiedDate = DateTime.Now;

                var result = psipDb.SaveChanges();

                if (result <= 0)
                {
                    tMsg = "刪除失敗，請重新確認！";
                }
            }

            return Json(tMsg);
        }
        #endregion

        #region 模組別ID轉換成名稱
        /// <summary>
        /// 模組別ID轉換成名稱
        /// </summary>
        /// <param name="cModuleID">模組別</param>
        /// <returns></returns>
        private string TransModuleID(string cModuleID)
        {
            string reValue = string.Empty;

            switch (cModuleID)
            {
                case "ALL":
                    reValue = "全模組";
                    break;
                case "PRODUCT":
                    reValue = "產品";
                    break;
                case "SALES":
                    reValue = "業務";
                    break;
                case "PROSERVICE":
                    reValue = "專業服務";
                    break;
                case "HR":
                    reValue = "人力";
                    break;
                case "FIN":
                    reValue = "財務";
                    break;
                case "DOCMA":
                    reValue = "文管";
                    break;
                case "INFO":
                    reValue = "資訊";
                    break;
                case "ONESERVICE":
                    reValue = "One Service";
                    break;
            }

            return reValue;
        }
        #endregion

        #endregion -----資訊系統作業設定 End-----

        #region -----資訊系統參數作業設定 Start-----
        /// <summary>
        /// 資訊系統參數作業設定
        /// </summary>
        /// <returns></returns>
        public IActionResult SysParameter()
        {
            #region 程式作業編號檔系統ID清單
            var ModuleList = findModuleIDList();

            var OperationList = new List<SelectListItem>();
            OperationList.Add(new SelectListItem { Text = " ", Value = "" });

            ViewBag.ModuleList = ModuleList;
            ViewBag.OperationList = OperationList;            
            #endregion

            return View();
        }

        #region 資訊系統參數作業設定查詢結果
        /// <summary>
        /// 資訊系統參數作業設定查詢結果
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cFunctionID">功能別</param>
        /// <param name="cCompanyID">公司別</param>
        /// <param name="cNo">參數No</param>
        /// <param name="cValue">參數值</param>
        /// <param name="cDescription">參數值說明</param>
        /// <returns></returns>
        public IActionResult SysParameterResult(string cOperationID, string cFunctionID, string cCompanyID, string cNo, string cValue, string cDescription)
        {
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            #region 組待查詢清單
            var beans = psipDb.TbOneSysParameters.OrderBy(x => x.COperationId).OrderBy(x => x.CFunctionId).OrderBy(x => x.CCompanyId).OrderBy(x => x.CNo)
                                              .Where(x => x.Disabled == 0 && 
                                                            (string.IsNullOrEmpty(cOperationID) ? true : x.COperationId.ToString() == cOperationID) &&
                                                            (string.IsNullOrEmpty(cCompanyID) ? true : x.CCompanyId == cCompanyID) &&
                                                            (string.IsNullOrEmpty(cFunctionID) ? true : x.CFunctionId == cFunctionID) &&
                                                            (string.IsNullOrEmpty(cNo) ? true : x.CNo.Contains(cNo.Trim())) &&
                                                            (string.IsNullOrEmpty(cValue) ? true : x.CValue.Contains(cValue.Trim())) &&
                                                            (string.IsNullOrEmpty(cDescription) ? true : x.CDescription.Contains(cDescription.Trim())));

            foreach (var bean in beans)
            {
                string[] QueryInfo = new string[8];

                QueryInfo[0] = bean.CId.ToString();                   //系統ID
                QueryInfo[1] = bean.COperationId.ToString();           //程式作業編號檔系統ID
                QueryInfo[2] = bean.CFunctionId;                       //功能別
                QueryInfo[3] = TransSysFunctionID(bean.CFunctionId);   //功能別名稱
                QueryInfo[4] = bean.CCompanyId;                        //公司別
                QueryInfo[5] = bean.CNo;                              //參數No
                QueryInfo[6] = bean.CValue;                           //參數值        
                QueryInfo[7] = bean.CDescription;                     //參數值說明       

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #region 儲存資訊系統參數設定檔
        /// <summary>
        /// 儲存資訊系統參數設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cFunctionID">功能別</param>
        /// <param name="cCompanyID">公司別</param>
        /// <param name="cNo">參數No</param>
        /// <param name="cValue">參數值</param>
        /// <param name="cDescription">參數值說明</param>
        /// <returns></returns>
        public ActionResult SaveSys(string cID, string cOperationID, string cFunctionID, string cCompanyID, string cNo, string cValue, string cDescription)
        {
            string tMsg = string.Empty;

            try
            {
                int result = 0;
                if (cID == null)
                {
                    #region -- 新增 --
                    var prBean = psipDb.TbOneSysParameters.FirstOrDefault(x => x.Disabled == 0 && 
                                                                            x.COperationId.ToString() == cOperationID && 
                                                                            x.CFunctionId == cFunctionID && 
                                                                            x.CCompanyId == cCompanyID &&
                                                                            x.CNo == cNo &&
                                                                            x.CValue == cValue);
                    if (prBean == null)
                    {
                        TbOneSysParameter prBean1 = new TbOneSysParameter();                        
                        prBean1.COperationId = Guid.Parse(cOperationID);
                        prBean1.CFunctionId = cFunctionID;
                        prBean1.CCompanyId = cCompanyID;
                        prBean1.CNo = cNo.Trim();
                        prBean1.CValue = cValue.Trim();
                        prBean1.CDescription = cDescription.Trim();
                        prBean1.Disabled = 0;

                        prBean1.CreatedUserName = "SYS";
                        prBean1.CreatedDate = DateTime.Now;

                        psipDb.TbOneSysParameters.Add(prBean1);
                        result = psipDb.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此系統參數已存在，請重新輸入！";
                    }
                    #endregion                
                }
                else
                {
                    #region -- 編輯 --
                    var prBean = psipDb.TbOneSysParameters.FirstOrDefault(x => x.Disabled == 0 &&
                                                                            x.CId.ToString() != cID &&
                                                                            x.COperationId.ToString() == cOperationID &&
                                                                            x.CFunctionId == cFunctionID &&
                                                                            x.CCompanyId == cCompanyID &&
                                                                            x.CNo == cNo &&
                                                                            x.CValue == cValue);
                    if (prBean == null)
                    {
                        var prBean1 = psipDb.TbOneSysParameters.FirstOrDefault(x => x.CId.ToString() == cID);
                        prBean1.CValue = cValue.Trim();
                        prBean1.CDescription = cDescription.Trim();

                        prBean1.ModifiedUserName = "SYS";
                        prBean1.ModifiedDate = DateTime.Now;
                        result = psipDb.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此系統參數已存在，請重新輸入！";
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

        #region 刪除資訊系統參數設定檔
        /// <summary>
        /// 刪除資訊系統參數設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <returns></returns>
        public ActionResult DeleteSysParameter(string cID)
        {
            var ctBean = psipDb.TbOneSysParameters.FirstOrDefault(x => x.CId.ToString() == cID);
            ctBean.Disabled = 1;
            ctBean.ModifiedUserName = "SYS"; //EmpBean.EmployeeCName;
            ctBean.ModifiedDate = DateTime.Now;

            var result = psipDb.SaveChanges();

            return Json(result);
        }
        #endregion

        #region 參數功能別ID轉換成名稱
        /// <summary>
        /// 參數功能別ID轉換成名稱
        /// </summary>
        /// <param name="cFunctionID">模組別</param>
        /// <returns></returns>
        private string TransSysFunctionID(string cFunctionID)
        {
            string reValue = string.Empty;

            switch (cFunctionID.ToUpper())
            {
                case "SENDMAIL":
                    reValue = "寄送Mail";
                    break;
                case "ACCOUNT":
                    reValue = "人員帳號";
                    break;
                case "OTHER":
                    reValue = "其他自定義";
                    break;                
            }

            return reValue;
        }
        #endregion

        #region 取得所有程式作業編號檔系統ID清單
        public List<SelectListItem> findModuleIDList()
        {
            List<string> tPSList = new List<string>();

            string ProductName = string.Empty;

            var beans = psipDb.TbOneOperationParameters.OrderBy(x => x.CModuleId).Where(x => x.Disabled == 0);

            var ModuleList = new List<SelectListItem>();
            ModuleList.Add(new SelectListItem { Text = " ", Value = "" });

            foreach (var bean in beans)
            {
                if (!tPSList.Contains(bean.CModuleId))
                {
                    ProductName = bean.CModuleId + "_" + TransModuleID(bean.CModuleId);

                    ModuleList.Add(new SelectListItem { Text = ProductName, Value = bean.CModuleId });
                    tPSList.Add(bean.CModuleId);
                }
            }

            return ModuleList;
        }
        #endregion

        #region Ajax傳入模組別並取得程式作業編號清單
        /// <summary>
        /// Ajax傳入模組別並取得程式作業編號清單
        /// </summary>
        /// <param name="cModuleID">模組別</param>
        /// <returns></returns>
        public IActionResult AjaxfindOperationList(string cModuleID)
        {
            var tList = new List<SelectListItem>();

            tList = findOperationList(cModuleID);

            ViewBag.OperationList = tList;
            return Json(tList);
        }
        #endregion

        #region 傳入模組別並取得程式作業編號清單
        /// <summary>
        /// 傳入模組別並取得程式作業編號清單
        /// </summary>
        /// <param name="cModuleID">模組別</param>
        /// <returns></returns>
        public List<SelectListItem> findOperationList(string cModuleID)
        {
            List<string> tPSList = new List<string>();

            string tProductSeries = string.Empty;
            string ProductName = string.Empty;

            var beanProducts = psipDb.TbOneOperationParameters.OrderBy(x => x.COperationId).Where(x => x.Disabled == 0 && x.CModuleId == cModuleID);

            var ProductList = new List<SelectListItem>();
            ProductList.Add(new SelectListItem { Text = " ", Value = "" });

            foreach (var bean in beanProducts)
            {
                if (!tPSList.Contains(bean.COperationId))
                {
                    ProductName = bean.COperationId + "_" + bean.COperationName;

                    ProductList.Add(new SelectListItem { Text = ProductName, Value = bean.COperationId });
                    tPSList.Add(bean.COperationId);
                }
            }

            return ProductList;
        }
        #endregion

        #region Ajax傳入模組別和程式作業編號並取得系統GUID
        /// <summary>
        /// Ajax傳入模組別和程式作業編號並取得系統GUID
        /// </summary>
        /// <param name="cModuleID">模組別</param>
        /// <param name="cOperationID">程式作業編號</param>
        /// <returns></returns>
        public IActionResult AjaxfindOperationID(string cModuleID, string cOperationID)
        {
            string reValue = string.Empty;

            reValue = findOperationID(cModuleID, cOperationID);

            return Json(reValue);
        }
        #endregion

        #region 傳入模組別和程式作業編號並取得系統GUID
        /// <summary>
        /// 傳入模組別和程式作業編號並取得系統GUID
        /// </summary>
        /// <param name="cModuleID">模組別</param>
        /// <param name="cOperationID">程式作業編號</param>
        /// <returns></returns>
        public string findOperationID(string cModuleID, string cOperationID)
        {
            string reValue = string.Empty;

            var bean = psipDb.TbOneOperationParameters.FirstOrDefault(x => x.Disabled == 0 && x.CModuleId == cModuleID && x.COperationId == cOperationID);

            if (bean != null)
            {
                reValue = bean.CId.ToString();
            }

            return reValue;
        }
        #endregion

        #endregion -----資訊系統參數作業設定 End-----

        #region -----資訊系統角色權限作業設定 Start-----
        /// <summary>
        /// 資訊系統角色權限作業設定
        /// </summary>
        /// <returns></returns>
        public IActionResult RoleParameter()
        {
            #region 程式作業編號檔系統ID清單
            var ModuleList = findModuleIDList();

            var OperationList = new List<SelectListItem>();
            OperationList.Add(new SelectListItem { Text = " ", Value = "" });

            ViewBag.ModuleList = ModuleList;
            ViewBag.OperationList = OperationList;
            #endregion

            return View();
        }

        #region 資訊系統角色權限作業設定查詢結果
        /// <summary>
        /// 資訊系統角色權限作業設定查詢結果
        /// </summary>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cFunctionID">功能別</param>
        /// <param name="cCompanyID">公司別</param>        
        /// <param name="cValue">參數值</param>
        /// <param name="cDescription">參數值說明</param>        
        /// <returns></returns>
        public IActionResult RoleParameterResult(string cOperationID, string cFunctionID, string cCompanyID, string cValue, string cDescription)
        {
            List<string[]> QueryToList = new List<string[]>();    //查詢出來的清單

            #region 組待查詢清單
            var beans = psipDb.TbOneRoleParameters.OrderBy(x => x.COperationId).OrderBy(x => x.CFunctionId).OrderBy(x => x.CCompanyId).OrderBy(x => x.CValue)
                                               .Where(x => x.Disabled == 0 &&
                                                            (string.IsNullOrEmpty(cOperationID) ? true : x.COperationId.ToString() == cOperationID) &&
                                                            (string.IsNullOrEmpty(cCompanyID) ? true : x.CCompanyId == cCompanyID) &&                                                            
                                                            (string.IsNullOrEmpty(cFunctionID) ? true : x.CFunctionId == cFunctionID) &&
                                                            (string.IsNullOrEmpty(cValue) ? true : x.CValue.Contains(cValue.Trim())) &&
                                                            (string.IsNullOrEmpty(cDescription) ? true : x.CDescription.Contains(cDescription.Trim())));

            foreach (var bean in beans)
            {
                string[] QueryInfo = new string[12];

                QueryInfo[0] = bean.CId.ToString();                    //系統ID
                QueryInfo[1] = bean.COperationId.ToString();            //程式作業編號檔系統ID
                QueryInfo[2] = bean.CFunctionId;                        //功能別
                QueryInfo[3] = TransRoleFunctionID(bean.CFunctionId);   //功能別名稱
                QueryInfo[4] = bean.CCompanyId;                         //公司別                
                QueryInfo[5] = bean.CValue;                            //參數值        
                QueryInfo[6] = bean.CDescription;                       //參數值說明
                QueryInfo[7] = bean.CIncludeSubDept;                    //是否含子部門
                QueryInfo[8] = bean.CExeQuery;                         //是否可執行查詢
                QueryInfo[9] = bean.CExeInsert;                        //是否可執行新增
                QueryInfo[10] = bean.CExeEdit;                         //是否可執行編輯
                QueryInfo[11] = bean.CExeDel;                          //是否可執行刪除

                QueryToList.Add(QueryInfo);
            }

            ViewBag.QueryToListBean = QueryToList;
            #endregion

            return View();
        }
        #endregion

        #region 儲存資資訊系統角色權限設定檔
        /// <summary>
        /// 儲存資訊系統角色權限設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <param name="cOperationID">程式作業編號檔系統ID</param>
        /// <param name="cFunctionID">功能別</param>
        /// <param name="cCompanyID">公司別</param>        
        /// <param name="cValue">參數值</param>
        /// <param name="cDescription">參數值說明</param>
        /// <param name="cIncludeSubDept">是否含子部門(Y、N)</param>
        /// <param name="cExeQuery">是否可執行查詢(Y、N)</param>
        /// <param name="cExeInsert">是否可執行新增(Y、N)</param>
        /// <param name="cExeEdit">是否可執行編輯(Y、N)</param>
        /// <param name="cExeDel">是否可執行刪除(Y、N)</param>
        /// <returns></returns>
        public ActionResult SaveRole(string cID, string cOperationID, string cFunctionID, string cCompanyID, string cValue, string cDescription,
                                   string cIncludeSubDept, string cExeQuery, string cExeInsert, string cExeEdit, string cExeDel)
        {
            string tMsg = string.Empty;

            try
            {
                int result = 0;
                if (cID == null)
                {
                    #region -- 新增 --
                    var prBean = psipDb.TbOneRoleParameters.FirstOrDefault(x => x.Disabled == 0 &&
                                                                            x.COperationId.ToString() == cOperationID &&
                                                                            x.CFunctionId == cFunctionID &&
                                                                            x.CCompanyId == cCompanyID &&                                                                            
                                                                            x.CValue == cValue);
                    if (prBean == null)
                    {
                        TbOneRoleParameter prBean1 = new TbOneRoleParameter();
                        prBean1.COperationId = Guid.Parse(cOperationID);
                        prBean1.CFunctionId = cFunctionID;
                        prBean1.CCompanyId = cCompanyID;                        
                        prBean1.CValue = cValue.Trim();
                        prBean1.CDescription = cDescription.Trim();
                        prBean1.CIncludeSubDept = cIncludeSubDept == null ? "N" : cIncludeSubDept;
                        prBean1.CExeQuery = cExeQuery == null ? "N" : cExeQuery;
                        prBean1.CExeInsert = cExeInsert == null ? "N" : cExeInsert; 
                        prBean1.CExeEdit = cExeEdit == null ? "N" : cExeEdit;
                        prBean1.CExeDel = cExeDel == null ? "N" : cExeDel;
                        prBean1.Disabled = 0;

                        prBean1.CreatedUserName = "SYS";
                        prBean1.CreatedDate = DateTime.Now;

                        psipDb.TbOneRoleParameters.Add(prBean1);
                        result = psipDb.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此角色權限已存在，請重新輸入！";
                    }
                    #endregion                
                }
                else
                {
                    #region -- 編輯 --
                    var prBean = psipDb.TbOneRoleParameters.FirstOrDefault(x => x.Disabled == 0 &&
                                                                            x.CId.ToString() != cID &&
                                                                            x.COperationId.ToString() == cOperationID &&
                                                                            x.CFunctionId == cFunctionID &&
                                                                            x.CCompanyId == cCompanyID &&                                                                            
                                                                            x.CValue == cValue);
                    if (prBean == null)
                    {
                        var prBean1 = psipDb.TbOneRoleParameters.FirstOrDefault(x => x.CId.ToString() == cID);
                        prBean1.CValue = cValue.Trim();
                        prBean1.CDescription = cDescription.Trim();
                        prBean1.CIncludeSubDept = cIncludeSubDept == null ? "N" : cIncludeSubDept;
                        prBean1.CExeQuery = cExeQuery == null ? "N" : cExeQuery;
                        prBean1.CExeInsert = cExeInsert == null ? "N" : cExeInsert; 
                        prBean1.CExeEdit = cExeEdit == null ? "N" : cExeEdit;
                        prBean1.CExeDel = cExeDel == null ? "N" : cExeDel;

                        prBean1.ModifiedUserName = "SYS";
                        prBean1.ModifiedDate = DateTime.Now;
                        result = psipDb.SaveChanges();
                    }
                    else
                    {
                        tMsg = "此角色權限已存在，請重新輸入！";
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

        #region 刪除資訊系統角色權限設定檔
        /// <summary>
        /// 刪除資訊系統角色權限設定檔
        /// </summary>
        /// <param name="cID">系統ID</param>
        /// <returns></returns>
        public ActionResult DeleteRoleParameter(string cID)
        {
            var ctBean = psipDb.TbOneRoleParameters.FirstOrDefault(x => x.CId.ToString() == cID);
            ctBean.Disabled = 1;
            ctBean.ModifiedUserName = "SYS"; //EmpBean.EmployeeCName;
            ctBean.ModifiedDate = DateTime.Now;

            var result = psipDb.SaveChanges();

            return Json(result);
        }
        #endregion

        #region 角色權限功能別ID轉換成名稱
        /// <summary>
        /// 角色權限功能別ID轉換成名稱
        /// </summary>
        /// <param name="cFunctionID">模組別</param>
        /// <returns></returns>
        private string TransRoleFunctionID(string cFunctionID)
        {
            string reValue = string.Empty;

            switch (cFunctionID.ToUpper())
            {
                case "DEPT":
                    reValue = "依部門";
                    break;
                case "PERSON":
                    reValue = "依人員";
                    break;                
            }

            return reValue;
        }
        #endregion

        #endregion -----資訊系統角色權限作業設定 End-----

        #region -----共用參數 Start-----

        #region 公司別轉換
        /// <summary>
        /// 公司別轉換
        /// </summary>
        /// <param name="CompanyID">T012、T016、C069、T022</param>
        /// <returns></returns>
        public string TrnasCompanyID(string CompanyID)
        {
            string reValue = string.Empty;

            switch(CompanyID)
            {
                case "T012":
                    reValue = "Comp-1";
                    break;
                case "T016":
                    reValue = "Comp-2";
                    break;
                case "C069":
                    reValue = "Comp-3";
                    break;
                case "T022":
                    reValue = "Comp-4";
                    break;
            }

            return reValue;
        }
        #endregion       

        #region Ajax用中文或英文姓名查詢人員
        /// <summary>
        /// Ajax用中文或英文姓名查詢人員
        /// </summary>
        /// <param name="keyword">中文/英文姓名</param>
        /// <param name="CompanyID">公司別</param>
        /// <returns></returns>
        public IActionResult AjaxfindEmployeeByKeyword(string keyword, string CompanyID)
        {
            string compcde = TrnasCompanyID(CompanyID);

            object contentObj = null;            

            contentObj = bpmDB.TblEmployees.Where(x => (string.IsNullOrEmpty(compcde) ? true : x.CEmployeeCompanyCode.Trim() == compcde) &&
                                                    (x.CEmployeeAccount.Contains(keyword) || x.CEmployeeCName.Contains(keyword)) &&
                                                    (x.CEmployeeLeaveReason == null && x.CEmployeeLeaveDay == null)).Take(5);



            string json = JsonConvert.SerializeObject(contentObj);
            return Content(json, "application/json");
        }
        #endregion

        #region Ajax用部門代號/名稱查詢部門代號
        /// <summary>
        /// Ajax用部門代號/名稱查詢部門代號
        /// </summary>
        /// <param name="keyword">中文/英文姓名</param>
        /// <param name="CompanyID">公司別</param>
        /// <returns></returns>
        public IActionResult AjaxfindDeptByKeyword(string keyword, string CompanyID)
        {
            string compcde = CompanyID == "ALL" ? "" : CompanyID.Substring(2, 2);

            var beans = bpmDB.TblDepartments.Where(x => x.CDepartmentIsEnable == true && 
                                                    (string.IsNullOrEmpty(compcde) ? true : x.Pk.Trim().Substring(0, 2) == compcde) &&
                                                    (x.CDepartmentName.Contains(keyword) || x.Pk.Contains(keyword))).Take(10);

            
            return Json(beans);
        }
        #endregion

        #endregion -----共用參數 End-----
    }
}
