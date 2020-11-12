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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JianHeMES.AuthAttributes;

namespace JianHeMES.Controllers
{
    public class FinalQCsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController com = new CommonController();
        public ActionResult New_Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "FinalQCs", act = "New_Index" });
            }
            return View();
        }
        public ActionResult New_FinalQC()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "FinalQCs", act = "New_FinalQC" });
            }
            return View();
        }
        public ActionResult recordBarcode()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "FinalQCs", act = "recordBarcode" });
            }
            return View();
        }

        #region ----------FinalQC首页
        // GET: FinalQCs
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "FinalQCs", act = "Index" });
            }
            ViewBag.Display = "display:none";//隐藏View基本情况信息
            ViewBag.Legend = "display:none";//隐藏图例
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.FQCNormal = FQCNormalList();
            ViewBag.Repetition = Repetition();//是否重复FQC
            ViewBag.NotDo = null;
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
            resultlist = await db.FinalQC.Where(c => c.OrderNum == orderNum && (c.OldOrderNum == null || c.OldOrderNum == orderNum)).ToListAsync();
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

            #region   ----------筛选从未开始、未完成、正在进行FQC的条码清单------------
            List<FinalQC> BarCodesList = (from m in db.FinalQC where m.OrderNum == orderNum && (m.OldOrderNum == null || m.OldOrderNum == orderNum) select m).ToList();
            List<string> NotFQCList = new List<string>();
            List<string> DoingNow = new List<string>();
            List<string> NeverFinishList = new List<string>();
            foreach (var barcode in BarCodesList)
            {
                if ((from m in db.FinalQC where m.BarCodesNum == barcode.BarCodesNum && (m.OldBarCodesNum == null || m.OldBarCodesNum == barcode.BarCodesNum) select m).Count() == 0)
                {
                    NotFQCList.Add(barcode.BarCodesNum);
                }
            }
            ViewBag.NotDo = NotFQCList;//输出未进行FQC的条码清单
            int barcodeslistcount = NotFQCList.Count;//未进行FQC数量
            ViewBag.NotDoCount = barcodeslistcount;//未进行FQC数量

            foreach (var barcode in BarCodesList)
            {
                if (db.FinalQC.Where(m => m.BarCodesNum == barcode.BarCodesNum && (m.OldBarCodesNum == null || m.OldBarCodesNum == barcode.BarCodesNum)).Where(m => m.FQCCheckBT != null && m.FQCCheckFT == null).Count() > 0)
                {
                    DoingNow.Add(barcode.BarCodesNum);
                }
            }
            ViewBag.DoingNowList = DoingNow;//正在做FQC的条码清单
            ViewBag.DoingNowListCount = DoingNow.Count();//正在做FQC的条码个数

            foreach (var barcode in BarCodesList)
            {
                if (resultlist.Where(m => m.BarCodesNum == barcode.BarCodesNum).Count(c => c.FQCCheckFinish == true) == 0)
                {
                    NeverFinishList.Add(barcode.BarCodesNum);
                }
            }
            ViewBag.NeverFinishList = NeverFinishList.Except(NotFQCList).Except(DoingNow);//输出未完成PQC的条码清单
            ViewBag.NeverFinishListCount = NeverFinishList.Except(NotFQCList).Except(DoingNow).Count();//未完成PQC的条码数量
            #endregion

            #region----------筛选出完成组装，超过24小时未进入老化工序的记录JSON------------
            var BarcodesListByOrderNum = resultlist.Select(c => c.BarCodesNum).Distinct().ToList();//1和2
            JObject BorcodesOvertimeList = new JObject();//3
            var AllBurn_inRecordByOrderNum = db.Burn_in.Where(c => c.OrderNum == orderNum && (c.OldOrderNum == null || c.OldOrderNum == orderNum)).ToList();//4
            foreach (var item in BarcodesListByOrderNum)
            {
                //老化记录数
                int AllBurn_in_Records_Count = AllBurn_inRecordByOrderNum.Count(c => c.BarCodesNum == item);
                if (AllBurn_in_Records_Count == 0)
                {
                    var FQC_count = resultlist.Count(c => c.BarCodesNum == item && c.FQCCheckFinish == true);
                    if (FQC_count >= 1)
                    {
                        var FQC_FT = resultlist.Where(c => c.BarCodesNum == item && c.FQCCheckFinish == true).FirstOrDefault().FQCCheckFT;
                        var SubTime = DateTime.Now - FQC_FT;
                        if (FQC_FT != null && SubTime.Value.TotalMinutes > 1440) //1天：1*24*60
                        {
                            BorcodesOvertimeList.Add(item, "此模组在" + FQC_FT.ToString() + "完成组装,已经超过" + SubTime.Value.Days + "天" + SubTime.Value.Hours + "小时" + SubTime.Value.Minutes + "分未进入老化工序！");
                        }
                    }
                }
            }
            ViewBag.Overtime = BorcodesOvertimeList;
            #endregion

            //读出订单中模组总数量
            var Quantity = (from m in db.OrderMgm
                            where (m.OrderNum == orderNum)
                            select m.Boxes).FirstOrDefault();
            //统计校正结果正常的模组数量
            var FQC_Normal_Count = resultlist.Where(m => m.FQCCheckFinish == true && m.RepetitionFQCCheck == false).Count();
            var FQC_Abnormal_Count = resultlist.Where(m => m.FQCCheckFinish != false).Count();

            //将模组总数量、正常的模组数量、未完成校正模组数量、订单号信息传递到View页面
            ViewBag.Quantity = Quantity;
            ViewBag.NormalCount = FQC_Normal_Count;
            ViewBag.AbnormalCount = FQC_Abnormal_Count;
            ViewBag.RecordCount = resultlist.Count();
            ViewBag.NeverFinish = Quantity - FQC_Normal_Count;
            ViewBag.orderNum = orderNum;

            resultlist = resultlist.OrderBy(c => c.BarCodesNum).ToList();
            return View(resultlist);
        }

        public ActionResult NewIndex(string ordernum)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Appearances", act = "Index" });
            }
            var info = db.FinalQC.Where(c => c.OrderNum == ordernum && (c.OldOrderNum == null || c.OldOrderNum == ordernum)).Select(c => new CommonController.TempIndex
            {
                BarCodesNum = c.BarCodesNum,
                EndTime = c.FQCCheckFT,
                Finish = c.FQCCheckFinish,
                StarTime = c.FQCCheckBT,
                OrderNum = c.OrderNum,
                Group = c.Group,
                Principal = c.FQCPrincipal,
                Abnormal = c.FinalQC_FQCCheckAbnormal,
                Repetition = c.RepetitionFQCCheck,
                RepetitionCause = c.RepetitionFQCCheckCause,

            }).ToList();

            return Content(JsonConvert.SerializeObject(com.GeneralIndex(ordernum, info)));
        }
        #endregion


        #region ----------FinalQC_B方法

        public ActionResult FinalQC_B()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "FinalQCs", act = "FinalQC_B" });
            }
            CommonController com = new CommonController();
            //if (((Users)Session["User"]).Role == "FQC" || com.isCheckRole("FQC管理", "开始FQC检验", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            return View();
            ////}
            //return Content("<script>alert('对不起，您不能进行FQC操作，请联系品质部管理人员！');window.location.href='../FinalQCs';</script>");
        }

        // POST: FinalQCs/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FinalQC_B([Bind(Include = "Id,OrderNum,BarCodesNum,FQCCheckBT,FQCPrincipal,FQCCheckFT,FQCCheckDate,FQCCheckTime,FQCCheckTimeSpan,FinalQC_FQCCheckAbnormal,RepetitionFQCCheck,RepetitionFQCCheckCause,FQCCheckFinish,Remark,Department1,Group")] FinalQC finalQC, string nuoOrder = null, string nuoBarCode = null, string isnuo = null)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "FinalQCs", act = "FinalQC_B" });
            }

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
            var fqc_recordlist = db.FinalQC.Where(c => c.BarCodesNum == finalQC.BarCodesNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == finalQC.BarCodesNum));

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
                int fqc_record_count = db.FinalQC.Where(c => c.BarCodesNum == finalQC.BarCodesNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == finalQC.BarCodesNum)).Count();


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
                        finalQC.OldOrderNum = finalQC.OrderNum;
                        finalQC.OldBarCodesNum = finalQC.BarCodesNum;
                        if (ModelState.IsValid)
                        {
                            db.FinalQC.Add(finalQC);
                            await db.SaveChangesAsync();
                            //添加关联表
                            if (!string.IsNullOrEmpty(nuoBarCode) && isnuo == "true")
                            {
                                BarCodeRelation barcoderelation = new BarCodeRelation() { OldOrderNum = nuoOrder, OldBarCodeNum = nuoBarCode, NewBarCodesNum = finalQC.BarCodesNum, NewOrderNum = finalQC.OrderNum, Procedure = "FQC", UsserID = ((Users)Session["User"]).UserNum, CreateDate = DateTime.Now };
                                if (!comm.InsertRelation(barcoderelation))
                                {
                                    ModelState.AddModelError("", "挪用订单失败，此条码已挪用过库存条码");
                                    return View(finalQC);
                                }
                                #region 挪用修改原条码工序记录
                                com.NuoOperation(nuoBarCode, nuoOrder, finalQC.BarCodesNum, finalQC.OrderNum);
                                #endregion
                            }
                            return RedirectToAction("FinalQC_F", new { finalQC.Id });
                        }
                    }
                    else  //重复FQC未勾上，提示错误
                    {
                        ModelState.AddModelError("", "此模组未完成FQC工作，不能进行“重复”FQC工作，请取消“重复”FQC选项勾！");
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
                        finalQC.OldBarCodesNum = finalQC.BarCodesNum;
                        finalQC.OldOrderNum = finalQC.OrderNum;
                        if (ModelState.IsValid)
                        {
                            db.FinalQC.Add(finalQC);
                            await db.SaveChangesAsync();
                            //添加关联表
                            if (!string.IsNullOrEmpty(nuoBarCode) && isnuo == "true")
                            {
                                BarCodeRelation barcoderelation = new BarCodeRelation() { OldOrderNum = nuoOrder, OldBarCodeNum = nuoBarCode, NewBarCodesNum = finalQC.BarCodesNum, NewOrderNum = finalQC.OrderNum, Procedure = "FQC", UsserID = ((Users)Session["User"]).UserNum, CreateDate = DateTime.Now };
                                if (!comm.InsertRelation(barcoderelation))
                                {
                                    ModelState.AddModelError("", "挪用订单失败，此条码已挪用过库存条码");
                                    return View(finalQC);
                                }
                                #region 挪用修改原条码工序记录
                                com.NuoOperation(nuoBarCode, nuoOrder, finalQC.BarCodesNum, finalQC.OrderNum);
                                #endregion
                            }
                            return RedirectToAction("FinalQC_F", new { finalQC.Id });
                        }
                    }
                    else  //重复FQC未勾上，提示错误
                    {
                        ModelState.AddModelError("", "此模组已完成FQC工作，只能进行“重复”FQC工作，请勾上“重复”FQC选项勾！");
                        return View(finalQC);
                    }
                }
            }
            else //4.无记录，直接创建
            {
                if (finalQC.RepetitionFQCCheck)
                {
                    ModelState.AddModelError("", "模组未从未进行过FQC工作，不能进行“重复”FQC工作，不能勾选“重复”FQC选项！");
                    finalQC.RepetitionFQCCheck = true;
                    return View(finalQC);
                }
                finalQC.FQCPrincipal = ((Users)Session["User"]).UserName;
                finalQC.FQCCheckBT = DateTime.Now;
                finalQC.OldOrderNum = finalQC.OrderNum;
                finalQC.OldBarCodesNum = finalQC.BarCodesNum;
                if (ModelState.IsValid)
                {
                    db.FinalQC.Add(finalQC);
                    await db.SaveChangesAsync();
                    //添加关联表
                    if (!string.IsNullOrEmpty(nuoBarCode) && isnuo == "true")
                    {
                        BarCodeRelation barcoderelation = new BarCodeRelation() { OldOrderNum = nuoOrder, OldBarCodeNum = nuoBarCode, NewBarCodesNum = finalQC.BarCodesNum, NewOrderNum = finalQC.OrderNum, Procedure = "FQC", UsserID = ((Users)Session["User"]).UserNum, CreateDate = DateTime.Now };
                        if (!comm.InsertRelation(barcoderelation))
                        {
                            ModelState.AddModelError("", "挪用订单失败，此条码已挪用过库存条码");
                            return View(finalQC);
                        }
                        #region 挪用修改原条码工序记录
                        com.NuoOperation(nuoBarCode, nuoOrder, finalQC.BarCodesNum, finalQC.OrderNum);
                        #endregion
                    }
                    return RedirectToAction("FinalQC_F", new { finalQC.Id });
                }
                //ModelState.AddModelError("", finalQC.BarCodesNum + "已完成PQC" );
                //return View(finalQC);
            }
            return View(finalQC);
        }


        public async Task<ActionResult> FinalQC_B1(FinalQC finalQC)
        {
            JObject result = new JObject();
            //创建一个集合存放此条码对应的记录
            var fqc_recordlist = db.FinalQC.Where(c => c.BarCodesNum == finalQC.BarCodesNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == finalQC.BarCodesNum));
            //1.判断条码是否存在
            //var aaa = db.BarCodes.Where(c => c.BarCodesNum == finalQC.BarCodesNum).Count();
            if (db.BarCodes.Where(c => c.BarCodesNum == finalQC.BarCodesNum).Count() == 0)
            {
                result.Add("mes", "条码号不存在，请检查条码输入是否正确！");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));
            }
            //2.判断条码跟订单号是否相符
            else if (finalQC.OrderNum != db.BarCodes.Where(c => c.BarCodesNum == finalQC.BarCodesNum).FirstOrDefault().OrderNum)
            {
                result.Add("mes", "条码号不属于" + finalQC.OrderNum + "订单，应该属于" + db.BarCodes.Where(c => c.BarCodesNum == finalQC.BarCodesNum).FirstOrDefault().OrderNum + "订单！");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));
            }

            //3.判断PQC是否已经完成,如果PQC已完成，允许进行FQC
            else if (fqc_recordlist.Count() > 0)
            {
                //统计此条码在FinalQC表中的记录个数
                int fqc_record_count = db.FinalQC.Where(c => c.BarCodesNum == finalQC.BarCodesNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == finalQC.BarCodesNum)).Count();


                //(1).有不完整的记录（有开始时间没有完成时间）
                if (fqc_recordlist.Where(c => c.FQCCheckBT != null && c.FQCCheckFT == null).Count() > 0)
                {

                    result.Add("mes", "成功");
                    result.Add("pass", true);
                    return Content(JsonConvert.SerializeObject(result));
                }

                //(2).有记录，FQC异常，未完成FQC，创建一条新记录（不允许勾选“重复FQC”）
                else if (fqc_recordlist.Where(c => c.FQCCheckFinish == true).Count() == 0)
                {
                    if (finalQC.RepetitionFQCCheck == false)  //重复FQC要求不能勾上
                    {
                        finalQC.FQCPrincipal = ((Users)Session["User"]).UserName;
                        finalQC.FQCCheckBT = DateTime.Now;
                        finalQC.OldOrderNum = finalQC.OrderNum;
                        finalQC.OldBarCodesNum = finalQC.BarCodesNum;
                        if (ModelState.IsValid)
                        {
                            db.FinalQC.Add(finalQC);
                            await db.SaveChangesAsync();
                            result.Add("mes", "成功");
                            result.Add("pass", true);
                            return Content(JsonConvert.SerializeObject(result));
                        }
                    }
                    else  //重复FQC未勾上，提示错误
                    {
                        result.Add("mes", "此模组未完成FQC工作，不能进行“重复”FQC工作，请取消“重复”FQC选项勾！");
                        result.Add("pass", false);
                        return Content(JsonConvert.SerializeObject(result));

                    }
                }

                //(3).检查是否已经有首次FinalQC完成，如果首次FQC已完成，进入重复FQC
                else  //首次FQC已完成，应该进入重复FQC
                {
                    if (finalQC.RepetitionFQCCheck == true)  //重复FQC已经勾上
                    {
                        finalQC.FQCPrincipal = ((Users)Session["User"]).UserName;
                        finalQC.FQCCheckBT = DateTime.Now;
                        finalQC.OldBarCodesNum = finalQC.BarCodesNum;
                        finalQC.OldOrderNum = finalQC.OrderNum;
                        if (ModelState.IsValid)
                        {
                            db.FinalQC.Add(finalQC);
                            await db.SaveChangesAsync();
                            result.Add("mes", "成功");
                            result.Add("pass", true);
                            return Content(JsonConvert.SerializeObject(result));
                        }
                    }
                    else  //重复FQC未勾上，提示错误
                    {
                        result.Add("mes", "此模组已完成FQC工作，只能进行“重复”FQC工作，请勾上“重复”FQC选项勾！");
                        result.Add("pass", false);
                        return Content(JsonConvert.SerializeObject(result));
                    }
                }
            }
            else //4.无记录，直接创建
            {
                if (finalQC.RepetitionFQCCheck)
                {
                    result.Add("mes", "模组未从未进行过FQC工作，不能进行“重复”FQC工作，不能勾选“重复”FQC选项！");
                    result.Add("pass", false);
                    return Content(JsonConvert.SerializeObject(result));
                }
                finalQC.FQCPrincipal = ((Users)Session["User"]).UserName;
                finalQC.FQCCheckBT = DateTime.Now;
                finalQC.OldOrderNum = finalQC.OrderNum;
                finalQC.OldBarCodesNum = finalQC.BarCodesNum;
                if (ModelState.IsValid)
                {
                    db.FinalQC.Add(finalQC);
                    await db.SaveChangesAsync();
                    result.Add("mes", "成功");
                    result.Add("pass", true);
                    return Content(JsonConvert.SerializeObject(result));
                }
                //ModelState.AddModelError("", finalQC.BarCodesNum + "已完成PQC" );
                //return View(finalQC);
            }
            result.Add("mes", "逻辑错误");
            result.Add("pass", false);
            return Content(JsonConvert.SerializeObject(result));
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
            //添加库存条码信息
            var nuobarcode = db.BarCodeRelation.Where(c => c.NewBarCodesNum == finalQC.BarCodesNum && c.NewOrderNum == finalQC.OrderNum).Select(c => c.OldBarCodeNum).FirstOrDefault();
            if (nuobarcode == null)
                ViewBag.nuoBarcode = "";
            else
                ViewBag.nuoBarcode = nuobarcode;

            return View(finalQC);
        }

        // POST: FinalQCs/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FinalQC_F([Bind(Include = "Id,OrderNum,BarCodesNum,FQCCheckBT,FQCPrincipal,FQCCheckFT,FQCCheckDate,FQCCheckTime,FQCCheckTimeSpan,FinalQC_FQCCheckAbnormal,RepetitionFQCCheck,RepetitionFQCCheckCause,FQCCheckFinish,Remark,Department1,Group")] FinalQC finalQC)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "FinalQCs", act = "FinalQC_B" });
            }
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
                finalQC.OldOrderNum = finalQC.OrderNum;
                finalQC.OldBarCodesNum = finalQC.BarCodesNum;
                db.Entry(finalQC).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("FinalQC_B");
            }
            return View(finalQC);
        }

        public async Task<ActionResult> FinalQC_F1(FinalQC finalQC, string nuoOrder = null, string nuoBarCode = null, string isnuo = null)
        {
            JObject result = new JObject();
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
                finalQC.OldOrderNum = finalQC.OrderNum;
                finalQC.OldBarCodesNum = finalQC.BarCodesNum;
                db.Entry(finalQC).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //添加关联表
                if (!string.IsNullOrEmpty(nuoBarCode) && isnuo == "true")
                {
                    BarCodeRelation barcoderelation = new BarCodeRelation() { OldOrderNum = nuoOrder, OldBarCodeNum = nuoBarCode, NewBarCodesNum = finalQC.BarCodesNum, NewOrderNum = finalQC.OrderNum, Procedure = "FQC", UsserID = ((Users)Session["User"]).UserNum, CreateDate = DateTime.Now };
                    if (!comm.InsertRelation(barcoderelation))
                    {
                        result.Add("mes", "挪用订单失败，此条码已挪用过库存条码");
                        result.Add("pass", false);
                        return Content(JsonConvert.SerializeObject(result));
                    }
                    #region 挪用修改原条码工序记录
                    com.NuoOperation(nuoBarCode, nuoOrder, finalQC.BarCodesNum, finalQC.OrderNum);
                    #endregion
                }

                result.Add("mes", "成功");
                result.Add("pass", true);
                return Content(JsonConvert.SerializeObject(result));
            }
            result.Add("mes", "失败");
            result.Add("pass", false);
            return Content(JsonConvert.SerializeObject(result));

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

        private ActionResult GetOrderList1()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //----------------------------------------------------------------------------------------
        #endregion


        #region ----------其他方法
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


        //客户订单
        //扫码记录
        public ActionResult AddCustomer_Order(string ordernum, string barcode)
        {
            JObject result = new JObject();
            var barcodelist = db.Customer_Order.Where(c => c.OrderNum == ordernum).Select(c => c.BarCodesNum).ToList();
            if (barcodelist.Contains(barcode))
            {
                result.Add("mes", "条码重复");
                result.Add("pass", false);
                result.Add("ordernum", ordernum);
                JArray barcodearray = new JArray();
                barcodelist.ForEach(c => barcodearray.Add(c));
                result.Add("barcode", barcodearray);//数组
                result.Add("Count", barcodearray.Count);//数量
                return Content(JsonConvert.SerializeObject(result));

            }
            if (db.BarCodes.Where(c => c.BarCodesNum == barcode).Select(c => c.OrderNum).FirstOrDefault() != ordernum)
            {
                result.Add("mes", "条码与订单不符合");
                result.Add("pass", false);
                result.Add("ordernum", ordernum);
                JArray barcodearray = new JArray();
                barcodelist.ForEach(c => barcodearray.Add(c));
                result.Add("barcode", barcodearray);//数组
                result.Add("Count", barcodearray.Count);//数量
                return Content(JsonConvert.SerializeObject(result));
            }
            else
            {
                Customer_Order order = new Customer_Order() { OrderNum = ordernum, BarCodesNum = barcode, FQCPrincipal = (Users)Session["User"] == null ? "没有账号" : ((Users)Session["User"]).UserName, FQCCheckBT = DateTime.Now };
                db.Customer_Order.Add(order);
                db.SaveChanges();
                result.Add("mes", "新增成功");
                result.Add("pass", true);
                barcodelist = db.Customer_Order.Where(c => c.OrderNum == ordernum).OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();//新增的
                result.Add("ordernum", ordernum);
                JArray barcodearray = new JArray();
                barcodelist.ForEach(c => barcodearray.Add(c));
                result.Add("barcode", barcodearray);//数组
                result.Add("Count", barcodearray.Count);//数组
                return Content(JsonConvert.SerializeObject(result));
            }
        }

        //显示
        public ActionResult DisplayCustomer_Order(string ordernum)
        {
            JObject result = new JObject();

            var barcodelist = db.Customer_Order.Where(c => c.OrderNum == ordernum).OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();//新增的
            result.Add("ordernum", ordernum);
            JArray barcodearray = new JArray();
            barcodelist.ForEach(c => barcodearray.Add(c));
            result.Add("barcode", barcodearray);//数组
            result.Add("Count", barcodearray.Count);//数组
            result.Add("QTY", db.OrderMgm.Where(c => c.OrderNum == ordernum).FirstOrDefault().Models);//订单模块总数
            return Content(JsonConvert.SerializeObject(result));
        }
    }


    public class FinalQCs_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController com = new CommonController();
        private Common_ApiController comapi = new Common_ApiController();

        //首页
        [HttpPost]
        [ApiAuthorize]
        public JObject NewIndex([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();
            var info = db.FinalQC.Where(c => c.OrderNum == ordernum && (c.OldOrderNum == null || c.OldOrderNum == ordernum)).Select(c => new CommonController.TempIndex
            {
                BarCodesNum = c.BarCodesNum,
                EndTime = c.FQCCheckFT,
                Finish = c.FQCCheckFinish,
                StarTime = c.FQCCheckBT,
                OrderNum = c.OrderNum,
                Group = c.Group,
                Principal = c.FQCPrincipal,
                Abnormal = c.FinalQC_FQCCheckAbnormal,
                Repetition = c.RepetitionFQCCheck,
                RepetitionCause = c.RepetitionFQCCheckCause,
                id = c.Id

            }).ToList();

            return com.GetModuleFromJobjet(com.GeneralIndex(ordernum, info));
        }


        #region ----------FinalQC_B方法
        [HttpPost]
        [ApiAuthorize]
        public JObject FinalQC_B([System.Web.Http.FromBody]JObject data)
        {
            string BarCodesNum = data["BarCodesNum"].ToString();
            string OrderNum = data["OrderNum"].ToString();
            string UserName = data["UserName"].ToString();
            string Department = data["Department"].ToString();
            string Group = data["Group"].ToString();
            bool RepetitionFQCCheck = bool.Parse(data["RepetitionFQCCheck"].ToString());
            string RepetitionFQCCheckCause = data["RepetitionFQCCheckCause"].ToString();

            JObject result = new JObject();
            //创建一个集合存放此条码对应的记录
            var PQC_recordlist = db.Assemble.Where(c => c.BoxBarCode == BarCodesNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodesNum)&&c.PQCCheckFinish==true);

            //检查条码和订单是否准确
            var barcodeCheck = comapi.CheckBarcode(OrderNum, BarCodesNum);
            if (barcodeCheck != "true")
            {
                return com.GetModuleFromJobjet(result,false, barcodeCheck);
            }
            //3.判断PQC是否已经完成,如果PQC已完成，允许进行FQC
            else if (PQC_recordlist.Count() > 0)
            {
                var startStatu = comapi.CheckSectionStart(BarCodesNum, "FinalQCs", "BarCodesNum", "FQCCheckFT", "FQCCheckFinish");
                if (RepetitionFQCCheck == false)
                {
                    if (startStatu == "新增")
                    {
                        FinalQC finalQC = new FinalQC() { OrderNum = OrderNum, BarCodesNum = BarCodesNum, FQCCheckBT = DateTime.Now, FQCPrincipal = UserName, Department1 = Department, Group = Group, OldBarCodesNum = BarCodesNum, OldOrderNum = OrderNum };
                        db.FinalQC.Add(finalQC);
                        db.SaveChangesAsync();
                        result.Add("id", finalQC.Id);
                        result.Add("startTime", finalQC.FQCCheckBT);
                        return com.GetModuleFromJobjet(result,true, "成功");

                    }
                    else if (startStatu == "修改")
                    {
                        var info = db.FinalQC.Where(c => c.BarCodesNum == BarCodesNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodesNum) && c.FQCCheckBT != null && c.FQCCheckFT == null).Select(c => new { c.Id, c.FQCCheckBT }).FirstOrDefault();
                        result.Add("id", info.Id);
                        result.Add("startTime", info.FQCCheckBT);
                        return com.GetModuleFromJobjet(result,true, "成功");
                    }
                    else if (startStatu == "重复")
                    {
                        result.Add("id", null);
                        result.Add("startTime", null);
                        return com.GetModuleFromJobjet(result,false, "此模组已完成FQC工作，只能进行“重复”FQC工作，请勾上“重复”FQC选项勾！");
                    }
                }
                else
                {
                    if (startStatu == "重复")
                    {
                        FinalQC finalQC = new FinalQC() { OrderNum = OrderNum, BarCodesNum = BarCodesNum, FQCCheckBT = DateTime.Now, FQCPrincipal = UserName, Department1 = Department, Group = Group, OldBarCodesNum = BarCodesNum, OldOrderNum = OrderNum, RepetitionFQCCheck = true, RepetitionFQCCheckCause = RepetitionFQCCheckCause };
                        db.FinalQC.Add(finalQC);
                        db.SaveChangesAsync();
                        result.Add("id", finalQC.Id);
                        result.Add("startTime", finalQC.FQCCheckBT);
                        return com.GetModuleFromJobjet(result,true, "成功");
                    }
                    else if (startStatu == "新增" || startStatu == "修改")
                    {
                        result.Add("id", null);
                        result.Add("startTime", null);
                        return com.GetModuleFromJobjet(result,false, "此模组未完成FQC工作，不能进行“重复”FQC工作，请取消“重复”FQC选项勾！");
                    }
                }
            }
            else
            {
                result.Add("id", null);
                result.Add("startTime", null);
                return com.GetModuleFromJobjet(result,false, "条码未进行PQC,请先完成PQC操作");
            }
            result.Add("id", null);
            result.Add("startTime", null);
            return com.GetModuleFromJobjet(result,false, "逻辑错误");
        }
        /*
        public JObject FinalQC_B([System.Web.Http.FromBody]JObject data)
        {
            string BarCodesNum = data["BarCodesNum"].ToString();
            string OrderNum = data["OrderNum"].ToString();
            string UserName = data["UserName"].ToString();
            string Department = data["Department"].ToString();
            string Group = data["Group"].ToString();
            bool RepetitionFQCCheck =bool.Parse( data["RepetitionFQCCheck"].ToString());
            string RepetitionFQCCheckCause =  data["RepetitionFQCCheckCause"].ToString();


            JObject result = new JObject();
            //创建一个集合存放此条码对应的记录
            var PQC_recordlist = db.Assemble.Where(c => c.BoxBarCode == BarCodesNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodesNum));
            //1.判断条码是否存在
            //var aaa = db.BarCodes.Where(c => c.BarCodesNum == finalQC.BarCodesNum).Count();
            if (db.BarCodes.Where(c => c.BarCodesNum == BarCodesNum&&c.BarCodeType=="模组").Count() == 0)
            {
                result.Add("mes", "模组条码号不存在，请检查模组条码输入是否正确！");
                result.Add("pass", false);
                result.Add("id", null);
                result.Add("startTime", null);
                return com.GetModuleFromJobjet(result);
            }
            //2.判断条码跟订单号是否相符
            else if (OrderNum != db.BarCodes.Where(c => c.BarCodesNum == BarCodesNum).FirstOrDefault().OrderNum)
            {
                result.Add("mes", "条码号不属于" + OrderNum + "订单，应该属于" + db.BarCodes.Where(c => c.BarCodesNum == BarCodesNum).FirstOrDefault().OrderNum + "订单！");
                result.Add("pass", false);
                result.Add("id", null);
                result.Add("startTime", null);
                return com.GetModuleFromJobjet(result);
            }

            //3.判断PQC是否已经完成,如果PQC已完成，允许进行FQC
            else if (PQC_recordlist.Count() > 0)
            {
                //统计此条码在FinalQC表中的记录个数
                var fqc_record_count = db.FinalQC.Where(c => c.BarCodesNum == BarCodesNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodesNum)).ToList();

                if (fqc_record_count.Count > 0)
                {
                    //(1).有不完整的记录（有开始时间没有完成时间）
                    if (fqc_record_count.Where(c => c.FQCCheckBT != null && c.FQCCheckFT == null).Count() > 0)
                    {
                        var info = db.FinalQC.Where(c => c.BarCodesNum == BarCodesNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodesNum) && c.FQCCheckBT != null && c.FQCCheckFT == null).Select(c => new { c.Id, c.FQCCheckBT }).FirstOrDefault();
                        result.Add("mes", "成功");
                        result.Add("pass", true);
                        result.Add("id", info.Id);
                        result.Add("startTime", info.FQCCheckBT);
                        return com.GetModuleFromJobjet(result);
                    }

                    //(2).有记录，FQC异常，未完成FQC，创建一条新记录（不允许勾选“重复FQC”）
                    else if (fqc_record_count.Where(c => c.FQCCheckFinish == true).Count() == 0)
                    {
                        if (RepetitionFQCCheck == false)  //重复FQC要求不能勾上
                        {
                            FinalQC finalQC = new FinalQC() { OrderNum = OrderNum, BarCodesNum = BarCodesNum, FQCCheckBT = DateTime.Now, FQCPrincipal = UserName, Department1 = Department, Group = Group, OldBarCodesNum = BarCodesNum, OldOrderNum = OrderNum };

                            if (ModelState.IsValid)
                            {
                                db.FinalQC.Add(finalQC);
                                db.SaveChangesAsync();
                                result.Add("mes", "成功");
                                result.Add("pass", true);
                                var id = finalQC.Id.ToString();
                                var starttime = db.FinalQC.Where(c => c.Id == finalQC.Id).Select(c => c.FQCCheckBT).FirstOrDefault();
                                result.Add("id", finalQC.Id);
                                result.Add("startTime", starttime);
                                return com.GetModuleFromJobjet(result);
                            }
                        }
                        else  //重复FQC未勾上，提示错误
                        {
                            result.Add("mes", "此模组未完成FQC工作，不能进行“重复”FQC工作，请取消“重复”FQC选项勾！");
                            result.Add("pass", false);
                            result.Add("id",null);
                            result.Add("startTime", null);
                            return com.GetModuleFromJobjet(result);

                        }
                    }

                    //(3).检查是否已经有首次FinalQC完成，如果首次FQC已完成，进入重复FQC
                    else  //首次FQC已完成，应该进入重复FQC
                    {
                        if (RepetitionFQCCheck == true)  //重复FQC已经勾上
                        {
                            FinalQC finalQC = new FinalQC() { OrderNum = OrderNum, BarCodesNum = BarCodesNum, FQCCheckBT = DateTime.Now, FQCPrincipal = UserName, Department1 = Department, Group = Group, OldBarCodesNum = BarCodesNum, OldOrderNum = OrderNum,RepetitionFQCCheck=true,RepetitionFQCCheckCause= RepetitionFQCCheckCause };

                            if (ModelState.IsValid)
                            {
                                db.FinalQC.Add(finalQC);
                                db.SaveChangesAsync();
                                result.Add("mes", "成功");
                                result.Add("pass", true);
                                var id = finalQC.Id.ToString();
                                var starttime = db.FinalQC.Where(c => c.Id == finalQC.Id).Select(c => c.FQCCheckBT).FirstOrDefault();
                                result.Add("id", finalQC.Id);
                                result.Add("startTime", starttime);
                                result.Add("id", null);
                                result.Add("startTime", null);
                                return com.GetModuleFromJobjet(result);
                            }
                        }
                        else  //重复FQC未勾上，提示错误
                        {
                            result.Add("mes", "此模组已完成FQC工作，只能进行“重复”FQC工作，请勾上“重复”FQC选项勾！");
                            result.Add("pass", false);
                            result.Add("id", null);
                            result.Add("startTime", null);
                            return com.GetModuleFromJobjet(result);
                        }
                    }
                }
                else //4.无记录，直接创建
                {
                    if (RepetitionFQCCheck)
                    {
                        result.Add("mes", "模组未从未进行过FQC工作，不能进行“重复”FQC工作，不能勾选“重复”FQC选项！");
                        result.Add("pass", false);
                        result.Add("id", null);
                        result.Add("startTime", null);
                        return com.GetModuleFromJobjet(result);
                    }
                    FinalQC finalQC = new FinalQC() { OrderNum = OrderNum, BarCodesNum = BarCodesNum, FQCCheckBT = DateTime.Now, FQCPrincipal = UserName, Department1 = Department, Group = Group, OldBarCodesNum = BarCodesNum, OldOrderNum = OrderNum };

                    if (ModelState.IsValid)
                    {
                        db.FinalQC.Add(finalQC);
                        db.SaveChangesAsync();
                        result.Add("mes", "成功");
                        result.Add("pass", true);
                        var id = finalQC.Id.ToString();
                        var starttime = db.FinalQC.Where(c => c.Id == finalQC.Id).Select(c => c.FQCCheckBT).FirstOrDefault();
                        result.Add("id", finalQC.Id);
                        result.Add("startTime", starttime);
                        return com.GetModuleFromJobjet(result);
                    }
                    //ModelState.AddModelError("", finalQC.BarCodesNum + "已完成PQC" );
                    //return View(finalQC);
                }
            }
            else
            {
                result.Add("mes", "条码未进行PQC,请先完成PQC操作");
                result.Add("pass", false);
                result.Add("id", null);
                result.Add("startTime", null);
                return com.GetModuleFromJobjet(result);
            }
            result.Add("mes", "逻辑错误");
            result.Add("pass", false);
            result.Add("id", null);
            result.Add("startTime", null);
            return com.GetModuleFromJobjet(result);
        }
        */
        #endregion


        #region ----------FinalQC_F方法
        [HttpPost]
        [ApiAuthorize]
        public JObject FinalQC_F([System.Web.Http.FromBody]JObject data)
        {
            /*
             * FinalQC finalQC, string nuoOrder = null, string nuoBarCode = null, string isnuo = null, int userNum = 0
             */
            int id = int.Parse(data["Id"].ToString());
            string nuoOrder = data["nuoOrder"].ToString();
            string nuoBarCode = data["nuoBarCode"].ToString();
            string isnuo = data["isnuo"].ToString();
            int userNum = int.Parse(data["userNum"].ToString());
            string FinalQC_FQCCheckAbnormal = data["FinalQC_FQCCheckAbnormal"].ToString();
            string Remark = data["Remark"].ToString();

            var finalQC = db.FinalQC.Find(id);
            JObject result = new JObject();
            //获取完成时间
            finalQC.FQCCheckFT = DateTime.Now;
            //计算时长
            JObject time = comapi.CalculateTimespan(finalQC.FQCCheckBT.Value, finalQC.FQCCheckFT.Value);
            finalQC.FQCCheckTime = TimeSpan.Parse(time["Time"].ToString());
            finalQC.FQCCheckDate = int.Parse(time["Date"].ToString());
            finalQC.FQCCheckTimeSpan = time["TimeSpan"].ToString();
           
            finalQC.FinalQC_FQCCheckAbnormal = FinalQC_FQCCheckAbnormal;
            finalQC.Remark = Remark;
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
                finalQC.OldOrderNum = finalQC.OrderNum;
                finalQC.OldBarCodesNum = finalQC.BarCodesNum;
                db.Entry(finalQC).State = EntityState.Modified;
                db.SaveChangesAsync();
                //添加关联表
                if (!string.IsNullOrEmpty(nuoBarCode) && isnuo == "true")
                {
                    BarCodeRelation barcoderelation = new BarCodeRelation() { OldOrderNum = nuoOrder, OldBarCodeNum = nuoBarCode, NewBarCodesNum = finalQC.BarCodesNum, NewOrderNum = finalQC.OrderNum, Procedure = "FQC", UsserID = userNum, CreateDate = DateTime.Now };
                    if (!comm.InsertRelation(barcoderelation))
                    {
                        return com.GetModuleFromJobjet(result,false, "挪用订单失败，此条码已挪用过库存条码");
                    }
                    #region 挪用修改原条码工序记录
                    com.NuoOperation(nuoBarCode, nuoOrder, finalQC.BarCodesNum, finalQC.OrderNum);
                    #endregion
                }
                return com.GetModuleFromJobjet(result,true, "成功");
            }
            return com.GetModuleFromJobjet(result,false, "失败");

        }
        #endregion

        [HttpPost]
        [ApiAuthorize]
        public JObject CheckFinish([System.Web.Http.FromBody]JObject data)
        {
            string barcode = data["barcode"].ToString();
            JObject result = new JObject();
            var fqc = db.FinalQC.Where(c => c.BarCodesNum == barcode && c.FQCCheckFinish == true && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).FirstOrDefault();
            if (fqc != null)
            {
                return com.GetModuleFromJobjet(result,true);
            }
            else
            {
                return com.GetModuleFromJobjet(result,false);
            }

        }

        //客户订单
        //扫码记录
        [HttpPost]
        [ApiAuthorize]
        public JObject AddCustomer_Order([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();
            string barcode = data["barcode"].ToString();
            string UserName = data["UserName"].ToString();

            JObject result = new JObject();
            var barcodelist = db.Customer_Order.Where(c => c.OrderNum == ordernum).Select(c => c.BarCodesNum).ToList();
            if (barcodelist.Contains(barcode))
            {
                result.Add("ordernum", ordernum);
                JArray barcodearray = new JArray();
                barcodelist.ForEach(c => barcodearray.Add(c));
                result.Add("barcode", barcodearray);//数组
                result.Add("Count", barcodearray.Count);//数量
                return com.GetModuleFromJobjet(result,false, "条码重复");

            }
            if (db.BarCodes.Where(c => c.BarCodesNum == barcode).Select(c => c.OrderNum).FirstOrDefault() != ordernum)
            {
                result.Add("ordernum", ordernum);
                JArray barcodearray = new JArray();
                barcodelist.ForEach(c => barcodearray.Add(c));
                result.Add("barcode", barcodearray);//数组
                result.Add("Count", barcodearray.Count);//数量
                return com.GetModuleFromJobjet(result,false, "条码与订单不符合");
            }
            else
            {
                Customer_Order order = new Customer_Order() { OrderNum = ordernum, BarCodesNum = barcode, FQCPrincipal = UserName, FQCCheckBT = DateTime.Now };
                db.Customer_Order.Add(order);
                db.SaveChanges();
                barcodelist = db.Customer_Order.Where(c => c.OrderNum == ordernum).OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();//新增的
                result.Add("ordernum", ordernum);
                JArray barcodearray = new JArray();
                barcodelist.ForEach(c => barcodearray.Add(c));
                result.Add("barcode", barcodearray);//数组
                result.Add("Count", barcodearray.Count);//数组
                return com.GetModuleFromJobjet(result,true, "新增成功");
            }
        }

        //显示
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayCustomer_Order([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();
            JObject result = new JObject();

            var barcodelist = db.Customer_Order.Where(c => c.OrderNum == ordernum).OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();//新增的
            result.Add("ordernum", ordernum);
            JArray barcodearray = new JArray();
            barcodelist.ForEach(c => barcodearray.Add(c));
            result.Add("barcode", barcodearray);//数组
            result.Add("Count", barcodearray.Count);//数组
            result.Add("QTY", db.OrderMgm.Where(c => c.OrderNum == ordernum).FirstOrDefault().Models);//订单模块总数
            return com.GetModuleFromJobjet(result);
        }
    }
}

