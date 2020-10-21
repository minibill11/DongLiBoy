﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.AuthAttributes;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebGrease.Css.Extensions;

namespace JianHeMES.Controllers
{
    public class AssemblesController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        public Assemble assemble = null;
        private CommonalityController comm = new CommonalityController();
        private CommonController common = new CommonController();


        #region --------------------PQC异常列表

        //private List<SelectListItem> SetAbnormalList()
        //{
        //    return new List<SelectListItem>()
        //    {
        //        new SelectListItem{   Text = "请选择维修情况", Value = ""  },
        //        new SelectListItem{   Text = "正常", Value = "1"  },
        //        new SelectListItem{   Text = "箱体表面", Value = "2"  },
        //        new SelectListItem{   Text = "丝印LOGO", Value = "3"  },
        //        new SelectListItem{   Text = "箱门/门锁", Value = "4"  },
        //        new SelectListItem{   Text = "面罩", Value = "5"  },
        //        new SelectListItem{   Text = "灯管", Value = "6"  },
        //        new SelectListItem{   Text = "指示灯/按钮/显示模块", Value = "7"  },
        //        new SelectListItem{   Text = "锁扣", Value = "8"  },
        //        new SelectListItem{   Text = "把手/提手", Value = "9"  },
        //        new SelectListItem{   Text = "航插座/防水索头", Value = "10"  },
        //        new SelectListItem{   Text = "模块", Value = "11"  },
        //        new SelectListItem{   Text = "红胶/三防漆", Value = "12"  },
        //        new SelectListItem{   Text = "线材/接扎线方式", Value = "13"  },
        //        new SelectListItem{   Text = "风扇/温控断路器/滤波器", Value = "14"  },
        //        new SelectListItem{   Text = "系统卡/功能卡", Value = "15"  },
        //        new SelectListItem{   Text = "电源/保护盖接线座/防尘网", Value = "16"  },
        //        new SelectListItem{   Text = "接地", Value = "17"  },
        //        new SelectListItem{   Text = "胶条", Value = "18"  },
        //        new SelectListItem{   Text = "箱内外观", Value = "19"  },
        //        new SelectListItem{   Text = "标识", Value = "20"  },
        //        new SelectListItem{   Text = "螺丝/压铆", Value = "21"  },
        //        new SelectListItem{   Text = "刮痕/掉漆喷漆/沙眼", Value = "22"  },
        //        new SelectListItem{   Text = "间隙/错位/高低不平", Value = "23"  },
        //        new SelectListItem{   Text = "安装孔", Value = "24"  },
        //        new SelectListItem{   Text = "插销/弹柱/定位柱", Value = "25"  },
        //        new SelectListItem{   Text = "各组件间无干涉", Value = "26"  },
        //        new SelectListItem{   Text = "模组对角平整缝隙", Value = "27"  },
        //        new SelectListItem{   Text = "拼装测试", Value = "28"  },
        //        new SelectListItem{   Text = "单色和全彩测试", Value = "29"  },
        //        new SelectListItem{   Text = "扫描检验", Value = "30"  },
        //        new SelectListItem{   Text = "单灯显示效果", Value = "31"  },
        //        new SelectListItem{   Text = "亮暗线", Value = "32"  },
        //        new SelectListItem{   Text = "指示灯/自检/LCD模块", Value = "33"  },
        //        new SelectListItem{   Text = "黑屏检验", Value = "34"  },
        //        new SelectListItem{   Text = "产品能效功率测试", Value = "35"  },
        //        new SelectListItem{   Text = "防尘", Value = "36"  },
        //        new SelectListItem{   Text = "防水", Value = "37"  }
        //    };
        //}




        #endregion

        #region --------------------PQCNormal列表
        private List<SelectListItem> PQCNormalList()
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

        #region --------------------维修列表

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

