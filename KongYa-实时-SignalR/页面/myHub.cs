using JianHeMES.Models;
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
using JianHeMES.Areas.KongYaHT.Models;

namespace JianHeMES.Hubs
{


    [HubName("myHub")]
    public class MyHub : Hub
    {
        // Is set via the constructor on each creation
        private Broadcaster _broadcaster;
        public MyHub()
            : this(Broadcaster.Instance)
        {
        }
        public MyHub(Broadcaster broadcaster)
        {
            _broadcaster = broadcaster;
        }
    }

    /// <summary>
    /// 数据广播器
    /// </summary>
    public class Broadcaster
    {
        private readonly static Lazy<Broadcaster> _instance =
            new Lazy<Broadcaster>(() => new Broadcaster());
        private readonly IHubContext _hubContext;
        private Timer _broadcastLoop;

        public Broadcaster()
        {
            // 获取所有连接的句柄，方便后面进行消息广播
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            // Start the broadcast loop
            _broadcastLoop = new Timer(
                BroadcastShape,
                null,
                1000,
                1000);
        }

        private void BroadcastShape(object state)
        {   // 定期执行的方法
            //空压房数据
            JObject KY_json = new JObject();
            using (var KY_db = new kongyadbEntities())
            {
            var KY_aircomp1 = (from m in KY_db.aircomp1 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_aircomp2 = (from m in KY_db.aircomp2 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_aircomp3 = (from m in KY_db.aircomp3 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_airbottle1 = (from m in KY_db.airbottle1 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_airbottle2 = (from m in KY_db.airbottle2 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_airbottle3 = (from m in KY_db.airbottle3 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_dryer1 = (from m in KY_db.dryer1 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_dryer2 = (from m in KY_db.dryer2 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_headerpipe3inch = (from m in KY_db.headerpipe3inch select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_headerpipe4inch = (from m in KY_db.headerpipe4inch select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_room = (from m in KY_db.room select m).OrderByDescending(p => p.id).FirstOrDefault();
            KY_json.Add("aircomp1", JsonConvert.SerializeObject(KY_aircomp1));
            KY_json.Add("aircomp2", JsonConvert.SerializeObject(KY_aircomp2));
            KY_json.Add("aircomp3", JsonConvert.SerializeObject(KY_aircomp3));
            KY_json.Add("airbottle1", JsonConvert.SerializeObject(KY_airbottle1));
            KY_json.Add("airbottle2", JsonConvert.SerializeObject(KY_airbottle2));
            KY_json.Add("airbottle3", JsonConvert.SerializeObject(KY_airbottle3));
            KY_json.Add("dryer1", JsonConvert.SerializeObject(KY_dryer1));
            KY_json.Add("dryer2", JsonConvert.SerializeObject(KY_dryer2));
            KY_json.Add("headerpipe3inch", JsonConvert.SerializeObject(KY_headerpipe3inch));
            KY_json.Add("headerpipe4inch", JsonConvert.SerializeObject(KY_headerpipe4inch));
            KY_json.Add("room", JsonConvert.SerializeObject(KY_room));
            }
            _hubContext.Clients.All.sendKY(KY_json);


            #region ---------其他广播数据-----------
            ////三楼温湿度数据
            //JObject TH3_json = new JObject();   //创建JSON对象
            ////取出数据
            //using (var KY_db = new kongyadbEntities())
            //{
            //var TH_40001676_6 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "6") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40001676_9 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "9") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40001676_10 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "10") select m).OrderByDescending(p => p.id).FirstOrDefault();
            ////存入JSON对象
            //TH3_json.Add("TH_40001676_1", JsonConvert.SerializeObject(TH_40001676_6));
            //TH3_json.Add("TH_40001676_2", JsonConvert.SerializeObject(TH_40001676_9));
            //TH3_json.Add("TH_40001676_3", JsonConvert.SerializeObject(TH_40001676_10));
            //}
            ////广播发送JSON数据
            //_hubContext.Clients.All.sendTH3(TH3_json);

            ////四楼温湿度数据
            //JObject TH4_json = new JObject();   
            //using (var KY_db = new kongyadbEntities())
            //{
            //var TH_40001676_1 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "1") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40001676_2 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "2") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40001676_3 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "3") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40001676_4 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "4") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40001676_5 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "5") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //TH4_json.Add("TH_40001676_1", JsonConvert.SerializeObject(TH_40001676_1));                                                                                                  
            //TH4_json.Add("TH_40001676_2", JsonConvert.SerializeObject(TH_40001676_2));
            //TH4_json.Add("TH_40001676_3", JsonConvert.SerializeObject(TH_40001676_3));
            //TH4_json.Add("TH_40001676_4", JsonConvert.SerializeObject(TH_40001676_4));
            //TH4_json.Add("TH_40001676_5", JsonConvert.SerializeObject(TH_40001676_5));
            //}
            //_hubContext.Clients.All.sendTH4(TH4_json);

            ////五楼温湿度数据
            //JObject TH5_json = new JObject();  
            //using (var KY_db = new kongyadbEntities())
            //{
            //var TH_40000938_1 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "1") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_2 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "2") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_3 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "3") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_4 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "4") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_5 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "5") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_6 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "6") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_7 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "7") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_8 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "8") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_9 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "9") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_10 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "10") select m).OrderByDescending(p => p.id).FirstOrDefault();  
            //var TH_40000938_11 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "11") select m).OrderByDescending(p => p.id).FirstOrDefault();  
            //var TH_40000938_12 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "12") select m).OrderByDescending(p => p.id).FirstOrDefault();      
            //TH5_json.Add("TH_400938_1", JsonConvert.SerializeObject(TH_40000938_1));
            //TH5_json.Add("TH_400938_2", JsonConvert.SerializeObject(TH_40000938_2));
            //TH5_json.Add("TH_400938_3", JsonConvert.SerializeObject(TH_40000938_3));
            //TH5_json.Add("TH_400938_4", JsonConvert.SerializeObject(TH_40000938_4));
            //TH5_json.Add("TH_400938_5", JsonConvert.SerializeObject(TH_40000938_5));
            //TH5_json.Add("TH_400938_6", JsonConvert.SerializeObject(TH_40000938_6));
            //TH5_json.Add("TH_400938_7", JsonConvert.SerializeObject(TH_40000938_7));
            //TH5_json.Add("TH_400938_8", JsonConvert.SerializeObject(TH_40000938_8));
            //TH5_json.Add("TH_400938_9", JsonConvert.SerializeObject(TH_40000938_9));
            //TH5_json.Add("TH_400938_10", JsonConvert.SerializeObject(TH_40000938_10));
            //TH5_json.Add("TH_400938_11", JsonConvert.SerializeObject(TH_40000938_11));
            //TH5_json.Add("TH_400938_12", JsonConvert.SerializeObject(TH_40000938_12));
            //}
            //_hubContext.Clients.All.sendTH5(TH5_json);

            ////六楼温湿度数据
            //JObject TH6_json = new JObject();   
            //using (var KY_db = new kongyadbEntities())
            //{
            //var TH6_room = (from m in db.room select m).OrderByDescending(p => p.id).FirstOrDefault();
            //TH6_json.Add("room6", JsonConvert.SerializeObject(TH6_room));
            //}
            //_hubContext.Clients.All.sendTH6(TH6_json);
            #endregion
        }

        public static Broadcaster Instance
        {
            get
            {
                return _instance.Value;
            }
        }
    }
}