using JianHeMES.Areas.KongYaHT.Models;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace JianHeMES.Controllers
{
    public class TestApiController : ApiController
    {
        private kongyadbEntities db = new kongyadbEntities();

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }



 
        public bool Index(string jsondata)   //接收jsondata
        {
            CalibrationRecord data = JsonConvert.DeserializeObject<CalibrationRecord>(jsondata);   //使用JsonConvert.DeserializeObject<类名>（字符串）来把传过来的字符串解析为对应的类
            return true;
        }


        public string ElectVal()   //接收jsondata
        {
            var value = db.aircomp3.OrderByDescending(c => c.recordingTime).FirstOrDefault().current_u.ToString();
            return value;
        }



        public JObject HTChartsLeft(string point, DateTime left)
        {
            IQueryable<THhistory> queryRecords = null;
            JObject aa = new JObject();
            queryRecords = (from m in db.THhistory
                            where (m.DeviceID == "40004493" && m.NodeID == "1" && m.RecordTime < left)
                            orderby m.id descending
                            select m).Take(100).OrderBy(m => m.RecordTime);
            if (queryRecords.Count() == 0)
            {
                aa.Add("无数据", "无数据");
                return aa;
            }
            //ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
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

            //ViewData["queryJsonObj"] = queryJsonObj;  //输出JSON
            return queryJsonObj;
        }

    }
}