﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JianHeMES.Controllers
{
    public class BarCodesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        CommonController com = new CommonController();

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
            ViewBag.BarCodeTypeList = GetBarCodeTypeList();//向View传递OrderNum订单号列表.

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string OrderNum, string BarCodeNum, string BarCodeType, int PageIndex = 0)
        {
            var barcodes = db.BarCodes as IQueryable<BarCodes>;
            if (!String.IsNullOrEmpty(OrderNum))
            {
                barcodes = barcodes.Where(m => m.OrderNum.Contains(OrderNum));
            }

            if (!String.IsNullOrEmpty(BarCodeType))
            {
                barcodes = barcodes.Where(m => m.BarCodeType == BarCodeType);
            }

            if (!String.IsNullOrEmpty(BarCodeNum))
            {
                barcodes = barcodes.Where(m => m.BarCodesNum.Contains(BarCodeNum));
            }

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

            barcodes = barcodes.OrderBy(c => c.BarCodesNum);
            ViewBag.OrderNumList = GetOrderNumList();
            ViewBag.BarCodeTypeList = GetBarCodeTypeList();
            return View(await barcodes.ToListAsync());
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


        //创建模组条码
        [HttpPost]
        public ContentResult CreateModuleGroupBarCodes(int? id)
        {
            if (Session["User"] == null)
            {
                return Content("<script>alert('用户未登录，订单的模块条码创建失败！');history.go(-1);</script>");
            }
            //if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || com.isCheckRole("条码管理", "创建模组条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum) || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC打标员")
            //{
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return Content("<script>alert('此订单不存在！');history.go(-1);</script>");
            }
            if (orderMgm.BarCodeCreated == 1)
            {
                return Content("<script>alert('此订单的模组已经创建过条码，不能重复创建！');history.go(-1);</script>");
            }

            BarCodes aBarCode = new BarCodes();
            aBarCode.OrderNum = orderMgm.OrderNum;
            aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
            aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
            aBarCode.BarCodeType = "模组";
            aBarCode.Creator = ((Users)Session["User"]).UserName;
            aBarCode.CreateDate = DateTime.Now;

            //生成模组条码
            for (int i = 1; i <= orderMgm.Boxes; i++)
            {
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "A" + i.ToString("00000");
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }

            //修改订单的模组条码生成状态为1，表示已经生成.修改订单中的条码创建人
            orderMgm.BarCodeCreated = 1;
            orderMgm.BarCodeCreateDate = DateTime.Now;
            orderMgm.BarCodeCreator = ((Users)Session["User"]).UserName;
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
        public ContentResult CreateModulePieceBarCodes(int? id)
        {
            if (Session["User"] == null)
            {
                return Content("<script>alert('用户未登录，订单的模块条码创建失败！');</script>");
            }
            //if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || com.isCheckRole("条码管理", "创建模块条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum) || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC打标员")
            //{
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return Content("<script>alert('此订单不存在！');history.go(-1);</script>");
            }
            if (orderMgm.ModulePieceBarCodeCreated == 1)
            {
                return Content("<script>alert('此订单的模块已经创建过条码，不能重复创建！');history.go(-1);</script>");
            }

            BarCodes aBarCode = new BarCodes();
            aBarCode.OrderNum = orderMgm.OrderNum;
            aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
            aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
            aBarCode.BarCodeType = "模块";
            aBarCode.Creator = ((Users)Session["User"]).UserName;
            aBarCode.CreateDate = DateTime.Now;

            //生成模块条码
            for (int i = 1; i <= orderMgm.Models; i++)
            {
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "B" + i.ToString("00000");
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }

            //修改订单的模块条码生成状态为1，表示已经生成.修改订单中的条码创建人
            orderMgm.ModulePieceBarCodeCreated = 1;
            orderMgm.ModulePieceBarCodeCreateDate = DateTime.Now;
            orderMgm.ModulePieceBarCodeCreator = ((Users)Session["User"]).UserName;
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
        public ContentResult CreatePowerBarCodes(int? id)
        {
            if (Session["User"] == null)
            {
                return Content("<script>alert('用户未登录，订单的电源条码创建失败！');history.go(-1);</script>");
            }
            //if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || com.isCheckRole("条码管理", "创建电源条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum) || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC打标员")
            //{
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return Content("<script>alert('此订单不存在！');history.go(-1);</script>");
            }
            if (orderMgm.PowerBarCodeCreated == 1)
            {
                return Content("<script>alert('此订单的电源已经创建过条码，不能重复创建！');history.go(-1);</script>");
            }

            BarCodes aBarCode = new BarCodes();
            aBarCode.OrderNum = orderMgm.OrderNum;
            aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
            aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
            aBarCode.BarCodeType = "电源";
            aBarCode.Creator = ((Users)Session["User"]).UserName;
            aBarCode.CreateDate = DateTime.Now;

            //生成电源条码
            for (int i = 1; i <= orderMgm.Powers; i++)
            {
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "C" + i.ToString("00000");
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }

            //修改订单的电源条码生成状态为1，表示已经生成.修改订单中的条码创建人
            orderMgm.PowerBarCodeCreated = 1;
            orderMgm.PowerBarCodeCreateDate = DateTime.Now;
            orderMgm.PowerBarCodeCreator = ((Users)Session["User"]).UserName;
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
        public ContentResult CreateAdapterCardBarCodes(int? id)
        {
            if (Session["User"] == null)
            {
                return Content("<script>alert('用户未登录，订单的转接卡条码创建失败！');history.go(-1);</script>");
            }
            //if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || com.isCheckRole("条码管理", "创建转接卡条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum) || ((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "PC打标员")
            //{
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return Content("<script>alert('此订单不存在！');history.go(-1);</script>");
            }
            if (orderMgm.AdapterCardBarCodeCreated == 1)
            {
                return Content("<script>alert('此订单的转接卡已经创建过条码，不能重复创建！');history.go(-1);</script>");
            }

            BarCodes aBarCode = new BarCodes();
            aBarCode.OrderNum = orderMgm.OrderNum;
            aBarCode.IsRepertory = orderMgm.IsRepertory;//如果订单号为库存批次，条码也为库存
            aBarCode.BarCode_Prefix = orderMgm.BarCode_Prefix;
            aBarCode.BarCodeType = "转接卡";
            aBarCode.Creator = ((Users)Session["User"]).UserName;
            aBarCode.CreateDate = DateTime.Now;

            //生成电源条码
            for (int i = 1; i <= orderMgm.AdapterCard; i++)
            {
                aBarCode.BarCodesNum = orderMgm.BarCode_Prefix + "D" + i.ToString("00000");
                db.BarCodes.Add(aBarCode);
                db.SaveChanges();
            }

            //修改订单的电源条码生成状态为1，表示已经生成.修改订单中的条码创建人
            orderMgm.AdapterCardBarCodeCreated = 1;
            orderMgm.AdapterCardBarCodeCreateDate = DateTime.Now;
            orderMgm.AdapterCardBarCodeCreator = ((Users)Session["User"]).UserName;
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

            ViewBag.assemble = db.Assemble.Where(c => c.BoxBarCode == BarCodeNum).ToList();
            ViewBag.FinalQC = db.FinalQC.Where(c => c.BarCodesNum == BarCodeNum).ToList();
            ViewBag.burn_in = db.Burn_in.Where(c => c.BarCodesNum == BarCodeNum).ToList();
            ViewBag.calibration = db.CalibrationRecord.Where(c => c.BarCodesNum == BarCodeNum).ToList();
            ViewBag.appearance = db.Appearance.Where(c => c.BarCodesNum == BarCodeNum).ToList();

            return View(barCodes);
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
        public ActionResult Edit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "BarCodes", act = "Edit" + "/" + id.ToString() });
            }
            //if (((Users)Session["User"]).Role == "ME工程师" || com.isCheckRole("条码管理", "修改条码", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BarCodes barCodes = db.BarCodes.Find(id);
            if (barCodes == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypeList = SetTypeList();
            ViewBag.BarCodeType = barCodes.BarCodeType;
            return View(barCodes);
            //}
            //return RedirectToAction("Index", "BarCodes");
        }

        // POST: BarCodes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OrderNum,ToOrderNum,BarCode_Prefix,BarCodesNum,ModuleGroupNum,BarCodeType,CreateDate,Creator,IsRepertory,Remark")] BarCodes barCodes)
        {

            if (ModelState.IsValid)
            {
                db.Entry(barCodes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypeList = SetTypeList();
            ViewBag.BarCodeType = barCodes.BarCodeType;
            return View(barCodes);
        }

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
            // JObject relctionitem = new JObject();
            // JObject relaction = new JObject();
            #region 以前版本
            //var relation = db.BarCodeRelation.ToList();
            //if (!string.IsNullOrEmpty(orderNum))
            //{
            //    relation = relation.Where(c => c.NewOrderNum == orderNum || c.OldOrderNum == orderNum).ToList();

            //}
            //if (!string.IsNullOrEmpty(barcode))
            //{
            //    relation = relation.Where(c => c.OldBarCodeNum == barcode || c.NewBarCodesNum == barcode).ToList();
            //}
            //int i = 0;
            //foreach (var item in relation)
            //{
            //    relctionitem.Add("NewOrderNum", item.NewOrderNum);
            //    relctionitem.Add("NewBarCodesNum", item.NewBarCodesNum);
            //    relctionitem.Add("OldOrderNum", item.OldOrderNum);
            //    relctionitem.Add("OldBarCodeNum", item.OldBarCodeNum);
            //    relctionitem.Add("Procedure", item.Procedure);
            //    var name = db.Users.Where(c => c.UserNum == item.UsserID).Select(c => c.UserName).FirstOrDefault();
            //    relctionitem.Add("Name", name);
            //    relaction.Add(i.ToString(), relctionitem);
            //    i++;
            //    relctionitem = new JObject();
            //}
            #endregion
            List<BarCodeRelation> relation = new List<BarCodeRelation>();
            List<BarCodeRelation> olderelation = new List<BarCodeRelation>();
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
            if (relation.Count != 0 && olderelation.Count != 0)
            {

                foreach (var newitem in relation)
                {
                    JArray oldebacodeitem = new JArray();
                    JObject message = new JObject();

                    oldebacodeitem = FindOld(newitem.NewBarCodesNum);
                    // oldebacodeitem.Add(newebacodeitem);
                    message.Add("ordernum", newitem.NewOrderNum);
                    message.Add("barcode", newitem.NewBarCodesNum);
                    oldebacodeitem.Add(message);

                    JArray newebacodeitem = new JArray();
                    newebacodeitem = FindNew(newitem.NewBarCodesNum);
                    var vv = newebacodeitem.ToList();
                    vv.Reverse();
                    vv.ForEach(c => oldebacodeitem.Add(c));
                    //JArray newebacodeitem = new JArray();

                    oldtotal.Add(oldebacodeitem);
                }

                foreach (var newitem in olderelation)
                {
                    string message1 = JsonConvert.SerializeObject(oldtotal);
                    if (message1.Contains(newitem.OldBarCodeNum)||message1.Contains(newitem.NewBarCodesNum))
                    {
                        continue;
                    }
                    JArray newebacodeitem = new JArray();
                    newebacodeitem = FindNew(newitem.OldBarCodeNum);
                    JObject message = new JObject();
                    message.Add("ordernum", newitem.OldOrderNum);
                    message.Add("barcode", newitem.OldBarCodeNum);
                    newebacodeitem.Add(message);
                    var vv = newebacodeitem.ToList();
                    vv.Reverse();
                    newebacodeitem = new JArray();
                    vv.ForEach(c => newebacodeitem.Add(c));
                    oldtotal.Add(newebacodeitem);
                }
            }
            if (relation.Count != 0 && olderelation.Count == 0)
            {
                foreach (var newitem in relation)
                {
                    JArray oldebacodeitem = new JArray();
                    JObject message = new JObject();

                    oldebacodeitem = FindOld(newitem.NewBarCodesNum);
                    // oldebacodeitem.Add(newebacodeitem);
                    message.Add("ordernum", newitem.NewOrderNum);
                    message.Add("barcode", newitem.NewBarCodesNum);
                    oldebacodeitem.Add(message);
                    oldtotal.Add(oldebacodeitem);
                }
            }
            if (relation.Count == 0 && olderelation.Count != 0)
            {

                foreach (var newitem in olderelation)
                {
                    JArray newebacodeitem = new JArray();
                    newebacodeitem = FindNew(newitem.OldBarCodeNum);
                    JObject message = new JObject();
                    message.Add("ordernum", newitem.OldOrderNum);
                    message.Add("barcode", newitem.OldBarCodeNum);
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

    }
}
