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
        //各工序直通率查询首页
        public ActionResult GetPassThrough()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "KPI", act = "GetPassThrough" });
            }
            return View();
        }
        [HttpPost]
        //各工序直通率查询
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
                var linenumlist = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum) && c.JobContent == jobcontent).Select(c=>c.LineNum).Distinct().ToList();
                foreach (var item in linenumlist)
                {
                    JObject smtitem = new JObject();
                    var smtinfo = db.SMT_ProductionData.Where(c => orderlist.Contains(c.OrderNum) && c.JobContent == jobcontent&&c.LineNum==item).ToList();
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
            var group = db.AfterWelding.Where(c => orderlist.Contains(c.Ordernum)).Select(c => c.Group).Distinct().ToList() ;
            JArray afterArray = new JArray();
            foreach (var after in group)
            {
                if (after == "")
                    continue;
                JObject item = new JObject();
                if (string.IsNullOrEmpty(after))
                {
                    var aftertotelcount = db.AfterWelding.Count(c => orderlist.Contains(c.Ordernum) && (c.Group == after|| c.Group == ""));
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
            var  assemblessgrou= db.Assemble.Where(c => orderlist.Contains(c.OrderNum) && c.PQCCheckFT != null && c.RepetitionPQCCheck == false).Select(c=>c.Group).Distinct().ToList();
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
            var fqcgroup= db.FinalQC.Where(c => orderlist.Contains(c.OrderNum) && c.FQCCheckFT != null && c.RepetitionFQCCheck == false).Select(c=>c.Group).Distinct().ToList();
            JArray fqcarray = new JArray();
            foreach (var fqcitem in fqcgroup)
            {
                if (fqcitem == "")
                    continue;
                JObject item = new JObject();
                if (string.IsNullOrEmpty(fqcitem))
                {
                    var FQCcount = db.FinalQC.Where(c => orderlist.Contains(c.OrderNum) && c.FQCCheckFT != null && c.RepetitionFQCCheck == false && (c.Group == fqcitem||c.Group=="")).ToList();
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
            var burngroup = db.Burn_in.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c=>c.Group).Distinct().ToList();
            JArray burnArray = new JArray();
            foreach (var burnitem in burngroup)
            {
                if (burnitem == "")
                    continue;
                JObject item = new JObject();
                if (string.IsNullOrEmpty(burnitem))
                {
                    var burncount = db.Burn_in.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null&&(c.Group==burnitem||c.Group=="")).ToList();
                    var burntotal = burncount.Select(c => c.BarCodesNum).Distinct().Count();
                    var burnPasscount = burncount.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var burnAbnormal = burncount.Where(c => c.OQCCheckFinish == false).Count();
                    item.Add("group", "没有分配班组的数据");
                    item.Add("passThrough", burntotal == 0 ? "-%" : Math.Round((double)burnPasscount * 100 / burntotal, 2) + "%");
                    item.Add("abnormal", burntotal == 0 ? "-%" : Math.Round((double)burnAbnormal * 100 / burntotal, 2) + "%");
                }
                else
                {
                    var burncount = db.Burn_in.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null&&c.Group == burnitem).ToList();
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
            var calibgroup = db.CalibrationRecord.Where(c => orderlist.Contains(c.OrderNum) && c.FinishCalibration != null && c.RepetitionCalibration == false).Select(c=>c.Group).Distinct().ToList();
            JArray calibArray = new JArray();
            foreach (var calibitem in calibgroup)
            {
                if (calibitem == "")
                    continue;
                JObject item = new JObject();
                if (string.IsNullOrEmpty(calibitem))
                {
                    var calibcount = db.CalibrationRecord.Where(c => orderlist.Contains(c.OrderNum) && c.FinishCalibration != null && c.RepetitionCalibration == false&&(c.Group==calibitem||c.Group=="")).ToList();
                    var calibtotal = calibcount.Select(c => c.BarCodesNum).Distinct().Count();
                    var calibPasscount = calibcount.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var calibAbnormal = calibcount.Where(c => c.Normal == false).Count();
                    item.Add("group", "没有分配班组的数据");
                    item.Add("passThrough", calibtotal == 0 ? "-%" : Math.Round((double)calibPasscount * 100 / calibtotal, 2) + "%");
                    item.Add("abnormal", calibtotal == 0 ? "-%" : Math.Round((double)calibAbnormal * 100 / calibtotal, 2) + "%");
                }
                else
                {
                    var calibcount = db.CalibrationRecord.Where(c => orderlist.Contains(c.OrderNum) && c.FinishCalibration != null && c.RepetitionCalibration == false&& c.Group == calibitem).ToList();
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
            var appearangroup = db.Appearance.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null).Select(c=>c.Group).Distinct().ToList();
            JArray appearanArray = new JArray();
            foreach (var appearanitem in appearangroup)
            {
                if (appearanitem == "")
                    continue;
                JObject item = new JObject();
                if (string.IsNullOrEmpty(appearanitem))
                {
                    var appearancount = db.Appearance.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null&&(c.Group==appearanitem||c.Group=="")).ToList();
                    var appearantotal = appearancount.Select(c => c.BarCodesNum).Distinct().Count();
                    var appearanPasscount = appearancount.GroupBy(c => c.BarCodesNum).Where(c => c.Count() < 2).ToList().Count();
                    var appearanAbnormal = appearancount.Where(c => c.OQCCheckFinish == false).Count();
                    item.Add("group", "没有分配班组的数据");
                    item.Add("passThrough", appearantotal == 0 ? "-%" : Math.Round((double)appearanPasscount * 100 / appearantotal, 2) + "%");
                    item.Add("abnormal", appearantotal == 0 ? "-%" : Math.Round((double)appearanAbnormal * 100 / appearantotal, 2) + "%");
                }
                else
                {
                    var appearancount = db.Appearance.Where(c => orderlist.Contains(c.OrderNum) && c.OQCCheckFT != null&& c.Group == appearanitem).ToList();
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
    }
}
