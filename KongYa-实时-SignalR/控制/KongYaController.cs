using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using JianHeMES.Models;
using Newtonsoft.Json;
using JianHeMES.Areas.KongYaHT.Models;


namespace JianHeMES.Areas.kongya.Controllers
{
    public class KongYaController : Controller
    {
        private kongyadbEntities db =new kongyadbEntities();

        // GET: kongya/KongYa
        public ActionResult KongYaIndex()
        {
            return View();
        }
        public ActionResult ThirdFloor()
        {
            return View();
        }
        public ActionResult FourthFloor()
        {
            return View();
        }

        public ActionResult FifthFloor()
        {
            return View();
        }

        public ActionResult Sixthfloor()
        {
            return View();
        }
        public ActionResult FloorDataShow()
        {
            return View();
        }

        public ActionResult FloorData3()
        {
            return View();
        }
        public ActionResult FloorData4()
        {
            return View();
        }
        public ActionResult FloorData5()
        {
            return View();
        }
        public ActionResult FloorData6()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Sixthfloor(DateTime a,DateTime b)
        {
            kongyadbEntities db =new kongyadbEntities();
            IQueryable<room> rooms = null;
            TimeSpan sectionTime = new TimeSpan(0,0,30,0);
            if (a!=null)
            {
                DateTime get_a = a;
                DateTime get_earler_a;
                get_earler_a = get_a - sectionTime;//时间运行，求更早的值
                rooms = from m in db.room
                        where (m.recordingTime >get_earler_a && m.recordingTime<get_a)
                        select m;
            }
            return View(rooms);
        }

    }
}