﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JianHeMES.Controllers
{
    public class CalibrationRecordsController : Controller
    {


        private ApplicationDbContext db = new ApplicationDbContext();

        ModuleGroupCalibrationViewModel CalibrationRecordVM = new ModuleGroupCalibrationViewModel();


        #region --------------------模组校正首页
        [HttpGet]
        //GET: CalibrationRecords
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            CalibrationRecordVM.AllCalibrationRecord = null;
            ViewBag.Display = "display:none";//隐藏View基本情况信息
            ViewBag.OrderList = GetOrderListForIndex();//向View传递OrderNum订单号列表.

            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Index(string orderNum, string moduleGroupNum, string searchString, int PageIndex = 0)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            //IQueryable<CalibrationRecord> AllCalibrationRecords = null;
            List<CalibrationRecord> AllCalibrationRecords = new List<CalibrationRecord>();

            //检查orderNum和searchString是否为空
            if (orderNum == "")
            {
                //调出全部记录      
                //AllCalibrationRecords = from m in db.CalibrationRecord
                //                        select m;
                AllCalibrationRecords = await db.CalibrationRecord.ToListAsync();
            }
            else
            {
                //筛选出对应orderNum所有记录
                //AllCalibrationRecords = from m in db.CalibrationRecord
                //                        where (m.OrderNum == orderNum)
                //                        select m;
                AllCalibrationRecords = await db.CalibrationRecord.Where(c => c.OrderNum == orderNum).ToListAsync();
                if (AllCalibrationRecords.Count() == 0)
                {
                    var barcodelist =await db.BarCodes.Where(c => c.ToOrderNum == orderNum).ToListAsync();

                    foreach (var item in barcodelist)
                    {
                        AllCalibrationRecords.AddRange(db.CalibrationRecord.Where(c => c.BarCodesNum == item.BarCodesNum));
                    }
                }
            }
            #region-------------按描述条件查询
            if (!String.IsNullOrEmpty(searchString))
            {   //从调出的记录中筛选含searchString内容的记录
                AllCalibrationRecords = AllCalibrationRecords.Where(s => s.AbnormalDescription != null && s.AbnormalDescription.Contains(searchString)).ToList();
            }
            #endregion

            #region-------------按描模组号查询
            if (!String.IsNullOrEmpty(moduleGroupNum))
            {   //从调出的记录中筛选含searchString内容的记录
                AllCalibrationRecords = AllCalibrationRecords.Where(s => s.AbnormalDescription != null && s.ModuleGroupNum.Contains(moduleGroupNum.ToUpper())).ToList();
            }
            #endregion


            //取出对应orderNum校正时长所有记录
            IQueryable<TimeSpan?> TimeSpanList = from m in db.CalibrationRecord
                                                 where (m.OrderNum == orderNum)
                                                 orderby m.CalibrationTime
                                                 select m.CalibrationTime;

            //计算校正总时长
            TimeSpan TotalTimeSpan = DateTime.Now - DateTime.Now;
            if (AllCalibrationRecords.Where(x => x.Normal == true).Count() != 0)
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
                ViewBag.TotalTimeSpan = "暂时没有已完成校正的模组";
            }

            //计算平均用时
            TimeSpan AvgTimeSpan = DateTime.Now - DateTime.Now;
            int Order_CR_valid_Count = AllCalibrationRecords.Where(x => x.CalibrationTime != null).Count();
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
            //CalibrationRecordVM.AllCalibrationRecord = await AllCalibrationRecords.ToListAsync();
            CalibrationRecordVM.AllCalibrationRecord = AllCalibrationRecords.ToList();
            //统计校正结果正常的模组数量
            CalibrationRecordVM.Order_CR_Normal_Count = AllCalibrationRecords.Where(x => x.Normal == true).Count();
            var Abnormal_Count = AllCalibrationRecords.Where(x => x.Normal == false).Count();
            //读出订单中模组总数量
            CalibrationRecordVM.Order_MG_Quantity = (from m in db.OrderInformation
                                                     where (m.OrderNum == orderNum)
                                                     select m.ModuleGroupQuantity).FirstOrDefault();
            //将模组总数量、正常的模组数量、未完成校正模组数量、订单号信息传递到View页面
            ViewBag.Quantity = CalibrationRecordVM.Order_MG_Quantity;
            ViewBag.NormalCount = CalibrationRecordVM.Order_CR_Normal_Count;
            ViewBag.AbnormalCount = Abnormal_Count;
            ViewBag.RecordCount = AllCalibrationRecords.Count();
            ViewBag.NeverFinish = CalibrationRecordVM.Order_MG_Quantity - CalibrationRecordVM.Order_CR_Normal_Count;
            ViewBag.orderNum = orderNum;

            //未选择订单时隐藏基本信息设置
            if (ViewBag.Quantity == 0)
            { ViewBag.Display = "display:none"; }
            else { ViewBag.Display = "display:normal"; }

            ViewBag.OrderList = GetOrderListForIndex();//向View传递OrderNum订单号列表.

            //分页计算功能
            var recordCount = AllCalibrationRecords.Count();
            var pageCount = GetPageCount(recordCount);
            if (PageIndex >= pageCount && pageCount >= 1)
            {
                PageIndex = pageCount - 1;
            }

            CalibrationRecordVM.AllCalibrationRecord = CalibrationRecordVM.AllCalibrationRecord.OrderByDescending(m => m.BeginCalibration)//按条码排序
                                                                            .Skip(PageIndex * PAGE_SIZE)
                                                                            .Take(PAGE_SIZE).ToList();
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageCount = pageCount;


            //将分页后的结果转成JSON数据           
            //var data0 = CalibrationRecordVM.AllCalibrationRecord.OrderByDescending(m => m.BeginCalibration)
            //                                                                .Skip(PageIndex * PAGE_SIZE)
            //                                                                .Take(PAGE_SIZE).ToList();
            //ViewData["data"] = JsonConvert.SerializeObject(data0);
            //ViewData["data"] = JsonConvert.SerializeObject(CalibrationRecordVM.AllCalibrationRecord.OrderByDescending(m => m.BeginCalibration).ToList());

            return View(CalibrationRecordVM);
        }
        #endregion

        #region --------------------校正开始

        // GET: CalibrationRecords/Create
        public ActionResult CreateCal() //async Task<ActionResult> Create()
        {
            ViewBag.OrderList = GetOrderList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "校正员" || ((Users)Session["User"]).Role == "系统管理员")
            {
                return View();
            }
            return RedirectToAction("Index");
        }

        // POST: CalibrationRecords/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCal([Bind(Include = "ID,OrderNum,ModuleGroupNum,BarCodesNum,BeginCalibration,FinishCalibration,Normal,AbnormalDescription,CalibrationTime,CalibrationTimeSpan,Operator,RepetitionCalibration,RepetitionCalibrationCause")] CalibrationRecord calibrationRecord)
        {
            //if (Session["User"] == null)
            //{
            //    return RedirectToAction("Login", "Users");
            //}
            ViewBag.OrderList = GetOrderList();
            var ordernum = db.BarCodes.Where(c => c.BarCodesNum == calibrationRecord.BarCodesNum).ToList().FirstOrDefault();
            
            //找不到订单
            if (ordernum==null)
            {
                ModelState.AddModelError("", "找不到此条码号，请检查订单号或联系PC部门是否已经创建此订单和条码！");
                return View(calibrationRecord);
            }
            
            //订单和条码都正确
            if (ordernum.OrderNum == calibrationRecord.OrderNum)//检查条码是否属于此订单
            {
                
                //如果有正在校正并没有正常完成的记录，打开此记录
                if ( db.CalibrationRecord.Where(c=> c.OrderNum == calibrationRecord.OrderNum && c.BarCodesNum == calibrationRecord.BarCodesNum && c.BeginCalibration!=null && c.FinishCalibration==null).Count()>0)//检查是否有正在校正并没有正常完成的记录
                {   
                    var record = db.CalibrationRecord.Where(c => c.BarCodesNum == calibrationRecord.BarCodesNum && c.BeginCalibration != null && c.FinishCalibration == null).FirstOrDefault();
                    return RedirectToAction("FinishCal", new { record.ID });
                }
                
                //找到已经完成校正的记录，是否重复校正？并写明原因
                else if (db.CalibrationRecord.Where(c =>c.OrderNum==calibrationRecord.OrderNum && c.BarCodesNum==calibrationRecord.BarCodesNum && c.Normal == true && c.BeginCalibration != null && c.FinishCalibration != null).Count() > 0) //已经校正完成
                {   
                    if (calibrationRecord.RepetitionCalibration==true)//重复校正已打钩
                    {
                        calibrationRecord.Operator = ((Users)Session["User"]).UserName;
                        calibrationRecord.BeginCalibration = DateTime.Now;
                        if (ModelState.IsValid)
                        {
                            db.CalibrationRecord.Add(calibrationRecord);
                            db.SaveChanges();
                        }
                        return RedirectToAction("FinishCal", new { calibrationRecord.ID });
                    }
                    else //重复校正没有打钩
                    {
                        ModelState.AddModelError("", "此模组条码已经通过校正了！是否重复校正？");
                        return View(calibrationRecord);
                    }
                }
                
                //如果没有正常的校正记录，新建记录，保存记录
                else
                {
                    if(calibrationRecord.RepetitionCalibration == true)  //重复校正已打钩
                    {
                        ModelState.AddModelError("", "此模组条码从未进行过校正！不能进行\"重复校正\"工作,请取消\"是否重复校正\"选项钩\"？");
                        return View(calibrationRecord);
                    }
                    calibrationRecord.Operator = ((Users)Session["User"]).UserName;
                    calibrationRecord.BeginCalibration = DateTime.Now;
                    if (ModelState.IsValid)
                    {
                        db.CalibrationRecord.Add(calibrationRecord);
                        db.SaveChanges();
                        //把箱体号存到对应的条码号记录中
                        if (calibrationRecord.BarCodesNum != null)
                        {
                            if ((from m in db.BarCodes where m.BarCodesNum == calibrationRecord.BarCodesNum select m).Count() > 0)
                            {
                                var barcode = (from m in db.BarCodes where m.BarCodesNum == calibrationRecord.BarCodesNum select m).FirstOrDefault();
                                barcode.ModuleGroupNum = calibrationRecord.ModuleGroupNum;
                                db.Entry(barcode).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                        }
                        return RedirectToAction("FinishCal", new { calibrationRecord.ID });
                    }
                    else //模型数据有误
                    {
                        //返回提示信息
                        ModelState.AddModelError("", "信息有误，请检查！");
                        return View(calibrationRecord);
                    }
                }
            }
            //订单选择有误，返回提示信息
            else
            {
                
                ModelState.AddModelError("", "模组条码号应该属于"+ ordernum.OrderNum + "订单，请确定订单是否正确！");
                return View(calibrationRecord);
            }
        }


        #endregion

        #region --------------------校正完成
        // GET: CalibrationRecords/Edit/5
        public ActionResult FinishCal(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalibrationRecord calibrationRecord = db.CalibrationRecord.Find(id);
            if (calibrationRecord == null)
            {
                return HttpNotFound();
            }
            return View(calibrationRecord);
        }

        // POST: CalibrationRecords/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinishCal([Bind(Include = "ID,OrderNum,ModuleGroupNum,BarCodesNum,BeginCalibration,FinishCalibration,Normal,AbnormalDescription,CalibrationTime,CalibrationTimeSpan,Operator,RepetitionCalibration,RepetitionCalibrationCause")] CalibrationRecord calibrationRecord)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (calibrationRecord.FinishCalibration == null)
            {
                calibrationRecord.FinishCalibration = DateTime.Now;

                var BC = calibrationRecord.BeginCalibration.Value;
                var FC = calibrationRecord.FinishCalibration.Value;
                var CT = FC - BC;
                calibrationRecord.CalibrationTime = CT;
                calibrationRecord.CalibrationTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
            }

            if (ModelState.IsValid)
            {
                db.Entry(calibrationRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CreateCal");
            }
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            return View(calibrationRecord);
        }
        #endregion



        #region --------------------Edit页面

        // GET: CalibrationRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalibrationRecord calibrationRecord = db.CalibrationRecord.Find(id);
            if (calibrationRecord == null)
            {
                return HttpNotFound();
            }
            return View(calibrationRecord);
        }

        // POST: CalibrationRecords/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OrderNum,ModuleGroupNum,BarCodesNum,BeginCalibration,FinishCalibration,Normal,AbnormalDescription,CalibrationTime,CalibrationTimeSpan,Operator,RepetitionCalibration,RepetitionCalibrationCause")] CalibrationRecord calibrationRecord)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }


            if (calibrationRecord.FinishCalibration == null)
            {
                calibrationRecord.FinishCalibration = DateTime.Now;

                var BC = calibrationRecord.BeginCalibration.Value;
                var FC = calibrationRecord.FinishCalibration.Value;
                var CT = FC - BC;
                calibrationRecord.CalibrationTime = CT;
                calibrationRecord.CalibrationTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
            }
            if (ModelState.IsValid)
            {
                db.Entry(calibrationRecord).State = EntityState.Modified;
                db.SaveChanges();
                //tempOrderNum = calibrationRecord.OrderNum;
                return RedirectToAction("Create");
            }
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            return View(calibrationRecord);
        }


        #endregion


        #region --------------------Details页

        // GET: CalibrationRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalibrationRecord calibrationRecord = db.CalibrationRecord.Find(id);
            if (calibrationRecord == null)
            {
                return HttpNotFound();
            }
            return View(calibrationRecord);
        }
        #endregion


        #region --------------------Delete页
        // GET: CalibrationRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalibrationRecord calibrationRecord = db.CalibrationRecord.Find(id);
            if (calibrationRecord == null)
            {
                return HttpNotFound();
            }
            return View(calibrationRecord);
        }

        // POST: CalibrationRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CalibrationRecord calibrationRecord = db.CalibrationRecord.Find(id);
            db.CalibrationRecord.Remove(calibrationRecord);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion


        #region --------------------其他方法

        private TimeSpan DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;

            TimeSpan ts = DateTime1.Subtract(DateTime2).Duration();
            dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
            //return dateDiff;
            return ts;

        }

        private TimeSpan TimeSpanAdd(TimeSpan TimeSpan1, TimeSpan TimeSpan2)
        {
            TimeSpan TimeAdd = new TimeSpan(TimeSpan1.Ticks);
            TimeSpan TotalTimeSpan = TimeSpan1.Add(TimeSpan2).Duration();
            return TotalTimeSpan;
        }






        #endregion


        #region --------------------取出整个OrderNum订单号列表
        private List<SelectListItem> GetOrderListForIndex()
        {
            var orders = db.OrderInformation.OrderByDescending(m => m.CreateDate).Select(m => m.OrderNum).ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            var list = db.OrderMgm.OrderByDescending(c => c.ID).Select(c => c.OrderNum).ToList();
            var listall = orders.Union(list).ToList();
            var items = new List<SelectListItem>();
            foreach (var value in listall)
            {
                items.Add(new SelectListItem
                {
                    Text = value,
                    Value = value
                });
            }
            return items;
        }

        private List<SelectListItem> GetOrderList()
        {
            var list = db.OrderMgm.OrderByDescending(c => c.ID).Select(c => c.OrderNum).ToList();
            var items = new List<SelectListItem>();
            foreach (var i in list)
            {
                items.Add(new SelectListItem
                {
                    Text = i,
                    Value = i
                });
            }
            return items;
        }

        //----------------------------------------------------------------------------------------
        #endregion

        #region --------------------分页函数
        static List<ModuleGroupCalibrationViewModel> GetPageListByIndex(List<ModuleGroupCalibrationViewModel> list, int pageIndex)
        {
            int pageSize = 10;
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }


        //分页方法
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

        private ModuleGroupCalibrationViewModel GetPagedDataSource(IQueryable<CalibrationRecord> AllCalibrationRecords, int pageIndex, int recordCount)
        {
            var pageCount = GetPageCount(recordCount);
            if (pageIndex >= pageCount && pageCount >= 1)
            {
                pageIndex = pageCount - 1;
            }
            CalibrationRecordVM.AllCalibrationRecord = AllCalibrationRecords.OrderByDescending(m => m.BeginCalibration)
                                                                           .Skip(pageIndex * PAGE_SIZE)
                                                                           .Take(PAGE_SIZE).ToList();
            return CalibrationRecordVM;
            //return CalibrationRecordVM.AllCalibrationRecords.OrderByDescending(m => m.BeginCalibration)
            //                            .Skip(pageIndex * PAGE_SIZE)
            //                            .Take(PAGE_SIZE).ToList();
        }


        //----------------------------------------------------------------------------------------
        #endregion

    }
}