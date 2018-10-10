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

namespace JianHeMESEntities.Hubs
{
    //SMT SMT生产信息
    #region --------------------------------SMT生产信息-------------------------------------

    [HubName("SMT")]
    public class SMT : Hub
    {
        // Is set via the constructor on each creation
        private SMTBroadcaster SMTBroadcaster_broadcaster;
        public SMT()
            : this(SMTBroadcaster.SMTInstance)
        {
        }
        public SMT(SMTBroadcaster SMTbroadcaster)
        {
            SMTBroadcaster_broadcaster = SMTbroadcaster;
        }
    }

    /// <summary>
    /// 数据广播器
    /// </summary>
    public class SMTBroadcaster
    {
        private readonly static Lazy<SMTBroadcaster> SMT_instance =
            new Lazy<SMTBroadcaster>(() => new SMTBroadcaster());

        private readonly IHubContext SMT_hubContext;

        private Timer SMT_broadcastLoop;

        public SMTBroadcaster()
        {
            // 获取所有连接的句柄，方便后面进行消息广播
            SMT_hubContext = GlobalHost.ConnectionManager.GetHubContext<SMT>();
            // Start the broadcast loop
            SMT_broadcastLoop = new Timer(
                SMTBroadcastShape,
                null,
                0,
                3000);
        }


