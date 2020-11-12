﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace JianHeMES.Hubs
{
    public class ChatHub : Hub
    {
        public void Hello()
        {
            //Clients.All.hello();
            Clients.All.welcome("大家好,欢迎阅读本篇文章");
        }

        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public void Send1(string name, string message)
        {
            //var identity = this.Clients.Caller;
            string connectionId = "";
            Clients.All.addNewMessageToPage1(name, message, connectionId);
        }


    }
}