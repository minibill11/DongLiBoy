﻿using JianHeMES.AuthAttributes;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class Personnel_WorkingHours_ReportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region---页面
        public ActionResult Personnel_Working()//人员工时报表首页
        {
            return View();
        }

        public ActionResult Group_Working()//班组工时报表首页
        {
            return View();
        }

        public ActionResult Department_Working()//部门工时报表首页
        {
            return View();
        }

        public ActionResult OvertimeLeague()//加班排行榜页面
        {
            return View();
        }

        public ActionResult TimeDifference()//月工时差异表
        {
            return View();
        }

        public ActionResult MonthlyReport()//月报/半月报页面
        {
            return View();
        }

        public ActionResult MonthlyContrast()//月报/半月报对比页面
        {
            return View();
        }

        public ActionResult CorporateCalendar()//企业行事历
        {
            return View();
        }

        #endregion

        #region---人员工时明细表方法
        //查询方法
        public ActionResult Schedule_PersonnelHours(int? year, int? month, int? week)
        {
            JObject table = new JObject();
            JObject personnel = new JObject();
            if (year == null || month == null || week == null)
            {
                table.Add("Meg", false);
                table.Add("Feg", year == null ? "年份" : "" + month == null ? "月份" : "" + week == null ? "周" : "" + "未选择！");
                return Content(JsonConvert.SerializeObject(table));
            }
            var workList = db.Personnel_WorkingHours.Where(c => c.Year == year && c.Month == month && c.Week == week).ToList();
            if (workList.Count > 0)
            {
                foreach (var item in workList)
                {
                    var hours = workList.Where(c => c.Year == year && c.Month == month && c.Week == week && c.UserNumber == item.UserNumber).FirstOrDefault();
                    //Id
                    personnel.Add("Id", hours.Id == 0 ? 0 : hours.Id);
                    //工号
                    personnel.Add("UserNumber", hours.UserNumber == null ? null : hours.UserNumber);
                    //姓名
                    personnel.Add("UserName", hours.UserName == null ? null : hours.UserName);
                    //部门
                    personnel.Add("Department", hours.Department == null ? null : hours.Department);
                    //班组
                    personnel.Add("Group", hours.Group == null ? null : hours.Group);
                    //正班工时
                    personnel.Add("ZhengBan_Work", hours.ZhengBan_Work == 0 ? 0 : hours.ZhengBan_Work);
                    //加班1(工作日加班)
                    personnel.Add("Work_Overtime1", hours.Work_Overtime1 == 0 ? 0 : hours.Work_Overtime1);
                    //加班2(假日加班：礼拜六礼拜天)
                    personnel.Add("Work_Overtime2", hours.Work_Overtime2 == 0 ? 0 : hours.Work_Overtime2);
                    //加班3(节日加班：如元旦)
                    personnel.Add("Work_Overtime3", hours.Work_Overtime3 == 0 ? 0 : hours.Work_Overtime3);
                    //加班1日人均加班工时
                    personnel.Add("Work_Overtime_Average1", hours.Work_Overtime_Average1 == 0 ? 0 : hours.Work_Overtime_Average1);
                    //加班2日人均加班工时
                    personnel.Add("Work_Overtime_Average2", hours.Work_Overtime_Average2 == 0 ? 0 : hours.Work_Overtime_Average2);
                    //加班3日人均加班工时
                    personnel.Add("Work_Overtime_Average3", hours.Work_Overtime_Average3 == 0 ? 0 : hours.Work_Overtime_Average3);
                    //人员加班总值
                    personnel.Add("Total_Overtime", hours.Total_Overtime == 0 ? 0 : hours.Total_Overtime);
                    table.Add("Table", personnel);
                    personnel = new JObject();
                }

            }
            else
            {
                table.Add("Meg", false);
                table.Add("Feg", year + "年" + month + "月第" + week + "周没有记录或尝未输入数据！");
                return Content(JsonConvert.SerializeObject(table));
            }
            return Content(JsonConvert.SerializeObject(table));
        }

        //批量添加方法       
        public ActionResult Batch_PersonnelHours(List<Personnel_WorkingHours> workingHours, int year, int month, int week)
        {
            JObject batch = new JObject();
            int count = 0;
            if (workingHours.Count > 0 && year != 0 && month != 0 && week != 0)
            {
                var CreateDate = DateTime.Now;
                var Creator = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
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
                foreach (var data in workingHours)
                {
                    int Working = db.Personnel_WorkingHours.Count(c => c.Year == data.Year && c.Month == data.Month && c.Week == data.Week && c.UserNumber == data.UserNumber);
                    if (Working > 0)
                    {
                        if (result == "")
                        {
                            result = data.Year + "年" + data.Month + "月第" + data.Week + "周" + data.UserNumber;
                        }
                        else
                        {
                            result = result + "," + data.Year + "年" + data.Month + "月第" + data.Week + "周" + data.UserNumber;
                        }
                    }
                }
                if (result != "")
                {
                    batch.Add("Meg", false);
                    batch.Add("Feg", result + "数据已经存在，不能再输入！");
                    return Content(JsonConvert.SerializeObject(batch));
                }
                foreach (var data in workingHours)
                {
                    data.CreateDate = CreateDate;
                    data.Creator = Creator;
                    data.Monday = Monday;
                    data.Sunday = Sunday;
                    data.Year = year;
                    data.Month = month;
                    data.Week = week;
                    db.Personnel_WorkingHours.Add(data);
                    count += db.SaveChanges();
                }
                if (count == workingHours.Count)
                {
                    batch.Add("Meg", true);
                    batch.Add("Feg", "保存成功！");
                    return Content(JsonConvert.SerializeObject(batch));
                }
                else
                {
                    batch.Add("Meg", false);
                    batch.Add("Feg", "保存失败！");
                    return Content(JsonConvert.SerializeObject(batch));
                }
            }
            batch.Add("Meg", false);
            batch.Add("Feg", "数据错误！");
            return Content(JsonConvert.SerializeObject(batch));
        }

        //修改数据
        public ActionResult Modify_WorkingHours(Personnel_WorkingHours hoursList, int year, int month, int week)
        {
            JObject retul = new JObject();
            if (year != 0 && month != 0 && week != 0 && hoursList != null)
            {
                hoursList.ModifyName = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                hoursList.ModifyDate = DateTime.Now;
                db.Entry(hoursList).State = EntityState.Modified;//修改数据
                int count = db.SaveChanges();//保存数据库
                if (count > 0)
                {
                    retul.Add("Meg", true);
                    retul.Add("Feg", "修改成功！");
                    retul.Add("HoursList", JsonConvert.SerializeObject(hoursList));
                    return Content(JsonConvert.SerializeObject(retul));
                }
                else
                {
                    retul.Add("Meg", false);
                    retul.Add("Feg", "修改失败！");
                    return Content(JsonConvert.SerializeObject(retul));
                }
            }
            retul.Add("Meg", false);
            retul.Add("Feg", "数据错误！");
            return Content(JsonConvert.SerializeObject(retul));
        }

        #endregion     

        #region---企业行事历方法
        //查询方法
        public ActionResult CalendarTable(int? year, int? month)
        {
            JObject table = new JObject();
            JObject retul = new JObject();
            List<Personnel_Corporate_Calendar> corporate = new List<Personnel_Corporate_Calendar>();
            if (year != 0 && month != 0)
            {
                corporate = db.Personnel_Corporate_Calendar.Where(c => c.Date.Year == year && c.Date.Month == month).ToList();
            }
            if (year != 0 && month == 0)
            {
                corporate = db.Personnel_Corporate_Calendar.Where(c => c.Date.Year == year).ToList();
            }
            if (corporate.Count > 0)
            {
                foreach (var item in corporate)
                {
                    //id
                    table.Add("Id", item.Id == 0 ? 0 : item.Id);
                    //日期
                    table.Add("Date", item.Date);
                    //星期
                    table.Add("Week", item.Week == null ? null : item.Week);
                    //日历类型
                    table.Add("CalendarType", item.CalendarType == null ? null : item.CalendarType);
                    //日历类型名称
                    table.Add("Calendar_TypeName", item.Calendar_TypeName == null ? null : item.Calendar_TypeName);
                    //日历名称
                    table.Add("CalendarName", item.CalendarName == null ? null : item.CalendarName);
                    //时间类型
                    table.Add("TimeType", item.TimeType == null ? null : item.TimeType);
                    //时间类型名称
                    table.Add("TimeTypeName", item.TimeTypeName == null ? null : item.TimeTypeName);
                    //备注
                    table.Add("Remark", item.Remark == null ? null : item.Remark);
                    retul.Add("Table", table);
                    table = new JObject();
                }
            }
            return Content(JsonConvert.SerializeObject(retul));
        }

        //批量添加数据
        public ActionResult Batch_CorporateCalendar(List<Personnel_Corporate_Calendar> calendars)
        {
            JObject corpor = new JObject();
            JArray res = new JArray();
            DateTime? retul = null;
            int count = 0;
            if (calendars.Count > 0)
            {
                foreach (var item in calendars)
                {
                    item.CreateDate = DateTime.Now;
                    item.Creator = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                    if (db.Personnel_Corporate_Calendar.Count(c => c.Date == item.Date) > 0)
                    {
                        retul = item.Date;
                        res.Add(retul);
                    }
                }
                if (retul != null)//判断repat(存储相同数据的字段)是否为空
                {
                    corpor.Add("Meg", false);
                    corpor.Add("Feg", "数据重复！");
                    corpor.Add("Retul", JsonConvert.SerializeObject(res));
                    return Content(JsonConvert.SerializeObject(corpor));
                }
                db.Personnel_Corporate_Calendar.AddRange(calendars);//把数据保存到相对应的表里
                count += db.SaveChanges();
                if (count == calendars.Count)
                {
                    corpor.Add("Meg", true);
                    corpor.Add("Feg", "添加成功！");
                    return Content(JsonConvert.SerializeObject(corpor));
                }
                else
                {
                    corpor.Add("Meg", false);
                    corpor.Add("Feg", "添加失败！");
                    return Content(JsonConvert.SerializeObject(corpor));
                }
            }
            corpor.Add("Meg", false);
            corpor.Add("Feg", "数据错误！");
            return Content(JsonConvert.SerializeObject(corpor));
        }

        //修改数据
        public ActionResult Modify_Corporate(Personnel_Corporate_Calendar corporate)
        {
            JObject table = new JObject();
            if (corporate.Date != null && corporate != null)
            {
                corporate.ModifyName = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                corporate.ModifyDate = DateTime.Now;
                db.Entry(corporate).State = EntityState.Modified;//修改数据
                int count = db.SaveChanges();//保存数据库
                if (count > 0)
                {
                    table.Add("Meg", true);
                    table.Add("Feg", "修改成功！");
                    table.Add("Corporate", JsonConvert.SerializeObject(corporate));
                    return Content(JsonConvert.SerializeObject(table));
                }
                else
                {
                    table.Add("Meg", false);
                    table.Add("Feg", "修改失败！");
                    return Content(JsonConvert.SerializeObject(table));
                }
            }
            table.Add("Meg", false);
            table.Add("Feg", "数据错误！");
            return Content(JsonConvert.SerializeObject(table));
        }

        #endregion

    }


    //Api接口部分

    public class Personnel_WorkingHours_Report_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController common = new CommonController();

        #region---企业行事历方法
        //查询方法
        [HttpPost]
        [ApiAuthorize]
        public JObject CalendarTable([System.Web.Http.FromBody]JObject data)
        {
            JObject table = new JObject();
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year == 0 ? 0 : obj.year;//年
            int month = obj.month == 0 ? 0 : obj.month;//月
            List<Personnel_Corporate_Calendar> corporate = new List<Personnel_Corporate_Calendar>();
            if (year != 0 && month != 0)
            {
                corporate = db.Personnel_Corporate_Calendar.Where(c => c.Date.Year == year && c.Date.Month == month).ToList();
            }
            if (year != 0 && month == 0)
            {
                corporate = db.Personnel_Corporate_Calendar.Where(c => c.Date.Year == year).ToList();
            }
            if (corporate.Count > 0)
            {
                foreach (var item in corporate)
                {
                    //id
                    table.Add("Id", item.Id == 0 ? 0 : item.Id);
                    //日期
                    table.Add("Date", item.Date);
                    //星期
                    table.Add("Week", item.Week == null ? null : item.Week);
                    //日历类型
                    table.Add("CalendarType", item.CalendarType == null ? null : item.CalendarType);
                    //日历类型名称
                    table.Add("Calendar_TypeName", item.Calendar_TypeName == null ? null : item.Calendar_TypeName);
                    //日历名称
                    table.Add("CalendarName", item.CalendarName == null ? null : item.CalendarName);
                    //时间类型
                    table.Add("TimeType", item.TimeType == null ? null : item.TimeType);
                    //时间类型名称
                    table.Add("TimeTypeName", item.TimeTypeName == null ? null : item.TimeTypeName);
                    //备注
                    table.Add("Remark", item.Remark == null ? null : item.Remark);
                    result.Add("Table", table);
                    table = new JObject();
                }
            }
            return common.GetModuleFromJobjet(result);
        }

        //批量添加数据
        [HttpPost]
        [ApiAuthorize]
        public JObject Batch_CorporateCalendar([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            JArray res = new JArray();
            DateTime? retul = null;
            int count = 0;
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            List<Personnel_Corporate_Calendar> calendars = obj.calendars == null ? null : obj.calendars;
            if (calendars.Count > 0)
            {
                foreach (var item in calendars)
                {
                    item.CreateDate = DateTime.Now;
                    item.Creator = auth.UserName;
                    if (db.Personnel_Corporate_Calendar.Count(c => c.Date == item.Date) > 0)
                    {
                        retul = item.Date;
                        res.Add(retul);
                    }
                }
                if (retul != null)//判断repat(存储相同数据的字段)是否为空
                {
                    result.Add("Meg", false);
                    result.Add("Feg", "数据重复！");
                    result.Add("Retul", JsonConvert.SerializeObject(res));
                    return common.GetModuleFromJobjet(result);
                }
                db.Personnel_Corporate_Calendar.AddRange(calendars);//把数据保存到相对应的表里
                count += db.SaveChanges();
                if (count == calendars.Count)
                {
                    result.Add("Meg", true);
                    result.Add("Feg", "添加成功！");
                    return common.GetModuleFromJobjet(result);
                }
                else
                {
                    result.Add("Meg", false);
                    result.Add("Feg", "添加失败！");
                    return common.GetModuleFromJobjet(result);
                }
            }
            result.Add("Meg", false);
            result.Add("Feg", "数据错误！");
            return common.GetModuleFromJobjet(result);
        }

        //修改数据
        [HttpPost]
        [ApiAuthorize]
        public JObject Modify_Corporate([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            Personnel_Corporate_Calendar corporate = obj.corporate == null ? null : obj.corporate;

            if (corporate.Date != null && corporate != null)
            {
                corporate.ModifyName = auth.UserName;
                corporate.ModifyDate = DateTime.Now;
                db.Entry(corporate).State = EntityState.Modified;//修改数据
                int count = db.SaveChanges();//保存数据库
                if (count > 0)
                {
                    result.Add("Meg", true);
                    result.Add("Feg", "修改成功！");
                    result.Add("Corporate", JsonConvert.SerializeObject(corporate));
                    return common.GetModuleFromJobjet(result);
                }
                else
                {
                    result.Add("Meg", false);
                    result.Add("Feg", "修改失败！");
                    return common.GetModuleFromJobjet(result);
                }
            }
            result.Add("Meg", false);
            result.Add("Feg", "数据错误！");
            return common.GetModuleFromJobjet(result);
        }


        #endregion

        #region---人员工时明细表方法

        //查询方法
        [HttpPost]
        [ApiAuthorize]
        public JObject Schedule_PersonnelHours([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year == 0 ? 0 : obj.year;//年
            int month = obj.month == 0 ? 0 : obj.month;//月
            int week = obj.week == 0 ? 0 : obj.week;//周
            JObject personnel = new JObject();
            if (year == 0 || month == 0 || week == 0)
            {
                result.Add("Result", false);
                result.Add("Message", year == 0 ? "年份" : "" + month == null ? "月份" : "" + week == null ? "周" : "" + "未选择！");
                return common.GetModuleFromJobjet(result);
            }
            var workList = db.Personnel_WorkingHours.Where(c => c.Year == year && c.Month == month && c.Week == week).ToList();
            if (workList.Count > 0)
            {
                foreach (var item in workList)
                {
                    var hours = workList.Where(c => c.UserNumber == item.UserNumber).FirstOrDefault();
                    //Id
                    personnel.Add("Id", hours.Id == 0 ? 0 : hours.Id);
                    //工号
                    personnel.Add("UserNumber", hours.UserNumber == null ? null : hours.UserNumber);
                    //姓名
                    personnel.Add("UserName", hours.UserName == null ? null : hours.UserName);
                    //部门
                    personnel.Add("Department", hours.Department == null ? null : hours.Department);
                    //班组
                    personnel.Add("Group", hours.Group == null ? null : hours.Group);
                    //正班工时
                    personnel.Add("ZhengBan_Work", hours.ZhengBan_Work == 0 ? 0 : hours.ZhengBan_Work);
                    //加班1(工作日加班)
                    personnel.Add("Work_Overtime1", hours.Work_Overtime1 == 0 ? 0 : hours.Work_Overtime1);
                    //加班2(假日加班：礼拜六礼拜天)
                    personnel.Add("Work_Overtime2", hours.Work_Overtime2 == 0 ? 0 : hours.Work_Overtime2);
                    //加班3(节日加班：如元旦)
                    personnel.Add("Work_Overtime3", hours.Work_Overtime3 == 0 ? 0 : hours.Work_Overtime3);
                    //加班1日人均加班工时
                    personnel.Add("Work_Overtime_Average1", hours.Work_Overtime_Average1 == 0 ? 0 : hours.Work_Overtime_Average1);
                    //加班2日人均加班工时
                    personnel.Add("Work_Overtime_Average2", hours.Work_Overtime_Average2 == 0 ? 0 : hours.Work_Overtime_Average2);
                    //加班3日人均加班工时
                    personnel.Add("Work_Overtime_Average3", hours.Work_Overtime_Average3 == 0 ? 0 : hours.Work_Overtime_Average3);
                    //人员加班总值
                    personnel.Add("Total_Overtime", hours.Total_Overtime == 0 ? 0 : hours.Total_Overtime);
                    result.Add("Table", personnel);
                    personnel = new JObject();
                }

            }
            else
            {
                result.Add("Result", false);
                result.Add("Message", year + "年" + month + "月第" + week + "周没有记录或尝未输入数据！");
                return common.GetModuleFromJobjet(result);
            }
            return common.GetModuleFromJobjet(result);
        }

        //批量添加方法   
        [HttpPost]
        [ApiAuthorize]
        public JObject Batch_PersonnelHours([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year == 0 ? 0 : obj.year;//年
            int month = obj.month == 0 ? 0 : obj.month;//月
            int week = obj.week == 0 ? 0 : obj.week;//周
            List<Personnel_WorkingHours> workingHours = obj.workingHours == null ? null : obj.workingHours;
            int count = 0;
            if (workingHours.Count > 0 && year != 0 && month != 0 && week != 0)
            {
                var CreateDate = DateTime.Now;
                var Creator = auth.UserName;
                var weekDay = CreateDate.DayOfWeek.ToString();//得到今天是周几
                string batch = "";

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
                foreach (var date in workingHours)
                {
                    int Working = db.Personnel_WorkingHours.Count(c => c.Year == date.Year && c.Month == date.Month && c.Week == date.Week && c.UserNumber == date.UserNumber);
                    if (Working > 0)
                    {
                        if (batch == "")
                        {
                            batch = date.Year + "年" + date.Month + "月第" + date.Week + "周" + date.UserNumber;
                        }
                        else
                        {
                            batch = result + "," + date.Year + "年" + date.Month + "月第" + date.Week + "周" + date.UserNumber;
                        }
                    }
                }
                if (batch != "")
                {
                    result.Add("Result", false);
                    result.Add("Message", batch + "数据已经存在，不能再输入！");
                    return common.GetModuleFromJobjet(result);
                }
                foreach (var date in workingHours)
                {
                    date.CreateDate = CreateDate;
                    date.Creator = Creator;
                    date.Monday = Monday;
                    date.Sunday = Sunday;
                    date.Year = year;
                    date.Month = month;
                    date.Week = week;
                    db.Personnel_WorkingHours.Add(date);
                    count += db.SaveChanges();
                }
                if (count == workingHours.Count)
                {
                    result.Add("Result", true);
                    result.Add("Message", "保存成功！");
                    return common.GetModuleFromJobjet(result);
                }
                else
                {
                    result.Add("Result", false);
                    result.Add("Message", "保存失败！");
                    return common.GetModuleFromJobjet(result);
                }
            }
            result.Add("Result", false);
            result.Add("Message", "数据错误！");
            return common.GetModuleFromJobjet(result);
        }

        //修改数据
        [HttpPost]
        [ApiAuthorize]
        public JObject Modify_WorkingHours([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year == 0 ? 0 : obj.year;//年
            int month = obj.month == 0 ? 0 : obj.month;//月
            int week = obj.week == 0 ? 0 : obj.week;//周
            Personnel_WorkingHours hoursList = obj.hoursList == null ? null : obj.hoursList;
            if (year != 0 && month != 0 && week != 0 && hoursList != null)
            {
                hoursList.ModifyName = auth.UserName;
                hoursList.ModifyDate = DateTime.Now;
                db.Entry(hoursList).State = EntityState.Modified;//修改数据
                int count = db.SaveChanges();//保存数据库
                if (count > 0)
                {
                    result.Add("Result", true);
                    result.Add("Message", "修改成功！");
                    return common.GetModuleFromJobjet(result);
                }
                else
                {
                    result.Add("Result", false);
                    result.Add("Message", "修改失败！");
                    return common.GetModuleFromJobjet(result);
                }
            }
            result.Add("Result", false);
            result.Add("Message", "数据错误！");
            return common.GetModuleFromJobjet(result);
        }

        #endregion

        #region---班组工时明细表

        //班组工时明细表
        [HttpPost]
        [ApiAuthorize]
        public JObject TeamDetail([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            JArray dp_depar = new JArray();
            JObject dp_group = new JObject();
            JObject dp_group2 = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year == 0 ? 0 : obj.year;//年
            int month = obj.month == 0 ? 0 : obj.month;//月
            int week = obj.week == 0 ? 0 : obj.week;//周
            if (year == 0 || month == 0 || week == 0)
            {
                result.Add("Result", false);
                result.Add("Message", year == 0 ? "年份" : "" + month == null ? "月份" : "" + week == null ? "周" : "" + "未选择！");
                return common.GetModuleFromJobjet(result);
            }
            var Overtime1 = db.Personnel_Corporate_Calendar.Count(c => c.CalendarType == "001" && c.Calendar_TypeName == "工作日");
            var Overtime2 = db.Personnel_Corporate_Calendar.Count(c => c.CalendarType == "002" && c.Calendar_TypeName == "假日");
            var Overtime3 = db.Personnel_Corporate_Calendar.Count(c => c.CalendarType == "003" && c.Calendar_TypeName == "节日");
            var woringHours = db.Personnel_WorkingHours.Where(c => c.Year == year && c.Month == month && c.Week == week).ToList();
            int Factory_number = 0;
            double Factory_zhengBan_Work = 0;
            double Factory_work_Overtime1 = 0;
            double Factory_work_Overtime2 = 0;
            double Factory_work_Overtime3 = 0;
            if (woringHours.Count > 0)
            {
                var departmentList = woringHours.Select(c => c.Department).Distinct();
                foreach (var department in departmentList)
                {
                    var groupList = woringHours.Where(c => c.Department == department).Select(c => c.Group).Distinct();
                    foreach (var group in groupList)
                    {
                        int number = woringHours.Count(c => c.Department == department && c.Group == group);//人数
                        Factory_number = Factory_number + number;
                        double zhengBan_Work = woringHours.Where(c => c.Department == department && c.Group == group).Sum(c => c.ZhengBan_Work);//正班工时
                        Factory_zhengBan_Work = Factory_zhengBan_Work + zhengBan_Work;
                        double work_Overtime1 = woringHours.Where(c => c.Department == department && c.Group == group).Sum(c => c.Work_Overtime1);//加班1
                        Factory_work_Overtime1 = Factory_work_Overtime1 + work_Overtime1;
                        double work_Overtime2 = woringHours.Where(c => c.Department == department && c.Group == group).Sum(c => c.Work_Overtime2);//加班2
                        Factory_work_Overtime2 = Factory_work_Overtime2 + work_Overtime2;
                        double work_Overtime3 = woringHours.Where(c => c.Department == department && c.Group == group).Sum(c => c.Work_Overtime3);//加班1
                        Factory_work_Overtime3 = Factory_work_Overtime3 + work_Overtime3;
                        double work_Overtime_Average1 = work_Overtime1 / Overtime1 / number;//加班1日人均加班工时
                        double work_Overtime_Average2 = work_Overtime2 / Overtime2 / number;//加班2日人均加班工时
                        double work_Overtime_Average3 = work_Overtime3 / Overtime3 / number;//加班3日人均加班工时
                        double total_Overtime = work_Overtime1 + work_Overtime2 + work_Overtime3;//班组加班总值
                        dp_group.Add("Department", department);//部门
                        dp_group.Add("Group", group);//班组
                        dp_group.Add("Number", number);//人数
                        dp_group.Add("ZhengBan_Work", zhengBan_Work);//正班工时
                        dp_group.Add("work_Overtime1", work_Overtime1);//加班1
                        dp_group.Add("work_Overtime2", work_Overtime2);//加班2
                        dp_group.Add("work_Overtime3", work_Overtime3);//加班3
                        dp_group.Add("Work_Overtime_Average1", work_Overtime_Average1);//加班1日人均加班工时
                        dp_group.Add("Work_Overtime_Average2", work_Overtime_Average2);//加班2日人均加班工时
                        dp_group.Add("Work_Overtime_Average3", work_Overtime_Average3);//加班3日人均加班工时
                        dp_group.Add("Total_Overtime", total_Overtime);//班组加班总值
                        dp_depar.Add(dp_group);
                        dp_group = new JObject();
                    }
                }
                double Factory_Average1 = Factory_work_Overtime1 / Overtime1 / Factory_number;//全厂加班1日人均加班工时
                double Factory_Average2 = Factory_work_Overtime2 / Overtime2 / Factory_number;//全厂加班2日人均加班工时
                double Factory_Average3 = Factory_work_Overtime3 / Overtime3 / Factory_number;//全厂加班3日人均加班工时
                double Factory_total_Overtime = Factory_work_Overtime1 + Factory_work_Overtime2 + Factory_work_Overtime3;//全厂部门加班总值
                result.Add("Factory_number", Factory_number);//全厂总人数
                result.Add("Factory_zhengBan_Work", Factory_zhengBan_Work); //全厂正班工时
                result.Add("Factory_work_Overtime1", Factory_work_Overtime1);//全厂加班1
                result.Add("Factory_work_Overtime2", Factory_work_Overtime2); //全厂加班2
                result.Add("Factory_work_Overtime3", Factory_work_Overtime3); //全厂加班3
                result.Add("Factory_Average1", Factory_Average1);//全厂加班1日人均加班工时
                result.Add("Factory_Average2", Factory_Average2); //全厂加班2日人均加班工时
                result.Add("Factory_Average3", Factory_Average3);//全厂加班3日人均加班工时
                result.Add("Factory_total_Overtime", Factory_total_Overtime); //全厂部门加班总值
                result.Add("Message", dp_depar);//班组清单数据
                dp_group2 = new JObject();
            }
            return common.GetModuleFromJobjet(result);
        }

        #endregion

        #region---部门工时明细表
        [HttpPost]
        [ApiAuthorize]
        public JObject Department_Working([System.Web.Http.FromBody]JObject data)//部门工时报表首页
        {
            JObject result = new JObject();
            JObject dp_group = new JObject();
            JArray dp_group2 = new JArray();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year == 0 ? 0 : obj.year;//年
            int month = obj.month == 0 ? 0 : obj.month;//月
            int week = obj.week == 0 ? 0 : obj.week;//周
            if (year == 0 || month == 0 || week == 0)
            {
                result.Add("Result", false);
                result.Add("Message", year == 0 ? "年份" : "" + month == null ? "月份" : "" + week == null ? "周" : "" + "未选择！");
                return common.GetModuleFromJobjet(result);
            }
            var Overtime1 = db.Personnel_Corporate_Calendar.Count(c => c.CalendarType == "001" && c.Calendar_TypeName == "工作日");
            var Overtime2 = db.Personnel_Corporate_Calendar.Count(c => c.CalendarType == "002" && c.Calendar_TypeName == "假日");
            var Overtime3 = db.Personnel_Corporate_Calendar.Count(c => c.CalendarType == "003" && c.Calendar_TypeName == "节日");
            var woringHours = db.Personnel_WorkingHours.Where(c => c.Year == year && c.Month == month && c.Week == week).ToList();
            int Factory_number = 0;
            double Factory_zhengBan_Work = 0;
            double Factory_work_Overtime1 = 0;
            double Factory_work_Overtime2 = 0;
            double Factory_work_Overtime3 = 0;
            if (woringHours.Count > 0)
            {
                var departmentList = woringHours.Select(c => c.Department).Distinct();
                foreach (var department in departmentList)
                {
                    int number = woringHours.Count(c => c.Department == department);//人数
                    Factory_number = Factory_number + number;
                    double zhengBan_Work = woringHours.Where(c => c.Department == department).Sum(c => c.ZhengBan_Work);//正班工时
                    Factory_zhengBan_Work = Factory_zhengBan_Work + zhengBan_Work;
                    double work_Overtime1 = woringHours.Where(c => c.Department == department).Sum(c => c.Work_Overtime1);//加班1
                    Factory_work_Overtime1 = Factory_work_Overtime1 + work_Overtime1;
                    double work_Overtime2 = woringHours.Where(c => c.Department == department).Sum(c => c.Work_Overtime2);//加班2
                    Factory_work_Overtime2 = Factory_work_Overtime2 + work_Overtime2;
                    double work_Overtime3 = woringHours.Where(c => c.Department == department).Sum(c => c.Work_Overtime3);//加班1
                    Factory_work_Overtime3 = Factory_work_Overtime3 + work_Overtime3;
                    double work_Overtime_Average1 = work_Overtime1 / Overtime1 / number;//加班1日人均加班工时
                    double work_Overtime_Average2 = work_Overtime2 / Overtime2 / number;//加班2日人均加班工时
                    double work_Overtime_Average3 = work_Overtime3 / Overtime3 / number;//加班3日人均加班工时
                    double total_Overtime = work_Overtime1 + work_Overtime2 + work_Overtime3;//部门加班总值
                    double capita_Overtime = (total_Overtime / number) / (Overtime1 + Overtime2);//人均日加班工时
                    double total_WorkingHours = total_Overtime + zhengBan_Work;//总工时
                    dp_group.Add("Department", department);//部门
                    dp_group.Add("Number", number);//人数
                    dp_group.Add("ZhengBan_Work", zhengBan_Work);//正班工时
                    dp_group.Add("Work_Overtime1", work_Overtime1);//加班1
                    dp_group.Add("Work_Overtime2", work_Overtime2);//加班2
                    dp_group.Add("Work_Overtime3", work_Overtime3);//加班3
                    dp_group.Add("Work_Overtime_Average1", work_Overtime_Average1);//加班1日人均加班工时
                    dp_group.Add("Work_Overtime_Average2", work_Overtime_Average2);//加班2日人均加班工时
                    dp_group.Add("Work_Overtime_Average3", work_Overtime_Average3);//加班3日人均加班工时
                    dp_group.Add("Total_Overtime", total_Overtime);//班组加班总值
                    dp_group.Add("Capita_Overtime", capita_Overtime);//人均日加班工时
                    dp_group.Add("Total_WorkingHours", total_WorkingHours);//总工时
                    dp_group2.Add(dp_group);
                    dp_group = new JObject();
                }
                double Factory_Average1 = Factory_work_Overtime1 / Overtime1 / Factory_number;//全厂加班1日人均加班工时
                double Factory_Average2 = Factory_work_Overtime2 / Overtime2 / Factory_number;//全厂加班2日人均加班工时
                double Factory_Average3 = Factory_work_Overtime3 / Overtime3 / Factory_number;//全厂加班3日人均加班工时
                double Factory_total_Overtime = Factory_work_Overtime1 + Factory_work_Overtime2 + Factory_work_Overtime3;//全厂部门加班总值
                double Factory_capita_Overtime = (Factory_total_Overtime / Factory_number) / (Overtime1 + Overtime2);//全厂人均日加班工时
                double Factory_total_WorkingHours = Factory_total_Overtime + Factory_zhengBan_Work;//全厂总工时

                result.Add("Factory_number", Factory_number);//全厂总人数
                result.Add("Factory_zhengBan_Work", Factory_zhengBan_Work); //全厂正班工时
                result.Add("Factory_work_Overtime1", Factory_work_Overtime1);//全厂加班1
                result.Add("Factory_work_Overtime2", Factory_work_Overtime2); //全厂加班2
                result.Add("Factory_work_Overtime3", Factory_work_Overtime3); //全厂加班3
                result.Add("Factory_Average1", Factory_Average1);//全厂加班1日人均加班工时
                result.Add("Factory_Average2", Factory_Average2); //全厂加班2日人均加班工时
                result.Add("Factory_Average3", Factory_Average3);//全厂加班3日人均加班工时
                result.Add("Factory_total_Overtime", Factory_total_Overtime); //全厂部门加班总值
                result.Add("Factory_capita_Overtime", Factory_capita_Overtime); //全厂人均日加班工时
                result.Add("Factory_total_WorkingHours", Factory_total_WorkingHours); //全厂总工时
                result.Add("Message", dp_group2);
                dp_group2 = new JArray();
            }
            return common.GetModuleFromJobjet(result);
        }
        #endregion

        #region----找最大的年月周数据(班组的对比表和部门的对比表)

        [HttpPost]
        [ApiAuthorize]
        //找最大的年月周数据
        public JObject DefaualInfo()
        {
            JObject lastInfo = new JObject();
            JObject countdown_SecondInfo = new JObject();
            JObject result = new JObject();
            //查找id倒数第二的12个数据组
            var countdown_Second = new List<Personnel_WorkingHours>();
            //倒数第二数据组的年月周
            string countdown_Second_timeTostring = null;
            //所有年月周分组
            var totalTime = db.Personnel_WorkingHours.GroupBy(c => new { c.Year, c.Month, c.Week }).Select(g => g.Key).ToList();
            int count = totalTime.Count();
            //判断是否小于2
            if (count >= 2)
            {
                var countdown_Second_time = totalTime[count - 2];
                countdown_Second = db.Personnel_WorkingHours.Where(c => c.Year == countdown_Second_time.Year && c.Month == countdown_Second_time.Month && c.Week == countdown_Second_time.Week).ToList();
                countdown_Second_timeTostring = "year:" + countdown_Second_time.Year + ",month:" + countdown_Second_time.Month + ",week:" + countdown_Second_time.Week;
            }
            else
            {
                countdown_Second = null;
            }
            //最后数据组的年月周
            var last_time = db.Personnel_WorkingHours.OrderByDescending(c => c.Id).Select(c => new { c.Year, c.Month, c.Week }).FirstOrDefault();

            //查找ID 最后的数据组 
            var last = db.Personnel_WorkingHours.Where(c => c.Year == last_time.Year && c.Month == last_time.Month && c.Week == last_time.Week).ToList();

            //转为JToken
            var lastJson = JsonConvert.SerializeObject(last);
            var lastJtoken = JsonConvert.DeserializeObject<JToken>(lastJson);
            var last_timeTostring = string.Join(",", last_time);
            //转为JToken
            var countdown_SecondJson = JsonConvert.SerializeObject(countdown_Second);
            var countdown_SencondJtoken = JsonConvert.DeserializeObject<JToken>(countdown_SecondJson);

            //最后数据组的jobject 添加
            lastInfo.Add("Time", "year:" + last_time.Year + ",month:" + last_time.Month + ",week:" + last_time.Week); ;
            lastInfo.Add("Date", lastJtoken);
            //倒数第二数据组jobject添加
            countdown_SecondInfo.Add("Time", countdown_Second_timeTostring);
            countdown_SecondInfo.Add("Date", countdown_SencondJtoken);

            result.Add("Last", lastInfo);
            result.Add("Countdown_Second", countdown_SecondInfo);
            if (totalTime.Count != 0)
            {
                return common.GetModuleFromJobjet(result);
            }
            else
            {
                return common.GetModuleFromJobjet(result);
            }
        }
        #endregion

        #region---加班排行榜方法

        //人员加班排行榜
        [HttpPost]
        [ApiAuthorize]
        public JObject Personnel_OvertimeList([System.Web.Http.FromBody]JObject data)//加班排行榜页面
        {
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year == 0 ? 0 : obj.year;//年
            int month = obj.month == 0 ? 0 : obj.month;//月
            int week = obj.week == 0 ? 0 : obj.week;//周
            if (year == 0 || month == 0 || week == 0)
            {
                result.Add("Result", false);
                result.Add("Message", year == 0 ? "年份" : "" + month == null ? "月份" : "" + week == null ? "周" : "" + "未选择！");
                return common.GetModuleFromJobjet(result);
            }
            var workList = db.Personnel_WorkingHours.Where(c => c.Year == year && c.Month == month && c.Week == week).ToList();
            if (workList.Count > 0)
            {

            }





            return common.GetModuleFromJobjet(result);
        }

        //班组加班1日人均加班工时排行榜
        [HttpPost]
        [ApiAuthorize]
        public JObject TeamCapita_OvertimeList([System.Web.Http.FromBody]JObject data)//加班排行榜页面
        {
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year == 0 ? 0 : obj.year;//年
            int month = obj.month == 0 ? 0 : obj.month;//月
            int week = obj.week == 0 ? 0 : obj.week;//周







            return common.GetModuleFromJobjet(result);
        }

        //班组加班2日人均加班工时排行榜
        [HttpPost]
        [ApiAuthorize]
        public JObject TeamCapita2_OvertimeList([System.Web.Http.FromBody]JObject data)//加班排行榜页面
        {
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year == 0 ? 0 : obj.year;//年
            int month = obj.month == 0 ? 0 : obj.month;//月
            int week = obj.week == 0 ? 0 : obj.week;//周







            return common.GetModuleFromJobjet(result);
        }


        //班组加班总值平均值排行榜
        [HttpPost]
        [ApiAuthorize]
        public JObject TotalAverage_OvertimeList([System.Web.Http.FromBody]JObject data)//加班排行榜页面
        {
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year == 0 ? 0 : obj.year;//年
            int month = obj.month == 0 ? 0 : obj.month;//月
            int week = obj.week == 0 ? 0 : obj.week;//周







            return common.GetModuleFromJobjet(result);
        }


        #endregion

        #region---月工时差异表方法

        #endregion

        #region---月报/半月报明细表/对比表方法

        #endregion


    }
}