using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class ProductionControlController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Assemble_CompeleteRate(string OrderNum, string Content)
        {

            return View();
        }

        public ActionResult Assemble_PassRate()
        {

            return View();
        }
        public ActionResult Burn_in_CompeleteRate()
        {

            return View();
        }

        public ActionResult Burn_in_PassRate()
        {

            return View();
        }
        public ActionResult Calibration_CompeleteRate()
        {

            return View();
        }

        public ActionResult Calibration_PassRate()
        {

            return View();
        }
        public ActionResult Appearances_CompeleteRate()
        {

            return View();
        }

        public ActionResult Appearances_PassRate()
        {

            return View();
        }

    }
}