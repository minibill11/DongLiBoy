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

namespace JianHeMES.Controllers
{
    public class PackagingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //// GET: Packagings
        //public async Task<ActionResult> Index()
        //{
        //    return View(await db.Packaging.ToListAsync());
        //}


        #region -------包装首页---------

        // GET: Packagings
        public async Task<ActionResult> Index()
        {
            ViewBag.Display = "display:none";//隐藏View基本情况信息
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            return View(await db.Packaging.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string OrderNum, string searchString, int PageIndex = 0)
        {
            var AllPackagingRecords = db.Packaging as IQueryable<Packaging>;
            List<Packaging> AllPackagingRecordsList = null;
            if (String.IsNullOrEmpty(OrderNum))
            {
                //调出全部记录      
                AllPackagingRecords = from m in db.Packaging
                                      select m;
            }
            else
            {
                //筛选出对应orderNum所有记录
                AllPackagingRecords = from m in db.Packaging
                                      where (m.OrderNum == OrderNum)
                                       select m;
            }
            //检查orderNum和searchString是否为空
            if (!String.IsNullOrEmpty(searchString))
            {   //从调出的记录中筛选含searchString内容的记录
                AllPackagingRecords = AllPackagingRecords.Where(m => m.OrderNum.Contains(searchString));
            }

            //取出对应orderNum包装时长所有记录
            IQueryable<TimeSpan?> TimeSpanList = from m in db.Packaging
                                                 where (m.OrderNum == OrderNum)
                                                 orderby m.OQCCheckTime
                                                 select m.OQCCheckTime;
            //计算外观电检总时长
            TimeSpan TotalTimeSpan = DateTime.Now - DateTime.Now;
            if (AllPackagingRecords.Where(x => x.Packaging_OQCCheckAbnormal == 1).Count() != 0)    //Packaging_OQCCheckAbnormal的值是1为正常
            {
                foreach (var m in TimeSpanList)
                {
                    if (m != null)
                    {
                        TotalTimeSpan = TotalTimeSpan.Add(m.Value).Duration();
                    }
                }
                ViewBag.TotalTimeSpan = TotalTimeSpan.Hours.ToString() + "小时" + TotalTimeSpan.Minutes.ToString() + "分" + TotalTimeSpan.Seconds.ToString() + "秒";
            }
            else
            {
                ViewBag.TotalTimeSpan = "暂时没有已完成包装的模组";
            }

            //计算平均用时
            TimeSpan AvgTimeSpan = DateTime.Now - DateTime.Now;
            int Order_CR_valid_Count = AllPackagingRecords.Where(x => x.OQCCheckTime != null).Count();
            int TotalTimeSpanSecond = Convert.ToInt32(TotalTimeSpan.Hours.ToString()) * 3600 + Convert.ToInt32(TotalTimeSpan.Minutes.ToString()) * 60 + Convert.ToInt32(TotalTimeSpan.Seconds.ToString());
            int AvgTimeSpanInSecond = 0;
            if (Order_CR_valid_Count != 0)
            {
                AvgTimeSpanInSecond = TotalTimeSpanSecond / Order_CR_valid_Count;
                int AvgTimeSpanMinute = AvgTimeSpanInSecond / 60;
                int AvgTimeSpanSecond = AvgTimeSpanInSecond % 60;
                ViewBag.AvgTimeSpan = AvgTimeSpanMinute + "分" + AvgTimeSpanSecond + "秒";//向View传递计算平均用时
            }
            else
            {
                ViewBag.AvgTimeSpan = "暂时没有已完成包装的模组";//向View传递计算平均用时
            }

            //列出记录
            AllPackagingRecordsList = AllPackagingRecords.ToList();
            //统计包装结果正常的模组数量
            var Order_CR_Normal_Count = AllPackagingRecords.Where(x => x.Packaging_OQCCheckAbnormal == 1).Count();
            var Abnormal_Count = AllPackagingRecords.Where(x => x.Packaging_OQCCheckAbnormal != 1).Count();
            //读出订单中模组总数量
            var Order_MG_Quantity = (from m in db.OrderMgm
                                     where (m.OrderNum == OrderNum)
                                     select m.Boxes).FirstOrDefault();
            //将模组总数量、正常的模组数量、未完成包装模组数量、订单号信息传递到View页面
            ViewBag.Quantity = Order_MG_Quantity;
            ViewBag.NormalCount = Order_CR_Normal_Count;
            ViewBag.AbnormalCount = Abnormal_Count;
            ViewBag.RecordCount = AllPackagingRecords.Count();
            ViewBag.NeverFinish = Order_MG_Quantity - Order_CR_Normal_Count;
            ViewBag.orderNum = OrderNum;

            //未选择订单时隐藏基本信息设置
            if (ViewBag.Quantity == 0)
            { ViewBag.Display = "display:none"; }
            else { ViewBag.Display = "display:normal"; }

            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.


            //分页计算功能
            var recordCount = AllPackagingRecords.Count();
            var pageCount = GetPageCount(recordCount);
            if (PageIndex >= pageCount && pageCount >= 1)
            {
                PageIndex = pageCount - 1;
            }
            AllPackagingRecords = AllPackagingRecords.OrderByDescending(m => m.OQCCheckBT)
                                .Skip(PageIndex * PAGE_SIZE)
                                .Take(PAGE_SIZE);
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageCount = pageCount;
            ViewBag.OrderNumList = GetOrderNumList();

            return View(AllPackagingRecordsList);
        }

        #endregion


        // GET: Packagings/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Packaging packaging = await db.Packaging.FindAsync(id);
            if (packaging == null)
            {
                return HttpNotFound();
            }
            return View(packaging);
        }

