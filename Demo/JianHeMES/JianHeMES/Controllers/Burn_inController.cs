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
using System.Web.Routing;

namespace JianHeMES.Controllers
{
    public class Burn_inController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region  -----维修列表-----------

        private List<SelectListItem> SetRepairList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "请选择维修情况",
                    Value = ""
                },
                new SelectListItem
                {
                    Text = "正常",
                    Value = "正常"
                },
                new SelectListItem
                {
                    Text = "现场维修",
                    Value = "现场维修"
                },
                new SelectListItem
                {
                    Text = "转维修站",
                    Value = "转维修站"
                }
            };
        }

        #endregion

        #region -------老化首页---------
        // GET: Burn_in
        public async Task<ActionResult> Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            ViewBag.Display = "display:none";//隐藏View基本情况信息
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string OrderNum, string searchString, int PageIndex = 0)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var AllBurn_inRecords = db.Burn_in as IQueryable<Burn_in>;
            List<Burn_in> AllBurn_inRecordsList = null;
            if (String.IsNullOrEmpty(OrderNum))
            {
                //调出全部记录      
                AllBurn_inRecords = from m in db.Burn_in
                                        select m;
            }
            else
            {
                //筛选出对应orderNum所有记录
                AllBurn_inRecords = from m in db.Burn_in
                                    where (m.OrderNum == OrderNum)
                                    select m;
            }
            //检查orderNum和searchString是否为空
            if (!String.IsNullOrEmpty(searchString))
            {   //从调出的记录中筛选含searchString内容的记录
                AllBurn_inRecords = AllBurn_inRecords.Where(m => m.OrderNum.Contains(searchString));
            }

            //取出对应orderNum校正时长所有记录
            IQueryable<TimeSpan?> TimeSpanList = from m in db.Burn_in
                                                 where (m.OrderNum == OrderNum)
                                                 orderby m.OQCCheckTime
                                                 select m.OQCCheckTime;
            //计算校正总时长
            TimeSpan TotalTimeSpan = DateTime.Now - DateTime.Now;
            if (AllBurn_inRecords.Where(x => x.Burn_in_OQCCheckAbnormal == "1").Count() != 0)    //Burn_in_OQCCheckAbnormal的值是1为正常
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
                ViewBag.TotalTimeSpan = "暂时没有已完成老化的模组";
            }

            //计算平均用时
            TimeSpan AvgTimeSpan = DateTime.Now - DateTime.Now;
            int Order_CR_valid_Count = AllBurn_inRecords.Where(x => x.OQCCheckTime != null).Count();
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
                ViewBag.AvgTimeSpan = "暂时没有已完成校正的模组";//向View传递计算平均用时
            }

            //列出记录
            AllBurn_inRecordsList = AllBurn_inRecords.ToList();
            //统计老化结果正常的模组数量
            var Order_CR_Normal_Count = AllBurn_inRecords.Where(x => x.Burn_in_OQCCheckAbnormal == "1").Count();
            var Abnormal_Count = AllBurn_inRecords.Where(x => x.Burn_in_OQCCheckAbnormal != "1").Count();
            //读出订单中模组总数量
            var Order_MG_Quantity = (from m in db.OrderMgm
                                     where (m.OrderNum == OrderNum)
                                     select m.Boxes).FirstOrDefault();
            //将模组总数量、正常的模组数量、未完成老化模组数量、订单号信息传递到View页面
            ViewBag.Quantity = Order_MG_Quantity;
            ViewBag.NormalCount = Order_CR_Normal_Count;
            ViewBag.AbnormalCount = Abnormal_Count;
            ViewBag.RecordCount = AllBurn_inRecords.Count();
            ViewBag.NeverFinish = Order_MG_Quantity - Order_CR_Normal_Count;
            ViewBag.orderNum = OrderNum;

            //未选择订单时隐藏基本信息设置
            if (ViewBag.Quantity == 0)
            { ViewBag.Display = "display:none"; }
            else { ViewBag.Display = "display:normal"; }

            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.


            //分页计算功能
            var recordCount = AllBurn_inRecords.Count();
            var pageCount = GetPageCount(recordCount);
            if (PageIndex >= pageCount && pageCount >= 1)
            {
                PageIndex = pageCount - 1;
            }
            AllBurn_inRecords = AllBurn_inRecords.OrderByDescending(m => m.OQCCheckBT)
                                .Skip(PageIndex * PAGE_SIZE)
                                .Take(PAGE_SIZE);
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageCount = pageCount;
            ViewBag.OrderNumList = GetOrderNumList();

            return View(AllBurn_inRecords);
            //return View(AllBurn_inRecordsList);
        }

        #endregion


        #region   ----------其他页面-------------------
        // GET: Burn_in/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Burn_in burn_in = await db.Burn_in.FindAsync(id);
            if (burn_in == null)
            {
                return HttpNotFound();
            }
            return View(burn_in);
        }

        // GET: Burn_in/Create
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }

        // POST: Burn_in/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Burn_in_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Burn_in burn_in)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                db.Burn_in.Add(burn_in);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(burn_in);
        }

        // GET: Burn_in/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Burn_in burn_in = await db.Burn_in.FindAsync(id);
            if (burn_in == null)
            {
                return HttpNotFound();
            }
            ViewBag.RepairList = SetRepairList();
            return View(burn_in);
        }

        // POST: Burn_in/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Burn_in_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Burn_in burn_in)
        {
            if (ModelState.IsValid)
            {
                db.Entry(burn_in).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(burn_in);
        }

        // GET: Burn_in/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Burn_in burn_in = await db.Burn_in.FindAsync(id);
            if (burn_in == null)
            {
                return HttpNotFound();
            }
            return View(burn_in);
        }

        // POST: Burn_in/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Burn_in burn_in = await db.Burn_in.FindAsync(id);
            db.Burn_in.Remove(burn_in);
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

        #endregion


        #region ----------老化开始-----------------


        // GET: Burn_in/Burn_in_B
        public ActionResult Burn_in_B()
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "老化OQC" || ((Users)Session["User"]).Role == "系统管理员")
            {
                return View();
            }
            return RedirectToAction("Index");

        }

        // POST: Burn_in/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Burn_in_B([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Burn_in_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Burn_in burn_in)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == burn_in.BarCodesNum) == null)
            {
                ModelState.AddModelError("", "框体条码不存在，请检查条码输入是否正确！");
                return View(burn_in);
            }
            //在BarCodesNum条码表中找到此条码号
            //Burn_in，准备在Assembles组装记录表中新建记录，包括OrderNum、BoxBarCode、PQCCheckBT、AssemblePQCPrincipal
            if (db.Burn_in.FirstOrDefault(u => u.BarCodesNum == burn_in.BarCodesNum) == null)
            {
                //var burn_inRecord = db.Burn_in.FirstOrDefault(u => u.BarCodesNum == burn_in.BarCodesNum);
                burn_in.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == burn_in.BarCodesNum).FirstOrDefault().OrderNum;
                burn_in.OQCCheckBT = DateTime.Now;
                burn_in.OQCPrincipal = ((Users)Session["User"]).UserName;
                db.Burn_in.Add(burn_in);
                db.SaveChanges();
                return RedirectToAction("Burn_in_F", new { burn_in.Id });
            }
            //在Assembles组装记录表中找到对应BoxBarCode的记录，如果记录中没有正常的，准备在Assembles组装记录表中新建记录，如果有正常记录将提示不能重复进行QC
            else if (db.Burn_in.Count(u => u.BarCodesNum == burn_in.BarCodesNum) >= 1)
            {
                var burn_in_list = db.Burn_in.Where(m => m.BarCodesNum == burn_in.BarCodesNum).ToList();
                int normalCount = burn_in_list.Where(m => m.Burn_in_OQCCheckAbnormal == "正常").Count();
                if (normalCount == 0)
                {
                    burn_in.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == burn_in.BarCodesNum).FirstOrDefault().OrderNum;
                    burn_in.OQCCheckBT = DateTime.Now;
                    burn_in.OQCPrincipal = ((Users)Session["User"]).UserName;
                    db.Burn_in.Add(burn_in);
                    db.SaveChanges();
                    return RedirectToAction("Burn_in_F", new { burn_in.Id });
                }
                else
                {
                    return Content("<script>alert('此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！');window.location.href='../Burn_in';</script>");
                }
            }
            else
            {
                //return RedirectToAction("Burn_in_F", new { burn_in.Id });
                return RedirectToAction("Index");
            }
        }
        #endregion


        #region -------------老化完成------------
        // GET: Burn_in/Burn_in_F
        public ActionResult Burn_in_F(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Burn_in burn_in = db.Burn_in.Find(id);
            if (burn_in == null)
            {
                return HttpNotFound();
            }
            ViewBag.RepairList = SetRepairList();
            return View(burn_in);
        }

        // POST: Burn_in/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Burn_in_F([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Burn_in_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Burn_in burn_in)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (burn_in.OQCCheckFT == null)
            {
                burn_in.OQCCheckFT = DateTime.Now;
                burn_in.OQCPrincipal = ((Users)Session["User"]).UserName;
                var BT = burn_in.OQCCheckBT.Value;
                var FT = burn_in.OQCCheckFT.Value;
                var CT = FT - BT;
                burn_in.OQCCheckTime = CT;
                burn_in.OQCCheckTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
                burn_in.OQCCheckFinish = true;
            }

            if (ModelState.IsValid)
            {
                db.Entry(burn_in).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Burn_in_B");
            }
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            return View(burn_in);
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
