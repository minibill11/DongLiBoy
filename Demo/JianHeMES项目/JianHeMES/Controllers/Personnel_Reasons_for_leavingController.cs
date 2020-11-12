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
using JianHeMES.AuthAttributes;

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
            int benshen = 0;//工作本身不满意
            int wenhua = 0;//文化氛围不满意
            int shenti = 0;//身体的健康
            int jiating = 0;//家庭原因
            int proba = 0;//试用期不合格
            int tiate = 0;//协商解除
            int reasons = 0;//其他原因
            int resign = 0;//离退类型：辞职
            int since = 0;//离退类型：自离
            int lay = 0;//离退类型：辞职
            int retied = 0;//离退类型：退休
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

                //离退类型汇总

                var Types_cizhi = leaving.Count(c => c.Resign == 1 && c.Department_leaving == department);
                resign = resign + Types_cizhi;
                Department_record.Add("types_cizhi", Types_cizhi);//辞职

                var Types_zili = leaving.Count(c => c.SinceFrom == 1 && c.Department_leaving == department);
                since = since + Types_zili;
                Department_record.Add("types_zili", Types_zili);//自离

                var Types_citui = leaving.Count(c => c.LayOff == 1 && c.Department_leaving == department);
                lay = lay + Types_citui;
                Department_record.Add("types_citui", Types_citui);//辞退

                var Types_tuixiu = leaving.Count(c => c.Retired == 1 && c.Department_leaving == department);
                retied = retied + Types_tuixiu;
                Department_record.Add("types_tuixiu", Types_tuixiu);//退休

                //离职原因汇总
                var py_couxin = leaving.Count(c => c.Pay_dissatisfaction == 1 && c.Department_leaving == department);
                couxin = couxin + py_couxin;
                Department_record.Add("py_couxin", py_couxin);//薪酬不满意

                var py_zhidu = leaving.Count(c => c.NotSystem == 1 && c.Department_leaving == department);
                zhidu = zhidu + py_zhidu;
                Department_record.Add("py_zhidu", py_zhidu);//制度不满意

                var py_guan = leaving.Count(c => c.Notmanagement == 1 && c.Department_leaving == department);
                guan = guan + py_guan;
                Department_record.Add("py_guan", py_guan);//管理不满意

                var py_gong = leaving.Count(c => c.Jobenvironment == 1 && c.Department_leaving == department);
                gong = gong + py_gong;
                Department_record.Add("py_gong", py_gong);//工作环境不满意

                var py_benshen = leaving.Count(c => c.NotWork_Itself == 1 && c.Department_leaving == department);
                benshen = benshen + py_benshen;
                Department_record.Add("py_benshen", py_benshen);//工作本身不满意

                var py_wenhua = leaving.Count(c => c.NotCultural_Atmosphere == 1 && c.Department_leaving == department);
                wenhua = wenhua + py_wenhua;
                Department_record.Add("py_wenhua", py_wenhua);//文化氛围不满意

                var py_geng = leaving.Count(c => c.Better_development == 1 && c.Department_leaving == department);
                geng = geng + py_geng;
                Department_record.Add("py_geng", py_geng);//更好的发展

                var py_shenti = leaving.Count(c => c.HealthyBody == 1 && c.Department_leaving == department);
                shenti = shenti + py_shenti;
                Department_record.Add("py_shenti", py_shenti);//身体的健康

                var py_jiating = leaving.Count(c => c.Family_Reasons == 1 && c.Department_leaving == department);
                jiating = jiating + py_jiating;
                Department_record.Add("py_jiating", py_jiating);//家庭原因

                var py_proba = leaving.Count(c => c.NotProbation_Period == 1 && c.Department_leaving == department);
                proba = proba + py_proba;
                Department_record.Add("py_proba", py_proba);//试用期不合格

                var py_tiate = leaving.Count(c => c.Negotiate_Remove == 1 && c.Department_leaving == department);
                tiate = tiate + py_tiate;
                Department_record.Add("py_tiate", py_tiate);//协商解除

                var py_reasons = leaving.Count(c => c.Other_Reasons == 1 && c.Department_leaving == department);
                reasons = reasons + py_reasons;
                Department_record.Add("py_reasons", py_reasons);//其他原因


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

            #region--操作族离职原因
            //操作族公司离职原因
            var leave_xinzi = leaving.Count(c => c.levelType == "操作族" && c.Pay_dissatisfaction != 0);
            leave_record1.Add("leave_xinzi", leave_xinzi);//leave_xinzi薪酬不满意

            var leave_zhidu = leaving.Count(c => c.levelType == "操作族" && c.NotSystem != 0);
            leave_record1.Add("leave_zhidu", leave_zhidu);// leave_zhidu制度不满意

            var leave_guanli = leaving.Count(c => c.levelType == "操作族" && c.Notmanagement != 0);
            leave_record1.Add("leave_guanli", leave_guanli);//leave_guanli管理不满意

            var leave_huanji = leaving.Count(c => c.levelType == "操作族" && c.Jobenvironment != 0);
            leave_record1.Add("leave_huanji", leave_huanji);//leave_huanji工作环境不满意

            var leave_benshen = leaving.Count(c => c.levelType == "操作族" && c.NotWork_Itself != 0);
            leave_record1.Add("leave_benshen", leave_benshen);//leave_benshen工作本身不满意

            var leave_wenhua = leaving.Count(c => c.levelType == "操作族" && c.NotCultural_Atmosphere != 0);
            leave_record1.Add("leave_wenhua", leave_wenhua);//leave_wenhua文化氛围不满意

            //操作族个人离职原因
            var leave_genghao = leaving.Count(c => c.levelType == "操作族" && c.Better_development != 0);
            leave_record1.Add("leave_genghao", leave_genghao);// leave_genghao更好的发展

            var leave_shenti = leaving.Count(c => c.levelType == "操作族" && c.HealthyBody != 0);
            leave_record1.Add("leave_shenti", leave_shenti);//leave_shenti身体的健康

            var leave_jiating = leaving.Count(c => c.levelType == "操作族" && c.Family_Reasons != 0);
            leave_record1.Add("leave_jiating", leave_jiating);//leave_jiating家庭原因

            //试用期不合格离职原因
            var leave_trial = leaving.Count(c => c.levelType == "操作族" && c.NotProbation_Period != 0);
            leave_record1.Add("leave_trial", leave_trial);//leave_trial试用期不合格

            //协商解除的离职原因
            var leave_remove = leaving.Count(c => c.levelType == "操作族" && c.Negotiate_Remove != 0);
            leave_record1.Add("leave_remove", leave_remove);//leave_remove协商解除

            //其他原因
            var leave_other = leaving.Count(c => c.levelType == "操作族" && c.Other_Reasons != 0);
            leave_record1.Add("leave_other", leave_other);//leave_other其他原因

            #endregion

            #region--技术族离职原因
            //技术族离职原因（公司）
            var tech_couxin = leaving.Count(c => c.levelType == "技术族" && c.Pay_dissatisfaction != 0);
            leave_record1.Add("tech_couxin", tech_couxin);//tech_couxin薪酬不满意

            var tech_zhidu = leaving.Count(c => c.levelType == "技术族" && c.NotSystem != 0);
            leave_record1.Add("tech_zhidu", tech_zhidu);//tech_zhidu 制度不满意

            var tech_guanli = leaving.Count(c => c.levelType == "技术族" && c.Notmanagement != 0);
            leave_record1.Add("tech_guanli", tech_guanli);//tech_guanli管理不满意

            var tech_gongzuo = leaving.Count(c => c.levelType == "技术族" && c.Jobenvironment != 0);
            leave_record1.Add("tech_gongzuo", tech_gongzuo);//tech_gongzuo工作环境不满意

            var tech_benshen = leaving.Count(c => c.levelType == "技术族" && c.NotWork_Itself != 0);
            leave_record1.Add("tech_benshen", tech_benshen);//tech_benshen工作本身不满意

            var tech_wenhua = leaving.Count(c => c.levelType == "技术族" && c.NotCultural_Atmosphere != 0);
            leave_record1.Add("tech_wenhua", tech_wenhua);//tech_wenhua文化氛围不满意

            //技术族个人离职原因
            var tech_fazhang = leaving.Count(c => c.levelType == "技术族" && c.Better_development != 0);
            leave_record1.Add("tech_fazhang", tech_fazhang);//tech_fazhang更好的发展

            var tech_shenti = leaving.Count(c => c.levelType == "技术族" && c.HealthyBody != 0);
            leave_record1.Add("tech_shenti", tech_shenti);//tech_shenti身体的健康

            var tech_jiating = leaving.Count(c => c.levelType == "技术族" && c.Family_Reasons != 0);
            leave_record1.Add("tech_jiating", tech_jiating);//tech_jiating家庭原因

            //试用期不合格离职原因
            var tech_trial = leaving.Count(c => c.levelType == "技术族" && c.NotProbation_Period != 0);
            leave_record1.Add("tech_trial", tech_trial);//tech_trial试用期不合格

            //协商解除的离职原因
            var tech_remove = leaving.Count(c => c.levelType == "技术族" && c.Negotiate_Remove != 0);
            leave_record1.Add("tech_remove", tech_remove);//tech_remove协商解除

            //其他原因
            var tech_other = leaving.Count(c => c.levelType == "技术族" && c.Other_Reasons != 0);
            leave_record1.Add("tech_other", tech_other);//tech_other其他原因

            #endregion

            #region--专业族离职原因
            //专业族离职原因（公司）
            var profe_couxin = leaving.Count(c => c.levelType == "专业族" && c.Pay_dissatisfaction != 0);
            leave_record1.Add("profe_couxin", profe_couxin);//profe_couxin薪酬不满意

            var profe_zhidu = leaving.Count(c => c.levelType == "专业族" && c.NotSystem != 0);
            leave_record1.Add("profe_zhidu", profe_zhidu);//profe_zhidu 制度不满意

            var profe_guanli = leaving.Count(c => c.levelType == "专业族" && c.Notmanagement != 0);
            leave_record1.Add("profe_guanli", profe_guanli);//profe_guanli管理不满意

            var profe_gongzuo = leaving.Count(c => c.levelType == "专业族" && c.Jobenvironment != 0);
            leave_record1.Add("profe_gongzuo", profe_gongzuo);//profe_gongzuo工作环境不满意

            var profe_benshen = leaving.Count(c => c.levelType == "专业族" && c.NotWork_Itself != 0);
            leave_record1.Add("profe_benshen", profe_benshen);//profe_benshen工作本身不满意

            var profe_wenhua = leaving.Count(c => c.levelType == "专业族" && c.NotCultural_Atmosphere != 0);
            leave_record1.Add("profe_wenhua", profe_wenhua);//profe_wenhua文化氛围不满意

            //专业族个人离职原因
            var profe_fazhang = leaving.Count(c => c.levelType == "专业族" && c.Better_development != 0);
            leave_record1.Add("cause_fazhang", profe_fazhang);//profe_fazhang更好的发展

            var profe_shenti = leaving.Count(c => c.levelType == "专业族" && c.HealthyBody != 0);
            leave_record1.Add("profe_shenti", profe_shenti);//profe_shenti身体的健康

            var profe_jiating = leaving.Count(c => c.levelType == "专业族" && c.Family_Reasons != 0);
            leave_record1.Add("profe_jiating", profe_jiating);//profe_jiating家庭原因

            //试用期不合格离职原因
            var profe_trial = leaving.Count(c => c.levelType == "专业族" && c.NotProbation_Period != 0);
            leave_record1.Add("profe_trial", profe_trial);//profe_trial试用期不合格

            //协商解除的离职原因
            var profe_remove = leaving.Count(c => c.levelType == "专业族" && c.Negotiate_Remove != 0);
            leave_record1.Add("profe_remove", profe_remove);//profe_remove协商解除

            //其他原因
            var profe_other = leaving.Count(c => c.levelType == "专业族" && c.Other_Reasons != 0);
            leave_record1.Add("profe_other", profe_other);//profe_other其他原因

            #endregion

            #region--服务族离职原因
            //服务族离职原因（公司）
            var service_couxin = leaving.Count(c => c.levelType == "服务族" && c.Pay_dissatisfaction != 0);
            leave_record1.Add("service_couxin", service_couxin);//service_couxin薪酬不满意

            var service_zhidu = leaving.Count(c => c.levelType == "服务族" && c.NotSystem != 0);
            leave_record1.Add("service_zhidu", service_zhidu);//service_zhidu 制度不满意

            var service_guanli = leaving.Count(c => c.levelType == "服务族" && c.Notmanagement != 0);
            leave_record1.Add("service_guanli", service_guanli);//service_guanli管理不满意

            var service_gongzuo = leaving.Count(c => c.levelType == "服务族" && c.Jobenvironment != 0);
            leave_record1.Add("service_gongzuo", service_gongzuo);//service_gongzuo工作环境不满意

            var service_benshen = leaving.Count(c => c.levelType == "服务族" && c.NotWork_Itself != 0);
            leave_record1.Add("service_benshen", service_benshen);//service_benshen工作本身不满意

            var service_wenhua = leaving.Count(c => c.levelType == "服务族" && c.NotCultural_Atmosphere != 0);
            leave_record1.Add("service_wenhua", service_wenhua);//service_wenhua文化氛围不满意

            //服务族个人离职原因
            var service_fazhang = leaving.Count(c => c.levelType == "服务族" && c.Better_development != 0);
            leave_record1.Add("service_fazhang", service_fazhang);//service_fazhang更好的发展

            var service_shenti = leaving.Count(c => c.levelType == "服务族" && c.HealthyBody != 0);
            leave_record1.Add("service_shenti", service_shenti);//service_shenti身体的健康

            var service_jiating = leaving.Count(c => c.levelType == "服务族" && c.Family_Reasons != 0);
            leave_record1.Add("service_jiating", service_jiating);//service_jiating家庭原因

            //试用期不合格离职原因
            var service_trial = leaving.Count(c => c.levelType == "服务族" && c.NotProbation_Period != 0);
            leave_record1.Add("service_trial", service_trial);//service_trial试用期不合格

            //协商解除的离职原因
            var service_remove = leaving.Count(c => c.levelType == "服务族" && c.Negotiate_Remove != 0);
            leave_record1.Add("service_remove", service_remove);//service_remove协商解除

            //其他原因
            var service_other = leaving.Count(c => c.levelType == "服务族" && c.Other_Reasons != 0);
            leave_record1.Add("service_other", service_other);//service_other其他原因

            #endregion

            #region--管理族离职原因
            //管理族离职原因（公司）
            var manage_couxin = leaving.Count(c => c.levelType == "管理族" && c.Pay_dissatisfaction != 0);
            leave_record1.Add("manage_couxin", manage_couxin);//manage_couxin薪酬不满意

            var manage_zhidu = leaving.Count(c => c.levelType == "管理族" && c.NotSystem != 0);
            leave_record1.Add("manage_zhidu", manage_zhidu);//manage_zhidu 制度不满意

            var manage_guanli = leaving.Count(c => c.levelType == "管理族" && c.Notmanagement != 0);
            leave_record1.Add("manage_guanli", manage_guanli);//manage_guanli管理不满意

            var manage_gongzuo = leaving.Count(c => c.levelType == "管理族" && c.Jobenvironment != 0);
            leave_record1.Add("manage_gongzuo", manage_gongzuo);//manage_gongzuo工作环境不满意

            var manage_benshen = leaving.Count(c => c.levelType == "管理族" && c.NotWork_Itself != 0);
            leave_record1.Add("manage_benshen", manage_benshen);//manage_benshen工作本身不满意

            var manage_wenhua = leaving.Count(c => c.levelType == "管理族" && c.NotCultural_Atmosphere != 0);
            leave_record1.Add("manage_wenhua", manage_wenhua);//manage_wenhua文化氛围不满意

            //管理族个人离职原因
            var manage_fazhang = leaving.Count(c => c.levelType == "管理族" && c.Better_development != 0);
            leave_record1.Add("manage_fazhang", manage_fazhang);//manage_fazhang更好的发展

            var manage_shenti = leaving.Count(c => c.levelType == "管理族" && c.HealthyBody != 0);
            leave_record1.Add("manage_shenti", manage_shenti);//manage_shenti身体的健康

            var manage_jiating = leaving.Count(c => c.levelType == "管理族" && c.Family_Reasons != 0);
            leave_record1.Add("manage_jiating", manage_jiating);//manage_jiating家庭原因

            //试用期不合格离职原因
            var manage_trial = leaving.Count(c => c.levelType == "管理族" && c.NotProbation_Period != 0);
            leave_record1.Add("manage_trial", manage_trial);//manage_trial试用期不合格

            //协商解除的离职原因
            var manage_remove = leaving.Count(c => c.levelType == "管理族" && c.Negotiate_Remove != 0);
            leave_record1.Add("manage_remove", manage_remove);//manage_remove协商解除

            //其他原因
            var manage_other = leaving.Count(c => c.levelType == "管理族" && c.Other_Reasons != 0);
            leave_record1.Add("manage_other", manage_other);//manage_other其他原因

            #endregion

            //公司离职原因小计
            var Pay_diss = leave_xinzi + tech_couxin + profe_couxin + service_couxin + manage_couxin;
            leave_record1.Add("pay_diss", Pay_diss);//薪酬部满意小计
            var System = leave_zhidu + tech_zhidu + profe_zhidu + service_zhidu + manage_zhidu;
            leave_record1.Add("system", System);//制度不满意小计
            var management = leave_guanli + tech_guanli + profe_guanli + service_guanli + manage_guanli;
            leave_record1.Add("management", management);//管理不满意小计
            var Joben = leave_huanji + tech_gongzuo + profe_gongzuo + service_gongzuo + manage_gongzuo;
            leave_record1.Add("joben", Joben);//工作环境不满意小计
            var Itself = leave_benshen + tech_benshen + profe_benshen + service_benshen + manage_benshen;
            leave_record1.Add("itself", Itself);//工作本身不满意小计
            var Cultural = leave_wenhua + tech_wenhua + profe_wenhua + service_wenhua + manage_wenhua;
            leave_record1.Add("cultural", Cultural);//文化氛围不满意小计

            //个人离职原因小计
            var Better = leave_genghao + tech_fazhang + profe_fazhang + service_fazhang + manage_fazhang;
            leave_record1.Add("better", Better);//更好的发展小计
            var Healthy = leave_shenti + tech_shenti + profe_shenti + service_shenti + manage_shenti;
            leave_record1.Add("healthy", Healthy);//身体健康小计
            var Family = leave_jiating + tech_jiating + profe_jiating + service_jiating + manage_jiating;
            leave_record1.Add("family", Family);//家庭原因小计

            //试用期不合格离职原因小计
            var Period = leave_trial + tech_trial + profe_trial + service_trial + manage_trial;
            leave_record1.Add("period", Period);

            //协商解除的离职原因小计
            var Remove = leave_remove + tech_remove + profe_remove + service_remove + manage_remove;
            leave_record1.Add("remove", Remove);

            //其他原因小计
            var Other = leave_other + tech_other + profe_other + service_other + manage_other;
            leave_record1.Add("other", Other);

            #region ---表2
            //操作族的离职个人原因小计
            var Operating_sum = leave_genghao + leave_shenti + leave_jiating;
            leave_record1.Add("Operating_sum", Operating_sum);
            //操作族的离职公司原因小计
            var Operating_sum1 = leave_xinzi + leave_zhidu + leave_guanli + leave_huanji + leave_benshen + leave_wenhua;
            leave_record1.Add("Operating_sum1", Operating_sum1);
            var Operating_sum2 = leave_trial;
            leave_record1.Add("Operating_sum2", Operating_sum2);
            var Operating_sum3 = leave_remove;
            leave_record1.Add("Operating_sum3", Operating_sum3);
            var Operating_sum4 = leave_other;
            leave_record1.Add("Operating_sum4", Operating_sum4);

            //技术族的离职原因小计
            var Technology_sum = tech_fazhang + tech_shenti + tech_jiating;
            leave_record1.Add("Technology_sum", Technology_sum);
            //技术族的离职公司原因小计
            var Technology_sum1 = tech_couxin + tech_zhidu + tech_guanli + tech_gongzuo + tech_benshen + tech_wenhua;
            leave_record1.Add("Technology_sum1", Technology_sum1);
            var Technology_sum2 = tech_trial;
            leave_record1.Add("Technology_sum2", Technology_sum2);
            var Technology_sum3 = tech_remove;
            leave_record1.Add("Technology_sum3", Technology_sum3);
            var Technology_sum4 = tech_other;
            leave_record1.Add("Technology_sum4", Technology_sum4);

            //专业族的离职原因小计
            var Professional_sum = profe_fazhang + profe_shenti + profe_jiating;
            leave_record1.Add("Professional_sum", Professional_sum);
            //专业族的离职公司原因小计
            var Professional_sum1 = profe_couxin + profe_zhidu + profe_guanli + profe_gongzuo + profe_benshen + profe_wenhua;
            leave_record1.Add("Professional_sum1", Professional_sum1);
            var Professional_sum2 = profe_trial;
            leave_record1.Add("Professional_sum2", Professional_sum2);
            var Professional_sum3 = profe_remove;
            leave_record1.Add("Professional_sum3", Professional_sum3);
            var Professional_sum4 = profe_other;
            leave_record1.Add("Professional_sum4", Professional_sum4);

            //服务族的离职原因小计
            var Service_sum = service_fazhang + service_shenti + service_jiating;
            leave_record1.Add("Service_sum", Service_sum);
            //服务族的离职公司原因小计
            var Service_sum1 = service_couxin + service_zhidu + service_guanli + service_gongzuo + service_benshen + service_wenhua;
            leave_record1.Add("Service_sum1", Service_sum1);
            var Service_sum2 = service_trial;
            leave_record1.Add("Service_sum2", Service_sum2);
            var Service_sum3 = service_remove;
            leave_record1.Add("Service_sum3", Service_sum3);
            var Service_sum4 = service_other;
            leave_record1.Add("Service_sum4", Service_sum4);

            //管理族的离职原因小计
            var Management_sum = manage_fazhang + manage_shenti + manage_jiating;
            leave_record1.Add("Management_sum", Management_sum);
            //管理族的离职公司原因小计
            var Management_sum1 = manage_couxin + manage_zhidu + manage_guanli + manage_gongzuo + manage_benshen + manage_wenhua;
            leave_record1.Add("Management_sum1", Management_sum1);
            var Management_sum2 = manage_trial;
            leave_record1.Add("Management_sum2", Management_sum2);
            var Management_sum3 = manage_remove;
            leave_record1.Add("Management_sum3", Management_sum3);
            var Management_sum4 = manage_other;
            leave_record1.Add("Management_sum4", Management_sum4);

            #endregion
            //合计
            var cause_heji = Pay_diss + System + management + Joben + Itself + Cultural + Better + Healthy + Family + Period + Remove + Other;
            leave_record1.Add("cause_heji", cause_heji);
            //个人原因总数
            var gern = Operating_sum + Technology_sum + Professional_sum + Service_sum + Management_sum;
            leave_record2.Add("gern", gern);
            //公司原因
            var gern1 = Operating_sum1 + Technology_sum1 + Professional_sum1 + Service_sum1 + Management_sum1;
            leave_record2.Add("gern1", gern1);
            var gern2 = Operating_sum2 + Technology_sum2 + Professional_sum2 + Service_sum2 + Management_sum2;
            leave_record2.Add("gern2", gern2);
            var gern3 = Operating_sum3 + Technology_sum3 + Professional_sum3 + Service_sum3 + Management_sum3;
            leave_record2.Add("gern3", gern3);
            var gern4 = Operating_sum4 + Technology_sum4 + Professional_sum4 + Service_sum4 + Management_sum4;
            leave_record2.Add("gern4", gern4);
            #region--- 操作族占比
            //操作族所有原因占比人数
            var Operating = Operating_sum1 + Operating_sum + Operating_sum2 + Operating_sum3 + Operating_sum4;
            //公司原因
            var proportion_couxin = (Operating == 0 ? 0 : Operating_sum1 * 100 / Operating).ToString("F2") + "%";//薪酬不满意
            leave_record2.Add("proportion_couxin", proportion_couxin);
            //个人原因
            var proportion_genghao = (Operating == 0 ? 0 : Operating_sum * 100 / Operating).ToString("F2") + "%";//更好的发展
            leave_record2.Add("proportion_genghao", proportion_genghao);
            var proportion_trial = (Operating == 0 ? 0 : Operating_sum2 * 100 / Operating).ToString("F2") + "%";//试用期不合格原因
            leave_record2.Add("proportion_trial", proportion_trial);
            var proportion_remove = (Operating == 0 ? 0 : Operating_sum3 * 100 / Operating).ToString("F2") + "%";//协商解除
            leave_record2.Add("proportion_remove", proportion_remove);
            var proportion_other = (Operating == 0 ? 0 : Operating_sum4 * 100 / Operating).ToString("F2") + "%";//其他原因
            leave_record2.Add("proportion_other", proportion_other);
            #endregion

            #region--- 技术族占比
            //技术族原因占比总人数
            var Technology = Technology_sum1 + Technology_sum + Technology_sum2 + Technology_sum3 + Technology_sum4;
            var Tec_couxin = (Technology == 0 ? 0 : Technology_sum1 * 100 / Technology).ToString("F2") + "%";//公司原因
            leave_record2.Add("tec_couxin", Tec_couxin);
            var Tec_genghao = (Technology == 0 ? 0 : Technology_sum * 100 / Technology).ToString("F2") + "%";//个人原因
            leave_record2.Add("tec_genghao", Tec_genghao);
            var Tec_trial = (Technology == 0 ? 0 : Technology_sum2 * 100 / Technology).ToString("F2") + "%";//试用期不合格原因
            leave_record2.Add("tec_trial", Tec_trial);
            var Tec_remove = (Technology == 0 ? 0 : Technology_sum3 * 100 / Technology).ToString("F2") + "%";//协商解除
            leave_record2.Add("tec_remove", Tec_remove);
            var Tec_other = (Technology == 0 ? 0 : Technology_sum4 * 100 / Technology).ToString("F2") + "%";//其他原因
            leave_record2.Add("tec_other", Tec_other);
            #endregion

            #region---专业族占比
            //专业族原因占比总人数
            var Professional = Professional_sum1 + Professional_sum + Professional_sum2 + Professional_sum3 + Professional_sum4;
            var Pro_couxin = (Professional == 0 ? 0 : Professional_sum1 * 100 / Professional).ToString("F2") + "%";//薪酬不满意
            leave_record2.Add("pro_couxin", Pro_couxin);
            var Pro_genghao = (Professional == 0 ? 0 : Professional_sum * 100 / Professional).ToString("F2") + "%";//更好的发展
            leave_record2.Add("pro_genghao", Pro_genghao);
            var Pro_trial = (Professional == 0 ? 0 : (Professional_sum2 * 100 / Professional)).ToString("F2") + "%";//试用期不合格原因
            leave_record2.Add("pro_trial", Pro_trial);
            var Pro_remove = (Professional == 0 ? 0 : (Professional_sum3 * 100 / Professional)).ToString("F2") + "%";//协商解除
            leave_record2.Add("pro_remove", Pro_remove);
            var Pro_other = (Professional == 0 ? 0 : (Professional_sum4 * 100 / Professional)).ToString("F2") + "%";//其他原因
            leave_record2.Add("pro_other", Pro_other);
            #endregion

            #region---服务族占比
            //服务族原因占比总人数
            var Service = Service_sum1 + Service_sum + Service_sum2 + Service_sum3 + Service_sum4;
            var Ser_couxin = (Service == 0 ? 0 : Service_sum1 * 100 / Service).ToString("F2") + "%";//薪酬不满意
            leave_record2.Add("ser_couxin", Ser_couxin);
            var Ser_genghao = (Service == 0 ? 0 : Service_sum * 100 / Service).ToString("F2") + "%";//更好的发展
            leave_record2.Add("ser_genghao", Ser_genghao);
            var Ser_trial = (Service == 0 ? 0 : (Service_sum2 * 100 / Service)).ToString("F2") + "%";//试用期不合格原因
            leave_record2.Add("ser_trial", Ser_trial);
            var Ser_remove = (Service == 0 ? 0 : (Service_sum3 * 100 / Service)).ToString("F2") + "%";//协商解除
            leave_record2.Add("ser_remove", Ser_remove);
            var Ser_other = (Service == 0 ? 0 : (Service_sum4 * 100 / Service)).ToString("F2") + "%";//其他原因
            leave_record2.Add("ser_other", Ser_other);
            #endregion

            #region---管理族占比
            //管理族原因占比总人数
            var Management = Management_sum1 + Management_sum + Management_sum2 + Management_sum3 + Management_sum4;
            //公司原因
            var Man_couxin = (Management == 0 ? 0 : Management_sum1 * 100 / Management).ToString("F2") + "%"; ;//薪酬不满意
            leave_record2.Add("man_couxin", Man_couxin);
            //个人原因
            var Man_genghao = (Management == 0 ? 0 : Management_sum * 100 / Management).ToString("F2") + "%";//更好的发展
            leave_record2.Add("man_genghao", Man_genghao);
            var Man_trial = (Management == 0 ? 0 : (Management_sum2 * 100 / Management)).ToString("F2") + "%";//试用期不合格原因
            leave_record2.Add("man_trial", Man_trial);
            var Man_remove = (Management == 0 ? 0 : (Management_sum3 * 100 / Management)).ToString("F2") + "%";//协商解除
            leave_record2.Add("man_remove", Man_remove);
            var Man_other = (Management == 0 ? 0 : (Management_sum4 * 100 / Management)).ToString("F2") + "%";//其他原因
            leave_record2.Add("man_other", Man_other);
            #endregion


            //离职原因占比
            var Leaving_salary = ((cause_heji == 0 ? 0 : (Pay_diss * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (System * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (management * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (Joben * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (Itself * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (Cultural * 100 / cause_heji))).ToString("F2") + "%";//薪酬不满意
            leave_record2.Add("leaving_salary", Leaving_salary);
            var Leaving_Better = ((cause_heji == 0 ? 0 : (Better * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (Healthy * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (Family * 100 / cause_heji))).ToString("F2") + "%";//更好的发展
            leave_record2.Add("leaving_Better", Leaving_Better);
            var Leaving_Period = (cause_heji == 0 ? 0 : (Period * 100 / cause_heji)).ToString("F2") + "%";//试用期不合格
            leave_record2.Add("leaving_Period", Leaving_Period);
            var Leaving_Remove = (cause_heji == 0 ? 0 : (Remove * 100 / cause_heji)).ToString("F2") + "%";//协商解除
            leave_record2.Add("leaving_Remove", Leaving_Remove);
            var Leaving_Other = (cause_heji == 0 ? 0 : (Other * 100 / cause_heji)).ToString("F2") + "%";//其他原因
            leave_record2.Add("leaving_Other", Leaving_Other);

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
            var leave = db.Personnel_Reasons_for_leaving.Where(c => c.Department_leaving == department && c.LastDate.Value.Year == year && c.LastDate.Value.Month == mouth).ToList();
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
                if (item.Resign != 0)//辞职
                {
                    //离退类型：辞职
                    userItem.Add("Resign", 1);
                }
                else if (item.SinceFrom != 0)//自离
                {
                    //离退类型：自离
                    userItem.Add("Resign", 2);
                }
                else if (item.LayOff != 0)//辞退
                {
                    //离退类型：辞退
                    userItem.Add("Resign", 3);
                }
                else if (item.Retired != 0)//退休
                {
                    //离退类型：退休
                    userItem.Add("Resign", 4);
                }
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
                //工作本身不满意
                userItem.Add("NotWork_Itself", item.NotWork_Itself);
                //文化氛围不满意
                userItem.Add("NotCultural_Atmosphere", item.NotCultural_Atmosphere);
                //更好的发展Better_development
                userItem.Add("Better_development", item.Better_development);
                //身体健康原因
                userItem.Add("HealthyBody", item.HealthyBody);
                //家庭原因
                userItem.Add("Family_Reasons", item.Family_Reasons);
                //试用期不合格
                userItem.Add("NotProbation_Period", item.NotProbation_Period);
                //协商解除
                userItem.Add("Negotiate_Remove", item.Negotiate_Remove);
                //其他原因
                userItem.Add("Other_Reasons", item.Other_Reasons);

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


        #region ---修改方法
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
        public ActionResult Create([Bind(Include = "Id,Name,jobNum,Department_leaving,DP_group,Position,levelType,HireDate,LastDate,Remark,Department_of_conclusion,HR_conclusion,Pay_dissatisfaction,NotSystem,Notmanagement,Jobenvironment,NotWork_Itself,NotCultural_Atmosphere,Better_development,HealthyBody,Family_Reasons,NotProbation_Period,Negotiate_Remove,Other_Reasons,Resign,SinceFrom,LayOff,Retired,Creator,CreateDate")] Personnel_Reasons_for_leaving personnel_Reasons_for_leaving)
        {
            //personnel_Reasons_for_leaving.CreateDate = DateTime.Now;
            //    personnel_Reasons_for_leaving.Creator = ((Users)Session["User"]).UserName;

            //db.Entry(personnel_Reasons_for_leaving).State = EntityState.Added;
            if (personnel_Reasons_for_leaving.Pay_dissatisfaction != 0 || personnel_Reasons_for_leaving.NotSystem != 0 || personnel_Reasons_for_leaving.Notmanagement != 0 || personnel_Reasons_for_leaving.Jobenvironment != 0 || personnel_Reasons_for_leaving.Better_development != 0 || personnel_Reasons_for_leaving.NotProbation_Period != 0 || personnel_Reasons_for_leaving.NotWork_Itself != 0 || personnel_Reasons_for_leaving.NotCultural_Atmosphere != 0 || personnel_Reasons_for_leaving.Negotiate_Remove != 0 || personnel_Reasons_for_leaving.Family_Reasons != 0 || personnel_Reasons_for_leaving.HealthyBody != 0 || personnel_Reasons_for_leaving.Other_Reasons != 0)
            {

                var amend = db.Personnel_Reasons_for_leaving.Where(c => c.jobNum == personnel_Reasons_for_leaving.jobNum && c.Name == personnel_Reasons_for_leaving.Name).FirstOrDefault();
                if (amend != null)
                {
                    amend.Department_leaving = personnel_Reasons_for_leaving.Department_leaving;
                    amend.levelType = personnel_Reasons_for_leaving.levelType;
                    amend.Pay_dissatisfaction = personnel_Reasons_for_leaving.Pay_dissatisfaction;
                    amend.NotSystem = personnel_Reasons_for_leaving.NotSystem;
                    amend.Notmanagement = personnel_Reasons_for_leaving.Notmanagement;
                    amend.Jobenvironment = personnel_Reasons_for_leaving.Jobenvironment;
                    amend.Better_development = personnel_Reasons_for_leaving.Better_development;
                    amend.NotWork_Itself = personnel_Reasons_for_leaving.NotWork_Itself;
                    amend.NotCultural_Atmosphere = personnel_Reasons_for_leaving.NotCultural_Atmosphere;
                    amend.HealthyBody = personnel_Reasons_for_leaving.HealthyBody;
                    amend.Family_Reasons = personnel_Reasons_for_leaving.Family_Reasons;
                    amend.NotProbation_Period = personnel_Reasons_for_leaving.NotProbation_Period;
                    amend.Negotiate_Remove = personnel_Reasons_for_leaving.Negotiate_Remove;
                    amend.Other_Reasons = personnel_Reasons_for_leaving.Other_Reasons;
                    if (personnel_Reasons_for_leaving.Resign == 1)//辞职
                    {
                        amend.Resign = 1;
                        amend.Retired = 0;
                        amend.LayOff = 0;
                        amend.SinceFrom = 0;
                    }
                    else if (personnel_Reasons_for_leaving.Resign == 2)//自离
                    {
                        amend.Resign = 0;
                        amend.Retired = 0;
                        amend.LayOff = 0;
                        amend.SinceFrom = 1;
                    }
                    else if (personnel_Reasons_for_leaving.Resign == 3)//辞退
                    {
                        amend.Resign = 0;
                        amend.Retired = 0;
                        amend.LayOff = 1;
                        amend.SinceFrom = 0;
                    }
                    else if (personnel_Reasons_for_leaving.Resign == 4)//退休
                    {
                        amend.Resign = 0;
                        amend.Retired = 1;
                        amend.LayOff = 0;
                        amend.SinceFrom = 0;
                    }
                    amend.Remark = personnel_Reasons_for_leaving.Remark;

                    db.SaveChanges();
                    return Content("ok");
                }
                if (ModelState.IsValid)
                {
                    if (personnel_Reasons_for_leaving.Resign == 1)//辞职
                    {
                        personnel_Reasons_for_leaving.Resign = 1;
                        personnel_Reasons_for_leaving.Retired = 0;
                        personnel_Reasons_for_leaving.LayOff = 0;
                        personnel_Reasons_for_leaving.SinceFrom = 0;
                        db.SaveChanges();

                    }
                    else if (personnel_Reasons_for_leaving.Resign == 2)//自离
                    {
                        personnel_Reasons_for_leaving.Resign = 0;
                        personnel_Reasons_for_leaving.Retired = 0;
                        personnel_Reasons_for_leaving.LayOff = 0;
                        personnel_Reasons_for_leaving.SinceFrom = 1;
                        db.SaveChanges();

                    }
                    else if (personnel_Reasons_for_leaving.Resign == 3)//辞退
                    {
                        personnel_Reasons_for_leaving.Resign = 0;
                        personnel_Reasons_for_leaving.Retired = 0;
                        personnel_Reasons_for_leaving.LayOff = 1;
                        personnel_Reasons_for_leaving.SinceFrom = 0;
                        db.SaveChanges();

                    }
                    else if (personnel_Reasons_for_leaving.Resign == 4)//退休
                    {
                        personnel_Reasons_for_leaving.Resign = 0;
                        personnel_Reasons_for_leaving.Retired = 1;
                        personnel_Reasons_for_leaving.LayOff = 0;
                        personnel_Reasons_for_leaving.SinceFrom = 0;
                        db.SaveChanges();

                    }
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
                item.CreateDate = DateTime.Now;
                item.Creator = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                if (db.Personnel_Reasons_for_leaving.Count(c => c.jobNum == item.jobNum) != 0)
                    repeat = repeat + item.jobNum + ",";
            }
            if (!string.IsNullOrEmpty(repeat))
            {
                return Content(repeat);
            }
            if (ModelState.IsValid)
            {
                foreach (var it in inputList)
                {
                    if (it.Resign == 1)//辞职
                    {
                        it.Resign = 1;
                        it.Retired = 0;
                        it.LayOff = 0;
                        it.SinceFrom = 0;
                        db.SaveChanges();
                    }
                    else if (it.Resign == 2)//自离
                    {
                        it.Resign = 0;
                        it.Retired = 0;
                        it.LayOff = 0;
                        it.SinceFrom = 1;
                        db.SaveChanges();
                    }
                    else if (it.Resign == 3)//辞退
                    {
                        it.Resign = 0;
                        it.Retired = 0;
                        it.LayOff = 1;
                        it.SinceFrom = 0;
                        db.SaveChanges();
                    }
                    else if (it.Resign == 4)//退休
                    {
                        it.Resign = 0;
                        it.Retired = 1;
                        it.LayOff = 0;
                        it.SinceFrom = 0;
                        db.SaveChanges();
                    }
                }
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
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,jobNum,Department_leaving,DP_group,Position,levelType,HireDate,LastDate,Remark,Department_of_conclusion,HR_conclusion,Pay_dissatisfaction,NotSystem,Notmanagement,Jobenvironment,NotWork_Itself,NotCultural_Atmosphere,Better_development,HealthyBody,Family_Reasons,NotProbation_Period,Negotiate_Remove,Other_Reasons,Resign,SinceFrom,LayOff,Retired,Creator,CreateDate")] Personnel_Reasons_for_leaving personnel_Reasons_for_leaving)
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
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;//添加删除操作时间
            operaterecord.Operator = ((Users)Session["User"]).UserName;//添加删除操作人
            //添加操作记录
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除离职原因记录工号为" + personnel_Reasons_for_leaving.jobNum + "数据记录";
            db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
            db.Personnel_Reasons_for_leaving.Remove(personnel_Reasons_for_leaving);
            int count = db.SaveChanges();
            if (count > 0)
            {
                return Content("true");
            }
            else
            {
                return Content("false");
            }
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

    //Api接口部分

    public class Personnel_Reasons_for_leaving_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController common = new CommonController();


        #region------离职情况汇总调差表查看
        [HttpPost]
        [ApiAuthorize]
        public JObject For_leaving([System.Web.Http.FromBody]JObject data)
        {
            JObject for_leaving_Table = new JObject();
            JObject Department_record = new JObject();
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int? Year = obj.Year;//年
            int? Month = obj.Month;//月
            var Year_Month_Allrecord = db.Personnel_daily.ToList();
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
                return common.GetModuleFromJobjet(result, false, "没有记录!");
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
            int benshen = 0;//工作本身不满意
            int wenhua = 0;//文化氛围不满意
            int shenti = 0;//身体的健康
            int jiating = 0;//家庭原因
            int proba = 0;//试用期不合格
            int tiate = 0;//协商解除
            int reasons = 0;//其他原因
            int resign = 0;//离退类型：辞职
            int since = 0;//离退类型：自离
            int lay = 0;//离退类型：辞职
            int retied = 0;//离退类型：退休
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

                //离退类型汇总

                var Types_cizhi = leaving.Count(c => c.Resign == 1 && c.Department_leaving == department);
                resign = resign + Types_cizhi;
                Department_record.Add("types_cizhi", Types_cizhi);//辞职

                var Types_zili = leaving.Count(c => c.SinceFrom == 1 && c.Department_leaving == department);
                since = since + Types_zili;
                Department_record.Add("types_zili", Types_zili);//自离

                var Types_citui = leaving.Count(c => c.LayOff == 1 && c.Department_leaving == department);
                lay = lay + Types_citui;
                Department_record.Add("types_citui", Types_citui);//辞退

                var Types_tuixiu = leaving.Count(c => c.Retired == 1 && c.Department_leaving == department);
                retied = retied + Types_tuixiu;
                Department_record.Add("types_tuixiu", Types_tuixiu);//退休

                //离职原因汇总
                var py_couxin = leaving.Count(c => c.Pay_dissatisfaction == 1 && c.Department_leaving == department);
                couxin = couxin + py_couxin;
                Department_record.Add("py_couxin", py_couxin);//薪酬不满意

                var py_zhidu = leaving.Count(c => c.NotSystem == 1 && c.Department_leaving == department);
                zhidu = zhidu + py_zhidu;
                Department_record.Add("py_zhidu", py_zhidu);//制度不满意

                var py_guan = leaving.Count(c => c.Notmanagement == 1 && c.Department_leaving == department);
                guan = guan + py_guan;
                Department_record.Add("py_guan", py_guan);//管理不满意

                var py_gong = leaving.Count(c => c.Jobenvironment == 1 && c.Department_leaving == department);
                gong = gong + py_gong;
                Department_record.Add("py_gong", py_gong);//工作环境不满意

                var py_benshen = leaving.Count(c => c.NotWork_Itself == 1 && c.Department_leaving == department);
                benshen = benshen + py_benshen;
                Department_record.Add("py_benshen", py_benshen);//工作本身不满意

                var py_wenhua = leaving.Count(c => c.NotCultural_Atmosphere == 1 && c.Department_leaving == department);
                wenhua = wenhua + py_wenhua;
                Department_record.Add("py_wenhua", py_wenhua);//文化氛围不满意

                var py_geng = leaving.Count(c => c.Better_development == 1 && c.Department_leaving == department);
                geng = geng + py_geng;
                Department_record.Add("py_geng", py_geng);//更好的发展

                var py_shenti = leaving.Count(c => c.HealthyBody == 1 && c.Department_leaving == department);
                shenti = shenti + py_shenti;
                Department_record.Add("py_shenti", py_shenti);//身体的健康

                var py_jiating = leaving.Count(c => c.Family_Reasons == 1 && c.Department_leaving == department);
                jiating = jiating + py_jiating;
                Department_record.Add("py_jiating", py_jiating);//家庭原因

                var py_proba = leaving.Count(c => c.NotProbation_Period == 1 && c.Department_leaving == department);
                proba = proba + py_proba;
                Department_record.Add("py_proba", py_proba);//试用期不合格

                var py_tiate = leaving.Count(c => c.Negotiate_Remove == 1 && c.Department_leaving == department);
                tiate = tiate + py_tiate;
                Department_record.Add("py_tiate", py_tiate);//协商解除

                var py_reasons = leaving.Count(c => c.Other_Reasons == 1 && c.Department_leaving == department);
                reasons = reasons + py_reasons;
                Department_record.Add("py_reasons", py_reasons);//其他原因


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

            #region--操作族离职原因
            //操作族公司离职原因
            var leave_xinzi = leaving.Count(c => c.levelType == "操作族" && c.Pay_dissatisfaction != 0);
            leave_record1.Add("leave_xinzi", leave_xinzi);//leave_xinzi薪酬不满意

            var leave_zhidu = leaving.Count(c => c.levelType == "操作族" && c.NotSystem != 0);
            leave_record1.Add("leave_zhidu", leave_zhidu);// leave_zhidu制度不满意

            var leave_guanli = leaving.Count(c => c.levelType == "操作族" && c.Notmanagement != 0);
            leave_record1.Add("leave_guanli", leave_guanli);//leave_guanli管理不满意

            var leave_huanji = leaving.Count(c => c.levelType == "操作族" && c.Jobenvironment != 0);
            leave_record1.Add("leave_huanji", leave_huanji);//leave_huanji工作环境不满意

            var leave_benshen = leaving.Count(c => c.levelType == "操作族" && c.NotWork_Itself != 0);
            leave_record1.Add("leave_benshen", leave_benshen);//leave_benshen工作本身不满意

            var leave_wenhua = leaving.Count(c => c.levelType == "操作族" && c.NotCultural_Atmosphere != 0);
            leave_record1.Add("leave_wenhua", leave_wenhua);//leave_wenhua文化氛围不满意

            //操作族个人离职原因
            var leave_genghao = leaving.Count(c => c.levelType == "操作族" && c.Better_development != 0);
            leave_record1.Add("leave_genghao", leave_genghao);// leave_genghao更好的发展

            var leave_shenti = leaving.Count(c => c.levelType == "操作族" && c.HealthyBody != 0);
            leave_record1.Add("leave_shenti", leave_shenti);//leave_shenti身体的健康

            var leave_jiating = leaving.Count(c => c.levelType == "操作族" && c.Family_Reasons != 0);
            leave_record1.Add("leave_jiating", leave_jiating);//leave_jiating家庭原因

            //试用期不合格离职原因
            var leave_trial = leaving.Count(c => c.levelType == "操作族" && c.NotProbation_Period != 0);
            leave_record1.Add("leave_trial", leave_trial);//leave_trial试用期不合格

            //协商解除的离职原因
            var leave_remove = leaving.Count(c => c.levelType == "操作族" && c.Negotiate_Remove != 0);
            leave_record1.Add("leave_remove", leave_remove);//leave_remove协商解除

            //其他原因
            var leave_other = leaving.Count(c => c.levelType == "操作族" && c.Other_Reasons != 0);
            leave_record1.Add("leave_other", leave_other);//leave_other其他原因

            #endregion

            #region--技术族离职原因
            //技术族离职原因（公司）
            var tech_couxin = leaving.Count(c => c.levelType == "技术族" && c.Pay_dissatisfaction != 0);
            leave_record1.Add("tech_couxin", tech_couxin);//tech_couxin薪酬不满意

            var tech_zhidu = leaving.Count(c => c.levelType == "技术族" && c.NotSystem != 0);
            leave_record1.Add("tech_zhidu", tech_zhidu);//tech_zhidu 制度不满意

            var tech_guanli = leaving.Count(c => c.levelType == "技术族" && c.Notmanagement != 0);
            leave_record1.Add("tech_guanli", tech_guanli);//tech_guanli管理不满意

            var tech_gongzuo = leaving.Count(c => c.levelType == "技术族" && c.Jobenvironment != 0);
            leave_record1.Add("tech_gongzuo", tech_gongzuo);//tech_gongzuo工作环境不满意

            var tech_benshen = leaving.Count(c => c.levelType == "技术族" && c.NotWork_Itself != 0);
            leave_record1.Add("tech_benshen", tech_benshen);//tech_benshen工作本身不满意

            var tech_wenhua = leaving.Count(c => c.levelType == "技术族" && c.NotCultural_Atmosphere != 0);
            leave_record1.Add("tech_wenhua", tech_wenhua);//tech_wenhua文化氛围不满意

            //技术族个人离职原因
            var tech_fazhang = leaving.Count(c => c.levelType == "技术族" && c.Better_development != 0);
            leave_record1.Add("tech_fazhang", tech_fazhang);//tech_fazhang更好的发展

            var tech_shenti = leaving.Count(c => c.levelType == "技术族" && c.HealthyBody != 0);
            leave_record1.Add("tech_shenti", tech_shenti);//tech_shenti身体的健康

            var tech_jiating = leaving.Count(c => c.levelType == "技术族" && c.Family_Reasons != 0);
            leave_record1.Add("tech_jiating", tech_jiating);//tech_jiating家庭原因

            //试用期不合格离职原因
            var tech_trial = leaving.Count(c => c.levelType == "技术族" && c.NotProbation_Period != 0);
            leave_record1.Add("tech_trial", tech_trial);//tech_trial试用期不合格

            //协商解除的离职原因
            var tech_remove = leaving.Count(c => c.levelType == "技术族" && c.Negotiate_Remove != 0);
            leave_record1.Add("tech_remove", tech_remove);//tech_remove协商解除

            //其他原因
            var tech_other = leaving.Count(c => c.levelType == "技术族" && c.Other_Reasons != 0);
            leave_record1.Add("tech_other", tech_other);//tech_other其他原因

            #endregion

            #region--专业族离职原因
            //专业族离职原因（公司）
            var profe_couxin = leaving.Count(c => c.levelType == "专业族" && c.Pay_dissatisfaction != 0);
            leave_record1.Add("profe_couxin", profe_couxin);//profe_couxin薪酬不满意

            var profe_zhidu = leaving.Count(c => c.levelType == "专业族" && c.NotSystem != 0);
            leave_record1.Add("profe_zhidu", profe_zhidu);//profe_zhidu 制度不满意

            var profe_guanli = leaving.Count(c => c.levelType == "专业族" && c.Notmanagement != 0);
            leave_record1.Add("profe_guanli", profe_guanli);//profe_guanli管理不满意

            var profe_gongzuo = leaving.Count(c => c.levelType == "专业族" && c.Jobenvironment != 0);
            leave_record1.Add("profe_gongzuo", profe_gongzuo);//profe_gongzuo工作环境不满意

            var profe_benshen = leaving.Count(c => c.levelType == "专业族" && c.NotWork_Itself != 0);
            leave_record1.Add("profe_benshen", profe_benshen);//profe_benshen工作本身不满意

            var profe_wenhua = leaving.Count(c => c.levelType == "专业族" && c.NotCultural_Atmosphere != 0);
            leave_record1.Add("profe_wenhua", profe_wenhua);//profe_wenhua文化氛围不满意

            //专业族个人离职原因
            var profe_fazhang = leaving.Count(c => c.levelType == "专业族" && c.Better_development != 0);
            leave_record1.Add("cause_fazhang", profe_fazhang);//profe_fazhang更好的发展

            var profe_shenti = leaving.Count(c => c.levelType == "专业族" && c.HealthyBody != 0);
            leave_record1.Add("profe_shenti", profe_shenti);//profe_shenti身体的健康

            var profe_jiating = leaving.Count(c => c.levelType == "专业族" && c.Family_Reasons != 0);
            leave_record1.Add("profe_jiating", profe_jiating);//profe_jiating家庭原因

            //试用期不合格离职原因
            var profe_trial = leaving.Count(c => c.levelType == "专业族" && c.NotProbation_Period != 0);
            leave_record1.Add("profe_trial", profe_trial);//profe_trial试用期不合格

            //协商解除的离职原因
            var profe_remove = leaving.Count(c => c.levelType == "专业族" && c.Negotiate_Remove != 0);
            leave_record1.Add("profe_remove", profe_remove);//profe_remove协商解除

            //其他原因
            var profe_other = leaving.Count(c => c.levelType == "专业族" && c.Other_Reasons != 0);
            leave_record1.Add("profe_other", profe_other);//profe_other其他原因

            #endregion

            #region--服务族离职原因
            //服务族离职原因（公司）
            var service_couxin = leaving.Count(c => c.levelType == "服务族" && c.Pay_dissatisfaction != 0);
            leave_record1.Add("service_couxin", service_couxin);//service_couxin薪酬不满意

            var service_zhidu = leaving.Count(c => c.levelType == "服务族" && c.NotSystem != 0);
            leave_record1.Add("service_zhidu", service_zhidu);//service_zhidu 制度不满意

            var service_guanli = leaving.Count(c => c.levelType == "服务族" && c.Notmanagement != 0);
            leave_record1.Add("service_guanli", service_guanli);//service_guanli管理不满意

            var service_gongzuo = leaving.Count(c => c.levelType == "服务族" && c.Jobenvironment != 0);
            leave_record1.Add("service_gongzuo", service_gongzuo);//service_gongzuo工作环境不满意

            var service_benshen = leaving.Count(c => c.levelType == "服务族" && c.NotWork_Itself != 0);
            leave_record1.Add("service_benshen", service_benshen);//service_benshen工作本身不满意

            var service_wenhua = leaving.Count(c => c.levelType == "服务族" && c.NotCultural_Atmosphere != 0);
            leave_record1.Add("service_wenhua", service_wenhua);//service_wenhua文化氛围不满意

            //服务族个人离职原因
            var service_fazhang = leaving.Count(c => c.levelType == "服务族" && c.Better_development != 0);
            leave_record1.Add("service_fazhang", service_fazhang);//service_fazhang更好的发展

            var service_shenti = leaving.Count(c => c.levelType == "服务族" && c.HealthyBody != 0);
            leave_record1.Add("service_shenti", service_shenti);//service_shenti身体的健康

            var service_jiating = leaving.Count(c => c.levelType == "服务族" && c.Family_Reasons != 0);
            leave_record1.Add("service_jiating", service_jiating);//service_jiating家庭原因

            //试用期不合格离职原因
            var service_trial = leaving.Count(c => c.levelType == "服务族" && c.NotProbation_Period != 0);
            leave_record1.Add("service_trial", service_trial);//service_trial试用期不合格

            //协商解除的离职原因
            var service_remove = leaving.Count(c => c.levelType == "服务族" && c.Negotiate_Remove != 0);
            leave_record1.Add("service_remove", service_remove);//service_remove协商解除

            //其他原因
            var service_other = leaving.Count(c => c.levelType == "服务族" && c.Other_Reasons != 0);
            leave_record1.Add("service_other", service_other);//service_other其他原因

            #endregion

            #region--管理族离职原因
            //管理族离职原因（公司）
            var manage_couxin = leaving.Count(c => c.levelType == "管理族" && c.Pay_dissatisfaction != 0);
            leave_record1.Add("manage_couxin", manage_couxin);//manage_couxin薪酬不满意

            var manage_zhidu = leaving.Count(c => c.levelType == "管理族" && c.NotSystem != 0);
            leave_record1.Add("manage_zhidu", manage_zhidu);//manage_zhidu 制度不满意

            var manage_guanli = leaving.Count(c => c.levelType == "管理族" && c.Notmanagement != 0);
            leave_record1.Add("manage_guanli", manage_guanli);//manage_guanli管理不满意

            var manage_gongzuo = leaving.Count(c => c.levelType == "管理族" && c.Jobenvironment != 0);
            leave_record1.Add("manage_gongzuo", manage_gongzuo);//manage_gongzuo工作环境不满意

            var manage_benshen = leaving.Count(c => c.levelType == "管理族" && c.NotWork_Itself != 0);
            leave_record1.Add("manage_benshen", manage_benshen);//manage_benshen工作本身不满意

            var manage_wenhua = leaving.Count(c => c.levelType == "管理族" && c.NotCultural_Atmosphere != 0);
            leave_record1.Add("manage_wenhua", manage_wenhua);//manage_wenhua文化氛围不满意

            //管理族个人离职原因
            var manage_fazhang = leaving.Count(c => c.levelType == "管理族" && c.Better_development != 0);
            leave_record1.Add("manage_fazhang", manage_fazhang);//manage_fazhang更好的发展

            var manage_shenti = leaving.Count(c => c.levelType == "管理族" && c.HealthyBody != 0);
            leave_record1.Add("manage_shenti", manage_shenti);//manage_shenti身体的健康

            var manage_jiating = leaving.Count(c => c.levelType == "管理族" && c.Family_Reasons != 0);
            leave_record1.Add("manage_jiating", manage_jiating);//manage_jiating家庭原因

            //试用期不合格离职原因
            var manage_trial = leaving.Count(c => c.levelType == "管理族" && c.NotProbation_Period != 0);
            leave_record1.Add("manage_trial", manage_trial);//manage_trial试用期不合格

            //协商解除的离职原因
            var manage_remove = leaving.Count(c => c.levelType == "管理族" && c.Negotiate_Remove != 0);
            leave_record1.Add("manage_remove", manage_remove);//manage_remove协商解除

            //其他原因
            var manage_other = leaving.Count(c => c.levelType == "管理族" && c.Other_Reasons != 0);
            leave_record1.Add("manage_other", manage_other);//manage_other其他原因

            #endregion

            //公司离职原因小计
            var Pay_diss = leave_xinzi + tech_couxin + profe_couxin + service_couxin + manage_couxin;
            leave_record1.Add("pay_diss", Pay_diss);//薪酬部满意小计
            var System = leave_zhidu + tech_zhidu + profe_zhidu + service_zhidu + manage_zhidu;
            leave_record1.Add("system", System);//制度不满意小计
            var management = leave_guanli + tech_guanli + profe_guanli + service_guanli + manage_guanli;
            leave_record1.Add("management", management);//管理不满意小计
            var Joben = leave_huanji + tech_gongzuo + profe_gongzuo + service_gongzuo + manage_gongzuo;
            leave_record1.Add("joben", Joben);//工作环境不满意小计
            var Itself = leave_benshen + tech_benshen + profe_benshen + service_benshen + manage_benshen;
            leave_record1.Add("itself", Itself);//工作本身不满意小计
            var Cultural = leave_wenhua + tech_wenhua + profe_wenhua + service_wenhua + manage_wenhua;
            leave_record1.Add("cultural", Cultural);//文化氛围不满意小计

            //个人离职原因小计
            var Better = leave_genghao + tech_fazhang + profe_fazhang + service_fazhang + manage_fazhang;
            leave_record1.Add("better", Better);//更好的发展小计
            var Healthy = leave_shenti + tech_shenti + profe_shenti + service_shenti + manage_shenti;
            leave_record1.Add("healthy", Healthy);//身体健康小计
            var Family = leave_jiating + tech_jiating + profe_jiating + service_jiating + manage_jiating;
            leave_record1.Add("family", Family);//家庭原因小计

            //试用期不合格离职原因小计
            var Period = leave_trial + tech_trial + profe_trial + service_trial + manage_trial;
            leave_record1.Add("period", Period);

            //协商解除的离职原因小计
            var Remove = leave_remove + tech_remove + profe_remove + service_remove + manage_remove;
            leave_record1.Add("remove", Remove);

            //其他原因小计
            var Other = leave_other + tech_other + profe_other + service_other + manage_other;
            leave_record1.Add("other", Other);

            #region ---表2
            //操作族的离职个人原因小计
            var Operating_sum = leave_genghao + leave_shenti + leave_jiating;
            leave_record1.Add("Operating_sum", Operating_sum);
            //操作族的离职公司原因小计
            var Operating_sum1 = leave_xinzi + leave_zhidu + leave_guanli + leave_huanji + leave_benshen + leave_wenhua;
            leave_record1.Add("Operating_sum1", Operating_sum1);
            var Operating_sum2 = leave_trial;
            leave_record1.Add("Operating_sum2", Operating_sum2);
            var Operating_sum3 = leave_remove;
            leave_record1.Add("Operating_sum3", Operating_sum3);
            var Operating_sum4 = leave_other;
            leave_record1.Add("Operating_sum4", Operating_sum4);

            //技术族的离职原因小计
            var Technology_sum = tech_fazhang + tech_shenti + tech_jiating;
            leave_record1.Add("Technology_sum", Technology_sum);
            //技术族的离职公司原因小计
            var Technology_sum1 = tech_couxin + tech_zhidu + tech_guanli + tech_gongzuo + tech_benshen + tech_wenhua;
            leave_record1.Add("Technology_sum1", Technology_sum1);
            var Technology_sum2 = tech_trial;
            leave_record1.Add("Technology_sum2", Technology_sum2);
            var Technology_sum3 = tech_remove;
            leave_record1.Add("Technology_sum3", Technology_sum3);
            var Technology_sum4 = tech_other;
            leave_record1.Add("Technology_sum4", Technology_sum4);

            //专业族的离职原因小计
            var Professional_sum = profe_fazhang + profe_shenti + profe_jiating;
            leave_record1.Add("Professional_sum", Professional_sum);
            //专业族的离职公司原因小计
            var Professional_sum1 = profe_couxin + profe_zhidu + profe_guanli + profe_gongzuo + profe_benshen + profe_wenhua;
            leave_record1.Add("Professional_sum1", Professional_sum1);
            var Professional_sum2 = profe_trial;
            leave_record1.Add("Professional_sum2", Professional_sum2);
            var Professional_sum3 = profe_remove;
            leave_record1.Add("Professional_sum3", Professional_sum3);
            var Professional_sum4 = profe_other;
            leave_record1.Add("Professional_sum4", Professional_sum4);

            //服务族的离职原因小计
            var Service_sum = service_fazhang + service_shenti + service_jiating;
            leave_record1.Add("Service_sum", Service_sum);
            //服务族的离职公司原因小计
            var Service_sum1 = service_couxin + service_zhidu + service_guanli + service_gongzuo + service_benshen + service_wenhua;
            leave_record1.Add("Service_sum1", Service_sum1);
            var Service_sum2 = service_trial;
            leave_record1.Add("Service_sum2", Service_sum2);
            var Service_sum3 = service_remove;
            leave_record1.Add("Service_sum3", Service_sum3);
            var Service_sum4 = service_other;
            leave_record1.Add("Service_sum4", Service_sum4);

            //管理族的离职原因小计
            var Management_sum = manage_fazhang + manage_shenti + manage_jiating;
            leave_record1.Add("Management_sum", Management_sum);
            //管理族的离职公司原因小计
            var Management_sum1 = manage_couxin + manage_zhidu + manage_guanli + manage_gongzuo + manage_benshen + manage_wenhua;
            leave_record1.Add("Management_sum1", Management_sum1);
            var Management_sum2 = manage_trial;
            leave_record1.Add("Management_sum2", Management_sum2);
            var Management_sum3 = manage_remove;
            leave_record1.Add("Management_sum3", Management_sum3);
            var Management_sum4 = manage_other;
            leave_record1.Add("Management_sum4", Management_sum4);

            #endregion
            //合计
            var cause_heji = Pay_diss + System + management + Joben + Itself + Cultural + Better + Healthy + Family + Period + Remove + Other;
            leave_record1.Add("cause_heji", cause_heji);
            //个人原因总数
            var gern = Operating_sum + Technology_sum + Professional_sum + Service_sum + Management_sum;
            leave_record2.Add("gern", gern);
            //公司原因
            var gern1 = Operating_sum1 + Technology_sum1 + Professional_sum1 + Service_sum1 + Management_sum1;
            leave_record2.Add("gern1", gern1);
            var gern2 = Operating_sum2 + Technology_sum2 + Professional_sum2 + Service_sum2 + Management_sum2;
            leave_record2.Add("gern2", gern2);
            var gern3 = Operating_sum3 + Technology_sum3 + Professional_sum3 + Service_sum3 + Management_sum3;
            leave_record2.Add("gern3", gern3);
            var gern4 = Operating_sum4 + Technology_sum4 + Professional_sum4 + Service_sum4 + Management_sum4;
            leave_record2.Add("gern4", gern4);
            #region--- 操作族占比
            //操作族所有原因占比人数
            var Operating = Operating_sum1 + Operating_sum + Operating_sum2 + Operating_sum3 + Operating_sum4;
            //公司原因
            var proportion_couxin = (Operating == 0 ? 0 : Operating_sum1 * 100 / Operating).ToString("F2") + "%";//薪酬不满意
            leave_record2.Add("proportion_couxin", proportion_couxin);
            //个人原因
            var proportion_genghao = (Operating == 0 ? 0 : Operating_sum * 100 / Operating).ToString("F2") + "%";//更好的发展
            leave_record2.Add("proportion_genghao", proportion_genghao);
            var proportion_trial = (Operating == 0 ? 0 : Operating_sum2 * 100 / Operating).ToString("F2") + "%";//试用期不合格原因
            leave_record2.Add("proportion_trial", proportion_trial);
            var proportion_remove = (Operating == 0 ? 0 : Operating_sum3 * 100 / Operating).ToString("F2") + "%";//协商解除
            leave_record2.Add("proportion_remove", proportion_remove);
            var proportion_other = (Operating == 0 ? 0 : Operating_sum4 * 100 / Operating).ToString("F2") + "%";//其他原因
            leave_record2.Add("proportion_other", proportion_other);
            #endregion

            #region--- 技术族占比
            //技术族原因占比总人数
            var Technology = Technology_sum1 + Technology_sum + Technology_sum2 + Technology_sum3 + Technology_sum4;
            var Tec_couxin = (Technology == 0 ? 0 : Technology_sum1 * 100 / Technology).ToString("F2") + "%";//公司原因
            leave_record2.Add("tec_couxin", Tec_couxin);
            var Tec_genghao = (Technology == 0 ? 0 : Technology_sum * 100 / Technology).ToString("F2") + "%";//个人原因
            leave_record2.Add("tec_genghao", Tec_genghao);
            var Tec_trial = (Technology == 0 ? 0 : Technology_sum2 * 100 / Technology).ToString("F2") + "%";//试用期不合格原因
            leave_record2.Add("tec_trial", Tec_trial);
            var Tec_remove = (Technology == 0 ? 0 : Technology_sum3 * 100 / Technology).ToString("F2") + "%";//协商解除
            leave_record2.Add("tec_remove", Tec_remove);
            var Tec_other = (Technology == 0 ? 0 : Technology_sum4 * 100 / Technology).ToString("F2") + "%";//其他原因
            leave_record2.Add("tec_other", Tec_other);
            #endregion

            #region---专业族占比
            //专业族原因占比总人数
            var Professional = Professional_sum1 + Professional_sum + Professional_sum2 + Professional_sum3 + Professional_sum4;
            var Pro_couxin = (Professional == 0 ? 0 : Professional_sum1 * 100 / Professional).ToString("F2") + "%";//薪酬不满意
            leave_record2.Add("pro_couxin", Pro_couxin);
            var Pro_genghao = (Professional == 0 ? 0 : Professional_sum * 100 / Professional).ToString("F2") + "%";//更好的发展
            leave_record2.Add("pro_genghao", Pro_genghao);
            var Pro_trial = (Professional == 0 ? 0 : (Professional_sum2 * 100 / Professional)).ToString("F2") + "%";//试用期不合格原因
            leave_record2.Add("pro_trial", Pro_trial);
            var Pro_remove = (Professional == 0 ? 0 : (Professional_sum3 * 100 / Professional)).ToString("F2") + "%";//协商解除
            leave_record2.Add("pro_remove", Pro_remove);
            var Pro_other = (Professional == 0 ? 0 : (Professional_sum4 * 100 / Professional)).ToString("F2") + "%";//其他原因
            leave_record2.Add("pro_other", Pro_other);
            #endregion

            #region---服务族占比
            //服务族原因占比总人数
            var Service = Service_sum1 + Service_sum + Service_sum2 + Service_sum3 + Service_sum4;
            var Ser_couxin = (Service == 0 ? 0 : Service_sum1 * 100 / Service).ToString("F2") + "%";//薪酬不满意
            leave_record2.Add("ser_couxin", Ser_couxin);
            var Ser_genghao = (Service == 0 ? 0 : Service_sum * 100 / Service).ToString("F2") + "%";//更好的发展
            leave_record2.Add("ser_genghao", Ser_genghao);
            var Ser_trial = (Service == 0 ? 0 : (Service_sum2 * 100 / Service)).ToString("F2") + "%";//试用期不合格原因
            leave_record2.Add("ser_trial", Ser_trial);
            var Ser_remove = (Service == 0 ? 0 : (Service_sum3 * 100 / Service)).ToString("F2") + "%";//协商解除
            leave_record2.Add("ser_remove", Ser_remove);
            var Ser_other = (Service == 0 ? 0 : (Service_sum4 * 100 / Service)).ToString("F2") + "%";//其他原因
            leave_record2.Add("ser_other", Ser_other);
            #endregion

            #region---管理族占比
            //管理族原因占比总人数
            var Management = Management_sum1 + Management_sum + Management_sum2 + Management_sum3 + Management_sum4;
            //公司原因
            var Man_couxin = (Management == 0 ? 0 : Management_sum1 * 100 / Management).ToString("F2") + "%"; ;//薪酬不满意
            leave_record2.Add("man_couxin", Man_couxin);
            //个人原因
            var Man_genghao = (Management == 0 ? 0 : Management_sum * 100 / Management).ToString("F2") + "%";//更好的发展
            leave_record2.Add("man_genghao", Man_genghao);
            var Man_trial = (Management == 0 ? 0 : (Management_sum2 * 100 / Management)).ToString("F2") + "%";//试用期不合格原因
            leave_record2.Add("man_trial", Man_trial);
            var Man_remove = (Management == 0 ? 0 : (Management_sum3 * 100 / Management)).ToString("F2") + "%";//协商解除
            leave_record2.Add("man_remove", Man_remove);
            var Man_other = (Management == 0 ? 0 : (Management_sum4 * 100 / Management)).ToString("F2") + "%";//其他原因
            leave_record2.Add("man_other", Man_other);
            #endregion


            //离职原因占比
            var Leaving_salary = ((cause_heji == 0 ? 0 : (Pay_diss * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (System * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (management * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (Joben * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (Itself * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (Cultural * 100 / cause_heji))).ToString("F2") + "%";//薪酬不满意
            leave_record2.Add("leaving_salary", Leaving_salary);
            var Leaving_Better = ((cause_heji == 0 ? 0 : (Better * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (Healthy * 100 / cause_heji)) + (cause_heji == 0 ? 0 : (Family * 100 / cause_heji))).ToString("F2") + "%";//更好的发展
            leave_record2.Add("leaving_Better", Leaving_Better);
            var Leaving_Period = (cause_heji == 0 ? 0 : (Period * 100 / cause_heji)).ToString("F2") + "%";//试用期不合格
            leave_record2.Add("leaving_Period", Leaving_Period);
            var Leaving_Remove = (cause_heji == 0 ? 0 : (Remove * 100 / cause_heji)).ToString("F2") + "%";//协商解除
            leave_record2.Add("leaving_Remove", Leaving_Remove);
            var Leaving_Other = (cause_heji == 0 ? 0 : (Other * 100 / cause_heji)).ToString("F2") + "%";//其他原因
            leave_record2.Add("leaving_Other", Leaving_Other);

            #endregion

            leave_record3.Add("leave_record1", leave_record1);
            leave_record3.Add("leave_record2", leave_record2);
            result.Add("leave_record3", leave_record3);
            result.Add("for_leaving_Table", for_leaving_Table);

            return common.GetModuleFromJobjet(result);
        }

        #endregion

        #region------点击部门查看离职原因明细
        [HttpPost]
        [ApiAuthorize]
        public JObject Leaving_reason([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int? year = obj.year;//年
            int? mouth = obj.mouth;//月
            string department = obj.department == null ? null : obj.department;//部门
            if (department == null || year == null || mouth == null)
            {
                return common.GetModuleFromJobjet(result, false, department == null ? "部门为空！" : "" + year == null ? "年份未选择" : "" + mouth == null ? "月份未选择" : "");
            }
            JObject userItem = new JObject();
            //JObject userJobject = new JObject();
            // var leave_mingxi = db.Personnel_Reasons_for_leaving.ToList();
            int L = 0;
            var leave = db.Personnel_Reasons_for_leaving.Where(c => c.Department_leaving == department && c.LastDate.Value.Year == year && c.LastDate.Value.Month == mouth).ToList();
            if (leave.Count > 0)
            {
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
                    if (item.Resign != 0)//辞职
                    {
                        //离退类型：辞职
                        userItem.Add("Resign", 1);
                    }
                    else if (item.SinceFrom != 0)//自离
                    {
                        //离退类型：自离
                        userItem.Add("Resign", 2);
                    }
                    else if (item.LayOff != 0)//辞退
                    {
                        //离退类型：辞退
                        userItem.Add("Resign", 3);
                    }
                    else if (item.Retired != 0)//退休
                    {
                        //离退类型：退休
                        userItem.Add("Resign", 4);
                    }
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
                    //工作本身不满意
                    userItem.Add("NotWork_Itself", item.NotWork_Itself);
                    //文化氛围不满意
                    userItem.Add("NotCultural_Atmosphere", item.NotCultural_Atmosphere);
                    //更好的发展Better_development
                    userItem.Add("Better_development", item.Better_development);
                    //身体健康原因
                    userItem.Add("HealthyBody", item.HealthyBody);
                    //家庭原因
                    userItem.Add("Family_Reasons", item.Family_Reasons);
                    //试用期不合格
                    userItem.Add("NotProbation_Period", item.NotProbation_Period);
                    //协商解除
                    userItem.Add("Negotiate_Remove", item.Negotiate_Remove);
                    //其他原因
                    userItem.Add("Other_Reasons", item.Other_Reasons);

                    result.Add(L.ToString(), userItem);
                    L++;
                    userItem = new JObject();
                }
                return common.GetModuleFromJobjet(result);
            }
            else
                return common.GetModuleFromJobjet(result, false, "没有记录");
        }

        #endregion

        #region ------修改/单个添加离职原因明细记录方法
        [HttpPost]
        [ApiAuthorize]
        public JObject Create([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            Personnel_Reasons_for_leaving personnel_Reasons_for_leaving = JsonConvert.DeserializeObject<Personnel_Reasons_for_leaving>(JsonConvert.SerializeObject(data));

            if (personnel_Reasons_for_leaving.Pay_dissatisfaction != 0 || personnel_Reasons_for_leaving.NotSystem != 0 || personnel_Reasons_for_leaving.Notmanagement != 0 || personnel_Reasons_for_leaving.Jobenvironment != 0 || personnel_Reasons_for_leaving.Better_development != 0 || personnel_Reasons_for_leaving.NotProbation_Period != 0 || personnel_Reasons_for_leaving.NotWork_Itself != 0 || personnel_Reasons_for_leaving.NotCultural_Atmosphere != 0 || personnel_Reasons_for_leaving.Negotiate_Remove != 0 || personnel_Reasons_for_leaving.Family_Reasons != 0 || personnel_Reasons_for_leaving.HealthyBody != 0 || personnel_Reasons_for_leaving.Other_Reasons != 0)
            {
                var amend = db.Personnel_Reasons_for_leaving.Where(c => c.jobNum == personnel_Reasons_for_leaving.jobNum && c.Name == personnel_Reasons_for_leaving.Name).FirstOrDefault();
                if (amend != null)
                {
                    amend.Department_leaving = personnel_Reasons_for_leaving.Department_leaving;
                    amend.levelType = personnel_Reasons_for_leaving.levelType;
                    amend.Pay_dissatisfaction = personnel_Reasons_for_leaving.Pay_dissatisfaction;
                    amend.NotSystem = personnel_Reasons_for_leaving.NotSystem;
                    amend.Notmanagement = personnel_Reasons_for_leaving.Notmanagement;
                    amend.Jobenvironment = personnel_Reasons_for_leaving.Jobenvironment;
                    amend.Better_development = personnel_Reasons_for_leaving.Better_development;
                    amend.NotWork_Itself = personnel_Reasons_for_leaving.NotWork_Itself;
                    amend.NotCultural_Atmosphere = personnel_Reasons_for_leaving.NotCultural_Atmosphere;
                    amend.HealthyBody = personnel_Reasons_for_leaving.HealthyBody;
                    amend.Family_Reasons = personnel_Reasons_for_leaving.Family_Reasons;
                    amend.NotProbation_Period = personnel_Reasons_for_leaving.NotProbation_Period;
                    amend.Negotiate_Remove = personnel_Reasons_for_leaving.Negotiate_Remove;
                    amend.Other_Reasons = personnel_Reasons_for_leaving.Other_Reasons;
                    if (personnel_Reasons_for_leaving.Resign == 1)//辞职
                    {
                        amend.Resign = 1;
                        amend.Retired = 0;
                        amend.LayOff = 0;
                        amend.SinceFrom = 0;
                    }
                    else if (personnel_Reasons_for_leaving.Resign == 2)//自离
                    {
                        amend.Resign = 0;
                        amend.Retired = 0;
                        amend.LayOff = 0;
                        amend.SinceFrom = 1;
                    }
                    else if (personnel_Reasons_for_leaving.Resign == 3)//辞退
                    {
                        amend.Resign = 0;
                        amend.Retired = 0;
                        amend.LayOff = 1;
                        amend.SinceFrom = 0;
                    }
                    else if (personnel_Reasons_for_leaving.Resign == 4)//退休
                    {
                        amend.Resign = 0;
                        amend.Retired = 1;
                        amend.LayOff = 0;
                        amend.SinceFrom = 0;
                    }
                    amend.Remark = personnel_Reasons_for_leaving.Remark;
                    int count = db.SaveChanges();
                    if (count > 0)
                    {
                        return common.GetModuleFromJobjet(result, true, "修改成功");
                    }
                    else
                    {
                        return common.GetModuleFromJobjet(result, false, "修改失败！");
                    }
                }
                else
                {
                    if (personnel_Reasons_for_leaving.Resign == 1)//辞职
                    {
                        personnel_Reasons_for_leaving.Resign = 1;
                        personnel_Reasons_for_leaving.Retired = 0;
                        personnel_Reasons_for_leaving.LayOff = 0;
                        personnel_Reasons_for_leaving.SinceFrom = 0;
                        db.SaveChanges();

                    }
                    else if (personnel_Reasons_for_leaving.Resign == 2)//自离
                    {
                        personnel_Reasons_for_leaving.Resign = 0;
                        personnel_Reasons_for_leaving.Retired = 0;
                        personnel_Reasons_for_leaving.LayOff = 0;
                        personnel_Reasons_for_leaving.SinceFrom = 1;
                        db.SaveChanges();

                    }
                    else if (personnel_Reasons_for_leaving.Resign == 3)//辞退
                    {
                        personnel_Reasons_for_leaving.Resign = 0;
                        personnel_Reasons_for_leaving.Retired = 0;
                        personnel_Reasons_for_leaving.LayOff = 1;
                        personnel_Reasons_for_leaving.SinceFrom = 0;
                        db.SaveChanges();

                    }
                    else if (personnel_Reasons_for_leaving.Resign == 4)//退休
                    {
                        personnel_Reasons_for_leaving.Resign = 0;
                        personnel_Reasons_for_leaving.Retired = 1;
                        personnel_Reasons_for_leaving.LayOff = 0;
                        personnel_Reasons_for_leaving.SinceFrom = 0;
                        db.SaveChanges();

                    }
                    db.Personnel_Reasons_for_leaving.Add(personnel_Reasons_for_leaving);
                    int count = db.SaveChanges();
                    if (count > 0)
                    {
                        return common.GetModuleFromJobjet(result, true, "添加成功");
                    }
                    else
                    {
                        return common.GetModuleFromJobjet(result, false, "添加失败！");
                    }
                }
            }
            return common.GetModuleFromJobjet(result, false, "添加失败！");
        }

        #endregion

        #region------批量增加离职原因记录方法
        [HttpPost]
        [ApiAuthorize]
        public JObject Batch_leave([System.Web.Http.FromBody]JObject data)
        {
            JArray result = new JArray();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            List<Personnel_Reasons_for_leaving> inputList = obj.inputList;
            if (inputList.Count > 0)
            {
                foreach (var item in inputList)
                {
                    item.CreateDate = DateTime.Now;
                    item.Creator = auth.UserName;
                    if (db.Personnel_Reasons_for_leaving.Count(c => c.jobNum == item.jobNum) != 0)
                        result.Add(item.jobNum);
                }
                if (result.Count > 0)
                {
                    return common.GetModuleFromJarray(result, false, "数据重复");
                }
                foreach (var it in inputList)
                {
                    if (it.Resign == 1)//辞职
                    {
                        it.Resign = 1;
                        it.Retired = 0;
                        it.LayOff = 0;
                        it.SinceFrom = 0;
                        db.SaveChanges();
                    }
                    else if (it.Resign == 2)//自离
                    {
                        it.Resign = 0;
                        it.Retired = 0;
                        it.LayOff = 0;
                        it.SinceFrom = 1;
                        db.SaveChanges();
                    }
                    else if (it.Resign == 3)//辞退
                    {
                        it.Resign = 0;
                        it.Retired = 0;
                        it.LayOff = 1;
                        it.SinceFrom = 0;
                        db.SaveChanges();
                    }
                    else if (it.Resign == 4)//退休
                    {
                        it.Resign = 0;
                        it.Retired = 1;
                        it.LayOff = 0;
                        it.SinceFrom = 0;
                        db.SaveChanges();
                    }
                }
                db.Personnel_Reasons_for_leaving.AddRange(inputList);
                int count = db.SaveChanges();
                if (count > 0)
                {
                    return common.GetModuleFromJarray(result, true, "添加成功");
                }
                else
                {
                    return common.GetModuleFromJarray(result, false, "添加失败");
                }
            }
            return common.GetModuleFromJarray(result, false, "添加失败");
        }

        #endregion

        #region ------删除离职原因记录方法
        [HttpPost]
        [ApiAuthorize]
        public JObject DeleteConfirmed([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int id = obj.id;//id
            if (id != 0)
            {
                var leaving = db.Personnel_Reasons_for_leaving.Where(c => c.Id == id).FirstOrDefault();
                UserOperateLog operaterecord = new UserOperateLog();
                operaterecord.OperateDT = DateTime.Now;//添加删除操作时间
                operaterecord.Operator = auth.UserName;//添加删除操作人                                                      
                operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除离职原因记录工号为" + leaving.jobNum + "数据记录"; //添加操作记录
                db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
                db.Personnel_Reasons_for_leaving.Remove(leaving);
                int count = db.SaveChanges();
                if (count > 0)
                {
                    return common.GetModuleFromJobjet(result, true, "删除成功");
                }
                else
                {
                    return common.GetModuleFromJobjet(result, false, "删除失败");
                }
            }
            return common.GetModuleFromJobjet(result, false, "删除失败");
        }

        #endregion



    }



}