        // GET: Packagings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Packagings/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Packaging_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Packaging packaging)
        {
            if (ModelState.IsValid)
            {
                db.Packaging.Add(packaging);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(packaging);
        }

        // GET: Packagings/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Packaging packaging = await db.Packaging.FindAsync(id);
            if (packaging == null)
            {
                return HttpNotFound();
            }
            return View(packaging);
        }

        // POST: Packagings/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Packaging_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Packaging packaging)
        {
            if (ModelState.IsValid)
            {
                db.Entry(packaging).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(packaging);
        }

        // GET: Packagings/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Packaging packaging = await db.Packaging.FindAsync(id);
            if (packaging == null)
            {
                return HttpNotFound();
            }
            return View(packaging);
        }

        // POST: Packagings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Packaging packaging = await db.Packaging.FindAsync(id);
            db.Packaging.Remove(packaging);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        #region ----------包装开始-----------------


        // GET: Packagings/Packaging_B
        public ActionResult Packaging_B()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            return View();
        }

        // POST: Packaging/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Packaging_B([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Packaging_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Packaging Packaging)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            Packaging.OQCCheckBT = DateTime.Now;

            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

            if (ModelState.IsValid)
            {
                //Burn_in burn_incheck = await db.Burn_in.FindAsync(burn_in.BarCodesNum);

                //if (burn_incheck==null)
                //{ 
                //db.Burn_in.Add(burn_in);
                //await db.SaveChangesAsync();
                //return RedirectToAction("Burn_in_F", new { id = burn_in.Id });
                ////return RedirectToAction("Index");
                // }
                //else
                //{
                //    return RedirectToAction("Burn_in_F", new { id = burn_incheck.Id });
                //}
                db.Packaging.Add(Packaging);
                await db.SaveChangesAsync();
                return RedirectToAction("Packaging_F", new { id = Packaging.Id });
            }
            return View(Packaging);
        }
        #endregion


        #region ------------包装完成------------
        // GET: Packagings/Packaging_F
        public ActionResult Packaging_F(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Packaging packaging = db.Packaging.Find(id);
            if (packaging == null)
            {
                return HttpNotFound();
            }
            return View(packaging);
        }

        // POST: Packagings/Packaging_F
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Packaging_F([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Packaging_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Packaging Packaging)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (Packaging.OQCCheckFT == null)
            {
                Packaging.OQCCheckFT = DateTime.Now;
                Packaging.OQCPrincipal = ((Users)Session["User"]).UserName;
                var BT = Packaging.OQCCheckBT.Value;
                var FT = Packaging.OQCCheckFT.Value;
                var CT = FT - BT;
                Packaging.OQCCheckTime = CT;
                Packaging.OQCCheckTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
                Packaging.OQCCheckFinish = true;
            }

            if (ModelState.IsValid)
            {
                db.Entry(Packaging).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Packaging_B");
            }
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            return View(Packaging);
        }

        #endregion



        #region ------------------ 取出整个OrderMgms的OrderNum订单号列表.--------------------------------------------------
        private List<SelectListItem> GetOrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.OrderCreateDate).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
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

        #region  -------------检索订单号------
        private List<SelectListItem> GetOrderNumList()
        {
            var ordernum = db.OrderMgm.OrderBy(m => m.OrderNum).Select(m => m.OrderNum).Distinct();

            var ordernumitems = new List<SelectListItem>();
            foreach (string num in ordernum)
            {
                ordernumitems.Add(new SelectListItem
                {
                    Text = num,
                    Value = num
                });
            }
            return ordernumitems;
        }
        #endregion

        #region  -----------分页------------
        private static readonly int PAGE_SIZE = 10;

        private int GetPageCount(int recordCount)
        {
            int pageCount = recordCount / PAGE_SIZE;
            if (recordCount % PAGE_SIZE != 0)
            {
                pageCount += 1;
            }
            return pageCount;
        }
        #endregion

    }
}