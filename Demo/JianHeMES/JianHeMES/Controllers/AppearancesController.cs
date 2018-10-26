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

        #region --------------------OQCNormal列表
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


        #region --------------------维修列表-----------

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


        #region --------------------外观首页---------
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
        public ActionResult Index(string OrderNum, string BoxBarCode, string AppearancesNormal, string searchString, int PageIndex = 0)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            //var AllAppearanceRecords = db.Appearance as IQueryable<Appearance>;
            List<Appearance> AllAppearanceRecords = new List<Appearance>(); ;
            List<Appearance> AllAppearanceRecordsList = null;
            //计算外观包装用时
            TimeSpan TotalTime = new TimeSpan();
            

            if (String.IsNullOrEmpty(OrderNum))
            {
                //调出全部记录      
                //AllAppearanceRecords = from m in db.Appearance
                //                       select m;
                AllAppearanceRecords = db.Appearance.ToList();
                ViewBag.TotalTime = "";
            }
            else
            {
                //筛选出对应orderNum所有记录
                //AllAppearanceRecords = from m in db.Appearance
                //                       where (m.OrderNum == OrderNum)
                //                       select m;
                AllAppearanceRecords = db.Appearance.Where(c => c.OrderNum == OrderNum).ToList();
                if (AllAppearanceRecords.Count() == 0)
                {
                    var barcodelist = db.BarCodes.Where(c => c.ToOrderNum == OrderNum).ToList();
                    //..TODO..待优化
                    foreach (var item in barcodelist)
                    {
                        AllAppearanceRecords.AddRange(db.Appearance.Where(c => c.BarCodesNum == item.BarCodesNum));
                    }
                }
                if(AllAppearanceRecords.Max(c => c.OQCCheckFT)!=null && AllAppearanceRecords.Min(c => c.OQCCheckBT)!=null)
                {
                    TotalTime = Convert.ToDateTime(AllAppearanceRecords.Max(c => c.OQCCheckFT)) - Convert.ToDateTime(AllAppearanceRecords.Min(c => c.OQCCheckBT));
                    ViewBag.TotalTime = TotalTime.Hours.ToString() + "小时" + TotalTime.Minutes.ToString() + "分" + TotalTime.Seconds.ToString() + "秒";
                }

            }

            

            //统计外观包装结果正常的模组数量
            var Order_CR_Normal_Count = AllAppearanceRecords.Where(x => x.RepairCondition == "正常").Count();
            var Abnormal_Count = AllAppearanceRecords.Where(x => x.Appearance_OQCCheckAbnormal != "正常").Count();

            #region  ---------按条码筛选--------------
            if (BoxBarCode != "")
            {
                AllAppearanceRecords = AllAppearanceRecords.Where(x => x.BarCodesNum == BoxBarCode).ToList();
            }
            #endregion

            #region   ---------筛选正常、异常-------------
            //正常、异常记录筛选
            if (AppearancesNormal == "异常")
            {
                AllAppearanceRecords = AllAppearanceRecords.Where(c => c.Appearance_OQCCheckAbnormal != "正常").ToList();
                //AllAppearanceRecords = from m in AllAppearanceRecords where (m.Appearance_OQCCheckAbnormal != "正常") select m;
            }
            else if (AppearancesNormal == "正常")
            {
                AllAppearanceRecords = AllAppearanceRecords.Where(c => c.Appearance_OQCCheckAbnormal == "正常").ToList();
                //AllAppearanceRecords = from m in AllAppearanceRecords where (m.Appearance_OQCCheckAbnormal == "正常") select m;
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

            //检查searchString是否为空
            if (!String.IsNullOrEmpty(searchString))
            {   //从调出的记录中筛选含searchString内容的记录
                AllAppearanceRecords = AllAppearanceRecords.Where(m => m.Appearance_OQCCheckAbnormal!=null && m.Appearance_OQCCheckAbnormal.Contains(searchString)).ToList();
            }

            //取出对应orderNum外观包装时长所有记录
            IQueryable<TimeSpan?> TimeSpanList = from m in db.Appearance
                                                 where (m.OrderNum == OrderNum)
                                                 orderby m.OQCCheckTime
                                                 select m.OQCCheckTime;
            //计算外观包装总时长
            TimeSpan TotalTimeSpan = new TimeSpan();
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
            AllAppearanceRecords = AllAppearanceRecords.OrderBy(m => m.BarCodesNum)
                                .Skip(PageIndex * PAGE_SIZE)
                                .Take(PAGE_SIZE).ToList();
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageCount = pageCount;
            ViewBag.OrderNumList = GetOrderNumList();
            ViewBag.AppearancesNormal = AppearancesNormalList();

            return View(AllAppearanceRecords);
            //return View(AllAppearanceRecordsList);
        }
        #endregion


        #region --------------------其他页面
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
        public async Task<ActionResult> Create([Bind(Include = "Id,OrderNum,BarCodesNum,ModuleGroupNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Appearance_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Appearance appearance)
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
        public async Task<ActionResult> Edit([Bind(Include = "Id,OrderNum,BarCodesNum,ModuleGroupNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Appearance_OQCCheckAbnormal,RepairCondition,OQCCheckFinish")] Appearance appearance)
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
        #endregion


        #region --------------------外观电检包装开始-----------------


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
        public async Task<ActionResult> Appearance_B([Bind(Include = "Id,OrderNum,ToOrderNum,BarCodesNum,ModuleGroupNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Appearance_OQCCheckAbnormal,RepairCondition,OQCCheckFinish,Remark")] Appearance appearance)
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.


            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum) == null)
            {
                ModelState.AddModelError("", "模组条码不存在，请检查条码输入是否正确！");
                return View(appearance);
            }
            //对应条码的全部外观包装记录
            var appearanceRecord = db.Appearance.Where(u => u.BarCodesNum == appearance.BarCodesNum).ToList();
            appearance.OQCCheckBT = DateTime.Now;
            appearance.OQCPrincipal = ((Users)Session["User"]).UserName;
            //增加模组箱体号到外观记录中
            appearance.ModuleGroupNum = (from m in db.BarCodes where m.BarCodesNum == appearance.BarCodesNum select m).FirstOrDefault().ModuleGroupNum;
            var barcoderecord = db.BarCodes.FirstOrDefault(c => c.BarCodesNum == appearance.BarCodesNum);

            //在Appearance条码表中找不到记录
            if (appearanceRecord.Count()==0)
            {
                if (appearance.OrderNum == db.BarCodes.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum).OrderNum)//检查条码是否存在
                {
                    //修改BarCodes表中的ToOrderNum记录
                    if (barcoderecord.IsRepertory == true) //如果是库存条码
                    {
                        //转新订单已经存在
                        if (barcoderecord.ToOrderNum != null) 
                        {
                            ModelState.AddModelError("", "此模组条码已经绑定给订单" + barcoderecord.ToOrderNum + "了，请选择正确的库存订单号和新订单号！");
                            return View(appearance);
                        }
                        //转新订单为空
                        else
                        {
                            barcoderecord.ToOrderNum = appearance.ToOrderNum;//添加新订单ToOrderNum到条码表的ToOrderNum中
                            db.Entry(barcoderecord).State = EntityState.Modified;  //修改记录状态
                            db.Appearance.Add(appearance); //添加外观记录
                            await db.SaveChangesAsync();   //保存记录
                            return RedirectToAction("Appearance_F", new { appearance.Id });
                        }
                    }
                    else //如果不是库存条码，走正常保存流程
                    {
                        db.Appearance.Add(appearance);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Appearance_F", new { appearance.Id });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "此模组条码不属于所选订单，应该属于"+ db.BarCodes.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum).OrderNum + "订单，请选择正确的订单号！");
                    return View(appearance);
                }
            }

            //在Appearance条码表中找到1条以上记录
            else if (appearanceRecord.Count() >= 1)
            {
                int doing_appearance_Count = appearanceRecord.Count(m=>m.OQCCheckBT != null && m.OQCCheckFT == null);//统计正在外观包装记录数量
                int normalCount = appearanceRecord.Where(m => m.RepairCondition == "正常" && m.OQCCheckFinish == true).Count();//统计完成记录数量
                //没有完成外观包装记录，没有正在做外观包装记录，可能有1个以上异常记录
                if (normalCount == 0 && doing_appearance_Count == 0)
                {
                    //检验条码订单号是否正确存在
                    if (appearance.OrderNum == db.BarCodes.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum).OrderNum) 
                    {
                        appearance.OrderNum = db.BarCodes.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum).OrderNum;
                        appearance.OQCCheckBT = DateTime.Now;
                        appearance.OQCPrincipal = ((Users)Session["User"]).UserName;
                        //增加模组箱体号到外观记录中
                        appearance.ModuleGroupNum = (from m in db.BarCodes where m.BarCodesNum == appearance.BarCodesNum select m).FirstOrDefault().ModuleGroupNum;
                        string toordernum = db.BarCodes.FirstOrDefault(c => c.BarCodesNum == appearance.BarCodesNum).ToOrderNum;
                        if (appearance.ToOrderNum!= toordernum)
                        {
                            ModelState.AddModelError("", "该模组条码已经绑定给订单" + toordernum + "了，请选择正确的新订单号！");
                            return View(appearance);
                        }
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
                //没有完成外观包装记录，有1个以上正在做外观包装记录
                else if (normalCount == 0 && doing_appearance_Count >= 1 )
                {
                    var appearance_doing_record = appearanceRecord.FirstOrDefault(m => m.OQCCheckBT != null && m.OQCCheckFT == null);//读出第一个正在做外观包装的记录
                    return RedirectToAction("Appearance_F", new { appearance_doing_record.Id }); //在“完成外观包装”页面打开正在做外观包装的记录，完成关闭外观包装操作
                }
                //有完成外观包装记录
                else if (normalCount >= 1 && doing_appearance_Count == 0)
                {
                    //如果是库存条码,转订单操作
                    if ( appearance.ToOrderNum!=null && barcoderecord.IsRepertory == true)
                    {
                        if (barcoderecord.ToOrderNum != null) //转新订单已经存在
                        {
                            ModelState.AddModelError("", "此模组条码已经绑定给订单" + barcoderecord.ToOrderNum + "了，请选择正确的库存订单号和新订单号！");
                            return View(appearance);
                        }
                        else//转新订单为空
                        {
                            barcoderecord.ToOrderNum = appearance.ToOrderNum;//添加新订单ToOrderNum
                            db.Entry(barcoderecord).State = EntityState.Modified;
                            db.Appearance.Add(appearance);// 是否重复存储？
                            await db.SaveChangesAsync();
                            return RedirectToAction("Appearance_F", new { appearance.Id });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！");
                        return View(appearance);
                    }
                }
                else if (normalCount >= 1 && doing_appearance_Count >= 1)
                {
                    var appearance_doing_record = appearanceRecord.FirstOrDefault(m => m.OQCCheckBT != null && m.OQCCheckFT == null);//读出第一个正在做外观包装的记录
                    return RedirectToAction("Appearance_F", new { appearance_doing_record.Id }); //在“完成外观包装”页面打开正在做外观包装的记录，完成关闭外观包装操作
                }
                else
                {
                    //return Content("<script>alert('此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！');window.location.href='../Appearances';</script>");
                    ModelState.AddModelError("", "此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！");
                    return View(appearance);
                }
            }
            //其他情况
            else
            {
                ModelState.AddModelError("", "特殊异常情况，请联系系统管理员！");
                return View(appearance);
            }
        }
        #endregion


        #region --------------------外观电检包装完成------------
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
        public async Task<ActionResult> Appearance_F([Bind(Include = "Id,OrderNum,ToOrderNum,BarCodesNum,ModuleGroupNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Appearance_OQCCheckAbnormal,RepairCondition,OQCCheckFinish,Remark")] Appearance appearance)
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
                if (appearance.Appearance_OQCCheckAbnormal == null)
                {
                    appearance.Appearance_OQCCheckAbnormal = "正常";
                }
                if (appearance.Appearance_OQCCheckAbnormal == "正常" && appearance.RepairCondition == "正常")
                {
                    appearance.OQCCheckFinish = true;
                }
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



        #region --------------------GetOrderList()取出整个OrderMgms的OrderNum订单号列表.--------------------------------------------------
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
        //----------------------------------------------------------------------------------------
        #endregion

        #region --------------------检索订单号------
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

        #region --------------------分页------------
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
