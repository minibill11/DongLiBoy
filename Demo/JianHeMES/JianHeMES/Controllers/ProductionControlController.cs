using JianHeMES.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
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



        public ActionResult Index()
        {
            return View();
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
            var assembleRecord = (from m in db.Assemble where m.OrderNum == OrderNum select m).OrderBy(x => x.BoxBarCode).ToList();//订单PQC全部记录
            var assembleRecordBarCodeList = assembleRecord.Select(m => m.BoxBarCode).Distinct().ToList();//PQC记录全部条码清单(去重)

            var finished = assembleRecord.Count(m => m.PQCCheckAbnormal == "正常");//3.订单已完成PQC个数
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

            var abnormalList_temp1 = (from m in assembleRecord where m.PQCCheckAbnormal != "正常" orderby m.BoxBarCode select m).ToList();//10.异常记录清单(包含正在PQC)

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
                if (temp.Count()>=1 && temp.Count(c => c.PQCCheckFinish == true)==0)
                {
                    unfinishAndRecord_temp.AddRange(temp);
                }
            }
            unfinishAndRecord_temp = unfinishAndRecord_temp.Except(going_temp).ToList();
            var unfinishAndRecord = Assemble_PutOutJson(unfinishAndRecord_temp);
            #endregion


            var unbeginRecord_temp = orderBoxBarCodeList.ToArray().Except(finishedList.ToArray()).ToList().Except(going_temp.Select(c=>c.BoxBarCode).ToArray()).ToList();//14.未开始PQC的条码清单、个数(排除已完成（包含正常异常）、正在进行)

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
                if(unbeginRecord_temp.IndexOf(item)==unbeginRecord_temp.Count()-1)
                {
                    unbeginRecord = unbeginRecord + "\"]";
                }
            }

            var passed_temp = assembleRecord.Where(x => x.PQCCheckAbnormal == "正常").ToList();//16.已经完成PQC的条码清单、个数
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
                //17.异常信息统计
            };

            ViewBag.abnormalList_temp = abnormalList_temp;//10.异常记录清单
            ViewBag.finishedAnd2record_temp = finishedAnd2record_temp;//12.经过2次及以上PQC已完成的条码清单、个数
            ViewBag.unfinishAndRecord_temp = unfinishAndRecord_temp;//13.经过1次以上PQC未通过的条码清单、个数
            ViewBag.unbeginRecord_temp = unbeginRecord_temp;//14.未开始PQC的条码清单、个数
            ViewBag.going_temp = going_temp;//15.正在进行PQC的条码清单、个数
            ViewBag.passed_temp = passed_temp;//16.已经完成PQC的条码清单、个数
            ViewBag.JsonObj = JsonObj;

            #endregion
            return View();
        }

        public JObject Assemble_PutOutJson(List<Assemble> inputlist)  //Assemble记录转Json方法
        {
            JObject OutPutJson = new JObject();
            OutPutJson.Add("title", "[Id,OrderNum,BarCode_Prefix,BoxBarCode,AssembleBT,AssemblePrincipal,AssembleFT," +
                "ModelList,AssembleTime,AssembleFinish,WaterproofTestBT,WaterproofTestPrincipal,WaterproofTestFT," +
                "WaterproofTestTime,WaterproofAbnormal,WaterproofMaintaince,WaterproofTestFinish,AssembleAdapterCardBT," +
                "AssembleAdapterCardPrincipal,AssembleAdapterCardFT,AssembleAdapterTime,AssembleAdapterFinish," +
                "AdapterCard_Power_Collection,ViewCheckBT,AssembleViewCheckPrincipal,ViewCheckFT,ViewCheckTime," +
                "ViewCheckAbnormal,ViewCheckFinish,ElectricityCheckBT,AssembleElectricityCheckPrincipal,ElectricityCheckFT," +
                "ElectricityCheckTime,ElectricityCheckAbnormal,ElectricityCheckFinish,AssembleLineId," +
                "AdapterCard_Power_List,PQCCheckBT,AssemblePQCPrincipal,PQCCheckFT,PQCCheckTime,PQCCheckAbnormal," +
                "PQCRepairCondition,PQCCheckFinish]");
            foreach (var item in inputlist)
            {
                OutPutJson.Add((inputlist.IndexOf(item)+1).ToString(), "["+item.Id +","+ item.OrderNum + "," + item.BarCode_Prefix + "," + 
                    item.BoxBarCode + "," + item.AssembleBT + "," + item.AssemblePrincipal + "," + item.AssembleFT + "," + item.ModelList + "," +
                    item.AssembleTime + "," + item.AssembleFinish + "," + item.WaterproofTestBT + "," + item.WaterproofTestPrincipal + "," + 
                    item.WaterproofTestFT + "," + item.WaterproofTestTime + "," + item.WaterproofAbnormal + "," + item.WaterproofMaintaince + "," +
                    item.WaterproofTestFinish + "," + item.AssembleAdapterCardBT + "," + item.AssembleAdapterCardPrincipal + "," + 
                    item.AssembleAdapterCardFT + "," + item.AssembleAdapterTime + "," + item.AssembleAdapterFinish + "," + 
                    item.AdapterCard_Power_Collection + "," + item.ViewCheckBT + "," + item.AssembleViewCheckPrincipal + "," +
                    item.ViewCheckFT + item.ViewCheckTime + "," + item.ViewCheckAbnormal + "," + item.ViewCheckFinish + "," + 
                    item.ElectricityCheckBT + "," + item.AssembleElectricityCheckPrincipal + "," + item.ElectricityCheckFT + "," + 
                    item.ElectricityCheckTime + "," + item.ElectricityCheckAbnormal + "," + item.ElectricityCheckFinish + "," + 
                    item.AssembleLineId + "," + item.AdapterCard_Power_List + "," + item.PQCCheckBT + "," + item.AssemblePQCPrincipal + "," + 
                    item.PQCCheckFT + "," + item.PQCCheckTime + "," + item.PQCCheckAbnormal + "," + item.PQCRepairCondition + "," + item.PQCCheckFinish+"]");
            }
            return OutPutJson;
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
            var Burn_in_Record = (from m in db.Burn_in where m.OrderNum == OrderNum select m).OrderBy(x => x.BarCodesNum).ToList();//订单OQC全部记录
            var Burn_in_RecordBarCodeList = Burn_in_Record.Select(m => m.BarCodesNum).Distinct().ToList();//OQC记录全部条码清单(去重)

            var finished = Burn_in_Record.Count(m => m.OQCCheckFinish == true);//3.订单已完成OQC个数
            var finishedList = Burn_in_Record.Where(m => m.OQCCheckFinish == true).Select(m => m.BarCodesNum).ToList(); //订单已完成OQC的条码清单

            var Burn_in_OQC_Count = Burn_in_Record.Count();//4.订单OQC全部记录条数

            var finisthRate = (Convert.ToDouble(finished) / modelGroupQuantity * 100).ToString("F2");//5.完成率：完成数/订单的模组数

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

            var passed_temp = Burn_in_Record.Where(x => x.OQCCheckFinish==true).ToList();//16.已经完成OQC的条码清单、个数
            var passed = Burn_in_PutOutJson(passed_temp);
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
                //17.异常信息统计
            };

            ViewBag.abnormalList_temp = abnormalList_temp;//10.异常记录清单
            //ViewBag.finishedAnd2record_temp = finishedAnd2record_temp;//12.经过2次及以上OQC已完成的条码清单、个数
            //ViewBag.unfinishAndRecord_temp = unfinishAndRecord_temp;//13.经过1次以上OQC未通过的条码清单、个数
            ViewBag.unbeginRecord_temp = unbeginRecord_temp;//14.未开始OQC的条码清单、个数
            ViewBag.going_temp = going_temp;//15.正在进行OQC的条码清单、个数
            ViewBag.passed_temp = passed_temp;//16.已经完成OQC的条码清单、个数
            ViewBag.JsonObj = JsonObj;

            #endregion

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
            var orderBoxBarCodeList = db.BarCodes.Where(m => m.OrderNum == OrderNum).Select(m => m.BarCodesNum).ToList();//订单的所有条码清单(值为空)
            var Calibration_Record = (from m in db.CalibrationRecord where m.OrderNum == OrderNum select m).OrderBy(x => x.ModuleGroupNum).ToList();//订单校正全部记录
            var Calibration_RecordBarCodeList = Calibration_Record.Select(m => m.ModuleGroupNum).Distinct().ToList();//校正记录全部条码(模组号)清单(去重)

            var finished = Calibration_Record.Count(m => m.Normal == true);//3.订单已完成校正个数
            var finishedList = Calibration_Record.Where(m => m.Normal == true).Select(m => m.ModuleGroupNum).ToList(); //订单已完成校正的条码(模组号)清单

            var Calibration_Count = Calibration_Record.Count();//4.订单校正全部记录条数

            var finisthRate = (Convert.ToDouble(finished) / modelGroupQuantity * 100).ToString("F2");//5.完成率：完成数/订单的模组数

            var passRate = (Convert.ToDouble(finished) / Calibration_Count * 100).ToString("F2");//6.合格率：完成数/记录数

            #region ---------------------一次直通记录、个数、直通率----------------------
            //---------------------一次直通记录----------------------
            var Calibration_Record_abnormal_BoxBarCode_list = Calibration_Record.Where(c=>c.AbnormalDescription != "正常").Select(c => c.ModuleGroupNum).ToList();//异常记录(模组号)清单
            var firstPassYield_temp = Calibration_Record.DistinctBy(c => c.ModuleGroupNum).Where(c=>c.AbnormalDescription == "正常" && c.Normal == true).ToList();//Finish记录
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

            var unbeginRecord_temp = orderBoxBarCodeList.Except(finishedList).ToList().Except(going_temp.Select(c => c.ModuleGroupNum)).ToList();//14.未开始校正的条码清单、个数(排除已完成（包含正常异常）、正在进行)

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

            var passed_temp = Calibration_Record.Where(x => x.AbnormalDescription == "正常" || x.AbnormalDescription == null && x.Normal == true ).ToList();//16.已经完成校正的条码清单、个数
            var passed = Calibration_PutOutJson(passed_temp);
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
                    item.AbnormalDescription + ","  + item.Normal + "]");
            }
            return OutPutJson;
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
            var orderBoxBarCodeList = db.BarCodes.Where(m => m.OrderNum == OrderNum).Select(m => m.BarCodesNum).ToList();//订单的所有条码清单
            var Appearance_Record = (from m in db.Appearance where m.OrderNum == OrderNum select m).OrderBy(x => x.BarCodesNum).ToList();//订单外观包装OQC全部记录
            var Appearance_RecordBarCodeList = Appearance_Record.Select(m => m.BarCodesNum).Distinct().ToList();//外观包装OQC记录全部条码清单(去重)

            var finished = Appearance_Record.Count(m => m.OQCCheckFinish == true);//3.订单已完成外观包装OQC个数
            var finishedList = Appearance_Record.Where(m => m.OQCCheckFinish == true).Select(m => m.BarCodesNum).ToList(); //订单已完成外观包装OQC的条码清单

            var Appearance_OQC_Count = Appearance_Record.Count();//4.订单外观包装OQC全部记录条数

            var finisthRate = (Convert.ToDouble(finished) / modelGroupQuantity * 100).ToString("F2");//5.完成率：完成数/订单的模组数

            var passRate = (Convert.ToDouble(finished) / Appearance_OQC_Count * 100).ToString("F2");//6.合格率：完成数/记录数

            #region ---------------------一次直通记录、个数、直通率----------------------
            //---------------------一次直通记录----------------------
            var Appearance_Record_abnormal_BoxBarCode_list = Appearance_Record.Where(c => c.RepairCondition != "正常" || c.Appearance_OQCCheckAbnormal != "正常").Select(c => c.BarCodesNum).ToList();//异常记录
            var firstPassYield_temp = Appearance_Record.DistinctBy(c => c.BarCodesNum).Where(c => c.Appearance_OQCCheckAbnormal == "正常" && c.RepairCondition == "正常" && c.OQCCheckFinish == true).ToList();//Finish记录
            List<Appearance> firstPassYield_expect = new List<Appearance>();//有异常记录的条码Finish记录
            foreach (var item in Appearance_Record_abnormal_BoxBarCode_list)
            {
                foreach (var burn_in_erecord in firstPassYield_temp)
                {
                    if (burn_in_erecord.BarCodesNum == item)
                    {
                        firstPassYield_expect.Add(burn_in_erecord);
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
                //17.异常信息统计
            };

            ViewBag.abnormalList_temp = abnormalList_temp;//10.异常记录清单
            //ViewBag.finishedAnd2record_temp = finishedAnd2record_temp;//12.经过2次及以上外观包装OQC已完成的条码清单、个数
            //ViewBag.unfinishAndRecord_temp = unfinishAndRecord_temp;//13.经过1次以上外观包装OQC未通过的条码清单、个数
            ViewBag.unbeginRecord_temp = unbeginRecord_temp;//14.未开始外观包装OQC的条码清单、个数
            ViewBag.going_temp = going_temp;//15.正在进行外观包装OQC的条码清单、个数
            ViewBag.passed_temp = passed_temp;//16.已经完成外观包装OQC的条码清单、个数
            ViewBag.JsonObj = JsonObj;

            #endregion

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

        #endregion

    }
}