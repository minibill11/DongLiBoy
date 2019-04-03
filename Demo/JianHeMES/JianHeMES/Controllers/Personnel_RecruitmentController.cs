﻿using System;
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
    public class Personnel_RecruitmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        #region------------招聘周报查看
        public async Task<ActionResult> Recruitment()
        {
            return View(await db.Personnel_Recruitment.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> Recruitment(int? Year, int? Month, int? Week)
        {
            if (Year == null || Month == null || Week == null)
            {
                return Content(Year == null ? "年份" : "" + Month == null ? "月份" : "" + Week == null ? "周" : "" + "未选择！");
            }
            var datalist = await db.Personnel_Recruitment.Where(c => c.Year == Year && c.Month == Month && c.Week == Week).ToListAsync();
            if (datalist != null)
            {
                return Content(JsonConvert.SerializeObject(datalist));
            }
            else
            {
                return Content(Year + "年" + Month + "月第" + Week + "周没有记录或尝未输入数据！");
            }
        }
        #endregion


        #region------------招聘周报输入
        public async Task<ActionResult> RecruitmentInput()
        {
            return View(await db.Personnel_Recruitment.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> RecruitmentInput(List<Personnel_Recruitment> weekDataList)
        {

            if(weekDataList!=null)
            {
                var CreateDate = DateTime.Now;
                var Creator = ((Users)Session["User"]).UserName;
                var weekDay = CreateDate.DayOfWeek.ToString();//得到今天是周几
                string result = "";

                #region-----算出周一和周日的日期
                DateTime Monday = DateTime.Now;
                DateTime Sunday = DateTime.Now;
                switch (weekDay)
                {
                    case "Monday"://星期一
                        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                        Sunday = Sunday.AddDays(6);
                        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                        break;
                    case "Tuesday"://星期二
                        Monday = Monday.AddDays(-1);
                        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                        Sunday = Sunday.AddDays(5);
                        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                        break;
                    case "Wednesday": //星期三 
                        Monday = Monday.AddDays(-2);
                        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                        Sunday = Sunday.AddDays(4);
                        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                        break;
                    case "Thursday": //星期四
                        Monday = Monday.AddDays(-3);
                        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                        Sunday = Sunday.AddDays(3);
                        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                        break;
                    case "Friday": //星期五
                        Monday = Monday.AddDays(-4);
                        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                        Sunday = Sunday.AddDays(2);
                        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                        break;
                    case "Saturday"://星期六
                        Monday = Monday.AddDays(-5);
                        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                        Sunday = Sunday.AddDays(1);
                        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                        break;
                    case "Sunday"://星期日
                        Monday = Monday.AddDays(-6);
                        Monday = new DateTime(Monday.Year, Monday.Month, Monday.Day);
                        Sunday = new DateTime(Sunday.Year, Sunday.Month, Sunday.Day, 23, 59, 59);
                        break;
                }
                #endregion
                foreach (var data in weekDataList)
                {
                    int isExist = db.Personnel_Recruitment.Count(c => c.Year == data.Year && c.Month == data.Month && c.Week == data.Week && c.Department_weekly == data.Department_weekly && c.Demand_jobs == data.Demand_jobs);
                    if (isExist > 0)
                    {
                        if (result == "")
                        {
                            result = data.Year + "年" + data.Month + "月第" + data.Week + "周" + data.Department_weekly + data.Demand_jobs;
                        }
                        else
                        {
                            result = result + "," + data.Year + "年" + data.Month + "月第" + data.Week + "周" + data.Department_weekly + data.Demand_jobs;
                        }
                    }
                }
                if (result != "") return Content(result + "数据已经存在，不能再输入！");
                foreach (var data in weekDataList)
                {
                    data.CreateDate = CreateDate;
                    data.Creator = Creator;
                    data.Monday = Monday;
                    data.Sunday = Sunday;
                    db.Personnel_Recruitment.Add(data);
                    await db.SaveChangesAsync();
                }
                return Content("保存成功");
            }
            return Content("保存失败");
        }
        #endregion


        #region------------招聘周报修改
        public async Task<ActionResult> RecruitmentEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Recruitment personnel_Recruitment = await db.Personnel_Recruitment.FindAsync(id);
            if (personnel_Recruitment == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Recruitment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RecruitmentEdit([Bind(Include = "Id,Department_weekly,Demand_jobs,Compile,Demand_number,Employed,Unfinished_nember,Application_date,Work_date,Invite_Plan_Cycle,Invite_Actaul_Cycle,Date,CreateDate,Creator,Year,Month,Week,Week_number,Monday,Sunday")] Personnel_Recruitment personnel_Recruitment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personnel_Recruitment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Recruitment");
            }
            return View(personnel_Recruitment);
        }
        #endregion



        #region------------其他方法
        // GET: Personnel_Recruitment
        public async Task<ActionResult> Index()
        {
            return View(await db.Personnel_Recruitment.ToListAsync());
        }

        // GET: Personnel_Recruitment/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Recruitment personnel_Recruitment = await db.Personnel_Recruitment.FindAsync(id);
            if (personnel_Recruitment == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Recruitment);
        }

        // GET: Personnel_Recruitment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Personnel_Recruitment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Department_weekly,Demand_jobs,Compile,Demand_number,Employed,Unfinished_nember,Application_date,Work_date,Invite_Plan_Cycle,Invite_Actaul_Cycle,Date,CreateDate,Creator,Year,Month,Week,Week_number,Monday,Sunday")] Personnel_Recruitment personnel_Recruitment)
        {
            if (ModelState.IsValid)
            {
                db.Personnel_Recruitment.Add(personnel_Recruitment);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(personnel_Recruitment);
        }

        // GET: Personnel_Recruitment/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Recruitment personnel_Recruitment = await db.Personnel_Recruitment.FindAsync(id);
            if (personnel_Recruitment == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Recruitment);
        }

        // POST: Personnel_Recruitment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Department_weekly,Demand_jobs,Compile,Demand_number,Employed,Unfinished_nember,Application_date,Work_date,Invite_Plan_Cycle,Invite_Actaul_Cycle,Date,CreateDate,Creator,Year,Month,Week,Week_number,Monday,Sunday")] Personnel_Recruitment personnel_Recruitment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personnel_Recruitment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Recruitment");
            }
            return View(personnel_Recruitment);
        }

        // GET: Personnel_Recruitment/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Recruitment personnel_Recruitment = await db.Personnel_Recruitment.FindAsync(id);
            if (personnel_Recruitment == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Recruitment);
        }

        // POST: Personnel_Recruitment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Personnel_Recruitment personnel_Recruitment = await db.Personnel_Recruitment.FindAsync(id);
            db.Personnel_Recruitment.Remove(personnel_Recruitment);
            await db.SaveChangesAsync();
            return RedirectToAction("Recruitment");
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