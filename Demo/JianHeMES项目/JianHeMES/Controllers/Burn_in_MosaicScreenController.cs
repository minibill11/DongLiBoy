﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.AuthAttributes;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JianHeMES.Controllers
{
    public class Burn_in_MosaicScreenController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public class TempBurn_in
        {

            public string BarCodesNum { get; set; }
            public DateTime? OQCCheckBT { get; set; }
            public DateTime? OQCCheckFT { get; set; }
            public bool OQCCheckFinish { get; set; }
            public string Burn_in_OQCCheckAbnormal { get; set; }
        }
        public class TempBurn_in_MosaicScreen
        {

            public string BurnInShelfNum { get; set; }
            public string OrderNum { get; set; }
            public string BarCodesNum { get; set; }
        }
        #region 开始拼屏
        public ActionResult mosaicScreen_B()
        {
            //if (com.isCheckRole("老化管理", "老化拼屏操作", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{

            return View();
            //}
            //return Content("<script>alert('对不起，您不能进行老化拼屏操作操作，请联系品质部管理人员！');window.location.href='../FinalQCs';</script>");

        }
        /// <summary>
        /// 添加拼屏扫码记录
        /// </summary>
        /// 循环条码列表找到没有重复的列表,添加进拼屏表中,并提供给前端显示那一部分完成拼屏,找到有重复的列表,记录并显示那一部分的列表有重复
        /// <param name="OrderNum">订单号</param>
        /// <param name="BurnInShelfNum">老化架号</param>
        /// <param name="BarCodesNumList">条码列表</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult mosaicScreen_Begin(string OrderNum, string BurnInShelfNum, List<string> BarCodesNumList,string Department1,string Group)
        {
            var newRecord = new Burn_in_MosaicScreen();
            newRecord.OrderNum = OrderNum;
            string result_putin = "";
            string result_never_putin = "";
            foreach (var item in BarCodesNumList)//循环条码列表
            {
                var count = db.Burn_in_MosaicScreen.Count(c => c.BarCodesNum == item&&(c.OldBarCodesNum==null||c.OldBarCodesNum==item));//查找拼屏报是否有相同的数据
                if (count == 0)//没有相同数据,创建新数据
                {
                    newRecord.BarCodesNum = item;
                    newRecord.OQCMosaicStartTime = DateTime.Now;
                    newRecord.OQCPrincipalNum = ((Users)Session["User"]).UserNum.ToString();
                    newRecord.OldBarCodesNum = item;
                    newRecord.OldOrderNum = OrderNum;
                    newRecord.Department1 = Department1;
                    newRecord.Group = Group;
                       
                    // "16621";
                    newRecord.BurnInShelfNum = BurnInShelfNum;
                    db.Burn_in_MosaicScreen.Add(newRecord);//保存内容
                    db.SaveChanges();
                    if (result_putin == "")//将创建新的条码存进result_putin
                    {
                        result_putin = item;
                    }
                    else
                    {
                        result_putin = result_putin + ",\n" + item;
                    }
                }
                else//否则将相同数据的的条码存进result_never_putin
                {
                    if (result_never_putin == "")
                    {
                        result_never_putin = item;
                    }
                    else
                    {
                        result_never_putin = result_never_putin + ",\n" + item;
                    }
                }
            }
            if (result_putin != "")
            {
                result_putin = result_putin + "\n完成拼屏操作。";
            }
            if (result_never_putin != "")
            {
                result_never_putin = result_never_putin + "\n此条码已经在老化架上，请确认条码的准确。";
            }
            if (result_putin + result_never_putin != "")
            {
                return Content(result_putin + result_never_putin);
            }
            else
            {
                return Content("没有条码。");
            }
            //return Json(data,JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 开始扫码清单
        /// </summary>
        /// 读取拼屏表中该老化架号的所有条码,根据这些条码找到老化表中已经开始老化的条码和已经完成老化的条码,将已经开始老化的条码返回给前面.在老化架号的所有条码剔除掉已经开始老化的条码,剩下已完成的和未开始的条码,在从这些条码中剔除到已完成的条码,身下未开始的条码,返回给前面
        /// <param name="BurnInShelfNum">老化架号</param>
        /// <returns></returns>
        public ActionResult disPlayMosaicScreenList(string BurnInShelfNum)
        {
            JObject list = new JObject();
            //读取拼屏表中该老化架号的所有条码
            var mosaicScreenList = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == BurnInShelfNum && c.OQCMosaicStartTime != null && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)).Select(c => c.BarCodesNum).ToList();
            //读取条码中已经开始老化的条码
            var startBurn_inList = db.Burn_in.Where(c => mosaicScreenList.Contains(c.BarCodesNum) && c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)).Select(c => c.BarCodesNum).ToList();
            //读取条码中已经完成老化的条码
            var endStartBurn_inList = db.Burn_in.Where(c => mosaicScreenList.Contains(c.BarCodesNum) && c.OQCCheckBT != null && c.OQCCheckFT != null && c.OQCCheckFinish == true && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)).Select(c => c.BarCodesNum).ToList();
            //去除已经开始和已经完成老化的条码，得到已经拼屏但没做任何操作的条码
            var temp = mosaicScreenList.Except(startBurn_inList).ToList();
            var notStartBurn_inList = temp.Except(endStartBurn_inList).ToList();

            list.Add("startburn_in", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(startBurn_inList)));
            list.Add("notstartburn_in", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(notStartBurn_inList)));
            return Content(JsonConvert.SerializeObject(list));
        }

        /// <summary>
        /// 修改拼屏号
        /// </summary>
        /// 根据条码找到条码信息,替换老化架号
        /// <param name="oldShelfNum">旧的老化架号</param>
        /// <param name="newShelfNum">新的老化架号</param>
        /// <param name="barcodeList">条码列表</param>
        public void editShelfNum(string oldShelfNum, string newShelfNum, List<string> barcodeList)
        {
            foreach (var item in barcodeList)
            {
                var edit = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == oldShelfNum && c.BarCodesNum == item).FirstOrDefault();//找到条码信息
                edit.BurnInShelfNum = newShelfNum;//替换老化架号
                db.SaveChangesAsync();
            }
        }

        #region 完成拼屏  暂时不用
        public ActionResult mosaicScreen_F()
        {
            //权限
            ViewBag.OrderList = GetOrderList();
            return View();

        }
        [HttpPost]
        public ActionResult mosaicScreenFinish(string OrderNum, List<string> BarCodesNumList)
        {
            foreach (var barcode in BarCodesNumList)
            {
                var mosaicScreen = db.Burn_in_MosaicScreen.Where(c => c.OrderNum == OrderNum && c.BarCodesNum == barcode && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).FirstOrDefault();
                if (mosaicScreen != null)
                {
                    mosaicScreen.OQCMosaicEndTime = DateTime.Now;
                    db.SaveChanges();
                }
            }
            return Content("完成拼屏");


        }
        #endregion

        //拼屏数据显示
        public ActionResult mosaicScreen_ShelfQuery()
        {
            return View();

        }

        /// <summary>
        /// 拼屏数据显示
        /// </summary>
        /// 根据筛选条件中找到符合条件的拼屏列表,//循环老化架号,根据老化架号,在符合条件的拼屏列表中找到订单列表,循环订单列表,根据订单和老化架号,找到符合条件的拼屏列表里的条码列表,循环条码列表,判断条码是属于 未开始拼屏/已拼屏易老化/有异常 传给前端
        /// <param name="seletorder">选择的订单号</param>
        /// <param name="shelfNum">选择的老化架号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult mosaicScreen_ShelfQueryData(List<string>seletorder,List<string> shelfNum)
        {
            JObject oneShelfNum = new JObject();
            JArray ShelfNumJosnList = new JArray();
            JObject onebarcodeInfo = new JObject();
            JArray barcodeJsonList = new JArray();
            JObject oneOrderNum = new JObject();
            JArray OrderNumJsonList = new JArray();
            var BurnInShelfNumList = new List<string>();
            var BurnInmosaic = new List<TempBurn_in_MosaicScreen>();
            if (seletorder != null)//根据订单找到拼屏数据 (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)这个是剔除掉挪用
            {
                BurnInmosaic = db.Burn_in_MosaicScreen.Where(c => seletorder.Contains(c.OrderNum)&&(c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum) && c.OQCMosaicEndTime == null).Select(c => new TempBurn_in_MosaicScreen { BurnInShelfNum = c.BurnInShelfNum, OrderNum = c.OrderNum, BarCodesNum = c.BarCodesNum }).ToList();
            }
            else//没有订单号,则查找全部数据,挪用除外
            {
                BurnInmosaic = db.Burn_in_MosaicScreen.Where(c => (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum) && c.OQCMosaicEndTime == null).Select(c => new TempBurn_in_MosaicScreen { BurnInShelfNum = c.BurnInShelfNum, OrderNum = c.OrderNum, BarCodesNum = c.BarCodesNum }).ToList();
            }
            if (shelfNum != null)//如果有老化架号,根据老化架号找
            {
                BurnInShelfNumList = shelfNum;
            }
            else//没有则找出所有老化架号
            {
                BurnInShelfNumList = BurnInmosaic.OrderBy(c => c.BurnInShelfNum).Select(c => c.BurnInShelfNum).Distinct().ToList();
            }

            var tempburn = db.Burn_in.Where(c => c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && (c.OldBarCodesNum == c.BarCodesNum || c.OldBarCodesNum == null)).Select(c => new TempBurn_in { BarCodesNum = c.BarCodesNum, Burn_in_OQCCheckAbnormal = c.Burn_in_OQCCheckAbnormal, OQCCheckBT = c.OQCCheckBT, OQCCheckFinish = c.OQCCheckFinish, OQCCheckFT = c.OQCCheckFT }).ToList();//查找老化已开始未完成的列表

            foreach (var item in BurnInShelfNumList)//循环老化架号
            {
                var OrderNumList = BurnInmosaic.Where(c => c.BurnInShelfNum == item).Select(c => c.OrderNum).Distinct().ToList();//根据老化架号查找订单号
                
                foreach (var OrderNum in OrderNumList)//循环订单号
                {
                    var barcodeList = BurnInmosaic.OrderBy(c => c.BarCodesNum).Where(c => c.BurnInShelfNum == item && c.OrderNum == OrderNum).Select(c => c.BarCodesNum).ToList();//找到已经排序号的条码号
                    foreach (var barcode in barcodeList)//循环条码号
                    {
                        onebarcodeInfo.Add("barcode", barcode);//条码号
                        var currentburn = tempburn.Where(c => c.BarCodesNum == barcode).FirstOrDefault();//是否在已开始为完成老化列表
                        if (currentburn == null)//不是,代表未开始老化拼屏
                        {
                            onebarcodeInfo.Add("status", 0);
                            onebarcodeInfo.Add("Abnormal", "");
                        }
                        //已经拼屏已经开始老化
                        else if (currentburn.Burn_in_OQCCheckAbnormal == null)
                        {
                            onebarcodeInfo.Add("status", 1);
                            onebarcodeInfo.Add("Abnormal", "");
                        }
                        //有异常
                        else if (currentburn.Burn_in_OQCCheckAbnormal != null)
                        {
                            onebarcodeInfo.Add("status", 2);

                            onebarcodeInfo.Add("Abnormal", currentburn.Burn_in_OQCCheckAbnormal);
                        }

                        barcodeJsonList.Add(onebarcodeInfo);

                        onebarcodeInfo = new JObject();
                    }
                    oneOrderNum.Add("barcodelist", barcodeJsonList);//条码列表
                    oneOrderNum.Add("ordernum", OrderNum);//订单号
                    OrderNumJsonList.Add(oneOrderNum);

                    barcodeJsonList = new JArray();
                    oneOrderNum = new JObject();

                }
                if (OrderNumJsonList.Count == 0)
                {
                    continue;
                }
                oneShelfNum.Add("ShelfNum", item);//老化架号
                oneShelfNum.Add("content", OrderNumJsonList);//条码内容
                ShelfNumJosnList.Add(oneShelfNum);

                OrderNumJsonList = new JArray();
                oneShelfNum = new JObject();

            }

            if (BurnInShelfNumList.Count != 0)
            {
                return Content(JsonConvert.SerializeObject(ShelfNumJosnList));
            }
            return Content("没有数据");
        }



        public ActionResult mosaicScreen_ShelfQueryHistory()
        {
            return View();

        }

        /// <summary>
        /// 老化拼屏历史查询
        /// </summary>
        /// 根据订单找到老化架号,循环老化架号,根据老化架号和订单找到拼屏表的条码列表.循环条码,判断条码是属于 已经开始老化/有异常/已经完成老化/没有开始,那种状态,显示给前端
        /// <param name="ordernum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult mosaicScreen_ShelfQueryHistory(string ordernum)
        {
            JArray total = new JArray();
            JObject BurnSelf = new JObject();
            var BurnInShelfNumList = db.Burn_in_MosaicScreen.Where(c => c.OrderNum == ordernum&& (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)).Select(c => c.BurnInShelfNum).Distinct().ToList();//根据订单查找拼屏表里的老化架号
            foreach (var item in BurnInShelfNumList)//循环老化架号
            {

                BurnSelf.Add("ShelfNum", item);//老化架号
                var barcodeList = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == item && c.OrderNum == ordernum&& (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)).Select(c => c.BarCodesNum).ToList();//根据订单和老化架号找到条码列表

                JArray barcodeListJarrry = new JArray();
                foreach (var barcode in barcodeList)//循环条码列表
                {
                    JObject onebarcodeInfo = new JObject();
                    onebarcodeInfo.Add("barcode", barcode);

                    //已经开始老化
                    if (db.Burn_in.Count(c => c.BarCodesNum == barcode && c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal == null&& (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)) > 0)//查看条码是否存在老化表,并且老化信息为已开始未结束没异常非挪用
                    {
                        onebarcodeInfo.Add("status", 1);
                        onebarcodeInfo.Add("Abnormal", "");
                    }
                    //有异常
                    else if (db.Burn_in.Count(c => c.BarCodesNum == barcode && c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal != null&& (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)) >= 1)//查看条码是否存在老化表,并且老化信息为已开始,未结束,有异常,非挪用
                    {
                        onebarcodeInfo.Add("status", 2);
                        var Abnormal = db.Burn_in.Where(c => c.BarCodesNum == barcode && c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal != null&& (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)).Select(c => c.Burn_in_OQCCheckAbnormal).FirstOrDefault();
                        onebarcodeInfo.Add("Abnormal", Abnormal);
                    }
                    //已完成老化
                    else if (db.Burn_in.Count(c => c.BarCodesNum == barcode && c.OQCCheckBT != null && c.OQCCheckFT != null && c.OQCCheckFinish == true&& (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)) > 0)///查看条码是否存在老化表,并且老化信息为已开始,已结束,非挪用
                    {
                        onebarcodeInfo.Add("status", 3);
                        onebarcodeInfo.Add("Abnormal", "");
                    }
                    else//没有开始,不存在老化表
                    {
                        onebarcodeInfo.Add("status", 0);
                        onebarcodeInfo.Add("Abnormal", "");
                    }
                    barcodeListJarrry.Add(onebarcodeInfo);
                }
                JObject oneOrderNum = new JObject();
                JArray ordernumJoject = new JArray();
                if (barcodeList.Count != 0)
                {

                    oneOrderNum.Add("barcodelist", barcodeListJarrry);
                    oneOrderNum.Add("ordernum", ordernum);
                }
                ordernumJoject.Add(oneOrderNum);
                BurnSelf.Add("content", ordernumJoject);
                total.Add(BurnSelf);
            }
            if (BurnInShelfNumList.Count != 0)
            {
                return Content(JsonConvert.SerializeObject(total));
            }
            return Content("没有数据");

        }

        // GET: Burn_in_MosaicScreen
        public ActionResult Index()
        {
            return View(db.Burn_in_MosaicScreen.ToList());

        }
        /// <summary>
        /// 现在没用到
        /// </summary>
        /// <param name="burnInShelfNum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string burnInShelfNum)
        {
            JObject MosaicScreen = new JObject();
            JObject MosaicScreenFinshing = new JObject();
            JObject MosaicScreenBeagin = new JObject();
            var count = db.Burn_in_MosaicScreen.Count(c => c.BurnInShelfNum == burnInShelfNum);
            var OQCinfoFinshing = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == burnInShelfNum && c.OQCMosaicStartTime != null && c.OQCMosaicEndTime != null).ToList();
            var OQCinfoBeagin = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == burnInShelfNum && c.OQCMosaicStartTime != null && c.OQCMosaicEndTime == null).ToList();
            foreach (var item in OQCinfoFinshing)
            {
                MosaicScreenFinshing.Add("BarCodesNum", item.BarCodesNum);
            }
            foreach (var item in OQCinfoBeagin)
            {
                MosaicScreenBeagin.Add("BarCodesNum", item.BarCodesNum);
            }
            //已完成列表
            MosaicScreen.Add("Finashing", MosaicScreenFinshing);
            //已完成数量
            MosaicScreen.Add("FinashingCount", OQCinfoFinshing.Count());
            //未完成列表
            MosaicScreen.Add("Beagin", MosaicScreenBeagin);
            //未完成数量
            MosaicScreen.Add("BeaginCount", OQCinfoBeagin.Count());
            //总数量
            MosaicScreen.Add("Count", count);
            return Content(JsonConvert.SerializeObject(MosaicScreen));

        }
        // GET: Burn_in_MosaicScreen/Details/5 
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Burn_in_MosaicScreen burn_in_MosaicScreen = db.Burn_in_MosaicScreen.Find(id);
            if (burn_in_MosaicScreen == null)
            {
                return HttpNotFound();
            }
            return View(burn_in_MosaicScreen);
        }

        // GET: Burn_in_MosaicScreen/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Burn_in_MosaicScreen/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public ActionResult Create(List<Burn_in_MosaicScreen> burn_in_MosaicScreenList)
        {
            foreach (var burn_in_MosaicScreen in burn_in_MosaicScreenList)
            {
                if (ModelState.IsValid)
                {
                    db.Burn_in_MosaicScreen.Add(burn_in_MosaicScreen);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        // GET: Burn_in_MosaicScreen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Burn_in_MosaicScreen burn_in_MosaicScreen = db.Burn_in_MosaicScreen.Find(id);
            if (burn_in_MosaicScreen == null)
            {
                return HttpNotFound();
            }
            return View(burn_in_MosaicScreen);
        }

        // POST: Burn_in_MosaicScreen/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OrderNum,BarCodesNum,BurnInShelfNum,OQCPrincipalNum,OQCMosaicStartTime,OQCMosaicEndTime,Remark")] Burn_in_MosaicScreen burn_in_MosaicScreen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(burn_in_MosaicScreen).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(burn_in_MosaicScreen);
        }

        // GET: Burn_in_MosaicScreen/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Burn_in_MosaicScreen burn_in_MosaicScreen = db.Burn_in_MosaicScreen.Find(id);
            if (burn_in_MosaicScreen == null)
            {
                return HttpNotFound();
            }
            return View(burn_in_MosaicScreen);
        }

        // POST: Burn_in_MosaicScreen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Burn_in_MosaicScreen burn_in_MosaicScreen = db.Burn_in_MosaicScreen.Find(id);
            db.Burn_in_MosaicScreen.Remove(burn_in_MosaicScreen);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 检查开始拼屏FQC是否检查
        /// </summary>
        /// 循环条码列表,查找条码信息,找不到条码信息提示"未找到此条码",否则查看条码信息的订单与传过来的订单是否相符,不符提示"条码不属于此订单",否则判断老化表信息是否为空,不为空再判断是否已经完成老化或正在老化.如果老化信息为空,判断拼屏 信息是否未空,不为空提示"条码已经拼屏",为空再判断fqc信息是否为空,为空提示"FQC未检查",否则提示正常
        /// <param name="ordernum">订单号</param>
        /// <param name="barcodeList">条码列表</param>
        /// <returns></returns>
        public ActionResult CheckFQC(string ordernum, List<string> barcodeList)
        {
            JObject FQCcheckList = new JObject();
            foreach (var barcode in barcodeList)//循环条码列表
            {
                var mosicScreenInfo = db.Burn_in_MosaicScreen.Where(c => c.BarCodesNum == barcode&& (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).FirstOrDefault();//根据条码找到拼屏信息
                var burn_in = db.Burn_in.Where(c => c.BarCodesNum == barcode && c.OrderNum == ordernum && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).FirstOrDefault();//根据条码找到老化信息
                var barcodeinfo = db.BarCodes.Where(c => c.BarCodesNum == barcode).FirstOrDefault();//根据条码找到条码信息
                var FQCcount = db.FinalQC.Where(c => c.BarCodesNum == barcode && c.FQCCheckFinish == true && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();//根据条码找到FQC信息
                if (barcodeinfo == null)//在条码信息找不到条码
                {
                    FQCcheckList.Add(barcode, "未找到此条码");
                    continue;
                }
                if (barcodeinfo.OrderNum != ordernum)//在条码信息找到的条码订单与传过来的订单不符
                {
                    FQCcheckList.Add(barcode, "条码不属于此订单");
                    continue;
                }
                if (burn_in != null)//老化表信息不为空
                {
                    if (burn_in.OQCCheckFinish == true)//老化已完成
                    {
                        FQCcheckList.Add(barcode, "条码已经完成老化");
                        continue;
                    }
                    if (burn_in.OQCCheckFinish == false && burn_in.OQCCheckBT != null && burn_in.OQCCheckFT == null)//老化未完成,已开始
                    {
                        FQCcheckList.Add(barcode, "条码已经在老化");
                        continue;
                    }
                }
                if (mosicScreenInfo != null)//拼屏信息不为空
                {
                    FQCcheckList.Add(barcode, "条码已经拼屏");
                    continue;
                }
                if (FQCcount == 0)//fqc信息为空
                {
                    FQCcheckList.Add(barcode, "FQC未检查");
                    continue;
                }
                else
                {
                    FQCcheckList.Add(barcode, "正常");
                }
            }
            return Content(JsonConvert.SerializeObject(FQCcheckList));
        }
        
        /// <summary>
        ///检查条码是否能完成拼屏
        /// </summary>
        /// 这个方法没用,没有完成拼屏这个动作
        /// <param name="ordernum">订单号</param>
        /// <param name="barcodeList">条码列表</param>
        /// <returns></returns>
        public ActionResult CheckMosciScreenF(string ordernum, List<string> barcodeList)
        {
            JObject checkList = new JObject();
            foreach (var item in barcodeList)
            {
                var mosicScreenInfo = db.Burn_in_MosaicScreen.Where(c => c.BarCodesNum == item && (c.OldBarCodesNum == null || c.OldBarCodesNum == item)).FirstOrDefault();//拼屏信息
                if (mosicScreenInfo == null)//没有拼屏信息
                {
                    checkList.Add(item, "未找到此条码");
                    continue;
                }
                else
                {
                    if (mosicScreenInfo.OQCMosaicStartTime == null)//
                    {
                        checkList.Add(item, "未开始拼屏");
                        continue;
                    }
                    else if (mosicScreenInfo.OQCMosaicEndTime != null)
                    {
                        checkList.Add(item, "已完成拼屏");
                        continue;
                    }
                    else if (ordernum != mosicScreenInfo.OrderNum)
                    {
                        checkList.Add(item, "条码不属于此订单");
                        continue;
                    }
                    else
                    {
                        checkList.Add(item, "正常");
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(checkList));
        }

        #region ---------------------------------------GetOrderList()取出整个OrderMgms的OrderNum订单号列表
        private List<SelectListItem> GetOrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
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
        #endregion

        #region --------------------GetnuoOrderList()取出整个OrderMgms的挪用单号列表
        private List<SelectListItem> GetnuoOrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Where(m => m.IsRepertory == true).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
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

        private ActionResult GetnuoOrderList1()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Where(m => m.IsRepertory == true).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //----------------------------------------------------------------------------------------
        #endregion

        #region --------------------GetMosaiOrderList()取出整个OrderMgms的挪用单号列表
        private List<SelectListItem> GetMosaiOrderList()
        {
            var orders = db.Burn_in_MosaicScreen.OrderByDescending(m => m.Id).Select(m => m.OrderNum).Distinct();    //增加.Distinct()后会重新按OrderNum升序排序
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

        private ActionResult GetMosaiOrderList1()
        {
            var orders = db.Burn_in_MosaicScreen.OrderByDescending(m => m.Id).Select(m => m.OrderNum).Distinct();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //----------------------------------------------------------------------------------------
        #endregion

        #region --------------------GetmosaiShelfNumList()取出整个OrderMgms的挪用单号列表
        private List<SelectListItem> GetmosaiShelfNumList()
        {
            var orders = db.Burn_in_MosaicScreen.OrderByDescending(m => m.Id).Select(m => m.BurnInShelfNum).Distinct();    //增加.Distinct()后会重新按OrderNum升序排序
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

        private ActionResult GetmosaiShelfNumList1()
        {
            var orders = db.Burn_in_MosaicScreen.OrderByDescending(m => m.Id).Select(m => m.BurnInShelfNum).Distinct();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //----------------------------------------------------------------------------------------
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    
    public class Burn_in_MosaicScreen_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Common_ApiController comapi = new Common_ApiController();
        private CommonController com = new CommonController();

        public class TempBurn_in
        {

            public string BarCodesNum { get; set; }
            public DateTime? OQCCheckBT { get; set; }
            public DateTime? OQCCheckFT { get; set; }
            public bool OQCCheckFinish { get; set; }
            public string Burn_in_OQCCheckAbnormal { get; set; }
        }
        public class TempBurn_in_MosaicScreen
        {

            public string BurnInShelfNum { get; set; }
            public string OrderNum { get; set; }
            public string BarCodesNum { get; set; }
        }

        #region 开始拼屏
        /// <summary>
        /// 添加拼屏扫码记录
        /// </summary>
        /// 循环条码列表找到没有重复的列表,添加进拼屏表中,并提供给前端显示那一部分完成拼屏,找到有重复的列表,记录并显示那一部分的列表有重复
        /// <param name="OrderNum">订单号</param>
        /// <param name="BurnInShelfNum">老化架号</param>
        /// <param name="BarCodesNumList">条码列表</param>
        /// <returns></returns>
        
        [HttpPost]
        [ApiAuthorize]
        public JObject mosaicScreen_Begin([System.Web.Http.FromBody]JObject data)
        {
            string OrderNum = data["OrderNum"].ToString();
            string UserNum = data["UserNum"].ToString();
            string Department = data["Department"].ToString();
            string Group = data["Group"].ToString();
            string BurnInShelfNum = data["BurnInShelfNum"].ToString();
            JArray BarCodesNumList = (JArray)data["BarCodesNumList"];
            var newRecord = new Burn_in_MosaicScreen();
            JObject result = new JObject();
            newRecord.OrderNum = OrderNum;
            string result_putin = "";
            string result_never_putin = "";
            foreach (var token in BarCodesNumList)//循环条码列表
            {
                var item = token.ToString();
                var count = db.Burn_in_MosaicScreen.Count(c => c.BarCodesNum == item && (c.OldBarCodesNum == null || c.OldBarCodesNum == item));//查找拼屏报是否有相同的数据
                if (count == 0)//没有相同数据,创建新数据
                {
                    newRecord.BarCodesNum = item;
                    newRecord.OQCMosaicStartTime = DateTime.Now;
                    newRecord.OQCPrincipalNum = UserNum;
                    newRecord.OldBarCodesNum = item;
                    newRecord.OldOrderNum = OrderNum;
                    newRecord.Department1 = Department;
                    newRecord.Group = Group;

                    // "16621";
                    newRecord.BurnInShelfNum = BurnInShelfNum;
                    db.Burn_in_MosaicScreen.Add(newRecord);//保存内容
                    db.SaveChanges();
                    if (result_putin == "")//将创建新的条码存进result_putin
                    {
                        result_putin = item;
                    }
                    else
                    {
                        result_putin = result_putin + ",\n" + item;
                    }
                }
                else//否则将相同数据的的条码存进result_never_putin
                {
                    if (result_never_putin == "")
                    {
                        result_never_putin = item;
                    }
                    else
                    {
                        result_never_putin = result_never_putin + ",\n" + item;
                    }
                }
            }
            if (result_putin != "")
            {
                result_putin = result_putin + "\n完成拼屏操作。";
            }
            if (result_never_putin != "")
            {
                result_never_putin = result_never_putin + "\n此条码已经在老化架上，请确认条码的准确。";
            }
            if (result_putin + result_never_putin != "")
            {
                return com.GetModuleFromJobjet(result, true, result_putin + result_never_putin);
            }
            else
            {
                return com.GetModuleFromJobjet(result, false, "没有条码");
            }
            //return Json(data,JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 开始扫码清单
        /// </summary>
        /// 读取拼屏表中该老化架号的所有条码,根据这些条码找到老化表中已经开始老化的条码和已经完成老化的条码,将已经开始老化的条码返回给前面.在老化架号的所有条码剔除掉已经开始老化的条码,剩下已完成的和未开始的条码,在从这些条码中剔除到已完成的条码,身下未开始的条码,返回给前面
        /// <param name="BurnInShelfNum">老化架号</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject disPlayMosaicScreenList([System.Web.Http.FromBody]JObject data)
        {
            string BurnInShelfNum = data["BurnInShelfNum"].ToString();
            JObject list = new JObject();
            //读取拼屏表中该老化架号的所有条码
            var mosaicScreenList = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == BurnInShelfNum && c.OQCMosaicStartTime != null && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)).Select(c => c.BarCodesNum).ToList();
            //读取条码中已经开始老化的条码
            var startBurn_inList = db.Burn_in.Where(c => mosaicScreenList.Contains(c.BarCodesNum) && c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)).Select(c => c.BarCodesNum).ToList();
            //读取条码中已经完成老化的条码
            var endStartBurn_inList = db.Burn_in.Where(c => mosaicScreenList.Contains(c.BarCodesNum) && c.OQCCheckBT != null && c.OQCCheckFT != null && c.OQCCheckFinish == true && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)).Select(c => c.BarCodesNum).ToList();
            //去除已经开始和已经完成老化的条码，得到已经拼屏但没做任何操作的条码
            var temp = mosaicScreenList.Except(startBurn_inList).ToList();
            var notStartBurn_inList = temp.Except(endStartBurn_inList).ToList();

            list.Add("startburn_in", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(startBurn_inList)));
            list.Add("notstartburn_in", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(notStartBurn_inList)));
            return com.GetModuleFromJobjet(list);
        }


        /// <summary>
        /// 修改拼屏号
        /// </summary>
        /// 根据条码找到条码信息,替换老化架号
        /// <param name="oldShelfNum">旧的老化架号</param>
        /// <param name="newShelfNum">新的老化架号</param>
        /// <param name="barcodeList">条码列表</param>
     
        [HttpPost]
        [ApiAuthorize]
        public void editShelfNum([System.Web.Http.FromBody]JObject data)
        {
            string oldShelfNum = data["oldShelfNum"].ToString();
            string newShelfNum = data["newShelfNum"].ToString();
            JArray barcodeList =(JArray) data["barcodeList"];
            foreach (var token in barcodeList)
            {
                var item = token.ToString();
                var edit = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == oldShelfNum && c.BarCodesNum == item).FirstOrDefault();//找到条码信息
                edit.BurnInShelfNum = newShelfNum;//替换老化架号
                db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 拼屏数据显示
        /// </summary>
        /// 根据筛选条件中找到符合条件的拼屏列表,//循环老化架号,根据老化架号,在符合条件的拼屏列表中找到订单列表,循环订单列表,根据订单和老化架号,找到符合条件的拼屏列表里的条码列表,循环条码列表,判断条码是属于 未开始拼屏/已拼屏易老化/有异常 传给前端
        /// <param name="seletorder">选择的订单号</param>
        /// <param name="shelfNum">选择的老化架号</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject mosaicScreen_ShelfQueryData([System.Web.Http.FromBody]JObject data)
        {
            JObject oneShelfNum = new JObject();
            JArray ShelfNumJosnList = new JArray();
            JObject onebarcodeInfo = new JObject();
            JArray barcodeJsonList = new JArray();
            JObject oneOrderNum = new JObject();
            JArray OrderNumJsonList = new JArray();
            var seletorder = data["seletorder"].ToList();
            JArray shelfNum = (JArray)data["shelfNum"];

            var BurnInShelfNumList = new List<string>();
            var BurnInmosaic = new List<TempBurn_in_MosaicScreen>();
            if (seletorder != null)//根据订单找到拼屏数据 (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)这个是剔除掉挪用
            {
                BurnInmosaic = db.Burn_in_MosaicScreen.Where(c => seletorder.Contains(c.OrderNum) && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum) && c.OQCMosaicEndTime == null).Select(c => new TempBurn_in_MosaicScreen { BurnInShelfNum = c.BurnInShelfNum, OrderNum = c.OrderNum, BarCodesNum = c.BarCodesNum }).ToList();
            }
            else//没有订单号,则查找全部数据,挪用除外
            {
                BurnInmosaic = db.Burn_in_MosaicScreen.Where(c => (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum) && c.OQCMosaicEndTime == null).Select(c => new TempBurn_in_MosaicScreen { BurnInShelfNum = c.BurnInShelfNum, OrderNum = c.OrderNum, BarCodesNum = c.BarCodesNum }).ToList();
            }
            if (shelfNum != null)//如果有老化架号,根据老化架号找
            {
                foreach (var it in shelfNum)
                {
                    BurnInShelfNumList.Add(it.ToString());
                }
            }
            else//没有则找出所有老化架号
            {
                BurnInShelfNumList = BurnInmosaic.OrderBy(c => c.BurnInShelfNum).Select(c => c.BurnInShelfNum).Distinct().ToList();
            }

            var tempburn = db.Burn_in.Where(c => c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && (c.OldBarCodesNum == c.BarCodesNum || c.OldBarCodesNum == null)).Select(c => new TempBurn_in { BarCodesNum = c.BarCodesNum, Burn_in_OQCCheckAbnormal = c.Burn_in_OQCCheckAbnormal, OQCCheckBT = c.OQCCheckBT, OQCCheckFinish = c.OQCCheckFinish, OQCCheckFT = c.OQCCheckFT }).ToList();//查找老化已开始未完成的列表

            foreach (var item in BurnInShelfNumList)//循环老化架号
            {
                var OrderNumList = BurnInmosaic.Where(c => c.BurnInShelfNum == item).Select(c => c.OrderNum).Distinct().ToList();//根据老化架号查找订单号

                foreach (var OrderNum in OrderNumList)//循环订单号
                {
                    var barcodeList = BurnInmosaic.OrderBy(c => c.BarCodesNum).Where(c => c.BurnInShelfNum == item && c.OrderNum == OrderNum).Select(c => c.BarCodesNum).ToList();//找到已经排序号的条码号
                    foreach (var barcode in barcodeList)//循环条码号
                    {
                        onebarcodeInfo.Add("barcode", barcode);//条码号
                        var currentburn = tempburn.Where(c => c.BarCodesNum == barcode).FirstOrDefault();//是否在已开始为完成老化列表
                        if (currentburn == null)//不是,代表未开始老化拼屏
                        {
                            onebarcodeInfo.Add("status", 0);
                            onebarcodeInfo.Add("Abnormal", "");
                        }
                        //已经拼屏已经开始老化
                        else if (currentburn.Burn_in_OQCCheckAbnormal == null)
                        {
                            onebarcodeInfo.Add("status", 1);
                            onebarcodeInfo.Add("Abnormal", "");
                        }
                        //有异常
                        else if (currentburn.Burn_in_OQCCheckAbnormal != null)
                        {
                            onebarcodeInfo.Add("status", 2);

                            onebarcodeInfo.Add("Abnormal", currentburn.Burn_in_OQCCheckAbnormal);
                        }

                        barcodeJsonList.Add(onebarcodeInfo);

                        onebarcodeInfo = new JObject();
                    }
                    oneOrderNum.Add("barcodelist", barcodeJsonList);//条码列表
                    oneOrderNum.Add("ordernum", OrderNum);//订单号
                    OrderNumJsonList.Add(oneOrderNum);

                    barcodeJsonList = new JArray();
                    oneOrderNum = new JObject();

                }
                if (OrderNumJsonList.Count == 0)
                {
                    continue;
                }
                oneShelfNum.Add("ShelfNum", item);//老化架号
                oneShelfNum.Add("content", OrderNumJsonList);//条码内容
                ShelfNumJosnList.Add(oneShelfNum);

                OrderNumJsonList = new JArray();
                oneShelfNum = new JObject();

            }

            if (BurnInShelfNumList.Count != 0)
            {
                return com.GetModuleFromJarray(ShelfNumJosnList);
            }
            return com.GetModuleFromJarray(ShelfNumJosnList);
        }


        /// <summary>
        /// 老化拼屏历史查询
        /// </summary>
        /// 根据订单找到老化架号,循环老化架号,根据老化架号和订单找到拼屏表的条码列表.循环条码,判断条码是属于 已经开始老化/有异常/已经完成老化/没有开始,那种状态,显示给前端
        /// <param name="ordernum"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject mosaicScreen_ShelfQueryHistory([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString(); 
            JArray total = new JArray();
            JObject BurnSelf = new JObject();
            var BurnInShelfNumList = db.Burn_in_MosaicScreen.Where(c => c.OrderNum == ordernum && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)).Select(c => c.BurnInShelfNum).Distinct().ToList();//根据订单查找拼屏表里的老化架号
            foreach (var item in BurnInShelfNumList)//循环老化架号
            {

                BurnSelf.Add("ShelfNum", item);//老化架号
                var barcodeList = db.Burn_in_MosaicScreen.Where(c => c.BurnInShelfNum == item && c.OrderNum == ordernum && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)).Select(c => c.BarCodesNum).ToList();//根据订单和老化架号找到条码列表

                JArray barcodeListJarrry = new JArray();
                foreach (var barcode in barcodeList)//循环条码列表
                {
                    JObject onebarcodeInfo = new JObject();
                    onebarcodeInfo.Add("barcode", barcode);

                    //已经开始老化
                    if (db.Burn_in.Count(c => c.BarCodesNum == barcode && c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal == null && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)) > 0)//查看条码是否存在老化表,并且老化信息为已开始未结束没异常非挪用
                    {
                        onebarcodeInfo.Add("status", 1);
                        onebarcodeInfo.Add("Abnormal", "");
                    }
                    //有异常
                    else if (db.Burn_in.Count(c => c.BarCodesNum == barcode && c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal != null && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)) >= 1)//查看条码是否存在老化表,并且老化信息为已开始,未结束,有异常,非挪用
                    {
                        onebarcodeInfo.Add("status", 2);
                        var Abnormal = db.Burn_in.Where(c => c.BarCodesNum == barcode && c.OQCCheckBT != null && c.OQCCheckFT == null && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal != null && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)).Select(c => c.Burn_in_OQCCheckAbnormal).FirstOrDefault();
                        onebarcodeInfo.Add("Abnormal", Abnormal);
                    }
                    //已完成老化
                    else if (db.Burn_in.Count(c => c.BarCodesNum == barcode && c.OQCCheckBT != null && c.OQCCheckFT != null && c.OQCCheckFinish == true && (c.OldBarCodesNum == null || c.OldBarCodesNum == c.BarCodesNum)) > 0)///查看条码是否存在老化表,并且老化信息为已开始,已结束,非挪用
                    {
                        onebarcodeInfo.Add("status", 3);
                        onebarcodeInfo.Add("Abnormal", "");
                    }
                    else//没有开始,不存在老化表
                    {
                        onebarcodeInfo.Add("status", 0);
                        onebarcodeInfo.Add("Abnormal", "");
                    }
                    barcodeListJarrry.Add(onebarcodeInfo);
                }
                JObject oneOrderNum = new JObject();
                JArray ordernumJoject = new JArray();
                if (barcodeList.Count != 0)
                {

                    oneOrderNum.Add("barcodelist", barcodeListJarrry);
                    oneOrderNum.Add("ordernum", ordernum);
                }
                ordernumJoject.Add(oneOrderNum);
                BurnSelf.Add("content", ordernumJoject);
                total.Add(BurnSelf);
            }
            if (BurnInShelfNumList.Count != 0)
            {
                return com.GetModuleFromJarray(total);
            }
            return com.GetModuleFromJarray(total);

        }

        /// <summary>
        /// 检查开始拼屏FQC是否检查
        /// </summary>
        /// 循环条码列表,查找条码信息,找不到条码信息提示"未找到此条码",否则查看条码信息的订单与传过来的订单是否相符,不符提示"条码不属于此订单",否则判断老化表信息是否为空,不为空再判断是否已经完成老化或正在老化.如果老化信息为空,判断拼屏 信息是否未空,不为空提示"条码已经拼屏",为空再判断fqc信息是否为空,为空提示"FQC未检查",否则提示正常
        /// <param name="ordernum">订单号</param>
        /// <param name="barcodeList">条码列表</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject CheckFQC([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();
            JArray barcodeList =(JArray) data["ordernum"];
            JObject FQCcheckList = new JObject();
            foreach (var item in barcodeList)//循环条码列表
            {
                var barcode = item.ToString();
                var mosicScreenInfo = db.Burn_in_MosaicScreen.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).FirstOrDefault();//根据条码找到拼屏信息
                var burn_in = db.Burn_in.Where(c => c.BarCodesNum == barcode && c.OrderNum == ordernum && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).FirstOrDefault();//根据条码找到老化信息
                var barcodeinfo = db.BarCodes.Where(c => c.BarCodesNum == barcode).FirstOrDefault();//根据条码找到条码信息
                var FQCcount = db.FinalQC.Where(c => c.BarCodesNum == barcode && c.FQCCheckFinish == true && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();//根据条码找到FQC信息
                if (barcodeinfo == null)//在条码信息找不到条码
                {
                    FQCcheckList.Add(barcode, "未找到此条码");
                    continue;
                }
                if (barcodeinfo.OrderNum != ordernum)//在条码信息找到的条码订单与传过来的订单不符
                {
                    FQCcheckList.Add(barcode, "条码不属于此订单");
                    continue;
                }
                if (burn_in != null)//老化表信息不为空
                {
                    if (burn_in.OQCCheckFinish == true)//老化已完成
                    {
                        FQCcheckList.Add(barcode, "条码已经完成老化");
                        continue;
                    }
                    if (burn_in.OQCCheckFinish == false && burn_in.OQCCheckBT != null && burn_in.OQCCheckFT == null)//老化未完成,已开始
                    {
                        FQCcheckList.Add(barcode, "条码已经在老化");
                        continue;
                    }
                }
                if (mosicScreenInfo != null)//拼屏信息不为空
                {
                    FQCcheckList.Add(barcode, "条码已经拼屏");
                    continue;
                }
                if (FQCcount == 0)//fqc信息为空
                {
                    FQCcheckList.Add(barcode, "FQC未检查");
                    continue;
                }
                else
                {
                    FQCcheckList.Add(barcode, "正常");
                }
            }
            return com.GetModuleFromJobjet(FQCcheckList);
        }

        /// <summary>
        ///检查条码是否能完成拼屏
        /// </summary>
        /// 这个方法没用,没有完成拼屏这个动作
        /// <param name="ordernum">订单号</param>
        /// <param name="barcodeList">条码列表</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject CheckMosciScreenF([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();
            JArray barcodeList = (JArray)data["ordernum"];
            JObject checkList = new JObject();
            foreach (var token in barcodeList)
            {
                var item = token.ToString();
                var mosicScreenInfo = db.Burn_in_MosaicScreen.Where(c => c.BarCodesNum == item && (c.OldBarCodesNum == null || c.OldBarCodesNum == item)).FirstOrDefault();//拼屏信息
                if (mosicScreenInfo == null)//没有拼屏信息
                {
                    checkList.Add(item, "未找到此条码");
                    continue;
                }
                else
                {
                    if (mosicScreenInfo.OQCMosaicStartTime == null)//
                    {
                        checkList.Add(item, "未开始拼屏");
                        continue;
                    }
                    else if (mosicScreenInfo.OQCMosaicEndTime != null)
                    {
                        checkList.Add(item, "已完成拼屏");
                        continue;
                    }
                    else if (ordernum != mosicScreenInfo.OrderNum)
                    {
                        checkList.Add(item, "条码不属于此订单");
                        continue;
                    }
                    else
                    {
                        checkList.Add(item, "正常");
                    }
                }
            }
            return com.GetModuleFromJobjet(checkList);
        }
        
    }
}
