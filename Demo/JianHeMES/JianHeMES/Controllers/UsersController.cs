using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;

namespace JianHeMES.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        public ActionResult Index()
        {
            ViewBag.Department = GetDepartmentList();
            ViewBag.Role = GetRoleList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            //if(((Users)Session["User"]).Role== "系统管理员")
            //{
            //    return View(db.Users.OrderBy(m => m.Department).ToList());
            //}
            return View();
        }

        [HttpPost]
        public ActionResult Index( string UserName, string Department/*,int UserAuthorize*/, string Role/*, int LineNum*/, string Description,int UserNum = 0, int PageIndex = 0)
        {
            ViewBag.Department = GetDepartmentList();
            ViewBag.Role = GetRoleList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "系统管理员")
            {
                var userlist = db.Users.OrderBy(m=>m.Department).ToList();

                if (UserNum!=0)
                {
                    userlist = userlist.OrderBy(m => m.Department).Where(m => m.UserNum == UserNum).ToList();
                }
                if (UserName!="")
                {
                    userlist = userlist.OrderBy(m => m.Department).Where(m => m.UserName.Contains(UserName)).ToList();
                }
                if (Department != "")
                {
                    userlist = userlist.Where(m => m.Department == Department).ToList();
                }
                //if (!String.IsNullOrEmpty(UserAuthorize.ToString()))
                //{
                //    userlist = userlist.OrderBy(m => m.Department).Where(m => m.UserAuthorize == UserAuthorize).ToList();
                //}
                if (Role != "")
                {
                    userlist = userlist.OrderBy(m => m.Department).Where(m => m.Role == Role).ToList();
                }
                //if (!String.IsNullOrEmpty(LineNum.ToString()))
                //{
                //    userlist = userlist.OrderBy(m => m.Department).Where(m => m.LineNum == LineNum).ToList();
                //}
                if (Description != "")
                {
                    userlist = userlist.OrderBy(m => m.Department).Where(m => m.Description.Contains(Description)).ToList();
                }
                return View(userlist);
            }
            return View();
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }

        // POST: Users/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserNum,UserName,Password,CreateDate,Creator,UserAuthorize,Role,Department,LineNum,Deleter,DeleteDate,Description")] Users users)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            users.Creator = ((Users)Session["User"]).UserName; 
            users.CreateDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(users);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserNum,UserName,Password,CreateDate,Creator,UserAuthorize,Role,Department,LineNum,Deleter,DeleteDate,Description")] Users users)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        public ActionResult Modifypwd(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Modifypwd([Bind(Include = "ID,UserNum,UserName,Password,CreateDate,Creator,UserAuthorize,Role,Department,LineNum,Deleter,DeleteDate,Description")] Users users)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "CalibrationRecords");
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            db.Users.Remove(users);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Users user)
        {
            var item = db.Users.FirstOrDefault(u => u.UserNum == user.UserNum && u.PassWord == user.PassWord);
            if (item != null)
            {
                Session["User"] = item;
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "登录出错，请检查员工编号及密码是否有误！");
            return View(user);
        }
        [HttpPost]
        public ActionResult loginCheck(Users user)
        {
            var item = db.Users.FirstOrDefault(u => u.UserNum == user.UserNum && u.PassWord == user.PassWord);
            if (item != null)
            {
                Session["User"] = item;
                return Content("success");
            }
            return Content("error");
        }
        public ActionResult Logoff()
        {
            Session.Clear();
            return RedirectToAction("Index", "CalibrationRecords");
        }


        #region ---------------------------------------GetDepartmentList()取出部门列表
        private List<SelectListItem> GetDepartmentList()
        {
            var departmentlist = db.Users.Select(m => m.Department).Distinct().ToList();
            var items = new List<SelectListItem>();
            foreach (string department in departmentlist)
            {
                items.Add(new SelectListItem
                {
                    Text = department,
                    Value = department
                });
            }
            return items;
        }
        #endregion

        #region ---------------------------------------GetRoleList()取出角色列表
        private List<SelectListItem> GetRoleList()
        {
            var rolelist = db.Users.Select(m => m.Role).Distinct().ToList();
            var items = new List<SelectListItem>();
            foreach (string role in rolelist)
            {
                items.Add(new SelectListItem
                {
                    Text = role,
                    Value = role
                });
            }
            return items;
        }
        #endregion


        #region ---------------------------------------分页
        private static readonly int PAGE_SIZE = 10;

        private int GetPageCount(int recordCount)
        {
            int pageCount = recordCount / PAGE_SIZE;
            if (recordCount % PAGE_SIZE != 0)
            {
                pageCount += 1;
            }
            return pageCount;
        }
        #endregion


    }
}
