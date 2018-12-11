using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json;
using JianHeMES.Hubs;
using JianHeMES.Areas.KongYaHT.Models;
using Newtonsoft.Json.Linq;

namespace JianHeMES.Controllers
{
    public class TestController : Controller
    {
        private kongyadbEntities db = new kongyadbEntities();
        private ApplicationDbContext mesdb = new ApplicationDbContext();

        // GET: Test
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult b_a_list()
        {
            string ordernum = "2018-YA403-4";
            var list = mesdb.BarCodes.Where(c => c.OrderNum == ordernum).ToList();
            List<string> barcodeslist = new List<string>();
            var appearancefinistlist = mesdb.Appearance.Where(c => c.OrderNum == ordernum && c.OQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
            var burn_in = mesdb.Burn_in.Where(c => c.OrderNum == ordernum && c.OQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
            var result = appearancefinistlist.Except(burn_in).ToList();
            ViewBag.resultlist = result;
            return View(result);
        }

        [HttpPost]
        public ActionResult b_a_list(string ordernum)
        {
            var list = mesdb.BarCodes.Where(c => c.OrderNum == ordernum).ToList();
            List<string> barcodeslist = new List<string>();
            var appearancefinistlist = mesdb.Appearance.Where(c => c.OrderNum == ordernum && c.OQCCheckFinish==true).Select(c => c.BarCodesNum).ToList();
            var burn_in = mesdb.Burn_in.Where(c => c.OrderNum == ordernum && c.OQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
            var result = appearancefinistlist.Except(burn_in).ToList();
            ViewBag.resultlist = result;
            return View(result);
        }


        public ActionResult CSVtest()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CSVtest(Array file)
        {

            return View();
        }


        [HttpPost]
        public ActionResult Index(string jsondata)   //接收jsondata
        {
            CalibrationRecord data = JsonConvert.DeserializeObject<CalibrationRecord>(jsondata);   //使用JsonConvert.DeserializeObject<类名>（字符串）来把传过来的字符串解析为对应的类
            return View();
        }

        [HttpPost]
        public ActionResult ElectVal()   //接收jsondata
        {
            var value = db.aircomp3.OrderByDescending(c => c.recordingTime).FirstOrDefault().current_u.ToString();
            return Content(value);
        }


        [HttpPost]
        public ActionResult HTChartsLeft(string point, DateTime left)
        {
            IQueryable<THhistory> queryRecords = null;

            queryRecords = (from m in db.THhistory
                            where (m.DeviceID == "40004493" && m.NodeID == "1" && m.RecordTime < left)
                            orderby m.id descending
                            select m).Take(100).OrderBy(m => m.RecordTime);
            if (queryRecords.Count() == 0)
            {
                return Content("无数据");
            }
            ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
            queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });

            #region ---------------将对象转为列矩阵JSON
            List<Double> TemList = new List<double>();
            List<Double> HumList = new List<double>();
            List<DateTime> RecordTimeList = new List<DateTime>();
            foreach (var firstRecord in queryRecords)
            {
                TemList.Add(Convert.ToDouble(firstRecord.Tem));
                HumList.Add(Convert.ToDouble(firstRecord.Hum));
                RecordTimeList.Add(Convert.ToDateTime(firstRecord.RecordTime));
            }
            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            JObject queryJsonObj = new JObject
            {
                { "Tem", JsonConvert.SerializeObject(TemList) },
                { "Hum", JsonConvert.SerializeObject(HumList) },
                { "RecordTime", JsonConvert.SerializeObject(RecordTimeList,iso).Replace("\"","")},
            };   //创建JSON对象
            #endregion

            ViewData["queryJsonObj"] = queryJsonObj;  //输出JSON
            return Content(JsonConvert.SerializeObject(queryJsonObj));
        }

    }
}