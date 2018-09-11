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
using Newtonsoft.Json.Linq;
using System.Web.Helpers;

namespace JianHeMES.Controllers
{
    public class Burn_inController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region --------------------OQCNormal列表--------------------
        private List<SelectListItem> OQCNormalList()
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

        #region --------------------FinishStatus列表--------------------
        private List<SelectListItem> FinishStatusList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "完成",
                    Value = "完成"
                },
                new SelectListItem
                {
                    Text = "未完成",
                    Value = "未完成"
                }
            };
        }
        #endregion

        


        #region  -----维修列表-----------

        private List<SelectListItem> SetRepairList()
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
            ViewBag.OQCNormal = OQCNormalList();
            ViewBag.FinishStatus = FinishStatusList();

            ViewBag.NotDo = null;
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string OrderNum, string BoxBarCode, string OQCNormal,string FinishStatus, string searchString, int PageIndex = 0)
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

            //统计老化结果正常的模组数量
            var Order_CR_Normal_Count = AllBurn_inRecords.Where(x => x.RepairCondition == "正常").Count();
            var Abnormal_Count = AllBurn_inRecords.Where(x => x.RepairCondition != "正常" && x.RepairCondition != null).Count();

            #region  ---------按条码筛选--------------
            if (BoxBarCode != "")
            {
                AllBurn_inRecords = AllBurn_inRecords.Where(x => x.BarCodesNum == BoxBarCode);
                //Allassembles = from m in Allassembles where (m.BoxBarCode == BoxBarCode) select m;
            }
            #endregion

            #region   ---------筛选正常、异常-------------
            //正常、异常记录筛选
            if (OQCNormal == "异常")
            {
                AllBurn_inRecords = from m in AllBurn_inRecords where (m.RepairCondition != "正常") select m;
            }
            else if (OQCNormal == "正常")
            {
                AllBurn_inRecords = from m in AllBurn_inRecords where (m.RepairCondition == "正常") select m;
            }
            #endregion

            #region   ---------筛选完成、未完成-------------
            //完成、未完成记录筛选
            if (FinishStatus == "完成")
            {
                AllBurn_inRecords = from m in AllBurn_inRecords where (m.OQCCheckFT != null) select m;
            }
            else if (FinishStatus == "未完成")
            {
                AllBurn_inRecords = from m in AllBurn_inRecords where (m.OQCCheckFT == null) select m;
            }
            #endregion


            #region   ----------筛选从未开始做的条码清单------------
            //取出订单的全部条码
            List<BarCodes> BarCodesList = (from m in db.BarCodes where m.OrderNum == OrderNum select m).ToList();
            ArrayList NotDoOQCList = new ArrayList();
            foreach (var barcode in BarCodesList)
            {
                if ((from m in db.Burn_in where m.BarCodesNum == barcode.BarCodesNum select m).Count() == 0)
                {
                    NotDoOQCList.Add(barcode.BarCodesNum);
                }
            }
            ViewBag.NotDo = NotDoOQCList;//输出未完成的条码清单
            int barcodeslistcount = NotDoOQCList.Count;
            ViewBag.NotDoCount = barcodeslistcount;//未完成数量
            #endregion

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
            //计算老化总时长
            TimeSpan TotalTimeSpan = DateTime.Now - DateTime.Now;
            if (AllBurn_inRecords.Where(x => x.RepairCondition == "正常").Count() != 0)    //Burn_in_OQCCheckAbnormal的值是1为正常
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
            ViewBag.OQCNormal = OQCNormalList();
            ViewBag.FinishStatus = FinishStatusList();

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


        #region -------------单个模组老化开始-----------------


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
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == burn_in.BarCodesNum) == null)
            {
                ModelState.AddModelError("", "模组条码不存在，请检查条码输入是否正确！");
                return View(burn_in);
            }
            //在BarCodesNum条码表中找到此条码号
            //Burn_in，准备在Assembles组装记录表中新建记录，包括OrderNum、BoxBarCode、PQCCheckBT、AssemblePQCPrincipal
            if (db.Burn_in.FirstOrDefault(u => u.BarCodesNum == burn_in.BarCodesNum) == null)
            {
                if (burn_in.OrderNum == db.BarCodes.Where(u => u.BarCodesNum == burn_in.BarCodesNum).FirstOrDefault().OrderNum)
                {
                    //var burn_inRecord = db.Burn_in.FirstOrDefault(u => u.BarCodesNum == burn_in.BarCodesNum);
                    burn_in.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == burn_in.BarCodesNum).FirstOrDefault().OrderNum;
                    burn_in.OQCCheckBT = DateTime.Now;
                    burn_in.OQCPrincipal = ((Users)Session["User"]).UserName;
                    db.Burn_in.Add(burn_in);
                    db.SaveChanges();
                    //return RedirectToAction("Burn_in_F", new { burn_in.Id });
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "该模组条码不属于所选订单，请选择正确的订单号！");
                    return View(burn_in);
                }
            }
            //在Assembles组装记录表中找到对应BoxBarCode的记录，如果记录中没有正常的，准备在Assembles组装记录表中新建记录，如果有正常记录将提示不能重复进行QC
            else if (db.Burn_in.Count(u => u.BarCodesNum == burn_in.BarCodesNum) >= 1)
            {
                var burn_in_list = db.Burn_in.Where(m => m.BarCodesNum == burn_in.BarCodesNum).ToList();
                int normalCount = burn_in_list.Where(m => m.OQCCheckFinish == true).Count();
                if (normalCount == 0)
                {
                    foreach (var item in burn_in_list)
                    {
                        if (item.OQCCheckBT != null && item.OQCCheckFT == null)  //如果只有开始时间，没有结束时间，把此记录调出来
                        {
                            burn_in = item;
                            return RedirectToAction("Burn_in_F", new { burn_in.Id });
                        }
                    }

                    if (burn_in.OrderNum == db.BarCodes.Where(u => u.BarCodesNum == burn_in.BarCodesNum).FirstOrDefault().OrderNum)
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
                        ModelState.AddModelError("", "该模组条码不属于所选订单，请选择正确的订单号！");
                        return View(burn_in);
                    }
                }
                else
                {
                    //return Content("<script>alert('此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！');window.location.href='../Burn_in';</script>");
                    ModelState.AddModelError("", "此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！");
                    return View(burn_in);
                }
            }
            else
            {
                return RedirectToAction("Burn_in_F", new { burn_in.Id });
                //return RedirectToAction("Index");
            }
        }
        #endregion

        #region -----------------------老化异常录入--------------------------------

        public ActionResult AbnormalRecordInput()
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

            ViewBag.RepairList = SetRepairList();
            ViewBag.record = null;

            return View();
        }

        //[HttpPost]
        public ActionResult Get_Burn_in_Record(string OrderNum, string BarCodeNum)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Burn_in record = db.Burn_in.Where(m => m.OrderNum == OrderNum && m.BarCodesNum == BarCodeNum && m.OQCCheckBT != null && m.OQCCheckFT == null).FirstOrDefault();
            if (record != null)
            {
                List<Object> recordData = new List<object>();
                recordData.Add(new
                {
                    RecordId = record.Id,
                    RecordOrderNum = record.OrderNum,
                    RecordBarCodesNum = record.BarCodesNum,
                    RecordOQCCheckBT = record.OQCCheckBT.ToString(),
                    recordOQCPrincipal = record.OQCPrincipal,
                    RecordBurn_in_OQCCheckAbnormal_old = record.Burn_in_OQCCheckAbnormal
                });
                return Json(recordData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content("不存在正在进行老化的此模组");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> AbnormalRecordInput(string Id, string OrderNum, string BarCodeNum, string RepairCondition,string OQCPrincipal,string Burn_in_OQCCheckAbnormal)
        public async Task<ActionResult> AbnormalRecordInput(string Id, string Burn_in_OQCCheckAbnormal)
        //public async Task<ActionResult> AbnormalRecordInput([Bind(Include = "Id,Burn_in_OQCCheckAbnormal")] Burn_in burn_in)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Burn_in burn_in = await db.Burn_in.FindAsync(Convert.ToInt32(Id));

            if (burn_in == null)
            {
                return HttpNotFound();
            }
            else
            {
                burn_in.Burn_in_OQCCheckAbnormal = burn_in.Burn_in_OQCCheckAbnormal + Burn_in_OQCCheckAbnormal;
                burn_in.RepairCondition = "现场维修";

                if (ModelState.IsValid)
                {
                    db.Entry(burn_in).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.RepairList = SetRepairList();
            return View(burn_in);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> AbnormalRecordInput([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Burn_in_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Burn_in burn_in)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(burn_in).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.RepairList = SetRepairList();

        //    return View(burn_in);
        //}
        #endregion

        #region -------------单个模组老化完成------------
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
                //var BT = burn_in.OQCCheckBT.Value;
                //var FT = burn_in.OQCCheckFT.Value;
                //var CT = FT - BT;
                var CT = burn_in.OQCCheckFT.Value - burn_in.OQCCheckBT.Value;
                //burn_in.OQCCheckTime = CT;
                //burn_in.OQCCheckTime = new TimeSpan(CT.Days,CT.Hours,CT.Minutes,CT.Seconds);
                //burn_in.OQCCheckTime = (DateTime.Now-burn_in.OQCCheckBT).Value;
                burn_in.OQCPrincipal = ((Users)Session["User"]).UserName;
                burn_in.OQCCheckTimeSpan = CT.Days.ToString() + "天" + CT.Hours.ToString() + "时" + CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
                if(burn_in.Burn_in_OQCCheckAbnormal==null)
                {
                    burn_in.Burn_in_OQCCheckAbnormal = "正常";
                }
                if (burn_in.RepairCondition == null)
                {
                    burn_in.RepairCondition= "正常";
                }
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


        #region ---------------------------------------批量模组老化开始--------------------------------------------


        // GET: Burn_in/Burn_in_B
        public ActionResult Burn_in_Batch_B()
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
        public ActionResult Burn_in_Batch_B(string OrderNum, List<string> BarCodesNumList)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }


            return RedirectToAction("Index");
        }

        #region ---------------批量模组开始老化功能------------
        [HttpPost]
        public ActionResult Burn_in_Batch_Begin(string OrderNum, List<string> BarCodesNumList)
        {
            var newRecord = new Burn_in();
            newRecord.OrderNum = OrderNum;
            foreach (var item in BarCodesNumList)
            {
                newRecord.BarCodesNum = item;
                newRecord.OQCCheckBT = DateTime.Now;
                newRecord.OQCPrincipal = ((Users)Session["User"]).UserName;
                db.Burn_in.Add(newRecord);
                db.SaveChanges();
            }
            return Content("完成录入");
            //return Json(data,JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region --------------批量模组查验功能--------------
        public ActionResult CheckBarCodesList_B(string OrderNum, List<string> BarCodesNumList)
        {
            var barcodelist = new List<Object>();  //1.条码
            var barcodeinorder = new List<Object>(); //2.是否在订单中
            var barcodeinotherorder = new List<Object>();//3.在其他订单号中
            var barcodebegintime = new List<string>();//4.开始时间
            var barcodefinishtime = new List<string>();//5.完成时间
            var barcodestatus = new List<Object>();//6.老化状态
            var barcodecomfirm = new List<Object>();//7.可以进行老化
            var data = new List<Object>();

            ////测试用数据
            //OrderNum = "2017-TEST-1";
            //BarCodesNumList = (from m in db.BarCodes where m.OrderNum==OrderNum select m.BarCodesNum).ToList();
            //BarCodesNumList.Add("2017-TEST-1A00001");
            //BarCodesNumList.Add("2017-TEST-1A00002");
            //BarCodesNumList.Add("2017-TEST-1A00003");
            //BarCodesNumList.Add("2017-TEST-1A00004");
            //BarCodesNumList.Add("2017-TEST-1A00005");

            foreach (var barcode in BarCodesNumList)
            {
                //一、检查此条码是否在此订单中（如果不存在，则返回条码所属的订单号，返回条码老化工序的状态，并跳出foreach循环）
                var barcodelistinOrderNum = from m in db.BarCodes where m.OrderNum == OrderNum select m.BarCodesNum;//取出订单的所有条码号
                var barcodeCountin = (from m in barcodelistinOrderNum where m == barcode select m).Count();//查询条码在订单中和条目数
                //订单中有此条码
                if (barcodeCountin == 1)
                {
                    //二、检查此条码是否已经正在老化（已经开始老化，但还没有老化完成时间,返回在何时已经开始了老化）
                    //三、检查此条码是否已经通过了老化（已经完成了老化工作），如果完成了，返回在何时已经开始、完成了老化

                    //此条码在OrderNum订单中
                    barcodelist.Add(barcode);//1.条码
                    barcodeinorder.Add("Yes");//2.是否在订单中
                    barcodeinotherorder.Add("");//3.在其他订单号中

                    //取出所有此条码的老化记录列表(包括已经开始老化未完成的、已经老化完成的)
                    var burninrecords = from m in db.Burn_in where m.BarCodesNum == barcode select m;

                    //无记录
                    if (burninrecords.Count() == 0)
                    {
                        //此条码在OrderNum订单中
                        barcodebegintime.Add("");//4.开始时间
                        barcodefinishtime.Add("");//5.完成时间
                        barcodestatus.Add("未开始老化");//6.老化状态
                        barcodecomfirm.Add("Yes");//7.可以进行老化
                    }
                    //一个记录
                    if (burninrecords.Count() == 1)
                    {
                        barcodebegintime.Add(Convert.ToString(burninrecords.FirstOrDefault().OQCCheckBT));//4.开始时间
                        if (burninrecords.FirstOrDefault().OQCCheckFT != null && burninrecords.FirstOrDefault().OQCCheckFinish == true)
                        {
                            //有一个记录，并且已经完成的
                            barcodefinishtime.Add(Convert.ToString(burninrecords.FirstOrDefault().OQCCheckFT));//5.完成时间
                            barcodestatus.Add("已经完成老化"); //6.老化状态
                            barcodecomfirm.Add("No");//7.可以进行老化
                        }
                        else if (burninrecords.FirstOrDefault().OQCCheckFT == null && burninrecords.FirstOrDefault().OQCCheckFinish == false)
                        {
                            //有一个记录，没有完成时间的
                            barcodefinishtime.Add("");//5.完成时间
                            barcodestatus.Add("正在老化"); //6.老化状态
                            barcodecomfirm.Add("No");//7.可以进行老化
                        }
                        else if (burninrecords.FirstOrDefault().OQCCheckFT != null && burninrecords.FirstOrDefault().OQCCheckFinish == false && burninrecords.FirstOrDefault().Burn_in_OQCCheckAbnormal != "正常")
                        {
                            //有一个记录，已经完成老化，有异常，没有通过OQC
                            barcodefinishtime.Add(Convert.ToString(burninrecords.FirstOrDefault().OQCCheckFT));//5.完成时间
                            barcodestatus.Add("未通过老化"); //6.老化状态
                            barcodecomfirm.Add("Yes");//7.可以进行老化
                        }
                    }
                    //超过一个记录
                    if (burninrecords.Count() > 1)
                    {
                        int finisttrue = (from m in burninrecords where m.OQCCheckFinish == true select m).Count();
                        //有一个完成记录
                        if (finisttrue == 1)
                        {
                            var record = (from m in burninrecords where m.OQCCheckFinish == true select m).ToList();
                            barcodebegintime.Add(Convert.ToString(record.FirstOrDefault().OQCCheckBT));//4.开始时间
                            barcodefinishtime.Add(Convert.ToString(record.FirstOrDefault().OQCCheckFT));//5.完成时间
                            barcodestatus.Add("经过" + burninrecords.Count() + "次老化，已经完成老化"); //6.老化状态
                            barcodecomfirm.Add("No");//7.可以进行老化
                        }
                        //没有完成记录
                        else if (finisttrue == 0)
                        {
                            //检查是否有正在进行老化的记录，有、无
                            if (burninrecords.Where(m => m.OQCCheckFT == null).ToList().Count() == 1)
                            {
                                var record = burninrecords.Where(m => m.OQCCheckFT == null).ToList().FirstOrDefault();
                                barcodebegintime.Add(Convert.ToString(record.OQCCheckBT));//4.开始时间
                                barcodefinishtime.Add("");//5.完成时间
                                barcodestatus.Add("第" + burninrecords.Count() + "次老化，正在老化"); //6.老化状态
                                barcodecomfirm.Add("No");//7.可以进行老化
                            }
                            else
                            {
                                barcodebegintime.Add("");//4.开始时间
                                barcodefinishtime.Add("");//5.完成时间
                                barcodestatus.Add("经过" + burninrecords.Count() + "次老化，但未通过老化"); //6.老化状态
                                barcodecomfirm.Add("Yes");//7.可以进行老化
                            }
                        }
                    }
                }
                //订单中没有此条码
                else if (barcodeCountin == 0)
                {
                    var record = from m in db.Burn_in where m.BarCodesNum == barcode select m;
                    var AnotherOrderNum = from m in db.BarCodes where m.BarCodesNum == barcode select m.OrderNum;
                    barcodelist.Add(barcode);//1.条码
                    barcodeinorder.Add("No"); //2.是否在订单中
                    barcodeinotherorder.Add(AnotherOrderNum);//3.在其他订单号中
                    barcodebegintime.Add("");//4.开始时间
                    barcodefinishtime.Add("");//5.完成时间
                    barcodestatus.Add(""); //6.老化状态
                    barcodecomfirm.Add("");//7.可以进行老化
                    //if (record.Count()>0)
                    //{
                    //    var finishtrue = (from m in record where m.OQCCheckFinish == true select m).Count();
                    //    if(finishtrue==1)
                    //    {
                    //       barcodestatus.Add("老化完成"); //6.老化状态
                    //       barcodecomfirm.Add("NO");//7.可以进行老化
                    //    }
                    //    else if (finishtrue==0)
                    //    {
                    //        barcodestatus.Add("经过"+record.Count()+"次老化，但未完成老化"); //6.老化状态
                    //        barcodecomfirm.Add("Yes");//7.可以进行老化
                    //    }
                    //}
                    //else
                    //{
                    //    barcodestatus.Add("未开始老化");//6.老化状态
                    //    barcodecomfirm.Add("Yes");//7.可以进行老化
                    //}
                }
            }
            data.Add(new
            {
                BarcodeList = barcodelist,
                BarcodeInOrder = barcodeinorder,
                BarcodeInOtherOrder = barcodeinotherorder,
                BarcodeBeginTime = barcodebegintime,
                BarcodeFinishTime = barcodefinishtime,
                BarcodeStatus = barcodestatus,
                BarcodeComfirm = barcodecomfirm
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region ------------------------------------批量模组老化完成（修改中）---------------------------------------------

        // GET: Burn_in/Burn_in_F
        public ActionResult Burn_in_Batch_F()
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

            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Burn_in burn_in = db.Burn_in.Find(id);
            //if (burn_in == null)
            //{
            //    return HttpNotFound();
            //}
            //ViewBag.RepairList = SetRepairList();
            return RedirectToAction("Index");
        }

        // POST: Burn_in/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Burn_in_Batch_F(string OrderNum, List<string> BarCodesNumList)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }


            return RedirectToAction("Index");
        }

        #region --------------批量模组查验功能--------------
        public ActionResult CheckBarCodesList_F(string OrderNum, List<string> BarCodesNumList)
        {
            var barcodelist = new List<Object>();  //1.条码
            var barcodeinorder = new List<Object>(); //2.是否在订单中
            var barcodeinotherorder = new List<Object>();//3.在其他订单号中
            var barcodebegintime = new List<string>();//4.开始时间
            var barcodefinishtime = new List<string>();//5.完成时间
            var barcodestatus = new List<Object>();//6.老化状态
            var barcodecomfirm = new List<Object>();//7.可以进行老化完成
            var data = new List<Object>();


            foreach (var barcode in BarCodesNumList)
            {
                //一、检查此条码是否在此订单中（如果不存在，则返回条码所属的订单号，返回条码老化工序的状态，并跳出foreach循环）
                var barcodelistinOrderNum = from m in db.BarCodes where m.OrderNum == OrderNum select m.BarCodesNum;//取出订单的所有条码号
                var barcodeCountin = (from m in barcodelistinOrderNum where m == barcode select m).Count();//查询条码在订单中和条目数
                //订单中有此条码
                if (barcodeCountin == 1)
                {
                    //二、检查此条码是否已经正在老化（已经开始老化，但还没有老化完成时间,返回在何时已经开始了老化）
                    //三、检查此条码是否已经通过了老化（已经完成了老化工作），如果完成了，返回在何时已经开始、完成了老化

                    //此条码在OrderNum订单中
                    barcodelist.Add(barcode);//1.条码
                    barcodeinorder.Add("Yes");//2.是否在订单中
                    barcodeinotherorder.Add("");//3.在其他订单号中

                    //取出所有此条码的老化记录列表(包括已经开始老化未完成的、已经老化完成的)
                    var burninrecords = from m in db.Burn_in where m.BarCodesNum == barcode select m;

                    //无记录
                    if (burninrecords.Count() == 0)
                    {
                        //此条码在OrderNum订单中
                        barcodebegintime.Add("");//4.开始时间
                        barcodefinishtime.Add("");//5.完成时间
                        barcodestatus.Add("未开始老化");//6.老化状态
                        barcodecomfirm.Add("No");//7.可以进行老化完成
                    }
                    //一个记录
                    if (burninrecords.Count() == 1)
                    {
                        barcodebegintime.Add(Convert.ToString(burninrecords.FirstOrDefault().OQCCheckBT));//4.开始时间
                        if (burninrecords.FirstOrDefault().OQCCheckFT != null && burninrecords.FirstOrDefault().OQCCheckFinish == true)
                        {
                            //有一个记录，并且已经完成的
                            barcodefinishtime.Add(Convert.ToString(burninrecords.FirstOrDefault().OQCCheckFT));//5.完成时间
                            barcodestatus.Add("已经完成老化"); //6.老化状态
                            barcodecomfirm.Add("No");//7.可以进行老化完成
                        }
                        else if (burninrecords.FirstOrDefault().OQCCheckFT == null && burninrecords.FirstOrDefault().OQCCheckFinish == false)
                        {
                            //有一个记录，没有完成时间的
                            barcodefinishtime.Add("");//5.完成时间
                            barcodestatus.Add("正在老化"); //6.老化状态
                            barcodecomfirm.Add("Yes");//7.可以进行老化完成
                        }
                        else if (burninrecords.FirstOrDefault().OQCCheckFT != null && burninrecords.FirstOrDefault().OQCCheckFinish == false && burninrecords.FirstOrDefault().Burn_in_OQCCheckAbnormal != "正常")
                        {
                            //有一个记录，已经完成老化，有异常，没有通过OQC
                            barcodefinishtime.Add(Convert.ToString(burninrecords.FirstOrDefault().OQCCheckFT));//5.完成时间
                            barcodestatus.Add("未通过老化"); //6.老化状态
                            barcodecomfirm.Add("No");//7.可以进行老化完成
                        }
                    }
                    //超过一个记录
                    if (burninrecords.Count() > 1)
                    {
                        int finisttrue = (from m in burninrecords where m.OQCCheckFinish == true select m).Count();
                        //有一个完成记录
                        if (finisttrue == 1)
                        {
                            var record = (from m in burninrecords where m.OQCCheckFinish == true select m).ToList();
                            barcodebegintime.Add(Convert.ToString(record.FirstOrDefault().OQCCheckBT));//4.开始时间
                            barcodefinishtime.Add(Convert.ToString(record.FirstOrDefault().OQCCheckFT));//5.完成时间
                            barcodestatus.Add("经过" + burninrecords.Count() + "次老化，已经完成老化"); //6.老化状态
                            barcodecomfirm.Add("No");//7.可以进行老化
                        }
                        //没有完成记录
                        else if (finisttrue == 0)
                        {
                            //检查是否有正在进行老化的记录，有、无
                            if (burninrecords.Where(m => m.OQCCheckFT == null).ToList().Count() == 1)
                            {
                                var record = burninrecords.Where(m => m.OQCCheckFT == null).ToList().FirstOrDefault();
                                barcodebegintime.Add(Convert.ToString(record.OQCCheckBT));//4.开始时间
                                barcodefinishtime.Add("");//5.完成时间
                                barcodestatus.Add("第" + burninrecords.Count() + "次老化，正在老化"); //6.老化状态
                                barcodecomfirm.Add("Yes");//7.可以进行老化
                            }
                            else
                            {
                                barcodebegintime.Add("");//4.开始时间
                                barcodefinishtime.Add("");//5.完成时间
                                barcodestatus.Add("经过" + burninrecords.Count() + "次老化，但未通过老化"); //6.老化状态
                                barcodecomfirm.Add("No");//7.可以进行老化
                            }
                        }
                    }
                }
                //订单中没有此条码
                else if (barcodeCountin == 0)
                {
                    var record = from m in db.Burn_in where m.BarCodesNum == barcode select m;
                    var AnotherOrderNum = from m in db.BarCodes where m.BarCodesNum == barcode select m.OrderNum;
                    barcodelist.Add(barcode);//1.条码
                    barcodeinorder.Add("No"); //2.是否在订单中
                    barcodeinotherorder.Add(AnotherOrderNum);//3.在其他订单号中
                    barcodebegintime.Add("");//4.开始时间
                    barcodefinishtime.Add("");//5.完成时间
                    barcodestatus.Add(""); //6.老化状态
                    barcodecomfirm.Add("");//7.可以进行老化
                    //if (record.Count()>0)
                    //{
                    //    var finishtrue = (from m in record where m.OQCCheckFinish == true select m).Count();
                    //    if(finishtrue==1)
                    //    {
                    //       barcodestatus.Add("老化完成"); //6.老化状态
                    //       barcodecomfirm.Add("NO");//7.可以进行老化
                    //    }
                    //    else if (finishtrue==0)
                    //    {
                    //        barcodestatus.Add("经过"+record.Count()+"次老化，但未完成老化"); //6.老化状态
                    //        barcodecomfirm.Add("Yes");//7.可以进行老化
                    //    }
                    //}
                    //else
                    //{
                    //    barcodestatus.Add("未开始老化");//6.老化状态
                    //    barcodecomfirm.Add("Yes");//7.可以进行老化
                    //}
                }
            }
            data.Add(new
            {
                BarcodeList = barcodelist,
                BarcodeInOrder = barcodeinorder,
                BarcodeInOtherOrder = barcodeinotherorder,
                BarcodeBeginTime = barcodebegintime,
                BarcodeFinishTime = barcodefinishtime,
                BarcodeStatus = barcodestatus,
                BarcodeComfirm = barcodecomfirm
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ---------------批量模组完成老化功能------------
        [HttpPost]
        public ActionResult Burn_in_Batch_Finish(string OrderNum, List<string> BarCodesNumList)
        {
            var burn_in_List = from m in db.Burn_in where m.OrderNum == OrderNum select m;
            foreach (var item in BarCodesNumList)
            {
                var burn_in = (from x in burn_in_List where (x.BarCodesNum == item && x.OQCCheckFT == null) select x).FirstOrDefault();
                burn_in.OQCCheckFT =DateTime.Now;
                var CT = burn_in.OQCCheckFT.Value - burn_in.OQCCheckBT.Value;
                burn_in.OQCCheckTimeSpan = CT.Days.ToString() + "天" + CT.Hours.ToString() + "时" + CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
                burn_in.OQCPrincipal = ((Users)Session["User"]).UserName;
                if (burn_in.Burn_in_OQCCheckAbnormal == null)
                {
                    burn_in.Burn_in_OQCCheckAbnormal = "正常";
                }
                if (burn_in.RepairCondition == null)
                {
                    burn_in.RepairCondition = "正常";
                }
                burn_in.OQCCheckFinish = true;
                db.SaveChanges();
            }
            return Content("老化完成");
        }
        #endregion

        #region  ------------------以前的代码------------
        //// GET: Burn_in/Burn_in_F
        //public ActionResult Burn_in_Batch_F(int? id)
        //{
        //    if (Session["User"] == null)
        //    {
        //        return RedirectToAction("Login", "Users");
        //    }

        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Burn_in burn_in = db.Burn_in.Find(id);
        //    if (burn_in == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.RepairList = SetRepairList();
        //    return View(burn_in);
        //}

        //// POST: Burn_in/Edit/5
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Burn_in_Batch_F([Bind(Include = "Id,OrderNum,BarCodesNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Burn_in_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Burn_in burn_in)
        //{
        //    if (Session["User"] == null)
        //    {
        //        return RedirectToAction("Login", "Users");
        //    }
        //    if (burn_in.OQCCheckFT == null)
        //    {
        //        burn_in.OQCCheckFT = DateTime.Now;
        //        burn_in.OQCPrincipal = ((Users)Session["User"]).UserName;
        //        var BT = burn_in.OQCCheckBT.Value;
        //        var FT = burn_in.OQCCheckFT.Value;
        //        var CT = FT - BT;
        //        burn_in.OQCCheckTime = CT;
        //        burn_in.OQCCheckTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
        //        burn_in.OQCCheckFinish = true;
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(burn_in).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Burn_in_B");
        //    }
        //    ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
        //    return View(burn_in);
        //}

        //public ActionResult CheckBarCodesList_F(string barcodeslist)
        //{
        //    string returnjson = null;
        //    return Content(returnjson);
        //}
        #endregion

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
