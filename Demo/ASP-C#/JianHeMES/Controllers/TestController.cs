using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json;
using JianHeMES.Hubs;

namespace JianHeMES.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult CSVtest()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CSVtest(Array file)
        {

            return View();
        }


        [HttpPost]
        public ActionResult Index(string jsondata)   //接收jsondata
        {
            CalibrationRecord data = JsonConvert.DeserializeObject<CalibrationRecord>(jsondata);   //使用JsonConvert.DeserializeObject<类名>（字符串）来把传过来的字符串解析为对应的类
            return View();
        }

    }
}