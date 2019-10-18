﻿using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class PackagingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public class Sequence
        {
            public string Prefix { get; set; }

            public string Suffix { get; set; }

            public int Num { get; set; }

            public bool Rule { get; set; }

            public int startNum { get; set; }

        }
        #region 暂时不用的
        // GET: Packing_BasicInfo/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Packing_BasicInfo packing_BasicInfo = db.Packing_BasicInfo.Find(id);
        //    if (packing_BasicInfo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(packing_BasicInfo);
        //}

        //// GET: Packing_BasicInfo/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,OrderNum,Type,OuterBoxCapacity,Quantity,Creator,CreateDate,Remark")] Packing_BasicInfo packing_BasicInfo)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Packing_BasicInfo.Add(packing_BasicInfo);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(packing_BasicInfo);
        //}

        //// GET: Packing_BasicInfo/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Packing_BasicInfo packing_BasicInfo = db.Packing_BasicInfo.Find(id);
        //    if (packing_BasicInfo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(packing_BasicInfo);
        //}

        //// POST: Packing_BasicInfo/Edit/5
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,OrderNum,Type,OuterBoxCapacity,Quantity,Creator,CreateDate,Remark")] Packing_BasicInfo packing_BasicInfo)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(packing_BasicInfo).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(packing_BasicInfo);
        //}

        //// GET: Packing_BasicInfo/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Packing_BasicInfo packing_BasicInfo = db.Packing_BasicInfo.Find(id);
        //    if (packing_BasicInfo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(packing_BasicInfo);
        //}

        //// POST: Packing_BasicInfo/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Packing_BasicInfo packing_BasicInfo = db.Packing_BasicInfo.Find(id);
        //    db.Packing_BasicInfo.Remove(packing_BasicInfo);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //#region -------包装首页---------

        //// GET: Packagings
        //public async Task<ActionResult> Index()
        //{
        //    ViewBag.Display = "display:none";//隐藏View基本情况信息
        //    ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
        //    return View(await db.Packaging.ToListAsync());
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Index(string OrderNum, string searchString, int PageIndex = 0)
        //{
        //    var AllPackagingRecords = db.Packaging as IQueryable<Packaging>;
        //    List<Packaging> AllPackagingRecordsList = null;
        //    if (String.IsNullOrEmpty(OrderNum))
        //    {
        //        //调出全部记录      
        //        AllPackagingRecords = from m in db.Packaging
        //                              select m;
        //    }
        //    else
        //    {
        //        //筛选出对应orderNum所有记录
        //        AllPackagingRecords = from m in db.Packaging
        //                              where (m.OrderNum == OrderNum)
        //                               select m;
        //    }
        //    //检查orderNum和searchString是否为空
        //    if (!String.IsNullOrEmpty(searchString))
        //    {   //从调出的记录中筛选含searchString内容的记录
        //        AllPackagingRecords = AllPackagingRecords.Where(m => m.OrderNum.Contains(searchString));
        //    }

        //    //取出对应orderNum包装时长所有记录
        //    IQueryable<TimeSpan?> TimeSpanList = from m in db.Packaging
        //                                         where (m.OrderNum == OrderNum)
        //                                         orderby m.OQCCheckTime
        //                                         select m.OQCCheckTime;
        //    //计算外观电检总时长
        //    TimeSpan TotalTimeSpan = DateTime.Now - DateTime.Now;
        //    if (AllPackagingRecords.Where(x => x.Packaging_OQCCheckAbnormal == 1).Count() != 0)    //Packaging_OQCCheckAbnormal的值是1为正常
        //    {
        //        foreach (var m in TimeSpanList)
        //        {
        //            if (m != null)
        //            {
        //                TotalTimeSpan = TotalTimeSpan.Add(m.Value).Duration();
        //            }
        //        }
        //        ViewBag.TotalTimeSpan = TotalTimeSpan.Hours.ToString() + "小时" + TotalTimeSpan.Minutes.ToString() + "分" + TotalTimeSpan.Seconds.ToString() + "秒";
        //    }
        //    else
        //    {
        //        ViewBag.TotalTimeSpan = "暂时没有已完成包装的模组";
        //    }

        //    //计算平均用时
        //    TimeSpan AvgTimeSpan = DateTime.Now - DateTime.Now;
        //    int Order_CR_valid_Count = AllPackagingRecords.Where(x => x.OQCCheckTime != null).Count();
        //    int TotalTimeSpanSecond = Convert.ToInt32(TotalTimeSpan.Hours.ToString()) * 3600 + Convert.ToInt32(TotalTimeSpan.Minutes.ToString()) * 60 + Convert.ToInt32(TotalTimeSpan.Seconds.ToString());
        //    int AvgTimeSpanInSecond = 0;
        //    if (Order_CR_valid_Count != 0)
        //    {
        //        AvgTimeSpanInSecond = TotalTimeSpanSecond / Order_CR_valid_Count;
        //        int AvgTimeSpanMinute = AvgTimeSpanInSecond / 60;
        //        int AvgTimeSpanSecond = AvgTimeSpanInSecond % 60;
        //        ViewBag.AvgTimeSpan = AvgTimeSpanMinute + "分" + AvgTimeSpanSecond + "秒";//向View传递计算平均用时
        //    }
        //    else
        //    {
        //        ViewBag.AvgTimeSpan = "暂时没有已完成包装的模组";//向View传递计算平均用时
        //    }

        //    //列出记录
        //    AllPackagingRecordsList = AllPackagingRecords.ToList();
        //    //统计包装结果正常的模组数量
        //    var Order_CR_Normal_Count = AllPackagingRecords.Where(x => x.Packaging_OQCCheckAbnormal == 1).Count();
        //    var Abnormal_Count = AllPackagingRecords.Where(x => x.Packaging_OQCCheckAbnormal != 1).Count();
        //    //读出订单中模组总数量
        //    var Order_MG_Quantity = (from m in db.OrderMgm
        //                             where (m.OrderNum == OrderNum)
        //                             select m.Boxes).FirstOrDefault();
        //    //将模组总数量、正常的模组数量、未完成包装模组数量、订单号信息传递到View页面
        //    ViewBag.Quantity = Order_MG_Quantity;
        //    ViewBag.NormalCount = Order_CR_Normal_Count;
        //    ViewBag.AbnormalCount = Abnormal_Count;
        //    ViewBag.RecordCount = AllPackagingRecords.Count();
        //    ViewBag.NeverFinish = Order_MG_Quantity - Order_CR_Normal_Count;
        //    ViewBag.orderNum = OrderNum;

        //    //未选择订单时隐藏基本信息设置
        //    if (ViewBag.Quantity == 0)
        //    { ViewBag.Display = "display:none"; }
        //    else { ViewBag.Display = "display:normal"; }

        //    ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.


        //    //分页计算功能
        //    var recordCount = AllPackagingRecords.Count();
        //    var pageCount = GetPageCount(recordCount);
        //    if (PageIndex >= pageCount && pageCount >= 1)
        //    {
        //        PageIndex = pageCount - 1;
        //    }
        //    AllPackagingRecords = AllPackagingRecords.OrderByDescending(m => m.OQCCheckBT)
        //                        .Skip(PageIndex * PAGE_SIZE)
        //                        .Take(PAGE_SIZE);
        //    ViewBag.PageIndex = PageIndex;
        //    ViewBag.PageCount = pageCount;
        //    ViewBag.OrderNumList = GetOrderNumList();

        //    return View(AllPackagingRecordsList);
        //}

        //#endregion


        //// GET: Packagings/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Packaging packaging = await db.Packaging.FindAsync(id);
        //    if (packaging == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(packaging);
        //}

        //// GET: Packagings/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Packagings/Create
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Packaging_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Packaging packaging)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Packaging.Add(packaging);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    return View(packaging);
        //}

        //// GET: Packagings/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Packaging packaging = await db.Packaging.FindAsync(id);
        //    if (packaging == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(packaging);
        //}

        //// POST: Packagings/Edit/5
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Packaging_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Packaging packaging)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(packaging).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(packaging);
        //}

        //// GET: Packagings/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Packaging packaging = await db.Packaging.FindAsync(id);
        //    if (packaging == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(packaging);
        //}

        //// POST: Packagings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Packaging packaging = await db.Packaging.FindAsync(id);
        //    db.Packaging.Remove(packaging);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}


        //#region ----------包装开始-----------------


        //// GET: Packagings/Packaging_B
        //public ActionResult Packaging_B()
        //{
        //    if (Session["User"] == null)
        //    {
        //        return RedirectToAction("Login", "Users", new { col = "Packagings", act = "Packaging_B" });
        //    }
        //    ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
        //    return View();
        //}

        //// POST: Packaging/Create
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Packaging_B([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Packaging_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Packaging Packaging)
        //{
        //    if (Session["User"] == null)
        //    {
        //        return RedirectToAction("Login", "Users" ,new { col = "Packagings", act = "Packaging_B" });
        //    }
        //    Packaging.OQCCheckBT = DateTime.Now;

        //    ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

        //    if (ModelState.IsValid)
        //    {
        //        //Burn_in burn_incheck = await db.Burn_in.FindAsync(burn_in.BarCodesNum);

        //        //if (burn_incheck==null)
        //        //{ 
        //        //db.Burn_in.Add(burn_in);
        //        //await db.SaveChangesAsync();
        //        //return RedirectToAction("Burn_in_F", new { id = burn_in.Id });
        //        ////return RedirectToAction("Index");
        //        // }
        //        //else
        //        //{
        //        //    return RedirectToAction("Burn_in_F", new { id = burn_incheck.Id });
        //        //}
        //        db.Packaging.Add(Packaging);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Packaging_F", new { id = Packaging.Id });
        //    }
        //    return View(Packaging);
        //}
        //#endregion


        //#region ------------包装完成------------
        //// GET: Packagings/Packaging_F
        //public ActionResult Packaging_F(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Packaging packaging = db.Packaging.Find(id);
        //    if (packaging == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(packaging);
        //}

        //// POST: Packagings/Packaging_F
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Packaging_F([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Packaging_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Packaging Packaging)
        //{
        //    if (Session["User"] == null)
        //    {
        //        return RedirectToAction("Login", "Users", new { col = "Packagings", act = "Packaging_F"+"/"+Packaging.Id.ToString() });
        //    }
        //    if (Packaging.OQCCheckFT == null)
        //    {
        //        Packaging.OQCCheckFT = DateTime.Now;
        //        Packaging.OQCPrincipal = ((Users)Session["User"]).UserName;
        //        var BT = Packaging.OQCCheckBT.Value;
        //        var FT = Packaging.OQCCheckFT.Value;
        //        var CT = FT - BT;
        //        Packaging.OQCCheckTime = CT;
        //        Packaging.OQCCheckTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
        //        Packaging.OQCCheckFinish = true;
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(Packaging).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Packaging_B");
        //    }
        //    ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
        //    return View(Packaging);
        //}

        //#endregion



        //#region ------------------ 取出整个OrderMgms的OrderNum订单号列表.--------------------------------------------------
        //private List<SelectListItem> GetOrderList()
        //{
        //    var orders = db.OrderMgm.OrderByDescending(m => m.OrderCreateDate).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
        //    var items = new List<SelectListItem>();
        //    foreach (string order in orders)
        //    {
        //        items.Add(new SelectListItem
        //        {
        //            Text = order,
        //            Value = order
        //        });
        //    }
        //    return items;
        //}
        ////----------------------------------------------------------------------------------------
        //#endregion

        //#region  -------------检索订单号------
        //private List<SelectListItem> GetOrderNumList()
        //{
        //    var ordernum = db.OrderMgm.OrderBy(m => m.OrderNum).Select(m => m.OrderNum).Distinct();

        //    var ordernumitems = new List<SelectListItem>();
        //    foreach (string num in ordernum)
        //    {
        //        ordernumitems.Add(new SelectListItem
        //        {
        //            Text = num,
        //            Value = num
        //        });
        //    }
        //    return ordernumitems;
        //}
        //#endregion

        //#region  -----------分页------------
        //private static readonly int PAGE_SIZE = 10;

        //private int GetPageCount(int recordCount)
        //{
        //    int pageCount = recordCount / PAGE_SIZE;
        //    if (recordCount % PAGE_SIZE != 0)
        //    {
        //        pageCount += 1;
        //    }
        //    return pageCount;
        //}
        //#endregion

        /*
    //外箱QOC确认箱体号显示
    //public ActionResult DisplayWarehouseOQC(string ordernum)
    //{
    //    JObject modeuleJobject = new JObject();
    //    JObject order = new JObject();
    //    var boxNum = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.IsOut == false).Select(c => c.OuterBoxBarcode).Distinct().ToList();
    //    int i = 0;
    //    foreach (var boxbarcode in boxNum)
    //    {
    //        var moduleNumList = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.OuterBoxBarcode == boxbarcode ).Select(c => c.ModuleGroupNum).ToList();
    //        var warehounum = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.OuterBoxBarcode == boxbarcode).Select(c => c.WarehouseNum).FirstOrDefault();
    //        modeuleJobject.Add("module", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(moduleNumList)));
    //        modeuleJobject.Add("warehounum", warehounum);
    //        modeuleJobject.Add("outherNum", boxbarcode);
    //        modeuleJobject.Add("status", false);
    //        order.Add(i.ToString(), modeuleJobject);
    //        modeuleJobject = new JObject();
    //        i++;
    //    }
    //    return Content(JsonConvert.SerializeObject(order));
    //}
    */
        //外箱条码生产
        //public string GetOuterBoxBarCode(string ordernum)
        //{
        //    string[] str = ordernum.Split('-');
        //    string start = str[0].Substring(2);
        //    //得到订单总的箱体数
        //    //var OuterBoxCapacity = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Select(c => c.OuterBoxCapacity).ToList();
        //    //var count = OuterBoxCapacity.Sum();
        //    var count = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Sum(c => c.OuterBoxCapacity);
        //    //已经打印的箱体数
        //    //var printCount = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum).Select(c => c.OuterBoxBarcode).Distinct().ToList().Count();
        //    //string Num = (count - printCount).ToString().PadLeft(3, '0');

        //    string OuterBoxBarCodeNum = start + str[1] + str[2] + "-";
        //    for (int i = 1; i <= count; i++)
        //    {
        //        if (db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == (OuterBoxBarCodeNum + i.ToString().PadLeft(3, '0'))) == 0)
        //        {
        //            OuterBoxBarCodeNum = OuterBoxBarCodeNum + i.ToString().PadLeft(3, '0');
        //            return OuterBoxBarCodeNum;
        //        }
        //    }
        //    return "";
        //}

        //通过订单，显示里面的箱体号
        //public ActionResult BoxNum(string ordernum)
        //{
        //    JObject modeuleJobject = new JObject();
        //    JObject order = new JObject();
        //    var boxNum = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum ).Select(c => c.OuterBoxBarcode).Distinct().ToList();
        //    int i = 0;
        //    foreach (var boxbarcode in boxNum)
        //    {
        //        var moduleNumList = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum && c.OuterBoxBarcode == boxbarcode).Select(c => c.ModuleGroupNum).ToList();
        //        modeuleJobject.Add("module", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(moduleNumList)));
        //        modeuleJobject.Add("outherNum", boxbarcode);
        //        modeuleJobject.Add("status", false);
        //        order.Add(i.ToString(), modeuleJobject);
        //        modeuleJobject = new JObject();
        //        i++;
        //    }
        //    return Content(JsonConvert.SerializeObject(order));
        //}
        #endregion

        #region 内箱包装信息录入
        //添加包装基本信息,除了尾箱，一个订单的包装类型的信息应该只有一条
        [HttpPost]
        public ActionResult CreatePacking(List<Packing_BasicInfo> packinginfo)
        {
            #region  版本1
            //先删除原有的
            var ordernum = packinginfo.Select(c => c.OrderNum).FirstOrDefault();
            var list = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).ToList();
            db.Packing_BasicInfo.RemoveRange(list);
            db.SaveChanges();
            //生成新的
            if (ModelState.IsValid)
            {
                db.Packing_BasicInfo.AddRange(packinginfo);
                db.SaveChanges();
                return Content("ok");
            }
            #endregion
            return View();
        }

        //根据给的订单号，显示包装信息
        public ActionResult GetValueFromOrderNum(string ordernum)
        {
            JObject valueitem = new JObject();
            JObject value = new JObject();
            var packingList = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).ToList();
            if (packingList == null)
            {
                return Content("");
            }
            int i = 0;
            foreach (var item in packingList)
            {
                valueitem.Add("packingType", item.Type);
                valueitem.Add("itemNum", item.OuterBoxCapacity);
                valueitem.Add("Num", item.Quantity);
                if (item.IsSeparate)
                {
                    valueitem.Add("isSeparate", true);
                    valueitem.Add("screenNum", item.ScreenNum);
                    var print = db.Packing_BarCodePrinting.Where(c => c.OrderNum == item.OrderNum && c.Type == item.Type).ToList();
                    if (print.Count == 0)
                    {
                        valueitem.Add("update", "true");
                        valueitem.Add("min", 0);
                    }
                    else
                    {
                        valueitem.Add("update", "false");
                        var count = db.Packing_BarCodePrinting.Where(c => c.OrderNum == item.OrderNum && c.Type == item.Type&&c.ScreenNum==item.ScreenNum).Select(c => c.OuterBoxBarcode).Distinct().Count();
                        valueitem.Add("min", count);
                    }
                }
                else
                {
                    valueitem.Add("isSeparate", false);
                    valueitem.Add("screenNum", null);
                    var print = db.Packing_BarCodePrinting.Where(c => c.OrderNum == item.OrderNum && c.Type == item.Type).ToList();
                    if (print.Count == 0)
                    {
                        valueitem.Add("update", "true");
                        valueitem.Add("min", 0);
                    }
                    else
                    {
                        valueitem.Add("update", "false");
                        var count = db.Packing_BarCodePrinting.Where(c => c.OrderNum == item.OrderNum && c.Type == item.Type).Select(c => c.OuterBoxBarcode).Distinct().Count();
                        valueitem.Add("min", count);
                    }
                }
                
                value.Add(i.ToString(), valueitem);
                i++;
                valueitem = new JObject();
            }
            return Content(JsonConvert.SerializeObject(value));
        }
        // 内箱确认录入
        public ActionResult InnerBoxCheck(List<Packing_InnerCheck> packing_InnerChecks)
        {
            if (ModelState.IsValid)
            {
                string error = "";
                foreach (var item in packing_InnerChecks)
                {
                    var check = db.Packing_InnerCheck.Count(c => c.Barcode == item.Barcode);
                    if (check > 0)
                    {
                        error = error + item.Barcode + ",";
                        continue;
                    }
                    item.QC_Operator = ((Users)Session["User"]).UserName;
                    item.QC_ComfirmDate = DateTime.Now;
                    db.Packing_InnerCheck.Add(item);
                    db.SaveChanges();
                };
                //db.Packing_InnerCheck.AddRange(packing_InnerChecks);
                //db.SaveChanges();
                if (!string.IsNullOrEmpty(error))
                {
                    return Content(error + "条码已重复");
                }
                return Content("true");
            }
            return Content("false");
        }

        //输出已经确认的条码列表
        public ActionResult GetBarcodeList(string ordernum)
        {
            var list = db.Packing_InnerCheck.Where(c => c.OrderNum == ordernum).OrderBy(c => c.Barcode).ToList();
            JArray List = new JArray();
            foreach (var item in list)
            {
                JObject itemjob = new JObject();
                itemjob.Add("barcode", item.Barcode);
                itemjob.Add("boxcode", item.ModuleNum);
                List.Add(itemjob);
            }
            return Content(JsonConvert.SerializeObject(List));
        }
        #endregion

        #region 外箱操作
        //打印条码界面，显示完成数量
        public ActionResult GetcompleteInfo(string ordernum)
        {

            var type = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Select(c => c.Type).Distinct().ToList();
            JArray total = new JArray();
            foreach (var item in type)
            {
                var screenNumList = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == item).Select(c => c.ScreenNum).ToList();
                foreach (var screenNum in screenNumList)
                {
                    JObject info = new JObject();
                    var totleNum = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNum).Sum(c => c.Quantity);
                    var printBarcodeinfo = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNum).Select(c => c.OuterBoxBarcode).Distinct();
                    int printBarcode = printBarcodeinfo.Count();
                    //类型
                    info.Add("type", item);
                    //完成数量
                    info.Add("completeNum", printBarcode.ToString() + "/" + totleNum.ToString());
                    //屏序
                    info.Add("screenNum", screenNum);
                    //完成率
                    info.Add("complete", totleNum == 0 ? 0 + "%" : ((printBarcode * 100) / totleNum).ToString("F2") + "%");

                    total.Add(info);
                }
            }
            JObject info2 = new JObject();
            var printBarcodeinfo2 = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum).Select(c => c.OuterBoxBarcode).Distinct().Count();
            var totle = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).ToList();
            var totleNum2 = 0;
            if (totle.Count!=0)
            {
                 totleNum2 = totle.Sum(c => c.Quantity);
            }
           
            //类型
            info2.Add("type", "总计");
            //完成数量
            info2.Add("completeNum", printBarcodeinfo2.ToString() + "/" + totleNum2.ToString());
            //屏序
            info2.Add("screenNum", "--");
            //完成率
            info2.Add("complete", totleNum2 == 0 ? 0 + "%" : ((printBarcodeinfo2 * 100) / totleNum2).ToString("F2") + "%");

            total.Add(info2);

            return Content(JsonConvert.SerializeObject(total));
        }

        //根据条码号，显示箱体号
        public string GetModulbarCode(string barcode)
        {
            var modul = db.BarCodes.Where(c => c.BarCodesNum == barcode).Select(c => c.ModuleGroupNum).FirstOrDefault();
            if (!string.IsNullOrEmpty(modul))
            {
                return modul;
            }
            else
                return "";
        }

        //判断条码与订单是否相符
        public string IsCheckBarcode(string ordernum, string barcode, bool isInner = false)
        {
            var order = db.BarCodes.Where(c => c.BarCodesNum == barcode).Select(c => c.OrderNum).FirstOrDefault();
            var exit = db.Appearance.Count(c => c.BarCodesNum == barcode && c.OQCCheckFinish == true);
            var iscCheck = db.Packing_InnerCheck.Count(c => c.Barcode == barcode);
            if (order == null)
            {
                return "不存在此条码";
            }
            else if (order != ordernum)
            {
                return "此条码的订单号应为" + order;
            }
            else if (exit == 0)
            {
                return "此条码未通过外观电检";
            }
            else if (isInner == true && iscCheck != 0)
            {
                return "此条码已完成过内箱装箱确认";
            }
            else
            {
                return "true";
            }
        }
        //通过输入订单号和包装类型，传出数量
        public int GetNum(string ordernum, string type, int screenNum = 1)
        {
            #region 旧版本
            var printBarcode = db.Packing_BarCodePrinting.Count(c => c.OrderNum == ordernum && c.Type == type && c.ScreenNum == screenNum);
            var typeNum = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == type && c.ScreenNum == screenNum).ToList();
            //没有在订单中找到此包装类型
            if (typeNum.Count == 0)
            {
                return 0;
            }
            //如果只有一条数据，说明此订单没有选择此包箱类型做尾箱
            else
            {
                if (typeNum.Count == 1)
                {
                    var num = typeNum.FirstOrDefault().Quantity;
                    var boxnum = typeNum.FirstOrDefault().OuterBoxCapacity;
                    //查看打印的条码数是否超过定义数
                    if (printBarcode < (num * boxnum))
                    {
                        return boxnum;
                    }
                    else
                        return 0;
                }
                //如果有2条数据，说明此订单选择此包箱类型做尾箱
                if (typeNum.Count == 2)
                {
                    var numMin = typeNum.Min(c => c.Quantity);
                    var numMax = typeNum.Max(c => c.Quantity);
                    var boxnumMin = typeNum.Where(c => c.Quantity == numMin).FirstOrDefault().OuterBoxCapacity;
                    var boxnumMax = typeNum.Where(c => c.Quantity == numMax).FirstOrDefault().OuterBoxCapacity;
                    //整箱没打印完，先打印整箱
                    if (printBarcode < numMax * boxnumMax)
                    {
                        return boxnumMax;
                    }
                    //整箱打印完，尾箱没打印
                    else if (printBarcode < ((numMax * boxnumMax) + numMin * boxnumMin))
                    {
                        return boxnumMin;
                    }
                    //已经打印完了
                    else
                        return 0;
                }
            }
            return 0;
            #endregion


        }

        //外箱条码标签信息
        public ActionResult GetOuterBoxBarCodeInfo(string ordernum, int screenNum)
        {
            //得到订单总的箱体数
            //var OuterBoxCapacity = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Select(c => c.Quantity).ToList();
            //var count = OuterBoxCapacity.Sum();

            var count = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum).Sum(c => c.Quantity);
            //已经打印的箱体数
            var printCount = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum).Select(c => c.OuterBoxBarcode).Distinct().ToList().Count();
            if (printCount < count)
            {
                JObject info = new JObject();

                #region 外箱条码生成
                //string[] str = ordernum.Split('-');
                //string start = str[0].Substring(2);
                //var lastNum = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum).OrderByDescending(c => c.Date).Select(c => c.OuterBoxBarcode).FirstOrDefault();
                //string Num = "001";
                //if (lastNum != null)
                //{
                //    string[] boxNum = lastNum.Split('-');
                //    Num = (int.Parse(boxNum[2]) + 1).ToString().PadLeft(3, '0');
                //}

                string[] str = ordernum.Split('-');
                string start = str[0].Substring(2);
                string OuterBoxBarCodeNum = start + str[1] + "-" + str[2] + "-" + screenNum.ToString().PadLeft(2, '0') + "-";
                int SN = 0;
                for (int i = 1; i <= count; i++)
                {
                    var num = (OuterBoxBarCodeNum + i.ToString().PadLeft(3, '0'));
                    if (db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == num) == 0)
                    {
                        OuterBoxBarCodeNum = OuterBoxBarCodeNum + i.ToString().PadLeft(3, '0');
                        SN = i;
                        //外箱条码
                        info.Add("boxNum", OuterBoxBarCodeNum);
                        //SN/TN
                        info.Add("SNTN", SN + "/" + count);
                        return Content(JsonConvert.SerializeObject(info));
                    }

                }
                #endregion
                return null;
            }
            else
            {
                return null;
            }

        }

        //通过输入外箱条码显示标签信息
        public ActionResult GetOutherBoxBarCodeInfoFromOutherBarcode(string outherbarcode)
        {
            JObject info = new JObject();
            var barcode = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outherbarcode).ToList();
            if (barcode.Count == 0)
            {
                return Content("false");
            }

            else
            {
                //订单号
                info.Add("OrderNum", barcode.FirstOrDefault().OrderNum);
                //外箱条码号
                info.Add("OuterBoxBarcode", barcode.FirstOrDefault().OuterBoxBarcode);
                //是否有logo
                info.Add("IsLogo", barcode.FirstOrDefault().IsLogo);
                //物料描述
                info.Add("Materiel", barcode.FirstOrDefault().Materiel);
                //SN/TN
                var order = barcode.FirstOrDefault().OrderNum;
                var count = db.Packing_BasicInfo.Where(c => c.OrderNum == order).Sum(c => c.Quantity).ToString();
                info.Add("SNTN", barcode.FirstOrDefault().SNTN + "/" + count);
                //数量
                info.Add("Count", barcode.Count());
                //装箱类型
                info.Add("Type", barcode.FirstOrDefault().Type);
                //模组列表
                JArray moduleNum = new JArray();
                foreach (var item in barcode)
                {
                    JObject bar = new JObject();
                    bar.Add("barcode", item.BarCodeNum);
                    bar.Add("boxcode", item.ModuleGroupNum == null ? "" : item.ModuleGroupNum);
                    moduleNum.Add(bar);
                }
                info.Add("ModuleNum", moduleNum);
            }
            return Content(JsonConvert.SerializeObject(info));
        }

        public string CheckBarcode(string barcode)
        {
            var exit = db.Packing_BarCodePrinting.Where(c => c.BarCodeNum == barcode).Select(c => c.BarCodeNum).FirstOrDefault();
            if (exit != null)
            {
                return "false";
            }
            else
                return "true";
        }

        //创建外箱打印记录
        public string CreatePackangPrint(List<Packing_BarCodePrinting> printings, bool isupdate = false)
        {
            if (ModelState.IsValid)
            {
                if (isupdate)//更新条码信息
                {
                    string outherbarcode = printings.FirstOrDefault().OuterBoxBarcode;
                    var delete = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outherbarcode).ToList();
                    db.Packing_BarCodePrinting.AddRange(printings);
                    db.Packing_BarCodePrinting.RemoveRange(delete);
                    db.SaveChanges();
                    return "true";
                }
                string error = "";
                foreach (var item in printings)
                {
                    var exit = db.Packing_BarCodePrinting.Where(c => c.BarCodeNum == item.BarCodeNum).FirstOrDefault();
                    if (exit != null)
                    {
                        error = error + exit.BarCodeNum + "已包装在" + exit.OuterBoxBarcode + ",";
                    }
                }
                if (!string.IsNullOrEmpty(error))
                {
                    return error;
                }
                else
                {
                    var temp = printings.FirstOrDefault().OrderNum;
                    var count = db.Packing_BarCodePrinting.Count(c => c.OrderNum == temp);
                    var real = db.Packing_BasicInfo.Where(c => c.OrderNum == temp);
                    int realCount = 0;
                    foreach (var item in real)
                    {
                        realCount = realCount + (item.Quantity * item.OuterBoxCapacity);
                    }
                    if (realCount < count + printings.Count())
                    {
                        return "已超过定义的包装数量";
                    }
                    db.Packing_BarCodePrinting.AddRange(printings);
                    db.SaveChanges();
                    return "true";
                }
            }
            return "传入类型不对，请确认";
        }

        //外箱条码OQC确认 没用
        public void CheckOQC(string ordernum, List<string> outherboxbarcode)
        {
            foreach (var barcode in outherboxbarcode)
            {
                var list = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum && c.OuterBoxBarcode == barcode).ToList();
                if (list.Count != 0)
                {
                    foreach (var item in list)
                    {
                        item.QC_Operator = ((Users)Session["User"]).UserName;
                        item.QC_ComfirmDate = DateTime.Now;
                        db.SaveChangesAsync();
                    }

                }
            }
        }

        //删除外箱条码记录
        public void DeleteBarcode(string ordernum, List<string> barcodelist, bool inside = false)
        {
            string message = "";
            foreach (var item in barcodelist)
            {
                if (inside)
                {
                    var warehous = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.OuterBoxBarcode == item).ToList();
                    db.Warehouse_Join.RemoveRange(warehous);
                    db.SaveChanges();

                    string barcdeoList = "";
                    foreach (var barcode in warehous)
                    {
                        if (barcdeoList == "")
                            barcdeoList = barcode.BarCodeNum;
                        else
                            barcdeoList = barcdeoList + "," + barcode.BarCodeNum;
                    }
                    message = message + "外箱条码:" + item + ",模组条码:" + barcdeoList;
                }
                else
                {
                    var list = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum && c.OuterBoxBarcode == item).ToList();
                    db.Packing_BarCodePrinting.RemoveRange(list);
                    db.SaveChanges();

                    string barcdeoList = "";
                    foreach (var barcode in list)
                    {
                        if (barcdeoList == "")
                            barcdeoList = barcode.BarCodeNum;
                        else
                            barcdeoList = barcdeoList + "," + barcode.BarCodeNum;
                    }
                    message = message + "外箱条码:" + item + ",模组条码:" + barcdeoList;
                }
            }
            UserOperateLog log = new UserOperateLog { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = inside == true ? ("删除入库记录//" + message) : ("删除外箱记录//" + message) };
            db.UserOperateLog.Add(log);
            db.SaveChanges();
        }

        #endregion

        #region 仓库录入
        //外箱录入确认箱体号显示，只用其中一个
        public ActionResult DisplayWarehouseInser(string ordernum)
        {
            JObject modeuleJobject = new JObject();
            JObject order = new JObject();
            var boxNum = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum).Select(c => c.OuterBoxBarcode).Distinct().ToList();
            int i = 0;
            foreach (var boxbarcode in boxNum)
            {
                var warehoure = db.Warehouse_Join.Count(c => c.OrderNum == ordernum && c.OuterBoxBarcode == boxbarcode&&c.IsOut==false);
                if (warehoure > 0)
                {
                    continue;
                }
                var moduleNumList = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum && c.OuterBoxBarcode == boxbarcode).ToList();
                moduleNumList.ForEach(c => c.ModuleGroupNum = c.ModuleGroupNum == null ? c.BarCodeNum : c.ModuleGroupNum);
                var list = moduleNumList.Select(c => c.ModuleGroupNum);
                modeuleJobject.Add("module", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(list)));
                modeuleJobject.Add("outherNum", boxbarcode);
                modeuleJobject.Add("status", false);
                order.Add(i.ToString(), modeuleJobject);
                modeuleJobject = new JObject();
                i++; ;
            }
            return Content(JsonConvert.SerializeObject(order));
        }
        //外箱录入确认箱体号，根据条码搜索 只用其中一个
        public ActionResult DisplayWarehouseInserFromBarcode(List<string> barcodeList)
        {
            JObject modeuleJobject = new JObject();
            JObject order = new JObject();
            var boxNum = new List<string>();
            boxNum = db.Packing_BarCodePrinting.Where(c => barcodeList.Contains(c.OuterBoxBarcode)).Select(c => c.OuterBoxBarcode).Distinct().ToList();
            if (boxNum.Count() == 0)
            {
                boxNum = db.Packing_BarCodePrinting.Where(c => barcodeList.Contains(c.BarCodeNum)).Select(c => c.OuterBoxBarcode).Distinct().ToList();
            }
            int i = 0;
            foreach (var boxbarcode in boxNum)
            {
                var warehoure = db.Warehouse_Join.Count(c => c.OuterBoxBarcode == boxbarcode&&c.IsOut==false);
                if (warehoure > 0)
                {
                    continue;
                }
                var moduleNumList = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == boxbarcode).ToList();
                moduleNumList.ForEach(c => c.ModuleGroupNum = c.ModuleGroupNum == null ? c.BarCodeNum : c.ModuleGroupNum);
                var list = moduleNumList.Select(c => c.ModuleGroupNum);
                modeuleJobject.Add("module", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(list)));
                modeuleJobject.Add("outherNum", boxbarcode);
                modeuleJobject.Add("status", false);
                order.Add(i.ToString(), modeuleJobject);
                modeuleJobject = new JObject();
                i++; ;
            }
            return Content(JsonConvert.SerializeObject(order));
        }

        //外箱入库信息显示
        public ActionResult DisaplyMessage(string outerBarcode)
        {
            JObject message = new JObject();

            var warejoin = db.Warehouse_Join.Count(c => c.OuterBoxBarcode == outerBarcode&&c.IsOut==false);
            if (warejoin > 0)
            {
                message.Add("ordernum", null);
                message.Add("table", null);
                message.Add("barcode", null);
                message.Add("message", "此条码已入库");
                return Content(JsonConvert.SerializeObject(message));
            }

            var ordernum = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outerBarcode).Select(c => c.OrderNum).FirstOrDefault();
            if (ordernum == null)
            {
                message.Add("ordernum", null);
                message.Add("table", null);
                message.Add("barcode", null);
                message.Add("message", "找不到此条码");
                return Content(JsonConvert.SerializeObject(message));
            }
            message.Add("ordernum", ordernum);
            var type = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Select(c => c.Type).Distinct().ToList();
            JArray total = new JArray();
            foreach (var item in type)
            {
                var printBarcodeinfo = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum && c.Type == item).Select(c => c.OuterBoxBarcode).Distinct();
                int printBarcode = printBarcodeinfo.Count();
                var screenNumList = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == item).Select(c => c.ScreenNum).ToList();
                foreach (var screenNum in screenNumList)
                {
                    JObject info = new JObject();
                    var totleNum = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNum).Sum(c => c.Quantity);

                    //类型
                    info.Add("type", item);
                    //完成数量
                    info.Add("completeNum", printBarcode.ToString() + "/" + totleNum.ToString());
                    //屏序
                    info.Add("screenNum", screenNum);
                    //完成率
                    info.Add("complete", totleNum == 0 ? 0 + "%" : ((printBarcode * 100) / totleNum).ToString("F2") + "%");

                    total.Add(info);
                }
            }
            JObject info2 = new JObject();
            var printBarcodeinfo2 = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum).Select(c => c.OuterBoxBarcode).Distinct().Count();
            var totleNum2 = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Sum(c => c.Quantity);
            //类型
            info2.Add("type", "总计");
            //完成数量
            info2.Add("completeNum", printBarcodeinfo2.ToString() + "/" + totleNum2.ToString());
            //屏序
            info2.Add("screenNum", "--");
            //完成率
            info2.Add("complete", totleNum2 == 0 ? 0 + "%" : ((printBarcodeinfo2 * 100) / totleNum2).ToString("F2") + "%");

            total.Add(info2);

            message.Add("table", total);
            message.Add("barcode", outerBarcode);
            message.Add("message", "");
            return Content(JsonConvert.SerializeObject(message));
        }

        //外箱入库录入
        public ActionResult CretecWarehouseInfo(string warehouseNum, List<string> outherboxbarcode)
        {
            if (string.IsNullOrEmpty(warehouseNum) || outherboxbarcode.Count == 0)
            {

            }
            JArray total = new JArray();
            List<string> ordernumList = new List<string>();
            foreach (var outheritem in outherboxbarcode)
            {
                //外箱条码号已包装的记录
                var barcodeList = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outheritem).ToList();
                //订单号
                var ordernum = barcodeList.FirstOrDefault().OrderNum;
                if (!ordernumList.Contains(ordernum))
                {
                    ordernumList.Add(ordernum);
                }
                foreach (var item in barcodeList)
                {
                    Warehouse_Join join = new Warehouse_Join() { OrderNum = ordernum, BarCodeNum = item.BarCodeNum, ModuleGroupNum = item.ModuleGroupNum, OuterBoxBarcode = outheritem, Operator = ((Users)Session["User"]).UserName, Date = DateTime.Now, WarehouseNum = warehouseNum };
                    db.Warehouse_Join.Add(join);
                    db.SaveChangesAsync();
                }
                //((Users)Session["User"]).UserName
            }

            foreach (var item in ordernumList)
            {
                JObject orderjobject = new JObject();
                orderjobject.Add("orderNum", item);

                var outherBacode = db.Warehouse_Join.Where(c => c.OrderNum == item && outherboxbarcode.Contains(c.OuterBoxBarcode)).Select(c => c.OuterBoxBarcode).Distinct().ToList();
                JArray barcode = new JArray();
                barcode.Add(outherBacode);
                orderjobject.Add("barcode", barcode);
                //计划包装数量
                var planPrint = db.Packing_BasicInfo.Where(c => c.OrderNum == item).Sum(c => c.Quantity);
                //已包装数量
                var printCount = db.Packing_BarCodePrinting.Where(c => c.OrderNum == item).Select(c => c.OuterBoxBarcode).Distinct().Count();
                //入库数量
                var warhhousr = db.Warehouse_Join.Where(c => c.OrderNum == item).Select(c => c.OuterBoxBarcode).Distinct().Count();

                //已入库
                orderjobject.Add("warehousjoinCount", warhhousr);
                //剩下已包装未入库
                orderjobject.Add("printCount", printCount - warhhousr);
                //未包装数量
                orderjobject.Add("notPrintCount", planPrint - printCount);
                total.Add(orderjobject);
            }
            return Content(JsonConvert.SerializeObject(total));

        }



        //外箱入库OQC确认没用
        public void CheckWarehouseOQC(string ordernum, List<string> outherboxbarcode)
        {
            foreach (var barcode in outherboxbarcode)
            {
                var list = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.OuterBoxBarcode == barcode).ToList();
                if (list.Count != 0)
                {
                    foreach (var item in list)
                    {
                        item.QC_Operator = ((Users)Session["User"]).UserName;
                        item.QC_ComfirmDate = DateTime.Now;
                        db.SaveChangesAsync();
                    }

                }
            }
        }

        //外箱库位号修改箱体显示
        public ActionResult DisplayWarehouseEdit(string ordernum)
        {
            JObject modeuleJobject = new JObject();
            JObject order = new JObject();
            var boxNum = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.IsOut == false).Select(c => c.OuterBoxBarcode).Distinct().ToList();
            int i = 0;
            foreach (var boxbarcode in boxNum)
            {
                var moduleNumList = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.OuterBoxBarcode == boxbarcode).ToList();
                moduleNumList.ForEach(c => c.ModuleGroupNum = c.ModuleGroupNum == null ? c.BarCodeNum : c.ModuleGroupNum);
                var list = moduleNumList.Select(c => c.ModuleGroupNum);
                var warehounum = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.OuterBoxBarcode == boxbarcode).Select(c => c.WarehouseNum).FirstOrDefault();
                modeuleJobject.Add("module", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(list)));
                modeuleJobject.Add("warehounum", warehounum);
                modeuleJobject.Add("outherNum", boxbarcode);
                modeuleJobject.Add("status", false);
                order.Add(i.ToString(), modeuleJobject);
                modeuleJobject = new JObject();
                i++;
            }
            return Content(JsonConvert.SerializeObject(order));
        }
        //外箱库位号修改确认箱体号，根据条码搜索
        public ActionResult DisplayWarehouseEditFromBarcode(List<string> barcodeList)
        {
            JObject modeuleJobject = new JObject();
            JObject order = new JObject();
            var boxNum = new List<string>();
            boxNum = db.Warehouse_Join.Where(c => barcodeList.Contains(c.OuterBoxBarcode) && c.IsOut == false).Select(c => c.OuterBoxBarcode).Distinct().ToList();
            if (boxNum.Count() == 0)
            {
                boxNum = db.Warehouse_Join.Where(c => barcodeList.Contains(c.BarCodeNum) && c.IsOut == false).Select(c => c.OuterBoxBarcode).Distinct().ToList();
            }
            int i = 0;
            foreach (var boxbarcode in boxNum)
            {
                var moduleNumList = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == boxbarcode).ToList();
                moduleNumList.ForEach(c => c.ModuleGroupNum = c.ModuleGroupNum == null ? c.BarCodeNum : c.ModuleGroupNum);
                var list = moduleNumList.Select(c => c.ModuleGroupNum);
                var warehounum = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == boxbarcode).Select(c => c.WarehouseNum).FirstOrDefault();
                modeuleJobject.Add("module", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(list)));
                modeuleJobject.Add("warehounum", warehounum);
                modeuleJobject.Add("outherNum", boxbarcode);
                modeuleJobject.Add("status", false);
                order.Add(i.ToString(), modeuleJobject);
                modeuleJobject = new JObject();
                i++; ;
            }
            return Content(JsonConvert.SerializeObject(order));
        }
        [HttpPost]
        //修改外箱库位号
        public void StockNumEdit(string ordernum, List<string> outherboxbarcode, string warehouNum)
        {
            foreach (var barcode in outherboxbarcode)
            {
                var list = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.OuterBoxBarcode == barcode&&c.IsOut==false).ToList();
                if (list.Count != 0)
                {
                    foreach (var item in list)
                    {
                        item.WarehouseNum = warehouNum;
                        db.SaveChangesAsync();
                    }

                }
            }
        }
        #endregion

        //出库信息记录
        public ActionResult WarehouseOut(List<string> barcode,string warehousordernum)
        {
            JArray total = new JArray();
            List<string> ordernumList = new List<string>();
            foreach (var item in barcode)
            {
                var info = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == item&&c.IsOut==false).ToList();
                var ordernum = info.FirstOrDefault().OrderNum;
                if (!ordernumList.Contains(ordernum))
                {
                    ordernumList.Add(ordernum);
                }
                var num = 0;
                var list= db.Warehouse_Join.Where(c => c.OuterBoxBarcode == item && c.IsOut == true).ToList();
                if (list.Count == 0)
                { num = 1; }
                else
                {
                    num = list.Max(c => c.WarehouseOutNum)+1;
                }
                foreach (var warehouse_Join in info)
                {
                    
                    warehouse_Join.IsOut = true;
                    warehouse_Join.WarehouseOutDate = DateTime.Now;
                    warehouse_Join.WarehouseOutOperator = "张丹媛";
                    warehouse_Join.WarehouseOutNum = num;
                    warehouse_Join.NewBarcode = warehousordernum;
                    db.Entry(warehouse_Join).State = EntityState.Modified;
                    var print = db.Packing_BarCodePrinting.Where(c => c.BarCodeNum == warehouse_Join.BarCodeNum && c.OuterBoxBarcode == warehouse_Join.OuterBoxBarcode).FirstOrDefault();
                    db.Packing_BarCodePrinting.Remove(print);
                    var innercheck = db.Packing_InnerCheck.Where(c => c.Barcode == warehouse_Join.BarCodeNum).FirstOrDefault();
                    if (innercheck != null)
                        db.Packing_InnerCheck.Remove(innercheck);
                    db.SaveChanges();
                }
            }

            foreach (var item in ordernumList)
            {
                JObject orderjobject = new JObject();
                orderjobject.Add("orderNum", item);
                var maxnum = db.Warehouse_Join.Where(c => c.OrderNum == item).Max(c => c.WarehouseNum);
                var warehouse = db.Warehouse_Join.Where(c => c.OrderNum == item && c.WarehouseNum == maxnum&&c.IsOut==true).Select(c=>c.OuterBoxBarcode).Distinct().ToList().Count;

                //计划包装数量
                var planPrint = db.Packing_BasicInfo.Where(c => c.OrderNum == item).Sum(c => c.Quantity);
                if (warehouse == planPrint)
                {
                    orderjobject.Add("barcode", "");
                    orderjobject.Add("warehousOutCount", "");
                    orderjobject.Add("warehousCount","" );
                    orderjobject.Add("notPrintCount", "");
                    orderjobject.Add("message", "订单已全部出库，包装记录清零");
                    total.Add(orderjobject);
                    continue;
                }

                var outherBacode = db.Warehouse_Join.Where(c => c.OrderNum == item && barcode.Contains(c.OuterBoxBarcode)).Select(c => c.OuterBoxBarcode).Distinct().ToList();
                JArray barcodes = new JArray();
                barcodes.Add(outherBacode);
                orderjobject.Add("barcode", barcodes);
                //已包装数量
                var printCount = db.Packing_BarCodePrinting.Where(c => c.OrderNum == item).Select(c => c.OuterBoxBarcode).Distinct().Count();
                //出库数量
                var warhhousrout = db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();
                //入库数量
                var warhhousr = db.Warehouse_Join.Where(c => c.OrderNum == item).Select(c => c.OuterBoxBarcode).Distinct().Count();

                //已出库
                orderjobject.Add("warehousOutCount", warhhousrout);
                //剩下未出库数量
                orderjobject.Add("warehousCount", warhhousr - warhhousrout);
                //未包装数量
                orderjobject.Add("notPrintCount", planPrint - printCount);
                orderjobject.Add("message", "");
                total.Add(orderjobject);
            }
            return Content(JsonConvert.SerializeObject(total));
        }

        #region 产值操作

        //创建产值
        public string Procudtion_valueCrete(Production_Value value)
        {
            if (ModelState.IsValid)
            {

                var exit = db.Production_Value.Where(c => c.OrderNum == value.OrderNum).Select(c => c.OrderNum).FirstOrDefault();
                if (!string.IsNullOrEmpty(exit))
                {
                    return exit + "订单已有产值记录，请确认订单";
                }
                value.CreateDate = DateTime.Now;
                db.Production_Value.Add(value);
                db.SaveChanges();
                return "true";
            }
            return "false";
        }
        //历史记录查询
        public ActionResult HistoryProduction_value()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            CommonalityController comm = new CommonalityController();
            JObject value = new JObject();
            JObject total = new JObject();
            var yesterday = DateTime.Now.AddDays(-1);
            var productionOrder = db.Packing_BasicInfo.Select(c => c.OrderNum).Distinct().ToList();
            int i = 0;
            foreach (var item in productionOrder)
            {
                var productionvalue = db.Production_Value.Where(c => c.OrderNum == item).FirstOrDefault();

                //订单号
                value.Add("OrderNum", item);

                //模组数量
                //var basicinfo = db.Packing_BasicInfo.Where(c => c.OrderNum == item).ToList();
                //int moduleCount = 0;
                //foreach (var basic in basicinfo)
                //{
                //    moduleCount = moduleCount + (basic.OuterBoxCapacity * basic.Quantity);
                //}
                var moduleCount = db.OrderMgm.Where(c => c.OrderNum == item).Select(c => c.Boxes).FirstOrDefault();
                value.Add("moduleCount", moduleCount);
                //包装件数
                var quantity = db.Packing_BasicInfo.Where(c => c.OrderNum == item).Sum(c => c.Quantity);
                //已包装数量
                var packingCount = db.Packing_BarCodePrinting.Count(c => c.OrderNum == item).ToString() + "(" + db.Packing_BarCodePrinting.Where(c => c.OrderNum == item).Select(c => c.OuterBoxBarcode).Distinct().Count() + "/" + quantity.ToString() + ")";
                value.Add("packingCount", packingCount);
                //已入库数量
                var warehousJoincount = db.Warehouse_Join.Count(c => c.OrderNum == item);
                var warehousJoinCount = warehousJoincount.ToString() + "(" + db.Warehouse_Join.Where(c => c.OrderNum == item && c.Date != null).Select(c => c.OuterBoxBarcode).Distinct().Count() + "/" + quantity.ToString() + ")";
                value.Add("warehousJoinCount", warehousJoinCount);
                //已出库数量
                var warehousOutcount = db.Warehouse_Join.Count(c => c.OrderNum == item && c.IsOut == true);
                var warehousOutCount = warehousOutcount.ToString() + "(" + db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count() + "/" + quantity.ToString() + ")";
                value.Add("warehousOutCount", warehousOutCount);
                //库存数量
                var outheCount = db.Warehouse_Join.Where(c => c.OrderNum == item && c.Date != null).Select(c => c.OuterBoxBarcode).Distinct().Count() - db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();
                var stockCount = (warehousJoincount - warehousOutcount).ToString() + "(" + outheCount + ")";
                value.Add("stockCount", stockCount);

                //挪用信息
                JArray nuoInfo = new JArray();
                var warehouse = db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true && c.NewBarcode != null).Select(c=>c.WarehouseOutNum).Distinct().ToList();
                foreach (var num in warehouse)
                {
                    var ordernum = db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true && c.NewBarcode != null && c.WarehouseOutNum == num).Select(c => c.NewBarcode).FirstOrDefault();
                    nuoInfo.Add("第"+num+"次出库到"+ordernum+"订单");
                }
                //var old = db.BarCodeRelation.Where(c => c.OldOrderNum == item).ToList();
                //if (old.Count() != 0)
                //{
                //    var oldselectnew = old.Select(c => c.NewOrderNum).Distinct();
                //    foreach (var newitem in oldselectnew)
                //    {
                //        var count = old.Count(c => c.NewOrderNum == newitem);
                //        nuoInfo.Add("挪到订单" + newitem + "(" + count + "条)");
                //    }
                //}
                //var newbarcode = db.BarCodeRelation.Where(c => c.NewOrderNum == item).ToList();
                //if (newbarcode.Count() != 0)
                //{
                //    var newselectold = newbarcode.Select(c => c.OldOrderNum).Distinct();
                //    foreach (var olditem in newselectold)
                //    {
                //        var count = newbarcode.Count(c => c.OldOrderNum == olditem);
                //        nuoInfo.Add("挪用订单" + olditem + "(" + count + "条)");
                //    }
                //}
                value.Add("nuoInfo", nuoInfo);
                //入库完成率
                var warehousJoinComplete = (moduleCount == 0 ? 0 : (decimal)(warehousJoincount * 100) / moduleCount).ToString("F2") + "%";
                value.Add("warehousJoinComplete", warehousJoinComplete);
                //出库完成率
                var warehousOutComplete = (moduleCount == 0 ? 0 : (decimal)(warehousOutcount * 100) / moduleCount).ToString("F2") + "%";
                value.Add("warehousOutComplete", warehousOutComplete);

                if (productionvalue == null)
                {
                    value.Add("id", 0);
                    //总产值
                    value.Add("Worth", "- -");
                    //目前入库产值
                    value.Add("warehouseJoinValue", "- -");
                    //未完成产值
                    value.Add("uncompleteValue", "- -");
                    //备注
                    value.Add("remark", "");
                }
                else
                {
                    value.Add("id", productionvalue.Id);
                    //总产值
                    value.Add("Worth", productionvalue.Worth);
                    //目前入库产值
                    var warehouseJoinValue = warehousJoincount * (moduleCount == 0 ? 0 : productionvalue.Worth / moduleCount);
                    value.Add("warehouseJoinValue", warehouseJoinValue.ToString("F2"));
                    //未完成产值
                    var uncompleteValue = productionvalue.Worth - warehouseJoinValue;
                    value.Add("uncompleteValue", uncompleteValue.ToString("F2"));
                    //备注
                    value.Add("remark", productionvalue.Remark);
                }

                total.Add(i.ToString(), value);
                i++;
                value = new JObject();
            }
            return Content(JsonConvert.SerializeObject(total));
        }

        //历史查询也查询
        public ActionResult CheckDateProduction_value(DateTime? day1, DateTime? day2, int? year, int? mouth)
        {
            JObject value = new JObject();
            JObject total = new JObject();
            var yesterday = DateTime.Now.AddDays(-1);
            var productionOrder = db.Warehouse_Join.ToList();
            if (day1 != null & day2 != null)
            {
                if (day1 < day2)
                    productionOrder = productionOrder.Where(c => c.Date > day1 && c.Date < day2).ToList();
                else
                    productionOrder = productionOrder.Where(c => c.Date > day2 && c.Date < day1).ToList();

            }
            if (year != null)
            {
                productionOrder = productionOrder.Where(c => c.Date.Value.Year == year).ToList();
            }
            if (mouth != null)
            {
                productionOrder = productionOrder.Where(c => c.Date.Value.Month == mouth).ToList();
            }
            var odernumlist = productionOrder.Select(c => c.OrderNum).Distinct().ToList();
            int i = 0;
            foreach (var item in odernumlist)
            {
                var productionvalue = db.Production_Value.Where(c => c.OrderNum == item).FirstOrDefault();

                //订单号
                value.Add("OrderNum", item);

                //模组数量
                //var basicinfo = db.Packing_BasicInfo.Where(c => c.OrderNum == item).ToList();
                //int moduleCount = 0;
                //foreach (var basic in basicinfo)
                //{
                //    moduleCount = moduleCount + (basic.OuterBoxCapacity * basic.Quantity);
                //}
                var moduleCount = db.OrderMgm.Where(c => c.OrderNum == item).Select(c => c.Boxes).FirstOrDefault();
                value.Add("moduleCount", moduleCount);
                //包装件数
                var quantity = db.Packing_BasicInfo.Where(c => c.OrderNum == item).Sum(c => c.Quantity);
                //已包装数量
                var packingCount = db.Packing_BarCodePrinting.Count(c => c.OrderNum == item).ToString() + "(" + db.Packing_BarCodePrinting.Where(c => c.OrderNum == item).Select(c => c.OuterBoxBarcode).Distinct().Count() + "/" + quantity.ToString() + ")";
                value.Add("packingCount", packingCount);
                //已入库数量
                var warehousJoincount = db.Warehouse_Join.Count(c => c.OrderNum == item);
                var warehousJoinCount = warehousJoincount.ToString() + "(" + db.Warehouse_Join.Where(c => c.OrderNum == item && c.Date != null).Select(c => c.OuterBoxBarcode).Distinct().Count() + "/" + quantity.ToString() + ")";
                value.Add("warehousJoinCount", warehousJoinCount);
                //已出库数量
                var warehousOutcount = db.Warehouse_Join.Count(c => c.OrderNum == item && c.IsOut == true);
                var warehousOutCount = warehousOutcount.ToString() + "(" + db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count() + "/" + quantity.ToString() + ")";
                value.Add("warehousOutCount", warehousOutCount);
                //库存数量
                var outheCount = db.Warehouse_Join.Where(c => c.OrderNum == item && c.Date != null).Select(c => c.OuterBoxBarcode).Distinct().Count() - db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();
                var stockCount = (warehousJoincount - warehousOutcount).ToString() + "(" + outheCount + ")";
                value.Add("stockCount", stockCount);

                //挪用信息
                JArray nuoInfo = new JArray();
                var old = db.BarCodeRelation.Where(c => c.OldOrderNum == item).ToList();
                if (old.Count() != 0)
                {
                    var oldselectnew = old.Select(c => c.NewOrderNum).Distinct();
                    foreach (var newitem in oldselectnew)
                    {
                        var count = old.Count(c => c.NewOrderNum == newitem);
                        nuoInfo.Add("挪到订单" + newitem + "(" + count + "条)");
                    }
                }
                var newbarcode = db.BarCodeRelation.Where(c => c.NewOrderNum == item).ToList();
                if (newbarcode.Count() != 0)
                {
                    var newselectold = newbarcode.Select(c => c.OldOrderNum).Distinct();
                    foreach (var olditem in newselectold)
                    {
                        var count = newbarcode.Count(c => c.OldOrderNum == olditem);
                        nuoInfo.Add("挪用订单" + olditem + "(" + count + "条)");
                    }
                }
                value.Add("nuoInfo", nuoInfo);

                //入库完成率
                var warehousJoinComplete = (moduleCount == 0 ? 0 : (decimal)(warehousJoincount * 100) / moduleCount).ToString("F2") + "%";
                value.Add("warehousJoinComplete", warehousJoinComplete);
                //出库完成率
                var warehousOutComplete = (moduleCount == 0 ? 0 : (decimal)(warehousOutcount * 100) / moduleCount).ToString("F2") + "%";
                value.Add("warehousOutComplete", warehousOutComplete);
                if (productionvalue == null)
                {
                    value.Add("id", 0);
                    //总产值
                    value.Add("Worth", "- -");
                    //目前入库产值
                    value.Add("warehouseJoinValue", "- -");
                    //未完成产值
                    value.Add("uncompleteValue", "- -");
                    //备注
                    value.Add("remark", "");
                }
                else
                {
                    value.Add("id", productionvalue.Id);
                    //总产值
                    value.Add("Worth", productionvalue.Worth);
                    //目前入库产值
                    var warehouseJoinValue = warehousJoincount * (moduleCount == 0 ? 0 : productionvalue.Worth / moduleCount);
                    value.Add("warehouseJoinValue", warehouseJoinValue.ToString("F2"));
                    //未完成产值
                    var uncompleteValue = productionvalue.Worth - warehouseJoinValue;
                    value.Add("uncompleteValue", uncompleteValue.ToString("F2"));
                    //备注
                    value.Add("remark", productionvalue.Remark);
                }

                total.Add(i.ToString(), value);
                i++;
                value = new JObject();
            }
            return Content(JsonConvert.SerializeObject(total));
        }

        //修改产值记录
        public string UpdateProductionValue(Production_Value value)
        {
            if (ModelState.IsValid)
            {
                db.Entry(value).State = EntityState.Modified;
                db.SaveChanges();
                return "true";
            }

            else return "false";
        }

        //删除产值记录
        public string DeleteProduction(Production_Value value)
        {
            if (ModelState.IsValid)
            {
                db.Production_Value.Remove(value);
                db.SaveChanges();
                return "true";
            }
            else return "false";
        }
        //页面
        public ActionResult ViewDisplay(string ordernum)
        {
            ViewBag.ordernum = ordernum;
            return View();
        }
        //显示出入库信息
        public ActionResult DisplayWarehouse(string ordernum)
        {
            ViewBag.outherbarcode = GetOutherBarcode(ordernum);
            JObject joinjobject = new JObject();
            JObject total = new JObject();
            var warehous_join = db.Warehouse_Join.Where(c => c.OrderNum == ordernum).ToList();
            var barcode_join = warehous_join.Select(c => c.OuterBoxBarcode).Distinct().ToList();
            int i = 0;
            foreach (var join in barcode_join)
            {
                joinjobject.Add("otherBarcode", join);
                var status = warehous_join.Where(c => c.OuterBoxBarcode == join).Select(c => c.IsOut).First();
                joinjobject.Add("status", status == true ? "出库" : "入库");
                var jointime = warehous_join.Where(c => c.OuterBoxBarcode == join).Select(c => c.Date).FirstOrDefault();
                var outtime = warehous_join.Where(c => c.OuterBoxBarcode == join).Select(c => c.WarehouseOutDate).FirstOrDefault();
                joinjobject.Add("time", status == true ? outtime : jointime);
                var modulebarcode = warehous_join.Where(c => c.OuterBoxBarcode == join);
                var warehousenum = warehous_join.Where(c => c.OuterBoxBarcode == join).Select(c => c.WarehouseNum).FirstOrDefault();
                joinjobject.Add("warehousenum", warehousenum);
                JArray barcode = new JArray();
                foreach (var item in modulebarcode)
                {
                    barcode.Add(item.BarCodeNum + ":" + item.ModuleGroupNum);
                }
                joinjobject.Add("codeList", barcode);

                total.Add(i.ToString(), joinjobject);
                joinjobject = new JObject();
                i++;
            }
            return Content(JsonConvert.SerializeObject(total));
        }

        //点击订单号显示具体信息的头
        public ActionResult DisplayTop(string ordernum)
        {
            JObject value = new JObject();
            var productionvalue = db.Production_Value.Where(c => c.OrderNum == ordernum).FirstOrDefault();

            //订单号
            value.Add("OrderNum", ordernum);

            //模组数量
            //var basicinfo = db.Packing_BasicInfo.Where(c => c.OrderNum == item).ToList();
            //int moduleCount = 0;
            //foreach (var basic in basicinfo)
            //{
            //    moduleCount = moduleCount + (basic.OuterBoxCapacity * basic.Quantity);
            //}
            var moduleCount = db.OrderMgm.Where(c => c.OrderNum == ordernum).Select(c => c.Boxes).FirstOrDefault();
            value.Add("moduleCount", moduleCount);
            //包装件数
            var quantity = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Sum(c => c.Quantity);
            //已包装数量
            var packingCount = db.Packing_BarCodePrinting.Count(c => c.OrderNum == ordernum).ToString() + "(" + db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum).Select(c => c.OuterBoxBarcode).Distinct().Count() + "/" + quantity.ToString() + ")";
            value.Add("packingCount", packingCount);
            //已入库数量
            var warehousJoincount = db.Warehouse_Join.Count(c => c.OrderNum == ordernum);
            var warehousJoinCount = warehousJoincount.ToString() + "(" + db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.Date != null).Select(c => c.OuterBoxBarcode).Distinct().Count() + "/" + quantity.ToString() + ")";
            value.Add("warehousJoinCount", warehousJoinCount);
            //已出库数量
            var warehousOutcount = db.Warehouse_Join.Count(c => c.OrderNum == ordernum && c.IsOut == true);
            var warehousOutCount = warehousOutcount.ToString() + "(" + db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count() + "/" + quantity.ToString() + ")";
            value.Add("warehousOutCount", warehousOutCount);

            //入库完成率
            var warehousJoinComplete = (moduleCount == 0 ? 0 : (decimal)(warehousJoincount * 100) / moduleCount).ToString("F2") + "%";
            value.Add("warehousJoinComplete", warehousJoinComplete);
            //出库完成率
            var warehousOutComplete = (moduleCount == 0 ? 0 : (decimal)(warehousOutcount * 100) / moduleCount).ToString("F2") + "%";
            value.Add("warehousOutComplete", warehousOutComplete);
            if (productionvalue == null)
            {
                value.Add("id", 0);
                //总产值
                value.Add("Worth", "- -");
                //目前入库产值
                value.Add("warehouseJoinValue", "- -");
                //未完成产值
                value.Add("uncompleteValue", "- -");
                //备注
                value.Add("remark", "");
            }
            else
            {
                value.Add("id", productionvalue.Id);
                //总产值
                value.Add("Worth", productionvalue.Worth);
                //目前入库产值
                var warehouseJoinValue = warehousJoincount * (moduleCount == 0 ? 0 : productionvalue.Worth / moduleCount);
                value.Add("warehouseJoinValue", warehouseJoinValue.ToString("F2"));
                //未完成产值
                var uncompleteValue = productionvalue.Worth - warehouseJoinValue;
                value.Add("uncompleteValue", uncompleteValue.ToString("F2"));
                //备注
                value.Add("remark", productionvalue.Remark);
            }

            return Content(JsonConvert.SerializeObject(value));
        }

        //根据模组号或者外箱条码筛选
        public ActionResult Displaybarcode(string ordernum, string outherBarcode, string moduleBarbode)
        {
            ViewBag.outherbarcode = GetOutherBarcode(ordernum);
            JObject outher = new JObject();
            JObject total = new JObject();
            var info = db.Warehouse_Join.Where(c => c.OrderNum == ordernum).ToList();
            if (info != null)
            {
                if (!string.IsNullOrEmpty(outherBarcode))
                {
                    info = info.Where(c => c.OuterBoxBarcode == outherBarcode).ToList();
                }
                if (!string.IsNullOrEmpty(moduleBarbode))
                {
                    info = info.Where(c => c.BarCodeNum == moduleBarbode).ToList();
                }
                var outherList = info.Select(c => c.OuterBoxBarcode).Distinct().ToList();
                int i = 0;
                foreach (var item in outherList)
                {
                    outher.Add("otherBarcode", item);
                    var status = info.Where(c => c.OuterBoxBarcode == item).Select(c => c.IsOut).First();
                    outher.Add("status", status == true ? "出库" : "入库");
                    var barcodelist = info.Where(c => c.OuterBoxBarcode == item).Select(c => c.BarCodeNum).ToList();
                    outher.Add("codeList", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(barcodelist)));

                    total.Add(i.ToString(), outher);
                    outher = new JObject();
                    i++;
                }
                return Content(JsonConvert.SerializeObject(total));
            }
            return Content("");
        }

        //获取折线数据
        public ActionResult BrokenlineMessage(int year)
        {
            JObject message = new JObject();
            var num = db.Warehouse_Join.Where(c => c.Date.Value.Year == year).ToList();

            for (int i = 1; i < 13; i++)
            {
                decimal sum = 0;
                var month = num.Where(c => c.Date.Value.Month == i).Select(c => c.OrderNum).Distinct().ToList();
                foreach (var item in month)
                {
                    var productionvalue = db.Production_Value.Where(c => c.OrderNum == item).FirstOrDefault();
                    if (productionvalue != null)
                    {
                        var warehousJoinCount = db.Warehouse_Join.Count(c => c.OrderNum == item);
                        var moduleCount = db.OrderMgm.Where(c => c.OrderNum == item).Select(c => c.Boxes).FirstOrDefault();
                        var warehouseJoinValue = warehousJoinCount * (moduleCount == 0 ? 0 : productionvalue.Worth / moduleCount);
                        sum = sum + warehouseJoinValue;
                    }

                }
                message.Add(i.ToString(), sum.ToString("F2"));
            }
            return Content(JsonConvert.SerializeObject(message));
        }

        #endregion

        #region 列表获取
        //录入包装基本信息的订单列表
        public ActionResult GetOrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum).ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //获得录入基本包装信息的订单列表
        private ActionResult GetPackagingOrderList()
        {
            var orders = db.Packing_BasicInfo.OrderByDescending(m => m.Id).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //获得打印条码的订单列表
        private ActionResult GetPrintOrderList()
        {
            var orders = db.Packing_BarCodePrinting.Where(c => c.QC_Operator != null).OrderByDescending(m => m.Id).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //获得包装类型
        public ActionResult GetBoxType(string ordernum)
        {
            var orders = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Select(m => m.Type).Distinct();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        public ActionResult GetScreenNum(string ordernum, string type)
        {
            JObject message = new JObject();
            var count = db.Packing_BasicInfo.Count(c => c.OrderNum == ordernum && c.IsSeparate == true);
            if (count > 0)
                message.Add("IsScreen", true);
            else
                message.Add("IsScreen", false);

            var orders = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == type).Select(m => m.ScreenNum).Distinct();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            message.Add("List", result);
            return Content(JsonConvert.SerializeObject(message));
        }

        //获得仓库外箱条码列表
        public ActionResult GetOutherBarcode(string ordernum)
        {
            var orders = db.Warehouse_Join.Where(c => c.OrderNum == ordernum).Select(m => m.OuterBoxBarcode).Distinct();    //增加.Distinct()后会重新按OrderNum升序排序
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

        #region 模组规则


        public ActionResult RuleEnter()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "RuleEnter" });
            }
            return View();
        }

        //订单输入规则前判断
        public ActionResult CheckRule(string ordenum)
        {
            JObject result = new JObject();
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json") == true)
            {
                if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\delete.json") == true)
                {
                    var deletejson = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json");
                    var jarray = JsonConvert.DeserializeObject<JArray>(deletejson).ToList();
                    if (jarray.Contains(ordenum))
                    {
                        result.Add("candelete", false);
                        result.Add("canAdd", false);
                        result.Add("mesage", "此订单需等外观OQC确认删除，如有疑问请联系OQC人员");
                        return Content(JsonConvert.SerializeObject(result));
                    }
                }
                var manualjson = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json");
                var manualjarray = JsonConvert.DeserializeObject<JObject>(manualjson);
                if (manualjarray.Property("isManual")!=null)
                {
                    result.Add("candelete", true);
                    result.Add("canAdd", false);
                    result.Add("mesage", "此订单已有手工录入规则，如需重新录入，请先删除规则!");
                    return Content(JsonConvert.SerializeObject(result));
                }
                result.Add("candelete", true);
                result.Add("canAdd", false);
                result.Add("mesage", "此订单已有模组规则，如需重新录入，请先删除规则!");
                return Content(JsonConvert.SerializeObject(result));

            }
            var appearance = db.Appearance.Count(c => c.OrderNum == ordenum && c.OQCCheckFT != null);
            if (appearance > 0)
            {
                result.Add("candelete", false);
                result.Add("canAdd", false);
                result.Add("mesage", "此订单已有外观电检记录!");
                return Content(JsonConvert.SerializeObject(result));
            }

            var cab = db.CalibrationRecord.Where(c => c.OrderNum == ordenum).Select(c => c.ModuleGroupNum).ToList();
            if (cab.Count(c => c != null) > 0)
            {
                result.Add("candelete", false);
                result.Add("canAdd", false);
                result.Add("mesage", "此订单已有校正模组号!");
                return Content(JsonConvert.SerializeObject(result));
            }
            result.Add("candelete", false);
            result.Add("canAdd", true);
            result.Add("mesage", "");
            return Content(JsonConvert.SerializeObject(result));
        }

        //没有模组条码规则输入
        public void SetJsonFile(List<Sequence> sequences, string ordenum,bool isManual)
        {
            JArray number = new JArray();
            JObject normal = new JObject();
            if (Directory.Exists(@"D:\MES_Data\TemDate\OrderSequence") == false)//如果不存在就创建订单文件夹
            {
                Directory.CreateDirectory(@"D:\MES_Data\TemDate\OrderSequence");
            }
            if (isManual)
            {
                normal.Add("isManual","手工录入");
                string output1 = Newtonsoft.Json.JsonConvert.SerializeObject(normal, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json", output1);
            }
            else
            {
                foreach (var item in sequences)
                {
                    for (int i = 0; i < item.Num; i++)
                    {
                        var serial = "";
                        if (item.Rule)
                        {
                            var num = item.Num + item.startNum - 1;
                            serial = (item.startNum + i).ToString().PadLeft(num.ToString().Length, '0');
                        }
                        else
                        {
                            serial = (item.startNum + i).ToString(); ;
                        }
                        string seq = item.Prefix + serial + item.Suffix;
                        number.Add(seq);
                    }
                }
                normal.Add("Normal", number);

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(normal, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json", output);
            }
        }

        //选出特殊的模组号
        public bool SetSpecialNum(string ordenum, List<string> num)
        {

            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json") == true)
            {
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json");
                var json = JsonConvert.DeserializeObject<JObject>(jsonstring);


                if (json.Property("Special") != null)
                {
                    ////List<JToken> spe = new List<JToken>();
                    List<JToken> normaladd = new List<JToken>();
                    List<JToken> normaldelete = new List<JToken>();
                    var special = (JArray)json["Special"];
                    var normal = (JArray)json["Normal"];

                    foreach (var item in special)
                    {
                        if (num == null)
                        {
                            normaladd.Add(item);
                        }
                       else if (!num.Contains(item.ToString()))
                            normaladd.Add(item);
                    }
                    foreach (var item in normal)
                    {
                        if (num == null)
                        {
                            continue;
                        }
                        if (num.Contains(item.ToString()))
                        {
                            //spe.Add(item);
                            normaldelete.Add(item);
                        }
                    }

                    normaladd.ForEach(c => c.Remove());
                    normal.Add(normaladd);
                    normaldelete.ForEach(c => c.Remove());
                    special.Add(normaldelete);

                    json["Special"] = special;
                    json["Normal"] = normal;

                }
                else
                {
                    var jarray = json["Normal"].ToList();
                    JArray sepcial = new JArray();
                    List<JToken> delete = new List<JToken>();

                    foreach (var item in jarray)
                    {
                        if (num.Contains(item.ToString()))
                        {
                            sepcial.Add(item);
                            delete.Add(item);
                        }
                    }

                    delete.ForEach(c => c.Remove());
                    json.Add("Special", sepcial);

                }
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json", output);
                return true;
            }
            return false;
        }

        //查询订单的模组号规则
        public ActionResult SelectModule(string ordenum,bool reverse=true)
        {
            JObject result = new JObject();
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json") == true)
            {
                if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\delete.json") == true)
                {
                    var deletejson = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json");
                    var jarray1 = JsonConvert.DeserializeObject<JArray>(deletejson).ToList();
                    if (jarray1.Contains(ordenum))
                    {
                        result.Add("isManual", false);
                        result.Add("mudule", "");
                        return Content(JsonConvert.SerializeObject(result));
                    }
                }
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json");
                var json = JsonConvert.DeserializeObject<JObject>(jsonstring);
                if (json.Property("isManual") != null)
                {
                    result.Add("isManual", true);
                    result.Add("mudule","");
                    return Content(JsonConvert.SerializeObject(result));
                }
                var jarray = json["Normal"];
                JToken mudule = null;
                if (reverse)
                {
                    int index = jarray.Count();
                    mudule = jarray[index-1];
                }
                else
                {
                    mudule = jarray[0];
                }
                result.Add("isManual", false);
                result.Add("mudule", mudule.ToString());
                return Content(JsonConvert.SerializeObject(result));

            }
            result.Add("isManual", false);
            result.Add("mudule", "");
            return Content(JsonConvert.SerializeObject(result));
        }

        //使用了模组号
        public bool DelteMudole(string ordenum, string num)
        {
            if (Session["User"] == null)//为了跳到外观完成的方法，外观完成能返回登陆页面
            {
                return true;
            }
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json") == true)
            {
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json");
                var json = JsonConvert.DeserializeObject<JObject>(jsonstring);
                var normal = json["Normal"].ToList();
                if (normal.Contains(num))
                {
                    var index = normal.IndexOf(num);
                    normal[index].Remove();
                    string output2 = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json", output2);
                    return true;
                }
                else
                {
                    if (json.Property("Special") != null)
                    {
                        var special = json["Special"].ToList();
                        if (special.Contains(num))
                        {
                            var index = special.IndexOf(num);
                            special[index].Remove();
                            string output2 = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
                            System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json", output2);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
            }
            return false;
        }

        //根据订单号，显示已使用和未使用的模组号
        public ActionResult SetModule(string ordenum)
        {
            JObject info = new JObject();
            JArray notuse = new JArray();
            JArray use = new JArray();
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json") == true)
            {

                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json");
                var json = JsonConvert.DeserializeObject<JObject>(jsonstring);
                var jarray = json["Normal"].ToList();
                notuse.Add(jarray);
                if (json.Property("Special") != null)
                {
                    var jarray1 = json["Special"].ToList();
                    notuse.Add(jarray1);
                }
                info.Add("notuse", notuse);

                //已使用

                var moduleList = db.Appearance.Where(c => c.OrderNum == ordenum).Select(c => c.ModuleGroupNum).Distinct().ToList();
                use.Add(moduleList);
                info.Add("use", use);
                return Content(JsonConvert.SerializeObject(info));
            }
            else
            {
                var cab = db.CalibrationRecord.Where(c => c.OrderNum == ordenum).Select(c => c.ModuleGroupNum).Distinct().ToList();
                var moduleList = db.Appearance.Where(c => c.OrderNum == ordenum).Select(c => c.ModuleGroupNum).Distinct().ToList();
                if (cab == null)
                {
                    return null;
                }
                var except = cab.Except(moduleList).ToList();
                notuse.Add(except);
                info.Add("notuse", notuse);

                use.Add(moduleList);
                info.Add("use", use);
                return Content(JsonConvert.SerializeObject(info));
            }

        }

        //删除规则前提示
        public string Tips(string ordernum)
        {
            string mesage = "";
            //入库记录
            var warehouecount = db.Warehouse_Join.Count(c => c.OrderNum == ordernum);
            if (warehouecount > 0)
            { mesage = mesage + "入库有" + warehouecount + "条记录，"; }

            //外箱标签
            var pritxount = db.Packing_BarCodePrinting.Count(c => c.OrderNum == ordernum);
            if (pritxount > 0)
            { mesage = mesage + "外箱标签有" + pritxount + "条记录，"; }

            //内箱记录
            var innercount = db.Packing_InnerCheck.Count(c => c.OrderNum == ordernum);
            if (innercount > 0)
            { mesage = mesage + "内箱确认有" + innercount + "条记录，"; }

            //电检记录
            var appercount = db.Appearance.Count(c => c.OrderNum == ordernum && c.OQCCheckFT != null);
            if (appercount > 0)
            { mesage = mesage + "电检完成有" + appercount + "条记录，"; }

            if (string.IsNullOrEmpty(mesage))
            {
                mesage = "此订单没有使用模组号记录";
            }
            return mesage;
        }

        //删除规则
        public void DeleteMoudueRuleAsync(string ordernum)
        {
            //if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordernum + ".json") == true)
            //{
            //    var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordernum + ".json");
            //    var json = JsonConvert.DeserializeObject<JObject>(jsonstring);
            //    json.Add("delete", 1);
            //    string output = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
            //    System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordernum + ".json", output);

            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\delete.json") != true)
            {
                JArray deletevalue = new JArray();
                deletevalue.Add(ordernum);
                string deletestringFirst = Newtonsoft.Json.JsonConvert.SerializeObject(deletevalue, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json", deletestringFirst);
            }
            else
            {
                var deletejson = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json");
                var jarray = JsonConvert.DeserializeObject<JArray>(deletejson);
                jarray.Add(ordernum);
                string deletestring = Newtonsoft.Json.JsonConvert.SerializeObject(jarray, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json", deletestring);
            }

        }

        //查看模组号的使用情况
        public ActionResult DiaplayMuduleUserMessage()
        {
            DirectoryInfo directory = new DirectoryInfo(@"D:\MES_Data\TemDate\OrderSequence\");
            JArray total = new JArray();
            foreach (var item in directory.GetFileSystemInfos())
            {
                JObject file = new JObject();
                string name = System.IO.Path.GetFileNameWithoutExtension(item.FullName);
                if (name == "delete")
                { continue; }
                file.Add("ordernum", name);

                //已使用的模组号列表
                var appearances = db.Appearance.OrderBy(c => c.ModuleGroupNum).Where(c => c.OrderNum == name && c.ModuleGroupNum != null).Select(c => new { c.BarCodesNum, c.ModuleGroupNum }).ToList();
                JArray module = new JArray();
                foreach (var app in appearances)
                {
                    module.Add(app.BarCodesNum + "：" + app.ModuleGroupNum);
                }

                file.Add("user", module);

                //未使用
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + name + ".json");
                var json = JsonConvert.DeserializeObject<JObject>(jsonstring);
                JArray notuser = new JArray();
                if (json.Property("isManual") != null)
                {
                    notuser.Add("手工录入");
                }
                else
                {
                    var jarray = json["Normal"].ToList();
                    notuser.Add(jarray);
                    if (json.Property("Special") != null)
                    {
                        var jarray1 = json["Special"].ToList();
                        notuser.Add(jarray1);
                    }
                }
                file.Add("notuser", notuser);

                total.Add(file);
            }
            return Content(JsonConvert.SerializeObject(total));
        }
        #endregion

        public ActionResult inputPackaging()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "inputPackaging" });
            }
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            return View();
        }

        public ActionResult insidePrint()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "insidePrint" });
            }
            return View();
        }

        public ActionResult insideConfirm()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "insideConfirm" });
            }
            return View();
        }

        public ActionResult outsideBinningPrint()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "outsideBinningPrint" });
            }
            return View();
        }

        public ActionResult outsideBinning()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "outsideBinning" });
            }
            return View();
        }
        public ActionResult outsideConfirm()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "outsideConfirm" });
            }
            return View();
        }

        public ActionResult inStockConfirm()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "inStockConfirm" });
            }
            return View();
        }

        public ActionResult PingZhioutStockConfirm()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "PingZhioutStockConfirm" });
            }
            return View();
        }

        public ActionResult stockNumEdit()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "stockNumEdit" });
            }
            return View();
        }

        public ActionResult board()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "board" });
            }
            return View();
        }
        public ActionResult CreteBoard()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "CreteBoard" });
            }
            return View();
        }

        public ActionResult HistoryBoard()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "HistoryBoard" });
            }
            return View();
        }
        public ActionResult outStockConfirm()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "outStockConfirm" });
            }
            return View();
        }

        public ActionResult DeleteConfirm()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "DeleteConfirm" });
            }
            return View();
        }

        public ActionResult DeleteConfirmAll()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "DeleteConfirm" });
            }
            return View();
        }

        public ActionResult lookTag()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "lookTag" });
            }
            return View();
        }

        public ActionResult lookJson()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "lookJson" });
            }
            return View();
        }

        #region---包装打印标签
        //打印标签
        [HttpPost]
        public ActionResult OutsideBoxLablePrint(int screennum = 1, string ordernum = "", string packagingordernum = "", string outsidebarcode = "", string material_discription = "", int pagecount = 1, string sntn = "", string qty = "", bool logo = true, string ip = "", int port = 0, int concentration = 5, string[] mn_list = null)
        {
            //组织数据
            //var AllBitmap = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo);
            //MemoryStream ms = new MemoryStream();
            //AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //AllBitmap.Dispose();
            //return File(ms.ToArray(), "image/Png");
            if (!String.IsNullOrEmpty(packagingordernum)) ordernum = packagingordernum;//如果有包装新订单号，则使用包装新订单号。
            var bm = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum);
            string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
            int totalbytes = bm.ToString().Length;
            int rowbytes = 10;
            string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
            data += totalbytes + "," + rowbytes + "," + hex;
            data += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
            string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
            return Content(result);
        }
        #endregion

        #region---查看重打印外箱标签
        //根据订单输出外箱条码号清单
        [HttpPost]
        public async Task<ActionResult> OutputOutsideBoxBarCodeNumList(string ordernum)
        {
            var OutsideBoxBarCodeNumList = await db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum).Select(c => c.OuterBoxBarcode).Distinct().ToListAsync();
            return Content(JsonConvert.SerializeObject(OutsideBoxBarCodeNumList));
        }


        //根据订单输出可删除的外箱条码号清单
        [HttpPost]
        public async Task<ActionResult> OutputOutsideBoxBarCodeNumListCanDel(string ordernum)
        {
            var OutsideBoxBarCodePackagingNumList = await db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum).Select(c => c.OuterBoxBarcode).Distinct().ToListAsync();
            var OutsideBoxBarCodeWarehouseJoinNumList = await db.Warehouse_Join.Where(c => c.OrderNum == ordernum).Select(c => c.OuterBoxBarcode).ToListAsync();
            var OutsideBoxBarCodeNumList = OutsideBoxBarCodePackagingNumList.Except(OutsideBoxBarCodeWarehouseJoinNumList);
            return Content(JsonConvert.SerializeObject(OutsideBoxBarCodeNumList));
        }


        public ActionResult OutsideBoxLableLogoChange()
        {
            //if (Session["User"] == null)
            //{
            //    return RedirectToAction("Login", "Users", new { col = "Packagings", act = "OutsideBoxLableLogoChange" });
            //}
            return View();
        }

        [HttpPost]
        public ActionResult OutsideBoxLableLogoGet(string ordernum)
        {
            JObject result = new JObject();
            var ordernum_list = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum);
            var screen_list = ordernum_list.Select(c => c.ScreenNum).Distinct();
            foreach (var screen in screen_list)
            {
                var ordernum_list_screen_list = ordernum_list.Where(c => c.ScreenNum == screen);
                var outsideboxbarcodelist = ordernum_list_screen_list.Select(c => c.OuterBoxBarcode).Distinct();
                List<Packing_BarCodePrinting> record_screen = new List<Packing_BarCodePrinting>();
                foreach(var item in outsideboxbarcodelist)
                {
                    record_screen.Add(ordernum_list_screen_list.Where(c => c.OuterBoxBarcode == item).FirstOrDefault());
                }
                result.Add("屏序" + screen, JsonConvert.SerializeObject(record_screen));
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpPost]
        public ActionResult OutsideBoxLableLogoChange(string ordernum, string outerboxbarcode,bool logo)
        {
            var recordlist = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum && c.OuterBoxBarcode == outerboxbarcode);
            int count = 0;
            foreach(var record in recordlist)
            {
                record.IsLogo = logo;
            }
            count = db.SaveChanges();
            if(count > 0)
            {
                return Content(outerboxbarcode+"已改为"+(logo==true?"有":"无")+"LOGO.");
            }
            return Content("无修改.");
        }


        #region---自用，入库后删除标签（出库后不能删）
        //根据订单输出包装完成，已入库，但未出库清单
        [HttpPost]
        public async Task<ActionResult> OutputOutsideBoxBarCodeNumListWarehouseJoinNotOutput(string ordernum)
        {
            var OutsideBoxBarCodeWarehouseJoinNumList = await db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.IsOut == false).Select(c => c.OuterBoxBarcode).Distinct().ToListAsync();
            return Content(JsonConvert.SerializeObject(OutsideBoxBarCodeWarehouseJoinNumList));
        }
        #endregion

        //检查外箱条码号是否存在
        [HttpPost]
        public bool CheckOutsideBoxBadeCodeNumExist(string outsidebarcode)
        {
            var count = db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == outsidebarcode);
            if (count > 0) return true;
            else return false;
        }

        //前端用 检查外箱条码号是否存在
        public ActionResult CheckOutsideBoxBadeCodeNumExist1(string outsidebarcode, string ordernum = null)
        {
            JObject message = new JObject();
            var count = db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == outsidebarcode);
            if (count > 0)
            {
                var warejoin = db.Warehouse_Join.Count(c => c.OuterBoxBarcode == outsidebarcode);
                if (warejoin == 0)
                {
                    message.Add("message", "此条码未入库");
                    message.Add("warehouseNum", "");
                    message.Add("barcode", outsidebarcode);
                    return Content(JsonConvert.SerializeObject(message));
                }
                var wareout = db.Warehouse_Join.Count(c => c.OuterBoxBarcode == outsidebarcode && c.IsOut == true);
                if (wareout != 0)
                {
                    var warehouseNum = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == outsidebarcode && c.IsOut == true).Select(c => c.WarehouseNum).FirstOrDefault();
                    message.Add("message", "此条码已出库");
                    message.Add("warehouseNum", warehouseNum);
                    message.Add("barcode", outsidebarcode);
                    return Content(JsonConvert.SerializeObject(message));
                }
                if (!string.IsNullOrEmpty(ordernum))
                {
                    string currentorder = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).FirstOrDefault().OrderNum;
                    if (ordernum != currentorder)
                    {
                        message.Add("message", "此条码的订单号应该是:" + currentorder);
                        message.Add("warehouseNum", "");
                        message.Add("barcode", outsidebarcode);
                        return Content(JsonConvert.SerializeObject(message));
                    }

                }
                var warejoinNum = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == outsidebarcode && c.IsOut == false).Select(c => c.WarehouseNum).FirstOrDefault();
                message.Add("message", "");
                message.Add("warehouseNum", warejoinNum);
                message.Add("barcode", outsidebarcode);
                return Content(JsonConvert.SerializeObject(message));
            }
            else
            {
                message.Add("message", "没有找到此条码");
                message.Add("warehouseNum", "");
                message.Add("barcode", outsidebarcode);
                return Content(JsonConvert.SerializeObject(message));
            }
        }

        //根据外箱条码号条码输出标签图片
        [HttpPost]
        public ActionResult OutsideBoxLablePrintToImg(string outsidebarcode)
        {
            var outsidebarcode_recordlist = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode);
            var screem = outsidebarcode_recordlist.FirstOrDefault().ScreenNum;
            string ordernum = outsidebarcode_recordlist.FirstOrDefault().OrderNum;
            string type = outsidebarcode_recordlist.FirstOrDefault().Type;
            string material_discription = outsidebarcode_recordlist.FirstOrDefault().Materiel;
            string sntn = outsidebarcode_recordlist.FirstOrDefault().SNTN + "/" + db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.ScreenNum == screem).Sum(c => c.Quantity);
            bool logo = outsidebarcode_recordlist.FirstOrDefault().IsLogo;
            string[] mn_list = outsidebarcode_recordlist.Select(c => c.ModuleGroupNum).ToArray();
            if (mn_list[0] == null)
            {
                mn_list = outsidebarcode_recordlist.Select(c => c.BarCodeNum).ToArray();
            }
            string qty = mn_list.Count().ToString();
            int screennum = screem;   //屏序号
            //如果有包装新订单号，则使用包装新订单号。
            string packagingordernum = outsidebarcode_recordlist.FirstOrDefault().PackagingOrderNum;
            ordernum = String.IsNullOrEmpty(packagingordernum) ?outsidebarcode_recordlist.FirstOrDefault().OrderNum: packagingordernum;
            var AllBitmap = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum);
            MemoryStream ms = new MemoryStream();
            AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            AllBitmap.Dispose();
            return File(ms.ToArray(), "image/Png");

            #region--2019-9-29以前代码
            //var screem = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).Select(c => c.ScreenNum).FirstOrDefault();
            //string type = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).FirstOrDefault().Type;
            //string ordernum = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).FirstOrDefault().OrderNum;
            //string material_discription = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).Select(c => c.Materiel).FirstOrDefault();
            //string sntn = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).FirstOrDefault().SNTN + "/" + db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.ScreenNum == screem).Sum(c => c.Quantity);
            //bool logo = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).FirstOrDefault().IsLogo;
            //string[] mn_list = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).Select(c => c.ModuleGroupNum).ToArray();
            //if (mn_list[0] == null)
            //{
            //    mn_list = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).Select(c => c.BarCodeNum).ToArray();
            //}
            //string qty = mn_list.Count().ToString();
            //int screennum = screem;   //屏序号
            //var AllBitmap = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum);
            //MemoryStream ms = new MemoryStream();
            //AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //AllBitmap.Dispose();
            //return File(ms.ToArray(), "image/Png");
            #endregion
        }

        public ActionResult OutsideBoxLablePrintAgain(string outsidebarcode, int pagecount = 1, string ip = "", int port = 0)
        {
            if (!CheckOutsideBoxBadeCodeNumExist(outsidebarcode)) return Content("外箱条码号不存在!");
            var outsidebarcode_recordlist = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode);
            var screem = outsidebarcode_recordlist.FirstOrDefault().ScreenNum;
            string ordernum = outsidebarcode_recordlist.FirstOrDefault().OrderNum;
            string type = outsidebarcode_recordlist.FirstOrDefault().Type;
            string material_discription = outsidebarcode_recordlist.FirstOrDefault().Materiel;
            string sntn = outsidebarcode_recordlist.FirstOrDefault().SNTN + "/" + db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.ScreenNum == screem).Sum(c => c.Quantity);
            bool logo = outsidebarcode_recordlist.FirstOrDefault().IsLogo;
            string[] mn_list = outsidebarcode_recordlist.Select(c => c.ModuleGroupNum).ToArray();
            if (mn_list[0] == null)
            {
                mn_list = outsidebarcode_recordlist.Select(c => c.BarCodeNum).ToArray();
            }
            string qty = mn_list.Count().ToString();
            int screennum = screem;   //屏序号
            //如果有包装新订单号，则使用包装新订单号。
            string packagingordernum = outsidebarcode_recordlist.FirstOrDefault().PackagingOrderNum;
            ordernum = String.IsNullOrEmpty(packagingordernum) ? outsidebarcode_recordlist.FirstOrDefault().OrderNum : packagingordernum;
            var bm = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum);
            int totalbytes = bm.ToString().Length;
            int rowbytes = 10;
            string data = "^XA^MD5~DGR:ZONE.GRF,";
            string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
            data += totalbytes + "," + rowbytes + "," + hex;
            data += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
            string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
            return Content(result);


            #region---2019-9-29以前的代码
            //var screem = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).Select(c => c.ScreenNum).FirstOrDefault();
            //string type = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).FirstOrDefault().Type;
            //string ordernum = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).FirstOrDefault().OrderNum;
            //string material_discription = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).Select(c => c.Materiel).FirstOrDefault();
            //string sntn = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).FirstOrDefault().SNTN + "/" + db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.ScreenNum == screem).Sum(c => c.Quantity);
            //bool logo = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).FirstOrDefault().IsLogo;
            //string[] mn_list = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).Select(c => c.ModuleGroupNum).ToArray();
            //if (mn_list.Count() == 0)
            //{
            //    mn_list = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode).Select(c => c.BarCodeNum).ToArray();
            //}
            //string qty = mn_list.Count().ToString();
            //int screennum = screem;   //屏序号
            //var bm = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum);
            //int totalbytes = bm.ToString().Length;
            //int rowbytes = 10;
            //string data = "^XA^MD5~DGR:ZONE.GRF,";
            //string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
            //data += totalbytes + "," + rowbytes + "," + hex;
            //data += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
            //string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
            //return Content(result);

            ////var AllBitmap = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo);
            ////MemoryStream ms = new MemoryStream();
            ////AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ////AllBitmap.Dispose();
            ////return File(ms.ToArray(), "image/Png");
            #endregion
        }
        #endregion

        #region---生成标签图片
        //生成标签
        public Bitmap CreateOutsideBoxLable(string[] mn_list, string ordernum = "", string outsidebarcode = "", string material_discription = "", string sntn = "", string qty = "", bool logo = true, int screennum = 1)
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
            //竖线
            theGraphics.DrawLine(pen, 250, 280, 250, 400);
            theGraphics.DrawLine(pen, 400, 280, 400, 400);
            theGraphics.DrawLine(pen, 570, 280, 570, 400);

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
            else
            {
                //引入订单号
                System.Drawing.Font myFont_ordernum;
                myFont_ordernum = new System.Drawing.Font("Microsoft YaHei UI", 55, FontStyle.Bold);
                StringFormat geshi = new StringFormat();
                geshi.Alignment = StringAlignment.Center; //居中
                theGraphics.DrawString(ordernum, myFont_ordernum, bush, 100, 60);
            }

            //引入条码
            //if (String.IsNullOrEmpty(outsidebarcode)) return Content("条码号为空！");
            Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(outsidebarcode, 700, 100);
            double beishuhege = 0.7;
            theGraphics.DrawImage(bmp_barcode, 130, 170, (float)(bmp_barcode.Width * beishuhege), (float)(bmp_barcode.Height * beishuhege));

            //引入条码号
            System.Drawing.Font myFont_boxbarcode;
            myFont_boxbarcode = new System.Drawing.Font("Microsoft YaHei UI", 22, FontStyle.Bold);
            theGraphics.DrawString(outsidebarcode, myFont_boxbarcode, bush, 270, 240);

            //引入物料描述
            System.Drawing.Font myFont_material_discription;
            myFont_material_discription = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString("物料描述(DESC)", myFont_material_discription, bush, 55, 295);

            //引入物料描述内容
            System.Drawing.Font myFont_material_discription_content;
            StringFormat geshi1 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            myFont_material_discription_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(material_discription, myFont_material_discription_content, bush, 275, 295);

            //引入屏序号
            System.Drawing.Font myFont_screennum;
            myFont_screennum = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString("屏序号(NO.)", myFont_screennum, bush, 410, 295);

            //引入屏序号值
            System.Drawing.Font myFont_screennum_data;
            StringFormat geshi2 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            myFont_screennum_data = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(screennum.ToString(), myFont_screennum_data, bush, 610, 295);

            //引入SN/TN
            System.Drawing.Font myFont_sntn;
            myFont_sntn = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString("套件/数(SN/TN)", myFont_sntn, bush, 55, 355);

            //引入SN/TN内容
            System.Drawing.Font myFont_sntn_content;
            myFont_sntn_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(sntn, myFont_sntn_content, bush, 290, 355);

            //引入数量QTY
            System.Drawing.Font myFont_qty;
            myFont_qty = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString("数量(QTY)", myFont_qty, bush, 410, 355);

            //引入数量QTY内容
            System.Drawing.Font myFont_qty_content;
            myFont_qty_content = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(qty + " PCS", myFont_qty_content, bush, 585, 355);

            //引入模组号清单
            int mn_E_count = mn_list.Count();
            //12位模组号以上，包括条码号
            if (mn_E_count > 12 && mn_E_count <= 20)
            {
                if (mn_list[0].Length < 7)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 28, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 420;
                    int left_x = 70;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        if ((i % 4) != 0)
                        {
                            left_x += 155;
                        }
                        else
                        {
                            top_y += 100;
                            left_x -= 465;
                        }
                    }
                }
                else
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 22, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 420;
                    int left_x = 55;
                    for (int i = 0; i < mn_list.Count(); i++)
                    {
                        theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        if ((i % 2) == 0)
                        {
                            left_x += 315;
                        }
                        else
                        {
                            top_y += 50;
                            left_x -= 315;
                        }
                    }
                }
            }
            //11-12位模组号
            else if (mn_E_count > 10 && mn_E_count <= 12)
            {
                if (mn_list[0].Length < 7)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 50, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 420;
                    int left_x = 90;
                    for (int i = 0; i < mn_list.Count(); i++)
                    {
                        theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        if ((i % 2) == 0)
                        {
                            left_x += 290;
                        }
                        else
                        {
                            top_y += 85;
                            left_x -= 290;
                        }
                    }
                }
                else
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 26, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 420;
                    int left_x = 160;
                    for (int i = 0; i < mn_list.Count(); i++)
                    {
                        theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 42;
                    }
                }
            }
            //9-10位模组号
            else if (mn_E_count > 8 && mn_E_count <= 10)
            {
                if (mn_list[0].Length < 7)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 45, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 420;
                    int left_x = 90;
                    for (int i = 0; i < mn_list.Count(); i++)
                    {
                        theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        if ((i % 2) == 0)
                        {
                            left_x += 290;
                        }
                        else
                        {
                            top_y += 100;
                            left_x -= 290;
                        }
                    }
                }
                else
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 30, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 420;
                    int left_x = 150;
                    for (int i = 0; i < mn_list.Count(); i++)
                    {
                        theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 50;
                    }

                }

            }
            //7-8位模组号
            else if (mn_E_count > 6 && mn_E_count <= 8)
            {
                if (mn_list[0].Length < 7)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 50, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 420;
                    int left_x = 80;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        if ((i % 2) != 0)
                        {
                            left_x += 300;
                        }
                        else
                        {
                            top_y += 126;
                            left_x -= 300;
                        }
                    }
                }
                else
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 36, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 410;
                    int left_x = 110;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 65;
                    }
                }
            }
            //1-6位模组号
            else if (mn_E_count <= 6)
            {
                if (mn_list[0].Length < 7)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 50, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 450;
                    int left_x = 80;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        if ((i % 2) != 0)
                        {
                            left_x += 300;
                        }
                        else
                        {
                            top_y += 150;
                            left_x -= 300;
                        }
                    }
                }
                else
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 40, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 420;
                    int left_x = 80;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 85;
                    }

                }
            }
            else
            {
                if (mn_list[0].Length < 7)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 28, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 420;
                    int left_x = 70;
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
                            left_x -= 465;
                        }
                    }
                }
                else
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 16, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 420;
                    int left_x = 90;
                    for (int i = 0; i < mn_list.Count(); i++)
                    {
                        theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        if ((i % 2) == 0)
                        {
                            left_x += 315;
                        }
                        else
                        {
                            top_y += 26;
                            left_x -= 315;
                        }
                    }
                }
            }

            //组织数据
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
        }
        #endregion


        public ActionResult OutsideBoxLableRetrun(string bitmap)
        {
            Bitmap theBitmap = ZebraUnity.Base64ToBitmap(bitmap);//把base64字符串转换成Bitmap
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));
            MemoryStream ms = new MemoryStream();
            theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            theBitmap.Dispose();//
            return File(ms.ToArray(), "image/Png");
        }
    }


}
