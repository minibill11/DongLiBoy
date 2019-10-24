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
using Newtonsoft.Json;
using static JianHeMES.Controllers.CommonalityController;
using System.Web.Script.Serialization;
using System.Collections;
using System.IO;

namespace JianHeMES.Controllers
{
    public class Personnel_Quality_objectivesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public class Rink
        {
            public decimal toal { get; set; }
            public string assdepar { get; set; }
            public string dp_gur { get; set; }
            public int wj { get; set; }
            public decimal ccl { get; set; }
            public string pm { get; set; }
            public string jf { get; set; }
            public int bzrs { get; set; }

        }
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Quality_objectives", act = "Index" });
            }
            return View();
        }

        public ActionResult Summarizing()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Quality_objectives", act = "Summarizing" });
            }
            return View();
        }

        #region --------------------DepartmentList()检索部门
        public ActionResult DepartmentList()
        {
            var depar = db.Personnel_IndexName.OrderByDescending(m => m.Id).Select(c => c.DepartmentData).Distinct();
            return Content(JsonConvert.SerializeObject(depar));
        }
        #endregion

        #region --------------------AssessmentList()检索考核指标比例版本
        public ActionResult AssessmentList()
        {
            var indica = db.Personnel_Assessment_indicators.OrderByDescending(m => m.Id).Select(c => new { c.indName, c.Price }).ToList();
            return Content(JsonConvert.SerializeObject(indica));
        }
        #endregion

        #region -----质量目标达成状况统计表显示
        public ActionResult Quality(string departmentdata, int year, int month)
        {
            //传入值有效，查数据
            var data = db.Personnel_Quality_objectives.Where(c => c.DepartmentData == departmentdata && c.Year == year && c.Month <= month).ToList();
            JObject userItem = new JObject();
            JObject userJobject = new JObject();
            int i = 0;
            //被考核部门有几个,此被考核部门班组有几个，项目、指标名称有几组
            var departmentlist = data.Select(c => c.AssetDepartment).Distinct().ToList();
            foreach (var department in departmentlist)//循环被考核部门
            {
                var DG_list = data.Where(c => c.AssetDepartment == department).Select(c => c.DP_Group).Distinct().ToList();
                foreach (var dg in DG_list)//循环被考核部门的班组
                {
                    var project_list = data.Where(c => c.AssetDepartment == department && c.DP_Group == dg).Select(c => c.Project).Distinct().ToList();
                    foreach (var project in project_list)//循环被考核部门的项目
                    {
                        var depart_DG_project_datalist = data.Where(c => c.AssetDepartment == department && c.DP_Group == dg && c.Project == project).Select(c => c.IndexName).ToList();
                        foreach (var target in depart_DG_project_datalist)//循环被考核部门的指标名称
                        {
                            var asset_target = data.Where(c => c.AssetDepartment == department && c.DP_Group == dg && c.Project == project && c.IndexName == target).Select(c => c.Target + c.Target_value + c.Target2).ToList();
                            foreach (var itge in asset_target)//循环被考核部门的目标值
                            {
                                var tarodlist = data.Where(c => c.AssetDepartment == department && c.DP_Group == dg && c.Project == project && c.IndexName == target && c.Target + c.Target_value + c.Target2 == itge).OrderBy(c => c.Month).ToList();
                                //ID
                                userItem.Add("Id", tarodlist.FirstOrDefault().Id);
                                //部门
                                userItem.Add("AssetDepartment", tarodlist.FirstOrDefault().AssetDepartment);
                                //班组
                                userItem.Add("DP_Group", tarodlist.FirstOrDefault().DP_Group);
                                //项目
                                userItem.Add("project", tarodlist.FirstOrDefault().Project);
                                //指标名称
                                userItem.Add("indexName", tarodlist.FirstOrDefault().IndexName);
                                //目标值
                                userItem.Add("target", tarodlist.FirstOrDefault().Target + tarodlist.FirstOrDefault().Target_value + tarodlist.FirstOrDefault().Target2);
                                foreach (var da in tarodlist)//循环月份
                                {
                                    //月份实际完成值
                                    userItem.Add(da.Month.ToString(), da.Actual_completion_value + da.Actua1);
                                }
                                userJobject.Add(i.ToString(), userItem);
                                userItem = new JObject();
                                i++;
                            }
                        }
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(userJobject));
        }

        #endregion

        #region----优秀班组评优汇总表
        public ActionResult Excellent(int year, int month)
        {
            JObject output1 = new JObject();
            JArray userItem = new JArray();
            var Appraising = db.Personnel_Quality_objectives.Where(c => c.Year == year && c.Month == month).ToList();//按年月取数据
            var department = Appraising.Select(c => c.AssetDepartment).Distinct().ToList();//被考核部门清单
            var project = db.Personnel_Assessment_indicators.OrderBy(c => c.Id).Select(c => c.indName).ToList();//被考核部门被考核班组的被考核项
            List<Rink> ment = new List<Rink>();
            foreach (var item_eng in department)
            {
                var dp_ge = Appraising.Where(c => c.AssetDepartment == item_eng).Select(c => c.DP_Group).Distinct().ToList();//被考核部门的被考核班组
                foreach (var ihte in dp_ge)
                {
                    var item1 = Appraising.Where(c => c.AssetDepartment == item_eng && c.DP_Group == ihte).ToList();//被考核部门的被考核班组的数据
                    decimal Total_Points = 0;
                    //ID
                    output1.Add("Id", item1.FirstOrDefault().Id);
                    //被考核部门
                    output1.Add("AssetDepartment", item1.FirstOrDefault().AssetDepartment);
                    //班组
                    output1.Add("DP_Group", item1.FirstOrDefault().DP_Group);
                    int ii = 1;
                    //foreach (var item_peoject in peoject)
                    foreach (var item_peoject in project)
                    {
                        var item = Appraising.Where(c => c.AssetDepartment == item_eng && c.DP_Group == ihte && c.Project == item_peoject).ToList();
                        //项目
                        output1.Add("Project" + ii, item.Count == 0 ? "未考核" : item.FirstOrDefault().Project);
                        //指标名称
                        output1.Add("IndexName" + ii, item.Count == 0 ? "未考核" : item.FirstOrDefault().IndexName);
                        //目标值
                        output1.Add("Target_value" + ii, item.Count == 0 ? "未考核" : item.FirstOrDefault().Target + item.FirstOrDefault().Target_value + item.FirstOrDefault().Target2);
                        //实际值
                        output1.Add("Actual_completion_value" + ii, item.Count == 0 ? "未考核" : item.FirstOrDefault().Actual_completion_value + item.FirstOrDefault().Actua1);
                        string ikge = "";
                        //考核部门
                        foreach (var it in item)
                        {
                            ikge = ikge + it.DepartmentData + ",";
                        }
                        output1.Add("DepartmentData" + ii, item.Count == 0 ? "未考核" : ikge);
                        decimal su = 100;
                        decimal Total = 0;
                        var de = db.Personnel_Assessment_indicators.Where(c => c.indName == item_peoject).Select(c => c.Price).FirstOrDefault();
                        var target_list = item.Select(c => new { c.Target, c.Target_value, c.Target2 }).FirstOrDefault();
                        var actual_list = item.Select(c => new { c.Actual_completion_value, c.Actua1 }).FirstOrDefault();
                        if (item.Count == 0)
                        {
                            output1.Add("danx" + ii, "未考核");//单项得分
                            output1.Add("defen" + ii, "未考核");//得分小计
                        }
                        else
                        {
                            if (target_list.Target == "≧" && target_list.Target2 == "%")
                            {
                                if (target_list.Target_value > actual_list.Actual_completion_value)
                                {
                                    output1.Add("danx" + ii, 0);//单项得分
                                    decimal Bdefen = Total = 0 * de;
                                    output1.Add("defen" + ii, Bdefen);//得分小计
                                }
                                else if (target_list.Target_value <= actual_list.Actual_completion_value)
                                {
                                    decimal danx = su;
                                    output1.Add("danx" + ii, danx);//单项得分
                                    decimal dexfen = Total = danx * de;
                                    output1.Add("defen" + ii, dexfen);//得分小计
                                }
                            }
                            else if(target_list.Target == "≦" && target_list.Target2 == "%")
                            {
                                if (target_list.Target_value > actual_list.Actual_completion_value)
                                {
                                    decimal danx = su;
                                    output1.Add("danx" + ii, danx);//单项得分
                                    decimal dexfen = Total = danx * de;
                                    output1.Add("defen" + ii, dexfen);//得分小计
                                   
                                }
                                else if (target_list.Target_value <= actual_list.Actual_completion_value)
                                {
                                    output1.Add("danx" + ii, 0);//单项得分
                                    decimal Bdefen = Total = 0 * de;
                                    output1.Add("defen" + ii, Bdefen);//得分小计
                                }
                            }
                            else if (target_list.Target == "≦" && target_list.Target2 == "个订单")
                            {
                                if (target_list.Target_value > actual_list.Actual_completion_value)
                                {
                                    decimal danx = su;
                                    output1.Add("danx" + ii, danx);//单项得分
                                    decimal dexfen = Total = danx * de;
                                    output1.Add("defen" + ii, dexfen);//得分小计
                                }
                                else if (target_list.Target_value <= actual_list.Actual_completion_value)
                                {
                                    output1.Add("danx" + ii, 0);//单项得分
                                    decimal Bdefen = Total = 0 * de;
                                    output1.Add("defen" + ii, Bdefen);//得分小计
                                }
                            }
                            else if (item.FirstOrDefault().Target_value == item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "人力资源部")
                            {
                                output1.Add("danx" + ii, su);
                                decimal Cdefen = Total = su * de;
                                output1.Add("defen", Cdefen);
                            }
                            else if (item.FirstOrDefault().Target_value < item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "人力资源部")
                            {
                                decimal rte = su - (decimal)(item.FirstOrDefault().Actual_completion_value - item.FirstOrDefault().Target_value);//单项得分
                                output1.Add("danx" + ii, rte);//单项得分
                                decimal Ddefen = Total = rte * de;//得分小计
                                output1.Add("defen" + ii, Ddefen);//得分小计
                            }
                            else if (item.FirstOrDefault().Target_value > item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "人力资源部")
                            {
                                decimal tef = su + (decimal)(item.FirstOrDefault().Target_value - item.FirstOrDefault().Actual_completion_value);//单项得分
                                output1.Add("danx" + ii, tef);//单项得分
                                decimal Edefen = Total = tef * de;//得分小计
                                output1.Add("defen" + ii, Edefen);//得分小计
                            }
                            else if (item.FirstOrDefault().Target_value == item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "行政后勤部")
                            {
                                output1.Add("danx" + ii, su);//单项得分
                                decimal Fdefen = Total = su * de;//得分小计
                                output1.Add("defen" + ii, Fdefen);//得分小计
                            }
                            else if (item.FirstOrDefault().Target_value < item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "行政后勤部")
                            {
                                decimal dt = su + (decimal)(item.FirstOrDefault().Actual_completion_value - item.FirstOrDefault().Target_value);//单项得分
                                output1.Add("danx" + ii, dt);//单项得分
                                decimal Gdefen = Total = dt * de;//得分小计
                                output1.Add("defen" + ii, Gdefen);  //得分小计                                    
                            }
                            else if (item.FirstOrDefault().Target_value > item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "行政后勤部")
                            {
                                decimal sde = su - (decimal)(item.FirstOrDefault().Target_value - item.FirstOrDefault().Actual_completion_value);//单项得分
                                output1.Add("danx" + ii, sde);//单项得分
                                decimal Hdefen = Total = sde * de;//得分小计
                                output1.Add("defen" + ii, Hdefen); //得分小计 
                            }
                        }                       
                        Total_Points = Total_Points + Total;
                        ii++;
                    }
                    output1.Add("total_points", Total_Points);
                    var record_byAsDepGP_Group_Y_M = db.Personnel_Ranking_management.Where(c => c.AsseDepartment == item_eng && c.DP_Group == ihte && c.Year == year && c.Month == month).FirstOrDefault();
                    output1.Add("wj", record_byAsDepGP_Group_Y_M == null ? 0 : record_byAsDepGP_Group_Y_M.Disciplinary);
                    output1.Add("ccl", record_byAsDepGP_Group_Y_M == null ? 0 : record_byAsDepGP_Group_Y_M.Attendance_value);
                    output1.Add("PM", "");
                    output1.Add("JF", "");
                    userItem.Add(output1);
                    output1 = new JObject();
                }
            }

            #region--排名计算

            List<Rink> rinklist = new List<Rink>();//取出所有部门、组、总分
            List<Rink> rinklist2 = new List<Rink>();//取出所有部门、组、总分
            foreach (var i in userItem)
            {
                Rink record = new Rink();
                record.assdepar = i["AssetDepartment"].ToString();
                record.dp_gur = i["DP_Group"].ToString();
                record.toal = Convert.ToDecimal(i["total_points"]);
                var re = db.Personnel_Ranking_management.Where(c => c.AsseDepartment == record.assdepar && c.DP_Group == record.dp_gur && c.Year == year && c.Month == month).FirstOrDefault();
                record.wj = re == null ? 0 : re.Disciplinary;
                record.ccl = re == null ? 0 : re.Attendance_value;
                record.bzrs = re == null ? 0 : re.GroupNumber;
                rinklist.Add(record);
                rinklist2.Add(record);
            }
            var rank = new JArray(userItem.OrderByDescending(c => c["total_points"]));//按总分排序
            //对rinklist排序：total,wj,ccl
            rinklist = rinklist.Where(c => c.toal >= 90 && c.bzrs >= 5).OrderByDescending(c => c.toal).ThenBy(c => c.wj).ThenByDescending(c => c.ccl).ToList();
            int pm_num = 1;
            #region---三列相同判断的排名方法
            for (int k = 0; k < rinklist.Count(); k++)
            {
                if (pm_num > 5) break;
                var it = rinklist[k];
                var count_three_col_same = rinklist.Where(c => c.toal == it.toal && c.wj == it.wj && c.ccl == it.ccl);
                if (count_three_col_same.Count() > 1)
                {
                    foreach (var ii in count_three_col_same)
                    {
                        rinklist[k].pm = String.IsNullOrEmpty(ii.pm) ? ($"第{pm_num}名") : ii.pm;
                        rinklist[k].jf = jfreturn(pm_num);
                        k++;
                    }
                    k--;
                }
                else
                {
                    rinklist[k].pm = $"第{pm_num}名";
                    rinklist[k].jf = jfreturn(pm_num);
                }
                pm_num++;
            }
            //把排名放回汇总表
            foreach (var addval in rinklist)
            {
                rank.Where(c => c["AssetDepartment"].ToString() == addval.assdepar && c["DP_Group"].ToString() == addval.dp_gur).FirstOrDefault()["PM"] = addval.pm;
                rank.Where(c => c["AssetDepartment"].ToString() == addval.assdepar && c["DP_Group"].ToString() == addval.dp_gur).FirstOrDefault()["JF"] = addval.jf;
            }
            #endregion
            #region---三列相同判断的倒数排名方法
            rinklist2 = rinklist2.OrderBy(c => c.toal).ThenByDescending(c => c.wj).ThenBy(c => c.ccl).ToList();
            var re_pm_three_fu = rinklist2.Select(c => c.toal).Distinct().Take(3).LastOrDefault();//-3fs
            rinklist2 = rinklist2.Where(c => c.toal <= re_pm_three_fu).ToList();
            int pmnum = 1;
            for (int g = 0; g < rinklist2.Count(); g++)
            {
                if (pmnum > 3) break;
                var tg = rinklist2[g];
                var count_three = rinklist2.Where(c => c.toal == tg.toal && c.wj == tg.wj && c.ccl == tg.ccl);
                if (count_three.Count() > 1)
                {
                    foreach (var ip in count_three)
                    {
                        rinklist2[g].pm = String.IsNullOrEmpty(ip.pm) ? ($"倒数第{pmnum}名") : ip.pm;
                        rinklist2[g].jf = jfreturn(-pmnum);
                        g++;
                    }
                    g--;
                }
                else
                {
                    rinklist2[g].pm = $"倒数第{pmnum}名";
                    rinklist2[g].jf = jfreturn(-pmnum);
                }
                pmnum++;
            }
            //把倒数排名放回汇总表
            foreach (var addlist in rinklist2)
            {
                rank.Where(c => c["AssetDepartment"].ToString() == addlist.assdepar && c["DP_Group"].ToString() == addlist.dp_gur).FirstOrDefault()["PM"] = addlist.pm;
                rank.Where(c => c["AssetDepartment"].ToString() == addlist.assdepar && c["DP_Group"].ToString() == addlist.dp_gur).FirstOrDefault()["JF"] = addlist.jf;
            }
            #endregion
            #endregion
            return Content(JsonConvert.SerializeObject(rank));
        }
        public string Excellent2(int year, int month)
        {
            JObject output1 = new JObject();
            JArray userItem = new JArray();
            var Appraising = db.Personnel_Quality_objectives.Where(c => c.Year == year && c.Month == month).ToList();//按年月取数据
            var department = Appraising.Select(c => c.AssetDepartment).Distinct().ToList();//被考核部门清单
            var project = db.Personnel_Assessment_indicators.OrderBy(c => c.Id).Select(c => c.indName).ToList();//被考核部门被考核班组的被考核项
            List<Rink> ment = new List<Rink>();
            foreach (var item_eng in department)
            {
                var dp_ge = Appraising.Where(c => c.AssetDepartment == item_eng).Select(c => c.DP_Group).Distinct().ToList();//被考核部门的被考核班组
                foreach (var ihte in dp_ge)
                {
                    var item1 = Appraising.Where(c => c.AssetDepartment == item_eng && c.DP_Group == ihte).ToList();//被考核部门的被考核班组的数据
                    decimal Total_Points = 0;
                    //ID
                    output1.Add("Id", item1.FirstOrDefault().Id);
                    //被考核部门
                    output1.Add("AssetDepartment", item1.FirstOrDefault().AssetDepartment);
                    //班组
                    output1.Add("DP_Group", item1.FirstOrDefault().DP_Group);
                    int ii = 1;
                    //foreach (var item_peoject in peoject)
                    foreach (var item_peoject in project)
                    {
                        var item = Appraising.Where(c => c.AssetDepartment == item_eng && c.DP_Group == ihte && c.Project == item_peoject).ToList();
                        //项目
                        output1.Add("Project" + ii, item.Count == 0 ? "未考核" : item.FirstOrDefault().Project);
                        //指标名称
                        output1.Add("IndexName" + ii, item.Count == 0 ? "未考核" : item.FirstOrDefault().IndexName);
                        //目标值
                        output1.Add("Target_value" + ii, item.Count == 0 ? "未考核" : item.FirstOrDefault().Target + item.FirstOrDefault().Target_value + item.FirstOrDefault().Target2);
                        //实际值
                        output1.Add("Actual_completion_value" + ii, item.Count == 0 ? "未考核" : item.FirstOrDefault().Actual_completion_value + item.FirstOrDefault().Actua1);
                        string ikge = "";
                        //考核部门
                        foreach (var it in item)
                        {
                            ikge = ikge + it.DepartmentData + ",";
                        }
                        output1.Add("DepartmentData" + ii, item.Count == 0 ? "未考核" : ikge);
                        decimal su = 100;
                        decimal Total = 0;
                        var de = db.Personnel_Assessment_indicators.Where(c => c.indName == item_peoject).Select(c => c.Price).FirstOrDefault();
                        var target_list = item.Select(c => new { c.Target, c.Target_value, c.Target2 }).FirstOrDefault();
                        var actual_list = item.Select(c => new { c.Actual_completion_value, c.Actua1 }).FirstOrDefault();
                        if (item.Count == 0)
                        {
                            output1.Add("danx" + ii, "未考核");//单项得分
                            output1.Add("defen" + ii, "未考核");//得分小计
                        }
                        else
                        {
                            if (target_list.Target == "≧" && target_list.Target2 == "%")
                            {
                                if (target_list.Target_value > actual_list.Actual_completion_value)
                                {
                                    output1.Add("danx" + ii, 0);//单项得分
                                    decimal Bdefen = Total = 0 * de;
                                    output1.Add("defen" + ii, Bdefen);//得分小计
                                }
                                else if (target_list.Target_value <= actual_list.Actual_completion_value)
                                {
                                    decimal danx = su;
                                    output1.Add("danx" + ii, danx);//单项得分
                                    decimal dexfen = Total = danx * de;
                                    output1.Add("defen" + ii, dexfen);//得分小计
                                }
                            }
                            else if (target_list.Target == "≦" && target_list.Target2 == "%")
                            {
                                if (target_list.Target_value > actual_list.Actual_completion_value)
                                {
                                    decimal danx = su;
                                    output1.Add("danx" + ii, danx);//单项得分
                                    decimal dexfen = Total = danx * de;
                                    output1.Add("defen" + ii, dexfen);//得分小计

                                }
                                else if (target_list.Target_value <= actual_list.Actual_completion_value)
                                {
                                    output1.Add("danx" + ii, 0);//单项得分
                                    decimal Bdefen = Total = 0 * de;
                                    output1.Add("defen" + ii, Bdefen);//得分小计
                                }
                            }
                            else if (target_list.Target == "≦" && target_list.Target2 == "个订单")
                            {
                                if (target_list.Target_value > actual_list.Actual_completion_value)
                                {
                                    decimal danx = su;
                                    output1.Add("danx" + ii, danx);//单项得分
                                    decimal dexfen = Total = danx * de;
                                    output1.Add("defen" + ii, dexfen);//得分小计
                                }
                                else if (target_list.Target_value <= actual_list.Actual_completion_value)
                                {
                                    output1.Add("danx" + ii, 0);//单项得分
                                    decimal Bdefen = Total = 0 * de;
                                    output1.Add("defen" + ii, Bdefen);//得分小计
                                }
                            }
                            else if (item.FirstOrDefault().Target_value == item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "人力资源部")
                            {
                                output1.Add("danx" + ii, su);
                                decimal Cdefen = Total = su * de;
                                output1.Add("defen", Cdefen);
                            }
                            else if (item.FirstOrDefault().Target_value < item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "人力资源部")
                            {
                                decimal rte = su - (decimal)(item.FirstOrDefault().Actual_completion_value - item.FirstOrDefault().Target_value);//单项得分
                                output1.Add("danx" + ii, rte);//单项得分
                                decimal Ddefen = Total = rte * de;//得分小计
                                output1.Add("defen" + ii, Ddefen);//得分小计
                            }
                            else if (item.FirstOrDefault().Target_value > item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "人力资源部")
                            {
                                decimal tef = su + (decimal)(item.FirstOrDefault().Target_value - item.FirstOrDefault().Actual_completion_value);//单项得分
                                output1.Add("danx" + ii, tef);//单项得分
                                decimal Edefen = Total = tef * de;//得分小计
                                output1.Add("defen" + ii, Edefen);//得分小计
                            }
                            else if (item.FirstOrDefault().Target_value == item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "行政后勤部")
                            {
                                output1.Add("danx" + ii, su);//单项得分
                                decimal Fdefen = Total = su * de;//得分小计
                                output1.Add("defen" + ii, Fdefen);//得分小计
                            }
                            else if (item.FirstOrDefault().Target_value < item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "行政后勤部")
                            {
                                decimal dt = su + (decimal)(item.FirstOrDefault().Actual_completion_value - item.FirstOrDefault().Target_value);//单项得分
                                output1.Add("danx" + ii, dt);//单项得分
                                decimal Gdefen = Total = dt * de;//得分小计
                                output1.Add("defen" + ii, Gdefen);  //得分小计                                    
                            }
                            else if (item.FirstOrDefault().Target_value > item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "行政后勤部")
                            {
                                decimal sde = su - (decimal)(item.FirstOrDefault().Target_value - item.FirstOrDefault().Actual_completion_value);//单项得分
                                output1.Add("danx" + ii, sde);//单项得分
                                decimal Hdefen = Total = sde * de;//得分小计
                                output1.Add("defen" + ii, Hdefen); //得分小计 
                            }
                        }
                        Total_Points = Total_Points + Total;
                        ii++;
                    }
                    output1.Add("total_points", Total_Points);
                    var record_byAsDepGP_Group_Y_M = db.Personnel_Ranking_management.Where(c => c.AsseDepartment == item_eng && c.DP_Group == ihte && c.Year == year && c.Month == month).FirstOrDefault();
                    output1.Add("wj", record_byAsDepGP_Group_Y_M == null ? 0 : record_byAsDepGP_Group_Y_M.Disciplinary);
                    output1.Add("ccl", record_byAsDepGP_Group_Y_M == null ? 0 : record_byAsDepGP_Group_Y_M.Attendance_value);
                    output1.Add("PM", "");
                    output1.Add("JF", "");
                    userItem.Add(output1);
                    output1 = new JObject();
                }
            }

            #region--排名计算

            List<Rink> rinklist = new List<Rink>();//取出所有部门、组、总分
            List<Rink> rinklist2 = new List<Rink>();//取出所有部门、组、总分
            foreach (var i in userItem)
            {
                Rink record = new Rink();
                record.assdepar = i["AssetDepartment"].ToString();
                record.dp_gur = i["DP_Group"].ToString();
                record.toal = Convert.ToDecimal(i["total_points"]);
                var re = db.Personnel_Ranking_management.Where(c => c.AsseDepartment == record.assdepar && c.DP_Group == record.dp_gur && c.Year == year && c.Month == month).FirstOrDefault();
                record.wj = re == null ? 0 : re.Disciplinary;
                record.ccl = re == null ? 0 : re.Attendance_value;
                record.bzrs = re == null ? 0 : re.GroupNumber;
                rinklist.Add(record);
                rinklist2.Add(record);
            }
            var rank = new JArray(userItem.OrderByDescending(c => c["total_points"]));//按总分排序
            //对rinklist排序：total,wj,ccl
            rinklist = rinklist.Where(c => c.toal >= 90 && c.bzrs >= 5).OrderByDescending(c => c.toal).ThenBy(c => c.wj).ThenByDescending(c => c.ccl).ToList();
            int pm_num = 1;
            #region---三列相同判断的排名方法
            for (int k = 0; k < rinklist.Count(); k++)
            {
                if (pm_num > 5) break;
                var it = rinklist[k];
                var count_three_col_same = rinklist.Where(c => c.toal == it.toal && c.wj == it.wj && c.ccl == it.ccl);
                if (count_three_col_same.Count() > 1)
                {
                    foreach (var ii in count_three_col_same)
                    {
                        rinklist[k].pm = String.IsNullOrEmpty(ii.pm) ? ($"第{pm_num}名") : ii.pm;
                        rinklist[k].jf = jfreturn(pm_num);
                        k++;
                    }
                    k--;
                }
                else
                {
                    rinklist[k].pm = $"第{pm_num}名";
                    rinklist[k].jf = jfreturn(pm_num);
                }
                pm_num++;
            }
            //把排名放回汇总表
            foreach (var addval in rinklist)
            {
                rank.Where(c => c["AssetDepartment"].ToString() == addval.assdepar && c["DP_Group"].ToString() == addval.dp_gur).FirstOrDefault()["PM"] = addval.pm;
                rank.Where(c => c["AssetDepartment"].ToString() == addval.assdepar && c["DP_Group"].ToString() == addval.dp_gur).FirstOrDefault()["JF"] = addval.jf;
            }
            #endregion
            #region---三列相同判断的倒数排名方法
            rinklist2 = rinklist2.OrderBy(c => c.toal).ThenByDescending(c => c.wj).ThenBy(c => c.ccl).ToList();
            var re_pm_three_fu = rinklist2.Select(c => c.toal).Distinct().Take(3).LastOrDefault();//-3fs
            rinklist2 = rinklist2.Where(c => c.toal <= re_pm_three_fu).ToList();
            int pmnum = 1;
            for (int g = 0; g < rinklist2.Count(); g++)
            {
                if (pmnum > 3) break;
                var tg = rinklist2[g];
                var count_three = rinklist2.Where(c => c.toal == tg.toal && c.wj == tg.wj && c.ccl == tg.ccl);
                if (count_three.Count() > 1)
                {
                    foreach (var ip in count_three)
                    {
                        rinklist2[g].pm = String.IsNullOrEmpty(ip.pm) ? ($"倒数第{pmnum}名") : ip.pm;
                        rinklist2[g].jf = jfreturn(-pmnum);
                        g++;
                    }
                    g--;
                }
                else
                {
                    rinklist2[g].pm = $"倒数第{pmnum}名";
                    rinklist2[g].jf = jfreturn(-pmnum);
                }
                pmnum++;
            }
            //把倒数排名放回汇总表
            foreach (var addlist in rinklist2)
            {
                rank.Where(c => c["AssetDepartment"].ToString() == addlist.assdepar && c["DP_Group"].ToString() == addlist.dp_gur).FirstOrDefault()["PM"] = addlist.pm;
                rank.Where(c => c["AssetDepartment"].ToString() == addlist.assdepar && c["DP_Group"].ToString() == addlist.dp_gur).FirstOrDefault()["JF"] = addlist.jf;
            }
            #endregion
            #endregion
            return JsonConvert.SerializeObject(rank);
        }

        //public ActionResult Excellent(int year, int month)
        //{
        //    JObject output1 = new JObject();
        //    JArray userItem = new JArray();
        //    var Appraising = db.Personnel_Quality_objectives.Where(c => c.Year == year && c.Month == month).ToList();
        //    var department = Appraising.Select(c => c.AssetDepartment).Distinct().ToList();
        //    List<Rink> ment = new List<Rink>();
        //    foreach (var item_eng in department)
        //    {
        //        var dp_ge = Appraising.Where(c => c.AssetDepartment == item_eng).Select(c => c.DP_Group).Distinct().ToList();
        //        foreach (var ihte in dp_ge)
        //        {
        //            var peoject = Appraising.Where(c => c.AssetDepartment == item_eng && c.DP_Group == ihte).Select(c => c.Project).Distinct().ToList();
        //            var item1 = Appraising.Where(c => c.AssetDepartment == item_eng && c.DP_Group == ihte).ToList();
        //            //ID
        //            output1.Add("Id", item1.FirstOrDefault().Id);
        //            //被考核部门
        //            output1.Add("AssetDepartment", item1.FirstOrDefault().AssetDepartment);
        //            //班组
        //            output1.Add("DP_Group", item1.FirstOrDefault().DP_Group);
        //            JArray merge = new JArray();
        //            decimal Total_Points = 0;
        //            foreach (var item_peoject in peoject)
        //            {
        //                JObject output = new JObject();
        //                var item = Appraising.Where(c => c.AssetDepartment == item_eng && c.DP_Group == ihte && c.Project == item_peoject).ToList();
        //                //项目
        //                output.Add("Project", item.FirstOrDefault().Project);
        //                //指标名称
        //                output.Add("IndexName", item.FirstOrDefault().IndexName);
        //                //目标值
        //                output.Add("Target_value", item.FirstOrDefault().Target + item.FirstOrDefault().Target_value + item.FirstOrDefault().Target2);
        //                //实际值
        //                output.Add("Actual_completion_value", item.FirstOrDefault().Actual_completion_value + item.FirstOrDefault().Actua1);
        //                string ikge = "";
        //                //考核部门
        //                foreach (var it in item)
        //                {
        //                    ikge = ikge + it.DepartmentData + ",";
        //                }
        //                output.Add("DepartmentData", ikge);
        //                decimal su = 100;
        //                decimal Total = 0;
        //                var project = item.FirstOrDefault().Project;
        //                var de = db.Personnel_Assessment_indicators.Where(c => c.indName == project).Select(c => c.Price).FirstOrDefault();

        //                if (item.FirstOrDefault().Target_value <= item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData != "人力资源部" && item.FirstOrDefault().DepartmentData != "行政后勤部")
        //                {
        //                    int danx = 0;
        //                    output.Add("danx", danx);//单项得分
        //                    decimal dexfen = Total = danx * de;
        //                    output.Add("defen", dexfen);//得分小计
        //                }
        //                else if (item.FirstOrDefault().Target_value > item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData != "人力资源部" && item.FirstOrDefault().DepartmentData != "行政后勤部")
        //                {
        //                    output.Add("danx", su);//单项得分
        //                    decimal Bdefen = Total = su * de;
        //                    output.Add("defen", Bdefen);//得分小计
        //                }
        //                else if (item.FirstOrDefault().Target_value == item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "人力资源部")
        //                {
        //                    output.Add("danx", su);
        //                    decimal Cdefen = Total = su * de;
        //                    output.Add("defen", Cdefen);
        //                }
        //                else if (item.FirstOrDefault().Target_value < item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "人力资源部")
        //                {
        //                    decimal rte = su - (decimal)(item.FirstOrDefault().Actual_completion_value - item.FirstOrDefault().Target_value);//单项得分
        //                    output.Add("danx", rte);//单项得分
        //                    decimal Ddefen = Total = rte * de;//得分小计
        //                    output.Add("defen", Ddefen);//得分小计
        //                }
        //                else if (item.FirstOrDefault().Target_value > item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "人力资源部")
        //                {
        //                    decimal tef = su + (decimal)(item.FirstOrDefault().Target_value - item.FirstOrDefault().Actual_completion_value);//单项得分
        //                    output.Add("danx", tef);//单项得分
        //                    decimal Edefen = Total = tef * de;//得分小计
        //                    output.Add("defen", Edefen);//得分小计
        //                }
        //                else if (item.FirstOrDefault().Target_value == item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "行政后勤部")
        //                {
        //                    output.Add("danx", su);//单项得分
        //                    decimal Fdefen = Total = su * de;//得分小计
        //                    output.Add("defen", Fdefen);//得分小计
        //                }
        //                else if (item.FirstOrDefault().Target_value < item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "行政后勤部")
        //                {
        //                    decimal dt = su + (decimal)(item.FirstOrDefault().Actual_completion_value - item.FirstOrDefault().Target_value);//单项得分
        //                    output.Add("danx", dt);//单项得分
        //                    decimal Gdefen = Total = dt * de;//得分小计
        //                    output.Add("defen", Gdefen);  //得分小计                                    
        //                }
        //                else if (item.FirstOrDefault().Target_value > item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "行政后勤部")
        //                {
        //                    decimal sde = su - (decimal)(item.FirstOrDefault().Target_value - item.FirstOrDefault().Actual_completion_value);//单项得分
        //                    output.Add("danx", sde);//单项得分
        //                    decimal Hdefen = Total = sde * de;//得分小计
        //                    output.Add("defen", Hdefen); //得分小计 
        //                }
        //                Total_Points = Total_Points + Total;
        //                merge.Add(output);
        //                output = new JObject();
        //            }
        //            output1.Add("merge", merge);
        //            output1.Add("total_points", Total_Points);
        //            var ttt = db.Personnel_Ranking_management.Where(c => c.AsseDepartment == item_eng && c.DP_Group == ihte && c.Year == year && c.Month == month).FirstOrDefault();
        //            output1.Add("wj", ttt == null ? 0 : ttt.Disciplinary);
        //            output1.Add("ccl", ttt == null ? 0 : ttt.Attendance_value);
        //            output1.Add("PM", "");
        //            output1.Add("JF", "");
        //            merge = new JArray();
        //            userItem.Add(output1);
        //            output1 = new JObject();
        //        }
        //    }

        //    #region--排名计算

        //    List<Rink> rinklist = new List<Rink>();//取出所有部门、组、总分
        //    List<Rink> rinklist2 = new List<Rink>();//取出所有部门、组、总分
        //    foreach (var i in userItem)
        //    {
        //        Rink record = new Rink();
        //        record.assdepar = i["AssetDepartment"].ToString();
        //        record.dp_gur = i["DP_Group"].ToString();
        //        record.toal = Convert.ToDecimal(i["total_points"]);
        //        var re = db.Personnel_Ranking_management.Where(c => c.AsseDepartment == record.assdepar && c.DP_Group == record.dp_gur && c.Year == year && c.Month == month).FirstOrDefault();
        //        record.wj = re == null ? 0 : re.Disciplinary;
        //        record.ccl = re == null ? 0 : re.Attendance_value;
        //        record.bzrs = re == null ? 0 : re.GroupNumber;
        //        rinklist.Add(record);
        //        rinklist2.Add(record);
        //    }
        //    var rank = new JArray(userItem.OrderByDescending(c => c["total_points"]));//按总分排序
        //    //对rinklist排序：total,wj,ccl
        //    rinklist = rinklist.Where(c => c.toal >= 90 && c.bzrs >= 5).OrderByDescending(c => c.toal).ThenBy(c => c.wj).ThenByDescending(c => c.ccl).ToList();
        //    int pm_num = 1;
        //    #region---三列相同判断的排名方法
        //    for (int k = 0; k < rinklist.Count(); k++)
        //    {
        //        if (pm_num > 5) break;
        //        var it = rinklist[k];
        //        var count_three_col_same = rinklist.Where(c => c.toal == it.toal && c.wj == it.wj && c.ccl == it.ccl);
        //        if (count_three_col_same.Count() > 1)
        //        {
        //            foreach (var ii in count_three_col_same)
        //            {
        //                rinklist[k].pm = String.IsNullOrEmpty(ii.pm) ? ($"第{pm_num}名") : ii.pm;
        //                rinklist[k].jf = jfreturn(pm_num);
        //                k++;
        //            }
        //            k--;
        //        }
        //        else
        //        {
        //            rinklist[k].pm = $"第{pm_num}名";
        //            rinklist[k].jf = jfreturn(pm_num);
        //        }
        //        pm_num++;
        //    }
        //    //把排名放回汇总表
        //    foreach (var addval in rinklist)
        //    {
        //        rank.Where(c => c["AssetDepartment"].ToString() == addval.assdepar && c["DP_Group"].ToString() == addval.dp_gur).FirstOrDefault()["PM"] = addval.pm;
        //        rank.Where(c => c["AssetDepartment"].ToString() == addval.assdepar && c["DP_Group"].ToString() == addval.dp_gur).FirstOrDefault()["JF"] = addval.jf;
        //    }
        //    #endregion
        //    #region---三列相同判断的倒数排名方法
        //    rinklist2 = rinklist2.OrderBy(c => c.toal).ThenByDescending(c => c.wj).ThenBy(c => c.ccl).ToList();
        //    var re_pm_three_fu = rinklist2.Select(c => c.toal).Distinct().Take(3).LastOrDefault();//-3fs
        //    rinklist2 = rinklist2.Where(c => c.toal <= re_pm_three_fu).ToList();
        //    int pmnum = 1;
        //    for (int g = 0; g < rinklist2.Count(); g++)
        //    {
        //        if (pmnum > 3) break;
        //        var tg = rinklist2[g];
        //        var count_three = rinklist2.Where(c => c.toal == tg.toal && c.wj == tg.wj && c.ccl == tg.ccl);
        //        if (count_three.Count() > 1)
        //        {
        //            foreach (var ip in count_three)
        //            {
        //                rinklist2[g].pm = String.IsNullOrEmpty(ip.pm) ? ($"倒数第{pmnum}名") : ip.pm;
        //                rinklist2[g].jf = jfreturn(-pmnum);
        //                g++;
        //            }
        //            g--;
        //        }
        //        else
        //        {
        //            rinklist2[g].pm = $"倒数第{pmnum}名";
        //            rinklist2[g].jf = jfreturn(-pmnum);
        //        }
        //        pmnum++;
        //    }
        //    //把倒数排名放回汇总表
        //    foreach (var addlist in rinklist2)
        //    {
        //        rank.Where(c => c["AssetDepartment"].ToString() == addlist.assdepar && c["DP_Group"].ToString() == addlist.dp_gur).FirstOrDefault()["PM"] = addlist.pm;
        //        rank.Where(c => c["AssetDepartment"].ToString() == addlist.assdepar && c["DP_Group"].ToString() == addlist.dp_gur).FirstOrDefault()["JF"] = addlist.jf;
        //    }
        //    #endregion
        //    #endregion
        //    return Content(JsonConvert.SerializeObject(rank));
        //}

        //积分管理
        public string jfreturn(int t)
        {
            int s = 0;
            switch (t)
            {
                case 1:
                    s = 10;
                    break;
                case 2:
                    s = 5;
                    break;
                case 3:
                    s = 5;
                    break;
                case 4:
                    s = 2;
                    break;
                case 5:
                    s = 2;
                    break;
                case -1:
                    s = -10;
                    break;
                case -2:
                    s = -5;
                    break;
                case -3:
                    s = -2;
                    break;
            }
            return s.ToString();
        }

        #endregion

        #region---- 导出excel表格
        //汇总表实体
        public class SelectionCore
        {
            //public int Id { get; set; }
            public string AssetDepartment { get; set; }
            public string DP_Group { get; set; }
            public string Project1 { get; set; }
            public string IndexName1 { get; set; }
            public string Target_value1 { get; set; }
            public string Actual_completion_value1 { get; set; }
            public string danx1 { get; set; }
            public string defen1 { get; set; }
            public string DepartmentData1 { get; set; }
            public string Project2 { get; set; }
            public string IndexName2 { get; set; }
            public string Target_value2 { get; set; }
            public string Actual_completion_value2 { get; set; }
            public string danx2 { get; set; }
            public string defen2 { get; set; }
            public string DepartmentData2 { get; set; }
            public string Project3 { get; set; }
            public string IndexName3 { get; set; }
            public string Target_value3 { get; set; }
            public string Actual_completion_value3 { get; set; }
            public string danx3 { get; set; }
            public string defen3 { get; set; }
            public string DepartmentData3 { get; set; }
            public string Project4 { get; set; }
            public string IndexName4 { get; set; }
            public string Target_value4 { get; set; }
            public string Actual_completion_value4 { get; set; }
            public string danx4 { get; set; }
            public string defen4 { get; set; }
            public string DepartmentData4 { get; set; }
            public string total_points { get; set; }
            public string wj { get; set; }
            public string ccl { get; set; }
            public string PM { get; set; }
            public string JF { get; set; }
        }

        [HttpPost]
        public FileContentResult ExcelOutPut(int year, int month)
        {
            var json = Excellent2(year, month);
            //Json转DataTable
            // var res = ExcelExportHelper.JsonToDataTable(json);
            //DataTable转Excel表格输出
            //json转LIST
            var json1 = JsonConvert.DeserializeObject(json);
            List<SelectionCore> recordlist = new List<SelectionCore>();
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            recordlist = Serializer.Deserialize<List<SelectionCore>>(json);
            foreach (var item in recordlist)
            {
                if (item.JF == null) item.JF = "";
                if (item.PM == null) item.PM = "";
            }
            string[] columns = { "被考核部门", "班组", "项目", "指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "数据来源和提供部门", "项目", "指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "数据来源和提供部门", "项目", "指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "数据来源和提供部门", "项目", "指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "数据来源和提供部门", "总得分", "违纪记录", "出勤率", "当月排名", "当月积分", "备注" };
            var result = ExcelExportHelper.ExportExcel(recordlist, year + "年" + month + "月评优汇", false, columns);
            return File(result, ExcelExportHelper.ExcelContentType, year + "年" + month + "月评优汇总表.xlsx");
        }

        #endregion

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Quality_objectives personnel_Quality_objectives = db.Personnel_Quality_objectives.Find(id);
            if (personnel_Quality_objectives == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Quality_objectives);
        }

        #region---添加数据
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Leave", act = "Index" });
            }
            return View();
        }

        [HttpPost]

        public ActionResult Create(Personnel_Quality_objectives personnel_Quality_objectives)
        {
            if (personnel_Quality_objectives != null)
            {
                if (db.Personnel_Quality_objectives.Count(c => c.DepartmentData == personnel_Quality_objectives.DepartmentData && c.DP_Group == personnel_Quality_objectives.DP_Group && c.Project == personnel_Quality_objectives.Project && c.Year == personnel_Quality_objectives.Year && c.Month == personnel_Quality_objectives.Month) > 0)
                {
                    return Content("已有重复数据，请重新输入数据！");
                }
                personnel_Quality_objectives.CreateDate = DateTime.Now;
                personnel_Quality_objectives.Creator = ((Users)Session["user"]).UserName;
                db.Personnel_Quality_objectives.Add(personnel_Quality_objectives);
                db.SaveChanges();
                return Content("true");
            }
            return Content("新增出错，请确认数据是否规范！");
        }
        #endregion

        #region----批量添加数据
        public ActionResult Batch_InputStaff()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Quality_objectives", act = "Batch_InputStaff" });
            }
            return View();
        }

        [HttpPost]
        public ActionResult Batch_InputStaff(List<Personnel_Quality_objectives> inputList)
        {
            string repeat = null;
            foreach (var item in inputList)
            {
                if (db.Personnel_Quality_objectives.Count(c => c.DepartmentData == item.DepartmentData && c.AssetDepartment == item.AssetDepartment && c.DP_Group == c.DP_Group && c.Project == item.Project && c.IndexName == item.IndexName && c.Year == item.Year && c.Month == item.Month) > 0)
                    repeat = repeat + item.AssetDepartment + item.DP_Group + item.Project + item.IndexName + item.DepartmentData + ",";
            }
            if (!string.IsNullOrEmpty(repeat))
            {
                return Content(repeat + "已经有相同的数据，请重新输入");
            }
            if (ModelState.IsValid)
            {
                db.Personnel_Quality_objectives.AddRange(inputList);
                db.SaveChanges();
                return Content("true");
            }
            return Content("false");
        }
        #endregion

        #region-----修改方法
        public ActionResult Edit(Personnel_Quality_objectives personnel_Quality_objectives)
        {
            if (personnel_Quality_objectives != null)
            {
                personnel_Quality_objectives.ModifyTime = DateTime.Now;
                personnel_Quality_objectives.ModifyName = ((Users)Session["User"]).UserName;
                db.Entry(personnel_Quality_objectives).State = EntityState.Modified;
                db.SaveChangesAsync();
                return Content("true");
            }
            return Content("修改出错，请确认数据是否正确");
        }
        #endregion

        #region---删除方法
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Quality_objectives personnel_Quality_objectives = db.Personnel_Quality_objectives.Find(id);
            if (personnel_Quality_objectives == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Quality_objectives);
        }

        // POST: Personnel_Quality_objectives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Personnel_Quality_objectives personnel_Quality_objectives = db.Personnel_Quality_objectives.Find(id);
            db.Personnel_Quality_objectives.Remove(personnel_Quality_objectives);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region------保存出勤违纪记录表数据表信息
        public ActionResult Ranking_Points()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Ranking_Points(List<Personnel_Ranking_management> rankList)
        {
            string repeat = null;
            foreach (var item in rankList)
            {
                if (db.Personnel_Ranking_management.Count(c => c.AsseDepartment == item.AsseDepartment && c.DP_Group == c.DP_Group && c.Year == item.Year && c.Month == item.Month) > 0)
                    repeat = repeat + item.AsseDepartment + item.DP_Group + item.Year + item.Month + ",";
            }
            if (!string.IsNullOrEmpty(repeat))
            {
                return Content(repeat + "已经有相同的数据，请重新输入");
            }
            if (ModelState.IsValid)
            {
                db.Personnel_Ranking_management.AddRange(rankList);
                db.SaveChangesAsync();
                return Content("true");
            }
            return Content("false");
        }
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
