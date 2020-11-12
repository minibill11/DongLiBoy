﻿using JianHeMES.AuthAttributes;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using static JianHeMES.Controllers.CommonalityController;

namespace JianHeMES.Controllers
{
    public class KPIController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController commom = new CommonController();
        // GET: KPI
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "Index" });
            }
            return View();
        }

        public ActionResult KPI_Index_Month()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_Index_Month" });
            }
            return View();
        }
        public ActionResult KPI_Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_Index" });
            }
            return View();
        }
        public ActionResult KPI_Index_History()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_Index_History" });
            }
            return View();
        }
        public ActionResult KPI_Daily()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "KPI", act = "KPI_Daily" });
            }
            return View();
        }
        public ActionResult KPI_Inspection()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "KPI", act = "KPI_Inspection" });
            }
            return View();
        }
        public ActionResult KPI_StemFrom()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "KPI", act = "KPI_StemFrom" });
            }
            return View();
        }
        public ActionResult KPI_Ranking()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "KPI", act = "KPI_Ranking" });
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
        #region 直通率
        public class Temp
        {
            public string Name { get; set; }
            public string OrderNum { get; set; }
            public string JobContent { get; set; }
            public string BarCodesNum { get; set; }
            public string Group { get; set; }
            public bool Finish { get; set; }
            public int Passcount { get; set; }
            public int AbnormalCount { get; set; }
        }
        //各工序直通率查询首页
        public ActionResult GetPassThrough()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "GetPassThrough" });
            }
            return View();
        }
        public ActionResult GetPassThrough2(string type, int year, int mouth, string ordernum)
        {
            var orderlist = new List<string>();
            if (!string.IsNullOrEmpty(ordernum))
            {
                orderlist.Add(ordernum);
            }
            else
            {
                var info = db.OrderMgm.Where(c => c.PlatformType == type).ToList();

                if (year != 0)
                {
                    info = info.Where(c => c.ContractDate.Year == year).ToList();
                }
                if (mouth != 0)
                {
                    info = info.Where(c => c.ContractDate.Month == mouth).ToList();

                }
                orderlist = info.Select(c => c.OrderNum).ToList();
            }
            JArray total = new JArray();
            JArray array = new JArray();
            JObject result = new JObject();
            result.Add("seaction", "模块段");
            //smt直通率
            var smt = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum)).Select(c => c.JobContent).Distinct().ToList();
            foreach (var jobcontent in smt)
            {
                JObject content = new JObject();
                var linenumlist = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum) && c.JobContent == jobcontent).Select(c => new Temp { Group = c.LineNum.ToString(), Passcount = c.NormalCount, AbnormalCount = c.AbnormalCount, Name = "SMT" }).Distinct().ToList();
                content = GetPassThroughitem(jobcontent, linenumlist);
                array.Add(content);
            }
            //AI直通率
            var ai = db.ModuleAI.Where(c => orderlist.Contains(c.Ordernum)).Select(c => new Temp { Group = c.Machine, Finish = c.IsAbnormal, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("AI", ai));
            //后焊直通率
            var group = db.AfterWelding.Where(c => orderlist.Contains(c.Ordernum)).Select(c => new Temp { Group = c.Group, Finish = c.IsAbnormal, BarCodesNum = c.ModuleBarcode }).ToList();

            array.Add(GetPassThroughitem("后焊", group));
            //后焊抽检直通率
            var groupSampl = db.ModuleSampling.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "后焊").Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("后焊抽检", groupSampl));
            //灌胶前电测
            var Electric1 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "灌胶前电检").Select(c => new Temp { Group = c.Group, Finish = c.ElectricInspectionResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("灌胶前电检", Electric1));
            //模块电测
            var Electric2 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "模块电检").Select(c => new Temp { Group = c.Group, Finish = c.ElectricInspectionResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("模块电检", Electric2));
            //模块送检
            var Electric3 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "模块电检" && c.IsSampling == true && c.ElectricInspectionResult == true).Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("模块送检", Electric3));
            //模块老化
            var burn = db.ModuleBurnIn.Where(c => orderlist.Contains(c.Ordernum) && c.BurnInEndTime != null).Select(c => new Temp { Group = c.Group, Finish = c.BurninResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("模块老化", burn));
            //模块老化送检
            var burnSampl = db.ModuleBurnIn.Where(c => orderlist.Contains(c.Ordernum) && c.BurnInEndTime != null && c.IsSampling == true && c.BurninResult == true
            ).Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("老化送检", burnSampl));
            //外观电检
            var Electric4 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "外观电检").Select(c => new Temp { Group = c.Group, Finish = c.ElectricInspectionResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("外观电检", Electric4));

            result.Add("content", array);
            total.Add(result);
            result = new JObject();
            array = new JArray();
            result.Add("seaction", "模组段");
            //组装直通率
            var assemblessgrou = db.Assemble.Where(c => orderlist.Contains(c.OrderNum) && c.PQCCheckFT != null && c.RepetitionPQCCheck == false).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BoxBarCode, Finish = c.PQCCheckFinish }).ToList();
            array.Add(GetPassThroughitem("组装", assemblessgrou));

            //FQC直通率
            var fqcgroup = db.FinalQC.Where(c => orderlist.Contains(c.OrderNum) && c.FQCCheckFT != null && c.RepetitionFQCCheck == false).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.FQCCheckFinish }).ToList();
            array.Add(GetPassThroughitem("FQC", fqcgroup));


            //老化直通率
            var burngroup = db.Burn_in.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.Burn_in_OQCCheckAbnormal == "正常" ? true : false }).ToList();
            array.Add(GetPassThroughitem("老化", burngroup));
            //校正直通率
            var calibgroup = db.CalibrationRecord.Where(c => orderlist.Contains(c.OrderNum) && c.FinishCalibration != null && c.RepetitionCalibration == false).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.Normal }).ToList();
            array.Add(GetPassThroughitem("校正", calibgroup));
            //外观直通率
            var appearangroup = db.Appearance.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.OQCCheckFinish }).ToList();
            array.Add(GetPassThroughitem("外观", appearangroup));
            result.Add("content", array);
            total.Add(result);

            return Content(JsonConvert.SerializeObject(total));
        }

        public JObject GetPassThroughitem(string name, List<Temp> temps)
        {
            JObject result = new JObject();
            result.Add("title", name);
            JArray array = new JArray();
            List<string> groupitem = temps.Select(c => c.Group).Distinct().ToList();
            groupitem = groupitem.OrderBy(c => c).ToList();
            foreach (var item in groupitem)
            {
                JObject obj = new JObject();
                if (temps.Count(c => c.Name == "SMT") != 0)
                {
                    obj.Add("group", item + "线");
                    var totalinfo = temps.Where(c => c.Group == item).ToList();
                    var totalNum = totalinfo.Sum(c => c.AbnormalCount) + totalinfo.Sum(c => c.Passcount);
                    //直通率
                    var info = temps.Where(c => c.Group == item).Select(c => c.Passcount).ToList();
                    var passtrough = info.Sum();
                    obj.Add("passThrough", totalNum == 0 ? "-%" : Math.Round((double)passtrough * 100 / totalNum, 2) + "%");

                    //异常率
                    var info2 = temps.Where(c => c.Group == item).Select(c => c.AbnormalCount).ToList();
                    var abnormal = info2.Sum();
                    obj.Add("abnormal", totalNum == 0 ? "-%" : Math.Round((double)abnormal * 100 / totalNum, 2) + "%");

                    array.Add(obj);
                }
                else if (item == "")
                    continue;
                else if (string.IsNullOrEmpty(item))
                {
                    obj.Add("group", "没有班组");
                    int totalNum = temps.Where(c => c.Group == item || c.Group == "").Select(c => c.BarCodesNum).Distinct().Count();
                    //合格率
                    var abnormal = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == false).Select(c => c.BarCodesNum).ToList();

                    //直通率
                    var pass = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == true).Count();

                    var info = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == true && !abnormal.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).Distinct().Count();
                    //var passtrough = info.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var abno = abnormal.Distinct().Count();
                    obj.Add("passThrough", totalNum == 0 ? "-%" : Math.Round((double)info * 100 / totalNum, 2) + "%");
                    obj.Add("abnormal", totalNum == 0 ? "-%" : Math.Round((double)abno * 100 / totalNum, 2) + "%");

                    array.Add(obj);
                }
                else
                {
                    obj.Add("group", item);
                    int totalNum = temps.Where(c => c.Group == item).Select(c => c.BarCodesNum).Distinct().Count(); ;
                    //异常率
                    var abnormal = temps.Where(c => (c.Group == item) && c.Finish == false).Select(c => c.BarCodesNum).ToList();

                    var pass = temps.Where(c => (c.Group == item) && c.Finish == true).Count();

                    //直通率
                    var info = temps.Where(c => c.Group == item && c.Finish == true && !abnormal.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).Distinct().Count();
                    //var passtrough = info.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    obj.Add("passThrough", totalNum == 0 ? "-%" : Math.Round((double)info * 100 / totalNum, 2) + "%");
                    var abno = abnormal.Distinct().Count();
                    obj.Add("abnormal", totalNum == 0 ? "-%" : Math.Round((double)abno * 100 / totalNum, 2) + "%");

                    array.Add(obj);
                }
            }
            if (temps.Count(c => c.Name == "SMT") != 0)
            {
                //合计直通率
                JObject totalobj = new JObject();
                totalobj.Add("group", "合计");
                var total = temps.Sum(c => c.AbnormalCount) + temps.Sum(c => c.Passcount);
                var totalpasstrough = temps.Sum(c => c.Passcount);
                var totalabnormal = temps.Sum(c => c.AbnormalCount);

                totalobj.Add("passThrough", temps.Count == 0 ? "-%" : Math.Round((double)totalpasstrough * 100 / total, 2) + "%");

                //合计异常率
                totalobj.Add("abnormal", temps.Count == 0 ? "-%" : Math.Round((double)totalabnormal * 100 / total, 2) + "%");
                array.Add(totalobj);
            }
            else
            {
                //合计直通率
                JObject totalobj = new JObject();
                totalobj.Add("group", "合计");
                var totalabnormal = temps.Where(c => c.Finish == false).Select(c => c.BarCodesNum).ToList();
                var totalpasstrough = temps.Where(c => c.Finish == true && !totalabnormal.Contains(c.BarCodesNum)).GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                var totalpasstrough2 = temps.Where(c => c.Finish == true && !totalabnormal.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).Distinct().Count();
                var totalCount = temps.Select(c => c.BarCodesNum).Distinct().Count();
                totalobj.Add("passThrough", temps.Count == 0 ? "-%" : Math.Round((double)totalpasstrough2 * 100 / totalCount, 2) + "%");
                var pass = temps.Where(c => c.Finish == true).Count();
                var abno = totalabnormal.Distinct().Count();
                //合计异常率
                totalobj.Add("abnormal", temps.Count == 0 ? "-%" : Math.Round((double)abno * 100 / totalCount, 2) + "%");
                array.Add(totalobj);
            }
            result.Add("array", array);
            return result;
        }

        //各工序直通率查询
        public ActionResult ChartGetPassThrough(string type, int year, int mouth, string ordernum)
        {
            var orderlist = new List<string>();
            if (!string.IsNullOrEmpty(ordernum))
            {
                orderlist.Add(ordernum);
            }
            else
            {
                var info = db.OrderMgm.Where(c => c.PlatformType == type).ToList();

                if (year != 0)
                {
                    info = info.Where(c => c.ContractDate.Year == year).ToList();
                }
                if (mouth != 0)
                {
                    info = info.Where(c => c.ContractDate.Month == mouth).ToList();

                }
                orderlist = info.Select(c => c.OrderNum).ToList();
            }
            JArray total = new JArray();
            JArray array = new JArray();
            JObject result = new JObject();
            //smt直通率
            var smt = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum)).Select(c => c.JobContent).Distinct().ToList();
            foreach (var jobcontent in smt)
            {
                JObject content = new JObject();
                var linenumlist = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum) && c.JobContent == jobcontent).Select(c => new Temp { Group = c.LineNum.ToString(), Passcount = c.NormalCount, AbnormalCount = c.AbnormalCount, Name = "SMT" }).Distinct().ToList();
                content = ChartGetPassThroughitem(jobcontent, linenumlist);
                array.Add(content);
            }
            //AI直通率
            var ai = db.ModuleAI.Where(c => orderlist.Contains(c.Ordernum)).Select(c => new Temp { Group = c.Machine, Finish = c.IsAbnormal, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("AI", ai));
            //后焊直通率
            var group = db.AfterWelding.Where(c => orderlist.Contains(c.Ordernum)).Select(c => new Temp { Group = c.Group, Finish = c.IsAbnormal, BarCodesNum = c.ModuleBarcode }).ToList();

            array.Add(ChartGetPassThroughitem("后焊", group));
            //后焊抽检直通率
            var groupSampl = db.AfterWelding.Where(c => orderlist.Contains(c.Ordernum) && c.IsSampling == true).Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("后焊抽检", groupSampl));
            //灌胶前电测
            var Electric1 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "灌胶前电检").Select(c => new Temp { Group = c.Group, Finish = c.ElectricInspectionResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("灌胶前电检", Electric1));
            //模块电测
            var Electric2 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "模块电检").Select(c => new Temp { Group = c.Group, Finish = c.ElectricInspectionResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("模块电检", Electric2));
            //模块送检
            var Electric3 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "模块电检" && c.IsSampling == true && c.ElectricInspectionResult == true).Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("模块送检", Electric3));
            //模块老化
            var burn = db.ModuleBurnIn.Where(c => orderlist.Contains(c.Ordernum) && c.BurnInEndTime != null).Select(c => new Temp { Group = c.Group, Finish = c.BurninResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("模块老化", burn));
            //模块老化送检
            var burnSampl = db.ModuleBurnIn.Where(c => orderlist.Contains(c.Ordernum) && c.BurnInEndTime != null && c.IsSampling == true && c.BurninResult == true
            ).Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("老化送检", burnSampl));
            //外观电检
            var Electric4 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "外观电检").Select(c => new Temp { Group = c.Group, Finish = c.ElectricInspectionResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("外观电检", Electric4));

            result.Add("MK", array);
            array = new JArray();
            //组装直通率
            var assemblessgrou = db.Assemble.Where(c => orderlist.Contains(c.OrderNum) && c.PQCCheckFT != null && c.RepetitionPQCCheck == false).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BoxBarCode, Finish = c.PQCCheckFinish }).ToList();
            array.Add(ChartGetPassThroughitem("组装", assemblessgrou));

            //FQC直通率
            var fqcgroup = db.FinalQC.Where(c => orderlist.Contains(c.OrderNum) && c.FQCCheckFT != null && c.RepetitionFQCCheck == false).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.FQCCheckFinish }).ToList();
            array.Add(ChartGetPassThroughitem("FQC", fqcgroup));


            //老化直通率
            var burngroup = db.Burn_in.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.Burn_in_OQCCheckAbnormal == "正常" ? true : false }).ToList();
            array.Add(ChartGetPassThroughitem("老化", burngroup));
            //校正直通率
            var calibgroup = db.CalibrationRecord.Where(c => orderlist.Contains(c.OrderNum) && c.FinishCalibration != null && c.RepetitionCalibration == false).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.Normal }).ToList();
            array.Add(ChartGetPassThroughitem("校正", calibgroup));
            //外观直通率
            var appearangroup = db.Appearance.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.OQCCheckFinish }).ToList();
            array.Add(ChartGetPassThroughitem("外观", appearangroup));
            result.Add("MZ", array);

            return Content(JsonConvert.SerializeObject(result));
        }

        public JObject ChartGetPassThroughitem(string name, List<Temp> temps)
        {
            JObject result = new JObject();
            result.Add("title", name);
            JArray array = new JArray();
            List<string> groupitem = temps.Select(c => c.Group).Distinct().ToList();
            groupitem = groupitem.OrderBy(c => c).ToList();
            foreach (var item in groupitem)
            {
                JObject obj = new JObject();
                if (temps.Count(c => c.Name == "SMT") != 0)
                {
                    obj.Add("title", item + "线");
                    var totalinfo = temps.Where(c => c.Group == item).ToList();
                    var totalNum = totalinfo.Sum(c => c.AbnormalCount) + totalinfo.Sum(c => c.Passcount);
                    //直通率
                    var info = temps.Where(c => c.Group == item).Select(c => c.Passcount).ToList();
                    var passtrough = info.Sum();
                    obj.Add("passThrough", totalNum == 0 ? 0 : Math.Round((double)passtrough * 100 / totalNum, 2));

                    //异常率
                    var info2 = temps.Where(c => c.Group == item).Select(c => c.AbnormalCount).ToList();
                    var abnormal = info2.Sum();
                    obj.Add("abnormal", totalNum == 0 ? 0 : Math.Round((double)abnormal * 100 / totalNum, 2));

                    array.Add(obj);
                }
                else if (item == "")
                    continue;
                else if (string.IsNullOrEmpty(item))
                {
                    obj.Add("title", "没有班组");
                    int totalNum = temps.Where(c => c.Group == item || c.Group == "").Select(c => c.BarCodesNum).Distinct().Count();
                    //合格率
                    var abnormal = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == false).Select(c => c.BarCodesNum).ToList();

                    //直通率
                    var pass = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == true).Count();

                    var info = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == true && !abnormal.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).Distinct().Count();
                    //var passtrough = info.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var abno = abnormal.Distinct().Count();
                    obj.Add("passThrough", totalNum == 0 ? 0 : Math.Round((double)info * 100 / totalNum, 2));
                    obj.Add("abnormal", totalNum == 0 ? 0 : Math.Round((double)abno * 100 / totalNum, 2));

                    array.Add(obj);
                }
                else
                {
                    obj.Add("title", item);
                    int totalNum = temps.Where(c => c.Group == item).Select(c => c.BarCodesNum).Distinct().Count(); ;
                    //异常率
                    var abnormal = temps.Where(c => (c.Group == item) && c.Finish == false).Select(c => c.BarCodesNum).ToList();

                    var pass = temps.Where(c => (c.Group == item) && c.Finish == true).Count();

                    //直通率
                    var info = temps.Where(c => c.Group == item && c.Finish == true && !abnormal.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).Distinct().Count();
                    //var passtrough = info.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    obj.Add("passThrough", totalNum == 0 ? 0 : Math.Round((double)info * 100 / totalNum, 2));
                    var abno = abnormal.Distinct().Count();
                    obj.Add("abnormal", totalNum == 0 ? 0 : Math.Round((double)abno * 100 / totalNum, 2));

                    array.Add(obj);
                }
            }
            double TpassThrough;
            double Tabnormal;
            if (temps.Count(c => c.Name == "SMT") != 0)
            {
                //合计直通率
                var total = temps.Sum(c => c.AbnormalCount) + temps.Sum(c => c.Passcount);
                var totalpasstrough = temps.Sum(c => c.Passcount);
                var totalabnormal = temps.Sum(c => c.AbnormalCount);
                TpassThrough = temps.Count == 0 ? 0 : Math.Round((double)totalpasstrough * 100 / total, 2);

                //合计异常率
                Tabnormal = temps.Count == 0 ? 0 : Math.Round((double)totalabnormal * 100 / total, 2);

            }
            else
            {
                //合计直通率

                var totalabnormal = temps.Where(c => c.Finish == false).Select(c => c.BarCodesNum).ToList();
                var totalpasstrough2 = temps.Where(c => c.Finish == true && !totalabnormal.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).Distinct().Count();
                var totalCount = temps.Select(c => c.BarCodesNum).Distinct().Count();

                var abno = totalabnormal.Distinct().Count();
                //合计异常率
                TpassThrough = temps.Count == 0 ? 0 : Math.Round((double)totalpasstrough2 * 100 / totalCount, 2);
                Tabnormal = temps.Count == 0 ? 0 : Math.Round((double)abno * 100 / totalCount, 2);
            }
            result.Add("passThrough", TpassThrough);
            result.Add("abnormal", Tabnormal);
            result.Add("groupList", array);
            return result;
        }

        [HttpPost]
        public ActionResult GetPassThrough(string type, int year, int mouth, bool week, int day, string ordernum)
        {
            var orderlist = new List<string>();
            if (!string.IsNullOrEmpty(ordernum))
            {
                orderlist.Add(ordernum);
            }
            else
            {
                var info = db.OrderMgm.Where(c => c.PlatformType == type).ToList();
                if (week)
                {
                    var starttime = new DateTime(year, mouth, day);
                    var endtime = starttime.AddDays(7);
                    info = info.Where(c => c.ContractDate >= starttime && c.ContractDate <= endtime).ToList();
                }
                else
                {
                    if (year != 0)
                    {
                        info = info.Where(c => c.ContractDate.Year == year).ToList();
                    }
                    if (mouth != 0)
                    {
                        info = info.Where(c => c.ContractDate.Month == mouth).ToList();

                    }
                    if (day != 0)
                    {
                        info = info.Where(c => c.ContractDate.Day == day).ToList();
                    }
                }

                orderlist = info.Select(c => c.OrderNum).ToList();
            }
            JArray arr = new JArray();
            JObject obj = new JObject();
            //smt直通率
            var smt = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum)).Select(c => c.JobContent).Distinct().ToList();
            JArray smtarray = new JArray();
            foreach (var jobcontent in smt)
            {
                JObject content = new JObject();
                content.Add("jobname", jobcontent);
                JArray contenitem = new JArray();
                var linenumlist = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum) && c.JobContent == jobcontent).Select(c => c.LineNum).Distinct().ToList();
                foreach (var item in linenumlist)
                {
                    JObject smtitem = new JObject();
                    var smtinfo = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum) && c.JobContent == jobcontent && c.LineNum == item).ToList();
                    var total = smtinfo.Sum(c => c.NormalCount) + smtinfo.Sum(c => c.AbnormalCount);
                    var passcount = smtinfo.Sum(c => c.NormalCount);
                    var abnormal = smtinfo.Sum(c => c.AbnormalCount);
                    smtitem.Add("line", item);
                    smtitem.Add("passThrough", total == 0 ? "-%" : Math.Round((double)passcount * 100 / total, 2) + "%");
                    smtitem.Add("abnormal", total == 0 ? "-%" : Math.Round((double)abnormal * 100 / total, 2) + "%");
                    contenitem.Add(smtitem);
                }
                content.Add("content", contenitem);
                smtarray.Add(content);
            }
            obj.Add("smt", smtarray);
            //后焊直通率
            var group = db.AfterWelding.Where(c => orderlist.Contains(c.Ordernum)).Select(c => c.Group).Distinct().ToList();
            JArray afterArray = new JArray();
            foreach (var after in group)
            {
                if (after == "")
                    continue;
                JObject item = new JObject();
                if (string.IsNullOrEmpty(after))
                {
                    var aftertotelcount = db.AfterWelding.Count(c => orderlist.Contains(c.Ordernum) && (c.Group == after || c.Group == ""));
                    var afterPassThrough = db.AfterWelding.Count(c => orderlist.Contains(c.Ordernum) && c.IsAbnormal == false && c.Group == after);
                    item.Add("group", "没有分配班组的数据");//组
                    item.Add("passThrough", aftertotelcount == 0 ? "-%" : Math.Round((double)afterPassThrough / aftertotelcount, 2) + "%");
                }
                else
                {
                    var aftertotelcount = db.AfterWelding.Count(c => orderlist.Contains(c.Ordernum) && c.Group == after);
                    var afterPassThrough = db.AfterWelding.Count(c => orderlist.Contains(c.Ordernum) && c.IsAbnormal == false && c.Group == after);
                    item.Add("group", after);//组
                    item.Add("passThrough", aftertotelcount == 0 ? "-%" : Math.Round((double)afterPassThrough / aftertotelcount, 2) + "%");
                }
                afterArray.Add(item);
            }
            obj.Add("after", afterArray);
            //组装直通率
            var assemblessgrou = db.Assemble.Where(c => orderlist.Contains(c.OrderNum) && c.PQCCheckFT != null && c.RepetitionPQCCheck == false).Select(c => c.Group).Distinct().ToList();
            JArray assmebless = new JArray();
            foreach (var assembless in assemblessgrou)
            {
                if (assembless == "")
                    continue;
                JObject item = new JObject();
                if (string.IsNullOrEmpty(assembless))
                {
                    var assemblesscount = db.Assemble.Where(c => orderlist.Contains(c.OrderNum) && c.PQCCheckFT != null && c.RepetitionPQCCheck == false && (c.Group == assembless || c.Group == "")).ToList();
                    var assemblesstotal = assemblesscount.Select(c => c.BoxBarCode).Distinct().Count();
                    var assemblessPasscount = assemblesscount.GroupBy(c => c.BoxBarCode).Where(c => c.Count() < 2).ToList().Count();
                    var assemblessAbnormal = assemblesscount.Where(c => c.PQCCheckFinish == false).Count();
                    item.Add("group", "没有分配班组的数据");
                    item.Add("passThrough", assemblesstotal == 0 ? "-%" : Math.Round((double)assemblessPasscount * 100 / assemblesstotal, 2) + "%");
                    item.Add("abnormal", assemblesstotal == 0 ? "-%" : Math.Round((double)assemblessAbnormal * 100 / assemblesstotal, 2) + "%");
                }
                else
                {
                    var assemblesscount = db.Assemble.Where(c => orderlist.Contains(c.OrderNum) && c.PQCCheckFT != null && c.RepetitionPQCCheck == false && c.Group == assembless).ToList();
                    var assemblesstotal = assemblesscount.Select(c => c.BoxBarCode).Distinct().Count();
                    var assemblessPasscount = assemblesscount.GroupBy(c => c.BoxBarCode).Where(c => c.Count() < 2).ToList().Count();
                    var assemblessAbnormal = assemblesscount.Where(c => c.PQCCheckFinish == false).Count();
                    item.Add("group", assembless);
                    item.Add("passThrough", assemblesstotal == 0 ? "-%" : Math.Round((double)assemblessPasscount * 100 / assemblesstotal, 2) + "%");
                    item.Add("abnormal", assemblesstotal == 0 ? "-%" : Math.Round((double)assemblessAbnormal * 100 / assemblesstotal, 2) + "%");
                }
                assmebless.Add(item);
            }
            obj.Add("assemble", assmebless);

            //FQC直通率
            var fqcgroup = db.FinalQC.Where(c => orderlist.Contains(c.OrderNum) && c.FQCCheckFT != null && c.RepetitionFQCCheck == false).Select(c => c.Group).Distinct().ToList();
            JArray fqcarray = new JArray();
            foreach (var fqcitem in fqcgroup)
            {
                if (fqcitem == "")
                    continue;
                JObject item = new JObject();
                if (string.IsNullOrEmpty(fqcitem))
                {
                    var FQCcount = db.FinalQC.Where(c => orderlist.Contains(c.OrderNum) && c.FQCCheckFT != null && c.RepetitionFQCCheck == false && (c.Group == fqcitem || c.Group == "")).ToList();
                    var FQCtotal = FQCcount.Select(c => c.BarCodesNum).Distinct().Count();
                    var FQCPasscount = FQCcount.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var FQCAbnormal = FQCcount.Where(c => c.FQCCheckFinish == false).Count();
                    item.Add("group", "没有分配班组的数据");
                    item.Add("passThrough", FQCtotal == 0 ? "-%" : Math.Round((double)FQCPasscount * 100 / FQCtotal, 2) + "%");
                    item.Add("abnormal", FQCtotal == 0 ? "-%" : Math.Round((double)FQCAbnormal * 100 / FQCtotal, 2) + "%");
                }
                else
                {
                    var FQCcount = db.FinalQC.Where(c => orderlist.Contains(c.OrderNum) && c.FQCCheckFT != null && c.RepetitionFQCCheck == false && c.Group == fqcitem).ToList();
                    var FQCtotal = FQCcount.Select(c => c.BarCodesNum).Distinct().Count();
                    var FQCPasscount = FQCcount.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var FQCAbnormal = FQCcount.Where(c => c.FQCCheckFinish == false).Count();
                    item.Add("group", fqcitem);
                    item.Add("passThrough", FQCtotal == 0 ? "-%" : Math.Round((double)FQCPasscount * 100 / FQCtotal, 2) + "%");
                    item.Add("abnormal", FQCtotal == 0 ? "-%" : Math.Round((double)FQCAbnormal * 100 / FQCtotal, 2) + "%");
                }
                fqcarray.Add(item);
            }
            obj.Add("fqc", fqcarray);
            //老化直通率
            var burngroup = db.Burn_in.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c => c.Group).Distinct().ToList();
            JArray burnArray = new JArray();
            foreach (var burnitem in burngroup)
            {
                if (burnitem == "")
                    continue;
                JObject item = new JObject();
                if (string.IsNullOrEmpty(burnitem))
                {
                    var burncount = db.Burn_in.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null && (c.Group == burnitem || c.Group == "")).ToList();
                    var burntotal = burncount.Select(c => c.BarCodesNum).Distinct().Count();
                    var burnPasscount = burncount.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var burnAbnormal = burncount.Where(c => c.OQCCheckFinish == false).Count();
                    item.Add("group", "没有分配班组的数据");
                    item.Add("passThrough", burntotal == 0 ? "-%" : Math.Round((double)burnPasscount * 100 / burntotal, 2) + "%");
                    item.Add("abnormal", burntotal == 0 ? "-%" : Math.Round((double)burnAbnormal * 100 / burntotal, 2) + "%");
                }
                else
                {
                    var burncount = db.Burn_in.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null && c.Group == burnitem).ToList();
                    var burntotal = burncount.Select(c => c.BarCodesNum).Distinct().Count();
                    var burnPasscount = burncount.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var burnAbnormal = burncount.Where(c => c.OQCCheckFinish == false).Count();
                    item.Add("group", burnitem);
                    item.Add("passThrough", burntotal == 0 ? "-%" : Math.Round((double)burnPasscount * 100 / burntotal, 2) + "%");
                    item.Add("abnormal", burntotal == 0 ? "-%" : Math.Round((double)burnAbnormal * 100 / burntotal, 2) + "%");
                }
                burnArray.Add(item);

            }
            obj.Add("burnin", burnArray);
            //校正直通率
            var calibgroup = db.CalibrationRecord.Where(c => orderlist.Contains(c.OrderNum) && c.FinishCalibration != null && c.RepetitionCalibration == false).Select(c => c.Group).Distinct().ToList();
            JArray calibArray = new JArray();
            foreach (var calibitem in calibgroup)
            {
                if (calibitem == "")
                    continue;
                JObject item = new JObject();
                if (string.IsNullOrEmpty(calibitem))
                {
                    var calibcount = db.CalibrationRecord.Where(c => orderlist.Contains(c.OrderNum) && c.FinishCalibration != null && c.RepetitionCalibration == false && (c.Group == calibitem || c.Group == "")).ToList();
                    var calibtotal = calibcount.Select(c => c.BarCodesNum).Distinct().Count();
                    var calibPasscount = calibcount.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var calibAbnormal = calibcount.Where(c => c.Normal == false).Count();
                    item.Add("group", "没有分配班组的数据");
                    item.Add("passThrough", calibtotal == 0 ? "-%" : Math.Round((double)calibPasscount * 100 / calibtotal, 2) + "%");
                    item.Add("abnormal", calibtotal == 0 ? "-%" : Math.Round((double)calibAbnormal * 100 / calibtotal, 2) + "%");
                }
                else
                {
                    var calibcount = db.CalibrationRecord.Where(c => orderlist.Contains(c.OrderNum) && c.FinishCalibration != null && c.RepetitionCalibration == false && c.Group == calibitem).ToList();
                    var calibtotal = calibcount.Select(c => c.BarCodesNum).Distinct().Count();
                    var calibPasscount = calibcount.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var calibAbnormal = calibcount.Where(c => c.Normal == false).Count();
                    item.Add("group", calibitem);
                    item.Add("passThrough", calibtotal == 0 ? "-%" : Math.Round((double)calibPasscount * 100 / calibtotal, 2) + "%");
                    item.Add("abnormal", calibtotal == 0 ? "-%" : Math.Round((double)calibAbnormal * 100 / calibtotal, 2) + "%");
                }
                calibArray.Add(item);
            }
            obj.Add("calibration", calibArray);
            //外观直通率
            var appearangroup = db.Appearance.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c => c.Group).Distinct().ToList();
            JArray appearanArray = new JArray();
            foreach (var appearanitem in appearangroup)
            {
                if (appearanitem == "")
                    continue;
                JObject item = new JObject();
                if (string.IsNullOrEmpty(appearanitem))
                {
                    var appearancount = db.Appearance.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null && (c.Group == appearanitem || c.Group == "")).ToList();
                    var appearantotal = appearancount.Select(c => c.BarCodesNum).Distinct().Count();
                    var appearanPasscount = appearancount.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var appearanAbnormal = appearancount.Where(c => c.OQCCheckFinish == false).Count();
                    item.Add("group", "没有分配班组的数据");
                    item.Add("passThrough", appearantotal == 0 ? "-%" : Math.Round((double)appearanPasscount * 100 / appearantotal, 2) + "%");
                    item.Add("abnormal", appearantotal == 0 ? "-%" : Math.Round((double)appearanAbnormal * 100 / appearantotal, 2) + "%");
                }
                else
                {
                    var appearancount = db.Appearance.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null && c.Group == appearanitem).ToList();
                    var appearantotal = appearancount.Select(c => c.BarCodesNum).Distinct().Count();
                    var appearanPasscount = appearancount.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var appearanAbnormal = appearancount.Where(c => c.OQCCheckFinish == false).Count();
                    item.Add("group", appearanitem);
                    item.Add("passThrough", appearantotal == 0 ? "-%" : Math.Round((double)appearanPasscount * 100 / appearantotal, 2) + "%");
                    item.Add("abnormal", appearantotal == 0 ? "-%" : Math.Round((double)appearanAbnormal * 100 / appearantotal, 2) + "%");
                }
                appearanArray.Add(item);
            }
            obj.Add("appearance", appearanArray);
            arr.Add(obj);
            return Content(JsonConvert.SerializeObject(arr));
        }
        [HttpPost]
        //通过年月,找到当月有几个周
        public ActionResult GetWeek(int year, int mouth)
        {
            var time = new DateTime(year, mouth, 1);
            var lastday = DateTime.DaysInMonth(year, mouth);
            var index = (int)time.DayOfWeek == 0 ? 7 : (int)time.DayOfWeek;//当月1号是星期几
            var firstweekLastday = 7 - index + 1;//得到第一周的最后一天
            var week = Math.Ceiling((decimal)(lastday - firstweekLastday) / 7) + 1;
            JArray result = new JArray();
            for (var i = 1; i <= week; i++)
            {
                JObject List = new JObject();
                List.Add("value", i);
                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //平台列表
        public ActionResult GetPassThroughType()
        {
            var type = db.OrderMgm.Select(c => c.PlatformType).Distinct().ToList();
            JArray result = new JArray();
            foreach (var item in type)
            {
                JObject List = new JObject();
                List.Add("value", item);
                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region---KPI班组流失率

        public class ExportTurnoverCheckSshift
        {
            public string Department { get; set; }//部门
            public string Group { get; set; }//班组       
            public string IndicatorsName { get; set; }//指标名称
            public double IndicatorsValue { get; set; }//目标值
            public string SourceDepartment { get; set; }//数据来源部门
            public int BeginNumber { get; set; }//月初人数
            public int EndNumber { get; set; }//月末人数
            public int AverageNum { get; set; }//平均人数

            #region--天数1-31
            public int Day1 { get; set; }
            public int Day2 { get; set; }
            public int Day3 { get; set; }
            public int Day4 { get; set; }
            public int Day5 { get; set; }
            public int Day6 { get; set; }
            public int Day7 { get; set; }
            public int Day8 { get; set; }
            public int Day9 { get; set; }
            public int Day10 { get; set; }
            public int Day11 { get; set; }
            public int Day12 { get; set; }
            public int Day13 { get; set; }
            public int Day14 { get; set; }
            public int Day15 { get; set; }
            public int Day16 { get; set; }
            public int Day17 { get; set; }
            public int Day18 { get; set; }
            public int Day19 { get; set; }
            public int Day20 { get; set; }
            public int Day21 { get; set; }
            public int Day22 { get; set; }
            public int Day23 { get; set; }
            public int Day24 { get; set; }
            public int Day25 { get; set; }
            public int Day26 { get; set; }
            public int Day27 { get; set; }
            public int Day28 { get; set; }
            public int Day29 { get; set; }
            public int Day30 { get; set; }
            public int Day31 { get; set; }

            #endregion

            public int TurnoverMonth { get; set; }//月流失人数
            public double Actual { get; set; }//实际得分
            public double Differences { get; set; }//与目标值差异
            public double Subtotal { get; set; }//得分小计
        }

        public ActionResult KPI_DeparturesNum()//班组人员流失率首页
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "KPI", act = "KPI_DeparturesNum" });
            }
            return View();
        }

        //批量上传班组流失率
        public ActionResult DeparturesNum(List<KPI_TurnoverRate> turnoverRates, DateTime dateLoss)
        {
            JObject loss = new JObject();
            JArray copy = new JArray();
            string rates = null;
            if (turnoverRates.Count > 0 && dateLoss != null && turnoverRates.FirstOrDefault().Department != null && turnoverRates.FirstOrDefault().Group != null)
            {
                foreach (var item in turnoverRates)
                {
                    item.Createor = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                    item.CreateTime = DateTime.Now;
                    if (db.KPI_TurnoverRate.Count(c => c.Department == item.Department && c.Group == item.Group && c.DateLoss == dateLoss) > 0)
                    {
                        rates = rates + item.Department + "," + item.Group + "," + dateLoss + "的记录重复了" + ";";
                        copy.Add(rates);
                    }
                }
                if (rates != null)
                {
                    loss.Add("meg", false);
                    loss.Add("feg", copy);
                    return Content(JsonConvert.SerializeObject(loss));
                }
                foreach (var it in turnoverRates)
                {
                    it.DateLoss = dateLoss;
                    db.SaveChanges();
                }
                db.KPI_TurnoverRate.AddRange(turnoverRates);
                int count = db.SaveChanges();
                if (count > 0)
                {
                    loss.Add("meg", true);
                    loss.Add("feg", "保存成功！");
                    loss.Add("loss", JsonConvert.SerializeObject(turnoverRates));
                    return Content(JsonConvert.SerializeObject(loss));
                }
                else
                {
                    loss.Add("meg", false);
                    loss.Add("feg", "保存出错！");
                    return Content(JsonConvert.SerializeObject(loss));
                }
            }
            loss.Add("meg", false);
            loss.Add("feg", "数据错误/人员流失日期为空");
            return Content(JsonConvert.SerializeObject(loss));
        }

        //查询班组人员流失率汇总表
        public ActionResult Turnover_CheckSshift(int? year, int? month)
        {
            JObject Rate = new JObject();
            JArray table = new JArray();
            JObject retul = new JObject();
            JArray meg = new JArray();
            JObject tall = new JObject();
            JArray chek = new JArray();
            var checkList = db.KPI_TurnoverRate.ToList();
            List<KPI_TurnoverRate> Turnover = new List<KPI_TurnoverRate>();
            double averageNum = 0;//平均人数
            double turnoverMonth = 0;//月流失人数
            double actual = 0;//实际得分
            double differences = 0;//与目标值的差异
            double subtotal = 0;//得分小计
            if (year != 0 && month != 0)
            {
                Turnover = checkList.Where(c => c.DateLoss.Year == year && c.DateLoss.Month == month).ToList();
            }
            if (year != 0 && month == 0)
            {
                Turnover = checkList.Where(c => c.DateLoss.Year == year).ToList();
            }
            if (Turnover.Count > 0)
            {
                var checkList1 = Turnover.Select(c => c.Department).Distinct().ToList();
                foreach (var ite in checkList1)
                {
                    var group = Turnover.Where(c => c.Department == ite).Select(c => c.Group).Distinct().ToList();
                    foreach (var it in group)
                    {
                        var rate = Turnover.Where(c => c.Department == ite && c.Group == it).Distinct().FirstOrDefault();
                        //Id
                        Rate.Add("Id", rate.Id == 0 ? 0 : rate.Id);
                        //被考核部门
                        Rate.Add("Department", ite == null ? null : ite);
                        //班组
                        Rate.Add("Group", it == null ? null : it);
                        //指标名称                      
                        Rate.Add("IndicatorsName", "班组流失率");
                        //目标值
                        Rate.Add("IndicatorsValue", rate.IndicatorsValue);
                        //数据来源部门
                        Rate.Add("SourceDepartment", "人力资源部");
                        //月初人数
                        Rate.Add("BeginNumber", rate.BeginNumber == 0 ? 0 : rate.BeginNumber);
                        //月末人数
                        var endNumber = Turnover.Where(c => c.Department == ite && c.Group == it).OrderByDescending(c => c.Id).Select(c => c.EndNumber).FirstOrDefault();
                        Rate.Add("EndNumber", endNumber == 0 ? 0 : endNumber);
                        //平均人数                     
                        double num = rate.BeginNumber + endNumber;
                        averageNum = Math.Ceiling(num / 2);
                        Rate.Add("AverageNum", averageNum == 0 ? 0 : averageNum);
                        Rate.Add("Year", rate.DateLoss.Year);
                        Rate.Add("Month", rate.DateLoss.Month);
                        DateTime dt = rate.DateLoss;
                        int sumday = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1).Day;//一个月有多少天     
                        for (var i = 1; i <= sumday; i++)
                        {
                            var ifno = Turnover.Where(c => c.Department == ite && c.Group == it && c.DateLoss.Day == i).Select(c => c.LossNumber).FirstOrDefault();
                            //流失日期
                            tall.Add("DateLoss", i);
                            //流失人数
                            tall.Add("LossNumber", ifno == 0 ? 0 : ifno);
                            turnoverMonth = turnoverMonth + ifno;
                            meg.Add(tall);
                            tall = new JObject();
                        }
                        Rate.Add("Rate", meg);
                        meg = new JArray();
                        actual = actual + (turnoverMonth / averageNum);
                        differences = (rate.IndicatorsValue == 0 ? 0 : actual - rate.IndicatorsValue);
                        if (rate.IndicatorsValue != 0)
                        {
                            if (actual <= rate.IndicatorsValue)
                            {
                                subtotal = 100;

                            }
                            else if (actual > rate.IndicatorsValue)
                            {
                                subtotal = 100 - (actual - rate.IndicatorsValue);
                            }
                        }
                        //月流失人数
                        Rate.Add("TurnoverMonth", turnoverMonth == 0 ? 0 : turnoverMonth);
                        turnoverMonth = new int();
                        //月流失率（实际得分）
                        Rate.Add("Actual", double.Parse(actual.ToString("0.00")));
                        actual = new double();
                        //与目标值的差异    
                        Rate.Add("Differences", double.Parse(differences.ToString("0.00")));
                        differences = new double();
                        //得分小计
                        Rate.Add("Subtotal", subtotal == 0 ? 0 : subtotal);
                        subtotal = new double();
                        table.Add(Rate);
                        Rate = new JObject();
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(table));
        }

        //修改班组人员流失率
        public ActionResult ModifyTurnover(KPI_TurnoverRate turnoverRate)
        {
            JObject table = new JObject();
            int count = 0;
            if (turnoverRate != null && turnoverRate.DateLoss != null && turnoverRate.Department != null && turnoverRate.Group != null)
            {
                if (db.KPI_TurnoverRate.Count(c => c.Department == turnoverRate.Department && c.Group == turnoverRate.Group && c.DateLoss == turnoverRate.DateLoss) > 0)
                {
                    turnoverRate.ModifierName = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";//添加修改人
                    turnoverRate.ModifierDate = DateTime.Now;//添加修改时间
                    db.Entry(turnoverRate).State = EntityState.Modified;//修改数据
                    count = db.SaveChanges();
                }
                else
                {
                    turnoverRate.CreateTime = DateTime.Now;
                    turnoverRate.Createor = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                    db.KPI_TurnoverRate.Add(turnoverRate);
                    count = db.SaveChanges();
                }
                if (count > 0)
                {
                    table.Add("Meg", true);
                    table.Add("Feg", "修改成功");
                    table.Add("TurnoverRate", JsonConvert.SerializeObject(turnoverRate));
                    return Content(JsonConvert.SerializeObject(table));
                }
                else
                {
                    table.Add("Meg", false);
                    table.Add("Feg", "修改失败");
                    return Content(JsonConvert.SerializeObject(table));
                }
            }
            table.Add("Meg", false);
            table.Add("Feg", "数据错误");
            return Content(JsonConvert.SerializeObject(table));
        }

        //导出班组人员流失率汇总表(Excel)
        [HttpPost]
        public FileContentResult Turnover_OutputExcel(int year, int month)
        {
            var dataList = db.KPI_TurnoverRate.Where(c => c.DateLoss.Year == year && c.DateLoss.Month == month).ToList();
            List<ExportTurnoverCheckSshift> Resultlist = new List<ExportTurnoverCheckSshift>();
            var departmentlist = dataList.Select(c => c.Department).Distinct().ToList();
            foreach (var item in departmentlist)//循环被考核部门
            {
                var DG_list = dataList.Where(c => c.Department == item).Select(c => c.Group).Distinct().ToList();
                foreach (var ite in DG_list)//循环被考核部门的班组
                {
                    ExportTurnoverCheckSshift at = new ExportTurnoverCheckSshift();
                    var tarodlist = dataList.Where(c => c.Department == item && c.Group == ite).ToList();
                    at.Department = tarodlist.FirstOrDefault().Department;//部门
                    at.Group = tarodlist.FirstOrDefault().Group;//班组
                    at.IndicatorsName = "班组流失率";//指标名称
                    at.IndicatorsValue = tarodlist.FirstOrDefault().IndicatorsValue;//目标值
                    at.SourceDepartment = "人力资源部";
                    at.BeginNumber = tarodlist.FirstOrDefault().BeginNumber;//月初人数
                    var endNumber = dataList.Where(c => c.Department == item && c.Group == ite).OrderByDescending(c => c.Id).Select(c => c.EndNumber).FirstOrDefault();
                    at.EndNumber = endNumber;//月末人数
                    double num = tarodlist.FirstOrDefault().BeginNumber + endNumber;
                    double averageNum = Math.Ceiling(num / 2);
                    at.AverageNum = (int)averageNum;//平均人数

                    //赋值31天
                    int totalscore = 0;
                    var filds = at.GetType().GetFields(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy);
                    filds = filds.Where(c => c.Name.Contains("Day")).ToArray();
                    for (var i = 1; i <= filds.Count(); i++)
                    {
                        var ifno = dataList.Where(c => c.Department == item && c.Group == ite && c.DateLoss.Day == i).Select(c => c.LossNumber).FirstOrDefault();
                        filds[i - 1].SetValue(at, ifno);
                        totalscore = totalscore + ifno;
                    }
                    at.TurnoverMonth = totalscore;//月流失人数
                    double actual = at.TurnoverMonth / (double)at.AverageNum;
                    at.Actual = actual;//实际得分
                    at.Differences = at.Actual - tarodlist.FirstOrDefault().IndicatorsValue;//与目标值的差异
                    double subtotal = 0;
                    if (at.Actual <= tarodlist.FirstOrDefault().IndicatorsValue)
                    {
                        subtotal = 100;

                    }
                    else if (at.Actual > tarodlist.FirstOrDefault().IndicatorsValue)
                    {
                        subtotal = 100 - (at.Actual - tarodlist.FirstOrDefault().IndicatorsValue);
                    }
                    at.Subtotal = subtotal;//得分小计
                    Resultlist.Add(at);
                }
            }
            // 导出表格名称
            string tableName = "班组人员流失率汇总表" + year + "年" + month + "月";
            if (Resultlist.Count() > 0)
            {
                string[] columns = { "部门名称", "班组名称", "考核指标名", "目标值", "数据来源", "月初人数", "月末人数", "平均人数", "1日", "2日", "3日", "4日", "5日", "6日", "7日", "8日", "9日", "10日", "11日", "12日", "13日", "14日", "15日", "16日", "17日", "18日", "19日", "20日", "21日", "22日", "23日", "24日", "25日", "26日", "27日", "28日", "29日", "30日", "31日", "月流失人数", "实际得分", "与目标值差异", "得分小计" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
            }
            else
            {
                ExportTurnoverCheckSshift at1 = new ExportTurnoverCheckSshift();
                at1.Group = "没有找到相关记录！";
                Resultlist.Add(at1);
                string[] columns = { "部门名称", "班组名称", "考核指标名", "目标值", "数据来源", "月初人数", "月末人数", "平均人数", "1日", "2日", "3日", "4日", "5日", "6日", "7日", "8日", "9日", "10日", "11日", "12日", "13日", "14日", "15日", "16日", "17日", "18日", "19日", "20日", "21日", "22日", "23日", "24日", "25日", "26日", "27日", "28日", "29日", "30日", "31日", "月流失人数", "实际得分", "与目标值差异", "得分小计" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
            }
        }

        #endregion

        #region--7S

        #region--页面       
        public ActionResult KPI_7S_Summarizing()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_7S_Summarizing" });
            }
            return View();
        }
        public ActionResult KPI_7S_SummarizingRanking()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_7S_SummarizingRanking" });
            }
            return View();
        }

        public ActionResult KPI_7S_GradeStandardInput()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_7S_GradeStandardInput" });
            }
            return View();
        }

        public ActionResult KPI_7S_RegionInput()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_7S_RegionInput" });
            }
            return View();
        }
        public ActionResult KPI_7S_RecordInput()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_7S_RecordInput" });
            }
            return View();
        }

        public ActionResult KPI_7S_Detail()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_7S_Detail" });
            }
            return View();
        }

        public ActionResult KPI_7S_Summarizing_Daily()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_7S_Summarizing_Daily" });
            }
            return View();
        }

        public ActionResult KPI_7S_RecordInput_Mobile()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_7S_RecordInput_Mobile" });
            }
            return View();
        }

        #endregion

        #region---检查参考标准

        #region--检查参考标准数据录入   
        [HttpPost]
        public ActionResult ReferenceStandard_Input(List<KPI_7S_ReferenceStandard> record)
        {
            if (record.Count > 0)
            {
                int savecout = 0;
                foreach (var item in record)
                {
                    //检验记录是否存在
                    if (db.KPI_7S_ReferenceStandard.Count(c => c.PointsType == item.PointsType && c.ReferenceStandard == item.ReferenceStandard) < 1)
                    {
                        item.InputTime = DateTime.Now;
                        item.InputPerson = ((Users)Session["User"]).UserName;
                        db.SaveChanges();
                        db.KPI_7S_ReferenceStandard.Add(item);
                        savecout += db.SaveChanges();
                    }
                }
                if (savecout > 0) return Content("保存成功！");
                else return Content("保存失败！");
            }
            else return Content("传入参数为空！");
        }
        #endregion

        #region--检查参考标准查询
        [HttpPost]
        public ActionResult ReferenceStandard_query(string[] pointsType)
        {
            List<KPI_7S_ReferenceStandard> newList = new List<KPI_7S_ReferenceStandard>();
            var record = db.KPI_7S_ReferenceStandard.ToList();
            if (pointsType != null)
            {
                foreach (var item in pointsType)
                {
                    var list = record.Where(c => c.PointsType == item).ToList();
                    newList = newList.Concat(list).ToList();
                }
            }
            return Content(JsonConvert.SerializeObject(newList));
        }
        #endregion

        #region--检查参考标准修改
        [HttpPost]
        public ActionResult ReferenceStandard_modify(int id, string pointsType, string referenceStandard)
        {
            if (id != 0 && !String.IsNullOrEmpty(pointsType) && !String.IsNullOrEmpty(referenceStandard))
            {
                var record = db.KPI_7S_ReferenceStandard.Where(c => c.Id == id).FirstOrDefault();
                record.PointsType = pointsType;
                record.ReferenceStandard = referenceStandard;
                record.ModifyPerson = ((Users)Session["User"]).UserName;
                record.ModifyTime = DateTime.Now;
                int count = db.SaveChanges();
                if (count > 0) return Content("修改成功！");
                else return Content("修改失败！");
            }
            else return Content("传入参数为空");
        }
        #endregion

        #region--检查参考标准删除
        [HttpPost]
        public ActionResult ReferenceStandard_delete(int id)
        {
            var removeList = db.KPI_7S_ReferenceStandard.Where(c => c.Id == id).ToList();
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;
            operaterecord.Operator = ((Users)Session["User"]).UserName;
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除了7S扣分类型为：" + removeList.FirstOrDefault().PointsType + "的扣分参照标准项,标准项为：" + removeList.FirstOrDefault().ReferenceStandard + "记录Id为：" + removeList.FirstOrDefault().Id + ".";
            db.KPI_7S_ReferenceStandard.RemoveRange(removeList);
            db.UserOperateLog.Add(operaterecord);//保存删除日志
            int count = db.SaveChanges();
            if (count > 0) return Content("删除成功！");
            else return Content("删除失败！");
        }
        #endregion

        #endregion

        #region---区域数据

        #region---区域数据录入
        [HttpPost]
        public ActionResult Region_Input(List<KPI_7S_DistrictPosition> record)
        {
            if (record.Count > 0)
            {
                int savecout = 0;
                foreach (var item in record)
                {
                    //检验记录是否存在
                    if (db.KPI_7S_DistrictPosition.Count(c => c.Department == item.Department && c.Group == item.Group && c.Position == item.Position && c.District == item.District && c.VersionsTime.Value.Year == item.VersionsTime.Value.Year && c.VersionsTime.Value.Month == item.VersionsTime.Value.Month && c.VersionsTime.Value.Day == item.VersionsTime.Value.Day) < 1)
                    {
                        item.InputTime = DateTime.Now;
                        item.InputPerson = ((Users)Session["User"]).UserName;
                        db.SaveChanges();
                        db.KPI_7S_DistrictPosition.Add(item);
                        savecout += db.SaveChanges();
                    }
                }
                if (savecout > 0) return Content("保存成功！");
                else return Content("保存失败！");
            }
            else return Content("传入参数为空！");
        }
        #endregion

        #region---区域数据修改
        [HttpPost]
        /// department 部门
        /// position  位置
        /// district  区域号
        public ActionResult Region_modify(int id, string department, string position, int district, string group, double targetValue)
        {
            if (id != 0 && !String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district != 0)
            {
                //检查修改记录的是否已存在
                //if (db.KPI_7S_DistrictPosition.Count(c => c.Department == department && c.Position == position && c.District == district && c.Group == group) < 1)
                //{
                var record = db.KPI_7S_DistrictPosition.Where(c => c.Id == id).FirstOrDefault();
                record.Department = department;//部门
                record.Position = position;//位置
                record.District = district;//区域号
                record.Group = group;//班组
                record.TargetValue = targetValue;//目标值
                record.ModifyPerson = ((Users)Session["User"]).UserName;//修改人
                record.ModifyTime = DateTime.Now;//修改时间
                int count = db.SaveChanges();
                if (count > 0) return Content("修改成功！");
                else return Content("修改失败！");
                // }
                //else return Content("修改失败，记录已存在！");
            }
            else
            {
                return Content("传入参数为空！");
            }
        }

        #endregion

        #region---区域数据删除
        [HttpPost]
        public ActionResult Region_delete(int id)
        {
            var removeList = db.KPI_7S_DistrictPosition.Where(c => c.Id == id).ToList();
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;
            operaterecord.Operator = ((Users)Session["User"]).UserName;
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除了7S区域表中的：" + removeList.FirstOrDefault().Department + "，" + removeList.FirstOrDefault().Position + "位置的区域记录，区域号为：" + removeList.FirstOrDefault().District + "，记录Id为：" + removeList.FirstOrDefault().Id + ".";
            db.KPI_7S_DistrictPosition.RemoveRange(removeList);
            db.UserOperateLog.Add(operaterecord);//保存删除日志
            int count = db.SaveChanges();
            if (count > 0) return Content("删除成功！");
            else return Content("删除失败！");
        }
        #endregion

        #region---区域数据查询
        [HttpPost]
        /// department  部门
        /// district    区域号
        public ActionResult Region_query(string department, int? district, string position, DateTime versionsTime)
        {
            List<KPI_7S_DistrictPosition> recordList = new List<KPI_7S_DistrictPosition>();
            if (!String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == null)
            {//整个部门
                recordList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.VersionsTime.Value.Year == versionsTime.Year && c.VersionsTime.Value.Month == versionsTime.Month && c.VersionsTime.Value.Day == versionsTime.Day).ToList();
            }
            else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district == null)
            {//按部门、位置查
                recordList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.VersionsTime.Value.Year == versionsTime.Year && c.VersionsTime.Value.Month == versionsTime.Month && c.VersionsTime.Value.Day == versionsTime.Day).ToList();
            }
            else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district != null)
            { //按部门、位置、区域号                                
                recordList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.District == district && c.VersionsTime.Value.Year == versionsTime.Year && c.VersionsTime.Value.Month == versionsTime.Month && c.VersionsTime.Value.Day == versionsTime.Day).ToList();
            }
            JArray result = new JArray();
            foreach (var item in recordList)
            {
                JObject record = new JObject();
                record.Add("Id", item.Id);
                record.Add("Department", item.Department);
                record.Add("Position", item.Position);
                record.Add("Group", item.Group);
                record.Add("District", item.District);
                record.Add("TargetValue", item.TargetValue);
                record.Add("VersionsTime", Convert.ToDateTime(item.VersionsTime).ToString("yyyy-MM-dd"));
                result.Add(record);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region---获取版本时间
        public ActionResult GetVersions()
        {
            var list = db.KPI_7S_DistrictPosition.Select(c => c.VersionsTime).Distinct().ToList();
            JArray result = new JArray();
            foreach (var item in list)
            {
                JObject record = new JObject();
                record.Add("value", Convert.ToDateTime(item).ToString("yyyy-MM-dd"));
                record.Add("label", Convert.ToDateTime(item).ToString("yyyy-MM-dd"));
                result.Add(record);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region--获取部门下拉列表
        public ActionResult GetDepartmentList()
        {
            var departmentList = db.KPI_7S_DistrictPosition.Select(c => c.Department).Distinct().ToList();
            JArray result = new JArray();
            JObject record = new JObject();
            if (departmentList.Count > 0)
            {
                record.Add("value", "全部部门");
                record.Add("label", "全部部门");
                result.Add(record);
            }
            foreach (var item in departmentList)
            {
                JObject List = new JObject();
                List.Add("value", item);
                List.Add("label", item);
                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region--根据部门获取区域号下拉列表
        [HttpPost]
        public ActionResult GetDistrictList(string department, string position)
        {
            if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position))
            {
                var districtList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position).Select(c => c.District).Distinct().ToList();
                JArray result = new JArray();
                JObject record = new JObject();
                if (districtList.Count > 0)
                {
                    record.Add("value", "");
                    record.Add("label", "全部区域");
                    result.Add(record);
                }
                foreach (var item in districtList)
                {
                    JObject List = new JObject();
                    List.Add("value", item);
                    List.Add("label", item);
                    result.Add(List);
                }
                return Content(JsonConvert.SerializeObject(result));
            }
            else return Content("传入数据为空！");
        }
        #endregion

        #region--根据版本时间找出全部区域号
        public ActionResult GetDistrict(DateTime date)
        {
            var versionTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= date).Select(c => c.VersionsTime).Distinct().Max();
            var record = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == versionTime).Select(c => c.District).Distinct().ToList();
            JArray result = new JArray();
            foreach (var item in record)
            {
                JObject res = new JObject();
                res.Add("value", item);
                res.Add("label", item);
                result.Add(res);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region--根据区域号找位置、部门
        [HttpPost]
        public ActionResult GetPosition(int district, DateTime date)
        {
            var versionTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= date).Select(c => c.VersionsTime).Distinct().Max();
            var list = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == versionTime).ToList();
            //找位置
            JObject result = new JObject();
            JArray positionList = new JArray();
            var position = list.Where(c => c.District == district).ToList();
            foreach (var i in position)
            {
                JObject res = new JObject();
                res.Add("value", i.Position);
                res.Add("label", i.Position);
                positionList.Add(res);
            }
            result.Add("positionList", positionList);
            //找部门
            JArray depList = new JArray();
            var dep = list.Where(c => c.District == district).Select(c => c.Department).Distinct().ToList();
            foreach (var item in dep)
            {
                JObject res = new JObject();
                res.Add("value", item);
                res.Add("label", item);
                depList.Add(res);
            }
            result.Add("depList", depList);
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion
        #endregion

        #region---日检、周检、巡检数据录入
        public class selectOption
        {
            public string Check_Type { get; set; }//检查类型
            public DateTime Date { get; set; }//检查时间
            public string Department { get; set; }//部门
            public string Position { get; set; }//位置
            public int District { get; set; } //区域号
            //public string ResponsiblePerson { get; set; }//责任人
            public string Check_Person { get; set; }//检查人
        }
        [HttpPost]
        public ActionResult Check_Record(List<KPI_7S_Record> record, selectOption formInfo)
        {
            JArray check = new JArray();
            var res = db.KPI_7S_Record.Where(c => c.Year == formInfo.Date.Year && c.Month == formInfo.Date.Month).ToList();
            foreach (var j in record)
            {
                if (res.Where(c => c.Department == formInfo.Department && c.Position == formInfo.Position && c.District == formInfo.District && c.Check_Type == formInfo.Check_Type && c.Date.Year == formInfo.Date.Year && c.Date.Month == formInfo.Date.Month && c.Date.Day == formInfo.Date.Day && c.PointsDeducted_Type == j.PointsDeducted_Type).Count() > 0)
                {
                    check.Add(j.PointsDeducted_Type);
                }
            }
            return Content(JsonConvert.SerializeObject(check));
        }
        [HttpPost]
        public ActionResult RecordInput(List<KPI_7S_Record> record, selectOption formInfo)
        {
            if (record.Count > 0 && formInfo != null)
            {
                JArray weekday = new JArray();
                weekday = getWeekNumInMonth(formInfo.Date);
                int savecout = 0;
                var list = db.KPI_7S_Record.Where(c => c.Year == formInfo.Date.Year && c.Month == formInfo.Date.Month && c.Check_Type != "日检").ToList();
                foreach (var item in record)
                {
                    item.Department = formInfo.Department;//部门
                    item.Group = formInfo.Position;//班组
                    item.Position = formInfo.Position;//位置
                    item.District = formInfo.District;//区域号
                    item.Date = formInfo.Date;//检查日期
                    item.Check_Type = formInfo.Check_Type;//检查类型
                    item.Week = Convert.ToInt32(weekday[1]);//第几周
                    item.Year = formInfo.Date.Year;//年
                    item.Month = formInfo.Date.Month;//月                    
                    item.InputPerson = ((Users)Session["User"]).UserName;//录入人
                    item.InputTime = DateTime.Now;//录入时间 
                    item.Check_Person = formInfo.Check_Person;//检查人
                    if (formInfo.Check_Type != "日检")
                    {
                        //检验是否有重复出现的问题
                        if (list.Where(c => c.Department == formInfo.Department && c.Position == formInfo.Position && c.District == formInfo.District && c.PointsDeducted_Type == item.PointsDeducted_Type && c.PointsDeducted_Item == item.PointsDeducted_Item && c.ProblemDescription == item.ProblemDescription).Count() > 0)
                        {
                            item.RepetitionPointsDeducted = 2;//重复出现扣两分
                        }
                    }
                    db.SaveChanges();
                    db.KPI_7S_Record.Add(item);
                    savecout += db.SaveChanges();
                }
                if (savecout > 0) return Content("保存成功！");
                else return Content("保存失败！");
            }
            else return Content("传入数据为空！");
        }
        #endregion

        #region---日检、周检、巡检图片上传
        [HttpPost]
        /// <param name="pictureFile">图片</param>
        /// <param name="department">部门</param>
        /// <param name="position">位置</param>
        /// <param name="check_date">检查日期</param>
        /// <param name="check_Type">检查类型（日检、周检、抽检）</param>
        /// <param name="pointsDeducted_Type">7S扣分类型（整理、整顿、清洁、清扫、安全、节约、素养）</param>
        /// <param name="uploadType">上传类型（改善前、改善后）</param>
        /// <returns></returns>
        public bool ImageUpload(List<string> pictureFile, string department, string position, DateTime check_date, string check_Type, string pointsDeducted_Type, string uploadType, int district)
        {
            string datatime = check_date.Year.ToString() + "." + check_date.Month.ToString() + "." + check_date.Day.ToString();
            if (Request.Files.Count > 0 && !String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && !String.IsNullOrEmpty(check_Type) && !String.IsNullOrEmpty(pointsDeducted_Type) && !String.IsNullOrEmpty(uploadType))
            {
                if (Directory.Exists(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\") == false)//判断总路径是否存在
                {
                    Directory.CreateDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\");//创建总路径
                };

                if (Directory.Exists(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\") == false)//判断图片路径是否存在
                {
                    Directory.CreateDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\");//创建图片路径
                }
                foreach (var item in pictureFile)
                {
                    HttpPostedFileBase file = Request.Files["UploadFile_Ingredients" + pictureFile.IndexOf(item)];
                    var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();
                    List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\");//遍历文件夹中的个数
                    if (fileType == ".jpg")//判断文件后缀
                    {
                        int jpg_count = filesInfo.Where(c => c.Name.StartsWith(uploadType + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                        file.SaveAs(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + (jpg_count + 1) + fileType);//文件追加命名
                    }
                    else if (fileType == ".png")
                    {
                        int pdf_count = filesInfo.Where(c => c.Name.StartsWith(uploadType + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".png").Count();
                        file.SaveAs(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + (pdf_count + 1) + fileType);//文件追加命名
                    }
                    else if (fileType == ".jpeg")
                    {
                        int pdf_count = filesInfo.Where(c => c.Name.StartsWith(uploadType + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpeg").Count();
                        file.SaveAs(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + (pdf_count + 1) + fileType);//文件追加命名
                    }
                }
                return true;
            }
            return false;
        }

        public JArray QueryImage(string department, string position, DateTime check_date, string check_Type, string pointsDeducted_Type, string uploadType, int district)
        {
            string datatime = check_date.Year.ToString() + "." + check_date.Month.ToString() + "." + check_date.Day.ToString();
            JObject result = new JObject();
            JObject pictureAddress_list = new JObject();
            JObject drawFiles_list = new JObject();
            List<FileInfo> draw_files = null;
            if (Directory.Exists(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\"))//判断图纸的总路径是否存在
            {
                draw_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\");//遍历图纸文件夹的文件个数
            }
            if (draw_files != null)
            {
                JArray drawjpg_list = new JArray();//用于存放后缀为.jpg图纸的数组                   
                foreach (var i in draw_files)
                {
                    string path1 = @"/MES_Data/7S/" + check_date.Year.ToString() + "/" + check_date.Month.ToString() + "/" + check_Type + "/" + datatime + "/" + department + "/" + position + "/" + district + "/" + pointsDeducted_Type + "/" + uploadType + "/" + i;//组合出路径                                       
                    drawjpg_list.Add(path1);
                }
                return drawjpg_list;
            }
            else
            {
                JArray drawjpg_list = new JArray();//用于存放后缀为.jpg图纸的数组    
                return drawjpg_list;
            }
        }
        #endregion

        #region--- 扣分记录录入页面 获取录入数据
        [HttpPost]
        public ActionResult GetInputData(DateTime? time)
        {
            JArray recordList1 = new JArray();
            var list = db.KPI_7S_ReferenceStandard.OrderByDescending(c => c.PointsType).ToList();
            var list1 = list.Select(c => c.PointsType).Distinct().ToList();
            foreach (var item in list1)
            {
                JObject record = new JObject();
                JArray selectoption = new JArray();
                JArray option = new JArray();
                var list2 = list.Where(c => c.PointsType == item).ToList();
                foreach (var i in list2)
                {
                    JObject res = new JObject();
                    res.Add("value", i.ReferenceStandard);
                    res.Add("count", 0);
                    selectoption.Add(res);
                }
                record.Add("PointsDeducted_Type", item);
                record.Add("PointsDeducted_Item", option);
                record.Add("selectoption", selectoption);
                record.Add("ProblemDescription", "");
                record.Add("PointsDeducted", "");//扣分
                record.Add("BeforeImprovement", "");//改善前
                record.Add("AfterImprovement", "");//改善后
                record.Add("RectificationTime", time == null ? GetTime(DateTime.Now) : GetTime(Convert.ToDateTime(time)));//限期整改时间
                record.Add("Rectification_Confim", "");//整改结果
                recordList1.Add(record);
            }
            return Content(JsonConvert.SerializeObject(recordList1));
        }
        #region---根据当前日期 获取下周四日期
        public string GetTime(DateTime date)
        {
            DateTime time = date;
            switch ((int)date.DayOfWeek)
            {
                case 0:
                    time = date.AddDays(4);
                    break;
                case 1:
                    time = date.AddDays(10);
                    break;
                case 2:
                    time = date.AddDays(9);
                    break;
                case 3:
                    time = date.AddDays(8);
                    break;
                case 4:
                    time = date.AddDays(7);
                    break;
                case 5:
                    time = date.AddDays(6);
                    break;
                default:
                    time = date.AddDays(5);
                    break;
            }
            return time.ToString("yyyy-MM-dd");
        }
        #endregion
        #endregion

        #region--- 删除照片法
        [HttpPost]
        public string DeleteImg(string path, string department, string position, DateTime check_date, string check_Type, string pointsDeducted_Type, string uploadType, int district)//path(路径)
        {
            string datatime = check_date.Year.ToString() + "." + check_date.Month.ToString() + "." + check_date.Day.ToString();
            if (!String.IsNullOrEmpty(path))
            {
                string fileType = path.Substring(path.Length - 4, 4);//扩展名
                string old_path = @"D:" + path.Replace('/', '\\');//整个文件路径
                FileInfo path_file = new FileInfo(old_path);//建立文件对象
                int serial_N = Convert.ToInt16(path_file.Name.Split('_')[1].Split('.')[0]);//文件序号
                string new_path = @"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "_Delete_File\\";//新目录路径
                if (Directory.Exists(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "_Delete_File\\") == false)　//目录是否存在
                {

                    Directory.CreateDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "_Delete_File\\");
                    FileInfo new_file = new FileInfo(new_path + uploadType + "_1" + fileType);
                    path_file.CopyTo(new_file.FullName);//复制文件到新目录
                    path_file.Delete();//删除原文件
                    List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\");
                    int filecount = filesInfo.Where(c => c.Extension == fileType).Count();//文件夹里的文件个数
                    for (int i = serial_N; i < filecount + 1; i++)
                    {
                        string filepath = @"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + (i + 1) + fileType;
                        string newfilepath = @"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + i + fileType;
                        System.IO.File.Move(filepath, newfilepath);
                    }
                }
                else  //已有删除目录时，修改文件名
                {
                    List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "_Delete_File\\");
                    int count = filesInfo.Where(c => c.Extension == fileType).Count();
                    FileInfo new_file = new FileInfo(new_path + uploadType + "_" + (count + 1) + fileType);
                    path_file.CopyTo(new_file.FullName);//复制文件到新目录
                    path_file.Delete();//删除原文件
                    List<FileInfo> picturefilesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\");
                    int filecount = picturefilesInfo.Where(c => c.Extension == fileType).Count();//文件夹里的文件个数
                    for (int i = serial_N; i < filecount + 1; i++)
                    {
                        string filepath = @"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + (i + 1) + fileType;
                        string newfilepath = @"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + i + fileType;
                        System.IO.File.Move(filepath, newfilepath);
                    }
                }
                return "删除成功";
            }
            else
            {
                return "没有路径";
            }
        }
        #endregion

        #region--根据部门、班组获取位置下拉列表
        [HttpPost]
        public ActionResult GetPositionList(string department)
        {
            if (!String.IsNullOrEmpty(department))
            {
                var positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department).Select(c => c.Position).Distinct().ToList();
                JArray result = new JArray();
                JObject record = new JObject();
                if (positionList.Count > 0)
                {
                    record.Add("value", "");
                    record.Add("label", "全部位置");
                    result.Add(record);
                }
                foreach (var item in positionList)
                {
                    JObject List = new JObject();
                    List.Add("value", item);
                    List.Add("label", item);
                    result.Add(List);
                }
                return Content(JsonConvert.SerializeObject(result));
            }
            else return Content("传入数据为空！");
        }
        #endregion

        #region--根据部门、班组获取位置下拉列表
        [HttpPost]
        public ActionResult PositionSelect(string department, string group)
        {
            if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(group))
            {
                var positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Group == group).Select(c => c.Position).Distinct().ToList();
                JArray result = new JArray();
                JObject record = new JObject();
                if (positionList.Count > 0)
                {
                    record.Add("value", "");
                    record.Add("label", "全部位置");
                    result.Add(record);
                }
                foreach (var item in positionList)
                {
                    JObject List = new JObject();
                    List.Add("value", item);
                    List.Add("label", item);
                    result.Add(List);
                }
                return Content(JsonConvert.SerializeObject(result));
            }
            else return Content("传入数据为空！");
        }
        #endregion

        #region--根据部门获取班组下拉列表
        [HttpPost]
        public ActionResult GroupList(string department)
        {
            if (!String.IsNullOrEmpty(department))
            {
                var groupList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department).Select(c => c.Group).Distinct().ToList();
                JArray result = new JArray();
                JObject record = new JObject();
                if (groupList.Count > 0)
                {
                    record.Add("value", "");
                    record.Add("label", "全部班组");
                    result.Add(record);
                }
                foreach (var item in groupList)
                {
                    JObject List = new JObject();
                    List.Add("value", item);
                    List.Add("label", item);
                    result.Add(List);
                }
                return Content(JsonConvert.SerializeObject(result));
            }
            else return Content("传入数据为空！");
        }
        #endregion

        #region--日检、周检、巡检详细页按天查询
        [HttpPost]
        ///department  部门
        ///position    班组
        ///date        检查日期
        ///check_Type  检查类型（日检、周检、巡检）
        ///week        第几周（在周报汇总表点击进去详细页时需要传第几周）在详细页查询时，选择检查类型为周检时可以不传
        ///district    区域号（从日报汇总表，周报、巡检汇总表跳进来需传区域号，在详细页查询则可不传)
        public ActionResult Detail_Query(string department, string position, DateTime date, string check_Type, int? week, int? district)
        {
            JArray result = new JArray();
            List<KPI_7S_Record> list = new List<KPI_7S_Record>();

            if (check_Type != "周检")
            {
                if (!String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == null)
                {//整个部门
                    list = db.KPI_7S_Record.Where(c => c.Department == department && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == date.Day).ToList();
                }
                else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district == null)
                {//按部门、位置查
                    list = db.KPI_7S_Record.Where(c => c.Department == department && c.Position == position && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == date.Day).ToList();
                }
                else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district != null)
                { //按部门、位置、区域号                                
                    list = db.KPI_7S_Record.Where(c => c.Department == department && c.Position == position && c.District == district && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == date.Day).ToList();
                }
            }
            else
            {//等于周检
                JArray weekday = new JArray();
                if (week == null)
                {
                    weekday = getWeekNumInMonth(date);
                }
                if (!String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == null)
                {//按部门查
                    int Weekly = 0;
                    if (week == null) Weekly = Convert.ToInt32(weekday[1]);
                    else Weekly = Convert.ToInt32(week);
                    list = db.KPI_7S_Record.Where(c => c.Department == department && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Week == Weekly).ToList();
                }
                else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district == null)
                {
                    //按部门查
                    int Weekly = 0;
                    if (week == null) Weekly = Convert.ToInt32(weekday[1]);
                    else Weekly = Convert.ToInt32(week);
                    list = db.KPI_7S_Record.Where(c => c.Department == department && c.Position == position && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Week == Weekly).ToList();
                }
                else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district != null)
                {
                    int Weekly = 0;
                    if (week == null) Weekly = Convert.ToInt32(weekday[1]);
                    else Weekly = Convert.ToInt32(week);
                    list = db.KPI_7S_Record.Where(c => c.Department == department && c.Position == position && c.District == district && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Week == Weekly).ToList();
                }
            }
            foreach (var item in list)
            {
                JObject record = new JObject();
                record.Add("Id", item.Id);//ID
                record.Add("Date", item.Date.ToString("yyyy-MM-dd"));//检查日期
                record.Add("Department", item.Department);//部门
                record.Add("Group", item.Position);//班组
                record.Add("District", item.District); //区域号
                                                       // record.Add("ResponsiblePerson", item.ResponsiblePerson);//责任人
                record.Add("PointsDeducted_Type", item.PointsDeducted_Type);//扣分类型
                JArray PD_Item = new JArray();
                if (item.Remark != "未交表")
                {
                    var pdItem = item.PointsDeducted_Item.Split('|');
                    foreach (var i in pdItem)
                    {
                        PD_Item.Add(i);
                    }
                }
                record.Add("PointsDeducted_Item", PD_Item);//扣分参考标准
                record.Add("ProblemDescription", item.ProblemDescription);//问题描述
                record.Add("PointsDeducted", item.PointsDeducted.ToString("0.##"));//7S扣分
                if (check_Type != "日检")
                {
                    record.Add("RectificationPoints", item.RectificationPoints == 0 ? " " : item.RectificationPoints.ToString("0.##"));//限期未整改扣分
                    record.Add("RepetitionPointsDeducted", item.RepetitionPointsDeducted == 0 ? " " : item.RepetitionPointsDeducted.ToString("0.##"));//重复出现扣分
                }
                if (item.PointsDeducted_Type != null)//扣分类型不等于空的 是不扣分的记录，不扣分的记录有图片上传
                {
                    JArray beforeImprovementImage = new JArray();
                    beforeImprovementImage = QueryImage(item.Department, item.Position, item.Date, check_Type, item.PointsDeducted_Type, "改善前", item.District);
                    record.Add("BeforeImprovement", beforeImprovementImage);//改善前
                    JArray afterImprovementImage = new JArray();
                    afterImprovementImage = QueryImage(item.Department, item.Position, item.Date, check_Type, item.PointsDeducted_Type, "改善后", item.District);
                    record.Add("AfterImprovement", afterImprovementImage);//改善后
                }
                else
                {
                    JArray beforeImprovementImage = new JArray();
                    record.Add("BeforeImprovement", beforeImprovementImage);//改善前
                    JArray afterImprovementImage = new JArray();
                    record.Add("AfterImprovement", afterImprovementImage);//改善后
                }
                record.Add("RectificationTime", item.RectificationTime == null ? null : item.RectificationTime?.ToString("yyyy-MM-dd"));//限期整改时间
                record.Add("Rectification_Confim", item.Rectification_Confim);//整改结果
                result.Add(record);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region--日检、周检、巡检详细页按月查询
        [HttpPost]
        ///department  部门
        ///position    班组
        ///date        检查日期
        ///check_Type  检查类型（日检、周检、巡检）       
        ///district    区域号(如果是在详细页查询，不需要传区域号)
        public ActionResult Month_Query(string department, string position, DateTime date, string check_Type, int? district)
        {
            JArray result = new JArray();
            List<KPI_7S_Record> list = new List<KPI_7S_Record>();
            if (!String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == null)
            {//整个部门
                list = db.KPI_7S_Record.Where(c => c.Department == department && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Month == date.Month).ToList();
            }
            else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district == null)
            {//按部门、位置查
                list = db.KPI_7S_Record.Where(c => c.Department == department && c.Position == position && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Month == date.Month).ToList();
            }
            else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district != null)
            { //按部门、位置、区域号
                list = db.KPI_7S_Record.Where(c => c.Department == department && c.Position == position && c.Check_Type == check_Type && c.District == district && c.Date.Year == date.Year && c.Month == date.Month).ToList();
            }
            foreach (var item in list)
            {
                JObject record = new JObject();
                record.Add("Id", item.Id);//ID
                record.Add("Date", item.Date.ToString("yyyy-MM-dd"));//检查日期
                record.Add("Department", item.Department);//部门
                record.Add("Group", item.Position);//班组
                record.Add("District", item.District); //区域号
                //record.Add("ResponsiblePerson", item.ResponsiblePerson);//责任人
                record.Add("PointsDeducted_Type", item.PointsDeducted_Type);//扣分类型
                JArray PD_Item = new JArray();
                if (item.PointsDeducted_Item != null)
                {
                    var pdItem = item.PointsDeducted_Item.Split('|');
                    foreach (var i in pdItem)
                    {
                        PD_Item.Add(i);
                    }
                }
                record.Add("PointsDeducted_Item", PD_Item);//扣分参考标准
                record.Add("ProblemDescription", item.ProblemDescription);//问题描述
                record.Add("PointsDeducted", item.PointsDeducted.ToString("0.##"));//7S扣分
                if (check_Type != "日检")
                {
                    record.Add("RectificationPoints", item.RectificationPoints == 0 ? " " : item.RectificationPoints.ToString("0.##"));//限期未整改扣分
                    record.Add("RepetitionPointsDeducted", item.RepetitionPointsDeducted == 0 ? " " : item.RepetitionPointsDeducted.ToString("0.##"));//重复出现扣分
                }
                if (item.PointsDeducted_Type != null)//扣分类型不等于空的 是不扣分的记录，不扣分的记录有图片上传
                {
                    JArray beforeImprovementImage = new JArray();
                    beforeImprovementImage = QueryImage(item.Department, item.Position, item.Date, check_Type, item.PointsDeducted_Type, "改善前", item.District);
                    record.Add("BeforeImprovement", beforeImprovementImage);//改善前
                    JArray afterImprovementImage = new JArray();
                    afterImprovementImage = QueryImage(item.Department, item.Position, item.Date, check_Type, item.PointsDeducted_Type, "改善后", item.District);
                    record.Add("AfterImprovement", afterImprovementImage);//改善后
                }
                else
                {
                    JArray beforeImprovementImage = new JArray();
                    record.Add("BeforeImprovement", beforeImprovementImage);//改善前
                    JArray afterImprovementImage = new JArray();
                    record.Add("AfterImprovement", afterImprovementImage);//改善后
                }
                record.Add("RectificationTime", item.RectificationTime == null ? null : item.RectificationTime?.ToString("yyyy-MM-dd"));//限期整改时间
                record.Add("Rectification_Confim", item.Rectification_Confim);//整改结果
                result.Add(record);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        #endregion

        #region---详细页删除方法
        public ActionResult KPI_7S_Delete(int id)
        {
            if (id != 0)
            {
                var list = db.KPI_7S_Record.Where(c => c.Id == id).ToList();
                var record = list.FirstOrDefault();
                if (record.Remark != "未交表")
                {
                    //1.查找出改善前、改善后的图片   string department, string position, DateTime check_date, string check_Type, string pointsDeducted_Type, string uploadType, int district
                    JArray beforeImprovementImage = new JArray();
                    beforeImprovementImage = QueryImage(record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善前", record.District);
                    JArray afterImprovementImage = new JArray();
                    afterImprovementImage = QueryImage(record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善后", record.District);
                    if (beforeImprovementImage.Count > 0 && beforeImprovementImage.Count < 2)
                    {
                        foreach (var item in beforeImprovementImage)
                        {
                            DeleteImg(item.ToString(), record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善前", record.District);
                        }
                    }
                    else
                    {
                        foreach (var item in beforeImprovementImage)
                        {
                            beforeImprovementImage = QueryImage(record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善前", record.District);
                            DeleteImg(beforeImprovementImage[0].ToString(), record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善前", record.District);
                        }
                    }
                    if (afterImprovementImage.Count > 0 && afterImprovementImage.Count < 2)
                    {
                        foreach (var item in afterImprovementImage)
                        {
                            DeleteImg(item.ToString(), record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善后", record.District);
                        }
                    }
                    else
                    {
                        foreach (var item in afterImprovementImage)
                        {
                            afterImprovementImage = QueryImage(record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善后", record.District);
                            DeleteImg(afterImprovementImage[0].ToString(), record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善后", record.District);
                        }
                    }
                    UserOperateLog operaterecord = new UserOperateLog();
                    operaterecord.OperateDT = DateTime.Now;
                    operaterecord.Operator = ((Users)Session["User"]).UserName;
                    operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除了7S记录表的扣分记录,类型为：" + record.Check_Type + ",扣分类型为：" + record.PointsDeducted_Type + "扣分值为：" + record.PointsDeducted + "，记录Id为：" + record.Id + ".";
                    db.KPI_7S_Record.RemoveRange(list);
                    db.UserOperateLog.Add(operaterecord);//保存删除日志
                    int count = db.SaveChanges();
                    if (count > 0) return Content("删除成功！");
                    else return Content("删除失败！");
                }
                else
                {
                    UserOperateLog operaterecord = new UserOperateLog();
                    operaterecord.OperateDT = DateTime.Now;
                    operaterecord.Operator = ((Users)Session["User"]).UserName;
                    operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除了7S记录表的扣分记录,类型为：" + record.Check_Type + ",扣分类型为：" + record.PointsDeducted_Type + "扣分值为：" + record.PointsDeducted + "，记录Id为：" + record.Id + ".";
                    db.KPI_7S_Record.RemoveRange(list);
                    db.UserOperateLog.Add(operaterecord);//保存删除日志
                    int count = db.SaveChanges();
                    if (count > 0) return Content("删除成功！");
                    else return Content("删除失败！");
                }
            }
            return Content("");
        }

        #endregion

        #region--审核限期整改记录
        public ActionResult Approve(int id, bool result)
        {
            var record = db.KPI_7S_Record.FirstOrDefault(c => c.Id == id);
            record.Rectification_Confim = result;//整改结果
            record.RectificationPerson = ((Users)Session["User"]).UserName;//整改人
            record.ModifyTime = DateTime.Now;//整改时间
            if (result == false)
            {
                record.RectificationPoints = record.PointsDeducted;//整改不通过扣同样的分
            }
            int count = db.SaveChanges();
            if (count > 0) return Content("审核成功");
            else return Content("审核失败");
        }
        #endregion

        #region--日检、巡检汇总表
        public ActionResult Daily_SumQuery(string department, DateTime date, string position, int? district, string check_Type)
        {
            if (!String.IsNullOrEmpty(department))
            {
                DateTime dt = date;
                int sumday = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1).Day;//一个月有多少天               
                JArray result = new JArray();
                JArray arr = new JArray();
                List<KPI_7S_DistrictPosition> positionList = new List<KPI_7S_DistrictPosition>();
                var depTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= date).Select(c => c.VersionsTime).Distinct().ToList().Max();//获取版本时间
                if (department == "全部部门" && String.IsNullOrEmpty(position) && district == null)//全部部门
                {
                    positionList = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == depTime).ToList();
                }
                else if (department != "全部部门" && !String.IsNullOrEmpty(position) && district == null)//部门、位置
                {
                    positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.VersionsTime == depTime).ToList();
                }
                else if (department != "全部部门" && !String.IsNullOrEmpty(position) && district != null)//部门、位置、区域
                {
                    positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.District == district && c.VersionsTime == depTime).ToList();
                }
                else if (department != "全部部门" && !String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == null)//单个部门
                {
                    positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.VersionsTime == depTime).ToList();
                }
                List<KPI_7S_Record> recordList = new List<KPI_7S_Record>();

                if (check_Type == "日检")
                {
                    recordList = db.KPI_7S_Record.Where(c => c.Date.Year == date.Year && c.Date.Month == date.Month && c.Check_Type == "日检").ToList();//找出所有部门整个月的数据
                }
                else
                {
                    recordList = db.KPI_7S_Record.Where(c => c.Date.Year == date.Year && c.Date.Month == date.Month && c.Check_Type == "巡检").ToList();//找出所有部门整个月的数据
                }
                foreach (var item in positionList)
                {
                    JArray dateList = new JArray();
                    JObject list = new JObject();
                    list.Add("Department", item.Department);//部门
                    list.Add("Position", item.Position);//位置
                    list.Add("District", item.District);//区域
                    decimal sum = 0;//得分小计
                    for (var i = 1; i <= sumday; i++)
                    {
                        decimal grade = 0;
                        if (check_Type == "日检")
                        {
                            grade = recordList.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == i).Select(c => c.PointsDeducted).FirstOrDefault();
                        }
                        else
                        {
                            decimal grade1 = recordList.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == i && c.Check_Type == "巡检").Select(c => c.PointsDeducted).DefaultIfEmpty().Sum();//7S扣分
                            decimal grade2 = recordList.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == i && c.Check_Type == "巡检").Select(c => c.RepetitionPointsDeducted).DefaultIfEmpty().Sum();//重复出现扣分
                            decimal grade3 = recordList.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == i && c.Check_Type == "巡检").Select(c => c.RectificationPoints).DefaultIfEmpty().Sum();//限期未整改扣分
                            grade = grade1 + grade2 + grade3;//7S扣分+重出现扣分+未整改扣分
                        }
                        if (grade == 0) dateList.Add(" ");
                        else dateList.Add(grade);
                        sum += grade;
                    }
                    list.Add("PointsDeducted", dateList);
                    list.Add("GradeSum", sum);
                    result.Add(list);
                }
                return Content(JsonConvert.SerializeObject(result));
            }
            return Content("");
        }
        #endregion

        #region--周检汇总表
        [HttpPost]
        public ActionResult Week_SumQuery(string department, DateTime date, string position, int? district)
        {
            DateTime CurDate = date;  // 当前指定月份
            DateTime dt = CurDate.AddDays(1 - CurDate.Day).AddMonths(1).AddDays(-1);  // 返回指定当前月份的最后一
            JArray weekday = new JArray();
            weekday = getWeekNumInMonth(dt);//获取一个月有几周

            List<KPI_7S_DistrictPosition> positionList = new List<KPI_7S_DistrictPosition>();
            var list = db.KPI_7S_Record.Where(c => c.Date.Year == date.Year && c.Month == date.Month && c.Check_Type == "周检").ToList();//找出当月等于周检的记录
            var depTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= date).Select(c => c.VersionsTime).Distinct().ToList().Max();//获取版本时间
            if (department == "全部部门" && String.IsNullOrEmpty(position) && district == null)//全部部门
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && !String.IsNullOrEmpty(position) && district == null)//部门、位置
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && !String.IsNullOrEmpty(position) && district != null)//部门、位置、区域
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.District == district && c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && !String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == null)//单个部门
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.VersionsTime == depTime).ToList();
            }
            JArray result = new JArray();
            List<KPI_7S_Record> depList = new List<KPI_7S_Record>();
            foreach (var item in positionList)
            {
                JArray dateList = new JArray();
                JObject redlist = new JObject();
                redlist.Add("Department", item.Department);//部门
                redlist.Add("Position", item.Position);//位置
                redlist.Add("District", item.District);//区域
                decimal sum = 0;//扣分合计
                for (var i = 1; i <= Convert.ToInt32(weekday[1]); i++)//周
                {
                    decimal grade = 0;
                    decimal Grade_week = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Week == i).Select(c => c.PointsDeducted).DefaultIfEmpty().DefaultIfEmpty().Sum();//正常扣分
                    decimal Week_NotCorrected = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Week == i).Select(c => c.RectificationPoints).DefaultIfEmpty().DefaultIfEmpty().Sum();//限期未整改扣分
                    decimal Week_repetition = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Week == i).Select(c => c.RepetitionPointsDeducted).DefaultIfEmpty().DefaultIfEmpty().Sum();//重复出现改扣分
                    grade = Grade_week + Week_NotCorrected + Week_repetition;
                    if (grade == 0) dateList.Add(" ");
                    else dateList.Add(grade);
                    sum += grade;
                }
                redlist.Add("Week", dateList);
                redlist.Add("GradeSum", sum);
                result.Add(redlist);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region---7S汇总表
        public ActionResult KPI_7S_SummarySheet(string department, string group, string position, int? district, DateTime date)
        {
            JArray result = new JArray();
            List<KPI_7S_DistrictPosition> positionList = new List<KPI_7S_DistrictPosition>();
            var depTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= date).Select(c => c.VersionsTime).Distinct().ToList().Max();//获取版本时间
            if (department == "全部部门" && String.IsNullOrEmpty(position) && district == null && String.IsNullOrEmpty(group))//全部部门
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && String.IsNullOrEmpty(position) && district == null && !String.IsNullOrEmpty(group))//部门、班组
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Group == group && c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && !String.IsNullOrEmpty(position) && district == null && !String.IsNullOrEmpty(group))//部门、班组、位置
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.Group == group && c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && !String.IsNullOrEmpty(position) && district != null && !String.IsNullOrEmpty(group))//部门、班组、位置、区域
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Group == group && c.Position == position && c.District == district && c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && !String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == null)//单个部门
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.VersionsTime == depTime).ToList();
            }
            var list = db.KPI_7S_Record.Where(c => c.Date.Year == date.Year && c.Month == date.Month).ToList();//找出当月的记录
            foreach (var item in positionList)
            {
                JObject redlist = new JObject();
                redlist.Add("Department", item.Department);//部门
                redlist.Add("Group", item.Group == null ? " " : item.Group);//区域
                redlist.Add("Position", item.Position);//位置
                redlist.Add("District", item.District);//区域                
                var Grade_daily = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "日检").Select(c => c.PointsDeducted).DefaultIfEmpty().Sum();//日检
                var Grade_week = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "周检").Select(c => c.PointsDeducted).DefaultIfEmpty().Sum();//周检
                var Grade_random = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "巡检").Select(c => c.PointsDeducted).DefaultIfEmpty().Sum();//巡检正常扣分

                var Week_NotCorrected = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "周检").Select(c => c.RectificationPoints).DefaultIfEmpty().Sum();//周检未整改
                var Random_NotCorrected = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "巡检").Select(c => c.RectificationPoints).DefaultIfEmpty().Sum();//巡检未整改

                var Week_Repetition = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "周检").Select(c => c.RepetitionPointsDeducted).DefaultIfEmpty().Sum();//周检重复出现
                var Random_Repetition = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "巡检").Select(c => c.RepetitionPointsDeducted).DefaultIfEmpty().Sum();//巡检重复出现

                decimal total = 100 - Grade_daily - Grade_week - (Week_NotCorrected + Random_NotCorrected) - (Week_Repetition + Random_Repetition) - Grade_random;//实际得分
                redlist.Add("Grade_daily", Grade_daily);//日检扣分
                redlist.Add("Grade_week", Grade_week);//周检正常扣分     
                redlist.Add("Week_NotCorrected", Week_NotCorrected);//周检未整改扣分
                redlist.Add("Week_Repetition", Week_Repetition);//周检重复出现扣分
                redlist.Add("Week_Sum", Grade_week + Week_NotCorrected + Week_Repetition);//周检扣分合计

                redlist.Add("Grade_random", Grade_random);//巡检正常扣分
                redlist.Add("Random_NotCorrected", Random_NotCorrected);//巡检未整改扣分
                redlist.Add("Random_Repetition", Random_Repetition);//巡检重复出现扣分
                redlist.Add("Random_Sum", Grade_random + Random_NotCorrected + Random_Repetition);//巡检扣分合计
                redlist.Add("TotalPoints", total);//月末得分
                result.Add(redlist);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region---日检不交表扣分
        public void KPI_7S_daily()
        {
            while (true)
            {
                DateTime time = DateTime.Now;
                if (time.Hour == 17 && time.Minute == 0)  //如果当前时间是14点00分
                {
                    var depTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= time).Select(c => c.VersionsTime).Distinct().ToList().Max();//获取版本时间
                    IEnumerable<KPI_7S_DistrictPosition> list = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == depTime);
                    var depList = list.Select(c => c.Department).Distinct().ToList();//先找出部门
                    IEnumerable<KPI_7S_Record> allList = db.KPI_7S_Record.Where(c => c.Date.Year == time.Year && c.Date.Month == time.Month && c.Check_Type == "日检");//查找当月所有记录
                    foreach (var item in depList)
                    {
                        var record = list.Where(c => c.Department == item).ToList();//找出部门位置区域                                                                                 
                        if (allList.Count(c => c.Department == item && c.Date.Year == time.Year && c.Date.Month == time.Month && c.Date.Day == time.Day) < 1)   //判断今天是否有记录
                        {
                            JArray weekday = new JArray();
                            weekday = getWeekNumInMonth(time);
                            if (Convert.ToInt32(weekday[0]) == 3)//判断一周有3天 等于三天就查询前两是否有提交,只要有一天有记录，那么本周这个部门的所有区域都不扣分
                            {
                                if (time.DayOfWeek.ToString() == "Friday")
                                {
                                    DateTime yesterday = time.AddDays(-1);//昨天
                                    DateTime beforeday = time.AddDays(-2);//前天
                                    if (allList.Count(c => c.Department == item && c.Date.Year == yesterday.Year && c.Date.Month == yesterday.Month && c.Date.Day == yesterday.Day && c.PointsDeducted_Type != null) == 0)//判断昨天是否有上传
                                    {
                                        if (allList.Count(c => c.Department == item && c.Date.Year == beforeday.Year && c.Date.Month == beforeday.Month && c.Date.Day == beforeday.Day && c.PointsDeducted_Type != null) == 0)//判断前天是否有，等于零没有
                                        {
                                            CreateRecord(record, Convert.ToInt32(weekday[1]), item);
                                            string address = "";
                                            var adress_list = db.UserItemEmail.Where(c => c.ProcesName == "7S发送").ToList();//找出发送人名单
                                            var email = commom.SendEmail("zhongql@lcjh.local", adress_list.Select(c => c.EmailAddress).ToList(), "日检不交表邮件通知1", "不交7S日检表", adress_list.Select(c => c.EmailAddress).ToList(), address);
                                        }
                                    }
                                }
                            }
                            else if (Convert.ToInt32(weekday[0]) == 4)//如果一周除去周末有4天，本周从星期二开始
                            {
                                if (time.DayOfWeek.ToString() == "Thursday" || time.DayOfWeek.ToString() == "Friday")
                                {
                                    DateTime yesterday = time.AddDays(-1);//昨天
                                    DateTime beforeday = time.AddDays(-2);//前天
                                    DateTime beforeday1 = time.AddDays(-3);//大前天
                                    int submitNum = 1;//本来今天没交，星期二星期三都不进来检查是否有提交，星期四第三天没交就进来检查
                                    if (allList.Count(c => c.Department == item && c.Date.Year == yesterday.Year && c.Date.Month == yesterday.Month && c.Date.Day == yesterday.Day && c.PointsDeducted_Type != null) == 0)//昨天
                                    {
                                        submitNum++;
                                    }
                                    if (allList.Count(c => c.Department == item && c.Date.Year == beforeday.Year && c.Date.Month == beforeday.Month && c.Date.Day == beforeday.Day && c.PointsDeducted_Type != null) == 0)//前天
                                    {
                                        submitNum++;
                                    }
                                    if (time.DayOfWeek.ToString() == "Friday")
                                    {
                                        if (allList.Count(c => c.Department == item && c.Date.Year == beforeday1.Year && c.Date.Month == beforeday1.Month && c.Date.Day == beforeday1.Day && c.PointsDeducted_Type != null) == 0)//大前天
                                        {
                                            submitNum++;
                                        }
                                    }
                                    if (submitNum >= 3)
                                    {
                                        CreateRecord(record, Convert.ToInt32(weekday[1]), item);
                                        string address = "";
                                        var adress_list = db.UserItemEmail.Where(c => c.ProcesName == "7S发送").ToList();//找出发送人名单
                                        var email = commom.SendEmail("zhongql@lcjh.local", adress_list.Select(c => c.EmailAddress).ToList(), "日检不交表邮件通知2", "不交7S日检表", adress_list.Select(c => c.EmailAddress).ToList(), address);
                                    }
                                }
                            }
                            else if (Convert.ToInt32(weekday[0]) == 5)//如果第一周有5天
                            {
                                if (time.DayOfWeek.ToString() == "Wednesday" || time.DayOfWeek.ToString() == "Thursday" || time.DayOfWeek.ToString() == "Friday")
                                {
                                    DateTime yesterday = time.AddDays(-1);//昨天
                                    DateTime beforeday = time.AddDays(-2);//前天
                                    DateTime beforeday1 = time.AddDays(-3);//前前天
                                    DateTime beforeday2 = time.AddDays(-4);//前前前天
                                                                           //如果星期三没交，就检查前两天是否提交
                                    int submitNum = 1;//星期三没交初始值等于1
                                    if (allList.Count(c => c.Department == item && c.Date.Year == yesterday.Year && c.Date.Month == yesterday.Month && c.Date.Day == yesterday.Day && c.PointsDeducted_Type != null) == 0)//昨天
                                    {
                                        submitNum++;
                                    }
                                    if (allList.Count(c => c.Department == item && c.Date.Year == beforeday.Year && c.Date.Month == beforeday.Month && c.Date.Day == beforeday.Day && c.PointsDeducted_Type != null) == 0)//前天
                                    {
                                        submitNum++;
                                    }
                                    if (time.DayOfWeek.ToString() == "Thursday")
                                    {
                                        if (allList.Count(c => c.Department == item && c.Date.Year == beforeday1.Year && c.Date.Month == beforeday1.Month && c.Date.Day == beforeday1.Day && c.PointsDeducted_Type != null) == 0)//前前天
                                        {
                                            submitNum++;
                                        }
                                    }
                                    if (time.DayOfWeek.ToString() == "Friday")
                                    {
                                        if (allList.Count(c => c.Department == item && c.Date.Year == beforeday1.Year && c.Date.Month == beforeday1.Month && c.Date.Day == beforeday1.Day && c.PointsDeducted_Type != null) == 0)//前前天
                                        {
                                            submitNum++;
                                        }
                                        if (allList.Count(c => c.Department == item && c.Date.Year == beforeday2.Year && c.Date.Month == beforeday2.Month && c.Date.Day == beforeday2.Day && c.PointsDeducted_Type != null) == 0)//前前前天
                                        {
                                            submitNum++;
                                        }
                                    }
                                    if (submitNum >= 3)
                                    {
                                        CreateRecord(record, Convert.ToInt32(weekday[1]), item);
                                        string address = "";
                                        var adress_list = db.UserItemEmail.Where(c => c.ProcesName == "7S发送").ToList();//找出发送人名单
                                        var email = commom.SendEmail("zhongql@lcjh.local", adress_list.Select(c => c.EmailAddress).ToList(), "日检不交表邮件通知3", "不交7S日检表", adress_list.Select(c => c.EmailAddress).ToList(), address);
                                    }
                                }
                            }
                        }
                    }
                    string address1 = "";
                    var adress_list1 = db.UserItemEmail.Where(c => c.ProcesName == "7S发送").ToList();//找出发送人名单
                    var email1 = commom.SendEmail("zhongql@lcjh.local", adress_list1.Select(c => c.EmailAddress).ToList(), "测试日检不交表邮件通知4", "不交7S日检表", adress_list1.Select(c => c.EmailAddress).ToList(), address1);
                    Thread.CurrentThread.Join(new TimeSpan(0, 2, 0));//阻止设定时间
                }
                else
                {
                    TimeSpan span = new TimeSpan();
                    DateTime t1 = new DateTime(time.Year, time.Month, time.Day, hour: 17, minute: 0, second: 0);
                    if (time.Hour >= 17 && time.Minute > 0)
                    {
                        span = t1.AddDays(1) - time;
                    }
                    else span = t1 - time;
                    Thread.CurrentThread.Join(new TimeSpan(span.Hours, span.Minutes, 0));//阻止设定时间
                }
            }
        }

        public void CreateRecord(List<KPI_7S_DistrictPosition> list, int week, string department)
        {
            IEnumerable<KPI_7S_Record> record = db.KPI_7S_Record.Where(c => c.Department == department && c.Date.Year == DateTime.Now.Year && c.Date.Month == DateTime.Now.Month && c.Check_Type != "周检" && c.Check_Type != "巡检");
            foreach (var j in list)
            {
                List<KPI_7S_Record> newList = new List<KPI_7S_Record>();
                KPI_7S_Record newRecord = new KPI_7S_Record();
                newRecord.Department = j.Department;//部门
                newRecord.Group = j.Position;//班组
                newRecord.Position = j.Position;//位置
                newRecord.District = j.District;//区域号
                newRecord.Check_Type = "日检";//检查类型
                newRecord.Date = DateTime.Now;//日期
                newRecord.PointsDeducted_Type = null;//扣分类型
                newRecord.PointsDeducted_Item = null;//7S扣分项
                newRecord.ProblemDescription = DateTime.Now.ToString("yyyy-MM-dd") + ",未交日报表扣分";//问题描述
                newRecord.PointsDeducted = 1;//7S扣分
                newRecord.InputPerson = null;//录入人
                newRecord.InputTime = DateTime.Now;//录入时间
                newRecord.RectificationTime = null;// 限期整改时间
                newRecord.RectificationResults = null;//整改结果描述
                newRecord.RectificationPerson = null;
                newRecord.ModifyTime = null;
                newRecord.RectificationPoints = 0;
                newRecord.RepetitionPointsDeducted = 0;
                newRecord.Rectification_Confim = null;//整改结果
                newRecord.Week = week;
                newRecord.Year = DateTime.Now.Year;
                newRecord.Month = DateTime.Now.Month;
                newRecord.Remark = "未交表";//备注
                if (record.Count(c => c.Department == j.Department && c.Position == j.Position && c.District == j.District && c.Date.Year == DateTime.Now.Year && c.Date.Month == DateTime.Now.Month && c.Date.Day == DateTime.Now.Day && c.Remark == "未交表") < 1)
                {
                    newList.Add(newRecord);
                    db.KPI_7S_Record.AddRange(newList);
                    db.SaveChanges();
                }
            }
        }
        public JArray getWeekNumInMonth(DateTime daytime)
        {
            JArray res = new JArray();
            int dayInMonth = daytime.Day;
            DateTime firstDay = daytime.AddDays(1 - daytime.Day);//本月第一天            
            int weekday = (int)firstDay.DayOfWeek == 0 ? 7 : (int)firstDay.DayOfWeek;//本月第一天是周几           
            int firstWeekEndDay = 7 - (weekday - 1);//本月第一周有几天                     
            int diffday = dayInMonth - firstWeekEndDay;//当前日期和第一周之差
            diffday = diffday > 0 ? diffday : 1;
            int WeekNumInMonth = ((diffday % 7) == 0 ? (diffday / 7 - 1) : (diffday / 7)) + 1 + (dayInMonth > firstWeekEndDay ? 1 : 0);//当前是第几周,如果整除7就减一天
            if (WeekNumInMonth == 1)
            {
                int weekdays = firstWeekEndDay - 2;//去掉周末两天
                res.Add(weekdays);
            }
            else if (WeekNumInMonth == 2 || WeekNumInMonth == 3)
            {
                res.Add(5);
            }
            else if (WeekNumInMonth == 4 || WeekNumInMonth == 5 || WeekNumInMonth == 6)
            {
                DateTime startWeek = daytime.AddDays(1 - Convert.ToInt32(daytime.DayOfWeek.ToString("d")));  //本周周一
                DateTime endWeek = startWeek.AddDays(6);  //本周周日
                if (endWeek.Month != daytime.Month)
                {
                    if (endWeek.Day == 6)//本周最后一天等于6号，那么这周出去星期周末只有一天
                    {
                        res.Add(1);
                    }
                    else if (endWeek.Day == 5)//本周最后一天等于5号，那么这周出去星期周末只有两天
                    {
                        res.Add(2);
                    }
                    else if (endWeek.Day == 4)//本周最后一天等于4号，那么这周出去星期周末只有三天
                    {
                        res.Add(3);
                    }
                    else if (endWeek.Day == 3)//本周最后一天等于3号，那么这周出去星期周末只有四天
                    {
                        res.Add(4);
                    }
                    else if (endWeek.Day == 2)//本周最后一天等于2号，那么这周出去星期周末只有五天
                    {
                        res.Add(5);
                    }
                    else if (endWeek.Day == 1)//本周最后一天等于1号，那么这周出去星期周末只有五天
                    {
                        res.Add(5);
                    }
                }
                else
                {
                    res.Add(5);
                }
            }
            res.Add(WeekNumInMonth);
            return res;
        }
        #endregion

        #region---导出excel
        [HttpPost]
        public FileContentResult ExportExcel(string value, DateTime check_date, string check_type)
        {
            JArray total = JsonConvert.DeserializeObject<JArray>(value);
            JArray result = new JArray();
            //导出表格名称
            string tableName = "";
            if (check_type == "日检")
            {
                tableName = "(" + check_date.Year + "年" + check_date.Month + "月" + check_date.Day + "日" + "7S日检汇总表)";
            }
            else if (check_type == "周检")
            {
                tableName = "(" + check_date.Year + "年" + check_date.Month + "月" + check_date.Day + "日" + "7S周检汇总表)";
            }
            else if (check_type == "巡检")
            {
                tableName = "(" + check_date.Year + "年" + check_date.Month + "月" + check_date.Day + "日" + "7S巡检汇总表)";
            }
            else
            {//7S汇总表导出
                tableName = "(" + check_date.Year + "年" + check_date.Month + "月" + "7S汇总表)";
            }

            JObject resultitem = new JObject();
            resultitem.Add("DataTable", total);
            if (check_type == "日检" || check_type == "巡检")
            {
                int sumday = check_date.AddDays(1 - check_date.Day).AddMonths(1).AddDays(-1).Day;
                if (sumday == 28)
                {
                    string[] DayArraycolumns = { "部门", "位置", "区域号", "1日", "2日", "3日", "4日", "5日", "6日", "7日", "8日", "9日", "10日", "11日", "12日", "13日", "14日", "15日", "16日", "17日", "18日", "19日", "20日", "21日", "22日", "23日", "24日", "25日", "26日", "27日", "28日", "扣分合计" };
                    resultitem.Add("Columns", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(DayArraycolumns)));
                }
                else if (sumday == 29)
                {
                    string[] DayArraycolumns = { "部门", "位置", "区域号", "1日", "2日", "3日", "4日", "5日", "6日", "7日", "8日", "9日", "10日", "11日", "12日", "13日", "14日", "15日", "16日", "17日", "18日", "19日", "20日", "21日", "22日", "23日", "24日", "25日", "26日", "27日", "28日", "29日", "扣分合计" };
                    resultitem.Add("Columns", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(DayArraycolumns)));
                }
                else if (sumday == 30)
                {
                    string[] DayArraycolumns = { "部门", "位置", "区域号", "1日", "2日", "3日", "4日", "5日", "6日", "7日", "8日", "9日", "10日", "11日", "12日", "13日", "14日", "15日", "16日", "17日", "18日", "19日", "20日", "21日", "22日", "23日", "24日", "25日", "26日", "27日", "28日", "29日", "30日", "扣分合计" };
                    resultitem.Add("Columns", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(DayArraycolumns)));
                }
                else
                {
                    string[] DayArraycolumns = { "部门", "位置", "区域号", "1日", "2日", "3日", "4日", "5日", "6日", "7日", "8日", "9日", "10日", "11日", "12日", "13日", "14日", "15日", "16日", "17日", "18日", "19日", "20日", "21日", "22日", "23日", "24日", "25日", "26日", "27日", "28日", "29日", "30日", "31日", "扣分合计" };
                    resultitem.Add("Columns", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(DayArraycolumns)));
                }
            }
            else if (check_type == "周检")
            {
                DateTime CurDate = check_date;
                DateTime dt = CurDate.AddDays(1 - CurDate.Day).AddMonths(1).AddDays(-1);
                JArray weekday = new JArray();
                weekday = getWeekNumInMonth(dt);
                if (Convert.ToInt32(weekday[1]) == 4)
                {
                    string[] DayArraycolumns = { "部门", "位置", "区域号", "第一周", "第二周", "第三周", "第四周", "月末的分" };
                    resultitem.Add("Columns", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(DayArraycolumns)));
                }
                else if (Convert.ToInt32(weekday[1]) == 5)
                {
                    string[] DayArraycolumns = { "部门", "位置", "区域号", "第一周", "第二周", "第三周", "第四周", "第五周", "月末的分" };
                    resultitem.Add("Columns", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(DayArraycolumns)));
                }
                else
                {
                    string[] DayArraycolumns = { "部门", "位置", "区域号", "第一周", "第二周", "第三周", "第四周", "第五周", "第六周", "月末的分" };
                    resultitem.Add("Columns", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(DayArraycolumns)));
                }
            }
            else
            {//汇总表
                string[] DayArraycolumns = { "部门", "班组", "位置", "区域号", "日检扣分", "周检正常扣分", "周检未整改扣分", "周检重复出现扣分", "周检扣分合计", "巡检正常扣分", "巡检未整改扣分", "巡检重复出现扣分", "巡检扣分合计", "月末得分" };
                resultitem.Add("Columns", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(DayArraycolumns)));
            }
            result.Add(resultitem);
            resultitem = new JObject();
            byte[] filecontent = ExcelExportHelper.ExportExcel2(result, tableName, false);
            return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
        }
        #endregion

        #region--手机端 获取扣分类型
        [HttpPost]
        public ActionResult GetPointsType()
        {
            var list = db.KPI_7S_ReferenceStandard.OrderByDescending(c => c.PointsType).ToList();
            var record = list.Select(c => c.PointsType).Distinct().ToList();
            JArray result = new JArray();
            foreach (var item in record)
            {
                JObject res = new JObject();
                res.Add("value", item);
                res.Add("label", item);
                result.Add(res);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region--手机端 根据扣分类型获取扣分项
        [HttpPost]
        public ActionResult GetReferenceStandard(string pointsType)
        {
            if (!String.IsNullOrEmpty(pointsType))
            {
                JArray result = new JArray();
                var list = db.KPI_7S_ReferenceStandard.Where(c => c.PointsType == pointsType).Select(c => c.ReferenceStandard).ToList();
                foreach (var item in list)
                {
                    JObject res = new JObject();
                    res.Add("value", item);
                    res.Add("count", 0);
                    result.Add(res);
                }
                return Content(JsonConvert.SerializeObject(result));
            }
            return Content("传入参数为空！");
        }
        #endregion

        #region---手机端 删除未创建记录的照片
        public ActionResult KPI_7S_DeleteImage(string department, string position, DateTime check_date, string check_Type, string pointsDeducted_Type, int district)
        {
            if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && !String.IsNullOrEmpty(check_Type) && !String.IsNullOrEmpty(pointsDeducted_Type) && district != 0)
            {
                JArray beforeImprovementImage = new JArray();
                beforeImprovementImage = QueryImage(department, position, check_date, check_Type, pointsDeducted_Type, "改善前", district);
                JArray afterImprovementImage = new JArray();
                afterImprovementImage = QueryImage(department, position, check_date, check_Type, pointsDeducted_Type, "改善后", district);
                int num = 0;
                int num1 = beforeImprovementImage.Count() + afterImprovementImage.Count();
                if (beforeImprovementImage.Count > 0 && beforeImprovementImage.Count < 2)
                {
                    foreach (var item in beforeImprovementImage)
                    {
                        var res = DeleteImg(item.ToString(), department, position, check_date, check_Type, pointsDeducted_Type, "改善前", district);
                        if (res.ToString() == "删除成功") num++;
                    }
                }
                else
                {
                    foreach (var item in beforeImprovementImage)
                    {
                        beforeImprovementImage = QueryImage(department, position, check_date, check_Type, pointsDeducted_Type, "改善前", district);
                        var res = DeleteImg(beforeImprovementImage[0].ToString(), department, position, check_date, check_Type, pointsDeducted_Type, "改善前", district);
                        if (res.ToString() == "删除成功") num++;
                    }
                }
                if (afterImprovementImage.Count > 0 && afterImprovementImage.Count < 2)
                {
                    foreach (var item in afterImprovementImage)
                    {
                        var res = DeleteImg(item.ToString(), department, position, check_date, check_Type, pointsDeducted_Type, "改善后", district);
                        if (res.ToString() == "删除成功") num++;
                    }
                }
                else
                {
                    foreach (var item in afterImprovementImage)
                    {
                        afterImprovementImage = QueryImage(department, position, check_date, check_Type, pointsDeducted_Type, "改善后", district);
                        var res = DeleteImg(afterImprovementImage[0].ToString(), department, position, check_date, check_Type, pointsDeducted_Type, "改善后", district);
                        if (res.ToString() == "删除成功") num++;
                    }
                }
                if (num == num1) return Content("删除成功");
                else return Content("删除失败");
            }
            return Content("传入参数为空！");
        }
        #endregion

        #region---7S日检交表记录查询
        public ActionResult KPI_7S_DailyRecord()
        {
            return View();
        }
        public ActionResult DailyRecord_Query(DateTime date, string department)
        {
            if (!string.IsNullOrEmpty(department))
            {
                int sumday = date.AddDays(1 - date.Day).AddMonths(1).AddDays(-1).Day;//一个月有多少天  
                DateTime dt = date.AddMonths(1);
                dt = dt.AddDays(0 - dt.Day);
                JArray result = new JArray();
                var versionstime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= dt).Select(c => c.VersionsTime).Distinct().Max();
                List<KPI_7S_DistrictPosition> depList = new List<KPI_7S_DistrictPosition>();
                if (department == "全部部门")
                {
                    depList = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == versionstime).ToList();
                }
                else
                {
                    depList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.VersionsTime == versionstime).ToList();
                }
                var monthList = db.KPI_7S_Record.Where(c => c.Year == date.Year && c.Month == date.Month && c.Check_Type != "周检" && c.Check_Type != "巡检" && c.Remark != "未交表").ToList();
                foreach (var item in depList)
                {
                    JArray dateList = new JArray();
                    JObject redlist = new JObject();
                    redlist.Add("Department", item.Department);//部门
                    redlist.Add("Position", item.Position);//位置
                    redlist.Add("District", item.District);//区域

                    for (int i = 1; i <= sumday; i++)
                    {
                        var res = monthList.Count(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Year == date.Year && c.Month == date.Month && c.Date.Day == i);
                        if (res == 0) dateList.Add("false");
                        else dateList.Add("true");
                    }
                    redlist.Add("DailyRecord", dateList);
                    result.Add(redlist);
                }
                return Content(JsonConvert.SerializeObject(result));
            }
            else return Content("传入参数为空！");
        }
        #endregion

        #region----详细页修改限期整改时间
        public ActionResult RectificationTime_modify(int id, string time)
        {
            int saveNum = 0;
            if (id != 0)
            {
                var list = db.KPI_7S_Record.Where(c => c.Id == id).FirstOrDefault();
                list.RectificationTime = Convert.ToDateTime(time);
                saveNum = db.SaveChanges();
            }
            if (saveNum > 0) return Content("修改成功！");
            else return Content("修改失败！");
        }
        #endregion

        #region--部门得分排名
        public class ranking_list
        {
            public int Ranking { get; set; }
            public string Department { get; set; }
            public string Score { get; set; }
        }
        public ActionResult KPI_7S_Ranking(DateTime time)
        {
            JArray res = new JArray();
            var depTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= time).Select(c => c.VersionsTime).Distinct().ToList().Max();//获取版本时间
            var depList = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == depTime).Select(c => c.Department).Distinct().ToList();//根据版本时间找出部门
            var allList = db.KPI_7S_Record.Where(c => c.Year == time.Year && c.Month == time.Month).ToList();//找出当月的所有记录
            foreach (var item in depList)
            {
                JObject record = new JObject();
                var daily = allList.Where(c => c.Department == item && c.Check_Type == "日检").Select(c => c.PointsDeducted).Sum();
                var week = allList.Where(c => c.Department == item && c.Check_Type == "周检").Select(c => c.PointsDeducted).Sum();
                var random = allList.Where(c => c.Department == item && c.Check_Type == "巡检").Select(c => c.PointsDeducted).Sum();
                var rectification = allList.Where(c => c.Department == item).Select(c => c.RectificationPoints).Sum();
                var repetition = allList.Where(c => c.Department == item).Select(c => c.RepetitionPointsDeducted).Sum();
                record.Add("Department", item);//部门
                record.Add("Score", 100 - daily - week - random - rectification - repetition);
                res.Add(record);
            }
            var rank = new JArray(res.OrderByDescending(c => c["Score"]));//按高到低排序
            List<ranking_list> result = new List<ranking_list>();
            int num = 1;
            //开始排名
            foreach (var i in rank)
            {
                ranking_list record = new ranking_list();
                if (num == 1)
                {
                    record.Ranking = num;//名次
                    record.Department = i["Department"].ToString();//部门
                    record.Score = i["Score"].ToString();//分数
                    result.Add(record);
                    num++;
                }
                else
                {
                    var iShave = result.Count(c => c.Score == i["Score"].ToString());
                    if (iShave > 0)
                    {
                        record.Ranking = num - 1;//名次
                        record.Department = i["Department"].ToString();//部门
                        record.Score = i["Score"].ToString();//分数
                        result.Add(record);
                    }
                    else
                    {
                        record.Ranking = num;//名次
                        record.Department = i["Department"].ToString();//部门
                        record.Score = i["Score"].ToString();//分数
                        result.Add(record);
                        num++;
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #endregion

        #region 指标名称清单数据录入 
        //显示
        public ActionResult DisplayKPIIndicators(DateTime? time, string Department, string Group)
        {
            JArray result = new JArray();
            List<KPI_Indicators> value;
            if (time == null)
            {
                var timecheck = db.KPI_Indicators.Max(c => c.ExecutionTime);
                value = db.KPI_Indicators.Where(c => c.ExecutionTime == timecheck).ToList();
            }
            else
            {
                var timecheck = db.KPI_Indicators.OrderByDescending(c => c.ExecutionTime).Where(c => c.ExecutionTime <= time).Select(c => c.ExecutionTime).FirstOrDefault();
                value = db.KPI_Indicators.Where(c => c.ExecutionTime == timecheck).ToList();
            }
            if (!string.IsNullOrEmpty(Department))
            {
                value = value.Where(c => c.Department == Department).ToList();
            }
            if (!string.IsNullOrEmpty(Group))
            {
                value = value.Where(c => c.Group == Group).ToList();
            }
            foreach (var item in value)
            {
                JObject obj = new JObject();
                obj.Add("ID", item.Id);//部门
                obj.Add("Department", item.Department);//部门
                obj.Add("Group", item.Group);//班组
                obj.Add("IndicatorsName", item.IndicatorsName);//指标名
                obj.Add("IndicatorsDefine", item.IndicatorsDefine);//指标定义
                obj.Add("ComputationalFormula", item.ComputationalFormula);//指标计算工时
                obj.Add("IndicatorsValue", item.IndicatorsValue);//目标值
                obj.Add("IndicatorsValueUnit", item.IndicatorsValueUnit);//目标值
                obj.Add("DataName", item.DataName);//数据名称
                obj.Add("Cycle", item.Cycle);//数据周期
                obj.Add("SourceDepartment", item.SourceDepartment);//数据工号
                obj.Add("DataInputor", item.DataInputor);//数据姓名
                obj.Add("DataInputTime", item.DataInputTime == null ? null : item.DataInputTime.ToString());//录入时间
                obj.Add("IndicatorsType", item.IndicatorsType);//考核类型  品质或效率
                obj.Add("IndicatorsStatue", item.IndicatorsStatue);//考核异常或者正常
                obj.Add("ExecutionTime", item.ExecutionTime.ToString());//品质考核类型, 直通 或者合格率

                result.Add(obj);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //单个新增
        public string SingleAddKPIIndicators(KPI_Indicators indicators)
        {
            var info = db.KPI_Indicators.Count(c => c.Department == indicators.Department && c.Group == indicators.Group && c.IndicatorsType == indicators.IndicatorsType && c.ExecutionTime == indicators.ExecutionTime);
            if (info != 0)
            {
                return "已有重复数据";
            }

            db.KPI_Indicators.Add(indicators);
            db.SaveChanges();
            return "true";
        }

        //增加一个新版本
        public string BacthAddKPIIndicators(List<KPI_Indicators> indicators)
        {
            foreach (var item in indicators)
            {
                var num = db.KPI_Indicators.Count(c => c.ExecutionTime == item.ExecutionTime && c.Group == item.Group && c.Department
                == item.Department && c.IndicatorsType == item.IndicatorsType);
                if (num != 0)
                {
                    return "部门班组" + item.Department + item.Group + "已有重复的版本信息";
                }
            }

            db.KPI_Indicators.AddRange(indicators);
            db.SaveChanges();
            return "新增成功";
        }

        //修改
        public string UpdateKPIIndicators(KPI_Indicators indicators)
        {
            var info = db.KPI_Indicators.Find(indicators.Id);
            if (indicators.Department != info.Department || indicators.Group != info.Group || indicators.IndicatorsType != info.IndicatorsType)
            {
                var check = db.KPI_Indicators.Count(c => c.Department == indicators.Department && c.Group == indicators.Group && c.IndicatorsName == indicators.IndicatorsName && c.ExecutionTime == indicators.ExecutionTime && c.IndicatorsType == indicators.IndicatorsType);
                if (check != 0)
                {
                    return "已有重复数据";
                }
            }
            if (info.ExecutionTime != indicators.ExecutionTime)
            {
                db.KPI_Indicators.Add(indicators);
                db.SaveChanges();
                return "修改成功";
            }
            else
            {
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "数据来源修改" + info.Department + "->" + indicators.Department + "," + info.Group + "->" + indicators.Group + "," + info.IndicatorsDefine + "->" + indicators.IndicatorsDefine + "," + info.IndicatorsName + "->" + indicators.IndicatorsName + "," + info.IndicatorsStatue + "->" + indicators.IndicatorsStatue + "," + info.IndicatorsTimeSpan + "->" + indicators.IndicatorsTimeSpan + "," + info.ComputationalFormula + "->" + indicators.ComputationalFormula + "," + info.Weight + "->" + indicators.Weight + "," + info.IndicatorsValue + "->" + indicators.IndicatorsValue + "," + info.IndicatorsValueUnit + "->" + indicators.IndicatorsValueUnit + "," + info.DataName + "->" + indicators.DataName + "," + info.Cycle + "->" + indicators.Cycle + "," + info.SourceDepartment + "->" + indicators.SourceDepartment + "," + info.DataInputor + "->" + indicators.DataInputor + "," + info.DataInputTime + "->" + indicators.DataInputTime + "," + info.IndicatorsType + "->" + indicators.IndicatorsType + "," + info.QualityStatue + "->" + indicators.QualityStatue };
                info.Department = indicators.Department;
                info.Group = indicators.Group;
                info.IndicatorsDefine = indicators.IndicatorsDefine;
                info.IndicatorsName = indicators.IndicatorsName;
                info.IndicatorsStatue = indicators.IndicatorsStatue;
                info.IndicatorsTimeSpan = indicators.IndicatorsTimeSpan;
                info.ComputationalFormula = indicators.ComputationalFormula;
                info.Weight = indicators.Weight;
                info.IndicatorsValue = indicators.IndicatorsValue;
                info.IndicatorsValueUnit = indicators.IndicatorsValueUnit;
                info.DataName = indicators.DataName;
                info.Cycle = indicators.Cycle;
                info.SourceDepartment = indicators.SourceDepartment;
                info.DataInputor = indicators.DataInputor;
                info.DataInputTime = indicators.DataInputTime;
                info.Section = indicators.Section;
                info.Process = indicators.Process;
                info.IndicatorsType = indicators.IndicatorsType;
                info.QualityStatue = indicators.QualityStatue;
                db.UserOperateLog.Add(log);
                db.SaveChanges();


                return "修改成功";
            }
        }

        //删除 
        public void DeleteKPIIndicators(int id)
        {
            var deleteinfo = db.KPI_Indicators.Find(id);
            UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "删除数据来源表" + deleteinfo.Department + deleteinfo.Group + deleteinfo.IndicatorsType };
            db.KPI_Indicators.Remove(deleteinfo);
            db.UserOperateLog.Add(log);
            db.SaveChanges();
        }

        //导出到EXCEL表
        public FileContentResult OutputExcel(string value)
        {

            JArray result = JsonConvert.DeserializeObject<JArray>(value);
            var time = result[0]["ExecutionTime"].ToString();
            //var recordList = db.KPI_Indicators.Where(c => c.ExecutionTime == time).ToList();

            //导出表格名称
            string tableName = "指标名称清单表";

            tableName = tableName + "(按" + time + "时间查找)";
            JArray totale = new JArray();
            JObject item = new JObject();
            item.Add("DataTable", result);

            string[] columns = { "部门", "班组", "考核指标名", "指标定义", "计算公式", "目标值", "目标值单位", "数据名称", "数据提供周期(天/周/月)", "数据录入工号", "数据录入姓名", "录入时间", "考核类型", "考核正常/异常", "执行时间" };
            item.Add("Columns", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(columns)));

            totale.Add(item);
            byte[] filecontent = ExcelExportHelper.ExportExcel2(totale, tableName, false, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");


        }

        #endregion

        #region 品质效率实际记录
        //显示
        public ActionResult DisplayActualRecord(string ordernum, string deparment, string group, string process, string section, DateTime? time)
        {
            JArray result = new JArray();
            var totalvalue = db.KPI_ActualRecord.ToList();
            var kpiinde = db.KPI_Indicators.ToList();
            if (!string.IsNullOrEmpty(ordernum))
            {
                totalvalue = totalvalue.Where(c => c.OrderNum == ordernum).ToList();
            }
            if (!string.IsNullOrEmpty(deparment))
            {
                totalvalue = totalvalue.Where(c => c.Department == deparment).ToList();
                kpiinde = kpiinde.Where(c => c.Department == deparment).ToList();
            }
            if (!string.IsNullOrEmpty(group))
            {
                totalvalue = totalvalue.Where(c => c.Group == group).ToList();
                kpiinde = kpiinde.Where(c => c.Group == group).ToList();
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
                var timecheck = db.KPI_Indicators.OrderByDescending(c => c.ExecutionTime).Where(c => c.ExecutionTime <= time).Select(c => c.ExecutionTime).FirstOrDefault();
                totalvalue = totalvalue.Where(c => c.ActualTime >= time && c.ActualTime < time.Value.AddMonths(1)).ToList();
                kpiinde = kpiinde.Where(c => c.ExecutionTime == time).ToList();
            }
            foreach (var item in totalvalue)
            {
                JObject obj = new JObject();
                obj.Add("ID", item.Id); //订单
                obj.Add("OrderNum", item.OrderNum); //订单
                obj.Add("Department", item.Department);//部门
                obj.Add("Group", item.Group);//班组
                obj.Add("Process", item.Process);//工段
                obj.Add("Section", item.Section);//工序
                obj.Add("IndicatorsType", item.IndicatorsType);//品质或者效率
                var info = kpiinde.Select(c => c.IndicatorsValueUnit).FirstOrDefault();
                obj.Add("ActualNormalNum", item.ActualNormalNum.ToString());//正常数量
                obj.Add("ActualAbnormalNum", item.ActualAbnormalNum);//异常数量
                obj.Add("ActualTime", item.ActualTime.ToString());//异常数量
                obj.Add("Message", item.ActualAbnormalDescription);//异常信息
                obj.Add("ActualCreateor", item.ActualCreateor);//录入人员
                result.Add(obj);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //新增
        public string AddActualRecord(List<KPI_ActualRecord> actualRecord)
        {
            if (Session["User"] == null)
            {
                return "Login";
            }
            string error = "";
            foreach (var item in actualRecord)
            {
                //拿到正确版本时间
                var version = db.KPI_Indicators.Where(c => c.ExecutionTime <= DateTime.Now).Select(c => c.ExecutionTime).Distinct().ToList();
                var exetime = version.Max();
                #region  判断时间是否正确
                //var timetip = db.KPI_Indicators.Where(c => c.Department == item.Department && c.Group == item.Group && c.IndicatorsType == item.IndicatorsType&&c.ExecutionTime== exetime).Select(c => c.DataInputTime).FirstOrDefault();
                //string[] spitinfo = timetip.Split('/');
                //if (spitinfo[0] == "天")
                //{
                //    int day = int.Parse(spitinfo[1]) - 1;
                //    var beforday = DateTime.Now.AddDays(-day).Date;
                //    if (item.ActualTime != beforday)
                //    {
                //        return "录入时间与定义的时间不符合,请确认时间准确";
                //    }
                //    var time = spitinfo[2];
                //    var hourse = int.Parse(time.Split(':')[0]);
                //    var minute = int.Parse(time.Split(':')[1]);
                //    if (DateTime.Now.Hour > hourse)
                //    {
                //        return "录入时间与定义的时间不符合,请确认时间准确";
                //    }
                //    else if (DateTime.Now.Hour == hourse && DateTime.Now.Minute > minute)
                //    {
                //        return "录入时间与定义的时间不符合,请确认时间准确";
                //    }
                //}
                //else if (spitinfo[0] == "月")
                //{
                //    int mounte = int.Parse(spitinfo[1]) - 1;
                //    var beforday = DateTime.Now.AddMonths(-mounte).Date;
                //    if (item.ActualTime.Value.Month != beforday.Month || item.ActualTime.Value.Year != beforday.Year)
                //    {
                //        return "录入时间与定义的时间不符合,请确认时间准确";
                //    }
                //    var day = int.Parse(spitinfo[2]);
                //    var beforday2 = beforday.AddDays(-day);
                //    if (item.ActualTime < beforday2)
                //    {
                //        return "录入时间与定义的时间不符合,请确认时间准确";
                //    }
                //}
                #endregion
                #region 判断录入人员是否正确
                if (((Users)Session["User"]).Role != "系统管理员")
                {
                    var sourceName = db.KPI_Indicators.Where(c => c.Department == item.Department && c.Group == item.Group && c.IndicatorsType == item.IndicatorsType && c.ExecutionTime == exetime).Select(c => c.DataInputor).FirstOrDefault();
                    var sourceUserID = db.KPI_Indicators.Where(c => c.Department == item.Department && c.Group == item.Group && c.IndicatorsType == item.IndicatorsType && c.ExecutionTime == exetime).Select(c => c.SourceDepartment).FirstOrDefault();
                    if (sourceName == null)
                    {
                        return "找不到" + item.Department + item.Group + "录入人员信息,请确定部门班组的准确,注意大小写和中英文";
                    }
                    if (sourceUserID == null)
                    {
                        return "找不到" + item.Department + item.Group + "录入人员信息,请确定部门班组的准确,注意大小写和中英文";
                    }
                    var namelist = sourceName.Split('/');
                    var idList = sourceUserID.Split('/');
                    int[] idintList = Array.ConvertAll<string, int>(idList, delegate (string s) { return int.Parse(s); });
                    var username = ((Users)Session["User"]).UserName;
                    var userid = ((Users)Session["User"]).UserNum;

                    if (!namelist.Contains(username) || !idintList.Contains(userid))
                    {
                        return "登录账号不在录入人员列表当中,请确认录入信息";
                    }
                }
                #endregion
                var info = db.KPI_ActualRecord.Count(c => c.ActualTime == item.ActualTime && c.OrderNum == item.OrderNum && c.Department == item.Department && c.Group == item.Group && c.Process == item.Process && c.Section == item.Section && c.IndicatorsType == item.IndicatorsType);
                if (info != 0)
                {
                    error = error + item.OrderNum + "在" + item.ActualTime + "已有" + item.Department + item.Group + "的生产记录.";
                }

            }
            if (string.IsNullOrEmpty(error))
            {
                actualRecord.ForEach(c => { c.ActualCreateor = ((Users)Session["User"]).UserName; c.ActualCreateTime = DateTime.Now; });
                db.KPI_ActualRecord.AddRange(actualRecord);
                db.SaveChanges();
                return "新增成功";
            }
            else
            {
                return error;
            }

        }
        //修改,时间不能修改
        public ActionResult UpdateActualRecord(int id, KPI_ActualRecord actualRecord)
        {
            var info = db.KPI_ActualRecord.Find(id);
            JObject result = new JObject();
            if (info == null)
            {
                result.Add("mes", "没有找到数据");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));
            }
            if (((Users)Session["User"]).UserName != info.ActualCreateor && ((Users)Session["User"]).Role != "系统管理员")
            {
                result.Add("mes", "你没有权限删除该条信息");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));

            }
            info.OrderNum = actualRecord.OrderNum;
            info.Department = actualRecord.Department;
            info.Group = actualRecord.Group;
            info.Process = actualRecord.Process;
            info.Section = actualRecord.Section;
            info.IndicatorsType = actualRecord.IndicatorsType;
            info.ActualNormalNum = actualRecord.ActualNormalNum;
            info.ActualAbnormalNum = actualRecord.ActualAbnormalNum;
            info.ActualTime = actualRecord.ActualTime;
            info.ActualAbnormalDescription = actualRecord.ActualAbnormalDescription;

            db.SaveChanges();
            result.Add("mes", "修改成功");
            result.Add("pass", true);
            return Content(JsonConvert.SerializeObject(result));

        }

        //删除

        public ActionResult DeleteActualRecord(List<int> id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_Inspection" });
            }
            JObject result = new JObject();
            var info = db.KPI_ActualRecord.Where(c => id.Contains(c.Id)).ToList();
            if (info.Count == 0)
            {
                result.Add("mes", "找不到该信息");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));
            }
            var cretor = info.Select(c => c.ActualCreateor).Distinct().ToList();
            if (cretor.Count > 1 || !cretor.Contains(((Users)Session["User"]).UserName) && ((Users)Session["User"]).Role != "系统管理员")
            {
                result.Add("mes", "你没有权限删除信息,信息的创建人员并不全是你");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));

            }
            db.KPI_ActualRecord.RemoveRange(info);
            db.SaveChanges();
            UserOperateLog log = new UserOperateLog()
            {
                OperateDT = DateTime.Now,
                Operator = ((Users)Session["User"]).UserName,
                OperateRecord = "删除生产/送检数据" + string.Join(" ", info.Select(c => c.Department).Distinct().ToList()) + string.Join(" ", info.Select(c => c.Group).Distinct().ToList())
            };
            db.UserOperateLog.Add(log);
            db.SaveChanges();
            result.Add("mes", "删除成功");
            result.Add("pass", true);
            return Content(JsonConvert.SerializeObject(result));
        }

        #endregion

        #region  ---KPI显示
        public class TempIndicators
        {
            public string Department { get; set; }
            public string Group { get; set; }
            public double IndicatorsValue { get; set; }
            public string IndicatorsName { get; set; }
            public string SourceDepartment { get; set; }
            public string IndicatorsType { get; set; }
            public string Cycle { get; set; }
            public string IndicatorsValueUnit { get; set; }
            public string Process { get; set; }
            public string Section { get; set; }
            public string IndicatorsStatue { get; set; }
            public decimal Weight { get; set; }
        }
        public class TempValue
        {

            public string Department { get; set; }
            public string Group { get; set; }
            public string Process { get; set; }
            public string Section { get; set; }
            public string CheckDepartment { get; set; }
            public string CheckGroup { get; set; }
            public string CheckProcess { get; set; }
            public string CheckSection { get; set; }
            public string Types { get; set; }
            public DateTime? Time { get; set; }
            public int Num { get; set; }
            public int CheckNum { get; set; }
            public int AbNum { get; set; }

        }
        public class TempRate
        {
            public int Id { get; set; }
            public string Department { get; set; }
            public string Group { get; set; }
            public double IndicatorsValue { get; set; }//目标值
            public int BeginNumber { get; set; }
            public int EndNumber { get; set; }
            public int LossNumber { get; set; }
            public DateTime DateLoss { get; set; }
        }
        public class RankingList
        {
            public string Department { get; set; }//部门
            public string Group { get; set; }//班组
            public string Ethnic_Group { get; set; }//族群
            public decimal AvagePersonNum { get; set; }//平均人数
            public decimal InductrialAccident { get; set; }//当月有无工伤事故
            public decimal Attendance { get; set; }//出勤率
            public string ViolationsMessage { get; set; }//行政违规情况
            public DateTime? Time { get; set; }//记录年月月份
        }
        public class RinkCalculate
        {
            public double TotalScore { get; set; }//总分
            public string Department { get; set; }//部门
            public string Group { get; set; }//班组
            public string Ethnic_Group { get; set; }//族群
            public decimal AvagePersonNum { get; set; }//平均人数
            public decimal InductrialAccident { get; set; }//当月有无工伤事故
            public decimal Attendance { get; set; }//出勤率
            public string ViolationsMessage { get; set; }//行政违规情况
            public int Ranking { get; set; }//排名
            public int Integral { get; set; }//积分
        }

        //总表显示
        public ActionResult DisplayTotal(DateTime time)
        {
            JArray result = new JArray();
            JArray table = new JArray();
            ////拿到对的版本
            //var version = db.KPI_Indicators.Where(c => c.ExecutionTime <= time).Select(c => c.ExecutionTime).Distinct().ToList();
            ////取值
            //var exetime = version.Max();

            //拿到对的版本，取最大值
            var exetime = db.KPI_Indicators.Where(c => c.ExecutionTime <= time).Max(c => c.ExecutionTime);

            //指标参数录入表（数据来源表）
            var total = db.KPI_Indicators.Where(c => c.ExecutionTime == exetime).Select(c => new TempIndicators { Department = c.Department, Group = c.Group, Cycle = c.Cycle, IndicatorsValueUnit = c.IndicatorsValueUnit, IndicatorsName = c.IndicatorsName, IndicatorsValue = c.IndicatorsValue, SourceDepartment = c.SourceDepartment, Process = c.Process, Section = c.Section, IndicatorsType = c.IndicatorsType, Weight = c.Weight, IndicatorsStatue = c.IndicatorsStatue });
            //品质效率计划表
            var PlanValue = db.Plan_FromKPI.Where(c => c.PlanTime.Value.Year == time.Year).Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.PlanTime, Num = c.PlanNum, Types = c.IndicatorsType, CheckDepartment = c.CheckDepartment, CheckGroup = c.CheckGroup, CheckProcess = c.CheckProcess, CheckSection = c.CheckSection, CheckNum = c.CheckNum });
            //品质效率实际记录表
            var ActualValue = db.KPI_ActualRecord.Where(c => c.ActualTime.Value.Year == time.Year).Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.ActualTime, Num = c.ActualNormalNum, Types = c.IndicatorsType, AbNum = c.ActualAbnormalNum });
            //班组流失率表
            //var TurnoverValue = db.KPI_TurnoverRate.Where(c => c.DateLoss.Year == time.Year && c.DateLoss.Month == time.Month).Select(c => new TempRate { Department = c.Department, Group = c.Group, BeginNumber = c.BeginNumber, EndNumber = c.EndNumber, LossNumber = c.LossNumber, DateLoss = c.DateLoss, IndicatorsValue = c.IndicatorsValue });
            var TurnoverValue = db.KPI_TurnoverRate.Where(c => c.DateLoss.Year == time.Year && c.DateLoss.Month == time.Month).Select(c => new { c.Id, c.Department, c.Group, c.BeginNumber, c.EndNumber, c.LossNumber, c.DateLoss, c.IndicatorsValue }).ToList();
            //7S
            var KPI_7SValue = db.KPI_7S_Record.Where(c => c.Date.Year == time.Year && c.Date.Month == time.Month).ToList();
            //var versionTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= time).Select(c => c.VersionsTime).Distinct().ToList().Max();
            var versionTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= time).Max(c => c.VersionsTime);
            var positionList = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == versionTime).ToList();

            //排名计算的条件
            var TotalDisplayValue = db.KPI_TotalDisplay.Where(c => c.Time.Value.Year == time.Year && c.Time.Value.Month == time.Month).Select(c => new RankingList { Department = c.Department, Group = c.Group, AvagePersonNum = c.AvagePersonNum, InductrialAccident = c.InductrialAccident, Attendance = c.Attendance, ViolationsMessage = c.ViolationsMessage, Time = c.Time, Ethnic_Group = c.Ethnic_Group });
            //  var KpiInfo = db.KPI_TotalDisplay.Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.ActualTime, Num = c.ActualNormalNum });
            List<RinkCalculate> calculates = new List<RinkCalculate>();
            List<RinkCalculate> calculates2 = new List<RinkCalculate>();
            JArray DayArray = new JArray();
            var deparment = total.Select(c => c.Department).Distinct();
            foreach (var dep in deparment)
            {
                var group = total.Where(c => c.Department == dep).Select(c => c.Group).Distinct();
                foreach (var groupitem in group)
                {
                    JObject obj = new JObject();
                    obj.Add("Department", dep);
                    obj.Add("Group", groupitem);
                    double QualityGoalScore = 0;
                    double EfficiencyGoalScore = 0;
                    #region 效率指标
                    var efficiency = total.Where(c => c.Department == dep && c.Group == groupitem && c.IndicatorsType == "效率指标").FirstOrDefault();
                    var totalday = DateTime.DaysInMonth(time.Year, time.Month);
                    if (efficiency == null)
                    {
                        obj.Add("Efficiency_IndexName", null);
                        obj.Add("Efficiency_Target", null);
                        obj.Add("Efficiency_Actual", null);
                        obj.Add("Efficiency_Single", null);//单项得分
                        obj.Add("Efficiency_Score", null);//得分小计
                    }
                    else
                    {
                        //Efficiency_IndexName考核指标名
                        obj.Add("Efficiency_IndexName", efficiency.IndicatorsName);
                        //Efficiency_Target目标值（值+单位）
                        obj.Add("Efficiency_Target", efficiency.IndicatorsValue + efficiency.IndicatorsValueUnit);
                        int PlanTotalValue = 0;
                        int ActualTotalValue = 0;
                        List<decimal> oneScore = new List<decimal>();
                        for (var i = 1; i <= totalday; i++)
                        {
                            //创建具体的日期
                            var Ftime = new DateTime(time.Year, time.Month, i);
                            //计划
                            var plancount = 0;
                            var actualcount = 0;
                            var Mkactualcount = 0;
                            var Mkplancount = 0;
                            //数据来源表目标值单位不是%以笔/单/次,这种一般都是没有计划,直接拿生产表中的异常数量
                            if(efficiency.Cycle=="月")
                            {
                                var sectionlist3 = new List<string>();
                                sectionlist3.Add("送检");//没有找计划拿到工段工序,自己给一个用于在生产/送检拿到数据
                                actualcount = commom.AcuteNum3(efficiency.Department, efficiency.Group, sectionlist3, efficiency.IndicatorsStatue, "效率指标", Ftime);
                                plancount = commom.AcuteNum3(efficiency.Department, efficiency.Group, sectionlist3, "", "效率指标", Ftime);
                                PlanTotalValue = PlanTotalValue + plancount;
                                ActualTotalValue = ActualTotalValue + actualcount;
                                break;
                            }
                            else if (efficiency.IndicatorsValueUnit != "%")
                            {
                                var actual = ActualValue.Where(c => c.Department == efficiency.Department && c.Group == efficiency.Group && c.Time.Value == Ftime && c.Types == "效率指标").Select(c => c.AbNum).ToList();
                                actualcount = actual.Count != 0 ? actual.Sum() : 0;
                            }
                            else
                            {
                                var sectionlist3 = new List<string>();
                                sectionlist3.Add("送检");//没有找计划拿到工段工序,自己给一个用于在生产/送检拿到数据
                                var planlist = PlanValue.Where(c => c.Department == efficiency.Department && c.Group == efficiency.Group && c.Time.Value == Ftime).ToList();

                                plancount = planlist.Count == 0 ? 0 : planlist.Sum(c => c.Num);
                                if (planlist.Count != 0)
                                {
                                    //var sectionlist = new List<string>();
                                    ////计划部门班组的工段工序列表
                                    //var distinSection = planlist.Select(c => new { c.Section, c.Process }).Distinct().ToList();
                                    //distinSection.ForEach(c => sectionlist.Add(c.Section + c.Process));

                                    //计划部门班组的工段工序列表
                                    var sectionlist = planlist.Select(c => c.Section + c.Process).Distinct().ToList();

                                    //计划不为空,并且单位是以%,计划数为分母,生产数为分子
                                    actualcount = commom.AcuteNum3(dep, groupitem, sectionlist, efficiency.IndicatorsStatue, "效率指标", Ftime);

                                }
                                else
                                {
                                    if (efficiency.Department == "品质部")
                                    {
                                        actualcount = commom.AcuteNum3(efficiency.Department, efficiency.Group, sectionlist3, "正常", "效率指标", Ftime, "模组");
                                        Mkactualcount = commom.AcuteNum3(efficiency.Department, efficiency.Group, sectionlist3, "正常", "效率指标", Ftime, "模块");
                                        plancount = commom.AcuteNum3(efficiency.Department, efficiency.Group, sectionlist3, "", "效率指标", Ftime, "模组");
                                        Mkplancount = commom.AcuteNum3(efficiency.Department, efficiency.Group, sectionlist3, "", "效率指标", Ftime, "模块");
                                        //只有模组算模组,只有模块算模块,有模组模块算平均
                                        var mz = plancount == 0 ? 0 : (actualcount * 100 / plancount);
                                        var mk = Mkplancount == 0 ? 0 : (Mkactualcount * 100 / Mkplancount);
                                        var value = mz == 0 ? mk : (mk == 0 ? mz : Math.Round((decimal)(mk + mz) / 2, 2));

                                        if (value != 0)
                                        { oneScore.Add(value); }
                                    }
                                    else
                                    {
                                        //拿到检验部门所负责的部门班组列表
                                        var departmentList = PlanValue.Where(c => c.CheckDepartment == efficiency.Department && c.CheckGroup == efficiency.Group && c.Time.Value == Ftime).Select(c => new { c.Department, c.Group }).Distinct().ToList();

                                        //计划为空,单位是以%,并且没有有所负责的部门班组列表 ,单纯没做计划,直接显示生产信息
                                        if (departmentList.Count == 0)
                                        {
                                            actualcount = commom.AcuteNum3(dep, groupitem, sectionlist3, efficiency.IndicatorsStatue, "效率指标", Ftime);
                                        }
                                        //计划为空,单位是以%,并且有所负责的部门班组列表 ,一般为检验部门,检验部门一般不做计划 效率指标计算是检验数/生产数  品质指标是不漏件数/检验数
                                        else
                                        {
                                            foreach (var onedep in departmentList)
                                            {
                                                //var sectionlist = new List<string>();
                                                //var sectionlist2 = PlanValue.Where(c => c.Department == onedep.Department && c.Group == onedep.Group && c.Time.Value == Ftime).Select(c => new { c.Section, c.Process }).Distinct().ToList();
                                                //sectionlist2.ForEach(c => sectionlist.Add(c.Section + c.Process));

                                                var sectionlist = PlanValue.Where(c => c.Department == onedep.Department && c.Group == onedep.Group && c.Time.Value == Ftime).Select(c => c.Section + c.Process).Distinct().ToList();


                                                //拿到所负责部门班组列表的生产正常记录,用于做分母
                                                plancount = plancount + commom.AcuteNum3(onedep.Department, onedep.Group, sectionlist, "正常", "效率指标", Ftime);
                                            }
                                            //拿到所负责部门班组列表的品质记录,正常+异常,用于做分子

                                            actualcount = actualcount + commom.AcuteNum3(efficiency.Department, efficiency.Group, sectionlist3, "", "品质指标", Ftime);

                                        }
                                    }
                                }
                            }
                            //实际
                            //var actualcount = ActualValue.Where(c => c.Process == efficiency.Process && c.Section == efficiency.Section && c.Department == dep && c.Group == groupitem && c.Time.Value == Ftime).Select(c => c.Num).FirstOrDefault();
                            //第二版本V2.0
                            PlanTotalValue = PlanTotalValue + plancount;
                            ActualTotalValue = ActualTotalValue + actualcount;

                        }
                        double difference = 0;
                        if (efficiency.IndicatorsValueUnit == "%")
                        {
                            double score = 0;
                            if (oneScore.Count != 0)
                            {
                                score = Math.Round((double)oneScore.Sum() / oneScore.Count(), 2);
                            }
                            else
                            {
                                score = PlanTotalValue == 0 ? 0 : Math.Round((double)ActualTotalValue * 100 / PlanTotalValue, 2);
                            }
                            obj.Add("Efficiency_Actual", score);//实际得分
                            if (efficiency.IndicatorsStatue == "正常")
                            {
                                difference = score - efficiency.IndicatorsValue;
                            }
                            else
                            {
                                difference = efficiency.IndicatorsValue - score;
                            }

                        }
                        else
                        {
                            obj.Add("Efficiency_Actual", ActualTotalValue);//实际得分
                            if (efficiency.IndicatorsStatue == "正常")
                            {
                                difference = ActualTotalValue - efficiency.IndicatorsValue;
                            }
                            else
                            {
                                difference = efficiency.IndicatorsValue - ActualTotalValue;
                            }
                        }

                        obj.Add("Efficiency_Single", difference >= 0 ? 100 : 0);//单项得分
                        EfficiencyGoalScore = difference >= 0 ? 35 : 0;
                        obj.Add("Efficiency_Score", EfficiencyGoalScore);//得分小计
                    }
                    #endregion

                    #region 品质指标
                    var quality = total.Where(c => c.Department == dep && c.Group == groupitem && c.IndicatorsType == "品质指标").FirstOrDefault();
                    if (quality == null)
                    {
                        obj.Add("Quality_IndexName", null);
                        obj.Add("Quality_Target", null);
                        obj.Add("Quality_Actual", null);//实际得分
                        obj.Add("Quality_Single", null);//单项得分
                        obj.Add("Quality_Score", null);//得分小计
                    }
                    else
                    {
                        obj.Add("Quality_IndexName", quality.IndicatorsName);
                        obj.Add("Quality_Target", quality.IndicatorsValue + quality.IndicatorsValueUnit);
                        int qualityPlanTotalValue = 0;
                        int qualityActualTotalValue = 0;
                        var qualitytotalday = DateTime.DaysInMonth(time.Year, time.Month);
                        for (var i = 1; i <= totalday; i++)
                        {
                            var Ftime = new DateTime(time.Year, time.Month, i);
                            //计划
                            var plancount = 0;
                            var actualcount = 0;
                            if (quality.Cycle == "月")
                            {
                                var sectionlist3 = new List<string>();
                                sectionlist3.Add("送检");//没有找计划拿到工段工序,自己给一个用于在生产/送检拿到数据
                                plancount = commom.AcuteNum3(quality.Department, quality.Group, sectionlist3, "", "品质指标", Ftime);
                                actualcount = commom.AcuteNum3(quality.Department, quality.Group, sectionlist3, quality.IndicatorsStatue, "品质指标", Ftime);
                                qualityPlanTotalValue = qualityPlanTotalValue + plancount;
                                qualityActualTotalValue = qualityActualTotalValue + actualcount;
                                break;
                            }
                            //数据来源表目标值单位不是%以笔/单/次,这种一般都是没有计划,直接拿生产表中的异常数量
                            if (quality.IndicatorsValueUnit != "%")
                            {
                                var actual = ActualValue.Where(c => c.Department == quality.Department && c.Group == quality.Group && c.Time.Value == Ftime && c.Types == "品质指标").Select(c => c.AbNum).ToList();
                                actualcount = actual.Count != 0 ? actual.Sum() : 0;
                            }
                            else
                            {
                                //var planlist = PlanValue.Where(c => c.Department == quality.Department && c.Group == quality.Group && c.Time.Value == Ftime).ToList();

                                //plancount = planlist.Count == 0 ? 0 : planlist.Sum(c => c.Num);

                                ////计划不为空,并且单位是以%,一般为生产部门班组,计划数为分母,生产数为分子
                                //if (planlist.Count != 0)
                                //{
                                //    var sectionlist = new List<string>();
                                //    var distinSection = planlist.Select(c => new { c.Section, c.Process }).Distinct().ToList();
                                //    distinSection.ForEach(c => sectionlist.Add(c.Section + c.Process));
                                //    actualcount = commom.AcuteNum3(dep, groupitem, sectionlist, quality.IndicatorsStatue, "品质指标", Ftime);
                                //}
                                //else
                                //{
                                //计划为空,并且单位是以% ,一般为检验部门,检验部门一般不做计划 效率指标计算是检验数/生产数  品质指标是不漏件数/检验数


                                var sectionlist = new List<string>();
                                sectionlist.Add("送检");//没有找计划拿到工段工序,自己给一个用于在生产/送检拿到数据
                                plancount = commom.AcuteNum3(quality.Department, quality.Group, sectionlist, "", "品质指标", Ftime);
                                actualcount = commom.AcuteNum3(quality.Department, quality.Group, sectionlist, quality.IndicatorsStatue, "品质指标", Ftime);
                                //}
                                //else
                                //{

                                //    //拿到检验部门所负责的部门班组列表
                                //    var departmentList = PlanValue.Where(c => c.CheckDepartment == quality.Department && c.CheckGroup == quality.Group && c.Time.Value == Ftime).Select(c => new { c.Department, c.Group }).Distinct().ToList();
                                //    // 品质指标是不漏件数 / 检验数
                                //    foreach (var onedep in departmentList)
                                //    {
                                //        var sectionlist = new List<string>();
                                //        var sectionlist2 = PlanValue.Where(c => c.Department == onedep.Department && c.Group == onedep.Group && c.Time.Value == Ftime).Select(c => new { c.Section, c.Process }).Distinct().ToList();
                                //        sectionlist2.ForEach(c => sectionlist.Add(c.Section + c.Process));
                                //        // 拿到所负责部门班组列表的品质记录正常数,用于做分母
                                //        plancount = plancount + commom.AcuteNum3(onedep.Department, onedep.Group, sectionlist, "", "品质指标", Ftime);
                                //    }
                                //    //找到检验部门的品质指标数据,拿到其中的正常数据/异常数据
                                //    var sectionlist3 = new List<string>();
                                //    sectionlist3.Add("送检");//没有找计划拿到工段工序,自己给一个用于在生产/送检拿到数据
                                //    actualcount = commom.AcuteNum3(quality.Department, quality.Group, sectionlist3, quality.IndicatorsStatue, "品质指标", Ftime);
                                //}
                                // }

                            }
                            qualityPlanTotalValue = qualityPlanTotalValue + plancount;
                            qualityActualTotalValue = qualityActualTotalValue + actualcount;

                        }

                        double difference2 = 0;
                        if (quality.IndicatorsValueUnit == "%")
                        {
                            obj.Add("Quality_Actual", qualityPlanTotalValue == 0 ? 0 : Math.Round((decimal)qualityActualTotalValue * 100 / qualityPlanTotalValue, 2));//实际得分
                            if (quality.IndicatorsStatue == "正常")
                            {
                                difference2 = (qualityPlanTotalValue == 0 ? 0 : (qualityActualTotalValue * 100 / qualityPlanTotalValue)) - quality.IndicatorsValue;
                            }
                            else
                            {
                                difference2 = qualityPlanTotalValue == 0 ? 0 : quality.IndicatorsValue - (qualityActualTotalValue * 100 / qualityPlanTotalValue);
                            }

                        }
                        else
                        {
                            obj.Add("Quality_Actual", qualityActualTotalValue);//实际得分
                            if (quality.IndicatorsStatue == "正常")
                            {
                                difference2 = qualityActualTotalValue - quality.IndicatorsValue;
                            }
                            else
                            {
                                difference2 = quality.IndicatorsValue - qualityActualTotalValue;
                            }
                        }
                        obj.Add("Quality_Single", difference2 >= 0 ? 100 : 0);//单项得分


                        //obj.Add("QualityActualScore", Math.Round((decimal)qualityActualTotalValue / qualityPlanTotalValue, 2));//实际得分
                        //obj.Add("QualityGoal", (qualityActualTotalValue / qualityPlanTotalValue) - quality.IndicatorsValue > 0 ? 100 : 0);//单项得分
                        QualityGoalScore = difference2 >= 0 ? 35 : 0;//得分小计
                        obj.Add("Quality_Score", QualityGoalScore);//得分小计
                    }
                    #endregion

                    #region 月度流失率指标TurnoverValue
                    double TurnoverGoalScore = 0;
                    var turnover = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).Select(c => c.IndicatorsValue).FirstOrDefault();
                    if (turnover == 0)
                    {
                        obj.Add("Turnover_IndexName", null);
                        obj.Add("Turnover_Target", null);
                        obj.Add("Turnover_Actual", null);//实际得分
                        obj.Add("Turnover_Single", null);//单项得分
                        obj.Add("Turnover_Score", null);//得分小计
                    }
                    else
                    {
                        obj.Add("Turnover_IndexName", "班组流失率");
                        obj.Add("Turnover_Target", turnover + "%");
                        double averageNum = 0;  //月平均人数
                        double actual = 0;
                        double IndividualScores = 0;
                        //月初人数
                        var beginNumber = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).OrderBy(c => c.Id).Select(c => c.BeginNumber).FirstOrDefault();
                        //月末人数最少
                        var endNumber = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).OrderByDescending(c => c.Id).Select(c => c.EndNumber).FirstOrDefault();
                        double num = beginNumber + endNumber;
                        averageNum = Math.Ceiling(num / 2);
                        //averageNum = (beginNumber + endNumber) / 2;
                        //var turnovertotalday = DateTime.DaysInMonth(time.Year, time.Month);

                        int turnoverTotalValue = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).Sum(c => c.LossNumber);
                        //for (var i = 1; i <= totalday; i++)
                        //{
                        //    //var Ftime = new DateTime(time.Year, time.Month, i);
                        //    var turnoverCount = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem && c.DateLoss.Year == time.Year && c.DateLoss.Month == time.Month && c.DateLoss.Day == i).Select(c => c.LossNumber).FirstOrDefault();
                        //    turnoverTotalValue = turnoverTotalValue + turnoverCount;
                        //}
                        if (averageNum == 0)
                        {
                            actual = 0;
                        }
                        else
                        {
                            actual = Math.Round((double)turnoverTotalValue / averageNum, 2) * 100;
                        }
                        if (actual <= turnover)
                        {
                            IndividualScores = 100;
                        }
                        else if (actual > turnover)
                        {
                            IndividualScores = 100 - (actual - turnover);
                        }
                        double weight = 0.15;
                        obj.Add("Turnover_Actual", actual + "%");//实际得分                       
                        obj.Add("Turnover_Single", IndividualScores);//单项得分
                        TurnoverGoalScore = IndividualScores * weight;//得分小计
                        obj.Add("Turnover_Score", TurnoverGoalScore.ToString("0.##"));//得分小计
                    }
                    #endregion

                    #region 7S评比
                    var KPI_7SValue_List = KPI_7SValue.Where(c => c.Department == dep && c.Group == groupitem).ToList();
                    var KPI_7SValue_List_Week = KPI_7SValue_List.Where(c => c.Check_Type == "周检").ToList();
                    var KPI_7SValue_List_Random = KPI_7SValue_List.Where(c => c.Check_Type == "巡检").ToList();

                    //var Grade_daily = KPI_7SValue_List.Where(c => c.Check_Type == "日检").Select(c => c.PointsDeducted).DefaultIfEmpty().Sum();//日检正常扣分
                    //var Grade_week = KPI_7SValue_List_Week.Select(c => c.PointsDeducted).DefaultIfEmpty().Sum();//周检正常扣分

                    //var Week_NotCorrected = KPI_7SValue_List_Week.Select(c => c.RectificationPoints).DefaultIfEmpty().Sum();//周检未整改
                    //var Random_NotCorrected = KPI_7SValue_List_Random.Select(c => c.RectificationPoints).DefaultIfEmpty().Sum();//巡检未整改

                    //var Week_repetition = KPI_7SValue_List_Week.Select(c => c.RepetitionPointsDeducted).DefaultIfEmpty().Sum();//周检重复出现
                    //var Random_repetition = KPI_7SValue_List_Random.Select(c => c.RepetitionPointsDeducted).DefaultIfEmpty().Sum();//巡检重复出现

                    //var Grade_random = KPI_7SValue_List_Random.Select(c => c.PointsDeducted).DefaultIfEmpty().Sum();//抽检正常扣分


                    var Grade_daily = KPI_7SValue_List.Where(c => c.Check_Type == "日检").Sum(c => c.PointsDeducted);//日检正常扣分
                    var Grade_week = KPI_7SValue_List_Week.Sum(c => c.PointsDeducted);//周检正常扣分
                    var Week_NotCorrected = KPI_7SValue_List_Week.Sum(c => c.RectificationPoints);//周检未整改
                    var Random_NotCorrected = KPI_7SValue_List_Random.Sum(c => c.RectificationPoints);//巡检未整改
                    var Week_repetition = KPI_7SValue_List_Week.Sum(c => c.RepetitionPointsDeducted);//周检重复出现
                    var Random_repetition = KPI_7SValue_List_Random.Sum(c => c.RepetitionPointsDeducted);//巡检重复出现
                    var Grade_random = KPI_7SValue_List_Random.Sum(c => c.PointsDeducted);//抽检正常扣分


                    decimal actualscore = 100 - Grade_daily - Grade_week - (Week_NotCorrected + Random_NotCorrected) - (Week_repetition + Random_repetition) - Grade_random;//实际得分

                    var comparison = positionList.Where(c => c.Department == dep && c.Group == groupitem).FirstOrDefault();
                    double ComparisonGoalScore = 0;
                    if (comparison == null)
                    {
                        obj.Add("Comparison_IndexName", null);
                        obj.Add("Comparison_Target", null);
                        obj.Add("Comparison_Actual", null);//实际得分
                        obj.Add("Comparison_Single", null);//单项得分
                        obj.Add("Comparison_Score", null);//得分小计
                    }
                    else
                    {
                        obj.Add("Comparison_IndexName", "7S评比指标");
                        obj.Add("Comparison_Target", comparison.TargetValue);
                        obj.Add("Comparison_Actual", (double)actualscore <= 0.00 ? "0" : actualscore.ToString("0.##"));//实际得分                       
                        double KPI7STurnoverGoal = 0;
                        if ((double)actualscore >= comparison.TargetValue)
                        {
                            KPI7STurnoverGoal = 100;
                        }
                        else if ((double)actualscore < comparison.TargetValue)
                        {
                            KPI7STurnoverGoal = 100 - (comparison.TargetValue - (double)actualscore);
                        }
                        obj.Add("Comparison_Single", KPI7STurnoverGoal <= 0.00 ? "0" : KPI7STurnoverGoal.ToString("0.##"));//单项得分
                        double Weight_7S = 0.15;
                        ComparisonGoalScore = KPI7STurnoverGoal * Weight_7S;
                        obj.Add("Comparison_Score", ComparisonGoalScore <= 0.00 ? "0" : ComparisonGoalScore.ToString("0.##"));//得分小计
                    }
                    #endregion
                    var conditions = TotalDisplayValue.Where(c => c.Department == dep && c.Group == groupitem).Select(c => new { c.InductrialAccident, c.ViolationsMessage, c.AvagePersonNum, c.Attendance, c.Ethnic_Group }).FirstOrDefault();
                    double TotalScore = EfficiencyGoalScore + QualityGoalScore + TurnoverGoalScore + ComparisonGoalScore;//合计总分
                    obj.Add("TotalScore", TotalScore);//合计总分
                    obj.Add("Greater", TotalScore >= 90 == true ? true : false); //合计总分是否大于90
                    decimal avagePersonNum = 0;
                    if (conditions == null)
                    {
                        //月初人数
                        var beginNumber = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).OrderBy(c => c.Id).Select(c => c.BeginNumber).FirstOrDefault();
                        //月末人数最少
                        var endNumber = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).OrderByDescending(c => c.Id).Select(c => c.EndNumber).FirstOrDefault();
                        decimal num = beginNumber + endNumber;
                        avagePersonNum = Math.Ceiling(num / 2);
                        obj.Add("AvagePersonNum", avagePersonNum);//平均人数
                        obj.Add("InductrialAccident", 0);//当月有无工伤事故
                        obj.Add("Ethnic_Group", 0);//族群
                        obj.Add("Attendance", 0);//出勤率
                        obj.Add("ViolationsMessage", "无");//行政违纪情况
                        obj.Add("Ranking", 0);//排名
                        obj.Add("integral", 0);//积分
                    }
                    else
                    {
                        if (conditions.AvagePersonNum == 0)
                        {
                            //月初人数
                            var beginNumber = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).OrderBy(c => c.Id).Select(c => c.BeginNumber).FirstOrDefault();
                            //月末人数最少
                            var endNumber = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).OrderByDescending(c => c.Id).Select(c => c.EndNumber).FirstOrDefault();
                            decimal num = beginNumber + endNumber;
                            avagePersonNum = Math.Ceiling(num / 2);
                            obj.Add("AvagePersonNum", avagePersonNum);//平均人数
                        }
                        else
                        {
                            avagePersonNum = conditions.AvagePersonNum;
                            obj.Add("AvagePersonNum", conditions.AvagePersonNum);//平均人数
                        }
                        obj.Add("InductrialAccident", conditions.InductrialAccident == 0 ? 0 : conditions.InductrialAccident);//当月有无工伤事故
                        obj.Add("Ethnic_Group", conditions.Ethnic_Group == null ? null : conditions.Ethnic_Group);//族群
                        var attendance = (conditions.Attendance * 100);
                        obj.Add("Attendance", attendance);
                        obj.Add("ViolationsMessage", conditions.ViolationsMessage == null ? null : conditions.ViolationsMessage);//行政违纪情况
                        obj.Add("Ranking", 0);//排名
                        obj.Add("integral", 0);//积分
                        RinkCalculate rink = new RinkCalculate();
                        rink.Department = dep;
                        rink.Group = groupitem;
                        rink.TotalScore = TotalScore;
                        rink.AvagePersonNum = avagePersonNum;
                        rink.Ethnic_Group = conditions.Ethnic_Group;
                        rink.Attendance = conditions.Attendance;
                        rink.InductrialAccident = conditions.InductrialAccident;
                        rink.ViolationsMessage = conditions.ViolationsMessage;
                        calculates.Add(rink);
                        calculates2.Add(rink);

                    }
                    DayArray.Add(obj);
                    obj = new JObject();
                }
            }
            var rank = new JArray(DayArray.OrderByDescending(c => c["TotalScore"]));//按总分排序
            calculates = calculates.Where(c => c.TotalScore >= 90 && c.AvagePersonNum >= 5 && c.InductrialAccident == 0 && c.Ethnic_Group == "操作族").OrderByDescending(c => c.TotalScore).ThenByDescending(c => c.Attendance).ToList();
            //排名计算
            int ranking = 1;
            #region---三列相同判断的排名方法
            for (int k = 0; k < calculates.Count(); k++)
            {
                if (ranking > 5) break;
                var it = calculates[k];
                var count_three = calculates.Where(c => c.TotalScore == it.TotalScore && c.Attendance == it.Attendance);
                if (count_three.Count() > 1)
                {
                    foreach (var ii in count_three)
                    {
                        calculates[k].Ranking = ii.Ranking == 0 ? ranking : ii.Ranking;
                        calculates[k].Integral = Integral_Manage(ranking);
                        k++;
                    }
                    k--;
                }
                else
                {
                    calculates[k].Ranking = ranking;
                    calculates[k].Integral = Integral_Manage(ranking);
                }
                ranking++;
            }
            //把排名放回汇总表
            foreach (var addval in calculates)
            {
                rank.Where(c => c["Department"].ToString() == addval.Department && c["Group"].ToString() == addval.Group).FirstOrDefault()["Ranking"] = addval.Ranking;
                rank.Where(c => c["Department"].ToString() == addval.Department && c["Group"].ToString() == addval.Group).FirstOrDefault()["integral"] = addval.Integral;
            }

            #endregion

            #region---三列相同判断的倒数排名方法
            calculates2 = calculates2.Where(c => c.AvagePersonNum >= 5 && c.Ethnic_Group == "操作族").OrderBy(c => c.TotalScore).ThenByDescending(c => c.ViolationsMessage).ThenBy(c => c.Attendance).ToList();
            var re_pm = calculates2.Select(c => c.TotalScore).Distinct().Take(3).LastOrDefault();//取总得分最低分的三个
            calculates2 = calculates2.Where(c => c.TotalScore <= re_pm).ToList();
            int ranking1 = 1;
            for (int g = 0; g < calculates2.Count(); g++)
            {
                if (ranking1 > 3) break;
                var tg = calculates2[g];
                var count_three = calculates2.Where(c => c.TotalScore == tg.TotalScore && c.ViolationsMessage == tg.ViolationsMessage && c.Attendance == tg.Attendance);
                if (count_three.Count() > 1)
                {
                    foreach (var ip in count_three)
                    {
                        calculates2[g].Ranking = ip.Ranking == 0 ? -ranking1 : ip.Ranking;
                        calculates2[g].Integral = Integral_Manage(-ranking1);
                        g++;
                    }
                    g--;
                }
                else
                {
                    calculates2[g].Ranking = -ranking1;
                    calculates2[g].Integral = Integral_Manage(-ranking1);
                }
                ranking1++;
            }
            //把倒数排名放回汇总表
            foreach (var addval in calculates2)
            {
                rank.Where(c => c["Department"].ToString() == addval.Department && c["Group"].ToString() == addval.Group).FirstOrDefault()["Ranking"] = addval.Ranking;
                rank.Where(c => c["Department"].ToString() == addval.Department && c["Group"].ToString() == addval.Group).FirstOrDefault()["integral"] = addval.Integral;
            }
            #endregion

            JObject code = new JObject();
            JObject retul = new JObject();
            JObject confirm = new JObject();//确认
            JObject audit = new JObject();//审核
            JObject approved = new JObject();//核准
            var codelist = db.KPI_ReviewSummary.Where(c => c.Year == time.Year && c.Month == time.Month).ToList();
            if (codelist.Count == 0)
            {
                code.Add("Code", null);
            }
            else
            {
                foreach (var it in codelist)
                {
                    if (it.Type == "确认")
                    {
                        confirm.Add("HRAuditDate", it.HRAuditDate == null ? null : it.HRAuditDate.ToString());
                        confirm.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                        confirm.Add("HRjudge", it.HRjudge == false ? false : it.HRjudge);
                    }
                    if (it.Type == "审核")
                    {
                        audit.Add("HRAuditDate", it.HRAuditDate == null ? null : it.HRAuditDate.ToString());
                        audit.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                        audit.Add("HRjudge", it.HRjudge == false ? false : it.HRjudge);
                    }
                    if (it.Type == "核准")
                    {
                        approved.Add("HRAuditDate", it.HRAuditDate == null ? null : it.HRAuditDate.ToString());
                        approved.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                        approved.Add("HRjudge", it.HRjudge == false ? false : it.HRjudge);
                    }
                }
                code.Add("Confirm", confirm);//确认
                confirm = new JObject();
                code.Add("Audit", audit);//审核
                audit = new JObject();
                code.Add("Approved", approved);//核准
                approved = new JObject();
            }
            retul.Add("Code", code);
            retul.Add("Rank", rank);
            return Content(JsonConvert.SerializeObject(retul));
        }

        //积分管理
        public int Integral_Manage(int t)
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
            return s;
        }

        //效率效率指标显示
        public ActionResult DiasplyEfficiency(DateTime time, string stuta, string deparment, List<string> group)
        {
            JObject result = new JObject();
            //拿到对的版本
            var version = db.KPI_Indicators.Where(c => c.ExecutionTime.Value.Year <= time.Year && c.ExecutionTime.Value.Month <= time.Month).Select(c => c.ExecutionTime).Distinct().ToList();
            if (version != null)
            {
                //取值
                var exetime = version.Max();
                var total = db.KPI_Indicators.Where(c => c.ExecutionTime == exetime && c.IndicatorsType == stuta).Select(c => new TempIndicators { Department = c.Department, Group = c.Group, Cycle = c.Cycle, IndicatorsValueUnit = c.IndicatorsValueUnit, IndicatorsName = c.IndicatorsName, IndicatorsValue = c.IndicatorsValue, SourceDepartment = c.DataInputor, Process = c.Process, Section = c.Section, IndicatorsStatue = c.IndicatorsStatue }).ToList();

                var PlanValue = db.Plan_FromKPI.Where(c => c.PlanTime.Value.Year == time.Year).Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.PlanTime, Num = c.PlanNum, Types = c.IndicatorsType, CheckDepartment = c.CheckDepartment, CheckGroup = c.CheckGroup, CheckProcess = c.CheckProcess, CheckSection = c.CheckSection }).ToList();

                var ActualValue = db.KPI_ActualRecord.Where(c => c.IndicatorsType == stuta && c.ActualTime.Value.Year == time.Year).Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.ActualTime, Num = c.ActualNormalNum, AbNum = c.ActualAbnormalNum, Types = c.IndicatorsType }).ToList();
                if (!string.IsNullOrEmpty(deparment))
                {
                    total = total.Where(c => c.Department == deparment).ToList();
                }
                if (group != null && group.Count != 0)
                {
                    total = total.Where(c => group.Contains(c.Group)).ToList();
                }


                //拿到日数据
                JArray DayArray = new JArray();
                var depar = total.Where(c => c.Cycle == "天").Select(c => c.Department).Distinct();
                foreach (var dep in depar)
                {
                    var d_group = total.Where(c => c.Cycle == "天" && c.Department == dep).Select(c => c.Group).Distinct();
                    foreach (var groupitem in d_group)
                    {
                        var dayvalue = total.Where(c => c.Cycle == "天" && c.Department == dep && c.Group == groupitem).FirstOrDefault();
                        JObject obj = new JObject();
                        obj.Add("Department", dayvalue.Department);
                        obj.Add("Group", dayvalue.Group);
                        obj.Add("IndicatorsName", dayvalue.IndicatorsName);
                        obj.Add("IndicatorsValue", dayvalue.IndicatorsValue + dayvalue.IndicatorsValueUnit);
                        obj.Add("SourceDepartment", dayvalue.SourceDepartment);
                        JArray actualvalue = new JArray();//实际数据
                        int PlanTotalValue = 0;
                        int ActualTotalValue = 0;
                        List<decimal> oneScore = new List<decimal>();
                        var totalday = DateTime.DaysInMonth(time.Year, time.Month);
                        for (var i = 1; i <= totalday; i++)
                        {
                            //每天的日期
                            var Ftime = new DateTime(time.Year, time.Month, i);

                            var plancount = 0;
                            var Mkplancount = 0;
                            var actualcount = 0;
                            var Mkactualcount = 0;
                            var sectionlist3 = new List<string>();
                            sectionlist3.Add("送检");//没有找计划拿到工段工序,自己给一个用于在生产/送检拿到数据

                            /*
                             * 计算方式:
                             * 1.目标值单位是以笔/次/单.效率计算=生产的异常数,品质计算=品质的异常数
                             * 2.目标值单位是%,计划不为空, 效率计算=生产数/计划数,品质计算=品质正常数/送检总数
                             * 3.目标值单位是%,计划为空,部门为品质部,效率计算=(模块生产正常数/模块生产总数+模组生产正常数/模组生产总数)/2,品质计算=品质正常数/送检总数
                             * 4.目标值单位是%,计划为空.部门除品质部外,在计划中能找到所负责被检验部门班组的,效率计算=检验数/生产数,品质计算=品质正常数/送检总数
                             * 5.目标值单位是%,计划为空.部门除品质部外,在计划中能找不到所负责被检验部门班组的,判定为单纯没做计划,计算方式,效率计算=生产数/计划数,品质计算=品质正常数/送检总数
                             */
                            //数据来源表目标值单位不是%以笔/单/次,这种一般都是没有计划,直接拿生产表中的异常数量
                            if (dayvalue.IndicatorsValueUnit != "%")
                            {
                                //生产表中的异常数量
                                var actual = ActualValue.Where(c => c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime && c.Types == stuta).Select(c => c.AbNum).ToList();
                                actualcount = actual.Count != 0 ? actual.Sum() : 0;
                                //没有计划
                                PlanTotalValue = PlanTotalValue + plancount;
                                ActualTotalValue = ActualTotalValue + actualcount;
                                actualvalue.Add(actualcount.ToString());
                            }
                            else
                            {
                                if (stuta == "效率指标")
                                {
                                    var planlist = PlanValue.Where(c => c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime).ToList();
                                    plancount = planlist.Count == 0 ? 0 : planlist.Sum(c => c.Num);
                                    //效率指标,目标值单位是%.但没有计划
                                    if (planlist.Count != 0)
                                    {
                                        var sectionlist = new List<string>();
                                        var distinSection = planlist.Select(c => new { c.Section, c.Process }).Distinct().ToList();
                                        distinSection.ForEach(c => sectionlist.Add(c.Section + c.Process));
                                        //计划不为空,并且单位是以%,计划数为分母,生产数为分子
                                        actualcount = commom.AcuteNum3(dep, groupitem, sectionlist, dayvalue.IndicatorsStatue, stuta, Ftime);
                                    }
                                    else
                                    {
                                        if (dayvalue.Department == "品质部")
                                        {
                                            actualcount = commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, "正常", "效率指标", Ftime, "模组");
                                            Mkactualcount = commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, "正常", "效率指标", Ftime, "模块");
                                            plancount = commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, "", "效率指标", Ftime, "模组");
                                            Mkplancount = commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, "", "效率指标", Ftime, "模块");
                                        }
                                        else
                                        {
                                            //拿到检验部门所负责的部门班组列表
                                            var departmentList = PlanValue.Where(c => c.CheckDepartment == dayvalue.Department && c.CheckGroup == dayvalue.Group && c.Time.Value == Ftime).Select(c => new { c.Department, c.Group }).Distinct().ToList();

                                            //计划为空,单位是以%,并且没有有所负责的部门班组列表 ,单纯没做计划,直接显示生产信息
                                            if (departmentList.Count == 0)
                                            {
                                                actualcount = commom.AcuteNum3(dep, groupitem, sectionlist3, dayvalue.IndicatorsStatue, stuta, Ftime);
                                            }
                                            //计划为空,单位是以%,并且有所负责的部门班组列表 ,一般为检验部门,检验部门一般不做计划 效率指标计算是检验数/生产数  品质指标是不漏件数/检验数
                                            else
                                            {
                                                foreach (var onedep in departmentList)
                                                {
                                                    var sectionlist = new List<string>();
                                                    var sectionlist2 = PlanValue.Where(c => c.Department == onedep.Department && c.Group == onedep.Group && c.Time.Value == Ftime).Select(c => new { c.Section, c.Process }).Distinct().ToList();
                                                    sectionlist2.ForEach(c => sectionlist.Add(c.Section + c.Process));

                                                    //拿到所负责部门班组列表的生产正常记录,用于做分母
                                                    plancount = plancount + commom.AcuteNum3(onedep.Department, onedep.Group, sectionlist, "正常", stuta, Ftime);
                                                }
                                                //拿到所负责部门班组列表的品质记录,正常+异常,用于做分子

                                                actualcount = actualcount + commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, "", "品质指标", Ftime);

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    plancount = commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, "", "品质指标", Ftime);
                                    actualcount = commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, dayvalue.IndicatorsStatue, "品质指标", Ftime);
                                }

                                PlanTotalValue = PlanTotalValue + plancount + Mkplancount;
                                ActualTotalValue = ActualTotalValue + actualcount + Mkactualcount;
                                if (Mkactualcount != 0 || Mkplancount != 0)
                                {
                                    var mz = plancount == 0 ? 0 : (actualcount * 100 / plancount);
                                    var mk = Mkplancount == 0 ? 0 : (Mkactualcount * 100 / Mkplancount);
                                    var value = mz == 0 ? mk : (mk == 0 ? mz : Math.Round((decimal)(mk + mz) / 2, 2));

                                    if (value != 0)
                                    { oneScore.Add(value); }
                                    actualvalue.Add(value + "% 模组:" + actualcount + "/" + plancount + " 模块:" + Mkactualcount + "/" + Mkplancount);
                                }
                                else
                                {
                                    actualvalue.Add(plancount == 0 ? "0% " + actualcount.ToString() + "/0" : Math.Round((decimal)actualcount * 100 / plancount, 2) + "% " + actualcount + "/" + plancount);
                                }
                            }
                        }
                        obj.Add("ActualValue", actualvalue);//每月实际数
                        obj.Add("PlanTotalValue", PlanTotalValue);//月总计划数
                        obj.Add("ActualTotalValue", ActualTotalValue);//月总实际数
                        double difference = 0;
                        if (dayvalue.IndicatorsValueUnit == "%")
                        {
                            double score = 0;
                            if (oneScore.Count != 0)
                            {
                                score = Math.Round((double)oneScore.Sum() / oneScore.Count(), 2);
                            }
                            else
                            {
                                score = PlanTotalValue == 0 ? 0 : Math.Round((double)ActualTotalValue * 100 / PlanTotalValue, 2);
                            }
                            obj.Add("ActualScore", score + "%");//实际得分
                            if (dayvalue.IndicatorsStatue == "正常")
                            {
                                difference = Math.Round(score - dayvalue.IndicatorsValue, 2);
                            }
                            else
                            {
                                difference = Math.Round(dayvalue.IndicatorsValue - score, 2);
                            }

                        }
                        else
                        {
                            if (dayvalue.IndicatorsStatue == "正常")
                            {
                                difference = Math.Round(ActualTotalValue - dayvalue.IndicatorsValue, 2);
                            }
                            else
                            {
                                difference = Math.Round(dayvalue.IndicatorsValue - ActualTotalValue, 2);
                            }
                            obj.Add("ActualScore", difference >= 0 ? "100%" : "0%");//实际得分
                        }
                        obj.Add("DifferencesValue", difference);//与目标值差异
                        obj.Add("Goal", difference >= 0 ? 100 : 0);//得分小计
                        DayArray.Add(obj);
                    }
                }
                result.Add("DayArray", DayArray);

                //拿到月数据
                JArray MouthArray = new JArray();
                var deparment2 = total.Where(c => c.Cycle == "月").Select(c => c.Department).Distinct().ToList();
                foreach (var dep in deparment2)
                {
                    var d_group = total.Where(c => c.Cycle == "月" && c.Department == dep).Select(c => c.Group).Distinct().ToList();
                    foreach (var groupitem in d_group)
                    {
                        var dayvalue = total.Where(c => c.Cycle == "月" && c.Department == dep && c.Group == groupitem).FirstOrDefault();
                        JObject obj = new JObject();
                        obj.Add("Department", dayvalue.Department);
                        obj.Add("Group", dayvalue.Group);
                        obj.Add("IndicatorsName", dayvalue.IndicatorsName);
                        obj.Add("IndicatorsValue", dayvalue.IndicatorsValue + dayvalue.IndicatorsValueUnit);
                        obj.Add("SourceDepartment", dayvalue.SourceDepartment);
                        JArray planvalue = new JArray(); //计划数据
                        JArray actualvalue = new JArray();//实际数据
                        int PlanTotalValue = 0;
                        int ActualTotalValue = 0;
                        for (var i = 1; i <= 12; i++)
                        {
                            var Ftime = new DateTime(time.Year, i, 1);
                            /*
                             * 计算方式
                             * 1.目标值单位是以笔/次/单.效率计算=生产的异常数,品质计算=品质的异常数
                             * 2.目标值单位是%,计划不为空, 效率计算=生产数/计划数,品质计算=品质正常数/送检总数
                             * 3.目标值单位是%,计划为空,判定为单纯没做计划,计算方式,效率计算=生产数/计划数,品质计算=品质正常数/送检总数
                             */
                            //计划
                            var plancount = 0;
                            var actualcount = 0;
                            if (dayvalue.IndicatorsValueUnit != "%")
                            {
                                //生产表中的异常数量
                                var actual = ActualValue.Where(c => c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime && c.Types == stuta).Select(c => c.AbNum).ToList();
                                actualcount = actual.Count != 0 ? actual.Sum() : 0;
                                //没有计划
                                PlanTotalValue = PlanTotalValue + plancount;
                                ActualTotalValue = ActualTotalValue + actualcount;
                                actualvalue.Add(actualcount.ToString());
                            }
                            else
                            {
                                var planlist = PlanValue.Where(c => c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime).ToList();
                                plancount = planlist.Count == 0 ? 0 : planlist.Sum(c => c.Num);

                                //版本2
                                if (planlist.Count != 0)
                                {
                                    var sectionlist = new List<string>();
                                    planlist.ForEach(c => sectionlist.Add(c.Section + c.Process));
                                    actualcount = commom.AcuteNum3(dep, groupitem, sectionlist, dayvalue.IndicatorsStatue, stuta, Ftime);
                                }
                                else
                                {
                                    var sectionlist3 = new List<string>();
                                    sectionlist3.Add("送检");
                                    if (dep == "品质部")
                                    {
                                        plancount = commom.AcuteNum3(dep, groupitem, sectionlist3,"", stuta, Ftime);
                                    }
                                    actualcount = commom.AcuteNum3(dep, groupitem, sectionlist3, dayvalue.IndicatorsStatue, stuta, Ftime);
                                }
                                PlanTotalValue = PlanTotalValue + plancount;
                                ActualTotalValue = ActualTotalValue + actualcount;
                                planvalue.Add(plancount);
                                actualvalue.Add(plancount == 0 ? actualcount.ToString() : Math.Round((decimal)actualcount * 100 / plancount, 2) + "% " + actualcount + "/" + plancount);
                            }
                        }
                        obj.Add("ActualValue", actualvalue);//每年实际数
                        obj.Add("PlanTotalValue", PlanTotalValue);//年总计划数
                        obj.Add("ActualTotalValue", ActualTotalValue);//年总实际数

                        double difference = 0;
                        if (dayvalue.IndicatorsValueUnit == "%")
                        {
                            obj.Add("ActualScore", PlanTotalValue == 0 ? "0%" : Math.Round((decimal)ActualTotalValue * 100 / PlanTotalValue, 2) + "%");//实际得分
                            if (dayvalue.IndicatorsStatue == "正常")
                            {
                                difference = (PlanTotalValue == 0 ? 0 : (ActualTotalValue * 100 / PlanTotalValue)) - dayvalue.IndicatorsValue;
                            }
                            else
                            {
                                difference = PlanTotalValue == 0 ? 0 : dayvalue.IndicatorsValue - (ActualTotalValue * 100 / PlanTotalValue);
                            }

                        }
                        else
                        {
                            obj.Add("ActualScore", ActualTotalValue);//实际得分
                            if (dayvalue.IndicatorsStatue == "正常")
                            {
                                difference = ActualTotalValue - dayvalue.IndicatorsValue;
                            }
                            else
                            {
                                difference = dayvalue.IndicatorsValue - ActualTotalValue;
                            }
                        }
                        obj.Add("DifferencesValue", difference);//与目标值差异
                        obj.Add("Goal", difference >= 0 ? 100 : 0);//得分小计
                                                                   //obj.Add("ActualScore", PlanTotalValue == 0 ? "0%" : Math.Round((decimal)ActualTotalValue * 100 / PlanTotalValue, 2) + "%");//实际得分
                                                                   //obj.Add("DifferencesValue", (PlanTotalValue == 0 ? 0 : (ActualTotalValue * 100 / PlanTotalValue)) - dayvalue.IndicatorsValue);//与目标值差异
                                                                   //obj.Add("Goal", (PlanTotalValue == 0 ? 0 : (ActualTotalValue * 100 / PlanTotalValue)) - dayvalue.IndicatorsValue > 0 ? 100 : 0);//得分小计

                        MouthArray.Add(obj);
                    }
                }
                result.Add("MouthArray", MouthArray);

                #region
                ////拿到单/笔/次数据
                //JArray OneOfTimeArray = new JArray();
                //var deparment3 = total.Where(c => c.Cycle == "天" && c.IndicatorsValueUnit != "%").Select(c => c.Department).Distinct();
                //foreach (var dep in deparment3)
                //{
                //    var group = total.Where(c => c.Cycle == "天" && c.IndicatorsValueUnit != "%" && c.Department == dep).Select(c => c.Group).Distinct();
                //    foreach (var groupitem in group)
                //    {
                //        var dayvalue = total.Where(c => c.Cycle == "天" && c.IndicatorsValueUnit != "%" && c.Department == dep && c.Group == groupitem).FirstOrDefault();
                //        JObject obj = new JObject();
                //        obj.Add("Department", dayvalue.Department);
                //        obj.Add("Group", dayvalue.Group);
                //        obj.Add("IndicatorsName", dayvalue.IndicatorsName);
                //        obj.Add("IndicatorsValue", dayvalue.IndicatorsValue + dayvalue.IndicatorsValueUnit);
                //        obj.Add("SourceDepartment", dayvalue.SourceDepartment);
                //        //JArray planvalue = new JArray(); //计划数据
                //        JArray actualvalue = new JArray();//实际数据
                //        //int PlanTotalValue = 0;
                //        int ActualTotalValue = 0;
                //        var totalday = DateTime.DaysInMonth(time.Year, time.Month);
                //        for (var i = 1; i <= totalday; i++)
                //        {
                //            var Ftime = new DateTime(time.Year, time.Month, i);
                //            var actualcount = 0;
                //            //实际
                //            if (dayvalue.IndicatorsStatue == "正常" || dayvalue.IndicatorsStatue == null)
                //            {
                //                actualcount = ActualValue.Where(c => c.Process == dayvalue.Process && c.Section == dayvalue.Section && c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime).Select(c => c.Num).FirstOrDefault();
                //            }
                //            else
                //            {
                //                actualcount = ActualValue.Where(c => c.Process == dayvalue.Process && c.Section == dayvalue.Section && c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime).Select(c => c.AbNum).FirstOrDefault();
                //            }

                //            ActualTotalValue = ActualTotalValue + actualcount;
                //            actualvalue.Add(actualcount);
                //        }
                //        obj.Add("ActualValue", actualvalue);//每月实际数
                //        obj.Add("ActualTotalValue", ActualTotalValue);//月总实际数
                //        obj.Add("ActualScore", Math.Round(ActualTotalValue / dayvalue.IndicatorsValue, 2));//实际得分
                //        obj.Add("DifferencesValue", (ActualTotalValue / dayvalue.IndicatorsValue) - dayvalue.IndicatorsValue);//与目标值差异
                //        obj.Add("Goal", (ActualTotalValue / dayvalue.IndicatorsValue) - dayvalue.IndicatorsValue > 0 ? 100 : 0);//得分小计

                //        DayArray.Add(obj);
                //    }
                //}
                //result.Add("OneOfTimeArray", OneOfTimeArray);
                #endregion
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //效率或品质指标显示EXCEL表格导出
        public FileContentResult EfficiencyQualityOutputExcel(string value, string year, string mounth, string state)
        {
            JObject total = JsonConvert.DeserializeObject<JObject>(value);
            var DayArray = (JArray)total["DayArray"];
            var MouthArray = (JArray)total["MouthArray"];
            JArray result = new JArray();

            //导出表格名称
            string tableName = "(" + year + "年" + mounth + "月" + state + "指标日报表)";

            //日表
            JObject resultitem = new JObject();
            resultitem.Add("DataTable", DayArray);

            List<string> DayArraycolumns = new List<string> { "部门名称", "班组名称", "考核指标名", "目标值", "数据来源" };
            var totalday = DateTime.DaysInMonth(int.Parse(year), int.Parse(mounth));
            for (var z = 1; z <= totalday; z++)
            {
                DayArraycolumns.Add(z + "日");
            }
            DayArraycolumns.Add("月计划产量");
            DayArraycolumns.Add("月实际产量");
            DayArraycolumns.Add("实际得分");
            DayArraycolumns.Add("与目标值差异");
            DayArraycolumns.Add("得分小计");
            resultitem.Add("Columns", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(DayArraycolumns)));
            result.Add(resultitem);
            resultitem = new JObject();
            //月表
            resultitem.Add("DataTable", MouthArray);
            string[] MouthArraycolumns = { "部门名称", "班组名称", "考核指标名", "目标值", "数据来源", "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月", "月计划产量", "月实际产量", "实际得分", "与目标值差异", "得分小计" };
            resultitem.Add("Columns", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(MouthArraycolumns)));
            result.Add(resultitem);
            byte[] filecontent = ExcelExportHelper.ExportExcel2(result, tableName, false);
            return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");

        }
        //月流失率显示

        //7s显示

        #endregion

        #region---排名计算条件
        //查询条件
        public ActionResult Calculation_Conditions(int? year, int? month, string department, string group)
        {
            JObject table = new JObject();
            JArray retul = new JArray();
            List<KPI_TotalDisplay> TotalDisplay = new List<KPI_TotalDisplay>();
            if (year != null && month != null)
            {
                TotalDisplay = db.KPI_TotalDisplay.Where(c => c.Time.Value.Year == year && c.Time.Value.Month == month).ToList();
            }
            if (department != null && year != null && month != null)
            {
                TotalDisplay = db.KPI_TotalDisplay.Where(c => c.Department == department && c.Time.Value.Year == year && c.Time.Value.Month == month).ToList();
            }
            if (group != null && year != null && month != null)
            {
                TotalDisplay = db.KPI_TotalDisplay.Where(c => c.Group == group && c.Time.Value.Year == year && c.Time.Value.Month == month).ToList();
            }
            if (department != null && year == null && month == null)
            {
                TotalDisplay = db.KPI_TotalDisplay.Where(c => c.Department == department).ToList();
            }
            if (group != null && year == null && month == null)
            {
                TotalDisplay = db.KPI_TotalDisplay.Where(c => c.Group == group).ToList();
            }
            if (TotalDisplay.Count > 0)
            {
                foreach (var item in TotalDisplay)
                {
                    //Id
                    table.Add("Id", item.Id == 0 ? 0 : item.Id);
                    //部门
                    table.Add("Department", item.Department == null ? null : item.Department);
                    //班组
                    table.Add("Group", item.Group == null ? null : item.Group);
                    //族群
                    table.Add("Ethnic_Group", item.Ethnic_Group == null ? null : item.Ethnic_Group);
                    //平均人数
                    table.Add("AvagePersonNum", item.AvagePersonNum == 0 ? 0 : item.AvagePersonNum);
                    //当月有无工伤事故
                    table.Add("InductrialAccident", item.InductrialAccident == 0 ? 0 : item.InductrialAccident);
                    //出勤率
                    var Attendance = (item.Attendance * 100) + "%";
                    table.Add("Attendance", Attendance);
                    //行政违规情况
                    table.Add("ViolationsMessage", item.ViolationsMessage == null ? null : item.ViolationsMessage);
                    //记录年月月份
                    var dateTime = string.Format("{0:yyyy-MM-dd}", item.Time);
                    table.Add("Time", dateTime == null ? null : dateTime);
                    retul.Add(table);
                    table = new JObject();
                }
            }
            return Content(JsonConvert.SerializeObject(retul));
        }

        //批量添加数据
        public ActionResult AddCalculation(List<KPI_TotalDisplay> totalDisplays)
        {
            JObject total = new JObject();
            JArray copy = new JArray();
            string rates = null;
            int count = 0;
            if (totalDisplays.Count > 0)
            {
                foreach (var item in totalDisplays)
                {
                    item.CreateTime = DateTime.Now;
                    item.Createor = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                    if (db.KPI_TotalDisplay.Count(c => c.Time == item.Time && c.Department == item.Department && c.Group == item.Group) > 0)
                    {
                        rates = rates + item.Time + "," + item.Department + "," + item.Group + "的记录重复了" + ";";
                        copy.Add(rates);
                    }
                }
                if (rates != null)
                {
                    total.Add("meg", false);
                    total.Add("feg", copy);
                    return Content(JsonConvert.SerializeObject(total));
                }
                db.KPI_TotalDisplay.AddRange(totalDisplays);
                count += db.SaveChanges();
                if (count == totalDisplays.Count)
                {
                    total.Add("meg", true);
                    total.Add("feg", "保存成功！");
                    total.Add("loss", JsonConvert.SerializeObject(totalDisplays));
                    return Content(JsonConvert.SerializeObject(total));
                }
                else
                {
                    total.Add("meg", false);
                    total.Add("feg", "保存出错！");
                    return Content(JsonConvert.SerializeObject(total));
                }
            }
            total.Add("meg", false);
            total.Add("feg", "数据错误/数据为空");
            return Content(JsonConvert.SerializeObject(total));
        }

        #endregion

        #region---班组评优月度汇总表方法（四个指标的汇总及审核操作方法）
        public class ExportHistoryQuery
        {
            public string Department { get; set; }
            public string Group { get; set; }
            public string Efficiency_Indicators { get; set; }
            public string Efficiency_IndexName { get; set; }
            public double Efficiency_Target { get; set; }
            public double Efficiency_Actual { get; set; }
            public double Efficiency_Single { get; set; }
            public double Efficiency_Score { get; set; }
            public string Quality_Indicators { get; set; }
            public string Quality_IndexName { get; set; }
            public double Quality_Target { get; set; }
            public double Quality_Actual { get; set; }
            public double Quality_Single { get; set; }
            public double Quality_Score { get; set; }
            public string Turnover_Indicators { get; set; }
            public string Turnover_IndexName { get; set; }
            public double Turnover_Target { get; set; }
            public double Turnover_Actual { get; set; }
            public double Turnover_Single { get; set; }
            public double Turnover_Score { get; set; }
            public string Comparison_Indicators { get; set; }
            public string Comparison_IndexName { get; set; }
            public double Comparison_Target { get; set; }
            public double Comparison_Actual { get; set; }
            public double Comparison_Single { get; set; }
            public double Comparison_Score { get; set; }
            public decimal TotalScore { get; set; }
            public bool Compare { get; set; }
            public decimal AvagePersonNum { get; set; }
            public decimal InductrialAccident { get; set; }
            public string Ethnic_Group { get; set; }
            public string Attendance { get; set; }
            public string ViolationsMessage { get; set; }
            public int Ranking { get; set; }
            public int Integral { get; set; }

        }

        //按年查找已核准的优秀班组清单
        public ActionResult ApprovedList(int year)
        {
            JObject table = new JObject();
            JArray list = new JArray();
            var monthList = db.KPI_ReviewSummary.GroupBy(c => c.Month).Select(c => new { Month = c.Key, List = c.ToList() }).ToList();
            if (monthList.Count > 0)
            {
                foreach (var m in monthList)
                {
                    table.Add("Time", year + "-" + m.Month);
                    bool judge = true;
                    foreach (var it in m.List)
                    {
                        if (it.Type == "确认")
                        {
                            table.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                            table.Add("HRAuditDate", it.HRAuditDate == null ? null : Convert.ToDateTime(it.HRAuditDate).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        if (it.Type == "审核")
                        {
                            table.Add("Audit", it.HRAudit == null ? null : it.HRAudit);
                            table.Add("AuditDate", it.HRAuditDate == null ? null : Convert.ToDateTime(it.HRAuditDate).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        if (it.Type == "核准")
                        {
                            if (it.HRAudit == null) judge = false;
                            table.Add("Approved", it.HRAudit == null ? null : it.HRAudit);
                            table.Add("ApprovedDate", it.HRAuditDate == null ? null : Convert.ToDateTime(it.HRAuditDate).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                    }
                    if (judge == true)
                    {
                        list.Add(table);
                        table = new JObject();
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(list));
        }


        //班组评优月度汇总表历史查询
        public ActionResult HistoryQuery(int year, int month)
        {
            JObject table = new JObject();
            JObject code = new JObject();
            JArray query = new JArray();
            JObject target = new JObject();
            var queryList = db.KPI_MonthlyIndicators.ToList();
            if (year != 0 && month != 0)
            {
                queryList = queryList.Where(c => c.Year == year && c.Month == month).ToList();
            }
            if (queryList.Count > 0)
            {
                foreach (var item in queryList)
                {
                    //id
                    table.Add("Id", item.Id == 0 ? 0 : item.Id);
                    //被考核部门
                    table.Add("Department", item.Department == null ? null : item.Department);
                    //班组
                    table.Add("Group", item.Group == null ? null : item.Group);
                    //效率指标
                    table.Add("Efficiency_Indicators", item.Efficiency_Indicators == null ? null : item.Efficiency_Indicators);
                    //指标名称(效率)
                    table.Add("Efficiency_IndexName", item.Efficiency_IndexName == null ? null : item.Efficiency_IndexName);
                    //目标值(效率)
                    var Efficiency_Unit = db.KPI_Indicators.Where(c => c.ExecutionTime.Value.Year == year && c.ExecutionTime.Value.Month == month && c.Department == item.Department && c.Group == item.Group && c.IndicatorsName == item.Efficiency_IndexName && c.IndicatorsValue == item.Efficiency_Target).Select(c => c.IndicatorsValueUnit).FirstOrDefault();
                    table.Add("Efficiency_Target", item.Efficiency_Target == 0 ? null : item.Efficiency_Target + Efficiency_Unit);
                    //实际完成值(效率)
                    table.Add("Efficiency_Actual", item.Efficiency_Actual == 0 ? 0 : item.Efficiency_Actual);
                    //单项得分(效率)
                    table.Add("Efficiency_Single", item.Efficiency_Single == 0 ? 0 : item.Efficiency_Single);
                    //得分小计(效率)
                    table.Add("Efficiency_Score", item.Efficiency_Score == 0 ? 0 : item.Efficiency_Score);

                    //品质指标
                    table.Add("Quality_Indicators", item.Quality_Indicators == null ? null : item.Quality_Indicators);
                    //指标名称(品质)
                    table.Add("Quality_IndexName", item.Quality_IndexName == null ? null : item.Quality_IndexName);
                    //目标值(品质)
                    var Quality_Unit = db.KPI_Indicators.Where(c => c.ExecutionTime.Value.Year == year && c.ExecutionTime.Value.Month == month && c.Department == item.Department && c.Group == item.Group && c.IndicatorsName == item.Quality_IndexName && c.IndicatorsValue == item.Quality_Target).Select(c => c.IndicatorsValueUnit).FirstOrDefault();
                    table.Add("Quality_Target", item.Quality_Target == 0 ? null : item.Quality_Target + Quality_Unit);
                    //实际完成值(品质)
                    table.Add("Quality_Actual", item.Quality_Actual == 0 ? 0 : item.Quality_Actual);
                    //单项得分(品质)
                    table.Add("Quality_Single", item.Quality_Single == 0 ? 0 : item.Quality_Single);
                    //得分小计(品质)
                    table.Add("Quality_Score", item.Quality_Score == 0 ? 0 : item.Quality_Score);

                    //月度流失率指标
                    table.Add("Turnover_Indicators", item.Turnover_Indicators == null ? null : item.Turnover_Indicators);
                    //指标名称(品质)
                    table.Add("Turnover_IndexName", item.Turnover_IndexName == null ? null : item.Turnover_IndexName);
                    //目标值(品质)
                    table.Add("Turnover_Target", item.Turnover_Target == 0 ? null : item.Turnover_Target + "%");
                    //实际完成值(品质)
                    table.Add("Turnover_Actual", item.Turnover_Actual == 0 ? 0 : item.Turnover_Actual);
                    //单项得分(品质)
                    table.Add("Turnover_Single", item.Turnover_Single == 0 ? 0 : item.Turnover_Single);
                    //得分小计(品质)
                    table.Add("Turnover_Score", item.Turnover_Score == 0 ? 0 : item.Turnover_Score);

                    //7S评比指标
                    table.Add("Comparison_Indicators", item.Comparison_Indicators == null ? null : item.Comparison_Indicators);
                    //指标名称(品质)
                    table.Add("Comparison_IndexName", item.Comparison_IndexName == null ? null : item.Comparison_IndexName);
                    //目标值(品质)
                    table.Add("Comparison_Target", item.Comparison_Target == 0 ? 0 : item.Comparison_Target);
                    //实际完成值(品质)
                    table.Add("Comparison_Actual", item.Comparison_Actual == 0 ? 0 : item.Comparison_Actual);
                    //单项得分(品质)
                    table.Add("Comparison_Single", item.Comparison_Single == 0 ? 0 : item.Comparison_Single);
                    //得分小计(品质)
                    table.Add("Comparison_Score", item.Comparison_Score == 0 ? 0 : item.Comparison_Score);

                    //合计总分
                    table.Add("TotalScore", item.TotalScore == 0 ? 0 : item.TotalScore);
                    //合计总分是否大于等于90
                    table.Add("Compare", item.TotalScore >= 90 == true ? true : false);
                    //平均人数（不少于5人）
                    table.Add("AvagePersonNum", item.AvagePersonNum == 0 ? 0 : item.AvagePersonNum);
                    //当月有无工伤事故
                    table.Add("InductrialAccident", item.InductrialAccident == 0 ? 0 : item.InductrialAccident);

                    //族群
                    table.Add("Ethnic_Group", item.Ethnic_Group == null ? null : item.Ethnic_Group);

                    //出勤率
                    table.Add("Attendance", item.Attendance == 0 ? 0 : item.Attendance);
                    //行政违规情况
                    table.Add("ViolationsMessage", item.ViolationsMessage == null ? null : item.ViolationsMessage);
                    //排名
                    table.Add("Ranking", item.Ranking == 0 ? 0 : item.Ranking);
                    //积分
                    table.Add("integral", item.integral == 0 ? 0 : item.integral);
                    query.Add(table);
                    table = new JObject();
                }
            }
            var codelist = db.KPI_ReviewSummary.Where(c => c.Year == year && c.Month == month).ToList();
            if (codelist.Count == 0)
            {
                code.Add("Code", null);
            }
            else
            {
                //审核列表
                JObject confirm = new JObject();//确认
                JObject audit = new JObject();//审核
                JObject approved = new JObject();//核准
                foreach (var it in codelist)
                {
                    if (it.Type == "确认")
                    {
                        confirm.Add("HRAuditDate", it.HRAuditDate == null ? null : it.HRAuditDate.ToString());
                        confirm.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                        confirm.Add("HRjudge", it.HRjudge == false ? false : it.HRjudge);
                    }
                    if (it.Type == "审核")
                    {
                        audit.Add("HRAuditDate", it.HRAuditDate == null ? null : it.HRAuditDate.ToString());
                        audit.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                        audit.Add("HRjudge", it.HRjudge == false ? false : it.HRjudge);
                    }
                    if (it.Type == "核准")
                    {
                        approved.Add("HRAuditDate", it.HRAuditDate == null ? null : it.HRAuditDate.ToString());
                        approved.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                        approved.Add("HRjudge", it.HRjudge == false ? false : it.HRjudge);
                    }
                }
                code.Add("Confirm", confirm);//确认
                confirm = new JObject();
                code.Add("Audit", audit);//审核
                audit = new JObject();
                code.Add("Approved", approved);//核准
                approved = new JObject();
            }
            target.Add("Query", query);
            target.Add("Code", code);
            return Content(JsonConvert.SerializeObject(target));
        }

        //批准后保存班组评优月度汇总表
        public ActionResult ADDSummaryTable(List<KPI_MonthlyIndicators> indicators, int year, int month)
        {
            JObject table = new JObject();
            string rates = null;
            JArray copy = new JArray();
            if (indicators.Count > 0 && year != 0 && month != 0)
            {
                foreach (var ite in indicators)
                {
                    if (db.KPI_MonthlyIndicators.Count(c => c.Year == year && c.Month == month && c.Department == ite.Department && c.Group == ite.Group) > 0)
                    {
                        rates = rates + year + "-" + month + "," + ite.Department + "," + ite.Group + "的记录重复了" + ";";
                        copy.Add(rates);
                    }
                }
                if (rates != null)
                {
                    table.Add("meg", false);
                    table.Add("feg", copy);
                    return Content(JsonConvert.SerializeObject(table));
                }
                foreach (var item in indicators)
                {
                    item.Year = year;
                    item.Month = month;
                    db.SaveChanges();
                }
                db.KPI_MonthlyIndicators.AddRange(indicators);
                int count = db.SaveChanges();
                if (count > 0)
                {
                    table.Add("mes", true);
                    table.Add("feg", "保存成功");
                    table.Add("con", JsonConvert.SerializeObject(indicators));
                    return Content(JsonConvert.SerializeObject(table));
                }
                else
                {
                    table.Add("mes", false);
                    table.Add("feg", "保存失败");
                    return Content(JsonConvert.SerializeObject(table));
                }
            }
            table.Add("mes", false);
            table.Add("feg", "数据错误");
            return Content(JsonConvert.SerializeObject(table));
        }

        //审核班组评优月度汇总表方法
        //public ActionResult Audit_SummarySheet1(int year, int month, string hrAudit, bool hrjudge, string type)
        //{
        //    JObject review = new JObject();
        //    if (year != 0 && month != 0 && hrAudit != null)
        //    {
        //        KPI_ReviewSummary summary = new KPI_ReviewSummary();
        //        var audit = db.KPI_ReviewSummary.Where(c => c.Year == year && c.Month == month && c.HRAudit == hrAudit && c.HRjudge == hrjudge).FirstOrDefault();
        //        if (audit == null)
        //        {
        //            summary.Year = year;
        //            summary.Month = month;
        //            summary.HRAudit = hrAudit;
        //            summary.Type = type;
        //            summary.Department = ((Users)Session["User"]).Department;
        //            summary.HRAuditDate = DateTime.Now;
        //            summary.HRjudge = hrjudge;
        //            db.KPI_ReviewSummary.Add(summary);
        //            int count = db.SaveChanges();
        //            if (count > 0)
        //            {
        //                review.Add("Meg", true);
        //                review.Add("Feg", "审核成功！");
        //                review.Add("HRAudit", hrAudit);
        //                review.Add("HRAuditDate", summary.HRAuditDate.ToString());
        //                return Content(JsonConvert.SerializeObject(review));
        //            }
        //            else
        //            {
        //                review.Add("Meg", false);
        //                review.Add("Feg", "审核失败！");
        //                return Content(JsonConvert.SerializeObject(review));
        //            }
        //        }
        //        else
        //        {
        //            review.Add("Meg", false);
        //            review.Add("Feg", year + "年" + month + "月" + "的月度指标汇总数据，您已审核！");
        //            return Content(JsonConvert.SerializeObject(review));
        //        }
        //    }
        //    review.Add("Meg", false);
        //    review.Add("Feg", "数据错误！");
        //    return Content(JsonConvert.SerializeObject(review));
        //}
        public ActionResult Audit_SummarySheet(int year, int month, string hrAudit, bool hrjudge, string type)
        {
            JObject review = new JObject();
            if (year != 0 && month != 0 && hrAudit != null)
            {
                KPI_ReviewSummary summary = new KPI_ReviewSummary();
                var audit = db.KPI_ReviewSummary.Where(c => c.Year == year && c.Month == month && c.Type == type).FirstOrDefault();
                if (audit == null)
                {
                    if (db.KPI_ReviewSummary.Where(c => c.Year == year && c.Month == month && c.HRAudit == hrAudit).Select(c => c.Type).FirstOrDefault() != null)
                    {
                        review.Add("Meg", false);
                        review.Add("Feg", hrAudit + "已经审核过了,不能重复审核!");
                        return Content(JsonConvert.SerializeObject(review));
                    }
                    summary.Year = year;
                    summary.Month = month;
                    summary.HRAudit = hrAudit;
                    summary.Type = type;
                    summary.Department = ((Users)Session["User"]).Department;
                    summary.HRAuditDate = DateTime.Now;
                    summary.HRjudge = hrjudge;
                    db.KPI_ReviewSummary.Add(summary);
                    int count = db.SaveChanges();
                    if (count > 0)
                    {
                        review.Add("Meg", true);
                        review.Add("Feg", "审核成功！");
                        review.Add("HRAudit", hrAudit);
                        review.Add("HRAuditDate", summary.HRAuditDate.ToString());
                        return Content(JsonConvert.SerializeObject(review));
                    }
                    else
                    {
                        review.Add("Meg", false);
                        review.Add("Feg", "审核失败！");
                        return Content(JsonConvert.SerializeObject(review));
                    }
                }
                else
                {
                    review.Add("Meg", false);
                    review.Add("Feg", year + "年" + month + "月" + "的月度指标汇总数据，已审核！,请刷新");
                    return Content(JsonConvert.SerializeObject(review));
                }
            }
            review.Add("Meg", false);
            review.Add("Feg", "数据错误！");
            return Content(JsonConvert.SerializeObject(review));
        }

        //导出班组评优月度汇总表(Excel),tableData(前面需要把数据全部转换成字符串，再传给后台)
        [HttpPost]
        public FileContentResult Export_SummaryTable(int year, int month)
        {
            var dataList = db.KPI_MonthlyIndicators.Where(c => c.Year == year && c.Month == month).ToList();
            List<ExportHistoryQuery> Resultlist = new List<ExportHistoryQuery>();
            var departmentlist = dataList.Select(c => c.Department).Distinct().ToList();
            foreach (var item in departmentlist)//循环被考核部门
            {
                var DG_list = dataList.Where(c => c.Department == item).Select(c => c.Group).Distinct().ToList();
                foreach (var ite in DG_list)//循环被考核部门的班组
                {
                    ExportHistoryQuery at = new ExportHistoryQuery();
                    var tarodlist = dataList.Where(c => c.Department == item && c.Group == ite).ToList();
                    at.Department = tarodlist.FirstOrDefault().Department;
                    at.Group = tarodlist.FirstOrDefault().Group;
                    //效率
                    at.Efficiency_Indicators = "效率指标";
                    at.Efficiency_IndexName = tarodlist.FirstOrDefault().Efficiency_IndexName;
                    at.Efficiency_Target = tarodlist.FirstOrDefault().Efficiency_Target;
                    at.Efficiency_Actual = tarodlist.FirstOrDefault().Efficiency_Actual;
                    at.Efficiency_Single = tarodlist.FirstOrDefault().Efficiency_Single;
                    at.Efficiency_Score = tarodlist.FirstOrDefault().Efficiency_Score;
                    //品质
                    at.Quality_Indicators = "品质指标";
                    at.Quality_IndexName = tarodlist.FirstOrDefault().Quality_IndexName;
                    at.Quality_Target = tarodlist.FirstOrDefault().Quality_Target;
                    at.Quality_Actual = tarodlist.FirstOrDefault().Quality_Actual;
                    at.Quality_Single = tarodlist.FirstOrDefault().Quality_Single;
                    at.Quality_Score = tarodlist.FirstOrDefault().Quality_Score;
                    //月流失率
                    at.Turnover_Indicators = "月流失率指标";
                    at.Turnover_IndexName = tarodlist.FirstOrDefault().Turnover_IndexName;
                    at.Turnover_Target = tarodlist.FirstOrDefault().Turnover_Target;
                    at.Turnover_Actual = tarodlist.FirstOrDefault().Turnover_Actual;
                    at.Turnover_Single = tarodlist.FirstOrDefault().Turnover_Single;
                    at.Turnover_Score = tarodlist.FirstOrDefault().Turnover_Score;
                    //7S评比
                    at.Comparison_Indicators = "7S评比指标";
                    at.Comparison_IndexName = tarodlist.FirstOrDefault().Comparison_IndexName;
                    at.Comparison_Target = tarodlist.FirstOrDefault().Comparison_Target;
                    at.Comparison_Actual = tarodlist.FirstOrDefault().Comparison_Actual;
                    at.Comparison_Single = tarodlist.FirstOrDefault().Comparison_Single;
                    at.Comparison_Score = tarodlist.FirstOrDefault().Comparison_Score;

                    at.TotalScore = tarodlist.FirstOrDefault().TotalScore;
                    at.Compare = tarodlist.FirstOrDefault().TotalScore >= 90 == true ? true : false;
                    at.AvagePersonNum = tarodlist.FirstOrDefault().AvagePersonNum;
                    at.InductrialAccident = tarodlist.FirstOrDefault().InductrialAccident;
                    at.Ethnic_Group = tarodlist.FirstOrDefault().Ethnic_Group;
                    at.Attendance = tarodlist.FirstOrDefault().Attendance.ToString() + "%";
                    at.ViolationsMessage = tarodlist.FirstOrDefault().ViolationsMessage;
                    at.Ranking = tarodlist.FirstOrDefault().Ranking;
                    at.Integral = tarodlist.FirstOrDefault().integral;

                    Resultlist.Add(at);
                }
            }
            // 导出表格名称
            string tableName = "班组评优汇总表" + year + "年" + month + "月";
            if (Resultlist.Count() > 0)
            {
                string[] columns = { "部门", "班组", "效率指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "品质指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "月度流失率指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "7S评比指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "合计总分", "总分不低于90分", "平均人数（不少于5人）", "当月有无工伤事故", "族群", "出勤率", "行政违纪情况", "排名", "积分" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
            }
            else
            {
                ExportHistoryQuery at1 = new ExportHistoryQuery();
                at1.Group = "没有找到相关记录！";
                Resultlist.Add(at1);
                string[] columns = { "部门", "班组", "效率指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "品质指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计","月度流失率指标","考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计" ,
                    "7S评比指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计" ,"合计总分", "总分不低于90分", "平均人数（不少于5人）", "当月有无工伤事故","族群", "出勤率", "行政违纪情况","排名", "积分"};
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
            }
        }

        //导出临时查询结果为Excel文件
        [HttpPost]
        public FileContentResult Export_TemporaryTable(List<KPI_MonthlyIndicators> indicators, int year, int month)
        {
            List<ExportHistoryQuery> Resultlist = new List<ExportHistoryQuery>();
            var departmentlist = indicators.Select(c => c.Department).Distinct().ToList();
            foreach (var item in departmentlist)//循环被考核部门
            {
                var DG_list = indicators.Where(c => c.Department == item).Select(c => c.Group).Distinct().ToList();
                foreach (var ite in DG_list)//循环被考核部门的班组
                {
                    ExportHistoryQuery at = new ExportHistoryQuery();
                    var tarodlist = indicators.Where(c => c.Department == item && c.Group == ite).ToList();
                    at.Department = tarodlist.FirstOrDefault().Department;
                    at.Group = tarodlist.FirstOrDefault().Group;
                    //效率
                    at.Efficiency_Indicators = tarodlist.FirstOrDefault().Efficiency_Indicators;
                    at.Efficiency_IndexName = tarodlist.FirstOrDefault().Efficiency_IndexName;
                    at.Efficiency_Target = tarodlist.FirstOrDefault().Efficiency_Target;
                    at.Efficiency_Actual = tarodlist.FirstOrDefault().Efficiency_Actual;
                    at.Efficiency_Single = tarodlist.FirstOrDefault().Efficiency_Single;
                    at.Efficiency_Score = tarodlist.FirstOrDefault().Efficiency_Score;
                    //品质
                    at.Quality_Indicators = tarodlist.FirstOrDefault().Quality_Indicators;
                    at.Quality_IndexName = tarodlist.FirstOrDefault().Quality_IndexName;
                    at.Quality_Target = tarodlist.FirstOrDefault().Quality_Target;
                    at.Quality_Actual = tarodlist.FirstOrDefault().Quality_Actual;
                    at.Quality_Single = tarodlist.FirstOrDefault().Quality_Single;
                    at.Quality_Score = tarodlist.FirstOrDefault().Quality_Score;
                    //月流失率
                    at.Turnover_Indicators = tarodlist.FirstOrDefault().Turnover_Indicators;
                    at.Turnover_IndexName = tarodlist.FirstOrDefault().Turnover_IndexName;
                    at.Turnover_Target = tarodlist.FirstOrDefault().Turnover_Target;
                    at.Turnover_Actual = tarodlist.FirstOrDefault().Turnover_Actual;
                    at.Turnover_Single = tarodlist.FirstOrDefault().Turnover_Single;
                    at.Turnover_Score = tarodlist.FirstOrDefault().Turnover_Score;
                    //7S评比
                    at.Comparison_Indicators = tarodlist.FirstOrDefault().Comparison_Indicators;
                    at.Comparison_IndexName = tarodlist.FirstOrDefault().Comparison_IndexName;
                    at.Comparison_Target = tarodlist.FirstOrDefault().Comparison_Target;
                    at.Comparison_Actual = tarodlist.FirstOrDefault().Comparison_Actual;
                    at.Comparison_Single = tarodlist.FirstOrDefault().Comparison_Single;
                    at.Comparison_Score = tarodlist.FirstOrDefault().Comparison_Score;

                    at.TotalScore = tarodlist.FirstOrDefault().TotalScore;
                    at.Compare = tarodlist.FirstOrDefault().TotalScore >= 90 == true ? true : false;
                    at.AvagePersonNum = tarodlist.FirstOrDefault().AvagePersonNum;
                    at.InductrialAccident = tarodlist.FirstOrDefault().InductrialAccident;
                    at.Ethnic_Group = tarodlist.FirstOrDefault().Ethnic_Group;
                    at.Attendance = tarodlist.FirstOrDefault().Attendance.ToString() + "%";
                    at.ViolationsMessage = tarodlist.FirstOrDefault().ViolationsMessage;
                    at.Ranking = tarodlist.FirstOrDefault().Ranking;
                    at.Integral = tarodlist.FirstOrDefault().integral;

                    Resultlist.Add(at);
                }
            }
            // 导出表格名称

            string tableName = "班组评优汇总表" + year + "年" + month + "月";
            if (Resultlist.Count() > 0)
            {
                string[] columns = { "部门", "班组", "效率指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "品质指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计","月度流失率指标","考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计" ,
                    "7S评比指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计" ,"合计总分", "总分不低于90分", "平均人数（不少于5人）", "当月有无工伤事故","族群", "出勤率", "行政违纪情况","排名", "积分"};
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
            }
            else
            {
                ExportHistoryQuery at1 = new ExportHistoryQuery();
                at1.Group = "没有找到相关记录！";
                Resultlist.Add(at1);
                string[] columns = { "部门", "班组", "效率指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "品质指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计","月度流失率指标","考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计" ,
                    "7S评比指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计" ,"合计总分", "总分不低于90分", "平均人数（不少于5人）", "当月有无工伤事故","族群", "出勤率", "行政违纪情况","排名", "积分"};
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
            }
        }
        #endregion

        #region---光荣榜（优秀班组）
        public class IntegralList
        {
            public string group { get; set; }
            public int integral { get; set; }
            public string department { get; set; }
        }

        //班组评优月度积分统计表 首页
        public ActionResult KPI_HonorRoll()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_HonorRoll" });
            }
            return View();
        }

        //上传优秀班组图片
        public bool UploadFile_KPI(int year, int month, int ranking, string group)
        {
            string rankingList = "第" + ranking + "名";
            string datetime = null;
            if (Request.Files.Count > 0)//判断请求文件是否大于0
            {
                HttpPostedFileBase file = Request.Files["UploadFile_KPI"];//获取上传内容
                var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();//截取文件名的后缀

                if (year != 0 && month != 0 && group != null && rankingList != null)
                {
                    datetime = year + "年" + month + "月";
                    //判断该文件夹是否等于false（有没有该文件夹）
                    if (Directory.Exists(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\") == false)//如果不存在就创建订单文件夹
                    {
                        //创建文件夹
                        Directory.CreateDirectory(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\");
                    }
                    if (fileType == ".pdf")
                    {
                        //把文件保存到对应的文件夹下
                        file.SaveAs(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\" + group + "\\" + rankingList + ".pdf");
                    }
                    if (fileType == ".jpg")
                    {
                        //把文件保存到对应的文件夹下
                        file.SaveAs(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\" + group + "\\" + rankingList + ".jpg");
                    }
                }
                else if (year != 0 && month == 0 && group != null && rankingList != null)
                {
                    datetime = year + "年";
                    //判断该文件夹是否等于false（有没有该文件夹）
                    if (Directory.Exists(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\") == false)//如果不存在就创建订单文件夹
                    {
                        //创建文件夹
                        Directory.CreateDirectory(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\");
                    }
                    if (fileType == ".pdf")
                    {
                        //把文件保存到对应的文件夹下
                        file.SaveAs(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\" + group + "\\" + rankingList + ".pdf");
                    }
                    if (fileType == ".jpg")
                    {
                        //把文件保存到对应的文件夹下
                        file.SaveAs(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\" + group + "\\" + rankingList + ".jpg");
                    }
                }
                return true;
            }
            return false;
        }

        //优秀班组图片更换(替换)上传
        public bool UploadFile_KPI_Replace(string OrignFile, int year, int month, int ranking, string group)
        {
            bool retul = true;
            string rankingList = "第" + ranking + "名";
            string datetime = null;
            if (year != 0 && month != 0)
            {
                datetime = year + "年" + month + "月";
            }
            else if (year != 0 && month == 0)
            {
                datetime = year + "年";
            }
            var filename = OrignFile.Substring(OrignFile.LastIndexOf('/') + 1);//以最后一个斜杠截取路径（只保留文件名）
            if (Directory.Exists(@"D:\MES_Data\优秀班组光荣榜\" + datetime + "\\" + group + "\\已替换文件\\") == false)
            {
                Directory.CreateDirectory(@"D:\MES_Data\优秀班组光荣榜\" + datetime + "\\" + group + "\\已替换文件\\");
            }
            var NewFile = @"D:\MES_Data\优秀班组光荣榜\" + datetime + "\\" + group + "\\已替换文件\\" + filename;
            try
            {
                var conversion = OrignFile.Replace("/", "\\");//把旧文件路径的斜杠（/）替换成这个斜杠（\）
                System.IO.File.Move("D:" + conversion, NewFile);//把旧文件移动到NewFile这个文件夹下
            }
            catch //捕捉异常
            {
                retul = false;
            }
            if (!UploadFile_KPI(year, month, ranking, group))//判断调用的方法是否有这些参数
            {
                retul = false;
            }
            return retul;
        }

        //显示年/月度优秀班组前五名的数据和图片
        public ActionResult Monthly_ExcellentTeam(int year, int month)
        {
            JObject groupList = new JObject();
            JArray table = new JArray();
            List<KPI_MonthlyIndicators> rankingList = new List<KPI_MonthlyIndicators>();
            string datetime = null;
            List<IntegralList> integ = new List<IntegralList>();

            if (year != 0 && month != 0)//按月度查询光荣榜
            {
                rankingList = db.KPI_MonthlyIndicators.Where(c => c.Year == year && c.Month == month && c.Ranking != 0 && c.integral != 0).ToList();
                datetime = year + "年" + month + "月";
                var codeList = rankingList.Where(c => c.Ranking != 0 && c.integral != 0).Select(c => c.Department).Distinct().ToList();
                foreach (var item in codeList)
                {
                    var gioupList = rankingList.Where(c => c.Ranking != 0 && c.integral != 0 && c.Department == item).Select(c => c.Group).Distinct().ToList();
                    foreach (var ite in gioupList)
                    {
                        int inte = rankingList.Where(c => c.Department == item && c.Group == ite && c.Year == year && c.Month == month).Select(c => c.Ranking).FirstOrDefault();
                        var ranking = "第" + inte + "名";
                        //名次                      
                        groupList.Add("Ranking", ranking == null ? null : ranking);
                        //图片路径 
                        if (Directory.Exists(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\" + ite + "\\" + ranking) == false)
                        {
                            groupList.Add("Path", null);
                        }
                        else
                        {
                            List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\" + ite + "\\" + ranking);

                            string path = @"/MES_Data/优秀班组光荣榜/月度/" + datetime + "/" + ite + "/" + ranking;//组合出路径
                            var filetype = path.Split('.');//将组合出来的路径以点分隔，方便下一步判断后缀
                            if (filetype[1] == "jpg")//后缀为jpg
                            {
                                groupList.Add("Path", path == null ? null : path);
                            }
                            else //后缀为其他
                            {
                                groupList.Add("Path", path == null ? null : path);
                            }
                        }
                        table.Add(groupList);
                        groupList = new JObject();
                    }
                }
            }
            if (year != 0 && month == 0)//按年度查询光荣榜
            {
                rankingList = db.KPI_MonthlyIndicators.Where(c => c.Year == year && c.Ranking != 0 && c.integral != 0).ToList();
                datetime = year + "年";
                var codeList = rankingList.Where(c => c.Ranking != 0 && c.integral != 0).Select(c => c.Department).Distinct().ToList();
                foreach (var item in codeList)
                {
                    var gioupList = rankingList.Where(c => c.Ranking != 0 && c.integral != 0 && c.Department == item).Select(c => c.Group).Distinct().ToList();
                    foreach (var ite in gioupList)
                    {
                        int integl = rankingList.Where(c => c.Department == item && c.Group == ite && c.Year == year).Select(c => c.integral).Sum();
                        IntegralList aa = new IntegralList() { department = item, group = ite, integral = integl };
                        integ.Add(aa);
                    }
                }
                int i = 1;
                var f = integ.OrderByDescending(c => c.integral).Take(5);
                foreach (var it in f)
                {
                    //部门
                    groupList.Add("Department", it.department == null ? null : it.department);
                    //班组
                    groupList.Add("Group", it.group == null ? null : it.group);
                    string ranking = "第" + i + "名";
                    //名次                      
                    groupList.Add("Ranking", ranking);
                    i++;
                    //图片路径  
                    if (Directory.Exists(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\" + it.group + "\\" + ranking) == false)
                    {
                        groupList.Add("Path", null);
                    }
                    else
                    {
                        List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\" + it.group + "\\" + ranking);

                        string path = @"/MES_Data/优秀班组光荣榜/年度/" + datetime + "/" + it.group + "/" + ranking;//组合出路径
                        var filetype = path.Split('.');//将组合出来的路径以点分隔，方便下一步判断后缀
                        if (filetype[1] == "jpg")//后缀为jpg
                        {
                            groupList.Add("Path", path == null ? null : path);
                        }
                        else //后缀为其他
                        {
                            groupList.Add("Path", path == null ? null : path);
                        }
                    }
                    table.Add(groupList);
                    groupList = new JObject();
                }
            }
            return Content(JsonConvert.SerializeObject(table));
        }

        #endregion

        #region---班组评优月度积分统计表

        //班组评优月度积分统计表 首页
        public ActionResult IntegralTable_Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "IntegralTable_Index" });
            }
            return View();
        }


        //查看班组评优月度积分统计表
        public ActionResult IntegralTable(int year)
        {
            JObject table = new JObject();
            JArray userJobject = new JArray();
            JObject time = new JObject();
            var dataList = db.KPI_MonthlyIndicators.ToList();
            if (year != 0)
            {
                dataList = dataList.Where(c => c.Year == year).ToList();
            }
            int combined = 0;
            var departmentlist = dataList.Select(c => c.Department).Distinct().ToList();
            foreach (var item in departmentlist)//循环被考核部门
            {
                var DG_list = dataList.Where(c => c.Department == item).Select(c => c.Group).Distinct().ToList();
                foreach (var ite in DG_list)//循环被考核部门的班组
                {
                    var tarodlist = dataList.Where(c => c.Department == item && c.Group == ite).FirstOrDefault();
                    //ID
                    table.Add("Id", tarodlist.Id == 0 ? 0 : tarodlist.Id);
                    //部门
                    table.Add("Department", tarodlist.Department == null ? null : tarodlist.Department);
                    //班组
                    table.Add("Group", tarodlist.Group == null ? null : tarodlist.Group);
                    for (var i = 1; i <= 12; i++)
                    {
                        var date = dataList.Where(c => c.Department == item && c.Group == ite && c.Year == year && c.Month == i).Select(c => c.integral).FirstOrDefault();
                        //月份积分
                        if (i == 1) time.Add("January", date);
                        if (i == 2) time.Add("February", date);
                        if (i == 3) time.Add("March", date);
                        if (i == 4) time.Add("April", date);
                        if (i == 5) time.Add("May", date);
                        if (i == 6) time.Add("June", date);
                        if (i == 7) time.Add("July", date);
                        if (i == 8) time.Add("August", date);
                        if (i == 9) time.Add("September", date);
                        if (i == 10) time.Add("October", date);
                        if (i == 11) time.Add("November", date);
                        if (i == 12) time.Add("December", date);
                        combined = combined + date;
                    }
                    table.Add("Time", time);
                    time = new JObject();
                    //年度积分合计
                    table.Add("TotalScore", combined == 0 ? 0 : combined);
                    combined = new int();
                    userJobject.Add(table);
                    table = new JObject();
                }
            }
            return Content(JsonConvert.SerializeObject(userJobject));
        }

        //导出Excel表格自定义类
        public class ExportIntegralTable
        {
            public string Department { get; set; }//部门
            public string Group { get; set; }//班组
            public int Month1 { get; set; }//1月
            public int Month2 { get; set; }//2月
            public int Month3 { get; set; }//3月
            public int Month4 { get; set; }//4月
            public int Month5 { get; set; }//5月
            public int Month6 { get; set; }//6月
            public int Month7 { get; set; }//7月
            public int Month8 { get; set; }//8月
            public int Month9 { get; set; }//9月
            public int Month10 { get; set; }//10月
            public int Month11 { get; set; }//11月
            public int Month12 { get; set; }//12月
            public int YearIntegral { get; set; }//年度积分合计

        }

        //导出班组评优月度积分统计表(Excel)
        [HttpPost]
        public FileContentResult Integral_OutputExcel(int year)
        {
            var dataList = db.KPI_MonthlyIndicators.Where(c => c.Year == year).ToList();
            List<ExportIntegralTable> Resultlist = new List<ExportIntegralTable>();
            var departmentlist = dataList.Select(c => c.Department).Distinct().ToList();
            foreach (var item in departmentlist)//循环被考核部门
            {
                var DG_list = dataList.Where(c => c.Department == item).Select(c => c.Group).Distinct().ToList();
                foreach (var ite in DG_list)//循环被考核部门的班组
                {
                    ExportIntegralTable at = new ExportIntegralTable();
                    var tarodlist = dataList.Where(c => c.Department == item && c.Group == ite).ToList();
                    at.Department = tarodlist.FirstOrDefault().Department;
                    at.Group = tarodlist.FirstOrDefault().Group;
                    //赋值12个月
                    int totalscore = 0;
                    var filds = at.GetType().GetFields(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy);
                    filds = filds.Where(c => c.Name.Contains("Month")).ToArray();
                    for (var i = 0; i < filds.Count(); i++)
                    {

                        var ifno = dataList.Where(c => c.Department == item && c.Group == ite && c.Month == i).Select(c => c.integral).FirstOrDefault();
                        filds[i].SetValue(at, ifno);
                        totalscore = totalscore + ifno;
                    }
                    at.YearIntegral = totalscore;
                    Resultlist.Add(at);
                }
            }
            // 导出表格名称
            string tableName = "班组评优月度积分统计表";
            if (Resultlist.Count() > 0)
            {
                string[] columns = { "部门", "班组", "1月", "2月", "3月", "4月", "5月", "7月", "8月", "9月", "10月", "11月", "12月", "年度积分合计" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
            }
            else
            {
                ExportIntegralTable at1 = new ExportIntegralTable();
                at1.Group = "没有找到相关记录！";
                Resultlist.Add(at1);
                string[] columns = { "部门", "班组", "1月", "2月", "3月", "4月", "5月", "7月", "8月", "9月", "10月", "11月", "12月", "年度积分合计" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
            }
        }

        #endregion

        #region 列表
        //拿到工段列表
        public ActionResult GetSectionList()
        {
            var orders = db.Plan_SectionParameter.OrderByDescending(m => m.Id).Select(m => m.Section).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //根据工段拿到工序
        public ActionResult GetProcessList(string section)
        {
            var orders = db.Plan_SectionParameter.OrderByDescending(m => m.Id).Where(c => c.Section == section).Select(m => m.Process).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //指标查询执行时间下拉表
        public ActionResult GetExTime()
        {
            var orders = db.KPI_Indicators.OrderByDescending(m => m.Id).Select(m => m.ExecutionTime).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item.ToString());

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //全部部门
        public ActionResult GetDeprment()
        {
            var orders = db.KPI_Indicators.OrderByDescending(m => m.Id).Select(m => m.Department).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //全部班组
        public ActionResult GetGroup()
        {
            var orders = db.KPI_Indicators.OrderByDescending(m => m.Id).Select(m => m.Group).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //查找班组
        public ActionResult GetDepartmentGroup(string Department)
        {
            var orders = db.KPI_Indicators.OrderByDescending(m => m.Id).Where(c => c.Department == Department).Select(m => m.Group).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //查找所有部门班组工段工序对应json
        public ActionResult GetRelation()
        {
            JArray result = new JArray();
            var tempvalue = db.Plan_SectionParameter.Select(c => new TempIndicators { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section });
            var deparment = tempvalue.Select(c => c.Department).Distinct().ToList();
            foreach (var depitem in deparment)//循环部门
            {
                JObject dep = new JObject();
                dep.Add("Department", depitem);
                var group = tempvalue.Where(c => c.Department == depitem).Select(c => c.Group).Distinct().ToList();
                JArray groupArray = new JArray();
                foreach (var groupitem in group)//循环班组
                {
                    JObject groupobj = new JObject();
                    groupobj.Add("Group", groupitem);
                    var seaction = tempvalue.Where(c => c.Department == depitem && c.Group == groupitem).Select(c => c.Section).Distinct().ToList();
                    JArray seaArray = new JArray();
                    foreach (var sea in seaction)//循环工段
                    {
                        JObject seaobj = new JObject();
                        seaobj.Add("Section", sea);
                        var process = tempvalue.Where(c => c.Department == depitem && c.Group == groupitem && c.Section == sea).Select(c => c.Process).Distinct().ToList();
                        JArray proArray = new JArray();
                        foreach (var pro in process)//循环工序
                        {
                            JObject proobj = new JObject();
                            proobj.Add("Process", pro);
                            proArray.Add(proobj);
                        }
                        seaobj.Add("TProcess", proArray);
                        seaArray.Add(seaobj);
                    }
                    groupobj.Add("TSection", seaArray);
                    groupArray.Add(groupobj);
                }
                dep.Add("TGroup", groupArray);
                result.Add(dep);
            }

            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion
    }

    public class KPI_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController commom = new CommonController();

        #region---自定义类
        public class TempIndicators
        {
            public string Department { get; set; }
            public string Group { get; set; }
            public double IndicatorsValue { get; set; }
            public string IndicatorsName { get; set; }
            public string SourceDepartment { get; set; }
            public string IndicatorsType { get; set; }
            public string Cycle { get; set; }
            public string IndicatorsValueUnit { get; set; }
            public string Process { get; set; }
            public string Section { get; set; }
            public string IndicatorsStatue { get; set; }
            public decimal Weight { get; set; }
        }
        public class TempValue
        {

            public string Department { get; set; }
            public string Group { get; set; }
            public string Process { get; set; }
            public string Section { get; set; }
            public string CheckDepartment { get; set; }
            public string CheckGroup { get; set; }
            public string CheckProcess { get; set; }
            public string CheckSection { get; set; }
            public string Types { get; set; }
            public DateTime? Time { get; set; }
            public int Num { get; set; }
            public int CheckNum { get; set; }
            public int AbNum { get; set; }

        }

        #endregion

        #region---7S管理

        #region---检查参考标准

        #region--检查参考标准数据录入   
        [HttpPost]
        [ApiAuthorize]
        public JObject ReferenceStandard_Input([System.Web.Http.FromBody]JObject data)//List<KPI_7S_ReferenceStandard> record
        {
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            List<KPI_7S_ReferenceStandard> record = (List<KPI_7S_ReferenceStandard>)JsonHelper.jsonDes<List<KPI_7S_ReferenceStandard>>(data["record"].ToString());
            JObject result = new JObject();
            if (record.Count > 0)
            {
                int savecout = 0;
                foreach (var item in record)
                {
                    //检验记录是否存在
                    if (db.KPI_7S_ReferenceStandard.Count(c => c.PointsType == item.PointsType && c.ReferenceStandard == item.ReferenceStandard) < 1)
                    {
                        item.InputTime = DateTime.Now;
                        item.InputPerson = auth.UserName;
                        db.SaveChanges();
                        db.KPI_7S_ReferenceStandard.Add(item);
                        savecout += db.SaveChanges();
                    }
                }
                if (savecout > 0)
                {
                    return commom.GetModuleFromJobjet(result, true, "保存成功！");
                }
                else
                {
                    return commom.GetModuleFromJobjet(result, false, "保存失败！");
                }
            }
            else
            {
                return commom.GetModuleFromJobjet(result, false, "传入参数为空！");
            }
        }
        #endregion

        #region--检查参考标准查询
        [HttpPost]
        [ApiAuthorize]
        public JObject ReferenceStandard_query([System.Web.Http.FromBody]JObject data)//string[] pointsType
        {
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            JArray array = (JArray)JsonConvert.DeserializeObject(obj.pointsType.ToString());
            string[] pointsType = array.ToObject<List<string>>().ToArray();
            List<KPI_7S_ReferenceStandard> newList = new List<KPI_7S_ReferenceStandard>();
            var record = db.KPI_7S_ReferenceStandard.ToList();
            if (pointsType != null)
            {
                foreach (var item in pointsType)
                {
                    var list = record.Where(c => c.PointsType == item).ToList();
                    newList = newList.Concat(list).ToList();
                }
            }
            if (newList.Count > 0)
            {
                result.Add("Data", JsonConvert.SerializeObject(newList));
                return commom.GetModuleFromJobjet(result, true, "查询成功！");
            }
            else
            {
                result.Add("Data", JsonConvert.SerializeObject(newList));
                return commom.GetModuleFromJobjet(result, true, "暂无数据！");
            }
        }
        #endregion

        #region--检查参考标准修改
        [HttpPost]
        [ApiAuthorize]
        public JObject ReferenceStandard_modify([System.Web.Http.FromBody]JObject data)//int id, string pointsType, string referenceStandard
        {
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int id = int.Parse(data["id"].ToString());
            string pointsType = obj.pointsType;
            string referenceStandard = obj.referenceStandard;
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            if (id != 0 && !String.IsNullOrEmpty(pointsType) && !String.IsNullOrEmpty(referenceStandard))
            {
                var record = db.KPI_7S_ReferenceStandard.Where(c => c.Id == id).FirstOrDefault();
                record.PointsType = pointsType;
                record.ReferenceStandard = referenceStandard;
                record.ModifyPerson = auth.UserName;
                record.ModifyTime = DateTime.Now;
                int count = db.SaveChanges();
                if (count > 0)
                {
                    return commom.GetModuleFromJobjet(result, true, "修改成功！");
                }
                else
                {
                    return commom.GetModuleFromJobjet(result, false, "修改失败！");
                }
            }
            else
            {
                return commom.GetModuleFromJobjet(result, false, "传入参数为空！");
            }
        }
        #endregion

        #region--检查参考标准删除
        [HttpPost]
        [ApiAuthorize]
        public JObject ReferenceStandard_del([System.Web.Http.FromBody]JObject data)//int id  //原方法名：ReferenceStandard_delete
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            int id = int.Parse(data["id"].ToString());
            var removeList = db.KPI_7S_ReferenceStandard.Where(c => c.Id == id).ToList();
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;
            operaterecord.Operator = auth.UserName;
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除了7S扣分类型为：" + removeList.FirstOrDefault().PointsType + "的扣分参照标准项,标准项为：" + removeList.FirstOrDefault().ReferenceStandard + "记录Id为：" + removeList.FirstOrDefault().Id + ".";
            db.KPI_7S_ReferenceStandard.RemoveRange(removeList);
            db.UserOperateLog.Add(operaterecord);//保存删除日志
            int count = db.SaveChanges();
            if (count > 0)
            {
                return commom.GetModuleFromJobjet(result, true, "删除成功！");
            }
            else
            {
                return commom.GetModuleFromJobjet(result, false, "删除失败！");
            }
        }
        #endregion

        #endregion

        #region---区域数据

        #region---区域数据录入
        [HttpPost]
        [ApiAuthorize]
        public JObject Region_Input([System.Web.Http.FromBody]JObject data)//List<KPI_7S_DistrictPosition> record
        {
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            List<KPI_7S_DistrictPosition> record = (List<KPI_7S_DistrictPosition>)JsonHelper.jsonDes<List<KPI_7S_DistrictPosition>>(data["record"].ToString());
            JObject result = new JObject();
            if (record.Count > 0)
            {
                int savecout = 0;
                foreach (var item in record)
                {
                    //检验记录是否存在
                    if (db.KPI_7S_DistrictPosition.Count(c => c.Department == item.Department && c.Group == item.Group && c.Position == item.Position && c.District == item.District && c.VersionsTime.Value.Year == item.VersionsTime.Value.Year && c.VersionsTime.Value.Month == item.VersionsTime.Value.Month && c.VersionsTime.Value.Day == item.VersionsTime.Value.Day) < 1)
                    {
                        item.InputTime = DateTime.Now;
                        item.InputPerson = auth.UserName;
                        db.SaveChanges();
                        db.KPI_7S_DistrictPosition.Add(item);
                        savecout += db.SaveChanges();
                    }
                }
                if (savecout > 0) return commom.GetModuleFromJobjet(result, true, "保存成功！");
                else return commom.GetModuleFromJobjet(result, false, "保存失败！");
            }
            else return commom.GetModuleFromJobjet(result, false, "传入参数为空！");
        }
        #endregion

        #region---区域数据修改
        [HttpPost]
        [ApiAuthorize]
        public JObject Region_modify([System.Web.Http.FromBody]JObject data)//int id, string department, string position, int district, string group, double targetValue
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int id = int.Parse(data["id"].ToString());
            string department = obj.department;//部门
            string position = obj.position;//位置
            int district = int.Parse(data["district"].ToString());//区域号
            string group = obj.group;//班组
            double targetValue = Convert.ToDouble(data["targetValue"]);//目标值
            if (id != 0 && !String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district != 0)
            {
                var record = db.KPI_7S_DistrictPosition.Where(c => c.Id == id).FirstOrDefault();
                record.Department = department;//部门
                record.Position = position;//位置
                record.District = district;//区域号
                record.Group = group;//班组
                record.TargetValue = targetValue;//目标值
                record.ModifyPerson = auth.UserName;//修改人
                record.ModifyTime = DateTime.Now;//修改时间
                int count = db.SaveChanges();
                if (count > 0) return commom.GetModuleFromJobjet(result, true, "修改成功！");
                else return commom.GetModuleFromJobjet(result, false, "修改失败！");
            }
            else
            {
                return commom.GetModuleFromJobjet(result, false, "传入参数为空！");
            }
        }

        #endregion

        #region---区域数据删除
        [HttpPost]
        [ApiAuthorize]
        public JObject Region_del([System.Web.Http.FromBody]JObject data)//int id   原方法名：Region_delete
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            int id = int.Parse(data["id"].ToString());
            var removeList = db.KPI_7S_DistrictPosition.Where(c => c.Id == id).ToList();
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;
            operaterecord.Operator = auth.UserName;
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除了7S区域表中的：" + removeList.FirstOrDefault().Department + "，" + removeList.FirstOrDefault().Position + "位置的区域记录，区域号为：" + removeList.FirstOrDefault().District + "，记录Id为：" + removeList.FirstOrDefault().Id + ".";
            db.KPI_7S_DistrictPosition.RemoveRange(removeList);
            db.UserOperateLog.Add(operaterecord);//保存删除日志
            int count = db.SaveChanges();
            if (count > 0) return commom.GetModuleFromJobjet(result, true, "删除成功！");
            else return commom.GetModuleFromJobjet(result, false, "删除失败！");
        }
        #endregion

        #region---区域数据查询
        [HttpPost]
        [ApiAuthorize]
        public JObject Region_query([System.Web.Http.FromBody]JObject data)//string department, int? district, string position, DateTime versionsTime
        {
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string department = obj.department;//部门
            var quyuhao = (obj.district).ToString();
            int district = 0;
            if (!String.IsNullOrEmpty(quyuhao))
            {
                district = obj.district;
            }
            string position = data["position"].ToString();//位置
            DateTime versionsTime = Convert.ToDateTime(data["versionsTime"]);//目标值
            List<KPI_7S_DistrictPosition> recordList = new List<KPI_7S_DistrictPosition>();
            if (!String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == 0)
            {//整个部门
                recordList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.VersionsTime.Value.Year == versionsTime.Year && c.VersionsTime.Value.Month == versionsTime.Month && c.VersionsTime.Value.Day == versionsTime.Day).ToList();
            }
            else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district == 0)
            {//按部门、位置查
                recordList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.VersionsTime.Value.Year == versionsTime.Year && c.VersionsTime.Value.Month == versionsTime.Month && c.VersionsTime.Value.Day == versionsTime.Day).ToList();
            }
            else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district != 0)
            { //按部门、位置、区域号                                
                recordList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.District == district && c.VersionsTime.Value.Year == versionsTime.Year && c.VersionsTime.Value.Month == versionsTime.Month && c.VersionsTime.Value.Day == versionsTime.Day).ToList();
            }
            JArray result = new JArray();
            foreach (var item in recordList)
            {
                JObject record = new JObject();
                record.Add("Id", item.Id);
                record.Add("Department", item.Department);
                record.Add("Position", item.Position);
                record.Add("Group", item.Group);
                record.Add("District", item.District);
                record.Add("TargetValue", item.TargetValue);
                record.Add("VersionsTime", Convert.ToDateTime(item.VersionsTime).ToString("yyyy-MM-dd"));
                result.Add(record);
            }
            return commom.GetModuleFromJarray(result, true, "查询成功！");
        }
        #endregion

        #region---获取版本时间
        [HttpPost]
        [ApiAuthorize]
        public JObject GainVersions()//原方法名： GetVersions
        {
            var list = db.KPI_7S_DistrictPosition.Select(c => c.VersionsTime).Distinct().ToList();
            JArray result = new JArray();
            foreach (var item in list)
            {
                JObject record = new JObject();
                record.Add("value", Convert.ToDateTime(item).ToString("yyyy-MM-dd"));
                record.Add("label", Convert.ToDateTime(item).ToString("yyyy-MM-dd"));
                result.Add(record);
            }
            return commom.GetModuleFromJarray(result, true, "查询版本成功！");
        }
        #endregion

        #region--获取部门下拉列表
        [HttpPost]
        [ApiAuthorize]
        public JObject GainDepartmentList()//原方法名： GetDepartmentList
        {
            var departmentList = db.KPI_7S_DistrictPosition.Select(c => c.Department).Distinct().ToList();
            JArray result = new JArray();
            JObject record = new JObject();
            if (departmentList.Count > 0)
            {
                record.Add("value", "全部部门");
                record.Add("label", "全部部门");
                result.Add(record);
            }
            foreach (var item in departmentList)
            {
                JObject List = new JObject();
                List.Add("value", item);
                List.Add("label", item);
                result.Add(List);
            }
            return commom.GetModuleFromJarray(result, true, "查询成功！");
        }
        #endregion

        #region--根据部门获取区域号下拉列表
        [HttpPost]
        [ApiAuthorize]
        public JObject GainDistrictList([System.Web.Http.FromBody]JObject data)//string department, string position    //原方法名: GetDistrictList
        {
            string department = data["department"].ToString();//部门            
            string position = data["position"].ToString();//位置
            JArray result = new JArray();
            if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position))
            {
                var districtList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position).Select(c => c.District).Distinct().ToList();
                JObject record = new JObject();
                if (districtList.Count > 0)
                {
                    record.Add("value", "");
                    record.Add("label", "全部区域");
                    result.Add(record);
                }
                foreach (var item in districtList)
                {
                    JObject List = new JObject();
                    List.Add("value", item);
                    List.Add("label", item);
                    result.Add(List);
                }
                return commom.GetModuleFromJarray(result, true, "查询成功！");
            }
            else return commom.GetModuleFromJarray(result, false, "传入参数为空！");
        }
        #endregion

        #region--根据版本时间找出全部区域号
        [HttpPost]
        [ApiAuthorize]
        public JObject GainDistrict([System.Web.Http.FromBody]JObject data)//DateTime date //原方法名：GetDistrict
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            JArray result = new JArray();
            if (obj.date != null && obj.date != "")
            {
                DateTime date = Convert.ToDateTime(data["date"]);//时间
                var versionTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= date).Select(c => c.VersionsTime).Distinct().Max();
                var record = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == versionTime).Select(c => c.District).Distinct().ToList();
                foreach (var item in record)
                {
                    JObject res = new JObject();
                    res.Add("value", item);
                    res.Add("label", item);
                    result.Add(res);
                }
                return commom.GetModuleFromJarray(result, true, "查询成功！");
            }
            else
            {
                return commom.GetModuleFromJarray(result, false, "时间为空！");
            }
        }
        #endregion

        #region--根据区域号找位置、部门
        [HttpPost]
        [ApiAuthorize]
        public JObject GainPosition([System.Web.Http.FromBody]JObject data)//int district, DateTime date    原方法名：GetPosition
        {
            int district = int.Parse(data["district"].ToString());//区域号
            DateTime date = Convert.ToDateTime(data["date"]);
            var versionTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= date).Select(c => c.VersionsTime).Distinct().Max();
            var list = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == versionTime).ToList();
            //找位置
            JObject result = new JObject();
            JArray positionList = new JArray();
            var position = list.Where(c => c.District == district).ToList();
            foreach (var i in position)
            {
                JObject res = new JObject();
                res.Add("value", i.Position);
                res.Add("label", i.Position);
                positionList.Add(res);
            }
            result.Add("positionList", positionList);
            //找部门
            JArray depList = new JArray();
            var dep = list.Where(c => c.District == district).Select(c => c.Department).Distinct().ToList();
            foreach (var item in dep)
            {
                JObject res = new JObject();
                res.Add("value", item);
                res.Add("label", item);
                depList.Add(res);
            }
            result.Add("depList", depList);
            return commom.GetModuleFromJobjet(result, false, "查询成功！");
        }
        #endregion
        #endregion

        #region---日检、周检、巡检数据录入

        #region---临时类      
        public class selectOption
        {
            public string Check_Type { get; set; }//检查类型
            public DateTime Date { get; set; }//检查时间
            public string Department { get; set; }//部门
            public string Position { get; set; }//位置
            public int District { get; set; } //区域号
            //public string ResponsiblePerson { get; set; }//责任人
            public string Check_Person { get; set; }//检查人
        }
        #endregion

        #region---上传记录前检验是否已有对应的记录录入，有前端就数据提示用户
        [HttpPost]
        [ApiAuthorize]
        public JObject Check_Record([System.Web.Http.FromBody]JObject data)//原方法参数：List<KPI_7S_Record> record, selectOption formInfo
        {
            List<KPI_7S_Record> record = (List<KPI_7S_Record>)JsonHelper.jsonDes<List<KPI_7S_Record>>(data["record"].ToString());//表中记录
            selectOption formInfo = JsonConvert.DeserializeObject<selectOption>(JsonConvert.SerializeObject(data["formInfo"]));
            JArray check = new JArray();
            var res = db.KPI_7S_Record.Where(c => c.Year == formInfo.Date.Year && c.Month == formInfo.Date.Month).ToList();
            foreach (var j in record)
            {
                if (res.Where(c => c.Department == formInfo.Department && c.Position == formInfo.Position && c.District == formInfo.District && c.Check_Type == formInfo.Check_Type && c.Date.Year == formInfo.Date.Year && c.Date.Month == formInfo.Date.Month && c.Date.Day == formInfo.Date.Day && c.PointsDeducted_Type == j.PointsDeducted_Type).Count() > 0)
                {
                    check.Add(j.PointsDeducted_Type);
                }
            }
            return commom.GetModuleFromJarray(check, true, "查询成功！");
        }
        #endregion

        #region---扣分记录上传       
        [HttpPost]
        [ApiAuthorize]
        public JObject RecordInput([System.Web.Http.FromBody]JObject data)//原方法参数：List<KPI_7S_Record> record, selectOption formInfo
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            List<KPI_7S_Record> record = (List<KPI_7S_Record>)JsonHelper.jsonDes<List<KPI_7S_Record>>(data["record"].ToString());//表中记录
            selectOption formInfo = JsonConvert.DeserializeObject<selectOption>(JsonConvert.SerializeObject(data["formInfo"]));
            if (record.Count > 0 && formInfo != null)
            {
                JArray weekday = new JArray();
                weekday = getWeekNumInMonth(formInfo.Date);//获取是第几周
                int savecout = 0;
                var list = db.KPI_7S_Record.Where(c => c.Year == formInfo.Date.Year && c.Month == formInfo.Date.Month && c.Check_Type != "日检").ToList();//找出当月除了日检的记录，方面下面检查记录是否有重复出现的问题
                foreach (var item in record)
                {
                    item.Department = formInfo.Department;//部门
                    item.Group = formInfo.Position;//班组
                    item.Position = formInfo.Position;//位置
                    item.District = formInfo.District;//区域号
                    item.Date = formInfo.Date;//检查日期
                    item.Check_Type = formInfo.Check_Type;//检查类型
                    item.Week = Convert.ToInt32(weekday[1]);//第几周
                    item.Year = formInfo.Date.Year;//年
                    item.Month = formInfo.Date.Month;//月                    
                    item.InputPerson = auth.UserName;//录入人
                    item.InputTime = DateTime.Now;//录入时间 
                    item.Check_Person = formInfo.Check_Person;//检查人
                    if (formInfo.Check_Type != "日检")
                    {
                        //检验是否有重复出现的问题
                        if (list.Where(c => c.Department == formInfo.Department && c.Position == formInfo.Position && c.District == formInfo.District && c.PointsDeducted_Type == item.PointsDeducted_Type && c.PointsDeducted_Item == item.PointsDeducted_Item && c.ProblemDescription == item.ProblemDescription).Count() > 0)
                        {
                            item.RepetitionPointsDeducted = 2;//重复出现扣两分
                        }
                    }
                    db.SaveChanges();
                    db.KPI_7S_Record.Add(item);
                    savecout += db.SaveChanges();
                }
                if (savecout > 0) return commom.GetModuleFromJobjet(result, true, "保存成功！");
                else return commom.GetModuleFromJobjet(result, false, "保存失败！");
            }
            else return commom.GetModuleFromJobjet(result, false, "传入数据为空！");
        }
        #endregion

        #endregion

        #region---日检、周检、巡检图片上传
        [HttpPost]
        /// <param name="department">部门</param>
        /// <param name="position">位置</param>
        /// <param name="check_date">检查日期</param>
        /// <param name="check_Type">检查类型（日检、周检、抽检）</param>
        /// <param name="pointsDeducted_Type">7S扣分类型（整理、整顿、清洁、清扫、安全、节约、素养）</param>
        /// <param name="uploadType">上传类型（改善前、改善后）</param>
        /// <returns></returns>   
        public bool ImageUpload()//原方法参数：List<string> pictureFile, string department, string position, DateTime check_date, string check_Type, string pointsDeducted_Type, string uploadType, int district
        {
            string department = HttpContext.Current.Request["department"];//部门
            string position = HttpContext.Current.Request["position"];//位置
            DateTime check_date = Convert.ToDateTime(HttpContext.Current.Request["check_date"]);//检查日期
            string check_Type = HttpContext.Current.Request["check_Type"];//检查类型
            string pointsDeducted_Type = HttpContext.Current.Request["pointsDeducted_Type"];//扣分标准项
            string uploadType = HttpContext.Current.Request["uploadType"];//上传类型
            int district = int.Parse(HttpContext.Current.Request["district"]);//区域号
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//上传的多张图片  这个“MS_HttpContext”参数名不需要改           
            HttpRequestBase requests = context.Request;

            string datatime = check_date.Year.ToString() + "." + check_date.Month.ToString() + "." + check_date.Day.ToString();
            if (requests.Files.Count > 0 && !String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && !String.IsNullOrEmpty(check_Type) && !String.IsNullOrEmpty(pointsDeducted_Type) && !String.IsNullOrEmpty(uploadType))
            {
                if (Directory.Exists(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\") == false)//判断总路径是否存在
                {
                    Directory.CreateDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\");//创建总路径
                };

                if (Directory.Exists(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\") == false)//判断图片路径是否存在
                {
                    Directory.CreateDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\");//创建图片路径
                }
                for (int i = 0; i < requests.Files.Count; i++)
                {
                    var file = requests.Files[i];
                    var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();
                    List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\");//遍历文件夹中的个数
                    if (fileType == ".jpg")//判断文件后缀
                    {
                        int jpg_count = filesInfo.Where(c => c.Name.StartsWith(uploadType + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                        file.SaveAs(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + (jpg_count + 1) + fileType);//文件追加命名
                    }
                    else if (fileType == ".png")
                    {
                        int pdf_count = filesInfo.Where(c => c.Name.StartsWith(uploadType + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".png").Count();
                        file.SaveAs(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + (pdf_count + 1) + fileType);//文件追加命名
                    }
                    else if (fileType == "jpeg")
                    {
                        int pdf_count = filesInfo.Where(c => c.Name.StartsWith(uploadType + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                        file.SaveAs(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + (pdf_count + 1) + ".jpg");//文件追加命名
                    }
                }
                return true;
            }
            return false;
        }
        [HttpPost]
        [ApiAuthorize]
        public JObject LookImage([System.Web.Http.FromBody]JObject data)
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string department = obj.department;
            string position = obj.position;
            DateTime check_date = Convert.ToDateTime(obj.check_date);
            string check_Type = obj.check_Type;
            string pointsDeducted_Type = obj.pointsDeducted_Type;
            string uploadType = obj.uploadType;
            int district = Convert.ToInt32(obj.district);
            return commom.GetModuleFromJarray(QueryImage(department, position, check_date, check_Type, pointsDeducted_Type, uploadType, district), true, "查询成功！");
        }
        public JArray QueryImage(string department, string position, DateTime check_date, string check_Type, string pointsDeducted_Type, string uploadType, int district)
        {
            string datatime = check_date.Year.ToString() + "." + check_date.Month.ToString() + "." + check_date.Day.ToString();
            JObject result = new JObject();
            JObject pictureAddress_list = new JObject();
            JObject drawFiles_list = new JObject();
            List<FileInfo> draw_files = null;
            if (Directory.Exists(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\"))//判断图纸的总路径是否存在
            {
                draw_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\");//遍历图纸文件夹的文件个数
            }
            if (draw_files != null)
            {
                JArray drawjpg_list = new JArray();//用于存放后缀为.jpg图纸的数组                   
                foreach (var i in draw_files)
                {
                    string path1 = @"/MES_Data/7S/" + check_date.Year.ToString() + "/" + check_date.Month.ToString() + "/" + check_Type + "/" + datatime + "/" + department + "/" + position + "/" + district + "/" + pointsDeducted_Type + "/" + uploadType + "/" + i;//组合出路径                                       
                    drawjpg_list.Add(path1);
                }
                return drawjpg_list;
            }
            else
            {
                JArray drawjpg_list = new JArray();//用于存放后缀为.jpg图纸的数组    
                return drawjpg_list;
            }
        }
        #endregion

        #region--- 扣分记录录入页面 获取录入数据
        [HttpPost]
        [ApiAuthorize]
        public JObject GainInputData([System.Web.Http.FromBody]JObject data)//DateTime? time  //原方法名GetInputData
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            DateTime? time = obj.time == null ? DateTime.Now : Convert.ToDateTime(data["time"].ToString());
            JArray recordList1 = new JArray();
            var list = db.KPI_7S_ReferenceStandard.OrderByDescending(c => c.PointsType).ToList();
            var list1 = list.Select(c => c.PointsType).Distinct().ToList();
            foreach (var item in list1)
            {
                JObject record = new JObject();
                JArray selectoption = new JArray();
                JArray option = new JArray();
                var list2 = list.Where(c => c.PointsType == item).ToList();
                foreach (var i in list2)
                {
                    JObject res = new JObject();
                    res.Add("value", i.ReferenceStandard);
                    res.Add("count", 0);
                    selectoption.Add(res);
                }
                record.Add("PointsDeducted_Type", item);
                record.Add("PointsDeducted_Item", option);
                record.Add("selectoption", selectoption);
                record.Add("ProblemDescription", "");
                record.Add("PointsDeducted", "");//扣分
                record.Add("BeforeImprovement", "");//改善前
                record.Add("AfterImprovement", "");//改善后
                record.Add("RectificationTime", GetTime(Convert.ToDateTime(time)));//限期整改时间
                record.Add("Rectification_Confim", "");//整改结果
                recordList1.Add(record);
            }
            return commom.GetModuleFromJarray(recordList1, true, "查询成功！");
        }

        public JObject GainTime([System.Web.Http.FromBody]JObject data)
        {
            DateTime time = Convert.ToDateTime(data["date"].ToString());
            JObject result = new JObject();
            result.Add("time", GetTime(time));
            return commom.GetModuleFromJobjet(result);
        }
        #region---根据当前日期 获取下周四日期
        public string GetTime(DateTime date)//原方法名：GetTime
        {
            DateTime time = date;
            switch ((int)date.DayOfWeek)
            {
                case 0:
                    time = date.AddDays(4);
                    break;
                case 1:
                    time = date.AddDays(10);
                    break;
                case 2:
                    time = date.AddDays(9);
                    break;
                case 3:
                    time = date.AddDays(8);
                    break;
                case 4:
                    time = date.AddDays(7);
                    break;
                case 5:
                    time = date.AddDays(6);
                    break;
                default:
                    time = date.AddDays(5);
                    break;
            }
            return time.ToString("yyyy-MM-dd");
        }
        #endregion
        #endregion

        #region--- 删除照片法
        [HttpPost]
        [ApiAuthorize]
        public string DelImg([System.Web.Http.FromBody]JObject data)//path(路径) //原方法名：DeleteImg   //原方法参数：string path, string department, string position, DateTime check_date, string check_Type, string pointsDeducted_Type, string uploadType, int district
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string path = obj.path;
            string department = obj.department;
            string position = obj.position;
            string check_Type = obj.check_Type;
            DateTime check_date = Convert.ToDateTime(data["check_date"]);//目标值
            string pointsDeducted_Type = obj.pointsDeducted_Type;
            string uploadType = obj.uploadType;
            int district = data["district"] == null ? 0 : int.Parse(data["district"].ToString());
            string datatime = check_date.Year.ToString() + "." + check_date.Month.ToString() + "." + check_date.Day.ToString();
            JObject result = new JObject();
            if (!String.IsNullOrEmpty(path))
            {
                string fileType = path.Substring(path.Length - 4, 4);//扩展名
                string old_path = @"D:" + path.Replace('/', '\\');//整个文件路径
                FileInfo path_file = new FileInfo(old_path);//建立文件对象
                int serial_N = Convert.ToInt16(path_file.Name.Split('_')[1].Split('.')[0]);//文件序号
                string new_path = @"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "_Delete_File\\";//新目录路径
                if (Directory.Exists(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "_Delete_File\\") == false)　//目录是否存在
                {

                    Directory.CreateDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "_Delete_File\\");
                    FileInfo new_file = new FileInfo(new_path + uploadType + "_1" + fileType);
                    path_file.CopyTo(new_file.FullName);//复制文件到新目录
                    path_file.Delete();//删除原文件
                    List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\");
                    int filecount = filesInfo.Where(c => c.Extension == fileType).Count();//文件夹里的文件个数
                    for (int i = serial_N; i < filecount + 1; i++)
                    {
                        string filepath = @"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + (i + 1) + fileType;
                        string newfilepath = @"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + i + fileType;
                        System.IO.File.Move(filepath, newfilepath);
                    }
                }
                else  //已有删除目录时，修改文件名
                {
                    List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "_Delete_File\\");
                    int count = filesInfo.Where(c => c.Extension == fileType).Count();
                    FileInfo new_file = new FileInfo(new_path + uploadType + "_" + (count + 1) + fileType);
                    path_file.CopyTo(new_file.FullName);//复制文件到新目录
                    path_file.Delete();//删除原文件
                    List<FileInfo> picturefilesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\");
                    int filecount = picturefilesInfo.Where(c => c.Extension == fileType).Count();//文件夹里的文件个数
                    for (int i = serial_N; i < filecount + 1; i++)
                    {
                        string filepath = @"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + (i + 1) + fileType;
                        string newfilepath = @"D:\MES_Data\7S\" + check_date.Year.ToString() + "\\" + check_date.Month.ToString() + "\\" + check_Type + "\\" + datatime + "\\" + department + "\\" + position + "\\" + district + "\\" + pointsDeducted_Type + "\\" + uploadType + "\\" + uploadType + "_" + i + fileType;
                        System.IO.File.Move(filepath, newfilepath);
                    }
                }
                return "删除成功";
            }
            else
            {
                return "没有路径";
            }
        }
        #endregion

        #region--根据部门、班组获取位置下拉列表
        [HttpPost]
        [ApiAuthorize]
        public JObject GainPositionList([System.Web.Http.FromBody]JObject data)//原方法名：GetPositionList  //原方法参数：string department
        {
            string department = data["department"].ToString();
            JArray result = new JArray();
            JObject record = new JObject();
            if (!String.IsNullOrEmpty(department))
            {
                var positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department).Select(c => c.Position).Distinct().ToList();
                if (positionList.Count > 0)
                {
                    record.Add("value", "");
                    record.Add("label", "全部位置");
                    result.Add(record);
                }
                foreach (var item in positionList)
                {
                    JObject List = new JObject();
                    List.Add("value", item);
                    List.Add("label", item);
                    result.Add(List);
                }
                return commom.GetModuleFromJarray(result, true, "查询成功！");
            }
            else return commom.GetModuleFromJarray(result, false, "传入数据为空！");
        }
        #endregion

        #region--根据部门、班组获取位置下拉列表
        [HttpPost]
        [ApiAuthorize]
        public JObject PositionSelect([System.Web.Http.FromBody]JObject data)//原方法参数：string department, string group
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string department = obj.department;
            string group = obj.group;
            JArray result = new JArray();
            JObject record = new JObject();
            if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(group))
            {
                var positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Group == group).Select(c => c.Position).Distinct().ToList();
                if (positionList.Count > 0)
                {
                    record.Add("value", "");
                    record.Add("label", "全部位置");
                    result.Add(record);
                }
                foreach (var item in positionList)
                {
                    JObject List = new JObject();
                    List.Add("value", item);
                    List.Add("label", item);
                    result.Add(List);
                }
                return commom.GetModuleFromJarray(result, true, "查询成功！");
            }
            else return commom.GetModuleFromJarray(result, false, "传入数据为空！");
        }
        #endregion

        #region--根据部门获取班组下拉列表
        [HttpPost]
        [ApiAuthorize]
        public JObject GroupList([System.Web.Http.FromBody]JObject data)//string department
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string department = obj.department;//部门
            JArray result = new JArray();
            JObject record = new JObject();
            if (!String.IsNullOrEmpty(department))
            {
                var groupList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department).Select(c => c.Group).Distinct().ToList();
                if (groupList.Count > 0)
                {
                    record.Add("value", "");
                    record.Add("label", "全部班组");
                    result.Add(record);
                }
                foreach (var item in groupList)
                {
                    JObject List = new JObject();
                    List.Add("value", item);
                    List.Add("label", item);
                    result.Add(List);
                }
                return commom.GetModuleFromJarray(result, true, "查询成功");
            }
            else return commom.GetModuleFromJarray(result, true, "传入数据为空！");
        }
        #endregion

        #region--日检、周检、巡检详细页按天查询
        [HttpPost]
        [ApiAuthorize]
        ///department  部门
        ///position    班组
        ///date        检查日期
        ///check_Type  检查类型（日检、周检、巡检）
        ///week        第几周（在周报汇总表点击进去详细页时需要传第几周）在详细页查询时，选择检查类型为周检时可以不传
        ///district    区域号（从日报汇总表，周报、巡检汇总表跳进来需传区域号，在详细页查询则可不传)
        public JObject Detail_Query([System.Web.Http.FromBody]JObject data)//原方法参数：string department, string position, DateTime date, string check_Type, int? week, int? district
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string department = obj.department;
            string position = obj.position;
            DateTime date = Convert.ToDateTime(data["date"]);
            string check_Type = obj.check_Type;
            var num = (obj.week).ToString();
            int week = 0;
            if (!String.IsNullOrEmpty(num)) {
                week = obj.week;
            }
            //int? week = data["week"] == null ? 0 : int.Parse(data["week"].ToString());
            var quyuhao = (obj.district).ToString();
            int district = 0;
            if (!String.IsNullOrEmpty(quyuhao))
            {
                district = obj.district;
            }
            JArray result = new JArray();
            List<KPI_7S_Record> list = new List<KPI_7S_Record>();
            if (check_Type != "周检")
            {
                if (!String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == 0)
                {//整个部门
                    list = db.KPI_7S_Record.Where(c => c.Department == department && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == date.Day).ToList();
                }
                else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district == 0)
                {//按部门、位置查
                    list = db.KPI_7S_Record.Where(c => c.Department == department && c.Position == position && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == date.Day).ToList();
                }
                else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district != 0)
                { //按部门、位置、区域号                                
                    list = db.KPI_7S_Record.Where(c => c.Department == department && c.Position == position && c.District == district && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == date.Day).ToList();
                }
            }
            else
            {//等于周检
                JArray weekday = new JArray();
                if (week == 0)
                {
                    weekday = getWeekNumInMonth(date);
                }
                if (!String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == 0)
                {//按部门查
                    int Weekly = 0;
                    if (week == 0) Weekly = Convert.ToInt32(weekday[1]);
                    else Weekly = Convert.ToInt32(week);
                    list = db.KPI_7S_Record.Where(c => c.Department == department && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Week == Weekly).ToList();
                }
                else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district == 0)
                {
                    //按部门查
                    int Weekly = 0;
                    if (week == 0) Weekly = Convert.ToInt32(weekday[1]);
                    else Weekly = Convert.ToInt32(week);
                    list = db.KPI_7S_Record.Where(c => c.Department == department && c.Position == position && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Week == Weekly).ToList();
                }
                else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district != 0)
                {
                    int Weekly = 0;
                    if (week == 0) Weekly = Convert.ToInt32(weekday[1]);
                    else Weekly = Convert.ToInt32(week);
                    list = db.KPI_7S_Record.Where(c => c.Department == department && c.Position == position && c.District == district && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Week == Weekly).ToList();
                }
            }
            foreach (var item in list)
            {
                JObject record = new JObject();
                record.Add("Id", item.Id);//ID
                record.Add("Date", item.Date.ToString("yyyy-MM-dd"));//检查日期
                record.Add("Department", item.Department);//部门
                record.Add("Group", item.Position);//班组
                record.Add("District", item.District); //区域号
                                                       // record.Add("ResponsiblePerson", item.ResponsiblePerson);//责任人
                record.Add("PointsDeducted_Type", item.PointsDeducted_Type);//扣分类型
                JArray PD_Item = new JArray();
                if (item.Remark != "未交表")
                {
                    var pdItem = item.PointsDeducted_Item.Split('|');
                    foreach (var i in pdItem)
                    {
                        PD_Item.Add(i);
                    }
                }
                record.Add("PointsDeducted_Item", PD_Item);//扣分参考标准
                record.Add("ProblemDescription", item.ProblemDescription);//问题描述
                record.Add("PointsDeducted", item.PointsDeducted.ToString("0.##"));//7S扣分
                if (check_Type != "日检")
                {
                    record.Add("RectificationPoints", item.RectificationPoints == 0 ? " " : item.RectificationPoints.ToString("0.##"));//限期未整改扣分
                    record.Add("RepetitionPointsDeducted", item.RepetitionPointsDeducted == 0 ? " " : item.RepetitionPointsDeducted.ToString("0.##"));//重复出现扣分
                }
                if (item.PointsDeducted_Type != null)//扣分类型不等于空的 是不扣分的记录，不扣分的记录有图片上传
                {
                    JArray beforeImprovementImage = new JArray();
                    beforeImprovementImage = QueryImage(item.Department, item.Position, item.Date, check_Type, item.PointsDeducted_Type, "改善前", item.District);
                    record.Add("BeforeImprovement", beforeImprovementImage);//改善前
                    JArray afterImprovementImage = new JArray();
                    afterImprovementImage = QueryImage(item.Department, item.Position, item.Date, check_Type, item.PointsDeducted_Type, "改善后", item.District);
                    record.Add("AfterImprovement", afterImprovementImage);//改善后
                }
                else
                {
                    JArray beforeImprovementImage = new JArray();
                    record.Add("BeforeImprovement", beforeImprovementImage);//改善前
                    JArray afterImprovementImage = new JArray();
                    record.Add("AfterImprovement", afterImprovementImage);//改善后
                }
                record.Add("RectificationTime", item.RectificationTime == null ? null : item.RectificationTime?.ToString("yyyy-MM-dd"));//限期整改时间
                record.Add("Rectification_Confim", item.Rectification_Confim);//整改结果
                result.Add(record);
            }
            return commom.GetModuleFromJarray(result, true, "查询成功！");
        }
        #endregion

        #region--日检、周检、巡检详细页按月查询
        [HttpPost]
        [ApiAuthorize]
        ///department  部门
        ///position    班组
        ///date        检查日期
        ///check_Type  检查类型（日检、周检、巡检）       
        ///district    区域号(如果是在详细页查询，不需要传区域号)
        public JObject Month_Query([System.Web.Http.FromBody]JObject data)//原方法参数：string department, string position, DateTime date, string check_Type, int? district
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string department = obj.department;
            string position = obj.position;
            DateTime date = Convert.ToDateTime(data["date"].ToString());
            string check_Type = obj.check_Type;
            //int? district = data["district"] == null ? 0 : int.Parse(data["district"].ToString());
            var quyuhao = (obj.district).ToString();
            int district = 0;
            if (!String.IsNullOrEmpty(quyuhao))
            {
                district = obj.district;
            }
            JArray result = new JArray();
            List<KPI_7S_Record> list = new List<KPI_7S_Record>();
            if (!String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == 0)
            {//整个部门
                list = db.KPI_7S_Record.Where(c => c.Department == department && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Month == date.Month).ToList();
            }
            else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district == 0)
            {//按部门、位置查
                list = db.KPI_7S_Record.Where(c => c.Department == department && c.Position == position && c.Check_Type == check_Type && c.Date.Year == date.Year && c.Month == date.Month).ToList();
            }
            else if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district != 0)
            { //按部门、位置、区域号
                list = db.KPI_7S_Record.Where(c => c.Department == department && c.Position == position && c.Check_Type == check_Type && c.District == district && c.Date.Year == date.Year && c.Month == date.Month).ToList();
            }
            foreach (var item in list)
            {
                JObject record = new JObject();
                record.Add("Id", item.Id);//ID
                record.Add("Date", item.Date.ToString("yyyy-MM-dd"));//检查日期
                record.Add("Department", item.Department);//部门
                record.Add("Group", item.Position);//班组
                record.Add("District", item.District); //区域号
                //record.Add("ResponsiblePerson", item.ResponsiblePerson);//责任人
                record.Add("PointsDeducted_Type", item.PointsDeducted_Type);//扣分类型
                JArray PD_Item = new JArray();
                if (item.PointsDeducted_Item != null)
                {
                    var pdItem = item.PointsDeducted_Item.Split('|');
                    foreach (var i in pdItem)
                    {
                        PD_Item.Add(i);
                    }
                }
                record.Add("PointsDeducted_Item", PD_Item);//扣分参考标准
                record.Add("ProblemDescription", item.ProblemDescription);//问题描述
                record.Add("PointsDeducted", item.PointsDeducted.ToString("0.##"));//7S扣分
                if (check_Type != "日检")
                {
                    record.Add("RectificationPoints", item.RectificationPoints == 0 ? " " : item.RectificationPoints.ToString("0.##"));//限期未整改扣分
                    record.Add("RepetitionPointsDeducted", item.RepetitionPointsDeducted == 0 ? " " : item.RepetitionPointsDeducted.ToString("0.##"));//重复出现扣分
                }
                if (item.PointsDeducted_Type != null)//扣分类型不等于空的 是不扣分的记录，不扣分的记录有图片上传
                {
                    JArray beforeImprovementImage = new JArray();
                    beforeImprovementImage = QueryImage(item.Department, item.Position, item.Date, check_Type, item.PointsDeducted_Type, "改善前", item.District);
                    record.Add("BeforeImprovement", beforeImprovementImage);//改善前
                    JArray afterImprovementImage = new JArray();
                    afterImprovementImage = QueryImage(item.Department, item.Position, item.Date, check_Type, item.PointsDeducted_Type, "改善后", item.District);
                    record.Add("AfterImprovement", afterImprovementImage);//改善后
                }
                else
                {
                    JArray beforeImprovementImage = new JArray();
                    record.Add("BeforeImprovement", beforeImprovementImage);//改善前
                    JArray afterImprovementImage = new JArray();
                    record.Add("AfterImprovement", afterImprovementImage);//改善后
                }
                record.Add("RectificationTime", item.RectificationTime == null ? null : item.RectificationTime?.ToString("yyyy-MM-dd"));//限期整改时间
                record.Add("Rectification_Confim", item.Rectification_Confim);//整改结果
                result.Add(record);
            }
            return commom.GetModuleFromJarray(result, true, "查询成功！");
        }
        #endregion

        #region---详细页删除方法
        [HttpPost]
        [ApiAuthorize]
        public JObject KPI_7S_Del([System.Web.Http.FromBody]JObject data)//原方法参数：int id  //原方法名：KPI_7S_Delete
        {
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            int id = int.Parse(data["id"].ToString());
            JObject result = new JObject();
            if (id != 0)
            {
                var list = db.KPI_7S_Record.Where(c => c.Id == id).ToList();
                var record = list.FirstOrDefault();
                if (record.Remark != "未交表")
                {
                    //1.查找出改善前、改善后的图片   string department, string position, DateTime check_date, string check_Type, string pointsDeducted_Type, string uploadType, int district
                    JArray beforeImprovementImage = new JArray();
                    beforeImprovementImage = QueryImage(record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善前", record.District);
                    JArray afterImprovementImage = new JArray();
                    afterImprovementImage = QueryImage(record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善后", record.District);
                    if (beforeImprovementImage.Count > 0 && beforeImprovementImage.Count < 2)
                    {
                        foreach (var item in beforeImprovementImage)
                        {
                            DelImg(toJobject(item.ToString(), record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善前", record.District));
                            //DelImg(item.ToString(), record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善前", record.District);
                        }
                    }
                    else
                    {
                        foreach (var item in beforeImprovementImage)
                        {
                            beforeImprovementImage = QueryImage(record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善前", record.District);
                            DelImg(toJobject(beforeImprovementImage[0].ToString(), record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善前", record.District));
                            //DeleteImg(beforeImprovementImage[0].ToString(), record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善前", record.District);
                        }
                    }
                    if (afterImprovementImage.Count > 0 && afterImprovementImage.Count < 2)
                    {
                        foreach (var item in afterImprovementImage)
                        {
                            DelImg(toJobject(item.ToString(), record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善后", record.District));
                            //DeleteImg(item.ToString(), record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善后", record.District);
                        }
                    }
                    else
                    {
                        foreach (var item in afterImprovementImage)
                        {
                            afterImprovementImage = QueryImage(record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善后", record.District);
                            DelImg(toJobject(afterImprovementImage[0].ToString(), record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善后", record.District));
                            //DeleteImg(afterImprovementImage[0].ToString(), record.Department, record.Position, record.Date, record.Check_Type, record.PointsDeducted_Type, "改善后", record.District);
                        }
                    }
                    UserOperateLog operaterecord = new UserOperateLog();
                    operaterecord.OperateDT = DateTime.Now;
                    operaterecord.Operator = auth.UserName;
                    operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除了7S记录表的扣分记录,类型为：" + record.Check_Type + ",扣分类型为：" + record.PointsDeducted_Type + "扣分值为：" + record.PointsDeducted + "，记录Id为：" + record.Id + ".";
                    db.KPI_7S_Record.RemoveRange(list);
                    db.UserOperateLog.Add(operaterecord);//保存删除日志
                    int count = db.SaveChanges();
                    if (count > 0) return commom.GetModuleFromJobjet(result, true, "删除成功！");
                    else return commom.GetModuleFromJobjet(result, false, "删除失败！");
                }
                else
                {
                    UserOperateLog operaterecord = new UserOperateLog();
                    operaterecord.OperateDT = DateTime.Now;
                    operaterecord.Operator = auth.UserName;
                    operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除了7S记录表的扣分记录,类型为：" + record.Check_Type + ",扣分类型为：" + record.PointsDeducted_Type + "扣分值为：" + record.PointsDeducted + "，记录Id为：" + record.Id + ".";
                    db.KPI_7S_Record.RemoveRange(list);
                    db.UserOperateLog.Add(operaterecord);//保存删除日志
                    int count = db.SaveChanges();
                    if (count > 0) return commom.GetModuleFromJobjet(result, true, "删除成功！");
                    else return commom.GetModuleFromJobjet(result, false, "删除失败！");
                }
            }
            return commom.GetModuleFromJobjet(result, false, "传入参数为空！");
        }

        public JObject toJobject(string path, string department, string position, DateTime check_date, string check_Type, string pointsDeducted_Type, string uploadType, int district)
        {
            JObject parameter = new JObject();
            parameter.Add("path", path);
            parameter.Add("department", department);
            parameter.Add("position", position);
            parameter.Add("check_date", check_date);
            parameter.Add("check_Type", check_Type);
            parameter.Add("pointsDeducted_Type", pointsDeducted_Type);
            parameter.Add("uploadType", uploadType);
            parameter.Add("district", district);
            return parameter;
        }
        #endregion

        #region--审核限期整改记录
        [HttpPost]
        [ApiAuthorize]
        public JObject Approve([System.Web.Http.FromBody]JObject data)//原方法参数：int id, bool result
        {
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            int id = int.Parse(data["id"].ToString());
            bool result = bool.Parse(data["result"].ToString());
            JObject result1 = new JObject();
            var record = db.KPI_7S_Record.FirstOrDefault(c => c.Id == id);
            record.Rectification_Confim = result;//整改结果
            record.RectificationPerson = auth.UserName;//整改人
            record.ModifyTime = DateTime.Now;//整改时间
            if (result == false)
            {
                record.RectificationPoints = record.PointsDeducted;//整改不通过扣同样的分
            }
            int count = db.SaveChanges();
            if (count > 0) return commom.GetModuleFromJobjet(result1, true, "审核成功");
            else return commom.GetModuleFromJobjet(result1, false, "审核失败");
        }
        #endregion

        #region--日检、巡检汇总表
        [HttpPost]
        [ApiAuthorize]
        public JObject Daily_SumQuery([System.Web.Http.FromBody]JObject data)//原方法参数：string department, DateTime date, string position, int? district, string check_Type
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string department = obj.department;//部门
            DateTime date = Convert.ToDateTime(data["date"]);
            string position = obj.position;//位置
            //int? district = data["district"] == null ? 0 : int.Parse(data["district"].ToString());
            var quyuhao = (obj.district).ToString();
            int district = 0;
            if (!String.IsNullOrEmpty(quyuhao))
            {
                district = obj.district;
            }
            string check_Type = obj.check_Type;//检查类型
            JArray result = new JArray();
            if (!String.IsNullOrEmpty(department))
            {
                DateTime dt = date;
                int sumday = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1).Day;//一个月有多少天                               
                JArray arr = new JArray();
                List<KPI_7S_DistrictPosition> positionList = new List<KPI_7S_DistrictPosition>();
                var depTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= date).Select(c => c.VersionsTime).Distinct().ToList().Max();//获取版本时间
                if (department == "全部部门" && String.IsNullOrEmpty(position) && district == 0)//全部部门
                {
                    positionList = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == depTime).ToList();
                }
                else if (department != "全部部门" && !String.IsNullOrEmpty(position) && district == 0)//部门、位置
                {
                    positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.VersionsTime == depTime).ToList();
                }
                else if (department != "全部部门" && !String.IsNullOrEmpty(position) && district != 0)//部门、位置、区域
                {
                    positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.District == district && c.VersionsTime == depTime).ToList();
                }
                else if (department != "全部部门" && !String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == 0)//单个部门
                {
                    positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.VersionsTime == depTime).ToList();
                }
                List<KPI_7S_Record> recordList = new List<KPI_7S_Record>();

                if (check_Type == "日检")
                {
                    recordList = db.KPI_7S_Record.Where(c => c.Date.Year == date.Year && c.Date.Month == date.Month && c.Check_Type == "日检").ToList();//找出所有部门整个月的数据
                }
                else
                {
                    recordList = db.KPI_7S_Record.Where(c => c.Date.Year == date.Year && c.Date.Month == date.Month && c.Check_Type == "巡检").ToList();//找出所有部门整个月的数据
                }
                foreach (var item in positionList)
                {
                    JArray dateList = new JArray();
                    JObject list = new JObject();
                    list.Add("Department", item.Department);//部门
                    list.Add("Position", item.Position);//位置
                    list.Add("District", item.District);//区域
                    decimal sum = 0;//得分小计
                    for (var i = 1; i <= sumday; i++)
                    {
                        decimal grade = 0;
                        if (check_Type == "日检")
                        {
                            grade = recordList.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == i).Select(c => c.PointsDeducted).FirstOrDefault();
                        }
                        else
                        {
                            decimal grade1 = recordList.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == i && c.Check_Type == "巡检").Select(c => c.PointsDeducted).DefaultIfEmpty().Sum();//7S扣分
                            decimal grade2 = recordList.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == i && c.Check_Type == "巡检").Select(c => c.RepetitionPointsDeducted).DefaultIfEmpty().Sum();//重复出现扣分
                            decimal grade3 = recordList.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Date.Year == date.Year && c.Date.Month == date.Month && c.Date.Day == i && c.Check_Type == "巡检").Select(c => c.RectificationPoints).DefaultIfEmpty().Sum();//限期未整改扣分
                            grade = grade1 + grade2 + grade3;//7S扣分+重出现扣分+未整改扣分
                        }
                        if (grade == 0) dateList.Add(" ");
                        else dateList.Add(grade);
                        sum += grade;
                    }
                    list.Add("PointsDeducted", dateList);
                    list.Add("GradeSum", sum);
                    result.Add(list);
                }
                return commom.GetModuleFromJarray(result, true, "查询成功！");
            }
            return commom.GetModuleFromJarray(result, false, "传入参数为空！");
        }
        #endregion

        #region--周检汇总表
        [HttpPost]
        [ApiAuthorize]
        public JObject Week_SumQuery([System.Web.Http.FromBody]JObject data)// string department, DateTime date, string position, int? district
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string department = obj.department;
            string position = obj.position;
            //int? district = data["district"] == null ? 0 : int.Parse(data["district"].ToString());
            var quyuhao = (obj.district).ToString();
            int district = 0;
            if (!String.IsNullOrEmpty(quyuhao))
            {
                district = obj.district;
            }
            DateTime date = obj.date;
            DateTime CurDate = date;  // 当前指定月份
            DateTime dt = CurDate.AddDays(1 - CurDate.Day).AddMonths(1).AddDays(-1);  // 返回指定当前月份的最后一
            JArray weekday = new JArray();
            weekday = getWeekNumInMonth(dt);//获取一个月有几周

            List<KPI_7S_DistrictPosition> positionList = new List<KPI_7S_DistrictPosition>();
            var list = db.KPI_7S_Record.Where(c => c.Date.Year == date.Year && c.Month == date.Month && c.Check_Type == "周检").ToList();//找出当月等于周检的记录
            var depTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= date).Select(c => c.VersionsTime).Distinct().ToList().Max();//获取版本时间
            if (department == "全部部门" && String.IsNullOrEmpty(position) && district == 0)//全部部门
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && !String.IsNullOrEmpty(position) && district == 0)//部门、位置
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && !String.IsNullOrEmpty(position) && district != 0)//部门、位置、区域
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.District == district && c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && !String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == 0)//单个部门
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.VersionsTime == depTime).ToList();
            }
            JArray result = new JArray();
            List<KPI_7S_Record> depList = new List<KPI_7S_Record>();
            foreach (var item in positionList)
            {
                JArray dateList = new JArray();
                JObject redlist = new JObject();
                redlist.Add("Department", item.Department);//部门
                redlist.Add("Position", item.Position);//位置
                redlist.Add("District", item.District);//区域
                decimal sum = 0;//扣分合计
                for (var i = 1; i <= Convert.ToInt32(weekday[1]); i++)//周
                {
                    decimal grade = 0;
                    decimal Grade_week = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Week == i).Select(c => c.PointsDeducted).DefaultIfEmpty().DefaultIfEmpty().Sum();//正常扣分
                    decimal Week_NotCorrected = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Week == i).Select(c => c.RectificationPoints).DefaultIfEmpty().DefaultIfEmpty().Sum();//限期未整改扣分
                    decimal Week_repetition = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Week == i).Select(c => c.RepetitionPointsDeducted).DefaultIfEmpty().DefaultIfEmpty().Sum();//重复出现改扣分
                    grade = Grade_week + Week_NotCorrected + Week_repetition;
                    if (grade == 0) dateList.Add(" ");
                    else dateList.Add(grade);
                    sum += grade;
                }
                redlist.Add("Week", dateList);
                redlist.Add("GradeSum", sum);
                result.Add(redlist);
            }
            return commom.GetModuleFromJarray(result, true, "查询成功！");
        }
        #endregion

        #region---7S汇总表
        [HttpPost]
        [ApiAuthorize]
        public JObject KPI_7S_SummarySheet([System.Web.Http.FromBody]JObject data)//原方法参数：string department, string group, string position, int? district, DateTime date
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string department = obj.department;
            string group = obj.group;
            string position = obj.position;
            //int? district = data["district"] == null ? 0 : int.Parse(data["district"].ToString());
            var quyuhao = (obj.district).ToString();
            int district = 0;
            if (!String.IsNullOrEmpty(quyuhao))
            {
                district = obj.district;
            }
            DateTime date = Convert.ToDateTime(obj.date);
            JArray result = new JArray();
            List<KPI_7S_DistrictPosition> positionList = new List<KPI_7S_DistrictPosition>();
            var depTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= date).Select(c => c.VersionsTime).Distinct().ToList().Max();//获取版本时间
            if (department == "全部部门" && String.IsNullOrEmpty(position) && district == 0 && String.IsNullOrEmpty(group))//全部部门
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && String.IsNullOrEmpty(position) && district == 0 && !String.IsNullOrEmpty(group))//部门、班组
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Group == group && c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && !String.IsNullOrEmpty(position) && district == 0 && !String.IsNullOrEmpty(group))//部门、班组、位置
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Position == position && c.Group == group && c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && !String.IsNullOrEmpty(position) && district != 0 && !String.IsNullOrEmpty(group))//部门、班组、位置、区域
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.Group == group && c.Position == position && c.District == district && c.VersionsTime == depTime).ToList();
            }
            else if (department != "全部部门" && !String.IsNullOrEmpty(department) && String.IsNullOrEmpty(position) && district == 0)//单个部门
            {
                positionList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.VersionsTime == depTime).ToList();
            }
            var list = db.KPI_7S_Record.Where(c => c.Date.Year == date.Year && c.Month == date.Month).ToList();//找出当月的记录
            foreach (var item in positionList)
            {
                JObject redlist = new JObject();
                redlist.Add("Department", item.Department);//部门
                redlist.Add("Group", item.Group == null ? " " : item.Group);//区域
                redlist.Add("Position", item.Position);//位置
                redlist.Add("District", item.District);//区域                
                var Grade_daily = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "日检").Select(c => c.PointsDeducted).DefaultIfEmpty().Sum();//日检
                var Grade_week = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "周检").Select(c => c.PointsDeducted).DefaultIfEmpty().Sum();//周检
                var Grade_random = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "巡检").Select(c => c.PointsDeducted).DefaultIfEmpty().Sum();//巡检正常扣分

                var Week_NotCorrected = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "周检").Select(c => c.RectificationPoints).DefaultIfEmpty().Sum();//周检未整改
                var Random_NotCorrected = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "巡检").Select(c => c.RectificationPoints).DefaultIfEmpty().Sum();//巡检未整改

                var Week_Repetition = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "周检").Select(c => c.RepetitionPointsDeducted).DefaultIfEmpty().Sum();//周检重复出现
                var Random_Repetition = list.Where(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Check_Type == "巡检").Select(c => c.RepetitionPointsDeducted).DefaultIfEmpty().Sum();//巡检重复出现

                decimal total = 100 - Grade_daily - Grade_week - (Week_NotCorrected + Random_NotCorrected) - (Week_Repetition + Random_Repetition) - Grade_random;//实际得分
                redlist.Add("Grade_daily", Grade_daily);//日检扣分
                redlist.Add("Grade_week", Grade_week);//周检正常扣分     
                redlist.Add("Week_NotCorrected", Week_NotCorrected);//周检未整改扣分
                redlist.Add("Week_Repetition", Week_Repetition);//周检重复出现扣分
                redlist.Add("Week_Sum", Grade_week + Week_NotCorrected + Week_Repetition);//周检扣分合计

                redlist.Add("Grade_random", Grade_random);//巡检正常扣分
                redlist.Add("Random_NotCorrected", Random_NotCorrected);//巡检未整改扣分
                redlist.Add("Random_Repetition", Random_Repetition);//巡检重复出现扣分
                redlist.Add("Random_Sum", Grade_random + Random_NotCorrected + Random_Repetition);//巡检扣分合计
                redlist.Add("TotalPoints", total);//月末得分
                result.Add(redlist);
            }
            return commom.GetModuleFromJarray(result, true, "查询成功！");
        }
        #endregion

        #region--手机端 获取扣分类型
        [HttpPost]
        [ApiAuthorize]
        public JObject GainPointsType()//原方法名：GetPointsType
        {
            var list = db.KPI_7S_ReferenceStandard.OrderByDescending(c => c.PointsType).ToList();
            var record = list.Select(c => c.PointsType).Distinct().ToList();
            JArray result = new JArray();
            foreach (var item in record)
            {
                JObject res = new JObject();
                res.Add("value", item);
                res.Add("label", item);
                result.Add(res);
            }
            return commom.GetModuleFromJarray(result, true, "查询成功！");
        }
        #endregion

        #region--手机端 根据扣分类型获取扣分项
        [HttpPost]
        [ApiAuthorize]
        public JObject GetReferenceStandard([System.Web.Http.FromBody]JObject data)//string pointsType
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string pointsType = obj.pointsType;
            JArray result = new JArray();
            if (!String.IsNullOrEmpty(pointsType))
            {
                var list = db.KPI_7S_ReferenceStandard.Where(c => c.PointsType == pointsType).Select(c => c.ReferenceStandard).ToList();
                foreach (var item in list)
                {
                    JObject res = new JObject();
                    res.Add("value", item);
                    res.Add("count", 0);
                    result.Add(res);
                }
                return commom.GetModuleFromJarray(result, true, "查询成功！");
            }
            return commom.GetModuleFromJarray(result, false, "传入参数为空！");
        }
        #endregion

        #region---手机端 删除未创建记录的照片
        [HttpPost]
        [ApiAuthorize]
        public JObject KPI_7S_DelImage([System.Web.Http.FromBody]JObject data)//原方法名：KPI_7S_DeleteImage    //原方法参数：string department, string position, DateTime check_date, string check_Type, string pointsDeducted_Type, int district
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string department = obj.department;
            string position = obj.position;
            DateTime check_date = Convert.ToDateTime(data["check_date"]);
            string check_Type = obj.check_Type;
            string pointsDeducted_Type = obj.pointsDeducted_Type;
            int district = int.Parse(data["district"].ToString());
            JObject result = new JObject();
            if (!String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && !String.IsNullOrEmpty(check_Type) && !String.IsNullOrEmpty(pointsDeducted_Type) && district != 0)
            {
                JArray beforeImprovementImage = new JArray();
                beforeImprovementImage = QueryImage(department, position, check_date, check_Type, pointsDeducted_Type, "改善前", district);
                JArray afterImprovementImage = new JArray();
                afterImprovementImage = QueryImage(department, position, check_date, check_Type, pointsDeducted_Type, "改善后", district);
                int num = 0;
                int num1 = beforeImprovementImage.Count() + afterImprovementImage.Count();
                if (beforeImprovementImage.Count > 0 && beforeImprovementImage.Count < 2)
                {
                    foreach (var item in beforeImprovementImage)
                    {
                        var res = DelImg(toJobject(item.ToString(), department, position, check_date, check_Type, pointsDeducted_Type, "改善前", district));//DeleteImg(item.ToString(), department, position, check_date, check_Type, pointsDeducted_Type, "改善前", district);
                        if (res.ToString() == "删除成功") num++;
                    }
                }
                else
                {
                    foreach (var item in beforeImprovementImage)
                    {
                        beforeImprovementImage = QueryImage(department, position, check_date, check_Type, pointsDeducted_Type, "改善前", district);
                        var res = DelImg(toJobject(beforeImprovementImage[0].ToString(), department, position, check_date, check_Type, pointsDeducted_Type, "改善前", district)); //DeleteImg(beforeImprovementImage[0].ToString(), department, position, check_date, check_Type, pointsDeducted_Type, "改善前", district);
                        if (res.ToString() == "删除成功") num++;
                    }
                }
                if (afterImprovementImage.Count > 0 && afterImprovementImage.Count < 2)
                {
                    foreach (var item in afterImprovementImage)
                    {
                        var res = DelImg(toJobject(item.ToString(), department, position, check_date, check_Type, pointsDeducted_Type, "改善后", district)); //DeleteImg(item.ToString(), department, position, check_date, check_Type, pointsDeducted_Type, "改善后", district);
                        if (res.ToString() == "删除成功") num++;
                    }
                }
                else
                {
                    foreach (var item in afterImprovementImage)
                    {
                        afterImprovementImage = QueryImage(department, position, check_date, check_Type, pointsDeducted_Type, "改善后", district);
                        var res = DelImg(toJobject(afterImprovementImage[0].ToString(), department, position, check_date, check_Type, pointsDeducted_Type, "改善后", district)); //DeleteImg(afterImprovementImage[0].ToString(), department, position, check_date, check_Type, pointsDeducted_Type, "改善后", district);
                        if (res.ToString() == "删除成功") num++;
                    }
                }
                if (num == num1) return commom.GetModuleFromJobjet(result, true, "删除成功");
                else return commom.GetModuleFromJobjet(result, false, "删除失败");
            }
            return commom.GetModuleFromJobjet(result, false, "传入参数为空！");
        }
        #endregion

        #region---7S日检交表记录查询
        [HttpPost]
        [ApiAuthorize]
        public JObject DailyRecord_Query([System.Web.Http.FromBody]JObject data)//原方法参数：DateTime date, string department
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            DateTime date = Convert.ToDateTime(data["date"]);
            string department = obj.department;
            JArray result = new JArray();
            if (!string.IsNullOrEmpty(department))
            {
                int sumday = date.AddDays(1 - date.Day).AddMonths(1).AddDays(-1).Day;//一个月有多少天  
                DateTime dt = date.AddMonths(1);
                dt = dt.AddDays(0 - dt.Day);
                var versionstime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= dt).Select(c => c.VersionsTime).Distinct().Max();
                List<KPI_7S_DistrictPosition> depList = new List<KPI_7S_DistrictPosition>();
                if (department == "全部部门")
                {
                    depList = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == versionstime).ToList();
                }
                else
                {
                    depList = db.KPI_7S_DistrictPosition.Where(c => c.Department == department && c.VersionsTime == versionstime).ToList();
                }
                var monthList = db.KPI_7S_Record.Where(c => c.Year == date.Year && c.Month == date.Month && c.Check_Type != "周检" && c.Check_Type != "巡检" && c.Remark != "未交表").ToList();
                foreach (var item in depList)
                {
                    JArray dateList = new JArray();
                    JObject redlist = new JObject();
                    redlist.Add("Department", item.Department);//部门
                    redlist.Add("Position", item.Position);//位置
                    redlist.Add("District", item.District);//区域

                    for (int i = 1; i <= sumday; i++)
                    {
                        var res = monthList.Count(c => c.Department == item.Department && c.Position == item.Position && c.District == item.District && c.Year == date.Year && c.Month == date.Month && c.Date.Day == i);
                        if (res == 0) dateList.Add("false");
                        else dateList.Add("true");
                    }
                    redlist.Add("DailyRecord", dateList);
                    result.Add(redlist);
                }
                return commom.GetModuleFromJarray(result, true, "查询成功！");
            }
            else return commom.GetModuleFromJarray(result, false, "传入参数为空！");
        }
        #endregion

        #region----详细页修改限期整改时间
        [HttpPost]
        [ApiAuthorize]
        public JObject RectificationTime_modify([System.Web.Http.FromBody]JObject data)//int id, string time
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int id = int.Parse(data["id"].ToString());
            JObject result = new JObject();
            string time = obj.time;
            int saveNum = 0;
            if (id != 0)
            {
                var list = db.KPI_7S_Record.Where(c => c.Id == id).FirstOrDefault();
                list.RectificationTime = Convert.ToDateTime(time);
                saveNum = db.SaveChanges();
            }
            if (saveNum > 0) return commom.GetModuleFromJobjet(result, true, "修改成功！");
            else return commom.GetModuleFromJobjet(result, false, "修改失败！");
        }
        #endregion

        #region--部门得分排名
        public class ranking_list
        {
            public int Ranking { get; set; }
            public string Department { get; set; }
            public string Score { get; set; }
        }
        public JObject KPI_7S_Ranking([System.Web.Http.FromBody]JObject data)//原方法参数：DateTime time
        {
            DateTime time = Convert.ToDateTime(data["time"]);
            JArray res = new JArray();
            var depTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= time).Select(c => c.VersionsTime).Distinct().ToList().Max();//获取版本时间
            var depList = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == depTime).Select(c => c.Department).Distinct().ToList();//根据版本时间找出部门
            var allList = db.KPI_7S_Record.Where(c => c.Year == time.Year && c.Month == time.Month).ToList();//找出当月的所有记录
            foreach (var item in depList)
            {
                JObject record = new JObject();
                var daily = allList.Where(c => c.Department == item && c.Check_Type == "日检").Select(c => c.PointsDeducted).Sum();
                var week = allList.Where(c => c.Department == item && c.Check_Type == "周检").Select(c => c.PointsDeducted).Sum();
                var random = allList.Where(c => c.Department == item && c.Check_Type == "巡检").Select(c => c.PointsDeducted).Sum();
                var rectification = allList.Where(c => c.Department == item).Select(c => c.RectificationPoints).Sum();
                var repetition = allList.Where(c => c.Department == item).Select(c => c.RepetitionPointsDeducted).Sum();
                record.Add("Department", item);//部门
                record.Add("Score", 100 - daily - week - random - rectification - repetition);
                res.Add(record);
            }
            var rank = new JArray(res.OrderByDescending(c => c["Score"]));//按高到低排序
            List<ranking_list> rankList = new List<ranking_list>();
            int num = 1;
            //开始排名
            foreach (var i in rank)
            {
                ranking_list record = new ranking_list();
                if (num == 1)
                {
                    record.Ranking = num;//名次
                    record.Department = i["Department"].ToString();//部门
                    record.Score = i["Score"].ToString();//分数
                    rankList.Add(record);
                    num++;
                }
                else
                {
                    var iShave = rankList.Count(c => c.Score == i["Score"].ToString());
                    if (iShave > 0)
                    {
                        record.Ranking = num - 1;//名次
                        record.Department = i["Department"].ToString();//部门
                        record.Score = i["Score"].ToString();//分数
                        rankList.Add(record);
                    }
                    else
                    {
                        record.Ranking = num;//名次
                        record.Department = i["Department"].ToString();//部门
                        record.Score = i["Score"].ToString();//分数
                        rankList.Add(record);
                        num++;
                    }
                }
            }
            JObject result = new JObject();
            result.Add("Data", JsonConvert.SerializeObject(rankList));
            return commom.GetModuleFromJobjet(result, true, "查询成功！");
        }
        #endregion

        #region---获取一周有几天
        public JArray getWeekNumInMonth(DateTime daytime)
        {
            JArray res = new JArray();
            int dayInMonth = daytime.Day;
            DateTime firstDay = daytime.AddDays(1 - daytime.Day);//本月第一天            
            int weekday = (int)firstDay.DayOfWeek == 0 ? 7 : (int)firstDay.DayOfWeek;//本月第一天是周几           
            int firstWeekEndDay = 7 - (weekday - 1);//本月第一周有几天                     
            int diffday = dayInMonth - firstWeekEndDay;//当前日期和第一周之差
            diffday = diffday > 0 ? diffday : 1;
            int WeekNumInMonth = ((diffday % 7) == 0 ? (diffday / 7 - 1) : (diffday / 7)) + 1 + (dayInMonth > firstWeekEndDay ? 1 : 0);//当前是第几周,如果整除7就减一天
            if (WeekNumInMonth == 1)
            {
                int weekdays = firstWeekEndDay - 2;//去掉周末两天
                res.Add(weekdays);
            }
            else if (WeekNumInMonth == 2 || WeekNumInMonth == 3)
            {
                res.Add(5);
            }
            else if (WeekNumInMonth == 4 || WeekNumInMonth == 5 || WeekNumInMonth == 6)
            {
                DateTime startWeek = daytime.AddDays(1 - Convert.ToInt32(daytime.DayOfWeek.ToString("d")));  //本周周一
                DateTime endWeek = startWeek.AddDays(6);  //本周周日
                if (endWeek.Month != daytime.Month)
                {
                    if (endWeek.Day == 6)//本周最后一天等于6号，那么这周出去星期周末只有一天
                    {
                        res.Add(1);
                    }
                    else if (endWeek.Day == 5)//本周最后一天等于5号，那么这周出去星期周末只有两天
                    {
                        res.Add(2);
                    }
                    else if (endWeek.Day == 4)//本周最后一天等于4号，那么这周出去星期周末只有三天
                    {
                        res.Add(3);
                    }
                    else if (endWeek.Day == 3)//本周最后一天等于3号，那么这周出去星期周末只有四天
                    {
                        res.Add(4);
                    }
                    else if (endWeek.Day == 2)//本周最后一天等于2号，那么这周出去星期周末只有五天
                    {
                        res.Add(5);
                    }
                    else if (endWeek.Day == 1)//本周最后一天等于1号，那么这周出去星期周末只有五天
                    {
                        res.Add(5);
                    }
                }
                else
                {
                    res.Add(5);
                }
            }
            res.Add(WeekNumInMonth);
            return res;
        }
        #endregion
        #endregion

        #region 直通率
        public class Temp
        {
            public string Name { get; set; }
            public string OrderNum { get; set; }
            public string JobContent { get; set; }
            public string BarCodesNum { get; set; }
            public string Group { get; set; }
            public bool Finish { get; set; }
            public int Passcount { get; set; }
            public int AbnormalCount { get; set; }
        }
        //各工序直通率查询首页
        [HttpPost]
        [ApiAuthorize]
        public JObject PassThrough([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string ordernum = obj.ordernum;
            string type = obj.type;
            int year = obj.year;
            int mouth = obj.mouth;

            var orderlist = new List<string>();
            if (!string.IsNullOrEmpty(ordernum))
            {
                orderlist.Add(ordernum);
            }
            else
            {
                var info = db.OrderMgm.Where(c => c.PlatformType == type).ToList();

                if (year != 0)
                {
                    info = info.Where(c => c.ContractDate.Year == year).ToList();
                }
                if (mouth != 0)
                {
                    info = info.Where(c => c.ContractDate.Month == mouth).ToList();

                }
                orderlist = info.Select(c => c.OrderNum).ToList();
            }
            JArray total = new JArray();
            JArray array = new JArray();
            JObject result = new JObject();
            result.Add("seaction", "模块段");
            //smt直通率
            var smt = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum)).Select(c => c.JobContent).Distinct().ToList();
            foreach (var jobcontent in smt)
            {
                JObject content = new JObject();
                var linenumlist = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum) && c.JobContent == jobcontent).Select(c => new Temp { Group = c.LineNum.ToString(), Passcount = c.NormalCount, AbnormalCount = c.AbnormalCount, Name = "SMT" }).Distinct().ToList();
                content = GetPassThroughitem(jobcontent, linenumlist);
                array.Add(content);
            }
            //AI直通率
            var ai = db.ModuleAI.Where(c => orderlist.Contains(c.Ordernum)).Select(c => new Temp { Group = c.Machine, Finish = c.IsAbnormal, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("AI", ai));
            //后焊直通率
            var group = db.AfterWelding.Where(c => orderlist.Contains(c.Ordernum)).Select(c => new Temp { Group = c.Group, Finish = c.IsAbnormal, BarCodesNum = c.ModuleBarcode }).ToList();

            array.Add(GetPassThroughitem("后焊", group));
            //后焊抽检直通率
            var groupSampl = db.ModuleSampling.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "后焊").Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("后焊抽检", groupSampl));
            //灌胶前电测
            var Electric1 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "灌胶前电检").Select(c => new Temp { Group = c.Group, Finish = c.ElectricInspectionResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("灌胶前电检", Electric1));
            //模块电测
            var Electric2 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "模块电检").Select(c => new Temp { Group = c.Group, Finish = c.ElectricInspectionResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("模块电检", Electric2));
            //模块送检
            var Electric3 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "模块电检" && c.IsSampling == true && c.ElectricInspectionResult == true).Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("模块送检", Electric3));
            //模块老化
            var burn = db.ModuleBurnIn.Where(c => orderlist.Contains(c.Ordernum) && c.BurnInEndTime != null).Select(c => new Temp { Group = c.Group, Finish = c.BurninResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("模块老化", burn));
            //模块老化送检
            var burnSampl = db.ModuleBurnIn.Where(c => orderlist.Contains(c.Ordernum) && c.BurnInEndTime != null && c.IsSampling == true && c.BurninResult == true
            ).Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("老化送检", burnSampl));
            //外观电检
            var Electric4 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "外观电检").Select(c => new Temp { Group = c.Group, Finish = c.ElectricInspectionResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(GetPassThroughitem("外观电检", Electric4));

            result.Add("content", array);
            total.Add(result);
            result = new JObject();
            array = new JArray();
            result.Add("seaction", "模组段");
            //组装直通率
            var assemblessgrou = db.Assemble.Where(c => orderlist.Contains(c.OrderNum) && c.PQCCheckFT != null && c.RepetitionPQCCheck == false).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BoxBarCode, Finish = c.PQCCheckFinish }).ToList();
            array.Add(GetPassThroughitem("组装", assemblessgrou));

            //FQC直通率
            var fqcgroup = db.FinalQC.Where(c => orderlist.Contains(c.OrderNum) && c.FQCCheckFT != null && c.RepetitionFQCCheck == false).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.FQCCheckFinish }).ToList();
            array.Add(GetPassThroughitem("FQC", fqcgroup));


            //老化直通率
            var burngroup = db.Burn_in.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.Burn_in_OQCCheckAbnormal == "正常" ? true : false }).ToList();
            array.Add(GetPassThroughitem("老化", burngroup));
            //校正直通率
            var calibgroup = db.CalibrationRecord.Where(c => orderlist.Contains(c.OrderNum) && c.FinishCalibration != null && c.RepetitionCalibration == false).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.Normal }).ToList();
            array.Add(GetPassThroughitem("校正", calibgroup));
            //外观直通率
            var appearangroup = db.Appearance.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.OQCCheckFinish }).ToList();
            array.Add(GetPassThroughitem("外观", appearangroup));
            result.Add("content", array);
            total.Add(result);

            return commom.GetModuleFromJarray(total);
        }

        public JObject GetPassThroughitem(string name, List<Temp> temps)
        {
            JObject result = new JObject();
            result.Add("title", name);
            JArray array = new JArray();
            List<string> groupitem = temps.Select(c => c.Group).Distinct().ToList();
            groupitem = groupitem.OrderBy(c => c).ToList();
            foreach (var item in groupitem)
            {
                JObject obj = new JObject();
                if (temps.Count(c => c.Name == "SMT") != 0)
                {
                    obj.Add("group", item + "线");
                    var totalinfo = temps.Where(c => c.Group == item).ToList();
                    var totalNum = totalinfo.Sum(c => c.AbnormalCount) + totalinfo.Sum(c => c.Passcount);
                    //直通率
                    var info = temps.Where(c => c.Group == item).Select(c => c.Passcount).ToList();
                    var passtrough = info.Sum();
                    obj.Add("passThrough", totalNum == 0 ? "-%" : Math.Round((double)passtrough * 100 / totalNum, 2) + "%");

                    //异常率
                    var info2 = temps.Where(c => c.Group == item).Select(c => c.AbnormalCount).ToList();
                    var abnormal = info2.Sum();
                    obj.Add("abnormal", totalNum == 0 ? "-%" : Math.Round((double)abnormal * 100 / totalNum, 2) + "%");

                    array.Add(obj);
                }
                else if (item == "")
                    continue;
                else if (string.IsNullOrEmpty(item))
                {
                    obj.Add("group", "没有班组");
                    int totalNum = temps.Where(c => c.Group == item || c.Group == "").Select(c => c.BarCodesNum).Distinct().Count();
                    //合格率
                    var abnormal = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == false).Select(c => c.BarCodesNum).ToList();

                    //直通率
                    var pass = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == true).Count();

                    var info = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == true && !abnormal.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).Distinct().Count();
                    //var passtrough = info.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var abno = abnormal.Distinct().Count();
                    obj.Add("passThrough", totalNum == 0 ? "-%" : Math.Round((double)info * 100 / totalNum, 2) + "%");
                    obj.Add("abnormal", totalNum == 0 ? "-%" : Math.Round((double)abno * 100 / totalNum, 2) + "%");

                    array.Add(obj);
                }
                else
                {
                    obj.Add("group", item);
                    int totalNum = temps.Where(c => c.Group == item).Select(c => c.BarCodesNum).Distinct().Count(); ;
                    //异常率
                    var abnormal = temps.Where(c => (c.Group == item) && c.Finish == false).Select(c => c.BarCodesNum).ToList();

                    var pass = temps.Where(c => (c.Group == item) && c.Finish == true).Count();

                    //直通率
                    var info = temps.Where(c => c.Group == item && c.Finish == true && !abnormal.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).Distinct().Count();
                    //var passtrough = info.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    obj.Add("passThrough", totalNum == 0 ? "-%" : Math.Round((double)info * 100 / totalNum, 2) + "%");
                    var abno = abnormal.Distinct().Count();
                    obj.Add("abnormal", totalNum == 0 ? "-%" : Math.Round((double)abno * 100 / totalNum, 2) + "%");

                    array.Add(obj);
                }
            }
            if (temps.Count(c => c.Name == "SMT") != 0)
            {
                //合计直通率
                JObject totalobj = new JObject();
                totalobj.Add("group", "合计");
                var total = temps.Sum(c => c.AbnormalCount) + temps.Sum(c => c.Passcount);
                var totalpasstrough = temps.Sum(c => c.Passcount);
                var totalabnormal = temps.Sum(c => c.AbnormalCount);

                totalobj.Add("passThrough", temps.Count == 0 ? "-%" : Math.Round((double)totalpasstrough * 100 / total, 2) + "%");

                //合计异常率
                totalobj.Add("abnormal", temps.Count == 0 ? "-%" : Math.Round((double)totalabnormal * 100 / total, 2) + "%");
                array.Add(totalobj);
            }
            else
            {
                //合计直通率
                JObject totalobj = new JObject();
                totalobj.Add("group", "合计");
                var totalabnormal = temps.Where(c => c.Finish == false).Select(c => c.BarCodesNum).ToList();
                var totalpasstrough = temps.Where(c => c.Finish == true && !totalabnormal.Contains(c.BarCodesNum)).GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                var totalpasstrough2 = temps.Where(c => c.Finish == true && !totalabnormal.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).Distinct().Count();
                var totalCount = temps.Select(c => c.BarCodesNum).Distinct().Count();
                totalobj.Add("passThrough", temps.Count == 0 ? "-%" : Math.Round((double)totalpasstrough2 * 100 / totalCount, 2) + "%");
                var pass = temps.Where(c => c.Finish == true).Count();
                var abno = totalabnormal.Distinct().Count();
                //合计异常率
                totalobj.Add("abnormal", temps.Count == 0 ? "-%" : Math.Round((double)abno * 100 / totalCount, 2) + "%");
                array.Add(totalobj);
            }
            result.Add("array", array);
            return result;
        }

        //各工序直通率查询
        [HttpPost]
        [ApiAuthorize]
        public JObject ChartGetPassThrough([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string ordernum = obj.ordernum;
            string type = obj.type;
            int year = obj.year;
            int mouth = obj.mouth;

            var orderlist = new List<string>();
            if (!string.IsNullOrEmpty(ordernum))
            {
                orderlist.Add(ordernum);
            }
            else
            {
                var info = db.OrderMgm.Where(c => c.PlatformType == type).ToList();

                if (year != 0)
                {
                    info = info.Where(c => c.ContractDate.Year == year).ToList();
                }
                if (mouth != 0)
                {
                    info = info.Where(c => c.ContractDate.Month == mouth).ToList();

                }
                orderlist = info.Select(c => c.OrderNum).ToList();
            }
            JArray total = new JArray();
            JArray array = new JArray();
            JObject result = new JObject();
            //smt直通率
            var smt = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum)).Select(c => c.JobContent).Distinct().ToList();
            foreach (var jobcontent in smt)
            {
                JObject content = new JObject();
                var linenumlist = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum) && c.JobContent == jobcontent).Select(c => new Temp { Group = c.LineNum.ToString(), Passcount = c.NormalCount, AbnormalCount = c.AbnormalCount, Name = "SMT" }).Distinct().ToList();
                content = ChartGetPassThroughitem(jobcontent, linenumlist);
                array.Add(content);
            }
            //AI直通率
            var ai = db.ModuleAI.Where(c => orderlist.Contains(c.Ordernum)).Select(c => new Temp { Group = c.Machine, Finish = c.IsAbnormal, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("AI", ai));
            //后焊直通率
            var group = db.AfterWelding.Where(c => orderlist.Contains(c.Ordernum)).Select(c => new Temp { Group = c.Group, Finish = c.IsAbnormal, BarCodesNum = c.ModuleBarcode }).ToList();

            array.Add(ChartGetPassThroughitem("后焊", group));
            //后焊抽检直通率
            var groupSampl = db.AfterWelding.Where(c => orderlist.Contains(c.Ordernum) && c.IsSampling == true).Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("后焊抽检", groupSampl));
            //灌胶前电测
            var Electric1 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "灌胶前电检").Select(c => new Temp { Group = c.Group, Finish = c.ElectricInspectionResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("灌胶前电检", Electric1));
            //模块电测
            var Electric2 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "模块电检").Select(c => new Temp { Group = c.Group, Finish = c.ElectricInspectionResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("模块电检", Electric2));
            //模块送检
            var Electric3 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "模块电检" && c.IsSampling == true && c.ElectricInspectionResult == true).Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("模块送检", Electric3));
            //模块老化
            var burn = db.ModuleBurnIn.Where(c => orderlist.Contains(c.Ordernum) && c.BurnInEndTime != null).Select(c => new Temp { Group = c.Group, Finish = c.BurninResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("模块老化", burn));
            //模块老化送检
            var burnSampl = db.ModuleBurnIn.Where(c => orderlist.Contains(c.Ordernum) && c.BurnInEndTime != null && c.IsSampling == true && c.BurninResult == true
            ).Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("老化送检", burnSampl));
            //外观电检
            var Electric4 = db.ElectricInspection.Where(c => orderlist.Contains(c.Ordernum) && c.Section == "外观电检").Select(c => new Temp { Group = c.Group, Finish = c.ElectricInspectionResult, BarCodesNum = c.ModuleBarcode }).ToList();
            array.Add(ChartGetPassThroughitem("外观电检", Electric4));

            result.Add("MK", array);
            array = new JArray();
            //组装直通率
            var assemblessgrou = db.Assemble.Where(c => orderlist.Contains(c.OrderNum) && c.PQCCheckFT != null && c.RepetitionPQCCheck == false).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BoxBarCode, Finish = c.PQCCheckFinish }).ToList();
            array.Add(ChartGetPassThroughitem("组装", assemblessgrou));

            //FQC直通率
            var fqcgroup = db.FinalQC.Where(c => orderlist.Contains(c.OrderNum) && c.FQCCheckFT != null && c.RepetitionFQCCheck == false).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.FQCCheckFinish }).ToList();
            array.Add(ChartGetPassThroughitem("FQC", fqcgroup));


            //老化直通率
            var burngroup = db.Burn_in.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.Burn_in_OQCCheckAbnormal == "正常" ? true : false }).ToList();
            array.Add(ChartGetPassThroughitem("老化", burngroup));
            //校正直通率
            var calibgroup = db.CalibrationRecord.Where(c => orderlist.Contains(c.OrderNum) && c.FinishCalibration != null && c.RepetitionCalibration == false).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.Normal }).ToList();
            array.Add(ChartGetPassThroughitem("校正", calibgroup));
            //外观直通率
            var appearangroup = db.Appearance.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.OQCCheckFinish }).ToList();
            array.Add(ChartGetPassThroughitem("外观", appearangroup));
            result.Add("MZ", array);

            return commom.GetModuleFromJobjet(result);
        }

        public JObject ChartGetPassThroughitem(string name, List<Temp> temps)
        {
            JObject result = new JObject();
            result.Add("title", name);
            JArray array = new JArray();
            List<string> groupitem = temps.Select(c => c.Group).Distinct().ToList();
            groupitem = groupitem.OrderBy(c => c).ToList();
            foreach (var item in groupitem)
            {
                JObject obj = new JObject();
                if (temps.Count(c => c.Name == "SMT") != 0)
                {
                    obj.Add("title", item + "线");
                    var totalinfo = temps.Where(c => c.Group == item).ToList();
                    var totalNum = totalinfo.Sum(c => c.AbnormalCount) + totalinfo.Sum(c => c.Passcount);
                    //直通率
                    var info = temps.Where(c => c.Group == item).Select(c => c.Passcount).ToList();
                    var passtrough = info.Sum();
                    obj.Add("passThrough", totalNum == 0 ? 0 : Math.Round((double)passtrough * 100 / totalNum, 2));

                    //异常率
                    var info2 = temps.Where(c => c.Group == item).Select(c => c.AbnormalCount).ToList();
                    var abnormal = info2.Sum();
                    obj.Add("abnormal", totalNum == 0 ? 0 : Math.Round((double)abnormal * 100 / totalNum, 2));

                    array.Add(obj);
                }
                else if (item == "")
                    continue;
                else if (string.IsNullOrEmpty(item))
                {
                    obj.Add("title", "没有班组");
                    int totalNum = temps.Where(c => c.Group == item || c.Group == "").Select(c => c.BarCodesNum).Distinct().Count();
                    //合格率
                    var abnormal = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == false).Select(c => c.BarCodesNum).ToList();

                    //直通率
                    var pass = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == true).Count();

                    var info = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == true && !abnormal.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).Distinct().Count();
                    //var passtrough = info.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var abno = abnormal.Distinct().Count();
                    obj.Add("passThrough", totalNum == 0 ? 0 : Math.Round((double)info * 100 / totalNum, 2));
                    obj.Add("abnormal", totalNum == 0 ? 0 : Math.Round((double)abno * 100 / totalNum, 2));

                    array.Add(obj);
                }
                else
                {
                    obj.Add("title", item);
                    int totalNum = temps.Where(c => c.Group == item).Select(c => c.BarCodesNum).Distinct().Count(); ;
                    //异常率
                    var abnormal = temps.Where(c => (c.Group == item) && c.Finish == false).Select(c => c.BarCodesNum).ToList();

                    var pass = temps.Where(c => (c.Group == item) && c.Finish == true).Count();

                    //直通率
                    var info = temps.Where(c => c.Group == item && c.Finish == true && !abnormal.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).Distinct().Count();
                    //var passtrough = info.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    obj.Add("passThrough", totalNum == 0 ? 0 : Math.Round((double)info * 100 / totalNum, 2));
                    var abno = abnormal.Distinct().Count();
                    obj.Add("abnormal", totalNum == 0 ? 0 : Math.Round((double)abno * 100 / totalNum, 2));

                    array.Add(obj);
                }
            }
            double TpassThrough;
            double Tabnormal;
            if (temps.Count(c => c.Name == "SMT") != 0)
            {
                //合计直通率
                var total = temps.Sum(c => c.AbnormalCount) + temps.Sum(c => c.Passcount);
                var totalpasstrough = temps.Sum(c => c.Passcount);
                var totalabnormal = temps.Sum(c => c.AbnormalCount);
                TpassThrough = temps.Count == 0 ? 0 : Math.Round((double)totalpasstrough * 100 / total, 2);

                //合计异常率
                Tabnormal = temps.Count == 0 ? 0 : Math.Round((double)totalabnormal * 100 / total, 2);

            }
            else
            {
                //合计直通率

                var totalabnormal = temps.Where(c => c.Finish == false).Select(c => c.BarCodesNum).ToList();
                var totalpasstrough2 = temps.Where(c => c.Finish == true && !totalabnormal.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).Distinct().Count();
                var totalCount = temps.Select(c => c.BarCodesNum).Distinct().Count();

                var abno = totalabnormal.Distinct().Count();
                //合计异常率
                TpassThrough = temps.Count == 0 ? 0 : Math.Round((double)totalpasstrough2 * 100 / totalCount, 2);
                Tabnormal = temps.Count == 0 ? 0 : Math.Round((double)abno * 100 / totalCount, 2);
            }
            result.Add("passThrough", TpassThrough);
            result.Add("abnormal", Tabnormal);
            result.Add("groupList", array);
            return result;
        }
        #endregion

        #region 指标名称清单数据录入 
        //显示
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayKPIIndicators([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            DateTime time = datavalue.time;
            string Department = datavalue.Department;
            string Group = datavalue.Group;

            JArray result = new JArray();
            List<KPI_Indicators> value;
            if (time == null)
            {
                var timecheck = db.KPI_Indicators.Max(c => c.ExecutionTime);
                value = db.KPI_Indicators.Where(c => c.ExecutionTime == timecheck).ToList();
            }
            else
            {
                var timecheck = db.KPI_Indicators.OrderByDescending(c => c.ExecutionTime).Where(c => c.ExecutionTime <= time).Select(c => c.ExecutionTime).FirstOrDefault();
                value = db.KPI_Indicators.Where(c => c.ExecutionTime == timecheck).ToList();
            }
            if (!string.IsNullOrEmpty(Department))
            {
                value = value.Where(c => c.Department == Department).ToList();
            }
            if (!string.IsNullOrEmpty(Group))
            {
                value = value.Where(c => c.Group == Group).ToList();
            }
            foreach (var item in value)
            {
                JObject obj = new JObject();
                obj.Add("ID", item.Id);//部门
                obj.Add("Department", item.Department);//部门
                obj.Add("Group", item.Group);//班组
                obj.Add("IndicatorsName", item.IndicatorsName);//指标名
                obj.Add("IndicatorsDefine", item.IndicatorsDefine);//指标定义
                obj.Add("ComputationalFormula", item.ComputationalFormula);//指标计算工时
                obj.Add("IndicatorsValue", item.IndicatorsValue);//目标值
                obj.Add("IndicatorsValueUnit", item.IndicatorsValueUnit);//目标值
                obj.Add("DataName", item.DataName);//数据名称
                obj.Add("Cycle", item.Cycle);//数据周期
                obj.Add("SourceDepartment", item.SourceDepartment);//数据工号
                obj.Add("DataInputor", item.DataInputor);//数据姓名
                obj.Add("DataInputTime", item.DataInputTime == null ? null : item.DataInputTime.ToString());//录入时间
                obj.Add("IndicatorsType", item.IndicatorsType);//考核类型  品质或效率
                obj.Add("IndicatorsStatue", item.IndicatorsStatue);//考核异常或者正常
                obj.Add("ExecutionTime", item.ExecutionTime.ToString());//品质考核类型, 直通 或者合格率

                result.Add(obj);
            }
            return commom.GetModuleFromJarray(result);
        }

        //单个新增
        [HttpPost]
        [ApiAuthorize]
        public JObject SingleAddKPIIndicators([System.Web.Http.FromBody]JObject data)
        {
            KPI_Indicators indicators = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            var info = db.KPI_Indicators.Count(c => c.Department == indicators.Department && c.Group == indicators.Group && c.IndicatorsType == indicators.IndicatorsType && c.ExecutionTime == indicators.ExecutionTime);
            if (info != 0)
            {
                return commom.GetModuleFromJarray(null, false, "有重复数据");
            }

            db.KPI_Indicators.Add(indicators);
            db.SaveChanges();
            return commom.GetModuleFromJarray(null, true, "成功");
        }

        //增加一个新版本
        [HttpPost]
        [ApiAuthorize]
        public JObject BacthAddKPIIndicators([System.Web.Http.FromBody]JObject data)
        {
            List<KPI_Indicators> indicators = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            foreach (var item in indicators)
            {
                var num = db.KPI_Indicators.Count(c => c.ExecutionTime == item.ExecutionTime && c.Group == item.Group && c.Department
                == item.Department && c.IndicatorsType == item.IndicatorsType);
                if (num != 0)
                {
                    return commom.GetModuleFromJarray(null, false, "部门班组" + item.Department + item.Group + "已有重复的版本信息");
                }
            }

            db.KPI_Indicators.AddRange(indicators);
            db.SaveChanges();
            return commom.GetModuleFromJarray(null, true, "新增成功");
        }

        //修改
        [HttpPost]
        [ApiAuthorize]
        public JObject UpdateKPIIndicators([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            KPI_Indicators indicators = obj.indicators;
            string UserName = obj.UserName;
            var info = db.KPI_Indicators.Find(indicators.Id);
            if (indicators.Department != info.Department || indicators.Group != info.Group || indicators.IndicatorsType != info.IndicatorsType)
            {
                var check = db.KPI_Indicators.Count(c => c.Department == indicators.Department && c.Group == indicators.Group && c.IndicatorsName == indicators.IndicatorsName && c.ExecutionTime == indicators.ExecutionTime && c.IndicatorsType == indicators.IndicatorsType);
                if (check != 0)
                {
                    return commom.GetModuleFromJarray(null, false, "已有重复数据");
                }
            }
            if (info.ExecutionTime != indicators.ExecutionTime)
            {
                db.KPI_Indicators.Add(indicators);
                db.SaveChanges();
                return commom.GetModuleFromJarray(null, true, "修改成功");
            }
            else
            {
                UserOperateLog log = new UserOperateLog() { Operator = UserName, OperateDT = DateTime.Now, OperateRecord = "数据来源修改" + info.Department + "->" + indicators.Department + "," + info.Group + "->" + indicators.Group + "," + info.IndicatorsDefine + "->" + indicators.IndicatorsDefine + "," + info.IndicatorsName + "->" + indicators.IndicatorsName + "," + info.IndicatorsStatue + "->" + indicators.IndicatorsStatue + "," + info.IndicatorsTimeSpan + "->" + indicators.IndicatorsTimeSpan + "," + info.ComputationalFormula + "->" + indicators.ComputationalFormula + "," + info.Weight + "->" + indicators.Weight + "," + info.IndicatorsValue + "->" + indicators.IndicatorsValue + "," + info.IndicatorsValueUnit + "->" + indicators.IndicatorsValueUnit + "," + info.DataName + "->" + indicators.DataName + "," + info.Cycle + "->" + indicators.Cycle + "," + info.SourceDepartment + "->" + indicators.SourceDepartment + "," + info.DataInputor + "->" + indicators.DataInputor + "," + info.DataInputTime + "->" + indicators.DataInputTime + "," + info.IndicatorsType + "->" + indicators.IndicatorsType + "," + info.QualityStatue + "->" + indicators.QualityStatue };
                info.Department = indicators.Department;
                info.Group = indicators.Group;
                info.IndicatorsDefine = indicators.IndicatorsDefine;
                info.IndicatorsName = indicators.IndicatorsName;
                info.IndicatorsStatue = indicators.IndicatorsStatue;
                info.IndicatorsTimeSpan = indicators.IndicatorsTimeSpan;
                info.ComputationalFormula = indicators.ComputationalFormula;
                info.Weight = indicators.Weight;
                info.IndicatorsValue = indicators.IndicatorsValue;
                info.IndicatorsValueUnit = indicators.IndicatorsValueUnit;
                info.DataName = indicators.DataName;
                info.Cycle = indicators.Cycle;
                info.SourceDepartment = indicators.SourceDepartment;
                info.DataInputor = indicators.DataInputor;
                info.DataInputTime = indicators.DataInputTime;
                info.Section = indicators.Section;
                info.Process = indicators.Process;
                info.IndicatorsType = indicators.IndicatorsType;
                info.QualityStatue = indicators.QualityStatue;
                db.UserOperateLog.Add(log);
                db.SaveChanges();


                return commom.GetModuleFromJarray(null, true, "修改成功");
            }
        }

        //删除 
        [HttpPost]
        [ApiAuthorize]
        public void DeleteKPIIndicators([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            int id = obj.id;
            string UserName = obj.UserName;
            var deleteinfo = db.KPI_Indicators.Find(id);
            UserOperateLog log = new UserOperateLog() { Operator = UserName, OperateDT = DateTime.Now, OperateRecord = "删除数据来源表" + deleteinfo.Department + deleteinfo.Group + deleteinfo.IndicatorsType };
            db.KPI_Indicators.Remove(deleteinfo);
            db.UserOperateLog.Add(log);
            db.SaveChanges();
        }

        //导出到EXCEL表


        #endregion

        #region 品质效率实际记录
        //显示
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayActualRecord([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string ordernum = datavalue.ordernum;
            string deparment = datavalue.deparment;
            string group = datavalue.group;
            string process = datavalue.process;
            string section = datavalue.section;
            DateTime? time = datavalue.time;

            JArray result = new JArray();
            var totalvalue = db.KPI_ActualRecord.ToList();
            var kpiinde = db.KPI_Indicators.ToList();
            if (!string.IsNullOrEmpty(ordernum))
            {
                totalvalue = totalvalue.Where(c => c.OrderNum == ordernum).ToList();
            }
            if (!string.IsNullOrEmpty(deparment))
            {
                totalvalue = totalvalue.Where(c => c.Department == deparment).ToList();
                kpiinde = kpiinde.Where(c => c.Department == deparment).ToList();
            }
            if (!string.IsNullOrEmpty(group))
            {
                totalvalue = totalvalue.Where(c => c.Group == group).ToList();
                kpiinde = kpiinde.Where(c => c.Group == group).ToList();
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
                var timecheck = db.KPI_Indicators.OrderByDescending(c => c.ExecutionTime).Where(c => c.ExecutionTime <= time).Select(c => c.ExecutionTime).FirstOrDefault();
                totalvalue = totalvalue.Where(c => c.ActualTime >= time && c.ActualTime < time.Value.AddMonths(1)).ToList();
                kpiinde = kpiinde.Where(c => c.ExecutionTime == time).ToList();
            }
            foreach (var item in totalvalue)
            {
                JObject obj = new JObject();
                obj.Add("ID", item.Id); //订单
                obj.Add("OrderNum", item.OrderNum); //订单
                obj.Add("Department", item.Department);//部门
                obj.Add("Group", item.Group);//班组
                obj.Add("Process", item.Process);//工段
                obj.Add("Section", item.Section);//工序
                obj.Add("IndicatorsType", item.IndicatorsType);//品质或者效率
                var info = kpiinde.Select(c => c.IndicatorsValueUnit).FirstOrDefault();
                obj.Add("ActualNormalNum", item.ActualNormalNum.ToString());//正常数量
                obj.Add("ActualAbnormalNum", item.ActualAbnormalNum);//异常数量
                obj.Add("ActualTime", item.ActualTime.ToString());//异常数量
                obj.Add("Message", item.ActualAbnormalDescription);//异常信息
                obj.Add("ActualCreateor", item.ActualCreateor);//录入人员
                result.Add(obj);
            }
            return commom.GetModuleFromJarray(result);
        }

        //新增
        [HttpPost]
        [ApiAuthorize]
        public JObject AddActualRecord([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            List<KPI_ActualRecord> actualRecord = obj.actualRecord;
            string Role = obj.Role;
            string UserName = obj.UserName;
            int UserNum = obj.UserNum;

            string error = "";
            foreach (var item in actualRecord)
            {
                //拿到正确版本时间
                var version = db.KPI_Indicators.Where(c => c.ExecutionTime <= DateTime.Now).Select(c => c.ExecutionTime).Distinct().ToList();
                var exetime = version.Max();
                #region  判断时间是否正确
                //var timetip = db.KPI_Indicators.Where(c => c.Department == item.Department && c.Group == item.Group && c.IndicatorsType == item.IndicatorsType&&c.ExecutionTime== exetime).Select(c => c.DataInputTime).FirstOrDefault();
                //string[] spitinfo = timetip.Split('/');
                //if (spitinfo[0] == "天")
                //{
                //    int day = int.Parse(spitinfo[1]) - 1;
                //    var beforday = DateTime.Now.AddDays(-day).Date;
                //    if (item.ActualTime != beforday)
                //    {
                //        return "录入时间与定义的时间不符合,请确认时间准确";
                //    }
                //    var time = spitinfo[2];
                //    var hourse = int.Parse(time.Split(':')[0]);
                //    var minute = int.Parse(time.Split(':')[1]);
                //    if (DateTime.Now.Hour > hourse)
                //    {
                //        return "录入时间与定义的时间不符合,请确认时间准确";
                //    }
                //    else if (DateTime.Now.Hour == hourse && DateTime.Now.Minute > minute)
                //    {
                //        return "录入时间与定义的时间不符合,请确认时间准确";
                //    }
                //}
                //else if (spitinfo[0] == "月")
                //{
                //    int mounte = int.Parse(spitinfo[1]) - 1;
                //    var beforday = DateTime.Now.AddMonths(-mounte).Date;
                //    if (item.ActualTime.Value.Month != beforday.Month || item.ActualTime.Value.Year != beforday.Year)
                //    {
                //        return "录入时间与定义的时间不符合,请确认时间准确";
                //    }
                //    var day = int.Parse(spitinfo[2]);
                //    var beforday2 = beforday.AddDays(-day);
                //    if (item.ActualTime < beforday2)
                //    {
                //        return "录入时间与定义的时间不符合,请确认时间准确";
                //    }
                //}
                #endregion
                #region 判断录入人员是否正确
                if (Role != "系统管理员")
                {
                    var sourceName = db.KPI_Indicators.Where(c => c.Department == item.Department && c.Group == item.Group && c.IndicatorsType == item.IndicatorsType && c.ExecutionTime == exetime).Select(c => c.DataInputor).FirstOrDefault();
                    var sourceUserID = db.KPI_Indicators.Where(c => c.Department == item.Department && c.Group == item.Group && c.IndicatorsType == item.IndicatorsType && c.ExecutionTime == exetime).Select(c => c.SourceDepartment).FirstOrDefault();
                    if (sourceName == null)
                    {
                        return commom.GetModuleFromJobjet(null, false, "找不到" + item.Department + item.Group + "录入人员信息,请确定部门班组的准确,注意大小写和中英文");
                    }
                    if (sourceUserID == null)
                    {
                        return commom.GetModuleFromJobjet(null, false, "找不到" + item.Department + item.Group + "录入人员信息,请确定部门班组的准确,注意大小写和中英文");
                    }
                    var namelist = sourceName.Split('/');
                    var idList = sourceUserID.Split('/');
                    int[] idintList = Array.ConvertAll<string, int>(idList, delegate (string s) { return int.Parse(s); });
                    var username = UserName;
                    var userid = UserNum;

                    if (!namelist.Contains(username) || !idintList.Contains(userid))
                    {
                        return commom.GetModuleFromJobjet(null, false, "登录账号不在录入人员列表当中,请确认录入信息");
                    }
                }
                #endregion
                var info = db.KPI_ActualRecord.Count(c => c.ActualTime == item.ActualTime && c.OrderNum == item.OrderNum && c.Department == item.Department && c.Group == item.Group && c.Process == item.Process && c.Section == item.Section && c.IndicatorsType == item.IndicatorsType);
                if (info != 0)
                {
                    error = error + item.OrderNum + "在" + item.ActualTime + "已有" + item.Department + item.Group + "的生产记录.";
                }

            }
            if (string.IsNullOrEmpty(error))
            {
                actualRecord.ForEach(c => { c.ActualCreateor = UserName; c.ActualCreateTime = DateTime.Now; });
                db.KPI_ActualRecord.AddRange(actualRecord);
                db.SaveChanges();
                return commom.GetModuleFromJobjet(null, true, "新增成功");
            }
            else
            {
                return commom.GetModuleFromJobjet(null, false, error);
            }

        }

        //修改,时间不能修改
        [HttpPost]
        [ApiAuthorize]
        public JObject UpdateActualRecord([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            int id = obj.id;
            string UserName = obj.UserName;
            string Role = obj.Role;
            KPI_ActualRecord actualRecord = obj.actualRecord;

            var info = db.KPI_ActualRecord.Find(id);
            JObject result = new JObject();
            if (info == null)
            {
                result.Add("mes", "没有找到数据");
                result.Add("pass", false);
                return commom.GetModuleFromJobjet(null, false, "没有找到数据");
            }
            if (UserName != info.ActualCreateor && Role != "系统管理员")
            {
                result.Add("mes", "你没有权限删除该条信息");
                result.Add("pass", false);
                return commom.GetModuleFromJobjet(null, false, "你没有权限删除该条信息");

            }
            info.OrderNum = actualRecord.OrderNum;
            info.Department = actualRecord.Department;
            info.Group = actualRecord.Group;
            info.Process = actualRecord.Process;
            info.Section = actualRecord.Section;
            info.IndicatorsType = actualRecord.IndicatorsType;
            info.ActualNormalNum = actualRecord.ActualNormalNum;
            info.ActualAbnormalNum = actualRecord.ActualAbnormalNum;
            info.ActualTime = actualRecord.ActualTime;
            info.ActualAbnormalDescription = actualRecord.ActualAbnormalDescription;

            db.SaveChanges();
            result.Add("mes", "修改成功");
            result.Add("pass", true);
            return commom.GetModuleFromJobjet(null, true, "修改成功");

        }

        //删除
        [HttpPost]
        [ApiAuthorize]
        public JObject DeleteActualRecord([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            List<int> id = datavalue.id;
            string UserName = datavalue.UserName;
            string Role = datavalue.Role;

            JObject result = new JObject();
            var info = db.KPI_ActualRecord.Where(c => id.Contains(c.Id)).ToList();
            if (info.Count == 0)
            {
                return commom.GetModuleFromJobjet(null, false, "找不到该信息");
            }
            var cretor = info.Select(c => c.ActualCreateor).Distinct().ToList();
            if (cretor.Count > 1 || !cretor.Contains(UserName) && Role != "系统管理员")
            {
                return commom.GetModuleFromJobjet(null, false, "没有权限删除信息,信息的创建人员并不全是你");
            }
            db.KPI_ActualRecord.RemoveRange(info);
            db.SaveChanges();
            UserOperateLog log = new UserOperateLog()
            {
                OperateDT = DateTime.Now,
                Operator = UserName,
                OperateRecord = "删除生产/送检数据" + string.Join(" ", info.Select(c => c.Department).Distinct().ToList()) + string.Join(" ", info.Select(c => c.Group).Distinct().ToList())
            };
            db.UserOperateLog.Add(log);
            db.SaveChanges();
            return commom.GetModuleFromJobjet(null, true, "删除成功");
        }

        #endregion

        #region---- 查找所有部门班组工段工序对应json
        [HttpPost]
        [ApiAuthorize]
        public JObject Relation()
        {
            JArray result = new JArray();
            var tempvalue = db.Plan_SectionParameter.Select(c => new TempIndicators { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section });
            var deparment = tempvalue.Select(c => c.Department).Distinct().ToList();
            foreach (var depitem in deparment)//循环部门
            {
                JObject dep = new JObject();
                dep.Add("Department", depitem);
                var group = tempvalue.Where(c => c.Department == depitem).Select(c => c.Group).Distinct().ToList();
                JArray groupArray = new JArray();
                foreach (var groupitem in group)//循环班组
                {
                    JObject groupobj = new JObject();
                    groupobj.Add("Group", groupitem);
                    var seaction = tempvalue.Where(c => c.Department == depitem && c.Group == groupitem).Select(c => c.Section).Distinct().ToList();
                    JArray seaArray = new JArray();
                    foreach (var sea in seaction)//循环工段
                    {
                        JObject seaobj = new JObject();
                        seaobj.Add("Section", sea);
                        var process = tempvalue.Where(c => c.Department == depitem && c.Group == groupitem && c.Section == sea).Select(c => c.Process).Distinct().ToList();
                        JArray proArray = new JArray();
                        foreach (var pro in process)//循环工序
                        {
                            JObject proobj = new JObject();
                            proobj.Add("Process", pro);
                            proArray.Add(proobj);
                        }
                        seaobj.Add("TProcess", proArray);
                        seaArray.Add(seaobj);
                    }
                    groupobj.Add("TSection", seaArray);
                    groupArray.Add(groupobj);
                }
                dep.Add("TGroup", groupArray);
                result.Add(dep);
            }

            return commom.GetModuleFromJarray(result);
        }

        #endregion

        #region 效率/品质日报表
        //效率效率指标显示
        [HttpPost]
        [ApiAuthorize]
        public JObject DiasplyEfficiency([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            DateTime time = datavalue.time;
            string stuta = datavalue.stuta;
            string deparment = datavalue.deparment;
            List<string> group = datavalue.group;

            JObject result = new JObject();
            //拿到对的版本
            var version = db.KPI_Indicators.Where(c => c.ExecutionTime.Value.Year <= time.Year && c.ExecutionTime.Value.Month <= time.Month).Select(c => c.ExecutionTime).Distinct().ToList();
            if (version != null)
            {
                //取值
                var exetime = version.Max();
                var total = db.KPI_Indicators.Where(c => c.ExecutionTime == exetime && c.IndicatorsType == stuta).Select(c => new TempIndicators { Department = c.Department, Group = c.Group, Cycle = c.Cycle, IndicatorsValueUnit = c.IndicatorsValueUnit, IndicatorsName = c.IndicatorsName, IndicatorsValue = c.IndicatorsValue, SourceDepartment = c.DataInputor, Process = c.Process, Section = c.Section, IndicatorsStatue = c.IndicatorsStatue }).ToList();

                var PlanValue = db.Plan_FromKPI.Where(c => c.PlanTime.Value.Year == time.Year).Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.PlanTime, Num = c.PlanNum, Types = c.IndicatorsType, CheckDepartment = c.CheckDepartment, CheckGroup = c.CheckGroup, CheckProcess = c.CheckProcess, CheckSection = c.CheckSection }).ToList();

                var ActualValue = db.KPI_ActualRecord.Where(c => c.IndicatorsType == stuta && c.ActualTime.Value.Year == time.Year).Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.ActualTime, Num = c.ActualNormalNum, AbNum = c.ActualAbnormalNum, Types = c.IndicatorsType }).ToList();
                if (!string.IsNullOrEmpty(deparment))
                {
                    total = total.Where(c => c.Department == deparment).ToList();
                }
                if (group != null && group.Count != 0)
                {
                    total = total.Where(c => group.Contains(c.Group)).ToList();
                }


                //拿到日数据
                JArray DayArray = new JArray();
                var depar = total.Where(c => c.Cycle == "天").Select(c => c.Department).Distinct();
                foreach (var dep in depar)
                {
                    var d_group = total.Where(c => c.Cycle == "天" && c.Department == dep).Select(c => c.Group).Distinct();
                    foreach (var groupitem in d_group)
                    {
                        var dayvalue = total.Where(c => c.Cycle == "天" && c.Department == dep && c.Group == groupitem).FirstOrDefault();
                        JObject obj = new JObject();
                        obj.Add("Department", dayvalue.Department);
                        obj.Add("Group", dayvalue.Group);
                        obj.Add("IndicatorsName", dayvalue.IndicatorsName);
                        obj.Add("IndicatorsValue", dayvalue.IndicatorsValue + dayvalue.IndicatorsValueUnit);
                        obj.Add("SourceDepartment", dayvalue.SourceDepartment);
                        JArray actualvalue = new JArray();//实际数据
                        int PlanTotalValue = 0;
                        int ActualTotalValue = 0;
                        List<decimal> oneScore = new List<decimal>();
                        var totalday = DateTime.DaysInMonth(time.Year, time.Month);
                        for (var i = 1; i <= totalday; i++)
                        {
                            //每天的日期
                            var Ftime = new DateTime(time.Year, time.Month, i);

                            var plancount = 0;
                            var Mkplancount = 0;
                            var actualcount = 0;
                            var Mkactualcount = 0;
                            var sectionlist3 = new List<string>();
                            sectionlist3.Add("送检");//没有找计划拿到工段工序,自己给一个用于在生产/送检拿到数据

                            /*
                             * 计算方式:
                             * 1.目标值单位是以笔/次/单.效率计算=生产的异常数,品质计算=品质的异常数
                             * 2.目标值单位是%,计划不为空, 效率计算=生产数/计划数,品质计算=品质正常数/送检总数
                             * 3.目标值单位是%,计划为空,部门为品质部,效率计算=(模块生产正常数/模块生产总数+模组生产正常数/模组生产总数)/2,品质计算=品质正常数/送检总数
                             * 4.目标值单位是%,计划为空.部门除品质部外,在计划中能找到所负责被检验部门班组的,效率计算=检验数/生产数,品质计算=品质正常数/送检总数
                             * 5.目标值单位是%,计划为空.部门除品质部外,在计划中能找不到所负责被检验部门班组的,判定为单纯没做计划,计算方式,效率计算=生产数/计划数,品质计算=品质正常数/送检总数
                             */
                            //数据来源表目标值单位不是%以笔/单/次,这种一般都是没有计划,直接拿生产表中的异常数量
                            if (dayvalue.IndicatorsValueUnit != "%")
                            {
                                //生产表中的异常数量
                                var actual = ActualValue.Where(c => c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime && c.Types == stuta).Select(c => c.AbNum).ToList();
                                actualcount = actual.Count() != 0 ? actual.Sum() : 0;
                                //没有计划
                                PlanTotalValue = PlanTotalValue + plancount;
                                ActualTotalValue = ActualTotalValue + actualcount;
                                actualvalue.Add(actualcount.ToString());
                            }
                            else
                            {
                                if (stuta == "效率指标")
                                {
                                    var planlist = PlanValue.Where(c => c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime).ToList();
                                    plancount = planlist.Count() == 0 ? 0 : planlist.Sum(c => c.Num);
                                    //效率指标,目标值单位是%.但没有计划
                                    if (planlist.Count() != 0)
                                    {
                                        var sectionlist = new List<string>();
                                        var distinSection = planlist.Select(c => new { c.Section, c.Process }).Distinct().ToList();
                                        distinSection.ForEach(c => sectionlist.Add(c.Section + c.Process));
                                        //计划不为空,并且单位是以%,计划数为分母,生产数为分子
                                        actualcount = commom.AcuteNum3(dep, groupitem, sectionlist, dayvalue.IndicatorsStatue, stuta, Ftime);
                                    }
                                    else
                                    {
                                        if (dayvalue.Department == "品质部")
                                        {
                                            actualcount = commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, "正常", "效率指标", Ftime, "模组");
                                            Mkactualcount = commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, "正常", "效率指标", Ftime, "模块");
                                            plancount = commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, "", "效率指标", Ftime, "模组");
                                            Mkplancount = commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, "", "效率指标", Ftime, "模块");
                                        }
                                        else
                                        {
                                            //拿到检验部门所负责的部门班组列表
                                            var departmentList = PlanValue.Where(c => c.CheckDepartment == dayvalue.Department && c.CheckGroup == dayvalue.Group && c.Time.Value == Ftime).Select(c => new { c.Department, c.Group }).Distinct().ToList();

                                            //计划为空,单位是以%,并且没有有所负责的部门班组列表 ,单纯没做计划,直接显示生产信息
                                            if (departmentList.Count() == 0)
                                            {
                                                actualcount = commom.AcuteNum3(dep, groupitem, sectionlist3, dayvalue.IndicatorsStatue, stuta, Ftime);
                                            }
                                            //计划为空,单位是以%,并且有所负责的部门班组列表 ,一般为检验部门,检验部门一般不做计划 效率指标计算是检验数/生产数  品质指标是不漏件数/检验数
                                            else
                                            {
                                                foreach (var onedep in departmentList)
                                                {
                                                    var sectionlist = new List<string>();
                                                    var sectionlist2 = PlanValue.Where(c => c.Department == onedep.Department && c.Group == onedep.Group && c.Time.Value == Ftime).Select(c => new { c.Section, c.Process }).Distinct().ToList();
                                                    sectionlist2.ForEach(c => sectionlist.Add(c.Section + c.Process));

                                                    //拿到所负责部门班组列表的生产正常记录,用于做分母
                                                    plancount = plancount + commom.AcuteNum3(onedep.Department, onedep.Group, sectionlist, "正常", stuta, Ftime);
                                                }
                                                //拿到所负责部门班组列表的品质记录,正常+异常,用于做分子

                                                actualcount = actualcount + commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, "", "品质指标", Ftime);

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    plancount = commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, "", "品质指标", Ftime);
                                    actualcount = commom.AcuteNum3(dayvalue.Department, dayvalue.Group, sectionlist3, dayvalue.IndicatorsStatue, "品质指标", Ftime);
                                }

                                PlanTotalValue = PlanTotalValue + plancount + Mkplancount;
                                ActualTotalValue = ActualTotalValue + actualcount + Mkactualcount;
                                if (Mkactualcount != 0 || Mkplancount != 0)
                                {
                                    var mz = plancount == 0 ? 0 : (actualcount * 100 / plancount);
                                    var mk = Mkplancount == 0 ? 0 : (Mkactualcount * 100 / Mkplancount);
                                    var value = mz == 0 ? mk : (mk == 0 ? mz : Math.Round((decimal)(mk + mz) / 2, 2));

                                    if (value != 0)
                                    { oneScore.Add(value); }
                                    actualvalue.Add(value + "% 模组:" + actualcount + "/" + plancount + " 模块:" + Mkactualcount + "/" + Mkplancount);
                                }
                                else
                                {
                                    actualvalue.Add(plancount == 0 ? "0% " + actualcount.ToString() + "/0" : Math.Round((decimal)actualcount * 100 / plancount, 2) + "% " + actualcount + "/" + plancount);
                                }
                            }
                        }
                        obj.Add("ActualValue", actualvalue);//每月实际数
                        obj.Add("PlanTotalValue", PlanTotalValue);//月总计划数
                        obj.Add("ActualTotalValue", ActualTotalValue);//月总实际数
                        double difference = 0;
                        if (dayvalue.IndicatorsValueUnit == "%")
                        {
                            double score = 0;
                            if (oneScore.Count != 0)
                            {
                                score = Math.Round((double)oneScore.Sum() / oneScore.Count(), 2);
                            }
                            else
                            {
                                score = PlanTotalValue == 0 ? 0 : Math.Round((double)ActualTotalValue * 100 / PlanTotalValue, 2);
                            }
                            obj.Add("ActualScore", score + "%");//实际得分
                            if (dayvalue.IndicatorsStatue == "正常")
                            {
                                difference = Math.Round(score - dayvalue.IndicatorsValue, 2);
                            }
                            else
                            {
                                difference = Math.Round(dayvalue.IndicatorsValue - score, 2);
                            }

                        }
                        else
                        {
                            if (dayvalue.IndicatorsStatue == "正常")
                            {
                                difference = Math.Round(ActualTotalValue - dayvalue.IndicatorsValue, 2);
                            }
                            else
                            {
                                difference = Math.Round(dayvalue.IndicatorsValue - ActualTotalValue, 2);
                            }
                            obj.Add("ActualScore", difference >= 0 ? "100%" : "0%");//实际得分
                        }
                        obj.Add("DifferencesValue", difference);//与目标值差异
                        obj.Add("Goal", difference >= 0 ? 100 : 0);//得分小计
                        DayArray.Add(obj);
                    }
                }
                result.Add("DayArray", DayArray);

                //拿到月数据
                JArray MouthArray = new JArray();
                var deparment2 = total.Where(c => c.Cycle == "月").Select(c => c.Department).Distinct().ToList();
                foreach (var dep in deparment2)
                {
                    var d_group = total.Where(c => c.Cycle == "月" && c.Department == dep).Select(c => c.Group).Distinct().ToList();
                    foreach (var groupitem in d_group)
                    {
                        var dayvalue = total.Where(c => c.Cycle == "月" && c.Department == dep && c.Group == groupitem).FirstOrDefault();
                        JObject obj = new JObject();
                        obj.Add("Department", dayvalue.Department);
                        obj.Add("Group", dayvalue.Group);
                        obj.Add("IndicatorsName", dayvalue.IndicatorsName);
                        obj.Add("IndicatorsValue", dayvalue.IndicatorsValue + dayvalue.IndicatorsValueUnit);
                        obj.Add("SourceDepartment", dayvalue.SourceDepartment);
                        JArray planvalue = new JArray(); //计划数据
                        JArray actualvalue = new JArray();//实际数据
                        int PlanTotalValue = 0;
                        int ActualTotalValue = 0;
                        for (var i = 1; i <= 12; i++)
                        {
                            var Ftime = new DateTime(time.Year, i, 1);
                            /*
                             * 计算方式
                             * 1.目标值单位是以笔/次/单.效率计算=生产的异常数,品质计算=品质的异常数
                             * 2.目标值单位是%,计划不为空, 效率计算=生产数/计划数,品质计算=品质正常数/送检总数
                             * 3.目标值单位是%,计划为空,判定为单纯没做计划,计算方式,效率计算=生产数/计划数,品质计算=品质正常数/送检总数
                             */
                            //计划
                            var plancount = 0;
                            var actualcount = 0;
                            if (dayvalue.IndicatorsValueUnit != "%")
                            {
                                //生产表中的异常数量
                                var actual = ActualValue.Where(c => c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime && c.Types == stuta).Select(c => c.AbNum).ToList();
                                actualcount = actual.Count() != 0 ? actual.Sum() : 0;
                                //没有计划
                                PlanTotalValue = PlanTotalValue + plancount;
                                ActualTotalValue = ActualTotalValue + actualcount;
                                actualvalue.Add(actualcount.ToString());
                            }
                            else
                            {
                                var planlist = PlanValue.Where(c => c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime).ToList();
                                plancount = planlist.Count() == 0 ? 0 : planlist.Sum(c => c.Num);

                                //版本2
                                if (planlist.Count() != 0)
                                {
                                    var sectionlist = new List<string>();
                                    planlist.ForEach(c => sectionlist.Add(c.Section + c.Process));
                                    actualcount = commom.AcuteNum3(dep, groupitem, sectionlist, dayvalue.IndicatorsStatue, stuta, Ftime);
                                }
                                else
                                {
                                    var sectionlist3 = new List<string>();
                                    sectionlist3.Add("送检");
                                    actualcount = commom.AcuteNum3(dep, groupitem, sectionlist3, dayvalue.IndicatorsStatue, stuta, Ftime);
                                }
                                PlanTotalValue = PlanTotalValue + plancount;
                                ActualTotalValue = ActualTotalValue + actualcount;
                                planvalue.Add(plancount);
                                actualvalue.Add(plancount == 0 ? actualcount.ToString() : Math.Round((decimal)actualcount * 100 / plancount, 2) + "% " + actualcount + "/" + plancount);
                            }
                        }
                        obj.Add("ActualValue", actualvalue);//每年实际数
                        obj.Add("PlanTotalValue", PlanTotalValue);//年总计划数
                        obj.Add("ActualTotalValue", ActualTotalValue);//年总实际数

                        double difference = 0;
                        if (dayvalue.IndicatorsValueUnit == "%")
                        {
                            obj.Add("ActualScore", PlanTotalValue == 0 ? "0%" : Math.Round((decimal)ActualTotalValue * 100 / PlanTotalValue, 2) + "%");//实际得分
                            if (dayvalue.IndicatorsStatue == "正常")
                            {
                                difference = (PlanTotalValue == 0 ? 0 : (ActualTotalValue * 100 / PlanTotalValue)) - dayvalue.IndicatorsValue;
                            }
                            else
                            {
                                difference = PlanTotalValue == 0 ? 0 : dayvalue.IndicatorsValue - (ActualTotalValue * 100 / PlanTotalValue);
                            }

                        }
                        else
                        {
                            obj.Add("ActualScore", ActualTotalValue);//实际得分
                            if (dayvalue.IndicatorsStatue == "正常")
                            {
                                difference = ActualTotalValue - dayvalue.IndicatorsValue;
                            }
                            else
                            {
                                difference = dayvalue.IndicatorsValue - ActualTotalValue;
                            }
                        }
                        obj.Add("DifferencesValue", difference);//与目标值差异
                        obj.Add("Goal", difference >= 0 ? 100 : 0);//得分小计
                        MouthArray.Add(obj);
                    }
                }
                result.Add("MouthArray", MouthArray);

            }
            return commom.GetModuleFromJobjet(result);
        }
        #endregion

        #region---KPI班组流失率

        #region
        //public class ExportTurnoverCheckSshift
        //{
        //    public string Department { get; set; }//部门
        //    public string Group { get; set; }//班组       
        //    public string IndicatorsName { get; set; }//指标名称
        //    public double IndicatorsValue { get; set; }//目标值
        //    public string SourceDepartment { get; set; }//数据来源部门
        //    public int BeginNumber { get; set; }//月初人数
        //    public int EndNumber { get; set; }//月末人数
        //    public int AverageNum { get; set; }//平均人数

        //    #region--天数1-31
        //    public int Day1 { get; set; }
        //    public int Day2 { get; set; }
        //    public int Day3 { get; set; }
        //    public int Day4 { get; set; }
        //    public int Day5 { get; set; }
        //    public int Day6 { get; set; }
        //    public int Day7 { get; set; }
        //    public int Day8 { get; set; }
        //    public int Day9 { get; set; }
        //    public int Day10 { get; set; }
        //    public int Day11 { get; set; }
        //    public int Day12 { get; set; }
        //    public int Day13 { get; set; }
        //    public int Day14 { get; set; }
        //    public int Day15 { get; set; }
        //    public int Day16 { get; set; }
        //    public int Day17 { get; set; }
        //    public int Day18 { get; set; }
        //    public int Day19 { get; set; }
        //    public int Day20 { get; set; }
        //    public int Day21 { get; set; }
        //    public int Day22 { get; set; }
        //    public int Day23 { get; set; }
        //    public int Day24 { get; set; }
        //    public int Day25 { get; set; }
        //    public int Day26 { get; set; }
        //    public int Day27 { get; set; }
        //    public int Day28 { get; set; }
        //    public int Day29 { get; set; }
        //    public int Day30 { get; set; }
        //    public int Day31 { get; set; }

        //    #endregion

        //    public int TurnoverMonth { get; set; }//月流失人数
        //    public double Actual { get; set; }//实际得分
        //    public double Differences { get; set; }//与目标值差异
        //    public double Subtotal { get; set; }//得分小计
        //}

        #endregion

        //批量上传班组流失率
        [HttpPost]
        [ApiAuthorize]
        public JObject DeparturesNum([System.Web.Http.FromBody]JObject data)
        {
            JArray result = new JArray();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            DateTime dateLoss = obj.dateLoss;
            List<KPI_TurnoverRate> turnoverRates = (List<KPI_TurnoverRate>)JsonHelper.jsonDes<List<KPI_TurnoverRate>>(data["turnoverRates"].ToString());
            if (turnoverRates.Count > 0 && dateLoss != null && turnoverRates.FirstOrDefault().Department != null && turnoverRates.FirstOrDefault().Group != null)
            {
                foreach (var item in turnoverRates)
                {
                    item.Createor = auth.UserName;
                    item.CreateTime = DateTime.Now;
                    if (db.KPI_TurnoverRate.Count(c => c.Department == item.Department && c.Group == item.Group && c.DateLoss == dateLoss) > 0)
                    {
                        result.Add(item.Department + "," + item.Group + "," + dateLoss + "的记录重复了" + ";");
                    }
                }
                if (result.Count > 0)
                {
                    return commom.GetModuleFromJarray(result, false, "数据重复");
                }
                foreach (var it in turnoverRates)
                {
                    it.DateLoss = dateLoss;
                    db.SaveChanges();
                }
                db.KPI_TurnoverRate.AddRange(turnoverRates);
                int count = db.SaveChanges();
                if (count > 0)
                {
                    return commom.GetModuleFromJarray(result, true, "添加成功");
                }
                else
                {
                    return commom.GetModuleFromJarray(result, false, "添加失败");
                }
            }
            return commom.GetModuleFromJarray(result, false, "添加失败");
        }

        //查询班组人员流失率汇总表
        [HttpPost]
        [ApiAuthorize]
        public JObject Turnover_CheckSshift([System.Web.Http.FromBody]JObject data)
        {
            JObject Rate = new JObject();
            JArray result = new JArray();
            JObject retul = new JObject();
            JArray meg = new JArray();
            JObject tall = new JObject();
            JArray chek = new JArray();
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            int? year = obj.year;
            int? month = obj.month;
            var checkList = db.KPI_TurnoverRate.ToList();
            List<KPI_TurnoverRate> Turnover = new List<KPI_TurnoverRate>();
            double averageNum = 0;//平均人数
            double turnoverMonth = 0;//月流失人数
            double actual = 0;//实际得分
            double differences = 0;//与目标值的差异
            double subtotal = 0;//得分小计
            if (year != 0 && month != 0)
            {
                Turnover = checkList.Where(c => c.DateLoss.Year == year && c.DateLoss.Month == month).ToList();
            }
            if (year != 0 && month == 0)
            {
                Turnover = checkList.Where(c => c.DateLoss.Year == year).ToList();
            }
            if (Turnover.Count > 0)
            {
                var checkList1 = Turnover.Select(c => c.Department).Distinct().ToList();
                foreach (var ite in checkList1)
                {
                    var group = Turnover.Where(c => c.Department == ite).Select(c => c.Group).Distinct().ToList();
                    foreach (var it in group)
                    {
                        var rate = Turnover.Where(c => c.Department == ite && c.Group == it).Distinct().FirstOrDefault();
                        //Id
                        Rate.Add("Id", rate.Id == 0 ? 0 : rate.Id);
                        //被考核部门
                        Rate.Add("Department", ite == null ? null : ite);
                        //班组
                        Rate.Add("Group", it == null ? null : it);
                        //指标名称                      
                        Rate.Add("IndicatorsName", "班组流失率");
                        //目标值
                        Rate.Add("IndicatorsValue", rate.IndicatorsValue);
                        //数据来源部门
                        Rate.Add("SourceDepartment", "人力资源部");
                        //月初人数
                        Rate.Add("BeginNumber", rate.BeginNumber == 0 ? 0 : rate.BeginNumber);
                        //月末人数
                        var endNumber = Turnover.Where(c => c.Department == ite && c.Group == it).OrderByDescending(c => c.Id).Select(c => c.EndNumber).FirstOrDefault();
                        Rate.Add("EndNumber", endNumber == 0 ? 0 : endNumber);
                        //平均人数                     
                        double num = rate.BeginNumber + endNumber;
                        averageNum = Math.Ceiling(num / 2);
                        Rate.Add("AverageNum", averageNum == 0 ? 0 : averageNum);
                        Rate.Add("Year", rate.DateLoss.Year);
                        Rate.Add("Month", rate.DateLoss.Month);
                        DateTime dt = rate.DateLoss;
                        int sumday = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1).Day;//一个月有多少天     
                        for (var i = 1; i <= sumday; i++)
                        {
                            var ifno = Turnover.Where(c => c.Department == ite && c.Group == it && c.DateLoss.Day == i).Select(c => c.LossNumber).FirstOrDefault();
                            //流失日期
                            tall.Add("DateLoss", i);
                            //流失人数
                            tall.Add("LossNumber", ifno == 0 ? 0 : ifno);
                            turnoverMonth = turnoverMonth + ifno;
                            meg.Add(tall);
                            tall = new JObject();
                        }
                        Rate.Add("Rate", meg);
                        meg = new JArray();
                        actual = actual + (turnoverMonth / averageNum);
                        differences = (rate.IndicatorsValue == 0 ? 0 : actual - rate.IndicatorsValue);
                        if (rate.IndicatorsValue != 0)
                        {
                            if (actual <= rate.IndicatorsValue)
                            {
                                subtotal = 100;

                            }
                            else if (actual > rate.IndicatorsValue)
                            {
                                subtotal = 100 - (actual - rate.IndicatorsValue);
                            }
                        }
                        //月流失人数
                        Rate.Add("TurnoverMonth", turnoverMonth == 0 ? 0 : turnoverMonth);
                        turnoverMonth = new int();
                        //月流失率（实际得分）
                        Rate.Add("Actual", double.Parse(actual.ToString("0.00")));
                        actual = new double();
                        //与目标值的差异    
                        Rate.Add("Differences", double.Parse(differences.ToString("0.00")));
                        differences = new double();
                        //得分小计
                        Rate.Add("Subtotal", subtotal == 0 ? 0 : subtotal);
                        subtotal = new double();
                        result.Add(Rate);
                        Rate = new JObject();
                    }
                }
            }
            return commom.GetModuleFromJarray(result);
        }

        //修改班组人员流失率
        [HttpPost]
        [ApiAuthorize]
        public JObject ModifyTurnover([System.Web.Http.FromBody]JObject data)
        {
            JArray result = new JArray();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            KPI_TurnoverRate turnoverRate = JsonConvert.DeserializeObject<KPI_TurnoverRate>(JsonConvert.SerializeObject(data));
            int count = 0;
            if (turnoverRate != null && turnoverRate.DateLoss != null && turnoverRate.Department != null && turnoverRate.Group != null)
            {
                if (db.KPI_TurnoverRate.Count(c => c.Department == turnoverRate.Department && c.Group == turnoverRate.Group && c.DateLoss == turnoverRate.DateLoss) > 0)
                {
                    turnoverRate.ModifierName = auth.UserName;//添加修改人
                    turnoverRate.ModifierDate = DateTime.Now;//添加修改时间
                    db.Entry(turnoverRate).State = EntityState.Modified;//修改数据
                    count = db.SaveChanges();
                }
                else
                {
                    turnoverRate.CreateTime = DateTime.Now;
                    turnoverRate.Createor = auth.UserName;
                    db.KPI_TurnoverRate.Add(turnoverRate);
                    count = db.SaveChanges();
                }
                if (count > 0)
                {
                    return commom.GetModuleFromJarray(result, true, "修改成功");
                }
                else
                {
                    return commom.GetModuleFromJarray(result, false, "修改失败");
                }
            }
            return commom.GetModuleFromJarray(result, false, "修改失败");
        }

        //导出班组人员流失率汇总表(Excel)
        //[HttpPost]
        //public FileContentResult Turnover_OutputExcel(int year, int month)
        //{
        //    var dataList = db.KPI_TurnoverRate.Where(c => c.DateLoss.Year == year && c.DateLoss.Month == month).ToList();
        //    List<ExportTurnoverCheckSshift> Resultlist = new List<ExportTurnoverCheckSshift>();
        //    var departmentlist = dataList.Select(c => c.Department).Distinct().ToList();
        //    foreach (var item in departmentlist)//循环被考核部门
        //    {
        //        var DG_list = dataList.Where(c => c.Department == item).Select(c => c.Group).Distinct().ToList();
        //        foreach (var ite in DG_list)//循环被考核部门的班组
        //        {
        //            ExportTurnoverCheckSshift at = new ExportTurnoverCheckSshift();
        //            var tarodlist = dataList.Where(c => c.Department == item && c.Group == ite).ToList();
        //            at.Department = tarodlist.FirstOrDefault().Department;//部门
        //            at.Group = tarodlist.FirstOrDefault().Group;//班组
        //            at.IndicatorsName = "班组流失率";//指标名称
        //            at.IndicatorsValue = tarodlist.FirstOrDefault().IndicatorsValue;//目标值
        //            at.SourceDepartment = "人力资源部";
        //            at.BeginNumber = tarodlist.FirstOrDefault().BeginNumber;//月初人数
        //            var endNumber = dataList.Where(c => c.Department == item && c.Group == ite).OrderByDescending(c => c.Id).Select(c => c.EndNumber).FirstOrDefault();
        //            at.EndNumber = endNumber;//月末人数
        //            double num = tarodlist.FirstOrDefault().BeginNumber + endNumber;
        //            double averageNum = Math.Ceiling(num / 2);
        //            at.AverageNum = (int)averageNum;//平均人数

        //            //赋值31天
        //            int totalscore = 0;
        //            var filds = at.GetType().GetFields(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy);
        //            filds = filds.Where(c => c.Name.Contains("Day")).ToArray();
        //            for (var i = 1; i <= filds.Count(); i++)
        //            {
        //                var ifno = dataList.Where(c => c.Department == item && c.Group == ite && c.DateLoss.Day == i).Select(c => c.LossNumber).FirstOrDefault();
        //                filds[i - 1].SetValue(at, ifno);
        //                totalscore = totalscore + ifno;
        //            }
        //            at.TurnoverMonth = totalscore;//月流失人数
        //            double actual = at.TurnoverMonth / (double)at.AverageNum;
        //            at.Actual = actual;//实际得分
        //            at.Differences = at.Actual - tarodlist.FirstOrDefault().IndicatorsValue;//与目标值的差异
        //            double subtotal = 0;
        //            if (at.Actual <= tarodlist.FirstOrDefault().IndicatorsValue)
        //            {
        //                subtotal = 100;

        //            }
        //            else if (at.Actual > tarodlist.FirstOrDefault().IndicatorsValue)
        //            {
        //                subtotal = 100 - (at.Actual - tarodlist.FirstOrDefault().IndicatorsValue);
        //            }
        //            at.Subtotal = subtotal;//得分小计
        //            Resultlist.Add(at);
        //        }
        //    }
        //    // 导出表格名称
        //    string tableName = "班组人员流失率汇总表" + year + "年" + month + "月";
        //    if (Resultlist.Count() > 0)
        //    {
        //        string[] columns = { "部门名称", "班组名称", "考核指标名", "目标值", "数据来源", "月初人数", "月末人数", "平均人数", "1日", "2日", "3日", "4日", "5日", "6日", "7日", "8日", "9日", "10日", "11日", "12日", "13日", "14日", "15日", "16日", "17日", "18日", "19日", "20日", "21日", "22日", "23日", "24日", "25日", "26日", "27日", "28日", "29日", "30日", "31日", "月流失人数", "实际得分", "与目标值差异", "得分小计" };
        //        byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
        //        return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
        //    }
        //    else
        //    {
        //        ExportTurnoverCheckSshift at1 = new ExportTurnoverCheckSshift();
        //        at1.Group = "没有找到相关记录！";
        //        Resultlist.Add(at1);
        //        string[] columns = { "部门名称", "班组名称", "考核指标名", "目标值", "数据来源", "月初人数", "月末人数", "平均人数", "1日", "2日", "3日", "4日", "5日", "6日", "7日", "8日", "9日", "10日", "11日", "12日", "13日", "14日", "15日", "16日", "17日", "18日", "19日", "20日", "21日", "22日", "23日", "24日", "25日", "26日", "27日", "28日", "29日", "30日", "31日", "月流失人数", "实际得分", "与目标值差异", "得分小计" };
        //        byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
        //        return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
        //    }
        //}

        #endregion

        #region---排名计算条件
        //查询条件
        [HttpPost]
        [ApiAuthorize]
        public JObject Calculation_Conditions([System.Web.Http.FromBody]JObject data)
        {
            JObject table = new JObject();
            JArray result = new JArray();
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            int? year = obj.year;
            int? month = obj.month;
            string department = obj.month;
            string group = obj.group;
            List<KPI_TotalDisplay> TotalDisplay = new List<KPI_TotalDisplay>();
            if (year != null && month != null)
            {
                TotalDisplay = db.KPI_TotalDisplay.Where(c => c.Time.Value.Year == year && c.Time.Value.Month == month).ToList();
            }
            if (department != null && year != null && month != null)
            {
                TotalDisplay = db.KPI_TotalDisplay.Where(c => c.Department == department && c.Time.Value.Year == year && c.Time.Value.Month == month).ToList();
            }
            if (group != null && year != null && month != null)
            {
                TotalDisplay = db.KPI_TotalDisplay.Where(c => c.Group == group && c.Time.Value.Year == year && c.Time.Value.Month == month).ToList();
            }
            if (department != null && year == null && month == null)
            {
                TotalDisplay = db.KPI_TotalDisplay.Where(c => c.Department == department).ToList();
            }
            if (group != null && year == null && month == null)
            {
                TotalDisplay = db.KPI_TotalDisplay.Where(c => c.Group == group).ToList();
            }
            if (TotalDisplay.Count > 0)
            {
                foreach (var item in TotalDisplay)
                {
                    //Id
                    table.Add("Id", item.Id == 0 ? 0 : item.Id);
                    //部门
                    table.Add("Department", item.Department == null ? null : item.Department);
                    //班组
                    table.Add("Group", item.Group == null ? null : item.Group);
                    //族群
                    table.Add("Ethnic_Group", item.Ethnic_Group == null ? null : item.Ethnic_Group);
                    //平均人数
                    table.Add("AvagePersonNum", item.AvagePersonNum == 0 ? 0 : item.AvagePersonNum);
                    //当月有无工伤事故
                    table.Add("InductrialAccident", item.InductrialAccident == 0 ? 0 : item.InductrialAccident);
                    //出勤率
                    var Attendance = (item.Attendance * 100) + "%";
                    table.Add("Attendance", Attendance);
                    //行政违规情况
                    table.Add("ViolationsMessage", item.ViolationsMessage == null ? null : item.ViolationsMessage);
                    //记录年月月份
                    var dateTime = string.Format("{0:yyyy-MM-dd}", item.Time);
                    table.Add("Time", dateTime == null ? null : dateTime);
                    result.Add(table);
                    table = new JObject();
                }
            }
            return commom.GetModuleFromJarray(result);
        }

        //批量添加数据
        [HttpPost]
        [ApiAuthorize]
        public JObject AddCalculation([System.Web.Http.FromBody]JObject data)
        {
            JArray result = new JArray();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            List<KPI_TotalDisplay> totalDisplays = (List<KPI_TotalDisplay>)JsonHelper.jsonDes<List<KPI_TotalDisplay>>(data["totalDisplays"].ToString());
            int count = 0;
            if (totalDisplays.Count > 0)
            {
                foreach (var item in totalDisplays)
                {
                    item.CreateTime = DateTime.Now;
                    item.Createor = auth.UserName;
                    if (db.KPI_TotalDisplay.Count(c => c.Time == item.Time && c.Department == item.Department && c.Group == item.Group) > 0)
                    {
                        result.Add(item.Time + item.Department + "," + item.Group + "的记录重复了" + ";");
                    }
                }
                if (result.Count > 0)
                {
                    return commom.GetModuleFromJarray(result, false, "数据重复");
                }
                db.KPI_TotalDisplay.AddRange(totalDisplays);
                count += db.SaveChanges();
                if (count == totalDisplays.Count)
                {
                    return commom.GetModuleFromJarray(result, true, "添加成功！");
                }
                else
                {
                    return commom.GetModuleFromJarray(result, false, "添加失败");
                }
            }
            return commom.GetModuleFromJarray(result, false, "添加失败");
        }

        #endregion

        #region---班组评优月度积分统计表

        //查看班组评优月度积分统计表
        [HttpPost]
        [ApiAuthorize]
        public JObject IntegralTable([System.Web.Http.FromBody]JObject data)
        {
            JObject table = new JObject();
            JObject time = new JObject();
            JArray result = new JArray();
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            int year = obj.year;
            var dataList = db.KPI_MonthlyIndicators.ToList();
            if (year != 0)
            {
                dataList = dataList.Where(c => c.Year == year).ToList();
            }
            int combined = 0;
            var departmentlist = dataList.Select(c => c.Department).Distinct().ToList();
            foreach (var item in departmentlist)//循环被考核部门
            {
                var DG_list = dataList.Where(c => c.Department == item).Select(c => c.Group).Distinct().ToList();
                foreach (var ite in DG_list)//循环被考核部门的班组
                {
                    var tarodlist = dataList.Where(c => c.Department == item && c.Group == ite).FirstOrDefault();
                    //ID
                    table.Add("Id", tarodlist.Id == 0 ? 0 : tarodlist.Id);
                    //部门
                    table.Add("Department", tarodlist.Department == null ? null : tarodlist.Department);
                    //班组
                    table.Add("Group", tarodlist.Group == null ? null : tarodlist.Group);
                    for (var i = 1; i <= 12; i++)
                    {
                        var date = dataList.Where(c => c.Department == item && c.Group == ite && c.Year == year && c.Month == i).Select(c => c.integral).FirstOrDefault();
                        //月份积分
                        if (i == 1) time.Add("January", date);
                        if (i == 2) time.Add("February", date);
                        if (i == 3) time.Add("March", date);
                        if (i == 4) time.Add("April", date);
                        if (i == 5) time.Add("May", date);
                        if (i == 6) time.Add("June", date);
                        if (i == 7) time.Add("July", date);
                        if (i == 8) time.Add("August", date);
                        if (i == 9) time.Add("September", date);
                        if (i == 10) time.Add("October", date);
                        if (i == 11) time.Add("November", date);
                        if (i == 12) time.Add("December", date);
                        combined = combined + date;
                    }
                    table.Add("Time", time);
                    time = new JObject();
                    //年度积分合计
                    table.Add("TotalScore", combined == 0 ? 0 : combined);
                    combined = new int();
                    result.Add(table);
                    table = new JObject();
                }
            }
            return commom.GetModuleFromJarray(result);
        }

        #region---导出Excel
        ////导出Excel表格自定义类
        //public class ExportIntegralTable
        //{
        //    public string Department { get; set; }//部门
        //    public string Group { get; set; }//班组
        //    public int Month1 { get; set; }//1月
        //    public int Month2 { get; set; }//2月
        //    public int Month3 { get; set; }//3月
        //    public int Month4 { get; set; }//4月
        //    public int Month5 { get; set; }//5月
        //    public int Month6 { get; set; }//6月
        //    public int Month7 { get; set; }//7月
        //    public int Month8 { get; set; }//8月
        //    public int Month9 { get; set; }//9月
        //    public int Month10 { get; set; }//10月
        //    public int Month11 { get; set; }//11月
        //    public int Month12 { get; set; }//12月
        //    public int YearIntegral { get; set; }//年度积分合计

        //}

        ////导出班组评优月度积分统计表(Excel)
        //[HttpPost]
        //public FileContentResult Integral_OutputExcel(int year)
        //{
        //    var dataList = db.KPI_MonthlyIndicators.Where(c => c.Year == year).ToList();
        //    List<ExportIntegralTable> Resultlist = new List<ExportIntegralTable>();
        //    var departmentlist = dataList.Select(c => c.Department).Distinct().ToList();
        //    foreach (var item in departmentlist)//循环被考核部门
        //    {
        //        var DG_list = dataList.Where(c => c.Department == item).Select(c => c.Group).Distinct().ToList();
        //        foreach (var ite in DG_list)//循环被考核部门的班组
        //        {
        //            ExportIntegralTable at = new ExportIntegralTable();
        //            var tarodlist = dataList.Where(c => c.Department == item && c.Group == ite).ToList();
        //            at.Department = tarodlist.FirstOrDefault().Department;
        //            at.Group = tarodlist.FirstOrDefault().Group;
        //            //赋值12个月
        //            int totalscore = 0;
        //            var filds = at.GetType().GetFields(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy);
        //            filds = filds.Where(c => c.Name.Contains("Month")).ToArray();
        //            for (var i = 0; i < filds.Count(); i++)
        //            {

        //                var ifno = dataList.Where(c => c.Department == item && c.Group == ite && c.Month == i).Select(c => c.integral).FirstOrDefault();
        //                filds[i].SetValue(at, ifno);
        //                totalscore = totalscore + ifno;
        //            }
        //            at.YearIntegral = totalscore;
        //            Resultlist.Add(at);
        //        }
        //    }
        //    // 导出表格名称
        //    string tableName = "班组评优月度积分统计表";
        //    if (Resultlist.Count() > 0)
        //    {
        //        string[] columns = { "部门", "班组", "1月", "2月", "3月", "4月", "5月", "7月", "8月", "9月", "10月", "11月", "12月", "年度积分合计" };
        //        byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
        //        return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
        //    }
        //    else
        //    {
        //        ExportIntegralTable at1 = new ExportIntegralTable();
        //        at1.Group = "没有找到相关记录！";
        //        Resultlist.Add(at1);
        //        string[] columns = { "部门", "班组", "1月", "2月", "3月", "4月", "5月", "7月", "8月", "9月", "10月", "11月", "12月", "年度积分合计" };
        //        byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
        //        return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
        //    }
        //}
        #endregion

        #endregion

        #region---光荣榜（优秀班组）
        public class IntegralList
        {
            public string group { get; set; }
            public int integral { get; set; }
            public string department { get; set; }
        }

        //上传优秀班组图片
        [HttpPost]
        [ApiAuthorize]
        public bool UploadFile_KPI()
        {
            string datetime = null;
            bool result = false;
            int year = int.Parse(HttpContext.Current.Request["year"]);//年
            int month = int.Parse(HttpContext.Current.Request["month"]);//月
            int ranking = int.Parse(HttpContext.Current.Request["ranking"]);//名称
            string group = HttpContext.Current.Request["group"];
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//上传的问题件  这个“MS_HttpContext”参数名不需要改
            HttpRequestBase requests = context.Request;
            string rankingList = "第" + ranking + "名";
            if (requests.Files.Count > 0)
            {
                for (int i = 0; i < requests.Files.Count; i++)
                {
                    var file = requests.Files[i];
                    var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();
                    if (year != 0 && month != 0 && group != null && rankingList != null)
                    {
                        datetime = year + "年" + month + "月";
                        //判断该文件夹是否等于false（有没有该文件夹）
                        if (Directory.Exists(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\") == false)//如果不存在就创建订单文件夹
                        {
                            //创建文件夹
                            Directory.CreateDirectory(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\");
                        }
                        if (fileType == ".pdf")
                        {
                            //把文件保存到对应的文件夹下
                            file.SaveAs(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\" + group + "\\" + rankingList + ".pdf");
                            result = true;
                        }
                        else if (fileType == ".jpg")
                        {
                            //把文件保存到对应的文件夹下
                            file.SaveAs(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\" + group + "\\" + rankingList + ".jpg");
                            result = true;
                        }
                        else
                        {
                            //把文件保存到对应的文件夹下
                            file.SaveAs(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\" + group + "\\" + rankingList + fileType);
                            result = true;
                        }
                    }
                    else if (year != 0 && month == 0 && group != null && rankingList != null)
                    {
                        datetime = year + "年";
                        //判断该文件夹是否等于false（有没有该文件夹）
                        if (Directory.Exists(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\") == false)//如果不存在就创建订单文件夹
                        {
                            //创建文件夹
                            Directory.CreateDirectory(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\");
                        }
                        if (fileType == ".pdf")
                        {
                            //把文件保存到对应的文件夹下
                            file.SaveAs(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\" + group + "\\" + rankingList + ".pdf");
                            result = true;
                        }
                        else if (fileType == ".jpg")
                        {
                            //把文件保存到对应的文件夹下
                            file.SaveAs(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\" + group + "\\" + rankingList + ".jpg");
                            result = true;
                        }
                        else
                        {
                            //把文件保存到对应的文件夹下
                            file.SaveAs(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\" + group + "\\" + rankingList + fileType);
                            result = true;
                        }
                    }
                }
                return result;
            }
            return result;
        }

        //优秀班组图片更换(替换)上传
        [HttpPost]
        [ApiAuthorize]
        public bool UploadFile_KPI_Replace([System.Web.Http.FromBody] JObject data)
        {
            bool result = true;
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string OrignFile = obj.OrignFile;//路径
            int year = obj.year;//年
            int month = obj.month;//月
            int ranking = obj.ranking;//名次
            string group = obj.group;//班组
            string rankingList = "第" + ranking + "名";
            string datetime = null;
            if (year != 0 && month != 0)
            {
                datetime = year + "年" + month + "月";
            }
            else if (year != 0 && month == 0)
            {
                datetime = year + "年";
            }
            var filename = OrignFile.Substring(OrignFile.LastIndexOf('/') + 1);//以最后一个斜杠截取路径（只保留文件名）
            if (Directory.Exists(@"D:\MES_Data\优秀班组光荣榜\" + datetime + "\\" + group + "\\已替换文件\\") == false)
            {
                Directory.CreateDirectory(@"D:\MES_Data\优秀班组光荣榜\" + datetime + "\\" + group + "\\已替换文件\\");
            }
            var NewFile = @"D:\MES_Data\优秀班组光荣榜\" + datetime + "\\" + group + "\\已替换文件\\" + filename;
            try
            {
                var conversion = OrignFile.Replace("/", "\\");//把旧文件路径的斜杠（/）替换成这个斜杠（\）
                System.IO.File.Move("D:" + conversion, NewFile);//把旧文件移动到NewFile这个文件夹下
            }
            catch //捕捉异常
            {
                result = false;
            }
            if (!UploadFile_KPI())//判断调用的方法是否有这些参数
            {
                result = false;
            }
            return result;
        }

        //显示年/月度优秀班组前五名的数据和图片
        [HttpPost]
        [ApiAuthorize]
        public JObject Monthly_ExcellentTeam([System.Web.Http.FromBody] JObject data)
        {
            JObject groupList = new JObject();
            JArray result = new JArray();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year;//年
            int month = obj.month;//月
            List<KPI_MonthlyIndicators> rankingList = new List<KPI_MonthlyIndicators>();
            string datetime = null;
            List<IntegralList> integ = new List<IntegralList>();

            if (year != 0 && month != 0)//按月度查询光荣榜
            {
                rankingList = db.KPI_MonthlyIndicators.Where(c => c.Year == year && c.Month == month && c.Ranking != 0 && c.integral != 0).ToList();
                datetime = year + "年" + month + "月";
                var codeList = rankingList.Where(c => c.Ranking != 0 && c.integral != 0).Select(c => c.Department).Distinct().ToList();
                foreach (var item in codeList)
                {
                    var gioupList = rankingList.Where(c => c.Ranking != 0 && c.integral != 0 && c.Department == item).Select(c => c.Group).Distinct().ToList();
                    foreach (var ite in gioupList)
                    {
                        int inte = rankingList.Where(c => c.Department == item && c.Group == ite && c.Year == year && c.Month == month).Select(c => c.Ranking).FirstOrDefault();
                        var ranking = "第" + inte + "名";
                        //名次                      
                        groupList.Add("Ranking", ranking == null ? null : ranking);
                        //图片路径 
                        if (Directory.Exists(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\" + ite + "\\" + ranking) == false)
                        {
                            groupList.Add("Path", null);
                        }
                        else
                        {
                            List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\优秀班组光荣榜\月度\" + datetime + "\\" + ite + "\\" + ranking);

                            string path = @"/MES_Data/优秀班组光荣榜/月度/" + datetime + "/" + ite + "/" + ranking;//组合出路径
                            var filetype = path.Split('.');//将组合出来的路径以点分隔，方便下一步判断后缀
                            if (filetype[1] == "pdf")//后缀为jpg
                            {
                                groupList.Add("Path", path == null ? null : path);
                            }
                            else //后缀为其他
                            {
                                groupList.Add("Path", path == null ? null : path);
                            }
                        }
                        result.Add(groupList);
                        groupList = new JObject();
                    }
                }
            }
            if (year != 0 && month == 0)//按年度查询光荣榜
            {
                rankingList = db.KPI_MonthlyIndicators.Where(c => c.Year == year && c.Ranking != 0 && c.integral != 0).ToList();
                datetime = year + "年";
                var codeList = rankingList.Where(c => c.Ranking != 0 && c.integral != 0).Select(c => c.Department).Distinct().ToList();
                foreach (var item in codeList)
                {
                    var gioupList = rankingList.Where(c => c.Ranking != 0 && c.integral != 0 && c.Department == item).Select(c => c.Group).Distinct().ToList();
                    foreach (var ite in gioupList)
                    {
                        int integl = rankingList.Where(c => c.Department == item && c.Group == ite && c.Year == year).Select(c => c.integral).Sum();
                        IntegralList aa = new IntegralList() { department = item, group = ite, integral = integl };
                        integ.Add(aa);
                    }
                }
                int i = 1;
                var f = integ.OrderByDescending(c => c.integral).Take(5);
                foreach (var it in f)
                {
                    //部门
                    groupList.Add("Department", it.department == null ? null : it.department);
                    //班组
                    groupList.Add("Group", it.group == null ? null : it.group);
                    string ranking = "第" + i + "名";
                    //名次                      
                    groupList.Add("Ranking", ranking);
                    i++;
                    //图片路径  
                    if (Directory.Exists(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\" + it.group + "\\" + ranking) == false)
                    {
                        groupList.Add("Path", null);
                    }
                    else
                    {
                        List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\优秀班组光荣榜\年度\" + datetime + "\\" + it.group + "\\" + ranking);

                        string path = @"/MES_Data/优秀班组光荣榜/年度/" + datetime + "/" + it.group + "/" + ranking;//组合出路径
                        var filetype = path.Split('.');//将组合出来的路径以点分隔，方便下一步判断后缀
                        if (filetype[1] == "pdf")//后缀为jpg
                        {
                            groupList.Add("Path", path == null ? null : path);
                        }
                        else //后缀为其他
                        {
                            groupList.Add("Path", path == null ? null : path);
                        }
                    }
                    result.Add(groupList);
                    groupList = new JObject();
                }
            }
            return commom.GetModuleFromJarray(result);
        }

        #endregion

        #region---班组评优月度汇总表方法（四个指标的汇总及审核操作方法）
        public class ExportHistoryQuery
        {
            public string Department { get; set; }
            public string Group { get; set; }
            public string Efficiency_Indicators { get; set; }
            public string Efficiency_IndexName { get; set; }
            public double Efficiency_Target { get; set; }
            public double Efficiency_Actual { get; set; }
            public double Efficiency_Single { get; set; }
            public double Efficiency_Score { get; set; }
            public string Quality_Indicators { get; set; }
            public string Quality_IndexName { get; set; }
            public double Quality_Target { get; set; }
            public double Quality_Actual { get; set; }
            public double Quality_Single { get; set; }
            public double Quality_Score { get; set; }
            public string Turnover_Indicators { get; set; }
            public string Turnover_IndexName { get; set; }
            public double Turnover_Target { get; set; }
            public double Turnover_Actual { get; set; }
            public double Turnover_Single { get; set; }
            public double Turnover_Score { get; set; }
            public string Comparison_Indicators { get; set; }
            public string Comparison_IndexName { get; set; }
            public double Comparison_Target { get; set; }
            public double Comparison_Actual { get; set; }
            public double Comparison_Single { get; set; }
            public double Comparison_Score { get; set; }
            public decimal TotalScore { get; set; }
            public bool Compare { get; set; }
            public decimal AvagePersonNum { get; set; }
            public decimal InductrialAccident { get; set; }
            public string Ethnic_Group { get; set; }
            public string Attendance { get; set; }
            public string ViolationsMessage { get; set; }
            public int Ranking { get; set; }
            public int Integral { get; set; }

        }

        //按年查找已核准的优秀班组清单
        [HttpPost]
        [ApiAuthorize]
        public JObject ApprovedList([System.Web.Http.FromBody] JObject data)
        {
            JObject table = new JObject();
            JArray result = new JArray();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year;//年
            var monthList = db.KPI_ReviewSummary.GroupBy(c => c.Month).Select(c => new { Month = c.Key, List = c.ToList() }).ToList();
            if (monthList.Count > 0)
            {
                foreach (var m in monthList)
                {
                    table.Add("Time", year + "-" + m.Month);
                    bool judge = true;
                    foreach (var it in m.List)
                    {
                        if (it.Type == "确认")
                        {
                            table.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                            table.Add("HRAuditDate", it.HRAuditDate == null ? null : Convert.ToDateTime(it.HRAuditDate).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        if (it.Type == "审核")
                        {
                            table.Add("Audit", it.HRAudit == null ? null : it.HRAudit);
                            table.Add("AuditDate", it.HRAuditDate == null ? null : Convert.ToDateTime(it.HRAuditDate).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        if (it.Type == "核准")
                        {
                            if (it.HRAudit == null) judge = false;
                            table.Add("Approved", it.HRAudit == null ? null : it.HRAudit);
                            table.Add("ApprovedDate", it.HRAuditDate == null ? null : Convert.ToDateTime(it.HRAuditDate).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                    }
                    if (judge == true)
                    {
                        result.Add(table);
                        table = new JObject();
                    }
                }
            }
            return commom.GetModuleFromJarray(result);
        }

        //班组评优月度汇总表历史查询
        [HttpPost]
        [ApiAuthorize]
        public JObject HistoryQuery([System.Web.Http.FromBody] JObject data)
        {
            JObject table = new JObject();
            JObject code = new JObject();
            JArray query = new JArray();
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year;//年
            int month = obj.month;//月
            var queryList = db.KPI_MonthlyIndicators.ToList();
            if (year != 0 && month != 0)
            {
                queryList = queryList.Where(c => c.Year == year && c.Month == month).ToList();
            }
            if (queryList.Count > 0)
            {
                foreach (var item in queryList)
                {
                    //id
                    table.Add("Id", item.Id == 0 ? 0 : item.Id);
                    //被考核部门
                    table.Add("Department", item.Department == null ? null : item.Department);
                    //班组
                    table.Add("Group", item.Group == null ? null : item.Group);
                    //效率指标
                    table.Add("Efficiency_Indicators", item.Efficiency_Indicators == null ? null : item.Efficiency_Indicators);
                    //指标名称(效率)
                    table.Add("Efficiency_IndexName", item.Efficiency_IndexName == null ? null : item.Efficiency_IndexName);
                    //目标值(效率)
                    var Efficiency_Unit = db.KPI_Indicators.Where(c => c.ExecutionTime.Value.Year == year && c.ExecutionTime.Value.Month == month && c.Department == item.Department && c.Group == item.Group && c.IndicatorsName == item.Efficiency_IndexName && c.IndicatorsValue == item.Efficiency_Target).Select(c => c.IndicatorsValueUnit).FirstOrDefault();
                    table.Add("Efficiency_Target", item.Efficiency_Target == 0 ? null : item.Efficiency_Target + Efficiency_Unit);
                    //实际完成值(效率)
                    table.Add("Efficiency_Actual", item.Efficiency_Actual == 0 ? 0 : item.Efficiency_Actual);
                    //单项得分(效率)
                    table.Add("Efficiency_Single", item.Efficiency_Single == 0 ? 0 : item.Efficiency_Single);
                    //得分小计(效率)
                    table.Add("Efficiency_Score", item.Efficiency_Score == 0 ? 0 : item.Efficiency_Score);

                    //品质指标
                    table.Add("Quality_Indicators", item.Quality_Indicators == null ? null : item.Quality_Indicators);
                    //指标名称(品质)
                    table.Add("Quality_IndexName", item.Quality_IndexName == null ? null : item.Quality_IndexName);
                    //目标值(品质)
                    var Quality_Unit = db.KPI_Indicators.Where(c => c.ExecutionTime.Value.Year == year && c.ExecutionTime.Value.Month == month && c.Department == item.Department && c.Group == item.Group && c.IndicatorsName == item.Quality_IndexName && c.IndicatorsValue == item.Quality_Target).Select(c => c.IndicatorsValueUnit).FirstOrDefault();
                    table.Add("Quality_Target", item.Quality_Target == 0 ? null : item.Quality_Target + Quality_Unit);
                    //实际完成值(品质)
                    table.Add("Quality_Actual", item.Quality_Actual == 0 ? 0 : item.Quality_Actual);
                    //单项得分(品质)
                    table.Add("Quality_Single", item.Quality_Single == 0 ? 0 : item.Quality_Single);
                    //得分小计(品质)
                    table.Add("Quality_Score", item.Quality_Score == 0 ? 0 : item.Quality_Score);

                    //月度流失率指标
                    table.Add("Turnover_Indicators", item.Turnover_Indicators == null ? null : item.Turnover_Indicators);
                    //指标名称(品质)
                    table.Add("Turnover_IndexName", item.Turnover_IndexName == null ? null : item.Turnover_IndexName);
                    //目标值(品质)
                    table.Add("Turnover_Target", item.Turnover_Target == 0 ? null : item.Turnover_Target + "%");
                    //实际完成值(品质)
                    table.Add("Turnover_Actual", item.Turnover_Actual == 0 ? 0 : item.Turnover_Actual);
                    //单项得分(品质)
                    table.Add("Turnover_Single", item.Turnover_Single == 0 ? 0 : item.Turnover_Single);
                    //得分小计(品质)
                    table.Add("Turnover_Score", item.Turnover_Score == 0 ? 0 : item.Turnover_Score);

                    //7S评比指标
                    table.Add("Comparison_Indicators", item.Comparison_Indicators == null ? null : item.Comparison_Indicators);
                    //指标名称(品质)
                    table.Add("Comparison_IndexName", item.Comparison_IndexName == null ? null : item.Comparison_IndexName);
                    //目标值(品质)
                    table.Add("Comparison_Target", item.Comparison_Target == 0 ? 0 : item.Comparison_Target);
                    //实际完成值(品质)
                    table.Add("Comparison_Actual", item.Comparison_Actual == 0 ? 0 : item.Comparison_Actual);
                    //单项得分(品质)
                    table.Add("Comparison_Single", item.Comparison_Single == 0 ? 0 : item.Comparison_Single);
                    //得分小计(品质)
                    table.Add("Comparison_Score", item.Comparison_Score == 0 ? 0 : item.Comparison_Score);

                    //合计总分
                    table.Add("TotalScore", item.TotalScore == 0 ? 0 : item.TotalScore);
                    //合计总分是否大于等于90
                    table.Add("Compare", item.TotalScore >= 90 == true ? true : false);
                    //平均人数（不少于5人）
                    table.Add("AvagePersonNum", item.AvagePersonNum == 0 ? 0 : item.AvagePersonNum);
                    //当月有无工伤事故
                    table.Add("InductrialAccident", item.InductrialAccident == 0 ? 0 : item.InductrialAccident);

                    //族群
                    table.Add("Ethnic_Group", item.Ethnic_Group == null ? null : item.Ethnic_Group);

                    //出勤率
                    table.Add("Attendance", item.Attendance == 0 ? 0 : item.Attendance);
                    //行政违规情况
                    table.Add("ViolationsMessage", item.ViolationsMessage == null ? null : item.ViolationsMessage);
                    //排名
                    table.Add("Ranking", item.Ranking == 0 ? 0 : item.Ranking);
                    //积分
                    table.Add("integral", item.integral == 0 ? 0 : item.integral);
                    query.Add(table);
                    table = new JObject();
                }
            }
            var codelist = db.KPI_ReviewSummary.Where(c => c.Year == year && c.Month == month).ToList();
            if (codelist.Count == 0)
            {
                code.Add("Code", null);
            }
            else
            {
                //审核列表
                JObject confirm = new JObject();//确认
                JObject audit = new JObject();//审核
                JObject approved = new JObject();//核准
                foreach (var it in codelist)
                {
                    if (it.Type == "确认")
                    {
                        confirm.Add("HRAuditDate", it.HRAuditDate == null ? null : it.HRAuditDate.ToString());
                        confirm.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                        confirm.Add("HRjudge", it.HRjudge == false ? false : it.HRjudge);
                    }
                    if (it.Type == "审核")
                    {
                        audit.Add("HRAuditDate", it.HRAuditDate == null ? null : it.HRAuditDate.ToString());
                        audit.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                        audit.Add("HRjudge", it.HRjudge == false ? false : it.HRjudge);
                    }
                    if (it.Type == "核准")
                    {
                        approved.Add("HRAuditDate", it.HRAuditDate == null ? null : it.HRAuditDate.ToString());
                        approved.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                        approved.Add("HRjudge", it.HRjudge == false ? false : it.HRjudge);
                    }
                }
                code.Add("Confirm", confirm);//确认
                confirm = new JObject();
                code.Add("Audit", audit);//审核
                audit = new JObject();
                code.Add("Approved", approved);//核准
                approved = new JObject();
            }
            result.Add("Query", query);
            result.Add("Code", code);
            return commom.GetModuleFromJobjet(result);
        }

        //批准后保存班组评优月度汇总表
        [HttpPost]
        [ApiAuthorize]
        public JObject ADDSummaryTable([System.Web.Http.FromBody] JObject data)
        {
            JArray result = new JArray();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = obj.year;//年
            int month = obj.month;//月
            List<KPI_MonthlyIndicators> indicators = (List<KPI_MonthlyIndicators>)JsonHelper.jsonDes<List<KPI_MonthlyIndicators>>(data["indicators"].ToString());
            if (indicators.Count > 0 && year != 0 && month != 0)
            {
                foreach (var ite in indicators)
                {
                    if (db.KPI_MonthlyIndicators.Count(c => c.Year == year && c.Month == month && c.Department == ite.Department && c.Group == ite.Group) > 0)
                    {
                        result.Add(year + "-" + month + "," + ite.Department + "," + ite.Group + "的记录重复了" + ";");
                    }
                }
                if (result.Count > 0)
                {
                    return commom.GetModuleFromJarray(result, false, "数据重复");
                }
                foreach (var item in indicators)
                {
                    item.Year = year;
                    item.Month = month;
                    db.SaveChanges();
                }
                db.KPI_MonthlyIndicators.AddRange(indicators);
                int count = db.SaveChanges();
                if (count > 0)
                {
                    return commom.GetModuleFromJarray(result, true, "保存成功");
                }
                else
                {
                    return commom.GetModuleFromJarray(result, false, "保存失败");
                }
            }
            return commom.GetModuleFromJarray(result, false, "保存失败");
        }

        //审核KPI总表
        [HttpPost]
        [ApiAuthorize]
        public JObject Audit_SummarySheet([System.Web.Http.FromBody] JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            int year = obj.year;//年
            int month = obj.month;//月
            string hrAudit = obj.hrAudit;
            bool hrjudge = obj.hrjudge;
            string type = obj.type;
            if (year != 0 && month != 0 && hrAudit != null)
            {
                KPI_ReviewSummary summary = new KPI_ReviewSummary();
                var audit = db.KPI_ReviewSummary.Where(c => c.Year == year && c.Month == month && c.Type == type).FirstOrDefault();
                if (audit == null)
                {
                    if (db.KPI_ReviewSummary.Where(c => c.Year == year && c.Month == month && c.HRAudit == hrAudit).Select(c => c.Type).FirstOrDefault() != null)
                    {
                        return commom.GetModuleFromJobjet(result, false, hrAudit + "已经审核过了,不能重复审核!");
                    }
                    summary.Year = year;
                    summary.Month = month;
                    summary.HRAudit = hrAudit;
                    summary.Type = type;
                    summary.Department = auth.Department;
                    summary.HRAuditDate = DateTime.Now;
                    summary.HRjudge = hrjudge;
                    db.KPI_ReviewSummary.Add(summary);
                    int count = db.SaveChanges();
                    if (count > 0)
                    {
                        return commom.GetModuleFromJobjet(result, true, "审核成功！");
                    }
                    else
                    {
                        return commom.GetModuleFromJobjet(result, false, "审核失败！");
                    }
                }
                else
                {
                    return commom.GetModuleFromJobjet(result, false, year + "年" + month + "月" + "的月度指标汇总数据，已审核！,请刷新");
                }
            }
            return commom.GetModuleFromJobjet(result, false, "审核失败！");
        }

        #region---导出Excel表格
        ////导出班组评优月度汇总表(Excel),tableData(前面需要把数据全部转换成字符串，再传给后台)
        //[HttpPost]
        //public FileContentResult Export_SummaryTable(int year, int month)
        //{
        //    var dataList = db.KPI_MonthlyIndicators.Where(c => c.Year == year && c.Month == month).ToList();
        //    List<ExportHistoryQuery> Resultlist = new List<ExportHistoryQuery>();
        //    var departmentlist = dataList.Select(c => c.Department).Distinct().ToList();
        //    foreach (var item in departmentlist)//循环被考核部门
        //    {
        //        var DG_list = dataList.Where(c => c.Department == item).Select(c => c.Group).Distinct().ToList();
        //        foreach (var ite in DG_list)//循环被考核部门的班组
        //        {
        //            ExportHistoryQuery at = new ExportHistoryQuery();
        //            var tarodlist = dataList.Where(c => c.Department == item && c.Group == ite).ToList();
        //            at.Department = tarodlist.FirstOrDefault().Department;
        //            at.Group = tarodlist.FirstOrDefault().Group;
        //            //效率
        //            at.Efficiency_Indicators = "效率指标";
        //            at.Efficiency_IndexName = tarodlist.FirstOrDefault().Efficiency_IndexName;
        //            at.Efficiency_Target = tarodlist.FirstOrDefault().Efficiency_Target;
        //            at.Efficiency_Actual = tarodlist.FirstOrDefault().Efficiency_Actual;
        //            at.Efficiency_Single = tarodlist.FirstOrDefault().Efficiency_Single;
        //            at.Efficiency_Score = tarodlist.FirstOrDefault().Efficiency_Score;
        //            //品质
        //            at.Quality_Indicators = "品质指标";
        //            at.Quality_IndexName = tarodlist.FirstOrDefault().Quality_IndexName;
        //            at.Quality_Target = tarodlist.FirstOrDefault().Quality_Target;
        //            at.Quality_Actual = tarodlist.FirstOrDefault().Quality_Actual;
        //            at.Quality_Single = tarodlist.FirstOrDefault().Quality_Single;
        //            at.Quality_Score = tarodlist.FirstOrDefault().Quality_Score;
        //            //月流失率
        //            at.Turnover_Indicators = "月流失率指标";
        //            at.Turnover_IndexName = tarodlist.FirstOrDefault().Turnover_IndexName;
        //            at.Turnover_Target = tarodlist.FirstOrDefault().Turnover_Target;
        //            at.Turnover_Actual = tarodlist.FirstOrDefault().Turnover_Actual;
        //            at.Turnover_Single = tarodlist.FirstOrDefault().Turnover_Single;
        //            at.Turnover_Score = tarodlist.FirstOrDefault().Turnover_Score;
        //            //7S评比
        //            at.Comparison_Indicators = "7S评比指标";
        //            at.Comparison_IndexName = tarodlist.FirstOrDefault().Comparison_IndexName;
        //            at.Comparison_Target = tarodlist.FirstOrDefault().Comparison_Target;
        //            at.Comparison_Actual = tarodlist.FirstOrDefault().Comparison_Actual;
        //            at.Comparison_Single = tarodlist.FirstOrDefault().Comparison_Single;
        //            at.Comparison_Score = tarodlist.FirstOrDefault().Comparison_Score;

        //            at.TotalScore = tarodlist.FirstOrDefault().TotalScore;
        //            at.Compare = tarodlist.FirstOrDefault().TotalScore >= 90 == true ? true : false;
        //            at.AvagePersonNum = tarodlist.FirstOrDefault().AvagePersonNum;
        //            at.InductrialAccident = tarodlist.FirstOrDefault().InductrialAccident;
        //            at.Ethnic_Group = tarodlist.FirstOrDefault().Ethnic_Group;
        //            at.Attendance = tarodlist.FirstOrDefault().Attendance.ToString() + "%";
        //            at.ViolationsMessage = tarodlist.FirstOrDefault().ViolationsMessage;
        //            at.Ranking = tarodlist.FirstOrDefault().Ranking;
        //            at.Integral = tarodlist.FirstOrDefault().integral;

        //            Resultlist.Add(at);
        //        }
        //    }
        //    // 导出表格名称
        //    string tableName = "班组评优汇总表" + year + "年" + month + "月";
        //    if (Resultlist.Count() > 0)
        //    {
        //        string[] columns = { "部门", "班组", "效率指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "品质指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "月度流失率指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "7S评比指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "合计总分", "总分不低于90分", "平均人数（不少于5人）", "当月有无工伤事故", "族群", "出勤率", "行政违纪情况", "排名", "积分" };
        //        byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
        //        return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
        //    }
        //    else
        //    {
        //        ExportHistoryQuery at1 = new ExportHistoryQuery();
        //        at1.Group = "没有找到相关记录！";
        //        Resultlist.Add(at1);
        //        string[] columns = { "部门", "班组", "效率指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "品质指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计","月度流失率指标","考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计" ,
        //            "7S评比指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计" ,"合计总分", "总分不低于90分", "平均人数（不少于5人）", "当月有无工伤事故","族群", "出勤率", "行政违纪情况","排名", "积分"};
        //        byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
        //        return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
        //    }
        //}

        ////导出临时查询结果为Excel文件
        //[HttpPost]
        //public FileContentResult Export_TemporaryTable(List<KPI_MonthlyIndicators> indicators, int year, int month)
        //{
        //    List<ExportHistoryQuery> Resultlist = new List<ExportHistoryQuery>();
        //    var departmentlist = indicators.Select(c => c.Department).Distinct().ToList();
        //    foreach (var item in departmentlist)//循环被考核部门
        //    {
        //        var DG_list = indicators.Where(c => c.Department == item).Select(c => c.Group).Distinct().ToList();
        //        foreach (var ite in DG_list)//循环被考核部门的班组
        //        {
        //            ExportHistoryQuery at = new ExportHistoryQuery();
        //            var tarodlist = indicators.Where(c => c.Department == item && c.Group == ite).ToList();
        //            at.Department = tarodlist.FirstOrDefault().Department;
        //            at.Group = tarodlist.FirstOrDefault().Group;
        //            //效率
        //            at.Efficiency_Indicators = tarodlist.FirstOrDefault().Efficiency_Indicators;
        //            at.Efficiency_IndexName = tarodlist.FirstOrDefault().Efficiency_IndexName;
        //            at.Efficiency_Target = tarodlist.FirstOrDefault().Efficiency_Target;
        //            at.Efficiency_Actual = tarodlist.FirstOrDefault().Efficiency_Actual;
        //            at.Efficiency_Single = tarodlist.FirstOrDefault().Efficiency_Single;
        //            at.Efficiency_Score = tarodlist.FirstOrDefault().Efficiency_Score;
        //            //品质
        //            at.Quality_Indicators = tarodlist.FirstOrDefault().Quality_Indicators;
        //            at.Quality_IndexName = tarodlist.FirstOrDefault().Quality_IndexName;
        //            at.Quality_Target = tarodlist.FirstOrDefault().Quality_Target;
        //            at.Quality_Actual = tarodlist.FirstOrDefault().Quality_Actual;
        //            at.Quality_Single = tarodlist.FirstOrDefault().Quality_Single;
        //            at.Quality_Score = tarodlist.FirstOrDefault().Quality_Score;
        //            //月流失率
        //            at.Turnover_Indicators = tarodlist.FirstOrDefault().Turnover_Indicators;
        //            at.Turnover_IndexName = tarodlist.FirstOrDefault().Turnover_IndexName;
        //            at.Turnover_Target = tarodlist.FirstOrDefault().Turnover_Target;
        //            at.Turnover_Actual = tarodlist.FirstOrDefault().Turnover_Actual;
        //            at.Turnover_Single = tarodlist.FirstOrDefault().Turnover_Single;
        //            at.Turnover_Score = tarodlist.FirstOrDefault().Turnover_Score;
        //            //7S评比
        //            at.Comparison_Indicators = tarodlist.FirstOrDefault().Comparison_Indicators;
        //            at.Comparison_IndexName = tarodlist.FirstOrDefault().Comparison_IndexName;
        //            at.Comparison_Target = tarodlist.FirstOrDefault().Comparison_Target;
        //            at.Comparison_Actual = tarodlist.FirstOrDefault().Comparison_Actual;
        //            at.Comparison_Single = tarodlist.FirstOrDefault().Comparison_Single;
        //            at.Comparison_Score = tarodlist.FirstOrDefault().Comparison_Score;

        //            at.TotalScore = tarodlist.FirstOrDefault().TotalScore;
        //            at.Compare = tarodlist.FirstOrDefault().TotalScore >= 90 == true ? true : false;
        //            at.AvagePersonNum = tarodlist.FirstOrDefault().AvagePersonNum;
        //            at.InductrialAccident = tarodlist.FirstOrDefault().InductrialAccident;
        //            at.Ethnic_Group = tarodlist.FirstOrDefault().Ethnic_Group;
        //            at.Attendance = tarodlist.FirstOrDefault().Attendance.ToString() + "%";
        //            at.ViolationsMessage = tarodlist.FirstOrDefault().ViolationsMessage;
        //            at.Ranking = tarodlist.FirstOrDefault().Ranking;
        //            at.Integral = tarodlist.FirstOrDefault().integral;

        //            Resultlist.Add(at);
        //        }
        //    }
        //    // 导出表格名称

        //    string tableName = "班组评优汇总表" + year + "年" + month + "月";
        //    if (Resultlist.Count() > 0)
        //    {
        //        string[] columns = { "部门", "班组", "效率指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "品质指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计","月度流失率指标","考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计" ,
        //            "7S评比指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计" ,"合计总分", "总分不低于90分", "平均人数（不少于5人）", "当月有无工伤事故","族群", "出勤率", "行政违纪情况","排名", "积分"};
        //        byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
        //        return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
        //    }
        //    else
        //    {
        //        ExportHistoryQuery at1 = new ExportHistoryQuery();
        //        at1.Group = "没有找到相关记录！";
        //        Resultlist.Add(at1);
        //        string[] columns = { "部门", "班组", "效率指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计", "品质指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计","月度流失率指标","考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计" ,
        //            "7S评比指标", "考核指标名称", "目标值", "实际完成值", "单项得分", "得分小计" ,"合计总分", "总分不低于90分", "平均人数（不少于5人）", "当月有无工伤事故","族群", "出勤率", "行政违纪情况","排名", "积分"};
        //        byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
        //        return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
        //    }
        //}

        #endregion

        #endregion

        #region  ---KPI显示
        public class TempRate
        {
            public int Id { get; set; }
            public string Department { get; set; }
            public string Group { get; set; }
            public double IndicatorsValue { get; set; }//目标值
            public int BeginNumber { get; set; }
            public int EndNumber { get; set; }
            public int LossNumber { get; set; }
            public DateTime DateLoss { get; set; }
        }
        public class RankingList
        {
            public string Department { get; set; }//部门
            public string Group { get; set; }//班组
            public string Ethnic_Group { get; set; }//族群
            public decimal AvagePersonNum { get; set; }//平均人数
            public decimal InductrialAccident { get; set; }//当月有无工伤事故
            public decimal Attendance { get; set; }//出勤率
            public string ViolationsMessage { get; set; }//行政违规情况
            public DateTime? Time { get; set; }//记录年月月份
        }
        public class RinkCalculate
        {
            public double TotalScore { get; set; }//总分
            public string Department { get; set; }//部门
            public string Group { get; set; }//班组
            public string Ethnic_Group { get; set; }//族群
            public decimal AvagePersonNum { get; set; }//平均人数
            public decimal InductrialAccident { get; set; }//当月有无工伤事故
            public decimal Attendance { get; set; }//出勤率
            public string ViolationsMessage { get; set; }//行政违规情况
            public int Ranking { get; set; }//排名
            public int Integral { get; set; }//积分
        }

        //总表显示
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayTotal([System.Web.Http.FromBody] JObject data)
        {
            var obj_i = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            DateTime time = obj_i.time;
            JArray table = new JArray();
            //拿到对的版本，取最大值
            var exetime = db.KPI_Indicators.Where(c => c.ExecutionTime <= time).Max(c => c.ExecutionTime);

            //指标参数录入表（数据来源表）
            var total = db.KPI_Indicators.Where(c => c.ExecutionTime == exetime).Select(c => new TempIndicators { Department = c.Department, Group = c.Group, Cycle = c.Cycle, IndicatorsValueUnit = c.IndicatorsValueUnit, IndicatorsName = c.IndicatorsName, IndicatorsValue = c.IndicatorsValue, SourceDepartment = c.SourceDepartment, Process = c.Process, Section = c.Section, IndicatorsType = c.IndicatorsType, Weight = c.Weight, IndicatorsStatue = c.IndicatorsStatue });
            //品质效率计划表
            var PlanValue = db.Plan_FromKPI.Where(c => c.PlanTime.Value.Year == time.Year).Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.PlanTime, Num = c.PlanNum, Types = c.IndicatorsType, CheckDepartment = c.CheckDepartment, CheckGroup = c.CheckGroup, CheckProcess = c.CheckProcess, CheckSection = c.CheckSection, CheckNum = c.CheckNum });
            //品质效率实际记录表
            var ActualValue = db.KPI_ActualRecord.Where(c => c.ActualTime.Value.Year == time.Year).Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.ActualTime, Num = c.ActualNormalNum, Types = c.IndicatorsType, AbNum = c.ActualAbnormalNum });
            //班组流失率表          
            var TurnoverValue = db.KPI_TurnoverRate.Where(c => c.DateLoss.Year == time.Year && c.DateLoss.Month == time.Month).Select(c => new { c.Id, c.Department, c.Group, c.BeginNumber, c.EndNumber, c.LossNumber, c.DateLoss, c.IndicatorsValue }).ToList();
            //7S
            var KPI_7SValue = db.KPI_7S_Record.Where(c => c.Date.Year == time.Year && c.Date.Month == time.Month).ToList();
            var versionTime = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime <= time).Max(c => c.VersionsTime);
            var positionList = db.KPI_7S_DistrictPosition.Where(c => c.VersionsTime == versionTime).ToList();

            //排名计算的条件
            var TotalDisplayValue = db.KPI_TotalDisplay.Where(c => c.Time.Value.Year == time.Year && c.Time.Value.Month == time.Month).Select(c => new RankingList { Department = c.Department, Group = c.Group, AvagePersonNum = c.AvagePersonNum, InductrialAccident = c.InductrialAccident, Attendance = c.Attendance, ViolationsMessage = c.ViolationsMessage, Time = c.Time, Ethnic_Group = c.Ethnic_Group });
            List<RinkCalculate> calculates = new List<RinkCalculate>();
            List<RinkCalculate> calculates2 = new List<RinkCalculate>();
            JArray DayArray = new JArray();
            var deparment = total.Select(c => c.Department).Distinct();
            foreach (var dep in deparment)
            {
                var group = total.Where(c => c.Department == dep).Select(c => c.Group).Distinct();
                foreach (var groupitem in group)
                {
                    JObject obj = new JObject();
                    obj.Add("Department", dep);
                    obj.Add("Group", groupitem);
                    double QualityGoalScore = 0;
                    double EfficiencyGoalScore = 0;
                    #region 效率指标
                    var efficiency = total.Where(c => c.Department == dep && c.Group == groupitem && c.IndicatorsType == "效率指标").FirstOrDefault();
                    var totalday = DateTime.DaysInMonth(time.Year, time.Month);
                    if (efficiency == null)
                    {
                        obj.Add("Efficiency_IndexName", null);
                        obj.Add("Efficiency_Target", null);
                        obj.Add("Efficiency_Actual", null);
                        obj.Add("Efficiency_Single", null);//单项得分
                        obj.Add("Efficiency_Score", null);//得分小计
                    }
                    else
                    {
                        //Efficiency_IndexName考核指标名
                        obj.Add("Efficiency_IndexName", efficiency.IndicatorsName);
                        //Efficiency_Target目标值（值+单位）
                        obj.Add("Efficiency_Target", efficiency.IndicatorsValue + efficiency.IndicatorsValueUnit);
                        int PlanTotalValue = 0;
                        int ActualTotalValue = 0;
                        List<decimal> oneScore = new List<decimal>();
                        for (var i = 1; i <= totalday; i++)
                        {
                            //创建具体的日期
                            var Ftime = new DateTime(time.Year, time.Month, i);
                            //计划
                            var plancount = 0;
                            var actualcount = 0;
                            var Mkactualcount = 0;
                            var Mkplancount = 0;
                            //数据来源表目标值单位不是%以笔/单/次,这种一般都是没有计划,直接拿生产表中的异常数量
                            if (efficiency.IndicatorsValueUnit != "%")
                            {
                                var actual = ActualValue.Where(c => c.Department == efficiency.Department && c.Group == efficiency.Group && c.Time.Value == Ftime && c.Types == "效率指标").Select(c => c.AbNum).ToList();
                                actualcount = actual.Count != 0 ? actual.Sum() : 0;
                            }
                            else
                            {
                                var sectionlist3 = new List<string>();
                                sectionlist3.Add("送检");//没有找计划拿到工段工序,自己给一个用于在生产/送检拿到数据
                                var planlist = PlanValue.Where(c => c.Department == efficiency.Department && c.Group == efficiency.Group && c.Time.Value == Ftime).ToList();

                                plancount = planlist.Count == 0 ? 0 : planlist.Sum(c => c.Num);
                                if (planlist.Count != 0)
                                {
                                    //var sectionlist = new List<string>();
                                    ////计划部门班组的工段工序列表
                                    //var distinSection = planlist.Select(c => new { c.Section, c.Process }).Distinct().ToList();
                                    //distinSection.ForEach(c => sectionlist.Add(c.Section + c.Process));

                                    //计划部门班组的工段工序列表
                                    var sectionlist = planlist.Select(c => c.Section + c.Process).Distinct().ToList();

                                    //计划不为空,并且单位是以%,计划数为分母,生产数为分子
                                    actualcount = commom.AcuteNum3(dep, groupitem, sectionlist, efficiency.IndicatorsStatue, "效率指标", Ftime);

                                }
                                else
                                {
                                    if (efficiency.Department == "品质部")
                                    {
                                        actualcount = commom.AcuteNum3(efficiency.Department, efficiency.Group, sectionlist3, "正常", "效率指标", Ftime, "模组");
                                        Mkactualcount = commom.AcuteNum3(efficiency.Department, efficiency.Group, sectionlist3, "正常", "效率指标", Ftime, "模块");
                                        plancount = commom.AcuteNum3(efficiency.Department, efficiency.Group, sectionlist3, "", "效率指标", Ftime, "模组");
                                        Mkplancount = commom.AcuteNum3(efficiency.Department, efficiency.Group, sectionlist3, "", "效率指标", Ftime, "模块");
                                        //只有模组算模组,只有模块算模块,有模组模块算平均
                                        var mz = plancount == 0 ? 0 : (actualcount * 100 / plancount);
                                        var mk = Mkplancount == 0 ? 0 : (Mkactualcount * 100 / Mkplancount);
                                        var value = mz == 0 ? mk : (mk == 0 ? mz : Math.Round((decimal)(mk + mz) / 2, 2));

                                        if (value != 0)
                                        { oneScore.Add(value); }
                                    }
                                    else
                                    {
                                        //拿到检验部门所负责的部门班组列表
                                        var departmentList = PlanValue.Where(c => c.CheckDepartment == efficiency.Department && c.CheckGroup == efficiency.Group && c.Time.Value == Ftime).Select(c => new { c.Department, c.Group }).Distinct().ToList();

                                        //计划为空,单位是以%,并且没有有所负责的部门班组列表 ,单纯没做计划,直接显示生产信息
                                        if (departmentList.Count == 0)
                                        {
                                            actualcount = commom.AcuteNum3(dep, groupitem, sectionlist3, efficiency.IndicatorsStatue, "效率指标", Ftime);
                                        }
                                        //计划为空,单位是以%,并且有所负责的部门班组列表 ,一般为检验部门,检验部门一般不做计划 效率指标计算是检验数/生产数  品质指标是不漏件数/检验数
                                        else
                                        {
                                            foreach (var onedep in departmentList)
                                            {
                                                //var sectionlist = new List<string>();
                                                //var sectionlist2 = PlanValue.Where(c => c.Department == onedep.Department && c.Group == onedep.Group && c.Time.Value == Ftime).Select(c => new { c.Section, c.Process }).Distinct().ToList();
                                                //sectionlist2.ForEach(c => sectionlist.Add(c.Section + c.Process));

                                                var sectionlist = PlanValue.Where(c => c.Department == onedep.Department && c.Group == onedep.Group && c.Time.Value == Ftime).Select(c => c.Section + c.Process).Distinct().ToList();


                                                //拿到所负责部门班组列表的生产正常记录,用于做分母
                                                plancount = plancount + commom.AcuteNum3(onedep.Department, onedep.Group, sectionlist, "正常", "效率指标", Ftime);
                                            }
                                            //拿到所负责部门班组列表的品质记录,正常+异常,用于做分子

                                            actualcount = actualcount + commom.AcuteNum3(efficiency.Department, efficiency.Group, sectionlist3, "", "品质指标", Ftime);

                                        }
                                    }
                                }
                            }
                            //实际
                       
                            //第二版本V2.0
                            PlanTotalValue = PlanTotalValue + plancount;
                            ActualTotalValue = ActualTotalValue + actualcount;

                        }
                        double difference = 0;
                        if (efficiency.IndicatorsValueUnit == "%")
                        {
                            double score = 0;
                            if (oneScore.Count != 0)
                            {
                                score = Math.Round((double)oneScore.Sum() / oneScore.Count(), 2);
                            }
                            else
                            {
                                score = PlanTotalValue == 0 ? 0 : Math.Round((double)ActualTotalValue * 100 / PlanTotalValue, 2);
                            }
                            obj.Add("Efficiency_Actual", score);//实际得分
                            if (efficiency.IndicatorsStatue == "正常")
                            {
                                difference = score - efficiency.IndicatorsValue;
                            }
                            else
                            {
                                difference = efficiency.IndicatorsValue - score;
                            }

                        }
                        else
                        {
                            obj.Add("Efficiency_Actual", ActualTotalValue);//实际得分
                            if (efficiency.IndicatorsStatue == "正常")
                            {
                                difference = ActualTotalValue - efficiency.IndicatorsValue;
                            }
                            else
                            {
                                difference = efficiency.IndicatorsValue - ActualTotalValue;
                            }
                        }

                        obj.Add("Efficiency_Single", difference >= 0 ? 100 : 0);//单项得分
                        EfficiencyGoalScore = difference >= 0 ? 35 : 0;
                        obj.Add("Efficiency_Score", EfficiencyGoalScore);//得分小计
                    }
                    #endregion

                    #region 品质指标
                    var quality = total.Where(c => c.Department == dep && c.Group == groupitem && c.IndicatorsType == "品质指标").FirstOrDefault();
                    if (quality == null)
                    {
                        obj.Add("Quality_IndexName", null);
                        obj.Add("Quality_Target", null);
                        obj.Add("Quality_Actual", null);//实际得分
                        obj.Add("Quality_Single", null);//单项得分
                        obj.Add("Quality_Score", null);//得分小计
                    }
                    else
                    {
                        obj.Add("Quality_IndexName", quality.IndicatorsName);
                        obj.Add("Quality_Target", quality.IndicatorsValue + quality.IndicatorsValueUnit);
                        int qualityPlanTotalValue = 0;
                        int qualityActualTotalValue = 0;
                        var qualitytotalday = DateTime.DaysInMonth(time.Year, time.Month);
                        for (var i = 1; i <= totalday; i++)
                        {
                            var Ftime = new DateTime(time.Year, time.Month, i);
                            //计划
                            var plancount = 0;
                            var actualcount = 0;
                            //数据来源表目标值单位不是%以笔/单/次,这种一般都是没有计划,直接拿生产表中的异常数量
                            if (quality.IndicatorsValueUnit != "%")
                            {
                                var actual = ActualValue.Where(c => c.Department == quality.Department && c.Group == quality.Group && c.Time.Value == Ftime && c.Types == "品质指标").Select(c => c.AbNum).ToList();
                                actualcount = actual.Count != 0 ? actual.Sum() : 0;
                            }
                            else
                            {

                                var sectionlist = new List<string>();
                                sectionlist.Add("送检");//没有找计划拿到工段工序,自己给一个用于在生产/送检拿到数据
                                plancount = commom.AcuteNum3(quality.Department, quality.Group, sectionlist, "", "品质指标", Ftime);
                                actualcount = commom.AcuteNum3(quality.Department, quality.Group, sectionlist, quality.IndicatorsStatue, "品质指标", Ftime);
                            }
                            qualityPlanTotalValue = qualityPlanTotalValue + plancount;
                            qualityActualTotalValue = qualityActualTotalValue + actualcount;

                        }

                        double difference2 = 0;
                        if (quality.IndicatorsValueUnit == "%")
                        {
                            obj.Add("Quality_Actual", qualityPlanTotalValue == 0 ? 0 : Math.Round((decimal)qualityActualTotalValue * 100 / qualityPlanTotalValue, 2));//实际得分
                            if (quality.IndicatorsStatue == "正常")
                            {
                                difference2 = (qualityPlanTotalValue == 0 ? 0 : (qualityActualTotalValue * 100 / qualityPlanTotalValue)) - quality.IndicatorsValue;
                            }
                            else
                            {
                                difference2 = qualityPlanTotalValue == 0 ? 0 : quality.IndicatorsValue - (qualityActualTotalValue * 100 / qualityPlanTotalValue);
                            }

                        }
                        else
                        {
                            obj.Add("Quality_Actual", qualityActualTotalValue);//实际得分
                            if (quality.IndicatorsStatue == "正常")
                            {
                                difference2 = qualityActualTotalValue - quality.IndicatorsValue;
                            }
                            else
                            {
                                difference2 = quality.IndicatorsValue - qualityActualTotalValue;
                            }
                        }
                        obj.Add("Quality_Single", difference2 >= 0 ? 100 : 0);//单项得分
                        QualityGoalScore = difference2 >= 0 ? 35 : 0;//得分小计
                        obj.Add("Quality_Score", QualityGoalScore);//得分小计
                    }
                    #endregion

                    #region 月度流失率指标TurnoverValue
                    double TurnoverGoalScore = 0;
                    var turnover = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).Select(c => c.IndicatorsValue).FirstOrDefault();
                    if (turnover == 0)
                    {
                        obj.Add("Turnover_IndexName", null);
                        obj.Add("Turnover_Target", null);
                        obj.Add("Turnover_Actual", null);//实际得分
                        obj.Add("Turnover_Single", null);//单项得分
                        obj.Add("Turnover_Score", null);//得分小计
                    }
                    else
                    {
                        obj.Add("Turnover_IndexName", "班组流失率");
                        obj.Add("Turnover_Target", turnover + "%");
                        double averageNum = 0;  //月平均人数
                        double actual = 0;
                        double IndividualScores = 0;
                        //月初人数
                        var beginNumber = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).OrderBy(c => c.Id).Select(c => c.BeginNumber).FirstOrDefault();
                        //月末人数最少
                        var endNumber = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).OrderByDescending(c => c.Id).Select(c => c.EndNumber).FirstOrDefault();
                        double num = beginNumber + endNumber;
                        averageNum = Math.Ceiling(num / 2);
                        int turnoverTotalValue = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).Sum(c => c.LossNumber);
                        if (averageNum == 0)
                        {
                            actual = 0;
                        }
                        else
                        {
                            actual = Math.Round(turnoverTotalValue / averageNum, 2) * 100;
                        }
                        if (actual <= turnover)
                        {
                            IndividualScores = 100;
                        }
                        else if (actual > turnover)
                        {
                            IndividualScores = 100 - (actual - turnover);
                        }
                        double weight = 0.15;
                        obj.Add("Turnover_Actual", actual + "%");//实际得分                       
                        obj.Add("Turnover_Single", IndividualScores);//单项得分
                        TurnoverGoalScore = IndividualScores * weight;//得分小计
                        obj.Add("Turnover_Score", TurnoverGoalScore.ToString("0.##"));//得分小计
                    }
                    #endregion

                    #region 7S评比
                    var KPI_7SValue_List = KPI_7SValue.Where(c => c.Department == dep && c.Group == groupitem).ToList();
                    var KPI_7SValue_List_Week = KPI_7SValue_List.Where(c => c.Check_Type == "周检").ToList();
                    var KPI_7SValue_List_Random = KPI_7SValue_List.Where(c => c.Check_Type == "巡检").ToList();

                    var Grade_daily = KPI_7SValue_List.Where(c => c.Check_Type == "日检").Sum(c => c.PointsDeducted);//日检正常扣分
                    var Grade_week = KPI_7SValue_List_Week.Sum(c => c.PointsDeducted);//周检正常扣分
                    var Week_NotCorrected = KPI_7SValue_List_Week.Sum(c => c.RectificationPoints);//周检未整改
                    var Random_NotCorrected = KPI_7SValue_List_Random.Sum(c => c.RectificationPoints);//巡检未整改
                    var Week_repetition = KPI_7SValue_List_Week.Sum(c => c.RepetitionPointsDeducted);//周检重复出现
                    var Random_repetition = KPI_7SValue_List_Random.Sum(c => c.RepetitionPointsDeducted);//巡检重复出现
                    var Grade_random = KPI_7SValue_List_Random.Sum(c => c.PointsDeducted);//抽检正常扣分


                    decimal actualscore = 100 - Grade_daily - Grade_week - (Week_NotCorrected + Random_NotCorrected) - (Week_repetition + Random_repetition) - Grade_random;//实际得分

                    var comparison = positionList.Where(c => c.Department == dep && c.Group == groupitem).FirstOrDefault();
                    double ComparisonGoalScore = 0;
                    if (comparison == null)
                    {
                        obj.Add("Comparison_IndexName", null);
                        obj.Add("Comparison_Target", null);
                        obj.Add("Comparison_Actual", null);//实际得分
                        obj.Add("Comparison_Single", null);//单项得分
                        obj.Add("Comparison_Score", null);//得分小计
                    }
                    else
                    {
                        obj.Add("Comparison_IndexName", "7S评比指标");
                        obj.Add("Comparison_Target", comparison.TargetValue);
                        obj.Add("Comparison_Actual", (double)actualscore <= 0.00 ? "0" : actualscore.ToString("0.##"));//实际得分                       
                        double KPI7STurnoverGoal = 0;
                        if ((double)actualscore >= comparison.TargetValue)
                        {
                            KPI7STurnoverGoal = 100;
                        }
                        else if ((double)actualscore < comparison.TargetValue)
                        {
                            KPI7STurnoverGoal = 100 - (comparison.TargetValue - (double)actualscore);
                        }
                        obj.Add("Comparison_Single", KPI7STurnoverGoal <= 0.00 ? "0" : KPI7STurnoverGoal.ToString("0.##"));//单项得分
                        double Weight_7S = 0.15;
                        ComparisonGoalScore = KPI7STurnoverGoal * Weight_7S;
                        obj.Add("Comparison_Score", ComparisonGoalScore <= 0.00 ? "0" : ComparisonGoalScore.ToString("0.##"));//得分小计
                    }
                    #endregion
                    var conditions = TotalDisplayValue.Where(c => c.Department == dep && c.Group == groupitem).Select(c => new { c.InductrialAccident, c.ViolationsMessage, c.AvagePersonNum, c.Attendance, c.Ethnic_Group }).FirstOrDefault();
                    double TotalScore = EfficiencyGoalScore + QualityGoalScore + TurnoverGoalScore + ComparisonGoalScore;//合计总分
                    obj.Add("TotalScore", TotalScore);//合计总分
                    obj.Add("Greater", TotalScore >= 90 == true ? true : false); //合计总分是否大于90
                    decimal avagePersonNum = 0;
                    if (conditions == null)
                    {
                        //月初人数
                        var beginNumber = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).OrderBy(c => c.Id).Select(c => c.BeginNumber).FirstOrDefault();
                        //月末人数最少
                        var endNumber = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).OrderByDescending(c => c.Id).Select(c => c.EndNumber).FirstOrDefault();
                        decimal num = beginNumber + endNumber;
                        avagePersonNum = Math.Ceiling(num / 2);
                        obj.Add("AvagePersonNum", avagePersonNum);//平均人数
                        obj.Add("InductrialAccident", 0);//当月有无工伤事故
                        obj.Add("Ethnic_Group", 0);//族群
                        obj.Add("Attendance", 0);//出勤率
                        obj.Add("ViolationsMessage", "无");//行政违纪情况
                        obj.Add("Ranking", 0);//排名
                        obj.Add("integral", 0);//积分
                    }
                    else
                    {
                        if (conditions.AvagePersonNum == 0)
                        {
                            //月初人数
                            var beginNumber = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).OrderBy(c => c.Id).Select(c => c.BeginNumber).FirstOrDefault();
                            //月末人数最少
                            var endNumber = TurnoverValue.Where(c => c.Department == dep && c.Group == groupitem).OrderByDescending(c => c.Id).Select(c => c.EndNumber).FirstOrDefault();
                            decimal num = beginNumber + endNumber;
                            avagePersonNum = Math.Ceiling(num / 2);
                            obj.Add("AvagePersonNum", avagePersonNum);//平均人数
                        }
                        else
                        {
                            avagePersonNum = conditions.AvagePersonNum;
                            obj.Add("AvagePersonNum", conditions.AvagePersonNum);//平均人数
                        }
                        obj.Add("InductrialAccident", conditions.InductrialAccident == 0 ? 0 : conditions.InductrialAccident);//当月有无工伤事故
                        obj.Add("Ethnic_Group", conditions.Ethnic_Group == null ? null : conditions.Ethnic_Group);//族群
                        var attendance = (conditions.Attendance * 100);
                        obj.Add("Attendance", attendance);
                        obj.Add("ViolationsMessage", conditions.ViolationsMessage == null ? null : conditions.ViolationsMessage);//行政违纪情况
                        obj.Add("Ranking", 0);//排名
                        obj.Add("integral", 0);//积分
                        RinkCalculate rink = new RinkCalculate();
                        rink.Department = dep;
                        rink.Group = groupitem;
                        rink.TotalScore = TotalScore;
                        rink.AvagePersonNum = avagePersonNum;
                        rink.Ethnic_Group = conditions.Ethnic_Group;
                        rink.Attendance = conditions.Attendance;
                        rink.InductrialAccident = conditions.InductrialAccident;
                        rink.ViolationsMessage = conditions.ViolationsMessage;
                        calculates.Add(rink);
                        calculates2.Add(rink);

                    }
                    DayArray.Add(obj);
                    obj = new JObject();
                }
            }
            var rank = new JArray(DayArray.OrderByDescending(c => c["TotalScore"]));//按总分排序
            calculates = calculates.Where(c => c.TotalScore >= 90 && c.AvagePersonNum >= 5 && c.InductrialAccident == 0 && c.Ethnic_Group == "操作族").OrderByDescending(c => c.TotalScore).ThenByDescending(c => c.Attendance).ToList();
            //排名计算
            int ranking = 1;
            #region---三列相同判断的排名方法
            for (int k = 0; k < calculates.Count(); k++)
            {
                if (ranking > 5) break;
                var it = calculates[k];
                var count_three = calculates.Where(c => c.TotalScore == it.TotalScore && c.Attendance == it.Attendance);
                if (count_three.Count() > 1)
                {
                    foreach (var ii in count_three)
                    {
                        calculates[k].Ranking = ii.Ranking == 0 ? ranking : ii.Ranking;
                        calculates[k].Integral = Integral_Manage(ranking);
                        k++;
                    }
                    k--;
                }
                else
                {
                    calculates[k].Ranking = ranking;
                    calculates[k].Integral = Integral_Manage(ranking);
                }
                ranking++;
            }
            //把排名放回汇总表
            foreach (var addval in calculates)
            {
                rank.Where(c => c["Department"].ToString() == addval.Department && c["Group"].ToString() == addval.Group).FirstOrDefault()["Ranking"] = addval.Ranking;
                rank.Where(c => c["Department"].ToString() == addval.Department && c["Group"].ToString() == addval.Group).FirstOrDefault()["integral"] = addval.Integral;
            }

            #endregion

            #region---三列相同判断的倒数排名方法
            calculates2 = calculates2.Where(c => c.AvagePersonNum >= 5 && c.Ethnic_Group == "操作族").OrderBy(c => c.TotalScore).ThenByDescending(c => c.ViolationsMessage).ThenBy(c => c.Attendance).ToList();
            var re_pm = calculates2.Select(c => c.TotalScore).Distinct().Take(3).LastOrDefault();//取总得分最低分的三个
            calculates2 = calculates2.Where(c => c.TotalScore <= re_pm).ToList();
            int ranking1 = 1;
            for (int g = 0; g < calculates2.Count(); g++)
            {
                if (ranking1 > 3) break;
                var tg = calculates2[g];
                var count_three = calculates2.Where(c => c.TotalScore == tg.TotalScore && c.ViolationsMessage == tg.ViolationsMessage && c.Attendance == tg.Attendance);
                if (count_three.Count() > 1)
                {
                    foreach (var ip in count_three)
                    {
                        calculates2[g].Ranking = ip.Ranking == 0 ? -ranking1 : ip.Ranking;
                        calculates2[g].Integral = Integral_Manage(-ranking1);
                        g++;
                    }
                    g--;
                }
                else
                {
                    calculates2[g].Ranking = -ranking1;
                    calculates2[g].Integral = Integral_Manage(-ranking1);
                }
                ranking1++;
            }
            //把倒数排名放回汇总表
            foreach (var addval in calculates2)
            {
                rank.Where(c => c["Department"].ToString() == addval.Department && c["Group"].ToString() == addval.Group).FirstOrDefault()["Ranking"] = addval.Ranking;
                rank.Where(c => c["Department"].ToString() == addval.Department && c["Group"].ToString() == addval.Group).FirstOrDefault()["integral"] = addval.Integral;
            }
            #endregion

            JObject code = new JObject();
            JObject result = new JObject();
            JObject confirm = new JObject();//确认
            JObject audit = new JObject();//审核
            JObject approved = new JObject();//核准
            var codelist = db.KPI_ReviewSummary.Where(c => c.Year == time.Year && c.Month == time.Month).ToList();
            if (codelist.Count == 0)
            {
                code.Add("Code", null);
            }
            else
            {
                foreach (var it in codelist)
                {
                    if (it.Type == "确认")
                    {
                        confirm.Add("HRAuditDate", it.HRAuditDate == null ? null : it.HRAuditDate.ToString());
                        confirm.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                        confirm.Add("HRjudge", it.HRjudge == false ? false : it.HRjudge);
                    }
                    if (it.Type == "审核")
                    {
                        audit.Add("HRAuditDate", it.HRAuditDate == null ? null : it.HRAuditDate.ToString());
                        audit.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                        audit.Add("HRjudge", it.HRjudge == false ? false : it.HRjudge);
                    }
                    if (it.Type == "核准")
                    {
                        approved.Add("HRAuditDate", it.HRAuditDate == null ? null : it.HRAuditDate.ToString());
                        approved.Add("HRAudit", it.HRAudit == null ? null : it.HRAudit);
                        approved.Add("HRjudge", it.HRjudge == false ? false : it.HRjudge);
                    }
                }
                code.Add("Confirm", confirm);//确认
                confirm = new JObject();
                code.Add("Audit", audit);//审核
                audit = new JObject();
                code.Add("Approved", approved);//核准
                approved = new JObject();
            }
            result.Add("Code", code);
            result.Add("Rank", rank);
            return commom.GetModuleFromJobjet(result);
        }

        //积分管理
        public int Integral_Manage(int t)
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
            return s;
        }     

        #endregion


    }
}
