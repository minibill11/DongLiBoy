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


        #region------------------管理界面
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

        public ActionResult SMT_ProductionPlan()
        {
            var records = db.SMT_ProductionPlan.OrderBy(c=>c.LineNum).Where(c=>DbFunctions.DiffDays(c.CreateDate,DateTime.Now)<=0).ToList();
            return View(records);
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
                    var IsExistOrder = db.SMT_ProductionPlan.Where(c => c.OrderNum == item.OrderNum && c.LineNum==item.LineNum && DbFunctions.DiffDays(c.CreateDate,DateTime.Now )==0).Count();
                    if (IsExistOrder > 0)
                    {
                        SMT_OrderInfoCheck_result.Add(item.OrderNum, "此产线已存在该订单计划");
                    }
                    else
                    {
                        SMT_OrderInfoCheck_result.Add(item.OrderNum, "此产线可以做订单计划");
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
            if (((Users)Session["User"]).Role == "SMT看板管理员" || ((Users)Session["User"]).Role == "系统管理员")
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
            return Content("<script>alert('对不起，您的不能管理产线信息，请联系SMT看板管理员！');window.location.href='../SMT_Mangage';</script>");
        }

        [HttpPost]
        public async Task<ActionResult> SMT_ProductionPlanEdit([Bind(Include = "Id,LineNum,OrderNum,CreateDate,Quantity,PlatformType,Customer")]SMT_ProductionPlan record)
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


        #region------------------生产信息

        public ActionResult SMT_ProductionInfo_old()
        {
            return View();
        }


            // GET: SMT总览表
            public ActionResult SMT_ProductionInfo()
        {
            //SMT生产数据
            JObject LineData = new JObject();
            //SMT生产线号
            var LineNumList = db.SMT_ProductionLineInfo.OrderBy(d => d.LineNum).Select(c => c.LineNum).Distinct().ToList();
            JObject LineOrderDetails = new JObject();
            //今天全部计划生产的订单清单
            var PlanProductionOrderNumList = db.SMT_ProductionPlan.Where(c => c.CreateDate.Value.Year == DateTime.Now.Year && c.CreateDate.Value.Month == DateTime.Now.Month && c.CreateDate.Value.Day == DateTime.Now.Day).ToList();
            //今天全部生产记录清单
            var TodayProductionData = db.SMT_ProductionData.Where(c => c.ProductionDate.Value.Year == DateTime.Now.Year && c.ProductionDate.Value.Month == DateTime.Now.Month && c.ProductionDate.Value.Day == DateTime.Now.Day).ToList();
            //foreach每条生产线
            foreach (var linenum in LineNumList)
            {
                //取出今天本生产线的计划订单清单
                List<string> planOrderNumList = PlanProductionOrderNumList.Where(c => c.LineNum == linenum).Select(c => c.OrderNum).ToList();
                //foreach本生产线的每个计划订单号
                foreach (var planorder in planOrderNumList)
                {
                    //今天良品
                    int todayNormalcount = TodayProductionData.Where(c => c.OrderNum == planorder).FirstOrDefault()==null?0: TodayProductionData.Where(c => c.OrderNum == planorder).FirstOrDefault().NormalCount;
                    //今天不良品
                    int todayAbnormalcount = TodayProductionData.Where(c => c.OrderNum == planorder).FirstOrDefault() == null ? 0 : TodayProductionData.Where(c => c.OrderNum == planorder).FirstOrDefault().AbnormalCount;
                    //订单总良品
                    int orderNormalcount = 0;
                    if(db.SMT_ProductionData.Where(c => c.OrderNum == planorder).Count()>0)
                    {
                        orderNormalcount = db.SMT_ProductionData.Where(c => c.OrderNum == planorder).Sum(c => c.NormalCount);
                    }
                    //订单总不良品
                    int orderAbnormalcount = 0;
                    if(db.SMT_ProductionData.Where(c => c.OrderNum == planorder).Count()>0)
                    {
                        orderAbnormalcount = db.SMT_ProductionData.Where(c => c.OrderNum == planorder).Sum(c => c.AbnormalCount);
                    }
                    //订单日计划
                    int todayPlanQuantity = PlanProductionOrderNumList.Where(c => c.OrderNum == planorder).FirstOrDefault().Quantity;
                    //订单数量
                    int orderQuantity = db.SMT_OrderInfo.Where(c => c.OrderNum == planorder).FirstOrDefault() == null ? 0 : db.SMT_OrderInfo.Where(c => c.OrderNum == planorder).FirstOrDefault().Quantity;
                    //产线信息
                    var lineInfo = db.SMT_ProductionLineInfo.Where(c => c.LineNum == linenum).FirstOrDefault();

                    //建一个List装一行信息
                    List<string> detailInfor_planorder = new List<string>();
                    //往List填写“平台”信息
                    detailInfor_planorder.Add(db.SMT_OrderInfo.Where(c => c.OrderNum == planorder).FirstOrDefault() == null ? "" : db.SMT_OrderInfo.Where(c => c.OrderNum == planorder).FirstOrDefault().PlatformType);
                    //往List填写“订单数量”信息
                    detailInfor_planorder.Add(orderQuantity==0?"": orderQuantity.ToString());
                    //往List填写“日计划数量”信息
                    detailInfor_planorder.Add(todayPlanQuantity.ToString());
                    //往List填写“日生产完成数量”信息
                    detailInfor_planorder.Add((todayNormalcount + todayAbnormalcount).ToString());
                    //往List填写“日生产完成率”信息
                    detailInfor_planorder.Add(((todayNormalcount + todayAbnormalcount) * 100 / todayPlanQuantity).ToString("F2")+"%");
                    //往List填写“不良品”信息
                    detailInfor_planorder.Add(todayAbnormalcount.ToString());
                    //往List填写“良品”信息
                    detailInfor_planorder.Add(todayNormalcount.ToString());
                    //往List填写“不良率”信息
                    detailInfor_planorder.Add(todayAbnormalcount + todayNormalcount == 0?"0.00%": (todayAbnormalcount * 100 / (todayAbnormalcount + todayNormalcount)).ToString("F2") + "%");
                    //往List填写“总良品”信息
                    detailInfor_planorder.Add(orderNormalcount.ToString());
                    //往List填写“总不良品”信息
                    detailInfor_planorder.Add(orderAbnormalcount.ToString());
                    //往List填写“总不良品率”信息
                    detailInfor_planorder.Add(orderNormalcount + orderAbnormalcount == 0?"0.00%": (orderAbnormalcount * 100 / (orderNormalcount + orderAbnormalcount)).ToString("F2") + "%");
                    //往List填写“累计数量”信息
                    detailInfor_planorder.Add((orderNormalcount + orderAbnormalcount).ToString());
                    //往List填写“订单完成率”信息
                    detailInfor_planorder.Add(orderQuantity==0? "0.00%": ((orderNormalcount + orderAbnormalcount) * 100 / orderQuantity).ToString("F2") + "%");
                    //往List填写“班组”信息
                    detailInfor_planorder.Add(lineInfo.Team);
                    //往List填写“组长”信息
                    detailInfor_planorder.Add(lineInfo.GroupLeader);
                    //往List填写“状态”信息

                    if (TodayProductionData.Where(c => c.OrderNum == planorder).Count()>0)
                    {
                        var subtime = DateTime.Now.Subtract(Convert.ToDateTime(TodayProductionData.Where(c => c.OrderNum == planorder).FirstOrDefault().ProductionDate));
                        if (subtime.TotalMinutes>5)
                        {
                            detailInfor_planorder.Add("待生产");
                        }
                        else
                        {
                            detailInfor_planorder.Add("生产中");
                        }
                    }
                    else
                    {
                        detailInfor_planorder.Add("待生产");
                    }
                    LineOrderDetails.Add(planorder, JsonConvert.SerializeObject(detailInfor_planorder));
                }
                LineData.Add(linenum.ToString(), JsonConvert.SerializeObject(LineOrderDetails));
                LineOrderDetails = new JObject();
            }
            ViewBag.json = LineData;
            return View();
        }


        [HttpPost]
        public ActionResult SMT_ProductionInfo(string getdata)
        {
            //SMT生产数据
            JObject LineData = new JObject();
            //SMT生产线号
            var LineNumList = db.SMT_ProductionLineInfo.OrderBy(d => d.LineNum).Select(c => c.LineNum).Distinct().ToList();
            JObject LineOrderDetails = new JObject();
            //今天全部计划生产的订单清单
            var PlanProductionOrderNumList = db.SMT_ProductionPlan.Where(c => c.CreateDate.Value.Year == DateTime.Now.Year && c.CreateDate.Value.Month == DateTime.Now.Month && c.CreateDate.Value.Day == DateTime.Now.Day).ToList();
            //今天全部生产记录清单
            var TodayProductionData = db.SMT_ProductionData.Where(c => c.ProductionDate.Value.Year == DateTime.Now.Year && c.ProductionDate.Value.Month == DateTime.Now.Month && c.ProductionDate.Value.Day == DateTime.Now.Day).ToList();
            //foreach每条生产线
            foreach (var linenum in LineNumList)
            {
                //取出今天本生产线的计划订单清单
                List<string> planOrderNumList = PlanProductionOrderNumList.Where(c => c.LineNum == linenum).Select(c => c.OrderNum).ToList();
                //foreach本生产线的每个计划订单号
                foreach (var planorder in planOrderNumList)
                {
                    //今天良品
                    int todayNormalcount = TodayProductionData.Where(c => c.OrderNum == planorder).FirstOrDefault() == null ? 0 : TodayProductionData.Where(c => c.OrderNum == planorder).FirstOrDefault().NormalCount;
                    //今天不良品
                    int todayAbnormalcount = TodayProductionData.Where(c => c.OrderNum == planorder).FirstOrDefault() == null ? 0 : TodayProductionData.Where(c => c.OrderNum == planorder).FirstOrDefault().AbnormalCount;
                    //订单总良品
                    int orderNormalcount = 0;
                    if (db.SMT_ProductionData.Where(c => c.OrderNum == planorder).Count() > 0)
                    {
                        orderNormalcount = db.SMT_ProductionData.Where(c => c.OrderNum == planorder).Sum(c => c.NormalCount);
                    }
                    //订单总不良品
                    int orderAbnormalcount = 0;
                    if (db.SMT_ProductionData.Where(c => c.OrderNum == planorder).Count() > 0)
                    {
                        orderAbnormalcount = db.SMT_ProductionData.Where(c => c.OrderNum == planorder).Sum(c => c.AbnormalCount);
                    }
                    //订单日计划
                    int todayPlanQuantity = PlanProductionOrderNumList.Where(c => c.OrderNum == planorder).FirstOrDefault().Quantity;
                    //订单数量
                    int orderQuantity = db.SMT_OrderInfo.Where(c => c.OrderNum == planorder).FirstOrDefault() == null ? 0 : db.SMT_OrderInfo.Where(c => c.OrderNum == planorder).FirstOrDefault().Quantity;
                    //产线信息
                    var lineInfo = db.SMT_ProductionLineInfo.Where(c => c.LineNum == linenum).FirstOrDefault();

                    //建一个List装一行信息
                    List<string> detailInfor_planorder = new List<string>();
                    //往List填写“平台”信息
                    detailInfor_planorder.Add(db.SMT_OrderInfo.Where(c => c.OrderNum == planorder).FirstOrDefault() == null ? "" : db.SMT_OrderInfo.Where(c => c.OrderNum == planorder).FirstOrDefault().PlatformType);
                    //往List填写“订单数量”信息
                    detailInfor_planorder.Add(orderQuantity == 0 ? "" : orderQuantity.ToString());
                    //往List填写“日计划数量”信息
                    detailInfor_planorder.Add(todayPlanQuantity.ToString());
                    //往List填写“日生产完成数量”信息
                    detailInfor_planorder.Add((todayNormalcount + todayAbnormalcount).ToString());
                    //往List填写“日生产完成率”信息
                    detailInfor_planorder.Add(((todayNormalcount + todayAbnormalcount) * 100 / todayPlanQuantity).ToString("F2") + "%");
                    //往List填写“不良品”信息
                    detailInfor_planorder.Add(todayAbnormalcount.ToString());
                    //往List填写“良品”信息
                    detailInfor_planorder.Add(todayNormalcount.ToString());
                    //往List填写“不良率”信息
                    detailInfor_planorder.Add(todayAbnormalcount + todayNormalcount == 0 ? "0.00%" : (todayAbnormalcount * 100 / (todayAbnormalcount + todayNormalcount)).ToString("F2") + "%");
                    //往List填写“总良品”信息
                    detailInfor_planorder.Add(orderNormalcount.ToString());
                    //往List填写“总不良品”信息
                    detailInfor_planorder.Add(orderAbnormalcount.ToString());
                    //往List填写“总不良品率”信息
                    detailInfor_planorder.Add(orderNormalcount + orderAbnormalcount == 0 ? "0.00%" : (orderAbnormalcount * 100 / (orderNormalcount + orderAbnormalcount)).ToString("F2") + "%");
                    //往List填写“累计数量”信息
                    detailInfor_planorder.Add((orderNormalcount + orderAbnormalcount).ToString());
                    //往List填写“订单完成率”信息
                    detailInfor_planorder.Add(orderQuantity == 0 ? "0.00%" : ((orderNormalcount + orderAbnormalcount) * 100 / orderQuantity).ToString("F2") + "%");
                    //往List填写“班组”信息
                    detailInfor_planorder.Add(lineInfo.Team);
                    //往List填写“组长”信息
                    detailInfor_planorder.Add(lineInfo.GroupLeader);
                    //往List填写“状态”信息

                    if (TodayProductionData.Where(c => c.OrderNum == planorder).Count() > 0)
                    {
                        var subtime = DateTime.Now.Subtract(Convert.ToDateTime(TodayProductionData.Where(c => c.OrderNum == planorder).FirstOrDefault().ProductionDate));
                        if (subtime.TotalMinutes > 5)
                        {
                            detailInfor_planorder.Add("待生产");
                        }
                        else
                        {
                            detailInfor_planorder.Add("生产中");
                        }
                    }
                    else
                    {
                        detailInfor_planorder.Add("待生产");
                    }
                    LineOrderDetails.Add(planorder, JsonConvert.SerializeObject(detailInfor_planorder));
                }
                LineData.Add(linenum.ToString(), JsonConvert.SerializeObject(LineOrderDetails));
                LineOrderDetails = new JObject();
            }
            return Content(JsonConvert.SerializeObject(LineData));
        }

        //产线看板页面
        //[HttpPost]
        public ActionResult SMT_ProductionLineInfo(int LineNum)
        {
            //内容:痴线号，时间，班组，组长，正在生产的订单，良品数量，不良品数量，不良率，产线累计个数，订单完成率，今天计划订单，产线状态
            ViewBag.LineNum = LineNum;//获取产线号
            return View();
        }

        #endregion


        #region------------------生产操作(数据)
        // GET: SMT产线未段工位输入操作
        public ActionResult SMT_Operator()
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            //内容：用户名，产线号，正在生产的订单，时间，今天生产的订单及数量（良品、不良品）
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role != "SMT操作员" || ((Users)Session["User"]).Role != "系统管理员")
            {
                return View();
            }
            return Content("对不起，您的不能进入产线操作，请联系SMT看板管理员！");
        }

        [HttpPost]
        public ActionResult SMT_Operator(FormCollection fc)
        {
            ViewBag.OrderList = GetOrderList();//向View传递OrderNum订单号列表.
            SMT_ProductionData productiondata = new SMT_ProductionData();
            productiondata.OrderNum = fc["OrderNum"];
            productiondata.LineNum = Convert.ToInt32(fc["LineNum"]);
            productiondata.Operator = ((Users)Session["User"]).UserName;
            productiondata.NormalCount = Convert.ToInt32(fc["normalQ"]);
            productiondata.AbnormalCount = Convert.ToInt32(fc["abnormalQ"]);
            productiondata.BeginTime = Convert.ToDateTime(fc["begintime"]);
            productiondata.EndTime = Convert.ToDateTime(fc["endtime"]);
            productiondata.Result = Convert.ToBoolean(fc["result"]);
            productiondata.BarcodeNum = fc["barcodeNum"];
            productiondata.ProductionDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.SMT_ProductionData.Add(productiondata);
                db.SaveChanges();
            }
            return Content("<script>alert('保存成功！');window.location.href='../SMT/SMT_Operator';</script>");

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



        #region------------------其他方法

        //// GET: SMT/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: SMT/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: SMT/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: SMT/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: SMT/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: SMT/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: SMT/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        #endregion

        #region ---------------------------------------FinishStatus列表
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


        #region ---------------------------------------产线状态列表
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



        /// <summary>
        /// 对象转换成json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObject">需要格式化的对象</param>
        /// <returns>Json字符串</returns>
        public static string DataContractJsonSerialize<T>(T jsonObject)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            string json = null;
            using (MemoryStream ms = new MemoryStream()) //定义一个stream用来存发序列化之后的内容
            {
                serializer.WriteObject(ms, jsonObject);
                json = Encoding.UTF8.GetString(ms.GetBuffer()); //将stream读取成一个字符串形式的数据，并且返回
                ms.Close();
            }
            return json;
        }

        /// <summary>
        /// json字符串转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">要转换成对象的json字符串</param>
        /// <returns></returns>
        public static T DataContractJsonDeserialize<T>(string json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            T obj = default(T);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
            }
            return obj;
        }

    }
}
