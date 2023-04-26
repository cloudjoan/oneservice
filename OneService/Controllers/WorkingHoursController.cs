using Microsoft.AspNetCore.Mvc;
using OneService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Linq;
using NuGet.Packaging.Signing;
using OneService.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using Org.BouncyCastle.Utilities;

namespace OneService.Controllers
{
    public class WorkingHoursController : Controller
    {
        BIContext biDB = new BIContext();
        PSIPContext psipDB = new PSIPContext();
        TAIFContext bpmDB = new TAIFContext();
        MCSWorkflowContext eipDB = new MCSWorkflowContext();
        CommonFunction CMF = new CommonFunction();
        ERP_PROXY_DBContext proxyDB = new ERP_PROXY_DBContext();

        /// <summary>
        /// 登入者帳號
        /// </summary>
        string pLoginAccount = string.Empty;
       
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
        /// 程式作業編號檔系統ID(ALL，固定的GUID)
        /// </summary>
        string pSysOperationID = "F8EFC55F-FA77-4731-BB45-2F2147244A2D";

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();

            var userAccount = User.Identity.Name;
            System.Diagnostics.Debug.WriteLine(userAccount);

            var client = new RestClient("http://tsti-sapapp01.etatung.com.tw/api/ticc");

            var request = new RestRequest();
            request.Method = RestSharp.Method.Post;

            Dictionary<Object, Object> parameters = new Dictionary<Object, Object>();
            parameters.Add("SAP_FUNCTION_NAME", "ZFM_TICC_SERIAL_SEARCH");
            parameters.Add("IV_SERIAL", "2CE3231K6X");

            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", parameters, ParameterType.RequestBody);

            RestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);

            var data = (JObject)JsonConvert.DeserializeObject(response.Content);

            System.Diagnostics.Debug.WriteLine(data["ET_REQUEST"]["SyncRoot"][0]["cNAMEField"]);

            ViewBag.deptName = HttpContext.Session.GetString(SessionKey.DEPT_NAME);
            ViewBag.userName = HttpContext.Session.GetString(SessionKey.USER_NAME);
            ViewBag.userErpId = HttpContext.Session.GetString(SessionKey.USER_ERP_ID);

