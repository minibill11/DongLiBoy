﻿using System;
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

namespace JianHeMES.Controllers
{
    public class ModuleManagementController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        #region 临时类
        private class TempIndex
        {
            public string Ordernum { get; set; }
            public string Machine { get; set; }
            public bool IsSampling { get; set; }
            public bool SamplingResult { get; set; }

        }
        #endregion
        #region------规则/内箱/外箱页面
        public async Task<ActionResult> Rule()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "Rule" });
            }
            return View();
        }
        public async Task<ActionResult> Inside()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "Inside" });
            }
            return View();
        }
        public async Task<ActionResult> Outside()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "Outside" });
            }
            return View();
        }
        #endregion
        #region------模块看板
        //模块看板首页
        public async Task<ActionResult> Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "Index" });
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(List<string> orderunm)
        {
            JArray total = new JArray();
            var totalOrderdum = db.OrderMgm.Where(c => c.Models != 0).ToList();
            if (orderunm.Count != 0)
            {
                totalOrderdum = totalOrderdum.Where(c => orderunm.Contains(c.OrderNum)).ToList();
            }
            var ai = db.ModuleAI.Select(c => new TempIndex { Ordernum = c.Ordernum, Machine = c.Machine });
            var after = db.AfterWelding.Select(c => new TempIndex { Ordernum = c.Ordernum, IsSampling = c.IsSampling, SamplingResult = c.SamplingResult });
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
                var Sampling = after.Count(c => c.Ordernum == item.OrderNum && c.IsSampling == true);
                result.Add("samplingCount", Math.Round((decimal)Sampling * 100 / item.Models, 2) + "%(" + Sampling + "/" + item.Models + ")");
                //后焊抽检合格率
                //var samplingab=after.Count(c => c.Ordernum == item.OrderNum && c.IsSampling == true);
                //result.Add("samplingPass", Sampling == 0 ? "0" :Math.Round((decimal)samplingab * 100 / Sampling, 2) + "%(" + samplingab + "/" + Sampling + ")");

                //后焊产线完成情况
                JArray array = new JArray();
                result.Add("afterLine", array);
                //灌胶电测完成率
                result.Add("gulePass", null);
                //灌胶电测直通率
                result.Add("gulePassThrough", null);
                //面罩电测完成率
                result.Add("maskPass", null);
                //面罩电测直通率
                result.Add("maskPassThrough", null);
                //电测后抽检率
                result.Add("electricalSamplingCount", null);
                //电测后抽检合格率
                result.Add("electricalSamplingPass", null);
                //老化完成率
                result.Add("burnPass", null);
                //老化直通率
                result.Add("burnPassThrough", null);
                //外观完成率
                result.Add("appearancePass", null);
                //外观直通率
                result.Add("appearancePassThrough", null);
                //内箱包装数量
                var innside = db.ModuleInsideTheBox.Where(c => c.OrderNum == item.OrderNum).Select(c => new { c.ModuleBarcode, c.InnerBarcode }).ToList();
                var innsiderule = db.ModulePackageRule.Where(c => c.OrderNum == item.OrderNum && c.Statue == "纸箱").Select(c => new { c.Quantity, c.OuterBoxCapacity }).ToList();
                if (innsiderule.Count != 0)
                {
                    var totalpack = innsiderule.Sum(c => c.Quantity);
                    var onenum = innsiderule.Sum(c => c.OuterBoxCapacity);
                    result.Add("innerPackCount", innside.Select(c => c.InnerBarcode).Distinct().Count() + "/" + totalpack + "箱(" + innside.Select(c => c.ModuleBarcode).Distinct().Count() + "/" + totalpack * onenum + "件)");
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
                result.Add("warehousr", null);
                total.Add(result);
            }
            return Content(JsonConvert.SerializeObject(total));
        }
        #endregion
        public class CheckList
        {
            public string Barcode { get; set; }
            public bool Finshi { get; set; }
        }
        #region------各工序功能

        #region------ 工段通用方法
        #region    --------------------查询订单已完成、未完成、未开始条码
        [HttpPost]
        public ActionResult Checklist(string orderNum, string statue)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            JObject stationResult = new JObject();//输出结果JObject

            switch (statue)
            {
                case "后焊":
                    var value = db.AfterWelding.Where(c => c.Ordernum == orderNum).OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();
                    stationResult = ChecklistItem(value, orderNum);
                    break;
                case "电测":
                    var value2 = db.ElectricInspection.Where(c => c.Ordernum == orderNum).OrderBy(c => c.ModuleBarcode).Select(c => c.ModuleBarcode).ToList();
                    stationResult = ChecklistItem(value2, orderNum);
                    break;

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
                    //后焊
                    var afterWelding = db.AfterWelding.Where(c => c.ModuleBarcode == barcode).ToList();
                    item.Add("Name", "后焊");
                    item.Add("Have", afterWelding.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    //电检
                    var electricInspection = db.ElectricInspection.Where(c => c.ModuleBarcode == barcode).ToList();
                    item.Add("Name", "电检");
                    item.Add("Have", electricInspection.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    //老化
                    var moduleBurnIn = db.ModuleBurnIn.Where(c => c.ModuleBarcode == barcode).ToList();
                    item.Add("Name", "老化");
                    item.Add("Have", moduleBurnIn.Count == 0 ? false : true);
                    array.Add(item);
                    item = new JObject();

                    //外观
                    var moduleAppearance = db.ModuleAppearance.Where(c => c.ModuleBarcode == barcode).ToList();
                    item.Add("Name", "外观");
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
                case "后焊":
                    count = db.AfterWelding.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode);
                    //isSampling = db.AfterWelding.Where(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode).Select(c => c.IsSampling).FirstOrDefault() ;
                    break;
                case "电检":
                    count = db.ElectricInspection.Count(c => c.Ordernum == ordernum && c.ModuleBarcode == barcode);
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

        #region------AI

        #endregion

        #region ------后焊
        #region----------后焊创建新数据
        public ActionResult AfterWeldingCreate()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "AfterWeldingCreate" });
            }
            return View();
        }

        // POST: ModuleManagement/AfterWeldingCreate
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AfterWeldingCreate(AfterWelding afterWelding)
        {
            JObject result = new JObject();
            if (ModelState.IsValid)
            {
                if (db.AfterWelding.Count(c => c.ModuleBarcode == afterWelding.ModuleBarcode) > 0)
                {
                    result.Add("result", false);
                    result.Add("mes", "该条码已有后焊记录");
                    return Content(JsonConvert.SerializeObject(result));
                }
                afterWelding.AfterWeldingTime = DateTime.Now;
                afterWelding.AfterWeldingor = ((Users)Session["User"]).UserName;
                db.AfterWelding.Add(afterWelding);
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

        #region----------后焊抽样送检工序
        //后焊工序
        // GET: ModuleManagement/AfterWeldingCreate
        public ActionResult AfterWeldingSampling()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "AfterWeldingSampling" });
            }
            return View();
        }

        // POST: ModuleManagement/AfterWeldingCreate
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AfterWeldingSampling(string barcode, string remak, string Abnormal, string Department, string Group, bool isAbnormal)
        {
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

        #region------灌胶前电检工序
        //灌胶前电检工序
        // GET: ModuleManagement/ElectricInspectionBeforeGlueFillingCreate
        public ActionResult ElectricInspectionBeforeGlueFillingCreate()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "ElectricInspectionBeforeGlueFillingCreate" });
            }
            return View();
        }

        // POST: ModuleManagement/ElectricInspectionBeforeGlueFillingCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ElectricInspectionBeforeGlueFillingCreate(ElectricInspection electricInspectionBeforeGlueFilling)
        {
            if (ModelState.IsValid)
            {
                db.ElectricInspection.Add(electricInspectionBeforeGlueFilling);
                await db.SaveChangesAsync();
                return RedirectToAction("ElectricInspectionBeforeGlueFillingCreate");
            }
            return View(electricInspectionBeforeGlueFilling);
        }
        #endregion

        #region------灌胶后电检工序
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

        #region ------老化

        #region 老化新增
        public ActionResult BurninCreate()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "BurninCreate" });
            }
            return View();
        }
        // POST: ModuleManagement/AfterWeldingCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BurninCreate(ModuleBurnIn moduleBurn)
        {
            if (ModelState.IsValid)
            {
                db.ModuleBurnIn.Add(moduleBurn);
                await db.SaveChangesAsync();
                return RedirectToAction("AfterWeldingCreate");
            }
            return View(moduleBurn);
        }
        #endregion

        #region 老化异常输入
        public ActionResult BurninAbnormal()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "BurninAbnormal" });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BurninAbnormal(ModuleBurnIn moduleBurn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(moduleBurn).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("AfterWeldingCreate");
            }
            return View(moduleBurn);
        }

        #region 老化完成
        public ActionResult BurninComplete()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ModuleManagement", act = "BurninComplete" });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BurninComplete(ModuleBurnIn moduleBurn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(moduleBurn).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("AfterWeldingCreate");
            }
            return View(moduleBurn);
        }
        #endregion
        #endregion

        #endregion

        #region ------外观电检
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
        #endregion

        #region ------内箱or外箱装箱规则
        #region 内箱OR外箱装箱规则
        public ActionResult GetInnerInfo(List<ModulePackageRule> modulePackageRule)
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
            JObject valueitem = new JObject();
            JObject value = new JObject();
            var packingList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue).ToList();//根据订单显示包装规则信息

            if (packingList == null)//如果规则信息为空
            {
                return Content("");
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
                if (statue == "纸箱"|| statue == "纸盒")
                {
                    count = db.ModuleInsideTheBox.Where(c => c.IsEmbezzle == false && c.IsMixture == false && c.Type == item.Type && c.OrderNum == item.OrderNum&&c.Statue== statue).Select(c => c.InnerBarcode).Distinct().Count(); ;//根据订单号和类型找 没有混装,没有挪用的 纸箱包装打印记录
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
            return Content(JsonConvert.SerializeObject(value));
        }

        #endregion

        #region------内箱装箱记录
        //输入订单号显示内箱条码和SN/TN/类型
        public ActionResult GetModuleInsideTheBoxInfo(string ordernum, string type,string statue)
        {
            JObject result = new JObject();
            var totalCount = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Statue == statue && c.Type == type).ToList();
            if (totalCount.Count == 0)
            {
                result.Add("mes", "没有创建装箱规则");
                result.Add("InnerBarcode", null);
                result.Add("SN/TN", null);
                return Content(JsonConvert.SerializeObject(result));
            }
            else
            {
                var count = totalCount.Sum(c => c.Quantity);
                var princount = db.ModuleInsideTheBox.Where(c => c.OrderNum == ordernum && c.Type == type).Select(c => c.InnerBarcode).Distinct().Count();//这里要加statue
                JArray typearray = new JArray();
                totalCount.ForEach(c => typearray.Add(c.Type));
                if (princount >= count)
                {
                    result.Add("mes", "已打印完");
                    result.Add("InnerBarcode", null);
                    result.Add("type", null);
                    result.Add("SN/TN", null);
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
                for (var i = 1; i <= count; i++)
                {
                    var temp = InnerBarcode + i.ToString().PadLeft(4, '0');
                    if (db.ModuleInsideTheBox.Where(c => c.InnerBarcode == temp).Count() == 0)
                    {
                        result.Add("mes", "成功");
                        result.Add("InnerBarcode", temp);
                        result.Add("type", typearray);
                        result.Add("SN/TN", i + "/" + count);
                        return Content(JsonConvert.SerializeObject(result));
                    }
                }
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
            result.Add("result", true);
            result.Add("mes", "包装成功");
            return Content(JsonConvert.SerializeObject(result));
        }

        //根据订单判断是否有内内箱
        public ActionResult GetpackList(string ordernum)
        {
            var orders = db.ModulePackageRule.OrderByDescending(m => m.ID).Where(c => c.OrderNum == ordernum&&c.Statue!="外箱").Select(m => m.Statue).ToList();    //增加.Distinct()后会重新按OrderNum升序排序
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

            var basicInfo = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == type && c.ScreenNum == screenNum && c.Batch == batchNum).ToList(); //查找包装规则信息
            var typeList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum).Select(c => c.Type).Distinct().ToList();//查找订单查包装规则的类型
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
            var count = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batchNum).Sum(c => c.Quantity);//根据订单、分屏号查找规定的包装数量总和
            var printCount = db.ModuleOutsideTheBox.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batchNum).Select(c => c.OutsideBarcode).Distinct().ToList().Count();//根据订单、分屏号查找打印的外箱数量
            if (printCount < count)//判断打印的数量是否大于定义包装数量,如果打印数量大于等于定义包装数量,则返回null
            {
                #region 外箱条码生成

                string[] str = ordernum.Split('-');//将订单号分割
                string OuterBoxBarCodeNum = "";
                if (str.Count() == 2)
                {
                    OuterBoxBarCodeNum = "MK"+ordernum + "-" + batchNum.ToString().PadLeft(2, '0') + "-" + screenNum.ToString().PadLeft(2, '0') + "-";
                }
                else
                {
                    string start = str[0].Substring(2);//取出 如2017-test-1 的17
                    OuterBoxBarCodeNum = "MK" + start + str[1] + "-" + str[2] + "-" + batchNum.ToString().PadLeft(2, '0') + "-" + screenNum.ToString().PadLeft(2, '0') + "-";//外箱条码组成 

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
                var batchList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == item).Select(c => c.Batch).Distinct().ToList();//根据订单、类型查找包装规则的分屏号
                foreach (var batchnum in batchList)//循环批号
                {
                    var screemNumList = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == item && c.Batch == batchnum).Select(c => c.ScreenNum).Distinct().ToList();
                    foreach (var screenNumitem in screemNumList)//循环屏序号
                    {
                        JObject info = new JObject();
                        var totleNum = db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNumitem && c.Batch == batchnum).ToList().Count() == 0 ? 0 : db.ModulePackageRule.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNumitem && c.Batch == batchnum).Sum(c => c.Quantity);//根据订单号、类型、分屏号查找包装规则的包装总数量(用于显示完成数量的分母显示)
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
            var totle = db.ModulePackageRule.Where(c => c.OrderNum == ordernum).ToList();
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
                order = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == barcode&&c.Statue=="纸箱").Select(c => c.OrderNum).FirstOrDefault();//根据条码在条码表找订单
            }
            else if (statue == "纸箱" && isthree == 0)
            {
                order = db.BarCodes.Where(c => c.BarCodesNum == barcode && c.BarCodeType == "模块").Select(c => c.OrderNum).FirstOrDefault();//根据条码在条码表找订单
            }
            else if (statue == "纸箱" && isthree != 0)
            {
                order = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == barcode&&c.Statue=="纸盒").Select(c => c.OrderNum).FirstOrDefault();//根据条码在内箱条码表找订单
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
                    Printingexit = db.ModuleInsideTheBox.Where(c => c.ModuleBarcode == barcode&&c.Statue=="纸箱").Select(c => c.ModuleBarcode).FirstOrDefault();//查找打印表里是否有相同的条码号
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


        #region 内箱标签打印
        /// <summary>
        /// 后台绘图后打印方法
        /// </summary>
        /// <param name="pagecount"></param>
        /// <param name="barcode"></param>
        /// <param name="modulenum"></param>
        /// <param name="concentration">浓度</param>

        /// <returns></returns>

        public ActionResult InsideBoxLablePrint(string modulenum, string orderNum, int pagecount, string language, string sntn, string barcode = "", string ip = "", int port = 0, int concentration = 5, bool testswitch = false)
        {
            var list = modulenum.Split(',');
            //开始绘制图片
            int initialWidth = 510, initialHeight = 250;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                    //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));

            Pen pen = new Pen(Color.Black, 3);
            theGraphics.DrawRectangle(pen, 36, 10, 460, 220); //460矩形的宽度 220矩形的高度
                                                              //横线            
            theGraphics.DrawLine(pen, 36, 35, 495, 35);
            theGraphics.DrawLine(pen, 36, 60, 495, 60);
            theGraphics.DrawLine(pen, 36, 110, 495, 110);

            //竖线
            theGraphics.DrawLine(pen, 240, 10, 240, 60);

            //引入订单号文字
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center; //居中 
            Rectangle orderRectangle = new Rectangle(14, 12, 245, 105);//文本矩形
            System.Drawing.Font myFont_order;
            myFont_order = new System.Drawing.Font("Microsoft YaHei UI", 14, FontStyle.Regular);
            if (language == "中")
            {
                theGraphics.DrawString("订   单   号", myFont_order, bush, orderRectangle, format);
            }
            else if (language == "英")
            {
                theGraphics.DrawString("Order  No", myFont_order, bush, orderRectangle, format);
            }
            else
            {
                Rectangle orderRectangle1 = new Rectangle(13, 12, 245, 105);//文本矩形
                theGraphics.DrawString("订单号 Order NO", myFont_order, bush, orderRectangle1, format);
            }

            //引入订单号值
            Rectangle orderNumberRectangle = new Rectangle(245, 12, 245, 105);
            theGraphics.DrawString(orderNum, myFont_order, bush, orderNumberRectangle, format);

            //引入件号、数文字
            if (language == "中")
            {
                Rectangle sntnRectangle = new Rectangle(12, 36, 245, 105);
                theGraphics.DrawString("件号 / 数", myFont_order, bush, sntnRectangle, format);
            }
            else if (language == "英")
            {
                Rectangle sntnRectangle = new Rectangle(12, 36, 245, 105);
                theGraphics.DrawString("SN / TN", myFont_order, bush, sntnRectangle, format);
            }
            else
            {
                Rectangle sntnRectangle = new Rectangle(21, 36, 245, 105);
                theGraphics.DrawString("件号 / 数（SN / TN）", myFont_order, bush, sntnRectangle, format);
            }

            //引入件号/数值
            Rectangle sntnValueRectangle = new Rectangle(245, 35, 245, 105);
            theGraphics.DrawString(sntn, myFont_order, bush, sntnValueRectangle, format);

            //引入条码
            StringFormat geshi = new StringFormat();
            geshi.Alignment = StringAlignment.Center; //居中
            Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(barcode, 380, 30);
            theGraphics.DrawImage(bmp_barcode, 80, 65, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

            //引入条码号
            System.Drawing.Font myFont_modulebarcodenum;
            myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 11, FontStyle.Regular);
            StringFormat geshi1 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            theGraphics.DrawString(barcode, myFont_modulebarcodenum, bush, 260, 92, geshi);

            //// 引入模组号
            if (list.Length <= 4)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Center;
                int top_y = 150;
                int left_x = 100;
                for (int i = 1; i < list.Count() + 1; i++)
                {
                    theGraphics.DrawString(list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    left_x += 110;
                }
            }
            else if (list.Length > 4 && list.Length <= 8)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Center;
                int top_y = 135;
                int left_x = 120;
                for (int i = 1; i < list.Count() + 1; i++)
                {
                    theGraphics.DrawString(list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 4) != 0)
                    {
                        left_x += 100;
                    }
                    else
                    {
                        top_y += 40;
                        left_x -= 300;
                    }
                }
            }
            else if (list.Length > 8 && list.Length <= 12)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Center;
                int top_y = 115;
                int left_x = 120;
                for (int i = 1; i < list.Count() + 1; i++)
                {
                    theGraphics.DrawString(list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 4) != 0)
                    {
                        left_x += 100;
                    }
                    else
                    {
                        top_y += 40;
                        left_x -= 300;
                    }
                }
            }
            else if (list.Length > 12 && list.Length <= 16)
            {
                System.Drawing.Font myFont_modulenum_list;
                myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 14, FontStyle.Bold);
                StringFormat listformat = new StringFormat();
                listformat.Alignment = StringAlignment.Center;
                int top_y = 115;
                int left_x = 120;
                for (int i = 1; i < list.Count() + 1; i++)
                {
                    theGraphics.DrawString(list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 4) != 0)
                    {
                        left_x += 100;
                    }
                    else
                    {
                        top_y += 30;
                        left_x -= 300;
                    }
                }
            }
            else
            {

            }

            string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值

            //string ip = "172.16.99.240";//打印机IP地址
            //int port = 9101;//打印机端口

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

        #region 模块外箱标签打印
        #region---包装打印标签
        //打印标签
        [HttpPost]
        public ActionResult OutsideBoxLablePrint(int screennum = 1, string ordernum = "", string packagingordernum = "", string outsidebarcode = "", string material_discription = "", int pagecount = 1, string sntn = "", string qty = "", bool logo = true, string ip = "", int port = 0, int concentration = 5, string[] mn_list = null, double? g_Weight = null, double? n_Weight = null, string leng = "", bool testswitch = false)
        {
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
            if (leng == "中")
            {
                theGraphics.DrawString("毛重量(kg)", myFont_g_Weight, bush, 55, 295);
            }
            else if (leng == "英")
            {
                theGraphics.DrawString("G.W.(kg)", myFont_g_Weight, bush, 55, 295);
            }
            else
            {
                theGraphics.DrawString("毛重量(G.W.)kg", myFont_g_Weight, bush, 55, 295);
            }

            //引入毛重量值
            System.Drawing.Font myFont_g_Weight_content;
            StringFormat geshi1 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            myFont_g_Weight_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);

            //double G_Weight = g_Weight == null ? 0 : (double)g_Weight;           
            theGraphics.DrawString(g_Weight.ToString(), myFont_g_Weight_content, bush, 280, 295);


            //引入净重
            System.Drawing.Font myFont_n_Weight;
            myFont_n_Weight = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            if (leng == "中")
            {
                theGraphics.DrawString("净重(kg)", myFont_n_Weight, bush, 410, 295);
            }
            else if (leng == "英")
            {
                theGraphics.DrawString("N.W.(kg)", myFont_n_Weight, bush, 410, 295);
            }
            else
            {
                theGraphics.DrawString("净重(N.W.)kg", myFont_n_Weight, bush, 410, 295);
            }

            //引入净重值
            System.Drawing.Font myFont_n_Weight_content;
            StringFormat geshi2 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            myFont_n_Weight_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            //double N_Weight = n_Weight == null ? 0 : (double)n_Weight;
            theGraphics.DrawString(n_Weight.ToString(), myFont_n_Weight_content, bush, 600, 295);


            //引入物料描述
            System.Drawing.Font myFont_material_discription;
            myFont_material_discription = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            if (leng == "中")
            {
                theGraphics.DrawString("物料描述", myFont_material_discription, bush, 55, 355);
            }
            else if (leng == "英")
            {
                theGraphics.DrawString("DESC", myFont_material_discription, bush, 55, 355);
            }
            else
            {
                theGraphics.DrawString("物料描述(DESC)", myFont_material_discription, bush, 55, 355);
            }

            //引入物料描述内容
            System.Drawing.Font myFont_material_discription_content;
            myFont_material_discription_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(material_discription, myFont_material_discription_content, bush, 275, 355);

            //引入屏序号
            System.Drawing.Font myFont_screennum;
            myFont_screennum = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            if (leng == "中")
            {
                theGraphics.DrawString("屏序号", myFont_screennum, bush, 410, 355);
            }
            else if (leng == "英")
            {
                theGraphics.DrawString("NO.", myFont_screennum, bush, 410, 355);
            }
            else
            {
                theGraphics.DrawString("屏序号(NO.)", myFont_screennum, bush, 410, 355);
            }

            //引入屏序号值
            System.Drawing.Font myFont_screennum_data;
            StringFormat geshi3 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            myFont_screennum_data = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(screennum.ToString(), myFont_screennum_data, bush, 615, 355);

            ////引入SN/TN
            System.Drawing.Font myFont_sntn;
            myFont_sntn = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            if (leng == "中")
            {
                theGraphics.DrawString("件号/数", myFont_sntn, bush, 55, 415);
            }
            else if (leng == "英")
            {
                theGraphics.DrawString("SN/TN", myFont_sntn, bush, 55, 415);
            }
            else
            {
                theGraphics.DrawString("件号/数(SN/TN)", myFont_sntn, bush, 55, 415);
            }

            //引入SN/TN内容
            System.Drawing.Font myFont_sntn_content;
            myFont_sntn_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(sntn, myFont_sntn_content, bush, 290, 415);

            //引入数量QTY
            System.Drawing.Font myFont_qty;
            myFont_qty = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            if (leng == "中")
            {
                theGraphics.DrawString("数量", myFont_qty, bush, 410, 415);
            }
            else if (leng == "英")
            {
                theGraphics.DrawString("QTY", myFont_qty, bush, 410, 415);
            }
            else
            {
                theGraphics.DrawString("数量(QTY)", myFont_qty, bush, 410, 415);
            }

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
                int left_x = 70;
                for (int i = 0; i < mn_list.Count(); i++)
                {
                    theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    if ((i % 2) == 0)
                    {
                        left_x += 300;
                    }
                    else
                    {
                        top_y += 90;
                        left_x = 70;
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
        #endregion

        #region 模块内箱标签打印
        public ActionResult ModuleInsideBoxLablePrint(string[] barcodelist, string orderNumber, string InnerBarcode, string ip = "", int port = 0, int concentration = 5)
        {
            var quantity = db.ModulePackageRule.Where(c => c.OrderNum == orderNumber && c.Statue == "纸箱").Select(c => c.Quantity).ToList();
            var tn = 0;
            if (quantity.Count != 0)
            {
                tn = quantity.Sum();
            }
            var sn = db.ModuleInsideTheBox.Where(c => c.InnerBarcode == InnerBarcode).Select(c => c.SN).FirstOrDefault();

            var bm = CreateIntsideBoxLable(barcodelist, orderNumber, InnerBarcode, sn + "/" + tn);
            int totalbytes = bm.ToString().Length;//返回参数总共字节数
            int rowbytes = 10; //返回参数每行的字节数
            string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
            string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);//位图转ZPL指令
            data += totalbytes + "," + rowbytes + "," + hex;
            data += "^LH0,3^FO38,0^XGR:ZONE.GRF^FS^XZ";
            var result = ZebraUnity.IPPrint(data.ToString(), 1, ip, port);

            return Content(result);
        }

        #region--生成模块内箱标签图片
        /// ------<SPC_CreateOutsideBoxLable_summary>
        /// 1.方法的作用：生成备品、配件外箱标签图片
        /// 2.方法的参数及用法： ordernum(订单号),spc_outsidebarcode(外箱号),spcType（外箱类型）,sntn（件号/数）,batch（批次）,logo，g_Weight(净重量),n_Weight(毛重量)。用法：用于组合出外箱标签图片。
        /// 3.方法具体逻辑顺序,判断条件：（1）定义整个标签的宽和高、定义好笔的大小，根据需求绘制出整个标签的样板，（2）在对应的位置引入相对应的参数，并设置字体大小{包括：ordernum(订单号),
        /// spc_outsidebarcode(外箱号), spcType（外箱类型）,sntn（件号/数）,batch（批次）,logo，g_Weight(净重量),n_Weight(毛重量)}，（3）最后调用ConvertTo1Bpp1()与ToGray()方法调整图像灰度、
        /// 灰度反转以及二值化处理（图形转二值）。
        /// 4.方法（可能）有的结果：将图形转二值结果输出。
        /// ------</SPC_CreateOutsideBoxLable_summary>
        public Bitmap CreateIntsideBoxLable(string[] mn_list, string ordernum, string innboxNUm, string sntn)
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
            theGraphics.DrawString("订单号:    "+ ordernum, myFont_orderNumber1, bush, 60, 60);//{

            //System.Drawing.Font myFont_orderNumber;
            ////字体微软雅黑，大小40，样式加粗
            //myFont_orderNumber = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Bold);
            ////设置格式
            //StringFormat format = new StringFormat();
            //format.Alignment = StringAlignment.Center;
            //theGraphics.DrawString(ordernum, myFont_orderNumber, bush, 100, 60);

            //}
            System.Drawing.Font myFont_jianhao;
            myFont_jianhao = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString("件号/数(SN/TN):    "+ sntn, myFont_jianhao, bush, 60, 110);
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
            //12位模组号以上，包括条码号
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
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
            #endregion
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
}
