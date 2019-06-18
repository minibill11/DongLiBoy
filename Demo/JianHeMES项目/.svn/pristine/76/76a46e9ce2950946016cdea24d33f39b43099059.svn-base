using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class ProductionPlansController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        Common com = new Common();

        // GET: ProductionPlans
        public ActionResult Index()
        {
            return View();
        }




        #region---------------------产线计划管理 SMT_ProductionPlan
        [HttpGet]
        public ActionResult SMT_ProductionPlan()
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.LineNumList = GetLineNumList();
            ViewBag.JobContentList = GetJobContentList();
            var records = db.SMT_ProductionPlan.OrderBy(c => c.LineNum).Where(c => DbFunctions.DiffDays(c.PlanProductionDate, DateTime.Now) <= 0).ToList();
            return View(records);
        }

        [HttpPost]
        public ActionResult SMT_ProductionPlan(string orderNum, int? lineNum, string jobContent, DateTime? planProductionDate, string remark)
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.LineNumList = GetLineNumList();
            ViewBag.JobContentList = GetJobContentList();
            List<SMT_ProductionPlan> recordlist = new List<SMT_ProductionPlan>();
            if (String.IsNullOrEmpty(orderNum) && lineNum == null && String.IsNullOrEmpty(jobContent) && planProductionDate == null && String.IsNullOrEmpty(remark))
            {
                return View(recordlist);
            }
            recordlist = db.SMT_ProductionPlan.ToList();
            if (!String.IsNullOrEmpty(orderNum))
            {
                recordlist = recordlist.Where(c => c.OrderNum == orderNum).ToList();
            }
            if (lineNum != null)
            {
                recordlist = recordlist.Where(c => c.LineNum == lineNum).ToList();
            }
            if (!String.IsNullOrEmpty(jobContent))
            {
                recordlist = recordlist.Where(c => c.JobContent == jobContent).ToList();
            }
            if (planProductionDate != null)
            {
                recordlist = recordlist.Where(c => c.PlanProductionDate.Value.Year == planProductionDate.Value.Year && c.PlanProductionDate.Value.Month == planProductionDate.Value.Month && c.PlanProductionDate.Value.Day == planProductionDate.Value.Day).ToList();
            }
            if (!String.IsNullOrEmpty(remark))
            {
                recordlist = recordlist.Where(c => c.Remark.Contains(remark) == true).ToList();
            }
            return View(recordlist);
        }

        public ActionResult SMT_ProductionPlanCreate()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT", act = "SMT_ProductionPlanCreate" });
            }
            if (((Users)Session["User"]).Role == "PC计划员" || com.isCheckRole("SMT计划管理", "创建订单计划", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            {
                ViewBag.Status = ProductionLineStatus();
                return View();
            }
            return Content("<script>alert('对不起，您的不能管理产线信息，请联系SMT看板管理员！');window.location.href='../SMT/SMT_Mangage';</script>");
        }

        [HttpPost]
        public ActionResult SMT_ProductionPlanCheck(string records)
        {
            List<SMT_ProductionPlan> orders = new List<SMT_ProductionPlan>();

            JObject SMT_OrderInfoCheck_result = new JObject();
            orders = JsonConvert.DeserializeObject<List<SMT_ProductionPlan>>(records);
            if (orders != null)
            {
                foreach (var item in orders)
                {
                    var IsExistOrder = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum == item.LineNum && c.JobContent == item.JobContent && DbFunctions.DiffDays(item.PlanProductionDate, c.PlanProductionDate) >= 0).Count();
                    if (IsExistOrder > 0)
                    {
                        SMT_OrderInfoCheck_result.Add(item.LineNum + "_" + item.OrderNum + "_" + item.JobContent, "此产线已存在该订单计划");
                    }
                    else
                    {
                        if (db.OrderMgm.Count(c => c.OrderNum == item.OrderNum) > 0)
                        {
                            SMT_OrderInfoCheck_result.Add(item.LineNum + "_" + item.OrderNum + "_" + item.JobContent, "此产线可以做订单计划");
                        }
                        else
                        {
                            SMT_OrderInfoCheck_result.Add(item.LineNum + "_" + item.OrderNum + "_" + item.JobContent, "此订单号不存在");
                        }
                    }
                }
                return Json(SMT_OrderInfoCheck_result.ToString(), JsonRequestBehavior.AllowGet);
            }
            return View(records);
        }

        [HttpPost]
        public ActionResult SMT_ProductionPlanCreate(string records)
        {
            List<SMT_ProductionPlan> orders = new List<SMT_ProductionPlan>();

            orders = JsonConvert.DeserializeObject<List<SMT_ProductionPlan>>(records);
            if (orders != null)
            {
                foreach (var item in orders)
                {
                    if (ModelState.IsValid)
                    {
                        db.SMT_ProductionPlan.Add(item);
                        db.SaveChanges();
                        int countItem = db.SMT_ProductionBoardTable.Count(c => c.OrderNum == item.OrderNum && c.JobContent == item.JobContent && c.LineNum == item.LineNum);
                        if (countItem == 0)
                        {
                            SMT_ProductionBoardTable new_record = new SMT_ProductionBoardTable { OrderNum = item.OrderNum, JobContent = item.JobContent, LineNum = item.LineNum };
                            db.SMT_ProductionBoardTable.Add(new_record);
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("SMT_ProductionPlan");
            }
            return View(records);
        }

        public async Task<ActionResult> SMT_ProductionPlanEdit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT", act = "SMT_ProductionPlanEdit" + "/" + id.ToString() });
            }
            if (((Users)Session["User"]).Role == "PC计划员" || com.isCheckRole("SMT计划管理", "修改订单计划", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SMT_ProductionPlan record = await db.SMT_ProductionPlan.FindAsync(id);
                if (record == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Status = ProductionLineStatus();
                return View(record);
            }
            return Content("<script>alert('对不起，您的不能管理产线信息，请联系系统管理员！');window.location.href='../SMT/SMT_ProductionPlan';</script>");
        }

        [HttpPost]
        public async Task<ActionResult> SMT_ProductionPlanEdit([Bind(Include = "Id,LineNum,OrderNum,Capacity,Quantity,JobContent,CreateDate,PlanProductionDate,Creator,Remark")]SMT_ProductionPlan record)
        {
            if (ModelState.IsValid)
            {
                db.Entry(record).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("SMT_ProductionPlan");
            }
            return View(record);
        }

        // GET: SMT_ProductionPlan/Delete/
        public async Task<ActionResult> SMT_ProductionPlanDelete(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT", act = "SMT_ProductionPlanDelete" + "/" + id.ToString() });
            }
            if (((Users)Session["User"]).Role == "PC计划员" || com.isCheckRole("SMT计划管理", "删除订单计划", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SMT_ProductionPlan record = await db.SMT_ProductionPlan.FindAsync(id);
                if (record == null)
                {
                    return HttpNotFound();
                }
                return View(record);
            }
            return Content("<script>alert('对不起，您的不能管理产线信息，请联系系统管理员！');window.location.href='../SMT_ProductionPlan';</script>");
        }

        // POST: SMT_ProductionPlan/Delete/
        [HttpPost, ActionName("SMT_ProductionPlanDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SMT_ProductionPlanDeleteConfirmed(int id)
        {
            SMT_ProductionPlan record = await db.SMT_ProductionPlan.FindAsync(id);
            db.SMT_ProductionPlan.Remove(record);
            await db.SaveChangesAsync();
            return Content("<script>alert('SMT计划已经成功删除！');window.location.href='../SMT_ProductionPlan';</script>");
        }

        #endregion


        #region --------------------产线状态列表
        private List<SelectListItem> ProductionLineStatus()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "生产中",
                    Value = "生产中"
                },
                new SelectListItem
                {
                    Text = "待料",
                    Value = "待料"
                },
                new SelectListItem
                {
                    Text = "设备保养",
                    Value = "设备保养"
                },
                new SelectListItem
                {
                    Text = "停产",
                    Value = "停产"
                }
            };
        }
        #endregion


        #region --------------------GetOrderList()取出整个OrderMgms的OrderNum订单号列表
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


        #region --------------------GetLineNumList()取出SMT的LineNumList订单号列表
        private List<SelectListItem> GetLineNumList()
        {
            var lineNumList = db.SMT_ProductionLineInfo.OrderBy(m => m.LineNum).Select(m => m.LineNum).ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            var items = new List<SelectListItem>();
            foreach (int lineNum in lineNumList)
            {
                items.Add(new SelectListItem
                {
                    Text = lineNum.ToString(),
                    Value = lineNum.ToString()
                });
            }
            return items;
        }
        #endregion


        #region --------------------GetJobContentList()取出SMT_ProductionPlan的JobContentList订单号列表
        private List<SelectListItem> GetJobContentList()
        {
            var jobContentList = db.SMT_ProductionPlan.Select(m => m.JobContent).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            var items = new List<SelectListItem>();
            foreach (string jobContent in jobContentList)
            {
                items.Add(new SelectListItem
                {
                    Text = jobContent,
                    Value = jobContent
                });
            }
            return items;
        }
        #endregion


        #region --------------------GetTodayPlanOrderNumList() 取出今天SMT计划的所有OrderNum订单号列表
        private List<SelectListItem> GetTodayPlanOrderNumList()
        {
            var date = DateTime.Now;
            var orders = db.SMT_ProductionPlan.Where(c => c.PlanProductionDate.Value.Year == date.Year && c.PlanProductionDate.Value.Month == date.Month && c.PlanProductionDate.Value.Day == date.Day).OrderByDescending(m => m.Id).Select(m => m.OrderNum).Distinct();    //增加.Distinct()后会重新按OrderNum升序排序
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


        #region -----------------PlatformTypeList()取出整个OrderMgms的PlatformTypeList列表
        private List<SelectListItem> PlatformTypeList()
        {
            var orders = db.OrderMgm.Select(m => m.PlatformType).Distinct().ToList();
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

    }
}