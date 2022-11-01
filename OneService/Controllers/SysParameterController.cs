using Microsoft.AspNetCore.Mvc;
using OneService.Models;
using System.ComponentModel.DataAnnotations;

namespace OneService.Controllers
{
	public class SysParameterController : Controller
	{        
        PSIPContext psipDb = new PSIPContext();

        #region 資訊系統作業設定
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
            var beans = psipDb.TbOneOperationParameters.OrderBy(x => x.CModuleId).Where(x => x.Disabled == 0 &&
                                                           (string.IsNullOrEmpty(cModuleID) ? true : x.CModuleId == cModuleID) &&
                                                           (string.IsNullOrEmpty(cOperationID) ? true : x.COperationId.Contains(cOperationID)) &&
                                                           (string.IsNullOrEmpty(cOperationName) ? true : x.COperationName.Contains(cOperationName)));

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
        public ActionResult SaveSYS(string cID, string cModuleID, string cOperationID, string cOperationName, string cOperationURL)
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
                        prBean1.COperationId = cOperationID;
                        prBean1.COperationName = cOperationName;
                        prBean1.COperationUrl = cOperationURL == null ? "" : cOperationURL;
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
                    prBean.COperationName = cOperationName;
                    prBean.COperationUrl = cOperationURL == null ? "": cOperationURL;

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
            var ctBean = psipDb.TbOneOperationParameters.FirstOrDefault(x => x.CId.ToString() == cID);
            ctBean.Disabled = 1;
            ctBean.ModifiedUserName = "SYS"; //EmpBean.EmployeeCName;
            ctBean.ModifiedDate = DateTime.Now;

            var result = psipDb.SaveChanges();

            return Json(result);
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

        #endregion
    }
}
