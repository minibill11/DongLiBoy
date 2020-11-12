using JianHeMES.AuthAttributes;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static JianHeMES.Controllers.CommonalityController;

namespace JianHeMES.Controllers
{
    public class PlansController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: KPI
        public ActionResult KPI_Plan_Gantt_Query()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Plans", act = "KPI_Plan_Gantt_Query" });
            }
            return View();
        }
        public ActionResult Section_Enter()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Plans", act = "Section_Enter" });
            }
            return View();
        }

        public ActionResult KPI_Plan()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Plans", act = "KPI_Plan" });
            }
            return View();
        }

        public ActionResult KPI_Plan_Gantt_Add()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Plans", act = "KPI_Plan_Gantt_Add" });
            }
            return View();
        }


        // GET: KPI/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: KPI/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KPI/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: KPI/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: KPI/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: KPI/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: KPI/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #region 工段工序功能集合
        //显示工段工序
        public ActionResult DisplayPlanSectionParameter()
        {
            JArray result = new JArray();
            var value = db.Plan_SectionParameter.Select(c => c.Department).Distinct().ToList();
            foreach (var dep in value)
            {
                var group = db.Plan_SectionParameter.Where(c => c.Department == dep).Select(c => c.Group).Distinct().ToList();
                foreach (var groupitem in group)
                {
                    var seaction = db.Plan_SectionParameter.Where(c => c.Department == dep && c.Group == groupitem).Select(c => c.Section).Distinct().ToList();
                    foreach (var seac in seaction)
                    {
                        var process = db.Plan_SectionParameter.Where(c => c.Department == dep && c.Group == groupitem && c.Section == seac).Select(c => c.Process).Distinct().ToList();
                        foreach (var proitem in process)
                        {
                            var iteminfo = db.Plan_SectionParameter.Where(c => c.Department == dep && c.Group == groupitem && c.Section == seac && c.Process == proitem && c.IndicatorsType == "效率指标").FirstOrDefault();
                            JObject obj = new JObject();
                            obj.Add("Department", dep);
                            obj.Add("Group", groupitem);
                            obj.Add("Section", seac);
                            obj.Add("Process", proitem);
                            if (iteminfo != null)
                            {
                                obj.Add("Xid", iteminfo.Id);
                                obj.Add("XTable", iteminfo.Table);
                                obj.Add("XFinshTime", iteminfo.FinshTime);
                                obj.Add("XFinsh", iteminfo.Finsh);
                                obj.Add("XDerparment", iteminfo.Derparment);
                            }
                            else
                            {
                                obj.Add("Xid", null);
                                obj.Add("XTable", null);
                                obj.Add("XFinshTime", null);
                                obj.Add("XFinsh", null);
                                obj.Add("XDerparment", null);
                            }
                            var iteminfo2 = db.Plan_SectionParameter.Where(c => c.Department == dep && c.Group == groupitem && c.Section == seac && c.Process == proitem && c.IndicatorsType == "品质指标").FirstOrDefault();
                            if (iteminfo2 != null)
                            {
                                obj.Add("Pid", iteminfo2.Id);
                                obj.Add("PTable", iteminfo2.Table);
                                obj.Add("PFinshTime", iteminfo2.FinshTime);
                                obj.Add("PFinsh", iteminfo2.Finsh);
                                obj.Add("PDerparment", iteminfo2.Derparment);
                            }
                            else
                            {
                                obj.Add("Pid", null);
                                obj.Add("PTable", null);
                                obj.Add("PFinshTime", null);
                                obj.Add("PFinsh", null);
                                obj.Add("PDerparment", null);
                            }
                            result.Add(obj);
                        }
                    }

                }

            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //输入工段工序
        public ActionResult AddPlanSectionParameter(string Department, string Group, string seaction, string process, string XTable, string XFinshTime, string XFinsh, string XDerparment, string PTable, string PFinshTime, string PFinsh, string PDerparment)
        {
            JObject obj = new JObject();

            var info = db.Plan_SectionParameter.Count(c => c.Department == Department && c.Group == Group && c.Process == process && c.Section == seaction);
            if (info != 0)
            {
                obj.Add("mes", "已有重复工段工序");
                obj.Add("id", null);
                obj.Add("Department", null);
                obj.Add("Group", null);
                obj.Add("Process", null);
                obj.Add("Section", null);
                obj.Add("XTable", null);
                obj.Add("XFinshTime", null);
                obj.Add("XFinsh", null);
                obj.Add("XDerparment", null);
                obj.Add("PTable", null);
                obj.Add("PFinshTime", null);
                obj.Add("PFinsh", null);
                obj.Add("PDerparment", null);
                return Content(JsonConvert.SerializeObject(obj));
            }
            Plan_SectionParameter plan_ = new Plan_SectionParameter() { Department = Department, Group = Group, Process = process, Section = seaction, IndicatorsType = "品质指标", Table = PTable, FinshTime = PFinshTime, Finsh = PFinsh, Derparment = PDerparment, Createor = ((Users)Session["User"]) == null ? "测试人员" : ((Users)Session["User"]).UserName, CreateTime = DateTime.Now };
            Plan_SectionParameter plan_2 = new Plan_SectionParameter() { Department = Department, Group = Group, Process = process, Section = seaction, IndicatorsType = "效率指标", Table = XTable, FinshTime = XFinshTime, Finsh = XFinsh, Derparment = XDerparment, Createor = ((Users)Session["User"]) == null ? "测试人员" : ((Users)Session["User"]).UserName, CreateTime = DateTime.Now };
            db.Plan_SectionParameter.Add(plan_);
            db.Plan_SectionParameter.Add(plan_2);
            db.SaveChanges();

            var item = db.Plan_SectionParameter.Where(c => c.Department == Department && c.Group == Group && c.Process == process && c.Section == seaction && c.IndicatorsType == "效率指标").FirstOrDefault();
            var item2 = db.Plan_SectionParameter.Where(c => c.Department == Department && c.Group == Group && c.Process == process && c.Section == seaction && c.IndicatorsType == "品质指标").FirstOrDefault();
            obj.Add("mes", "新增成功");

            obj.Add("Department", item.Department);
            obj.Add("Group", item.Group);
            obj.Add("Process", item.Process);
            obj.Add("Section", item.Section);
            if (item != null)
            {
                obj.Add("Xid", item.Id);
                obj.Add("XTable", item.Table);
                obj.Add("XFinshTime", item.FinshTime);
                obj.Add("XFinsh", item.Finsh);
                obj.Add("XDerparment", item.Derparment);
            }
            else
            {
                obj.Add("XTable", null);
                obj.Add("XFinshTime", null);
                obj.Add("XFinsh", null);
                obj.Add("XDerparment", null);
            }
            if (item2 != null)
            {
                obj.Add("Pid", item2.Id);
                obj.Add("PTable", item2.Table);
                obj.Add("PFinshTime", item2.FinshTime);
                obj.Add("PFinsh", item2.Finsh);
                obj.Add("PDerparment", item2.Derparment);
            }
            else
            {
                obj.Add("Pid", null);
                obj.Add("PTable", null);
                obj.Add("PFinshTime", null);
                obj.Add("PFinsh", null);
                obj.Add("PDerparment", null);
            }
            return Content(JsonConvert.SerializeObject(obj));
        }

        //修改工段工序
        public string UpdatePlanSectionParameter(int xid, int pid, string newProcess, string newSection, string newDepartment, string newGroup, string XTable, string XFinshTime, string XFinsh, string XDerparment, string PTable, string PFinshTime, string PFinsh, string PDerparment)
        {
            var info = db.Plan_SectionParameter.Find(xid);
            if (info == null)
            {
                return "找不到该信息记录";
            }
            if (info.Process != newProcess || info.Section != newSection || info.Department != newDepartment || info.Group != newGroup)
            {
                //判断是否有重复的记录
                var check = db.Plan_SectionParameter.Count(c => c.Process == newProcess && c.Section == newSection && c.Department == newDepartment && c.Group == newGroup);
                if (check != 0)
                {
                    return "已有重复记录";
                }
            }
            // var info = info.Where(c => c.IndicatorsType == "效率").FirstOrDefault();
            info.Department = newDepartment;
            info.Group = newGroup;
            info.Process = newProcess;
            info.Section = newSection;
            info.Table = XTable;
            info.FinshTime = XFinshTime;
            info.Finsh = XFinsh;
            info.Derparment = XDerparment;
            var qinfo = db.Plan_SectionParameter.Find(pid);
            qinfo.Department = newDepartment;
            qinfo.Group = newGroup;
            qinfo.Process = newProcess;
            qinfo.Section = newSection;
            qinfo.Table = PTable;
            qinfo.FinshTime = PFinshTime;
            qinfo.Finsh = PFinsh;
            qinfo.Derparment = PDerparment;

            db.SaveChanges();
            return "修改成功";
        }

        //删除工段工序
        public string DeletePlanSectionParameter(int xid, int pid)
        {
            var info = db.Plan_SectionParameter.Find(xid);
            var info2 = db.Plan_SectionParameter.Find(pid);
            if (info == null)
            {
                return "找不到改信息记录";
            }
            db.Plan_SectionParameter.Remove(info);
            db.Plan_SectionParameter.Remove(info2);
            db.SaveChanges();
            return "删除成功";
        }
        #endregion

        #region 各工段工序计划录入
        //显示
        public ActionResult DisplayPlan_FromKPI(string ordernum, string deparment, string group, string process, string section, DateTime? time)
        {
            JArray result = new JArray();
            var totalvalue = db.Plan_FromKPI.ToList();
            if (!string.IsNullOrEmpty(ordernum))
            {
                totalvalue = totalvalue.Where(c => c.OrderNum == ordernum).ToList();
            }
            if (!string.IsNullOrEmpty(deparment))
            {
                totalvalue = totalvalue.Where(c => c.Department == deparment).ToList();
            }
            if (!string.IsNullOrEmpty(group))
            {
                totalvalue = totalvalue.Where(c => c.Group == group).ToList();
            }
            if (!string.IsNullOrEmpty(process))
            {
                totalvalue = totalvalue.Where(c => c.Process == process).ToList();
            }
            if (!string.IsNullOrEmpty(section))
            {
                totalvalue = totalvalue.Where(c => c.Section == section).ToList();
            }

            if (time != null)
            {
                totalvalue = totalvalue.Where(c => c.PlanTime >= time && c.PlanTime < time.Value.AddMonths(1)).ToList();
            }
            foreach (var item in totalvalue)
            {
                JObject obj = new JObject();
                obj.Add("ID", item.Id); //订单
                obj.Add("OrderNum", item.OrderNum); //订单
                obj.Add("Department", item.Department);//部门
                obj.Add("Group", item.Group);//班组
                obj.Add("Process", item.Process);//工段
                obj.Add("Section", item.Section);//工序
                obj.Add("IndicatorsType", item.IndicatorsType);//品质或者效率
                obj.Add("CheckDepartment", item.CheckDepartment);//检验部门
                obj.Add("CheckGroup", item.CheckGroup);//检验班组
                obj.Add("CheckSection", item.CheckSection);//抽检或全检
                obj.Add("CheckProcess", item.CheckProcess);//抽检或全检
                obj.Add("CheckNum", item.CheckNum);//抽检或全检
                obj.Add("CheckType", item.CheckType);//抽检或全检
                obj.Add("PlanTime", item.PlanTime.ToString());//计划时间
                obj.Add("PlanNum", item.PlanNum);//计划数量
                obj.Add("PlanCreateor", item.PlanCreateor);//创建人员
                result.Add(obj);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //新增
        public string AddPlan_FromKPI(List<Plan_FromKPI> Record)
        {
            if (Session["User"] == null)
            {
                return "Login";
            }
            string error = "";
            foreach (var item in Record)
            {
                var info = db.Plan_FromKPI.Count(c => c.PlanTime == item.PlanTime && c.OrderNum == item.OrderNum && c.Department == item.Department && c.Group == item.Group && c.Process == item.Process && c.Section == item.Section);
                if (info != 0)
                {
                    error = error + item.OrderNum + "在" + item.PlanTime + "已有" + item.Department + item.Group + "的计划记录.";

                }
            }
            if (string.IsNullOrEmpty(error))
            {
                Record.ForEach(c => { c.PlanCreateor = ((Users)Session["User"]).UserName; c.PlanCreateTime = DateTime.Now; });
                db.Plan_FromKPI.AddRange(Record);
                db.SaveChanges();
                return "新增成功";
            }
            else
            {
                return error;
            }

        }
        //修改
        public ActionResult UpdatePlan_FromKPI(int id, Plan_FromKPI Record)
        {
            var info = db.Plan_FromKPI.Find(id);
            JObject result = new JObject();
            if (info == null)
            {
                result.Add("mes", "没有找到数据");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));
            }
            if (((Users)Session["User"]).UserName != info.PlanCreateor && ((Users)Session["User"]).Role != "系统管理员")
            {
                result.Add("mes", "你没有权限删除该条信息");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));

            }
            info.OrderNum = Record.OrderNum;
            info.Department = Record.Department;
            info.Group = Record.Group;
            info.Process = Record.Process;
            info.Section = Record.Section;
            info.IndicatorsType = Record.IndicatorsType;
            info.CheckDepartment = Record.CheckDepartment;
            info.CheckGroup = Record.CheckGroup;
            info.CheckProcess = Record.CheckProcess;
            info.CheckSection = Record.CheckSection;
            info.CheckNum = Record.CheckNum;
            info.CheckType = Record.CheckType;
            info.PlanTime = Record.PlanTime;
            info.PlanNum = Record.PlanNum;

            db.SaveChanges();
            result.Add("mes", "修改成功");
            result.Add("pass", true);
            return Content(JsonConvert.SerializeObject(result));

        }

        //删除

        public ActionResult DeletePlan_FromKPI(List<int> id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Plans", act = "KPI_Plan" });
            }
            var info = db.Plan_FromKPI.Where(c => id.Contains(c.Id));
            JObject result = new JObject();
            if (info.Count() == 0)
            {
                result.Add("mes", "找不到该信息");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));
            }

            if ((info.Select(c => c.PlanCreateor).Distinct().Count() > 1 || info.Select(c => c.PlanCreateor).FirstOrDefault() != ((Users)Session["User"]).UserName) && ((Users)Session["User"]).Role != "系统管理员")
            {
                var name = ((Users)Session["User"]).UserName;
                var username = info.Where(c => c.PlanCreateor != name).Select(c => c.PlanCreateor).Distinct().ToList();
                result.Add("mes", "记录包含" + string.Join(",", username) + "的记录,你没有权限删除该条信息");
                result.Add("pass", false);
                return Content(JsonConvert.SerializeObject(result));

            }

            db.Plan_FromKPI.RemoveRange(info);
            db.SaveChanges();
            UserOperateLog log = new UserOperateLog()
            {
                OperateDT = DateTime.Now,
                Operator = ((Users)Session["User"]).UserName,
                OperateRecord = "删除计划数据" + string.Join(" ", info.Select(c => c.Department).Distinct().ToList()) + string.Join(" ", info.Select(c => c.Group).Distinct().ToList())
            };
            db.UserOperateLog.Add(log);
            db.SaveChanges();
            result.Add("mes", "删除成功");
            result.Add("pass", true);
            return Content(JsonConvert.SerializeObject(result));
        }


        //表格导出
        public FileContentResult ExportExce()
        {
            string[] columns = { "部门", "班组", "工段", "工序" };

            var centeruserlist = db.Plan_SectionParameter.Select(c => new { c.Department, c.Group, c.Section, c.Process }).Distinct().ToList();

            byte[] filecontent = ExcelExportHelper.ExportExcel(centeruserlist, "工段工序", false, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, "工段工序" + ".xlsx");
        }
        #endregion
    }
    
    public class Plans_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonController com = new CommonController();

        #region 工段工序功能集合
        //显示工段工序(只显示部门班组工段工序)
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayPlanSectionParameter()
        {
            JArray result = new JArray();
            var value = db.Plan_SectionParameter.Select(c => c.Department).Distinct().ToList();
            foreach (var dep in value)
            {
                var group = db.Plan_SectionParameter.Where(c => c.Department == dep).Select(c => c.Group).Distinct().ToList();
                foreach (var groupitem in group)
                {
                    var seaction = db.Plan_SectionParameter.Where(c => c.Department == dep && c.Group == groupitem).Select(c => c.Section).Distinct().ToList();
                    foreach (var seac in seaction)
                    {
                        var process = db.Plan_SectionParameter.Where(c => c.Department == dep && c.Group == groupitem && c.Section == seac).Select(c => c.Process).Distinct().ToList();
                        foreach (var proitem in process)
                        {
                            var info = db.Plan_SectionParameter.Where(c => c.Department == dep && c.Group == groupitem && c.Section == seac && c.Process == proitem && c.IndicatorsType == "效率指标").FirstOrDefault();
                            JObject obj = new JObject();
                            obj.Add("id", info.Id);
                            obj.Add("Department", dep);
                            obj.Add("Group", groupitem);
                            obj.Add("Section", seac);
                            obj.Add("Process", proitem);
                            result.Add(obj);
                        }
                    }

                }

            }
            return com.GetModuleFromJarray(result);
        }

        //输入工段工序(只输入部门班组工段工序)
        [HttpPost]
        [ApiAuthorize]
        public JObject AddPlanSectionParameter([System.Web.Http.FromBody]JObject data)
        {
            string Department = data["Department"].ToString();
            string Group = data["Group"].ToString();
            string process = data["process"].ToString();
            string seaction = data["seaction"].ToString();
            string UserName = data["UserName"].ToString();

            JObject result = new JObject();
            var info = db.Plan_SectionParameter.Count(c => c.Department == Department && c.Group == Group && c.Process == process && c.Section == seaction);
            if (info != 0)
            {
                return com.GetModuleFromJobjet(null, false, "已有重复工段工序");
            }

            Plan_SectionParameter plan_ = new Plan_SectionParameter() { Department = Department, Group = Group, Process = process, Section = seaction, Createor = UserName, CreateTime = DateTime.Now, IndicatorsType = "效率指标" };
            db.Plan_SectionParameter.Add(plan_);
            db.SaveChanges();

            result.Add("id", plan_.Id);
            result.Add("Department", Department);
            result.Add("Group", Group);
            result.Add("Process", process);
            result.Add("Section", seaction);

            return com.GetModuleFromJobjet(result, true, "新增成功");
        }

        //修改工段工序
        [HttpPost]
        [ApiAuthorize]
        public JObject UpdatePlanSectionParameter([System.Web.Http.FromBody]JObject data)
        {
            int id = int.Parse(data["id"].ToString());
            string newDepartment = data["newDepartment"].ToString();
            string newGroup = data["newGroup"].ToString();
            string newProcess = data["newProcess"].ToString();
            string newSection = data["newSection"].ToString();
            string UserName = data["UserName"].ToString();

            var info = db.Plan_SectionParameter.Find(id);
            if (info == null)
            {
                return com.GetModuleFromJarray(null, false, "找不到该信息记录");
            }
            if (info.Process != newProcess || info.Section != newSection || info.Department != newDepartment || info.Group != newGroup)
            {
                //判断是否有重复的记录
                var check = db.Plan_SectionParameter.Count(c => c.Process == newProcess && c.Section == newSection && c.Department == newDepartment && c.Group == newGroup);
                if (check != 0)
                {
                    return com.GetModuleFromJarray(null, false, "已有重复记录");
                }
            }
            UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = UserName, OperateRecord = "修改工段工序,从" + info.Department + info.Group + info.Process + info.Section + "改为" + newDepartment + newGroup + newProcess + newSection };
            info.Department = newDepartment;
            info.Group = newGroup;
            info.Process = newProcess;
            info.Section = newSection;
            db.UserOperateLog.Add(log);
            db.SaveChanges();

            return com.GetModuleFromJarray(null, true, "修改成功");
        }

        //删除工段工序
        [HttpPost]
        [ApiAuthorize]
        public JObject DeletePlanSectionParameter([System.Web.Http.FromBody]JObject data)
        {
            int id =int.Parse( data["id"].ToString());
            string UserName = data["UserName"].ToString();
            var info = db.Plan_SectionParameter.Find(id);
            if (info == null)
            {
                return com.GetModuleFromJarray(null,false,"找不到改信息记录");
            }
            db.Plan_SectionParameter.Remove(info);
            UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = UserName, OperateRecord = "删除工段工序," + info.Department + info.Group + info.Process + info.Section };
            db.UserOperateLog.Add(log);
            db.SaveChanges();

            return com.GetModuleFromJarray(null, true, "删除成功");
        }
        #endregion

        #region 各工段工序计划录入
        //显示
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayPlan_FromKPI([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string ordernum = datavalue.ordernum;//订单
            string deparment = datavalue.deparment;//部门
            string group = datavalue.group;//班组
            string section = datavalue.section;//工段
            string process = datavalue.process;//工序
            DateTime? time = datavalue.time;//时间

            JArray result = new JArray();
            var totalvalue = db.Plan_FromKPI.ToList();
            if (!string.IsNullOrEmpty(ordernum))
            {
                totalvalue = totalvalue.Where(c => c.OrderNum == ordernum).ToList();
            }
            if (!string.IsNullOrEmpty(deparment))
            {
                totalvalue = totalvalue.Where(c => c.Department == deparment).ToList();
            }
            if (!string.IsNullOrEmpty(group))
            {
                totalvalue = totalvalue.Where(c => c.Group == group).ToList();
            }
            if (!string.IsNullOrEmpty(process))
            {
                totalvalue = totalvalue.Where(c => c.Process == process).ToList();
            }
            if (!string.IsNullOrEmpty(section))
            {
                totalvalue = totalvalue.Where(c => c.Section == section).ToList();
            } 

            if (time != null)
            {
                totalvalue = totalvalue.Where(c => c.PlanTime >= time && c.PlanTime < time.Value.AddMonths(1)).ToList();
            }
            foreach (var item in totalvalue)
            {
                JObject obj = new JObject();
                obj.Add("ID", item.Id); //订单
                obj.Add("OrderNum", item.OrderNum); //订单
                obj.Add("Department", item.Department);//部门
                obj.Add("Group", item.Group);//班组
                obj.Add("Process", item.Process);//工段
                obj.Add("Section", item.Section);//工序
                obj.Add("IndicatorsType", item.IndicatorsType);//品质或者效率
                obj.Add("CheckDepartment", item.CheckDepartment);//检验部门
                obj.Add("CheckGroup", item.CheckGroup);//检验班组
                obj.Add("CheckSection", item.CheckSection);//抽检或全检
                obj.Add("CheckProcess", item.CheckProcess);//抽检或全检
                obj.Add("CheckNum", item.CheckNum);//抽检或全检
                obj.Add("CheckType", item.CheckType);//抽检或全检
                obj.Add("PlanTime", item.PlanTime.ToString());//计划时间
                obj.Add("PlanNum", item.PlanNum);//计划数量
                obj.Add("PlanCreateor", item.PlanCreateor);//创建人员
                result.Add(obj);
            }
            return com.GetModuleFromJarray(result);
        }

        //新增
        [HttpPost]
        [ApiAuthorize]
        public JObject AddPlan_FromKPI([System.Web.Http.FromBody]JObject data)
        {
            List<Plan_FromKPI> Record = JsonConvert.DeserializeObject<List<Plan_FromKPI>>(JsonConvert.SerializeObject(data["Record"]));
            string UserName = data["UserName"].ToString();
            string error = "";
            foreach (var item in Record)
            {
                var info = db.Plan_FromKPI.Count(c => c.PlanTime == item.PlanTime && c.OrderNum == item.OrderNum && c.Department == item.Department && c.Group == item.Group && c.Process == item.Process && c.Section == item.Section);
                if (info != 0)
                {
                    error = error + item.OrderNum + "在" + item.PlanTime + "已有" + item.Department + item.Group + "的计划记录.";

                }
            }
            if (string.IsNullOrEmpty(error))
            {
                Record.ForEach(c => { c.PlanCreateor =UserName; c.PlanCreateTime = DateTime.Now; });
                db.Plan_FromKPI.AddRange(Record);
                db.SaveChanges();
                return com.GetModuleFromJarray(null,true,"新增成功");
            }
            else
            {
                return com.GetModuleFromJarray(null, false, error);
            }

        }
        //修改
        public JObject UpdatePlan_FromKPI([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));

            int id = datavalue.id;
            string UserName = datavalue.UserName;
            string Role = datavalue.Role;
            Plan_FromKPI Record = JsonConvert.DeserializeObject<Plan_FromKPI>(JsonConvert.SerializeObject(datavalue.Record));

            var info = db.Plan_FromKPI.Find(id);
            JObject result = new JObject();
            if (info == null)
            {
                return com.GetModuleFromJarray(null,false, "没有找到数据");
            }
            if (UserName != info.PlanCreateor && Role != "系统管理员")
            {
                return com.GetModuleFromJarray(null, false, "你没有权限删除该条信息");

            }
            UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = UserName, OperateRecord = "修改KPI计划" };
            info.OrderNum = Record.OrderNum;
            info.Department = Record.Department;
            info.Group = Record.Group;
            info.Process = Record.Process;
            info.Section = Record.Section;
            info.IndicatorsType = Record.IndicatorsType;
            info.CheckDepartment = Record.CheckDepartment;
            info.CheckGroup = Record.CheckGroup;
            info.CheckProcess = Record.CheckProcess;
            info.CheckSection = Record.CheckSection;
            info.CheckNum = Record.CheckNum;
            info.CheckType = Record.CheckType;
            info.PlanTime = Record.PlanTime;
            info.PlanNum = Record.PlanNum;
            db.UserOperateLog.Add(log);
            db.SaveChanges();
            result.Add("mes", "修改成功");
            result.Add("pass", true);
            return com.GetModuleFromJarray(null, true, "修改成功");

        }

        //删除

        public JObject DeletePlan_FromKPI([System.Web.Http.FromBody]JObject data)
        {
            List<int> id = JsonConvert.DeserializeObject<List<int>>(JsonConvert.SerializeObject(data["id"]));
            string UserName = data["UserName"].ToString();
            string Role = data["UserName"].ToString();

            var info = db.Plan_FromKPI.Where(c => id.Contains(c.Id));
            JObject result = new JObject();
            if (info.Count() == 0)
            {
                return com.GetModuleFromJarray(null,false, "找不到该信息");
            }

            if ((info.Select(c => c.PlanCreateor).Distinct().Count() > 1 || info.Select(c => c.PlanCreateor).FirstOrDefault() != UserName) && Role != "系统管理员")
            {
                var name = UserName;
                var username = info.Where(c => c.PlanCreateor != name).Select(c => c.PlanCreateor).Distinct().ToList();
                result.Add("mes", "记录包含" + string.Join(",", username) + "的记录,你没有权限删除该条信息");
                result.Add("pass", false);
                return com.GetModuleFromJarray(null, false, "记录包含" + string.Join(",", username) + "的记录,你没有权限删除该条信息");

            }

            db.Plan_FromKPI.RemoveRange(info);
            db.SaveChanges();
            UserOperateLog log = new UserOperateLog()
            {
                OperateDT = DateTime.Now,
                Operator = UserName,
                OperateRecord = "删除计划数据" + string.Join(" ", info.Select(c => c.Department).Distinct().ToList()) + string.Join(" ", info.Select(c => c.Group).Distinct().ToList())
            };
            db.UserOperateLog.Add(log);
            db.SaveChanges();
            result.Add("mes", "删除成功");
            result.Add("pass", true);
            return com.GetModuleFromJarray(null, true, "删除成功");
        }


        #endregion
    }
    
}
