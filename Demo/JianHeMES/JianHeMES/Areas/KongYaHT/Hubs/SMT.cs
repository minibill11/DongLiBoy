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
                60000);
        }


        private void SMTBroadcastShape(object state)
        {
            //JObject SMT_Board = new JObject(); //创建各条产线看板JSON对象  1.1 JSON
            //JObject SMT_ProductionInfo = new JObject(); //创建SMT总看板JSON对象 1.1 JSON
            //JObject SMT_ProductionInfo_old = new JObject(); //创建SMT总看板JSON对象 1.1 JSON
            //JObject SMT_Operator = new JObject(); //创建各条产线操作员JSON对象

            JObject SMT_ProductionInfo = new JObject();
            JObject OrderNumdata = new JObject();
            JObject LineData = new JObject();
            JObject SMT_Manage = new JObject();
            using (var db = new ApplicationDbContext())
            {
                //取出计划生产订单清单（全部产线）(计划创建时间>2019-3-1)
                var SMT_ProductionPlans = db.SMT_ProductionPlan.Where(c=>c.CreateDate>=new DateTime (2019,3,1)).OrderBy(c => c.LineNum);

                //取出今天计划生产订单清单
                var Today_SMT_ProductionPlans = db.SMT_ProductionPlan.Where(c => DbFunctions.DiffDays(c.PlanProductionDate, DateTime.Now) == 0).OrderBy(c => c.LineNum);
                //取出产线信息
                var SMT_ProductionLineInfos = db.SMT_ProductionLineInfo;
                //取出所有生产数据
                var SMT_ProductionDatas = db.SMT_ProductionData;
                //取出今天的生产数据
                var Today_SMT_ProductionDatas = db.SMT_ProductionData.Where(c => DbFunctions.DiffDays(c.ProductionDate, DateTime.Now) == 0).OrderBy(c => c.LineNum);

                //取出现有产线列表
                var SMT_LineList = SMT_ProductionLineInfos.Select(c => c.LineNum);

                var SMT_ProductionPlans_by_Group_temp = SMT_ProductionPlans.Join(SMT_ProductionDatas,c=> c.OrderNum,a=>a.OrderNum,(c,a)=>new{ c.OrderNum, c.LineNum, c.JobContent,a.ProductionDate }).OrderByDescending(a=>a.ProductionDate).Select(b => new { b.OrderNum, b.LineNum, b.JobContent,b.ProductionDate });
                var SMT_ProductionPlans_by_Group_temp1 = SMT_ProductionPlans_by_Group_temp.OrderByDescending(c => c.ProductionDate).Select(c => new { c.LineNum, c.OrderNum, c.JobContent }).Distinct();

                //取出计划订单清单(去重)
                var SMT_ProductionPlans_list = SMT_ProductionPlans.Select(c =>c.OrderNum).Distinct();


                #region-------------------SMT_Manage看板管理 JSON
                //foreach ( var item in SMT_LineList)
                //{
                //    JObject lineInfo = new JObject();
                //    var lastProductionRecord = SMT_ProductionDatas.Where(c => c.LineNum == item).OrderByDescending(c => c.ProductionDate).FirstOrDefault();
                //    var lastProductionDate = Convert.ToDateTime(lastProductionRecord.ProductionDate);
                //    if((DateTime.Now-Convert.ToDateTime(lastProductionDate)).TotalHours>3) //超3小时
                //    {
                //        lineInfo.Add("Status", "待料");
                //        lineInfo.Add("Producing_OrderNum", "");
                //        lineInfo.Add("Team", "");
                //        lineInfo.Add("GroupLeader", "");
                //    }
                //    else
                //    {
                //        lineInfo.Add("Status", "生产中");
                //        lineInfo.Add("Producing_OrderNum", lastProductionRecord.OrderNum);
                //        lineInfo.Add("Team","");
                //        lineInfo.Add("GroupLeader", lastProductionRecord.Operator);
                //    }
                //    SMT_Manage.Add(item.ToString(), JsonConvert.SerializeObject(lineInfo));
                //}
                #endregion


                #region-------------------SMT总看板SMT_ProductionInfo_new JSON
                foreach (var item in SMT_ProductionPlans_list)
                {
                    var ordernumMgms = db.OrderMgm.Where(c => c.OrderNum == item).FirstOrDefault();//订单基本信息
                    var orderAllRecordList = db.SMT_ProductionData.Where(c => c.OrderNum == item);//订单全部记录

                    var SMT_ProductionPlans_by_Group = SMT_ProductionPlans.Where(c=>c.OrderNum==item).GroupBy(c => new { c.OrderNum,c.LineNum, c.JobContent }).Select(c=>new { c.Key.OrderNum, c.Key.LineNum, c.Key.JobContent }).Distinct()/*.ToList()*/;

                    int s = 1;

                    foreach (var Group_item in SMT_ProductionPlans_by_Group)
                    {
                        var today_NomalCount = db.SMT_ProductionData.Where(c =>c.OrderNum ==Group_item.OrderNum && c.JobContent == Group_item.JobContent && c.LineNum == Group_item.LineNum && DbFunctions.DiffDays(c.BeginTime,DateTime.Now)>0 ).Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == Group_item.OrderNum && c.JobContent == Group_item.JobContent && c.LineNum == Group_item.LineNum && DbFunctions.DiffDays(c.BeginTime, DateTime.Now) > 0).Sum(c => c.NormalCount);//今天良品数
                        var today_AbnomalCount = db.SMT_ProductionData.Where(c => c.OrderNum == Group_item.OrderNum && c.JobContent == Group_item.JobContent && c.LineNum == Group_item.LineNum && DbFunctions.DiffDays(c.BeginTime, DateTime.Now) > 0).Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == Group_item.OrderNum && c.JobContent == Group_item.JobContent && c.LineNum == Group_item.LineNum && DbFunctions.DiffDays(c.BeginTime, DateTime.Now) > 0).Sum(c => c.AbnormalCount);//今天不良品数
                        var PlanQuantity = db.SMT_ProductionPlan.Where(c => c.OrderNum == Group_item.OrderNum && c.JobContent == Group_item.JobContent && c.LineNum == Group_item.LineNum && DbFunctions.DiffDays(c.PlanProductionDate, DateTime.Now)>0).Count() == 0 ? 0 : db.SMT_ProductionPlan.Where(c => c.OrderNum == Group_item.OrderNum && c.JobContent == Group_item.JobContent && c.LineNum == Group_item.LineNum && DbFunctions.DiffDays(c.PlanProductionDate, DateTime.Now) > 0).FirstOrDefault().Quantity;//今天计划数
                        var Capacity = db.SMT_ProductionPlan.Where(c => c.OrderNum == Group_item.OrderNum && c.JobContent == Group_item.JobContent && c.LineNum == Group_item.LineNum && DbFunctions.DiffDays(c.PlanProductionDate, DateTime.Now) > 0).Count() == 0 ? 0 : db.SMT_ProductionPlan.Where(c => c.JobContent == Group_item.JobContent && c.LineNum == Group_item.LineNum && DbFunctions.DiffDays(c.PlanProductionDate, DateTime.Now) > 0).FirstOrDefault().Capacity;//今天计划产能
                        var lineAllNomalCount = orderAllRecordList.Where(c => c.LineNum == Group_item.LineNum && c.JobContent == Group_item.JobContent).Count() == 0 ? 0 : orderAllRecordList.Where(c=>c.LineNum==Group_item.LineNum && c.JobContent== Group_item.JobContent).Sum(c => c.NormalCount);//订单在此线别的良品总数
                        var lineAllAbnomalCount = orderAllRecordList.Where(c => c.LineNum == Group_item.LineNum && c.JobContent == Group_item.JobContent).Count() == 0 ? 0 : orderAllRecordList.Where(c => c.LineNum == Group_item.LineNum && c.JobContent == Group_item.JobContent).Sum(c => c.AbnormalCount);//订单在此线别的不良品总数
                        var allNomalCount = orderAllRecordList.Where(c => c.JobContent == Group_item.JobContent).Count()==0?0: orderAllRecordList.Where(c => c.JobContent == Group_item.JobContent).Sum(c => c.NormalCount);//订单对应工作内容良品总数
                        var allAbnomalCount = orderAllRecordList.Where(c => c.JobContent == Group_item.JobContent).Count()==0?0:orderAllRecordList.Where(c => c.JobContent == Group_item.JobContent).Sum(c => c.AbnormalCount);//订单对应工作内容不良品总数
                        var begintime = orderAllRecordList.Min(c => c.BeginTime);//开始时间
                        var endTime = orderAllRecordList.Max(c => c.EndTime);//结束时间
                        LineData.Add("Id", ordernumMgms == null ? "" : ordernumMgms.ID.ToString());//订单ID号
                        LineData.Add("OrderNum", ordernumMgms == null ? "" : item);//订单ID号
                        LineData.Add("PlatformType", ordernumMgms == null ? "" : ordernumMgms.PlatformType);//平台
                        if (Group_item.JobContent == "灯面" || Group_item.JobContent == "IC面")
                        {
                            LineData.Add("Models", ordernumMgms == null ? 0 : ordernumMgms.Models);//模块数
                        }
                        else if (Group_item.JobContent != null && Group_item.JobContent.Contains("转接卡") == true)
                        {
                            LineData.Add("Models", ordernumMgms == null ? 0 : ordernumMgms.AdapterCard);//模块数
                        }
                        else if (Group_item.JobContent != null && Group_item.JobContent.Contains("电源") == true)
                        {
                            LineData.Add("Models", ordernumMgms == null ? 0 : ordernumMgms.Powers);//模块数
                        }
                        else
                        {
                            LineData.Add("Models", "");//模块数
                        }
                        LineData.Add("ProcessingRequire", ordernumMgms == null ? "" : ordernumMgms.ProcessingRequire);//制程要求
                        LineData.Add("JobContent", Group_item.JobContent);//工作内容
                        LineData.Add("StandardRequire", ordernumMgms == null ? "" : ordernumMgms.StandardRequire);//标准要求
                        LineData.Add("PlanQuantity", PlanQuantity);//今天计划数
                        LineData.Add("Capacity", Capacity);//产能
                        LineData.Add("HandSampleScedule", ordernumMgms == null ? "" : ordernumMgms.HandSampleScedule);//小样进度
                        LineData.Add("LineNum", Group_item.LineNum);//线别
                        LineData.Add("NormalCount", today_NomalCount);//今天良品数
                        LineData.Add("AbnormalCount", today_AbnomalCount);//今天不良品数
                        LineData.Add("TodayFinishRate", PlanQuantity == 0 ? "" : (((decimal)(today_NomalCount + today_AbnomalCount)) / PlanQuantity * 100).ToString("F2"));//今天完成率
                        LineData.Add("TodayPassRate", today_NomalCount == 0 ? "" : ((decimal)today_NomalCount / (today_NomalCount + today_AbnomalCount) * 100).ToString("F2")); //今天合格率
                        LineData.Add("LineAllNomalCount", lineAllNomalCount);//订单在此线别的良品总数
                        LineData.Add("LineAllAbnomalCount", lineAllAbnomalCount); //订单在此线别的不良品总数
                        LineData.Add("AllNormalCount", allNomalCount);//订单对应工作内容良品总数
                        LineData.Add("AllAbnormalCount", allAbnomalCount); //订单对应工作内容不良品总数
                        LineData.Add("OrderFinishRate", (allNomalCount + allAbnomalCount) == 0 || ordernumMgms == null ? "" : (((decimal)(allNomalCount + allAbnomalCount)) / ordernumMgms.Models * 100).ToString("F2"));//订单总完成率
                        LineData.Add("OrderPassRate", (allNomalCount + allAbnomalCount) == 0 ? "" : ((decimal)allNomalCount / (allNomalCount + allAbnomalCount) * 100).ToString("F2")); //订单总合格率
                        LineData.Add("BeginTime", begintime.ToString());//开始时间
                        if (ordernumMgms != null)
                        {
                            if (allNomalCount + allAbnomalCount == ordernumMgms.Models)//完成时间
                            {
                                LineData.Add("EndTime", endTime);
                                LineData.Add("TotalTime", (Convert.ToDateTime(endTime) - Convert.ToDateTime(begintime)).ToString());
                            }
                        }
                        else
                        {
                            LineData.Add("EndTime", "");
                            LineData.Add("TotalTime", "");
                        }
                        OrderNumdata.Add(s.ToString(), JsonConvert.SerializeObject(LineData));
                        LineData = new JObject();
                        s++;
                    }
                    SMT_ProductionInfo.Add(item, JsonConvert.SerializeObject(OrderNumdata));
                    OrderNumdata = new JObject();
                }

                #region-----------之前代码
                //foreach (var item in tem_SMT_ProductionPlans)
                //{
                //    //订单的计划线别清单
                //    var linePlanList = SMT_ProductionPlans.Where(c => c.OrderNum == item).OrderBy(c => c.LineNum).ToList();//订单今天计划清单
                //    var ordernumMgms = db.OrderMgm.Where(c => c.OrderNum == item).FirstOrDefault();//订单基本信息
                //    var orderAllRecordList = db.SMT_ProductionData.Where(c => c.OrderNum == item).ToList();//订单全部记录
                //    int i = 1;
                //    foreach (var it in linePlanList)
                //    {
                //        var nomalCount = Today_SMT_ProductionDatas.Where(c => c.LineNum == it.LineNum && c.OrderNum == item && c.JobContent == it.JobContent).Sum(c => c.NormalCount);//今天的良品数
                //        var AbnomalCount = Today_SMT_ProductionDatas.Where(c => c.LineNum == it.LineNum && c.OrderNum == item && c.JobContent == it.JobContent).Sum(c => c.AbnormalCount);//今天不良品数
                //        var begintime = orderAllRecordList.Where(c => c.OrderNum == item && c.LineNum == it.LineNum && c.JobContent == it.JobContent).Min(c => c.BeginTime).ToString();//订单开始时间
                //        var lineAllNomalCount = orderAllRecordList.Where(c => c.JobContent == it.JobContent).Count() == 0 ? 0 : orderAllRecordList.Where(c => c.JobContent == it.JobContent).Sum(c => c.NormalCount);//订单在此线别的良品总数
                //        var lineAllAbnomalCount = orderAllRecordList.Where(c => c.JobContent == it.JobContent).Count() == 0 ? 0 : orderAllRecordList.Where(c => c.JobContent == it.JobContent).Sum(c => c.AbnormalCount);//订单在此线别的不良品总数
                //        var allNomalCount = orderAllRecordList.Where(c => c.OrderNum == item && c.JobContent == it.JobContent).Count() == 0 ? 0 : orderAllRecordList.Where(c => c.OrderNum == item && c.JobContent == it.JobContent).Sum(c => c.NormalCount);//订单对应工作内容良品总数
                //        var allAbnomalCount = orderAllRecordList.Where(c => c.OrderNum == item && c.JobContent == it.JobContent).Count() == 0 ? 0 : orderAllRecordList.Where(c => c.OrderNum == item && c.JobContent == it.JobContent).Sum(c => c.AbnormalCount);//订单对应工作内容不良品总数
                //        var endTime = orderAllRecordList.Where(c => c.OrderNum == item && c.JobContent == it.JobContent).Max(c => c.EndTime);//结束时间
                //        LineData.Add("Id", ordernumMgms == null ? "" : ordernumMgms.ID.ToString());//订单ID号
                //        LineData.Add("PlatformType", ordernumMgms == null ? "" : ordernumMgms.PlatformType);//平台
                //        LineData.Add("Models", ordernumMgms == null ? 0 : ordernumMgms.Models);//模块数
                //        LineData.Add("ProcessingRequire", ordernumMgms == null ? "" : ordernumMgms.ProcessingRequire);//制程要求
                //        LineData.Add("JobContent", it.JobContent);//SMT_ProductionPlans.Where(c => c.OrderNum == item && c.LineNum == it).FirstOrDefault().JobContent);//工作内容
                //        LineData.Add("StandardRequire", ordernumMgms == null ? "" : ordernumMgms.StandardRequire);//标准要求
                //        LineData.Add("PlanQuantity", it.Quantity);//今天计划数
                //        LineData.Add("Capacity", it.Capacity);//产能
                //        LineData.Add("HandSampleScedule", ordernumMgms == null ? "" : ordernumMgms.HandSampleScedule);//小样进度
                //        LineData.Add("LineNum", it.LineNum);//线别
                //        LineData.Add("NormalCount", nomalCount);//今天良品数
                //        LineData.Add("AbnormalCount", AbnomalCount); //今天不良品数
                //        LineData.Add("TodayFinishRate", (nomalCount + AbnomalCount) == 0 ? "" : (((decimal)(nomalCount + AbnomalCount)) / it.Quantity * 100).ToString("F2"));//今天完成率
                //        LineData.Add("TodayPassRate", nomalCount == 0 ? "" : ((decimal)nomalCount / (nomalCount + AbnomalCount) * 100).ToString("F2")); //今天合格率
                //        LineData.Add("LineAllNomalCount", lineAllNomalCount);//订单在此线别的良品总数
                //        LineData.Add("LineAllAbnomalCount", lineAllAbnomalCount); //订单在此线别的不良品总数
                //        LineData.Add("AllNormalCount", allNomalCount);//订单对应工作内容良品总数
                //        LineData.Add("AllAbnormalCount", allAbnomalCount); //订单对应工作内容不良品总数
                //        LineData.Add("OrderFinishRate", (allNomalCount + allAbnomalCount) == 0 ? "" : (((decimal)(allNomalCount + allAbnomalCount)) / ordernumMgms.Models * 100).ToString("F2"));//订单总完成率
                //        LineData.Add("OrderPassRate", allNomalCount == 0 ? "" : ((decimal)allNomalCount / (allNomalCount + allAbnomalCount) * 100).ToString("F2")); //订单总合格率
                //        LineData.Add("BeginTime", begintime);//开始时间
                //        if (ordernumMgms != null)
                //        {
                //            if (allNomalCount + allAbnomalCount == ordernumMgms.Models)//完成时间
                //            {
                //                LineData.Add("EndTime", endTime);
                //                LineData.Add("TotalTime", (Convert.ToDateTime(endTime) - Convert.ToDateTime(begintime)).ToString());
                //            }
                //        }
                //        else
                //        {
                //            LineData.Add("EndTime", "");
                //            LineData.Add("TotalTime", "");
                //        }
                //        OrderNumdata.Add(i.ToString(), JsonConvert.SerializeObject(LineData));
                //        LineData = new JObject();
                //        i++;
                //    }
                //    SMT_ProductionInfo_new.Add(item, JsonConvert.SerializeObject(OrderNumdata));
                //    OrderNumdata = new JObject();
                //}
                #endregion

                #endregion

                SMT_ProductionLineInfo LineInfo = new SMT_ProductionLineInfo();//产线信息
                List<SMT_ProductionData> LineProductionData = new List<SMT_ProductionData>();

                #region-------------------SMT总看板SMT_ProductionInfo JSON
                ////SMT生产线号
                //var LineNumList = db.SMT_ProductionLineInfo.OrderBy(d => d.LineNum).Select(c => c.LineNum).Distinct().ToList();
                //JObject LineOrderDetails = new JObject();
                ////今天全部计划生产的订单清单
                //var PlanProductionOrderNumList = db.SMT_ProductionPlan.Where(c => DbFunctions.DiffDays(c.CreateDate, DateTime.Now) == 0).ToList();
                ////今天全部生产记录清单
                //var TodayProductionData = db.SMT_ProductionData.Where(c => DbFunctions.DiffDays(c.ProductionDate, DateTime.Now) == 0).ToList();
                ////foreach每条生产线
                //foreach (var linenum in LineNumList)
                //{
                //    //取出今天本生产线的计划订单清单
                //    List<string> planOrderNumList = PlanProductionOrderNumList.Where(c => c.LineNum == linenum).Select(c => c.OrderNum).ToList();
                //    //foreach本生产线的每个计划订单号
                //    foreach (var planorder in planOrderNumList)
                //    {
                //        //今天良品
                //        int todayNormalcount = TodayProductionData.Where(c => c.OrderNum == planorder).FirstOrDefault() == null ? 0 : TodayProductionData.Where(c => c.OrderNum == planorder).Sum(c=>c.NormalCount);
                //        //今天不良品
                //        int todayAbnormalcount = TodayProductionData.Where(c => c.OrderNum == planorder).FirstOrDefault() == null ? 0 : TodayProductionData.Where(c => c.OrderNum == planorder).Sum(c => c.AbnormalCount);
                //        //订单总良品
                //        int orderNormalcount = 0;
                //        if (db.SMT_ProductionData.Where(c => c.OrderNum == planorder).Count() > 0)
                //        {
                //            orderNormalcount = db.SMT_ProductionData.Where(c => c.OrderNum == planorder).Sum(c => c.NormalCount);
                //        }
                //        //订单总不良品
                //        int orderAbnormalcount = 0;
                //        if (db.SMT_ProductionData.Where(c => c.OrderNum == planorder).Count() > 0)
                //        {
                //            orderAbnormalcount = db.SMT_ProductionData.Where(c => c.OrderNum == planorder).Sum(c => c.AbnormalCount);
                //        }
                //        //订单日计划
                //        int todayPlanQuantity = PlanProductionOrderNumList.Where(c => c.OrderNum == planorder).FirstOrDefault().Quantity;
                //        //订单数量
                //        int orderQuantity = db.OrderMgm.Where(c => c.OrderNum == planorder).FirstOrDefault() == null ? 0 : db.OrderMgm.Where(c => c.OrderNum == planorder).FirstOrDefault().Models;
                //        //产线信息
                //        var lineInfo = db.SMT_ProductionLineInfo.Where(c => c.LineNum == linenum).FirstOrDefault();

                //        //建一个List装一行信息
                //        List<string> detailInfor_planorder = new List<string>();
                //        //往List填写“平台”信息
                //        detailInfor_planorder.Add(db.OrderMgm.Where(c => c.OrderNum == planorder).FirstOrDefault() == null ? "" : db.OrderMgm.Where(c => c.OrderNum == planorder).FirstOrDefault().PlatformType);
                //        //往List填写“订单数量”信息
                //        detailInfor_planorder.Add(orderQuantity == 0 ? "" : orderQuantity.ToString());
                //        //往List填写“日计划数量”信息
                //        detailInfor_planorder.Add(todayPlanQuantity.ToString());
                //        //往List填写“日生产完成数量”信息
                //        detailInfor_planorder.Add((todayNormalcount + todayAbnormalcount).ToString());
                //        //往List填写“日生产完成率”信息
                //        detailInfor_planorder.Add(((todayNormalcount + todayAbnormalcount) * 100 / todayPlanQuantity).ToString("F2") + "%");
                //        //往List填写“不良品”信息
                //        detailInfor_planorder.Add(todayAbnormalcount.ToString());
                //        //往List填写“良品”信息
                //        detailInfor_planorder.Add(todayNormalcount.ToString());
                //        //往List填写“不良率”信息
                //        detailInfor_planorder.Add(todayAbnormalcount + todayNormalcount == 0 ? "0.00%" : (todayAbnormalcount * 100 / (todayAbnormalcount + todayNormalcount)).ToString("F2") + "%");
                //        //往List填写“总良品”信息
                //        detailInfor_planorder.Add(orderNormalcount.ToString());
                //        //往List填写“总不良品”信息
                //        detailInfor_planorder.Add(orderAbnormalcount.ToString());
                //        //往List填写“总不良品率”信息
                //        detailInfor_planorder.Add(orderNormalcount + orderAbnormalcount == 0 ? "0.00%" : (orderAbnormalcount * 100 / (orderNormalcount + orderAbnormalcount)).ToString("F2") + "%");
                //        //往List填写“累计数量”信息
                //        detailInfor_planorder.Add((orderNormalcount + orderAbnormalcount).ToString());
                //        //往List填写“订单完成率”信息
                //        detailInfor_planorder.Add(orderQuantity == 0 ? "0.00%" : ((orderNormalcount + orderAbnormalcount) * 100 / orderQuantity).ToString("F2") + "%");
                //        //往List填写“班组”信息
                //        detailInfor_planorder.Add(lineInfo.Team);
                //        //往List填写“组长”信息
                //        detailInfor_planorder.Add(lineInfo.GroupLeader);
                //        //往List填写“状态”信息

                //        if (TodayProductionData.Where(c => c.OrderNum == planorder).Count() > 0)
                //        {
                //            var subtime = DateTime.Now.Subtract(Convert.ToDateTime(TodayProductionData.Where(c => c.OrderNum == planorder).OrderByDescending(c=>c.ProductionDate).FirstOrDefault().ProductionDate));
                //            if (subtime.TotalMinutes > 90)
                //            {
                //                detailInfor_planorder.Add("待生产");
                //            }
                //            else
                //            {
                //                detailInfor_planorder.Add("生产中");
                //            }
                //        }
                //        else
                //        {
                //            detailInfor_planorder.Add("待生产");
                //        }
                //        LineOrderDetails.Add(planorder, JsonConvert.SerializeObject(detailInfor_planorder));
                //    }
                //    SMT_ProductionInfo.Add(linenum.ToString(), JsonConvert.SerializeObject(LineOrderDetails));//产线i的看板信息  1.1.1 JSON-->1.1 JSON
                //    LineOrderDetails = new JObject();
                //}
                #endregion


                #region-------------------SMT总看板SMT_ProductionInfo_old JSON
                //foreach (var i in SMT_LineList.ToList())
                //{
                //    JObject SMT_LineInfo = new JObject();//各条产线JSON对象  1.1.1 JSON

                //    int x = 1;
                //    //产线i生产数据
                //    LineProductionData = Today_SMT_ProductionDatas.Where(c => c.LineNum == i).ToList();//产线i生产数据
                //    LineInfo = SMT_ProductionLineInfos.Where(c => c.LineNum == i).ToList().FirstOrDefault();//产线i信息

                //    //产线i今天生产订单
                //    JObject LineTodayFinishOrder = new JObject();
                //    JObject OrderQuantity = new JObject();
                //    var LineTodayFinishOrderList = LineProductionData.Select(c => c.OrderNum).ToList();
                //    var OrderQuantityList = SMT_OrderInfos.Select(c => c.OrderNum);//订单数量
                //    foreach (var item in LineTodayFinishOrderList)
                //    {
                //        //订单数量
                //        foreach (var order in OrderQuantityList)
                //        {
                //            if (item == order)
                //            {
                //                OrderQuantity.Add(x.ToString(), SMT_OrderInfos.Where(c => c.OrderNum == order).FirstOrDefault().Models.ToString());
                //            }
                //        }
                //        //产线i今天生产订单
                //        LineTodayFinishOrder.Add(x.ToString(), item);
                //        x++;
                //    }
                //    SMT_LineInfo.Add("LineTodayFinishOrder", LineTodayFinishOrder);//产线i今天生产订单
                //    SMT_LineInfo.Add("OrderQuantity", OrderQuantity);//订单数量

                //    //产线i今天生产订单合格品
                //    JObject LineTodayFinishOrderNormal = new JObject();
                //    var LineTodayFinishOrderNormalList = LineProductionData.Select(c => c.NormalCount).ToList();
                //    x = 1;
                //    foreach (var item in LineTodayFinishOrderNormalList)
                //    {
                //        LineTodayFinishOrderNormal.Add(x.ToString(), item);
                //        x++;
                //    }
                //    SMT_LineInfo.Add("LineTodayFinishOrderNormal", LineTodayFinishOrderNormal);//产线i今天生产订单合格品

                //    //产线i今天生产订单不良品
                //    JObject LineTodayFinishOrderAbnormal = new JObject();
                //    var LineTodayFinishOrderAbnormalList = LineProductionData.Select(c => c.AbnormalCount).ToList();
                //    x = 1;
                //    foreach (var item in LineTodayFinishOrderAbnormalList)
                //    {
                //        LineTodayFinishOrderAbnormal.Add(x.ToString(), item);
                //        x++;
                //    }
                //    SMT_LineInfo.Add("LineTodayFinishOrderAbnormal", LineTodayFinishOrderAbnormal);//产线i今天生产订单不良品

                //    //产线i今天计划生产订单
                //    JObject LinePlanOrder = new JObject();
                //    var LinePlanOrderList = SMT_ProductionPlans.Where(c => c.LineNum == i).Select(c => c.OrderNum).ToList();
                //    x = 1;
                //    foreach (var item in LinePlanOrderList)
                //    {
                //        LinePlanOrder.Add(x.ToString(), item);
                //        x++;
                //    }
                //    SMT_LineInfo.Add("LinePlanOrder", LinePlanOrder);//产线i今天计划生产订单

                //    SMT_LineInfo.Add("LineDoingOrder", LineInfo.ProducingOrderNum);//产线i正在生产订单
                //    SMT_LineInfo.Add("LineTotalQuantity", LineProductionData.Sum(c => c.NormalCount + c.AbnormalCount));//产线i累计数量
                //    SMT_LineInfo.Add("Team", LineInfo.Team);//产线i的班组
                //    SMT_LineInfo.Add("GroupLeader", LineInfo.GroupLeader);//产线i组长
                //    SMT_LineInfo.Add("LineStatus", LineInfo.Status);//产线i状态
                //    SMT_ProductionInfo_old.Add(i.ToString(), SMT_LineInfo);//产线i的看板信息  1.1.1 JSON-->1.1 JSON
                //}
                #endregion


                #region-------------------各条产线SMT_ProductionLineInfo看板JSON       (待修改)

                //foreach (var i in SMT_LineList)
                //{
                //    JObject SMT_LineInfo = new JObject();//各条产线JSON对象  1.1.1 JSON

                //    LineInfo = SMT_ProductionLineInfos.Where(c => c.LineNum == i).ToList().FirstOrDefault();//产线i信息
                //    LineProductionData = Today_SMT_ProductionDatas.Where(c => c.LineNum == i).ToList();//产线i生产数据
                //    int OrderTotalFinishQuantity = Today_SMT_ProductionDatas.Where(c => c.LineNum == i && c.OrderNum == LineInfo.ProducingOrderNum).Sum(c => c.NormalCount + c.AbnormalCount); //产线i正在生产的订单累计生产数量
                //    int TotalFinishQuantity = Today_SMT_ProductionDatas.Where(c => c.LineNum == i).Sum(c => c.NormalCount + c.AbnormalCount); //产线累计生产数量

                //    //产线i今天生产订单   //订单数量
                //    string LineTodayFinishOrder = "";
                //    string OrderQuantity = "";
                //    var LineTodayFinishOrderList = LineProductionData.Select(c => c.OrderNum).ToList();
                //    var OrderList = SMT_OrderInfos.Select(c => c.OrderNum);
                //    foreach (var item in LineTodayFinishOrderList)
                //    {
                //        //订单数量
                //        foreach (var order in OrderList)
                //        {
                //            if (item == order)
                //            {
                //                if (OrderQuantity == "")
                //                {
                //                    OrderQuantity = SMT_OrderInfos.Where(c => c.OrderNum == order).FirstOrDefault().Models.ToString();
                //                }
                //                else
                //                {
                //                    OrderQuantity = OrderQuantity + "," + SMT_OrderInfos.Where(c => c.OrderNum == order).FirstOrDefault().Models.ToString();
                //                }
                //            }
                //        }
                //        //产线i今天生产订单
                //        if (LineTodayFinishOrder == "")
                //        {
                //            LineTodayFinishOrder = item.ToString();
                //        }
                //        else
                //        {
                //            LineTodayFinishOrder = LineTodayFinishOrder + "," + item;
                //        }
                //    }
                //    SMT_LineInfo.Add("LineTodayFinishOrder", LineTodayFinishOrder);//产线i今天生产订单
                //    SMT_LineInfo.Add("OrderQuantity", OrderQuantity);//订单数量


                //    //产线i今天生产订单合格品
                //    string LineTodayFinishOrderNormal = "";
                //    var LineTodayFinishOrderNormalList = LineProductionData.Select(c => c.NormalCount);
                //    foreach (var item in LineTodayFinishOrderNormalList)
                //    {
                //        if (LineTodayFinishOrderNormal == "")
                //        {
                //            LineTodayFinishOrderNormal = item.ToString();
                //        }
                //        else
                //        {
                //            LineTodayFinishOrderNormal = LineTodayFinishOrderNormal + "," + item;
                //        }
                //    }
                //    SMT_LineInfo.Add("LineTodayFinishOrderNormal", LineTodayFinishOrderNormal);//产线i今天生产订单合格品

                //    //产线i今天生产订单不良品
                //    string LineTodayFinishOrderAbnormal = "";
                //    var LineTodayFinishOrderAbnormalList = LineProductionData.Select(c => c.AbnormalCount).ToList();
                //    foreach (var item in LineTodayFinishOrderAbnormalList)
                //    {
                //        if (LineTodayFinishOrderAbnormal == "")
                //        {
                //            LineTodayFinishOrderAbnormal = item.ToString();
                //        }
                //        else
                //        {
                //            LineTodayFinishOrderAbnormal = LineTodayFinishOrderAbnormal + "," + item;
                //        }
                //    }
                //    SMT_LineInfo.Add("LineTodayFinishOrderAbnormal", LineTodayFinishOrderAbnormal);//产线i今天生产订单不良品

                //    //产线i今天计划生产订单：
                //    var TodayPlanOrder = db.SMT_ProductionPlan.Where(c => c.LineNum == i && DbFunctions.DiffDays(c.CreateDate, DateTime.Now) == 0).ToList();
                //    JObject LinePlanOrder = new JObject();
                //    //产线i今天计划生产订单、数量、平台、客户
                //    int x = 0;
                //    foreach (var item in TodayPlanOrder)
                //    {
                //        LinePlanOrder.Add(x++.ToString(), item.OrderNum + "," + item.Quantity + "," + item.JobContent + "," + item.Remark);
                //    }
                //    SMT_LineInfo.Add("LinePlanOrder", LinePlanOrder);

                //    SMT_LineInfo.Add("Date", DateTime.Now.ToString());//产线i日期
                //    SMT_LineInfo.Add("Team", LineInfo.Team);//产线i的班组
                //    SMT_LineInfo.Add("GroupLeader", LineInfo.GroupLeader);//产线i组长
                //    SMT_LineInfo.Add("LineStatus", LineInfo.Status);//产线i状态
                //    SMT_LineInfo.Add("LineDoingOrder", LineInfo.ProducingOrderNum);//产线i正在生产订单
                //    SMT_LineInfo.Add("LineTotalQuantity", LineProductionData.Where(c => c.OrderNum == LineInfo.ProducingOrderNum).Sum(c => c.NormalCount + c.AbnormalCount));//产线i今天生产数量汇总
                //    SMT_LineInfo.Add("TotalFinishQuantity", TotalFinishQuantity);//产线累计生产数量
                //    SMT_LineInfo.Add("OrderTotalFinishQuantity", OrderTotalFinishQuantity);//产线i正在生产的订单累计生产数量

                //    SMT_Board.Add(i.ToString(), SMT_LineInfo);//产线i的看板信息   1.1.1 JSON-->1.1 JSON
                //}

                #endregion


                #region-------------------各条产线SMT_Operator操作员JSON       (待修改)
                //foreach (var i in SMT_LineList)
                //{
                //    JObject SMT_LineInfo = new JObject();//各条产线JSON对象  1.1.1 JSON

                //    LineInfo = SMT_ProductionLineInfos.Where(c => c.LineNum == i).ToList().FirstOrDefault();//产线i信息
                //    LineProductionData = Today_SMT_ProductionDatas.Where(c => c.LineNum == i).ToList();//产线i生产数据
                //    int TotalFinishQuantity = Today_SMT_ProductionDatas.Where(c => c.LineNum == i).Sum(c => c.NormalCount + c.AbnormalCount); //产线累计生产数量
                //    int OrderTotalFinishQuantity = Today_SMT_ProductionDatas.Where(c => c.LineNum == i && c.OrderNum == LineInfo.ProducingOrderNum).Sum(c => c.NormalCount + c.AbnormalCount); //产线i正在生产的订单累计生产数量

                //    //产线i今天生产订单   //订单数量
                //    string LineTodayFinishOrder = "";
                //    string OrderQuantity = "";
                //    var LineTodayFinishOrderList = LineProductionData.Select(c => c.OrderNum).ToList();
                //    var OrderList = SMT_OrderInfos.Select(c => c.OrderNum);
                //    foreach (var item in LineTodayFinishOrderList)
                //    {
                //        //订单数量
                //        foreach (var order in OrderList)
                //        {
                //            if (item == order)
                //            {
                //                if (OrderQuantity == "")
                //                {
                //                    OrderQuantity = SMT_OrderInfos.Where(c => c.OrderNum == order).FirstOrDefault().Models.ToString();
                //                }
                //                else
                //                {
                //                    OrderQuantity = OrderQuantity + "," + SMT_OrderInfos.Where(c => c.OrderNum == order).FirstOrDefault().Models.ToString();
                //                }
                //            }
                //        }
                //        //产线i今天生产订单
                //        if (LineTodayFinishOrder == "")
                //        {
                //            LineTodayFinishOrder = item;
                //        }
                //        else
                //        {
                //            LineTodayFinishOrder = LineTodayFinishOrder + "," + item;
                //        }
                //    }
                //    SMT_LineInfo.Add("LineTodayFinishOrder", LineTodayFinishOrder);//产线i今天生产订单
                //    SMT_LineInfo.Add("OrderQuantity", OrderQuantity);//订单数量


                //    //产线i今天生产订单合格品
                //    string LineTodayFinishOrderNormal = "";
                //    var LineTodayFinishOrderNormalList = LineProductionData.Select(c => c.NormalCount);
                //    foreach (var item in LineTodayFinishOrderNormalList)
                //    {
                //        if (LineTodayFinishOrderNormal == "")
                //        {
                //            LineTodayFinishOrderNormal = item.ToString();
                //        }
                //        else
                //        {
                //            LineTodayFinishOrderNormal = LineTodayFinishOrderNormal + "," + item;
                //        }
                //    }
                //    SMT_LineInfo.Add("LineTodayFinishOrderNormal", LineTodayFinishOrderNormal);//产线i今天生产订单合格品

                //    //产线i今天生产订单不良品
                //    string LineTodayFinishOrderAbnormal = "";
                //    var LineTodayFinishOrderAbnormalList = LineProductionData.Select(c => c.AbnormalCount).ToList();
                //    foreach (var item in LineTodayFinishOrderAbnormalList)
                //    {
                //        if (LineTodayFinishOrderAbnormal == "")
                //        {
                //            LineTodayFinishOrderAbnormal = item.ToString();
                //        }
                //        else
                //        {
                //            LineTodayFinishOrderAbnormal = LineTodayFinishOrderAbnormal + "," + item;
                //        }
                //    }
                //    SMT_LineInfo.Add("LineTodayFinishOrderAbnormal", LineTodayFinishOrderAbnormal);//产线i今天生产订单不良品

                //    //产线i今天计划生产订单
                //    string LinePlanOrder = "";
                //    var LinePlanOrderList = SMT_ProductionPlans.Where(c => c.LineNum == i).Select(c => c.OrderNum).ToList();
                //    foreach (var item in LinePlanOrderList)
                //    {
                //        if (LinePlanOrder == "")
                //        {
                //            LinePlanOrder = item;
                //        }
                //        else
                //        {
                //            LinePlanOrder = LinePlanOrder + "," + item;
                //        }
                //    }
                //    SMT_LineInfo.Add("LinePlanOrder", LinePlanOrder);//产线i今天计划生产订单

                //    SMT_LineInfo.Add("LineDoingOrder", LineInfo.ProducingOrderNum);//产线i正在生产订单
                //    SMT_LineInfo.Add("LineTodayFinishOrderQuantity", LineProductionData.Sum(c => c.NormalCount + c.AbnormalCount));//产线i今天生产订单数量
                //    SMT_LineInfo.Add("LineTotalQuantity", OrderTotalFinishQuantity);//产线i累计数量
                //    SMT_LineInfo.Add("Team", LineInfo.Team);//产线i的班组
                //    SMT_LineInfo.Add("GroupLeader", LineInfo.GroupLeader);//产线i组长
                //    SMT_LineInfo.Add("LineStatus", LineInfo.Status);//产线i状态

                //    SMT_Operator.Add(i.ToString(), SMT_LineInfo);//产线i的看板信息   1.1.1 JSON-->1.1 JSON
                //}
                #endregion
            }

            //广播发送JSON数据
            //SMT_hubContext.Clients.All.sendSMT_Manage(SMT_Manage);
            //SMT_hubContext.Clients.All.sendSMT_Board(SMT_Board);
            SMT_hubContext.Clients.All.sendSMT_ProductionInfo(SMT_ProductionInfo);
            //SMT_hubContext.Clients.All.sendSMT_ProductionInfo(SMT_ProductionInfo);
            //SMT_hubContext.Clients.All.sendSMT_ProductionInfo_old(SMT_ProductionInfo_old);
            //SMT_hubContext.Clients.All.sendSMT_Operator(SMT_Operator);
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