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
            //三楼温湿度数据
            JObject ProductionControlIndex = new JObject();   //创建JSON对象
            //取出数据
            using (var db = new ApplicationDbContext())
            {
                var OrderList_All = (from m in db.OrderMgm select m).OrderBy(c => c.BarCodeCreated);
                var OrderList_Finished = from m in OrderList_All where m.CompletedRate == 100 select m;
                var OrderList_UnFinished = from m in OrderList_All where m.CompletedRate != 100 select m;

                int i = 1;
                foreach (var item in OrderList_UnFinished.ToList())
                {
                    //存入JSON对象
                    var OrderNum = new JObject
                    {
                        { "OrderNum", item.OrderNum },
                        { "Quantity", item.Boxes+"个" },
                        { "PlatformType", item.PlatformType },
                        { "PlanInputTime", item.PlanInputTime.ToString() },
                        { "PlanCompleteTime", item.PlanCompleteTime.ToString() },
                };

                    //-------------------组装部分
                    var AssembleRecord = from m in db.Assemble where m.OrderNum == item.OrderNum select m;//查出OrderNum的所有组装记录
                    if (AssembleRecord!=null)
                    {
                        if (AssembleRecord.OrderBy(c => c.PQCCheckBT).Count() == 0)
                        {
                            OrderNum.Add("ActualProductionTime", "");//按PQCCheckBT顺序排序，选择第一个记录的PQCCheckBT值
                        }
                        else
                        {
                            OrderNum.Add("ActualProductionTime", AssembleRecord.OrderBy(c => c.PQCCheckBT).FirstOrDefault().PQCCheckBT.ToString());//按PQCCheckBT顺序排序，选择第一个记录的PQCCheckBT值
                        }
                        Decimal Assemble_Normal = AssembleRecord.Where(m => m.PQCCheckAbnormal == "正常").Count();//组装PQC正常个数
                        //计算组装完成率、合格率
                        if (Assemble_Normal == 0)
                        {
                            OrderNum.Add("Assemble_Finish_Rate", "--%");
                            OrderNum.Add("Assemble_Pass_Rate", "--%");
                        }
                        else
                        {
                            OrderNum.Add("Assemble_Finish_Rate", (Assemble_Normal / item.Boxes * 100).ToString("F2") + "%");
                            OrderNum.Add("Assemble_Pass_Rate",(Assemble_Normal / AssembleRecord.Count() * 100).ToString("F2") + "%");
                        }
                    }
                    else
                    {
                        OrderNum.Add("ActualProductionTime", "未开始生产");
                        OrderNum.Add("Assemble_Finish_Rate", "--");
                        OrderNum.Add("Assemble_Pass_Rate", "--");
                    }

                    //--------------------老化部分
                    var Burn_in_Record = from m in db.Burn_in where m.OrderNum == item.OrderNum select m;//查出OrderNum的所有老化记录
                    if(Burn_in_Record!=null)
                    {
                        Decimal Burn_in_Normal = Burn_in_Record.Where(m => m.Burn_in_OQCCheckAbnormal == "正常").Count();//老化正常个数
                        //计算老化完成率、合格率
                        if (Burn_in_Normal == 0)
                        {
                            OrderNum.Add("Burn_in_Finish_Rate", "--%");
                            OrderNum.Add("Burn_in_Pass_Rate", "--%");
                        }
                        else
                        {
                            OrderNum.Add("Burn_in_Finish_Rate", (Burn_in_Normal / item.Boxes*100).ToString("F2"));
                            OrderNum.Add("Burn_in_Pass_Rate", (Burn_in_Normal / Burn_in_Record.Count()*100).ToString("F2") + "%");
                        }
                    }
                    else
                    {
                        OrderNum.Add("Burn_in_Finish_Rate", "--%");
                        OrderNum.Add("Burn_in_Pass_Rate", "--%");
                    }

                    //---------------------校正部分
                    var Calibration_Record = from m in db.CalibrationRecord where m.OrderNum == item.OrderNum select m;//查出OrderNum的所有校正记录
                    if(Calibration_Record!=null)
                    {
                        Decimal Calibration_Normal = Calibration_Record.Where(m => m.Normal == true).Count();//校正正常个数
                        //计算校正完成率、合格率
                        if (Calibration_Normal == 0)
                        {
                            OrderNum.Add("Calibration_Finish_Rate", "--%");
                            OrderNum.Add("Calibration_Pass_Rate", "--%");
                        }
                        else
                        {
                            OrderNum.Add("Calibration_Finish_Rate", (Calibration_Normal / item.Boxes*100).ToString("F2") );
                            OrderNum.Add("Calibration_Pass_Rate", (Calibration_Normal / Calibration_Record.Count()*100).ToString("F2") + "%");
                        }
                    }
                    else
                    {
                        OrderNum.Add("Calibration_Finish_Rate", "--%");
                        OrderNum.Add("Calibration_Pass_Rate", "--%");
                    }

                    //---------------------外观包装部分
                    var Appearances_Record = from m in db.Appearance where m.OrderNum == item.OrderNum select m;//查出OrderNum的所有外观包装记录
                    if(Appearances_Record!=null)
                    {
                        Decimal Appearances_Normal = Appearances_Record.Where(m => m.Appearance_OQCCheckAbnormal == "正常").Count();//外观包装正常个数
                        //计算外观包装完成率、合格率
                        if (Appearances_Normal == 0)
                        {
                            OrderNum.Add("Appearances_Finish_Rate", "--%");
                            OrderNum.Add("Appearances_Pass_Rate", "--%");
                        }
                        else
                        {
                            OrderNum.Add("Appearances_Finish_Rate", (Appearances_Normal / item.Boxes*100).ToString("F2") + "%");
                            OrderNum.Add("Appearances_Pass_Rate", (Appearances_Normal / Appearances_Record.Count()*100).ToString("F2") + "%");
                        }
                    }
                    else
                    {
                        OrderNum.Add("Appearances_Finish_Rate", "--%");
                        OrderNum.Add("Appearances_Pass_Rate", "--%");
                    }

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