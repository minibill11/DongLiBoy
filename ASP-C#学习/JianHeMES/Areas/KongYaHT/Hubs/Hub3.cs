﻿using JianHeMES.Models;
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


    [HubName("Hub3")]
    public class Hub3 : Hub
    {
        // Is set via the constructor on each creation
        private Broadcaster3 _broadcaster3;
        public Hub3()
            : this(Broadcaster3.Instance3)
        {
        }
        public Hub3(Broadcaster3 broadcaster3)
        {
            _broadcaster3 = broadcaster3;
        }
    }

    /// <summary>
    /// 数据广播器
    /// </summary>
    public class Broadcaster3
    {
        private readonly static Lazy<Broadcaster3> _instance3 =
            new Lazy<Broadcaster3>(() => new Broadcaster3());

        private readonly IHubContext _hubContext3;

        private Timer _broadcastLoop3;

        public Broadcaster3()
        {
            // 获取所有连接的句柄，方便后面进行消息广播
            _hubContext3 = GlobalHost.ConnectionManager.GetHubContext<Hub3>();
            // Start the broadcast loop
            _broadcastLoop3 = new Timer(
                BroadcastShape,
                null,
                0,
                1000);
        }

        private void BroadcastShape(object state)
        {   // 定期执行的方法
            //三楼温湿度数据
            JObject TH3_json = new JObject();   //创建JSON对象
            //取出数据
            using (var db = new kongyadbEntities())
            {
                var TH_40001676_6 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40001676" && m.NodeID == "6") select m).FirstOrDefault();
                var TH_40001676_9 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40001676" && m.NodeID == "9") select m).FirstOrDefault();
                var TH_40001676_10 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40001676" && m.NodeID == "10") select m).FirstOrDefault();
                var TH_40004493_2 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004493" && m.NodeID == "2") select m).FirstOrDefault();

                
                //存入JSON对象
                TH3_json.Add("TH_40001676_6", JsonConvert.SerializeObject(TH_40001676_6));
                TH3_json.Add("TH_40001676_9", JsonConvert.SerializeObject(TH_40001676_9));
                TH3_json.Add("TH_40001676_10", JsonConvert.SerializeObject(TH_40001676_10));
                TH3_json.Add("TH_40004493_2", JsonConvert.SerializeObject(TH_40004493_2));
            }
            //广播发送JSON数据
            _hubContext3.Clients.All.sendTH3(TH3_json);

        }

        public static Broadcaster3 Instance3
        {
            get
            {
                return _instance3.Value;
            }
        }
    }
}