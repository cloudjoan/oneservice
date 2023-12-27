using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneService.Models;
using OneService.Utils;
using System.Data;
using System.Security.Policy;
using static OneService.Controllers.ServiceRequestController;

namespace OneService.Controllers
{
    public class LuckyDrawController : Controller
    {
        MCSWorkflowContext  eipDB       = new MCSWorkflowContext();
        CommonFunction      CMF         = new CommonFunction();
        APP_DATAContext     AppDataDB   = new APP_DATAContext();

        /// <summary>
        /// 登入者帳號
        /// </summary>
        string pLoginAccount = string.Empty;

        /// <summary>
        /// 登入者是否為MIS(true.是 false.否)
        /// </summary>
        bool pIsMIS = false;

        /// <summary>
        /// 登入者是否為HR(true.是 false.否)
        /// </summary>
        bool pIsHR = false;

        /// <summary>
        /// 登入者是否有權限異動資料(true.是 false.否)
        /// </summary>
        bool pIsEdit = false;

        /// <summary>
        /// 抽獎活動ID
        /// </summary>
        int pDrawID = 0;

        /// <summary>
        /// 抽獎活動ID
        /// </summary>
        string pDrawName = "";

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

		/// <summary>
		/// 登入者是否為管理者(true.是 false.否)
		/// </summary>
		bool pIsManager = false;

		#region 取得登入帳號權限
		/// <summary>
		/// 取得登入帳號權限
		/// </summary>
		public void getLoginAccount()
        {
            #region 測試用
            //pLoginAccount = @"etatung\elvis.chang";     //MIS
            //pLoginAccount = @"etatung\Allen.Chen";      //陳勁嘉(主管)
            //pLoginAccount = @"etatung\Boyen.Chen";      //陳建良(主管)
            //pLoginAccount = @"etatung\Aniki.Huang";     //黃志豪(主管)
            //pLoginAccount = @"etatung\jack.hung";       //洪佑典(主管)
            //pLoginAccount = @"etatung\Steve.Guo";       //郭翔元         
            //pLoginAccount = @"etatung\Jordan.Chang";    //張景堯
            #endregion

            pLoginAccount = HttpContext.Session.GetString(SessionKey.USER_ACCOUNT); //正式用

            #region One Service相關帳號
            pIsMIS = CMF.getIsMIS(pLoginAccount, pSysOperationID);
            pIsHR   = CMF.getParaAuthority(pLoginAccount, pSysOperationID, "ALL", "HR");
            pIsEdit = (pIsMIS == true || pIsHR == true) ? true : false;

            ViewBag.pIsMIS  = pIsMIS;
            ViewBag.pIsHR   = pIsHR;
            ViewBag.pIsEdit = pIsEdit;
            #endregion
        }
		#endregion

		#region 取得人員相關資料
		public void getEmployeeInfo()
		{
			CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
			EmpBean = CMF.findEmployeeInfo(pLoginAccount);

			ViewBag.cLoginUser_Name             = EmpBean.EmployeeCName;
			ViewBag.cLoginUser_EmployeeNO       = EmpBean.EmployeeNO;
			ViewBag.cLoginUser_ERPID            = EmpBean.EmployeeERPID;
			ViewBag.cLoginUser_WorkPlace        = EmpBean.WorkPlace;
			ViewBag.cLoginUser_DepartmentName   = EmpBean.DepartmentName;
			ViewBag.cLoginUser_DepartmentNO     = EmpBean.DepartmentNO;
			ViewBag.cLoginUser_ProfitCenterID   = EmpBean.ProfitCenterID;
			ViewBag.cLoginUser_CostCenterID     = EmpBean.CostCenterID;
			ViewBag.cLoginUser_CompCode         = EmpBean.CompanyCode;
			ViewBag.cLoginUser_BUKRS            = EmpBean.BUKRS;
			ViewBag.pIsManager                  = EmpBean.IsManager;
			ViewBag.empEngName                  = EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName;

			pCompanyCode    = EmpBean.BUKRS;
			pIsManager      = EmpBean.IsManager;
		}
		#endregion

