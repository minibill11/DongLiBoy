using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;

namespace JianHeMES.Controllers
{
    public class OrderInformationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region --------------------Index首页
        // GET: OrderInformations
        public ActionResult Index()
        {
            //分页
            var OrderInformations = db.OrderInformation as IQueryable<OrderInformation>;
            var recordCount = OrderInformations.Count();
            var pageCount = GetPageCount(recordCount);

            ViewBag.PAGE_SIZE = PAGE_SIZE;
            ViewBag.PageIndex = 0;
            ViewBag.PageCount = pageCount;

            return View(GetPagedDataSource(OrderInformations, 0, recordCount));
        }

        [HttpPost]
        public ActionResult Index(int PageIndex)
        {
            //分页
            var OrderInformations = db.OrderInformation as IQueryable<OrderInformation>;

            var recordCount = OrderInformations.Count();
            var pageCount = GetPageCount(recordCount);
            if (PageIndex >= pageCount && pageCount >= 1)
            {
                PageIndex = pageCount - 1;
            }

            OrderInformations = OrderInformations.OrderByDescending(m => m.CreateDate)
                                .Skip(PageIndex * PAGE_SIZE).Take(PAGE_SIZE);

            ViewBag.PAGE_SIZE = PAGE_SIZE;
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageCount = pageCount;

            return View(OrderInformations.ToList());
        }
        #endregion

        #region --------------------Details页
        // GET: OrderInformations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderInformation orderInformation = db.OrderInformation.Find(id);
            if (orderInformation == null)
            {
                return HttpNotFound();
            }
            return View(orderInformation);
        }

        #endregion

        #region --------------------Create页
        // GET: OrderInformations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderInformations/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OrderNum,ModuleGroupQuantity,PlaceAnOrderDate,DateOfDelivery,CreateDate")] OrderInformation orderInformation)
        {



            if (ModelState.IsValid)
            {
                orderInformation.CreateDate = DateTime.Now;
                db.OrderInformation.Add(orderInformation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(orderInformation);
        }
        #endregion

        #region --------------------Edit页
        // GET: OrderInformations/Edit/5
        public ActionResult Edit(int? id)
        {



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderInformation orderInformation = db.OrderInformation.Find(id);
            if (orderInformation == null)
            {
                return HttpNotFound();
            }
            return View(orderInformation);
        }

        // POST: OrderInformations/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OrderNum,ModuleGroupQuantity,PlaceAnOrderDate,DateOfDelivery,CreateDate")] OrderInformation orderInformation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderInformation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(orderInformation);
        }
        #endregion

        #region --------------------Delete页
        // GET: OrderInformations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderInformation orderInformation = db.OrderInformation.Find(id);
            if (orderInformation == null)
            {
                return HttpNotFound();
            }
            return View(orderInformation);
        }

        // POST: OrderInformations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderInformation orderInformation = db.OrderInformation.Find(id);
            db.OrderInformation.Remove(orderInformation);
            db.SaveChanges();
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

        #region --------------------分页功能函数
        private static readonly int PAGE_SIZE = 15;
        static List<OrderInformation> GetPageListByIndex(List<OrderInformation> list, int pageIndex)
        {
            //int pageSize = 10;
            return list.Skip((pageIndex - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();
        }
      
        //分页方法

        private int GetPageCount(int recordCount)
        {
            int pageCount = recordCount / PAGE_SIZE;
            if (recordCount % PAGE_SIZE != 0)
            {
                pageCount += 1;
            }
            return pageCount;
        }

        private List<OrderInformation> GetPagedDataSource(IQueryable<OrderInformation> Informations, int pageIndex, int recordCount)
        {
            var pageCount = GetPageCount(recordCount);
            if (pageIndex >= pageCount && pageCount >= 1)
            {
                pageIndex = pageCount - 1;
            }

            return Informations.OrderByDescending(m => m.CreateDate)
                                                         .Skip(pageIndex * PAGE_SIZE)
                                                         .Take(PAGE_SIZE).ToList(); 
        }
        #endregion

    }
}


