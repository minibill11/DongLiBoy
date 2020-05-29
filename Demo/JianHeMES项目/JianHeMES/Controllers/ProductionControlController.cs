﻿using JianHeMES.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace JianHeMES.Controllers
{
    public class ProductionControlController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController com = new CommonController();



        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Details()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "ProductionControl", act = "Details" });
            }
            return View();
        }


        #region -----------------ProductionControlHistory生产管控历史记录页面

        public ActionResult ProductionControlHistory()
        {
            ViewBag.PlatformType = PlatformTypeList();
            ViewBag.OrderNumList = GetOrderList();
            return View();
        }
        //[HttpPost]
        //public ActionResult ProductionControlHistory(string PlatformType, string orderNum)
        //{
        //    ViewBag.PlatformType = PlatformTypeList();
        //    ViewBag.OrderNumList = GetOrderList();
        //    JObject ProductionControlHistory = new JObject();   //创建JSON对象
        //    //取出数据
        //    int i = 1;
        //    using (var db = new ApplicationDbContext())
        //    {
        //        var OrderList_All = (from m in db.OrderMgm select m).OrderBy(c => c.BarCodeCreated).ToList();
        //        if (PlatformType!="")
        //        {
        //            OrderList_All = OrderList_All.Where(m => m.PlatformType == PlatformType).ToList();
        //        }
        //        if(!String.IsNullOrEmpty(orderNum))
        //        {
        //            OrderList_All = OrderList_All.Where(c => c.OrderNum == orderNum).ToList();
        //        }
        //        //把2017-TEST-1放入排除清单中
        //        OrderList_All = OrderList_All.Where(c => c.OrderNum != "2017-TEST-1").ToList();
        //        //----------------------------------------------------------------------------------

        //        foreach (var item in OrderList_All)
        //        {
        //            //存入JSON对象
        //            var OrderNum = new JObject
        //            {
        //                {"Id",item.ID },
        //                { "OrderNum", item.OrderNum },
        //                { "Quantity", item.Boxes },
        //                { "PlatformType", item.PlatformType },
        //                { "PlanInputTime", item.PlanInputTime.ToString() },
        //                { "PlanCompleteTime", item.PlanCompleteTime.ToString() },
        //            };

        //            var beginttime = db.Assemble.Where(c => c.OrderNum == item.OrderNum).Min(c => c.PQCCheckBT);//取出订单开始装配生产的PQCCheckBT值
        //            if (beginttime == null)
        //            {
        //                beginttime = db.Burn_in.Where(c => c.OrderNum == item.OrderNum).Min(c => c.OQCCheckBT);//取出订单开始老化调试的OQCCheckBT值
        //                if (beginttime == null)
        //                {
        //                    beginttime = db.CalibrationRecord.Where(c => c.OrderNum == item.OrderNum).Min(c => c.BeginCalibration);//取出订单开始校正的BeginCalibration值
        //                    if (beginttime == null)
        //                    {
        //                        beginttime = db.Appearance.Where(c => c.OrderNum == item.OrderNum).Min(c => c.OQCCheckBT);//取出订单开始包装电检检查的OQCCheckBT值
        //                    }
        //                }
        //            }
        //            if (beginttime != null)
        //            {
        //                OrderNum.Add("ActualProductionTime", beginttime.ToString());
        //            }
        //            else
        //            {
        //                OrderNum.Add("ActualProductionTime", "未开始");
        //            }


        //            //var finishtime = db.Appearance.Where(c => c.OrderNum == item.OrderNum).Max(c => c.OQCCheckFT);//取出最后包装记录的OQCCheckFT值
        //            //var totaltime = finishtime - beginttime;
        //            //OrderNum.Add("ActualFinishTime", finishtime.ToString());
        //            //OrderNum.Add("TotalTime", totaltime.ToString());
        //            //完成时间
        //            int modelGroupQuantity = OrderList_All.Where(c => c.OrderNum == item.OrderNum).FirstOrDefault().Boxes;//订单模组数量
        //            DateTime? finishtime = new DateTime();
        //            if (db.Appearance.Count(c => c.OrderNum == item.OrderNum && c.OQCCheckFinish == true) == modelGroupQuantity)
        //            {
        //                finishtime = db.Appearance.Where(c => c.OrderNum == item.OrderNum).Max(c => c.OQCCheckFT);
        //            }
        //            else
        //            {
        //                finishtime = null;
        //            }
        //            var totaltime = finishtime == null ? "" : (finishtime - beginttime).ToString();
        //            OrderNum.Add("ActualFinishTime", finishtime.ToString());
        //            OrderNum.Add("TotalTime", totaltime);



        //            #region-------------------组装部分
        //            //-------------------组装部分
        //            var AssembleRecord = (from m in db.Assemble where m.OrderNum == item.OrderNum select m).ToList();//查出OrderNum的所有组装记录
        //            if (AssembleRecord.Count() > 0)
        //            {
        //                Decimal Assemble_Normal = AssembleRecord.Where(m => m.PQCCheckAbnormal == "正常").Count();//组装PQC正常个数
        //                OrderNum.Add("Assemble_Finish", Convert.ToInt32(Assemble_Normal));
        //                OrderNum.Add("AssembleRecord_Count", AssembleRecord.Count());
        //                //计算组装完成率、合格率
        //                if (Assemble_Normal == 0)
        //                {
        //                    OrderNum.Add("Assemble_Finish_Rate", "0%");
        //                    OrderNum.Add("Assemble_Pass_Rate", "0%");
        //                }
        //                else
        //                {
        //                    OrderNum.Add("Assemble_Finish_Rate", (Assemble_Normal / item.Boxes * 100).ToString("F2") + "%");
        //                    OrderNum.Add("Assemble_Pass_Rate", (Assemble_Normal / AssembleRecord.Count() * 100).ToString("F2") + "%");
        //                }
        //            }
        //            else
        //            {
        //                OrderNum.Add("Assemble_Finish_Rate", "--%");
        //                OrderNum.Add("Assemble_Pass_Rate", "--%");
        //            }
        //            #endregion

        //            #region-------------------FQC部分
        //            var FinalQC_Record = db.FinalQC.Where(c => c.OrderNum == item.OrderNum).ToList();
        //            if (FinalQC_Record.Count > 0)
        //            {
        //                Decimal FinalQC_Finish = FinalQC_Record.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Count();
        //                OrderNum.Add("FinalQC_Finish", Convert.ToInt32(FinalQC_Finish)); //FQC完成个数
        //                OrderNum.Add("FinalQC_Record_Count", FinalQC_Record.Count);      //FQC记录个数
        //                OrderNum.Add("FinalQC_Spot_Count", FinalQC_Record.Select(c => c.BarCodesNum).Distinct().Count());      //FQC抽检个数 
        //                if (FinalQC_Finish == 0)
        //                {
        //                    OrderNum.Add("FinalQC_Finish_Rate", "0%"); //FQC完成率
        //                    OrderNum.Add("FinalQC_Pass_Rate", "0%");   //FQC合格率
        //                }
        //                else
        //                {
        //                    //OrderNum.Add("FinalQC_Finish_Rate", (FinalQC_Finish / FinalQC_Record.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2") + "%");  //FQC完成率
        //                    OrderNum.Add("FinalQC_Finish_Rate", (FinalQC_Finish / item.Boxes * 100).ToString("F2") + "%");  //FQC抽检比例
        //                    OrderNum.Add("FinalQC_Pass_Rate", (FinalQC_Finish / FinalQC_Record.Count() * 100).ToString("F2") + "%"); //FQC合格率
        //                }
        //            }
        //            else
        //            {
        //                OrderNum.Add("FinalQC_Finish_Rate", "--%");  //FQC完成率
        //                OrderNum.Add("FinalQC_Pass_Rate", "--%");    //FQC合格率
        //            }
        //            #endregion

        //            #region--------------------老化部分
        //            //--------------------老化部分
        //            var Burn_in_Record = (from m in db.Burn_in where m.OrderNum == item.OrderNum select m).ToList();//查出OrderNum的所有老化记录
        //            int Burn_in_Record_Count = Burn_in_Record.Select(m => m.BarCodesNum).Distinct().ToList().Count();
        //            if (Burn_in_Record.Count() > 0)
        //            {
        //                Decimal Burn_in_Normal = Burn_in_Record.Where(m => m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化正常个数
        //                //Decimal Burn_in_FirstPass = Burn_in_Record.Where(m => m.OQCCheckFinish == true && m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化工序直通个数
        //                Decimal Burn_in_Finish = Burn_in_Record.Count(m => m.OQCCheckFinish == true); //完成老化工序的个数
        //                OrderNum.Add("Burn_in_Finish", Convert.ToInt32(Burn_in_Finish));
        //                OrderNum.Add("Burn_in_Record_Count", Burn_in_Record.Count());
        //                OrderNum.Add("Burn_in_Count", Burn_in_Record_Count);
        //                //计算老化完成率、合格率
        //                //if (Burn_in_Finish == 0)
        //                //{
        //                //    OrderNum.Add("Burn_in_Finish_Rate", "0%");
        //                //    OrderNum.Add("Burn_in_Pass_Rate", "0%");
        //                //}
        //                //else
        //                //{
        //                    //OrderNum.Add("Burn_in_Finish_Rate", (Burn_in_Finish / Burn_in_Record_Count * 100).ToString("F2") + "%");
        //                    OrderNum.Add("Burn_in_Finish_Rate", (Convert.ToDecimal(Burn_in_Record.Count()) / item.Boxes * 100).ToString("F2") + "%");
        //                    OrderNum.Add("Burn_in_Pass_Rate", (Burn_in_Finish / Burn_in_Record.Count() * 100).ToString("F2") + "%");
        //                //}
        //            }
        //            else
        //            {
        //                OrderNum.Add("Burn_in_Finish_Rate", "--%");
        //                OrderNum.Add("Burn_in_Pass_Rate", "--%");
        //            }
        //            #endregion

        //            #region---------------------校正部分
        //            //---------------------校正部分
        //            var Calibration_Record = (from m in db.CalibrationRecord where m.OrderNum == item.OrderNum && m.RepetitionCalibration == false select m).ToList();//查出OrderNum的所有校正记录
        //            var Calibration_Record_Count = Calibration_Record.Select(m => m.BarCodesNum).Distinct().Count();//条码记录去重
        //            if (Calibration_Record_Count<=1)
        //            {
        //                Calibration_Record_Count = Calibration_Record.Count();
        //            }
        //            if (Calibration_Record.Count() > 0)
        //            {
        //                Decimal Calibration_Normal = Calibration_Record.Where(m => m.Normal == true).Count();//校正正常个数
        //                OrderNum.Add("Calibration_Finish", Convert.ToInt32(Calibration_Normal));
        //                OrderNum.Add("Calibration_Record_Count", Calibration_Record.Count());
        //                OrderNum.Add("Calibration_Count", Calibration_Record_Count);
        //                //计算校正完成率、合格率
        //                if (Calibration_Normal == 0)
        //                {
        //                    OrderNum.Add("Calibration_Finish_Rate", "0%");
        //                    OrderNum.Add("Calibration_Pass_Rate", "0%");
        //                }
        //                else
        //                {
        //                    //OrderNum.Add("Calibration_Finish_Rate", (Calibration_Normal / Calibration_Record_Count * 100).ToString("F2") + "%");
        //                    OrderNum.Add("Calibration_Finish_Rate", (Calibration_Normal / item.Boxes * 100).ToString("F2") + "%");
        //                    OrderNum.Add("Calibration_Pass_Rate", (Calibration_Normal / Calibration_Record.Count() * 100).ToString("F2") + "%");
        //                }
        //            }
        //            else
        //            {
        //                OrderNum.Add("Calibration_Finish_Rate", "--%");
        //                OrderNum.Add("Calibration_Pass_Rate", "--%");
        //            }
        //            #endregion

        //            #region---------------------外观包装部分
        //            //---------------------外观包装部分
        //            //var Appearances_Record = (from m in db.Appearance where m.OrderNum == item.OrderNum select m).OrderBy(m=>m.BarCodesNum).ToList();//查出OrderNum的所有外观包装记录
        //            var Appearances_Record = db.Appearance.Where(m => m.OrderNum == item.OrderNum).OrderBy(m => m.BarCodesNum).ToList();
        //            List<string> Appearances_Record_Count = Appearances_Record.Select(m => m.BarCodesNum).Distinct().ToList();//条码记录去重后个数
        //            //HashSet<string> Appearances_Record_Count2 = new HashSet<string>(Appearances_Record_Count);
        //            if (Appearances_Record.Count() > 0)
        //            {
        //                //Decimal Appearances_Normal = Appearances_Record.Where(m => m.Appearance_OQCCheckAbnormal == "正常").Count();//外观包装正常个数
        //                Decimal Appearances_Finish = Appearances_Record.Where(m => m.OQCCheckFinish == true).Count();//外观包装完成个数
        //                OrderNum.Add("Appearances_Finish", Convert.ToInt32(Appearances_Finish));
        //                OrderNum.Add("Appearances_Record_Count", Appearances_Record.Count());
        //                OrderNum.Add("Appearances_Count", Appearances_Record_Count.Count());
        //                //计算外观包装完成率、合格率
        //                if (Appearances_Finish == 0)
        //                {
        //                    OrderNum.Add("Appearances_Finish_Rate", "0%");
        //                    OrderNum.Add("Appearances_Pass_Rate", "0%");
        //                }
        //                else
        //                {
        //                    //OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / Appearances_Record_Count.Count() * 100).ToString("F2") + "%");//完成数/条码记录去重后个数
        //                    OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / item.Boxes * 100).ToString("F2") + "%");//完成数/条码记录去重后个数
        //                    OrderNum.Add("Appearances_Pass_Rate", (Appearances_Finish / Appearances_Record.Count() * 100).ToString("F2") + "%");//完成数/记录条数
        //                }
        //            }
        //            else
        //            {
        //                //使用库存出库订单
        //                Appearances_Record = db.Appearance.Where(c => c.ToOrderNum == item.OrderNum).ToList();
        //                Appearances_Record_Count = Appearances_Record.Select(m => m.BarCodesNum).Distinct().ToList();//条码记录去重
        //                //Appearances_Record_Count2 = new HashSet<string>(Appearances_Record_Count);
        //                if (Appearances_Record.Count() > 0)
        //                {
        //                    Decimal Appearances_Finish = Appearances_Record.Where(m => m.OQCCheckFinish == true).Count();//外观包装完成个数
        //                    OrderNum.Add("Appearances_Finish", Convert.ToInt32(Appearances_Finish));
        //                    OrderNum.Add("Appearances_Record_Count", Appearances_Record.Count());
        //                    OrderNum.Add("Appearances_Count", Appearances_Record_Count.Count());
        //                    OrderNum.Remove("ActualProductionTime");
        //                    OrderNum.Add("ActualProductionTime", Appearances_Record.Min(c => c.OQCCheckBT).ToString()); //取出最早记录的包装OQCCheckBT值
        //                    //计算外观包装完成率、合格率
        //                    if (Appearances_Finish == 0)
        //                    {
        //                        OrderNum.Add("Appearances_Finish_Rate", "0%");
        //                        OrderNum.Add("Appearances_Pass_Rate", "0%");
        //                    }
        //                    else
        //                    {
        //                        //OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / Appearances_Record_Count.Count() * 100).ToString("F2") + "%");//完成数/条码记录去重后个数
        //                        OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / item.Boxes * 100).ToString("F2") + "%");//完成数/条码记录去重后个数
        //                        OrderNum.Add("Appearances_Pass_Rate", (Appearances_Finish / Appearances_Record.Count() * 100).ToString("F2") + "%");//完成数/记录条数
        //                    }


        //                }
        //                else
        //                {
        //                    OrderNum.Add("Appearances_Finish_Rate", "--%");
        //                    OrderNum.Add("Appearances_Pass_Rate", "--%");
        //                }
        //            }
        //            #endregion

        //            #region---------------------PQC超24小时未进入老化工序和老化后72小时未进入包装工序提示信息部分
        //            //此条码PQC完成，到现在为止超过24小时未有老化记录，输出提示；
        //            //此条码PQC完成，老化的开始时间－PQC完成时间超24小时，输出提示；
        //            //此条码老化完成，到现在为止超过72小时未有包装记录，输出提示；
        //            //此条码老化完成，包装的开始时间－老化完成时间超72小时，输出提示；
        //            List<string> barCodesList = db.BarCodes.Where(c => c.OrderNum == item.OrderNum).OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();
        //            int Overtime_never_join_burn_in_count = 0;
        //            int Overtime_never_join_appearances_count = 0;
        //            //var AssembleRecord_count = AssembleRecord.Where(c => c.OrderNum == item.OrderNum).Count();
        //            string assemble_Finish_Rate = (string)OrderNum["Assemble_Finish_Rate"];
        //            var burn_in_Finish_Rate = OrderNum.Value<string>("Burn_in_Finish_Rate");
        //            var appearances_Finish_Rate = OrderNum.Value<string>("Appearances_Finish_Rate");
        //            //如果组装和老化调试都未开始或都已经完成，则输出空值
        //            if (assemble_Finish_Rate == "100.00%" && burn_in_Finish_Rate == "100.00%" || assemble_Finish_Rate == "--%" && burn_in_Finish_Rate == "--%")
        //            {
        //                OrderNum.Add("Overtime_never_join_burn_in", "");
        //            }
        //            else
        //            {
        //                //计算PQC完超过24小时未进入老化调试输出，如果下一工序已经有记录，则不算入超时数量，只要有一个超时，就跳出foreach 循环
        //                foreach (var it in barCodesList)
        //                {
        //                    //取出组装已经完成的记录
        //                    var assemble_record = AssembleRecord.Where(c => c.BoxBarCode == it && c.PQCCheckFT != null && c.PQCCheckFinish == true && c.RepetitionPQCCheck != true).FirstOrDefault();
        //                    if (assemble_record != null)
        //                    {
        //                        //取出组装完成时间
        //                        var assembleFinishTime_temp = assemble_record.PQCCheckFT;
        //                        var burn_in_Earliest_Record = Burn_in_Record.Where(c => c.BarCodesNum == it && c.OQCCheckBT != null).OrderBy(c => c.OQCCheckBT).FirstOrDefault();
        //                        //如果老化调试最早时间为空，则老化调试没有记录
        //                        if (burn_in_Earliest_Record == null)
        //                        {
        //                            ////取出老化调试最早时间
        //                            //var burn_in_BeginTime_temp = Burn_in_Record.Where(c => c.BarCodesNum == it && c.OQCCheckBT != null).Min(c => c.OQCCheckBT);
        //                            //DateTime burn_in_BeginTime = burn_in_BeginTime_temp == null ? DateTime.Now : new DateTime(burn_in_BeginTime_temp.Value.Year, burn_in_BeginTime_temp.Value.Month, burn_in_BeginTime_temp.Value.Day, burn_in_BeginTime_temp.Value.Hour, burn_in_BeginTime_temp.Value.Minute, burn_in_BeginTime_temp.Value.Second);
        //                            DateTime assembleFinishTime = assembleFinishTime_temp == null ? DateTime.Now : new DateTime(assembleFinishTime_temp.Value.Year, assembleFinishTime_temp.Value.Month, assembleFinishTime_temp.Value.Day, assembleFinishTime_temp.Value.Hour, assembleFinishTime_temp.Value.Minute, assembleFinishTime_temp.Value.Second);
        //                            TimeSpan overtime_never_join_appearances = DateTime.Now - assembleFinishTime;
        //                            if (overtime_never_join_appearances.TotalHours >= 24)
        //                            {
        //                                Overtime_never_join_burn_in_count++;
        //                            }
        //                        }
        //                    }
        //                    if (Overtime_never_join_burn_in_count >= 1)
        //                        break;
        //                }
        //                OrderNum.Add("Overtime_never_join_burn_in", Overtime_never_join_burn_in_count);
        //            }
        //            //如果老化调试和包装都未开始或都已经完成，则输出空值
        //            if (burn_in_Finish_Rate == "100.00%" && appearances_Finish_Rate == "100.00%" || burn_in_Finish_Rate == "--%" && appearances_Finish_Rate == "--%")
        //            {
        //                OrderNum.Add("Overtime_never_join_appearances", "");
        //            }
        //            else
        //            {
        //                //计算老化调试完超过72小时未进入包装工序输出，如果下一工序已经有记录，则不算入超时数量，只要有一个超时，就跳出foreach 循环
        //                foreach (var it in barCodesList)
        //                {
        //                    //取出老化调试已经完成的记录
        //                    var burn_in_record = Burn_in_Record.Where(c => c.BarCodesNum == it && c.OQCCheckFT != null && c.OQCCheckFinish == true).FirstOrDefault();
        //                    if (burn_in_record != null)
        //                    {
        //                        //取出老化调试完成时间
        //                        var burn_in_FinishTime_temp = burn_in_record.OQCCheckFT;
        //                        var appearances_Earliest_Record = Appearances_Record.Where(c => c.BarCodesNum == it && c.OQCCheckBT != null).OrderBy(c => c.OQCCheckBT).FirstOrDefault();
        //                        //var appearances_BeginTime_temp = Appearances_Record.Where(c => c.BarCodesNum == it && c.OQCCheckBT != null).Min(c => c.OQCCheckBT);
        //                        //DateTime appearances_BeginTime = appearances_BeginTime_temp == null ? DateTime.Now : new DateTime(appearances_BeginTime_temp.Value.Year, appearances_BeginTime_temp.Value.Month, appearances_BeginTime_temp.Value.Day, appearances_BeginTime_temp.Value.Hour, appearances_BeginTime_temp.Value.Minute, appearances_BeginTime_temp.Value.Second);
        //                        if (appearances_Earliest_Record == null)
        //                        {
        //                            DateTime burn_in_FinishTime = burn_in_FinishTime_temp == null ? DateTime.Now : new DateTime(burn_in_FinishTime_temp.Value.Year, burn_in_FinishTime_temp.Value.Month, burn_in_FinishTime_temp.Value.Day, burn_in_FinishTime_temp.Value.Hour, burn_in_FinishTime_temp.Value.Minute, burn_in_FinishTime_temp.Value.Second);
        //                            TimeSpan overtime_never_join_appearances = DateTime.Now - burn_in_FinishTime;
        //                            if (overtime_never_join_appearances.TotalHours >= 72)
        //                            {
        //                                Overtime_never_join_appearances_count++;
        //                            }
        //                        }
        //                    }
        //                    if (Overtime_never_join_appearances_count >= 1)
        //                        break;
        //                }
        //                OrderNum.Add("Overtime_never_join_appearances", Overtime_never_join_appearances_count);
        //            }

        //            #endregion

        //            #region---------------------AOD特采部分
        //            var AOD_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().AOD_Description;
        //            OrderNum.Add("AOD_Description", AOD_Description);
        //            #endregion


        //            #region---------------------异常订单部分
        //            var AbnormalOrder_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().AbnormalOrder_Description;
        //            OrderNum.Add("AbnormalOrder_Description", AbnormalOrder_Description);
        //            #endregion


        //            ProductionControlHistory.Add(i.ToString(), OrderNum);
        //            i++;
        //        }
        //    }

        //    ViewBag.History = ProductionControlHistory;
        //    return View();
        //}

        [HttpPost]
        public ActionResult ProductionControlHistory(List<string> PlatformType, List<string> orderNum)
        {
            ViewBag.PlatformType = PlatformTypeList();
            ViewBag.OrderNumList = GetOrderList();
            JObject ProductionControlHistory = new JObject();   //创建JSON对象
            //取出数据
            int i = 1;
            using (var db = new ApplicationDbContext())
            {
                var OrderList_All = (from m in db.OrderMgm select m).OrderBy(c => c.BarCodeCreated).ToList();
                if (PlatformType != null)
                {
                    OrderList_All = OrderList_All.Where(c => PlatformType.Contains(c.PlatformType)).ToList();
                }
                if (orderNum != null)
                {
                    OrderList_All = OrderList_All.Where(c => orderNum.Contains(c.OrderNum)).ToList();
                }
                //把2017-TEST-1放入排除清单中
                OrderList_All = OrderList_All.Where(c => c.OrderNum != "2017-TEST-1").ToList();
                //----------------------------------------------------------------------------------
                var newbarcodelist = db.Warehouse_Join.Where(c => c.NewBarcode != c.CartonOrderNum&&c.NewBarcode!=null).Select(c => c.NewBarcode).ToList();
                foreach (var item in OrderList_All)
                {
                    //存入JSON对象
                    var OrderNum = new JObject
                    {
                        {"Id",item.ID },
                        { "OrderNum", item.OrderNum },
                        { "Quantity", item.Boxes },
                        { "PlatformType", item.PlatformType },
                        { "PlanInputTime", item.PlanInputTime.ToString() },
                        { "PlanCompleteTime", item.PlanCompleteTime.ToString() },
                    };
                    var nuobarcodelist = new List<string>();
                    if (newbarcodelist.Contains(item.OrderNum))
                    {
                        nuobarcodelist = db.Warehouse_Join.Where(c => c.NewBarcode != c.CartonOrderNum && c.NewBarcode == item.OrderNum).Select(c => c.BarCodeNum).ToList();
                    }
                    var beginttime = db.Assemble.Where(c => c.OrderNum == item.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == item.OrderNum)).Min(c => c.PQCCheckBT);//取出订单开始装配生产的PQCCheckBT值
                    if (nuobarcodelist.Count != 0)//查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                    {
                        var temp = db.FinalQC.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).Min(c => c.FQCCheckBT);//找到挪用出库的FinalQC最小开始时间
                        beginttime = beginttime == null ? temp : (temp == null ? beginttime : (beginttime > temp ? temp : beginttime));//如果beginttime和temp都不为null,谁小beginttime取谁的值,如果其中一个为null,取另一个不为null 的值,如果都为null,取null值
                    }
                    if (beginttime == null)
                    {
                        beginttime = db.Burn_in.Where(c => c.OrderNum == item.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == item.OrderNum)).Min(c => c.OQCCheckBT);//取出订单开始老化调试的OQCCheckBT值
                        if (nuobarcodelist.Count != 0)// 查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                        {
                            var temp = db.Burn_in.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).Min(c => c.OQCCheckBT);//找到挪用出库的Burn_in最小开始时间
                            beginttime = beginttime == null ? temp : (temp == null ? beginttime : (beginttime > temp ? temp : beginttime));//如果beginttime和temp都不为null,谁小beginttime取谁的值,如果其中一个为null,取另一个不为null 的值,如果都为null,取null值
                        }
                        if (beginttime == null)
                        {
                            beginttime = db.CalibrationRecord.Where(c => c.OrderNum == item.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == item.OrderNum)).Min(c => c.BeginCalibration);//取出订单开始校正的BeginCalibration值
                            if (nuobarcodelist.Count != 0)// 查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                            {
                                var temp = db.CalibrationRecord.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).Min(c => c.BeginCalibration);//找到挪用出库的CalibrationRecord最小开始时间
                                beginttime = beginttime == null ? temp : (temp == null ? beginttime : (beginttime > temp ? temp : beginttime));//如果beginttime和temp都不为null,谁小beginttime取谁的值,如果其中一个为null,取另一个不为null 的值,如果都为null,取null值
                            }
                            if (beginttime == null)
                            {
                                beginttime = db.Appearance.Where(c => c.OrderNum == item.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == item.OrderNum)).Min(c => c.OQCCheckBT);//取出订单开始包装电检检查的OQCCheckBT值
                                if (nuobarcodelist.Count != 0)// 查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                                {
                                    var temp = db.Appearance.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).Min(c => c.OQCCheckBT);//找到挪用出库的Appearance最小开始时间
                                    beginttime = beginttime == null ? temp : (temp == null ? beginttime : (beginttime > temp ? temp : beginttime));//如果beginttime和temp都不为null,谁小beginttime取谁的值,如果其中一个为null,取另一个不为null 的值,如果都为null,取null值
                                }
                            }
                        }
                    }
                    if (beginttime != null)
                    {
                        OrderNum.Add("ActualProductionTime", beginttime.ToString());
                    }
                    else
                    {
                        OrderNum.Add("ActualProductionTime", "未开始");
                    }

                    //var finishtime = db.Appearance.Where(c => c.OrderNum == item.OrderNum).Max(c => c.OQCCheckFT);//取出最后包装记录的OQCCheckFT值
                    //var totaltime = finishtime - beginttime;
                    //OrderNum.Add("ActualFinishTime", finishtime.ToString());
                    //OrderNum.Add("TotalTime", totaltime.ToString());
                    //完成时间
                    int modelGroupQuantity = OrderList_All.Where(c => c.OrderNum == item.OrderNum).FirstOrDefault().Boxes;//订单模组数量
                    DateTime? finishtime = new DateTime();
                    var current = com.GetCurrentwarehousList(item.OrderNum);
                    if (current.Count(c => c.OrderNum == item.OrderNum && c.IsOut == true) >= modelGroupQuantity)
                    {
                        finishtime = current.Where(c => c.OrderNum == item.OrderNum && c.IsOut == true).Max(c => c.WarehouseOutDate);
                    }
                    else
                    {
                        finishtime = null;
                    }
                    var totaltime = finishtime == null ? "" : (finishtime - beginttime).ToString();
                    OrderNum.Add("ActualFinishTime", finishtime.ToString());
                    OrderNum.Add("TotalTime", totaltime);


                    #region-------------------组装部分
                    //-------------------组装部分
                    var AssembleRecord = (from m in db.Assemble where m.OrderNum == item.OrderNum && (m.OldOrderNum == null || m.OldOrderNum == item.OrderNum) select m).ToList();//查出OrderNum的所有组装记录
                    if (nuobarcodelist.Count != 0)//查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                    {
                        var temp = db.Assemble.Where(c => nuobarcodelist.Contains(c.BoxBarCode) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).ToList();//找到挪用出库的Assemble的数据
                        AssembleRecord.AddRange(temp);//将数据加入AssembleRecord中
                    }
                    if (AssembleRecord.Count() > 0)
                    {
                        Decimal Assemble_Normal = AssembleRecord.Where(m => m.PQCCheckAbnormal == "正常").Count();//组装PQC正常个数
                        OrderNum.Add("Assemble_Finish", Convert.ToInt32(Assemble_Normal));
                        OrderNum.Add("AssembleRecord_Count", AssembleRecord.Count());
                        //计算组装完成率、合格率
                        if (Assemble_Normal == 0)
                        {
                            OrderNum.Add("Assemble_Finish_Rate", "0%");
                            OrderNum.Add("Assemble_Pass_Rate", "0%");
                        }
                        else
                        {
                            OrderNum.Add("Assemble_Finish_Rate", (Assemble_Normal / item.Boxes * 100).ToString("F2") + "%");
                            OrderNum.Add("Assemble_Pass_Rate", (Assemble_Normal / AssembleRecord.Count() * 100).ToString("F2") + "%");
                        }
                    }
                    else
                    {
                        OrderNum.Add("Assemble_Finish_Rate", "--%");
                        OrderNum.Add("Assemble_Pass_Rate", "--%");
                    }
                    #endregion

                    #region-------------------FQC部分
                    var FinalQC_Record = db.FinalQC.Where(c => c.OrderNum == item.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == item.OrderNum)).ToList();
                    if (nuobarcodelist.Count != 0)//查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                    {
                        var temp = db.FinalQC.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).ToList();//找到挪用出库的FinalQC的数据
                        FinalQC_Record.AddRange(temp);//将数据加入FinalQC_Record中
                    }
                    if (FinalQC_Record.Count > 0)
                    {
                        Decimal FinalQC_Finish = FinalQC_Record.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Count();
                        OrderNum.Add("FinalQC_Finish", Convert.ToInt32(FinalQC_Finish)); //FQC完成个数
                        OrderNum.Add("FinalQC_Record_Count", FinalQC_Record.Count);      //FQC记录个数
                        OrderNum.Add("FinalQC_Spot_Count", FinalQC_Record.Select(c => c.BarCodesNum).Distinct().Count());      //FQC抽检个数 
                        if (FinalQC_Finish == 0)
                        {
                            OrderNum.Add("FinalQC_Finish_Rate", "0%"); //FQC完成率
                            OrderNum.Add("FinalQC_Pass_Rate", "0%");   //FQC合格率
                        }
                        else
                        {
                            //OrderNum.Add("FinalQC_Finish_Rate", (FinalQC_Finish / FinalQC_Record.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2") + "%");  //FQC完成率
                            OrderNum.Add("FinalQC_Finish_Rate", (FinalQC_Finish / item.Boxes * 100).ToString("F2") + "%");  //FQC抽检比例
                            OrderNum.Add("FinalQC_Pass_Rate", (FinalQC_Finish / FinalQC_Record.Count() * 100).ToString("F2") + "%"); //FQC合格率
                        }
                    }
                    else
                    {
                        OrderNum.Add("FinalQC_Finish_Rate", "--%");  //FQC完成率
                        OrderNum.Add("FinalQC_Pass_Rate", "--%");    //FQC合格率
                    }
                    #endregion

                    #region--------------------老化部分
                    //--------------------老化部分
                    var Burn_in_Record = (from m in db.Burn_in where m.OrderNum == item.OrderNum && (m.OldOrderNum == null || m.OldOrderNum == item.OrderNum) select m).ToList();//查出OrderNum的所有老化记录
                    if (nuobarcodelist.Count != 0)//查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                    {
                        var temp = db.Burn_in.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).ToList();//找到挪用出库的Burn_in的数据
                        Burn_in_Record.AddRange(temp);//将数据加入Burn_in_Record中
                    }
                    int Burn_in_Record_Count = Burn_in_Record.Select(m => m.BarCodesNum).Distinct().ToList().Count();
                    if (Burn_in_Record.Count() > 0)
                    {
                        Decimal Burn_in_Normal = Burn_in_Record.Where(m => m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化正常个数
                        //Decimal Burn_in_FirstPass = Burn_in_Record.Where(m => m.OQCCheckFinish == true && m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化工序直通个数
                        Decimal Burn_in_Finish = Burn_in_Record.Count(m => m.OQCCheckFinish == true); //完成老化工序的个数
                        OrderNum.Add("Burn_in_Finish", Convert.ToInt32(Burn_in_Finish));
                        OrderNum.Add("Burn_in_Record_Count", Burn_in_Record.Count());
                        OrderNum.Add("Burn_in_Count", Burn_in_Record_Count);
                        //计算老化完成率、合格率
                        //if (Burn_in_Finish == 0)
                        //{
                        //    OrderNum.Add("Burn_in_Finish_Rate", "0%");
                        //    OrderNum.Add("Burn_in_Pass_Rate", "0%");
                        //}
                        //else
                        //{
                        //OrderNum.Add("Burn_in_Finish_Rate", (Burn_in_Finish / Burn_in_Record_Count * 100).ToString("F2") + "%");
                        OrderNum.Add("Burn_in_Finish_Rate", (Convert.ToDecimal(Burn_in_Record.Count()) / item.Boxes * 100).ToString("F2") + "%");
                        OrderNum.Add("Burn_in_Pass_Rate", (Burn_in_Finish / Burn_in_Record.Count() * 100).ToString("F2") + "%");
                        //}
                    }
                    else
                    {
                        OrderNum.Add("Burn_in_Finish_Rate", "--%");
                        OrderNum.Add("Burn_in_Pass_Rate", "--%");
                    }
                    #endregion

                    #region---------------------校正部分
                    //---------------------校正部分
                    var Calibration_Record = (from m in db.CalibrationRecord where m.OrderNum == item.OrderNum && (m.OldOrderNum == null || m.OldOrderNum == item.OrderNum) && m.RepetitionCalibration == false select m).ToList();//查出OrderNum的所有校正记录
                    if (nuobarcodelist.Count != 0)//查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                    {
                        var temp = db.CalibrationRecord.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).ToList();//找到挪用出库的CalibrationRecord的数据
                        Calibration_Record.AddRange(temp);//将数据加入Calibration_Record中
                    }
                    var Calibration_Record_Count = Calibration_Record.Select(m => m.BarCodesNum).Distinct().Count();//条码记录去重
                    if (Calibration_Record_Count <= 1)
                    {
                        Calibration_Record_Count = Calibration_Record.Count();
                    }
                    if (Calibration_Record.Count() > 0)
                    {
                        Decimal Calibration_Normal = Calibration_Record.Where(m => m.Normal == true).Count();//校正正常个数
                        OrderNum.Add("Calibration_Finish", Convert.ToInt32(Calibration_Normal));
                        OrderNum.Add("Calibration_Record_Count", Calibration_Record.Count());
                        OrderNum.Add("Calibration_Count", Calibration_Record_Count);
                        //计算校正完成率、合格率
                        if (Calibration_Normal == 0)
                        {
                            OrderNum.Add("Calibration_Finish_Rate", "0%");
                            OrderNum.Add("Calibration_Pass_Rate", "0%");
                        }
                        else
                        {
                            //OrderNum.Add("Calibration_Finish_Rate", (Calibration_Normal / Calibration_Record_Count * 100).ToString("F2") + "%");
                            OrderNum.Add("Calibration_Finish_Rate", (Calibration_Normal / item.Boxes * 100).ToString("F2") + "%");
                            OrderNum.Add("Calibration_Pass_Rate", (Calibration_Normal / Calibration_Record.Count() * 100).ToString("F2") + "%");
                        }
                    }
                    else
                    {
                        OrderNum.Add("Calibration_Finish_Rate", "--%");
                        OrderNum.Add("Calibration_Pass_Rate", "--%");
                    }
                    #endregion

                    #region---------------------外观包装部分
                    //---------------------外观包装部分
                    //var Appearances_Record = (from m in db.Appearance where m.OrderNum == item.OrderNum select m).OrderBy(m=>m.BarCodesNum).ToList();//查出OrderNum的所有外观包装记录
                    var Appearances_Record = db.Appearance.Where(m => m.OrderNum == item.OrderNum && (m.OldOrderNum == null || m.OldOrderNum == item.OrderNum)).OrderBy(m => m.BarCodesNum).ToList();
                    if (nuobarcodelist.Count != 0)//查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                    {
                        var temp = db.Appearance.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).ToList();//找到挪用出库的Appearance的数据
                        Appearances_Record.AddRange(temp);//将数据加入Appearances_Record中
                    }
                    List<string> Appearances_Record_Count = Appearances_Record.Select(m => m.BarCodesNum).Distinct().ToList();//条码记录去重后个数
                    //HashSet<string> Appearances_Record_Count2 = new HashSet<string>(Appearances_Record_Count);
                    if (Appearances_Record.Count() > 0)
                    {
                        //Decimal Appearances_Normal = Appearances_Record.Where(m => m.Appearance_OQCCheckAbnormal == "正常").Count();//外观包装正常个数
                        Decimal Appearances_Finish = Appearances_Record.Where(m => m.OQCCheckFinish == true).Count();//外观包装完成个数
                        OrderNum.Add("Appearances_Finish", Convert.ToInt32(Appearances_Finish));
                        OrderNum.Add("Appearances_Record_Count", Appearances_Record.Count());
                        OrderNum.Add("Appearances_Count", Appearances_Record_Count.Count());
                        //计算外观包装完成率、合格率
                        if (Appearances_Finish == 0)
                        {
                            OrderNum.Add("Appearances_Finish_Rate", "0%");
                            OrderNum.Add("Appearances_Pass_Rate", "0%");
                        }
                        else
                        {
                            //OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / Appearances_Record_Count.Count() * 100).ToString("F2") + "%");//完成数/条码记录去重后个数
                            OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / item.Boxes * 100).ToString("F2") + "%");//完成数/条码记录去重后个数
                            OrderNum.Add("Appearances_Pass_Rate", (Appearances_Finish / Appearances_Record.Count() * 100).ToString("F2") + "%");//完成数/记录条数
                        }
                    }
                    else
                    {
                        //使用库存出库订单
                        Appearances_Record = db.Appearance.Where(c => c.ToOrderNum == item.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == item.OrderNum)).ToList();
                        Appearances_Record_Count = Appearances_Record.Select(m => m.BarCodesNum).Distinct().ToList();//条码记录去重
                        //Appearances_Record_Count2 = new HashSet<string>(Appearances_Record_Count);
                        if (Appearances_Record.Count() > 0)
                        {
                            Decimal Appearances_Finish = Appearances_Record.Where(m => m.OQCCheckFinish == true).Count();//外观包装完成个数
                            OrderNum.Add("Appearances_Finish", Convert.ToInt32(Appearances_Finish));
                            OrderNum.Add("Appearances_Record_Count", Appearances_Record.Count());
                            OrderNum.Add("Appearances_Count", Appearances_Record_Count.Count());
                            OrderNum.Remove("ActualProductionTime");
                            OrderNum.Add("ActualProductionTime", Appearances_Record.Min(c => c.OQCCheckBT).ToString()); //取出最早记录的包装OQCCheckBT值
                            //计算外观包装完成率、合格率
                            if (Appearances_Finish == 0)
                            {
                                OrderNum.Add("Appearances_Finish_Rate", "0%");
                                OrderNum.Add("Appearances_Pass_Rate", "0%");
                            }
                            else
                            {
                                //OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / Appearances_Record_Count.Count() * 100).ToString("F2") + "%");//完成数/条码记录去重后个数
                                OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / item.Boxes * 100).ToString("F2") + "%");//完成数/条码记录去重后个数
                                OrderNum.Add("Appearances_Pass_Rate", (Appearances_Finish / Appearances_Record.Count() * 100).ToString("F2") + "%");//完成数/记录条数
                            }


                        }
                        else
                        {
                            OrderNum.Add("Appearances_Finish_Rate", "--%");
                            OrderNum.Add("Appearances_Pass_Rate", "--%");
                        }
                    }
                    #endregion

                    #region---------------------PQC超24小时未进入老化工序和老化后72小时未进入包装工序提示信息部分
                    //此条码PQC完成，到现在为止超过24小时未有老化记录，输出提示；
                    //此条码PQC完成，老化的开始时间－PQC完成时间超24小时，输出提示；
                    //此条码老化完成，到现在为止超过72小时未有包装记录，输出提示；
                    //此条码老化完成，包装的开始时间－老化完成时间超72小时，输出提示；
                    List<string> barCodesList = db.BarCodes.Where(c => c.OrderNum == item.OrderNum).OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();
                    int Overtime_never_join_burn_in_count = 0;
                    int FinalQC_Overtime_never_join_burn_in_count = 0;
                    int Overtime_never_join_appearances_count = 0;
                    //var AssembleRecord_count = AssembleRecord.Where(c => c.OrderNum == item.OrderNum).Count();
                    string assemble_Finish_Rate = (string)OrderNum["Assemble_Finish_Rate"];
                    string FinalQC_Finish_Rate = (string)OrderNum["FinalQC_Finish_Rate"];
                    var burn_in_Finish_Rate = OrderNum.Value<string>("Burn_in_Finish_Rate");
                    var appearances_Finish_Rate = OrderNum.Value<string>("Appearances_Finish_Rate");
                    //如果组装和老化调试都未开始或都已经完成，则输出空值
                    if (assemble_Finish_Rate == "100.00%" && burn_in_Finish_Rate == "100.00%" || assemble_Finish_Rate == "--%" && burn_in_Finish_Rate == "--%")
                    {
                        OrderNum.Add("Overtime_never_join_burn_in", "");
                    }
                    else
                    {
                        //计算PQC完超过24小时未进入老化调试输出，如果下一工序已经有记录，则不算入超时数量，只要有一个超时，就跳出foreach 循环
                        foreach (var it in barCodesList)
                        {
                            //取出组装已经完成的记录
                            var assemble_record = AssembleRecord.Where(c => c.BoxBarCode == it && c.PQCCheckFT != null && c.PQCCheckFinish == true && c.RepetitionPQCCheck != true).FirstOrDefault();
                            if (assemble_record != null)
                            {
                                //取出组装完成时间
                                var assembleFinishTime_temp = assemble_record.PQCCheckFT;
                                var burn_in_Earliest_Record = Burn_in_Record.Where(c => c.BarCodesNum == it && c.OQCCheckBT != null).OrderBy(c => c.OQCCheckBT).FirstOrDefault();
                                //如果老化调试最早时间为空，则老化调试没有记录
                                if (burn_in_Earliest_Record == null)
                                {
                                    ////取出老化调试最早时间
                                    //var burn_in_BeginTime_temp = Burn_in_Record.Where(c => c.BarCodesNum == it && c.OQCCheckBT != null).Min(c => c.OQCCheckBT);
                                    //DateTime burn_in_BeginTime = burn_in_BeginTime_temp == null ? DateTime.Now : new DateTime(burn_in_BeginTime_temp.Value.Year, burn_in_BeginTime_temp.Value.Month, burn_in_BeginTime_temp.Value.Day, burn_in_BeginTime_temp.Value.Hour, burn_in_BeginTime_temp.Value.Minute, burn_in_BeginTime_temp.Value.Second);
                                    DateTime assembleFinishTime = assembleFinishTime_temp == null ? DateTime.Now : new DateTime(assembleFinishTime_temp.Value.Year, assembleFinishTime_temp.Value.Month, assembleFinishTime_temp.Value.Day, assembleFinishTime_temp.Value.Hour, assembleFinishTime_temp.Value.Minute, assembleFinishTime_temp.Value.Second);
                                    TimeSpan overtime_never_join_appearances = DateTime.Now - assembleFinishTime;
                                    if (overtime_never_join_appearances.TotalHours >= 24)
                                    {
                                        Overtime_never_join_burn_in_count++;
                                    }
                                }
                            }
                            if (Overtime_never_join_burn_in_count >= 1)
                                break;
                        }
                        OrderNum.Add("Overtime_never_join_burn_in", Overtime_never_join_burn_in_count);
                    }

                    //如果FQC和老化调试都未开始或都已经完成，则输出空值
                    if (FinalQC_Finish_Rate == "100.00%" && burn_in_Finish_Rate == "100.00%" || FinalQC_Finish_Rate == "--%" && burn_in_Finish_Rate == "--%")
                    {
                        OrderNum.Add("FinalQC_Overtime_never_join_burn_in", "");
                    }
                    else
                    {
                        //计算FQC完超过24小时未进入老化调试输出，如果下一工序已经有记录，则不算入超时数量，只要有一个超时，就跳出foreach 循环
                        foreach (var it in barCodesList)
                        {
                            //取出FQC已经完成的记录
                            var FinalQC_record = FinalQC_Record.Where(c => c.BarCodesNum == it && c.FQCCheckFT != null && c.FQCCheckFinish == true && c.RepetitionFQCCheck != true).FirstOrDefault();
                            if (FinalQC_record != null)
                            {
                                //取出FQC完成时间
                                var FinalQCFinishTime_temp = FinalQC_record.FQCCheckFT;
                                var burn_in_Earliest_Record = Burn_in_Record.Where(c => c.BarCodesNum == it && c.OQCCheckBT != null).OrderBy(c => c.OQCCheckBT).FirstOrDefault();
                                //如果老化调试最早时间为空，则老化调试没有记录
                                if (burn_in_Earliest_Record == null)
                                {
                                    DateTime FinalQCFinishTime = FinalQCFinishTime_temp == null ? DateTime.Now : Convert.ToDateTime(FinalQCFinishTime_temp);//new DateTime(FinalQCFinishTime_temp.Value.Year, FinalQCFinishTime_temp.Value.Month, FinalQCFinishTime_temp.Value.Day, FinalQCFinishTime_temp.Value.Hour, FinalQCFinishTime_temp.Value.Minute, FinalQCFinishTime_temp.Value.Second);
                                    TimeSpan FinalQC_overtime_never_join_burn_in = DateTime.Now - FinalQCFinishTime;
                                    if (FinalQC_overtime_never_join_burn_in.TotalHours >= 24)
                                    {
                                        FinalQC_Overtime_never_join_burn_in_count++;
                                    }
                                }
                            }
                            if (FinalQC_Overtime_never_join_burn_in_count >= 1)
                                break;
                        }
                        OrderNum.Add("FinalQC_Overtime_never_join_burn_in", FinalQC_Overtime_never_join_burn_in_count);
                    }


                    //如果老化调试和包装都未开始或都已经完成，则输出空值
                    if (burn_in_Finish_Rate == "100.00%" && appearances_Finish_Rate == "100.00%" || burn_in_Finish_Rate == "--%" && appearances_Finish_Rate == "--%")
                    {
                        OrderNum.Add("Overtime_never_join_appearances", "");
                    }
                    else
                    {
                        //计算老化调试完超过72小时未进入包装工序输出，如果下一工序已经有记录，则不算入超时数量，只要有一个超时，就跳出foreach 循环
                        foreach (var it in barCodesList)
                        {
                            //取出老化调试已经完成的记录
                            var burn_in_record = Burn_in_Record.Where(c => c.BarCodesNum == it && c.OQCCheckFT != null && c.OQCCheckFinish == true).FirstOrDefault();
                            if (burn_in_record != null)
                            {
                                //取出老化调试完成时间
                                var burn_in_FinishTime_temp = burn_in_record.OQCCheckFT;
                                var appearances_Earliest_Record = Appearances_Record.Where(c => c.BarCodesNum == it && c.OQCCheckBT != null).OrderBy(c => c.OQCCheckBT).FirstOrDefault();
                                //var appearances_BeginTime_temp = Appearances_Record.Where(c => c.BarCodesNum == it && c.OQCCheckBT != null).Min(c => c.OQCCheckBT);
                                //DateTime appearances_BeginTime = appearances_BeginTime_temp == null ? DateTime.Now : new DateTime(appearances_BeginTime_temp.Value.Year, appearances_BeginTime_temp.Value.Month, appearances_BeginTime_temp.Value.Day, appearances_BeginTime_temp.Value.Hour, appearances_BeginTime_temp.Value.Minute, appearances_BeginTime_temp.Value.Second);
                                if (appearances_Earliest_Record == null)
                                {
                                    DateTime burn_in_FinishTime = burn_in_FinishTime_temp == null ? DateTime.Now : new DateTime(burn_in_FinishTime_temp.Value.Year, burn_in_FinishTime_temp.Value.Month, burn_in_FinishTime_temp.Value.Day, burn_in_FinishTime_temp.Value.Hour, burn_in_FinishTime_temp.Value.Minute, burn_in_FinishTime_temp.Value.Second);
                                    TimeSpan overtime_never_join_appearances = DateTime.Now - burn_in_FinishTime;
                                    if (overtime_never_join_appearances.TotalHours >= 72)
                                    {
                                        Overtime_never_join_appearances_count++;
                                    }
                                }
                            }
                            if (Overtime_never_join_appearances_count >= 1)
                                break;
                        }
                        OrderNum.Add("Overtime_never_join_appearances", Overtime_never_join_appearances_count);
                    }

                    #endregion

                    #region---------------------AOD特采部分
                    var AOD_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().AOD_Description;
                    OrderNum.Add("AOD_Description", AOD_Description);
                    if (!string.IsNullOrEmpty(AOD_Description))
                    {
                        if (!comm.CheckJpgExit(item.OrderNum, "AOD_Files"))
                            OrderNum.Add("AOD_DescriptionJpg", "true");
                        else
                            OrderNum.Add("AOD_DescriptionJpg", "false");

                        if (!comm.CheckpdfExit(item.OrderNum, "AOD_Files"))
                            OrderNum.Add("AOD_DescriptionPdf", "true");
                        else
                            OrderNum.Add("AOD_DescriptionPdf", "false");
                    }

                    #endregion

                    #region---------------------异常订单部分
                    var AbnormalOrder_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().AbnormalOrder_Description;
                    OrderNum.Add("AbnormalOrder_Description", AbnormalOrder_Description);
                    #endregion

                    #region---------------------组装首样部分
                    var AssembleFirstSample_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().AssembleFirstSample_Description;
                    OrderNum.Add("AssembleFirstSample_Description", AssembleFirstSample_Description);
                    if (!string.IsNullOrEmpty(AssembleFirstSample_Description))
                    {
                        if (!comm.CheckJpgExit(item.OrderNum, "AssembleSample_Files"))
                            OrderNum.Add("AssembleFirstSample_DescriptionJpg", "true");
                        else
                            OrderNum.Add("AssembleFirstSample_DescriptionJpg", "false");

                        if (!comm.CheckpdfExit(item.OrderNum, "AssembleSample_Files"))
                            OrderNum.Add("AssembleFirstSample_DescriptionPdf", "true");
                        else
                            OrderNum.Add("AssembleFirstSample_DescriptionPdf", "false");
                    }
                    #endregion

                    #region---------------------组装异常部分
                    var AssembleAbnormal_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().AssembleAbnormal_Description;
                    OrderNum.Add("AssembleAbnormal_Description", AssembleAbnormal_Description);
                    if (!string.IsNullOrEmpty(AssembleAbnormal_Description))
                    {
                        if (!comm.CheckJpgExit(item.OrderNum, "AssembleAbnormalOrder_Files"))
                            OrderNum.Add("AssembleAbnormal_DescriptionJpg", "true");
                        else
                            OrderNum.Add("AssembleAbnormal_DescriptionJpg", "false");

                        if (!comm.CheckpdfExit(item.OrderNum, "AssembleAbnormalOrder_Files"))
                            OrderNum.Add("AssembleAbnormal_DescriptionPdf", "true");
                        else
                            OrderNum.Add("AssembleAbnormal_DescriptionPdf", "false");
                    }
                    #endregion

                    #region---------------------老化首样部分
                    var BurnInFirstSample_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().BurnInFirstSample_Description;
                    OrderNum.Add("BurnInFirstSample_Description", BurnInFirstSample_Description);
                    if (!string.IsNullOrEmpty(BurnInFirstSample_Description))
                    {
                        if (!comm.CheckJpgExit(item.OrderNum, "BurnInSample_Files"))
                            OrderNum.Add("BurnInFirstSample_DescriptionJpg", "true");
                        else
                            OrderNum.Add("BurnInFirstSample_DescriptionJpg", "false");

                        if (!comm.CheckpdfExit(item.OrderNum, "BurnInSample_Files"))
                            OrderNum.Add("BurnInFirstSample_DescriptionPdf", "true");
                        else
                            OrderNum.Add("BurnInFirstSample_DescriptionPdf", "false");
                    }
                    #endregion

                    #region---------------------老化异常部分
                    var BurninAbnormal_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().BurninAbnormal_Description;
                    OrderNum.Add("BurninAbnormal_Description", BurninAbnormal_Description);//BurnInAbnormalOrder_Files
                    if (!string.IsNullOrEmpty(BurninAbnormal_Description))
                    {
                        if (!comm.CheckJpgExit(item.OrderNum, "BurnInAbnormalOrder_Files"))
                            OrderNum.Add("BurninAbnormal_DescriptionJpg", "true");
                        else
                            OrderNum.Add("BurninAbnormal_DescriptionJpg", "false");

                        if (!comm.CheckpdfExit(item.OrderNum, "BurnInAbnormalOrder_Files"))
                            OrderNum.Add("BurninAbnormal_DescriptionPdf", "true");
                        else
                            OrderNum.Add("BurninAbnormal_DescriptionPdf", "false");
                    }
                    #endregion

                    #region---------------------包装首样部分
                    var AppearanceFirstSample_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().AppearanceFirstSample_Description;
                    OrderNum.Add("AppearanceFirstSample_Description", AppearanceFirstSample_Description);
                    if (!string.IsNullOrEmpty(AppearanceFirstSample_Description))
                    {
                        if (!comm.CheckJpgExit(item.OrderNum, "AppearanceSample_Files"))
                            OrderNum.Add("AppearanceFirstSample_DescriptionJpg", "true");
                        else
                            OrderNum.Add("AppearanceFirstSample_DescriptionJpg", "false");

                        if (!comm.CheckpdfExit(item.OrderNum, "AppearanceSample_Files"))
                            OrderNum.Add("AppearanceFirstSample_DescriptionPdf", "true");
                        else
                            OrderNum.Add("AppearanceFirstSample_DescriptionPdf", "false");
                    }
                    #endregion

                    #region---------------------包装异常部分
                    var AppearanceAbnormal_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().AppearanceAbnormal_Description;
                    OrderNum.Add("AppearanceAbnormal_Description", AppearanceAbnormal_Description);
                    if (!string.IsNullOrEmpty(AppearanceAbnormal_Description))
                    {
                        if (!comm.CheckJpgExit(item.OrderNum, "AppearanceAbnormalOrder_Files"))
                            OrderNum.Add("AppearanceAbnormal_DescriptionJpg", "true");
                        else
                            OrderNum.Add("AppearanceAbnormal_DescriptionJpg", "false");

                        if (!comm.CheckpdfExit(item.OrderNum, "AppearanceAbnormalOrder_Files"))
                            OrderNum.Add("AppearanceAbnormal_DescriptionPdf", "true");
                        else
                            OrderNum.Add("AppearanceAbnormal_DescriptionPdf", "false");
                    }
                    #endregion

                    #region --------------------------SMT首样
                    //小样进度
                    OrderNum.Add("HandSampleScedule", item.HandSampleScedule);
                    //是否有图片
                    if (comm.CheckJpgExit(item.OrderNum, "SmallSample_Files"))
                        OrderNum.Add("HandSampleSceduleJpg", "false");
                    else
                        OrderNum.Add("HandSampleSceduleJpg", "true");
                    //是否有PDF文件
                    if (comm.CheckpdfExit(item.OrderNum, "SmallSample_Files"))
                        OrderNum.Add("HandSampleScedulePdf", "false");
                    else
                        OrderNum.Add("HandSampleScedulePdf", "true");

                    //是否有小样报告
                    var sample=db.Small_Sample.OrderBy(c=>c.Id).Where(c => (c.OrderNumber == item.OrderNum || c.OrderNumber.Contains(item.OrderNum)) && c.Approved!=null && c.ApprovedResult == true).ToList();
                    if (sample.Count != 0)
                    {
                        JArray sampleJarray = new JArray();
                        int k = 1;
                        foreach (var sampleitem in sample)
                        {
                            JObject sampleJobject = new JObject();
                            sampleJobject.Add("id", sampleitem.Id);
                            sampleJobject.Add("Name", "NO." + k);
                            k++;
                            sampleJarray.Add(sampleJobject);
                        }
                        OrderNum.Add("HandSampleSceduleReport", sampleJarray);
                    }
                    else
                        OrderNum.Add("HandSampleSceduleReport", "false");

                    #endregion

                    #region------------------------- SMT异常
                    var SMTAbnormal_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().SMTAbnormal_Description;
                    OrderNum.Add("SMTAbnormal_Description", SMTAbnormal_Description);
                    if (!string.IsNullOrEmpty(SMTAbnormal_Description))
                    {
                        if (!comm.CheckJpgExit(item.OrderNum, "SMTAbnormalOrder_Files"))
                            OrderNum.Add("SMTAbnormal_DescriptionJpg", "true");
                        else
                            OrderNum.Add("SMTAbnormal_DescriptionJpg", "false");

                        if (!comm.CheckpdfExit(item.OrderNum, "SMTAbnormalOrder_Files"))
                            OrderNum.Add("SMTAbnormal_DescriptionPdf", "true");
                        else
                            OrderNum.Add("SMTAbnormal_DescriptionPdf", "false");
                    }
                    #endregion

                    //模块数
                    var ModelNum = 0;
                    //var adapterNum = 0;
                    //var powerNum = 0;
                    var NormalNumSum = 0;
                    var AbnormalNumSum = 0;
                    //var lampNormalNumSum = 0;
                    //var lampAbnormalNumSum = 0;
                    //var adapterNormalNumSum = 0;
                    //var adapterAbnormalNumSum = 0;
                    //var powerNormalNumSum = 0;
                    //var powerAbnormalNumSum = 0;
                    JObject FinishRateitem = new JObject();
                    JObject FinishRate = new JObject();
                    JObject displayFinishRate = new JObject();
                    JObject PassRateitem = new JObject();
                    JObject PassRate = new JObject();
                    JObject displayPassRate = new JObject();
                    //JObject total = new JObject();
                    var jobcontenlist = db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum).Select(c => c.JobContent).Distinct().ToList();
                    int j = 0;
                    foreach (var jobconten in jobcontenlist)
                    {
                        if (jobconten == "灯面" || jobconten == "IC面")
                        {
                            ModelNum = item.Models;
                        }
                        else if (jobconten.Contains("转接卡") == true)
                        {
                            ModelNum = item.AdapterCard;
                        }
                        else if (jobconten.Contains("电源") == true)
                        {
                            ModelNum = item.Powers;
                        }
                        //对应工作内容良品总数
                        NormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == jobconten).Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == jobconten).Sum(c => c.NormalCount);

                        //对应工作内容不良品总数
                        AbnormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == jobconten).Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == jobconten).Sum(c => c.AbnormalCount);

                        //面
                        FinishRateitem.Add("jobconten", jobconten);
                        //总完成率分子
                        FinishRateitem.Add("FinishRateMolecule", NormalNumSum);
                        //总完成率分母
                        FinishRateitem.Add("FinishRateDenominator", ModelNum);
                        //总完成率
                        FinishRateitem.Add("FinishRate", ModelNum == 0 ? "" : (((decimal)(NormalNumSum + AbnormalNumSum)) / ModelNum * 100).ToString("F2") + "%");

                        //面
                        PassRateitem.Add("jobconten", jobconten);
                        //总合格率分子
                        PassRateitem.Add("PassRateMolecule", NormalNumSum);
                        //总合格率分母
                        PassRateitem.Add("PassRateDenominator", NormalNumSum + AbnormalNumSum);
                        //总合格率
                        PassRateitem.Add("PassRate", (AbnormalNumSum + NormalNumSum) == 0 ? "" : ((decimal)NormalNumSum / (NormalNumSum + AbnormalNumSum) * 100).ToString("F2") + "%");

                        if (jobconten == "灯面" || jobconten == "IC面")
                        {
                            displayFinishRate.Add(j.ToString(), FinishRateitem);
                            displayPassRate.Add(j.ToString(), PassRateitem);
                        }

                        FinishRate.Add(j.ToString(), FinishRateitem);
                        PassRate.Add(j.ToString(), PassRateitem);
                        FinishRateitem = new JObject();
                        PassRateitem = new JObject();
                        j++;
                    }
                    OrderNum.Add("displayFinishRate", displayFinishRate);
                    OrderNum.Add("extendFinishRate", FinishRate);
                    OrderNum.Add("displayPassRate", displayPassRate);
                    OrderNum.Add("dextendPassRate", PassRate);

                    #region 入出库
                    var joincount = current.Where(c=>c.CartonOrderNum==item.OrderNum).Select(c => c.OuterBoxBarcode).Distinct().Count(); ;
                    var outcount = current.Where(c => c.IsOut == true&& c.CartonOrderNum == item.OrderNum).Select(c => c.OuterBoxBarcode).Distinct().Count();
                    OrderNum.Add("joinAndOutDepot", (joincount == 0 ? "--" : joincount.ToString()) + "/" + (outcount == 0 ? "--" : outcount.ToString()));
                    #endregion

                    ProductionControlHistory.Add(i.ToString(), OrderNum);
                    i++;
                    #endregion
                }
            }

            ViewBag.History = ProductionControlHistory;
            return View();
        }

        #region -----------------JSON格式化重新排序

        /// <summary>
        /// JSON格式化重新排序
        /// </summary>
        /// <param name="jobj">原始JSON JToken.Parse(string json);</param>
        /// <param name="obj">初始值Null</param>
        /// <returns></returns>
        public static string SortJson(JToken jobj, JToken obj)
        {
            if (obj == null)
            {
                obj = new JObject();
            }
            List<JToken> list = jobj.ToList<JToken>();
            if (jobj.Type == JTokenType.Object)//非数组
            {
                List<string> listsort = new List<string>();
                foreach (var item in list)
                {
                    string name = JProperty.Load(item.CreateReader()).Name;
                    listsort.Add(name);
                }
                listsort.Sort();
                List<JToken> listTemp = new List<JToken>();
                foreach (var item in listsort)
                {
                    listTemp.Add(list.Where(p => JProperty.Load(p.CreateReader()).Name == item).FirstOrDefault());
                }
                list = listTemp;
                //list.Sort((p1, p2) => JProperty.Load(p1.CreateReader()).Name.GetAnsi() - JProperty.Load(p2.CreateReader()).Name.GetAnsi());

                foreach (var item in list)
                {
                    JProperty jp = JProperty.Load(item.CreateReader());
                    if (item.First.Type == JTokenType.Object)
                    {
                        JObject sub = new JObject();
                        (obj as JObject).Add(jp.Name, sub);
                        SortJson(item.First, sub);
                    }
                    else if (item.First.Type == JTokenType.Array)
                    {
                        JArray arr = new JArray();
                        if (obj.Type == JTokenType.Object)
                        {
                            (obj as JObject).Add(jp.Name, arr);
                        }
                        else if (obj.Type == JTokenType.Array)
                        {
                            (obj as JArray).Add(arr);
                        }
                        SortJson(item.First, arr);
                    }
                    else if (item.First.Type != JTokenType.Object && item.First.Type != JTokenType.Array)
                    {
                        (obj as JObject).Add(jp.Name, item.First);
                    }
                }
            }
            else if (jobj.Type == JTokenType.Array)//数组
            {
                foreach (var item in list)
                {
                    List<JToken> listToken = item.ToList<JToken>();
                    List<string> listsort = new List<string>();
                    foreach (var im in listToken)
                    {
                        string name = JProperty.Load(im.CreateReader()).Name;
                        listsort.Add(name);
                    }
                    listsort.Sort();
                    List<JToken> listTemp = new List<JToken>();
                    foreach (var im2 in listsort)
                    {
                        listTemp.Add(listToken.Where(p => JProperty.Load(p.CreateReader()).Name == im2).FirstOrDefault());
                    }
                    list = listTemp;

                    listToken = list;
                    // listToken.Sort((p1, p2) => JProperty.Load(p1.CreateReader()).Name.GetAnsi() - JProperty.Load(p2.CreateReader()).Name.GetAnsi());
                    JObject item_obj = new JObject();
                    foreach (var token in listToken)
                    {
                        JProperty jp = JProperty.Load(token.CreateReader());
                        if (token.First.Type == JTokenType.Object)
                        {
                            JObject sub = new JObject();
                            (obj as JObject).Add(jp.Name, sub);
                            SortJson(token.First, sub);
                        }
                        else if (token.First.Type == JTokenType.Array)
                        {
                            JArray arr = new JArray();
                            if (obj.Type == JTokenType.Object)
                            {
                                (obj as JObject).Add(jp.Name, arr);
                            }
                            else if (obj.Type == JTokenType.Array)
                            {
                                (obj as JArray).Add(arr);
                            }
                            SortJson(token.First, arr);
                        }
                        else if (item.First.Type != JTokenType.Object && item.First.Type != JTokenType.Array)
                        {
                            if (obj.Type == JTokenType.Object)
                            {
                                (obj as JObject).Add(jp.Name, token.First);
                            }
                            else if (obj.Type == JTokenType.Array)
                            {
                                item_obj.Add(jp.Name, token.First);
                            }
                        }
                    }
                    if (obj.Type == JTokenType.Array)
                    {
                        (obj as JArray).Add(item_obj);
                    }

                }
            }
            string ret = obj.ToString(Formatting.None);
            return ret;
        }

        #endregion

        #region -----------------PlatformTypeList()取出整个OrderMgms的PlatformTypeList列表
        private List<SelectListItem> PlatformTypeList()
        {
            var orders = db.OrderMgm.Select(m => m.PlatformType).Distinct().ToList();
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


        public class Temp
        {
            public string BarCodesNum { get; set; }
            public string CheckAbnormal { get; set; }
            public string Group { get; set; }
            public bool CheckFinish { get; set; }
            public bool RepetitionCalibration { get; set; }
            public TimeSpan? CheckTime { get; set; }
            public DateTime? CheckBT { get; set; }
            public DateTime? CheckFT { get; set; }
        }
        public ActionResult Total(string OrderNum, string statue)
        {
            JObject result = new JObject();

            var orderBoxBarCodeList = db.BarCodes.Where(m => m.OrderNum == OrderNum).Select(m => m.BarCodesNum).ToList();//订单的所有条码清单

            if (statue == "FQC")
            {
                var FQC = db.FinalQC.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum) && c.RepetitionFQCCheck == false).Select(m => new Temp { BarCodesNum = m.BarCodesNum, CheckAbnormal = m.FinalQC_FQCCheckAbnormal, CheckFinish = m.FQCCheckFinish, Group = m.Group, CheckTime = m.FQCCheckTime, CheckBT = m.FQCCheckBT, CheckFT = m.FQCCheckFT }).OrderBy(x => x.BarCodesNum).ToList();
                //查找是否有挪用直接出库给客户记录
                var newbarcode = db.Warehouse_Join.Where(c => c.NewBarcode == OrderNum && c.CartonOrderNum != OrderNum).Select(c => c.BarCodeNum).ToList();
                if (newbarcode.Count != 0)
                {
                    var temp = db.FinalQC.Where(c => newbarcode.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || newbarcode.Contains(c.OldBarCodesNum))).Select(m => new Temp { BarCodesNum = m.BarCodesNum, CheckAbnormal = m.FinalQC_FQCCheckAbnormal, CheckFinish = m.FQCCheckFinish, Group = m.Group, CheckTime = m.FQCCheckTime, CheckBT = m.FQCCheckBT, CheckFT = m.FQCCheckFT }).ToList();
                    FQC.AddRange(temp);
                }
                var burn_in_Record = db.Burn_in.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).Select(c => c.BarCodesNum).ToList();
                result = TotalJobject(FQC, orderBoxBarCodeList, OrderNum, burn_in_Record, 24);
            }

            if (statue == "老化")
            {
                var Burn_in_Record = db.Burn_in.Where(m => m.OrderNum == OrderNum && (m.OldOrderNum == null || m.OldOrderNum == OrderNum)).Select(m => new Temp { BarCodesNum = m.BarCodesNum, CheckAbnormal = m.Burn_in_OQCCheckAbnormal, CheckFinish = m.OQCCheckFinish, Group = m.Group, CheckTime = m.OQCCheckTime, CheckBT = m.OQCCheckBT, CheckFT = m.OQCCheckFT }).OrderBy(x => x.BarCodesNum).ToList();//订单OQC全部录                        
                //查找是否有挪用直接出库给客户记录
                var newbarcode = db.Warehouse_Join.Where(c => c.NewBarcode == OrderNum && c.CartonOrderNum != OrderNum).Select(c => c.BarCodeNum).ToList();
                if (newbarcode.Count != 0)
                {
                    var temp = db.Burn_in.Where(c => newbarcode.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || newbarcode.Contains(c.OldBarCodesNum))).Select(m => new Temp { BarCodesNum = m.BarCodesNum, CheckAbnormal = m.Burn_in_OQCCheckAbnormal, CheckFinish = m.OQCCheckFinish, Group = m.Group, CheckTime = m.OQCCheckTime, CheckBT = m.OQCCheckBT, CheckFT = m.OQCCheckFT }).ToList();
                    Burn_in_Record.AddRange(temp);
                }
                var appearent = db.Appearance.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).Select(c => c.BarCodesNum).ToList();
                result = TotalJobject(Burn_in_Record, orderBoxBarCodeList, OrderNum, appearent, 72);
            }
            if (statue == "校正")
            {
                var calib = db.CalibrationRecord.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum) && c.RepetitionCalibration == false).Select(m => new Temp { BarCodesNum = m.BarCodesNum, CheckAbnormal = m.AbnormalDescription, CheckFinish = m.Normal, Group = m.Group, CheckTime = m.CalibrationTime, CheckBT = m.BeginCalibration, CheckFT = m.FinishCalibration }).OrderBy(x => x.BarCodesNum).ToList();
                //查找是否有挪用直接出库给客户记录
                var newbarcode = db.Warehouse_Join.Where(c => c.NewBarcode == OrderNum && c.CartonOrderNum != OrderNum).Select(c => c.BarCodeNum).ToList();
                if (newbarcode.Count != 0)
                {
                    var temp = db.CalibrationRecord.Where(c => newbarcode.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || newbarcode.Contains(c.OldBarCodesNum))).Select(m => new Temp { BarCodesNum = m.BarCodesNum, CheckAbnormal = m.AbnormalDescription, CheckFinish = m.Normal, Group = m.Group, CheckTime = m.CalibrationTime, CheckBT = m.BeginCalibration, CheckFT = m.FinishCalibration }).ToList();
                    calib.AddRange(temp);
                }
                result = TotalJobject(calib, orderBoxBarCodeList, OrderNum);
            }
            if (statue == "外观")
            {
                var appearance = db.Appearance.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).Select(m => new Temp { BarCodesNum = m.BarCodesNum, CheckAbnormal = m.Appearance_OQCCheckAbnormal, CheckFinish = m.OQCCheckFinish, Group = m.Group, CheckTime = m.OQCCheckTime, CheckBT = m.OQCCheckBT, CheckFT = m.OQCCheckFT }).OrderBy(x => x.BarCodesNum).ToList();
                //查找是否有挪用直接出库给客户记录
                var newbarcode = db.Warehouse_Join.Where(c => c.NewBarcode == OrderNum && c.CartonOrderNum != OrderNum).Select(c => c.BarCodeNum).ToList();
                if (newbarcode.Count != 0)
                {
                    var temp = db.Appearance.Where(c => newbarcode.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || newbarcode.Contains(c.OldBarCodesNum))).Select(m => new Temp { BarCodesNum = m.BarCodesNum, CheckAbnormal = m.Appearance_OQCCheckAbnormal, CheckFinish = m.OQCCheckFinish, Group = m.Group, CheckTime = m.OQCCheckTime, CheckBT = m.OQCCheckBT, CheckFT = m.OQCCheckFT }).ToList();
                    appearance.AddRange(temp);
                }
                var pack = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == OrderNum).Select(c => c.BarCodeNum).ToList();
                result = TotalJobject(appearance, orderBoxBarCodeList, OrderNum, pack, 72);
            }
            return Content(JsonConvert.SerializeObject(result));
        }


        public JObject TotalJobject(List<Temp> statueInfo, List<String> orderBoxBarCodeList, string OrderNum, List<string> barcodeList = null, int house = 0)
        {
            JObject jsonobj = new JObject();
            jsonobj.Add("OrderNum", OrderNum);//订单号
            var order = (from m in db.OrderMgm where m.OrderNum == OrderNum select m).FirstOrDefault();//取出订单
            jsonobj.Add("PlatformType", order.PlatformType);//平台类型
            jsonobj.Add("DeliveryDate", order.DeliveryDate); //出货日期
            jsonobj.Add("modelGroupQuantity", order.Boxes);//模组数
            #region ---------------------直通率/合格率/完成率----------------------

            var abnormal_BoxBarCode_list = statueInfo.Where(c => c.CheckAbnormal != "正常").Select(c => c.BarCodesNum).ToList();//异常记录
            var firstPassYield_temp = statueInfo.DistinctBy(c => c.BarCodesNum).Where(c => c.CheckAbnormal == "正常" && c.CheckFinish == true).ToList();//Finish记录
            var RecordBarCodeList = statueInfo.Select(m => m.BarCodesNum).Distinct().ToList();//OQC记录全部条码清单(去重)

            List<string> firstPassYield = new List<string>();
            firstPassYield = firstPassYield_temp.Where(c => !abnormal_BoxBarCode_list.Contains(c.BarCodesNum)).Select(c => c.BarCodesNum).ToList();//一次直通记录
            jsonobj.Add("firstPassYield", firstPassYield.Count());//直通数 
            jsonobj.Add("TotalCount", statueInfo.Count());//总次数 

            var finished = statueInfo.Count(m => m.CheckFinish == true);//3.订单已完成OQC个数
            jsonobj.Add("finishedCount", finished);//已完成数 

            var finisthRate = (Convert.ToDouble(finished) / RecordBarCodeList.Count() * 100).ToString("F2");//5.完成率：完成数/订单的模组数
            jsonobj.Add("finisthRate", finisthRate);//完成率 

            var passRate = (Convert.ToDouble(finished) / statueInfo.Count() * 100).ToString("F2");//6.合格率：完成数/记录数
            jsonobj.Add("passRate", passRate);//合格率 

            var firstPassYield_Rate = (Convert.ToDouble(firstPassYield.Count) / order.Boxes * 100).ToString("F2");//7.直通率：直通数/模组数

            jsonobj.Add("firstPassYield_Rate", firstPassYield_Rate);//直通率 
            #endregion

            #region---------------------操作时间--------------------------
            //操作记录时长(Timespan  00:00:05.4780887)
            var time = statueInfo.Select(c => c.CheckTime).ToList();
            TimeSpan? time2 = new TimeSpan();
            time.ForEach(c => time2 = time2 + c);
            if (time2 == null)
            {
                //ViewBag.OperationTime = "0秒";
                //ViewBag.OperationAverageTime = "0秒";
                jsonobj.Add("OperationTime", "0秒");//操作记录时长
                jsonobj.Add("OperationAverageTime", "0秒");//操作记录平均时长
            }
            else
            {
                //ViewBag.OperationTime = comm.changetimespan(time2.Value.TotalMilliseconds);
                //平均操作时长(毫秒)
                var time3 = time2.Value.TotalMilliseconds / statueInfo.Count();
                //ViewBag.OperationAverageTime = comm.changetimespan(time3);
                jsonobj.Add("OperationTime", comm.changetimespan(time2.Value.TotalMilliseconds));//操作记录时长
                jsonobj.Add("OperationAverageTime", comm.changetimespan(time3));//操作记录平均时长
            }
            //异常操作记录时长(Timespan  00:00:05.4780887)
            var value = statueInfo.Where(c => c.CheckFinish == false).Select(c => c.CheckTime).ToList();
            if (value.Count == 0)
            {
                //ViewBag.OperationAbnormalTime = "0秒";
                //ViewBag.OperationAverageAbnormalTime = "0秒";
                jsonobj.Add("OperationAbnormalTime", "0秒");//异常操作记录时长
                jsonobj.Add("OperationAbnormalAverageTime", "0秒");//异常操作记录平均时长
            }
            else
            {
                TimeSpan? time4 = new TimeSpan();
                value.ForEach(c => time4 = c == null ? time4 : time4 + c);
                //ViewBag.OperationAbnormalTime = comm.changetimespan(time4.Value.TotalMilliseconds);
                //异常平均操作时长(毫秒)
                var time5 = time4.Value.TotalMilliseconds / value.Count();
                //ViewBag.OperationAverageAbnormalTime = comm.changetimespan(time5);
                jsonobj.Add("OperationAbnormalTime", comm.changetimespan(time4.Value.TotalMilliseconds));//异常操作记录时长
                jsonobj.Add("OperationAbnormalAverageTime", comm.changetimespan(time5));//异常操作记录平均时长
            }
            #endregion

            #region---------------------正在进行清单----------------
            JArray going = new JArray();
            var going_temp = statueInfo.Where(x => x.CheckBT != null && x.CheckFT == null).ToList();//15.正在进行OQC的条码清单、个数
            var groupList1 = going_temp.Select(c => c.Group).Distinct().ToList();
            groupList1.ForEach(c =>
            {
                JObject item = new JObject(); ;
                item.Add("group", c);
                JArray array = new JArray();
                var info = going_temp.Where(x => x.Group == c).Select(x => x.BarCodesNum).ToList();
                info.ForEach(a => array.Add(a));
                item.Add("barcodeList", array);
                item.Add("itemcount", info.Count());
                going.Add(item);
            });
            jsonobj.Add("going", going);//正在进行清单

            #endregion

            #region ---------------------未开始清单-------------------
            var finishedList = statueInfo.Where(m => m.CheckFinish == true).Select(m => m.BarCodesNum).ToList(); //订单已完成OQC的条码清单
            var unbeginRecord_temp = orderBoxBarCodeList.ToArray().Except(finishedList.ToArray()).ToList().Except(going_temp.Select(c => c.BarCodesNum).ToArray()).ToList();//14.未开始条码清单、个数(排除已完成（包含正常异常）、正在进行)
            jsonobj.Add("unbeginRecord_temp", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(unbeginRecord_temp)));//未开始清单
            jsonobj.Add("unbeginRecord_tempCount", unbeginRecord_temp.Count());//未开始清单

            #endregion

            #region --------------------------已完成清单------------------
            JArray finasharray = new JArray();
            var passed_temp = statueInfo.Where(x => x.CheckFinish == true).ToList();//16.已经完成OQC的条码清单、个数
            var groupList2 = passed_temp.Select(c => c.Group).Distinct().ToList();
            groupList2.ForEach(c =>
            {
                JObject item = new JObject(); ;
                item.Add("group", c);
                JArray array = new JArray();
                var info = passed_temp.Where(x => x.Group == c).Select(x => x.BarCodesNum).ToList();
                info.ForEach(a => array.Add(a));
                item.Add("barcodeList", array);
                item.Add("itemcount", info.Count());
                finasharray.Add(item);
            });
            jsonobj.Add("passed_temp", finasharray);//已完成老化清单
            #endregion



            #region ---------------上一个工序完成后超过house小时未开始下一工序的条码清单-----------------

            //appearance_list 包装全部记录
            if (barcodeList != null)
            {
                //直接挪用出库给客户订单的,不算在超过HOUSE未开始清单列表
                var newbarcode = db.Warehouse_Join.Where(c => c.NewBarcode == OrderNum && c.CartonOrderNum != OrderNum).Select(c => c.BarCodeNum).ToList();
                //finish_burn_in_list 已完成清单,并超过house小时的
                var finish_burn_in_list = statueInfo.Where(c => c.CheckFT != null && c.CheckFinish == true && (DateTime.Now - c.CheckFT).Value.TotalHours >= house && !barcodeList.Contains(c.BarCodesNum)&&!newbarcode.Contains(c.BarCodesNum)).OrderBy(c => c.BarCodesNum).ToList();
                JArray seventGroup = new JArray();
                var group = finish_burn_in_list.Select(c => c.Group).Distinct().ToList();//分组列表
                foreach (var seventgroup in group)
                {
                    JObject item = new JObject(); ;
                    item.Add("group", seventgroup);
                    JArray array = new JArray();
                    var info = finish_burn_in_list.Where(c => c.Group == seventgroup).Select(c => c.BarCodesNum).ToList();
                    info.ForEach(c => array.Add(c));
                    item.Add("barcodeList", array);
                    item.Add("itemcount", info.Count());
                    seventGroup.Add(item);
                }
                //ViewBag.Overtime_never_join_appearance = Overtime_never_join_appearance_list;
                jsonobj.Add("Overtime_never_join_appearance", seventGroup);
            }
            #endregion

            #region -----------------------------异常记录清单------------------------
            JArray abnormalarray = new JArray();
            var abnormalList_temp1 = (from m in statueInfo where m.CheckAbnormal != "正常" orderby m.BarCodesNum select m).ToList();//10.异常记录清单(包含正在OQC)

            var going_temp_normal = going_temp.Where(m => m.CheckAbnormal == null).ToList();
            var abnormalList_temp = abnormalList_temp1.Except(going_temp_normal).ToList();
            var groupList3 = abnormalList_temp.Select(c => c.Group).Distinct().ToList();
            groupList3.ForEach(c =>
            {
                JObject item = new JObject(); ;
                item.Add("group", c);
                JArray array = new JArray();
                var info = abnormalList_temp.Where(x => x.Group == c).Select(x => new { x.BarCodesNum, x.CheckAbnormal }).ToList();
                info.ForEach(a =>
                {
                    JObject itemitem = new JObject();
                    itemitem.Add("barcode", a.BarCodesNum);
                    itemitem.Add("abnormal", a.CheckAbnormal);
                    array.Add(itemitem);
                });
                item.Add("barcodeList", array);
                item.Add("itemcount", info.Count());
                abnormalarray.Add(item);
            });
            jsonobj.Add("abnormalList_temp", abnormalarray);//异常清单

            #endregion

            return jsonobj;
        }


        #region -----------------组装PQC详情页面

        [HttpPost]
        public ActionResult Assemble(string OrderNum)
        {
            #region ---------------读取数据，处理数据

            ViewBag.OrderNum = OrderNum;//1.订单号

            var order = (from m in db.OrderMgm where m.OrderNum == OrderNum select m).FirstOrDefault();//取出订单
            ViewBag.PlatformType = order.PlatformType; //平台类型
            ViewBag.DeliveryDate = order.DeliveryDate; //出货日期

            var modelGroupQuantity = (from m in db.OrderMgm where m.OrderNum == OrderNum select m).ToList().FirstOrDefault().Boxes;//2.订单模组数
            var orderBoxBarCodeList = db.BarCodes.Where(m => m.OrderNum == OrderNum).Select(m => m.BarCodesNum).ToList();//订单的所有条码清单
            var assembleRecord = (from m in db.Assemble where m.OrderNum == OrderNum && (m.OldOrderNum == null || m.OldOrderNum == OrderNum) select m).OrderBy(x => x.BoxBarCode).ToList();//订单PQC全部记录
            var assembleRecordBarCodeList = assembleRecord.Select(m => m.BoxBarCode).Distinct().ToList();//PQC记录全部条码清单(去重)
            //操作记录时长(Timespan  00:00:05.4780887)
            var time = assembleRecord.Select(c => c.PQCCheckTime).ToList();
            TimeSpan? time2 = new TimeSpan();
            time.ForEach(c => time2= time2 + c);
            ViewBag.OperationTime = time2;
            //平均操作时长(毫秒)
            var time3 = time2.Value.TotalMilliseconds / assembleRecord.Count();
            ViewBag.OperationAverageTime =comm.changetimespan(time3);
            //异常操作记录时长(Timespan  00:00:05.4780887)
            var value = assembleRecord.Where(c=>c.PQCCheckFinish==false).Select(c => c.PQCCheckTime).ToList();
            TimeSpan? time4 = new TimeSpan();
            value.ForEach(c => time4 = c == null ? time4 : time4 + c);
            ViewBag.OperationAbnormalTime = time4;
            //异常平均操作时长(毫秒)
            var time5 = time4.Value.TotalMilliseconds / value.Count();
            ViewBag.OperationAverageAbnormalTime =  comm.changetimespan(time5);

            var finished = assembleRecord.Count(m => m.PQCCheckFinish == true);//3.订单已完成PQC个数
            var finishedList = assembleRecord.Where(m => m.PQCCheckAbnormal == "正常").Select(m => m.BoxBarCode).ToList(); //订单已完成PQC的条码清单

            var assemblePQC_Count = assembleRecord.Count();//4.订单PQC全部记录条数

            var finisthRate = (Convert.ToDouble(finished) / modelGroupQuantity * 100).ToString("F2");//5.完成率：完成数/订单的模组数

            var passRate = (Convert.ToDouble(finished) / assemblePQC_Count * 100).ToString("F2");//6.合格率：完成数/记录数

            #region ---------------------一次直通记录、个数、直通率----------------------
            //---------------------一次直通记录----------------------
            var assembleRecord_abnormal_BoxBarCode_list = assembleRecord.Where(c => c.PQCRepairCondition != "正常" || c.PQCCheckAbnormal != "正常").Select(c => c.BoxBarCode).ToList();//异常记录
            var firstPassYield_temp = assembleRecord.DistinctBy(c => c.BoxBarCode).Where(c => c.PQCCheckAbnormal == "正常" && c.PQCRepairCondition == "正常" && c.PQCCheckFinish == true).ToList();//Finish记录
            List<Assemble> firstPassYield_expect = new List<Assemble>();//有异常记录的条码Finish记录
            foreach (var item in assembleRecord_abnormal_BoxBarCode_list)
            {
                foreach (var assemblerecord in firstPassYield_temp)
                {
                    if (assemblerecord.BoxBarCode == item)
                    {
                        firstPassYield_expect.Add(assemblerecord);
                    }
                }
            }
            List<Assemble> firstPassYield = new List<Assemble>();
            firstPassYield = firstPassYield_temp.Except(firstPassYield_expect).ToList();//一次直通记录
            var firstPassYieldCount = firstPassYield.Count();//8.一次直通数
            var firstPassYield_Rate = (Convert.ToDouble(firstPassYieldCount) / modelGroupQuantity * 100).ToString("F2");//7.直通率：直通数/模组数
            #endregion


            var going_temp = assembleRecord.Where(x => x.PQCCheckBT != null && x.PQCCheckFT == null).ToList();//15.正在进行PQC的条码清单、个数
            var going = Assemble_PutOutJson(going_temp);

            var abnormalList_temp1 = (from m in assembleRecord where m.PQCRepairCondition != "正常" orderby m.BoxBarCode select m).ToList();//10.异常记录清单(包含正在PQC)

            var abnormalList_temp = abnormalList_temp1.Except(going_temp).ToList();

            var abnormalList = Assemble_PutOutJson(abnormalList_temp);

            #region ----------11.异常工时----------
            //----------11.异常工时----------
            int days = 0, hours = 0, minutes = 0, seconds = 0;
            foreach (var item in abnormalList_temp)
            {
                if (item.PQCCheckTime != null)
                {
                    days = days + item.PQCCheckTime.Value.Days;
                    hours = hours + item.PQCCheckTime.Value.Hours;
                    minutes = minutes + item.PQCCheckTime.Value.Minutes;
                    seconds = seconds + item.PQCCheckTime.Value.Seconds;
                }
            }
            TimeSpan abnormal_time = new TimeSpan(days, hours, minutes, seconds); //11.异常工时
            #endregion

            var abnormal_Count = (from m in assembleRecord where m.PQCCheckAbnormal != "正常" select m).Count();//9.异常次数


            #region ---------------12.经过2次及以上PQC已完成的条码清单-----------------
            List<Assemble> finishedAnd2record_temp = new List<Assemble>();//12.经过2次及以上PQC已完成的条码清单、个数
            foreach (var item in assembleRecordBarCodeList)
            {
                if (assembleRecord.Where(c => c.BoxBarCode == item).ToList().Count() > 1)
                {
                    var i = assembleRecord.Where(c => c.BoxBarCode == item).ToList().Count(c => c.PQCCheckFinish == true);
                    if (i == 1)
                    {
                        finishedAnd2record_temp.AddRange(assembleRecord.Where(c => c.BoxBarCode == item).ToList());
                    }
                }
            }
            var finishedAnd2record = Assemble_PutOutJson(finishedAnd2record_temp);
            #endregion


            #region ---------------13.经过1次以上PQC未通过的条码清单-----------------
            List<Assemble> unfinishAndRecord_temp = new List<Assemble>();//13.经过1次以上PQC未通过的条码清单、个数
            foreach (var item in assembleRecordBarCodeList)
            {
                List<Assemble> temp = assembleRecord.Where(c => c.BoxBarCode == item).ToList();
                if (temp.Count() >= 1 && temp.Count(c => c.PQCCheckFinish == true) == 0)
                {
                    unfinishAndRecord_temp.AddRange(temp);
                }
            }
            unfinishAndRecord_temp = unfinishAndRecord_temp.Except(going_temp).ToList();
            var unfinishAndRecord = Assemble_PutOutJson(unfinishAndRecord_temp);
            #endregion


            #region ---------------PQC完成后超过24小时未开始老化工序的条码清单-----------------
            //finish_assemble_list 已完成组装清单
            var finish_assemble_list = assembleRecord.Where(c => c.PQCCheckFT != null && c.PQCCheckFinish == true && c.RepetitionPQCCheck == false).ToList();
            //burn_in_Record 老化全部记录
            var burn_in_Record = db.Burn_in.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).ToList();
            List<Assemble> Overtime_never_join_burn_in_list = new List<Assemble>();
            foreach (var record in finish_assemble_list)
            {
                if (burn_in_Record.Count(c => c.BarCodesNum == record.BoxBarCode) == 0 && (DateTime.Now - record.PQCCheckFT).Value.TotalHours >= 24)
                {
                    Overtime_never_join_burn_in_list.Add(record);
                }
            }
            ViewBag.Overtime_never_join_burn_in = Overtime_never_join_burn_in_list;
            #endregion


            var unbeginRecord_temp = orderBoxBarCodeList.ToArray().Except(finishedList.ToArray()).ToList().Except(going_temp.Select(c => c.BoxBarCode).ToArray()).ToList();//14.未开始PQC的条码清单、个数(排除已完成（包含正常异常）、正在进行)

            string unbeginRecord = null;
            foreach (var item in unbeginRecord_temp)
            {
                if (unbeginRecord == null)
                {
                    unbeginRecord = "[\"" + item;
                }
                else
                {
                    unbeginRecord = unbeginRecord + "\",\"" + item;
                }
                if (unbeginRecord_temp.IndexOf(item) == unbeginRecord_temp.Count() - 1)
                {
                    unbeginRecord = unbeginRecord + "\"]";
                }
            }

            var passed_temp = assembleRecord.Where(x => x.PQCCheckFinish == true).ToList();//16.已经完成PQC的条码清单、个数
            var passed = Assemble_PutOutJson(passed_temp);
            //string abnormalStatistics = null; //17.异常信息统计

            #endregion

            #region ---------------将对象转为列矩阵JSON
            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            //创建JSON对象
            JObject JsonObj = new JObject
            {
                { "OrderNum", OrderNum },//1.订单号
                { "modelGroupQuantity", modelGroupQuantity },//2.订单模组数
                { "finished", finished},//3.订单已完成PQC个数
                { "assemblePQC_Count", assemblePQC_Count},//4.订单PQC全部记录条数
                { "finisthRate", finisthRate },//5.完成率：完成数/订单的模组数
                { "passRate", passRate },//6.合格率：完成数/记录数
                { "firstPassYield_Rate", firstPassYield_Rate },//7.直通率：直通数/模组数
                { "firstPassYieldCount", firstPassYieldCount },//8.直通数
                { "abnormal_Count", abnormal_Count },//9.异常次数
                { "abnormalListNum", abnormalList_temp.Count() },//10.异常记录清单
                { "abnormal_time",abnormal_time },//11.异常工时
                { "finishedAnd2recordNum", finishedAnd2record_temp.Count() },//12.经过2次及以上PQC已完成的条码清单、个数
                { "unfinishAndRecordNum", unfinishAndRecord_temp.Count() },//13.经过1次以上PQC未通过的条码清单、个数
                { "unbeginRecordNum", unbeginRecord_temp.Count() },//14.未开始PQC的条码清单、个数
                { "goingNum", going_temp.Count() },//15.正在进行PQC的条码清单、个数
                { "passedNum", passed_temp.Count() },//16.已经完成PQC的条码清单、个数
                { "Overtime_never_join_burn_in", Overtime_never_join_burn_in_list.Count() },//PQC完成后超过24小时未开始老化工序的条码清单个数
                //17.异常信息统计
            };
            #endregion

            ViewBag.abnormalList_temp = abnormalList_temp;//10.异常记录清单
            ViewBag.finishedAnd2record_temp = finishedAnd2record_temp;//12.经过2次及以上PQC已完成的条码清单、个数
            ViewBag.unfinishAndRecord_temp = unfinishAndRecord_temp;//13.经过1次以上PQC未通过的条码清单、个数
            ViewBag.unbeginRecord_temp = unbeginRecord_temp;//14.未开始PQC的条码清单、个数
            ViewBag.going_temp = going_temp;//15.正在进行PQC的条码清单、个数
            ViewBag.passed_temp = passed_temp;//16.已经完成PQC的条码清单、个数
            ViewBag.JsonObj = JsonObj;

            return View();
        }

        public JObject Assemble_PutOutJson(List<Assemble> inputlist)  //Assemble记录转Json方法
        {
            JObject OutPutJson = new JObject();
            OutPutJson.Add("title", "[Id,OrderNum,BarCode_Prefix,BoxBarCode,AssembleBT,AssemblePrincipal,AssembleFT," +
                "ModelList,AssembleTime,AssembleFinish,WaterproofTestBT,WaterproofTestPrincipal,WaterproofTestFT," +
                "WaterproofTestTimeSpan,WaterproofAbnormal,WaterproofMaintaince,WaterproofTestFinish,AssembleAdapterCardBT," +
                "AssembleAdapterCardPrincipal,AssembleAdapterCardFT,AssembleAdapterTime,AssembleAdapterFinish," +
                "AdapterCard_Power_Collection,ViewCheckBT,AssembleViewCheckPrincipal,ViewCheckFT,ViewCheckTime," +
                "ViewCheckAbnormal,ViewCheckFinish,ElectricityCheckBT,AssembleElectricityCheckPrincipal,ElectricityCheckFT," +
                "ElectricityCheckTime,ElectricityCheckAbnormal,ElectricityCheckFinish,AssembleLineId," +
                "AdapterCard_Power_List,PQCCheckBT,AssemblePQCPrincipal,PQCCheckFT,PQCCheckTime,PQCCheckAbnormal," +
                "PQCRepairCondition,PQCCheckFinish]");
            foreach (var item in inputlist)
            {
                OutPutJson.Add((inputlist.IndexOf(item) + 1).ToString(), "[" + item.Id + "," + item.OrderNum + "," + item.BarCode_Prefix + "," +
                    item.BoxBarCode + "," + item.AssembleBT + "," + item.AssemblePrincipal + "," + item.AssembleFT + "," + item.ModelList + "," +
                    item.AssembleTime + "," + item.AssembleFinish + "," + item.WaterproofTestBT + "," + item.WaterproofTestPrincipal + "," +
                    item.WaterproofTestFT + "," + item.WaterproofTestTimeSpan + "," + item.WaterproofAbnormal + "," + item.WaterproofMaintaince + "," +
                    item.WaterproofTestFinish + "," + item.AssembleAdapterCardBT + "," + item.AssembleAdapterCardPrincipal + "," +
                    item.AssembleAdapterCardFT + "," + item.AssembleAdapterTime + "," + item.AssembleAdapterFinish + "," +
                    item.AdapterCard_Power_Collection + "," + item.ViewCheckBT + "," + item.AssembleViewCheckPrincipal + "," +
                    item.ViewCheckFT + item.ViewCheckTime + "," + item.ViewCheckAbnormal + "," + item.ViewCheckFinish + "," +
                    item.ElectricityCheckBT + "," + item.AssembleElectricityCheckPrincipal + "," + item.ElectricityCheckFT + "," +
                    item.ElectricityCheckTime + "," + item.ElectricityCheckAbnormal + "," + item.ElectricityCheckFinish + "," +
                    item.AssembleLineId + "," + item.AdapterCard_Power_List + "," + item.PQCCheckBT + "," + item.AssemblePQCPrincipal + "," +
                    item.PQCCheckFT + "," + item.PQCCheckTime + "," + item.PQCCheckAbnormal + "," + item.PQCRepairCondition + "," + item.PQCCheckFinish + "]");
            }
            return OutPutJson;
        }

      
        #endregion


        public List<string> FindOld(string baroceod)
        {
            List<string> itemjarray = new List<string>();
            var haveold = db.BarCodeRelation.Where(c => c.NewBarCodesNum == baroceod).ToList();
            if (haveold.Count != 0)
            {
                //var oldbarcode = haveold.Select(c => c.OldBarCodeNum).ToList();
                foreach (var item in haveold)
                {
                    itemjarray = FindOld(item.OldBarCodeNum);
                    itemjarray.Add(item.OldBarCodeNum);
                }
            }
            return itemjarray;
        }

        #region-------------------FQC详情页面
        [HttpPost]
        public ActionResult FinalQC(string OrderNum)
        {

            JObject JsonObj = new JObject();

            var ordernum_info = db.OrderMgm.Where(c => c.OrderNum == OrderNum).FirstOrDefault();
            //对应订单记录集合
            var FinalQC_Record_List = db.FinalQC.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).ToList();
            //查找是否有挪用直接出库给客户记录
            var newbarcode = db.Warehouse_Join.Where(c => c.NewBarcode == OrderNum && c.CartonOrderNum != OrderNum).Select(c => c.BarCodeNum).ToList();
            if (newbarcode.Count != 0)
            {
                var temp = db.FinalQC.Where(c => newbarcode.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || newbarcode.Contains(c.OldBarCodesNum))).ToList();
                FinalQC_Record_List.AddRange(temp);
            }
            //异常记录集合
            var FQC_Abnormal_record = FinalQC_Record_List.Where(c => c.FinalQC_FQCCheckAbnormal != "正常").OrderBy(c => c.BarCodesNum).ToList();
            //直通记录个数
            List<FinalQC> FirstPassYield_record_list = new List<FinalQC>();
            var barcodesnumlist = FinalQC_Record_List.Select(c => c.BarCodesNum).Distinct().ToList();
            foreach (var item in barcodesnumlist)
            {
                if (FinalQC_Record_List.Where(c => c.BarCodesNum == item).Count() == 1)
                {
                    var record = FinalQC_Record_List.Where(c => c.BarCodesNum == item).FirstOrDefault();
                    if (record.FQCCheckFinish == true && record.RepetitionFQCCheck == false)
                    {
                        FirstPassYield_record_list.Add(record);
                    }
                }
            }
            //异常工时

            //抽检完成率、合格率
            Decimal FinalQC_Finish = FinalQC_Record_List.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Count();
            if (FinalQC_Record_List.Count > 0)
            {
                //JsonObj.Add("FinalQC_Finish", Convert.ToInt32(FinalQC_Finish)); //FQC完成个数
                //JsonObj.Add("FinalQC_Record_Count", FinalQC_Record_List.Count);      //FQC记录个数
                //JsonObj.Add("FinalQC_Spot_Count", FinalQC_Record_List.Select(c => c.BarCodesNum).Distinct().Count());      //FQC抽检个数 
                if (FinalQC_Finish == 0)
                {
                    JsonObj.Add("FinalQC_Finish_Rate", "0%"); //10.FQC完成率
                    JsonObj.Add("FinalQC_Pass_Rate", "0%");   //11.FQC合格率
                    JsonObj.Add("FirstPassYield_Rate", "0%"); //12.直通率
                }
                else
                {
                    JsonObj.Add("FinalQC_Finish_Rate", (FinalQC_Finish / FinalQC_Record_List.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2") + "%");  //10.FQC完成率
                    JsonObj.Add("FinalQC_Pass_Rate", (FinalQC_Finish / FinalQC_Record_List.Count() * 100).ToString("F2") + "%");                                          //11.FQC合格率
                    JsonObj.Add("FirstPassYield_Rate", ((Decimal)FirstPassYield_record_list.Count / FinalQC_Finish * 100).ToString("F2") + "%");                             //12.直通率

                }
            }
            else
            {
                JsonObj.Add("FinalQC_Finish_Rate", "--%");  //10.FQC完成率
                JsonObj.Add("FinalQC_Pass_Rate", "--%");    //11.FQC合格率
                JsonObj.Add("FirstPassYield_Rate", "--%");  //12.直通率

            }
            //正在进行FQC条码清单
            var going_list = FinalQC_Record_List.Where(c => c.FQCCheckBT != null && c.FQCCheckFT == null).ToList();
            JsonObj.Add("going_list_Count", going_list.Count);
            //已经完成FQC条码清单
            var passed_list = FinalQC_Record_List.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).OrderBy(c => c.BarCodesNum).ToList();
            JsonObj.Add("passed_list_Count", going_list.Count);
            //异常记录清单
            JsonObj.Add("abnormalList_Count", FQC_Abnormal_record.Count);


            JsonObj.Add("OrderNum", OrderNum);   //1.订单号
            JsonObj.Add("PlatformType", ordernum_info.PlatformType);    //2.平台类型
            JsonObj.Add("DeliveryDate", ordernum_info.DeliveryDate);   //3.出货日期
            JsonObj.Add("Boxes", ordernum_info.Boxes);   //4.模组数
            JsonObj.Add("FQC_Abnormal_Count", FQC_Abnormal_record.Count);   //5.异常记录次数
            JsonObj.Add("FQC_Finish_Count", FinalQC_Record_List.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Count());   //6.已完成FQC次数
            JsonObj.Add("FQC_Record_Count", FinalQC_Record_List.Count);  //7.总FQC次数
            JsonObj.Add("FirstPassYieldCount", FirstPassYield_record_list.Count);      //8.直通记录个数 

            //操作记录时长(Timespan  00:00:05.4780887)
            var time = FinalQC_Record_List.Select(c => c.FQCCheckTime).ToList();
            TimeSpan? time2 = new TimeSpan();
            time.ForEach(c => time2 = time2 + c);
            if (time2 == null)
            {
                JsonObj.Add("OperationTime", "0秒");
                JsonObj.Add("OperationAverageTime", "0秒");
            }
            else
            {
                JsonObj.Add("OperationTime", comm.changetimespan(time2.Value.TotalMilliseconds));
                //平均操作时长(毫秒)
                var time3 = time2.Value.TotalMilliseconds / FinalQC_Record_List.Count();
                JsonObj.Add("OperationAverageTime", comm.changetimespan(time3));
            }
            //异常操作记录时长(Timespan  00:00:05.4780887)
            var value = FinalQC_Record_List.Where(c => c.FQCCheckFinish == false).Select(c => c.FQCCheckTime).ToList();
            if (value.Count == 0)
            {
                JsonObj.Add("OperationAbnormalTime", "0秒");
                JsonObj.Add("OperationAverageAbnormalTime","0秒");
            }
            else
            {
                TimeSpan? time4 = new TimeSpan();
                value.ForEach(c => time4 = c == null ? time4 : time4 + c);
                JsonObj.Add("OperationAbnormalTime", comm.changetimespan(time4.Value.TotalMilliseconds));
                //异常平均操作时长(毫秒)
                var time5 = time4.Value.TotalMilliseconds / value.Count();
                JsonObj.Add("OperationAverageAbnormalTime", comm.changetimespan(time5));
            }

             ViewBag.going_list = going_list;  //正在进行FQC条码清单
            ViewBag.passed_list = passed_list;   //已经完成FQC条码清单
            ViewBag.abnormalList = FQC_Abnormal_record;    //异常记录清单
                                                           //JsonObj.Add("going_list",JsonConvert.SerializeObject(going_list));  //正在进行FQC条码清单
                                                           //JsonObj.Add("passed_list", JsonConvert.SerializeObject(passed_list));  //已经完成FQC条码清单
                                                           //JsonObj.Add("abnormalList",JsonConvert.SerializeObject(FQC_Abnormal_record));  //异常记录清单
                                                           //JsonObj.Add("Abnormal_time",);  //9.异常工时

            #region ---------------FQC完成后超过24小时未开始老化工序的条码清单-----------------
            //finish_FQC_list 已完成FQC清单
            var finish_FQC_list = FinalQC_Record_List.Where(c => c.FQCCheckFT != null && c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).ToList();
            //burn_in_Record 老化全部记录
            var burn_in_Record = db.Burn_in.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).ToList();
            List<FinalQC> Overtime_never_join_burn_in_list = new List<FinalQC>();
            foreach (var record in finish_FQC_list)
            {
                if (burn_in_Record.Count(c => c.BarCodesNum == record.BarCodesNum) == 0 && (DateTime.Now - record.FQCCheckFT).Value.TotalHours >= 24)
                {
                    Overtime_never_join_burn_in_list.Add(record);
                }
            }
            JsonObj.Add("Overtime_never_join_burn_in_Count", Overtime_never_join_burn_in_list.Count);
            ViewBag.Overtime_never_join_burn_in = Overtime_never_join_burn_in_list;
            #endregion

            #region ---------------未完成FQC工序的条码清单-----------------
            var barcodelist = db.BarCodes.Where(c => c.OrderNum == OrderNum).OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();
            var FinishFQC_List = FinalQC_Record_List.Where(c => c.FQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
            var Never_FinishFQC_List = barcodelist.Except(FinishFQC_List).OrderBy(c => c).ToList();
            JsonObj.Add("Never_FinishFQC_List_Count", Never_FinishFQC_List.Count);
            ViewBag.unbeginRecord_temp = Never_FinishFQC_List;
            #endregion

            ViewBag.JsonObj = JsonObj;
            return View();
        }

        public ActionResult FinalNuo(string OrderNum)
        {
            var barcodelist = db.BarCodeRelation.Where(c => c.NewOrderNum == OrderNum).ToList();
            JArray abnormal = new JArray();
            JArray jarraynomal = new JArray();
            JObject total = new JObject();
            foreach (var item in barcodelist)
            {
                JObject nuojobject = new JObject();
                List<string> oldbarcode = FindOld(item.NewBarCodesNum);
                JArray barcodeStatu = new JArray();
                foreach (var displaybarcode in oldbarcode)
                {
                    JObject barcodeitem = new JObject();
                    barcodeitem.Add("barcode", displaybarcode);
                    var statu = db.FinalQC.Where(c =>( c.OldBarCodesNum == displaybarcode||c.OldBarCodesNum==null) && c.OrderNum == OrderNum && c.BarCodesNum == item.NewBarCodesNum).ToList();
                    if (statu.Count != 0)
                    {
                        var abnormalist = statu.Where(c => c.FQCCheckBT != null && c.FQCCheckFT != null && c.FQCCheckFinish == false && c.FinalQC_FQCCheckAbnormal != null).Select(c => c.FinalQC_FQCCheckAbnormal).ToArray();
                        if (abnormalist.Count() != 0)
                        {
                            JObject abnarmalitem = new JObject();
                            abnarmalitem.Add("barcode", displaybarcode);
                            abnarmalitem.Add("abnormal", string.Join(",", abnormalist));
                            abnormal.Add(abnarmalitem);
                        }
                        if (statu.Count(c => c.FQCCheckBT != null && c.FQCCheckFinish == true) != 0)
                        {
                            var begintime = statu.OrderByDescending(c => c.FQCCheckFT).Where(c => c.FQCCheckBT != null && c.FQCCheckFinish == true).Select(c => c.FQCCheckFT).FirstOrDefault();
                            var burnin = db.Burn_in.OrderBy(c => c.OQCCheckBT).Where(c => (c.OldBarCodesNum == displaybarcode || c.OldBarCodesNum == null) && c.OrderNum == OrderNum && c.BarCodesNum == item.NewBarCodesNum).Select(c => c.OQCCheckBT).FirstOrDefault();
                            if (burnin == null && (DateTime.Now - begintime).Value.TotalHours >= 24)
                            {
                                barcodeitem.Add("statue", "已超过24小时未进入老化");
                            }
                            else
                            {
                                barcodeitem.Add("statue", "已完成");
                            }
                        }
                        else
                        {
                            barcodeitem.Add("statue", "进行中");
                        }
                    }
                    else
                    {
                        barcodeitem.Add("statue", "未开始");
                    }
                    barcodeStatu.Add(barcodeitem);
                }
                oldbarcode.Add(item.NewBarCodesNum);
                JArray infoArray = new JArray();
                oldbarcode.ForEach(c => infoArray.Add(c));
                nuojobject.Add("info", infoArray);
                nuojobject.Add("barcodeInfo", barcodeStatu);
                jarraynomal.Add(nuojobject);
            }
            total.Add("normal", jarraynomal);
            total.Add("abnormal", abnormal);
            return Content(JsonConvert.SerializeObject(total));
        }

        #endregion




        #region -----------------调试老化OQC详情页面
        [HttpPost]
        public ActionResult Burn_in(string OrderNum)
        {

            #region ---------------读取数据，处理数据

            ViewBag.OrderNum = OrderNum;//1.订单号

            var order = (from m in db.OrderMgm where m.OrderNum == OrderNum select m).FirstOrDefault();//取出订单
            ViewBag.PlatformType = order.PlatformType; //平台类型
            ViewBag.DeliveryDate = order.DeliveryDate; //出货日期

            var modelGroupQuantity = (from m in db.OrderMgm where m.OrderNum == OrderNum select m).ToList().FirstOrDefault().Boxes;//2.订单模组数
            var orderBoxBarCodeList = db.BarCodes.Where(m => m.OrderNum == OrderNum).Select(m => m.BarCodesNum).ToList();//订单的所有条码清单
            var Burn_in_Record = (from m in db.Burn_in where m.OrderNum == OrderNum && (m.OldOrderNum == null || m.OldOrderNum == OrderNum) select m).OrderBy(x => x.BarCodesNum).ToList();//订单OQC全部记录
            //查找是否有挪用直接出库给客户记录
            var newbarcode = db.Warehouse_Join.Where(c => c.NewBarcode == OrderNum && c.CartonOrderNum != OrderNum).Select(c => c.BarCodeNum).ToList();
            if (newbarcode.Count != 0)
            {
                var temp = db.Burn_in.Where(c => newbarcode.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || newbarcode.Contains(c.OldBarCodesNum))).ToList();
                Burn_in_Record.AddRange(temp);
            }

            var Burn_in_RecordBarCodeList = Burn_in_Record.Select(m => m.BarCodesNum).Distinct().ToList();//OQC记录全部条码清单(去重)

            //操作记录时长(Timespan  00:00:05.4780887)
            var time = Burn_in_Record.Select(c => c.OQCCheckTime).ToList();
            TimeSpan? time2 = new TimeSpan();
            time.ForEach(c => time2 = time2 + c);
            if (time2 == null)
            {
                ViewBag.OperationTime = "0秒";
                ViewBag.OperationAverageTime = "0秒";
            }
            else
            {
                ViewBag.OperationTime = comm.changetimespan(time2.Value.TotalMilliseconds);
                //平均操作时长(毫秒)
                var time3 = time2.Value.TotalMilliseconds / Burn_in_Record.Count();
                ViewBag.OperationAverageTime = comm.changetimespan(time3);
            }
            //异常操作记录时长(Timespan  00:00:05.4780887)
            var value = Burn_in_Record.Where(c => c.OQCCheckFinish == false).Select(c => c.OQCCheckTime).ToList();
            if (value.Count == 0)
            {
                ViewBag.OperationAbnormalTime = "0秒";
                ViewBag.OperationAverageAbnormalTime = "0秒";
            }
            else
            {
                TimeSpan? time4 = new TimeSpan();
                value.ForEach(c => time4 = c == null ? time4 : time4 + c);
                ViewBag.OperationAbnormalTime = comm.changetimespan(time4.Value.TotalMilliseconds);
                //异常平均操作时长(毫秒)
                var time5 = time4.Value.TotalMilliseconds / value.Count();
                ViewBag.OperationAverageAbnormalTime = comm.changetimespan(time5);
            }
            var finished = Burn_in_Record.Count(m => m.OQCCheckFinish == true);//3.订单已完成OQC个数
            var finishedList = Burn_in_Record.Where(m => m.OQCCheckFinish == true).Select(m => m.BarCodesNum).ToList(); //订单已完成OQC的条码清单

            var Burn_in_OQC_Count = Burn_in_Record.Count();//4.订单OQC全部记录条数

            var finisthRate = (Convert.ToDouble(finished) / Burn_in_RecordBarCodeList.Count() * 100).ToString("F2");//5.完成率：完成数/订单的模组数

            var passRate = (Convert.ToDouble(finished) / Burn_in_OQC_Count * 100).ToString("F2");//6.合格率：完成数/记录数

            #region ---------------------一次直通记录、个数、直通率----------------------
            //---------------------一次直通记录----------------------
            var Burn_in_Record_abnormal_BoxBarCode_list = Burn_in_Record.Where(c => c.RepairCondition != "正常" || c.Burn_in_OQCCheckAbnormal != "正常").Select(c => c.BarCodesNum).ToList();//异常记录
            var firstPassYield_temp = Burn_in_Record.DistinctBy(c => c.BarCodesNum).Where(c => c.Burn_in_OQCCheckAbnormal == "正常" && c.RepairCondition == "正常" && c.OQCCheckFinish == true).ToList();//Finish记录
            List<Burn_in> firstPassYield_expect = new List<Burn_in>();//有异常记录的条码Finish记录
            foreach (var item in Burn_in_Record_abnormal_BoxBarCode_list)
            {
                foreach (var burn_in_erecord in firstPassYield_temp)
                {
                    if (burn_in_erecord.BarCodesNum == item)
                    {
                        firstPassYield_expect.Add(burn_in_erecord);
                    }
                }
            }
            List<Burn_in> firstPassYield = new List<Burn_in>();
            firstPassYield = firstPassYield_temp.Except(firstPassYield_expect).ToList();//一次直通记录
            var firstPassYieldCount = firstPassYield.Count();//8.一次直通数
            var firstPassYield_Rate = (Convert.ToDouble(firstPassYieldCount) / modelGroupQuantity * 100).ToString("F2");//7.直通率：直通数/模组数
            #endregion


            var going_temp = Burn_in_Record.Where(x => x.OQCCheckBT != null && x.OQCCheckFT == null).ToList();//15.正在进行OQC的条码清单、个数
            var going = Burn_in_PutOutJson(going_temp);

            var abnormalList_temp1 = (from m in Burn_in_Record where m.Burn_in_OQCCheckAbnormal != "正常" orderby m.BarCodesNum select m).ToList();//10.异常记录清单(包含正在OQC)

            var going_temp_normal = going_temp.Where(m => m.Burn_in_OQCCheckAbnormal == null).ToList();
            var abnormalList_temp = abnormalList_temp1.Except(going_temp_normal).ToList();

            var abnormalList = Burn_in_PutOutJson(abnormalList_temp);

            #region MyRegion
            //#region ----------11.异常工时----------
            ////----------11.异常工时----------
            //int days = 0, hours = 0, minutes = 0, seconds = 0;
            //foreach (var item in abnormalList_temp)
            //{
            //    if (item.OQCCheckTime != null)
            //    {
            //        days = days + item.OQCCheckTime.Value.Days;
            //        hours = hours + item.OQCCheckTime.Value.Hours;
            //        minutes = minutes + item.OQCCheckTime.Value.Minutes;
            //        seconds = seconds + item.OQCCheckTime.Value.Seconds;
            //    }
            //}
            //TimeSpan abnormal_time = new TimeSpan(days, hours, minutes, seconds); //11.异常工时
            //#endregion
            //
            //var abnormal_Count = (from m in Burn_in_Record where m.Burn_in_OQCCheckAbnormal != "正常" select m).Count();//9.异常次数
            //
            //#region ---------------12.经过2次及以上PQC已完成的条码清单-----------------
            //List<Burn_in> finishedAnd2record_temp = new List<Burn_in>();//12.经过2次及以上PQC已完成的条码清单、个数
            //foreach (var item in Burn_in_RecordBarCodeList)
            //{
            //    if (Burn_in_Record.Where(c => c.BarCodesNum == item).ToList().Count() > 1)
            //    {
            //        var i = Burn_in_Record.Where(c => c.BarCodesNum == item).ToList().Count(c => c.OQCCheckFinish == true);
            //        if (i == 1)
            //        {
            //            finishedAnd2record_temp.AddRange(Burn_in_Record.Where(c => c.BarCodesNum == item).ToList());
            //        }
            //    }
            //}
            //var finishedAnd2record = Burn_in_PutOutJson(finishedAnd2record_temp);
            //#endregion


            //#region ---------------13.经过1次以上PQC未通过的条码清单-----------------
            //List<Burn_in> unfinishAndRecord_temp = new List<Burn_in>();//13.经过1次以上OQC未通过的条码清单、个数
            //foreach (var item in Burn_in_RecordBarCodeList)
            //{
            //    List<Burn_in> temp = Burn_in_Record.Where(c => c.BarCodesNum == item).ToList();
            //    if (temp.Count() >= 1 && temp.Count(c => c.OQCCheckFinish == true) == 0)
            //    {
            //        unfinishAndRecord_temp.AddRange(temp);
            //    }
            //}
            //unfinishAndRecord_temp = unfinishAndRecord_temp.Except(going_temp).ToList();
            //var unfinishAndRecord = Burn_in_PutOutJson(unfinishAndRecord_temp);
            //#endregion

            #endregion

            var unbeginRecord_temp = orderBoxBarCodeList.ToArray().Except(finishedList.ToArray()).ToList().Except(going_temp.Select(c => c.BarCodesNum).ToArray()).ToList();//14.未开始OQC的条码清单、个数(排除已完成（包含正常异常）、正在进行)

            string unbeginRecord = null;
            foreach (var item in unbeginRecord_temp)
            {
                if (unbeginRecord == null)
                {
                    unbeginRecord = "[\"" + item;
                }
                else
                {
                    unbeginRecord = unbeginRecord + "\",\"" + item;
                }
                if (unbeginRecord_temp.IndexOf(item) == unbeginRecord_temp.Count() - 1)
                {
                    unbeginRecord = unbeginRecord + "\"]";
                }
            }

            var passed_temp = Burn_in_Record.Where(x => x.OQCCheckFinish == true).ToList();//16.已经完成OQC的条码清单、个数
            var passed = Burn_in_PutOutJson(passed_temp);
            //string abnormalStatistics = null; //17.异常信息统计

            #region ---------------PQC完成后超过24小时未开始老化工序的条码清单-----------------
            //finish_assemble_list 已完成组装清单
            var finish_assemble_list = db.Assemble.Where(c => c.OrderNum == OrderNum && c.PQCCheckFT != null && c.PQCCheckFinish == true && c.RepetitionPQCCheck == false && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).OrderBy(c => c.BoxBarCode).ToList();
            //burn_in_Record 老化全部记录
            var burn_in_Record = db.Burn_in.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).ToList();
            List<Assemble> Overtime_never_join_burn_in_list = new List<Assemble>();
            foreach (var record in finish_assemble_list)
            {
                if (burn_in_Record.Count(c => c.BarCodesNum == record.BoxBarCode) == 0 && (DateTime.Now - record.PQCCheckFT).Value.TotalHours >= 24)
                {
                    Overtime_never_join_burn_in_list.Add(record);
                }
            }
            ViewBag.Overtime_never_join_burn_in = Overtime_never_join_burn_in_list;
            #endregion

            #region ---------------老化完成后超过72小时未开始包装工序的条码清单-----------------
            //finish_burn_in_list 已完成老化清单
            var finish_burn_in_list = db.Burn_in.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum) && c.OQCCheckFT != null && c.OQCCheckFinish == true).OrderBy(c => c.BarCodesNum).ToList();
            //appearance_list 包装全部记录
            var appearance_list = db.Appearance.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).ToList();
            List<Burn_in> Overtime_never_join_appearance_list = new List<Burn_in>();
            foreach (var record in finish_burn_in_list)
            {
                if (appearance_list.Count(c => c.BarCodesNum == record.BarCodesNum) == 0 && (DateTime.Now - record.OQCCheckFT).Value.TotalHours >= 72)
                {
                    Overtime_never_join_appearance_list.Add(record);
                }
            }
            ViewBag.Overtime_never_join_appearance = Overtime_never_join_appearance_list;
            #endregion


            #endregion

            #region ---------------将对象转为列矩阵JSON
            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            //创建JSON对象
            JObject JsonObj = new JObject
            {
                { "OrderNum", OrderNum },//1.订单号
                { "modelGroupQuantity", modelGroupQuantity },//2.订单模组数
                { "finished", finished},//3.订单已完成OQC个数
                { "Burn_in_PQC_Count", Burn_in_OQC_Count},//4.订单OQC全部记录条数
                { "finisthRate", finisthRate },//5.完成率：完成数/订单的模组数
                { "passRate", passRate },//6.合格率：完成数/记录数
                { "firstPassYield_Rate", firstPassYield_Rate },//7.直通率：直通数/模组数
                { "firstPassYieldCount", firstPassYieldCount },//8.直通数
                //{ "abnormal_Count", abnormal_Count },//9.异常次数
                { "abnormalListNum", abnormalList_temp.Count() },//10.异常记录清单
                //{ "abnormal_time",abnormal_time },//11.异常工时
                //{ "finishedAnd2recordNum", finishedAnd2record_temp.Count() },//12.经过2次及以上OQC已完成的条码清单、个数
                //{ "unfinishAndRecordNum", unfinishAndRecord_temp.Count() },//13.经过1次以上OQC未通过的条码清单、个数
                { "unbeginRecordNum", unbeginRecord_temp.Count() },//14.未开始OQC的条码清单、个数
                { "goingNum", going_temp.Count() },//15.正在进行OQC的条码清单、个数
                { "passedNum", passed_temp.Count() },//16.已经完成OQC的条码清单、个数
                { "Overtime_never_join_burn_in", Overtime_never_join_burn_in_list.Count() },//PQC完成后超过24小时未开始老化工序的条码清单个数
                { "Overtime_never_join_appearance", Overtime_never_join_appearance_list.Count() },//老化完成后超过72小时未开始包装工序的条码清单个数
                //17.异常信息统计
            };
            #endregion

            ViewBag.abnormalList_temp = abnormalList_temp;//10.异常记录清单
            //ViewBag.finishedAnd2record_temp = finishedAnd2record_temp;//12.经过2次及以上OQC已完成的条码清单、个数
            //ViewBag.unfinishAndRecord_temp = unfinishAndRecord_temp;//13.经过1次以上OQC未通过的条码清单、个数
            ViewBag.unbeginRecord_temp = unbeginRecord_temp;//14.未开始OQC的条码清单、个数
            ViewBag.going_temp = going_temp;//15.正在进行OQC的条码清单、个数
            ViewBag.passed_temp = passed_temp;//16.已经完成OQC的条码清单、个数
            ViewBag.JsonObj = JsonObj;


            return View();
        }

        public JObject Burn_in_PutOutJson(List<Burn_in> inputlist)
        {
            JObject OutPutJson = new JObject();
            OutPutJson.Add("title", "[Id,OrderNum,BarCodesNum,A,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Burn_in_OQCCheckAbnormal," +
                "RepairCondition,OQCCheckFinish]");
            foreach (var item in inputlist)
            {
                OutPutJson.Add((inputlist.IndexOf(item) + 1).ToString(), "[" + item.Id + "," + item.OrderNum + "," + item.BarCodesNum + "," +
                    item.OQCCheckBT + "," + item.OQCPrincipal + "," + item.OQCCheckFT + "," + item.OQCCheckTime + "," + item.OQCCheckTimeSpan + "," +
                    item.Burn_in_OQCCheckAbnormal + "," + item.RepairCondition + "," + item.OQCCheckFinish + "]");
            }
            return OutPutJson;
        }

        public ActionResult Burn_inNuo(string OrderNum)
        {
            var barcodelist = db.BarCodeRelation.Where(c => c.NewOrderNum == OrderNum).ToList();
            JArray abnormal = new JArray();
            JArray jarraynomal = new JArray();
            JObject total = new JObject();
            foreach (var item in barcodelist)
            {
                JObject nuojobject = new JObject();
                List<string> oldbarcode = FindOld(item.NewBarCodesNum);
                JArray barcodeStatu = new JArray();
                foreach (var displaybarcode in oldbarcode)
                {
                    JObject barcodeitem = new JObject();
                    barcodeitem.Add("barcode", displaybarcode);
                    var statu = db.Burn_in.Where(c => (c.OldBarCodesNum == displaybarcode || c.OldBarCodesNum == null) && c.OrderNum == OrderNum && c.BarCodesNum == item.NewBarCodesNum).ToList();
                    if (statu.Count != 0)
                    {
                        var abnormalist = statu.Where(c => c.OQCCheckBT != null && c.OQCCheckFT != null && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal != null).Select(c => c.Burn_in_OQCCheckAbnormal).ToArray();
                        if (abnormalist.Count() != 0)
                        {
                            JObject abnarmalitem = new JObject();
                            abnarmalitem.Add("barcode", displaybarcode);
                            abnarmalitem.Add("abnormal", string.Join(",", abnormalist));
                            abnormal.Add(abnarmalitem);
                        }
                        if (statu.Count(c => c.OQCCheckBT != null && c.OQCCheckFinish == true) != 0)
                        {
                            var begintime = db.Assemble.OrderByDescending(c => c.PQCCheckFT).Where(c => (c.OldBarCodesNum == displaybarcode || c.OldBarCodesNum == null) && c.OrderNum == OrderNum && c.BoxBarCode == item.NewBarCodesNum && c.PQCCheckBT != null && c.PQCCheckFinish == true).Select(c => c.PQCCheckFT).FirstOrDefault();
                            var burnin = db.Burn_in.OrderBy(c => c.OQCCheckBT).Where(c => (c.OldBarCodesNum == displaybarcode || c.OldBarCodesNum == null) && c.OrderNum == OrderNum && c.BarCodesNum == item.NewBarCodesNum).Select(c => c.OQCCheckBT).FirstOrDefault();
                            var burninfinash = db.Burn_in.OrderByDescending(c => c.OQCCheckFT).Where(c => (c.OldBarCodesNum == displaybarcode || c.OldBarCodesNum == null) && c.OrderNum == OrderNum && c.BarCodesNum == item.NewBarCodesNum && c.OQCCheckFinish == true).Select(c => c.OQCCheckFT).FirstOrDefault();
                            var packingBegin = db.Appearance.OrderBy(c => c.OQCCheckBT).Where(c => (c.OldBarCodesNum == displaybarcode || c.OldBarCodesNum == null) && c.OrderNum == OrderNum && c.BarCodesNum == item.NewBarCodesNum).Select(c => c.OQCCheckBT).FirstOrDefault();
                            if (burnin == null && (DateTime.Now - begintime).Value.TotalHours >= 24)
                            {
                                barcodeitem.Add("statue", "PQC后超过24小时未进入老化");
                            }
                            else if (packingBegin == null && (DateTime.Now - burninfinash).Value.TotalHours >= 72)
                            {
                                barcodeitem.Add("statue", "老化后超过72小时未进入包装");
                            }
                            else
                            {
                                barcodeitem.Add("statue", "已完成");
                            }
                        }
                        else
                        {
                            barcodeitem.Add("statue", "进行中");
                        }
                    }
                    else
                    {
                        barcodeitem.Add("statue", "未开始");
                    }
                    barcodeStatu.Add(barcodeitem);
                }
                oldbarcode.Add(item.NewBarCodesNum);
                JArray infoArray = new JArray();
                oldbarcode.ForEach(c => infoArray.Add(c));
                nuojobject.Add("info", infoArray);
                nuojobject.Add("barcodeInfo", barcodeStatu);
                jarraynomal.Add(nuojobject);
            }

            total.Add("normal", jarraynomal);
            total.Add("abnormal", abnormal);
            return Content(JsonConvert.SerializeObject(total));
        }
        #endregion




        #region -----------------校正详情页面
        [HttpPost]
        public ActionResult Calibration(string OrderNum)
        {

            #region ---------------读取数据，处理数据

            ViewBag.OrderNum = OrderNum;//1.订单号

            var order = (from m in db.OrderMgm where m.OrderNum == OrderNum select m).FirstOrDefault();//取出订单
            ViewBag.PlatformType = order.PlatformType; //平台类型
            ViewBag.DeliveryDate = order.DeliveryDate; //出货日期

            var modelGroupQuantity = (from m in db.OrderMgm where m.OrderNum == OrderNum select m).FirstOrDefault().Boxes;//2.订单模组数
            var orderBoxBarCodeList = db.BarCodes.Where(m => m.OrderNum == OrderNum).OrderBy(c => c.BarCodesNum).Select(m => m.BarCodesNum).ToList();//订单的所有条码清单(值为空)
            var Calibration_Record = (from m in db.CalibrationRecord where m.OrderNum == OrderNum && (m.OldOrderNum == null || m.OldOrderNum == OrderNum) select m).OrderBy(x => x.ModuleGroupNum).ToList();//订单校正全部记录
            //查找是否有挪用直接出库给客户记录
            var newbarcode = db.Warehouse_Join.Where(c => c.NewBarcode == OrderNum && c.CartonOrderNum != OrderNum).Select(c => c.BarCodeNum).ToList();
            if (newbarcode.Count != 0)
            {
                var temp = db.CalibrationRecord.Where(c => newbarcode.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || newbarcode.Contains(c.OldBarCodesNum))).ToList();
                Calibration_Record.AddRange(temp);
            }
            var Calibration_RecordBarCodeList = Calibration_Record.Select(m => m.BarCodesNum).Distinct().ToList();//校正记录全部条码(模组号)清单(去重)

            //操作记录时长(Timespan  00:00:05.4780887)
            var time = Calibration_Record.Select(c => c.CalibrationTime).ToList();
            TimeSpan? time2 = new TimeSpan();
            time.ForEach(c => time2 = time2 + c);
            if (time2 == null)
            {
                ViewBag.OperationTime = "0秒";
                ViewBag.OperationAverageTime = "0秒";
            }
            else
            {
                ViewBag.OperationTime = comm.changetimespan(time2.Value.TotalMilliseconds);
                //平均操作时长(毫秒)
                var time3 = time2.Value.TotalMilliseconds / Calibration_Record.Count();
                ViewBag.OperationAverageTime = comm.changetimespan(time3);
            }
            //异常操作记录时长(Timespan  00:00:05.4780887)
            var value = Calibration_Record.Where(c => c.Normal == false).Select(c => c.CalibrationTime).ToList();
            if (value.Count == 0)
            {
                ViewBag.OperationAbnormalTime = "0秒";
                ViewBag.OperationAverageAbnormalTime = "0秒";
            }
            else
            {
                TimeSpan? time4 = new TimeSpan();
                value.ForEach(c => time4 = c == null ? time4 : time4 + c);
                ViewBag.OperationAbnormalTime = comm.changetimespan(time4.Value.TotalMilliseconds);
                //异常平均操作时长(毫秒)
                var time5 = time4.Value.TotalMilliseconds / value.Count();
                ViewBag.OperationAverageAbnormalTime = comm.changetimespan(time5);
            }

            var finished = Calibration_Record.Count(m => m.Normal == true && m.RepetitionCalibration == false);//3.订单已完成校正个数
            var finishedList = Calibration_Record.Where(m => m.Normal == true).Select(m => m.ModuleGroupNum).ToList(); //订单已完成校正的条码(模组号)清单

            var Calibration_Count = Calibration_Record.Where(c => c.RepetitionCalibration == false).Count();//4.订单校正全部记录条数

            var finisthRate = (Convert.ToDouble(finished) / Calibration_RecordBarCodeList.Count() * 100).ToString("F2");//5.完成率：完成数/订单的模组数

            var passRate = (Convert.ToDouble(finished) / Calibration_Count * 100).ToString("F2");//6.合格率：完成数/记录数

            #region ---------------------一次直通记录、个数、直通率----------------------
            //---------------------一次直通记录----------------------
            var Calibration_Record_abnormal_BoxBarCode_list = Calibration_Record.Where(c => c.AbnormalDescription != "正常").Select(c => c.ModuleGroupNum).ToList();//异常记录(模组号)清单
            var firstPassYield_temp = Calibration_Record.DistinctBy(c => c.ModuleGroupNum).Where(c => c.AbnormalDescription == "正常" && c.Normal == true).ToList();//Finish记录
            List<CalibrationRecord> firstPassYield_expect = new List<CalibrationRecord>();//有异常记录的条码Finish记录
            foreach (var item in Calibration_Record_abnormal_BoxBarCode_list)
            {
                foreach (var Calibration_record in firstPassYield_temp)
                {
                    if (Calibration_record.ModuleGroupNum == item)
                    {
                        firstPassYield_expect.Add(Calibration_record);
                    }
                }
            }
            List<CalibrationRecord> firstPassYield = new List<CalibrationRecord>();
            firstPassYield = firstPassYield_temp.Except(firstPassYield_expect).ToList();//一次直通记录
            var firstPassYieldCount = firstPassYield.Count();//8.一次直通数
            var firstPassYield_Rate = (Convert.ToDouble(firstPassYieldCount) / modelGroupQuantity * 100).ToString("F2");//7.直通率：直通数/模组数
            #endregion


            var going_temp = Calibration_Record.Where(x => x.BeginCalibration != null && x.FinishCalibration == null).ToList();//15.正在进行校正的条码清单、个数
            var going = Calibration_PutOutJson(going_temp);

            var abnormalList_temp1 = (from m in Calibration_Record where m.AbnormalDescription != "正常" orderby m.ModuleGroupNum select m).ToList();//10.异常记录清单(包含正在校正)

            var abnormalList_temp = abnormalList_temp1.Except(going_temp).ToList();

            var abnormalList = Calibration_PutOutJson(abnormalList_temp);

            #region MyRegion
            //#region ----------11.异常工时----------
            ////----------11.异常工时----------
            //int days = 0, hours = 0, minutes = 0, seconds = 0;
            //foreach (var item in abnormalList_temp)
            //{
            //    if (item.CalibrationTime != null)
            //    {
            //        days = days + item.CalibrationTime.Value.Days;
            //        hours = hours + item.CalibrationTime.Value.Hours;
            //        minutes = minutes + item.CalibrationTime.Value.Minutes;
            //        seconds = seconds + item.CalibrationTime.Value.Seconds;
            //    }
            //}
            //TimeSpan abnormal_time = new TimeSpan(days, hours, minutes, seconds); //11.异常工时


            ////#endregion

            //var abnormal_Count = (from m in Calibration_Record where (m.AbnormalDescription == null || m.AbnormalDescription != "正常") select m).Count();//9.异常次数

            //#region ---------------12.经过2次及以上校正已完成的条码清单-----------------
            //List<CalibrationRecord> finishedAnd2record_temp = new List<CalibrationRecord>();//12.经过2次及以上校正已完成的条码清单、个数
            //foreach (var item in Calibration_RecordBarCodeList)
            //{
            //    if (Calibration_Record.Where(c => c.ModuleGroupNum == item).ToList().Count() > 1)
            //    {
            //        var i = Calibration_Record.Where(c => c.ModuleGroupNum == item).ToList().Count(c => c.Normal == true);
            //        if (i == 1)
            //        {
            //            finishedAnd2record_temp.AddRange(Calibration_Record.Where(c => c.ModuleGroupNum == item).ToList());
            //        }
            //    }
            //}
            //var finishedAnd2record = Burn_in_PutOutJson(finishedAnd2record_temp);
            //#endregion


            //#region ---------------13.经过1次以上校正未通过的条码清单-----------------
            //List<CalibrationRecord> unfinishAndRecord_temp = new List<CalibrationRecord>();//13.经过1次以上校正未通过的条码清单、个数
            //foreach (var item in Calibration_RecordBarCodeList)
            //{
            //    List<CalibrationRecord> temp = Calibration_Record.Where(c => c.ModuleGroupNum == item).ToList();
            //    if (temp.Count() >= 1 && temp.Count(c => c.Normal == true) == 0)
            //    {
            //        unfinishAndRecord_temp.AddRange(temp);
            //    }
            //}
            //unfinishAndRecord_temp = unfinishAndRecord_temp.Except(going_temp).ToList();
            //var unfinishAndRecord = Burn_in_PutOutJson(unfinishAndRecord_temp);
            //#endregion

            #endregion

            var passed_temp = Calibration_Record.Where(x => x.AbnormalDescription == "正常" || x.AbnormalDescription == null && x.Normal == true).OrderBy(x => x.BarCodesNum).ToList();//16.已经完成校正的条码清单、个数
            List<string> passedlist = new List<string>();
            passedlist = passed_temp.OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();
            var passed = Calibration_PutOutJson(passed_temp);

            //var unbeginRecord_temp = orderBoxBarCodeList.Except(finishedList).ToList().Except(going_temp.Select(c => c.ModuleGroupNum)).ToList();//14.未开始校正的条码清单、个数(排除已完成（包含正常异常）、正在进行)
            var unbeginRecord_temp = orderBoxBarCodeList.Except(passedlist).ToList();//14.未开始校正的条码清单、个数(排除已完成（包含正常异常）、正在进行)

            string unbeginRecord = null;
            foreach (var item in unbeginRecord_temp)
            {
                if (unbeginRecord == null)
                {
                    unbeginRecord = "[\"" + item;
                }
                else
                {
                    unbeginRecord = unbeginRecord + "\",\"" + item;
                }
                if (unbeginRecord_temp.IndexOf(item) == unbeginRecord_temp.Count() - 1)
                {
                    unbeginRecord = unbeginRecord + "\"]";
                }
            }


            //string abnormalStatistics = null; //17.异常信息统计

            #endregion

            #region ---------------将对象转为列矩阵JSON
            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            //创建JSON对象
            JObject JsonObj = new JObject
            {
                { "OrderNum", OrderNum },//1.订单号
                { "modelGroupQuantity", modelGroupQuantity },//2.订单模组数
                { "finished", finished},//3.订单已完成校正个数
                { "Calibration_Count", Calibration_Count},//4.订单校正全部记录条数
                { "finisthRate", finisthRate },//5.完成率：完成数/订单的模组数
                { "passRate", passRate },//6.合格率：完成数/记录数
                { "firstPassYield_Rate", firstPassYield_Rate },//7.直通率：直通数/模组数
                { "firstPassYieldCount", firstPassYieldCount },//8.直通数
                //{ "abnormal_Count", abnormal_Count },//9.异常次数
                { "abnormalListNum", abnormalList_temp.Count() },//10.异常记录清单
                //{ "abnormal_time",abnormal_time },//11.异常工时
                //{ "finishedAnd2recordNum", finishedAnd2record_temp.Count() },//12.经过2次及以上校正已完成的条码清单、个数
                //{ "unfinishAndRecordNum", unfinishAndRecord_temp.Count() },//13.经过1次以上校正未通过的条码清单、个数
                { "unbeginRecordNum", unbeginRecord_temp.Count() },//14.未开始校正的条码清单、个数
                { "goingNum", going_temp.Count() },//15.正在进行校正的条码清单、个数
                { "passedNum", passed_temp.Count() },//16.已经完成校正的条码清单、个数
                //17.异常信息统计
            };

            ViewBag.abnormalList_temp = abnormalList_temp;//10.异常记录清单
            //ViewBag.finishedAnd2record_temp = finishedAnd2record_temp;//12.经过2次及以上校正已完成的条码清单、个数
            //ViewBag.unfinishAndRecord_temp = unfinishAndRecord_temp;//13.经过1次以上OQC未通过的条码清单、个数
            ViewBag.unbeginRecord_temp = unbeginRecord_temp;//14.未开始校正的条码清单、个数
            ViewBag.going_temp = going_temp;//15.正在进行校正的条码清单、个数
            ViewBag.passed_temp = passed_temp;//16.已经完成校正的条码清单、个数
            ViewBag.JsonObj = JsonObj;

            #endregion

            return View();
        }

        public JObject Calibration_PutOutJson(List<CalibrationRecord> inputlist)
        {
            JObject OutPutJson = new JObject();
            OutPutJson.Add("title", "[Id,OrderNum,ModuleGroupNum,A,BeginCalibration,Operator,FinishCalibration,CalibrationTime,CalibrationTimeSpan,AbnormalDescription,Normal]");
            foreach (var item in inputlist)
            {
                OutPutJson.Add((inputlist.IndexOf(item) + 1).ToString(), "[" + item.ID + "," + item.OrderNum + "," + item.ModuleGroupNum + "," +
                    item.BeginCalibration + "," + item.Operator + "," + item.FinishCalibration + "," + item.CalibrationTime + "," + item.CalibrationTimeSpan + "," +
                    item.AbnormalDescription + "," + item.Normal + "]");
            }
            return OutPutJson;
        }

        public ActionResult CalibrationNuo(string OrderNum)
        {
            var barcodelist = db.BarCodeRelation.Where(c => c.NewOrderNum == OrderNum).ToList();
            JArray abnormal = new JArray();
            JArray jarraynomal = new JArray();
            JObject total = new JObject();
            foreach (var item in barcodelist)
            {
                JObject nuojobject = new JObject();
                List<string> oldbarcode = FindOld(item.NewBarCodesNum);
                JArray barcodeStatu = new JArray();
                foreach (var displaybarcode in oldbarcode)
                {
                    JObject barcodeitem = new JObject();
                    barcodeitem.Add("barcode", displaybarcode);
                    var statu = db.CalibrationRecord.Where(c => (c.OldBarCodesNum == displaybarcode || c.OldBarCodesNum == null) && c.OrderNum == OrderNum && c.BarCodesNum == item.NewBarCodesNum).ToList();
                    if (statu.Count != 0)
                    {
                        var abnormalist = statu.Where(c => c.BeginCalibration != null && c.FinishCalibration != null && c.Normal == false && c.AbnormalDescription != null).Select(c => c.AbnormalDescription).ToArray();
                        if (abnormalist.Count() != 0)
                        {
                            JObject abnarmalitem = new JObject();
                            abnarmalitem.Add("barcode", displaybarcode);
                            abnarmalitem.Add("abnormal", string.Join(",", abnormalist));
                            abnormal.Add(abnarmalitem);
                        }
                        if (statu.Count(c => c.BeginCalibration != null && c.Normal == true) != 0)
                        {
                            barcodeitem.Add("statue", "已完成");

                        }
                        else
                        {
                            barcodeitem.Add("statue", "进行中");
                        }
                    }
                    else
                    {
                        barcodeitem.Add("statue", "未开始");
                    }
                    barcodeStatu.Add(barcodeitem);
                }
                oldbarcode.Add(item.NewBarCodesNum);
                JArray infoArray = new JArray();
                oldbarcode.ForEach(c => infoArray.Add(c));
                nuojobject.Add("info", infoArray);
                nuojobject.Add("barcodeInfo", barcodeStatu);
                jarraynomal.Add(nuojobject);
            }
            total.Add("normal", jarraynomal);
            total.Add("abnormal", abnormal);
            return Content(JsonConvert.SerializeObject(total));
        }
        #endregion




        #region -----------------外观包装OQC详情页面
        [HttpPost]
        public ActionResult Appearance(string OrderNum)
        {

            #region ---------------读取数据，处理数据

            ViewBag.OrderNum = OrderNum;//1.订单号

            var order = (from m in db.OrderMgm where m.OrderNum == OrderNum select m).FirstOrDefault();//取出订单
            ViewBag.PlatformType = order.PlatformType; //平台类型
            ViewBag.DeliveryDate = order.DeliveryDate; //出货日期

            var modelGroupQuantity = (from m in db.OrderMgm where m.OrderNum == OrderNum select m).ToList().FirstOrDefault().Boxes;//2.订单模组数
            var orderBoxBarCodeList = db.BarCodes.Where(m => m.OrderNum == OrderNum).OrderBy(c => c.BarCodesNum).Select(m => m.BarCodesNum).ToList();//订单的所有条码清单

            var Appearance_Record = (from m in db.Appearance where m.OrderNum == OrderNum && (m.OldOrderNum == null || m.OldOrderNum == OrderNum) select m).OrderBy(x => x.BarCodesNum).ToList();//订单外观包装OQC全部记录
            if (Appearance_Record == null)
            {
                Appearance_Record = db.Appearance.Where(c => c.ToOrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).OrderBy(c => c.BarCodesNum).ToList();
            }
            //查找是否有挪用直接出库给客户记录
            var newbarcode = db.Warehouse_Join.Where(c => c.NewBarcode == OrderNum && c.CartonOrderNum != OrderNum).Select(c => c.BarCodeNum).ToList();
            if (newbarcode.Count != 0)
            {
                var temp = db.Appearance.Where(c => newbarcode.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || newbarcode.Contains(c.OldBarCodesNum))).ToList();
                Appearance_Record.AddRange(temp);
            }
            var Appearance_RecordBarCodeList = Appearance_Record.Select(m => m.BarCodesNum).Distinct().ToList();//外观包装OQC记录全部条码清单(去重)

            //操作记录时长(Timespan  00:00:05.4780887)
            var time = Appearance_Record.Select(c => c.OQCCheckTime).ToList();
            TimeSpan? time2 = new TimeSpan();
            time.ForEach(c => time2 = time2 + c);
            if (time2 == null)
            {
                ViewBag.OperationTime = "0秒";
                ViewBag.OperationAverageTime = "0秒";
            }
            else
            {
                ViewBag.OperationTime = comm.changetimespan(time2.Value.TotalMilliseconds);
                //平均操作时长(毫秒)
                var time3 = time2.Value.TotalMilliseconds / Appearance_Record.Count();
                ViewBag.OperationAverageTime = comm.changetimespan(time3);
            }
            //异常操作记录时长(Timespan  00:00:05.4780887)
            var value = Appearance_Record.Where(c => c.OQCCheckFinish == false).Select(c => c.OQCCheckTime).ToList();
            if (value.Count == 0)
            {
                ViewBag.OperationAbnormalTime ="0秒" ;
                ViewBag.OperationAverageAbnormalTime ="0秒";
            }
            else
            {
                TimeSpan? time4 = new TimeSpan();
                time.ForEach(c => time4 = c == null ? time4 : time4 + c);
                ViewBag.OperationAbnormalTime = comm.changetimespan(time4.Value.TotalMilliseconds);
                //异常平均操作时长(毫秒)
                var time5 = time4.Value.TotalMilliseconds / value.Count();
                ViewBag.OperationAverageAbnormalTime = comm.changetimespan(time5);
            }

            var finished = Appearance_Record.Count(m => m.OQCCheckFinish == true);//3.订单已完成外观包装OQC个数
            var finishedList = Appearance_Record.Where(m => m.OQCCheckFinish == true).Select(m => m.BarCodesNum).ToList(); //订单已完成外观包装OQC的条码清单

            var Appearance_OQC_Count = Appearance_Record.Count();//4.订单外观包装OQC全部记录条数

            var finisthRate = (Convert.ToDouble(finished) / Appearance_RecordBarCodeList.Count() * 100).ToString("F2");//5.完成率：完成数/订单的模组数

            var passRate = (Convert.ToDouble(finished) / Appearance_OQC_Count * 100).ToString("F2");//6.合格率：完成数/记录数

            #region ---------------------一次直通记录、个数、直通率----------------------
            //---------------------一次直通记录----------------------
            var Appearance_Record_abnormal_BoxBarCode_list = Appearance_Record.Where(c => c.RepairCondition != "正常" || c.Appearance_OQCCheckAbnormal != "正常").Select(c => c.BarCodesNum).ToList();//异常记录
            var firstPassYield_temp = Appearance_Record.DistinctBy(c => c.BarCodesNum).Where(c => c.Appearance_OQCCheckAbnormal == "正常" && c.RepairCondition == "正常" && c.OQCCheckFinish == true).ToList();//Finish记录
            List<Appearance> firstPassYield_expect = new List<Appearance>();//有异常记录的条码Finish记录
            foreach (var item in Appearance_Record_abnormal_BoxBarCode_list)
            {
                foreach (var appearance_erecord in firstPassYield_temp)
                {
                    if (appearance_erecord.BarCodesNum == item)
                    {
                        firstPassYield_expect.Add(appearance_erecord);
                    }
                }
            }
            List<Appearance> firstPassYield = new List<Appearance>();
            firstPassYield = firstPassYield_temp.Except(firstPassYield_expect).ToList();//一次直通记录
            var firstPassYieldCount = firstPassYield.Count();//8.一次直通数
            var firstPassYield_Rate = (Convert.ToDouble(firstPassYieldCount) / modelGroupQuantity * 100).ToString("F2");//7.直通率：直通数/模组数
            #endregion


            var going_temp = Appearance_Record.Where(x => x.OQCCheckBT != null && x.OQCCheckFT == null).ToList();//15.正在进行外观包装OQC的条码清单、个数
            var going = Appearance_PutOutJson(going_temp);

            var abnormalList_temp1 = (from m in Appearance_Record where m.Appearance_OQCCheckAbnormal != "正常" orderby m.BarCodesNum select m).ToList();//10.异常记录清单(包含正在外观包装OQC)

            var abnormalList_temp = abnormalList_temp1.Except(going_temp).ToList();

            var abnormalList = Appearance_PutOutJson(abnormalList_temp);

            #region MyRegion
            //#region ----------11.异常工时----------
            ////----------11.异常工时----------
            //int days = 0, hours = 0, minutes = 0, seconds = 0;
            //foreach (var item in abnormalList_temp)
            //{
            //    if (item.OQCCheckTime != null)
            //    {
            //        days = days + item.OQCCheckTime.Value.Days;
            //        hours = hours + item.OQCCheckTime.Value.Hours;
            //        minutes = minutes + item.OQCCheckTime.Value.Minutes;
            //        seconds = seconds + item.OQCCheckTime.Value.Seconds;
            //    }
            //}
            //TimeSpan abnormal_time = new TimeSpan(days, hours, minutes, seconds); //11.异常工时
            //#endregion
            //
            //var abnormal_Count = (from m in Appearance_Record where m.Appearance_OQCCheckAbnormal != "正常" select m).Count();//9.异常次数
            //
            //#region ---------------12.经过2次及以上外观包装OQC已完成的条码清单-----------------
            //List<Burn_in> finishedAnd2record_temp = new List<Burn_in>();//12.经过2次及以上外观包装QC已完成的条码清单、个数
            //foreach (var item in Appearance_RecordBarCodeList)
            //{
            //    if (Appearance_Record.Where(c => c.BarCodesNum == item).ToList().Count() > 1)
            //    {
            //        var i = Appearance_Record.Where(c => c.BarCodesNum == item).ToList().Count(c => c.OQCCheckFinish == true);
            //        if (i == 1)
            //        {
            //            finishedAnd2record_temp.AddRange(Appearance_Record.Where(c => c.BarCodesNum == item).ToList());
            //        }
            //    }
            //}
            //var finishedAnd2record = Appearance_PutOutJson(finishedAnd2record_temp);
            //#endregion


            //#region ---------------13.经过1次以上外观包装QC未通过的条码清单-----------------
            //List<Appearance> unfinishAndRecord_temp = new List<Appearance>();//13.经过1次以上外观包装OQC未通过的条码清单、个数
            //foreach (var item in Appearance_RecordBarCodeList)
            //{
            //    List<Appearance> temp = Appearance_Record.Where(c => c.BarCodesNum == item).ToList();
            //    if (temp.Count() >= 1 && temp.Count(c => c.OQCCheckFinish == true) == 0)
            //    {
            //        unfinishAndRecord_temp.AddRange(temp);
            //    }
            //}
            //unfinishAndRecord_temp = unfinishAndRecord_temp.Except(going_temp).ToList();
            //var unfinishAndRecord = Appearance_PutOutJson(unfinishAndRecord_temp);
            //#endregion

            #endregion

            //var unbeginRecord_temp = orderBoxBarCodeList.ToArray().Except(finishedList.ToArray()).ToList().Except(going_temp.Select(c => c.BarCodesNum).ToArray()).ToList();//14.未开始外观包装OQC的条码清单、个数(排除已完成（包含正常异常）、正在进行)
            var unbeginRecord_temp = orderBoxBarCodeList.ToArray().Except(Appearance_Record.Select(m => m.BarCodesNum).ToList().Distinct().ToArray()).ToList();//14.未开始外观包装OQC的条码清单、个数(排除已有记录)

            string unbeginRecord = null;
            foreach (var item in unbeginRecord_temp)
            {
                if (unbeginRecord == null)
                {
                    unbeginRecord = "[\"" + item;
                }
                else
                {
                    unbeginRecord = unbeginRecord + "\",\"" + item;
                }
                if (unbeginRecord_temp.IndexOf(item) == unbeginRecord_temp.Count() - 1)
                {
                    unbeginRecord = unbeginRecord + "\"]";
                }
            }

            var passed_temp = Appearance_Record.Where(x => x.Appearance_OQCCheckAbnormal == "正常").ToList();//16.已经完成外观包装OQC的条码清单、个数
            var passed = Appearance_PutOutJson(passed_temp);
            //string abnormalStatistics = null; //17.异常信息统计

            #region ---------------老化完成后超过72小时未开始包装工序的条码清单-----------------
            //finish_burn_in_list 已完成老化清单
            var finish_burn_in_list = db.Burn_in.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum) && c.OQCCheckFT != null && c.OQCCheckFinish == true).OrderBy(c => c.BarCodesNum).ToList();
            //appearance_list 包装全部记录
            var appearance_list = db.Appearance.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).ToList();
            List<Burn_in> Overtime_never_join_appearance_list = new List<Burn_in>();
            foreach (var record in finish_burn_in_list)
            {
                if (appearance_list.Count(c => c.BarCodesNum == record.BarCodesNum) == 0 && (DateTime.Now - record.OQCCheckFT).Value.TotalHours >= 72)
                {
                    Overtime_never_join_appearance_list.Add(record);
                }
            }
            ViewBag.Overtime_never_join_appearance = Overtime_never_join_appearance_list;
            #endregion


            #endregion

            #region ---------------将对象转为列矩阵JSON
            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            //创建JSON对象
            JObject JsonObj = new JObject
            {
                { "OrderNum", OrderNum },//1.订单号
                { "modelGroupQuantity", modelGroupQuantity },//2.订单模组数
                { "finished", finished},//3.订单已完成外观包装OQC个数
                { "Appearance_PQC_Count", Appearance_OQC_Count},//4.订单外观包装OQC全部记录条数
                { "finisthRate", finisthRate },//5.完成率：完成数/订单的模组数
                { "passRate", passRate },//6.合格率：完成数/记录数
                { "firstPassYield_Rate", firstPassYield_Rate },//7.直通率：直通数/模组数
                { "firstPassYieldCount", firstPassYieldCount },//8.直通数
                //{ "abnormal_Count", abnormal_Count },//9.异常次数
                { "abnormalListNum", abnormalList_temp.Count() },//10.异常记录清单
                //{ "abnormal_time",abnormal_time },//11.异常工时
                //{ "finishedAnd2recordNum", finishedAnd2record_temp.Count() },//12.经过2次及以上外观包装OQC已完成的条码清单、个数
                //{ "unfinishAndRecordNum", unfinishAndRecord_temp.Count() },//13.经过1次以上外观包装OQC未通过的条码清单、个数
                { "unbeginRecordNum", unbeginRecord_temp.Count() },//14.未开始OQC的条码清单、个数
                { "goingNum", going_temp.Count() },//15.正在进行外观包装OQC的条码清单、个数
                { "passedNum", passed_temp.Count() },//16.已经完成外观包装OQC的条码清单、个数
                { "Overtime_never_join_appearance", Overtime_never_join_appearance_list.Count() },//老化完成后超过72小时未开始包装工序的条码清单个数
                //17.异常信息统计
            };
            #endregion

            ViewBag.abnormalList_temp = abnormalList_temp;//10.异常记录清单
            //ViewBag.finishedAnd2record_temp = finishedAnd2record_temp;//12.经过2次及以上外观包装OQC已完成的条码清单、个数
            //ViewBag.unfinishAndRecord_temp = unfinishAndRecord_temp;//13.经过1次以上外观包装OQC未通过的条码清单、个数
            ViewBag.unbeginRecord_temp = unbeginRecord_temp;//14.未开始外观包装OQC的条码清单、个数
            ViewBag.going_temp = going_temp;//15.正在进行外观包装OQC的条码清单、个数
            ViewBag.passed_temp = passed_temp;//16.已经完成外观包装OQC的条码清单、个数
            ViewBag.JsonObj = JsonObj;


            return View();
        }

        public JObject Appearance_PutOutJson(List<Appearance> inputlist)
        {
            JObject OutPutJson = new JObject();
            OutPutJson.Add("title", "[Id,OrderNum,BarCodesNum,A,OQCCheckBT,OQCPrincipal,OQCCheckFT,OQCCheckTime,OQCCheckTimeSpan,Appearance_OQCCheckAbnormal," +
                "RepairCondition,OQCCheckFinish]");
            foreach (var item in inputlist)
            {
                OutPutJson.Add((inputlist.IndexOf(item) + 1).ToString(), "[" + item.Id + "," + item.OrderNum + "," + item.BarCodesNum + "," +
                    item.OQCCheckBT + "," + item.OQCPrincipal + "," + item.OQCCheckFT + "," + item.OQCCheckTime + "," + item.OQCCheckTimeSpan + "," +
                    item.Appearance_OQCCheckAbnormal + "," + item.RepairCondition + "," + item.OQCCheckFinish + "]");
            }
            return OutPutJson;
        }

        public ActionResult AppearanceNuo(string OrderNum)
        {
            var barcodelist = db.BarCodeRelation.Where(c => c.NewOrderNum == OrderNum).ToList();
            JArray abnormal = new JArray();
            JArray jarraynomal = new JArray();
            JObject total = new JObject();
            foreach (var item in barcodelist)
            {
                JObject nuojobject = new JObject();
                List<string> oldbarcode = FindOld(item.NewBarCodesNum);
                JArray barcodeStatu = new JArray();
                foreach (var displaybarcode in oldbarcode)
                {
                    JObject barcodeitem = new JObject();
                    barcodeitem.Add("barcode", displaybarcode);
                    var statu = db.Appearance.Where(c => (c.OldBarCodesNum == displaybarcode || c.OldBarCodesNum == null) && c.OrderNum == OrderNum && c.BarCodesNum == item.NewBarCodesNum).ToList();
                    if (statu.Count != 0)
                    {
                        var abnormalist = statu.Where(c => c.OQCCheckBT != null && c.OQCCheckFT != null && c.OQCCheckFinish == false && c.Appearance_OQCCheckAbnormal != null).Select(c => c.Appearance_OQCCheckAbnormal).ToArray();
                        if (abnormalist.Count() != 0)
                        {
                            JObject abnarmalitem = new JObject();
                            abnarmalitem.Add("barcode", displaybarcode);
                            abnarmalitem.Add("abnormal", string.Join(",", abnormalist));
                            abnormal.Add(abnarmalitem);
                        }
                        if (statu.Count(c => c.OQCCheckBT != null && c.OQCCheckFinish == true) != 0)
                        {
                            
                            var burninfinash = db.Burn_in.OrderByDescending(c => c.OQCCheckFT).Where(c => (c.OldBarCodesNum == displaybarcode || c.OldBarCodesNum == null) && c.OrderNum == OrderNum && c.BarCodesNum == item.NewBarCodesNum && c.OQCCheckFinish == true).Select(c => c.OQCCheckFT).FirstOrDefault();
                            var packingBegin = statu.OrderBy(c => c.OQCCheckBT).Select(c => c.OQCCheckBT).FirstOrDefault();

                             if (packingBegin == null && (DateTime.Now - burninfinash).Value.TotalHours >= 72)
                            {
                                barcodeitem.Add("statue", "老化后超过72小时未进入包装");
                            }
                            else
                            {
                                barcodeitem.Add("statue", "已完成");
                            }
                        }
                        else
                        {
                            barcodeitem.Add("statue", "进行中");
                        }
                    }
                    else
                    {
                        barcodeitem.Add("statue", "未开始");
                    }
                    barcodeStatu.Add(barcodeitem);
                }
                oldbarcode.Add(item.NewBarCodesNum);
                JArray infoArray = new JArray();
                oldbarcode.ForEach(c => infoArray.Add(c));
                nuojobject.Add("info", infoArray);
                nuojobject.Add("barcodeInfo", barcodeStatu);
                jarraynomal.Add(nuojobject);
            }
            total.Add("normal", jarraynomal);
            total.Add("abnormal",abnormal);
            return Content(JsonConvert.SerializeObject(total));
        }
        #endregion


        #region --------------------GetOrderList()取出整个OrderMgms的OrderNum订单号列表
        private List<SelectListItem> GetOrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
            var items = new List<SelectListItem>();
            foreach (string order in orders)
            {
                if (order == "2017-TEST-1")
                {
                    continue;
                }
                items.Add(new SelectListItem
                {
                    Text = order,
                    Value = order
                });
            }
            return items;
        }
        #endregion

    }

}