		public IActionResult LuckyDrawPrize()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();
            getEmployeeInfo();

            string  DrawYear        = DateTime.Today.ToString("yyyy");
            bool    tIsFormal       = false;
            string  tONEURLName     = string.Empty;
            string  tAttachURLName  = string.Empty;

            #region Request參數       
            pDrawID = 0;
            if (HttpContext.Request.Query["DrawID"].FirstOrDefault() != null)
            {
                pDrawID = int.Parse(HttpContext.Request.Query["DrawID"].FirstOrDefault());                
            }

            pDrawName = "";
            if (HttpContext.Request.Query["DrawName"].FirstOrDefault() != null)
            {
                pDrawName = HttpContext.Request.Query["DrawName"].FirstOrDefault();
            }

            if (HttpContext.Request.Query["tMessage"].FirstOrDefault() != null)
            {
                //記錄匯入Excel成功或失敗訊息！
                ViewBag.Message = HttpContext.Request.Query["tMessage"].FirstOrDefault();
            }
            #endregion  

            #region 取得系統位址參數相關資訊
            SRSYSPARAINFO ParaBean = CMF.findSRSYSPARAINFO(pOperationID_GenerallySR);

            tIsFormal = ParaBean.IsFormal;

            tONEURLName     = ParaBean.ONEURLName;
            tAttachURLName  = ParaBean.AttachURLName;

            if (tIsFormal)
            {
                ViewBag.DownloadURL = tONEURLName + "/files/LuckyDraw_Example.XLSX";
            }
            else
            {
                ViewBag.DownloadURL = "http://" + tAttachURLName + "/files/LuckyDraw_Example.XLSX";
            }
            #endregion

            DrawYearCH(DrawYear);

            ViewBag.DrawYear    = DrawYear;
            ViewBag.DrawID      = pDrawID;
            ViewBag.DrawName    = pDrawName;

            return View();
        }

        #region 抽獎獎品查詢結果
        /// <summary>
        /// 抽獎獎品查詢結果
        /// </summary>
        /// <param name="DrawID">抽獎活動編號</param>
        /// <returns></returns>
        public IActionResult QueryLuckyDrawPrizeResult(int DrawID, string DrawName, string DrawYear)
        {
            getLoginAccount();
            getEmployeeInfo();
            
            var PrizeList = AppDataDB.TbLuckydrawPrizes.Where(x => x.DrawId == DrawID && x.DisabledMark == false).OrderBy(x => x.SortNo).ToList();

            ViewBag.PrizeList       = PrizeList;
            ViewBag.DrawID2         = DrawID;
            ViewBag.DrawName        = DrawName;
            ViewBag.DrawYear2       = DrawYear;

            return View();
        }
        #endregion

        #region 儲存獎品項目
        [HttpPost]
        public IActionResult SavePrize(IFormCollection formCollection)
        
