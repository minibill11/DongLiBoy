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

        [HttpPost]
        public ActionResult Assemble(string OrderNum)
        {
            #region ---------------读取数据，处理数据

            ViewBag.OrderNum = OrderNum;//1.订单号

            var modelGroupQuantity = (from m in db.OrderMgm where m.OrderNum == OrderNum select m).ToList().FirstOrDefault().Boxes;//2.订单模组数
            var orderBoxBarCodeList = db.BarCodes.Where(m => m.OrderNum == OrderNum).Select(m => m.BarCodesNum).ToList();//订单的所有条码清单
            var assembleRecord = (from m in db.Assemble where m.OrderNum == OrderNum select m).OrderBy(x => x.BoxBarCode).ToList();//订单PQC全部记录
            var assembleRecordBarCodeList = assembleRecord.Select(m => m.BoxBarCode).Distinct().ToList();//PQC记录全部条码清单(去重)

            var finished = assembleRecord.Count(m => m.PQCCheckAbnormal == "正常");//3.订单已完成PQC个数
            var finishedList = assembleRecord.Where(m => m.PQCCheckAbnormal == "正常").Select(m => m.BoxBarCode).ToList(); //订单已完成PQC的条码清单

             var assemblePQC_Count = assembleRecord.Count();//4.订单PQC全部记录条数

            var finisthRate = (Convert.ToDouble(finished) / modelGroupQuantity * 100).ToString("F2");//5.完成率：完成数/订单的模组数

            var passRate = (Convert.ToDouble(finished) / assemblePQC_Count * 100).ToString("F2");//6.合格率：完成数/记录数

            var firstPassYield = assembleRecord.Where(x => x.BoxBarCode.Count()==1 ).ToList().Where(x=>x.PQCCheckAbnormal == "正常").ToList(); //直通记录(待验证)(未完成)
            //var firstPassYield2 = assembleRecord.GroupBy(c => c.BoxBarCode).Select(x => x.FirstOrDefault()); //直通记录(待验证)(未完成)

            var firstPassYieldCount = firstPassYield.Count();//8.直通数(待验证)(未完成)

            var firstPassYield_Rate = (Convert.ToDouble(firstPassYieldCount) / modelGroupQuantity * 100).ToString("F2");//7.直通率：直通数/模组数(待验证)(未完成)

            var abnormalList_temp = (from m in assembleRecord where m.PQCCheckAbnormal != "正常" orderby m.BoxBarCode select m).ToList();//10.异常记录清单
            var abnormalList = PutOutJson(abnormalList_temp);

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

            var abnormal_Count = (from m in assembleRecord where m.PQCCheckAbnormal != "正常" select m).Count();//9.异常次数

            //var finishedAnd2record_temp = assembleRecord.Where(x => x.BoxBarCode.Count() >= 2 ).ToList().Where(x=>x.PQCCheckAbnormal=="正常").ToList();//12.经过2次及以上PQC已完成的条码清单、个数(有问题，未完成)
            var finishedAnd2record_temp = assembleRecord.Where(x => x.BoxBarCode.Count()>=2).ToList();//12.经过2次及以上PQC已完成的条码清单、个数
            var finishedAnd2record = PutOutJson(finishedAnd2record_temp);

            var unfinishAndRecord_temp = assembleRecord.Where(x => x.BoxBarCode.Count() >= 1 && x.PQCCheckAbnormal != "正常").ToList(); //13.经过1次以上PQC未通过的条码清单、个数(有问题，未完成)
            var unfinishAndRecord = PutOutJson(unfinishAndRecord_temp);

            var unbeginRecord_temp = orderBoxBarCodeList.ToArray().Except(finishedList.ToArray()).ToList();//14.未开始PQC的条码清单、个数
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

            var going_temp = assembleRecord.Where(x => x.PQCCheckBT != null && x.PQCCheckFT == null).ToList();//15.正在进行PQC的条码清单、个数
            var going = PutOutJson(going_temp);

            var passed_temp = assembleRecord.Where(x => x.PQCCheckAbnormal == "正常").ToList();//16.已经完成PQC的条码清单、个数
            var passed = PutOutJson(passed_temp);
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


        public JObject PutOutJson(List<Assemble> inputlist)
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

        public List<Assemble> PutOutList(List<Assemble> inputlist)
        {
            List<Assemble> OutPutList = (from m in inputlist select new Assemble
            {
                Id = m.Id,
                OrderNum = m.OrderNum,
                BarCode_Prefix = m.BarCode_Prefix,
                BoxBarCode = m.BoxBarCode,
                AssembleBT = m.AssembleBT,
                AssemblePrincipal = m.AssemblePrincipal,
                AssembleFT = m.AssembleFT,
                ModelList = m.ModelList,
                AssembleTime = m.AssembleTime,
                AssembleFinish = m.AssembleFinish,
                WaterproofTestBT = m.WaterproofTestBT,
                WaterproofTestPrincipal = m.WaterproofTestPrincipal,
                WaterproofTestFT = m.WaterproofTestFT,
                WaterproofTestTime = m.WaterproofTestTime,
                WaterproofAbnormal = m.WaterproofAbnormal,
                WaterproofMaintaince = m.WaterproofMaintaince,
                WaterproofTestFinish = m.WaterproofTestFinish,
                AssembleAdapterCardBT = m.AssembleAdapterCardBT,
                AssembleAdapterCardPrincipal = m.AssembleAdapterCardPrincipal,
                AssembleAdapterCardFT = m.AssembleAdapterCardFT,
                AssembleAdapterTime = m.AssembleAdapterTime,
                AssembleAdapterFinish = m.AssembleAdapterFinish,
                AdapterCard_Power_Collection = m.AdapterCard_Power_Collection,
                ViewCheckBT = m.ViewCheckBT,
                AssembleViewCheckPrincipal = m.AssembleViewCheckPrincipal,
                ViewCheckFT = m.ViewCheckFT,
                ViewCheckTime = m.ViewCheckTime,
                ViewCheckAbnormal = m.ViewCheckAbnormal,
                ViewCheckFinish = m.ViewCheckFinish,
                ElectricityCheckBT = m.ElectricityCheckBT,
                AssembleElectricityCheckPrincipal = m.AssembleElectricityCheckPrincipal,
                ElectricityCheckFT = m.ElectricityCheckFT,
                ElectricityCheckTime = m.ElectricityCheckTime,
                ElectricityCheckAbnormal = m.ElectricityCheckAbnormal,
                ElectricityCheckFinish = m.ElectricityCheckFinish,
                AssembleLineId = m.AssembleLineId,
                AdapterCard_Power_List = m.AdapterCard_Power_List,
                PQCCheckBT = m.PQCCheckBT,
                AssemblePQCPrincipal = m.AssemblePQCPrincipal,
                PQCCheckFT = m.PQCCheckFT,
                PQCCheckTime = m.PQCCheckTime,
                PQCCheckAbnormal = m.PQCCheckAbnormal,
                PQCRepairCondition = m.PQCRepairCondition,
                PQCCheckFinish = m.PQCCheckFinish
            }).ToList();
            return OutPutList;
        }

        public string ObjectToJSON(List<Assemble> objList)
        {
            string Jsonstring = null;
            DataContractJsonSerializer json = new DataContractJsonSerializer(objList.GetType());
            //序列化
            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, objList);
                Jsonstring = Encoding.UTF8.GetString(stream.ToArray());
            }
            return Jsonstring;
        }
        
        public List<T> JsonToList<T>(string jsonText)
        {
            List<T> list = new List<T>();
            DataContractJsonSerializer _Json = new DataContractJsonSerializer(list.GetType());
            byte[] _Using = System.Text.Encoding.UTF8.GetBytes(jsonText);
            System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream(_Using);
            _MemoryStream.Position = 0;
            return (List<T>)_Json.ReadObject(_MemoryStream);
        }

        [HttpPost]
        public ActionResult Burn_in(string OrderNum)
        {

            return View();
        }


        [HttpPost]
        public ActionResult Calibration(string OrderNum)
        {

            return View();
        }


        [HttpPost]
        public ActionResult Appearances(string OrderNum)
        {

            return View();
        }


        [HttpPost]
        public ActionResult Assemble_CompeleteRate(string OrderNum, string Content)
        {

            return View();
        }

        public ActionResult Assemble_PassRate()
        {

            return View();
        }
        public ActionResult Burn_in_CompeleteRate()
        {

            return View();
        }

        public ActionResult Burn_in_PassRate()
        {

            return View();
        }
        public ActionResult Calibration_CompeleteRate()
        {

            return View();
        }

        public ActionResult Calibration_PassRate()
        {

            return View();
        }
        public ActionResult Appearances_CompeleteRate()
        {

            return View();
        }

        public ActionResult Appearances_PassRate()
        {

            return View();
        }

    }
}