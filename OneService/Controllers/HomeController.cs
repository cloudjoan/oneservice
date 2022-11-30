using Microsoft.AspNetCore.Mvc;
using OneService.Models;
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

        public IActionResult Login()
        {
            return View();
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