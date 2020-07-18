﻿using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static JianHeMES.Controllers.CommonalityController;

namespace JianHeMES.Controllers
{
    public class PackagingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonController commom = new CommonController();
        private CommonalityController comm = new CommonalityController();

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

        #region 包装基本信息录入

        /// <summary>
        /// 作用:添加包装基本信息,除了尾箱，一个订单的包装类型的信息应该只有一条
        /// 逻辑：先根据订单删除原有的,在添加刚刚传过来的
        /// 结果:返回成功
        /// </summary>
        /// <param name="packinginfo">规则信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreatePacking(List<Packing_BasicInfo> packinginfo, string Department1, string Group)
        {
            #region  版本1
            //先删除原有的
            var ordernum = packinginfo.Select(c => c.OrderNum).FirstOrDefault();//提出订单号
            var list = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).ToList();//根据订单找到信息
            db.Packing_BasicInfo.RemoveRange(list);//移除
            db.SaveChanges();
            //生成新的
            if (ModelState.IsValid)//判断数据格式是否正确
            {
                packinginfo.ForEach(c => { c.Department = Department1; c.Group = Group; });
                db.Packing_BasicInfo.AddRange(packinginfo);//添加
                db.SaveChanges();
                return Content("ok");
            }
            #endregion
            return View();
        }

        /// <summary>
        /// 作用:根据给的订单号，显示包装信息
        /// 逻辑:根据订单号查找包装录入规则,把其中的包装的类型,容量,数量是否分屏,能否修改信息,将信息放回前端(能放修改逻辑:没有打印的可以修改,已经打印的,返回前端一个已经打印的数量,前端根据这个数量,设置只能增加,不能减少)
        /// 结果:将信息放回前端
        /// </summary>
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
        public ActionResult GetValueFromOrderNum(string ordernum)
        {
            JObject valueitem = new JObject();
            JObject value = new JObject();
            var packingList = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).ToList();//根据订单显示包装规则信息
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
                                                    //分屏
                                                    //if (item.IsSeparate)
                                                    //{
                valueitem.Add("isSeparate", item.IsSeparate); //是否分屏
                valueitem.Add("screenNum", item.ScreenNum); //分屏号
                valueitem.Add("isBatch", item.IsBatch); //是否分批
                valueitem.Add("batch", item.Batch); //批号
                var print = db.Packing_BarCodePrinting.Where(c => (c.CartonOrderNum == item.OrderNum || c.EmbezzleOrderNum == item.OrderNum) && c.Type == item.Type && c.QC_Operator == null && c.Batch == item.Batch && c.ScreenNum == item.ScreenNum).ToList();//根据订单号和类型找包装打印记录
                if (print.Count == 0)
                {
                    valueitem.Add("update", "true"); //没有包装打印记录，可以修改
                    valueitem.Add("min", 0);
                }
                else
                {
                    valueitem.Add("update", "false");//有包装打印记录，不可以修改
                    var count = db.Packing_BarCodePrinting.Where(c => (c.CartonOrderNum == item.OrderNum || c.EmbezzleOrderNum == item.OrderNum) && c.Type == item.Type && c.ScreenNum == item.ScreenNum && c.QC_Operator == null && c.Batch == item.Batch).Select(c => c.OuterBoxBarcode).Distinct().Count();//根据订单、类型、分屏号查找包装打印数量
                    valueitem.Add("min", count);
                }


                //}
                ////不分屏
                //else
                //{
                //valueitem.Add("isSeparate", false);//是否分屏
                //valueitem.Add("screenNum", null);
                //var print = db.Packing_BarCodePrinting.Where(c => (c.CartonOrderNum == item.OrderNum || c.EmbezzleOrderNum == item.OrderNum) && c.Type == item.Type && c.QC_Operator == null).ToList();//根据订单号和类型找包装打印记录
                //if (print.Count == 0)
                //{
                //    valueitem.Add("update", "true");//没有包装打印记录，可以修改
                //    valueitem.Add("min", 0);
                //}
                //else
                // {
                //    valueitem.Add("update", "false");//有包装打印记录，不可以修改
                //    //var count = db.Packing_BarCodePrinting.Where(c => c.OrderNum == item.OrderNum && c.Type == item.Type).Select(c => c.OuterBoxBarcode).Distinct().Count();//根据订单、类型、分屏号查找包装打印数量

                //    var printList = db.Packing_BarCodePrinting.Where(c => (c.CartonOrderNum == item.OrderNum || c.EmbezzleOrderNum == item.OrderNum) && c.Type == item.Type && c.QC_Operator == null).Select(c => c.OuterBoxBarcode).Distinct().Count();//查找已打印的数量

                //    valueitem.Add("min", printList);
                //}
                //}

                value.Add(i.ToString(), valueitem);
                i++;
                valueitem = new JObject();
            }
            return Content(JsonConvert.SerializeObject(value));
        }
        #endregion

        #region 内箱包装信息录入
        /// <summary>
        /// 作用:内箱确认录入
        /// 逻辑:查找内箱包装确认表,查看是否有重复记录
        /// 结果:,查看是否有重复记录,如果有,怎提示出来,没有则录入表中
        /// </summary>
        /// <param name="packing_InnerChecks">内箱确认数据集合</param>
        /// <returns></returns>
        public ActionResult InnerBoxCheck(List<Packing_InnerCheck> packing_InnerChecks, string Department1, string Group)
        {
            if (ModelState.IsValid)//判断数据格式是否正确
            {
                string error = "";
                foreach (var item in packing_InnerChecks)//循环内箱确认数据
                {
                    var check = db.Packing_InnerCheck.Count(c => c.Barcode == item.Barcode);//查找是否含有重复条码
                    if (check > 0)//如果有,则记录在error,进行下一个循环
                    {
                        error = error + item.Barcode + ",";
                        continue;
                    }
                    item.QC_Operator = ((Users)Session["User"]).UserName;//登录人
                    item.QC_ComfirmDate = DateTime.Now;//现在时间
                    item.Department = Department1;
                    item.Group = Group;
                    db.Packing_InnerCheck.Add(item);//添加数据
                    db.SaveChanges();
                };
                if (!string.IsNullOrEmpty(error))//判断error是否有内容,由内容则提示错误
                {
                    return Content(error + "条码已重复");
                }
                return Content("true");
            }
            return Content("false");
        }

        /// <summary>
        /// 作用:输出已经确认的条码列表
        /// 逻辑:根据订单号找出已经内箱确认的条码列表
        /// 结果:返回带有条码,和模组号的json内容
        /// </summary>
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
        public ActionResult GetBarcodeList(string ordernum)
        {
            //根据订单号查内箱确认清单
            var list = db.Packing_InnerCheck.Where(c => c.OrderNum == ordernum).OrderBy(c => c.Barcode).ToList();//从内箱确认呢表中根据订单找到条码号
            JArray List = new JArray();
            foreach (var item in list)//循环条码列表
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

        /// <summary>
        /// 通过订单号和是否尾箱,得到毛重量和净重
        /// </summary>
        /// 先根据订单和类型.屏序找到信息,判断是否是尾箱,是尾箱找尾箱的重量,不是尾箱找整箱重量,没有信息返回false
        /// <param name="ordernum">订单号</param>
        /// <param name="IsLast">是否尾箱</param>
        /// <param name="type">类型</param>
        /// <param name="screenNum">屏序</param>
        /// <returns></returns>
        public ActionResult GetWight(string ordernum, bool IsLast, string type, int screenNum)
        {
            JObject result = new JObject();
            var info = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == type && c.ScreenNum == screenNum).ToList(); //查找包装规则信息
            if (info.Count == 0)//如果找不到包装规则信息
            {
                result.Add("G_Weight", null);//毛重量
                result.Add("N_Weight", null);//净重
                result.Add("pass", false);
                result.Add("mes", "没找到该订单的类型包装信息");
                return Content(JsonConvert.SerializeObject(result));
            }
            if (IsLast)//是尾箱
            {
                var last = info.Where(c => c.Tail_G_Weight != null && c.Tail_N_Weight != null).FirstOrDefault();//查找尾箱重量不为0的信息
                if (last != null)//如果信息不为空
                {
                    result.Add("G_Weight", last.Tail_G_Weight);//毛重量
                    result.Add("N_Weight", last.Tail_N_Weight);//净重
                    result.Add("pass", true);
                    result.Add("mes", "成功");
                    return Content(JsonConvert.SerializeObject(result));
                }

            }
            else//不是尾箱
            {
                var full = info.Where(c => c.Full_G_Weight != null && c.Full_N_Weight != null).FirstOrDefault();//找到整箱重量不为0的信息
                if (full != null)//如果信息不为空
                {
                    result.Add("G_Weight", full.Full_G_Weight);//毛重量
                    result.Add("N_Weight", full.Full_N_Weight);//净重
                    result.Add("pass", true);
                    result.Add("mes", "成功");
                    return Content(JsonConvert.SerializeObject(result));
                }
            }
            //都不行
            result.Add("G_Weight", null);//毛重量
            result.Add("N_Weight", null);//净重
            result.Add("pass", false);
            result.Add("mes", "找不到重量信息,请确认已经定义了重量信息");
            return Content(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 作用:打印条码界面，显示完成数量
        /// 逻辑:根据订单查找包装规则中定义的类型集合,循环类型,找到每个类型对应的屏序号集合,循环屏序号,将其中的类型,完成数量(打印数/定义数),屏序,完成率(打印数 除以 定义数),和总的完成数量(打印数/定义数),屏序,完成率(打印数 除以 定义数)显示出来
        /// 结果:返回json文件或者空josn文件
        /// </summary>
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
        public ActionResult GetcompleteInfo(string ordernum)
        {

            //var type = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Select(c => c.Type).Distinct().ToList();//查找订单查包装规则的类型
            JArray total = new JArray();
            total = GetcompleteInfoFunction(ordernum);

            return Content(JsonConvert.SerializeObject(total));
        }
        public JArray GetcompleteInfoFunction(string ordernum)
        {
            var type = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Select(c => c.Type).Distinct().ToList();//查找订单查包装规则的类型
            JArray total = new JArray();
            foreach (var item in type)//循环类型
            {
                var batchList = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == item).Select(c => c.Batch).Distinct().ToList();//根据订单、类型查找包装规则的分屏号
                foreach (var batchnum in batchList)//循环批号
                {
                    var screemNumList = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == item && c.Batch == batchnum).Select(c => c.ScreenNum).Distinct().ToList();
                    foreach (var screenNum in screemNumList)//循环屏序号
                    {
                        JObject info = new JObject();
                        var totleNum = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNum && c.Batch == batchnum).ToList().Count() == 0 ? 0 : db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNum && c.Batch == batchnum).Sum(c => c.Quantity);//根据订单号、类型、分屏号查找包装规则的包装总数量(用于显示完成数量的分母显示)
                        var printBarcodeinfo = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == ordernum && c.Type == item && c.ScreenNum == screenNum && c.QC_Operator == null && c.Batch == batchnum).Select(c => c.OuterBoxBarcode).Distinct();//根据订单号根据订单号、类型、分屏号查找包装打印外箱列表(用于显示完成数量的分子显示)
                        int printBarcode = printBarcodeinfo.Count();
                        //类型
                        info.Add("type", item);
                        //完成数量
                        info.Add("completeNum", printBarcode.ToString() + "/" + totleNum.ToString());
                        //屏序
                        info.Add("screenNum", screenNum);
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
            var printBarcodeinfo2 = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == ordernum && c.QC_Operator == null).Select(c => c.OuterBoxBarcode).Distinct().Count();//根据订单查包装打印的总的外箱数量
            var totle = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).ToList();
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

            return total;
        }

        /// <summary>
        /// 作用:根据条码号，显示箱体号,用于打印外箱标签,扫条码显示箱体号
        /// 逻辑:查找条码表,根据订单找到模组号
        /// 结果:如果没有模组号,则返回空模组和订单json,有则返回模组和订单json
        /// </summary>
        /// <param name="barcode">条码号</param>
        /// <returns></returns>
        public ActionResult GetModulbarCode(string barcode)
        {
            var list = db.BarCodes.Where(c => c.BarCodesNum == barcode).Select(c => new { c.ModuleGroupNum, c.OrderNum }).FirstOrDefault();//根据条码找模组号
            JObject result = new JObject();
            if (!string.IsNullOrEmpty(list.ModuleGroupNum))//判断是否有模组号,有就返回模组号
            {
                result.Add("module", list.ModuleGroupNum);
                result.Add("orderNum", list.OrderNum);
                return Content(JsonConvert.SerializeObject(result));
            }
            else//没有模组号为空
            {
                result.Add("module", "");
                result.Add("orderNum", list.OrderNum);
                return Content(JsonConvert.SerializeObject(result));
            }
        }

        /// <summary>
        /// 作用:判断条码与订单是否相符(用于内箱检测时或者是外箱打印条码时,扫条码,判断条码是否与订单相符)
        /// 逻辑与结果:
        /// 根据条码在BarCodes找到订单号,订单号找不到则返回"不存在此条码".
        /// 如果找到的订单号与传过来的订单号不服,则返回"此条码的订单号应为XX",如果传过来的hybrid为true,则不进行此判断,.
        /// 根据条码在Appearance表找已经完成电检的信息,如果没有条码信息,则返回"此条码未通过外观电检"
        /// 根据条码在iscCheck表中找是否有信息,有信息,并且传过来的isInner为true ,则返回"此条码已完成过内箱装箱确认"
        /// </summary>
        /// <param name="ordernum">订单</param>
        /// <param name="barcode">条码</param>
        /// <param name="isInner">是否有完成内箱检查</param>
        /// <param name="hybrid">是否是混装,混装不进行条码与订单判断</param>
        /// <returns></returns>
        public string IsCheckBarcode(string ordernum, string barcode, bool isInner = false, bool hybrid = false)
        {
            var order = db.BarCodes.Where(c => c.BarCodesNum == barcode).Select(c => c.OrderNum).FirstOrDefault();//根据条码在条码表找订单
            var exit = db.Appearance.Count(c => c.BarCodesNum == barcode && c.OQCCheckFinish == true);//根据条码查找完成电检数量
            var iscCheck = db.Packing_InnerCheck.Count(c => c.Barcode == barcode);//查找内检完的数量
            if (order == null)
            {
                return "不存在此条码";
            }
            else if (order != ordernum && hybrid == false)
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

        /// <summary>
        /// 作用:通过输入订单号和包装类型，传出数量
        /// </summary>
        /// 逻辑与结果:现找到此类型此订单已经打印了几个,再从规则表里找到此类型此订单一共要打印几个,
        /// 如果在规则表里没有找到数据,则表示没有此类型,
        /// 若只有一条数据,则用规则表里的数据-打印数据,若值==或小于0则表示该类型已经全部打印完了,否则返回数值
        /// 若有两条数据,表示其中一条是尾箱,在两条信息中,找到数量大的那条,表示整箱,查看整箱是否全部打印完了,没有打印完,返回数值,打印完了,在去找数量小的那条,表示尾箱,查看尾箱是否全部打印完,没有打印完返回数值,否则表示都打印完了
        /// <param name="ordernum">订单号</param>
        /// <param name="type">类型</param>
        /// <param name="screenNum">分屏号,默认为1</param>
        /// <returns></returns>
        public int GetNum(string ordernum, string type, int screenNum = 1, int batchNum = 1)
        {

            var printBarcode = db.Packing_BarCodePrinting.Count(c => c.CartonOrderNum == ordernum && c.Type == type && c.ScreenNum == screenNum && c.QC_Operator == null && c.Batch == batchNum);//根据订单号,类型,分屏号在打印表中找符合条件的数量
            var typeNum = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == type && c.ScreenNum == screenNum && c.Batch == batchNum).ToList();//根绝订单,装箱类型,分屏号找到包装数量信息
            //没有在订单中找到此包装信息
            if (typeNum.Count == 0)
            {
                return 0;
            }
            else
            {
                if (typeNum.Count == 1)//如果只有一条数据，说明此订单没有选择此包箱类型做尾箱
                {
                    var num = typeNum.FirstOrDefault().Quantity;//定义的箱数
                    var boxnum = typeNum.FirstOrDefault().OuterBoxCapacity;//定义的每箱容量
                    //查看打印的条码数是否超过定义数
                    if (printBarcode < (num * boxnum))//没有超过就返回 定义的每箱容量 给前端,提示此装箱类型,只能装这么多箱数
                    {
                        return boxnum;
                    }
                    else
                        return 0;
                }
                //如果有2条数据，说明此订单选择此包箱类型做尾箱(现在没有定义尾箱)
                if (typeNum.Count == 2)
                {
                    var numMin = typeNum.Min(c => c.Quantity);//比较小的箱数,代表尾箱箱数
                    var numMax = typeNum.Max(c => c.Quantity);//比较大的箱数,代表正常箱数
                    var boxnumMin = typeNum.Where(c => c.Quantity == numMin).FirstOrDefault().OuterBoxCapacity;//比较小的每箱容量,代表尾箱每箱容量
                    var boxnumMax = typeNum.Where(c => c.Quantity == numMax).FirstOrDefault().OuterBoxCapacity;//比较大的每箱容量,代表正常每箱容量
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

        }

        /// <summary>
        /// 外箱条码标签信息
        /// </summary>
        /// 找到定义的包装数量,找到已经打印的数量,判断定义数量是否大于打印数量,不是则返回null,
        /// 是则 分割订单号,得到外箱标签的前缀 如17TEST-1-01-,从1开始循环,总循环定义数,找到已打印的标签里面有没有相同的外箱标签条码号,没有则返回此条码号.有则继续循环
        /// <param name="ordernum">订单</param>
        /// <param name="screenNum">分屏号</param>
        /// <returns></returns>
        public ActionResult GetOuterBoxBarCodeInfo(string ordernum, int screenNum, int batchNum)
        {
            var count = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batchNum).ToList().Count() == 0 ? 0 : db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batchNum).Sum(c => c.Quantity);//根据订单、分屏号查找规定的包装数量总和
            var printCount = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batchNum).Select(c => c.OuterBoxBarcode).Distinct().ToList().Count();//根据订单、分屏号查找打印的外箱数量
            if (printCount < count)//判断打印的数量是否大于定义包装数量,如果打印数量大于等于定义包装数量,则返回null
            {
                JObject info = new JObject();

                #region 外箱条码生成

                string[] str = ordernum.Split('-');//将订单号分割
                string OuterBoxBarCodeNum = "";
                string OldOuterBoxBarCodeNum = "";
                if (str.Count() == 2)
                {
                    OuterBoxBarCodeNum = ordernum + "-" + batchNum.ToString().PadLeft(2, '0') + "-" + screenNum.ToString().PadLeft(2, '0') + "-";
                }
                else
                {
                    string start = str[0].Substring(2);//取出 如2017-test-1 的17
                    OuterBoxBarCodeNum = start + str[1] + "-" + str[2] + "-" + batchNum.ToString().PadLeft(2, '0') + "-" + screenNum.ToString().PadLeft(2, '0') + "-";//外箱条码组成 
                    OldOuterBoxBarCodeNum = start + str[1] + "-" + str[2] + "-" + screenNum.ToString().PadLeft(2, '0') + "-";//外箱条码组成 
                }
                int SN = 0;
                for (int i = 1; i <= count; i++)//从1开始循环,最大数为定义的打印数,用来确定标签的序列号
                {
                    var num = OuterBoxBarCodeNum + i.ToString().PadLeft(3, '0');//生成含有序列数的标签号
                    var oldnum = OldOuterBoxBarCodeNum + i.ToString().PadLeft(3, '0');//生成旧的含有序列数的标签号
                    if (db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == oldnum) == 0 && db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == num) == 0)//判断打印表里是否有相同的标签号,没有则将此标签号存入数据中,有则继续循环
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

        /// <summary>
        /// 查看条码号是否已经打印过了
        /// </summary>
        /// 在Packing_BarCodePrinting表中查看是否有重复的条码号,有则false,无则true 
        /// <param name="barcode">条码号</param>
        /// <returns></returns>
        public string CheckBarcode(string barcode)
        {
            var exit = db.Packing_BarCodePrinting.Where(c => c.BarCodeNum == barcode).Select(c => c.BarCodeNum).FirstOrDefault();//查找打印表里是否有相同的条码号
            if (exit != null)//有就传fasle
            {
                return "false";
            }
            else//没有就传true
                return "true";
        }

        /// <summary>
        /// 创建外箱打印记录
        /// </summary>
        /// 首先检查需要录入外箱打印表里面的条码号是否重复,重复则提示错误,然后再找订单定义的总数量,将已打印的数量+此次要添加记录的数量,等到的数量与定义总数量做对比,如果定义总数量小于打印数量,则提示错误,否则添加数据
        /// <param name="printings">外箱打印记录</param>
        /// <param name="ordernum">订单号</param>
        /// <param name="isupdate">是否是更新</param>
        /// <returns></returns>
        public string CreatePackangPrint(List<Packing_BarCodePrinting> printings, string ordernum, string Department1, string Group, bool isupdate = false)
        {
            if (ModelState.IsValid)//判断数据格式是否正确
            {
                if (isupdate)//更新条码信息，现在没用
                {
                    string outherbarcode = printings.FirstOrDefault().OuterBoxBarcode;
                    printings.ForEach(c => c.Date = DateTime.Now);
                    var delete = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outherbarcode && c.QC_Operator == null).ToList();
                    db.Packing_BarCodePrinting.AddRange(printings);
                    db.Packing_BarCodePrinting.RemoveRange(delete);
                    db.SaveChanges();
                    return "true";
                }
                string error = "";
                foreach (var item in printings)//循环传过来的打印数据集合
                {
                    var exit = db.Packing_BarCodePrinting.Where(c => c.BarCodeNum == item.BarCodeNum && c.QC_Operator == null).FirstOrDefault();//查看条码是否已经打印了
                    if (exit != null)//已经打印,将信息记录在error中
                    {
                        error = error + exit.BarCodeNum + "已包装在" + exit.OuterBoxBarcode + ",";
                    }
                }
                if (!string.IsNullOrEmpty(error))//如果error含有数据,则显示数据
                {
                    return error;
                }
                else//代表没有重复打印的
                {
                    // var temp = printings.FirstOrDefault().OrderNum;
                    var count = db.Packing_BarCodePrinting.Count(c => c.CartonOrderNum == ordernum && c.QC_Operator == null);//根据订单找打印表数量,代表此订单 已打印的数量
                    var real = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum);//查看此订单是否有定义装箱规则
                    int realCount = 0;
                    foreach (var item in real)//循环
                    {
                        realCount = realCount + (item.Quantity * item.OuterBoxCapacity);//计算出总共应该打印多少条码数(装箱容量*装箱数)
                    }
                    if (realCount < count + printings.Count())//如果已打印的数量大于定义总数量
                    {
                        return "已超过定义的包装数量";
                    }
                    printings.ForEach(c => { c.CartonOrderNum = ordernum; c.Department = Department1; c.Group = Group; });//保存数据
                    db.Packing_BarCodePrinting.AddRange(printings);
                    db.SaveChanges();
                    return "true";
                }
            }
            return "传入类型不对，请确认";
        }

        #region 删除外箱条码
        /// <summary>
        /// 删除外箱条码记录
        /// </summary>
        /// 查看inside是否为true ,为true则是删除入库,否则是删除外箱打印记录,删除完后,记录日志
        /// <param name="ordernum">订单号</param>
        /// <param name="barcodelist">外箱条码列表</param>
        /// <param name="inside">是否是删除入库的</param>
        public void DeleteBarcode(string ordernum, List<string> barcodelist, bool inside = false)
        {
            string message = "";
            foreach (var item in barcodelist)//循环条码列表
            {
                if (inside)//删除入库记录
                {
                    var warehous = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == item && c.IsOut == false).ToList();//未出库已入库的列表
                    db.Warehouse_Join.RemoveRange(warehous);//删除
                    db.SaveChanges();

                    string barcdeoList = "";
                    foreach (var barcode in warehous)//循环未出库已入库的列表,将条码全部存入barcdeoList
                    {
                        if (barcdeoList == "")
                            barcdeoList = barcode.BarCodeNum + ":" + barcode.ModuleGroupNum;
                        else
                            barcdeoList = barcdeoList + "," + barcode.BarCodeNum + ":" + barcode.ModuleGroupNum;
                    }
                    message = message + "外箱条码:" + item + ",模组条码:" + barcdeoList;//信息
                }
                else//删除外箱记录
                {
                    var list = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == item && c.QC_Operator == null).ToList();//已打印的列表
                    db.Packing_BarCodePrinting.RemoveRange(list);//移除记录
                    db.SaveChanges();

                    string barcdeoList = "";
                    foreach (var barcode in list)//循环已打印的列表,将条码全部存入barcdeoList
                    {
                        if (barcdeoList == "")
                            barcdeoList = barcode.BarCodeNum + ":" + barcode.ModuleGroupNum;
                        else
                            barcdeoList = barcdeoList + "," + barcode.BarCodeNum + ":" + barcode.ModuleGroupNum;
                    }
                    message = message + "外箱条码:" + item + ",模组条码:" + barcdeoList;//信息
                }
            }
            //填写日志
            UserOperateLog log = new UserOperateLog { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = inside == true ? ("删除入库记录//" + message) : ("删除外箱记录//" + message) };
            db.UserOperateLog.Add(log);
            db.SaveChanges();
        }
        #endregion
        #endregion

        #region 备品配件包装操作
        public ActionResult SPC_Display()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "SPC_Display" });
            }
            ViewBag.OrderList = GetOrderList();
            return View();
        }

        #region---1.物料基本信息输入页面
        public ActionResult SPC_Addbasic_information()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "SPC_Display" });
            }
            return View();
        }

        [HttpPost]
        #region---判断此订单号、屏序、批次、类型是否已有对应的清单上传
        public ActionResult SPC_existsData(string ordernum, int batch, int screenNum, string materialType)
        {
            var result = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batch && c.SPC_Material_Type == materialType).Count();
            if (result > 0) return Content(JsonConvert.SerializeObject(result));
            else return Content("没有对应记录！");
        }
        #endregion

        #region---导入页面查订单、屏序、批次
        [HttpPost]
        public ActionResult SPC_Addbasic_information_select(string ordernumber, int screennumber, int batch, string spc_Material_Type)
        {
            //根据订单号、屏序、批次查找记录
            var resultlist = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernumber && c.ScreenNum == screennumber && c.Batch == batch && c.SPC_Material_Type == spc_Material_Type).ToList();
            var result = resultlist.Select(c => new { c.OrderNum, c.Batch, c.ScreenNum, c.MaterialNumber, c.Material_Name, c.Specification_Description, c.Quantity, c.Unit, c.Id, c.SPC_Material_Type, c.SPC_OuterBoxBarcode }).ToList();
            if (result.Count <= 0) return Content("没有记录！");
            else return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region--- 导入页面删除订单、屏序、批次所有记录
        [HttpPost]
        public ActionResult SPC_Addbasic_information_delete(string ordernumber, int screennumber, int batch, string spc_Material_Type)
        {
            //根据订单号、屏序、批次删除
            var resultlist = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernumber && c.ScreenNum == screennumber && c.Batch == batch && c.SPC_Material_Type == spc_Material_Type).ToList();
            db.Packing_SPC_Records.RemoveRange(resultlist);
            int resutl = db.SaveChanges();
            string username = ((Users)Session["User"]).UserName;//登录人
            var message = db.Personnel_Roster.Where(c => c.Name == username).FirstOrDefault();//去花名册找登录人所在部门
            string address = "";
            string content = "订单号为：" + ordernumber + ",屏序为：" + screennumber + ",批次为：" + batch + "物料类型为：" + spc_Material_Type + "的所有物料已被删除。";
            if (resutl > 0)
            {
                if (message.Department == "总装一部")
                {
                    var adress_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件发送老化包装一部").ToList();//找出发送人名单
                    var copy_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件抄送老化包装一部").ToList();//找出发送人名单
                    var sender = db.UserItemEmail.Where(c => c.ProcesName == "备品配件一部发送人").FirstOrDefault();
                    var item = commom.SendEmail(sender.EmailAddress, adress_list.Select(c => c.EmailAddress).ToList(), content, "备品配件整批物料删除通知", copy_list.Select(c => c.EmailAddress).ToList(), address);
                    if (item == true) return Content("删除成功，邮件发送成功！");
                    else return Content("删除成功，邮件发送失败！");
                }
                else if (message.Department == "总装二部")
                {
                    var adress_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件发送老化包装二部").ToList();//找出发送人名单
                    var copy_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件抄送老化包装二部").ToList();//找出抄送人名单
                    var sender = db.UserItemEmail.Where(c => c.ProcesName == "备品配件二部发送人").FirstOrDefault();
                    var item = commom.SendEmail(sender.EmailAddress, adress_list.Select(c => c.EmailAddress).ToList(), content, "备品配件整批物料删除通知", copy_list.Select(c => c.EmailAddress).ToList(), address);
                    if (item == true) return Content("删除成功，邮件发送成功！");//判断邮件是否发送成功
                    else return Content("删除成功，邮件发送失败！");
                }
                else
                {
                    return Content("删除成功，未发送邮件！");
                }
            }
            else
            {
                return Content("删除失败！");
            }
        }
        #endregion

        #region--- 导入页面单个物料删除
        [HttpPost]
        public ActionResult SPC_single_delete_mterial(int id)
        {
            //根据id删除物料
            var resultlist = db.Packing_SPC_Records.Where(c => c.Id == id).ToList();
            db.Packing_SPC_Records.RemoveRange(resultlist);
            int result = db.SaveChanges();
            string username = ((Users)Session["User"]).UserName;//登录人
            var message = db.Personnel_Roster.Where(c => c.Name == username).FirstOrDefault();//去花名册找登录人所在部门
            string address = "";
            var con = resultlist.FirstOrDefault();
            string content = "订单号为：" + con.OrderNum + ",屏序为：" + con.ScreenNum + ",批次为：" + con.Batch + ",物料类型为：" + con.SPC_Material_Type + ",物料号为：" + con.MaterialNumber + ",数量为：" + con.Quantity + "个的物料已被删除。";
            if (result > 0)
            {
                if (message.Department == "总装一部")
                {
                    var adress_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件发送老化包装一部").ToList();//找出发送人名单
                    var copy_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件抄送老化包装一部").ToList();//找出发送人名单 
                    var sender = db.UserItemEmail.Where(c => c.ProcesName == "备品配件一部发送人").FirstOrDefault();
                    var item = commom.SendEmail(sender.EmailAddress, adress_list.Select(c => c.EmailAddress).ToList(), content, "备品配件单个物料删除通知", copy_list.Select(c => c.EmailAddress).ToList(), address);
                    if (item == true) return Content("删除成功，邮件发送成功！");
                    else return Content("删除成功，邮件发送失败！");
                }
                else if (message.Department == "总装二部")
                {
                    var adress_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件发送老化包装二部").ToList();//找出发送人名单
                    var copy_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件抄送老化包装二部").ToList();//找出抄送人名单
                    var sender = db.UserItemEmail.Where(c => c.ProcesName == "备品配件二部发送人").FirstOrDefault();
                    var item = commom.SendEmail(sender.EmailAddress, adress_list.Select(c => c.EmailAddress).ToList(), content, "备品配件单个物料删除通知", copy_list.Select(c => c.EmailAddress).ToList(), address);
                    if (item == true) return Content("删除成功，邮件发送成功！");//判断邮件是否发送成功
                    else return Content("删除成功，邮件发送失败！");
                }
                else
                {
                    return Content("删除成功，未发送邮件！");
                }
            }
            else return Content("删除失败！");
        }
        #endregion

        #region--- 导入页面修改描述信息方法
        [HttpPost]
        public ActionResult SPC_Addbasic_information_modify(List<Packing_SPC_Records> newlist)
        {
            var ol = newlist.FirstOrDefault().Id;
            var con = db.Packing_SPC_Records.FirstOrDefault(c => c.Id == ol);
            var change = newlist.FirstOrDefault();
            //邮件内容
            string content = "订单号为：" + con.OrderNum + ", " + "屏序为：" + con.ScreenNum + ", " + "批次为：" + con.Batch + " 的料号(" + con.MaterialNumber + ")，物料类型为：" + con.SPC_Material_Type + "，";
            if (con.MaterialNumber != change.MaterialNumber)
            {
                content += "更改料号为（" + change.MaterialNumber + ")";
            };
            if (con.Material_Name != change.Material_Name)
            {
                content += "      " + "更改了物料名称";
            };
            if (con.Specification_Description != change.Specification_Description)
            {
                content += "      " + "更改了规格描述";
            }
            if (con.Quantity != change.Quantity)
            {
                content += "      " + "更改了数量";
            }
            if (con.Unit != change.Unit)
            {
                content += "      " + "更改了单位";
            }
            if (con.ScreenNum != change.ScreenNum)
            {
                content += "      " + "更改了屏序，屏序为："+change.ScreenNum;
            }
            if (con.Batch != change.Batch)
            {
                content += "      " + "更改了批次，批次为："+change.Batch;
            }
            if (con.SPC_Material_Type!= change.SPC_Material_Type)
            {
                content += "      " + "更改了物料类型，类型为："+change.SPC_Material_Type;
            }
            if (newlist != null)
            {
                int count = 0;
                foreach (var item in newlist)
                {
                    var record = db.Packing_SPC_Records.FirstOrDefault(c => c.Id == item.Id);
                    record.MaterialNumber = item.MaterialNumber;//物料号
                    record.Material_Name = item.Material_Name;//物料名称
                    record.Specification_Description = item.Specification_Description;//规格描述
                    record.Unit = item.Unit;//单位
                    record.Quantity = item.Quantity;//数量
                    record.ScreenNum = item.ScreenNum;//屏序
                    record.Batch = item.Batch;//批次
                    record.SPC_Material_Type = item.SPC_Material_Type;//物料类型
                    count = count + db.SaveChanges();
                }
                string username = ((Users)Session["User"]).UserName;//登录人
                var message = db.Personnel_Roster.Where(c => c.Name == username).FirstOrDefault();//去花名册找登录人所在部门
                string address = "";
                if (count > 0)
                {
                    if (message.Department == "总装一部")
                    {
                        var adress_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件发送老化包装一部").ToList();//找出发送人名单
                        var copy_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件抄送老化包装一部").ToList();//找出发送人名单    
                        var sender = db.UserItemEmail.Where(c => c.ProcesName == "备品配件一部发送人").FirstOrDefault();
                        var item = commom.SendEmail(sender.EmailAddress, adress_list.Select(c => c.EmailAddress).ToList(), content, "备品配件物料修改通知", copy_list.Select(c => c.EmailAddress).ToList(), address);
                        if (item == true) return Content("修改成功，邮件发送成功！");
                        else return Content("修改成功，邮件发送失败！");
                    }
                    else if (message.Department == "总装二部")
                    {
                        var adress_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件发送老化包装二部").ToList();//找出发送人名单
                        var copy_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件抄送老化包装二部").ToList();//找出抄送人名单
                        var sender = db.UserItemEmail.Where(c => c.ProcesName == "备品配件二部发送人").FirstOrDefault();
                        var item = commom.SendEmail(sender.EmailAddress, adress_list.Select(c => c.EmailAddress).ToList(), content, "备品配件物料修改通知", copy_list.Select(c => c.EmailAddress).ToList(), address);
                        if (item == true) return Content("修改成功，邮件发送成功！");//判断邮件是否发送成功
                        else return Content("修改成功，邮件发送失败！");
                    }
                    else
                    {
                        return Content("修改成功，未发送邮件！");
                    }
                }
                else return Content("修改失败！");
            }
            else
            {
                return Content("传进来的数据为空!");
            }
        }
        #endregion

        #region--清单导入（文件上传）
        public class updatamodel
        {
            public string OrderNum { get; set; }//订单号
            public int? ScreenNum { get; set; }//屏序
            public int? Batch { get; set; }//批次
            public string Material_Name { get; set; }//物品名称
            public string MaterialNumber { get; set; }//物料号
            public string Specification_Description { get; set; }//规格描述
            public Decimal Quantity { get; set; }//数量
            public string Unit { get; set; }//单位
            public int? TableNumber { get; set; }//表序
            public string SPC_Material_Type { get; set; }//物品类型
            public string SPC_projectName { get; set; }//物品类型
        }

        #region----上传清单并读取数据
        public ActionResult SPC_Addbasic_information_updatafile()
        {
            try
            {
                HttpPostedFileBase uploadfile = Request.Files["fileup"];//上传的文件
                string ordernum = Request.Form["ordernum"];//普通参数获取  订单号
                string pingxu = Request.Form["screenNum"];//屏序
                string pici = Request.Form["batch"];//批次
                string spc_Material_Type = Request.Form["spc_Material_Type"];//批次
                string projectname = Request.Form["projectName"];//项目名
                //批次
                if (uploadfile == null)
                {
                    return Content("no:非法上传");
                }
                if (uploadfile.FileName == "")
                {
                    return Content("no:请选择文件");
                }
                string fileExt = Path.GetExtension(uploadfile.FileName);
                StringBuilder sbtime = new StringBuilder();
                sbtime.Append(DateTime.Now.Year).Append(DateTime.Now.Month).Append(DateTime.Now.Day).Append(DateTime.Now.Hour).Append(DateTime.Now.Minute).Append(DateTime.Now.Second);
                string dir = "/UploadFile/" + sbtime.ToString() + fileExt;
                string realfilepath = Request.MapPath(dir);
                string readDir = Path.GetDirectoryName(realfilepath);
                if (!Directory.Exists(readDir))
                    Directory.CreateDirectory(readDir);
                uploadfile.SaveAs(realfilepath);
                //提取数据 
                var dt = ExcelTool.ExcelToDataTable(true, realfilepath);
                //找出开始行
                int beginline = 0;
                //末尾行
                int dataendline = dt.Rows.Count - 1;
                List<updatamodel> recordlist = new List<updatamodel>();
                updatamodel record = new updatamodel();
                for (int i = beginline; i <= dataendline; i++)
                {
                    int? batch = int.Parse(pici);
                    int? screenNum = int.Parse(pingxu);
                    record.OrderNum = ordernum;//订单号
                    record.Batch = batch;//批次
                    record.ScreenNum = screenNum;//屏序
                    record.SPC_Material_Type = spc_Material_Type;
                    record.Material_Name = dt.Rows[i][0].ToString();//物品名称
                    record.MaterialNumber = dt.Rows[i][1].ToString() == "" ? "" : dt.Rows[i][1].ToString();//物料号
                    record.Specification_Description = dt.Rows[i][2].ToString();//规格描述
                    record.Quantity = dt.Rows[i][3] == "" ? 0 : Convert.ToDecimal(dt.Rows[i][3]);//数量
                    record.Unit = dt.Rows[i][4].ToString();//单位
                    record.SPC_projectName = projectname;//项目名
                    if (record.Material_Name != "" && record.Quantity != 0 && record.Unit != "")
                    {
                        recordlist.Add(record);
                        record = new updatamodel();
                    }
                }
                if (recordlist.Count > 0)
                {
                    return Content(JsonConvert.SerializeObject(recordlist));
                }
                else
                {
                    return Content("未读取到excel文件");
                }
            }
            catch
            {
                return Content("文件上传失败！");
            }
        }
        #endregion

        #region----保存清单数据
        public ActionResult SaveListing(List<Packing_SPC_Records> list)
        {
            if (list.Count != 0)
            {
                JArray arr = new JArray();
                foreach (var item in list)
                {
                    if (item.MaterialNumber == null)//没有物料编号                      
                    {
                        Packing_SPC_Records recor = new Packing_SPC_Records();
                        recor.ListingImport_Operator = ((Users)Session["User"]).UserName;//清单导入人
                        recor.ListingImport_Date = DateTime.Now;//清单导入时间
                        recor.OrderNum = item.OrderNum;
                        recor.MaterialNumber = item.MaterialNumber;
                        recor.Specification_Description = item.Specification_Description;//规格描述
                        recor.Quantity = item.Quantity;//数量
                        recor.Material_Name = item.Material_Name;//物品名称
                        recor.Unit = item.Unit;//单位
                        recor.Batch = item.Batch; //批次
                        recor.ScreenNum = item.ScreenNum;//屏序
                        recor.SPC_Material_Type = item.SPC_Material_Type;//物品类型
                        recor.SPC_projectName = item.SPC_projectName;
                        db.Packing_SPC_Records.Add(recor);
                        int savecout = db.SaveChanges();
                        if (savecout < 0) arr.Add(item.MaterialNumber);
                    }
                    else//有物料编号
                    {
                        Packing_SPC_Records pack = new Packing_SPC_Records();
                        pack.ListingImport_Operator = ((Users)Session["User"]).UserName;//清单导入人
                        pack.ListingImport_Date = DateTime.Now;//清单导入时间
                        pack.OrderNum = item.OrderNum;//订单号
                        pack.MaterialNumber = item.MaterialNumber;//物料编号
                        pack.Specification_Description = item.Specification_Description;//规格描述
                        pack.Quantity = item.Quantity;//数量
                        pack.Material_Name = item.Material_Name;//物品名称
                        pack.Unit = item.Unit;//单位
                        pack.Batch = item.Batch;//批次
                        pack.ScreenNum = item.ScreenNum;//屏序
                        pack.SPC_Material_Type = item.SPC_Material_Type;//物品类型
                        pack.SPC_projectName = item.SPC_projectName;
                        db.Packing_SPC_Records.Add(pack);
                        int savecout = db.SaveChanges();
                        if (savecout < 0) arr.Add(item.MaterialNumber);
                    }
                }
                return Content(JsonConvert.SerializeObject(arr));
            }
            else
            {
                return Content("保存失败");
            }
        }
        #endregion
        #endregion               

        #region--- 删除照片、图纸方法
        [HttpPost]
        public ActionResult DeleteImg(string path, string MaterialNumber, string pictureType)//path(路径)、MaterialNumber（物料号）、pictureType（文件类型）
        {
            //1.检查路径的文件是否存在？存在就跳到第２步，不存在就返回提示信息
            //2.移动文件到指定目录，修改文件名(移动后的文件名和原文件夹的文件名)，返回提示信息
            var diff_materialNumber = MaterialNumber.Split('-');
            if (!String.IsNullOrEmpty(path))
            {
                string fileType = path.Substring(path.Length - 4, 4);//扩展名
                string old_path = @"D:" + path.Replace('/', '\\');//整个文件路径
                FileInfo path_file = new FileInfo(old_path);//建立文件对象
                int serial_N = Convert.ToInt16(path_file.Name.Split('_')[1].Split('.')[0]);//文件序号
                string new_path = @"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "_Delete_File\\";//新目录路径
                if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "_Delete_File\\") == false)　//目录是否存在
                {

                    Directory.CreateDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "_Delete_File\\");
                    FileInfo new_file = new FileInfo(new_path + MaterialNumber + "_1" + fileType);
                    path_file.CopyTo(new_file.FullName);//复制文件到新目录
                    path_file.Delete();//删除原文件
                    List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\");
                    int filecount = filesInfo.Where(c => c.Extension == fileType).Count();//文件夹里的文件个数
                    for (int i = serial_N; i < filecount + 1; i++)
                    {
                        string filepath = @"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\" + MaterialNumber + "_" + (i + 1) + fileType;
                        string newfilepath = @"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\" + MaterialNumber + "_" + i + fileType;
                        System.IO.File.Move(filepath, newfilepath);
                    }
                }
                else  //已有删除目录时，修改文件名
                {
                    List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "_Delete_File\\");
                    int count = filesInfo.Where(c => c.Extension == fileType).Count();
                    FileInfo new_file = new FileInfo(new_path + MaterialNumber + "_" + (count + 1) + fileType);
                    //int t = 2;
                    //while(new_file.Exists)
                    //{
                    //    new_file = new FileInfo(new_path + MaterialNumber + "_" + (count + t) + fileType);
                    //    t++;
                    //}
                    path_file.CopyTo(new_file.FullName);//复制文件到新目录
                    path_file.Delete();//删除原文件
                    List<FileInfo> picturefilesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\");
                    int filecount = picturefilesInfo.Where(c => c.Extension == fileType).Count();//文件夹里的文件个数
                    for (int i = serial_N; i < filecount + 1; i++)
                    {
                        string filepath = @"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\" + MaterialNumber + "_" + (i + 1) + fileType;
                        string newfilepath = @"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\" + MaterialNumber + "_" + i + fileType;
                        System.IO.File.Move(filepath, newfilepath);
                    }
                }
                return Content("删除成功");
            }
            else
            {
                return Content("没有路径");
            }
        }

        #endregion

        #region----上传照片、图纸方法      
        /// ------<UploadFile_Ingredients_summary>
        /// 1.方法的作用：上传图片、图纸。
        /// 2.方法的参数及用法：pictureFile（数组）  用法：将数组中的图片或图纸上传到服务器或本地。    pictureType（图片类型）、MaterialNumber（物料号） 用法：查询路径或创建路径。
        /// 3.方法具体逻辑顺序,判断条件：（1）将传进来的物料号分隔（2）判断文件请求个数是否大于0，（2.1）小于0直接返回false，（2.2）大于0，判断路径是否存在  （2.2.1）存在就判断传进来的
        /// pictureType（文件类型）类型是图片就跳进if分支，在使用pictureType，以及分隔后的MaterialNumber去判断路径是否存在，不存在就创建路径，存在就foreach数组（pictureFile），调用
        /// GetAllFilesInDirectory()方法查找文件夹中的文件个数，在判断当前遍历的文件后缀是.jpg还是.pdf,在跳进相应的if分支，追加文件命名并保存文件。（2.2.2）如（2.2.1）判断文件类型是图纸，那么就
        /// 跳进else分支，在使用pictureType，以及分隔后的MaterialNumber去判断路径是否存在，不存在就创建路径，存在就foreach数组（pictureFile），调用GetAllFilesInDirectory()方法查找文件夹中的文件个数，
        /// 在一次判断当前遍历的文件后缀是.jpg还是.pdf,在跳进相应的if分支，追加文件命名并保存文件。
        /// 4.方法（可能）有的结果：结果一：当Request.Files.Count 小于零时，直接返回“false”   结果二：当 Request.Files.Count大于零时，直接返回“true”。
        /// ------</UploadFile_Ingredients_summary>
        [HttpPost]
        public bool UploadFile_Ingredients(List<string> pictureFile, string pictureType, string MaterialNumber)
        {
            var diff_materialNumber = MaterialNumber.Split('-');//将物料号分隔
            if (Request.Files.Count > 0)
            {
                if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\") == false)//判断总路径是否存在
                {
                    Directory.CreateDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\");//创建总路径
                };
                if (pictureType == "Picture")//判断是文件类型是图片
                {
                    if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\") == false)//判断图片路径是否存在
                    {
                        Directory.CreateDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\");//创建图片路径
                    }
                    foreach (var item in pictureFile)
                    {
                        HttpPostedFileBase file = Request.Files["UploadFile_Ingredients" + pictureFile.IndexOf(item)];
                        var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();
                        List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\");//遍历文件夹中的个数
                        if (fileType == ".jpg")//判断文件后缀
                        {
                            int jpg_count = filesInfo.Where(c => c.Name.StartsWith(MaterialNumber + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                            file.SaveAs(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\" + MaterialNumber + "_" + (jpg_count + 1) + fileType);//文件追加命名
                        }
                        else if (fileType == ".pdf")
                        {
                            int pdf_count = filesInfo.Where(c => c.Name.StartsWith(MaterialNumber + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").Count();
                            file.SaveAs(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\" + MaterialNumber + "_" + (pdf_count + 1) + fileType);//文件追加命名
                        }
                    }
                }
                else//判断文件类型是图纸
                {
                    if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\") == false)//判断图纸总路径是否存在
                    {
                        Directory.CreateDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\");//创建图纸路径
                    }
                    foreach (var i in pictureFile)
                    {
                        HttpPostedFileBase file = Request.Files["UploadFile_Ingredients" + pictureFile.IndexOf(i)];
                        var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();
                        List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\");//遍历文件个数
                        if (fileType == ".jpg")//判断后缀
                        {
                            int jpg_count = filesInfo.Where(c => c.Name.StartsWith(MaterialNumber + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                            file.SaveAs(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\" + MaterialNumber + "_" + (jpg_count + 1) + fileType);//文件追加命名
                        }
                        else if (fileType == ".pdf")
                        {
                            int pdf_count = filesInfo.Where(c => c.Name.StartsWith(MaterialNumber + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").Count();
                            file.SaveAs(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + MaterialNumber + "\\" + pictureType + "\\" + MaterialNumber + "_" + (pdf_count + 1) + fileType);//文件追加命名
                        }
                    }
                }
                return true;
            }
            return false;
        }
        #endregion

        #region 基本信息查询页获取物料编号列表 
        /// ------<GetMaterialNumber_summary>
        /// 1.方法的作用：获取表中所有物料号信息
        /// 2.方法的参数及用法：无
        /// 3.方法具体逻辑顺序,判断条件：找出Packing_SPC_Records表中所有物料号，去重后会重新按MaterialNumber升序排序。
        /// 4.方法（可能）有的结果：将找出来升序排序后的物料号输出。
        /// ------</GetMaterialNumber_summary>
        public ActionResult GetMaterialNumber()
        {
            var orders = db.Packing_SPC_Records.Where(c => c.MaterialNumber != null).ToList();//增加.Distinct()后会重新按MaterialNumber升序排序
            var result1 = orders.Select(c => c.MaterialNumber).Distinct().ToList();
            JArray result = new JArray();
            foreach (var item in result1)
            {
                JObject List = new JObject();
                List.Add("value", item);
                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 拆分记录
        /// ------<SPC_DataFractionation_summary>
        /// 1.方法的作用：拆分数据库中指定记录
        /// 2.方法具体逻辑顺序,判断条件：使用id找出记录，判断传进来的两个数量之和是否等于数据库中的数量，等于则进行记录拆分，修改使用id找出来的记录的数量并保存，
        /// 同时使用id找出来的记录new一条新记录，修改数量并保存。如果两数之和与数据库中的记录不等，输出提示
        /// ------</SPC_DataFractionation_summary>
        public ActionResult SPC_DataFractionation(Decimal quantityOne, Decimal quantityTwo, int id)
        {
            if (quantityOne != 0 && quantityTwo != 0)
            {
                var sum = quantityOne + quantityTwo;
                var result = db.Packing_SPC_Records.Where(c => c.Id == id).ToList();
                var total = result.FirstOrDefault().Quantity;
                int count = 0;
                int count1 = 0;
                if (sum == total)
                {
                    result.FirstOrDefault().Quantity = quantityOne;
                    result.FirstOrDefault().Split_Operator = ((Users)Session["User"]).UserName;//拆分人
                    result.FirstOrDefault().Split_Date = DateTime.Now;//拆分时间
                    count = count + db.SaveChanges();
                    Packing_SPC_Records newList = new Packing_SPC_Records();
                    List<Packing_SPC_Records> list = new List<Packing_SPC_Records>();
                    //箱号、物品分类、物料描述、logo、备注、表序
                    newList.OrderNum = result.FirstOrDefault().OrderNum;
                    newList.SPC_OuterBoxBarcode = result.FirstOrDefault().SPC_OuterBoxBarcode;
                    newList.Material_Name = result.FirstOrDefault().Material_Name;
                    newList.Specification_Description = result.FirstOrDefault().Specification_Description;
                    newList.MaterialNumber = result.FirstOrDefault().MaterialNumber;
                    newList.Quantity = quantityTwo;
                    newList.SPC_Material_Type = result.FirstOrDefault().SPC_Material_Type;
                    newList.SPC_Label_Operator = result.FirstOrDefault().SPC_Label_Operator;
                    newList.SPC_Label_Date = result.FirstOrDefault().SPC_Label_Date;
                    newList.SPC_Material_Confim = result.FirstOrDefault().SPC_Material_Confim;
                    newList.SPC_Material_Confim_Operator = result.FirstOrDefault().SPC_Material_Confim_Operator;
                    newList.SPC_Material_Confim_Date = result.FirstOrDefault().SPC_Material_Confim_Date;
                    newList.SPC_OuterBox_Type = result.FirstOrDefault().SPC_OuterBox_Type;
                    newList.ScreenNum = result.FirstOrDefault().ScreenNum;
                    newList.Batch = result.FirstOrDefault().Batch;
                    newList.G_Weight = result.FirstOrDefault().G_Weight;
                    newList.N_Weight = result.FirstOrDefault().N_Weight;
                    newList.SNTN = result.FirstOrDefault().SNTN;
                    newList.SPC_Packaging_Operator = result.FirstOrDefault().SPC_Packaging_Operator;
                    newList.SPC_Packaging_Date = result.FirstOrDefault().SPC_Packaging_Date;
                    newList.Unit = result.FirstOrDefault().Unit;
                    newList.QC_Department = result.FirstOrDefault().QC_Department;
                    newList.QC_Group = result.FirstOrDefault().QC_Group;
                    newList.Pa_Department = result.FirstOrDefault().Pa_Department;
                    newList.Pa_QCGroup = result.FirstOrDefault().Pa_QCGroup;
                    newList.ListingImport_Operator = result.FirstOrDefault().ListingImport_Operator;
                    newList.ListingImport_Date = result.FirstOrDefault().ListingImport_Date;
                    newList.Print_OutsideBox_Operator = result.FirstOrDefault().Print_OutsideBox_Operator;
                    newList.Print_OutsideBox_Date = result.FirstOrDefault().Print_OutsideBox_Date;
                    newList.Split_Operator = ((Users)Session["User"]).UserName;//拆分人
                    newList.Split_Date = DateTime.Now;//拆分时间
                    list.Add(newList);
                    db.Packing_SPC_Records.AddRange(list);
                    count1 = count1 + db.SaveChanges();
                    if (count == 1 && count1 == 1) return Content("拆分成功！");
                    else return Content("拆分失败！");
                }
                else
                {
                    return Content("两数之和与原来的数量不相等！");

                }
            }
            return Content("");
        }
        #endregion

        #region---单个物料增加
        public ActionResult SPC_SingleMaterialAddition(List<Packing_SPC_Records> newList)
        {
            if (newList.Count != 0)
            {
                string username = ((Users)Session["User"]).UserName;//登录人
                var message = db.Personnel_Roster.Where(c => c.Name == username).FirstOrDefault();//去花名册找登录人所在部门
                string address = "";
                foreach (var item in newList)
                {
                    string content = "";
                    if (!String.IsNullOrEmpty(item.MaterialNumber))
                    {
                        content = "订单号：" + item.OrderNum + "，屏序为：" + item.ScreenNum + "，批次为：" + item.Batch + "物料类型为：" + item.SPC_Material_Type + "的清单增加了一个物料，物料号为：" + item.MaterialNumber;
                    }
                    else
                    {
                        content = "订单号：" + item.OrderNum + "，屏序为：" + item.ScreenNum + "，批次为：" + item.Batch + "物料类型为：" + item.SPC_Material_Type + "的清单增加了一个物料，物料名称为：" + item.Material_Name;
                    }
                    Packing_SPC_Records recor = new Packing_SPC_Records();
                    recor.ListingImport_Operator = ((Users)Session["User"]).UserName;//添加人
                    recor.ListingImport_Date = DateTime.Now;//添加时间
                    recor.OrderNum = item.OrderNum;
                    recor.MaterialNumber = item.MaterialNumber;
                    recor.Specification_Description = item.Specification_Description;//规格描述
                    recor.Quantity = item.Quantity;//数量
                    recor.Material_Name = item.Material_Name;//物品名称
                    recor.Unit = item.Unit;//单位
                    recor.Batch = item.Batch; //批次
                    recor.ScreenNum = item.ScreenNum;//屏序
                    recor.SPC_Material_Type = item.SPC_Material_Type;//物品类型
                    db.Packing_SPC_Records.Add(recor);
                    int savecout = db.SaveChanges();
                    if (savecout > 0)
                    {
                        if (message.Department == "总装一部")
                        {
                            var adress_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件发送老化包装一部").ToList();//找出发送人名单
                            var copy_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件抄送老化包装一部").ToList();//找出发送人名单 
                            var sender = db.UserItemEmail.Where(c => c.ProcesName == "备品配件一部发送人").FirstOrDefault();
                            var result = commom.SendEmail(sender.EmailAddress, adress_list.Select(c => c.EmailAddress).ToList(), content, "备品配件单个物料添加通知", copy_list.Select(c => c.EmailAddress).ToList(), address);
                            if (result == true) return Content("添加成功，邮件发送成功！");
                            else return Content("添加成功，邮件发送失败！");
                        }
                        else if (message.Department == "总装二部")
                        {
                            var adress_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件发送老化包装二部").ToList();//找出发送人名单
                            var copy_list = db.UserItemEmail.Where(c => c.ProcesName == "备品配件抄送老化包装二部").ToList();//找出抄送人名单
                            var sender = db.UserItemEmail.Where(c => c.ProcesName == "备品配件二部发送人").FirstOrDefault();
                            var result = commom.SendEmail(sender.EmailAddress, adress_list.Select(c => c.EmailAddress).ToList(), content, "备品配件单个物料删除通知", copy_list.Select(c => c.EmailAddress).ToList(), address);
                            if (result == true) return Content("添加成功，邮件发送成功！");//判断邮件是否发送成功
                            else return Content("添加成功，邮件发送失败！");
                        }
                        else
                        {
                            return Content("添加成功，未发送邮件！");
                        }
                    }
                    else return Content("添加失败！");
                }
            }
            return View();
        }
        #endregion

        #region---增加外箱号
        [HttpPost]
        public ActionResult SPC_SaveOutsideBox(List<Packing_SPC_Records> newList)
        {
            if (newList.Count != 0)
            {
                int count = 0;
                foreach (var i in newList)
                {
                    var original = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == i.SPC_OuterBoxBarcode && c.SPC_OuterBox_Type != null).FirstOrDefault();
                    var list = db.Packing_SPC_Records.Where(c => c.Id == i.Id).FirstOrDefault();
                    if (list.SPC_OuterBoxBarcode == i.SPC_OuterBoxBarcode)
                    {//传进来的箱号数据库中的相等就不进行保存动作

                    }
                    else
                    {
                        int str = Convert.ToInt32((i.SPC_OuterBoxBarcode.Split('B'))[1]);

                        list.SNTN = str.ToString();
                        list.SPC_OuterBoxBarcode = i.SPC_OuterBoxBarcode;
                        list.SPC_Packaging_Operator = ((Users)Session["User"]).UserName;//检验人
                        list.SPC_Packaging_Date = DateTime.Now;//检验时间

                        list.SPC_OuterBox_Type = original.SPC_OuterBox_Type == null ? null : original.SPC_OuterBox_Type;
                        list.SPC_OutsideBoxLanguage = original.SPC_OutsideBoxLanguage == null ? null : original.SPC_OutsideBoxLanguage;
                        list.IsLogo = original.IsLogo == null ? null : original.IsLogo;
                        list.G_Weight = original.G_Weight == null ? null : original.G_Weight;
                        list.N_Weight = original.N_Weight == null ? null : original.N_Weight;
                        count = count + db.SaveChanges();
                    }
                }
                if (count > 0) return Content("True");
                else return Content("false");
            }
            else return Content("数据为空!");
        }
        #endregion

        #endregion

        #region---2.物料基本信息查询页
        /// ------<SPC_Display_summary>
        /// 1.方法的作用：物料基本信息查询。
        /// 2.方法的参数及用法：materialNumber(物料号)、productName(物料名)  用法：用于当做查询条件。
        /// 3.方法具体逻辑顺序,判断条件：(1)找出表中所有记录，在找出传进来的物料名的记录（2）foreach第一步找出来的记录，分隔物料编号，判断（Picture）图片路径是否存在，若存在，就直接调用GetAllFilesInDirectory()
        /// 方法遍历文件夹中的图片（3）判断第二步遍历出来的dir_files是否等于null，如果不等null,就循环dir_files这个数组，在判断当前遍历的图片后缀是否为jpg,是就将图片路径add进picturejpg_list数组，否则add进
        /// picturepdf_list数组。（4）判断（Draw）图纸路径是否存在，若存在，就直接调用GetAllFilesInDirectory() 方法遍历文件夹中的图纸（5）判断第四步遍历出来的draw_files是否等于null，如果不等null,
        /// 就循环draw_files这个数组，在判断当前遍历的图纸后缀是否为jpg,是就将图片路径add进drawjpg_list数组，否则add进drawpdf_list数组。
        /// 4.方法（可能）有的结果：将查找出来的结果输出（有的物料号没有上传图片或者图纸，就找不到该物料号对应的图片或图纸）。
        /// ------</SPC_Display_summary> 
        [HttpPost]
        public ActionResult SPC_Display(string materialNumber, string productName)
        {
            JArray totalresult = new JArray();

            var recordlist = CommonERPDB.ERP_MaterialQuery_SPC(materialNumber, productName).Select(c => new { MaterialNumber = c.ima01, Material_Name = c.ima02, Specification_Description = c.ima021 }).ToList();
            foreach (var item in recordlist)//循环找出来的记录
            {
                JObject result = new JObject();
                result.Add("Specifications", item.Specification_Description);//规格描述
                result.Add("Material_Name", item.Material_Name);//物料名称
                result.Add("MaterialNumber", item.MaterialNumber);//物料编号
                //result.Add("Id", item.Id);
                JObject pictureAddress_list = new JObject();
                JObject drawFiles_list = new JObject();
                List<FileInfo> dir_files = null;
                var diff_materialNumber = item.MaterialNumber.Split('-');//分隔物料号
                if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + item.MaterialNumber + "\\Picture\\"))//判断图片的总路径是否存在
                {
                    dir_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + item.MaterialNumber + "\\Picture\\");//遍历图片文件夹的个数
                }
                if (dir_files != null)//判断上一步遍历出来的数组是否为空
                {
                    JArray picturejpg_list = new JArray();//用于存放后缀为.jpg图片的数组
                    JArray picturepdf_list = new JArray();//用于存放后缀为.pdf图片的数组
                    foreach (var i in dir_files)
                    {
                        string path = @"/MES_Data/SpareParts/" + diff_materialNumber[0] + "/" + item.MaterialNumber + "/" + "Picture" + "/" + i;//组合出路径
                        var filetype = path.Split('.');//将组合出来的路径以点分隔，方便下一步判断后缀
                        if (filetype[1] == "jpg")//后缀为jpg
                        {
                            picturejpg_list.Add(path);
                        }
                        else //后缀为其他
                        {
                            picturepdf_list.Add(path);
                        }
                    }
                    pictureAddress_list.Add("picturejpg_list", picturejpg_list);
                    pictureAddress_list.Add("picturepdf_list", picturepdf_list);
                }
                List<FileInfo> draw_files = null;
                if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + item.MaterialNumber + "\\Draw\\"))//判断图纸的总路径是否存在
                {
                    draw_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + item.MaterialNumber + "\\Draw\\");//遍历图纸文件夹的文件个数
                }
                if (draw_files != null)
                {
                    JArray drawjpg_list = new JArray();//用于存放后缀为.jpg图纸的数组
                    JArray drawpdf_list = new JArray();//用于存放后缀为.pdf图纸的数组
                    foreach (var i in draw_files)
                    {
                        string path1 = @"/MES_Data/SpareParts/" + diff_materialNumber[0] + "/" + item.MaterialNumber + "/" + "Draw" + "/" + i;//组合出路径
                        var filetype = path1.Split('.');// 将组合出来的路径以点分隔，方便下一步判断后缀
                        if (filetype[1] == "jpg")//判断文件后缀
                        {
                            drawjpg_list.Add(path1);
                        }
                        else
                        {
                            drawpdf_list.Add(path1);
                        }
                    }
                    drawFiles_list.Add("drawjpg_list", drawjpg_list);
                    drawFiles_list.Add("drawpdf_list", drawpdf_list);
                }
                result.Add("draw", drawFiles_list);
                result.Add("picture", pictureAddress_list);
                totalresult.Add(result);
            }
            return Content(JsonConvert.SerializeObject(totalresult));
        }
        #endregion

        #region----3.备品配件检验页
        public ActionResult SPC_Packaging()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "SPC_Packaging" });
            }
            ViewBag.OrderList = GetOrderList();
            return View();
        }

        #region--保存检验信息
        [HttpPost]
        public ActionResult SPC_Packaging(int id, string orderNumber, string spc_OuterBoxBarcode, string materialNumber, Decimal quantity, int screenNum, int batch, string sntn, string spc_OuterBox_Type, string spc_Description, string materialType, string Department, string Group)
        {
            int count = 0;
            if (materialNumber == "无")
            {
                var result = db.Packing_SPC_Records.FirstOrDefault(c => c.Id == id);
                result.SPC_Packaging_Operator = ((Users)Session["User"]).UserName;//检验人
                result.SPC_Packaging_Date = DateTime.Now;//检验时间
                result.SPC_OuterBoxBarcode = spc_OuterBoxBarcode;//外箱号  
                result.SNTN = sntn;//件号/数
                result.SPC_OuterBox_Type = spc_OuterBox_Type;//外箱类型   
                result.QC_Department = Department;//部门
                result.QC_Group = Group;//班组
                count = count + db.SaveChanges();
                if (count > 0) return Content("保存成功！");
                else return Content("false");
            }
            else//物料号不为空，判断数量是否相同，相同就把规格描述传进来，否则规格描述传null
            {
                if (spc_Description == null)
                {
                    var record = db.Packing_SPC_Records.FirstOrDefault(c => c.OrderNum == orderNumber && c.MaterialNumber == materialNumber && c.Quantity == quantity && c.Batch == batch &&
                    c.ScreenNum == screenNum && c.SPC_Material_Type == materialType && c.SPC_OuterBoxBarcode == null);
                    if (record != null)
                    {
                        record.SPC_Packaging_Operator = ((Users)Session["User"]).UserName;//检验人
                        record.SPC_Packaging_Date = DateTime.Now;//检验时间
                        record.SPC_OuterBoxBarcode = spc_OuterBoxBarcode;//外箱号  
                        record.SNTN = sntn;//件号/数
                        record.SPC_OuterBox_Type = spc_OuterBox_Type;//外箱类型 
                        record.QC_Department = Department;
                        record.QC_Group = Group;//班组
                        int num = db.SaveChanges();
                        if (num > 0) return Content("检验成功！");
                        else return Content("检验失败！");
                    }
                    else
                    {
                        return Content("未找到对应记录！");
                    }
                }
                else
                {
                    var record = db.Packing_SPC_Records.FirstOrDefault(c => c.OrderNum == orderNumber && c.MaterialNumber == materialNumber && c.Quantity == quantity &&
                    c.Batch == batch && c.ScreenNum == screenNum && c.Specification_Description == spc_Description && c.SPC_Material_Type == materialType && c.SPC_Packaging_Operator == null);
                    if (record != null)
                    {
                        record.SPC_Packaging_Operator = ((Users)Session["User"]).UserName;//检验人
                        record.SPC_Packaging_Date = DateTime.Now;//检验时间
                        record.SPC_OuterBoxBarcode = spc_OuterBoxBarcode;//外箱号  
                        record.SNTN = sntn;//件号/数
                        record.SPC_OuterBox_Type = spc_OuterBox_Type;//外箱类型 
                        record.QC_Department = Department;
                        record.QC_Group = Group;//班组
                        int num = db.SaveChanges();
                        if (num > 0) return Content("检验成功！");
                        else return Content("检验失败！");
                    }
                    else
                    {
                        return Content("未找到对应记录！");
                    }
                }
            }
        }
        #endregion

        #region-- 根据订单号拿物料号
        /// ------<GetMaterialNumberByOrderNumber_summary>
        /// 1.方法的作用：根据订单号拿物料号。
        /// 2.方法的参数及用法：ordernumber(订单号)  用法：用于当做查询条件。
        /// 3.方法具体逻辑顺序,判断条件：根据前端传入的订单号查询该订单号所对应的物料号以及数量，再将结果输出。
        /// 4.方法（可能）有的结果：将查询出来的物料号、数量输出。
        /// ------</GetMaterialNumberByOrderNumber_summary>
        [HttpPost]
        public ActionResult GetMaterialNumberByOrderNumber(string ordernumber)
        {
            var recordlist = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernumber).Select(c => new { c.MaterialNumber, c.Quantity }).ToList();//查询订单号对应的物料及数量
            return Content(JsonConvert.SerializeObject(recordlist));
        }
        #endregion

        #region--根据物料号拿对应数量
        /// ------<GetMaterialNumberByQuantity_summary>
        /// 1.方法的作用：根据物料号拿对应数量
        /// 2.方法的参数及用法：ordernumber(订单号),materialNumber(物料号)   用法：用于当做查询条件。
        /// 3.方法具体逻辑顺序,判断条件：(1).使用传进来的materialNumber(物料号)找出该物料号对应的订单号  (2).在判断找出的订单号是否等于前端传进来的订单号，如不相等直接将第一步（1）找出来的
        /// 订单号输出，提示“此物料号对应的订单号”。如找出来的订单号与传进来的订单号相等，那么使用订单号与物料号找出对应的数量。
        /// 4.方法（可能）有的结果：结果一：输出订单号、物料号所所对应的的数量。  结果二：使用物料号找出来的订单号与传进来的订单号不相等，则输出该物料号所对应的订单号提示。
        /// ------</GetMaterialNumberByQuantity_summary>
        public ActionResult GetMaterialNumberByQuantity(string ordernumber, string materialNumber, int batch, int screenNum, string materialType)
        {
            if (materialNumber != "无")
            {
                JObject repat = new JObject();
                var recordlist = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernumber && c.MaterialNumber == materialNumber && c.ScreenNum == screenNum && c.Batch == batch && c.SPC_Material_Type == materialType).Select(c => c.Quantity).ToList();//使用订单号，物料号找出对应的数量
                if (recordlist.Count > 0)
                {
                    //判断是否已备料
                    var conList = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernumber && c.MaterialNumber == materialNumber && c.ScreenNum == screenNum && c.Batch == batch && c.SPC_OuterBoxBarcode == null && c.SPC_Material_Type == materialType).ToList();
                    if (conList.Count > 0)
                    {
                        //有记录，判断该物料号对应的记录有几条，有两条就返回对应的物料描述，如果有一条，则不返回对应的描述信息
                        if (conList.Count > 1)//对条记录，返回数量、规格描述
                        {
                            var result = conList.Select(c => new { c.Quantity, c.Specification_Description }).ToList();
                            return Content(JsonConvert.SerializeObject(result));
                        }
                        else//只有一条记录，直接返回数量l
                        {
                            var result = conList.Select(c => c.Quantity).ToList();
                            return Content(JsonConvert.SerializeObject(result));//返回未备料的记录
                        }
                    }
                    else
                    {
                        return Content("此物料已检验！");
                    }
                }
                else
                {
                    return Content("没找到物料号对应的数量！");
                }
            }
            else
            {
                var resultList = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernumber && c.MaterialNumber == null && c.Batch == batch && c.ScreenNum == screenNum && c.SPC_Material_Type == materialType && c.SPC_OuterBoxBarcode == null).ToList();
                if (resultList.Count > 0)
                {
                    var result = resultList.Select(c => new { c.Material_Name, c.Quantity, c.Id });
                    return Content(JsonConvert.SerializeObject(result));
                }
                else
                {
                    return Content("找不到没有物料号的记录");
                }
            }

        }
        #endregion

        #region--生成外箱条码号
        /// -------<createOutBoxNum_summary>
        /// 1.方法的作用：生成外箱条码号。
        /// 2.方法的参数及用法：orderNumber(订单号)、screenNum(屏序号)   用法：查询条件。
        /// 3.方法具体逻辑顺序,判断条件：(1)判断传进来的订单号是否为空，（2）当订单号不为空时，使用orderNumber（订单号）、screenNum（屏序号）查找出该订单号所有外箱号不等于null的个数并去重，
        ///  (2.1)将订单号以“-”分隔，（2.2）判断前端传入的屏序号的长度是否小于2，小于就在屏序号前面补零，否则直接使用。（2.3）拼接成外箱条码号。 （3）订单号为空时，直接输出“创建外箱号出错！”。
        /// 4.方法（可能）有的结果：结果一:前端传进来的orderNumber(订单号)为空，输出“创建外箱号出错！”  结果二：传进来的外箱号不为空时，将组织好的外箱号输出。
        /// ------</createOutBoxNum_summary>
        public ActionResult createOutBoxNum(string orderNumber, int screenNum, int batch, string outsideBox)
        {
            JObject info = new JObject();
            if (!String.IsNullOrEmpty(orderNumber))
            {
                #region--原来自动生成外箱号方法
                //找出已装箱总数
                //var count = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == screenNum && c.SPC_OuterBoxBarcode != null&&c.Batch==batch).Select(c => c.SPC_OuterBoxBarcode).Distinct().ToList().Count();
                //找出已装箱的
                //var boxed = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == screenNum && c.SPC_OuterBoxBarcode != null && c.Batch == batch).Select(c => c.SPC_OuterBoxBarcode).Distinct().ToList();
                //int num = 0;
                //int i = 1;
                //foreach (var item in boxed)
                //{
                //    string[] end = item.Split('B');
                //    if (end[1].StartsWith("0"))//判断开始位是0，取最后一位判断
                //    {
                //        var last = end[1].Substring(end[1].Length - 1, 1);//截取最后一位，方便下一步判断
                //        if (i.ToString() == last)
                //        { }
                //        else
                //        {
                //            num = i;
                //            break;
                //        }
                //    }
                //    else //开始位不是0，直接取两位
                //    {
                //        if (i.ToString() == end[1])
                //        { }
                //        else
                //        {
                //            num = i;
                //        }
                //    }
                //    i++;
                //}
                //if (num == 0)
                //{
                //    num = i++;
                //}
                //string[] str = orderNumber.Split('-');//以—分隔订单号  2019-YA736-1
                //string start = str[0].Substring(2);//取最后两位
                //string str1 = "";
                //if (screenNum.ToString().Length < 2)//判断传进来的屏序号是否小于两位
                //{
                //    str1 = screenNum.ToString().PadLeft(2, '0');//小于2两位，直接在屏序号前补领
                //}
                //else//直接使用传进来的屏序号
                //{
                //    str1 = screenNum.ToString();
                //}
                ////判断传进来的批次是否小于两位，小于两位补零
                //string Batch = "";
                //if (batch.ToString().Length < 2)
                //{
                //    Batch = batch.ToString().PadLeft(2, '0');
                //}
                //else
                //{
                //    Batch = batch.ToString();
                //}
                //string OuterBoxBarCodeNum = start + str[1] + "-" + str[2] + '-' + Batch + "-" + str1 + "-B" + outsideBox.ToString().PadLeft(2, '0');//组合出外箱条码号
                //info.Add("OuterBoxBarCodeNum", OuterBoxBarCodeNum);
                //return Content(JsonConvert.SerializeObject(info));
                #endregion
                JObject res = new JObject();
                string[] str = orderNumber.Split('-');//以—分隔订单号  2019-YA736-1
                string start = str[0].Substring(2);//取最后两位
                string str1 = "";
                if (screenNum.ToString().Length < 2)//判断传进来的屏序号是否小于两位
                {
                    str1 = screenNum.ToString().PadLeft(2, '0');//小于2两位，直接在屏序号前补领
                }
                else//直接使用传进来的屏序号
                {
                    str1 = screenNum.ToString();
                }
                //判断传进来的批次是否小于两位，小于两位补零
                string Batch = "";
                if (batch.ToString().Length < 2)
                {
                    Batch = batch.ToString().PadLeft(2, '0');
                }
                else
                {
                    Batch = batch.ToString();
                }
                string OuterBoxBarCodeNum = start + str[1] + "-" + str[2] + '-' + Batch + "-" + str1 + "-B" + outsideBox.ToString().PadLeft(2, '0');//组合出外箱条码号
                if (!String.IsNullOrEmpty(OuterBoxBarCodeNum))
                {
                    var content = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == OuterBoxBarCodeNum).Count();
                    if (content > 0)
                    {
                        var type = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == OuterBoxBarCodeNum).FirstOrDefault().SPC_OuterBox_Type;
                        res.Add("res", "外箱号已存在！");
                        res.Add("OuterBoxBarCodeNum", OuterBoxBarCodeNum);
                        res.Add("type", type);
                    }
                    else
                    {
                        res.Add("res", "外箱号不存在！");
                        res.Add("OuterBoxBarCodeNum", OuterBoxBarCodeNum);
                        res.Add("type", "");
                    }
                }
                return Content(JsonConvert.SerializeObject(res));
            }
            else return Content("创建外箱号出错!");
        }
        #endregion

        #region--备品、配件物料标签打印
        public ActionResult SPC_MaterialLablePrint()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "SPC_Packaging" });
            }
            return View();
        }

        #region --根据订单号、物料号输出物料标签图片  
        /// ------<Outspc_MaterialLableToImg_summary>
        /// 1.方法的作用：根据传入的订单号、物料号输出物料标签图片
        /// 2.方法的参数及用法：orderNum（订单号）、materialNumber（物料号）  用法：用于当做查询条件。
        /// 3.方法具体逻辑顺序,判断条件：（1）判断传进来的orderNum（订单号）、materialNumber（物料号）是否为空，(2)当两者都不为空时，（2.1）使用订单号、物料号找出对应的记录{包括：ordernum（订单号）、
        ///   materialNum（物料号）、spc_material_type（物料类型）、material_name（物料名称）、specifications（规格描述）、quantity（数量）}  （2.2）调用SPC_MaterialLable()方法，将参数传进去，组合出
        ///   图片，将图片输出。 （3）当两者都为空时，输出提示“订单号或物料号不能为空!”。
        /// 4.方法（可能）有的结果：结果一：当传进来的订单号、物料号都为空时，直接输出提示“订单号或物料号不能为空!”   结果二：两者都不为空时，将组织号的图片输出。
        /// ------</Outspc_MaterialLableToImg_summary>
        public ActionResult Outspc_MaterialLableToImg(string orderNum, string materialNumber, Decimal quantity, int batch, int screenNum, string Language, int id, string materialType)
        {
            if (!String.IsNullOrEmpty(orderNum) && !String.IsNullOrEmpty(materialNumber))
            {
                var list = db.Packing_SPC_Records.Where(c => c.Id == id).FirstOrDefault();//找出订单号、物料号对应的整条记录
                var AllBitmap = SPC_MaterialLable(list.OrderNum, list.MaterialNumber, materialType, list.Material_Name, list.Specification_Description, list.Quantity, Language, list.Unit); //调用SPC_MaterialLable()方法，绘制出物料标签图片
                MemoryStream ms = new MemoryStream();
                AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                AllBitmap.Dispose();
                return File(ms.ToArray(), "image/Png");
            }
            return Content("订单号或物料号不能为空!");
        }

        /// ------<Outspc_MaterialLableAgain_summary>
        /// 1.方法的作用：打印物料标签图片
        /// 2.方法的参数及用法：orderNum（订单号）、materialNumber（物料号） 用法：用于当做查询条件。   IP（IP地址）、port（端口）  用法：用于连接打印机。
        /// 3.方法具体逻辑顺序,判断条件：（1）使用传进来的订单号、物料号查询出记录{包括：ordernum（订单号）、materialNum（物料号）、spc_material_type（物料类型）、material_name（物品名称）、
        /// specifications（规格描述）、quantity（数量）} （2）调用SPC_MaterialLable() 方法，将第一步找出来的参数传进去，组合出图片，调用BmpToZpl()方法将位图转ZPL指令,最后调用IPPrint()方法，
        /// 连接IP和端口打印内容。
        /// 4.方法（可能）有的结果：调用IPPrint()方法，连接IP和端口打印内容（1）"打印成功！"  （2）"打印连接失败,请检查打印机是否断网或未开机！";
        /// ------</Outspc_MaterialLableAgain_summary>
        public ActionResult Outspc_MaterialLableAgain(string Language, List<int> idlist, string Department, string Group, string ip = "", int port = 0, int concentration = 5)
        {
            int printsuccesscount = 0;
            foreach (var id in idlist)
            {
                //从表中找出记录
                var record = db.Packing_SPC_Records.Where(c => c.Id == id).FirstOrDefault();
                var bm = SPC_MaterialLable(record.OrderNum, record.MaterialNumber, record.SPC_Material_Type, record.Material_Name, record.Specification_Description, record.Quantity, Language, record.Unit);//调用SPC_MaterialLable()方法，绘制出物料标签图片
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);//调用BmpToZpl()方法将位图转ZPL指令
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
                Thread.CurrentThread.Join(new TimeSpan(0, 0, 0, 0, 800));
                var res = ZebraUnity.IPPrint(data.ToString(), 1, ip, port);
                if (res == "打印成功！")
                {
                    //将打印人员打印时间保存到数据库                
                    printsuccesscount++;
                    record.SPC_Label_Operator = ((Users)Session["User"]).UserName;//登录人
                    record.SPC_Label_Date = DateTime.Now;//现在时间
                    record.Pa_Department = Department;//部门
                    record.Pa_QCGroup = Group;//班组
                    db.SaveChanges();
                }
            }
            string result = (printsuccesscount > 0 ? printsuccesscount + "个打印成功!" : "") + ((idlist.Count - printsuccesscount) > 0 ? (idlist.Count - printsuccesscount) + "个打印失败！" : "");
            return Content(result);
        }
        #endregion

        #region--生成备品配件物料标签图片
        /// <summary>
        /// 1.方法的作用：生成备品、配件物料标签图片
        /// 2.方法的参数及用法： ordernum(订单号),materialNumber(物料号),spc_material_type（物料类型）,material_name（物料名）,specifications（规格描述）,quantity(数量)。用法：用于组合出物料标签图片。
        /// 3.方法具体逻辑顺序,判断条件：（1）定义整个物料标签的宽和高、定义好笔的大小，根据需求绘制出整个物料标签的模板，（2）在对应的位置引入相对应的参数，并设置字体大小{参数包括：ordernum(订单号),
        /// materialNumber(物料号),spc_material_type（物料类型）,material_name（物料名）,specifications（规格描述）,quantity(数量)}，（3）最后调用ConvertTo1Bpp1()与ToGray()方法调整图像灰度、 灰度反
        /// 转以及二值化处理（图形转二值）。
        /// 4.方法（可能）有的结果：将图形转二值结果输出。
        /// </summary>
        public Bitmap SPC_MaterialLable(string orderNumber, string materialNumber, string materialType, string material_name, string specifications, Decimal Quantity, string Language, string unit)
        {
            //开始绘制物料标签
            int initialWidth = 750, initialHeihgt = 583;
            Bitmap AllBitmap = new Bitmap(initialWidth, initialHeihgt);
            Graphics theGraphics = Graphics.FromImage(AllBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            Pen pen = new Pen(Color.Black, 3);//定义笔的大小
            theGraphics.DrawRectangle(pen, 50, 50, 650, 483); //650矩形的宽度 //483矩形的高度
            //画横线
            theGraphics.DrawLine(pen, 50, 105, 700, 105);
            theGraphics.DrawLine(pen, 50, 167, 700, 167);
            theGraphics.DrawLine(pen, 50, 229, 700, 229);
            theGraphics.DrawLine(pen, 50, 320, 700, 320);
            theGraphics.DrawLine(pen, 50, 473, 700, 473);
            //画竖线
            theGraphics.DrawLine(pen, 205, 105, 205, 532);
            //引入类型  
            //string spcType = "备品、配件";
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center; //居中           
            Rectangle spcTypeRectangle = new Rectangle(50, 65, 650, 50);
            System.Drawing.Font myFont_spcType;
            myFont_spcType = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            if (Language == "中文")
            {
                theGraphics.DrawString(materialType, myFont_spcType, bush, spcTypeRectangle, format);
            }
            else
            {
                if (materialType == "备品")
                {
                    theGraphics.DrawString("Spare Parts", myFont_spcType, bush, spcTypeRectangle, format);//备品
                }
                else
                {
                    theGraphics.DrawString("Accessories", myFont_spcType, bush, spcTypeRectangle, format);//配件
                }
            }

            //引入订单号文字           
            Rectangle orderRectangle = new Rectangle(50, 125, 155, 105);//文本矩形
            System.Drawing.Font myFont_order;
            myFont_order = new System.Drawing.Font("Microsoft YaHei UI", 16, FontStyle.Regular);
            if (Language == "中文")
            {
                theGraphics.DrawString("订单号", myFont_order, bush, orderRectangle, format);
            }
            else
            {
                theGraphics.DrawString("Project name", myFont_order, bush, orderRectangle, format);
            }

            //引入订单号值           
            Rectangle orderNumberRectangle = new Rectangle(205, 125, 495, 105);
            theGraphics.DrawString(orderNumber, myFont_order, bush, orderNumberRectangle, format);

            //引入物料名称文字            
            Rectangle materialnameRectangle = new Rectangle(50, 187, 155, 105);
            if (Language == "中文")
            {
                theGraphics.DrawString("物料名称", myFont_order, bush, materialnameRectangle, format);
            }
            else
            {
                theGraphics.DrawString("Descriptions", myFont_order, bush, materialnameRectangle, format);
            }

            //引入物料名称值
            Rectangle materialNameRectangle = new Rectangle(205, 187, 495, 105);
            if (Language == "中文")
            {
                theGraphics.DrawString(material_name, myFont_order, bush, materialNameRectangle, format);
            }
            else//英文
            {
                if (material_name.Length <= 50)
                {
                    if (material_name.Contains("\n"))
                    {//如果物料名称中没有\n 换行符，就直接使用查询出来的值。如：IC；PCB这些没有后面没有换行符和中文，就直接引用查询出来的值
                        var materialName = material_name.Split('\n');
                        theGraphics.DrawString(materialName[0], myFont_order, bush, materialNameRectangle, format);
                    }
                    else
                    {
                        theGraphics.DrawString(material_name, myFont_order, bush, materialNameRectangle, format);
                    }
                }
                else
                {
                    Rectangle materialNameRectangle1 = new Rectangle(205, 170, 480, 105);
                    if (material_name.Contains("\n"))
                    {//如果物料名称中没有\n 换行符，就直接使用查询出来的值。如：IC；PCB这些没有后面没有换行符和中文，就直接引用查询出来的值
                        var materialName = material_name.Split('\n');
                        theGraphics.DrawString(materialName[0], myFont_order, bush, materialNameRectangle1, format);
                    }
                    else
                    {
                        theGraphics.DrawString(material_name, myFont_order, bush, materialNameRectangle1, format);
                    }
                }
            }

            //引入物料编号文字           

            if (Language == "中文")
            {
                Rectangle materialnumberRectangle = new Rectangle(50, 259, 155, 105);
                theGraphics.DrawString("物料编号", myFont_order, bush, materialnumberRectangle, format);
            }
            else
            {
                Rectangle materialnumberRectangle = new Rectangle(50, 250, 155, 105);
                theGraphics.DrawString("Material number", myFont_order, bush, materialnumberRectangle, format);
            }
            //引入物料条码
            if (materialNumber != null)//如果物料号不为空 就输出物料条形码，否则不输出条形码
            {
                Bitmap spc_materialnumberBarcode = BarCodeLablePrint.BarCodeToImg(materialNumber, 500, 100);
                double beishuhege = 0.5;
                theGraphics.DrawImage(spc_materialnumberBarcode, 330, 240, (float)(spc_materialnumberBarcode.Width * beishuhege), (float)(spc_materialnumberBarcode.Height * beishuhege));
            }
            //引入物料编号
            if (materialNumber != null)//如果物料号不为空 就引入物料号，否则不引入
            {
                System.Drawing.Font myFont_spc_materialnumber;
                myFont_spc_materialnumber = new System.Drawing.Font("Microsoft YaHei UI", 14, FontStyle.Bold);
                Rectangle materialnumberRectangle1 = new Rectangle(205, 295, 495, 105);
                theGraphics.DrawString(materialNumber, myFont_spc_materialnumber, bush, materialnumberRectangle1, format);
            }

            //引入规格描述文字                 
            if (Language == "中文")
            {
                Rectangle specificationsRectangle = new Rectangle(50, 381, 155, 105);
                theGraphics.DrawString("规格描述", myFont_order, bush, specificationsRectangle, format);
            }
            else
            {
                Rectangle specificationsRectangle = new Rectangle(50, 361, 155, 105);
                theGraphics.DrawString("Model or specification", myFont_order, bush, specificationsRectangle, format);
            }

            //引入规格描述内容
            if (specifications != null)//没有物料描述则不引入
            {
                if (Language == "中文")
                { //中文直接引用
                    if (specifications.Length < 100)
                    {
                        System.Drawing.Font myFont_spc_specification;
                        StringFormat format1 = new StringFormat();
                        format1.Alignment = StringAlignment.Near;
                        myFont_spc_specification = new System.Drawing.Font("You Yuan UI", 15, FontStyle.Regular);
                        Rectangle specificationRectangle = new Rectangle(210, 360, 495, 105);
                        theGraphics.DrawString(specifications, myFont_spc_specification, bush, specificationRectangle, format1);
                    }
                    else if (specifications.Length >= 100 && specifications.Length <= 190)
                    {
                        System.Drawing.Font myFont_spc_specification;
                        StringFormat format1 = new StringFormat();
                        format1.Alignment = StringAlignment.Near;
                        myFont_spc_specification = new System.Drawing.Font("You Yuan UI", 13, FontStyle.Regular);
                        Rectangle specificationRectangle = new Rectangle(210, 340, 495, 145);
                        theGraphics.DrawString(specifications, myFont_spc_specification, bush, specificationRectangle, format1);
                    }
                    else if (specifications.Length > 190 && specifications.Length < 303)
                    {
                        System.Drawing.Font myFont_spc_specification;
                        StringFormat format1 = new StringFormat();
                        format1.Alignment = StringAlignment.Near;
                        myFont_spc_specification = new System.Drawing.Font("You Yuan UI", 11, FontStyle.Regular);
                        Rectangle specificationRectangle = new Rectangle(210, 330, 495, 145);
                        theGraphics.DrawString(specifications, myFont_spc_specification, bush, specificationRectangle, format1);
                    }
                    else
                    {
                        System.Drawing.Font myFont_spc_specification;
                        StringFormat format1 = new StringFormat();
                        format1.Alignment = StringAlignment.Near;
                        myFont_spc_specification = new System.Drawing.Font("You Yuan UI", 10, FontStyle.Regular);
                        Rectangle specificationRectangle = new Rectangle(210, 330, 495, 145);
                        theGraphics.DrawString(specifications, myFont_spc_specification, bush, specificationRectangle, format1);
                    }
                }
                else
                {//英文
                    var specification = specifications.Split('\n');
                    if (specification[0].Length < 150)
                    {
                        System.Drawing.Font myFont_spc_specification;
                        StringFormat format1 = new StringFormat();
                        format1.Alignment = StringAlignment.Near;
                        myFont_spc_specification = new System.Drawing.Font("You Yuan UI", 15, FontStyle.Regular);
                        Rectangle specificationRectangle = new Rectangle(210, 360, 495, 105);
                        theGraphics.DrawString(specification[0], myFont_spc_specification, bush, specificationRectangle, format1);
                    }
                    else if (specification[0].Length >= 150 && specifications.Length <= 300)
                    {
                        System.Drawing.Font myFont_spc_specification;
                        StringFormat format1 = new StringFormat();
                        format1.Alignment = StringAlignment.Near;
                        myFont_spc_specification = new System.Drawing.Font("You Yuan UI", 13, FontStyle.Regular);
                        Rectangle specificationRectangle = new Rectangle(210, 340, 495, 145);
                        theGraphics.DrawString(specification[0], myFont_spc_specification, bush, specificationRectangle, format1);
                    }
                    else if (specification[0].Length > 300 && specification[0].Length < 500)
                    {
                        System.Drawing.Font myFont_spc_specification;
                        StringFormat format1 = new StringFormat();
                        format1.Alignment = StringAlignment.Near;
                        myFont_spc_specification = new System.Drawing.Font("You Yuan UI", 11, FontStyle.Regular);
                        Rectangle specificationRectangle = new Rectangle(210, 330, 495, 145);
                        theGraphics.DrawString(specification[0], myFont_spc_specification, bush, specificationRectangle, format1);
                    }
                    else
                    {
                        System.Drawing.Font myFont_spc_specification;
                        StringFormat format1 = new StringFormat();
                        format1.Alignment = StringAlignment.Near;
                        myFont_spc_specification = new System.Drawing.Font("You Yuan UI", 10, FontStyle.Regular);
                        Rectangle specificationRectangle = new Rectangle(210, 330, 495, 145);
                        theGraphics.DrawString(specification[0], myFont_spc_specification, bush, specificationRectangle, format1);
                    }
                }
            }

            //引入数量文字         
            Rectangle numRectangle = new Rectangle(50, 486, 155, 105);
            if (Language == "中文")
            {
                theGraphics.DrawString("数量", myFont_order, bush, numRectangle, format);
            }
            else
            {
                theGraphics.DrawString("QTY", myFont_order, bush, numRectangle, format);
            }

            //引入数量值
            var str = Quantity.ToString().Split('.');
            string qty = "";
            if (str[1] == "00")
            {
                qty = str[0] + "    " + unit;
            }
            else
            {
                var tem = str[1].ToArray();
                if (tem[1].ToString() == "0")
                {
                    qty = str[0] + (tem[0].ToString() == "0" ? "" : "." + tem[0].ToString()) + "    " + unit;
                }
                else
                {
                    qty = Quantity.ToString() + "    " + unit;
                }
            }
            Rectangle quantityRectangle = new Rectangle(205, 490, 495, 105);
            theGraphics.DrawString(qty.ToString(), myFont_order, bush, quantityRectangle, format);

            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
        }
        #endregion

        #endregion

        #region--根据订单号查询该订单号所有物料信息
        /// ------<Get_SPC_MaterialInfo_summary>
        /// 1.方法的作用：
        /// 2.方法的参数及用法：ordernumber(订单号)   用法：用于当做查询条件。
        /// 3.方法具体逻辑顺序,判断条件：根据传进来的订单号查询该订单号对应的所有物料号。
        /// 4.方法（可能）有的结果：将查询到的物料号输出。
        /// ------</Get_SPC_MaterialInfo_summary>
        [HttpPost]
        public ActionResult Get_SPC_MaterialInfo(string orderNumber, int ScreenNum, int Batch, string MaterialType)
        {
            var result = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == ScreenNum && c.Batch == Batch && c.SPC_Material_Type == MaterialType).Select(c => new { c.MaterialNumber, c.Quantity, c.Id, c.Material_Name }).ToList();//查找出订单号对应的物料编号
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region---输出订单装箱进度 
        [HttpPost]
        public ActionResult SPC_Containerised(string orderNumber, int screenNum, int batch)
        {
            JArray result = new JArray();
            JObject content = new JObject();
            var res = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == screenNum && c.Batch == batch).ToList().Count();//判断数据库是否有此订单号
            if (res > 0)
            {
                var order = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == screenNum && c.Batch == batch).Select(c => c.OrderNum).FirstOrDefault();//找出订单号
                var resultTotal = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == screenNum && c.Batch == batch).Select(c => c.SPC_OuterBoxBarcode).ToList().Count();//找出该订单的总数
                //找出已检验的数量
                var containerised = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == screenNum && c.Batch == batch && c.SPC_Packaging_Operator != null).Select(c => c.SPC_Packaging_Operator).ToList().Count() == 0 ? 0 : db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == screenNum && c.Batch == batch && c.SPC_Packaging_Operator != null).Select(c => c.SPC_Packaging_Operator).ToList().Count();
                var complete = containerised == 0 ? 0 + "%" : ((containerised * 100) / resultTotal).ToString("F2") + "%";//完成率
                content.Add("orderNum", order);//订单号
                content.Add("stocked", containerised + "/" + resultTotal);//组合出“已备料/物料总数”
                content.Add("complete", complete);//完成率
                result.Add(content);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region---根据订单号获取所有外箱信息
        public ActionResult getOrderNumInfo(string orderNum)
        {
            var list = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNum && c.SPC_OuterBoxBarcode != null).Select(c => new { c.OrderNum, c.SPC_OuterBoxBarcode, c.SNTN, c.ScreenNum, c.Batch, c.SPC_OuterBox_Type }).Distinct().ToList();
            return Content(JsonConvert.SerializeObject(list));
        }
        #endregion
        #endregion

        #region----4.备品配件包装修改页
        public ActionResult SPC_Packaging_Modify()   //打开时页面
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "SPC_Packaging_Modify" });
            }
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            return View();
        }

        #region---根据订单号查询记录
        /// ------<SPC_Packaging_Modify_summary>
        /// 1.方法的作用：根据订单号找出该订单号的所有记录。
        /// 2.方法的参数及用法：ordernumber（订单号）  用法：当做查询条件
        /// 3.方法具体逻辑顺序,判断条件：使用传进来的ordernumber（订单号）查询出字段为{OrderNum（订单号）, MaterialNumber（物料号）, SPC_OuterBoxBarcode（外箱号）, Quantity（数量）, 
        /// ScreenNum(屏序号), SNTN（件号/数）, Id }的记录，并将结果输出。
        /// 4.方法（可能）有的结果：将查询的结果输出。
        /// -----</SPC_Packaging_Modify_summary> 
        [HttpPost]
        public ActionResult SPC_Packaging_Modify(string ordernumber)
        {
            var totalList = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernumber).Select(c => new { c.OrderNum, c.MaterialNumber, c.SPC_OuterBoxBarcode, c.Quantity, c.ScreenNum, c.SNTN, c.Id, c.Batch, c.Material_Name, c.SPC_Material_Type, c.G_Weight, c.N_Weight, c.SPC_OuterBox_Type }).OrderBy(s => s.SPC_OuterBoxBarcode).ToList();
            return Content(JsonConvert.SerializeObject(totalList));
        }
        #endregion

        #region---删除外箱记录方法
        /// ------<SPC_Packaging_Delete_summary>
        /// 1.方法的作用：删除物料的外箱记录。
        /// 2.方法的参数及用法：spc_list(数组)  用法：循环数组（spc_list）将字段赋值为null{包括：SPC_OuterBoxBarcode（外箱号）、ScreenNum（屏序号）、SNTN（件号/数）}。
        /// 3.方法具体逻辑顺序,判断条件：（1）foreach数组（spc_list），使用当前遍历的OrderNum（订单号）、SPC_OuterBoxBarcode(外箱号)、MaterialNumber（物料号）、Quantity（数量）、ScreenNum（屏序号）、
        /// SNTN（件号/数）找出对应的记录，（2）判断找出来的记录是否等于null，若不等于null进入if分支，将当前遍历的SNTN（件号/数）、ScreenNum（屏序号）、SPC_OuterBoxBarcode（外箱号）赋值为null和零
        ///（3）判断保存记录是否大于零，大于输出“已删除条" + count + "记录!”， 否则输出“false”。
        /// 4.方法（可能）有的结果：结果一：删除成功，输出"已删除条" + count + "记录!"      结果二：删除失败输出“false”
        /// -------</SPC_Packaging_Delete_summary> 
        [HttpPost]
        public ActionResult SPC_Packaging_Delete(int id)
        {
            int count = 0;
            if (id != 0)
            {
                var record = db.Packing_SPC_Records.Where(c => c.Id == id).FirstOrDefault();
                var content = "备品配件删除外箱号，订单" + record.OrderNum + "，Id为" + record.Id + "，外箱条码" + record.SPC_OuterBoxBarcode + "，物料号为" + record.MaterialNumber + "，物品类型为" + record.SPC_Material_Type + "，数量为" + record.Quantity + "个的物料外箱已被删除";
                if (record != null)
                {
                    record.SPC_OuterBoxBarcode = null;//外箱号                                                         
                    record.SNTN = null;//件号/数
                    record.G_Weight = null;//净重量
                    record.N_Weight = null;//毛重量
                    record.SPC_OuterBox_Type = null;//外箱类型
                    record.SPC_Packaging_Operator = null;
                    record.IsLogo = null;
                    record.SPC_OutsideBoxLanguage = null;
                    record.SPC_Packaging_Date = null;
                }
                count = count + db.SaveChanges();
                if (count > 0)
                {
                    //填写日志
                    UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = content };
                    db.UserOperateLog.Add(log);
                    db.SaveChanges();
                    return Content("删除成功");
                }
                return Content("false");
            }
            return Content("false");
        }
        #region---只能删除尾箱方法
        //int count = 0;
        //foreach (var item in spc_list)
        //{
        //    var list = db.Packing_SPC_Records.Where(c => c.OrderNum == item.OrderNum && c.SPC_OuterBoxBarcode == item.SPC_OuterBoxBarcode).Select(c => c.SPC_OuterBoxBarcode).ToList();
        //    if (list.Count == 1)//等于一是该箱的最后一个物料，使用订单号，屏序、批次 找出订单号去重后的所有外箱号，在使用list与（endbox的最后一个箱号）的尾号（B后面两位）判断是否相等
        //    {
        //        var endbox = db.Packing_SPC_Records.Where(c => c.OrderNum == item.OrderNum && c.ScreenNum == item.ScreenNum && c.Batch == item.Batch && c.SPC_OuterBoxBarcode != null).Select(c => c.SPC_OuterBoxBarcode).Distinct().ToList();
        //        var endnum = endbox.LastOrDefault().Split('B');
        //        var endnumber = list.FirstOrDefault().Split('B');
        //        if (endnumber[1] == endnum[1])
        //        { //相等是尾箱 可以删除
        //            var record = db.Packing_SPC_Records.Where(c => c.Id == item.Id).FirstOrDefault();
        //            if (record != null)
        //            {
        //                record.SPC_OuterBoxBarcode = null;//外箱号                                                         
        //                record.SNTN = null;//件号/数
        //                record.G_Weight = null;//净重量
        //                record.N_Weight = null;//毛重量
        //                record.SPC_OuterBox_Type = null;//外箱类型
        //            }
        //            count = count + db.SaveChanges();
        //        }
        //        else
        //        {//不是尾箱，不能删除
        //            return Content("不是尾箱，无法删除！");
        //        }
        //    }
        //    else
        //    {//不是该箱的最后一个物料，可以直接进入else删除
        //        var record = db.Packing_SPC_Records.Where(c => c.Id == item.Id).FirstOrDefault();
        //        if (record != null)
        //        {
        //            record.SPC_OuterBoxBarcode = null;//外箱号                                                         
        //            record.SNTN = null;//件号/数
        //            record.G_Weight = null;//净重量
        //            record.N_Weight = null;//毛重量
        //            record.SPC_OuterBox_Type = null;//外箱类型
        //        }
        //        count = count + db.SaveChanges();
        //    }

        //}
        //if (count > 0) return Content("已删除条" + count + "记录!");//删除成功
        //return Content("false");//删除失败
        //}
        #endregion
        #endregion

        #region---根据外箱号获取该箱所有物料信息
        public ActionResult getBoxedMaterial(string OutsideBarcode)
        {
            if (!String.IsNullOrEmpty(OutsideBarcode))
            {
                var result = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == OutsideBarcode).Select(c => new { c.MaterialNumber, c.Quantity, c.ScreenNum, c.SPC_Material_Type, c.Id }).ToList();
                return Content(JsonConvert.SerializeObject(result));
            }
            return Content("");
        }
        #endregion

        #region---移箱方法
        public ActionResult SPC_moveBox(int id, string spc_OuterBoxBarcode)
        {
            if (id != 0 && !String.IsNullOrEmpty(spc_OuterBoxBarcode))
            {
                int count = 0;
                var record = db.Packing_SPC_Records.Where(c => c.Id == id).FirstOrDefault();
                var res = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == spc_OuterBoxBarcode).FirstOrDefault();
                var content = "备品配件物料移箱，" + "Id为" + record.Id + ",订单号为：" + record.OrderNum + "，外箱条码" + record.SPC_OuterBoxBarcode + "，物料号为" +
                    record.MaterialNumber + "，物品类型为" + record.SPC_Material_Type + "，数量为" + record.Quantity + "个的物料已被移动到外箱号为：" + spc_OuterBoxBarcode + "的外箱中";
                if (record != null)
                {
                    record.SPC_OuterBoxBarcode = spc_OuterBoxBarcode;
                    record.SNTN = res.SNTN;
                    count = count + db.SaveChanges();
                }
                if (count > 0)
                {
                    //填写日志
                    UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = content };
                    db.UserOperateLog.Add(log);
                    db.SaveChanges();
                    return Content("移箱成功");
                }
                return Content("移箱失败");
            }
            else return Content("移箱失败");
        }
        #endregion

        #region--修改外箱类型
        public ActionResult modify_ousitType(string spc_OuterBoxBarcode, string spc_OuterBox_Type)
        {
            var result = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == spc_OuterBoxBarcode).ToList();
            int count = 0;
            foreach (var item in result)
            {
                item.SPC_OuterBox_Type = spc_OuterBox_Type;
                count = count + db.SaveChanges();
            }
            if (count > 0) return Content("修改成功！");
            else return Content("修改失败！");
        }
        #endregion

        #endregion

        #region---5.打印外箱标签
        public ActionResult SPC_PrintOuterBoxLable()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "SPC_PrintOuterBoxLable" });
            }
            ViewBag.OrderList = GetOrderList();
            return View();
        }
        #region--备品、配件外箱标签打印

        #region--根据外箱号输出外箱标签图片
        /// ------<Outspc_OutsideBoxLabelToImg_summary>
        /// 1.方法的作用：根据外箱号输出外箱标签图片
        /// 2.方法的参数及用法：orderNumber(订单号)、pc_OuterBoxBarcode(外箱号)  用法：用于当做查询条件。
        /// 3.方法具体逻辑顺序,判断条件：(1).判断orderNumber(订单号)、pc_OuterBoxBarcode(外箱号)是否为空，(2)两者都不为空时，（2.1）使用订单号、外箱号找出对应的记录 { 包括：ordernum（订单号）、
        /// spc_outsidebarcode（外箱条码号）、spcType（类型）、outsideTotal（外箱号总数）、sntn（件号/数）、batch（批次）、g_Weight（净重）、g_Weight(毛重)、logo } (2.2)在调用SPC_CreateOutsideBoxLable()方法，
        /// 将参数传过去，组织成图片，将图片输出。  （3）当订单号与外箱号传过来为空时，直接输出提示“订单号或外箱号不能为空！”。
        /// 4.方法（可能）有的结果：结果一：当订单号与外箱号传过来为空时，直接输出提示“订单号或外箱号不能为空！”。 结果二：传过来的订单号、外箱号不为空，将组织好的图片输出。
        /// ------</Outspc_OutsideBoxLabelToImg_summary>
        public ActionResult Outspc_OutsideBoxLabelToImg(string orderNumber, string spc_OuterBoxBarcode, string language, bool? logo = true)
        {
            if (!String.IsNullOrEmpty(orderNumber) && !String.IsNullOrEmpty(spc_OuterBoxBarcode))
            {
                var spc_outsidebarcode_list = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.SPC_OuterBoxBarcode == spc_OuterBoxBarcode);
                int? screenNum = spc_outsidebarcode_list.FirstOrDefault().ScreenNum;
                int? batch = spc_outsidebarcode_list.FirstOrDefault().Batch;
                string ordernum = spc_outsidebarcode_list.FirstOrDefault().OrderNum;//订单号
                string spc_outsidebarcode = spc_outsidebarcode_list.FirstOrDefault().SPC_OuterBoxBarcode;//外箱号
                string spcType = spc_outsidebarcode_list.FirstOrDefault().SPC_OuterBox_Type;//物品类型
                //找出外箱号总数
                var outsideTotal = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.SPC_OuterBoxBarcode != null && c.Batch == batch && c.ScreenNum == screenNum).Select(c => c.SPC_OuterBoxBarcode).Distinct().ToList().Count();
                int lastTwo = 2;
                string partNumber = spc_outsidebarcode.Substring(spc_outsidebarcode.Length - lastTwo);//找出外箱号最后两位做件号
                string partNum = "";
                if (partNumber[0] == '0')
                {
                    partNum = partNumber[1].ToString();
                }
                else
                {
                    partNum = partNumber;
                }
                string sntn = partNum + "/" + outsideTotal;//组合成：件号/数
                //int? batch = spc_outsidebarcode_list.FirstOrDefault().Batch;//批次
                double? g_Weight = spc_outsidebarcode_list.FirstOrDefault().G_Weight;//净重 
                double? n_Weight = spc_outsidebarcode_list.FirstOrDefault().N_Weight;//毛重
                var AllBitmap = SPC_CreateOutsideBoxLable(ordernum, spc_outsidebarcode, spcType, sntn, batch, language, g_Weight, n_Weight, logo);
                MemoryStream ms = new MemoryStream();
                AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                AllBitmap.Dispose();
                return File(ms.ToArray(), "image/Png");
            }
            return Content("订单号或外箱号不能为空！");
        }
        /// ------<Outspc_OutsideBoxLabelAgain_summary>
        /// 1.方法的作用：打印外箱标签
        /// 2.方法的参数及用法：orderNumber（订单号）、spc_OuterBoxBarcode（外箱号） 用法：用于当做查询条件。  ip(IP地址)、port(端口) 用法：用于连接打印机。
        /// 3.方法具体逻辑顺序,判断条件：(1)判断传进来orderNumber(订单号)、spc_OuterBoxBarcode(外箱号)是否为空,(2)两者不为空时，（2.1）使用订单号、外箱号找出对应的记录 { 包括：ordernum（订单号）、
        /// spc_outsidebarcode（外箱条码号）、spcType（外箱类型）、outsideTotal（外箱号总数）、sntn（件号/数）、batch（批次）、g_Weight（净重量）、g_Weight(毛重量)、logo } (2.2)用ordernum（订单号）、
        /// spc_outsidebarcode（外箱号）找出表中对应的记录，在foreach()找出来的记录，将组合好的sntn(件号/数)保存到数据库中, (2.3)调用SPC_CreateOutsideBoxLable()方法，将参数传过去，组合出图片，
        /// 调用BmpToZpl()方法将位图转ZPL指令,最后调用IPPrint()方法，连接IP和端口打印内容。（3）当订单号与外箱号传过来为空时，直接输出提示“订单号或外箱号不能为空！”。
        /// 4.方法（可能）有的结果：（1）当订单号与外箱号传过来为空时，直接输出提示“订单号或外箱号不能为空！”。 （2）传过来的订单号、外箱号不为空时，调用IPPrint()方法，连接IP和端口打印内容。
        /// （2.1）"打印成功！";（2.2）"打印连接失败,请检查打印机是否断网或未开机！";
        /// ------</Outspc_OutsideBoxLabelAgain_summary>
        public ActionResult Outspc_OutsideBoxLabelAgain(string orderNumber, List<string> spc_OuterBoxBarcode, string language, string ip = "", int port = 0, bool? logo = true, int concentration = 5)
        {
            int printcount = 0;
            foreach (var item in spc_OuterBoxBarcode)
            {
                var list = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.SPC_OuterBoxBarcode == item).FirstOrDefault();
                var outsideTotal = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.SPC_OuterBoxBarcode != null && c.ScreenNum == list.ScreenNum && c.Batch == list.Batch).Select(c => c.SPC_OuterBoxBarcode).Distinct().ToList().Count();//找出此订单号的外箱总数量
                int i = 2;
                string partNumber = item.Substring(item.Length - i);//找出件号
                string partNum = "";
                if (partNumber[0] == '0')
                {
                    partNum = partNumber[1].ToString();
                }
                else
                {
                    partNum = partNumber;
                }
                string sntn = partNum + "/" + outsideTotal;   //组合成：件号/数               
                var bm = SPC_CreateOutsideBoxLable(list.OrderNum, list.SPC_OuterBoxBarcode, list.SPC_OuterBox_Type, sntn, list.Batch, language, list.G_Weight, list.N_Weight, logo);
                int totalbytes = bm.ToString().Length;//返回参数总共字节数
                int rowbytes = 10; //返回参数每行的字节数
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);//位图转ZPL指令
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
                var result = ZebraUnity.IPPrint(data.ToString(), 1, ip, port);
                //string result = "打印成功！";
                if (result == "打印成功！")
                {
                    printcount++;
                    var newlist = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == list.SPC_OuterBoxBarcode).ToList();
                    foreach (var j in newlist)
                    {
                        j.SNTN = sntn;
                        j.IsLogo = logo;
                        j.SPC_OutsideBoxLanguage = language;
                        j.Print_OutsideBox_Operator = ((Users)Session["User"]).UserName;//打印人
                        j.Print_OutsideBox_Date = DateTime.Now;//打印时间
                        db.SaveChanges();
                    }
                }
            }
            string res = (printcount > 0 ? printcount + "个打印成功!" : "") + ((spc_OuterBoxBarcode.Count - printcount) > 0 ? (spc_OuterBoxBarcode.Count - printcount) + "个打印失败！" : "");
            return Content(res);
        }
        #endregion

        #region--根据订单号输出外箱号
        /// ------<Output_CartonNumber_summary>
        /// 1.方法的作用：根据订单号输出外箱号。
        /// 2.方法的参数及用法：orderNum（订单号）  用法：当做查询条件。
        /// 3.方法具体逻辑顺序,判断条件：根据传进来的订单号找出SPC_OuterBoxBarcode（外箱号）不等于null的外箱号记录，然后去重输出。
        /// 4.方法（可能）有的结果：将找出来的外箱号输出。
        /// ------</Output_CartonNumber_summary>
        public ActionResult Output_CartonNumber(string orderNum)
        {
            var list = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNum && c.SPC_OuterBoxBarcode != null).Select(c => new { c.SPC_OuterBoxBarcode, c.G_Weight, c.N_Weight, c.SPC_OuterBox_Type, c.IsLogo, c.SPC_OutsideBoxLanguage }).Distinct().ToList();
            return Content(JsonConvert.SerializeObject(list));
        }
        public ActionResult getoutbox(string orderNum)
        {
            var list = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNum && c.SPC_OuterBoxBarcode != null).Select(c => c.SPC_OuterBoxBarcode).Distinct().ToList();
            JArray result = new JArray();
            foreach (var item in list)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region--保存净重毛重量
        public ActionResult SPC_saveWeight(string SPC_OuterBoxBarcode, double G_Weight, double N_Weight, string SPC_OuterBox_Type)
        {
            int count = 0;
            var recoed = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == SPC_OuterBoxBarcode).ToList();
            var res = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == SPC_OuterBoxBarcode).FirstOrDefault();
            if (res.G_Weight == G_Weight && res.N_Weight == N_Weight && res.SPC_OuterBox_Type == SPC_OuterBox_Type)//传进来的数据与原来的相同，就不进行保存动作
            {
                return Content("保存成功！");
            }
            else
            {
                foreach (var item in recoed)
                {
                    item.G_Weight = G_Weight;
                    item.N_Weight = N_Weight;
                    item.SPC_OuterBox_Type = SPC_OuterBox_Type;
                    count = db.SaveChanges();
                }
                if (count > 0) return Content("保存成功！");
                else return Content("保存失败！");
            }
        }
        #endregion

        #endregion

        #region--生成备品、配件外箱标签图片
        /// ------<SPC_CreateOutsideBoxLable_summary>
        /// 1.方法的作用：生成备品、配件外箱标签图片
        /// 2.方法的参数及用法： ordernum(订单号),spc_outsidebarcode(外箱号),spcType（外箱类型）,sntn（件号/数）,batch（批次）,logo，g_Weight(净重量),n_Weight(毛重量)。用法：用于组合出外箱标签图片。
        /// 3.方法具体逻辑顺序,判断条件：（1）定义整个标签的宽和高、定义好笔的大小，根据需求绘制出整个标签的样板，（2）在对应的位置引入相对应的参数，并设置字体大小{包括：ordernum(订单号),
        /// spc_outsidebarcode(外箱号), spcType（外箱类型）,sntn（件号/数）,batch（批次）,logo，g_Weight(净重量),n_Weight(毛重量)}，（3）最后调用ConvertTo1Bpp1()与ToGray()方法调整图像灰度、
        /// 灰度反转以及二值化处理（图形转二值）。
        /// 4.方法（可能）有的结果：将图形转二值结果输出。
        /// ------</SPC_CreateOutsideBoxLable_summary>
        public Bitmap SPC_CreateOutsideBoxLable(string ordernum, string spc_outsidebarcode, string spcType, string sntn, int? batch, string language, double? g_Weight = null, double? n_Weight = null, bool? logo = true)
        {
            int initialWidth = 750, initialHeight = 583;
            Bitmap AllBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(AllBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            Pen pen = new Pen(Color.Black, 3);//定义笔的大小
            theGraphics.DrawRectangle(pen, 50, 50, 650, 483);  //x,y,width:绘制矩形的宽度,height：绘制的矩形的高度
            //画横线                                                 
            theGraphics.DrawLine(pen, 50, 161, 700, 161);//起点x,起点y坐标，终点x,终点y坐标
            theGraphics.DrawLine(pen, 50, 270, 700, 270);
            theGraphics.DrawLine(pen, 50, 400, 700, 400);
            //theGraphics.DrawLine(pen, 358, 353, 700, 353);
            //画竖线
            theGraphics.DrawLine(pen, 200, 270, 200, 532);
            theGraphics.DrawLine(pen, 358, 270, 358, 532);
            theGraphics.DrawLine(pen, 515, 270, 515, 532);

            //引入logo
            if (logo == true)
            {
                Bitmap bmp_logo = new Bitmap(@"D:\\MES_Data\\LOGO_black.png");
                theGraphics.DrawImage(bmp_logo, 65, 60, (float)(bmp_logo.Width), (float)(bmp_logo.Height));
                //引入订单号
                System.Drawing.Font myFont_orderNumber;
                //字体微软雅黑，大小40，样式加粗
                myFont_orderNumber = new System.Drawing.Font("Microsoft YaHei UI", 38, FontStyle.Bold);
                //设置格式
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                theGraphics.DrawString(ordernum, myFont_orderNumber, bush, 230, 90);
            }
            else
            {
                System.Drawing.Font myFont_orderNumber;
                //字体微软雅黑，大小40，样式加粗
                myFont_orderNumber = new System.Drawing.Font("Microsoft YaHei UI", 40, FontStyle.Bold);
                //设置格式
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                theGraphics.DrawString(ordernum, myFont_orderNumber, bush, 210, 80);

            }
            //引入条形码
            Bitmap spc_barcode = BarCodeLablePrint.BarCodeToImg(spc_outsidebarcode, 550, 60);
            //double beishuhege = 0.7;
            theGraphics.DrawImage(spc_barcode, 100, 170, (float)spc_barcode.Width, (float)spc_barcode.Height);

            //引入条码号
            System.Drawing.Font myFont_spc_OuterBoxBarcode;
            myFont_spc_OuterBoxBarcode = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Bold);
            theGraphics.DrawString(spc_outsidebarcode, myFont_spc_OuterBoxBarcode, bush, 240, 230);

            //物料描述
            if (language == "中文")
            {
                System.Drawing.Font myFont_materialDescription;
                myFont_materialDescription = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("物料描述", myFont_materialDescription, bush, 70, 320);
            }
            else
            {
                System.Drawing.Font myFont_materialDescription;
                myFont_materialDescription = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("DESC", myFont_materialDescription, bush, 85, 320);
            }

            //引入外箱类型
            if (language == "中文")
            {
                System.Drawing.Font myFont_spc;
                myFont_spc = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                if (spcType == "配件、备品")
                {
                    theGraphics.DrawString(spcType, myFont_spc, bush, 210, 320);
                }
                else
                {
                    theGraphics.DrawString(spcType, myFont_spc, bush, 250, 320);
                }
            }
            else
            {
                System.Drawing.Font myFont_spc;
                myFont_spc = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                if (spcType == "配件、备品")
                {
                    theGraphics.DrawString("Accessories", myFont_spc, bush, 210, 290);
                    theGraphics.DrawString("and", myFont_spc, bush, 250, 315);
                    theGraphics.DrawString("Spare parts", myFont_spc, bush, 210, 340);
                }
                else if (spcType == "备品")
                {
                    theGraphics.DrawString("Spare Parts", myFont_spc, bush, 210, 320);
                }
                else
                {//配件
                    theGraphics.DrawString("Accessories", myFont_spc, bush, 210, 320);
                }
            }

            //引入件号/数
            if (language == "中文")
            {
                System.Drawing.Font myFont_jianhao;
                myFont_jianhao = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("件号/数", myFont_jianhao, bush, 388, 320);
            }
            else
            {
                System.Drawing.Font myFont_jianhao;
                myFont_jianhao = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("SN/TN", myFont_jianhao, bush, 396, 320);
            }

            //引入SNTN
            //设置格式            
            System.Drawing.Font myFont_snt;
            myFont_snt = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(sntn, myFont_snt, bush, 580, 320);

            //净重量
            if (language == "中文")
            {
                System.Drawing.Font myFont_N_Weight;
                myFont_N_Weight = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("净重量", myFont_N_Weight, bush, 395, 440);
                theGraphics.DrawString("(kg)", myFont_N_Weight, bush, 418, 475);
            }
            else
            {
                System.Drawing.Font myFont_N_Weight;
                myFont_N_Weight = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("N.W.(kg)", myFont_N_Weight, bush, 383, 445);
            }

            //毛重量
            System.Drawing.Font myFont_G_Weight;
            myFont_G_Weight = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            if (language == "中文")
            {

                myFont_G_Weight = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("毛重量", myFont_G_Weight, bush, 78, 440);
                theGraphics.DrawString("(kg)", myFont_G_Weight, bush, 98, 475);
            }
            else
            {
                myFont_G_Weight = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
                theGraphics.DrawString("G.W.(kg)", myFont_G_Weight, bush, 68, 445);
            }

            System.Drawing.Font myFont_Weight;
            myFont_Weight = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            StringFormat format_Weight = new StringFormat();
            format_Weight.Alignment = StringAlignment.Center;

            //毛重量值           
            double GV = g_Weight == null ? 0 : (double)g_Weight;
            theGraphics.DrawString(GV.ToString(), myFont_G_Weight, bush, 250, 445);

            //净重量值
            double NV = n_Weight == null ? 0 : (double)n_Weight;
            theGraphics.DrawString(NV.ToString(), myFont_G_Weight, bush, 578, 445);

            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
        }
        #endregion       

        #region----输出Excel表(装箱清单)

        public FileContentResult ExportExcel(string spc_OuterBoxBarcode)
        {
            string[] columns = { "箱号", "物料名称", "物料编号", "规格描述", "数量", "单位", "类型" };
            var temp = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == spc_OuterBoxBarcode).OrderByDescending(c => c.SPC_Material_Type).ToList();
            var str = Convert.ToInt32((spc_OuterBoxBarcode.Split('B'))[1]);
            foreach (var item in temp)
            {
                item.SPC_OuterBoxBarcode = str.ToString();
            }
            var centeruserlist = temp.Select(c => new { c.SPC_OuterBoxBarcode, c.Material_Name, c.MaterialNumber, c.Specification_Description, c.Quantity, c.Unit, c.SPC_Material_Type }).ToList();
            var boxtype = temp.Select(c => c.SPC_projectName).Distinct().FirstOrDefault();
            byte[] filecontent = ExcelExportHelper.ExportExcel(centeruserlist, boxtype, false, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, "装箱清单" + ".xlsx");
        }
        #endregion

        #endregion

        #region---6.物料备料
        public ActionResult SPC_StockConfirm()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "SPC_StockConfirm" });
            }
            ViewBag.OrderList = GetOrderList();
            return View();
        }

        #region---物料备料确认
        /// ------<SPC_StockConfirm_summary>
        /// 1.方法的作用：用于确认物料备料确认。
        /// 2.方法的参数及用法：orderNum(订单号)、materialNum(物料号)、quantity(数量) 用法：用于当做查询条件。material_Confirm(备料状态)、confirm_Operator(备料人) 用法：补全记录。
        /// 3.方法具体逻辑顺序,判断条件：根据传入的订单号、物料号、数量找到Packing_SPC_Records表中对应的记录，再将传进来的记录补全（补全字段包括：material_Confirm(备料状态)、confirm_Operator(备料人)、SPC_Material_Confim_Date(备料时间)）。
        /// 4.方法（可能）有的结果：备料成功或者备料失败。
        /// ------</SPC_StockConfirm_summary>    
        [HttpPost]
        public ActionResult SPC_StockConfirm(string orderNum, string materialNum, Decimal quantity, bool material_Confirm, string confirm_Operator, int? batch, int? screenNum, string specification_Description, string MaterialType)
        {
            if (specification_Description == null)
            {
                var record = db.Packing_SPC_Records.FirstOrDefault(c => c.OrderNum == orderNum && c.MaterialNumber == materialNum && c.Quantity == quantity && c.Batch == batch && c.ScreenNum == screenNum && (c.SPC_Material_Confim != true || c.SPC_Material_Confim == null) && c.SPC_Material_Type == MaterialType);
                if (record != null)
                {
                    record.SPC_Material_Confim = material_Confirm;//备料状态
                    record.SPC_Material_Confim_Operator = confirm_Operator;//备料人
                    record.SPC_Material_Confim_Date = DateTime.Now;//备料时间
                    int count = db.SaveChanges();
                    if (count > 0) return Content("确认备料成功！");
                    else return Content("确认备料失败！");
                }
                else
                {
                    return Content("未找到对应记录！");
                }
            }
            else
            {
                var record = db.Packing_SPC_Records.FirstOrDefault(c => c.OrderNum == orderNum && c.MaterialNumber == materialNum && c.Quantity == quantity && c.Batch == batch && c.ScreenNum == screenNum && c.Specification_Description == specification_Description && (c.SPC_Material_Confim != true || c.SPC_Material_Confim == null) && c.SPC_Material_Type == MaterialType);
                if (record != null)
                {
                    record.SPC_Material_Confim = material_Confirm;//备料状态
                    record.SPC_Material_Confim_Operator = confirm_Operator;//备料人
                    record.SPC_Material_Confim_Date = DateTime.Now;//备料时间
                    int count = db.SaveChanges();
                    if (count > 0) return Content("确认备料成功！");
                    else return Content("确认备料失败！");
                }
                else
                {
                    return Content("未找到对应记录！");
                }

            }
        }
        //没有物料号的备料方法
        public ActionResult NoMaterialNumber_StockConfirm(bool material_Confirm, string confirm_Operator, int id, string MaterialType)
        {
            var record = db.Packing_SPC_Records.FirstOrDefault(c => c.Id == id);
            record.SPC_Material_Confim = material_Confirm;//备料状态
            record.SPC_Material_Confim_Operator = confirm_Operator;//备料人
            record.SPC_Material_Confim_Date = DateTime.Now;//备料时间
            int count = db.SaveChanges();
            if (count > 0) return Content("确认备料成功！");
            else return Content("确认备料失败！");

        }
        #endregion

        #region---找出未备料的物料对应数量
        /// ------<GetMaterialNumByQuantity_summary>
        /// 1.方法的作用：找出物料号还未备料对应的数量。
        /// 2.方法的参数及用法：ordernumber(订单号)、materialNumber(物料号)  用法：用于当做查询条件。
        /// 3.方法具体逻辑顺序,判断条件：（1）根据订单号、物料号查询表中是否存在对应的记录，（2）若不存在直接返回false；（3）若存在就用订单号、物料号
        /// 找出字段：SPC_Material_Confim != true || c.SPC_Material_Confim == null的记录，（3.1）最后在判断记录的个数是否大于零，大于就将对应的数量输出，（3.2）小于就输出“此物料已备料！”的提示。
        /// 4.方法（可能）有的结果：订单号、物料号不存在表中，直接返回“false”；订单号、物料号存在判断是否已经备料、如已备料直接输出"此物料已备料！"，若是未备料则将对应的数量输出。
        /// ------</GetMaterialNumByQuantity_summary>
        public ActionResult GetMaterialNumByQuantity(string ordernumber, string materialNumber, int? batch, int? screenNum, string MaterialType)
        {
            if (materialNumber != "无")
            {
                if (!String.IsNullOrEmpty(ordernumber) && !String.IsNullOrEmpty(materialNumber) && batch != null && screenNum != null)
                {
                    var recodlist = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernumber && c.MaterialNumber == materialNumber && c.Batch == batch && c.ScreenNum == screenNum && (c.SPC_Material_Confim != true || c.SPC_Material_Confim == null) && c.SPC_Material_Type == MaterialType).ToList();
                    //判断订单号、物料号是否有记录
                    if (recodlist.Count > 0)
                    {
                        //有记录，判断该物料号对应的记录有几条，有两条就返回对应的物料描述，如果有一条，则不返回对应的描述信息
                        if (recodlist.Count > 1)//对条记录，返回数量、规格描述
                        {
                            var result = recodlist.Select(c => new { c.Quantity, c.Specification_Description }).ToList();
                            return Content(JsonConvert.SerializeObject(result));//返回未备料的记录
                        }
                        else//只有一条记录，直接返回数量
                        {
                            var result = recodlist.Select(c => c.Quantity).ToList();
                            return Content(JsonConvert.SerializeObject(result));//返回未备料的记录
                        }
                    }
                    else
                    {
                        return Content("找不到订单号、物料号对应的记录!");
                    }
                }
                return Content("传进来的参数为空!");
            }
            else
            {
                if (!String.IsNullOrEmpty(ordernumber) && batch != null && screenNum != null)
                {//使用订单号、屏序、批次找出物料编号为空的物料名称以及数量
                    var recodlist = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernumber && c.Batch == batch && c.ScreenNum == screenNum && c.MaterialNumber == null && (c.SPC_Material_Confim != true || c.SPC_Material_Confim == null) && c.SPC_Material_Type == MaterialType).ToList();
                    if (recodlist.Count > 0)
                    {
                        var result = recodlist.Select(c => new { c.Material_Name, c.Quantity, c.Id }).ToList();
                        return Content(JsonConvert.SerializeObject(result));//返回未备料的记录
                    }
                    else
                    {
                        return Content("找不到没有物料号的记录!");
                    }
                }
                return Content("传进来的参数为空!");
            }
        }
        #endregion

        #region---输出备料进度
        /// ------<SPC_stock_summary>
        /// 1.方法的作用：用于输出备料的进度。
        /// 2.方法的参数及用法：ordernumber(订单号)  用法：当做查询条件。
        /// 3.方法具体逻辑顺序,判断条件：（1）查询订单号是否存在表中，（2）如存在就先找出该订单号物料的总数及该订单号已备料的数量，再拿已备料数量*100除以该订单所有物料号的总数，
        /// （3）最后得出整个订单的完成率。
        /// 4.方法（可能）有的结果：输出整个订单的备料进度。
        /// ------</SPC_stock_summary>
        public ActionResult SPC_stock(string orderNumber, int screenNum, int batch)
        {
            JArray result = new JArray();
            JObject content = new JObject();
            var res = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == screenNum && c.Batch == batch).ToList().Count();//判断数据库是否有此订单号
            if (res > 0)
            {
                var order = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == screenNum && c.Batch == batch).Select(c => c.OrderNum).FirstOrDefault();//找出订单号
                var resultTotal = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == screenNum && c.Batch == batch).Select(c => c.SPC_Material_Confim).ToList().Count();//找出该订单的总数
                //找出已备料的数量
                var stocked = db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == screenNum && c.Batch == batch && c.SPC_Material_Confim == true).Select(c => c.SPC_Material_Confim).ToList().Count() == 0 ? 0 : db.Packing_SPC_Records.Where(c => c.OrderNum == orderNumber && c.ScreenNum == screenNum && c.Batch == batch && c.SPC_Material_Confim == true).Select(c => c.SPC_Material_Confim).ToList().Count();
                var complete = stocked == 0 ? 0 + "%" : ((stocked * 100) / resultTotal).ToString("F2") + "%";//完成率
                content.Add("orderNum", order);//订单号
                content.Add("stocked", stocked + "/" + resultTotal);//组合出“已备料/物料总数”
                content.Add("complete", complete);//完成率
                result.Add(content);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region---获取已备料数据
        [HttpPost]
        public ActionResult SPC_getData(string ordernum, int? batch, int? screenNum)
        {
            var info = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernum && c.ScreenNum == screenNum && c.Batch == batch && c.SPC_Material_Confim != null).Select(c => new { c.Id, c.MaterialNumber, c.Quantity, c.Material_Name }).ToList();
            return Content(JsonConvert.SerializeObject(info));
        }
        #endregion

        #region--删除已备料信息
        public ActionResult SPC_deleteStockConfirmInfo(int id)
        {
            var resultList = db.Packing_SPC_Records.Where(c => c.Id == id).FirstOrDefault();
            resultList.SPC_Material_Confim = null;
            resultList.SPC_Material_Confim = null;
            resultList.SPC_Material_Confim_Operator = null;
            int count = db.SaveChanges();
            if (count > 0) return Content("删除成功！");
            else return Content("删除失败！");
        }
        #endregion

        #region----根据物料号检验对应订单号
        public ActionResult checkOrdernum(string MaterialNumber, bool flag)
        {
            if (flag)
            {
                var resultList = db.Packing_SPC_Records.Where(c => c.MaterialNumber == MaterialNumber && c.SPC_OuterBoxBarcode == null).Select(c => c.OrderNum).ToList().Distinct();
                return Content(JsonConvert.SerializeObject(resultList));
            }
            else
            {
                var resultList = db.Packing_SPC_Records.Where(c => c.MaterialNumber == MaterialNumber && c.SPC_Material_Confim == null).Select(c => c.OrderNum).ToList().Distinct();
                return Content(JsonConvert.SerializeObject(resultList));
            }
        }
        #endregion

        #endregion

        #region---7.首页查询页(参考入库出库总查询页)
        public ActionResult SPC_QueryByOrderNumber()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "SPC_Packaging" });
            }
            return View();
        }

        #region---查询所有信息----
        /// ------<SPC_QueryByOrderNumber_summary>
        ///1.方法的作用：用于备品、配件首页信息查询 {(1)前端传订单号：可根据订单号查询出此订单号的所有外箱号、物料号、数量、备料状态、是否有图片、图纸。 (2)若无订单号传入则可查询表中所有数据 }
        ///2.方法的参数及用法：ordernumber(订单号)   用法：当做查询条件。
        ///3.方法具体逻辑顺序,判断条件：判断ordernumber(订单号)是否为空，(1)当订单号不为空时，根据订单号找出该订单所有不等于null的外箱号、再根据找出来的外箱号找出此外箱号的所有物料号，
        ///  最后再使用找出来的物料号查询指定文件夹是否有上传对应的图片及图纸，在将数组组合成数组输出。（2）当订单号为空时，则查询表中所有订单数据，再根据订单号的物料号找指定文件夹中
        ///  是否有图片或图纸，在将数组组合成数组输出。
        ///4.方法（可能）有的结果：结果一、针对传入的订单号查询出该订单号所有信息（包括所有外箱号及未装箱物料、物料号、数量、备料状态、是否有图片、图纸)；
        ///  结果二：如传入订单号为空，则查询出数据库中所有物料信息（包括所有外箱号及未装箱物料、物料号、数量、备料状态、是否有图片、图纸）
        /// ------</SPC_QueryByOrderNumber_summary>
        [HttpPost]
        public ActionResult SPC_QueryByOrderNumber(string ordernumber)
        {
            JObject results = new JObject();
            JObject material_Description = new JObject();
            if (!String.IsNullOrEmpty(ordernumber)) //前端传过来的订单号不为空则根据订单号查找
            {
                var orderlist = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernumber).ToList();//找出该订单号的所有记录
                var outerBoxList = orderlist.Where(c => c.SPC_OuterBoxBarcode != null).OrderBy(c => c.SPC_OuterBoxBarcode).ThenBy(c => c.MaterialNumber).ToList();//根据订单号找出该订单所有不等于null的外箱号
                var outerBoxListNull = orderlist.Where(c => c.SPC_OuterBoxBarcode == null).ToList();//根据订单号找出该订单所有等于null的外箱号
                JArray resualList = new JArray();
                JObject res = new JObject();
                //订单的物料种类：物料号个数
                material_Description.Add("material_Count", orderlist.Select(c => c.MaterialNumber).Count());
                //已装箱个数
                material_Description.Add("outsideBox_Count", db.Packing_SPC_Records.Where(c => c.OrderNum == ordernumber && c.SPC_OuterBoxBarcode != null).Select(c => c.SPC_OuterBoxBarcode).Distinct().ToList().Count());
                //已确认物料数量
                material_Description.Add("material_Confrim", orderlist.Where(c => c.SPC_Material_Confim == true).Count());
                results.Add("Description", material_Description);
                if (outerBoxList.Count > 0)  //有外箱号
                {
                    foreach (var item in outerBoxList)
                    {
                        res.Add("SPC_OuterBoxBarcode", item.SPC_OuterBoxBarcode);
                        res.Add("MaterialNumber", item.MaterialNumber);
                        res.Add("Quantity", item.Quantity);
                        res.Add("Material_Name", item.Material_Name);
                        res.Add("ScreenNum", item.ScreenNum);
                        res.Add("Batch", item.Batch);
                        if (item.SPC_Material_Confim == true)//判断是否已备料
                        {
                            res.Add("SPC_Material_Confim", "已备料");
                        }
                        else
                        {
                            res.Add("SPC_Material_Confim", "未备料");
                        }
                        if (item.MaterialNumber != null)
                        {
                            var diff_materialNumber = item.MaterialNumber.Split('-');//分隔物料号，使用分隔好的前几位当做图片路劲中的变量
                            List<FileInfo> dir_files = null;
                            List<FileInfo> draw_files = null;
                            if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + item.MaterialNumber + "\\Picture\\"))//判断图片路径是否存在
                            {
                                dir_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + item.MaterialNumber + "\\Picture\\");//调用GetAllFilesInDirectory()遍历文件夹中的图片
                            }
                            if (dir_files == null || dir_files.Count == 0)//判断上一步遍历出来的结果
                            {
                                res.Add("IncludePic", "否");//小于零或者等于null表示该物料号还未上传图片
                            }
                            else
                            {
                                res.Add("IncludePic", "是");//否则该物料号已上传图片
                            }
                            if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + item.MaterialNumber + "\\Draw\\"))//判断图纸路径是否存在
                            {
                                draw_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + item.MaterialNumber + "\\Draw\\");//调用GetAllFilesInDirectory()方法遍历文件夹中的图片
                            }
                            if (draw_files == null || draw_files.Count == 0)//判断上一步遍历出来的结果
                            {
                                res.Add("IncludeDraw", "否");//小于零或者等于null表示该物料号还未上传图纸
                            }
                            else
                            {
                                res.Add("IncludeDraw", "是");//否则该物料号已上传图纸
                            }
                        }
                        else
                        {
                            res.Add("IncludePic", "否");
                            res.Add("IncludeDraw", "否");
                        }
                        resualList.Add(res);
                        res = new JObject();
                    }
                }
                if (outerBoxListNull.Count > 0)  //没有外箱号
                {
                    foreach (var i in outerBoxListNull)
                    {
                        res.Add("SPC_OuterBoxBarcode", "未装箱");
                        res.Add("MaterialNumber", i.MaterialNumber);
                        res.Add("Quantity", i.Quantity);
                        res.Add("Material_Name", i.Material_Name);
                        res.Add("ScreenNum", i.ScreenNum);
                        res.Add("Batch", i.Batch);
                        if (i.SPC_Material_Confim == true)//判断是否已备料
                        {
                            res.Add("SPC_Material_Confim", "已备料");
                        }
                        else
                        {
                            res.Add("SPC_Material_Confim", "未备料");
                        }
                        if (i.MaterialNumber != null)
                        {
                            var diff_materialNumber = i.MaterialNumber.Split('-');//分隔物料号，使用分隔好的前几位当做图片路劲中的变量
                            List<FileInfo> dir_files = null;
                            List<FileInfo> draw_files = null;
                            if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + i.MaterialNumber + "\\Picture\\"))//判断图片路径是否存在
                            {
                                dir_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + i.MaterialNumber + "\\Picture\\");//调用GetAllFilesInDirectory()遍历文件夹中的图片
                            }
                            if (dir_files == null || dir_files.Count == 0)//判断上一步遍历出来的结果
                            {
                                res.Add("IncludePic", "否");//小于零或者等于null表示该物料号还未上传图片
                            }
                            else
                            {
                                res.Add("IncludePic", "是");//否则该物料号已上传图片
                            }
                            if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + i.MaterialNumber + "\\Draw\\"))//判断图纸路径是否存在
                            {
                                draw_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + i.MaterialNumber + "\\Draw\\");//调用GetAllFilesInDirectory()遍历文件夹中的图片
                            }
                            if (draw_files == null || draw_files.Count == 0)//判断上一步遍历出来的结果
                            {
                                res.Add("IncludeDraw", "否");//小于零或者等于null表示该物料号还未上传图纸
                            }
                            else
                            {
                                res.Add("IncludeDraw", "是");//否则该物料号已上传图纸
                            }
                        }
                        else
                        {
                            res.Add("IncludePic", "否");
                            res.Add("IncludeDraw", "否");
                        }
                        resualList.Add(res);
                        res = new JObject();
                    }
                }
                results.Add("ArrayData", resualList);
            }
            return Content(JsonConvert.SerializeObject(results));
            #region--查询所有订单，不用的方法
            //else  //前端没有传订单号，则查数据库中全部订单信息
            //{
            //    var allList = db.Packing_SPC_Records.OrderBy(c => c.OrderNum).ThenBy(c => c.SPC_OuterBoxBarcode).ThenBy(c => c.MaterialNumber);
            //    JObject res = new JObject();
            //    JArray resualList = new JArray();
            //    foreach (var item in allList.Select(c => c.OrderNum).Distinct().ToList())//foreach订单号，找出订单号所有外箱号不等于null的物料、以及外箱号等于null的订单
            //    {
            //        var outerBoxList = allList.Where(c => c.OrderNum == item && c.SPC_OuterBoxBarcode != null).ToList();//找出外箱号不为null的记录
            //        var outerBoxListNull = allList.Where(c => c.OrderNum == item && c.SPC_OuterBoxBarcode == null).ToList();//找出外箱号为null的记录
            //        if (outerBoxList.Count > 0)//有外箱号
            //        {
            //            foreach (var i in outerBoxList)
            //            {
            //                res.Add("OrderNum", i.OrderNum);
            //                res.Add("SPC_OuterBoxBarcode", i.SPC_OuterBoxBarcode);
            //                res.Add("MaterialNumber", i.MaterialNumber);
            //                res.Add("Quantity", i.Quantity);
            //                if (i.SPC_Material_Confim == true)//判断是否已备料
            //                {
            //                    res.Add("SPC_Material_Confim", "已备料");
            //                }
            //                else
            //                {
            //                    res.Add("SPC_Material_Confim", "未备料");
            //                }
            //                if (i.MaterialNumber != null)
            //                {
            //                    var diff_materialNumber = i.MaterialNumber.Split('-');//分隔物料号，使用分隔好的前几位当做图片路劲中的变量
            //                    List<FileInfo> dir_files = null;
            //                    List<FileInfo> draw_files = null;
            //                    if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + i.MaterialNumber + "\\Picture\\"))//判断图片路径是否存在
            //                    {
            //                        dir_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + i.MaterialNumber + "\\Picture\\");//调用GetAllFilesInDirectory()遍历文件夹中的图片
            //                    }
            //                    if (dir_files == null || dir_files.Count == 0)//判断上一步遍历出来的结果
            //                    {
            //                        res.Add("IncludePic", "否");// 小于零或者等于null表示该物料号还未上传图片
            //                    }
            //                    else
            //                    {
            //                        res.Add("IncludePic", "是");//否则该物料号已上传图片
            //                    }
            //                    if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + i.MaterialNumber + "\\Draw\\"))//判断图纸路径是否存在
            //                    {
            //                        draw_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + i.MaterialNumber + "\\Draw\\");//调用GetAllFilesInDirectory()遍历文件夹中的图片
            //                    }
            //                    if (draw_files == null || draw_files.Count == 0)//判断上一步遍历出来的结果
            //                    {
            //                        res.Add("IncludeDraw", "否");// 小于零或者等于null表示该物料号还未上传图纸
            //                    }
            //                    else
            //                    {
            //                        res.Add("IncludeDraw", "是");//否则该物料号已上传图纸
            //                    }
            //                }
            //                else {
            //                    res.Add("IncludePic", "否");
            //                    res.Add("IncludeDraw", "否");
            //                }                            
            //                resualList.Add(res);
            //                res = new JObject();
            //            }
            //        }
            //        if (outerBoxListNull.Count > 0)//没有外箱号
            //        {
            //            foreach (var j in outerBoxListNull)
            //            {
            //                res.Add("OrderNum", j.OrderNum);
            //                res.Add("SPC_OuterBoxBarcode", "未装箱");
            //                res.Add("MaterialNumber", j.MaterialNumber);
            //                res.Add("Quantity", j.Quantity);
            //                if (j.SPC_Material_Confim == true)//判断是否已备料
            //                {
            //                    res.Add("SPC_Material_Confim", "已备料");
            //                }
            //                else
            //                {
            //                    res.Add("SPC_Material_Confim", "未备料");
            //                }
            //                if (j.MaterialNumber != null)
            //                {
            //                    var diff_materialNumber = j.MaterialNumber.Split('-');//分隔物料号，使用分隔好的前几位当做图片路劲中的变量
            //                    List<FileInfo> dir_files = null;
            //                    List<FileInfo> draw_files = null;
            //                    if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + j.MaterialNumber + "\\Picture\\"))//判断图片路径是否存在
            //                    {
            //                        dir_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + j.MaterialNumber + "\\Picture\\");//调用GetAllFilesInDirectory()遍历文件夹中的图片
            //                    }
            //                    if (dir_files == null || dir_files.Count == 0)//判断上一步遍历出来的结果
            //                    {
            //                        res.Add("IncludePic", "否");// 小于零或者等于null表示该物料号还未上传图片
            //                    }
            //                    else
            //                    {
            //                        res.Add("IncludePic", "是");//否则该物料号已上传图片
            //                    }
            //                    if (Directory.Exists(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + j.MaterialNumber + "\\Draw\\"))//判断图纸路径是否存在
            //                    {
            //                        draw_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\SpareParts\" + diff_materialNumber[0] + "\\" + j.MaterialNumber + "\\Draw\\");//调用GetAllFilesInDirectory()遍历文件夹中的图片
            //                    }
            //                    if (draw_files == null || draw_files.Count == 0)//判断上一步遍历出来的结果
            //                    {
            //                        res.Add("IncludeDraw", "否");// 小于零或者等于null表示该物料号还未上传图纸
            //                    }
            //                    else
            //                    {
            //                        res.Add("IncludeDraw", "是");//否则该物料号已上传图纸
            //                    }
            //                }
            //                else {
            //                    res.Add("IncludePic", "否");
            //                    res.Add("IncludeDraw", "否");
            //                }                            
            //                resualList.Add(res);
            //                res = new JObject();
            //            }
            //        }
            //    }
            //    results.Add("ArrayData", resualList);
            //    material_Description.Add("material_Count", "");
            //    material_Description.Add("outsideBox_Count", "");
            //    material_Description.Add("material_Confrim", "");
            //    results.Add("Description", material_Description);
            //    return Content(JsonConvert.SerializeObject(results));
            //}
            #endregion
        }
        #endregion

        #region---获取订单号----

        /// ------<GetOrdernum_summary>
        ///1.方法的作用：找出Packing_SPC_Records表中所有订单号，便于前端备品配件首页根据订单号查询信息。
        ///2.方法参数及用法：无
        ///3.方法具体逻辑、判断条件：直接查询表中所有订单号记录，然后去重输出。
        ///4.方法（可能）有的结果：将表格中所有订单号输出。
        /// ------</GetOrdernum_summary>
        [HttpPost]
        public ActionResult GetOrdernum()
        {
            var orderList = db.Packing_SPC_Records.Select(c => c.OrderNum).Distinct().ToList();
            JArray result = new JArray();
            foreach (var item in orderList)
            {
                JObject List = new JObject();
                List.Add("value", item);
                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #endregion

        #region ---8.备品配件出入库
        public ActionResult SPC_storage()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "SPC_storage" });
            }
            return View();
        }
        public ActionResult SPC_storageConfirm(string outerBarcode)
        {
            if (!String.IsNullOrEmpty(outerBarcode))
            {
                JObject message = new JObject();
                var warejoin = db.Warehouse_Join.Count(c => c.OuterBoxBarcode == outerBarcode && (c.OrderNum == c.NewBarcode || c.NewBarcode == null));//入库订单列表
                if (warejoin > 0)
                {
                    message.Add("message", "此条码已入库");
                    return Content(JsonConvert.SerializeObject(message));
                }

                var recode = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == outerBarcode && c.SPC_OuterBoxBarcode != null).Select(c => c.SPC_OuterBoxBarcode).FirstOrDefault();
                if (recode == null)//在打印表里找不到此外箱条码信息
                {
                    message.Add("message", "找不到此条码!");
                    return Content(JsonConvert.SerializeObject(message));
                }
                if (message.Count == 0)
                {
                    var spc_outsidebarcode_list = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == outerBarcode);
                    string ordernum = spc_outsidebarcode_list.FirstOrDefault().OrderNum;//订单号
                    string spc_outsidebarcode = outerBarcode;//外箱号
                    string spcType = spc_outsidebarcode_list.FirstOrDefault().SPC_OuterBox_Type;//物品类型
                    string language = spc_outsidebarcode_list.FirstOrDefault().SPC_OutsideBoxLanguage;
                    bool? logo = spc_outsidebarcode_list.FirstOrDefault().IsLogo;
                    //找出外箱号总数
                    var outsideTotal = db.Packing_SPC_Records.Where(c => c.OrderNum == ordernum && c.SPC_OuterBoxBarcode != null).Select(c => c.SPC_OuterBoxBarcode).Distinct().ToList().Count();
                    int lastTwo = 2;
                    string partNumber = outerBarcode.Substring(outerBarcode.Length - lastTwo);//找出外箱号最后两位做件号
                    string partNum = "";
                    if (partNumber[0] == '0') partNum = partNumber[1].ToString();
                    else partNum = partNumber;
                    string sntn = partNum + "/" + outsideTotal;//组合成：件号/数
                    int? batch = spc_outsidebarcode_list.FirstOrDefault().Batch;//批次
                    double? g_Weight = spc_outsidebarcode_list.FirstOrDefault().G_Weight;//净重 
                    double? n_Weight = spc_outsidebarcode_list.FirstOrDefault().N_Weight;//毛重                
                    var AllBitmap = SPC_CreateOutsideBoxLable(ordernum, spc_outsidebarcode, spcType, sntn, batch, language, g_Weight, n_Weight, logo);
                    MemoryStream ms = new MemoryStream();
                    AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    AllBitmap.Dispose();
                    return File(ms.ToArray(), "image/Png");
                }
            }
            return Content("");
        }
        //备品入库确认
        public ActionResult SPC_Confirm(string warehouseNum, string outherboxbarcode, string Department, string Group)
        {
            List<string> ordernumList = new List<string>();
            if (db.Warehouse_Join.Count(c => c.OuterBoxBarcode == outherboxbarcode && c.IsOut == false) == 0)
            {
                var barcodeList = db.Packing_SPC_Records.Where(c => c.SPC_OuterBoxBarcode == outherboxbarcode).ToList();
                var ordernum = barcodeList.Select(c => c.OrderNum).FirstOrDefault();
                foreach (var item in barcodeList)
                {
                    Warehouse_Join join = new Warehouse_Join() { OrderNum = item.OrderNum, OuterBoxBarcode = outherboxbarcode, Operator = ((Users)Session["User"]).UserName, Date = DateTime.Now, WarehouseNum = warehouseNum, CartonOrderNum = ordernum, State = "备品", Department = Department, Group = Group };
                    db.Warehouse_Join.Add(join);
                    var count = db.SaveChanges();
                    if (count > 0) return Content("入库成功!");
                    else return Content("入库失败！");
                }
            }
            return Content("");
        }

        //备品出库确认
        public ActionResult SPC_OutStockComifirm()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "SPC_OutStockComifirm" });
            }
            return View();
        }

        //outsidebarcode 外箱条码
        //ordernum  订单号
        //isInside  是外箱 还是尾箱
        //material 物料号
        public ActionResult SPC_Checkbarcode(string outsidebarcode, string material, string ordernum, bool isInside = false)
        {
            JObject message = new JObject();
            //判断记录表里有没有该箱号号
            var count = db.Packing_SPC_Records.Count(c => (isInside == false ? c.SPC_OuterBoxBarcode == outsidebarcode : c.SPC_OuterBoxBarcode == outsidebarcode && c.MaterialNumber == material) && c.SPC_Packaging_Operator != null);
            if (count > 0)
            {
                if (!string.IsNullOrEmpty(ordernum))
                {//传进来是的外箱号检验外箱号跟订单号是否匹配       
                    string currentorder = db.Packing_SPC_Records.Where(c => (isInside == false ? c.SPC_OuterBoxBarcode == outsidebarcode : c.SPC_OuterBoxBarcode == outsidebarcode && c.MaterialNumber == material) && c.SPC_Packaging_Operator != null).Select(c => c.OrderNum).FirstOrDefault();
                    if (ordernum != currentorder)
                    {
                        message.Add("message", "此外箱条码的订单号应该是:" + currentorder);
                        message.Add("warehouseNum", "");
                        message.Add("barcode", outsidebarcode);
                        return Content(JsonConvert.SerializeObject(message));
                    }
                }
                List<Warehouse_Join> result = new List<Warehouse_Join>();
                result = GetList(ordernum);
                var warejoin = result.Count(c => isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.OuterBoxBarcode == outsidebarcode);
                if (warejoin == 0)
                {
                    message.Add("message", isInside == false ? "此外箱条码未入库" : "此物料物料条码未入库");
                    message.Add("warehouseNum", "");
                    message.Add("barcode", outsidebarcode);
                    return Content(JsonConvert.SerializeObject(message));
                }
                var wareout = result.Where(c => isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.OuterBoxBarcode == outsidebarcode).Select(c => c.IsOut);
                if (wareout.Contains(true))
                {
                    var max = db.Warehouse_Join.Where(c => (isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.OuterBoxBarcode == outsidebarcode) && c.IsOut == true).Max(c => c.WarehouseOutNum);
                    var warehouseNum = db.Warehouse_Join.Where(c => (isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.OuterBoxBarcode == outsidebarcode) && c.IsOut == true && c.WarehouseOutNum == max).Select(c => c.WarehouseNum).FirstOrDefault();
                    message.Add("message", isInside == false ? "此外箱条码已出库" : "此外箱条码已出库");
                    message.Add("warehouseNum", warehouseNum);
                    message.Add("barcode", outsidebarcode);
                    return Content(JsonConvert.SerializeObject(message));
                }
                var warejoinNum = db.Warehouse_Join.Where(c => (isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.OuterBoxBarcode == outsidebarcode) && c.IsOut == false).Select(c => c.WarehouseNum).FirstOrDefault();
                message.Add("message", "");
                message.Add("warehouseNum", warejoinNum);
                message.Add("barcode", outsidebarcode);
                return Content(JsonConvert.SerializeObject(message));
            }
            else
            {
                message.Add("message", isInside == false ? "没有找到此外箱条码" : "没有找到此物料号");
                message.Add("warehouseNum", "");
                message.Add("barcode", outsidebarcode);
                return Content(JsonConvert.SerializeObject(message));
            }
        }
        public List<Warehouse_Join> GetList(string ordernum)
        {
            List<Warehouse_Join> result = new List<Warehouse_Join>();
            //查找订单所有的外箱条码列表
            var list = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.State == "备品").ToList();//本订单出库
            var list2 = db.Warehouse_Join.Where(c => c.OrderNum != c.NewBarcode && c.NewBarcode == ordernum && c.State == "备品").ToList();//挪用别人的信息
            list.AddRange(list2);
            //查找订单所有未出库的列表
            var falselist = list.Where(c => c.IsOut == false).ToList();
            //查找已出库，并且不是挪用出库，剔除混装在内的列表
            var truelist = list.Where(c => c.IsOut == true).ToList();
            if (falselist.Count() == 0 && truelist.Count() == 0)//  未出库列表表为0.出库列表为0 ,代表没有符合条件的出入库信息
            {
                return result;
            }
            result.AddRange(falselist);//添加入库信息,不管是几次出库,入库状态都不会重复
            if (truelist.Count() != 0)//出库数量不为零
            {
                foreach (var item in truelist)//循环出库列表
                {
                    if (truelist.Count(c => c.OuterBoxBarcode == item.OuterBoxBarcode) != 0)//选取每一个外箱条码最后一次出库记录
                    {
                        var itemMaxNUm = truelist.Where(c => c.OuterBoxBarcode == item.OuterBoxBarcode).Max(c => c.WarehouseOutNum);
                        var currentwarehouout = truelist.Where(c => c.OuterBoxBarcode == item.OuterBoxBarcode && c.WarehouseOutNum == itemMaxNUm).ToList();
                        result.AddRange(currentwarehouout);//添加进记录
                    }
                }
            }
            return result;
        }

        //barcode条码号   warehousordernum  出库订单号或者是挪用的订单号   isDelete 是否挪用 按物料号挪用或者是按外箱号挪用  //Transportation 物料挪用
        //string warehousordernum, bool isDelete , int screenNum, int batchNum, bool isInside = false, bool isLogo = true
        public ActionResult SPC_WarehouseOut(List<string> barcode, string Transportation, string WarehouseOutDocuments, string Department, string Group)
        {//正常出库
            int count = 0;
            foreach (var item in barcode)
            {
                var info = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == item && c.IsOut == false).ToList();//已入库未出库列表
                var ordernum = info.Select(c => c.CartonOrderNum).FirstOrDefault();//查找订单               
                var num = 1;
                //添加出库信息
                foreach (var warehouse_Join in info)
                {
                    warehouse_Join.IsOut = true;//true为出库
                    warehouse_Join.WarehouseOutDate = DateTime.Now;//现在时间
                    warehouse_Join.WarehouseOutOperator = ((Users)Session["User"]).UserName;//登录人
                    warehouse_Join.WarehouseOutNum = num;//出库次数                        
                    warehouse_Join.Transportation = Transportation;//运输方式
                    warehouse_Join.WarehouseOutDocuments = WarehouseOutDocuments;//物流号
                    warehouse_Join.WarehouseOutDepartment = Department;//部门
                    warehouse_Join.WarehouseOutGroup = Group;//班组                    
                    db.Entry(warehouse_Join).State = EntityState.Modified;
                    count = count + db.SaveChanges();
                }
            }
            if (count > 0) return Content("出库成功！");
            else return Content("出库失败！");
        }
        #endregion
        #endregion

        #region 仓库录入

        /// <summary>
        /// 外箱入库信息显示
        /// </summary>
        /// 查看入库表是否有相同的外箱号,包括已出库的,如果有则提示已入库
        /// 查找打印表中是否有传过来的外箱条码,没有则提示找不到此条码
        /// 否则根据类型和分屏号,将订单的完成信息显示出来(先根据类型循环,在根据分屏号循环,找到不同类型和分屏号的打印数量和定义数量,一一显示出来)
        /// <param name="outerBarcode"></param>
        /// <returns></returns>
        public ActionResult DisaplyMessage(string outerBarcode)
        {
            JObject message = new JObject();

            var warejoin = db.Warehouse_Join.Count(c => c.OuterBoxBarcode == outerBarcode && (c.OrderNum == c.NewBarcode || c.NewBarcode == null));//入库订单列表
            if (warejoin > 0)//含有入库信息
            {
                message.Add("ordernum", null);
                message.Add("table", null);
                message.Add("barcode", null);
                message.Add("message", "此条码已入库");
                return Content(JsonConvert.SerializeObject(message));
            }
            var ordernum = "";
            if (outerBarcode.Substring(outerBarcode.Length - 5, 2) == "MK")
            {
                ordernum = db.ModuleOutsideTheBox.Where(c => c.OutsideBarcode == outerBarcode).Select(c => c.OrderNum).FirstOrDefault();//打印表查找外箱条码
            }
            else
            {
                ordernum = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outerBarcode && c.QC_Operator == null).Select(c => c.CartonOrderNum).FirstOrDefault();//打印表查找外箱条码
            }
            if (ordernum == null)//在打印表里找不到此外箱条码信息
            {
                message.Add("ordernum", null);
                message.Add("table", null);
                message.Add("barcode", null);
                message.Add("message", "找不到此条码");
                return Content(JsonConvert.SerializeObject(message));
            }
            message.Add("ordernum", ordernum);

            //var type = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Select(c => c.Type).Distinct().ToList();//订单包装类型列表
            JArray total = new JArray();
            total = GetcompleteInfoFunction(ordernum);
            //foreach (var item in type)//循环包装类型
            //{
            //    var printBarcodeinfo = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == ordernum && c.Type == item && c.QC_Operator == null).Select(c => c.OuterBoxBarcode).Distinct();//查找已打印外箱条码列表（每个类型）
            //    int printBarcode = printBarcodeinfo.Count();
            //    var screenNumList = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == item).Select(c => c.ScreenNum).ToList();//查找分屏号（每个类型）
            //    foreach (var screenNum in screenNumList)
            //    {
            //        JObject info = new JObject();
            //        var totleNum = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNum).Count() == 0 ? 0 : db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == item && c.ScreenNum == screenNum).Sum(c => c.Quantity);//查找包装规定的总箱数（每个类型）

            //        //类型
            //        info.Add("type", item);
            //        //完成数量
            //        info.Add("completeNum", printBarcode.ToString() + "/" + totleNum.ToString());
            //        //屏序
            //        info.Add("screenNum", screenNum);
            //        //完成率
            //        info.Add("complete", totleNum == 0 ? 0 + "%" : ((printBarcode * 100) / totleNum).ToString("F2") + "%");

            //        total.Add(info);
            //    }
            //}
            //JObject info2 = new JObject();
            //var printBarcodeinfo2 = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == ordernum && c.QC_Operator == null).Select(c => c.OuterBoxBarcode).Distinct().Count();//查找已打印外箱条码列表（总）
            //var totleNum2 = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Count() == 0 ? 0 : db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Sum(c => c.Quantity);//查找包装规定的总箱数（总）
            ////类型
            //info2.Add("type", "总计");
            ////完成数量
            //info2.Add("completeNum", printBarcodeinfo2.ToString() + "/" + totleNum2.ToString());
            ////屏序
            //info2.Add("screenNum", "--");
            ////完成率
            //info2.Add("complete", totleNum2 == 0 ? 0 + "%" : ((printBarcodeinfo2 * 100) / totleNum2).ToString("F2") + "%");

            //total.Add(info2);

            message.Add("table", total);
            message.Add("barcode", outerBarcode);
            message.Add("message", "");
            return Content(JsonConvert.SerializeObject(message));
        }

        /// <summary>
        /// 外箱入库录入
        /// </summary>
        /// 循环传过来的外箱条码列表,判断Warehouse_Join是否有已入库的信息,如果已有入库信息,则不录入表中,否则录入表中,并往ordernumList添加订单.
        /// 循环ordernumList,将订单号,条码号,计划包装数量,已包装数量,入库数量,已入库,剩下已包装未入库,未包装数量信息 返回给前端
        /// <param name="warehouseNum">库位号</param>
        /// <param name="outherboxbarcode">外箱条码列表</param>
        /// <returns></returns>
        public ActionResult CretecWarehouseInfo(string warehouseNum, List<string> outherboxbarcode, string Department1, string Group)
        {
            if (string.IsNullOrEmpty(warehouseNum) || outherboxbarcode.Count == 0)//没用
            {

            }
            JArray total = new JArray();
            List<string> ordernumList = new List<string>();
            foreach (var outheritem in outherboxbarcode)//循环外箱条码列表
            {
                if (db.Warehouse_Join.Count(c => c.OuterBoxBarcode == outheritem && c.IsOut == false) == 0)//查找外箱条码是否已入库没出库,是的话不做任何操作
                {
                    if (outheritem.Substring(outheritem.Length - 5, 2) == "MK")
                    {
                        //外箱条码号已包装的记录
                        var barcodeList = db.ModuleOutsideTheBox.Where(c => c.OutsideBarcode == outheritem).ToList();
                        //订单号
                        var ordernum = barcodeList.Select(c => c.OrderNum).FirstOrDefault();
                        if (!ordernumList.Contains(ordernum))
                        {
                            ordernumList.Add(ordernum);//添加要入库的订单列表
                        }
                        //往数据库加数据
                        foreach (var item in barcodeList)
                        {
                            Warehouse_Join join = new Warehouse_Join() { OrderNum = item.OrderNum, BarCodeNum = item.InnerBarcode, OuterBoxBarcode = outheritem, Operator = ((Users)Session["User"]).UserName, Date = DateTime.Now, WarehouseNum = warehouseNum, CartonOrderNum = ordernum, State = "模块", Department = Department1, Group = Group };
                            db.Warehouse_Join.Add(join);
                            db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        //外箱条码号已包装的记录
                        var barcodeList = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outheritem && c.QC_Operator == null).ToList();
                        //订单号
                        var ordernum = barcodeList.Select(c => c.CartonOrderNum).FirstOrDefault();
                        if (!ordernumList.Contains(ordernum))
                        {
                            ordernumList.Add(ordernum);//添加要入库的订单列表
                        }
                        //往数据库加数据
                        foreach (var item in barcodeList)
                        {
                            Warehouse_Join join = new Warehouse_Join() { OrderNum = item.OrderNum, BarCodeNum = item.BarCodeNum, ModuleGroupNum = item.ModuleGroupNum, OuterBoxBarcode = outheritem, Operator = ((Users)Session["User"]).UserName, Date = DateTime.Now, WarehouseNum = warehouseNum, CartonOrderNum = ordernum, State = "模组", Department = Department1, Group = Group };
                            db.Warehouse_Join.Add(join);
                            db.SaveChangesAsync();
                        }
                    }
                }
            }
            //根据不同的订单列表提示信息
            foreach (var item in ordernumList)
            {
                JObject orderjobject = new JObject();
                orderjobject.Add("orderNum", item);//订单号

                var outherBacode = db.Warehouse_Join.Where(c => c.OrderNum == item && outherboxbarcode.Contains(c.OuterBoxBarcode)).Select(c => c.OuterBoxBarcode).Distinct().ToList();//查找外箱条码号
                JArray barcode = new JArray();
                barcode.Add(outherBacode);
                orderjobject.Add("barcode", barcode);//外箱条码号
                //计划包装数量
                var planPrint = db.Packing_BasicInfo.Where(c => c.OrderNum == item).Count() == 0 ? 0 : db.Packing_BasicInfo.Where(c => c.OrderNum == item).Sum(c => c.Quantity);
                //已包装数量
                var printCount = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == item && c.QC_Operator == null).Select(c => c.OuterBoxBarcode).Distinct().Count();
                //入库数量
                var warhhousr = commom.GetCurrentwarehousList(item).Select(c => c.OuterBoxBarcode).Distinct().Count();

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

        /// <summary>
        /// 修改外箱库位号
        /// </summary>
        /// 循环外箱条码号,根据外箱条码号找Warehouse_Join中是否有未出库的信息,如果有,则修改库位号,没有则跳过,最后填写日志
        /// <param name="ordernum">订单号</param>
        /// <param name="outherboxbarcode">外箱条码号</param>
        /// <param name="warehouNum">库位号</param>
        [HttpPost]
        public void StockNumEdit(string ordernum, List<string> outherboxbarcode, string warehouNum)
        {
            string warehouse = "";
            foreach (var barcode in outherboxbarcode)//循环外箱条码集合
            {
                var list = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == barcode && c.IsOut == false).ToList();//查找列表已入库但没出库列表
                warehouse = list.FirstOrDefault().WarehouseNum;//得到原库位号
                if (list.Count != 0)
                {
                    foreach (var item in list)
                    {
                        item.WarehouseNum = warehouNum;//修改新库位号
                        db.SaveChangesAsync();
                    }

                }
            }
            //填写日志
            UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "外箱库位号修改，订单" + ordernum + ",外箱条码" + string.Join(",", outherboxbarcode.ToArray()) + "原库位号为" + warehouse + "修改为" + warehouNum };
            db.UserOperateLog.Add(log);
            db.SaveChanges();
        }

        public ActionResult WithdRawingWarehouse(List<string> outherboxbarcode)
        {
            JObject result = new JObject();
            if (Session["User"] == null)//判断是否有登录,没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "outStockConfirm" });
            }
            var IsOut = db.Warehouse_Join.Where(c => outherboxbarcode.Contains(c.OuterBoxBarcode) && c.IsOut == true).ToList();
            if (IsOut.Count != 0)
            {
                string outhre = string.Join(",", IsOut.Select(c => c.OuterBoxBarcode).ToList());
                result.Add("message", "退库失败,外箱条码" + outhre + "已出库");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));
            }
            foreach (var item in outherboxbarcode)
            {
                var warehouse = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == item).ToList();
                string barcodelist = string.Join("/", warehouse.Select(c => c.BarCodeNum).ToList());
                string message = "删除入库记录,外箱条码是" + item + ",包含模组是" + barcodelist + ",库位号是" + warehouse.Select(c => c.WarehouseNum).FirstOrDefault() + ",记录的部门班组信息是" + warehouse.Select(c => c.Department).FirstOrDefault() + "/" + warehouse.Select(c => c.Group).FirstOrDefault();
                UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName, OperateRecord = message };
                db.UserOperateLog.Add(log);
                db.Warehouse_Join.RemoveRange(warehouse);
                db.SaveChanges();
            }
            result.Add("message", "退库成功");
            result.Add("pass", true);
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 出库记录
        /// <summary>
        /// 出库信息记录  
        /// </summary>
        /// 循环条码列表,判断入库信息里是否有改条码信息,如果没有,则跳过,有则将订单号记录到ordernumList中,并查找出库信息里有没有改条码信息,如果没有则出库次数定义为1,有则在原有的出库次数中+1,数据库修改信息,如果isDelete为true,需要删除打印信息和内箱确认信息
        /// 循环ordernumList,如果出库数量==打印数量,并且isDelete为true,则删除服务器的json文件,
        /// 返回订单号,条码号,已出库数量,剩下未出库数量,未包装数量给前端
        /// <param name="barcode">条码号列表</param>
        /// <param name="warehousordernum">出库的订单号,如果是出库则是自身的订单号,如果是挪用出库则是挪用的订单号</param>
        /// <param name="isDelete">isDelet为ture表示是挪用出库,需要删除打印信息和内箱确认信息</param>
        /// <param name="Transportation">物流方式</param>
        /// <param name="WarehouseOutDocuments">出库单号</param>
        /// <returns></returns>
        public ActionResult WarehouseOut4(List<string> barcode, string warehousordernum, bool isDelete, string Transportation, string WarehouseOutDocuments, string Department1, string Group, int screenNum = 1, int batchNum = 1, bool isInside = false, bool isLogo = true)
        {
            if (Session["User"] == null)//判断是否有登录,没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "outStockConfirm" });
            }
            JObject total = new JObject();
            JArray array = new JArray();
            List<string> ordernumList = new List<string>();
            string errorordernum = "";
            string errorordernum2 = "";
            JArray newOutherBarcode = new JArray();
            if (isInside)//内箱出库
            {
                if (barcode[0].Substring(barcode[0].Length - 5, 2) == "MK")//模块不能选择内箱出库
                {
                    JObject orderjobject = new JObject();
                    orderjobject.Add("orderNum", null);//订单号
                    orderjobject.Add("barcode", null);
                    orderjobject.Add("warehousOutCount", null);
                    orderjobject.Add("warehousCount", null);
                    orderjobject.Add("notPrintCount", null);
                    orderjobject.Add("message", "模块不能选择内箱出库,请重新选择");
                    array.Add(orderjobject);
                    total.Add("mes", array);
                    total.Add("pass", false);
                    total.Add("newOutherBarocode", newOutherBarcode);//挪用后新生成的外箱条码
                    return Content(JsonConvert.SerializeObject(total));
                }
                string newoutherbarcode = "";
                if (isDelete)//是否是挪用,挪用要先判断挪用的定义数量有没有大于出库数量,而后再修改打印信息
                {
                    //定义的包装数量
                    var quantiy = db.Packing_BasicInfo.Where(c => c.OrderNum == warehousordernum && c.ScreenNum == screenNum && c.Batch == batchNum).Select(c => new { c.Quantity, c.OuterBoxCapacity }).ToList();
                    var totalnum = 0;
                    //定义总件数
                    quantiy.ForEach(c => totalnum = totalnum + (c.OuterBoxCapacity * c.Quantity));
                    // 已经打印的数量
                    var completePrint = db.Packing_BarCodePrinting.Where(c => (c.CartonOrderNum == warehousordernum || c.EmbezzleOrderNum == warehousordernum) && c.ScreenNum == screenNum && c.Batch == batchNum).Select(c => c.BarCodeNum).Distinct().Count();
                    //如果已经超过或者等于这个数量
                    if (totalnum <= completePrint)
                    {
                        errorordernum2 = errorordernum2 + string.Join(",", isInside) + ",";
                    }
                    else
                    {
                        var printinfo = db.Packing_BarCodePrinting.Where(c => barcode.Contains(c.BarCodeNum)).ToList();//外箱条码信息

                        var count = db.Packing_BasicInfo.Where(c => c.OrderNum == warehousordernum).Sum(c => c.Quantity);//挪用定义的总数量
                        #region 外箱条码生成
                        string[] str = warehousordernum.Split('-');//将订单号分割
                        string start = str[0].Substring(2);//取出 如2017-test-1 的17
                        string OuterBoxBarCodeNum = start + str[1] + "-" + str[2] + "-" + screenNum.ToString().PadLeft(2, '0') + "-" + batchNum.ToString().PadLeft(2, '0') + "-";//外箱条码组成 
                        for (int i = 1; i <= count; i++)//从1开始循环,最大数为定义的打印数,用来确定标签的序列号
                        {
                            var sequence = (OuterBoxBarCodeNum + i.ToString().PadLeft(3, '0'));//生成含有序列数的标签号
                            if (db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == sequence) == 0)//判断打印表里是否有相同的标签号,没有则将此标签号存入数据中,有则继续循环
                            {
                                newOutherBarcode.Add(sequence);
                                newoutherbarcode = sequence;
                                printinfo.ForEach(c => { c.OuterBoxBarcode = sequence; c.EmbezzleOrderNum = warehousordernum; c.ScreenNum = screenNum; c.SNTN = i.ToString(); c.IsLogo = isLogo; c.Batch = batchNum; });//修改标签信息
                                break;//找到符合的条码就跳出循环
                            }
                        }
                        #endregion
                    }
                }
                if (string.IsNullOrEmpty(errorordernum2) && string.IsNullOrEmpty(errorordernum2))
                {
                    var info = db.Warehouse_Join.Where(c => barcode.Contains(c.BarCodeNum) && c.IsOut == false).ToList();//已入库未出库列表
                    var ordernum = info.Select(c => c.CartonOrderNum).FirstOrDefault();//查找订单
                    if (!ordernumList.Contains(ordernum))
                    {
                        ordernumList.Add(ordernum);//记录要出库的订单列表
                    }
                    //查找上一次出库的次数，如果没有出过库，则为1
                    var num = 0;
                    var list = db.Warehouse_Join.Where(c => barcode.Contains(c.BarCodeNum) && c.IsOut == true).ToList();
                    if (list.Count == 0)
                    { num = 1; }
                    else//否则在上一次出库次数的基础上+1
                    {
                        num = list.Max(c => c.WarehouseOutNum) + 1;
                    }
                    //添加出库信息
                    foreach (var warehouse_Join in info)
                    {
                        if (isDelete && !string.IsNullOrEmpty(newoutherbarcode))
                        {
                            warehouse_Join.OuterBoxBarcode = newoutherbarcode;
                        }
                        warehouse_Join.IsOut = true;//true为出库
                        warehouse_Join.WarehouseOutDate = DateTime.Now;//现在时间
                        warehouse_Join.WarehouseOutOperator = ((Users)Session["User"]).UserName;//登录人
                        warehouse_Join.WarehouseOutNum = num;//出库次数
                        warehouse_Join.NewBarcode = warehousordernum;//出库订单,出库则是自身的订单号,挪用出库则是挪用的订单号
                        warehouse_Join.Transportation = Transportation;//运输方式
                        warehouse_Join.WarehouseOutDepartment = Department1;
                        warehouse_Join.WarehouseOutGroup = Group;
                        warehouse_Join.WarehouseOutDocuments = WarehouseOutDocuments;//出库单号
                        db.Entry(warehouse_Join).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }
            }
            else
            {
                foreach (var item in barcode)//循环前端传过来的外箱条码
                {
                    string newoutherbarcode = "";

                    if (isDelete)
                    {
                        var print = db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == item);//查找被挪用的外箱 里面的容量是多少
                        var basicInfo = db.Packing_BasicInfo.Where(c => c.OrderNum == warehousordernum && c.ScreenNum == screenNum && c.Batch == batchNum).ToList();//根据订单号查找包装规则信息
                        if (basicInfo.Count(c => c.OuterBoxCapacity == print) == 0)//查看包装类型里,有没有符合外箱条码的类型,没有则保存到errorordernum
                        {
                            errorordernum = errorordernum + item + ",";
                            continue;
                        }
                        var quantiy = basicInfo.Where(c => c.OuterBoxCapacity == print).Sum(c => c.Quantity);//定义的包装数量
                        string type = "1装" + print;
                        var completePrint = db.Packing_BarCodePrinting.Where(c => (c.CartonOrderNum == warehousordernum || c.EmbezzleOrderNum == warehousordernum) && c.Type == type && c.ScreenNum == screenNum && c.Batch == batchNum).Select(c => c.OuterBoxBarcode).Distinct().Count();//已经打印的数量
                        if (quantiy <= completePrint)//如果已经超过或者等于这个数量
                        {
                            errorordernum2 = errorordernum2 + item + ",";
                            continue;
                        }
                        //否则修改外箱标签信息
                        var printinfo = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == item).ToList();//外箱条码信息

                        var count = db.Packing_BasicInfo.Where(c => c.OrderNum == warehousordernum).Sum(c => c.Quantity);//挪用定义的总数量
                        #region 外箱条码生成
                        string[] str = warehousordernum.Split('-');//将订单号分割
                        string start = str[0].Substring(2);//取出 如2017-test-1 的17
                        string OuterBoxBarCodeNum = start + str[1] + "-" + str[2] + "-" + screenNum.ToString().PadLeft(2, '0') + "-" + batchNum.ToString().PadLeft(2, '0') + "-";//外箱条码组成 
                        for (int i = 1; i <= count; i++)//从1开始循环,最大数为定义的打印数,用来确定标签的序列号
                        {
                            var sequence = (OuterBoxBarCodeNum + i.ToString().PadLeft(3, '0'));//生成含有序列数的标签号
                            if (db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == sequence) == 0)//判断打印表里是否有相同的标签号,没有则将此标签号存入数据中,有则继续循环
                            {
                                newOutherBarcode.Add(sequence);
                                newoutherbarcode = sequence;
                                printinfo.ForEach(c => { c.OuterBoxBarcode = sequence; c.EmbezzleOrderNum = warehousordernum; c.ScreenNum = screenNum; c.SNTN = i.ToString(); c.IsLogo = isLogo; c.Batch = batchNum; });//修改标签信息
                                break;//找到符合的条码就跳出循环
                            }
                        }
                        #endregion
                    }
                    if (string.IsNullOrEmpty(errorordernum2) && string.IsNullOrEmpty(errorordernum2))
                    {
                        var info = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == item && c.IsOut == false).ToList();//已入库未出库列表
                        var ordernum = info.Select(c => c.CartonOrderNum).FirstOrDefault();//查找订单
                        if (!ordernumList.Contains(ordernum))
                        {
                            ordernumList.Add(ordernum);//记录要出库的订单列表
                        }
                        //查找上一次出库的次数，如果没有出过库，则为1
                        var num = 0;
                        var list = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == item && c.IsOut == true).ToList();
                        if (list.Count == 0)
                        { num = 1; }
                        else//否则在上一次出库次数的基础上+1
                        {
                            num = list.Max(c => c.WarehouseOutNum) + 1;
                        }
                        //添加出库信息
                        foreach (var warehouse_Join in info)
                        {
                            if (isDelete && !string.IsNullOrEmpty(newoutherbarcode))
                            {
                                warehouse_Join.OuterBoxBarcode = newoutherbarcode;
                            }
                            warehouse_Join.IsOut = true;//true为出库
                            warehouse_Join.WarehouseOutDate = DateTime.Now;//现在时间
                            warehouse_Join.WarehouseOutOperator = ((Users)Session["User"]).UserName;//登录人
                            warehouse_Join.WarehouseOutNum = num;//出库次数
                            warehouse_Join.NewBarcode = warehousordernum;//出库订单,出库则是自身的订单号,挪用出库则是挪用的订单号
                            warehouse_Join.Transportation = Transportation;//运输方式
                            warehouse_Join.WarehouseOutDepartment = Department1;
                            warehouse_Join.WarehouseOutGroup = Group;
                            warehouse_Join.WarehouseOutDocuments = WarehouseOutDocuments;//出库单号
                            db.Entry(warehouse_Join).State = EntityState.Modified;

                            db.SaveChanges();
                        }

                    }
                }
            }
            if (!string.IsNullOrEmpty(errorordernum))
            {
                JObject orderjobject = new JObject();
                orderjobject.Add("orderNum", null);//订单号
                orderjobject.Add("barcode", null);
                orderjobject.Add("warehousOutCount", null);
                orderjobject.Add("warehousCount", null);
                orderjobject.Add("notPrintCount", null);
                orderjobject.Add("message", "条码" + errorordernum + "与挪用订单定义" + screenNum + "屏序与" + batchNum + "批号的装箱类型不相符, 请联系相关人员进行修改");
                array.Add(orderjobject);
                total.Add("mes", array);
                total.Add("pass", false);
                total.Add("newOutherBarocode", newOutherBarcode);//挪用后新生成的外箱条码
            }
            else if (!string.IsNullOrEmpty(errorordernum2))
            {
                JObject orderjobject = new JObject();
                orderjobject.Add("orderNum", null);//订单号
                orderjobject.Add("barcode", null);
                orderjobject.Add("warehousOutCount", null);
                orderjobject.Add("warehousCount", null);
                orderjobject.Add("notPrintCount", null);
                orderjobject.Add("message", "条码" + errorordernum + "出库失败,原因是" + warehousordernum + "订单定义的包装数量已全部打印完了");
                array.Add(orderjobject);
                total.Add("mes", array);
                total.Add("pass", false);
                total.Add("newOutherBarocode", newOutherBarcode);//挪用后新生成的外箱条码

            }
            else
            {
                //订单出库显示信息
                foreach (var item in ordernumList)
                {
                    JObject orderjobject = new JObject();
                    orderjobject.Add("orderNum", item);//订单号
                    var warehouseList = commom.GetCurrentwarehousList(item);//获取本次周期的出入库信息
                    var warehouse = db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true && c.NewBarcode != item && c.State == "模组").Select(c => c.OuterBoxBarcode).Distinct().Count();//出库了的数量
                                                                                                                                                                                                     //计划包装数量
                    var planPrint = db.Packing_BasicInfo.Where(c => c.OrderNum == item).Count() == 0 ? 0 : db.Packing_BasicInfo.Where(c => c.OrderNum == item).Sum(c => c.Quantity);
                    //现在挪用出库要还能找到之前的信息,暂时去掉
                    //if (warehouse == planPrint && isDelete)//出库的数目等于计划包装数目，全部挪用完毕
                    //{
                    //    if (System.IO.File.Exists(@"D:\MES_Data\TemDate\ProductionControllerExcept.json") == true)
                    //    {
                    //        var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\ProductionControllerExcept.json");
                    //        var list = jsonstring.Split(',').ToList();
                    //        if (list.Contains(item))
                    //        {
                    //            list.Remove(item);
                    //            string[] array = list.ToArray();
                    //            jsonstring = string.Join(",", array);
                    //        }
                    //        System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\ProductionControllerExcept.json", jsonstring);
                    //    }
                    //    orderjobject.Add("barcode", "");
                    //    orderjobject.Add("warehousOutCount", "");
                    //    orderjobject.Add("warehousCount", "");
                    //    orderjobject.Add("notPrintCount", "");
                    //    orderjobject.Add("message", "订单已全部出库，包装记录清零");
                    //    total.Add(orderjobject);
                    //    //var rule = db.Packing_BasicInfo.Where(c => c.OrderNum == item).ToList();//不能删除装箱规则,因为就算是被挪用出库,也还要在产值看板查看
                    //    //db.Packing_BasicInfo.RemoveRange(rule);
                    //    //db.SaveChanges();
                    //    continue;
                    //}

                    //var outherBacode = db.Warehouse_Join.Where(c => c.OrderNum == item && barcode.Contains(c.OuterBoxBarcode)).Select(c => c.OuterBoxBarcode).Distinct().ToList();
                    JArray barcodes = new JArray();
                    barcodes.Add(barcode);
                    orderjobject.Add("barcode", barcodes);
                    //已包装数量
                    var printCount = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == item && c.QC_Operator == null).Select(c => c.OuterBoxBarcode).Distinct().Count();
                    //出库数量
                    var warhhousrout = warehouseList.Where(c => c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();
                    //入库数量
                    var warhhousr = warehouseList.Select(c => c.OuterBoxBarcode).Distinct().Count();

                    //已出库
                    orderjobject.Add("warehousOutCount", warhhousrout);
                    //剩下未出库数量
                    orderjobject.Add("warehousCount", warhhousr - warhhousrout);
                    //未包装数量
                    orderjobject.Add("notPrintCount", planPrint - printCount);
                    orderjobject.Add("message", "");
                    array.Add(orderjobject);
                }
                total.Add("mes", array);
                total.Add("pass", true);
                total.Add("newOutherBarocode", newOutherBarcode);//挪用后新生成的外箱条码
            }
            return Content(JsonConvert.SerializeObject(total));
        }

        public ActionResult WarehouseOut(List<string> barcode, string warehousordernum, bool isDelete, string Transportation, string WarehouseOutDocuments, string Department1, string Group, int screenNum = 1, int batchNum = 1, bool isInside = false, bool isLogo = true)
        {
            if (Session["User"] == null)//判断是否有登录,没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "outStockConfirm" });
            }
            JObject total = new JObject();
            JArray array = new JArray();
            List<string> ordernumList = new List<string>();
            string errorordernum = "";
            JArray newOutherBarcode = new JArray();
            int totalquantity = 0;
            int printcount = 0;
            if (isInside)//内箱出库
            {
                if (barcode[0].Substring(barcode[0].Length - 5, 2) == "MK")//模块不能选择内箱出库
                {
                    JObject orderjobject = new JObject();
                    orderjobject.Add("orderNum", null);//订单号
                    orderjobject.Add("barcode", null);
                    orderjobject.Add("warehousOutCount", null);
                    orderjobject.Add("warehousCount", null);
                    orderjobject.Add("notPrintCount", null);
                    orderjobject.Add("message", "模块不能选择内箱出库,请重新选择");
                    array.Add(orderjobject);
                    total.Add("mes", array);
                    total.Add("pass", false);
                    total.Add("newOutherBarocode", newOutherBarcode);//挪用后新生成的外箱条码
                    return Content(JsonConvert.SerializeObject(total));
                }
                string newoutherbarcode = "";
                if (isDelete)//是否是挪用,挪用要先判断挪用的定义数量有没有大于出库数量,而后再修改打印信息
                {
                    var OuterBoxBarcodelist = db.Packing_BarCodePrinting.Count(c => barcode.Contains(c.BarCodeNum));

                    var OuterBoxCapacity = db.Packing_BasicInfo.Where(c => c.OrderNum == warehousordernum && c.ScreenNum == screenNum && c.Batch == batchNum).Select(c => c.OuterBoxCapacity).ToList();
                    var quantity = db.Packing_BasicInfo.Where(c => c.OrderNum == warehousordernum && c.ScreenNum == screenNum && c.Batch == batchNum).Select(c => c.Quantity).ToList();
                    totalquantity = quantity.Count == 0 ? 0 : quantity.Sum();
                    var outerside = db.Packing_BarCodePrinting.Where(c => c.OrderNum == warehousordernum).Select(c => c.OuterBoxBarcode).ToList();
                    printcount = db.Packing_BarCodePrinting.Count(c => (c.CartonOrderNum == warehousordernum || c.EmbezzleOrderNum == warehousordernum) && c.ScreenNum == screenNum && c.Batch == batchNum);

                    var jobject = WarehouseOutItem(OuterBoxBarcodelist, OuterBoxCapacity, outerside, printcount, totalquantity, warehousordernum, screenNum, batchNum);
                    if (jobject["pass"].ToString() == "False")
                    {
                        errorordernum = errorordernum + jobject["message"].ToString();
                    }
                    if (jobject["pass"].ToString() == "True")
                    {
                        var updatelist = db.Packing_BarCodePrinting.Where(c => barcode.Contains(c.BarCodeNum)).ToList();
                        newoutherbarcode = jobject["message"].ToString();
                        var SNTN = jobject["SN"].ToString();
                        updatelist.ForEach(c => { c.OuterBoxBarcode = newoutherbarcode; c.EmbezzleOrderNum = warehousordernum; c.ScreenNum = screenNum; c.SNTN = SNTN; c.IsLogo = isLogo; c.Batch = batchNum; });//修改标签信息
                    }
                }
                if (string.IsNullOrEmpty(errorordernum))
                {
                    var info = db.Warehouse_Join.Where(c => barcode.Contains(c.BarCodeNum) && c.IsOut == false).ToList();//已入库未出库列表
                    var ordernum = info.Select(c => c.CartonOrderNum).FirstOrDefault();//查找订单
                    if (!ordernumList.Contains(ordernum))
                    {
                        ordernumList.Add(ordernum);//记录要出库的订单列表
                    }
                    //查找上一次出库的次数，如果没有出过库，则为1
                    var num = 0;
                    var list = db.Warehouse_Join.Where(c => barcode.Contains(c.BarCodeNum) && c.IsOut == true).ToList();
                    if (list.Count == 0)
                    { num = 1; }
                    else//否则在上一次出库次数的基础上+1
                    {
                        num = list.Max(c => c.WarehouseOutNum) + 1;
                    }
                    //添加出库信息
                    foreach (var warehouse_Join in info)
                    {
                        if (isDelete && !string.IsNullOrEmpty(newoutherbarcode))
                        {
                            warehouse_Join.OuterBoxBarcode = newoutherbarcode;
                        }
                        warehouse_Join.IsOut = true;//true为出库
                        warehouse_Join.WarehouseOutDate = DateTime.Now;//现在时间
                        warehouse_Join.WarehouseOutOperator = ((Users)Session["User"]).UserName;//登录人
                        warehouse_Join.WarehouseOutNum = num;//出库次数
                        warehouse_Join.NewBarcode = warehousordernum;//出库订单,出库则是自身的订单号,挪用出库则是挪用的订单号
                        warehouse_Join.Transportation = Transportation;//运输方式
                        warehouse_Join.WarehouseOutDepartment = Department1;
                        warehouse_Join.WarehouseOutGroup = Group;
                        warehouse_Join.WarehouseOutDocuments = WarehouseOutDocuments;//出库单号
                        db.Entry(warehouse_Join).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }
            }
            else
            {
                foreach (var item in barcode)//循环前端传过来的外箱条码
                {
                    string newoutherbarcode = "";

                    if (isDelete)
                    {
                        if (barcode[0].Substring(barcode[0].Length - 5, 2) == "MK")
                        {
                            var OuterBoxBarcodelist = db.ModuleOutsideTheBox.Count(c => c.OutsideBarcode == item);
                            var OuterBoxCapacity = db.ModulePackageRule.Where(c => c.OrderNum == warehousordernum && c.ScreenNum == screenNum && c.Batch == batchNum && c.Statue == "外箱").Select(c => c.OuterBoxCapacity).ToList();
                            var quantity = db.ModulePackageRule.Where(c => c.OrderNum == warehousordernum && c.ScreenNum == screenNum && c.Batch == batchNum && c.Statue == "外箱").Select(c => c.Quantity).ToList();
                            totalquantity = quantity.Count == 0 ? 0 : quantity.Sum();
                            var outerside = db.ModuleOutsideTheBox.Where(c => c.OrderNum == warehousordernum || c.EmbezzleOrdeNum == warehousordernum).Select(c => c.OutsideBarcode).ToList();
                            printcount = db.ModuleOutsideTheBox.Count(c => (c.OrderNum == warehousordernum || c.EmbezzleOrdeNum == warehousordernum) && c.ScreenNum == screenNum && c.Batch == batchNum);

                            var jobject = WarehouseOutItem(OuterBoxBarcodelist, OuterBoxCapacity, outerside, printcount, totalquantity, warehousordernum, screenNum, batchNum, true);
                            var aa = jobject["pass"];
                            var bb = jobject["pass"].ToString();
                            if (jobject["pass"].ToString() == "False")
                            {
                                errorordernum = errorordernum + jobject["message"].ToString();
                            }
                            if (jobject["pass"].ToString() == "True")
                            {
                                var updatelist = db.ModuleOutsideTheBox.Where(c => c.OutsideBarcode == item).ToList();
                                newoutherbarcode = jobject["message"].ToString();
                                var SNTN = int.Parse(jobject["SN"].ToString());
                                updatelist.ForEach(c => { c.OutsideBarcode = newoutherbarcode; c.EmbezzleOrdeNum = warehousordernum; c.ScreenNum = screenNum; c.SN = SNTN; c.IsLogo = isLogo; c.Batch = batchNum; });//修改标签信息
                            }
                        }
                        else
                        {
                            //查找外箱标签装了几个模组
                            var OuterBoxBarcodelist = db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == item);
                            //查找包装规则在同一屏序,批次定义了几个1装几
                            var OuterBoxCapacity = db.Packing_BasicInfo.Where(c => c.OrderNum == warehousordernum && c.ScreenNum == screenNum && c.Batch == batchNum).Select(c => c.OuterBoxCapacity).ToList();
                            //查找包装规则在同一屏序,批次定义了几箱
                            var quantity = db.Packing_BasicInfo.Where(c => c.OrderNum == warehousordernum && c.ScreenNum == screenNum && c.Batch == batchNum).Select(c => c.Quantity).ToList();
                            totalquantity = quantity.Count == 0 ? 0 : quantity.Sum();
                            //查找这个订单总共打印的外箱标签总列表,用于挪用生产新外箱条码,筛选里面缺号的,或者是往后补
                            var outerside = db.Packing_BarCodePrinting.Where(c => c.OrderNum == warehousordernum).Select(c => c.OuterBoxBarcode).ToList();
                            //查找同一屏序,批号总共打印了几箱,用于判断还能不能打印
                            printcount = db.Packing_BarCodePrinting.Count(c => (c.CartonOrderNum == warehousordernum || c.EmbezzleOrderNum == warehousordernum) && c.ScreenNum == screenNum && c.Batch == batchNum);

                            var jobject = WarehouseOutItem(OuterBoxBarcodelist, OuterBoxCapacity, outerside, printcount, totalquantity, warehousordernum, screenNum, batchNum);
                            if (jobject["pass"].ToString() == "False")
                            {
                                errorordernum = errorordernum + jobject["message"].ToString();
                            }
                            if (jobject["pass"].ToString() == "True")
                            {
                                var updatelist = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == item).ToList();
                                newoutherbarcode = jobject["message"].ToString();
                                var SNTN = jobject["SN"].ToString();
                                updatelist.ForEach(c => { c.OuterBoxBarcode = newoutherbarcode; c.EmbezzleOrderNum = warehousordernum; c.ScreenNum = screenNum; c.SNTN = SNTN; c.IsLogo = isLogo; c.Batch = batchNum; });//修改标签信息
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(errorordernum))
                    {
                        var info = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == item && c.IsOut == false).ToList();//已入库未出库列表
                        var ordernum = info.Select(c => c.CartonOrderNum).FirstOrDefault();//查找订单
                        if (!ordernumList.Contains(ordernum))
                        {
                            ordernumList.Add(ordernum);//记录要出库的订单列表
                        }
                        //查找上一次出库的次数，如果没有出过库，则为1
                        var num = 0;
                        var list = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == item && c.IsOut == true).ToList();
                        if (list.Count == 0)
                        { num = 1; }
                        else//否则在上一次出库次数的基础上+1
                        {
                            num = list.Max(c => c.WarehouseOutNum) + 1;
                        }
                        //添加出库信息
                        foreach (var warehouse_Join in info)
                        {
                            if (isDelete && !string.IsNullOrEmpty(newoutherbarcode))
                            {
                                warehouse_Join.OuterBoxBarcode = newoutherbarcode;
                            }
                            warehouse_Join.IsOut = true;//true为出库
                            warehouse_Join.WarehouseOutDate = DateTime.Now;//现在时间
                            warehouse_Join.WarehouseOutOperator = ((Users)Session["User"]).UserName;//登录人
                            warehouse_Join.WarehouseOutNum = num;//出库次数
                            warehouse_Join.NewBarcode = warehousordernum;//出库订单,出库则是自身的订单号,挪用出库则是挪用的订单号
                            warehouse_Join.Transportation = Transportation;//运输方式
                            warehouse_Join.WarehouseOutDepartment = Department1;
                            warehouse_Join.WarehouseOutGroup = Group;
                            warehouse_Join.WarehouseOutDocuments = WarehouseOutDocuments;//出库单号
                            db.Entry(warehouse_Join).State = EntityState.Modified;

                            db.SaveChanges();
                        }

                    }
                }
            }
            if (!string.IsNullOrEmpty(errorordernum))
            {
                JObject orderjobject = new JObject();
                orderjobject.Add("orderNum", null);//订单号
                orderjobject.Add("barcode", null);
                orderjobject.Add("warehousOutCount", null);
                orderjobject.Add("warehousCount", null);
                orderjobject.Add("notPrintCount", null);
                orderjobject.Add("message", errorordernum);
                array.Add(orderjobject);
                total.Add("mes", array);
                total.Add("pass", false);
                total.Add("newOutherBarocode", newOutherBarcode);//挪用后新生成的外箱条码
            }
            else
            {
                //订单出库显示信息
                foreach (var item in ordernumList)
                {
                    JObject orderjobject = new JObject();
                    orderjobject.Add("orderNum", item);//订单号
                    var warehouseList = commom.GetCurrentwarehousList(item);//获取本次周期的出入库信息
                    var warehouse = db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true && c.NewBarcode != item && c.State == "模组").Select(c => c.OuterBoxBarcode).Distinct().Count();//出库了的数量

                    //现在挪用出库要还能找到之前的信息,暂时去掉
                    JArray barcodes = new JArray();
                    barcodes.Add(barcode);
                    orderjobject.Add("barcode", barcodes);

                    //出库数量
                    var warhhousrout = warehouseList.Where(c => c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();
                    //入库数量
                    var warhhousr = warehouseList.Select(c => c.OuterBoxBarcode).Distinct().Count();

                    //已出库
                    orderjobject.Add("warehousOutCount", warhhousrout);
                    //剩下未出库数量
                    orderjobject.Add("warehousCount", warhhousr - warhhousrout);
                    //未包装数量
                    orderjobject.Add("notPrintCount", totalquantity - printcount);
                    orderjobject.Add("message", "");
                    array.Add(orderjobject);
                }
                total.Add("mes", array);
                total.Add("pass", true);
                total.Add("newOutherBarocode", newOutherBarcode);//挪用后新生成的外箱条码
            }
            return Content(JsonConvert.SerializeObject(total));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barcodelist">条码列表</param>
        /// <param name="OuterBoxCapacity">包装定义的件数</param>
        /// <param name="completePrint">总打印标签次数</param>
        /// <param name="totalquantiy">总定义标签箱数</param>
        /// <param name="warehousordernum"></param>
        /// <param name="screenNum"></param>
        /// <param name="batchNum"></param>
        public JObject WarehouseOutItem(int barcodelist, List<int> OuterBoxCapacity, List<string> outhreBarcodeList, int completePrint, int totalquantiy, string warehousordernum, int screenNum, int batchNum, bool Ismokuai = false)
        {
            JObject result = new JObject();

            //var print = barcodelist;//查找被挪用的外箱 里面的容量是多少
            //var basicInfo = db.Packing_BasicInfo.Where(c => c.OrderNum == warehousordernum && c.ScreenNum == screenNum && c.Batch == batchNum).ToList();//根据订单号查找包装规则信息
            if (OuterBoxCapacity.Count == 0 || !OuterBoxCapacity.Contains(barcodelist))//查看包装类型里,有没有符合外箱条码的类型,没有则保存到errorordernum
            {
                result.Add("pass", false);
                result.Add("message", warehousordernum + "没有定义1装" + barcodelist);
                return result;
            }
            if (totalquantiy <= completePrint)//如果已经超过或者等于这个数量
            {
                result.Add("pass", false);
                result.Add("message", warehousordernum + "已经打印了" + completePrint + "箱,定义的箱数是" + totalquantiy);
                return result;
            }

            #region 外箱条码生成
            string[] str = warehousordernum.Split('-');//将订单号分割
            string start = str[0].Substring(2);//取出 如2017-test-1 的17
            string OuterBoxBarCodeNum = "";
            if (Ismokuai)
            {
                OuterBoxBarCodeNum = start + str[1] + "-" + str[2] + "-" + screenNum.ToString().PadLeft(2, '0') + "-" + batchNum.ToString().PadLeft(2, '0') + "-MK";//外箱条码组成 
            }
            else
            {
                OuterBoxBarCodeNum = start + str[1] + "-" + str[2] + "-" + screenNum.ToString().PadLeft(2, '0') + "-" + batchNum.ToString().PadLeft(2, '0') + "-";//外箱条码组成 
            }
            for (int i = 1; i <= totalquantiy; i++)//从1开始循环,最大数为定义的打印数,用来确定标签的序列号
            {
                var sequence = (OuterBoxBarCodeNum + i.ToString().PadLeft(3, '0'));//生成含有序列数的标签号
                if (!outhreBarcodeList.Contains(sequence))//判断打印表里是否有相同的标签号,没有则将此标签号存入数据中,有则继续循环
                {
                    //newOutherBarcode.Add(sequence);
                    result.Add("pass", true);
                    result.Add("message", sequence);
                    result.Add("SN", i);
                    return result;
                }
            }
            result.Add("pass", false);
            result.Add("message", "找不到能生成的外箱列表");
            return result;
            #endregion
        }

        public void WarehouseOutItem2(List<string> barcodelist, bool isDelete, string newoutherbarcode, string warehousordernum, string Transportation, string Department1, string Group, string WarehouseOutDocuments)
        {
            List<string> ordernumList = new List<string>();
            var info = db.Warehouse_Join.Where(c => barcodelist.Contains(c.BarCodeNum) && c.IsOut == false).ToList();//已入库未出库列表
            var ordernum = info.Select(c => c.CartonOrderNum).FirstOrDefault();//查找订单
            if (!ordernumList.Contains(ordernum))
            {
                ordernumList.Add(ordernum);//记录要出库的订单列表
            }
            //查找上一次出库的次数，如果没有出过库，则为1
            var num = 0;
            var list = db.Warehouse_Join.Where(c => barcodelist.Contains(c.BarCodeNum) && c.IsOut == true).ToList();
            if (list.Count == 0)
            { num = 1; }
            else//否则在上一次出库次数的基础上+1
            {
                num = list.Max(c => c.WarehouseOutNum) + 1;
            }
            //添加出库信息
            foreach (var warehouse_Join in info)
            {
                if (isDelete && !string.IsNullOrEmpty(newoutherbarcode))
                {
                    warehouse_Join.OuterBoxBarcode = newoutherbarcode;
                }
                warehouse_Join.IsOut = true;//true为出库
                warehouse_Join.WarehouseOutDate = DateTime.Now;//现在时间
                warehouse_Join.WarehouseOutOperator = ((Users)Session["User"]).UserName;//登录人
                warehouse_Join.WarehouseOutNum = num;//出库次数
                warehouse_Join.NewBarcode = warehousordernum;//出库订单,出库则是自身的订单号,挪用出库则是挪用的订单号
                warehouse_Join.Transportation = Transportation;//运输方式
                warehouse_Join.WarehouseOutDepartment = Department1;
                warehouse_Join.WarehouseOutGroup = Group;
                warehouse_Join.WarehouseOutDocuments = WarehouseOutDocuments;//出库单号
                db.Entry(warehouse_Join).State = EntityState.Modified;

                db.SaveChanges();
            }
        }

        ///// <summary>
        ///// 检查挪用出库订单是否能挪用
        ///// </summary>
        ///// 检查订单是否创建了包装规则,创建规则里面的类型与外箱条码是否相同,并且是否分屏
        ///// <param name="ordernum">订单号</param>
        ///// <param name="outherbarcode">外箱条码</param>
        ///// <returns></returns>
        //public ActionResult CheackEmbezzleOrderNum(string ordernum, List<string> outherbarcode, int screen)
        //{
        //    JObject result = new JObject();
        //    var info = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.ScreenNum == screen).ToList();//根据订单号查找包装规则信息
        //    foreach (var item in outherbarcode)
        //    {
        //        var print = db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == item);//查找被挪用的外箱 里面的容量是多少

        //        if (info.Count(c => c.OuterBoxCapacity == print) == 0)//查看包装类型里,有没有符合外箱条码的类型,没有则返回错误
        //        {
        //            result.Add("result", false);//能否挪用
        //            result.Add("mes", "此挪用订单没有定义" + print + "装容量," + screen + "屏序的包装规则,请联系相关人员进行修改");//返回信息
        //            result.Add("Isscreen", false);//是否分屏
        //            result.Add("screennum", null);//分屏号列表
        //        }
        //        //else//包装规则能找到相应包装类型
        //        //{
        //        //    if (info.FirstOrDefault().IsSeparate == true)//分屏
        //        //    {
        //        //        result.Add("result", true);//能否挪用
        //        //        result.Add("mes", "成功");//返回信息
        //        //        var screem = info.Where(c => c.OuterBoxCapacity == print).Select(c => c.ScreenNum).ToList();//读取所有分屏号
        //        //        JArray array = new JArray();
        //        //        screem.ForEach(c => array.Add(c));//将分屏号保存到array中
        //        //        result.Add("Isscreen", true);//是否分屏
        //        //        result.Add("screennum", array);//分屏号列表
        //        //    }
        //        //    else//不分屏
        //        //    {
        //        //        result.Add("result", true);//能否挪用
        //        //        result.Add("mes", "成功");//返回信息
        //        //        result.Add("Isscreen", false);//是否分屏
        //        //        result.Add("screennum", null);//分屏号列表
        //        //    }

        //        //}
        //    }
        //    return Content(JsonConvert.SerializeObject(result));

        //}

        /// <summary>
        /// 根据订单号找到屏序列表和批号列表
        /// </summary>
        /// 判断订单号有没有建立包装规则,没有发挥错误,有则在查看是否有分屏,有分屏则返回分屏列表,没有则返回没有分屏信息和批号信息
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>

        public ActionResult GetEmbezzleOrderNumBatch(string ordernum)
        {
            JObject jObject = new JObject();
            var orders = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).ToList();//订单是否有挪用信息
            if (orders.Count == 0)//没有建立规则
            {
                jObject.Add("mes", "挪用订单没有建立规则,请联系相关人员");
                jObject.Add("haveBatch", false);
                jObject.Add("BatchList", null);
            }
            //else if (warehouser!=0)
            //{
            //    jObject.Add("mes", "挪用订单已有自身出入库信息,请确认订单正确,或联系人员删除出入库信息");
            //    jObject.Add("haveScreen", false);
            //    jObject.Add("ScreenList", null);
            //}
            else
            {
                if (orders.FirstOrDefault().IsBatch == false)//没有批号
                {
                    jObject.Add("mes", "没有分批");
                    jObject.Add("haveBatch", false);
                    jObject.Add("BatchList", null);
                }
                else//有分屏
                {
                    JArray result = new JArray();
                    var batch = orders.Select(c => c.Batch).Distinct();//查找批号列表
                    foreach (var item in batch)//循环拿到批号jarray 数据
                    {
                        JObject List = new JObject();
                        List.Add("value", item);

                        result.Add(List);
                    }
                    jObject.Add("mes", "成功");
                    jObject.Add("haveBatch", true);
                    jObject.Add("BatchList", result);
                }
            }
            return Content(JsonConvert.SerializeObject(jObject));
        }

        /// <summary>
        /// 根据订单号找到屏序列表和批号列表
        /// </summary>
        /// 判断订单号有没有建立包装规则,没有发挥错误,有则在查看是否有分屏,有分屏则返回分屏列表,没有则返回没有分屏信息和批号信息
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>

        public ActionResult GetEmbezzleOrderNumScreen(string ordernum, int batchnum)
        {
            JObject jObject = new JObject();
            var orders = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Batch == batchnum).ToList();//订单是否有挪用信息
            if (orders.Count == 0)//没有建立规则
            {
                jObject.Add("mes", "挪用订单没有建立规则,请联系相关人员");
                jObject.Add("haveScreen", false);
                jObject.Add("ScreenList", null);
            }
            //else if (warehouser!=0)
            //{
            //    jObject.Add("mes", "挪用订单已有自身出入库信息,请确认订单正确,或联系人员删除出入库信息");
            //    jObject.Add("haveScreen", false);
            //    jObject.Add("ScreenList", null);
            //}
            else
            {
                if (orders.FirstOrDefault().IsSeparate == false)//没有分屏
                {
                    jObject.Add("mes", "没有分屏");
                    jObject.Add("haveScreen", false);
                    jObject.Add("ScreenList", null);
                }
                else//有分屏
                {
                    JArray result = new JArray();
                    var screen = orders.Select(c => c.ScreenNum).Distinct();//查找分屏列表
                    foreach (var item in screen)//循环拿到分屏jarray 数据
                    {
                        JObject List = new JObject();
                        List.Add("value", item);

                        result.Add(List);
                    }
                    jObject.Add("mes", "成功");
                    jObject.Add("haveScreen", true);
                    jObject.Add("ScreenList", result);
                }
            }
            return Content(JsonConvert.SerializeObject(jObject));
        }

        #endregion
        //public ActionResult CheackBarcodeFromWarehouseOut(string barcode,bool isinnek,string ordernum=null)
        //{

        //}

        ///// <summary>
        ///// 返回挪用订单需要打印的参数信息
        ///// </summary>
        ///// <param name="ordernum">订单号</param>
        ///// <param name="outherBarcode">外箱条码列表</param>
        ///// <param name="screenNum">屏序号</param>
        ///// <returns></returns>
        //public ActionResult GetOutsideBoxLablePrintParameter(string ordernum, List<string> outherBarcode, int screenNum)
        //{
        //    JObject result = new JObject();
        //    JArray total = new JArray();
        //    var count = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).Sum(c => c.Quantity);
        //    foreach (var item in outherBarcode)
        //    {
        //        var info = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == item && c.EmbezzleOrderNum == ordernum).ToList();
        //        //result.Add("ordernum", ordernum);//订单,显示的订单
        //        //result.Add("screen", screenNum);//屏序号
        //        //result.Add("outheBarcode", item);//外箱条码
        //        result.Add("G_Weight", info.FirstOrDefault().G_Weight);//毛重
        //        result.Add("N_Weight", info.FirstOrDefault().N_Weight);//净重
        //        result.Add("Materiel", info.FirstOrDefault().Materiel);//物料描述
        //        result.Add("SN/TN", info.FirstOrDefault().SNTN + "/" + count);//件数/号
        //        result.Add("ModuleCount", info.Count);//数量
        //        var barcode = info.Select(c => new { c.BarCodeNum, c.ModuleGroupNum }).ToList();
        //        JArray ModuleList = new JArray();
        //        barcode.ForEach(c => ModuleList.Add(c.ModuleGroupNum != null ? c.ModuleGroupNum : c.BarCodeNum));// 如果模组号不为空则传模组号,否则条码号
        //        result.Add("ModuleList", ModuleList);//模组列表
        //        result.Add("IsLogo", info.FirstOrDefault().IsLogo);//是否有LOGO

        //        total.Add(result);
        //    }
        //    return Content(JsonConvert.SerializeObject(total)); ;
        //}

        #region 产值操作

        /// <summary>
        /// 创建产值
        /// </summary>
        /// 判断已有的产值表中是否有相同的数据,有则提示错误,无则添加
        /// <param name="value">产值数据组</param>
        /// <returns></returns>
        public string Procudtion_valueCrete(Production_Value value)
        {
            if (ModelState.IsValid)
            {

                var exit = db.Production_Value.Where(c => c.OrderNum == value.OrderNum).Select(c => c.OrderNum).FirstOrDefault();//查找产值表是否有相同的订单信息
                if (!string.IsNullOrEmpty(exit))//如果有,提示错误
                {
                    return exit + "订单已有产值记录，请确认订单";
                }
                value.CreateDate = DateTime.Now;
                db.Production_Value.Add(value);//否则添加数据
                db.SaveChanges();
                return "true";
            }
            return "false";
        }

        /// <summary>
        /// 历史记录查询
        /// </summary>
        /// <returns></returns>
        public ActionResult HistoryProduction_value()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            CommonalityController comm = new CommonalityController();
            JObject value = new JObject();
            JObject total = new JObject();
            var yesterday = DateTime.Now.AddDays(-1);//查找昨天的日期
            var productionOrder = db.Packing_BasicInfo.Select(c => c.OrderNum).Distinct().ToList();//查找定义了包装规则的所有订单
            int i = 0;
            foreach (var item in productionOrder)//循环订单
            {
                var productionvalue = db.Production_Value.Where(c => c.OrderNum == item).FirstOrDefault();//在产值表查找订单

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
                var quantity = db.Packing_BasicInfo.Where(c => c.OrderNum == item).Sum(c => c.Quantity);//总箱数
                //已包装数量
                var modulecount = db.Packing_BarCodePrinting.Count(c => c.CartonOrderNum == item && c.QC_Operator == null);//已打印的件数
                var outhercount = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == item && c.QC_Operator == null).Select(c => c.OuterBoxBarcode).Distinct().Count();//已打印的箱数

                value.Add("packingCount", modulecount + "(" + outhercount + "/" + quantity.ToString() + ")");

                #region 查找当前出入库的情况
                var temresult = commom.GetCurrentwarehousList(item);
                #endregion

                var warehousJoincount = temresult.Where(c => c.CartonOrderNum == item).Select(c => c.BarCodeNum).Distinct().Count();//已入库和已出库的件数
                var bigJoincount = temresult.Where(c => c.CartonOrderNum == item).Select(c => c.OuterBoxBarcode).Distinct().Count();//已入库和已出库的箱数
                var warehousOutcount = temresult.Where(c => c.IsOut == true && c.CartonOrderNum == item).Select(c => c.BarCodeNum).Distinct().Count();//已出库的件数
                var bigOutcount = temresult.Where(c => c.IsOut == true && c.CartonOrderNum == item).Select(c => c.OuterBoxBarcode).Distinct().Count();//已出库的箱数

                //已入库数量.Count(c => c.OrderNum == item)
                value.Add("warehousJoinCount", warehousJoincount + "(" + bigJoincount + "/" + quantity.ToString() + ")");
                //已出库数量
                //warehousOutcount = temresult.Where(c => ).Select(c => c.BarCodeNum).Distinct().Count();
                value.Add("warehousOutCount", warehousOutcount + "(" + bigOutcount + "/" + quantity.ToString() + ")");

                //库存数量
                //var outheCount = temresult.Where(c => c.OrderNum == item && c.Date != null).Select(c => c.OuterBoxBarcode).Distinct().Count() - temresult.Where(c => c.OrderNum == item && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();
                var stockCount = (warehousJoincount - warehousOutcount).ToString() + "(" + (bigJoincount - bigOutcount) + ")";
                value.Add("stockCount", stockCount);

                //挪用信息
                JArray nuoInfo = new JArray();
                var warehousenum = db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true && c.NewBarcode != null && c.NewBarcode != item && c.State == "模组").Select(c => c.WarehouseOutNum).Distinct().ToList();//查找出入库表里已出库,并且NewBarcode不等于自己订单的出库次数集合
                foreach (var num in warehousenum)
                {
                    var ordernum = db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true && c.NewBarcode != item && c.WarehouseOutNum == num && c.State == "模组").Select(c => new { c.NewBarcode, c.OuterBoxBarcode }).ToList();//将挪用出库信息筛选出来

                    nuoInfo.Add("第" + num + "次出库到" + ordernum.FirstOrDefault().NewBarcode + "订单" + ordernum.Select(c => c.OuterBoxBarcode).Distinct().Count() + "箱(" + ordernum.Count + "件);");//整理信息
                }

                value.Add("nuoInfo", nuoInfo);
                //入库完成率
                var current = temresult.Where(c => c.OrderNum == item).Select(c => c.BarCodeNum).Distinct().Count();
                //warehousJoincount = warehousJoincount - current;
                var warehousJoinComplete = (moduleCount == 0 ? 0 : (decimal)(current * 100) / moduleCount).ToString("F2") + "%";
                value.Add("warehousJoinComplete", warehousJoinComplete);
                //出库完成率
                //warehousOutcount = warehousOutcount - current;
                var currentout = temresult.Where(c => c.OrderNum == item && c.IsOut == true).Select(c => c.BarCodeNum).Distinct().Count();
                var warehousOutComplete = (moduleCount == 0 ? 0 : (decimal)(currentout * 100) / moduleCount).ToString("F2") + "%";
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

        /// <summary>
        /// 历史查询
        /// </summary>
        /// <param name="day1">起始时间</param>
        /// <param name="day2">结束时间</param>
        /// <param name="year">年</param>
        /// <param name="mouth">月</param>
        /// <param name="Ordernums">订单号</param>
        /// <returns></returns>
        public ActionResult CheckDateProduction_value(DateTime? day1, DateTime? day2, int? year, int? mouth, List<string> Ordernums)
        {
            JObject value = new JObject();
            JObject total = new JObject();
            var yesterday = DateTime.Now.AddDays(-1);//昨天的时间
            var odernumlist = new List<string>();
            if (Ordernums.Count != 0)//如果订单号不为空,则直接用传过来的订单号查询
            {
                //productionOrder = productionOrder.Where(c => c.OrderNum == Ordernums||c.NewBarcode==Ordernums).ToList();

                odernumlist.AddRange(Ordernums);
            }
            else
            {
                var productionOrder = db.Warehouse_Join.Where(c => c.State == "模组").ToList();
                if (day1 != null && day2 != null)//如果起始时间和结束时间都不为null
                {
                    if (day1 < day2)//如果day1 < day2,说明day1是比较前的时间 day2是比较后的时间
                        productionOrder = productionOrder.Where(c => c.Date > day1 && c.Date < day2).ToList();//查找时间段里面的出入库数据
                    else
                        productionOrder = productionOrder.Where(c => c.Date > day2 && c.Date < day1).ToList();//查找时间段里面的出入库数据

                }
                if (year != null)//如果年不为空,根据年来查找数据
                {
                    productionOrder = productionOrder.Where(c => c.Date != null && c.Date.Value.Year == year).ToList();
                }
                if (mouth != null)//如果月不为空,根据月来查找数据
                {
                    productionOrder = productionOrder.Where(c => c.Date != null && c.Date.Value.Month == mouth).ToList();
                }
                odernumlist = productionOrder.Select(c => c.OrderNum).Distinct().ToList();//最后筛选完的数据,把订单号提取出来
            }

            int i = 0;
            foreach (var item in odernumlist)//循环订单号
            {
                var productionvalue = db.Production_Value.Where(c => c.OrderNum == item).FirstOrDefault();

                //订单号
                value.Add("OrderNum", item);

                //模组数量
                var moduleCount = db.OrderMgm.Where(c => c.OrderNum == item).Select(c => c.Boxes).FirstOrDefault();
                value.Add("moduleCount", moduleCount);
                //包装件数
                var quantitycount = db.Packing_BasicInfo.Where(c => c.OrderNum == item).ToList();
                var quantity = 0;
                if (quantitycount.Count != 0)
                {
                    quantity = quantitycount.Sum(c => c.Quantity);
                }
                //已包装数量
                var modulecount = db.Packing_BarCodePrinting.Count(c => c.CartonOrderNum == item && c.QC_Operator == null);//已打印的件数
                var outhercount = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == item && c.QC_Operator == null).Select(c => c.OuterBoxBarcode).Distinct().Count();//已打印的箱数
                value.Add("packingCount", modulecount + "(" + outhercount + "/" + quantity.ToString() + ")");

                #region 查找当前出入库的情况
                var temresult = commom.GetCurrentwarehousList(item);
                #endregion

                var warehousJoincount = temresult.Where(c => c.CartonOrderNum == item || c.NewBarcode == item).Select(c => c.BarCodeNum).Distinct().Count();//已入库和已出库的件数
                var bigJoincount = temresult.Where(c => c.CartonOrderNum == item || c.NewBarcode == item).Select(c => c.OuterBoxBarcode).Distinct().Count();//已入库和已出库的箱数
                var warehousOutcount = temresult.Where(c => c.IsOut == true && c.CartonOrderNum == item || c.NewBarcode == item).Select(c => c.BarCodeNum).Distinct().Count();//已出库的件数
                var bigOutcount = temresult.Where(c => c.IsOut == true && c.CartonOrderNum == item || c.NewBarcode == item).Select(c => c.OuterBoxBarcode).Distinct().Count();//已出库的箱数

                //已入库数量.Count(c => c.OrderNum == item)
                value.Add("warehousJoinCount", warehousJoincount + "(" + bigJoincount + "/" + quantity.ToString() + ")");
                //已出库数量
                //warehousOutcount = temresult.Where(c => ).Select(c => c.BarCodeNum).Distinct().Count();
                value.Add("warehousOutCount", warehousOutcount + "(" + bigOutcount + "/" + quantity.ToString() + ")");

                //库存数量
                //var outheCount = temresult.Where(c => c.OrderNum == item && c.Date != null).Select(c => c.OuterBoxBarcode).Distinct().Count() - temresult.Where(c => c.OrderNum == item && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();
                var stockCount = (warehousJoincount - warehousOutcount).ToString() + "(" + (bigJoincount - bigOutcount) + ")";
                value.Add("stockCount", stockCount);

                //挪用信息
                JArray nuoInfo = new JArray();
                var NewBarcode = db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true && c.NewBarcode != null && c.NewBarcode != " " && c.NewBarcode != item && c.State == "模组").Select(c => c.NewBarcode).Distinct().ToList();
                foreach (var Barcode in NewBarcode)
                {
                    var ordernum1 = db.Warehouse_Join.Where(c => c.OrderNum == item && c.IsOut == true && c.NewBarcode == Barcode && c.State == "模组").Select(c => new { c.NewBarcode, c.OuterBoxBarcode }).ToList();
                    nuoInfo.Add("出库到" + ordernum1.FirstOrDefault().NewBarcode + "订单" + ordernum1.Select(c => c.OuterBoxBarcode).Distinct().Count() + "箱(" + ordernum1.Count + "件);");
                }
                var NewBarcode2 = db.Warehouse_Join.Where(c => c.NewBarcode == item && c.OrderNum != c.NewBarcode && c.IsOut == true && c.State == "模组").Select(c => c.OrderNum).Distinct().ToList();
                foreach (var Barcode in NewBarcode2)
                {
                    var ordernum1 = db.Warehouse_Join.Where(c => c.OrderNum == Barcode && c.IsOut == true && c.NewBarcode == item && c.State == "模组").Select(c => new { c.OrderNum, c.OuterBoxBarcode }).ToList();
                    nuoInfo.Add("从" + ordernum1.FirstOrDefault().OrderNum + "订单挪了" + ordernum1.Select(c => c.OuterBoxBarcode).Distinct().Count() + "箱(" + ordernum1.Count + "件);");
                }

                value.Add("nuoInfo", nuoInfo);
                //入库完成率
                var current = temresult.Where(c => c.OrderNum == item || c.NewBarcode == item).Select(c => c.BarCodeNum).Distinct().Count();
                //warehousJoincount = warehousJoincount - current;
                var warehousJoinComplete = (moduleCount == 0 ? 0 : (decimal)(current * 100) / moduleCount).ToString("F2") + "%";
                value.Add("warehousJoinComplete", warehousJoinComplete);
                //出库完成率
                //warehousOutcount = warehousOutcount - current;
                var currentout = temresult.Where(c => c.OrderNum == item && c.IsOut == true || c.NewBarcode == item).Select(c => c.BarCodeNum).Distinct().Count();
                var warehousOutComplete = (moduleCount == 0 ? 0 : (decimal)(currentout * 100) / moduleCount).ToString("F2") + "%";
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

        /// <summary>
        /// 修改产值记录
        /// </summary>
        /// 修改值
        /// <param name="value">产值数据集合</param>
        /// <returns></returns>
        public string UpdateProductionValue(Production_Value value)
        {
            if (ModelState.IsValid)//判断数据格式是否正确
            {
                db.Entry(value).State = EntityState.Modified;
                db.SaveChanges();//修改保存
                return "true";
            }

            else return "false";
        }

        /// <summary>
        /// 获得仓库外箱条码列表
        /// </summary>
        /// 根据订单号查找出入库表,找到外箱标签列表,显示出来
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
        public ActionResult GetOutherBarcode(string ordernum)
        {
            var orders = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.State == "模组").Select(m => m.OuterBoxBarcode).Distinct(); //根据订单号找出入库信息中外箱条码号
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //页面
        public ActionResult ViewDisplay(string ordernum)
        {
            ViewBag.ordernum = ordernum;
            return View();
        }
        /// <summary>
        /// 显示出入库信息
        /// </summary>
        /// 先查出本周期的出入库清单warehous_join.提取出外箱条码标签,循环,将出入库状态,时间,库位号,和条码+模组号(如果是混装的,则是混装订单号+条码+模组号),将信息传给前端
        /// <param name="ordernum">订单号</param>
        /// <param name="desc">是否降序</param>
        /// <returns></returns>
        public ActionResult DisplayWarehouse(string ordernum, bool desc = false)
        {
            ViewBag.outherbarcode = GetOutherBarcode(ordernum);
            JObject joinjobject = new JObject();
            JObject total = new JObject();
            var warehous_join = commom.GetCurrentwarehousList(ordernum);//本次周期出入库情况
            var barcode_join = warehous_join.Select(c => c.OuterBoxBarcode).Distinct().ToList();//查找外箱条码号列表
            int i = 0;
            if (desc == false)//是否是正序
            {
                barcode_join = barcode_join.OrderBy(c => c).ToList();
            }
            else//是否是倒序
            {
                barcode_join = barcode_join.OrderByDescending(c => c).ToList();
            }
            foreach (var join in barcode_join)//循环外箱条码列表
            {
                joinjobject.Add("otherBarcode", join);//外箱条码号

                var status = warehous_join.Where(c => c.OuterBoxBarcode == join).Select(c => c.IsOut).First();//查找对应的外箱的出入库情况
                joinjobject.Add("status", status == true ? "出库" : "入库");
                var jointime = warehous_join.Where(c => c.OuterBoxBarcode == join).Select(c => c.Date).FirstOrDefault();//入库时间
                var outtime = warehous_join.Where(c => c.OuterBoxBarcode == join).Select(c => c.WarehouseOutDate).FirstOrDefault();//出库时间
                joinjobject.Add("time", status == true ? outtime : jointime);//如果是出库状态则显示出库时间.如果是入库状态则显示入库时间
                var modulebarcode = warehous_join.Where(c => c.OuterBoxBarcode == join);
                var warehousenum = warehous_join.Where(c => c.OuterBoxBarcode == join).Select(c => c.WarehouseNum).FirstOrDefault();//库位号
                joinjobject.Add("warehousenum", warehousenum);
                JArray barcode = new JArray();
                foreach (var item in modulebarcode)//根据外箱条码,找到里面的条码的出入库信息
                {
                    if (item.CartonOrderNum != item.OrderNum)//符合条件表示混装
                    {
                        barcode.Add(item.OrderNum + "--" + item.BarCodeNum + ":" + item.ModuleGroupNum);//添加订单号
                    }
                    else
                    {
                        barcode.Add(item.BarCodeNum + ":" + item.ModuleGroupNum);//否则正常装,不加订单号
                    }
                }
                joinjobject.Add("codeList", barcode);
                total.Add(i.ToString(), joinjobject);
                joinjobject = new JObject();
                i++;
            }
            return Content(JsonConvert.SerializeObject(total));
        }

        /// <summary>
        /// 点击订单号显示具体信息的头
        /// </summary>
        /// 根据订单号从OrderMgm查模组数量,Packing_BasicInfo查找包装件数,Packing_BarCodePrinting查找已包装数量,查找本次周期出入库情况,根据本次周期出入库情况查找已入库数量,已出库数量,入库完成率,出库完成率,如果Production_Value表中有找到对应信心,则如实显示产值信息,否则显示"--"
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
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
            var packinglist = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum).ToList();
            var quantity = packinglist.Count > 0 ? packinglist.Sum(c => c.Quantity) : 0;
            //已包装数量
            var modulecount = db.Packing_BarCodePrinting.Count(c => c.CartonOrderNum == ordernum && c.QC_Operator == null);//已打印件数
            var outhercount = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == ordernum && c.QC_Operator == null).Select(c => c.OuterBoxBarcode).Distinct().Count();//已打印箱数
            value.Add("packingCount", modulecount + "(" + outhercount + "/" + quantity.ToString() + ")");


            #region 查找当前出入库的情况
            var temresult = commom.GetCurrentwarehousList(ordernum);
            #endregion

            var warehousJoincount = temresult.Where(c => c.CartonOrderNum == ordernum).Select(c => c.BarCodeNum).Distinct().Count();//已入库和已出库件数
            var bigJoincount = temresult.Where(c => c.CartonOrderNum == ordernum).Select(c => c.OuterBoxBarcode).Distinct().Count();//已入库和已出库箱数
            var warehousOutcount = temresult.Where(c => c.IsOut == true && c.CartonOrderNum == ordernum).Select(c => c.BarCodeNum).Distinct().Count();//已出库件数
            var bigOutcount = temresult.Where(c => c.IsOut == true && c.CartonOrderNum == ordernum).Select(c => c.OuterBoxBarcode).Distinct().Count();//已出库箱数

            //挪用信息
            JArray nuoInfo = new JArray();
            var NewBarcode = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.IsOut == true && c.NewBarcode != null && c.NewBarcode != " " && c.NewBarcode != ordernum && c.State == "模组").Select(c => c.NewBarcode).Distinct().ToList();
            foreach (var Barcode in NewBarcode)
            {
                var ordernum1 = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.IsOut == true && c.NewBarcode == Barcode && c.State == "模组").Select(c => new { c.NewBarcode, c.OuterBoxBarcode }).ToList();
                nuoInfo.Add("出库到" + ordernum1.FirstOrDefault().NewBarcode + "订单" + ordernum1.Select(c => c.OuterBoxBarcode).Distinct().Count() + "箱(" + ordernum1.Count + "件);");
            }
            var NewBarcode2 = db.Warehouse_Join.Where(c => c.NewBarcode == ordernum && c.OrderNum != c.NewBarcode && c.IsOut == true && c.State == "模组").Select(c => c.OrderNum).Distinct().ToList();
            foreach (var Barcode in NewBarcode2)
            {
                var ordernum1 = db.Warehouse_Join.Where(c => c.OrderNum == Barcode && c.IsOut == true && c.NewBarcode == ordernum && c.State == "模组").Select(c => new { c.OrderNum, c.OuterBoxBarcode }).ToList();
                nuoInfo.Add("从" + ordernum1.FirstOrDefault().OrderNum + "订单挪了" + ordernum1.Select(c => c.OuterBoxBarcode).Distinct().Count() + "箱(" + ordernum1.Count + "件);");
            }
            value.Add("nuoInfo", nuoInfo);

            //已入库数量.Count(c => c.OrderNum == item)
            value.Add("warehousJoinCount", warehousJoincount + "(" + bigJoincount + "/" + quantity.ToString() + ")");
            //已出库数量
            //warehousOutcount = temresult.Where(c => ).Select(c => c.BarCodeNum).Distinct().Count();
            value.Add("warehousOutCount", warehousOutcount + "(" + bigOutcount + "/" + quantity.ToString() + ")");

            //入库完成率
            var current = temresult.Where(c => c.OrderNum == ordernum).Select(c => c.BarCodeNum).Distinct().Count();
            //warehousJoincount = warehousJoincount - current;
            var warehousJoinComplete = (moduleCount == 0 ? 0 : (decimal)(current * 100) / moduleCount).ToString("F2") + "%";
            value.Add("warehousJoinComplete", warehousJoinComplete);
            //出库完成率
            //warehousOutcount = warehousOutcount - current;
            var currentout = temresult.Where(c => c.OrderNum == ordernum && c.IsOut == true).Select(c => c.BarCodeNum).Distinct().Count();
            var warehousOutComplete = (moduleCount == 0 ? 0 : (decimal)(currentout * 100) / moduleCount).ToString("F2") + "%";
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



        /// <summary>
        /// 获取折线数据
        /// </summary>
        /// 根据年份找到出入库表的信息,然后循环每个月,从1月开始到12月,查找每个月数据中的订单集合,循环订单,根据订单找到产值表的信息,如果找不到产值表信息,则跳过,否则就找到此月份此订单的出入库数量,在计算数量产值(出入库数量*(总产值/订单定义箱数)),将值赋给sum,得到每个月份的总产值
        /// <param name="year">年份</param>
        /// <returns></returns>
        public ActionResult BrokenlineMessage(int year)
        {
            JObject message = new JObject();
            var num = db.Warehouse_Join.Where(c => c.Date.Value.Year == year && c.State == "模组").ToList();//根据年份找到出入库表的信息

            for (int i = 1; i < 13; i++)//循环每个月,从1月开始到12月
            {
                decimal sum = 0;
                var month = num.Where(c => c.Date.Value.Month == i).Select(c => c.OrderNum).Distinct().ToList();//查找每个月数据中的订单集合
                foreach (var item in month)//循环订单
                {
                    var productionvalue = db.Production_Value.Where(c => c.OrderNum == item).FirstOrDefault();//根据订单找到产值表的信息
                    if (productionvalue != null)//如果找不到产值表信息,则跳过
                    {
                        var warehousJoinCount = db.Warehouse_Join.Count(c => c.OrderNum == item && c.State == "模组");//此月份此订单的出入库数量
                        var moduleCount = db.OrderMgm.Where(c => c.OrderNum == item).Select(c => c.Boxes).FirstOrDefault();//订单定义总数量
                        var warehouseJoinValue = warehousJoinCount * (moduleCount == 0 ? 0 : productionvalue.Worth / moduleCount);//计算产值
                        sum = sum + warehouseJoinValue;
                    }

                }
                message.Add(i.ToString(), sum.ToString("F2"));
            }
            return Content(JsonConvert.SerializeObject(message));
        }


        /// <summary>
        /// 显示未包装列表
        /// </summary>
        /// 先找未开始包装列表,从BarCodes表中根据订单找到所有条码号,剔除掉已经打印的所有条码号,得到未开始包装列表,列表为0返回null
        /// 未入库列表,根据订单找到打印的所有外箱条码号,剔除掉出库入库的外箱条码号,得到未入库列表,列表为0返回null
        /// 未出库列表,找到出入库表中未出库的外箱条码号,列表为0返回null
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
        public ActionResult DisplayNotPackingList(string ordernum)
        {
            JObject result = new JObject();
            var barcodeList = db.BarCodes.Where(c => c.OrderNum == ordernum && c.BarCodeType == "模组").Select(c => c.BarCodesNum).Distinct().ToList();//从BarCodes表中根据订单找到所有条码号
            var packgeList = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum && c.QC_Operator == null).Select(c => c.BarCodeNum).Distinct().ToList();//根据订单找到打印的所有条码号
            var packgeOtutherList = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum && c.QC_Operator == null).Select(c => c.OuterBoxBarcode).Distinct().ToList();//根据订单找到打印的所有外箱条码号
            var warehousjoin = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.State == "模组").Select(c => c.OuterBoxBarcode).Distinct().ToList();//根据订单找到出入库的外箱条码号
            var NotWarehousout = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.CartonOrderNum == ordernum & c.IsOut == false && c.State == "模组").Select(c => c.OuterBoxBarcode).Distinct().ToList();//根据订单找到入库条码号
            var NotPacking = barcodeList.Except(packgeList).OrderBy(c => c).ToList();//未开始包装列表
            var NotWarehousjoin = packgeOtutherList.Except(warehousjoin).OrderBy(c => c).ToList();//未入库列表
            if (NotPacking.Count > 0)//未开始包装
            {
                List<string> temp = new List<string>();
                NotPacking.ForEach(c =>
                {
                    var module = db.BarCodes.Where(a => a.BarCodesNum == c).Select(a => a.ModuleGroupNum).FirstOrDefault();
                    temp.Add(c + ":" + module);
                });
                result.Add("NotPackingarray", JsonConvert.SerializeObject(temp));
            }
            else
            {
                result.Add("NotPackingarray", null);
            }
            if (NotWarehousjoin.Count > 0)//未入库
            {
                result.Add("NotWarehousjoinarray", JsonConvert.SerializeObject(NotWarehousjoin));
            }
            else
            {
                result.Add("NotWarehousjoinarray", null);
            }
            if (NotWarehousout.Count > 0)//未出库
            {
                result.Add("NotWarehousoutarray", JsonConvert.SerializeObject(NotWarehousout));
            }
            else
            {
                result.Add("NotWarehousoutarray", null);
            }
            return Content(JsonConvert.SerializeObject(result));

        }
        #endregion

        #region 列表获取
        //查找OrderMgm表的订单列表
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

        /// <summary>
        /// 查找模组号规则中的json文件集合
        /// </summary>
        /// <returns></returns>
        public ActionResult GetJosnOrderList()
        {
            DirectoryInfo directory = new DirectoryInfo(@"D:\MES_Data\TemDate\OrderSequence\");
            var fullfile = directory.GetFileSystemInfos();
            JArray result = new JArray();
            foreach (var item in fullfile)
            {
                JObject List = new JObject();
                string name = System.IO.Path.GetFileNameWithoutExtension(item.FullName);
                if (name == "delete")
                { continue; }
                List.Add("value", name);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //根据订单查找装箱规则中的所有类型集合
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

        /// <summary>
        /// 返回分屏号
        /// </summary>
        /// 先判断是否分屏,返回前端,在列出分屏号清单
        /// <param name="ordernum">订单</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public ActionResult GetScreenNum(string ordernum, string type, int batchNum)
        {
            JObject message = new JObject();
            var count = db.Packing_BasicInfo.Count(c => c.OrderNum == ordernum && c.IsSeparate == true);//是否分屏
            if (count > 0)
                message.Add("IsScreen", true);
            else
                message.Add("IsScreen", false);

            var orders = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == type && c.Batch == batchNum).Select(m => m.ScreenNum).Distinct();    //分屏号清单
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

        public ActionResult GetBatchNum(string ordernum, string type)
        {
            JObject message = new JObject();
            var count = db.Packing_BasicInfo.Count(c => c.OrderNum == ordernum && c.IsBatch == true);//是否分批
            if (count > 0)
                message.Add("IsBatch", true);
            else
                message.Add("IsBatch", false);

            var orders = db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.Type == type).Select(m => m.Batch).Distinct();    //分屏号清单
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
        #endregion

        #region 模组规则

        //页面
        public ActionResult RuleEnter()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Packagings", act = "RuleEnter" });
            }
            return View();
        }

        /// <summary>
        /// 订单输入规则前判断
        /// </summary>
        /// 可以添加(canAdd=true) 可以删除(candelete=true)
        /// 1.先判断是否有对应订单的json文件,没有则,在判断是否有外观电检记录,有记录提示不能添加,否则再判断是否有校正记录,有校正记录则提示不能添加,以上两个都没有记录,返回能添加
        /// 判断有对应的json文件,则判断是否含有delete.json,如果没有delete.json,则提示可以删除,不能添加.如果有delete.json文件,判断delete.json文件里有没有对应的订单号,有则显示"此订单需等外观OQC确认删除，如有疑问请联系OQC人员",否则就提示可以删除,不能添加
        /// <param name="ordenum">订单号</param>
        /// <returns></returns>
        public ActionResult CheckRule(string ordenum)
        {
            JObject result = new JObject();
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json") == true)//是否有对应的json文件
            {
                if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\delete.json") == true)//是否有delete.json文件
                {
                    var deletejson = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json");
                    var jarray = JsonConvert.DeserializeObject<JArray>(deletejson).ToList();//读取delete.json文件的内容
                    if (jarray.Contains(ordenum))//查看 内容是否含有订单号
                    {
                        result.Add("candelete", false);
                        result.Add("canAdd", false);
                        result.Add("mesage", "此订单需等外观OQC确认删除，如有疑问请联系OQC人员");
                        return Content(JsonConvert.SerializeObject(result));
                    }
                }
                //var manualjson = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json");
                //var manualjarray = JsonConvert.DeserializeObject<JObject>(manualjson);

                result.Add("candelete", true);
                result.Add("canAdd", false);
                result.Add("mesage", "此订单已有模组规则，如需重新录入，请先删除规则!");
                return Content(JsonConvert.SerializeObject(result));

            }
            var appearance = db.Appearance.Count(c => c.OrderNum == ordenum && c.OQCCheckFT != null && (c.OldOrderNum == null || c.OldOrderNum == ordenum));//查看是否有电检记录
            if (appearance > 0)
            {
                result.Add("candelete", false);
                result.Add("canAdd", false);
                result.Add("mesage", "此订单已有外观电检记录!");
                return Content(JsonConvert.SerializeObject(result));
            }

            var cab = db.CalibrationRecord.Where(c => c.OrderNum == ordenum && (c.OldOrderNum == null || c.OldOrderNum == ordenum)).Select(c => c.ModuleGroupNum).ToList();//查看是否有校正记录
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

        /// <summary>
        /// 没有模组条码规则输入
        /// </summary>
        /// //判断路径D:\MES_Data\TemDate\OrderSequence有没有文件夹,没有就创建,循环规则类型数据,根据规则类型数据创建模组号,并存入json文件中
        /// <param name="sequences">规则类型数据</param>
        /// <param name="ordenum">订单号</param>
        public void SetJsonFile(List<Sequence> sequences, string ordenum)
        {
            JArray number = new JArray();
            JObject normal = new JObject();
            if (Directory.Exists(@"D:\MES_Data\TemDate\OrderSequence") == false)//如果不存在就创建订单文件夹
            {
                Directory.CreateDirectory(@"D:\MES_Data\TemDate\OrderSequence");
            }
            foreach (var item in sequences)//循环类型数据
            {
                for (int i = 0; i < item.Num; i++)//循环规则号定义数量
                {
                    var serial = "";
                    if (item.Rule)//判断是否补零,true为补零
                    {
                        var num = item.Num + item.startNum - 1; //找到最大数,如数量为91,起始值为10,10+91-1=100,即最大位数为3位,规则里的序号为010,011,012,013
                        serial = (item.startNum + i).ToString().PadLeft(num.ToString().Length, '0');
                    }
                    else
                    {
                        serial = (item.startNum + i).ToString(); //不补零,直接输出,如上面的例子 10,11,12,13
                    }
                    string seq = item.Prefix + serial + item.Suffix;
                    number.Add(seq);
                }
            }
            normal.Add("Normal", number);

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(normal, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json", output);//将数据存入json文件中

            //填写日志
            UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "创建订单号" + ordenum + "规则,选择自动录入" };
            db.UserOperateLog.Add(log);
            db.SaveChanges();

        }

        /// <summary>
        /// 选出特殊的模组号
        /// </summary>
        /// 判断有没有对应的json文件,没有就返回false,有的话,读取json文件内容,先看其中的Special是否为空,为空则直接把Normal里面选出来的特殊模组号去掉,移到Special里.
        /// Special不为空,如果传进来的num是空的,则把Special的数据都移到Normal,如果num不为空,将不包含在Special里面的不分模组移到Normal,再将包含在Normal里面的部分模组移到Special里
        /// <param name="ordenum">订单</param>
        /// <param name="num">选出来特殊的模组号</param>
        /// <returns></returns>
        public bool SetSpecialNum(string ordenum, List<string> num)
        {

            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json") == true)//判断有没有对应的json文件
            {
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json");
                var json = JsonConvert.DeserializeObject<JObject>(jsonstring);//读取json内容


                if (json.Property("Special") != null)//判断是否有Special
                {
                    ////List<JToken> spe = new List<JToken>();
                    List<JToken> normaladd = new List<JToken>();
                    List<JToken> normaldelete = new List<JToken>();
                    var special = (JArray)json["Special"];
                    var normal = (JArray)json["Normal"];

                    foreach (var item in special)//循环Special
                    {
                        if (num == null)//如果num为空.直接把Special的内容移到Normal
                        {
                            normaladd.Add(item);
                        }
                        else if (!num.Contains(item.ToString()))//将Special和num 的相差部分移给 Normal
                            normaladd.Add(item);
                    }
                    foreach (var item in normal)//循环Normal
                    {
                        if (num == null)
                        {
                            continue;
                        }
                        if (num.Contains(item.ToString())) //将Normal和num的并集部门移给sepcial
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

                    foreach (var item in jarray)//循环Normal
                    {
                        if (num.Contains(item.ToString()))
                        {
                            sepcial.Add(item);
                            delete.Add(item);
                        }
                    }

                    delete.ForEach(c => c.Remove());//将num移除Normal,移到Special
                    json.Add("Special", sepcial);

                }
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json", output);//保存到json文件
                return true;
            }
            return false;
        }

        /// <summary>
        /// 查询订单的模组号规则
        /// </summary>
        /// 在BarCodes根据条码找到模组号,判断模组号是否为空并且selectModule是否为true,都为真则返回找出来的模组号.
        /// 否则查找是否有对应的json文件,如果有,在找delete.json文件里有没有含有传过来的订单号,如果有,返回空模组号,没有就跳过.如果不含有delete.json文件,则找ordenum.json文件里是否有isManual,这代表手工,现在没用,有手工,返回空模组号,没有手工,则读取Normal的模组号,如果Normal为空,返回空模组号,如果reverse为true,返回Normal最后面的一个,否则返回Normal最前面的一个.
        /// 如果没找到对应放json文件,直接返回空模组号.moduleList是能选的模组号列表
        /// <param name="ordenum">订单号</param>
        /// <param name="barcode">条码号</param>
        /// <param name="reverse">是否倒序</param>
        /// <param name="selectModule">是否选择原模组号</param>
        /// <returns></returns>
        public ActionResult SelectModule(string ordenum, string barcode = null, bool reverse = true, bool selectModule = true)
        {
            JObject result = new JObject();
            var module = db.BarCodes.Where(c => c.BarCodesNum == barcode).Select(c => c.ModuleGroupNum).FirstOrDefault();
            if (!string.IsNullOrEmpty(module) && selectModule)//判断模组号是否为空并且selectModule是否为true,都为真则返回找出来的模组号
            {
                result.Add("mudule", module);
                result.Add("moduleList", null);
                return Content(JsonConvert.SerializeObject(result));
            }

            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json") == true)//判断是否有对应的json文件
            {
                if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\delete.json") == true)//判断是否有delete.json文件
                {
                    var deletejson = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json");
                    var jarray1 = JsonConvert.DeserializeObject<JArray>(deletejson).ToList();
                    if (jarray1.Contains(ordenum))//判断delete文件是否含有传过来的订单号
                    {
                        result.Add("mudule", "");
                        result.Add("moduleList", null);
                        return Content(JsonConvert.SerializeObject(result));
                    }
                }
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json");
                var json = JsonConvert.DeserializeObject<JObject>(jsonstring);//读取数据
                if (json.Property("isManual") != null)//是否手工
                {
                    result.Add("mudule", "");
                    result.Add("moduleList", null);
                    return Content(JsonConvert.SerializeObject(result));
                }
                var jarray = json["Normal"];//读取Normal数据
                JToken mudule = null;

                if (jarray.Count() == 0) //Normal数据是否为空
                {
                    result.Add("mudule", "");
                    result.Add("moduleList", null);
                    return Content(JsonConvert.SerializeObject(result));
                }
                if (reverse)//是否倒序
                {
                    int index = jarray.Count();
                    mudule = jarray[index - 1];
                }
                else//正序
                {
                    mudule = jarray[0];
                }
                result.Add("mudule", mudule.ToString());//返回模组号

                JArray modulelist = new JArray();
                foreach (var i in jarray)
                {
                    JObject jobj = new JObject();
                    jobj.Add("value", i);
                    modulelist.Add(jobj);
                }
                result.Add("moduleList", modulelist);
                return Content(JsonConvert.SerializeObject(result));

            }
            result.Add("mudule", "");
            result.Add("moduleList", null);
            return Content(JsonConvert.SerializeObject(result));
        }
        /*
        //根据订单号，显示已使用和未使用的模组号
        //public ActionResult SetModule(string ordenum)
        //{
        //    JObject info = new JObject();
        //    JArray notuse = new JArray();
        //    JArray use = new JArray();
        //    if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json") == true)
        //    {

        //        var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json");
        //        var json = JsonConvert.DeserializeObject<JObject>(jsonstring);
        //        var jarray = json["Normal"].ToList();
        //        notuse.Add(jarray);
        //        if (json.Property("Special") != null)
        //        {
        //            var jarray1 = json["Special"].ToList();
        //            notuse.Add(jarray1);
        //        }
        //        info.Add("notuse", notuse);

        //        //已使用

        //        var moduleList = db.Appearance.Where(c => c.OrderNum == ordenum && (c.OldOrderNum == null || c.OldOrderNum == ordenum)).Select(c => c.ModuleGroupNum).Distinct().ToList();
        //        use.Add(moduleList);
        //        info.Add("use", use);
        //        return Content(JsonConvert.SerializeObject(info));
        //    }
        //    else
        //    {
        //        var cab = db.CalibrationRecord.Where(c => c.OrderNum == ordenum && (c.OldOrderNum == null || c.OldOrderNum == ordenum)).Select(c => c.ModuleGroupNum).Distinct().ToList();
        //        var moduleList = db.Appearance.Where(c => c.OrderNum == ordenum && (c.OldOrderNum == null || c.OldOrderNum == ordenum)).Select(c => c.ModuleGroupNum).Distinct().ToList();
        //        if (cab == null)
        //        {
        //            return null;
        //        }
        //        var except = cab.Except(moduleList).ToList();
        //        notuse.Add(except);
        //        info.Add("notuse", notuse);

        //        use.Add(moduleList);
        //        info.Add("use", use);
        //        return Content(JsonConvert.SerializeObject(info));
        //    }

        //}
        */

        /// <summary>
        /// 删除规则前提示
        /// </summary>
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
        public string Tips(string ordernum)
        {
            string mesage = "";
            //入库记录
            var warehouecount = commom.GetCurrentwarehousList(ordernum).Count();
            if (warehouecount > 0)
            { mesage = mesage + "入库有" + warehouecount + "条记录，"; }

            //外箱标签
            var pritxount = db.Packing_BarCodePrinting.Count(c => c.CartonOrderNum == ordernum && c.QC_Operator == null);
            if (pritxount > 0)
            { mesage = mesage + "外箱标签有" + pritxount + "条记录，"; }

            //内箱记录
            var innercount = db.Packing_InnerCheck.Count(c => c.OrderNum == ordernum);
            if (innercount > 0)
            { mesage = mesage + "内箱确认有" + innercount + "条记录，"; }

            //电检记录
            var appercount = db.Appearance.Count(c => c.OrderNum == ordernum && c.OQCCheckFT != null && (c.OldOrderNum == null || c.OldOrderNum == ordernum));
            if (appercount > 0)
            { mesage = mesage + "电检完成有" + appercount + "条记录，"; }

            if (string.IsNullOrEmpty(mesage))
            {
                mesage = "此订单没有使用模组号记录";
            }
            return mesage;
        }

        /// <summary>
        /// 删除规则
        /// </summary>
        /// 如果存在delete.json,读取delete.json文件内容,并把新删除的订单号加进去,并保存,如果没有delete.json文件,则新建一个并把刚才的内容加进去
        /// <param name="ordernum">订单号</param>
        public void DeleteMoudueRuleAsync(string ordernum)
        {
            //if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordernum + ".json") == true)
            //{
            //    var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordernum + ".json");
            //    var json = JsonConvert.DeserializeObject<JObject>(jsonstring);
            //    json.Add("delete", 1);
            //    string output = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
            //    System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordernum + ".json", output);

            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\delete.json") != true)//没有delete.json文件
            {
                JArray deletevalue = new JArray();
                deletevalue.Add(ordernum);//新建对象.吧订单号加进去
                string deletestringFirst = Newtonsoft.Json.JsonConvert.SerializeObject(deletevalue, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json", deletestringFirst);//保存
            }
            else//有delete.json文件
            {
                var deletejson = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json");
                var jarray = JsonConvert.DeserializeObject<JArray>(deletejson);//读取数据
                jarray.Add(ordernum);//将内容加进去
                string deletestring = Newtonsoft.Json.JsonConvert.SerializeObject(jarray, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json", deletestring);//保存
            }

        }

        /// <summary>
        /// 查看模组号的使用情况
        /// </summary>
        /// 判断传过来的订单号列表是否有值,没有值就把D:\MES_Data\TemDate\OrderSequence\目录里除了delete.json的所有订单json文件的文件名放到namelist,订单号列表有值,就直接把订单列表放到namelist中.循环namelist.从电检表里找到已使用的模组号,再从json文件中找到未使用的模组号,如果json文件中含有isManual,表示手工录入,不含有则把Noraml和Special的模组号传给前面 .找到未使用模组号的条码列表(用条码表找的条码列表剔除电检表已经使用的条码列表),返回前面
        /// <param name="ordernum">订单号列表</param>
        /// <returns></returns>
        public ActionResult DiaplayMuduleUserMessage(List<string> ordernum)
        {
            DirectoryInfo directory = new DirectoryInfo(@"D:\MES_Data\TemDate\OrderSequence\");
            JArray total = new JArray();
            List<string> namelist = new List<string>();
            if (ordernum != null && ordernum.Count != 0)//ordernum有值,直接赋给namelist
            {
                namelist = ordernum;
            }
            else//没值,直接从本地json文件中找到所有json文件名.赋给namelist
            {
                foreach (var item in directory.GetFileSystemInfos())
                {
                    string name = System.IO.Path.GetFileNameWithoutExtension(item.FullName);
                    if (name == "delete")//除去delete
                    { continue; }
                    namelist.Add(name);
                }
            }
            foreach (var name in namelist)//循环namelist
            {
                JObject file = new JObject();
                file.Add("ordernum", name);//订单号

                //已使用的模组号列表
                var appearances = db.Appearance.OrderBy(c => c.ModuleGroupNum).Where(c => c.OrderNum == name && c.ModuleGroupNum != null && (c.OldOrderNum == null || c.OldOrderNum == name)).Select(c => new { c.BarCodesNum, c.ModuleGroupNum }).ToList();//从电检表找已使用的模组哈
                JArray module = new JArray();
                foreach (var app in appearances)
                {
                    JObject item = new JObject();
                    item.Add("barcode", app.BarCodesNum);//条码号
                    item.Add("module", app.ModuleGroupNum);//模组号
                    module.Add(item);
                }

                file.Add("user", module);

                //未使用
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + name + ".json");
                var json = JsonConvert.DeserializeObject<JObject>(jsonstring);//读取json文件数据
                JObject not = new JObject();
                JArray notuser = new JArray();
                if (json.Property("isManual") != null)//手工录入
                {
                    notuser.Add("手工录入");
                }
                else
                {
                    var jarray = json["Normal"].ToList();
                    notuser.Add(jarray);//将Normal的模组号放入
                    if (json.Property("Special") != null)
                    {
                        var jarray1 = json["Special"].ToList();
                        notuser.Add(jarray1);//将Special模组号放入
                    }
                }
                var barcodelist = db.BarCodes.OrderBy(c => c.BarCodesNum).Where(c => c.OrderNum == name).Select(c => c.BarCodesNum).ToList();//根据订单找到条码列表
                var notuserbarcodelist = barcodelist.Except(appearances.Select(c => c.BarCodesNum).ToList()).ToList();//找到未使用模组号的条码列表
                JArray barcodeArr = new JArray();
                foreach (var bar in notuserbarcodelist)
                {
                    barcodeArr.Add(bar);
                }
                not.Add("module", notuser);//已使用模组号条码列表
                //not.Add("barcode", JsonConvert.SerializeObject(notuserbarcodelist));
                not.Add("barcode", barcodeArr);//未使用模组号条码列表
                JArray notArr = new JArray();
                notArr.Add(not);
                file.Add("notuser", notArr);

                file.Add("count", barcodelist.Count);//条码总数量
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

        #region---查看重打印外箱标签
        //根据订单输出外箱条码号清单
        [HttpPost]
        public async Task<ActionResult> OutputOutsideBoxBarCodeNumList(string ordernum)
        {
            var OutsideBoxBarCodeNumList = await db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == ordernum || c.EmbezzleOrderNum == ordernum).Select(c => c.OuterBoxBarcode).Distinct().ToListAsync();
            List<string> result = new List<string>();
            foreach (var item in OutsideBoxBarCodeNumList)
            {
                result.Add(item);
            }
            return Content(JsonConvert.SerializeObject(result));
        }


        //根据订单输出可删除的外箱条码号清单
        [HttpPost]
        public async Task<ActionResult> OutputOutsideBoxBarCodeNumListCanDel(string ordernum)
        {
            var OutsideBoxBarCodePackagingNumList = await db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == ordernum || c.EmbezzleOrderNum == ordernum).Select(c => c.OuterBoxBarcode).Distinct().ToListAsync();
            var OutsideBoxBarCodeWarehouseJoinNumList = commom.GetCurrentwarehousList(ordernum).Select(c => c.OuterBoxBarcode).Distinct().ToList();
            var OutsideBoxBarCodeNumList = OutsideBoxBarCodePackagingNumList.Except(OutsideBoxBarCodeWarehouseJoinNumList);
            List<string> result = new List<string>();
            foreach (var item in OutsideBoxBarCodeNumList)
            {
                result.Add(item);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        ////查找当前订单号的出入库情况
        //public List<Warehouse_Join> GetCurrentwarehousList(string ordernum)
        //{
        //    var list = db.Warehouse_Join.Where(c => c.OrderNum == ordernum).ToList();

        //    var chongfu = list.Where(c => list.Count(a => a.BarCodeNum == c.BarCodeNum) > 1).Select(c => c.BarCodeNum).Distinct().ToList();
        //    var dange = list.Where(c => list.Count(a => a.BarCodeNum == c.BarCodeNum) == 1).Select(c => c.OuterBoxBarcode).Distinct().ToList();

        //    if (chongfu.Count == 0)
        //    {
        //        return list;
        //    }
        //    else
        //    {
        //        var time= list.Where(c => list.Count(a => a.BarCodeNum == c.BarCodeNum) > 1).Max(c => c.Date);
        //        DateTime updatetime = new DateTime(2019,10,17,12,00,00);
        //        if ( time< updatetime)
        //        {
        //            return list;
        //        }
        //        List<Warehouse_Join> result = new List<Warehouse_Join>();
        //        foreach (var dan in dange)
        //        {
        //            var info = list.Where(c => c.OuterBoxBarcode == dan).ToList();
        //            if (info.FirstOrDefault().IsOut == false)
        //            {
        //                result.AddRange(info);
        //            }
        //            else
        //            {
        //                if (db.Packing_BarCodePrinting.Count(c => c.OrderNum == ordernum && c.OuterBoxBarcode == dan) == 0)
        //                {
        //                    result.AddRange(info);
        //                }
        //            }
        //        }
        //        foreach (var item in chongfu)
        //        {
        //            var current = list.Where(c => c.BarCodeNum == item).Select(c => c.IsOut).ToList();
        //            if (current.Contains(false))
        //            {
        //                Warehouse_Join aa = list.Where(c => c.BarCodeNum == item && c.IsOut == false).FirstOrDefault();
        //                result.Add(aa);
        //            }
        //            else
        //            {
        //                if (db.Packing_BarCodePrinting.Count(c => c.OrderNum == ordernum && c.BarCodeNum == item) != 0)
        //                {
        //                    continue;
        //                }
        //                var max = list.Where(c => c.BarCodeNum == item).Max(c => c.WarehouseOutNum);
        //                Warehouse_Join aa = list.Where(c => c.BarCodeNum == item && c.WarehouseOutNum == max).FirstOrDefault();
        //                result.Add(aa);
        //            }

        //        }
        //        return result;
        //    }
        //}
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
            var ordernum_list = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == ordernum && c.QC_Operator == null);
            var screen_list = ordernum_list.Select(c => c.ScreenNum).Distinct();
            foreach (var screen in screen_list)
            {
                var ordernum_list_screen_list = ordernum_list.Where(c => c.ScreenNum == screen);
                var outsideboxbarcodelist = ordernum_list_screen_list.Select(c => c.OuterBoxBarcode).Distinct().ToList();
                var warehouout = commom.GetCurrentwarehousList(ordernum).Where(c => c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().ToList();
                var excrp = outsideboxbarcodelist.Except(warehouout);
                List<Packing_BarCodePrinting> record_screen = new List<Packing_BarCodePrinting>();
                List<string> info = new List<string>();
                foreach (var item in excrp)
                {
                    info.Add(item);
                }
                foreach (var item in info)
                {
                    record_screen.Add(ordernum_list_screen_list.Where(c => c.OuterBoxBarcode == item).FirstOrDefault());
                }
                result.Add("屏序" + screen, JsonConvert.SerializeObject(record_screen));
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpPost]
        public ActionResult OutsideBoxLableLogoChange(string ordernum, string outerboxbarcode, bool logo)
        {
            var recordlist = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outerboxbarcode && c.QC_Operator == null);
            int count = 0;
            foreach (var record in recordlist)
            {
                record.IsLogo = logo;
            }
            UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "外箱LOGO修改：外箱条码" + outerboxbarcode + "已改为" + (logo == true ? "有" : "无") + "LOGO" };
            db.UserOperateLog.Add(log);
            db.SaveChanges();
            count = db.SaveChanges();
            if (count > 0)
            {
                return Content(outerboxbarcode + "已改为" + (logo == true ? "有" : "无") + "LOGO.");
            }
            return Content("无修改.");
        }


        #region---自用，入库后删除标签（出库后不能删）
        //根据订单输出包装完成，已入库，但未出库清单
        [HttpPost]
        public async Task<ActionResult> OutputOutsideBoxBarCodeNumListWarehouseJoinNotOutput(string ordernum)
        {
            var OutsideBoxBarCodeWarehouseJoinNumList = await db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.IsOut == false && c.State == "模组").Select(c => c.OuterBoxBarcode).Distinct().ToListAsync();
            return Content(JsonConvert.SerializeObject(OutsideBoxBarCodeWarehouseJoinNumList));
        }
        #endregion

        //检查外箱条码号是否存在
        [HttpPost]
        public bool CheckOutsideBoxBadeCodeNumExist(string outsidebarcode)
        {
            var count = db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == outsidebarcode && c.QC_Operator == null);
            if (count > 0) return true;
            else return false;
        }

        //前端用 检查外箱条码号是否存在
        public ActionResult CheckOutsideBoxBadeCodeNumExist1(string outsidebarcode, string ordernum = null, bool isInside = false)
        {
            JObject message = new JObject(); var count = 0;
            if (outsidebarcode.Substring(outsidebarcode.Length - 5, 2) == "MK")//是不是模块
            {
                count = isInside == false ? db.ModuleOutsideTheBox.Count(c => c.OutsideBarcode == outsidebarcode) : 0;
            }
            else
            {
                count = db.Packing_BarCodePrinting.Count(c => (isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.BarCodeNum == outsidebarcode) && c.QC_Operator == null);
            }
            if (count > 0)
            {
                if (!string.IsNullOrEmpty(ordernum))
                {
                    string currentorder = "";
                    if (outsidebarcode.Substring(outsidebarcode.Length - 5, 2) == "MK")//是不是模块
                    {
                        currentorder = db.ModuleOutsideTheBox.Where(c => c.OutsideBarcode == outsidebarcode).Select(c => c.OrderNum).FirstOrDefault();
                    }
                    else
                    {
                        currentorder = db.Packing_BarCodePrinting.Where(c => (isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.BarCodeNum == outsidebarcode) && c.QC_Operator == null).Select(c => c.CartonOrderNum).FirstOrDefault();
                    }
                    if (ordernum != currentorder)
                    {
                        message.Add("message", isInside == false ? "此外箱条码的订单号应该是:" + currentorder : "此内箱条码的订单号应该是:" + currentorder);
                        message.Add("warehouseNum", "");
                        message.Add("barcode", outsidebarcode);
                        return Content(JsonConvert.SerializeObject(message));
                    }

                }
                #region 查找当前出入库情况
                List<Warehouse_Join> result = new List<Warehouse_Join>();
                if (string.IsNullOrEmpty(ordernum))
                {
                    var currentorder = db.Warehouse_Join.Where(c => isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.BarCodeNum == outsidebarcode).Select(c => c.OrderNum).FirstOrDefault();
                    result = commom.GetCurrentwarehousList2(currentorder);
                    if (outsidebarcode.Substring(outsidebarcode.Length - 5, 2) == "MK")
                        result = result.Where(c => c.State == "模块").ToList();
                    else
                        result = result.Where(c => c.State == "模组").ToList();

                }
                else
                {
                    result = commom.GetCurrentwarehousList2(ordernum);
                    if (outsidebarcode.Substring(outsidebarcode.Length - 5, 2) == "MK")
                        result = result.Where(c => c.State == "模块").ToList();
                    else
                        result = result.Where(c => c.State == "模组").ToList();
                }
                #endregion
                var warejoin = result.Count(c => isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.BarCodeNum == outsidebarcode);
                if (warejoin == 0)
                {
                    message.Add("message", isInside == false ? "此外箱条码未入库" : "此内箱条码未入库");
                    message.Add("warehouseNum", "");
                    message.Add("barcode", outsidebarcode);
                    return Content(JsonConvert.SerializeObject(message));
                }
                var wareout = result.Where(c => isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.BarCodeNum == outsidebarcode).Select(c => c.IsOut);
                if (wareout.Contains(true))
                {
                    var max = db.Warehouse_Join.Where(c => (isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.BarCodeNum == outsidebarcode) && c.IsOut == true).Max(c => c.WarehouseOutNum);
                    var warehouseNum = db.Warehouse_Join.Where(c => (isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.BarCodeNum == outsidebarcode) && c.IsOut == true && c.WarehouseOutNum == max).Select(c => c.WarehouseNum).FirstOrDefault();
                    message.Add("message", isInside == false ? "此外箱条码已出库" : "此外箱条码已出库");
                    message.Add("warehouseNum", warehouseNum);
                    message.Add("barcode", outsidebarcode);
                    return Content(JsonConvert.SerializeObject(message));
                }

                var warejoinNum = db.Warehouse_Join.Where(c => (isInside == false ? c.OuterBoxBarcode == outsidebarcode : c.BarCodeNum == outsidebarcode) && c.IsOut == false).Select(c => c.WarehouseNum).FirstOrDefault();
                message.Add("message", "");
                message.Add("warehouseNum", warejoinNum);
                message.Add("barcode", outsidebarcode);
                return Content(JsonConvert.SerializeObject(message));
            }
            else
            {
                message.Add("message", isInside == false ? "没有找到此外箱条码" : "没有找到此内箱条码");
                message.Add("warehouseNum", "");
                message.Add("barcode", outsidebarcode);
                return Content(JsonConvert.SerializeObject(message));
            }
        }

        //前端用 检查外箱条码号是否存在(检查未入库已打印的条码)
        public ActionResult CheckOutsideBoxBadeCodeNumExist2(string outsidebarcode, string ordernum = null)
        {
            JObject message = new JObject();
            var count = db.Packing_BarCodePrinting.Count(c => c.OuterBoxBarcode == outsidebarcode && c.QC_Operator == null);
            if (count > 0)
            {
                #region 查找当前出入库情况
                List<Warehouse_Join> result = new List<Warehouse_Join>();
                if (string.IsNullOrEmpty(ordernum))
                {
                    var currentorder = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode && c.QC_Operator == null).Select(c => c.CartonOrderNum).FirstOrDefault();
                    result = commom.GetCurrentwarehousList(currentorder);
                }
                else
                {
                    result = commom.GetCurrentwarehousList(ordernum);
                }
                #endregion
                var warejoin = result.Count(c => c.OuterBoxBarcode == outsidebarcode);
                if (warejoin == 0)
                {
                    message.Add("message", null);
                    message.Add("warehouseNum", "");
                    message.Add("barcode", outsidebarcode);
                    return Content(JsonConvert.SerializeObject(message));
                }
                var wareout = result.Where(c => c.OuterBoxBarcode == outsidebarcode).Select(c => c.IsOut);
                if (!wareout.Contains(false))
                {
                    var max = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == outsidebarcode && c.IsOut == true).Max(c => c.WarehouseOutNum);
                    var warehouseNum = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == outsidebarcode && c.IsOut == true && c.WarehouseOutNum == max).Select(c => c.WarehouseNum).FirstOrDefault();
                    message.Add("message", "此条码已出库");
                    message.Add("warehouseNum", warehouseNum);
                    message.Add("barcode", outsidebarcode);
                    return Content(JsonConvert.SerializeObject(message));
                }
                if (!string.IsNullOrEmpty(ordernum))
                {
                    string currentorder = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode && c.QC_Operator == null).FirstOrDefault().OrderNum;
                    if (ordernum != currentorder)
                    {
                        message.Add("message", "此条码的订单号应该是:" + currentorder);
                        message.Add("warehouseNum", "");
                        message.Add("barcode", outsidebarcode);
                        return Content(JsonConvert.SerializeObject(message));
                    }

                }
                var warejoinNum = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == outsidebarcode && c.IsOut == false).Select(c => c.WarehouseNum).FirstOrDefault();
                message.Add("message", "此条码已入库");
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
        public ActionResult OutsideBoxLablePrintToImg(string outsidebarcode, string leng = "")
        {
            var outsidebarcode_recordlist = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode && c.QC_Operator == null);
            var screem = outsidebarcode_recordlist.FirstOrDefault().ScreenNum;
            var batch = outsidebarcode_recordlist.FirstOrDefault().Batch;
            var ordernum1 = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode && c.QC_Operator == null).Select(c => new { c.CartonOrderNum, c.EmbezzleOrderNum }).FirstOrDefault();//订单和挪用订单
            string ordernum = string.IsNullOrEmpty(ordernum1.EmbezzleOrderNum) ? ordernum1.CartonOrderNum : ordernum1.EmbezzleOrderNum;//如果挪用订单不为空，则显示挪用订单，否则显示本来订单号
            string type = outsidebarcode_recordlist.FirstOrDefault().Type;
            string material_discription = outsidebarcode_recordlist.FirstOrDefault().Materiel;
            string sntn = outsidebarcode_recordlist.FirstOrDefault().SNTN + "/" + db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.ScreenNum == screem && c.Batch == batch).Sum(c => c.Quantity);
            bool logo = outsidebarcode_recordlist.FirstOrDefault().IsLogo;
            double? g_Weight = outsidebarcode_recordlist.FirstOrDefault().G_Weight;
            double? n_Weight = outsidebarcode_recordlist.FirstOrDefault().N_Weight;
            string[] mn_list = outsidebarcode_recordlist.Select(c => c.ModuleGroupNum).ToArray();
            if (mn_list[0] == null)
            {
                mn_list = outsidebarcode_recordlist.Select(c => c.BarCodeNum).ToArray();
            }
            string qty = mn_list.Count().ToString();
            int screennum = screem;   //屏序号
                                      //如果有包装新订单号，则使用包装新订单号。
            string packagingordernum = outsidebarcode_recordlist.FirstOrDefault().PackagingOrderNum;
            ordernum = String.IsNullOrEmpty(packagingordernum) ? ordernum : packagingordernum;
            var AllBitmap = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
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
            //var AllBitmap = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum,leng);
            //MemoryStream ms = new MemoryStream();
            //AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //AllBitmap.Dispose();
            //return File(ms.ToArray(), "image/Png");
            #endregion
        }

        public ActionResult OutsideBoxLablePrintAgain(string outsidebarcode, int pagecount = 1, int concentration = 5, string ip = "", int port = 0, string leng = "")
        {
            if (!CheckOutsideBoxBadeCodeNumExist(outsidebarcode)) return Content("外箱条码号不存在!");
            var outsidebarcode_recordlist = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode && c.QC_Operator == null);
            var screem = outsidebarcode_recordlist.FirstOrDefault().ScreenNum;
            var batch = outsidebarcode_recordlist.FirstOrDefault().Batch;
            var ordernum1 = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == outsidebarcode && c.QC_Operator == null).Select(c => new { c.CartonOrderNum, c.EmbezzleOrderNum }).FirstOrDefault();//订单和挪用订单
            string ordernum = string.IsNullOrEmpty(ordernum1.EmbezzleOrderNum) ? ordernum1.CartonOrderNum : ordernum1.EmbezzleOrderNum;//如果挪用订单不为空，则显示挪用订单，否则显示本来订单号
            string type = outsidebarcode_recordlist.FirstOrDefault().Type;
            string material_discription = outsidebarcode_recordlist.FirstOrDefault().Materiel;
            string sntn = outsidebarcode_recordlist.FirstOrDefault().SNTN + "/" + db.Packing_BasicInfo.Where(c => c.OrderNum == ordernum && c.ScreenNum == screem && c.Batch == batch).Sum(c => c.Quantity);
            bool logo = outsidebarcode_recordlist.FirstOrDefault().IsLogo;
            double? g_Weight = outsidebarcode_recordlist.FirstOrDefault().G_Weight;
            double? n_Weight = outsidebarcode_recordlist.FirstOrDefault().N_Weight;
            string[] mn_list = outsidebarcode_recordlist.Select(c => c.ModuleGroupNum).ToArray();
            if (mn_list[0] == null)
            {
                mn_list = outsidebarcode_recordlist.Select(c => c.BarCodeNum).ToArray();
            }
            string qty = mn_list.Count().ToString();
            int screennum = screem;   //屏序号
                                      //如果有包装新订单号，则使用包装新订单号。
            string packagingordernum = outsidebarcode_recordlist.FirstOrDefault().PackagingOrderNum;
            ordernum = String.IsNullOrEmpty(packagingordernum) ? ordernum : packagingordernum;
            var bm = CreateOutsideBoxLable(mn_list, ordernum, outsidebarcode, material_discription, sntn, qty, logo, screennum, g_Weight, n_Weight, leng);
            int totalbytes = bm.ToString().Length;
            int rowbytes = 10;
            string data = "^XA^MD5~DGR:ZONE.GRF,";
            string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
            data += totalbytes + "," + rowbytes + "," + hex;
            data += "^LH0,0^FO38,0^XGR:ZONE.GRF^FS^XZ";
            string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
            return Content(result);



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
            if (mn_E_count > 12 && mn_E_count <= 20)
            {
                if (mn_list[0].Length < 7)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 28, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 485;
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
                    int top_y = 470;
                    int left_x = 70;
                    for (int i = 0; i < mn_list.Count(); i++)
                    {
                        theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        if ((i % 2) == 0)
                        {
                            left_x += 315;
                        }
                        else
                        {
                            top_y += 45;
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
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 48, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 460;
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
                            top_y += 80;
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
                    int top_y = 485;
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
                            top_y += 70;
                            left_x -= 315;
                        }
                    }
                }
            }
            //9-10位模组号
            else if (mn_E_count > 8 && mn_E_count <= 10)
            {
                if (mn_list[0].Length < 8)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 45, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 465;
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
                            top_y += 90;
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
                    int top_y = 490;
                    int left_x = 70;
                    for (int i = 0; i < mn_list.Count(); i++)
                    {
                        theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        if ((i % 2) == 0)
                        {
                            left_x += 315;
                        }
                        else
                        {
                            top_y += 85;
                            left_x -= 315;
                        }
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
                    int top_y = 475;
                    int left_x = 70;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        if ((i % 2) != 0)
                        {
                            left_x += 300;
                        }
                        else
                        {
                            top_y += 120;
                            left_x -= 300;
                        }
                    }
                }
                else
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 33, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Center;
                    int top_y = 485;
                    int left_x = 360;
                    for (int i = 0; i < mn_list.Count(); i++)
                    {
                        theGraphics.DrawString(mn_list[i], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        //if ((i % 2) == 0)
                        //{
                        //    left_x += 315;
                        //}
                        //else
                        //{
                        top_y += 55;
                        //left_x -= 310;
                        //}
                    }
                }
            }
            //5-6位模组号
            else if (mn_E_count > 4 && mn_E_count <= 6)
            {
                if (mn_list[0].Length < 7)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 50, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 485;
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
                    listformat.Alignment = StringAlignment.Center;
                    int top_y = 465;
                    int left_x = 365;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 75;
                    }
                }
            }
            //4位模组号
            else if (mn_E_count == 4)
            {
                if (mn_list[0].Length < 7)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 70, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Center;
                    int top_y = 460;
                    int left_x = 365;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 120;
                    }
                }
                else
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 60, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Center;
                    int top_y = 485;
                    int left_x = 365;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 100;
                    }
                }
            }
            //3位模组号
            else if (mn_E_count == 3)
            {
                if (mn_list[0].Length < 7)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 80, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Center;
                    int top_y = 485;
                    int left_x = 365;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 140;
                    }
                }
                else
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 60, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Center;
                    int top_y = 485;
                    int left_x = 365;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 140;
                    }
                }
            }
            //2位模组号
            else if (mn_E_count == 2)
            {
                if (mn_list[0].Length < 7)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 100, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Center;
                    int top_y = 550;
                    int left_x = 365;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 150;
                    }
                }
                else
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 40, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Center;
                    int top_y = 550;
                    int left_x = 365;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1].ToUpper(), myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 150;
                    }
                }
            }
            //1位模组号
            else if (mn_E_count == 1)
            {
                if (mn_list[0].Length == 6)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 100, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Center;
                    int top_y = 485;
                    int left_x = 365;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 200;
                    }
                }
                else if (mn_list[0].Length == 5)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 120, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Center;
                    int top_y = 485;
                    int left_x = 365;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 200;
                    }
                }
                else if (mn_list[0].Length <= 4)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 160, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Center;
                    int top_y = 500;
                    int left_x = 365;
                    for (int i = 1; i < mn_list.Count() + 1; i++)
                    {
                        theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                        top_y += 200;
                    }
                }
                else
                {
                    Rectangle specificationRectangle = new Rectangle(90, 480, 600, 600);
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 60, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    theGraphics.DrawString(mn_list[0], myFont_modulenum_list, bush, specificationRectangle, listformat);

                    //System.Drawing.Font myFont_modulenum_list;
                    //myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 60, FontStyle.Bold);
                    //StringFormat listformat = new StringFormat();
                    //listformat.Alignment = StringAlignment.Center;
                    //int top_y = 485;
                    //int left_x = 365;
                    //for (int i = 1; i < mn_list.Count() + 1; i++)
                    //{
                    //    theGraphics.DrawString(mn_list[i - 1], myFont_modulenum_list, bush, left_x, top_y, listformat);
                    //    top_y += 200;
                    //}
                }
            }
            else
            {
                if (mn_list[0].Length < 7)
                {
                    System.Drawing.Font myFont_modulenum_list;
                    myFont_modulenum_list = new System.Drawing.Font("Microsoft YaHei UI", 20, FontStyle.Bold);
                    StringFormat listformat = new StringFormat();
                    listformat.Alignment = StringAlignment.Near;
                    int top_y = 465;
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
                            top_y += 39;
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
                    int top_y = 465;
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

        #region------导出包装数据

        public class OutputPackagingDatatoExcelClass
        {
            public string OutSideBoxNum { get; set; }
            public string BarcodesNum { get; set; }
            public string ModuleGroupNum { get; set; }
            public string InWarehouseDate { get; set; }
            public string OutWarehouseDate { get; set; }
            public string WarehourseStation { get; set; }
        }

        public FileContentResult OutputPackagingDatatoExcel(string ordernum)
        {
            var list = commom.GetCurrentwarehousList(ordernum).OrderBy(c => c.OuterBoxBarcode).ThenBy(c => c.ModuleGroupNum).Select(c => new { c.OuterBoxBarcode, c.BarCodeNum, c.ModuleGroupNum, c.Date, c.WarehouseOutDate, c.WarehouseNum }).ToList();
            List<OutputPackagingDatatoExcelClass> result = new List<OutputPackagingDatatoExcelClass>();
            foreach (var item in list)
            {
                var it = new OutputPackagingDatatoExcelClass();
                it.OutSideBoxNum = item.OuterBoxBarcode;
                it.BarcodesNum = item.BarCodeNum;
                it.ModuleGroupNum = item.ModuleGroupNum;
                it.InWarehouseDate = item.Date == null ? "" : item.Date.ToString();
                it.OutWarehouseDate = item.WarehouseOutDate == null ? "" : item.WarehouseOutDate.ToString();
                it.WarehourseStation = item.WarehouseNum;
                result.Add(it);
            }
            string[] columns = { "外箱条码号", "条码号", "模组号", "入库时间", "出库时间", "库位号" };
            byte[] filecontent = ExcelExportHelper.ExportExcel(result, ordernum, false, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, ordernum + "出入库情况.xlsx");
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

        //根据外箱条码反推订单号
        //public string GetOrdernumFromBarcode(string outherbarcode)
        //{
        //    string[] list = outherbarcode.Split('-');
        //    var first = list[0];
        //    var num = list[1];
        //    var year = first.Substring(0, 2);
        //    var content = first.Substring(2);
        //    var ordenum = "20" + year + "-" + content + "-" + num;
        //    return ordenum;
        //}
    }


}

