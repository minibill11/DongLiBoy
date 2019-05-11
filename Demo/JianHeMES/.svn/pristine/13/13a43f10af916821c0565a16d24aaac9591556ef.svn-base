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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JianHeMES.Controllers
{
    public class Personnel_FrameworkController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();

        // GET: Personnel_Framework
        public async Task<ActionResult> Index()
        {
            JObject manageJobect = new JObject();
            JObject depatmentJobject = new JObject();
            JObject groupJobject = new JObject();
            JObject positionjobject = new JObject();
            var manageList = db.Personnel_Framework.Select(c => c.Central_layer).Distinct().ToList();
            manageList.RemoveAll(null);
            foreach (var manage in manageList)
            {
                var depatmentList = db.Personnel_Framework.Where(c => c.Central_layer == manage).Select(c => c.Department).Distinct().ToList();
                depatmentList.RemoveAll(null);
                foreach (var department in depatmentList)
                {
                    var dp_groupList = db.Personnel_Framework.Where(c => c.Central_layer == manage && c.Department == department).Select(c => c.Group).Distinct().ToList();
                    dp_groupList.RemoveAll(null);
                    foreach (var group in dp_groupList)
                    {
                        var positionList = db.Personnel_Framework.Where(c => c.Central_layer == manage && c.Department == department && c.Group == group).Select(c => c.Position).Distinct().ToList();
                        int i = 0;
                        foreach (var position in positionList)
                        {
                            positionjobject.Add(i.ToString(), position);
                            i++;
                        }
                        groupJobject.Add(group, positionjobject);
                        positionjobject = new JObject();
                    }
                    depatmentJobject.Add(department, groupJobject);
                    groupJobject = new JObject();

                }
                manageJobect.Add(manage, depatmentJobject);
                depatmentJobject = new JObject();

            }
            if (manageList.Count != 0)
            {
                return Content(JsonConvert.SerializeObject(manageJobect));
            }

            return Content("获取数据失败");
        }


        [HttpPost]
        public ActionResult Index(string a)
        {
            //List<string> info = new List<string>();
            //if (manage != null && department == null && group == null)
            //    info = db.Personnel_Framework.Where(c => c.Central_layer == manage).Select(c => c.Department).Distinct().ToList();
            //else if (manage != null && department != null && group == null)
            //    info = db.Personnel_Framework.Where(c => c.Central_layer == manage && c.Department == department).Select(c => c.Group).Distinct().ToList();

            //else if (manage != null && department != null && group != null)
            //    info = db.Personnel_Framework.Where(c => c.Central_layer == manage && c.Department == department && c.Group == group).Select(c => c.Position).Distinct().ToList();
            //else
            //    return Content("获取数据失败");
            //if (info.Count != 0)
            //{ return Content(JsonConvert.SerializeObject(info)); }
            
            return Content("获取数据失败");
        }

        // GET: Personnel_Framework/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Framework personnel_Framework = await db.Personnel_Framework.FindAsync(id);
            if (personnel_Framework == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Framework);
        }

        // GET: Personnel_Framework/Create
        public ActionResult CreateFramework()
        {
            return View();
        }

        // POST: Personnel_Framework/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateFramework([Bind(Include = "Id,Central_layer,Department,Group,Position,CreateDate,Creator")] Personnel_Framework personnel_Framework)
        {

            if (ModelState.IsValid)
            {
                db.Personnel_Framework.Add(personnel_Framework);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(personnel_Framework);
        }

        // GET: Personnel_Framework/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Framework personnel_Framework = await db.Personnel_Framework.FindAsync(id);
            if (personnel_Framework == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Framework);
        }

        // POST: Personnel_Framework/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Central_layer,Department,Group,Position,CreateDate,Creator")] Personnel_Framework personnel_Framework)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personnel_Framework).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(personnel_Framework);
        }

        // GET: Personnel_Framework/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Framework personnel_Framework = await db.Personnel_Framework.FindAsync(id);
            if (personnel_Framework == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Framework);
        }

        // POST: Personnel_Framework/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Personnel_Framework personnel_Framework = await db.Personnel_Framework.FindAsync(id);
            db.Personnel_Framework.Remove(personnel_Framework);
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
    }
}
