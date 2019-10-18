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
using System.Diagnostics;
using JianHeMES.Controllers;

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
                10000);
        }


        private void SMTBroadcastShape(object state)
        {
            //JObject SMT_ProductionInfo = new JObject();
            //JObject OrderNumdata = new JObject();
            //JObject LineData = new JObject();

            JObject SMT_Manage = new JObject();
            JObject SMT_ProductionInfo1 = new JObject();
            JObject OrderNumdata1 = new JObject();
            JObject LineData1 = new JObject();
            CommonalityController comm = new CommonalityController();
            using (var db = new ApplicationDbContext())
            {
                
                #region-------------------SMT总看板SMT_ProductionInfo1 JSON

                #region-----版本1
                var SMT_ProductionBoard = (from c in db.SMT_ProductionBoardTable select new SMT_ProductionBoardResule { OrderNum = c.OrderNum, LineNum = c.LineNum, JobContent = c.JobContent }).ToList();

                var SMT_ProductionPlans = (from c in db.SMT_ProductionPlan select new SMT_ProductionPlansResule { OrderNum = c.OrderNum, LineNum = c.LineNum, JobContent = c.JobContent, planDateTime = c.PlanProductionDate, Quantity = c.Quantity, Capacity = c.Capacity }).ToList();

                var SMT_ProductionDatas = (from c in db.SMT_ProductionData select new SMT_ProductionDateResule { OrderNum = c.OrderNum, LineNum = c.LineNum, JobContent = c.JobContent, BeginTime = c.BeginTime, EndTime = c.EndTime, AbnormalCount = c.AbnormalCount, NormalCount = c.NormalCount }).ToList();

                var orderMug = (from c in db.OrderMgm select new OrderMgmResule { OrderNum = c.OrderNum, ID = c.ID, Models = c.Models, AdapterCard = c.AdapterCard, Powers = c.Powers, PlatformType = c.PlatformType, ProcessingRequire = c.ProcessingRequire, HandSampleScedule = c.HandSampleScedule, StandardRequire = c.StandardRequire }).ToList();


                List<string> productionBoardList = new List<string>();
                List<string> firstList = new List<string>();
                List<string> secondList = new List<string>();
                List<string> threeList = new List<string>();

                foreach (var item in SMT_ProductionBoard)
                {
                    var Plancount = SMT_ProductionPlans.Where(c => c.OrderNum == item.OrderNum && c.LineNum == item.LineNum && c.JobContent == item.JobContent && string.Format("{0:yyyy-MM-dd}", c.planDateTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Select(c => c.OrderNum).ToList();
                    //今天是否录有产线
                    var DateCount = SMT_ProductionDatas.Where(c => c.OrderNum == item.OrderNum && c.LineNum == item.LineNum && c.JobContent == item.JobContent && string.Format("{0:yyyy-MM-dd}", c.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Select(c => c.OrderNum).ToList();

                    if (Plancount.Count != 0)
                    {
                        if (DateCount.Count != 0)
                            productionBoardList.Add(item.OrderNum);
                        else
                            firstList.Add(item.OrderNum);
                    }
                    else
                    {
                        if (DateCount.Count != 0)
                            secondList.Add(item.OrderNum);
                        else
                            threeList.Add(item.OrderNum);
                    }

                }

                productionBoardList.AddRange(firstList);
                productionBoardList.AddRange(secondList);
                productionBoardList.AddRange(threeList);

                //用时1.3S
                //-------------------------------------



                foreach (var Listitem in productionBoardList)
                {
                    if (!SMT_ProductionInfo1.ContainsKey(Listitem))
                    {
                        var planOrderNum = SMT_ProductionBoard.Where(c => c.OrderNum == Listitem);
                        int i = 1;
                        foreach (var itemPlan in planOrderNum)
                        {
                            //订单记录信息
                            var OrderMgmsInfo = orderMug.Where(c => c.OrderNum == itemPlan.OrderNum).Select(c => new { c.ID, c.Models, c.AdapterCard, c.Powers, c.PlatformType, c.StandardRequire, c.HandSampleScedule, c.ProcessingRequire }).FirstOrDefault();
                            if (OrderMgmsInfo == null)
                                continue;
                            //今天计划数
                            var todayPlanNum = SMT_ProductionPlans.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent && string.Format("{0:yyyy-MM-dd}", c.planDateTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Select(c => c.Quantity).FirstOrDefault();

                            //今天产能
                            var todayCapacity = SMT_ProductionPlans.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent && string.Format("{0:yyyy-MM-dd}", c.planDateTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Select(c => c.Capacity).FirstOrDefault();

                            //今天良品
                            var todayNormalNum = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent && string.Format("{0:yyyy-MM-dd}", c.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Count() == 0 ? 0 : SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent && string.Format("{0:yyyy-MM-dd}", c.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Sum(C => C.NormalCount);

                            //今天不良品
                            var todayAbnormalNum = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent && string.Format("{0:yyyy-MM-dd}", c.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Count() == 0 ? 0 : SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent && string.Format("{0:yyyy-MM-dd}", c.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Sum(c => c.AbnormalCount);

                            //对应线别良品总数
                            var lineNormalNumSum = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent).Count() == 0 ? 0 : SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent).Sum(c => c.NormalCount);

                            //对应线别不良品总数
                            var lineAbnormalNumSum = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent).Count() == 0 ? 0 : SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent).Sum(c => c.AbnormalCount);

                            //对应工作内容良品总数
                            var jobContentNormalNumSum = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.JobContent.Equals(itemPlan.JobContent,StringComparison.OrdinalIgnoreCase)).Count() == 0 ? 0 : SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.JobContent.Equals(itemPlan.JobContent, StringComparison.OrdinalIgnoreCase)).Sum(c => c.NormalCount);

                            //对应工作内容不良品总数
                            var jobContentAbnormalNumSum = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.JobContent.Equals(itemPlan.JobContent, StringComparison.OrdinalIgnoreCase)).Count() == 0 ? 0 : SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.JobContent.Equals(itemPlan.JobContent, StringComparison.OrdinalIgnoreCase)).Sum(c => c.AbnormalCount);

                            //开始时间
                            var lineBegintime = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent).Min(c => c.BeginTime);
                            //结束时间
                            var lineEndTime = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent).Max(c => c.EndTime);

                            

                            //模块术
                            var ModelNum = 0;
                            if (itemPlan.JobContent == "灯面" || itemPlan.JobContent == "IC面")
                            {
                                ModelNum=  OrderMgmsInfo.Models;
                            }
                            else if (itemPlan.JobContent != null && itemPlan.JobContent.Contains("转接卡") == true)
                            {
                                ModelNum = OrderMgmsInfo.AdapterCard;
                            }
                            else if (itemPlan.JobContent != null && itemPlan.JobContent.Contains("电源") == true)
                            {
                                ModelNum = OrderMgmsInfo.Powers;
                            }

                            if ((jobContentAbnormalNumSum + jobContentNormalNumSum >= ModelNum && (DateTime.Now - Convert.ToDateTime(lineEndTime)).Days >= 1))
                            {
                                continue;
                            }
                            //订单ID号
                            LineData1.Add("Id", OrderMgmsInfo.ID.ToString());
                            //订单ID号
                            LineData1.Add("OrderNum", itemPlan.OrderNum);
                            //平台
                            LineData1.Add("PlatformType", OrderMgmsInfo.PlatformType);
                            //制作要求
                            LineData1.Add("ProcessingRequire", OrderMgmsInfo.ProcessingRequire);
                            //标准要求
                            LineData1.Add("StandardRequire", OrderMgmsInfo.StandardRequire);
                            //模块数
                            LineData1.Add("Models", ModelNum);
                            //工作内容
                            LineData1.Add("JobContent", itemPlan.JobContent);
                            //今天计划数
                            LineData1.Add("PlanQuantity", todayPlanNum.ToString());
                            //今天产能
                            LineData1.Add("Capacity", todayCapacity.ToString());
                            //小样进度
                            LineData1.Add("HandSampleScedule", OrderMgmsInfo.HandSampleScedule);
                            //是否有图片
                            if(comm.CheckJpgExit(itemPlan.OrderNum, "SmallSample_Files"))
                                LineData1.Add("HandSampleSceduleJpg", "false");
                            else
                                LineData1.Add("HandSampleSceduleJpg", "true");
                            //是否有PDF文件
                            if (comm.CheckpdfExit(itemPlan.OrderNum, "SmallSample_Files"))
                                LineData1.Add("HandSampleScedulePdf", "false");
                            else
                                LineData1.Add("HandSampleScedulePdf", "true");
                            //线别
                            LineData1.Add("LineNum", itemPlan.LineNum);
                            //今天良品数
                            LineData1.Add("NormalCount", todayNormalNum);
                            //今天不良品数
                            LineData1.Add("AbnormalCount", todayAbnormalNum);
                            //今天完成率
                            LineData1.Add("TodayFinishRate", todayPlanNum == 0 ? "" : (((decimal)todayNormalNum + todayAbnormalNum) / todayPlanNum * 100).ToString("F2"));
                            //今天合格率
                            LineData1.Add("TodayPassRate", todayPlanNum == 0 || todayNormalNum + todayAbnormalNum == 0 ? "" : ((decimal)todayNormalNum / (todayNormalNum + todayAbnormalNum) * 100).ToString("F2"));
                            //对应线别良品总数
                            LineData1.Add("LineAllNomalCount", lineNormalNumSum);
                            //对应线别不良品总数
                            LineData1.Add("LineAllAbnomalCount", lineAbnormalNumSum);
                            //对应工作内容良品总数
                            LineData1.Add("AllNormalCount", jobContentNormalNumSum);
                            //对应工作内容不良品总数
                            LineData1.Add("AllAbnormalCount", jobContentAbnormalNumSum);
                            //订单总完成率
                            LineData1.Add("OrderFinishRate", ModelNum == 0 ? "" : (((decimal)(jobContentNormalNumSum + jobContentAbnormalNumSum) * 100) / ModelNum ).ToString("F2"));
                            //订单总合格率
                            LineData1.Add("OrderPassRate", (jobContentAbnormalNumSum + jobContentNormalNumSum) == 0 ? "" : ((decimal)jobContentNormalNumSum * 100 / (jobContentNormalNumSum + jobContentAbnormalNumSum)).ToString("F2"));
                            //开始时间
                            LineData1.Add("BeginTime", lineBegintime.ToString());
                            //结束时间
                            if (jobContentAbnormalNumSum + jobContentNormalNumSum >= ModelNum)
                            {
                                LineData1.Add("EndTime", lineEndTime.ToString());
                                LineData1.Add("TotalTime", (Convert.ToDateTime(lineEndTime) - Convert.ToDateTime(lineBegintime)).ToString());
                            }
                            else
                            {
                                LineData1.Add("EndTime", "");
                                LineData1.Add("TotalTime", "");
                            }
                            //生产用时



                            OrderNumdata1.Add(i.ToString(), JsonConvert.SerializeObject(LineData1));
                            LineData1.RemoveAll();
                            i++;
                        }
                        SMT_ProductionInfo1.Add(Listitem, JsonConvert.SerializeObject(OrderNumdata1));
                        OrderNumdata1.RemoveAll(); ;
                    }
                }
                #endregion
                #endregion
           
            }

            //广播发送JSON数据
            //SMT_hubContext.Clients.All.sendSMT_ProductionInfo(SMT_ProductionInfo);
            SMT_hubContext.Clients.All.sendSMT_ProductionInfo1(SMT_ProductionInfo1);
        }

        public static SMTBroadcaster SMTInstance
        {
            get
            {
                return SMT_instance.Value;
            }
        }
    }

    internal class SMT_ProductionPlansResule
    {
        public string OrderNum { get; set; }
        public int LineNum { get; set; }
        public string JobContent { get; set; }

        public DateTime? planDateTime { get; set; }

        public int Quantity { get; set; }

        public decimal Capacity { get; set; }
    }

    internal class SMT_ProductionDateResule
    {
        public string OrderNum { get; set; }
        public int LineNum { get; set; }
        public string JobContent { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int NormalCount { get; set; }
        public int AbnormalCount { get; set; }

    }

    internal class SMT_ProductionBoardResule
    {
        public string OrderNum { get; set; }
        public int LineNum { get; set; }
        public string JobContent { get; set; }

    }

    internal class OrderMgmResule
    {
        public string OrderNum { get; set; }
        public int ID { get; set; }
        public int Models { get; set; }
        public int AdapterCard { get; set; }

        public int Powers { get; set; }

        public string PlatformType { get; set; }

        public string StandardRequire { get; set; }

        public string HandSampleScedule { get; set; }

        public string ProcessingRequire { get; set; }
    }

    #endregion
}