        public ActionResult New_Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Assembles", act = "New_Index" });
            }
            return View();
        }
        public ActionResult New_Assemble()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Assembles", act = "New_Assemble" });
            }
            return View();
        }
        #region --------------------PQC工段
        /// <summary>
        /// PQC工段
        /// </summary>
        // TODO : 增加电源和转接卡条码号存储功能
        public ActionResult PQCCheckB()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Assembles", act = "PQCCheckB" });
            }
            CommonController com = new CommonController();
            //if (((Users)Session["User"]).Role == "组装PQC" || com.isCheckRole("PQC管理", "PQC检查", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            return View();
            //}
            //return RedirectToAction("AssembleIndex");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PQCCheckB([Bind(Include = "Id,OrderNum,BarCode_Prefix,BoxBarCode,PQCCheckBT,AssemblePQCPrincipal,RepetitionPQCCheck,RepetitionPQCCheckCause,Remark,Department1,Group")] Assemble assemble, string nuoOrder = null, string nuoBarCode = null, string isnuo = null)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Assembles", act = "PQCCheckB" });
            }
            ////验证此条码是否已经完成PQC
            //int finisthcount = db.Assemble.Where(c => c.BoxBarCode == assemble.BoxBarCode && c.PQCCheckAbnormal == "正常" && c.RepetitionPQCCheck == false).Count();
            ////2.未完成，“重复”打钩，因为没有已完成PQC记录，提示不能打钩
            //if (finisthcount == 0 && assemble.RepetitionPQCCheck == true)
            //{
            //    ModelState.AddModelError("", "该模组条码未完成PQC,不能进行重复PQC打钩！");
            //    return View(assemble);
            //}

            ////3.已完成PQC,，“重复”不打钩，提示“重复”要打钩
            //if (finisthcount > 0 && assemble.RepetitionPQCCheck == false)
            //{
            //    ModelState.AddModelError("", "该模组条码可能未完成或重复PQC未打钩！");
            //    return View(assemble);
            //}
            ////1.未完成，“重复”不打钩，使用原来的方法操作,和4.已完成，“重复”打钩,使用原来的方法操作，再加上对RepetitioPQCCheck修改为true,RepetitionPQCCheckCause增加内容
            ////if (finisthcount==0 && assemble.RepetitionPQCCheck == false || finisthcount>0 && assemble.RepetitionPQCCheck == true)
            ////{
            ////}
            ////1.未完成，“重复”不打钩，使用原来的方法操作
            ////2.未完成，“重复”打钩，因为没有已完成PQC记录，提示不能打钩
            ////3.已完成，“重复”不打钩，提示“重复”要打钩
            ////4.已完成，“重复”打钩,使用原来的方法操作，再加上对RepetitioPQCCheck修改为true,RepetitionPQCCheckCause增加内容

            //在BarCodes条码表中找不到此条码号
            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == assemble.BoxBarCode) == null)
            {
                ModelState.AddModelError("", "模组条码不存在，请检查条码输入是否正确！");
                return View(assemble);
            }
            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == assemble.BoxBarCode).OrderNum != assemble.OrderNum)
            {
                ModelState.AddModelError("", "该模组条码不属于所选订单，请选择正确的订单号！");
                return View(assemble);
            }
            //在BarCodes条码表中找到此条码号
            //在Assembles组装记录表中找不到对应BoxBarCode的记录，准备在Assembles组装记录表中新建记录，包括OrderNum、BoxBarCode、PQCCheckBT、AssemblePQCPrincipal
            if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode && (u.OldBarCodesNum == assemble.BoxBarCode || u.OldBarCodesNum == null)) == null)
            {
                if (assemble.OrderNum == db.BarCodes.Where(u => u.BarCodesNum == assemble.BoxBarCode).FirstOrDefault().OrderNum)
                {
                    //var assembleRecord = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);
                    assemble.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == assemble.BoxBarCode).FirstOrDefault().OrderNum;
                    assemble.BarCode_Prefix = db.BarCodes.Where(u => u.BarCodesNum == assemble.BoxBarCode).FirstOrDefault().BarCode_Prefix;
                    assemble.PQCCheckBT = DateTime.Now;
                    assemble.OldBarCodesNum = assemble.BoxBarCode;
                    assemble.OldOrderNum = assemble.OrderNum;
                    assemble.AssemblePQCPrincipal = ((Users)Session["User"]).UserName;
                    db.Assemble.Add(assemble);
                    db.SaveChanges();
                    //添加关联表
                    if (!string.IsNullOrEmpty(nuoBarCode) && isnuo == "true")
                    {
                        BarCodeRelation barcoderelation = new BarCodeRelation() { OldOrderNum = nuoOrder, OldBarCodeNum = nuoBarCode, NewBarCodesNum = assemble.BoxBarCode, NewOrderNum = assemble.OrderNum, Procedure = "PQC", UsserID = ((Users)Session["User"]).UserNum, CreateDate = DateTime.Now };
                        if (!comm.InsertRelation(barcoderelation))
                        {
                            ModelState.AddModelError("", "挪用订单失败，此条码已挪用过库存条码");
                            return View(assemble);
                        }
                        #region 挪用修改原条码工序记录
                        common.NuoOperation(nuoBarCode, nuoOrder, assemble.BoxBarCode, assemble.OrderNum);
                        #endregion
                    }
                    return RedirectToAction("PQCCheckF", new { assemble.Id });
                }
                else
                {
                    ModelState.AddModelError("", "该模组条码不属于所选订单，请选择正确的订单号！");
                    return View(assemble);
                }
            }
            //在Assembles组装记录表中找到对应BoxBarCode的记录，如果记录中没有完成的，准备在Assembles组装记录表中新建记录，如果有正常记录将提示不能重复进行QC
            else if (db.Assemble.Count(u => u.BoxBarCode == assemble.BoxBarCode && (u.OldBarCodesNum == assemble.BoxBarCode || u.OldBarCodesNum == null)) >= 1)
            {
                var assemblelist = db.Assemble.Where(m => m.BoxBarCode == assemble.BoxBarCode && (m.OldBarCodesNum == assemble.BoxBarCode || m.OldBarCodesNum == null)).ToList();
                int finishCount = assemblelist.Where(m => m.PQCCheckFinish == true).Count();
                if (finishCount == 0)
                {
                    foreach (var item in assemblelist)
                    {
                        if (item.PQCCheckBT != null && item.PQCCheckFT == null)  //如果只有开始时间，没有结束时间，把此记录调出来
                        {
                            assemble = item;
                            return RedirectToAction("PQCCheckF", new { assemble.Id });
                        }
                    }
                    if (assemble.OrderNum == db.BarCodes.Where(u => u.BarCodesNum == assemble.BoxBarCode).FirstOrDefault().OrderNum)
                    {
                        assemble.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == assemble.BoxBarCode).FirstOrDefault().OrderNum;
                        assemble.PQCCheckBT = DateTime.Now;
                        assemble.AssemblePQCPrincipal = ((Users)Session["User"]).UserName;
                        assemble.OldBarCodesNum = assemble.BoxBarCode;
                        assemble.OldOrderNum = assemble.OrderNum;
                        db.Assemble.Add(assemble);
                        db.SaveChanges();
                        //添加关联表
                        if (!string.IsNullOrEmpty(nuoBarCode) && isnuo == "true")
                        {
                            BarCodeRelation barcoderelation = new BarCodeRelation() { OldOrderNum = nuoOrder, OldBarCodeNum = nuoBarCode, NewBarCodesNum = assemble.BoxBarCode, NewOrderNum = assemble.OrderNum, Procedure = "PQC", UsserID = ((Users)Session["User"]).UserNum, CreateDate = DateTime.Now };
                            if (!comm.InsertRelation(barcoderelation))
                            {
                                ModelState.AddModelError("", "挪用订单失败，此条码已挪用过库存条码");
                                return View(assemble);
                            }
                            #region 挪用修改原条码工序记录
                            common.NuoOperation(nuoBarCode, nuoOrder, assemble.BoxBarCode, assemble.OrderNum);
                            #endregion
                        }
                        return RedirectToAction("PQCCheckF", new { assemble.Id });
                    }
                    else
                    {
                        ModelState.AddModelError("", "该模组条码不属于所选订单，请选择正确的订单号！");
                        return View(assemble);
                    }

                }
                else
                {
                    //return Content("<script>alert('此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！');window.location.href='../Assembles/AssembleIndex';</script>");
                    ModelState.AddModelError("", "此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！");
                    return View(assemble);
                }
            }
            else
            {
                return RedirectToAction("AssembleIndex");
            }


        }

        public ActionResult PQCCheckF(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Assembles", act = "PQCCheckF" + "/" + id.ToString() });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assemble assemble = db.Assemble.Find(id);
            if (assemble == null)
            {
                return HttpNotFound();
            }
            //添加库存条码信息
            var nuobarcode = db.BarCodeRelation.Where(c => c.NewBarCodesNum == assemble.BoxBarCode && c.NewOrderNum == assemble.OrderNum).Select(c => c.OldBarCodeNum).FirstOrDefault();
            if (nuobarcode == null)
                ViewBag.nuoBarcode = "";
            else
                ViewBag.nuoBarcode = nuobarcode;
            //ViewBag.AbnormalList = SetAbnormalList();
            ViewBag.RepairList = SetRepairList();
            return View(assemble);
        }

        [HttpPost]

        public ActionResult PQCCheckF([Bind(Include = "Id,OrderNum,BarCode_Prefix,BoxBarCode,BarCode_Prefix,AssembleBT,AssembleFT,AssembleTime,AssembleFinish,WaterproofTestBT,WaterproofTestFT,WaterproofTestTimeSpan,WaterproofAbnormal,WaterproofMaintaince,WaterproofTestFinish,AssembleAdapterCardBT,AssembleAdapterCardFT,AssembleAdapterTime,AssembleAdapterFinish,ViewCheckBT,ViewCheckFT,ViewCheckTime,ViewCheckAbnormal,ViewCheckFinish,ElectricityCheckBT,ElectricityCheckFT,ElectricityCheckTime,ElectricityCheckAbnormal,ElectricityCheckFinish,PQCCheckBT,AssemblePQCPrincipal,AssembleLineId,PQCCheckFT,PQCCheckTime,PQCCheckAbnormal,PQCRepairCondition,PQCCheckFinish,RepetitionPQCCheck,RepetitionPQCCheckCause,Remark,Department1,Group")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Assembles", act = "PQCCheckF" + "/" + assemble.Id.ToString() });
            }

            if (assemble.PQCCheckFT == null)
            {
                assemble.PQCCheckFT = DateTime.Now;
                assemble.PQCCheckTime = assemble.PQCCheckFT.Value.Subtract(assemble.PQCCheckBT.Value).Duration();
                assemble.OldBarCodesNum = assemble.BoxBarCode;
                assemble.OldOrderNum = assemble.OrderNum;
                if (assemble.PQCCheckTime.Value.Days > 0)  //时间和天数分开两个字段存储
                {
                    assemble.PQCCheckDate = assemble.PQCCheckTime.Value.Days;
                    assemble.PQCCheckTime = new TimeSpan(0, assemble.PQCCheckTime.Value.Hours, assemble.PQCCheckTime.Value.Minutes, assemble.PQCCheckTime.Value.Seconds);
                }
                assemble.AssembleLineId = Convert.ToInt16(Request["AssembleLineId"]);
                if (assemble.PQCCheckAbnormal == null)
                {
                    assemble.PQCCheckAbnormal = "正常";
                }
                if (assemble.PQCRepairCondition == null)
                {
                    assemble.PQCRepairCondition = "正常";
                }
                if (assemble.PQCRepairCondition == "转维修站" && assemble.PQCCheckAbnormal == "正常" || assemble.PQCRepairCondition == "现场维修" && assemble.PQCCheckAbnormal == "正常")
                {
                    ModelState.AddModelError("", "“转维修站”或“现场维修”时，异常情况不能为“正常”！");
                    return View(assemble);
                }
                Boolean abnormal = assemble.PQCCheckAbnormal == "正常" ? true : false;
                Boolean repair = assemble.PQCRepairCondition == "正常" ? true : false;
                if (abnormal && repair)
                {
                    assemble.PQCCheckFinish = true;
                }
                else assemble.PQCCheckFinish = false;
            }
            else
            {
                ModelState.AddModelError("", "记录已存在，不能更改！");
                return View(assemble);
            }

            if (ModelState.IsValid)
            {
                db.Entry(assemble).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PQCCheckB", "Assembles");
            }
            return View(assemble);
        }

        #endregion

        #region    --------------------首页

        public ActionResult AssembleIndex()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Assembles", act = "AssembleIndex" });
            }
            ViewBag.Display = "display:none";//隐藏View基本情况信息
            ViewBag.Legend = "display:none";//隐藏图例
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.PQCNormal = PQCNormalList();
            ViewBag.NotDo = null;
            return View();
        }



        [HttpPost]
        public ActionResult AssembleIndex(string orderNum, string BoxBarCode, string PQCNormal, string Remark/*, int PageIndex = 0*/)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Assembles", act = "AssembleIndex" });
            }
            List<Assemble> Allassembles = new List<Assemble>();
            List<Assemble> AllassemblesList = new List<Assemble>();
            if (String.IsNullOrEmpty(orderNum))
            {
                ////调出全部记录      
                Allassembles = db.Assemble.ToList();
            }
            else
            {
                //筛选出对应orderNum所有记录
                Allassembles = db.Assemble.Where(c => c.OrderNum == orderNum && (c.OldOrderNum == null || c.OldOrderNum == orderNum)).ToList();
                if (Allassembles.Count() == 0)
                {
                    var barcodelist = db.BarCodes.Where(c => c.ToOrderNum == orderNum).ToList();
                    foreach (var item in barcodelist)
                    {
                        Allassembles.AddRange(db.Assemble.Where(c => c.BoxBarCode == item.BarCodesNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == item.BarCodesNum)));
                    }
                }
            }

            //统计校正结果正常的模组数量
            var assemble_Normal_Count = Allassembles.Where(m => m.PQCCheckAbnormal == "正常").Count();
            var assemble_Abnormal_Count = Allassembles.Where(m => m.PQCCheckAbnormal != "正常").Count();

            #region  ---------按备注筛选--------------
            if (Remark != "")
            {
                Allassembles = Allassembles.Where(m => m.Remark != null && m.Remark.Contains(Remark)).ToList();
            }
            #endregion

            #region  ---------按条码筛选--------------
            if (BoxBarCode != "")
            {
                Allassembles = Allassembles.Where(x => x.BoxBarCode == BoxBarCode && (x.OldBarCodesNum == null || x.OldBarCodesNum == BoxBarCode)).ToList();
            }
            #endregion

            #region   ---------筛选正常、异常-------------
            //正常、异常记录筛选
            if (PQCNormal == "异常")
            {
                //Allassembles = from m in Allassembles where (m.PQCCheckAbnormal != "正常") select m;
                Allassembles = Allassembles.Where(c => c.PQCCheckAbnormal != "正常").ToList();
            }
            else if (PQCNormal == "正常")
            {
                //Allassembles = from m in Allassembles where (m.PQCCheckAbnormal == "正常") select m;
                Allassembles = Allassembles.Where(c => c.PQCCheckAbnormal == "正常").ToList();

            }
            #endregion

            #region   ----------筛选从未开始、未完成、正在进行PQC的条码清单------------
            List<BarCodes> BarCodesList = (from m in db.BarCodes where m.OrderNum == orderNum select m).ToList();
            //List<string> NotPQCList = new List<string>();
            List<string> NotPQCList = new List<string>();
            List<string> DoingNow = new List<string>();
            List<string> NeverFinishList = new List<string>();
            foreach (var barcode in BarCodesList)
            {
                if ((from m in db.Assemble where m.BoxBarCode == barcode.BarCodesNum && (m.OldBarCodesNum == null || m.OldBarCodesNum == BoxBarCode) select m).Count() == 0)
                {
                    NotPQCList.Add(barcode.BarCodesNum);
                }
            }
            ViewBag.NotDo = NotPQCList;//输出未进行PQC的条码清单
            int barcodeslistcount = NotPQCList.Count;//未进行PQC数量
            ViewBag.NotDoCount = barcodeslistcount;//未进行PQC数量

            foreach (var barcode in BarCodesList)
            {
                if (db.Assemble.Where(m => m.BoxBarCode == barcode.BarCodesNum && (m.OldBarCodesNum == null || m.OldBarCodesNum == BoxBarCode)).Where(m => m.PQCCheckBT != null && m.PQCCheckFT == null).Count() > 0)
                {
                    DoingNow.Add(barcode.BarCodesNum);
                }
            }
            ViewBag.DoingNowList = DoingNow;//正在做PQC的条码清单
            ViewBag.DoingNowListCount = DoingNow.Count();//正在做PQC的条码个数

            foreach (var barcode in BarCodesList)
            {
                if (Allassembles.Where(m => m.BoxBarCode == barcode.BarCodesNum && (m.OldBarCodesNum == null || m.OldBarCodesNum == BoxBarCode)).Count(c => c.PQCCheckFinish == true) == 0)
                {
                    NeverFinishList.Add(barcode.BarCodesNum);
                }
            }
            ViewBag.NeverFinishList = NeverFinishList.Except(NotPQCList).Except(DoingNow);//输出未完成PQC的条码清单
            ViewBag.NeverFinishListCount = NeverFinishList.Except(NotPQCList).Except(DoingNow).Count();//未完成PQC的条码数量

            #endregion

            //检查orderNum和searchString是否为空
            //if (!String.IsNullOrEmpty(searchString))
            //{   //从调出的记录中筛选含searchString内容的记录
            //    assembles = assembles.Where(s => s.AbnormalDescription.Contains(searchString));
            //}

            #region----------筛选出完成组装，超过24小时未进入老化工序的记录JSON------------
            var BarcodesListByOrderNum = Allassembles.Select(c => c.BoxBarCode).Distinct().ToList();//1和2
            JObject BorcodesOvertimeList = new JObject();//3
            var AllBurn_inRecordByOrderNum = db.Burn_in.Where(c => c.OrderNum == orderNum && (c.OldOrderNum == null || c.OldOrderNum == orderNum)).ToList();//4
            foreach (var item in BarcodesListByOrderNum)
            {
                //老化记录数
                int AllBurn_in_Records_Count = AllBurn_inRecordByOrderNum.Count(c => c.BarCodesNum == item);
                if (AllBurn_in_Records_Count == 0)
                {
                    var assembles_count = Allassembles.Count(c => c.BoxBarCode == item && c.PQCCheckFinish == true);
                    if (assembles_count >= 1)
                    {
                        var Aassembles_FT = Allassembles.Where(c => c.BoxBarCode == item && c.PQCCheckFinish == true).FirstOrDefault().PQCCheckFT;
                        var SubTime = DateTime.Now - Aassembles_FT;
                        if (Aassembles_FT != null && SubTime.Value.TotalMinutes > 1440) //1天：1*24*60
                        {
                            BorcodesOvertimeList.Add(item, "此模组在" + Aassembles_FT.ToString() + "完成组装,已经超过" + SubTime.Value.Days + "天" + SubTime.Value.Hours + "小时" + SubTime.Value.Minutes + "分未进入老化工序！");
                        }
                    }

                }
            }
            ViewBag.Overtime = BorcodesOvertimeList;
            #endregion


            #region   ----------计算总时长和平均时长------------
            //取出对应orderNum对应组装中PQC时长所有记录
            IQueryable<TimeSpan?> TimeSpanList = from m in db.Assemble
                                                 where (m.OrderNum == orderNum) && (m.OldOrderNum == null || m.OldOrderNum == orderNum)
                                                 //orderby m.PQCCheckTime
                                                 select m.PQCCheckTime;
            //计算校正总时长  TotalTimeSpan
            TimeSpan TotalTimeSpan = new TimeSpan();
            if (Allassembles.Where(x => x.PQCCheckAbnormal == "正常").Count() != 0)
            {
                foreach (var m in TimeSpanList)
                {
                    if (m != null)
                    {
                        TotalTimeSpan = TotalTimeSpan.Add(m.Value).Duration();
                    }
                }
                int days = 0;
                if (db.Assemble.Where(m => m.OrderNum == orderNum && (m.OldOrderNum == null || m.OldOrderNum == orderNum)).ToList().Sum(c => c.PQCCheckDate) > 0)
                {
                    days = db.Assemble.Where(m => m.OrderNum == orderNum && (m.OldOrderNum == null || m.OldOrderNum == orderNum)).ToList().Sum(c => c.PQCCheckDate);
                    TotalTimeSpan = new TimeSpan(TotalTimeSpan.Days + days, TotalTimeSpan.Hours, TotalTimeSpan.Minutes, TotalTimeSpan.Seconds);
                }

                if (TotalTimeSpan.Hours > 0)
                {
                    ViewBag.TotalTimeSpan = TotalTimeSpan.Hours.ToString() + "小时" + TotalTimeSpan.Minutes.ToString() + "分" + TotalTimeSpan.Seconds.ToString() + "秒";
                }
                else if (TotalTimeSpan.Hours == 0 && TotalTimeSpan.Minutes > 0)
                {
                    ViewBag.TotalTimeSpan = TotalTimeSpan.Minutes.ToString() + "分" + TotalTimeSpan.Seconds.ToString() + "秒";
                }
                else if (TotalTimeSpan.Hours == 0 && TotalTimeSpan.Minutes == 0 && TotalTimeSpan.Seconds > 0)
                {
                    ViewBag.TotalTimeSpan = TotalTimeSpan.Seconds.ToString() + "秒";
                }
                else
                {
                    ViewBag.TotalTimeSpan = "";
                }
            }
            else
            {
                ViewBag.TotalTimeSpan = "暂时没有已完成组装PQC的模组";
            }

            //计算平均用时  AvgTimeSpan
            //TimeSpan AvgTimeSpan = new TimeSpan();
            int Order_CR_valid_Count = Allassembles.Where(x => x.PQCCheckTime != null).Count();
            int TotalTimeSpanSecond = Convert.ToInt32(TotalTimeSpan.Hours.ToString()) * 3600 + Convert.ToInt32(TotalTimeSpan.Minutes.ToString()) * 60 + Convert.ToInt32(TotalTimeSpan.Seconds.ToString());
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
                    ViewBag.AvgTimeSpan = "";//向View传递计算平均用时
                }
            }
            else
            {
                ViewBag.AvgTimeSpan = "暂时没有已完成组装PQC的模组";//向View传递计算平均用时
            }
            #endregion

            //列出记录
            AllassemblesList = Allassembles.OrderBy(c => c.BoxBarCode).ToList();

            //取出记录对应的模块清单
            List<string> list = new List<string>();
            foreach (Assemble item in AllassemblesList)
            {
                List<ModelCollections> modelCollectionsList = db.ModelCollections.Where(m => m.BoxBarCode == item.BoxBarCode).ToList();
                if (modelCollectionsList != null)
                {
                    //item.ModelList = JsonConvert.SerializeObject(modelCollectionsList);
                    foreach (var it in modelCollectionsList)
                    {
                        item.ModelList = item.ModelList + "位置ID:" + it.StationId + "," + "\r\n" + "模块条码号:" + it.BarCodesNum + "；" + "\r\n";
                    }
                }
                List<AdapterCard_Power_Collection> AdapterCard_Power_List = db.AdapterCard_Power_Collection.Where(m => m.BoxBarCode == item.BoxBarCode).ToList();
                if (AdapterCard_Power_List != null)
                {
                    foreach (var it in AdapterCard_Power_List)
                    {
                        item.AdapterCard_Power_List = item.AdapterCard_Power_List + it.BarCodesNum + ",";
                    }
                }
            }

            //读出订单中模组总数量
            var assemble_Quantity = (from m in db.OrderMgm
                                     where (m.OrderNum == orderNum)
                                     select m.Boxes).FirstOrDefault();
            //将模组总数量、正常的模组数量、未完成校正模组数量、订单号信息传递到View页面
            ViewBag.Quantity = assemble_Quantity;
            ViewBag.NormalCount = assemble_Normal_Count;
            ViewBag.AbnormalCount = assemble_Abnormal_Count;
            ViewBag.RecordCount = Allassembles.Count();
            ViewBag.NeverFinish = assemble_Quantity - assemble_Normal_Count;
            ViewBag.orderNum = orderNum;

            //未选择订单时隐藏基本信息设置
            if (ViewBag.Quantity == 0)
            { ViewBag.Display = "display:none"; }
            else { ViewBag.Display = "display:normal"; }
            ViewBag.Legend = "display:normal";//显示图例

            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.PQCNormal = PQCNormalList();

            //分页计算功能
            //var recordCount = Allassembles.Count();
            //var pageCount = GetPageCount(recordCount);
            //if (PageIndex >= pageCount && pageCount >= 1)
            //{
            //    PageIndex = pageCount - 1;
            //}

            //AllassemblesList = AllassemblesList.OrderBy(m => m.BoxBarCode)//按条码排序
            //                     .Skip(PageIndex * PAGE_SIZE)
            //                     .Take(PAGE_SIZE).ToList();
            //ViewBag.PageIndex = PageIndex;
            //ViewBag.PageCount = pageCount;

            return View(AllassemblesList);
        }

        #endregion


        #region    --------------------查询订单已完成、未完成、未开始条码
        [HttpPost]
        public ActionResult Assemblechecklist(string orderNum, string station)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Assembles", act = "AssembleIndex" });
            }
            List<Assemble> Allassembles = new List<Assemble>();//订单全部组装记录
            List<string> NotDoList = new List<string>();//未开始做条码清单
            List<string> NeverFinish = new List<string>();//未完成条码清单
            List<string> FinishList = new List<string>();//已完成条码清单
            JObject stationResult = new JObject();//输出结果JObject
            if (!String.IsNullOrEmpty(orderNum) && !String.IsNullOrEmpty(station))
            {
                //调出订单对应全部记录      
                Allassembles = db.Assemble.Where(c => c.OrderNum == orderNum && (c.OldOrderNum == null || c.OldOrderNum == orderNum)).OrderBy(c => c.BoxBarCode).ToList();
            }
            //调出订单所有条码清单
            List<string> barcodelist = db.BarCodes.Where(c => c.OrderNum == orderNum && c.BarCodeType == "模组").OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();
            List<string> recordlist = new List<string>();
            if (Allassembles == null)
            {
                stationResult.Add("NotDoList", JsonConvert.SerializeObject(barcodelist));
                stationResult.Add("NeverFinish", JsonConvert.SerializeObject(NeverFinish));
                stationResult.Add("FinishList", JsonConvert.SerializeObject(FinishList));
            }
            else
            {
                #region   ---------------选择器------------------
                switch (station)
                {
                    case "AssembleStation": //
                        recordlist = Allassembles.Select(c => c.BoxBarCode).Distinct().ToList();
                        //未开始做条码清单
                        NotDoList = barcodelist.Except(recordlist).ToList();
                        //已完成条码清单
                        FinishList = Allassembles.Where(c => c.OrderNum == orderNum && c.AssembleFinish == true).Select(c => c.BoxBarCode).Distinct().ToList();
                        //未完成条码清单
                        NeverFinish = Allassembles.Where(c => c.OrderNum == orderNum && c.AssembleFinish == false).Select(c => c.BoxBarCode).Distinct().ToList().Except(FinishList).ToList();
                        break;
                    case "WaterproofTest": //
                        recordlist = Allassembles.Where(c => c.OrderNum == orderNum && c.WaterproofTestBT != null).Select(c => c.BoxBarCode).Distinct().ToList();
                        //未开始做条码清单
                        NotDoList = barcodelist.Except(recordlist).ToList();
                        //已完成条码清单
                        FinishList = Allassembles.Where(c => c.OrderNum == orderNum && c.WaterproofTestFinish == true).Select(c => c.BoxBarCode).Distinct().ToList();
                        //未完成条码清单
                        NeverFinish = Allassembles.Where(c => c.OrderNum == orderNum && c.WaterproofTestFinish == false).Select(c => c.BoxBarCode).Distinct().ToList().Except(FinishList).ToList();
                        break;
                    case "AssembleAdapterCard": //
                        recordlist = Allassembles.Where(c => c.OrderNum == orderNum && c.AssembleAdapterCardBT != null).Select(c => c.BoxBarCode).Distinct().ToList();
                        //未开始做条码清单
                        NotDoList = barcodelist.Except(recordlist).ToList();
                        //已完成条码清单
                        FinishList = Allassembles.Where(c => c.OrderNum == orderNum && c.AssembleAdapterFinish == true).Select(c => c.BoxBarCode).Distinct().ToList();
                        //未完成条码清单
                        NeverFinish = Allassembles.Where(c => c.OrderNum == orderNum && c.AssembleAdapterFinish == false).Select(c => c.BoxBarCode).Distinct().ToList().Except(FinishList).ToList();
                        break;
                    case "ViewCheck": //
                        recordlist = Allassembles.Where(c => c.OrderNum == orderNum && c.ViewCheckBT != null).Select(c => c.BoxBarCode).Distinct().ToList();
                        //未开始做条码清单
                        NotDoList = barcodelist.Except(recordlist).ToList();
                        //已完成条码清单
                        FinishList = Allassembles.Where(c => c.OrderNum == orderNum && c.ViewCheckFinish == true).Select(c => c.BoxBarCode).Distinct().ToList();
                        //未完成条码清单
                        NeverFinish = Allassembles.Where(c => c.OrderNum == orderNum && c.ViewCheckFinish == false).Select(c => c.BoxBarCode).Distinct().ToList().Except(FinishList).ToList();
                        break;
                    case "ElectricityCheck": //
                        recordlist = Allassembles.Where(c => c.OrderNum == orderNum && c.ElectricityCheckBT != null).Select(c => c.BoxBarCode).Distinct().ToList();
                        //未开始做条码清单
                        NotDoList = barcodelist.Except(recordlist).ToList();
                        //已完成条码清单
                        FinishList = Allassembles.Where(c => c.OrderNum == orderNum && c.ElectricityCheckFinish == true).Select(c => c.BoxBarCode).Distinct().ToList();
                        //未完成条码清单
                        NeverFinish = Allassembles.Where(c => c.OrderNum == orderNum && c.ElectricityCheckFinish == false).Select(c => c.BoxBarCode).Distinct().ToList().Except(FinishList).ToList();
                        break;
                    case "PQCCheck": //

                        var distinBarcodelist = (from s in Allassembles group s by s.BoxBarCode into g select new { id = g.Max(x => x.Id), BarCodesNum = g.Key, Normal = g.OrderByDescending(x => x.Id).Select(c => c.PQCCheckFinish).FirstOrDefault(),FT = g.OrderByDescending(x => x.Id).Select(x => x.PQCCheckFT).FirstOrDefault() }).ToList();
                        distinBarcodelist = distinBarcodelist.OrderBy(c => c.BarCodesNum).ToList();
                        //拿到表格中存在的条码清单
                        var anoBarcode = distinBarcodelist.Where(c => c.FT == null || (c.Normal == true && c.FT != null)).Select(c => c.BarCodesNum).ToList();
                        //未开始做条码清单
                        NotDoList = barcodelist.Except(anoBarcode).ToList();
                        
                        //未完成条码清单
                        NeverFinish = distinBarcodelist.Where(c => c.Normal == false && c.FT == null).Select(c => c.BarCodesNum ).ToList();
                        //已完成条码清单
                        FinishList = distinBarcodelist.Where(c => c.Normal == true).Select(c => c.BarCodesNum).ToList();
                        break;
                }
                #endregion
                stationResult.Add("NotDoList", JsonConvert.SerializeObject(NotDoList));
                stationResult.Add("NeverFinish", JsonConvert.SerializeObject(NeverFinish));
                stationResult.Add("FinishList", JsonConvert.SerializeObject(FinishList));
            }
            return Content(JsonConvert.SerializeObject(stationResult));
        }
        #endregion


        #region --------------------检查条码是否存在
        [HttpPost]
        public Boolean CheckBarCode(string barcode)
        {
            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == barcode) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region --------------------其他方法
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Assemble assemble = db.Assemble.Find(id);
            db.Assemble.Remove(assemble);
            db.SaveChanges();
            return RedirectToAction("AssembleIndex");
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

        public ActionResult OrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
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
        #region --------------------GetnuoOrderList()取出整个OrderMgms的挪用单号列表
        private List<SelectListItem> GetnuoOrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Where(m => m.IsRepertory == true).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
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

        #region --------------------分页函数
        static List<Assemble> GetPageListByIndex(List<Assemble> list, int pageIndex)
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

        private List<Assemble> GetPagedDataSource(List<Assemble> assemble, int pageIndex, int recordCount)
        {
            var pageCount = GetPageCount(recordCount);
            if (pageIndex >= pageCount && pageCount >= 1)
            {
                pageIndex = pageCount - 1;
            }
            assemble = assemble.OrderByDescending(m => m.PQCCheckBT)
                               .Skip(pageIndex * PAGE_SIZE)
                               .Take(PAGE_SIZE).ToList();
            return assemble;
        }
        #endregion

    }


    public class Assembles_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController common = new CommonController();
        private Common_ApiController comapi = new Common_ApiController();


        /*
        //public JObject PQCCheckB([System.Web.Http.FromBody]JObject data)
        //{
        //    
        //    string deprment = data["deprment"].ToString();
        //    string group = data["group"].ToString();
        //    string ordernum = data["ordernum"].ToString();
        //    string barcode = data["barcode"].ToString();
        //    string UserName = data["UserName"].ToString();

        //    JObject result = new JObject();
        //    //检查条码和订单是否准确
        //    var barcodeCheck = comapi.CheckBarcode(ordernum, barcode);
        //    if (barcodeCheck!="true")
        //    {
        //        result.Add("mes", barcodeCheck);
        //        result.Add("pass", false);
        //        return common.GetModuleFromJobjet(result);
        //    }

        //    //在BarCodes条码表中找到此条码号
        //    //在Assembles组装记录表中找不到对应BoxBarCode的记录，准备在Assembles组装记录表中新建记录，包括OrderNum、BoxBarCode、PQCCheckBT、AssemblePQCPrincipal
        //    else if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == barcode && (u.OldBarCodesNum == barcode || u.OldBarCodesNum == null)) == null)
        //    {
        //        //var assembleRecord = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);
        //        var prefix = db.BarCodes.Where(u => u.BarCodesNum == barcode).FirstOrDefault().BarCode_Prefix;
        //        Assemble ass = new Assemble() { OrderNum = ordernum, OldOrderNum = ordernum, BoxBarCode = barcode, OldBarCodesNum = barcode, BarCode_Prefix = prefix, PQCCheckBT = DateTime.Now, AssemblePQCPrincipal = UserName, Department1 = deprment, Group = group };
        //        db.Assemble.Add(ass);
        //        db.SaveChanges();
        //        result.Add("mes", "成功");
        //        result.Add("pass", true);
        //        result.Add("id", ass.Id);
        //        return common.GetModuleFromJobjet(result);

        //    }
        //    //在Assembles组装记录表中找到对应BoxBarCode的记录，如果记录中没有完成的，准备在Assembles组装记录表中新建记录，如果有正常记录将提示不能重复进行QC
        //    else if (db.Assemble.Count(u => u.BoxBarCode == barcode && (u.OldBarCodesNum == barcode || u.OldBarCodesNum == null)) >= 1)
        //    {
        //        //1未完成的记录2.异常记录
        //        //如果只有开始时间，没有结束时间，把此记录调出来
        //        var assemblelist = db.Assemble.Where(m => m.BoxBarCode == barcode && (m.OldBarCodesNum == barcode || m.OldBarCodesNum == null) && m.PQCCheckBT != null && m.PQCCheckFT == null).FirstOrDefault();
        //        var finshassmeble = db.Assemble.Count(m => m.BoxBarCode == barcode && (m.OldBarCodesNum == barcode || m.OldBarCodesNum == null) && m.PQCCheckBT != null && m.PQCCheckFT != null && m.PQCCheckFinish == true);
        //        if (assemblelist != null)
        //        {
        //            result.Add("mes", "成功");
        //            result.Add("pass", true);
        //            result.Add("id", assemblelist.Id);
        //            return common.GetModuleFromJobjet(result);
        //        }
        //        //如果没有正常完成记录,只有异常记录的,则创建新记录
        //        else if (finshassmeble == 0)
        //        {
        //            var prefix = db.BarCodes.Where(u => u.BarCodesNum == barcode).FirstOrDefault().BarCode_Prefix;
        //            Assemble ass = new Assemble() { OrderNum = ordernum, OldOrderNum = ordernum, BoxBarCode = barcode, OldBarCodesNum = barcode, BarCode_Prefix = prefix, PQCCheckBT = DateTime.Now, AssemblePQCPrincipal = UserName, Department1 = deprment, Group = group };
        //            db.Assemble.Add(ass);
        //            db.SaveChanges();

        //            result.Add("mes", "成功");
        //            result.Add("pass", true);
        //            result.Add("id", ass.Id);
        //            return common.GetModuleFromJobjet(result);
        //        }
        //        //有正常记录的则返回错误
        //        else
        //        {
        //            //return Content("<script>alert('此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！');window.location.href='../Assembles/AssembleIndex';</script>");
        //            result.Add("mes", "此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！");
        //            result.Add("pass", false);
        //            result.Add("id", null);
        //            return common.GetModuleFromJobjet(result);
        //        }
        //    }

        //    result.Add("mes", "操作失败");
        //    result.Add("pass", false);
        //    result.Add("id", null);
        //    return common.GetModuleFromJobjet(result);
        //}
        */

        [HttpPost]
        [ApiAuthorize]
        public JObject PQCCheckB([System.Web.Http.FromBody]JObject data)
        {
            /*
             * string deprment, string group, string ordernum, string barcode, string remark, string UserName
             */
            string deprment = data["deprment"].ToString();
            string group = data["group"].ToString();
            string ordernum = data["ordernum"].ToString();
            string barcode = data["barcode"].ToString();
            string UserName = data["UserName"].ToString();

            JObject result = new JObject();
            //检查条码和订单是否准确
            var barcodeCheck = comapi.CheckBarcode(ordernum, barcode);
            if (barcodeCheck != "true")
            {
                return common.GetModuleFromJobjet(result,false, barcodeCheck);
            }

            var startStatu = comapi.CheckSectionStart(barcode, "Assembles", "BoxBarCode", "PQCCheckFT", "PQCCheckFinish");
            if (startStatu == "新增")
            {
                var prefix = db.BarCodes.Where(u => u.BarCodesNum == barcode).FirstOrDefault().BarCode_Prefix;
                Assemble ass = new Assemble() { OrderNum = ordernum, OldOrderNum = ordernum, BoxBarCode = barcode, OldBarCodesNum = barcode, BarCode_Prefix = prefix, PQCCheckBT = DateTime.Now, AssemblePQCPrincipal = UserName, Department1 = deprment, Group = group };
                db.Assemble.Add(ass);
                db.SaveChanges();
                result.Add("id", ass.Id);
                return common.GetModuleFromJobjet(result,true, "成功");
            }
            else if (startStatu == "修改")
            {
                var assemblelist = db.Assemble.Where(m => m.BoxBarCode == barcode && (m.OldBarCodesNum == barcode || m.OldBarCodesNum == null) && m.PQCCheckBT != null && m.PQCCheckFT == null).FirstOrDefault();
                result.Add("id", assemblelist.Id);
                return common.GetModuleFromJobjet(result, true, "成功");
            }
            else if (startStatu == "重复")
            {
                result.Add("id", null);
                return common.GetModuleFromJobjet(result,false, "此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！");
            }
            result.Add("id", null);
            return common.GetModuleFromJobjet(result,false, "操作失败");
        }

        [HttpPost]
        [ApiAuthorize]
        public JObject PQCCheckF([System.Web.Http.FromBody]JObject data)
        {
            int id = int.Parse(data["id"].ToString());
            string remark = data["remark"].ToString();
            int line = int.Parse(data["line"].ToString());
            string PQCRepairCondition = data["PQCRepairCondition"].ToString();
            int UserId = int.Parse(data["UserId"].ToString());
            string Messagr = data["Messagr"].ToString();
            string nuoOrder = data["nuoOrder"].ToString();
            string nuoBarCode = data["nuoBarCode"].ToString();
            string isnuo = data["isnuo"].ToString();

            JObject result = new JObject();
            var assembleinfo = db.Assemble.Find(id);
            if (assembleinfo.PQCCheckFT == null)
            {
                //时间计算
                assembleinfo.PQCCheckFT = DateTime.Now;
                JObject time = comapi.CalculateTimespan(assembleinfo.PQCCheckBT.Value, assembleinfo.PQCCheckFT.Value);
                assembleinfo.PQCCheckTime = TimeSpan.Parse(time["Time"].ToString());
                assembleinfo.PQCCheckDate = int.Parse(time["Date"].ToString());
               
                assembleinfo.AssembleLineId = line;
                if (PQCRepairCondition == "正常")
                {
                    assembleinfo.PQCCheckAbnormal = "正常";
                    assembleinfo.PQCRepairCondition = "正常";
                    assembleinfo.PQCCheckFinish = true;
                }
                else if (Messagr == null)
                {
                    return common.GetModuleFromJobjet(result,false, "转维修站”或“现场维修”时，异常情况不能为“正常!");
                }
                else
                {
                    assembleinfo.PQCCheckAbnormal = Messagr;
                    assembleinfo.PQCRepairCondition = PQCRepairCondition;
                }

                //添加关联表
                if (!string.IsNullOrEmpty(nuoBarCode) && isnuo == "true")
                {
                    BarCodeRelation barcoderelation = new BarCodeRelation() { OldOrderNum = nuoOrder, OldBarCodeNum = nuoBarCode, NewBarCodesNum = assembleinfo.BoxBarCode, NewOrderNum = assembleinfo.OrderNum, Procedure = "PQC", UsserID = UserId, CreateDate = DateTime.Now };
                    if (!comm.InsertRelation(barcoderelation))
                    {
                        return common.GetModuleFromJobjet(result, false, "挪用订单失败，此条码已挪用过库存条码");
                    }
                    #region 挪用修改原条码工序记录
                    common.NuoOperation(nuoBarCode, nuoOrder, assembleinfo.BoxBarCode, assembleinfo.OrderNum);
                    #endregion
                }
            }
            else
            {
                return common.GetModuleFromJobjet(result,false, "记录已存在，不能更改！");
            }

            db.Entry(assembleinfo).State = EntityState.Modified;
            db.SaveChanges();
            return common.GetModuleFromJobjet(result,true, "成功");


        }

        [HttpPost]
        [ApiAuthorize]
        public JObject NewIndex([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();
            var info = db.Assemble.Where(c => c.OrderNum == ordernum && (c.OldOrderNum == null || c.OldOrderNum == ordernum)).Select(c => new CommonController.TempIndex { BarCodesNum = c.BoxBarCode, EndTime = c.PQCCheckFT, Finish = c.PQCCheckFinish, StarTime = c.PQCCheckBT, OrderNum = c.OrderNum, Group = c.Group, Principal = c.AssemblePQCPrincipal, RepairCondition = c.PQCRepairCondition, Line = c.AssembleLineId, Repetition = c.RepetitionPQCCheck, RepetitionCause = c.RepetitionPQCCheckCause, Abnormal = c.PQCCheckAbnormal, id = c.Id }).ToList();

            return common.GetModuleFromJobjet(common.GeneralIndex(ordernum, info));
        }



        #region    --------------------查询订单已完成、未完成、未开始条码
        [HttpPost]
        [ApiAuthorize]
        public JObject Assemblechecklist([System.Web.Http.FromBody]JObject data)
        {
            string orderNum = data["orderNum"].ToString();
            JObject stationResult = new JObject();//输出结果JObject
            if (!String.IsNullOrEmpty(orderNum))
            {
                //调出订单对应全部记录      
               var  Allassembles = db.Assemble.Where(c => c.OrderNum == orderNum && (c.OldOrderNum == null || c.OldOrderNum == orderNum)).Select(c=>new Common_ApiController.Temp { id=c.Id,BarCodesNum=c.BoxBarCode,Finsh=c.PQCCheckFinish,FinshTime=c.PQCCheckFT}).ToList();
                stationResult = comapi.SectionCehckList(orderNum, Allassembles);
            }
            return common.GetModuleFromJobjet(stationResult);
        }
        #endregion


        #region --------------------检查条码是否存在
        //[HttpPost]
        //[ApiAuthorize]
        //public JObject CheckBarCode([System.Web.Http.FromBody]JObject data)
        //{
        //    string barcode = data["barcode"].ToString();
        //    JObject result = new JObject();
        //    if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == barcode) == null)
        //    {
        //        result.Add("value", false);
        //        return common.GetModuleFromJobjet(result);
        //    }
        //    else
        //    {
        //        result.Add("value", true);
        //        return common.GetModuleFromJobjet(result);
        //    }
        //}
        #endregion



        #region --------------------GetOrderList()取出整个OrderMgms的OrderNum订单号列表
        [HttpPost]
        [ApiAuthorize]
        public JObject OrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return common.GetModuleFromJarray(result);
        }
        //----------------------------------------------------------------------------------------
        #endregion
        #region --------------------GetnuoOrderList()取出整个OrderMgms的挪用单号列表
        //public JObject nuoOrderList()
        //{
        //    var orders = db.OrderMgm.OrderByDescending(m => m.ID).Where(m => m.IsRepertory == true).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
        //    JArray result = new JArray();
        //    foreach (var item in orders)
        //    {
        //        JObject List = new JObject();
        //        List.Add("value", item);

        //        result.Add(List);
        //    }
        //    return common.GetModuleFromJarray(result);
        //}
        //----------------------------------------------------------------------------------------
        #endregion
    }
}