        {
            string tErrorMsg = string.Empty;

            int     DrawID      = int.Parse(formCollection["hid_DrawID"].FirstOrDefault());
            string  DrawName    = formCollection["hid_DrawName"].FirstOrDefault();
            string  DrawYear    = formCollection["hid_DrawYear"].FirstOrDefault();

            string[] PrizeID        = formCollection["hid_PrizeID"];
            string[] txtPrizeName   = formCollection["txtPrizeName"];
            string[] txtPrizePrice  = formCollection["txtPrizePrice"];
            string[] txtPrizeAmount = formCollection["txtPrizeAmount"];
            string[] attach         = formCollection["hid_filezone"];
            string[] OverAYearMark  = formCollection["hid_OverAYear"];

            getLoginAccount();
            getEmployeeInfo();

            try
            {
                if (PrizeID != null && PrizeID.Length > 0) 
                {
                    int result = 0;

                    #region 抽獎主檔
                    if (DrawID == 0) // 新增
                    {
                        TbLuckydraw bean = new TbLuckydraw();

                        bean.DrawYear   = DrawYear;
                        bean.DrawName   = DrawName;
                        bean.InsertUser = ViewBag.cLoginUser_ERPID;
                        bean.InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                        AppDataDB.TbLuckydraws.Add(bean);

                        result = AppDataDB.SaveChanges();

                        DrawID = bean.DrawId;
                    }
                    else // 修改
                    {
                        var bean = AppDataDB.TbLuckydraws.Where(x => x.DrawId == DrawID && x.DisabledMark == false).FirstOrDefault();

                        if (bean != null) 
                        {
                            bean.DrawName   = DrawName;
                            bean.ModifyUser = ViewBag.cLoginUser_ERPID;
                            bean.ModifyTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                            result = AppDataDB.SaveChanges();
                        }                        
                    }
                    #endregion

                    if (result > 0)
                    {
                        int Count = PrizeID.Length;

                        for (int i = 0; i < Count; i++)
                        {
                            #region 抽獎獎品
                            if (PrizeID[i] == "0") // 新增
                            {
                                TbLuckydrawPrize bean = new TbLuckydrawPrize();

                                bean.DrawId         = DrawID;
                                bean.SortNo         = i + 1;
                                bean.PrizeName      = txtPrizeName[i];
                                bean.PrizePic       = attach[i];
                                bean.PrizeAmount    = int.Parse(txtPrizeAmount[i]);
                                bean.OverAyearMark  = OverAYearMark[i] == "Y" ? true : false;
                                bean.PrizePrice     = int.Parse(txtPrizePrice[i]);
                                bean.DrawAmount     = 0;
                                bean.InsertUser     = ViewBag.cLoginUser_ERPID;
                                bean.InsertTime     = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                                AppDataDB.TbLuckydrawPrizes.Add(bean);
                            }
                            else // 修改
                            {
                                var bean = AppDataDB.TbLuckydrawPrizes.Where(x => x.PrizeId == int.Parse(PrizeID[i])).FirstOrDefault();

                                if (bean != null)
                                {
                                    bean.SortNo         = i + 1;
                                    bean.PrizeName      = txtPrizeName[i];
                                    bean.PrizePic       = attach[i];
                                    bean.PrizeAmount    = int.Parse(txtPrizeAmount[i]);
                                    bean.OverAyearMark  = OverAYearMark[i] == "Y" ? true : false;
                                    bean.PrizePrice     = int.Parse(txtPrizePrice[i]);
                                    bean.ModifyUser     = ViewBag.cLoginUser_ERPID;
                                    bean.ModifyTime     = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                }
                            }
                            #endregion
                        }
                        result = AppDataDB.SaveChanges();

                        if (result <= 0) 
                        {
                            tErrorMsg += "年度【" + DrawYear + "】活動【" + DrawName + "】，異動抽獎獎品失敗！" + Environment.NewLine;
                            CMF.writeToLog("DrawID：" + DrawID, "savePrize", tErrorMsg, ViewBag.empEngName);
                        }
                    }
                    else
                    {
                        tErrorMsg += "年度【" + DrawYear + "】活動【" + DrawName + "】，異動抽獎主檔失敗！" + Environment.NewLine;
                        CMF.writeToLog("DrawID：" + DrawID, "savePrize", tErrorMsg, ViewBag.empEngName);
                    }
                }
            }
            catch (Exception e)
            {
                //SendMailByAPI("PSO儲存支援紀錄", null, "Elvis.Chang@etatung.com;Joy.Chi@etatung.com", "", "", "PSO儲存支援紀錄_error", oppNo + DateTime.Now.ToString("_yyyy-MM-dd HH:mm:ss") + "<br>" + e.ToString(), null, null);
            }

            return RedirectToAction("LuckyDrawPrize", new { DrawID = DrawID, DrawName = DrawName, DrawYear = DrawYear });
        }
        #endregion

