using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json;

namespace JianHeMES.Controllers
{
    public class AssemblesController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        public Assemble assemble = null;


        #region  -----//IPQC异常列表-----------

        private List<SelectListItem> SetAbnormalList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem{   Text = "请选择维修情况", Value = ""  },
                new SelectListItem{   Text = "正常", Value = "1"  },
                new SelectListItem{   Text = "箱体表面", Value = "2"  },
                new SelectListItem{   Text = "丝印LOGO", Value = "3"  },
                new SelectListItem{   Text = "箱门/门锁", Value = "4"  },
                new SelectListItem{   Text = "面罩", Value = "5"  },
                new SelectListItem{   Text = "灯管", Value = "6"  },
                new SelectListItem{   Text = "指示灯/按钮/显示模块", Value = "7"  },
                new SelectListItem{   Text = "锁扣", Value = "8"  },
                new SelectListItem{   Text = "把手/提手", Value = "9"  },
                new SelectListItem{   Text = "航插座/防水索头", Value = "10"  },
                new SelectListItem{   Text = "模块", Value = "11"  },
                new SelectListItem{   Text = "红胶/三防漆", Value = "12"  },
                new SelectListItem{   Text = "线材/接扎线方式", Value = "13"  },
                new SelectListItem{   Text = "风扇/温控断路器/滤波器", Value = "14"  },
                new SelectListItem{   Text = "系统卡/功能卡", Value = "15"  },
                new SelectListItem{   Text = "电源/保护盖接线座/防尘网", Value = "16"  },
                new SelectListItem{   Text = "接地", Value = "17"  },
                new SelectListItem{   Text = "胶条", Value = "18"  },
                new SelectListItem{   Text = "箱内外观", Value = "19"  },
                new SelectListItem{   Text = "标识", Value = "20"  },
                new SelectListItem{   Text = "螺丝/压铆", Value = "21"  },
                new SelectListItem{   Text = "刮痕/掉漆喷漆/沙眼", Value = "22"  },
                new SelectListItem{   Text = "间隙/错位/高低不平", Value = "23"  },
                new SelectListItem{   Text = "安装孔", Value = "24"  },
                new SelectListItem{   Text = "插销/弹柱/定位柱", Value = "25"  },
                new SelectListItem{   Text = "各组件间无干涉", Value = "26"  },
                new SelectListItem{   Text = "模组对角平整缝隙", Value = "27"  },
                new SelectListItem{   Text = "拼装测试", Value = "28"  },
                new SelectListItem{   Text = "单色和全彩测试", Value = "29"  },
                new SelectListItem{   Text = "扫描检验", Value = "30"  },
                new SelectListItem{   Text = "单灯显示效果", Value = "31"  },
                new SelectListItem{   Text = "亮暗线", Value = "32"  },
                new SelectListItem{   Text = "指示灯/自检/LCD模块", Value = "33"  },
                new SelectListItem{   Text = "黑屏检验", Value = "34"  },
                new SelectListItem{   Text = "产品能效功率测试", Value = "35"  },
                new SelectListItem{   Text = "防尘", Value = "36"  },
                new SelectListItem{   Text = "防水", Value = "37"  }
            };
        }

        #endregion


        // GET: Assembles

        #region -------------组装模块工段-----------------

        // GET: Assembles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Assembles/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OrderNum,BoxBarCode,AssembleBT,AssembleFT,AssembleTime,AssembleFinish,WaterproofTestBT,WaterproofTestFT,WaterproofTestTime,WaterproofAbnormal,WaterproofMaintaince,WaterproofTestFinish,AssembleAdapterCardBT,AssembleAdapterCardFT,AssembleAdapterTime,AssembleAdapterFinish,ViewCheckBT,ViewCheckFT,ViewCheckTime,ViewCheckAbnormal,ViewCheckFinish,ElectricityCheckBT,ElectricityCheckFT,ElectricityCheckTime,ElectricityCheckAbnormal,ElectricityCheckFinish,IPQCCheckBT,IPQCCheckFT,IPQCCheckTime,IPQCCheckAbnormal,IPQCCheckFinish")] Assemble assemble)
        {
            if (ModelState.IsValid)
            {
                db.Assemble.Add(assemble);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(assemble);
        }

        [HttpGet]
        public ActionResult AssembleStationB()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssembleStationB([Bind(Include = "Id,OrderNum,BoxBarCode,AssembleBT,AssembleFT")]Assemble assemble)
        {
            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == assemble.BoxBarCode) == null)
            //if (db.BarCodes.Where(u => u.BarCodesNum == model.BoxBarCode) != null)
            {
                ModelState.AddModelError("", "框体条码不存在，请检查条码输入是否正确！");
                return View(assemble);
            }

            assemble.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == assemble.BoxBarCode).FirstOrDefault().OrderNum;
            assemble.AssembleBT = DateTime.Now;

            db.Assemble.Add(assemble);
            //db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();
            //db.Configuration.ValidateOnSaveEnabled = true;
            return RedirectToAction("AssembleStationF", new { assemble.Id});
        }

        // GET: Assembles/Edit/5
        public ActionResult AssembleStationF(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assemble assemble = db.Assemble.Find(id);
            if (assemble == null)
            {
                return HttpNotFound();
            }
            return View(assemble);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssembleStationF([Bind(Include = "Id,OrderNum,BoxBarCode,ModelCollections,AssembleBT,AssemblePrincipal,AssembleFT,AssembleTime,AssembleFinish")] Assemble assemble)
        {
            List<ModelCollections> modelCollections = new List<ModelCollections>();
            if (assemble.AssembleFT == null)
            {
                assemble.AssembleFT = DateTime.Now;
                var BC = assemble.AssembleBT.Value;
                var FC = assemble.AssembleFT.Value;
                var CT = FC - BC;
                assemble.AssembleTime = CT;
                assemble.AssembleFinish = true;
                modelCollections = JsonConvert.DeserializeObject<List<ModelCollections>>(assemble.ModelCollections.First());
                //assemble.AssembleTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
            }
            if (ModelState.IsValid)
            {
                db.Entry(assemble).State = EntityState.Modified;
                db.SaveChanges();
                foreach (var item in modelCollections)
                {
                    //item.OrderNum = assemble.OrderNum;
                    //item.BarCodesNum = assemble.BoxBarCode;
                    db.ModelCollections.Add(item);
                    db.SaveChanges();
                }
                return RedirectToAction("AssembleStationB", "Assembles");
            }
            return View(assemble);
        }

        public ActionResult WaterproofTestB()
        {
            return View();
        }
        #endregion

        #region -------------防水测试工段------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WaterproofTestB([Bind(Include = "Id,BoxBarCode,WaterproofTestBT")] Assemble assemble)
        {
            if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode) == null)
            {
                ModelState.AddModelError("", "框体条码在组装记录中找不到，请检查条码输入是否正确，或检查组装工作是否已经完成！");
                return View(assemble);
            }
            assemble = db.Assemble.FirstOrDefault(u=>u.BoxBarCode == assemble.BoxBarCode);

            if (assemble.AssembleFinish==false)
            {
                ModelState.AddModelError("", "组装工作尚未完成，不能进行防水工作！");
                return View(assemble);
            }

            if (assemble.WaterproofTestBT == null)
            {
                assemble.WaterproofTestBT = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("WaterproofTestF", new { assemble.Id });
            }
            //if (ModelState.IsValid)
            //{
            //    db.Entry(assemble).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("WaterproofTestF", new { assemble.Id });
            //}
            //return View(assemble);
            return View(assemble);
        }

        public ActionResult WaterproofTestF(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assemble assemble = db.Assemble.Find(id);
            if (assemble == null)
            {
                return HttpNotFound();
            }
            return View(assemble);
        }

        [HttpPost]
        public ActionResult WaterproofTestF([Bind(Include = "Id,BoxBarCode,WaterproofTestBT,WaterproofTestPrincipal,WaterproofTestFT,WaterproofTestTime,WaterproofAbnormal,WaterproofMaintaince,WaterproofTestFinish")] Assemble assemble)
        {
            assemble = db.Assemble.FirstOrDefault(u => u.Id == assemble.Id);
            if (assemble.WaterproofTestFT == null)
            {
                assemble.WaterproofTestFT = DateTime.Now;
                var BC = assemble.WaterproofTestBT.Value;
                var FC = assemble.WaterproofTestFT.Value;
                var CT = FC - BC;
                assemble.WaterproofTestTime = CT;
                assemble.WaterproofAbnormal = 0;
                assemble.WaterproofMaintaince = "待编写";
                assemble.WaterproofTestFinish = true;
                //assemble.AssembleTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
            }
            if (ModelState.IsValid)
            {
                db.Entry(assemble).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("WaterproofTestB", "Assembles");
            }
            return View(assemble);
        }
        #endregion

        #region 组装电源、转接卡工段
        /// <summary>
        /// 电源、转接卡工段
        /// </summary>
        // TODO : 增加电源和转接卡条码号存储功能
        public ActionResult AssembleAdapterCardB()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssembleAdapterCardB([Bind(Include = "Id,BoxBarCode,AssembleAdapterCardBT,AssembleAdapterCardPrincipal")] Assemble assemble)
        {

            if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode) == null)
            {
                ModelState.AddModelError("", "框体条码在组装记录中找不到，请检查条码输入是否正确，或检查防水测试工作是否已经完成！");
                return View(assemble);
            }
            assemble = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);

            if (assemble.WaterproofTestFinish == false)
            {
                ModelState.AddModelError("", "防水测试工作尚未完成，不能进行转接卡及电源组装工作！");
                return View(assemble);
            }

            if (assemble.AssembleAdapterCardBT == null)
            {
                assemble.AssembleAdapterCardBT = DateTime.Now;
                assemble.AssembleAdapterCardPrincipal = "";
                db.SaveChanges();
                return RedirectToAction("AssembleAdapterCardF", new { assemble.Id });
            }
            return View(assemble);
        }

        public ActionResult AssembleAdapterCardF(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assemble assemble = db.Assemble.Find(id);
            if (assemble == null)
            {
                return HttpNotFound();
            }
            return View(assemble);
        }


        [HttpPost]
        public ActionResult AssembleAdapterCardF([Bind(Include = "Id,BoxBarCode,AssembleAdapterCardBT,AssembleAdapterCardPrincipal,AssembleAdapterCardFT,AssembleAdapterTime,AssembleAdapterFinish")] Assemble assemble)
        {
            assemble = db.Assemble.FirstOrDefault(u => u.Id == assemble.Id);
            if (assemble.AssembleAdapterCardFT == null)
            {
                assemble.AssembleAdapterCardFT = DateTime.Now;
                var BC = assemble.AssembleAdapterCardBT.Value;
                var FC = assemble.AssembleAdapterCardFT.Value;
                var CT = FC - BC;
                assemble.AssembleAdapterTime = CT;
                assemble.AssembleAdapterFinish = true;
                //assemble.AssembleTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
            }
            if (ModelState.IsValid)
            {
                db.Entry(assemble).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AssembleAdapterCardB", "Assembles");
            }
            return View(assemble);
        }
        #endregion

        #region 视检工段
        /// <summary>
        /// 视检工段
        /// </summary>
        // TODO : 增加电源和转接卡条码号存储功能
        public ActionResult ViewCheckB()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewCheckB([Bind(Include = "Id,BoxBarCode,ViewCheckBT,AssembleViewCheckPrincipal")] Assemble assemble)
        {

            if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode) == null)
            {
                ModelState.AddModelError("", "框体条码在组装记录中找不到，请检查条码输入是否正确，或检查转接卡、电源组装工作是否已经完成！");
                return View(assemble);
            }
            assemble = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);

            if (assemble.AssembleAdapterFinish == false)
            {
                ModelState.AddModelError("", "转接卡、电源组装工作尚未完成，不能进行视检工作！");
                return View(assemble);
            }

            if (assemble.ViewCheckBT == null)
            {
                assemble.ViewCheckBT = DateTime.Now;
                assemble.AssembleViewCheckPrincipal = "";
                db.SaveChanges();
                return RedirectToAction("ViewCheckF", new { assemble.Id });
            }
            return View(assemble);
        }

        public ActionResult ViewCheckF(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assemble assemble = db.Assemble.Find(id);
            if (assemble == null)
            {
                return HttpNotFound();
            }
            return View(assemble);
        }

        [HttpPost]
        public ActionResult ViewCheckF([Bind(Include = "Id,BoxBarCode,ViewCheckBT,AssembleViewCheckPrincipal,ViewCheckFT,ViewCheckTime,ViewCheckFinish")] Assemble assemble)
        {
            assemble = db.Assemble.FirstOrDefault(u => u.Id == assemble.Id);
            if (assemble.ViewCheckFT == null)
            {
                assemble.ViewCheckFT = DateTime.Now;
                var BC = assemble.ViewCheckBT.Value;
                var FC = assemble.ViewCheckFT.Value;
                var CT = FC - BC;
                assemble.ViewCheckTime = CT;
                assemble.ViewCheckFinish = true;
                //assemble.AssembleTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
            }
            if (ModelState.IsValid)
            {
                db.Entry(assemble).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewCheckB", "Assembles");
            }
            return View(assemble);
        }
        #endregion

        #region 电检工段
        /// <summary>
        /// 电检工段
        /// </summary>
        // TODO : 增加电源和转接卡条码号存储功能
        public ActionResult ElectricityCheckB()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ElectricityCheckB([Bind(Include = "Id,BoxBarCode,ElectricityCheckBT,AssembleElectricityCheckPrincipal")] Assemble assemble)
        {

            if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode) == null)
            {
                ModelState.AddModelError("", "框体条码在组装记录中找不到，请检查条码输入是否正确，或检查视检工作是否已经完成！");
                return View(assemble);
            }
            assemble = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);

            if (assemble.ViewCheckFinish == false)
            {
                ModelState.AddModelError("", "视检工作尚未完成，不能进行电检工作！");
                return View(assemble);
            }

            if (assemble.ElectricityCheckBT == null)
            {
                assemble.ElectricityCheckBT = DateTime.Now;
                assemble.AssembleElectricityCheckPrincipal = "";
                db.SaveChanges();
                return RedirectToAction("ElectricityCheckF", new { assemble.Id });
            }
            return View(assemble);
        }

        public ActionResult ElectricityCheckF(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assemble assemble = db.Assemble.Find(id);
            if (assemble == null)
            {
                return HttpNotFound();
            }
            return View(assemble);
        }

        [HttpPost]
        public ActionResult ElectricityCheckF([Bind(Include = "Id,BoxBarCode,ElectricityCheckBT,AssembleElectricityCheckPrincipal,ElectricityCheckFT,ElectricityCheckTime,ElectricityCheckAbnormal,ElectricityCheckFinish")] Assemble assemble)
        {
            assemble = db.Assemble.FirstOrDefault(u => u.Id == assemble.Id);
            if (assemble.ElectricityCheckFT == null)
            {
                assemble.ElectricityCheckFT = DateTime.Now;
                var BC = assemble.ElectricityCheckBT.Value;
                var FC = assemble.ElectricityCheckFT.Value;
                var CT = FC - BC;
                assemble.ElectricityCheckTime = CT;
                assemble.ElectricityCheckAbnormal = 0;
                assemble.ElectricityCheckFinish = true;
                //assemble.AssembleTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
            }
            if (ModelState.IsValid)
            {
                db.Entry(assemble).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ElectricityCheckB", "Assembles");
            }
            return View(assemble);
        }
        #endregion

        #region IPQC工段
        /// <summary>
        /// IPQC工段
        /// </summary>
        // TODO : 增加电源和转接卡条码号存储功能
        public ActionResult IPQCCheckB()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "组装PQC"|| ((Users)Session["User"]).Role == "系统管理员")
            {
                return View();
            }
            return RedirectToAction("AssembleIndex");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IPQCCheckB([Bind(Include = "Id,OrderNum,BoxBarCode,IPQCCheckBT,AssembleIPQCPrincipal")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            //在BarCodes条码表中找不到此条码号
            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == assemble.BoxBarCode) == null)   
            {
                ModelState.AddModelError("", "框体条码不存在，请检查条码输入是否正确！");
                return View(assemble);
            }
            //在BarCodes条码表中找到此条码号
            //在Assembles组装记录表中找不到对应BoxBarCode的记录，准备在Assembles组装记录表中新建记录，包括OrderNum、BoxBarCode、IPQCCheckBT、AssembleIPQCPrincipal
            if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode)==null) 
             {
                var assembleRecord = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);
                assemble.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == assemble.BoxBarCode).FirstOrDefault().OrderNum;    
                assemble.IPQCCheckBT = DateTime.Now;
                assemble.AssembleIPQCPrincipal = ((Users)Session["User"]).UserName;
                db.Assemble.Add(assemble);
                db.SaveChanges();
                return RedirectToAction("IPQCCheckF", new { assemble.Id });
            }
            else  //在Assembles组装记录表中找到对应BoxBarCode的记录，准备修改Assembles组装表中记录IPQCCheckBT,AssembleIPQCPrincipal
            {
                assemble = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);
                if (assemble.IPQCCheckBT == null)
                {
                    //assemble = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);
                    assemble.IPQCCheckBT = DateTime.Now;
                    assemble.AssembleIPQCPrincipal = ((Users)Session["User"]).UserName; 
                    db.SaveChanges();
                    return RedirectToAction("IPQCCheckF", new { assemble.Id });
                }else
                {
                    //assemble = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);
                    //取出对应的id号
                    return RedirectToAction("IPQCCheckF", new { assemble.Id });
                }
            }
            //if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode) == null)
            //{
            //    ModelState.AddModelError("", "框体条码在组装记录中找不到，请检查条码输入是否正确，或检查电检工作是否已经完成！");
            //    return View(assemble);
            //}
            //if (assemble.ElectricityCheckFinish == false)
            //{
            //    ModelState.AddModelError("", "电检工作尚未完成，不能进行IPQC检查工作！");
            //    return View(assemble);
            //}
            //return View(assemble);
        }

        public ActionResult IPQCCheckF(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assemble assemble = db.Assemble.Find(id);
            if (assemble == null)
            {
                return HttpNotFound();
            }
           ViewBag.AbnormalList = SetAbnormalList();
           return View(assemble);
        }

        [HttpPost]
        
        //public ActionResult IPQCCheckF([Bind(Include = "Id,OrderNum,BoxBarCode,IPQCCheckBT,IPQCPrincipal,AssembleLineId,IPQCCheckFT,IPQCCheckTime,IPQCCheckAbnormal,IPQCCheckFinish")] Assemble assemble)
        public ActionResult IPQCCheckF([Bind(Include = "Id,OrderNum,BoxBarCode,AssembleBT,AssembleFT,AssembleTime,AssembleFinish,WaterproofTestBT,WaterproofTestFT,WaterproofTestTime,WaterproofAbnormal,WaterproofMaintaince,WaterproofTestFinish,AssembleAdapterCardBT,AssembleAdapterCardFT,AssembleAdapterTime,AssembleAdapterFinish,ViewCheckBT,ViewCheckFT,ViewCheckTime,ViewCheckAbnormal,ViewCheckFinish,ElectricityCheckBT,ElectricityCheckFT,ElectricityCheckTime,ElectricityCheckAbnormal,ElectricityCheckFinish,IPQCCheckBT,AssembleIPQCPrincipal,AssembleLineId,IPQCCheckFT,IPQCCheckTime,IPQCCheckAbnormal,IPQCCheckFinish")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            assemble = db.Assemble.FirstOrDefault(u => u.Id == assemble.Id);
            if (assemble.IPQCCheckFT == null)
            {
                assemble.IPQCCheckFT = DateTime.Now;
                var BC = assemble.IPQCCheckBT.Value;
                var FC = assemble.IPQCCheckFT.Value;
                var CT = FC - BC;
                assemble.IPQCCheckTime = CT;
                //assemble.AssembleLineId = 3;
                assemble.AssembleLineId = Convert.ToInt16(Request["AssembleLineId"]);
                assemble.IPQCCheckAbnormal = Convert.ToInt16(Request["IPQCCheckAbnormal"]);
                //assemble.IPQCCheckAbnormal = 1;
                assemble.IPQCCheckFinish = true;
                //assemble.AssembleTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
            }
            else
            {
                ModelState.AddModelError("", "记录已存在，不能更改！");
                return View(assemble);
            }

            if (ModelState.IsValid)
            {
                db.Entry(assemble).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IPQCCheckB", "Assembles");
            }
            return View(assemble);
        }
        #endregion

        #region    --------------------首页------------------

        public ActionResult AssembleIndex()
        {
            //CalibrationRecordVM.AllCalibrationRecord = null;
            ViewBag.Display = "display:none";//隐藏View基本情况信息
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

            return View();

            //return View(db.Assemble.ToList());
        }

        [HttpPost]
        public ActionResult AssembleIndex(string orderNum, /*string searchString,*/ int PageIndex = 0)
        {
            IQueryable<Assemble> Allassembles = null;
            List<Assemble> AllassemblesList = null;
            if (orderNum == "")
            {
                //调出全部记录      
                Allassembles = from m in db.Assemble
                            select m;
            }
            else
            {
                //筛选出对应orderNum所有记录
                Allassembles = from m in db.Assemble
                            where (m.OrderNum == orderNum)
                            select m;
            }


            //检查orderNum和searchString是否为空
            //if (!String.IsNullOrEmpty(searchString))
            //{   //从调出的记录中筛选含searchString内容的记录
            //    assembles = assembles.Where(s => s.AbnormalDescription.Contains(searchString));
            //}

            //取出对应orderNum对应组装中IPQC时长所有记录
            IQueryable<TimeSpan?> TimeSpanList = from m in db.Assemble
                                                    where (m.OrderNum == orderNum)
                                                    orderby m.IPQCCheckTime
                                                    select m.IPQCCheckTime;

            //计算校正总时长  TotalTimeSpan
            TimeSpan TotalTimeSpan = DateTime.Now - DateTime.Now;
            if (Allassembles.Where(x => x.IPQCCheckAbnormal == 1).Count() != 0)
            {
                foreach (var m in TimeSpanList)
                {
                    if (m != null)
                    {
                        TotalTimeSpan = TotalTimeSpan.Add(m.Value).Duration();
                    }
                }
                ViewBag.TotalTimeSpan = TotalTimeSpan.Hours.ToString() + "小时" + TotalTimeSpan.Minutes.ToString() + "分" + TotalTimeSpan.Seconds.ToString() + "秒";
            }
            else
            {
                ViewBag.TotalTimeSpan = "暂时没有已完成校正的模组";
            }

            //计算平均用时  AvgTimeSpan
            TimeSpan AvgTimeSpan = DateTime.Now - DateTime.Now;
            int Order_CR_valid_Count = Allassembles.Where(x => x.IPQCCheckTime != null).Count();
            int TotalTimeSpanSecond = Convert.ToInt32(TotalTimeSpan.Hours.ToString()) * 3600 + Convert.ToInt32(TotalTimeSpan.Minutes.ToString()) * 60 + Convert.ToInt32(TotalTimeSpan.Seconds.ToString());
            int AvgTimeSpanInSecond = 0;
            if (Order_CR_valid_Count != 0)
            {
                AvgTimeSpanInSecond = TotalTimeSpanSecond / Order_CR_valid_Count;
                int AvgTimeSpanMinute = AvgTimeSpanInSecond / 60;
                int AvgTimeSpanSecond = AvgTimeSpanInSecond % 60;
                ViewBag.AvgTimeSpan = AvgTimeSpanMinute + "分" + AvgTimeSpanSecond + "秒";//向View传递计算平均用时
            }
            else
            {
                ViewBag.AvgTimeSpan = "暂时没有已完成校正的模组";//向View传递计算平均用时
            }

            //列出记录
            AllassemblesList = Allassembles.ToList();

            //foreach( var item in AllassemblesList)
            //{
            //    List<ModelCollections> modelCollectionsList = db.ModelCollections.Where(m => m.BoxBarCode == item.BoxBarCode).ToList();
            //    item.ModelCollections =JsonConvert.SerializeObject(modelCollectionsList);
            //}

            //统计校正结果正常的模组数量
            var assemble_Normal_Count = Allassembles.Where(m=>m.IPQCCheckAbnormal ==1).Count();
            var assemble_Abnormal_Count = Allassembles.Where(m => m.IPQCCheckAbnormal != 1).Count();
            //读出订单中模组总数量
            var assemble_Quantity = (from m in db.OrderMgm
                                     where (m.OrderNum == orderNum)
                                     select m.Boxes).FirstOrDefault();
            //将模组总数量、正常的模组数量、未完成校正模组数量、订单号信息传递到View页面
            ViewBag.Quantity = assemble_Quantity;
            ViewBag.NormalCount = assemble_Normal_Count;
            ViewBag.AbnormalCount = assemble_Abnormal_Count;
            ViewBag.RecordCount = Allassembles.Count();
            ViewBag.NeverFinish = assemble_Quantity - assemble_Normal_Count;
            ViewBag.orderNum = orderNum;

            //未选择订单时隐藏基本信息设置
            if (ViewBag.Quantity == 0)
            { ViewBag.Display = "display:none"; }
            else { ViewBag.Display = "display:normal"; }

            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

            //分页计算功能
            var recordCount = Allassembles.Count();
            var pageCount = GetPageCount(recordCount);
            if (PageIndex >= pageCount && pageCount >= 1)
            {
                PageIndex = pageCount - 1;
            }

            AllassemblesList = AllassemblesList//.OrderByDescending(m => m.BeginCalibration)
                                 .Skip(PageIndex * PAGE_SIZE)
                                 .Take(PAGE_SIZE).ToList();
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageCount = pageCount;

            return View(AllassemblesList);


        }

        #endregion

        #region    -------------其他方法----------

        // GET: Assembles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assemble assemble = db.Assemble.Find(id);
            if (assemble == null)
            {
                return HttpNotFound();
            }
            return View(assemble);
        }

        // GET: Assembles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assemble assemble = db.Assemble.Find(id);
            if (assemble == null)
            {
                return HttpNotFound();
            }
            return View(assemble);
        }

        // POST: Assembles/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OrderNum,BoxBarCode,AssembleBT,AssembleFT,AssembleTime,AssembleFinish,WaterproofTestBT,WaterproofTestFT,WaterproofTestTime,WaterproofAbnormal,WaterproofMaintaince,WaterproofTestFinish,AssembleAdapterCardBT,AssembleAdapterCardFT,AssembleAdapterTime,AssembleAdapterFinish,ViewCheckBT,ViewCheckFT,ViewCheckTime,ViewCheckAbnormal,ViewCheckFinish,ElectricityCheckBT,ElectricityCheckFT,ElectricityCheckTime,ElectricityCheckAbnormal,ElectricityCheckFinish,IPQCCheckBT,AssembleIPQCPrincipal,AssembleLineId,IPQCCheckFT,IPQCCheckTime,IPQCCheckAbnormal,IPQCCheckFinish")] Assemble assemble)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assemble).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AssembleIndex");
            }
            return View(assemble);
        }

        // GET: Assembles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assemble assemble = db.Assemble.Find(id);
            if (assemble == null)
            {
                return HttpNotFound();
            }
            return View(assemble);
        }

        // POST: Assembles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Assemble assemble = db.Assemble.Find(id);
            db.Assemble.Remove(assemble);
            db.SaveChanges();
            return RedirectToAction("AssembleIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region ------------------ 取出整个OrderMgms的OrderNum订单号列表.--------------------------------------------------
        private List<SelectListItem> GetOrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.OrderCreateDate).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
            var items = new List<SelectListItem>();
            foreach (string order in orders)
            {
                items.Add(new SelectListItem
                {
                    Text = order,
                    Value = order
                });
            }
            return items;
        }
        //----------------------------------------------------------------------------------------
        #endregion

        #region ------------------ 分页函数 ----------------------------
        static List<Assemble> GetPageListByIndex(List<Assemble> list, int pageIndex)
        {
            int pageSize = 10;
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }


        //分页方法
        private static readonly int PAGE_SIZE = 5;

        private int GetPageCount(int recordCount)
        {
            int pageCount = recordCount / PAGE_SIZE;
            if (recordCount % PAGE_SIZE != 0)
            {
                pageCount += 1;
            }
            return pageCount;
        }

        private List<Assemble> GetPagedDataSource(List<Assemble> assemble, int pageIndex, int recordCount)
        {
            var pageCount = GetPageCount(recordCount);
            if (pageIndex >= pageCount && pageCount >= 1)
            {
                pageIndex = pageCount - 1;
            }
            assemble = assemble.OrderByDescending(m => m.IPQCCheckBT)
                               .Skip(pageIndex * PAGE_SIZE)
                               .Take(PAGE_SIZE).ToList();
            return assemble;
         }
        #endregion




    }
}
