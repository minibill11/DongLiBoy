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
using Newtonsoft.Json.Linq;
using iTextSharp.text.pdf;

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
            //var datalist = await db.Personnel_Turnoverrate.Where(c => c.Year == Year && c.Month == Month).ToListAsync();
            JObject Turnoverrate_Table = new JObject();
            JObject  Department_record = new JObject();
            var Year_Month_Allrecord = await db.Personnel_daily.Where(c => c.Date.Value.Year == Year && c.Date.Value.Month == Month).ToListAsync();

            var Department_list = Year_Month_Allrecord.Select(c => c.Department).Distinct();
            decimal average_sum = 0;
            int departure_month_sum = 0;
            int i = 0;
            foreach (var department in Department_list)
            {
                ////月初日期
                //var begin_date  = Year_Month_Allrecord.Where(c => c.Department == department).Min(c=>c.Date);
                ////月末日期
                //var end_date = Year_Month_Allrecord.Where(c => c.Department == department).Max(c => c.Date);
                //月初人数
                Department_record.Add("department", department);
                var begin_day_of_month = Year_Month_Allrecord.Where(c => c.Department == department).OrderBy(c=>c.Date).FirstOrDefault().Employees_personnel;
                Department_record.Add("begin_day_of_month", begin_day_of_month);
                //月末人数
                var end_day_of_month = Year_Month_Allrecord.Where(c => c.Department == department).OrderByDescending(c=>c.Date).FirstOrDefault().Employees_personnel;
                Department_record.Add("end_day_of_month", end_day_of_month);
                //平均人数
                decimal average = (begin_day_of_month + end_day_of_month) / 2;
                average_sum = average_sum + average;
                Department_record.Add("average", average);
                //整月离职人数之和
                var departure_sum = Year_Month_Allrecord.Where(c => c.Department == department).Sum(c => c.Todoy_dimission_employees_over7days);
                departure_month_sum = departure_month_sum + departure_sum;
                Department_record.Add("leave_sum", departure_sum);
                //流失率
                decimal turnoverrate = departure_sum * 100/ average;
                Department_record.Add("turnoverrate", turnoverrate);
                Turnoverrate_Table.Add(i.ToString(), Department_record);
                Department_record = new JObject();
                i++;
            }
            
            //右下角总平均数值
            Turnoverrate_Table.Add("month_average", average_sum==0?0:departure_month_sum * 100 / average_sum);
            if (Turnoverrate_Table.Values() != null&& Department_list.Count()!=0)
            {
                
                return Content(JsonConvert.SerializeObject(Turnoverrate_Table));
                
            }
            else
            {
                return Content("没有记录！");
            }
        }
        

        //流失率年度折线图数据
        [HttpPost]
        public async Task<ActionResult> TurnoverrateYearlyLineChartData(int? Year)
        {

            if (Year == null)
            {
                return Content(Year == null ? "年份未选择！" : "");
            }
            //var datalist = await db.Personnel_Turnoverrate.Where(c => c.Year == Year).GroupBy(c=>c.Month).ToListAsync();
            //JObject data = new JObject();
            JObject yearChartDate = new JObject();
            string[] List = new string[12];
            string[] YearList = new string[12];
            JObject depChartDate = new JObject();
            JObject totalCharDate = new JObject();

            var Year_Allrecord =await db.Personnel_daily.Where(c => c.Date.Value.Year == Year).ToArrayAsync();
            var department = Year_Allrecord.Select(c => c.Department).Distinct();
            var mouths = Year_Allrecord.Select(c => c.Date.Value.Month).Distinct();

            //折线图
            foreach (var mouth in mouths)
            {
                decimal average_sum = 0;
                int departure_month_sum = 0;
                //decimal avageTurnoverrate = 0;

                foreach (var dep in department)
                {
                    int count = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == mouth).Count();
                    if (count != 0)
                    {
                        //月初人数
                        var begin_day_of_month = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == mouth).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;
                        //月末人数
                        var end_day_of_month = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == mouth).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel;
                        //平均数
                        decimal average = (begin_day_of_month + end_day_of_month) / 2;
                        average_sum = average_sum + average;
                        //离职人数总和
                        var departure_sum = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == mouth).Sum(c => c.Todoy_dimission_employees_over7days);
                        departure_month_sum = departure_month_sum + departure_sum;
                        //每个部门的每月流失率
                        decimal turnoverrate = departure_sum * 100 / average;
                        //部门
                       // avageTurnoverrate = avageTurnoverrate + turnoverrate;
                    }
                }
                YearList[mouth - 1] =average_sum==0?0.ToString():(departure_month_sum * 100 / average_sum).ToString("F2");
                
            }
            string yearListString = string.Join(",", YearList);
            //柱状图
            foreach (var dep in department)
            {
                decimal average_sum = 0;
                int departure_month_sum = 0;
                //decimal avageTurnoverrate = 0;
               
                foreach (var mouth in mouths)
                {
                    int count = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == mouth).Count();
                    if (count == 0)
                    {
                        List[mouth-1] = "0.00";
                    }
                    else
                    {
                        //月初人数
                        var begin_day_of_month = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == mouth).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;
                        //月末人数
                        var end_day_of_month = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == mouth).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel;
                        //平均数
                        decimal average = (begin_day_of_month + end_day_of_month) / 2;
                        average_sum = average_sum + average;
                        //离职人数总和
                        var departure_sum = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == mouth).Sum(c => c.Todoy_dimission_employees_over7days);
                        departure_month_sum = departure_month_sum + departure_sum;
                        //每个部门的每月流失率
                        decimal turnoverrate = departure_sum * 100 / average;
                        
                        List[mouth-1]=turnoverrate.ToString("F2");

                        //部门
                       // avageTurnoverrate = avageTurnoverrate + turnoverrate;
                    }
                }
                depChartDate.Add("name",dep);
                string mouthList = string.Join(",", List);
                depChartDate.Add("data", mouthList);
                //depChartDate.Add("mouthYear", departure_month_sum * 100 / average_sum);
                //各个部门的每月流失率
                yearChartDate.Add(dep, depChartDate);
                List = new string[12];
                depChartDate=new JObject();
            }

            totalCharDate.Add("line", yearListString);
            totalCharDate.Add("columnar", yearChartDate);
            if (department.Count()!=0&& mouths.Count()!=0)
            {
                return Content(JsonConvert.SerializeObject(totalCharDate));
            }
            else
            {
                return Content("没有记录！");
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
            //var datalist = await db.Personnel_Turnoverrate.Where(c => c.Department == department && c.Year == year).ToListAsync();
            JObject MouthList = new JObject();
            JObject totalList = new JObject();
            var Year_Department_Allrecord = await db.Personnel_daily.Where(c => c.Date.Value.Year == year&&c.Department==department).ToArrayAsync();
            var mouths= Year_Department_Allrecord.Select(c => c.Date.Value.Month).Distinct();
            decimal totalTurnoverrate = 0;
            int i =0;
            foreach (var mouth in mouths)
            {
                i++;
                //月初人数
                int count = Year_Department_Allrecord.Where(c => c.Date.Value.Month == mouth).Count();
                
                MouthList.Add("Mcouth", mouth);
                var begin_day_of_month = Year_Department_Allrecord.Where(c => c.Date.Value.Month == mouth).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;
                MouthList.Add("begin_day_of_month", begin_day_of_month);
                //月末人数
                var end_day_of_month = Year_Department_Allrecord.Where(c => c.Date.Value.Month == mouth).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel;
                MouthList.Add("end_day_of_month", end_day_of_month);
                //平均人数
                decimal average = (begin_day_of_month + end_day_of_month) / 2;
                //average_sum = average_sum + average;
                MouthList.Add("average", average);
                //整月离职人数之和
                var departure_sum = Year_Department_Allrecord.Where(c => c.Date.Value.Month == mouth).Sum(c => c.Todoy_dimission_employees_over7days);
                //departure_month_sum = departure_month_sum + departure_sum;
                MouthList.Add("leave_sum", departure_sum);
                //流失率
                decimal turnoverrate = departure_sum * 100 / average;
                totalTurnoverrate = totalTurnoverrate + turnoverrate;
                MouthList.Add("turnoverrate", turnoverrate);

                totalList.Add(mouth.ToString(), MouthList);
                MouthList=new JObject();
            }
            totalList.Add("avgturnoverrate", totalTurnoverrate / i);
            if (totalList != null)
            {
                return Content(JsonConvert.SerializeObject(totalList));
            }
            else
            {
                return Content("没有记录！");
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

    internal class Personnel_dailyResult
    {
        public string Department { get; set; }
        public DateTime? Date { get; set; }

        public int Todoy_dimission_employees_over7days { get; set; }

        public int Employees_personnel { get; set; }
    }
}