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
using System.Diagnostics;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace JianHeMES.Hubs
{
    /// < summary>
    /// SmartEMSHub 这是我们要定义的hub
    /// </ summary>
    public class SmartEMSHub : Hub
    {
        /// < summary>
        /// 
        /// </ summary>
        public static List<string> Users = new List<string>();

        /// <summary>
        /// The OnConnected event.
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            string clientId = GetClientId();
            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);
            }
            Send(Users.Count);
            var context = GlobalHost.ConnectionManager.GetHubContext<SmartEMSHub>();
            context.Clients.Client(clientId).updateUserName(clientId);
            return base.OnConnected();
        }

        /// <summary>
        /// The OnReconnected event.
        /// </summary>
        /// <returns></returns>
        public override Task OnReconnected()
        {
            string clientId = GetClientId();
            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);
            }
            Send(Users.Count);
            return base.OnReconnected();
        }

        /// <summary>
        /// The OnDisconnected event.
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            string clientId = GetClientId();

            if (Users.IndexOf(clientId) > -1)
            {
                Users.Remove(clientId);
            }
            Send(Users.Count);
            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Get's the currently connected Id of the client.
        /// This is unique for each client and is used to identify
        /// a connection.
        /// </summary>
        /// <returns></returns>
        private string GetClientId()
        {
            string clientId = "";

            // clientId passed from application 
            if (Context.QueryString["clientId"] != null)
            {
                clientId = this.Context.QueryString["clientId"];
            }

            if (string.IsNullOrEmpty(clientId.Trim()))
            {
                clientId = Context.ConnectionId;
            }

            return clientId;
        }

        /// <summary>
        /// Sends the update user count to the listening view.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        public void Send(int count)
        {
            // Call the addNewMessageToPage method to update clients.
            var context = GlobalHost.ConnectionManager.GetHubContext<SmartEMSHub>();
            context.Clients.All.updateUsersOnlineCount(count);
        }
        /// <summary>
        /// 自己写的一个服务端方法Hello.
        /// </summary>
        /// <param name="msg">参数
        /// </param>
        public void Hello(string msg)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<SmartEMSHub>();
            context.Clients.All.clientMethod("server:" + msg);
        }
    }

    [HubName("ChatHub")]
    public class ChatHub2 : PersistentConnection
    {
        public JObject SendMsg(ChatMessageInfo info)
        {
            JObject res = new JObject();
            res.Add("username", info.UserName+"ChatHub");
            res.Add("message", info.Message);
            //这里的Show代表是客户端的方法，具体可以细看SignalR的说明
            //return await Clients.All.SendAsync("Show", info.UserName + ":" + info.Message);
            return res;
        }

    }





    [HubName("TestHub")]
    public class TestHub : Hub
    {
        private Broadcaster1 _broadcaster1;

        private readonly IHubContext _hubContext1;
        private Timer _broadcastLoop1;
        //public TestHub() : this(Instance1)
        //{
        //}
        public TestHub(Broadcaster1 broadcaster1)
        {
            _broadcaster1 = broadcaster1;
        }

        public void Broadcaster1()
        {
            //// 获取所有连接的句柄，方便后面进行消息广播
            //_hubContext1 = GlobalHost.ConnectionManager.GetHubContext<TestHub>();
            // Start the broadcast loop
            _broadcastLoop1 = new Timer(
                SendMsg,
                null,
                0,
                1000);
        }

        [HubMethodName("Send")]
        public void Send(string name, string message)
        {
            name = "He " + name;
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }

        private void SendMsg(object state)
        {

            JObject res = new JObject();
            res.Add("username", "TestHub的消息");
            res.Add("message", "结果是这样的");
            //这里的Show代表是客户端的方法，具体可以细看SignalR的说明
            Clients.All.sendTestHub(res);
        }

        public void SendMsg2(ChatMessageInfo info)
        {

            JObject res = new JObject();
            res.Add("username", info.UserName + "TestHub");
            res.Add("message", info.Message);
            //这里的Show代表是客户端的方法，具体可以细看SignalR的说明
            //return await Clients.All.SendAsync("Show", info.UserName + ":" + info.Message);
            Clients.All.sendTestHub2(res);

        }


        private JObject SendMsg3(object state)
        {

            JObject res = new JObject();
            res.Add("username", "TestHub的消息");
            res.Add("message", "结果是这样的");
            //这里的Show代表是客户端的方法，具体可以细看SignalR的说明
           return res;
        }

        public void ServerShow(string text)
        {
            Clients.All.clientShow(text);
        }


        public void Test(string msg)
        {
            Clients.All.test(msg); //←framework的写法

            //Clients.All.InvokeAsync("test", msg); //←core的写法
            //Clients.All.InvokeAsync("test", "参数1", "参数2", "参数3");
        }

        //public static Broadcaster1 Instance1
        //{
        //    get
        //    {
        //        return _instance1.Value;
        //    }
        //}

        //protected Task OnConnected(IRequest request, string connectionId)
        //{
        //    ////在数据库中存储新的连接
        //    //await _service.SaveNewConnectionAsync(connectionId);
        //    string message = request.User.Identity.IsAuthenticated ? "welcome," + request.User.Identity.Name : "You must be logged in!";
        //    return _hubContext1.Clients.Client.SendMsg(connectionId, message);
        //}

    }

    public class ChatHub3 : Hub
    {

        public Task SendMsg(string username, string message)
        {
            return Clients.All.SendAsync("Show", username, message);
        }
        //public Task SendMsgToUser(int userid,string message)
        //{
        //    return Clients.Client(userid).SendAsync();
        //}
    }


    public class ChatMessageInfo
    {
        public string UserName { get; set; }
        public string Message { get; set; }
    }


    //[HubName("TH_Hub")]
    //public class TH_Hub : Hub
    //{
    //    //private readonly IHubContext<TH_Hub> _hubContext;
    //    //public HubController(IHubContext<TH_Hub> hubContext)
    //    //{
    //    //    _hubContext = hubContext;
    //    //}

    //    public Task SendMsg(string info)
    //    { //这里的Show代表是客户端的方法，具体可以细看SignalR的说明
    //        //return Clients.All.SendAsync("Show", info.UserName + ":" + info.Message);
    //        if(info == "FirstFloorTH")
    //        {
    //            return Clients.All.SendAsync("FirstFloorTH", FirstFloorTH());
    //        }
    //        return Clients.All.SendAsync("FirstFloorTH", FirstFloorTH());
    //    }

    //    public JObject FirstFloorTH()
    //    {   // 定期执行的方法
    //        //一楼温湿度数据
    //        JObject TH1_json = new JObject();   //创建JSON对象
    //                                            //取出数据
    //        using (var db = new kongyadbEntities())
    //        {
    //            var TH_40004493_1 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004493" && m.NodeID == "1") select m).FirstOrDefault();
    //            var TH_40004518_1 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004518" && m.NodeID == "1") select m).FirstOrDefault();//二楼
    //            var TH_40004518_2 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004518" && m.NodeID == "2") select m).FirstOrDefault();
    //            var TH_40004518_3 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004518" && m.NodeID == "3") select m).FirstOrDefault();
    //            var TH_40004518_6 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004518" && m.NodeID == "6") select m).FirstOrDefault();
    //            var TH_40004518_4 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004518" && m.NodeID == "4") select m).FirstOrDefault();
    //            var TH_40004518_5 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004518" && m.NodeID == "5") select m).FirstOrDefault();

    //            var TH_40004557_1 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004557" && m.NodeID == "1") select m).FirstOrDefault();
    //            var TH_40004557_2 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004557" && m.NodeID == "2") select m).FirstOrDefault();
    //            var TH_40004557_3 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004557" && m.NodeID == "3") select m).FirstOrDefault();
    //            var TH_40004557_4 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004557" && m.NodeID == "4") select m).FirstOrDefault();
    //            var TH_40004557_5 = (from m in db.THhistory.OrderByDescending(p => p.id) where (m.DeviceID == "40004557" && m.NodeID == "5") select m).FirstOrDefault();
    //            var Noise = db.THhistory.OrderByDescending(c => c.id).Where(c => c.DeviceID == "40004518" && c.NodeID == "32").FirstOrDefault();
    //            //存入JSON对象
    //            TH1_json.Add("TH_40004493_1", JsonConvert.SerializeObject(TH_40004493_1));
    //            TH1_json.Add("TH_40004518_1", JsonConvert.SerializeObject(TH_40004518_1));
    //            TH1_json.Add("TH_40004518_2", JsonConvert.SerializeObject(TH_40004518_2));
    //            TH1_json.Add("TH_40004518_3", JsonConvert.SerializeObject(TH_40004518_3));
    //            TH1_json.Add("TH_40004518_6", JsonConvert.SerializeObject(TH_40004518_6));
    //            TH1_json.Add("TH_40004518_4", JsonConvert.SerializeObject(TH_40004518_4));
    //            TH1_json.Add("TH_40004518_5", JsonConvert.SerializeObject(TH_40004518_5));
    //            TH1_json.Add("TH_40004557_1", JsonConvert.SerializeObject(TH_40004557_1));
    //            TH1_json.Add("TH_40004557_2", JsonConvert.SerializeObject(TH_40004557_2));
    //            TH1_json.Add("TH_40004557_3", JsonConvert.SerializeObject(TH_40004557_3));
    //            TH1_json.Add("TH_40004557_4", JsonConvert.SerializeObject(TH_40004557_4));
    //            TH1_json.Add("TH_40004557_5", JsonConvert.SerializeObject(TH_40004557_5));

    //            TH1_json.Add("Noise", JsonConvert.SerializeObject(Noise));

    //        }
    //        //广播发送JSON数据
    //        //_hubContext1.Clients.All.sendTH1(TH1_json);
    //        return TH1_json;
    //    }
    //}

}