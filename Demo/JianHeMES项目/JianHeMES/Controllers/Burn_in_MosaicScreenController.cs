﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JianHeMES.Controllers
{
    public class Burn_in_MosaicScreenController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public class TempBurn_in
        {

            public string BarCodesNum { get; set; }
            public DateTime? OQCCheckBT { get; set; }
            public DateTime? OQCCheckFT { get; set; }
            public bool OQCCheckFinish { get; set; }
            public string Burn_in_OQCCheckAbnormal { get; set; }
        }
        public class TempBurn_in_MosaicScreen
        {

            public string BurnInShelfNum { get; set; }
            public string OrderNum { get; set; }
            public string BarCodesNum { get; set; }
        }
        #region 开始拼屏
        public ActionResult mosaicScreen_B()
        {
            //if (com.isCheckRole("老化管理", "老化拼屏操作", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.nuoOrderList = GetnuoOrderList();

            return View();
            //}
            //return Content("<script>alert('对不起，您不能进行老化拼屏操作操作，请联系品质部管理人员！');window.location.href='../FinalQCs';</script>");

        }
        [HttpPost]
        public ActionResult mosaicScreen_Begin(string OrderNum, string BurnInShelfNum, List<string> BarCodesNumList)
        {
            var newRecord = new Burn_in_MosaicScreen();
            newRecord.OrderNum = OrderNum;
            string result_putin = "";
            string result_never_putin = "";
            foreach (var item in BarCodesNumList)
            {
                var count = db.Burn_in_MosaicScreen.Count(c => c.BarCodesNum == item);
                if (count == 0)
                {
                    newRecord.BarCodesNum = item;
                    newRecord.OQCMosaicStartTime = DateTime.Now;
                    newRecord.OQCPrincipalNum = ((Users)Session["User"]).UserNum.ToString();

                    // "16621";
                    newRecord.BurnInShelfNum = BurnInShelfNum;
                    db.Burn_in_MosaicScreen.Add(newRecord);
                    db.SaveChanges();
                    if (result_putin == "")
                    {
                        result_putin = item;
                    }
                    else
                    {
                        result_putin = result_putin + ",\n" + item;
                    }
                }
                else
                {
                    if (result_never_putin == "")
                    {
                        result_never_putin = item;
                    }
                    else
                    {
                        result_never_putin = result_never_putin + ",\n" + item;
                    }
                }
            }
            if (result_putin != "")
            {
                result_putin = result_putin + "\n完成拼屏操作。";
            }
            if (result_never_putin != "")
            {
                result_never_putin = result_never_putin + "\n此条码已经在老化架上，请确认条码的准确。";
            }
            if (result_putin + result_never_putin != "")
            {
                return Content(result_putin + result_never_putin);
            }
            else
            {
                return Content("没有条码。");
            }
            //return Json(data,JsonRequestBehavior.AllowGet);
        }
        #endregion

        //开始扫码清单
        public ActionResult disPlayMosaicScreenList(string BurnInShelfNum)
        {
            JObject list = new JObject();
            //读取拼屏表中该老化架号的所有条码
            var mosaicScreenList = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == BurnInShelfNum && c.OQCMosaicStartTime != null).Select(c => c.BarCodesNum).ToList();
            //读取条码中已经开始老化的条码
            var startBurn_inList = db.Burn_in.Where(c => mosaicScreenList.Contains(c.BarCodesNum) && c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false).Select(c => c.BarCodesNum).ToList();
            //读取条码中已经完成老化的条码
            var endStartBurn_inList = db.Burn_in.Where(c => mosaicScreenList.Contains(c.BarCodesNum) && c.OQCCheckBT != null && c.OQCCheckFT != null && c.OQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
            //去除已经开始和已经完成老化的条码，得到已经拼屏但没做任何操作的条码
            var temp = mosaicScreenList.Except(startBurn_inList).ToList();
            var notStartBurn_inList = temp.Except(endStartBurn_inList).ToList();

            list.Add("startburn_in", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(startBurn_inList)));
            list.Add("notstartburn_in", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(notStartBurn_inList)));
            return Content(JsonConvert.SerializeObject(list));
        }

        //修改拼屏号
        public void editShelfNum(string oldShelfNum, string newShelfNum, List<string> barcodeList)
        {
            foreach (var item in barcodeList)
            {
                var edit = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == oldShelfNum && c.BarCodesNum == item).FirstOrDefault();
                edit.BurnInShelfNum = newShelfNum;
                db.SaveChangesAsync();
            }
        }

        #region 完成拼屏  暂时不用
        public ActionResult mosaicScreen_F()
        {
            //权限
            ViewBag.OrderList = GetOrderList();
            return View();

        }
        [HttpPost]
        public ActionResult mosaicScreenFinish(string OrderNum, List<string> BarCodesNumList)
        {
            foreach (var barcode in BarCodesNumList)
            {
                var mosaicScreen = db.Burn_in_MosaicScreen.Where(c => c.OrderNum == OrderNum && c.BarCodesNum == barcode).FirstOrDefault();
                if (mosaicScreen != null)
                {
                    mosaicScreen.OQCMosaicEndTime = DateTime.Now;
                    db.SaveChanges();
                }
            }
            return Content("完成拼屏");


        }
        #endregion

        //拼屏数据显示
        public ActionResult mosaicScreen_ShelfQuery()
        {
            ViewBag.ordernum = GetMosaiOrderList();
            ViewBag.burn = GetmosaiShelfNumList();
            return View();

        }
        [HttpPost]
        public ActionResult mosaicScreen_ShelfQueryData()
        {
            JObject oneShelfNum = new JObject();
            JArray ShelfNumJosnList = new JArray();
            JObject onebarcodeInfo = new JObject();
            JArray barcodeJsonList = new JArray();
            JObject oneOrderNum = new JObject();
            JArray OrderNumJsonList = new JArray();
            var displayList = new List<string>();
            var BurnInmosaic = (from c in db.Burn_in_MosaicScreen select new TempBurn_in_MosaicScreen { BurnInShelfNum = c.BurnInShelfNum, OrderNum = c.OrderNum, BarCodesNum = c.BarCodesNum }).ToList();
            var BurnInShelfNumList = BurnInmosaic.OrderBy(c => c.BurnInShelfNum).Select(c => c.BurnInShelfNum).Distinct().ToList();

            var tempburn = db.Burn_in.Where(c => c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false).Select(c => new TempBurn_in { BarCodesNum = c.BarCodesNum, Burn_in_OQCCheckAbnormal = c.Burn_in_OQCCheckAbnormal, OQCCheckBT = c.OQCCheckBT, OQCCheckFinish = c.OQCCheckFinish, OQCCheckFT = c.OQCCheckFT }).ToList();
            var tempburn1 = db.Burn_in.Where(c => c.OQCCheckBT != null && c.OQCCheckFT != null && c.OQCCheckFinish == true).Select(c => new TempBurn_in { BarCodesNum = c.BarCodesNum, Burn_in_OQCCheckAbnormal = c.Burn_in_OQCCheckAbnormal, OQCCheckBT = c.OQCCheckBT, OQCCheckFinish = c.OQCCheckFinish, OQCCheckFT = c.OQCCheckFT }).ToList();
            foreach (var item in BurnInShelfNumList)
            {
                var OrderNumList = BurnInmosaic.Where(c => c.BurnInShelfNum == item).Select(c => c.OrderNum).Distinct().ToList();


                foreach (var OrderNum in OrderNumList)
                {
                    var barcodeList = BurnInmosaic.OrderBy(c => c.BarCodesNum).Where(c => c.BurnInShelfNum == item && c.OrderNum == OrderNum).Select(c => c.BarCodesNum).ToList();
                    //条码中已经完成老化的条码集合
                    var endStartBurn_inList = tempburn1.Where(c => barcodeList.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).ToList();
                    //去除已经完成老化的条码
                    displayList = barcodeList.Except(endStartBurn_inList).ToList();

                    foreach (var barcode in displayList)
                    {
                        onebarcodeInfo.Add("barcode", barcode);
                        var currentburn = tempburn.OrderByDescending(c => c.OQCCheckBT).Where(c => c.BarCodesNum == barcode).FirstOrDefault();
                        if (currentburn == null)
                        {
                            onebarcodeInfo.Add("status", 0);
                            onebarcodeInfo.Add("Abnormal", "");
                        }
                        //已经拼屏已经开始老化
                        else if (currentburn.Burn_in_OQCCheckAbnormal == null)
                        {
                            onebarcodeInfo.Add("status", 1);
                            onebarcodeInfo.Add("Abnormal", "");
                        }
                        //有异常
                        else if (currentburn.Burn_in_OQCCheckAbnormal != null)
                        {
                            onebarcodeInfo.Add("status", 2);

                            onebarcodeInfo.Add("Abnormal", currentburn.Burn_in_OQCCheckAbnormal);
                        }

                        barcodeJsonList.Add(onebarcodeInfo);

                        onebarcodeInfo = new JObject();
                    }
                    if (displayList.Count == 0)
                    {
                        continue;
                    }

                    oneOrderNum.Add("barcodelist", barcodeJsonList);
                    oneOrderNum.Add("ordernum", OrderNum);
                    OrderNumJsonList.Add(oneOrderNum);

                    barcodeJsonList = new JArray();
                    oneOrderNum = new JObject();

                }
                if (OrderNumJsonList.Count == 0)
                {
                    continue;
                }
                oneShelfNum.Add("ShelfNum", item);
                oneShelfNum.Add("content", OrderNumJsonList);
                ShelfNumJosnList.Add(oneShelfNum);

                OrderNumJsonList = new JArray();
                oneShelfNum = new JObject();

            }

            if (BurnInShelfNumList.Count != 0)
            {
                return Content(JsonConvert.SerializeObject(ShelfNumJosnList));
            }
            return Content("没有数据");
        }

        public ActionResult mosaicScreen_ShelfQueryHistory()
        {
            return View();

        }

        [HttpPost]
        public ActionResult mosaicScreen_ShelfQueryHistory(string ordernum)
        {
            JArray total = new JArray();
            JObject BurnSelf = new JObject();
            var BurnInShelfNumList = db.Burn_in_MosaicScreen.Where(c => c.OrderNum == ordernum).Select(c => c.BurnInShelfNum).Distinct().ToList();
            foreach (var item in BurnInShelfNumList)
            {

                BurnSelf.Add("ShelfNum", item);
                var barcodeList = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == item && c.OrderNum == ordernum).Select(c => c.BarCodesNum).ToList();

                JArray barcodeListJarrry = new JArray();
                foreach (var barcode in barcodeList)
                {
                    JObject onebarcodeInfo = new JObject();
                    onebarcodeInfo.Add("barcode", barcode);

                    //已经开始老化
                    if (db.Burn_in.Count(c => c.BarCodesNum == barcode && c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal == null) > 0)
                    {
                        onebarcodeInfo.Add("status", 1);
                        onebarcodeInfo.Add("Abnormal", "");
                    }
                    //有异常
                    else if (db.Burn_in.Count(c => c.BarCodesNum == barcode && c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal != null) >= 1)
                    {
                        onebarcodeInfo.Add("status", 2);
                        var Abnormal = db.Burn_in.Where(c => c.BarCodesNum == barcode && c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal != null).Select(c => c.Burn_in_OQCCheckAbnormal).FirstOrDefault();
                        onebarcodeInfo.Add("Abnormal", Abnormal);
                    }
                    //已完成老化
                    else if (db.Burn_in.Count(c => c.BarCodesNum == barcode && c.OQCCheckBT != null && c.OQCCheckFT != null && c.OQCCheckFinish == true) > 0)
                    {
                        onebarcodeInfo.Add("status", 3);
                        onebarcodeInfo.Add("Abnormal", "");
                    }
                    else
                    {
                        onebarcodeInfo.Add("status", 0);
                        onebarcodeInfo.Add("Abnormal", "");
                    }
                    barcodeListJarrry.Add(onebarcodeInfo);
                }
                JObject oneOrderNum = new JObject();
                JArray ordernumJoject = new JArray();
                if (barcodeList.Count != 0)
                {

                    oneOrderNum.Add("barcodelist", barcodeListJarrry);
                    oneOrderNum.Add("ordernum", ordernum);
                }
                ordernumJoject.Add(oneOrderNum);
                BurnSelf.Add("content", ordernumJoject);
                total.Add(BurnSelf);
            }
            if (BurnInShelfNumList.Count != 0)
            {
                return Content(JsonConvert.SerializeObject(total));
            }
            return Content("没有数据");

        }

        // GET: Burn_in_MosaicScreen
        public ActionResult Index()
        {
            return View(db.Burn_in_MosaicScreen.ToList());

        }
        [HttpPost]
        public ActionResult Index(string burnInShelfNum)
        {
            JObject MosaicScreen = new JObject();
            JObject MosaicScreenFinshing = new JObject();
            JObject MosaicScreenBeagin = new JObject();
            var count = db.Burn_in_MosaicScreen.Count(c => c.BurnInShelfNum == burnInShelfNum);
            var OQCinfoFinshing = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == burnInShelfNum && c.OQCMosaicStartTime != null && c.OQCMosaicEndTime != null).ToList();
            var OQCinfoBeagin = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == burnInShelfNum && c.OQCMosaicStartTime != null && c.OQCMosaicEndTime == null).ToList();
            foreach (var item in OQCinfoFinshing)
            {
                MosaicScreenFinshing.Add("BarCodesNum", item.BarCodesNum);
            }
            foreach (var item in OQCinfoBeagin)
            {
                MosaicScreenBeagin.Add("BarCodesNum", item.BarCodesNum);
            }
            //已完成列表
            MosaicScreen.Add("Finashing", MosaicScreenFinshing);
            //已完成数量
            MosaicScreen.Add("FinashingCount", OQCinfoFinshing.Count());
            //未完成列表
            MosaicScreen.Add("Beagin", MosaicScreenBeagin);
            //未完成数量
            MosaicScreen.Add("BeaginCount", OQCinfoBeagin.Count());
            //总数量
            MosaicScreen.Add("Count", count);
            return Content(JsonConvert.SerializeObject(MosaicScreen));

        }
        // GET: Burn_in_MosaicScreen/Details/5 
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Burn_in_MosaicScreen burn_in_MosaicScreen = db.Burn_in_MosaicScreen.Find(id);
            if (burn_in_MosaicScreen == null)
            {
                return HttpNotFound();
            }
            return View(burn_in_MosaicScreen);
        }

        // GET: Burn_in_MosaicScreen/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Burn_in_MosaicScreen/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public ActionResult Create(List<Burn_in_MosaicScreen> burn_in_MosaicScreenList)
        {
            foreach (var burn_in_MosaicScreen in burn_in_MosaicScreenList)
            {
                if (ModelState.IsValid)
                {
                    db.Burn_in_MosaicScreen.Add(burn_in_MosaicScreen);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        // GET: Burn_in_MosaicScreen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Burn_in_MosaicScreen burn_in_MosaicScreen = db.Burn_in_MosaicScreen.Find(id);
            if (burn_in_MosaicScreen == null)
            {
                return HttpNotFound();
            }
            return View(burn_in_MosaicScreen);
        }

        // POST: Burn_in_MosaicScreen/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OrderNum,BarCodesNum,BurnInShelfNum,OQCPrincipalNum,OQCMosaicStartTime,OQCMosaicEndTime,Remark")] Burn_in_MosaicScreen burn_in_MosaicScreen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(burn_in_MosaicScreen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(burn_in_MosaicScreen);
        }

        // GET: Burn_in_MosaicScreen/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Burn_in_MosaicScreen burn_in_MosaicScreen = db.Burn_in_MosaicScreen.Find(id);
            if (burn_in_MosaicScreen == null)
            {
                return HttpNotFound();
            }
            return View(burn_in_MosaicScreen);
        }

        // POST: Burn_in_MosaicScreen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Burn_in_MosaicScreen burn_in_MosaicScreen = db.Burn_in_MosaicScreen.Find(id);
            db.Burn_in_MosaicScreen.Remove(burn_in_MosaicScreen);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //检查开始拼屏FQC是否检查
        public ActionResult CheckFQC(string ordernum, List<string> barcodeList)
        {
            JObject FQCcheckList = new JObject();
            foreach (var barcode in barcodeList)
            {
                var mosicScreenInfo = db.Burn_in_MosaicScreen.Where(c => c.BarCodesNum == barcode).FirstOrDefault();
                var burn_in = db.Burn_in.Where(c => c.BarCodesNum == barcode && c.OrderNum == ordernum).FirstOrDefault();
                var barcodeinfo = db.BarCodes.Where(c => c.BarCodesNum == barcode).FirstOrDefault();
                var FQCcount = db.FinalQC.Where(c => c.BarCodesNum == barcode && c.FQCCheckFinish == true).Count();
                if (barcodeinfo == null)
                {
                    FQCcheckList.Add(barcode, "未找到此条码");
                    continue;
                }
                if (barcodeinfo.OrderNum != ordernum)
                {
                    FQCcheckList.Add(barcode, "条码不属于此订单");
                    continue;
                }
                if (burn_in != null)
                {
                    if (burn_in.OQCCheckFinish == true)
                    {
                        FQCcheckList.Add(barcode, "条码已经完成老化");
                        continue;
                    }
                    if (burn_in.OQCCheckFinish == false && burn_in.OQCCheckBT != null && burn_in.OQCCheckFT == null)
                    {
                        FQCcheckList.Add(barcode, "条码已经在老化");
                        continue;
                    }
                }
                if (mosicScreenInfo != null)
                {
                    FQCcheckList.Add(barcode, "条码已经拼屏");
                    continue;
                }
                if (FQCcount == 0)
                {
                    FQCcheckList.Add(barcode, "FQC未检查");
                    continue;
                }
                else
                {
                    FQCcheckList.Add(barcode, "正常");
                }
            }
            return Content(JsonConvert.SerializeObject(FQCcheckList));
        }
        //检查条码是否能完成拼屏
        public ActionResult CheckMosciScreenF(string ordernum, List<string> barcodeList)
        {
            JObject checkList = new JObject();
            foreach (var item in barcodeList)
            {
                var mosicScreenInfo = db.Burn_in_MosaicScreen.Where(c => c.BarCodesNum == item).FirstOrDefault();
                if (mosicScreenInfo == null)
                {
                    checkList.Add(item, "未找到此条码");
                    continue;
                }
                else
                {
                    if (mosicScreenInfo.OQCMosaicStartTime == null)
                    {
                        checkList.Add(item, "未开始拼屏");
                        continue;
                    }
                    else if (mosicScreenInfo.OQCMosaicEndTime != null)
                    {
                        checkList.Add(item, "已完成拼屏");
                        continue;
                    }
                    else if (ordernum != mosicScreenInfo.OrderNum)
                    {
                        checkList.Add(item, "条码不属于此订单");
                        continue;
                    }
                    else
                    {
                        checkList.Add(item, "正常");
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(checkList));
        }

        #region ---------------------------------------GetOrderList()取出整个OrderMgms的OrderNum订单号列表
        private List<SelectListItem> GetOrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
            var items = new List<SelectListItem>();
            foreach (string order in orders)
            {
                items.Add(new SelectListItem
                {
                    Text = order,
                    Value = order
                });
            }
            return items;
        }
        #endregion

        #region --------------------GetnuoOrderList()取出整个OrderMgms的挪用单号列表
        private List<SelectListItem> GetnuoOrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Where(m => m.IsRepertory == true).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
            var items = new List<SelectListItem>();
            foreach (string order in orders)
            {
                items.Add(new SelectListItem
                {
                    Text = order,
                    Value = order
                });
            }
            return items;
        }
        //----------------------------------------------------------------------------------------
        #endregion

        #region --------------------GetMosaiOrderList()取出整个OrderMgms的挪用单号列表
        private List<SelectListItem> GetMosaiOrderList()
        {
            var orders = db.Burn_in_MosaicScreen.OrderByDescending(m => m.Id).Select(m => m.OrderNum).Distinct();    //增加.Distinct()后会重新按OrderNum升序排序
            var items = new List<SelectListItem>();
            foreach (string order in orders)
            {
                items.Add(new SelectListItem
                {
                    Text = order,
                    Value = order
                });
            }
            return items;
        }
        //----------------------------------------------------------------------------------------
        #endregion

        #region --------------------GetmosaiShelfNumList()取出整个OrderMgms的挪用单号列表
        private List<SelectListItem> GetmosaiShelfNumList()
        {
            var orders = db.Burn_in_MosaicScreen.OrderByDescending(m => m.Id).Select(m => m.BurnInShelfNum).Distinct();    //增加.Distinct()后会重新按OrderNum升序排序
            var items = new List<SelectListItem>();
            foreach (string order in orders)
            {
                items.Add(new SelectListItem
                {
                    Text = order,
                    Value = order
                });
            }
            return items;
        }
        //----------------------------------------------------------------------------------------
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
