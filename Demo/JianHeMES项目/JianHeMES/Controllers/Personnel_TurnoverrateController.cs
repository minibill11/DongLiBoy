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
using Newtonsoft.Json.Linq;
using iTextSharp.text.pdf;
using Org.BouncyCastle.Utilities;
using JianHeMES.AuthAttributes;

namespace JianHeMES.Controllers
{
    public class Personnel_TurnoverrateController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        CommonController date = new CommonController();

        #region-----------流失率查看
        public ActionResult Turnoverrate()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Turnoverrate", act = "Turnoverrate" });
            }
            return View();
        }

        //按月查看部门流失率情况
        [HttpPost]
        public async Task<ActionResult> Turnoverrate(int? Year, int? Month)
        {
            if (Year == null || Month == null)
            {
                return Content(Year == null ? "年份" : "" + Month == null ? "月份" : "" + "未选择！");
            }
            //var datalist = await db.Personnel_Turnoverrate.Where(c => c.Year == Year && c.Month == Month).ToListAsync();
            JObject Turnoverrate_Table = new JObject();
            JObject Department_record = new JObject();
            var Year_Month_Allrecord = await db.Personnel_daily.Where(c => c.Date.Value.Year == Year && c.Date.Value.Month == Month).ToListAsync();
            var Department_list = Year_Month_Allrecord.Select(c => c.Department).Distinct().ToList();
            if (Year_Month_Allrecord.Count() == 0)
            {
                return Content("没有记录！");
            }
            var departmentlist = new List<Personnel_Architecture>();
            DateTime exdate = new DateTime(Year.Value, Month.Value, 28, 0, 0, 0);
            departmentlist = date.CompanyDatetime(exdate);
            List<string> dp_list = new List<string>();
            List<string> others = new List<string>();
            foreach (var s in departmentlist)
            {
                if (Department_list.Contains(s.Department))
                {
                    dp_list.Add(s.Department);
                }
                else
                {
                    others.Add(s.Department);
                }
            }
            dp_list.AddRange(others);
            decimal average_sum = 0;
            decimal totalbegin = 0;
            decimal totalend = 0;
            int departure_month_sum = 0;
            int i = 0;
            foreach (var department in dp_list)
            {
                ////月初日期
                //var begin_date  = Year_Month_Allrecord.Where(c => c.Department == department).Min(c=>c.Date);
                ////月末日期
                //var end_date = Year_Month_Allrecord.Where(c => c.Department == department).Max(c => c.Date);
                int countByDepartment = Year_Month_Allrecord.Count(c => c.Department == department);

                Department_record.Add("department", department);
                //月初人数
                var begin_day_of_month = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == department).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;
                totalbegin = totalbegin + begin_day_of_month;
                Department_record.Add("begin_day_of_month", begin_day_of_month);
                //月末人数
                var end_day_of_month = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == department).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel + Year_Month_Allrecord.Where(c => c.Department == department).OrderByDescending(c => c.Date).FirstOrDefault().Today_on_board_employees;
                totalend = totalend + end_day_of_month;
                Department_record.Add("end_day_of_month", end_day_of_month);
                //平均人数
                decimal average = (begin_day_of_month + end_day_of_month) / 2;
                Department_record.Add("average", average);
                //整月离职人数之和
                var departure_sum = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == department).Sum(c => c.Todoy_dimission_employees_over7days);
                departure_month_sum = departure_month_sum + departure_sum;
                Department_record.Add("leave_sum", departure_sum);
                //流失率
                decimal turnoverrate = average == 0 ? 0 : departure_sum * 100 / average;
                Department_record.Add("turnoverrate", turnoverrate);
                Turnoverrate_Table.Add(i.ToString(), Department_record);
                Department_record = new JObject();
                i++;
            }
            average_sum = (totalbegin + totalend) / 2;
            //右下角总平均数值
            Turnoverrate_Table.Add("month_average", average_sum == 0 ? 0 : departure_month_sum * 100 / average_sum);
            return Content(JsonConvert.SerializeObject(Turnoverrate_Table));
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
            string List = "";
            string[] YearList = new string[12];
            JObject depChartDate = new JObject();
            JObject totalCharDate = new JObject();
            var Year_Allrecord = await db.Personnel_daily.Where(c => c.Date.Value.Year == Year).ToArrayAsync();
            var department0 = Year_Allrecord.Select(c => c.Department).Distinct().ToList();
            var months = Year_Allrecord.Select(c => c.Date.Value.Month).Distinct().ToList();

            #region------折线图
            foreach (var month in months)
            {
                decimal average_sum = 0;
                decimal stratNum = 0;
                decimal endNum = 0;
                int departure_month_sum = 0;
                //decimal avageTurnoverrate = 0;

                var departmentlist = new List<Personnel_Architecture>();
                //DateTime exT = new DateTime(Year.Value, 1, 1);
                //var extlist = db.Personnel_Architecture.Where(c => c.ExecutionTime > exT).Select(c => c.ExecutionTime).Distinct().ToList();
                //List<List<string>> alllist = new List<List<string>>();
                //foreach (var it in extlist)
                //{
                //    alllist.Add(db.Personnel_Architecture.Where(c => c.ExecutionTime == it).OrderBy(c => c.Department).Select(c => c.Department).ToList());
                //}
                //alllist.Distinct();
                ////DateTime exdate = new DateTime(Year.Value, mouth, 29, 0, 0, 0);
                //departmentlist = date.CompanyDatetime(exdate);
                //foreach (var iny in departmentlist)
                //{
                //    var dat = db.Personnel_Architecture.Where(c => c.Department == iny.Department).Select(c => c.Department).ToList();//4
                //    var gd = dat.Where(c => c.ExecutionTime.Value.Year <= exdate.Year && c.ExecutionTime.Value.Month <= exdate.Month).ToString();
                //}

                var t = new DateTime(Year.Value, month, 28);
                var lastVT = db.Personnel_Architecture.Where(c => c.ExecutionTime <= t).Max(c => c.ExecutionTime);
                departmentlist = db.Personnel_Architecture.Where(c => c.ExecutionTime == lastVT).ToList();

                List<string> department = new List<string>();
                List<string> others = new List<string>();
                foreach (var s in departmentlist)
                {
                    if (department0.Contains(s.Department))
                    {
                        department.Add(s.Department);
                    }
                    else
                    {
                        others.Add(s.Department);
                    }
                }
                department.AddRange(others);

                foreach (var dep in department)
                {
                    int count = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == month).Count();
                    if (count != 0)
                    {
                        //月初人数 
                        var begin_day_of_month = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == month).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;
                        stratNum = stratNum + begin_day_of_month;
                        //月末人数
                        var end_day_of_month = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == month).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel + Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == month).OrderByDescending(c => c.Date).FirstOrDefault().Today_on_board_employees;
                        endNum = endNum + end_day_of_month;
                        //平均数
                        decimal average = (begin_day_of_month + end_day_of_month) / 2;
                        //average_sum = average_sum + average;
                        //离职人数总和
                        var departure_sum = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == month).Sum(c => c.Todoy_dimission_employees_over7days);
                        departure_month_sum = departure_month_sum + departure_sum;
                        //每个部门的每月流失率
                        decimal turnoverrate = departure_sum * 100 / average;
                        //部门
                        // avageTurnoverrate = avageTurnoverrate + turnoverrate;
                    }
                }
                average_sum = (stratNum + endNum) / 2;
                YearList[month - 1] = average_sum == 0 ? 0.ToString() : (departure_month_sum * 100 / average_sum).ToString("F2");

            }
            string yearListString = string.Join(",", YearList);
            totalCharDate.Add("line", yearListString);
            #endregion

            #region -----柱状图
            foreach (var itme in months)
            {
                decimal average_sum1 = 0;
                int departure_month_sum1 = 0;
                var departmentlist = new List<Personnel_Architecture>();
                var t = new DateTime(Year.Value, itme, 28);
                var lastVT = db.Personnel_Architecture.Where(c => c.ExecutionTime <= t).Max(c => c.ExecutionTime);
                departmentlist = db.Personnel_Architecture.Where(c => c.ExecutionTime == lastVT).ToList();

                List<string> department = new List<string>();
                List<string> others = new List<string>();
                foreach (var s in departmentlist)
                {
                    if (department0.Contains(s.Department))
                    {
                        department.Add(s.Department);
                    }
                    else
                    {
                        others.Add(s.Department);
                    }
                }
                department.AddRange(others);

                foreach (var dep in department)
                {
                    //decimal avageTurnoverrate = 0;

                    int count = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == itme).Count();
                    if (count == 0)
                    {
                        List = "0.00";
                    }
                    else
                    {
                        //月初人数 
                        var begin_day_of_month = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == itme).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;
                        //月末人数
                        var end_day_of_month = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == itme).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel + Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == itme).OrderByDescending(c => c.Date).FirstOrDefault().Today_on_board_employees;
                        //平均数
                        decimal average = (begin_day_of_month + end_day_of_month) / 2;
                        average_sum1 = average_sum1 + average;
                        //离职人数总和
                        var departure_sum = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == itme).Sum(c => c.Todoy_dimission_employees_over7days);
                        departure_month_sum1 = departure_month_sum1 + departure_sum;
                        //每个部门的每月流失率
                        decimal turnoverrate = departure_sum * 100 / average;

                        List = turnoverrate.ToString("F2");

                        //部门
                        // avageTurnoverrate = avageTurnoverrate + turnoverrate;
                    }
                    depChartDate.Add(dep, List);
                    //string mouthList = string.Join(",", List);
                    //depChartDate.Add("data", mouthList);
                    //depChartDate.Add("mouthYear", departure_month_sum * 100 / average_sum);
                    //各个部门的每月流失率

                }
                yearChartDate.Add(itme.ToString(), depChartDate);
                depChartDate = new JObject();
            }

            totalCharDate.Add("columnar", yearChartDate);


            if (months.Count() != 0)
            {
                return Content(JsonConvert.SerializeObject(totalCharDate));
            }

            #endregion
            return Content("没有记录！");

        }
        //查看部门整年流失率情况
        [HttpPost]
        public async Task<ActionResult> TurnoverrateYearly(string department, int? year)
        {
            if (department == null || year == null)
            {
                return Content(department == null ? "部门为空！" : "" + year == null ? "年份未选择！" : "");
            }
            //var datalist = await db.Personnel_Turnoverrate.Where(c => c.Department == department && c.Year == year).ToListAsync();
            JObject MouthList = new JObject();
            JObject totalList = new JObject();
            var Year_Department_Allrecord = await db.Personnel_daily.Where(c => c.Date.Value.Year == year && c.Department == department).ToArrayAsync();
            var mouths = Year_Department_Allrecord.Select(c => c.Date.Value.Month).Distinct();
            decimal totalTurnoverrate = 0;
            int i = 0;
            foreach (var mouth in mouths)
            {
                i++;

                int count = Year_Department_Allrecord.Where(c => c.Date.Value.Month == mouth).Count();

                MouthList.Add("Mcouth", mouth);
                //月初人数
                var begin_day_of_month = Year_Department_Allrecord.Where(c => c.Date.Value.Month == mouth).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;
                MouthList.Add("begin_day_of_month", begin_day_of_month);
                //月末人数
                var end_day_of_month = Year_Department_Allrecord.Where(c => c.Date.Value.Month == mouth).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel + Year_Department_Allrecord.Where(c => c.Date.Value.Month == mouth).OrderByDescending(c => c.Date).FirstOrDefault().Today_on_board_employees;
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
                MouthList = new JObject();
            }
            totalList.Add("avgturnoverrate", totalTurnoverrate / i);
            if (Year_Department_Allrecord.Count() != 0)
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
        public ActionResult TurnoverrateInput()
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


    //API接口
    public class Personnel_Turnoverrate_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController common = new CommonController();
        CommonController dateTime = new CommonController();

      

        #region ------ 按月查看部门流失率情况
        [HttpPost]
        [ApiAuthorize]
        public JObject Turnoverrate([System.Web.Http.FromBody]JObject data)
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
            JObject Department_record = new JObject();
            var Year_Month_Allrecord = db.Personnel_daily.Where(c => c.Date.Value.Year == Year && c.Date.Value.Month == Month).ToList();
            var Department_list = Year_Month_Allrecord.Select(c => c.Department).Distinct().ToList();
            if (Year_Month_Allrecord.Count() == 0)
            {
                return common.GetModuleFromJobjet(result, false, "没有记录！");
            }
            var departmentlist = new List<Personnel_Architecture>();
            DateTime exdate = new DateTime(Year.Value, Month.Value, 28, 0, 0, 0);
            departmentlist = dateTime.CompanyDatetime(exdate);
            List<string> dp_list = new List<string>();
            List<string> others = new List<string>();
            foreach (var s in departmentlist)
            {
                if (Department_list.Contains(s.Department))
                {
                    dp_list.Add(s.Department);
                }
                else
                {
                    others.Add(s.Department);
                }
            }
            dp_list.AddRange(others);
            decimal average_sum = 0;
            decimal totalbegin = 0;
            decimal totalend = 0;
            int departure_month_sum = 0;
            int i = 0;
            foreach (var department in dp_list)
            {
                ////月初日期
                //var begin_date  = Year_Month_Allrecord.Where(c => c.Department == department).Min(c=>c.Date);
                ////月末日期
                //var end_date = Year_Month_Allrecord.Where(c => c.Department == department).Max(c => c.Date);
                int countByDepartment = Year_Month_Allrecord.Count(c => c.Department == department);

                Department_record.Add("department", department);
                //月初人数
                var begin_day_of_month = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == department).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;
                totalbegin = totalbegin + begin_day_of_month;
                Department_record.Add("begin_day_of_month", begin_day_of_month);
                //月末人数
                var end_day_of_month = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == department).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel + Year_Month_Allrecord.Where(c => c.Department == department).OrderByDescending(c => c.Date).FirstOrDefault().Today_on_board_employees;
                totalend = totalend + end_day_of_month;
                Department_record.Add("end_day_of_month", end_day_of_month);
                //平均人数
                decimal average = (begin_day_of_month + end_day_of_month) / 2;
                Department_record.Add("average", average);
                //整月离职人数之和
                var departure_sum = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == department).Sum(c => c.Todoy_dimission_employees_over7days);
                departure_month_sum = departure_month_sum + departure_sum;
                Department_record.Add("leave_sum", departure_sum);
                //流失率
                decimal turnoverrate = average == 0 ? 0 : departure_sum * 100 / average;
                Department_record.Add("turnoverrate", turnoverrate);
                result.Add(i.ToString(), Department_record);
                Department_record = new JObject();
                i++;
            }
            average_sum = (totalbegin + totalend) / 2;
            //右下角总平均数值
            result.Add("month_average", average_sum == 0 ? 0 : departure_month_sum * 100 / average_sum);
            return common.GetModuleFromJobjet(result);
        }

        #endregion

        #region------ 流失率年度折线图数据
        [HttpPost]
        [ApiAuthorize]
        public JObject TurnoverrateYearlyLineChartData([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int? Year = obj.Year;//年
            if (Year == null)
            {
                return common.GetModuleFromJobjet(result, false, Year == null ? "年份未选择！" : "");
            }
            JObject yearChartDate = new JObject();
            string List = "";
            string[] YearList = new string[12];
            JObject depChartDate = new JObject();
            var Year_Allrecord = db.Personnel_daily.Where(c => c.Date.Value.Year == Year).ToArray();
            var department0 = Year_Allrecord.Select(c => c.Department).Distinct().ToList();
            var months = Year_Allrecord.Select(c => c.Date.Value.Month).Distinct().ToList();
            if (months.Count() > 0)
            {
                #region------折线图
                foreach (var month in months)
                {
                    decimal average_sum = 0;
                    decimal stratNum = 0;
                    decimal endNum = 0;
                    int departure_month_sum = 0;
                    var departmentlist = new List<Personnel_Architecture>();
                    var t = new DateTime(Year.Value, month, 28);
                    var lastVT = db.Personnel_Architecture.Where(c => c.ExecutionTime <= t).Max(c => c.ExecutionTime);
                    departmentlist = db.Personnel_Architecture.Where(c => c.ExecutionTime == lastVT).ToList();

                    List<string> department = new List<string>();
                    List<string> others = new List<string>();
                    foreach (var s in departmentlist)
                    {
                        if (department0.Contains(s.Department))
                        {
                            department.Add(s.Department);
                        }
                        else
                        {
                            others.Add(s.Department);
                        }
                    }
                    department.AddRange(others);

                    foreach (var dep in department)
                    {
                        int count = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == month).Count();
                        if (count != 0)
                        {
                            //月初人数 
                            var begin_day_of_month = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == month).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;
                            stratNum = stratNum + begin_day_of_month;
                            //月末人数
                            var end_day_of_month = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == month).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel + Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == month).OrderByDescending(c => c.Date).FirstOrDefault().Today_on_board_employees;
                            endNum = endNum + end_day_of_month;
                            //平均数
                            decimal average = (begin_day_of_month + end_day_of_month) / 2;
                            //average_sum = average_sum + average;
                            //离职人数总和
                            var departure_sum = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == month).Sum(c => c.Todoy_dimission_employees_over7days);
                            departure_month_sum = departure_month_sum + departure_sum;
                            //每个部门的每月流失率
                            decimal turnoverrate = departure_sum * 100 / average;
                            //部门
                            // avageTurnoverrate = avageTurnoverrate + turnoverrate;
                        }
                    }
                    average_sum = (stratNum + endNum) / 2;
                    YearList[month - 1] = average_sum == 0 ? 0.ToString() : (departure_month_sum * 100 / average_sum).ToString("F2");

                }
                string yearListString = string.Join(",", YearList);
                result.Add("line", yearListString);
                #endregion

                #region -----柱状图
                foreach (var itme in months)
                {
                    decimal average_sum1 = 0;
                    int departure_month_sum1 = 0;
                    var departmentlist = new List<Personnel_Architecture>();
                    var t = new DateTime(Year.Value, itme, 28);
                    var lastVT = db.Personnel_Architecture.Where(c => c.ExecutionTime <= t).Max(c => c.ExecutionTime);
                    departmentlist = db.Personnel_Architecture.Where(c => c.ExecutionTime == lastVT).ToList();

                    List<string> department = new List<string>();
                    List<string> others = new List<string>();
                    foreach (var s in departmentlist)
                    {
                        if (department0.Contains(s.Department))
                        {
                            department.Add(s.Department);
                        }
                        else
                        {
                            others.Add(s.Department);
                        }
                    }
                    department.AddRange(others);

                    foreach (var dep in department)
                    {
                        //decimal avageTurnoverrate = 0;

                        int count = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == itme).Count();
                        if (count == 0)
                        {
                            List = "0.00";
                        }
                        else
                        {
                            //月初人数 
                            var begin_day_of_month = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == itme).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;
                            //月末人数
                            var end_day_of_month = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == itme).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel + Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == itme).OrderByDescending(c => c.Date).FirstOrDefault().Today_on_board_employees;
                            //平均数
                            decimal average = (begin_day_of_month + end_day_of_month) / 2;
                            average_sum1 = average_sum1 + average;
                            //离职人数总和
                            var departure_sum = Year_Allrecord.Where(c => c.Department == dep && c.Date.Value.Month == itme).Sum(c => c.Todoy_dimission_employees_over7days);
                            departure_month_sum1 = departure_month_sum1 + departure_sum;
                            //每个部门的每月流失率
                            decimal turnoverrate = departure_sum * 100 / average;

                            List = turnoverrate.ToString("F2");

                            //部门
                            // avageTurnoverrate = avageTurnoverrate + turnoverrate;
                        }
                        depChartDate.Add(dep, List);
                        //string mouthList = string.Join(",", List);
                        //depChartDate.Add("data", mouthList);
                        //depChartDate.Add("mouthYear", departure_month_sum * 100 / average_sum);
                        //各个部门的每月流失率

                    }
                    yearChartDate.Add(itme.ToString(), depChartDate);
                    depChartDate = new JObject();
                }
                result.Add("columnar", yearChartDate);
                #endregion

                return common.GetModuleFromJobjet(result);
            }
            else
                return common.GetModuleFromJobjet(result, false, "没有记录！");
        }
        #endregion

        #region------ 查看部门整年流失率情况
        [HttpPost]
        [ApiAuthorize]
        public JObject TurnoverrateYearly([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int? year = obj.year;//年
            string department = obj.department == null ? null : obj.department;
            if (department == null || year == null)
            {
                return common.GetModuleFromJobjet(result, false, department == null ? "部门为空！" : "" + year == null ? "年份未选择！" : "");
            }
            //var datalist = await db.Personnel_Turnoverrate.Where(c => c.Department == department && c.Year == year).ToListAsync();
            JObject MouthList = new JObject();
            var Year_Department_Allrecord = db.Personnel_daily.Where(c => c.Date.Value.Year == year && c.Department == department).ToArray();
            if (Year_Department_Allrecord.Count() > 0)
            {
                var mouths = Year_Department_Allrecord.Select(c => c.Date.Value.Month).Distinct();
                decimal totalTurnoverrate = 0;
                int i = 0;
                foreach (var mouth in mouths)
                {
                    i++;
                    int count = Year_Department_Allrecord.Where(c => c.Date.Value.Month == mouth).Count();

                    MouthList.Add("Mcouth", mouth);
                    //月初人数
                    var begin_day_of_month = Year_Department_Allrecord.Where(c => c.Date.Value.Month == mouth).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;
                    MouthList.Add("begin_day_of_month", begin_day_of_month);
                    //月末人数
                    var end_day_of_month = Year_Department_Allrecord.Where(c => c.Date.Value.Month == mouth).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel + Year_Department_Allrecord.Where(c => c.Date.Value.Month == mouth).OrderByDescending(c => c.Date).FirstOrDefault().Today_on_board_employees;
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

                    result.Add(mouth.ToString(), MouthList);
                    MouthList = new JObject();
                }
                result.Add("avgturnoverrate", totalTurnoverrate / i);

                return common.GetModuleFromJobjet(result);
            }
            else
            {
                return common.GetModuleFromJobjet(result, false, "没有记录！");
            }
        }

        #endregion



    }


}

