﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using JianHeMES.Models;
using JianHeMES.Migrations;
using System.Web.Http;
using StackExchange.Profiling.EntityFramework6;
using StackExchange.Profiling;

namespace JianHeMES
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            MiniProfilerEF6.Initialize();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(App_Start.WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //启用压缩
            BundleTable.EnableOptimizations = true;
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)//这里是允许本地访问启动监控,可不写
            {
                MiniProfiler.Start();

            }
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }
    }
}
