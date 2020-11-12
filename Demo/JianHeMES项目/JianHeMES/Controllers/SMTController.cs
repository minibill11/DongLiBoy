using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JianHeMES.Models;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net;
using System.Data.Entity;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using Microsoft.Owin.Security.Twitter.Messages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JianHeMES.AuthAttributes;

namespace JianHeMES.Controllers
{
    public class SMTController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        CommonController com = new CommonController();

        #region------------------SMT管理界面(产线管理，产线生产计划)
        // GET: SMT管理主页
        public class Temp
        {
            public string line { get; set; }

            public decimal value { get; set; }
        }
        public ActionResult SMT_Manage()
        {
            List<SMT_ProductionLineInfo> SMT_ProcutionLineInfos = new List<SMT_ProductionLineInfo>();
            SMT_ProcutionLineInfos = db.SMT_ProductionLineInfo.ToList();
            if (SMT_ProcutionLineInfos != null)
            {
                return View(SMT_ProcutionLineInfos);
            }
            else
                return View();
        }


        #region---------------------产线管理 SMT_ProductionLine

        public ActionResult SMT_ProductionLineCreate()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT", act = "SMT_ProductionLineCreate" });
            }
            //if (((Users)Session["User"]).Department == "SMT部" && ((Users)Session["User"]).Role == "主管" || ((Users)Session["User"]).Role == "SMT看板管理员" || com.isCheckRole("产线管理", "添加产线", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            ViewBag.Status = ProductionLineStatus();
            return View();
            //}
            //return Content("<script>alert('对不起，您不能管理产线信息，请联系SMT看板管理员！');window.location.href='../SMT/SMT_Manage';</script>");
        }

        [HttpPost]
        public ActionResult SMT_ProductionLineCreate([Bind(Include = "Id,LineNum,ProducingOrderNum,CreateDate,Team,GroupLeader,Status")] SMT_ProductionLineInfo newline)
        {
            //if (db.SMT_ProductionLineInfo.Where(c => c.LineNum == newline.LineNum).Count() > 0)
            //{
            //    ModelState.AddModelError("", "此产线已经存在，请检查产线号是否正确！");
            //    return View(newline);
            //}
            newline.CreateDate = DateTime.Now;
            ViewBag.Status = ProductionLineStatus();
            if (ModelState.IsValid)
            {
                db.SMT_ProductionLineInfo.Add(newline);
                db.SaveChanges();
                return RedirectToAction("SMT_Manage");
            }
            else
            {
                return View(newline);
            }
        }


        public async Task<ActionResult> SMT_ProductionLineEdit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT", act = "SMT_ProductionLineEdit" + "/" + id.ToString() });
            }
            //if (((Users)Session["User"]).Department == "SMT部" && ((Users)Session["User"]).Role == "主管" || ((Users)Session["User"]).Role == "SMT看板管理员" || com.isCheckRole("产线管理", "修改产线", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SMT_ProductionLineInfo record = await db.SMT_ProductionLineInfo.FindAsync(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = ProductionLineStatus();
            ViewBag.StatusValue = record.Status;
            return View(record);
            //}
            //return Content("<script>alert('对不起，您的不能管理产线信息，请联系SMT看板管理员！');window.location.href='../SMT_Manage';</script>");
        }

        [HttpPost]
        public async Task<ActionResult> SMT_ProductionLineEdit([Bind(Include = "Id,LineNum,ProducingOrderNum,CreateDate,Team,GroupLeader,Status")]SMT_ProductionLineInfo record)
        {
            if (ModelState.IsValid)
            {
                var old = db.SMT_ProductionLineInfo.Find(record.Id);
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "SMT产线信息修改：原信息产线、订单号、创建记录日期、班组组长、产线状态为" + old.LineNum + "," + old.ProducingOrderNum + "," + old.CreateDate + "," + old.Team + "," + old.GroupLeader + "," + old.Status + "修改为," + record.LineNum + "," + record.ProducingOrderNum + "," + record.CreateDate + "," + record.Team + "," + record.GroupLeader + "," + record.Status };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                old.LineNum = record.LineNum;
                old.ProducingOrderNum = record.ProducingOrderNum;
                old.CreateDate = record.CreateDate;
                old.Team = record.Team;
                old.GroupLeader = record.GroupLeader;
                old.Status = record.Status;
                await db.SaveChangesAsync();
                return RedirectToAction("SMT_Manage");
            }
            return View(record);
        }

        #endregion


        #region---------------------订单管理 SMT_OrderManage

        // GET: SMT订单信息管理
        public ActionResult SMT_OrderManage()
        {
            ViewBag.FinishStatus = FinishStatusList();
            return View();
        }

        [HttpPost]
        public PartialViewResult SMT_OrderManage(string FinishStatus)
        {
            List<SMT_OrderInfo> QueryResult = new List<SMT_OrderInfo>();
            //筛选完成状态
            if (FinishStatus != null)
            {
                if (FinishStatus == "")
                {
                    QueryResult = db.SMT_OrderInfo.ToList();
                }
                else if (FinishStatus == "完成")
                {
                    QueryResult = db.SMT_OrderInfo.Where(c => c.Status == true).ToList();
                }
                else if (FinishStatus == "未完成")
                {
                    QueryResult = db.SMT_OrderInfo.Where(c => c.Status == false).ToList();
                }
            }
            //System.Threading.Thread.Sleep(2000);
            ViewBag.FinishStatus = FinishStatusList();
            return PartialView(QueryResult);
        }

        public ActionResult SMT_OrderInfoCreate()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT", act = "SMT_OrderInfoCreate" });
            }
            //if (((Users)Session["User"]).Department == "SMT部" && ((Users)Session["User"]).Role == "主管" || ((Users)Session["User"]).Role == "SMT看板管理员" || com.isCheckRole("SMT管理", "SMT订单创建", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            return View();
            //}
            //return Content("<script>alert('对不起，您的不能管理产线信息，请联系SMT看板管理员！');window.location.href='../SMT/SMT_OrderManage';</script>");
        }

        [HttpPost]
        public ActionResult SMT_OrderInfoCheck(string records)
        {
            List<SMT_OrderInfo> orders = new List<SMT_OrderInfo>();

            JObject SMT_OrderInfoCheck_result = new JObject();
            orders = JsonConvert.DeserializeObject<List<SMT_OrderInfo>>(records);
            if (orders != null)
            {
                foreach (var item in orders)
                {
                    var IsExistOrder = db.SMT_OrderInfo.Where(c => c.OrderNum == item.OrderNum).Count();
                    if (IsExistOrder > 0)
                    {
                        SMT_OrderInfoCheck_result.Add(item.OrderNum, "订单已存在");
                    }
                    else
                    {
                        SMT_OrderInfoCheck_result.Add(item.OrderNum, "订单不存在");
                    }
                }
                //return Content(SMT_OrderInfoCheck_result.ToString());
                return Json(SMT_OrderInfoCheck_result.ToString(), JsonRequestBehavior.AllowGet);
            }
            return View(records);
        }

        [HttpPost]
        public ActionResult SMT_OrderInfoCreate(string records)
        {
            List<SMT_OrderInfo> orders = new List<SMT_OrderInfo>();

            orders = JsonConvert.DeserializeObject<List<SMT_OrderInfo>>(records);
            if (orders != null)
            {
                foreach (var item in orders)
                {
                    if (ModelState.IsValid)
                    {
                        db.SMT_OrderInfo.Add(item);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("SMT_OrderManage");
            }
            return View(records);
        }

        public async Task<ActionResult> SMT_OrderInfoEdit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT", act = "SMT_OrderInfoEdit" + "/" + id.ToString() });
            }
            //if (((Users)Session["User"]).Department == "SMT部" && ((Users)Session["User"]).Role == "主管" || ((Users)Session["User"]).Role == "SMT看板管理员" || com.isCheckRole("SMT管理", "SMT订单修改", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SMT_OrderInfo record = await db.SMT_OrderInfo.FindAsync(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = ProductionLineStatus();
            return View(record);
            //}
            //return Content("<script>alert('对不起，您的不能管理产线信息，请联系SMT看板管理员！');window.location.href='../SMT_OrderManage';</script>");
        }

        [HttpPost]
        public async Task<ActionResult> SMT_OrderInfoEdit([Bind(Include = "Id,OrderNum,LineNum,Quantity,PlatformType,Customer,DeliveryDate,Status")]SMT_OrderInfo record)
        {
            if (ModelState.IsValid)
            {
                db.Entry(record).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("SMT_OrderManage");
            }
            return View(record);
        }

        #endregion


        #region---------------------产线计划管理 SMT_ProductionPlan
        public ActionResult New_SMT_ProductionPlan()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT", act = "New_SMT_ProductionPlan" });
            }
            return View();
        }

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

        public ActionResult SMT_ProductionPlan1(string orderNum, int? lineNum, string jobContent, DateTime? planProductionDate, string remark)
        {
            JArray result = new JArray();
            var smtplanvalue = db.SMT_ProductionPlan.Select(c => new { c.LineNum, c.OrderNum, c.JobContent, c.Quantity, c.Capacity, c.PlanProductionDate, c.Remark, c.Id }).ToList();
            if (!string.IsNullOrEmpty(orderNum))
            {
                smtplanvalue = smtplanvalue.Where(c => c.OrderNum == orderNum).ToList();
            }
            if (lineNum != null)
            {
                smtplanvalue = smtplanvalue.Where(c => c.LineNum == lineNum).ToList();
            }
            if (!string.IsNullOrEmpty(jobContent))
            {
                smtplanvalue = smtplanvalue.Where(c => c.JobContent == jobContent).ToList();
            }
            if (planProductionDate != null)
            {
                smtplanvalue = smtplanvalue.Where(c => c.PlanProductionDate == planProductionDate).ToList();
            }
            foreach (var item in smtplanvalue)
            {
                JObject obj = new JObject();
                obj.Add("Id", item.Id);
                obj.Add("LineNum", item.LineNum);
                obj.Add("OrderNum", item.OrderNum);
                obj.Add("JobContent", item.JobContent);
                obj.Add("Capacity", item.Capacity);
                obj.Add("Quantity", item.Quantity);
                obj.Add("CreateDate", string.Format("{0:yyyy-MM-dd }", item.PlanProductionDate));
                obj.Add("Remark", item.Remark);
                result.Add(obj);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult SMT_ProductionPlanCreate()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT", act = "SMT_ProductionPlanCreate" });
            }
            //if (((Users)Session["User"]).Role == "PC计划员" || com.isCheckRole("SMT计划管理", "创建订单计划", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            ViewBag.Status = ProductionLineStatus();
            return View();
            //}
            //return Content("<script>alert('对不起，您的不能管理产线信息，请联系SMT看板管理员！');window.location.href='../SMT/SMT_Mangage';</script>");
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
                    //string b = string.Format("{0:yyyy-MM-dd}", c.PlanProductionDate);
                    var IsExistOrder = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum == item.LineNum && c.JobContent == item.JobContent && c.PlanProductionDate.Value.Year == item.PlanProductionDate.Value.Year && c.PlanProductionDate.Value.Month == item.PlanProductionDate.Value.Month && c.PlanProductionDate.Value.Day == item.PlanProductionDate.Value.Day).Count();
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
        public ActionResult SMT_ProductionPlanCreate(string records, string Department1, string Group)
        {
            List<SMT_ProductionPlan> orders = new List<SMT_ProductionPlan>();

            orders = JsonConvert.DeserializeObject<List<SMT_ProductionPlan>>(records);
            if (orders != null)
            {
                foreach (var item in orders)
                {
                    if (ModelState.IsValid)
                    {
                        item.Department = Department1;
                        item.Group = Group;
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
            //if (((Users)Session["User"]).Role == "PC计划员" || com.isCheckRole("SMT计划管理", "修改订单计划", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
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
            // }
            // return Content("<script>alert('对不起，您的不能管理产线信息，请联系系统管理员！');window.location.href='../SMT/SMT_ProductionPlan';</script>");
        }

        [HttpPost]
        public async Task<ActionResult> SMT_ProductionPlanEdit([Bind(Include = "Id,LineNum,OrderNum,Capacity,Quantity,JobContent,CreateDate,PlanProductionDate,Creator,Remark")]SMT_ProductionPlan record)
        {
            if (ModelState.IsValid)
            {
                var old = db.SMT_ProductionPlan.Find(record.Id);
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "SMT计划修改：原数据产线、订单号、计划产能、计划数量、工作内容、计划生产日期、备注为" + old.LineNum + "，" + old.OrderNum + "，" + old.Capacity + "," + old.Quantity + "，" + old.JobContent + "，" + old.CreateDate + "," + old.Remark + "修改为" + record.LineNum + "，" + record.OrderNum + "，" + record.Capacity + "," + record.Quantity + "，" + record.JobContent + "，" + record.CreateDate + "," + record.Remark };
                db.UserOperateLog.Add(log);
                old.LineNum = record.LineNum;
                old.OrderNum = record.OrderNum;
                old.Capacity = record.Capacity;
                old.Quantity = record.Quantity;
                old.JobContent = record.JobContent;
                old.CreateDate = record.CreateDate;
                old.Remark = record.Remark;

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
            // if (((Users)Session["User"]).Role == "PC计划员" || com.isCheckRole("SMT计划管理", "删除订单计划", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //  {

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
            //}
            //return Content("<script>alert('对不起，您的不能管理产线信息，请联系系统管理员！');window.location.href='../SMT_ProductionPlan';</script>");
        }

        // POST: SMT_ProductionPlan/Delete/
        [HttpPost, ActionName("SMT_ProductionPlanDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SMT_ProductionPlanDeleteConfirmed(int id)
        {
            SMT_ProductionPlan record = await db.SMT_ProductionPlan.FindAsync(id);
            UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "SMT计划删除：原数据产线、订单号、计划产能、计划数量、工作内容、计划生产日期、备注为" + record.LineNum + "，" + record.OrderNum + "，" + record.Capacity + "," + record.Quantity + "，" + record.JobContent + "，" + record.CreateDate + "," + record.Remark };
            db.UserOperateLog.Add(log);
            db.SMT_ProductionPlan.Remove(record);
            await db.SaveChangesAsync();
            return Content("<script>alert('SMT计划已经成功删除！');window.location.href='../SMT_ProductionPlan';</script>");
        }

        public ActionResult SMT_ProductionPlanBatchDeleteConfirmed(List<int> id)
        {
            JObject result = new JObject();
            List<SMT_ProductionPlan> record = db.SMT_ProductionPlan.Where(c => id.Contains(c.Id)).ToList();
            var userlis = record.Select(c => c.Creator).Distinct().ToList();
            var name = ((Users)Session["User"]).UserName;
            if ((userlis.Count > 1 || !userlis.Contains(name)) && ((Users)Session["User"]).Role != "系统管理员")
            {
                result.Add("Result", false);
                var username = record.Where(c => c.Creator != name).Select(c => c.Creator).Distinct().ToList();
                result.Add("Message", "记录包含" + string.Join(",", username) + "的记录,你没有权限删除该条信息");
                return Content(JsonConvert.SerializeObject(result));
            }
            var ordernumlist = record.Select(c => c.OrderNum).ToList();
            var time = record.Select(c => c.PlanProductionDate).ToList();
            UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "SMT计划删除：订单" + string.Join(",", ordernumlist) + ";计划时间" + string.Join(",", time) };
            db.UserOperateLog.Add(log);
            db.SMT_ProductionPlan.RemoveRange(record);
            db.SaveChanges();
            result.Add("Result", true);
            result.Add("Message", "SMT计划已经成功删除");
            return Content(JsonConvert.SerializeObject(result));
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #endregion


        #region---------------------用户管理 SMT_UserManage

        // GET: SMT用户管理
        //public ActionResult SMT_UserManage()
        //{
        //    if (Session["User"] == null)
        //    {
        //        return RedirectToAction("Login", "Users", new { col = "SMT", act = "SMT_UserManage" });
        //    }
        //    List<Users> SMT_User = db.Users.Where(c => c.Department == "SMT").ToList();
        //    string userRole = ((Users)Session["User"]).Role;
        //    string department = ((Users)Session["User"]).Department;
        //    if (userRole == "经理" & department == "SMT" || userRole == "系统管理员")
        //    {
        //        return View(SMT_User);
        //    }
        //    if (userRole == "SMT看板管理员" & department == "SMT" || userRole == "系统管理员")
        //    {
        //        SMT_User = SMT_User.Where(c => c.Role != "经理").ToList();
        //        return View(SMT_User);
        //    }
        //    return Content("<script>alert('对不起，您的不能管理SMT产线用户，请联系SMT经理！');window.location.href='../SMT/SMT_Mangage';</script>");
        //}


        // GET: SMT_User/Details/5
        public async Task<ActionResult> SMT_UserDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: SMT_User/Create
        public ActionResult SMT_UserCreate()
        {
            ViewBag.createRoleList = createRoleList();//创建用户角色列表
            return View();
        }

        // POST: Packagings/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SMT_UserCreate([Bind(Include = "ID,UserNum,UserName,Password,CreateDate,Creator,UserAuthorize,Role,Department,LineNum,Deleter,DeleteDate,Description")] Users user)
        {
            ViewBag.createRoleList = createRoleList();//创建用户角色列表
            user.Department = "SMT";
            user.CreateDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                //return RedirectToAction("SMT_UserMangage");
                return Content("<script>alert('用户" + user.UserName + "添加成功！');window.location.href='../SMT/SMT_UserMangage';</script>");

            }
            return View(user);
        }

        // GET: SMT_User/Edit/5
        public async Task<ActionResult> SMT_UserEdit(int? id)
        {
            ViewBag.createRoleList = createRoleList();//创建用户角色列表
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: SMT_User/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SMT_UserEdit([Bind(Include = "ID,UserNum,UserName,Password,CreateDate,Creator,UserAuthorize,Role,Department,LineNum,Deleter,DeleteDate,Description")] Users user)
        {
            ViewBag.createRoleList = createRoleList();//创建用户角色列表
            user.Department = "SMT";
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //return RedirectToAction("SMT_UserMangage");
                return Content("<script>alert('用户信息已成功修改！');window.location.href='../SMT_UserMangage';</script>");
            }
            return View(user);
        }

        // GET: SMT_User/Delete/5
        public async Task<ActionResult> SMT_UserDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: SMT_User/Delete/5
        [HttpPost, ActionName("SMT_UserDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Users user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            //return RedirectToAction("SMT_UserMangage");
            return Content("<script>alert('用户已经成功删除！');window.location.href='../SMT_UserMangage';</script>");
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


        #endregion


        #region------------------SMT生产信息（总看板、产线看板、查找历史记录） SMT_ProductionLineInfo  SMT_ProductionData

        // GET: SMT总览表
        public ActionResult SMT_ProductionInfo()
        {
            return View();
        }

        public ActionResult SMT_ProductionInfo1()
        {
            return View();
        }

        public ActionResult SMT_ProductionLineInfo(string LineNum)
        {
            ViewBag.LineNum = LineNum;//产线号
            return View();
        }

        [HttpPost]
        public ActionResult SMT_ProductionLineInfo(int LineNum)
        {
            //内容:痴线号，时间，班组，组长，正在生产的订单，良品数量，不良品数量，不良率，产线累计个数，订单完成率，今天计划订单，产线状态
            ViewBag.LineNum = LineNum;//获取产线号

            JObject lineBoardDay = new JObject();
            JObject lineBoardNight = new JObject();
            JObject day = new JObject();
            JObject night = new JObject();
            JObject result = new JObject();

            var productionDataTime = new List<SMT_ProductionData>();
            if (DateTime.Now.Hour >= 8)
            {
                var daystart = DateTime.Now.Date.AddHours(8);
                var dayend = DateTime.Now.Date.AddHours(23);
                productionDataTime = db.SMT_ProductionData.Where(c => c.LineNum == LineNum && c.BeginTime >= daystart && c.BeginTime <= dayend).OrderBy(c => c.BeginTime).ToList();
            }
            else
            {
                DateTime? yestoday = DateTime.Now.AddDays(-1).Date.AddHours(8);
                productionDataTime = db.SMT_ProductionData.Where(c => c.LineNum == LineNum && c.BeginTime <= DateTime.Now && c.BeginTime >= yestoday).OrderBy(c => c.BeginTime).ToList();
            }
            decimal totalCapacity = 0;
            int totalNormalCount = 0;
            int totalAbnormalCount = 0;
            int total = 0;
            int daynum = 0;
            int nightnum = 0;

            foreach (var time in productionDataTime)
            {
                if (time.BeginTime == null || time.EndTime == null)
                {
                    continue;
                }
                //是否是今天
                bool istoday = string.Format("{0:yyyy-MM-dd}", time.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                //是否是昨天
                bool isyestoday = string.Format("{0:yyyy-MM-dd}", time.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(-1));
                //时数间隔
                double num = (time.EndTime - time.BeginTime).Value.TotalHours;


                //白班生产时段记录
                if (time.BeginTime.Value.Hour >= 8 && time.BeginTime.Value.Hour < 20)
                {
                    //订单号
                    lineBoardDay.Add("ordernmu", time.OrderNum);
                    //制作面
                    lineBoardDay.Add("jobContent", time.JobContent);
                    //time.BeginTime.Value.Hour + ":" + time.BeginTime.Value.Minute
                    lineBoardDay.Add("ProductionBegin", string.Format("{0:HH:mm}", time.BeginTime));
                    lineBoardDay.Add("ProductionEnd", string.Format("{0:HH:mm}", time.EndTime));

                    //标准产能
                    var Capacity = db.SMT_ProductionPlan.Where(c => c.OrderNum == time.OrderNum && c.LineNum == LineNum).Select(c => c.Capacity).FirstOrDefault();
                    lineBoardDay.Add("Capacity", Capacity);
                    totalCapacity = totalCapacity + Capacity * Decimal.Parse(num.ToString());

                    //实际完成
                    lineBoardDay.Add("NormalCount", time.NormalCount);
                    totalNormalCount = totalNormalCount + time.NormalCount;

                    //目的达成率
                    lineBoardDay.Add("achievement", Capacity == 0 ? "" : (time.NormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                    //不良数
                    lineBoardDay.Add("AbnormalCount", time.AbnormalCount);
                    totalAbnormalCount = totalAbnormalCount + time.AbnormalCount;

                    //不良率
                    lineBoardDay.Add("Defective", Capacity == 0 ? "" : (time.AbnormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                    //备注
                    var remark = db.SMT_ProductionPlan.Where(c => c.OrderNum == time.OrderNum && c.LineNum == LineNum).Select(c => c.Remark).FirstOrDefault();
                    lineBoardDay.Add("Remark", remark);

                    day.Add(daynum.ToString(), lineBoardDay);
                    daynum++;
                    total++;
                    lineBoardDay = new JObject();
                }
                //晚班生产记录
                else
                {
                    //今天晚上20-24
                    if (time.BeginTime.Value.Hour >= 20 || time.BeginTime.Value.Hour < 8)
                    {
                        lineBoardNight.Add("ProductionBegin", time.BeginTime.Value.Hour + ":" + time.BeginTime.Value.Minute);
                        lineBoardNight.Add("ProductionEnd", time.EndTime.Value.Hour + ":" + time.EndTime.Value.Minute);
                    }
                    else
                    {
                        continue;
                    }
                    //else if (DateTime.Now.Hour < 8 && (istoday && time.BeginTime.Value.Hour < 8) || (isyestoday && time.BeginTime.Value.Hour >= 20))
                    //{
                    //    //今天0-8和昨天的20-24
                    //    lineBoardNight.Add("ProductionBegin", time.BeginTime.Value);
                    //    lineBoardNight.Add("ProductionEnd", time.EndTime.Value);
                    //}
                    //订单号
                    lineBoardNight.Add("ordernmu", time.OrderNum);
                    //制作面
                    lineBoardNight.Add("jobContent", time.JobContent);

                    //标准产能
                    var Capacity = db.SMT_ProductionPlan.Where(c => c.OrderNum == time.OrderNum && c.LineNum == LineNum).Select(c => c.Capacity).FirstOrDefault();
                    lineBoardNight.Add("Capacity", Capacity);
                    totalCapacity = totalCapacity + Capacity * Decimal.Parse(num.ToString());

                    //实际完成
                    lineBoardNight.Add("NormalCount", time.NormalCount);
                    totalNormalCount = totalNormalCount + time.NormalCount;

                    //目的达成率
                    lineBoardNight.Add("achievement", Capacity == 0 ? "" : (time.NormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                    //不良数
                    lineBoardNight.Add("AbnormalCount", time.AbnormalCount);
                    totalAbnormalCount = totalAbnormalCount + time.AbnormalCount;

                    //不良率
                    lineBoardNight.Add("Defective", Capacity == 0 ? "" : (time.AbnormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                    //备注
                    var remark = db.SMT_ProductionPlan.Where(c => c.OrderNum == time.OrderNum && c.LineNum == LineNum).Select(c => c.Remark).FirstOrDefault();
                    lineBoardNight.Add("Remark", remark);

                    night.Add(nightnum.ToString(), lineBoardNight);
                    nightnum++;
                    total++;
                    lineBoardNight = new JObject();
                }
            }
            //白班的数据集
            result.Add("day", day);
            //晚班的数据集
            result.Add("night", night);
            //生产日期
            result.Add("todaty", string.Format("{0:yyyy-MM-dd}", DateTime.Now));
            //班组
            var groupLeader = db.SMT_ProductionLineInfo.Where(c => c.LineNum == LineNum).Select(c => c.GroupLeader).FirstOrDefault();
            result.Add("groupLeader", groupLeader);
            //生产人数
            result.Add("personNum", "");
            //统计
            result.Add("totalNum", total.ToString());
            //计划完成数
            result.Add("totalCapacity", totalCapacity);
            //实际总完成
            result.Add("totalNormalCount", totalNormalCount);
            //总完成率
            result.Add("totalAchievement", totalCapacity == 0 ? "0" : (totalNormalCount / totalCapacity * 100).ToString("F2") + "%");
            //不良总数
            result.Add("totalAbnormalCount", totalAbnormalCount);
            //总不良率
            result.Add("totalDefective", totalCapacity == 0 ? "0" : (totalAbnormalCount / totalCapacity * 100).ToString("F2") + "%");

            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult SMT_ProductionLineInfoHistory()
        {
            ViewBag.OrderNumList = GetSMTOrderList();
            ViewBag.jobContentList = jobContentList();
            return View();
        }
        [HttpPost]
        public ActionResult SMT_ProductionLineInfoHistory(string OrderNum, int LineNum, DateTime? startTime, DateTime? endtime, string jobcontent)
        {
            ViewBag.OrderNumList = GetSMTOrderList();
            ViewBag.jobContentList = jobContentList();
            JObject smtInfo = new JObject();
            JObject smtInfoList = new JObject();
            JObject message = new JObject();
            JObject result = new JObject();
            decimal totalCapacity = 0;
            int totalNormalCount = 0;
            int totalAbnormalCount = 0;
            var Message = new List<SMT_ProductionData>();

            //第一种查询，输入线别和开始日期
            if (LineNum != 0 && startTime != null)
            {

                if (endtime == null)
                {
                    var twoTime = startTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    Message = db.SMT_ProductionData.Where(c => c.LineNum == LineNum && c.BeginTime >= startTime && c.BeginTime <= twoTime).ToList();
                }
                else
                {
                    Message = db.SMT_ProductionData.Where(c => c.LineNum == LineNum && c.BeginTime >= startTime && c.BeginTime <= endtime).ToList();
                }

                var timeSpan = Message.Select(c => c.BeginTime.Value.Date).Distinct().ToList();
                int total = 0;
                foreach (var time in timeSpan)
                {
                    DateTime nightTime = time.AddHours(23).AddMinutes(59).AddSeconds(59);
                    var smtMessage = Message.Where(c => c.BeginTime >= time && c.BeginTime <= nightTime).ToList();
                    int i = 0;
                    foreach (var item in smtMessage)
                    {
                        double num = (item.EndTime - item.BeginTime).Value.TotalHours;
                        //线别
                        smtInfo.Add("LineNum", item.LineNum);
                        //生产时段
                        smtInfo.Add("startime", string.Format("{0:yyyy/MM/dd HH:mm}", item.BeginTime));
                        smtInfo.Add("endtime", string.Format("{0:yyyy/MM/dd HH:mm}", item.EndTime));
                        //制作面
                        smtInfo.Add("jobContent", item.JobContent);
                        //订单号
                        smtInfo.Add("orderNum", item.OrderNum);
                        //标准产能
                        var Capacity = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum == LineNum).Select(c => c.Capacity).FirstOrDefault();
                        totalCapacity = totalCapacity + Capacity * Decimal.Parse(num.ToString());
                        smtInfo.Add("Capacity", Capacity);

                        //实际完成
                        smtInfo.Add("NormalCount", item.NormalCount);
                        totalNormalCount = totalNormalCount + item.NormalCount;

                        //目的达成率
                        smtInfo.Add("achievement", Capacity == 0 ? "" : (item.NormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                        //不良数
                        smtInfo.Add("AbnormalCount", item.AbnormalCount);
                        totalAbnormalCount = totalAbnormalCount + item.AbnormalCount;

                        //不良率
                        smtInfo.Add("Defective", Capacity == 0 ? "" : (item.AbnormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                        //备注
                        var remark = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum == LineNum).Select(c => c.Remark).FirstOrDefault();
                        smtInfo.Add("Remark", remark);

                        smtInfoList.Add(i.ToString(), smtInfo);
                        i++;
                        total++;
                        smtInfo = new JObject();
                    }
                    message.Add(time.ToString(), smtInfoList);
                    smtInfoList = new JObject();
                }
                //数据集
                result.Add("message", message);
                message = new JObject();
                //统计
                result.Add("totalNum", total.ToString());
                //计划完成数
                result.Add("totalCapacity", totalCapacity);
                //实际总完成
                result.Add("totalNormalCount", totalNormalCount);
                //总完成率
                result.Add("totalAchievement", totalCapacity == 0 ? "0" : (totalNormalCount / totalCapacity * 100).ToString("F2") + "%");
                //不良总数
                result.Add("totalAbnormalCount", totalAbnormalCount);
                //总不良率
                result.Add("totalDefective", totalCapacity == 0 ? "0" : (totalAbnormalCount / totalCapacity * 100).ToString("F2") + "%");
            }
            else
            {
                //第二种查询，输入订单号和只做面
                if (OrderNum != null && jobcontent != null)
                {
                    Message = db.SMT_ProductionData.Where(c => c.OrderNum == OrderNum && c.JobContent == jobcontent).OrderBy(c => c.OrderNum).ToList();
                }
                if (OrderNum == null && jobcontent != null)
                {
                    Message = db.SMT_ProductionData.Where(c => c.JobContent == jobcontent).OrderBy(c => c.OrderNum).ToList();
                }
                if (OrderNum != null && jobcontent == null)
                {
                    Message = db.SMT_ProductionData.Where(c => c.OrderNum == OrderNum).OrderBy(c => c.OrderNum).ToList();
                }
                int j = 0;
                foreach (var item in Message)
                {
                    double num = (item.EndTime - item.BeginTime).Value.TotalHours;
                    //线别
                    smtInfo.Add("LineNum", item.LineNum);
                    //生产时段
                    smtInfo.Add("startime", string.Format("{0:yyyy/MM/dd HH:mm}", item.BeginTime));
                    smtInfo.Add("endtime", string.Format("{0:yyyy/MM/dd HH:mm}", item.EndTime));
                    //制作面
                    smtInfo.Add("jobContent", item.JobContent);
                    //订单号
                    smtInfo.Add("orderNum", item.OrderNum);
                    //标准产能
                    var Capacity = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.JobContent == jobcontent).Select(c => c.Capacity).FirstOrDefault();
                    totalCapacity = totalCapacity + Capacity * Decimal.Parse(num.ToString());
                    smtInfo.Add("Capacity", Capacity);

                    //实际完成
                    smtInfo.Add("NormalCount", item.NormalCount);
                    totalNormalCount = totalNormalCount + item.NormalCount;

                    //目的达成率
                    smtInfo.Add("achievement", Capacity == 0 ? "" : (item.NormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                    //不良数
                    smtInfo.Add("AbnormalCount", item.AbnormalCount);
                    totalAbnormalCount = totalAbnormalCount + item.AbnormalCount;

                    //不良率
                    smtInfo.Add("Defective", Capacity == 0 ? "" : (item.AbnormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                    //备注
                    var remark = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum == LineNum).Select(c => c.Remark).FirstOrDefault();
                    smtInfo.Add("Remark", remark);

                    message.Add(j.ToString(), smtInfo);
                    j++;
                    smtInfo = new JObject();
                }
                result.Add("message", message);
                message = new JObject();
                //统计
                result.Add("totalNum", j.ToString());
                //计划完成数
                result.Add("totalCapacity", totalCapacity);
                //实际总完成
                result.Add("totalNormalCount", totalNormalCount);
                //总完成率
                result.Add("totalAchievement", totalCapacity == 0 ? "0" : (totalNormalCount / totalCapacity * 100).ToString("F2") + "%");
                //不良总数
                result.Add("totalAbnormalCount", totalAbnormalCount);
                //总不良率
                result.Add("totalDefective", totalCapacity == 0 ? "0" : (totalAbnormalCount / totalCapacity * 100).ToString("F2") + "%");
            }
            var CC = Content(JsonConvert.SerializeObject(result));
            return Content(JsonConvert.SerializeObject(result));
        }
        public ActionResult SMT_ProductionInfoHistory()
        {
            ViewBag.PlatformType = PlatformTypeList();
            ViewBag.OrderNumList = GetSMTOrderList();
            ViewBag.jobContentList = jobContentList();
            return View();
        }
        [HttpPost]
        public ActionResult SMT_ProductionInfoHistory(List<string> ordernum, List<string> platformType, List<string> jobContent)
        {
            ViewBag.PlatformType = PlatformTypeList();
            ViewBag.OrderNumList = GetSMTOrderList();
            ViewBag.jobContentList = jobContentList();
            JObject SMT_Manage = new JObject();
            JObject SMT_ProductionInfo1 = new JObject();
            JObject OrderNumdata1 = new JObject();
            JObject LineData1 = new JObject();
            CommonalityController comm = new CommonalityController();
            var SMT_ProductionBoard = (from c in db.SMT_ProductionBoardTable select new SMT_ProductionBoardResule { OrderNum = c.OrderNum, LineNum = c.LineNum, JobContent = c.JobContent }).ToList();

            var SMT_ProductionPlans = (from c in db.SMT_ProductionPlan select new SMT_ProductionPlansResule { OrderNum = c.OrderNum, LineNum = c.LineNum, JobContent = c.JobContent, planDateTime = c.PlanProductionDate, Quantity = c.Quantity, Capacity = c.Capacity }).ToList();

            var SMT_ProductionDatas = (from c in db.SMT_ProductionData select new SMT_ProductionDateResule { OrderNum = c.OrderNum, LineNum = c.LineNum, JobContent = c.JobContent, BeginTime = c.BeginTime, EndTime = c.EndTime, AbnormalCount = c.AbnormalCount, NormalCount = c.NormalCount }).ToList();

            var orderMug = (from c in db.OrderMgm select new OrderMgmResule { OrderNum = c.OrderNum, ID = c.ID, Models = c.Models, AdapterCard = c.AdapterCard, Powers = c.Powers, PlatformType = c.PlatformType, ProcessingRequire = c.ProcessingRequire, HandSampleScedule = c.HandSampleScedule, StandardRequire = c.StandardRequire }).ToList();

            if (ordernum != null)
            {
                SMT_ProductionBoard = SMT_ProductionBoard.Where(c => ordernum.Contains(c.OrderNum)).ToList();
            }
            if (jobContent != null)
            {
                SMT_ProductionBoard = SMT_ProductionBoard.Where(c => jobContent.Contains(c.JobContent)).ToList();
            }
            if (platformType != null)
            {
                var orderList = orderMug.Where(c => platformType.Contains(c.PlatformType)).Select(c => c.OrderNum).ToList();
                SMT_ProductionBoard = SMT_ProductionBoard.Where(c => orderList.Contains(c.OrderNum)).ToList();
            }
            var productionBoardList = SMT_ProductionBoard.Select(c => c.OrderNum).ToList();

            foreach (var Listitem in productionBoardList)
            {
                if (!SMT_ProductionInfo1.ContainsKey(Listitem))
                {
                    var planOrderNum = SMT_ProductionBoard.Where(c => c.OrderNum == Listitem);
                    int i = 1;
                    foreach (var itemPlan in planOrderNum)
                    {
                        //订单记录信息
                        var OrderMgmsInfo = orderMug.Where(c => c.OrderNum == itemPlan.OrderNum).Select(c => new { c.ID, c.Models, c.AdapterCard, c.Powers, c.PlatformType, c.StandardRequire, c.HandSampleScedule, c.ProcessingRequire }).FirstOrDefault();
                        if (OrderMgmsInfo == null)
                            continue;
                        //今天计划数
                        var todayPlanNum = SMT_ProductionPlans.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent && string.Format("{0:yyyy-MM-dd}", c.planDateTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Select(c => c.Quantity).FirstOrDefault();

                        //今天产能
                        var todayCapacity = SMT_ProductionPlans.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent && string.Format("{0:yyyy-MM-dd}", c.planDateTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Select(c => c.Capacity).FirstOrDefault();

                        //今天良品
                        var todayNormalNum = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent && string.Format("{0:yyyy-MM-dd}", c.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Count() == 0 ? 0 : SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent && string.Format("{0:yyyy-MM-dd}", c.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Sum(C => C.NormalCount);

                        //今天不良品
                        var todayAbnormalNum = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent && string.Format("{0:yyyy-MM-dd}", c.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Count() == 0 ? 0 : SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent && string.Format("{0:yyyy-MM-dd}", c.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).Sum(c => c.AbnormalCount);

                        //对应线别良品总数
                        var lineNormalNumSum = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent).Count() == 0 ? 0 : SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent).Sum(c => c.NormalCount);

                        //对应线别不良品总数
                        var lineAbnormalNumSum = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent).Count() == 0 ? 0 : SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent).Sum(c => c.AbnormalCount);

                        //对应工作内容良品总数
                        var jobContentNormalNumSum = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.JobContent == itemPlan.JobContent).Count() == 0 ? 0 : SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.JobContent == itemPlan.JobContent).Sum(c => c.NormalCount);

                        //对应工作内容不良品总数
                        var jobContentAbnormalNumSum = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.JobContent == itemPlan.JobContent).Count() == 0 ? 0 : SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.JobContent == itemPlan.JobContent).Sum(c => c.AbnormalCount);

                        //开始时间
                        var lineBegintime = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent).Min(c => c.BeginTime);
                        //结束时间
                        var lineEndTime = SMT_ProductionDatas.Where(c => c.OrderNum == itemPlan.OrderNum && c.LineNum == itemPlan.LineNum && c.JobContent == itemPlan.JobContent).Max(c => c.EndTime);



                        //模块术
                        var ModelNum = 0;
                        if (itemPlan.JobContent == "灯面" || itemPlan.JobContent == "IC面")
                        {
                            ModelNum = OrderMgmsInfo.Models;
                        }
                        else if (itemPlan.JobContent != null && itemPlan.JobContent.Contains("转接卡") == true)
                        {
                            ModelNum = OrderMgmsInfo.AdapterCard;
                        }
                        else if (itemPlan.JobContent != null && itemPlan.JobContent.Contains("电源") == true)
                        {
                            ModelNum = OrderMgmsInfo.Powers;
                        }
                        //订单ID号
                        LineData1.Add("Id", OrderMgmsInfo.ID.ToString());
                        //订单ID号
                        LineData1.Add("OrderNum", itemPlan.OrderNum);
                        //平台
                        LineData1.Add("PlatformType", OrderMgmsInfo.PlatformType);
                        //制作要求
                        LineData1.Add("ProcessingRequire", OrderMgmsInfo.ProcessingRequire);
                        //标准要求
                        LineData1.Add("StandardRequire", OrderMgmsInfo.StandardRequire);
                        //模块数
                        LineData1.Add("Models", ModelNum);
                        //工作内容
                        LineData1.Add("JobContent", itemPlan.JobContent);
                        //今天计划数
                        LineData1.Add("PlanQuantity", todayPlanNum.ToString());
                        //今天产能
                        LineData1.Add("Capacity", todayCapacity.ToString());
                        //小样进度
                        LineData1.Add("HandSampleScedule", OrderMgmsInfo.HandSampleScedule);
                        //是否有图片
                        if (comm.CheckJpgExit(itemPlan.OrderNum, "SmallSample_Files"))
                            LineData1.Add("HandSampleSceduleJpg", "false");
                        else
                            LineData1.Add("HandSampleSceduleJpg", "true");
                        //是否有PDF文件
                        if (comm.CheckpdfExit(itemPlan.OrderNum, "SmallSample_Files"))
                            LineData1.Add("HandSampleScedulePdf", "false");
                        else
                            LineData1.Add("HandSampleScedulePdf", "true");

                        var sample = db.Small_Sample.OrderBy(c => c.Id).Where(c => (c.OrderNumber == itemPlan.OrderNum || c.OrderNumber.Contains(itemPlan.OrderNum)) && c.Approved != null && c.ApprovedResult == true).ToList();
                        if (sample.Count != 0)
                        {
                            JArray sampleJarray = new JArray();
                            int k = 1;
                            foreach (var sampleitem in sample)
                            {
                                JObject sampleJobject = new JObject();
                                sampleJobject.Add("id", sampleitem.Id);
                                sampleJobject.Add("Name", "NO." + k);
                                k++;
                                sampleJarray.Add(sampleJobject);
                            }
                            LineData1.Add("HandSampleSceduleReport", sampleJarray);
                        }
                        else
                            LineData1.Add("HandSampleSceduleReport", "false");

                        //异常
                        var SMTAbnormal_Description = db.OrderMgm.Where(c => c.OrderNum == itemPlan.OrderNum).ToList().FirstOrDefault().SMTAbnormal_Description;
                        LineData1.Add("SMTAbnormal_Description", SMTAbnormal_Description);
                        if (!string.IsNullOrEmpty(SMTAbnormal_Description))
                        {
                            if (!comm.CheckJpgExit(itemPlan.OrderNum, "SMTAbnormalOrder_Files"))
                                LineData1.Add("SMTAbnormal_DescriptionJpg", "true");
                            else
                                LineData1.Add("SMTAbnormal_DescriptionJpg", "false");

                            if (!comm.CheckpdfExit(itemPlan.OrderNum, "SMTAbnormalOrder_Files"))
                                LineData1.Add("SMTAbnormal_DescriptionPdf", "true");
                            else
                                LineData1.Add("SMTAbnormal_DescriptionPdf", "false");
                        }
                        //线别
                        LineData1.Add("LineNum", itemPlan.LineNum);
                        //今天良品数
                        LineData1.Add("NormalCount", todayNormalNum);
                        //今天不良品数
                        LineData1.Add("AbnormalCount", todayAbnormalNum);
                        //今天完成率
                        LineData1.Add("TodayFinishRate", todayPlanNum == 0 ? "" : (((decimal)todayNormalNum + todayAbnormalNum) / todayPlanNum * 100).ToString("F2"));
                        //今天合格率
                        LineData1.Add("TodayPassRate", todayPlanNum == 0 || todayNormalNum + todayAbnormalNum == 0 ? "" : ((decimal)todayNormalNum / (todayNormalNum + todayAbnormalNum) * 100).ToString("F2"));
                        //对应线别良品总数
                        LineData1.Add("LineAllNomalCount", lineNormalNumSum);
                        //对应线别不良品总数
                        LineData1.Add("LineAllAbnomalCount", lineAbnormalNumSum);
                        //对应工作内容良品总数
                        LineData1.Add("AllNormalCount", jobContentNormalNumSum);
                        //对应工作内容不良品总数
                        LineData1.Add("AllAbnormalCount", jobContentAbnormalNumSum);
                        //订单总完成率
                        LineData1.Add("OrderFinishRate", jobContentAbnormalNumSum + jobContentNormalNumSum == 0 ? "" : (((decimal)(jobContentNormalNumSum + jobContentAbnormalNumSum)) / ModelNum * 100).ToString("F2"));
                        //订单总合格率
                        LineData1.Add("OrderPassRate", (jobContentAbnormalNumSum + jobContentNormalNumSum) == 0 ? "" : ((decimal)jobContentNormalNumSum / (jobContentNormalNumSum + jobContentAbnormalNumSum) * 100).ToString("F2"));
                        //开始时间
                        LineData1.Add("BeginTime", lineBegintime.ToString());
                        //结束时间
                        if (jobContentAbnormalNumSum + jobContentNormalNumSum >= ModelNum)
                        {
                            LineData1.Add("EndTime", lineEndTime.ToString());
                            LineData1.Add("TotalTime", (Convert.ToDateTime(lineEndTime) - Convert.ToDateTime(lineBegintime)).ToString());
                        }
                        else
                        {
                            LineData1.Add("EndTime", "");
                            LineData1.Add("TotalTime", "");
                        }
                        //生产用时



                        OrderNumdata1.Add(i.ToString(), JsonConvert.SerializeObject(LineData1));
                        LineData1.RemoveAll();
                        i++;
                    }
                    SMT_ProductionInfo1.Add(Listitem, JsonConvert.SerializeObject(OrderNumdata1));
                    OrderNumdata1.RemoveAll(); ;
                }
            }
            ViewBag.History = SMT_ProductionInfo1;
            return View();
        }



        #region----------SMT生产信息查找历史记录 SMT_ProductionData
        public ActionResult SMT_ProductionData()
        {
            if ((Users)Session["User"] == null)
            {
                ViewBag.display = false;
            }
            else
            {
                ViewBag.display = com.isCheckRole("SMT管理", "显示数据生产操作按钮", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum);
            }
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.LineNumList = GetLineNumList();
            ViewBag.JobContentList = GetJobContentList();
            ViewBag.OrderInfo = null;
            var records = db.SMT_ProductionData.OrderBy(c => c.LineNum).Where(c => DbFunctions.DiffDays(c.ProductionDate, DateTime.Now) <= 0).ToList();
            return View(records);
        }

        [HttpPost]
        public ActionResult SMT_ProductionData(string orderNum, int? lineNum, string jobContent, DateTime? ProductionDate)
        {
            ViewBag.display = com.isCheckRole("SMT管理", "显示数据生产操作按钮", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum);
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.LineNumList = GetLineNumList();
            ViewBag.JobContentList = GetJobContentList();

            if (!String.IsNullOrEmpty(orderNum) && lineNum == null && String.IsNullOrEmpty(jobContent) && ProductionDate == null)
            {
                ViewBag.OrderInfo = db.OrderMgm.FirstOrDefault(c => c.OrderNum == orderNum);
            }
            else
            {
                ViewBag.OrderInfo = null;
            }
            List<SMT_ProductionData> recordlist = new List<SMT_ProductionData>();
            if (String.IsNullOrEmpty(orderNum) && lineNum == null && String.IsNullOrEmpty(jobContent) && ProductionDate == null)
            {
                return View(recordlist);
            }
            recordlist = db.SMT_ProductionData.ToList();
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
            if (ProductionDate != null)
            {
                recordlist = recordlist.Where(c => c.ProductionDate.Value.Year == ProductionDate.Value.Year && c.ProductionDate.Value.Month == ProductionDate.Value.Month && c.ProductionDate.Value.Day == ProductionDate.Value.Day).ToList();
            }
            return View(recordlist);
        }
        #endregion


        #endregion


        #region------------------生产操作Operator(数据录入、修改、删除)
        // GET: SMT产线未段工位输入操作
        #region----------数据生产录入
        public ActionResult SMT_Operator()
        {
            //ViewBag.OrderNumList = JsonConvert.SerializeObject(GetTodayPlanOrderNumList());//向View传递OrderNum订单号列表.
            //前端
            //var list = JSON.parse('@Html.Raw(ViewBag.OrderNumList)');
            ViewBag.OrderNumList = GetOrderList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT", act = "SMT_Operator" });
            }
            //if (((Users)Session["User"]).Role == "SMT看板管理员" || ((Users)Session["User"]).Department == "SMT部" && ((Users)Session["User"]).Role == "主管" || com.isCheckRole("产线管理", "产线信息录入", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            return View();
            // }
            // return Content("对不起，您的不能进入产线操作，请联系SMT看板管理员！");
        }


        [HttpPost]
        public ActionResult SMT_Operator(List<SMT_ProductionData> SMT_ProductionDataList, string Department1, string Group)
        {
            //ViewBag.OrderNumList = JsonConvert.SerializeObject(GetTodayPlanOrderNumList());//向View传递OrderNum订单号列表.
            //前端
            //var list = JSON.parse('@Html.Raw(ViewBag.OrderNumList)');
            ViewBag.OrderNumList = GetOrderList();
            List<SMT_ProductionData> result = new List<SMT_ProductionData>();
            foreach (var item in SMT_ProductionDataList)
            {
                var count = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum == item.LineNum && c.JobContent == item.JobContent && DbFunctions.DiffDays(item.BeginTime, c.PlanProductionDate) == 0).Count();
                if (count == 0)
                {
                    return Content("订单" + item.OrderNum + "在产线" + item.LineNum + "没有做" + item.JobContent + "生产计划！");
                }
                else
                {
                    item.Department = Department1;
                    item.Group = Group;
                    result.Add(item);
                }
            }
            if (ModelState.IsValid)
            {
                db.SMT_ProductionData.AddRange(result);
                db.SaveChanges();
            }
            var ordernum = SMT_ProductionDataList.Select(c => c.OrderNum).Distinct().ToList();
            foreach (var orditem in ordernum)
            {
                var board = db.ModuleBoard.Where(c => c.Ordernum == orditem && c.Section == "SMT").FirstOrDefault();
                if (board == null)
                {
                    ModuleBoard moduleBoard = new ModuleBoard() { Ordernum = orditem, Section = "SMT", UpdateDate = DateTime.Now };
                    db.ModuleBoard.Add(moduleBoard);
                    db.SaveChanges();
                }
                else
                {
                    board.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                }
            }
            return Content("保存成功");//跳转到../SMT/SMT_Operator
        }


        [HttpPost]
        public ActionResult GetOrderQuantityCount(string ordernum)
        {
            if (ordernum == "")
            {
                return Content("");
            }
            //int normalCount = 0,abnormalCount=0;
            JObject result = new JObject();
            var ordernum_record = db.SMT_ProductionData.Where(c => c.OrderNum == ordernum).ToList();
            if (ordernum_record.Count == 0)
            {
                result.Add("result", "此订单无生产记录");
                return Content(JsonConvert.SerializeObject(result));
            }
            var jobContentList = ordernum_record.Select(c => c.JobContent).Distinct().ToList();
            if (jobContentList != null)
            {
                result.Add("订单模块总数：", db.OrderMgm.Where(c => c.OrderNum == ordernum).FirstOrDefault().Models);
                foreach (var jobcontent in jobContentList)
                {
                    var normalCount = ordernum_record.Where(c => c.JobContent == jobcontent).Sum(c => c.NormalCount);
                    var abnormalCount = ordernum_record.Where(c => c.JobContent == jobcontent).Sum(c => c.AbnormalCount);
                    result.Add(jobcontent + "良品数：", normalCount);
                    result.Add(jobcontent + "不良品数：", abnormalCount);
                    result.Add(jobcontent + "总数：", normalCount + abnormalCount);
                }
                return Content(JsonConvert.SerializeObject(result));
            }
            return Content("");
        }


        [HttpPost]
        public ActionResult SMT_Operator_getBeginTime(string ordernum, int linenum)   //history.go(-1);
        {
            var record = db.SMT_ProductionData.Where(c => c.OrderNum == ordernum && c.LineNum == linenum && c.EndTime.Value.Year == DateTime.Now.Year && c.EndTime.Value.Month == DateTime.Now.Month && c.EndTime.Value.Day == DateTime.Now.Day).OrderByDescending(c => c.Id).FirstOrDefault();
            string lastrecordendtime = record == null ? "" : record.EndTime.ToString();
            return Content(lastrecordendtime);
        }
        #endregion

        #region----------数据生产修改
        public async Task<ActionResult> SMT_ProductionDataEdit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT", act = "SMT_ProductionDataEdit" + "/" + id.ToString() });
            }
            //if (((Users)Session["User"]).Role == "SMT看板管理员" || ((Users)Session["User"]).Department == "SMT部" && ((Users)Session["User"]).Role == "主管" || com.isCheckRole("SMT管理", "修改SMT生产记录", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            //{
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SMT_ProductionData record = await db.SMT_ProductionData.FindAsync(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Status = ProductionLineStatus();
            ViewBag.OrderNum = GetOrderList();
            ViewBag.OrderNumValue = record.OrderNum;
            return View(record);
            //}
            //return Content("<script>alert('对不起，您的不能修改生产数据，请联系系统管理员！');history.go(-1);</script>");//window.location.href='../SMT_Mangage';
        }

        [HttpPost]
        public async Task<ActionResult> SMT_ProductionDataEdit([Bind(Include = "Id,LineNum,OrderNum,JobContent,NormalCount,AbnormalCount,BeginTime,EndTime,BarcodeNum,Result,ProductionDate,Operator")]SMT_ProductionData record)
        {
            if (ModelState.IsValid)
            {
                var old = db.SMT_ProductionData.Find(record.Id);
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "SMT生产数据修改：原数据产线、订单号、工作内容、良品数量、不良品数量、开始时间、结束时间为" + old.LineNum + "，" + old.OrderNum + "，" + old.JobContent + "," + old.NormalCount + "，" + old.AbnormalCount + "，" + old.BeginTime + "," + old.EndTime + "修改为" + record.LineNum + "，" + record.OrderNum + "，" + record.JobContent + "," + record.NormalCount + "，" + record.AbnormalCount + "，" + record.BeginTime + "," + record.EndTime };
                db.UserOperateLog.Add(log);
                old.LineNum = record.LineNum;
                old.OrderNum = record.OrderNum;
                old.JobContent = record.JobContent;
                old.NormalCount = record.NormalCount;
                old.AbnormalCount = record.AbnormalCount;
                old.BeginTime = record.BeginTime;
                old.EndTime = record.EndTime;

                await db.SaveChangesAsync();

                var board = db.ModuleBoard.Where(c => c.Ordernum == record.OrderNum && c.Section == "SMT").FirstOrDefault();
                if (board == null)
                {
                    ModuleBoard moduleBoard = new ModuleBoard() { Ordernum = record.OrderNum, Section = "SMT", UpdateDate = DateTime.Now };
                    db.ModuleBoard.Add(moduleBoard);
                    db.SaveChanges();
                }
                else
                {
                    board.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                }

                return RedirectToAction("SMT_ProductionData");
            }
            return View(record);
        }
        #endregion

        #region----------数据生产删除
        public async Task<ActionResult> SMT_ProductionDataDelete(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT", act = "SMT_ProductionDataDelete" + "/" + id.ToString() });
            }
            //if (((Users)Session["User"]).Role == "PC计划员" || com.isCheckRole("SMT管理", "删除SMT生产记录", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            // {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SMT_ProductionData record = await db.SMT_ProductionData.FindAsync(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            return View(record);
            // }
            //  return Content("<script>alert('对不起，您的不能管理产线信息，请联系系统管理员！');window.location.href='../SMT_ProductionPlan';</script>");
        }

        // POST: SMT_ProductionData/Delete/
        [HttpPost, ActionName("SMT_ProductionDataDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SMT_ProductionDataDeleteConfirmed(int id)
        {
            SMT_ProductionData record = await db.SMT_ProductionData.FindAsync(id);

            UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "SMT数据生产删除：原数据产线、订单号、工作内容、良品数量、不良品数量、开始时间、结束时间为" + record.LineNum + "，" + record.OrderNum + "，" + record.JobContent + "," + record.NormalCount + "，" + record.AbnormalCount + "，" + record.BeginTime + "," + record.EndTime };
            db.UserOperateLog.Add(log);
            db.SMT_ProductionData.Remove(record);
            var board = db.ModuleBoard.Where(c => c.Ordernum == record.OrderNum && c.Section == "SMT").FirstOrDefault();
            if (board == null)
            {
                ModuleBoard moduleBoard = new ModuleBoard() { Ordernum = record.OrderNum, Section = "SMT", UpdateDate = DateTime.Now };
                db.ModuleBoard.Add(moduleBoard);
                db.SaveChanges();
            }
            else
            {
                board.UpdateDate = DateTime.Now;
                db.SaveChanges();
            }
            await db.SaveChangesAsync();
            return Content("<script>alert('SMT生产数据已经成功删除！');window.location.href='../SMT_ProductionData';</script>");
        }
        #endregion

        #endregion


        #region------------------走廊显示屏
        public ActionResult SMT_CorridorView()
        {
            return View();
        }
        #endregion


        #region --------------------FinishStatus列表
        private List<SelectListItem> FinishStatusList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "完成",
                    Value = "完成"
                },
                new SelectListItem
                {
                    Text = "未完成",
                    Value = "未完成"
                }
            };
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


        #region --------------------创建角色列表
        private List<SelectListItem> createRoleList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "操作员",
                    Value = "操作员"
                },
                new SelectListItem
                {
                    Text = "SMT看板管理员",
                    Value = "SMT看板管理员"
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
        public ActionResult GetOrderArr()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);
                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
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

        public ActionResult GetLineNumArr()
        {
            var lineNumList = db.SMT_ProductionLineInfo.OrderBy(m => m.LineNum).Select(m => m.LineNum).ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in lineNumList)
            {
                JObject List = new JObject();
                List.Add("value", item);
                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
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
        public ActionResult GetJobContentArr()
        {
            var jobContentList = db.SMT_ProductionPlan.Select(m => m.JobContent).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in jobContentList)
            {
                JObject List = new JObject();
                List.Add("value", item);
                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
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

        #region -------------------------SMT看板IC面看板
        public ActionResult DisplayICContenInfo()
        {
            //if (Session["User"] == null)
            //{
            //    return RedirectToAction("Login", "Users", new { col = "SMT", act = "DisplayICContenInfo" });
            //};
            return View();
        }
        [HttpPost]
        public ActionResult DisplayICContenInfo(DateTime? time)
        {
            int lintPlan = 0;
            int lineshiji = 0;
            int lineaccess = 0;
            List<Temp> efficiency = new List<Temp>();
            List<Temp> quality = new List<Temp>();
            JObject result = new JObject();
            var addtime = new DateTime();
            if (time == null)
            {
                time = DateTime.Now.Date;
                addtime = time.Value.AddDays(1);
            }
            else
            {
                time = time.Value.Date;
                addtime = time.Value.AddDays(1).AddHours(8);
            }
            var time2 = time.Value.AddHours(8);
            JArray content = new JArray();
            for (var i = 1; i < 8; i++)
            {
                var status = db.SMT_ProductionLineInfo.Where(c => c.LineNum == i).Select(c => c.Status).FirstOrDefault();
                var plan = db.SMT_ProductionPlan.Where(c => c.LineNum == i && c.PlanProductionDate == time).Select(c => c.OrderNum).ToList();

                JObject item = new JObject();
                JArray orderJarray = new JArray();
                //线别
                item.Add("Line", i);
                int orderplannum = 0;
                int ordernum = 0;
                int orderaccessnum = 0;
                foreach (var itemorder in plan)
                {
                    JObject ordercontent = new JObject();
                    var totalordernum = db.OrderMgm.Where(c => c.OrderNum == itemorder).Select(c => c.Models).FirstOrDefault();
                    var planNum = db.SMT_ProductionPlan.Where(c => c.LineNum == i && c.PlanProductionDate == time && c.OrderNum == itemorder).Select(c => c.Quantity).FirstOrDefault();
                    var date = db.SMT_ProductionData.Where(c => c.LineNum == i && c.BeginTime >= time2 && c.EndTime <= addtime && c.OrderNum == itemorder).ToList();
                    var num = 0;
                    var notnum = 0;
                    foreach (var dateitem in date)
                    {
                        num = num + dateitem.AbnormalCount + dateitem.NormalCount;
                        notnum = notnum + dateitem.AbnormalCount;
                    }
                    var access = num - notnum;
                    orderaccessnum = orderaccessnum + access;
                    //订单号
                    ordercontent.Add("OrderNum", itemorder);
                    //生产状态
                    ordercontent.Add("Statu", status);
                    //订单总数
                    ordercontent.Add("totalNum", totalordernum);
                    //目标投入
                    ordercontent.Add("targetNum", planNum);
                    orderplannum = orderplannum + planNum;
                    //实际产出
                    ordercontent.Add("actualNum", num);
                    ordernum = ordernum + num;
                    //累计产出
                    var cumulative = db.SMT_ProductionData.Where(c => c.LineNum == i && c.EndTime <= addtime && c.OrderNum == itemorder).ToList();
                    var cumulativenum = 0;
                    cumulative.ForEach(c => cumulativenum = cumulativenum + c.AbnormalCount + c.NormalCount);
                    ordercontent.Add("cumulative", cumulativenum);
                    //达成率
                    var pass = planNum == 0 ? 0 : ((decimal)num * 100 / planNum);
                    ordercontent.Add("PassRate", Math.Round(Convert.ToDecimal(pass), 2));
                    //不良数
                    ordercontent.Add("AbnormalCount", notnum);
                    //良率
                    var yield = num == 0 ? 0 : ((decimal)access * 100 / num);
                    ordercontent.Add("accessRate", Math.Round(Convert.ToDecimal(yield), 2));
                    orderJarray.Add(ordercontent);

                    lintPlan = lintPlan + planNum;
                    lineshiji = lineshiji + num;
                    lineaccess = lineaccess + access;

                }
                item.Add("Content", orderJarray);

                if (status == "生产中")
                {
                    Temp eff = new Temp() { line = i + "线", value = Math.Round(Convert.ToDecimal(orderplannum == 0 ? 0 : ((decimal)ordernum * 100 / orderplannum)), 2) };
                    Temp qua = new Temp() { line = i + "线", value = Math.Round(Convert.ToDecimal(ordernum == 0 ? 0 : ((decimal)orderaccessnum * 100 / ordernum)), 2) };
                    efficiency.Add(eff);
                    quality.Add(qua);
                }
                content.Add(item);
            }

            result.Add("achievementRate", lintPlan == 0 ? 0 : Math.Round(Convert.ToDecimal((decimal)lineshiji * 100 / lintPlan), 2));
            result.Add("ProductionYield", lineshiji == 0 ? 0 : Math.Round(Convert.ToDecimal((decimal)lineaccess * 100 / lineshiji), 2));
            var lasteff = efficiency.OrderBy(c => c.value).Where(c => c.value < 98).Take(3).ToList();
            if (lasteff.Count > 3)
            {
                lasteff = lasteff.Take(3).ToList();
            }
            var lastqua = quality.OrderBy(c => c.value).Take(3).ToList();
            var count = lasteff.Count;
            JObject effjobject = new JObject();
            JObject quajobject = new JObject();
            JArray line = new JArray();
            JArray value = new JArray();
            JArray line2 = new JArray();
            JArray value2 = new JArray();
            foreach (var eff in lasteff)
            {
                line.Add(eff.line);
                value.Add(eff.value);
            }
            foreach (var qua in lastqua)
            {
                line2.Add(qua.line);
                value2.Add(qua.value);
            }
            effjobject.Add("line", line);
            effjobject.Add("value", value);
            quajobject.Add("line", line2);
            quajobject.Add("value", value2);
            result.Add("efficiency", effjobject);
            result.Add("quality", quajobject);
            result.Add("content", content);
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region -------------------------SMT看板灯面看板
        public ActionResult DisplayLightContenInfo()
        {
            //if (Session["User"] == null)
            //{
            //    return RedirectToAction("Login", "Users", new { col = "SMT", act = "DisplayLightContenInfo" });
            //};
            return View();
        }
        [HttpPost]
        public ActionResult DisplayLightContenInfo(DateTime? time)
        {
            int lintPlan = 0;
            int lineshiji = 0;
            int production = 0;
            int notproduction = 0;
            List<Temp> efficiency = new List<Temp>();
            List<Temp> quality = new List<Temp>();
            JObject result = new JObject();
            var addtime = new DateTime();
            if (time == null)
            {
                time = DateTime.Now.Date;
                addtime = time.Value.AddDays(1);
            }
            else
            {
                time = time.Value.Date;
                addtime = time.Value.AddDays(1).AddHours(8);
            }
            var time2 = time.Value.AddHours(8);
            JArray content = new JArray();
            for (var i = 8; i < 15; i++)
            {
                var info = db.SMT_ProductionLineInfo.Where(c => c.LineNum == i).FirstOrDefault();
                var plan = db.SMT_ProductionPlan.Where(c => c.LineNum == i && c.PlanProductionDate == time).Select(c => c.OrderNum).ToList();



                JArray orderjarry = new JArray();
                JObject item = new JObject();
                //线别
                item.Add("Line", i);
                int orderplannum = 0;
                int ordernum = 0;
                int ordernotnum = 0;
                int ordetverr = 0;
                foreach (var itemorder in plan)
                {
                    JObject ordercontent = new JObject();
                    var verr = 0;
                    var totalordernum = db.OrderMgm.Where(c => c.OrderNum == itemorder).Select(c => c.Models).FirstOrDefault();
                    var planNum = db.SMT_ProductionPlan.Where(c => c.LineNum == i && c.PlanProductionDate == time && c.OrderNum == itemorder).Select(c => c.Quantity).FirstOrDefault();
                    var date = db.SMT_ProductionData.Where(c => c.LineNum == i && c.BeginTime >= time2 && c.EndTime <= addtime && c.OrderNum == itemorder).ToList();
                    var num = 0;
                    var notnum = 0;
                    foreach (var dateitem in date)
                    {
                        num = num + dateitem.AbnormalCount + dateitem.NormalCount;
                        notnum = notnum + dateitem.AbnormalCount;
                        verr = dateitem.VeneerPoints;
                    }
                    var access = num - notnum;
                    ordernotnum = ordernotnum + notnum;
                    //订单号
                    ordercontent.Add("OrderNum", itemorder);
                    //生产状态
                    ordercontent.Add("Statu", info.Status);
                    //订单总数
                    ordercontent.Add("totalNum", totalordernum);
                    //目标投入
                    ordercontent.Add("targetNum", planNum);
                    orderplannum = orderplannum + planNum;
                    //实际产出
                    ordercontent.Add("actualNum", num);
                    ordernum = ordernum + num;
                    //累计产出
                    var cumulative = db.SMT_ProductionData.Where(c => c.LineNum == i && c.EndTime <= addtime && c.OrderNum == itemorder).ToList();
                    var cumulativenum = 0;
                    cumulative.ForEach(c => cumulativenum = cumulativenum + c.AbnormalCount + c.NormalCount);
                    ordercontent.Add("cumulative", cumulativenum);
                    //达成率
                    var pass = planNum == 0 ? 0 : ((decimal)num * 100 / planNum);
                    ordercontent.Add("PassRate", Math.Round(Convert.ToDecimal(pass), 2));
                    //单板点数
                    ordercontent.Add("VeneerPoints", verr);
                    ordetverr = ordetverr + verr;
                    //不良数点数
                    ordercontent.Add("AbnormalCount", notnum);
                    //DPPM
                    var yield = num == 0 || verr == 0 ? 0 : ((decimal)notnum / (num * verr) * 1000000);
                    ordercontent.Add("DPPM", Math.Round(Convert.ToDecimal(yield), 2));
                    orderjarry.Add(ordercontent);

                    lintPlan = lintPlan + planNum;
                    lineshiji = lineshiji + num;
                    production = production + verr * num;
                    notproduction = notproduction + notnum;
                }
                if (info.Status == "生产中")
                {
                    Temp eff = new Temp() { line = i + "线", value = Math.Round(Convert.ToDecimal(orderplannum == 0 ? 0 : ((decimal)ordernum * 100 / orderplannum)), 2) };
                    Temp qua = new Temp() { line = i + "线", value = Math.Round(Convert.ToDecimal(ordernum == 0 || ordetverr == 0 ? 0 : ((decimal)ordernotnum / (ordernum * ordetverr) * 1000000)), 2) };
                    efficiency.Add(eff);
                    quality.Add(qua);
                }
                item.Add("Content", orderjarry);
                content.Add(item);
            }

            result.Add("achievementRate", lintPlan == 0 ? 0 : Math.Round(Convert.ToDecimal((decimal)lineshiji * 100 / lintPlan), 2));
            result.Add("ProductionYield", production == 0 ? 0 : Math.Round(Convert.ToDecimal((decimal)notproduction / production * 1000000), 2));
            var lasteff = efficiency.OrderBy(c => c.value).Where(c => c.value < 95).ToList();
            if (lasteff.Count > 3)
            {
                lasteff = lasteff.Take(3).ToList();
            }
            var lastqua = quality.OrderByDescending(c => c.value).Take(3).ToList();

            JObject effjobject = new JObject(), quajobject = new JObject();
            JArray line = new JArray(), value = new JArray(), line2 = new JArray(), value2 = new JArray();
            foreach (var eff in lasteff)
            {
                line.Add(eff.line);
                value.Add(eff.value);
            }
            foreach (var qua in lastqua)
            {
                line2.Add(qua.line);
                value2.Add(qua.value);
            }
            effjobject.Add("line", line);
            effjobject.Add("value", value);
            quajobject.Add("line", line2);
            quajobject.Add("value", value2);
            result.Add("efficiency", effjobject);
            result.Add("quality", quajobject);
            result.Add("content", content);
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        //查找SMT历史记录的工作内容列表
        private List<SelectListItem> jobContentList()
        {
            var orders = db.SMT_ProductionBoardTable.Select(m => m.JobContent).Distinct().ToList();
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

        //查找SMT历史记录的订单列表
        private List<SelectListItem> GetSMTOrderList()
        {
            var orders = db.SMT_ProductionBoardTable.Select(m => m.OrderNum).Distinct();    //增加.Distinct()后会重新按OrderNum升序排序
            var items = new List<SelectListItem>();
            foreach (string order in orders)
            {
                if (order == "2017-TEST-1")
                {
                    continue;
                }
                items.Add(new SelectListItem
                {
                    Text = order,
                    Value = order
                });
            }
            return items;
        }


        internal class SMT_ProductionPlansResule
        {
            public string OrderNum { get; set; }
            public int LineNum { get; set; }
            public string JobContent { get; set; }

            public DateTime? planDateTime { get; set; }

            public int Quantity { get; set; }

            public decimal Capacity { get; set; }
        }

        internal class SMT_ProductionDateResule
        {
            public string OrderNum { get; set; }
            public int LineNum { get; set; }
            public string JobContent { get; set; }
            public DateTime? BeginTime { get; set; }
            public DateTime? EndTime { get; set; }
            public int NormalCount { get; set; }
            public int AbnormalCount { get; set; }

        }

        internal class SMT_ProductionBoardResule
        {
            public string OrderNum { get; set; }
            public int LineNum { get; set; }
            public string JobContent { get; set; }

        }

        internal class OrderMgmResule
        {
            public string OrderNum { get; set; }
            public int ID { get; set; }
            public int Models { get; set; }
            public int AdapterCard { get; set; }

            public int Powers { get; set; }

            public string PlatformType { get; set; }

            public string StandardRequire { get; set; }

            public string HandSampleScedule { get; set; }

            public string ProcessingRequire { get; set; }
        }

    }

    public class SMT_ApiController : System.Web.Http.ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        CommonController com = new CommonController();


        #region 临时类
        public class Temp
        {
            public string line { get; set; }

            public decimal value { get; set; }
        }
        #endregion

        #region------------------产线状态看板
        //查看
        [HttpPost]
        [ApiAuthorize]
        public JObject SMT_Manage()
        {
            JArray result = new JArray();
            for (int i = 1; i <= 14; i++)
            {
                JObject item = new JObject();
                var info = db.SMT_ProductionLineInfo.Where(c => c.LineNum == i).FirstOrDefault();
                item.Add("Line", i); //线别
                item.Add("Ordernum", info.ProducingOrderNum);//订单
                item.Add("Status", info.Status);//状态
                item.Add("Team", info.Team);//班次
                item.Add("GroupLeader", info.GroupLeader);//组长
                result.Add(item);
            }
            return com.GetModuleFromJarray(result);
        }
        //修改(产线和创建时间不能修改)
        [HttpPost]
        [ApiAuthorize]
        public void ProductionLineEdit([System.Web.Http.FromBody]JObject data)
        {
            SMT_ProductionLineInfo info = JsonConvert.DeserializeObject<SMT_ProductionLineInfo>(JsonConvert.SerializeObject(data["info"]));
            string UserName = data["UserName"].ToString();
            var value = db.SMT_ProductionLineInfo.Where(c => c.LineNum == info.LineNum).FirstOrDefault();

            UserOperateLog log = new UserOperateLog() { Operator = UserName, OperateDT = DateTime.Now, OperateRecord = "SMT产线信息修改：原信息产线、订单号、创建记录日期、班组组长、产线状态为" + value.LineNum + "," + value.ProducingOrderNum + "," + value.CreateDate + "," + value.Team + "," + value.GroupLeader + "," + value.Status + "修改为," + info.LineNum + "," + info.ProducingOrderNum + "," + info.CreateDate + "," + info.Team + "," + info.GroupLeader + "," + info.Status };
            db.UserOperateLog.Add(log);
            value.ProducingOrderNum = info.ProducingOrderNum;
            value.Team = info.Team;
            value.GroupLeader = info.GroupLeader;
            value.Status = info.Status;
            db.SaveChanges();

        }

        /// <summary>
        /// 点击线别查看详细生产情况
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject SMT_ProductionLineInfo([System.Web.Http.FromBody]JObject data)
        {
            //内容:痴线号，时间，班组，组长，正在生产的订单，良品数量，不良品数量，不良率，产线累计个数，订单完成率，今天计划订单，产线状态
            //
            int LineNum = int.Parse(data["LineNum"].ToString());
            JObject lineBoardDay = new JObject();
            JObject lineBoardNight = new JObject();
            JObject day = new JObject();
            JObject night = new JObject();
            JObject result = new JObject();

            var productionDataTime = new List<SMT_ProductionData>();
            if (DateTime.Now.Hour >= 8)
            {
                var daystart = DateTime.Now.Date.AddHours(8);
                var dayend = DateTime.Now.Date.AddHours(23);
                productionDataTime = db.SMT_ProductionData.Where(c => c.LineNum == LineNum && c.BeginTime >= daystart && c.BeginTime <= dayend).OrderBy(c => c.BeginTime).ToList();
            }
            else
            {
                DateTime? yestoday = DateTime.Now.AddDays(-1).Date.AddHours(8);
                productionDataTime = db.SMT_ProductionData.Where(c => c.LineNum == LineNum && c.BeginTime <= DateTime.Now && c.BeginTime >= yestoday).OrderBy(c => c.BeginTime).ToList();
            }
            decimal totalCapacity = 0;
            int totalNormalCount = 0;
            int totalAbnormalCount = 0;
            int total = 0;
            int daynum = 0;
            int nightnum = 0;

            foreach (var time in productionDataTime)
            {
                if (time.BeginTime == null || time.EndTime == null)
                {
                    continue;
                }
                //是否是今天
                bool istoday = string.Format("{0:yyyy-MM-dd}", time.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                //是否是昨天
                bool isyestoday = string.Format("{0:yyyy-MM-dd}", time.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(-1));
                //时数间隔
                double num = (time.EndTime - time.BeginTime).Value.TotalHours;


                //白班生产时段记录
                if (time.BeginTime.Value.Hour >= 8 && time.BeginTime.Value.Hour < 20)
                {
                    //订单号
                    lineBoardDay.Add("ordernmu", time.OrderNum);
                    //制作面
                    lineBoardDay.Add("jobContent", time.JobContent);
                    //time.BeginTime.Value.Hour + ":" + time.BeginTime.Value.Minute
                    lineBoardDay.Add("ProductionBegin", string.Format("{0:HH:mm}", time.BeginTime));
                    lineBoardDay.Add("ProductionEnd", string.Format("{0:HH:mm}", time.EndTime));

                    //标准产能
                    var Capacity = db.SMT_ProductionPlan.Where(c => c.OrderNum == time.OrderNum && c.LineNum == LineNum).Select(c => c.Capacity).FirstOrDefault();
                    lineBoardDay.Add("Capacity", Capacity);
                    totalCapacity = totalCapacity + Capacity * Decimal.Parse(num.ToString());

                    //实际完成
                    lineBoardDay.Add("NormalCount", time.NormalCount);
                    totalNormalCount = totalNormalCount + time.NormalCount;

                    //目的达成率
                    lineBoardDay.Add("achievement", Capacity == 0 ? "" : (time.NormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                    //不良数
                    lineBoardDay.Add("AbnormalCount", time.AbnormalCount);
                    totalAbnormalCount = totalAbnormalCount + time.AbnormalCount;

                    //不良率
                    lineBoardDay.Add("Defective", Capacity == 0 ? "" : (time.AbnormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                    //备注
                    var remark = db.SMT_ProductionPlan.Where(c => c.OrderNum == time.OrderNum && c.LineNum == LineNum).Select(c => c.Remark).FirstOrDefault();
                    lineBoardDay.Add("Remark", remark);

                    day.Add(daynum.ToString(), lineBoardDay);
                    daynum++;
                    total++;
                    lineBoardDay = new JObject();
                }
                //晚班生产记录
                else
                {
                    //今天晚上20-24
                    if (time.BeginTime.Value.Hour >= 20 || time.BeginTime.Value.Hour < 8)
                    {
                        lineBoardNight.Add("ProductionBegin", time.BeginTime.Value.Hour + ":" + time.BeginTime.Value.Minute);
                        lineBoardNight.Add("ProductionEnd", time.EndTime.Value.Hour + ":" + time.EndTime.Value.Minute);
                    }
                    else
                    {
                        continue;
                    }
                    //订单号
                    lineBoardNight.Add("ordernmu", time.OrderNum);
                    //制作面
                    lineBoardNight.Add("jobContent", time.JobContent);

                    //标准产能
                    var Capacity = db.SMT_ProductionPlan.Where(c => c.OrderNum == time.OrderNum && c.LineNum == LineNum).Select(c => c.Capacity).FirstOrDefault();
                    lineBoardNight.Add("Capacity", Capacity);
                    totalCapacity = totalCapacity + Capacity * Decimal.Parse(num.ToString());

                    //实际完成
                    lineBoardNight.Add("NormalCount", time.NormalCount);
                    totalNormalCount = totalNormalCount + time.NormalCount;

                    //目的达成率
                    lineBoardNight.Add("achievement", Capacity == 0 ? "" : (time.NormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                    //不良数
                    lineBoardNight.Add("AbnormalCount", time.AbnormalCount);
                    totalAbnormalCount = totalAbnormalCount + time.AbnormalCount;

                    //不良率
                    lineBoardNight.Add("Defective", Capacity == 0 ? "" : (time.AbnormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                    //备注
                    var remark = db.SMT_ProductionPlan.Where(c => c.OrderNum == time.OrderNum && c.LineNum == LineNum).Select(c => c.Remark).FirstOrDefault();
                    lineBoardNight.Add("Remark", remark);

                    night.Add(nightnum.ToString(), lineBoardNight);
                    nightnum++;
                    total++;
                    lineBoardNight = new JObject();
                }
            }
            //白班的数据集
            result.Add("day", day);
            //晚班的数据集
            result.Add("night", night);
            //生产日期
            result.Add("todaty", string.Format("{0:yyyy-MM-dd}", DateTime.Now));
            //班组
            var groupLeader = db.SMT_ProductionLineInfo.Where(c => c.LineNum == LineNum).Select(c => c.GroupLeader).FirstOrDefault();
            result.Add("groupLeader", groupLeader);
            //生产人数
            result.Add("personNum", "");
            //统计
            result.Add("totalNum", total.ToString());
            //计划完成数
            result.Add("totalCapacity", totalCapacity);
            //实际总完成
            result.Add("totalNormalCount", totalNormalCount);
            //总完成率
            result.Add("totalAchievement", totalCapacity == 0 ? "0" : (totalNormalCount / totalCapacity * 100).ToString("F2") + "%");
            //不良总数
            result.Add("totalAbnormalCount", totalAbnormalCount);
            //总不良率
            result.Add("totalDefective", totalCapacity == 0 ? "0" : (totalAbnormalCount / totalCapacity * 100).ToString("F2") + "%");

            return com.GetModuleFromJobjet(result);
        }

        /// <summary>
        /// 点击线别查看详细生产情况历史
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject SMT_ProductionLineInfoHistory([System.Web.Http.FromBody]JObject data)
        {
            //string OrderNum, int LineNum, DateTime? startTime, DateTime? endtime, string jobcontent
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string OrderNum = datavalue.OrderNum;
            string jobcontent = datavalue.jobcontent;
            int LineNum = datavalue.LineNum;
            DateTime? startTime = datavalue.startTime;
            DateTime? endtime = datavalue.v;

            JObject smtInfo = new JObject();
            JObject smtInfoList = new JObject();
            JObject message = new JObject();
            JObject result = new JObject();
            decimal totalCapacity = 0;
            int totalNormalCount = 0;
            int totalAbnormalCount = 0;
            var Message = new List<SMT_ProductionData>();

            //第一种查询，输入线别和开始日期
            if (LineNum != 0 && startTime != null)
            {

                if (endtime == null)
                {
                    var twoTime = startTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    Message = db.SMT_ProductionData.Where(c => c.LineNum == LineNum && c.BeginTime >= startTime && c.BeginTime <= twoTime).ToList();
                }
                else
                {
                    Message = db.SMT_ProductionData.Where(c => c.LineNum == LineNum && c.BeginTime >= startTime && c.BeginTime <= endtime).ToList();
                }

                var timeSpan = Message.Select(c => c.BeginTime.Value.Date).Distinct().ToList();
                int total = 0;
                foreach (var time in timeSpan)
                {
                    DateTime nightTime = time.AddHours(23).AddMinutes(59).AddSeconds(59);
                    var smtMessage = Message.Where(c => c.BeginTime >= time && c.BeginTime <= nightTime).ToList();
                    int i = 0;
                    foreach (var item in smtMessage)
                    {
                        double num = (item.EndTime - item.BeginTime).Value.TotalHours;
                        //线别
                        smtInfo.Add("LineNum", item.LineNum);
                        //生产时段
                        smtInfo.Add("startime", string.Format("{0:yyyy/MM/dd HH:mm}", item.BeginTime));
                        smtInfo.Add("endtime", string.Format("{0:yyyy/MM/dd HH:mm}", item.EndTime));
                        //制作面
                        smtInfo.Add("jobContent", item.JobContent);
                        //订单号
                        smtInfo.Add("orderNum", item.OrderNum);
                        //标准产能
                        var Capacity = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum == LineNum).Select(c => c.Capacity).FirstOrDefault();
                        totalCapacity = totalCapacity + Capacity * Decimal.Parse(num.ToString());
                        smtInfo.Add("Capacity", Capacity);

                        //实际完成
                        smtInfo.Add("NormalCount", item.NormalCount);
                        totalNormalCount = totalNormalCount + item.NormalCount;

                        //目的达成率
                        smtInfo.Add("achievement", Capacity == 0 ? "" : (item.NormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                        //不良数
                        smtInfo.Add("AbnormalCount", item.AbnormalCount);
                        totalAbnormalCount = totalAbnormalCount + item.AbnormalCount;

                        //不良率
                        smtInfo.Add("Defective", Capacity == 0 ? "" : (item.AbnormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                        //备注
                        var remark = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum == LineNum).Select(c => c.Remark).FirstOrDefault();
                        smtInfo.Add("Remark", remark);

                        smtInfoList.Add(i.ToString(), smtInfo);
                        i++;
                        total++;
                        smtInfo = new JObject();
                    }
                    message.Add(time.ToString(), smtInfoList);
                    smtInfoList = new JObject();
                }
                //数据集
                result.Add("message", message);
                message = new JObject();
                //统计
                result.Add("totalNum", total.ToString());
                //计划完成数
                result.Add("totalCapacity", totalCapacity);
                //实际总完成
                result.Add("totalNormalCount", totalNormalCount);
                //总完成率
                result.Add("totalAchievement", totalCapacity == 0 ? "0" : (totalNormalCount / totalCapacity * 100).ToString("F2") + "%");
                //不良总数
                result.Add("totalAbnormalCount", totalAbnormalCount);
                //总不良率
                result.Add("totalDefective", totalCapacity == 0 ? "0" : (totalAbnormalCount / totalCapacity * 100).ToString("F2") + "%");
            }
            //第二种查询，输入订单号和制程面
            else
            {
                if (OrderNum != null && jobcontent != null)
                {
                    Message = db.SMT_ProductionData.Where(c => c.OrderNum == OrderNum && c.JobContent == jobcontent).OrderBy(c => c.OrderNum).ToList();
                }
                if (OrderNum == null && jobcontent != null)
                {
                    Message = db.SMT_ProductionData.Where(c => c.JobContent == jobcontent).OrderBy(c => c.OrderNum).ToList();
                }
                if (OrderNum != null && jobcontent == null)
                {
                    Message = db.SMT_ProductionData.Where(c => c.OrderNum == OrderNum).OrderBy(c => c.OrderNum).ToList();
                }
                int j = 0;
                foreach (var item in Message)
                {
                    double num = (item.EndTime - item.BeginTime).Value.TotalHours;
                    //线别
                    smtInfo.Add("LineNum", item.LineNum);
                    //生产时段
                    smtInfo.Add("startime", string.Format("{0:yyyy/MM/dd HH:mm}", item.BeginTime));
                    smtInfo.Add("endtime", string.Format("{0:yyyy/MM/dd HH:mm}", item.EndTime));
                    //制作面
                    smtInfo.Add("jobContent", item.JobContent);
                    //订单号
                    smtInfo.Add("orderNum", item.OrderNum);
                    //标准产能
                    var Capacity = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.JobContent == jobcontent).Select(c => c.Capacity).FirstOrDefault();
                    totalCapacity = totalCapacity + Capacity * Decimal.Parse(num.ToString());
                    smtInfo.Add("Capacity", Capacity);

                    //实际完成
                    smtInfo.Add("NormalCount", item.NormalCount);
                    totalNormalCount = totalNormalCount + item.NormalCount;

                    //目的达成率
                    smtInfo.Add("achievement", Capacity == 0 ? "" : (item.NormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                    //不良数
                    smtInfo.Add("AbnormalCount", item.AbnormalCount);
                    totalAbnormalCount = totalAbnormalCount + item.AbnormalCount;

                    //不良率
                    smtInfo.Add("Defective", Capacity == 0 ? "" : (item.AbnormalCount * 100 / (Capacity * Decimal.Parse(num.ToString()))).ToString("F2") + "%");

                    //备注
                    var remark = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum == LineNum).Select(c => c.Remark).FirstOrDefault();
                    smtInfo.Add("Remark", remark);

                    message.Add(j.ToString(), smtInfo);
                    j++;
                    smtInfo = new JObject();
                }
                result.Add("message", message);
                message = new JObject();
                //统计
                result.Add("totalNum", j.ToString());
                //计划完成数
                result.Add("totalCapacity", totalCapacity);
                //实际总完成
                result.Add("totalNormalCount", totalNormalCount);
                //总完成率
                result.Add("totalAchievement", totalCapacity == 0 ? "0" : (totalNormalCount / totalCapacity * 100).ToString("F2") + "%");
                //不良总数
                result.Add("totalAbnormalCount", totalAbnormalCount);
                //总不良率
                result.Add("totalDefective", totalCapacity == 0 ? "0" : (totalAbnormalCount / totalCapacity * 100).ToString("F2") + "%");
            }
            return com.GetModuleFromJobjet(result);
        }
        #endregion

        #region---------------------产线计划管理 SMT_ProductionPlan
        /// <summary>
        /// SMT计划首页
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject SMT_ProductionPlan([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string orderNum = datavalue.orderNum;
            string jobContent = datavalue.jobContent;
            string remark = datavalue.remark;
            int? lineNum = datavalue.lineNum;
            DateTime? planProductionDate = datavalue.planProductionDate;
            //string orderNum, int? lineNum, string jobContent, DateTime? planProductionDate, string remark
            JArray result = new JArray();
            var smtplanvalue = db.SMT_ProductionPlan.Select(c => new { c.LineNum, c.OrderNum, c.JobContent, c.Quantity, c.Capacity, c.PlanProductionDate, c.Remark, c.Id }).ToList();
            if (!string.IsNullOrEmpty(orderNum))
            {
                smtplanvalue = smtplanvalue.Where(c => c.OrderNum == orderNum).ToList();
            }
            if (lineNum != null)
            {
                smtplanvalue = smtplanvalue.Where(c => c.LineNum == lineNum).ToList();
            }
            if (!string.IsNullOrEmpty(jobContent))
            {
                smtplanvalue = smtplanvalue.Where(c => c.JobContent == jobContent).ToList();
            }
            if (planProductionDate != null)
            {
                smtplanvalue = smtplanvalue.Where(c => c.PlanProductionDate == planProductionDate).ToList();
            }
            foreach (var item in smtplanvalue)
            {
                JObject obj = new JObject();
                obj.Add("Id", item.Id);
                obj.Add("LineNum", item.LineNum);
                obj.Add("OrderNum", item.OrderNum);
                obj.Add("JobContent", item.JobContent);
                obj.Add("Capacity", item.Capacity);
                obj.Add("Quantity", item.Quantity);
                obj.Add("CreateDate", string.Format("{0:yyyy-MM-dd }", item.PlanProductionDate));
                obj.Add("Remark", item.Remark);
                result.Add(obj);
            }
            return com.GetModuleFromJarray(result);
        }

        /// <summary>
        /// SMT计划创建前判断
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject SMT_ProductionPlanCreateCheck([System.Web.Http.FromBody]JObject data)
        {
            List<SMT_ProductionPlan> orders = JsonConvert.DeserializeObject<List<SMT_ProductionPlan>>(JsonConvert.SerializeObject(data));
            foreach (var item in orders)
            {
                //string b = string.Format("{0:yyyy-MM-dd}", c.PlanProductionDate);
                var IsExistOrder = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum == item.LineNum && c.JobContent == item.JobContent && c.PlanProductionDate == item.PlanProductionDate).Count();
                if (IsExistOrder > 0)
                {
                    return com.GetModuleFromJarray(null, false, item.LineNum + "_" + item.OrderNum + "_" + item.JobContent + "已存在SMT计划");
                }
                else
                {
                    if (db.OrderMgm.Count(c => c.OrderNum == item.OrderNum) == 0)
                    {
                        return com.GetModuleFromJarray(null, false, item.OrderNum + "此订单号不存在");
                    }
                }
            }
            return com.GetModuleFromJarray(null, true, "OK");

        }

        /// <summary>
        /// SMT计划创建(部门班组往数据里面放)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public void SMT_ProductionPlanCreate([System.Web.Http.FromBody]JObject data)
        {
            List<SMT_ProductionPlan> orders = JsonConvert.DeserializeObject<List<SMT_ProductionPlan>>(JsonConvert.SerializeObject(data));

            orders.ForEach(c => c.CreateDate = DateTime.Now);
            db.SMT_ProductionPlan.AddRange(orders);
            db.SaveChanges();

        }
        /// <summary>
        /// SMT计划修改 (只能修改产能,数量.备注)
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject SMT_ProductionPlanEdit([System.Web.Http.FromBody]JObject data)
        {
            int Id = int.Parse(data["Id"].ToString());
            int Quantity = int.Parse(data["Quantity"].ToString());
            decimal Capacity = decimal.Parse(data["Capacity"].ToString());
            string Remark = data["Remark"].ToString();
            string UserName = data["UserName"].ToString();
            var old = db.SMT_ProductionPlan.Find(Id);
            if (old != null)
            {
                UserOperateLog log = new UserOperateLog() { Operator = UserName, OperateDT = DateTime.Now, OperateRecord = "SMT计划修改：" + old.LineNum + "线，" + old.OrderNum + "，" + old.JobContent + "，" + old.PlanProductionDate + "，产能" + old.Capacity + ",数量" + old.Quantity + ",备注" + old.Remark + "修改为" + Capacity + "," + Quantity + "，" + "," + Remark };
                db.UserOperateLog.Add(log);

                old.Capacity = Capacity;
                old.Quantity = Quantity;
                old.Remark = Remark;
                db.SaveChangesAsync();
                return com.GetModuleFromJarray(null, true, "修改成功");
            }
            return com.GetModuleFromJarray(null, false, "找不到此数据");

        }

        /// <summary>
        /// SMT计划删除
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public JObject SMT_ProductionPlanBatchDelConfirmed([System.Web.Http.FromBody]JObject data)
        {
            List<int> id = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data["idList"]));//idlist
            string UserName = data["UserName"].ToString();
            string Role = data["Role"].ToString();
            List<SMT_ProductionPlan> record = db.SMT_ProductionPlan.Where(c => id.Contains(c.Id)).ToList();
            var userlis = record.Select(c => c.Creator).Distinct().ToList();
            var name = UserName;
            if ((userlis.Count > 1 || !userlis.Contains(name)) && Role != "系统管理员")
            {
                var username = record.Where(c => c.Creator != name).Select(c => c.Creator).Distinct().ToList();
                return com.GetModuleFromJarray(null, false, "记录包含" + string.Join(",", username) + "的记录,你没有权限删除该条信息");
            }
            var ordernumlist = record.Select(c => c.OrderNum).ToList();
            var time = record.Select(c => c.PlanProductionDate).ToList();
            UserOperateLog log = new UserOperateLog() { Operator = UserName, OperateDT = DateTime.Now, OperateRecord = "SMT计划删除：订单" + string.Join(",", ordernumlist) + ";计划时间" + string.Join(",", time) };
            db.UserOperateLog.Add(log);
            db.SMT_ProductionPlan.RemoveRange(record);
            db.SaveChanges();
            return com.GetModuleFromJarray(null, true, "SMT计划已经成功删除");
        }

        #endregion

        #region------------------SMT生产信息（历史记录)
        //SMT 生产看板 历史
        [HttpPost]
        [ApiAuthorize]
        public JObject SMT_ProductionInfoHistory([System.Web.Http.FromBody]JObject data)
        {
            List<string> ordernum = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(data["ordernum"]));
            List<string> jobContent = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(data["jobContent"]));
            List<string> platformType = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(data["platformType"]));

            JArray result = new JArray();
            CommonalityController comm = new CommonalityController(); ;

            var SMT_ProductionPlans = db.SMT_ProductionPlan.Select(c => new { c.OrderNum, c.LineNum, c.JobContent, c.PlanProductionDate, c.Quantity, c.Capacity }).ToList();

            var SMT_ProductionDatas = db.SMT_ProductionData.Select(c => new { c.OrderNum, c.LineNum, c.JobContent, c.BeginTime, c.EndTime, c.AbnormalCount, c.NormalCount }).ToList();

            var orderMug = db.OrderMgm.Select(c => new { c.OrderNum, c.ID, c.Models, c.AdapterCard, c.Powers, c.PlatformType, c.ProcessingRequire, c.HandSampleScedule, c.StandardRequire }).ToList();

            if (ordernum != null)
            {
                SMT_ProductionPlans = SMT_ProductionPlans.Where(c => ordernum.Contains(c.OrderNum)).ToList();
            }
            if (jobContent != null)
            {
                SMT_ProductionPlans = SMT_ProductionPlans.Where(c => jobContent.Contains(c.JobContent)).ToList();
            }
            if (platformType != null)
            {
                var orderList = orderMug.Where(c => platformType.Contains(c.PlatformType)).Select(c => c.OrderNum).ToList();
                SMT_ProductionPlans = SMT_ProductionPlans.Where(c => orderList.Contains(c.OrderNum)).ToList();
            }
            var OrderList = SMT_ProductionPlans.Select(c => c.OrderNum).Distinct().ToList();

            foreach (var orderItem in OrderList)
            {
                JObject OrderNumdata = new JObject();
                //订单记录信息
                var OrderMgmsInfo = orderMug.Where(c => c.OrderNum == orderItem).Select(c => new { c.ID, c.Models, c.AdapterCard, c.Powers, c.PlatformType, c.StandardRequire, c.HandSampleScedule, c.ProcessingRequire }).FirstOrDefault();
                #region ----PC+小样
                //订单ID号
                OrderNumdata.Add("Id", OrderMgmsInfo.ID.ToString());
                //订单号
                OrderNumdata.Add("OrderNum", orderItem);
                //平台
                OrderNumdata.Add("PlatformType", OrderMgmsInfo.PlatformType);
                //制作要求
                OrderNumdata.Add("ProcessingRequire", OrderMgmsInfo.ProcessingRequire);
                //标准要求
                OrderNumdata.Add("StandardRequire", OrderMgmsInfo.StandardRequire);
                //小样进度
                OrderNumdata.Add("HandSampleScedule", OrderMgmsInfo.HandSampleScedule);
                //是否有图片
                if (comm.CheckJpgExit(orderItem, "SmallSample_Files"))
                    OrderNumdata.Add("HandSampleSceduleJpg", "false");
                else
                    OrderNumdata.Add("HandSampleSceduleJpg", "true");
                //是否有PDF文件
                if (comm.CheckpdfExit(orderItem, "SmallSample_Files"))
                    OrderNumdata.Add("HandSampleScedulePdf", "false");
                else
                    OrderNumdata.Add("HandSampleScedulePdf", "true");

                var sample = db.Small_Sample.OrderBy(c => c.Id).Where(c => (c.OrderNumber == orderItem || c.OrderNumber.Contains(orderItem)) && c.Approved != null && c.ApprovedResult == true).ToList();
                if (sample.Count != 0)
                {
                    JArray sampleJarray = new JArray();
                    int k = 1;
                    foreach (var sampleitem in sample)
                    {
                        JObject sampleJobject = new JObject();
                        sampleJobject.Add("id", sampleitem.Id);
                        sampleJobject.Add("Name", "NO." + k);
                        k++;
                        sampleJarray.Add(sampleJobject);
                    }
                    OrderNumdata.Add("HandSampleSceduleReport", sampleJarray);
                }
                else
                    OrderNumdata.Add("HandSampleSceduleReport", "false");

                //异常
                var SMTAbnormal_Description = db.OrderMgm.Where(c => c.OrderNum == orderItem).ToList().FirstOrDefault().SMTAbnormal_Description;
                OrderNumdata.Add("SMTAbnormal_Description", SMTAbnormal_Description);
                if (!string.IsNullOrEmpty(SMTAbnormal_Description))
                {
                    if (!comm.CheckJpgExit(orderItem, "SMTAbnormalOrder_Files"))
                        OrderNumdata.Add("SMTAbnormal_DescriptionJpg", "true");
                    else
                        OrderNumdata.Add("SMTAbnormal_DescriptionJpg", "false");

                    if (!comm.CheckpdfExit(orderItem, "SMTAbnormalOrder_Files"))
                        OrderNumdata.Add("SMTAbnormal_DescriptionPdf", "true");
                    else
                        OrderNumdata.Add("SMTAbnormal_DescriptionPdf", "false");
                }
                #endregion
                var planLine = SMT_ProductionPlans.Where(c => c.OrderNum == orderItem).Select(c => c.LineNum).Distinct().ToList();

                JArray LineArray = new JArray();
                foreach (var itemPlan in planLine)
                {
                    JObject LineData = new JObject();
                    var planInfo = SMT_ProductionPlans.Where(c => c.OrderNum == orderItem && c.LineNum == itemPlan).ToList();
                    //线别
                    LineData.Add("LineNum", itemPlan);
                    //模块术
                    var ModelNum = 0;
                    if (planInfo.FirstOrDefault().JobContent == "灯面" || planInfo.FirstOrDefault().JobContent == "IC面")
                    {
                        ModelNum = OrderMgmsInfo.Models;
                    }
                    else if (planInfo.FirstOrDefault().JobContent != null && planInfo.FirstOrDefault().JobContent.Contains("转接卡") == true)
                    {
                        ModelNum = OrderMgmsInfo.AdapterCard;
                    }
                    else if (planInfo.FirstOrDefault().JobContent != null && planInfo.FirstOrDefault().JobContent.Contains("电源") == true)
                    {
                        ModelNum = OrderMgmsInfo.Powers;
                    }

                    //模块数
                    LineData.Add("Models", ModelNum);
                    //工作内容
                    LineData.Add("JobContent", planInfo.FirstOrDefault().JobContent);
                    #region 今天计划
                    var todayplan = planInfo.Where(c => string.Format("{0:yyyy-MM-dd}", c.PlanProductionDate) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).ToList();
                    int todayPlanNum = 0;
                    if (todayplan.Count == 0)
                    {
                        //今天计划数
                        LineData.Add("PlanQuantity", 0);
                        //今天产能
                        LineData.Add("Capacity", 0);
                    }
                    else
                    {
                        //今天计划数
                        todayPlanNum = todayplan.Sum(c => c.Quantity);
                        LineData.Add("PlanQuantity", todayPlanNum);
                        //今天产能
                        decimal plancapity = todayplan.Sum(c => c.Capacity);
                        LineData.Add("Capacity", plancapity);
                    }
                    #endregion

                    #region 今天产量
                    var totayData = SMT_ProductionDatas.Where(c => c.OrderNum == orderItem && c.LineNum == itemPlan && c.JobContent == planInfo.FirstOrDefault().JobContent && string.Format("{0:yyyy-MM-dd}", c.BeginTime) == string.Format("{0:yyyy-MM-dd}", DateTime.Now)).ToList();
                    if (totayData.Count == 0)
                    {
                        //今天良品
                        LineData.Add("NormalCount", 0);
                        //今天不良品数
                        LineData.Add("AbnormalCount", 0);
                        //今天完成率
                        LineData.Add("TodayFinishRate", "");
                        //今天合格率
                        LineData.Add("TodayPassRate", "");
                    }
                    else
                    {
                        //今天良品
                        var todayNormalNum = totayData.Sum(C => C.NormalCount);
                        LineData.Add("NormalCount", todayNormalNum);
                        //今天不良品
                        var todayAbnormalNum = totayData.Sum(c => c.AbnormalCount);
                        LineData.Add("AbnormalCount", todayAbnormalNum);
                        //今天完成率
                        LineData.Add("TodayFinishRate", todayPlanNum == 0 ? "" : (((decimal)todayNormalNum + todayAbnormalNum) / todayPlanNum * 100).ToString("F2"));
                        //今天合格率
                        LineData.Add("TodayPassRate", ((decimal)todayNormalNum / (todayNormalNum + todayAbnormalNum) * 100).ToString("F2"));

                    }

                    #endregion

                    #region 订单对应线别 
                    var OrdernumlineData = SMT_ProductionDatas.Where(c => c.OrderNum == orderItem && c.LineNum == itemPlan).ToList();
                    if (OrdernumlineData.Count == 0)
                    {
                        //对应线别良品总数
                        LineData.Add("LineAllNomalCount", 0);
                        //对应线别不良品总数
                        LineData.Add("LineAllAbnomalCount", 0);
                    }
                    else
                    {
                        //对应线别良品总数
                        var lineNormalNumSum = OrdernumlineData.Sum(c => c.NormalCount);
                        LineData.Add("LineAllNomalCount", lineNormalNumSum);

                        //对应线别不良品总数
                        var lineAbnormalNumSum = OrdernumlineData.Sum(c => c.AbnormalCount);
                        LineData.Add("LineAllAbnomalCount", lineAbnormalNumSum);
                    }
                    #endregion

                    #region 订单对应工作内容
                    var OrdernumJobcontentData = SMT_ProductionDatas.Where(c => c.OrderNum == orderItem && c.JobContent == planInfo.FirstOrDefault().JobContent).ToList();
                    var jobContentNormalNumSum = 0;
                    var jobContentAbnormalNumSum = 0;
                    if (OrdernumJobcontentData.Count == 0)
                    {
                        //对应工作内容良品总数
                        LineData.Add("AllNormalCount", 0);
                        //对应工作内容不良品总数
                        LineData.Add("AllAbnormalCount", 0);
                    }
                    else
                    {
                        //对应工作内容良品总数
                        jobContentNormalNumSum = OrdernumJobcontentData.Sum(c => c.NormalCount);
                        LineData.Add("AllNormalCount", jobContentNormalNumSum);

                        //对应工作内容不良品总数
                        jobContentAbnormalNumSum = OrdernumJobcontentData.Sum(c => c.AbnormalCount);
                        LineData.Add("AllAbnormalCount", jobContentAbnormalNumSum);
                    }

                    #endregion

                    #region 统计
                    //订单总完成率
                    LineData.Add("OrderFinishRate", jobContentAbnormalNumSum + jobContentNormalNumSum == 0 ? "" : (((decimal)(jobContentNormalNumSum + jobContentAbnormalNumSum)) / ModelNum * 100).ToString("F2"));
                    //订单总合格率
                    LineData.Add("OrderPassRate", (jobContentAbnormalNumSum + jobContentNormalNumSum) == 0 ? "" : ((decimal)jobContentNormalNumSum / (jobContentNormalNumSum + jobContentAbnormalNumSum) * 100).ToString("F2"));

                    //开始时间
                    var lineBegintime = SMT_ProductionDatas.Where(c => c.OrderNum == orderItem && c.LineNum == itemPlan).Min(c => c.BeginTime);
                    LineData.Add("BeginTime", lineBegintime.ToString());

                    var lineEndTime = SMT_ProductionDatas.Where(c => c.OrderNum == orderItem && c.LineNum == itemPlan).Max(c => c.EndTime);
                    if (jobContentAbnormalNumSum + jobContentNormalNumSum >= ModelNum)
                    {
                        //结束时间
                        LineData.Add("EndTime", lineEndTime.ToString());
                        //生产用时
                        LineData.Add("TotalTime", (Convert.ToDateTime(lineEndTime) - Convert.ToDateTime(lineBegintime)).ToString());
                    }
                    else
                    {
                        //结束时间
                        LineData.Add("EndTime", "");
                        //生产用时
                        LineData.Add("TotalTime", "");
                    }
                    #endregion
                    LineArray.Add(LineData);
                }
                OrderNumdata.Add("LineArray", LineArray);
                result.Add(OrderNumdata);
            }

            return com.GetModuleFromJarray(result);

        }

        #endregion

        #region------------------生产操作Operator(数据录入、修改、删除)

        #region----------SMT生产信息查找历史记录 SMT_ProductionData
        [HttpPost]
        [ApiAuthorize]
        public JObject SMT_ProductionData([System.Web.Http.FromBody]JObject data)
        {
            string orderNum = data["orderNum"].ToString();
            string jobContent = data["jobContent"].ToString();
            int lineNum = int.Parse(data["lineNum"].ToString());
            DateTime? ProductionDate = DateTime.Parse(data["ProductionDate"].ToString());

            JObject result = new JObject();
            result.Add("orderinfo", null);
            var smt_Data = db.SMT_ProductionData.Select(c => new { c.LineNum, c.OrderNum, c.JobContent, c.AbnormalCount, c.NormalCount, c.BeginTime, c.EndTime, c.ProductionDate, c.Operator, c.VeneerPoints, c.BarcodeNum, c.Result }).ToList();
            if (!string.IsNullOrEmpty(orderNum))
            {
                smt_Data = smt_Data.Where(c => c.OrderNum == orderNum).ToList();
                var ordervalue = db.OrderMgm.Where(c => c.OrderNum == orderNum).FirstOrDefault();
                JObject orderinfo = new JObject();
                orderinfo.Add("OrderNum", ordervalue.OrderNum);//订单
                orderinfo.Add("Models", ordervalue.Models);//模块数
                orderinfo.Add("PlanInputTime", ordervalue.PlanInputTime);//计划投入时间
                orderinfo.Add("DeliveryDate", ordervalue.DeliveryDate);//出货时间
                orderinfo.Add("PlatformType", ordervalue.PlatformType);//平台类型
                orderinfo.Add("ProcessingRequire", ordervalue.ProcessingRequire);//制程要求
                orderinfo.Add("StandardRequire", ordervalue.StandardRequire);//标准要求 
                result["orderinfo"] = orderinfo;
            }
            if (!string.IsNullOrEmpty(jobContent))
            {
                smt_Data = smt_Data.Where(c => c.JobContent == jobContent).ToList();
            }
            if (lineNum != 0)
            {
                smt_Data = smt_Data.Where(c => c.LineNum == lineNum).ToList();
            }
            if (ProductionDate != null)
            {
                smt_Data = smt_Data.Where(c => c.ProductionDate == ProductionDate).ToList();
            }
            smt_Data = smt_Data.OrderBy(c => c.LineNum).ToList();
            #region IC面/灯面总数统计
            if (smt_Data.Count == 0)
            {
                result.Add("TotalNum", null);
            }
            else
            {
                JObject Total = new JObject();
                //ic良品
                var icNornal = smt_Data.Count(c => c.JobContent == "IC面") == 0 ? 0 : smt_Data.Where(c => c.JobContent == "IC面").Sum(c => c.NormalCount);
                Total.Add("icNornal", icNornal);
                //ic不良
                var icAb = smt_Data.Count(c => c.JobContent == "IC面") == 0 ? 0 : smt_Data.Where(c => c.JobContent == "IC面").Sum(c => c.AbnormalCount);
                Total.Add("icAb", icAb);
                //灯面良品
                var lingNornal = smt_Data.Count(c => c.JobContent == "灯面") == 0 ? 0 : smt_Data.Where(c => c.JobContent == "灯面").Sum(c => c.NormalCount);
                Total.Add("lingNornal", lingNornal);
                //灯面不良
                var lingAb = smt_Data.Count(c => c.JobContent == "灯面") == 0 ? 0 : smt_Data.Where(c => c.JobContent == "灯面").Sum(c => c.AbnormalCount);
                Total.Add("lingAb", lingAb);
                result.Add("TotalNum", Total);
            }
            #endregion
            JArray value = new JArray();
            foreach (var item in smt_Data)
            {
                JObject obj = new JObject();
                obj.Add("LineNum", item.LineNum);//产线
                obj.Add("OrderNum", item.OrderNum);//订单
                obj.Add("JobContent", item.JobContent);//工作内容
                obj.Add("NormalCount", item.NormalCount);//良品数量
                obj.Add("AbnormalCount", item.AbnormalCount);//不良品数量
                obj.Add("VeneerPoints", item.VeneerPoints);//单板点数
                obj.Add("BeginTime", item.BeginTime);//开始时间
                obj.Add("EndTime", item.EndTime);//结束时间
                obj.Add("BarcodeNum", item.BarcodeNum);//条码号
                obj.Add("Result", item.Result);//产品状态
                obj.Add("ProductionDate", item.ProductionDate);//日期
                obj.Add("Operator", item.Operator);//操作员
                value.Add(obj);
            }
            result.Add("value", value);
            return com.GetModuleFromJobjet(result);
        }

        #endregion

        #region----------数据生产录入
        /// <summary>
        /// 生产数据录入前,输入订单,判断订单生产情况
        /// </summary>
        /// <param name="ordernum"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject GetOrderQuantityCount(string ordernum)
        {
            JObject result = new JObject();
            var ordernum_record = db.SMT_ProductionData.Where(c => c.OrderNum == ordernum).ToList();
            if (ordernum_record.Count == 0)
            {
                return com.GetModuleFromJarray(null, null, "此订单无生产记录");
            }
            var jobContentList = ordernum_record.Select(c => c.JobContent).Distinct().ToList();
            if (jobContentList != null)
            {
                string mes = "";
                mes = "订单模块总数：" + db.OrderMgm.Where(c => c.OrderNum == ordernum).FirstOrDefault().Models + ";";
                foreach (var jobcontent in jobContentList)
                {
                    var normalCount = ordernum_record.Where(c => c.JobContent == jobcontent).Sum(c => c.NormalCount);
                    var abnormalCount = ordernum_record.Where(c => c.JobContent == jobcontent).Sum(c => c.AbnormalCount);
                    mes = mes + jobcontent + "良品数：" + normalCount + ";";
                    mes = mes + jobcontent + "不良品数：" + abnormalCount + ";";
                    mes = mes + jobcontent + "总数：" + normalCount + abnormalCount + ";";
                }
                return com.GetModuleFromJarray(null, null, mes);
            }
            return com.GetModuleFromJarray(null, null, " ");
        }

        /// <summary>
        /// 生产数据录入
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject SMT_Operator([System.Web.Http.FromBody]JObject data)
        {
            List<SMT_ProductionData> SMT_ProductionDataList = JsonConvert.DeserializeObject<List<SMT_ProductionData>>(JsonConvert.SerializeObject(data["SMT_ProductionDataList"]));
            string Department = data["Department"].ToString();
            string Group = data["Group"].ToString();
            List<SMT_ProductionData> result = new List<SMT_ProductionData>();
            foreach (var item in SMT_ProductionDataList)
            {
                var count = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum == item.LineNum && c.JobContent == item.JobContent && DbFunctions.DiffDays(item.BeginTime, c.PlanProductionDate) == 0).Count();
                if (count == 0)
                {
                    return com.GetModuleFromJarray(null, false, "订单" + item.OrderNum + "在产线" + item.LineNum + "没有做" + item.JobContent + "生产计划！");
                }
                else
                {
                    item.Department = Department;
                    item.Group = Group;
                    result.Add(item);
                }
            }
            if (ModelState.IsValid)
            {
                db.SMT_ProductionData.AddRange(result);
                db.SaveChanges();
            }
            var ordernum = SMT_ProductionDataList.Select(c => c.OrderNum).Distinct().ToList();
            foreach (var orditem in ordernum)
            {
                var board = db.ModuleBoard.Where(c => c.Ordernum == orditem && c.Section == "SMT").FirstOrDefault();
                if (board == null)
                {
                    ModuleBoard moduleBoard = new ModuleBoard() { Ordernum = orditem, Section = "SMT", UpdateDate = DateTime.Now };
                    db.ModuleBoard.Add(moduleBoard);
                    db.SaveChanges();
                }
                else
                {
                    board.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                }
            }
            return com.GetModuleFromJarray(null, true, "保存成功");//跳转到../SMT/SMT_Operator
        }

        #endregion

        #region----------数据生产修改
        [HttpPost]
        [ApiAuthorize]
        public JObject SMT_ProductionDataEdit([System.Web.Http.FromBody]JObject data)
        {
            SMT_ProductionData record = JsonConvert.DeserializeObject<SMT_ProductionData>(JsonConvert.SerializeObject(data["record"]));
            string UserName = data["UserName"].ToString(); ;
            if (ModelState.IsValid)
            {
                var old = db.SMT_ProductionData.Find(record.Id);
                if (old == null)
                {
                    return com.GetModuleFromJarray(null, false, "没有找到此数据");
                }
                UserOperateLog log = new UserOperateLog() { Operator = UserName, OperateDT = DateTime.Now, OperateRecord = "SMT生产数据修改：原数据产线、订单号、工作内容、良品数量、不良品数量、开始时间、结束时间为" + old.LineNum + "，" + old.OrderNum + "，" + old.JobContent + "," + old.NormalCount + "，" + old.AbnormalCount + "，" + old.BeginTime + "," + old.EndTime + "修改为" + record.LineNum + "，" + record.OrderNum + "，" + record.JobContent + "," + record.NormalCount + "，" + record.AbnormalCount + "，" + record.BeginTime + "," + record.EndTime };
                db.UserOperateLog.Add(log);
                old.LineNum = record.LineNum;
                old.OrderNum = record.OrderNum;
                old.JobContent = record.JobContent;
                old.NormalCount = record.NormalCount;
                old.AbnormalCount = record.AbnormalCount;
                old.BeginTime = record.BeginTime;
                old.EndTime = record.EndTime;

                db.SaveChangesAsync();

                var board = db.ModuleBoard.Where(c => c.Ordernum == record.OrderNum && c.Section == "SMT").FirstOrDefault();
                if (board == null)
                {
                    ModuleBoard moduleBoard = new ModuleBoard() { Ordernum = record.OrderNum, Section = "SMT", UpdateDate = DateTime.Now };
                    db.ModuleBoard.Add(moduleBoard);
                    db.SaveChanges();
                }
                else
                {
                    board.UpdateDate = DateTime.Now;
                    db.SaveChanges();
                }
                return com.GetModuleFromJarray(null, true, "成功");
            }
            return com.GetModuleFromJarray(null, false, "数据格式错误");
        }
        #endregion

        #region----------数据生产删除

        [HttpPost]
        [ApiAuthorize]
        public JObject SMT_ProductionDataDeleteConfirmed([System.Web.Http.FromBody]JObject data)
        {
            int id = int.Parse(data["id"].ToString());
            string UserName = data["UserName"].ToString();
            SMT_ProductionData record = db.SMT_ProductionData.Find(id);

            if (record == null)
            {
                return com.GetModuleFromJarray(null, false, "没有找到此记录");
            }
            UserOperateLog log = new UserOperateLog() { Operator = UserName, OperateDT = DateTime.Now, OperateRecord = "SMT数据生产删除：原数据产线、订单号、工作内容、良品数量、不良品数量、开始时间、结束时间为" + record.LineNum + "，" + record.OrderNum + "，" + record.JobContent + "," + record.NormalCount + "，" + record.AbnormalCount + "，" + record.BeginTime + "," + record.EndTime };
            db.UserOperateLog.Add(log);
            db.SMT_ProductionData.Remove(record);
            var board = db.ModuleBoard.Where(c => c.Ordernum == record.OrderNum && c.Section == "SMT").FirstOrDefault();
            if (board == null)
            {
                ModuleBoard moduleBoard = new ModuleBoard() { Ordernum = record.OrderNum, Section = "SMT", UpdateDate = DateTime.Now };
                db.ModuleBoard.Add(moduleBoard);
                db.SaveChanges();
            }
            else
            {
                board.UpdateDate = DateTime.Now;
                db.SaveChanges();
            }
            db.SaveChangesAsync();
            return com.GetModuleFromJarray(null, true, "删除成功"); ;
        }
        #endregion

        #endregion
        
        #region -------------------------SMT看板IC面看板
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayICContenInfo([System.Web.Http.FromBody]JObject data)
        {
            DateTime? time = DateTime.Parse( data["time"].ToString());
            int lintPlan = 0;
            int lineshiji = 0;
            int lineaccess = 0;
            List<Temp> efficiency = new List<Temp>();
            List<Temp> quality = new List<Temp>();
            JObject result = new JObject();
            var addtime = new DateTime();
            if (time == null)
            {
                time = DateTime.Now.Date;
                addtime = time.Value.AddDays(1);
            }
            else
            {
                time = time.Value.Date;
                addtime = time.Value.AddDays(1).AddHours(8);
            }
            var time2 = time.Value.AddHours(8);
            JArray content = new JArray();
            for (var i = 1; i < 8; i++)
            {
                var status = db.SMT_ProductionLineInfo.Where(c => c.LineNum == i).Select(c => c.Status).FirstOrDefault();
                var plan = db.SMT_ProductionPlan.Where(c => c.LineNum == i && c.PlanProductionDate == time).Select(c => c.OrderNum).ToList();

                JObject item = new JObject();
                JArray orderJarray = new JArray();
                //线别
                item.Add("Line", i);
                int orderplannum = 0;
                int ordernum = 0;
                int orderaccessnum = 0;
                foreach (var itemorder in plan)
                {
                    JObject ordercontent = new JObject();
                    var totalordernum = db.OrderMgm.Where(c => c.OrderNum == itemorder).Select(c => c.Models).FirstOrDefault();
                    var planNum = db.SMT_ProductionPlan.Where(c => c.LineNum == i && c.PlanProductionDate == time && c.OrderNum == itemorder).Select(c => c.Quantity).FirstOrDefault();
                    var date = db.SMT_ProductionData.Where(c => c.LineNum == i && c.BeginTime >= time2 && c.EndTime <= addtime && c.OrderNum == itemorder).ToList();
                    var num = 0;
                    var notnum = 0;
                    foreach (var dateitem in date)
                    {
                        num = num + dateitem.AbnormalCount + dateitem.NormalCount;
                        notnum = notnum + dateitem.AbnormalCount;
                    }
                    var access = num - notnum;
                    orderaccessnum = orderaccessnum + access;
                    //订单号
                    ordercontent.Add("OrderNum", itemorder);
                    //生产状态
                    ordercontent.Add("Statu", status);
                    //订单总数
                    ordercontent.Add("totalNum", totalordernum);
                    //目标投入
                    ordercontent.Add("targetNum", planNum);
                    orderplannum = orderplannum + planNum;
                    //实际产出
                    ordercontent.Add("actualNum", num);
                    ordernum = ordernum + num;
                    //累计产出
                    var cumulative = db.SMT_ProductionData.Where(c => c.LineNum == i && c.EndTime <= addtime && c.OrderNum == itemorder).ToList();
                    var cumulativenum = 0;
                    cumulative.ForEach(c => cumulativenum = cumulativenum + c.AbnormalCount + c.NormalCount);
                    ordercontent.Add("cumulative", cumulativenum);
                    //达成率
                    var pass = planNum == 0 ? 0 : ((decimal)num * 100 / planNum);
                    ordercontent.Add("PassRate", Math.Round(Convert.ToDecimal(pass), 2));
                    //不良数
                    ordercontent.Add("AbnormalCount", notnum);
                    //良率
                    var yield = num == 0 ? 0 : ((decimal)access * 100 / num);
                    ordercontent.Add("accessRate", Math.Round(Convert.ToDecimal(yield), 2));
                    orderJarray.Add(ordercontent);

                    lintPlan = lintPlan + planNum;
                    lineshiji = lineshiji + num;
                    lineaccess = lineaccess + access;

                }
                item.Add("Content", orderJarray);

                if (status == "生产中")
                {
                    Temp eff = new Temp() { line = i + "线", value = Math.Round(Convert.ToDecimal(orderplannum == 0 ? 0 : ((decimal)ordernum * 100 / orderplannum)), 2) };
                    Temp qua = new Temp() { line = i + "线", value = Math.Round(Convert.ToDecimal(ordernum == 0 ? 0 : ((decimal)orderaccessnum * 100 / ordernum)), 2) };
                    efficiency.Add(eff);
                    quality.Add(qua);
                }
                content.Add(item);
            }

            result.Add("achievementRate", lintPlan == 0 ? 0 : Math.Round(Convert.ToDecimal((decimal)lineshiji * 100 / lintPlan), 2));
            result.Add("ProductionYield", lineshiji == 0 ? 0 : Math.Round(Convert.ToDecimal((decimal)lineaccess * 100 / lineshiji), 2));
            var lasteff = efficiency.OrderBy(c => c.value).Where(c => c.value < 98).Take(3).ToList();
            if (lasteff.Count > 3)
            {
                lasteff = lasteff.Take(3).ToList();
            }
            var lastqua = quality.OrderBy(c => c.value).Take(3).ToList();
            var count = lasteff.Count;
            JObject effjobject = new JObject();
            JObject quajobject = new JObject();
            JArray line = new JArray();
            JArray value = new JArray();
            JArray line2 = new JArray();
            JArray value2 = new JArray();
            foreach (var eff in lasteff)
            {
                line.Add(eff.line);
                value.Add(eff.value);
            }
            foreach (var qua in lastqua)
            {
                line2.Add(qua.line);
                value2.Add(qua.value);
            }
            effjobject.Add("line", line);
            effjobject.Add("value", value);
            quajobject.Add("line", line2);
            quajobject.Add("value", value2);
            result.Add("efficiency", effjobject);
            result.Add("quality", quajobject);
            result.Add("content", content);
            return com.GetModuleFromJobjet(result);
        }
        #endregion

        #region -------------------------SMT看板灯面看板
        [HttpPost]
        public JObject DisplayLightContenInfo([System.Web.Http.FromBody]JObject data)
        {
            DateTime? time = DateTime.Parse(data["time"].ToString());
            int lintPlan = 0;
            int lineshiji = 0;
            int production = 0;
            int notproduction = 0;
            List<Temp> efficiency = new List<Temp>();
            List<Temp> quality = new List<Temp>();
            JObject result = new JObject();
            var addtime = new DateTime();
            if (time == null)
            {
                time = DateTime.Now.Date;
                addtime = time.Value.AddDays(1);
            }
            else
            {
                time = time.Value.Date;
                addtime = time.Value.AddDays(1).AddHours(8);
            }
            var time2 = time.Value.AddHours(8);
            JArray content = new JArray();
            for (var i = 8; i < 15; i++)
            {
                var info = db.SMT_ProductionLineInfo.Where(c => c.LineNum == i).FirstOrDefault();
                var plan = db.SMT_ProductionPlan.Where(c => c.LineNum == i && c.PlanProductionDate == time).Select(c => c.OrderNum).ToList();



                JArray orderjarry = new JArray();
                JObject item = new JObject();
                //线别
                item.Add("Line", i);
                int orderplannum = 0;
                int ordernum = 0;
                int ordernotnum = 0;
                int ordetverr = 0;
                foreach (var itemorder in plan)
                {
                    JObject ordercontent = new JObject();
                    var verr = 0;
                    var totalordernum = db.OrderMgm.Where(c => c.OrderNum == itemorder).Select(c => c.Models).FirstOrDefault();
                    var planNum = db.SMT_ProductionPlan.Where(c => c.LineNum == i && c.PlanProductionDate == time && c.OrderNum == itemorder).Select(c => c.Quantity).FirstOrDefault();
                    var date = db.SMT_ProductionData.Where(c => c.LineNum == i && c.BeginTime >= time2 && c.EndTime <= addtime && c.OrderNum == itemorder).ToList();
                    var num = 0;
                    var notnum = 0;
                    foreach (var dateitem in date)
                    {
                        num = num + dateitem.AbnormalCount + dateitem.NormalCount;
                        notnum = notnum + dateitem.AbnormalCount;
                        verr = dateitem.VeneerPoints;
                    }
                    var access = num - notnum;
                    ordernotnum = ordernotnum + notnum;
                    //订单号
                    ordercontent.Add("OrderNum", itemorder);
                    //生产状态
                    ordercontent.Add("Statu", info.Status);
                    //订单总数
                    ordercontent.Add("totalNum", totalordernum);
                    //目标投入
                    ordercontent.Add("targetNum", planNum);
                    orderplannum = orderplannum + planNum;
                    //实际产出
                    ordercontent.Add("actualNum", num);
                    ordernum = ordernum + num;
                    //累计产出
                    var cumulative = db.SMT_ProductionData.Where(c => c.LineNum == i && c.EndTime <= addtime && c.OrderNum == itemorder).ToList();
                    var cumulativenum = 0;
                    cumulative.ForEach(c => cumulativenum = cumulativenum + c.AbnormalCount + c.NormalCount);
                    ordercontent.Add("cumulative", cumulativenum);
                    //达成率
                    var pass = planNum == 0 ? 0 : ((decimal)num * 100 / planNum);
                    ordercontent.Add("PassRate", Math.Round(Convert.ToDecimal(pass), 2));
                    //单板点数
                    ordercontent.Add("VeneerPoints", verr);
                    ordetverr = ordetverr + verr;
                    //不良数点数
                    ordercontent.Add("AbnormalCount", notnum);
                    //DPPM
                    var yield = num == 0 || verr == 0 ? 0 : ((decimal)notnum / (num * verr) * 1000000);
                    ordercontent.Add("DPPM", Math.Round(Convert.ToDecimal(yield), 2));
                    orderjarry.Add(ordercontent);

                    lintPlan = lintPlan + planNum;
                    lineshiji = lineshiji + num;
                    production = production + verr * num;
                    notproduction = notproduction + notnum;
                }
                if (info.Status == "生产中")
                {
                    Temp eff = new Temp() { line = i + "线", value = Math.Round(Convert.ToDecimal(orderplannum == 0 ? 0 : ((decimal)ordernum * 100 / orderplannum)), 2) };
                    Temp qua = new Temp() { line = i + "线", value = Math.Round(Convert.ToDecimal(ordernum == 0 || ordetverr == 0 ? 0 : ((decimal)ordernotnum / (ordernum * ordetverr) * 1000000)), 2) };
                    efficiency.Add(eff);
                    quality.Add(qua);
                }
                item.Add("Content", orderjarry);
                content.Add(item);
            }

            result.Add("achievementRate", lintPlan == 0 ? 0 : Math.Round(Convert.ToDecimal((decimal)lineshiji * 100 / lintPlan), 2));
            result.Add("ProductionYield", production == 0 ? 0 : Math.Round(Convert.ToDecimal((decimal)notproduction / production * 1000000), 2));
            var lasteff = efficiency.OrderBy(c => c.value).Where(c => c.value < 95).ToList();
            if (lasteff.Count > 3)
            {
                lasteff = lasteff.Take(3).ToList();
            }
            var lastqua = quality.OrderByDescending(c => c.value).Take(3).ToList();

            JObject effjobject = new JObject(), quajobject = new JObject();
            JArray line = new JArray(), value = new JArray(), line2 = new JArray(), value2 = new JArray();
            foreach (var eff in lasteff)
            {
                line.Add(eff.line);
                value.Add(eff.value);
            }
            foreach (var qua in lastqua)
            {
                line2.Add(qua.line);
                value2.Add(qua.value);
            }
            effjobject.Add("line", line);
            effjobject.Add("value", value);
            quajobject.Add("line", line2);
            quajobject.Add("value", value2);
            result.Add("efficiency", effjobject);
            result.Add("quality", quajobject);
            result.Add("content", content);
            return com.GetModuleFromJobjet(result);

        }
        #endregion
        

    }
}
