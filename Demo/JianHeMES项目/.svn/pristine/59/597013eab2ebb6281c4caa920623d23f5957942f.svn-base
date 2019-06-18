using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace JianHeMES
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces:new string[] { "JianHeMES.Controllers" } //new一个namespaces
            );


            //routes.MapRoute(
            //    name: "CalibrationRecord",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "CalibrationRecords", action = "Create", id = UrlParameter.Optional },
            //    namespaces: new string[] { "JianHeMES.Controllers" } //new一个namespaces
            //);

        }
    }
}
