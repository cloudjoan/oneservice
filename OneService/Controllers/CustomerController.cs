using Microsoft.AspNetCore.Mvc;
using OneService.Models;
using OneService.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace OneService.Controllers
{
	public class CustomerController : Controller
	{
		BIContext			BiDB	= new BIContext();
		ERP_PROXY_DBContext ProxyDB = new ERP_PROXY_DBContext();
		CommonFunction		CMF		= new CommonFunction();

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

		#region 取得登入帳號權限
		/// <summary>
		/// 取得登入帳號權限
		/// </summary>
		public void getLoginAccount()
		{
			#region 測試用
			//pLoginAccount = @"etatung\Joy.Chi";     // MIS
			#endregion

			pLoginAccount = HttpContext.Session.GetString(SessionKey.USER_ACCOUNT); //正式用

			#region One Service相關帳號
			pIsMIS			= CMF.getIsMIS(pLoginAccount, pSysOperationID);
			pIsCSManager	= CMF.getIsCustomerServiceManager(pLoginAccount, pSysOperationID);
			pIsCS			= CMF.getIsCustomerService(pLoginAccount, pSysOperationID);

			ViewBag.pIsMIS			= pIsMIS;
			ViewBag.pIsCSManager	= pIsCSManager;
			ViewBag.pIsCS			= pIsCS;
			#endregion
		}
		#endregion

		public IActionResult CustomerJourney(string CusId)
		{
			string	CusName			= "";
			string	CusIdN			= "";
			decimal	OppCount		= 0;
			decimal OppDealCount	= 0;
			int		OppAmount		= 0;
			int		OppDealAmount	= 0;
			string	OppDealPercent	= "0%";
			decimal SOCount			= 0;
			decimal SODealCount		= 0;
			int		SOAmount		= 0;
			int		SODealAmount	= 0;
			string	SODealPercent	= "0%";
			decimal SVSRCount		= 0;
			decimal SVSRDealCount	= 0;
			string	SVSRDealPercent = "0%";

			if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == null || HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != "true")
			{
				return RedirectToAction("Login", "Home");
			}

			getLoginAccount();

			#region 取得登入人員資訊
			CommonFunction.EmployeeBean EmpBean = new CommonFunction.EmployeeBean();
			EmpBean = CMF.findEmployeeInfo(pLoginAccount);

			ViewBag.cLoginUser_Name				= EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
			ViewBag.cLoginUser_EmployeeNO		= EmpBean.EmployeeNO;
			ViewBag.cLoginUser_ERPID			= EmpBean.EmployeeERPID;
			ViewBag.cLoginUser_WorkPlace		= EmpBean.WorkPlace;
			ViewBag.cLoginUser_DepartmentName	= EmpBean.DepartmentName;
			ViewBag.cLoginUser_DepartmentNO		= EmpBean.DepartmentNO;
			ViewBag.cLoginUser_ProfitCenterID	= EmpBean.ProfitCenterID;
			ViewBag.cLoginUser_CostCenterID		= EmpBean.CostCenterID;
			ViewBag.cLoginUser_CompCode			= EmpBean.CompanyCode;
			ViewBag.cLoginUser_BUKRS			= EmpBean.BUKRS;
			ViewBag.cLoginUser_IsManager		= EmpBean.IsManager;
			ViewBag.empEngName					= EmpBean.EmployeeCName + " " + EmpBean.EmployeeEName.Replace(".", " ");
			#endregion

			ViewBag.OppsInfo	= null;
			ViewBag.SOsInfo		= null;
			ViewBag.SVSRsInfo	= null;

			if (!string.IsNullOrEmpty(CusId))
			{
				// 商機
				var OppsInfo		= BiDB.SdOppheads.Where(x => x.CustomerId == CusId).ToList();
				if(OppsInfo != null)
				{
					ViewBag.OppsInfo	= OppsInfo;
					OppCount			= OppsInfo.Count;
					OppAmount			= Convert.ToInt32(OppsInfo.Select(x => x.OppRevenue).Sum());
					OppDealCount		= OppsInfo.Where(x => x.OppPhasePercent == " 100%").Count();
					OppDealAmount		= Convert.ToInt32(OppsInfo.Where(x => x.OppPhasePercent == " 100%").Select(x => x.OppRevenue).Sum());
					OppDealPercent		= (OppCount != 0 && OppDealCount != 0) ? Convert.ToDouble((OppDealCount / OppCount) * 100).ToString("f2") + "%" : "0%";
				}
				

				// 訂單
				var SOsInfo			= BiDB.MartAnalyseSos.Where(x => x.CustomerId == CusId).ToList();
				if(SOsInfo != null)
				{
					ViewBag.SOsInfo = SOsInfo;
					SOCount			= SOsInfo.Count;
					SOAmount		= Convert.ToInt32(SOsInfo.Select(x => x.Soamount).Sum());
					SODealCount		= SOsInfo.Where(x => x.PhaseBl == "已結案").Count();
					SODealAmount	= Convert.ToInt32(SOsInfo.Where(x => x.PhaseBl == "已結案").Select(x => x.Soamount).Sum());
					SODealPercent	= (SOCount != 0 && SODealCount != 0) ? Convert.ToDouble((SODealCount / SOCount) * 100).ToString("f2") + "%" : "0%";
				}

				// 報修
				var SVSRsInfo		= BiDB.SvSrs.Where(x => x.CustomerId == CusId).ToList();
				if(SVSRsInfo != null)
				{
					ViewBag.SVSRsInfo	= SVSRsInfo;
					SVSRCount			= SVSRsInfo.Count;
					SVSRDealCount		= SVSRsInfo.Where(x => x.StatusText == "結案").Count();
					SVSRDealPercent		= (SVSRCount != 0 && SVSRDealCount != 0) ? Convert.ToDouble((SVSRDealCount / SVSRCount) * 100).ToString("f2") + "%" : "0%";
				}

				var CusInfo			= ProxyDB.ViewCustomerandpersonals.Where(x => x.Kna1Kunnr == CusId).FirstOrDefault();

				if(CusInfo != null)
				{ CusName = CusInfo.Kna1Name1; }
				CusIdN = CusId;
			}

			ViewBag.CusName			= CusName;
			ViewBag.CusId			= CusIdN;
			ViewBag.OppCount		= OppCount;
			ViewBag.OppAmount		= OppAmount;
			ViewBag.OppDealCount	= OppDealCount;
			ViewBag.OppDealAmount	= OppDealAmount;
			ViewBag.OppDealPercent	= OppDealPercent;
			ViewBag.SOCount			= SOCount;
			ViewBag.SOAmount		= SOAmount;
			ViewBag.SODealCount		= SODealCount;
			ViewBag.SODealAmount	= SODealAmount;
			ViewBag.SODealPercent	= SODealPercent;
			ViewBag.SVSRCount		= SVSRCount;
			ViewBag.SVSRDealCount	= SVSRDealCount;
			ViewBag.SVSRDealPercent = SVSRDealPercent;

			return View();
		}

		public IActionResult QueryCostRevenueByOpp(string Opp_No)
		{
			if(!string.IsNullOrEmpty(Opp_No))
			{
				var OppDeatils = BiDB.SdCostAnalysisHeaders.Where(x => x.CrmOppNo == Opp_No.TrimStart('0')).ToList();

				if (OppDeatils != null)
				{ ViewBag.OppDeatils = OppDeatils; }
				else
				{ ViewBag.OppDeatils = null; }
			}			

			return View();
		}
	}
}
