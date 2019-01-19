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
    public class FinalQCsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        #region ----------FinalQC首页
        // GET: FinalQCs
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            ViewBag.Display = "display:none";//隐藏View基本情况信息
            ViewBag.Legend = "display:none";//隐藏图例
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.FQCNormal = FQCNormalList();
            ViewBag.Repetition = Repetition();//是否重复FQC
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(string orderNum, string repetition, string FQCNormal, string Remark, int PageIndex = 0)
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.FQCNormal = FQCNormalList();
            ViewBag.Repetition = Repetition();//是否重复FQC

            List<FinalQC> resultlist = new List<FinalQC>();
            //根据订单号筛选
            resultlist = await db.FinalQC.Where(c => c.OrderNum == orderNum).ToListAsync();
            //根据FQCNormal筛选
            if (FQCNormal != "")
            {
                if (FQCNormal == "正常")
                {
                    resultlist = resultlist.Where(c => c.FinalQC_FQCCheckAbnormal == "正常").ToList();
                }
                else
                {
                    resultlist = resultlist.Where(c => c.FinalQC_FQCCheckAbnormal != "正常").ToList();
                }
            }
            //根据首次或重复FQC筛选
            if (repetition != "")
            {
                if (repetition == "首次FQC")
                {
                    resultlist = resultlist.Where(c => c.RepetitionFQCCheck == false).ToList();
                }
                else  //筛选重复FQC记录
                {
                    resultlist = resultlist.Where(c => c.RepetitionFQCCheck == true).ToList();
                }
            }
            //根据备注内容筛选
            if (Remark != "")
            {
                resultlist = resultlist.Where(c => c.Remark != null && c.Remark.Contains(Remark)).ToList();
            }
            return View(resultlist);
        }
        #endregion


        #region ----------FinalQC_B方法

        public ActionResult FinalQC_B()
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "FQC" || ((Users)Session["User"]).Role == "系统管理员")
            {
                return View();
            }
            return Content("<script>alert('对不起，您不能进行FQC操作，请联系品质部管理人员！');window.location.href='../FinalQCs';</script>");
        }

        // POST: FinalQCs/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FinalQC_B([Bind(Include = "Id,OrderNum,BarCodesNum,FQCCheckBT,FQCPrincipal,FQCCheckFT,FQCCheckDate,FQCCheckTime,FQCCheckTimeSpan,FinalQC_FQCCheckAbnormal,RepetitionFQCCheck,RepetitionFQCCheckCause,FQCCheckFinish,Remark")] FinalQC finalQC)
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.


            //1.判断条码是否存在
            //var aaa = db.BarCodes.Where(c => c.BarCodesNum == finalQC.BarCodesNum).Count();
            if (db.BarCodes.Where(c => c.BarCodesNum == finalQC.BarCodesNum).Count() == 0)
            {
                ModelState.AddModelError("", "条码号不存在，请检查条码输入是否正确！");
                return View(finalQC);
            }
            //2.判断条码跟订单号是否相符
            if (finalQC.OrderNum != db.BarCodes.Where(c => c.BarCodesNum == finalQC.BarCodesNum).FirstOrDefault().OrderNum)
            {
                ModelState.AddModelError("", "条码号不属于" + finalQC.OrderNum + "订单，应该属于" + db.BarCodes.Where(c => c.BarCodesNum == finalQC.BarCodesNum).FirstOrDefault().OrderNum + "订单！");
                return View(finalQC);
            }

            //创建一个集合存放此条码对应的记录
            var fqc_recordlist = db.FinalQC.Where(c => c.BarCodesNum == finalQC.BarCodesNum).ToList();

            //3.判断PQC是否已经完成,如果PQC已完成，允许进行FQC
            if (fqc_recordlist.Count() > 0)
            {
                //FQC判断1.有记录已完成，要求“重复”打钩，2.有异常记录未通过FQC，“重复”不能打钩，3.有记录，有开始时间，没有完成时间，4，没有记录

                //统计此条码在FinalQC表中的记录个数，如果Fqc_record_count=0，则直接创建记录。
                //如果>0，创建一个集合存放此条码对应的记录
                //检查是否有记录是有开始时间没有完成时间，有则打开记录。如果没有，接着做下面的检查
                //检查是否已经有首次FinalQC完成，如果没有，按首次FinalQC进行，“重复FQC”选项不能勾选，如果勾选上了，输出错误提示“此模组尝未进行FQC工作，不能进行“重复FQC”工作，请取消“重复FQC”选项勾！”
                //检查如果此条码首次FinalQC已经完成，则按重复FinalQC进行，“重复FQC”选项要求要被勾选上，如果没有勾选上，输出错误提示“此模组已经完成FQC工作，只能进行“重复FQC”工作，请勾上“重复FQC”选项勾！”

                //统计此条码在FinalQC表中的记录个数
                int fqc_record_count = db.FinalQC.Where(c => c.BarCodesNum == finalQC.BarCodesNum).Count();


                //(1).有不完整的记录（有开始时间没有完成时间）
                if (fqc_recordlist.Where(c => c.FQCCheckBT != null && c.FQCCheckFT == null).Count() > 0)
                {
                    return RedirectToAction("FinalQC_F", new { fqc_recordlist.Where(c => c.FQCCheckBT != null && c.FQCCheckFT == null).FirstOrDefault().Id });
                }

                //(2).有记录，FQC异常，未完成FQC，创建一条新记录（不允许勾选“重复FQC”）
                else if (fqc_recordlist.Where(c => c.FQCCheckFinish == true).Count() == 0)
                {
                    if (finalQC.RepetitionFQCCheck == false)  //重复FQC要求不能勾上
                    {
                        finalQC.FQCPrincipal = ((Users)Session["User"]).UserName;
                        finalQC.FQCCheckBT = DateTime.Now;
                        if (ModelState.IsValid)
                        {
                            db.FinalQC.Add(finalQC);
                            await db.SaveChangesAsync();
                            return RedirectToAction("FinalQC_F", new { finalQC.Id });
                        }
                    }
                    else  //重复FQC未勾上，提示错误
                    {
                        ModelState.AddModelError("", "此模组未完成FQC工作，不能进行“重复FQC”工作，请取消“重复FQC”选项勾！");
                        return View(finalQC);
                    }
                }

                //(3).检查是否已经有首次FinalQC完成，如果首次FQC已完成，进入重复FQC
                else  //首次FQC已完成，应该进入重复FQC
                {
                    if (finalQC.RepetitionFQCCheck == true)  //重复FQC已经勾上
                    {
                        finalQC.FQCPrincipal = ((Users)Session["User"]).UserName;
                        finalQC.FQCCheckBT = DateTime.Now;
                        if (ModelState.IsValid)
                        {
                            db.FinalQC.Add(finalQC);
                            await db.SaveChangesAsync();
                            return RedirectToAction("FinalQC_F", new { finalQC.Id });
                        }
                    }
                    else  //重复FQC未勾上，提示错误
                    {
                        ModelState.AddModelError("", "此模组已完成FQC工作，只能进行“重复FQC”工作，请勾上“重复FQC”选项勾！");
                        return View(finalQC);
                    }
                }
            }
            else //.无记录，直接创建
            {
                finalQC.FQCPrincipal = ((Users)Session["User"]).UserName;
                finalQC.FQCCheckBT = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.FinalQC.Add(finalQC);
                    await db.SaveChangesAsync();
                    return RedirectToAction("FinalQC_F", new { finalQC.Id });
                }

                //ModelState.AddModelError("", finalQC.BarCodesNum + "已完成PQC" );
                //return View(finalQC);
            }
            return View(finalQC);
        }
        #endregion


        #region ----------FinalQC_F方法
        public async Task<ActionResult> FinalQC_F(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinalQC finalQC = await db.FinalQC.FindAsync(id);
            if (finalQC == null)
            {
                return HttpNotFound();
            }
            return View(finalQC);
        }

        // POST: FinalQCs/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FinalQC_F([Bind(Include = "Id,OrderNum,BarCodesNum,FQCCheckBT,FQCPrincipal,FQCCheckFT,FQCCheckDate,FQCCheckTime,FQCCheckTimeSpan,FinalQC_FQCCheckAbnormal,RepetitionFQCCheck,RepetitionFQCCheckCause,FQCCheckFinish,Remark")] FinalQC finalQC)
        {
            //获取完成时间
            finalQC.FQCCheckFT = DateTime.Now;
            //计算时长
            finalQC.FQCCheckTime = finalQC.FQCCheckFT - finalQC.FQCCheckBT;
            //输出时间戳字符串
            finalQC.FQCCheckTimeSpan = (finalQC.FQCCheckTime.Value.Days > 0 ? finalQC.FQCCheckTime.Value.Days + "天" : "") + (finalQC.FQCCheckTime.Value.Hours > 0 ? finalQC.FQCCheckTime.Value.Hours + "小时" : "") + (finalQC.FQCCheckTime.Value.Minutes > 0 ? finalQC.FQCCheckTime.Value.Minutes + "分" : "") + finalQC.FQCCheckTime.Value.Seconds + "秒";
            //时长超一天，把天数部分存储在另一字段FQCCheckDate中
            finalQC.FQCCheckDate = finalQC.FQCCheckTime.Value.Days > 0 ? finalQC.FQCCheckTime.Value.Days : 0;
            //时长保留时分秒部分存储在FQCCheckTime中
            finalQC.FQCCheckTime = new TimeSpan(finalQC.FQCCheckTime.Value.Hours, finalQC.FQCCheckTime.Value.Minutes, finalQC.FQCCheckTime.Value.Seconds);

            if (finalQC.FinalQC_FQCCheckAbnormal != "正常")
            {
                finalQC.FQCCheckFinish = false;
            }
            else
            {
                finalQC.FQCCheckFinish = true;
            }

            if (ModelState.IsValid)
            {
                db.Entry(finalQC).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("FinalQC_B");
            }
            return View(finalQC);
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


        #region ----------其他方法

        // GET: FinalQCs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinalQC finalQC = await db.FinalQC.FindAsync(id);
            if (finalQC == null)
            {
                return HttpNotFound();
            }
            return View(finalQC);
        }

        // GET: FinalQCs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FinalQCs/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,OrderNum,BarCodesNum,FQCCheckBT,FQCPrincipal,FQCCheckFT,FQCCheckDate,FQCCheckTime,FQCCheckTimeSpan,FinalQC_FQCCheckAbnormal,RepetitionFQCCheck,RepetitionFQCCheckCause,FQCCheckFinish,Remark")] FinalQC finalQC)
        {
            if (ModelState.IsValid)
            {
                db.FinalQC.Add(finalQC);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(finalQC);
        }

        // GET: FinalQCs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinalQC finalQC = await db.FinalQC.FindAsync(id);
            if (finalQC == null)
            {
                return HttpNotFound();
            }
            return View(finalQC);
        }

        // POST: FinalQCs/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,OrderNum,BarCodesNum,FQCCheckBT,FQCPrincipal,FQCCheckFT,FQCCheckDate,FQCCheckTime,FQCCheckTimeSpan,FinalQC_FQCCheckAbnormal,RepetitionFQCCheck,RepetitionFQCCheckCause,FQCCheckFinish,Remark")] FinalQC finalQC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(finalQC).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(finalQC);
        }

        // GET: FinalQCs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FinalQC finalQC = await db.FinalQC.FindAsync(id);
            if (finalQC == null)
            {
                return HttpNotFound();
            }
            return View(finalQC);
        }

        // POST: FinalQCs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            FinalQC finalQC = await db.FinalQC.FindAsync(id);
            db.FinalQC.Remove(finalQC);
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

        #region --------------------FQCNormal列表
        private List<SelectListItem> FQCNormalList()
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

        #region --------------------是否重复FQC列表
        private List<SelectListItem> Repetition()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "首次FQC",
                    Value = "首次FQC"
                },
                new SelectListItem
                {
                    Text = "重复FQC",
                    Value = "重复FQC"
                }
            };
        }
        #endregion

        #region --------------------分页函数
        static List<FinalQC> GetPageListByIndex(List<FinalQC> list, int pageIndex)
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

        private List<FinalQC> GetPagedDataSource(List<FinalQC> finalQCs, int pageIndex, int recordCount)
        {
            var pageCount = GetPageCount(recordCount);
            if (pageIndex >= pageCount && pageCount >= 1)
            {
                pageIndex = pageCount - 1;
            }
            finalQCs = finalQCs.OrderByDescending(m => m.FQCCheckBT)
                               .Skip(pageIndex * PAGE_SIZE)
                               .Take(PAGE_SIZE).ToList();
            return finalQCs;
        }
        #endregion


    }
}