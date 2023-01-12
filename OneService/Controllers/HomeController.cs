using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OneService.Models;
using OneService.Utils;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;

namespace OneService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }


        public IActionResult Login()
        {
            if (HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) != null && HttpContext.Session.GetString(SessionKey.LOGIN_STATUS) == "true")
            {
                return RedirectToAction("Index", "WorkingHours");
            }
            else
            {
                ViewBag.errorMsg = HttpContext.Session.GetString(SessionKey.LOGIN_MESSAGE);
                return View();
            }
            
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult DoLogin(IFormCollection formCollection)
        {
            if (IsAuthenticated(formCollection["account"], formCollection["password"]))
            {
                MCSWorkflowContext eipDB = new MCSWorkflowContext();
                var empBean = eipDB.ViewEmpInfoWithoutLeaves.FirstOrDefault(x => x.Account.ToUpper() == @"etatung\" + formCollection["account"]);

                HttpContext.Session.SetString(SessionKey.LOGIN_STATUS, "true");
                HttpContext.Session.SetString(SessionKey.USER_ACCOUNT, @"etatung\" + formCollection["account"]);
                HttpContext.Session.SetString(SessionKey.USER_ERP_ID, empBean.ErpId);
                HttpContext.Session.SetString(SessionKey.USER_NAME, empBean.EmpName);
                HttpContext.Session.SetString(SessionKey.DEPT_ID, empBean.DeptId);
                HttpContext.Session.SetString(SessionKey.DEPT_NAME, empBean.DeptName);
                HttpContext.Session.SetString(SessionKey.LOGIN_MESSAGE, "");

                return RedirectToAction("CreateWH", "WorkingHours");
            }
            else
            {
                HttpContext.Session.SetString(SessionKey.LOGIN_STATUS, "false");
                HttpContext.Session.SetString(SessionKey.LOGIN_MESSAGE, "帳號或密碼錯誤？");
                
                return RedirectToAction("Login");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Demo()
        {
            PSIPContext psipDB = new PSIPContext();
            var beans = psipDB.TbBulletinItems;
            ViewBag.beans = beans;
            return View();
        }

        //AD帳密驗證
        public static bool IsAuthenticated(string username, string pwd)
        {
            try
            {
                using (var ldapConnection = new LdapConnection(new LdapDirectoryIdentifier("139.223.14.100")))
                {
                    ldapConnection.Credential = new NetworkCredential(@"etatung\" + username, pwd);
                    ldapConnection.AuthType = AuthType.Basic;
                    ldapConnection.Bind();
                    return true;
                }
            }
            catch (LdapException ex)
            {
                return false;
            }
        }
    }
}