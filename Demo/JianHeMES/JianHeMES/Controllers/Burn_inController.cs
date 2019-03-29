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
using System.Web.Helpers;
using Newtonsoft.Json;

namespace JianHeMES.Controllers
{
    public class Burn_inController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region ---------------------------------------OQCNormal列表
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

        #region ---------------------------------------FinishStatus列表
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


        #region ---------------------------------------维修列表

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

        #region ---------------------------------------老化首页 Index
        // GET: Burn_in
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            ViewBag.Display = "display:none";//隐藏View基本情况信息
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.OQCNormal = OQCNormalList();
            ViewBag.FinishStatus = FinishStatusList();
            ViewBag.Overtime = "";
            ViewBag.Overtime2 = "";
            ViewBag.Finish = null;
            ViewBag.NotDo = null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string OrderNum, string BoxBarCode, string OQCNormal, string FinishStatus, string searchString/*, int PageIndex = 0*/)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            List<Burn_in> AllBurn_inRecords = new List<Burn_in>();
            List<Burn_in> AllBurn_inRecordsList = null;
            if (String.IsNullOrEmpty(OrderNum))
            {
                //调出全部记录      
                //AllBurn_inRecords = from m in db.Burn_in
                //                    select m;
                AllBurn_inRecords = db.Burn_in.ToList();
            }
            else
            {
                //筛选出对应orderNum所有记录
                AllBurn_inRecords = db.Burn_in.Where(c => c.OrderNum == OrderNum).ToList();
                if (AllBurn_inRecords.Count() == 0)
                {
                    var barcodelist = db.BarCodes.Where(c => c.ToOrderNum == OrderNum).ToList();
                    //..TODO..待优化
                    foreach (var item in barcodelist)
                    {
                        AllBurn_inRecords.AddRange(db.Burn_in.Where(c => c.BarCodesNum == item.BarCodesNum));
                    }
                }
            }

            //统计老化结果正常的模组数量
            var Order_CR_Normal_Count = AllBurn_inRecords.Where(x => x.RepairCondition == "正常").Count();
            var Abnormal_Count = AllBurn_inRecords.Where(x => x.RepairCondition != "正常" && x.RepairCondition != null).Count();

            #region---------按条码筛选--------------
            if (BoxBarCode != "")
            {
                AllBurn_inRecords = AllBurn_inRecords.Where(x => x.BarCodesNum == BoxBarCode).ToList();
            }
            #endregion

            #region---------筛选正常、异常-------------
            //正常、异常记录筛选
            if (OQCNormal == "异常")
            {
                AllBurn_inRecords = AllBurn_inRecords.Where(c => c.Burn_in_OQCCheckAbnormal !=null).ToList();
            }
            else if (OQCNormal == "正常")
            {
                AllBurn_inRecords = AllBurn_inRecords.Where(c => c.Burn_in_OQCCheckAbnormal == null).ToList();
            }
            #endregion

            #region---------筛选完成、未完成-------------
            //完成、未完成记录筛选
            if (FinishStatus == "完成")
            {
                AllBurn_inRecords = AllBurn_inRecords.Where(c => c.OQCCheckFT != null).ToList();
            }
            else if (FinishStatus == "未完成")
            {
                AllBurn_inRecords = AllBurn_inRecords.Where(c => c.OQCCheckFT == null).ToList();
            }
            #endregion

            #region---------异常描述条件筛选-------------

            //检查searchString是否为空
            if (!String.IsNullOrEmpty(searchString))
            {   //从调出的记录中筛选含searchString内容的记录
                AllBurn_inRecords = AllBurn_inRecords.Where(m => m.Burn_in_OQCCheckAbnormal!=null && m.Burn_in_OQCCheckAbnormal.Contains(searchString)).ToList();
            }
            #endregion

            #region----------筛选从未开始、正在老化调试OQC的条码清单------------
            //取出订单的全部条码
            List<BarCodes> BarCodesList = (from m in db.BarCodes where m.OrderNum == OrderNum select m).ToList();
            List<string> NotDoOQCList = new List<string>();
            List<string> DoingNowOQCList = new List<string>();
            foreach (var barcode in BarCodesList)
            {
                if ((from m in db.Burn_in where m.BarCodesNum == barcode.BarCodesNum select m).Count() == 0)
                {
                    NotDoOQCList.Add(barcode.BarCodesNum);
                }
            }
            ViewBag.NotDo = NotDoOQCList;//输出未开始做OQC的条码清单
            int barcodeslistcount = NotDoOQCList.Count;
            ViewBag.NotDoCount = barcodeslistcount;//未开始数量

            foreach (var barcode in BarCodesList)
            {
                if (db.Burn_in.Where(m=>m.BarCodesNum==barcode.BarCodesNum).Where(m=>m.OQCCheckBT!=null && m.OQCCheckFT==null).Count() > 0)
                {
                    DoingNowOQCList.Add(barcode.BarCodesNum);
                }
            }
            ViewBag.DoingNowOQCList = DoingNowOQCList;//正在做老化调试OQC的条码清单
            ViewBag.DoingNowOQCListCount = DoingNowOQCList.Count(); //正在做老化调试OQC的条码清单个数
            #endregion

            #region----------计算总时长和平均时长------------

            //取出对应orderNum校正时长所有记录
            var recordlist = db.Burn_in.Where(m => m.OrderNum == OrderNum).ToList();
            //计算老化总时长
            TimeSpan TotalTimeSpan = new TimeSpan();
            if (AllBurn_inRecords.Where(x => x.RepairCondition == "正常").Count() != 0)    //Burn_in_OQCCheckAbnormal的值是1为正常
            {
                foreach (var m in recordlist)
                {
                    if (m.OQCCheckTime!= null)
                    {
                        TotalTimeSpan = TotalTimeSpan.Add(m.OQCCheckTime.Value);
                    }
                }
                var days = recordlist.Sum(c => c.OQCCheckDate);
                if(days>0)
                {
                    TotalTimeSpan = new TimeSpan(TotalTimeSpan.Days + days, TotalTimeSpan.Hours, TotalTimeSpan.Minutes, TotalTimeSpan.Seconds);
                }

                if (TotalTimeSpan.Days > 0)
                {
                    ViewBag.TotalTimeSpan = TotalTimeSpan.Days.ToString() + "天" + TotalTimeSpan.Hours.ToString() + "小时" + TotalTimeSpan.Minutes.ToString() + "分" + TotalTimeSpan.Seconds.ToString() + "秒";
                }
                else if (TotalTimeSpan.Days == 0 && TotalTimeSpan.Hours > 0)
                {
                    ViewBag.TotalTimeSpan = TotalTimeSpan.Hours.ToString() + "小时" + TotalTimeSpan.Minutes.ToString() + "分" + TotalTimeSpan.Seconds.ToString() + "秒";
                }
                else if (TotalTimeSpan.Days == 0 && TotalTimeSpan.Hours == 0 && TotalTimeSpan.Minutes > 0)
                {
                    ViewBag.TotalTimeSpan = TotalTimeSpan.Minutes.ToString() + "分" + TotalTimeSpan.Seconds.ToString() + "秒";
                }
                else if (TotalTimeSpan.Days == 0 && TotalTimeSpan.Hours == 0 && TotalTimeSpan.Minutes == 0 && TotalTimeSpan.Seconds > 0)
                {
                    ViewBag.TotalTimeSpan = TotalTimeSpan.Seconds.ToString() + "秒";
                }
                else
                {
                    ViewBag.AvgTimeSpan = "";
                }
            }
            else
            {
                if (recordlist.Count()>0)
                {
                    ViewBag.TotalTimeSpan = "";
                }
                else
                {
                    ViewBag.TotalTimeSpan = "暂时没有已完成老化的模组";
                }
            }

            //计算平均用时
            int Order_CR_valid_Count = AllBurn_inRecords.Where(x => x.OQCCheckTime != null).Count();
            //int TotalTimeSpanSecond = Convert.ToInt32(TotalTimeSpan.Hours.ToString()) * 3600 + Convert.ToInt32(TotalTimeSpan.Minutes.ToString()) * 60 + Convert.ToInt32(TotalTimeSpan.Seconds.ToString());
            int TotalTimeSpanSecond = Convert.ToInt32(TotalTimeSpan.TotalSeconds);
            int AvgTimeSpanInSecond = 0;
            if (Order_CR_valid_Count != 0)
            {
                AvgTimeSpanInSecond = TotalTimeSpanSecond / Order_CR_valid_Count;
                int tem = 0;
                int AvgTimeSpanHour = AvgTimeSpanInSecond / 3600;
                tem = AvgTimeSpanInSecond % 3600;
                int AvgTimeSpanMinute = tem / 60;
                int AvgTimeSpanSecond = tem % 60;
                if (AvgTimeSpanHour > 0)
                {
                    ViewBag.AvgTimeSpan = AvgTimeSpanHour + "时" + AvgTimeSpanMinute + "分" + AvgTimeSpanSecond + "秒";//向View传递计算平均用时
                }
                else if (AvgTimeSpanHour == 0 && AvgTimeSpanMinute > 0)
                {
                    ViewBag.AvgTimeSpan = AvgTimeSpanMinute + "分" + AvgTimeSpanSecond + "秒";//向View传递计算平均用时
                }
                else if (AvgTimeSpanHour == 0 && AvgTimeSpanMinute == 0 && AvgTimeSpanSecond > 0)
                {
                    ViewBag.AvgTimeSpan = AvgTimeSpanSecond + "秒";//向View传递计算平均用时
                }
                else
                {
                    ViewBag.AvgTimeSpan = "";
                }
            }
            else
            {
                if (recordlist.Count() > 0)
                {
                    ViewBag.AvgTimeSpan = "";
                }
                else
                {
                    ViewBag.AvgTimeSpan = "暂时没有已完成老化的模组";  //向View传递计算平均用时
                }
            }
            #endregion

            //列出记录
            AllBurn_inRecordsList = AllBurn_inRecords.OrderBy(c=>c.BarCodesNum).ToList();

            #region
            //查出完成老化工序后，超过三天还没有进入包装工序的条码记录清单
            //[{"18AA99999A0002","超多久没有进入包装"},{"18AA99999A0002","超多久没有进入包装"}]
            //JObject OvertimeNeverAppearance = new JObject();
            //var AllBurn_inRecords_BarcodesNum_List = AllBurn_inRecordsList.Select(c => c.BarCodesNum).ToList();
            //var AppearanceRecordByOrderNum = db.Appearance.Where(c => c.OrderNum == OrderNum).ToList();
            //foreach(var item in AllBurn_inRecords_BarcodesNum_List)
            //{
            //    int Appearance_Record_Count_by_BarcodesNum = AppearanceRecordByOrderNum.Count(c => c.BarCodesNum == item);//此模组（item）的包装记录数
            //    if(Appearance_Record_Count_by_BarcodesNum==0)  //包装无记录时进入if
            //    {
            //        var Burn_in_MaxFT = AllBurn_inRecordsList.Where(c => c.BarCodesNum == item).Max(c => c.OQCCheckFT);  //取出此模组（item）老化的最大完成时间
            //        var SubTime = DateTime.Now - Burn_in_MaxFT; //求（现在-此模组（item）老化的最大完成时间）的值SubTime
            //        if (Burn_in_MaxFT !=null && SubTime.Value.TotalMinutes > 4320)  //如果SubTime大于4320分钟（3天:3*24*60）
            //        {
            //            OvertimeNeverAppearance.Add(item, "此模组"+Burn_in_MaxFT.ToString()+"完成老化后，已经超过" +  SubTime.Value.Days+ "天"+ SubTime.Value.Hours + "小时"+ SubTime.Value.Minutes + "分还没有进入包装工序！");
            //        }
            //    }
            //}
            //ViewBag.Overtime = OvertimeNeverAppearance;

            //1.查老化的记录（类型是list < burn_in >）
            //2.取出条码清单（list<string>）
            //3.创建一个JObject实体，用于记录超时信息（类型是JObject，格式是[{“以条码号为键”,”超时信息为值”},{“”,””}]）例如：[{“18AA99999A00002”,”此模组在2019-1-4 10:00:00完成老化，已经超过4天5小时未进入包装工序！”}，]
            //4.调出订单在包装的全部记录（List<appearance>）
            //5.在包装的全部记录中，通过foreach条码清单（list<string>），统计包装中有多少条记录
            //6.如果记录数==0，则进入计算时间差程序，并判定时间是否>3天，如果大于3天，则进入记录超时程序
            //7.输出ViewBag.Overtime给前端
            #endregion

            #region----------筛选出完成老化，超过72小时未进入包装工序的记录JSON------------
            var BarcodesNumListByOrderNum = AllBurn_inRecords.Select(c => c.BarCodesNum).ToList();//1和2
            JObject BorcodesNumOvertimeList = new JObject();//3
            var AppearanceRecordByOrderNum = db.Appearance.Where(c => c.OrderNum == OrderNum).ToList();//4
            foreach(var item in BarcodesNumListByOrderNum)
            {
                int AppearanceRecord_count = AppearanceRecordByOrderNum.Count(c => c.BarCodesNum == item);
                if(AppearanceRecord_count==0)
                {
                    var Burn_in_MaxFT = AllBurn_inRecords.Where(c => c.BarCodesNum == item).FirstOrDefault().OQCCheckFT;
                    var SubTime = DateTime.Now - Burn_in_MaxFT;
                    if (Burn_in_MaxFT != null && SubTime.Value.TotalMinutes > 4320) //3天：3*24*60
                    {
                        BorcodesNumOvertimeList.Add(item,"此模组在" + Burn_in_MaxFT.ToString() +"完成老化,已经超过" + SubTime.Value.Days +"天" + SubTime.Value.Hours + "小时" + SubTime.Value.Minutes + "分未进入包装工序！");
                    }
                }
            }
            ViewBag.Overtime = BorcodesNumOvertimeList;
            #endregion

            //ArrayList re2 = new ArrayList();
            //foreach (var item in BorcodesNumOvertimeList)
            //{
            //    string[] dd = new string[2];
            //    dd[0] = item.Key;
            //    dd[1] = item.Value.ToString();
            //    re2.Add(dd);
            //}
            //ViewBag.Overtime2 = re2.ToString();

            //计算已经完成OQC的数量
            var Finish_Count = AllBurn_inRecords.Count(c => c.OQCCheckFinish == true);

            //读出订单中模组总数量
            var Order_MG_Quantity = (from m in db.OrderMgm
                                     where (m.OrderNum == OrderNum)
                                     select m.Boxes).FirstOrDefault();
            //将模组总数量、正常的模组数量、未完成老化模组数量、订单号信息传递到View页面
            ViewBag.Quantity = Order_MG_Quantity;
            ViewBag.NormalCount = Order_CR_Normal_Count;
            ViewBag.AbnormalCount = Abnormal_Count;
            ViewBag.RecordCount = AllBurn_inRecords.Count();
            ViewBag.Finish = AllBurn_inRecordsList.Count(c => c.OQCCheckFinish == true);
            ViewBag.NeverFinish = Order_MG_Quantity - ViewBag.Finish;
            ViewBag.orderNum = OrderNum;

            //未选择订单时隐藏基本信息设置
            if (ViewBag.Quantity == 0)
            { ViewBag.Display = "display:none"; }
            else { ViewBag.Display = "display:normal"; }

            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.OQCNormal = OQCNormalList();
            ViewBag.FinishStatus = FinishStatusList();

            //分页计算功能
            //var recordCount = AllBurn_inRecords.Count();
            //var pageCount = GetPageCount(recordCount);
            //if (PageIndex >= pageCount && pageCount >= 1)
            //{
            //    PageIndex = pageCount - 1;
            //}
            //AllBurn_inRecords = AllBurn_inRecords.OrderBy(m => m.BarCodesNum)
            //                    .Skip(PageIndex * PAGE_SIZE)
            //                    .Take(PAGE_SIZE).ToList();
            //ViewBag.PageIndex = PageIndex;
            //ViewBag.PageCount = pageCount;
            ViewBag.OrderNumList = GetOrderNumList();

            return View(AllBurn_inRecordsList);
        }
        #endregion
        
        #region ---------------------------------------其他页面
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

        #region ---------------------------------------单个模组老化开始


        // GET: Burn_in/Burn_in_B
        public ActionResult Burn_in_B()
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "TEST" || ((Users)Session["User"]).Role == "老化OQC" || ((Users)Session["User"]).Role == "系统管理员")
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
            if (db.Burn_in.Count(u => u.BarCodesNum == burn_in.BarCodesNum) == 0)//没有记录
            {
                if (burn_in.OrderNum == db.BarCodes.Where(u => u.BarCodesNum == burn_in.BarCodesNum).FirstOrDefault().OrderNum)
                {
                    burn_in.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == burn_in.BarCodesNum).FirstOrDefault().OrderNum;
                    burn_in.OQCCheckBT = DateTime.Now;
                    burn_in.OQCPrincipal = ((Users)Session["User"]).UserName;
                    db.Burn_in.Add(burn_in);
                    await db.SaveChangesAsync();
                    //return RedirectToAction("Burn_in_B");
                    return Content("<script>alert('模组"+ burn_in.BarCodesNum + "已成功开始进入老化！');window.location.href='../Burn_in/Burn_in_B';</script>");
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
                            //return Content("<script>alert('此模组已经开始老化，不能重复开始老化！');window.location.href='../Burn_in/Burn_in_B';</script>");
                            ModelState.AddModelError("", "此模组已经开始老化，不能重复开始老化！");
                            return View(burn_in);

                        }
                    }

                    if (burn_in.OrderNum == db.BarCodes.Where(u => u.BarCodesNum == burn_in.BarCodesNum).FirstOrDefault().OrderNum)
                    {
                        burn_in.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == burn_in.BarCodesNum).FirstOrDefault().OrderNum;
                        burn_in.OQCCheckBT = DateTime.Now;
                        burn_in.OQCPrincipal = ((Users)Session["User"]).UserName;
                        db.Burn_in.Add(burn_in);
                        await db.SaveChangesAsync();
                        //return RedirectToAction("Burn_in_B", new { burn_in.Id });
                        return Content("<script>alert('模组" + burn_in.BarCodesNum + "已成功开始进入老化！');window.location.href='../Burn_in/Burn_in_B';</script>");
                    }
                    else
                    {
                        ModelState.AddModelError("", "该模组条码不属于所选订单，请选择正确的订单号！");
                        return View(burn_in);
                    }
                }
                else
                {
                    //return Content("<script>alert('此模组已经完成老化，不能重复老化！');window.location.href='../Burn_in';</script>");
                    ModelState.AddModelError("", "此模组已经完成老化，不能重复老化！");
                    return View(burn_in);
                }
            }
            else
            {
                return RedirectToAction("Burn_in_B", new { burn_in.Id });
            }
        }
        #endregion

        #region ---------------------------------------老化异常录入

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
        public async Task<ActionResult> AbnormalRecordInput(string Id, string Burn_in_OQCCheckAbnormal)
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

            ViewBag.RepairList = SetRepairList();

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
        #endregion

        #region ---------------------------------------单个模组老化完成
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
                var CT = burn_in.OQCCheckFT.Value - burn_in.OQCCheckBT.Value;
                if (CT.Days > 0)  //老化时长超过1天
                {
                    burn_in.OQCCheckDate = CT.Days;
                    burn_in.OQCCheckTime = new TimeSpan(CT.Hours, CT.Minutes, CT.Seconds);
                }
                else    //老化时长不超过1天
                {
                    burn_in.OQCCheckTime = CT;
                }
                burn_in.OQCPrincipal = ((Users)Session["User"]).UserName;
                burn_in.OQCCheckTimeSpan = CT.Days.ToString() + "天" + CT.Hours.ToString() + "时" + CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
                if (burn_in.Burn_in_OQCCheckAbnormal == null)
                {
                    burn_in.Burn_in_OQCCheckAbnormal = "正常";
                }
                if (burn_in.RepairCondition == null)
                {
                    burn_in.RepairCondition = "正常";
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
        
        #region ---------------------------------------批量模组老化开始
        public ActionResult Burn_in_Batch_B()
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "TEST" || ((Users)Session["User"]).Role == "老化OQC" || ((Users)Session["User"]).Role == "系统管理员")
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
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

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
            string result_putin = "";
            string result_never_putin = "";
            foreach (var item in BarCodesNumList)
            {
                var count = db.Burn_in.Count(c => c.BarCodesNum == item);
                if(count==0)
                {
                    newRecord.BarCodesNum = item;
                    newRecord.OQCCheckBT = DateTime.Now;
                    newRecord.OQCPrincipal = ((Users)Session["User"]).UserName;
                    db.Burn_in.Add(newRecord);
                    db.SaveChanges();
                    if (result_putin=="")
                    {
                        result_putin = item;
                    }else
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
            if(result_putin!="")
            {
                result_putin = result_putin + "\n已经开始老化。";
            }
            if(result_never_putin!="")
            {
                result_never_putin = result_never_putin + "\n已经在老化，没有进入重复开始老化。";
            }
            if(result_putin + result_never_putin!="")
            {
                return Content(result_putin + result_never_putin);
            }
            else
            {
                return Content("没有模组开始老化。");
            }
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
                        int finishtrue = burninrecords.Count(m=>m.OQCCheckFinish == true);
                        //有一个完成记录
                        if (finishtrue >= 1)
                        {
                            var record = (from m in burninrecords where m.OQCCheckFinish == true select m).ToList();
                            barcodebegintime.Add(Convert.ToString(record.FirstOrDefault().OQCCheckBT));//4.开始时间
                            barcodefinishtime.Add(Convert.ToString(record.FirstOrDefault().OQCCheckFT));//5.完成时间
                            barcodestatus.Add("经过" + burninrecords.Count() + "次老化，已经完成老化"); //6.老化状态
                            barcodecomfirm.Add("No");//7.可以进行老化
                        }
                        //没有完成记录
                        else /*if (finishtrue == 0)*/
                        {
                            //检查是否有正在进行老化的记录，有、无
                            if (burninrecords.Count(m =>m.OQCCheckBT!=null && m.OQCCheckFT == null) >= 1)
                            {
                                var record = burninrecords.Where(m => m.OQCCheckBT != null && m.OQCCheckFT == null).ToList().FirstOrDefault();
                                barcodebegintime.Add(Convert.ToString(record.OQCCheckBT));//4.开始时间
                                barcodefinishtime.Add("");//5.完成时间
                                barcodestatus.Add("第" + burninrecords.Count(m => m.OQCCheckBT != null && m.OQCCheckFT == null) + "次老化，正在老化"); //6.老化状态
                                barcodecomfirm.Add("No");//7.可以进行老化
                            }
                            else
                            {
                                barcodebegintime.Add("");//4.开始时间
                                barcodefinishtime.Add("");//5.完成时间
                                barcodestatus.Add("经过" + burninrecords.Count(m => m.OQCCheckBT != null && m.OQCCheckFT !=null) + "次老化，但未通过老化"); //6.老化状态
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

        #region ---------------------------------------批量模组老化完成

        // GET: Burn_in/Burn_in_F
        public ActionResult Burn_in_Batch_F()
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "TEST" || ((Users)Session["User"]).Role == "老化OQC" || ((Users)Session["User"]).Role == "系统管理员")
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
        public ActionResult Burn_in_Batch_F(string OrderNum, List<string> BarCodesNumList)
        {

            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
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
                burn_in.OQCCheckFT = DateTime.Now;
                var CT = burn_in.OQCCheckFT.Value - burn_in.OQCCheckBT.Value;
                if(CT.Days>0)  //老化时长超过1天
                {
                    burn_in.OQCCheckDate = CT.Days;
                    burn_in.OQCCheckTime = new TimeSpan(CT.Hours,CT.Minutes,CT.Seconds);
                }
                else    //老化时长不超过1天
                {
                    burn_in.OQCCheckTime = CT;
                }
                burn_in.OQCPrincipal = ((Users)Session["User"]).UserName;
                burn_in.OQCCheckTimeSpan = CT.Days.ToString() + "天" + CT.Hours.ToString() + "时" + CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
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
            
        #region    --------------------查询订单已完成、未完成、未开始条码
        [HttpPost]
        public ActionResult Burn_inChecklist(string orderNum)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            List<Burn_in> AllBurn_in = new List<Burn_in>();//订单全部包装记录
            List<string> NotDoList = new List<string>();//未开始做条码清单
            List<string> NeverFinish = new List<string>();//未完成条码清单
            List<string> FinishList = new List<string>();//已完成条码清单
            JObject stationResult = new JObject();//输出结果JObject
            if (!String.IsNullOrEmpty(orderNum))
            {
                //调出订单对应全部记录      
                AllBurn_in = db.Burn_in.Where(c => c.OrderNum == orderNum).OrderBy(c => c.BarCodesNum).ToList();
            }
            //调出订单所有条码清单
            List<string> barcodelist = db.BarCodes.Where(c => c.OrderNum == orderNum).OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();
            List<string> recordlist = new List<string>();
            if (AllBurn_in == null)
            {
                stationResult.Add("NotDoList", JsonConvert.SerializeObject(barcodelist));
                stationResult.Add("NeverFinish", JsonConvert.SerializeObject(NeverFinish));
                stationResult.Add("FinishList", JsonConvert.SerializeObject(FinishList));
            }
            else
            {
                recordlist = AllBurn_in.Select(c => c.BarCodesNum).Distinct().ToList();
                //未开始做条码清单
                NotDoList = barcodelist.Except(recordlist).ToList();
                //已完成条码清单
                FinishList = AllBurn_in.Where(c=>c.OQCCheckFinish == true).Select(c => c.BarCodesNum).Distinct().ToList();
                //未完成条码清单
                NeverFinish = AllBurn_in.Where(c=>c.OQCCheckFinish == false).Select(c => c.BarCodesNum).Distinct().ToList().Except(FinishList).ToList();
                stationResult.Add("NotDoList", JsonConvert.SerializeObject(NotDoList));
                stationResult.Add("NeverFinish", JsonConvert.SerializeObject(NeverFinish));
                stationResult.Add("FinishList", JsonConvert.SerializeObject(FinishList));
            }
            return Content(JsonConvert.SerializeObject(stationResult));
        }
        #endregion
        
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
        //----------------------------------------------------------------------------------------
        #endregion

        #region ---------------------------------------检索订单号
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

        #region ---------------------------------------分页
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

