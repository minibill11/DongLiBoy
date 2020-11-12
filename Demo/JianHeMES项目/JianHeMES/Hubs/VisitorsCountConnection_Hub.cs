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
using System.Diagnostics;

namespace JianHeMES.Hubs
{
    [HubName("VCC")]
    public class VisitorsCountConnection: PersistentConnection
    {
        private static int connections = 0;
        public class ConnnectionUser
        {
            public string UserName { get; set; }
            public string UserNumber { get; set; }
            public string Department { get; set; }
            public string ConnectionId { get; set; }
        }
        public static List<ConnnectionUser> connnectionUserLists = new List<ConnnectionUser>();
        protected override Task OnConnected(IRequest request, string connectionId)
        {
            Interlocked.Increment(ref connections);
            //var userNumber =  request.Headers[];
            ConnnectionUser connectionUser = new ConnnectionUser();
            AuthInfo auth = new AuthInfo();//(AuthInfo)this.request.RequestContext.RouteData.Values["Authorization"];
            connectionUser.UserName = auth.UserNumber;
            connectionUser.ConnectionId = connectionId;
            connnectionUserLists.Add(connectionUser);
            Debug.WriteLine("Visitors:" + connections);
            return base.OnConnected(request, connectionId);
        }
        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {
            Interlocked.Decrement(ref connections);
            Debug.WriteLine("Visitors:" + connections);
            return base.OnDisconnected(request, connectionId, stopCalled);
        }
        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            var message = JsonConvert.DeserializeObject<ChatMessageInfo>(data);
            if(String.IsNullOrEmpty(message.Message))
            {
               
            }

            return base.OnReceived(request, connectionId, data);
        }
    }
}