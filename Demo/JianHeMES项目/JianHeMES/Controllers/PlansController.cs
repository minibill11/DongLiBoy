﻿using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class PlansController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: KPI
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Plans", act = "Index" });
            }
            return View();
        }
        public ActionResult Section_Enter()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Plans", act = "Section_Enter" });
            }
            return View();
        }

        // GET: KPI/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: KPI/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KPI/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: KPI/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: KPI/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: KPI/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: KPI/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #region 工段工序功能集合
        //显示工段工序
        public ActionResult DisplayPlanSectionParameter()
        {
            JArray result = new JArray();
            var value = db.Plan_SectionParameter.ToList();
            foreach (var item in value)
            {
                JObject obj = new JObject();
                obj.Add("id", item.Id);
                obj.Add("Process", item.Process);
                obj.Add("Section", item.Section);
                obj.Add("Table", item.Table);
                result.Add(obj);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //输入工段工序
        public ActionResult AddPlanSectionParameter(string Process, string Section)
        {
            JObject obj = new JObject();
            
            var info = db.Plan_SectionParameter.Count(c => c.Process == Process && c.Section == Section);
            if (info != 0)
            {
                obj.Add("mes", "已有重复工段工序");
                obj.Add("id", null);
                obj.Add("Process", null);
                obj.Add("Section", null);
                obj.Add("Table", null);
                return Content(JsonConvert.SerializeObject(obj));
            }
            Plan_SectionParameter parameter = new Plan_SectionParameter() { Process = Process, Section = Section, Createor = ((Users)Session["User"]) == null ? "测试人员" : ((Users)Session["User"]).UserName, CreateTime = DateTime.Now };
            db.Plan_SectionParameter.Add(parameter);
            db.SaveChanges();

            var item = db.Plan_SectionParameter.Where(c => c.Process == Process && c.Section == Section).FirstOrDefault();
            obj.Add("mes", "新增成功");
            obj.Add("id", item.Id);
            obj.Add("Process", item.Process);
            obj.Add("Section", item.Section);
            obj.Add("Table", item.Table);
            return Content(JsonConvert.SerializeObject(obj));
        }

        //修改工段工序
        public string UpdatePlanSectionParameter(int id,string Process, string Section,string Table=null)
        {
            var info = db.Plan_SectionParameter.Find(id);
            if (info == null)
            {
                return "找不到改信息记录";
            }
            //判断是否有重复的记录
            var check = db.Plan_SectionParameter.Count(c => c.Process == Process && c.Section == Section);
            if (check != 0)
            {
                return "已有重复记录";
            }
            //判断指标清单表是否有记录
            var info2 = db.KPI_Indicators.Where(c => c.Process == info.Process && c.Section == info.Section).ToList();

            info.Process = Process;
            info.Section = Section;
            info.Table = Table;

            info2.ForEach(c => {
                c.Section = Section;
                c.Process = Process;
                var temp= (c.ActualSQL.Substring(c.ActualSQL.LastIndexOf("from") + 5)).Split(' ');
                c.ActualSQL.Replace(temp[0], Table);
            });
            db.SaveChanges();
            return "true";
        }

        //删除工段工序
        public string DeletePlanSectionParameter(int id)
        {
            var info = db.Plan_SectionParameter.Find(id);
            if (info == null)
            {
                return "找不到改信息记录";
            }
            db.Plan_SectionParameter.Remove(info);
            db.SaveChanges();
            return "true";
        }
        #endregion

        #region 各工段工序计划录入
        //显示
        public ActionResult DisplayPlan_FromKPI(string ordernum, string deparment, string group, string process, string section, DateTime? time)
        {
            JArray result = new JArray();
            var totalvalue = db.Plan_FromKPI.ToList();
            if (!string.IsNullOrEmpty(ordernum))
            {
                totalvalue = totalvalue.Where(c => c.OrderNum == ordernum).ToList();
            }
            if (!string.IsNullOrEmpty(deparment))
            {
                totalvalue = totalvalue.Where(c => c.Department == deparment).ToList();
            }
            if (!string.IsNullOrEmpty(group))
            {
                totalvalue = totalvalue.Where(c => c.Group == group).ToList();
            }
            if (!string.IsNullOrEmpty(process))
            {
                totalvalue = totalvalue.Where(c => c.Process == process).ToList();
            }
            if (!string.IsNullOrEmpty(section))
            {
                totalvalue = totalvalue.Where(c => c.Section == section).ToList();
            }

            if (time != null)
            {
                totalvalue = totalvalue.Where(c => c.PlanTime == time).ToList();
            }
            foreach (var item in totalvalue)
            {
                JObject obj = new JObject();
                obj.Add("OrderNum", item.OrderNum); //订单
                obj.Add("Department", item.Department);//部门
                obj.Add("Group", item.Group);//班组
                obj.Add("Process", item.Process);//工段
                obj.Add("Section", item.Section);//工序
                obj.Add("IndicatorsType", item.IndicatorsType);//品质或者效率
                obj.Add("CheckDepartment", item.CheckDepartment);//检验部门
                obj.Add("CheckGroup", item.CheckGroup);//检验班组
                obj.Add("CheckType", item.CheckType);//抽检或全检
                obj.Add("PlanTime", item.PlanTime);//计划时间
                obj.Add("PlanNum", item.PlanNum);//计划数量
                result.Add(obj);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //新增
        public string AddPlan_FromKPI(Plan_FromKPI Record)
        {
            var info = db.Plan_FromKPI.Count(c => c.PlanTime == Record.PlanTime && c.OrderNum == Record.OrderNum && c.Department == Record.Department && c.Group == Record.Group && c.Process == Record.Process && c.Section == Record.Section);
            if (info != 0)
            {
                return "已有重复记录";

            }
            Record.PlanCreateor = ((Users)Session["User"]) == null ? "张三" : ((Users)Session["User"]).UserName;
            Record.PlanCreateTime = DateTime.Now;
            db.Plan_FromKPI.Add(Record);
            db.SaveChanges();
            return "true";

        }
        //修改
        public string UpdatePlan_FromKPI(int id, Plan_FromKPI Record)
        {
            var info = db.Plan_FromKPI.Find(id);
            if (info == null)
            {
                return "没有找到数据";
            }
            info.OrderNum = Record.OrderNum;
            info.Department = Record.Department;
            info.Group = Record.Group;
            info.Process = Record.Process;
            info.Section = Record.Section;
            info.IndicatorsType = Record.IndicatorsType;
            info.CheckDepartment = Record.CheckDepartment;
            info.CheckGroup = Record.CheckGroup;
            info.CheckType = Record.CheckType;
            info.PlanTime = Record.PlanTime;
            info.PlanNum = Record.PlanNum;

            db.SaveChanges();
            return "true";

        }

        //删除

        public string DeletePlan_FromKPI(int id)
        {
            var info = db.Plan_FromKPI.Find(id);
            if (info == null)
            {
                return "找不到该信息";
            }
            db.Plan_FromKPI.Remove(info);
            db.SaveChanges();
            return "true";
        }
        #endregion
    }
}
