using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

namespace JianHeMES.Controllers
{
    public class AssemblesController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        public Assemble assemble = null;


        #region  -----//PQC异常列表-----------

        //private List<SelectListItem> SetAbnormalList()
        //{
        //    return new List<SelectListItem>()
        //    {
        //        new SelectListItem{   Text = "请选择维修情况", Value = ""  },
        //        new SelectListItem{   Text = "正常", Value = "1"  },
        //        new SelectListItem{   Text = "箱体表面", Value = "2"  },
        //        new SelectListItem{   Text = "丝印LOGO", Value = "3"  },
        //        new SelectListItem{   Text = "箱门/门锁", Value = "4"  },
        //        new SelectListItem{   Text = "面罩", Value = "5"  },
        //        new SelectListItem{   Text = "灯管", Value = "6"  },
        //        new SelectListItem{   Text = "指示灯/按钮/显示模块", Value = "7"  },
        //        new SelectListItem{   Text = "锁扣", Value = "8"  },
        //        new SelectListItem{   Text = "把手/提手", Value = "9"  },
        //        new SelectListItem{   Text = "航插座/防水索头", Value = "10"  },
        //        new SelectListItem{   Text = "模块", Value = "11"  },
        //        new SelectListItem{   Text = "红胶/三防漆", Value = "12"  },
        //        new SelectListItem{   Text = "线材/接扎线方式", Value = "13"  },
        //        new SelectListItem{   Text = "风扇/温控断路器/滤波器", Value = "14"  },
        //        new SelectListItem{   Text = "系统卡/功能卡", Value = "15"  },
        //        new SelectListItem{   Text = "电源/保护盖接线座/防尘网", Value = "16"  },
        //        new SelectListItem{   Text = "接地", Value = "17"  },
        //        new SelectListItem{   Text = "胶条", Value = "18"  },
        //        new SelectListItem{   Text = "箱内外观", Value = "19"  },
        //        new SelectListItem{   Text = "标识", Value = "20"  },
        //        new SelectListItem{   Text = "螺丝/压铆", Value = "21"  },
        //        new SelectListItem{   Text = "刮痕/掉漆喷漆/沙眼", Value = "22"  },
        //        new SelectListItem{   Text = "间隙/错位/高低不平", Value = "23"  },
        //        new SelectListItem{   Text = "安装孔", Value = "24"  },
        //        new SelectListItem{   Text = "插销/弹柱/定位柱", Value = "25"  },
        //        new SelectListItem{   Text = "各组件间无干涉", Value = "26"  },
        //        new SelectListItem{   Text = "模组对角平整缝隙", Value = "27"  },
        //        new SelectListItem{   Text = "拼装测试", Value = "28"  },
        //        new SelectListItem{   Text = "单色和全彩测试", Value = "29"  },
        //        new SelectListItem{   Text = "扫描检验", Value = "30"  },
        //        new SelectListItem{   Text = "单灯显示效果", Value = "31"  },
        //        new SelectListItem{   Text = "亮暗线", Value = "32"  },
        //        new SelectListItem{   Text = "指示灯/自检/LCD模块", Value = "33"  },
        //        new SelectListItem{   Text = "黑屏检验", Value = "34"  },
        //        new SelectListItem{   Text = "产品能效功率测试", Value = "35"  },
        //        new SelectListItem{   Text = "防尘", Value = "36"  },
        //        new SelectListItem{   Text = "防水", Value = "37"  }
        //    };
        //}




        #endregion


        #region  -----维修列表-----------

