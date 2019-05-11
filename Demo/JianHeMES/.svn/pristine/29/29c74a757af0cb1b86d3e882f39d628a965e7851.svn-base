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
using System.ComponentModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Net.Mail;
using System.Text;
using System.Web.Helpers;
using System.Diagnostics;

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
        public ActionResult FirstFloor()
        {
            return View();
        }

        public ActionResult SecondFloor()
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

        public ActionResult KYDataShow()
        {
            return View();
        }

        public ActionResult KYDataCheck()
        {
            return View();
        }
        #endregion


        #region    ---------------------全厂温湿度Charts图表---------------------------------------------

        //首次打开页面
        [HttpGet]
        public ActionResult HTCharts(string point= "三楼插件")   
        {
            #region 监测点信息
            //Floor  point              DeviceID  NodeID
            //一楼   一楼篮球场         40004493   1        2018-8-1新增
            //一楼   一楼配套加工(1)    40004518   1        2018-8-9新增
            //一楼   一楼配套加工(2)    40004518   2        2018-8-9新增
            //一楼   一楼配套加工(3)    40004518   3        2018-8-9新增
            //一楼   一楼配套加工(4)    40004518   4        2018-8-9新增
            //一楼   一楼配套加工(5)    40004518   5        2018-8-9新增
            //一楼   一楼配套加工(6)    40004518   6        2018-8-9新增
            //三楼   面罩               40021209   1
            //三楼   三楼仓库           40004493   2        2018-8-1新增
            //三楼   烤箱房             40001676   6
            //三楼   插件               40001676   9
            //四楼   四楼小间距装配     40004518   7        2018-8-28从40001676转到40004518
            //四楼   四楼组装线1号　    40021216   1        2019-3-13新增
            //四楼   四楼组装线2号      40021216   2　　　　2019-3-13新增
            //四楼   四楼喷墨房1号　    40021216   3        2018-12-3从四楼小间距装配迁移到四楼喷墨房1号
            //四楼   四楼喷墨房2号      40021216   4　　　　2018-12-3从四楼组装迁移到四楼喷墨房2号
            ////四楼   烤箱房             40001676   1        2019-3-13移除
            ////四楼   老化               40001676   2        2019-3-13移除
            //四楼   组装               40001676   3
            //四楼   线材               40001676   4
            //四楼   四楼老化调试       40001676   5
            //四楼   四楼老化调试2号    40004493   4        2018-8-1新增
            //四楼   四楼老化调试3号    40004493   5        2018-8-1新增
            //四楼   四楼仓库           40004493   3        2018-8-1新增
            //四楼   四楼小样房         40004493   7        2019-3-20新增
            //四楼   四楼首样房         40004493   8        2019-3-20新增
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT物料暂存        40000938   6
            //五楼   电子仓7            40000938   7
            //五楼   电子仓8            40000938   8
            //五楼   电子仓9            40000938   9
            //五楼   电子仓10           40000938   10       
            //五楼   电子仓11           40000938   11
            //五楼   电子仓12           40000938   12
            //六楼  空压房             
            #endregion

            //string ResultJson=null;
            kongyadbEntities db = new kongyadbEntities();
            List<THhistory> firstRecords = new List<THhistory>();
            int max_id = 0;
            #region   ---------------选择器------------------
            switch (point)
            {
                case "一楼篮球场": //一楼   一楼篮球场   40004493   1        2018-8-1新增
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "1")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "一楼配套加工(1)": //一楼   一楼配套加工1   40004518   1        2018-8-9新增
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "1")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "一楼配套加工(2)": //一楼   一楼配套加工2   40004518   2        2018-8-9新增
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "2")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "一楼配套加工(3)": //一楼   一楼配套加工3   40004518   3        2018-8-9新增
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "3")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "一楼配套加工(4)": //一楼   一楼配套加工4   40004518   4        2018-8-9新增
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "4")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "一楼配套加工(5)": //一楼   一楼配套加工5   40004518   5        2018-8-9新增
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "5")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "一楼配套加工(6)": //一楼   一楼配套加工6   40004518   6        2018-8-9新增
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "6")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;

                case "三楼烤箱房":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "6")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "三楼插件":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "9")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "三楼面罩":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021209" && m.NodeID == "1")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "三楼仓库":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "2")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "四楼喷墨房1号": 
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021216" && m.NodeID == "3" && m.DeviceName == "四楼喷墨房1号40021216#3")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.Tem, m.Hum, m.RecordTime });
                    break;
                case "四楼喷墨房2号":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021216" && m.NodeID == "4" && m.DeviceName == "四楼喷墨房2号40021216#4")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼组装线1号":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021216" && m.NodeID == "1" && m.DeviceName == "四楼组装线1号40021216#1")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼组装线2号":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021216" && m.NodeID == "2" && m.DeviceName == "四楼组装线2号40021216#2")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼线材":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "4")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼小样房":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "7" && m.DeviceName == "四楼小样房40004493#7")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼首样房":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "8" && m.DeviceName == "四楼首样房40004493#8")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "6")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试2号":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "4")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试3号":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "5")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼仓库":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "3")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT产线1":
                    firstRecords = db.THhistory.Where(c => c.DeviceID == "40021210" && c.NodeID == "1" && c.DeviceName == "五楼SMT产线40021210#1").OrderByDescending(c=>c.id).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT产线2":
                    max_id = db.THhistory.Max(c => c.id);
                    firstRecords = db.THhistory.Where(c => c.DeviceID == "40021210" && c.NodeID == "2" && c.DeviceName == "五楼SMT产线40021210#2").OrderByDescending(c => c.id).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT物料暂存":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "6")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓7":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "7")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓8":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "8")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓9":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "9")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓10":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "10")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓11":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "11")
                                    orderby m.id descending
                                    select m).Take(1500).ToList();
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓12":
                    firstRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40000938" && m.NodeID == "12")
                                    orderby m.id descending
                                    select m).Take(1500).ToList(); 
                    ViewBag.Station = firstRecords.FirstOrDefault().DeviceName;
                    firstRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
            }
            #endregion

            #region 将对象转为列矩阵JSON
            List<Double> TemList = new List<double>();
            List<Double> HumList = new List<double>();
            List<DateTime> RecordTimeList = new List<DateTime>();
            firstRecords = firstRecords.OrderBy(c => c.RecordTime).ToList();
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
            return View(ResultJsonObj);
            //return Json(ResultJsonObj,JsonRequestBehavior.AllowGet);
        }


        //向左移动加载数据
        [HttpPost]
        public ActionResult HTChartsLeft(string point, DateTime left)
        {
            #region 监测点信息
            //Floor  point              DeviceID  NodeID
            //一楼   一楼篮球场         40004493   1        2018-8-1新增
            //一楼   一楼配套加工(1)    40004518   1        2018-8-9新增
            //一楼   一楼配套加工(2)    40004518   2        2018-8-9新增
            //一楼   一楼配套加工(3)    40004518   3        2018-8-9新增
            //一楼   一楼配套加工(4)    40004518   4        2018-8-9新增
            //一楼   一楼配套加工(5)    40004518   5        2018-8-9新增
            //一楼   一楼配套加工(6)    40004518   6        2018-8-9新增
            //三楼   烤箱房             40001676   6
            //三楼   插件               40001676   9
            //三楼   面罩               40021209   1
            //三楼   三楼仓库           40004493   2        2018-8-1新增
            //四楼   四楼小间距装配     40004518   7        2018-8-28从40001676转到40004518
            //四楼   四楼组装线1号　    40021216   1        2019-3-13新增
            //四楼   四楼组装线2号      40021216   2　　　　2019-3-13新增
            //四楼   四楼喷墨房1号　    40021216   3        2018-12-3从四楼小间距装配迁移到四楼喷墨房1号
            //四楼   四楼喷墨房2号      40021216   4　　　　2018-12-3从四楼组装迁移到四楼喷墨房2号
            //四楼   烤箱房             40001676   1
            //四楼   老化               40001676   2
            //四楼   组装               40001676   3
            //四楼   线材               40001676   4
            //四楼   四楼老化调试       40001676   5
            //四楼   四楼老化调试2号    40004493   4        2018-8-1新增
            //四楼   四楼老化调试3号    40004493   5        2018-8-1新增
            //四楼   四楼仓库           40004493   3        2018-8-1新增
            //四楼   四楼小样房         40004493   7        2019-3-20新增
            //四楼   四楼首样房         40004493   8        2019-3-20新增
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT物料暂存        40000938   6
            //五楼   电子仓7            40000938   7
            //五楼   电子仓8            40000938   8
            //五楼   电子仓9            40000938   9
            //五楼   电子仓10           40000938   10       
            //五楼   电子仓11           40000938   11
            //五楼   电子仓12           40000938   12
            //六楼  空压房             
            #endregion
            IQueryable<THhistory> queryRecords = null;
            //DateTime lefttime =new DateTime(left);
            #region   ---------------选择器------------------
            switch (point)
            {
                case "一楼篮球场40004493#1":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "1" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(1)40004518#1":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "1" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(2)40004518#2":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "2" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(3)40004518#3":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "3" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(4)40004518#4":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "4" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(5)40004518#5":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "5" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(6)40004518#6":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "6" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼小间距装配40004518#7":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "7" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;

                case "三楼烤箱房40001676#6":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "6" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
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
                case "三楼面罩40021209#1":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021209" && m.NodeID == "1" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "3#三楼仓库40004493#2":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "2" && m.RecordTime < left)
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

                case "四楼组装线1号40021216#1":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021216" && m.NodeID == "1" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;

                case "四楼组装线1号40021216#2":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021216" && m.NodeID == "2" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼喷墨房1号40021216#3":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021216" && m.NodeID == "3" && m.DeviceName == "四楼喷墨房1号40021216#3" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼线材40021216#4":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021216" && m.NodeID == "4" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;

                case "四楼小样房40004493#7":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "7" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼首样房40004493#8":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "8" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试40004493#6":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "6" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试2号40004493#4":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "4" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试3号40004493#5":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "5" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "4#四楼仓库40004493#3":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "3" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT产线40021210#1":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021210" && m.NodeID == "1" && m.DeviceName == "五楼SMT产线40021210#1" && m.RecordTime < left)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT产线40021210#2":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021210" && m.NodeID == "2" && m.DeviceName=="五楼SMT产线40021210#2" && m.RecordTime < left)
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


        //向右移动加载数据
        [HttpPost]
        public ActionResult HTChartsRight(string point, DateTime right)
        {
            #region 监测点信息
            //Floor  point              DeviceID  NodeID
            //一楼   一楼篮球场         40004493   1        2018-8-1新增
            //一楼   一楼配套加工(1)    40004518   1        2018-8-9新增
            //一楼   一楼配套加工(2)    40004518   2        2018-8-9新增
            //一楼   一楼配套加工(3)    40004518   3        2018-8-9新增
            //一楼   一楼配套加工(4)    40004518   4        2018-8-9新增
            //一楼   一楼配套加工(5)    40004518   5        2018-8-9新增
            //一楼   一楼配套加工(6)    40004518   6        2018-8-9新增
            //三楼   烤箱房             40001676   6
            //三楼   插件               40001676   9
            //三楼   面罩               40021209   1
            //三楼   三楼仓库           40004493   2        2018-8-1新增
            //四楼   四楼小间距装配     40004518   7        2018-8-28从40001676转到40004518
            //四楼   四楼组装线1号　    40021216   1        2019-3-13新增
            //四楼   四楼组装线2号      40021216   2　　　　2019-3-13新增
            //四楼   四楼喷墨房1号　    40021216   3        2018-12-3从四楼小间距装配迁移到四楼喷墨房1号
            //四楼   四楼喷墨房2号      40021216   4　　　　2018-12-3从四楼组装迁移到四楼喷墨房2号
            //四楼   烤箱房             40001676   1
            //四楼   老化               40001676   2
            //四楼   组装               40001676   3
            //四楼   线材               40001676   4
            //四楼   四楼老化调试       40001676   5
            //四楼   四楼老化调试2号    40004493   4        2018-8-1新增
            //四楼   四楼老化调试3号    40004493   5        2018-8-1新增
            //四楼   四楼仓库           40004493   3        2018-8-1新增
            //四楼   四楼小样房         40004493   7        2019-3-20新增
            //四楼   四楼首样房         40004493   8        2019-3-20新增
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT物料暂存        40000938   6
            //五楼   电子仓7            40000938   7
            //五楼   电子仓8            40000938   8
            //五楼   电子仓9            40000938   9
            //五楼   电子仓10           40000938   10       
            //五楼   电子仓11           40000938   11
            //五楼   电子仓12           40000938   12
            //六楼  空压房             
            #endregion
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<THhistory> queryRecords = null;
            #region   ---------------选择器------------------
            switch (point)
            {
                case "一楼篮球场40004493#1":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "1" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(1)40004518#1":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "1" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(2)40004518#2":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "2" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(3)40004518#3":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "3" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(4)40004518#4":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "4" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(5)40004518#5":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "5" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(6)40004518#6":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "6" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼小间距装配40004518#7":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004518" && m.NodeID == "7" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;

                case "三楼烤箱房40001676#6":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40001676" && m.NodeID == "6" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count()==0)
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
                case "三楼面罩40021209#1":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021209" && m.NodeID == "1" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "3#三楼仓库40004493#2":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "2" && m.RecordTime > right)
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

                case "四楼组装线1号40021216#1":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021216" && m.NodeID == "1" && m.DeviceName == "四楼组装线1号40021216#1" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;

                case "四楼组装线2号40021216#2":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021216" && m.NodeID == "2" && m.DeviceName == "四楼组装线2号40021216#2" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;



                //四楼   四楼喷墨房1号　    40004518   7        2018-12-3从四楼小间距装配迁移到四楼喷墨房1号
                case "四楼喷墨房1号40021216#3":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021216" && m.NodeID == "3" && m.DeviceName == "四楼喷墨房1号40021216#3" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;


                //四楼   四楼喷墨房2号      40001676   3　　　　2018-12-3从四楼组装迁移到四楼喷墨房2号
                case "四楼喷墨房2号40021216#4":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021216" && m.NodeID == "4" && m.DeviceName == "四楼喷墨房2号40021216#4" && m.RecordTime > right)
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
                case "四楼小样房40004493#7":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "7" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼首样房40004493#8":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "8" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试40004493#6":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "6" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试2号40004493#4":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "4" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试3号40004493#5":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "5" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "4#四楼仓库40004493#3":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40004493" && m.NodeID == "3" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                // case "五楼SMT产线40000938#4":
                //    queryRecords = (from m in db.THhistory
                //                    where (m.DeviceID == "40000938" && m.NodeID == "4" && m.RecordTime > right)
                //                    orderby m.id
                //                    select m).Take(1500).OrderBy(m => m.RecordTime);
                //    if (queryRecords.Count() == 0)
                //    {
                //        return Content("无数据");
                //    }
                //    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                //    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                //    break;
                //case "五楼SMT产线40000938#5":
                //    queryRecords = (from m in db.THhistory
                //                    where (m.DeviceID == "40000938" && m.NodeID == "5" && m.RecordTime > right)
                //                    orderby m.id
                //                    select m).Take(1500).OrderBy(m => m.RecordTime);
                //    if (queryRecords.Count() == 0)
                //    {
                //        return Content("无数据");
                //    }
                //    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                //    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                //    break;
                case "五楼SMT产线40021210#1":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021210" && m.NodeID == "1" && m.RecordTime > right)
                                    orderby m.id
                                    select m).Take(1500).OrderBy(m => m.RecordTime);
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT产线40021210#2":
                    queryRecords = (from m in db.THhistory
                                    where (m.DeviceID == "40021210" && m.NodeID == "2" && m.RecordTime > right)
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


        //按照给写时间段查询数据加载页面
        [HttpPost]
        public ActionResult HTChartsDuring(string point, DateTime begin, DateTime end)
        {
            #region 监测点信息
            //Floor  point              DeviceID  NodeID
            //一楼   一楼篮球场         40004493   1        2018-8-1新增
            //一楼   一楼配套加工(1)    40004518   1        2018-8-9新增
            //一楼   一楼配套加工(2)    40004518   2        2018-8-9新增
            //一楼   一楼配套加工(3)    40004518   3        2018-8-9新增
            //一楼   一楼配套加工(4)    40004518   4        2018-8-9新增
            //一楼   一楼配套加工(5)    40004518   5        2018-8-9新增
            //一楼   一楼配套加工(6)    40004518   6        2018-8-9新增
            //三楼   烤箱房             40001676   6
            //三楼   插件               40001676   9
            //三楼   面罩               40021209   1
            //三楼   三楼仓库           40004493   2        2018-8-1新增
            //四楼   四楼小间距装配     40004518   7        2018-8-28从40001676转到40004518
            //四楼   四楼组装线1号　    40021216   1        2019-3-13新增
            //四楼   四楼组装线2号      40021216   2　　　　2019-3-13新增
            //四楼   四楼喷墨房1号　    40021216   3        2018-12-3从四楼小间距装配迁移到四楼喷墨房1号
            //四楼   四楼喷墨房2号      40021216   4　　　　2018-12-3从四楼组装迁移到四楼喷墨房2号
            //四楼   烤箱房             40001676   1
            //四楼   老化               40001676   2
            //四楼   组装               40001676   3
            //四楼   线材               40001676   4
            //四楼   四楼老化调试       40001676   5
            //四楼   四楼老化调试2号    40004493   4        2018-8-1新增
            //四楼   四楼老化调试3号    40004493   5        2018-8-1新增
            //四楼   四楼仓库           40004493   3        2018-8-1新增
            //四楼   四楼小样房         40004493   7        2019-3-20新增
            //四楼   四楼首样房         40004493   8        2019-3-20新增
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT物料暂存        40000938   6
            //五楼   电子仓7            40000938   7
            //五楼   电子仓8            40000938   8
            //五楼   电子仓9            40000938   9
            //五楼   电子仓10           40000938   10       
            //五楼   电子仓11           40000938   11
            //五楼   电子仓12           40000938   12
            //六楼  空压房             
            #endregion
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<THhistory> queryRecords = null;
            #region   ---------------选择器------------------
            switch (point)
            {
                case "一楼篮球场40004493#1":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "1" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(1)40004518#1":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004518" && m.NodeID == "1" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(2)40004518#2":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004518" && m.NodeID == "2" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(3)40004518#3":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004518" && m.NodeID == "3" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(4)40004518#4":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004518" && m.NodeID == "4" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(5)40004518#5":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004518" && m.NodeID == "5" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(6)40004518#6":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004518" && m.NodeID == "6" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼小间距装配40004518#7":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004518" && m.NodeID == "7" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;

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
                case "三楼面罩40021209#1":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021209" && m.NodeID == "1" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "3#三楼仓库40004493#2":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "2" && m.RecordTime > begin && m.RecordTime < end)
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

                case "四楼组装线1号40021216#1":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021216" && m.NodeID == "1" && m.DeviceName == "四楼组装线1号40021216#1" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;

                case "四楼组装线2号40021216#2":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021216" && m.NodeID == "2" && m.DeviceName == "四楼组装线2号40021216#2" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;



                //四楼   四楼喷墨房1号　    40004518   7        2018-12-3从四楼小间距装配迁移到四楼喷墨房1号
                case "四楼喷墨房1号40021216#3":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021216" && m.NodeID == "3" && m.DeviceName == "四楼喷墨房1号40021216#3" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;


                //四楼   四楼喷墨房2号      40001676   3　　　　2018-12-3从四楼组装迁移到四楼喷墨房2号
                case "四楼喷墨房2号40021216#4":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021216" && m.NodeID == "4" && m.DeviceName== "四楼喷墨房2号40021216#4" && m.RecordTime > begin && m.RecordTime < end)
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
                case "四楼小样房40004493#7":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "7" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼首样房40004493#8":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "8" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试40004493#6":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "6" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试2号40004493#4":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "4" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试3号40004493#5":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "5" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "4#四楼仓库40004493#3":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "3" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                //case "五楼SMT产线40000938#4":
                //    queryRecords = from m in db.THhistory
                //                   where (m.DeviceID == "40000938" && m.NodeID == "4" && m.RecordTime > begin && m.RecordTime < end)
                //                   orderby m.id
                //                   select m;
                //    if (queryRecords.Count() == 0)
                //    {
                //        return Content("无数据");
                //    }
                //    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                //    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                //    break;
                //case "五楼SMT产线40000938#5":
                //    queryRecords = from m in db.THhistory
                //                   where (m.DeviceID == "40000938" && m.NodeID == "5" && m.RecordTime > begin && m.RecordTime < end)
                //                   orderby m.id
                //                   select m;
                //    if (queryRecords.Count() == 0)
                //    {
                //        return Content("无数据");
                //    }
                //    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                //    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                //    break;
                case "五楼SMT产线40021210#1":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021210" && m.NodeID == "1" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT产线40021210#2":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021210" && m.NodeID == "2" && m.RecordTime > begin && m.RecordTime < end)
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

            #region  ---------------将对象转为列矩阵JSON---
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

        public class ResponseResult
        {
            public Double Tem;
            public Double Hum;
            public DateTime RcordTime;
        }
        #endregion


        #region    ---------------------空压房空压机Charts图表---------------------------------------------

        //首次打开空压机页面
        [HttpGet]
        public ActionResult KY_COMP_Charts(string point = "空压机1")
        {
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<aircomp1> firstRecords1 = null;
            IQueryable<aircomp2> firstRecords2 = null;
            IQueryable<aircomp3> firstRecords3 = null;
            #region   ---------------选择器------------------
            switch (point)
            {
                case "空压机1":
                    firstRecords1 = (from m in db.aircomp1
                                     where (m.recordingTime.Second == 0)
                                     orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "空压机1";
                    firstRecords1.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime});
                    break;
                case "空压机2":
                    firstRecords2 = (from m in db.aircomp2
                                     where (m.recordingTime.Second == 0)
                                     orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "空压机2";
                    firstRecords2.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    break;
                case "空压机3":
                    firstRecords3 = (from m in db.aircomp3
                                     where (m.recordingTime.Second == 0)
                                     orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "空压机3";
                    firstRecords3.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    break;
            }
            #endregion

            #region 将对象转为列矩阵JSON
            List<Double> PreList = new List<double>();
            List<Double> TemList = new List<double>();
            List<Double> EleList = new List<double>();
            List<DateTime> RecordTimeList = new List<DateTime>();
            if (firstRecords1 != null)
            {
                foreach (var firstRecord in firstRecords1)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    EleList.Add(Convert.ToDouble(firstRecord.current_u));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (firstRecords2 != null)
            {
                foreach (var firstRecord in firstRecords2)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    EleList.Add(Convert.ToDouble(firstRecord.current_u));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (firstRecords3 != null)
            {
                foreach (var firstRecord in firstRecords3)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    EleList.Add(Convert.ToDouble(firstRecord.current_u));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }

            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            JObject ResultJsonObj = new JObject //创建JSON对象
            {
                { "Pre",JsonConvert.SerializeObject(PreList)},
                { "Tem",JsonConvert.SerializeObject(TemList)},
                { "Ele",  JsonConvert.SerializeObject(EleList) },
                { "RecordTime", JsonConvert.SerializeObject(RecordTimeList,iso).Replace("\"","")},
            };
            #endregion

            ViewData["ResultJsonObj"] = ResultJsonObj;  //输出JSON
            return View(ResultJsonObj);
        }


        //向左移动加载数据
        [HttpPost]
        public ActionResult KY_COMP_ChartsLeft(string point, DateTime left)
        {
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<aircomp1> queryRecords1 = null;
            IQueryable<aircomp2> queryRecords2 = null;
            IQueryable<aircomp3> queryRecords3 = null;
            //DateTime lefttime =new DateTime(left);
            #region   ---------------选择器------------------
            switch (point)
            {
                case "空压机1":
                    queryRecords1 = (from m in db.aircomp1
                                    where (m.recordingTime < left && m.recordingTime.Second ==0)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords1.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "空压机1";
                    queryRecords1.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    break;
                case "空压机2":
                    queryRecords2 = (from m in db.aircomp2
                                     where (m.recordingTime < left && m.recordingTime.Second == 0)
                                     orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords2.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "空压机2";
                    queryRecords2.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    break;
                case "空压机3":
                    queryRecords3 = (from m in db.aircomp3
                                     where (m.recordingTime < left && m.recordingTime.Second == 0)
                                     orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords3.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "空压机3";
                    queryRecords3.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    break;
            }
            #endregion

            #region ---------------将对象转为列矩阵JSON
            List<Double> PreList = new List<double>();
            List<Double> TemList = new List<double>();
            List<Double> EleList = new List<double>();
            List<DateTime> RecordTimeList = new List<DateTime>();
            if (queryRecords1 != null)
            {
                foreach (var firstRecord in queryRecords1)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    EleList.Add(Convert.ToDouble(firstRecord.current_u));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords2 != null)
            {
                foreach (var firstRecord in queryRecords2)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    EleList.Add(Convert.ToDouble(firstRecord.current_u));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords3 != null)
            {
                foreach (var firstRecord in queryRecords3)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    EleList.Add(Convert.ToDouble(firstRecord.current_u));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            JObject queryJsonObj = new JObject
            {
                { "Pre",JsonConvert.SerializeObject(PreList)},
                { "Tem",JsonConvert.SerializeObject(TemList)},
                { "Ele",  JsonConvert.SerializeObject(EleList) },
                { "RecordTime", JsonConvert.SerializeObject(RecordTimeList,iso).Replace("\"","")},
            };   //创建JSON对象
            #endregion

            ViewData["queryJsonObj"] = queryJsonObj;  //输出JSON
            return Content(JsonConvert.SerializeObject(queryJsonObj));
        }


        //向右移动加载数据
        [HttpPost]
        public ActionResult KY_COMP_ChartsRight(string point, DateTime right)
        {
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<aircomp1> queryRecords1 = null;
            IQueryable<aircomp2> queryRecords2 = null;
            IQueryable<aircomp3> queryRecords3 = null;
            #region   ---------------选择器------------------
            switch (point)
            {
                case "空压机1":
                    queryRecords1 = (from m in db.aircomp1
                                     where (m.recordingTime > right && m.recordingTime.Second == 0)
                                     orderby m.id
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords1.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "空压机1";
                    queryRecords1.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    break;
                case "空压机2":
                    queryRecords2 = (from m in db.aircomp2
                                     where (m.recordingTime > right && m.recordingTime.Second == 0)
                                     orderby m.id
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords2.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "空压机2";
                    queryRecords2.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    break;
                case "空压机3":
                    queryRecords3 = (from m in db.aircomp3
                                     where (m.recordingTime > right && m.recordingTime.Second == 0)
                                     orderby m.id
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords3.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "空压机3";
                    queryRecords3.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    break;
            }
            #endregion

            #region ---------------将对象转为列矩阵JSON
            List<Double> PreList = new List<double>();
            List<Double> TemList = new List<double>();
            List<Double> EleList = new List<double>();
            List<DateTime> RecordTimeList = new List<DateTime>();
            if (queryRecords1 != null)
            {
                foreach (var firstRecord in queryRecords1)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    EleList.Add(Convert.ToDouble(firstRecord.current_u));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords2 != null)
            {
                foreach (var firstRecord in queryRecords2)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    EleList.Add(Convert.ToDouble(firstRecord.current_u));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords3 != null)
            {
                foreach (var firstRecord in queryRecords3)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    EleList.Add(Convert.ToDouble(firstRecord.current_u));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            JObject queryJsonObj = new JObject
            {
                { "Pre",JsonConvert.SerializeObject(PreList)},
                { "Tem",JsonConvert.SerializeObject(TemList)},
                { "Ele",  JsonConvert.SerializeObject(EleList) },
                { "RecordTime", JsonConvert.SerializeObject(RecordTimeList,iso).Replace("\"","")},
            };   //创建JSON对象
            #endregion

            ViewData["queryJsonObj"] = queryJsonObj;  //输出JSON
            return Content(JsonConvert.SerializeObject(queryJsonObj));
        }


        //按照给写时间段查询数据加载页面
        [HttpPost]
        public ActionResult KY_COMP_ChartsDuring(string point, DateTime begin, DateTime end)
        {
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<aircomp1> queryRecords1 = null;
            IQueryable<aircomp2> queryRecords2 = null;
            IQueryable<aircomp3> queryRecords3 = null;
            #region   ---------------选择器------------------
            switch (point)
            {
                case "空压机1":
                    queryRecords1 = (from m in db.aircomp1
                                     where (m.recordingTime > begin && m.recordingTime < end  &&m.recordingTime.Second ==0)
                                     orderby m.id descending
                                     select m).OrderBy(m => m.recordingTime);
                    if (queryRecords1.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "空压机1";
                    queryRecords1.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    break;
                case "空压机2":
                    queryRecords2 = (from m in db.aircomp2
                                     where (m.recordingTime > begin && m.recordingTime < end && m.recordingTime.Second == 0)
                                     orderby m.id descending
                                     select m).OrderBy(m => m.recordingTime);
                    if (queryRecords2.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "空压机2";
                    queryRecords2.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    break;
                case "空压机3":
                    queryRecords3 = (from m in db.aircomp3
                                     where (m.recordingTime > begin && m.recordingTime < end && m.recordingTime.Second == 0)
                                     orderby m.id descending
                                     select m).OrderBy(m => m.recordingTime);
                    if (queryRecords3.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "空压机3";
                    queryRecords3.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    break;
            }
            #endregion

            #region ---------------将对象转为列矩阵JSON
            List<Double> PreList = new List<double>();
            List<Double> TemList = new List<double>();
            List<Double> EleList = new List<double>();
            List<DateTime> RecordTimeList = new List<DateTime>();
            if (queryRecords1 != null)
            {
                foreach (var firstRecord in queryRecords1)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    EleList.Add(Convert.ToDouble(firstRecord.current_u));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords2 != null)
            {
                foreach (var firstRecord in queryRecords2)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    EleList.Add(Convert.ToDouble(firstRecord.current_u));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords3 != null)
            {
                foreach (var firstRecord in queryRecords3)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    EleList.Add(Convert.ToDouble(firstRecord.current_u));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            JObject queryJsonObj = new JObject
            {
                { "Pre",JsonConvert.SerializeObject(PreList)},
                { "Tem",JsonConvert.SerializeObject(TemList)},
                { "Ele",  JsonConvert.SerializeObject(EleList) },
                { "RecordTime", JsonConvert.SerializeObject(RecordTimeList,iso).Replace("\"","")},
            };   //创建JSON对象
            #endregion
            ViewData["queryJsonObj"] = queryJsonObj;  //输出JSON
            return Content(JsonConvert.SerializeObject(queryJsonObj));
        }

        #endregion


        #region    ---------------------空压房气体状态Charts图表---------------------------------------------

        //首次打开空压机页面
        [HttpGet]
        public ActionResult KY_Charts(string point = "气罐1")
        {
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<airbottle1> firstRecords1 = null;
            IQueryable<airbottle2> firstRecords2 = null;
            IQueryable<airbottle3> firstRecords3 = null;
            IQueryable<dryer1> firstRecords4 = null;
            IQueryable<dryer2> firstRecords5 = null;
            IQueryable<headerpipe3inch> firstRecords6 = null;
            IQueryable<headerpipe4inch> firstRecords7 = null;
            IQueryable<room> firstRecords8 = null;

            #region   ---------------选择器------------------
            switch (point)
            {
                case "气罐1":
                    firstRecords1 = (from m in db.airbottle1
                                    where (m.recordingTime.Second == 0 )
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "气罐1";
                    firstRecords1.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "气罐2":
                    firstRecords2 = (from m in db.airbottle2
                                    where (m.recordingTime.Second == 0)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "气罐2";
                    firstRecords2.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "气罐3":
                    firstRecords3 = (from m in db.airbottle3
                                    where (m.recordingTime.Second == 0)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "气罐3";
                    firstRecords3.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "冷干机1":
                    firstRecords4 = (from m in db.dryer1
                                    where (m.recordingTime.Second == 0)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "冷干机1";
                    firstRecords4.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "冷干机2":
                    firstRecords5 = (from m in db.dryer2
                                    where (m.recordingTime.Second == 0)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "冷干机2";
                    firstRecords5.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "3寸气管出口":
                    firstRecords6 = (from m in db.headerpipe3inch
                                    where (m.recordingTime.Second == 0)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "3寸气管出口";
                    firstRecords6.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "4寸气管出口":
                    firstRecords7 = (from m in db.headerpipe4inch
                                    where (m.recordingTime.Second == 0)
                                    orderby m.id descending
                                    select m).Take(1500).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "4寸气管出口";
                    firstRecords7.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "空压房内温湿度":
                    firstRecords8 = (from m in db.room
                                     where (m.recordingTime.Second == 0)
                                     orderby m.id descending
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "空压房内温湿度";
                    firstRecords8.Select(m => new { m.temperature, m.humidity, m.recordingTime });
                    break;

            }
            #endregion

            #region 将对象转为列矩阵JSON
            List<Double> PreList = new List<double>();
            List<Double> TemList = new List<double>();
            List<Double> HumList = new List<double>();
            List<DateTime> RecordTimeList = new List<DateTime>();
            if (firstRecords1 != null)
            {
                foreach (var firstRecord in firstRecords1)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (firstRecords2 != null)
            {
                foreach (var firstRecord in firstRecords2)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (firstRecords3 != null)
            {
                foreach (var firstRecord in firstRecords3)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (firstRecords4 != null)
            {
                foreach (var firstRecord in firstRecords4)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (firstRecords5 != null)
            {
                foreach (var firstRecord in firstRecords5)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (firstRecords6 != null)
            {
                foreach (var firstRecord in firstRecords6)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (firstRecords7 != null)
            {
                foreach (var firstRecord in firstRecords7)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (firstRecords8 != null)
            {
                foreach (var firstRecord in firstRecords8)
                {
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }


            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            JObject ResultJsonObj = new JObject //创建JSON对象
            {
                { "Pre",JsonConvert.SerializeObject(PreList)},
                { "Tem",JsonConvert.SerializeObject(TemList)},
                { "Hum",  JsonConvert.SerializeObject(HumList) },
                { "RecordTime", JsonConvert.SerializeObject(RecordTimeList,iso).Replace("\"","")},
            };
            #endregion

            ViewData["ResultJsonObj"] = ResultJsonObj;  //输出JSON
            return View(ResultJsonObj);
        }


        //向左移动加载数据
        [HttpPost]
        public ActionResult KY_ChartsLeft(string point, DateTime left)
        {
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<airbottle1> queryRecords1 = null;
            IQueryable<airbottle2> queryRecords2 = null;
            IQueryable<airbottle3> queryRecords3 = null;
            IQueryable<dryer1> queryRecords4 = null;
            IQueryable<dryer2> queryRecords5 = null;
            IQueryable<headerpipe3inch> queryRecords6 = null;
            IQueryable<headerpipe4inch> queryRecords7 = null;
            IQueryable<room> queryRecords8 = null;

            #region   ---------------选择器------------------
            switch (point)
            {
                case "气罐1":
                    queryRecords1 = (from m in db.airbottle1
                                     where (m.recordingTime.Second == 0 && m.recordingTime <left)
                                     orderby m.id descending
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords1.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "气罐1";
                    queryRecords1.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "气罐2":
                    queryRecords2 = (from m in db.airbottle2
                                     where (m.recordingTime.Second == 0 && m.recordingTime < left)
                                     orderby m.id descending
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords2.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "气罐2";
                    queryRecords2.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "气罐3":
                    queryRecords3 = (from m in db.airbottle3
                                     where (m.recordingTime.Second == 0 && m.recordingTime < left)
                                     orderby m.id descending
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords3.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "气罐3";
                    queryRecords3.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "冷干机1":
                    queryRecords4 = (from m in db.dryer1
                                     where (m.recordingTime.Second == 0 && m.recordingTime < left)
                                     orderby m.id descending
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords4.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "冷干机1";
                    queryRecords4.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "冷干机2":
                    queryRecords5 = (from m in db.dryer2
                                     where (m.recordingTime.Second == 0 && m.recordingTime < left)
                                     orderby m.id descending
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords5.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "冷干机2";
                    queryRecords5.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "3寸气管出口":
                    queryRecords6 = (from m in db.headerpipe3inch
                                     where (m.recordingTime.Second == 0 && m.recordingTime < left)
                                     orderby m.id descending
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords6.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "3寸气管出口";
                    queryRecords6.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "4寸气管出口":
                    queryRecords7 = (from m in db.headerpipe4inch
                                     where (m.recordingTime.Second == 0 && m.recordingTime < left)
                                     orderby m.id descending
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords7.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "4寸气管出口";
                    queryRecords7.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "空压房内温湿度":
                    queryRecords8 = (from m in db.room
                                     where (m.recordingTime.Second == 0 && m.recordingTime < left)
                                     orderby m.id descending
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords8.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "空压房内温湿度";
                    queryRecords8.Select(m => new { m.temperature, m.humidity, m.recordingTime });
                    break;

            }
            #endregion

            #region 将对象转为列矩阵JSON
            List<Double> PreList = new List<double>();
            List<Double> TemList = new List<double>();
            List<Double> HumList = new List<double>();
            List<DateTime> RecordTimeList = new List<DateTime>();
            if (queryRecords1 != null)
            {
                foreach (var firstRecord in queryRecords1)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords2 != null)
            {
                foreach (var firstRecord in queryRecords2)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords3 != null)
            {
                foreach (var firstRecord in queryRecords3)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords4 != null)
            {
                foreach (var firstRecord in queryRecords4)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords5 != null)
            {
                foreach (var firstRecord in queryRecords5)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords6 != null)
            {
                foreach (var firstRecord in queryRecords6)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords7 != null)
            {
                foreach (var firstRecord in queryRecords7)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords8 != null)
            {
                foreach (var firstRecord in queryRecords8)
                {
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }


            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            JObject ResultJsonObj = new JObject //创建JSON对象
            {
                { "Pre",JsonConvert.SerializeObject(PreList)},
                { "Tem",JsonConvert.SerializeObject(TemList)},
                { "Hum",  JsonConvert.SerializeObject(HumList) },
                { "RecordTime", JsonConvert.SerializeObject(RecordTimeList,iso).Replace("\"","")},
            };
            #endregion

            ViewData["ResultJsonObj"] = ResultJsonObj;  //输出JSON
            return Content(JsonConvert.SerializeObject(ResultJsonObj));
        }


        //向右移动加载数据
        [HttpPost]
        public ActionResult KY_ChartsRight(string point, DateTime right)
        {
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<airbottle1> queryRecords1 = null;
            IQueryable<airbottle2> queryRecords2 = null;
            IQueryable<airbottle3> queryRecords3 = null;
            IQueryable<dryer1> queryRecords4 = null;
            IQueryable<dryer2> queryRecords5 = null;
            IQueryable<headerpipe3inch> queryRecords6 = null;
            IQueryable<headerpipe4inch> queryRecords7 = null;
            IQueryable<room> queryRecords8 = null;

            #region   ---------------选择器------------------
            switch (point)
            {
                case "气罐1":
                    queryRecords1 = (from m in db.airbottle1
                                     where (m.recordingTime.Second == 0 && m.recordingTime > right)
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords1.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "气罐1";
                    queryRecords1.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "气罐2":
                    queryRecords2 = (from m in db.airbottle2
                                     where (m.recordingTime.Second == 0 && m.recordingTime > right)
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords2.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "气罐2";
                    queryRecords2.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "气罐3":
                    queryRecords3 = (from m in db.airbottle3
                                     where (m.recordingTime.Second == 0 && m.recordingTime > right)
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords3.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "气罐3";
                    queryRecords3.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "冷干机1":
                    queryRecords4 = (from m in db.dryer1
                                     where (m.recordingTime.Second == 0 && m.recordingTime > right)
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords4.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "冷干机1";
                    queryRecords4.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "冷干机2":
                    queryRecords5 = (from m in db.dryer2
                                     where (m.recordingTime.Second == 0 && m.recordingTime > right)
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords5.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "冷干机2";
                    queryRecords5.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "3寸气管出口":
                    queryRecords6 = (from m in db.headerpipe3inch
                                     where (m.recordingTime.Second == 0 && m.recordingTime > right)
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords6.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "3寸气管出口";
                    queryRecords6.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "4寸气管出口":
                    queryRecords7 = (from m in db.headerpipe4inch
                                     where (m.recordingTime.Second == 0 && m.recordingTime > right)
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords7.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "4寸气管出口";
                    queryRecords7.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "空压房内温湿度":
                    queryRecords8 = (from m in db.room
                                     where (m.recordingTime.Second == 0 && m.recordingTime > right)
                                     select m).Take(1500).OrderBy(m => m.recordingTime);
                    if (queryRecords8.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "空压房内温湿度";
                    queryRecords8.Select(m => new { m.temperature, m.humidity, m.recordingTime });
                    break;

            }
            #endregion

            #region 将对象转为列矩阵JSON
            List<Double> PreList = new List<double>();
            List<Double> TemList = new List<double>();
            List<Double> HumList = new List<double>();
            List<DateTime> RecordTimeList = new List<DateTime>();
            if (queryRecords1 != null)
            {
                foreach (var firstRecord in queryRecords1)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords2 != null)
            {
                foreach (var firstRecord in queryRecords2)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords3 != null)
            {
                foreach (var firstRecord in queryRecords3)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords4 != null)
            {
                foreach (var firstRecord in queryRecords4)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords5 != null)
            {
                foreach (var firstRecord in queryRecords5)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords6 != null)
            {
                foreach (var firstRecord in queryRecords6)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords7 != null)
            {
                foreach (var firstRecord in queryRecords7)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords8 != null)
            {
                foreach (var firstRecord in queryRecords8)
                {
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }


            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            JObject ResultJsonObj = new JObject //创建JSON对象
            {
                { "Pre",JsonConvert.SerializeObject(PreList)},
                { "Tem",JsonConvert.SerializeObject(TemList)},
                { "Hum",  JsonConvert.SerializeObject(HumList) },
                { "RecordTime", JsonConvert.SerializeObject(RecordTimeList,iso).Replace("\"","")},
            };
            #endregion

            ViewData["ResultJsonObj"] = ResultJsonObj;  //输出JSON
            return Content(JsonConvert.SerializeObject(ResultJsonObj));
        }


        //按照给写时间段查询数据加载页面
        [HttpPost]
        public ActionResult KY_ChartsDuring(string point, DateTime begin, DateTime end)
        {
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<airbottle1> queryRecords1 = null;
            IQueryable<airbottle2> queryRecords2 = null;
            IQueryable<airbottle3> queryRecords3 = null;
            IQueryable<dryer1> queryRecords4 = null;
            IQueryable<dryer2> queryRecords5 = null;
            IQueryable<headerpipe3inch> queryRecords6 = null;
            IQueryable<headerpipe4inch> queryRecords7 = null;
            IQueryable<room> queryRecords8 = null;

            #region   ---------------选择器------------------
            switch (point)
            {
                case "气罐1":
                    queryRecords1 = from m in db.airbottle1
                                     where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                     orderby m.id
                                     select m;
                    if (queryRecords1.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "气罐1";
                    queryRecords1.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "气罐2":
                    queryRecords2 = (from m in db.airbottle2
                                     where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                     orderby m.id
                                     select m).OrderBy(m => m.recordingTime);
                    if (queryRecords2.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "气罐2";
                    queryRecords2.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "气罐3":
                    queryRecords3 = (from m in db.airbottle3
                                     where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                     orderby m.id
                                     select m).OrderBy(m => m.recordingTime);
                    if (queryRecords3.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "气罐3";
                    queryRecords3.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "冷干机1":
                    queryRecords4 = (from m in db.dryer1
                                     where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                     orderby m.id
                                     select m).OrderBy(m => m.recordingTime);
                    if (queryRecords4.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "冷干机1";
                    queryRecords4.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "冷干机2":
                    queryRecords5 = (from m in db.dryer2
                                     where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                     orderby m.id
                                     select m).OrderBy(m => m.recordingTime);
                    if (queryRecords5.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "冷干机2";
                    queryRecords5.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "3寸气管出口":
                    queryRecords6 = (from m in db.headerpipe3inch
                                     where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                     orderby m.id
                                     select m).OrderBy(m => m.recordingTime);
                    if (queryRecords6.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "3寸气管出口";
                    queryRecords6.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "4寸气管出口":
                    queryRecords7 = (from m in db.headerpipe4inch
                                     where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                     orderby m.id
                                     select m).OrderBy(m => m.recordingTime);
                    if (queryRecords7.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "4寸气管出口";
                    queryRecords7.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    break;
                case "空压房内温湿度":
                    queryRecords8 = (from m in db.room
                                     where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                     orderby m.id
                                     select m).OrderBy(m => m.recordingTime);
                    if (queryRecords8.Count() == 0)
                    {
                        return Content("无数据");
                    }
                    ViewBag.Station = "空压房内温湿度";
                    queryRecords8.Select(m => new { m.temperature, m.humidity, m.recordingTime });
                    break;
            }
            #endregion

            #region 将对象转为列矩阵JSON
            List<Double> PreList = new List<double>();
            List<Double> TemList = new List<double>();
            List<Double> HumList = new List<double>();
            List<DateTime> RecordTimeList = new List<DateTime>();
            if (queryRecords1 != null)
            {
                foreach (var firstRecord in queryRecords1)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords2 != null)
            {
                foreach (var firstRecord in queryRecords2)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords3 != null)
            {
                foreach (var firstRecord in queryRecords3)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords4 != null)
            {
                foreach (var firstRecord in queryRecords4)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords5 != null)
            {
                foreach (var firstRecord in queryRecords5)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords6 != null)
            {
                foreach (var firstRecord in queryRecords6)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords7 != null)
            {
                foreach (var firstRecord in queryRecords7)
                {
                    PreList.Add(Convert.ToDouble(firstRecord.pressure));
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }
            if (queryRecords8 != null)
            {
                foreach (var firstRecord in queryRecords8)
                {
                    TemList.Add(Convert.ToDouble(firstRecord.temperature));
                    HumList.Add(Convert.ToDouble(firstRecord.humidity));
                    RecordTimeList.Add(Convert.ToDateTime(firstRecord.recordingTime));
                }
            }


            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            JObject ResultJsonObj = new JObject //创建JSON对象
            {
                { "Pre",JsonConvert.SerializeObject(PreList)},
                { "Tem",JsonConvert.SerializeObject(TemList)},
                { "Hum",  JsonConvert.SerializeObject(HumList) },
                { "RecordTime", JsonConvert.SerializeObject(RecordTimeList,iso).Replace("\"","")},
            };
            #endregion

            ViewData["ResultJsonObj"] = ResultJsonObj;  //输出JSON
            return Content(JsonConvert.SerializeObject(ResultJsonObj));
        }

        #endregion



        #region  -------------ExportToExcel将记录输出Excel表格方法------------------
        public class ResultTH
        {
            public double Tem { get; set; }
            public double Hum { get; set; }
            public DateTime RecordTime { get; set; }
        }

        public class ResultTHToExcel
        {
            public string point { get; set; }
            public double Tem { get; set; }
            public double Hum { get; set; }
            public DateTime RecordTime { get; set; }
        }

        public class ResultKY
        {
            public double Pre { get; set; }
            public double Tem { get; set; }
            public double Hum { get; set; }
            public DateTime RecordTime { get; set; }
        }

        public class ResultKY_COM
        {
            public double Pre { get; set; }
            public double Tem { get; set; }
            public double Current_u { get; set; }
            public DateTime RecordTime { get; set; }
        }


        [HttpPost]
        public FileContentResult THExportToExcel(string point, DateTime begin, DateTime end)
        {
            #region 监测点信息
            //Floor  point              DeviceID  NodeID
            //一楼   一楼篮球场         40004493   1        2018-8-1新增
            //一楼   一楼配套加工(1)    40004518   1        2018-8-9新增
            //一楼   一楼配套加工(2)    40004518   2        2018-8-9新增
            //一楼   一楼配套加工(3)    40004518   3        2018-8-9新增
            //一楼   一楼配套加工(4)    40004518   4        2018-8-9新增
            //一楼   一楼配套加工(5)    40004518   5        2018-8-9新增
            //一楼   一楼配套加工(6)    40004518   6        2018-8-9新增
            //三楼   烤箱房             40001676   6
            //三楼   插件               40001676   9
            //三楼   面罩               40021209   1
            //三楼   三楼仓库           40004493   2        2018-8-1新增
            //四楼   四楼小间距装配     40004518   7        2018-8-28从40001676转到40004518
            //四楼   四楼组装线1号　    40021216   1        2019-3-13新增
            //四楼   四楼组装线2号      40021216   2　　　　2019-3-13新增
            //四楼   四楼喷墨房1号　    40021216   3        2018-12-3从四楼小间距装配迁移到四楼喷墨房1号
            //四楼   四楼喷墨房2号      40021216   4　　　　2018-12-3从四楼组装迁移到四楼喷墨房2号
            //四楼   烤箱房             40001676   1
            //四楼   老化               40001676   2
            //四楼   组装               40001676   3
            //四楼   线材               40001676   4
            //四楼   四楼老化调试       40001676   5
            //四楼   四楼老化调试2号    40004493   4        2018-8-1新增
            //四楼   四楼老化调试3号    40004493   5        2018-8-1新增
            //四楼   四楼仓库           40004493   3        2018-8-1新增
            //四楼   四楼小样房         40004493   7        2019-3-20新增
            //四楼   四楼首样房         40004493   8        2019-3-20新增
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT               
            //五楼   SMT物料暂存        40000938   6
            //五楼   电子仓7            40000938   7
            //五楼   电子仓8            40000938   8
            //五楼   电子仓9            40000938   9
            //五楼   电子仓10           40000938   10       
            //五楼   电子仓11           40000938   11
            //五楼   电子仓12           40000938   12
            //六楼  空压房             
            #endregion

            kongyadbEntities db = new kongyadbEntities();
            IQueryable<THhistory> queryRecords = null;
            List<ResultTH> Resultlist = new List<ResultTH>();
            #region   ---------------选择器------------------
            switch (point)
            {
                case "一楼配套加工(5)40004518#5":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004518" && m.NodeID == "5" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(6)40004518#6":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004518" && m.NodeID == "6" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(1)40004518#1":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004518" && m.NodeID == "1" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(4)40004518#4":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004518" && m.NodeID == "4" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼配套加工(2)40004518#2":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004518" && m.NodeID == "2" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "一楼篮球场40004493#1":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "1" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                //case "一楼配套加工(3)40004518#3":
                //    queryRecords = from m in db.THhistory
                //                   where (m.DeviceID == "40004518" && m.NodeID == "3" && m.RecordTime > begin && m.RecordTime < end)
                //                   orderby m.id
                //                   select m;
                //    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                //    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                //    break;

                //case "三楼烤箱房40001676#6":
                //    queryRecords = from m in db.THhistory
                //                   where (m.DeviceID == "40001676" && m.NodeID == "6" && m.RecordTime > begin && m.RecordTime < end)
                //                   orderby m.id
                //                   select m;
                //    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                //    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                //    break;
                case "三楼插件40001676#9":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "9" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "三楼面罩40021209#1":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021209" && m.NodeID == "1" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "3#三楼仓库40004493#2":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "2" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                //case "四楼烤箱房40001676#1":
                //    queryRecords = from m in db.THhistory
                //                   where (m.DeviceID == "40001676" && m.NodeID == "1" && m.RecordTime > begin && m.RecordTime < end)
                //                   orderby m.id
                //                   select m;
                //    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                //    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                //    break;
                //四楼   四楼喷墨房1号　    40004518   7        2018-12-3从四楼小间距装配迁移到四楼喷墨房1号
                case "四楼喷墨房1号40021216#3":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021216" && m.NodeID == "3" && m.DeviceName== "四楼喷墨房1号40021216#3" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                //四楼   四楼喷墨房2号      40001676   3　　　　2018-12-3从四楼组装迁移到四楼喷墨房2号
                case "四楼喷墨房2号40021216#4":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021216" && m.NodeID == "4" && m.DeviceName == "四楼喷墨房2号40021216#4" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼组装线1号40021216#1":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021216" && m.NodeID == "1" && m.DeviceName == "四楼组装线1号40021216#1" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;

                case "四楼组装线2号40021216#2":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021216" && m.NodeID == "2" && m.DeviceName == "四楼组装线2号40021216#2" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼线材40001676#4":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40001676" && m.NodeID == "4" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试3号40004493#5":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "5" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                //case "四楼老化40001676#2":
                //    queryRecords = from m in db.THhistory
                //                   where (m.DeviceID == "40001676" && m.NodeID == "2" && m.RecordTime > begin && m.RecordTime < end)
                //                   orderby m.id
                //                   select m;
                //    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                //    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                //    break;
                case "四楼小样房40004493#7":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "7" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼首样房40004493#8":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "8" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试40004493#6":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "6" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "四楼老化调试2号40004493#4":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "4" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;

                case "4#四楼仓库40004493#3":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40004493" && m.NodeID == "3" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT产线40021210#2":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021210" && m.NodeID == "2" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                        ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT产线40021210#1":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40021210" && m.NodeID == "1" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                        ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼SMT物料暂存40000938#6":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "6" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#7":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "7" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#8":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "8" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#9":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "9" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    if (queryRecords.Count() == 0)
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#10":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "10" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#11":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "11" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
                case "五楼电子仓40000938#12":
                    queryRecords = from m in db.THhistory
                                   where (m.DeviceID == "40000938" && m.NodeID == "12" && m.RecordTime > begin && m.RecordTime < end)
                                   orderby m.id
                                   select m;
                    ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
                    queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });
                    break;
            }

            foreach (var item in queryRecords)
            {
                ResultTH at = new ResultTH();
                at.Tem = Convert.ToDouble(item.Tem);
                at.Hum = Convert.ToDouble(item.Hum);
                at.RecordTime = Convert.ToDateTime(item.RecordTime);
                Resultlist.Add(at);
            }

            #endregion

            string[] columns = { "温度(℃)", "湿度(RH%)", "记录时间" };
            //byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, "", false, columns);
            byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, point, false, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, point + ".xlsx");
        }

        [HttpPost]
        public FileContentResult KYExportToExcel(string point, DateTime begin, DateTime end)
        {

            kongyadbEntities db = new kongyadbEntities();
            IQueryable<airbottle1> queryRecords1 = null;
            IQueryable<airbottle2> queryRecords2 = null;
            IQueryable<airbottle3> queryRecords3 = null;
            IQueryable<dryer1> queryRecords4 = null;
            IQueryable<dryer2> queryRecords5 = null;
            IQueryable<headerpipe3inch> queryRecords6 = null;
            IQueryable<headerpipe4inch> queryRecords7 = null;
            IQueryable<room> queryRecords8 = null;
            List<ResultKY> Resultlist = new List<ResultKY>();
            #region   ---------------选择器------------------
            switch (point)
            {
                case "气罐1":
                    queryRecords1 = from m in db.airbottle1
                                    where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                    orderby m.id
                                    select m;
                    ViewBag.Station = "气罐1";
                    queryRecords1.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    foreach (var item in queryRecords1)
                    {
                        ResultKY pat = new ResultKY();
                        pat.Pre = Convert.ToDouble(item.pressure);
                        pat.Tem = Convert.ToDouble(item.temperature);
                        pat.Hum = Convert.ToDouble(item.humidity);
                        pat.RecordTime = Convert.ToDateTime(item.recordingTime);
                        Resultlist.Add(pat);
                    }
                    break;
                case "气罐2":
                    queryRecords2 = from m in db.airbottle2
                                    where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                    orderby m.id
                                    select m;
                    ViewBag.Station = "气罐2";
                    queryRecords2.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    foreach (var item in queryRecords2)
                    {
                        ResultKY pat = new ResultKY();
                        pat.Pre = Convert.ToDouble(item.pressure);
                        pat.Tem = Convert.ToDouble(item.temperature);
                        pat.Hum = Convert.ToDouble(item.humidity);
                        pat.RecordTime = Convert.ToDateTime(item.recordingTime);
                        Resultlist.Add(pat);
                    }
                    break;
                case "气罐3":
                    queryRecords3 = from m in db.airbottle3
                                    where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                    orderby m.id
                                    select m;
                    ViewBag.Station = "气罐3";
                    queryRecords3.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    foreach (var item in queryRecords3)
                    {
                        ResultKY pat = new ResultKY();
                        pat.Pre = Convert.ToDouble(item.pressure);
                        pat.Tem = Convert.ToDouble(item.temperature);
                        pat.Hum = Convert.ToDouble(item.humidity);
                        pat.RecordTime = Convert.ToDateTime(item.recordingTime);
                        Resultlist.Add(pat);
                    }
                    break;
                case "冷干机1":
                    queryRecords4 = from m in db.dryer1
                                    where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                    orderby m.id
                                    select m;
                    ViewBag.Station = "气罐3";
                    queryRecords4.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    foreach (var item in queryRecords4)
                    {
                        ResultKY pat = new ResultKY();
                        pat.Pre = Convert.ToDouble(item.pressure);
                        pat.Tem = Convert.ToDouble(item.temperature);
                        pat.Hum = Convert.ToDouble(item.humidity);
                        pat.RecordTime = Convert.ToDateTime(item.recordingTime);
                        Resultlist.Add(pat);
                    }
                    break;
                case "冷干机2":
                    queryRecords5 = from m in db.dryer2
                                    where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                    orderby m.id
                                    select m;
                    ViewBag.Station = "气罐3";
                    queryRecords5.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    foreach (var item in queryRecords5)
                    {
                        ResultKY pat = new ResultKY();
                        pat.Pre = Convert.ToDouble(item.pressure);
                        pat.Tem = Convert.ToDouble(item.temperature);
                        pat.Hum = Convert.ToDouble(item.humidity);
                        pat.RecordTime = Convert.ToDateTime(item.recordingTime);
                        Resultlist.Add(pat);
                    }
                    break;
                case "3寸气管出口":
                    queryRecords6 = from m in db.headerpipe3inch
                                    where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                    orderby m.id
                                    select m;
                    ViewBag.Station = "气罐3";
                    queryRecords6.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    foreach (var item in queryRecords6)
                    {
                        ResultKY pat = new ResultKY();
                        pat.Pre = Convert.ToDouble(item.pressure);
                        pat.Tem = Convert.ToDouble(item.temperature);
                        pat.Hum = Convert.ToDouble(item.humidity);
                        pat.RecordTime = Convert.ToDateTime(item.recordingTime);
                        Resultlist.Add(pat);
                    }
                    break;
                case "4寸气管出口":
                    queryRecords7 = from m in db.headerpipe4inch
                                    where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                    orderby m.id
                                    select m;
                    ViewBag.Station = "气罐3";
                    queryRecords7.Select(m => new { m.temperature, m.humidity, m.pressure, m.recordingTime });
                    foreach (var item in queryRecords7)
                    {
                        ResultKY pat = new ResultKY();
                        pat.Pre = Convert.ToDouble(item.pressure);
                        pat.Tem = Convert.ToDouble(item.temperature);
                        pat.Hum = Convert.ToDouble(item.humidity);
                        pat.RecordTime = Convert.ToDateTime(item.recordingTime);
                        Resultlist.Add(pat);
                    }
                    break;
                case "空压房内温湿度":
                    queryRecords8 = from m in db.room
                                    where (m.recordingTime.Second == 0 && m.recordingTime > begin && m.recordingTime < end)
                                    orderby m.id
                                    select m;
                    ViewBag.Station = "气罐3";
                    queryRecords8.Select(m => new { m.temperature, m.humidity, m.recordingTime });
                    foreach (var item in queryRecords8)
                    {
                        ResultKY pat = new ResultKY();
                        pat.Tem = Convert.ToDouble(item.temperature);
                        pat.Hum = Convert.ToDouble(item.humidity);
                        pat.RecordTime = Convert.ToDateTime(item.recordingTime);
                        Resultlist.Add(pat);
                    }
                    break;
            }
            #endregion

            string[] columns = { "压力(Mpa)", "温度(℃)", "湿度(RH%)", "记录时间" };
            //byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, "", false, columns);
            byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, point, false, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, point + ".xlsx");
        }

        [HttpPost]
        public FileContentResult KY_COM_ExportToExcel(string point, DateTime begin, DateTime end)
        {
            kongyadbEntities db = new kongyadbEntities();
            IQueryable<aircomp1> queryRecords1 = null;
            IQueryable<aircomp2> queryRecords2 = null;
            IQueryable<aircomp3> queryRecords3 = null;
            List<ResultKY_COM> Resultlist = new List<ResultKY_COM>();

            #region  ----------选择器-------------
            switch (point)
            {
                case "空压机1":
                    queryRecords1 = (from m in db.aircomp1
                                     where (m.recordingTime > begin && m.recordingTime < end /*&& m.recordingTime.Second == 0*/)    //注释部分是输出分钟为单位的筛选
                                     orderby m.id descending
                                     select m).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "空压机1";
                    queryRecords1.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    foreach (var item in queryRecords1)
                    {
                        ResultKY_COM pat = new ResultKY_COM();
                        pat.Pre = Convert.ToDouble(item.pressure);
                        pat.Tem = Convert.ToDouble(item.temperature);
                        pat.Current_u = Convert.ToDouble(item.current_u);
                        pat.RecordTime = Convert.ToDateTime(item.recordingTime);
                        Resultlist.Add(pat);
                    }
                    break;
                case "空压机2":
                    queryRecords2 = (from m in db.aircomp2
                                     where (m.recordingTime > begin && m.recordingTime < end /*&& m.recordingTime.Second == 0*/)    //注释部分是输出分钟为单位的筛选
                                     orderby m.id descending
                                     select m).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "空压机2";
                    queryRecords2.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    foreach (var item in queryRecords2)
                    {
                        ResultKY_COM pat = new ResultKY_COM();
                        pat.Pre = Convert.ToDouble(item.pressure);
                        pat.Tem = Convert.ToDouble(item.temperature);
                        pat.Current_u = Convert.ToDouble(item.current_u);
                        pat.RecordTime = Convert.ToDateTime(item.recordingTime);
                        Resultlist.Add(pat);
                    }
                    break;
                case "空压机3":
                    queryRecords3 = (from m in db.aircomp3
                                     where (m.recordingTime > begin && m.recordingTime < end /*&& m.recordingTime.Second == 0*/)  //注释部分是输出分钟为单位的筛选
                                     orderby m.id descending
                                     select m).OrderBy(m => m.recordingTime);
                    ViewBag.Station = "空压机3";
                    queryRecords3.Select(m => new { m.pressure, m.temperature, m.current_u, m.recordingTime });
                    foreach (var item in queryRecords3)
                    {
                        ResultKY_COM pat = new ResultKY_COM();
                        pat.Pre = Convert.ToDouble(item.pressure);
                        pat.Tem = Convert.ToDouble(item.temperature);
                        pat.Current_u = Convert.ToDouble(item.current_u);
                        pat.RecordTime = Convert.ToDateTime(item.recordingTime);
                        Resultlist.Add(pat);
                    }
                    break;
            }
            #endregion
            string[] columns = { "压力(Mpa)", "温度(℃)", "电流(A)", "记录时间" };
            //byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, "", false, columns);
            byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, point, false, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, point+".xlsx");
        }

        #endregion



        #region  -------------Excel导出帮助类---------------
        /// <summary>
        /// Excel导出帮助类
        /// </summary>
        public class ExcelExportHelper
        {
            public static string ExcelContentType
            {
                get
                {
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                }
            }
            /// <summary>
            /// List转DataTable
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="data"></param>
            /// <returns></returns>
            public static DataTable ListToDataTable<T>(List<T> data)
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
                DataTable dataTable = new DataTable();
                for (int i = 0; i < properties.Count; i++)
                {
                    PropertyDescriptor property = properties[i];
                    dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
                object[] values = new object[properties.Count];
                foreach (T item in data)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = properties[i].GetValue(item);
                    }
                    dataTable.Rows.Add(values);
                }
                return dataTable;
            }
            /// <summary>
            /// 导出Excel
            /// </summary>
            /// <param name="dataTable">数据源</param>
            /// <param name="heading">工作簿Worksheet</param>
            /// <param name="showSrNo">//是否显示行编号</param>
            /// <param name="columnsToTake">要导出的列</param>
            /// <returns></returns>
            public static byte[] ExportExcel(DataTable dataTable, string heading = "", bool showSrNo = false, params string[] columnsToTake)
            {
                byte[] result = null;
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(string.Format("{0}Data", heading));
                    int startRowFrom = string.IsNullOrEmpty(heading) ? 1 : 3; //开始的行
                                                                              //是否显示行编号
                    if (showSrNo)
                    {
                        DataColumn dataColumn = dataTable.Columns.Add("序号", typeof(int));
                        dataColumn.SetOrdinal(0);
                        int index = 1;
                        foreach (DataRow item in dataTable.Rows)
                        {
                            item[0] = index;
                            index++;
                        }
                    }
                    //Add Content Into the Excel File
                    workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);
                    // autofit width of cells with small content 
                    int columnIndex = 1;
                    foreach (DataColumn item in dataTable.Columns)
                    {
                        ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                        int maxLength = columnCells.Max(cell => cell.Value.ToString().Count());
                        if (maxLength < 150)
                        {
                            workSheet.Column(columnIndex).AutoFit();
                        }
                        columnIndex++;
                    }
                    // format header - bold, yellow on black 
                    using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                    {
                        r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        r.Style.Font.Bold = true;
                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
                    }
                    // format cells - add borders 
                    using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                    {
                        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    }
                    //// removed ignored columns 
                    //for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                    //{
                    //    if (i == 0 && showSrNo)
                    //    {
                    //        continue;
                    //    }
                    //    if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                    //    {
                    //        workSheet.DeleteColumn(i + 1);
                    //    }
                    //}
                    if (!String.IsNullOrEmpty(heading))
                    {
                        workSheet.Cells["A1"].Value = heading;

                        int c = 1;
                        foreach (var a in columnsToTake)
                        {
                            workSheet.Cells[3, c].Value = a;
                            workSheet.Cells[3, c].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                            workSheet.Cells[3, c].Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                            c++;
                        }

                        workSheet.Column(dataTable.Columns.Count).Style.Numberformat.Format = "yyyy-MM-dd hh:mm";
                        //workSheet.Column(dataTable.Columns.Count).Style.ShrinkToFit = true;  //字体自适应大小
                        workSheet.Column(dataTable.Columns.Count).Width = 20;//设置列宽
                        for (int i = dataTable.Columns.Count-1; i>0;i--)  //设置前几列的列宽
                        {
                        workSheet.Column(i).Width = 12;//设置列宽
                        }

                        workSheet.InsertColumn(1, 1);
                        workSheet.InsertRow(1, 1);
                        workSheet.Column(1).Width = 5;

                        workSheet.Cells["B2"].Style.Font.Size = 18;
                    }
                    result = package.GetAsByteArray();
                }
                return result;
            }
            /// <summary>
            /// 导出Excel
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="data"></param>
            /// <param name="heading"></param>
            /// <param name="isShowSlNo"></param>
            /// <param name="ColumnsToTake"></param>
            /// <returns></returns>
            public static byte[] ExportExcel<T>(List<T> data, string heading = "", bool isShowSlNo = false, params string[] ColumnsToTake)
            {
                return ExportExcel(ListToDataTable<T>(data), heading, isShowSlNo, ColumnsToTake);
            }
        }

        #endregion



        #region  -------------导出温湿度记录输出Excel表格方法------------------
        //..TODO..
        public ActionResult OutputTHDataToExcel()
        {
            ViewBag.PointList = db.THhistory.Select(m => m.DeviceName).Distinct();

            return View();
        }

        [HttpPost]
        public FileContentResult OutputTHDataToExcel(List<string> querylist, string Key, List<DateTime> Time, DateTime Begin, DateTime End)
        {
            kongyadbEntities db = new kongyadbEntities();
            List <THhistory> queryRecords = new List<THhistory>();
            List<ResultTHToExcel> Resultlist = new List<ResultTHToExcel>();

            #region-------测试数据------
            //list = db.THhistory.Select(m => m.DeviceName).ToList().Distinct().ToList();  //全部温湿度信息点清单
            //string Key1 = "Timeing", Key2 = "Periods";

            //Time.Add(Convert.ToDateTime("2018-10-23 12,00"));
            //Time.Add(Convert.ToDateTime("2018-10-23 13,00"));
            //Time.Add(Convert.ToDateTime("2018-10-23 14,00"));

            //Begin = Convert.ToDateTime("2018-10-20 00,00");
            //End = DateTime.Now;


            //if (querylist==null)  //全部点输出
            //{
            //    list = db.THhistory.Select(m => m.DeviceName).ToList().Distinct().ToList();  //全部温湿度信息点清单
            //    foreach (var item in list)
            //    {
            //        if (Key == "Timing")  //时间点输出
            //        {
            //            foreach (var it in Time)
            //            {

            //            }
            //        }
            //        else if (Key == "Periods") //时间段输出
            //        {

            //        }
            //    }
            //}
            //else //按照清单输出点输出
            //{
            //    foreach(var item in querylist)   //按温湿度信息点清单查询
            //    {

            //    }
            //}

            //string Point = "四楼组装40001676#3";
            //string Key = "Timeing";
            //DateTime dt = new DateTime(2018, 9, 1, 8, 0, 00, 0);
            //List<DateTime> Time = new List<DateTime>();
            //Time.Add(dt);
            //DateTime dt1 = new DateTime(2018, 9, 1, 15, 00, 00, 0);
            //Time.Add(dt1);
            //DateTime Begin = new DateTime(2018, 9, 1, 0, 0, 00, 0);
            //DateTime End = new DateTime(2018, 9, 10, 23, 59, 59, 0);
            #endregion

            if (querylist==null)  //全部监测点输出
            {
                querylist= db.THhistory.Select(m => m.DeviceName).Distinct().ToList();
                foreach (var Point in querylist)  //全部监测点输出
                {
                    if (Key == "Timeing")  //时间点输出
                    {
                        foreach (var t in Time)
                        {
                            queryRecords.AddRange(db.THhistory.OrderBy(c => c.DeviceName).ThenBy(c => c.RecordTime).Where(c => c.DeviceName == Point).Where(c => c.RecordTime > Begin && c.RecordTime < End).Where(c => c.RecordTime.Value.Hour == t.Hour && c.RecordTime.Value.Minute == t.Minute));
                        }
                    }
                    if (Key == "Periods")  //时间段输出
                    {
                            queryRecords.AddRange(db.THhistory.OrderBy(c => c.DeviceName).ThenBy(c => c.RecordTime).Where(c => c.DeviceName == Point).Where(c => c.RecordTime > Begin && c.RecordTime < End));
                    }
                }
            }
            else  //按选择的监测点输出
            {
                foreach (var Point in querylist)  
                {
                    if (Key == "Timeing")  //时间点输出
                    {
                        foreach (var t in Time)
                        {
                            queryRecords.AddRange(db.THhistory.OrderBy(c => c.DeviceName).ThenBy(c => c.RecordTime).Where(c => c.DeviceName == Point).Where(c => c.RecordTime > Begin && c.RecordTime < End).Where(c => c.RecordTime.Value.Hour == t.Hour && c.RecordTime.Value.Minute == t.Minute));
                        }
                    }
                    if (Key == "Periods")  //时间段输出
                    {
                            queryRecords.AddRange(db.THhistory.OrderBy(c => c.DeviceName).ThenBy(c => c.RecordTime).Where(c => c.DeviceName == Point).Where(c => c.RecordTime > Begin && c.RecordTime < End));
                    }
                }
            }

            //数据重组
            if (queryRecords != null)
            {
                foreach (var item in queryRecords)
                {
                    ResultTHToExcel at = new ResultTHToExcel();
                    at.point = item.DeviceName;
                    at.Tem = Convert.ToDouble(item.Tem);
                    at.Hum = Convert.ToDouble(item.Hum);
                    at.RecordTime = Convert.ToDateTime(item.RecordTime);
                    Resultlist.Add(at);
                }
            }

            if (Resultlist.Count()>0)
            {
                string[] columns = { "监测点", "温度(℃)", "湿度(RH%)", "记录时间" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, "温湿度记录", false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, "温湿度记录.xlsx");
            }
            else
            {
                ResultTHToExcel at = new ResultTHToExcel();
                at.point = "开始时间：";
                at.RecordTime = Begin;
                Resultlist.Add(at);
                ResultTHToExcel at1 = new ResultTHToExcel();
                at1.point = "结束时间：";
                at1.RecordTime = End;
                Resultlist.Add(at1);
                ResultTHToExcel at2 = new ResultTHToExcel();
                at2.point = "此时间段内没有找到相关记录！";
                at2.RecordTime = DateTime.Now;
                Resultlist.Add(at2);
                string[] columns = { "监测点", "温度(℃)", "湿度(RH%)", "记录时间" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, "温湿度记录", false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, "温湿度记录.xlsx");
            }
        }
            #endregion



        #region -----------系统通讯异常邮件通知

        /// <summary>
        /// 三台空压机通讯有问题（网络、弱电箱电源等问题）
        /// </summary>
        /// <param name="a"></param>
        [HttpPost]
        public void SendEmail(string a)
        {
            var errorMessage = "发送失败";
            #region MyRegion
            //try
            //{
            //    WebMail.SmtpServer = "smtp.qq.com";
            //    WebMail.SmtpPort = 25;//端口号，不同SMTP服务器可能不同，可以查一下
            //    WebMail.EnableSsl = false;//禁用SSL
            //    WebMail.UserName = "3490212659";
            //    WebMail.Password = "ABC@123.";
            //    WebMail.From = "3490212659@qq.com";
            //    WebMail.Send("250389538@qq.com,3490212659@qq.com", "空压机房异常", "空压机已经连续" + a + "没有新值，请检查采集程序及空压房弱电箱情况！");
            //}
            #endregion

            try
            {
                WebMail.SmtpServer = "smtp.163.com";
                WebMail.SmtpPort = 25;//SMTP服务器端口号
                WebMail.EnableSsl = true;//禁用SSL
                WebMail.UserName = "18665227107";
                WebMail.Password = "ABC@123.";
                WebMail.From = "18665227107@163.com";
                #region MyRegion
                //List<string> EmailList = new List<string>
                //{
                // "2906397229@qq.com",
                // "250389538@qq.com",
                // "3490212659@qq.com"
                //};
                //foreach (var i in EmailList)
                //{
                //    WebMail.Send(i, "MES系统信息", "三台空压机已经连续" + a + "没有新值，请检查采集程序及空压房弱电箱情况！");
                //}
                #endregion

                WebMail.Send("2906397229@qq.com,250389538@qq.com,3490212659@qq.com", "MES系统信息", "三台空压机已经连续" + a + "没有新值，请检查采集程序及空压房弱电箱情况！");
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }




        #endregion

    }
}