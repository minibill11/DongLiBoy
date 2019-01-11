using Microsoft.AspNet.SignalR;
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

        public ProductionControlIndexBroadcaster()
        {
            // 获取所有连接的句柄，方便后面进行消息广播
            ProductionControlIndex_hubContext = GlobalHost.ConnectionManager.GetHubContext<ProductionControlIndex>();
            // Start the broadcast loop
            ProductionControlIndex_broadcastLoop = new Timer(
                ProductionControlIndexBroadcastShape,
                null,
                0,
                5000);
        }

        private void ProductionControlIndexBroadcastShape(object state)
        {

            JObject ProductionControlIndex = QueryExcution();


            #region----------原来方法

            //// 定期执行的方法
            //JObject ProductionControlIndex = new JObject();   //创建JSON对象

            ////取出数据
            //using (var db = new ApplicationDbContext())
            //{


            //    //DateTime pt = new DateTime();
            //    //pt = DateTime.Now;



            //    var OrderList_All = (from m in db.OrderMgm select m).OrderBy(c => c.BarCodeCreated).ToList();
            //    //var OrderList_Finished = from m in OrderList_All where m.CompletedRate == 100 select m;
            //    List<OrderMgm> OutputOrderList = new List<OrderMgm>();
            //    List<OrderMgm> ExpectList = new List<OrderMgm>();

            //    foreach (var item in OrderList_All)
            //    {
            //        if (db.Appearance.Where(c => c.OrderNum == item.OrderNum).Count(c => c.OQCCheckFinish == true) == item.Boxes)  //包装数量＝订单数量
            //        {
            //            var appearanceLastTime = db.Appearance.Where(c => c.OrderNum == item.OrderNum).ToList().Max(c => c.OQCCheckFT);
            //            var sub = (DateTime.Now - Convert.ToDateTime(appearanceLastTime)).Days > 4 ? true : false;
            //            if ( sub )
            //            {
            //                ExpectList.Add(item);
            //            }
            //        }
            //        if (db.Appearance.Count(c => c.OrderNum == item.OrderNum) !=0)
            //        {
            //            if ((DateTime.Now - Convert.ToDateTime(db.Appearance.Where(c => c.OrderNum == item.OrderNum).ToList().Max(c => c.OQCCheckFT))).Days > 30 )//包装数量<订单数量，包装最后日期是一个月以前的排除在生产管控清单外
            //            {
            //                ExpectList.Add(item);
            //            }
            //        }
            //    }
            //    OutputOrderList = OrderList_All.Except(ExpectList).ToList();

            //    var OrderList_UnFinished = from m in OrderList_All where m.CompletedRate != 100 select m;

            //    int i = 1;


            //    //TimeSpan te = DateTime.Now - pt;
            //    //string aa = "";
            //    //pt = DateTime.Now;
            //    //te = new TimeSpan();


            //    //foreach (var item in OrderList_UnFinished.ToList())
            //    foreach (var item in OutputOrderList)
            //    {
            //        //存入JSON对象
            //        var OrderNum = new JObject
            //        {
            //            {"Id",item.ID },
            //            { "OrderNum", item.OrderNum },
            //            { "Quantity", item.Boxes },
            //            { "PlatformType", item.PlatformType },
            //            { "PlanInputTime", item.PlanInputTime.ToString() },
            //            { "PlanCompleteTime", item.PlanCompleteTime.ToString() },
            //        };

            //        DateTime? beginttime = new DateTime();
            //        beginttime = db.Assemble.Where(c=>c.OrderNum==item.OrderNum).Min(c => c.PQCCheckBT);//取出订单开始装配生产的PQCCheckBT值
            //        if (beginttime == null)
            //        {
            //            beginttime = db.Burn_in.Where(c => c.OrderNum == item.OrderNum).Min(c => c.OQCCheckBT);//取出订单开始老化调试的OQCCheckBT值
            //            if(beginttime == null)
            //            {
            //                beginttime = db.CalibrationRecord.Where(c => c.OrderNum == item.OrderNum).Min(c => c.BeginCalibration);//取出订单开始校正的BeginCalibration值
            //                if (beginttime == null)
            //                {
            //                    beginttime = db.Appearance.Where(c => c.OrderNum == item.OrderNum).Min(c => c.OQCCheckBT);//取出订单开始包装电检检查的OQCCheckBT值
            //                }
            //            }
            //        }
            //        if (beginttime!=null)
            //        {
            //            OrderNum.Add("ActualProductionTime", beginttime.ToString());
            //        }
            //        else
            //        {
            //            OrderNum.Add("ActualProductionTime", "未开始");
            //        }
            //        var finishtime = db.Appearance.Where(c => c.OrderNum == item.OrderNum).Max(c => c.OQCCheckFT);//取出最后包装记录的OQCCheckFT值

            //        var totaltime = finishtime - beginttime;
            //        OrderNum.Add("ActualFinishTime", finishtime.ToString());
            //        OrderNum.Add("TotalTime", totaltime.ToString());



            //        //te = DateTime.Now - pt;
            //        //aa = "";
            //        //pt = DateTime.Now;
            //        //te = new TimeSpan();

            //        #region-------------------组装部分
            //        //-------------------组装部分
            //        var AssembleRecord = (from m in db.Assemble where m.OrderNum == item.OrderNum select m).ToList();//查出OrderNum的所有组装记录
            //        if (AssembleRecord.Count()>0)
            //        {
            //            Decimal Assemble_Normal = AssembleRecord.Where(m => m.PQCCheckFinish == true &&m.RepetitionPQCCheck==false).Count();//组装PQC完成个数
            //            OrderNum.Add("Assemble_Finish", Convert.ToInt32(Assemble_Normal));
            //            OrderNum.Add("AssembleRecord_Count", AssembleRecord.Count());
            //            //计算组装完成率、合格率
            //            if (Assemble_Normal == 0)
            //            {
            //                OrderNum.Add("Assemble_Finish_Rate", "0%");
            //                OrderNum.Add("Assemble_Pass_Rate", "0%");
            //            }
            //            else
            //            {
            //                OrderNum.Add("Assemble_Finish_Rate", (Assemble_Normal / item.Boxes * 100).ToString("F2") + "%");
            //                OrderNum.Add("Assemble_Pass_Rate",(Assemble_Normal / AssembleRecord.Count() * 100).ToString("F2") + "%");
            //            }
            //        }
            //        else
            //        {
            //            OrderNum.Add("Assemble_Finish_Rate", "--%");
            //            OrderNum.Add("Assemble_Pass_Rate", "--%");
            //        }

            //        //te= DateTime.Now - pt;
            //        //aa = "";
            //        //pt = DateTime.Now;
            //        //te = new TimeSpan();
            //        #endregion

            //        #region-------------------FQC部分
            //        var FinalQC_Record = db.FinalQC.Where(c => c.OrderNum == item.OrderNum).ToList();
            //        if(FinalQC_Record.Count>0)
            //        {
            //            Decimal FinalQC_Finish = FinalQC_Record.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Count();
            //            OrderNum.Add("FinalQC_Finish",Convert.ToInt32(FinalQC_Finish));  //FQC完成个数
            //            OrderNum.Add("FinalQC_Record_Count", FinalQC_Record.Count);      //FQC记录个数 
            //            OrderNum.Add("FinalQC_Spot_Count", FinalQC_Record.Select(c => c.BarCodesNum).Distinct().Count());      //FQC抽检个数 
            //            if (FinalQC_Finish==0)
            //            {
            //                OrderNum.Add("FinalQC_Finish_Rate", "0%");  //FQC完成率
            //                OrderNum.Add("FinalQC_Pass_Rate", "0%");    //FQC合格率
            //            }
            //            else
            //            {
            //                OrderNum.Add("FinalQC_Finish_Rate", (FinalQC_Finish / FinalQC_Record.Select(c=>c.BarCodesNum).Distinct().Count() * 100).ToString("F2") + "%");  //FQC完成率
            //                OrderNum.Add("FinalQC_Pass_Rate", (FinalQC_Finish / FinalQC_Record.Count() * 100).ToString("F2") + "%");                                        //FQC合格率
            //            }
            //        }
            //        else
            //        {
            //            OrderNum.Add("FinalQC_Finish_Rate", "--%");  //FQC完成率
            //            OrderNum.Add("FinalQC_Pass_Rate", "--%");    //FQC合格率
            //        }

            //        //te = DateTime.Now - pt;
            //        //aa = " ";
            //        //pt = DateTime.Now;
            //        //te = new TimeSpan();
            //        #endregion

            //        #region--------------------老化部分
            //        //--------------------老化部分
            //        var Burn_in_Record = (from m in db.Burn_in where m.OrderNum == item.OrderNum select m).ToList();//查出OrderNum的所有老化记录
            //        int Burn_in_Record_Count = Burn_in_Record.Select(m => m.BarCodesNum).Distinct().Count();
            //        if (Burn_in_Record.Count()>0)
            //        {
            //            Decimal Burn_in_Normal = Burn_in_Record.Where(m => m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化正常个数
            //            //Decimal Burn_in_FirstPass = Burn_in_Record.Where(m => m.OQCCheckFinish == true && m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化工序直通个数
            //            Decimal Burn_in_Finish = Burn_in_Record.Count(m => m.OQCCheckFinish == true); //完成老化工序的个数
            //            OrderNum.Add("Burn_in_Finish", Convert.ToInt32(Burn_in_Finish));
            //            OrderNum.Add("Burn_in_Record_Count", Burn_in_Record_Count);
            //            OrderNum.Add("Burn_in_Count", Burn_in_Record.Count());
            //            //计算老化完成率、合格率
            //            if (Burn_in_Finish == 0)
            //            {
            //                OrderNum.Add("Burn_in_Finish_Rate", "0%");
            //                OrderNum.Add("Burn_in_Pass_Rate", "0%");
            //            }
            //            else
            //            {
            //                OrderNum.Add("Burn_in_Finish_Rate", (Burn_in_Finish / Burn_in_Record_Count * 100).ToString("F2") + "%");
            //                OrderNum.Add("Burn_in_Pass_Rate", (Burn_in_Finish / Burn_in_Record.Count()*100).ToString("F2") + "%");
            //            }
            //        }
            //        else
            //        {
            //            OrderNum.Add("Burn_in_Finish_Rate", "--%");
            //            OrderNum.Add("Burn_in_Pass_Rate", "--%");
            //        }

            //        //te = DateTime.Now - pt;
            //        //aa = " ";
            //        //pt = DateTime.Now;
            //        //te = new TimeSpan();

            //        #endregion

            //        #region---------------------校正部分
            //        //---------------------校正部分
            //        var Calibration_Record = (from m in db.CalibrationRecord where m.OrderNum == item.OrderNum && m.RepetitionCalibration == false select m).ToList();//查出OrderNum的所有校正记录
            //        int Calibration_Record_Count = Calibration_Record.Select(m => m.BarCodesNum).Distinct().Count();
            //        if (Calibration_Record.Count()>0)
            //        {
            //            Decimal Calibration_Normal = Calibration_Record.Where(m => m.Normal == true).Count();//校正正常个数
            //            OrderNum.Add("Calibration_Finish", Convert.ToInt32(Calibration_Normal));
            //            OrderNum.Add("Calibration_Record_Count", Calibration_Record_Count);
            //            OrderNum.Add("Calibration_Count", Calibration_Record.Count());
            //            //计算校正完成率、合格率
            //            if (Calibration_Normal == 0)
            //            {
            //                OrderNum.Add("Calibration_Finish_Rate", "0%");
            //                OrderNum.Add("Calibration_Pass_Rate", "0%");
            //            }
            //            else
            //            {
            //                OrderNum.Add("Calibration_Finish_Rate", (Calibration_Normal / Calibration_Record_Count * 100).ToString("F2") + "%");
            //                OrderNum.Add("Calibration_Pass_Rate", (Calibration_Normal / Calibration_Record.Count()*100).ToString("F2") + "%");
            //            }
            //        }
            //        else
            //        {
            //            OrderNum.Add("Calibration_Finish_Rate", "--%");
            //            OrderNum.Add("Calibration_Pass_Rate", "--%");
            //        }


            //        //te = DateTime.Now - pt;
            //        //aa = " ";
            //        //pt = DateTime.Now;
            //        //te = new TimeSpan();

            //        #endregion

            //        #region---------------------外观包装部分
            //        //---------------------外观包装部分
            //        var Appearances_Record = (from m in db.Appearance where m.OrderNum == item.OrderNum select m).ToList();//查出OrderNum的所有外观包装记录
            //        int Appearances_Record_Count = Appearances_Record.Select(m => m.BarCodesNum).Distinct().Count();
            //        if (Appearances_Record.Count()>0)
            //        {
            //            //Decimal Appearances_Normal = Appearances_Record.Where(m => m.Appearance_OQCCheckAbnormal == "正常").Count();//外观包装正常个数
            //            Decimal Appearances_Finish = Appearances_Record.Where(m => m.OQCCheckFinish == true).Count();//外观包装完成个数
            //            OrderNum.Add("Appearances_Finish", Convert.ToInt32(Appearances_Finish));
            //            OrderNum.Add("Appearances_Record_Count", Appearances_Record.Count());
            //            OrderNum.Add("Appearances_Count", Appearances_Record_Count);
            //            //计算外观包装完成率、合格率
            //            if (Appearances_Finish == 0)
            //            {
            //                OrderNum.Add("Appearances_Finish_Rate", "0%");
            //                OrderNum.Add("Appearances_Pass_Rate", "0%");
            //            }
            //            else
            //            {
            //                OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / Appearances_Record_Count * 100).ToString("F2") + "%");
            //                OrderNum.Add("Appearances_Pass_Rate", (Appearances_Finish / Appearances_Record.Count() * 100).ToString("F2") + "%");
            //            }
            //        }
            //        else  
            //        {
            //            //使用库存出库订单
            //            Appearances_Record = db.Appearance.Where(c => c.ToOrderNum == item.OrderNum).ToList();
            //            if (Appearances_Record.Count() > 0)
            //            {
            //                Decimal Appearances_Finish = Appearances_Record.Where(m => m.OQCCheckFinish == true).Count();//外观包装完成个数
            //                OrderNum.Add("Appearances_Finish", Convert.ToInt32(Appearances_Finish));
            //                OrderNum.Add("Appearances_Record_Count", Appearances_Record.Count());
            //                OrderNum.Add("Appearances_Count", Appearances_Record_Count);
            //                OrderNum.Remove("ActualProductionTime");
            //                OrderNum.Add("ActualProductionTime", Appearances_Record.Min(c => c.OQCCheckBT).ToString()); //取出最早记录的包装OQCCheckBT值
            //                //计算外观包装完成率、合格率
            //                if (Appearances_Finish == 0)
            //                {
            //                    OrderNum.Add("Appearances_Finish_Rate", "0%");
            //                    OrderNum.Add("Appearances_Pass_Rate", "0%");
            //                }
            //                else
            //                {
            //                    OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / Appearances_Record_Count * 100).ToString("F2") + "%");
            //                    OrderNum.Add("Appearances_Pass_Rate", (Appearances_Finish / Appearances_Record.Count() * 100).ToString("F2") + "%");
            //                }


            //            }
            //            else
            //            {
            //                OrderNum.Add("Appearances_Finish_Rate", "--%");
            //                OrderNum.Add("Appearances_Pass_Rate", "--%");
            //            }
            //        }


            //        //te = DateTime.Now - pt;
            //        //aa = " ";
            //        //pt = DateTime.Now;
            //        //te = new TimeSpan();

            //        #endregion

            //        #region---------------------AOD特采部分
            //        var AOD_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().AOD_Description;
            //        OrderNum.Add("AOD_Description", AOD_Description);
            //        #endregion


            //        ProductionControlIndex.Add(i.ToString(), OrderNum);
            //        i++;
            //    }
            //}


            #endregion



            //广播发送JSON数据
            ProductionControlIndex_hubContext.Clients.All.sendProductionControlIndex(ProductionControlIndex);
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
            List<OrderMgm> OrderList_All = (from m in db.OrderMgm select m).OrderBy(c => c.BarCodeCreated).ToList();
            List<OrderMgm> OutputOrderList = new List<OrderMgm>();
            List<OrderMgm> ExpectList = new List<OrderMgm>();
            List<OrderMgm> ExpectList2 = new List<OrderMgm>();

            foreach (var item in OrderList_All)
            {
                List<Appearance> ordernum_record_list = db.Appearance.Where(c => c.OrderNum == item.OrderNum).ToList();

                if (ordernum_record_list.Count(c => c.OQCCheckFinish == true) == item.Boxes)  //包装数量＝订单数量
                {
                    var appearanceLastTime = ordernum_record_list.Max(c => c.OQCCheckFT);
                    var sub = (DateTime.Now - Convert.ToDateTime(appearanceLastTime)).TotalHours > 24 ? true : false;
                    if (sub)
                    {
                        ExpectList.Add(item);
                    }
                    else
                    {
                        ExpectList2.Add(item);
                    }
                }
                if (ordernum_record_list.Count != 0)
                {
                    if ((DateTime.Now - Convert.ToDateTime(ordernum_record_list.Max(c => c.OQCCheckFT))).Days > 30)//包装数量<订单数量，包装最后日期是一个月以前的排除在生产管控清单外
                    {
                        ExpectList.Add(item);
                    }
                }
            }

            ////只保留3个完成的订单显示在生产管控表中，算出需要排除的订单加入ExpectList中
            //if(ExpectList2.Count>3)
            //{
            //    List<Appearance> appearacnelist = new List<Appearance>();
            //    foreach (var item in ExpectList2)
            //    {
            //        appearacnelist.AddRange(db.Appearance.Where(c => c.OrderNum == item.OrderNum && c.OQCCheckFinish==true).ToList());
            //    }
            //    var orderlist = appearacnelist.OrderBy(c => c.OQCCheckFT).Select(c=>c.OrderNum).Distinct().ToList();
            //    for(int j = ExpectList2.Count; j > 3; j--)
            //    {
            //
            //    }
            //}

            //把2017-TEST-1放入排除清单中
            ExpectList.Add(OrderList_All.Where(c=>c.OrderNum=="2017-TEST-1").FirstOrDefault());
            //----------------------------------------------------------------------------------

            OutputOrderList = OrderList_All.Except(ExpectList).ToList();
            //var OrderList_UnFinished = from m in OrderList_All where m.CompletedRate != 100 select m;

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
                beginttime = db.Assemble.Where(c => c.OrderNum == item.OrderNum).Min(c => c.PQCCheckBT);//取出订单开始装配生产的PQCCheckBT值
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
                if (db.Appearance.Count(c => c.OrderNum == item.OrderNum && c.OQCCheckFinish == true) == modelGroupQuantity)
                {
                    finishtime = db.Appearance.Where(c => c.OrderNum == item.OrderNum).Max(c => c.OQCCheckFT); 
                }
                else
                {
                    finishtime = null;
                }
                var totaltime = finishtime==null?"": (finishtime - beginttime).ToString();
                OrderNum.Add("ActualFinishTime", finishtime.ToString());
                OrderNum.Add("TotalTime", totaltime);

                #region-------------------组装部分
                //-------------------组装部分
                var AssembleRecord = (from m in db.Assemble where m.OrderNum == item.OrderNum select m).ToList();//查出OrderNum的所有组装记录
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
                var FinalQC_Record = db.FinalQC.Where(c => c.OrderNum == item.OrderNum).ToList();
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
                        OrderNum.Add("FinalQC_Finish_Rate", (FinalQC_Finish / item.Boxes * 100).ToString("F2") + "%");  //FQC比例
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
                var Burn_in_Record = (from m in db.Burn_in where m.OrderNum == item.OrderNum select m).ToList();//查出OrderNum的所有老化记录
                int Burn_in_Record_Count = Burn_in_Record.Select(m => m.BarCodesNum).Distinct().Count();
                if (Burn_in_Record.Count() > 0)
                {
                    Decimal Burn_in_Normal = Burn_in_Record.Where(m => m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化正常个数
                                                                                                                   //Decimal Burn_in_FirstPass = Burn_in_Record.Where(m => m.OQCCheckFinish == true && m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化工序直通个数
                    Decimal Burn_in_Finish = Burn_in_Record.Count(m => m.OQCCheckFinish == true); //完成老化工序的个数
                    OrderNum.Add("Burn_in_Finish", Convert.ToInt32(Burn_in_Finish));
                    OrderNum.Add("Burn_in_Record_Count", Burn_in_Record_Count);
                    OrderNum.Add("Burn_in_Count", Burn_in_Record.Count());
                    OrderNum.Add("Burn_in_Doing_Count", Burn_in_Record.Where(c=>c.OQCCheckBT!=null && c.OQCCheckFT==null).Count());//正在老化个数
                    //计算老化完成率、合格率
                    if (Burn_in_Finish == 0)
                    {
                        OrderNum.Add("Burn_in_Finish_Rate", "0%");
                        OrderNum.Add("Burn_in_Pass_Rate", "0%");
                    }
                    else
                    {
                        //OrderNum.Add("Burn_in_Finish_Rate", (Burn_in_Finish / Burn_in_Record_Count * 100).ToString("F2") + "%");
                        OrderNum.Add("Burn_in_Finish_Rate", (Burn_in_Finish / item.Boxes * 100).ToString("F2") + "%");
                        OrderNum.Add("Burn_in_Pass_Rate", (Burn_in_Finish / Burn_in_Record.Count() * 100).ToString("F2") + "%");
                    }
                }
                else
                {
                    OrderNum.Add("Burn_in_Finish_Rate", "--%");
                    OrderNum.Add("Burn_in_Pass_Rate", "--%");
                }
                #endregion

                #region---------------------校正部分
                //---------------------校正部分
                var Calibration_Record = (from m in db.CalibrationRecord where m.OrderNum == item.OrderNum && m.RepetitionCalibration == false select m).ToList();//查出OrderNum的所有校正记录
                int Calibration_Record_Count = Calibration_Record.Select(m => m.BarCodesNum).Distinct().Count();
                if (Calibration_Record.Count() > 0)
                {
                    Decimal Calibration_Normal = Calibration_Record.Where(m => m.Normal == true).Count();//校正正常个数
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
                var Appearances_Record = (from m in db.Appearance where m.OrderNum == item.OrderNum select m).ToList();//查出OrderNum的所有外观包装记录
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
                        OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / item.Boxes * 100).ToString("F2") + "%");
                        OrderNum.Add("Appearances_Pass_Rate", (Appearances_Finish / Appearances_Record.Count() * 100).ToString("F2") + "%");
                    }
                }
                else
                {
                    //使用库存出库订单
                    Appearances_Record = db.Appearance.Where(c => c.ToOrderNum == item.OrderNum).ToList();
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
                            OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / Appearances_Record_Count * 100).ToString("F2") + "%");
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

                #region---------------------AOD特采部分
                var AOD_Description = db.OrderMgm.Where(c => c.OrderNum == item.OrderNum).ToList().FirstOrDefault().AOD_Description;
                OrderNum.Add("AOD_Description", AOD_Description);
                #endregion


                ProductionControlIndex.Add(i.ToString(), OrderNum);
                i++;
            }
            return ProductionControlIndex;
        }

        #endregion

    }

    #endregion
}