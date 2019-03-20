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


    [HubName("Hub5")]
    public class Hub5 : Hub
    {
        // Is set via the constructor on each creation
        private Broadcaster5 _broadcaster5;
        public Hub5()
            : this(Broadcaster5.Instance5)
        {
        }
        public Hub5(Broadcaster5 broadcaster5)
        {
            _broadcaster5 = broadcaster5;
        }
    }

    /// <summary>
    /// 数据广播器
    /// </summary>
    public class Broadcaster5
    {
        private readonly static Lazy<Broadcaster5> _instance5 =
            new Lazy<Broadcaster5>(() => new Broadcaster5());

        private readonly IHubContext _hubContext5;

        private Timer _broadcastLoop5;

        public Broadcaster5()
        {
            // 获取所有连接的句柄，方便后面进行消息广播
            _hubContext5 = GlobalHost.ConnectionManager.GetHubContext<Hub5>();
            // Start the broadcast loop
            _broadcastLoop5 = new Timer(
                BroadcastShape,
                null,
                0,
                10000);
        }

        private void BroadcastShape(object state)
        {   // 定期执行的方法
            //五楼温湿度数据
            JObject TH5_json = new JObject();
            using (var db = new kongyadbEntities())
            {
                var TH_40021210_1 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40021210" && m.NodeID == "1") select m).FirstOrDefault();
                var TH_40021210_2 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40021210" && m.NodeID == "2") select m).FirstOrDefault();
                var TH_40000938_6 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40000938" && m.NodeID == "6") select m).FirstOrDefault();
                var TH_40000938_7 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40000938" && m.NodeID == "7") select m).FirstOrDefault();
                var TH_40000938_8 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40000938" && m.NodeID == "8") select m).FirstOrDefault();
                var TH_40000938_9 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40000938" && m.NodeID == "9") select m).FirstOrDefault();
                var TH_40000938_10 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40000938" && m.NodeID == "10") select m).FirstOrDefault();
                var TH_40000938_11 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40000938" && m.NodeID == "11") select m).FirstOrDefault();
                var TH_40000938_12 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40000938" && m.NodeID == "12") select m).FirstOrDefault();
                TH5_json.Add("TH_40021210_1", JsonConvert.SerializeObject(TH_40021210_1));
                TH5_json.Add("TH_40021210_2", JsonConvert.SerializeObject(TH_40021210_2));
                TH5_json.Add("TH_40000938_6", JsonConvert.SerializeObject(TH_40000938_6));
                TH5_json.Add("TH_40000938_7", JsonConvert.SerializeObject(TH_40000938_7));
                TH5_json.Add("TH_40000938_8", JsonConvert.SerializeObject(TH_40000938_8));
                TH5_json.Add("TH_40000938_9", JsonConvert.SerializeObject(TH_40000938_9));
                TH5_json.Add("TH_40000938_10", JsonConvert.SerializeObject(TH_40000938_10));
                TH5_json.Add("TH_40000938_11", JsonConvert.SerializeObject(TH_40000938_11));
                TH5_json.Add("TH_40000938_12", JsonConvert.SerializeObject(TH_40000938_12));
            }
            _hubContext5.Clients.All.sendTH5(TH5_json);

        }

        public static Broadcaster5 Instance5
        {
            get
            {
                return _instance5.Value;
            }
        }
    }
}