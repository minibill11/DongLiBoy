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

namespace JianHeMES.Controllers
{
    public class SMTController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();


        #region------------------SMT管理界面(产线管理，产线生产计划)
        // GET: SMT管理主页

        public ActionResult SMT_Mangage()
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


        #region---------------------产线管理

        public ActionResult SMT_ProductionLineCreate()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "SMT看板管理员" || ((Users)Session["User"]).Role == "系统管理员")
            {
                ViewBag.Status = ProductionLineStatus();
                return View();
            }
            return Content("<script>alert('对不起，您不能管理产线信息，请联系SMT看板管理员！');window.location.href='../SMT/SMT_Mangage';</script>");
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
                return RedirectToAction("SMT_Mangage");
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
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "SMT看板管理员" || ((Users)Session["User"]).Role == "系统管理员")
            {
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
            }
            return Content("<script>alert('对不起，您的不能管理产线信息，请联系SMT看板管理员！');window.location.href='../SMT_Mangage';</script>");
        }

        [HttpPost]
        public async Task<ActionResult> SMT_ProductionLineEdit([Bind(Include = "Id,LineNum,ProducingOrderNum,CreateDate,Team,GroupLeader,Status")]SMT_ProductionLineInfo record)
        {
            if (ModelState.IsValid)
            {
                db.Entry(record).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("SMT_Mangage");
            }
            return View(record);
        }

        #endregion


        #region---------------------订单管理

        // GET: SMT订单信息管理
        public ActionResult SMT_OrderMangage()
        {
            ViewBag.FinishStatus = FinishStatusList();
            return View();
        }

        [HttpPost]
        public PartialViewResult SMT_OrderMangage(string FinishStatus)
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
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "SMT看板管理员" || ((Users)Session["User"]).Role == "系统管理员")
            {
                return View();
            }
            return Content("<script>alert('对不起，您的不能管理产线信息，请联系SMT看板管理员！');window.location.href='../SMT/SMT_OrderMangage';</script>");
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
                    if (IsExistOrder>0)
                    {
                        SMT_OrderInfoCheck_result.Add(item.OrderNum, "订单已存在");
                    }
                    else
                    {
                        SMT_OrderInfoCheck_result.Add(item.OrderNum, "订单不存在");
                    }
                }
                //return Content(SMT_OrderInfoCheck_result.ToString());
                return Json(SMT_OrderInfoCheck_result.ToString(),JsonRequestBehavior.AllowGet);
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
                return RedirectToAction("SMT_OrderMangage");
            }
            return View(records);
        }

        public async Task<ActionResult> SMT_OrderInfoEdit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "SMT看板管理员" || ((Users)Session["User"]).Role == "系统管理员")
            {
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
            }
            return Content("<script>alert('对不起，您的不能管理产线信息，请联系SMT看板管理员！');window.location.href='../SMT_OrderMangage';</script>");
        }

        [HttpPost]
        public async Task<ActionResult> SMT_OrderInfoEdit([Bind(Include = "Id,OrderNum,LineNum,Quantity,PlatformType,Customer,DeliveryDate,Status")]SMT_OrderInfo record)
        {
            if (ModelState.IsValid)
            {
                db.Entry(record).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("SMT_OrderMangage");
            }
            return View(record);
        }

        #endregion


        #region---------------------产线计划管理
        [HttpGet]
        public ActionResult SMT_ProductionPlan()
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.LineNumList = GetLineNumList();
            ViewBag.JobContentList = GetJobContentList();
            var records = db.SMT_ProductionPlan.OrderBy(c=>c.LineNum).Where(c=>DbFunctions.DiffDays(c.PlanProductionDate, DateTime.Now)<=0).ToList();
            return View(records);
        }

        [HttpPost]
        public ActionResult SMT_ProductionPlan(string orderNum,int? lineNum,string jobContent, DateTime? planProductionDate, string remark)
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
            if (lineNum != null )
            {
                recordlist = recordlist.Where(c => c.LineNum == lineNum).ToList();
            }
            if(!String.IsNullOrEmpty(jobContent))
            {
                recordlist = recordlist.Where(c => c.JobContent == jobContent).ToList();
            }
            if(planProductionDate!=null)
            {
                recordlist = recordlist.Where(c => c.PlanProductionDate.Value.Year == planProductionDate.Value.Year && c.PlanProductionDate.Value.Month == planProductionDate.Value.Month && c.PlanProductionDate.Value.Day == planProductionDate.Value.Day).ToList();
            }
            if (!String.IsNullOrEmpty(remark))
            {
                recordlist = recordlist.Where(c => c.Remark.Contains(remark)).ToList();
            }
            return View(recordlist);
        }

        public ActionResult SMT_ProductionPlanCreate()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "系统管理员")
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
                    var IsExistOrder = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum==item.LineNum && c.JobContent==item.JobContent && DbFunctions.DiffDays(item.PlanProductionDate,c.PlanProductionDate)>=0).Count();
                    if (IsExistOrder > 0)
                    {
                        SMT_OrderInfoCheck_result.Add(item.LineNum+"_"+item.OrderNum+"_"+item.JobContent, "此产线已存在该订单计划");
                    }
                    else
                    {
                        if (db.OrderMgm.Count(c=>c.OrderNum==item.OrderNum) > 0)
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
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "PC计划员" || ((Users)Session["User"]).Role == "系统管理员")
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
            return Content("<script>alert('对不起，您的不能管理产线信息，请联系系统管理员！');window.location.href='../SMT_Mangage';</script>");
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
        #endregion


        #region---------------------用户管理

        // GET: SMT用户管理
        public ActionResult SMT_UserMangage()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            List < Users > SMT_User = db.Users.Where(c => c.Department == "SMT").ToList();
            string userRole = ((Users)Session["User"]).Role;
            string department = ((Users)Session["User"]).Department;
            if (userRole == "经理" & department=="SMT"|| userRole == "系统管理员")
            {
                return View(SMT_User);
            }
            if (userRole == "SMT看板管理员" & department == "SMT" || userRole == "系统管理员")
            {
                SMT_User = SMT_User.Where(c => c.Role != "经理").ToList();
                return View(SMT_User);
            }
            return Content("<script>alert('对不起，您的不能管理SMT产线用户，请联系SMT经理！');window.location.href='../SMT/SMT_Mangage';</script>");
        }


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


        #region------------------SMT生产信息（总看板、产线看板、查找历史记录）

        // GET: SMT总览表
        public ActionResult SMT_ProductionInfo()
        {
            return View();
        }

        public ActionResult SMT_ProductionLineInfo(int LineNum)
        {
            //内容:痴线号，时间，班组，组长，正在生产的订单，良品数量，不良品数量，不良率，产线累计个数，订单完成率，今天计划订单，产线状态
            ViewBag.LineNum = LineNum;//获取产线号
            return View();
        }

        #region----------SMT生产信息查找历史记录
        public ActionResult SMT_ProductionData()
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.LineNumList = GetLineNumList();
            ViewBag.JobContentList = GetJobContentList();
            var records = db.SMT_ProductionData.OrderBy(c => c.LineNum).Where(c => DbFunctions.DiffDays(c.ProductionDate, DateTime.Now) <= 0).ToList();
            return View(records);
        }

        [HttpPost]
        public ActionResult SMT_ProductionData(string orderNum, int? lineNum, string jobContent, DateTime? ProductionDate)
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            ViewBag.LineNumList = GetLineNumList();
            ViewBag.JobContentList = GetJobContentList();
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


        #region------------------生产操作(数据录入)
        // GET: SMT产线未段工位输入操作

        public ActionResult SMT_Operator()
        {
            //ViewBag.OrderNumList = JsonConvert.SerializeObject(GetTodayPlanOrderNumList());//向View传递OrderNum订单号列表.
            //前端
            //var list = JSON.parse('@Html.Raw(ViewBag.OrderNumList)');
            ViewBag.OrderNumList = GetTodayPlanOrderNumList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role != "SMT看板管理员" || ((Users)Session["User"]).Department != "SMT部" && ((Users)Session["User"]).Role != "主管" || ((Users)Session["User"]).Role != "系统管理员")
            {
                return View();
            }
            return Content("对不起，您的不能进入产线操作，请联系SMT看板管理员！");
        }


        [HttpPost]
        public ActionResult SMT_Operator(List<SMT_ProductionData> SMT_ProductionDataList)
        {
            //ViewBag.OrderNumList = JsonConvert.SerializeObject(GetTodayPlanOrderNumList());//向View传递OrderNum订单号列表.
            //前端
            //var list = JSON.parse('@Html.Raw(ViewBag.OrderNumList)');
            ViewBag.OrderNumList = GetTodayPlanOrderNumList();
            List<SMT_ProductionData> result = new List<SMT_ProductionData>();
            foreach(var item in SMT_ProductionDataList)
            {
                var count = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum == item.LineNum && c.JobContent==item.JobContent && DbFunctions.DiffDays(c.PlanProductionDate, DateTime.Now) == 0).Count();
                if (count == 0)
                {
                    return Content("订单" + item.OrderNum + "在产线" + item.LineNum + "没有做" + item.JobContent + "生产计划！");
                }
                else
                {
                    result.Add(item);
                }
            }
            if (ModelState.IsValid)
            {
                db.SMT_ProductionData.AddRange(result);
                db.SaveChanges();
            }
            return Content("保存成功");//跳转到../SMT/SMT_Operator
        }


        [HttpPost]
        public ActionResult SMT_Operator_getBeginTime(string ordernum,int linenum)   //history.go(-1);
        {
            var record = db.SMT_ProductionData.Where(c => c.OrderNum == ordernum && c.LineNum == linenum && c.EndTime.Value.Year==DateTime.Now.Year && c.EndTime.Value.Month == DateTime.Now.Month && c.EndTime.Value.Day == DateTime.Now.Day).OrderByDescending(c => c.Id).FirstOrDefault();
            string lastrecordendtime = record == null ? "" : record.EndTime.ToString();
            return Content(lastrecordendtime);
        }
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
            var orders = db.SMT_ProductionPlan.Where(c=>c.PlanProductionDate.Value.Year==date.Year && c.PlanProductionDate.Value.Month == date.Month && c.PlanProductionDate.Value.Day == date.Day).OrderByDescending(m => m.Id).Select(m => m.OrderNum).Distinct();    //增加.Distinct()后会重新按OrderNum升序排序
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
