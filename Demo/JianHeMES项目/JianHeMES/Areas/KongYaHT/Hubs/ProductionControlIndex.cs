﻿using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JianHeMES.Models;
using System.IO;
using JianHeMES.Controllers;
//using JianHeMESEntities.Areas.KongYaHT.Models;

namespace JianHeMESEntities.Hubs
{
    //ProductionControlIndex生产监控首页
    #region --------------------------------ProductionControlIndex生产监控首页-------------------------------------

    [HubName("ProductionControlIndex")]
    public class ProductionControlIndex : Hub
    {
        // Is set via the constructor on each creation
        private ProductionControlIndexBroadcaster ProductionControlIndexBroadcaster_broadcaster;
        public ProductionControlIndex()
            : this(ProductionControlIndexBroadcaster.ProductionControlIndexInstance)
        {
        }
        public ProductionControlIndex(ProductionControlIndexBroadcaster ProductionControlIndexbroadcaster)
        {
            ProductionControlIndexBroadcaster_broadcaster = ProductionControlIndexbroadcaster;
        }
    }

    /// <summary>
    /// 数据广播器
    /// </summary>
    public class ProductionControlIndexBroadcaster
    {
        private readonly static Lazy<ProductionControlIndexBroadcaster> ProductionControlIndex_instance =
            new Lazy<ProductionControlIndexBroadcaster>(() => new ProductionControlIndexBroadcaster());

        private readonly IHubContext ProductionControlIndex_hubContext;

        private Timer ProductionControlIndex_broadcastLoop;

        private static bool aa = false;

        public ProductionControlIndexBroadcaster()
        {
            // 获取所有连接的句柄，方便后面进行消息广播
            ProductionControlIndex_hubContext = GlobalHost.ConnectionManager.GetHubContext<ProductionControlIndex>();
            // Start the broadcast loop
            ProductionControlIndex_broadcastLoop = new Timer(
                ProductionControlIndexBroadcastShape,
                null,
                0,
                60000);
        }

        private void ProductionControlIndexBroadcastShape(object state)
        {
            JObject ProductionControlIndex1 = QueryExcution();

            //if (!aa)
            //{
            //    JObject ProductionControlIndex = GetNewInfo();
            //    ProductionControlIndex_hubContext.Clients.All.sendProductionControlIndex(ProductionControlIndex);
            //}
            //else
            //{
            ProductionControlIndex_hubContext.Clients.All.sendProductionControlIndex(ProductionControlIndex1);
            // }
            //广播发送JSON数据

        }

        public static ProductionControlIndexBroadcaster ProductionControlIndexInstance
        {
            get
            {
                return ProductionControlIndex_instance.Value;
            }
        }

        #region-----------查询数据方法

