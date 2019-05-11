using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using static JianHeMES.Controllers.CommonalityController;
using System.ComponentModel;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace JianHeMES.Controllers
{
    public class CourtScreenModuleInfoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: CourtScreenModuleInfoes
        public async Task<ActionResult> Index()
        {
            ViewBag.OrderList = GetCourtScreenOrderNumList();//向View传递OrderNum订单号列表.
            return View(await db.CourtScreenModuleInfoes.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> Index(string orderNum, string remark)
        {
            ViewBag.OrderList = GetCourtScreenOrderNumList();//向View传递OrderNum订单号列表.
            var recordList = db.CourtScreenModuleInfoes.Where(c=>c.OrderNum==orderNum);
            if(!String.IsNullOrEmpty(remark))
            {
                recordList = recordList.Where(c => c.Remark.Contains(remark));
            }
            return View(await recordList.ToListAsync());
        }


        public ActionResult Input()
        {
            ViewBag.OrderList = GetOrderNumList();//向View传递OrderNum订单号列表.
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Input(string orderNum, string modelNum, List<CourtScreenModuleInfo> recordList)
        {
            ViewBag.OrderList = GetOrderNumList();//向View传递OrderNum订单号列表.
            if (orderNum == null || modelNum == null || recordList == null)
            {
                return Content("数据为空，请输入正确的数据");
            }
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "CourtScreenModuleInfoes", act = "Index"});
            }
            try
            {
                db.CourtScreenModuleInfoes.AddRange(recordList);
                await db.SaveChangesAsync();
            }
            catch
            {
                return Content("条码输入失败！");
            }
            return Content("全部插入成功");
        }

        [HttpPost]
        public async Task<ActionResult> CheckList(string orderNum, string modelNum, List<CourtScreenModuleInfo> recordList)
        {
            if (orderNum == null || modelNum == null || recordList == null)
            {
                return Content("请输入正确的数据");
            }
            string repeatNum = null;
            foreach (var record in recordList)
            {
                var count = await db.CourtScreenModuleInfoes.Where(c => c.OrderNum == orderNum && c.ModuleNum == modelNum && c.BarCode == record.BarCode).CountAsync();
                if (count != 0)
                {//重复条码
                    repeatNum = repeatNum + record.BarCode + ",";
                }
            }
            if (repeatNum != null)
            {
                return Content(repeatNum + "这些条码重复，请确认后重新输入");
            }
            else
            return Content("成功");
        }

        #region  -------------导出球场屏记录输出Excel表格方法------------------

        public class ResultToExcel
        {
            //订单号
            public string OrderNum { get; set; }
            //模组号
            public string ModuleNum { get; set; }
            //条码
            public string BarCode { get; set; }
            //类型
            public string Type { get; set; }
            //状态
            public int Status { get; set; }
            //备注
            public string Remark { get; set; }
            //录入人
            public string Creater { get; set; }
            //录入时间
            public string CreateDate { get; set; }
        }


        [HttpPost]
        public FileContentResult OutputExcel(string orderNum)
        {
            List<CourtScreenModuleInfo> queryRecords = db.CourtScreenModuleInfoes.Where(c=>c.OrderNum==orderNum).OrderBy(c=>c.ModuleNum).ThenBy(c=>c.BarCode).ToList();
            List<ResultToExcel> Resultlist = new List<ResultToExcel>();
            if (queryRecords != null)
            {
                foreach (var item in queryRecords)
                {
                    ResultToExcel at = new ResultToExcel();
                    at.OrderNum = item.OrderNum;
                    at.ModuleNum = item.ModuleNum;
                    at.BarCode = item.BarCode;
                    at.Type = item.Type;
                    at.Status = item.Status;
                    at.Remark = item.Remark;
                    at.Creater = item.Creater;
                    at.CreateDate = item.CreateDate==null?" ":string.Format("{0:yyyy-MM-dd HH:mm}", item.CreateDate);
                    Resultlist.Add(at);
                }
            }
            if (Resultlist.Count() > 0)
            {
                string[] columns = { "订单号", "模组号", "条码", "类型", "状态", "备注", "录入人", "录入时间" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, orderNum+"条码记录表", false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, orderNum+"条码记录表.xlsx");
            }
            else
            {
                ResultToExcel at1 = new ResultToExcel();
                at1.OrderNum = "没有找到相关记录！";
                Resultlist.Add(at1);
                string[] columns = { "订单号", "模组号", "条码", "类型", "状态", "备注", "录入人", "录入时间" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, "条码记录表", false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, "条码记录表.xlsx");
            }
        }
        #endregion




        #region-----------其他方法

        // GET: CourtScreenModuleInfoes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourtScreenModuleInfo courtScreenModuleInfo = await db.CourtScreenModuleInfoes.FindAsync(id);
            if (courtScreenModuleInfo == null)
            {
                return HttpNotFound();
            }
            return View(courtScreenModuleInfo);
        }

        // GET: CourtScreenModuleInfoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CourtScreenModuleInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,OrderNum,ModuleNum,Type,BarCode,Status,Remark,Creater,CreateDate")] CourtScreenModuleInfo courtScreenModuleInfo)
        {
            if (ModelState.IsValid)
            {
                db.CourtScreenModuleInfoes.Add(courtScreenModuleInfo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(courtScreenModuleInfo);
        }

        // GET: CourtScreenModuleInfoes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourtScreenModuleInfo courtScreenModuleInfo = await db.CourtScreenModuleInfoes.FindAsync(id);
            if (courtScreenModuleInfo == null)
            {
                return HttpNotFound();
            }
            return View(courtScreenModuleInfo);
        }

        // POST: CourtScreenModuleInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,OrderNum,ModuleNum,Type,BarCode,Status,Remark,Creater,CreateDate")] CourtScreenModuleInfo courtScreenModuleInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courtScreenModuleInfo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(courtScreenModuleInfo);
        }

        public async Task<ActionResult> Edit1(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourtScreenModuleInfo courtScreenModuleInfo = await db.CourtScreenModuleInfoes.FindAsync(id);
            if (courtScreenModuleInfo == null)
            {
                return HttpNotFound();
            }
            return View(courtScreenModuleInfo);
        }

        // POST: CourtScreenModuleInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit1([Bind(Include = "Id,OrderNum,ModuleNum,Type,BarCode,Status,Remark,Creater,CreateDate")] CourtScreenModuleInfo courtScreenModuleInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courtScreenModuleInfo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(courtScreenModuleInfo);
        }


        // GET: CourtScreenModuleInfoes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourtScreenModuleInfo courtScreenModuleInfo = await db.CourtScreenModuleInfoes.FindAsync(id);
            if (courtScreenModuleInfo == null)
            {
                return HttpNotFound();
            }
            return View(courtScreenModuleInfo);
        }

        // POST: CourtScreenModuleInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CourtScreenModuleInfo courtScreenModuleInfo = await db.CourtScreenModuleInfoes.FindAsync(id);
            db.CourtScreenModuleInfoes.Remove(courtScreenModuleInfo);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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

        #region ---------------------------------------检索订单号
        private List<SelectListItem> GetOrderNumList()
        {
            var ordernum = db.OrderMgm.OrderBy(m => m.OrderNum).Select(m => m.OrderNum).Distinct();

            var ordernumitems = new List<SelectListItem>();
            foreach (string num in ordernum)
            {
                ordernumitems.Add(new SelectListItem
                {
                    Text = num,
                    Value = num
                });
            }
            return ordernumitems;
        }
        #endregion

        #region ---------------------------------------检索球场屏订单号
        private List<SelectListItem> GetCourtScreenOrderNumList()
        {
            var ordernum = db.CourtScreenModuleInfoes.OrderBy(m => m.OrderNum).Select(m => m.OrderNum).Distinct();

            var ordernumitems = new List<SelectListItem>();
            foreach (string num in ordernum)
            {
                ordernumitems.Add(new SelectListItem
                {
                    Text = num,
                    Value = num
                });
            }
            return ordernumitems;
        }
        #endregion
    }
}
