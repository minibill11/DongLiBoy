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
using JianHeMES.AuthAttributes;

namespace JianHeMES.Controllers
{
    public class Personnel_RecruitmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        #region------------招聘周报查看
        public ActionResult Recruitment()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Recruitment", act = "Recruitment" });
            }
            return View();
        }

        [HttpPost]
        public ActionResult DefaualInfo()
        {
            JObject info = new JObject();
            //获取数据
            var lastTime = db.Personnel_Recruitment.OrderByDescending(c => new { c.Year, c.Month, c.Week }).Select(c => new { c.Year, c.Month, c.Week }).FirstOrDefault();
            var lastInfo = db.Personnel_Recruitment.Where(c => c.Year == lastTime.Year && c.Month == lastTime.Month && c.Week == lastTime.Week).ToList();
            //添加
            info.Add("Time", "year:" + lastTime.Year + ",month:" + lastTime.Month + ",week:" + lastTime.Week);
            info.Add("Data", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(lastInfo)));

            if (lastInfo.Count != 0)
            {
                return Content(JsonConvert.SerializeObject(info));
            }
            else
                return Content("数据加载失败");
        }
        [HttpPost]
        public async Task<ActionResult> Recruitment(int? Year, int? Month, int? Week)
        {
            if (Year == null || Month == null || Week == null)
            {
                return Content(Year == null ? "年份" : "" + Month == null ? "月份" : "" + Week == null ? "周" : "" + "未选择！");
            }
            JArray list = new JArray();
            var datalist = await db.Personnel_Recruitment.Where(c => c.Year == Year && c.Month == Month && c.Week == Week).ToListAsync();
            int totalemply = 0;
            int totalden = 0;
            if (datalist != null)
            {
                foreach (var item in datalist)
                {
                    JObject AA = new JObject();
                    int days = 0;
                    int count = 0;
                    if (item.Work_date == null)
                    {

                        DateTime? time = item.Application_date;
                        DateTime? end = item.Monday;
                        while (time < end)
                        {
                            if (time.Value.DayOfWeek == DayOfWeek.Sunday)
                                count++;
                            time = time.Value.AddDays(1);
                        }
                        days = (count + 1) * 5;
                    }
                    else
                    {
                        var time = item.Work_date - item.Application_date;
                        days = time.Value.Days;
                    }
                    item.Invite_Actaul_Cycle = days;
                    AA.Add("Id", item.Id);
                    AA.Add("Demand_jobs", item.Demand_jobs);
                    AA.Add("Department_weekly", item.Department_weekly);
                    AA.Add("Compile", item.Compile);
                    AA.Add("Demand_number", item.Demand_number);
                    AA.Add("Employed", item.Employed);
                    AA.Add("Unfinished_nember", item.Unfinished_nember);
                    AA.Add("Application_date", item.Application_date);
                    AA.Add("Work_date", item.Work_date);
                    AA.Add("Invite_Plan_Cycle", item.Invite_Plan_Cycle);
                    AA.Add("Invite_Actaul_Cycle", item.Invite_Actaul_Cycle);
                    list.Add(AA);
                    totalemply = totalemply + item.Employed;
                    if (item.Invite_Actaul_Cycle < item.Invite_Plan_Cycle)
                        totalden = totalden + item.Employed;
                    else
                        totalden = totalden + item.Demand_number;

                }
                //list.Add(datalist);
                list.Add(totalden == 0 ? "0.00%" : (totalemply * 100 / totalden).ToString("F2") + "%");
                return Content(JsonConvert.SerializeObject(list));
            }
            else
            {
                return Content(Year + "年" + Month + "月第" + Week + "周没有记录或尝未输入数据！");
            }
        }
        #endregion


        #region------------招聘周报输入
        public ActionResult RecruitmentInput()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Recruitment", act = "RecruitmentInput" });
            }
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> RecruitmentInput(List<Personnel_Recruitment> weekDataList)
        {

            if (weekDataList != null)
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


    //API接口
    public class Personnel_Recruitment_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController common = new CommonController();


        #region--------招聘周报查询方法
        [HttpPost]
        [ApiAuthorize]
        public JObject DefaualInfo()
        {
            JObject result = new JObject();
            //获取数据
            var lastTime = db.Personnel_Recruitment.OrderByDescending(c => new { c.Year, c.Month, c.Week }).Select(c => new { c.Year, c.Month, c.Week }).FirstOrDefault();
            var lastInfo = db.Personnel_Recruitment.Where(c => c.Year == lastTime.Year && c.Month == lastTime.Month && c.Week == lastTime.Week).ToList();
            //添加
            result.Add("Time", "year:" + lastTime.Year + ",month:" + lastTime.Month + ",week:" + lastTime.Week);
            result.Add("Data", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(lastInfo)));

            if (lastInfo.Count != 0)
            {
                return common.GetModuleFromJobjet(result);
            }
            else
                return common.GetModuleFromJobjet(result);
        }

        [HttpPost]
        [ApiAuthorize]
        public JObject Recruitment([System.Web.Http.FromBody]JObject data)
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
            var datalist = db.Personnel_Recruitment.Where(c => c.Year == Year && c.Month == Month && c.Week == Week).ToList();
            int totalemply = 0;
            int totalden = 0;
            if (datalist != null)
            {
                foreach (var item in datalist)
                {
                    JObject AA = new JObject();
                    int days = 0;
                    int count = 0;
                    if (item.Work_date == null)
                    {

                        DateTime? time = item.Application_date;
                        DateTime? end = item.Monday;
                        while (time < end)
                        {
                            if (time.Value.DayOfWeek == DayOfWeek.Sunday)
                                count++;
                            time = time.Value.AddDays(1);
                        }
                        days = (count + 1) * 5;
                    }
                    else
                    {
                        var time = item.Work_date - item.Application_date;
                        days = time.Value.Days;
                    }
                    item.Invite_Actaul_Cycle = days;
                    AA.Add("Id", item.Id);
                    AA.Add("Demand_jobs", item.Demand_jobs);
                    AA.Add("Department_weekly", item.Department_weekly);
                    AA.Add("Compile", item.Compile);
                    AA.Add("Demand_number", item.Demand_number);
                    AA.Add("Employed", item.Employed);
                    AA.Add("Unfinished_nember", item.Unfinished_nember);
                    AA.Add("Application_date", item.Application_date);
                    AA.Add("Work_date", item.Work_date);
                    AA.Add("Invite_Plan_Cycle", item.Invite_Plan_Cycle);
                    AA.Add("Invite_Actaul_Cycle", item.Invite_Actaul_Cycle);
                    result.Add(AA);
                    totalemply = totalemply + item.Employed;
                    if (item.Invite_Actaul_Cycle < item.Invite_Plan_Cycle)
                        totalden = totalden + item.Employed;
                    else
                        totalden = totalden + item.Demand_number;

                }
                result.Add(totalden == 0 ? "0.00%" : (totalemply * 100 / totalden).ToString("F2") + "%");
                if (result.Count > 0)
                {
                    return common.GetModuleFromJarray(result, true, "查询成功");
                }
                else
                {
                    return common.GetModuleFromJarray(result, false, Year + "年" + Month + "月第" + Week + "周没有记录！");
                }
            }
            else
            {
                return common.GetModuleFromJarray(result, false, Year + "年" + Month + "月第" + Week + "周没有记录");
            }
        }

        #endregion

        #region--------招聘周报批量输入方法
        [HttpPost]
        [ApiAuthorize]
        public JObject RecruitmentInput([System.Web.Http.FromBody]JObject data)
        {
            JArray result = new JArray();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            List<Personnel_Recruitment> weekDataList = obj.weekDataList;
            if (weekDataList.Count > 0)
            {
                var CreateDate = DateTime.Now;
                var Creator = auth.UserName;
                var weekDay = CreateDate.DayOfWeek.ToString();//得到今天是周几
                string res = "";

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
                foreach (var data1 in weekDataList)
                {
                    int isExist = db.Personnel_Recruitment.Count(c => c.Year == data1.Year && c.Month == data1.Month && c.Week == data1.Week && c.Department_weekly == data1.Department_weekly && c.Demand_jobs == data1.Demand_jobs);
                    if (isExist > 0)
                    {
                        if (res == "")
                        {
                            res = data1.Year + "年" + data1.Month + "月第" + data1.Week + "周" + data1.Department_weekly + data1.Demand_jobs;
                            result.Add(res);
                        }
                        else
                        {
                            res = data1.Year + "年" + data1.Month + "月第" + data1.Week + "周" + data1.Department_weekly + data1.Demand_jobs;
                            result.Add(res);
                        }
                    }
                }
                int count = 0;
                if (result.Count > 0) return common.GetModuleFromJarray(result, false, "数据已经存在，不能再输入！");
                foreach (var data1 in weekDataList)
                {
                    data1.CreateDate = CreateDate;
                    data1.Creator = Creator;
                    data1.Monday = Monday;
                    data1.Sunday = Sunday;
                    db.Personnel_Recruitment.Add(data1);
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

        #region--------招聘周报修改方法

        [HttpPost]
        [ApiAuthorize]
        public JObject RecruitmentEdit([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            Personnel_Recruitment personnel_Recruitment = JsonConvert.DeserializeObject<Personnel_Recruitment>(JsonConvert.SerializeObject(data));
            if (personnel_Recruitment != null && personnel_Recruitment.Id != 0)
            {
                personnel_Recruitment.CreateDate = DateTime.Now;
                personnel_Recruitment.Creator = auth.UserName;
                db.Entry(personnel_Recruitment).State = EntityState.Modified;
                int count = db.SaveChanges();
                if (count > 0)
                {
                    return common.GetModuleFromJobjet(result, true, "修改成功");
                }
                else
                {
                    return common.GetModuleFromJobjet(result, false, "修改失败");
                }
            }
            return common.GetModuleFromJobjet(result, false, "修改失败");
        }

        #endregion

        #region------- 删除招聘周报记录方法
        [HttpPost]
        [ApiAuthorize]
        public JObject RecruitmentDelete([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int id = obj.id;//id
            if (id != 0)
            {
                var Recrui = db.Personnel_Recruitment.Where(c => c.Id == id).FirstOrDefault();
                UserOperateLog operaterecord = new UserOperateLog();
                operaterecord.OperateDT = DateTime.Now;//添加删除操作时间
                operaterecord.Operator = auth.UserName;//添加删除操作人
                operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除" + Recrui.Creator + "创建的招聘周报记录";
                db.Personnel_Recruitment.Remove(Recrui);//删除对应的数据
                db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
                int count = db.SaveChanges();
                if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
                {
                    return common.GetModuleFromJobjet(result, true, "删除成功");
                }
                else //等于0（没有把数据保存到数据库或者保存出错）
                {
                    return common.GetModuleFromJobjet(result, false, "删除失败！");
                }
            }
            return common.GetModuleFromJobjet(result, false, "删除失败！");
        }
        #endregion



    }

}