        private List<SelectListItem> SetRepairList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "请选择维修情况",
                    Value = ""
                },
                new SelectListItem
                {
                    Text = "正常",
                    Value = "正常"
                },
                new SelectListItem
                {
                    Text = "现场维修",
                    Value = "现场维修"
                },
                new SelectListItem
                {
                    Text = "转维修站",
                    Value = "转维修站"
                }
            };
        }

        #endregion

        // GET: Assembles

        #region -------------组装模块工段-----------------

        // GET: Assembles/Create
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }

        // POST: Assembles/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OrderNum,BarCode_Prefix,BoxBarCode,AssembleBT,AssembleFT,AssembleTime,AssembleFinish,WaterproofTestBT,WaterproofTestFT,WaterproofTestTime,WaterproofAbnormal,WaterproofMaintaince,WaterproofTestFinish,AssembleAdapterCardBT,AssembleAdapterCardFT,AssembleAdapterTime,AssembleAdapterFinish,ViewCheckBT,ViewCheckFT,ViewCheckTime,ViewCheckAbnormal,ViewCheckFinish,ElectricityCheckBT,ElectricityCheckFT,ElectricityCheckTime,ElectricityCheckAbnormal,ElectricityCheckFinish,PQCCheckBT,PQCCheckFT,PQCCheckTime,PQCCheckAbnormal,PQCCheckFinish")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssembleStationB([Bind(Include = "Id,OrderNum,BarCode_Prefix,BoxBarCode,AssembleBT,AssemblePrincipal,AssembleFT")]Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == assemble.BoxBarCode) == null)
            //if (db.BarCodes.Where(u => u.BarCodesNum == model.BoxBarCode) != null)
            {
                ModelState.AddModelError("", "模组条码不存在，请检查条码输入是否正确！");
                return View(assemble);
            }
            //if(db.Assemble.Count(u=>u.BoxBarCode==assemble.BoxBarCode)>0)
            //{

            //}

            if (assemble.AssembleBT == null)
            {
                assemble.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == assemble.BoxBarCode).FirstOrDefault().OrderNum;
                assemble.AssembleBT = DateTime.Now;
                assemble.AssemblePrincipal = ((Users)Session["User"]).UserName;
                db.Assemble.Add(assemble);
                db.SaveChanges();
                return RedirectToAction("AssembleStationF", new { assemble.Id });
            }
            else
            {
                //assemble = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);
                //取出对应的id号
                return RedirectToAction("AssembleStationF", new { assemble.Id });
            }

            //assemble.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == assemble.BoxBarCode).FirstOrDefault().OrderNum;
            //assemble.AssembleBT = DateTime.Now;
            //assemble.AssemblePrincipal = ((Users)Session["User"]).UserName;
            //db.Assemble.Add(assemble);
            ////db.Configuration.ValidateOnSaveEnabled = false;
            //db.SaveChanges();
            ////db.Configuration.ValidateOnSaveEnabled = true;
            //return RedirectToAction("AssembleStationF", new { assemble.Id});
        }

        // GET: Assembles/Edit/5
        public ActionResult AssembleStationF(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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
        public ActionResult AssembleStationF([Bind(Include = "Id,OrderNum,BarCode_Prefix,BoxBarCode,ModelCollections,AssembleBT,AssemblePrincipal,AssembleFT,AssembleTime,AssembleFinish")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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

        #endregion

        #region -------------防水测试工段------------

        public ActionResult WaterproofTestB()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WaterproofTestB([Bind(Include = "Id,BoxBarCode,WaterproofTestBT,WaterproofTestPrincipal")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }


            if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode) == null)
            {
                ModelState.AddModelError("", "模组条码在组装记录中找不到，请检查条码输入是否正确，或检查组装工作是否已经完成！");
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
                assemble.WaterproofTestPrincipal = ((Users)Session["User"]).UserName;
                db.SaveChanges();
                return RedirectToAction("WaterproofTestF", new { assemble.Id });
            }
            else
            {
                //取出对应的id号
                return RedirectToAction("WaterproofTestF", new { assemble.Id });
            }

            //if (ModelState.IsValid)
            //{
            //    db.Entry(assemble).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("WaterproofTestF", new { assemble.Id });
            //}
            //return View(assemble);
            //return View(assemble);
        }

        public ActionResult WaterproofTestF(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }


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
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }


            assemble = db.Assemble.FirstOrDefault(u => u.Id == assemble.Id);
            if (assemble.WaterproofTestFT == null)
            {
                assemble.WaterproofTestFT = DateTime.Now;
                var BC = assemble.WaterproofTestBT.Value;
                var FC = assemble.WaterproofTestFT.Value;
                var CT = FC - BC;
                assemble.WaterproofTestTime = CT;
                //assemble.WaterproofAbnormal = 1;
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

        #region ----------组装电源、转接卡工段-----------
        /// <summary>
        /// 电源、转接卡工段
        /// </summary>
        // TODO : 增加电源和转接卡条码号存储功能
        public ActionResult AssembleAdapterCardB()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssembleAdapterCardB([Bind(Include = "Id,BoxBarCode,AssembleAdapterCardBT,AssembleAdapterCardPrincipal")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }


            if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode) == null)
            {
                ModelState.AddModelError("", "模组条码在组装记录中找不到，请检查条码输入是否正确，或检查防水测试工作是否已经完成！");
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
                assemble.AssembleAdapterCardPrincipal = ((Users)Session["User"]).UserName;
                db.SaveChanges();
                return RedirectToAction("AssembleAdapterCardF", new { assemble.Id });
            }
            else
            {
                //assemble = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);
                //取出对应的id号
                return RedirectToAction("AssembleAdapterCardF", new { assemble.Id });
            }
            //return View(assemble);
        }

        public ActionResult AssembleAdapterCardF(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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
        public ActionResult AssembleAdapterCardF([Bind(Include = "Id,OrderNum,BoxBarCode,ModelCollections,AssembleBT,ModelList,AssemblePrincipal,AssembleFT,AssembleTime,AssembleFinish,WaterproofTestBT,WaterproofTestPrincipal,WaterproofTestFT,WaterproofTestTime,WaterproofAbnormal,WaterproofMaintaince,WaterproofTestFinish,AssembleAdapterCardBT,AdapterCard_Power_Collection,AdapterCard_Power_List,AssembleAdapterCardPrincipal,AssembleAdapterCardFT,AssembleAdapterTime,AssembleAdapterFinish,ViewCheckBT,AssembleViewCheckPrincipal,ViewCheckFT,ViewCheckTime,ViewCheckAbnormal,ViewCheckFinish,ElectricityCheckBT,AssembleElectricityCheckPrincipal,ElectricityCheckFT,ElectricityCheckTime,ElectricityCheckAbnormal,ElectricityCheckFinish,PQCCheckBT,AssemblePQCPrincipal,AssembleLineId,PQCCheckFT,PQCCheckTime,PQCCheckAbnormal,PQCCheckFinish")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            List<AdapterCard_Power_Collection> adapter_Power_Collection = new List<AdapterCard_Power_Collection>();
            if (assemble.AssembleAdapterCardFT == null)
            {
                assemble.AssembleAdapterCardFT = DateTime.Now;
                var BC = assemble.AssembleAdapterCardBT.Value;
                var FC = assemble.AssembleAdapterCardFT.Value;
                var CT = FC - BC;
                assemble.AssembleAdapterTime = CT;
                assemble.AssembleAdapterFinish = true;
                var aa = assemble.AdapterCard_Power_Collection;
                adapter_Power_Collection = JsonConvert.DeserializeObject<List<AdapterCard_Power_Collection>>(assemble.AdapterCard_Power_Collection.First());
                //assemble.AssembleTimeSpan = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
            }
            if (ModelState.IsValid)
            {
                db.Entry(assemble).State = EntityState.Modified;
                db.SaveChanges();
                foreach (var item in adapter_Power_Collection)
                {
                    db.AdapterCard_Power_Collection.Add(item);
                    db.SaveChanges();
                }
                return RedirectToAction("AssembleAdapterCardB", "Assembles");
            }
            return View(assemble);
        }
        #endregion

        #region ----------------视检工段-----------------
        /// <summary>
        /// 视检工段
        /// </summary>
        // TODO : 增加电源和转接卡条码号存储功能
        public ActionResult ViewCheckB()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewCheckB([Bind(Include = "Id,BoxBarCode,ViewCheckBT,AssembleViewCheckPrincipal")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode) == null)
            {
                ModelState.AddModelError("", "模组条码在组装记录中找不到，请检查条码输入是否正确，或检查转接卡、电源组装工作是否已经完成！");
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
                assemble.AssembleViewCheckPrincipal = ((Users)Session["User"]).UserName;
                db.SaveChanges();
                return RedirectToAction("ViewCheckF", new { assemble.Id });
            }
            else
            {
                //取出对应的id号
                return RedirectToAction("ViewCheckF", new { assemble.Id });
            }

            //return View(assemble);
        }

        public ActionResult ViewCheckF(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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
        public ActionResult ViewCheckF([Bind(Include = "Id,BoxBarCode,ViewCheckBT,AssembleViewCheckPrincipal,ViewCheckFT,ViewCheckTime,ViewCheckAbnormal,ViewCheckFinish")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
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

        #region ----------------电检工段----------------
        /// <summary>
        /// 电检工段
        /// </summary>
        // TODO : 增加电源和转接卡条码号存储功能
        public ActionResult ElectricityCheckB()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ElectricityCheckB([Bind(Include = "Id,BoxBarCode,ElectricityCheckBT,AssembleElectricityCheckPrincipal")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode) == null)
            {
                ModelState.AddModelError("", "模组条码在组装记录中找不到，请检查条码输入是否正确，或检查视检工作是否已经完成！");
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
                assemble.AssembleElectricityCheckPrincipal = ((Users)Session["User"]).UserName;
                db.SaveChanges();
                return RedirectToAction("ElectricityCheckF", new { assemble.Id });
            }
            else
            {
                //assemble = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);
                //取出对应的id号
                return RedirectToAction("ElectricityCheckF", new { assemble.Id });
            }

            //return View(assemble);
        }

        public ActionResult ElectricityCheckF(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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

        #region --------------------PQC工段-------------------
        /// <summary>
        /// PQC工段
        /// </summary>
        // TODO : 增加电源和转接卡条码号存储功能
        public ActionResult PQCCheckB()
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
        public ActionResult PQCCheckB([Bind(Include = "Id,OrderNum,BarCode_Prefix,BoxBarCode,PQCCheckBT,AssemblePQCPrincipal")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            //在BarCodes条码表中找不到此条码号
            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == assemble.BoxBarCode) == null)   
            {
                ModelState.AddModelError("", "模组条码不存在，请检查条码输入是否正确！");
                return View(assemble);
            }
            //在BarCodes条码表中找到此条码号
            //在Assembles组装记录表中找不到对应BoxBarCode的记录，准备在Assembles组装记录表中新建记录，包括OrderNum、BoxBarCode、PQCCheckBT、AssemblePQCPrincipal
            if (db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode) == null)
            {
                //var assembleRecord = db.Assemble.FirstOrDefault(u => u.BoxBarCode == assemble.BoxBarCode);
                assemble.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == assemble.BoxBarCode).FirstOrDefault().OrderNum;
                assemble.BarCode_Prefix = db.BarCodes.Where(u=>u.BarCodesNum == assemble.BoxBarCode).FirstOrDefault().BarCode_Prefix;
                assemble.PQCCheckBT = DateTime.Now;
                assemble.AssemblePQCPrincipal = ((Users)Session["User"]).UserName;
                db.Assemble.Add(assemble);
                db.SaveChanges();
                return RedirectToAction("PQCCheckF", new { assemble.Id });
            }
            //在Assembles组装记录表中找到对应BoxBarCode的记录，如果记录中没有正常的，准备在Assembles组装记录表中新建记录，如果有正常记录将提示不能重复进行QC
            else if (db.Assemble.Count(u => u.BoxBarCode == assemble.BoxBarCode) >= 1)
            {
                var assemblelist = db.Assemble.Where(m => m.BoxBarCode == assemble.BoxBarCode).ToList();
                int normalCount = assemblelist.Where(m => m.PQCCheckAbnormal == "正常").Count();
                if (normalCount==0)
                {
                    assemble.OrderNum = db.BarCodes.Where(u => u.BarCodesNum == assemble.BoxBarCode).FirstOrDefault().OrderNum;
                    assemble.PQCCheckBT = DateTime.Now;
                    assemble.AssemblePQCPrincipal = ((Users)Session["User"]).UserName;
                    db.Assemble.Add(assemble);
                    db.SaveChanges();
                    return RedirectToAction("PQCCheckF", new { assemble.Id });
                }
                else
                {
                    return Content("<script>alert('此模组已经完成PQC，不能对已通过PQC的模组进行重复PQC！');window.location.href='../Assembles/AssembleIndex';</script>");
                }
            }
            else
            {
                //return RedirectToAction("PQCCheckF", new { assemble.Id });
                return RedirectToAction("AssembleIndex");
            }
        }

        public ActionResult PQCCheckF(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assemble assemble = db.Assemble.Find(id);
            if (assemble == null)
            {
                return HttpNotFound();
            }
            //ViewBag.AbnormalList = SetAbnormalList();
            ViewBag.RepairList = SetRepairList();
            return View(assemble);
        }

        [HttpPost]
        
        //public ActionResult PQCCheckF([Bind(Include = "Id,OrderNum,BoxBarCode,PQCCheckBT,PQCPrincipal,AssembleLineId,PQCCheckFT,PQCCheckTime,PQCCheckAbnormal,PQCCheckFinish")] Assemble assemble)
        public ActionResult PQCCheckF([Bind(Include = "Id,OrderNum,BarCode_Prefix,BoxBarCode,BarCode_Prefix,AssembleBT,AssembleFT,AssembleTime,AssembleFinish,WaterproofTestBT,WaterproofTestFT,WaterproofTestTime,WaterproofAbnormal,WaterproofMaintaince,WaterproofTestFinish,AssembleAdapterCardBT,AssembleAdapterCardFT,AssembleAdapterTime,AssembleAdapterFinish,ViewCheckBT,ViewCheckFT,ViewCheckTime,ViewCheckAbnormal,ViewCheckFinish,ElectricityCheckBT,ElectricityCheckFT,ElectricityCheckTime,ElectricityCheckAbnormal,ElectricityCheckFinish,PQCCheckBT,AssemblePQCPrincipal,AssembleLineId,PQCCheckFT,PQCCheckTime,PQCCheckAbnormal,PQCRepairCondition,PQCCheckFinish")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            //assemble = db.Assemble.FirstOrDefault(u => u.Id == assemble.Id);
            if (assemble.PQCCheckFT == null)
            {
                assemble.PQCCheckFT = DateTime.Now;
                var BC = assemble.PQCCheckBT.Value;
                var FC = assemble.PQCCheckFT.Value;
                var CT = FC - BC;
                assemble.PQCCheckTime = CT;
                assemble.AssembleLineId = Convert.ToInt16(Request["AssembleLineId"]);
                //assemble.PQCCheckAbnormal = Convert.ToInt16(Request["PQCCheckAbnormal"]);
                //assemble.PQCCheckAbnormal = Request["PQCCheckAbnormal"];
                assemble.PQCCheckFinish = true;
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
                return RedirectToAction("PQCCheckB", "Assembles");
            }
            return View(assemble);
        }
        #endregion

        #region    --------------------首页------------------

        public ActionResult AssembleIndex()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            //CalibrationRecordVM.AllCalibrationRecord = null;
            ViewBag.Display = "display:none";//隐藏View基本情况信息
            ViewBag.Legend = "display:none";//隐藏图例
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.

            return View();

            //return View(db.Assemble.ToList());
        }

        [HttpPost]
        public ActionResult AssembleIndex(string orderNum, /*string searchString,*/ int PageIndex = 0)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            IQueryable<Assemble> Allassembles = null;
            List<Assemble> AllassemblesList = null;
            if (orderNum == "")
            {
                ////调出全部记录      
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

            //取出对应orderNum对应组装中PQC时长所有记录
            IQueryable<TimeSpan?> TimeSpanList = from m in db.Assemble
                                                    where (m.OrderNum == orderNum)
                                                    orderby m.PQCCheckTime
                                                    select m.PQCCheckTime;

            //计算校正总时长  TotalTimeSpan
            TimeSpan TotalTimeSpan = DateTime.Now - DateTime.Now;
            if (Allassembles.Where(x => x.PQCCheckAbnormal == "正常").Count() != 0)
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
            int Order_CR_valid_Count = Allassembles.Where(x => x.PQCCheckTime != null).Count();
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

            //取出记录对应的模块清单
            List<string> list = new List<string>();
            foreach (Assemble item in AllassemblesList)
            {
                List<ModelCollections> modelCollectionsList = db.ModelCollections.Where(m => m.BoxBarCode == item.BoxBarCode).ToList();
                if (modelCollectionsList != null)
                {
                    //item.ModelList = JsonConvert.SerializeObject(modelCollectionsList);
                    foreach (var it in modelCollectionsList)
                    {
                        item.ModelList = item.ModelList + "位置ID:"+it.StationId + "," + "\r\n"+"模块条码号:" + it.BarCodesNum+ "；"+ "\r\n";
                    }
                }
                List<AdapterCard_Power_Collection> AdapterCard_Power_List = db.AdapterCard_Power_Collection.Where(m => m.BoxBarCode == item.BoxBarCode).ToList();
                if (AdapterCard_Power_List != null)
                { 
                    foreach (var it in AdapterCard_Power_List)
                    {
                        item.AdapterCard_Power_List =item.AdapterCard_Power_List + it.BarCodesNum+",";
                    }
                }
                //item.ModelCollections.AddRange(list.ToList());
                //string modelCollectionsString = JsonConvert.SerializeObject(modelCollectionsList);
                //var itemId = item.Id;
                //var modifya = AllassemblesList.Where(m => m.Id == itemId).ToList().First();
                //modifya.ModelCollections.Add(modelCollectionsString);
            }

            //统计校正结果正常的模组数量
            var assemble_Normal_Count = Allassembles.Where(m=>m.PQCCheckAbnormal =="正常").Count();
            var assemble_Abnormal_Count = Allassembles.Where(m => m.PQCCheckAbnormal != "正常").Count();
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
            ViewBag.Legend = "display:normal";//显示图例

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

        #region  ---------------检查条码是否存在------------------
        [HttpPost]
        public Boolean CheckBarCode(string barcode)
        {
            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == barcode)==null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region    -------------其他方法----------

        // GET: Assembles/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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
        public ActionResult Edit([Bind(Include = "Id,OrderNum,BoxBarCode,AssembleBT,AssembleFT,AssembleTime,AssembleFinish,WaterproofTestBT,WaterproofTestFT,WaterproofTestTime,WaterproofAbnormal,WaterproofMaintaince,WaterproofTestFinish,AssembleAdapterCardBT,AssembleAdapterCardFT,AssembleAdapterTime,AssembleAdapterFinish,ViewCheckBT,ViewCheckFT,ViewCheckTime,ViewCheckAbnormal,ViewCheckFinish,ElectricityCheckBT,ElectricityCheckFT,ElectricityCheckTime,ElectricityCheckAbnormal,ElectricityCheckFinish,PQCCheckBT,AssemblePQCPrincipal,AssembleLineId,PQCCheckFT,PQCCheckTime,PQCCheckAbnormal,PQCCheckFinish")] Assemble assemble)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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
            assemble = assemble.OrderByDescending(m => m.PQCCheckBT)
                               .Skip(pageIndex * PAGE_SIZE)
                               .Take(PAGE_SIZE).ToList();
            return assemble;
         }
        #endregion

    }
}