        private void SMTBroadcastShape(object state)
        {
            JObject SMT_Board = new JObject(); //创建各条产线看板JSON对象  1.1 JSON
            JObject SMT_ProductionInfo = new JObject(); //创建SMT总看板JSON对象 1.1 JSON
            JObject SMT_Operator = new JObject(); //创建各条产线操作员JSON对象

            using (var db = new ApplicationDbContext())
            {
                //取出Status状态为false的订单表清单
                var SMT_OrderInfos = db.SMT_OrderInfo./*Where(c => c.Status == false).*/ToList();
                //取出今天计划生产订单清单（全部产线）
                var SMT_ProductionPlans = db.SMT_ProductionPlan.Where(c => DbFunctions.DiffDays(c.CreateDate, DateTime.Now) == 0).ToList();
                //取出产线信息
                var SMT_ProductionLineInfos = db.SMT_ProductionLineInfo.ToList();
                //取出今天的生产数据
                var SMT_ProductionDatas = db.SMT_ProductionData.Where(c => DbFunctions.DiffDays(c.ProductionDate, DateTime.Now) == 0).ToList();
                //取出SMT用户信息
                var SMT_Users = db.Users.Where(c => c.Department == "SMT").ToList();
                //取出现有产线列表
                var SMT_LineList = SMT_ProductionLineInfos.Select(c => c.LineNum).ToList();

                SMT_ProductionLineInfo LineInfo = new SMT_ProductionLineInfo();//产线信息
                List<SMT_ProductionData> LineProductionData = new List<SMT_ProductionData>();

                #region-------------------SMT总看板SMT_ProductionInfo JSON
                foreach (var i in SMT_LineList.ToList())
                {
                    JObject SMT_LineInfo = new JObject();//各条产线JSON对象  1.1.1 JSON

                    int x = 1;
                    //产线i生产数据
                    LineProductionData = SMT_ProductionDatas.Where(c => c.LineNum == i).ToList();//产线i生产数据
                    LineInfo = SMT_ProductionLineInfos.Where(c => c.LineNum == i).ToList().FirstOrDefault();//产线i信息

                    //产线i今天生产订单
                    JObject LineTodayFinishOrder = new JObject();
                    var LineTodayFinishOrderList = LineProductionData.Select(c => c.OrderNum).ToList();
                    foreach (var item in LineTodayFinishOrderList)
                    {
                        LineTodayFinishOrder.Add(x.ToString(), item);
                        x++;
                    }
                    SMT_LineInfo.Add("LineTodayFinishOrder", LineTodayFinishOrder);//产线i今天生产订单

                    //产线i今天生产订单合格品
                    JObject LineTodayFinishOrderNormal = new JObject();
                    var LineTodayFinishOrderNormalList = LineProductionData.Select(c => c.NormalCount).ToList();
                    x = 1;
                    foreach (var item in LineTodayFinishOrderNormalList)
                    {
                        LineTodayFinishOrderNormal.Add(x.ToString(), item);
                        x++;
                    }
                    SMT_LineInfo.Add("LineTodayFinishOrderNormal", LineTodayFinishOrderNormal);//产线i今天生产订单合格品

                    //产线i今天生产订单不良品
                    JObject LineTodayFinishOrderAbnormal = new JObject();
                    var LineTodayFinishOrderAbnormalList = LineProductionData.Select(c => c.AbnormalCount).ToList();
                    x = 1;
                    foreach (var item in LineTodayFinishOrderAbnormalList)
                    {
                        LineTodayFinishOrderAbnormal.Add(x.ToString(), item);
                        x++;
                    }
                    SMT_LineInfo.Add("LineTodayFinishOrderAbnormal", LineTodayFinishOrderAbnormal);//产线i今天生产订单不良品

                    //产线i今天计划生产订单
                    JObject LinePlanOrder = new JObject();
                    var LinePlanOrderList = SMT_ProductionPlans.Where(c=>c.LineNum==i).Select(c => c.OrderNum).ToList();
                    x = 1;
                    foreach (var item in LinePlanOrderList)
                    {
                        LinePlanOrder.Add(x.ToString(), item);
                        x++;
                    }
                    SMT_LineInfo.Add("LinePlanOrder", LinePlanOrder);//产线i今天计划生产订单

                    SMT_LineInfo.Add("LineDoingOrder", LineInfo.ProducingOrderNum);//产线i正在生产订单
                    SMT_LineInfo.Add("LineTodayFinishOrderQuantity", LineProductionData.Sum(c => c.NormalCount + c.AbnormalCount));//产线i今天生产订单数量
                    SMT_LineInfo.Add("LineTotalQuantity", LineProductionData.Sum(c => c.NormalCount + c.AbnormalCount));//产线i累计数量
                    SMT_LineInfo.Add("Team", LineInfo.Team);//产线i的班组
                    SMT_LineInfo.Add("GroupLeader", LineInfo.GroupLeader);//产线i组长
                    SMT_LineInfo.Add("LineStatus", LineInfo.Status);//产线i状态
                    SMT_ProductionInfo.Add(i.ToString(), SMT_LineInfo);//产线i的看板信息  1.1.1 JSON-->1.1 JSON
                }
                #endregion


                #region-------------------各条产线SMT_Board看板JSON
                foreach (var i in SMT_LineList)
                {
                    JObject SMT_LineInfo = new JObject();//各条产线JSON对象  1.1.1 JSON

                    LineInfo = SMT_ProductionLineInfos.Where(c => c.LineNum == i).ToList().FirstOrDefault();//产线i信息
                    LineProductionData = SMT_ProductionDatas.Where(c => c.LineNum == i).ToList();//产线i生产数据

                    int x = 1;
                    //产线i今天生产订单
                    JObject LineTodayFinishOrder = new JObject();
                    var LineTodayFinishOrderList = LineProductionData.Select(c => c.OrderNum).ToList();
                    foreach (var item in LineTodayFinishOrderList)
                    {
                        LineTodayFinishOrder.Add(item.ToString(), item);
                        x++;
                    }
                    SMT_LineInfo.Add("LineTodayFinishOrder", LineTodayFinishOrder);//产线i今天生产订单


                    //产线i今天生产订单合格品
                    JObject LineTodayFinishOrderNormal = new JObject();
                    var LineTodayFinishOrderNormalList = LineProductionData.Select(c => c.NormalCount);
                    x = 1;
                    foreach (var item in LineTodayFinishOrderNormalList)
                    {
                        LineTodayFinishOrderNormal.Add(item.ToString(), item);
                        x++;
                    }
                    SMT_LineInfo.Add("LineTodayFinishOrderNormal", LineTodayFinishOrderNormal);//产线i今天生产订单合格品

                    //产线i今天生产订单不良品
                    JObject LineTodayFinishOrderAbnormal = new JObject();
                    var LineTodayFinishOrderAbnormalList = LineProductionData.Select(c => c.AbnormalCount).ToList();
                    x = 1;
                    foreach (var item in LineTodayFinishOrderAbnormalList)
                    {
                        LineTodayFinishOrderAbnormal.Add(item.ToString(), item);
                        x++;
                    }
                    SMT_LineInfo.Add("LineTodayFinishOrderAbnormal", LineTodayFinishOrderAbnormal);//产线i今天生产订单不良品

                    //产线i今天计划生产订单
                    JObject LinePlanOrder = new JObject();
                    var LinePlanOrderList = SMT_ProductionPlans.Where(c => c.LineNum == i).Select(c => c.OrderNum).ToList();
                    x = 1;
                    foreach (var item in LinePlanOrderList)
                    {
                        LinePlanOrder.Add(item.ToString(), item);
                        x++;
                    }
                    SMT_LineInfo.Add("LinePlanOrder", LinePlanOrder);//产线i今天计划生产订单

                    SMT_LineInfo.Add("Date", DateTime.Now.ToString());//产线i日期
                    SMT_LineInfo.Add("Team", LineInfo.Team);//产线i的班组
                    SMT_LineInfo.Add("GroupLeader", LineInfo.GroupLeader);//产线i组长
                    SMT_LineInfo.Add("LineStatus", LineInfo.Status);//产线i状态
                    SMT_LineInfo.Add("LineDoingOrder", LineInfo.ProducingOrderNum);//产线i正在生产订单
                    SMT_LineInfo.Add("LineTotalQuantity", LineProductionData.Where(c => c.OrderNum == LineInfo.ProducingOrderNum).Sum(c => c.NormalCount + c.AbnormalCount));//产线i今天生产数量汇总
                    SMT_LineInfo.Add("OrderTotalFinishQuantity", SMT_ProductionDatas.Where(c => c.OrderNum == LineInfo.ProducingOrderNum).Sum(c => c.NormalCount + c.AbnormalCount));//产线i正在生产的订单累计生产数量
                    SMT_LineInfo.Add("OrderQuantity", SMT_OrderInfos.Where(c => c.OrderNum == LineInfo.ProducingOrderNum).Select(c => c.Quantity).ToString());//产线i正在生产的订单数量
                    SMT_Board.Add(i.ToString(), SMT_LineInfo);//产线i的看板信息   1.1.1 JSON-->1.1 JSON
                }

                #endregion


                #region-------------------各条产线SMT_Operator操作员JSON
                foreach (var i in SMT_LineList)
                {
                    JObject SMT_LineInfo = new JObject();//各条产线JSON对象  1.1.1 JSON

                    LineInfo = SMT_ProductionLineInfos.Where(c => c.LineNum == i).ToList().FirstOrDefault();//产线i信息
                    LineProductionData = SMT_ProductionDatas.Where(c => c.LineNum == i).ToList();//产线i生产数据

                    int x = 1;
                    //产线i今天生产订单
                    JObject LineTodayFinishOrder = new JObject();
                    var LineTodayFinishOrderList = LineProductionData.Select(c => c.OrderNum).ToList();
                    foreach (var item in LineTodayFinishOrderList)
                    {
                        LineTodayFinishOrder.Add(item.ToString(), item);
                        x++;
                    }
                    SMT_LineInfo.Add("LineTodayFinishOrder", LineTodayFinishOrder);//产线i今天生产订单


                    //产线i今天生产订单合格品
                    JObject LineTodayFinishOrderNormal = new JObject();
                    var LineTodayFinishOrderNormalList = LineProductionData.Select(c => c.NormalCount).ToList();
                    x = 1;
                    foreach (var item in LineTodayFinishOrderNormalList)
                    {
                        LineTodayFinishOrderNormal.Add(item.ToString(), item);
                        x++;
                    }
                    SMT_LineInfo.Add("LineTodayFinishOrderNormal", LineTodayFinishOrderNormal);//产线i今天生产订单合格品

                    //产线i今天生产订单不良品
                    JObject LineTodayFinishOrderAbnormal = new JObject();
                    var LineTodayFinishOrderAbnormalList = LineProductionData.Select(c => c.AbnormalCount).ToList();
                    x = 1;
                    foreach (var item in LineTodayFinishOrderAbnormalList)
                    {
                        LineTodayFinishOrderAbnormal.Add(item.ToString(), item);
                        x++;
                    }
                    SMT_LineInfo.Add("LineTodayFinishOrderAbnormal", LineTodayFinishOrderAbnormal);//产线i今天生产订单不良品

                    //产线i今天计划生产订单
                    JObject LinePlanOrder = new JObject();
                    var LinePlanOrderList = SMT_ProductionPlans.Where(c => c.LineNum == i).Select(c => c.OrderNum).ToList();
                    x = 1;
                    foreach (var item in LinePlanOrderList)
                    {
                        LinePlanOrder.Add(item.ToString(), item);
                        x++;
                    }
                    SMT_LineInfo.Add("LinePlanOrder", LinePlanOrder);//产线i今天计划生产订单

                    SMT_LineInfo.Add("LineDoingOrder", LineInfo.ProducingOrderNum);//产线i正在生产订单
                    SMT_LineInfo.Add("LineTodayFinishOrderQuantity", LineProductionData.Sum(c => c.NormalCount + c.AbnormalCount));//产线i今天生产订单数量
                    SMT_LineInfo.Add("LineTotalQuantity", LineProductionData.Sum(c => c.NormalCount + c.AbnormalCount));//产线i累计数量
                    SMT_LineInfo.Add("Team", LineInfo.Team);//产线i的班组
                    SMT_LineInfo.Add("GroupLeader", LineInfo.GroupLeader);//产线i组长
                    SMT_LineInfo.Add("LineStatus", LineInfo.Status);//产线i状态
                    SMT_Operator.Add(i.ToString(), SMT_LineInfo);//产线i的看板信息   1.1.1 JSON-->1.1 JSON
                }
                #endregion
            }

            //广播发送JSON数据
            SMT_hubContext.Clients.All.sendSMT_Board(SMT_Board);
            SMT_hubContext.Clients.All.sendSMT_ProductionInfo(SMT_ProductionInfo);
            SMT_hubContext.Clients.All.sendSMT_Operator(SMT_Operator);

        }

        public static SMTBroadcaster SMTInstance
        {
            get
            {
                return SMT_instance.Value;
            }
        }
    }
    #endregion
}