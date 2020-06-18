﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json.Linq;
using JianHeMES.Controllers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static JianHeMES.Controllers.CommonalityController;
using System.ComponentModel.DataAnnotations;

namespace JianHeMESEntities.Controllers
{
    public class OrderMgmsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        CommonController com = new CommonController();
        BarCodesController BarCodesController = new BarCodesController();
        public class UpdateModule
        {
            public string barcode { get; set; }
            public string module { get; set; }
        }
        // GET: OrderMgms
        public ActionResult orderRule()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "orderRule" });
            }
            return View();
        }

        #region --------------------GetOrderNumList()检索订单号
        private List<SelectListItem> GetOrderNumList()
        {
            var ordernum = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum).Distinct();

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

        private List<SelectListItem> GetPlatformTypeList()
        {
            var platformTypeList = new List<SelectListItem>();
            var platformTypelist = db.OrderMgm.Select(c => c.PlatformType).Distinct();
            foreach(var item in platformTypelist)
            {
                platformTypeList.Add(new SelectListItem
                {
                    Text = item,
                    Value = item
                });
            }
            return platformTypeList;
        }

        #endregion
        #region --------------------装配负责部门列表
        private List<SelectListItem> AssembleDepartmentList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "装配一部",
                    Value = "装配一部"
                },
                new SelectListItem
                {
                    Text = "装配二部",
                    Value = "装配二部"
                }
            };
        }
        #endregion

        #region --------------------分页
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

        #region --------------------首页
        // GET: OrderMgms
        public ActionResult Index()
        {
            ViewBag.OrderNumList = GetOrderNumList();//向View传递OrderNum订单号列表.
            ViewBag.PlatformTypeList = GetPlatformTypeList();//平台列表
            //ViewBag.BarCodeTypeList = GetBarCodeTypeList();
            //return View(GetPagedDataSource(barcodes, 0, recordCount));

            ViewBag.Display = "display:none";//隐藏View基本情况信息
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string OrderNum, string searchString, string PlatformType, int PageIndex = 0)
        {
            var ordernums = db.OrderMgm as IQueryable<OrderMgm>;
            if (!String.IsNullOrEmpty(OrderNum))
            {
                ordernums = ordernums.Where(m => m.OrderNum == OrderNum);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                ordernums = ordernums.Where(m => m.OrderNum.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(PlatformType))
            {
                ordernums = ordernums.Where(m => m.PlatformType.Contains(PlatformType));
            }

            var recordCount = ordernums.Count();
            var pageCount = GetPageCount(recordCount);
            if (PageIndex >= pageCount && pageCount >= 1)
            {
                PageIndex = pageCount - 1;
            }

            ordernums = ordernums.OrderBy(m => m.OrderNum)
                 .Skip(PageIndex * PAGE_SIZE).Take(PAGE_SIZE);
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageCount = pageCount;

            ViewBag.OrderNumList = GetOrderNumList();
            ViewBag.PlatformTypeList = GetPlatformTypeList();//平台列表

            ViewBag.OrderNum = OrderNum;
            ViewBag.searchString = searchString;
            ViewBag.PlatformType = PlatformType;
            return View(ordernums.ToList());
        }

        //查询结果输出Excel文件

　　　　public class OrdermgmsList
        {
            [Display(Name = "订单号"), StringLength(50)]
            public string OrderNum { get; set; }

            [Display(Name = "客户名称"), StringLength(50)]
            public string CustomerName { get; set; }

            [Display(Name = "下单日期")]
            public string ContractDate { get; set; }

            [Display(Name = "出货日期")]
            public string DeliveryDate { get; set; }

            [Display(Name = "计划投入时间")]
            public string PlanInputTime { get; set; }

            [Display(Name = "计划完成时间")]
            public string PlanCompleteTime { get; set; }

            [Display(Name = "平台型号")]
            public string PlatformType { get; set; }

            [Display(Name = "地区")]
            public string Area { get; set; }

            [Display(Name = "制程要求")]//有铅，无铅
            public string ProcessingRequire { get; set; }

            [Display(Name = "标准要求"), StringLength(50)] //商用，军用
            public string StandardRequire { get; set; }

            [Display(Name = "SMT产能")]
            public string CapacityQ { get; set; }

            [Display(Name = "小样进度"), StringLength(50)]
            public string HandSampleScedule { get; set; }

            [Display(Name = "模组数量")]
            public int Boxes { get; set; }

            [Display(Name = "模块数量")]
            public string Models { get; set; }

            [Display(Name = "电源数量")]
            public string Powers { get; set; }

            [Display(Name = "转接卡")]
            public string AdapterCard { get; set; }

            [Display(Name = "模组条码是否已经生成")]
            public string BarCodeCreated { get; set; }

            [Display(Name = "模组条码生成日期")]
            public string BarCodeCreateDate { get; set; }

            [Display(Name = "模组条码生成者"), StringLength(50)]
            public string BarCodeCreator { get; set; }

            [Display(Name = "是否为库存")]
            public string IsRepertory { get; set; }

            [Display(Name = "备注"), StringLength(500)]
            public string Remark { get; set; }
        }

        [HttpPost]
        public ActionResult IndexQueryToExcel(string OrderNum, string searchString, string PlatformType, int PageIndex = 0)
        {
            var ordernums = db.OrderMgm as IQueryable<OrderMgm>;
            if (!String.IsNullOrEmpty(OrderNum))
            {
                ordernums = ordernums.Where(m => m.OrderNum == OrderNum);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                ordernums = ordernums.Where(m => m.OrderNum.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(PlatformType))
            {
                ordernums = ordernums.Where(m => m.PlatformType.Contains(PlatformType));
            }

            List<OrdermgmsList> Resultlist = new List<OrdermgmsList>();
            foreach(var item in ordernums)
            {
                var record = new OrdermgmsList();
                record.OrderNum = item.OrderNum;
                record.CustomerName = item.CustomerName;
                record.ContractDate = string.Format("{0:d}",item.ContractDate);
                record.DeliveryDate = string.Format("{0:d}",item.DeliveryDate);
                record.PlanInputTime = string.Format("{0:d}",item.PlanInputTime);
                record.PlanCompleteTime = string.Format("{0:d}",item.PlanCompleteTime);
                record.PlatformType = item.PlatformType;
                record.Area = item.Area;
                record.ProcessingRequire = item.ProcessingRequire;
                record.StandardRequire = item.StandardRequire;
                record.CapacityQ = item.CapacityQ == 0 ?"": item.CapacityQ.ToString();
                record.HandSampleScedule = item.HandSampleScedule;
                record.Boxes = item.Boxes;
                record.Models = item.Models == 0 ? "" : item.Models.ToString(); 
                record.Powers = item.Powers == 0 ? "" : item.Powers.ToString();
                record.AdapterCard = item.AdapterCard == 0 ? "" : item.AdapterCard.ToString();
                record.BarCodeCreated = item.BarCodeCreated == 0 ? "否" : "是";
                record.BarCodeCreateDate = string.Format("{0:G}", item.BarCodeCreateDate);
                record.BarCodeCreator = item.BarCodeCreator;
                record.IsRepertory = item.IsRepertory==false? "否" : "是"; 
                record.Remark = item.Remark;
                Resultlist.Add(record);
            }
            if (ordernums.Count() > 0)
            {
                string[] columns = { "订单号", "客户名称", "下单日期", "出货日期", "计划投入时间", "计划完成时间", "平台型号", "地区", "制程要求", "标准要求", "SMT产能", "小样进度" , "模组数量", "模块数量", "电源数量", "转接卡", "模组条码是否已经生成", "模组条码生成日期", "模组条码生成者", "是否为库存", "备注" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, "订单搜索记录表", false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, "订单搜索记录表.xlsx");
            }
            else
            {
                OrdermgmsList at1 = new OrdermgmsList();
                at1.OrderNum = "没有找到相关记录！";
                Resultlist.Add(at1);
                string[] columns = { "订单号", "客户名称", "下单日期", "出货日期", "计划投入时间", "计划完成时间", "平台型号", "地区", "制程要求", "标准要求", "SMT产能", "小样进度", "模组数量", "模块数量", "电源数量", "转接卡", "模组条码是否已经生成", "模组条码生成日期", "模组条码生成者", "是否为库存", "备注" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, "订单搜索记录表", false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, "订单搜索记录表.xlsx");
            }
        }


        [HttpPost]
        public ActionResult IndexQueryToExcel2(string OrderNum, string searchString, string PlatformType, int PageIndex = 0)
        {
            var ordernums = db.OrderMgm as IQueryable<OrderMgm>;
            if (!String.IsNullOrEmpty(OrderNum))
            {
                ordernums = ordernums.Where(m => m.OrderNum == OrderNum);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                ordernums = ordernums.Where(m => m.OrderNum.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(PlatformType))
            {
                ordernums = ordernums.Where(m => m.PlatformType.Contains(PlatformType));
            }
            List<OrdermgmsList> Resultlist = new List<OrdermgmsList>();
            foreach (var item in ordernums)
            {
                var record = new OrdermgmsList();
                record.OrderNum = item.OrderNum;
                record.CustomerName = item.CustomerName;
                record.ContractDate = string.Format("{0:d}", item.ContractDate);
                record.DeliveryDate = string.Format("{0:d}", item.DeliveryDate);
                record.PlanInputTime = string.Format("{0:d}", item.PlanInputTime);
                record.PlanCompleteTime = string.Format("{0:d}", item.PlanCompleteTime);
                record.PlatformType = item.PlatformType;
                record.Area = item.Area;
                record.ProcessingRequire = item.ProcessingRequire;
                record.StandardRequire = item.StandardRequire;
                record.CapacityQ = item.CapacityQ == 0 ? "" : item.CapacityQ.ToString();
                record.HandSampleScedule = item.HandSampleScedule;
                record.Boxes = item.Boxes;
                record.Models = item.Models == 0 ? "" : item.Models.ToString();
                record.Powers = item.Powers == 0 ? "" : item.Powers.ToString();
                record.AdapterCard = item.AdapterCard == 0 ? "" : item.AdapterCard.ToString();
                record.BarCodeCreated = item.BarCodeCreated == 0 ? "否" : "是";
                record.BarCodeCreateDate = string.Format("{0:G}", item.BarCodeCreateDate);
                record.BarCodeCreator = item.BarCodeCreator;
                record.IsRepertory = item.IsRepertory == false ? "否" : "是";
                record.Remark = item.Remark;
                Resultlist.Add(record);
            }
            return new ExcelResult<OrdermgmsList>(Resultlist);

        }

        #endregion

        #region --------------------Details页
        // GET: OrderMgms/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Details" + "/" + id.ToString() });
            }
            //ViewBag.smallsmple = com.isCheckRole("订单管理", "修改小样进度", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum);

            //ViewBag.displaybarcode = com.isCheckRole("条码管理", "创建全部条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum);

            //ViewBag.TCorder = com.isCheckRole("订单管理", "特采订单", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum);

            //ViewBag.YCorder = com.isCheckRole("订单管理", "异常订单", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum);

            //ViewBag.XYorder = com.isCheckRole("订单管理", "小样订单", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum);

            //ViewBag.ZZfirst = com.isCheckRole("订单管理", "组装首件", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum);

            //ViewBag.LLfirst = com.isCheckRole("订单管理", "老化首件", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum);

            //ViewBag.BZfirst = com.isCheckRole("订单管理", "包装首件", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }

            #region----------订单各工段详细信息
            JObject ProductionDetailsJson = new JObject();
            int modelGroupQuantity = db.OrderMgm.Where(c => c.OrderNum == orderMgm.OrderNum).FirstOrDefault().Boxes;//订单模组数量

            #region----------订单在组装的统计数据
            var assembleRecord = db.Assemble.Where(c => c.OrderNum == orderMgm.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == orderMgm.OrderNum)).ToList();//订单在组装的全部记录
            //开始时间 
            var assemble_begintime = assembleRecord.Min(c => c.PQCCheckBT);
            ProductionDetailsJson.Add("AssembleBeginTime", assemble_begintime.ToString());//开始时间

            //最后时间
            var assemble_endtime = assembleRecord.Max(c => c.PQCCheckFT);
            ProductionDetailsJson.Add("AssembleEndTime", assemble_endtime.ToString());//最后时间

            //完成时间
            DateTime? assemble_finishtime = new DateTime();
            if (assembleRecord.Count(c => c.PQCCheckFinish == true) == modelGroupQuantity)
            {
                assemble_finishtime = assemble_endtime;
            }
            else
            {
                assemble_finishtime = null;
            }
            ProductionDetailsJson.Add("AssembleFinishTime", assemble_finishtime.ToString());//完成时间

            //作业时长
            var assemble_newtime = assemble_endtime - assemble_begintime;//目前时长
            ProductionDetailsJson.Add("AssembleNewTime", assemble_newtime.ToString());//目前时长

            //直通个数
            var assemble_NotFirstPassYield = assembleRecord.Where(c => c.PQCCheckFinish == false).Select(c => c.BoxBarCode).Distinct().ToList();
            var assemble_PassYield = assembleRecord.Where(c => c.PQCCheckFinish == true && c.RepetitionPQCCheck == false).Select(c => c.BoxBarCode).ToList();
            var assemble_FirstPassYield = assemble_PassYield.Except(assemble_NotFirstPassYield).ToList();//直通条码记录
            var assemble_FirstPassYieldCount = assemble_FirstPassYield.Count();//直通个数
            ProductionDetailsJson.Add("AssembleFirstPassYieldCount", assemble_FirstPassYieldCount.ToString());//直通个数
            if (assemble_FirstPassYieldCount == 0)
            {
                ProductionDetailsJson.Add("AssembleFirstPassYield_Rate", "");//直通率
            }
            else
            {
                var assemble_FirstPassYield_Rate = (Convert.ToDouble(assemble_FirstPassYieldCount) / modelGroupQuantity * 100).ToString("F2");//直通率：直通数/模组数
                ProductionDetailsJson.Add("AssembleFirstPassYield_Rate", assemble_FirstPassYield_Rate);//直通率
            }

            //正常个数
            int PassYieldCount = assemble_PassYield.Count();
            ProductionDetailsJson.Add("AssemblePassYieldCount", PassYieldCount.ToString());//正常个数

            //有效工时
            //异常个数
            //异常工时

            //完成率
            var assemble_finished = assembleRecord.Count(m => m.PQCCheckFinish == true && m.RepetitionPQCCheck == false);//订单已完成PQC个数
            var assemble_finishedList = assembleRecord.Where(m => m.PQCCheckFinish == true).Select(m => m.BoxBarCode).ToList(); //订单已完成PQC的条码清单
            var assemble_assemblePQC_Count = assembleRecord.Count();//订单PQC全部记录条数
            if (assemble_finished == 0)
            {
                ProductionDetailsJson.Add("AssembleFinisthRate", "");//完成率
                ProductionDetailsJson.Add("AssemblePassRate", "");//合格率
            }
            else
            {
                var assemble_finisthRate = (Convert.ToDouble(assemble_finished) / modelGroupQuantity * 100).ToString("F2");//完成率：完成数/订单的模组数
                ProductionDetailsJson.Add("AssembleFinisthRate", assemble_finisthRate);//完成率
                //合格率
                var passRate = (Convert.ToDouble(assemble_finished) / assemble_assemblePQC_Count * 100).ToString("F2");//合格率：完成数/记录数
                ProductionDetailsJson.Add("AssemblePassRate", passRate);//合格率
            }

            #endregion


            #region----------订单在FQC的统计数据
            var FinalQCRecord = db.FinalQC.OrderBy(c => c.BarCodesNum).Where(c => c.OrderNum == orderMgm.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == orderMgm.OrderNum)).ToList();//订单在组装的全部记录
            //开始时间 
            var FinalQC_begintime = FinalQCRecord.Min(c => c.FQCCheckBT);
            ProductionDetailsJson.Add("FinalQCBeginTime", FinalQC_begintime.ToString());//开始时间

            //最后时间
            var FinalQC_endtime = FinalQCRecord.Max(c => c.FQCCheckFT);
            ProductionDetailsJson.Add("FinalQCEndTime", FinalQC_endtime.ToString());//最后时间

            //完成时间
            DateTime? FinalQC_finishtime = new DateTime();
            if (FinalQCRecord.Count(c => c.FQCCheckFinish == true) == modelGroupQuantity)
            {
                FinalQC_finishtime = FinalQC_endtime;
            }
            else
            {
                FinalQC_finishtime = null;
            }
            ProductionDetailsJson.Add("FinalQCFinishTime", FinalQC_finishtime.ToString());//完成时间

            //作业时长
            var FinalQC_newtime = FinalQC_endtime - FinalQC_begintime;//目前时长
            ProductionDetailsJson.Add("FinalQCNewTime", FinalQC_newtime.ToString());//目前时长

            //直通个数
            var FinalQC_NotFirstPassYield = FinalQCRecord.Where(c => c.FQCCheckFinish == false).Select(c => c.BarCodesNum).Distinct().ToList();
            var FinalQC_PassYield = FinalQCRecord.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Select(c => c.BarCodesNum).ToList();
            var FinalQC_FirstPassYield = FinalQC_PassYield.Except(FinalQC_NotFirstPassYield).ToList();//直通条码记录
            var FinalQC_FirstPassYieldCount = FinalQC_FirstPassYield.Count();//直通个数
            ProductionDetailsJson.Add("FinalQCFirstPassYieldCount", FinalQC_FirstPassYieldCount.ToString());//直通个数
            if (FinalQC_FirstPassYieldCount == 0)
            {
                ProductionDetailsJson.Add("FinalQCFirstPassYield_Rate", "");//直通率
            }
            else
            {
                var FinalQC_FirstPassYield_Rate = (Convert.ToDouble(FinalQC_FirstPassYieldCount) / modelGroupQuantity * 100).ToString("F2");//直通率：直通数/模组数
                ProductionDetailsJson.Add("FinalQCFirstPassYield_Rate", FinalQC_FirstPassYield_Rate);//直通率
            }

            //正常个数
            int FinalQC_PassYieldCount = FinalQC_PassYield.Count();
            ProductionDetailsJson.Add("FinalQCPassYieldCount", FinalQC_PassYieldCount.ToString());//正常个数

            //有效工时
            //异常个数
            //异常工时

            //完成率
            var FinalQC_finished = FinalQCRecord.Count(m => m.FQCCheckFinish == true && m.RepetitionFQCCheck == false);//订单已完成PQC个数
            var FinalQC_finishedList = FinalQCRecord.Where(m => m.FQCCheckFinish == true).Select(m => m.BarCodesNum).ToList(); //订单已完成PQC的条码清单
            var FinalQC_Count = FinalQCRecord.Count();//订单PQC全部记录条数
            if (FinalQC_finished == 0)
            {
                ProductionDetailsJson.Add("FinalQCFinisthRate", "");//完成率
                ProductionDetailsJson.Add("FinalQCPassRate", "");//合格率
            }
            else
            {
                var FinalQC_finisthRate = (Convert.ToDouble(FinalQC_finished) / modelGroupQuantity * 100).ToString("F2");//完成率：完成数/订单的模组数
                ProductionDetailsJson.Add("FinalQCFinisthRate", FinalQC_finisthRate);//完成率
                //合格率
                var passRate = (Convert.ToDouble(FinalQC_finished) / FinalQC_Count * 100).ToString("F2");//合格率：完成数/记录数
                ProductionDetailsJson.Add("FinalQCPassRate", passRate);//合格率
            }
            #endregion


            #region----------订单在老化的统计数据
            var burn_inRecord = db.Burn_in.Where(c => c.OrderNum == orderMgm.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == orderMgm.OrderNum)).ToList();//订单在老化的全部记录
            //开始时间 
            var burn_in_begintime = burn_inRecord.Min(c => c.OQCCheckBT);
            ProductionDetailsJson.Add("Burn_inBeginTime", burn_in_begintime.ToString());//开始时间 

            //最后时间
            var burn_in_endtime = burn_inRecord.Max(c => c.OQCCheckFT);
            ProductionDetailsJson.Add("Burn_inEndTime", burn_in_endtime.ToString());//最后时间

            //完成时间
            DateTime? burn_infinishtime = new DateTime();
            if (burn_inRecord.Count(c => c.OQCCheckFinish == true) == modelGroupQuantity)
            {
                burn_infinishtime = burn_in_endtime;
            }
            else
            {
                burn_infinishtime = null;
            }
            ProductionDetailsJson.Add("Burn_inFinishTime", burn_infinishtime.ToString());//完成时间

            //作业时长
            var burn_innewtime = burn_in_endtime - burn_in_begintime;//目前时长
            ProductionDetailsJson.Add("Burn_inNewTime", burn_innewtime.ToString());//目前时长

            //直通个数
            var burn_in_NotFirstPassYield = burn_inRecord.Where(c => c.OQCCheckFinish == false).Select(c => c.BarCodesNum).Distinct().ToList();
            var burn_in_PassYield = burn_inRecord.Where(c => c.OQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
            var burn_in_FirstPassYield = burn_in_PassYield.Except(burn_in_NotFirstPassYield).ToList();//直通条码记录
            var burn_in_FirstPassYieldCount = burn_in_PassYield.Count();//直通个数
            ProductionDetailsJson.Add("Burn_inFirstPassYieldCount", burn_in_FirstPassYieldCount.ToString());//直通个数
            if (burn_in_FirstPassYieldCount == 0)
            {
                ProductionDetailsJson.Add("Burn_inFirstPassYield_Rate", "");//直通率
            }
            else
            {
                var burn_in_FirstPassYield_Rate = (Convert.ToDouble(burn_in_FirstPassYieldCount) / burn_inRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2");//直通率：直通数/模组数
                ProductionDetailsJson.Add("Burn_inFirstPassYield_Rate", burn_in_FirstPassYield_Rate);//直通率
            }

            //正常个数
            int burn_inPassYieldCount = burn_in_PassYield.Count();
            ProductionDetailsJson.Add("Burn_inPassYieldCount", burn_inPassYieldCount.ToString());

            //有效工时
            //异常个数
            //异常工时

            //完成率
            var burn_in_finished = burn_inRecord.Count(m => m.OQCCheckFinish == true);//订单已完成PQC个数
            var burn_in_finishedList = burn_inRecord.Where(m => m.OQCCheckFinish == true).Select(m => m.BarCodesNum).ToList(); //订单已完成PQC的条码清单
            var burn_in_Count = burn_inRecord.Count();//订单PQC全部记录条数
            if (burn_in_Count == 0)
            {
                ProductionDetailsJson.Add("Burn_inFinisthRate", "");
                ProductionDetailsJson.Add("Burn_inPassRate", "");

            }
            else
            {
                var burn_in_finisthRate = (Convert.ToDouble(burn_in_finished) / burn_inRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2");//完成率：完成数/订单的模组数
                ProductionDetailsJson.Add("Burn_inFinisthRate", burn_in_finisthRate);
                //合格率
                var burn_inPassRate = (Convert.ToDouble(burn_in_finished) / burn_in_Count * 100).ToString("F2");//合格率：完成数/记录数
                ProductionDetailsJson.Add("Burn_inPassRate", burn_inPassRate);
            }
            #endregion


            #region----------订单在校正的统计数据
            var calibrationRecord = db.CalibrationRecord.Where(c => c.OrderNum == orderMgm.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == orderMgm.OrderNum)).ToList();//订单在校正的全部记录
            //开始时间 
            var calibration_begintime = calibrationRecord.Min(c => c.BeginCalibration);
            ProductionDetailsJson.Add("CalibrationBeginTime", calibration_begintime.ToString());//开始时间 

            //最后时间
            var calibration_endtime = calibrationRecord.Max(c => c.FinishCalibration);
            ProductionDetailsJson.Add("CalibrationEndTime", calibration_endtime.ToString());//最后时间

            //完成时间
            DateTime? calibration_finishtime = new DateTime();
            if (calibrationRecord.Count(c => c.Normal == true) == modelGroupQuantity)
            {
                calibration_finishtime = calibration_endtime;
            }
            else
            {
                calibration_finishtime = null;
            }
            ProductionDetailsJson.Add("CalibrationFinishTime", calibration_finishtime.ToString());//完成时间

            //作业时长
            var calibration_newtime = calibration_endtime - calibration_begintime;//目前时长
            ProductionDetailsJson.Add("CalibrationNewTime", calibration_newtime.ToString());//目前时长

            //直通个数
            var calibration_NotFirstPassYield = calibrationRecord.Where(c => c.Normal == false && c.RepetitionCalibration == false).Select(c => c.BarCodesNum).Distinct().ToList();
            var calibration_PassYield = calibrationRecord.Where(c => c.Normal == true && c.RepetitionCalibration == false).Select(c => c.BarCodesNum).ToList();
            var calibration_FirstPassYield = calibration_PassYield.Except(calibration_NotFirstPassYield).ToList();//直通条码记录
            var calibration_FirstPassYieldCount = calibration_PassYield.Count();//直通个数
            ProductionDetailsJson.Add("CalibrationFirstPassYieldCount", calibration_FirstPassYieldCount.ToString());//直通个数
            if (calibration_FirstPassYieldCount == 0)
            {
                ProductionDetailsJson.Add("CalibrationFirstPassYield_Rate", "");//直通率
            }
            else
            {
                var calibration_FirstPassYield_Rate = (calibration_FirstPassYieldCount / calibrationRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2");//直通率：直通数/模组数
                ProductionDetailsJson.Add("CalibrationFirstPassYield_Rate", calibration_FirstPassYield_Rate);//直通率
            }

            //正常个数
            int calibration_PassYieldCount = calibration_PassYield.Count();
            ProductionDetailsJson.Add("CalibrationPassYieldCount", calibration_PassYieldCount.ToString());

            //有效工时
            //异常个数
            //异常工时

            //完成率
            //var calibration_finished = calibrationRecord.Count(m => m.Normal == true);//订单已完成PQC个数
            var calibration_finished = calibrationRecord.Where(c => c.Normal == true && c.RepetitionCalibration == false).Select(c => c.BarCodesNum).Distinct().Count();//订单已完成PQC个数
            var calibration_finishedList = calibrationRecord.Where(m => m.Normal == true && m.RepetitionCalibration == false).Select(m => m.BarCodesNum).ToList(); //订单已完成PQC的条码清单
            var calibration_Count = calibrationRecord.Where(c => c.RepetitionCalibration == false).Count();//订单PQC全部记录条数
            if (calibration_Count == 0)
            {
                ProductionDetailsJson.Add("CalibrationFinisthRate", "");
                ProductionDetailsJson.Add("CalibrationPassRate", "");
            }
            else
            {
                var calibration_finisthRate = Convert.ToDecimal(calibration_finished) / calibrationRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100;//完成率：完成数/订单的模组数
                ProductionDetailsJson.Add("CalibrationFinisthRate", calibration_finisthRate.ToString("F2"));
                //合格率
                var calibration_PassRate = (Convert.ToDecimal(calibration_finished) / calibration_Count) * 100;//合格率：完成数/记录数
                ProductionDetailsJson.Add("CalibrationPassRate", calibration_PassRate.ToString("F2"));
            }
            #endregion


            #region----------订单在外观包装的统计数据
            var appearanceRecord = db.Appearance.Where(c => c.OrderNum == orderMgm.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == orderMgm.OrderNum)).ToList();//订单在包装的全部记录
            //开始时间 
            var appearance_begintime = appearanceRecord.Min(c => c.OQCCheckBT);
            ProductionDetailsJson.Add("AppearanceBeginTime", appearance_begintime.ToString());//开始时间 

            //最后时间
            var appearance_endtime = appearanceRecord.Max(c => c.OQCCheckFT);
            ProductionDetailsJson.Add("AppearanceEndTime", appearance_endtime.ToString());//最后时间

            //完成时间
            DateTime? appearance_finishtime = new DateTime();
            if (appearanceRecord.Count(c => c.OQCCheckFinish == true) == modelGroupQuantity)
            {
                appearance_finishtime = appearance_endtime;
            }
            else
            {
                appearance_finishtime = null;
            }
            ProductionDetailsJson.Add("AppearanceFinishTime", appearance_finishtime.ToString());//完成时间

            //作业时长
            var appearance_newtime = appearance_endtime - appearance_begintime;//目前时长
            ProductionDetailsJson.Add("AppearanceNewTime", appearance_newtime.ToString());//目前时长

            //直通个数
            var appearance_NotFirstPassYield = appearanceRecord.Where(c => c.OQCCheckFinish == false).Select(c => c.BarCodesNum).Distinct().ToList();
            var appearance_PassYield = appearanceRecord.Where(c => c.OQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
            var appearance_FirstPassYield = appearance_PassYield.Except(appearance_NotFirstPassYield).ToList();//直通条码记录
            var appearance_FirstPassYieldCount = appearance_PassYield.Count();//直通个数
            ProductionDetailsJson.Add("AppearanceFirstPassYieldCount", appearance_FirstPassYieldCount.ToString());//直通个数
            if (appearance_FirstPassYieldCount == 0)
            {
                ProductionDetailsJson.Add("AppearanceFirstPassYield_Rate", "");//直通率
            }
            else
            {
                var appearance_FirstPassYield_Rate = (Convert.ToDouble(appearance_FirstPassYieldCount) / appearanceRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2");//直通率：直通数/模组数
                ProductionDetailsJson.Add("AppearanceFirstPassYield_Rate", appearance_FirstPassYield_Rate);//直通率
            }

            //正常个数
            int appearance_PassYieldCount = appearance_PassYield.Count();
            ProductionDetailsJson.Add("AppearancePassYieldCount", appearance_PassYieldCount.ToString());

            //有效工时
            //异常个数
            //异常工时

            //完成率
            var appearance_finished = appearanceRecord.Count(m => m.OQCCheckFinish == true);//订单已完成PQC个数
            var appearance_finishedList = appearanceRecord.Where(m => m.OQCCheckFinish == true).Select(m => m.BarCodesNum).ToList(); //订单已完成PQC的条码清单
            var appearance_Count = appearanceRecord.Count();//订单PQC全部记录条数
            if (appearance_Count == 0)
            {
                ProductionDetailsJson.Add("AppearanceFinisthRate", "");
                ProductionDetailsJson.Add("AppearancePassRate", "");
            }
            else
            {
                var appearance_finisthRate = (Convert.ToDouble(appearance_finished) / appearanceRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2");//完成率：完成数/订单的模组数
                ProductionDetailsJson.Add("AppearanceFinisthRate", appearance_finisthRate);
                //合格率
                var appearance_PassRate = (Convert.ToDouble(appearance_finished) / appearance_Count * 100).ToString("F2");//合格率：完成数/记录数
                ProductionDetailsJson.Add("AppearancePassRate", appearance_PassRate);
            }
            #endregion
            ViewBag.ProductionDetailsJson = ProductionDetailsJson;
            #endregion


            #region----------组装、FQC工段按线别分类输出条码清单
            //订单的全部组装PQC记录
            ViewBag.order_in_Assemble_Record = db.Assemble.OrderBy(c => c.BoxBarCode).Where(c => c.OrderNum == orderMgm.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == orderMgm.OrderNum)).ToList();
            //FQC全部组装记录
            //JObject FQCRecord = new JObject();
            //var count = FinalQCRecord.Select(c => c.FQCPrincipal).Distinct().ToList();
            //int line = 0;
            //foreach (var item in count)
            //{
            //    var info = FinalQCRecord.Where(c => c.FQCPrincipal == item).Select(c => c.BarCodesNum).ToArray();
            //    var infoString = string.Join(",", info);
            //    var linenum = db.Users.Where(c => c.UserName == item&&c.Department=="品质部").Select(c => c.LineNum).FirstOrDefault();
            //    infoString= infoString.Insert(0, linenum + ",");
            //    FQCRecord.Add(line.ToString(), infoString);
            //    line++;
            //}

            ViewBag.order_in_FQC_Record = FinalQCRecord;
            #endregion


            #region----------特采订单基本信息
            //检查特采订单文件目录是否存在
            string directory = "D:\\MES_Data\\AOD_Files\\" + orderMgm.OrderNum;
            if (Directory.Exists(@directory) == true)
            {
                ViewBag.Directory = "Exists";
            }
            //检查特采订单pdf文件是否存在
            string pdfFile = "D:\\MES_Data\\AOD_Files\\" + orderMgm.OrderNum + "\\" + orderMgm.OrderNum + "_AOD.pdf";
            if (System.IO.File.Exists(@pdfFile) == true)
            {
                ViewBag.PDf = "Exists";
            }
            List<FileInfo> filesInfo = new List<FileInfo>();
            JObject json = new JObject();
            if (ViewBag.Directory == "Exists")
            {
                filesInfo = GetAllFilesInDirectory(directory);
                filesInfo = filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum + "_AOD") && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
                //检查特采订单jpg文件是否存在
                if (filesInfo.Count > 0)
                {
                    ViewBag.Picture = "Exists";
                }
                int i = 1;
                foreach (var item in filesInfo)
                {
                    json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.jpgjson = json;
            }
            #endregion

            #region----------小样订单基本信息
            //检查小样订单文件目录是否存在
            string SmallSample_directory = "D:\\MES_Data\\SmallSample_Files\\" + orderMgm.OrderNum;
            if (Directory.Exists(@SmallSample_directory) == true)
            {
                ViewBag.SmallSample_Directory = "Exists";
            }
            //检查小样订单pdf文件是否存在
            string SmallSample_pdfFile = "D:\\MES_Data\\SmallSample_Files\\" + orderMgm.OrderNum + "\\" + orderMgm.OrderNum + "_SmallSample.pdf";
            if (System.IO.File.Exists(@SmallSample_pdfFile) == true)
            {
                ViewBag.SmallSample_PDf = "Exists";
            }
            List<FileInfo> SmallSample_filesInfo = new List<FileInfo>();
            JObject SmallSample_json = new JObject();
            if (ViewBag.SmallSample_Directory == "Exists")
            {
                SmallSample_filesInfo = GetAllFilesInDirectory(SmallSample_directory);
                SmallSample_filesInfo = SmallSample_filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum + "_SmallSample") && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
                //检查小样订单jpg文件是否存在
                if (SmallSample_filesInfo.Count > 0)
                {
                    ViewBag.SmallSample_Picture = "Exists";
                }
                int i = 1;
                foreach (var item in SmallSample_filesInfo)
                {
                    SmallSample_json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.SmallSample_jpgjson = SmallSample_json;
            }
            #endregion

            #region----------异常订单基本信息
            //检查异常订单文件目录是否存在
            string AbnormalOrder_directory = "D:\\MES_Data\\AbnormalOrder_Files\\" + orderMgm.OrderNum;
            if (Directory.Exists(@AbnormalOrder_directory) == true)
            {
                ViewBag.AbnormalOrder_Directory = "Exists";
            }
            //检查异常订单pdf文件是否存在
            string AbnormalOrder_pdfFile = "D:\\MES_Data\\AbnormalOrder_Files\\" + orderMgm.OrderNum + "\\" + orderMgm.OrderNum + "_AbnormalOrder.pdf";
            if (System.IO.File.Exists(@AbnormalOrder_pdfFile) == true)
            {
                ViewBag.AbnormalOrder_PDf = "Exists";
            }
            List<FileInfo> AbnormalOrder_filesInfo = new List<FileInfo>();
            JObject AbnormalOrder_json = new JObject();
            if (ViewBag.AbnormalOrder_Directory == "Exists")
            {
                AbnormalOrder_filesInfo = GetAllFilesInDirectory(AbnormalOrder_directory);
                AbnormalOrder_filesInfo = AbnormalOrder_filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum + "_AbnormalOrder") && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
                //检查异常订单jpg文件是否存在
                if (AbnormalOrder_filesInfo.Count > 0)
                {
                    ViewBag.AbnormalOrder_Picture = "Exists";
                }
                int i = 1;
                foreach (var item in AbnormalOrder_filesInfo)
                {
                    AbnormalOrder_json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.AbnormalOrder_jpgjson = AbnormalOrder_json;
            }
            #endregion


            #region----------各工段异常基本信息
            #region----------组装异常基本信息
            //检查组装异常文件目录是否存在
            string AssembleAbnormalOrder_directory = "D:\\MES_Data\\AssembleAbnormalOrder_Files\\" + orderMgm.OrderNum;
            if (Directory.Exists(@AssembleAbnormalOrder_directory) == true)
            {
                ViewBag.AssembleAbnormalOrder_Directory = "Exists";
            }
            //检查组装异常pdf文件是否存在
            string AssembleAbnormalOrder_pdfFile = "D:\\MES_Data\\AssembleAbnormalOrder_Files\\" + orderMgm.OrderNum + "\\" + orderMgm.OrderNum + "_AssembleAbnormalOrder.pdf";
            if (System.IO.File.Exists(@AssembleAbnormalOrder_pdfFile) == true)
            {
                ViewBag.AssembleAbnormalOrder_PDf = "Exists";
            }
            List<FileInfo> AssembleAbnormalOrder_filesInfo = new List<FileInfo>();
            JObject AssembleAbnormalOrder_json = new JObject();
            if (ViewBag.AssembleAbnormalOrder_Directory == "Exists")
            {
                AssembleAbnormalOrder_filesInfo = GetAllFilesInDirectory(AssembleAbnormalOrder_directory);
                AssembleAbnormalOrder_filesInfo = AssembleAbnormalOrder_filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum) && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
                //检查组装异常jpg文件是否存在
                if (AssembleAbnormalOrder_filesInfo.Count > 0)
                {
                    ViewBag.AssembleAbnormalOrder_Picture = "Exists";
                }
                int i = 1;
                foreach (var item in AssembleAbnormalOrder_filesInfo)
                {
                    AssembleAbnormalOrder_json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.AssembleAbnormalOrder_jpgjson = AssembleAbnormalOrder_json;
            }
            #endregion

            #region----------老化异常基本信息
            //检查老化异常文件目录是否存在
            string BurnInAbnormalOrder_directory = "D:\\MES_Data\\BurnInAbnormalOrder_Files\\" + orderMgm.OrderNum;
            if (Directory.Exists(@BurnInAbnormalOrder_directory) == true)
            {
                ViewBag.BurnInAbnormalOrder_Directory = "Exists";
            }
            //检查老化异常pdf文件是否存在
            string BurnInAbnormalOrder_pdfFile = "D:\\MES_Data\\BurnInAbnormalOrder_Files\\" + orderMgm.OrderNum + "\\" + orderMgm.OrderNum + "_BurnInAbnormalOrder.pdf";
            if (System.IO.File.Exists(@BurnInAbnormalOrder_pdfFile) == true)
            {
                ViewBag.BurnInAbnormalOrder_PDf = "Exists";
            }
            List<FileInfo> BurnInAbnormalOrder_filesInfo = new List<FileInfo>();
            JObject BurnInAbnormalOrder_json = new JObject();
            if (ViewBag.BurnInAbnormalOrder_Directory == "Exists")
            {
                BurnInAbnormalOrder_filesInfo = GetAllFilesInDirectory(BurnInAbnormalOrder_directory);
                BurnInAbnormalOrder_filesInfo = BurnInAbnormalOrder_filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum) && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
                //检查老化异常jpg文件是否存在
                if (BurnInAbnormalOrder_filesInfo.Count > 0)
                {
                    ViewBag.BurnInAbnormalOrder_Picture = "Exists";
                }
                int i = 1;
                foreach (var item in BurnInAbnormalOrder_filesInfo)
                {
                    BurnInAbnormalOrder_json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.BurnInAbnormalOrder_jpgjson = BurnInAbnormalOrder_json;
            }
            #endregion

            #region----------包装异常基本信息
            //检查包装异常文件目录是否存在
            string AppearanceAbnormalOrder_directory = "D:\\MES_Data\\AppearanceAbnormalOrder_Files\\" + orderMgm.OrderNum;
            if (Directory.Exists(@AppearanceAbnormalOrder_directory) == true)
            {
                ViewBag.AppearanceAbnormalOrder_Directory = "Exists";
            }
            //检查包装首件pdf文件是否存在
            string AppearanceAbnormalOrder_pdfFile = "D:\\MES_Data\\AppearanceAbnormalOrder_Files\\" + orderMgm.OrderNum + "\\" + orderMgm.OrderNum + "_AppearanceAbnormalOrder.pdf";
            if (System.IO.File.Exists(@AppearanceAbnormalOrder_pdfFile) == true)
            {
                ViewBag.AppearanceAbnormalOrder_PDf = "Exists";
            }
            List<FileInfo> AppearanceAbnormalOrder_filesInfo = new List<FileInfo>();
            JObject AppearanceAbnormalOrder_json = new JObject();
            if (ViewBag.AppearanceAbnormalOrder_Directory == "Exists")
            {
                AppearanceAbnormalOrder_filesInfo = GetAllFilesInDirectory(AppearanceAbnormalOrder_directory);
                AppearanceAbnormalOrder_filesInfo = AppearanceAbnormalOrder_filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum) && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
                //检查包装异常jpg文件是否存在
                if (AppearanceAbnormalOrder_filesInfo.Count > 0)
                {
                    ViewBag.AppearanceAbnormalOrder_Picture = "Exists";
                }
                int i = 1;
                foreach (var item in AppearanceAbnormalOrder_filesInfo)
                {
                    AppearanceAbnormalOrder_json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.AppearanceAbnormalOrder_jpgjson = AppearanceAbnormalOrder_json;
            }
            #endregion

            #region----------SMT异常基本信息
            //检查包装异常文件目录是否存在
            string SMTAbnormalOrder_directory = "D:\\MES_Data\\SMTAbnormalOrder_Files\\" + orderMgm.OrderNum;
            if (Directory.Exists(@SMTAbnormalOrder_directory) == true)
            {
                ViewBag.SMTAbnormalOrder_Directory = "Exists";
            }
            //检查包装首件pdf文件是否存在
            string SMTAbnormalOrder_pdfFile = "D:\\MES_Data\\SMTAbnormalOrder_Files\\" + orderMgm.OrderNum + "\\" + orderMgm.OrderNum + "_SMTAbnormalOrder.pdf";
            if (System.IO.File.Exists(@SMTAbnormalOrder_pdfFile) == true)
            {
                ViewBag.SMTAbnormalOrder_PDf = "Exists";
            }
            List<FileInfo> SMTAbnormalOrder_filesInfo = new List<FileInfo>();
            JObject SMTAbnormalOrder_json = new JObject();
            if (ViewBag.SMTAbnormalOrder_Directory == "Exists")
            {
                SMTAbnormalOrder_filesInfo = GetAllFilesInDirectory(SMTAbnormalOrder_directory);
                SMTAbnormalOrder_filesInfo = SMTAbnormalOrder_filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum) && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
                //检查包装异常jpg文件是否存在
                if (SMTAbnormalOrder_filesInfo.Count > 0)
                {
                    ViewBag.SMTAbnormalOrder_Picture = "Exists";
                }
                int i = 1;
                foreach (var item in SMTAbnormalOrder_filesInfo)
                {
                    SMTAbnormalOrder_json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.SMTAbnormalOrder_jpgjson = SMTAbnormalOrder_json;
            }
            #endregion

            #endregion


            #region----------各工段首件基本信息
            #region----------组装首件基本信息
            //检查组装首件文件目录是否存在
            string Assemble_directory = "D:\\MES_Data\\AssembleSample_Files\\" + orderMgm.OrderNum;
            if (Directory.Exists(@Assemble_directory) == true)
            {
                ViewBag.Assemble_Directory = "Exists";
            }
            //检查组装首件pdf文件是否存在
            string Assemble_pdfFile = "D:\\MES_Data\\AssembleSample_Files\\" + orderMgm.OrderNum + "\\" + orderMgm.OrderNum + "_AssembleSample.pdf";
            if (System.IO.File.Exists(@Assemble_pdfFile) == true)
            {
                ViewBag.Assemble_PDf = "Exists";
            }
            List<FileInfo> Assemble_filesInfo = new List<FileInfo>();
            JObject Assemble_json = new JObject();
            if (ViewBag.Assemble_Directory == "Exists")
            {
                Assemble_filesInfo = GetAllFilesInDirectory(Assemble_directory);
                Assemble_filesInfo = Assemble_filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum + "_AssembleSample") && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
                //检查组装首件jpg文件是否存在
                if (Assemble_filesInfo.Count > 0)
                {
                    ViewBag.Assemble_Picture = "Exists";
                }
                int i = 1;
                foreach (var item in Assemble_filesInfo)
                {
                    Assemble_json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.Assemble_jpgjson = Assemble_json;
            }
            #endregion

            #region----------老化首件基本信息
            //检查老化首件文件目录是否存在
            string BurnIn_directory = "D:\\MES_Data\\BurnInSample_Files\\" + orderMgm.OrderNum;
            if (Directory.Exists(@BurnIn_directory) == true)
            {
                ViewBag.BurnIn_Directory = "Exists";
            }
            //检查老化首件pdf文件是否存在
            string BurnIn_pdfFile = "D:\\MES_Data\\BurnInSample_Files\\" + orderMgm.OrderNum + "\\" + orderMgm.OrderNum + "_BurnInSample.pdf";
            if (System.IO.File.Exists(@BurnIn_pdfFile) == true)
            {
                ViewBag.BurnIn_PDf = "Exists";
            }
            List<FileInfo> BurnIn_filesInfo = new List<FileInfo>();
            JObject BurnIn_json = new JObject();
            if (ViewBag.BurnIn_Directory == "Exists")
            {
                BurnIn_filesInfo = GetAllFilesInDirectory(BurnIn_directory);
                BurnIn_filesInfo = BurnIn_filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum + "_BurnInSample") && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
                //检查老化首件jpg文件是否存在
                if (BurnIn_filesInfo.Count > 0)
                {
                    ViewBag.BurnIn_Picture = "Exists";
                }
                int i = 1;
                foreach (var item in BurnIn_filesInfo)
                {
                    BurnIn_json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.BurnIn_jpgjson = BurnIn_json;
            }
            #endregion

            #region----------包装首件基本信息
            //检查包装首件文件目录是否存在
            string Appearance_directory = "D:\\MES_Data\\AppearanceSample_Files\\" + orderMgm.OrderNum;
            if (Directory.Exists(@Appearance_directory) == true)
            {
                ViewBag.Appearance_Directory = "Exists";
            }
            //检查包装首件pdf文件是否存在
            string Appearance_pdfFile = "D:\\MES_Data\\AppearanceSample_Files\\" + orderMgm.OrderNum + "\\" + orderMgm.OrderNum + "_AppearanceSample.pdf";
            if (System.IO.File.Exists(@Appearance_pdfFile) == true)
            {
                ViewBag.Appearance_PDf = "Exists";
            }
            List<FileInfo> Appearance_filesInfo = new List<FileInfo>();
            JObject Appearance_json = new JObject();
            if (ViewBag.Appearance_Directory == "Exists")
            {
                Appearance_filesInfo = GetAllFilesInDirectory(Appearance_directory);
                Appearance_filesInfo = Appearance_filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum + "_AppearanceSample") && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
                //检查包装首件jpg文件是否存在
                if (Appearance_filesInfo.Count > 0)
                {
                    ViewBag.Appearance_Picture = "Exists";
                }
                int i = 1;
                foreach (var item in Appearance_filesInfo)
                {
                    Appearance_json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.Appearance_jpgjson = Appearance_json;
            }
            #endregion
            #endregion

            return View(orderMgm);
        }
        #endregion

        #region --------------------Create页
        // GET: OrderMgms/Create
        public ActionResult Create()
        {
            ViewBag.AssembleDepartmentList = AssembleDepartmentList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Create" });
            }
            //if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || com.isCheckRole("订单管理", "创建订单", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum) || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC组长")
            //{
            return View();

            //}
            //return Content("<script>alert('对不起，您未授权管理订单，请联系PC部经理！');window.location.href='../OrderMgms/Index';</script>");
            //return RedirectToAction("Index");
        }

        // POST: OrderMgms/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OrderNum,BarCode_Prefix,CustomerName,ContractDate,DeliveryDate,PlanInputTime,PlanCompleteTime,PlatformType,Area,ProcessingRequire,StandardRequire,Capacity,CapacityQ,HandSampleScedule,AssembleDepartment,Boxes,Models,ModelsMore,Powers,PowersMore,AdapterCard,AdapterCardMore,OrderCreateDate,BarCodeCreated,BarCodeCreateDate,BarCodeCreator,CompletedRate,IsRepertory,Remark")] OrderMgm orderMgm,string Department1,string Group)
        {
            ViewBag.AssembleDepartmentList = AssembleDepartmentList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Create" });
            }
            if (db.OrderMgm.Count(c => c.OrderNum == orderMgm.OrderNum) > 0)
            {
                ModelState.AddModelError("", "此订单号已存在，如订单信息有误，请进入订单详情修改信息！");
                return View(orderMgm);
            }
            //设置条码生成状态为0，表示未生成订单条码
            orderMgm.BarCodeCreated = 0;
            var small = db.Small_Sample.Where(c => c.OrderNumber.Contains(orderMgm.OrderNum)).FirstOrDefault();
            if (small != null)
            {
                if (small.ApprovedResult == true)
                    orderMgm.HandSampleScedule = "完成";
                else
                    orderMgm.HandSampleScedule = "进行中";
            }
            else
            {
                orderMgm.HandSampleScedule = "未开始";
            }
            orderMgm.OrderCreateDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                orderMgm.Department = Department1;
                orderMgm.Group = Group;

                db.OrderMgm.Add(orderMgm);
                db.SaveChanges();
                var id = db.OrderMgm.Where(c => c.OrderNum == orderMgm.OrderNum).Select(c=>c.ID).FirstOrDefault();
                if (orderMgm.Boxes != 0)//模组
                {
                    BarCodesController.CreateModuleGroupBarCodes(id, ((Users)Session["User"]).UserName);
                }
                if (orderMgm.Models != 0)//模块
                {
                    BarCodesController.CreateModulePieceBarCodes(id, ((Users)Session["User"]).UserName); //CreateModulePieceBarCodes2();
                }
                if (orderMgm.Powers!=0)//电源
                {
                    BarCodesController.CreatePowerBarCodes(id, ((Users)Session["User"]).UserName); //CreateModulePieceBarCodes2();
                }
                if (orderMgm.AdapterCard != 0)//转接卡
                {
                    BarCodesController.CreateAdapterCardBarCodes(id, ((Users)Session["User"]).UserName); //CreateModulePieceBarCodes2();
                }
                return Content("<script>alert('订单创建成功！');window.location.href='../OrderMgms/Index';</script>");
                //return RedirectToAction("Index");
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------Edit页
        // GET: OrderMgms/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.AssembleDepartmentList = AssembleDepartmentList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Edit" + "/" + id.ToString() });
            }
            //if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || com.isCheckRole("订单管理", "修改订单", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum) || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC组长")
            //{
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            return View(orderMgm);
            //}
            //return RedirectToAction("Index", "OrderMgms");
        }

        // POST: OrderMgms/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OrderNum,BarCode_Prefix,CustomerName,ContractDate,DeliveryDate,PlanInputTime,PlanCompleteTime,PlatformType,Area,ProcessingRequire,StandardRequire,Capacity,CapacityQ,HandSampleScedule,AssembleDepartment,Boxes,Models,ModelsMore,Powers,PowersMore,AdapterCard,AdapterCardMore,OrderCreateDate,BarCodeCreated,BarCodeCreateDate,BarCodeCreator,CompletedRate,IsRepertory,Remark")] OrderMgm orderMgm)
        {
            ViewBag.AssembleDepartmentList = AssembleDepartmentList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Edit" + "/" + orderMgm.ID });
            }
            int orginal_ordermgm_BoxsNum = (from m in db.OrderMgm where m.OrderNum == orderMgm.OrderNum select m.Boxes).SingleOrDefault();
            //1.修改后的订单模组数>原订单模组数且模组条码已经生成，在BarCodes表追加相应的条码号
            if (orderMgm.Boxes > orginal_ordermgm_BoxsNum && orderMgm.BarCodeCreated == 1)
            {
                int addcount = orderMgm.Boxes - orginal_ordermgm_BoxsNum;
                var bc = db.BarCodes.Where(c => c.OrderNum == orderMgm.OrderNum && c.BarCodeType == "模组").OrderByDescending(c => c.BarCodesNum).FirstOrDefault();
                if (bc == null)
                {
                    List<BarCodes> barCodes = new List<BarCodes>();
                    //生成模组条码
                    for (int i = 1; i <= orderMgm.Boxes; i++)
                    {
                        BarCodes aBarCode = new BarCodes();
                        aBarCode.OrderNum = orderMgm.OrderNum;
                        aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
                        aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                        aBarCode.BarCodeType = "模组";
                        aBarCode.Creator = ((Users)Session["User"]).UserName;
                        aBarCode.CreateDate = DateTime.Now;
                        aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "A" + i.ToString("00000");
                        barCodes.Add(aBarCode);
                    }
                    if (com.BulkInsert<BarCodes>("BarCodes", barCodes) == "false")
                    {
                        return Content("<script>alert('模组创建失败，请确保表与模型相符');history.go(-1);</script>");
                    }
                }
                else
                {
                    BarCodes barcode = new BarCodes { OrderNum = bc.OrderNum, ToOrderNum = bc.ToOrderNum, BarCode_Prefix = bc.BarCode_Prefix, BarCodesNum = bc.BarCodesNum, BarCodeType = bc.BarCodeType, CreateDate = DateTime.Now, Creator = bc.Creator, IsRepertory = bc.IsRepertory, Remark = bc.Remark };
                    for (int i = 1; i <= addcount; i++)
                    {
                        string s = barcode.BarCodesNum.Substring(barcode.BarCodesNum.Length - 5, 5);
                        int addbarcodenum = int.Parse(s) + 1;
                        barcode.BarCodesNum = barcode.BarCode_Prefix + "A" + addbarcodenum.ToString("00000");
                        db.BarCodes.Add(barcode);
                        db.SaveChanges();
                    }
                }
            }
            ////2.修改后的订单模组数<原订单模组数
            ////  检查对应缩少的那部分条码号是否有生产记录，如果无生产记录，询问是否修改？如果有生产记录，反馈“有生产记录，不能修改！”
            ////  如果修改，修改后把BarCodes表中的缩少的那部分条码号删除
            else if (orderMgm.Boxes < orginal_ordermgm_BoxsNum && orderMgm.BarCodeCreated == 1)
            {
                var num = orginal_ordermgm_BoxsNum - orderMgm.Boxes;
                var barcodeList = db.BarCodes.OrderByDescending(c => c.BarCodesNum).Where(c => c.OrderNum == orderMgm.OrderNum && c.BarCodeType == "模组").Take(num).ToList();
                var bc = barcodeList.Select(c => c.BarCodesNum).ToList();

                string mesage = string.Empty;
                var ass = db.Assemble.Where(c => bc.Contains(c.BoxBarCode));
                if (ass.Count() > 0)
                {
                    mesage = mesage + string.Join(",", ass.ToList().Select(c => c.BoxBarCode).Distinct().ToList()) + "有组装记录";
                }
                var fin = db.FinalQC.Where(c => bc.Contains(c.BarCodesNum));
                if (fin.Count() > 0)
                {
                    mesage = mesage + string.Join(",", fin.ToList().Select(c => c.BarCodesNum).Distinct().ToList()) + "有FQC记录";
                }
                var burn = db.Burn_in.Where(c => bc.Contains(c.BarCodesNum));
                if (burn.Count() > 0)
                {
                    mesage = mesage + string.Join(",", burn.ToList().Select(c => c.BarCodesNum).Distinct().ToList()) + "有老化记录";
                }
                var cab = db.CalibrationRecord.Where(c => bc.Contains(c.BarCodesNum));
                if (cab.Count() > 0)
                {
                    mesage = mesage + string.Join(",", cab.ToList().Select(c => c.BarCodesNum).Distinct().ToList()) + "有校正记录";
                }
                var app = db.Appearance.Where(c => bc.Contains(c.BarCodesNum));
                if (app.Count() > 0)
                {
                    mesage = mesage + string.Join(",", app.ToList().Select(c => c.BarCodesNum).Distinct().ToList()) + "有包装记录";
                }

                if (!string.IsNullOrEmpty(mesage))
                {
                    ModelState.AddModelError("", mesage);
                    return View(orderMgm);
                }
                else
                {
                    int count = orginal_ordermgm_BoxsNum - orderMgm.Boxes;
                    var delete = barcodeList.OrderByDescending(c => c.BarCodesNum).Take(count);
                    db.BarCodes.RemoveRange(delete);
                    db.SaveChanges();
                }
            }
            if (ModelState.IsValid)
            {
                var old = db.OrderMgm.Find(orderMgm.ID);
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "订单信息修改：原数据订单号、条码前缀、客户名称、下单日期、出货日期、计划投入时间、计划完成时间、平台型号、地区、制程要求、标准要求、SMT产能、模组数量、模块数量、额外预留模块数量、电源数量、额外预留电源数量、转接卡、额外预留转接卡、是否库存、备注为" + old.OrderNum + "，" + old.BarCode_Prefix + "，" + old.CustomerName + "," + old.ContractDate + "，" + old.DeliveryDate + "，" + old.PlanInputTime + "," + old.PlanCompleteTime + "，" + old.PlatformType + "，" + old.Area + "," + old.ProcessingRequire + "，" + old.StandardRequire + "，" + old.CapacityQ + "," + old.Boxes + "，" + old.Models + "，" + old.ModelsMore + "," + old.Powers + "，" + old.PowersMore + "，" + old.AdapterCard + "," + old.AdapterCardMore + "，" + old.IsRepertory + "，" + old.Remark + "修改为:" + orderMgm.OrderNum + "，" + orderMgm.BarCode_Prefix + "，" + orderMgm.CustomerName + "," + orderMgm.ContractDate + "，" + orderMgm.DeliveryDate + "，" + orderMgm.PlanInputTime + "," + orderMgm.PlanCompleteTime + "，" + orderMgm.PlatformType + "，" + orderMgm.Area + "," + orderMgm.ProcessingRequire + "，" + orderMgm.StandardRequire + "，" + orderMgm.CapacityQ + "," + orderMgm.Boxes + "，" + orderMgm.Models + "，" + orderMgm.ModelsMore + "," + orderMgm.Powers + "，" + orderMgm.PowersMore + "，" + orderMgm.AdapterCard + "," + orderMgm.AdapterCardMore + "，" + orderMgm.IsRepertory + "，" + orderMgm.Remark };
                db.UserOperateLog.Add(log);
                old.OrderNum = orderMgm.OrderNum;
                old.BarCode_Prefix = orderMgm.BarCode_Prefix;
                old.CustomerName = orderMgm.CustomerName;
                old.ContractDate = orderMgm.ContractDate;
                old.DeliveryDate = orderMgm.DeliveryDate;
                old.PlanInputTime = orderMgm.PlanInputTime;
                old.PlanCompleteTime = orderMgm.PlanCompleteTime;
                old.PlatformType = orderMgm.PlatformType;
                old.Area = orderMgm.Area;
                old.ProcessingRequire = orderMgm.ProcessingRequire;
                old.StandardRequire = orderMgm.StandardRequire;
                old.CapacityQ = orderMgm.CapacityQ;
                old.Boxes = orderMgm.Boxes;
                old.Models = orderMgm.Models;
                old.ModelsMore = orderMgm.ModelsMore;
                old.Powers = orderMgm.Powers;
                old.PowersMore = orderMgm.PowersMore;
                old.AdapterCard = orderMgm.AdapterCard;
                old.AdapterCardMore = orderMgm.AdapterCardMore;
                old.IsRepertory = orderMgm.IsRepertory;
                old.Remark = orderMgm.Remark;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }

        #endregion

        #region --------------------EditForSmallSample修改小样进度页

        public ActionResult EditForSmallSample(int? id)
        {
            ViewBag.AssembleDepartmentList = AssembleDepartmentList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "EditForSmallSample" + "/" + id.ToString() });
            }
            //if (((Users)Session["User"]).Role == "小样调试员" || com.isCheckRole("订单管理", "修改小样进度", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            ViewBag.HandSampleScedule = SmallSampleScedule();
            ViewBag.SmallSampleSceduleValue = orderMgm.HandSampleScedule;
            return View(orderMgm);
            //}
            //else{

            //  }
            //     return Content("<script>alert('对不起，您的不能管理小样进度，请联系技术部经理！');history.go(-1);</script>");

            //return RedirectToAction("Index", "OrderMgms");
        }


        // POST: OrderMgms/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditForSmallSample([Bind(Include = "ID,OrderNum,BarCode_Prefix,CustomerName,ContractDate,DeliveryDate,PlanInputTime,PlanCompleteTime,PlatformType,Area,ProcessingRequire,StandardRequire,Capacity,CapacityQ,HandSampleScedule,AssembleDepartment,Boxes,Models,ModelsMore,Powers,PowersMore,AdapterCard,AdapterCardMore,OrderCreateDate,BarCodeCreated,BarCodeCreateDate,BarCodeCreator,CompletedRate,IsRepertory,Remark")] OrderMgm orderMgm)
        {
            ViewBag.AssembleDepartmentList = AssembleDepartmentList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "EditForSmallSample" + "/" + orderMgm.ID.ToString() });
            }
            ViewBag.HandSampleScedule = SmallSampleScedule();
            ViewBag.SmallSampleSceduleValue = orderMgm.HandSampleScedule;
            if (ModelState.IsValid)
            {
                var old = db.OrderMgm.Find(orderMgm.ID);
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "小样状态修改：原数据订单号、小样状态为" + old.OrderNum + "，" + old.HandSampleScedule + "修改为" + orderMgm.OrderNum + "," + orderMgm.HandSampleScedule };
                db.UserOperateLog.Add(log);

                old.HandSampleScedule = orderMgm.HandSampleScedule;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------DeleteOrderNum方法

        /// <summary>
        /// 1.在订单Detail页面增加一个删除按钮，指向删除订单方法
        /// 2.删除订单方法：检查订单是否有扫码记录，
        ///   如果没有，可以删除，删除订单的同时，也删除订单对应的条码记录，删除完成后提示“已经删除订单和条码”
        ///   如果有扫码记录，提示不允许删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // assemble_begintime
        [HttpPost]
        public ActionResult DeleteOrderNum(string orderNum)
        {
            //取出订单
            var Record = db.OrderMgm.Where(c => c.OrderNum == orderNum).FirstOrDefault();
            if (Record != null)
            {
                //计算订单在组装、FQC、老化、校正、包装的记录条数、SMT生产记录
                var assembleCount = db.Assemble.Count(c => c.OrderNum == orderNum);
                var fqcCount = db.FinalQC.Count(c => c.OrderNum == orderNum);
                var burn_inCount = db.Burn_in.Count(c => c.OrderNum == orderNum);
                var calibrationCount = db.CalibrationRecord.Count(c => c.OrderNum == orderNum);
                var appearancesCount = db.Appearance.Count(c => c.OrderNum == orderNum);
                var smtCount = db.SMT_ProductionData.Count(c => c.OrderNum == orderNum);
                //如果订单在组装、FQC、老化、校正、包装的记录条数、SMT生产记录都为0，则删除订单信息和条码信息
                if (assembleCount == 0 && fqcCount == 0 && burn_inCount == 0 && calibrationCount == 0 && appearancesCount == 0 && smtCount == 0)
                {
                    //取出订单对应的条码
                    string sqlstring = "DELETE FROM BarCodes WHERE OrderNum='" + orderNum+"'";
                    var sqlresult = com.SQLAloneExecute(sqlstring);
                    if (sqlresult != "true")
                    {
                        return Content("<script>alert('"+ sqlresult + "！');window.location.href='../OrderMgms/Index';</script>");
                    }
                    
                    //删除SMT计划信息
                    var smtPlanList = db.SMT_ProductionPlan.Where(c => c.OrderNum == orderNum).ToList();
                    if (smtPlanList != null)
                    {
                        //删除SMT计划信息
                        foreach (var plan in smtPlanList)
                        {
                            db.SMT_ProductionPlan.Remove(plan);
                            db.SaveChanges();
                        }
                    }
                    //删除订单信息
                    db.OrderMgm.Remove(Record);
                    db.SaveChanges();
                    //保存删除信息
                    OrderMgm_Delete orderMgm_delete = new OrderMgm_Delete();
                    orderMgm_delete.OrderNum = Record.OrderNum;
                    orderMgm_delete.DeleteDate = DateTime.Now;
                    orderMgm_delete.Deleter = ((Users)Session["User"]).UserName;
                    db.OrderMgm_Delete.Add(orderMgm_delete);
                    db.SaveChanges();
                    return Content("<script>alert('你已删除订单和条码！');window.location.href='../OrderMgms/Index';</script>");
                }
                //如果订单在组装、FQC、老化、校正、包装的记录条数、SMT生产记录其中一个不为0，则返回提示信息
                else
                {
                    return Content("<script>alert('订单号" + orderNum + "有生产记录,不能删除此订单！');history.go(-1);</script>");
                }
            }
            else
            {
                return Content("<script>alert('订单不存在！');window.location.href='../OrderMgms/Index';</script>");
            }
        }

        #endregion

        #region --------------------Delete页
        // GET: OrderMgms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Delete" + "/" + id.ToString() });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            return View(orderMgm);
        }

        // POST: OrderMgms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            db.OrderMgm.Remove(orderMgm);
            db.SaveChanges();
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

        #region -------------------- 异常操作

        #region --------------------组装异常

        #region --------------------转为组装异常订单
        [HttpPost]
        public ActionResult AssembleAbnormalOrderConvert(int? id, string AssembleAbnormalOrder_Description)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Details" + "/" + id.ToString() });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            orderMgm.IsAssembleAbnormal = true;
            orderMgm.AssembleAbnormalConvertDate = DateTime.Now;
            orderMgm.AssembleAbnormalConverter = ((Users)Session["User"]).UserName;
            orderMgm.AssembleAbnormal_Description = AssembleAbnormalOrder_Description;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------上传组装异常单文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadAssembleAbnormalOrder(int id, string ordernum, string appNUM)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadAssembleAbnormalOrder"];
                var index = file.FileName.LastIndexOf('.');
                var fileType = file.FileName.Substring(index, file.FileName.Length - index).ToLower();
                if (Directory.Exists(@"D:\MES_Data\AssembleAbnormalOrder_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\AssembleAbnormalOrder_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\AssembleAbnormalOrder_Files\" + ordernum + "\\");
                //文件为jpg类型,并且appNUM为空
                if (fileType == ".jpg")
                {
                    if (string.IsNullOrEmpty(appNUM))
                    {
                        int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AssembleAbnormalOrder") && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\AssembleAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_AssembleAbnormalOrder" + (jpg_count + 1) + fileType);
                    }
                    else
                        file.SaveAs(@"D:\MES_Data\AssembleAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_" + appNUM + "_AssembleAbnormalOrder" + fileType);
                }
                else if (fileType == ".jpeg")
                {
                    if (string.IsNullOrEmpty(appNUM))
                    {
                        int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AssembleAbnormalOrder") && c.Name.Substring(c.Name.Length - 5, 5) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\AssembleAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_AssembleAbnormalOrder" + (jpg_count + 1) + fileType);
                    }
                    else
                        file.SaveAs(@"D:\MES_Data\AssembleAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_" + appNUM + "_AssembleAbnormalOrder" + fileType);
                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\AssembleAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_AssembleAbnormalOrder" + fileType);
                }
                return RedirectToAction("Details", "OrderMgms", new { id = id });
            }
            return View();
        }
        #endregion

        #region --------------------下载组装异常订单pdf文件
        [HttpPost]
        public ActionResult GetAssembleAbnormalOrderPDF(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AssembleAbnormalOrder_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此组装异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_AssembleAbnormalOrder.pdf").Count() > 0)
            {
                address = "/MES_Data/AssembleAbnormalOrder_Files" + "/" + ordernum + "/" + ordernum + "_AssembleAbnormalOrder.pdf";
            }
            else
            {
                return Content("此组装异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            return Content(address);
        }
        #endregion

        #region --------------------下载组装异常订单图片预览
        [HttpPost]
        public ActionResult GetAssembleAbnormalOrderImg(string ordernum)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\AssembleAbnormalOrder_Files\\" + ordernum + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum) && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
            JObject json = new JObject();
            int i = 1;
            foreach (var item in filesInfo)
            {
                json.Add(i.ToString(), item.Name);
                i++;
            }
            ViewBag.Assemblejpgjson = json;
            if (filesInfo.Count > 0)
            {
                return Content(json.ToString());
            }
            else
            {
                return Content("此组装异常单订单图片尚未上传！");
            }
        }
        #endregion

        #region --------------------查看组装异常订单pdf文档页面
        public ActionResult AssembleAbnormalOrder_pdf(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AssembleAbnormalOrder_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此组装异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            //List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_AssembleAbnormalOrder.pdf").Count() > 0)
            {
                address = "~/Scripts/pdf.js/web/viewer.html?file=\\MES_Data\\AssembleAbnormalOrder_Files\\" + ordernum + "\\" + ordernum + "_AssembleAbnormalOrder.pdf";
            }
            else
            {
                return Content("此组装异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            ViewBag.Assembleaddress = address;
            ViewBag.Assembleordernum = ordernum;
            return Redirect(address);
        }
        #endregion

        #endregion



        #region --------------------老化异常

        #region --------------------转为老化异常订单
        [HttpPost]
        public ActionResult BurnInAbnormalOrderConvert(int? id, string BurnInAbnormalOrder_Description)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Details" + "/" + id.ToString() });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            orderMgm.IsBurninAbnormal = true;
            orderMgm.BurninAbnormalConvertDate = DateTime.Now;
            orderMgm.BurninAbnormalConverter = ((Users)Session["User"]).UserName;
            orderMgm.BurninAbnormal_Description = BurnInAbnormalOrder_Description;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------上传老化异常单文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadBurnInAbnormalOrder(int id, string ordernum, string appNUM)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadBurnInAbnormalOrder"];
                var index = file.FileName.LastIndexOf('.');
                var fileType = file.FileName.Substring(index, file.FileName.Length - index).ToLower();
                //string ReName = ordernum + "_BurnInAbnormalOrder";
                if (Directory.Exists(@"D:\MES_Data\BurnInAbnormalOrder_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\BurnInAbnormalOrder_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\BurnInAbnormalOrder_Files\" + ordernum + "\\");
                //文件为jpg类型
                if (fileType == ".jpg")
                {
                    if (string.IsNullOrEmpty(appNUM))
                    {
                        int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_BurnInAbnormalOrder") && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\BurnInAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_BurnInAbnormalOrder" + (jpg_count + 1) + fileType);
                    }
                    else
                        file.SaveAs(@"D:\MES_Data\BurnInAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_" + appNUM + "_BurnInAbnormalOrder" + fileType);

                }
                else if (fileType == ".jpeg")
                {
                    if (string.IsNullOrEmpty(appNUM))
                    {
                        int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_BurnInAbnormalOrder") && c.Name.Substring(c.Name.Length - 5, 5) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\BurnInAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_BurnInAbnormalOrder" + (jpg_count + 1) + fileType);
                    }
                    else
                        file.SaveAs(@"D:\MES_Data\BurnInAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_" + appNUM + "_BurnInAbnormalOrder" + fileType);
                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\BurnInAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_BurnInAbnormalOrder" + fileType);
                }
                return RedirectToAction("Details", "OrderMgms", new { id = id });
            }
            return View();
        }
        #endregion

        #region --------------------下载老化异常订单pdf文件
        [HttpPost]
        public ActionResult GetBurnInAbnormalOrderPDF(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\BurnInAbnormalOrder_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此老化异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_BurnInAbnormalOrder.pdf").Count() > 0)
            {
                address = "/MES_Data/BurnInAbnormalOrder_Files" + "/" + ordernum + "/" + ordernum + "_BurnInAbnormalOrder.pdf";
            }
            else
            {
                return Content("此老化异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            return Content(address);
        }
        #endregion

        #region --------------------下载老化异常订单图片预览
        [HttpPost]
        public ActionResult GetBurnInAbnormalOrderImg(string ordernum)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\BurnInAbnormalOrder_Files\\" + ordernum + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum) && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
            JObject json = new JObject();
            int i = 1;
            foreach (var item in filesInfo)
            {
                json.Add(i.ToString(), item.Name);
                i++;
            }
            ViewBag.Assemblejpgjson = json;
            if (filesInfo.Count > 0)
            {
                return Content(json.ToString());
            }
            else
            {
                return Content("此老化异常单订单图片尚未上传！");
            }
        }
        #endregion

        #region --------------------查看老化异常订单pdf文档页面
        public ActionResult BurnInAbnormalOrder_pdf(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\BurnInAbnormalOrder_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此老化异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            //List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_BurnInAbnormalOrder.pdf").Count() > 0)
            {
                address = "~/Scripts/pdf.js/web/viewer.html?file=\\MES_Data\\BurnInAbnormalOrder_Files\\" + ordernum + "\\" + ordernum + "_BurnInAbnormalOrder.pdf";
            }
            else
            {
                return Content("此老化异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            ViewBag.BurnInaddress = address;
            ViewBag.BurnInordernum = ordernum;
            return Redirect(address);
        }
        #endregion

        #endregion



        #region --------------------包装异常
        #region --------------------转为包装异常订单
        [HttpPost]
        public ActionResult AppearanceAbnormalOrderConvert(int? id, string AppearanceAbnormalOrder_Description)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Details" + "/" + id.ToString() });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            orderMgm.IsAppearanceAbnormal = true;
            orderMgm.AppearanceAbnormalConvertDate = DateTime.Now;
            orderMgm.AppearanceAbnormalConverter = ((Users)Session["User"]).UserName;
            orderMgm.AppearanceAbnormal_Description = AppearanceAbnormalOrder_Description;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------上传包装异常单文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadAppearanceAbnormalOrder(int id, string ordernum, string appNUM)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadAppearanceAbnormalOrder"];
                var index = file.FileName.LastIndexOf('.');
                var fileType = file.FileName.Substring(index, file.FileName.Length - index).ToLower();
                //string ReName = ordernum + "_AppearanceAbnormalOrder";
                if (Directory.Exists(@"D:\MES_Data\AppearanceAbnormalOrder_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\AppearanceAbnormalOrder_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\AppearanceAbnormalOrder_Files\" + ordernum + "\\");
                if (fileType == ".jpg")
                {
                    //文件为jpg类型
                    if (string.IsNullOrEmpty(appNUM))
                    {
                        int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AppearanceAbnormalOrder") && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\AppearanceAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_AppearanceAbnormalOrder" + (jpg_count + 1) + fileType);
                    }
                    else
                        file.SaveAs(@"D:\MES_Data\AppearanceAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_" + appNUM + "_AppearanceAbnormalOrder" + fileType);

                }
                else if (fileType == ".jpeg")
                {
                    if (string.IsNullOrEmpty(appNUM))
                    {
                        int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AppearanceAbnormalOrder") && c.Name.Substring(c.Name.Length - 5, 5) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\AppearanceAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_AppearanceAbnormalOrder" + (jpg_count + 1) + fileType);
                    }
                    else
                        file.SaveAs(@"D:\MES_Data\AppearanceAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_" + appNUM + "_AppearanceAbnormalOrder" + fileType);
                }

                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\AppearanceAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_AppearanceAbnormalOrder" + fileType);
                }
                return RedirectToAction("Details", "OrderMgms", new { id = id });
            }
            return View();
        }
        #endregion

        #region --------------------下载包装异常订单pdf文件
        [HttpPost]
        public ActionResult GetAppearanceAbnormalOrderPDF(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AppearanceAbnormalOrder_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此包装异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_AppearanceAbnormalOrder.pdf").Count() > 0)
            {
                address = "/MES_Data/AppearanceAbnormalOrder_Files" + "/" + ordernum + "/" + ordernum + "_AppearanceAbnormalOrder.pdf";
            }
            else
            {
                return Content("此包装异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            return Content(address);
        }
        #endregion

        #region --------------------下载老化异常订单图片预览
        [HttpPost]
        public ActionResult GetAppearanceAbnormalOrderImg(string ordernum)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\AppearanceAbnormalOrder_Files\\" + ordernum + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum) && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
            JObject json = new JObject();
            int i = 1;
            foreach (var item in filesInfo)
            {
                json.Add(i.ToString(), item.Name);
                i++;
            }
            ViewBag.Assemblejpgjson = json;
            if (filesInfo.Count > 0)
            {
                return Content(json.ToString());
            }
            else
            {
                return Content("此包装异常单订单图片尚未上传！");
            }
        }
        #endregion

        #region --------------------查看包装异常订单pdf文档页面
        public ActionResult AppearanceAbnormalOrder_pdf(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AppearanceAbnormalOrder_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此包装异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_AppearanceAbnormalOrder.pdf").Count() > 0)
            {
                address = "~/Scripts/pdf.js/web/viewer.html?file=\\MES_Data\\AppearanceAbnormalOrder_Files\\" + ordernum + "\\" + ordernum + "_AppearanceAbnormalOrder.pdf";
            }
            else
            {
                return Content("此老包装常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            ViewBag.Appearanceaddress = address;
            ViewBag.Appearanceordernum = ordernum;
            return Redirect(address);
        }
        #endregion

        #endregion

        #region --------------------smt异常

        #region --------------------转为包装异常订单
        [HttpPost]
        public ActionResult SMTAbnormalOrderConvert(int? id, string SMTAbnormalOrder_Description)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Details" + "/" + id.ToString() });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            orderMgm.IsSMTAbnormal = true;
            orderMgm.SMTAbnormalConvertDate = DateTime.Now;
            orderMgm.SMTAbnormalConverter = ((Users)Session["User"]).UserName;
            orderMgm.SMTAbnormal_Description = SMTAbnormalOrder_Description;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------上传包装异常单文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadSMTAbnormalOrder(int id, string ordernum, string appNUM)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadSMTAbnormalOrder"];
                var index = file.FileName.LastIndexOf('.');
                var fileType = file.FileName.Substring(index, file.FileName.Length - index).ToLower();
                //string ReName = ordernum + "_AppearanceAbnormalOrder";
                if (Directory.Exists(@"D:\MES_Data\SMTAbnormalOrder_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\SMTAbnormalOrder_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\SMTAbnormalOrder_Files\" + ordernum + "\\");
                if (fileType == ".jpg")
                {
                    //文件为jpg类型
                    if (string.IsNullOrEmpty(appNUM))
                    {
                        int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_SMTAbnormalOrder") && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\SMTAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_SMTAbnormalOrder" + (jpg_count + 1) + fileType);
                    }
                    else
                        file.SaveAs(@"D:\MES_Data\SMTAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_" + appNUM + "_SMTAbnormalOrder" + fileType);

                }
                else if (fileType == ".jpeg")
                {
                    if (string.IsNullOrEmpty(appNUM))
                    {
                        int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_SMTAbnormalOrder") && c.Name.Substring(c.Name.Length - 5, 5) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\SMTAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_SMTAbnormalOrder" + (jpg_count + 1) + fileType);
                    }
                    else
                        file.SaveAs(@"D:\MES_Data\SMTAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_" + appNUM + "_SMTAbnormalOrder" + fileType);
                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\SMTAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_SMTAbnormalOrder" + fileType);
                }
                return RedirectToAction("Details", "OrderMgms", new { id = id });
            }
            return View();
        }
        #endregion

        #region --------------------下载包装异常订单pdf文件
        [HttpPost]
        public ActionResult GetSMTAbnormalOrderPDF(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\SMTAbnormalOrder_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此SMT异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_SMTAbnormalOrder.pdf").Count() > 0)
            {
                address = "/MES_Data/SMTAbnormalOrder_Files" + "/" + ordernum + "/" + ordernum + "_SMTAbnormalOrder.pdf";
            }
            else
            {
                return Content("此SMT异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            return Content(address);
        }
        #endregion

        #region --------------------下载老化异常订单图片预览
        [HttpPost]
        public ActionResult GetSMTAbnormalOrderImg(string ordernum)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\SMTAbnormalOrder_Files\\" + ordernum + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum) && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
            JObject json = new JObject();
            int i = 1;
            foreach (var item in filesInfo)
            {
                json.Add(i.ToString(), item.Name);
                i++;
            }
            ViewBag.Assemblejpgjson = json;
            if (filesInfo.Count > 0)
            {
                return Content(json.ToString());
            }
            else
            {
                return Content("此SMT异常单订单图片尚未上传！");
            }
        }
        #endregion

        #region --------------------查看包装异常订单pdf文档页面
        public ActionResult SMTAbnormalOrder_pdf(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\SMTAbnormalOrder_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此SMT异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_SMTAbnormalOrder.pdf").Count() > 0)
            {
                address = "~/Scripts/pdf.js/web/viewer.html?file=\\MES_Data\\SMTAbnormalOrder_Files\\" + ordernum + "\\" + ordernum + "_SMTAbnormalOrder.pdf";
            }
            else
            {
                return Content("此老SMT常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            ViewBag.SMTaddress = address;
            ViewBag.SMTordernum = ordernum;
            return Redirect(address);
        }
        #endregion
        #endregion       

        //以前异常订单
        #region --------------------转为异常订单
        [HttpPost]
        public ActionResult AbnormalOrderConvert(int? id, string AbnormalOrder_Description)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Details" + "/" + id.ToString() });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            orderMgm.IsAbnormalOrder = true;
            orderMgm.AbnormalOrderConvertDate = DateTime.Now;
            orderMgm.AbnormalOrderConverter = ((Users)Session["User"]).UserName;
            orderMgm.AbnormalOrder_Description = AbnormalOrder_Description;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------上传异常单文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadAbnormalOrder(int id, string ordernum)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadAbnormalOrder"];
                var index = file.FileName.LastIndexOf('.');
                var fileType = file.FileName.Substring(index, file.FileName.Length - index).ToLower();
                string ReName = ordernum + "_AbnormalOrder";
                if (Directory.Exists(@"D:\MES_Data\AbnormalOrder_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\AbnormalOrder_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\AbnormalOrder_Files\" + ordernum + "\\");
                //文件为jpg类型
                if (fileType == ".jpg")
                {
                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AbnormalOrder") && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                    file.SaveAs(@"D:\MES_Data\AbnormalOrder_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);
                }
                else if (fileType == ".jpeg")
                {

                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AbnormalOrder") && c.Name.Substring(c.Name.Length - 5, 5) == fileType).Count();
                    file.SaveAs(@"D:\MES_Data\AbnormalOrder_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);

                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\AbnormalOrder_Files\" + ordernum + "\\" + ReName + fileType);
                }
                return RedirectToAction("Details", "OrderMgms", new { id = id });
            }
            return View();
        }
        #endregion

        #region --------------------下载异常订单pdf文件
        [HttpPost]
        public ActionResult GetAbnormalOrderPDF(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AbnormalOrder_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_AbnormalOrder.pdf").Count() > 0)
            {
                address = "/MES_Data/AbnormalOrder_Files" + "/" + ordernum + "/" + ordernum + "_AbnormalOrder.pdf";
            }
            else
            {
                return Content("此异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            return Content(address);
        }
        #endregion

        #region --------------------下载异常订单图片预览
        [HttpPost]
        public ActionResult GetAbnormalOrderImg(string ordernum)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\AbnormalOrder_Files\\" + ordernum + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum + "_AbnormalOrder") && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
            JObject json = new JObject();
            int i = 1;
            foreach (var item in filesInfo)
            {
                json.Add(i.ToString(), item.Name);
                i++;
            }
            ViewBag.jpgjson = json;
            if (filesInfo.Count > 0)
            {
                return Content(json.ToString());
            }
            else
            {
                return Content("此异常单订单图片尚未上传！");
            }
        }
        #endregion

        #region --------------------查看异常订单pdf文档页面
        public ActionResult AbnormalOrder_pdf(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AbnormalOrder_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            //List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_AbnormalOrder.pdf").Count() > 0)
            {
                address = "~/Scripts/pdf.js/web/viewer.html?file=\\MES_Data\\AbnormalOrder_Files\\" + ordernum + "\\" + ordernum + "_AbnormalOrder.pdf";
            }
            else
            {
                return Content("此异常单pdf版文件尚未上传，无pdf文件可下载！");
            }
            ViewBag.address = address;
            ViewBag.ordernum = ordernum;
            return Redirect(address);
        }
        #endregion
        #endregion


        #region -------------------- 特采操作
        #region --------------------转为特采订单
        [HttpPost]
        public ActionResult AODConvert(int? id, string AOD_Description)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Details" + "/" + id.ToString() });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            orderMgm.IsAOD = true;
            orderMgm.AODConvertDate = DateTime.Now;
            orderMgm.AODConverter = ((Users)Session["User"]).UserName;
            orderMgm.AOD_Description = AOD_Description;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------上传特采文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadFile(int id, string ordernum)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadfile"];
                var index = file.FileName.LastIndexOf('.');
                var fileType = file.FileName.Substring(index, file.FileName.Length - index).ToLower();
                string ReName = ordernum + "_AOD";
                if (Directory.Exists(@"D:\MES_Data\AOD_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\AOD_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\AOD_Files\" + ordernum + "\\");
                //文件为jpg类型
                if (fileType == ".jpg")
                {
                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AOD") && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                    file.SaveAs(@"D:\MES_Data\AOD_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);
                }
                else if (fileType == ".jpeg")
                {

                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AOD") && c.Name.Substring(c.Name.Length - 5, 5) == fileType).Count();
                    file.SaveAs(@"D:\MES_Data\AOD_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);

                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\AOD_Files\" + ordernum + "\\" + ReName + fileType);
                }
                return RedirectToAction("Details", "OrderMgms", new { id = id });
            }
            return View();
        }
        #endregion

        #region --------------------下载特采订单pdf文件
        [HttpPost]
        public ActionResult GetPDF(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此特采单pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_AOD.pdf").Count() > 0)
            {
                address = "/MES_Data/AOD_Files" + "/" + ordernum + "/" + ordernum + "_AOD.pdf";
            }
            else
            {
                return Content("此特采单pdf版文件尚未上传，无pdf文件可下载！");
            }
            return Content(address);
        }
        #endregion

        #region --------------------下载特采订单图片预览
        [HttpPost]
        public ActionResult GetImg(string ordernum)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\AOD_Files\\" + ordernum + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum + "_AOD") && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
            JObject json = new JObject();
            int i = 1;
            foreach (var item in filesInfo)
            {
                json.Add(i.ToString(), item.Name);
                i++;
            }
            ViewBag.jpgjson = json;
            if (filesInfo.Count > 0)
            {
                return Content(json.ToString());
            }
            else
            {
                return Content("此特采单订单图片尚未上传！");
            }
        }
        #endregion

        #region --------------------查看特采订单pdf文档页面
        public ActionResult preview_pdf(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("<script>alert('此特采单pdf版文件尚未上传，无pdf文件可下载！');history.back(-1);</script>");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_AOD.pdf").Count() > 0)
            {
                address = "~/Scripts/pdf.js/web/viewer.html?file=\\MES_Data\\AOD_Files\\" + ordernum + "\\" + ordernum + "_AOD.pdf";
            }
            else
            {
                return Content("此特采单pdf版文件尚未上传，无pdf文件可下载！");
            }
            ViewBag.address = address;
            ViewBag.ordernum = ordernum;
            return Redirect(address);
        }
        #endregion
        #endregion


        #region -------------------- 小样操作
        #region --------------------上传小样文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadSmallSample(int id, string ordernum)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadfile"];
                var index = file.FileName.LastIndexOf('.');
                var fileType = file.FileName.Substring(index, file.FileName.Length - index).ToLower();
                var re = String.Equals(fileType, ".jpg") == true || String.Equals(fileType, ".jpeg") || String.Equals(fileType, ".pdf") == true ? false : true;
                if (re)
                {
                    return Content("<script>alert('您选择文件的文件类型不正确，请选择jpg、jpeg或pdf类型文件！');history.go(-1);</script>");
                }
                string ReName = ordernum + "_SmallSample";
                if (Directory.Exists(@"D:\MES_Data\SmallSample_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\SmallSample_Files\" + ordernum + "\\");
                }
                file.SaveAs(@"D:\MES_Data\SmallSample_Files\" + ordernum + "\\" + ReName + fileType);
                return Content("<script>alert('上传成功！');window.location.href='../OrderMgms/Details/" + id + "';</script>");
            }
            return Content("<script>alert('上传失败');history.go(-1);</script>");
        }
        #endregion

        #region --------------------查看小样订单图片预览
        [HttpPost]
        public ActionResult GetSmallSampleImg(string ordernum)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\SmallSample_Files\\" + ordernum + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum + "_SmallSample") && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
            JObject json = new JObject();
            int i = 1;
            if (filesInfo.Count() > 0)
            {
                foreach (var item in filesInfo)
                {
                    json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.jpgjson = json;
                return Content(json.ToString());
            }
            else
            {
                return Content("图片文档未上传或不存在！");
            }
        }
        #endregion

        #region --------------------查看小样单pdf文档页面

        [HttpPost]
        public ActionResult preview_SmallSample_pdf(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\SmallSample_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("pdf文档未上传或不存在！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_SmallSample.pdf").Count() > 0)
            {
                address = "/MES_Data/SmallSample_Files" + "/" + ordernum + "/" + ordernum + "_SmallSample.pdf";
            }
            else
            {
                return Content("pdf文档未上传或不存在！");
            }
            return Content(address);
        }
        #endregion
        #endregion


        #region -------------------- 首件操作

        #region -------------------- 组装首件
        #region -------------------- 订单组装首件
        [HttpPost]
        public ActionResult AssembleSampleConvert(int? id, string AssembleSample_Description)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Details" + "/" + id.ToString() });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            orderMgm.IsAssembleFirstSample = true;
            orderMgm.AssembleFirstSampleDate = DateTime.Now;
            orderMgm.AssembleFirstSampleConverter = ((Users)Session["User"]).UserName;
            orderMgm.AssembleFirstSample_Description = AssembleSample_Description;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------上传组装首件文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadAssembleSampleFile(int id, string ordernum)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadAssembleSamplefile"];
                var index = file.FileName.LastIndexOf('.');
                var fileType = file.FileName.Substring(index, file.FileName.Length - index).ToLower();
                string ReName = ordernum + "_AssembleSample";
                if (Directory.Exists(@"D:\MES_Data\AssembleSample_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\AssembleSample_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\AssembleSample_Files\" + ordernum + "\\");
                //文件为jpg类型
                if (fileType == ".jpg")
                {
                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AssembleSample") && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                    file.SaveAs(@"D:\MES_Data\AssembleSample_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);
                }
                else if (fileType == ".jpeg")
                {

                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AssembleSample") && c.Name.Substring(c.Name.Length - 5, 5) == fileType).Count();
                    file.SaveAs(@"D:\MES_Data\AssembleSample_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);

                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\AssembleSample_Files\" + ordernum + "\\" + ReName + fileType);
                }
                return RedirectToAction("Details", "OrderMgms", new { id = id });
            }
            return View();
        }
        #endregion

        #region --------------------查看组装首件图片预览
        [HttpPost]
        public ActionResult GetAssembleSampleImg(string ordernum)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\AssembleSample_Files\\" + ordernum + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum + "_AssembleSample") && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
            JObject json = new JObject();
            int i = 1;
            if (filesInfo.Count() > 0)
            {
                foreach (var item in filesInfo)
                {
                    json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.AssembleSample_jpgjson = json;
                return Content(json.ToString());
            }
            else
            {
                return Content("图片文档未上传或不存在！");
            }
        }
        #endregion

        #region --------------------查看组装首件pdf文档页面
        public ActionResult preview_AssembleSample_pdf(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AssembleSample_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此组装首件pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_AssembleSample.pdf").Count() > 0)
            {
                address = "/MES_Data/AssembleSample_Files" + "/" + ordernum + "/" + ordernum + "_AssembleSample.pdf";
            }
            else
            {
                return Content("此组装首件pdf版文件尚未上传，无pdf文件可下载！");
            }
            return Content(address);
        }
        #endregion

        #endregion

        #region -------------------- 老化首件
        #region -------------------- 订单老化首件
        [HttpPost]
        public ActionResult BurnInSampleConvert(int? id, string BurnIn_Description)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Details" + "/" + id.ToString() });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            orderMgm.IsBurnInFirstSample = true;
            orderMgm.BurnInFirstSampleDate = DateTime.Now;
            orderMgm.BurnInFirstSampleConverter = ((Users)Session["User"]).UserName;
            orderMgm.BurnInFirstSample_Description = BurnIn_Description;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------上传老化首件文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadBurnInSampleFile(int id, string ordernum)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadBurnInSamplefile"];
                var index = file.FileName.LastIndexOf('.');
                var fileType = file.FileName.Substring(index, file.FileName.Length - index).ToLower();
                string ReName = ordernum + "_BurnInSample";
                if (Directory.Exists(@"D:\MES_Data\BurnInSample_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\BurnInSample_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\BurnInSample_Files\" + ordernum + "\\");
                //文件为jpg类型
                if (fileType == ".jpg")
                {
                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_BurnInSample") && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                    file.SaveAs(@"D:\MES_Data\BurnInSample_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);
                }
                else if (fileType == ".jpeg")
                {

                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_BurnInSample") && c.Name.Substring(c.Name.Length - 5, 5) == fileType).Count();
                    file.SaveAs(@"D:\MES_Data\BurnInSample_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);

                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\BurnInSample_Files\" + ordernum + "\\" + ReName + fileType);
                }
                return RedirectToAction("Details", "OrderMgms", new { id = id });
            }
            return View();
        }
        #endregion

        #region --------------------查看老化首件图片预览
        [HttpPost]
        public ActionResult GetBurnInSampleImg(string ordernum)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\BurnInSample_Files\\" + ordernum + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum + "_BurnInSample") && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
            JObject json = new JObject();
            int i = 1;
            if (filesInfo.Count() > 0)
            {
                foreach (var item in filesInfo)
                {
                    json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.BurnInSample_jpgjson = json;
                return Content(json.ToString());
            }
            else
            {
                return Content("图片文档未上传或不存在！");
            }
        }
        #endregion

        #region --------------------查看老化首件pdf文档页面
        public ActionResult preview_BurnInSample_pdf(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\BurnInSample_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此老化首件pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_BurnInSample.pdf").Count() > 0)
            {
                address = "/MES_Data/BurnInSample_Files" + "/" + ordernum + "/" + ordernum + "_BurnInSample.pdf";
            }
            else
            {
                return Content("此老化首件pdf版文件尚未上传，无pdf文件可下载！");
            }
            return Content(address);
        }
        #endregion

        #endregion

        #region -------------------- 包装首件

        #region -------------------- 订单包装首件
        [HttpPost]
        public ActionResult AppearanceSampleConvert(int? id, string Appearance_Description)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Details" + "/" + id.ToString() });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            orderMgm.IsAppearanceFirstSample = true;
            orderMgm.AppearanceFirstSampleDate = DateTime.Now;
            orderMgm.AppearanceFirstSampleConverter = ((Users)Session["User"]).UserName;
            orderMgm.AppearanceFirstSample_Description = Appearance_Description;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------上传包装首件文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadAppearanceSampleFile(int id, string ordernum)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadAppearanceSamplefile"];
                var index = file.FileName.LastIndexOf('.');
                var fileType = file.FileName.Substring(index, file.FileName.Length - index).ToLower();
                string ReName = ordernum + "_AppearanceSample";
                if (Directory.Exists(@"D:\MES_Data\AppearanceSample_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\AppearanceSample_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\AppearanceSample_Files\" + ordernum + "\\");
                //文件为jpg类型
                if (fileType == ".jpg")
                {
                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AppearanceSample") && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                    file.SaveAs(@"D:\MES_Data\AppearanceSample_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);
                }
                else if (fileType == ".jpeg")
                {

                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AppearanceSample") && c.Name.Substring(c.Name.Length - 5, 5) == fileType).Count();
                    file.SaveAs(@"D:\MES_Data\AppearanceSample_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);

                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\AppearanceSample_Files\" + ordernum + "\\" + ReName + fileType);
                }
                return RedirectToAction("Details", "OrderMgms", new { id = id });
            }
            return View();
        }
        #endregion

        #region --------------------查看组装首件图片预览
        [HttpPost]
        public ActionResult GetAppearanceSampleImg(string ordernum)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\AppearanceSample_Files\\" + ordernum + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum + "_AppearanceSample") && (c.Name.Substring(c.Name.Length - 4, 4) == ".jpg" || c.Name.Substring(c.Name.Length - 5, 5) == ".jpeg")).ToList();
            JObject json = new JObject();
            int i = 1;
            if (filesInfo.Count() > 0)
            {
                foreach (var item in filesInfo)
                {
                    json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.AppearanceSample_jpgjson = json;
                return Content(json.ToString());
            }
            else
            {
                return Content("图片文档未上传或不存在！");
            }
        }
        #endregion

        #region --------------------查看组装首件pdf文档页面
        public ActionResult preview_AppearanceSample_pdf(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AppearanceSample_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此包装首件pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_AppearanceSample.pdf").Count() > 0)
            {
                address = "/MES_Data/AppearanceSample_Files" + "/" + ordernum + "/" + ordernum + "_AppearanceSample.pdf";
            }
            else
            {
                return Content("此包装首件pdf版文件尚未上传，无pdf文件可下载！");
            }
            return Content(address);
        }
        #endregion

        #endregion
        #endregion


        #region --------------------转异常
        [HttpPost]
        public ActionResult unusualConvert(int? id, string unusualDescription, string statue)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "OrderMgms", act = "Details" + "/" + id.ToString() });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            switch (statue)
            {
                case "组装异常":
                    orderMgm.IsAssembleAbnormal = true;
                    orderMgm.AssembleAbnormalConvertDate = DateTime.Now;
                    orderMgm.AssembleAbnormalConverter = ((Users)Session["User"]).UserName;
                    orderMgm.AssembleAbnormal_Description = unusualDescription;
                    break;
                case "老化异常":
                    orderMgm.IsBurninAbnormal = true;
                    orderMgm.BurninAbnormalConvertDate = DateTime.Now;
                    orderMgm.BurninAbnormalConverter = ((Users)Session["User"]).UserName;
                    orderMgm.BurninAbnormal_Description = unusualDescription;
                    break;
                case "包装异常":
                    orderMgm.IsAppearanceAbnormal = true;
                    orderMgm.AppearanceAbnormalConvertDate = DateTime.Now;
                    orderMgm.AppearanceAbnormalConverter = ((Users)Session["User"]).UserName;
                    orderMgm.AppearanceAbnormal_Description = unusualDescription;
                    break;
                case "SMT异常":
                    orderMgm.IsSMTAbnormal = true;
                    orderMgm.SMTAbnormalConvertDate = DateTime.Now;
                    orderMgm.SMTAbnormalConverter = ((Users)Session["User"]).UserName;
                    orderMgm.SMTAbnormal_Description = unusualDescription;
                    break;
                case "特采订单":
                    orderMgm.IsAOD = true;
                    orderMgm.AODConvertDate = DateTime.Now;
                    orderMgm.AODConverter = ((Users)Session["User"]).UserName;
                    orderMgm.AOD_Description = unusualDescription;
                    break;
                case "组装首件":
                    orderMgm.IsAssembleFirstSample = true;
                    orderMgm.AssembleFirstSampleDate = DateTime.Now;
                    orderMgm.AssembleFirstSampleConverter = ((Users)Session["User"]).UserName;
                    orderMgm.AssembleFirstSample_Description = unusualDescription;
                    break;
                case "老化首件":
                    orderMgm.IsBurnInFirstSample = true;
                    orderMgm.BurnInFirstSampleDate = DateTime.Now;
                    orderMgm.BurnInFirstSampleConverter = ((Users)Session["User"]).UserName;
                    orderMgm.BurnInFirstSample_Description = unusualDescription;
                    break;
                case "包装首件":
                    orderMgm.IsAppearanceFirstSample = true;
                    orderMgm.AppearanceFirstSampleDate = DateTime.Now;
                    orderMgm.AppearanceFirstSampleConverter = ((Users)Session["User"]).UserName;
                    orderMgm.AppearanceFirstSample_Description = unusualDescription;
                    break;
            }
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------上传文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadAssembleFile(int id, string ordernum, string appNUM, string statue)
        {
            string uploadFile = "";
            string diretoryName = "";
            string ReName = "";
            switch (statue)
            {
                case "组装异常":
                    uploadFile = "uploadAssembleAbnormalOrder";
                    diretoryName = "AssembleAbnormalOrder_Files";
                    ReName = "_AssembleAbnormalOrder";
                    break;
                case "老化异常":
                    uploadFile = "uploadBurnInAbnormalOrder";
                    diretoryName = "BurnInAbnormalOrder_Files";
                    ReName = "_BurnInAbnormalOrder";
                    break;
                case "包装异常":
                    uploadFile = "uploadAppearanceAbnormalOrder";
                    diretoryName = "AppearanceAbnormalOrder_Files";
                    ReName = "_AppearanceAbnormalOrder";
                    break;
                case "SMT异常":
                    uploadFile = "uploadSMTAbnormalOrder";
                    diretoryName = "SMTAbnormalOrder_Files";
                    ReName = "_SMTAbnormalOrder";
                    break;
                case "特采订单":
                    uploadFile = "uploadfile";
                    diretoryName = "AOD_Files";
                    ReName = "_AOD";
                    break;
                case "组装首件":
                    uploadFile = "uploadAssembleSamplefile";
                    diretoryName = "AssembleSample_Files";
                    ReName = "_AssembleSample";
                    break;
                case "老化首件":
                    uploadFile = "uploadBurnInSamplefile";
                    diretoryName = "BurnInSample_Files";
                    ReName = "_BurnInSample";
                    break;
                case "包装首件":
                    uploadFile = "uploadAppearanceSamplefile";
                    diretoryName = "AppearanceSample_Files";
                    ReName = "_AppearanceSample";
                    break;
                case "小样":
                    uploadFile = "uploadfile";
                    diretoryName = "SmallSample_Files";
                    ReName = "_SmallSample";
                    break;
            }
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[uploadFile];
                var index = file.FileName.LastIndexOf('.');
                var fileType = file.FileName.Substring(index, file.FileName.Length - index).ToLower();
                var re = String.Equals(fileType, ".jpg") == true || String.Equals(fileType, ".jpeg") || String.Equals(fileType, ".pdf") == true ? false : true;
                if (re)
                {
                    return Content("<script>alert('您选择文件的文件类型不正确，请选择jpg、jpeg或pdf类型文件！');history.go(-1);</script>");
                }
                if (Directory.Exists(@"D:\MES_Data\" + diretoryName + "\\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\" + diretoryName + "\\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\" + diretoryName + "\\" + ordernum + "\\");
                //文件为jpg类型
                if (fileType == ".jpg" || fileType == ".jpeg")
                {
                    if (string.IsNullOrEmpty(appNUM))
                    {
                        int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + ReName) && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\" + diretoryName + "\\" + ordernum + "\\" + ordernum + ReName + (jpg_count + 1) + fileType);
                    }
                    else
                        file.SaveAs(@"D:\MES_Data\AssembleAbnormalOrder_Files\" + ordernum + "\\" + ordernum + "_" + appNUM + ReName + fileType);
                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\" + diretoryName + "\\" + ordernum + "\\" + ordernum + ReName + fileType);
                }
                return RedirectToAction("Details", "OrderMgms", new { id = id });
            }
            return View();
        }
        #endregion

        #region --------------------查看图片预览
        [HttpPost]
        public ActionResult GetAssembleImg(string ordernum, string statue)
        {
            string diretoryName = "";
            string ReName = "";
            switch (statue)
            {
                case "组装异常":
                    diretoryName = "AssembleAbnormalOrder_Files";
                    ReName = "_AssembleAbnormalOrder";
                    break;
                case "老化异常":
                    diretoryName = "BurnInAbnormalOrder_Files";
                    ReName = "_BurnInAbnormalOrder";
                    break;
                case "包装异常":
                    diretoryName = "AppearanceAbnormalOrder_Files";
                    ReName = "_AppearanceAbnormalOrder";
                    break;
                case "SMT异常":
                    diretoryName = "SMTAbnormalOrder_Files";
                    ReName = "_SMTAbnormalOrder";
                    break;
                case "特采订单":
                    diretoryName = "AOD_Files";
                    ReName = "_AOD";
                    break;
                case "组装首件":
                    diretoryName = "AssembleSample_Files";
                    ReName = "_AssembleSample";
                    break;
                case "老化首件":
                    diretoryName = "BurnInSample_Files";
                    ReName = "_BurnInSample";
                    break;
                case "包装首件":
                    diretoryName = "AppearanceSample_Files";
                    ReName = "_AppearanceSample";
                    break;
                case "小样":
                    diretoryName = "SmallSample_Files";
                    ReName = "_SmallSample";
                    break;
            }
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\" + diretoryName + "\\" + ordernum + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum + ReName) && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
            JObject json = new JObject();
            int i = 1;
            if (filesInfo.Count() > 0)
            {
                foreach (var item in filesInfo)
                {
                    json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.AssembleSample_jpgjson = json;
                return Content(json.ToString());
            }
            else
            {
                return Content("图片文档未上传或不存在！");
            }
        }
        #endregion

        #region --------------------查看pdf文档页面
        public ActionResult preview_Assemble_pdf(string ordernum, string statue)
        {
            string diretoryName = "";
            string ReName = "";
            switch (statue)
            {
                case "组装异常":
                    diretoryName = "AssembleAbnormalOrder_Files";
                    ReName = "_AssembleAbnormalOrder";
                    break;
                case "老化异常":
                    diretoryName = "BurnInAbnormalOrder_Files";
                    ReName = "_BurnInAbnormalOrder";
                    break;
                case "包装异常":
                    diretoryName = "AppearanceAbnormalOrder_Files";
                    ReName = "_AppearanceAbnormalOrder";
                    break;
                case "SMT异常":
                    diretoryName = "SMTAbnormalOrder_Files";
                    ReName = "_SMTAbnormalOrder";
                    break;
                case "特采订单":
                    diretoryName = "AOD_Files";
                    ReName = "_AOD";
                    break;
                case "组装首件":
                    diretoryName = "AssembleSample_Files";
                    ReName = "_AssembleSample";
                    break;
                case "老化首件":
                    diretoryName = "BurnInSample_Files";
                    ReName = "_BurnInSample";
                    break;
                case "包装首件":
                    diretoryName = "AppearanceSample_Files";
                    ReName = "_AppearanceSample";
                    break;
                case "小样":
                    diretoryName = "SmallSample_Files";
                    ReName = "_SmallSample";
                    break;
            }
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\" + diretoryName + "\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此组装首件pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + ReName + ".pdf").Count() > 0)
            {
                address = "/MES_Data/" + diretoryName + "/" + ordernum + "/" + ordernum + ReName + ".pdf";
            }
            else
            {
                return Content("此组装首件pdf版文件尚未上传，无pdf文件可下载！");
            }
            return Content(address);
        }


        #endregion




        #region --------------------返回指定目录下所有文件信息
        /// <summary>  
        /// 返回指定目录下所有文件信息  
        /// </summary>  
        /// <param name="strDirectory">目录字符串</param>  
        /// <returns></returns>  
        public List<FileInfo> GetAllFilesInDirectory(string strDirectory)
        {
            List<FileInfo> listFiles = new List<FileInfo>(); //保存所有的文件信息  
            DirectoryInfo directory = new DirectoryInfo(strDirectory);
            DirectoryInfo[] directoryArray = directory.GetDirectories();
            FileInfo[] fileInfoArray = directory.GetFiles();
            if (fileInfoArray.Length > 0) listFiles.AddRange(fileInfoArray);
            foreach (DirectoryInfo _directoryInfo in directoryArray)
            {
                DirectoryInfo directoryA = new DirectoryInfo(_directoryInfo.FullName);
                DirectoryInfo[] directoryArrayA = directoryA.GetDirectories();
                FileInfo[] fileInfoArrayA = directoryA.GetFiles();
                if (fileInfoArrayA.Length > 0) listFiles.AddRange(fileInfoArrayA);
                GetAllFilesInDirectory(_directoryInfo.FullName);//递归遍历  
            }
            return listFiles;
        }
        #endregion

        #region --------------------SmallSampleScedule列表
        private List<SelectListItem> SmallSampleScedule()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "未开始",
                    Value = "未开始"
                },
                new SelectListItem
                {
                    Text = "进行中",
                    Value = "进行中"
                },
                new SelectListItem
                {
                    Text = "完成",
                    Value = "完成"
                }
            };
        }
        #endregion

        //批量修改订单模组号,查看
        public ActionResult BitchSelectModulNum(string ordernum)
        {
            var list = db.BarCodes.OrderBy(c=>c.BarCodesNum).Where(c => c.OrderNum == ordernum);
            JObject jobjet = new JObject();
            JArray barcode = new JArray();
            JArray module = new JArray();
            foreach (var item in list)
            {
                barcode.Add(item.BarCodesNum);
                if (item.ModuleGroupNum == null)
                    module.Add(" ");
                else
                    module.Add(item.ModuleGroupNum);
            }
            jobjet.Add("barcode", barcode);
            jobjet.Add("module", module);

            return Content(JsonConvert.SerializeObject(jobjet));
        }

        //批量修改订单模组号,修改
        public async Task<bool> BitchUpdateModulNumAsync(List<UpdateModule> updates)
        {
            int count = 0;
            string message = "";
            foreach (var item in updates)
            {
                var barcode = db.BarCodes.Where(c => c.BarCodesNum == item.barcode).FirstOrDefault();

                var modulelist = db.BarCodes.Where(c => c.OrderNum == barcode.OrderNum && !string.IsNullOrEmpty(c.ModuleGroupNum)).Select(c => c.ModuleGroupNum).ToList();
                if (modulelist.Contains(item.module))//判断输入的模组号是否重复
                {
                    return false;
                }
                message = message + "条码" + item.barcode + "模组号为" + barcode.ModuleGroupNum + ",修改为" + item.module;
                barcode.ModuleGroupNum = item.module;
                var calibrationRecord = db.CalibrationRecord.Where(c => c.BarCodesNum == item.barcode && (c.OldBarCodesNum == null || c.OldBarCodesNum == item.barcode)).ToList();
                if (calibrationRecord .Count()!=0)
                {
                    calibrationRecord.ForEach(c=>c.ModuleGroupNum = item.module);
                }

                var apper = db.Appearance.Where(c => c.BarCodesNum == item.barcode && (c.OldBarCodesNum == null || c.OldBarCodesNum == item.barcode)).ToList();
                if (apper.Count() != 0)
                {
                    apper.ForEach(c => c.ModuleGroupNum = item.module);
                }
                count += await db.SaveChangesAsync();
            }
            if (count != 0)
            {
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "修改模组号：" + message };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                return true;
            }
            else
                return false;
        }

    }
}

