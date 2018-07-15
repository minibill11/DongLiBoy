using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using JianHeMES.Models;
using Newtonsoft.Json;
using JianHeMES.Areas.KongYaHT.Models;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace JianHeMES.Areas.kongya.Controllers
{
    public class KongYaController : Controller
    {
        private kongyadbEntities db =new kongyadbEntities();


        #region ------------------------实时信息页面------------------------------
        // GET: kongya/KongYa
        public ActionResult KongYaIndex()
        {
            return View();
        }
        public ActionResult ThirdFloor()
        {
            return View();
        }
        public ActionResult FourthFloor()
        {
            return View();
        }

        public ActionResult FifthFloor()
        {
            return View();
        }

        public ActionResult Sixthfloor()
        {
            return View();
        }

        public ActionResult FloorDataShow()
        {
            return View();
        }

        #endregion



        #region    ---------------------Charts图表---------------------------------------------
        public ActionResult test1(string id,string xx)
        {
            return Content(xx+"成功了。"+id);
        }
        [HttpPost]
        public ActionResult Test(string point)
        {
            string ResultJson;
            kongyadbEntities db = new kongyadbEntities();

            IQueryable<THhistory> firstRecord = null;
            switch (point)
            {
                case "三楼面罩":
                    firstRecord = (from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "10")
                                   orderby m.id descending
                                   select m).Take(100);
                    ResultJson = "这里是面罩";
                    break;
                case "三楼插件":
                    firstRecord = (from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "9")
                                   orderby m.id descending
                                   select m).Take(100);
                    ResultJson = JsonConvert.SerializeObject(firstRecord);
                    break;
                case "三楼烤箱房":
                    firstRecord = (from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "6")
                                   orderby m.id descending
                                   select m).Take(2);
                    ResultJson = JsonConvert.SerializeObject(firstRecord);
                    break;
                default: ResultJson = "这里是Test的default:" + point; break;
            }
            ViewData["ResultJson"] = ResultJson;
            ViewData["ResultJson2"] = firstRecord;
            return Content(ResultJson);
        }



        [HttpGet]
        public ActionResult HTCharts(string point)
        {
            //Floor  point     DeviceID  NodeID
            //三楼   面罩      40001676   10
            //三楼   插件      40001676   9
            //三楼   烤箱房    40001676   6
            //四楼
            //四楼
            //四楼
            //五楼
            //五楼
            //五楼
            //五楼
            //五楼
            //五楼
            //五楼
            //六楼  空压房

            //string ResultJson=null;
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<THhistory> firstRecords = null;
            switch (point)
            {
                case "三楼烤箱房":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "6")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "三楼插件":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "9")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "三楼面罩":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "10")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "四楼烤箱房":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "1")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "四楼老化":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "2")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼组装":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "3")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼线材":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "4")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "5")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT物料暂存":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "6")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓7":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "7")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓8":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "8")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓9":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "9")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓10":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "10")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓11":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "11")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓12":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "12")
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
            }

            #region 将对象转为列矩阵JSON
            List<Double> TemList = new List<double>();
            List<Double> HumList = new List<double>();
            List<DateTime> RecordTimeList = new List<DateTime>();
            foreach (var firstRecord in firstRecords)
            {
                TemList.Add(Convert.ToDouble(firstRecord.Tem));
                HumList.Add(Convert.ToDouble(firstRecord.Hum));
                RecordTimeList.Add(Convert.ToDateTime(firstRecord.RecordTime));
            }
            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            JObject ResultJsonObj = new JObject //创建JSON对象
            {
                { "Tem",JsonConvert.SerializeObject(TemList)},
                { "Hum",  JsonConvert.SerializeObject(HumList) },
                { "RecordTime", JsonConvert.SerializeObject(RecordTimeList,iso).Replace("\"","")},
            };
            ViewData["ResultJsonObj"] = ResultJsonObj;
            #endregion
            //ViewData["ResultJsonObj"] = ResultJsonObj;
            return View();
            //return Json(ResultJsonObj,JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult HTCharts(string point, DateTime left)
        {
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<THhistory> queryRecords = null;
            switch (point)
            {
                case "三楼烤箱房":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "6" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "三楼插件":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "9" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "三楼面罩":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "10" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼烤箱房":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "1" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "2" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼组装":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "3" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼线材":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "4" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "5" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT物料暂存":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "6" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓7":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "7" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓8":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "8" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓9":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "9" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓10":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "10" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓11":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "11" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓12":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "12" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500);
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
            }

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
            ViewData["queryJsonObj"] = queryJsonObj;
            return View();
        }


        [HttpPost]
        public ActionResult HTCharts(string point, DateTime begin, DateTime end)
        {
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<THhistory> queryRecords = null;
            switch (point)
            {
                case "三楼烤箱房":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "6" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "三楼插件":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "9" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "三楼面罩":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "10" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼烤箱房":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "1" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "2" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼组装":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "3" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼线材":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "4" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "5" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT物料暂存":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "6" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓7":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "7" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓8":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "8" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓9":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "9" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓10":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "10" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓11":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "11" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓12":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "12" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
            }

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
            ViewData["queryJsonObj"] = queryJsonObj;
            return View();
        }
        [HttpPost]
        public ActionResult Sixthfloor(DateTime a,DateTime b)
        {
            kongyadbEntities db =new kongyadbEntities();
            IQueryable<room> rooms = null;
            TimeSpan sectionTime = new TimeSpan(0,0,30,0);
            if (a!=null)
            {
                DateTime get_a = a;
                DateTime get_earler_a;
                get_earler_a = get_a - sectionTime;//时间运行，求更早的值
                rooms = from m in db.room
                        where (m.recordingTime >get_earler_a && m.recordingTime<get_a)
                        select m;
            }
            return View(rooms);
        }
        #endregion 
    }
}