﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Routing;

namespace JianHeMES.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置跨域应用服务　
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors.SupportsCredentials=true);
            #region 跨域访问“Cors”配置及启用
            //var allowedMethods = ConfigurationManager.AppSettings["cors:allowedMethods"];
            //var allowedOrigin = ConfigurationManager.AppSettings["cors:allowedOrigin"];
            //var allowedHeaders = ConfigurationManager.AppSettings["cors:allowedHeaders"];
            //var geduCors = new EnableCorsAttribute(allowedOrigin, allowedHeaders, allowedMethods)
            //{
            //    SupportsCredentials = true
            //};
            //config.EnableCors(geduCors);
            #endregion
            // Web API 路由
            config.MapHttpAttributeRoutes();

            //将UTC转本地时间(LocalTime)
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new {httpMethod = new HttpMethodConstraint("POST")}
                );
        }
    }
}