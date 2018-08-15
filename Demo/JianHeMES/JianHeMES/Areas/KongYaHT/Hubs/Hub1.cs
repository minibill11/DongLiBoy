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


    [HubName("Hub1")]
    public class Hub1 : Hub
    {
        // Is set via the constructor on each creation
        private Broadcaster1 _broadcaster1;
        public Hub1()
            : this(Broadcaster1.Instance1)
        {
        }
        public Hub1(Broadcaster1 broadcaster1)
        {
            _broadcaster1 = broadcaster1;
        }
    }

    /// <summary>
    /// 数据广播器
    /// </summary>
    public class Broadcaster1
    {
        private readonly static Lazy<Broadcaster1> _instance1 =
            new Lazy<Broadcaster1>(() => new Broadcaster1());

        private readonly IHubContext _hubContext1;

        private Timer _broadcastLoop1;

        public Broadcaster1()
        {
            // 获取所有连接的句柄，方便后面进行消息广播
            _hubContext1 = GlobalHost.ConnectionManager.GetHubContext<Hub1>();
            // Start the broadcast loop
            _broadcastLoop1 = new Timer(
                BroadcastShape,
                null,
                0,
                1000);
        }

        private void BroadcastShape(object state)
        {   // 定期执行的方法
            //一楼温湿度数据
            JObject TH1_json = new JObject();   //创建JSON对象
            //取出数据
            using (var db = new kongyadbEntities())
            {
                var TH_40004493_1 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004493" && m.NodeID == "1") select m).FirstOrDefault();
                var TH_40004518_1 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004518" && m.NodeID == "1") select m).FirstOrDefault();
                var TH_40004518_2 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004518" && m.NodeID == "2") select m).FirstOrDefault();
                var TH_40004518_6 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004518" && m.NodeID == "6") select m).FirstOrDefault();
                var TH_40004518_4 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004518" && m.NodeID == "4") select m).FirstOrDefault();
                var TH_40004518_5 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004518" && m.NodeID == "5") select m).FirstOrDefault();

                //存入JSON对象
                TH1_json.Add("TH_40004493_1", JsonConvert.SerializeObject(TH_40004493_1));
                TH1_json.Add("TH_40004518_1", JsonConvert.SerializeObject(TH_40004518_1));
                TH1_json.Add("TH_40004518_2", JsonConvert.SerializeObject(TH_40004518_2));
                TH1_json.Add("TH_40004518_6", JsonConvert.SerializeObject(TH_40004518_6));
                TH1_json.Add("TH_40004518_4", JsonConvert.SerializeObject(TH_40004518_4));
                TH1_json.Add("TH_40004518_5", JsonConvert.SerializeObject(TH_40004518_5));

            }
            //广播发送JSON数据
            _hubContext1.Clients.All.sendTH1(TH1_json);

        }

        public static Broadcaster1 Instance1
        {
            get
            {
                return _instance1.Value;
            }
        }
    }
}