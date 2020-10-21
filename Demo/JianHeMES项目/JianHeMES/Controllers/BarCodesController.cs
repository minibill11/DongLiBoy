﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JianHeMES.AuthAttributes;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JianHeMES.Controllers
{
    public class BarCodesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        CommonController com = new CommonController();

        public class Sequence
        {
            public string Prefix { get; set; }

            public string Suffix { get; set; }

            public int Num { get; set; }

            public bool Rule { get; set; }

            public int startNum { get; set; }

        }

        #region --------------------类型列表

        private List<SelectListItem> SetTypeList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "请选择类型",
                    Value = ""
                },
                new SelectListItem
                {
                    Text = "模组",
                    Value = "模组"
                },
                new SelectListItem
                {
                    Text = "模块",
                    Value = "模块"
                },new SelectListItem
                {
                    Text = "电源",
                    Value = "电源"
                },new SelectListItem
                {
                    Text = "转接卡",
                    Value = "转接卡"
                },
            };
        }

        #endregion

        #region --------------------检索订单号

        private List<SelectListItem> GetOrderNumList()
        {
            var ordernum = db.BarCodes.OrderBy(m => m.OrderNum).Select(m => m.OrderNum).Distinct();

            var ordernumitems = new List<SelectListItem>();
            foreach (string major in ordernum)
            {
                ordernumitems.Add(new SelectListItem
                {
                    Text = major,
                    Value = major
                });
            }
            return ordernumitems;
        }

        public ActionResult GetOrderNumList1()
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
        private List<SelectListItem> GetnuoOrderNumList()
        {
            var newordernum = db.BarCodeRelation.Select(m => m.NewOrderNum).Distinct();
            var oldodernum = db.BarCodeRelation.Select(m => m.OldOrderNum).Distinct();
            var ordernum = newordernum.Union(oldodernum).ToList();
            var ordernumitems = new List<SelectListItem>();
            foreach (string major in ordernum)
            {
                ordernumitems.Add(new SelectListItem
                {
                    Text = major,
                    Value = major
                });
            }
            return ordernumitems;
        }

        private ActionResult GetnuoOrderNumList1()
        {
            var newordernum = db.BarCodeRelation.Select(m => m.NewOrderNum).Distinct();
            var oldodernum = db.BarCodeRelation.Select(m => m.OldOrderNum).Distinct();
            var ordernum = newordernum.Union(oldodernum).ToList();
            var ordernumitems = new List<SelectListItem>();
            JArray result = new JArray();
            foreach (string major in ordernum)
            {
                JObject List = new JObject();
                List.Add("value", major);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region --------------------检索类型

        private List<SelectListItem> GetBarCodeTypeList()
        {
            var barcodetype = db.BarCodes.OrderBy(m => m.BarCodeType).Select(m => m.BarCodeType).Distinct();

            var typeitems = new List<SelectListItem>();
            foreach (string major in barcodetype)
            {
                typeitems.Add(new SelectListItem
                {
                    Text = major,
                    Value = major
                });
            }
            return typeitems;
        }
        #endregion

        #region --------------------分页
        private static readonly int PAGE_SIZE = 20;

        private int GetPageCount(int recordCount)
        {
            int pageCount = recordCount / PAGE_SIZE;
            if (recordCount % PAGE_SIZE != 0)
            {
                pageCount += 1;
            }
            return pageCount;
        }

        private List<BarCodes> GetPagedDataSource(IQueryable<BarCodes> barcodes,
        int pageIndex, int recordCount)
        {
            var pageCount = GetPageCount(recordCount);
            if (pageIndex >= pageCount && pageCount >= 1)
            {
                pageIndex = pageCount - 1;
            }

            return barcodes.OrderBy(m => m.OrderNum)
                           .Skip(pageIndex * PAGE_SIZE)
                           .Take(PAGE_SIZE).ToList();
        }

        #endregion

        #region --------------------首页
        // GET: BarCodes
        public ActionResult Index()
        {

            ////分页
            //var barcodes = db.BarCodes as IQueryable<BarCodes>;
            //var recordCount = barcodes.Count();
            //var pageCount = GetPageCount(recordCount);

            //ViewBag.PageIndex = 0;
            //ViewBag.PageCount = pageCount;
            ////检索列表
            ViewBag.OrderNumList = GetOrderNumList();
            //ViewBag.BarCodeTypeList = GetBarCodeTypeList();
            //return View(GetPagedDataSource(barcodes, 0, recordCount));

            ViewBag.Display = "display:none";//隐藏View基本情况信息

            return View();

        }
        [HttpPost]
        public async Task<ActionResult> Index(string OrderNum, string BarCodeType, int PageIndex = 0)
        {
            JArray result = new JArray();
            //var barcodes = db.BarCodes as IQueryable<BarCodes>;
            if (String.IsNullOrEmpty(OrderNum))
            {
                return Content("无此订单号信息！");
            }
            if (db.OrderMgm.Count(c => c.OrderNum == OrderNum) == 0) return Content("无此订单号信息！");
            var barcodes = db.BarCodes.Where(c => c.OrderNum == OrderNum).Select(c => new { c.ID, c.BarCodesNum, c.OrderNum, c.ModuleGroupNum, c.BarCodeType }).OrderBy(c => c.BarCodesNum).ToList();

            if (!String.IsNullOrEmpty(BarCodeType))
            {
                barcodes = barcodes.Where(m => m.BarCodeType == BarCodeType).ToList();
            }

            //if (!String.IsNullOrEmpty(BarCodeNum))
            //{
            //    barcodes = barcodes.Where(m => m.BarCodesNum.Contains(BarCodeNum));
            //}

            //var recordCount = barcodes.Count();
            //var pageCount = GetPageCount(recordCount);
            //if (PageIndex >= pageCount && pageCount >= 1)
            //{
            //    PageIndex = pageCount - 1;
            //}

            //barcodes = barcodes.OrderBy(m => m.OrderNum).OrderBy(m=>m.BarCodesNum)
            //     .Skip(PageIndex * PAGE_SIZE).Take(PAGE_SIZE);
            //ViewBag.PageIndex = PageIndex;
            //ViewBag.PageCount = pageCount;

            //barcodes = barcodes.OrderBy(c => c.BarCodesNum).ToList();
            //ViewBag.OrderNumList = GetOrderNumList();
            //ViewBag.BarCodeTypeList = GetBarCodeTypeList();
            //return View(await barcodes.ToListAsync());
            //foreach (var item in barcodes)
            //{
            //    JObject obj = new JObject();
            //    obj.Add("ID", item.ID);
            //    obj.Add("ordernum", item.OrderNum);
            //    obj.Add("barcode", item.BarCodesNum);
            //    obj.Add("moduleNum", item.ModuleGroupNum);
            //    obj.Add("barcodeType", item.BarCodeType);
            //    result.Add(obj);
            //}
            //var aa= Content(JsonConvert.SerializeObject(barcodes));
            return Content(JsonConvert.SerializeObject(barcodes));
        }

        #endregion

        #region --------------------挪用页面
        public ActionResult DivertIndex()
        {
            ViewBag.OrderNumList = GetnuoOrderNumList();
            return View();
        }
        #endregion

        #region --------------------创建条码

        //TODO 修改为生成模组条码
        public ActionResult CreateBarCodes(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "BarCodes", act = "CreateBarCodes" + "/" + id.ToString() });
            }

            //if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || com.isCheckRole("条码管理","创建条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum) || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC打标员")
            //{
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            return View(orderMgm);

            //}
            //return RedirectToAction("Index", "BarCodes");

        }


        //创建模组、模块、电源、转接卡条码
        [HttpPost]
        public ActionResult CreateBarCodes(OrderMgm orderMgm)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "BarCodes", act = "Index" });
            }
            //if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || com.isCheckRole("条码管理", "创建全部条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum) || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC打标员")
            //{
            if (orderMgm.BarCodeCreated == 1)
            {
                return Content("<script>alert('此订单已经创建过条码，不能重复创建！');window.location.href='..';</script>");
            }

            BarCodes aBarCode = new BarCodes();
            aBarCode.OrderNum = orderMgm.OrderNum;
            aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存

            //  //批量生成模组条码
            //  List<BarCodes> barcodelist = new List<BarCodes>();
            //  for (int i = 1; i <= orderMgm.Boxes; i++)
            //  {
            //      aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
            //      aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "A" + i.ToString("00000");
            //      aBarCode.BarCodeType = "模组";
            //      aBarCode.Creator = ((Users)Session["User"]).UserName;
            //      aBarCode.CreateDate = DateTime.Now;
            //      barcodelist.Add(aBarCode);
            //  }
            //  db.BarCodes.Intersect(barcodelist);
            //  db.SaveChanges();


            //生成模组条码
            for (int i = 1; i <= orderMgm.Boxes; i++)
            {
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "A" + i.ToString("00000");
                aBarCode.BarCodeType = "模组";
                aBarCode.Creator = ((Users)Session["User"]).UserName;
                aBarCode.CreateDate = DateTime.Now;
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }
            //生成模块条码
            for (int i = 1; i <= orderMgm.Models; i++)
            {
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "B" + i.ToString("00000");
                aBarCode.BarCodeType = "模块";
                aBarCode.Creator = ((Users)Session["User"]).UserName;
                aBarCode.CreateDate = DateTime.Now;
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }
            //生成电源条码
            for (int i = 1; i <= orderMgm.Powers; i++)
            {
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "C" + i.ToString("00000");
                aBarCode.BarCodeType = "电源";
                aBarCode.Creator = ((Users)Session["User"]).UserName;
                aBarCode.CreateDate = DateTime.Now;
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }
            //生成转接卡条码
            for (int i = 1; i <= orderMgm.AdapterCard; i++)
            {
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "D" + i.ToString("00000");
                aBarCode.BarCodeType = "转接卡";
                aBarCode.Creator = ((Users)Session["User"]).UserName;
                aBarCode.CreateDate = DateTime.Now;
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }

            //修改订单的条码生成状态为1，表示已经生成.修改订单中的条码创建人
            //OrderMgm aOrder = new OrderMgm();
            IQueryable<OrderMgm> orderQuery = from m in db.OrderMgm
                                              where (m.ID == orderMgm.ID)
                                              select m;

            var aOrder = orderQuery.ToList().FirstOrDefault();
            aOrder.ID = orderMgm.ID;
            aOrder.BarCodeCreated = 1;
            aOrder.BarCodeCreateDate = DateTime.Now;
            aOrder.BarCodeCreator = ((Users)Session["User"]).UserName;
            db.SaveChanges();

            //分页
            //var barcodes = db.BarCodes.Where(m => m.OrderNum == orderMgm.OrderNum) as IQueryable<BarCodes>;
            var barcodes = db.BarCodes.Where(m => m.OrderNum == orderMgm.OrderNum).ToList();
            var recordCount = barcodes.Count();
            var pageCount = GetPageCount(recordCount);

            ViewBag.PageIndex = 0;
            ViewBag.PageCount = pageCount;

            ViewBag.OrderNumList = GetOrderNumList();
            ViewBag.BarCodeTypeList = GetBarCodeTypeList();
            //return View("Index",db.BarCodes.Where(m=>m.OrderNum== orderMgm.OrderNum).ToList());
            return View("Index", barcodes);
            //}
            //return RedirectToAction("Index", "BarCodes");
        }

        public ActionResult CreateBarCodes1(OrderMgm orderMgm)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "BarCodes", act = "Index" });
            }

            if (orderMgm.BarCodeCreated == 1)
            {
                return Content("<script>alert('此订单已经创建过条码，不能重复创建！');window.location.href='..';</script>");
            }

            BarCodes aBarCode = new BarCodes();
            aBarCode.OrderNum = orderMgm.OrderNum;
            aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存


            //生成模组条码
            for (int i = 1; i <= orderMgm.Boxes; i++)
            {
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "A" + i.ToString("00000");
                aBarCode.BarCodeType = "模组";
                aBarCode.Creator = ((Users)Session["User"]).UserName;
                aBarCode.CreateDate = DateTime.Now;
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }
            //生成模块条码
            for (int i = 1; i <= orderMgm.Models; i++)
            {
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "B" + i.ToString("00000");
                aBarCode.BarCodeType = "模块";
                aBarCode.Creator = ((Users)Session["User"]).UserName;
                aBarCode.CreateDate = DateTime.Now;
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }
            //生成电源条码
            for (int i = 1; i <= orderMgm.Powers; i++)
            {
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "C" + i.ToString("00000");
                aBarCode.BarCodeType = "电源";
                aBarCode.Creator = ((Users)Session["User"]).UserName;
                aBarCode.CreateDate = DateTime.Now;
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }
            //生成转接卡条码
            for (int i = 1; i <= orderMgm.AdapterCard; i++)
            {
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "D" + i.ToString("00000");
                aBarCode.BarCodeType = "转接卡";
                aBarCode.Creator = ((Users)Session["User"]).UserName;
                aBarCode.CreateDate = DateTime.Now;
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }

            //修改订单的条码生成状态为1，表示已经生成.修改订单中的条码创建人
            //OrderMgm aOrder = new OrderMgm();
            IQueryable<OrderMgm> orderQuery = from m in db.OrderMgm
                                              where (m.ID == orderMgm.ID)
                                              select m;

            var aOrder = orderQuery.ToList().FirstOrDefault();
            aOrder.ID = orderMgm.ID;
            aOrder.BarCodeCreated = 1;
            aOrder.BarCodeCreateDate = DateTime.Now;
            aOrder.BarCodeCreator = ((Users)Session["User"]).UserName;
            db.SaveChanges();

            var barcodes = db.BarCodes.Where(m => m.OrderNum == orderMgm.OrderNum).ToList();
            var recordCount = barcodes.Count();
            var pageCount = GetPageCount(recordCount);
            JObject result = new JObject();
            result.Add("mes", "成功");
            result.Add("pass", true);

            return Content(JsonConvert.SerializeObject(result));

        }


        //创建模组条码
        [HttpPost]
        public ContentResult CreateModuleGroupBarCodes(int? id, string username = null)
        {
            if (username == null && Session["User"] == null)
            {
                return Content("<script>alert('用户未登录，订单的模块条码创建失败！');history.go(-1);</script>");
            }
            //if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || com.isCheckRole("条码管理", "创建模组条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum) || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC打标员")
            //{
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            var count = db.BarCodes.Count(c => c.OrderNum == orderMgm.OrderNum && c.BarCodeType == "模组");
            if (orderMgm == null)
            {
                return Content("<script>alert('此订单不存在！');history.go(-1);</script>");
            }
            if (orderMgm.BarCodeCreated == 1 && count != 0)
            {
                return Content("<script>alert('此订单的模组已经创建过条码，不能重复创建！');history.go(-1);</script>");
            }
            List<BarCodes> barCodes = new List<BarCodes>();
            //生成模组条码
            for (int i = 1; i <= orderMgm.Boxes; i++)
            {
                BarCodes aBarCode = new BarCodes();
                aBarCode.OrderNum = orderMgm.OrderNum;
                aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodeType = "模组";
                aBarCode.Creator = username == null ? ((Users)Session["User"]).UserName : username;
                aBarCode.CreateDate = DateTime.Now;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "A" + i.ToString("00000");
                barCodes.Add(aBarCode);
            }
            if (com.BulkInsert<BarCodes>("BarCodes", barCodes) == "false")
            {
                return Content("<script>alert('模块创建失败，请确保表与模型相符');history.go(-1);</script>");
            }

            //修改订单的模组条码生成状态为1，表示已经生成.修改订单中的条码创建人
            orderMgm.BarCodeCreated = 1;
            orderMgm.BarCodeCreateDate = DateTime.Now;
            orderMgm.BarCodeCreator = username == null ? ((Users)Session["User"]).UserName : username;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();

            }
            return Content("<script>alert('订单的模组条码创建成功！');history.go(-1);</script>");
            //}
            //return Content("<script>alert('订单的模组条码创建失败！');history.go(-1);</script>");
        }

        //创建模块条码
        [HttpPost]
        public ActionResult CreateModulePieceBarCodes(int? id, string username = null)
        {
            if (username == null && Session["User"] == null)
            {
                return Content("<script>alert('用户未登录，订单的模块条码创建失败！');</script>");
            }
            //if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || com.isCheckRole("条码管理", "创建模块条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum) || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC打标员")
            //{
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            var count = db.BarCodes.Count(c => c.OrderNum == orderMgm.OrderNum && c.BarCodeType == "模块");
            if (orderMgm == null)
            {
                return Content("<script>alert('此订单不存在！');history.go(-1);</script>");
            }
            if (orderMgm.ModulePieceBarCodeCreated == 1 && count != 0)
            {
                return Content("<script>alert('此订单的模块已经创建过条码，不能重复创建！');history.go(-1);</script>");
            }


            List<BarCodes> barCodes = new List<BarCodes>();
            //生成模块条码
            for (int i = 1; i <= orderMgm.Models; i++)
            {
                BarCodes aBarCode = new BarCodes();
                aBarCode.OrderNum = orderMgm.OrderNum;
                aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodeType = "模块";
                aBarCode.Creator = username == null ? ((Users)Session["User"]).UserName : username;
                aBarCode.CreateDate = DateTime.Now;
                var order = orderMgm.OrderNum.Split('-');
                var temporder = order[2].PadLeft(2, '0');
                aBarCode.BarCodesNum = order[0] + order[1] + temporder + "B" + i.ToString("00000");
                barCodes.Add(aBarCode);

            }
            //Server=172.16.1.227;Database=JianHeMES;User Id=sa;Password=zdy123456;MultipleActiveResultSets=true


            if (com.BulkInsert<BarCodes>("BarCodes", barCodes) == "false")
            {
                return Content("<script>alert('模块创建失败，请确保表与模型相符');history.go(-1);</script>");
            }

            //修改订单的模块条码生成状态为1，表示已经生成.修改订单中的条码创建人
            orderMgm.ModulePieceBarCodeCreated = 1;
            orderMgm.ModulePieceBarCodeCreateDate = DateTime.Now;
            orderMgm.ModulePieceBarCodeCreator = username == null ? ((Users)Session["User"]).UserName : username;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();

            }
            return Content("<script>alert('订单的模块条码创建成功！');window.location.href='../OrderMgms/Details/" + id + "';</script>");
            //return Content("<script>alert('订单的模块条码创建失败！');history.go(-1);</script>");
            ////}

        }

        //创建电源条码
        [HttpPost]
        public ContentResult CreatePowerBarCodes(int? id, string username = null)
        {
            if (username == null && Session["User"] == null)
            {
                return Content("<script>alert('用户未登录，订单的电源条码创建失败！');history.go(-1);</script>");
            }
            //if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || com.isCheckRole("条码管理", "创建电源条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum) || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC打标员")
            //{
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            var count = db.BarCodes.Count(c => c.OrderNum == orderMgm.OrderNum && c.BarCodeType == "电源");
            if (orderMgm == null)
            {
                return Content("<script>alert('此订单不存在！');history.go(-1);</script>");
            }
            if (orderMgm.PowerBarCodeCreated == 1 && count != 0)
            {
                return Content("<script>alert('此订单的电源已经创建过条码，不能重复创建！');history.go(-1);</script>");
            }
            List<BarCodes> barCodes = new List<BarCodes>();

            //生成电源条码
            for (int i = 1; i <= orderMgm.Powers; i++)
            {
                BarCodes aBarCode = new BarCodes();
                aBarCode.OrderNum = orderMgm.OrderNum;
                aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodeType = "电源";
                aBarCode.Creator = username == null ? ((Users)Session["User"]).UserName : username;
                aBarCode.CreateDate = DateTime.Now;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "C" + i.ToString("00000");
                barCodes.Add(aBarCode);
            }
            if (com.BulkInsert<BarCodes>("BarCodes", barCodes) == "false")
            {
                return Content("<script>alert('模块创建失败，请确保表与模型相符');history.go(-1);</script>");
            }
            //修改订单的电源条码生成状态为1，表示已经生成.修改订单中的条码创建人
            orderMgm.PowerBarCodeCreated = 1;
            orderMgm.PowerBarCodeCreateDate = DateTime.Now;
            orderMgm.PowerBarCodeCreator = username == null ? ((Users)Session["User"]).UserName : username;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();

            }
            return Content("<script>alert('订单的电源条码创建成功！');window.location.href='../OrderMgms/Details/" + id + "';</script>");
            //return Content("<script>alert('订单的电源条码创建失败！');history.go(-1);</script>");
            //}

        }

        //创建转接卡条码
        [HttpPost]
        public ContentResult CreateAdapterCardBarCodes(int? id, string username = null)
        {
            if (username == null && Session["User"] == null)
            {
                return Content("<script>alert('用户未登录，订单的转接卡条码创建失败！');history.go(-1);</script>");
            }
            //if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || com.isCheckRole("条码管理", "创建转接卡条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum) || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC打标员")
            //{
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            var count = db.BarCodes.Count(c => c.OrderNum == orderMgm.OrderNum && c.BarCodeType == "转接卡");
            if (orderMgm == null)
            {
                return Content("<script>alert('此订单不存在！');history.go(-1);</script>");
            }
            if (orderMgm.AdapterCardBarCodeCreated == 1 && count != 0)
            {
                return Content("<script>alert('此订单的转接卡已经创建过条码，不能重复创建！');history.go(-1);</script>");
            }

            List<BarCodes> barCodes = new List<BarCodes>();
            //生成电源条码
            for (int i = 1; i <= orderMgm.AdapterCard; i++)
            {
                BarCodes aBarCode = new BarCodes();
                aBarCode.OrderNum = orderMgm.OrderNum;
                aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodeType = "转接卡";
                aBarCode.Creator = username == null ? ((Users)Session["User"]).UserName : username;
                aBarCode.CreateDate = DateTime.Now;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "D" + i.ToString("00000");
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }
            if (com.BulkInsert<BarCodes>("BarCodes", barCodes) == "false")
            {
                return Content("<script>alert('模块创建失败，请确保表与模型相符');history.go(-1);</script>");
            }
            //修改订单的电源条码生成状态为1，表示已经生成.修改订单中的条码创建人
            orderMgm.AdapterCardBarCodeCreated = 1;
            orderMgm.AdapterCardBarCodeCreateDate = DateTime.Now;
            orderMgm.AdapterCardBarCodeCreator = username == null ? ((Users)Session["User"]).UserName : username;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();

            }
            return Content("<script>alert('订单的转接卡条码创建成功！');window.location.href='../OrderMgms/Details/" + id + "';</script>");
            //return Content("<script>alert('订单的转接卡条码创建失败！');history.go(-1);</script>");
            //}

        }
        #endregion

        #region --------------------Details页

        // GET: BarCodes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BarCodes barCodes = db.BarCodes.Find(id);
            if (barCodes == null)
            {
                return HttpNotFound();
            }

            string BarCodeNum = barCodes.BarCodesNum;

            ViewBag.assemble = db.Assemble.Where(c => c.BoxBarCode == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            ViewBag.FinalQC = db.FinalQC.Where(c => c.BarCodesNum == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            ViewBag.burn_in = db.Burn_in.Where(c => c.BarCodesNum == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            ViewBag.calibration = db.CalibrationRecord.Where(c => c.BarCodesNum == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            ViewBag.appearance = db.Appearance.Where(c => c.BarCodesNum == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();

            return View(barCodes);
        }

        public ActionResult Details1(int? id)
        {
            JObject result = new JObject();
            if (id == null)
            {
                result.Add("mes", "没有ID信息");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));
            }
            BarCodes barCodes = db.BarCodes.Find(id);
            if (barCodes == null)
            {
                result.Add("mes", "没有找到此条码信息");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));
            }
            result.Add("OrderNum", barCodes.OrderNum);//订单号
            result.Add("BarCodesNum", barCodes.BarCodesNum);//条码号
            result.Add("BarCodeType", barCodes.BarCodeType);//类型
            result.Add("ModuleGroupNum", barCodes.ModuleGroupNum);//模组箱体号
            result.Add("Creator", barCodes.Creator);//创建人
            result.Add("CreateDate", barCodes.CreateDate);//创建时间
            result.Add("IsRepertory", barCodes.IsRepertory);//是否为库存
            result.Add("Remark", barCodes.Remark);//备注
            string BarCodeNum = barCodes.BarCodesNum;
            //PQC情况
            JArray assembleArrray = new JArray();
            var assemble = db.Assemble.Where(c => c.BoxBarCode == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            foreach (var item in assemble)
            {
                JObject obj = new JObject();
                obj.Add("PQCCheckBT", item.PQCCheckBT);//开始时间
                obj.Add("AssemblePQCPrincipal", item.AssemblePQCPrincipal);//PQC负责人
                obj.Add("PQCCheckFT", item.PQCCheckFT);//完成时间
                obj.Add("PQCCheckTime", item.PQCCheckTime);//pqc时长
                obj.Add("BarCode_Prefix", item.BarCode_Prefix);//条码前缀
                obj.Add("AssembleLineId", item.AssembleLineId);//产线
                obj.Add("PQCCheckAbnormal", item.PQCCheckAbnormal);//异常
                obj.Add("PQCRepairCondition", item.PQCRepairCondition);//维修情况
                obj.Add("PQCCheckFinish", item.PQCCheckFinish);//是否完成
                assembleArrray.Add(obj);
            }
            result.Add("PQC", assembleArrray);
            //FQC情况
            JArray fqcArrray = new JArray();
            var fqc = db.FinalQC.Where(c => c.BarCodesNum == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            foreach (var item in fqc)
            {
                JObject obj = new JObject();
                obj.Add("FQCCheckBT", item.FQCCheckBT);//开始时间
                obj.Add("FQCPrincipal", item.FQCPrincipal);//负责人
                obj.Add("FQCCheckFT", item.FQCCheckFT);//完成时间
                obj.Add("FQCCheckTimeSpan", item.FQCCheckTimeSpan);//时长
                obj.Add("FinalQC_FQCCheckAbnormal", item.FinalQC_FQCCheckAbnormal);//异常
                obj.Add("FQCCheckFinish", item.FQCCheckFinish);//是否完成
                fqcArrray.Add(obj);
            }
            result.Add("FQC", fqcArrray);
            //老化情况
            JArray burnArrray = new JArray();
            var burnin = db.Burn_in.Where(c => c.BarCodesNum == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            foreach (var item in burnin)
            {
                JObject obj = new JObject();
                obj.Add("OQCCheckBT", item.OQCCheckBT);//开始时间
                obj.Add("OQCPrincipal", item.OQCPrincipal);//负责人
                obj.Add("OQCCheckFT", item.OQCCheckFT);//完成时间
                obj.Add("OQCCheckTimeSpan", item.OQCCheckTimeSpan);//时长
                obj.Add("Burn_in_OQCCheckAbnormal", item.Burn_in_OQCCheckAbnormal);//异常
                obj.Add("RepairCondition", item.RepairCondition);//维修情况
                obj.Add("OQCCheckFinish", item.OQCCheckFinish);//是否完成
                burnArrray.Add(obj);
            }
            result.Add("Burnin", burnArrray);
            //校正情况
            JArray calibArrray = new JArray();
            var calibrationRecords = db.CalibrationRecord.Where(c => c.BarCodesNum == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            foreach (var item in calibrationRecords)
            {
                JObject obj = new JObject();
                obj.Add("BeginCalibration", item.BeginCalibration);//开始时间
                obj.Add("Operator", item.Operator);//负责人
                obj.Add("OQCCheckFT", item.FinishCalibration);//完成时间
                obj.Add("CalibrationTimeSpan", item.CalibrationTimeSpan);//时长
                obj.Add("ModuleGroupNum", item.ModuleGroupNum);//箱号
                obj.Add("AbnormalDescription", item.AbnormalDescription);//异常
                obj.Add("Normal", item.Normal);//是否完成
                calibArrray.Add(obj);
            }
            result.Add("CalibrationRecords", calibArrray);
            //包装情况
            JArray appearanceArrray = new JArray();
            var appearance = db.Appearance.Where(c => c.BarCodesNum == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            foreach (var item in appearance)
            {
                JObject obj = new JObject();
                obj.Add("OQCCheckBT", item.OQCCheckBT);//开始时间
                obj.Add("OQCPrincipal", item.OQCPrincipal);//负责人
                obj.Add("OQCCheckFinish", item.OQCCheckFinish);//完成时间
                obj.Add("OQCCheckTimeSpan", item.OQCCheckTimeSpan);//时长
                obj.Add("Appearance_OQCCheckAbnormal", item.Appearance_OQCCheckAbnormal);//异常
                obj.Add("ModuleGroupNum", item.ModuleGroupNum);//箱号
                obj.Add("RepairCondition", item.RepairCondition);//维修情况
                obj.Add("OQCCheckFinish", item.OQCCheckFinish);//是否完成
                appearanceArrray.Add(obj);
            }
            result.Add("Appearance", appearanceArrray);

            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region --------------------Create页
        // GET: BarCodes/Create
        public ActionResult Create()
        {
            ViewBag.TypeList = SetTypeList();
            return View();
        }

        // POST: BarCodes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OrderNum,ToOrderNum,BarCode_Prefix,BarCodesNum,ModuleGroupNum,BarCodeType,CreateDate,Creator,IsRepertory,Remark")] BarCodes barCodes)
        {
            if (ModelState.IsValid)
            {
                db.BarCodes.Add(barCodes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypeList = SetTypeList();
            return View(barCodes);
        }

        #endregion

        #region --------------------Edit页
        // GET: BarCodes/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (Session["User"] == null)
        //    {
        //        return RedirectToAction("Login", "Users", new { col = "BarCodes", act = "Edit" + "/" + id.ToString() });
        //    }
        //    //if (((Users)Session["User"]).Role == "ME工程师" || com.isCheckRole("条码管理", "修改条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
        //    //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BarCodes barCodes = db.BarCodes.Find(id);
        //    if (barCodes == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.TypeList = SetTypeList();
        //    ViewBag.BarCodeType = barCodes.BarCodeType;
        //    return View(barCodes);
        //    //}
        //    //return RedirectToAction("Index", "BarCodes");
        //}

        // POST: BarCodes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ID,OrderNum,ToOrderNum,BarCode_Prefix,BarCodesNum,ModuleGroupNum,BarCodeType,CreateDate,Creator,IsRepertory,Remark")] BarCodes barCodes)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        var modulelist = db.BarCodes.Where(c => c.OrderNum == barCodes.OrderNum).Select(c => c.ModuleGroupNum).ToList();
        //        if (modulelist.Contains(barCodes.ModuleGroupNum))//判断输入的模组号是否重复
        //        {
        //            var barcodeitem = db.BarCodes.Where(c => c.ModuleGroupNum == barCodes.ModuleGroupNum && c.OrderNum == barCodes.OrderNum).Select(c => c.BarCodesNum).FirstOrDefault();
        //            ModelState.AddModelError("", "模组号与条码" + barcodeitem + "重复");
        //            ViewBag.TypeList = SetTypeList();
        //            ViewBag.BarCodeType = barCodes.BarCodeType;
        //            return View(barCodes);
        //        }
        //        db.Entry(barCodes).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.TypeList = SetTypeList();
        //    ViewBag.BarCodeType = barCodes.BarCodeType;
        //    return View(barCodes);
        //}

        #endregion

        #region --------------------Delete页
        // GET: BarCodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BarCodes barCodes = db.BarCodes.Find(id);
            if (barCodes == null)
            {
                return HttpNotFound();
            }
            return View(barCodes);
        }

        // POST: BarCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BarCodes barCodes = db.BarCodes.Find(id);
            db.BarCodes.Remove(barCodes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        #region 查看挪用关联

        [HttpPost]
        public ActionResult BarcodeRelation(string orderNum = null, string barcode = null)
        {
            List<BarCodeRelation> relation = new List<BarCodeRelation>();//挪用别人列表
            List<BarCodeRelation> olderelation = new List<BarCodeRelation>();//被人挪用列表
            JArray oldtotal = new JArray();
            if (!string.IsNullOrEmpty(orderNum))
            {
                relation = db.BarCodeRelation.Where(c => c.NewOrderNum == orderNum).ToList();
                olderelation = db.BarCodeRelation.Where(c => c.OldOrderNum == orderNum).ToList();

            }
            if (!string.IsNullOrEmpty(barcode))
            {
                relation = db.BarCodeRelation.Where(c => c.NewBarCodesNum == barcode).ToList();
                olderelation = db.BarCodeRelation.Where(c => c.OldBarCodeNum == barcode).ToList();
            }
            if (relation.Count != 0 && olderelation.Count != 0)//被人挪用，也挪用别人
            {

                foreach (var newitem in relation)//循环挪用别人列表，找源头
                {
                    JArray oldebacodeitem = new JArray();
                    JObject message = new JObject();

                    oldebacodeitem = FindOld(newitem.NewBarCodesNum);
                    // oldebacodeitem.Add(newebacodeitem);
                    message.Add("ordernum", newitem.NewOrderNum);
                    message.Add("barcode", newitem.NewBarCodesNum);
                    message.Add("gongx", newitem.Procedure);
                    oldebacodeitem.Add(message);

                    JArray newebacodeitem = new JArray();
                    newebacodeitem = FindNew(newitem.NewBarCodesNum);
                    var vv = newebacodeitem.ToList();
                    vv.Reverse();
                    vv.ForEach(c => oldebacodeitem.Add(c));
                    //JArray newebacodeitem = new JArray();

                    oldtotal.Add(oldebacodeitem);
                }

                foreach (var newitem in olderelation)//循环被人挪用列表，找后续
                {
                    string message1 = JsonConvert.SerializeObject(oldtotal);
                    if (message1.Contains(newitem.OldBarCodeNum) || message1.Contains(newitem.NewBarCodesNum))
                    {
                        continue;
                    }
                    JArray newebacodeitem = new JArray();
                    newebacodeitem = FindNew(newitem.OldBarCodeNum);
                    JObject message = new JObject();
                    message.Add("ordernum", newitem.OldOrderNum);
                    message.Add("barcode", newitem.OldBarCodeNum);
                    message.Add("gongx", newitem.Procedure);
                    newebacodeitem.Add(message);
                    var vv = newebacodeitem.ToList();
                    vv.Reverse();
                    newebacodeitem = new JArray();
                    vv.ForEach(c => newebacodeitem.Add(c));
                    oldtotal.Add(newebacodeitem);
                }
            }
            if (relation.Count != 0 && olderelation.Count == 0)//只是挪用别人
            {
                foreach (var newitem in relation)
                {
                    JArray oldebacodeitem = new JArray();
                    JObject message = new JObject();

                    oldebacodeitem = FindOld(newitem.NewBarCodesNum);
                    // oldebacodeitem.Add(newebacodeitem);
                    message.Add("ordernum", newitem.NewOrderNum);
                    message.Add("barcode", newitem.NewBarCodesNum);
                    message.Add("gongx", newitem.Procedure);
                    oldebacodeitem.Add(message);
                    oldtotal.Add(oldebacodeitem);
                }
            }
            if (relation.Count == 0 && olderelation.Count != 0)//只是被人挪用
            {

                foreach (var newitem in olderelation)
                {
                    JArray newebacodeitem = new JArray();
                    newebacodeitem = FindNew(newitem.OldBarCodeNum);
                    JObject message = new JObject();
                    message.Add("ordernum", newitem.OldOrderNum);
                    message.Add("barcode", newitem.OldBarCodeNum);
                    message.Add("gongx", newitem.Procedure);
                    newebacodeitem.Add(message);
                    var vv = newebacodeitem.ToList();
                    vv.Reverse();
                    newebacodeitem = new JArray();
                    vv.ForEach(c => newebacodeitem.Add(c));
                    oldtotal.Add(newebacodeitem);
                }
            }
            return Content(JsonConvert.SerializeObject(oldtotal));
        }

        #endregion
        public JArray FindOld(string baroceod)
        {
            JArray itemjarray = new JArray();
            var haveold = db.BarCodeRelation.Where(c => c.NewBarCodesNum == baroceod).ToList();
            if (haveold.Count != 0)
            {
                //var oldbarcode = haveold.Select(c => c.OldBarCodeNum).ToList();
                foreach (var item in haveold)
                {
                    JObject message = new JObject();
                    itemjarray = FindOld(item.OldBarCodeNum);
                    message.Add("ordernum", item.OldOrderNum);
                    message.Add("barcode", item.OldBarCodeNum);
                    message.Add("gongx", item.Procedure);
                    itemjarray.Add(message);
                }
            }
            return itemjarray;
        }
        public JArray FindNew(string baroceod)
        {
            JArray itemjarray = new JArray();
            var haveold = db.BarCodeRelation.Where(c => c.OldBarCodeNum == baroceod).ToList();
            if (haveold.Count != 0)
            {
                //var oldbarcode = haveold.Select(c => c.NewBarCodesNum).ToList();
                foreach (var item in haveold)
                {
                    JObject message = new JObject();
                    itemjarray = FindNew(item.NewBarCodesNum);
                    message.Add("ordernum", item.NewOrderNum);
                    message.Add("barcode", item.NewBarCodesNum);
                    message.Add("gongx", item.Procedure);
                    itemjarray.Add(message);
                }
            }
            return itemjarray;
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

        #region -------------------条码创建后录入规则


        //条码规则创建
        public ActionResult SetJsonFile()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "BarCodes", act = "SetJsonFile" });
            }
            return View();
        }

        [HttpPost]
        public string SetJsonFile(List<Sequence> sequences, string ordernum, bool isJson = false)
        {
            List<string> number = new List<string>();
            //JObject normal = new JObject();
            var barcodeList = db.BarCodes.OrderBy(c => c.BarCodesNum).Where(c => c.OrderNum == ordernum && c.BarCodeType == "模组").ToList();
            if (barcodeList.Count == 0)
            {
                return "该订单没有创建条码号";
            }
            if (barcodeList.Count(c => c.ModuleGroupNum != null) != 0)
            {
                return "该订单条码已有模组号记录";
            }
            var cabinfo = db.CalibrationRecord.Count(c => c.OrderNum == ordernum && (c.OldOrderNum == null||c.OldOrderNum==ordernum));
            var appinfo = db.Appearance.Count(c => c.OrderNum == ordernum && (c.OldOrderNum == null || c.OldOrderNum == ordernum));
            if (cabinfo != 0 || appinfo != 0)
            {
                return cabinfo != 0 ? "校正有记录!不能创建规则" : "外观电检有记录!不能创建规则";
            }
            foreach (var item in sequences)
            {
                for (int i = 0; i < item.Num; i++)
                {
                    var serial = "";
                    if (item.Rule)
                    {
                        var num = item.Num + item.startNum - 1;
                        if (item.startNum + i < 10)
                        {
                            serial = (item.startNum + i).ToString().PadLeft(2, '0');
                        }
                        else
                        {
                            serial = (item.startNum + i).ToString().PadLeft(num.ToString().Length, '0');
                        }
                    }
                    else
                    {
                        serial = (item.startNum + i).ToString();
                    }
                    string seq = item.Prefix + serial + item.Suffix;
                    number.Add(seq);
                }
            }

            if (number.Count != barcodeList.Count)
            {
                return "录入的模组数量与条码数量不同，条码数量为" + barcodeList.Count;
            }
            if (number.GroupBy(c => c).Where(c => c.Count() > 1).ToList().Count != 0)
            {
                return "录入的有重复模组数,请确认";
            }
            else if (isJson)
            {
                JArray normal = new JArray();
                if (Directory.Exists(@"D:\MES_Data\TemDate\OrderSequence2") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\TemDate\OrderSequence2");
                }

                normal.Add(number);

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(normal, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json", output);//将数据存入json文件中

                //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "校正前创建订单号" + ordernum + "规则,选择json录入规则" };
                db.UserOperateLog.Add(log);
                db.SaveChanges();

            }
            else
            {
                for (int i = 0; i < number.Count; i++)
                {
                    barcodeList[i].ModuleGroupNum = number[i];
                    db.SaveChangesAsync();
                }
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "校正前创建订单号" + ordernum + "规则,自动分配录入条码模组" };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
            }

            return "true";
        }

        //条码规则复位
        public ActionResult ChangeRule(string ordernum)
        {
            JObject result = new JObject();
            var cabinfo = db.CalibrationRecord.Count(c => c.OrderNum == ordernum);
            var appinfo = db.Appearance.Count(c => c.OrderNum == ordernum);
            if (cabinfo != 0 || appinfo != 0)
            {
                result.Add("mes", cabinfo != 0 ? "校正有记录!不能复位" : "外观电检有记录!不能复位");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));
            }
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json") == true)
            {
                System.IO.File.Delete(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json");//删除对应的json文件
                UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName, OperateRecord = "删除校正前的条码规则,规则的录入是json模式" };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                result.Add("mes", "删除成功");
                result.Add("pass", true);
                return Content(JsonConvert.SerializeObject(result));
            }
            else
            {
                var Updatesql = "Update BarCodes SET ModuleGroupNum = NULL  where OrderNum='" + ordernum + "'";
                var mes = com.SQLAloneExecute(Updatesql);
                if (mes == "true")
                {
                    UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName, OperateRecord = "删除校正前的条码规则,规则的绑定条码录入" };
                    db.UserOperateLog.Add(log);
                    db.SaveChanges();
                    result.Add("mes", "删除成功");
                    result.Add("pass", true);
                    return Content(JsonConvert.SerializeObject(result));
                }
                else
                {
                    result.Add("mes", "删除失败,失败原因是" + mes);
                    result.Add("pass", false);
                    return Content(JsonConvert.SerializeObject(result));
                }
            }
        }

        //拿到模组号
        public ActionResult GetModuleNum(string ordernum, string statue)
        {
            var json = new JArray();
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json") == true)
            {
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json");
                json = JsonConvert.DeserializeObject<JArray>(jsonstring);//读取数据
            }
            var barcodemodule = db.BarCodes.OrderBy(c => c.ModuleGroupNum).Where(c => c.OrderNum == ordernum && c.ModuleGroupNum != null && c.BarCodeType == statue).Select(c => c.ModuleGroupNum).ToList();
            barcodemodule.ForEach(c => json.Add(c));

            return Content(JsonConvert.SerializeObject(json));
        }

        //订单输入规则前判断
        public ActionResult CheckRule(string ordenum)
        {
            JObject result = new JObject();
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence2\" + ordenum + ".json") == true)//是否有对应的json文件
            {
                result.Add("candelete", true);
                result.Add("canAdd", false);
                result.Add("mesage", "此订单已有模组号规则!");
                return Content(JsonConvert.SerializeObject(result));

            }
            var barcodemodule = db.BarCodes.Count(c => c.OrderNum == ordenum && c.ModuleGroupNum != null && c.BarCodeType == "模组");
            if (barcodemodule != 0)
            {
                result.Add("candelete", true);
                result.Add("canAdd", false);
                result.Add("mesage", "此订单条码已有模组号!");
                return Content(JsonConvert.SerializeObject(result));
            }

            result.Add("candelete", false);
            result.Add("canAdd", true);
            result.Add("mesage", "");
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region -------删除条码的模组
        public string DeleteModuleFromBarcode(string ordernum)
        {
            var barcodelist = db.BarCodes.Where(c => c.OrderNum == ordernum && c.BarCodeType == "模组").Select(c => c.BarCodesNum).ToList();
            var smtdate = db.SMT_ProductionData.Count(c => c.OrderNum == ordernum);//SMT记录信息
            var assmbles = db.Assemble.Count(c => barcodelist.Contains(c.BoxBarCode));//组装记录
            var FQC = db.FinalQC.Count(c => barcodelist.Contains(c.BarCodesNum));//FQC记录
            var calib = db.CalibrationRecord.Count(c => barcodelist.Contains(c.BarCodesNum));//校正记录
            var burn = db.Burn_in.Count(c => barcodelist.Contains(c.BarCodesNum));//老化记录
            var appearan = db.Appearance.Count(c => barcodelist.Contains(c.BarCodesNum));//外观记录记录
            if (smtdate == 0 && assmbles == 0 && FQC == 0 && calib == 0 && burn == 0 && appearan == 0)
            {
                var update = db.BarCodes.Where(c => c.OrderNum == ordernum && c.BarCodeType == "模组").ToList();
                update.ForEach(c => c.ModuleGroupNum = null);
                UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName, OperateRecord = "删除条码表的模组号,订单是" + ordernum };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                return "成功";
            }
            else
            {
                return "SMT有" + smtdate + "记录" + ",组装有" + assmbles + "记录" + ",FQC有" + FQC + "记录" + ",校正有" + calib + "记录" + ",老化有" + burn + "记录" + ",电检有" + appearan + "记录";
            }
        }
        #endregion

        #region -------------------内箱标签打印
        /// <summary>
        /// 后台绘图后打印方法
        /// </summary>
        /// <param name="pagecount"></param>
        /// <param name="barcode"></param>
        /// <param name="modulenum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsideBoxLablePrint(int pagecount, string ip = "", int port = 0, int concentration = 5, string ordernum = "")
        {
            var barcodeList = db.BarCodes.Where(c => c.OrderNum == ordernum).Select(c => new { c.BarCodesNum, c.ModuleGroupNum }).ToList();
            string total = "";
            foreach (var item in barcodeList)
            {
                //开始绘制图片
                int initialWidth = 600, initialHeight = 250;//宽2高1
                Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
                Graphics theGraphics = Graphics.FromImage(theBitmap);
                Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                        //呈现质量
                theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                //背景色
                theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));

                //引入模组号
                System.Drawing.Font myFont_modulenum;
                myFont_modulenum = new System.Drawing.Font("Microsoft YaHei UI", 80, FontStyle.Bold);//OCR-B//宋体
                StringFormat geshi = new StringFormat();
                geshi.Alignment = StringAlignment.Center; //居中
                theGraphics.DrawString(item.ModuleGroupNum, myFont_modulenum, bush, 260, 10, geshi);

                //引入条码
                Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item.BarCodesNum, 380, 30);
                double beishuhege = 0.99;
                theGraphics.DrawImage(bmp_barcode, 80, 150, (float)(bmp_barcode.Width * beishuhege), (float)(bmp_barcode.Height * beishuhege));

                //引入条码号
                System.Drawing.Font myFont_modulebarcodenum;
                myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 13, FontStyle.Regular);
                StringFormat geshi1 = new StringFormat();
                geshi1.Alignment = StringAlignment.Center; //居中
                theGraphics.DrawString(item.BarCodesNum, myFont_modulebarcodenum, bush, 270, 180, geshi);
                //结束图片绘制以上都是绘制图片的代码

                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
                Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值
                MemoryStream ms = new MemoryStream();
                theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                theBitmap.Dispose();
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO150,0^XGR:ZONE.GRF^FS^XZ";

                //string ip = "172.16.99.240";//打印机IP地址
                //int port = 9101;//打印机端口
                string result = IPPrinttest(data.ToString(), pagecount, ip, port);
                if (result != "打印成功！")
                {
                    total = total + item.BarCodesNum + "条码打印失败,";
                }
                //string ptrn = "InsideBoxLablePrinter";
                //bool resut = false;
                //if (pagecount == 1)
                //{
                //    if (BarCodeLablePrint.SendStringToPrinter(ptrn, data.ToString())) return Content("打印成功");
                //}
                //if (pagecount == 2)
                //{
                //    resut = BarCodeLablePrint.SendStringToPrinter(ptrn, data.ToString());
                //    resut = BarCodeLablePrint.SendStringToPrinter(ptrn, data.ToString());
                //    return Content("打印成功");
                //}
            }
            if (string.IsNullOrEmpty(total))
            {
                return Content("打印成功！");
            }
            else
            {
                return Content(total + "请检查打印机是否断网或未开机");
            }

            //return File(ms.ToArray(), "image/Png");
        }

        #endregion

        /// <summary>
        /// 批量打印模组和条码方法
        /// </summary>
        /// <param name="pagecount">打印份数</param>
        /// <param name="barcode">条码列表</param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="concentration"></param>
        /// <param name="testswitch"></param>
        /// <returns></returns>
        public ActionResult InsideListPrint(int pagecount, List<string> barcode, string ip = "", int port = 0, int concentration = 5, bool testswitch = false, int right = 0, int down = 0)
        {
            //开始绘制图片
            int initialWidth = 600, initialHeight = 250;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
            //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));

            //引入模组号
            StringFormat geshi = new StringFormat();
            geshi.Alignment = StringAlignment.Center; //居中
            int count = 0;
            int i = 1;
            for (var j = 0; j < pagecount; j++)
            {
                foreach (var item in barcode)
                {
                    var modulenum = db.BarCodes.Where(c => c.BarCodesNum == item).Select(c => c.ModuleGroupNum).FirstOrDefault();
                    if (modulenum.Length > 9)
                    {
                        System.Drawing.Font myFont_modulenum;
                        myFont_modulenum = new System.Drawing.Font("Microsoft YaHei UI", 40, FontStyle.Bold);//OCR-B//宋体
                        theGraphics.DrawString(modulenum, myFont_modulenum, bush, 260, 20, geshi);
                    }
                    else
                    {
                        System.Drawing.Font myFont_modulenum;
                        myFont_modulenum = new System.Drawing.Font("Microsoft YaHei UI", 55, FontStyle.Bold);//OCR-B//宋体
                        theGraphics.DrawString(modulenum, myFont_modulenum, bush, 260, 20, geshi);
                    }

                    //引入条码
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 380, 30);
                    //double beishuhege = 0.99;
                    theGraphics.DrawImage(bmp_barcode, 70, 150, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //引入条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 13, FontStyle.Regular);
                    StringFormat geshi1 = new StringFormat();
                    geshi1.Alignment = StringAlignment.Center; //居中
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, 260, 180, geshi);
                    //结束图片绘制以上都是绘制图片的代码

                    string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
                    Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值
                                                                                                                  //图片旋转180度
                                                                                                                  //bm.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                                                                                                  //string ip = "172.16.99.240";//打印机IP地址
                                                                                                                  //int port = 9101;//打印机端口

                    if (testswitch == true)
                    {
                        MemoryStream ms = new MemoryStream();
                        //图片旋转180度
                        //theBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        theBitmap.Dispose();
                        return File(ms.ToArray(), "image/Png");
                    }
                    else
                    {
                        int totalbytes = bm.ToString().Length;
                        int rowbytes = 10;
                        string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                        data += totalbytes + "," + rowbytes + "," + hex;
                        int x = 0 + right;
                        int y = 0 + down;
                        data += "^LH" + x + "," + y + "^FO150,0^XGR:ZONE.GRF^FS^XZ";
                        IPPrinttest(data.ToString(), 1, ip, port);
                        count++;
                        var print = db.BarCodes.Where(c => c.BarCodesNum == item).FirstOrDefault();
                        print.BarcodePrintCount++;
                        print.ModuleNumPrintCount++;
                        db.SaveChanges();
                    }
                    theBitmap = new Bitmap(initialWidth, initialHeight);
                    theGraphics = Graphics.FromImage(theBitmap);
                    bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                      //呈现质量
                    theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    //背景色
                    theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
                    if (i % 100 == 0)
                    {
                        Thread.CurrentThread.Join(new TimeSpan(0, 0, 2));
                    }
                    i++;
                    Thread.CurrentThread.Join(new TimeSpan(0, 0, 0, 0, 5));
                }
            }
            return Content(count + "个打印成功!" + (barcode.Count - count > 0 ? barcode.Count - count + "个打印失败！" : ""));
        }

        public ActionResult InsideListPrintNotModuleNum(int pagecount, List<string> barcode, string ip = "", int port = 0, int concentration = 5, bool testswitch = false, int right = 0, int down = 0)
        {
            //开始绘制图片
            int initialWidth = 380, initialHeight = 125;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
            //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));

            //引入模组号
            StringFormat geshi = new StringFormat();
            geshi.Alignment = StringAlignment.Center; //居中
            int count = 0;
            int i = 1;
            for (var j = 0; j < pagecount; j++)
            {
                foreach (var item in barcode)
                {
                    //引入条码
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 300, 58);
                    //double beishuhege = 1.2;
                    theGraphics.DrawImage(bmp_barcode, 25, 20, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //引入条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Arial", 18, FontStyle.Regular);
                    //StringFormat geshi1 = new StringFormat();
                    //geshi1.Alignment = StringAlignment.Center; //居中
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, 175, 75, geshi);
                    //结束图片绘制以上都是绘制图片的代码

                    string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
                    Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值
                                                                                                                  //图片旋转180度
                                                                                                                  //bm.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                                                                                                  //string ip = "172.16.99.240";//打印机IP地址
                                                                                                                  //int port = 9101;//打印机端口

                    if (testswitch == true)
                    {
                        MemoryStream ms = new MemoryStream();
                        //图片旋转180度
                        //theBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        theBitmap.Dispose();
                        return File(ms.ToArray(), "image/Png");
                    }
                    else
                    {
                        int totalbytes = bm.ToString().Length;
                        int rowbytes = 10;
                        string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                        data += totalbytes + "," + rowbytes + "," + hex;
                        int x = 85 + right;
                        int y = 0 + down;
                        data += "^LH" + x + "," + y + "^FO150,0^XGR:ZONE.GRF^FS^XZ";
                        IPPrinttest(data.ToString(), 1, ip, port);
                        count++;
                        var print = db.BarCodes.Where(c => c.BarCodesNum == item).FirstOrDefault();
                        print.BarcodePrintCount++;
                        db.SaveChanges();
                    }
                    theBitmap = new Bitmap(initialWidth, initialHeight);
                    theGraphics = Graphics.FromImage(theBitmap);
                    bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                      //呈现质量
                    theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    //背景色
                    theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
                    if (i % 100 == 0)
                    {
                        Thread.CurrentThread.Join(new TimeSpan(0, 0, 2));
                    }
                    i++;
                    Thread.CurrentThread.Join(new TimeSpan(0, 0, 0, 0, 5));
                }
            }
            return Content(count + "个打印成功!" + (barcode.Count - count > 0 ? barcode.Count - count + "个打印失败！" : ""));
        }

        public ActionResult InsideListPrintNotBarcode(int pagecount, List<string> ModuleNum, string Ordernum, string ip = "", int port = 0, int concentration = 5, bool testswitch = false, int right = 0, int down = 0)
        {
            //开始绘制图片
            int initialWidth = 380, initialHeight = 125;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
            //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));

            //引入模组号
            StringFormat geshi = new StringFormat();
            geshi.Alignment = StringAlignment.Center; //居中
            int count = 0;
            int i = 1;
            for (var j = 0; j < pagecount; j++)
            {
                foreach (var item in ModuleNum)
                {
                    if (item.Length <= 3)
                    {
                        Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 150, 40);
                        //double beishuhege = 1.2;
                        theGraphics.DrawImage(bmp_barcode, 100, 20, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    }
                    if (item.Length <= 5 || item.Length >= 4)
                    {
                        Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 180, 40);
                        //double beishuhege = 1.2;
                        theGraphics.DrawImage(bmp_barcode, 80, 20, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    }
                    else
                    {
                        //引入条码
                        Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 280, 40);
                        //double beishuhege = 1.2;
                        theGraphics.DrawImage(bmp_barcode, 25, 20, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    }

                    //引入条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Arial", 40, FontStyle.Regular);
                    //StringFormat geshi1 = new StringFormat();
                    //geshi1.Alignment = StringAlignment.Center; //居中
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, 175, 55, geshi);
                    //结束图片绘制以上都是绘制图片的代码
                    string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
                    Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值
                                                                                                                  //图片旋转180度
                                                                                                                  //bm.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                                                                                                  //string ip = "172.16.99.240";//打印机IP地址
                                                                                                                  //int port = 9101;//打印机端口

                    if (testswitch == true)
                    {
                        MemoryStream ms = new MemoryStream();
                        //图片旋转180度
                        //theBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        theBitmap.Dispose();
                        return File(ms.ToArray(), "image/Png");
                    }
                    else
                    {
                        int totalbytes = bm.ToString().Length;
                        int rowbytes = 10;
                        string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                        data += totalbytes + "," + rowbytes + "," + hex;
                        int x = 85 + right;
                        int y = 0 + down;
                        data += "^LH" + x + "," + y + "^FO150,0^XGR:ZONE.GRF^FS^XZ";
                        IPPrinttest(data.ToString(), 1, ip, port);
                        count++;
                        var print = db.BarCodes.Where(c => c.OrderNum == Ordernum && c.ModuleGroupNum == item && c.BarCodeType == "模组").FirstOrDefault();
                        if (print != null)
                        {
                            print.ModuleNumPrintCount++;
                            db.SaveChanges();
                        }
                    }
                    theBitmap = new Bitmap(initialWidth, initialHeight);
                    theGraphics = Graphics.FromImage(theBitmap);
                    bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                      //呈现质量
                    theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    //背景色
                    theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
                    if (i % 100 == 0)
                    {
                        Thread.CurrentThread.Join(new TimeSpan(0, 0, 2));
                    }
                    i++;
                    Thread.CurrentThread.Join(new TimeSpan(0, 0, 0, 0, 5));
                }
            }
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence2\" + Ordernum + ".json") == true)
            {
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + Ordernum + ".json");
                var json = JsonConvert.DeserializeObject<JArray>(jsonstring);//读取数据
                json.Add("已打印");
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + Ordernum + ".json", output);//将数据存入json文件中
            }
            return Content(count + "个打印成功!" + (ModuleNum.Count - count > 0 ? ModuleNum.Count - count + "个打印失败！" : ""));
        }

        public static string IPPrinttest(string data = "", int pagecount = 1, string ip = "", int port = 0)
        {
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(ip, port);
                StreamWriter writer = new StreamWriter(client.GetStream());
                for (int i = 1; i < pagecount + 1; i++)
                {
                    writer.Write(data);
                }
                writer.Flush();
                writer.Close();
                client.Close();
                return "打印成功！";
            }
            catch
            {
                return "打印连接失败,请检查打印机是否断网或未开机！";
            }
        }
    }

    public class BarCodesApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        CommonController com = new CommonController();

        public class Sequence
        {
            public string Prefix { get; set; }

            public string Suffix { get; set; }

            public int Num { get; set; }

            public bool Rule { get; set; }

            public int startNum { get; set; }

        }

        #region --------------------检索订单号

        [HttpPost]
        [ApiAuthorize]
        public string GetOrderNumList1()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return JsonConvert.SerializeObject(result);
        }
        [HttpPost]
        [ApiAuthorize]
        private string GetnuoOrderNumList1()
        {
            var newordernum = db.BarCodeRelation.Select(m => m.NewOrderNum).Distinct();
            var oldodernum = db.BarCodeRelation.Select(m => m.OldOrderNum).Distinct();
            var ordernum = newordernum.Union(oldodernum).ToList();
            var ordernumitems = new List<SelectListItem>();
            JArray result = new JArray();
            foreach (string major in ordernum)
            {
                JObject List = new JObject();
                List.Add("value", major);

                result.Add(List);
            }
            return JsonConvert.SerializeObject(result);
        }
        #endregion

        #region --------------------检索类型
        [HttpPost]
        [ApiAuthorize]
        private List<SelectListItem> GetBarCodeTypeList()
        {
            var barcodetype = db.BarCodes.OrderBy(m => m.BarCodeType).Select(m => m.BarCodeType).Distinct();

            var typeitems = new List<SelectListItem>();
            foreach (string major in barcodetype)
            {
                typeitems.Add(new SelectListItem
                {
                    Text = major,
                    Value = major
                });
            }
            return typeitems;
        }
        #endregion

        #region --------------------分页

        private static readonly int PAGE_SIZE = 20;

        [HttpPost]
        [ApiAuthorize]
        private int GetPageCount(int recordCount)
        {
            int pageCount = recordCount / PAGE_SIZE;
            if (recordCount % PAGE_SIZE != 0)
            {
                pageCount += 1;
            }
            return pageCount;
        }
        [HttpPost]
        [ApiAuthorize]
        private List<BarCodes> GetPagedDataSource(IQueryable<BarCodes> barcodes,
        int pageIndex, int recordCount)
        {
            var pageCount = GetPageCount(recordCount);
            if (pageIndex >= pageCount && pageCount >= 1)
            {
                pageIndex = pageCount - 1;
            }

            return barcodes.OrderBy(m => m.OrderNum)
                           .Skip(pageIndex * PAGE_SIZE)
                           .Take(PAGE_SIZE).ToList();
        }

        #endregion

        #region --------------------首页

        [HttpPost]
        [ApiAuthorize]
        public string Index(string OrderNum, string BarCodeType, int PageIndex = 0)
        {
            JArray result = new JArray();
            //var barcodes = db.BarCodes as IQueryable<BarCodes>;
            if (String.IsNullOrEmpty(OrderNum))
            {
                return "无此订单号信息！";
            }
            if (db.OrderMgm.Count(c => c.OrderNum == OrderNum) == 0) return "无此订单号信息！";
            var barcodes = db.BarCodes.Where(c => c.OrderNum == OrderNum).Select(c => new { c.ID, c.BarCodesNum, c.OrderNum, c.ModuleGroupNum, c.BarCodeType }).OrderBy(c => c.BarCodesNum).ToList();

            if (!String.IsNullOrEmpty(BarCodeType))
            {
                barcodes = barcodes.Where(m => m.BarCodeType == BarCodeType).ToList();
            }

            return JsonConvert.SerializeObject(barcodes);
        }

        #endregion

        #region --------------------创建条码

        [HttpPost]
        [ApiAuthorize]
        //创建模组、模块、电源、转接卡条码
        public string CreateBarCodes1(OrderMgm orderMgm)
        {

            if (orderMgm.BarCodeCreated == 1)
            {
                return "<script>alert('此订单已经创建过条码，不能重复创建！');window.location.href='..';</script>";
            }

            BarCodes aBarCode = new BarCodes();
            aBarCode.OrderNum = orderMgm.OrderNum;
            aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存


            //生成模组条码
            for (int i = 1; i <= orderMgm.Boxes; i++)
            {
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "A" + i.ToString("00000");
                aBarCode.BarCodeType = "模组";
                //aBarCode.Creator = ((Users)Session["User"]).UserName;
                aBarCode.CreateDate = DateTime.Now;
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }
            //生成模块条码
            for (int i = 1; i <= orderMgm.Models; i++)
            {
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "B" + i.ToString("00000");
                aBarCode.BarCodeType = "模块";
                //aBarCode.Creator = ((Users)Session["User"]).UserName;
                aBarCode.CreateDate = DateTime.Now;
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }
            //生成电源条码
            for (int i = 1; i <= orderMgm.Powers; i++)
            {
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "C" + i.ToString("00000");
                aBarCode.BarCodeType = "电源";
                //aBarCode.Creator = ((Users)Session["User"]).UserName;
                aBarCode.CreateDate = DateTime.Now;
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }
            //生成转接卡条码
            for (int i = 1; i <= orderMgm.AdapterCard; i++)
            {
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "D" + i.ToString("00000");
                aBarCode.BarCodeType = "转接卡";
                //aBarCode.Creator = ((Users)Session["User"]).UserName;
                aBarCode.CreateDate = DateTime.Now;
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }

            //修改订单的条码生成状态为1，表示已经生成.修改订单中的条码创建人
            //OrderMgm aOrder = new OrderMgm();
            IQueryable<OrderMgm> orderQuery = from m in db.OrderMgm
                                              where (m.ID == orderMgm.ID)
                                              select m;

            var aOrder = orderQuery.ToList().FirstOrDefault();
            aOrder.ID = orderMgm.ID;
            aOrder.BarCodeCreated = 1;
            aOrder.BarCodeCreateDate = DateTime.Now;
            //aOrder.BarCodeCreator = ((Users)Session["User"]).UserName;
            db.SaveChanges();

            var barcodes = db.BarCodes.Where(m => m.OrderNum == orderMgm.OrderNum).ToList();
            var recordCount = barcodes.Count();
            var pageCount = GetPageCount(recordCount);
            JObject result = new JObject();
            result.Add("mes", "成功");
            result.Add("pass", true);

            return JsonConvert.SerializeObject(result);

        }


        //创建模组条码
        [HttpPost]
        [ApiAuthorize]
        public string CreateModuleGroupBarCodes(int? id, string username = null)
        {
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            var count = db.BarCodes.Count(c => c.OrderNum == orderMgm.OrderNum && c.BarCodeType == "模组");
            if (orderMgm == null)
            {
                return "<script>alert('此订单不存在！');history.go(-1);</script>";
            }
            if (orderMgm.BarCodeCreated == 1 && count != 0)
            {
                return "<script>alert('此订单的模组已经创建过条码，不能重复创建！');history.go(-1);</script>";
            }
            List<BarCodes> barCodes = new List<BarCodes>();
            //生成模组条码
            for (int i = 1; i <= orderMgm.Boxes; i++)
            {
                BarCodes aBarCode = new BarCodes();
                aBarCode.OrderNum = orderMgm.OrderNum;
                aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodeType = "模组";
                // aBarCode.Creator = username == null ? ((Users)Session["User"]).UserName : username;
                aBarCode.CreateDate = DateTime.Now;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "A" + i.ToString("00000");
                barCodes.Add(aBarCode);
            }
            if (com.BulkInsert<BarCodes>("BarCodes", barCodes) == "false")
            {
                return "<script>alert('模块创建失败，请确保表与模型相符');history.go(-1);</script>";
            }

            //修改订单的模组条码生成状态为1，表示已经生成.修改订单中的条码创建人
            orderMgm.BarCodeCreated = 1;
            orderMgm.BarCodeCreateDate = DateTime.Now;
            //orderMgm.BarCodeCreator = username == null ? ((Users)Session["User"]).UserName : username;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();

            }
            return "<script>alert('订单的模组条码创建成功！');history.go(-1);</script>";
            //}
            //return Content("<script>alert('订单的模组条码创建失败！');history.go(-1);</script>");
        }

        //创建模块条码
        [HttpPost]
        [ApiAuthorize]
        public string CreateModulePieceBarCodes(int? id, string username = null)
        {
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            var count = db.BarCodes.Count(c => c.OrderNum == orderMgm.OrderNum && c.BarCodeType == "模块");
            if (orderMgm == null)
            {
                return "<script>alert('此订单不存在！');history.go(-1);</script>";
            }
            if (orderMgm.ModulePieceBarCodeCreated == 1 && count != 0)
            {
                return "<script>alert('此订单的模块已经创建过条码，不能重复创建！');history.go(-1);</script>";
            }


            List<BarCodes> barCodes = new List<BarCodes>();
            //生成模块条码
            for (int i = 1; i <= orderMgm.Models; i++)
            {
                BarCodes aBarCode = new BarCodes();
                aBarCode.OrderNum = orderMgm.OrderNum;
                aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodeType = "模块";
                //aBarCode.Creator = username == null ? ((Users)Session["User"]).UserName : username;
                aBarCode.CreateDate = DateTime.Now;
                var order = orderMgm.OrderNum.Split('-');
                var temporder = order[2].PadLeft(2, '0');
                aBarCode.BarCodesNum = order[0] + order[1] + temporder + "B" + i.ToString("00000");
                barCodes.Add(aBarCode);

            }
            //Server=172.16.1.227;Database=JianHeMES;User Id=sa;Password=zdy123456;MultipleActiveResultSets=true


            if (com.BulkInsert<BarCodes>("BarCodes", barCodes) == "false")
            {
                return "<script>alert('模块创建失败，请确保表与模型相符');history.go(-1);</script>";
            }

            //修改订单的模块条码生成状态为1，表示已经生成.修改订单中的条码创建人
            orderMgm.ModulePieceBarCodeCreated = 1;
            orderMgm.ModulePieceBarCodeCreateDate = DateTime.Now;
            //orderMgm.ModulePieceBarCodeCreator = username == null ? ((Users)Session["User"]).UserName : username;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();

            }
            return "<script>alert('订单的模块条码创建成功！');window.location.href='../OrderMgms/Details/" + id + "';</script>";
            //return Content("<script>alert('订单的模块条码创建失败！');history.go(-1);</script>");
            ////}

        }

        //创建电源条码
        [HttpPost]
        [ApiAuthorize]
        public string CreatePowerBarCodes(int? id, string username = null)
        {

            OrderMgm orderMgm = db.OrderMgm.Find(id);
            var count = db.BarCodes.Count(c => c.OrderNum == orderMgm.OrderNum && c.BarCodeType == "电源");
            if (orderMgm == null)
            {
                return "<script>alert('此订单不存在！');history.go(-1);</script>";
            }
            if (orderMgm.PowerBarCodeCreated == 1 && count != 0)
            {
                return "<script>alert('此订单的电源已经创建过条码，不能重复创建！');history.go(-1);</script>";
            }
            List<BarCodes> barCodes = new List<BarCodes>();

            //生成电源条码
            for (int i = 1; i <= orderMgm.Powers; i++)
            {
                BarCodes aBarCode = new BarCodes();
                aBarCode.OrderNum = orderMgm.OrderNum;
                aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodeType = "电源";
                //aBarCode.Creator = username == null ? ((Users)Session["User"]).UserName : username;
                aBarCode.CreateDate = DateTime.Now;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "C" + i.ToString("00000");
                barCodes.Add(aBarCode);
            }
            if (com.BulkInsert<BarCodes>("BarCodes", barCodes) == "false")
            {
                return "<script>alert('模块创建失败，请确保表与模型相符');history.go(-1);</script>";
            }
            //修改订单的电源条码生成状态为1，表示已经生成.修改订单中的条码创建人
            orderMgm.PowerBarCodeCreated = 1;
            orderMgm.PowerBarCodeCreateDate = DateTime.Now;
            // orderMgm.PowerBarCodeCreator = username == null ? ((Users)Session["User"]).UserName : username;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();

            }
            return "<script>alert('订单的电源条码创建成功！');window.location.href='../OrderMgms/Details/" + id + "';</script>";
            //return Content("<script>alert('订单的电源条码创建失败！');history.go(-1);</script>");
            //}

        }

        //创建转接卡条码
        [HttpPost]
        [ApiAuthorize]
        public string CreateAdapterCardBarCodes(int? id, string username = null)
        {
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            var count = db.BarCodes.Count(c => c.OrderNum == orderMgm.OrderNum && c.BarCodeType == "转接卡");
            if (orderMgm == null)
            {
                return "<script>alert('此订单不存在！');history.go(-1);</script>";
            }
            if (orderMgm.AdapterCardBarCodeCreated == 1 && count != 0)
            {
                return "<script>alert('此订单的转接卡已经创建过条码，不能重复创建！');history.go(-1);</script>";
            }

            List<BarCodes> barCodes = new List<BarCodes>();
            //生成电源条码
            for (int i = 1; i <= orderMgm.AdapterCard; i++)
            {
                BarCodes aBarCode = new BarCodes();
                aBarCode.OrderNum = orderMgm.OrderNum;
                aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
                aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
                aBarCode.BarCodeType = "转接卡";
                //aBarCode.Creator = username == null ? ((Users)Session["User"]).UserName : username;
                aBarCode.CreateDate = DateTime.Now;
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "D" + i.ToString("00000");
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }
            if (com.BulkInsert<BarCodes>("BarCodes", barCodes) == "false")
            {
                return "<script>alert('模块创建失败，请确保表与模型相符');history.go(-1);</script>";
            }
            //修改订单的电源条码生成状态为1，表示已经生成.修改订单中的条码创建人
            orderMgm.AdapterCardBarCodeCreated = 1;
            orderMgm.AdapterCardBarCodeCreateDate = DateTime.Now;
            //orderMgm.AdapterCardBarCodeCreator = username == null ? ((Users)Session["User"]).UserName : username;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();

            }
            return "<script>alert('订单的转接卡条码创建成功！');window.location.href='../OrderMgms/Details/" + id + "';</script>";
            //return Content("<script>alert('订单的转接卡条码创建失败！');history.go(-1);</script>");
            //}

        }
        #endregion

        #region --------------------Details页
        [HttpPost]
        [ApiAuthorize]
        public string Details(int? id)
        {
            JObject result = new JObject();
            if (id == null)
            {
                result.Add("mes", "没有ID信息");
                result.Add("pass", false);
                return JsonConvert.SerializeObject(result);
            }
            BarCodes barCodes = db.BarCodes.Find(id);
            if (barCodes == null)
            {
                result.Add("mes", "没有找到此条码信息");
                result.Add("pass", false);
                return JsonConvert.SerializeObject(result);
            }
            result.Add("OrderNum", barCodes.OrderNum);//订单号
            result.Add("BarCodesNum", barCodes.BarCodesNum);//条码号
            result.Add("BarCodeType", barCodes.BarCodeType);//类型
            result.Add("ModuleGroupNum", barCodes.ModuleGroupNum);//模组箱体号
            result.Add("Creator", barCodes.Creator);//创建人
            result.Add("CreateDate", barCodes.CreateDate);//创建时间
            result.Add("IsRepertory", barCodes.IsRepertory);//是否为库存
            result.Add("Remark", barCodes.Remark);//备注
            string BarCodeNum = barCodes.BarCodesNum;
            //PQC情况
            JArray assembleArrray = new JArray();
            var assemble = db.Assemble.Where(c => c.BoxBarCode == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            foreach (var item in assemble)
            {
                JObject obj = new JObject();
                obj.Add("PQCCheckBT", item.PQCCheckBT);//开始时间
                obj.Add("AssemblePQCPrincipal", item.AssemblePQCPrincipal);//PQC负责人
                obj.Add("PQCCheckFT", item.PQCCheckFT);//完成时间
                obj.Add("PQCCheckTime", item.PQCCheckTime);//pqc时长
                obj.Add("BarCode_Prefix", item.BarCode_Prefix);//条码前缀
                obj.Add("AssembleLineId", item.AssembleLineId);//产线
                obj.Add("PQCCheckAbnormal", item.PQCCheckAbnormal);//异常
                obj.Add("PQCRepairCondition", item.PQCRepairCondition);//维修情况
                obj.Add("PQCCheckFinish", item.PQCCheckFinish);//是否完成
                assembleArrray.Add(obj);
            }
            result.Add("PQC", assembleArrray);
            //FQC情况
            JArray fqcArrray = new JArray();
            var fqc = db.FinalQC.Where(c => c.BarCodesNum == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            foreach (var item in fqc)
            {
                JObject obj = new JObject();
                obj.Add("FQCCheckBT", item.FQCCheckBT);//开始时间
                obj.Add("FQCPrincipal", item.FQCPrincipal);//负责人
                obj.Add("FQCCheckFT", item.FQCCheckFT);//完成时间
                obj.Add("FQCCheckTimeSpan", item.FQCCheckTimeSpan);//时长
                obj.Add("FinalQC_FQCCheckAbnormal", item.FinalQC_FQCCheckAbnormal);//异常
                obj.Add("FQCCheckFinish", item.FQCCheckFinish);//是否完成
                fqcArrray.Add(obj);
            }
            result.Add("FQC", fqcArrray);
            //老化情况
            JArray burnArrray = new JArray();
            var burnin = db.Burn_in.Where(c => c.BarCodesNum == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            foreach (var item in burnin)
            {
                JObject obj = new JObject();
                obj.Add("OQCCheckBT", item.OQCCheckBT);//开始时间
                obj.Add("OQCPrincipal", item.OQCPrincipal);//负责人
                obj.Add("OQCCheckFT", item.OQCCheckFT);//完成时间
                obj.Add("OQCCheckTimeSpan", item.OQCCheckTimeSpan);//时长
                obj.Add("Burn_in_OQCCheckAbnormal", item.Burn_in_OQCCheckAbnormal);//异常
                obj.Add("RepairCondition", item.RepairCondition);//维修情况
                obj.Add("OQCCheckFinish", item.OQCCheckFinish);//是否完成
                burnArrray.Add(obj);
            }
            result.Add("Burnin", burnArrray);
            //校正情况
            JArray calibArrray = new JArray();
            var calibrationRecords = db.CalibrationRecord.Where(c => c.BarCodesNum == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            foreach (var item in calibrationRecords)
            {
                JObject obj = new JObject();
                obj.Add("BeginCalibration", item.BeginCalibration);//开始时间
                obj.Add("Operator", item.Operator);//负责人
                obj.Add("OQCCheckFT", item.FinishCalibration);//完成时间
                obj.Add("CalibrationTimeSpan", item.CalibrationTimeSpan);//时长
                obj.Add("ModuleGroupNum", item.ModuleGroupNum);//箱号
                obj.Add("AbnormalDescription", item.AbnormalDescription);//异常
                obj.Add("Normal", item.Normal);//是否完成
                calibArrray.Add(obj);
            }
            result.Add("CalibrationRecords", calibArrray);
            //包装情况
            JArray appearanceArrray = new JArray();
            var appearance = db.Appearance.Where(c => c.BarCodesNum == BarCodeNum && (c.OldBarCodesNum == null || c.OldBarCodesNum == BarCodeNum)).ToList();
            foreach (var item in appearance)
            {
                JObject obj = new JObject();
                obj.Add("OQCCheckBT", item.OQCCheckBT);//开始时间
                obj.Add("OQCPrincipal", item.OQCPrincipal);//负责人
                obj.Add("OQCCheckFinish", item.OQCCheckFinish);//完成时间
                obj.Add("OQCCheckTimeSpan", item.OQCCheckTimeSpan);//时长
                obj.Add("Appearance_OQCCheckAbnormal", item.Appearance_OQCCheckAbnormal);//异常
                obj.Add("ModuleGroupNum", item.ModuleGroupNum);//箱号
                obj.Add("RepairCondition", item.RepairCondition);//维修情况
                obj.Add("OQCCheckFinish", item.OQCCheckFinish);//是否完成
                appearanceArrray.Add(obj);
            }
            result.Add("Appearance", appearanceArrray);

            return JsonConvert.SerializeObject(result);
        }
        #endregion

        #region -------------------条码创建后录入规则


        //条码规则创建
        [HttpPost]
        [ApiAuthorize]
        public string SetJsonFile(List<Sequence> sequences, string ordernum, string UserName, bool isJson = false)
        {
            List<string> number = new List<string>();
            //JObject normal = new JObject();
            var barcodeList = db.BarCodes.OrderBy(c => c.BarCodesNum).Where(c => c.OrderNum == ordernum && c.BarCodeType == "模组").ToList();
            if (barcodeList.Count == 0)
            {
                return "该订单没有创建条码号";
            }
            if (barcodeList.Count(c => c.ModuleGroupNum != null) != 0)
            {
                return "该订单条码已有模组号记录";
            }
            var cabinfo = db.CalibrationRecord.Count(c => c.OrderNum == ordernum);
            var appinfo = db.Appearance.Count(c => c.OrderNum == ordernum);
            if (cabinfo != 0 || appinfo != 0)
            {
                return cabinfo != 0 ? "校正有记录!不能创建规则" : "外观电检有记录!不能创建规则";
            }
            foreach (var item in sequences)
            {
                for (int i = 0; i < item.Num; i++)
                {
                    var serial = "";
                    if (item.Rule)
                    {
                        var num = item.Num + item.startNum - 1;
                        if (item.startNum + i < 10)
                        {
                            serial = (item.startNum + i).ToString().PadLeft(2, '0');
                        }
                        else
                        {
                            serial = (item.startNum + i).ToString().PadLeft(num.ToString().Length, '0');
                        }
                    }
                    else
                    {
                        serial = (item.startNum + i).ToString();
                    }
                    string seq = item.Prefix + serial + item.Suffix;
                    number.Add(seq);
                }
            }

            if (number.Count != barcodeList.Count)
            {
                return "录入的模组数量与条码数量不同，条码数量为" + barcodeList.Count;
            }
            if (number.GroupBy(c => c).Where(c => c.Count() > 1).ToList().Count != 0)
            {
                return "录入的有重复模组数,请确认";
            }
            else if (isJson)
            {
                JArray normal = new JArray();
                if (Directory.Exists(@"D:\MES_Data\TemDate\OrderSequence2") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\TemDate\OrderSequence2");
                }

                normal.Add(number);

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(normal, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json", output);//将数据存入json文件中

                //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = UserName, OperateDT = DateTime.Now, OperateRecord = "校正前创建订单号" + ordernum + "规则,选择json录入规则" };
                db.UserOperateLog.Add(log);
                db.SaveChanges();

            }
            else
            {
                for (int i = 0; i < number.Count; i++)
                {
                    barcodeList[i].ModuleGroupNum = number[i];
                    db.SaveChangesAsync();
                }
                UserOperateLog log = new UserOperateLog() { Operator = UserName, OperateDT = DateTime.Now, OperateRecord = "校正前创建订单号" + ordernum + "规则,自动分配录入条码模组" };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
            }

            return "true";
        }

        [HttpPost]
        [ApiAuthorize]
        //条码规则复位
        public string ChangeRule(string ordernum, string UserName)
        {
            JObject result = new JObject();
            var cabinfo = db.CalibrationRecord.Count(c => c.OrderNum == ordernum);
            var appinfo = db.Appearance.Count(c => c.OrderNum == ordernum);
            if (cabinfo != 0 || appinfo != 0)
            {
                result.Add("mes", cabinfo != 0 ? "校正有记录!不能复位" : "外观电检有记录!不能复位");
                result.Add("pass", false);
                return JsonConvert.SerializeObject(result);
            }
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json") == true)
            {
                System.IO.File.Delete(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json");//删除对应的json文件
                UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = UserName, OperateRecord = "删除校正前的条码规则,规则的录入是json模式" };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                result.Add("mes", "删除成功");
                result.Add("pass", true);
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                var Updatesql = "Update BarCodes SET ModuleGroupNum = NULL  where OrderNum='" + ordernum + "'";
                var mes = com.SQLAloneExecute(Updatesql);
                if (mes == "true")
                {
                    UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = UserName, OperateRecord = "删除校正前的条码规则,规则的绑定条码录入" };
                    db.UserOperateLog.Add(log);
                    db.SaveChanges();
                    result.Add("mes", "删除成功");
                    result.Add("pass", true);
                    return JsonConvert.SerializeObject(result);
                }
                else
                {
                    result.Add("mes", "删除失败,失败原因是" + mes);
                    result.Add("pass", false);
                    return JsonConvert.SerializeObject(result);
                }
            }
        }

        [HttpPost]
        [ApiAuthorize]
        //拿到模组号
        public string GetModuleNum(string ordernum, string statue)
        {
            var json = new JArray();
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json") == true)
            {
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json");
                json = JsonConvert.DeserializeObject<JArray>(jsonstring);//读取数据
            }
            var barcodemodule = db.BarCodes.OrderBy(c => c.ModuleGroupNum).Where(c => c.OrderNum == ordernum && c.ModuleGroupNum != null && c.BarCodeType == statue).Select(c => c.ModuleGroupNum).ToList();
            barcodemodule.ForEach(c => json.Add(c));

            return JsonConvert.SerializeObject(json);
        }

        [HttpPost]
        [ApiAuthorize]
        //订单输入规则前判断
        public string CheckRule(string ordenum)
        {
            JObject result = new JObject();
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence2\" + ordenum + ".json") == true)//是否有对应的json文件
            {
                result.Add("candelete", true);
                result.Add("canAdd", false);
                result.Add("mesage", "此订单已有模组号规则!");
                return JsonConvert.SerializeObject(result);

            }
            var barcodemodule = db.BarCodes.Count(c => c.OrderNum == ordenum && c.ModuleGroupNum != null && c.BarCodeType == "模组");
            if (barcodemodule != 0)
            {
                result.Add("candelete", true);
                result.Add("canAdd", false);
                result.Add("mesage", "此订单条码已有模组号!");
                return JsonConvert.SerializeObject(result);
            }

            result.Add("candelete", false);
            result.Add("canAdd", true);
            result.Add("mesage", "");
            return JsonConvert.SerializeObject(result);
        }
        #endregion

        #region -------删除条码的模组

        [HttpPost]
        [ApiAuthorize]
        public string DeleteModuleFromBarcode(string ordernum, string UserName)
        {
            var barcodelist = db.BarCodes.Where(c => c.OrderNum == ordernum && c.BarCodeType == "模组").Select(c => c.BarCodesNum).ToList();
            var smtdate = db.SMT_ProductionData.Count(c => c.OrderNum == ordernum);//SMT记录信息
            var assmbles = db.Assemble.Count(c => barcodelist.Contains(c.BoxBarCode));//组装记录
            var FQC = db.FinalQC.Count(c => barcodelist.Contains(c.BarCodesNum));//FQC记录
            var calib = db.CalibrationRecord.Count(c => barcodelist.Contains(c.BarCodesNum));//校正记录
            var burn = db.Burn_in.Count(c => barcodelist.Contains(c.BarCodesNum));//老化记录
            var appearan = db.Appearance.Count(c => barcodelist.Contains(c.BarCodesNum));//外观记录记录
            if (smtdate == 0 && assmbles == 0 && FQC == 0 && calib == 0 && burn == 0 && appearan == 0)
            {
                var update = db.BarCodes.Where(c => c.OrderNum == ordernum && c.BarCodeType == "模组").ToList();
                update.ForEach(c => c.ModuleGroupNum = null);
                UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = UserName, OperateRecord = "删除条码表的模组号,订单是" + ordernum };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                return "成功";
            }
            else
            {
                return "SMT有" + smtdate + "记录" + ",组装有" + assmbles + "记录" + ",FQC有" + FQC + "记录" + ",校正有" + calib + "记录" + ",老化有" + burn + "记录" + ",电检有" + appearan + "记录";
            }
        }
        #endregion

        #region -------------------内箱标签打印
        /// <summary>
        /// 后台绘图后打印方法
        /// </summary>
        /// <param name="pagecount"></param>
        /// <param name="barcode"></param>
        /// <param name="modulenum"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public string InsideBoxLablePrint(int pagecount, string ip = "", int port = 0, int concentration = 5, string ordernum = "")
        {
            var barcodeList = db.BarCodes.Where(c => c.OrderNum == ordernum).Select(c => new { c.BarCodesNum, c.ModuleGroupNum }).ToList();
            string total = "";
            foreach (var item in barcodeList)
            {
                //开始绘制图片
                int initialWidth = 600, initialHeight = 250;//宽2高1
                Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
                Graphics theGraphics = Graphics.FromImage(theBitmap);
                Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                        //呈现质量
                theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                //背景色
                theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));

                //引入模组号
                System.Drawing.Font myFont_modulenum;
                myFont_modulenum = new System.Drawing.Font("Microsoft YaHei UI", 80, FontStyle.Bold);//OCR-B//宋体
                StringFormat geshi = new StringFormat();
                geshi.Alignment = StringAlignment.Center; //居中
                theGraphics.DrawString(item.ModuleGroupNum, myFont_modulenum, bush, 260, 10, geshi);

                //引入条码
                Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item.BarCodesNum, 380, 30);
                double beishuhege = 0.99;
                theGraphics.DrawImage(bmp_barcode, 80, 150, (float)(bmp_barcode.Width * beishuhege), (float)(bmp_barcode.Height * beishuhege));

                //引入条码号
                System.Drawing.Font myFont_modulebarcodenum;
                myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 13, FontStyle.Regular);
                StringFormat geshi1 = new StringFormat();
                geshi1.Alignment = StringAlignment.Center; //居中
                theGraphics.DrawString(item.BarCodesNum, myFont_modulebarcodenum, bush, 270, 180, geshi);
                //结束图片绘制以上都是绘制图片的代码

                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
                Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值
                MemoryStream ms = new MemoryStream();
                theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                theBitmap.Dispose();
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO150,0^XGR:ZONE.GRF^FS^XZ";

                //string ip = "172.16.99.240";//打印机IP地址
                //int port = 9101;//打印机端口
                string result = IPPrinttest(data.ToString(), pagecount, ip, port);
                if (result != "打印成功！")
                {
                    total = total + item.BarCodesNum + "条码打印失败,";
                }
            }
            if (string.IsNullOrEmpty(total))
            {
                return "打印成功！";
            }
            else
            {
                return total + "请检查打印机是否断网或未开机";
            }
        }

        #endregion

        /// <summary>
        /// 批量打印模组和条码方法
        /// </summary>
        /// <param name="pagecount">打印份数</param>
        /// <param name="barcode">条码列表</param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="concentration"></param>
        /// <param name="testswitch"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [ApiAuthorize]
        public string InsideListPrint(int pagecount, List<string> barcode, string ip = "", int port = 0, int concentration = 5, bool testswitch = false, int right = 0, int down = 0)
        {
            //开始绘制图片
            int initialWidth = 600, initialHeight = 250;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
            //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));

            //引入模组号
            StringFormat geshi = new StringFormat();
            geshi.Alignment = StringAlignment.Center; //居中
            int count = 0;
            for (var j = 0; j < pagecount; j++)
            {

                foreach (var item in barcode)
                {
                    var modulenum = db.BarCodes.Where(c => c.BarCodesNum == item).Select(c => c.ModuleGroupNum).FirstOrDefault();
                    if (modulenum.Length > 9)
                    {
                        System.Drawing.Font myFont_modulenum;
                        myFont_modulenum = new System.Drawing.Font("Microsoft YaHei UI", 40, FontStyle.Bold);//OCR-B//宋体
                        theGraphics.DrawString(modulenum, myFont_modulenum, bush, 260, 20, geshi);
                    }
                    else
                    {
                        System.Drawing.Font myFont_modulenum;
                        myFont_modulenum = new System.Drawing.Font("Microsoft YaHei UI", 55, FontStyle.Bold);//OCR-B//宋体
                        theGraphics.DrawString(modulenum, myFont_modulenum, bush, 260, 20, geshi);
                    }

                    //引入条码
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 380, 30);
                    //double beishuhege = 0.99;
                    theGraphics.DrawImage(bmp_barcode, 70, 150, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //引入条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 13, FontStyle.Regular);
                    StringFormat geshi1 = new StringFormat();
                    geshi1.Alignment = StringAlignment.Center; //居中
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, 260, 180, geshi);
                    //结束图片绘制以上都是绘制图片的代码

                    string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
                    Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值
                                                                                                                  //图片旋转180度
                                                                                                                  //bm.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                                                                                                  //string ip = "172.16.99.240";//打印机IP地址
                                                                                                                  //int port = 9101;//打印机端口

                    //if (testswitch == true)
                    //{
                    //    MemoryStream ms = new MemoryStream();
                    //    //图片旋转180度
                    //    //theBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    //    theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    //    theBitmap.Dispose();
                    //    return File(ms.ToArray(), "image/Png");
                    //}
                    //else
                    //{
                    int totalbytes = bm.ToString().Length;
                    int rowbytes = 10;
                    string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                    data += totalbytes + "," + rowbytes + "," + hex;
                    int x = 0 + right;
                    int y = 0 + down;
                    data += "^LH" + x + "," + y + "^FO150,0^XGR:ZONE.GRF^FS^XZ";
                    IPPrinttest(data.ToString(), 1, ip, port);
                    count++;
                    var print = db.BarCodes.Where(c => c.BarCodesNum == item).FirstOrDefault();
                    print.BarcodePrintCount++;
                    print.ModuleNumPrintCount++;
                    db.SaveChanges();
                    //}
                    theBitmap = new Bitmap(initialWidth, initialHeight);
                    theGraphics = Graphics.FromImage(theBitmap);
                    bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                      //呈现质量
                    theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    //背景色
                    theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
                    Thread.CurrentThread.Join(new TimeSpan(0, 0, 1));
                }
            }
            return count + "个打印成功!" + (barcode.Count - count > 0 ? barcode.Count - count + "个打印失败！" : "");
        }

        [HttpPost]
        [ApiAuthorize]
        public string InsideListPrintNotModuleNum(int pagecount, List<string> barcode, string ip = "", int port = 0, int concentration = 5, bool testswitch = false, int right = 0, int down = 0)
        {
            //开始绘制图片
            int initialWidth = 380, initialHeight = 125;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
            //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));

            //引入模组号
            StringFormat geshi = new StringFormat();
            geshi.Alignment = StringAlignment.Center; //居中
            int count = 0;
            for (var j = 0; j < pagecount; j++)
            {

                foreach (var item in barcode)
                {
                    //引入条码
                    Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 300, 58);
                    //double beishuhege = 1.2;
                    theGraphics.DrawImage(bmp_barcode, 25, 20, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    //引入条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Arial", 18, FontStyle.Regular);
                    //StringFormat geshi1 = new StringFormat();
                    //geshi1.Alignment = StringAlignment.Center; //居中
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, 175, 75, geshi);
                    //结束图片绘制以上都是绘制图片的代码

                    string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
                    Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值
                                                                                                                  //图片旋转180度
                                                                                                                  //bm.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                                                                                                  //string ip = "172.16.99.240";//打印机IP地址
                                                                                                                  //int port = 9101;//打印机端口

                    //if (testswitch == true)
                    //{
                    //    MemoryStream ms = new MemoryStream();
                    //    //图片旋转180度
                    //    //theBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    //    theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    //    theBitmap.Dispose();
                    //    return File(ms.ToArray(), "image/Png");
                    //}
                    //else
                    //{
                    int totalbytes = bm.ToString().Length;
                    int rowbytes = 10;
                    string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                    data += totalbytes + "," + rowbytes + "," + hex;
                    int x = 85 + right;
                    int y = 0 + down;
                    data += "^LH" + x + "," + y + "^FO150,0^XGR:ZONE.GRF^FS^XZ";
                    IPPrinttest(data.ToString(), 1, ip, port);
                    count++;
                    var print = db.BarCodes.Where(c => c.BarCodesNum == item).FirstOrDefault();
                    print.BarcodePrintCount++;
                    db.SaveChanges();
                    //}
                    theBitmap = new Bitmap(initialWidth, initialHeight);
                    theGraphics = Graphics.FromImage(theBitmap);
                    bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                      //呈现质量
                    theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    //背景色
                    theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
                    Thread.CurrentThread.Join(new TimeSpan(0, 0, 1));
                }
            }
            return count + "个打印成功!" + (barcode.Count - count > 0 ? barcode.Count - count + "个打印失败！" : "");
        }

        [HttpPost]
        [ApiAuthorize]
        public string InsideListPrintNotBarcode(int pagecount, List<string> ModuleNum, string Ordernum, string ip = "", int port = 0, int concentration = 5, bool testswitch = false, int right = 0, int down = 0)
        {
            //开始绘制图片
            int initialWidth = 380, initialHeight = 125;//宽2高1
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
            //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));

            //引入模组号
            StringFormat geshi = new StringFormat();
            geshi.Alignment = StringAlignment.Center; //居中
            int count = 0;
            for (var j = 0; j < pagecount; j++)
            {

                foreach (var item in ModuleNum)
                {
                    if (item.Length <= 3)
                    {
                        Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 150, 40);
                        //double beishuhege = 1.2;
                        theGraphics.DrawImage(bmp_barcode, 100, 20, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    }
                    if (item.Length <= 5 || item.Length >= 4)
                    {
                        Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 180, 40);
                        //double beishuhege = 1.2;
                        theGraphics.DrawImage(bmp_barcode, 80, 20, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    }
                    else
                    {
                        //引入条码
                        Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(item, 280, 40);
                        //double beishuhege = 1.2;
                        theGraphics.DrawImage(bmp_barcode, 25, 20, (float)(bmp_barcode.Width), (float)(bmp_barcode.Height));

                    }

                    //引入条码号
                    System.Drawing.Font myFont_modulebarcodenum;
                    myFont_modulebarcodenum = new System.Drawing.Font("Arial", 40, FontStyle.Regular);
                    //StringFormat geshi1 = new StringFormat();
                    //geshi1.Alignment = StringAlignment.Center; //居中
                    theGraphics.DrawString(item, myFont_modulebarcodenum, bush, 175, 55, geshi);
                    //结束图片绘制以上都是绘制图片的代码
                    string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";//^MD5浓度
                    Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值
                                                                                                                  //图片旋转180度
                                                                                                                  //bm.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                                                                                                  //string ip = "172.16.99.240";//打印机IP地址
                                                                                                                  //int port = 9101;//打印机端口

                    //if (testswitch == true)
                    //{
                    //    MemoryStream ms = new MemoryStream();
                    //    //图片旋转180度
                    //    //theBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    //    theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    //    theBitmap.Dispose();
                    //    return File(ms.ToArray(), "image/Png");
                    //}
                    //else
                    //{
                    int totalbytes = bm.ToString().Length;
                    int rowbytes = 10;
                    string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                    data += totalbytes + "," + rowbytes + "," + hex;
                    int x = 85 + right;
                    int y = 0 + down;
                    data += "^LH" + x + "," + y + "^FO150,0^XGR:ZONE.GRF^FS^XZ";
                    IPPrinttest(data.ToString(), 1, ip, port);
                    count++;
                    var print = db.BarCodes.Where(c => c.OrderNum == Ordernum && c.ModuleGroupNum == item && c.BarCodeType == "模组").FirstOrDefault();
                    if (print != null)
                    {
                        print.ModuleNumPrintCount++;
                        db.SaveChanges();
                    }
                    // }
                    theBitmap = new Bitmap(initialWidth, initialHeight);
                    theGraphics = Graphics.FromImage(theBitmap);
                    bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                                                                      //呈现质量
                    theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    //背景色
                    theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
                    Thread.CurrentThread.Join(new TimeSpan(0, 0, 1));
                }
            }
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence2\" + Ordernum + ".json") == true)
            {
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + Ordernum + ".json");
                var json = JsonConvert.DeserializeObject<JArray>(jsonstring);//读取数据
                json.Add("已打印");
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + Ordernum + ".json", output);//将数据存入json文件中
            }
            return count + "个打印成功!" + (ModuleNum.Count - count > 0 ? ModuleNum.Count - count + "个打印失败！" : "");
        }

        public static string IPPrinttest(string data = "", int pagecount = 1, string ip = "", int port = 0)
        {
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(ip, port);
                StreamWriter writer = new StreamWriter(client.GetStream());
                for (int i = 1; i < pagecount + 1; i++)
                {
                    writer.Write(data);
                }
                writer.Flush();
                writer.Close();
                client.Close();
                return "打印成功！";
            }
            catch
            {
                return "打印连接失败,请检查打印机是否断网或未开机！";
            }
        }
    }
}
