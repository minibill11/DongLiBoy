using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AspNetMvc.QuickStart.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserName, string Password)
        {
            if (UserName == "sanshi" && Password == "pass")
            {
                FormsAuthentication.RedirectFromLoginPage("sanshi", false);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }



        [Authorize]
        public ActionResult TransferMoney()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult TransferMoney(string ToAccount, int Money)
        {
            //if (Request.Url.Host != Request.UrlReferrer.Host)
            //{
            //    throw new Exception("Referrer validate fail!");
            //}
            // 这里放置转账业务代码

            ViewBag.ToAccount = ToAccount;
            ViewBag.Money = Money;
            return View();
        }
    }
}