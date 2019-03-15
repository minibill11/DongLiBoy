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
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;

namespace JianHeMES.Controllers
{
    public class PersonnelController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        #region-----------------Index首页方法

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


        public async Task<ActionResult> Index2()
        {
            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            ViewBag.date = date;
            ViewBag.Department = GetDepartmentList();
            ViewBag.Principal = GetPrincipalList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index2(string department, string principal, DateTime? date)
        {
            ViewBag.date = date;
            var result = await db.Personnel_daily.ToListAsync();
            if (!String.IsNullOrEmpty(department))
            {
                result = result.Where(c => c.Department == department).ToList();
            }
            if (!String.IsNullOrEmpty(principal))
            {
                result = result.Where(c => c.Principal == principal).ToList();
            }
            if (date != null)//取对应日期的记录
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
        #endregion


        #region-----------------Daily方法

        public ActionResult Daily()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> getjsondata(DateTime date)
        {
            if (date == null)
            {
                date = DateTime.Now;
            }
            DateTime dateV = new DateTime(date.Year, date.Month, date.Day);
            DateTime tomorrow = dateV.AddDays(1);
            DateTime monday = new DateTime(date.Year, date.Month, date.Day); 
            DateTime sunday = new DateTime(date.Year, date.Month, date.Day); 
            var arr = new List<Object>();
            var weeknum = date.DayOfWeek.ToString();//得到date对应是星期几
            List<string> departmentlist = new List<string>();
            if (date.Date < new DateTime(2019, 1, 2))
            {
                departmentlist = new List<string>
                {
                "PC部","MC部","SMT部","装配1部","装配2部","配套加工部","技术部","品质部","行政后勤部","人力资源部","财务部","品质技术中心总监","轮值厂长"
                };
            }
            else
            {
                departmentlist = new List<string>
                {
                "PC部","MC部","SMT部","装配1部","装配2部","配套加工部","技术部","品质部","行政后勤部","人力资源部","财务部","制造中心总监","工厂厂长"
                };
            }
            //取出本月数据
            var Month_recordlist = await db.Personnel_daily.Where(c => c.Date.Value.Year == dateV.Year && c.Date.Value.Month == dateV.Month).ToListAsync();
            //修正确定monday的日期
            switch (weeknum)
            {
                case "Monday": //星期一
                    sunday = dateV.AddDays(6);
                    break;
                case "Tuesday": //星期二
                    monday = dateV.AddDays(-1);
                    sunday = dateV.AddDays(5);
                    break;
                case "Wednesday": //星期三 
                    monday = dateV.AddDays(-2);
                    sunday = dateV.AddDays(4);
                    break;
                case "Thursday": //星期四
                    monday = dateV.AddDays(-3);
                    sunday = dateV.AddDays(3);
                    break;
                case "Friday": //星期五
                    monday = dateV.AddDays(-4);
                    sunday = dateV.AddDays(2);
                    break;
                case "Saturday": //星期六
                    monday = dateV.AddDays(-5);
                    sunday = dateV.AddDays(1);
                    break;
                case "Sunday": //星期日
                    monday = dateV.AddDays(-6);
                    break;
            }
            //取出本周数据
            var this_week_recordlist = await db.Personnel_daily.Where(c => c.Date > monday && c.Date<tomorrow).ToListAsync();
            //如果月和周数据都为空，则返回“无记录”
            if (Month_recordlist.Count == 0 && this_week_recordlist.Count() == 0 || date.Date > sunday || date < new DateTime(2018, 12, 3))
            {
                JObject temp = new JObject();
                temp.Add("无记录", "无记录");
                return Content(JsonConvert.SerializeObject(new JObject(temp)));
            }

            foreach (var item in departmentlist)
            {
                dynamic obj = new ExpandoObject();
                //(1)当天有记录、没有记录两个处理方式，用一个if
                int recordCount = Month_recordlist.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == date.Day && c.Department == item).Count();
                //当天记录
                var record = Month_recordlist.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == date.Day && c.Department == item).FirstOrDefault();
                if (recordCount > 0) //有记录
                {
                    obj.id = record.Id;
                    obj.Department = record.Department;
                    obj.Principal = record.Principal;
                    obj.Aurhorized_personnel = record.Aurhorized_personnel;
                    obj.Need_personnel = record.Need_personnel;
                    obj.Employees_personnel = record.Employees_personnel;
                    obj.Workers_personnel = record.Workers_personnel;
                    obj.Today_on_board_employees = record.Today_on_board_employees;
                    obj.Today_on_board_workers = record.Today_on_board_workers;
                    obj.Huizong = record.Employees_personnel + record.Workers_personnel + record.Today_on_board_employees + record.Today_on_board_workers;
                    obj.Dangqian_que = record.Need_personnel - obj.Huizong;
                    obj.Interview = record.Interview;
                    obj.Date = record.Date.ToString();
                    obj.Reporter = record.Reporter;
                    obj.Resigned_that_month = record.Resigned_that_month;
                    obj.Resigned_workers_that_month = record.Resigned_workers_that_month;
                }
                else  //当天无记录
                {
                    var lastrecord = Month_recordlist.Where(c => c.Department == item && c.Date < date).OrderByDescending(c => c.Id).Count() > 0 ? Month_recordlist.Where(c => c.Department == item && c.Date < date).OrderByDescending(c => c.Id).FirstOrDefault() : db.Personnel_daily.Where(c => c.Department == item && c.Date < date).OrderByDescending(c => c.Id).FirstOrDefault();
                    obj.id = 0;
                    obj.Department = lastrecord.Department;
                    obj.Principal = lastrecord.Principal;
                    obj.Aurhorized_personnel = lastrecord.Aurhorized_personnel;
                    obj.Need_personnel = lastrecord.Need_personnel;
                    obj.Employees_personnel = 0;
                    obj.Workers_personnel = 0;
                    obj.Today_on_board_employees = 0;
                    obj.Today_on_board_workers = 0;
                    obj.Huizong = 0;
                    obj.Dangqian_que = 0;
                    obj.Interview = 0;
                    obj.Date = "";
                    obj.Reporter = "未上报";
                    obj.Resigned_that_month = 0;// lastrecord.Resigned_that_month;//如果当天没有记录，是否应该赋最后一个记录的值？还是直接赋0?   
                    obj.Resigned_workers_that_month = 0;// lastrecord.Resigned_workers_that_month;//如果当天没有记录，是否应该赋最后一个记录的值？还是直接赋0?
                }
                DateTime mon_begin_date = new DateTime(date.Year, date.Month, 1);
                //本月入职正式工和劳务工人数
                obj.Month_on_board_employees = Month_recordlist.Where(c => c.Date > mon_begin_date && c.Date < tomorrow && c.Department == item).Sum(c => c.Today_on_board_employees);
                obj.Month_on_board_workers = Month_recordlist.Where(c => c.Date > mon_begin_date && c.Date < tomorrow && c.Department == item).Sum(c => c.Today_on_board_workers);
                //本月离职正式工和劳务工人数
                obj.Month_dimission_employees = Month_recordlist.Where(c => c.Date > mon_begin_date && c.Date < tomorrow && c.Department == item).Sum(c => c.Todoy_dimission_employees);
                obj.Month_dimission_workers = Month_recordlist.Where(c => c.Date > mon_begin_date && c.Date < tomorrow && c.Department == item).Sum(c => c.Todoy_dimission_workers);

                //(2)得到date对应是星期几后，根据星期几的值不同，做一个Switch处理不同数据
                //取出本周的记录集合
                List<Personnel_daily> week_recordlist = new List<Personnel_daily>();

                switch (weeknum)
                {
                    case "Monday":
                        week_recordlist = await db.Personnel_daily.Where(c=>c.Date>dateV && c.Date< tomorrow && c.Department == item).OrderBy(c => c.Date).ToListAsync();
                        if (week_recordlist.Count == 0)
                        {
                            obj.Mon_workers = 0;
                            obj.Mon_employees = 0;
                            obj.Week_total_employees = 0;
                            obj.Week_total_workers = 0;
                        }
                        else
                        {
                            obj.Mon_workers = record == null ? 0 : record.Today_on_board_workers;
                            obj.Mon_employees = record == null ? 0 : record.Today_on_board_employees;
                            //本周入职、离职正式工和劳务工人数
                            obj.Week_total_employees = record == null ? 0 : record.Today_on_board_employees;
                            obj.Week_total_workers = record == null ? 0 : record.Today_on_board_workers;
                        }
                        obj.Tues_workers = 0;
                        obj.Tues_employees = 0;
                        obj.Wednes_workers = 0;
                        obj.Wednes_employees = 0;
                        obj.Thurs_workers = 0;
                        obj.Thurs_employees = 0;
                        obj.Fri_workers = 0;
                        obj.Fri_employees = 0;
                        break;
                    case "Tuesday":
                        monday = dateV.AddDays(-1);
                        week_recordlist = await db.Personnel_daily.Where(c => c.Date > monday && c.Date < tomorrow && c.Department == item).OrderBy(c => c.Date).ToListAsync();
                        if (week_recordlist.Count == 0)
                        {
                            obj.Mon_workers = 0;
                            obj.Mon_employees = 0;
                            obj.Tues_workers = 0;
                            obj.Tues_employees = 0;
                            obj.Week_total_employees = 0;
                            obj.Week_total_workers = 0;
                        }
                        else
                        {
                            obj.Mon_workers = week_recordlist.FirstOrDefault().Date.Value.DayOfWeek.ToString() == "Monday" ? week_recordlist.FirstOrDefault().Today_on_board_workers : 0;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Mon_employees = week_recordlist.FirstOrDefault().Date.Value.DayOfWeek.ToString() == "Monday" ? week_recordlist.FirstOrDefault().Today_on_board_employees : 0;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_employees;
                            //本周入职、离职正式工和劳务工人数
                            obj.Week_total_employees = week_recordlist.Sum(c => c.Today_on_board_employees);
                            obj.Week_total_workers = week_recordlist.Sum(c => c.Today_on_board_workers);
                            obj.Tues_workers = record == null ? 0 : record.Today_on_board_workers;
                            obj.Tues_employees = record == null ? 0 : record.Today_on_board_employees;
                        }
                        obj.Wednes_workers = 0;
                        obj.Wednes_employees = 0;
                        obj.Thurs_workers = 0;
                        obj.Thurs_employees = 0;
                        obj.Fri_workers = 0;
                        obj.Fri_employees = 0;

                        break;
                    case "Wednesday": //星期三 
                        monday = dateV.AddDays(-2);
                        week_recordlist = await db.Personnel_daily.Where(c => c.Date > monday && c.Date < tomorrow && c.Department == item).OrderBy(c => c.Date).ToListAsync();
                        if (week_recordlist.Count == 0)
                        {
                            obj.Mon_workers = 0;
                            obj.Mon_employees = 0;
                            obj.Tues_workers = 0;
                            obj.Tues_employees = 0;
                            obj.Wednes_workers = 0;
                            obj.Wednes_employees = 0;
                            obj.Week_total_employees = 0;
                            obj.Week_total_workers = 0;
                        }
                        else
                        {
                            obj.Mon_workers = week_recordlist.FirstOrDefault().Date.Value.DayOfWeek.ToString() == "Monday" ? week_recordlist.FirstOrDefault().Today_on_board_workers : 0;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-2).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Mon_employees = week_recordlist.FirstOrDefault().Date.Value.DayOfWeek.ToString() == "Monday" ? week_recordlist.FirstOrDefault().Today_on_board_employees : 0;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-2).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Tues_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Tues_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_employees;
                            //本周入职、离职正式工和劳务工人数
                            obj.Week_total_employees = week_recordlist.Sum(c => c.Today_on_board_employees);
                            obj.Week_total_workers = week_recordlist.Sum(c => c.Today_on_board_workers);
                        }
                        obj.Wednes_workers = record == null ? 0 : record.Today_on_board_workers;
                        obj.Wednes_employees = record == null ? 0 : record.Today_on_board_employees;
                        obj.Thurs_workers = 0;
                        obj.Thurs_employees = 0;
                        obj.Fri_workers = 0;
                        obj.Fri_employees = 0;
                        break;
                    case "Thursday": //星期四
                        monday = dateV.AddDays(-3);
                        week_recordlist = await db.Personnel_daily.Where(c => c.Date > monday && c.Date < tomorrow && c.Department == item).OrderBy(c => c.Date).ToListAsync();
                        if (week_recordlist.Count == 0)
                        {
                            obj.Mon_workers = 0;
                            obj.Mon_employees = 0;
                            obj.Tues_workers = 0;
                            obj.Tues_employees = 0;
                            obj.Wednes_workers = 0;
                            obj.Wednes_employees = 0;
                            obj.Thurs_workers = 0;
                            obj.Thurs_employees = 0;
                            obj.Week_total_employees = 0;
                            obj.Week_total_workers = 0;
                        }
                        else
                        {
                            obj.Mon_workers = week_recordlist.FirstOrDefault().Date.Value.DayOfWeek.ToString() == "Monday" ? week_recordlist.FirstOrDefault().Today_on_board_workers : 0;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-3).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Mon_employees = week_recordlist.FirstOrDefault().Date.Value.DayOfWeek.ToString() == "Monday" ? week_recordlist.FirstOrDefault().Today_on_board_employees : 0;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-3).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Tues_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-2).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Tues_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-2).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Wednes_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Wednes_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Thurs_workers = record == null ? 0 : record.Today_on_board_workers;
                            obj.Thurs_employees = record == null ? 0 : record.Today_on_board_employees;
                            //本周入职、离职正式工和劳务工人数
                            obj.Week_total_employees = week_recordlist.Sum(c => c.Today_on_board_employees);
                            obj.Week_total_workers = week_recordlist.Sum(c => c.Today_on_board_workers);
                        }
                        obj.Fri_workers = 0;
                        obj.Fri_employees = 0;
                        break;
                    case "Friday": //星期五
                        monday = dateV.AddDays(-4);
                        week_recordlist = await db.Personnel_daily.Where(c => c.Date > monday && c.Date < tomorrow && c.Department == item).OrderBy(c => c.Date).ToListAsync();
                        if (week_recordlist.Count == 0)
                        {
                            obj.Mon_workers = 0;
                            obj.Mon_employees = 0;
                            obj.Tues_workers = 0;
                            obj.Tues_employees = 0;
                            obj.Wednes_workers = 0;
                            obj.Wednes_employees = 0;
                            obj.Thurs_workers = 0;
                            obj.Thurs_employees = 0;
                            obj.Fri_workers = 0;
                            obj.Fri_employees = 0;
                            obj.Week_total_employees = 0;
                            obj.Week_total_workers = 0;
                        }
                        else
                        {
                            obj.Mon_workers = week_recordlist.FirstOrDefault().Date.Value.DayOfWeek.ToString() == "Monday" ? week_recordlist.FirstOrDefault().Today_on_board_workers : 0;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-3).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Mon_employees = week_recordlist.FirstOrDefault().Date.Value.DayOfWeek.ToString() == "Monday" ? week_recordlist.FirstOrDefault().Today_on_board_employees : 0;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-3).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Tues_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-2).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Tues_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-2).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Wednes_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Wednes_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Thurs_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Thurs_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Fri_workers = record == null ? 0 : record.Today_on_board_workers;
                            obj.Fri_employees = record == null ? 0 : record.Today_on_board_employees;
                            //本周入职、离职正式工和劳务工人数
                            obj.Week_total_employees = week_recordlist.Sum(c => c.Today_on_board_employees);
                            obj.Week_total_workers = week_recordlist.Sum(c => c.Today_on_board_workers);
                        }
                        break;
                    case "Saturday":
                        monday = dateV.AddDays(-5);
                        week_recordlist = await db.Personnel_daily.Where(c => c.Date > monday && c.Date < tomorrow && c.Department == item).OrderBy(c => c.Date).ToListAsync();
                        if (week_recordlist.Count == 0)
                        {
                            obj.Mon_workers = 0;
                            obj.Mon_employees = 0;
                            obj.Tues_workers = 0;
                            obj.Tues_employees = 0;
                            obj.Wednes_workers = 0;
                            obj.Wednes_employees = 0;
                            obj.Thurs_workers = 0;
                            obj.Thurs_employees = 0;
                            obj.Fri_workers = 0;
                            obj.Fri_employees = 0;
                            obj.Week_total_employees = 0;
                            obj.Week_total_workers = 0;
                        }
                        else
                        {
                            obj.Mon_workers = week_recordlist.FirstOrDefault().Date.Value.DayOfWeek.ToString() == "Monday" ? week_recordlist.FirstOrDefault().Today_on_board_workers : 0;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-3).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Mon_employees = week_recordlist.FirstOrDefault().Date.Value.DayOfWeek.ToString() == "Monday" ? week_recordlist.FirstOrDefault().Today_on_board_employees : 0;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-3).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Tues_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-2).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Tues_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-2).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Wednes_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Wednes_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Thurs_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Thurs_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Fri_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Friday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Friday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Fri_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Friday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Friday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_employees;
                            //本周入职、离职正式工和劳务工人数
                            obj.Week_total_employees = week_recordlist.Sum(c => c.Today_on_board_employees);
                            obj.Week_total_workers = week_recordlist.Sum(c =>c.Today_on_board_workers);
                        }
                        break;
                    case "Sunday":
                        monday = dateV.AddDays(-6);
                        week_recordlist = await db.Personnel_daily.Where(c => c.Date > monday && c.Date < tomorrow && c.Department == item).OrderBy(c => c.Date).ToListAsync();
                        if (week_recordlist.Count == 0)
                        {
                            obj.Mon_workers = 0;
                            obj.Mon_employees = 0;
                            obj.Tues_workers = 0;
                            obj.Tues_employees = 0;
                            obj.Wednes_workers = 0;
                            obj.Wednes_employees = 0;
                            obj.Thurs_workers = 0;
                            obj.Thurs_employees = 0;
                            obj.Fri_workers = 0;
                            obj.Fri_employees = 0;
                            obj.Week_total_employees = 0;
                            obj.Week_total_workers = 0;
                        }
                        else
                        {
                            obj.Mon_workers = week_recordlist.FirstOrDefault().Date.Value.DayOfWeek.ToString() == "Monday" ? week_recordlist.FirstOrDefault().Today_on_board_workers : 0;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-3).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Mon_employees = week_recordlist.FirstOrDefault().Date.Value.DayOfWeek.ToString() == "Monday" ? week_recordlist.FirstOrDefault().Today_on_board_employees : 0;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-3).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Tues_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-2).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Tues_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Tuesday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-2).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Wednes_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Wednes_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Wednesday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Thurs_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Thurs_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Thursday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_employees;
                            obj.Fri_workers = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Friday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Friday").FirstOrDefault().Today_on_board_workers;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_workers;
                            obj.Fri_employees = week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Friday").Count() == 0 ? 0 : week_recordlist.Where(c => c.Date.Value.DayOfWeek.ToString() == "Friday").FirstOrDefault().Today_on_board_employees;//db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == dateV.AddDays(-1).Day).FirstOrDefault().Today_on_board_employees;
                            //本周入职、离职正式工和劳务工人数
                            obj.Week_total_employees = week_recordlist.Sum(c => c.Today_on_board_employees);
                            obj.Week_total_workers = week_recordlist.Sum(c =>c.Today_on_board_workers);
                        }
                        break;
                }
                //(3)把内容添加到JSON
                arr.Add(obj);
            }
            ////Json格式的要求{total:22,rows:{}}
            ////构造成Json的格式传递
            //var result = new { total = pagerInfo.RecordCount, rows = objList };
            //return ToJsonContentDate(result);
            if (arr == null)
            {
                return Content("无记录");
            }
            else
            {
                return Content(JsonConvert.SerializeObject(arr));
            }
        }
        #endregion


        #region-----------------Daily2方法
        public async Task<ActionResult> Daily2()
        {
            List<Personnel_daily> result = new List<Personnel_daily>();
            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            ViewBag.date = date.ToString("yyyy-MM-dd");
            //部门清单
            List<string> departmentlist = new List<string>
            {
                "PC部","MC部","SMT部","装配1部","装配2部","配套加工部","技术部","品质部","行政后勤部","人力资源部","财务部","品质技术中心总监","轮值厂长"
            };
            var Month_recordlist = await db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month).ToListAsync();
            foreach (var it in departmentlist)
            {
                var today_record = Month_recordlist.Where(c => c.Department == it && c.Date.Value.Date == date.Date);
                if (today_record.Count() > 0)
                {
                    //当天有记录
                    result.Add(today_record.FirstOrDefault());
                }
                else
                {
                    //当天没有记录时
                    var lastrecord = await db.Personnel_daily.Where(c => c.Department == it).OrderByDescending(i => i.Id).FirstOrDefaultAsync();
                    lastrecord.Employees_personnel = 0;
                    lastrecord.Workers_personnel = 0;
                    lastrecord.Today_on_board_employees = 0;
                    lastrecord.Today_on_board_workers = 0;
                    lastrecord.Date = new DateTime();
                    lastrecord.Reporter = "未上报";
                    result.Add(lastrecord);
                }
            }
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
                    collection = Month_recordlist.Where(c => c.Date > begindateTue && c.Date < enddateTue).ToList();
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
                    collection = Month_recordlist.Where(c => c.Date > begindateWed && c.Date < enddateWed).ToList();
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
                    collection = Month_recordlist.Where(c => c.Date > begindateThu && c.Date < enddateThu).ToList();
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
                    collection = Month_recordlist.Where(c => c.Date > begindateFri && c.Date < enddateFri).ToList();
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
                    collection = Month_recordlist.Where(c => c.Date > begindateSat && c.Date < enddateSat).ToList();
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
                    collection = Month_recordlist.Where(c => c.Date > begindateSun && c.Date < enddateSun).ToList();
                    date = date.AddDays(-6);
                    for (int i = 0; i < 5; i++)
                    {
                        dateVal.Add(date.ToLongDateString());
                        datelist.Add(date);
                        date = date.AddDays(1);
                    }
                    break;
            }
            //本周实际人数JSON,创建JSON对象
            JObject On_Board_JsonObj = new JObject
            {
                {"日期",JsonConvert.SerializeObject(dateVal) },
            };   
            //每日记录
            foreach (var item in departmentlist)
            {
                var dprecord = collection.Where(d => d.Department == item).OrderBy(c=>c.Date).ToList();
                //本周实际每日入职人数
                List<int> te = new List<int>();
                foreach (var d in datelist)
                {
                    var has_record = dprecord.Where(c => c.Date.Value.Date == d.Date).Count();
                    if (has_record > 0)
                    {
                        te.Add(dprecord.Where(c => c.Date.Value.Date == d.Date).FirstOrDefault().Today_on_board_workers);
                        te.Add(dprecord.Where(c => c.Date.Value.Date == d.Date).FirstOrDefault().Today_on_board_employees);
                    }
                    else  //部门今天没有记录的
                    {
                        te.Add(0);
                        te.Add(0);
                    }
                }
                te.Add(dprecord.Sum(c=>c.Today_on_board_workers+c.Today_on_board_employees));//本周入职合计
                //当月部门入职、离职人数合计
                te.Add(Month_recordlist.Where(c => c.Department == item).Sum(c => c.Today_on_board_employees + c.Today_on_board_workers));//当月入职人数
                te.Add(Month_recordlist.Where(c => c.Department == item).Sum(c => c.Todoy_dimission_employees + c.Todoy_dimission_workers));//当月离职人数
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
            //当月入职、离职人数合计
            sum.Add(db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month).Sum(c => c.Today_on_board_employees + c.Today_on_board_workers));//当月入职人数合计
            sum.Add(db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month).Sum(c => c.Todoy_dimission_employees + c.Todoy_dimission_workers));//当月离职人数合计
            On_Board_JsonObj.Add("合计",JsonConvert.SerializeObject(sum));
            ViewBag.json = On_Board_JsonObj;
            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> Daily2(string section , DateTime date)
        {
            DateTime dateAdd1 = date.AddDays(1);//选择的时间加一天的0:00:00
            ViewBag.date = Convert.ToDateTime(date).ToString("yyyy-MM-dd");
            List<Personnel_daily> result = new List<Personnel_daily>();
            List<string> departmentlist = new List<string>
            {
                "PC部","MC部","SMT部","装配1部","装配2部","配套加工部","技术部","品质部","行政后勤部","人力资源部","财务部","品质技术中心总监","轮值厂长"
            };
                var Month_recordlist = await db.Personnel_daily.Where(c => c.Date.Value.Year == date.Year && c.Date.Value.Month == date.Month).ToListAsync();

                if (section == null) { section = "week"; }
                foreach (var it in departmentlist)
                {
                    if (Month_recordlist.Where(c => c.Department == it && c.Date.Value.Date == date.Date).Count() > 0)
                    {
                        //当天有记录
                        result.Add(Month_recordlist.Where(c => c.Department == it && c.Date.Value.Date == date.Date).FirstOrDefault());
                    }
                    else
                    {
                        //当天没有记录时
                        var lastrecord = Month_recordlist.Where(c => c.Department == it).OrderByDescending(i => i.Id).FirstOrDefault();
                        if (lastrecord != null)
                        {
                            lastrecord.Employees_personnel = 0;
                            lastrecord.Workers_personnel = 0;
                            lastrecord.Today_on_board_employees = 0;
                            lastrecord.Today_on_board_workers = 0;
                            lastrecord.Date = new DateTime();
                            lastrecord.Reporter = "未上报";
                            result.Add(lastrecord);
                        }
                    }
                }
                //switch (section)
                //{
                //    case "week":
                //        //result = await Month_recordlist.Where(c=>c.Date.Value.Year==date.Year && c.Date.Value.Month == date.Month && c.Date.Value.Day == date.Day).ToListAsync();
                //        foreach (var it in departmentlist)
                //        {
                //            result.Add(Month_recordlist.Where(c => c.Department == it).OrderByDescending(c => c.Date).FirstOrDefault());
                //        }
                //        break;
                //    case "month":
                //        break;
                //    case "year":
                //        break;
                //}
                var weeknum = date.DayOfWeek;
                List<string> dateVal = new List<string>();
                List<DateTime> datelist = new List<DateTime>();
                List<Personnel_daily> collection = new List<Personnel_daily>();
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
                        collection = Month_recordlist.Where(c => c.Date > begindateTue && c.Date < enddate).ToList();
                        date = date.AddDays(-1);
                        for (int i = 0; i < 5; i++)
                        {
                            dateVal.Add(date.ToLongDateString());
                            datelist.Add(date);
                            date = date.AddDays(1);
                        }
                        break;
                    case "Wednesday":
                        DateTime begindateWed = date.AddDays(-2);
                        collection = Month_recordlist.Where(c => c.Date > begindateWed && c.Date < enddate).ToList();
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
                        collection = Month_recordlist.Where(c => c.Date > begindateThu && c.Date < enddate).ToList();
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
                        collection = Month_recordlist.Where(c => c.Date > begindateFri && c.Date < enddate).ToList();
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
                        collection = Month_recordlist.Where(c => c.Date > begindateSat && c.Date < enddate).ToList();
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
                        collection = Month_recordlist.Where(c => c.Date > begindateSun && c.Date < enddate).ToList();
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
                {"日期",JsonConvert.SerializeObject(dateVal) }
                };   //创建JSON对象
                //每日记录
                foreach (var item in departmentlist)
                {
                    var dprecord = collection.Where(d => d.Department == item).OrderBy(c => c.Date).ToList();
                    List<int> te = new List<int>();
                    foreach (var d in datelist)
                    {
                        if (dprecord.Where(c => c.Date.Value.Date == d.Date).Count() > 0)
                        {
                            te.Add(dprecord.Where(c => c.Date.Value.Date == d.Date).FirstOrDefault().Today_on_board_workers);
                            te.Add(dprecord.Where(c => c.Date.Value.Date == d.Date).FirstOrDefault().Today_on_board_employees);
                        }
                        else
                        {
                            te.Add(0);
                            te.Add(0);
                        }
                    }
                    te.Add(dprecord.Sum(c => c.Today_on_board_workers + c.Today_on_board_employees));//本周入职合计
                    //当月部门入职、离职人数合计
                    te.Add(Month_recordlist.Where(c => c.Date < dateAdd1 && c.Department == item).Sum(c => c.Today_on_board_employees + c.Today_on_board_workers));//当月入职人数
                    te.Add(Month_recordlist.Where(c => c.Date < dateAdd1 && c.Department == item).Sum(c => c.Todoy_dimission_employees + c.Todoy_dimission_workers));//当月离职人数
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
                //当月入职、离职人数合计
                sum.Add(Month_recordlist.Where(c => c.Date < dateAdd1).Sum(c => c.Today_on_board_employees + c.Today_on_board_workers));//当月入职人数合计
                sum.Add(Month_recordlist.Where(c => c.Date < dateAdd1).Sum(c => c.Todoy_dimission_employees + c.Todoy_dimission_workers));//当月离职人数合计
                On_Board_JsonObj.Add("合计", JsonConvert.SerializeObject(sum));
                ViewBag.json = On_Board_JsonObj;
                return View(result);
        }

        #endregion


        #region --------------------上传文件(Excel)方法
        [HttpPost]
        public ActionResult UploadFile(string department,DateTime dateTime)
        {
            if (Request.Files.Count > 0)
            {
                //HttpPostedFileBase file = Request.Files[0];    //方法1
                HttpPostedFileBase file = Request.Files["uploadfile"];   //方法2
                var fileType = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                var re = String.Equals(fileType, ".xls") == true||String.Equals(fileType, ".xlsx") == true ? false:true;
                if(re)
                {
                    return Content("<script>alert('您选择文件的文件类型不正确，请选择xls或xlsx类型文件！');history.go(-1);</script>");
                }
                string ReName = dateTime.Year + "-" + dateTime.Month.ToString("00") + "-" + dateTime.Day.ToString("00") + department + "人员动态";  //重命名文件
                if (Directory.Exists(@"D:\MES_Data\Personnel_Files\人员动态表\" + dateTime.Year + "\\" + department + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\Personnel_Files\人员动态表\" + dateTime.Year + "\\" + department + "\\");
                }
                //List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\Personnel_Files\人员动态表\"  + dateTime.Year + "\\" + department + "\\");
                file.SaveAs(@"D:\MES_Data\Personnel_Files\人员动态表\" + dateTime.Year + "\\" + department + "\\" + ReName + fileType);
                return Content("<script>alert('上传成功！');window.location.href='../Personnel/Daily';</script>");
            }
            return Content("<script>alert('请选择文件');history.go(-1);</script>");
        }

        #region --------------------返回指定目录下所有文件信息
        /// <summary>  
        /// 返回指定目录下所有文件信息  
        /// </summary>  
        /// <param name="strDirectory">目录字符串</param>  
        /// <returns></returns>  
        public List<FileInfo> GetAllFilesInDirectory(string strDirectory)
        {
            List<FileInfo> listFiles = new List<FileInfo>(); //保存所有的文件信息  
            DirectoryInfo directory = new DirectoryInfo(strDirectory);
            DirectoryInfo[] directoryArray = directory.GetDirectories();
            FileInfo[] fileInfoArray = directory.GetFiles();
            if (fileInfoArray.Length > 0) listFiles.AddRange(fileInfoArray);
            foreach (DirectoryInfo _directoryInfo in directoryArray)
            {
                DirectoryInfo directoryA = new DirectoryInfo(_directoryInfo.FullName);
                DirectoryInfo[] directoryArrayA = directoryA.GetDirectories();
                FileInfo[] fileInfoArrayA = directoryA.GetFiles();
                if (fileInfoArrayA.Length > 0) listFiles.AddRange(fileInfoArrayA);
                GetAllFilesInDirectory(_directoryInfo.FullName);//递归遍历  
            }
            return listFiles;
        }
        #endregion

        #endregion


        #region --------------------下载部门人员动态Excel文件
        [HttpPost]
        public ActionResult GetExcelFile(string department,DateTime date)
        {
            string file1 = "/Personnel_Files/人员动态表/" + date.Year + "/" + department + "/" + date.Year + "-" + date.Month.ToString("00") + "-" + date.Day.ToString("00") + department + "人员动态.xls";
            string file2 = "/Personnel_Files/人员动态表/" + date.Year + "/" + department + "/" + date.Year + "-" + date.Month.ToString("00") + "-" + date.Day.ToString("00") + department + "人员动态.xlsx";
            if (System.IO.File.Exists(Server.MapPath(file2)))
            {
                return File(file2,"xlsx/plain", date.Year + "-" + date.Month.ToString("00") + "-" + date.Day.ToString("00") + department + "人员动态.xlsx");
            }
            else
            {
                if (System.IO.File.Exists(Server.MapPath(file1)))
                {
                    return File(file1, "xls/plain", date.Year + "-" + date.Month.ToString("00") + "-" + date.Day.ToString("00") + department + "人员动态.xls");
                }
                else //如果文件夹不存在，提示信息
                {
                    //return File("", "text/plain");
                    return Content("文件不存在");
                }
            }
        }
        #endregion


        #region --------------------Details方法
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
        #endregion


        #region --------------------Create方法

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
        public async Task<ActionResult> Create([Bind(Include = "Id,Department,Principal,Aurhorized_personnel,Need_personnel,Employees_personnel,Workers_personnel,Today_on_board_employees,Today_on_board_workers,Interview,Todoy_dimission_employees,Todoy_dimission_workers,Resigned_that_month,Resigned_workers_that_month,Date,Reporter")] Personnel_daily personnel_daily)
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
        #endregion


        #region --------------------Edit 方法

        // GET: Personnel/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "文员" || ((Users)Session["User"]).Role == "系统管理员")
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
            else
            {
                return Content("<script>alert('对不起，您没有权修改人事日报信息，请联系人力资源部！');window.location.href='../Personnel/Daily';</script>");
            }

        }

        // POST: Personnel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Department,Principal,Aurhorized_personnel,Need_personnel,Employees_personnel,Workers_personnel,Today_on_board_employees,Today_on_board_workers,Interview,Todoy_dimission_employees,Todoy_dimission_workers,Resigned_that_month,Resigned_workers_that_month,Date,Reporter")] Personnel_daily personnel_daily)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personnel_daily).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Daily");
            }
            return View(personnel_daily);
        }
        #endregion


        #region --------------------Delete 方法

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
        #endregion


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
