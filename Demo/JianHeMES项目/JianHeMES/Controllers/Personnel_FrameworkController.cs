﻿using System;
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
        private static Dictionary<int, Personnel_Framework> info = new Dictionary<int, Personnel_Framework>();
        // List<Personnel_Organization> organizationsList = new List<Personnel_Organization>();
        //static JObject ordoragration = new JObject();


        // GET: Personnel_Framework
        #region 页面
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Framework", act = "Index" });
            }
            return View();
        }

        public async Task<ActionResult> Index2()
        {
            return View(await db.Personnel_Framework.ToListAsync());
        }

        #endregion

        #region 原组织架构代码
        [HttpPost]
        public async Task<ActionResult> Index(string a)
        {
            if (info.Count != 0)
            {
                info.Clear();
            }
            JObject manageJobect = new JObject();
            JObject depatmentJobject = new JObject();
            JObject groupJobject = new JObject();
            JObject positionjobject = new JObject();
            var manageList = await db.Personnel_Framework.Select(c => c.Central_layer).Distinct().ToListAsync();
            //manageList.RemoveAll(null);
            int i = 0;
            foreach (var manage in manageList)
            {
                var depatmentList = await db.Personnel_Framework.Where(c => c.Central_layer == manage).Select(c => c.Department).Distinct().ToListAsync();
                //depatmentList.RemoveAll(null);
                foreach (var department in depatmentList)
                {
                    var dp_groupList = await db.Personnel_Framework.Where(c => c.Central_layer == manage && c.Department == department).Select(c => c.Group).Distinct().ToListAsync();
                    //dp_groupList.RemoveAll(null);
                    foreach (var group in dp_groupList)
                    {
                        var positionList = await db.Personnel_Framework.Where(c => c.Central_layer == manage && c.Department == department && c.Group == group).Select(c => c.Position).Distinct().ToListAsync();

                        foreach (var position in positionList)
                        {
                            var count = await db.Personnel_Roster.CountAsync(c => c.Department == department && c.DP_Group == group && c.Position == position && c.Status != "离职员工");
                            positionjobject.Add(i.ToString(), position + "(" + count + ")");
                            Personnel_Framework item = new Personnel_Framework { Central_layer = manage, Department = department, Group = group, Position = position };
                            info.Add(i, item);
                            i++;
                        }
                        groupJobject.Add(group == null ? "其他" : group, positionjobject);
                        positionjobject = new JObject();
                    }
                    depatmentJobject.Add(department == null ? "其他" : department, groupJobject);
                    groupJobject = new JObject();

                }
                manageJobect.Add(manage == null ? "其他" : manage, depatmentJobject);
                depatmentJobject = new JObject();

            }
            if (manageList.Count != 0)
            {
                var cc = Content(JsonConvert.SerializeObject(manageJobect));
                return Content(JsonConvert.SerializeObject(manageJobect));
            }
            return Content("获取数据失败");
        }
        #endregion

        #region 框架输出
        /// <summary>
        /// 通过版本号,上一节点的部门和标题找到下一节点的信息
        /// </summary>
        /// 通过部门和标题找到下一节点的信息,如果找到的数据为空,则根据部门和标题找花名册的人员信息,如果数据不为空,返回数据,并根据负责人的信息找到
        /// <param name="version">版本号</param>
        /// <param name="department">部门</param>
        /// <param name="title">标题</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Framework(string version = null, string department = null, string title = null,string userGroup = null)
        {
            var currentseaction = db.Personnel_Organization.ToList();//找打所有版本信息
            if (!string.IsNullOrEmpty(version))//有传版本号,读版本号数据
            {
                if (version == "初始版本")
                {
                    currentseaction = currentseaction.Where(c => c.Version == null).ToList();
                }
                else
                {
                    currentseaction = currentseaction.Where(c => c.Version == version).ToList();
                }
            }
            else//没有默认读临时版本
            {
                currentseaction = currentseaction.Where(c => c.Version == "TempVersion").ToList();
            }

            if (!string.IsNullOrEmpty(department) && !string.IsNullOrEmpty(title))//是否有传部门和title
            {
                currentseaction = currentseaction.Where(c => c.Superior == title && (department == "null" ? c.SuperiorDepartment == null : c.SuperiorDepartment == department)).ToList();
            }
            else
            {
                currentseaction = currentseaction.Where(c => c.Superior == null && c.SuperiorDepartment == null).ToList();
            }


            JArray result = new JArray();
            if (currentseaction.Count == 0)//如果没找到数据,表示是最低节点职位,直接根据部门和职位在花名测找到人员信息
            {
                var belog = db.Personnel_Organization.Where(c => c.Subordinate == title && c.SubordinateDepartment == department&&c.Superior== userGroup).FirstOrDefault();
                if (belog.IsBelong == 4)//Belong=4代表此节点是职位节点,那后面的节点内容从花名册拿,否则就空着
                {
                    //查找花名册
                    var userinfo = db.Personnel_Roster.Where(c => c.Department == department && c.Position == title && c.Status != "离职员工" && c.DP_Group == belog.Superior).ToList();
                    
                    foreach (var item in userinfo)
                    {
                        JObject jobitem = new JObject();
                        jobitem.Add("id", "人");
                        //标题
                        jobitem.Add("title", item.Name);
                        //部门
                        jobitem.Add("department", item.Department);
                        //职位
                        jobitem.Add("position", item.Position);
                        //属于哪个层级
                        jobitem.Add("isBelogin", 5);

                        //姓名
                        jobitem.Add("name", item.Name);
                        //用户ID
                        jobitem.Add("userId", item.JobNum);
                        //用户属于的组
                        jobitem.Add("userGroup", item.DP_Group);
                        //在岗信息查看
                        var startime = DateTime.Now.Date;
                        var endtime = DateTime.Now.Date.AddDays(1);
                        var workinfo = db.Personnel_NotWorkingInfo.Where(c => c.Name == item.Name && c.jobNum == item.JobNum && c.CreateDate >= startime && c.CreateDate <= endtime).FirstOrDefault();
                        if (workinfo == null)
                            jobitem.Add("workStatue", "在岗");
                        else
                            jobitem.Add("workStatue", workinfo.Statue);

                        result.Add(jobitem);
                    }
                }
            }
            foreach (var item in currentseaction)//有数据则查找数据内容
            {
                JObject jobitem = new JObject();
                jobitem.Add("id", item.Id);
                //姓名
                jobitem.Add("name", item.Subordinate);
                //部门
                jobitem.Add("department", item.SubordinateDepartment);
                //职位
                jobitem.Add("position", item.Position);
                //属于哪个层级
                jobitem.Add("isBelogin", item.IsBelong);

                //查找花名册
                var userinfo = new Personnel_Roster();
                if (item.Subordinate == "工厂厂长")//厂长比较特殊,不用部门,直接根据职位填,写死
                {
                    userinfo = db.Personnel_Roster.Where(c => c.Position == "工厂厂长").FirstOrDefault();
                }
                else if (item.Name != null)//如果数据有记录Name这个值
                {
                    jobitem.Add("title", item.Name);
                    string name = item.Name;
                    if (item.Name.Contains("(兼)")) //判断是否是兼的部长
                    {
                        name = item.Name.Replace("(兼)", "");//把兼字去掉,然后用名字找花名册和今日员工在职状态信息
                    }
                    var user = db.Personnel_Roster.Where(c => c.Name == name && c.Status != "离职员工").FirstOrDefault();
                    if (user != null)
                    {
                        //用户ID
                        jobitem.Add("userId", user.JobNum);
                        //用户属于的组
                        jobitem.Add("userGroup", user.DP_Group);

                        //在岗信息查看
                        var startime = DateTime.Now.Date;
                        var endtime = DateTime.Now.Date.AddDays(1);
                        var workinfo = db.Personnel_NotWorkingInfo.Where(c => c.Name == name && c.jobNum == user.JobNum && c.CreateDate >= startime && c.CreateDate <= endtime).FirstOrDefault();
                        if (workinfo == null)
                            jobitem.Add("workStatue", "在岗");
                        else
                            jobitem.Add("workStatue", workinfo.Statue);
                        result.Add(jobitem);
                        continue;
                    }
                    else
                    {
                        //用户ID
                        jobitem.Add("userId", "000");
                        //用户属于的组
                        jobitem.Add("userGroup", item.Superior);

                        //在岗信息查看
                        jobitem.Add("workStatue", null);

                        result.Add(jobitem);
                        continue;
                    }
                }
                else
                {
                    userinfo = db.Personnel_Roster.Where(c => c.Department == item.SubordinateDepartment && c.Position == item.Position && c.DP_Group == item.Subordinate && c.Status != "离职员工").FirstOrDefault();
                }
                if (userinfo != null)
                {
                    //标题
                    jobitem.Add("title", userinfo.Name);
                    //用户ID
                    jobitem.Add("userId", userinfo.JobNum);
                    //用户属于的组
                    jobitem.Add("userGroup", userinfo.DP_Group);

                    //在岗信息查看
                    var startime = DateTime.Now.Date;
                    var endtime = DateTime.Now.Date.AddDays(1);
                    var workinfo = db.Personnel_NotWorkingInfo.Where(c => c.Name == userinfo.Name && c.jobNum == userinfo.JobNum && c.CreateDate >= startime && c.CreateDate <= endtime).FirstOrDefault();
                    if (workinfo == null)
                        jobitem.Add("workStatue", "在岗");
                    else
                        jobitem.Add("workStatue", workinfo.Statue);
                }
                else
                {
                    //姓名
                    jobitem.Add("title", "");
                    //用户ID
                    jobitem.Add("userId", "");
                    //用户属于的组
                    jobitem.Add("userGroup", item.Superior);

                    jobitem.Add("workStatue", "");
                }
                result.Add(jobitem);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        //修改人员在岗状态的权限设置
        public bool Alter(string userid, string loginid)
        {
            var departmentlist = new List<string>
                {
                    "PC部","MC部","SMT部","装配一部","装配二部","配套加工部","技术部","品质部","行政后勤部","人力资源部","财务部","制造中心","工厂厂长"
                };
            var Personal = db.Personnel_Roster.Where(c => c.JobNum == userid).FirstOrDefault();
            var info = db.Personnel_Roster.Where(c => c.JobNum == loginid).FirstOrDefault();


            if (info.DP_Group == "智造组")
            {
                return true;
            }
            if (info.Name == "刘露露")
            {
                if (Personal.Department != null)
                {
                    return true;
                }
            }
            if (info.Name == "桂玲玲")
            {
                if (Personal.Department == "PC部" || Personal.Department == "MC部")
                {
                    return true;
                }
            }
            else
            {
                if (Personal.Department == info.Department && info.Position == "文员")
                {
                    return true;
                }
            }
            return false;
        }

        //点击部门显示表格信息
        public ActionResult DepartmentUserInfo(string department)
        {
            JObject info = new JObject();
            var startime = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
            var endtime = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            //编制人数
            var Aurhorized = db.Personnel_daily.Where(c => c.Department == department && c.Date >= startime && c.Date <= endtime).Select(c => c.Aurhorized_personnel).FirstOrDefault();
            info.Add("Aurhorized", Aurhorized);
            //实际人数
            var Employees = db.Personnel_daily.Where(c => c.Department == department && c.Date >= startime && c.Date <= endtime).Select(c => c.Employees_personnel).FirstOrDefault();
            info.Add("Employees", Employees);
            //缺编人数
            var count = db.Personnel_Roster.Count(c => c.Department == department && c.Position == "储备干部");
            info.Add("LackAurhorized", Aurhorized - (Employees - count));
            //请假/旷工人数
            var notonduty = db.Personnel_NotWorkingInfo.Count(c => c.Department == department && c.CreateDate >= startime && c.CreateDate <= endtime && (c.Statue == "请假" || c.Statue == "临时务工"));
            info.Add("Notonduty", notonduty);
            //出勤人数
            info.Add("Attendance", Employees - notonduty);
            //待离职人数
            var waitQuit = db.Personnel_NotWorkingInfo.Count(c => c.Department == department && c.CreateDate >= startime && c.CreateDate <= endtime && c.Statue == "学生工");
            info.Add("waitQuit", waitQuit);
            return Content(JsonConvert.SerializeObject(info));
        }

        #endregion

        #region 组织架构修改员工在岗状态
        //修改员工状态
        public string UpateStatus(string userid, string statue)
        {
            var startime = DateTime.Now.Date;
            var endtime = DateTime.Now.Date.AddDays(1);
            var info = db.Personnel_NotWorkingInfo.Where(c => c.jobNum == userid && c.CreateDate >= startime && c.CreateDate <= endtime).FirstOrDefault();
            if (info != null)
            {
                if (statue == "在岗")
                {
                    db.Personnel_NotWorkingInfo.Remove(info);
                    db.SaveChanges();
                }
                else
                {
                    info.Statue = statue;
                    db.Entry(info).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                if (statue != "在岗")
                {
                    var userinfo = db.Personnel_Roster.Where(c => c.JobNum == userid).FirstOrDefault();
                    Personnel_NotWorkingInfo newInfo = new Personnel_NotWorkingInfo() { Name = userinfo.Name, Statue = statue, jobNum = userid, Department = userinfo.Department, DP_group = userinfo.DP_Group, Position = userinfo.Position, CreateDate = DateTime.Now, Creator = ((Users)Session["User"]).UserName };
                    db.Personnel_NotWorkingInfo.Add(newInfo);
                    db.SaveChanges();
                }
            }
            return statue;
        }
        #endregion

        #region 修改架构内人员信息，关联花名册
        //显示用户信息
        public ActionResult DisplayUserInfo(string department, string position, string group)
        {
            var userinfo = db.Personnel_Organization.Where(c => c.SubordinateDepartment == department && c.Position == position && c.Subordinate == group).FirstOrDefault();
            JArray result = new JArray();
            JObject jobitem = new JObject();
            jobitem.Add("name", userinfo.Name);
            result.Add(jobitem);

            return Content(JsonConvert.SerializeObject(result));
        }
        //修改用户的信息
        public bool GetUersInfo(string name, string jobnum, string deparment, string group, string position)
        {
            JObject userJobject = new JObject();
            var userinfo = db.Personnel_Organization.Where(c => c.SubordinateDepartment == deparment && c.Position == position && c.Subordinate == group).FirstOrDefault();
            if (userinfo != null)
            {
                userinfo.Name = name;
                db.SaveChanges();
                return true;
            }
            return false;

        }
        #endregion

        #region 节点操作相关代码
        //点击修改版本按钮
        public void NewVersion()
        {
            DelteVersion();
            var lastversion = db.Personnel_Organization.OrderByDescending(c => c.Id).Select(c => c.Version).FirstOrDefault();
            if (lastversion != "TempVersion")
            {
                var lastInfo = db.Personnel_Organization.Where(c => c.Version == lastversion).ToList();
                lastInfo.ForEach(c => c.Version = "TempVersion");
                db.Personnel_Organization.AddRange(lastInfo);
                db.SaveChanges();
            }
        }
        //点击修改版本按钮后，点击放弃保存按钮
        public bool DelteVersion()
        {
            var lastInfo = db.Personnel_Organization.Where(c => c.Version == "TempVersion").ToList();
            if (lastInfo.Count != 0)
            {
                db.Personnel_Organization.RemoveRange(lastInfo);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        //保存修改后的版本
        public bool AddVersion(string versionname = null)
        {
            if (versionname == null)
            {
                versionname = DateTime.Now.ToString();
            }
            var info = db.Personnel_Organization.Where(c => c.Version == "TempVersion").ToList();
            if (info.Count != 0)
            {
                if (db.Personnel_Organization.Count(c => c.Version == versionname) == 0)
                {
                    info.ForEach(c => c.Version = versionname);
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            return false;
        }
        //修改节点
        public ActionResult UpdateNoode(int id, string title, string position, string name)
        {
            JObject result = new JObject();
            var info = db.Personnel_Organization.Find(id);
            var currentinfo = db.Personnel_Organization.Where(c => c.Subordinate == info.Subordinate && c.SubordinateDepartment == info.SubordinateDepartment && c.Version == "TempVersion").FirstOrDefault();
            if (currentinfo == null)
            {
                result.Add("mes", "找不到当前节点信息");
                result.Add("pass", false);
                result.Add("id", id);
                result.Add("dep", null);
                return Content(JsonConvert.SerializeObject(result));
            }
            currentinfo.Name = name;
            string dep = currentinfo.SubordinateDepartment;
            //查找关联的子级
            var childList = db.Personnel_Organization.Where(c => c.Superior == currentinfo.Subordinate && c.SuperiorDepartment == currentinfo.SubordinateDepartment && c.Version == "TempVersion").ToList();
            if (childList.Count != 0)
            {
                childList.ForEach(c => c.Superior = title);
            }
            if (info.IsBelong < 3 && currentinfo.SubordinateDepartment != null)
            {
                currentinfo.SubordinateDepartment = title;
                childList.ForEach(c => c.SuperiorDepartment = title);
                if (info.IsBelong == 2)
                {
                    childList.ForEach(c => c.SubordinateDepartment = title);
                }
                dep = title;
            }
            currentinfo.Subordinate = title;
            currentinfo.Position = position;
            db.SaveChanges();

            result.Add("mes", "修改成功");
            result.Add("pass", true);
            result.Add("id", currentinfo.Id);
            result.Add("dep", dep);
            return Content(JsonConvert.SerializeObject(result));
        }

        //新增节点  
        public ActionResult AddNoode(int id, int fatherBelog, int childBelog, string title, string deparment, string position, string name)
        {
            JObject result = new JObject();
            if (fatherBelog > childBelog)
            {
                result.Add("mes", "子节点的层级大于父节点");
                result.Add("pass", false);
                result.Add("id", id);
                result.Add("isBelogin", fatherBelog);
                result.Add("deparment", null);
                return Content(JsonConvert.SerializeObject(result));
            }
            var info = db.Personnel_Organization.Find(id);
            var currentinfo = db.Personnel_Organization.Where(c => c.Superior == info.Superior && c.SuperiorDepartment == info.SuperiorDepartment && c.Subordinate == info.Subordinate && c.SubordinateDepartment == info.SubordinateDepartment && c.Version == "TempVersion").FirstOrDefault();
            Personnel_Organization organization = new Personnel_Organization() { Superior = currentinfo.Subordinate, SuperiorDepartment = currentinfo.SubordinateDepartment, Subordinate = name, SubordinateDepartment = deparment, CreateDate = DateTime.Now, Creator = ((Users)Session["User"]).UserName, IsBelong = childBelog, Position = position, Version = "TempVersion", Name = title };
            db.Personnel_Organization.Add(organization);
            db.SaveChanges();

            var newid = db.Personnel_Organization.Where(c => c.Superior == currentinfo.Subordinate && c.SuperiorDepartment == currentinfo.SubordinateDepartment && c.Subordinate == name && c.SubordinateDepartment == deparment && c.IsBelong == childBelog && c.Position == position && c.Version == "TempVersion" && c.Name == title).Select(c => c.Id).FirstOrDefault();

            result.Add("mes", "修改成功");
            result.Add("pass", true);
            result.Add("id", newid);
            result.Add("isBelogin", childBelog);
            result.Add("deparment", deparment);
            return Content(JsonConvert.SerializeObject(result));
        }

        //删除节点
        public string DelateNoode(int id)
        {
            var info = db.Personnel_Organization.Find(id);
            var currentinfo = db.Personnel_Organization.Where(c => c.Superior == info.Superior && c.SuperiorDepartment == info.SuperiorDepartment && c.Subordinate == info.Subordinate && c.SubordinateDepartment == info.SubordinateDepartment && c.Version == "TempVersion").FirstOrDefault();
            if (currentinfo == null)
            {
                return "找不到当前节点";
            }
            db.Personnel_Organization.Remove(currentinfo);
            db.SaveChanges();
            return "删除成功";
        }

        /// <summary>
        /// 修改或新增的节点标题相同,合并节点
        /// </summary>
        /// <param name="id">要合并的节点id</param>
        /// <param name="title">合并后的新标题</param>
        /// <param name="name">合并后的新负责人</param>
        /// <param name="belogin">要合并的是属于哪个层级的</param>
        /// <param name="sameName">被合并的标题名</param>
        public ActionResult Merge(int id, string title, string name, int belogin, string sameName)
        {
            JObject result = new JObject();
            var info = db.Personnel_Organization.Find(id);
            //查找要合并的节点信息
            var currentinfo2 = db.Personnel_Organization.Where(c => c.Subordinate == sameName && c.IsBelong == belogin && c.Version == "TempVersion").FirstOrDefault();
            if (currentinfo2 == null)
            {
                result.Add("mes", "找不到要合并的标题,请确认标题和层级的正确");
                result.Add("pass", false);
                result.Add("id", null);
                result.Add("dep", null);
                return Content(JsonConvert.SerializeObject(result));
            }
            //找到当前id的TempVersion节点数据
            var currentinfo = db.Personnel_Organization.Where(c => c.Superior == info.Superior && c.SuperiorDepartment == info.SuperiorDepartment && c.Subordinate == info.Subordinate && c.SubordinateDepartment == info.SubordinateDepartment && c.Version == "TempVersion").FirstOrDefault();
            if (currentinfo == null)
            {
                result.Add("mes", "找不到当前节点信息");
                result.Add("pass", false);
                result.Add("id", null);
                result.Add("dep", null);
                return Content(JsonConvert.SerializeObject(result));
            }
            string dep = "";
            if (currentinfo.IsBelong < 3)//修改的节点是部门和中心,所有的子节点都需要修改,不单是直接子节点
            {
                var subdeplist = db.Personnel_Organization.Where(c => c.SubordinateDepartment == currentinfo.SubordinateDepartment && c.Version == "TempVersion").ToList();//
                var supdeplist = db.Personnel_Organization.Where(c => c.SuperiorDepartment == currentinfo.SubordinateDepartment && c.Version == "TempVersion").ToList();//
                subdeplist.ForEach(c => c.SubordinateDepartment = title);
                supdeplist.ForEach(c => c.SuperiorDepartment = title);
                db.SaveChanges();
                //找到当前ID的子节列表
                var childList = db.Personnel_Organization.Where(c => c.Superior == currentinfo.Subordinate && c.SuperiorDepartment == title && c.Version == "TempVersion").ToList();
                if (childList.Count != 0)//修改子节点的数据
                {
                    childList.ForEach(c => c.Superior = title);
                }
                dep = title;
            }
            else
            {
                //找到当前ID的子节列表
                var childList = db.Personnel_Organization.Where(c => c.Superior == currentinfo.Subordinate && c.SuperiorDepartment == currentinfo.SubordinateDepartment && c.Version == "TempVersion").ToList();
                if (childList.Count != 0)//修改子节点的数据
                {
                    childList.ForEach(c => { c.Superior = title; c.SuperiorDepartment = currentinfo.SubordinateDepartment; });
                }
                dep = currentinfo.SubordinateDepartment;
            }
            //查找要合并的子节点信息
            var childList2 = db.Personnel_Organization.Where(c => c.Superior == currentinfo2.Subordinate && c.SuperiorDepartment == currentinfo2.SubordinateDepartment && c.Version == "TempVersion").ToList();
            if (childList2.Count != 0)//修改要合并子节点的数据
            {
                var childList3 = test(childList2);//找到要合并的所有子节点
                childList2.ForEach(c => { c.Superior = title; c.SuperiorDepartment = currentinfo.SubordinateDepartment; c.SubordinateDepartment = currentinfo.SubordinateDepartment; });
                childList3.ForEach(c => { c.SuperiorDepartment = currentinfo.SubordinateDepartment; c.SubordinateDepartment = currentinfo.SubordinateDepartment; });
            }

            //删掉要合并的节点
            db.Personnel_Organization.Remove(currentinfo2);
            //添加合并完成后节点的负责人
            currentinfo.Name = name;
            currentinfo.Subordinate = title;
            db.SaveChanges();
            result.Add("mes", "合并成功");
            result.Add("pass", true);
            result.Add("id", currentinfo.Id);
            result.Add("dep", dep);
            return Content(JsonConvert.SerializeObject(result));
        }


        //迭代器
        public List<Personnel_Organization> test(List<Personnel_Organization> value)
        {
            List<Personnel_Organization> result = new List<Personnel_Organization>();
            foreach (var item in value)
            {
                var childe = db.Personnel_Organization.Where(c => c.Superior == item.Subordinate && c.SuperiorDepartment == item.SubordinateDepartment && c.Version == "TempVersion").ToList();
                if (childe.Count != 0)
                {
                    result.Add(item);
                    result.AddRange(test(childe));
                }
                else
                    result.Add(item);
            }
            return result;
        }
        #endregion


        #region 版本列表获取
        public ActionResult VersionList()
        {
            var version = db.Personnel_Organization.OrderByDescending(c => c.Id).Where(c => c.Version != "TempVersion").Select(m => new { m.Version, m.Id }).ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            var version1 = version.Select(c => c.Version).Distinct().ToList();
            JArray result = new JArray();
            foreach (var item in version1)
            {
                JObject List = new JObject();
                if (item == null)
                    List.Add("value", "初始版本");
                else
                {
                    List.Add("value", item);
                }

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 暂时无用
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
        #endregion


        #region 以前组织架构图 代码
        ////返回员工状态
        //public string GetStatus(string userid)
        //{
        //    var startime = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
        //    var endtime = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        //    var status = db.Personnel_NotWorkingInfo.Where(c => c.jobNum == userid && c.CreateDate >= startime && c.CreateDate <= endtime).Select(c => c.Statue).FirstOrDefault();
        //    switch (status)
        //    {
        //        case 1:
        //            return "请假";
        //        case 2:
        //            return "新进";
        //        case 3:
        //            return "出差";
        //        case 4:
        //            return "旷工";
        //        case 5:
        //            return "辞职";
        //        case 6:
        //            return "临时工";
        //        default:
        //            return "在岗";
        //    }
        //}



        ////修改节点
        //public void updateNodde(string position, string Nodetitle, int mesID, string jobnum, string name)
        //{
        //    List<Personnel_Organization> updateList = new List<Personnel_Organization>();
        //    //string ss1 = "{\"id\": 445, \"userid\": [{\"name\": \"张丹媛\", \"status\": \"在岗\", \"department\": false,\"jobnum\": \"16621\"}], \"department\": \"false\",\"title\": \"备品测试2组\",\"name\": \"组长:张丹媛\",\"jobnum\": \"16621\"}";
        //    //JToken ss = (JToken)JsonConvert.DeserializeObject(ss1);
        //    //var id = int.Parse(ss["id"].ToString());

        //    var messagr = db.Personnel_Organization.Find(mesID);
        //    string[] SubordinateArray = messagr.Subordinate.Split(',');

        //    //var user = db.Personnel_Organization.Where(c => c.Superior == messagr.Subordinate).FirstOrDefault();

        //    //修改当前的节点

        //    string Subordinate = SubordinateArray[0];
        //    //string name = Nodename;
        //    string title = "";
        //    if (!string.IsNullOrEmpty(position) && position != " ")
        //    {
        //        var pos = position.Split(':');
        //        if (pos.Count() > 1)
        //        {
        //            title = Nodetitle + "," + pos[0];
        //        }
        //        //user.Subordinate = namearray[1];
        //    }
        //    else
        //        title = Nodetitle;

        //    if (title != messagr.Subordinate)
        //    {
        //        messagr.Subordinate = title;
        //        messagr.MessageID = mesID;
        //        messagr.Status = "修改";
        //        messagr.CreateDate = DateTime.Now;
        //        messagr.Creator = ((Users)Session["User"]).UserName;
        //        //修改当前节点的user信息
        //        //user.Superior = messagr.Subordinate;
        //        // user.UserID = Nodejobnum;
        //        // user.Status = "修改";
        //        //user.CreateDate = DateTime.Now;

        //        updateList.Add(messagr);
        //        //updateList.Add(user);

        //        //修改相关节点
        //        var connectNode = new List<Personnel_Organization>();
        //        if (messagr.SuperiorDepartment == null)
        //        {
        //            connectNode = db.Personnel_Organization.Where(c => c.Superior == Subordinate && c.SuperiorDepartment == Subordinate).ToList();
        //        }
        //        else
        //        {
        //            connectNode = db.Personnel_Organization.Where(c => c.Superior == Subordinate && c.SubordinateDepartment == messagr.SuperiorDepartment).ToList();

        //        }
        //        connectNode = GetList(connectNode);

        //        var connectNode1 = db.Personnel_Organization.Where(c => c.SuperiorDepartment == Subordinate).ToList();
        //        connectNode1 = GetList(connectNode1);

        //        var connectNode2 = db.Personnel_Organization.Where(c => c.SubordinateDepartment == Subordinate).ToList();
        //        connectNode2 = GetList(connectNode2);

        //        connectNode1.ForEach(c => { c.SuperiorDepartment = title; c.CreateDate = DateTime.Now; c.Status = "修改"; c.MessageID = c.Id; c.Creator = ((Users)Session["User"]).UserName; });
        //        connectNode.ForEach(c => { c.Superior = title; c.CreateDate = DateTime.Now; c.Status = "修改"; c.MessageID = c.Id; c.Creator = ((Users)Session["User"]).UserName; });
        //        connectNode2.ForEach(c => { c.SubordinateDepartment = title; c.CreateDate = DateTime.Now; c.Status = "修改"; c.MessageID = c.Id; c.Creator = ((Users)Session["User"]).UserName; });

        //        var cc = connectNode.Union(connectNode1).ToList();
        //        var bb = cc.Union(connectNode2).ToList();

        //        updateList.AddRange(bb);
        //        db.Personnel_Organization.AddRange(updateList);
        //        db.SaveChanges();
        //    }
        //    #region 修改花名册
        //    string dep = SubordinateArray[0];
        //    //部门或zhongx修改
        //    if (messagr.IsBelong == 1 || messagr.IsBelong == 2)
        //    {

        //        if (Nodetitle != dep)
        //        {
        //            var changList = db.Personnel_Roster.Where(c => c.Department == dep).ToList();
        //            changList.ForEach(c => c.Department = Nodetitle);
        //            var changList1 = db.Personnel_Roster.Where(c => c.DP_Group == dep).ToList();
        //            changList1.ForEach(c => c.DP_Group = Nodetitle);

        //        }
        //        if (!string.IsNullOrEmpty(position))
        //        {
        //            //if (jobnum != null)
        //            //{
        //            //    db.Personnel_Roster.Where(c => c.JobNum == jobnum).Select(c => c.Name).FirstOrDefault();

        //            //}
        //            string[] postions = position.Split(':');
        //            string positon = SubordinateArray[1];
        //            string updatepos = postions[0];
        //            var changList = db.Personnel_Roster.Where(c => c.Department == dep && c.DP_Group == dep && c.Position == positon).ToList();

        //            if (postions[0] != SubordinateArray[1])
        //            {
        //                changList.ForEach(c => c.Position = updatepos);
        //            }
        //            if (jobnum != null && name != null)
        //            {
        //                //var name = db.Personnel_Roster.Where(c => c.JobNum == jobnum).Select(c => c.Name).FirstOrDefault();
        //                changList.ForEach(c => { c.Name = name; c.JobNum = jobnum; });
        //            }
        //        }
        //        db.SaveChanges();
        //    }
        //    //职位修改
        //    else if (messagr.IsBelong == 5)
        //    {

        //        var changList1 = db.Personnel_Roster.Where(c => c.Position == dep && c.Department == messagr.SuperiorDepartment && c.DP_Group == messagr.Superior).ToList();
        //        changList1.ForEach(c => c.Position = Nodetitle);
        //        db.SaveChanges();

        //    }
        //    //车间或组修改
        //    else
        //    {
        //        if (Nodetitle != dep)
        //        {
        //            var changList1 = db.Personnel_Roster.Where(c => c.DP_Group == dep).ToList();
        //            changList1.ForEach(c => c.DP_Group = Nodetitle);

        //        }
        //        if (!string.IsNullOrEmpty(position))
        //        {
        //            //string[] namearray = Nodename.Split(':');
        //            if (position != SubordinateArray[1])
        //            {
        //                string positon = SubordinateArray[1];
        //                //string updatepos = namearray[0];
        //                var changList = db.Personnel_Roster.Where(c => c.Department == dep && c.DP_Group == dep && c.Position == positon).ToList();
        //                changList.ForEach(c => c.Position = position);
        //                if (jobnum != null && name != null)
        //                {
        //                    //var name = db.Personnel_Roster.Where(c => c.JobNum == jobnum).Select(c => c.Name).FirstOrDefault();
        //                    changList.ForEach(c => { c.Name = name; c.JobNum = jobnum; });
        //                }
        //            }

        //        }
        //        db.SaveChanges();
        //    }
        //    #endregion
        //}

        ////根据json修改表内容
        //public ActionResult ModifyTheNode(string newNode, List<string> AddMessage)
        //{
        //    //string aa = "{\"id\": \"1\", \"children\": [{ \"id\": \"769\", \"children\": [{ \"id\": \"770\",\"children\": [{ \"id\": \"771\"}, {\"id\": \"772\", \"children\": [{\"id\": \"773\",\"children\": [{ \"id\": \"userid14960\"},{ \"id\": \"userid15636\"}]}]}]}]}]}";
        //    // string bb = "{\"id\": \"1\", \"children\": [{ \"id\": \"431\", \"children\": [{ \"id\": \"436\",\"children\": [{ \"id\": \"444\"}, {\"id\": \"445\", \"children\": [{\"id\": \"449\",\"children\": [{\"id\":\"770\"},{ \"id\": \"773\"}]}]}]}]}]}";
        //    //string ee = "[{\"id\": 456456456, \"userid\": [{\"name\": \"张三\", \"status\": \"在岗\", \"department\": false,\"jobnum\": \"123456\"}], \"department\": \"false\",\"title\": \"张三\",\"name\": \"李四\"},{\"id\": 4568218461, \"userid\": [{\"name\": \"赵武\", \"status\": \"在岗\", \"department\": false,\"jobnum\": \"123456\"}], \"department\": \"false\",\"title\": \"测试部\",\"name\": \"经理:李军\"}]";
        //    //JToken cc = (JToken)JsonConvert.DeserializeObject(aa);
        //    //var aaaaa= (JArray)JsonConvert.DeserializeObject(ee);
        //    //JArray eee = (JArray)JsonConvert.DeserializeObject(ee);
        //    //JObject rr = (JObject)JsonConvert.DeserializeObject(bb);
        //    JObject aaaa = (JObject)JsonConvert.DeserializeObject(newNode);
        //    //var pere = db.Personnel_Organization.Find(796);
        //    var newNodeJToken = aaaa["children"].ToList();
        //    var oldNodeJtoken = ordoragration["children"].ToList();
        //    if (newNodeJToken != oldNodeJtoken)
        //    {
        //        if (!ForeDate_Jobejct(newNodeJToken, oldNodeJtoken, AddMessage))
        //            return Content("false");
        //        else
        //        {
        //            if (add.Count != 0)
        //            {
        //                db.Personnel_Organization.AddRange(add);
        //                db.SaveChanges();
        //            }
        //        }
        //    }
        //    ordoragration = new JObject();
        //    ordoragration = aaaa;
        //    return Content("true");
        //}
        //List<Personnel_Organization> add = new List<Personnel_Organization>();

        ////添加组织架构数据
        //public Personnel_Organization AddPersonnel_Organization(JToken item, Personnel_Organization old, List<string> ee)
        //{
        //    Personnel_Organization newmes = new Personnel_Organization();
        //    string id = item["id"].ToString();
        //    if (id.Contains("userid"))
        //    {
        //        string userid = id.Replace("userid", "");
        //        var restor = db.Personnel_Roster.Where(c => c.JobNum == userid).FirstOrDefault();
        //        if (old.IsBelong == 5)
        //        {
        //            restor.Position = old.Subordinate;
        //            restor.DP_Group = old.Superior;
        //            restor.Department = old.SuperiorDepartment;
        //            db.SaveChanges();
        //        }
        //        return newmes;
        //    }
        //    else
        //    {
        //        var jArrays = ee.ConvertAll(c => (JObject)JsonConvert.DeserializeObject(c));
        //        var info = jArrays.Where(c => c["id"].ToString() == id).FirstOrDefault();
        //        string na = "";
        //        string name = info["name"].ToString();
        //        string position = info["position"].ToString();
        //        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(position))
        //        {
        //            na = info["title"].ToString();
        //        }
        //        else
        //        {
        //            na = info["title"].ToString() + "," + position;

        //        }
        //        string sup = old.Subordinate.Split(',')[0];

        //        var isbelogin = int.Parse(info["belogin"].ToString());
        //        var sub = db.Personnel_Organization.Count(c => c.Superior == sup && c.Subordinate == na);
        //        if (sub > 0)
        //        {
        //            return null;
        //        }
        //        string supdepatment = "";
        //        if (old.IsBelong == 1 || old.IsBelong == 2)
        //        {
        //            supdepatment = sup;
        //        }
        //        else
        //        {
        //            supdepatment = old.SuperiorDepartment;
        //        }
        //        newmes = new Personnel_Organization() { Subordinate = na, SubordinateDepartment = old.Superior, SuperiorDepartment = supdepatment, Superior = sup, Creator = ((Users)Session["User"]).UserName, CreateDate = DateTime.Now, Status = "添加", IsBelong = isbelogin };
        //        return newmes;
        //    }
        //}

        ////新增节点循环
        //public bool AddForDate(List<JToken> aa, Personnel_Organization bb, List<string> ee)
        //{
        //    foreach (var item in aa)
        //    {
        //        string qq = item.ToString();
        //        var tt = qq.Trim(new char[2] { '[', ']' });
        //        JObject aaaa = JObject.Parse(tt);

        //        var rr = (JToken)aaaa;
        //        var newmes = AddPersonnel_Organization(rr, bb, ee);
        //        if (newmes == null)
        //        {
        //            return false;
        //        }
        //        if (newmes.Creator != null)
        //        {
        //            add.Add(newmes);
        //        }

        //        if (aaaa.Property("children") != null)
        //        {
        //            var chil = item[0]["children"].ToList();
        //            AddForDate(chil, newmes, ee);
        //        }
        //    }
        //    return true;
        //}

        ////删除节点记录在组织架构数据中
        //public void DeleteForDate(Personnel_Organization cc)
        //{
        //    var Superior = cc.Subordinate.Split(':')[0];
        //    var sub = db.Personnel_Organization.Where(c => c.Superior == Superior && c.SuperiorDepartment == cc.SubordinateDepartment).ToList();
        //    if (sub.Count() != 0)
        //    {
        //        foreach (var item in sub)
        //        {
        //            item.CreateDate = DateTime.Now;
        //            item.Creator = ((Users)Session["User"]).UserName;
        //            item.Status = "删除";
        //            add.Add(item);
        //            DeleteForDate(item);
        //        }
        //    }
        //}

        ////读取数据，判断节点是新增还是删除
        //public bool ForeDate_Jobejct(List<JToken> aa, List<JToken> bb, List<string> ee, Personnel_Organization old = null)
        //{
        //    if (aa.Count() != bb.Count())
        //    {

        //        List<string> id1 = new List<string>();
        //        List<string> id2 = new List<string>();
        //        List<string> www = aa.Select(c => c["id"].ToString()).ToList();
        //        List<string> eee = bb.Select(c => c["id"].ToString()).ToList();
        //        if (aa.Count > bb.Count())//新增了节点
        //        {
        //            var cc = www.Except(eee).ToList();
        //            foreach (var item in cc)
        //            {
        //                var rrr = aa.Where(c => c["id"].ToString() == item).FirstOrDefault();
        //                var res = AddPersonnel_Organization(rrr, old, ee);
        //                if (res == null)
        //                    return false;
        //                if (res.CreateDate != null)
        //                    add.Add(res);

        //                var childent = aa.Where(c => c["id"].ToString() == item).Select(c => c["children"]).ToList();
        //                if (childent[0] != null)
        //                {
        //                    if (!AddForDate(childent, res, ee))
        //                        return false;
        //                }
        //            }


        //        }
        //        else//删除节点
        //        {
        //            var cc = eee.Except(www).ToList();
        //            foreach (var deleteList in cc)
        //            {
        //                if (!deleteList.ToString().Contains("userid"))
        //                {
        //                    var id = int.Parse(deleteList.ToString());
        //                    var mesage = db.Personnel_Organization.Find(id);
        //                    //DeleteForDate(mesage);
        //                    mesage.CreateDate = DateTime.Now;
        //                    mesage.Creator = ((Users)Session["User"]).UserName;
        //                    mesage.Status = "删除";
        //                    add.Add(mesage);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < aa.Count(); i++)
        //        {
        //            if (aa[i]["children"] != null && bb[i]["children"] != null)
        //            {
        //                int id = int.Parse(aa[i]["id"].ToString());
        //                var personnel = db.Personnel_Organization.Find(id);
        //                if (!ForeDate_Jobejct(aa[i]["children"].ToList(), bb[i]["children"].ToList(), ee, personnel))
        //                    return false;
        //            }
        //            else if (bb[i]["children"] != null)
        //            {
        //                var child = bb[i]["children"].ToList();
        //                for (int j = 0; j < child.Count(); j++)
        //                {
        //                    string id = child[j]["id"].ToString();
        //                    if (!id.Contains("userid"))
        //                    {
        //                        var mesage = db.Personnel_Organization.Find(int.Parse(id));
        //                        //DeleteForDate(mesage);
        //                        mesage.CreateDate = DateTime.Now;
        //                        mesage.Creator = ((Users)Session["User"]).UserName;
        //                        mesage.Status = "删除";
        //                        add.Add(mesage);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return true;
        //}


        ////查询
        //public List<Personnel_Organization> Find_list(string str, string department, string belogGroup, DateTime? datetime = null)
        //{
        //    if (datetime == null)
        //    {
        //        datetime = DateTime.Now;
        //    }
        //    List<Personnel_Organization> result = new List<Personnel_Organization>();
        //    if (belogGroup != null)
        //    {
        //        result = db.Personnel_Organization.Where(c => c.Superior == str && c.SuperiorDepartment == department && c.CreateDate <= datetime && c.SubordinateDepartment == belogGroup).ToList();
        //    }
        //    else
        //    {
        //        result = db.Personnel_Organization.Where(c => c.Superior == str && c.SuperiorDepartment == department && c.CreateDate <= datetime).ToList();
        //    }
        //    result = GetList(result);
        //    return result;
        //}

        ////查找符合条件的list
        //public List<Personnel_Organization> GetList(List<Personnel_Organization> result)
        //{
        //    var deleteList = result.Where(c => c.Status == "删除").Select(c => c.Subordinate).ToList();
        //    if (deleteList.Count() != 0)
        //    {
        //        foreach (var item in deleteList)
        //        {
        //            var delte = result.Where(C => C.Subordinate == item).ToList();
        //            result = result.Except(delte).ToList();
        //        }
        //    }
        //    var updatelist = result.Where(c => c.Status == "修改").Select(C => C.MessageID).ToList();
        //    if (updatelist.Count() != 0)
        //    {
        //        foreach (var item in updatelist)
        //        {
        //            var update = result.Where(c => c.Id == item).FirstOrDefault();
        //            result.Remove(update);
        //        }
        //    }
        //    return result;
        //}

        //public JObject Foreach_username(string jobnum)
        //{
        //    JArray usernameArray = new JArray();
        //    var username = db.Personnel_Roster.Where(c => c.JobNum == jobnum).Select(c => c.Name).FirstOrDefault();
        //    var a = new JObject();
        //    a.Add("name", username);
        //    a.Add("status", GetStatus(jobnum));
        //    a.Add("department", false);
        //    a.Add("jobnum", jobnum);
        //    usernameArray.Add(a);

        //    return a;
        //}

        ////string currentDeprtment = "";
        ////迭代器
        //public JArray Foreach_iterator(List<Personnel_Organization> list_str, DateTime? date)
        //{

        //    var result = new JArray();
        //    if (list_str.Count > 0)
        //    {
        //        var obj_result = new JArray();
        //        var b = new JArray();
        //        var a = new JObject();


        //        foreach (var i in list_str)
        //        {

        //            var title = i.Subordinate;
        //            var name = "";
        //            a.Add("id", i.Id);
        //            a.Add("userid", b);
        //            a.Add("department", "false");
        //            a.Add("canmove", "false");
        //            if (i.Subordinate.Contains(","))
        //            {
        //                var info = i.Subordinate.Split(',');
        //                title = info[0];
        //                var userid = "";
        //                if (title == "品质技术中心")
        //                {
        //                    name = "总监:钟胜雄";
        //                    userid = db.Personnel_Roster.Where(c => c.Name == "钟胜雄").Select(c => c.JobNum).FirstOrDefault();
        //                    //a["jobnum"] = userid;
        //                    a["userid"] = Foreach_username(userid);
        //                }
        //                else
        //                {
        //                    var position = info[1];
        //                    var usermessage = db.Personnel_Roster.Where(c => c.DP_Group == title && c.Position == position && c.Status != "离职员工").FirstOrDefault();
        //                    //userid = FindUserIdstringList(info[0], info[1], i.SubordinateDepartment);
        //                    //var username = db.Personnel_Roster.Where(c => c.JobNum == userid).Select(c => c.Name).FirstOrDefault();
        //                    if (usermessage != null)
        //                    {
        //                        name = info[1] + ":" + usermessage.Name;
        //                        //a["jobnum"] = usermessage.JobNum;
        //                        a["userid"] = Foreach_username(usermessage.JobNum);
        //                    }
        //                }

        //            }

        //            a.Add("title", title);
        //            if (name != "")
        //            {

        //                a.Add("name", name);
        //            }
        //            else
        //            {
        //                a.Add("name", b);
        //            }

        //            a.Add("children", b);
        //            obj_result.Add(a);

        //            if (i.IsBelong == 2)//1是中心，2是部门，3是车间，4是组，5是职位
        //            {
        //                a["department"] = "true";
        //            }
        //            if (i.IsBelong == 1 || i.IsBelong == 2)
        //            {
        //                currentDeprtment = title;
        //            }
        //            //if (department.Contains(title))
        //            //{
        //            //    a["department"] = "true";
        //            //}
        //            //if (department.Contains(title)||title=="制造中心"||title=="品质技术中心")
        //            //{
        //            //    currentDeprtment = title;
        //            //}
        //            if (Find_list(title, currentDeprtment, i.Superior, date).Count > 0)
        //            {

        //                a["children"] = Foreach_iterator(Find_list(title, currentDeprtment, i.Superior, date), date);
        //            }
        //            else
        //            {
        //                var userid = db.Personnel_Roster.Where(c => c.Position == i.Subordinate && c.DP_Group == i.Superior && c.Department == i.SuperiorDepartment && c.Status != "离职员工").ToList();
        //                JArray userJarry = new JArray();
        //                foreach (var item in userid)
        //                {
        //                    JObject aa = new JObject();
        //                    aa.Add("id", "userid" + item.JobNum);
        //                    // aa.Add("jobnum" , item.JobNum);
        //                    aa.Add("department", "false");
        //                    aa.Add("userid", Foreach_username(item.JobNum));
        //                    aa.Add("name", item.Name);
        //                    aa.Add("title", item.Name);
        //                    aa.Add("canmove", true);
        //                    aa.Add("children", b);
        //                    userJarry.Add(aa);

        //                }
        //                a["children"] = userJarry;
        //                //botton = true;

        //            }
        //            a = new JObject();
        //        }
        //        return obj_result;
        //    }
        //    else return result;
        //}


        ////获取只有组织架构ID的json
        //public JArray GetID(JToken jObject)
        //{
        //    var cc = jObject["children"].ToList();
        //    if (cc.Count != 0 && cc[0] != new JArray())
        //    {
        //        JObject a = new JObject();
        //        JArray b = new JArray();
        //        JArray result = new JArray();
        //        foreach (var item in jObject["children"].ToList())
        //        {
        //            a.Add("id", item["id"]);
        //            a.Add("children", b);
        //            if (GetID(item) != null)
        //                a["children"] = GetID(item);
        //            //var bb= GetID(item);
        //            //if (bb != null)
        //            //{
        //            //    a.Add("children", bb);
        //            //}
        //            result.Add(a);
        //            a = new JObject();
        //        }
        //        return result;
        //    }
        //    return null;
        //}
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 测试代码 
        //    //foreach (var item in aa)
        //    //{
        //    //    foreach (var item2 in bb)
        //    //    {
        //    //        var chident = item["children"].ToList();
        //    //        var chident2 = item2["children"].ToList();
        //    //        if (chident != chident2)
        //    //        {

        //    //            //foreach (var list in chident)
        //    //            //{
        //    //            //    //diedaiqi(list);
        //    //            //    Personnel_Organization org2 = new Personnel_Organization();
        //    //            //    org2.Subordinate = item["title"].ToString();
        //    //            //    org2.Superior = aa.FirstOrDefault()["title"].ToString();
        //    //            //    var subdepartment = db.Personnel_Roster.Where(c => c.DP_Group == org2.Superior).Select(c => c.Department).FirstOrDefault();
        //    //            //    org2.SubordinateDepartment = org2.SuperiorDepartment = subdepartment;
        //    //            //    organizationsList.Add(org2);
        //    //            //}

        //    //        }
        //    //        else
        //    //        {
        //    //            Personnel_Organization org = new Personnel_Organization();
        //    //            string name = item["name"].ToString();
        //    //            string[] position = name.Split(':');
        //    //            if (position.Count() > 1)
        //    //            {
        //    //                org.Subordinate = item["title"].ToString() + "," + position[0];
        //    //            }
        //    //            else
        //    //                org.Subordinate = item["title"].ToString();
        //    //            org.Superior = aa.FirstOrDefault()["title"].ToString();
        //    //            var subdepartment = db.Personnel_Roster.Where(c => c.DP_Group == org.Superior).Select(c => c.Department).FirstOrDefault();
        //    //            org.SubordinateDepartment = org.SuperiorDepartment = subdepartment;
        //    //            organizationsList.Add(org);
        //    //        }
        //    //    }
        //    //}
        //}

        //只加载到部门
        //public JArray Foreach_iterator(List<Personnel_Organization> list_str)
        //{
        //    var result = new JArray();
        //    if (list_str.Count > 0)
        //    {
        //        var obj_result = new JArray();
        //        var b = new JArray();
        //        var a = new JObject();
        //        foreach (var i in list_str)
        //        {
        //            var name = i.Subordinate;
        //            var title = "";
        //            a.Add("userid", b);
        //            a.Add("department", "false");
        //            if (i.Subordinate.Contains(","))
        //            {
        //                var info = i.Subordinate.Split(',');
        //                name = info[0];
        //                var userid = "";
        //                if (name == "品质技术中心")
        //                {
        //                    title = "总监:钟胜雄";
        //                    userid = db.Personnel_Roster.Where(c => c.Name == "钟胜雄").Select(c => c.JobNum).FirstOrDefault();
        //                }
        //                else
        //                {
        //                    userid = FindUserIdstringList(info[0], info[1], i.SubordinateDepartment);
        //                    var username = db.Personnel_Roster.Where(c => c.JobNum == userid).Select(c => c.Name).FirstOrDefault();
        //                    title = info[1] + ":" + username;
        //                }
        //                string[] user = userid.Split(',');
        //                a["userid"] = Foreach_username(user);
        //            }

        //            a.Add("name", name);
        //            if (title != "")
        //            {

        //                a.Add("title", title);
        //            }
        //            else
        //            {
        //                a.Add("title", b);
        //            }

        //            a.Add("children", b);
        //            obj_result.Add(a);
        //            if (department.Contains(name))
        //            {
        //                a["department"] = "true";
        //            }
        //            if (Find_list(name, i.SubordinateDepartment).Count > 0 && ! department.Contains(name))
        //            {
        //                a["children"] = Foreach_iterator(Find_list(name, i.SubordinateDepartment));

        //            }
        //            else
        //            {
        //                string usernametostring = FindUserIdstringList(i.Superior, name, i.SubordinateDepartment);
        //                if (usernametostring != "")
        //                {
        //                    string[] userList = usernametostring.Split(',');
        //                    //a["children"] = Foreach_username(userList);
        //                    a["userid"] = Foreach_username(userList);
        //                    a["title"] = userList.Count();
        //                }
        //            }
        //            a = new JObject();
        //        }
        //        return obj_result;
        //    }
        //    else return result;
        //}
        #endregion
    }
}
