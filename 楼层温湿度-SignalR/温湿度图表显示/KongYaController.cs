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

        
        #region    ---------------------HTCharts图表---------------------------------------------

        //首次打开页面
        [HttpGet]
        public ActionResult HTCharts(string point= "三楼面罩")   
        {
            #region 监测点信息
            //Floor  point        DeviceID  NodeID
            //三楼   烤箱房       40001676   6
            //三楼   插件         40001676   9
            //三楼   面罩         40001676   10
            //四楼   烤箱房       40001676   1
            //四楼   老化         40001676   2
            //四楼   组装         40001676   3
            //四楼   线材         40001676   4
            //四楼   老化调试     40001676   5
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT物料暂存  40000938   6
            //五楼   电子仓7      40000938   7
            //五楼   电子仓8      40000938   8
            //五楼   电子仓9      40000938   9
            //五楼   电子仓10     40000938   10       
            //五楼   电子仓11     40000938   11
            //五楼   电子仓12     40000938   12
            //六楼  空压房
            #endregion

            //string ResultJson=null;
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<THhistory> firstRecords = null;
            #region   ---------------选择器------------------
            switch (point)
            {
                case "三楼烤箱房":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "6")
                                    orderby m.id descending
                                    select m).Take(10).OrderBy(m => m.RecordTime);
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
            #endregion

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
            #endregion
            ViewData["ResultJsonObj"] = ResultJsonObj;  //输出JSON
           return View();
            //return Json(ResultJsonObj,JsonRequestBehavior.AllowGet);
        }

        //向左移动加载数据
        [HttpPost]
        public ActionResult HTChartsLeft(string point, DateTime left)
        {
            #region 监测点信息
            //Floor  point        DeviceID  NodeID
            //三楼   烤箱房       40001676   6
            //三楼   插件         40001676   9
            //三楼   面罩         40001676   10
            //四楼   烤箱房       40001676   1
            //四楼   老化         40001676   2
            //四楼   组装         40001676   3
            //四楼   线材         40001676   4
            //四楼   老化调试     40001676   5
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT物料暂存  40000938   6
            //五楼   电子仓7      40000938   7
            //五楼   电子仓8      40000938   8
            //五楼   电子仓9      40000938   9
            //五楼   电子仓10     40000938   10       
            //五楼   电子仓11     40000938   11
            //五楼   电子仓12     40000938   12
            //六楼  空压房

            #endregion            kongyadbEntities db = new kongyadbEntities();
            IQueryable<THhistory> queryRecords = null;
            //DateTime lefttime =new DateTime(left);
            #region   ---------------选择器------------------
            switch (point)
            {
                case "三楼烤箱房40001676#6":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "6" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(30).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "三楼插件40001676#9":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "9" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "三楼面罩40001676#10":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "10" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼烤箱房40001676#1":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "1" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化40001676#2":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "2" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼组装40001676#3":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "3" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼线材40001676#4":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "4" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试40001676#5":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "5" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT物料暂存40000938#6":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "6" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#7":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "7" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#8":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "8" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#9":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "9" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#10":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "10" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#11":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "11" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#12":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "12" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
            }
            #endregion

            #region 将对象转为列矩阵JSON
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
            //return View();
            return Content(JsonConvert.SerializeObject(queryJsonObj));
        }

        //向右移动加载数据
        [HttpPost]
        public ActionResult HTChartsRight(string point, DateTime right)
        {
            #region 监测点信息
            //Floor  point        DeviceID  NodeID
            //三楼   烤箱房       40001676   6
            //三楼   插件         40001676   9
            //三楼   面罩         40001676   10
            //四楼   烤箱房       40001676   1
            //四楼   老化         40001676   2
            //四楼   组装         40001676   3
            //四楼   线材         40001676   4
            //四楼   老化调试     40001676   5
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT物料暂存  40000938   6
            //五楼   电子仓7      40000938   7
            //五楼   电子仓8      40000938   8
            //五楼   电子仓9      40000938   9
            //五楼   电子仓10     40000938   10       
            //五楼   电子仓11     40000938   11
            //五楼   电子仓12     40000938   12
            //六楼  空压房
            #endregion
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<THhistory> queryRecords = null;
            #region   ---------------选择器------------------
            switch (point)
            {
                case "三楼烤箱房40001676#6":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "6" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(10).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "三楼插件40001676#9":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "9" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "三楼面罩40001676#10":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "10" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼烤箱房40001676#1":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "1" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化40001676#2":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "2" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼组装40001676#3":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "3" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼线材40001676#4":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "4" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试40001676#5":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "5" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT物料暂存40000938#6":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "6" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#7":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "7" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#8":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "8" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#9":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "9" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#10":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "10" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#11":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "11" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#12":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "12" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
            }
            #endregion

            #region 将对象转为列矩阵JSON
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

        //按照给写时间段查询数据加载页面
        [HttpPost]
        public ActionResult HTChartsDuring(string point, DateTime begin, DateTime end)
        {
            #region 监测点信息
            //Floor  point        DeviceID  NodeID
            //三楼   烤箱房       40001676   6
            //三楼   插件         40001676   9
            //三楼   面罩         40001676   10
            //四楼   烤箱房       40001676   1
            //四楼   老化         40001676   2
            //四楼   组装         40001676   3
            //四楼   线材         40001676   4
            //四楼   老化调试     40001676   5
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT
            //五楼   SMT物料暂存  40000938   6
            //五楼   电子仓7      40000938   7
            //五楼   电子仓8      40000938   8
            //五楼   电子仓9      40000938   9
            //五楼   电子仓10     40000938   10       
            //五楼   电子仓11     40000938   11
            //五楼   电子仓12     40000938   12
            //六楼  空压房
            #endregion
            //查询代码未完善
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<THhistory> queryRecords = null;
            #region   ---------------选择器------------------
            switch (point)
            {
                case "三楼烤箱房40001676#6":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "6" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "三楼插件40001676#9":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "9" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "三楼面罩40001676#10":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "10" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼烤箱房40001676#1":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "1" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化40001676#2":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "2" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼组装40001676#3":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "3" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼线材40001676#4":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "4" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试40001676#5":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "5" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT物料暂存40000938#6":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "6" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#7":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "7" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#8":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "8" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#9":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "9" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#10":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "10" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#11":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "11" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#12":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "12" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
            }
            #endregion

            #region  -----将对象转为列矩阵JSON---
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

        #endregion 

    }
}