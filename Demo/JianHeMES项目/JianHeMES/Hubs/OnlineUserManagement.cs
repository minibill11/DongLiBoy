using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace JianHeMES.Areas.KongYaHT.Hubs
{

    [HubName("OnlineUserManagement")]
    public class OnlineUserManagement : Hub
    {
        static long counter = 0;//在线人数
        static long user_counter = 0; //在线用户人数

        public class online_user
        {
            public int userNum { get; set; }
            public string userName { get; set; }
            public string userDepartment { get; set; }
            public string userGroup { get; set; }
        }

        List<online_user> onlineUser = new List<online_user>();
        public override System.Threading.Tasks.Task OnConnected()//建立连接时访问
        {
            counter = counter + 1;//当用户进入页面在线人数加一
            Clients.All.UpdateCount(counter);//调用客户端方法更新接口
            return base.OnConnected();
        }
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)//断开连接时访问
        {
            counter = counter - 1;
            Clients.All.UpdateCount(counter);
            return base.OnDisconnected(stopCalled);
        }

        public void Hello()
        {
            Clients.All.hello();
        }
    }
}