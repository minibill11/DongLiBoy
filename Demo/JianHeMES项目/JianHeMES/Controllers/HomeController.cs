﻿using JianHeMES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class HomeController : Controller
    {
        private CommonController com = new CommonController();
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            if (Session["User"] != null)
            {
                var userid = ((Users)Session["User"]).UserNum;
                var item = db.Users.Where(c => c.UserNum == userid).FirstOrDefault();
                if (item.Department == "销售部")
                {
                    if (item != null)
                    {
                        Session["User"] = item;
                        return RedirectToAction("businessDepartment", "Query");

                    };
                }
                else if (item.Department == "采购部" || item.Department == "合约部" || item.Department == "财务部" || item.Department == "客户服务部")
                {
                    if (item != null)
                    {
                        Session["User"] = item;
                        return RedirectToAction("contractDepartment", "Query");
                    }
                }
            }
            return View();
        }
        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult Index3()
        {
            return View();
        }

        public ActionResult HomeIndex()
        {
            return View();
        }

        public ActionResult MES_Plans()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Home", act = "Index2" });
            }
            if (com.isCheckRole("模块管理", "MES计划模块", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            {
                return View();
            }
            return Content("false");
        }

        public ActionResult MES_Excution()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Home", act = "Index2" });
            }
            if (com.isCheckRole("模块管理", "MESExcution模块", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            {
                return View();
            }
            return Content("false");
        }

        public ActionResult HR()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Home", act = "Index2" });
            }
            if (com.isCheckRole("模块管理", "MES人事模块", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            {
                return View();
            }
            else
                return Content("false");
        }

        public ActionResult Environment()
        {
            return View();
        }


        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}