        public JObject QueryExcution()
        {

            JObject ProductionControlIndex = new JObject();   //创建JSON对象

            //取出数据
            ApplicationDbContext db = new ApplicationDbContext();
            CommonalityController comm = new CommonalityController();
            List<OrderMgm> OrderList_All = (from m in db.OrderMgm select m).ToList();//.OrderBy(c => c.BarCodeCreated)
            List<OrderMgm> OutputOrderList = new List<OrderMgm>();
            List<OrderMgm> ExpectList = new List<OrderMgm>();
            List<OrderMgm> ExpectList2 = new List<OrderMgm>();

            foreach (var item in OrderList_All)
            {
                List<Appearance> ordernum_appearance_list = db.Appearance.Where(c => c.OrderNum == item.OrderNum && c.OQCCheckFT != null&&c.OQCCheckFinish==true).ToList();
                //List<Warehouse_Join> ordernum_record_list = db.Warehouse_Join.Where(c => c.OrderNum == item.OrderNum&&c.IsOut==true).ToList();
                //模组数为0的托架产品
                if(item.Boxes==0)
                {
                    var packing_print_info = db.Packing_BarCodePrinting.Where(c=>c.OrderNum==item.OrderNum).ToList();
                    if(packing_print_info.Count()>0)
                    {
                        var warehouseout = db.Warehouse_Join.Where(c => c.OrderNum == item.OrderNum && c.IsOut == true).ToList();
                        if (warehouseout.Count == packing_print_info.Count)
                        {
                            var warehouseroutLastTime = warehouseout.Max(c => c.WarehouseOutDate);
                            var sub2 = (DateTime.Now - Convert.ToDateTime(warehouseroutLastTime)).TotalHours > 24 ? true : false;
                            if (sub2)
                            {
                                ExpectList.Add(item);
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                if (ordernum_appearance_list.Count() == item.Boxes)//电检全部完成
                {
                    var appearanceLastTime = ordernum_appearance_list.Max(c => c.OQCCheckFT);
                    var sub = (DateTime.Now - Convert.ToDateTime(appearanceLastTime)).TotalHours > 24 ? true : false;  // 包装电检全部完成并超24小时
                    if (sub)
                    {
                        var packing_basci_list = db.Packing_BasicInfo.Where(c => c.OrderNum == item.OrderNum).ToList();//确认是否有包装基本信息
                        //如果有包装信息，直接检查出库时间是否>24小时，如果>24小时，添加入排除清单
                        if (packing_basci_list.Count != 0 )
                        {
                            var warehouseout = db.Warehouse_Join.Where(c => c.OrderNum == item.OrderNum && c.IsOut == true).ToList();
                            if (warehouseout.Count() == item.Boxes)
                            {
                                var warehouseroutLastTime = warehouseout.Max(c => c.WarehouseOutDate);
                                var sub2 = (DateTime.Now - Convert.ToDateTime(warehouseroutLastTime)).TotalHours > 24 ? true : false;
                                if (sub2)
                                {
                                    ExpectList.Add(item);
                                }
                            }
                        }
                        //如果没有包装信息，直接添加入排除清单
                        else
                        {
                            ExpectList.Add(item);
                        }
                    }
                  
                }
                if (ordernum_appearance_list.Count != 0)
                {
                    if ((DateTime.Now - Convert.ToDateTime(ordernum_appearance_list.Max(c => c.OQCCheckFT))).Days > 30)//包装数量<订单数量，包装最后日期是一个月以前的排除在生产管控清单外
                    {
                        ExpectList.Add(item);
                    }
                }
                //    if (ordernum_record_list.Count() == item.Boxes)  //包装数量＝订单数量
                //    {
                //        var appearanceLastTime = ordernum_record_list.Max(c => c.WarehouseOutDate);
                //        var sub = (DateTime.Now - Convert.ToDateTime(appearanceLastTime)).TotalHours > 24 ? true : false;
                //        if (sub)
                //        {
                //            ExpectList.Add(item);
                //        }
                //        else
                //        {
                //            ExpectList2.Add(item);
                //        }
                //    }
                //    if (ordernum_record_list.Count != 0)
                //    {
                //        if ((DateTime.Now - Convert.ToDateTime(ordernum_record_list.Max(c => c.WarehouseOutDate))).Days > 30)//包装数量<订单数量，包装最后日期是一个月以前的排除在生产管控清单外
                //        {
                //            ExpectList.Add(item);
                //        }
                //    }
            }

            //把2017-TEST-1放入排除清单中
            ExpectList.Add(OrderList_All.Where(c => c.OrderNum == "2017-TEST-1").FirstOrDefault());
            //----------------------------------------------------------------------------------

            OutputOrderList = OrderList_All.Except(ExpectList).ToList();

            int i = 1;
            foreach (var item in OutputOrderList)
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

                DateTime? beginttime = new DateTime();
                beginttime = db.FinalQC.Where(c => c.OrderNum == item.OrderNum).Min(c => c.FQCCheckBT);//取出订单开始装配生产的PQCCheckBT值
                if (beginttime == null)
                {
                    beginttime = db.Burn_in.Where(c => c.OrderNum == item.OrderNum).Min(c => c.OQCCheckBT);//取出订单开始老化调试的OQCCheckBT值
                    if (beginttime == null)
                    {
                        beginttime = db.CalibrationRecord.Where(c => c.OrderNum == item.OrderNum).Min(c => c.BeginCalibration);//取出订单开始校正的BeginCalibration值
                        if (beginttime == null)
                        {
                            beginttime = db.Appearance.Where(c => c.OrderNum == item.OrderNum).Min(c => c.OQCCheckBT);//取出订单开始包装电检检查的OQCCheckBT值
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

                int modelGroupQuantity = OrderList_All.Where(c => c.OrderNum == item.OrderNum).FirstOrDefault().Boxes;//订单模组数量
                                                                                                                      //完成时间
                DateTime? finishtime = new DateTime();
                if (db.Warehouse_Join.Count(c => c.OrderNum == item.OrderNum && c.IsOut == true) == modelGroupQuantity)
                {
                    finishtime = db.Warehouse_Join.Where(c=>c.OrderNum == item.OrderNum && c.IsOut == true).Max(c => c.WarehouseOutDate);
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
                if (AssembleRecord.Count() > 0)
                {
                    Decimal Assemble_Normal = AssembleRecord.Where(m => m.PQCCheckFinish == true && m.RepetitionPQCCheck == false).Count();//组装PQC完成个数
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
                        OrderNum.Add("Assemble_Finish_Rate", item.Boxes==0?"--":(Assemble_Normal / item.Boxes * 100).ToString("F2") + "%");
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
                if (FinalQC_Record.Count > 0)
                {
                    Decimal FinalQC_Finish = FinalQC_Record.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Count();
                    OrderNum.Add("FinalQC_Finish", Convert.ToInt32(FinalQC_Finish));  //FQC完成个数
                    OrderNum.Add("FinalQC_Record_Count", FinalQC_Record.Count);      //FQC记录个数 
                    OrderNum.Add("FinalQC_Spot_Count", FinalQC_Record.Select(c => c.BarCodesNum).Distinct().Count());      //FQC抽检个数 
                    if (FinalQC_Finish == 0)
                    {
                        OrderNum.Add("FinalQC_Finish_Rate", "0%");  //FQC完成率
                        OrderNum.Add("FinalQC_Pass_Rate", "0%");    //FQC合格率
                    }
                    else
                    {
                        //OrderNum.Add("FinalQC_Finish_Rate", (FinalQC_Finish / FinalQC_Record.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2") + "%");  //FQC完成率
                        OrderNum.Add("FinalQC_Finish_Rate", item.Boxes == 0 ? "--" : (FinalQC_Finish * 100 / item.Boxes).ToString("F2") + "%");  //FQC比例
                        OrderNum.Add("FinalQC_Pass_Rate", (FinalQC_Finish / FinalQC_Record.Count() * 100).ToString("F2") + "%");                                        //FQC合格率
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
                int Burn_in_Record_Count = Burn_in_Record.Select(m => m.BarCodesNum).Distinct().Count();
                if (Burn_in_Record.Count() > 0)
                {
                    Decimal Burn_in_Normal = Burn_in_Record.Where(m => m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化正常个数
                                                                                                                   //Decimal Burn_in_FirstPass = Burn_in_Record.Where(m => m.OQCCheckFinish == true && m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化工序直通个数
                    Decimal Burn_in_Finish = Burn_in_Record.Count(m => m.OQCCheckFinish == true); //完成老化工序的个数
                    OrderNum.Add("Burn_in_Finish", Convert.ToInt32(Burn_in_Finish));
                    OrderNum.Add("Burn_in_Record_Count", Burn_in_Record_Count);
                    OrderNum.Add("Burn_in_Count", Burn_in_Record.Count());
                    OrderNum.Add("Burn_in_Doing_Count", Burn_in_Record.Where(c => c.OQCCheckBT != null && c.OQCCheckFT == null).Count());//正在老化个数
                    OrderNum.Add("Burn_in_Finish_Rate", item.Boxes == 0 ? "--" : (Convert.ToDecimal(Burn_in_Record.Count()) / item.Boxes * 100).ToString("F2") + "%");//计算老化完成率、合格率
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
                var Calibration_Record = (from m in db.CalibrationRecord where m.OrderNum == item.OrderNum && m.RepetitionCalibration == false && (m.OldOrderNum == null || m.OldOrderNum == item.OrderNum) select m).ToList();//查出OrderNum的所有校正记录
                int Calibration_Record_Count = Calibration_Record.Select(m => m.BarCodesNum).Count();
                if (Calibration_Record.Count() > 0)
                {
                    //Decimal Calibration_Normal = Calibration_Record.Where(m => m.Normal == true).Count();//校正正常个数
                    Decimal Calibration_Normal = Calibration_Record.Where(m => m.Normal == true).Select(m => m.BarCodesNum).Distinct().Count();//校正正常个数
                    OrderNum.Add("Calibration_Finish", Convert.ToInt32(Calibration_Normal));
                    OrderNum.Add("Calibration_Record_Count", Calibration_Record_Count);
                    OrderNum.Add("Calibration_Count", Calibration_Record.Count());
                    //计算校正完成率、合格率
                    if (Calibration_Normal == 0)
                    {
                        OrderNum.Add("Calibration_Finish_Rate", "0%");
                        OrderNum.Add("Calibration_Pass_Rate", "0%");
                    }
                    else
                    {
                        OrderNum.Add("Calibration_Finish_Rate", item.Boxes == 0 ? "--" : (Calibration_Normal / item.Boxes * 100).ToString("F2") + "%");
                        OrderNum.Add("Calibration_Pass_Rate", (Calibration_Normal / Calibration_Record_Count * 100).ToString("F2") + "%");
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
                var Appearances_Record = (from m in db.Appearance where m.OrderNum == item.OrderNum && (m.OldOrderNum == null || m.OldOrderNum == item.OrderNum) select m).ToList();//查出OrderNum的所有外观包装记录
                int Appearances_Record_Count = Appearances_Record.Select(m => m.BarCodesNum).Distinct().Count();
                if (Appearances_Record.Count() > 0)
                {
                    //Decimal Appearances_Normal = Appearances_Record.Where(m => m.Appearance_OQCCheckAbnormal == "正常").Count();//外观包装正常个数
                    Decimal Appearances_Finish = Appearances_Record.Where(m => m.OQCCheckFinish == true).Count();//外观包装完成个数
                    OrderNum.Add("Appearances_Finish", Convert.ToInt32(Appearances_Finish));
                    OrderNum.Add("Appearances_Record_Count", Appearances_Record.Count());
                    OrderNum.Add("Appearances_Count", Appearances_Record_Count);
                    //计算外观包装完成率、合格率
                    if (Appearances_Finish == 0)
                    {
                        OrderNum.Add("Appearances_Finish_Rate", "0%");
                        OrderNum.Add("Appearances_Pass_Rate", "0%");
                    }
                    else
                    {
                        //OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / Appearances_Record_Count * 100).ToString("F2") + "%");
                        OrderNum.Add("Appearances_Finish_Rate", item.Boxes == 0 ? "--" : (Appearances_Finish / item.Boxes * 100).ToString("F2") + "%");
                        OrderNum.Add("Appearances_Pass_Rate", (Appearances_Finish / Appearances_Record.Count() * 100).ToString("F2") + "%");
                    }
                }
                else
                {
                    //使用库存出库订单
                    Appearances_Record = db.Appearance.Where(c => c.ToOrderNum == item.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == item.OrderNum)).ToList();
                    if (Appearances_Record.Count() > 0)
                    {
                        Decimal Appearances_Finish = Appearances_Record.Where(m => m.OQCCheckFinish == true).Count();//外观包装完成个数
                        OrderNum.Add("Appearances_Finish", Convert.ToInt32(Appearances_Finish));
                        OrderNum.Add("Appearances_Record_Count", Appearances_Record.Count());
                        OrderNum.Add("Appearances_Count", Appearances_Record_Count);
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
                            OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / item.Boxes * 100).ToString("F2") + "%");
                            OrderNum.Add("Appearances_Pass_Rate", (Appearances_Finish / Appearances_Record.Count() * 100).ToString("F2") + "%");
                        }
                    }
                    else
                    {
                        OrderNum.Add("Appearances_Finish_Rate", "--%");
                        OrderNum.Add("Appearances_Pass_Rate", "--%");
                    }
                }
                #endregion

                #region---------------------PQC和FQC超24小时未进入老化工序和老化后72小时未进入包装工序提示信息部分
                //此条码PQC和FQC完成，到现在为止超过24小时未有老化记录，输出提示；
                //此条码PQC和FQC完成，老化的开始时间－PQC和FQC完成时间超24小时，输出提示；
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
                                TimeSpan overtime_never_join_burn_in = DateTime.Now - assembleFinishTime;
                                if (overtime_never_join_burn_in.TotalHours >= 24)
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

                #region SMT首样
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
                var joincount = db.Warehouse_Join.Where(c => c.OrderNum == item.OrderNum).Select(c => c.OuterBoxBarcode).Distinct().Count();
                var outcount = db.Warehouse_Join.Where(c => c.OrderNum == item.OrderNum && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();
                OrderNum.Add("joinAndOutDepot", (joincount==0?"--":joincount.ToString()) + "/" + (outcount == 0 ? "--":outcount.ToString()));
                #endregion

                #region 之前的
                //foreach (var jobconten in jobcontenlist)
                //{
                //    if (jobconten.Contains("IC面"))
                //    {
                //        ModelNum = item.Models;
                //        //对应工作内容良品总数
                //        ICNormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "IC面").Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "IC面").Sum(c => c.NormalCount);

                //        //对应工作内容不良品总数
                //        ICAbnormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "IC面").Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "IC面").Sum(c => c.AbnormalCount);

                //    }
                //    if (jobconten.Contains("灯面"))
                //    {
                //        ModelNum = item.Models;
                //        //对应工作内容良品总数
                //        lampNormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "灯面").Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "灯面").Sum(c => c.NormalCount);

                //        //对应工作内容不良品总数
                //        lampAbnormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "灯面").Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "灯面").Sum(c => c.AbnormalCount);

                //    }
                //    if (jobconten.Contains("转接卡"))
                //    {
                //        adapterNum = item.AdapterCard;
                //        //对应工作内容良品总数
                //        adapterNormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent.Contains("转接卡")).Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent.Contains("转接卡")).Sum(c => c.NormalCount);

                //        //对应工作内容不良品总数
                //        adapterAbnormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent.Contains("转接卡")).Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent.Contains("转接卡")).Sum(c => c.AbnormalCount);
                //    }
                //    if (jobconten.Contains("电源"))
                //    {
                //        powerNum = item.Powers;
                //        //对应工作内容良品总数
                //        powerNormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "电源").Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "电源").Sum(c => c.NormalCount);

                //        //对应工作内容不良品总数
                //        powerAbnormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "电源").Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "电源").Sum(c => c.AbnormalCount);

                //    }
                //}
                //#region smt 总完成率

                ////IC面总完成率分子
                //OrderNum.Add("SMTOrderICFinishRateMolecule", ICNormalNumSum);
                ////IC面总完成率分母
                //OrderNum.Add("SMTOrderICFinishRateDenominator", ModelNum);
                ////IC面总完成率
                //OrderNum.Add("SMTOrderICFinishRate", ModelNum == 0 ? "" : (((decimal)(ICNormalNumSum + ICAbnormalNumSum)) / ModelNum * 100).ToString("F2")+"%");

                ////灯面总完成率分子
                //OrderNum.Add("SMTOrderLampFinishRateMolecule", lampNormalNumSum);
                ////灯面总完成率分母
                //OrderNum.Add("SMTOrderLampFinishRateDenominator", ModelNum);
                //// 灯面总完成率
                //OrderNum.Add("SMTOrderLampFinishRate", ModelNum == 0 ? "" : (((decimal)(lampNormalNumSum + lampAbnormalNumSum)) / ModelNum * 100).ToString("F2") + "%");

                //#region 总完成率扩展框的内容
                ////IC面总完成率
                //extendFinishRate.Add("ICFinishRate", ModelNum == 0 ? "" : (((decimal)(ICNormalNumSum + ICAbnormalNumSum)) / ModelNum * 100).ToString("F2") + "%");
                //// 灯面总完成率
                //extendFinishRate.Add("LampFinishRate", ModelNum == 0 ? "" : (((decimal)(lampNormalNumSum + lampAbnormalNumSum)) / ModelNum * 100).ToString("F2") + "%");
                ////转接卡总完成率
                //extendFinishRate.Add("AdapterCardFinishRate", adapterNum == 0 ? "" : (((decimal)(adapterNormalNumSum + adapterAbnormalNumSum)) / adapterNum * 100).ToString("F2") + "%");
                ////电源总完成率
                //extendFinishRate.Add("PowersFinishRate", powerNum == 0 ? "" : (((decimal)(powerNormalNumSum + powerAbnormalNumSum)) / powerNum * 100).ToString("F2") + "%");
                //#endregion

                ////扩展框
                //OrderNum.Add("ExtendFinishRate", extendFinishRate);

                //#endregion

                //#region SMT合格率
                ////IC面总完成率分子
                //OrderNum.Add("SMTOrderICPassRateMolecule", ICNormalNumSum);
                ////IC面总完成率分母
                //OrderNum.Add("SMTOrderICPassRateDenominator", ICNormalNumSum + ICAbnormalNumSum);
                ////IC面总合格率
                //OrderNum.Add("SMTOrderICPassRate", (ICAbnormalNumSum + ICNormalNumSum) == 0 ? "" : ((decimal)ICNormalNumSum / (ICNormalNumSum + ICAbnormalNumSum) * 100).ToString("F2") + "%");

                ////灯面总完成率分子
                //OrderNum.Add("SMTOrderLampPassRateMolecule", lampNormalNumSum);
                ////灯面总完成率分母
                //OrderNum.Add("SMTOrderLampPassRateDenominator", lampNormalNumSum + lampAbnormalNumSum);
                ////灯面总合格率
                //OrderNum.Add("SMTOrderLampPassRate", (lampAbnormalNumSum + lampNormalNumSum) == 0 ? "" : ((decimal)lampNormalNumSum / (lampNormalNumSum + lampAbnormalNumSum) * 100).ToString("F2") + "%");

                //#region 总合格率扩展框的内容
                ////IC面总合格率
                //extendPassRate.Add("ICPassRate", (ICAbnormalNumSum + ICNormalNumSum) == 0 ? "" : ((decimal)ICNormalNumSum / (ICNormalNumSum + ICAbnormalNumSum) * 100).ToString("F2") + "%");
                //// 灯面总合格率
                //extendPassRate.Add("LampPassRate", (lampAbnormalNumSum + lampNormalNumSum) == 0 ? "" : ((decimal)lampNormalNumSum / (lampNormalNumSum + lampAbnormalNumSum) * 100).ToString("F2") + "%");
                ////转接卡总合格率
                //extendPassRate.Add("AdapterCardPassRate", (adapterAbnormalNumSum + adapterNormalNumSum) == 0 ? "" : ((decimal)adapterNormalNumSum / (adapterNormalNumSum + adapterAbnormalNumSum) * 100).ToString("F2") + "%");
                ////电源总合格率
                //extendPassRate.Add("PowersPassRate", (powerAbnormalNumSum + powerNormalNumSum) == 0 ? "" : ((decimal)powerNormalNumSum / (powerNormalNumSum + powerAbnormalNumSum) * 100).ToString("F2") + "%");
                //#endregion
                ////扩展框
                //OrderNum.Add("ExtendPassRate", extendPassRate);


                #endregion
                ProductionControlIndex.Add(i.ToString(), OrderNum);
                i++;
            }

            if (Directory.Exists(@"D:\MES_Data\TemDate\") == false)//如果不存在就创建订单文件夹
            {
                Directory.CreateDirectory(@"D:\MES_Data\TemDate\");
            }

            string output2 = Newtonsoft.Json.JsonConvert.SerializeObject(ProductionControlIndex, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\ProductionController.json", output2);

            return ProductionControlIndex;


        }


        #endregion

    }

    #endregion
}