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
using System.Globalization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JianHeMES.AuthAttributes;

namespace JianHeMES.Controllers
{
    public class Personnel_of_ContrastController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        #region----------------人工成本周记录对比
        public ActionResult Contrast()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_of_Contrast", act = "Contrast" });
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ContrastGetData(int? Year, int? Month, int? Week)
        {
            if (Year == null || Month == null || Week == null)
            {
                return Content(Year == null ? "年份" : "" + Month == null ? "月份" : "" + Week == null ? "周" : "" + "未选择！");
            }
            var datalist = await db.Personnel_of_Contrast.Where(c => c.Year == Year && c.Month == Month && c.Week == Week).ToListAsync();
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


        #region----------------人工成本月记录对比
        public ActionResult ContrastMonth()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ContrastMonthGetData(int? Year, int? Month)
        {
            if (Year == null || Month == null)
            {
                return Content(Year == null ? "年份" : "" + Month == null ? "月份" : "" + "未选择！");
            }
            var datalist = await db.Personnel_of_Contrast.Where(c => c.Year == Year && c.Month == Month).ToListAsync();
            List<int> weekNumList = datalist.Select(c => c.Week).Distinct().ToList();
            JObject data = new JObject();

            #region----------输出几周数据
            foreach (var i in weekNumList)
            {
                var dataWeekList = datalist.Where(c => c.Week == i).ToList();
                data.Add(i.ToString(), JsonConvert.SerializeObject(dataWeekList));
                //周总计
                JObject temp = new JObject();
                temp.Add("Number", dataWeekList.Sum(c => c.Number));
                temp.Add("Zhang_WorkingHour", dataWeekList.Sum(c => c.Zhang_WorkingHour));
                temp.Add("Zhang_Pay", dataWeekList.Sum(c => c.Zhang_Pay));
                temp.Add("Total_Staff", dataWeekList.Sum(c => c.Total_Staff));
                temp.Add("Overtime_Total", dataWeekList.Sum(c => c.Overtime_Total));
                temp.Add("Pay_Total", dataWeekList.Sum(c => c.Pay_Total));
                temp.Add("Daily_Pay", dataWeekList.Sum(c => c.Daily_Pay));
                data.Add("weektotal" + i.ToString(), temp);
            }
            #endregion

            #region----------计算月度合计
            JObject total = new JObject();
            var departmentList = datalist.Select(c => c.Department).Distinct().ToList();
            foreach (var d in departmentList)
            {
                JObject temp = new JObject();
                temp.Add("Number", datalist.Where(c => c.Department == d).Sum(c => c.Number));
                temp.Add("Zhang_WorkingHour", datalist.Where(c => c.Department == d).Sum(c => c.Zhang_WorkingHour));
                temp.Add("Zhang_Pay", datalist.Where(c => c.Department == d).Sum(c => c.Zhang_Pay));
                temp.Add("Total_Staff", datalist.Where(c => c.Department == d).Sum(c => c.Total_Staff));
                temp.Add("Overtime_Total", datalist.Where(c => c.Department == d).Sum(c => c.Overtime_Total));
                temp.Add("Pay_Total", datalist.Where(c => c.Department == d).Sum(c => c.Pay_Total));
                temp.Add("Daily_Pay", datalist.Where(c => c.Department == d).Sum(c => c.Daily_Pay));
                total.Add(d, temp);
            }
            data.Add("month_total", total);
            JObject te = new JObject();
            te.Add("Number", datalist.Sum(c => c.Number));
            te.Add("Zhang_WorkingHour", datalist.Sum(c => c.Zhang_WorkingHour));
            te.Add("Zhang_Pay", datalist.Sum(c => c.Zhang_Pay));
            te.Add("Total_Staff", datalist.Sum(c => c.Total_Staff));
            te.Add("Overtime_Total", datalist.Sum(c => c.Overtime_Total));
            te.Add("Pay_Total", datalist.Sum(c => c.Pay_Total));
            te.Add("Daily_Pay", datalist.Sum(c => c.Daily_Pay));
            data.Add("month_total_total", te);

            #endregion

            #region----------计算月度平均值
            JObject average = new JObject();
            foreach (var d in departmentList)
            {
                JObject temp = new JObject();
                temp.Add("Number", datalist.Where(c => c.Department == d).Average(c => c.Number));
                temp.Add("Zhang_WorkingHour", datalist.Where(c => c.Department == d).Average(c => c.Zhang_WorkingHour));
                temp.Add("Zhang_Pay", datalist.Where(c => c.Department == d).Average(c => c.Zhang_Pay));
                temp.Add("Total_Staff", datalist.Where(c => c.Department == d).Average(c => c.Total_Staff));
                temp.Add("Overtime_Total", datalist.Where(c => c.Department == d).Average(c => c.Overtime_Total));
                temp.Add("Pay_Total", datalist.Where(c => c.Department == d).Average(c => c.Pay_Total));
                temp.Add("Daily_Pay", datalist.Where(c => c.Department == d).Average(c => c.Daily_Pay));
                average.Add(d, temp);
            }
            data.Add("month_average", average);
            JObject tem = new JObject();
            tem.Add("Number", datalist.Average(c => c.Number));
            tem.Add("Zhang_WorkingHour", datalist.Average(c => c.Zhang_WorkingHour));
            tem.Add("Zhang_Pay", datalist.Average(c => c.Zhang_Pay));
            tem.Add("Total_Staff", datalist.Average(c => c.Total_Staff));
            tem.Add("Overtime_Total", datalist.Average(c => c.Overtime_Total));
            tem.Add("Pay_Total", datalist.Average(c => c.Pay_Total));
            tem.Add("Daily_Pay", datalist.Average(c => c.Daily_Pay));
            data.Add("month_average_averge", tem);
            #endregion

            return Content(JsonConvert.SerializeObject(data));
        }
        #endregion


        #region----------------人工成本周记录输入
        public ActionResult ContrastInput()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_of_Contrast", act = "ContrastInput" });
            }
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> ContrastInput(List<Personnel_of_Contrast> WeekDataList)
        {
            if (WeekDataList != null)
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
                foreach (var data in WeekDataList)
                {
                    int isExist = db.Personnel_of_Contrast.Count(c => c.Year == data.Year && c.Month == data.Month && c.Week == data.Week && c.Department == data.Department);
                    if (isExist > 0)
                    {
                        if (result == "")
                        {
                            result = data.Year + "年" + data.Month + "月第" + data.Week + "周" + data.Department;
                        }
                        else
                        {
                            result = result + "," + data.Year + "年" + data.Month + "月第" + data.Week + "周" + data.Department;
                        }
                    }
                }
                if (result != "") return Content(result + "数据已经存在，不能再输入！");
                foreach (var data in WeekDataList)
                {
                    data.CreateDate = CreateDate;
                    data.Creator = Creator;
                    data.Monday = Monday;
                    data.Sunday = Sunday;
                    db.Personnel_of_Contrast.Add(data);
                    await db.SaveChangesAsync();
                }
                return Content("保存成功");
            }
            return Content("保存失败");
        }
        #endregion


        #region----------------人工成本周记录修改
        //修改单条记录
        [HttpPost]
        public ActionResult ContrastEditRecord(int id)
        {
            return Content("");
        }

        //批量删除记录
        [HttpPost]
        public async Task<ActionResult> ContrastDelectList(int[] delete_id_list)
        {
            try
            {
                List<int> del_list = new List<int>(delete_id_list);
                List<Personnel_of_Contrast> personnel_of_Contrast_list = new List<Personnel_of_Contrast>();
                foreach (var item in delete_id_list)
                {
                    personnel_of_Contrast_list.Add(await db.Personnel_of_Contrast.FindAsync(item));
                }
                db.Personnel_of_Contrast.RemoveRange(personnel_of_Contrast_list);
                await db.SaveChangesAsync();
                return Content("删除成功！");
            }
            catch
            {
                return Content("删除失败！");
            }
        }


        public async Task<ActionResult> ContrastEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_of_Contrast personnel_of_Contrast = await db.Personnel_of_Contrast.FindAsync(id);
            if (personnel_of_Contrast == null)
            {
                return HttpNotFound();
            }
            return View(personnel_of_Contrast);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContrastEdit([Bind(Include = "Id,Department,Number,Zhang_WorkingHour,Zhang_Pay,Total_Staff,Overtime_Total,Pay_Total,Daily_Pay,Date,CreateDate,Creator,Year,Month,Week,Week_number,Monday,Sunday")] Personnel_of_Contrast personnel_of_Contrast)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personnel_of_Contrast).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Contrast");
            }
            return View(personnel_of_Contrast);
        }
        #endregion


        [HttpPost]
        public ActionResult DefaualInfo()
        {
            JObject lastInfo = new JObject();
            JObject countdown_SecondInfo = new JObject();
            JObject info = new JObject();
            //查找id倒数第二的12个数据组
            var countdown_Second = new List<Personnel_of_Contrast>();
            //倒数第二数据组的年月周
            string countdown_Second_timeTostring = null;
            //所有年月周分组
            var totalTime = db.Personnel_of_Contrast.GroupBy(c => new { c.Year, c.Month, c.Week }).Select(g => g.Key).ToList();
            int count = totalTime.Count();
            //判断是否小于2
            if (count >= 2)
            {
                var countdown_Second_time = totalTime[count - 2];
                countdown_Second = db.Personnel_of_Contrast.Where(c => c.Year == countdown_Second_time.Year && c.Month == countdown_Second_time.Month && c.Week == countdown_Second_time.Week).ToList();
                countdown_Second_timeTostring = "year:" + countdown_Second_time.Year + ",month:" + countdown_Second_time.Month + ",week:" + countdown_Second_time.Week;
            }
            else
            {
                countdown_Second = null;
            }
            //最后数据组的年月周
            var last_time = db.Personnel_of_Contrast.OrderByDescending(c => c.Id).Select(c => new { c.Year, c.Month, c.Week }).FirstOrDefault();

            //查找ID 最后的数据组 
            var last = db.Personnel_of_Contrast.Where(c => c.Year == last_time.Year && c.Month == last_time.Month && c.Week == last_time.Week).ToList();

            //转为JToken
            var lastJson = JsonConvert.SerializeObject(last);
            var lastJtoken = JsonConvert.DeserializeObject<JToken>(lastJson);
            var last_timeTostring = string.Join(",", last_time);
            //转为JToken
            var countdown_SecondJson = JsonConvert.SerializeObject(countdown_Second);
            var countdown_SencondJtoken = JsonConvert.DeserializeObject<JToken>(countdown_SecondJson);

            //最后数据组的jobject 添加
            lastInfo.Add("Time", "year:" + last_time.Year + ",month:" + last_time.Month + ",week:" + last_time.Week); ;
            lastInfo.Add("Data", lastJtoken);
            //倒数第二数据组jobject添加
            countdown_SecondInfo.Add("Time", countdown_Second_timeTostring);
            countdown_SecondInfo.Add("Data", countdown_SencondJtoken);

            info.Add("last", lastInfo);
            info.Add("countdown_Second", countdown_SecondInfo);
            if (totalTime.Count != 0)
            {
                return Content(JsonConvert.SerializeObject(info));
            }
            else
                return Content("数据查找失败");
        }
        #region----------------其他方法
        // GET: Personnel_of_Contrast
        public async Task<ActionResult> Index()
        {
            return View(await db.Personnel_of_Contrast.ToListAsync());
        }

        // GET: Personnel_of_Contrast/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_of_Contrast personnel_of_Contrast = await db.Personnel_of_Contrast.FindAsync(id);
            if (personnel_of_Contrast == null)
            {
                return HttpNotFound();
            }
            return View(personnel_of_Contrast);
        }

        // GET: Personnel_of_Contrast/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Personnel_of_Contrast/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Department,Number,Zhang_WorkingHour,Zhang_Pay,Total_Staff,Overtime_Total,Pay_Total,Daily_Pay,Date,CreateDate,Creator,Year,Month,Week,Week_number,Monday,Sunday")] Personnel_of_Contrast personnel_of_Contrast)
        {
            if (ModelState.IsValid)
            {
                db.Personnel_of_Contrast.Add(personnel_of_Contrast);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(personnel_of_Contrast);
        }

        // GET: Personnel_of_Contrast/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_of_Contrast personnel_of_Contrast = await db.Personnel_of_Contrast.FindAsync(id);
            if (personnel_of_Contrast == null)
            {
                return HttpNotFound();
            }
            return View(personnel_of_Contrast);
        }

        // POST: Personnel_of_Contrast/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Department,Number,Zhang_WorkingHour,Zhang_Pay,Total_Staff,Overtime_Total,Pay_Total,Daily_Pay,Date,CreateDate,Creator,Year,Month,Week,Week_number,Monday,Sunday")] Personnel_of_Contrast personnel_of_Contrast)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personnel_of_Contrast).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(personnel_of_Contrast);
        }

        // GET: Personnel_of_Contrast/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_of_Contrast personnel_of_Contrast = await db.Personnel_of_Contrast.FindAsync(id);
            if (personnel_of_Contrast == null)
            {
                return HttpNotFound();
            }
            return View(personnel_of_Contrast);
        }

        // POST: Personnel_of_Contrast/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Personnel_of_Contrast personnel_of_Contrast = await db.Personnel_of_Contrast.FindAsync(id);
            db.Personnel_of_Contrast.Remove(personnel_of_Contrast);
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

    //Api接口部分

    public class Personnel_of_Contrast_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController common = new CommonController();

        #region------找当月最大的两周
        [HttpPost]
        [ApiAuthorize]
        public JObject DefaualInfo()
        {
            JObject lastInfo = new JObject();
            JObject countdown_SecondInfo = new JObject();
            JObject result = new JObject();
            //查找id倒数第二的12个数据组
            var countdown_Second = new List<Personnel_of_Contrast>();
            //倒数第二数据组的年月周
            string countdown_Second_timeTostring = null;
            //所有年月周分组
            var totalTime = db.Personnel_of_Contrast.GroupBy(c => new { c.Year, c.Month, c.Week }).Select(g => g.Key).ToList();
            int count = totalTime.Count();
            //判断是否小于2
            if (count >= 2)
            {
                var countdown_Second_time = totalTime[count - 2];
                countdown_Second = db.Personnel_of_Contrast.Where(c => c.Year == countdown_Second_time.Year && c.Month == countdown_Second_time.Month && c.Week == countdown_Second_time.Week).ToList();
                countdown_Second_timeTostring = "year:" + countdown_Second_time.Year + ",month:" + countdown_Second_time.Month + ",week:" + countdown_Second_time.Week;
            }
            else
            {
                countdown_Second = null;
            }
            //最后数据组的年月周
            var last_time = db.Personnel_of_Contrast.OrderByDescending(c => c.Id).Select(c => new { c.Year, c.Month, c.Week }).FirstOrDefault();

            //查找ID 最后的数据组 
            var last = db.Personnel_of_Contrast.Where(c => c.Year == last_time.Year && c.Month == last_time.Month && c.Week == last_time.Week).ToList();

            //转为JToken
            var lastJson = JsonConvert.SerializeObject(last);
            var lastJtoken = JsonConvert.DeserializeObject<JToken>(lastJson);
            var last_timeTostring = string.Join(",", last_time);
            //转为JToken
            var countdown_SecondJson = JsonConvert.SerializeObject(countdown_Second);
            var countdown_SencondJtoken = JsonConvert.DeserializeObject<JToken>(countdown_SecondJson);

            //最后数据组的jobject 添加
            lastInfo.Add("Time", "year:" + last_time.Year + ",month:" + last_time.Month + ",week:" + last_time.Week); ;
            lastInfo.Add("Data", lastJtoken);
            //倒数第二数据组jobject添加
            countdown_SecondInfo.Add("Time", countdown_Second_timeTostring);
            countdown_SecondInfo.Add("Data", countdown_SencondJtoken);

            result.Add("last", lastInfo);
            result.Add("countdown_Second", countdown_SecondInfo);
            if (totalTime.Count != 0)
            {
                return common.GetModuleFromJobjet(result);
            }
            else
                return common.GetModuleFromJobjet(result);
        }
        #endregion

        #region----------------人工成本周记录对比
        [HttpPost]
        [ApiAuthorize]
        public JObject ContrastGetData([System.Web.Http.FromBody]JObject data)
        {
            JArray result = new JArray();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int? Year = obj.Year;//年
            int? Month = obj.Month;//月
            int? Week = obj.Week;//周
            if (Year == null || Month == null || Week == null)
            {
                return common.GetModuleFromJarray(result, false, Year == null ? "年份" : "" + Month == null ? "月份" : "" + Week == null ? "周" : "" + "未选择！");
            }
            var datalist = db.Personnel_of_Contrast.Where(c => c.Year == Year && c.Month == Month && c.Week == Week).ToList();
            result.Add(datalist);
            if (datalist != null)
            {
                return common.GetModuleFromJarray(result, true, "查询成功");
            }
            else
            {
                return common.GetModuleFromJarray(result, false, Year + "年" + Month + "月第" + Week + "周没有记录或尝未输入数据！");
            }
        }
        #endregion

        #region----------------人工成本周记录输入

        [HttpPost]
        [ApiAuthorize]
        public JObject ContrastInput([System.Web.Http.FromBody]JObject data)
        {
            JArray result = new JArray();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            List<Personnel_of_Contrast> WeekDataList = obj.WeekDataList;
            if (WeekDataList.Count > 0)
            {
                var CreateDate = DateTime.Now;
                var Creator = auth.UserName;
                var weekDay = CreateDate.DayOfWeek.ToString();//得到今天是周几
                string rep = "";
                int count = 0;
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
                foreach (var data1 in WeekDataList)
                {
                    int isExist = db.Personnel_of_Contrast.Count(c => c.Year == data1.Year && c.Month == data1.Month && c.Week == data1.Week && c.Department == data1.Department);
                    if (isExist > 0)
                    {
                        if (rep == "")
                        {
                            rep = data1.Year + "年" + data1.Month + "月第" + data1.Week + "周" + data1.Department;
                            result.Add(rep);
                        }
                        else
                        {
                            rep = result + "," + data1.Year + "年" + data1.Month + "月第" + data1.Week + "周" + data1.Department;
                            result.Add(rep);
                        }
                    }
                }
                if (result.Count > 0) return common.GetModuleFromJarray(result, false, "数据已经存在，不能再输入！");
                foreach (var data1 in WeekDataList)
                {
                    data1.CreateDate = CreateDate;
                    data1.Creator = Creator;
                    data1.Monday = Monday;
                    data1.Sunday = Sunday;
                    db.Personnel_of_Contrast.Add(data1);
                    count += db.SaveChanges();
                }
                if (count > 0)
                {
                    return common.GetModuleFromJarray(result, true, "保存成功");
                }
                else
                {
                    return common.GetModuleFromJarray(result, false, "保存失败");
                }
            }
            return common.GetModuleFromJarray(result, false, "保存失败");
        }
        #endregion

        #region----------------人工成本周记录批量删除

        [HttpPost]
        [ApiAuthorize]
        public JObject ContrastDelectList([System.Web.Http.FromBody]JObject data)
        {
            JArray result = new JArray();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int[] delete_id_list = obj.delete_id_list;
            int count = 0;
            try
            {
                List<int> del_list = new List<int>(delete_id_list);
                List<Personnel_of_Contrast> personnel_of_Contrast_list = new List<Personnel_of_Contrast>();
                foreach (var item in delete_id_list)
                {
                    personnel_of_Contrast_list.Add(db.Personnel_of_Contrast.Find(item));
                }
                db.Personnel_of_Contrast.RemoveRange(personnel_of_Contrast_list);
                count = db.SaveChanges();
                if (count > 0)
                {
                    return common.GetModuleFromJarray(result, true, "删除成功");
                }
                else
                {
                    return common.GetModuleFromJarray(result, false, "删除失败");
                }
            }
            catch
            {
                return common.GetModuleFromJarray(result, false, "删除失败");
            }
        }

        #endregion

        #region----------------人工成本月记录对比

        [HttpPost]
        [ApiAuthorize]
        public JObject ContrastMonthGetData([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int? Year = obj.Year;//年
            int? Month = obj.Month;//月
            if (Year == null || Month == null)
            {
                return common.GetModuleFromJobjet(result, false, Year == null ? "年份" : "" + Month == null ? "月份" : "" + "未选择！");
            }
            var datalist = db.Personnel_of_Contrast.Where(c => c.Year == Year && c.Month == Month).ToList();
            List<int> weekNumList = datalist.Select(c => c.Week).Distinct().ToList();

            #region----------输出几周数据
            foreach (var i in weekNumList)
            {
                var dataWeekList = datalist.Where(c => c.Week == i).ToList();
                result.Add(i.ToString(), JsonConvert.SerializeObject(dataWeekList));
                //周总计
                JObject temp = new JObject();
                temp.Add("Number", dataWeekList.Sum(c => c.Number));
                temp.Add("Zhang_WorkingHour", dataWeekList.Sum(c => c.Zhang_WorkingHour));
                temp.Add("Zhang_Pay", dataWeekList.Sum(c => c.Zhang_Pay));
                temp.Add("Total_Staff", dataWeekList.Sum(c => c.Total_Staff));
                temp.Add("Overtime_Total", dataWeekList.Sum(c => c.Overtime_Total));
                temp.Add("Pay_Total", dataWeekList.Sum(c => c.Pay_Total));
                temp.Add("Daily_Pay", dataWeekList.Sum(c => c.Daily_Pay));
                result.Add("weektotal" + i.ToString(), temp);
            }
            #endregion

            #region----------计算月度合计
            JObject total = new JObject();
            var departmentList = datalist.Select(c => c.Department).Distinct().ToList();
            foreach (var d in departmentList)
            {
                JObject temp = new JObject();
                temp.Add("Number", datalist.Where(c => c.Department == d).Sum(c => c.Number));
                temp.Add("Zhang_WorkingHour", datalist.Where(c => c.Department == d).Sum(c => c.Zhang_WorkingHour));
                temp.Add("Zhang_Pay", datalist.Where(c => c.Department == d).Sum(c => c.Zhang_Pay));
                temp.Add("Total_Staff", datalist.Where(c => c.Department == d).Sum(c => c.Total_Staff));
                temp.Add("Overtime_Total", datalist.Where(c => c.Department == d).Sum(c => c.Overtime_Total));
                temp.Add("Pay_Total", datalist.Where(c => c.Department == d).Sum(c => c.Pay_Total));
                temp.Add("Daily_Pay", datalist.Where(c => c.Department == d).Sum(c => c.Daily_Pay));
                total.Add(d, temp);
            }
            result.Add("month_total", total);
            JObject te = new JObject();
            te.Add("Number", datalist.Sum(c => c.Number));
            te.Add("Zhang_WorkingHour", datalist.Sum(c => c.Zhang_WorkingHour));
            te.Add("Zhang_Pay", datalist.Sum(c => c.Zhang_Pay));
            te.Add("Total_Staff", datalist.Sum(c => c.Total_Staff));
            te.Add("Overtime_Total", datalist.Sum(c => c.Overtime_Total));
            te.Add("Pay_Total", datalist.Sum(c => c.Pay_Total));
            te.Add("Daily_Pay", datalist.Sum(c => c.Daily_Pay));
            result.Add("month_total_total", te);

            #endregion

            #region----------计算月度平均值
            JObject average = new JObject();
            foreach (var d in departmentList)
            {
                JObject temp = new JObject();
                temp.Add("Number", datalist.Where(c => c.Department == d).Average(c => c.Number));
                temp.Add("Zhang_WorkingHour", datalist.Where(c => c.Department == d).Average(c => c.Zhang_WorkingHour));
                temp.Add("Zhang_Pay", datalist.Where(c => c.Department == d).Average(c => c.Zhang_Pay));
                temp.Add("Total_Staff", datalist.Where(c => c.Department == d).Average(c => c.Total_Staff));
                temp.Add("Overtime_Total", datalist.Where(c => c.Department == d).Average(c => c.Overtime_Total));
                temp.Add("Pay_Total", datalist.Where(c => c.Department == d).Average(c => c.Pay_Total));
                temp.Add("Daily_Pay", datalist.Where(c => c.Department == d).Average(c => c.Daily_Pay));
                average.Add(d, temp);
            }
            result.Add("month_average", average);
            JObject tem = new JObject();
            tem.Add("Number", datalist.Average(c => c.Number));
            tem.Add("Zhang_WorkingHour", datalist.Average(c => c.Zhang_WorkingHour));
            tem.Add("Zhang_Pay", datalist.Average(c => c.Zhang_Pay));
            tem.Add("Total_Staff", datalist.Average(c => c.Total_Staff));
            tem.Add("Overtime_Total", datalist.Average(c => c.Overtime_Total));
            tem.Add("Pay_Total", datalist.Average(c => c.Pay_Total));
            tem.Add("Daily_Pay", datalist.Average(c => c.Daily_Pay));
            result.Add("month_average_averge", tem);
            #endregion

            return common.GetModuleFromJobjet(result, true, "查询成功");
        }
        #endregion


    }







}
