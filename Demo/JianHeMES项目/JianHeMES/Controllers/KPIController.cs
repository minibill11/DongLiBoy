﻿using JianHeMES.Models;
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
    public class KPIController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: KPI
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "Index" });
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


        //各工序直通率查询
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
            var groupSampl = db.AfterWelding.Where(c => orderlist.Contains(c.Ordernum) && c.IsSampling == true).Select(c => new Temp { Group = c.Group, Finish = c.SamplingResult, BarCodesNum = c.ModuleBarcode }).ToList();
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
            var burngroup = db.Burn_in.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c => new Temp { Group = c.Group, BarCodesNum = c.BarCodesNum, Finish = c.OQCCheckFinish }).ToList();
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
                    obj.Add("group", item);
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
                    int totalNum = temps.Count(c => c.Group == item || c.Group == "");
                    //合格率
                    var abnormal = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == false).Select(c => c.BarCodesNum).ToList();

                    //直通率
                    var pass = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == true).Count();

                    var info = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == true && !abnormal.Contains(c.BarCodesNum)).ToList();
                    var passtrough = info.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();

                    obj.Add("passThrough", totalNum == 0 ? "-%" : Math.Round((double)passtrough * 100 / totalNum, 2) + "%");
                    obj.Add("abnormal", totalNum == 0 ? "-%" : Math.Round((double)pass * 100 / totalNum, 2) + "%");

                    array.Add(obj);
                }
                else
                {
                    obj.Add("group", item);
                    int totalNum = temps.Count(c => c.Group == item);
                    //异常率
                    var abnormal = temps.Where(c => c.Group == item && c.Finish == false).Select(c => c.BarCodesNum).ToList();

                    var pass = temps.Where(c => (c.Group == item || c.Group == "") && c.Finish == true).Count();

                    //直通率
                    var info = temps.Where(c => c.Group == item && c.Finish == true && !abnormal.Contains(c.BarCodesNum)).ToList();
                    var passtrough = info.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    obj.Add("passThrough", totalNum == 0 ? "-%" : Math.Round((double)passtrough * 100 / totalNum, 2) + "%");
                    obj.Add("abnormal", totalNum == 0 ? "-%" : Math.Round((double)pass * 100 / totalNum, 2) + "%");

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
                totalobj.Add("passThrough", temps.Count == 0 ? "-%" : Math.Round((double)totalpasstrough * 100 / temps.Count, 2) + "%");
                var pass = temps.Where(c => c.Finish == true).Count();
                //合计异常率
                totalobj.Add("abnormal", temps.Count == 0 ? "-%" : Math.Round((double)pass * 100 / temps.Count, 2) + "%");
                array.Add(totalobj);
            }
            result.Add("array", array);
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

        //批量上传班组流失率
        public ActionResult DeparturesNum(List<KPI_TurnoverRate> turnoverRates, DateTime dateLoss)
        {
            JObject loss = new JObject();
            JArray copy = new JArray();
            string rates = null;
            if (turnoverRates.Count > 0 && dateLoss != null)
            {
                foreach (var item in turnoverRates)
                {
                    if (db.KPI_TurnoverRate.Count(c => c.Department == item.Department && c.Group == item.Group && c.DateLoss == dateLoss) > 0)
                    {
                        rates = rates + item.Department + "," + item.Group + "," + dateLoss + "的记录重复了";
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

        //查询班组人员流失汇总表
        public ActionResult Turnover_CheckSshift(int? year, int? month)
        {
            JObject Rate = new JObject();
            JArray table = new JArray();
            JObject retul = new JObject();
            var checkList = db.KPI_TurnoverRate.Select(c => c.Department).Distinct().ToList();
            List<KPI_TurnoverRate> Turnover = new List<KPI_TurnoverRate>();
            int averageNum = 0;//平均人数
            int turnoverMonth = 0;//月流失人数
            double actual = 0;//实际得分
            double differences = 0;//与目标值的差异
            double subtotal = 0;//得分小计
            foreach (var ite in checkList)
            {
                var group = db.KPI_TurnoverRate.Where(c => c.Department == ite).Select(c => c.Group).Distinct().ToList();
                foreach (var it in group)
                {
                    if (year != 0 && month != 0)
                    {
                        Turnover = db.KPI_TurnoverRate.Where(c => c.DateLoss.Year == year && c.DateLoss.Month == month && c.Department == ite && c.Group == it).ToList();
                    }
                    if (year != 0 && month == 0)
                    {
                        Turnover = db.KPI_TurnoverRate.Where(c => c.DateLoss.Year == year && c.Department == ite && c.Group == it).ToList();
                    }
                    if (Turnover.Count > 0)
                    {
                        foreach (var item in Turnover)
                        {
                            //Id
                            Rate.Add("Id", item.Id == 0 ? 0 : item.Id);
                            //被考核部门
                            Rate.Add("Department", ite == null ? null : ite);
                            //班组
                            Rate.Add("Group", it == null ? null : it);
                            //指标名称
                            var indexName = db.KPI_Indicators.Where(c => c.Department == ite && c.Group == it && c.SourceDepartment == "人力资源部").Select(c => c.IndicatorsName).FirstOrDefault();
                            Rate.Add("IndicatorsName", indexName == null ? null : indexName);
                            //目标值
                            var indicators = db.KPI_Indicators.Where(c => c.Department == ite && c.Group == it && c.IndicatorsName == indexName).Select(c => c.IndicatorsValue).FirstOrDefault();
                            Rate.Add("IndicatorsValue", indicators);
                            //数据来源部门
                            Rate.Add("SourceDepartment", "人力资源部");
                            //月初人数
                            Rate.Add("BeginNumber", item.BeginNumber == 0 ? 0 : item.BeginNumber);
                            //月末人数
                            Rate.Add("EndNumber", item.EndNumber == 0 ? 0 : item.EndNumber);
                            //平均人数
                            averageNum = (item.BeginNumber + item.EndNumber) / 2;
                            Rate.Add("AverageNum", averageNum == 0 ? 0 : averageNum);
                            //流失日期
                            Rate.Add("DateLoss", item.DateLoss);
                            //流失人数
                            Rate.Add("LossNumber", item.LossNumber == 0 ? 0 : item.LossNumber);
                            //月流失人数
                            turnoverMonth = turnoverMonth + item.LossNumber;
                            //月流失率（实际得分）
                            actual = actual + (turnoverMonth / averageNum);
                            //与目标值的差异
                            double indicators_sum =indicators;
                            differences = differences + (indicators_sum - actual);
                            Rate.Add("Differences", differences == 0 ? 0 : differences);
                            //得分小计
                            if (actual <= indicators_sum)
                            {
                                subtotal = 100;
                                Rate.Add("Subtotal", subtotal);
                            }
                            else if (actual > indicators_sum)
                            {
                                subtotal = 100 - (indicators_sum - differences);
                                Rate.Add("Subtotal", subtotal);
                            }
                            table.Add(Rate);
                            Rate = new JObject();
                        }
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(table));
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

        public ActionResult KPI_7S_GradeStandardQuery()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_7S_GradeStandardQuery" });
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

        public ActionResult KPI_7S_RegionQuery()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "KPI_7S_RegionQuery" });
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
                    if (db.KPI_7S_DistrictPosition.Count(c => c.Department == item.Department && c.Group == item.Group && c.Position == item.Position && c.District == item.District) < 1)
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
        public ActionResult Region_modify(int id, string department, string position, int district,string group)
        {
            if (id != 0 && !String.IsNullOrEmpty(department) && !String.IsNullOrEmpty(position) && district != 0)
            {
                //检查修改记录的是否已存在
                if (db.KPI_7S_DistrictPosition.Count(c => c.Department == department && c.Position == position && c.District == district&&c.Group== group) < 1)
                {
                    var record = db.KPI_7S_DistrictPosition.Where(c => c.Id == id).FirstOrDefault();
                    record.Department = department;//部门
                    record.Position = position;//位置
                    record.District = district;//区域号
                    record.Group = group;//班组
                    record.ModifyPerson = ((Users)Session["User"]).UserName;//修改人
                    record.ModifyTime = DateTime.Now;//修改时间
                    int count = db.SaveChanges();
                    if (count > 0) return Content("修改成功！");
                    else return Content("修改失败！");
                }
                else return Content("修改失败，记录已存在！");
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
        public ActionResult Region_query(string department, int? district)
        {
            IEnumerable<KPI_7S_DistrictPosition> recordList = db.KPI_7S_DistrictPosition;
            if (!String.IsNullOrEmpty(department))
            {
                recordList = recordList.Where(c => c.Department == department).ToList();
            }
            if (district != null)
            {
                recordList = recordList.Where(c => c.District == district).ToList();
            }
            var res = recordList.Select(c => new { c.Id, c.Department, c.Position, c.District,c.Group}).ToList();
            return Content(JsonConvert.SerializeObject(res));
        }
        #endregion

        #region--获取部门下拉列表
        public ActionResult GetDepartmentList()
        {
            var departmentList = db.KPI_7S_DistrictPosition.Select(c => c.Department).Distinct().ToList();
            JArray result = new JArray();
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
        public ActionResult GetDistrictList(string department)
        {
            if (!String.IsNullOrEmpty(department))
            {
                var districtList = db.KPI_7S_DistrictPosition.Where(c=>c.Department==department).Select(c => c.District).Distinct().ToList();
                JArray result = new JArray();
                JObject record = new JObject();
                if (districtList.Count > 0) {
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

        #endregion

        #endregion


        #region 指标名称清单数据录入 
        //显示
        public ActionResult DisplayKPIIndicators(DateTime? time)
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
                value = db.KPI_Indicators.Where(c => c.ExecutionTime == time).ToList();
            }
            foreach (var item in value)
            {
                JObject obj = new JObject();
                obj.Add("Department", item.Department);//部门
                obj.Add("Group", item.Group);//班组
                obj.Add("IndicatorsName", item.IndicatorsName);//指标名
                obj.Add("IndicatorsDefine", item.IndicatorsDefine);//指标定义
                obj.Add("ComputationalFormula", item.ComputationalFormula);//指标计算工时
                obj.Add("IndicatorsValue", item.IndicatorsValue);//目标值
                obj.Add("DataName", item.DataName);//数据名称
                obj.Add("Cycle", item.Cycle);//数据周期
                obj.Add("SourceDepartment", item.SourceDepartment);//来源部门
                obj.Add("DataInputor", item.DataInputor);//录入人
                obj.Add("DataInputTime", item.DataInputTime);//录入时间
                obj.Add("IndicatorsType", item.IndicatorsType);//考核类型  品质或效率
                obj.Add("Process", item.Process);//考核工段
                obj.Add("Section", item.Section);//考核工序
                obj.Add("IndicatorsStatue", item.IndicatorsStatue);//考核异常或者正常
                //obj.Add("QualityStatue", item.QualityStatue);//品质考核类型, 直通 或者合格率

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
            var num = db.KPI_Indicators.Count(c => c.ExecutionTime == indicators.FirstOrDefault().ExecutionTime);
            if (num != 0)
            {
                return "已有重复的版本信息";
            }
            db.KPI_Indicators.AddRange(indicators);
            db.SaveChanges();
            return "true";
        }

        //修改
        public void UpdateKPIIndicators(KPI_Indicators indicators)
        {
            var info = db.KPI_Indicators.Find(indicators.Id);
            if (info.ExecutionTime != indicators.ExecutionTime)
            {
                db.KPI_Indicators.Add(indicators);
                db.SaveChanges();
            }
            else
            {
                db.Entry(indicators).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        //删除 
        public void DeleteKPIIndicators(int id)
        {
            var deleteinfo = db.KPI_Indicators.Find(id);
            db.KPI_Indicators.Remove(deleteinfo);
            db.SaveChanges();
        }

        #endregion

        #region 品质效率实际记录
        //显示
        public ActionResult DisplayActualRecord(string ordernum, string deparment, string group, string process, string section, DateTime? time)
        {
            JArray result = new JArray();
            var totalvalue = db.KPI_ActualRecord.ToList();
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
                totalvalue = totalvalue.Where(c => c.ActualTime == time).ToList();
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
                obj.Add("ActualNormalNum", item.ActualNormalNum);//正常数量
                obj.Add("ActualAbnormalNum", item.ActualAbnormalNum);//异常数量
                obj.Add("Message", item.ActualAbnormalDescription);//异常信息
                result.Add(obj);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //新增
        public string AddActualRecord(KPI_ActualRecord actualRecord)
        {
            var info = db.KPI_ActualRecord.Count(c => c.ActualTime == actualRecord.ActualTime && c.OrderNum == actualRecord.OrderNum && c.Department == actualRecord.Department && c.Group == actualRecord.Group && c.Process == actualRecord.Process && c.Section == actualRecord.Section);
            if (info != 0)
            {
                return "已有重复记录";

            }
            actualRecord.ActualCreateor = ((Users)Session["User"]) == null ? "张三" : ((Users)Session["User"]).UserName;
            actualRecord.ActualCreateTime = DateTime.Now;
            db.KPI_ActualRecord.Add(actualRecord);
            db.SaveChanges();
            return "true";

        }
        //修改
        public string UpdateActualRecord(int id, KPI_ActualRecord actualRecord)
        {
            var info = db.KPI_ActualRecord.Find(id);
            if (info == null)
            {
                return "没有找到数据";
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
            return "true";

        }

        //删除

        public string DeleteActualRecord(int id)
        {
            var info = db.KPI_ActualRecord.Find(id);
            if (info == null)
            {
                return "找不到该信息";
            }
            db.KPI_ActualRecord.Remove(info);
            db.SaveChanges();
            return "true";
        }

        #endregion

        #region  KPI显示
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
        }

        public class TempValue
        {
            public string Department { get; set; }
            public string Group { get; set; }
            public string Process { get; set; }
            public string Section { get; set; }
            public DateTime? Time { get; set; }
            public int Num { get; set; }

        }

        //总表显示
        public ActionResult DisplayTotal(DateTime time)

        {
            JObject result = new JObject();
            //拿到对的版本
            var version = db.KPI_Indicators.Where(c => c.ExecutionTime < time).Select(c => c.ExecutionTime).Distinct().ToList();
            if (version != null)
            {
                //取值
                var exetime = version.Max();
                var total = db.KPI_Indicators.Where(c => c.ExecutionTime == exetime).Select(c => new TempIndicators { Department = c.Department, Group = c.Group, Cycle = c.Cycle, IndicatorsValueUnit = c.IndicatorsValueUnit, IndicatorsName = c.IndicatorsName, IndicatorsValue = c.IndicatorsValue, SourceDepartment = c.SourceDepartment, Process = c.Process, Section = c.Section, IndicatorsType = c.IndicatorsType });

                var PlanValue = db.Plan_FromKPI.Where(c => c.PlanTime.Value.Year == time.Year).Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.PlanTime, Num = c.PlanNum });

                var ActualValue = db.KPI_ActualRecord.Where(c => c.ActualTime.Value.Year == time.Year).Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.ActualTime, Num = c.ActualNormalNum });

                //  var KpiInfo = db.KPI_TotalDisplay.Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.ActualTime, Num = c.ActualNormalNum });

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
                        //效率指标
                        var efficiency = total.Where(c => c.Department == dep && c.Group == groupitem && c.IndicatorsType == "效率").FirstOrDefault();
                        obj.Add("EfficiencyIndicatorsName", efficiency.IndicatorsName);
                        obj.Add("EfficiencyIndicatorsValue", efficiency.IndicatorsValue + efficiency.IndicatorsValueUnit);
                        int PlanTotalValue = 0;
                        int ActualTotalValue = 0;
                        var totalday = DateTime.DaysInMonth(time.Year, time.Month);
                        for (var i = 1; i <= totalday; i++)
                        {
                            var Ftime = new DateTime(time.Year, time.Month, i);
                            //计划
                            var plancount = PlanValue.Where(c => c.Process == efficiency.Process && c.Section == efficiency.Section && c.Department == dep && c.Group == groupitem && c.Time.Value == Ftime).Select(c => c.Num).FirstOrDefault();

                            //实际
                            var actualcount = ActualValue.Where(c => c.Process == efficiency.Process && c.Section == efficiency.Section && c.Department == dep && c.Group == groupitem && c.Time.Value == Ftime).Select(c => c.Num).FirstOrDefault();
                            PlanTotalValue = PlanTotalValue + plancount;
                            ActualTotalValue = ActualTotalValue + actualcount;

                        }
                        obj.Add("EfficiencyActualScore", Math.Round((decimal)ActualTotalValue / PlanTotalValue, 2));//实际得分
                        obj.Add("EfficiencyGoal", (ActualTotalValue / PlanTotalValue) - efficiency.IndicatorsValue > 0 ? 100 : 0);//单项得分
                        obj.Add("EfficiencyGoalScore", (ActualTotalValue / PlanTotalValue) - efficiency.IndicatorsValue > 0 ? 35 : 0);//得分小计
                        //品质指标
                        var quality = total.Where(c => c.Department == dep && c.Group == groupitem && c.IndicatorsType == "品质").FirstOrDefault();
                        obj.Add("QualityIndicatorsName", quality.IndicatorsName);
                        obj.Add("QualityIndicatorsValue", quality.IndicatorsValue + quality.IndicatorsValueUnit);
                        int qualityPlanTotalValue = 0;
                        int qualityActualTotalValue = 0;
                        var qualitytotalday = DateTime.DaysInMonth(time.Year, time.Month);
                        for (var i = 1; i <= totalday; i++)
                        {
                            var Ftime = new DateTime(time.Year, time.Month, i);
                            //计划
                            var plancount = PlanValue.Where(c => c.Process == efficiency.Process && c.Section == efficiency.Section && c.Department == dep && c.Group == groupitem && c.Time.Value == Ftime).Select(c => c.Num).FirstOrDefault();

                            //实际
                            var actualcount = ActualValue.Where(c => c.Process == efficiency.Process && c.Section == efficiency.Section && c.Department == dep && c.Group == groupitem && c.Time.Value == Ftime).Select(c => c.Num).FirstOrDefault();
                            qualityPlanTotalValue = qualityPlanTotalValue + plancount;
                            qualityActualTotalValue = qualityActualTotalValue + actualcount;

                        }
                        obj.Add("QualityActualScore", Math.Round((decimal)qualityActualTotalValue / qualityPlanTotalValue, 2));//实际得分
                        obj.Add("QualityGoal", (qualityActualTotalValue / qualityPlanTotalValue) - quality.IndicatorsValue > 0 ? 100 : 0);//单项得分
                        obj.Add("QualityGoalScore", (qualityActualTotalValue / qualityPlanTotalValue) - quality.IndicatorsValue > 0 ? 35 : 0);//得分小计
                        DayArray.Add(obj);
                    }
                }
                result.Add("DayArray", DayArray);


            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //效率效率指标显示
        public ActionResult DiasplyEfficiency(DateTime time, string stuta)
        {
            JObject result = new JObject();
            //拿到对的版本
            var version = db.KPI_Indicators.Where(c => c.ExecutionTime < time).Select(c => c.ExecutionTime).Distinct().ToList();
            if (version != null)
            {
                //取值
                var exetime = version.Max();
                var total = db.KPI_Indicators.Where(c => c.ExecutionTime == exetime && c.IndicatorsType == stuta).Select(c => new TempIndicators { Department = c.Department, Group = c.Group, Cycle = c.Cycle, IndicatorsValueUnit = c.IndicatorsValueUnit, IndicatorsName = c.IndicatorsName, IndicatorsValue = c.IndicatorsValue, SourceDepartment = c.SourceDepartment, Process = c.Process, Section = c.Section });

                var PlanValue = db.Plan_FromKPI.Where(c => c.IndicatorsType == stuta && c.PlanTime.Value.Year == time.Year).Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.PlanTime, Num = c.PlanNum });

                var ActualValue = db.KPI_ActualRecord.Where(c => c.IndicatorsType == stuta && c.ActualTime.Value.Year == time.Year).Select(c => new TempValue { Department = c.Department, Group = c.Group, Process = c.Process, Section = c.Section, Time = c.ActualTime, Num = c.ActualNormalNum });


                //拿到日数据
                JArray DayArray = new JArray();
                var deparment = total.Where(c => c.Cycle == "天" && c.IndicatorsValueUnit == "%").Select(c => c.Department).Distinct();
                foreach (var dep in deparment)
                {
                    var group = total.Where(c => c.Cycle == "天" && c.IndicatorsValueUnit == "%" && c.Department == dep).Select(c => c.Group).Distinct();
                    foreach (var groupitem in group)
                    {
                        var dayvalue = total.Where(c => c.Cycle == "天" && c.IndicatorsValueUnit == "%" && c.Department == dep && c.Group == groupitem).FirstOrDefault();
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
                        var totalday = DateTime.DaysInMonth(time.Year, time.Month);
                        for (var i = 1; i <= totalday; i++)
                        {
                            var Ftime = new DateTime(time.Year, time.Month, i);
                            //计划
                            var plancount = PlanValue.Where(c => c.Process == dayvalue.Process && c.Section == dayvalue.Section && c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime).Select(c => c.Num).FirstOrDefault();

                            //实际
                            var actualcount = ActualValue.Where(c => c.Process == dayvalue.Process && c.Section == dayvalue.Section && c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime).Select(c => c.Num).FirstOrDefault();
                            PlanTotalValue = PlanTotalValue + plancount;
                            ActualTotalValue = ActualTotalValue + actualcount;
                            planvalue.Add(plancount);
                            actualvalue.Add(actualcount);
                        }

                        obj.Add("PlanValue", planvalue);//每月计划数
                        obj.Add("PlanTotalValue", PlanTotalValue);//月总计划数
                        obj.Add("ActualValue", actualvalue);//每月实际数
                        obj.Add("ActualTotalValue", ActualTotalValue);//月总实际数
                        obj.Add("ActualScore", Math.Round((decimal)ActualTotalValue / PlanTotalValue, 2));//实际得分
                        obj.Add("DifferencesValue", (ActualTotalValue / PlanTotalValue) - dayvalue.IndicatorsValue);//与目标值差异
                        obj.Add("Goal", (ActualTotalValue / PlanTotalValue) - dayvalue.IndicatorsValue > 0 ? 100 : 0);//得分小计

                        DayArray.Add(obj);
                    }
                }
                result.Add("DayArray", DayArray);

                //拿到月数据
                JArray MouthArray = new JArray();
                var deparment2 = total.Where(c => c.Cycle == "月" && c.IndicatorsValueUnit == "%").Select(c => c.Department).Distinct();
                foreach (var dep in deparment2)
                {
                    var group = total.Where(c => c.Cycle == "月" && c.IndicatorsValueUnit == "%" && c.Department == dep).Select(c => c.Group).Distinct();
                    foreach (var groupitem in group)
                    {
                        var dayvalue = total.Where(c => c.Cycle == "月" && c.IndicatorsValueUnit == "%" && c.Department == dep && c.Group == groupitem).FirstOrDefault();
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
                            //计划
                            var plancount = PlanValue.Where(c => c.Process == dayvalue.Process && c.Section == dayvalue.Section && c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime).Select(c => c.Num).FirstOrDefault();

                            //实际
                            var actualcount = ActualValue.Where(c => c.Process == dayvalue.Process && c.Section == dayvalue.Section && c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime).Select(c => c.Num).FirstOrDefault();

                            PlanTotalValue = PlanTotalValue + plancount;
                            ActualTotalValue = ActualTotalValue + actualcount;
                            planvalue.Add(plancount);
                            actualvalue.Add(actualcount);
                        }
                        obj.Add("PlanValue", planvalue);//每年计划数
                        obj.Add("PlanTotalValue", PlanTotalValue);//年总计划数
                        obj.Add("ActualValue", actualvalue);//每年实际数
                        obj.Add("ActualTotalValue", ActualTotalValue);//年总实际数
                        obj.Add("ActualScore", Math.Round((decimal)ActualTotalValue / PlanTotalValue, 2));//实际得分
                        obj.Add("DifferencesValue", (ActualTotalValue / PlanTotalValue) - dayvalue.IndicatorsValue);//与目标值差异
                        obj.Add("Goal", (ActualTotalValue / PlanTotalValue) - dayvalue.IndicatorsValue > 0 ? 100 : 0);//得分小计

                        MouthArray.Add(obj);
                    }
                }
                result.Add("MouthArray", MouthArray);

                //拿到单/笔/次数据
                JArray OneOfTimeArray = new JArray();
                var deparment3 = total.Where(c => c.Cycle == "天" && c.IndicatorsValueUnit != "%").Select(c => c.Department).Distinct();
                foreach (var dep in deparment3)
                {
                    var group = total.Where(c => c.Cycle == "天" && c.IndicatorsValueUnit != "%" && c.Department == dep).Select(c => c.Group).Distinct();
                    foreach (var groupitem in group)
                    {
                        var dayvalue = total.Where(c => c.Cycle == "天" && c.IndicatorsValueUnit != "%" && c.Department == dep && c.Group == groupitem).FirstOrDefault();
                        JObject obj = new JObject();
                        obj.Add("Department", dayvalue.Department);
                        obj.Add("Group", dayvalue.Group);
                        obj.Add("IndicatorsName", dayvalue.IndicatorsName);
                        obj.Add("IndicatorsValue", dayvalue.IndicatorsValue + dayvalue.IndicatorsValueUnit);
                        obj.Add("SourceDepartment", dayvalue.SourceDepartment);
                        //JArray planvalue = new JArray(); //计划数据
                        JArray actualvalue = new JArray();//实际数据
                        //int PlanTotalValue = 0;
                        int ActualTotalValue = 0;
                        var totalday = DateTime.DaysInMonth(time.Year, time.Month);
                        for (var i = 1; i <= totalday; i++)
                        {
                            var Ftime = new DateTime(time.Year, time.Month, i);
                            //实际
                            var actualcount = ActualValue.Where(c => c.Process == dayvalue.Process && c.Section == dayvalue.Section && c.Department == dayvalue.Department && c.Group == dayvalue.Group && c.Time.Value == Ftime).Select(c => c.Num).FirstOrDefault();

                            ActualTotalValue = ActualTotalValue + actualcount;
                            actualvalue.Add(actualcount);
                        }
                        obj.Add("ActualValue", actualvalue);//每月实际数
                        obj.Add("ActualTotalValue", ActualTotalValue);//月总实际数
                        obj.Add("ActualScore", Math.Round(ActualTotalValue / dayvalue.IndicatorsValue, 2));//实际得分
                        obj.Add("DifferencesValue", (ActualTotalValue / dayvalue.IndicatorsValue) - dayvalue.IndicatorsValue);//与目标值差异
                        obj.Add("Goal", (ActualTotalValue / dayvalue.IndicatorsValue) - dayvalue.IndicatorsValue > 0 ? 100 : 0);//得分小计

                        DayArray.Add(obj);
                    }
                }
                result.Add("OneOfTimeArray", OneOfTimeArray);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //月流失率显示

        //7s显示

        #endregion
    }
}
