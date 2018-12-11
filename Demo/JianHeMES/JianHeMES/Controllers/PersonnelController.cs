using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace JianHeMES.Controllers
{
    public class PersonnelController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Personnel
        public async Task<ActionResult> Index()
        {
            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            ViewBag.date = date;
            ViewBag.Department = GetDepartmentList();
            ViewBag.Principal = GetPrincipalList();
            return View(await db.Personnel_daily.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> Index(string department, string principal, DateTime? date)
        {
            ViewBag.date = date;
            var result = await db.Personnel_daily.ToListAsync();
            if(!String.IsNullOrEmpty(department))
            {
                result = result.Where(c => c.Department == department).ToList();
            }
            if(!String.IsNullOrEmpty(principal))
            {
                result = result.Where(c => c.Principal == principal).ToList();
            }
            if (date!=null)//取对应日期的记录
            {
                result = result.Where(c => c.Date.Value.Year == date.Value.Year && c.Date.Value.Month == date.Value.Month && c.Date.Value.Day == date.Value.Day).ToList();
                var weeknum = date.Value.DayOfWeek;
                switch (weeknum.ToString())
                {
                    case "Monday":
                        break;
                    case "Tuesday":
                        DateTime begindateTue = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day - 1);
                        DateTime enddateTue = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day + 1);
                        ViewBag.Onboard = db.Personnel_daily.Where(c => c.Date > begindateTue && c.Date < enddateTue).ToList();
                        break;
                    case "Wednesday":
                        DateTime begindateWed = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day - 2);
                        DateTime enddateWed = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day + 1);
                        ViewBag.Onboard = db.Personnel_daily.Where(c => c.Date > begindateWed && c.Date < enddateWed).ToList();
                        break;
                    case "Thursday":
                        DateTime begindateThu = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day - 3);
                        DateTime enddateThu = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day + 1);
                        ViewBag.Onboard = db.Personnel_daily.Where(c => c.Date > begindateThu && c.Date < enddateThu).ToList();
                        break;
                    case "Friday":
                        DateTime begindateFri = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day - 4);
                        DateTime enddateFri = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day + 1);
                        ViewBag.Onboard = db.Personnel_daily.Where(c => c.Date > begindateFri && c.Date < enddateFri).ToList();
                        break;
                    case "Saturday":
                        DateTime begindateSat = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day - 5);
                        DateTime enddateSat = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day + 1);
                        ViewBag.Onboard = db.Personnel_daily.Where(c => c.Date > begindateSat && c.Date < enddateSat).ToList();
                        break;
                    case "Sunday":
                        DateTime begindateSun = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day - 6);
                        DateTime enddateSun = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day + 1);
                        ViewBag.Onboard = db.Personnel_daily.Where(c => c.Date > begindateSun && c.Date < enddateSun).ToList();
                        break;
                }
            }
            ViewBag.Department = GetDepartmentList();
            ViewBag.Principal = GetPrincipalList();
            return View(result);
        }

        #region-----------------Daily方法

        public async Task<ActionResult> Daily()
        {
            List<Personnel_daily> result = new List<Personnel_daily>();
            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            ViewBag.date = date.ToString("yyyy-MM-dd");
            result = db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == date.Day).ToList();
            var weeknum = date.DayOfWeek;
            List<string> dateVal = new List<string>();
            List<DateTime> datelist = new List<DateTime>();
            List<Personnel_daily> collection = new List<Personnel_daily>();
            switch (weeknum.ToString())
            {
                case "Monday":
                    collection = result;
                    for (int i=0; i<5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
                case "Tuesday":
                    DateTime begindateTue = new DateTime(date.Year, date.Month, date.Day - 1);
                    DateTime enddateTue = new DateTime(date.Year, date.Month, date.Day + 1);
                    collection = db.Personnel_daily.Where(c => c.Date > begindateTue && c.Date < enddateTue).ToList();
                    date = date.AddDays(-1);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString()); 
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
                case "Wednesday":
                    DateTime begindateWed = new DateTime(date.Year, date.Month, date.Day - 2);
                    DateTime enddateWed = new DateTime(date.Year, date.Month, date.Day + 1);
                    collection = db.Personnel_daily.Where(c => c.Date > begindateWed && c.Date < enddateWed).ToList();
                    date = date.AddDays(-2);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
                case "Thursday":
                    DateTime begindateThu = new DateTime(date.Year, date.Month, date.Day - 3);
                    DateTime enddateThu = new DateTime(date.Year, date.Month, date.Day + 1);
                    collection = db.Personnel_daily.Where(c => c.Date > begindateThu && c.Date < enddateThu).ToList();
                    date = date.AddDays(-3);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
                case "Friday":
                    DateTime begindateFri = new DateTime(date.Year, date.Month, date.Day - 4);
                    DateTime enddateFri = new DateTime(date.Year, date.Month, date.Day + 1);
                    collection = db.Personnel_daily.Where(c => c.Date > begindateFri && c.Date < enddateFri).ToList();
                    date = date.AddDays(-4);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
                case "Saturday":
                    DateTime begindateSat = new DateTime(date.Year, date.Month, date.Day - 5);
                    DateTime enddateSat = new DateTime(date.Year, date.Month, date.Day + 1);
                    collection = db.Personnel_daily.Where(c => c.Date > begindateSat && c.Date < enddateSat).ToList();
                    date = date.AddDays(-5);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
                case "Sunday":
                    DateTime begindateSun = new DateTime(date.Year, date.Month, date.Day - 6);
                    DateTime enddateSun = new DateTime(date.Year, date.Month, date.Day + 1);
                    collection = db.Personnel_daily.Where(c => c.Date > begindateSun && c.Date < enddateSun).ToList();
                    date = date.AddDays(-6);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
            }
            JObject On_Board_JsonObj = new JObject
            {
                {"日期",JsonConvert.SerializeObject(dateVal) },
            };   //创建JSON对象
            List<string> departmentlist = db.Personnel_daily.Select(c => c.Department).Distinct().ToList();
            //每日记录
            foreach (var item in departmentlist)
            {
                var dprecord = collection.Where(d => d.Department == item).OrderBy(c=>c.Date).ToList();
                List<int> te = new List<int>();
                foreach (var d in datelist)
                {
                    if (dprecord.Where(c => c.Date.Value.Year == d.Year && c.Date.Value.Month == d.Month && c.Date.Value.Day == d.Day).Count() > 0)
                    {
                        te.Add(dprecord.Where(c => c.Date.Value.Year == d.Year && c.Date.Value.Month == d.Month && c.Date.Value.Day == d.Day).FirstOrDefault().Today_on_board_workers);
                        te.Add(dprecord.Where(c => c.Date.Value.Year == d.Year && c.Date.Value.Month == d.Month && c.Date.Value.Day == d.Day).FirstOrDefault().Today_on_board_employees);
                    }
                    else
                    {
                        te.Add(0);
                        te.Add(0);
                    }
                }
                te.Add(dprecord.Sum(c=>c.Today_on_board_workers+c.Today_on_board_employees));//本周入职合计
                On_Board_JsonObj.Add(item, JsonConvert.SerializeObject(te));
            }
            //合计
            List<int> sum = new List<int>();
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Monday").Sum(c => c.Today_on_board_workers));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Monday").Sum(c => c.Today_on_board_employees));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Sum(c => c.Today_on_board_workers));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Sum(c => c.Today_on_board_employees));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").Sum(c => c.Today_on_board_workers));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").Sum(c => c.Today_on_board_employees));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").Sum(c => c.Today_on_board_workers));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").Sum(c => c.Today_on_board_employees));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Friday").Sum(c => c.Today_on_board_workers));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Friday").Sum(c => c.Today_on_board_employees));
            sum.Add(sum.Sum());
            On_Board_JsonObj.Add("合计",JsonConvert.SerializeObject(sum));
            ViewBag.json = On_Board_JsonObj;
            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> Daily(string section , DateTime date)
        {
            ViewBag.date = Convert.ToDateTime(date).ToString("yyyy-MM-dd");
            List<Personnel_daily> result = new List<Personnel_daily>();
            if(section==null){section = "week";}
            List<string> dateVal = new List<string>();
            List<DateTime> datelist = new List<DateTime>();
            List<Personnel_daily> collection = new List<Personnel_daily>();
            switch (section)
            {
                case "week":
                    result = await db.Personnel_daily.Where(c=>c.Date.Value.Year==date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == date.Day).ToListAsync();
                    break;
                case "month":
                    break;
                case "year":
                    break;
            }
            var weeknum = date.DayOfWeek;
            DateTime enddate = date.AddDays(1);
            switch (weeknum.ToString())
            {
                case "Monday":
                    collection = result;
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
                case "Tuesday":
                    DateTime begindateTue = date.AddDays(-1);
                    collection = db.Personnel_daily.Where(c => c.Date > begindateTue && c.Date < enddate).ToList();
                    date = date.AddDays(-1);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
                case "Wednesday":
                    DateTime begindateWed = date.AddDays(-2) ;
                    collection = db.Personnel_daily.Where(c => c.Date > begindateWed && c.Date < enddate).ToList();
                    date = date.AddDays(-2);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
                case "Thursday":
                    DateTime begindateThu = date.AddDays(-3);
                    collection = db.Personnel_daily.Where(c => c.Date > begindateThu && c.Date < enddate).ToList();
                    date = date.AddDays(-3);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
                case "Friday":
                    DateTime begindateFri = date.AddDays(-4);
                    collection = db.Personnel_daily.Where(c => c.Date > begindateFri && c.Date < enddate).ToList();
                    date = date.AddDays(-4);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
                case "Saturday":
                    DateTime begindateSat = date.AddDays(-5);
                    collection = db.Personnel_daily.Where(c => c.Date > begindateSat && c.Date < enddate).ToList();
                    date = date.AddDays(-5);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
                case "Sunday":
                    DateTime begindateSun = date.AddDays(-6);
                    collection = db.Personnel_daily.Where(c => c.Date > begindateSun && c.Date < enddate).ToList();
                    date = date.AddDays(-6);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
            }

            JObject On_Board_JsonObj = new JObject
            {
                {"日期",JsonConvert.SerializeObject(dateVal) },
            };   //创建JSON对象
            List<string> departmentlist = db.Personnel_daily.Select(c => c.Department).Distinct().ToList();
            //每日记录
            foreach (var item in departmentlist)
            {
                var dprecord = collection.Where(d => d.Department == item).OrderBy(c => c.Date).ToList();
                List<int> te = new List<int>();
                foreach (var d in datelist)
                {
                    if (dprecord.Where(c => c.Date.Value.Year == d.Year && c.Date.Value.Month == d.Month && c.Date.Value.Day == d.Day).Count() > 0)
                    {
                        te.Add(dprecord.Where(c => c.Date.Value.Year == d.Year && c.Date.Value.Month == d.Month && c.Date.Value.Day == d.Day).FirstOrDefault().Today_on_board_workers);
                        te.Add(dprecord.Where(c => c.Date.Value.Year == d.Year && c.Date.Value.Month == d.Month && c.Date.Value.Day == d.Day).FirstOrDefault().Today_on_board_employees);
                    }
                    else
                    {
                        te.Add(0);
                        te.Add(0);
                    }
                }
                te.Add(dprecord.Sum(c => c.Today_on_board_workers + c.Today_on_board_employees));//本周入职合计
                On_Board_JsonObj.Add(item, JsonConvert.SerializeObject(te));
            }
            //合计
            List<int> sum = new List<int>();
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Monday").Sum(c => c.Today_on_board_workers));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Monday").Sum(c => c.Today_on_board_employees));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Sum(c => c.Today_on_board_workers));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Sum(c => c.Today_on_board_employees));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").Sum(c => c.Today_on_board_workers));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").Sum(c => c.Today_on_board_employees));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").Sum(c => c.Today_on_board_workers));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").Sum(c => c.Today_on_board_employees));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Friday").Sum(c => c.Today_on_board_workers));
            sum.Add(collection.Where(c => c.Date.Value.DayOfWeek.ToString() == "Friday").Sum(c => c.Today_on_board_employees));
            sum.Add(sum.Sum());
            On_Board_JsonObj.Add("合计", JsonConvert.SerializeObject(sum));
            ViewBag.json = On_Board_JsonObj;
            return View(result);
        }

        #endregion

        // GET: Personnel/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_daily personnel_daily = await db.Personnel_daily.FindAsync(id);
            if (personnel_daily == null)
            {
                return HttpNotFound();
            }
            return View(personnel_daily);
        }

        // GET: Personnel/Create
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "文员" || ((Users)Session["User"]).Role == "系统管理员")
            {

                return View();
            }
            else
            {
                return Content("<script>alert('对不起，您不能提交人事日报信息，请联系人力资源部！');window.location.href='../Personnel/Daily';</script>");
                //return RedirectToAction("Daily");
            }
            //return View();
        }

        // POST: Personnel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Department,Principal,Aurhorized_personnel,Need_personnel,Employees_personnel,Workers_personnel,Today_on_board_employees,Today_on_board_workers,Interview,Todoy_dimission_employees,Todoy_dimission_workers,Resigned_that_month,Date,Reporter")] Personnel_daily personnel_daily)
        {
            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var recordCount = db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == date.Day && c.Department == personnel_daily.Department).Count();
            if(recordCount>0)
            {
                ModelState.AddModelError("","此部门的人事日报已经上报过，是否需要修改？");
                ViewBag.address = db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == date.Day && c.Department == personnel_daily.Department).FirstOrDefault().Id;
                return View(personnel_daily);
            }
            personnel_daily.Interview = 0;
            personnel_daily.Date = DateTime.Now;
            personnel_daily.Reporter = ((Users)Session["User"]).UserName;
            if (ModelState.IsValid)
            {
                db.Personnel_daily.Add(personnel_daily);
                await db.SaveChangesAsync();
                return RedirectToAction("Daily");
            }
            return View(personnel_daily);
        }

        // GET: Personnel/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_daily personnel_daily = await db.Personnel_daily.FindAsync(id);
            if (personnel_daily == null)
            {
                return HttpNotFound();
            }
            return View(personnel_daily);
        }

        // POST: Personnel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Department,Principal,Aurhorized_personnel,Need_personnel,Employees_personnel,Workers_personnel,Today_on_board_employees,Today_on_board_workers,Interview,Todoy_dimission_employees,Todoy_dimission_workers,Resigned_that_month,Date,Reporter")] Personnel_daily personnel_daily)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personnel_daily).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Daily");
            }
            return View(personnel_daily);
        }

        // GET: Personnel/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_daily personnel_daily = await db.Personnel_daily.FindAsync(id);
            if (personnel_daily == null)
            {
                return HttpNotFound();
            }
            return View(personnel_daily);
        }

        // POST: Personnel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Personnel_daily personnel_daily = await db.Personnel_daily.FindAsync(id);
            db.Personnel_daily.Remove(personnel_daily);
            await db.SaveChangesAsync();
            return RedirectToAction("Daily");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        #region ------------------GetDepartmentList()取出部门列表
        private List<SelectListItem> GetDepartmentList()
        {
            var Department = db.Personnel_daily.Select(m => m.Department).Distinct().ToList();
            var items = new List<SelectListItem>();
            foreach (string item in Department)
            {
                items.Add(new SelectListItem
                {
                    Text = item,
                    Value = item
                });
            }
            return items;
        }
        #endregion

        #region ------------------GetPrincipalList()取出部门列表
        private List<SelectListItem> GetPrincipalList()
        {
            var Department = db.Personnel_daily.Select(m => m.Principal).Distinct().ToList();
            var items = new List<SelectListItem>();
            foreach (string item in Department)
            {
                items.Add(new SelectListItem
                {
                    Text = item,
                    Value = item
                });
            }
            return items;
        }
        #endregion


    }


}
