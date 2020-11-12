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
using System.Drawing;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JianHeMES.AuthAttributes;

namespace JianHeMES.Controllers
{
    public class ModuleManagementController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        #region 临时类
        private class TempIndex
        {
            public string Ordernum { get; set; }
            public string Machine { get; set; }
            public string Barcode { get; set; }
            public bool IsSampling { get; set; }
            public bool FinshResult { get; set; }
            public string Seaction { get; set; }
            public DateTime? EndTime { get; set; }

        }

        public class CheckList
        {
            public string Barcode { get; set; }
            public bool Finshi { get; set; }
        }
        #endregion

        #region------规则/内箱/外箱页面  (页面汇总)
        public async Task<ActionResult> Rule()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "ModuleManagement", act = "Rule" });
            }
            return View();
        }
        public async Task<ActionResult> Inside()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "ModuleManagement", act = "Inside" });
            }
            return View();
        }
        public async Task<ActionResult> Outside()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "ModuleManagement", act = "Outside" });
            }
            return View();
        }
        public async Task<ActionResult> Again_Print()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "ModuleManagement", act = "Again_Print" });
            }
            return View();
        }
        public async Task<ActionResult> Delete_Tag()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "ModuleManagement", act = "Delete_Tag" });
            }
            return View();
        }

        //模块看板首页
        public async Task<ActionResult> Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "ModuleManagement", act = "Index" });
            }
            return View();
        }
        public async Task<ActionResult> Board()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "ModuleManagement", act = "Board" });
            }
            return View();
        }
        public async Task<ActionResult> Board_History()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "ModuleManagement", act = "Board_History" });
            }
            return View();
        }
        //正常流程电检
        public ActionResult normalCheck()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "ModuleManagement", act = "normalCheck" });
            }
            return View();
        }
        //抽检
        public ActionResult spotCheck()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "ModuleManagement", act = "spotCheck" });
            }
            return View();
        }
        //老化
        public ActionResult Burnin()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "ModuleManagement", act = "Burnin" });
            }
            return View();
        }
        #endregion

        #region------模块看板


        [HttpPost]
        public ActionResult Index(List<string> orderunm)
        {
            JArray total = new JArray();
            var totalOrderdum = db.OrderMgm.Where(c => c.Models != 0).ToList();
            if (orderunm.Count != 0)
            {
                totalOrderdum = totalOrderdum.Where(c => orderunm.Contains(c.OrderNum)).ToList();
            }
            else
            {
                var orderlist = db.AfterWelding.Select(c => c.Ordernum).Distinct().ToList();

                totalOrderdum = db.OrderMgm.Where(c => orderlist.Contains(c.OrderNum)).ToList();

            }
            var ai = db.ModuleAI.Select(c => new TempIndex { Ordernum = c.Ordernum, Machine = c.Machine });
            var after = db.AfterWelding.Select(c => new TempIndex { Ordernum = c.Ordernum, FinshResult = c.IsAbnormal });
            var SamplingList = db.ModuleSampling.Select(c => new TempIndex { Ordernum = c.Ordernum, Seaction = c.Section, FinshResult = c.SamplingResult });
            var electric = db.ElectricInspection.Select(c => new TempIndex { Ordernum = c.Ordernum, Seaction = c.Section, FinshResult = c.ElectricInspectionResult, Barcode = c.ModuleBarcode });
            var burn = db.ModuleBurnIn.Select(c => new TempIndex { Ordernum = c.Ordernum, FinshResult = c.BurninResult, Barcode = c.ModuleBarcode, EndTime = c.BurnInEndTime });
            foreach (var item in totalOrderdum)
            {
                JObject result = new JObject();
                //订单号
                result.Add("ordernum", item.OrderNum);
                //平台型号
                result.Add("type", item.PlatformType);
                //制程要求
                result.Add("processingRequire", item.ProcessingRequire);
                //标准要求
                result.Add("standardRequire", item.StandardRequire);
                //模块数
                result.Add("moduleNum", item.Models);
                //AI机台完成数量
                JArray aistring = new JArray();
                var moduleai = ai.Where(c => c.Ordernum == item.OrderNum).Select(c => c.Machine).Distinct().ToList();
                moduleai.ForEach(c =>
                {
                    var count = ai.Count(a => a.Ordernum == item.OrderNum && a.Machine == c);
                    aistring.Add(c + ":" + Math.Round((decimal)count / item.Models, 2) + "%(" + count + "/" + item.Models + ")");
                });
                result.Add("aiCount", JsonConvert.SerializeObject(aistring));
                //后焊完成率
                var aftercount = after.Count(c => c.Ordernum == item.OrderNum);
                result.Add("afterWeldingPass", Math.Round((decimal)aftercount * 100 / item.Models, 2) + "%(" + aftercount + "/" + item.Models + ")");
                //后焊抽检率
                var Sampling = SamplingList.Count(c => c.Ordernum == item.OrderNum && c.Seaction == "后焊");
                result.Add("samplingCount", Math.Round((decimal)Sampling * 100 / item.Models, 2) + "%(" + Sampling + "/" + item.Models + ")");
                //后焊抽检合格率
                //var samplingab=after.Count(c => c.Ordernum == item.OrderNum && c.IsSampling == true);
                //result.Add("samplingPass", Sampling == 0 ? "0" :Math.Round((decimal)samplingab * 100 / Sampling, 2) + "%(" + samplingab + "/" + Sampling + ")");

                //后焊产线完成情况
                JArray array = new JArray();
                result.Add("afterLine", array);


                //灌胶电测完成率
                var gulecount = electric.Where(c => c.Ordernum == item.OrderNum && c.Seaction == "灌胶前电检" && c.FinshResult == true).Select(c => c.Barcode).ToList();
                result.Add("gulePass", Math.Round((decimal)gulecount.Count * 100 / item.Models, 2) + "%(" + gulecount.Count + "/" + item.Models + ")");
                //灌胶电测直通率
                var abnonal = electric.Where(c => c.Ordernum == item.OrderNum && c.Seaction == "灌胶前电检" && c.FinshResult == false).Select(c => c.Barcode).ToList();
                var trrought = gulecount.Except(abnonal).ToList().Count;
                result.Add("gulePassThrough", Math.Round((decimal)trrought * 100 / item.Models, 2) + "%(" + trrought + "/" + item.Models + ")");


                //面罩电测完成率
                var maskcount = electric.Where(c => c.Ordernum == item.OrderNum && c.Seaction == "模块电检" && c.FinshResult == true).Select(c => c.Barcode).ToList(); ;
                result.Add("maskPass", Math.Round((decimal)maskcount.Count * 100 / item.Models, 2) + "%(" + maskcount.Count + "/" + item.Models + ")");
                //面罩电测直通率
                var maskabnonal = electric.Where(c => c.Ordernum == item.OrderNum && c.Seaction == "模块电检" && c.FinshResult == false).Select(c => c.Barcode).ToList();
                var maskthrought = maskcount.Except(maskabnonal).ToList().Count;
                result.Add("maskPassThrough", Math.Round((decimal)maskthrought * 100 / item.Models, 2) + "%(" + maskthrought + "/" + item.Models + ")");



                //电测后抽检率
                var electricalSampling = SamplingList.Count(c => c.Ordernum == item.OrderNum && c.Seaction == "模块电检");
                result.Add("electricalSamplingCount", Math.Round((decimal)electricalSampling * 100 / item.Models, 2) + "%(" + electricalSampling + "/" + item.Models + ")");
                //电测后抽检合格率
                var electricalSamplingPass = SamplingList.Count(c => c.Ordernum == item.OrderNum && c.Seaction == "模块电检" && c.FinshResult == true);
                result.Add("electricalSamplingPass", electricalSampling == 0 ? "0%" : Math.Round((decimal)electricalSamplingPass * 100 / electricalSampling, 2) + "%(" + electricalSamplingPass + "/" + electricalSampling + ")");


                //老化完成率
                var burninfins = burn.Where(c => c.Ordernum == item.OrderNum && c.EndTime != null).Select(c => c.Barcode).Count();
                var burnin = burn.Where(c => c.Ordernum == item.OrderNum && c.EndTime == null).Select(c => c.Barcode).Count();
                result.Add("burnPass", Math.Round((decimal)burninfins * 100 / item.Models, 2) + "%(" + burninfins + "/" + item.Models + ") " + burnin + "个进行中");
                //老化直通率
                var burninabnonal = burn.Count(c => c.Ordernum == item.OrderNum && c.FinshResult == true && c.EndTime != null);
                result.Add("burnPassThrough", Math.Round((decimal)burninabnonal * 100 / item.Models, 2) + "%(" + burninabnonal + "/" + item.Models + ")");


                //外观完成率
                var appearancecount = electric.Where(c => c.Ordernum == item.OrderNum && c.Seaction == "外观电检" && c.FinshResult == true).Select(c => c.Barcode).ToList();
                result.Add("appearancePass", Math.Round((decimal)appearancecount.Count * 100 / item.Models, 2) + "%(" + appearancecount.Count + "/" + item.Models + ")");
                //外观直通率
                var appearanceabnonal = electric.Where(c => c.Ordernum == item.OrderNum && c.Seaction == "外观电检" && c.FinshResult == false).Select(c => c.Barcode).ToList();
                var appearancethrought = appearancecount.Except(appearanceabnonal).ToList().Count;
                result.Add("appearancePassThrough", Math.Round((decimal)appearancethrought * 100 / item.Models, 2) + "%(" + appearancethrought + "/" + item.Models + ")");


                //内箱包装数量
                var innside = db.ModuleInsideTheBox.Where(c => c.OrderNum == item.OrderNum).Select(c => new { c.ModuleBarcode, c.InnerBarcode }).ToList();
                var innsiderule = db.ModulePackageRule.Where(c => c.OrderNum == item.OrderNum && c.Statue == "纸箱").Select(c => new { c.Quantity, c.OuterBoxCapacity }).ToList();
                if (innsiderule.Count != 0)
                {
                    var totalpack = innsiderule.Sum(c => c.Quantity);
                    var onenum = innsiderule.Sum(c => c.OuterBoxCapacity * c.Quantity);
                    result.Add("innerPackCount", innside.Select(c => c.InnerBarcode).Distinct().Count() + "/" + totalpack + "箱(" + innside.Select(c => c.ModuleBarcode).Distinct().Count() + "/" + onenum + "件)");
                }
                else
                {
                    result.Add("innerPackCount", "0/0箱");
                }
                //外箱包装数量
                var outside = db.ModuleOutsideTheBox.Where(c => c.OrderNum == item.OrderNum).Select(c => new { c.OutsideBarcode, c.InnerBarcode }).ToList();
                var outsiderule = db.ModulePackageRule.Where(c => c.OrderNum == item.OrderNum && c.Statue == "外箱").Select(c => new { c.Quantity, c.OuterBoxCapacity }).ToList();
                if (outsiderule.Count != 0)
                {
                    var totalpack = outsiderule.Sum(c => c.Quantity);
                    var onenum = outsiderule.Sum(c => c.OuterBoxCapacity);
                    result.Add("outsidepackCount", outside.Select(c => c.OutsideBarcode).Distinct().Count() + "/" + totalpack + "箱(" + outside.Select(c => c.InnerBarcode).Distinct().Count() + "/" + totalpack * onenum + "件)");
                }
                else
                {
                    result.Add("outsidepackCount", "0/0箱");
                }
                //出入库情况
                var warehouse = db.Warehouse_Join.Count(c => c.OrderNum == item.OrderNum && c.State == "模块");
                if (warehouse == 0)
                {
                    result.Add("warehousr", "入库/出库(0/0)");
                }
                else
                {
                    var warehouseout = db.Warehouse_Join.Count(c => c.OrderNum == item.OrderNum && c.State == "模块" && c.IsOut == true);
                    result.Add("warehousr", "入库/出库(" + warehouse + "/" + warehouseout + ")");
                }
                total.Add(result);
            }
            return Content(JsonConvert.SerializeObject(total));
        }




        //模块实时看板历史
        public JArray ModuleHistoryBoard(List<string> ordernum)
        {

            JArray ProductionControlIndex = new JArray();   //创建JSON对象

            var time = DateTime.Now;
            int span = 20;
            int i = 1;
            JArray array = new JArray();

            foreach (var item in ordernum)
            {
                var board = db.ModuleBoard.Where(c => c.IsComplete == false && c.Ordernum == item).ToList();
                var order = db.OrderMgm.Where(c => c.OrderNum == item).FirstOrDefault();
                var OrderNum = new JObject();
                //完成时间
                var warehouseccount = db.Warehouse_Join.Where(c => c.OrderNum == item && c.State == "模块" && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();
                var outsiderule = db.ModulePackageRule.Count(c => c.OrderNum == item && c.Statue == "外箱") == 0 ? 0 : db.ModulePackageRule.Where(c => c.OrderNum == item && c.Statue == "外箱").Sum(c => c.Quantity);
                if (warehouseccount != 0 && warehouseccount >= outsiderule)//判断时候出库完成
                {
                    var maxtime = db.Warehouse_Join.Where(c => c.OrderNum == item && c.State == "模块" && c.IsOut == true).Max(c => c.WarehouseOutDate);

                    OrderNum.Add("CompleteTime", maxtime);
                    OrderNum.Add("Timespan", maxtime - order.PlanInputTime);
                }
                else
                {
                    OrderNum.Add("CompleteTime", null);
                    OrderNum.Add("Timespan", null);
                }

                OrderNum.Add("Id", order.ID);
                OrderNum.Add("OrderNum", order.OrderNum);
                OrderNum.Add("PlatformType", order.PlatformType);
                OrderNum.Add("Models", order.Models);
                OrderNum.Add("PlanInputTime", order.PlanInputTime);

                #region SMT
                #region---------------------SMT首样部分
                var SMTFirstSample_Description = order.SMTFirstSample_Description;

                OrderNum.Add("SMTFirstSample_Description", SMTFirstSample_Description);
                if (!string.IsNullOrEmpty(SMTFirstSample_Description))
                {
                    if (!comm.CheckJpgExit(item, "SMTSample_Files"))
                        OrderNum.Add("SMTFirstSample_DescriptionJpg", "true");
                    else
                        OrderNum.Add("SMTFirstSample_DescriptionJpg", "false");

                    if (!comm.CheckpdfExit(item, "SMTSample_Files"))
                        OrderNum.Add("SMTFirstSample_DescriptionPdf", "true");
                    else
                        OrderNum.Add("SMTFirstSample_DescriptionPdf", "false");
                }
                #endregion

                #region------------------------- SMT首样
                // 小样进度
                OrderNum.Add("HandSampleScedule", order.HandSampleScedule);
                //是否有图片
                if (comm.CheckJpgExit(item, "SmallSample_Files"))
                    OrderNum.Add("HandSampleSceduleJpg", "false");
                else
                    OrderNum.Add("HandSampleSceduleJpg", "true");
                //是否有PDF文件
                if (comm.CheckpdfExit(item, "SmallSample_Files"))
                    OrderNum.Add("HandSampleScedulePdf", "false");
                else
                    OrderNum.Add("HandSampleScedulePdf", "true");

                //是否有小样报告
                var sample = db.Small_Sample.OrderBy(c => c.Id).Where(c => (c.OrderNumber == item || c.OrderNumber.Contains(item)) && c.Approved != null && c.ApprovedResult == true).ToList();
                if (sample.Count != 0)
                {
                    JArray sampleJarray = new JArray();
                    int k = 1;
                    foreach (var sampleitem in sample)
                    {
                        JObject sampleJobject = new JObject();
                        sampleJobject.Add("id", sampleitem.Id);
                        sampleJobject.Add("Name", "NO." + k);
                        k++;
                        sampleJarray.Add(sampleJobject);
                    }
                    OrderNum.Add("HandSampleSceduleReport", sampleJarray);
                }
                else
                    OrderNum.Add("HandSampleSceduleReport", "false");
                #endregion

                #region------------------------- SMT异常
                var SMTAbnormal_Description = order.SMTAbnormal_Description;
                OrderNum.Add("SMTAbnormal_Description", SMTAbnormal_Description);
                if (!string.IsNullOrEmpty(SMTAbnormal_Description))
                {
                    if (!comm.CheckJpgExit(item, "SMTAbnormalOrder_Files"))
                        OrderNum.Add("SMTAbnormal_DescriptionJpg", "true");
                    else
                        OrderNum.Add("SMTAbnormal_DescriptionJpg", "false");

                    if (!comm.CheckpdfExit(item, "SMTAbnormalOrder_Files"))
                        OrderNum.Add("SMTAbnormal_DescriptionPdf", "true");
                    else
                        OrderNum.Add("SMTAbnormal_DescriptionPdf", "false");
                }
                #endregion

                var ModelNum = 0;
                var NormalNumSum = 0;
                var AbnormalNumSum = 0;
                var jobcontenlist = db.SMT_ProductionData.Where(c => c.OrderNum == item).Select(c => c.JobContent).Distinct().ToList();
                JArray SmtArray = new JArray();
                foreach (var jobconten in jobcontenlist)
                {
                    JObject FinishRateitem = new JObject();
                    if (jobconten == "灯面" || jobconten == "IC面")
                    {
                        ModelNum = order.Models;
                    }
                    else if (jobconten.Contains("转接卡") == true)
                    {
                        ModelNum = order.AdapterCard;
                    }
                    else if (jobconten.Contains("电源") == true)
                    {
                        ModelNum = order.Powers;
                    }
                    //对应工作内容良品总数
                    if (db.SMT_ProductionData.Count(c => c.OrderNum == item && c.JobContent == jobconten) == 0)
                    {
                        AbnormalNumSum = 0;
                        NormalNumSum = 0;
                    }
                    else
                    {
                        NormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item && c.JobContent == jobconten).Sum(c => c.NormalCount);

                        //对应工作内容不良品总数
                        AbnormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item && c.JobContent == jobconten).Sum(c => c.AbnormalCount);
                    }
                    // 面
                    FinishRateitem.Add("Line", jobconten);
                    // 总完成率
                    FinishRateitem.Add("CompleteRate", ModelNum == 0 ? "" : (((decimal)(NormalNumSum + AbnormalNumSum)) / ModelNum * 100).ToString("F2") + "%");
                    // 总完成率分母
                    FinishRateitem.Add("CompleteInfo", (NormalNumSum + AbnormalNumSum) + "/" + ModelNum);
                    // 总合格率
                    FinishRateitem.Add("PassRate", (AbnormalNumSum + NormalNumSum) == 0 ? "" : ((decimal)NormalNumSum / (NormalNumSum + AbnormalNumSum) * 100).ToString("F2") + "%");
                    //总合格率分子
                    FinishRateitem.Add("PassInfo", NormalNumSum + "/" + (NormalNumSum + AbnormalNumSum));
                    FinishRateitem.Add("SamplingRate", null);
                    FinishRateitem.Add("SamplingInfo", null);

                    SmtArray.Add(FinishRateitem);


                }
                OrderNum.Add("SmtArray", SmtArray);
                #endregion

                #region AI
                JArray itemArray = new JArray();
                var line = db.ModuleAI.Where(c => c.Ordernum == item).Select(c => c.AbnormalResultMessage).Distinct().ToList();
                foreach (var lineitem in line)
                {
                    var info = db.ModuleAI.Where(c => c.Ordernum == item && c.AbnormalResultMessage == lineitem).Select(c => c.IsAbnormal).ToList();
                    JObject obj1 = new JObject();
                    obj1.Add("Line", lineitem);
                    obj1.Add("CompleteRate", Math.Round((double)info.Count * 100 / order.Models, 2) + "%");
                    obj1.Add("CompleteInfo", info.Count + "/" + order.Models);
                    obj1.Add("PassRate", Math.Round((double)info.Count(c => c == true) * 100 / order.Models, 2) + "%");
                    obj1.Add("PassInfo", info.Count(c => c == true) + "/" + order.Models);
                    obj1.Add("SamplingRate", null);
                    obj1.Add("SamplingInfo", null);
                    itemArray.Add(obj1);
                }
                OrderNum.Add("AI", itemArray);

                #endregion

                #region  后焊
                itemArray = new JArray();
                var line2 = db.AfterWelding.Where(c => c.Ordernum == item).Select(c => c.Line).Distinct().ToList();
                foreach (var lineitem in line2)
                {
                    var info = db.AfterWelding.Where(c => c.Ordernum == item && c.Line == lineitem).Select(c => c.IsAbnormal).ToList();
                    var aftersamp = db.ModuleSampling.Count(c => c.Ordernum == item && c.Section == "后焊");
                    JObject obj1 = new JObject();
                    obj1.Add("Line", lineitem);
                    obj1.Add("CompleteRate", Math.Round((double)info.Count * 100 / order.Models, 2) + "%");
                    obj1.Add("CompleteInfo", info.Count + "/" + order.Models);
                    obj1.Add("PassRate", Math.Round((double)info.Count(c => c == true) * 100 / order.Models, 2) + "%");
                    obj1.Add("PassInfo", info.Count(c => c == true) + "/" + order.Models);
                    obj1.Add("SamplingRate", Math.Round((double)aftersamp * 100 / info.Count, 2) + "%");
                    obj1.Add("SamplingInfo", aftersamp + "/" + info.Count);
                    itemArray.Add(obj1);
                }
                OrderNum.Add("After", itemArray);

                #endregion

                #region  灌胶前电测
                itemArray = new JArray();

                var info1 = db.ElectricInspection.Where(c => c.Ordernum == item && c.Section == "灌胶前电检").Select(c => c.ElectricInspectionResult).ToList();
                if (info1.Count != 0)
                {
                    var beforesamp = db.ModuleSampling.Count(c => c.Ordernum == item && c.Section == "灌胶前电检");
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", Math.Round((double)info1.Count * 100 / order.Models, 2) + "%");
                    obj.Add("CompleteInfo", info1.Count + "/" + order.Models);
                    obj.Add("PassRate", Math.Round((double)info1.Count(c => c == true) * 100 / order.Models, 2) + "%");
                    obj.Add("PassInfo", info1.Count(c => c == true) + "/" + order.Models);
                    obj.Add("SamplingRate", Math.Round((double)beforesamp * 100 / info1.Count, 2) + "%");
                    obj.Add("SamplingInfo", beforesamp + "/" + info1.Count);
                    itemArray.Add(obj);
                }
                OrderNum.Add("Before", itemArray);

                #endregion

                #region  灌胶后电测
                itemArray = new JArray();
                var info2 = db.ElectricInspection.Where(c => c.Ordernum == item && c.Section == "模块电检").Select(c => c.ElectricInspectionResult).ToList();
                if (info2.Count != 0)
                {
                    var latersamp = db.ModuleSampling.Count(c => c.Ordernum == item && c.Section == "模块电检");
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", Math.Round((double)info2.Count * 100 / order.Models, 2) + "%");
                    obj.Add("CompleteInfo", info2.Count + "/" + order.Models);
                    obj.Add("PassRate", Math.Round((double)info2.Count(c => c == true) * 100 / order.Models, 2) + "%");
                    obj.Add("PassInfo", info2.Count(c => c == true) + "/" + order.Models);
                    obj.Add("SamplingRate", Math.Round((double)latersamp * 100 / info2.Count, 2) + "%");
                    obj.Add("SamplingInfo", latersamp + "/" + info2.Count);
                    itemArray.Add(obj);
                }
                OrderNum.Add("Later", itemArray);

                #endregion

                #region  老化
                itemArray = new JArray();

                var info3 = db.ModuleBurnIn.Where(c => c.Ordernum == item && c.BurnInEndTime != null).Select(c => c.BurninResult).ToList();
                if (info3.Count != 0)
                {
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", Math.Round((double)info3.Count * 100 / order.Models, 2) + "%");
                    obj.Add("CompleteInfo", info3.Count + "/" + order.Models);
                    obj.Add("PassRate", Math.Round((double)info3.Count(c => c == true) * 100 / order.Models, 2) + "%");
                    obj.Add("PassInfo", info3.Count(c => c == true) + "/" + order.Models);
                    obj.Add("SamplingRate", null);
                    obj.Add("SamplingInfo", null);
                    itemArray.Add(obj);
                }
                OrderNum.Add("BurnIn", itemArray);

                #endregion

                #region  外观电检
                itemArray = new JArray();

                var info4 = db.ElectricInspection.Where(c => c.Ordernum == item && c.Section == "外观电检").Select(c => c.ElectricInspectionResult).ToList();
                if (info4.Count != 0)
                {
                    var appsamp = db.ModuleSampling.Count(c => c.Ordernum == item && c.Section == "外观电检");
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", Math.Round((double)info4.Count * 100 / order.Models, 2) + "%");
                    obj.Add("CompleteInfo", info4.Count + "/" + order.Models);
                    obj.Add("PassRate", Math.Round((double)info4.Count(c => c == true) * 100 / order.Models, 2) + "%");
                    obj.Add("PassInfo", info4.Count(c => c == true) + "/" + order.Models);
                    obj.Add("SamplingRate", null);
                    obj.Add("SamplingInfo", null);
                    itemArray.Add(obj);
                }
                OrderNum.Add("Appearance", itemArray);

                #endregion

                #region  内箱
                itemArray = new JArray();

                var info5 = db.ModuleInsideTheBox.Where(c => c.OrderNum == item && c.Statue == "纸箱").Select(c => c.InnerBarcode).Distinct().ToList().Count();
                if (info5 != 0)
                {
                    var rule = db.ModulePackageRule.Where(c => c.OrderNum == item && c.Statue == "纸箱").Sum(c => c.Quantity);
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", rule == 0 ? "0%" : Math.Round((double)info5 * 100 / rule, 2) + "%");
                    obj.Add("CompleteInfo", info5 + "/" + rule);
                    obj.Add("PassRate", null);
                    obj.Add("PassInfo", null);
                    obj.Add("SamplingRate", null);
                    obj.Add("SamplingInfo", null);
                    itemArray.Add(obj);
                }
                OrderNum.Add("InsideTheBox", itemArray);

                #endregion

                #region  外箱
                itemArray = new JArray();
                var info6 = db.ModuleOutsideTheBox.Where(c => c.OrderNum == item).Select(c => c.OutsideBarcode).Distinct().ToList().Count();
                if (info6 != 0)
                {
                    var rule2 = db.ModulePackageRule.Where(c => c.OrderNum == item && c.Statue == "外箱").Sum(c => c.Quantity);
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", rule2 == 0 ? "0%" : Math.Round((double)info6 * 100 / rule2, 2) + "%");
                    obj.Add("CompleteInfo", info6 + "/" + rule2);
                    obj.Add("PassRate", null);
                    obj.Add("PassInfo", null);
                    obj.Add("SamplingRate", null);
                    obj.Add("SamplingInfo", null);
                    itemArray.Add(obj);
                }
                OrderNum.Add("OutsideTheBox", itemArray);

                #endregion

                #region  出入库
                itemArray = new JArray();
                var join = db.Warehouse_Join.Count(c => c.OrderNum == item && c.State == "模块");
                if (join != 0)
                {
                    var wareshouseout = db.Warehouse_Join.Count(c => c.OrderNum == item && c.State == "模块" && c.IsOut == true);
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", join == 0 ? "0%" : Math.Round((double)wareshouseout * 100 / join, 2) + "%");
                    obj.Add("CompleteInfo", join + "/" + wareshouseout);
                    obj.Add("PassRate", null);
                    obj.Add("PassInfo", null);
                    obj.Add("SamplingRate", null);
                    obj.Add("SamplingInfo", null);
                    itemArray.Add(obj);
                }
                OrderNum.Add("Warehouse", itemArray);

                #endregion

                ProductionControlIndex.Add(OrderNum);
                i++;
            }

            string output2 = Newtonsoft.Json.JsonConvert.SerializeObject(ProductionControlIndex, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\ProductionController.json", output2);

            return ProductionControlIndex;


        }



        #endregion

        #region------各工序功能

        #region---------------------------- 工段通用方法--------------------------
        #region    --------------------查询订单已完成、未完成、未开始条码
        [HttpPost]
        public ActionResult Checklist(string orderNum, string statue, bool IsSamping)
        {
            //    if (Session["User"] == null)
            //    {
            //        return RedirectToAction("Login", "Users");
            //    }
            JObject stationResult = new JObject();//输出结果JObject
            var value = new List<string>();
            switch (statue)
            {
                case "AI":
                    value = db.ModuleAI.Where(c => c.Ordernum == orderNum).OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();
                    break;

                case "后焊":
                    value = db.AfterWelding.Where(c => c.Ordernum == orderNum).OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();
                    break;

                case "灌胶前电检":
                    value = db.ElectricInspection.Where(c => c.Ordernum == orderNum && c.Section == "灌胶前电检").OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();

                    break;
                case "模块电检":
                    value = db.ElectricInspection.Where(c => c.Ordernum == orderNum && c.Section == "模块电检").OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();

                    break;
                case "老化":
                    value = db.ModuleBurnIn.Where(c => c.Ordernum == orderNum).OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();

                    break;
                case "外观电检":
                    value = db.ElectricInspection.Where(c => c.Ordernum == orderNum && c.Section == "外观电检").OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();

                    break;

            }

            if (IsSamping)
            {
                var smaping = db.ModuleSampling.Where(c => c.Ordernum == orderNum && c.Section == statue).OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();
                stationResult = ChecklistSmaplingItem(value, smaping, orderNum);
            }
            else
            {
                stationResult = ChecklistItem(value, orderNum);
            }
            return Content(JsonConvert.SerializeObject(stationResult));
        }

        public JObject ChecklistItem(List<string> value, string orderNum)
        {

            List<string> NotDoList = new List<string>();//未开始做条码清单
            JObject stationResult = new JObject();//输出结果JObject
                                                  //调出订单所有条码清单
            List<string> barcodelist = db.BarCodes.Where(c => c.OrderNum == orderNum && c.BarCodeType == "模块").OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();
            if (value.Count == 0)
            {
                JArray array = new JArray();
                barcodelist.ForEach(c => array.Add(c));
                stationResult.Add("NotDoList", array);
                stationResult.Add("NeverFinish", null);
                stationResult.Add("FinishList", null);
            }
            else
            {

                //未开始做条码清单
                NotDoList = barcodelist.Except(value).ToList();
                JArray NotDoListarray = new JArray();
                JArray FinishListarray = new JArray();

                NotDoList.ForEach(c => NotDoListarray.Add(c));
                //已完成条码清单
                value.ForEach(c => FinishListarray.Add(c));

                stationResult.Add("NotDoList", NotDoListarray);
                stationResult.Add("NeverFinish", null);
                stationResult.Add("FinishList", FinishListarray);
            }
            return stationResult;
        }

        public JObject ChecklistSmaplingItem(List<string> finash, List<string> smapList, string orderNum)
        {

            List<string> NotDoList = new List<string>();//未开始做条码清单
            JObject stationResult = new JObject();//输出结果JObject
                                                  //调出订单所有条码清单
            var box = db.OrderMgm.Where(c => c.OrderNum == orderNum).Select(c => c.Models).FirstOrDefault();
            if (finash.Count == 0)
            {
                JArray array = new JArray();
                //barcodelist.ForEach(c => array.Add(c));
                stationResult.Add("NotDoList", null);
                stationResult.Add("SmaplingRate", "0%");
                stationResult.Add("FinishList", null);
            }
            else
            {

                //未开始做条码清单
                NotDoList = finash.Except(smapList).ToList();
                JArray NotDoListarray = new JArray();
                JArray FinishListarray = new JArray();

                NotDoList.ForEach(c => NotDoListarray.Add(c));
                //已完成条码清单
                smapList.ForEach(c => FinishListarray.Add(c));

                stationResult.Add("NotDoList", NotDoListarray);
                stationResult.Add("SmaplingRate", box == 0 ? "0%" : Math.Round((decimal)smapList.Count * 100 / finash.Count, 2) + "%");
                stationResult.Add("FinishList", FinishListarray);
            }
            return stationResult;
        }
        #endregion

        #region ----------条码验证检查
        public ActionResult AfterWeldingCheckBarcode(string orderNum, string barcode)
        {
            JObject result = new JObject();
            JArray array = new JArray();
            var info = db.BarCodes.Where(c => c.BarCodesNum == barcode).FirstOrDefault();
            if (info == null)
            {
                result.Add("result", false);
                result.Add("message", "没有找到条码信息");
                result.Add("period", null);
            }
            else
            {
                if (info.OrderNum != orderNum)
                {
                    result.Add("result", false);
                    result.Add("message", "此条码与订单不符,该条码的订单是" + info.OrderNum);
                    result.Add("period", null);
                }
                else
                {
                    result.Add("result", true);
                    result.Add("message", null);
                    JObject item = new JObject();
                    //ai
                    var ai = db.ModuleAI.Where(c => c.ModuleBarcode == barcode).ToList();
                    item.Add("Name", "AI");
                    item.Add("Have", ai.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();
                    //后焊
                    var afterWelding = db.AfterWelding.Where(c => c.ModuleBarcode == barcode).ToList();
                    item.Add("Name", "后焊");
                    item.Add("Have", afterWelding.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    //灌胶前电检
                    var electricInspection = db.ElectricInspection.Where(c => c.ModuleBarcode == barcode && c.Section == "灌胶前电检").ToList();
                    item.Add("Name", "灌胶前电检");
                    item.Add("Have", electricInspection.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    //模块电检
                    var electricInspection2 = db.ElectricInspection.Where(c => c.ModuleBarcode == barcode && c.Section == "模块电检").ToList();
                    item.Add("Name", "模块电检");
                    item.Add("Have", electricInspection2.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    //老化
                    var moduleBurnIn = db.ModuleBurnIn.Where(c => c.ModuleBarcode == barcode).ToList();
                    item.Add("Name", "老化");
                    item.Add("Have", moduleBurnIn.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    //外观
                    var moduleAppearance = db.ElectricInspection.Where(c => c.ModuleBarcode == barcode && c.Section == "外观电检").ToList();
                    item.Add("Name", "外观电检");
                    item.Add("Have", moduleAppearance.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    result.Add("period", array);
                }
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region ------抽检前条码检查
        public ActionResult CheckSampling(string ordernum, string barcode, string statue)
        {
            int count = 0;
            // bool isSampling = false;
            JObject result = new JObject();
            switch (statue)
            {
                case "AI":
                    count = db.ModuleAI.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode);
                    //isSampling = db.AfterWelding.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault() ;
                    break;
                case "后焊":
                    count = db.AfterWelding.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode);
                    //isSampling = db.AfterWelding.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault() ;
                    break;
                case "灌胶前电检":
                    count = db.ElectricInspection.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode && c.Section == "灌胶前电检");
                    //isSampling = db.ElectricInspection.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault();
                    break;
                case "模块电检":
                    count = db.ElectricInspection.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode && c.Section == "模块电检");
                    //isSampling = db.ElectricInspection.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault();
                    break;
                case "老化":
                    count = db.ModuleBurnIn.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode);
                    //isSampling = db.ElectricInspection.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault();
                    break;
                case "外观电检":
                    count = db.ElectricInspection.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode && c.Section == "外观电检");
                    //isSampling = db.ElectricInspection.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault();
                    break;
            }
            if (count == 0)
            {
                result.Add("result", false);
                result.Add("mes", "没有找到该条码信息");
            }
            else
            {
                //if (isSampling == true)
                //{
                //    result.Add("result", false);
                //    result.Add("mes", "此条码已抽检");
                //}
                //else
                //{
                result.Add("result", true);
                result.Add("mes", "成功");
                // }
            }
            return Content(JsonConvert.SerializeObject(result));
        }


        #endregion

        #endregion

        #region--------------------AI 暂时line不确定能不能拿到--------------------
        public ActionResult CreateAI(ModuleAI moduleAI)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "AfterWeldingCreate" });
            }
            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                if (db.ModuleAI.Count(c => c.ModuleBarcode == moduleAI.ModuleBarcode) > 0)
                {
                    result.Add("result", false);
                    result.Add("mes", "该条码已有AI记录");
                    return Content(JsonConvert.SerializeObject(result));
                }
                var line = moduleAI.ModuleBarcode.Substring(moduleAI.ModuleBarcode.Length - 1, 1);
                moduleAI.Machine = line;
                moduleAI.AITime = DateTime.Now;
                moduleAI.AIor = ((Users)Session["User"]).UserName;
                db.ModuleAI.Add(moduleAI);
                db.SaveChanges();
                UpdateNewTable(moduleAI.Ordernum, "AI", DateTime.Now);
                result.Add("result", true);
                result.Add("mes", "成功");
                return Content(JsonConvert.SerializeObject(result));

            }
            result.Add("result", false);
            result.Add("mes", "错误,数据格式不对");
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region -------------------------------后焊--------------------------------
        #region----------后焊创建新数据
        [HttpPost]
        public ActionResult AfterWeldingCreate(AfterWelding afterWelding)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "AfterWeldingCreate" });
            }
            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                if (db.AfterWelding.Count(c => c.ModuleBarcode == afterWelding.ModuleBarcode) > 0)
                {
                    result.Add("result", false);
                    result.Add("mes", "该条码已有后焊记录");
                    return Content(JsonConvert.SerializeObject(result));
                }
                if (db.BarCodes.Count(c => c.BarCodesNum == afterWelding.ModuleBarcode) != 0)
                {
                    afterWelding.AfterWeldingTime = DateTime.Now;
                    afterWelding.AfterWeldingor = ((Users)Session["User"]).UserName;
                    afterWelding.IsAbnormal = true;
                    db.AfterWelding.Add(afterWelding);
                    db.SaveChangesAsync();
                    UpdateNewTable(afterWelding.Ordernum, "后焊", DateTime.Now);
                    result.Add("result", true);
                    result.Add("mes", "成功");
                    return Content(JsonConvert.SerializeObject(result));
                }
                else
                {
                    result.Add("result", false);
                    result.Add("mes", "条码错误");
                    return Content(JsonConvert.SerializeObject(result));
                }

            }
            result.Add("result", false);
            result.Add("mes", "错误,数据格式不对");
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region----------后焊抽样送检工序

        [HttpPost]

        public ActionResult AfterWeldingSampling(string barcode, string remak, string Abnormal, string Department, string Group, bool isAbnormal)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "AfterWeldingSampling" });
            }
            JObject result = new JObject();
            var afterWelding = db.AfterWelding.Where(c => c.ModuleBarcode == barcode).FirstOrDefault();
            if (afterWelding != null)
            {
                afterWelding.SamplingTime = DateTime.Now;
                afterWelding.Samplingor = ((Users)Session["User"]).UserName;
                if (!string.IsNullOrEmpty(remak))
                {
                    afterWelding.Remark = afterWelding.Remark + "抽检备注:" + remak;
                }
                afterWelding.IsSampling = true;
                afterWelding.SamplingResult = isAbnormal;
                afterWelding.SamplingResultMessage = Abnormal;
                afterWelding.Department = Department;
                afterWelding.Group = Group;
                db.Entry(afterWelding).State = EntityState.Modified;
                db.SaveChangesAsync();
                result.Add("result", true);
                result.Add("mes", "成功");
                return Content(JsonConvert.SerializeObject(result));
            }
            result.Add("result", false);
            result.Add("mes", "错误,数据格式不对");
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion


        #endregion

        #region----------灌胶前电检工序 灌胶后电检工序 外观电检 数据创建----------

        [HttpPost]
        public async Task<ActionResult> ElectricInspectionBeforeGlueFillingCreate(ElectricInspection electricInspectionBeforeGlueFilling)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "ElectricInspectionBeforeGlueFillingCreate" });
            }
            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                if (db.ElectricInspection.Count(c => c.ModuleBarcode == electricInspectionBeforeGlueFilling.ModuleBarcode && c.Section == electricInspectionBeforeGlueFilling.Section) > 0)
                {
                    result.Add("result", false);
                    result.Add("mes", "该条码已有电检记录");
                    return Content(JsonConvert.SerializeObject(result));
                }
                electricInspectionBeforeGlueFilling.ElectricInspectionTime = DateTime.Now;
                electricInspectionBeforeGlueFilling.ElectricInspectionor = ((Users)Session["User"]).UserName;
                if (electricInspectionBeforeGlueFilling.ElectricInspectionMessage == "正常")
                {
                    electricInspectionBeforeGlueFilling.ElectricInspectionResult = true;
                }
                db.ElectricInspection.Add(electricInspectionBeforeGlueFilling);
                await db.SaveChangesAsync();
                UpdateNewTable(electricInspectionBeforeGlueFilling.Ordernum, electricInspectionBeforeGlueFilling.Section, DateTime.Now);
                result.Add("result", true);
                result.Add("mes", "成功");
                return Content(JsonConvert.SerializeObject(result));
            }
            result.Add("result", false);
            result.Add("mes", "错误,数据格式不对");
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region -------------------------------老化--------------------------------

        #region 老化新增

        [HttpPost]
        public async Task<ActionResult> BurninCreate(ModuleBurnIn moduleBurn)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "BurninCreate" });
            }
            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                if (db.ModuleBurnIn.Count(c => c.ModuleBarcode == moduleBurn.ModuleBarcode) > 0)
                {
                    result.Add("result", false);
                    result.Add("mes", "该条码已有老化记录");
                    return Content(JsonConvert.SerializeObject(result));
                }
                moduleBurn.BurnInStartor = ((Users)Session["User"]).UserName;
                moduleBurn.BurnInStartTime = DateTime.Now;
                moduleBurn.BurninResult = true;
                moduleBurn.OldOrdernum = moduleBurn.Ordernum;
                moduleBurn.OldModuleBarcode = moduleBurn.ModuleBarcode;
                db.ModuleBurnIn.Add(moduleBurn);
                await db.SaveChangesAsync();
                UpdateNewTable(moduleBurn.Ordernum, "老化", DateTime.Now);
                result.Add("result", true);
                result.Add("mes", "成功");
                return Content(JsonConvert.SerializeObject(result));
            }
            result.Add("result", false);
            result.Add("mes", "错误,数据格式不对");
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 老化异常输入


        [HttpPost]
        public async Task<ActionResult> BurninAbnormal(string ordernum, string modulebarcode, string BurninResult)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "BurninAbnormal" });
            }
            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                var info = db.ModuleBurnIn.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == modulebarcode && c.OldModuleBarcode == modulebarcode && c.BurnInEndor == null).FirstOrDefault();
                if (info == null)
                {
                    result.Add("result", false);
                    result.Add("mes", "没有找到老化中的条码,请检查条码准确");
                    return Content(JsonConvert.SerializeObject(result));
                }
                info.BurninResult = false;
                info.BurninMessage = BurninResult;
                await db.SaveChangesAsync();
                UpdateNewTable(ordernum, "老化", DateTime.Now);
                result.Add("result", true);
                result.Add("mes", "成功");
                return Content(JsonConvert.SerializeObject(result));
            }
            result.Add("result", false);
            result.Add("mes", "错误,数据格式不对");
            return Content(JsonConvert.SerializeObject(result));
        }

        #endregion

        #region 老化完成
        //老化完成前条码列表检查
        public ActionResult BurninCompleteCheck(string ordernum, List<string> modulbarcode)
        {
            JArray array = new JArray();
            foreach (var item in modulbarcode)
            {
                JObject result = new JObject();
                result.Add("barcode", item);
                var havintcout = db.ModuleBurnIn.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == item);
                var finshin = db.ModuleBurnIn.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == item && c.BurnInEndor != null);
                if (havintcout == 0)
                {
                    result.Add("pass", false);
                    result.Add("mes", "没有找到开始老化记录");
                }
                else if (finshin != 0)
                {
                    result.Add("pass", false);
                    result.Add("mes", "条码已经完成老化");
                }
                else
                {
                    result.Add("pass", true);
                    result.Add("mes", "正常");
                }
                array.Add(result);

            }
            return Content(JsonConvert.SerializeObject(array));
        }

        //老化完成数据保存
        [HttpPost]
        public async Task<ActionResult> BurninComplete(string ordernum, List<string> modulbarcode)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "BurninComplete" });
            }
            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                var info = db.ModuleBurnIn.Where(c => c.Ordernum == ordernum && modulbarcode.Contains(c.ModuleBarcode)).ToList();
                info.ForEach(c => { c.BurnInEndor = ((Users)Session["User"]).UserName; c.BurnInEndTime = DateTime.Now; });

                await db.SaveChangesAsync();
                UpdateNewTable(ordernum, "老化", DateTime.Now);
                result.Add("result", true);
                result.Add("mes", "成功");
                return Content(JsonConvert.SerializeObject(result));
            }
            result.Add("result", false);
            result.Add("mes", "错误,数据格式不对");
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion
        #endregion

        #region -------------------------------抽检--------------------------------
        public ActionResult Sampling(ModuleSampling moduleSampling)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "" });
            }
            JObject result = new JObject();
            var have = 0;
            if (moduleSampling.Section == "后焊")
            {
                have = db.AfterWelding.Count(c => c.ModuleBarcode == moduleSampling.ModuleBarcode);
            }
            if (moduleSampling.Section == "灌胶前电检")
            {
                have = db.ElectricInspection.Count(c => c.ModuleBarcode == moduleSampling.ModuleBarcode && c.Section == "灌胶前电检");
            }
            if (moduleSampling.Section == "模块电检")
            {
                have = db.ElectricInspection.Count(c => c.ModuleBarcode == moduleSampling.ModuleBarcode && c.Section == "模块电检");
            }
            if (moduleSampling.Section == "老化")
            {
                have = db.AfterWelding.Count(c => c.ModuleBarcode == moduleSampling.ModuleBarcode);
            }
            if (moduleSampling.Section == "外观电检")
            {
                have = db.ElectricInspection.Count(c => c.ModuleBarcode == moduleSampling.ModuleBarcode && c.Section == "外观电检");
            }
            if (have != 0)
            {
                var same = db.ModuleSampling.Count(c => c.ModuleBarcode == moduleSampling.ModuleBarcode && c.Section == moduleSampling.Section);
                if (same != 0)
                {
                    result.Add("result", false);
                    result.Add("mes", "错误,此条码已抽检");
                    return Content(JsonConvert.SerializeObject(result));
                }
                moduleSampling.SamplingTime = DateTime.Now;
                moduleSampling.Samplingor = ((Users)Session["User"]).UserName;
                if (moduleSampling.SamplingMessage == "正常")
                {
                    moduleSampling.SamplingResult = true;
                }
                db.ModuleSampling.Add(moduleSampling);
                db.SaveChanges();
                UpdateNewTable(moduleSampling.Ordernum, moduleSampling.Section, DateTime.Now);
                result.Add("result", true);
                result.Add("mes", "成功");
                return Content(JsonConvert.SerializeObject(result));
            }
            result.Add("result", false);
            result.Add("mes", "错误,此条码未进行" + moduleSampling.Section);
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        public void UpdateNewTable(string ordernum, string seaction, DateTime? time)
        {
            var board = db.ModuleBoard.Where(c => c.Ordernum == ordernum && c.Section == seaction).FirstOrDefault();
            if (board == null)
            {
                ModuleBoard moduleBoard = new ModuleBoard() { Ordernum = ordernum, Section = seaction, UpdateDate = time };
                db.ModuleBoard.Add(moduleBoard);
                db.SaveChanges();
            }
            else
            {
                board.UpdateDate = time;
                db.SaveChanges();
            }
        }

        #region 暂时不要的
        #region ------外观电检 不要
        public ActionResult AppearanceCreate()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "AppearanceCreate" });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AppearanceCreate(ModuleAppearance moduleAppearance)
        {
            if (ModelState.IsValid)
            {
                db.ModuleAppearance.Add(moduleAppearance);
                await db.SaveChangesAsync();
                return RedirectToAction("AfterWeldingCreate");
            }
            return View(moduleAppearance);
        }
        #endregion

        #region------灌胶后电检工序 不要
        //灌胶后电检工序
        // GET: ModuleManagement/ElectricInspectionAfterGlueFillingCreate
        public ActionResult ElectricInspectionAfterGlueFillingCreate()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "ElectricInspectionAfterGlueFillingCreate" });
            }
            return View();
        }

        // POST: ModuleManagement/ElectricInspectionAfterGlueFillingCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ElectricInspectionAfterGlueFillingCreate(ElectricInspection electricInspectionAfterGlueFilling)
        {
            if (ModelState.IsValid)
            {
                db.ElectricInspection.Add(electricInspectionAfterGlueFilling);
                await db.SaveChangesAsync();
                return RedirectToAction("ElectricInspectionAfterGlueFillingCreate");
            }
            return View(electricInspectionAfterGlueFilling);
        }
        #endregion

        #region ----------后焊首页
        public ActionResult AfterWeldingIndex()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "AfterWeldingIndex" });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AfterWeldingIndex(string ordernum, string barcode)
        {
            var afterWelding = db.AfterWelding.ToList();
            if (!string.IsNullOrEmpty(ordernum))
            {
                afterWelding = afterWelding.Where(c => c.Ordernum == ordernum).ToList();
            }
            if (!string.IsNullOrEmpty(barcode))
            {
                afterWelding = afterWelding.Where(c => c.ModuleBarcode == barcode).ToList();
            }


            return View(afterWelding);
        }
        #endregion


        //#region ----------后焊进行中和异常录入
        ////public async Task<ActionResult> AfterWeldingAbnormal(AfterWelding afterWelding)
        ////{
        ////    if (ModelState.IsValid)
        ////    {
        ////        db.Entry(afterWelding).State = EntityState.Modified;
        ////        await db.SaveChangesAsync();
        ////        return RedirectToAction("AfterWeldingCreate");
        ////    }
        ////    return View(afterWelding);
        ////}

        //#endregion

        #endregion
        #endregion

        #region ------内箱or外箱装箱规则
        #region 内箱OR外箱装箱规则
        public ActionResult GetInnerInfo(List<ModulePackageRule> modulePackageRule, string ITEMNO = null, string COLOURS = null, string Remark = null)
        {
            //先删除原有的
            var ordernum = modulePackageRule.Select(c => c.OrderNum).FirstOrDefault();//提出订单号
            var statue = modulePackageRule.FirstOrDefault().Statue;
            var list = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).ToList();//根据订单找到信息
            db.ModulePackageRule.RemoveRange(list);//移除
            db.SaveChanges();
            //生成新的
            if (ModelState.IsValid)//判断数据格式是否正确
            {
                modulePackageRule.ForEach(c => { c.CreateDate = DateTime.Now; c.Creator = ((Users)Session["User"]).UserName; });
                db.ModulePackageRule.AddRange(modulePackageRule);//添加
                db.SaveChanges();
                var info = db.ModulePackageRule.Where(c => c.OrderNum == ordernum).ToList();
                info.ForEach(c => { c.ITEMNO = ITEMNO; c.COLOURS = COLOURS; c.Remark = Remark; });
                db.SaveChanges();
                return Content("ok");
            }
            return View();
        }

        #endregion

        /// <summary>
        /// 作用:根据给的订单号，显示包装信息
        /// </summary>
        /// 逻辑:根据订单号查找包装录入规则,把其中的包装的类型,容量,数量是否分屏,能否修改信息,将信息放回前端(能放修改逻辑:没有打印的可以修改,已经打印的,返回前端一个已经打印的数量,前端根据这个数量,设置只能增加,不能减少)
        /// 结果:将信息放回前端
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
        public ActionResult GetValueFromOrderNum(string ordernum, string statue)
        {

            JArray value = new JArray();
            JObject result = new JObject();

            var packingList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).ToList();//根据订单显示包装规则信息

            if (packingList.Count == 0)//如果规则信息为空
            {
                return Content("");
            }
            int i = 0;
            foreach (var item in packingList)//循环规则信息
            {
                JObject valueitem = new JObject();
                valueitem.Add("packingType", item.Type);//包装类型
                valueitem.Add("itemNum", item.OuterBoxCapacity);//包装容量
                valueitem.Add("Num", item.Quantity);//包装数量
                valueitem.Add("batch", item.Batch);//批次
                valueitem.Add("isBatch", item.IsBatch);//是否批次
                valueitem.Add("screenNum", item.ScreenNum);//分屏
                valueitem.Add("isSeparate", item.IsSeparate);
                var count = 0;
                if (statue == "纸箱" || statue == "纸盒")
                {
                    count = db.ModuleInsideTheBox.Where(c => c.IsEmbezzle == false && c.IsMixture == false && c.Type == item.Type && c.OrderNum == item.OrderNum && c.Statue == statue).Select(c => c.InnerBarcode).Distinct().Count(); ;//根据订单号和类型找 没有混装,没有挪用的 纸箱包装打印记录
                }
                else
                {
                    count = db.ModuleOutsideTheBox.Where(c => c.IsEmbezzle == false && c.IsMixture == false && c.Type == item.Type && c.OrderNum == item.OrderNum).Select(c => c.OutsideBarcode).Distinct().Count(); ;//根据订单号和类型找 没有混装,没有挪用的 外箱包装打印记录
                }
                if (count == 0)
                {
                    valueitem.Add("update", "true"); //没有包装打印记录，可以修改
                    valueitem.Add("min", 0);
                }
                else
                {
                    valueitem.Add("update", "false");//有包装打印记录，不可以修改
                    valueitem.Add("min", count);
                }
                value.Add(valueitem);
                i++;
            }
            result.Add("Data", value);
            return Content(JsonConvert.SerializeObject(result));
        }

        //拿到规格型号,颜色,备注
        public ActionResult InformationFromOrderNum(string ordernum)
        {
            
            JObject result = new JObject();

            var packingList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum).ToList();//根据订单显示包装规则信息

            if (packingList.Count == 0)//如果规则信息为空
            {
                return Content("");
            }
            result.Add("ITEMNO", packingList.FirstOrDefault().ITEMNO);//规格型号
            result.Add("COLOURS", packingList.FirstOrDefault().COLOURS);//颜色
            result.Add("Remark", packingList.FirstOrDefault().Remark);//特长的一段字符串
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region------内箱装箱记录
        //获取模块未装箱条码类别
        public ActionResult NotPackingBarcode(string ordernum)
        {
            var barcode = db.BarCodes.Where(c => c.OrderNum == ordernum && c.BarCodeType == "模块").Select(c => c.BarCodesNum).ToList();
            var printbarcode = db.ModuleInsideTheBox.Where(c => c.OrderNum == ordernum && c.Statue == "纸箱").Select(c => c.ModuleBarcode).ToList();
            var notprintbarcode = barcode.Except(printbarcode).ToList();
            var sort = notprintbarcode.OrderBy(c => c).ToList();
            JArray result = new JArray();
            foreach (var item in sort)
            {
                result.Add(item);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        public void Remaining(string ordernum, string ip = "", int port = 0, int concentration = 5)
        {
            var havbarcode = db.AfterWelding.Where(c => c.Ordernum == ordernum).Select(c => c.ModuleBarcode).Distinct().ToList();
            var completebarcode = db.ModuleInsideTheBox.Where(c => c.OrderNum == ordernum && c.Statue == "纸箱").Select(c => c.ModuleBarcode).Distinct().ToList();
            var notcomlpete = havbarcode.Except(completebarcode).ToList();

            var type = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "纸箱").Select(c => c.Type).FirstOrDefault();
            var typenum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "纸箱").Select(c => c.OuterBoxCapacity).FirstOrDefault();
            var SN = db.ModuleInsideTheBox.Where(c => c.OrderNum == ordernum && c.Statue == "纸箱").Max(c => c.SN) + 1;
            var ordernumArray = ordernum.Split('-');
            var num = ordernumArray[2].PadLeft(2, '0');

            int i = 1;
            var count = notcomlpete.Count - 45;
            List<string> onebarcode = new List<string>();
            List<ModuleInsideTheBox> insideTheBoxes = new List<ModuleInsideTheBox>();
            foreach (var item in notcomlpete)
            {

                var InnerBarcode = ordernumArray[0] + ordernumArray[1] + num + "-ZX" + SN.ToString().PadLeft(4, '0');
                ModuleInsideTheBox box = new ModuleInsideTheBox() { OrderNum = ordernum, ModuleBarcode = item, SN = SN, InnerBarcode = InnerBarcode, InnerTime = DateTime.Now, Inneror = "唐硕贵", Type = type, Department = "总装二部", Group = "屏体包装一组", Statue = "纸箱" };

                insideTheBoxes.Add(box);
                onebarcode.Add(item);
                if (i % typenum == 0)
                {
                    SN++;
                    db.ModuleInsideTheBox.AddRange(insideTheBoxes);
                    db.SaveChanges();
                    List<string> barcodelsit = new List<string>();
                    var ss = onebarcode[1].Substring(onebarcode[1].IndexOf('B')).ToString();
                    onebarcode.ForEach(c => barcodelsit.Add(c.Substring(c.IndexOf('B')).ToString()));

                    ModuleInsideBoxLablePrint(barcodelsit.ToArray(), ordernum, InnerBarcode, ip, 9101, concentration);

                    onebarcode = new List<string>();
                    insideTheBoxes = new List<ModuleInsideTheBox>();
                }
                i++;
                if (i == count)
                { break; }
            }

        }
        //输入订单号显示内箱条码和SN/TN/类型
        public ActionResult GetModuleInsideTheBoxInfo(string ordernum, string type, string statue)
        {
            JObject result = new JObject();
            var totalCount = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).ToList();
            if (totalCount.Count == 0)
            {
                result.Add("mes", "没有创建装箱规则");
                result.Add("InnerBarcode", null);
                result.Add("SN/TN", null);
                result.Add("Complete", null);
                return Content(JsonConvert.SerializeObject(result));
            }
            else
            {
                var count = totalCount.Where(c => c.Type == type).Sum(c => c.Quantity);
                var totalcount = totalCount.Sum(c => c.Quantity);
                var princount = db.ModuleInsideTheBox.Where(c => c.OrderNum == ordernum && c.Type == type).Select(c => c.InnerBarcode).Distinct().Count();//这里要加statue
                JArray typearray = new JArray();
                totalCount.ForEach(c => typearray.Add(c.Type));
                if (princount >= count)
                {
                    result.Add("mes", "已打印完");
                    result.Add("InnerBarcode", null);
                    result.Add("type", null);
                    result.Add("SN/TN", null);
                    result.Add("Complete", null);
                    return Content(JsonConvert.SerializeObject(result));
                }
                var InnerBarcode = "";
                if (statue == "纸盒")
                {
                    var ordernumArray = ordernum.Split('-');
                    var num = ordernumArray[2].PadLeft(2, '0');
                    InnerBarcode = ordernumArray[0] + ordernumArray[1] + num + "-ZH";
                }
                else
                {
                    var ordernumArray = ordernum.Split('-');
                    var num = ordernumArray[2].PadLeft(2, '0');
                    InnerBarcode = ordernumArray[0] + ordernumArray[1] + num + "-ZX";
                }
                for (var i = 1; i <= totalcount; i++)
                {
                    var temp = InnerBarcode + i.ToString().PadLeft(4, '0');
                    if (db.ModuleInsideTheBox.Where(c => c.InnerBarcode == temp).Count() == 0)
                    {
                        result.Add("mes", "成功");
                        result.Add("InnerBarcode", temp);
                        result.Add("type", typearray);
                        result.Add("SN/TN", i + "/" + totalcount);
                        break;
                    }
                }
                var rule = db.ModulePackageRule.Where(c => c.OrderNum == ordernum).Select(c => new { c.ITEMNO, c.COLOURS, c.Remark }).FirstOrDefault();
                var boxnum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).Sum(c => c.Quantity * c.OuterBoxCapacity);
                result.Add("ITEMNO", rule.ITEMNO);
                result.Add("COLOURS", rule.COLOURS);
                result.Add("Remark", rule.Remark);
                result.Add("CTNS_PSC", boxnum);//数量 3/boxnum
                JArray completeArray = new JArray();
                var ruleinfo = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue != "外箱").Select(c => c.Statue).Distinct();
                foreach (var item in ruleinfo)
                {

                    var statueinfo = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == item).ToList();
                    foreach (var statuintem in statueinfo)
                    {
                        JObject obj = new JObject();
                        obj.Add("Statue", item);//纸盒或者纸箱
                        obj.Add("Type", statuintem.Type);//1装几
                        var printcount = db.ModuleInsideTheBox.Where(c => c.OrderNum == ordernum && c.Statue == item && c.Type == statuintem.Type).Select(c => c.InnerBarcode).Distinct().Count();
                        obj.Add("Num", printcount + "/" + statuintem.Quantity);
                        completeArray.Add(obj);
                    }
                }
                result.Add("Complete", completeArray);
                return Content(JsonConvert.SerializeObject(result));
            }
        }

        //创建内箱记录
        public ActionResult CreateModuleInsideTheBox(List<ModuleInsideTheBox> barcodeList, bool IsLast = false)
        {
            if (((Users)Session["User"]) == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "Inside" });
            }
            JObject result = new JObject();
            var type = barcodeList.Select(c => c.Type).FirstOrDefault();
            var aa = type.Substring(2);
            var num = int.Parse(type.Substring(2));
            if (barcodeList.Count != num && !IsLast)
            {
                result.Add("result", false);
                result.Add("mes", "包装数量有与规则不符");
                return Content(JsonConvert.SerializeObject(result));
            }

            foreach (var item in barcodeList)
            {
                item.InnerTime = DateTime.Now;
                item.Inneror = ((Users)Session["User"]).UserName;
            }
            db.ModuleInsideTheBox.AddRange(barcodeList);
            db.SaveChanges();
            UpdateNewTable(barcodeList.FirstOrDefault().OrderNum, "纸箱", DateTime.Now);
            result.Add("result", true);
            result.Add("mes", "包装成功");
            return Content(JsonConvert.SerializeObject(result));
        }

        //根据订单判断是否有内内箱
        public ActionResult GetpackList(string ordernum)
        {
            var orders = db.ModulePackageRule.OrderByDescending(m => m.ID).Where(c => c.OrderNum == ordernum && c.Statue != "外箱").Select(m => m.Statue).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region------外箱装箱记录

        //根据订单,类型,是否尾箱,屏号,批号,得到重量,完成数量情况,可以打印数量,标签信息
        public ActionResult GetOuthersideBoxInfo(string ordernum, string type, int screenNum = 1, int batchNum = 1, bool IsLast = false)
        {
            JObject result = new JObject();

            var basicInfo = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == type && c.ScreenNum == screenNum && c.Batch == batchNum && c.Statue == "外箱").ToList(); //查找包装规则信息
            var typeList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "外箱").Select(c => c.Type).Distinct().ToList();//查找订单查包装规则的类型
            if (basicInfo.Count == 0)//如果找不到包装规则信息
            {
                result.Add("G_Weight", null);//毛重量
                result.Add("N_Weight", null);//净重
                result.Add("Complate", null);
                result.Add("count", null);//装的数量
                result.Add("boxNum", null);//外箱条码
                result.Add("SNTN", null);//SN/TN
                result.Add("pass", false);
                result.Add("mes", "没找到该订单的类型包装信息");
                return Content(JsonConvert.SerializeObject(result));
            }
            #region 标签信息
            var count = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batchNum && c.Statue == "外箱").Sum(c => c.Quantity);//根据订单、分屏号查找规定的包装数量总和
            var printCount = db.ModuleOutsideTheBox.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batchNum).Select(c => c.OutsideBarcode).Distinct().ToList().Count();//根据订单、分屏号查找打印的外箱数量
            if (printCount < count)//判断打印的数量是否大于定义包装数量,如果打印数量大于等于定义包装数量,则返回null
            {
                #region 外箱条码生成

                string[] str = ordernum.Split('-');//将订单号分割
                string OuterBoxBarCodeNum = "";
                if (str.Count() == 2)
                {
                    OuterBoxBarCodeNum = ordernum + "-" + batchNum.ToString().PadLeft(2, '0') + "-" + screenNum.ToString().PadLeft(2, '0') + "-MK";
                }
                else
                {
                    string start = str[0].Substring(2);//取出 如2017-test-1 的17
                    OuterBoxBarCodeNum = start + str[1] + "-" + str[2] + "-" + batchNum.ToString().PadLeft(2, '0') + "-" + screenNum.ToString().PadLeft(2, '0') + "-MK";//外箱条码组成 

                }
                int SN = 0;
                for (int i = 1; i <= count; i++)//从1开始循环,最大数为定义的打印数,用来确定标签的序列号
                {
                    var num = OuterBoxBarCodeNum + i.ToString().PadLeft(3, '0');//生成含有序列数的标签号

                    if (db.ModuleOutsideTheBox.Count(c => c.OutsideBarcode == num) == 0)//判断打印表里是否有相同的标签号,没有则将此标签号存入数据中,有则继续循环
                    {
                        OuterBoxBarCodeNum = OuterBoxBarCodeNum + i.ToString().PadLeft(3, '0');
                        SN = i;
                        //外箱条码
                        result.Add("boxNum", OuterBoxBarCodeNum);
                        //SN/TN
                        result.Add("SNTN", SN + "/" + count);
                        break;
                    }

                }
                #endregion
            }
            else
            {
                result.Add("G_Weight", null);//毛重量
                result.Add("N_Weight", null);//净重
                result.Add("Complate", null);
                result.Add("count", null);
                result.Add("boxNum", null);//外箱条码
                result.Add("SNTN", null);//SN/TN
                result.Add("pass", false);
                result.Add("mes", "订单已经打印完");
                return Content(JsonConvert.SerializeObject(result));
            }
            #endregion
            #region 数量信息
            var countfromint = basicInfo.FirstOrDefault().OuterBoxCapacity;
            result.Add("count", countfromint);
            //itemof
            var rule = db.ModulePackageRule.Where(c => c.OrderNum == ordernum).Select(c=>new { c.ITEMNO,c.COLOURS,c.Remark}).FirstOrDefault();
            var boxnum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum&&c.Statue=="外箱").Sum(c=>c.Quantity*c.OuterBoxCapacity);
            result.Add("ITEMNO", rule.ITEMNO);
            result.Add("COLOURS", rule.COLOURS);
            result.Add("Remark", rule.Remark);
            result.Add("CTNS_PSC", boxnum);//数量 3/boxnum
            #endregion

            #region 重量信息
            if (IsLast)//是尾箱
            {
                var last = basicInfo.FirstOrDefault();//查找尾箱重量不为0的信息
                if (last != null)//如果信息不为空
                {
                    result.Add("G_Weight", last.Tail_G_Weight);//毛重量
                    result.Add("N_Weight", last.Tail_N_Weight);//净重
                }

            }
            else//不是尾箱
            {
                var full = basicInfo.FirstOrDefault();//找到整箱重量不为0的信息
                if (full != null)//如果信息不为空
                {
                    result.Add("G_Weight", full.Full_G_Weight);//毛重量
                    result.Add("N_Weight", full.Full_N_Weight);//净重
                }
            }
            #endregion
            #region 完成信息
            JArray total = new JArray();
            foreach (var item in typeList)//循环类型
            {
                var batchList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == item && c.Statue == "外箱").Select(c => c.Batch).Distinct().ToList();//根据订单、类型查找包装规则的分屏号
                foreach (var batchnum in batchList)//循环批号
                {
                    var screemNumList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == item && c.Batch == batchnum && c.Statue == "外箱").Select(c => c.ScreenNum).Distinct().ToList();
                    foreach (var screenNumitem in screemNumList)//循环屏序号
                    {
                        JObject info = new JObject();
                        var totleNum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNumitem && c.Batch == batchnum && c.Statue == "外箱").ToList().Count() == 0 ? 0 : db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNumitem && c.Batch == batchnum).Sum(c => c.Quantity);//根据订单号、类型、分屏号查找包装规则的包装总数量(用于显示完成数量的分母显示)
                        var printBarcodeinfo = db.ModuleOutsideTheBox.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNumitem && c.Batch == batchnum).Select(c => c.OutsideBarcode).Distinct();//根据订单号根据订单号、类型、分屏号查找包装打印外箱列表(用于显示完成数量的分子显示)
                        int printBarcode = printBarcodeinfo.Count();
                        //类型
                        info.Add("type", item);
                        //完成数量
                        info.Add("completeNum", printBarcode.ToString() + "/" + totleNum.ToString());
                        //屏序
                        info.Add("screenNum", screenNumitem);
                        //批次
                        info.Add("batchNum", batchnum);
                        //完成率
                        info.Add("complete", totleNum == 0 ? 0 + "%" : ((printBarcode * 100) / totleNum).ToString("F2") + "%");

                        total.Add(info);
                    }
                }
            }
            #region 计算总计
            JObject info2 = new JObject();
            var printBarcodeinfo2 = db.ModuleOutsideTheBox.Where(c => c.OrderNum == ordernum).Select(c => c.OutsideBarcode).Distinct().Count();//根据订单查包装打印的总的外箱数量
            var totle = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "外箱").ToList();
            var totleNum2 = 0;
            if (totle.Count != 0)
            {
                totleNum2 = totle.Sum(c => c.Quantity);//总的打印数量
            }
            #endregion
            //类型
            info2.Add("type", "总计");
            //完成数量
            info2.Add("completeNum", printBarcodeinfo2.ToString() + "/" + totleNum2.ToString());
            //屏序
            info2.Add("screenNum", "--");
            //批次
            info2.Add("batchNum", "--");
            //完成率
            info2.Add("complete", totleNum2 == 0 ? 0 + "%" : ((printBarcodeinfo2 * 100) / totleNum2).ToString("F2") + "%");

            total.Add(info2);
            result.Add("Complate", total);
            result.Add("pass", true);
            result.Add("mes", "查找成功");
            #endregion
            return Content(JsonConvert.SerializeObject(result));
        }

        //检验条码是否规范
        public ActionResult CheckBarcode(string ordernum, string barcode, string statue, bool hybrid = false)
        {
            JObject result = new JObject();
            var order = "";
            var isthree = db.ModulePackageRule.Count(c => c.OrderNum == ordernum && c.Statue == "纸盒");//是否是三级包装,是三级包装的话,内箱里的条码要找纸盒的外箱条码
            if (statue == "外箱")
            {
                order = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == barcode && c.Statue == "纸箱").Select(c => c.OrderNum).FirstOrDefault();//根据条码在条码表找订单
            }
            else if (statue == "纸箱" && isthree == 0)
            {
                order = db.BarCodes.Where(c => c.BarCodesNum == barcode && c.BarCodeType == "模块").Select(c => c.OrderNum).FirstOrDefault();//根据条码在条码表找订单
            }
            else if (statue == "纸箱" && isthree != 0)
            {
                order = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == barcode && c.Statue == "纸盒").Select(c => c.OrderNum).FirstOrDefault();//根据条码在内箱条码表找订单
            }
            else if (statue == "纸盒")
            {
                order = db.BarCodes.Where(c => c.BarCodesNum == barcode && c.BarCodeType == "模块").Select(c => c.OrderNum).FirstOrDefault();//根据条码在内箱条码表找订单
            }
            var exit = db.ModuleAppearance.Count(c => c.ModuleBarcode == barcode && c.AppearanceResult == true);//根据条码查找完成电检数量
                                                                                                                //var iscCheck = db.Packing_InnerCheck.Count(c => c.Barcode == barcode);//查找内检完的数量
            if (order == null)
            {
                result.Add("mes", "不存在此条码");
                result.Add("pass", false);
            }
            else if (order != ordernum && hybrid == false)
            {
                result.Add("mes", "此条码的订单号应为" + order);
                result.Add("pass", false);
            }
            //else if (exit == 0)
            //{
            //    result.Add("mes", "此条码未通过外观电检");
            //    result.Add("pass", false);
            //}

            else
            {
                string Printingexit = "";
                if (statue == "外箱")
                {
                    Printingexit = db.ModuleOutsideTheBox.Where(c => c.InnerBarcode == barcode).Select(c => c.InnerBarcode).FirstOrDefault();//查找打印表里是否有相同的条码号
                }
                if (statue == "纸箱")
                {
                    Printingexit = db.ModuleInsideTheBox.Where(c => c.ModuleBarcode == barcode && c.Statue == "纸箱").Select(c => c.ModuleBarcode).FirstOrDefault();//查找打印表里是否有相同的条码号
                }
                if (statue == "纸盒")
                {
                    Printingexit = db.ModuleInsideTheBox.Where(c => c.ModuleBarcode == barcode && c.Statue == "纸盒").Select(c => c.ModuleBarcode).FirstOrDefault();//查找打印表里是否有相同的条码号
                }
                if (!string.IsNullOrEmpty(Printingexit))//有就传fasle
                {
                    result.Add("mes", "此条码已打印 ");
                    result.Add("pass", false);
                }
                else
                {
                    result.Add("mes", "成功 ");
                    result.Add("pass", true);
                }

            }
            return Content(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 创建外箱打印记录
        /// </summary>
        /// 首先检查需要录入外箱打印表里面的条码号是否重复,重复则提示错误,然后再找订单定义的总数量,将已打印的数量+此次要添加记录的数量,等到的数量与定义总数量做对比,如果定义总数量小于打印数量,则提示错误,否则添加数据
        /// <param name="printings">外箱打印记录</param>
        /// <param name="ordernum">订单号</param>
        /// <param name="isupdate">是否是更新</param>
        /// <returns></returns>
        public ActionResult CreatePackangPrint(List<ModuleOutsideTheBox> printings, string ordernum, string Department1, string Group, bool IsLast = false)
        {
            if (((Users)Session["User"]) == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "页面" });
            }
            JObject result = new JObject();
            if (ModelState.IsValid)//判断数据格式是否正确
            {
                string error = "";

                var type = printings.Select(c => c.Type).FirstOrDefault();
                var aa = type.Substring(2);
                var num = int.Parse(type.Substring(2));
                if (printings.Count != num && !IsLast)
                {
                    result.Add("result", false);
                    result.Add("mes", "包装数量有与规则不符");
                    return Content(JsonConvert.SerializeObject(result));
                }
                foreach (var item in printings)//循环传过来的打印数据集合
                {
                    var exit = db.ModuleOutsideTheBox.Where(c => c.InnerBarcode == item.InnerBarcode).FirstOrDefault();//查看条码是否已经打印了
                    if (exit != null)//已经打印,将信息记录在error中
                    {
                        error = error + exit.InnerBarcode + "已包装在" + exit.OutsideBarcode + ",";
                    }
                }
                if (!string.IsNullOrEmpty(error))//如果error含有数据,则显示数据
                {
                    result.Add("mes", error);
                    result.Add("pass", false);
                    return Content(JsonConvert.SerializeObject(result));
                }

                else//代表没有重复打印的
                {
                    // var temp = printings.FirstOrDefault().OrderNum;
                    var count = db.ModuleOutsideTheBox.Count(c => c.OrderNum == ordernum);//根据订单找打印表数量,代表此订单 已打印的数量
                    var real = db.ModulePackageRule.Where(c => c.OrderNum == ordernum);//查看此订单是否有定义装箱规则
                    int realCount = 0;
                    foreach (var item in real)//循环
                    {
                        realCount = realCount + (item.Quantity * item.OuterBoxCapacity);//计算出总共应该打印多少条码数(装箱容量*装箱数)
                    }
                    if (realCount < count + printings.Count())//如果已打印的数量大于定义总数量
                    {
                        result.Add("mes", "已超过定义的包装数量");
                        result.Add("pass", false);
                        return Content(JsonConvert.SerializeObject(result));
                    }
                    printings.ForEach(c => { c.Department = Department1; c.Group = Group; c.InnerTime = DateTime.Now; c.Inneror = ((Users)Session["User"]).UserName; });//保存数据
                    db.ModuleOutsideTheBox.AddRange(printings);
                    db.SaveChanges();
                    UpdateNewTable(ordernum, "外箱", DateTime.Now);
                    result.Add("mes", "打印成功");
                    result.Add("pass", true);
                    return Content(JsonConvert.SerializeObject(result));
                }
            }
            result.Add("mes", "传入类型不对，请确认");
            result.Add("pass", false);
            return Content(JsonConvert.SerializeObject(result));
        }

        //列表,根据订单号找到类型
        public ActionResult GetTypeList(string ordernum, string statue)
        {
            var orders = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).Select(m => m.Type).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //列表,根据订单号,类型找到批号
        public ActionResult GetBatchList(string ordernum, string type, string statue)
        {
            var orders = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == type && c.Statue == statue).Select(m => m.Batch).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //列表,根据订单号,类型,批号找到屏号
        public ActionResult GetScreenList(string ordernum, string type, int batch, string statue)
        {
            var orders = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == type && c.Batch == batch && c.Statue == statue).Select(m => m.ScreenNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 内外箱共同操作

        //内外箱查看
        public ActionResult DisplayPackage(string ordernum, string packageBarcode, string statu)
        {
            JArray Bacodelist = new JArray();
            //List<string> Bacodelist = new List<string>();
            if (!string.IsNullOrEmpty(packageBarcode))
            {
                Bacodelist.Add(packageBarcode);
                return Content(JsonConvert.SerializeObject(Bacodelist));
            }
            if (statu == "纸箱" || statu == "纸盒")
            {
                var barcodelist = db.ModuleInsideTheBox.OrderBy(c => c.InnerBarcode).Where(c => c.OrderNum == ordernum && c.Statue == statu).Select(c => c.InnerBarcode).ToList();

                barcodelist = barcodelist.Distinct<string>().ToList();
                barcodelist.ForEach(c => Bacodelist.Add(c));
                return Content(JsonConvert.SerializeObject(Bacodelist));
            }
            else
            {
                var barcodelist = db.ModuleOutsideTheBox.OrderBy(c => c.OutsideBarcode).Where(c => c.OrderNum == ordernum).Select(c => c.OutsideBarcode).ToList();
                barcodelist = barcodelist.Distinct<string>().ToList();
                barcodelist.ForEach(c => Bacodelist.Add(c));
                return Content(JsonConvert.SerializeObject(Bacodelist));
            }
        }

        //内外箱删除列表显示
        public ActionResult DisplayDeletePackage(string ordernum, string packageBarcode, string statu)
        {
            JArray Bacodelist = new JArray();
            if (!string.IsNullOrEmpty(packageBarcode))
            {
                Bacodelist.Add(packageBarcode);
                return Content(JsonConvert.SerializeObject(Bacodelist));
            }
            if (statu == "纸箱" || statu == "纸盒")
            {
                var barcodelist = db.ModuleInsideTheBox.Where(c => c.OrderNum == ordernum && c.Statue == statu).Select(c => c.InnerBarcode).Distinct().ToList();

                var outside = db.ModuleOutsideTheBox.Where(c => c.OrderNum == ordernum).Select(c => c.InnerBarcode).Distinct().ToList();
                var result = barcodelist.Except(outside).ToList();
                result.ForEach(c => Bacodelist.Add(c));
                return Content(JsonConvert.SerializeObject(Bacodelist));
            }
            else
            {
                var barcodelist = db.ModuleOutsideTheBox.Where(c => c.OrderNum == ordernum).Select(c => c.OutsideBarcode).Distinct().ToList();
                var warehouse = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.State == "模块").Select(c => c.OuterBoxBarcode
                ).Distinct().ToList();
                var result = barcodelist.Except(warehouse).ToList();
                result.ForEach(c => Bacodelist.Add(c));

                return Content(JsonConvert.SerializeObject(Bacodelist));
            }
        }

        //内外箱删除方法
        public void DeletePackage(List<string> packageBarcode, string statu)
        {
            if (statu == "纸箱" || statu == "纸盒")
            {
                var list = db.ModuleInsideTheBox.Where(c => packageBarcode.Contains(c.InnerBarcode)).ToList();
                db.ModuleInsideTheBox.RemoveRange(list);
                db.SaveChanges();
                UpdateNewTable(list.FirstOrDefault().OrderNum, statu, DateTime.Now);
            }
            else
            {
                var list = db.ModuleOutsideTheBox.Where(c => packageBarcode.Contains(c.OutsideBarcode)).ToList();
                db.ModuleOutsideTheBox.RemoveRange(list);
                db.SaveChanges();
                UpdateNewTable(list.FirstOrDefault().OrderNum, statu, DateTime.Now);
            }

        }
        #endregion
        //列表选择
        public ActionResult GetOrderNumList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Where(c => c.Models != 0).Select(m => m.OrderNum).ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        #region 模块外箱标签打印
        #region---包装打印标签
        //打印标签
        [HttpPost]
        public ActionResult OutsideBoxLablePrint(int screennum = 1, string ordernum = "", string packagingordernum = "", string outsidebarcode = "", string material_discription = "", int pagecount = 1, string sntn = "", string qty = "", bool logo = true, string ip = "", int port = 0, int concentration = 5, string[] mn_list = null, double? g_Weight = null, double? n_Weight = null, string leng = "", bool testswitch = false, int BPPJNum = 0, string version = "old")
        {
            if (BPPJNum != 0)
            {
                var array = sntn.Split('/');
                sntn = array[0] + "/" + (int.Parse(array[1]) + BPPJNum);
            }
            if (testswitch == true)
            {
                //组织数据

                var AllBitmap = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
                MemoryStream ms = new MemoryStream();
                AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                AllBitmap.Dispose();
                return File(ms.ToArray(), "image/Png");
            }
            else
            {
                if (!String.IsNullOrEmpty(packagingordernum)) ordernum = packagingordernum;//如果有包装新订单号，则使用包装新订单号。
                if (version == "new")
                {
                    var bm = CreateOutsideBoxLableNewVersion(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
                    string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                    int totalbytes = bm.ToString().Length;
                    int rowbytes = 10;
                    string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                    data += totalbytes + "," + rowbytes + "," + hex;
                    data += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
                    string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                    return Content(result);
                }
                else
                {
                    var bm = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
                    string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                    int totalbytes = bm.ToString().Length;
                    int rowbytes = 10;
                    string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                    data += totalbytes + "," + rowbytes + "," + hex;
                    data += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
                    string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                    return Content(result);
                }
            }
        }
        #endregion

        #region---生成标签图片
        //生成标签
        public Bitmap CreateOutsideBoxLable(string[] mn_list, string ordernum = "", string outsidebarcode = "", string material_discription = "", string sntn = "", string qty = "", bool logo = true, int screennum = 1, double? g_Weight = null, double? n_Weight = null, string leng = "")
        {
            //开始绘制图片
            int initialWidth = 750, initialHeight = 1000;//高4宽3
            Bitmap AllBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(AllBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            //double beishuhege = 0.37;
            Pen pen = new Pen(Color.Black, 3);
            theGraphics.DrawRectangle(pen, 50, 50, 640, 890);
            //横线
            theGraphics.DrawLine(pen, 50, 160, 690, 160);
            theGraphics.DrawLine(pen, 50, 280, 690, 280);
            theGraphics.DrawLine(pen, 50, 340, 690, 340);
            theGraphics.DrawLine(pen, 50, 400, 690, 400);
            theGraphics.DrawLine(pen, 50, 460, 690, 460);
            //竖线
            theGraphics.DrawLine(pen, 250, 280, 250, 460);
            theGraphics.DrawLine(pen, 400, 280, 400, 460);
            theGraphics.DrawLine(pen, 570, 280, 570, 460);

            //引入LOGO
            if (logo)
            {
                Bitmap bmp_logo = new Bitmap(@"D:\\MES_Data\\LOGO_black.png");
                //double beishulogo = 0.95;
                theGraphics.DrawImage(bmp_logo, 65, 60, (float)(bmp_logo.Width), (float)(bmp_logo.Height));
                //引入订单号
                System.Drawing.Font myFont_ordernum;
                myFont_ordernum = new System.Drawing.Font("Microsoft YaHei UI", 40, FontStyle.Bold);
                StringFormat geshi = new StringFormat();
                geshi.Alignment = StringAlignment.Center; //居中
                                                          //geshi.Alignment = StringAlignment.Far; //右对齐
                theGraphics.DrawString(ordernum, myFont_ordernum, bush, 230, 90);
            }
            //引入订单号
            else
            {
                System.Drawing.Font myFont_ordernum;
                myFont_ordernum = new System.Drawing.Font("Microsoft YaHei UI", 55, FontStyle.Bold);
                StringFormat geshi = new StringFormat();
                geshi.Alignment = StringAlignment.Center; //居中
                theGraphics.DrawString(ordernum, myFont_ordernum, bush, 100, 60);
            }
            //引入条码
            //if (String.IsNullOrEmpty(outsidebarcode)) return Content("条码号为空！");
            Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(outsidebarcode, 600, 70);
            theGraphics.DrawImage(bmp_barcode, 70, 170, (float)bmp_barcode.Width, (float)bmp_barcode.Height);

            //引入条码号
            System.Drawing.Font myFont_boxbarcode;
            myFont_boxbarcode = new System.Drawing.Font("Microsoft YaHei UI", 22, FontStyle.Bold);
            theGraphics.DrawString(outsidebarcode, myFont_boxbarcode, bush, 200, 240);

            #region--未加净重毛重前版本
            ////引入物料描述
            //System.Drawing.Font myFont_material_discription;
            //myFont_material_discription = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString("物料描述(DESC)", myFont_material_discription, bush, 55, 295);

            ////引入物料描述内容
            //System.Drawing.Font myFont_material_discription_content;
            //StringFormat geshi1 = new StringFormat();
            //geshi1.Alignment = StringAlignment.Center; //居中
            //myFont_material_discription_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString(material_discription, myFont_material_discription_content, bush, 275, 295);

            ////引入屏序号
            //System.Drawing.Font myFont_screennum;
            //myFont_screennum = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString("屏序号(NO.)", myFont_screennum, bush, 410, 295);

            ////引入屏序号值
            //System.Drawing.Font myFont_screennum_data;
            //StringFormat geshi2 = new StringFormat();
            //geshi1.Alignment = StringAlignment.Center; //居中
            //myFont_screennum_data = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString(screennum.ToString(), myFont_screennum_data, bush, 610, 295);

            ////引入SN/TN
            //System.Drawing.Font myFont_sntn;
            //myFont_sntn = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString("件号/数(SN/TN)", myFont_sntn, bush, 55, 355);

            ////引入SN/TN内容
            //System.Drawing.Font myFont_sntn_content;
            //myFont_sntn_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString(sntn, myFont_sntn_content, bush, 290, 355);

            ////引入数量QTY
            //System.Drawing.Font myFont_qty;
            //myFont_qty = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString("数量(QTY)", myFont_qty, bush, 410, 355);

            ////引入数量QTY内容
            //System.Drawing.Font myFont_qty_content;
            //myFont_qty_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString(qty + " PCS", myFont_qty_content, bush, 585, 355);

            ////引入模组号清单
            //int mn_E_count = mn_list.Count();
            ////12位模组号以上，包括条码号
            //if (mn_E_count > 12 && mn_E_count <= 20)
            //{
            //    if (mn_list[0].Length < 7)
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 28, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 70;
            //        for (int i = 1; i < mn_list.Count() + 1; i++)
            //        {
            //            theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 4) != 0)
            //            {
            //                left_x += 155;
            //            }
            //            else
            //            {
            //                top_y += 100;
            //                left_x -= 465;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 22, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 55;
            //        for (int i = 0; i < mn_list.Count(); i++)
            //        {
            //            theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 2) == 0)
            //            {
            //                left_x += 315;
            //            }
            //            else
            //            {
            //                top_y += 50;
            //                left_x -= 315;
            //            }
            //        }
            //    }
            //}
            ////11-12位模组号
            //else if (mn_E_count > 10 && mn_E_count <= 12)
            //{
            //    if (mn_list[0].Length < 7)
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 50, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 90;
            //        for (int i = 0; i < mn_list.Count(); i++)
            //        {
            //            theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 2) == 0)
            //            {
            //                left_x += 290;
            //            }
            //            else
            //            {
            //                top_y += 85;
            //                left_x -= 290;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 26, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 160;
            //        for (int i = 0; i < mn_list.Count(); i++)
            //        {
            //            theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            top_y += 42;
            //        }
            //    }
            //}
            ////9-10位模组号
            //else if (mn_E_count > 8 && mn_E_count <= 10)
            //{
            //    if (mn_list[0].Length < 7)
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 45, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 90;
            //        for (int i = 0; i < mn_list.Count(); i++)
            //        {
            //            theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 2) == 0)
            //            {
            //                left_x += 290;
            //            }
            //            else
            //            {
            //                top_y += 100;
            //                left_x -= 290;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 30, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 150;
            //        for (int i = 0; i < mn_list.Count(); i++)
            //        {
            //            theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            top_y += 50;
            //        }

            //    }

            //}
            ////7-8位模组号
            //else if (mn_E_count > 6 && mn_E_count <= 8)
            //{
            //    if (mn_list[0].Length < 7)
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 50, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 80;
            //        for (int i = 1; i < mn_list.Count() + 1; i++)
            //        {
            //            theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 2) != 0)
            //            {
            //                left_x += 300;
            //            }
            //            else
            //            {
            //                top_y += 126;
            //                left_x -= 300;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 36, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 410;
            //        int left_x = 110;
            //        for (int i = 1; i < mn_list.Count() + 1; i++)
            //        {
            //            theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            top_y += 65;
            //        }
            //    }
            //}
            ////1-6位模组号
            //else if (mn_E_count <= 6)
            //{
            //    if (mn_list[0].Length < 7)
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 50, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 450;
            //        int left_x = 80;
            //        for (int i = 1; i < mn_list.Count() + 1; i++)
            //        {
            //            theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 2) != 0)
            //            {
            //                left_x += 300;
            //            }
            //            else
            //            {
            //                top_y += 150;
            //                left_x -= 300;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 40, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 80;
            //        for (int i = 1; i < mn_list.Count() + 1; i++)
            //        {
            //            theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            top_y += 85;
            //        }

            //    }
            //}
            //else
            //{
            //    if (mn_list[0].Length < 7)
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 28, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 70;
            //        for (int i = 1; i < mn_list.Count() + 1; i++)
            //        {
            //            theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 4) != 0)
            //            {
            //                left_x += 155;
            //            }
            //            else
            //            {
            //                top_y += 50;
            //                left_x -= 465;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 16, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 90;
            //        for (int i = 0; i < mn_list.Count(); i++)
            //        {
            //            theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 2) == 0)
            //            {
            //                left_x += 315;
            //            }
            //            else
            //            {
            //                top_y += 26;
            //                left_x -= 315;
            //            }
            //        }
            //    }
            //}
            #endregion

            //引入毛重量
            System.Drawing.Font myFont_g_Weight;
            myFont_g_Weight = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            System.Drawing.Font myFont_n_Weight;
            myFont_n_Weight = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            System.Drawing.Font myFont_material_discription;
            myFont_material_discription = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            System.Drawing.Font myFont_screennum;
            myFont_screennum = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            System.Drawing.Font myFont_sntn;
            myFont_sntn = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            System.Drawing.Font myFont_qty;
            myFont_qty = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            if (leng == "简")
            {
                theGraphics.DrawString("毛重量(kg)", myFont_g_Weight, bush, 55, 295);
                theGraphics.DrawString("净重(kg)", myFont_n_Weight, bush, 410, 295);
                theGraphics.DrawString("物料描述", myFont_material_discription, bush, 55, 355);
                theGraphics.DrawString("屏序号", myFont_screennum, bush, 410, 355);
                theGraphics.DrawString("件号/数", myFont_sntn, bush, 55, 415);
                theGraphics.DrawString("数量", myFont_qty, bush, 410, 415);
            }
            else if (leng == "英")
            {
                theGraphics.DrawString("G.W.(kg)", myFont_g_Weight, bush, 55, 295);
                theGraphics.DrawString("N.W.(kg)", myFont_n_Weight, bush, 410, 295);
                theGraphics.DrawString("DESC", myFont_material_discription, bush, 55, 355);
                theGraphics.DrawString("NO.", myFont_screennum, bush, 410, 355);
                theGraphics.DrawString("SN/TN", myFont_sntn, bush, 55, 415);
                theGraphics.DrawString("QTY", myFont_qty, bush, 410, 415);
            }
            else if (leng == "繁")
            {
                theGraphics.DrawString("毛重量(kg)", myFont_g_Weight, bush, 55, 295);
                theGraphics.DrawString("淨重(kg)", myFont_n_Weight, bush, 410, 295);
                theGraphics.DrawString("物料描述", myFont_material_discription, bush, 55, 355);
                theGraphics.DrawString("屏序號", myFont_screennum, bush, 410, 355);
                theGraphics.DrawString("件號/數", myFont_sntn, bush, 55, 415);
                theGraphics.DrawString("數量", myFont_qty, bush, 410, 415);
            }
            else if (leng == "繁/英")
            {
                theGraphics.DrawString("毛重量(G.W.)kg", myFont_g_Weight, bush, 55, 295);
                theGraphics.DrawString("淨重(N.W.)kg", myFont_n_Weight, bush, 410, 295);
                theGraphics.DrawString("物料描述(DESC)", myFont_material_discription, bush, 55, 355);
                theGraphics.DrawString("屏序號(NO.)", myFont_screennum, bush, 410, 355);
                theGraphics.DrawString("件號/數(SN/TN)", myFont_sntn, bush, 55, 415);
                theGraphics.DrawString("數量(QTY)", myFont_qty, bush, 410, 415);
            }
            else
            {
                theGraphics.DrawString("毛重量(G.W.)kg", myFont_g_Weight, bush, 55, 295);
                theGraphics.DrawString("净重(N.W.)kg", myFont_n_Weight, bush, 410, 295);
                theGraphics.DrawString("物料描述(DESC)", myFont_material_discription, bush, 55, 355);
                theGraphics.DrawString("屏序号(NO.)", myFont_screennum, bush, 410, 355);
                theGraphics.DrawString("件号/数(SN/TN)", myFont_sntn, bush, 55, 415);
                theGraphics.DrawString("数量(QTY)", myFont_qty, bush, 410, 415);
            }

            //引入毛重量值
            System.Drawing.Font myFont_g_Weight_content;
            StringFormat geshi1 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            myFont_g_Weight_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);

            //double G_Weight = g_Weight == null ? 0 : (double)g_Weight;           
            theGraphics.DrawString(g_Weight.ToString(), myFont_g_Weight_content, bush, 280, 295);
            

            //引入净重值
            System.Drawing.Font myFont_n_Weight_content;
            StringFormat geshi2 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            myFont_n_Weight_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //double N_Weight = n_Weight == null ? 0 : (double)n_Weight;
            theGraphics.DrawString(n_Weight.ToString(), myFont_n_Weight_content, bush, 600, 295);

            //引入物料描述内容
            if (leng == "英")
            {
                System.Drawing.Font myFont_material_discription_content;
                myFont_material_discription_content = new System.Drawing.Font("Microsoft YaHei UI", 15, FontStyle.Regular);
                theGraphics.DrawString(material_discription, myFont_material_discription_content, bush, 265, 355);
            }
            else
            {
                System.Drawing.Font myFont_material_discription_content;
                myFont_material_discription_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString(material_discription, myFont_material_discription_content, bush, 275, 355);
            }
           

            //引入屏序号值
            System.Drawing.Font myFont_screennum_data;
            StringFormat geshi3 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            myFont_screennum_data = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(screennum.ToString(), myFont_screennum_data, bush, 615, 355);

           
            //引入SN/TN内容
            System.Drawing.Font myFont_sntn_content;
            myFont_sntn_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(sntn, myFont_sntn_content, bush, 290, 415);

           
            //引入数量QTY内容
            System.Drawing.Font myFont_qty_content;
            myFont_qty_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(qty + " PCS", myFont_qty_content, bush, 585, 415);

            //引入模组号清单
            int mn_E_count = mn_list.Count();
            //12位模组号以上，包括条码号
            if (mn_E_count > 5 && mn_E_count <= 10)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 470;
                int left_x = 50;
                for (int i = 0; i < mn_list.Count(); i++)
                {
                    theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 2) == 0)
                    {
                        left_x += 325;
                    }
                    else
                    {
                        top_y += 50;
                        left_x = 50;
                    }
                }

            }
            //11-12位模组号
            else if (mn_E_count <= 5)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 30, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 485;
                int left_x = 180;
                for (int i = 0; i < mn_list.Count(); i++)
                {
                    theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);

                    top_y += 90;
                }

            }
            //组织数据
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
        }
        #endregion

        #region---生成标签图片新版本
        //生成标签
        public Bitmap CreateOutsideBoxLableNewVersion(string[] mn_list, string ordernum, string outsidebarcode, string material_discription, string sntn, string qty, bool logo = false, int screennum = 1, double? g_Weight = null, double? n_Weight = null, string leng = "")
        {
            //开始绘制图片
            int initialWidth = 750, initialHeight = 1000;//高4宽3
            Bitmap AllBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(AllBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            //double beishuhege = 0.37;
            Pen pen = new Pen(Color.Black, 3);
            theGraphics.DrawRectangle(pen, 50, 50, 640, 850);
            //横线
            theGraphics.DrawLine(pen, 50, 108, 690, 108);//起点x,起点y坐标，终点x,终点y坐标
            theGraphics.DrawLine(pen, 50, 175, 690, 175);
            theGraphics.DrawLine(pen, 50, 242, 690, 242);
            theGraphics.DrawLine(pen, 50, 308, 690, 308);
            theGraphics.DrawLine(pen, 50, 375, 690, 375);
            theGraphics.DrawLine(pen, 50, 441, 690, 441);
            theGraphics.DrawLine(pen, 50, 570, 690, 570);
            //竖线
            theGraphics.DrawLine(pen, 250, 50, 250, 242);
            theGraphics.DrawLine(pen, 400, 108, 400, 242);
            theGraphics.DrawLine(pen, 570, 108, 570, 242);

            theGraphics.DrawLine(pen, 250, 308, 250, 441);
            theGraphics.DrawLine(pen, 400, 308, 400, 441);
            theGraphics.DrawLine(pen, 570, 308, 570, 441);
           
            System.Drawing.Font myFont_g_Weight;
            myFont_g_Weight = new System.Drawing.Font("Microsoft YaHei UI", 13, FontStyle.Regular);

            if (leng == "简")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                System.Drawing.Font myFont2;
                myFont2 = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Regular);
                theGraphics.DrawString("订单号", myFont2, bush, 60, 60);

                theGraphics.DrawString("规格型号", myFont2, bush, 60, 120);

                theGraphics.DrawString("颜色", myFont2, bush, 405, 120);

                theGraphics.DrawString("数量", myFont2, bush, 60, 190);

                theGraphics.DrawString("件号", myFont2, bush, 405, 190);

                theGraphics.DrawString("物料描述", myFont2, bush, 60, 320);

                theGraphics.DrawString("屏序号", myFont2, bush, 405, 320);

                theGraphics.DrawString("毛重 kg", myFont2, bush, 60, 385);

                theGraphics.DrawString("净重 kg", myFont2, bush, 405, 385);

            }
            else if (leng == "简/英" || leng == "")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 13, FontStyle.Regular);
                System.Drawing.Font myFont2;
                myFont2 = new System.Drawing.Font("Microsoft YaHei UI", 15, FontStyle.Regular);
                theGraphics.DrawString("订单号 P0#", myFont2, bush, 60, 60);

                theGraphics.DrawString("规格型号ITEM NO", myFont2, bush, 60, 120);

                theGraphics.DrawString("颜色 COLOURS", myFont2, bush, 405, 120);

                theGraphics.DrawString("数量 QTY(CTNS/PCS)", myFont, bush, 60, 190);

                theGraphics.DrawString("件号 PACKAGE#", myFont2, bush, 405, 190);

                theGraphics.DrawString("物料描述(DESC)", myFont2, bush, 60, 320);

                theGraphics.DrawString("屏序号(NO.)", myFont2, bush, 405, 320);

                theGraphics.DrawString("毛重(G.W)kg", myFont2, bush, 60, 385);

                theGraphics.DrawString("净重(N.W)kg", myFont2, bush, 405, 385);
            }
            else if (leng == "繁")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                System.Drawing.Font myFont2;
                myFont2 = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Regular);
                theGraphics.DrawString("訂單號", myFont2, bush, 60, 60);

                theGraphics.DrawString("規格型號", myFont2, bush, 60, 120);

                theGraphics.DrawString("顏色", myFont2, bush, 405, 120);

                theGraphics.DrawString("數量", myFont2, bush, 60, 190);

                theGraphics.DrawString("件號", myFont2, bush, 405, 190);

                theGraphics.DrawString("物料描述", myFont, bush, 60, 320);

                theGraphics.DrawString("屏序號", myFont2, bush, 405, 320);

                theGraphics.DrawString("毛重 kg", myFont2, bush, 60, 385);

                theGraphics.DrawString("淨重 kg", myFont2, bush, 405, 385);
            }
            else if (leng == "繁/英")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 13, FontStyle.Regular);
                System.Drawing.Font myFont2;
                myFont2 = new System.Drawing.Font("Microsoft YaHei UI", 15, FontStyle.Regular);
                theGraphics.DrawString("訂單號 P0#", myFont2, bush, 60, 60);

                theGraphics.DrawString("規格型號ITEM NO", myFont2, bush, 60, 120);

                theGraphics.DrawString("顏色 COLOURS", myFont2, bush, 405, 120);

                theGraphics.DrawString("數量 QTY(CTNS/PCS)", myFont, bush, 60, 190);

                theGraphics.DrawString("件號 PACKAGE#", myFont2, bush, 405, 190);

                theGraphics.DrawString("物料描述(DESC)", myFont2, bush, 60, 320);

                theGraphics.DrawString("屏序號(NO.)", myFont2, bush, 405, 320);

                theGraphics.DrawString("毛重(G.W)kg", myFont2, bush, 60, 385);

                theGraphics.DrawString("淨重(N.W)kg", myFont2, bush, 405, 385);
            }
            else if (leng == "英")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 17, FontStyle.Regular);
                System.Drawing.Font myFont2;
                myFont2 = new System.Drawing.Font("Microsoft YaHei UI", 21, FontStyle.Regular);
                theGraphics.DrawString("P0#", myFont2, bush, 60, 60);

                theGraphics.DrawString("ITEM NO", myFont, bush, 60, 120);

                theGraphics.DrawString("COLOURS", myFont, bush, 405, 120);

                theGraphics.DrawString("QTY(CTNS/PCS)", myFont, bush, 60, 190);

                theGraphics.DrawString("PACKAGE#", myFont, bush, 405, 190);

                theGraphics.DrawString("DESC", myFont2, bush, 60, 320);

                theGraphics.DrawString("NO.", myFont2, bush, 405, 320);

                theGraphics.DrawString("(G.W)kg", myFont2, bush, 60, 385);

                theGraphics.DrawString("(N.W)kg", myFont2, bush, 405, 385);
            }
            //订单
            System.Drawing.Font value;
            value = new System.Drawing.Font("Microsoft YaHei UI", 15, FontStyle.Regular);
            System.Drawing.Font value2;
            value2 = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Regular);
            StringFormat geshi = new StringFormat();
            geshi.Alignment = StringAlignment.Center; //居中
            //geshi.LineAlignment = StringAlignment.Center; //居中
            theGraphics.DrawString(ordernum, value2, bush, 320, 60);

            //规格型号
            var info = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "外箱").FirstOrDefault();
            theGraphics.DrawString(info.ITEMNO, value2, bush, 258, 120);
            //颜色
            theGraphics.DrawString(info.COLOURS, value, bush, 575, 120);
            //数量
            string boxnum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "外箱").Sum(c => c.Quantity * c.OuterBoxCapacity).ToString();
            theGraphics.DrawString(qty + "/" + boxnum, value2, bush, 258, 190);
            //件号
            string[] s = sntn.Split('/');
            theGraphics.DrawString(s[0] + " OF " + s[1], value, bush, 575, 190);
            //备注
            theGraphics.DrawString(info.Remark, value2, bush, 215, 260);
            //物料描述
            if (leng == "英")
            {
                System.Drawing.Font value3;
                value3 = new System.Drawing.Font("Microsoft YaHei UI", 15, FontStyle.Regular);
                theGraphics.DrawString(material_discription, value3, bush, 255, 325);
            }
            else
            {
                theGraphics.DrawString(material_discription, value2, bush, 258, 320);
            }
            //屏序号
            theGraphics.DrawString(screennum.ToString(), value2, bush, 575, 320);
            //毛重
            theGraphics.DrawString(g_Weight.ToString(), value2, bush, 258, 385);
            //净重
            theGraphics.DrawString(n_Weight.ToString(), value2, bush, 575, 385);
            //条形码
            Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(outsidebarcode, 600, 70);
            theGraphics.DrawImage(bmp_barcode, 70, 450, (float)bmp_barcode.Width, (float)bmp_barcode.Height);

            //引入条码号
            System.Drawing.Font myFont_boxbarcode;
            myFont_boxbarcode = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(outsidebarcode, myFont_boxbarcode, bush, 220, 530);

            //引入模组号清单
            int mn_E_count = mn_list.Count();
            //12位模组号以上，包括条码号
            if (mn_E_count > 5 && mn_E_count <= 10)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Regular);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 600;
                int left_x = 50;
                for (int i = 0; i < mn_list.Count(); i++)
                {
                    theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 2) == 0)
                    {
                        left_x += 325;
                    }
                    else
                    {
                        top_y += 50;
                        left_x = 50;
                    }
                }

            }
            //11-12位模组号
            else if (mn_E_count <= 5)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 30, FontStyle.Regular);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 600;
                int left_x = 180;
                for (int i = 0; i < mn_list.Count(); i++)
                {
                    theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);

                    top_y += 90;
                }

            }
            //组织数据
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
        }
        #endregion

        #region ---查看标签
        public ActionResult OutsideBoxLablePrintToImg(string outsidebarcode, int BPPJNum = 0, string leng = "", string version = "old")
        {
            var outsidebarcode_recordlist = db.ModuleOutsideTheBox.Where(c => c.OutsideBarcode == outsidebarcode);
            var screem = outsidebarcode_recordlist.FirstOrDefault().ScreenNum;
            var batch = outsidebarcode_recordlist.FirstOrDefault().Batch;
            var ordernum1 = db.ModuleOutsideTheBox.Where(c => c.OutsideBarcode == outsidebarcode).Select(c => new { c.OrderNum, c.EmbezzleOrdeNum }).FirstOrDefault();//订单和挪用订单
            string ordernum = string.IsNullOrEmpty(ordernum1.EmbezzleOrdeNum) ? ordernum1.OrderNum : ordernum1.EmbezzleOrdeNum;//如果挪用订单不为空，则显示挪用订单，否则显示本来订单号
            string type = outsidebarcode_recordlist.FirstOrDefault().Type;
            string material_discription = outsidebarcode_recordlist.FirstOrDefault().Materiel;
            if (leng == "英")
            {
                material_discription = "LED Modules";
            }
            string sntn = "";
            if (BPPJNum != 0)
            {
                sntn = outsidebarcode_recordlist.FirstOrDefault().SN + "/" + (db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.ScreenNum == screem && c.Batch == batch && c.Statue == "外箱").Sum(c => c.Quantity) + BPPJNum);
            }
            else
            {
                sntn = outsidebarcode_recordlist.FirstOrDefault().SN + "/" + db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.ScreenNum == screem && c.Batch == batch && c.Statue == "外箱").Sum(c => c.Quantity);
            }
            bool logo = outsidebarcode_recordlist.FirstOrDefault().IsLogo;
            double? g_Weight = outsidebarcode_recordlist.FirstOrDefault().G_Weight;
            double? n_Weight = outsidebarcode_recordlist.FirstOrDefault().N_Weight;
            string[] mn_list = outsidebarcode_recordlist.Select(c => c.InnerBarcode).ToArray();

            string qty = mn_list.Count().ToString();
            int screennum = screem;   //屏序号
                                      //如果有包装新订单号，则使用包装新订单号。
            string packagingordernum = outsidebarcode_recordlist.FirstOrDefault().PackagingOrderNum;
            ordernum = String.IsNullOrEmpty(packagingordernum) ? ordernum : packagingordernum;
            if (version == "new")
            {
                var AllBitmap = CreateOutsideBoxLableNewVersion(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
                MemoryStream ms = new MemoryStream();
                AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                AllBitmap.Dispose();
                return File(ms.ToArray(), "image/Png");
            }
            else
            {
                var AllBitmap = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
                MemoryStream ms = new MemoryStream();
                AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                AllBitmap.Dispose();
                return File(ms.ToArray(), "image/Png");
            }
        }
        #endregion

        #region---重复打印
        public ActionResult OutsideBoxLablePrintAgain(string outsidebarcode, int BPPJNum = 0, int pagecount = 1, int concentration = 5, string ip = "", int port = 0, string leng = "", string version = "old")
        {
            var outsidebarcode_recordlist = db.ModuleOutsideTheBox.Where(c => c.OutsideBarcode == outsidebarcode);
            var screem = outsidebarcode_recordlist.FirstOrDefault().ScreenNum;
            var batch = outsidebarcode_recordlist.FirstOrDefault().Batch;
            var ordernum1 = db.ModuleOutsideTheBox.Where(c => c.OutsideBarcode == outsidebarcode).Select(c => new { c.OrderNum, c.EmbezzleOrdeNum }).FirstOrDefault();//订单和挪用订单
            string ordernum = string.IsNullOrEmpty(ordernum1.EmbezzleOrdeNum) ? ordernum1.OrderNum : ordernum1.EmbezzleOrdeNum;//如果挪用订单不为空，则显示挪用订单，否则显示本来订单号
            string type = outsidebarcode_recordlist.FirstOrDefault().Type;
            string material_discription = outsidebarcode_recordlist.FirstOrDefault().Materiel;
            if (leng == "英")
            {
                material_discription = "LED Modules";
            }
            string sntn = "";
            if (BPPJNum != 0)
            {
                sntn = outsidebarcode_recordlist.FirstOrDefault().SN + "/" + (db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.ScreenNum == screem && c.Batch == batch && c.Statue == "外箱").Sum(c => c.Quantity) + BPPJNum);
            }
            else
            {
                sntn = outsidebarcode_recordlist.FirstOrDefault().SN + "/" + db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.ScreenNum == screem && c.Batch == batch && c.Statue == "外箱").Sum(c => c.Quantity);
            }
            bool logo = outsidebarcode_recordlist.FirstOrDefault().IsLogo;
            double? g_Weight = outsidebarcode_recordlist.FirstOrDefault().G_Weight;
            double? n_Weight = outsidebarcode_recordlist.FirstOrDefault().N_Weight;
            string[] mn_list = outsidebarcode_recordlist.Select(c => c.InnerBarcode).ToArray();

            string qty = mn_list.Count().ToString();
            int screennum = screem;   //屏序号
                                      //如果有包装新订单号，则使用包装新订单号。
            string packagingordernum = outsidebarcode_recordlist.FirstOrDefault().PackagingOrderNum;
            ordernum = String.IsNullOrEmpty(packagingordernum) ? ordernum : packagingordernum;
            if (version == "new")
            {
                var bm = CreateOutsideBoxLableNewVersion(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                return Content(result);
            }
            else
            {
                var bm = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                return Content(result);
            }



        }
        #endregion
        #endregion

        #region 模块内箱标签打印

        public void temp(string ip = "", int port = 0, int concentration = 5)
        {
            string orderNumber = "2020-YA429-1";
            string[] sy = orderNumber.Split('-');
            //需要打印几个
            var num = 330;
            var barcode = db.BarCodes.Where(c => c.OrderNum == orderNumber && c.BarCodeType == "模块").Select(c => c.BarCodesNum).ToList();
            var printbarcode = db.ModuleInsideTheBox.Where(c => c.OrderNum == orderNumber && c.Statue == "纸箱").Select(c => c.ModuleBarcode).ToList();
            //需要打印几个
            var notprintbarcode = barcode.Except(printbarcode).ToList().Take(num);
            List<string> tempprint = new List<string>();
            List<string> lastprint = new List<string>();
            List<ModuleInsideTheBox> modules = new List<ModuleInsideTheBox>();
            var SN = 1;
            //拿到最后的一箱SN 
            if (db.ModuleInsideTheBox.Count(c => c.OrderNum == orderNumber && c.Statue == "纸箱") != 0)
            {
                SN = db.ModuleInsideTheBox.Where(c => c.OrderNum == orderNumber && c.Statue == "纸箱").Max(c => c.SN) + 1;
            }
            int i = 1;
            foreach (var item in notprintbarcode)
            {
                var sqlInnerBarcode = sy[0] + sy[1] + sy[2].PadLeft(2, '0') + "-ZX" + SN.ToString().PadLeft(4, '0');
                var sqltype = "1装10";
                tempprint.Add(item);
                if (i % 10 == 0)
                {
                    //string InnerBarcode = "2020YA36403-ZX00" + SN;
                    //sqlInnerBarcode = InnerBarcode;
                    var substring = new List<string>();
                    tempprint.ForEach(c => substring.Add(c.Substring(c.IndexOf('B'))));
                    var bm = CreateIntsideBoxLable(substring.ToArray(), orderNumber, sqlInnerBarcode, SN + "/100", "");
                    int totalbytes = bm.ToString().Length;//返回参数总共字节数
                    int rowbytes = 10; //返回参数每行的字节数
                    string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                    string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);//位图转ZPL指令
                    data += totalbytes + "," + rowbytes + "," + hex;
                    data += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
                    ZebraUnity.IPPrint(data.ToString(), 1, ip, port);

                    tempprint = new List<string>();
                }
                /*
                else if (i > 50)
                {
                    SN = 48;
                    sqlInnerBarcode = "2020YA36403-ZX0048";
                    sqltype = "1装5";
                    lastprint.Add(item);
                }
                sqlInnerBarcode = "2020YA36403-ZX00" + SN;
                if (sqltype != "1装5")
                {
                    sqltype = "1装10";
                }
                */

                //数据库添加信息
                ModuleInsideTheBox module = new ModuleInsideTheBox() { OrderNum = orderNumber, InnerBarcode = sqlInnerBarcode, ModuleBarcode = item, SN = SN, Inneror = "唐硕贵", InnerTime = DateTime.Now, Type = sqltype, Department = "总装二部", Group = "屏体包装一组", Statue = "纸箱" };
                modules.Add(module);
                if (i % 10 == 0)
                { SN++; }
                i++;

            }
            /*
            if (lastprint.Count == 5)
            {
                string InnerBarcode = "2020YA36403-ZX0048";
                var substring = new List<string>();
                lastprint.ForEach(c => substring.Add(c.Substring(c.IndexOf('B'))));
                var bm = CreateIntsideBoxLable(substring.ToArray(), orderNumber, InnerBarcode, "48/48");
                int totalbytes = bm.ToString().Length;//返回参数总共字节数
                int rowbytes = 10; //返回参数每行的字节数
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);//位图转ZPL指令
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
                //ZebraUnity.IPPrint(data.ToString(), 1, ip, port);


            }*/
            db.ModuleInsideTheBox.AddRange(modules);
            db.SaveChanges();
        }


        public ActionResult ModuleInsideBoxLablePrint(string[] barcodelist, string orderNumber, string InnerBarcode, string ip = "", int port = 0, int concentration = 5, string version = "old", string leng = "简")
        {
            var quantity = db.ModulePackageRule.Where(c => c.OrderNum == orderNumber && c.Statue == "纸箱").Select(c => c.Quantity).ToList();
            var tn = 0;
            if (quantity.Count != 0)
            {
                tn = quantity.Sum();
            }
            var sn = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == InnerBarcode).Select(c => c.SN).FirstOrDefault();
            if (version == "new")
            {
                string type = barcodelist.Count().ToString();

                Bitmap bm = CreateIntsideBoxLableNewVersion(barcodelist, orderNumber, InnerBarcode, sn + "/" + tn, leng);
                int totalbytes = bm.ToString().Length;//返回参数总共字节数
                int rowbytes = 10; //返回参数每行的字节数
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);//位图转ZPL指令
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
                var result = ZebraUnity.IPPrint(data.ToString(), 1, ip, port);

                return Content(result);
            }
            else
            {
                Bitmap bm = CreateIntsideBoxLable(barcodelist, orderNumber, InnerBarcode, sn + "/" + tn, leng);
                int totalbytes = bm.ToString().Length;//返回参数总共字节数
                int rowbytes = 10; //返回参数每行的字节数
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);//位图转ZPL指令
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
                var result = ZebraUnity.IPPrint(data.ToString(), 1, ip, port);

                return Content(result);
            }

        }

        #region--生成模块内箱标签图片
        public Bitmap CreateIntsideBoxLable(string[] mn_list, string ordernum, string innboxNUm, string sntn, string leng)
        {
            #region 60*30版本
            //int initialWidth = 500, initialHeight = 250;
            //Bitmap AllBitmap = new Bitmap(initialWidth, initialHeight);
            //Graphics theGraphics = Graphics.FromImage(AllBitmap);
            //Brush bush = new SolidBrush(System.Drawing.Color.Black);
            //theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            //Pen pen = new Pen(Color.Black, 3);//定义笔的大小
            //theGraphics.DrawRectangle(pen, 25, 16, 450, 218);  //x,y,width:绘制矩形的宽度,height：绘制的矩形的高度
            //                                                   //画横线                                                 
            //theGraphics.DrawLine(pen, 25, 50, 475, 50);//起点x,起点y坐标，终点x,终点y坐标
            //theGraphics.DrawLine(pen, 25, 83, 475, 83);
            //theGraphics.DrawLine(pen, 25, 141, 475, 141);
            ////theGraphics.DrawLine(pen, 358, 353, 700, 353);
            ////画竖线
            //theGraphics.DrawLine(pen, 267, 16, 267, 83);

            //System.Drawing.Font myFont_orderNumber1;
            //myFont_orderNumber1 = new System.Drawing.Font("Microsoft YaHei UI", 10, FontStyle.Regular);
            //theGraphics.DrawString("订单号 Order NO", myFont_orderNumber1, bush, 28, 20);//{

            //System.Drawing.Font myFont_orderNumber;
            ////字体微软雅黑，大小40，样式加粗
            //myFont_orderNumber = new System.Drawing.Font("Microsoft YaHei UI", 10, FontStyle.Bold);
            ////设置格式
            //StringFormat format = new StringFormat();
            //format.Alignment = StringAlignment.Center;
            //theGraphics.DrawString(ordernum, myFont_orderNumber, bush, 270, 20);

            ////}
            //System.Drawing.Font myFont_jianhao;
            //myFont_jianhao = new System.Drawing.Font("Microsoft YaHei UI", 10, FontStyle.Regular);
            //theGraphics.DrawString("件号/数(SN/TN)", myFont_jianhao, bush, 28, 55);
            ////引入SNTN
            ////设置格式            
            //System.Drawing.Font myFont_snt;
            //myFont_snt = new System.Drawing.Font("Microsoft YaHei UI", 10, FontStyle.Regular);
            //theGraphics.DrawString(sntn, myFont_snt, bush, 270, 55);


            ////引入条形码
            //Bitmap spc_barcode = BarCodeLablePrint.BarCodeToImg(innboxNUm, 400, 30);
            ////double beishuhege = 0.7;
            //theGraphics.DrawImage(spc_barcode, 45, 88, (float)spc_barcode.Width, (float)spc_barcode.Height);

            ////引入条码号
            //System.Drawing.Font myFont_spc_OuterBoxBarcode;
            //myFont_spc_OuterBoxBarcode = new System.Drawing.Font("Microsoft YaHei UI", 10, FontStyle.Bold);
            //theGraphics.DrawString(innboxNUm, myFont_spc_OuterBoxBarcode, bush, 190, 120);

            ////引入模组号清单
            //int mn_E_count = mn_list.Count();
            ////12位模组号以上，包括条码号
            //System.Drawing.Font myFont_modulenum_list;
            //myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 15, FontStyle.Bold);
            //StringFormat listformat = new StringFormat();
            //listformat.Alignment = StringAlignment.Near;
            //int top_y = 142;
            //int left_x = 25;
            //for (int i = 1; i < mn_list.Count() + 1; i++)
            //{
            //    theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //    if ((i % 4) != 0)
            //    {
            //        left_x += 110;
            //    }
            //    else
            //    {
            //        top_y += 20;
            //        left_x = 25;
            //    }
            //}
            //Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            //return bm;
            #endregion

            #region 备品配件大小的打印
            int initialWidth = 750, initialHeight = 583;
            Bitmap AllBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(AllBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            Pen pen = new Pen(Color.Black, 3);//定义笔的大小
            theGraphics.DrawRectangle(pen, 50, 50, 650, 483);  //x,y,width:绘制矩形的宽度,height：绘制的矩形的高度
                                                               //画横线                                                 
            theGraphics.DrawLine(pen, 50, 108, 700, 108);//起点x,起点y坐标，终点x,终点y坐标
            theGraphics.DrawLine(pen, 50, 175, 700, 175);
            theGraphics.DrawLine(pen, 50, 308, 700, 308);
            //theGraphics.DrawLine(pen, 358, 353, 700, 353);
            //画竖线
            // theGraphics.DrawLine(pen, 458, 50, 458, 175);
            System.Drawing.Font myFont_orderNumber1;
            myFont_orderNumber1 = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            System.Drawing.Font myFont_jianhao;
            myFont_jianhao = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            if (leng == "简")
            {
                theGraphics.DrawString("订单号:    " + ordernum, myFont_orderNumber1, bush, 60, 60);
                theGraphics.DrawString("件号/数(SN/TN):    " + sntn, myFont_jianhao, bush, 60, 110);
            }
            else if (leng == "英")
            {
                theGraphics.DrawString("Order No:    " + ordernum, myFont_orderNumber1, bush, 60, 60);
                theGraphics.DrawString("PACKAGE(SN/TN):    " + sntn, myFont_jianhao, bush, 60, 110);
            }
            else if (leng == "繁")
            {
                theGraphics.DrawString("訂單號:    " + ordernum, myFont_orderNumber1, bush, 60, 60);
                theGraphics.DrawString("件號/數(SN/TN):    " + sntn, myFont_jianhao, bush, 60, 110);
            }
            else if (leng == "繁/英")
            {
                theGraphics.DrawString("訂單號(Order No):   " + ordernum, myFont_orderNumber1, bush, 60, 60);
                theGraphics.DrawString("件號/數 PACKAGE(SN/TN):    " + sntn, myFont_jianhao, bush, 60, 110);
            }
            else
            {
                theGraphics.DrawString("订单号(Order No):    " + ordernum, myFont_orderNumber1, bush, 60, 60);
                theGraphics.DrawString("件号/数 PACKAGE(SN/TN):    " + sntn, myFont_jianhao, bush, 60, 110);
            }
           

            //System.Drawing.Font myFont_orderNumber;
            ////字体微软雅黑，大小40，样式加粗
            //myFont_orderNumber = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Bold);
            ////设置格式
            //StringFormat format = new StringFormat();
            //format.Alignment = StringAlignment.Center;
            //theGraphics.DrawString(ordernum, myFont_orderNumber, bush, 100, 60);

            //}
            //引入SNTN
            //设置格式            
            //System.Drawing.Font myFont_snt;
            //myFont_snt = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString(sntn, myFont_snt, bush, 100, 110);


            //引入条形码
            Bitmap spc_barcode = BarCodeLablePrint.BarCodeToImg(innboxNUm, 550, 60);
            //double beishuhege = 0.7;
            theGraphics.DrawImage(spc_barcode, 100, 180, (float)spc_barcode.Width, (float)spc_barcode.Height);

            //引入条码号
            System.Drawing.Font myFont_spc_OuterBoxBarcode;
            myFont_spc_OuterBoxBarcode = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Bold);
            theGraphics.DrawString(innboxNUm, myFont_spc_OuterBoxBarcode, bush, 250, 250);

            //引入模组号清单
            int mn_E_count = mn_list.Count();
            //20位模组号以上，包括条码号
            if (mn_list.Count() <= 20)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 28, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 310;
                int left_x = 60;
                for (int i = 1; i < mn_list.Count() + 1; i++)
                {
                    theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 4) != 0)
                    {
                        left_x += 155;
                    }
                    else
                    {
                        top_y += 50;
                        left_x = 60;
                    }
                }
            }
            else
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 310;
                int left_x = 60;
                for (int i = 1; i < mn_list.Count() + 1; i++)
                {
                    theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 5) != 0)
                    {
                        left_x += 124;
                    }
                    else
                    {
                        top_y += 30;
                        left_x = 60;
                    }
                }
            }
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
            #endregion
        }
        #endregion

        #region--生成模块新版内箱标签图片
        public Bitmap CreateIntsideBoxLableNewVersion(string[] mn_list, string ordernum, string innboxNUm, string sntn, string leng)
        {
            int initialWidth = 750, initialHeight = 1000;
            Bitmap AllBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(AllBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            Pen pen = new Pen(Color.Black, 3);//定义笔的大小
            theGraphics.DrawRectangle(pen, 50, 50, 650, 900);  //x,y,width:绘制矩形的宽度,height：绘制的矩形的高度
                                                               //画横线                                                 
            theGraphics.DrawLine(pen, 50, 108, 700, 108);//起点x,起点y坐标，终点x,终点y坐标
            theGraphics.DrawLine(pen, 50, 175, 700, 175);
            theGraphics.DrawLine(pen, 50, 242, 700, 242);
            theGraphics.DrawLine(pen, 50, 308, 700, 308);
            theGraphics.DrawLine(pen, 50, 375, 700, 375);
            theGraphics.DrawLine(pen, 50, 441, 700, 441);
            theGraphics.DrawLine(pen, 50, 570, 700, 570);
            //theGraphics.DrawLine(pen, 358, 353, 700, 353);
            //画竖线
            theGraphics.DrawLine(pen, 300, 50, 300, 375);
            if (leng == "简")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("订单号", myFont, bush, 60, 60);

                theGraphics.DrawString("规格型号", myFont, bush, 60, 120);

                theGraphics.DrawString("颜色", myFont, bush, 60, 190);

                theGraphics.DrawString("数量", myFont, bush, 60, 255);

                theGraphics.DrawString("件号", myFont, bush, 60, 320);
            }
            else if (leng == "简/英" || leng == "")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("订单号 PO#", myFont, bush, 60, 60);

                theGraphics.DrawString("规格型号 ITEM NO ", myFont, bush, 60, 120);

                theGraphics.DrawString("颜色 COLOURS", myFont, bush, 60, 190);

                theGraphics.DrawString("数量 QTY(PSC)", myFont, bush, 60, 255);

                theGraphics.DrawString("件号 PACKAGE#", myFont, bush, 60, 320);
            }
            else if (leng == "繁")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("訂單號", myFont, bush, 60, 60);

                theGraphics.DrawString("規格型號", myFont, bush, 60, 120);

                theGraphics.DrawString("顏色", myFont, bush, 60, 190);

                theGraphics.DrawString("數量", myFont, bush, 60, 255);

                theGraphics.DrawString("件號", myFont, bush, 60, 320);
            }
            else if (leng == "繁/英")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("訂單號 PO#", myFont, bush, 60, 60);

                theGraphics.DrawString("規格型號 ITEM NO", myFont, bush, 60, 120);

                theGraphics.DrawString("顏色 COLOURS", myFont, bush, 60, 190);

                theGraphics.DrawString("數量 QTY(PSC)", myFont, bush, 60, 255);

                theGraphics.DrawString("件號 PACKAGE#", myFont, bush, 60, 320);
            }
            else if (leng == "英")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("PO#", myFont, bush, 60, 60);

                theGraphics.DrawString("ITEM NO", myFont, bush, 60, 120);

                theGraphics.DrawString("COLOURS", myFont, bush, 60, 190);

                theGraphics.DrawString("QTY(PSC)", myFont, bush, 60, 255);

                theGraphics.DrawString("PACKAGE#", myFont, bush, 60, 320);
            }
            //订单号
            System.Drawing.Font value;
            value = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(ordernum, value, bush, 380, 60);
            //规格型号
            var info = db.ModulePackageRule.Where(c => c.OrderNum == ordernum).FirstOrDefault();
            theGraphics.DrawString(info.ITEMNO, value, bush, 380, 120);
            //颜色
            theGraphics.DrawString(info.COLOURS, value, bush, 380, 190);
            //数量
            string boxnum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "纸箱").Sum(c => c.Quantity * c.OuterBoxCapacity).ToString();
            int mn_E_count = mn_list.Count();
            theGraphics.DrawString(mn_E_count + "/" + boxnum, value, bush, 380, 255);
            //件号
            string[] s = sntn.Split('/');
            theGraphics.DrawString(s[0] + " OF " + s[1], value, bush, 380, 320);
            //备注
            theGraphics.DrawString(info.Remark, value, bush, 215, 388);
            //引入条形码
            Bitmap spc_barcode = BarCodeLablePrint.BarCodeToImg(innboxNUm, 550, 60);
            //double beishuhege = 0.7;
            theGraphics.DrawImage(spc_barcode, 100, 450, (float)spc_barcode.Width, (float)spc_barcode.Height);

            //引入条码号
            System.Drawing.Font myFont_spc_OuterBoxBarcode;
            myFont_spc_OuterBoxBarcode = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Bold);
            theGraphics.DrawString(innboxNUm, myFont_spc_OuterBoxBarcode, bush, 230, 520);

            //引入模组号清单

            //20位模组号以上，包括条码号
            if (mn_list.Count() <= 20)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 28, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 580;
                int left_x = 60;
                for (int i = 1; i < mn_list.Count() + 1; i++)
                {
                    theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 4) != 0)
                    {
                        left_x += 155;
                    }
                    else
                    {
                        top_y += 50;
                        left_x = 60;
                    }
                }
            }
            else
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 580;
                int left_x = 60;
                for (int i = 1; i < mn_list.Count() + 1; i++)
                {
                    theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 5) != 0)
                    {
                        left_x += 124;
                    }
                    else
                    {
                        top_y += 30;
                        left_x = 60;
                    }
                }
            }
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
        }
        #endregion

        #region ---查看内箱标签
        public ActionResult InsideBoxLablePrintToImg(string outsidebarcode, string statue, string version = "old", string leng = "简/英")
        {
            var outsidebarcode_recordlist = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == outsidebarcode);
            var ordernum = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == outsidebarcode).Select(c => c.OrderNum).FirstOrDefault();//订单和挪用订单
                                                                                                                                       //string ordernum = string.IsNullOrEmpty(ordernum1.EmbezzleOrdeNum) ? ordernum1.OrderNum : ordernum1.EmbezzleOrdeNum;//如果挪用订单不为空，则显示挪用订单，否则显示本来订单号
                                                                                                                                       //string type = outsidebarcode_recordlist.FirstOrDefault().Type;
            var sn = outsidebarcode_recordlist.FirstOrDefault().SN;
            var tn = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).Sum(c => c.Quantity);

            var mn_list = outsidebarcode_recordlist.Select(c => c.ModuleBarcode).ToList();
            List<string> barcodelsit = new List<string>();
            //var ss = mn_list[1].Substring(mn_list[1].IndexOf('B')).ToString();
            mn_list.ForEach(c => barcodelsit.Add(c.Substring(c.IndexOf('B')).ToString()));
            if (version == "new")
            {
                var boxnum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).Sum(c => c.Quantity * c.OuterBoxCapacity);
                var AllBitmap = CreateIntsideBoxLableNewVersion(barcodelsit.ToArray(), ordernum, outsidebarcode, sn + "/" + tn, leng);
                MemoryStream ms = new MemoryStream();
                AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                AllBitmap.Dispose();
                return File(ms.ToArray(), "image/Png");
            }
            else
            {
                var AllBitmap = CreateIntsideBoxLable(barcodelsit.ToArray(), ordernum, outsidebarcode, sn + "/" + tn, leng);
                MemoryStream ms = new MemoryStream();
                AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                AllBitmap.Dispose();
                return File(ms.ToArray(), "image/Png");
            }
        }

        #endregion

        #region---内箱重复打印
        public ActionResult InsideBoxLablePrintAgain(string outsidebarcode, string statue, int pagecount = 1, int concentration = 5, string ip = "", int port = 0, string leng = "", string version = "old")
        {
            var outsidebarcode_recordlist = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == outsidebarcode);
            var ordernum = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == outsidebarcode).Select(c => c.OrderNum).FirstOrDefault();//订单和挪用订单

            var sn = outsidebarcode_recordlist.FirstOrDefault().SN;
            var tn = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).Sum(c => c.Quantity);


            var mn_list = outsidebarcode_recordlist.Select(c => c.ModuleBarcode).ToList();
            List<string> barcodelsit = new List<string>();
            //var ss = mn_list[1].Substring(mn_list[1].IndexOf('B')).ToString();
            mn_list.ForEach(c => barcodelsit.Add(c.Substring(c.IndexOf('B')).ToString()));

            //如果有包装新订单号，则使用包装新订单号。
            if (version == "new")
            {
                var type = outsidebarcode_recordlist.FirstOrDefault().Type.Substring(2);
                var boxnum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).Sum(c => c.Quantity * c.OuterBoxCapacity);
                var bm = CreateIntsideBoxLableNewVersion(barcodelsit.ToArray(), ordernum, outsidebarcode, sn + "/" + tn, leng);
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                return Content(result);

            }
            else
            {
                var bm = CreateIntsideBoxLable(barcodelsit.ToArray(), ordernum, outsidebarcode, sn + "/" + tn, leng);
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                return Content(result);
            }

        }
        #endregion

        #endregion

        #region---模块条码打印---30*40
        public ActionResult ModuleBarcodePrinting1(string[] modulenum, int pagecount, string barcode = "", string ip = "", int port = 0, int concentration = 5, bool testswitch = false)
        {
            //开始绘制图片
            int initialWidth = 520, initialHeight = 250;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));

            Pen pen = new Pen(Color.Black, 1);
            theGraphics.DrawRectangle(pen, 32, 8, 475, 238); //460矩形的宽度 220矩形的高度
                                                             //横线            
            theGraphics.DrawLine(pen, 32, 40, 502, 40);
            theGraphics.DrawLine(pen, 32, 40, 502, 40);
            theGraphics.DrawLine(pen, 32, 72, 502, 72);
            theGraphics.DrawLine(pen, 32, 104, 502, 104);
            theGraphics.DrawLine(pen, 32, 136, 502, 136);
            theGraphics.DrawLine(pen, 32, 168, 502, 168);
            theGraphics.DrawLine(pen, 32, 200, 502, 200);
            //竖线
            theGraphics.DrawLine(pen, 260, 0, 260, 235);

            //引入条码
            int cout = 0;
            int left_x = 10;
            int top_y = 50;
            int left_xx = 23;
            int top_yy = 140;
            foreach (var item in modulenum)
            {
                if (cout <= 5)
                {
                    //条形码
                    StringFormat geshi = new StringFormat();
                    geshi.Alignment = StringAlignment.Center; //居中
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 200, 16);
                    theGraphics.DrawImage(bmp_barcode, top_y, left_x, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 8, FontStyle.Regular);
                    StringFormat geshi1 = new StringFormat();
                    geshi1.Alignment = StringAlignment.Center; //居中
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, top_yy, left_xx, geshi);
                    cout++;
                    left_x += 32;
                    left_xx += 32;
                }
                else if (cout >= 6 && cout <= 11)
                {
                    if (cout == 6)
                    {
                        left_x = 10;
                        left_xx = 23;
                        top_y = 290;
                        top_yy = 385;
                    }
                    //条形码
                    StringFormat geshi = new StringFormat();
                    geshi.Alignment = StringAlignment.Center; //居中
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 200, 16);
                    theGraphics.DrawImage(bmp_barcode, top_y, left_x, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 8, FontStyle.Regular);
                    StringFormat geshi1 = new StringFormat();
                    geshi1.Alignment = StringAlignment.Center; //居中
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, top_yy, left_xx, geshi);
                    cout++;
                    if (cout > 6)
                    {
                        left_x += 32;
                        left_xx += 32;
                    }
                }
            }

            string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值

            if (testswitch == true)
            {
                MemoryStream ms = new MemoryStream();
                theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                theBitmap.Dispose();
                return File(ms.ToArray(), "image/Png");
            }
            else
            {
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO150,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                return Content(result);
            }
        }
        #endregion

        #region---模块条码打印
        public ActionResult ModuleBarcodePrinting(string[] modulenum, int pagecount, string barcode = "", string ip = "", int port = 0, int concentration = 5, bool testswitch = false)
        {
            //开始绘制图片
            int initialWidth = 750, initialHeight = 1000;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //Brush bush1 = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(255, 255, 255));

            Pen pen = new Pen(Color.Black, 3);
            theGraphics.DrawRectangle(pen, 5, 40, 675, 895); //460矩形的宽度 220矩形的高度

            ////横线 
            int x_axis = 5;
            int y_axis = 72;
            int high = 678;
            for (var i = 0; i < 27; i++)
            {
                theGraphics.DrawLine(pen, x_axis, y_axis, high, y_axis);
                y_axis += 32;
            }

            ////竖线
            theGraphics.DrawLine(pen, 229, 40, 229, 935);
            theGraphics.DrawLine(pen, 453, 40, 453, 935);

            //引入条码
            int cout = 0;
            int left_x = 44;
            int top_y = 19;
            int left_xx = 57;
            int top_yy = 120;
            foreach (var item in modulenum)
            {
                if (cout <= 27)
                {
                    //条形码
                    StringFormat geshi = new StringFormat();
                    geshi.Alignment = StringAlignment.Center; //居中
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 200, 16);
                    theGraphics.DrawImage(bmp_barcode, top_y, left_x, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 8, FontStyle.Regular);
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, top_yy, left_xx, geshi);
                    cout++;
                    left_x += 32;
                    left_xx += 32;
                }
                else if (cout >= 28 && cout <= 55)
                {
                    if (cout == 28)
                    {
                        left_x = 44;
                        top_y = 245;

                        left_xx = 57;
                        top_yy = 350;
                    }
                    //条形码
                    StringFormat geshi = new StringFormat();
                    geshi.Alignment = StringAlignment.Center; //居中
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 200, 16);
                    theGraphics.DrawImage(bmp_barcode, top_y, left_x, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 8, FontStyle.Regular);
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, top_yy, left_xx, geshi);
                    cout++;
                    if (cout > 28 && cout <= 55)
                    {
                        left_x += 32;
                        left_xx += 32;
                    }
                }
                else if (cout >= 56 && cout <= 83)
                {
                    if (cout == 56)
                    {
                        left_x = 44;
                        top_y = 471;

                        left_xx = 57;
                        top_yy = 580;
                    }
                    //条形码
                    StringFormat geshi = new StringFormat();
                    geshi.Alignment = StringAlignment.Center; //居中
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 200, 16);
                    theGraphics.DrawImage(bmp_barcode, top_y, left_x, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 8, FontStyle.Regular);
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, top_yy, left_xx, geshi);
                    cout++;
                    if (cout > 56 && cout <= 83)
                    {
                        left_x += 32;
                        left_xx += 32;
                    }

                }
            }

            string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值

            if (testswitch == true)
            {
                MemoryStream ms = new MemoryStream();
                theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                theBitmap.Dispose();
                return File(ms.ToArray(), "image/Png");
            }
            else
            {
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO70,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                return Content(result);
            }
        }

        public ActionResult ModuleBarcodePrinting2(string[] modulenum, int pagecount, string barcode = "", string ip = "", int port = 0, int concentration = 5, bool testswitch = false)
        {
            string[] modulelist = new string[6];
            modulelist[0] = "2019YA00101B00001";
            modulelist[1] = "2019YA00101B00002";
            modulelist[2] = "2019YA00101B00003";
            //modulelist[3] = "2019YA00101B00004";
            //modulelist[4] = "2019YA00101B00005";
            //modulelist[5] = "2019YA00101B00006";

            //开始绘制图片
            int initialWidth = 716, initialHeight = 33;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //Brush bush1 = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(255, 255, 255));

            Pen pen = new Pen(Color.Black, 1);
            //theGraphics.DrawRectangle(pen, 0, 0, 700, 33); //460矩形的宽度 220矩形的高度
            ////横线 
            //theGraphics.DrawLine(pen, 0, 30, 932, 30);
            ////竖线
            //theGraphics.DrawLine(pen, 229, 0, 229, 28);
            //theGraphics.DrawLine(pen, 453, 0, 453, 28);

            //引入条码
            int default_left_y = 0;
            int default_top_x = 5;
            double default_left_yy = 9;
            int default_top_xx = 90;
            //int left_y = 0;
            int top_x = 5;
            //int left_yy = 13;
            int top_xx = 90;
            StringFormat geshi = new StringFormat();
            geshi.Alignment = StringAlignment.Center; //居中
            System.Drawing.Font myFont_modulebarcodenum;
            myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 10, FontStyle.Bold);
            for (var i = 1; i < modulenum.Count() + 1; i++)
            {
                if ((i % 3) == 0)
                {
                    //条形码
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(modulenum[i - 1], 200, 14);
                    theGraphics.DrawImage(bmp_barcode, top_x, default_left_y, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    theGraphics.DrawString(modulenum[i - 1], myFont_modulebarcodenum, bush, top_xx, (float)default_left_yy, geshi);
                    top_x = default_top_x;
                    top_xx = default_top_xx;
                }
                else
                {
                    //条形码
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(modulenum[i - 1], 200, 14);
                    theGraphics.DrawImage(bmp_barcode, top_x, default_left_y, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    theGraphics.DrawString(modulenum[i - 1], myFont_modulebarcodenum, bush, top_xx, (float)default_left_yy, geshi);
                    top_x += 228;
                    top_xx += 239;
                }
            }

            string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值

            if (testswitch == true)
            {
                MemoryStream ms = new MemoryStream();
                theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                theBitmap.Dispose();
                return File(ms.ToArray(), "image/Png");
            }
            else
            {
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO60,0^XGR:ZONE.GRF^FS^XZ";

                ////指令输出打印
                //concentration = 10;
                //data = "^XA^MD" + concentration + "^LH0,0.5^FO58,5^BCN,14,N,N^BY1,1,14^FD2019YA00101B00001^FS";//第一个条码
                //data += "^FO290,5^BCN,14,N,N^BY1,1,14^FD2019YA00101B00002^FS";//第二个条码
                //data += "^FO523,5^BCN,14,N,N^BY1,1,14^FD" + modulelist[2] + "^FS";// +^FSXZ第三个条码
                //data += "^FO90,20^A@N,9,5^FD2019YA00101B00001^FS";//第一个条码的字符数字
                //data += "^FO330,20^A@N,9,5^FD2019YA00101B00002^FS";//第二个条码的字符数字
                //data += "^FO570,20^A@N,9,5^FD" + modulelist[2] + "^FS^XZ";//第三个条码的字符数字


                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                return Content(result);

                /*条码打印命令说明
                ^XA //条码打印指令开始
                ^MD30 //设置色带颜色的深度, 取值范围从-30到30
                ^LH60,10 //设置条码纸的边距
                ^FO20,10 //设置条码左上角的位置
                ^ACN,18,10 //设置字体
                ^BY1.4,3,50 //设置条码样式。1.4是条码的缩放级别，3是条码中粗细柱的比例, 50是条码高度
                ^BC,,Y,N //打印code128的指令
                ^FD12345678^FS //设置要打印的内容, ^FD是要打印的条码内容^FS表示换行
                ^XZ //条码打印指令结束
                */
                //上面的指令会打印12345678的CODE128的条码
            }
        }

        #endregion

    }

    public class ModuleManagement_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController com = new CommonController();
        #region 临时类
        private class TempIndex
        {
            public string Ordernum { get; set; }
            public string Machine { get; set; }
            public string Barcode { get; set; }
            public bool IsSampling { get; set; }
            public bool FinshResult { get; set; }
            public string Seaction { get; set; }
            public DateTime? EndTime { get; set; }

        }

        public class CheckList
        {
            public string Barcode { get; set; }
            public bool Finshi { get; set; }
        }
        #endregion

        #region------模块看板


        [HttpPost]
        [ApiAuthorize]
        public JObject Index([System.Web.Http.FromBody]JObject data)
        {
            /* 1.如果有选择订单号,则根据选择的订单号查询,没有则查询有后焊记录的所有订单信息
             * 2.完成率=完成数量/订单定义模块数
             * 3.抽检率=抽检数量/订单定义模块数
             * 4.直通率=(完成条码去掉有过异常记录的条码)/订单定义模块数
             */
            var orderunm = data["orderunm"].ToList();//订单号列表
            JArray total = new JArray();
            var totalOrderdum = db.OrderMgm.Where(c => c.Models != 0).ToList();
            if (orderunm.Count != 0)
            {
                totalOrderdum = totalOrderdum.Where(c => orderunm.Contains(c.OrderNum)).ToList();
            }
            else
            {
                var orderlist = db.AfterWelding.Select(c => c.Ordernum).Distinct().ToList();

                totalOrderdum = db.OrderMgm.Where(c => orderlist.Contains(c.OrderNum)).ToList();

            }
            var ai = db.ModuleAI.Select(c => new TempIndex { Ordernum = c.Ordernum, Machine = c.Machine });
            var after = db.AfterWelding.Select(c => new TempIndex { Ordernum = c.Ordernum, FinshResult = c.IsAbnormal });
            var SamplingList = db.ModuleSampling.Select(c => new TempIndex { Ordernum = c.Ordernum, Seaction = c.Section, FinshResult = c.SamplingResult });
            var electric = db.ElectricInspection.Select(c => new TempIndex { Ordernum = c.Ordernum, Seaction = c.Section, FinshResult = c.ElectricInspectionResult, Barcode = c.ModuleBarcode });
            var burn = db.ModuleBurnIn.Select(c => new TempIndex { Ordernum = c.Ordernum, FinshResult = c.BurninResult, Barcode = c.ModuleBarcode, EndTime = c.BurnInEndTime });
            foreach (var item in totalOrderdum)
            {
                JObject result = new JObject();
                //订单号
                result.Add("ordernum", item.OrderNum);
                //平台型号
                result.Add("type", item.PlatformType);
                //制程要求
                result.Add("processingRequire", item.ProcessingRequire);
                //标准要求
                result.Add("standardRequire", item.StandardRequire);
                //模块数
                result.Add("moduleNum", item.Models);
                //AI机台完成数量
                JArray aistring = new JArray();
                var moduleai = ai.Where(c => c.Ordernum == item.OrderNum).Select(c => c.Machine).Distinct().ToList();
                moduleai.ForEach(c =>
                {
                    var count = ai.Count(a => a.Ordernum == item.OrderNum && a.Machine == c);
                    aistring.Add(c + ":" + Math.Round((decimal)count / item.Models, 2) + "%(" + count + "/" + item.Models + ")");
                });
                result.Add("aiCount", JsonConvert.SerializeObject(aistring));
                //后焊完成率
                var aftercount = after.Count(c => c.Ordernum == item.OrderNum);
                result.Add("afterWeldingPass", Math.Round((decimal)aftercount * 100 / item.Models, 2) + "%(" + aftercount + "/" + item.Models + ")");
                //后焊抽检率
                var Sampling = SamplingList.Count(c => c.Ordernum == item.OrderNum && c.Seaction == "后焊");
                result.Add("samplingCount", Math.Round((decimal)Sampling * 100 / item.Models, 2) + "%(" + Sampling + "/" + item.Models + ")");
                //后焊抽检合格率
                //var samplingab=after.Count(c => c.Ordernum == item.OrderNum && c.IsSampling == true);
                //result.Add("samplingPass", Sampling == 0 ? "0" :Math.Round((decimal)samplingab * 100 / Sampling, 2) + "%(" + samplingab + "/" + Sampling + ")");

                //后焊产线完成情况
                JArray array = new JArray();
                result.Add("afterLine", array);


                //灌胶电测完成率
                var gulecount = electric.Where(c => c.Ordernum == item.OrderNum && c.Seaction == "灌胶前电检" && c.FinshResult == true).Select(c => c.Barcode).ToList();
                result.Add("gulePass", Math.Round((decimal)gulecount.Count * 100 / item.Models, 2) + "%(" + gulecount.Count + "/" + item.Models + ")");
                //灌胶电测直通率
                var abnonal = electric.Where(c => c.Ordernum == item.OrderNum && c.Seaction == "灌胶前电检" && c.FinshResult == false).Select(c => c.Barcode).ToList();
                var trrought = gulecount.Except(abnonal).ToList().Count;
                result.Add("gulePassThrough", Math.Round((decimal)trrought * 100 / item.Models, 2) + "%(" + trrought + "/" + item.Models + ")");


                //面罩电测完成率
                var maskcount = electric.Where(c => c.Ordernum == item.OrderNum && c.Seaction == "模块电检" && c.FinshResult == true).Select(c => c.Barcode).ToList(); ;
                result.Add("maskPass", Math.Round((decimal)maskcount.Count * 100 / item.Models, 2) + "%(" + maskcount.Count + "/" + item.Models + ")");
                //面罩电测直通率
                var maskabnonal = electric.Where(c => c.Ordernum == item.OrderNum && c.Seaction == "模块电检" && c.FinshResult == false).Select(c => c.Barcode).ToList();
                var maskthrought = maskcount.Except(maskabnonal).ToList().Count;
                result.Add("maskPassThrough", Math.Round((decimal)maskthrought * 100 / item.Models, 2) + "%(" + maskthrought + "/" + item.Models + ")");



                //电测后抽检率
                var electricalSampling = SamplingList.Count(c => c.Ordernum == item.OrderNum && c.Seaction == "模块电检");
                result.Add("electricalSamplingCount", Math.Round((decimal)electricalSampling * 100 / item.Models, 2) + "%(" + electricalSampling + "/" + item.Models + ")");
                //电测后抽检合格率
                var electricalSamplingPass = SamplingList.Count(c => c.Ordernum == item.OrderNum && c.Seaction == "模块电检" && c.FinshResult == true);
                result.Add("electricalSamplingPass", electricalSampling == 0 ? "0%" : Math.Round((decimal)electricalSamplingPass * 100 / electricalSampling, 2) + "%(" + electricalSamplingPass + "/" + electricalSampling + ")");


                //老化完成率
                var burninfins = burn.Where(c => c.Ordernum == item.OrderNum && c.EndTime != null).Select(c => c.Barcode).Count();
                var burnin = burn.Where(c => c.Ordernum == item.OrderNum && c.EndTime == null).Select(c => c.Barcode).Count();
                result.Add("burnPass", Math.Round((decimal)burninfins * 100 / item.Models, 2) + "%(" + burninfins + "/" + item.Models + ") " + burnin + "个进行中");
                //老化直通率
                var burninabnonal = burn.Count(c => c.Ordernum == item.OrderNum && c.FinshResult == true && c.EndTime != null);
                result.Add("burnPassThrough", Math.Round((decimal)burninabnonal * 100 / item.Models, 2) + "%(" + burninabnonal + "/" + item.Models + ")");


                //外观完成率
                var appearancecount = electric.Where(c => c.Ordernum == item.OrderNum && c.Seaction == "外观电检" && c.FinshResult == true).Select(c => c.Barcode).ToList();
                result.Add("appearancePass", Math.Round((decimal)appearancecount.Count * 100 / item.Models, 2) + "%(" + appearancecount.Count + "/" + item.Models + ")");
                //外观直通率
                var appearanceabnonal = electric.Where(c => c.Ordernum == item.OrderNum && c.Seaction == "外观电检" && c.FinshResult == false).Select(c => c.Barcode).ToList();
                var appearancethrought = appearancecount.Except(appearanceabnonal).ToList().Count;
                result.Add("appearancePassThrough", Math.Round((decimal)appearancethrought * 100 / item.Models, 2) + "%(" + appearancethrought + "/" + item.Models + ")");


                //内箱包装数量
                var innside = db.ModuleInsideTheBox.Where(c => c.OrderNum == item.OrderNum).Select(c => new { c.ModuleBarcode, c.InnerBarcode }).ToList();
                var innsiderule = db.ModulePackageRule.Where(c => c.OrderNum == item.OrderNum && c.Statue == "纸箱").Select(c => new { c.Quantity, c.OuterBoxCapacity }).ToList();
                if (innsiderule.Count != 0)
                {
                    var totalpack = innsiderule.Sum(c => c.Quantity);
                    var onenum = innsiderule.Sum(c => c.OuterBoxCapacity * c.Quantity);
                    result.Add("innerPackCount", innside.Select(c => c.InnerBarcode).Distinct().Count() + "/" + totalpack + "箱(" + innside.Select(c => c.ModuleBarcode).Distinct().Count() + "/" + onenum + "件)");
                }
                else
                {
                    result.Add("innerPackCount", "0/0箱");
                }
                //外箱包装数量
                var outside = db.ModuleOutsideTheBox.Where(c => c.OrderNum == item.OrderNum).Select(c => new { c.OutsideBarcode, c.InnerBarcode }).ToList();
                var outsiderule = db.ModulePackageRule.Where(c => c.OrderNum == item.OrderNum && c.Statue == "外箱").Select(c => new { c.Quantity, c.OuterBoxCapacity }).ToList();
                if (outsiderule.Count != 0)
                {
                    var totalpack = outsiderule.Sum(c => c.Quantity);
                    var onenum = outsiderule.Sum(c => c.OuterBoxCapacity);
                    result.Add("outsidepackCount", outside.Select(c => c.OutsideBarcode).Distinct().Count() + "/" + totalpack + "箱(" + outside.Select(c => c.InnerBarcode).Distinct().Count() + "/" + totalpack * onenum + "件)");
                }
                else
                {
                    result.Add("outsidepackCount", "0/0箱");
                }
                //出入库情况
                var warehouse = db.Warehouse_Join.Count(c => c.OrderNum == item.OrderNum && c.State == "模块");
                if (warehouse == 0)
                {
                    result.Add("warehousr", "入库/出库(0/0)");
                }
                else
                {
                    var warehouseout = db.Warehouse_Join.Count(c => c.OrderNum == item.OrderNum && c.State == "模块" && c.IsOut == true);
                    result.Add("warehousr", "入库/出库(" + warehouse + "/" + warehouseout + ")");
                }
                total.Add(result);
            }
            return com.GetModuleFromJarray(total);
        }




        //模块实时看板历史
        [HttpPost]
        [ApiAuthorize]
        public JObject ModuleHistoryBoard([System.Web.Http.FromBody]JObject data)
        {
            /* 1.根据选择的订单号查询,
             * 2.完成率=完成数量/订单定义模块数
             * 3.抽检率=抽检数量/订单定义模块数
             * 4.直通率=(完成条码去掉有过异常记录的条码)/订单定义模块数
             */
            JArray ordernum = (JArray)data["ordernum"];//订单号
            JArray ProductionControlIndex = new JArray();   //创建JSON对象

            var time = DateTime.Now;
            int span = 20;
            int i = 1;
            JArray array = new JArray();

            foreach (var token in ordernum)
            {
                var item = token.ToString();
                var board = db.ModuleBoard.Where(c => c.IsComplete == false && c.Ordernum == item).ToList();
                var order = db.OrderMgm.Where(c => c.OrderNum == item).FirstOrDefault();
                var OrderNum = new JObject();
                //完成时间
                var warehouseccount = db.Warehouse_Join.Where(c => c.OrderNum == item && c.State == "模块" && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();
                var outsiderule = db.ModulePackageRule.Count(c => c.OrderNum == item && c.Statue == "外箱") == 0 ? 0 : db.ModulePackageRule.Where(c => c.OrderNum == item && c.Statue == "外箱").Sum(c => c.Quantity);
                if (warehouseccount != 0 && warehouseccount >= outsiderule)//判断时候出库完成
                {
                    var maxtime = db.Warehouse_Join.Where(c => c.OrderNum == item && c.State == "模块" && c.IsOut == true).Max(c => c.WarehouseOutDate);

                    OrderNum.Add("CompleteTime", maxtime);
                    OrderNum.Add("Timespan", maxtime - order.PlanInputTime);
                }
                else
                {
                    OrderNum.Add("CompleteTime", null);
                    OrderNum.Add("Timespan", null);
                }

                OrderNum.Add("Id", order.ID);
                OrderNum.Add("OrderNum", order.OrderNum);
                OrderNum.Add("PlatformType", order.PlatformType);
                OrderNum.Add("Models", order.Models);
                OrderNum.Add("PlanInputTime", order.PlanInputTime);

                #region SMT
                #region---------------------SMT首样部分
                var SMTFirstSample_Description = order.SMTFirstSample_Description;

                OrderNum.Add("SMTFirstSample_Description", SMTFirstSample_Description);
                if (!string.IsNullOrEmpty(SMTFirstSample_Description))
                {
                    if (!comm.CheckJpgExit(item, "SMTSample_Files"))
                        OrderNum.Add("SMTFirstSample_DescriptionJpg", "true");
                    else
                        OrderNum.Add("SMTFirstSample_DescriptionJpg", "false");

                    if (!comm.CheckpdfExit(item, "SMTSample_Files"))
                        OrderNum.Add("SMTFirstSample_DescriptionPdf", "true");
                    else
                        OrderNum.Add("SMTFirstSample_DescriptionPdf", "false");
                }
                #endregion

                #region------------------------- SMT首样
                // 小样进度
                OrderNum.Add("HandSampleScedule", order.HandSampleScedule);
                //是否有图片
                if (comm.CheckJpgExit(item, "SmallSample_Files"))
                    OrderNum.Add("HandSampleSceduleJpg", "false");
                else
                    OrderNum.Add("HandSampleSceduleJpg", "true");
                //是否有PDF文件
                if (comm.CheckpdfExit(item, "SmallSample_Files"))
                    OrderNum.Add("HandSampleScedulePdf", "false");
                else
                    OrderNum.Add("HandSampleScedulePdf", "true");

                //是否有小样报告
                var sample = db.Small_Sample.OrderBy(c => c.Id).Where(c => (c.OrderNumber == item || c.OrderNumber.Contains(item)) && c.Approved != null && c.ApprovedResult == true).ToList();
                if (sample.Count != 0)
                {
                    JArray sampleJarray = new JArray();
                    int k = 1;
                    foreach (var sampleitem in sample)
                    {
                        JObject sampleJobject = new JObject();
                        sampleJobject.Add("id", sampleitem.Id);
                        sampleJobject.Add("Name", "NO." + k);
                        k++;
                        sampleJarray.Add(sampleJobject);
                    }
                    OrderNum.Add("HandSampleSceduleReport", sampleJarray);
                }
                else
                    OrderNum.Add("HandSampleSceduleReport", "false");
                #endregion

                #region------------------------- SMT异常
                var SMTAbnormal_Description = order.SMTAbnormal_Description;
                OrderNum.Add("SMTAbnormal_Description", SMTAbnormal_Description);
                if (!string.IsNullOrEmpty(SMTAbnormal_Description))
                {
                    if (!comm.CheckJpgExit(item, "SMTAbnormalOrder_Files"))
                        OrderNum.Add("SMTAbnormal_DescriptionJpg", "true");
                    else
                        OrderNum.Add("SMTAbnormal_DescriptionJpg", "false");

                    if (!comm.CheckpdfExit(item, "SMTAbnormalOrder_Files"))
                        OrderNum.Add("SMTAbnormal_DescriptionPdf", "true");
                    else
                        OrderNum.Add("SMTAbnormal_DescriptionPdf", "false");
                }
                #endregion

                var ModelNum = 0;
                var NormalNumSum = 0;
                var AbnormalNumSum = 0;
                var jobcontenlist = db.SMT_ProductionData.Where(c => c.OrderNum == item).Select(c => c.JobContent).Distinct().ToList();
                JArray SmtArray = new JArray();
                foreach (var jobconten in jobcontenlist)
                {
                    JObject FinishRateitem = new JObject();
                    if (jobconten == "灯面" || jobconten == "IC面")
                    {
                        ModelNum = order.Models;
                    }
                    else if (jobconten.Contains("转接卡") == true)
                    {
                        ModelNum = order.AdapterCard;
                    }
                    else if (jobconten.Contains("电源") == true)
                    {
                        ModelNum = order.Powers;
                    }
                    //对应工作内容良品总数
                    if (db.SMT_ProductionData.Count(c => c.OrderNum == item && c.JobContent == jobconten) == 0)
                    {
                        AbnormalNumSum = 0;
                        NormalNumSum = 0;
                    }
                    else
                    {
                        NormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item && c.JobContent == jobconten).Sum(c => c.NormalCount);

                        //对应工作内容不良品总数
                        AbnormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item && c.JobContent == jobconten).Sum(c => c.AbnormalCount);
                    }
                    // 面
                    FinishRateitem.Add("Line", jobconten);
                    // 总完成率
                    FinishRateitem.Add("CompleteRate", ModelNum == 0 ? "" : (((decimal)(NormalNumSum + AbnormalNumSum)) / ModelNum * 100).ToString("F2") + "%");
                    // 总完成率分母
                    FinishRateitem.Add("CompleteInfo", (NormalNumSum + AbnormalNumSum) + "/" + ModelNum);
                    // 总合格率
                    FinishRateitem.Add("PassRate", (AbnormalNumSum + NormalNumSum) == 0 ? "" : ((decimal)NormalNumSum / (NormalNumSum + AbnormalNumSum) * 100).ToString("F2") + "%");
                    //总合格率分子
                    FinishRateitem.Add("PassInfo", NormalNumSum + "/" + (NormalNumSum + AbnormalNumSum));
                    FinishRateitem.Add("SamplingRate", null);
                    FinishRateitem.Add("SamplingInfo", null);

                    SmtArray.Add(FinishRateitem);


                }
                OrderNum.Add("SmtArray", SmtArray);
                #endregion

                #region AI
                JArray itemArray = new JArray();
                var line = db.ModuleAI.Where(c => c.Ordernum == item).Select(c => c.AbnormalResultMessage).Distinct().ToList();
                foreach (var lineitem in line)
                {
                    var info = db.ModuleAI.Where(c => c.Ordernum == item && c.AbnormalResultMessage == lineitem).Select(c => c.IsAbnormal).ToList();
                    JObject obj1 = new JObject();
                    obj1.Add("Line", lineitem);
                    obj1.Add("CompleteRate", Math.Round((double)info.Count * 100 / order.Models, 2) + "%");
                    obj1.Add("CompleteInfo", info.Count + "/" + order.Models);
                    obj1.Add("PassRate", Math.Round((double)info.Count(c => c == true) * 100 / order.Models, 2) + "%");
                    obj1.Add("PassInfo", info.Count(c => c == true) + "/" + order.Models);
                    obj1.Add("SamplingRate", null);
                    obj1.Add("SamplingInfo", null);
                    itemArray.Add(obj1);
                }
                OrderNum.Add("AI", itemArray);

                #endregion

                #region  后焊
                itemArray = new JArray();
                var line2 = db.AfterWelding.Where(c => c.Ordernum == item).Select(c => c.Line).Distinct().ToList();
                foreach (var lineitem in line2)
                {
                    var info = db.AfterWelding.Where(c => c.Ordernum == item && c.Line == lineitem).Select(c => c.IsAbnormal).ToList();
                    var aftersamp = db.ModuleSampling.Count(c => c.Ordernum == item && c.Section == "后焊");
                    JObject obj1 = new JObject();
                    obj1.Add("Line", lineitem);
                    obj1.Add("CompleteRate", Math.Round((double)info.Count * 100 / order.Models, 2) + "%");
                    obj1.Add("CompleteInfo", info.Count + "/" + order.Models);
                    obj1.Add("PassRate", Math.Round((double)info.Count(c => c == true) * 100 / order.Models, 2) + "%");
                    obj1.Add("PassInfo", info.Count(c => c == true) + "/" + order.Models);
                    obj1.Add("SamplingRate", Math.Round((double)aftersamp * 100 / info.Count, 2) + "%");
                    obj1.Add("SamplingInfo", aftersamp + "/" + info.Count);
                    itemArray.Add(obj1);
                }
                OrderNum.Add("After", itemArray);

                #endregion

                #region  灌胶前电测
                itemArray = new JArray();

                var info1 = db.ElectricInspection.Where(c => c.Ordernum == item && c.Section == "灌胶前电检").Select(c => c.ElectricInspectionResult).ToList();
                if (info1.Count != 0)
                {
                    var beforesamp = db.ModuleSampling.Count(c => c.Ordernum == item && c.Section == "灌胶前电检");
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", Math.Round((double)info1.Count * 100 / order.Models, 2) + "%");
                    obj.Add("CompleteInfo", info1.Count + "/" + order.Models);
                    obj.Add("PassRate", Math.Round((double)info1.Count(c => c == true) * 100 / order.Models, 2) + "%");
                    obj.Add("PassInfo", info1.Count(c => c == true) + "/" + order.Models);
                    obj.Add("SamplingRate", Math.Round((double)beforesamp * 100 / info1.Count, 2) + "%");
                    obj.Add("SamplingInfo", beforesamp + "/" + info1.Count);
                    itemArray.Add(obj);
                }
                OrderNum.Add("Before", itemArray);

                #endregion

                #region  灌胶后电测
                itemArray = new JArray();
                var info2 = db.ElectricInspection.Where(c => c.Ordernum == item && c.Section == "模块电检").Select(c => c.ElectricInspectionResult).ToList();
                if (info2.Count != 0)
                {
                    var latersamp = db.ModuleSampling.Count(c => c.Ordernum == item && c.Section == "模块电检");
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", Math.Round((double)info2.Count * 100 / order.Models, 2) + "%");
                    obj.Add("CompleteInfo", info2.Count + "/" + order.Models);
                    obj.Add("PassRate", Math.Round((double)info2.Count(c => c == true) * 100 / order.Models, 2) + "%");
                    obj.Add("PassInfo", info2.Count(c => c == true) + "/" + order.Models);
                    obj.Add("SamplingRate", Math.Round((double)latersamp * 100 / info2.Count, 2) + "%");
                    obj.Add("SamplingInfo", latersamp + "/" + info2.Count);
                    itemArray.Add(obj);
                }
                OrderNum.Add("Later", itemArray);

                #endregion

                #region  老化
                itemArray = new JArray();

                var info3 = db.ModuleBurnIn.Where(c => c.Ordernum == item && c.BurnInEndTime != null).Select(c => c.BurninResult).ToList();
                if (info3.Count != 0)
                {
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", Math.Round((double)info3.Count * 100 / order.Models, 2) + "%");
                    obj.Add("CompleteInfo", info3.Count + "/" + order.Models);
                    obj.Add("PassRate", Math.Round((double)info3.Count(c => c == true) * 100 / order.Models, 2) + "%");
                    obj.Add("PassInfo", info3.Count(c => c == true) + "/" + order.Models);
                    obj.Add("SamplingRate", null);
                    obj.Add("SamplingInfo", null);
                    itemArray.Add(obj);
                }
                OrderNum.Add("BurnIn", itemArray);

                #endregion

                #region  外观电检
                itemArray = new JArray();

                var info4 = db.ElectricInspection.Where(c => c.Ordernum == item && c.Section == "外观电检").Select(c => c.ElectricInspectionResult).ToList();
                if (info4.Count != 0)
                {
                    var appsamp = db.ModuleSampling.Count(c => c.Ordernum == item && c.Section == "外观电检");
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", Math.Round((double)info4.Count * 100 / order.Models, 2) + "%");
                    obj.Add("CompleteInfo", info4.Count + "/" + order.Models);
                    obj.Add("PassRate", Math.Round((double)info4.Count(c => c == true) * 100 / order.Models, 2) + "%");
                    obj.Add("PassInfo", info4.Count(c => c == true) + "/" + order.Models);
                    obj.Add("SamplingRate", null);
                    obj.Add("SamplingInfo", null);
                    itemArray.Add(obj);
                }
                OrderNum.Add("Appearance", itemArray);

                #endregion

                #region  内箱
                itemArray = new JArray();

                var info5 = db.ModuleInsideTheBox.Where(c => c.OrderNum == item && c.Statue == "纸箱").Select(c => c.InnerBarcode).Distinct().ToList().Count();
                if (info5 != 0)
                {
                    var rule = db.ModulePackageRule.Where(c => c.OrderNum == item && c.Statue == "纸箱").Sum(c => c.Quantity);
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", rule == 0 ? "0%" : Math.Round((double)info5 * 100 / rule, 2) + "%");
                    obj.Add("CompleteInfo", info5 + "/" + rule);
                    obj.Add("PassRate", null);
                    obj.Add("PassInfo", null);
                    obj.Add("SamplingRate", null);
                    obj.Add("SamplingInfo", null);
                    itemArray.Add(obj);
                }
                OrderNum.Add("InsideTheBox", itemArray);

                #endregion

                #region  外箱
                itemArray = new JArray();
                var info6 = db.ModuleOutsideTheBox.Where(c => c.OrderNum == item).Select(c => c.OutsideBarcode).Distinct().ToList().Count();
                if (info6 != 0)
                {
                    var rule2 = db.ModulePackageRule.Where(c => c.OrderNum == item && c.Statue == "外箱").Sum(c => c.Quantity);
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", rule2 == 0 ? "0%" : Math.Round((double)info6 * 100 / rule2, 2) + "%");
                    obj.Add("CompleteInfo", info6 + "/" + rule2);
                    obj.Add("PassRate", null);
                    obj.Add("PassInfo", null);
                    obj.Add("SamplingRate", null);
                    obj.Add("SamplingInfo", null);
                    itemArray.Add(obj);
                }
                OrderNum.Add("OutsideTheBox", itemArray);

                #endregion

                #region  出入库
                itemArray = new JArray();
                var join = db.Warehouse_Join.Count(c => c.OrderNum == item && c.State == "模块");
                if (join != 0)
                {
                    var wareshouseout = db.Warehouse_Join.Count(c => c.OrderNum == item && c.State == "模块" && c.IsOut == true);
                    JObject obj = new JObject();
                    obj.Add("Line", null);
                    obj.Add("CompleteRate", join == 0 ? "0%" : Math.Round((double)wareshouseout * 100 / join, 2) + "%");
                    obj.Add("CompleteInfo", join + "/" + wareshouseout);
                    obj.Add("PassRate", null);
                    obj.Add("PassInfo", null);
                    obj.Add("SamplingRate", null);
                    obj.Add("SamplingInfo", null);
                    itemArray.Add(obj);
                }
                OrderNum.Add("Warehouse", itemArray);

                #endregion

                ProductionControlIndex.Add(OrderNum);
                i++;
            }

            string output2 = Newtonsoft.Json.JsonConvert.SerializeObject(ProductionControlIndex, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\ProductionController.json", output2);

            return com.GetModuleFromJarray(ProductionControlIndex);


        }



        #endregion

        #region------各工序功能

        #region---------------------------- 工段通用方法--------------------------
        #region    --------------------查询订单已完成、未完成、未开始条码
        /*
         *1.根据传过来的工序,找到完成的条码列表value
         * 2.如果是抽检的列表 IsSamping传过来是true,则已完成是根据订单和工序找到的抽检条码,未完成是value.except(抽检条码),抽检率是抽检条码/value
         * 3.如果是生产工序列表,已完成是 value,未完成是null,未开始是订单的条码列表移除value.
         */
        [HttpPost]
        [ApiAuthorize]
        public JObject Checklist([System.Web.Http.FromBody]JObject data)
        {

            string statue = data["statue"].ToString();//工序
            string orderNum = data["orderNum"].ToString();//订单号
            bool IsSamping = bool.Parse(data["IsSamping"].ToString());//是否是抽检
            JObject stationResult = new JObject();//输出结果JObject
            var value = new List<string>();
            switch (statue)
            {
                case "AI":
                    value = db.ModuleAI.Where(c => c.Ordernum == orderNum).OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();
                    break;

                case "后焊":
                    value = db.AfterWelding.Where(c => c.Ordernum == orderNum).OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();
                    break;

                case "灌胶前电检":
                    value = db.ElectricInspection.Where(c => c.Ordernum == orderNum && c.Section == "灌胶前电检").OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();

                    break;
                case "模块电检":
                    value = db.ElectricInspection.Where(c => c.Ordernum == orderNum && c.Section == "模块电检").OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();

                    break;
                case "老化":
                    value = db.ModuleBurnIn.Where(c => c.Ordernum == orderNum).OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();

                    break;
                case "外观电检":
                    value = db.ElectricInspection.Where(c => c.Ordernum == orderNum && c.Section == "外观电检").OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();

                    break;

            }

            if (IsSamping)
            {
                var smaping = db.ModuleSampling.Where(c => c.Ordernum == orderNum && c.Section == statue).OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();
                stationResult = ChecklistSmaplingItem(value, smaping, orderNum);
            }
            else
            {
                stationResult = ChecklistItem(value, orderNum);
            }
            return com.GetModuleFromJobjet(stationResult);
        }

        [HttpPost]
        [ApiAuthorize]
        public JObject ChecklistItem(List<string> value, string orderNum)
        {

            List<string> NotDoList = new List<string>();//未开始做条码清单
            JObject stationResult = new JObject();//输出结果JObject
                                                  //调出订单所有条码清单
            List<string> barcodelist = db.BarCodes.Where(c => c.OrderNum == orderNum && c.BarCodeType == "模块").OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();
            if (value.Count == 0)
            {
                JArray array = new JArray();
                barcodelist.ForEach(c => array.Add(c));
                stationResult.Add("NotDoList", array);
                stationResult.Add("NeverFinish", null);
                stationResult.Add("FinishList", null);
            }
            else
            {

                //未开始做条码清单
                NotDoList = barcodelist.Except(value).ToList();
                JArray NotDoListarray = new JArray();
                JArray FinishListarray = new JArray();

                NotDoList.ForEach(c => NotDoListarray.Add(c));
                //已完成条码清单
                value.ForEach(c => FinishListarray.Add(c));

                stationResult.Add("NotDoList", NotDoListarray);
                stationResult.Add("NeverFinish", null);
                stationResult.Add("FinishList", FinishListarray);
            }
            return stationResult;
        }

        [HttpPost]
        [ApiAuthorize]
        public JObject ChecklistSmaplingItem(List<string> finash, List<string> smapList, string orderNum)
        {

            List<string> NotDoList = new List<string>();//未开始做条码清单
            JObject stationResult = new JObject();//输出结果JObject
                                                  //调出订单所有条码清单
            var box = db.OrderMgm.Where(c => c.OrderNum == orderNum).Select(c => c.Models).FirstOrDefault();
            if (finash.Count == 0)
            {
                JArray array = new JArray();
                //barcodelist.ForEach(c => array.Add(c));
                stationResult.Add("NotDoList", null);
                stationResult.Add("SmaplingRate", "0%");
                stationResult.Add("FinishList", null);
            }
            else
            {

                //未开始做条码清单
                NotDoList = finash.Except(smapList).ToList();
                JArray NotDoListarray = new JArray();
                JArray FinishListarray = new JArray();

                NotDoList.ForEach(c => NotDoListarray.Add(c));
                //已完成条码清单
                smapList.ForEach(c => FinishListarray.Add(c));

                stationResult.Add("NotDoList", NotDoListarray);
                stationResult.Add("SmaplingRate", box == 0 ? "0%" : Math.Round((decimal)smapList.Count * 100 / finash.Count, 2) + "%");
                stationResult.Add("FinishList", FinishListarray);
            }
            return stationResult;
        }
        #endregion

        #region ----------条码验证检查
        /* 1.根据条码在条码表找条码信息,判断条码是否准确
         * 2.在条码表判断条码和订单是否相符
         * 3.根据条码在 AI/后焊/灌胶前/模块/老化/外观电检表中找是否有数据记录,
         */
        [HttpPost]
        [ApiAuthorize]
        public JObject AfterWeldingCheckBarcode([System.Web.Http.FromBody]JObject data)
        {
            string barcode = data["barcode"].ToString();//条码
            string orderNum = data["orderNum"].ToString();//订单
            JObject result = new JObject();
            JArray array = new JArray();
            var info = db.BarCodes.Where(c => c.BarCodesNum == barcode).FirstOrDefault();
            if (info == null)
            {
                result.Add("result", false);
                result.Add("message", "没有找到条码信息");
                result.Add("period", null);
            }
            else
            {
                if (info.OrderNum != orderNum)
                {
                    result.Add("result", false);
                    result.Add("message", "此条码与订单不符,该条码的订单是" + info.OrderNum);
                    result.Add("period", null);
                }
                else
                {
                    result.Add("result", true);
                    result.Add("message", null);
                    JObject item = new JObject();
                    //ai
                    var ai = db.ModuleAI.Where(c => c.ModuleBarcode == barcode).ToList();
                    item.Add("Name", "AI");
                    item.Add("Have", ai.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();
                    //后焊
                    var afterWelding = db.AfterWelding.Where(c => c.ModuleBarcode == barcode).ToList();
                    item.Add("Name", "后焊");
                    item.Add("Have", afterWelding.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    //灌胶前电检
                    var electricInspection = db.ElectricInspection.Where(c => c.ModuleBarcode == barcode && c.Section == "灌胶前电检").ToList();
                    item.Add("Name", "灌胶前电检");
                    item.Add("Have", electricInspection.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    //模块电检
                    var electricInspection2 = db.ElectricInspection.Where(c => c.ModuleBarcode == barcode && c.Section == "模块电检").ToList();
                    item.Add("Name", "模块电检");
                    item.Add("Have", electricInspection2.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    //老化
                    var moduleBurnIn = db.ModuleBurnIn.Where(c => c.ModuleBarcode == barcode).ToList();
                    item.Add("Name", "老化");
                    item.Add("Have", moduleBurnIn.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    //外观
                    var moduleAppearance = db.ElectricInspection.Where(c => c.ModuleBarcode == barcode && c.Section == "外观电检").ToList();
                    item.Add("Name", "外观电检");
                    item.Add("Have", moduleAppearance.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    result.Add("period", array);
                }
            }
            return com.GetModuleFromJobjet(result);
        }
        #endregion

        #region ------抽检前条码检查
        /* 1.根据订单条码,找到工序对应的表中是否有数据记录
         * 2.如果没找到,提示没有找到该条码信息,否则成功
         */
        [HttpPost]
        [ApiAuthorize]
        public JObject CheckSampling([System.Web.Http.FromBody]JObject data)
        {
            string barcode = data["barcode"].ToString();//条码
            string ordernum = data["ordernum"].ToString();//订单
            string statue = data["statue"].ToString();//工序
            int count = 0;
            // bool isSampling = false;
            JObject result = new JObject();
            switch (statue)
            {
                case "AI":
                    count = db.ModuleAI.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode);
                    //isSampling = db.AfterWelding.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault() ;
                    break;
                case "后焊":
                    count = db.AfterWelding.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode);
                    //isSampling = db.AfterWelding.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault() ;
                    break;
                case "灌胶前电检":
                    count = db.ElectricInspection.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode && c.Section == "灌胶前电检");
                    //isSampling = db.ElectricInspection.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault();
                    break;
                case "模块电检":
                    count = db.ElectricInspection.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode && c.Section == "模块电检");
                    //isSampling = db.ElectricInspection.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault();
                    break;
                case "老化":
                    count = db.ModuleBurnIn.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode);
                    //isSampling = db.ElectricInspection.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault();
                    break;
                case "外观电检":
                    count = db.ElectricInspection.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode && c.Section == "外观电检");
                    //isSampling = db.ElectricInspection.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault();
                    break;
            }
            if (count == 0)
            {
                return com.GetModuleFromJobjet(result, false, "没有找到该条码信息");
            }
            else
            {
                return com.GetModuleFromJobjet(result, true, "成功");
            }
        }


        #endregion

        #endregion

        #region--------------------AI 暂时line不确定能不能拿到--------------------
        /*
         * 1.先判断该条码是否有AI记录.有则不能再创建
         * 2.line 获取是拿到条码的最后一位数
        */
        [HttpPost]
        [ApiAuthorize]
        public JObject AICreate([System.Web.Http.FromBody]JObject data)
        {
            string ModuleBarcode = data["ModuleBarcode"].ToString();//条码
            string Ordernum = data["Ordernum"].ToString();//订单
            string UserName = data["UserName"].ToString();//用户名
            string Department = data["Department"].ToString();//部门
            string Group = data["Group"].ToString();//班组
            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                if (db.ModuleAI.Count(c => c.ModuleBarcode == ModuleBarcode) > 0)
                {
                    return com.GetModuleFromJobjet(result, false, "该条码已有AI记录");
                }
                var line = ModuleBarcode.Substring(ModuleBarcode.Length - 1, 1);
                ModuleAI moduleAI = new ModuleAI();
                moduleAI.Machine = line;
                moduleAI.AITime = DateTime.Now;
                moduleAI.AIor = UserName;
                moduleAI.ModuleBarcode = ModuleBarcode;
                moduleAI.Ordernum = Ordernum;
                moduleAI.Department = Department;
                moduleAI.Group = Group;
                moduleAI.IsAbnormal = true;
                moduleAI.AbnormalResultMessage = "正常";
                db.ModuleAI.Add(moduleAI);
                db.SaveChanges();
                UpdateNewTable(moduleAI.Ordernum, "AI", DateTime.Now);
                return com.GetModuleFromJobjet(result, true, "成功");

            }
            return com.GetModuleFromJobjet(result, false, "错误,数据格式不对");
        }
        #endregion

        #region -------------------------------后焊--------------------------------
        #region----------后焊创建新数据
        /*
         * 判断条码有没有后焊记录,没有则创建后焊记录
         */
        [HttpPost]
        [ApiAuthorize]
        public JObject AfterWeldingCreate([System.Web.Http.FromBody]JObject data)
        {

            string ModuleBarcode = data["ModuleBarcode"].ToString();//条码
            string Ordernum = data["Ordernum"].ToString();//订单
            string UserName = data["UserName"].ToString();//用户名
            string Department = data["Department"].ToString();//部门
            string Group = data["Group"].ToString();//班组

            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                if (db.AfterWelding.Count(c => c.ModuleBarcode == ModuleBarcode) > 0)
                {
                    return com.GetModuleFromJobjet(result, false, "该条码已有后焊记录");
                }
                if (db.BarCodes.Count(c => c.BarCodesNum == ModuleBarcode) != 0)
                {
                    AfterWelding afterWelding = new AfterWelding();
                    afterWelding.ModuleBarcode = ModuleBarcode;
                    afterWelding.Ordernum = Ordernum;
                    afterWelding.Department = Department;
                    afterWelding.Group = Group;
                    afterWelding.AfterWeldingTime = DateTime.Now;
                    afterWelding.AfterWeldingor = UserName;
                    afterWelding.IsAbnormal = true;
                    afterWelding.AbnormalResultMessage = "正常";
                    db.AfterWelding.Add(afterWelding);
                    db.SaveChangesAsync();
                    UpdateNewTable(afterWelding.Ordernum, "后焊", DateTime.Now);
                    return com.GetModuleFromJobjet(result, true, "成功");
                }
                else
                {
                    return com.GetModuleFromJobjet(result, false, "条码错误");
                }

            }
            return com.GetModuleFromJobjet(result, false, "错误,数据格式不对");
        }
        #endregion

        #endregion

        #region----------灌胶前电检工序 灌胶后电检工序 外观电检 数据创建----------
        /*
        * 判断条码有没有电检记录,没有则创建电检记录
        */
        [HttpPost]
        [ApiAuthorize]
        public JObject ElectricInspectionBeforeGlueFillingCreate([System.Web.Http.FromBody]JObject data)
        {
            string ModuleBarcode = data["ModuleBarcode"].ToString();//条码
            string Ordernum = data["Ordernum"].ToString();//订单
            string UserName = data["UserName"].ToString();//用户名
            string Department = data["Department"].ToString();//部门
            string Group = data["Group"].ToString();//班组
            string Section = data["Section"].ToString();//工序
            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                if (db.ElectricInspection.Count(c => c.ModuleBarcode == ModuleBarcode && c.Section == Section) > 0)
                {
                    return com.GetModuleFromJobjet(result, false, "该条码已有电检记录");
                }
                ElectricInspection electricInspection = new ElectricInspection();
                electricInspection.ModuleBarcode = ModuleBarcode;
                electricInspection.Ordernum = Ordernum;
                electricInspection.ElectricInspectionTime = DateTime.Now;
                electricInspection.ElectricInspectionor = UserName;
                electricInspection.Section = Section;
                electricInspection.ElectricInspectionMessage = "正常";
                electricInspection.ElectricInspectionResult = true;
                electricInspection.Department = Department;
                electricInspection.Group = Group;

                db.ElectricInspection.Add(electricInspection);
                db.SaveChanges();
                UpdateNewTable(Ordernum, Section, DateTime.Now);
                return com.GetModuleFromJobjet(result, true, "成功");
            }
            return com.GetModuleFromJobjet(result, false, "错误,数据格式不对");
        }
        #endregion

        #region -------------------------------老化--------------------------------

        #region 老化新增
        /*
        * 判断条码有没有老化记录,没有则创建老化记录
        */
        [HttpPost]
        [ApiAuthorize]
        public JObject BurninCreate([System.Web.Http.FromBody]JObject data)
        {
            string ModuleBarcode = data["ModuleBarcode"].ToString();//条码
            string Ordernum = data["Ordernum"].ToString();//订单
            string UserName = data["UserName"].ToString();//用户名
            string Department = data["Department"].ToString();//部门
            string Group = data["Group"].ToString();//班组
            string BurninFrame = data["BurninFrame"].ToString();//老化架号
            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                if (db.ModuleBurnIn.Count(c => c.ModuleBarcode == ModuleBarcode) > 0)
                {
                    return com.GetModuleFromJobjet(result, false, "该条码已有老化记录");
                }
                ModuleBurnIn moduleBurn = new ModuleBurnIn();
                moduleBurn.BurnInStartor = UserName;
                moduleBurn.BurnInStartTime = DateTime.Now;
                moduleBurn.BurninFrame = BurninFrame;
                moduleBurn.BurninResult = true;
                moduleBurn.OldOrdernum = Ordernum;
                moduleBurn.Ordernum = Ordernum;
                moduleBurn.OldModuleBarcode = ModuleBarcode;
                moduleBurn.ModuleBarcode = ModuleBarcode;
                moduleBurn.Department = Department;
                moduleBurn.Group = Group;
                db.ModuleBurnIn.Add(moduleBurn);
                db.SaveChanges();
                UpdateNewTable(moduleBurn.Ordernum, "老化", DateTime.Now);
                return com.GetModuleFromJobjet(result, true, "成功");
            }
            return com.GetModuleFromJobjet(result, false, "错误,数据格式不对");
        }
        #endregion

        #region 老化异常输入
        /*
        * 判断条码有没有老化开始记录,有则保存异常信息,没有则提示没有找到老化中条码
        */
        [HttpPost]
        [ApiAuthorize]
        public JObject BurninAbnormal([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();//订单
            string modulebarcode = data["modulebarcode"].ToString();//条码
            string BurninResult = data["BurninResult"].ToString();//异常信息
            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                var info = db.ModuleBurnIn.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == modulebarcode && c.OldModuleBarcode == modulebarcode && c.BurnInEndor == null).FirstOrDefault();
                if (info == null)
                {
                    return com.GetModuleFromJobjet(result, false, "没有找到老化中的条码,请检查条码准确");
                }
                info.BurninResult = false;
                info.BurninMessage = BurninResult;
                db.SaveChanges();
                UpdateNewTable(ordernum, "老化", DateTime.Now);
                result.Add("result", true);
                result.Add("mes", "成功");
                return com.GetModuleFromJobjet(result, true, "成功");
            }
            result.Add("result", false);
            result.Add("mes", "错误,数据格式不对");
            return com.GetModuleFromJobjet(result, false, "错误,数据格式不对");
        }

        #endregion

        #region 老化完成
        /*
         * 老化完成前检查,检查是否已经开始老化,并且是否已经完成老化
         */
        [HttpPost]
        [ApiAuthorize]
        public JObject BurninCompleteCheck([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();//订单
            JArray modulbarcode = (JArray)data["modulbarcode"];//条码列表
            JArray array = new JArray();
            foreach (var token in modulbarcode)
            {
                string item = token.ToString();
                JObject result = new JObject();
                result.Add("barcode", item);
                var havintcout = db.ModuleBurnIn.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == item);
                var finshin = db.ModuleBurnIn.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == item && c.BurnInEndor != null);
                if (havintcout == 0)
                {
                    result.Add("Result", false);
                    result.Add("Message", "没有找到开始老化记录");
                }
                else if (finshin != 0)
                {
                    result.Add("Result", false);
                    result.Add("Message", "条码已经完成老化");
                }
                else
                {
                    result.Add("Result", true);
                    result.Add("Message", "正常");
                }
                array.Add(result);

            }
            return com.GetModuleFromJarray(array);
        }

        //老化完成数据保存
        [HttpPost]
        [ApiAuthorize]
        public JObject BurninComplete([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();//订单
            string UserName = data["UserName"].ToString();//用户
            List<string> modulbarcode =JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject( data["modulbarcode"]));//条码列表
            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                var info = db.ModuleBurnIn.Where(c => c.Ordernum == ordernum && modulbarcode.Contains(c.ModuleBarcode)).ToList();
                info.ForEach(c => { c.BurnInEndor = UserName; c.BurnInEndTime = DateTime.Now; });

                db.SaveChanges();
                UpdateNewTable(ordernum, "老化", DateTime.Now);
                return com.GetModuleFromJobjet(result, true, "成功");
            }
            return com.GetModuleFromJobjet(result, false, "错误,数据格式不对");
        }
        #endregion
        #endregion
        /*
        * 用于模块看板的更新数据,每做一个工序操作,会修改ModuleBoard数据的时间
        */
        public void UpdateNewTable(string ordernum, string seaction, DateTime? time)
        {
            var board = db.ModuleBoard.Where(c => c.Ordernum == ordernum && c.Section == seaction).FirstOrDefault();
            if (board == null)
            {
                ModuleBoard moduleBoard = new ModuleBoard() { Ordernum = ordernum, Section = seaction, UpdateDate = time };
                db.ModuleBoard.Add(moduleBoard);
                db.SaveChanges();
            }
            else
            {
                board.UpdateDate = time;
                db.SaveChanges();
            }
        }

        #region -------------------------------抽检--------------------------------
        /*
        * 判断条码是有已经有抽检记录,没有则创建抽检记录
        */
        [HttpPost]
        [ApiAuthorize]
        public JObject Sampling([System.Web.Http.FromBody]JObject data)
        {
            string ModuleBarcode = data["ModuleBarcode"].ToString();//条码
            string Ordernum = data["Ordernum"].ToString();//订单
            string UserName = data["UserName"].ToString();//用户名
            string Department = data["Department"].ToString();//部门
            string Group = data["Group"].ToString();//班组
            string Section = data["Section"].ToString();//工序
            JObject result = new JObject();
            var have = 0;
            if (Section == "后焊")
            {
                have = db.AfterWelding.Count(c => c.ModuleBarcode == ModuleBarcode);
            }
            if (Section == "灌胶前电检")
            {
                have = db.ElectricInspection.Count(c => c.ModuleBarcode == ModuleBarcode && c.Section == "灌胶前电检");
            }
            if (Section == "模块电检")
            {
                have = db.ElectricInspection.Count(c => c.ModuleBarcode == ModuleBarcode && c.Section == "模块电检");
            }
            if (Section == "老化")
            {
                have = db.AfterWelding.Count(c => c.ModuleBarcode == ModuleBarcode);
            }
            if (Section == "外观电检")
            {
                have = db.ElectricInspection.Count(c => c.ModuleBarcode == ModuleBarcode && c.Section == "外观电检");
            }
            if (have != 0)
            {
                var same = db.ModuleSampling.Count(c => c.ModuleBarcode == ModuleBarcode && c.Section == Section);
                if (same != 0)
                {
                    return com.GetModuleFromJobjet(result, false, "错误,此条码已抽检");
                }

                ModuleSampling moduleSampling = new ModuleSampling();
                moduleSampling.ModuleBarcode = ModuleBarcode;
                moduleSampling.Ordernum = Ordernum;
                moduleSampling.Section = Section;
                moduleSampling.SamplingTime = DateTime.Now;
                moduleSampling.Samplingor = UserName;
                moduleSampling.SamplingMessage = "正常";
                moduleSampling.SamplingResult = true;
                moduleSampling.Department = Department;
                moduleSampling.Group = Group;

                db.ModuleSampling.Add(moduleSampling);
                db.SaveChanges();
                UpdateNewTable(moduleSampling.Ordernum, moduleSampling.Section, DateTime.Now);
                return com.GetModuleFromJobjet(result, true, "成功");
            }
            return com.GetModuleFromJobjet(result, false, "错误,此条码未进行" + Section);
        }
        #endregion

        #endregion


        #region ------内箱or外箱装箱规则
        #region 内箱OR外箱装箱规则
        /*
        * 根据订单和装箱类型找到以前的装箱规则,如果有则删除掉以前,没有则不作处理
        * 将新传过来的装箱类型,每箱数量,包装箱件数,是否分屏,屏序,是否分批,批次号,存到数据中
        */
        [HttpPost]
        [ApiAuthorize]
        public JObject InnerInfo([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));

            string ITEMNO = obj.ITEMNO.ToString();//规格型号
            string COLOURS = obj.COLOURS.ToString();//颜色
            string Remark = obj.Remark.ToString();//备注
            string UserName = obj.UserName.ToString();//用户名
            //string Department = data["Department"].ToString();//部门
            //string Group = data["Group"].ToString();//班组
            List<ModulePackageRule> modulePackageRule = JsonConvert.DeserializeObject<List<ModulePackageRule>>(JsonConvert.SerializeObject(obj.modulePackageRule));
            //先删除原有的
            string ordernum = modulePackageRule.Select(c => c.OrderNum).FirstOrDefault();//提出订单号
            string statue = modulePackageRule.FirstOrDefault().Statue;
            var list = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).ToList();//根据订单找到信息
            db.ModulePackageRule.RemoveRange(list);//移除
            db.SaveChanges();
            //生成新的
            if (ModelState.IsValid)//判断数据格式是否正确
            {
                modulePackageRule.ForEach(c => { c.CreateDate = DateTime.Now; c.Creator = UserName; });
                db.ModulePackageRule.AddRange(modulePackageRule);//添加
                db.SaveChanges();
                var info = db.ModulePackageRule.Where(c => c.OrderNum == ordernum).ToList();
                info.ForEach(c => { c.ITEMNO = ITEMNO; c.COLOURS = COLOURS; c.Remark = Remark; });
                db.SaveChanges();
                return com.GetModuleFromJobjet(null, true, "成功");
            }
            return com.GetModuleFromJobjet(null, false, "失败");
        }

        #endregion

        /// <summary>
        /// 作用:根据给的订单号，显示包装信息
        /// </summary>
        /// 逻辑:根据订单号查找包装录入规则,把其中的包装的类型,容量,数量是否分屏,能否修改信息,将信息放回前端(能放修改逻辑:没有打印的可以修改,已经打印的,返回前端一个已经打印的数量,前端根据这个数量,设置只能增加,不能减少)
        /// 结果:将信息放回前端
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject ValueFromOrderNum([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string ordernum = obj.ordernum;//订单
            string statue = obj.statue;//纸盒纸箱外箱
            JObject valueitem = new JObject();
            JObject value = new JObject();
            var packingList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).ToList();//根据订单显示包装规则信息

            if (packingList == null)//如果规则信息为空
            {
                return com.GetModuleFromJobjet(null);
            }
            int i = 0;
            foreach (var item in packingList)//循环规则信息
            {
                valueitem.Add("packingType", item.Type);//包装类型
                valueitem.Add("itemNum", item.OuterBoxCapacity);//包装容量
                valueitem.Add("Num", item.Quantity);//包装数量
                valueitem.Add("batch", item.Batch);//批次
                valueitem.Add("isBatch", item.IsBatch);//是否批次
                valueitem.Add("screenNum", item.ScreenNum);//分屏
                valueitem.Add("isSeparate", item.IsSeparate);
                var count = 0;
                if (statue == "纸箱" || statue == "纸盒")
                {
                    count = db.ModuleInsideTheBox.Where(c => c.IsEmbezzle == false && c.IsMixture == false && c.Type == item.Type && c.OrderNum == item.OrderNum && c.Statue == statue).Select(c => c.InnerBarcode).Distinct().Count(); ;//根据订单号和类型找 没有混装,没有挪用的 纸箱包装打印记录
                }
                else
                {
                    count = db.ModuleOutsideTheBox.Where(c => c.IsEmbezzle == false && c.IsMixture == false && c.Type == item.Type && c.OrderNum == item.OrderNum).Select(c => c.OutsideBarcode).Distinct().Count(); ;//根据订单号和类型找 没有混装,没有挪用的 外箱包装打印记录
                }
                if (count == 0)
                {
                    valueitem.Add("update", "true"); //没有包装打印记录，可以修改
                    valueitem.Add("min", 0);
                }
                else
                {
                    valueitem.Add("update", "false");//有包装打印记录，不可以修改
                    valueitem.Add("min", count);
                }
                value.Add(i.ToString(), valueitem);
                i++;
                valueitem = new JObject();
            }
            return com.GetModuleFromJobjet(value);
        }

        [HttpPost]
        [ApiAuthorize]
        //拿到规格型号,颜色,备注
        public JObject InformationFromOrderNum([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string ordernum = obj.ordernum;//订单
            JObject result = new JObject();

            var packingList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum).ToList();//根据订单显示包装规则信息

            if (packingList.Count == 0)//如果规则信息为空
            {
                return com.GetModuleFromJobjet(null, false, "规则信息为空");
            }
            result.Add("ITEMNO", packingList.FirstOrDefault().ITEMNO);//规格型号
            result.Add("COLOURS", packingList.FirstOrDefault().COLOURS);//颜色
            result.Add("Remark", packingList.FirstOrDefault().Remark);//特长的一段字符串
            return com.GetModuleFromJobjet(result, true, "规则信息为空");
        }
        #endregion

        #region------内箱装箱记录

        /*
         * 1.不常用,内箱打印剩余条码
         * 根据订单拿到已经后焊的条码列表和已经装内箱的条码列表,求差得到已后焊但没有装内箱的条码列表
         * 循环已后焊但没有装内箱的条码列表,根据订单拼出内箱条码号,序列号那已装内省的最后一个序列号+1,根据装箱类型 例如1装10,循环到第10个,就开始打印条码.并且序列号+1.以此类推
        
        [HttpPost]
        [ApiAuthorize]
        public void Remaining([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();//订单
            string ip = data["ip"].ToString();//IP地址
            int concentration = int.Parse(data["concentration"].ToString());//浓度
            int notprintCount = int.Parse(data["notprintCount"].ToString());//不打印的数量,可能是备品数
            //后焊条码列表
            var havbarcode = db.AfterWelding.Where(c => c.Ordernum == ordernum).Select(c => c.ModuleBarcode).Distinct().ToList();
            //纸箱条码列表
            var completebarcode = db.ModuleInsideTheBox.Where(c => c.OrderNum == ordernum && c.Statue == "纸箱").Select(c => c.ModuleBarcode).Distinct().ToList();
            //得到已后焊但没有装内箱的条码列表
            var notcomlpete = havbarcode.Except(completebarcode).ToList();
            //拿到装箱规则定义的装箱类型
            var type = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "纸箱").Select(c => c.Type).FirstOrDefault();
            //拿到装箱规则定义的一箱装几个
            var typenum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "纸箱").Select(c => c.OuterBoxCapacity).FirstOrDefault();
            //拿到SN
            var SN = db.ModuleInsideTheBox.Where(c => c.OrderNum == ordernum && c.Statue == "纸箱").Max(c => c.SN) + 1;
            //将订单号分割,为后面拼出内箱条码做准备
            var ordernumArray = ordernum.Split('-');
            var num = ordernumArray[2].PadLeft(2, '0');

            int i = 1;
            var count = notcomlpete.Count - notprintCount;
            List<string> onebarcode = new List<string>();
            List<ModuleInsideTheBox> insideTheBoxes = new List<ModuleInsideTheBox>();
            foreach (var item in notcomlpete)
            {

                var InnerBarcode = ordernumArray[0] + ordernumArray[1] + num + "-ZX" + SN.ToString().PadLeft(4, '0');
                ModuleInsideTheBox box = new ModuleInsideTheBox() { OrderNum = ordernum, ModuleBarcode = item, SN = SN, InnerBarcode = InnerBarcode, InnerTime = DateTime.Now, Inneror = "唐硕贵", Type = type, Department = "总装二部", Group = "屏体包装一组", Statue = "纸箱" };

                insideTheBoxes.Add(box);
                onebarcode.Add(item);
                if (i % typenum == 0)
                {
                    SN++;
                    db.ModuleInsideTheBox.AddRange(insideTheBoxes);
                    db.SaveChanges();
                    List<string> barcodelsit = new List<string>();
                    var ss = onebarcode[1].Substring(onebarcode[1].IndexOf('B')).ToString();
                    onebarcode.ForEach(c => barcodelsit.Add(c.Substring(c.IndexOf('B')).ToString()));

                    ModuleInsideBoxLablePrint(barcodelsit.ToArray(), ordernum, InnerBarcode, ip, 9101, concentration);

                    onebarcode = new List<string>();
                    insideTheBoxes = new List<ModuleInsideTheBox>();
                }
                i++;
                if (i == count)
                { break; }
            }

        }
         */
        //输入订单号显示内箱条码和SN/TN/类型
        /* 1.先根据订单和装箱款式找是否有创建装箱规则
         * 2.拿到装箱规则定义的装箱数量,去对比已经装箱的数量,判断该订单是否都已经装箱完成
         * 3.判断没问题,先根据装箱款式,拼出正确的内箱条码的前缀,纸盒的是XXXX-ZH,纸箱是XXXX-ZX.循环定义的装箱数量从1的序列号开始,去装箱记录找是否有相同的内箱条码,直到找到没有记录的序列号,得到最新的内箱条码,装箱类型SN
         * 4.循环装箱款式,拿到装箱款式包含的所有装箱类型,并根据装箱款式和装箱类型拿到定义总数量和包装数量
         */
        [HttpPost]
        [ApiAuthorize]
        public JObject ModuleInsideTheBoxInfo([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();//订单
            string statue = data["statue"].ToString();//装箱款式
            string type = data["type"].ToString();//装箱类型
            JObject result = new JObject();
            var totalCount = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).ToList();
            if (totalCount.Count == 0)
            {
                result.Add("InnerBarcode", null);
                result.Add("SN/TN", null);
                result.Add("Complete", null);
                return com.GetModuleFromJobjet(result, false, "没有创建装箱规则");
            }
            else
            {
                var count = totalCount.Where(c => c.Type == type).Sum(c => c.Quantity);
                var totalcount = totalCount.Sum(c => c.Quantity);
                var princount = db.ModuleInsideTheBox.Where(c => c.OrderNum == ordernum && c.Type == type).Select(c => c.InnerBarcode).Distinct().Count();//这里要加statue
                JArray typearray = new JArray();
                totalCount.ForEach(c => typearray.Add(c.Type));
                if (princount >= count)
                {
                    result.Add("mes", "已打印完");
                    result.Add("InnerBarcode", null);
                    result.Add("type", null);
                    result.Add("SN/TN", null);
                    result.Add("Complete", null);
                    return com.GetModuleFromJobjet(result, false, "已打印完");
                }
                var InnerBarcode = "";
                if (statue == "纸盒")
                {
                    var ordernumArray = ordernum.Split('-');
                    var num = ordernumArray[2].PadLeft(2, '0');
                    InnerBarcode = ordernumArray[0] + ordernumArray[1] + num + "-ZH";
                }
                else
                {
                    var ordernumArray = ordernum.Split('-');
                    var num = ordernumArray[2].PadLeft(2, '0');
                    InnerBarcode = ordernumArray[0] + ordernumArray[1] + num + "-ZX";
                }
                for (var i = 1; i <= totalcount; i++)
                {
                    var temp = InnerBarcode + i.ToString().PadLeft(4, '0');
                    if (db.ModuleInsideTheBox.Where(c => c.InnerBarcode == temp).Count() == 0)
                    {
                        result.Add("mes", "成功");
                        result.Add("InnerBarcode", temp);
                        result.Add("type", typearray);
                        result.Add("SN/TN", i + "/" + totalcount);
                        break;
                    }
                }
                var rule = db.ModulePackageRule.Where(c => c.OrderNum == ordernum).Select(c => new { c.ITEMNO, c.COLOURS, c.Remark }).FirstOrDefault();
                var boxnum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).Sum(c => c.Quantity * c.OuterBoxCapacity);
                result.Add("ITEMNO", rule.ITEMNO);
                result.Add("COLOURS", rule.COLOURS);
                result.Add("Remark", rule.Remark);
                result.Add("CTNS_PSC", boxnum);//数量 3/boxnum
                JArray completeArray = new JArray();
                var ruleinfo = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue != "外箱").Select(c => c.Statue).Distinct();
                foreach (var item in ruleinfo)
                {

                    var statueinfo = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == item).ToList();
                    foreach (var statuintem in statueinfo)
                    {
                        JObject obj = new JObject();
                        obj.Add("Statue", item);//纸盒或者纸箱
                        obj.Add("Type", statuintem.Type);//1装几
                        var printcount = db.ModuleInsideTheBox.Where(c => c.OrderNum == ordernum && c.Statue == item && c.Type == statuintem.Type).Select(c => c.InnerBarcode).Distinct().Count();
                        obj.Add("Num", printcount + "/" + statuintem.Quantity);
                        completeArray.Add(obj);
                    }
                }
                result.Add("Complete", completeArray);
                return com.GetModuleFromJobjet(result, true, "成功");
            }
        }

        //创建内箱记录
        [HttpPost]
        [ApiAuthorize]
        public JObject ModuleInsideTheBoxCreate([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string type = obj.type;//1装10
            string ordernum = obj.ordernum;//订单
            string innerBarccode = obj.innerBarccode;//内箱标签
            string UserName = obj.UserName;//用户名
            int SN = obj.SN == null ? 0 : obj.SN;//SN
            string statue = obj.statue;//装箱款式
            string Department = obj.Department;//部门
            string Group = obj.Group;//班组
            List<string> barcodeList = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(obj.barcodeList));//条码列表
            bool IsLast = obj.IsLast == null ? false : obj.IsLast;//是否是尾箱
            List<ModuleInsideTheBox> moduleList = new List<ModuleInsideTheBox>();


            var aa = type.Substring(2);
            var num = int.Parse(type.Substring(2));
            if (barcodeList.Count != num && !IsLast)
            {
                return com.GetModuleFromJobjet(null, false, "包装数量有与规则不符");
            }

            foreach (var item in barcodeList)
            {
                ModuleInsideTheBox module = new ModuleInsideTheBox();
                module.ModuleBarcode = item.ToString();
                module.InnerBarcode = innerBarccode;
                module.OrderNum = ordernum;
                module.SN = SN;
                module.Type = type;
                module.Department = Department;
                module.Group = Group;
                module.Statue = statue;
                module.InnerTime = DateTime.Now;
                module.Inneror = UserName;
                moduleList.Add(module);
            }
            db.ModuleInsideTheBox.AddRange(moduleList);
            db.SaveChanges();
            UpdateNewTable(ordernum, "纸箱", DateTime.Now);
            return com.GetModuleFromJobjet(null, true, "包装成功");
        }

        //根据订单判断是否有内内箱
        [HttpPost]
        [ApiAuthorize]
        public JObject PackList([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();
            var orders = db.ModulePackageRule.OrderByDescending(m => m.ID).Where(c => c.OrderNum == ordernum && c.Statue != "外箱").Select(m => m.Statue).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return com.GetModuleFromJarray(result);
        }
        #endregion

        #region------外箱装箱记录

        //根据订单,类型,是否尾箱,屏号,批号,得到重量,完成数量情况,可以打印数量,标签信息
        //1.根据订单和类型,先判断是否有包装规则,没有直接返回错误提示,没找到该订单的类型包装信息
        //2标签信息:判断打印数量是否有超过定义数量,超过则返回错误提示"订单已经打印完",否则通过循环已经打印的条码找到下一个外箱条码
        //3数量信息:找到装箱规则定义的装箱数量
        //4重量信息:如果是尾箱,拿到尾箱的毛重和净重.否则拿不是尾箱的毛重和净重
        //5完成信息:先循环类型再循环批号再循环屏号,拿到完成数量,完成率.再计算总的完成率打印数/定义数
        [HttpPost]
        [ApiAuthorize]
        public JObject OuthersideBoxInfo([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string ordernum = obj.ordernum;//订单
            string type = obj.type;//类型
            int screenNum = obj.screenNum == null ? 1 : obj.screenNum;//屏序号
            int batchNum = obj.batchNum == null ? 1 : obj.batchNum;//批次号
            bool IsLast = obj.IsLast == null ? false : obj.IsLast;//是否是尾箱
            JObject result = new JObject();

            var basicInfo = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == type && c.ScreenNum == screenNum && c.Batch == batchNum && c.Statue == "外箱").ToList(); //查找包装规则信息
            var typeList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "外箱").Select(c => c.Type).Distinct().ToList();//查找订单查包装规则的类型
            if (basicInfo.Count == 0)//如果找不到包装规则信息
            {
                result.Add("G_Weight", null);//毛重量
                result.Add("N_Weight", null);//净重
                result.Add("Complate", null);
                result.Add("count", null);//装的数量
                result.Add("boxNum", null);//外箱条码
                result.Add("SNTN", null);//SN/TN
                return com.GetModuleFromJobjet(result, false, "没找到该订单的类型包装信息");
            }
            #region 标签信息
            var count = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batchNum && c.Statue == "外箱").Sum(c => c.Quantity);//根据订单、分屏号查找规定的包装数量总和
            var printCount = db.ModuleOutsideTheBox.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batchNum).Select(c => c.OutsideBarcode).Distinct().ToList().Count();//根据订单、分屏号查找打印的外箱数量
            if (printCount < count)//判断打印的数量是否大于定义包装数量,如果打印数量大于等于定义包装数量,则返回null
            {
                #region 外箱条码生成

                string[] str = ordernum.Split('-');//将订单号分割
                string OuterBoxBarCodeNum = "";
                if (str.Count() == 2)
                {
                    OuterBoxBarCodeNum = ordernum + "-" + batchNum.ToString().PadLeft(2, '0') + "-" + screenNum.ToString().PadLeft(2, '0') + "-MK";
                }
                else
                {
                    string start = str[0].Substring(2);//取出 如2017-test-1 的17
                    OuterBoxBarCodeNum = start + str[1] + "-" + str[2] + "-" + batchNum.ToString().PadLeft(2, '0') + "-" + screenNum.ToString().PadLeft(2, '0') + "-MK";//外箱条码组成 

                }
                int SN = 0;
                for (int i = 1; i <= count; i++)//从1开始循环,最大数为定义的打印数,用来确定标签的序列号
                {
                    var num = OuterBoxBarCodeNum + i.ToString().PadLeft(3, '0');//生成含有序列数的标签号

                    if (db.ModuleOutsideTheBox.Count(c => c.OutsideBarcode == num) == 0)//判断打印表里是否有相同的标签号,没有则将此标签号存入数据中,有则继续循环
                    {
                        OuterBoxBarCodeNum = OuterBoxBarCodeNum + i.ToString().PadLeft(3, '0');
                        SN = i;
                        //外箱条码
                        result.Add("boxNum", OuterBoxBarCodeNum);
                        //SN/TN
                        result.Add("SNTN", SN + "/" + count);
                        break;
                    }

                }
                #endregion
            }
            else
            {
                result.Add("G_Weight", null);//毛重量
                result.Add("N_Weight", null);//净重
                result.Add("Complate", null);
                result.Add("count", null);
                result.Add("boxNum", null);//外箱条码
                result.Add("SNTN", null);//SN/TN
                result.Add("pass", false);
                result.Add("mes", "订单已经打印完");
                return com.GetModuleFromJobjet(result, false, "订单已经打印完");
            }
            #endregion
            #region 数量信息
            var countfromint = basicInfo.FirstOrDefault().OuterBoxCapacity;
            result.Add("count", countfromint);
            var rule = db.ModulePackageRule.Where(c => c.OrderNum == ordernum).Select(c => new { c.ITEMNO, c.COLOURS, c.Remark }).FirstOrDefault();
            var boxnum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "外箱").Sum(c => c.Quantity * c.OuterBoxCapacity);
            result.Add("ITEMNO", rule.ITEMNO);
            result.Add("COLOURS", rule.COLOURS);
            result.Add("Remark", rule.Remark);
            result.Add("CTNS_PSC", boxnum);//数量 3/boxnum
            #endregion

            #region 重量信息
            if (IsLast)//是尾箱
            {
                var last = basicInfo.FirstOrDefault();//查找尾箱重量不为0的信息
                if (last != null)//如果信息不为空
                {
                    result.Add("G_Weight", last.Tail_G_Weight);//毛重量
                    result.Add("N_Weight", last.Tail_N_Weight);//净重
                }

            }
            else//不是尾箱
            {
                var full = basicInfo.FirstOrDefault();//找到整箱重量不为0的信息
                if (full != null)//如果信息不为空
                {
                    result.Add("G_Weight", full.Full_G_Weight);//毛重量
                    result.Add("N_Weight", full.Full_N_Weight);//净重
                }
            }
            #endregion
            #region 完成信息
            JArray total = new JArray();
            foreach (var item in typeList)//循环类型
            {
                var batchList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == item && c.Statue == "外箱").Select(c => c.Batch).Distinct().ToList();//根据订单、类型查找包装规则的分屏号
                foreach (var batchnum in batchList)//循环批号
                {
                    var screemNumList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == item && c.Batch == batchnum && c.Statue == "外箱").Select(c => c.ScreenNum).Distinct().ToList();
                    foreach (var screenNumitem in screemNumList)//循环屏序号
                    {
                        JObject info = new JObject();
                        var totleNum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNumitem && c.Batch == batchnum && c.Statue == "外箱").ToList().Count() == 0 ? 0 : db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNumitem && c.Batch == batchnum).Sum(c => c.Quantity);//根据订单号、类型、分屏号查找包装规则的包装总数量(用于显示完成数量的分母显示)
                        var printBarcodeinfo = db.ModuleOutsideTheBox.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNumitem && c.Batch == batchnum).Select(c => c.OutsideBarcode).Distinct();//根据订单号根据订单号、类型、分屏号查找包装打印外箱列表(用于显示完成数量的分子显示)
                        int printBarcode = printBarcodeinfo.Count();
                        //类型
                        info.Add("type", item);
                        //完成数量
                        info.Add("completeNum", printBarcode.ToString() + "/" + totleNum.ToString());
                        //屏序
                        info.Add("screenNum", screenNumitem);
                        //批次
                        info.Add("batchNum", batchnum);
                        //完成率
                        info.Add("complete", totleNum == 0 ? 0 + "%" : ((printBarcode * 100) / totleNum).ToString("F2") + "%");

                        total.Add(info);
                    }
                }
            }
            #region 计算总计
            JObject info2 = new JObject();
            var printBarcodeinfo2 = db.ModuleOutsideTheBox.Where(c => c.OrderNum == ordernum).Select(c => c.OutsideBarcode).Distinct().Count();//根据订单查包装打印的总的外箱数量
            var totle = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "外箱").ToList();
            var totleNum2 = 0;
            if (totle.Count != 0)
            {
                totleNum2 = totle.Sum(c => c.Quantity);//总的打印数量
            }
            #endregion
            //类型
            info2.Add("type", "总计");
            //完成数量
            info2.Add("completeNum", printBarcodeinfo2.ToString() + "/" + totleNum2.ToString());
            //屏序
            info2.Add("screenNum", "--");
            //批次
            info2.Add("batchNum", "--");
            //完成率
            info2.Add("complete", totleNum2 == 0 ? 0 + "%" : ((printBarcodeinfo2 * 100) / totleNum2).ToString("F2") + "%");

            total.Add(info2);
            result.Add("Complate", total);
            result.Add("pass", true);
            result.Add("mes", "查找成功");
            #endregion
            return com.GetModuleFromJobjet(result, true, "查找成功");
        }

        //检验条码是否规范
        //1.先判断订单是否是三级包装,即纸盒,纸箱,外箱.是三级包装的话,纸箱里面包的是纸盒,不是的话纸箱里面包的模块
        //2.根据传过过来的条码去找上一个包装工序的订单是什么,例如外箱工序要去找纸箱工序的订单,纸箱如果是三级包装则找纸盒,不是三级包装则找条码,纸盒的找条码.将找出来的订单和传过来的订单做对比,看是否相符
        //3.判断条码是否已经打印了,打印了则提示"此条码已打印",没有则返回成功
        [HttpPost]
        [ApiAuthorize]
        public JObject CheckBarcode([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string ordernum = data["ordernum"].ToString();
            string barcode = data["barcode"].ToString();
            string statue = data["statue"].ToString();
            bool hybrid = obj.hybrid == null ? false : obj.hybrid;

            bool pass = false;
            string mes = "";
            var order = "";
            var isthree = db.ModulePackageRule.Count(c => c.OrderNum == ordernum && c.Statue == "纸盒");//是否是三级包装,是三级包装的话,内箱里的条码要找纸盒的外箱条码
            if (statue == "外箱")
            {
                order = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == barcode && c.Statue == "纸箱").Select(c => c.OrderNum).FirstOrDefault();//根据条码在条码表找订单
            }
            else if (statue == "纸箱" && isthree == 0)
            {
                order = db.BarCodes.Where(c => c.BarCodesNum == barcode && c.BarCodeType == "模块").Select(c => c.OrderNum).FirstOrDefault();//根据条码在条码表找订单
            }
            else if (statue == "纸箱" && isthree != 0)
            {
                order = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == barcode && c.Statue == "纸盒").Select(c => c.OrderNum).FirstOrDefault();//根据条码在内箱条码表找订单
            }
            else if (statue == "纸盒")
            {
                order = db.BarCodes.Where(c => c.BarCodesNum == barcode && c.BarCodeType == "模块").Select(c => c.OrderNum).FirstOrDefault();//根据条码在内箱条码表找订单
            }
            var exit = db.ModuleAppearance.Count(c => c.ModuleBarcode == barcode && c.AppearanceResult == true);//根据条码查找完成电检数量
            //查找内检完的数量
            if (order == null)
            {
                mes = "不存在此条码";
                pass = false;
            }
            else if (order != ordernum && hybrid == false)
            {
                mes = "此条码的订单号应为" + order;
                pass = false;
            }
            else
            {
                string Printingexit = "";
                if (statue == "外箱")
                {
                    Printingexit = db.ModuleOutsideTheBox.Where(c => c.InnerBarcode == barcode).Select(c => c.InnerBarcode).FirstOrDefault();//查找打印表里是否有相同的条码号
                }
                if (statue == "纸箱")
                {
                    Printingexit = db.ModuleInsideTheBox.Where(c => c.ModuleBarcode == barcode && c.Statue == "纸箱").Select(c => c.ModuleBarcode).FirstOrDefault();//查找打印表里是否有相同的条码号
                }
                if (statue == "纸盒")
                {
                    Printingexit = db.ModuleInsideTheBox.Where(c => c.ModuleBarcode == barcode && c.Statue == "纸盒").Select(c => c.ModuleBarcode).FirstOrDefault();//查找打印表里是否有相同的条码号
                }
                if (!string.IsNullOrEmpty(Printingexit))//有就传fasle
                {
                    mes = "此条码已打印 ";
                    pass = false;
                }
                else
                {
                    mes = "成功 ";
                    pass = true;
                }

            }
            return com.GetModuleFromJobjet(null, pass, mes);
        }


        /// <summary>
        /// 创建外箱打印记录
        /// </summary>
        /// 首先检查需要录入外箱打印表里面的条码号是否重复,重复则提示错误,然后再找订单定义的总数量,将已打印的数量+此次要添加记录的数量,等到的数量与定义总数量做对比,如果定义总数量小于打印数量,则提示错误,否则添加数据
        /// <param name="printings">外箱打印记录</param>
        /// <param name="ordernum">订单号</param>
        /// <param name="isupdate">是否是更新</param>
        /// <returns></returns>

        [HttpPost]
        [ApiAuthorize]
        public JObject PackangPrint([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            List<ModuleOutsideTheBox> printings = data["printings"].ToObject<List<ModuleOutsideTheBox>>();//list对象
            bool IsLast = obj.IsLast == null ? false : obj.IsLast;//是否是尾箱
            string ordernum = data["ordernum"].ToString();//订单
            string Department = data["Department"].ToString();//部门
            string Group = data["Group"].ToString();//班组
            string UserName = data["UserName"].ToString();//用户名

            JObject result = new JObject();
            if (ModelState.IsValid)//判断数据格式是否正确
            {
                string error = "";

                var type = printings.Select(c => c.Type.ToString()).FirstOrDefault();
                var aa = type.Substring(2);
                var num = int.Parse(type.Substring(2));
                if (printings.Count != num && !IsLast)
                {
                    return com.GetModuleFromJobjet(null, false, "包装数量有与规则不符");
                }
                foreach (var item in printings)//循环传过来的打印数据集合
                {
                    var exit = db.ModuleOutsideTheBox.Where(c => c.InnerBarcode == item.InnerBarcode.ToString()).FirstOrDefault();//查看条码是否已经打印了
                    if (exit != null)//已经打印,将信息记录在error中
                    {
                        error = error + exit.InnerBarcode + "已包装在" + exit.OutsideBarcode + ",";
                    }
                }
                if (!string.IsNullOrEmpty(error))//如果error含有数据,则显示数据
                {
                    return com.GetModuleFromJobjet(null, false, error);
                }

                else//代表没有重复打印的
                {
                    // var temp = printings.FirstOrDefault().OrderNum;
                    var count = db.ModuleOutsideTheBox.Count(c => c.OrderNum == ordernum);//根据订单找打印表数量,代表此订单 已打印的数量
                    var real = db.ModulePackageRule.Where(c => c.OrderNum == ordernum);//查看此订单是否有定义装箱规则
                    int realCount = 0;
                    foreach (var item in real)//循环
                    {
                        realCount = realCount + (item.Quantity * item.OuterBoxCapacity);//计算出总共应该打印多少条码数(装箱容量*装箱数)
                    }
                    if (realCount < count + printings.Count())//如果已打印的数量大于定义总数量
                    {
                        return com.GetModuleFromJobjet(null, false, "已超过定义的包装数量");
                    }
                    printings.ForEach(c => { c.Department = Department; c.Group = Group; c.InnerTime = DateTime.Now; c.Inneror = UserName; });//保存数据
                    db.ModuleOutsideTheBox.AddRange(printings);
                    db.SaveChanges();
                    UpdateNewTable(ordernum, "外箱", DateTime.Now);
                    return com.GetModuleFromJobjet(null, true, "打印成功");
                }
            }
            return com.GetModuleFromJobjet(null, false, "传入类型不对，请确认");
        }

        //列表,根据订单号找到类型
        [HttpPost]
        [ApiAuthorize]
        public JObject TypeList([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();//订单
            string statue = data["statue"].ToString();//装箱款式
            var orders = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).Select(m => m.Type).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return com.GetModuleFromJarray(result);
        }

        //列表,根据订单号,类型找到批号
        [HttpPost]
        [ApiAuthorize]
        public JObject BatchList([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();//订单
            string type = data["type"].ToString();//类型
            string statue = data["statue"].ToString();//装箱款式
            var orders = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == type && c.Statue == statue).Select(m => m.Batch).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return com.GetModuleFromJarray(result);
        }

        //列表,根据订单号,类型,批号找到屏号
        [HttpPost]
        [ApiAuthorize]
        public JObject ScreenList([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();//订单
            string type = data["type"].ToString();//类型
            string statue = data["statue"].ToString();//装箱款式
            int batch = int.Parse(data["batch"].ToString());//批次
            var orders = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == type && c.Batch == batch && c.Statue == statue).Select(m => m.ScreenNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return com.GetModuleFromJarray(result);
        }
        #endregion

        #region 内外箱共同操作

        //内外箱查看
        //1.如果有根绝箱体条码筛选,则直接得到箱体条码列表
        //2.否则,如果是纸箱或纸盒,则在内箱表里根据订单和装箱款式找到已打印的箱体列表,并去除掉已经打印外箱的条码列表
        //3.如果是外箱,则在外箱表里找到已打印的箱体列表,并去除掉已经入库的条码列表
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayPackage([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();//订单
            string packageBarcode = data["packageBarcode"].ToString();//箱体条码
            string statu = data["statu"].ToString();//装箱款式
            JArray Bacodelist = new JArray();
            if (!string.IsNullOrEmpty(packageBarcode))
            {
                Bacodelist.Add(packageBarcode);
                return com.GetModuleFromJarray(Bacodelist);
            }
            if (statu == "纸箱" || statu == "纸盒")
            {
                var barcodelist = db.ModuleInsideTheBox.OrderBy(c => c.InnerBarcode).Where(c => c.OrderNum == ordernum && c.Statue == statu).Select(c => c.InnerBarcode).ToList();

                barcodelist = barcodelist.Distinct<string>().ToList();
                barcodelist.ForEach(c => Bacodelist.Add(c));
                return com.GetModuleFromJarray(Bacodelist);
            }
            else
            {
                var barcodelist = db.ModuleOutsideTheBox.OrderBy(c => c.OutsideBarcode).Where(c => c.OrderNum == ordernum).Select(c => c.OutsideBarcode).ToList();
                barcodelist = barcodelist.Distinct<string>().ToList();
                barcodelist.ForEach(c => Bacodelist.Add(c));
                return com.GetModuleFromJarray(Bacodelist);
            }
        }

        //内外箱删除列表显示
        //1.如果有根绝箱体条码筛选,则直接得到箱体条码列表
        //2.否则,如果是纸箱或纸盒,则在内箱表里根据订单和装箱款式找到已打印的箱体列表
        //3.如果是外箱,则在外箱表里找到已打印的箱体列表
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayDeletePackage([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();//订单
            string packageBarcode = data["packageBarcode"].ToString();//箱体条码
            string statu = data["statu"].ToString();//装箱款式
            JArray Bacodelist = new JArray();
            if (!string.IsNullOrEmpty(packageBarcode))
            {
                Bacodelist.Add(packageBarcode);
                return com.GetModuleFromJarray(Bacodelist);
            }
            if (statu == "纸箱" || statu == "纸盒")
            {
                var barcodelist = db.ModuleInsideTheBox.Where(c => c.OrderNum == ordernum && c.Statue == statu).Select(c => c.InnerBarcode).Distinct().ToList();

                var outside = db.ModuleOutsideTheBox.Where(c => c.OrderNum == ordernum).Select(c => c.InnerBarcode).Distinct().ToList();
                var result = barcodelist.Except(outside).ToList();
                result.ForEach(c => Bacodelist.Add(c));
                return com.GetModuleFromJarray(Bacodelist);
            }
            else
            {
                var barcodelist = db.ModuleOutsideTheBox.Where(c => c.OrderNum == ordernum).Select(c => c.OutsideBarcode).Distinct().ToList();
                var warehouse = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.State == "模块").Select(c => c.OuterBoxBarcode
                ).Distinct().ToList();
                var result = barcodelist.Except(warehouse).ToList();
                result.ForEach(c => Bacodelist.Add(c));

                return com.GetModuleFromJarray(Bacodelist);
            }
        }

        //内外箱删除方法
        //1.如果是纸盒或者纸箱,移除掉包含条码列表的内箱记录
        //2.如果是外选哪个,移除掉包含条码的外箱记录
        [HttpPost]
        [ApiAuthorize]
        public void PackageDelete([System.Web.Http.FromBody]JObject data)
        {
            string statu = data["statu"].ToString();//装箱款式
            var packageBarcode = data["statu"].ToList();//条码列表
            if (statu == "纸箱" || statu == "纸盒")
            {
                var list = db.ModuleInsideTheBox.Where(c => packageBarcode.Contains(c.InnerBarcode)).ToList();
                db.ModuleInsideTheBox.RemoveRange(list);
                db.SaveChanges();
                UpdateNewTable(list.FirstOrDefault().OrderNum, statu, DateTime.Now);
            }
            else
            {
                var list = db.ModuleOutsideTheBox.Where(c => packageBarcode.Contains(c.OutsideBarcode)).ToList();
                db.ModuleOutsideTheBox.RemoveRange(list);
                db.SaveChanges();
                UpdateNewTable(list.FirstOrDefault().OrderNum, statu, DateTime.Now);
            }

        }
        #endregion
        //列表选择
        [HttpPost]
        [ApiAuthorize]
        public JObject OrderNumList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Where(c => c.Models != 0).Select(m => m.OrderNum).ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return com.GetModuleFromJarray(result);
        }

        #region 模块外箱标签打印
        #region---包装打印标签
        //打印标签
        //1.先判断叠加备品配件数量是否为0,不为0则往sntn的分母叠加@

        [HttpPost]
        [ApiAuthorize]
        public JObject OutsideBoxLablePrint([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string ordernum = obj.ordernum;//订单
            string packagingordernum = obj.packagingordernum;//显示单号
            string outsidebarcode = obj.outsidebarcode;//外箱条码
            string material_discription = obj.material_discription;//物料描述
            string sntn = obj.sntn;//件数/号
            string qty = obj.qty;//数量10psc
            string ip = obj.ip;//打印机地址
            string leng = obj.leng;//语言选择
            int screennum = obj.screennum == null ? 1 : obj.screennum;//屏序号
            int pagecount = obj.pagecount == null ? 1 : obj.pagecount;//打印数量
            int port = obj.port;//端口
            int concentration = obj.concentration == null ? 5 : obj.concentration;//打印浓度
            int BPPJNum = obj.BPPJNum == null ? 0 : obj.BPPJNum;//叠加备品配件数量
            double g_Weight = obj.g_Weight;//毛重
            double n_Weight = obj.n_Weight;//净重
            bool logo = obj.logo == null ? true : obj.logo;//是否有LOGO
            string version = obj.version == null ? "old" : obj.version;//新版打印还是旧版打印
            var mn_list = data["mn_list"].ToObject<string[]>();
            if (BPPJNum != 0)
            {
                var array = sntn.Split('/');
                sntn = array[0] + "/" + (int.Parse(array[1]) + BPPJNum);
            }
            if (!String.IsNullOrEmpty(packagingordernum)) ordernum = packagingordernum;//如果有包装新订单号，则使用包装新订单号。
            if (version == "new")
            {
                var bm = CreateOutsideBoxLableNewVersion(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
                string value = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                value += totalbytes + "," + rowbytes + "," + hex;
                value += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(value.ToString(), pagecount, ip, port);
                return com.GetModuleFromJobjet(null, null, "打印成功");
            }
            else
            {
                var bm = OutsideBoxLableCreate(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
                string value = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                value += totalbytes + "," + rowbytes + "," + hex;
                value += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(value.ToString(), pagecount, ip, port);
                return com.GetModuleFromJobjet(null, null, "打印成功");
            }

        }
        #endregion

        #region---生成标签图片
        //生成标签
        public Bitmap OutsideBoxLableCreate(string[] mn_list, string ordernum = "", string outsidebarcode = "", string material_discription = "", string sntn = "", string qty = "", bool logo = true, int screennum = 1, double? g_Weight = null, double? n_Weight = null, string leng = "")
        {
            //开始绘制图片
            int initialWidth = 750, initialHeight = 1000;//高4宽3
            Bitmap AllBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(AllBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            //double beishuhege = 0.37;
            Pen pen = new Pen(Color.Black, 3);
            theGraphics.DrawRectangle(pen, 50, 50, 640, 890);
            //横线
            theGraphics.DrawLine(pen, 50, 160, 690, 160);
            theGraphics.DrawLine(pen, 50, 280, 690, 280);
            theGraphics.DrawLine(pen, 50, 340, 690, 340);
            theGraphics.DrawLine(pen, 50, 400, 690, 400);
            theGraphics.DrawLine(pen, 50, 460, 690, 460);
            //竖线
            theGraphics.DrawLine(pen, 250, 280, 250, 460);
            theGraphics.DrawLine(pen, 400, 280, 400, 460);
            theGraphics.DrawLine(pen, 570, 280, 570, 460);

            //引入LOGO
            if (logo)
            {
                Bitmap bmp_logo = new Bitmap(@"D:\\MES_Data\\LOGO_black.png");
                //double beishulogo = 0.95;
                theGraphics.DrawImage(bmp_logo, 65, 60, (float)(bmp_logo.Width), (float)(bmp_logo.Height));
                //引入订单号
                System.Drawing.Font myFont_ordernum;
                myFont_ordernum = new System.Drawing.Font("Microsoft YaHei UI", 40, FontStyle.Bold);
                StringFormat geshi = new StringFormat();
                geshi.Alignment = StringAlignment.Center; //居中
                                                          //geshi.Alignment = StringAlignment.Far; //右对齐
                theGraphics.DrawString(ordernum, myFont_ordernum, bush, 230, 90);
            }
            //引入订单号
            else
            {
                System.Drawing.Font myFont_ordernum;
                myFont_ordernum = new System.Drawing.Font("Microsoft YaHei UI", 55, FontStyle.Bold);
                StringFormat geshi = new StringFormat();
                geshi.Alignment = StringAlignment.Center; //居中
                theGraphics.DrawString(ordernum, myFont_ordernum, bush, 100, 60);
            }
            //引入条码
            //if (String.IsNullOrEmpty(outsidebarcode)) return Content("条码号为空！");
            Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(outsidebarcode, 600, 70);
            theGraphics.DrawImage(bmp_barcode, 70, 170, (float)bmp_barcode.Width, (float)bmp_barcode.Height);

            //引入条码号
            System.Drawing.Font myFont_boxbarcode;
            myFont_boxbarcode = new System.Drawing.Font("Microsoft YaHei UI", 22, FontStyle.Bold);
            theGraphics.DrawString(outsidebarcode, myFont_boxbarcode, bush, 200, 240);

            #region--未加净重毛重前版本
            ////引入物料描述
            //System.Drawing.Font myFont_material_discription;
            //myFont_material_discription = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString("物料描述(DESC)", myFont_material_discription, bush, 55, 295);

            ////引入物料描述内容
            //System.Drawing.Font myFont_material_discription_content;
            //StringFormat geshi1 = new StringFormat();
            //geshi1.Alignment = StringAlignment.Center; //居中
            //myFont_material_discription_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString(material_discription, myFont_material_discription_content, bush, 275, 295);

            ////引入屏序号
            //System.Drawing.Font myFont_screennum;
            //myFont_screennum = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString("屏序号(NO.)", myFont_screennum, bush, 410, 295);

            ////引入屏序号值
            //System.Drawing.Font myFont_screennum_data;
            //StringFormat geshi2 = new StringFormat();
            //geshi1.Alignment = StringAlignment.Center; //居中
            //myFont_screennum_data = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString(screennum.ToString(), myFont_screennum_data, bush, 610, 295);

            ////引入SN/TN
            //System.Drawing.Font myFont_sntn;
            //myFont_sntn = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString("件号/数(SN/TN)", myFont_sntn, bush, 55, 355);

            ////引入SN/TN内容
            //System.Drawing.Font myFont_sntn_content;
            //myFont_sntn_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString(sntn, myFont_sntn_content, bush, 290, 355);

            ////引入数量QTY
            //System.Drawing.Font myFont_qty;
            //myFont_qty = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString("数量(QTY)", myFont_qty, bush, 410, 355);

            ////引入数量QTY内容
            //System.Drawing.Font myFont_qty_content;
            //myFont_qty_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString(qty + " PCS", myFont_qty_content, bush, 585, 355);

            ////引入模组号清单
            //int mn_E_count = mn_list.Count();
            ////12位模组号以上，包括条码号
            //if (mn_E_count > 12 && mn_E_count <= 20)
            //{
            //    if (mn_list[0].Length < 7)
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 28, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 70;
            //        for (int i = 1; i < mn_list.Count() + 1; i++)
            //        {
            //            theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 4) != 0)
            //            {
            //                left_x += 155;
            //            }
            //            else
            //            {
            //                top_y += 100;
            //                left_x -= 465;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 22, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 55;
            //        for (int i = 0; i < mn_list.Count(); i++)
            //        {
            //            theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 2) == 0)
            //            {
            //                left_x += 315;
            //            }
            //            else
            //            {
            //                top_y += 50;
            //                left_x -= 315;
            //            }
            //        }
            //    }
            //}
            ////11-12位模组号
            //else if (mn_E_count > 10 && mn_E_count <= 12)
            //{
            //    if (mn_list[0].Length < 7)
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 50, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 90;
            //        for (int i = 0; i < mn_list.Count(); i++)
            //        {
            //            theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 2) == 0)
            //            {
            //                left_x += 290;
            //            }
            //            else
            //            {
            //                top_y += 85;
            //                left_x -= 290;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 26, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 160;
            //        for (int i = 0; i < mn_list.Count(); i++)
            //        {
            //            theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            top_y += 42;
            //        }
            //    }
            //}
            ////9-10位模组号
            //else if (mn_E_count > 8 && mn_E_count <= 10)
            //{
            //    if (mn_list[0].Length < 7)
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 45, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 90;
            //        for (int i = 0; i < mn_list.Count(); i++)
            //        {
            //            theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 2) == 0)
            //            {
            //                left_x += 290;
            //            }
            //            else
            //            {
            //                top_y += 100;
            //                left_x -= 290;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 30, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 150;
            //        for (int i = 0; i < mn_list.Count(); i++)
            //        {
            //            theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            top_y += 50;
            //        }

            //    }

            //}
            ////7-8位模组号
            //else if (mn_E_count > 6 && mn_E_count <= 8)
            //{
            //    if (mn_list[0].Length < 7)
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 50, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 80;
            //        for (int i = 1; i < mn_list.Count() + 1; i++)
            //        {
            //            theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 2) != 0)
            //            {
            //                left_x += 300;
            //            }
            //            else
            //            {
            //                top_y += 126;
            //                left_x -= 300;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 36, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 410;
            //        int left_x = 110;
            //        for (int i = 1; i < mn_list.Count() + 1; i++)
            //        {
            //            theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            top_y += 65;
            //        }
            //    }
            //}
            ////1-6位模组号
            //else if (mn_E_count <= 6)
            //{
            //    if (mn_list[0].Length < 7)
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 50, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 450;
            //        int left_x = 80;
            //        for (int i = 1; i < mn_list.Count() + 1; i++)
            //        {
            //            theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 2) != 0)
            //            {
            //                left_x += 300;
            //            }
            //            else
            //            {
            //                top_y += 150;
            //                left_x -= 300;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 40, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 80;
            //        for (int i = 1; i < mn_list.Count() + 1; i++)
            //        {
            //            theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            top_y += 85;
            //        }

            //    }
            //}
            //else
            //{
            //    if (mn_list[0].Length < 7)
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 28, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 70;
            //        for (int i = 1; i < mn_list.Count() + 1; i++)
            //        {
            //            theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 4) != 0)
            //            {
            //                left_x += 155;
            //            }
            //            else
            //            {
            //                top_y += 50;
            //                left_x -= 465;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        System.Drawing.Font myFont_modulenum_list;
            //        myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 16, FontStyle.Bold);
            //        StringFormat listformat = new StringFormat();
            //        listformat.Alignment = StringAlignment.Near;
            //        int top_y = 420;
            //        int left_x = 90;
            //        for (int i = 0; i < mn_list.Count(); i++)
            //        {
            //            theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //            if ((i % 2) == 0)
            //            {
            //                left_x += 315;
            //            }
            //            else
            //            {
            //                top_y += 26;
            //                left_x -= 315;
            //            }
            //        }
            //    }
            //}
            #endregion

            //引入毛重量
            System.Drawing.Font myFont_g_Weight;
            myFont_g_Weight = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            System.Drawing.Font myFont_n_Weight;
            myFont_n_Weight = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            System.Drawing.Font myFont_material_discription;
            myFont_material_discription = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            System.Drawing.Font myFont_screennum;
            myFont_screennum = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            System.Drawing.Font myFont_sntn;
            myFont_sntn = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            System.Drawing.Font myFont_qty;
            myFont_qty = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            if (leng == "简")
            {
                theGraphics.DrawString("毛重量(kg)", myFont_g_Weight, bush, 55, 295);
                theGraphics.DrawString("净重(kg)", myFont_n_Weight, bush, 410, 295);
                theGraphics.DrawString("物料描述", myFont_material_discription, bush, 55, 355);
                theGraphics.DrawString("屏序号", myFont_screennum, bush, 410, 355);
                theGraphics.DrawString("件号/数", myFont_sntn, bush, 55, 415);
                theGraphics.DrawString("数量", myFont_qty, bush, 410, 415);
            }
            else if (leng == "英")
            {
                theGraphics.DrawString("G.W.(kg)", myFont_g_Weight, bush, 55, 295);
                theGraphics.DrawString("N.W.(kg)", myFont_n_Weight, bush, 410, 295);
                theGraphics.DrawString("DESC", myFont_material_discription, bush, 55, 355);
                theGraphics.DrawString("NO.", myFont_screennum, bush, 410, 355);
                theGraphics.DrawString("SN/TN", myFont_sntn, bush, 55, 415);
                theGraphics.DrawString("QTY", myFont_qty, bush, 410, 415);
            }
            else if (leng == "繁")
            {
                theGraphics.DrawString("毛重量(kg)", myFont_g_Weight, bush, 55, 295);
                theGraphics.DrawString("淨重(kg)", myFont_n_Weight, bush, 410, 295);
                theGraphics.DrawString("物料描述", myFont_material_discription, bush, 55, 355);
                theGraphics.DrawString("屏序號", myFont_screennum, bush, 410, 355);
                theGraphics.DrawString("件號/數", myFont_sntn, bush, 55, 415);
                theGraphics.DrawString("數量", myFont_qty, bush, 410, 415);
            }
            else if (leng == "繁/英")
            {
                theGraphics.DrawString("毛重量(G.W.)kg", myFont_g_Weight, bush, 55, 295);
                theGraphics.DrawString("淨重(N.W.)kg", myFont_n_Weight, bush, 410, 295);
                theGraphics.DrawString("物料描述(DESC)", myFont_material_discription, bush, 55, 355);
                theGraphics.DrawString("屏序號(NO.)", myFont_screennum, bush, 410, 355);
                theGraphics.DrawString("件號/數(SN/TN)", myFont_sntn, bush, 55, 415);
                theGraphics.DrawString("數量(QTY)", myFont_qty, bush, 410, 415);
            }
            else
            {
                theGraphics.DrawString("毛重量(G.W.)kg", myFont_g_Weight, bush, 55, 295);
                theGraphics.DrawString("净重(N.W.)kg", myFont_n_Weight, bush, 410, 295);
                theGraphics.DrawString("物料描述(DESC)", myFont_material_discription, bush, 55, 355);
                theGraphics.DrawString("屏序号(NO.)", myFont_screennum, bush, 410, 355);
                theGraphics.DrawString("件号/数(SN/TN)", myFont_sntn, bush, 55, 415);
                theGraphics.DrawString("数量(QTY)", myFont_qty, bush, 410, 415);
            }

            //引入毛重量值
            System.Drawing.Font myFont_g_Weight_content;
            StringFormat geshi1 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            myFont_g_Weight_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);

            //double G_Weight = g_Weight == null ? 0 : (double)g_Weight;           
            theGraphics.DrawString(g_Weight.ToString(), myFont_g_Weight_content, bush, 280, 295);


            //引入净重值
            System.Drawing.Font myFont_n_Weight_content;
            StringFormat geshi2 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            myFont_n_Weight_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //double N_Weight = n_Weight == null ? 0 : (double)n_Weight;
            theGraphics.DrawString(n_Weight.ToString(), myFont_n_Weight_content, bush, 600, 295);

            //引入物料描述内容
            if (leng == "英")
            {
                System.Drawing.Font myFont_material_discription_content;
                myFont_material_discription_content = new System.Drawing.Font("Microsoft YaHei UI", 15, FontStyle.Regular);
                theGraphics.DrawString(material_discription, myFont_material_discription_content, bush, 265, 355);
            }
            else
            {
                System.Drawing.Font myFont_material_discription_content;
                myFont_material_discription_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString(material_discription, myFont_material_discription_content, bush, 275, 355);
            }


            //引入屏序号值
            System.Drawing.Font myFont_screennum_data;
            StringFormat geshi3 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            myFont_screennum_data = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(screennum.ToString(), myFont_screennum_data, bush, 615, 355);


            //引入SN/TN内容
            System.Drawing.Font myFont_sntn_content;
            myFont_sntn_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(sntn, myFont_sntn_content, bush, 290, 415);


            //引入数量QTY内容
            System.Drawing.Font myFont_qty_content;
            myFont_qty_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(qty + " PCS", myFont_qty_content, bush, 585, 415);

            //引入模组号清单
            int mn_E_count = mn_list.Count();
            //12位模组号以上，包括条码号
            if (mn_E_count > 5 && mn_E_count <= 10)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 470;
                int left_x = 50;
                for (int i = 0; i < mn_list.Count(); i++)
                {
                    theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 2) == 0)
                    {
                        left_x += 325;
                    }
                    else
                    {
                        top_y += 50;
                        left_x = 50;
                    }
                }

            }
            //11-12位模组号
            else if (mn_E_count <= 5)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 30, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 485;
                int left_x = 180;
                for (int i = 0; i < mn_list.Count(); i++)
                {
                    theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);

                    top_y += 90;
                }

            }
            //组织数据
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
        }
        #endregion

        #region---生成标签图片新版本
        //生成标签
        public Bitmap CreateOutsideBoxLableNewVersion(string[] mn_list, string ordernum, string outsidebarcode, string material_discription, string sntn, string qty, bool logo = true, int screennum = 1, double? g_Weight = null, double? n_Weight = null, string leng = "")
        {
            //开始绘制图片
            int initialWidth = 750, initialHeight = 1000;//高4宽3
            Bitmap AllBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(AllBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            //double beishuhege = 0.37;
            Pen pen = new Pen(Color.Black, 3);
            theGraphics.DrawRectangle(pen, 50, 50, 640, 850);
            //横线
            theGraphics.DrawLine(pen, 50, 108, 690, 108);//起点x,起点y坐标，终点x,终点y坐标
            theGraphics.DrawLine(pen, 50, 175, 690, 175);
            theGraphics.DrawLine(pen, 50, 242, 690, 242);
            theGraphics.DrawLine(pen, 50, 308, 690, 308);
            theGraphics.DrawLine(pen, 50, 375, 690, 375);
            theGraphics.DrawLine(pen, 50, 441, 690, 441);
            theGraphics.DrawLine(pen, 50, 570, 690, 570);
            //竖线
            theGraphics.DrawLine(pen, 250, 50, 250, 242);
            theGraphics.DrawLine(pen, 400, 108, 400, 242);
            theGraphics.DrawLine(pen, 570, 108, 570, 242);

            theGraphics.DrawLine(pen, 250, 308, 250, 441);
            theGraphics.DrawLine(pen, 400, 308, 400, 441);
            theGraphics.DrawLine(pen, 570, 308, 570, 441);

            System.Drawing.Font myFont_g_Weight;
            myFont_g_Weight = new System.Drawing.Font("Microsoft YaHei UI", 13, FontStyle.Regular);

            if (leng == "简")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                System.Drawing.Font myFont2;
                myFont2 = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Regular);
                theGraphics.DrawString("订单号", myFont2, bush, 60, 60);

                theGraphics.DrawString("规格型号", myFont2, bush, 60, 120);

                theGraphics.DrawString("颜色", myFont2, bush, 405, 120);

                theGraphics.DrawString("数量", myFont2, bush, 60, 190);

                theGraphics.DrawString("件号", myFont2, bush, 405, 190);

                theGraphics.DrawString("物料描述", myFont2, bush, 60, 320);

                theGraphics.DrawString("屏序号", myFont2, bush, 405, 320);

                theGraphics.DrawString("毛重 kg", myFont2, bush, 60, 385);

                theGraphics.DrawString("净重 kg", myFont2, bush, 405, 385);

            }
            else if (leng == "简/英" || leng == "")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 13, FontStyle.Regular);
                System.Drawing.Font myFont2;
                myFont2 = new System.Drawing.Font("Microsoft YaHei UI", 15, FontStyle.Regular);
                theGraphics.DrawString("订单号 P0#", myFont2, bush, 60, 60);

                theGraphics.DrawString("规格型号ITEM NO", myFont2, bush, 60, 120);

                theGraphics.DrawString("颜色 COLOURS", myFont2, bush, 405, 120);

                theGraphics.DrawString("数量 QTY(CTNS/PCS)", myFont, bush, 60, 190);

                theGraphics.DrawString("件号 PACKAGE#", myFont2, bush, 405, 190);

                theGraphics.DrawString("物料描述(DESC)", myFont2, bush, 60, 320);

                theGraphics.DrawString("屏序号(NO.)", myFont2, bush, 405, 320);

                theGraphics.DrawString("毛重(G.W)kg", myFont2, bush, 60, 385);

                theGraphics.DrawString("净重(N.W)kg", myFont2, bush, 405, 385);
            }
            else if (leng == "繁")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                System.Drawing.Font myFont2;
                myFont2 = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Regular);
                theGraphics.DrawString("訂單號", myFont2, bush, 60, 60);

                theGraphics.DrawString("規格型號", myFont2, bush, 60, 120);

                theGraphics.DrawString("顏色", myFont2, bush, 405, 120);

                theGraphics.DrawString("數量", myFont2, bush, 60, 190);

                theGraphics.DrawString("件號", myFont2, bush, 405, 190);

                theGraphics.DrawString("物料描述", myFont, bush, 60, 320);

                theGraphics.DrawString("屏序號", myFont2, bush, 405, 320);

                theGraphics.DrawString("毛重 kg", myFont2, bush, 60, 385);

                theGraphics.DrawString("淨重 kg", myFont2, bush, 405, 385);
            }
            else if (leng == "繁/英")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 13, FontStyle.Regular);
                System.Drawing.Font myFont2;
                myFont2 = new System.Drawing.Font("Microsoft YaHei UI", 15, FontStyle.Regular);
                theGraphics.DrawString("訂單號 P0#", myFont2, bush, 60, 60);

                theGraphics.DrawString("規格型號ITEM NO", myFont2, bush, 60, 120);

                theGraphics.DrawString("顏色 COLOURS", myFont2, bush, 405, 120);

                theGraphics.DrawString("數量 QTY(CTNS/PCS)", myFont, bush, 60, 190);

                theGraphics.DrawString("件號 PACKAGE#", myFont2, bush, 405, 190);

                theGraphics.DrawString("物料描述(DESC)", myFont2, bush, 60, 320);

                theGraphics.DrawString("屏序號(NO.)", myFont2, bush, 405, 320);

                theGraphics.DrawString("毛重(G.W)kg", myFont2, bush, 60, 385);

                theGraphics.DrawString("淨重(N.W)kg", myFont2, bush, 405, 385);
            }
            else if (leng == "英")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 17, FontStyle.Regular);
                System.Drawing.Font myFont2;
                myFont2 = new System.Drawing.Font("Microsoft YaHei UI", 21, FontStyle.Regular);
                theGraphics.DrawString("P0#", myFont2, bush, 60, 60);

                theGraphics.DrawString("ITEM NO", myFont, bush, 60, 120);

                theGraphics.DrawString("COLOURS", myFont, bush, 405, 120);

                theGraphics.DrawString("QTY(CTNS/PCS)", myFont, bush, 60, 190);

                theGraphics.DrawString("PACKAGE#", myFont, bush, 405, 190);

                theGraphics.DrawString("DESC", myFont2, bush, 60, 320);

                theGraphics.DrawString("NO.", myFont2, bush, 405, 320);

                theGraphics.DrawString("(G.W)kg", myFont2, bush, 60, 385);

                theGraphics.DrawString("(N.W)kg", myFont2, bush, 405, 385);
            }
            //订单
            System.Drawing.Font value;
            value = new System.Drawing.Font("Microsoft YaHei UI", 15, FontStyle.Regular);
            System.Drawing.Font value2;
            value2 = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Regular);
            StringFormat geshi = new StringFormat();
            geshi.Alignment = StringAlignment.Center; //居中
            //geshi.LineAlignment = StringAlignment.Center; //居中
            theGraphics.DrawString(ordernum, value2, bush, 320, 60);

            //规格型号
            var info = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "外箱").FirstOrDefault();
            theGraphics.DrawString(info.ITEMNO, value2, bush, 258, 120);
            //颜色
            theGraphics.DrawString(info.COLOURS, value, bush, 575, 120);
            //数量
            string boxnum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "外箱").Sum(c => c.Quantity * c.OuterBoxCapacity).ToString();
            theGraphics.DrawString(qty + "/" + boxnum, value2, bush, 258, 190);
            //件号
            string[] s = sntn.Split('/');
            theGraphics.DrawString(s[0] + " OF " + s[1], value, bush, 575, 190);
            //备注
            theGraphics.DrawString(info.Remark, value2, bush, 215, 260);
            //物料描述
            if (leng == "英")
            {
                System.Drawing.Font value3;
                value3 = new System.Drawing.Font("Microsoft YaHei UI", 15, FontStyle.Regular);
                theGraphics.DrawString(material_discription, value3, bush, 255, 325);
            }
            else
            {
                theGraphics.DrawString(material_discription, value2, bush, 258, 320);
            }
            //屏序号
            theGraphics.DrawString(screennum.ToString(), value2, bush, 575, 320);
            //毛重
            theGraphics.DrawString(g_Weight.ToString(), value2, bush, 258, 385);
            //净重
            theGraphics.DrawString(n_Weight.ToString(), value2, bush, 575, 385);
            //条形码
            Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(outsidebarcode, 600, 70);
            theGraphics.DrawImage(bmp_barcode, 70, 450, (float)bmp_barcode.Width, (float)bmp_barcode.Height);

            //引入条码号
            System.Drawing.Font myFont_boxbarcode;
            myFont_boxbarcode = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(outsidebarcode, myFont_boxbarcode, bush, 220, 530);

            //引入模组号清单
            int mn_E_count = mn_list.Count();
            //12位模组号以上，包括条码号
            if (mn_E_count > 5 && mn_E_count <= 10)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Regular);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 600;
                int left_x = 50;
                for (int i = 0; i < mn_list.Count(); i++)
                {
                    theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 2) == 0)
                    {
                        left_x += 325;
                    }
                    else
                    {
                        top_y += 50;
                        left_x = 50;
                    }
                }

            }
            //11-12位模组号
            else if (mn_E_count <= 5)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 30, FontStyle.Regular);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 600;
                int left_x = 180;
                for (int i = 0; i < mn_list.Count(); i++)
                {
                    theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);

                    top_y += 90;
                }

            }
            //组织数据
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
        }
        #endregion



        #region ---查看标签
        [HttpPost]
        [ApiAuthorize]
        public JObject OutsideBoxLablePrintToImg([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string outsidebarcode = obj.outsidebarcode;
            string leng = obj.leng;
            int BPPJNum = obj.BPPJNum == null ? 0 : obj.BPPJNum;

            JObject result = new JObject();
            var outsidebarcode_recordlist = db.ModuleOutsideTheBox.Where(c => c.OutsideBarcode == outsidebarcode);
            //如果有包装新订单号，则使用包装新订单号。
            var ordernum1 = db.ModuleOutsideTheBox.Where(c => c.OutsideBarcode == outsidebarcode).Select(c => new { c.OrderNum, c.EmbezzleOrdeNum }).FirstOrDefault();//订单和挪用订单
            string ordernum = string.IsNullOrEmpty(ordernum1.EmbezzleOrdeNum) ? ordernum1.OrderNum : ordernum1.EmbezzleOrdeNum;//如果挪用订单不为空，则显示挪用订单，否则显示本来订单号
            string packagingordernum = outsidebarcode_recordlist.FirstOrDefault().PackagingOrderNum;
            ordernum = String.IsNullOrEmpty(packagingordernum) ? ordernum : packagingordernum;
            result.Add("ordernum", ordernum);//显示的订单号

            result.Add("OutsideBarcode", outsidebarcode);//外箱条码号
            var screenNum = outsidebarcode_recordlist.FirstOrDefault().ScreenNum;
            result.Add("ScreenNum", screenNum);//屏序号
            string material_discription = outsidebarcode_recordlist.FirstOrDefault().Materiel;
            result.Add("Materiel", material_discription);//物料描述

            var batch = outsidebarcode_recordlist.FirstOrDefault().Batch;
            string sntn = "";
            if (BPPJNum != 0)
            {
                sntn = outsidebarcode_recordlist.FirstOrDefault().SN + "/" + (db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batch && c.Statue == "外箱").Sum(c => c.Quantity) + BPPJNum);
            }
            else
            {
                sntn = outsidebarcode_recordlist.FirstOrDefault().SN + "/" + db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batch && c.Statue == "外箱").Sum(c => c.Quantity);
            }
            result.Add("SNTN", sntn);//件号
            bool logo = outsidebarcode_recordlist.FirstOrDefault().IsLogo;
            result.Add("LOGO", logo);//logo
            double? g_Weight = outsidebarcode_recordlist.FirstOrDefault().G_Weight;
            result.Add("G_Weight", g_Weight);//毛重
            double? n_Weight = outsidebarcode_recordlist.FirstOrDefault().N_Weight;
            result.Add("N_Weight", n_Weight);//净重
            string[] mn_list = outsidebarcode_recordlist.Select(c => c.InnerBarcode).ToArray();
            result.Add("BarcodeList", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(mn_list)));//条码列表
            string qty = mn_list.Count().ToString();
            result.Add("Qty", qty);//数量
            var ruleinfo = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batch && c.Statue == "外箱").ToList();
            result.Add("itemno", ruleinfo.FirstOrDefault().ITEMNO);
            result.Add("COLOURS", ruleinfo.FirstOrDefault().COLOURS);
            result.Add("Remark", ruleinfo.FirstOrDefault().Remark);
            result.Add("BoxNum", ruleinfo.Sum(c => c.Quantity * c.OuterBoxCapacity));
            return com.GetModuleFromJobjet(result);
        }
        #endregion

        #region---重复打印
        [HttpPost]
        [ApiAuthorize]
        public JObject OutsideBoxLablePrintAgain([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string outsidebarcode = obj.outsidebarcode;//外箱条码
            string ip = obj.ip;//ip
            string leng = obj.leng;//语言
            int BPPJNum = obj.BPPJNum == null ? 0 : obj.BPPJNum; //备品叠加数量
            int pagecount = obj.pagecount == null ? 1 : obj.pagecount; //打印份数
            int concentration = obj.concentration == null ? 5 : obj.concentration; //打印浓度
            int port = obj.port == null ? 0 : obj.port; //端口
            string version = obj.version == null ? "old" : obj.version;//新版打印还是旧版打印

            var outsidebarcode_recordlist = db.ModuleOutsideTheBox.Where(c => c.OutsideBarcode == outsidebarcode);
            var screem = outsidebarcode_recordlist.FirstOrDefault().ScreenNum;
            var batch = outsidebarcode_recordlist.FirstOrDefault().Batch;
            var ordernum1 = db.ModuleOutsideTheBox.Where(c => c.OutsideBarcode == outsidebarcode).Select(c => new { c.OrderNum, c.EmbezzleOrdeNum }).FirstOrDefault();//订单和挪用订单
            string ordernum = string.IsNullOrEmpty(ordernum1.EmbezzleOrdeNum) ? ordernum1.OrderNum : ordernum1.EmbezzleOrdeNum;//如果挪用订单不为空，则显示挪用订单，否则显示本来订单号
            string type = outsidebarcode_recordlist.FirstOrDefault().Type;
            string material_discription = outsidebarcode_recordlist.FirstOrDefault().Materiel;
            if (leng == "英")
            {
                material_discription = "LED Modules";
            }
            string sntn = "";
            if (BPPJNum != 0)
            {
                sntn = outsidebarcode_recordlist.FirstOrDefault().SN + "/" + (db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.ScreenNum == screem && c.Batch == batch && c.Statue == "外箱").Sum(c => c.Quantity) + BPPJNum);
            }
            else
            {
                sntn = outsidebarcode_recordlist.FirstOrDefault().SN + "/" + db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.ScreenNum == screem && c.Batch == batch && c.Statue == "外箱").Sum(c => c.Quantity);
            }
            bool logo = outsidebarcode_recordlist.FirstOrDefault().IsLogo;
            double? g_Weight = outsidebarcode_recordlist.FirstOrDefault().G_Weight;
            double? n_Weight = outsidebarcode_recordlist.FirstOrDefault().N_Weight;
            string[] mn_list = outsidebarcode_recordlist.Select(c => c.InnerBarcode).ToArray();

            string qty = mn_list.Count().ToString();
            int screennum = screem;   //屏序号
                                      //如果有包装新订单号，则使用包装新订单号。
            string packagingordernum = outsidebarcode_recordlist.FirstOrDefault().PackagingOrderNum;
            ordernum = String.IsNullOrEmpty(packagingordernum) ? ordernum : packagingordernum;
            if (version == "new")
            {
                var bm = CreateOutsideBoxLableNewVersion(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string data1 = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data1 += totalbytes + "," + rowbytes + "," + hex;
                data1 += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                return com.GetModuleFromJobjet(null, null, "成功");
            }
            else
            {
                var bm = OutsideBoxLableCreate(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string data1 = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data1 += totalbytes + "," + rowbytes + "," + hex;
                data1 += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                return com.GetModuleFromJobjet(null, null, "成功");
            }
        }
        #endregion
        #endregion

        #region 模块内箱标签打印
        public void temp(string ip = "", int port = 0, int concentration = 5)
        {
            string orderNumber = "2020-YA364-3";
            var barcode = db.BarCodes.Where(c => c.OrderNum == orderNumber && c.BarCodeType == "模块").Select(c => c.BarCodesNum).ToList();
            var printbarcode = db.ModuleInsideTheBox.Where(c => c.OrderNum == orderNumber && c.Statue == "纸箱").Select(c => c.ModuleBarcode).ToList();
            var notprintbarcode = barcode.Except(printbarcode).ToList().Take(55);
            List<string> tempprint = new List<string>();
            List<string> lastprint = new List<string>();
            List<ModuleInsideTheBox> modules = new List<ModuleInsideTheBox>();
            var SN = db.ModuleInsideTheBox.Where(c => c.OrderNum == orderNumber && c.Statue == "纸箱").Max(c => c.SN) + 1;

            int i = 1;
            foreach (var item in notprintbarcode)
            {
                var sqlInnerBarcode = "";
                var sqltype = "";
                tempprint.Add(item);
                if (i % 10 == 0)
                {
                    string InnerBarcode = "2020YA36403-ZX00" + SN;
                    sqlInnerBarcode = InnerBarcode;
                    var substring = new List<string>();
                    tempprint.ForEach(c => substring.Add(c.Substring(c.IndexOf('B'))));
                    var bm = OutsideBoxLableCreate(substring.ToArray(), orderNumber, InnerBarcode, SN + "/48");
                    int totalbytes = bm.ToString().Length;//返回参数总共字节数
                    int rowbytes = 10; //返回参数每行的字节数
                    string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                    string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);//位图转ZPL指令
                    data += totalbytes + "," + rowbytes + "," + hex;
                    data += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
                    //ZebraUnity.IPPrint(data.ToString(), 1, ip, port);

                    tempprint = new List<string>();
                }
                else if (i > 50)
                {
                    SN = 48;
                    sqlInnerBarcode = "2020YA36403-ZX0048";
                    sqltype = "1装5";
                    lastprint.Add(item);
                }
                sqlInnerBarcode = "2020YA36403-ZX00" + SN;
                if (sqltype != "1装5")
                {
                    sqltype = "1装10";
                }
                //数据库添加信息
                ModuleInsideTheBox module = new ModuleInsideTheBox() { OrderNum = orderNumber, InnerBarcode = sqlInnerBarcode, ModuleBarcode = item, SN = SN, Inneror = "李三军", InnerTime = DateTime.Now, Type = sqltype, Department = "品质部", Group = "出货检验组", Statue = "纸箱" };
                modules.Add(module);
                if (i % 10 == 0)
                { SN++; }
                i++;

            }
            if (lastprint.Count == 5)
            {
                string InnerBarcode = "2020YA36403-ZX0048";
                var substring = new List<string>();
                lastprint.ForEach(c => substring.Add(c.Substring(c.IndexOf('B'))));
                var bm = OutsideBoxLableCreate(substring.ToArray(), orderNumber, InnerBarcode, "48/48");
                int totalbytes = bm.ToString().Length;//返回参数总共字节数
                int rowbytes = 10; //返回参数每行的字节数
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);//位图转ZPL指令
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
                //ZebraUnity.IPPrint(data.ToString(), 1, ip, port);


            }
            db.ModuleInsideTheBox.AddRange(modules);
            db.SaveChanges();
        }

        public JObject ModuleInsideBoxLablePrint([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string orderNumber = obj.orderNumber;//订单号
            string InnerBarcode = obj.InnerBarcode;//内箱条码
            string leng = obj.leng == null ? "简" : obj.leng;//语言
            string ip = obj.ip;//ip
            string version = obj.version == null ? "old" : obj.version;//新版打印还是旧版打印
            int port = obj.port == null ? 9101 : obj.port;//端口
            string[] barcodelist = JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(obj.barcodelist));//条码列表
            int concentration = obj.concentration == null ? 5 : obj.concentration;//浓度
            var quantity = db.ModulePackageRule.Where(c => c.OrderNum == orderNumber && c.Statue == "纸箱").Select(c => c.Quantity).ToList();
            var tn = 0;
            if (quantity.Count != 0)
            {
                tn = quantity.Sum();
            }
            var sn = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == InnerBarcode).Select(c => c.SN).FirstOrDefault();
            if (version == "new")
            {
                var bm = CreateIntsideBoxLableNewVersion(barcodelist, orderNumber, InnerBarcode, sn + "/" + tn, leng);
                int totalbytes = bm.ToString().Length;//返回参数总共字节数
                int rowbytes = 10; //返回参数每行的字节数
                string data1 = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);//位图转ZPL指令
                data1 += totalbytes + "," + rowbytes + "," + hex;
                data1 += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
                var result = ZebraUnity.IPPrint(data.ToString(), 1, ip, port);
            }
            else
            {
                var bm = CreateIntsideBoxLable(barcodelist, orderNumber, InnerBarcode, sn + "/" + tn, leng);
                int totalbytes = bm.ToString().Length;//返回参数总共字节数
                int rowbytes = 10; //返回参数每行的字节数
                string data1 = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);//位图转ZPL指令
                data1 += totalbytes + "," + rowbytes + "," + hex;
                data1 += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
                var result = ZebraUnity.IPPrint(data.ToString(), 1, ip, port);
            }
            return com.GetModuleFromJobjet(null, null, "成功");
        }

        #region--生成模块内箱标签图片
        public Bitmap CreateIntsideBoxLable(string[] mn_list, string ordernum, string innboxNUm, string sntn, string leng)
        {
            #region 60*30版本
            //int initialWidth = 500, initialHeight = 250;
            //Bitmap AllBitmap = new Bitmap(initialWidth, initialHeight);
            //Graphics theGraphics = Graphics.FromImage(AllBitmap);
            //Brush bush = new SolidBrush(System.Drawing.Color.Black);
            //theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            //Pen pen = new Pen(Color.Black, 3);//定义笔的大小
            //theGraphics.DrawRectangle(pen, 25, 16, 450, 218);  //x,y,width:绘制矩形的宽度,height：绘制的矩形的高度
            //                                                   //画横线                                                 
            //theGraphics.DrawLine(pen, 25, 50, 475, 50);//起点x,起点y坐标，终点x,终点y坐标
            //theGraphics.DrawLine(pen, 25, 83, 475, 83);
            //theGraphics.DrawLine(pen, 25, 141, 475, 141);
            ////theGraphics.DrawLine(pen, 358, 353, 700, 353);
            ////画竖线
            //theGraphics.DrawLine(pen, 267, 16, 267, 83);

            //System.Drawing.Font myFont_orderNumber1;
            //myFont_orderNumber1 = new System.Drawing.Font("Microsoft YaHei UI", 10, FontStyle.Regular);
            //theGraphics.DrawString("订单号 Order NO", myFont_orderNumber1, bush, 28, 20);//{

            //System.Drawing.Font myFont_orderNumber;
            ////字体微软雅黑，大小40，样式加粗
            //myFont_orderNumber = new System.Drawing.Font("Microsoft YaHei UI", 10, FontStyle.Bold);
            ////设置格式
            //StringFormat format = new StringFormat();
            //format.Alignment = StringAlignment.Center;
            //theGraphics.DrawString(ordernum, myFont_orderNumber, bush, 270, 20);

            ////}
            //System.Drawing.Font myFont_jianhao;
            //myFont_jianhao = new System.Drawing.Font("Microsoft YaHei UI", 10, FontStyle.Regular);
            //theGraphics.DrawString("件号/数(SN/TN)", myFont_jianhao, bush, 28, 55);
            ////引入SNTN
            ////设置格式            
            //System.Drawing.Font myFont_snt;
            //myFont_snt = new System.Drawing.Font("Microsoft YaHei UI", 10, FontStyle.Regular);
            //theGraphics.DrawString(sntn, myFont_snt, bush, 270, 55);


            ////引入条形码
            //Bitmap spc_barcode = BarCodeLablePrint.BarCodeToImg(innboxNUm, 400, 30);
            ////double beishuhege = 0.7;
            //theGraphics.DrawImage(spc_barcode, 45, 88, (float)spc_barcode.Width, (float)spc_barcode.Height);

            ////引入条码号
            //System.Drawing.Font myFont_spc_OuterBoxBarcode;
            //myFont_spc_OuterBoxBarcode = new System.Drawing.Font("Microsoft YaHei UI", 10, FontStyle.Bold);
            //theGraphics.DrawString(innboxNUm, myFont_spc_OuterBoxBarcode, bush, 190, 120);

            ////引入模组号清单
            //int mn_E_count = mn_list.Count();
            ////12位模组号以上，包括条码号
            //System.Drawing.Font myFont_modulenum_list;
            //myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 15, FontStyle.Bold);
            //StringFormat listformat = new StringFormat();
            //listformat.Alignment = StringAlignment.Near;
            //int top_y = 142;
            //int left_x = 25;
            //for (int i = 1; i < mn_list.Count() + 1; i++)
            //{
            //    theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
            //    if ((i % 4) != 0)
            //    {
            //        left_x += 110;
            //    }
            //    else
            //    {
            //        top_y += 20;
            //        left_x = 25;
            //    }
            //}
            //Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            //return bm;
            #endregion

            #region 备品配件大小的打印
            int initialWidth = 750, initialHeight = 583;
            Bitmap AllBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(AllBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            Pen pen = new Pen(Color.Black, 3);//定义笔的大小
            theGraphics.DrawRectangle(pen, 50, 50, 650, 483);  //x,y,width:绘制矩形的宽度,height：绘制的矩形的高度
                                                               //画横线                                                 
            theGraphics.DrawLine(pen, 50, 108, 700, 108);//起点x,起点y坐标，终点x,终点y坐标
            theGraphics.DrawLine(pen, 50, 175, 700, 175);
            theGraphics.DrawLine(pen, 50, 308, 700, 308);
            //theGraphics.DrawLine(pen, 358, 353, 700, 353);
            //画竖线
            // theGraphics.DrawLine(pen, 458, 50, 458, 175);
            System.Drawing.Font myFont_orderNumber1;
            myFont_orderNumber1 = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            System.Drawing.Font myFont_jianhao;
            myFont_jianhao = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            if (leng == "简")
            {
                theGraphics.DrawString("订单号:    " + ordernum, myFont_orderNumber1, bush, 60, 60);
                theGraphics.DrawString("件号/数(SN/TN):    " + sntn, myFont_jianhao, bush, 60, 110);
            }
            else if (leng == "英")
            {
                theGraphics.DrawString("Order No:    " + ordernum, myFont_orderNumber1, bush, 60, 60);
                theGraphics.DrawString("PACKAGE(SN/TN):    " + sntn, myFont_jianhao, bush, 60, 110);
            }
            else if (leng == "繁")
            {
                theGraphics.DrawString("訂單號:    " + ordernum, myFont_orderNumber1, bush, 60, 60);
                theGraphics.DrawString("件號/數(SN/TN):    " + sntn, myFont_jianhao, bush, 60, 110);
            }
            else if (leng == "繁/英")
            {
                theGraphics.DrawString("訂單號(Order No):   " + ordernum, myFont_orderNumber1, bush, 60, 60);
                theGraphics.DrawString("件號/數 PACKAGE(SN/TN):    " + sntn, myFont_jianhao, bush, 60, 110);
            }
            else
            {
                theGraphics.DrawString("订单号(Order No):    " + ordernum, myFont_orderNumber1, bush, 60, 60);
                theGraphics.DrawString("件号/数 PACKAGE(SN/TN):    " + sntn, myFont_jianhao, bush, 60, 110);
            }


            //System.Drawing.Font myFont_orderNumber;
            ////字体微软雅黑，大小40，样式加粗
            //myFont_orderNumber = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Bold);
            ////设置格式
            //StringFormat format = new StringFormat();
            //format.Alignment = StringAlignment.Center;
            //theGraphics.DrawString(ordernum, myFont_orderNumber, bush, 100, 60);

            //}
            //引入SNTN
            //设置格式            
            //System.Drawing.Font myFont_snt;
            //myFont_snt = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //theGraphics.DrawString(sntn, myFont_snt, bush, 100, 110);


            //引入条形码
            Bitmap spc_barcode = BarCodeLablePrint.BarCodeToImg(innboxNUm, 550, 60);
            //double beishuhege = 0.7;
            theGraphics.DrawImage(spc_barcode, 100, 180, (float)spc_barcode.Width, (float)spc_barcode.Height);

            //引入条码号
            System.Drawing.Font myFont_spc_OuterBoxBarcode;
            myFont_spc_OuterBoxBarcode = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Bold);
            theGraphics.DrawString(innboxNUm, myFont_spc_OuterBoxBarcode, bush, 250, 250);

            //引入模组号清单
            int mn_E_count = mn_list.Count();
            //20位模组号以上，包括条码号
            if (mn_list.Count() <= 20)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 28, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 310;
                int left_x = 60;
                for (int i = 1; i < mn_list.Count() + 1; i++)
                {
                    theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 4) != 0)
                    {
                        left_x += 155;
                    }
                    else
                    {
                        top_y += 50;
                        left_x = 60;
                    }
                }
            }
            else
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 310;
                int left_x = 60;
                for (int i = 1; i < mn_list.Count() + 1; i++)
                {
                    theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 5) != 0)
                    {
                        left_x += 124;
                    }
                    else
                    {
                        top_y += 30;
                        left_x = 60;
                    }
                }
            }
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
            #endregion
        }
        #endregion

        #region--生成模块新版内箱标签图片
        public Bitmap CreateIntsideBoxLableNewVersion(string[] mn_list, string ordernum, string innboxNUm, string sntn, string leng)
        {
            int initialWidth = 750, initialHeight = 1000;
            Bitmap AllBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(AllBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            Pen pen = new Pen(Color.Black, 3);//定义笔的大小
            theGraphics.DrawRectangle(pen, 50, 50, 650, 900);  //x,y,width:绘制矩形的宽度,height：绘制的矩形的高度
                                                               //画横线                                                 
            theGraphics.DrawLine(pen, 50, 108, 700, 108);//起点x,起点y坐标，终点x,终点y坐标
            theGraphics.DrawLine(pen, 50, 175, 700, 175);
            theGraphics.DrawLine(pen, 50, 242, 700, 242);
            theGraphics.DrawLine(pen, 50, 308, 700, 308);
            theGraphics.DrawLine(pen, 50, 375, 700, 375);
            theGraphics.DrawLine(pen, 50, 441, 700, 441);
            theGraphics.DrawLine(pen, 50, 570, 700, 570);
            //theGraphics.DrawLine(pen, 358, 353, 700, 353);
            //画竖线
            theGraphics.DrawLine(pen, 300, 50, 300, 375);
            if (leng == "简")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("订单号", myFont, bush, 60, 60);

                theGraphics.DrawString("规格型号", myFont, bush, 60, 120);

                theGraphics.DrawString("颜色", myFont, bush, 60, 190);

                theGraphics.DrawString("数量", myFont, bush, 60, 255);

                theGraphics.DrawString("件号", myFont, bush, 60, 320);
            }
            else if (leng == "简/英" || leng == "")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("订单号 PO#", myFont, bush, 60, 60);

                theGraphics.DrawString("规格型号 ITEM NO ", myFont, bush, 60, 120);

                theGraphics.DrawString("颜色 COLOURS", myFont, bush, 60, 190);

                theGraphics.DrawString("数量 QTY(PSC)", myFont, bush, 60, 255);

                theGraphics.DrawString("件号 PACKAGE#", myFont, bush, 60, 320);
            }
            else if (leng == "繁")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("訂單號", myFont, bush, 60, 60);

                theGraphics.DrawString("規格型號", myFont, bush, 60, 120);

                theGraphics.DrawString("顏色", myFont, bush, 60, 190);

                theGraphics.DrawString("數量", myFont, bush, 60, 255);

                theGraphics.DrawString("件號", myFont, bush, 60, 320);
            }
            else if (leng == "繁/英")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("訂單號 PO#", myFont, bush, 60, 60);

                theGraphics.DrawString("規格型號 ITEM NO", myFont, bush, 60, 120);

                theGraphics.DrawString("顏色 COLOURS", myFont, bush, 60, 190);

                theGraphics.DrawString("數量 QTY(PSC)", myFont, bush, 60, 255);

                theGraphics.DrawString("件號 PACKAGE#", myFont, bush, 60, 320);
            }
            else if (leng == "英")
            {
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("PO#", myFont, bush, 60, 60);

                theGraphics.DrawString("ITEM NO", myFont, bush, 60, 120);

                theGraphics.DrawString("COLOURS", myFont, bush, 60, 190);

                theGraphics.DrawString("QTY(PSC)", myFont, bush, 60, 255);

                theGraphics.DrawString("PACKAGE#", myFont, bush, 60, 320);
            }
            //订单号
            System.Drawing.Font value;
            value = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(ordernum, value, bush, 380, 60);
            //规格型号
            var info = db.ModulePackageRule.Where(c => c.OrderNum == ordernum).FirstOrDefault();
            theGraphics.DrawString(info.ITEMNO, value, bush, 380, 120);
            //颜色
            theGraphics.DrawString(info.COLOURS, value, bush, 380, 190);
            //数量
            string boxnum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == "纸箱").Sum(c => c.Quantity * c.OuterBoxCapacity).ToString();
            int mn_E_count = mn_list.Count();
            theGraphics.DrawString(mn_E_count + "/" + boxnum, value, bush, 380, 255);
            //件号
            string[] s = sntn.Split('/');
            theGraphics.DrawString(s[0] + " OF " + s[1], value, bush, 380, 320);
            //备注
            theGraphics.DrawString(info.Remark, value, bush, 215, 388);
            //引入条形码
            Bitmap spc_barcode = BarCodeLablePrint.BarCodeToImg(innboxNUm, 550, 60);
            //double beishuhege = 0.7;
            theGraphics.DrawImage(spc_barcode, 100, 450, (float)spc_barcode.Width, (float)spc_barcode.Height);

            //引入条码号
            System.Drawing.Font myFont_spc_OuterBoxBarcode;
            myFont_spc_OuterBoxBarcode = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Bold);
            theGraphics.DrawString(innboxNUm, myFont_spc_OuterBoxBarcode, bush, 230, 520);

            //引入模组号清单

            //20位模组号以上，包括条码号
            if (mn_list.Count() <= 20)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 28, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 580;
                int left_x = 60;
                for (int i = 1; i < mn_list.Count() + 1; i++)
                {
                    theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 4) != 0)
                    {
                        left_x += 155;
                    }
                    else
                    {
                        top_y += 50;
                        left_x = 60;
                    }
                }
            }
            else
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Near;
                int top_y = 580;
                int left_x = 60;
                for (int i = 1; i < mn_list.Count() + 1; i++)
                {
                    theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 5) != 0)
                    {
                        left_x += 124;
                    }
                    else
                    {
                        top_y += 30;
                        left_x = 60;
                    }
                }
            }
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
        }
        #endregion

        #region ---查看内箱标签
        [HttpPost]
        [ApiAuthorize]
        public JObject InsideBoxLablePrintToImg([System.Web.Http.FromBody]JObject data)
        {
            string outsidebarcode = data["outsidebarcode"].ToString();//外箱条码
            string statue = data["statue"].ToString();//装箱款式

            JObject result = new JObject();

            var outsidebarcode_recordlist = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == outsidebarcode);
            var ordernum = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == outsidebarcode).Select(c => c.OrderNum).FirstOrDefault();//订单和挪用订单
            var rule = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).Select(c => new { c.Quantity, c.OuterBoxCapacity, c.ITEMNO, c.COLOURS, c.Remark }).ToList();
           result.Add("Ordernum", ordernum);
            string type = outsidebarcode_recordlist.FirstOrDefault().Type;
            result.Add("type", type);
            int sn = outsidebarcode_recordlist.FirstOrDefault().SN;
            int tn= rule.Sum(c => c.Quantity);
            result.Add("SN", sn);
            result.Add("TN", tn);
            int boxnum = rule.Sum(c => c.Quantity*c.OuterBoxCapacity);
            result.Add("BoxNum", boxnum);
            result.Add("ITEMNO", rule.FirstOrDefault().ITEMNO);
            result.Add("COLOURS", rule.FirstOrDefault().COLOURS);
            result.Add("Remark", rule.FirstOrDefault().Remark);
            
             
            var mn_list = outsidebarcode_recordlist.Select(c => c.ModuleBarcode).ToList();
            List<string> barcodelsit = new List<string>();
            mn_list.ForEach(c => barcodelsit.Add(c.Substring(c.IndexOf('B')).ToString()));
            result.Add("BarcodeList", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(barcodelsit)));
            return com.GetModuleFromJobjet(result);
        }
        #endregion

        #region---内箱重复打印
        [HttpPost]
        [ApiAuthorize]
        public JObject InsideBoxLablePrintAgain([System.Web.Http.FromBody]JObject data)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string outsidebarcode = obj.outsidebarcode;//外箱条码
            string ip = obj.ip;//ip
            string statue = obj.statue;//装箱款式
            string leng = obj.leng;//语言
            int pagecount = obj.pagecount == null ? 1 : obj.pagecount; //打印份数
            int concentration = obj.concentration == null ? 5 : obj.concentration; //打印浓度
            int port = obj.port == null ? 0 : obj.port; //端口
            string version = obj.version == null ? "old" : obj.version;//新版打印还是旧版打印

            var outsidebarcode_recordlist = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == outsidebarcode);
            var ordernum = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == outsidebarcode).Select(c => c.OrderNum).FirstOrDefault();//订单和挪用订单

            string sntn = outsidebarcode_recordlist.FirstOrDefault().SN + "/" + db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).Sum(c => c.Quantity);

            var mn_list = outsidebarcode_recordlist.Select(c => c.ModuleBarcode).ToList();
            List<string> barcodelsit = new List<string>();
            var ss = mn_list[1].Substring(mn_list[1].IndexOf('B')).ToString();
            mn_list.ForEach(c => barcodelsit.Add(c.Substring(c.IndexOf('B')).ToString()));

            //如果有包装新订单号，则使用包装新订单号。
            if (version == "new")
            {
                var bm = CreateIntsideBoxLableNewVersion(barcodelsit.ToArray(), ordernum, outsidebarcode, sntn,leng);
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string data1 = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data1 += totalbytes + "," + rowbytes + "," + hex;
                data1 += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
            }
            else
            {
                var bm = CreateIntsideBoxLable(barcodelsit.ToArray(), ordernum, outsidebarcode, sntn,leng);
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string data1 = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data1 += totalbytes + "," + rowbytes + "," + hex;
                data1 += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
            }
            return com.GetModuleFromJobjet(null, null, "成功");



        }
        #endregion

        #endregion

        #region---模块条码打印---30*40
        [HttpPost]
        [ApiAuthorize]
        public JObject ModuleBarcodePrinting1(string[] modulenum, int pagecount, string barcode = "", string ip = "", int port = 0, int concentration = 5, bool testswitch = false)
        {
            //开始绘制图片
            int initialWidth = 520, initialHeight = 250;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));

            Pen pen = new Pen(Color.Black, 1);
            theGraphics.DrawRectangle(pen, 32, 8, 475, 238); //460矩形的宽度 220矩形的高度
                                                             //横线            
            theGraphics.DrawLine(pen, 32, 40, 502, 40);
            theGraphics.DrawLine(pen, 32, 40, 502, 40);
            theGraphics.DrawLine(pen, 32, 72, 502, 72);
            theGraphics.DrawLine(pen, 32, 104, 502, 104);
            theGraphics.DrawLine(pen, 32, 136, 502, 136);
            theGraphics.DrawLine(pen, 32, 168, 502, 168);
            theGraphics.DrawLine(pen, 32, 200, 502, 200);
            //竖线
            theGraphics.DrawLine(pen, 260, 0, 260, 235);

            //引入条码
            int cout = 0;
            int left_x = 10;
            int top_y = 50;
            int left_xx = 23;
            int top_yy = 140;
            foreach (var item in modulenum)
            {
                if (cout <= 5)
                {
                    //条形码
                    StringFormat geshi = new StringFormat();
                    geshi.Alignment = StringAlignment.Center; //居中
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 200, 16);
                    theGraphics.DrawImage(bmp_barcode, top_y, left_x, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 8, FontStyle.Regular);
                    StringFormat geshi1 = new StringFormat();
                    geshi1.Alignment = StringAlignment.Center; //居中
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, top_yy, left_xx, geshi);
                    cout++;
                    left_x += 32;
                    left_xx += 32;
                }
                else if (cout >= 6 && cout <= 11)
                {
                    if (cout == 6)
                    {
                        left_x = 10;
                        left_xx = 23;
                        top_y = 290;
                        top_yy = 385;
                    }
                    //条形码
                    StringFormat geshi = new StringFormat();
                    geshi.Alignment = StringAlignment.Center; //居中
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 200, 16);
                    theGraphics.DrawImage(bmp_barcode, top_y, left_x, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 8, FontStyle.Regular);
                    StringFormat geshi1 = new StringFormat();
                    geshi1.Alignment = StringAlignment.Center; //居中
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, top_yy, left_xx, geshi);
                    cout++;
                    if (cout > 6)
                    {
                        left_x += 32;
                        left_xx += 32;
                    }
                }
            }

            string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值

            if (testswitch == true)
            {
                MemoryStream ms = new MemoryStream();
                theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                theBitmap.Dispose();
                return com.GetModuleFromJobjet(null, null, "成功");
            }
            else
            {
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO150,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                return com.GetModuleFromJobjet(null, null, "成功");
            }
        }
        #endregion

        #region---模块条码打印
        [HttpPost]
        [ApiAuthorize]
        public JObject ModuleBarcodePrinting(string[] modulenum, int pagecount, string barcode = "", string ip = "", int port = 0, int concentration = 5, bool testswitch = false)
        {
            //开始绘制图片
            int initialWidth = 750, initialHeight = 1000;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //Brush bush1 = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(255, 255, 255));

            Pen pen = new Pen(Color.Black, 3);
            theGraphics.DrawRectangle(pen, 5, 40, 675, 895); //460矩形的宽度 220矩形的高度

            ////横线 
            int x_axis = 5;
            int y_axis = 72;
            int high = 678;
            for (var i = 0; i < 27; i++)
            {
                theGraphics.DrawLine(pen, x_axis, y_axis, high, y_axis);
                y_axis += 32;
            }

            ////竖线
            theGraphics.DrawLine(pen, 229, 40, 229, 935);
            theGraphics.DrawLine(pen, 453, 40, 453, 935);

            //引入条码
            int cout = 0;
            int left_x = 44;
            int top_y = 19;
            int left_xx = 57;
            int top_yy = 120;
            foreach (var item in modulenum)
            {
                if (cout <= 27)
                {
                    //条形码
                    StringFormat geshi = new StringFormat();
                    geshi.Alignment = StringAlignment.Center; //居中
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 200, 16);
                    theGraphics.DrawImage(bmp_barcode, top_y, left_x, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 8, FontStyle.Regular);
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, top_yy, left_xx, geshi);
                    cout++;
                    left_x += 32;
                    left_xx += 32;
                }
                else if (cout >= 28 && cout <= 55)
                {
                    if (cout == 28)
                    {
                        left_x = 44;
                        top_y = 245;

                        left_xx = 57;
                        top_yy = 350;
                    }
                    //条形码
                    StringFormat geshi = new StringFormat();
                    geshi.Alignment = StringAlignment.Center; //居中
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 200, 16);
                    theGraphics.DrawImage(bmp_barcode, top_y, left_x, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 8, FontStyle.Regular);
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, top_yy, left_xx, geshi);
                    cout++;
                    if (cout > 28 && cout <= 55)
                    {
                        left_x += 32;
                        left_xx += 32;
                    }
                }
                else if (cout >= 56 && cout <= 83)
                {
                    if (cout == 56)
                    {
                        left_x = 44;
                        top_y = 471;

                        left_xx = 57;
                        top_yy = 580;
                    }
                    //条形码
                    StringFormat geshi = new StringFormat();
                    geshi.Alignment = StringAlignment.Center; //居中
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 200, 16);
                    theGraphics.DrawImage(bmp_barcode, top_y, left_x, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 8, FontStyle.Regular);
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, top_yy, left_xx, geshi);
                    cout++;
                    if (cout > 56 && cout <= 83)
                    {
                        left_x += 32;
                        left_xx += 32;
                    }

                }
            }

            string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值

            if (testswitch == true)
            {
                MemoryStream ms = new MemoryStream();
                theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                theBitmap.Dispose();
                return com.GetModuleFromJobjet(null, null, "成功");
            }
            else
            {
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO70,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                return com.GetModuleFromJobjet(null, null, "成功");
            }
        }

        [HttpPost]
        [ApiAuthorize]
        public JObject ModuleBarcodePrinting2(string[] modulenum, int pagecount, string barcode = "", string ip = "", int port = 0, int concentration = 5, bool testswitch = false)
        {
            string[] modulelist = new string[6];
            modulelist[0] = "2019YA00101B00001";
            modulelist[1] = "2019YA00101B00002";
            modulelist[2] = "2019YA00101B00003";
            //modulelist[3] = "2019YA00101B00004";
            //modulelist[4] = "2019YA00101B00005";
            //modulelist[5] = "2019YA00101B00006";

            //开始绘制图片
            int initialWidth = 716, initialHeight = 33;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //Brush bush1 = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(255, 255, 255));

            Pen pen = new Pen(Color.Black, 1);
            //theGraphics.DrawRectangle(pen, 0, 0, 700, 33); //460矩形的宽度 220矩形的高度
            ////横线 
            //theGraphics.DrawLine(pen, 0, 30, 932, 30);
            ////竖线
            //theGraphics.DrawLine(pen, 229, 0, 229, 28);
            //theGraphics.DrawLine(pen, 453, 0, 453, 28);

            //引入条码
            int default_left_y = 0;
            int default_top_x = 5;
            double default_left_yy = 9;
            int default_top_xx = 90;
            //int left_y = 0;
            int top_x = 5;
            //int left_yy = 13;
            int top_xx = 90;
            StringFormat geshi = new StringFormat();
            geshi.Alignment = StringAlignment.Center; //居中
            System.Drawing.Font myFont_modulebarcodenum;
            myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 10, FontStyle.Bold);
            for (var i = 1; i < modulenum.Count() + 1; i++)
            {
                if ((i % 3) == 0)
                {
                    //条形码
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(modulenum[i - 1], 200, 14);
                    theGraphics.DrawImage(bmp_barcode, top_x, default_left_y, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    theGraphics.DrawString(modulenum[i - 1], myFont_modulebarcodenum, bush, top_xx, (float)default_left_yy, geshi);
                    top_x = default_top_x;
                    top_xx = default_top_xx;
                }
                else
                {
                    //条形码
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(modulenum[i - 1], 200, 14);
                    theGraphics.DrawImage(bmp_barcode, top_x, default_left_y, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //条码号
                    theGraphics.DrawString(modulenum[i - 1], myFont_modulebarcodenum, bush, top_xx, (float)default_left_yy, geshi);
                    top_x += 228;
                    top_xx += 239;
                }
            }

            string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值

            if (testswitch == true)
            {
                MemoryStream ms = new MemoryStream();
                theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                theBitmap.Dispose();
                return com.GetModuleFromJobjet(null, null, "成功");
            }
            else
            {
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO60,0^XGR:ZONE.GRF^FS^XZ";

                ////指令输出打印
                //concentration = 10;
                //data = "^XA^MD" + concentration + "^LH0,0.5^FO58,5^BCN,14,N,N^BY1,1,14^FD2019YA00101B00001^FS";//第一个条码
                //data += "^FO290,5^BCN,14,N,N^BY1,1,14^FD2019YA00101B00002^FS";//第二个条码
                //data += "^FO523,5^BCN,14,N,N^BY1,1,14^FD" + modulelist[2] + "^FS";// +^FSXZ第三个条码
                //data += "^FO90,20^A@N,9,5^FD2019YA00101B00001^FS";//第一个条码的字符数字
                //data += "^FO330,20^A@N,9,5^FD2019YA00101B00002^FS";//第二个条码的字符数字
                //data += "^FO570,20^A@N,9,5^FD" + modulelist[2] + "^FS^XZ";//第三个条码的字符数字


                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                return com.GetModuleFromJobjet(null, null, "成功");

                //条码打印命令说明
                //^XA //条码打印指令开始
                //^MD30 //设置色带颜色的深度, 取值范围从-30到30
                //^LH60,10 //设置条码纸的边距
                //^FO20,10 //设置条码左上角的位置
                //^ACN,18,10 //设置字体
                //^BY1.4,3,50 //设置条码样式。1.4是条码的缩放级别，3是条码中粗细柱的比例, 50是条码高度
                //^BC,,Y,N //打印code128的指令
                //^FD12345678^FS //设置要打印的内容, ^FD是要打印的条码内容^FS表示换行
                //^XZ //条码打印指令结束

                //上面的指令会打印12345678的CODE128的条码
            }
        }

        #endregion

    }
}
