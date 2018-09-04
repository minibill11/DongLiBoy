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
using System.Collections;

namespace JianHeMES.Controllers
{
    public class AppearancesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region --------------------OQCNormal列表--------------------
        private List<SelectListItem> AppearancesNormalList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "正常",
                    Value = "正常"
                },
                new SelectListItem
                {
                    Text = "异常",
                    Value = "异常"
                }
            };
        }
        #endregion


        #region  -----//维修列表-----------

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


        #region -------外观首页---------
        // GET: Appearances
        public async Task<ActionResult> Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            ViewBag.Display = "display:none";//隐藏View基本情况信息
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.AppearancesNormal = AppearancesNormalList();
            ViewBag.NotDo = null;
            //return View(await db.Appearance.ToListAsync());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string OrderNum, string AppearancesNormal, string searchString, int PageIndex = 0)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var AllAppearanceRecords = db.Appearance as IQueryable<Appearance>;
            List<Appearance> AllAppearanceRecordsList = null;
            if (String.IsNullOrEmpty(OrderNum))
            {
                //调出全部记录      
                AllAppearanceRecords = from m in db.Appearance
                                       select m;
            }
            else
            {
                //筛选出对应orderNum所有记录
                AllAppearanceRecords = from m in db.Appearance
                                       where (m.OrderNum == OrderNum)
                                       select m;
            }

            //统计外观包装结果正常的模组数量
            var Order_CR_Normal_Count = AllAppearanceRecords.Where(x => x.Appearance_OQCCheckAbnormal == "正常").Count();
            var Abnormal_Count = AllAppearanceRecords.Where(x => x.Appearance_OQCCheckAbnormal != "正常").Count();

            #region   ---------筛选正常、异常-------------
            //正常、异常记录筛选
            if (AppearancesNormal == "异常")
            {
                AllAppearanceRecords = from m in AllAppearanceRecords where (m.Appearance_OQCCheckAbnormal != "正常") select m;
            }
            else if (AppearancesNormal == "正常")
            {
                AllAppearanceRecords = from m in AllAppearanceRecords where (m.Appearance_OQCCheckAbnormal == "正常") select m;
            }

            #endregion

            #region   ----------筛选从未开始做的条码清单------------
            //取出订单的全部条码
            List<BarCodes> BarCodesList = (from m in db.BarCodes where m.OrderNum == OrderNum select m).ToList();
            ArrayList NotDoOQCList = new ArrayList();
            foreach (var barcode in BarCodesList)
            {
                if ((from m in db.Appearance where m.BarCodesNum == barcode.BarCodesNum select m).Count() == 0)
                {
                    NotDoOQCList.Add(barcode.BarCodesNum);
                }
            }
            ViewBag.NotDo = NotDoOQCList;//输出从未做的条码清单
            int barcodeslistcount = NotDoOQCList.Count;
            ViewBag.NotDoCount = barcodeslistcount;//从未开始做的数量
            #endregion

            //检查orderNum和searchString是否为空
            if (!String.IsNullOrEmpty(searchString))
            {   //从调出的记录中筛选含searchString内容的记录
                AllAppearanceRecords = AllAppearanceRecords.Where(m => m.OrderNum.Contains(searchString));
            }

            //取出对应orderNum外观包装时长所有记录
            IQueryable<TimeSpan?> TimeSpanList = from m in db.Appearance
                                                 where (m.OrderNum == OrderNum)
                                                 orderby m.OQCCheckTime
                                                 select m.OQCCheckTime;
            //计算外观包装总时长
            TimeSpan TotalTimeSpan = DateTime.Now - DateTime.Now;
            if (AllAppearanceRecords.Where(x => x.Appearance_OQCCheckAbnormal == "正常").Count() != 0)    //Appearance_OQCCheckAbnormal的值是1为正常
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
                ViewBag.TotalTimeSpan = "暂时没有已完成外观包装的模组";
            }

            //计算平均用时
            TimeSpan AvgTimeSpan = DateTime.Now - DateTime.Now;
            int Order_CR_valid_Count = AllAppearanceRecords.Where(x => x.OQCCheckTime != null).Count();
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
                ViewBag.AvgTimeSpan = "暂时没有已完成外观包装的模组";//向View传递计算平均用时
            }

            //列出记录
            AllAppearanceRecordsList = AllAppearanceRecords.ToList();

            //读出订单中模组总数量
            var Order_MG_Quantity = (from m in db.OrderMgm
                                     where (m.OrderNum == OrderNum)
                                     select m.Boxes).FirstOrDefault();
            //将模组总数量、正常的模组数量、未完成外观包装模组数量、订单号信息传递到View页面
            ViewBag.Quantity = Order_MG_Quantity;
            ViewBag.NormalCount = Order_CR_Normal_Count;
            ViewBag.AbnormalCount = Abnormal_Count;
            ViewBag.RecordCount = AllAppearanceRecords.Count();
            ViewBag.NeverFinish = Order_MG_Quantity - Order_CR_Normal_Count;
            ViewBag.orderNum = OrderNum;

            //未选择订单时隐藏基本信息设置
            if (ViewBag.Quantity == 0)
            { ViewBag.Display = "display:none"; }
            else { ViewBag.Display = "display:normal"; }

            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

            //分页计算功能
            var recordCount = AllAppearanceRecords.Count();
            var pageCount = GetPageCount(recordCount);
            if (PageIndex >= pageCount && pageCount >= 1)
            {
                PageIndex = pageCount - 1;
            }
            AllAppearanceRecords = AllAppearanceRecords.OrderByDescending(m => m.OQCCheckBT)
                                .Skip(PageIndex * PAGE_SIZE)
                                .Take(PAGE_SIZE);
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageCount = pageCount;
            ViewBag.OrderNumList = GetOrderNumList();
            ViewBag.AppearancesNormal = AppearancesNormalList();

            return View(AllAppearanceRecords);
            //return View(AllAppearanceRecordsList);
        }
        #endregion


        // GET: Appearances/Details/5
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
            Appearance appearance = await db.Appearance.FindAsync(id);
            if (appearance == null)
            {
                return HttpNotFound();
            }
            return View(appearance);
        }

        // GET: Appearances/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Appearances/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Appearance_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Appearance appearance)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                db.Appearance.Add(appearance);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(appearance);
        }

        // GET: Appearances/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appearance appearance = await db.Appearance.FindAsync(id);
            if (appearance == null)
            {
                return HttpNotFound();
            }
            ViewBag.RepairList = SetRepairList();
            return View(appearance);
        }

        // POST: Appearances/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Appearance_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Appearance appearance)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                db.Entry(appearance).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(appearance);
        }

        // GET: Appearances/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appearance appearance = await db.Appearance.FindAsync(id);
            if (appearance == null)
            {
                return HttpNotFound();
            }
            return View(appearance);
        }

        // POST: Appearances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Appearance appearance = await db.Appearance.FindAsync(id);
            db.Appearance.Remove(appearance);
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



        #region ----------外观电检开始-----------------


        // GET: Appearances/Appearance_B
        public ActionResult Appearance_B()
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "外观包装OQC" || ((Users)Session["User"]).Role == "系统管理员")
            {
                return View();
            }
            return RedirectToAction("Index");
        }

        // POST: Appearance/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Appearance_B([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Appearance_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Appearance appearance)
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum) == null)
            //if (db.BarCodes.Where(u => u.BarCodesNum == model.BoxBarCode) != null)
            {
                ModelState.AddModelError("", "模组条码不存在，请检查条码输入是否正确！");
                return View(appearance);
            }
            //在BarCodesNum条码表中找到此条码号
            //Burn_in，准备在Assembles组装记录表中新建记录，包括OrderNum、BoxBarCode、PQCCheckBT、AssemblePQCPrincipal
            if (db.Appearance.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum) == null)
            {
                if (appearance.OrderNum == db.BarCodes.Where(u => u.BarCodesNum == appearance.BarCodesNum).FirstOrDefault().OrderNum)
                {
                    var appearanceRecord = db.Appearance.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum);
                    appearance.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == appearance.BarCodesNum).FirstOrDefault().OrderNum;
                    appearance.OQCCheckBT = DateTime.Now;
                    appearance.OQCPrincipal = ((Users)Session["User"]).UserName;
                    db.Appearance.Add(appearance);
                    db.SaveChanges();
                    return RedirectToAction("Appearance_F", new { appearance.Id });
                }
                else
                {
                    ModelState.AddModelError("", "该模组条码不属于所选订单，请选择正确的订单号！");
                    return View(appearance);
                }
                
            }
            //在Assembles组装记录表中找到对应BoxBarCode的记录，如果记录中没有正常的，准备在Assembles组装记录表中新建记录，如果有正常记录将提示不能重复进行QC
            else if (db.Appearance.Count(u => u.BarCodesNum == appearance.BarCodesNum) >= 1)
            {
                var appearance_list = db.Appearance.Where(m => m.BarCodesNum == appearance.BarCodesNum).ToList();
                int normalCount = appearance_list.Where(m => m.Appearance_OQCCheckAbnormal == "正常").Count();
                if (normalCount == 0)
                {
                    if (appearance.OrderNum == db.BarCodes.Where(u => u.BarCodesNum == appearance.BarCodesNum).FirstOrDefault().OrderNum)
                    {
                        appearance.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == appearance.BarCodesNum).FirstOrDefault().OrderNum;
                        appearance.OQCCheckBT = DateTime.Now;
                        appearance.OQCPrincipal = ((Users)Session["User"]).UserName;
                        db.Appearance.Add(appearance);
                        db.SaveChanges();
                        return RedirectToAction("Appearance_F", new { appearance.Id });
                    }
                    else
                    {
                        ModelState.AddModelError("", "该模组条码不属于所选订单，请选择正确的订单号！");
                        return View(appearance);
                    }
                    
                }
                else
                {
                    //return Content("<script>alert('此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！');window.location.href='../Appearances';</script>");
                    ModelState.AddModelError("", "此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！");
                    return View(appearance);
                }
            }
            else
            {
                //return RedirectToAction("Appearance_F", new { appearance.Id });
                return RedirectToAction("Index");
            }
        }
        #endregion


        #region -------------外观电检完成------------
        // GET: Appearances/Appearance_F
        public ActionResult Appearance_F(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appearance appearance = db.Appearance.Find(id);
            if (appearance == null)
            {
                return HttpNotFound();
            }
            ViewBag.RepairList = SetRepairList();
            return View(appearance);
        }

        // POST: Appearances/Appearance_F
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Appearance_F([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Appearance_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Appearance appearance)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (appearance.OQCCheckFT == null)
            {
                appearance.OQCCheckFT = DateTime.Now;
                appearance.OQCPrincipal = ((Users)Session["User"]).UserName;
                var BT = appearance.OQCCheckBT.Value;
                var FT = appearance.OQCCheckFT.Value;
                var CT = FT - BT;
                appearance.OQCCheckTime = CT;
                appearance.OQCCheckTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
                appearance.OQCCheckFinish = true;
            }

            if (ModelState.IsValid)
            {
                db.Entry(appearance).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Appearance_B");
            }
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            return View(appearance);
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
