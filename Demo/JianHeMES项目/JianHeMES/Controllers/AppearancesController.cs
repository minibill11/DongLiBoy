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
using System.Web.Routing;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;

namespace JianHeMES.Controllers
{
    public class AppearancesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController common = new CommonController();
        public class TempCalibration
        {
            public int id { get; set; }
            public string barcode { get; set; }
            public string oldbarcode { get; set; }
            public string module { get; set; }
        }
        public class TempAppearabce
        {
            public int id { get; set; }
            public string barcode { get; set; }
            public string oldbarcode { get; set; }
            public string module { get; set; }
        }


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

        #region ---------------------------------------删除规则审核
        public ActionResult DeleteConfirm()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Appearances", act = "DeleteConfirm" });
            }
            return View();
        }
        #endregion
        #region --------------------外观首页---------
        // GET: Appearances
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Appearances", act = "Index" });
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
        public ActionResult Index(string OrderNum, string BoxBarCode, string AppearancesNormal, string searchString/*, int PageIndex = 0*/)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Appearances", act = "Index" });
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
                AllAppearanceRecords = db.Appearance.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).ToList();
                if (AllAppearanceRecords.Count() == 0)
                {
                    var barcodelist = db.BarCodes.Where(c => c.ToOrderNum == OrderNum).ToList();
                    //..TODO..待优化
                    foreach (var item in barcodelist)
                    {
                        AllAppearanceRecords.AddRange(db.Appearance.Where(c => c.BarCodesNum == item.BarCodesNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == item.BarCodesNum)));
                    }
                }
                if (AllAppearanceRecords.Max(c => c.OQCCheckFT) != null && AllAppearanceRecords.Min(c => c.OQCCheckBT) != null)
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
                AllAppearanceRecords = AllAppearanceRecords.Where(x => x.BarCodesNum == BoxBarCode && (x.OldBarCodesNum == null || x.OldBarCodesNum == BoxBarCode)).ToList();
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
                if ((from m in db.Appearance where m.BarCodesNum == barcode.BarCodesNum && (m.OldOrderNum == null || m.OldOrderNum == OrderNum) select m).Count() == 0)
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
                AllAppearanceRecords = AllAppearanceRecords.Where(m => m.Appearance_OQCCheckAbnormal != null && m.Appearance_OQCCheckAbnormal.Contains(searchString)).ToList();
            }

            //取出对应orderNum外观包装时长所有记录
            IQueryable<TimeSpan?> TimeSpanList = from m in db.Appearance
                                                 where (m.OrderNum == OrderNum) && (m.OldOrderNum == null || m.OldOrderNum == OrderNum)
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
            AllAppearanceRecordsList = AllAppearanceRecords.OrderBy(c => c.BarCodesNum).ToList();

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
            //var recordCount = AllAppearanceRecords.Count();
            //var pageCount = GetPageCount(recordCount);
            //if (PageIndex >= pageCount && pageCount >= 1)
            //{
            //    PageIndex = pageCount - 1;
            //}
            //AllAppearanceRecords = AllAppearanceRecords.OrderBy(m => m.BarCodesNum)
            //                    .Skip(PageIndex * PAGE_SIZE)
            //                    .Take(PAGE_SIZE).ToList();
            //ViewBag.PageIndex = PageIndex;
            //ViewBag.PageCount = pageCount;
            ViewBag.OrderNumList = GetOrderNumList();
            ViewBag.AppearancesNormal = AppearancesNormalList();

            return View(AllAppearanceRecordsList);
            //return View(AllAppearanceRecordsList);
        }
        #endregion

        #region --------------------统一查看和编辑模组号
        public ActionResult DisplayTotalModule()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Appearances", act = "DisplayTotalModule" });
            }
            return View();
        }
        #endregion
        #region --------------------其他页面
        // GET: Appearances/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Appearances", act = "Details" + "/" + id.ToString() });
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
                return RedirectToAction("Login", "Users", new { col = "Appearances", act = "Create" });
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
                return RedirectToAction("Login", "Users", new { col = "Appearances", act = "Edit" + "/" + id.ToString() });
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
                common.DelteMudole(appearance.OrderNum, appearance.ModuleGroupNum);
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
                return RedirectToAction("Login", "Users", new { col = "Appearances", act = "Appearance_B" });
            }
            //var code = db.UserRolelistTable.Where(c => c.RolesName == "老化管理" && c.Discription == "开始外观电检包装").Select(c => c.RolesCode).FirstOrDefault().ToString();
            //string name = ((Users)Session["User"]).UserName;
            //int id = ((Users)Session["User"]).UserNum;
            //var codeList = db.Useroles.Where(c => c.UserName == name && c.UserID == id).Select(c => c.Roles).FirstOrDefault();
            CommonController com = new CommonController();
            //if (((Users)Session["User"]).Role == "外观包装OQC" || com.isCheckRole("老化管理", "开始外观电检包装", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            return View();
            //}
            //return RedirectToAction("Index");
        }


        // POST: Appearance/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Appearance_B([Bind(Include = "Id,OrderNum,ToOrderNum,BarCodesNum,CustomerBarCodesNum,ModuleGroupNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Appearance_OQCCheckAbnormal,RepairCondition,OQCCheckFinish,Remark,Department1,Group")] Appearance appearance, string nuoOrder = null, string nuoBarCode = null, string isnuo = null)
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum) == null)
            {
                ModelState.AddModelError("", "模组条码不存在，请检查条码输入是否正确！");
                return View(appearance);
            }
            //对应条码的全部外观包装记录
            var appearanceRecord = db.Appearance.Where(u => u.BarCodesNum == appearance.BarCodesNum && (u.OldBarCodesNum == appearance.BarCodesNum || u.OldBarCodesNum == null)).ToList();
            appearance.OldOrderNum = appearance.OrderNum;
            appearance.OldBarCodesNum = appearance.BarCodesNum;
            appearance.OQCCheckBT = DateTime.Now;
            appearance.OQCPrincipal = ((Users)Session["User"]).UserName;
            //从条码表中读取模组箱体号，增加模组箱体号到外观记录中
            appearance.ModuleGroupNum = (from m in db.BarCodes where m.BarCodesNum == appearance.BarCodesNum select m).FirstOrDefault().ModuleGroupNum;

            var barcoderecord = db.BarCodes.FirstOrDefault(c => c.BarCodesNum == appearance.BarCodesNum);
            //if (barcoderecord.IsRepertory != true)
            //{
            //    if (string.IsNullOrEmpty(appearance.ModuleGroupNum))
            //    {
            //        appearance.ModuleGroupNum = db.CalibrationRecord.Where(c => c.OrderNum == appearance.OrderNum && c.BarCodesNum == appearance.BarCodesNum).Select(c => c.ModuleGroupNum).FirstOrDefault();
            //        if (string.IsNullOrEmpty(appearance.ModuleGroupNum))
            //        {
            //            ModelState.AddModelError("", "该模组条码校正箱号为空，不允许开始！");
            //            return View(appearance);
            //        }
            //        else
            //        {
            //            var barcodeinfo = db.BarCodes.Where(c => c.BarCodesNum == appearance.BarCodesNum).FirstOrDefault();
            //            if (barcodeinfo != null)
            //            {
            //                barcodeinfo.ModuleGroupNum = appearance.ModuleGroupNum;
            //                db.Entry(barcodeinfo).State = EntityState.Modified;
            //                db.SaveChanges();
            //            }
            //        }
            //    }
            //}

            //在Appearance条码表中找不到记录
            if (appearanceRecord.Count() == 0)
            {
                if (appearance.OrderNum == db.BarCodes.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum).OrderNum)//检查条码是否存在
                {
                    ////修改BarCodes表中的ToOrderNum记录
                    //if (barcoderecord.IsRepertory == true) //如果是库存条码
                    //{
                    //    //转新订单已经存在
                    //    if (barcoderecord.ToOrderNum != null)
                    //    {
                    //        ModelState.AddModelError("", "此模组条码已经绑定给订单" + barcoderecord.ToOrderNum + "了，请选择正确的库存订单号和新订单号！");
                    //        return View(appearance);
                    //    }
                    //    //转新订单为空
                    //    else
                    ////    {
                    //        barcoderecord.ToOrderNum = appearance.ToOrderNum;//添加新订单ToOrderNum到条码表的ToOrderNum中
                    //        db.Entry(barcoderecord).State = EntityState.Modified;  //修改记录状态
                    //        db.Appearance.Add(appearance); //添加外观记录
                    //        await db.SaveChangesAsync();   //保存记录
                    //        //添加关联表
                    //        if (!string.IsNullOrEmpty(nuoBarCode))
                    //            comm.InsertRelation(nuoBarCode, appearance.BarCodesNum, appearance.OrderNum, "Appearances");
                    //        return RedirectToAction("Appearance_F", new { appearance.Id });
                    //   // }
                    //}
                    //else //如果不是库存条码，走正常保存流程
                    //{

                    db.Appearance.Add(appearance);
                    await db.SaveChangesAsync();
                    //添加关联表
                    if (!string.IsNullOrEmpty(nuoBarCode) && isnuo == "true")
                    {
                        BarCodeRelation barcoderelation = new BarCodeRelation() { OldOrderNum = nuoOrder, OldBarCodeNum = nuoBarCode, NewBarCodesNum = appearance.BarCodesNum, NewOrderNum = appearance.OrderNum, Procedure = "Appearances", UsserID = ((Users)Session["User"]).UserNum, CreateDate = DateTime.Now };
                        if (!comm.InsertRelation(barcoderelation))
                        {
                            ModelState.AddModelError("", "挪用订单失败，此条码已挪用过库存条码");
                            return View(appearance);
                        }
                        #region 挪用修改原条码工序记录
                        common.NuoOperation(nuoBarCode, nuoOrder, appearance.BarCodesNum, appearance.OrderNum);
                        #endregion
                    }
                    return RedirectToAction("Appearance_F", new { appearance.Id });
                    //}
                }
                else
                {
                    ModelState.AddModelError("", "此模组条码不属于所选订单，应该属于" + db.BarCodes.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum).OrderNum + "订单，请选择正确的订单号！");
                    return View(appearance);
                }
            }

            //在Appearance条码表中找到1条以上记录
            else if (appearanceRecord.Count() >= 1)
            {
                int doing_appearance_Count = appearanceRecord.Count(m => m.OQCCheckBT != null && m.OQCCheckFT == null);//统计正在外观包装记录数量
                int normalCount = appearanceRecord.Where(m => m.RepairCondition == "正常" && m.OQCCheckFinish == true).Count();//统计完成记录数量
                //没有完成外观包装记录，没有正在做外观包装记录，可能有1个以上异常记录
                if (normalCount == 0 && doing_appearance_Count == 0)
                {
                    //检验条码订单号是否正确存在
                    if (appearance.OrderNum == db.BarCodes.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum).OrderNum)
                    {
                        //appearance.OrderNum = db.BarCodes.FirstOrDefault(u => u.BarCodesNum == appearance.BarCodesNum).OrderNum;
                        appearance.OQCCheckBT = DateTime.Now;
                        appearance.OQCPrincipal = ((Users)Session["User"]).UserName;
                        //增加模组箱体号到外观记录中
                        appearance.ModuleGroupNum = (from m in db.BarCodes where m.BarCodesNum == appearance.BarCodesNum select m).FirstOrDefault().ModuleGroupNum;
                        appearance.OldOrderNum = appearance.OrderNum;
                        appearance.OldBarCodesNum = appearance.BarCodesNum;
                        //原判断库存订单
                        //string toordernum = db.BarCodes.FirstOrDefault(c => c.BarCodesNum == appearance.BarCodesNum).ToOrderNum;
                        //if (appearance.ToOrderNum != toordernum)
                        //{
                        //    ModelState.AddModelError("", "该模组条码已经绑定给订单" + toordernum + "了，请选择正确的新订单号！");
                        //    return View(appearance);
                        //}
                        db.Appearance.Add(appearance);
                        db.SaveChanges();
                        //添加关联表
                        if (!string.IsNullOrEmpty(nuoBarCode) && isnuo == "true")
                        {
                            BarCodeRelation barcoderelation = new BarCodeRelation() { OldOrderNum = nuoOrder, OldBarCodeNum = nuoBarCode, NewBarCodesNum = appearance.BarCodesNum, NewOrderNum = appearance.OrderNum, Procedure = "Appearances", UsserID = ((Users)Session["User"]).UserNum, CreateDate = DateTime.Now };
                            if (!comm.InsertRelation(barcoderelation))
                            {
                                ModelState.AddModelError("", "挪用订单失败，此条码已挪用过库存条码");
                                return View(appearance);
                            }
                            #region 挪用修改原条码工序记录
                            common.NuoOperation(nuoBarCode, nuoOrder, appearance.BarCodesNum, appearance.OrderNum);
                            #endregion
                        }
                        return RedirectToAction("Appearance_F", new { appearance.Id });
                    }
                    else
                    {
                        ModelState.AddModelError("", "该模组条码不属于所选订单，请选择正确的订单号！");
                        return View(appearance);
                    }
                }
                //没有完成外观包装记录，有1个以上正在做外观包装记录
                else if (normalCount == 0 && doing_appearance_Count >= 1)
                {
                    var appearance_doing_record = appearanceRecord.FirstOrDefault(m => m.OQCCheckBT != null && m.OQCCheckFT == null);//读出第一个正在做外观包装的记录
                    return RedirectToAction("Appearance_F", new { appearance_doing_record.Id }); //在“完成外观包装”页面打开正在做外观包装的记录，完成关闭外观包装操作
                }
                //有完成外观包装记录
                else if (normalCount >= 1 && doing_appearance_Count == 0)
                {
                    //如果是库存条码,转订单操作
                    //if (appearance.ToOrderNum != null && barcoderecord.IsRepertory == true)
                    //{
                    //    if (barcoderecord.ToOrderNum != null) //转新订单已经存在
                    //    {
                    //        ModelState.AddModelError("", "此模组条码已经绑定给订单" + barcoderecord.ToOrderNum + "了，请选择正确的库存订单号和新订单号！");
                    //        return View(appearance);
                    //    }
                    //    else//转新订单为空
                    //    {
                    //        barcoderecord.ToOrderNum = appearance.ToOrderNum;//添加新订单ToOrderNum
                    //        db.Entry(barcoderecord).State = EntityState.Modified;
                    //        db.Appearance.Add(appearance);// 是否重复存储？
                    //        await db.SaveChangesAsync();
                    //        return RedirectToAction("Appearance_F", new { appearance.Id });
                    //    }
                    //}
                    //else
                    //{
                    ModelState.AddModelError("", "此模组已经完成外观包装，不能对已通过外观包装的模组进行重复外观包装！");
                    return View(appearance);
                    // }
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



        #region --------------------外观电检包装开始时检查条码在老化调试工序是否已经完成
        /// <summary>
        /// 外观包装输入条码时，检查模组是否已经完成老化调试或校正工序
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Appearance_B_Check(string barcode)
        {
            if (barcode == "")
            {
                return Content("此模组条码号为空！");
            }
            var burn_in_Finish_Record = db.Burn_in.Where(c => c.BarCodesNum == barcode && c.OQCCheckFinish == true && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();
            if (burn_in_Finish_Record > 0)
            {
                return Content("");
            }
            else
            {
                return Content("此模组老化调试工序尝未完成！");
            }
        }

        #endregion

        #region --------------------外观电检包装完成------------
        // GET: Appearances/Appearance_F
        public ActionResult Appearance_F(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Appearances", act = "Appearance_F" + "/" + id.ToString() });
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
            //添加库存条码信息
            var nuobarcode = db.BarCodeRelation.Where(c => c.NewBarCodesNum == appearance.BarCodesNum && c.NewOrderNum == appearance.OrderNum).Select(c => c.OldBarCodeNum).FirstOrDefault();
            if (nuobarcode == null)
                ViewBag.nuoBarcode = "";
            else
                ViewBag.nuoBarcode = nuobarcode;

            ViewBag.RepairList = SetRepairList();
            return View(appearance);
        }

        // POST: Appearances/Appearance_F
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Appearance_F([Bind(Include = "Id,OrderNum,ToOrderNum,BarCodesNum,CustomerBarCodesNum,ModuleGroupNum,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckDate,OQCCheckTime,OQCCheckTimeSpan,Appearance_OQCCheckAbnormal,RepairCondition,OQCCheckFinish,Remark,Department1,Group")] Appearance appearance)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Appearances", act = "Appearance_F" + "/" + appearance.Id.ToString() });
            }
            if (appearance.OQCCheckFT == null)
            {
                appearance.OQCCheckFT = DateTime.Now;
                appearance.OQCPrincipal = ((Users)Session["User"]).UserName;
                appearance.OldBarCodesNum = appearance.BarCodesNum;
                appearance.OldOrderNum = appearance.OrderNum;
                var BT = appearance.OQCCheckBT.Value;
                var FT = appearance.OQCCheckFT.Value;
                var CT = FT - BT;
                appearance.OQCCheckTime = CT;
                appearance.OQCCheckTimeSpan = CT.Days.ToString() + "天" + CT.Hours.ToString() + "小时" + CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
                if (CT.Days > 0)
                {
                    appearance.OQCCheckDate = CT.Days;
                    appearance.OQCCheckTime = new TimeSpan(CT.Hours, CT.Minutes, CT.Seconds);
                }
                if (appearance.Appearance_OQCCheckAbnormal == null)
                {
                    appearance.Appearance_OQCCheckAbnormal = "正常";
                }
                if (appearance.Appearance_OQCCheckAbnormal == "正常" && appearance.RepairCondition == "正常")
                {
                    appearance.OQCCheckFinish = true;
                }
            }
            var modulelist = db.Appearance.Where(c => c.OrderNum == appearance.OrderNum&&c.OQCCheckFinish==true&& !string.IsNullOrEmpty(c.ModuleGroupNum) && c.BarCodesNum != appearance.BarCodesNum&&c.OldOrderNum==appearance.OrderNum).Select(c => c.ModuleGroupNum).ToList();
            if (modulelist.Contains(appearance.ModuleGroupNum))//判断输入的模组号是否重复
            {
                var barcodeitem = db.Appearance.Where(c => c.ModuleGroupNum == appearance.ModuleGroupNum && c.OrderNum == appearance.OrderNum).Select(c => c.BarCodesNum).FirstOrDefault();
                ModelState.AddModelError("", "模组号与条码" + barcodeitem + "重复");
                ViewBag.OrderList = GetOrderList();
                ViewBag.RepairList = SetRepairList();
                return View(appearance);
            }
            if (ModelState.IsValid)
            {
                var modify_ModuleGroupNumber = await ModifyModuleFromAppearance_F(appearance.BarCodesNum, appearance.ModuleGroupNum);
                db.Entry(appearance).State = EntityState.Modified;
                await db.SaveChangesAsync();
                common.DelteMudole(appearance.OrderNum, appearance.ModuleGroupNum);
                return RedirectToAction("Appearance_B");
            }
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            return View(appearance);
        }


        //添加模组号
        public ActionResult AddModuleFromAppearance_F(string barcodes, string mudule)
        {
            var barcode = db.BarCodes.Where(C => C.BarCodesNum == barcodes).FirstOrDefault();
            var cali = db.CalibrationRecord.Where(c => c.BarCodesNum == barcodes).FirstOrDefault();
            if (cali != null)
            {
                cali.ModuleGroupNum = mudule;
                db.SaveChanges();
            }
            if (barcode.ModuleGroupNum == null)
            {
                barcode.ModuleGroupNum = mudule;
                db.SaveChanges();
                return Content("true");
            }
            return Content("false");
        }

        public async Task<ActionResult> ModifyModuleFromAppearance_F(string barcodes, string mudule)
        {
            var barcode = db.BarCodes.Where(C => C.BarCodesNum == barcodes).FirstOrDefault();
            barcode.ModuleGroupNum = mudule;
            var result = await db.SaveChangesAsync();
            var cali_list = await db.CalibrationRecord.Where(c => c.BarCodesNum == barcodes).ToListAsync();
            if (cali_list != null)
            {
                foreach (var cali in cali_list)
                {
                    cali.ModuleGroupNum = mudule;
                }
            }
            result = result + db.SaveChanges();
            if (result > 0) return Content("true");
            else return Content("false");
        }

        #endregion


        #region    --------------------查询订单已完成、未完成、未开始条码
        [HttpPost]
        public ActionResult AppearanceChecklist(string orderNum)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            List<Appearance> AllAppearance = new List<Appearance>();//订单全部包装记录
            List<string> NotDoList = new List<string>();//未开始做条码清单
            List<string> NeverFinish = new List<string>();//未完成条码清单
            List<string> FinishList = new List<string>();//已完成条码清单
            JObject stationResult = new JObject();//输出结果JObject
            if (!String.IsNullOrEmpty(orderNum))
            {
                //调出订单对应全部记录      
                AllAppearance = db.Appearance.Where(c => c.OrderNum == orderNum && (c.OldOrderNum == null || c.OldOrderNum == orderNum)).OrderBy(c => c.BarCodesNum).ToList();
            }
            //调出订单所有条码清单
            List<string> barcodelist = db.BarCodes.Where(c => c.OrderNum == orderNum && c.BarCodeType == "模组").OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();
            List<string> recordlist = new List<string>();
            if (AllAppearance == null)
            {
                stationResult.Add("NotDoList", JsonConvert.SerializeObject(barcodelist));
                stationResult.Add("NeverFinish", JsonConvert.SerializeObject(NeverFinish));
                stationResult.Add("FinishList", JsonConvert.SerializeObject(FinishList));
            }
            else
            {
                recordlist = AllAppearance.Select(c => c.BarCodesNum).Distinct().ToList();
                //未开始做条码清单
                NotDoList = barcodelist.Except(recordlist).ToList();
                //已完成条码清单
                FinishList = AllAppearance.Where(c => c.OrderNum == orderNum && c.OQCCheckFinish == true).Select(c => c.BarCodesNum).Distinct().ToList();
                //未完成条码清单
                NeverFinish = AllAppearance.Where(c => c.OrderNum == orderNum && c.OQCCheckFinish == false).Select(c => c.BarCodesNum).Distinct().ToList().Except(FinishList).ToList();
                stationResult.Add("NotDoList", JsonConvert.SerializeObject(NotDoList));
                stationResult.Add("NeverFinish", JsonConvert.SerializeObject(NeverFinish));
                stationResult.Add("FinishList", JsonConvert.SerializeObject(FinishList));
            }
            return Content(JsonConvert.SerializeObject(stationResult));
        }
        #endregion


        #region --------------------GetOrderList()取出整个OrderMgms的OrderNum订单号列表
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

        /// <summary>
        /// 用来做权限设置的,部分地方还有用到这个方法
        /// </summary>
        /// <param name="rolename"></param>
        /// <param name="discription"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isCheckRole(string rolename, string discription, string name, int id)
        {
            var code = db.UserRolelistTable.Where(c => c.RolesName == rolename && c.Discription == discription).Select(c => c.RolesCode).FirstOrDefault().ToString();
            var codeList = db.Useroles.Where(c => c.UserName == name && c.UserID == id).Select(c => c.Roles).FirstOrDefault();
            return codeList.Contains(code);
        }

        /// <summary>
        /// 用户电检时检查模组号
        /// </summary>
        /// 查找条码和校正表的模组号.如果两个都不为空,判断两个的模组号是否相同,形同,返回YES,并返回模组号.不同返回NO,并返回模组号.如果两个模组号都为空,返回YES,提示没有模组号.如果模组号只有其中一个有,返回YES,返回模组号
        /// <param name="barcode"></param>
        /// <returns></returns>
        public ActionResult DisplayBarcode(string barcode)
        {
            var calBarcode = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && c.Normal == true && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).OrderByDescending(C => C.FinishCalibration).Select(c => c.ModuleGroupNum).FirstOrDefault();//根据条码查找校正表的模组号

            var barscode = db.BarCodes.Where(c => c.BarCodesNum == barcode).Select(c => c.ModuleGroupNum).FirstOrDefault();//根据条码查找条码表的模组号
            if (calBarcode != null && barcode != null)//如果两个模组号都不为空
            {
                if (calBarcode == barscode)
                    return Content("YES,此条码校正模组号为:" + calBarcode + "，条码表模组号为:" + barscode);
                else
                    return Content("NO,此条码校正模组号为:" + calBarcode + "，条码表模组号为:" + barscode);
            }
            else if (calBarcode == null && barscode == null)//如果两个都为空
                return Content("YES,此条码没有模组号");
            else
                return Content((string.IsNullOrEmpty(calBarcode) ? "YES, 此模组条码未完成校正" : "YES,此条码校正模组号为:" + calBarcode) + "，条码表模组号为:" + barscode);




        }


        #region 内箱标签打印
        /// <summary>
        /// 后台绘图后打印方法
        /// </summary>
        /// <param name="pagecount"></param>
        /// <param name="barcode"></param>
        /// <param name="modulenum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsideBoxLablePrint(int pagecount, string barcode = "", string modulenum = "", string ip = "", int port = 0, int concentration = 5, bool testswitch = false)
        {
            //开始绘制图片
            int initialWidth = 600, initialHeight = 250;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
            //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));

            //引入模组号
            StringFormat geshi = new StringFormat();
            geshi.Alignment = StringAlignment.Center; //居中

            if (modulenum.Length > 6)
            {
                System.Drawing.Font myFont_modulenum;
                myFont_modulenum = new System.Drawing.Font("Microsoft YaHei UI", 60, FontStyle.Bold);//OCR-B//宋体
                theGraphics.DrawString(modulenum, myFont_modulenum, bush, 265, 20, geshi);
            }
            else
            {
                System.Drawing.Font myFont_modulenum;
                myFont_modulenum = new System.Drawing.Font("Microsoft YaHei UI", 80, FontStyle.Bold);//OCR-B//宋体
                theGraphics.DrawString(modulenum, myFont_modulenum, bush, 265, 10, geshi);
            }

            //引入条码
            Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(barcode, 380, 30);
            //double beishuhege = 0.99;
            theGraphics.DrawImage(bmp_barcode, 80, 150, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

            //引入条码号
            System.Drawing.Font myFont_modulebarcodenum;
            myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 13, FontStyle.Regular);
            StringFormat geshi1 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            theGraphics.DrawString(barcode, myFont_modulebarcodenum, bush, 270, 180, geshi);
            //结束图片绘制以上都是绘制图片的代码

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


        //截图或指令直接打印条码方法
        [HttpPost]
        public ActionResult InsideBoxLablePrint2(string bitmap = "", int pagecount = 1, string barcode = "", string modulenum = "", int concentration = 5)
        {
            string ptrn = "InsideBoxLablePrinter";
            if (!String.IsNullOrEmpty(barcode) && !String.IsNullOrEmpty(modulenum))
            {
                string data_s = "^XA";
                string modulenum_data = "^MD" + concentration + "^A@N,80,56,B:ST.FNT^BCN,0,Y,N,N^FO250,30BY20^FD" + modulenum + "^FS";//^MD5
                string barcode_data = "^FO200,115BY50^BCN,80,Y,N,N^A@N,30,21,B:ST.FNT^FD" + barcode + "^FS";
                string data_e = "^XZ";
                bool result = BarCodeLablePrint.SendStringToPrinter(ptrn, data_s + modulenum_data + barcode_data + data_e);
                if (result) return Content("打印成功");
                else return Content("打印失败");
            }
            else
            {
                Bitmap theBitmap = ZebraUnity.Base64ToBitmap(bitmap);//把base64字符串转换成Bitmap
                string sb = "^XA~DGR:ZONE.GRF,";
                Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));
                MemoryStream ms = new MemoryStream();
                theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                sb += totalbytes + "," + rowbytes + "," + hex;
                sb += "^LH0,0^FO170,10^XGR:ZONE.GRF^FS^XZ";
                theBitmap.Dispose();
                if (pagecount == 1)
                {
                    if (BarCodeLablePrint.SendStringToPrinter(ptrn, sb.ToString())) return Content("打印成功");
                    //string result = BarCodeLablePrint.IPPrint();
                }
                else if (pagecount == 2)
                {
                    bool resut = BarCodeLablePrint.SendStringToPrinter(ptrn, sb.ToString());
                    resut = BarCodeLablePrint.SendStringToPrinter(ptrn, sb.ToString());
                    if (resut) return Content("打印成功");
                    else return Content("打印失败");
                }
                return Content("打印失败");
            }
        }
        #endregion

        #region  模组号规则OQC确认删除
        /// <summary>
        /// 查看模组号规则的删除情况
        /// </summary>
        /// 找到delete.json,读取delete.json的数据,循环数据,找到本地有没有相同的文件名的json 文件,没有就直接移除delete.json的订单号,进行下一个循环,有的话就提示,入库,外箱,内箱确认,电检有多少条记录记录,并返回
        /// <returns></returns>
        public ActionResult DisplayOrdernum()
        {
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\delete.json") == true)//判断有没有delete.json,没有就返回null
            {
                var deletejson = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json");
                var jarray = JsonConvert.DeserializeObject<JArray>(deletejson).ToList();//读取数据
                JArray messagejson = new JArray();
                foreach (var item in jarray)//循环数据
                {
                    var ordernum = item.ToString();
                    if (!System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordernum + ".json"))//查看删除列表里有没有对应的json文件,如果没有,直接移除delete.json文件里面的ordernum,进行下一个循环
                    {
                        var index = jarray.FindIndex(c => c.ToString() == ordernum);
                        jarray.Remove(jarray[index]);
                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jarray, Newtonsoft.Json.Formatting.Indented);
                        System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json", output);
                        continue;
                    }
                    JObject messageitem = new JObject();
                    string mesage = "";
                    //入库记录
                    var warehouecount = common.GetCurrentwarehousList(ordernum).Count();
                    if (warehouecount > 0)
                    { mesage = mesage + "入库有" + warehouecount + "条记录，"; }

                    //外箱标签
                    var pritxount = db.Packing_BarCodePrinting.Count(c => c.OrderNum == ordernum && c.QC_Operator == null);
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
                    messageitem.Add("ordernum", ordernum);
                    messageitem.Add("message", mesage);
                    messagejson.Add(messageitem);
                }
                return Content(JsonConvert.SerializeObject(messagejson));
            }
            return null;
        }

        /// <summary>
        /// 确认删除
        /// </summary>
        /// 先判断有没有校正记录,有校正记录不能删除,判断有没有出库信息,有出库信息不能删,没有的话,先将BarCodes里面的模组号改为null,删除掉对应的json文件和包含在delete.json里的文件名,再分别删除入库,外箱,内箱,电检记录,记录日志
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
        public async Task<ActionResult> DeleteMoudleRuleAsync(string ordernum)
        {
            JObject message = new JObject();
            var cab = db.CalibrationRecord.Where(c => c.OrderNum == ordernum && c.ModuleGroupNum != null && (c.OldOrderNum == null || c.OldOrderNum == ordernum)).ToList();//查看是否有校正记录
            if (cab.Count != 0)
            {
                message.Add("message", "订单已有校正记录，不能删除");
                message.Add("result", false);
                return Content(JsonConvert.SerializeObject(message));
            }
            var current = common.GetCurrentwarehousList(ordernum);
            var warehoueout = current.Count(c => c.IsOut == true);
            if (warehoueout > 0)
            {
                message.Add("message", "订单已有出库信息，不能删除");
                message.Add("result", false);
                return Content(JsonConvert.SerializeObject(message));
            }
            var bac = db.BarCodes.Where(c => c.OrderNum == ordernum).ToList();//修改BarCodes表里的模组号,修改为null
            bac.ForEach(c => c.ModuleGroupNum = null);

            System.IO.File.Delete(@"D:\MES_Data\TemDate\OrderSequence\" + ordernum + ".json");//删除对应的json文件

            var deletejson = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json");//删除delete.json文件里面含有的文件名
            var jarray = JsonConvert.DeserializeObject<JArray>(deletejson).ToList();
            var index = jarray.FindIndex(c => c.ToString() == ordernum);
            jarray.Remove(jarray[index]);
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jarray, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json", output);//保存删除后的delete.json文件

            int count = 0;
            //入库记录
            var warehouecount = current.Count();
            if (warehouecount > 0)
            {
                db.Warehouse_Join.RemoveRange(current);
            }

            //外箱标签
            var pritxount = db.Packing_BarCodePrinting.Count(c => c.OrderNum == ordernum && c.QC_Operator == null);
            if (pritxount > 0)
            {
                var print = db.Packing_BarCodePrinting.Where(c => c.OrderNum == ordernum && c.QC_Operator == null);
                db.Packing_BarCodePrinting.RemoveRange(print);
            }

            //内箱记录
            var innercount = db.Packing_InnerCheck.Count(c => c.OrderNum == ordernum);
            if (innercount > 0)
            {
                var inner = db.Packing_InnerCheck.Where(c => c.OrderNum == ordernum);
                db.Packing_InnerCheck.RemoveRange(inner);
            }

            //电检记录
            var appercount = db.Appearance.Count(c => c.OrderNum == ordernum && c.OQCCheckFT != null);
            if (appercount > 0)
            {
                var appper = db.Appearance.Where(c => c.OrderNum == ordernum && c.OQCCheckFT != null);
                var barcode = db.BarCodes.Where(c => c.OrderNum == ordernum);
                await barcode.ForEachAsync(c => c.ModuleGroupNum = null);
                db.Appearance.RemoveRange(appper);

            }
            count += await db.SaveChangesAsync();

            if (count != 0 || (warehoueout == 0 && pritxount == 0 && innercount == 0 && appercount == 0))//有删除记录则填写日志
            {
                //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "订单模组号规则删除：订单号为" + ordernum };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                message.Add("message", "");
                message.Add("result", true);
                return Content(JsonConvert.SerializeObject(message));
            }
            else
            {
                message.Add("message", "操作数据失败");
                message.Add("result", false);
                return Content(JsonConvert.SerializeObject(message));
            }
        }

        /// <summary>
        /// 取消删除
        /// </summary>
        /// 找到delete.json,删除前端传过来的订单号
        /// <param name="ordernum">订单</param>
        public void CancelDelete(string ordernum)
        {
            var deletejson = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json");
            var jarray = JsonConvert.DeserializeObject<JArray>(deletejson).ToList();//读取文件
            var index = jarray.FindIndex(c => c.ToString() == ordernum);
            jarray.Remove(jarray[index]);//删除订单号
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jarray, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\delete.json", output);//保存文件
        }
        #endregion

        #region 电检完成后发现模组号为空，修补方法
        /// <summary>
        /// 根据条码找到已完成电检,并且模组号为空的信息,如果信息为空,提示"此条码不符合修改条件",否则,修改此信息的模组号号,并且修改条码表的模组号,
        /// </summary>
        /// <param name="modulenum">模组号</param>
        /// <param name="barcode">条码</param>
        /// <returns></returns>
        public ActionResult UpdateMuduleFromCompleteAppearances(string modulenum, string barcode)
        {
            JObject result = new JObject();
            var message = db.Appearance.Where(c => c.BarCodesNum == barcode && c.OQCCheckFinish == true && c.ModuleGroupNum == null).FirstOrDefault();//根据条码找到已完成电检,并且模组号为空的信息
            if (message != null)//判断信息是否为空,为空提示错误
            {
                common.UpdateTotalModule(barcode, modulenum, ((Users)Session["User"]).UserName);
                //var modulelist = db.Appearance.Where(c => c.OrderNum == message.OrderNum&&!string.IsNullOrEmpty(c.ModuleGroupNum)&&c.BarCodesNum!=barcode && c.OldOrderNum == message.OrderNum).Select(c => c.ModuleGroupNum).ToList();
                //if (modulelist.Contains(modulenum))//判断输入的模组号是否重复
                //{
                //    var barcodeitem = db.Appearance.Where(c => c.ModuleGroupNum == modulenum && c.OrderNum == message.OrderNum).Select(c => c.BarCodesNum).FirstOrDefault();
                //    result.Add("message", "模组号与条码" + barcodeitem + "重复");
                //    result.Add("result", false);
                //    return Content(JsonConvert.SerializeObject(result));
                //}
                //var barcodemeaage = db.BarCodes.Where(c => c.BarCodesNum == barcode).FirstOrDefault();//找到条码表信息
                //var calibrationRecord = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode).FirstOrDefault();//校正条码信息
                //if (calibrationRecord != null)
                //{
                //    calibrationRecord.ModuleGroupNum = modulenum;//修改校正表信息
                //    UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName, OperateRecord = "UpdateMuduleFromCompleteAppearances,修改校正表模组号,条码是" + barcode + "模组号是" + modulenum };
                //    db.UserOperateLog.Add(log);
                //}
                //message.ModuleGroupNum = modulenum;//修改电检表信息
                //barcodemeaage.ModuleGroupNum = modulenum;//修改条码信息
                //UserOperateLog log2 = new UserOperateLog() { OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName, OperateRecord = "UpdateMuduleFromCompleteAppearances,修改电检和条码表模组号,条码是" + barcode + "模组号是" + modulenum };
                //db.UserOperateLog.Add(log2);
                //db.Entry(barcodemeaage).State = EntityState.Modified;
                //db.Entry(message).State = EntityState.Modified;
                //db.SaveChanges();//保存数据
                result.Add("message", "");
                result.Add("result", true);
                return Content(JsonConvert.SerializeObject(result));
            }
            result.Add("message", "此条码不符合修改条件");
            result.Add("result", false);
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 三个表修改模组号
        /// <summary>
        /// 显示三表的现模组号信息
        /// </summary>
        /// 根据订单在条码标找到所有条码列表,在根据条码列表,找到条码表,校正表,电检表三个表的模组号,并显示出来
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisplayTotalModule(string ordernum,string type="模组")
        {
            JArray result = new JArray();
            var barcoldeList = db.BarCodes.Where(c => c.OrderNum == ordernum&&c.BarCodeType==type).OrderBy(c => c.BarCodesNum).Select(c => new { c.BarCodesNum, c.ModuleGroupNum,c.BarcodePrintCount,c.ModuleNumPrintCount }).ToList();//找到所以条码列表
            var tempcalibtation = db.CalibrationRecord.Where(c => c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum).Select(c => new TempCalibration() { barcode = c.BarCodesNum, module = c.ModuleGroupNum,id=c.ID,oldbarcode=c.OldBarCodesNum}).ToList();//校正信息
            var tempappearance = db.Appearance.Where(c => c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum).Select(c => new TempAppearabce() { barcode = c.BarCodesNum, module = c.ModuleGroupNum,id=c.Id , oldbarcode = c.OldBarCodesNum }).ToList();//电检信息
            foreach (var item in barcoldeList)
            {
                JObject value = new JObject();
                value.Add("barcode", item.BarCodesNum);//条码
                value.Add("barcodeModule", item.ModuleGroupNum);//条码模组
                var calibtationmodule = tempcalibtation.OrderByDescending(C=>C.id).Where(c => c.barcode == item.BarCodesNum&&(c.oldbarcode==null||c.oldbarcode==item.BarCodesNum)).Select(c => c.module).FirstOrDefault();
                value.Add("calibtationModule", calibtationmodule);//校正模组
                var appearanmodule = tempappearance.OrderByDescending(c=>c.id).Where(c => c.barcode == item.BarCodesNum&& (c.oldbarcode == null || c.oldbarcode == item.BarCodesNum)).Select(c => c.module).FirstOrDefault();
                value.Add("appearanModule", appearanmodule);//电检模组
                if (item.BarcodePrintCount == 0)
                {
                    value.Add("BarcodePrint", "条码未打印");//条码打印
                }
                else
                {
                    value.Add("BarcodePrint", "条码已打印"+ item.BarcodePrintCount + "次");//条码打印
                }
                if (item.ModuleNumPrintCount == 0)
                {
                    value.Add("ModuleNumPrint", "编号未打印");//编号打印
                }
                else
                {
                    value.Add("ModuleNumPrint", "编号已打印" + item.ModuleNumPrintCount + "次");//编号打印
                }
                result.Add(value);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        
        [HttpPost]
       
        /// <summary>
        /// 批量修改模组号
        /// </summary>
        /// 读取ListMessage 数据,并循环,拿到里面的单个条码号和模组号,根据条码号找到条码表,校正表,电检表的信息,修改模组号
        /// <param name="ListMessage">需要修改模组号的条码和模组号集合</param>
        public ActionResult BatchUpdatTotalModule(string ListMessage)
        {
            JArray value = JsonConvert.DeserializeObject<JArray>(ListMessage);
            JObject result = new JObject();
            var count = value.Count();
            var moduleList = new List<string>();
            moduleList= value.Select(c =>c["module"].ToString()).ToList();
            var temp=moduleList.GroupBy(c => c).Where(c => c.Count() > 1).ToList();
            var temp2 = temp.Where(c => !string.IsNullOrEmpty(c.Key)).ToList();
            if (temp2.Count!=0)
            {
                result.Add("mes", "模组号"+string.Join(",", temp2.Select(c=>c.Key)) +"重复");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));
            }
            for (int i = 0; i < count; i++)
            {
                var module = value[i]["module"].ToString();
                var barcode = value[i]["barcode"].ToString();//条码
                var bcode = db.BarCodes.Where(c => c.BarCodesNum == barcode).FirstOrDefault();
                var cab = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).ToList();//校正信息
                var appearanmodule = db.Appearance.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).ToList();//电检信息
                bcode.ModuleGroupNum = value[i]["module"].ToString();//模组
                if (cab.Count() != 0)//校正信息不为空
                {
                    cab.ForEach(c=>c.ModuleGroupNum = value[i]["module"].ToString()) ;//修改校正信息
                }
                if (appearanmodule.Count() != 0)//电检信息不为空
                {
                    appearanmodule.ForEach(c => c.ModuleGroupNum = value[i]["module"].ToString());//修改电检信息
                }
                //填写日志
                UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName, OperateRecord = "批量修改模组号,修改后条码为" + value[i]["module"].ToString() };
                db.UserOperateLog.Add(log);
                db.SaveChangesAsync();
            
            
        }
            result.Add("mes", "修改成功");
            result.Add("pass", true);
            return Content(JsonConvert.SerializeObject(result));
        }
    #endregion
}
}