            return View();
        }

        public IActionResult GetSRLabor(string erpId, string whType, string startDate, string endDate)
        {

            try
            {
                var viewWHBeans = psipDB.ViewWorkingHours.Where(x => x.UserErpId == erpId && (string.IsNullOrEmpty(whType) ? true : x.Whtype == whType)).ToList();

                viewWHBeans = viewWHBeans.Where(x => string.IsNullOrEmpty(startDate) ? true : Convert.ToDateTime(x.FinishTime) >= Convert.ToDateTime(startDate + " 00:00")).ToList();
                viewWHBeans = viewWHBeans.Where(x => string.IsNullOrEmpty(endDate) ? true : Convert.ToDateTime(x.FinishTime) <= Convert.ToDateTime(endDate + " 23:59")).ToList();

                ViewBag.viewWHBeans = viewWHBeans;

            }
            catch (Exception e)
            {
                ViewBag.viewWHBeans = null;
            }

            return View();

        }

        public IActionResult CreateWH()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
            {
                return RedirectToAction("Login", "Home");
            }

            getLoginAccount();

            ViewBag.now = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
            ViewBag.deptName = HttpContext.Session.GetString(SessionKey.DEPT_NAME);
            ViewBag.userName = HttpContext.Session.GetString(SessionKey.USER_NAME);
            ViewBag.userErpId = HttpContext.Session.GetString(SessionKey.USER_ERP_ID);
            ViewBag.session = HttpContext.Session;

            var beans = psipDB.TbWorkingHoursMains.Where(x => x.UserErpId == HttpContext.Session.GetString(SessionKey.USER_ERP_ID) && x.Disabled != 1).OrderByDescending(x => x.Id);
            ViewBag.beans = beans;

            //查詢自己有被assign的商機
            //var supportCrmBeans = psipDB.ViewProSupportEmps.Where(x => x.SupErpId == "10000542").Select(x => new SelectListItem { Value = x.CrmOppNo,  Text = x.CrmOppNo + "-" + x.Description, Selected = true }).ToList();
            ViewBag.supportCrmBeans = psipDB.ViewProSupportEmps.Where(x => x.SupErpId == HttpContext.Session.GetString(SessionKey.USER_ERP_ID));

            return View();
        }

        public IActionResult SaveWH(IFormCollection formCollection)
        {
            TbWorkingHoursMain bean;
            int prId = 0;
            if (!string.IsNullOrEmpty(formCollection["Id"]))
            {
                bean = psipDB.TbWorkingHoursMains.Find(int.Parse(formCollection["Id"]));
                bean.Whtype = formCollection["ddl_WHType"].ToString();
                bean.ActType = formCollection["ddl_ActType"].ToString();
                bean.CrmOppNo = formCollection["hid_CrmOppNo"].ToString();
                bean.CrmOppName = formCollection["hid_CrmOppName"].ToString();
                bean.WhDescript = formCollection["tbx_WhDescript"].ToString();
                bean.StartTime = formCollection["tbx_StartDate"].ToString() + " " + formCollection["hid_StartTime"].ToString();
                bean.EndTime = formCollection["tbx_EndDate"].ToString() + " " + formCollection["hid_EndTime"].ToString();
                bean.UpdateTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                bean.ModifyUser = HttpContext.Session.GetString(SessionKey.USER_NAME);
                prId = bean.PrId ?? 0;

                //計算工時分鐘數
                DateTime startTime = Convert.ToDateTime(bean.StartTime);
                DateTime endTime = Convert.ToDateTime(bean.EndTime);
                TimeSpan ts = endTime - startTime;
                bean.Labor = Convert.ToInt32(ts.TotalMinutes);

                bean.ActType = string.IsNullOrEmpty(bean.ActType) ? "L" : bean.ActType;
            }
            else
            {
                bean = new TbWorkingHoursMain();
                bean.UserName = formCollection["tbx_UserName"].ToString();
                bean.UserErpId = formCollection["hid_UserErpId"].ToString();
                bean.Whtype = formCollection["ddl_WHType"].ToString();
                bean.ActType = formCollection["ddl_ActType"].ToString();
                bean.CrmOppNo = formCollection["hid_CrmOppNo"].ToString();
                bean.CrmOppName = formCollection["hid_CrmOppName"].ToString();
                bean.WhDescript = formCollection["tbx_WhDescript"].ToString();
                bean.StartTime = formCollection["tbx_StartDate"].ToString() + " " + formCollection["hid_StartTime"].ToString();
                bean.EndTime = formCollection["tbx_EndDate"].ToString() + " " + formCollection["hid_EndTime"].ToString();
                bean.InsertTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                bean.ModifyUser = HttpContext.Session.GetString(SessionKey.USER_NAME);

                //計算工時分鐘數
                DateTime startTime = Convert.ToDateTime(bean.StartTime);
                DateTime endTime = Convert.ToDateTime(bean.EndTime);
                TimeSpan ts = endTime - startTime;
                bean.Labor = Convert.ToInt32(ts.TotalMinutes);

                bean.ActType = string.IsNullOrEmpty(bean.ActType) ? "L" : bean.ActType;

                psipDB.TbWorkingHoursMains.Add(bean);
            }

            //如果有商機跟專案管理的話，就需加入專案管理的工時計算
            if (!string.IsNullOrEmpty(bean.CrmOppNo))
            {
                var pjInfoBean = psipDB.TbProPjinfos.FirstOrDefault(x => x.CrmOppNo == bean.CrmOppNo);

                //取得MileStone
                string ms = GetMileStone(bean.CrmOppNo, bean.StartTime, bean.EndTime);

                var workHours = Math.Ceiling((decimal)bean.Labor / 60);

                //int? prId, string oppNo, string bundleMs, string bundleTask, string impBy, string ImplementersCount, string Attendees, string place, string startDate, string endDate, string workHours, string withPpl, string withPplPhone, string desc, string attach)
                var _prId = SavePjRecord(prId, bean.CrmOppNo, ms, "", "", "1", "", "", bean.StartTime, bean.EndTime, workHours.ToString(), "", "", bean.WhDescript, "");

                bean.PrId = _prId;
            }

            psipDB.SaveChanges();

            var actionType = formCollection["actionType"].ToString() ?? "NONE";

            if (actionType == "AJAX")
            {
                return Json("Finish");
            }

            return RedirectToAction("CreateWH");
        }

        public IActionResult GetWHById(int id)
        {
            var bean = psipDB.TbWorkingHoursMains.Find(id);
            return Json(bean);
        }

        public IActionResult DeleteWHById(int id)
        {
            var bean = psipDB.TbWorkingHoursMains.Find(id);
            bean.Disabled = 1;

            //如果有prId的話，也要刪掉PMO的工時
            if (bean.PrId != null)
            {
                var pjRecordBean = psipDB.TbProPjRecords.Find(bean.PrId);
                pjRecordBean.Disabled = 1;
            }

            psipDB.SaveChanges();

            return Json("OK");
        }


        #region -- 儲存專案執行紀錄 --
        /// <summary>儲存專案執行紀錄</summary>
        public int? SavePjRecord(int? prId, string oppNo, string bundleMs, string bundleTask, string impBy, string ImplementersCount, string Attendees, string place, string startDate, string endDate, string workHours, string withPpl, string withPplPhone, string desc, string attach)
        {
            #region -- 取得登入者資訊 --

            string userAccount = HttpContext.Session.GetString(SessionKey.USER_ACCOUNT);
            //取得登入者資訊            
            Person empBean = eipDB.People.FirstOrDefault(x => x.Account == userAccount && string.IsNullOrEmpty(x.LeaveReason));
            ViewEmpInfo empInfoBean = eipDB.ViewEmpInfos.FirstOrDefault(x => x.Account == userAccount);

            #endregion

            try
            {
                int result = 0;
                if (prId == 0) //新增
                {
                    #region -- 儲存專案執行紀錄 --
                    TbProPjRecord prBean = new TbProPjRecord();
                    prBean.CrmOppNo = OppNoFormat(oppNo);
                    prBean.BundleMs = bundleMs;
                    prBean.BundleTask = !string.IsNullOrEmpty(bundleTask) ? bundleTask : "";
                    prBean.Implementers = empBean.Account;
                    prBean.ImplementersCount = Convert.ToInt32(ImplementersCount);
                    prBean.Attendees = Attendees;   //edit by elvis 2022/07/18
                    prBean.Place = place;
                    prBean.StartDatetime = startDate; //Convert.ToDateTime(startDate).ToString("yyyy-MM-dd HH:mm:ss")
                    prBean.EndDatetime = endDate;
                    prBean.WorkHours = Convert.ToInt32(workHours);
                    prBean.TotalWorkHours = prBean.ImplementersCount * prBean.WorkHours;
                    prBean.WithPpl = withPpl;
                    prBean.WithPplPhone = withPplPhone;
                    prBean.Description = desc;
                    prBean.Attachment = attach;

                    prBean.CrErpId = empBean.ErpId;
                    prBean.CrAccount = empBean.Account.ToLower();
                    prBean.CrName = empBean.Name2;
                    prBean.CrEmail = empBean.Email;
                    prBean.CrCompCode = empBean.CompCde;
                    prBean.CrDeptId = empBean.DeptId;
                    prBean.CrDeptName = empInfoBean.DeptName;
                    prBean.Disabled = 0; // 0: false; 1: true(disabled)
                    prBean.InsertTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    prBean.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    psipDB.TbProPjRecords.Add(prBean);
                    psipDB.SaveChanges();

                    result = psipDB.TbProPjRecords.Max(x => x.Id);

                    //有設定任務編號，則要將執行紀錄的合計總工時，更新到任務統計的實際工時
                    if (!string.IsNullOrEmpty(prBean.BundleTask))
                    {
                        var qSameTask = psipDB.TbProPjRecords.Where(x => x.BundleMs == prBean.BundleMs && x.BundleTask == prBean.BundleTask && x.Disabled == 0).ToList();
                        if (qSameTask != null && qSameTask.Count > 0)
                        {
                            int sumSameTaskWorkHour = 0;
                            foreach (var pRecord in qSameTask)
                            {
                                sumSameTaskWorkHour += Convert.ToInt32(pRecord.TotalWorkHours);
                            }

                            int stair1No = Convert.ToInt32(prBean.BundleTask.Split('.')[0]);
                            int stair2No = prBean.BundleTask.Contains(".") ? Convert.ToInt32(prBean.BundleTask.Split('.')[1]) : 0;
                            int stair3No = 0;
                            if (prBean.BundleTask.Contains("."))
                            {
                                int tLen = prBean.BundleTask.Split('.').Length;

                                if (tLen >= 3)
                                {
                                    stair3No = Convert.ToInt32(prBean.BundleTask.Split('.')[2]);
                                }
                            }

                            var qTask = psipDB.TbProTasks.FirstOrDefault(x => x.Milestone == prBean.BundleMs && x.Stair1No == stair1No && x.Stair2No == stair2No && x.Stair3No == stair3No && x.Disabled == 0);
                            if (qTask != null)
                            {
                                qTask.WorkHours = sumSameTaskWorkHour;
                                psipDB.SaveChanges();
                            }
                        }
                    }
                    #endregion
                }
                else //編輯
                {
                    #region -- 編輯專案執行紀錄 --
                    var prBean = psipDB.TbProPjRecords.FirstOrDefault(x => x.Id == prId);
                    if (prBean != null)
                    {
                        prBean.CrmOppNo = OppNoFormat(oppNo);
                        prBean.BundleMs = bundleMs;
                        prBean.BundleTask = !string.IsNullOrEmpty(bundleTask) ? bundleTask : "";
                        prBean.Implementers = impBy;
                        prBean.ImplementersCount = Convert.ToInt32(ImplementersCount);
                        prBean.Attendees = Attendees;   //edit by elvis 2022/07/18
                        prBean.Place = place;
                        prBean.StartDatetime = startDate; //Convert.ToDateTime(startDate).ToString("yyyy-MM-dd HH:mm:ss")
                        prBean.EndDatetime = endDate;
                        prBean.WorkHours = Convert.ToInt32(workHours);
                        prBean.TotalWorkHours = prBean.ImplementersCount * prBean.WorkHours;
                        prBean.WithPpl = withPpl;
                        prBean.WithPplPhone = withPplPhone;
                        prBean.Description = desc;
                        prBean.Attachment = attach;

                        prBean.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        psipDB.SaveChanges();

                        result = (int)prId;

                        //有設定任務編號，則要將執行紀錄的合計總工時，更新到任務統計的實際工時
                        if (!string.IsNullOrEmpty(prBean.BundleTask))
                        {
                            var qSameTask = psipDB.TbProPjRecords.Where(x => x.BundleMs == prBean.BundleMs && x.BundleTask == prBean.BundleTask && x.Disabled == 0).ToList();
                            if (qSameTask != null && qSameTask.Count > 0)
                            {
                                int sumSameTaskWorkHour = 0;
                                foreach (var pRecord in qSameTask)
                                {
                                    sumSameTaskWorkHour += Convert.ToInt32(pRecord.TotalWorkHours);
                                }

                                int stair1No = Convert.ToInt32(prBean.BundleTask.Split('.')[0]);
                                int stair2No = prBean.BundleTask.Contains(".") ? Convert.ToInt32(prBean.BundleTask.Split('.')[1]) : 0;
                                int stair3No = 0;
                                if (prBean.BundleTask.Contains("."))
                                {
                                    int tLen = prBean.BundleTask.Split('.').Length;

                                    if (tLen >= 3)
                                    {
                                        stair3No = Convert.ToInt32(prBean.BundleTask.Split('.')[2]);
                                    }
                                }

                                var qTask = psipDB.TbProTasks.FirstOrDefault(x => x.Milestone == prBean.BundleMs && x.Stair1No == stair1No && x.Stair2No == stair2No && x.Stair3No == stair3No);
                                if (qTask != null)
                                {
                                    qTask.WorkHours = sumSameTaskWorkHour;
                                    psipDB.SaveChanges();
                                }
                            }
                        }
                    }
                    #endregion
                }
                return result;
            }
            catch (Exception e)
            {
                //SendMailByAPI("PMO儲存專案執行紀錄", null, "Elvis.Chang@etatung.com", "", "", "PMO儲存專案執行紀錄_錯誤", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "<br>prId: " + prId + "<br>" + e.ToString(), null, null);
                return null;
            }
        }
        #endregion

        #region -- 商機編號設定為10位數 --
        public string OppNoFormat(string oppNo)
        {
            var _oppNo = "0000000000" + oppNo;
            var oppNoFormat = _oppNo.Substring(_oppNo.Length - 10, 10);
            return oppNoFormat;
        }
        #endregion

        #region 依起訖時間，取得里程碑
        public string GetMileStone(string oppNo, string startDate, string endDate)
        {
            var beans = psipDB.TbProMilestones.Where(x => x.CrmOppNo == OppNoFormat(oppNo));
            var _startDate = Convert.ToDateTime(startDate);
            var _endDate = Convert.ToDateTime(endDate);

            foreach (var bean in beans)
            {
                if (!string.IsNullOrEmpty(bean.EstimatedDate))
                {
                    var esDate = Convert.ToDateTime(bean.EstimatedDate);

                    if (esDate.Year == _startDate.Year && esDate.Month == _startDate.Month && esDate.Day == _startDate.Day) return bean.MilestoneNo;

                    if (esDate >= _startDate && esDate <= _endDate) return bean.MilestoneNo;

                }
                else if (!string.IsNullOrEmpty(bean.EstimatedDate) && !string.IsNullOrEmpty(bean.FinishedDate))
                {
                    var esDate = Convert.ToDateTime(bean.EstimatedDate);
                    var fDate = Convert.ToDateTime(bean.FinishedDate);

                    if (fDate.Year == _startDate.Year && fDate.Month == _startDate.Month && fDate.Day == _startDate.Day) return bean.MilestoneNo;

                    if (esDate >= fDate)
                    {
                        if (_startDate >= esDate && _startDate <= fDate) return bean.MilestoneNo;
                        if (_endDate >= esDate && _endDate <= fDate) return bean.MilestoneNo;
                    }
                    else
                    {
                        if (_startDate >= fDate && _startDate <= esDate) return bean.MilestoneNo;
                        if (_endDate >= fDate && _endDate <= fDate) return bean.MilestoneNo;
                    }

                }
            }
            return "";
        }

		#endregion

		#region 匯出工時Excel
		public IActionResult ExportExcel()
		{
			 // 建立 Excel 工作簿
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");

            // 填充表格
            IRow headerRow = sheet.CreateRow(0);
			headerRow.CreateCell(0).SetCellValue("");
			headerRow.CreateCell(1).SetCellValue("工時類型(輸入代號):\nB(專案導入)\nC(內部作業)\nD(專業服務)");
            headerRow.CreateCell(2).SetCellValue("任務活動(輸入代號):\n");
			headerRow.CreateCell(3).SetCellValue("專案(商機號碼):");
			headerRow.CreateCell(4).SetCellValue("工作說明              ");
			headerRow.CreateCell(5).SetCellValue("時間起");
			headerRow.CreateCell(6).SetCellValue("時間訖");

			XSSFCellStyle cs = (XSSFCellStyle)workbook.CreateCellStyle();
           
            cs.WrapText = true; // 設定換行

            headerRow.Cells[1].CellStyle = cs;
			headerRow.Cells[2].CellStyle = cs;

			IRow dataRow = sheet.CreateRow(1);
			dataRow.CreateCell(0).SetCellValue("範例格式(請刪除行！)");
			dataRow.CreateCell(1).SetCellValue("B");
            dataRow.CreateCell(2).SetCellValue("D");
			dataRow.CreateCell(3).SetCellValue("153");
			dataRow.CreateCell(4).SetCellValue("協助專案推進");
			dataRow.CreateCell(5).SetCellValue("2023-01-01 10:00");
			dataRow.CreateCell(6).SetCellValue("2023-01-01 18:30");

            //自動調格式長度
            //for (int j = 0; j < 4; j++) sheet.AutoSizeColumn(j);

			// 將工作簿寫入 MemoryStream
			MemoryStream stream = new MemoryStream();
            workbook.Write(stream, true);
            stream.Flush();
            stream.Position = 0;

            // 回傳 Excel 檔案
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WH_Format.xlsx");
		}

		#endregion

		#region 匯入工時Excel

		[HttpPost]
		public IActionResult ImportExcel(IFormFile file)
		{
			Dictionary<string, string> whTypeDict = new Dictionary<string, string>();
			Dictionary<string, string> actTypeDict = new Dictionary<string, string>();

			whTypeDict.Add("B", "B.專案導入");
			whTypeDict.Add("C", "C.內部作業");
			whTypeDict.Add("D", "D.專業服務");

			actTypeDict.Add("D", "會議");
			actTypeDict.Add("E", "需求訪談");
			actTypeDict.Add("F", "分析/設計");
			actTypeDict.Add("G", "開發/測試");
			actTypeDict.Add("H", "佈版/版控");
			actTypeDict.Add("I", "教育訓練");
			actTypeDict.Add("J", "前置準備");
			actTypeDict.Add("K", "查修/維運");
			actTypeDict.Add("L", "文書處理");

			if (file != null)
            {
				using (var stream = file.OpenReadStream())
				{
					var workbook = new XSSFWorkbook(stream);
					var sheet = workbook.GetSheetAt(0);

					for (int i = 1; i <= sheet.LastRowNum; i++)
					{
						try
						{

							TbWorkingHoursMain bean = new TbWorkingHoursMain();
							var row = sheet.GetRow(i);
							bean.UserName = HttpContext.Session.GetString(SessionKey.USER_NAME);
							bean.UserErpId = HttpContext.Session.GetString(SessionKey.USER_ERP_ID);
							bean.Whtype = row.GetCell(1).StringCellValue;
							bean.ActType = row.GetCell(2).StringCellValue;
							bean.CrmOppNo = row.GetCell(3).StringCellValue;
							bean.WhDescript = row.GetCell(4).StringCellValue;
                            
                            switch (row.GetCell(5).CellType)
                            {
                                case CellType.Numeric:
									bean.StartTime = string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.FromOADate(row.GetCell(5).NumericCellValue));
									break;
                                case CellType.String:
									bean.StartTime = string.Format("{0:yyyy-MM-dd HH:mm}", Convert.ToDateTime(row.GetCell(5).StringCellValue));
									break;

                                default:
                                    break;
                            }

							switch (row.GetCell(6).CellType)
							{
								case CellType.Numeric:
									bean.EndTime = string.Format("{0:yyyy-MM-dd HH:mm}", DateTime.FromOADate(row.GetCell(6).NumericCellValue));
									break;
								case CellType.String:
									bean.EndTime = string.Format("{0:yyyy-MM-dd HH:mm}", Convert.ToDateTime(row.GetCell(6).StringCellValue));
									break;

								default:
									break;
							}

							bean.InsertTime = String.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
							bean.ModifyUser = HttpContext.Session.GetString(SessionKey.USER_NAME);

							if (!string.IsNullOrEmpty(bean.CrmOppNo))
							{
								var oppBean = proxyDB.TbCrmOppHeads.FirstOrDefault(x => x.CrmOppNo == OppNoFormat(bean.CrmOppNo));
								bean.CrmOppNo = oppBean.CrmOppNo;
								bean.CrmOppName = oppBean.CrmOppNo + " - " + oppBean.OppDescription;
							}

							//計算工時分鐘數
							DateTime startTime = Convert.ToDateTime(bean.StartTime);
							DateTime endTime = Convert.ToDateTime(bean.EndTime);
							TimeSpan ts = endTime - startTime;
							bean.Labor = Convert.ToInt32(ts.TotalMinutes);

							bean.ActType = string.IsNullOrEmpty(bean.ActType) ? "L" : bean.ActType;

                            //檢查資料是否合規
                            //工時類型、任務活動代碼不存在的話，就不匯入
                            if (!whTypeDict.ContainsKey(bean.Whtype)) continue;
							if (!actTypeDict.ContainsKey(bean.ActType)) continue;
                            //工時<=0也不匯入
                            if(bean.Labor <= 0) continue;

							psipDB.TbWorkingHoursMains.Add(bean);

							//如果有商機跟專案管理的話，就需加入專案管理的工時計算
							if (!string.IsNullOrEmpty(bean.CrmOppNo))
							{
								int prId = 0;
								var pjInfoBean = psipDB.TbProPjinfos.FirstOrDefault(x => x.CrmOppNo == bean.CrmOppNo);

								//取得MileStone
								string ms = GetMileStone(bean.CrmOppNo, bean.StartTime, bean.EndTime);

								var workHours = Math.Ceiling((decimal)bean.Labor / 60);

								//int? prId, string oppNo, string bundleMs, string bundleTask, string impBy, string ImplementersCount, string Attendees, string place, string startDate, string endDate, string workHours, string withPpl, string withPplPhone, string desc, string attach)
								var _prId = SavePjRecord(prId, bean.CrmOppNo, ms, "", "", "1", "", "", bean.StartTime, bean.EndTime, workHours.ToString(), "", "", bean.WhDescript, "");

								bean.PrId = _prId;
							}

							psipDB.SaveChanges();
						}
						catch (Exception e)
						{
							System.Diagnostics.Debug.WriteLine(e.Message);
						}

					}
				}
			}
			return RedirectToAction("CreateWH");
		}

		#endregion


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
            pIsCSManager = CMF.getIsCustomerServiceManager(pLoginAccount, pSysOperationID);
            pIsCS = CMF.getIsCustomerService(pLoginAccount, pSysOperationID);

            ViewBag.pIsMIS = pIsMIS;
            ViewBag.pIsCSManager = pIsCSManager;
            ViewBag.pIsCS = pIsCS;
            #endregion            
        }
        #endregion
    }

}
