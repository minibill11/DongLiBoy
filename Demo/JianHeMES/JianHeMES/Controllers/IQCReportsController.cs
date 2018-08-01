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

namespace JianHeMES.Controllers
{
    public class IQCReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region  -----//物料列表-----------

        private List<SelectListItem> Material()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "请选择物料",
                    Value = ""
                },
                new SelectListItem
                {
                    Text = "三合一贴片灯",
                    Value = "三合一贴片灯"
                },
                new SelectListItem
                {
                    Text = "LED插件灯",
                    Value = "LED插件灯"
                }
            };
        }

        #endregion

        // GET: IQCReports
        public async Task<ActionResult> Index()
        {
            return View(await db.IQCReports.ToListAsync());
        }

        // GET: IQCReports/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IQCReport iQCReport = await db.IQCReports.FindAsync(id);
            if (iQCReport == null)
            {
                return HttpNotFound();
            }
            return View(iQCReport);
        }

        // GET: IQCReports/Create
        public ActionResult IQCReportCreate()
        {
            ViewBag.Material = Material();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "IQC检验员" || ((Users)Session["User"]).Role == "系统管理员")
            {
                return View();
            }
            return View();
        }

        // POST: IQCReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IQCReportCreate([Bind(Include = "Id,Material_SN,RoHS_REACH,OrderNumber,EquipmentNum,Provider,MaterialName,ModelNumber,Specification,MaterialQuantity,IncomingDate,ApplyPurchaseOrderNum,BatchNum,InspectionDate,SamplingPlan,MaterialVersion,C1,C2,C3,C4,C5,C6,C7,C8,C9,D1,D2,D3,D4,D5,D6,D7,D8,D9,E1,E2,E3,E4,E5,E6,E7,E8,E9,F1,F2,F3,F4,F5,F6,F7,F8,F9,S0,S1,S11,S12,S13,S14,S15,S2,S21,S22,S23,S24,S25,S3,S31,S32,S33,S34,S35,SR,SRJson,SG,SGJson,SB,SBJson,R0,R1,R11,R12,R13,R2,R21,R22,R23,R3,R31,R32,R33,P0,P1,P11,P12,P13,P2,P21,P22,P23,P3,P31,P32,P33,AM,AM0,AM1,AM11,AM12,AM13,AM2,AM21,AM22,AM23,AM3,AM31,AM32,AM33,AM4,AM41,AM42,AM43,NG1,NG2,NG3,NGD,NGHandle,ReportRemark,Inspector,Creator,CreatedDate,Assessor,AssessedDate,AssessorRemark,AssessorPass,Approve,ApprovedDate,ApproveRemark,ApprovePass")] IQCReport iQCReport)
        {
            ViewBag.Material = Material();
            iQCReport.Creator = ((Users)Session["User"]).UserName;
            iQCReport.CreatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.IQCReports.Add(iQCReport);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(iQCReport);
        }

        // GET: IQCReports/Edit/5
        public async Task<ActionResult> IQCReportEdit(int? id)
        {
            ViewBag.Material = Material();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "IQC检验员" || ((Users)Session["User"]).Role == "系统管理员")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                IQCReport iQCReport = await db.IQCReports.FindAsync(id);
                if (iQCReport == null)
                {
                    return HttpNotFound();
                }
                return View(iQCReport);
            }
            return RedirectToAction("Index", "IQCReports");
        }

        // POST: IQCReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IQCReportEdit([Bind(Include = "Id,Material_SN,RoHS_REACH,OrderNumber,EquipmentNum,Provider,MaterialName,ModelNumber,Specification,MaterialQuantity,IncomingDate,ApplyPurchaseOrderNum,BatchNum,InspectionDate,SamplingPlan,MaterialVersion,C1,C2,C3,C4,C5,C6,C7,C8,C9,D1,D2,D3,D4,D5,D6,D7,D8,D9,E1,E2,E3,E4,E5,E6,E7,E8,E9,F1,F2,F3,F4,F5,F6,F7,F8,F9,S0,S1,S11,S12,S13,S14,S15,S2,S21,S22,S23,S24,S25,S3,S31,S32,S33,S34,S35,SR,SRJson,SG,SGJson,SB,SBJson,R0,R1,R11,R12,R13,R2,R21,R22,R23,R3,R31,R32,R33,P0,P1,P11,P12,P13,P2,P21,P22,P23,P3,P31,P32,P33,AM,AM0,AM1,AM11,AM12,AM13,AM2,AM21,AM22,AM23,AM3,AM31,AM32,AM33,AM4,AM41,AM42,AM43,NG1,NG2,NG3,NGD,NGHandle,ReportRemark,Inspector,Creator,CreatedDate,Assessor,AssessedDate,AssessorRemark,AssessorPass,Approve,ApprovedDate,ApproveRemark,ApprovePass")] IQCReport iQCReport)
        {
            ViewBag.Material = Material();
            if (ModelState.IsValid)
            {
                db.Entry(iQCReport).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(iQCReport);
        }

        public async Task<ActionResult> IQCReportAssessor(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "IQC组长" || ((Users)Session["User"]).Role == "系统管理员")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                IQCReport iQCReport = await db.IQCReports.FindAsync(id);
                if (iQCReport == null)
                {
                    return HttpNotFound();
                }
                return View(iQCReport);
            }
            return RedirectToAction("Index", "IQCReports");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IQCReportAssessor([Bind(Include = "Id,Material_SN,RoHS_REACH,OrderNumber,EquipmentNum,Provider,MaterialName,ModelNumber,Specification,MaterialQuantity,IncomingDate,ApplyPurchaseOrderNum,BatchNum,InspectionDate,SamplingPlan,MaterialVersion,C1,C2,C3,C4,C5,C6,C7,C8,C9,D1,D2,D3,D4,D5,D6,D7,D8,D9,E1,E2,E3,E4,E5,E6,E7,E8,E9,F1,F2,F3,F4,F5,F6,F7,F8,F9,S0,S1,S11,S12,S13,S14,S15,S2,S21,S22,S23,S24,S25,S3,S31,S32,S33,S34,S35,SR,SRJson,SG,SGJson,SB,SBJson,R0,R1,R11,R12,R13,R2,R21,R22,R23,R3,R31,R32,R33,P0,P1,P11,P12,P13,P2,P21,P22,P23,P3,P31,P32,P33,AM,AM0,AM1,AM11,AM12,AM13,AM2,AM21,AM22,AM23,AM3,AM31,AM32,AM33,AM4,AM41,AM42,AM43,NG1,NG2,NG3,NGD,NGHandle,ReportRemark,Inspector,Creator,CreatedDate,Assessor,AssessedDate,AssessorRemark,AssessorPass,Approve,ApprovedDate,ApproveRemark,ApprovePass")] IQCReport iQCReport)
        {
            iQCReport.Assessor = ((Users)Session["User"]).UserName;
            iQCReport.AssessedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(iQCReport).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(iQCReport);
        }

        public async Task<ActionResult> IQCReportApprove(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "品质部经理" || ((Users)Session["User"]).Role == "系统管理员")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                IQCReport iQCReport = await db.IQCReports.FindAsync(id);
                if (iQCReport == null)
                {
                    return HttpNotFound();
                }
                return View(iQCReport);
            }
            return RedirectToAction("Index", "IQCReports");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IQCReportApprove([Bind(Include = "Id,Material_SN,RoHS_REACH,OrderNumber,EquipmentNum,Provider,MaterialName,ModelNumber,Specification,MaterialQuantity,IncomingDate,ApplyPurchaseOrderNum,BatchNum,InspectionDate,SamplingPlan,MaterialVersion,C1,C2,C3,C4,C5,C6,C7,C8,C9,D1,D2,D3,D4,D5,D6,D7,D8,D9,E1,E2,E3,E4,E5,E6,E7,E8,E9,F1,F2,F3,F4,F5,F6,F7,F8,F9,S0,S1,S11,S12,S13,S14,S15,S2,S21,S22,S23,S24,S25,S3,S31,S32,S33,S34,S35,SR,SRJson,SG,SGJson,SB,SBJson,R0,R1,R11,R12,R13,R2,R21,R22,R23,R3,R31,R32,R33,P0,P1,P11,P12,P13,P2,P21,P22,P23,P3,P31,P32,P33,AM,AM0,AM1,AM11,AM12,AM13,AM2,AM21,AM22,AM23,AM3,AM31,AM32,AM33,AM4,AM41,AM42,AM43,NG1,NG2,NG3,NGD,NGHandle,ReportRemark,Inspector,Creator,CreatedDate,Assessor,AssessedDate,AssessorRemark,AssessorPass,Approve,ApprovedDate,ApproveRemark,ApprovePass")] IQCReport iQCReport)
        {
            iQCReport.Approve = ((Users)Session["User"]).UserName;
            iQCReport.ApprovedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(iQCReport).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(iQCReport);
        }


        // GET: IQCReports/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "系统管理员")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                IQCReport iQCReport = await db.IQCReports.FindAsync(id);
                if (iQCReport == null)
                {
                    return HttpNotFound();
                }
                return View(iQCReport);
            }
            return RedirectToAction("Index", "IQCReports");
        }

        // POST: IQCReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            IQCReport iQCReport = await db.IQCReports.FindAsync(id);
            db.IQCReports.Remove(iQCReport);
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
