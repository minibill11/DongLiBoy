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


    [HubName("Hub4")]
    public class Hub4 : Hub
    {
        // Is set via the constructor on each creation
        private Broadcaster4 _broadcaster4;
        public Hub4()
            : this(Broadcaster4.Instance4)
        {
        }
        public Hub4(Broadcaster4 broadcaster4)
        {
            _broadcaster4 = broadcaster4;
        }
    }

    /// <summary>
    /// 数据广播器
    /// </summary>
    public class Broadcaster4
    {
        private readonly static Lazy<Broadcaster4> _instance4 =
            new Lazy<Broadcaster4>(() => new Broadcaster4());

        private readonly IHubContext _hubContext4;

        private Timer _broadcastLoop4;

        public Broadcaster4()
        {
            // 获取所有连接的句柄，方便后面进行消息广播
            _hubContext4 = GlobalHost.ConnectionManager.GetHubContext<Hub4>();
            // Start the broadcast loop
            _broadcastLoop4 = new Timer(
                BroadcastShape,
                null,
                0,
                10000);
        }

        private void BroadcastShape(object state)
        {   // 定期执行的方法
            //四楼温湿度数据
            JObject TH4_json = new JObject();
            using (var db = new kongyadbEntities())
            {
                var TH_40001676_4 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40001676" && m.NodeID == "4") select m).FirstOrDefault();
                var TH_40021216_1 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40021216" && m.NodeID == "1") select m).FirstOrDefault();
                var TH_40021216_2 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40021216" && m.NodeID == "2") select m).FirstOrDefault();
                var TH_40021216_3 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40021216" && m.NodeID == "3") select m).FirstOrDefault();
                var TH_40021216_4 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40021216" && m.NodeID == "4") select m).FirstOrDefault();
                var TH_40004493_3 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004493" && m.NodeID == "3") select m).FirstOrDefault();
                var TH_40004493_4 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004493" && m.NodeID == "4") select m).FirstOrDefault();
                var TH_40004493_5 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004493" && m.NodeID == "5") select m).FirstOrDefault();
                var TH_40004493_6 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004493" && m.NodeID == "6") select m).FirstOrDefault();
                var TH_40004493_7 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004493" && m.NodeID == "7") select m).FirstOrDefault();
                var TH_40004493_8 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004493" && m.NodeID == "8") select m).FirstOrDefault();
                TH4_json.Add("TH_40001676_4", JsonConvert.SerializeObject(TH_40001676_4));
                TH4_json.Add("TH_40004493_3", JsonConvert.SerializeObject(TH_40004493_3));
                TH4_json.Add("TH_40004493_4", JsonConvert.SerializeObject(TH_40004493_4));
                TH4_json.Add("TH_40004493_5", JsonConvert.SerializeObject(TH_40004493_5));
                TH4_json.Add("TH_40004493_6", JsonConvert.SerializeObject(TH_40004493_6));
                TH4_json.Add("TH_40004493_7", JsonConvert.SerializeObject(TH_40004493_7));
                TH4_json.Add("TH_40004493_8", JsonConvert.SerializeObject(TH_40004493_8));
                TH4_json.Add("TH_40021216_1", JsonConvert.SerializeObject(TH_40021216_1));
                TH4_json.Add("TH_40021216_2", JsonConvert.SerializeObject(TH_40021216_2));
                TH4_json.Add("TH_40021216_3", JsonConvert.SerializeObject(TH_40021216_3));
                TH4_json.Add("TH_40021216_4", JsonConvert.SerializeObject(TH_40021216_4));
            }
            _hubContext4.Clients.All.sendTH4(TH4_json);

        }

        public static Broadcaster4 Instance4
        {
            get
            {
                return _instance4.Value;
            }
        }
    }
}