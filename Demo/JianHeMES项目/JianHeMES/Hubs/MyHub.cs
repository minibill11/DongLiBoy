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
        private kongyadbEntities db = new kongyadbEntities();

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


        private Random random = new Random();


        private void BroadcastShape(object state)
        {
            // 定期执行的方法
            // _hubContext.Clients.All.sendTest1(random.Next(1000).ToString());

            JObject json = new JObject();
            json.Add("A", random.Next(1000, 10000).ToString());
            json.Add("B", random.Next(20).ToString());
            _hubContext.Clients.All.sendTest1(json);


            JObject KY_json = new JObject();
            var KY_aircomp1 = (from m in db.aircomp1 select m).OrderByDescending(p => p.id).FirstOrDefault();
            string aircomp1 = "{'id':'" + KY_aircomp1.id.ToString() + "','pressure':'" + KY_aircomp1.pressure.ToString() + "','temperature':'" + KY_aircomp1.temperature.ToString() + "','current_u':'" + KY_aircomp1.current_u.ToString() + "','status':'" + KY_aircomp1.status.ToString() + "'}";
            var KY_aircomp2 = (from m in db.aircomp2 select m).OrderByDescending(p => p.id).FirstOrDefault();
            string aircomp2 = "{'id':'" + KY_aircomp2.id.ToString() + "','pressure':'" + KY_aircomp2.pressure.ToString() + "','temperature':'" + KY_aircomp2.temperature.ToString() + "','current_u':'" + KY_aircomp2.current_u.ToString() + "','status':'" + KY_aircomp2.status.ToString() + "'}";
            var KY_aircomp3 = (from m in db.aircomp3 select m).OrderByDescending(p => p.id).FirstOrDefault();
            string aircomp3 = "{'id':'" + KY_aircomp3.id.ToString() + "','pressure':'" + KY_aircomp3.pressure.ToString() + "','temperature':'" + KY_aircomp3.temperature.ToString() + "','current_u':'" + KY_aircomp3.current_u.ToString() + "','status':'" + KY_aircomp3.status.ToString() + "'}";

            KY_json.Add("A3",JsonConvert.SerializeObject(KY_aircomp1));
            KY_json.Add("aircomp1", aircomp1);
            KY_json.Add("aircomp2", aircomp2);
            KY_json.Add("aircomp3", aircomp3);

            //JObject aa = new JObject();
            //aa.Add("id",KY_aircomp1.id.ToString());
            //aa.Add("pressure",KY_aircomp1.pressure.ToString());
            _hubContext.Clients.All.sendTest2(KY_json);
           
            //var KY_aircomp2 = (from m in db.aircomp2 select m).Last();
            //var KY_aircomp3 = (from m in db.aircomp3 select m).Last();
            
            //KY_aircomp_json.Add(KY_aircomp1);
            //string aaa = JsonConvert.SerializeObject(KY_aircomp1);
            //KY_aircomp_json.Add("A1", aaa);
            //KY_aircomp_json.Add(KY_aircomp2);
            //KY_aircomp_json.Add(KY_aircomp3);

            //JObject KY_comproom_json = new JObject();
            //var KY_airbottle1 = (from m in db.airbottle1 select m).Last();
            //var KY_airbottle2 = (from m in db.airbottle2 select m).Last();
            //var KY_airbottle3 = (from m in db.airbottle3 select m).Last();
            //var KY_dryer1 = (from m in db.dryer1 select m).Last();
            //var KY_dryer2 = (from m in db.dryer2 select m).Last();
            //var KY_headerpipe3inch = (from m in db.headerpipe3inch select m).Last();
            //var KY_headerpipe4inch = (from m in db.headerpipe4inch select m).Last();
            //KY_comproom_json.Add(KY_airbottle1);
            //KY_comproom_json.Add(KY_airbottle2);
            //KY_comproom_json.Add(KY_airbottle3);
            //KY_comproom_json.Add(KY_dryer1);
            //KY_comproom_json.Add(KY_dryer2);
            //KY_comproom_json.Add(KY_headerpipe3inch);
            //KY_comproom_json.Add(KY_headerpipe4inch);
            //KY_comproom_json.Add(KY_aircomp1);
            //KY_comproom_json.Add(KY_aircomp2);
            //KY_comproom_json.Add(KY_aircomp3);

            //ViewData["data"] = JsonConvert.SerializeObject(KY_aircomp1);
            //ViewData["data"] = JsonConvert.SerializeObject(CalibrationRecordVM.AllCalibrationRecord.OrderByDescending(m => m.BeginCalibration).ToList());

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