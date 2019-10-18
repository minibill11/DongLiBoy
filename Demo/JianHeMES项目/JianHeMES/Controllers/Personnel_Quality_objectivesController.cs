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
            var Appraising = db.Personnel_Quality_objectives.Where(c => c.Year == year && c.Month == month).ToList();
            var department = Appraising.Select(c => c.AssetDepartment).Distinct().ToList();
            List<Rink> ment = new List<Rink>();
            foreach (var item_eng in department)
            {
                var dp_ge = Appraising.Where(c => c.AssetDepartment == item_eng).Select(c => c.DP_Group).Distinct().ToList();
                foreach (var ihte in dp_ge)
                {
                    var peoject = Appraising.Where(c => c.AssetDepartment == item_eng && c.DP_Group == ihte).Select(c => c.Project).Distinct().ToList();
                    var item1 = Appraising.Where(c => c.AssetDepartment == item_eng && c.DP_Group == ihte).ToList();
                    //ID
                    output1.Add("Id", item1.FirstOrDefault().Id);
                    //被考核部门
                    output1.Add("AssetDepartment", item1.FirstOrDefault().AssetDepartment);
                    //班组
                    output1.Add("DP_Group", item1.FirstOrDefault().DP_Group);
                    JArray merge = new JArray();
                    decimal Total_Points = 0;
                    foreach (var item_peoject in peoject)
                    {
                        JObject output = new JObject();
                        var item = Appraising.Where(c => c.AssetDepartment == item_eng && c.DP_Group == ihte && c.Project == item_peoject).ToList();
                        //项目
                        output.Add("Project", item.FirstOrDefault().Project);
                        //指标名称
                        output.Add("IndexName", item.FirstOrDefault().IndexName);
                        //目标值
                        output.Add("Target_value", item.FirstOrDefault().Target + item.FirstOrDefault().Target_value + item.FirstOrDefault().Target2);
                        //实际值
                        output.Add("Actual_completion_value", item.FirstOrDefault().Actual_completion_value + item.FirstOrDefault().Actua1);
                        string ikge = "";
                        //考核部门
                        foreach (var it in item)
                        {
                            ikge = ikge + it.DepartmentData + ",";
                        }
                        output.Add("DepartmentData", ikge);
                        decimal su = 100;
                        decimal Total = 0;
                        var project = item.FirstOrDefault().Project;
                        var de = db.Personnel_Assessment_indicators.Where(c => c.indName == project).Select(c => c.Price).FirstOrDefault();

                        if (item.FirstOrDefault().Target_value <= item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData != "人力资源部" && item.FirstOrDefault().DepartmentData != "行政后勤部")
                        {
                            int danx = 0;
                            output.Add("danx", danx);//单项得分
                            decimal dexfen = Total = danx * de;
                            output.Add("defen", dexfen);//得分小计
                        }
                        else if (item.FirstOrDefault().Target_value > item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData != "人力资源部" && item.FirstOrDefault().DepartmentData != "行政后勤部")
                        {
                            output.Add("danx", su);//单项得分
                            decimal Bdefen = Total = su * de;
                            output.Add("defen", Bdefen);//得分小计
                        }
                        else if (item.FirstOrDefault().Target_value == item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "人力资源部")
                        {
                            output.Add("danx", su);
                            decimal Cdefen = Total = su * de;
                            output.Add("defen", Cdefen);
                        }
                        else if (item.FirstOrDefault().Target_value < item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "人力资源部")
                        {
                            decimal rte = su - (decimal)(item.FirstOrDefault().Actual_completion_value - item.FirstOrDefault().Target_value);//单项得分
                            output.Add("danx", rte);//单项得分
                            decimal Ddefen = Total = rte * de;//得分小计
                            output.Add("defen", Ddefen);//得分小计
                        }
                        else if (item.FirstOrDefault().Target_value > item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "人力资源部")
                        {
                            decimal tef = su + (decimal)(item.FirstOrDefault().Target_value - item.FirstOrDefault().Actual_completion_value);//单项得分
                            output.Add("danx", tef);//单项得分
                            decimal Edefen = Total = tef * de;//得分小计
                            output.Add("defen", Edefen);//得分小计
                        }
                        else if (item.FirstOrDefault().Target_value == item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "行政后勤部")
                        {
                            output.Add("danx", su);//单项得分
                            decimal Fdefen = Total = su * de;//得分小计
                            output.Add("defen", Fdefen);//得分小计
                        }
                        else if (item.FirstOrDefault().Target_value < item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "行政后勤部")
                        {
                            decimal dt = su + (decimal)(item.FirstOrDefault().Actual_completion_value - item.FirstOrDefault().Target_value);//单项得分
                            output.Add("danx", dt);//单项得分
                            decimal Gdefen = Total = dt * de;//得分小计
                            output.Add("defen", Gdefen);  //得分小计                                    
                        }
                        else if (item.FirstOrDefault().Target_value > item.FirstOrDefault().Actual_completion_value && item.FirstOrDefault().DepartmentData == "行政后勤部")
                        {
                            decimal sde = su - (decimal)(item.FirstOrDefault().Target_value - item.FirstOrDefault().Actual_completion_value);//单项得分
                            output.Add("danx", sde);//单项得分
                            decimal Hdefen = Total = sde * de;//得分小计
                            output.Add("defen", Hdefen); //得分小计 
                        }
                        Total_Points = Total_Points + Total;
                        merge.Add(output);
                        output = new JObject();
                    }
                    output1.Add("merge", merge);
                    output1.Add("total_points", Total_Points);
                    output1.Add("PM", "");
                    output1.Add("JF", "");
                    merge = new JArray();
                    userItem.Add(output1);
                    output1 = new JObject();
                }
            }
            #region--排名计算
            List<Rink> rinklist = new List<Rink>();//取出所有部门、组、总分
            foreach (var i in userItem)
            {
                Rink record = new Rink();
                record.assdepar = i["AssetDepartment"].ToString();
                record.dp_gur = i["DP_Group"].ToString();
                record.toal = Convert.ToDecimal(i["total_points"]);
                var re = db.Personnel_Ranking_management.Where(c => c.AsseDepartment == record.assdepar && c.DP_Group == record.dp_gur && c.Year == year && c.Month == month).FirstOrDefault();
                record.wj = re==null?0:re.Disciplinary;
                record.ccl = re==null?0:re.Attendance_value;
                record.bzrs = re.GroupNumber;
                rinklist.Add(record);
            }
            var rank = new JArray(userItem.OrderByDescending(c => c["total_points"]));//按总分排序
            //对rinklist排序：total,wj,ccl
            rinklist = rinklist.Where(c=>c.toal>=90 && c.bzrs>=5).OrderByDescending(c => c.toal).ThenBy(c => c.wj).ThenByDescending(c => c.ccl).ToList();
            int pm_num=1;
            foreach(var i in rinklist)//一个一个算
            {
                var same_total = rinklist.Where(c => c.toal == i.toal);//总分相同的记录
                if(same_total.Count()>1)
                {
                    var same_wj = same_total.Where(c => c.wj == i.wj).OrderBy(c=>c.wj);//总分相同违纪相同的记录
                    if(same_wj.Count()>1)
                    {
                        var same_ccl = same_wj.Where(c => c.ccl == i.ccl).OrderByDescending(c=>c.ccl);
                        if(same_ccl.Count()>1)
                        {
                            foreach (var j in same_wj)
                            {
                                j.pm = "第" + pm_num + "名";
                                j.jf = jfreturn(pm_num);
                            }
                            pm_num = pm_num+same_ccl.Count();
                        }
                        else
                        {
                            foreach (var j in same_wj)
                            {
                                j.pm = "第" + pm_num + "名";
                                j.jf = jfreturn(pm_num);
                                pm_num++;
                            }
                        }
                    }
                    else
                    {
                        foreach(var j in same_wj)
                        {
                            j.pm = "第" + pm_num + "名";
                            j.jf = jfreturn(pm_num);
                            pm_num++;
                        }
                    }
                }
                else
                {
                    i.pm="第"+pm_num+"名";
                    i.jf = jfreturn(pm_num);
                }
            }
            foreach(var addval in rinklist)
            {
                rank.Where(c => c["AssetDepartment"].ToString() == addval.assdepar && c["DP_Group"].ToString() == addval.dp_gur).FirstOrDefault()["PM"] = addval.pm;
                rank.Where(c => c["AssetDepartment"].ToString() == addval.assdepar && c["DP_Group"].ToString() == addval.dp_gur).FirstOrDefault()["JF"] = addval.jf;
            }

            #region--以前排法

            ////需要排除<5人的班组
            //var excep = db.Personnel_Ranking_management.Where(c => c.GroupNumber < 5).Select(c => new { c.AsseDepartment, c.DP_Group, c.GroupNumber }).ToList();
            //foreach (var i in excep)
            //{
            //    var exlist = rinklist.Where(c => c.assdepar.Contains(i.AsseDepartment) && c.dp_gur.Contains(i.DP_Group)).ToList();
            //    rinklist.Except(exlist);
            //}
            //var rank = new JArray(userItem.OrderByDescending(c => c["total_points"]));//按总分排序
            //var pmm = rinklist.OrderBy(c => c.toal).Select(c => c.toal).Distinct().Where(c => c >= 90).Take(5).ToList();//取出前5名且大于90的分数数组
            //int t = 1;//名次
            //foreach (var p in pmm)
            //{
            //    int count = rank.Count(c => Convert.ToDecimal(c["total_points"]) == p);
            //    if (count > 1)
            //    {
            //        var list = rank.Where(c => Convert.ToDecimal(c["total_points"]) == p).ToList();
            //        //相关违纪记录
            //        List<Personnel_Ranking_management> recordlist = new List<Personnel_Ranking_management>();
            //        foreach (var item in list)//默认1条
            //        {
            //            //string abc = item[0]["AsseDepartment"].ToString();
            //            recordlist.AddRange(db.Personnel_Ranking_management.Where(c => c.AsseDepartment == item["AsseDepartment"].ToString() && c.DP_Group == item["DP_Group"].ToString() && c.Year == year && c.Month == month));
            //        }
            //        recordlist = recordlist.OrderBy(c => c.Disciplinary).ToList();//
            //        foreach (var i in recordlist)
            //        {
            //            int first_count = recordlist.Count(c => c.Disciplinary == recordlist.FirstOrDefault().Disciplinary);
            //            if (first_count == 1)
            //            {
            //                rank.Where(c => c["AssetDepartment"].ToString() == recordlist.FirstOrDefault().AsseDepartment && c["DP_Group"].ToString() == recordlist.FirstOrDefault().DP_Group && Convert.ToDecimal(c["total_points"]) == p).FirstOrDefault()["PM"] = "第" + t + "名";
            //                t++;
            //                recordlist.RemoveAt(0);
            //                recordlist = recordlist.OrderBy(c => c.Disciplinary).ToList();
            //            }
            //            else
            //            {
            //                //违纪数量相同，比较出勤率
            //                List<Personnel_Ranking_management> recordlist2 = recordlist.Where(c => c.Disciplinary == recordlist.FirstOrDefault().Disciplinary).ToList();
            //                int first_count2 = recordlist.Count(c => c.Disciplinary == recordlist2.FirstOrDefault().Disciplinary);
            //                if (first_count2 == 1)
            //                {
            //                    rank.Where(c => c["AssetDepartment"].ToString() == recordlist2.FirstOrDefault().AsseDepartment && c["DP_Group"].ToString() == recordlist2.FirstOrDefault().DP_Group && Convert.ToDecimal(c["total_points"]) == p).FirstOrDefault()["PM"] = "第" + t + "名";
            //                    t++;
            //                    recordlist.RemoveAt(0);
            //                    recordlist = recordlist.OrderBy(c => c.Disciplinary).ToList();
            //                }
            //                else
            //                {
            //                    foreach (var tt in recordlist2)
            //                    {
            //                        rank.Where(c => c["AssetDepartment"].ToString() == recordlist2.FirstOrDefault().AsseDepartment && c["DP_Group"].ToString() == recordlist2.FirstOrDefault().DP_Group && Convert.ToDecimal(c["total_points"]) == p).FirstOrDefault()["PM"] = "第" + t + "名";
            //                    }
            //                    t = t + recordlist2.Count;
            //                }
            //                recordlist.Except(recordlist2);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        rank.Where(c => Convert.ToDecimal(c["total_points"]) == p).FirstOrDefault()["PM"] = "第" + t + "名";
            //        rank.Where(c => Convert.ToDecimal(c["total_points"]) == p).FirstOrDefault()["JF"] = jfreturn(t);
            //        t++;
            //    }
            //}
            //var rinklist2 = rinklist.OrderBy(c=>c.toal).Take(3);//按总分排序
            //int k = -1;
            //List<decimal> jflist = rinklist2.Select(c => c.toal).Distinct().ToList();
            //foreach(var ii in jflist)
            //{
            //    var tt = rank.Where(c => Convert.ToDecimal(c["total_points"]) == ii);
            //    foreach(var t1 in tt)
            //    {
            //        t1["PM"] = "倒数第" + Math.Abs(k) + "名";
            //        t1["JF"] = jfreturn(k).ToString();
            //    }
            //    k = k - tt.Count();
            //}
            #endregion

            #endregion
            return Content(JsonConvert.SerializeObject(rank));
        }

        #endregion
        //积分管理
        public string jfreturn(int t)
        {
            int s= 0;
            switch(t)
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

        ////导出excel表格
        //public ActionResult Excellent(int year, int month)
        //{
        //    //Json转DataTable

        //    //DataTable转Excel表格输出

        //    return File(year+"年"+month+"月",Excel);
        //}
        
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
