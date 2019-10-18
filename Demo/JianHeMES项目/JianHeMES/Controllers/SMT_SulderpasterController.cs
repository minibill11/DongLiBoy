﻿using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Data.Entity;

namespace JianHeMES.Controllers
{
    public class SMT_SulderpasterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController com = new CommonalityController();
        // GET: SMT_Sulderpaster
        //锡膏入库
        public class tempWarehous
        {
            public string barcode { get; set; }
            public DateTime? time { get; set; }

            public int day { get; set; }

        }
        #region 页面
        public ActionResult AddWarehouseBaseInfo()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "AddWarehouseBaseInfo" });
            }
            return View();
        }

        //打印条码
        public ActionResult printBarcode()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "printBarcode" });
            }
            return View();
        }
        //锡膏入冰柜
        public ActionResult AddWarehouseFreezer()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "AddWarehouseFreezer" });
            }
            return View();
        }
        //出库
        public ActionResult outWarehouseFreezer()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "outWarehouseFreezer" });
            }
            return View();
        }
        //入SMT冰柜
        public ActionResult AddSMTFreezer()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "AddSMTFreezer" });
            }
            return View();
        }
        //回温
        public ActionResult Rewarming()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "Rewarming" });
            }
            return View();
        }

        //回温记录
        public ActionResult RewarmRecord()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "RewarmRecord" });
            }
            return View();
        }
        //搅拌
        public ActionResult Stir()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "Stir" });
            }
            return View();
        }
        //使用
        public ActionResult Use()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "Use" });
            }
            return View();
        }
        //回收
        public ActionResult recovery()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "recovery" });
            }
            return View();
        }
        //看板
        public ActionResult smtBoard()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "smtBoard" });
            }
            return View();
        }
        public ActionResult mcBoard()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "mcBoard" });
            }
            return View();
        }
        public ActionResult Addwarehouse_Material()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "Addwarehouse_Material" });
            }
            return View();
        }
        #endregion

        #region 物料表
        //录入物料信息
        [HttpPost]
        public async Task<ActionResult> Addwarehouse_MaterialAsync(List<Warehouse_Material> warehouse_Material)
        {
            int count = 0;
            JObject message = new JObject();
            if (ModelState.IsValid)
            {
                foreach (var item in warehouse_Material)
                {
                    var isExit = db.Warehouse_Material.Count(c => c.MaterialNum == item.MaterialNum);
                    if (isExit > 0)
                    {
                        message.Add("result", false);
                        message.Add("message", "已有重复的物料号");
                        return Content(JsonConvert.SerializeObject(message));
                    }
                    db.Warehouse_Material.Add(item);
                    count += await db.SaveChangesAsync();
                }
                if (count == warehouse_Material.Count())
                {
                    message.Add("result", true);
                    message.Add("message", "录入成功");
                    return Content(JsonConvert.SerializeObject(message));
                }
            }
            message.Add("result", false);
            message.Add("message", "输入的数据格式不对");
            return Content(JsonConvert.SerializeObject(message));
        }


        #endregion
        [HttpPost]
        #region MC看板
        public ActionResult MCBoard(string barcode, string receivingnum, string batch)
        {
            var barcodeList = db.Barcode_Solderpaste.ToList();
            if (!string.IsNullOrEmpty(receivingnum))
            {
                barcodeList = barcodeList.Where(c => c.ReceivingNum == receivingnum).ToList();
            }
            if (!string.IsNullOrEmpty(batch))
            {
                barcodeList = barcodeList.Where(c => c.Batch == batch).ToList();
            }
            if (!string.IsNullOrEmpty(barcode))
            {
                barcodeList = barcodeList.Where(c => c.SolderpasterBacrcode == barcode).ToList();
            }
            JArray result = new JArray();
            foreach (var item in barcodeList)
            {
                JObject jobjectitem = new JObject();
                jobjectitem.Add("barcode", item.SolderpasterBacrcode);
                //物料号
                jobjectitem.Add("ReceivingNum", item.ReceivingNum);
                //物料号
                jobjectitem.Add("Batch", item.Batch);
                //供应商
                jobjectitem.Add("Supplier", item.Supplier);
                //生产时间
                jobjectitem.Add("LeaveFactoryTime", item.LeaveFactoryTime);
                //型号
                jobjectitem.Add("SolderpasteType", item.SolderpasteType);
                var warehouse = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == item.SolderpasterBacrcode).FirstOrDefault();
                if (warehouse == null)
                    jobjectitem.Add("statue", "未入冰柜");
                else
                {
                    if (warehouse.WarehouseOutTime != null)
                        jobjectitem.Add("statue", "已出库");
                    else
                        jobjectitem.Add("statue", "已入库");

                }
                result.Add(jobjectitem);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 锡膏出入库 

        //仓库基本信息录入
        [HttpPost]
        public async Task<JArray> AddWarehouseBaseInfo(Warehouse_BaseInfo warehouse_BaseInfo)
        {
            if (((Users)Session["User"]) != null)
            {
                if (ModelState.IsValid)
                {
                    warehouse_BaseInfo.CheckTime = DateTime.Now;
                    warehouse_BaseInfo.Username = ((Users)Session["User"]).UserName;
                    warehouse_BaseInfo.JobNum = ((Users)Session["User"]).UserNum.ToString();
                    db.Warehouse_BaseInfo.Add(warehouse_BaseInfo);

                    if (!string.IsNullOrEmpty(warehouse_BaseInfo.ReceivingNum) || warehouse_BaseInfo.LeaveFactoryTime != null)
                    {
                        string year = warehouse_BaseInfo.LeaveFactoryTime.Value.Year.ToString().Substring(2);
                        JArray barcodeList = new JArray();
                        int count = 0;
                        for (int i = 1; i <= warehouse_BaseInfo.SolderpasteNum; i++)
                        {
                            int num = 1;
                            var lastBarcode = db.Barcode_Solderpaste.Where(c => c.ReceivingNum == warehouse_BaseInfo.ReceivingNum && c.LeaveFactoryTime == warehouse_BaseInfo.LeaveFactoryTime).OrderByDescending(c => c.SolderpasterBacrcode).FirstOrDefault();
                            if (lastBarcode != null)
                            {

                                string lastNUm = lastBarcode.SolderpasterBacrcode.Substring(lastBarcode.SolderpasterBacrcode.Length - 3, 3);
                                num = int.Parse(lastNUm) + 1;
                            }

                            string barcode = warehouse_BaseInfo.ReceivingNum + "-" + year + warehouse_BaseInfo.LeaveFactoryTime.Value.Month.ToString().PadLeft(2, '0') + warehouse_BaseInfo.LeaveFactoryTime.Value.Day.ToString().PadLeft(2, '0') + "-" + num.ToString().PadLeft(3, '0');

                            Barcode_Solderpaste barcode_Solderpaste = new Barcode_Solderpaste() { SolderpasterBacrcode = barcode, Batch = warehouse_BaseInfo.Batch, LeaveFactoryTime = warehouse_BaseInfo.LeaveFactoryTime, ReceivingNum = warehouse_BaseInfo.ReceivingNum, SolderpasteType = warehouse_BaseInfo.SolderpasteType, Supplier = warehouse_BaseInfo.Supplier, PrintTime = DateTime.Now, PrintName = ((Users)Session["User"]).UserName, EffectiveDay = warehouse_BaseInfo.EffectiveDay };
                            db.Barcode_Solderpaste.Add(barcode_Solderpaste);
                            count += await db.SaveChangesAsync();
                            barcodeList.Add(barcode);
                        }
                        if (count == warehouse_BaseInfo.SolderpasteNum + 1)
                            return barcodeList;
                    }
                    return null;


                }
            }
            return null;
        }

        //输入物料号，返回物料信息
        public ActionResult GetMaterialInfo(string Material)
        {
            var info = db.Warehouse_Material.Where(c => c.MaterialNum == Material).FirstOrDefault();
            JObject message = new JObject();
            if (info == null)
            {
                return null;
            }
            message.Add("type", info.Type);
            //厂商简称
            message.Add("ManufactorName", info.ManufactorName);
            //厂商编号
            message.Add("ManufactorNum", info.ManufactorNum);
            return Content(JsonConvert.SerializeObject(message));
        }

        //查看物料信息
        public ActionResult SelctMaterial()
        {
            var meterial = db.Warehouse_Material.ToList();
            JArray result = new JArray();
            result.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(meterial)));
            return Content(JsonConvert.SerializeObject(result));
        }

        //修改物料信息
        public void UpdateMaterial(Warehouse_Material warehouse_Material)
        {
            if (ModelState.IsValid)
            {
                db.Entry(warehouse_Material).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        //入仓库冰柜
        [HttpPost]
        public async Task<bool> AddWarehouseFreezerAsync(List<string> warehouse_FreezerList, string warehouseNum)
        {
            if (((Users)Session["User"]) == null)
            {
                return false;
            }
            int count = 0;
            foreach (var item in warehouse_FreezerList)
            {
                Warehouse_Freezer warehouse_Freezer = new Warehouse_Freezer();
                warehouse_Freezer.SolderpasterBacrcode = item;
                warehouse_Freezer.WarehouseNum = warehouseNum;
                warehouse_Freezer.IntoTime = DateTime.Now;
                warehouse_Freezer.UserName = ((Users)Session["User"]).UserName;
                warehouse_Freezer.JobNum = ((Users)Session["User"]).UserNum.ToString();
                db.Warehouse_Freezer.Add(warehouse_Freezer);
                count += await db.SaveChangesAsync();
            }
            if (count == warehouse_FreezerList.Count())
                return true;
            else
                return false;
            //}
            //return Content("false");
        }

        //出仓库冰柜
        public async Task<bool> UpdateWarehouseFreezerAsync(List<string> warehouse_FreezerList, string name, string jobnum)
        {
            if (((Users)Session["User"]) == null)
            {
                return false;
            }
            int count = 0;
            foreach (var item in warehouse_FreezerList)
            {

                var warehouse = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                warehouse.WarehouseOutTime = DateTime.Now;
                warehouse.WarehouseOutUserName = name;
                warehouse.WarehouseOutJobNum = jobnum;
                db.Entry(warehouse).State = System.Data.Entity.EntityState.Modified;
                count += await db.SaveChangesAsync();
            }
            if (count == warehouse_FreezerList.Count())
                return true;
            else
                return false;
            //}
            //return Content("false");
        }
        [HttpPost]
        //出库列表显示提示先入先出
        public ActionResult CheckWarehouse()
        {
            var warehous = db.Warehouse_Freezer.Where(c => c.WarehouseOutTime == null).Select(c => c.SolderpasterBacrcode).ToList();
            var tempwarehouse = new List<tempWarehous>();
            foreach (var item in warehous)
            {
                var time = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                tempWarehous temp = new tempWarehous() { barcode = item, time = time.LeaveFactoryTime, day = time.EffectiveDay };
                tempwarehouse.Add(temp);
            }
            JArray result = new JArray();
            tempwarehouse.OrderBy(c => c.time).ToList();
            foreach (var warehouseitem in tempwarehouse)
            {
                JObject jObjectitem = new JObject();
                jObjectitem.Add("barcode", warehouseitem.barcode);
                //生产日期
                jObjectitem.Add("time", warehouseitem.time);
                //有效天数
                var spam = (int)(DateTime.Now - warehouseitem.time).Value.TotalDays;
                var effectiveday = warehouseitem.day - spam;
                jObjectitem.Add("overdue", effectiveday);
                result.Add(jObjectitem);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 锡膏再次打印
        public JArray DisplayPrintAgainBarcode(string receivingNum, DateTime leaveFactoryTime)
        {
            var barcodeList = db.Barcode_Solderpaste.Where(c => c.ReceivingNum == receivingNum && c.LeaveFactoryTime == leaveFactoryTime).Select(c => c.SolderpasterBacrcode).ToArray();
            JArray result = new JArray();
            foreach (var item in barcodeList)
            {
                result.Add(item);
            }
            return result;
        }
        #endregion

        #region 入冰柜

        /// <summary>
        /// 条码检验
        /// </summary>
        /// <param name="smt_FreezerList">条码列表</param>
        /// <param name="statu">例：仓库，冰柜，回温，领用</param>
        /// <param name="belogin">冰柜专属 如：仓库，回温，领用</param>
        /// <returns></returns>
        public ActionResult CheckSMTFreezer(List<string> smt_FreezerList, string statu, string belogin = null)
        {
            JArray List = new JArray();
            foreach (var item in smt_FreezerList)
            {
                JObject jObject = new JObject();
                jObject.Add("barcode", item);
                var startime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.PrintTime).FirstOrDefault();
                if (startime == null)
                {
                    jObject.Add("Tips", "没有找到此条码");
                }
                else
                {
                    var spantime = com.TwoDTforMonth_sub(DateTime.Now, startime);
                    var recyle = db.SMT_Recycle.Count(c => c.SolderpasterBacrcode == item);
                    if (statu == "回收")
                    {
                        if (recyle != 0)
                            jObject.Add("Tips", "此条码已回收");
                        else
                            jObject.Add("Tips", "正常");
                    }
                    else if (spantime >= 5 && spantime < 6)
                    {
                        jObject.Add("Tips", "将过期");
                    }
                    else if (spantime >= 6)
                    {
                        jObject.Add("Tips", "已过期");
                    }

                    else if (recyle != 0)
                    {
                        jObject.Add("Tips", "此条码已回收");
                    }
                    else
                    {
                        if (statu == "冰柜")
                        {
                            var freezecount = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item && c.Belogin == belogin).OrderByDescending(c => c.IntoTime).Select(c => c.IntoTime).FirstOrDefault();
                            var emply = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                            if (belogin == "仓库")
                            {
                                if (freezecount != null)
                                { jObject.Add("Tips", "此条码已存在SMT冰柜中"); }
                                else
                                    jObject.Add("Tips", "正常");
                            }
                            else if (belogin == "回温")
                            {
                                var warmcount = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                                if (warmcount <= freezecount)
                                { jObject.Add("Tips", "此条码已存在SMT冰柜中"); }
                                else if (warmcount == null)
                                { jObject.Add("Tips", "此条码未经过回温"); }
                                else if (warmcount < emply)
                                { jObject.Add("Tips", "此条码已经领用过了"); }
                                else
                                    jObject.Add("Tips", "正常");
                            }
                            else if (belogin == "领用")
                            {
                                //var emply = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                                if (emply <= freezecount)
                                { jObject.Add("Tips", "此条码已存在SMT冰柜中"); }
                                else if (emply == null)
                                { jObject.Add("Tips", "此条码未使用过"); }
                                else
                                    jObject.Add("Tips", "正常");
                            }
                        }
                        else if (statu == "回温")
                        {
                            var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.IntoTime).Select(c => c.IntoTime).FirstOrDefault();
                            var warm = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                            if (warm != null && warm >= freeze)
                            { jObject.Add("Tips", "此条码已在回温中"); }
                            else
                                jObject.Add("Tips", "正常");
                        }
                        else if (statu == "领用")
                        {
                            var warm = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                            var stri = db.SMT_Stir.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();

                            var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.IntoTime).Select(c => c.IntoTime).FirstOrDefault();
                            //var emply = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                            if (stri < warm || stri == null)
                            {
                                jObject.Add("Tips", "此条码未经过搅拌");
                            }
                            //else if (emply != null || emply >= freeze)
                            //{ jObject.Add("Tips", "此条码已经在使用中"); }
                            else
                                jObject.Add("Tips", "正常");
                        }
                        else if (statu == "出库")
                        {
                            var warehouse = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                            if (warehouse == null)
                            { jObject.Add("Tips", "此条码未入库"); }
                            else
                            {
                                if (warehouse.WarehouseOutTime != null)
                                    jObject.Add("Tips", "此条码已出库");
                                else
                                    jObject.Add("Tips", "正常");
                            }
                        }
                        else
                        {
                            jObject.Add("Tips", "正常");
                        }
                    }
                }
                List.Add(jObject);
            }
            return Content(JsonConvert.SerializeObject(List));
        }

        //入SMT冰柜右边列表
        public ActionResult CheckSMTFreezeList()
        {
            var rewarm = db.Warehouse_Freezer.OrderBy(c => c.IntoTime).Where(c => c.WarehouseOutTime != null).Select(c => c.SolderpasterBacrcode).ToList();

            List<string> distin = new List<string>();
            //排序
            //var orderbydate=rewarm
            JArray list = new JArray();
            foreach (var item in rewarm)
            {
                //已经入过SMT冰柜
                if (db.SMT_Freezer.Count(c => c.SolderpasterBacrcode == item) > 0)
                { continue; }

                JObject rejobject = new JObject();

                rejobject.Add("barcode", item);
                //var time = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).Max(c => c.StartTime);
                var time = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == item).Max(c => c.IntoTime);
                var imespan = DateTime.Now - time;
                rejobject.Add("freezespan", imespan.Value.TotalSeconds);

                var productiontime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.LeaveFactoryTime).FirstOrDefault();
                if (productiontime == null)
                {
                    continue;
                }
                var span = (DateTime.Now.Date - productiontime).Value.TotalDays;
                var effiday = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.EffectiveDay).FirstOrDefault();
                rejobject.Add("overdue", effiday - span);

                list.Add(rejobject);

                distin.Add(item);
            }
            return Content(JsonConvert.SerializeObject(list));
        }


        //入SMT冰柜
        [HttpPost]
        public async Task<bool> AddSMTFreezerAsync(List<string> smt_FreezerList, string belogin)
        {
            if (((Users)Session["User"]) != null)
            {
                int count = 0;
                foreach (var item in smt_FreezerList)
                {
                    SMT_Freezer smt_Freezer = new SMT_Freezer();
                    smt_Freezer.SolderpasterBacrcode = item;
                    smt_Freezer.IntoTime = DateTime.Now;
                    smt_Freezer.UserName = ((Users)Session["User"]).UserName;
                    smt_Freezer.JobNum = ((Users)Session["User"]).UserNum.ToString();

                    smt_Freezer.Belogin = belogin;

                    db.SMT_Freezer.Add(smt_Freezer);
                    count += await db.SaveChangesAsync();
                }
                if (count == smt_FreezerList.Count())
                    return true;
                else
                    return false;

            }
            return false;
        }

        #endregion

        #region 回温
        //回温输入旁边列表
        public ActionResult CheckSMTRewarm()
        {
            var rewarm = db.SMT_Freezer.OrderBy(c => c.IntoTime).Select(c => c.SolderpasterBacrcode).ToList();

            List<string> distin = new List<string>();
            //排序
            //var orderbydate=rewarm
            JArray list = new JArray();
            foreach (var item in rewarm)
            {
                //去重
                if (distin.Contains(item))
                { continue; }

                JObject rejobject = new JObject();
                //最晚一条回温记录
                var warmcount = db.SMT_Rewarm.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == item).Select(c => c.StartTime).FirstOrDefault();
                //最晚一条搅拌记录
                var freecount = db.SMT_Freezer.OrderByDescending(c => c.IntoTime).Where(c => c.SolderpasterBacrcode == item).Select(c => c.IntoTime).FirstOrDefault(); ;
                if (warmcount == null || (warmcount != null && freecount != null && freecount > warmcount))
                {
                    rejobject.Add("barcode", item);
                    //var time = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).Max(c => c.StartTime);
                    var time = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).Max(c => c.IntoTime);
                    var imespan = DateTime.Now - time;
                    rejobject.Add("freezespan", imespan.Value.TotalSeconds);

                    var productiontime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.LeaveFactoryTime).FirstOrDefault();
                    if (productiontime == null)
                    {
                        continue;
                    }
                    var span = (DateTime.Now.Date - productiontime).Value.TotalDays;
                    var effiday = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.EffectiveDay).FirstOrDefault();
                    rejobject.Add("overdue", effiday - span);

                    list.Add(rejobject);
                }
                distin.Add(item);
            }
            return Content(JsonConvert.SerializeObject(list));
        }


        //开始回温
        [HttpPost]
        public async Task<bool> AddSMTRewarmAsync(List<string> smt_RewarmList)
        {
            if (((Users)Session["User"]) != null)
            {
                int count = 0;
                foreach (var item in smt_RewarmList)
                {
                    SMT_Rewarm smt_Rewarm = new SMT_Rewarm();
                    smt_Rewarm.SolderpasterBacrcode = item;
                    smt_Rewarm.StartTime = DateTime.Now;
                    smt_Rewarm.UserName = ((Users)Session["User"]).UserName;
                    smt_Rewarm.JobNum = ((Users)Session["User"]).UserNum.ToString();
                    db.SMT_Rewarm.Add(smt_Rewarm);
                    count += await db.SaveChangesAsync();
                }
                if (count == smt_RewarmList.Count())
                    return true;

                else
                    return false;
            }
            return false;
        }


        //回温记录
        public ActionResult RewarmInfo()
        {
            var rewarm = db.SMT_Rewarm.OrderBy(c => c.StartTime).Select(c => c.SolderpasterBacrcode).Distinct().ToList();
            List<string> distin = new List<string>();
            //排序
            //var orderbydate=rewarm
            JArray list = new JArray();
            foreach (var item in rewarm)
            {
                //去重
                if (distin.Contains(item))
                { continue; }
                JObject rejobject = new JObject();
                var warmcount = db.SMT_Rewarm.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == item).Select(c => c.StartTime).FirstOrDefault();
                var sitcount = db.SMT_Stir.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == item).Select(c => c.StartTime).FirstOrDefault();
                var free = db.SMT_Freezer.OrderByDescending(c => c.IntoTime).Where(c => c.SolderpasterBacrcode == item).Select(c => c.IntoTime).FirstOrDefault();
                if ((warmcount != null && free != null && warmcount > free) && ((warmcount != null && sitcount == null) || (warmcount != null && sitcount != null && sitcount < warmcount)))
                {
                    rejobject.Add("barcode", item);
                    var time = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).Max(c => c.StartTime);
                    var imespan = DateTime.Now - time;
                    rejobject.Add("retimespan", imespan.Value.TotalSeconds);

                    var productiontime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.LeaveFactoryTime).FirstOrDefault();
                    if (productiontime == null)
                    {
                        continue;
                    }
                    var span = (DateTime.Now.Date - productiontime).Value.TotalDays;
                    var effiday = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.EffectiveDay).FirstOrDefault();
                    rejobject.Add("overdue", effiday - span);

                    list.Add(rejobject);
                }
            }
            return Content(JsonConvert.SerializeObject(list));
        }

        [HttpPost]
        //回温记录搜索
        public ActionResult RewarmInfo(string barocde)
        {
            var barcode = db.SMT_Rewarm.Count(c => c.SolderpasterBacrcode == barocde);
            if (barcode != 0)
            {
                JObject rejobject = new JObject();
                var warmcount = db.SMT_Rewarm.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == barocde).Select(c => c.StartTime).FirstOrDefault();
                var sitcount = db.SMT_Stir.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == barocde).Select(c => c.StartTime).FirstOrDefault(); ;
                if ((warmcount != null && sitcount == null) || (warmcount != null && sitcount != null && sitcount < warmcount))
                {
                    rejobject.Add("barcode", barocde);
                    var time = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == barocde).Max(c => c.StartTime);
                    var imespan = DateTime.Now - time;
                    rejobject.Add("retimespan", imespan.Value.TotalSeconds);

                    var productiontime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == barocde).Select(c => c.LeaveFactoryTime).FirstOrDefault();
                    if (productiontime == null)
                    {
                        return Content("false");
                    }
                    var span = (DateTime.Now.Date - productiontime).Value.TotalDays;
                    var effiday = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == barocde).Select(c => c.EffectiveDay).FirstOrDefault();
                    rejobject.Add("overdue", effiday - span);

                    return Content(JsonConvert.SerializeObject(rejobject));
                }
                ModelState.AddModelError("", "此条码没有正在的回温记录");
            }
            ModelState.AddModelError("", "没有找到此条码");
            return Content("false");
        }
        #endregion

        #region 搅拌
        [HttpPost]
        public ActionResult DisplatStirInfo(string SolderpasterBacrcode)
        {
            JObject infojobject = new JObject();
            var free = db.SMT_Freezer.OrderByDescending(c => c.IntoTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).Select(c => c.IntoTime).FirstOrDefault();
            var warmtime = db.SMT_Rewarm.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).Select(c => c.StartTime).FirstOrDefault();
            var user = db.SMT_Employ.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).Select(c => c.StartTime).FirstOrDefault();
            var info = db.SMT_Stir.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).FirstOrDefault();

            //没有回温记录,并且回温记录小于领用记录
            if (warmtime == null || (warmtime != null && warmtime < free) || (user != null && info != null && warmtime < user))
            {
                infojobject.Add("first", null);
                infojobject.Add("second", null);
                infojobject.Add("three", null);
                infojobject.Add("flow", null);
                infojobject.Add("message", warmtime == null || (warmtime != null && warmtime < free) ? "此条码还未回温" : "此条码已经开始使用");
                infojobject.Add("canadd", false);
            }
            else
            {
                //有回温没有搅拌记录和 有回温有搅拌有领用，但回温记录大于搅拌记录
                if (info == null || (user != null && info != null && warmtime > info.StartTime))
                {
                    infojobject.Add("first", null);
                    infojobject.Add("second", null);
                    infojobject.Add("three", null);
                    infojobject.Add("flow", null);
                    infojobject.Add("message", "此条码还未开始搅拌");
                    infojobject.Add("canadd", true);
                }
                //有回温有搅拌没有领用记录 及  有回温有搅拌有领用，但回温小于搅拌，搅拌大于领用
                else if ((info != null && user == null) || (info != null && user != null & warmtime < info.StartTime && info.StartTime > user))
                {

                    infojobject.Add("first", info.StartTime);
                    infojobject.Add("second", info.SecondTime);
                    infojobject.Add("three", info.ThreeTime);
                    infojobject.Add("flow", info.FlorTime);
                    infojobject.Add("message", null);
                    infojobject.Add("canadd", true);

                }
            }
            return Content(JsonConvert.SerializeObject(infojobject));
        }

        //开始搅拌
        public bool AddSMTStir(string SolderpasterBacrcode, int num)
        {
            if (((Users)Session["User"]) != null)
            {

                var sMT_Stir = db.SMT_Stir.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).FirstOrDefault();
                switch (num)
                {
                    case 1:
                        SMT_Stir stir = new SMT_Stir() { SolderpasterBacrcode = SolderpasterBacrcode, UserName = ((Users)Session["User"]).UserName, JobNum = ((Users)Session["User"]).UserNum.ToString(), StartTime = DateTime.Now };
                        db.SMT_Stir.Add(stir);

                        break;
                    case 2:
                        if (sMT_Stir.SecondTime == null)
                        {
                            sMT_Stir.SecondTime = DateTime.Now;
                            sMT_Stir.SecondName = ((Users)Session["User"]).UserName;
                            sMT_Stir.SecondJobNum = ((Users)Session["User"]).UserNum.ToString();
                        }
                        break;
                    case 3:
                        if (sMT_Stir.ThreeTime == null)
                        {
                            sMT_Stir.ThreeTime = DateTime.Now;
                            sMT_Stir.ThreeJobName = ((Users)Session["User"]).UserName;
                            sMT_Stir.ThreeNum = ((Users)Session["User"]).UserNum.ToString();
                        }
                        break;
                    case 4:
                        if (sMT_Stir.FlorTime == null)
                        {
                            sMT_Stir.FlorTime = DateTime.Now;
                            sMT_Stir.FlorName = ((Users)Session["User"]).UserName;
                            sMT_Stir.FlorJobNum = ((Users)Session["User"]).UserNum.ToString();
                        }
                        break;
                    default:

                        break;
                }

                db.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region 使用
        [HttpPost]
        public async Task<bool> AddEmployAsync(List<string> smt_EmployList, string ordernum, int line)
        {
            if (((Users)Session["User"]) == null)
            {
                return false;
            }
            int count = 0;
            foreach (var item in smt_EmployList)
            {
                SMT_Employ sMT_Employ = new SMT_Employ() { SolderpasterBacrcode = item, StartTime = DateTime.Now, JobNum = ((Users)Session["User"]).UserNum.ToString(), UserName = ((Users)Session["User"]).UserName, OrderNum = ordernum, LineNum = line };
                db.SMT_Employ.Add(sMT_Employ);
                count += await db.SaveChangesAsync();
            }
            if (count == smt_EmployList.Count())
                return true;
            else
                return false;
        }

        #endregion

        #region  回收
        [HttpPost]
        public async Task<bool> RecycleAsync(List<string> smt_RecycleList)
        {
            if (((Users)Session["User"]) == null)
            {
                return false;
            }
            int count = 0;
            foreach (var item in smt_RecycleList)
            {
                SMT_Recycle sMT_Recycle = new SMT_Recycle() { SolderpasterBacrcode = item, RecoveTime = DateTime.Now, JobNum = ((Users)Session["User"]).UserNum.ToString(), UserName = ((Users)Session["User"]).UserName };
                db.SMT_Recycle.Add(sMT_Recycle);
                count += await db.SaveChangesAsync();
            }
            if (count == smt_RecycleList.Count())
                return true;
            else
                return false;
        }
        #endregion

        #region 看板


        public ActionResult Boadr()
        {
            JArray totle = new JArray();
            var freezebarcode = db.SMT_Freezer.Select(c => c.SolderpasterBacrcode).Distinct();
            foreach (var item in freezebarcode)
            {
                JObject barcodejobject = new JObject();

                var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderBy(c => c.IntoTime).ToList();
                for (int i = 0; i < freeze.Count(); i++)
                {
                    //SMT冰柜时间
                    JObject timeList = new JObject();
                    timeList.Add("barcode", item);
                    var freetime = freeze[i].IntoTime;
                    var freetiem2 = new DateTime?();
                    if (i + 1 == freeze.Count())
                    {
                        freetiem2 = DateTime.Now;
                    }
                    else
                    {
                        freetiem2 = freeze[i + 1].IntoTime;
                    }
                    timeList.Add("freezetime", freeze[i].UserName + ":" + freetime);
                    //回温时间
                    var warmtime = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    timeList.Add("warmtime", warmtime == null ? "--" : warmtime.UserName + ":" + warmtime.StartTime.ToString());
                    //搅拌时间
                    var stirtime = db.SMT_Stir.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    JArray stir = new JArray();
                    if (stirtime != null)
                    {
                        stir.Add(stirtime.UserName + ":" + stirtime.StartTime.ToString());
                        stir.Add(stirtime.SecondTime == null ? null : stirtime.SecondName + ":" + stirtime.SecondTime.ToString());
                        stir.Add(stirtime.ThreeTime == null ? null : stirtime.ThreeNum + ":" + stirtime.ThreeTime.ToString());
                        stir.Add(stirtime.FlorTime == null ? null : stirtime.FlorName + ":" + stirtime.FlorTime.ToString());
                        timeList.Add("stirtime", stir);
                    }
                    else
                    { timeList.Add("stirtime", "--"); }
                    //领用时间
                    var employlist = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).ToList();
                    JArray jarrayemply = new JArray();
                    foreach (var employ in employlist)
                    {
                        JObject jitem = new JObject();
                        if (employ != null)
                        {
                            jitem.Add("ordernum", employ.OrderNum);
                            jitem.Add("linnum", employ.LineNum);
                            jitem.Add("employtime", employ.UserName + ":" + employ.StartTime.ToString());
                        }
                        else
                        {
                            jitem.Add("ordernum", "--");
                            jitem.Add("linnum", "--");
                            jitem.Add("employtime", "--");
                        }
                        jarrayemply.Add(jitem);
                    }
                    timeList.Add("employ", jarrayemply);
                    //回收时间
                    var recycle = db.SMT_Recycle.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                    if (i + 1 == freeze.Count())
                    {
                        timeList.Add("recycletime", recycle == null ? "--" : recycle.UserName + ":" + recycle.RecoveTime.ToString());
                    }
                    else
                        timeList.Add("recycletime", "--");
                    barcodejobject.Add(i.ToString(), timeList);
                }
                totle.Add(barcodejobject);
            }
            return Content(JsonConvert.SerializeObject(totle));
        }

        [HttpPost]
        public ActionResult Boadr(string barcode, string bitch, string ReceivingNum, string Supplier, string ordernum)
        {
            var freezebarcode = db.SMT_Freezer.Select(c => c.SolderpasterBacrcode).Distinct();
            if (!string.IsNullOrEmpty(barcode))
            {
                freezebarcode = freezebarcode.Where(c => c == barcode);
            }
            if (!string.IsNullOrEmpty(bitch))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.Batch == bitch).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(ReceivingNum))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.ReceivingNum == ReceivingNum).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(Supplier))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.Supplier == Supplier).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(ordernum))
            {
                var barcodeList = db.SMT_Employ.Where(c => c.OrderNum == ordernum).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }

            JArray totle = new JArray();

            foreach (var item in freezebarcode)
            {
                JObject barcodejobject = new JObject();

                var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderBy(c => c.IntoTime).ToList();
                for (int i = 0; i < freeze.Count(); i++)
                {
                    //SMT冰柜时间
                    JObject timeList = new JObject();
                    timeList.Add("barcode", item);
                    var freetime = freeze[i].IntoTime;
                    var freetiem2 = new DateTime?();
                    if (i + 1 == freeze.Count())
                    {
                        freetiem2 = DateTime.Now;
                    }
                    else
                    {
                        freetiem2 = freeze[i + 1].IntoTime;
                    }
                    timeList.Add("freezetime", freeze[i].UserName + ":" + freetime);
                    //回温时间
                    var warmtime = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    timeList.Add("warmtime", warmtime == null ? "--" : warmtime.UserName + ":" + warmtime.StartTime.ToString());
                    //搅拌时间
                    var stirtime = db.SMT_Stir.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    JArray stir = new JArray();
                    if (stirtime != null)
                    {
                        stir.Add(stirtime.UserName + ":" + stirtime.StartTime.ToString());
                        stir.Add(stirtime.SecondTime == null ? null : stirtime.SecondName + ":" + stirtime.SecondTime.ToString());
                        stir.Add(stirtime.ThreeTime == null ? null : stirtime.ThreeNum + ":" + stirtime.ThreeTime.ToString());
                        stir.Add(stirtime.FlorTime == null ? null : stirtime.FlorName + ":" + stirtime.FlorTime.ToString());
                        timeList.Add("stirtime", stir);
                    }
                    else
                    { timeList.Add("stirtime", "--"); }
                    //领用时间
                      var employlist = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).ToList();
                    JArray jarrayemply = new JArray();
                    foreach (var employ in employlist)
                    {
                        JObject jitem = new JObject();
                        if (employ != null)
                        {
                            jitem.Add("ordernum", employ.OrderNum);
                            jitem.Add("linnum", employ.LineNum);
                            jitem.Add("employtime", employ.UserName + ":" + employ.StartTime.ToString());
                        }
                        else
                        {
                            jitem.Add("ordernum", "--");
                            jitem.Add("linnum", "--");
                            jitem.Add("employtime", "--");
                        }
                        jarrayemply.Add(jitem);
                    }
                    timeList.Add("employ", jarrayemply);
                    //回收时间
                    var recycle = db.SMT_Recycle.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                    if (i + 1 == freeze.Count())
                    {
                        timeList.Add("recycletime", recycle == null ? "--" : recycle.UserName + ":" + recycle.RecoveTime.ToString());
                    }
                    else
                        timeList.Add("recycletime", "--");
                    barcodejobject.Add(i.ToString(), timeList);
                }
                totle.Add(barcodejobject);
            }
            return Content(JsonConvert.SerializeObject(totle));
        }
        #endregion

        #region 列表获取
        //领用订单列表获取
        public ActionResult GetOrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }


        //看板条码列表获取
        public ActionResult GetBoardBarcodeList()
        {
            var orders = db.SMT_Freezer.OrderByDescending(m => m.ID).Select(m => m.SolderpasterBacrcode).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //看板批次列表获取
        public ActionResult GetBoardBatchList()
        {
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Select(m => m.Batch).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //看板厂商简称列表获取
        public ActionResult GetBoardSupplierList()
        {
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Select(m => m.Supplier).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //看板物料编号列表获取
        public ActionResult GetBoardReceivingNumList()
        {
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Select(m => m.ReceivingNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //看板订单列表获取
        public ActionResult GetBoardOrdernumList()
        {
            var orders = db.SMT_Employ.OrderByDescending(m => m.ID).Select(m => m.OrderNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //锡膏再打印物料号获取
        public ActionResult GetPrintAgainReceivingNumList()
        {
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Select(m => m.ReceivingNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //锡膏再打印生产日期获取
        public ActionResult GetPrintAgainLeaveFactoryTimeList(string receivingNum)
        {
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Where(c => c.ReceivingNum == receivingNum).Select(m => m.LeaveFactoryTime).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                var vaule = string.Format("{0:yyyy-MM-dd}", item);
                List.Add("value", vaule);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 暂时没用
        ////锡膏条码生成
        //public List<string> AddBarcode(string receivingNum, DateTime productionDate, int Num)
        //{
        //    if (!string.IsNullOrEmpty(receivingNum) || productionDate != null)
        //    {
        //        string year = productionDate.Year.ToString().Substring(2);
        //        List<string> barcodeList = new List<string>();
        //        for (int i = 1; i <= Num; i++)
        //        {
        //            var lastBarcode = db.Barcode_Solderpaste.Where(c => c.ReceivingNum == receivingNum).OrderByDescending(c => c.SolderpasterBacrcode).FirstOrDefault();
        //            if (lastBarcode != null&&lastBarcode.LeaveFactoryTime==productionDate)
        //            {

        //                string lastNUm = lastBarcode.SolderpasterBacrcode.Substring(lastBarcode.SolderpasterBacrcode.Length-3,3);
        //                i = int.Parse(lastNUm)+1;
        //            }

        //            string barcode = receivingNum + "-" + year + productionDate.Month.ToString() + productionDate.Day.ToString() + "-" + i.ToString().PadLeft(3, '0');
        //            barcodeList.Add(barcode);
        //        }
        //        return barcodeList;
        //    }
        //    return null;
        //}

        /// <summary>
        /// 打印条码
        /// </summary>
        /// <param name="Batch"> 批次</param>
        /// <param name="ReceivingNum">料号</param>
        /// <param name="LeaveFactoryTime">生产时间</param>
        /// <param name="Supplier">供应商</param>
        /// <param name="SolderpasteType">型号</param>
        //public async Task<bool> AddBarcode_SolderpasteAsync(string Batch, string ReceivingNum, DateTime LeaveFactoryTime, string Supplier, string SolderpasteType, int EffectiveDay, int Num)
        //{
        //    if (((Users)Session["User"]) != null)
        //    {
        //        return false;
        //    }
        //    if (!string.IsNullOrEmpty(ReceivingNum) || LeaveFactoryTime != null)
        //    {
        //        string year = LeaveFactoryTime.Year.ToString().Substring(2);
        //        List<string> barcodeList = new List<string>();
        //        int count = 0;
        //        for (int i = 1; i <= Num; i++)
        //        {
        //            var lastBarcode = db.Barcode_Solderpaste.Where(c => c.ReceivingNum == ReceivingNum).OrderByDescending(c => c.SolderpasterBacrcode).FirstOrDefault();
        //            if (lastBarcode != null && lastBarcode.LeaveFactoryTime == LeaveFactoryTime)
        //            {

        //                string lastNUm = lastBarcode.SolderpasterBacrcode.Substring(lastBarcode.SolderpasterBacrcode.Length - 3, 3);
        //                i = int.Parse(lastNUm) + 1;
        //            }

        //            string barcode = ReceivingNum + "-" + year + LeaveFactoryTime.Month.ToString() + LeaveFactoryTime.Day.ToString() + "-" + i.ToString().PadLeft(3, '0');

        //            Barcode_Solderpaste barcode_Solderpaste = new Barcode_Solderpaste() { SolderpasterBacrcode = barcode, Batch = Batch, LeaveFactoryTime = LeaveFactoryTime, ReceivingNum = ReceivingNum, SolderpasteType = SolderpasteType, Supplier = Supplier, PrintTime = DateTime.Now, PrintName = ((Users)Session["User"]).UserName };
        //            db.Barcode_Solderpaste.Add(barcode_Solderpaste);
        //            count += await db.SaveChangesAsync();
        //        }
        //        if (count == Num)
        //            return true;
        //    }
        //    return false;
        //}



        #endregion


        #region 打印锡膏标签
        [HttpPost]
        public ActionResult InsideBoxLable_Print(List<string> barcodelist, int pagecount = 1, string ip = "", int port = 0, int concentration = 30)
        {
            //string data = "^XA^MD30~DGR:ZONE.GRF,";
            JArray result = new JArray();
            foreach (var barcode in barcodelist)
            {
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                Bitmap bm = InsideBoxLable_DrawBitmap(barcode);
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO200,0^XGR:ZONE.GRF^FS^XZ";//FO.X,Y座标
                string message = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                JObject obj = new JObject();
                obj.Add("barcode", barcode);
                obj.Add("message", message);
                result.Add(obj);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpPost]
        public ActionResult InsideBoxLable_ToImg(string barcode = "")
        {
            Bitmap bm = InsideBoxLable_DrawBitmap(barcode);
            MemoryStream ms = new MemoryStream();
            bm.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            bm.Dispose();
            return File(ms.ToArray(), "image/Png");
        }


        private Bitmap InsideBoxLable_DrawBitmap(string barcode = "")
        {
            //开始绘制图片
            int initialWidth = 400, initialHeight = 150;//宽8高3
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
            //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));


            //引入条码
            Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(barcode, 388, 60);
            theGraphics.DrawImage(bmp_barcode, 5, 25, bmp_barcode.Width, bmp_barcode.Height);

            //引入条码号
            System.Drawing.Font myFont_modulebarcodenum;
            myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 15, FontStyle.Regular);
            StringFormat geshi1 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            theGraphics.DrawString(barcode, myFont_modulebarcodenum, bush, 200, 80, geshi1);
            //结束图片绘制以上都是绘制图片的代码

            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值
            return bm;
        }

        #endregion
    }
}