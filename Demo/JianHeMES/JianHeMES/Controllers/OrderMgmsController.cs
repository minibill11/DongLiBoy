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

namespace JianHeMESEntities.Controllers
{
    public class OrderMgmsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OrderMgms

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
            return View(ordernums.ToList());
        }
        #endregion

        #region --------------------Details页
        // GET: OrderMgms/Details/5
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
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }

            #region----------订单各工段详细信息
            JObject ProductionDetailsJson = new JObject();
            int modelGroupQuantity = db.OrderMgm.Where(c => c.OrderNum == orderMgm.OrderNum).FirstOrDefault().Boxes;//订单模组数量

            #region----------订单在组装的统计数据
            var assembleRecord = db.Assemble.Where(c => c.OrderNum == orderMgm.OrderNum).ToList();//订单在组装的全部记录
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
            var assemble_NotFirstPassYield = assembleRecord.Where(c=>c.PQCCheckFinish == false).Select(c=>c.BoxBarCode).Distinct().ToList();
            var assemble_PassYield = assembleRecord.Where(c=>c.PQCCheckFinish == true && c.RepetitionPQCCheck==false).Select(c => c.BoxBarCode).ToList();
            var assemble_FirstPassYield = assemble_PassYield.Except(assemble_NotFirstPassYield).ToList();//直通条码记录
            var assemble_FirstPassYieldCount = assemble_FirstPassYield.Count();//直通个数
            ProductionDetailsJson.Add("AssembleFirstPassYieldCount", assemble_FirstPassYieldCount.ToString());//直通个数
            if(assemble_FirstPassYieldCount==0)
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
            var assemble_finished = assembleRecord.Count(m => m.PQCCheckFinish == true && m.RepetitionPQCCheck==false);//订单已完成PQC个数
            var assemble_finishedList = assembleRecord.Where(m => m.PQCCheckFinish == true).Select(m => m.BoxBarCode).ToList(); //订单已完成PQC的条码清单
            var assemble_assemblePQC_Count = assembleRecord.Count();//订单PQC全部记录条数
            if(assemble_finished==0)
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
            var FinalQCRecord = db.FinalQC.Where(c => c.OrderNum == orderMgm.OrderNum).ToList();//订单在组装的全部记录
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
            var FinalQC_PassYield = FinalQCRecord.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck==false).Select(c => c.BarCodesNum).ToList();
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
            var FinalQC_finished = FinalQCRecord.Count(m => m.FQCCheckFinish == true && m.RepetitionFQCCheck==false);//订单已完成PQC个数
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
            var burn_inRecord = db.Burn_in.Where(c => c.OrderNum == orderMgm.OrderNum).ToList();//订单在老化的全部记录
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
            if(burn_in_FirstPassYieldCount==0)
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
            if(burn_in_Count==0)
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
            var calibrationRecord = db.CalibrationRecord.Where(c => c.OrderNum == orderMgm.OrderNum).ToList();//订单在校正的全部记录
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
            if(calibration_FirstPassYieldCount==0)
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
            var calibration_finished = calibrationRecord.Where(c=>c.Normal==true && c.RepetitionCalibration == false).Select(c=>c.BarCodesNum).Distinct().Count();//订单已完成PQC个数
            var calibration_finishedList = calibrationRecord.Where(m => m.Normal == true && m.RepetitionCalibration == false).Select(m => m.BarCodesNum).ToList(); //订单已完成PQC的条码清单
            var calibration_Count = calibrationRecord.Where(c=>c.RepetitionCalibration==false).Count();//订单PQC全部记录条数
            if(calibration_Count==0)
            {
                ProductionDetailsJson.Add("CalibrationFinisthRate", "");
                ProductionDetailsJson.Add("CalibrationPassRate", "");
            }
            else
            {
                var calibration_finisthRate = Convert.ToDecimal(calibration_finished) / calibrationRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100;//完成率：完成数/订单的模组数
                ProductionDetailsJson.Add("CalibrationFinisthRate", calibration_finisthRate.ToString("F2"));
                //合格率
                var calibration_PassRate = (Convert.ToDecimal(calibration_finished) / calibration_Count )* 100;//合格率：完成数/记录数
                ProductionDetailsJson.Add("CalibrationPassRate", calibration_PassRate.ToString("F2"));
            }
            #endregion


            #region----------订单在外观包装的统计数据
            var appearanceRecord = db.Appearance.Where(c => c.OrderNum == orderMgm.OrderNum).ToList();//订单在包装的全部记录
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
            if(appearance_FirstPassYieldCount==0)
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
            if(appearance_Count==0)
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


            #region----------组装工段按线别分类输出条码清单
            //订单的全部组装记录
            ViewBag.order_in_Assemble_Record = db.Assemble.Where(c => c.OrderNum == orderMgm.OrderNum).ToList();
            
            #endregion


            #region----------特采订单基本信息
            //检查特采订单文件目录是否存在
            string directory = "D:\\MES_Data\\AOD_Files\\" + orderMgm.OrderNum;
            if (Directory.Exists(@directory) == true)
            {
                ViewBag.Directory = "Exists";
            }
            //检查特采订单pdf文件是否存在
            string pdfFile = "D:\\MES_Data\\AOD_Files\\" + orderMgm.OrderNum + "\\"+ orderMgm.OrderNum + "_AOD.pdf";
            if (System.IO.File.Exists(@pdfFile) == true)
            {
                ViewBag.PDf = "Exists";
            }
            List<FileInfo> filesInfo = new List<FileInfo>();
            JObject json = new JObject();
            if(ViewBag.Directory =="Exists")
            {
                filesInfo = GetAllFilesInDirectory(directory);
                filesInfo = filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum + "_AOD") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
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
                SmallSample_filesInfo = SmallSample_filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum + "_SmallSample") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
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
                AbnormalOrder_filesInfo = AbnormalOrder_filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum + "_AbnormalOrder") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
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

            return View(orderMgm);
        }
        #endregion
                    
        #region --------------------Create页
        // GET: OrderMgms/Create
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || ((Users)Session["User"]).Role == "系统管理员" || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC组长")
            {
                return View();

            }
            return Content("<script>alert('对不起，您未授权管理订单，请联系PC部经理！');window.location.href='../OrderMgms/Index';</script>");
            //return RedirectToAction("Index");
        }

        // POST: OrderMgms/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OrderNum,BarCode_Prefix,CustomerName,ContractDate,DeliveryDate,PlanInputTime,PlanCompleteTime,PlatformType,Area,ProcessingRequire,StandardRequire,Capacity,CapacityQ,HandSampleScedule,Boxes,Models,ModelsMore,Powers,PowersMore,AdapterCard,AdapterCardMore,OrderCreateDate,BarCodeCreated,BarCodeCreateDate,BarCodeCreator,CompletedRate,IsRepertory,Remark")] OrderMgm orderMgm)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if(db.OrderMgm.Count(c=>c.OrderNum==orderMgm.OrderNum)>0)
            {
                ModelState.AddModelError("", "此订单号已存在，如订单信息有误，请进入订单详情修改信息！");
                return View(orderMgm);
            }
            //设置条码生成状态为0，表示未生成订单条码
            orderMgm.BarCodeCreated = 0;
            orderMgm.HandSampleScedule = "未开始";
            orderMgm.OrderCreateDate = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.OrderMgm.Add(orderMgm);
                    db.SaveChanges();
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
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || ((Users)Session["User"]).Role == "系统管理员" || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC组长")
            {
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
            return RedirectToAction("Index", "OrderMgms");
        }

        // POST: OrderMgms/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OrderNum,BarCode_Prefix,CustomerName,ContractDate,DeliveryDate,PlanInputTime,PlanCompleteTime,PlatformType,Area,ProcessingRequire,StandardRequire,Capacity,CapacityQ,HandSampleScedule,Boxes,Models,ModelsMore,Powers,PowersMore,AdapterCard,AdapterCardMore,OrderCreateDate,BarCodeCreated,BarCodeCreateDate,BarCodeCreator,CompletedRate,IsRepertory,Remark")] OrderMgm orderMgm)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            int orginal_ordermgm_BoxsNum = (from m in db.OrderMgm where m.OrderNum == orderMgm.OrderNum select m.Boxes).SingleOrDefault();
            //1.修改后的订单模组数>原订单模组数且模组条码已经生成，在BarCodes表追加相应的条码号
            if (orderMgm.Boxes > orginal_ordermgm_BoxsNum && orderMgm.BarCodeCreated == 1)
            {
                int addcount = orderMgm.Boxes - orginal_ordermgm_BoxsNum;
                var bc = db.BarCodes.Where(c => c.OrderNum == orderMgm.OrderNum && c.BarCodeType == "模组").OrderByDescending(c => c.BarCodesNum).FirstOrDefault();
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
            ////2.修改后的订单模组数<原订单模组数
            ////  检查对应缩少的那部分条码号是否有生产记录，如果无生产记录，询问是否修改？如果有生产记录，反馈“有生产记录，不能修改！”
            ////  如果修改，修改后把BarCodes表中的缩少的那部分条码号删除
            //else if (orderMgm.Boxes < orginal_ordermgm_BoxsNum)
            //{
            //}
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details",new { id = orderMgm.ID});
            }
            return View(orderMgm);
        }

        #endregion

        #region --------------------EditForSmallSample修改小样进度页

        public ActionResult EditForSmallSample(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "小样调试员" || ((Users)Session["User"]).Role == "系统管理员")
            {
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
            }
            else
            {
                return Content("<script>alert('对不起，您的不能管理小样进度，请联系技术部经理！');history.go(-1);</script>");
            }
            //return RedirectToAction("Index", "OrderMgms");
        }

        // POST: OrderMgms/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditForSmallSample([Bind(Include = "ID,OrderNum,BarCode_Prefix,CustomerName,ContractDate,DeliveryDate,PlanInputTime,PlanCompleteTime,PlatformType,Area,ProcessingRequire,StandardRequire,Capacity,CapacityQ,HandSampleScedule,Boxes,Models,ModelsMore,Powers,PowersMore,AdapterCard,AdapterCardMore,OrderCreateDate,BarCodeCreated,BarCodeCreateDate,BarCodeCreator,CompletedRate,IsRepertory,Remark")] OrderMgm orderMgm)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            ViewBag.HandSampleScedule = SmallSampleScedule();
            ViewBag.SmallSampleSceduleValue = orderMgm.HandSampleScedule;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
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
            var Record = db.OrderMgm.Where(c=>c.OrderNum==orderNum).FirstOrDefault();
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
                if (assembleCount==0 && fqcCount==0 && burn_inCount==0 && calibrationCount==0 && appearancesCount == 0 && smtCount == 0)
                {
                    //取出订单对应的条码
                    var barCodeList = db.BarCodes.Where(c => c.OrderNum == orderNum).ToList();
                    if (barCodeList != null)
                    {
                        //删除订单的条码信息
                        foreach(var barCode in barCodeList)
                        {
                            db.BarCodes.Remove(barCode);
                            db.SaveChanges();
                        }
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
                    return Content("<script>alert('订单号"+ orderNum +  "有生产记录,不能删除此订单！');history.go(-1);</script>");
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
                return RedirectToAction("Login", "Users");
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




        #region --------------------转为异常订单
        [HttpPost]
        public ActionResult AbnormalOrderConvert(int? id, string AbnormalOrder_Description)  
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
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
                var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();
                string ReName = ordernum + "_AbnormalOrder";
                if (Directory.Exists(@"D:\MES_Data\AbnormalOrder_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\AbnormalOrder_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\AbnormalOrder_Files\" + ordernum + "\\");
                //文件为jpg类型
                if (fileType == ".jpg")
                {
                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AbnormalOrder") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                    file.SaveAs(@"D:\MES_Data\AbnormalOrder_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);
                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\AbnormalOrder_Files\" + ordernum + "\\" + ReName + fileType);
                }
                //OrderMgm orderMgm = db.OrderMgm.Find(id);
                //if (orderMgm == null)
                //{
                //    return View(orderMgm);
                //}
                //if (ModelState.IsValid)
                //{
                //    db.Entry(orderMgm).State = EntityState.Modified;
                //    db.SaveChanges();
                //    return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
                //}
                //return View(orderMgm);
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
                address = "/AbnormalOrder_Files" + "/" + ordernum + "/" + ordernum + "_AbnormalOrder.pdf";
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
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum + "_AbnormalOrder") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
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
                address = "~/Scripts/pdf.js/web/viewer.html?file=\\AbnormalOrder_Files\\" + ordernum + "\\" + ordernum + "_AbnormalOrder.pdf";
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
                          



        #region --------------------转为特采订单
        [HttpPost]
        public ActionResult AODConvert(int? id,string AOD_Description)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
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
                var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();
                string ReName = ordernum + "_AOD";
                if (Directory.Exists(@"D:\MES_Data\AOD_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\AOD_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\AOD_Files\" + ordernum + "\\");
                //文件为jpg类型
                if(fileType==".jpg")
                {
                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AOD") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                    file.SaveAs(@"D:\MES_Data\AOD_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);
                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\AOD_Files\" + ordernum + "\\" + ReName + fileType);
                }
                //OrderMgm orderMgm = db.OrderMgm.Find(id);
                //if (orderMgm == null)
                //{
                //    return View(orderMgm);
                //}
                //if (ModelState.IsValid)
                //{
                //    db.Entry(orderMgm).State = EntityState.Modified;
                //    db.SaveChanges();
                //    return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
                //}
                //return View(orderMgm);
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
            if (filesInfo.Where(c=>c.Name == ordernum + "_AOD.pdf").Count()>0)
            {
                address = "/AOD_Files" + "/" + ordernum + "/" + ordernum + "_AOD.pdf";
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
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum + "_AOD") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
            JObject json = new JObject();
            int i = 1;
            foreach (var item in filesInfo)
            {
                json.Add(i.ToString(), item.Name);
                i++;
            }
            ViewBag.jpgjson = json;
            //return Json(json, JsonRequestBehavior.AllowGet);
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
                address = "~/Scripts/pdf.js/web/viewer.html?file=\\AOD_Files\\" + ordernum + "\\" + ordernum + "_AOD.pdf";
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




        #region --------------------上传小样文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadSmallSample(int id, string ordernum)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadfile"];
                var fileType = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                var re = String.Equals(fileType, ".jpg") == true || String.Equals(fileType, ".jpeg" )|| String.Equals(fileType, ".pdf") == true ? false : true;
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
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum + "_SmallSample") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
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
                address = "~/Scripts/pdf.js/web/viewer.html?file=\\SmallSample_Files\\" + ordernum + "\\" + ordernum + "_SmallSample.pdf";
            }
            else
            {
                return Content("pdf文档未上传或不存在！");
            }
            ViewBag.address = address;
            ViewBag.ordernum = ordernum;
            return Redirect(address);
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
                    Text = "待产",
                    Value = "待产"
                },
                new SelectListItem
                {
                    Text = "完成",
                    Value = "完成"
                }
            };
        }
        #endregion

    }
}

