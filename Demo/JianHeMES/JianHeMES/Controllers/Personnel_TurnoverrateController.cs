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
using Newtonsoft.Json;

namespace JianHeMES.Controllers
{
    public class Personnel_TurnoverrateController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        #region-----------流失率查看
        public async Task<ActionResult> Turnoverrate()
        {
            return View();
        }

        //按月查看部门流失率情况
        [HttpPost]
        public async Task<ActionResult> Turnoverrate(int? Year, int? Month)
        {

            if (Year == null || Month == null )
            {
                return Content(Year == null ? "年份" : "" + Month == null ? "月份" :  "" + "未选择！");
            }
            var datalist = await db.Personnel_Turnoverrate.Where(c => c.Year == Year && c.Month == Month).ToListAsync();
            if (datalist != null)
            {
                return Content(JsonConvert.SerializeObject(datalist));
            }
            else
            {
                return Content(Year + "年" + Month + "月没有记录或尝未输入数据！");
            }
        }

        //查看部门整年流失率情况
        [HttpPost]
        public async Task<ActionResult> TurnoverrateYearly(string department,int? year)
        {
            if (department == null || year == null)
            {
                return Content(department == null ? "部门为空！" : "" + year == null ? "年份未选择！" : "");
            }
            var datalist = await db.Personnel_Turnoverrate.Where(c => c.Department == department && c.Year == year).ToListAsync();
            if (datalist != null)
            {
                return Content(JsonConvert.SerializeObject(datalist));
            }
            else
            {
                return Content(department + "部" + year + "年度没有记录或尝未输入数据！");
            }
        }
        #endregion


        #region-----------流失率输入
        public async Task<ActionResult> TurnoverrateInput()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> TurnoverrateInput(List<Personnel_Turnoverrate> monthDataList)
        {

            if (monthDataList != null)
            {
                var CreateDate = DateTime.Now;
                var Creator = ((Users)Session["User"]).UserName;
                //var weekDay = CreateDate.DayOfWeek.ToString();//得到今天是周几
                string result = "";

                #region-----算出周一和周日的日期
                //DateTime Monday = DateTime.Now;
                //DateTime Sunday = DateTime.Now;
                //switch (weekDay)
                //{
                //    case "Monday"://星期一
                //        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                //        Sunday = Sunday.AddDays(6);
                //        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                //        break;
                //    case "Tuesday"://星期二
                //        Monday = Monday.AddDays(-1);
                //        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                //        Sunday = Sunday.AddDays(5);
                //        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                //        break;
                //    case "Wednesday": //星期三 
                //        Monday = Monday.AddDays(-2);
                //        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                //        Sunday = Sunday.AddDays(4);
                //        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                //        break;
                //    case "Thursday": //星期四
                //        Monday = Monday.AddDays(-3);
                //        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                //        Sunday = Sunday.AddDays(3);
                //        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                //        break;
                //    case "Friday": //星期五
                //        Monday = Monday.AddDays(-4);
                //        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                //        Sunday = Sunday.AddDays(2);
                //        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                //        break;
                //    case "Saturday"://星期六
                //        Monday = Monday.AddDays(-5);
                //        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                //        Sunday = Sunday.AddDays(1);
                //        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                //        break;
                //    case "Sunday"://星期日
                //        Monday = Monday.AddDays(-6);
                //        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                //        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                //        break;
                //}
                #endregion

                foreach (var data in monthDataList)
                {
                    int isExist = db.Personnel_Turnoverrate.Count(c => c.Year == data.Year && c.Month == data.Month && c.Department == data.Department);
                    if (isExist > 0)
                    {
                        if (result == "")
                        {
                            result = data.Year + "年" + data.Month + "月" + data.Department;
                        }
                        else
                        {
                            result = result + "," + data.Year + "年" + data.Month + "月" + data.Department;
                        }
                    }
                }
                if (result != "") return Content(result + "数据已经存在，不能再输入！");
                foreach (var data in monthDataList)
                {
                    data.CreateDate = CreateDate;
                    data.Creator = Creator;
                    //data.Monday = Monday;
                    //data.Sunday = Sunday;
                    db.Personnel_Turnoverrate.Add(data);
                    await db.SaveChangesAsync();
                }
                return Content("保存成功");
            }
            return Content("保存失败");
        }
        #endregion


        #region-----------流失率修改
        public async Task<ActionResult> TurnoverrateEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Turnoverrate personnel_Turnoverrate = await db.Personnel_Turnoverrate.FindAsync(id);
            if (personnel_Turnoverrate == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Turnoverrate);
        }

        // POST: Personnel_Turnoverrate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TurnoverrateEdit([Bind(Include = "Id,Department,Month_Beginning,Month_End,Average_Number,Departure,Turnover_Rate,Average_Value,Year,Month,Date,CreateDate,Creator")] Personnel_Turnoverrate personnel_Turnoverrate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personnel_Turnoverrate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(personnel_Turnoverrate);
        }
        #endregion


        #region-----------其他方法
        // GET: Personnel_Turnoverrate
        public async Task<ActionResult> Index()
        {
            return View(await db.Personnel_Turnoverrate.ToListAsync());
        }

        // GET: Personnel_Turnoverrate/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Turnoverrate personnel_Turnoverrate = await db.Personnel_Turnoverrate.FindAsync(id);
            if (personnel_Turnoverrate == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Turnoverrate);
        }

        // GET: Personnel_Turnoverrate/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Personnel_Turnoverrate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Department,Month_Beginning,Month_End,Average_Number,Departure,Turnover_Rate,Average_Value,Year,Month,Date,CreateDate,Creator")] Personnel_Turnoverrate personnel_Turnoverrate)
        {
            if (ModelState.IsValid)
            {
                db.Personnel_Turnoverrate.Add(personnel_Turnoverrate);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(personnel_Turnoverrate);
        }

        // GET: Personnel_Turnoverrate/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Turnoverrate personnel_Turnoverrate = await db.Personnel_Turnoverrate.FindAsync(id);
            if (personnel_Turnoverrate == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Turnoverrate);
        }

        // POST: Personnel_Turnoverrate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Department,Month_Beginning,Month_End,Average_Number,Departure,Turnover_Rate,Average_Value,Year,Month,Date,CreateDate,Creator")] Personnel_Turnoverrate personnel_Turnoverrate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personnel_Turnoverrate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(personnel_Turnoverrate);
        }

        // GET: Personnel_Turnoverrate/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Turnoverrate personnel_Turnoverrate = await db.Personnel_Turnoverrate.FindAsync(id);
            if (personnel_Turnoverrate == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Turnoverrate);
        }

        // POST: Personnel_Turnoverrate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Personnel_Turnoverrate personnel_Turnoverrate = await db.Personnel_Turnoverrate.FindAsync(id);
            db.Personnel_Turnoverrate.Remove(personnel_Turnoverrate);
            await db.SaveChangesAsync();
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
        #endregion

    }
}
