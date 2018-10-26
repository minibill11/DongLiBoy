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
        {   // 定期执行的方法
            JObject ProductionControlIndex = new JObject();   //创建JSON对象
            //取出数据
            using (var db = new ApplicationDbContext())
            {
                var OrderList_All = (from m in db.OrderMgm select m).OrderBy(c => c.BarCodeCreated).ToList();
                //var OrderList_Finished = from m in OrderList_All where m.CompletedRate == 100 select m;
                List<OrderMgm> OutputOrderList = new List<OrderMgm>();
                List<OrderMgm> ExpectList = new List<OrderMgm>();

                foreach (var item in OrderList_All)
                {
                    if (db.Appearance.Where(c => c.OrderNum == item.OrderNum).Count(c => c.OQCCheckFinish == true) == item.Boxes)  //包装数量＝订单数量
                    {
                        var appearanceLastTime = db.Appearance.Where(c => c.OrderNum == item.OrderNum).ToList().Max(c => c.OQCCheckFT);
                        var sub = (DateTime.Now - Convert.ToDateTime(appearanceLastTime)).Days > 3 ? true : false;
                        if ( sub )
                        {
                            ExpectList.Add(item);
                        }
                    }
                    if (db.Appearance.Count(c => c.OrderNum == item.OrderNum) !=0)
                    {
                        if ((DateTime.Now - Convert.ToDateTime(db.Appearance.Where(c => c.OrderNum == item.OrderNum).ToList().Max(c => c.OQCCheckFT))).Days > 30 )//包装数量<订单数量，包装最后日期是一个月以前的排除在生产管控清单外
                        {
                            ExpectList.Add(item);
                        }
                    }
                }
                OutputOrderList = OrderList_All.Except(ExpectList).ToList();

                var OrderList_UnFinished = from m in OrderList_All where m.CompletedRate != 100 select m;

                int i = 1;
                //foreach (var item in OrderList_UnFinished.ToList())
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

                    var beginttime = db.Assemble.Where(c=>c.OrderNum==item.OrderNum).Min(c => c.PQCCheckBT);//取出订单开始装配生产的PQCCheckBT值
                    var finishtime = db.Appearance.Where(c => c.OrderNum == item.OrderNum).Max(c => c.OQCCheckFT);//取出最后包装记录的OQCCheckFT值

                    var totaltime = finishtime - beginttime;
                    OrderNum.Add("ActualFinishTime", finishtime.ToString());
                    OrderNum.Add("TotalTime", totaltime.ToString());
                    #region-------------------组装部分
                    //-------------------组装部分
                    var AssembleRecord = (from m in db.Assemble where m.OrderNum == item.OrderNum select m).ToList();//查出OrderNum的所有组装记录
                    if (AssembleRecord.Count()>0)
                    {
                        OrderNum.Add("ActualProductionTime", AssembleRecord.Min(c => c.PQCCheckBT).ToString()); 
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
                            OrderNum.Add("Assemble_Pass_Rate",(Assemble_Normal / AssembleRecord.Count() * 100).ToString("F2") + "%");
                        }
                    }
                    else
                    {
                        OrderNum.Add("ActualProductionTime", "未开始");
                        OrderNum.Add("Assemble_Finish_Rate", "--%");
                        OrderNum.Add("Assemble_Pass_Rate", "--%");
                    }
                    #endregion

                    #region--------------------老化部分
                    //--------------------老化部分
                    var Burn_in_Record = (from m in db.Burn_in where m.OrderNum == item.OrderNum select m).ToList();//查出OrderNum的所有老化记录
                    if(Burn_in_Record.Count()>0)
                    {
                        Decimal Burn_in_Normal = Burn_in_Record.Where(m => m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化正常个数
                        //Decimal Burn_in_FirstPass = Burn_in_Record.Where(m => m.OQCCheckFinish == true && m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化工序直通个数
                        Decimal Burn_in_Finish = Burn_in_Record.Count(m => m.OQCCheckFinish == true); //完成老化工序的个数
                        OrderNum.Add("Burn_in_Finish", Convert.ToInt32(Burn_in_Finish));
                        OrderNum.Add("Burn_in_Count", Burn_in_Record.Count());
                        //计算老化完成率、合格率
                        if (Burn_in_Finish == 0)
                        {
                            OrderNum.Add("Burn_in_Finish_Rate", "0%");
                            OrderNum.Add("Burn_in_Pass_Rate", "0%");
                        }
                        else
                        {
                            OrderNum.Add("Burn_in_Finish_Rate", (Burn_in_Finish / item.Boxes*100).ToString("F2") + "%");
                            OrderNum.Add("Burn_in_Pass_Rate", (Burn_in_Finish / Burn_in_Record.Count()*100).ToString("F2") + "%");
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
                    var Calibration_Record = (from m in db.CalibrationRecord where m.OrderNum == item.OrderNum select m).ToList();//查出OrderNum的所有校正记录
                    if(Calibration_Record.Count()>0)
                    {
                        Decimal Calibration_Normal = Calibration_Record.Where(m => m.Normal == true).Count();//校正正常个数
                        OrderNum.Add("Calibration_Finish", Convert.ToInt32(Calibration_Normal));
                        OrderNum.Add("Calibration_Count", Calibration_Record.Count());
                        //计算校正完成率、合格率
                        if (Calibration_Normal == 0)
                        {
                            OrderNum.Add("Calibration_Finish_Rate", "0%");
                            OrderNum.Add("Calibration_Pass_Rate", "0%");
                        }
                        else
                        {
                            OrderNum.Add("Calibration_Finish_Rate", (Calibration_Normal / item.Boxes*100).ToString("F2") + "%");
                            OrderNum.Add("Calibration_Pass_Rate", (Calibration_Normal / Calibration_Record.Count()*100).ToString("F2") + "%");
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
                    if(Appearances_Record.Count()>0)
                    {
                        //Decimal Appearances_Normal = Appearances_Record.Where(m => m.Appearance_OQCCheckAbnormal == "正常").Count();//外观包装正常个数
                        Decimal Appearances_Finish = Appearances_Record.Where(m => m.OQCCheckFinish == true).Count();//外观包装完成个数
                        OrderNum.Add("Appearances_Finish", Convert.ToInt32(Appearances_Finish));
                        OrderNum.Add("Appearances_Count", Appearances_Record.Count());
                        //计算外观包装完成率、合格率
                        if (Appearances_Finish == 0)
                        {
                            OrderNum.Add("Appearances_Finish_Rate", "0%");
                            OrderNum.Add("Appearances_Pass_Rate", "0%");
                        }
                        else
                        {
                            OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / item.Boxes*100).ToString("F2") + "%");
                            OrderNum.Add("Appearances_Pass_Rate", (Appearances_Finish / Appearances_Record.Count()*100).ToString("F2") + "%");
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
                            OrderNum.Add("Appearances_Count", Appearances_Record.Count());
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


                    ProductionControlIndex.Add(i.ToString(), OrderNum);
                    i++;
                }

            }
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
    }
    #endregion
}