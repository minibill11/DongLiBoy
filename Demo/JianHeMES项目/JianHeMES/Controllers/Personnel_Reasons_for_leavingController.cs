﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using iTextSharp.text;

namespace JianHeMES.Controllers
{
    public class Personnel_Reasons_for_leavingController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// 不需要用input
        /// </summary>
        private static List<input> input = new List<input>();


        #region---离职情况汇总调差表查看

        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Reasons_for_leaving", act = "Index" });
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> For_leaving(int? Year = null, int? Month = null)
        {

            //if (Year == null || Month == null)
            //{
            //    return Content(Year == null ? "年份" : "" + Month == null ? "月份" : "" + "未选择！");
            //}
            JObject for_leaving_Table = new JObject();
            JObject Department_record = new JObject();
            JObject surface = new JObject();
            var Year_Month_Allrecord = await db.Personnel_daily.ToListAsync();
            var leaving = db.Personnel_Reasons_for_leaving.Where(c => c.LastDate != null).ToList();
            if (Year != null)
            {
                Year_Month_Allrecord = Year_Month_Allrecord.Where(c => c.Date.Value.Year == Year).ToList();
                leaving = leaving.Where(c => c.LastDate.Value.Year == Year).ToList();
            }
            if (Month != null)
            {
                Year_Month_Allrecord = Year_Month_Allrecord.Where(c => c.Date.Value.Month == Month).ToList();
                leaving = leaving.Where(c => c.LastDate.Value.Month == Month).ToList();
            }
            var Department_list = Year_Month_Allrecord.Select(c => c.Department).Distinct().ToList();

            //没有引用
            // var department_leaving = leaving.Select(c => c.Department_leaving).Distinct().ToList();
            var depar = new List<Personnel_Architecture>();
            if (Year_Month_Allrecord.Count() == 0)
            {
                JObject mes = new JObject();
                mes.Add("msg", "没有记录!");
                mes.Add("status", "202");
                return Content(JsonConvert.SerializeObject(mes));
            }
            else
            {
                CommonController date = new CommonController();
                DateTime exdate = new DateTime(Year.Value, Month.Value, 28, 0, 0, 0);
                depar = date.CompanyDatetime(exdate);
            }

            List<string> dp_list = new List<string>();
            List<string> others = new List<string>();
            foreach (var i in depar)
            {
                if (Department_list.Contains(i.Department))
                {
                    dp_list.Add(i.Department);
                }
                else
                {
                    others.Add(i.Department);
                }
            }
            dp_list.AddRange(others);
            decimal average_sum = 0;
            int departure_month_sum = 0;
            int k = 0;
            int jishu = 0;//技术族
            int caozuo = 0;//操作族
            int fuwu = 0;//服务族
            int zhuanye = 0;//专业族
            int guanli = 0;//管理族
            int couxin = 0;//薪酬不满意
            int zhidu = 0;//制度不满意
            int guan = 0;//管理不满意
            int gong = 0;//工作环境不满意
            int geng = 0;//更好的发展
            foreach (var department in dp_list)
            {
                int countByDepartment = Year_Month_Allrecord.Count(c => c.Department == department);
                if (Month == null)
                {
                    int average = 0;
                    double year_li = 0;
                    Department_record.Add("begin_day_of_month", "/");
                    Department_record.Add("end_day_of_month", "/");
                    var mouthlist = Year_Month_Allrecord.Where(c => c.Department == department).Select(c => c.Date.Value.Month).Distinct().ToList();
                    foreach (var item in mouthlist)
                    {
                        var newSoure = Year_Month_Allrecord.Where(c => c.Date.Value.Month == item).ToList();
                        //月初人数
                        var begin_day_of_month = countByDepartment == 0 ? 0 : newSoure.Where(c => c.Department == department).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;

                        //月末人数
                        var end_day_of_month = countByDepartment == 0 ? 0 : newSoure.Where(c => c.Department == department).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel + newSoure.Where(c => c.Department == department).OrderByDescending(c => c.Date).FirstOrDefault().Today_on_board_employees;

                        //平均人数
                        var averagemouth = (begin_day_of_month + end_day_of_month) / 2;
                        average = average + averagemouth;

                        //每月离职人数
                        var mouth_li = countByDepartment == 0 ? 0 : newSoure.Where(c => c.Department == department).Sum(c => c.Todoy_dimission_employees_nvever_over7days) + newSoure.Where(c => c.Department == department).Sum(c => c.Todoy_dimission_employees_over7days);
                        year_li = year_li + (mouth_li * 100 / averagemouth);
                        departure_month_sum = departure_month_sum + mouth_li;

                    }
                    int count = mouthlist.Count;
                    average = count == 0 ? 0 : average / count;
                    average_sum = average_sum + average;
                    Department_record.Add("average", average);

                    //保留两位小数
                    year_li = count == 0 ? 0 : year_li / count;

                    Department_record.Add("for_leaving", year_li.ToString("F2") + "%");

                }
                else
                {
                    //月初人数
                    var begin_day_of_month = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == department).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel;
                    Department_record.Add("begin_day_of_month", begin_day_of_month);
                    //月末人数
                    var end_day_of_month = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == department).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel + Year_Month_Allrecord.Where(c => c.Department == department).OrderByDescending(c => c.Date).FirstOrDefault().Today_on_board_employees;
                    Department_record.Add("end_day_of_month", end_day_of_month);

                    //平均人数
                    var average = (begin_day_of_month + end_day_of_month) / 2;

                    average_sum = average_sum + average;
                    Department_record.Add("average", average);
                    //离职人数
                    var li_departure = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == department).Sum(c => c.Todoy_dimission_employees_nvever_over7days) + Year_Month_Allrecord.Where(c => c.Department == department).Sum(c => c.Todoy_dimission_employees_over7days);
                    //离职人数总和
                    departure_month_sum = departure_month_sum + li_departure;
                    //保留两位小数
                    string for_leaving = (average == 0 ? 0 : li_departure * 100 / average).ToString("F2") + "%";
                    Department_record.Add("for_leaving", for_leaving);
                }
                //入职人数
                var ru_departure_sum = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == department).Sum(c => c.Today_on_board_employees);
                Department_record.Add("ru_departure_sum", ru_departure_sum);
                //入职人数总和
                //departure_month_sum = departure_month_sum + ru_departure_sum;

                //离职人数
                var li_departure_sum = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == department).Sum(c => c.Todoy_dimission_employees_nvever_over7days) + Year_Month_Allrecord.Where(c => c.Department == department).Sum(c => c.Todoy_dimission_employees_over7days);
                Department_record.Add("li_departure_sum", li_departure_sum);

                //离职族群汇总levelType
                var levelTypejishu = leaving.Count(c => c.levelType == "技术族" && c.Department_leaving == department);
                jishu = jishu + levelTypejishu;
                Department_record.Add("levelTypejishu", levelTypejishu);

                var levelTypecaozuo = leaving.Count(c => c.levelType == "操作族" && c.Department_leaving == department);
                caozuo = caozuo + levelTypecaozuo;
                Department_record.Add("levelTypecaozuo", levelTypecaozuo);

                var levelTypefuwu = leaving.Count(c => c.levelType == "服务族" && c.Department_leaving == department);
                fuwu = fuwu + levelTypefuwu;
                Department_record.Add("levelTypefuwu", levelTypefuwu);

                var levelTypezhuanye = leaving.Count(c => c.levelType == "专业族" && c.Department_leaving == department);
                zhuanye = zhuanye + levelTypezhuanye;
                Department_record.Add("levelTypezhuanye", levelTypezhuanye);

                var levelTypeguanli = leaving.Count(c => c.levelType == "管理族" && c.Department_leaving == department);
                guanli = guanli + levelTypeguanli;
                Department_record.Add("levelTypeguanli", levelTypeguanli);
                //离职原因
                var py_couxin = leaving.Count(c => c.Pay_dissatisfaction == 1 && c.Department_leaving == department);
                couxin = couxin + py_couxin;
                Department_record.Add("py_couxin", py_couxin);

                var py_zhidu = leaving.Count(c => c.NotSystem == 1 && c.Department_leaving == department);
                zhidu = zhidu + py_zhidu;
                Department_record.Add("py_zhidu", py_zhidu);

                var py_guan = leaving.Count(c => c.Notmanagement == 1 && c.Department_leaving == department);
                guan = guan + py_guan;
                Department_record.Add("py_guan", py_guan);

                var py_gong = leaving.Count(c => c.Jobenvironment == 1 && c.Department_leaving == department);
                gong = gong + py_gong;
                Department_record.Add("py_gong", py_gong);

                var py_geng = leaving.Count(c => c.Better_development == 1 && c.Department_leaving == department);
                geng = geng + py_geng;
                Department_record.Add("py_geng", py_geng);
                //离职率
                //decimal for_leaving = average == 0 ? 0 : li_departure_sum * 100 / average;

                Department_record.Add("department", department);
                for_leaving_Table.Add(k.ToString(), Department_record);
                Department_record = new JObject();
                k++;

            }

            //离职总率
            //for_leaving_Table.Add("month_average", average_sum == 0 ? 0 : departure_month_sum * 100 / average_sum);//表1
            //保留两位小数
            for_leaving_Table.Add("month_average", (average_sum == 0 ? 0 : departure_month_sum * 100 / average_sum).ToString("F2") + "%");//表1


            #region------表2
            JObject leave_record1 = new JObject();//离职原因
            JObject leave_record2 = new JObject();//占比
            JObject leave_record3 = new JObject();//表2
            List<string> levelType_list = new List<string> { "技术族", "专业族", "管理族", "服务族" };
            //操作族的离职原因
            var leave_xinzi = leaving.Count(c => c.levelType == "操作族" && c.Pay_dissatisfaction != 0);
            leave_record1.Add("leave_xinzi", leave_xinzi);//leave_xinzi薪酬不满意

            var leave_zhidu = leaving.Count(c => c.levelType == "操作族" && c.NotSystem != 0);
            leave_record1.Add("leave_zhidu", leave_zhidu);// leave_zhidu制度不满意

            var leave_guanli = leaving.Count(c => c.levelType == "操作族" && c.Notmanagement != 0);
            leave_record1.Add("leave_guanli", leave_guanli);//leave_guanli管理不满意

            var leave_huanji = leaving.Count(c => c.levelType == "操作族" && c.Jobenvironment != 0);
            leave_record1.Add("leave_huanji", leave_huanji);//leave_huanji工作环境不满意

            var leave_genghao = leaving.Count(c => c.levelType == "操作族" && c.Better_development != 0);
            leave_record1.Add("leave_genghao", leave_genghao);// leave_genghao更好的发展

            //非操作族离职原因
            var cause_couxin = leaving.Count(c => levelType_list.Contains(c.levelType) && c.Pay_dissatisfaction != 0);
            leave_record1.Add("cause_couxin", cause_couxin);//cause_couxin薪酬不满意

            var cause_zhidu = leaving.Count(c => levelType_list.Contains(c.levelType) && c.NotSystem != 0);
            leave_record1.Add("cause_zhidu", cause_zhidu);//cause_zhidu 制度不满意

            var cause_guanli = leaving.Count(c => levelType_list.Contains(c.levelType) && c.Notmanagement != 0);
            leave_record1.Add("cause_guanli", cause_guanli);//cause_guanli管理不满意

            var cause_gongzuo = leaving.Count(c => levelType_list.Contains(c.levelType) && c.Jobenvironment != 0);
            leave_record1.Add("cause_gongzuo", cause_gongzuo);//cause_gongzuo工作环境不满意

            var cause_fazhang = leaving.Count(c => levelType_list.Contains(c.levelType) && c.Better_development != 0);
            leave_record1.Add("cause_fazhang", cause_fazhang);//cause_fazhang更好的发展

            //离职原因小计
            var Pay_diss = leave_xinzi + cause_couxin;
            leave_record1.Add("Pay_diss", Pay_diss);
            var System = leave_zhidu + cause_zhidu;
            leave_record1.Add("System", System);
            var management = leave_guanli + cause_guanli;
            leave_record1.Add("management", management);
            var Joben = leave_huanji + cause_gongzuo;
            leave_record1.Add("Joben", Joben);
            var Better = leave_genghao + cause_fazhang;
            leave_record1.Add("Better", Better);

            #region 表2中没有操作族合计和非操作族合计
            ////操作族的离职原因小计
            var Operating_sum = leave_xinzi + leave_zhidu + leave_guanli + leave_huanji + leave_genghao;
            //leave_record1.Add("Operating_sum", Operating_sum);
            ////非操作族的离职原因小计
            var Notoperation_sum = cause_couxin + cause_zhidu + cause_guanli + cause_gongzuo + cause_fazhang;
            //leave_record1.Add("Notoperation_sum", Notoperation_sum);
            #endregion
            //合计
            var cause_heji = Pay_diss + System + management + Joben + Better;
            leave_record1.Add("cause_heji", cause_heji);
            //占比操作族
            var proportion_couxin = (Operating_sum == 0 ? 0 : (leave_xinzi * 100 / Operating_sum)).ToString("F2") + "%";//薪酬不满意
            leave_record2.Add("proportion_couxin", proportion_couxin);
            var proportion_zhidu = (Operating_sum == 0 ? 0 : (leave_zhidu * 100 / Operating_sum)).ToString("F2") + "%";//制度不满意
            leave_record2.Add("proportion_zhidu", proportion_zhidu);
            var proportion_guanli = (Operating_sum == 0 ? 0 : (leave_guanli * 100 / Operating_sum)).ToString("F2") + "%";//管理不满意
            leave_record2.Add("proportion_guanli", proportion_guanli);
            var proportion_huanji = (Operating_sum == 0 ? 0 : (leave_huanji * 100 / Operating_sum)).ToString("F2") + "%";//工作环境不满意
            leave_record2.Add("proportion_huanji", proportion_huanji);
            var proportion_genghao = (Operating_sum == 0 ? 0 : (leave_genghao * 100 / Operating_sum)).ToString("F2") + "%";//更好的发展
            leave_record2.Add("proportion_genghao", proportion_genghao);

            //非操作族占比
            var account_couxin = (Notoperation_sum == 0 ? 0 : (cause_couxin * 100 / Notoperation_sum)).ToString("F2") + "%";//薪酬不满意
            leave_record2.Add("account_couxin", account_couxin);
            var account_zhidu = (Notoperation_sum == 0 ? 0 : (cause_zhidu * 100 / Notoperation_sum)).ToString("F2") + "%";//制度不满意
            leave_record2.Add("account_zhidu", account_zhidu);
            var account_guanli = (Notoperation_sum == 0 ? 0 : (cause_guanli * 100 / Notoperation_sum)).ToString("F2") + "%";//管理不满意
            leave_record2.Add("account_guanli", account_guanli);
            var account_gongzuo = (Notoperation_sum == 0 ? 0 : (cause_gongzuo * 100 / Notoperation_sum)).ToString("F2") + "%";//工作环境不满意
            leave_record2.Add("account_gongzuo", account_gongzuo);
            var account_fazhang = (Notoperation_sum == 0 ? 0 : (cause_fazhang * 100 / Notoperation_sum)).ToString("F2") + "%";//更好的发展
            leave_record2.Add("account_fazhang", account_fazhang);

            //离职原因占比
            var Leaving_salary = (cause_heji == 0 ? 0 : (Pay_diss * 100 / cause_heji)).ToString("F2") + "%";//薪酬不满意
            leave_record2.Add("Leaving_salary", Leaving_salary);
            var Leaving_System = (cause_heji == 0 ? 0 : (System * 100 / cause_heji)).ToString("F2") + "%";//制度不满意
            leave_record2.Add("Leaving_System", Leaving_System);
            var Leaving_management = (cause_heji == 0 ? 0 : (management * 100 / cause_heji)).ToString("F2") + "%";//管理不满意
            leave_record2.Add("Leaving_management", Leaving_management);
            var Leaving_Joben = (cause_heji == 0 ? 0 : (Joben * 100 / cause_heji)).ToString("F2") + "%";//工作环境不满意
            leave_record2.Add("Leaving_Joben", Leaving_Joben);
            var Leaving_Better = (cause_heji == 0 ? 0 : (Better * 100 / cause_heji)).ToString("F2") + "%";//工作环境不满意
            leave_record2.Add("Leaving_Better", Leaving_Better);

            leave_record3.Add("leave_record1", leave_record1);
            leave_record3.Add("leave_record2", leave_record2);
            surface.Add("leave_record3", leave_record3);
            surface.Add("for_leaving_Table", for_leaving_Table);
            return Content(JsonConvert.SerializeObject(surface));
        }
    
    #endregion

    public string DefaualInfo()
    {
        var datetime = db.Personnel_Reasons_for_leaving.OrderByDescending(c => c.LastDate).Select(c => c.LastDate).FirstOrDefault();
        var xx = datetime.Value.Year + "," + datetime.Value.Month;
        return datetime.Value.Year + "," + datetime.Value.Month;
    }
    #region---点击部门查看离职原因明细表
    [HttpPost]
    public async Task<ActionResult> Leaving_reason(string department, int? year, int? mouth)
    {
        if (department == null || year == null || mouth == null)
        {
            return Content(department == null ? "部门为空！" : "" + year == null ? "年份未选择" : "" + mouth == null ? "月份未选择" : "");
        }
        if (Session["User"] == null)
        {
            return RedirectToAction("Login", "Users", new { col = "Personnel_Reasons_for_leaving", act = "Index" });
        }

        JObject userItem = new JObject();
        JObject userJobject = new JObject();
        var leave_mingxi = await db.Personnel_Reasons_for_leaving.ToListAsync();
        int L = 0;
        var leave = db.Personnel_Reasons_for_leaving.Where(c => c.Department_leaving == department).ToList();
        //添加日期为null的方法
        var aa = leave.Where(c => c.LastDate == null ? false : c.LastDate.Value.Year == year && c.LastDate == null ? false : c.LastDate.Value.Month == mouth).ToList();

        foreach (var item in aa)
        {
            //ID
            userItem.Add("Id", item.Id);
            //名字
            userItem.Add("Name", item.Name);
            //工号
            userItem.Add("jobNum", item.jobNum);
            //部门
            userItem.Add("department", item.Department_leaving);
            //组别
            userItem.Add("DP_group", item.DP_group);
            //职位名称
            userItem.Add("Position", item.Position);
            //族群levelType
            userItem.Add("levelType", item.levelType);
            //入职日期HireDate
            string Hiretostring = string.Format("{0:yyyy-MM-dd--HH-mm-ss}", item.HireDate);
            userItem.Add("HireDate", item.HireDate);
            //离职日期LastDate
            string Lasttostring = string.Format("{0:yyyy-MM-dd--HH-mm-ss}", item.LastDate);
            userItem.Add("LastDate", item.LastDate);
            //备注Remark
            userItem.Add("Remark", item.Remark);
            //部门结论Department_of_conclusion
            userItem.Add("Department_of_conclusion", item.Department_of_conclusion);
            //人力资源部结论HR_conclusion
            userItem.Add("HR_conclusion", item.HR_conclusion);
            //薪酬不满意Pay_dissatisfaction
            userItem.Add("Pay_dissatisfaction", item.Pay_dissatisfaction);
            //制度不满意NotSystem
            userItem.Add("NotSystem", item.NotSystem);
            //管理不满意Notmanagement
            userItem.Add("Notmanagement", item.Notmanagement);
            //工作环境不满意Jobenvironment
            userItem.Add("Jobenvironment", item.Jobenvironment);
            //更好的发展Better_development
            userItem.Add("Better_development", item.Better_development);

            userJobject.Add(L.ToString(), userItem);
            L++;
            userItem = new JObject();

        }
        return Content(JsonConvert.SerializeObject(userJobject));

    }

    #endregion

    #region Details方法
    public ActionResult Details(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Personnel_Reasons_for_leaving personnel_Reasons_for_leaving = db.Personnel_Reasons_for_leaving.Find(id);
        if (personnel_Reasons_for_leaving == null)
        {
            return HttpNotFound();
        }
        return View(personnel_Reasons_for_leaving);
    }
    #endregion

    //前端找不到这个方法
    #region ---保存数据方法
    public ActionResult Create()
    {
        if (Session["User"] == null)
        {
            return RedirectToAction("Login", "Users", new { col = "Personnel_Reasons_for_leaving", act = "Create" });
        }

        return View();
    }
    #region---保存离职原因信息
    public ActionResult ModifyCreate(string jobNum)
    {

        if (jobNum != null)
        {
            var Present = db.Personnel_Reasons_for_leaving.Where(c => c.jobNum == jobNum).FirstOrDefault();

            if (Present != null)
            {
                return Content(JsonConvert.SerializeObject(Present));
            }
        }
        return Content("false");
    }
    #endregion
    [HttpPost]
    public ActionResult Create([Bind(Include = "Id,Name,jobNum,Department_leaving,DP_group,Position,levelType,HireDate,LastDate,Remark,Department_of_conclusion,HR_conclusion,Pay_dissatisfaction,NotSystem,Notmanagement,Jobenvironment,Better_development,Creator,CreateDate")] Personnel_Reasons_for_leaving personnel_Reasons_for_leaving)
    {
        //personnel_Reasons_for_leaving.CreateDate = DateTime.Now;
        //    personnel_Reasons_for_leaving.Creator = ((Users)Session["User"]).UserName;

        //db.Entry(personnel_Reasons_for_leaving).State = EntityState.Added;
        if (personnel_Reasons_for_leaving.Pay_dissatisfaction != 0 || personnel_Reasons_for_leaving.NotSystem != 0 || personnel_Reasons_for_leaving.Notmanagement != 0 || personnel_Reasons_for_leaving.Jobenvironment != 0 || personnel_Reasons_for_leaving.Better_development != 0)
        {

            var amend = db.Personnel_Reasons_for_leaving.Where(c => c.jobNum == personnel_Reasons_for_leaving.jobNum).FirstOrDefault();
            if (amend != null)
            {
                amend.levelType = personnel_Reasons_for_leaving.levelType;
                amend.Pay_dissatisfaction = personnel_Reasons_for_leaving.Pay_dissatisfaction;
                amend.NotSystem = personnel_Reasons_for_leaving.NotSystem;
                amend.Notmanagement = personnel_Reasons_for_leaving.Notmanagement;
                amend.Jobenvironment = personnel_Reasons_for_leaving.Jobenvironment;
                amend.Better_development = personnel_Reasons_for_leaving.Better_development;
                db.SaveChanges();
                return Content("ok");
            }
            if (ModelState.IsValid)
            {
                db.Personnel_Reasons_for_leaving.Add(personnel_Reasons_for_leaving);
                db.SaveChanges();
                return Content("ok");
            }

        }
        return Content("离职原因为空，请重新输入");
    }
    #endregion

    #region------批量增加方法
    public ActionResult Batch_leave()
    {
        if (Session["User"] == null)
        {
            return RedirectToAction("Login", "Users", new { col = "Personnel_Reasons_for_leaving", act = "Batch_leave" });
        }
        return Content("<script>alert('对不起，您不能进行离职原因调查批量增加，请联系统管理人员！');window.location.href='../Personnel_Reasons_for_leaving';</script>");

    }



    [HttpPost]
    public ActionResult Batch_leave(List<Personnel_Reasons_for_leaving> inputList)
    {

        string repeat = null;
        foreach (var item in inputList)
        {
            if (db.Personnel_Reasons_for_leaving.Count(c => c.jobNum == item.jobNum) != 0)
                repeat = repeat + item.jobNum + ",";
        }
        if (!string.IsNullOrEmpty(repeat))
        {
            return Content(repeat);
        }
        if (ModelState.IsValid)
        {
            db.Personnel_Reasons_for_leaving.AddRange(inputList);
            db.SaveChangesAsync();
            return Content("true");

        }
        return Content("false");
    }

    #endregion

    #region----- 修改方法
    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Personnel_Reasons_for_leaving personnel_Reasons_for_leaving = db.Personnel_Reasons_for_leaving.Find(id);
        if (personnel_Reasons_for_leaving == null)
        {
            return HttpNotFound();
        }
        return View(personnel_Reasons_for_leaving);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Name,jobNum,Department_leaving,DP_group,Position,levelType,HireDate,LastDate,Remark,Department_of_conclusion,HR_conclusion,Pay_dissatisfaction,NotSystem,Notmanagement,Jobenvironment,Better_development")] Personnel_Reasons_for_leaving personnel_Reasons_for_leaving)
    {
        if (ModelState.IsValid)
        {
            if (!string.IsNullOrEmpty(personnel_Reasons_for_leaving.jobNum))
            {
                personnel_Reasons_for_leaving.CreateDate = DateTime.Now;
                personnel_Reasons_for_leaving.Creator = ((Users)Session["User"]).UserName;
                db.Entry(personnel_Reasons_for_leaving).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Content("true");
            }

        }
        return Content("修改出错，请确认数据是否正确");
    }

    #endregion

    #region ------删除
    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Personnel_Reasons_for_leaving personnel_Reasons_for_leaving = db.Personnel_Reasons_for_leaving.Find(id);
        if (personnel_Reasons_for_leaving == null)
        {
            return HttpNotFound();
        }
        return View(personnel_Reasons_for_leaving);
    }


    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
        Personnel_Reasons_for_leaving personnel_Reasons_for_leaving = db.Personnel_Reasons_for_leaving.Find(id);
        db.Personnel_Reasons_for_leaving.Remove(personnel_Reasons_for_leaving);
        db.SaveChanges();
        return Content("true");
    }
    #endregion

    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}
}
