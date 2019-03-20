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


    [HubName("Hub6")]
    public class Hub6 : Hub
    {
        // Is set via the constructor on each creation
        private Broadcaster6 _broadcaster6;
        public Hub6()
            : this(Broadcaster6.Instance6)
        {
        }
        public Hub6(Broadcaster6 broadcaster6)
        {
            _broadcaster6 = broadcaster6;
        }
    }

    /// <summary>
    /// 数据广播器
    /// </summary>
    public class Broadcaster6
    {
        private readonly static Lazy<Broadcaster6> _instance6 =
            new Lazy<Broadcaster6>(() => new Broadcaster6());

        private readonly IHubContext _hubContext6;

        private Timer _broadcastLoop6;

        public Broadcaster6()
        {
            // 获取所有连接的句柄，方便后面进行消息广播
            _hubContext6 = GlobalHost.ConnectionManager.GetHubContext<Hub6>();
            // Start the broadcast loop
            _broadcastLoop6 = new Timer(
                BroadcastShape,
                null,
                0,
                10000);
        }

        private void BroadcastShape(object state)
        {   // 定期执行的方法
            //六楼温湿度数据
            JObject TH6_json = new JObject();
            using (var db = new kongyadbEntities())
            {
                var TH6_room = (from m in db.room.OrderByDescending(p => p.id) select m).FirstOrDefault();
                TH6_json.Add("room6", JsonConvert.SerializeObject(TH6_room));
            }
            _hubContext6.Clients.All.sendTH6(TH6_json);

        }

        public static Broadcaster6 Instance6
        {
            get
            {
                return _instance6.Value;
            }
        }
    }
}