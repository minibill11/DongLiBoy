using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JianHeMES.Models;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class SMTController : Controller
    {
        #region------------------管理界面

        // GET: SMT管理主页
        public ActionResult SMT_Mangage()
        {
            return View();
        }

        // GET: SMT订单信息管理
        public ActionResult SMT_OrderMangage()
        {
            return View();
        }

        // GET: SMT用户管理
        public ActionResult SMT_UserMangage()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<Users> SMT_User = db.Users.Where(c => c.Department == "SMT").ToList();
            return View(SMT_User);
        }

        #endregion

        #region------------------生产信息
        // GET: SMT总览表
        public ActionResult Index()
        {
            return View();
        }

        //产线看板页面
        [HttpPost]
        public ActionResult LineInfo(int i)
        {
            ViewBag.LineNum = i;//获取产线号
            return View();
        }

        #endregion

        #region------------------生产操作
        // GET: SMT产线未段工位输入操作
        public ActionResult InPutInfo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult InPutInfo(string OrderNum, string Result)
        {

            return View();
        }

        #endregion







        #region------------------其他方法

        //// GET: SMT/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: SMT/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: SMT/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: SMT/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: SMT/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: SMT/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: SMT/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        #endregion
    }
}
