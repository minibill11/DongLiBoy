﻿using Owin;
using System.Threading;
using JianHeMES.Controllers;
using System.Web.Hosting;
using Microsoft.AspNet.SignalR;
using System;
using JianHeMES.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(JianHeMES.Startup))]

namespace JianHeMES
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.UseCors(CorsOptions.AllowAll);//允许跨域请求
            app.MapSignalR();//Hub连接
            SignalRConfig.Setup(app);//持久连接LongPolling
            CycleRuning cycleRuning = new CycleRuning();
            KPIController cycleRuning1 = new KPIController();
            Thread Small_Sample_EmailSend = new Thread(new ThreadStart(cycleRuning.Small_Sample_EmailSend));//小样
            Small_Sample_EmailSend.Start();
            //Thread EquipmentMonthMain_Email = new Thread(new ThreadStart(cycleRuning.MonthMain_Email));//设备月保养时间计划表
            //EquipmentMonthMain_Email.Start();
            //Thread EquipmentSafetystock_Email = new Thread(new ThreadStart(cycleRuning.Safetystock_Email));//设备安全库存清单
            //EquipmentSafetystock_Email.Start();
            //Thread CheckHarvesterProgram = new Thread(new ThreadStart(cycleRuning.CheckHarvesterProgram));//空压房
            //CheckHarvesterProgram.Start();

            //Thread TemperatureAndHumidity = new Thread(new ThreadStart(cycleRuning.TemperatureAndHumidity));//全厂温湿度监控
            //TemperatureAndHumidity.Start();

            Thread KPI_7S_daily = new Thread(new ThreadStart(cycleRuning1.KPI_7S_daily));//7S日报扣分
            KPI_7S_daily.Start();
        }
    }

    public class SignalRConfig
    {
        public static void Setup(IAppBuilder app)
        {
            var config = new HubConfiguration() { EnableJSONP = true };
            app.MapSignalR("/chathub", config);//添加跨域Hub接口名字和格式标准
            app.MapSignalR("/Hub1", config);//添加跨域Hub接口：一楼温湿度
            app.MapSignalR("/Hub2", config);//添加跨域Hub接口：二楼温湿度
            app.MapSignalR("/Hub3", config);//添加跨域Hub接口：三楼温湿度
            app.MapSignalR("/Hub4", config);//添加跨域Hub接口：四楼温湿度
            app.MapSignalR("/Hub5", config);//添加跨域Hub接口：五楼温湿度
            app.MapSignalR("/Hub6", config);//添加跨域Hub接口：六楼温湿度
            app.MapSignalR("/ModuleProductionControlIndex", config);//添加跨域Hub接口：模块看板
            app.MapSignalR("/ProductionControlIndex", config); //添加跨域Hub接口：生产管控看板
            app.MapSignalR("/Production_Value", config); //添加跨域Hub接口：产值看板
            app.MapSignalR("/myHub", config); //添加跨域Hub接口：空压机房
            app.MapSignalR("/OnlineUserManagement", config); //添加跨域Hub接口：在线用户管理
        }
    }
}