        #region 匯入抽獎獎品Excel
        [HttpPost]
        public IActionResult ImportExcel(IFormCollection formCollection, IFormFile postedFile)
        {
            string  tLog            = string.Empty;
            string  tErrorMsg       = string.Empty;
			string  DrawYear        = string.Empty;
			string  txtDrawName     = string.Empty;
			int     ddlDrawID       = 0;
            int     DrawID          = 0;
			int     cSortNo         = 0;
            int     cPrizePrice     = 0;
            int     cPrizeAmount    = 0;
			string  cPrizeName      = string.Empty;
            bool    cOverAYearMark  = false;

            Dictionary<string, DataTable> Dic = new Dictionary<string, DataTable>();
            DataTable dt = new DataTable();

            getLoginAccount();
            getEmployeeInfo();

			try
            {
				DrawYear    = formCollection["ddlDrawYear"].FirstOrDefault();
				txtDrawName = formCollection["txtDrawName"].FirstOrDefault();
                ddlDrawID   = int.Parse(formCollection["ddlDrawName"].FirstOrDefault());

				#region 取得匯入Excel相關
				Dic = CMF.ImportExcelToDataTable(postedFile, "預計獎品", 0);

                foreach (KeyValuePair<string, DataTable> item in Dic)
                {
                    #region 回傳結果訊息
                    tErrorMsg = item.Key;
                    #endregion

                    #region 回傳的DataTable
                    dt = item.Value.Clone();

                    foreach (DataRow dr in item.Value.Rows)
                    {
                        dt.Rows.Add(dr.ItemArray);
                    }
                    #endregion

                    break;
                }
                #endregion

                if (tErrorMsg == "")
                {
                    bool tIsDoADD = true;

                    #region 先儲存抽獎主檔
                    int result = 0;
                    if (ddlDrawID == 0)
                    { 
                        TbLuckydraw bean = new TbLuckydraw();

                        bean.DrawYear   = DrawYear;
                        bean.DrawName   = txtDrawName;
                        bean.InsertUser = ViewBag.cLoginUser_ERPID;
                        bean.InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                        AppDataDB.TbLuckydraws.Add(bean);

                        result = AppDataDB.SaveChanges();

                        DrawID = bean.DrawId;
                    }
                    else
                    {
                        var bean = AppDataDB.TbLuckydraws.Where(x => x.DrawId == ddlDrawID).FirstOrDefault();

                        if (bean != null)
                        {
                            bean.DrawName   = txtDrawName;
                            bean.ModifyUser = ViewBag.cLoginUser_ERPID;
                            bean.ModifyTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                            result = AppDataDB.SaveChanges();

                            DrawID = bean.DrawId;
                        }                        
                    }

                    if (result <= 0)
                    {
                        tErrorMsg += "年度【" + DrawYear + "】活動【" + txtDrawName + "】，異動抽獎主檔失敗！" + Environment.NewLine;
                        CMF.writeToLog("DrawID：" + DrawID, "saveLuckyDrawPrizeByExcel", tErrorMsg, ViewBag.empEngName);
                        tIsDoADD = false;
                    }
                    #endregion

                    #region 寫入DataTable到資料庫    
                    if (tIsDoADD)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            try
                            {
                                cSortNo         = int.Parse(dr[0].ToString());
                                cPrizeName      = dr[1].ToString();
                                cPrizePrice     = int.Parse(dr[2].ToString());
                                cPrizeAmount    = int.Parse(dr[3].ToString());
                                cOverAYearMark  = dr[4].ToString().ToUpper() == "Y" ? true : false;                                

								var bean = AppDataDB.TbLuckydrawPrizes.Where(x => x.DisabledMark == false && x.DrawId == DrawID && x.PrizeName == cPrizeName).FirstOrDefault();

                                if (bean != null)
                                {

                                    bean.SortNo         = cSortNo;
                                    bean.PrizeName      = cPrizeName;
                                    bean.PrizeAmount    = cPrizeAmount;
                                    bean.OverAyearMark  = cOverAYearMark;                                    
                                    bean.PrizePrice     = cPrizePrice;
									bean.ModifyUser     = ViewBag.cLoginUser_ERPID;
                                    bean.ModifyTime     = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                }
                                else
                                {
									TbLuckydrawPrize beanN = new TbLuckydrawPrize();

                                    beanN.DrawId        = DrawID;
									beanN.SortNo        = cSortNo;
									beanN.PrizeName     = cPrizeName;
									beanN.PrizeAmount   = cPrizeAmount;
									beanN.OverAyearMark = cOverAYearMark;
									beanN.PrizePrice    = cPrizePrice;
                                    beanN.DrawAmount    = 0;
									beanN.InsertUser    = ViewBag.cLoginUser_ERPID;
									beanN.InsertTime    = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

									AppDataDB.TbLuckydrawPrizes.Add(beanN);
								}								

                                int result2 = AppDataDB.SaveChanges();

                                if (result2 <= 0)
                                {
                                    tErrorMsg += "年度【" + DrawYear + "】活動【" + txtDrawName + "】，儲存獎品明細資料庫失敗！" + Environment.NewLine;
                                    CMF.writeToLog("DrawID：" + DrawID, "saveLuckyDrawPrizeByExcel", tErrorMsg, ViewBag.empEngName);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(tLog))
                                    {
                                        CMF.writeToLog("DrawID：" + DrawID, "saveLuckyDrawPrizeByExcel", tLog, ViewBag.empEngName);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                tErrorMsg += "年度【" + DrawYear + "】活動【" + txtDrawName + "】，儲存獎品明細資料庫失敗！" + e.Message + Environment.NewLine;
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
                    ViewBag.Message = "匯入失敗！原因：" + Environment.NewLine + tErrorMsg;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "匯入失敗！原因：" + ex.Message;
            }

            return RedirectToAction("LuckyDrawPrize", new { tMessage = ViewBag.Message, DrawID = DrawID, DrawName = txtDrawName, DrawYear = DrawYear });
        }
        #endregion

        public IActionResult DrawYearCH(string Draw_Year)
        {
            getLoginAccount();
            getEmployeeInfo();

            var DrawNameList = new List<SelectListItem>();

            DrawNameList.Clear();

            if (pIsEdit == true)
            { DrawNameList.Add(new SelectListItem { Value = "0", Text = "[新增活動]" }); }
            else
            { DrawNameList.Add(new SelectListItem { Value = "0", Text = "請選擇" }); }

            var LuckyDraws = AppDataDB.TbLuckydraws.Where(x => x.DrawYear == Draw_Year && x.DisabledMark == false).Select(x => new SelectListItem { Value = x.DrawId.ToString(), Text = x.DrawName }).ToList();
            foreach (var LuckyDraw in LuckyDraws)
            {
                DrawNameList.Add(LuckyDraw);
            }
            //DrawNameList.First().Selected = true; // 預設第一筆

            ViewBag.DrawNameList = DrawNameList;      

            if (DrawNameList != null)
            { return Json(DrawNameList); }
            else
            { return Json("Empty"); }
        }

        public IActionResult DeletePrize(int PrizeID)
        {
            getLoginAccount();
            getEmployeeInfo();

            int result = 0;

            try
            {
                var bean = AppDataDB.TbLuckydrawPrizes.FirstOrDefault(x => x.PrizeId == PrizeID);
                if (bean != null)
                {
                    bean.DisabledMark   = true;
                    bean.ModifyUser     = ViewBag.cLoginUser_ERPID;
                    bean.ModifyTime     = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                    result = AppDataDB.SaveChanges();
                }
            }
            catch (Exception e)
            {
                CMF.writeToLog("PrizeID：" + PrizeID, "DeletePrize", e.ToString(), ViewBag.empEngName);
            }

            return Json(result);
        }

        public IActionResult GetWinningList(int PrizeID)
        {
            getLoginAccount();
            getEmployeeInfo();

            var result = AppDataDB.TbLuckydrawPrizewinnings.Where(x => x.PrizeId == PrizeID && x.DisabledMark == false).OrderBy(x => x.InsertTime).ToList();

            return Json(result);
        }
    }
}
