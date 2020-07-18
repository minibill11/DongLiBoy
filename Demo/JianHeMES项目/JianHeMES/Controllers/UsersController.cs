﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static JianHeMES.Controllers.CommonalityController;
using Newtonsoft.Json.Converters;

namespace JianHeMES.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        CommonController com = new CommonController();

        #region 权限管理
        //主页面
        public ActionResult UserManage()
        {
            ViewBag.Department = GetDepartmentList();
            ViewBag.Role = GetRoleList();
            return View();
        }

        [HttpPost]
        public ActionResult UserManage(string json)
        {
            ViewBag.Department = GetDepartmentList();
            ViewBag.Role = GetRoleList();
            return View();
        }

        //显示权限管理的部门
        public async Task<ActionResult> displayDepartmentByRole()
        {
            if (((Users)Session["User"]).UserName == null)
            {
                return Content("请先登录");
            }
            //登录用户的职位
            string name = ((Users)Session["User"]).UserName;
            int id = ((Users)Session["User"]).UserNum;
            var userRoles = db.Useroles.Where(c => c.UserID == id && c.UserName == name).Select(c => c.Position).FirstOrDefault();
            //登录用户的部门
            string userDeparment = ((Users)Session["User"]).Department;

            JObject deparemtntJobject = new JObject();
            #region 之前的
            //var roleList = db.UserManageRoles.Where(c => c.AdministratorPosition == userRoles.ToString() && c.AdministratorDepartment == userDeparment).Select(c => c.ToBeManageDepartment).Distinct().ToList();
            //if (roleList.Count == 0)
            //{
            //    return Content("对不起，你没有此权限");
            //}
            #endregion
            var roleList = db.UserRolelistTable.Select(c => c.Department).Distinct().ToList();
            //roleList.Remove(userDeparment);
            for (int i = 0; i < roleList.Count(); i++)
            {
                deparemtntJobject.Add(i.ToString(), roleList[i]);
            }

            return Content(JsonConvert.SerializeObject(deparemtntJobject));

        }
        //显示权限管理的权限组
        public async Task<ActionResult> displayRoleGroupList(string department)
        {
            JObject groupNameJobject = new JObject();
            var groupname = db.UserRolelistTable.Where(c => c.Department == department).Select(c => c.RolesName).Distinct().ToList();
            if (groupname.Count() == 0)
            {
                return Content("该部门没有权限");
            }
            for (int i = 0; i < groupname.Count(); i++)
            {
                groupNameJobject.Add(i.ToString(), groupname[i]);
            }

            return Content(JsonConvert.SerializeObject(groupNameJobject));
        }

        //显示用户清单
        public async Task<ActionResult> displayUserList(string department, string groupRoleName)
        {

            JObject userMesage = new JObject();
            JObject canChageuser = new JObject();
            JObject haveroleJobjec = new JObject();
            JObject canchangeroleJobjec = new JObject();
            JObject justReadUser = new JObject();
            JObject user = new JObject();
            //登录用户的职位
            string name = ((Users)Session["User"]).UserName;
            int id = ((Users)Session["User"]).UserNum;
            var userRoles = db.Useroles.Where(c => c.UserID == id && c.UserName == name).Select(c => c.Position).FirstOrDefault();
            //登录用户的部门
            string userDeparment = ((Users)Session["User"]).Department;

            var roles = db.UserRolelistTable.Where(c => c.Department == department && c.RolesName == groupRoleName).Select(c => new { c.RolesCode, c.Discription }).ToList();
            if (userRoles == "系统管理员")
            {
                //可以修改权限的职位清单
                var canChagePositionList = db.Users.Where(c => c.Department == department).ToList();
                //具体权限清单

                int x = 0;
                //可修改人员的具体信息
                for (int j = 0; j < canChagePositionList.Count(); j++)
                {
                    var userInfo = canChagePositionList[j];
                    userMesage.Add("department", userInfo.Department);
                    userMesage.Add("userNum", userInfo.UserNum);
                    userMesage.Add("userName", userInfo.UserName);
                    userMesage.Add("position", userInfo.Role);
                    userMesage.Add("Time", userInfo.CreateDate);

                    //显示权限更多操作按钮和授权按钮
                    userMesage.Add("status", 0);

                    //判断当前用户是否被授权
                    var isCheckAuthorization = db.UserManageRoles.Where(c => c.AdministratorDepartment == department && c.AdministratorPosition == userInfo.Role && c.Distribution == 1 && c.Operator == name).ToList();
                    if (isCheckAuthorization.Count() == 0)
                        userMesage.Add("isCheckAuthorization", "未授权");
                    else
                        userMesage.Add("isCheckAuthorization", "已授权");


                    //获取现有的具体权限
                    var haveRole = db.Useroles.Where(c => c.UserName == userInfo.UserName && c.UserID == userInfo.UserNum && c.RolesName == groupRoleName).Select(c => c.Roles).FirstOrDefault();
                    List<string> haveRoleList = new List<string>();
                    if (!string.IsNullOrEmpty(haveRole))
                    {
                        haveRoleList = new List<string>(haveRole.Split(','));
                    }
                    for (int i = 0; i < roles.Count; i++)
                    {

                        if (haveRoleList.Contains(roles[i].RolesCode.ToString()))
                            haveroleJobjec.Add(i.ToString(), roles[i].Discription);
                        else
                            canchangeroleJobjec.Add(i.ToString(), roles[i].Discription);


                    }
                    //添加拥有的具体权限
                    if (haveroleJobjec.Count == 0)
                        userMesage.Add("haveRoleList", null);
                    else
                        userMesage.Add("haveRoleList", haveroleJobjec);

                    //添加未拥有的具体权限
                    if (canchangeroleJobjec.Count == 0)
                        userMesage.Add("canAddRoleList", null);
                    else
                        userMesage.Add("canAddRoleList", canchangeroleJobjec);


                    canChageuser.Add(x.ToString(), userMesage);
                    x++;
                    userMesage = new JObject();
                    haveroleJobjec = new JObject();
                    canchangeroleJobjec = new JObject();
                }
                user.Add("canChageUser", canChageuser);
                user.Add("justReadUser", null);
            }
            else if (userDeparment == "总经办" && userRoles == "工厂厂长")
            {
                var justReadUserInfo = db.Users.Where(c => c.Department == department).ToList();
                for (int j = 0; j < justReadUserInfo.Count(); j++)
                {
                    userMesage.Add("department", justReadUserInfo[j].Department);
                    userMesage.Add("userNum", justReadUserInfo[j].UserNum);
                    userMesage.Add("userName", justReadUserInfo[j].UserName);
                    userMesage.Add("position", justReadUserInfo[j].Role);
                    userMesage.Add("Time", justReadUserInfo[j].CreateDate);

                    string uname = justReadUserInfo[j].UserName;
                    int usernum = justReadUserInfo[j].UserNum;
                    var haveRole = db.Useroles.Where(c => c.UserName == uname && c.UserID == usernum && c.RolesName == groupRoleName).Select(c => c.Roles).FirstOrDefault();
                    if (string.IsNullOrEmpty(haveRole))
                    {
                        userMesage.Add("haveRoleList", null);
                        userMesage.Add("canAddRoleList", null);
                    }
                    else
                    {
                        List<string> haveRoleList = new List<string>(haveRole.Split(','));

                        for (int i = 0; i < roles.Count; i++)
                        {
                            if (haveRoleList.Contains(roles[i].RolesCode.ToString()))
                                haveroleJobjec.Add(i.ToString(), roles[i].Discription);
                        }
                        userMesage.Add("haveRoleList", haveroleJobjec);
                        userMesage.Add("canAddRoleList", null);
                    }
                    justReadUser.Add(j.ToString(), userMesage);
                    userMesage = new JObject();
                    haveroleJobjec = new JObject();

                }
                user.Add("canChageUser", null);
                user.Add("justReadUser", justReadUser);
            }
            else
            {
                user.Add("canChageUser", null);
                user.Add("justReadUser", null);
            }
            #region  之前的 
            ////可以修改权限的职位清单
            //var canChagePositionList = db.UserManageRoles.Where(c => c.AdministratorDepartment == userDeparment && c.AdministratorPosition == userRoles && c.ToBeManageDepartment == department).Select(c => c.ToBeManagePosition).ToList();

            //if (canChagePositionList.Count() == 0)
            //{
            //    return Content("没有记录！");
            //}
            ////具体权限清单
            //var roles = db.UserRolelistTable.Where(c => c.Department == department && c.RolesName == groupRoleName).Select(c => new { c.RolesCode, c.Discription }).ToList();
            ////部门的人员清单
            //var justReadUserInfo = db.Users.Where(c => c.Department == department).ToList();
            //int x = 0;
            ////可修改人员的具体信息
            //for (int j = 0; j < canChagePositionList.Count(); j++)
            //{
            //    string userrole = canChagePositionList[j];
            //    //获取可修改人员信息
            //    var userinfoList = db.Users.Where(c => c.Department == department && c.Role == userrole).ToList();
            //    foreach (var userInfo in userinfoList)
            //    {
            //        userMesage.Add("department", userInfo.Department);
            //        userMesage.Add("userNum", userInfo.UserNum);
            //        userMesage.Add("userName", userInfo.UserName);
            //        userMesage.Add("position", userInfo.Role);
            //        userMesage.Add("Time", userInfo.CreateDate);

            //        //去掉可以修改权限的人员
            //        justReadUserInfo.Remove(userInfo);
            //        string currentrole = userInfo.Role;
            //        var isCheckStatus = db.UserManageRoles.Where(c => c.AdministratorDepartment == userDeparment && c.AdministratorPosition == userRoles && c.ToBeManageDepartment == department && c.ToBeManagePosition == userInfo.Role).FirstOrDefault().Distribution;

            //        if (isCheckStatus == 0)//显示权限更多操作按钮和授权按钮
            //            userMesage.Add("status", 0);
            //        else
            //            userMesage.Add("status", 1);//不显示权限更多操作按钮和授权按钮
            //        //判断当前用户是否被授权
            //        var isCheckAuthorization = db.UserManageRoles.Where(c => c.AdministratorDepartment == department && c.AdministratorPosition == userInfo.Role && c.Distribution == 1 && c.Operator == name).ToList();
            //        if (isCheckAuthorization.Count() == 0)
            //            userMesage.Add("isCheckAuthorization", "未授权");
            //        else
            //            userMesage.Add("isCheckAuthorization", "已授权");


            //        //获取现有的具体权限
            //        var haveRole = db.Useroles.Where(c => c.UserName == userInfo.UserName && c.UserID == userInfo.UserNum && c.RolesName == groupRoleName).Select(c => c.Roles).FirstOrDefault();
            //        List<string> haveRoleList = new List<string>();
            //        if (!string.IsNullOrEmpty(haveRole))
            //        {
            //            haveRoleList = new List<string>(haveRole.Split(','));
            //        }
            //        for (int i = 0; i < roles.Count; i++)
            //        {

            //            if (haveRoleList.Contains(roles[i].RolesCode.ToString()))
            //                haveroleJobjec.Add(i.ToString(), roles[i].Discription);
            //            else
            //                canchangeroleJobjec.Add(i.ToString(), roles[i].Discription);


            //        }
            //        //添加拥有的具体权限
            //        if (haveroleJobjec.Count == 0)
            //            userMesage.Add("haveRoleList", null);
            //        else
            //            userMesage.Add("haveRoleList", haveroleJobjec);

            //        //添加未拥有的具体权限
            //        if (canchangeroleJobjec.Count == 0)
            //            userMesage.Add("canAddRoleList", null);
            //        else
            //            userMesage.Add("canAddRoleList", canchangeroleJobjec);


            //        canChageuser.Add(x.ToString(), userMesage);
            //        x++;
            //        userMesage = new JObject();
            //        haveroleJobjec = new JObject();
            //        canchangeroleJobjec = new JObject();
            //    }
            //}
            ////只可查看人员的具体信息
            //for (int j = 0; j < justReadUserInfo.Count(); j++)
            //{
            //    userMesage.Add("department", justReadUserInfo[j].Department);
            //    userMesage.Add("userNum", justReadUserInfo[j].UserNum);
            //    userMesage.Add("userName", justReadUserInfo[j].UserName);
            //    userMesage.Add("position", justReadUserInfo[j].Role);
            //    userMesage.Add("Time", justReadUserInfo[j].CreateDate);

            //    string uname = justReadUserInfo[j].UserName;
            //    int usernum = justReadUserInfo[j].UserNum;
            //    var haveRole = db.Useroles.Where(c => c.UserName == uname && c.UserID == usernum&&c.RolesName== groupRoleName).Select(c => c.Roles).FirstOrDefault();
            //    if (string.IsNullOrEmpty(haveRole))
            //    {
            //        userMesage.Add("haveRoleList", null);
            //        userMesage.Add("canAddRoleList", null);
            //    }
            //    else
            //    {
            //        List<string> haveRoleList = new List<string>(haveRole.Split(','));

            //        for (int i = 0; i < roles.Count; i++)
            //        {
            //            if (haveRoleList.Contains(roles[i].RolesCode.ToString()))
            //                haveroleJobjec.Add(i.ToString(), roles[i].Discription);
            //        }
            //        userMesage.Add("haveRoleList", haveroleJobjec);
            //        userMesage.Add("canAddRoleList", null);
            //    }
            //    justReadUser.Add(j.ToString(), userMesage);
            //    userMesage = new JObject();
            //    haveroleJobjec = new JObject();

            //}
            #endregion
            //var cc = Content(JsonConvert.SerializeObject(user));
            return Content(JsonConvert.SerializeObject(user));
        }

        //可管控界面列表
        public async Task<ActionResult> displayRoleTree(string department, string groupRoleName, string user)
        {

            JObject AsignJobject = new JObject();
            JObject currentAsignList = new JObject();
            JObject userBranchJobject = new JObject();
            List<string> chacked = new List<string>();

            string name = ((Users)Session["User"]).UserName;
            int id = ((Users)Session["User"]).UserNum;
            var userRoles = db.Useroles.Where(c => c.UserID == id && c.UserName == name).Select(c => c.Position).FirstOrDefault();
            //登录用户的部门
            string userDeparment = ((Users)Session["User"]).Department;

            //分支列表
            var userBranchResult = db.UserCanManagePosition.Where(c => c.Department == userDeparment && c.Position == userRoles).Select(c => c.CanManagePositionCodeList).FirstOrDefault();

            //拥有的分支列表
            var currentAsignResult = db.UserManageRoles.Where(c => c.AdministratorDepartment == department && c.AdministratorPosition == user && c.Distribution == 0).Select(c => new { c.ToBeManageDepartment, c.ToBeManagePosition }).ToList();

            if (currentAsignResult.Count() == 0)
            {
                AsignJobject.Add("currentAsign", null);
            }
            else
            {
                for (int i = 0; i < currentAsignResult.Count(); i++)
                {
                    chacked.Add(currentAsignResult[i].ToBeManageDepartment + currentAsignResult[i].ToBeManagePosition);
                    currentAsignList.Add(i.ToString(), currentAsignResult[i].ToBeManageDepartment + ":" + currentAsignResult[i].ToBeManagePosition);

                }
                AsignJobject.Add("currentAsign", currentAsignList);
            }
            //未拥有的分支列表

            if (!string.IsNullOrEmpty(userBranchResult))
            {
                List<string> userBranchList = new List<string>(userBranchResult.Split(','));
                for (int i = 0; i < userBranchList.Count(); i++)
                {
                    string pos = userBranchList[i];
                    var position = db.UserPositionList.Where(c => c.PositionCode == pos).Select(c => new { c.Department, c.Position }).FirstOrDefault();
                    if (!chacked.Contains(position.Department + position.Position) && department.Trim() + user.Trim() != position.Department + position.Position)
                        userBranchJobject.Add(i.ToString(), position.Department + ":" + position.Position);
                }
            }
            if (userBranchJobject.Count == 0)
            {
                AsignJobject.Add("canAddAsign", null);
            }
            else
            {
                AsignJobject.Add("canAddAsign", userBranchJobject);
            }
            return Content(JsonConvert.SerializeObject(AsignJobject));
        }

        //修改用户列表(方法有引用)
        public bool ChangeUserRole(string json)
        {
            //string jj = "{\"username\":\"张三\",\"userid\":\"123\",\"department\":\"品质部\",\"position\":\"组长\",\"rolename\":\"老化管理\",\"role\":\"单次老化,批量老化,批量老化完成\"}";
            if (string.IsNullOrEmpty(json))
            { return false; }
            JObject userJsonList = (JObject)JsonConvert.DeserializeObject(json);

            string user = userJsonList["username"].ToString();
            int userid = int.Parse(userJsonList["userid"].ToString());
            string department = userJsonList["department"].ToString();
            string position = userJsonList["position"].ToString();
            string add = userJsonList["add"].ToString();
            string delete = userJsonList["Delete"].ToString();
            string groupName = userJsonList["rolename"].ToString();

            List<string> roleList = new List<string>();
            //当前用户的rolecode
            var totalrolecode = db.Useroles.Where(c => c.UserName == user && c.UserID == userid && c.RolesName == groupName).Select(c => c.Roles).FirstOrDefault();

            if (!string.IsNullOrEmpty(totalrolecode))
            {
                roleList = new List<string>(totalrolecode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            }

            //新增的权限,打钩的
            JArray adduserList = (JArray)JsonConvert.DeserializeObject(add);
            foreach (var item in adduserList)
            {
                string discription = item.ToString();
                int code = db.UserRolelistTable.Where(c => c.RolesName == groupName && c.Discription == discription).Select(c => c.RolesCode).FirstOrDefault();
                roleList.Add(code.ToString());
            }


            //去掉的权限,去钩的
            JArray deleteuserList = (JArray)JsonConvert.DeserializeObject(delete);
            foreach (var item in deleteuserList)
            {
                string discription = item.ToString();
                int code = db.UserRolelistTable.Where(c => c.RolesName == groupName && c.Discription == discription).Select(c => c.RolesCode).FirstOrDefault();
                roleList.Remove(code.ToString());
            }

            //将List转换字符
            string rolecode = string.Join(",", roleList.ToArray());
            if (roleList.Count() == 0)
            {
                Deleterolename(userid, groupName);
                return true;
            }
            var checkUser = db.Useroles.Where(c => c.UserName == user && c.RolesName == groupName && c.UserID == userid);
            //如果没有这个用户信息，添加
            if (checkUser.Count() == 0)
            {
                Useroles userrole = new Useroles { UserName = user, UserID = userid, Department = department, Position = position, Roles = rolecode, RolesName = groupName };
                db.Useroles.Add(userrole);
                db.SaveChanges();
            }
            //如果有这个用户信息，修改roles
            else
            {
                checkUser.First().Roles = rolecode;
                db.SaveChangesAsync();
            }


            return true;
        }

        //修改可管控列表
        public void ChangeManageRole(string json)
        {
            //string jsosn = "{\"posittion\":\"111\",\"depa\":\"2222\",\"add\":{\"品质部\":{\"0\":\"经理,组长\"},\"装配1部\":{\"0\":\"经理,组长\"}},\"delete\":{\"品质部\":{\"0\":\"经理\",\"1\":\"组长\"},\"装配1部\":{\"0\":\"经理\",\"1\":\"组长\"}}}";
            if (!string.IsNullOrEmpty(json))
            {
                JObject userJsonList = (JObject)JsonConvert.DeserializeObject(json);
                string user = userJsonList["posittion"].ToString();
                string add = userJsonList["add"].ToString();//包括了部门和职位
                string delete = userJsonList["delete"].ToString();
                string department = userJsonList["depa"].ToString();

                string userName = ((Users)Session["User"]).UserName;

                //往UserManageRoles添加数据，加√的
                JArray deleteuserList = (JArray)JsonConvert.DeserializeObject(delete);
                foreach (var item in deleteuserList)
                {
                    string str = item.ToString();
                    var info = str.Split(':');
                    string dep = info[0];
                    string position = info[1];
                    UserManageRoles additem = new UserManageRoles { AdministratorDepartment = department, AdministratorPosition = user, ToBeManageDepartment = dep, ToBeManagePosition = position, Operator = userName, OperateDT = DateTime.Now, Distribution = 0 };
                    db.UserManageRoles.Add(additem);
                    db.SaveChangesAsync();
                }
                //往UserManageRoles删除数据，去√的
                JArray adduserList = (JArray)JsonConvert.DeserializeObject(add);
                foreach (var item in adduserList)
                {
                    string str = item.ToString();
                    var info = str.Split(':');
                    string dep = info[0];
                    string position = info[1];
                    var delteResult = db.UserManageRoles.Where(c => c.AdministratorDepartment == department && c.AdministratorPosition == user && c.ToBeManageDepartment == dep && c.ToBeManagePosition == position && c.Distribution == 0).FirstOrDefault();
                    db.UserManageRoles.Remove(delteResult);
                    db.SaveChangesAsync();
                }
            }
            //if (delete != null)
            //{
            //    JObject deleteInfo = (JObject)JsonConvert.DeserializeObject(delete);
            //    //删除去掉的职位
            //    foreach (var item in deleteInfo)
            //    {
            //        var value = (JObject)JsonConvert.DeserializeObject(item.Value.ToString());
            //        foreach (var positon in value)
            //        {
            //            var delteResult = db.UserManageRoles.Where(c => c.AdministratorDepartment == department && c.AdministratorPosition == user && c.ToBeManageDepartment == item.Key && c.ToBeManagePosition == positon.Value.ToString() && c.Distribution == 0).FirstOrDefault();
            //            db.UserManageRoles.Remove(delteResult);
            //            db.SaveChangesAsync();
            //        }

            //    }
            //}
            //if (add != null)
            //{
            //    JObject addInfo = (JObject)JsonConvert.DeserializeObject(add);
            //    string userName = ((Users)Session["User"]).UserName;
            //    //添加增加的职位
            //    foreach (var item in addInfo)
            //    {
            //        var value = (JObject)JsonConvert.DeserializeObject(item.Value.ToString());
            //        foreach (var positon in value)
            //        {
            //            UserManageRoles additem = new UserManageRoles { AdministratorDepartment = department, AdministratorPosition = user, ToBeManageDepartment = item.Key, ToBeManagePosition = positon.Value.ToString(), Operator = userName, OperateDT = DateTime.Now, Distribution = 0 };
            //            db.UserManageRoles.Add(additem);
            //            db.SaveChangesAsync();
            //        }

            //    }
            //}
        }

        //授权
        public string Authorization(string userposition, string department)
        {

            //登录职位
            string name = ((Users)Session["User"]).UserName;
            int id = ((Users)Session["User"]).UserNum;
            var userRoles = db.Useroles.Where(c => c.UserID == id && c.UserName == name).Select(c => c.Position).FirstOrDefault();
            //登录用户的部门
            string userDeparment = ((Users)Session["User"]).Department;
            var authorizationCount = db.UserManageRoles.Where(c => c.Operator == userRoles && c.Distribution == 1).Select(c => new { c.AdministratorDepartment, c.AdministratorPosition }).Distinct().ToList();
            if (authorizationCount.Count() < 3)
            {
                //授权人拥有的下属列表
                var list = db.UserManageRoles.Where(c => c.AdministratorDepartment == userDeparment && c.AdministratorPosition == userRoles && c.Distribution == 0).Select(c => new { c.ToBeManageDepartment, c.ToBeManagePosition }).ToList();
                //被授权人拥有的下属列表
                var currentList = db.UserManageRoles.Where(c => c.AdministratorDepartment == department && c.AdministratorPosition == userposition && c.Distribution == 0).Select(c => new { c.ToBeManageDepartment, c.ToBeManagePosition }).ToList();

                var exceptList = list.Except(currentList).ToList();
                foreach (var item in exceptList)
                {
                    if (userposition == item.ToBeManagePosition && department == item.ToBeManageDepartment)
                        continue;
                    UserManageRoles additem = new UserManageRoles { AdministratorDepartment = department, AdministratorPosition = userposition, ToBeManageDepartment = item.ToBeManageDepartment, ToBeManagePosition = item.ToBeManagePosition, Operator = name, OperateDT = DateTime.Now, Distribution = 1 };
                    db.UserManageRoles.Add(additem);
                    db.SaveChangesAsync();
                }
                return "授权成功";

            }
            else
                return "最多只能授权三个职位，请先取消已有的授权";//最多只能授权三个
        }
        //取消授权
        public void DelteAuthorization(string userposition, string department)
        {
            string username = ((Users)Session["User"]).UserName;
            var list = db.UserManageRoles.Where(c => c.AdministratorDepartment == department && c.AdministratorPosition == userposition && c.Operator == username && c.Distribution == 1).ToList();
            if (list != null)
            {
                foreach (var item in list)
                {
                    db.UserManageRoles.Remove(item);
                    db.SaveChanges();
                }
            }
        }

        //显示所有用户名的清单列表
        public ActionResult Query_Permissions()
        {
            JObject Table = new JObject();
            JArray UserList = new JArray();
            var namelist = db.Users.ToList();
            if (namelist.Count > 0)
            {
                foreach (var item in namelist)
                {
                    //用户名
                    Table.Add("UserName", item.UserName);
                    //工号
                    Table.Add("UserNum", item.UserNum);
                    UserList.Add(Table);
                    Table = new JObject();
                }
            }
            return Content(JsonConvert.SerializeObject(UserList));
        }

        //显示所有部门的清单列表
        public ActionResult QueryDepartment()
        {
            var depar_list = db.Users.OrderByDescending(m => m.ID).Select(c => c.Department).Distinct();
            return Content(JsonConvert.SerializeObject(depar_list));           
        }

        //根据部门或者用户名查询用户所有的权限permissions
        [HttpPost]
        public ActionResult PermissionList(string department, List<int> userID)
        {
            JArray table = new JArray();
            JObject roleNameList = new JObject();
            JArray roleList = new JArray();
            JArray discList = new JArray();
            JObject ainten = new JObject();
            var dataList = db.Useroles.ToList();
            if (!String.IsNullOrEmpty(department))
            {
                dataList = dataList.Where(c => c.Department == department).Distinct().ToList();
            }
            else if (userID.Count != 0)
            {
                List<Useroles> a = new List<Useroles>();
                foreach (var it in userID)
                {
                    var userlist = db.Useroles.Where(c => c.UserID == it).ToList();
                    a = a.Concat(userlist).ToList();
                }
                dataList = a;
            }
            if (dataList.Count > 0)
            {
                int k = 0;
                var userrolelisttable = db.UserRolelistTable.ToList();
                var idList = dataList.Select(c => c.UserID).Distinct().ToList();
                foreach (var itg in idList)
                {
                    var rolename = dataList.Where(c => c.UserID == itg).Select(c => new { c.Department, c.UserID, c.UserName, c.RolesName, c.Roles }).Distinct().ToList();
                    foreach (var ite in rolename)
                    {
                        string[] strCharArr = ite.Roles.Split(',');
                        var recordlist = userrolelisttable.Where(c => c.RolesName == ite.RolesName).Distinct().ToList();
                        for (int i = 0; strCharArr.Count() > i; i++)
                        {
                            int code = int.Parse(strCharArr[i]);
                            var Perm = recordlist.Where(c => c.RolesCode == code).FirstOrDefault().Discription;
                            roleList.Add(Perm);
                            k++;
                        }
                        //权限组
                        ainten.Add("RolesName", ite.RolesName);
                        //权限名
                        ainten.Add("Discription", roleList);
                        roleList = new JArray();
                        discList.Add(ainten);
                        ainten = new JObject();
                    }
                    var depar = dataList.Where(c => c.UserID == itg).Select(c => c.Department).FirstOrDefault();
                    //部门
                    roleNameList.Add("Department", depar);
                    //工号
                    roleNameList.Add("UserID", itg);
                    var uname = dataList.Where(c => c.UserID == itg).Select(c => c.UserName).FirstOrDefault();
                    //姓名
                    roleNameList.Add("UserName", uname);
                    //权限清单
                    roleNameList.Add("discList", discList);
                    discList = new JArray();
                    table.Add(roleNameList);
                    roleNameList = new JObject();
                }
            }
            return Content(JsonConvert.SerializeObject(table));
        }

        #endregion

        #region 暂时没有页面
        //根据权限组，显示所有权限
        public ActionResult DisplayRoleFormGroup(string department, string group)
        {
            JObject display = new JObject();
            var totalRole = db.UserRolelistTable.Where(c => c.Department == "智造部" && c.RolesName == group).Select(c => c.Discription).Distinct().ToList();//智造部的某个权限组的所有权限
            var haveRole = db.UserRolelistTable.Where(c => c.Department == department && c.RolesName == group).Select(c => c.Discription).Distinct().ToList();//前面传进来的部门和权限组的所有权限
            var canAddRole = totalRole.Except(haveRole).ToList();
            display.Add("haveRole", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(haveRole))); //前面传进来的部门和权限组的所有权限清单
            display.Add("canAddRole", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(canAddRole)));//智造部有的权限，是传值过来的部门，没有的权限清单
            return Content(JsonConvert.SerializeObject(display));
        }

        //删除已有的权限
        public void DeleteRole(string group, string Discription)
        {
            var rolelist = db.UserRolelistTable.Where(c => c.RolesName == group && c.Discription == Discription);
            int code = rolelist.FirstOrDefault().RolesCode;
            var userrolelist = db.Useroles.Where(c => c.RolesName == group).ToList();
            foreach (var item in userrolelist)
            {
                var list = item.Roles;
                var codeArray = new List<string>(list.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                if (codeArray.Contains(code.ToString()))
                {
                    codeArray.Remove(code.ToString());
                }
                if (codeArray.Count != 0)
                {
                    item.Roles = string.Join(",", codeArray.ToArray());
                }
                else
                    item.Roles = "";
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            db.UserRolelistTable.RemoveRange(rolelist);
            db.SaveChanges();
        }

        //修改权限名 单个的  update [UserRolelistTables] set Discription='要改的' where Discription='原来的'

        //往部门添加已有的权限组
        public void AddGroupToDeaprment(string department, string rolegroup, string codestring)
        {
            int code = db.UserRolelistTable.Where(c => c.Department == "智造部" && c.RolesName == rolegroup && c.Discription == codestring).Select(c => c.RolesCode).FirstOrDefault();
            UserRolelistTable aa = new UserRolelistTable() { Department = department, RolesName = rolegroup, RolesCode = code, Discription = codestring, OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName };
            db.UserRolelistTable.Add(aa);
            db.SaveChanges();
        }
        //添加新权限
        public void AddnewRole(List<string> department, string rolegroup, string codestring)
        {
            var codes = db.UserRolelistTable.Where(c => c.Department == "智造部" && c.RolesName == rolegroup).Select(c => c.RolesCode).ToList();
            int code = 1;
            if (codes.Count != 0)
            {
                code = codes.Max() + 1;
            }
            var isExit = db.Useroles.Where(c => c.RolesName == rolegroup && c.Department == "智造部").ToList();
            if (isExit.Count() > 0)
            {
                foreach (var item in isExit)
                {
                    item.Roles = item.Roles + "," + code;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                var administrator = db.Users.Where(c => c.Role == "系统管理员").ToList();
                foreach (var item in administrator)
                {
                    Useroles role = new Useroles() { Department = "智造部", RolesName = rolegroup, Position = "系统管理员", Roles = "1", UserID = item.UserNum, UserName = item.UserName };
                    db.Useroles.Add(role);
                    db.SaveChanges();
                }
            }
            if (!department.Contains("智造部"))
            {
                department.Add("智造部");
            }
            foreach (var item in department)
            {
                UserRolelistTable aa = new UserRolelistTable() { Department = item, RolesName = rolegroup, RolesCode = code, Discription = codestring, OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName };
                db.UserRolelistTable.Add(aa);
                db.SaveChanges();
            }
        }

        //添加新的系统管理员权限
        public void AddSystemRole(string userid, string name)
        {
            var current = db.Useroles.Where(c => c.UserID == 16621).ToList();
            foreach (var item in current)
            {
                item.UserID = int.Parse(userid);
                item.UserName = name;
                db.Useroles.Add(item);
                db.SaveChanges();
            }
        }
        #endregion

        #region--添加新页面（修改权限）

        //提供列表，从UserRolelistTable里找出部门，权限组，权限名的列表
        #region --------------------DepartmentList()检索部门
        public List<SelectListItem> DepartmentList()
        {
            var depar = db.UserRolelistTable.OrderByDescending(m => m.id).Select(c => c.Department).Distinct();

            var departmentitems = new List<SelectListItem>();
            foreach (string item in depar)
            {
                departmentitems.Add(new SelectListItem
                {
                    Text = item,
                    Value = item
                });
            }
            return departmentitems;
        }
        #endregion

        #region --------------------Permission_group()检索权限组RolesName
        public List<SelectListItem> Permission_group()
        {
            var group = db.UserRolelistTable.OrderByDescending(m => m.Department).Select(c => c.RolesName).Distinct();

            var Grouplist = new List<SelectListItem>();
            foreach (string item in group)
            {
                Grouplist.Add(new SelectListItem
                {
                    Text = item,
                    Value = item
                });
            }
            return Grouplist;
        }
        #endregion

        #region --------------------Discriptionlist()检索权限名Discription
        public List<SelectListItem> Discriptionlist()
        {
            var discriptionName = db.UserRolelistTable.OrderByDescending(m => m.Department == "智造部").Select(c => c.Discription).Distinct();

            var Namelist = new List<SelectListItem>();
            foreach (string item in discriptionName)
            {
                Namelist.Add(new SelectListItem
                {
                    Text = item,
                    Value = item
                });
            }
            return Namelist;
        }
        #endregion

        [HttpPost]
        public JObject UserRolesDeleteListCheck(string department, string rolesname, List<string> codename)
        {
            var disply = new List<UserRolelistTable>();
            disply = db.UserRolelistTable.Where(c => c.Department == department && c.RolesName == rolesname).ToList();//根据部门，权限组找已有的权限名
            var oldlist = disply.Select(c => c.Discription).ToList();//已有描述清单
            var deletelist = oldlist.Except(codename);//delete清单
            var deletecodelist = disply.Where(c => deletelist.Contains(c.Discription)).Select(c => c.RolesCode).ToList();
            List<string> userlist = new List<string>();
            JObject res = new JObject();
            foreach (var i in deletecodelist)
            {
                res.Add(disply.Where(c => c.RolesCode == i).Select(c => c.Discription).FirstOrDefault(), String.Join(",", db.Useroles.Where(c => c.Department == department && c.RolesName == rolesname && c.Roles.Contains(i.ToString())).Select(c => c.UserName).ToList()));
            }
            return res;
        }
        //修改权限名（权限名可以有多个）的方法
        [HttpPost]
        public bool Modifylimits(string department, string rolesname, List<string> codename)
        {
            var disply = new List<UserRolelistTable>();
            disply = db.UserRolelistTable.Where(c => c.Department == department && c.RolesName == rolesname).ToList();//根据部门，权限组找已有的权限名
            var oldlist = disply.Select(c => c.Discription).ToList();//已有描述清单
            var addlist = codename.Except(oldlist);//add清单
            var deletelist = oldlist.Except(codename);//delete清单

            //循环deletelist，删除已有的权限名
            foreach (var item in deletelist)
            {
                var deletecode = disply.Where(c => c.Discription == item).Select(c => c.RolesCode).FirstOrDefault().ToString();
                var deletecodeuserlist = db.Useroles.Where(c => c.Department == department && c.RolesName == rolesname && c.Roles.Contains(deletecode)).ToList();
                foreach (var it in deletecodeuserlist)
                {
                    var tem = it.Roles.Split(',').ToList();
                    tem.Remove(deletecode);
                    it.Roles = String.Join(",", tem);
                    db.SaveChanges();
                }
                db.UserRolelistTable.Remove(disply.Where(c => c.Discription == item).FirstOrDefault());
                db.SaveChanges();
            }
            //循环addlist，往数据库添加修改的新权限名(权限名可以有多个)
            foreach (var item in addlist)
            {
                int it = 0;
                if (db.UserRolelistTable.Count(c => c.Department == department && c.RolesName == rolesname) != 0)
                {
                    it = db.UserRolelistTable.Where(c => c.Department == department && c.RolesName == rolesname).Max(c => c.RolesCode);//取已有权限名对应的RolesCode最大值
                }
                UserRolelistTable record = new UserRolelistTable();
                record.Department = department;
                record.RolesCode = it + 1;
                record.RolesName = rolesname;
                record.Discription = item;
                record.Operator = ((Users)Session["User"]).UserName;
                record.OperateDT = DateTime.Now;
                db.UserRolelistTable.Add(record);
                db.SaveChanges();
            }
            return true;
        }

        #endregion

        #region----修改权限组、修改权限名、添加新权限、删除Useroles表里Roles字段为空的记录

        //修改权限组([Useroles]表和[UserRolelistTables]表)
        [HttpPost]
        public ActionResult Modify(string rolesname, string newrolesname)
        {
            var original = db.UserRolelistTable.Where(c => c.RolesName == rolesname).ToList();
            if (original != null)
            {
                foreach (var item in original)
                {

                    item.Department = item.Department;
                    item.RolesCode = item.RolesCode;
                    item.RolesName = newrolesname;
                    item.Discription = item.Discription;
                    item.Operator = ((Users)Session["User"]).UserName;
                    item.OperateDT = DateTime.Now;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChangesAsync();
                }
                var userr = db.Useroles.Where(c => c.RolesName == rolesname).ToList();
                foreach (var roles in userr)
                {
                    roles.UserName = roles.UserName;
                    roles.UserID = roles.UserID;
                    roles.Department = roles.Department;
                    roles.RolesName = newrolesname;
                    roles.Roles = roles.Roles;
                    roles.Position = roles.Position;
                    db.Entry(roles).State = EntityState.Modified;
                    db.SaveChangesAsync();
                }
                return Content("true");
            }
            return Content("修改出错，请确认数据是否正确");
        }

        //修改权限名
        [HttpPost]
        public ActionResult ModifyName(string rolesname, string discription, string newdiscrip)
        {
            var discrip = db.UserRolelistTable.Where(c => c.Discription == discription).ToList();
            if (discrip != null)
            {
                foreach (var it in discrip)
                {
                    it.Department = it.Department;
                    it.RolesCode = it.RolesCode;
                    it.RolesName = rolesname;
                    it.Discription = newdiscrip;
                    it.Operator = ((Users)Session["User"]).UserName;
                    it.OperateDT = DateTime.Now;
                    db.Entry(it).State = EntityState.Modified;
                    db.SaveChangesAsync();
                }
                return Content("true");
            }
            return Content("修改出错，请确认数据是否正确");
        }

        //添加新权限组和新权限名或者添加权限名
        [HttpPost]
        public ActionResult Addpermission(string department, string rolesname, string codename)
        {
            var disply = db.UserRolelistTable.Where(c => c.Department == department).ToList();
            foreach (var item in disply)
            {
                int i = 0;
                if (db.UserRolelistTable.Count(c => c.Department == department && c.RolesName == rolesname) != 0)
                {
                    i = db.UserRolelistTable.Where(c => c.Department == department && c.RolesName == rolesname).Max(c => c.RolesCode);//取已有权限名对应的RolesCode最大值
                }
                item.Department = department;
                item.RolesCode = i + 1;
                item.RolesName = rolesname;
                item.Discription = codename;
                item.Operator = ((Users)Session["User"]).UserName;
                item.OperateDT = DateTime.Now;
                db.UserRolelistTable.Add(item);
                db.SaveChanges();
                return Content("true");
            }
            return Content("添加失败");
        }

        //删除Useroles表里Roles字段为空的记录
        public void Deleterolename(int id, string rolesname)
        {
            var delete_roles = db.Useroles.Where(c => c.UserID == id && c.RolesName == rolesname).FirstOrDefault();
            var code = db.Useroles.Where(c => c.UserID == id && c.RolesName == rolesname).Select(c => c.Roles).ToList();
            foreach (var item in code)
            {
                if (item.Count() != 0)
                {
                    db.Useroles.Remove(delete_roles);
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region 用户管理

        #region---批量添加用户名
        public ActionResult BatchUsers()
        {
            return View();
        }
        public ActionResult ADDusers(List<Users> users)
        {
            JArray res = new JArray();
            JObject repat = new JObject();
            JObject result = new JObject();
            if (users.Count() != 0)
            {
                foreach (var item in users)
                {
                    item.CreateDate = DateTime.Now;
                    item.Creator = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                    if (db.Users.Count(c => c.UserNum == item.UserNum && c.UserName == item.UserName) > 0)
                    {
                        repat.Add("UserName", item.UserName);
                        repat.Add("UserNum", item.UserNum);
                        res.Add(repat);
                        repat = new JObject();
                    }
                }
                if (res.Count > 0)
                {
                    result.Add("repat", false);
                    result.Add("res", res);
                    return Content(JsonConvert.SerializeObject(result));
                }
                db.Users.AddRange(users);
                int savecount = db.SaveChanges();
                if (savecount > 0)
                {
                    result.Add("repat", true);
                    return Content(JsonConvert.SerializeObject(result));
                }
                else
                {
                    result.Add("repat", false);
                    return Content(JsonConvert.SerializeObject(result));
                }
            }
            result.Add("repat", false);
            return Content(JsonConvert.SerializeObject(result));
        }

        #endregion

        #region----查询系统用户方法、输出Excel表

        public ActionResult Usersquery()
        {
            return View();
        }

        public ActionResult UserqueryList()
        {
            JObject userdeapr = new JObject();
            JObject user = new JObject();
            JObject depar = new JObject();
            JArray table = new JArray();
            JArray deaprList = new JArray();
            JArray deaprList1 = new JArray();
            var departmentlist = db.Users.Select(c => c.Department).Distinct().ToList();//获取用户列表的部门
            foreach (var item in departmentlist)
            {
                var huaming = db.Personnel_Roster.Where(c => c.Department == item).Distinct().ToList();//获取花名册里的部门
                if (item != "智造部" && item != "总经办")
                {
                    var userlist = db.Users.Where(c => c.Department == item).Select(c => new { c.ID, c.Department, c.UserNum, c.UserName }).ToList();
                    if (userlist.Count > 0)
                    {
                        foreach (var ite in userlist)
                        {
                            if (huaming.Count > 0)
                            {
                                //ID
                                depar.Add("ID", ite.ID);
                                //部门
                                depar.Add("Department", ite.Department);
                                //工号
                                depar.Add("UserNum", ite.UserNum);
                                //用户名
                                depar.Add("UserName", ite.UserName);
                                table.Add(depar);
                                depar = new JObject();
                            }
                            else
                            {
                                //ID
                                depar.Add("ID", ite.ID);
                                //部门
                                depar.Add("Department", ite.Department);
                                //工号
                                depar.Add("UserNum", ite.UserNum);
                                //用户名
                                depar.Add("UserName", ite.UserName);
                                table.Add(depar);
                                depar = new JObject();
                            }
                        }
                    }
                }
                if (huaming.Count > 0)
                {
                    user.Add("DeaprList", item);
                    user.Add("Userlist", table);
                    table = new JArray();
                    deaprList.Add(user);
                    user = new JObject();
                }
                else
                {
                    user.Add("DeaprList", item);
                    user.Add("Userlist", table);
                    table = new JArray();
                    deaprList1.Add(user);
                    user = new JObject();
                }
            }
            userdeapr.Add("huizhou", deaprList);
            deaprList = new JArray();
            userdeapr.Add("shenzhen", deaprList1);
            deaprList1 = new JArray();
            return Content(JsonConvert.SerializeObject(userdeapr));
        }

        public class useroutlist
        {
            public string Department { get; set; }
            public string UserName { get; set; }
            public string UserNum { get; set; }
        }


        [HttpPost]
        public FileContentResult UserqueryListToExcel(string flag)
        {
            List<string> expectlist = new List<string>() { "智造部", "制造中心", "厂务中心", "品技中心" };
            string[] columns = { "部门", "姓名", "工号" };
            
            var factorydepartmentlist = db.Personnel_Roster.Select(c => c.Department).Distinct().Except(expectlist).ToList();
            if (flag == "工厂")
            {
                var factoryuserlist = db.Users.Where(c => factorydepartmentlist.Contains(c.Department)).OrderBy(c => c.Department).Select(c => new useroutlist{ Department =c.Department, UserName = c.UserName, UserNum = c.UserNum.ToString() }).ToList();
                factoryuserlist.ForEach(c => {c.UserNum = c.UserNum.PadLeft(5, '0');});
                byte[] filecontent = ExcelExportHelper.ExportExcel(factoryuserlist, "健和MES用户账号清单（" + DateTime.Now.ToString("D") + "）", false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, "健和MES用户账号清单（" + DateTime.Now.ToString("D") + "）.xlsx");
            }
            else if (flag == "总部")
            {
                expectlist.AddRange(factorydepartmentlist);
                var centerdepartmentlist = db.Users.Select(c => c.Department).Distinct().Except(expectlist).ToList();
                var centeruserlist = db.Users.Where(c => centerdepartmentlist.Contains(c.Department)).OrderBy(c => c.Department).Select(c => new useroutlist { Department = c.Department, UserName = c.UserName, UserNum = c.UserNum.ToString() }).ToList();
                centeruserlist.ForEach(c => { c.UserNum = c.UserNum.PadLeft(5, '0'); });
                byte[] filecontent = ExcelExportHelper.ExportExcel(centeruserlist, "深圳总部MES用户账号清单（" + DateTime.Now.ToString("D") + "）", false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, "深圳总部MES用户账号清单（" + DateTime.Now.ToString("D") + "）.xlsx");
            }
            else
            {
                var alluserlist = db.Users.OrderBy(c => c.Department).Select(c => new useroutlist { Department = c.Department, UserName = c.UserName, UserNum = c.UserNum.ToString() }).ToList();
                alluserlist.ForEach(c => { c.UserNum = c.UserNum.PadLeft(5, '0'); });
                byte[] filecontent = ExcelExportHelper.ExportExcel(alluserlist, "所有MES用户账号清单（" + DateTime.Now.ToString("D") + "）", false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, "所有MES用户账号清单（" + DateTime.Now.ToString("D") + "）.xlsx");
            }
        }

        #endregion

        #region---删除已离职用户
        public ActionResult DeleteUser(int id)
        {
            var record = db.Users.Where(c => c.ID == id).FirstOrDefault();//根据ID查询数据
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;//添加删除操作时间
            operaterecord.Operator = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";//添加删除操作人
            //添加操作记录（如：张三在2020年2月26日删除设备关键元器件为李四的记录）
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除名为" + record.UserName + "的账号。";
            db.Users.Remove(record);//删除对应的数据
            db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
            int count = db.SaveChanges();//保存
            if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
                return Content("true");
            else //countt等于0（没有把数据保存到数据库或者保存出错）
                return Content("false");
        }

        #endregion


        #region 之前的
        ////显示用户列表
        //public async Task<ActionResult> UserList(string department)
        //{
        //    var list = db.Users.Where(c => c.Department == department).ToList();
        //    if (list.Count == 0)
        //    {
        //        return Content("没有记录");
        //    }
        //    JObject userInfo = new JObject();
        //    JObject user = new JObject();
        //    for (int i = 0; i < list.Count(); i++)
        //    {
        //        var role = db.Useroles.Where(c => c.UserName == list[i].UserName && c.UserID == list[i].UserNum).Select(c => c.Position).FirstOrDefault();
        //        userInfo.Add("department", list[i].Department);
        //        userInfo.Add("userNum", list[i].UserNum);
        //        userInfo.Add("userName", list[i].UserName);
        //        userInfo.Add("position", role);
        //        userInfo.Add("Time", list[i].CreateDate);
        //        //userMesage.Add("department", userInfo.Department);
        //        //userMesage.Add("userNum", userInfo.UserNum);
        //        //userMesage.Add("userName", userInfo.UserName);
        //        //userMesage.Add("position", userInfo.Role);
        //        //userMesage.Add("Time", userInfo.CreateDate);
        //        user.Add(i.ToString(), userInfo);
        //        userInfo = new JObject();
        //    }
        //    return Content(JsonConvert.SerializeObject(user));
        //}

        ////添加用户信息
        //public ActionResult AddUser([Bind(Include = "ID,UserNum,UserName,Password,CreateDate,Creator,UserAuthorize,Role,Department,LineNum,Deleter,DeleteDate,Description")] Users users)
        //{
        //    if (Session["User"] == null)
        //    {
        //        return RedirectToAction("Login", "Users");
        //    }
        //    users.Creator = ((Users)Session["User"]).UserName;
        //    users.CreateDate = DateTime.Now;
        //    if (ModelState.IsValid)
        //    {
        //        db.Users.Add(users);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        ////修改用户信息
        //public void UpdateUser([Bind(Include = "ID,UserNum,UserName,Password,CreateDate,Creator,UserAuthorize,Role,Department,LineNum,Deleter,DeleteDate,Description")] Users users)
        //{
        //    if (ModelState.IsValid)//判断模型是否有效
        //    {
        //        db.Entry(users).State = EntityState.Modified;

        //    }
        //    var userrole = db.Useroles.Where(c => c.UserID == users.UserNum).FirstOrDefault();
        //    if (userrole.UserName != users.UserName)
        //        userrole.UserName = users.UserName;
        //    if (userrole.Department != users.Department)
        //        userrole.Department = users.Department;
        //    if (userrole.Position != users.Role)
        //        userrole.Position = users.Role;
        //    db.SaveChanges();

        //}

        ////删除用户信息
        //public void DeleteUser(int userid)
        //{
        //    db.Users.Remove(db.Users.Where(c => c.UserNum == userid).FirstOrDefault());
        //    db.Useroles.Remove(db.Useroles.Where(c => c.UserID == userid).FirstOrDefault());
        //    db.SaveChanges();
        //}

        //public async Task<ActionResult> displayRoleNameList()
        //{
        //    JObject List = new JObject();
        //    JObject roleList = new JObject();
        //    //所有权限组列表
        //    var rolename = db.UserRolelistTable.Select(c => c.RolesName).Distinct().ToList();
        //    //所有部门列表
        //    var totaldepartment = db.Users.Select(m => m.Department).Distinct().ToList();
        //    if (rolename.Count() == 0 || totaldepartment.Count() == 0)
        //    {
        //        return Content("没有数据");
        //    }
        //    for (int i = 0; i < rolename.Count(); i++)
        //    {
        //        string grouprole = rolename[i];
        //        var department = db.UserRolelistTable.Where(c => c.RolesName == grouprole).Select(c => c.Department).ToList();
        //        var canadddepar = totaldepartment.Except(department);
        //        roleList.Add("rolename", grouprole);
        //        roleList.Add("havedepartment", department.ToString());
        //        roleList.Add("canadddepartment", canadddepar.ToString());
        //        List.Add(i.ToString(), roleList);
        //        roleList = new JObject();
        //    }
        //    return Content(JsonConvert.SerializeObject(List));
        //}


        #endregion

        // GET: Users
        public ActionResult Index()
        {
            ViewBag.Department = GetDepartmentList();
            ViewBag.Role = GetRoleList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "Index" });
            }
            //if(((Users)Session["User"]).Role== "系统管理员")
            //{
            //    return View(db.Users.OrderBy(m => m.Department).ToList());
            //}
            return View();
        }

        [HttpPost]
        public ActionResult Index(string UserName, string Department/*,int UserAuthorize*/, string Role/*, int LineNum*/, string Description, int UserNum = 0, int PageIndex = 0)
        {
            ViewBag.Department = GetDepartmentList();
            ViewBag.Role = GetRoleList();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "Index" });
            }
            CommonController com = new CommonController();
            if (com.isCheckRole("用户管理", "用户管理", ((Users)Session["User"]).UserName, ((Users)Session["User"]).UserNum))
            {
                var userlist = db.Users.OrderBy(m => m.Department).ToList();

                if (UserNum != 0)
                {
                    userlist = userlist.OrderBy(m => m.Department).Where(m => m.UserNum == UserNum).ToList();
                }
                if (UserName != "")
                {
                    userlist = userlist.OrderBy(m => m.Department).Where(m => m.UserName.Contains(UserName)).ToList();
                }
                if (Department != "")
                {
                    userlist = userlist.Where(m => m.Department == Department).ToList();
                }
                //if (!String.IsNullOrEmpty(UserAuthorize.ToString()))
                //{
                //    userlist = userlist.OrderBy(m => m.Department).Where(m => m.UserAuthorize == UserAuthorize).ToList();
                //}
                if (Role != "")
                {
                    userlist = userlist.OrderBy(m => m.Department).Where(m => m.Role == Role).ToList();
                }
                //if (!String.IsNullOrEmpty(LineNum.ToString()))
                //{
                //    userlist = userlist.OrderBy(m => m.Department).Where(m => m.LineNum == LineNum).ToList();
                //}
                if (Description != "")
                {
                    userlist = userlist.OrderBy(m => m.Department).Where(m => m.Description.Contains(Description)).ToList();
                }
                return View(userlist);
            }
            return View();
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "Details" + "/" + id.ToString() });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "Create" });
            }

            return View();
        }

        // POST: Users/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserNum,UserName,Password,CreateDate,Creator,UserAuthorize,Role,Department,LineNum,Deleter,DeleteDate,Description")] Users users)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "Create" });
            }
            users.Creator = ((Users)Session["User"]).UserName;
            users.CreateDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(users);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "Edit" + "/" + id.ToString() });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserNum,UserName,Password,CreateDate,Creator,UserAuthorize,Role,Department,LineNum,Deleter,DeleteDate,Description")] Users users)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "Edit" + "/" + users.ID.ToString() });
            }

            if (ModelState.IsValid)//判断模型是否有效
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        public ActionResult Modifypwd(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "Modifypwd" + "/" + id.ToString() });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Modifypwd([Bind(Include = "ID,UserNum,UserName,Password,CreateDate,Creator,UserAuthorize,Role,Department,LineNum,Deleter,DeleteDate,Description")] Users users)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "Modifypwd" + "/" + users.ID.ToString() });
            }

            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "Delete" + "/" + id.ToString() });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            db.Users.Remove(users);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion


        #region 用户操作日志查询

        public ActionResult UerserOperateLogsQuery()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UerserOperateLogsQuery(DateTime? starttime, DateTime? endtime, string user, string content)
        {
            IEnumerable<UserOperateLog> loglist = db.UserOperateLog;
            if (!String.IsNullOrEmpty(user))
            {
                loglist = loglist.Where(c => c.Operator.Contains(user));
            }
            if (!String.IsNullOrEmpty(content))
            {
                loglist = loglist.Where(c => c.OperateRecord.Contains(content));
            }
            if (starttime != null && endtime != null)
            {
                loglist = loglist.Where(c => c.OperateDT > starttime && c.OperateDT < endtime);
            }
            var iso = new IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd hh:mm:ss";
            return Content(JsonConvert.SerializeObject(loglist,iso));
        }


        #endregion

        public ActionResult UserTeam()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "UserTeam" });
            }
            return View();
        }


        #region 产线班组操作
        /// <summary>
        /// 各工段获取班组列表
        /// </summary>
        /// 找到登录人的部门,根据部门找到班组,显示
        /// <returns></returns>
        public ActionResult DisplayGroup()
        {
            if (((Users)Session["User"]) == null)//判断是否有登录,没登录返回null
            {
                return null;
            }
            JObject result = new JObject();
            string deparment = ((Users)Session["User"]).Department;//找到部门
            result.Add("department", deparment);
            var grouplist = db.UserTemp.Where(c => c.Department == deparment).Select(c => c.Group).ToList();//根据部门找到班组列表
            JArray list = new JArray();
            grouplist.ForEach(c => list.Add(c));//将班组列表放到jarray 中
            result.Add("Group", list);
            return Content(JsonConvert.SerializeObject(result));

        }


        /// <summary>
        /// 显示班组信息
        /// </summary>
        /// 找到登录人的部门,根据部门找到班组,显示
        /// <returns></returns>
        public ActionResult indexDisplayGroup()
        {
            if (((Users)Session["User"]) == null)//判断是否有登录,没登录返回null
            {
                return null;
            }

            JArray total = new JArray();
            string deparment = ((Users)Session["User"]).Department;//找到部门
            string position = ((Users)Session["User"]).Role;
            if (position == "系统管理员"||deparment=="人力资源部")//如果是智造部显示所有部门班组
            {
                var deparmentlist = db.Personnel_Roster.Where(c=>c.Status!="离职人员").Select(c => c.Department).Distinct();
                foreach (var item in deparmentlist)
                {
                    JObject result = new JObject();
                    result.Add("department", item);
                    var grouplist = db.UserTemp.Where(c => c.Department == item).Select(c => c.Group).ToList();//根据部门找到班组列表
                    JArray list = new JArray();
                    grouplist.ForEach(c => list.Add(c));//将班组列表放到jarray 中
                    result.Add("Group", list);
                    total.Add(result);
                }
            }
            else if (deparment == "MC部")
            {
                JObject result = new JObject();//MC部
                result.Add("department", "MC部");
                var grouplist = db.UserTemp.Where(c => c.Department == "MC部").Select(c => c.Group).ToList();//根据部门找到班组列表
                JArray list = new JArray();
                grouplist.ForEach(c => list.Add(c));//将班组列表放到jarray 中
                result.Add("Group", list);
                total.Add(result);

                JObject result2 = new JObject();//pc部
                result2.Add("department", "PC部");
                var grouplist2 = db.UserTemp.Where(c => c.Department == "PC部").Select(c => c.Group).ToList();//根据部门找到班组列表
                JArray list2 = new JArray();
                grouplist2.ForEach(c => list2.Add(c));//将班组列表放到jarray 中
                result2.Add("Group", list2);
                total.Add(result2);
            }
            else if (deparment == "技术部")
            {
                JObject result = new JObject();//精机部
                result.Add("department", "精机部");
                var grouplist = db.UserTemp.Where(c => c.Department == "精机部").Select(c => c.Group).ToList();//根据部门找到班组列表
                JArray list = new JArray();
                grouplist.ForEach(c => list.Add(c));//将班组列表放到jarray 中
                result.Add("Group", list);
                total.Add(result);

                JObject result2 = new JObject();//技术部
                result2.Add("department", "技术部");
                var grouplist2 = db.UserTemp.Where(c => c.Department == "技术部").Select(c => c.Group).ToList();//根据部门找到班组列表
                JArray list2 = new JArray();
                grouplist2.ForEach(c => list2.Add(c));//将班组列表放到jarray 中
                result2.Add("Group", list2);
                total.Add(result2);
            }
            else
            {
                JObject result = new JObject();
                result.Add("department", deparment);
                var grouplist = db.UserTemp.Where(c => c.Department == deparment).Select(c => c.Group).ToList();//根据部门找到班组列表
                JArray list = new JArray();
                grouplist.ForEach(c => list.Add(c));//将班组列表放到jarray 中
                result.Add("Group", list);
                total.Add(result);
            }
            return Content(JsonConvert.SerializeObject(total));

        }
        /// <summary>
        /// 添加班组
        /// </summary>
        /// <param name="group">班组</param>
        /// <returns></returns>
        public string CreateGroup(string group, string deparment)
        {
            if (((Users)Session["User"]) == null)//判断是否有登录,没登录返回null
            {
                return "失败";
            }
            if (db.UserTemp.Count(c => c.Department == deparment && c.Group == group) != 0)
            {
                return "已有相同班组";
            }//判断是否有相同的班组
            UserTeam temp = new UserTeam() { Department = deparment, Group = group, Createdate = DateTime.Now, Createor = ((Users)Session["User"]).UserName };
            db.UserTemp.Add(temp);
            db.SaveChanges();
            return "成功";
        }


        /// <summary>
        /// 删除班组
        /// </summary>
        /// <param name="group">班组</param>
        /// <returns></returns>
        /// 
        public string DeleteGroup(string group, string deparment)
        {
            if (((Users)Session["User"]) == null)//判断是否有登录,没登录返回null
            {
                return "失败";
            }
            var temp = db.UserTemp.Where(c => c.Department == deparment && c.Group == group).FirstOrDefault();
            db.UserTemp.Remove(temp);
            //填写日志
            UserOperateLog log = new UserOperateLog { OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName, OperateRecord = "删除班组,部门是" + deparment + "班组是" + group };
            db.UserOperateLog.Add(log);
            db.SaveChanges();
            return "成功";
        }

        /// <summary>
        /// 修改班组
        /// </summary>
        /// <param name="newgroup">新班组</param>
        /// <param name="oldgroup">旧班组</param>
        /// <returns></returns>
        public string UpdateGroup(string newgroup, string oldgroup, string deparment)
        {
            if (((Users)Session["User"]) == null)//判断是否有登录,没登录返回null
            {
                return "失败";
            }
            var temp = db.UserTemp.Where(c => c.Department == deparment && c.Group == oldgroup).FirstOrDefault();
            temp.Group = newgroup;
            //填写日志
            UserOperateLog log = new UserOperateLog { OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName, OperateRecord = "修改班组,部门是" + deparment + "原班组是" + oldgroup + "改为" + newgroup };
            db.UserOperateLog.Add(log);
            db.SaveChanges();
            return "成功";
        }
        #endregion
        //给所有用户查看现有的权限大全
        public ActionResult DisplayRoleList(int userid)
        {
            var roleGroupList = db.UserRolelistTable.Select(c => c.RolesName).Distinct().ToList();
            JArray result = new JArray();
            var userRoleCode = db.Useroles.Where(c => c.UserID == userid).Select(c => new { c.Roles, c.RolesName }).ToList();
            foreach (var item in roleGroupList)
            {
                JObject groupJobject = new JObject();
                JArray roleJarray = new JArray();
                var roleList = db.UserRolelistTable.Where(c => c.RolesName == item).Select(c => c.Discription).Distinct().ToList();
                foreach (var role in roleList)
                {
                    JObject roleJobject = new JObject();
                    var code = db.UserRolelistTable.Where(c => c.RolesName == item && c.Discription == role).Select(c => c.RolesCode).Distinct().FirstOrDefault();
                    var rolocodeList = userRoleCode.Where(c => c.RolesName == item).Select(c => c.Roles).FirstOrDefault();
                    if (rolocodeList == null)
                    {
                        roleJobject.Add("name", role);
                        roleJobject.Add("ishaving", false);
                    }
                    else
                    {
                        string[] codeArray = rolocodeList.Split(',');
                        if (Array.IndexOf(codeArray, code.ToString()) == -1)
                        {
                            roleJobject.Add("name", role);
                            roleJobject.Add("ishaving", false);
                        }
                        else
                        {
                            roleJobject.Add("name", role);
                            roleJobject.Add("ishaving", true);
                        }
                    }
                    roleJarray.Add(roleJobject);
                }
                groupJobject.Add("groupName", item);
                groupJobject.Add("role", roleJarray);
                result.Add(groupJobject);
            }

            return Content(JsonConvert.SerializeObject(result));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ViewUserRights()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "ViewUserRights" });
            }
            return View();
        }
        public ActionResult PermissionList()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Users", act = "PermissionList" });
            }
            return View();
        }
        public ActionResult Login()
        {
            if (Session["User"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(Users user, string col, string act)
        {
            var item = db.Users.FirstOrDefault(u => u.UserNum == user.UserNum && u.PassWord == user.PassWord);
            if (item != null)
            {
                if (item.Department == "销售部")
                {

                    Session["User"] = item;
                    return RedirectToAction("businessDepartment", "Query");
                }
                else if (item.Department == "采购部" || item.Department == "合约部" || item.Department == "财务部" || item.Department == "客户服务部")
                {

                    Session["User"] = item;
                    return RedirectToAction("contractDepartment", "Query");

                }
                else
                {
                    Session["User"] = item;
                    if (col != null && act != null)
                        return RedirectToAction(act, col);
                    else
                        return RedirectToAction("Index", "Home");
                }


            }
            ModelState.AddModelError("", "登录出错，请检查员工编号及密码是否有误！");
            return View(user);
        }

        public ActionResult Login2()
        {
            if (Session["User"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login2(Users user, string col, string act)
        {
            var item = db.Users.FirstOrDefault(u => u.UserNum == user.UserNum && u.PassWord == user.PassWord);
            if (item != null)
            {
                if (item.Department == "销售部")
                {

                    Session["User"] = item;
                    return RedirectToAction("businessDepartment", "Query");
                }
                else if (item.Department == "采购部" || item.Department == "合约部" || item.Department == "财务部" || item.Department == "客户服务部")
                {

                    Session["User"] = item;
                    return RedirectToAction("contractDepartment", "Query");

                }
                else
                {
                    Session["User"] = item;
                    if (col != null && act != null)
                        return RedirectToAction(act, col);
                    else
                        return RedirectToAction("Index", "Home");
                }


            }
            ModelState.AddModelError("", "登录出错，请检查员工编号及密码是否有误！");
            return Content("false");
        }

        [HttpPost]
        public ActionResult loginCheck(Users user)
        {
            var item = db.Users.FirstOrDefault(u => u.UserNum == user.UserNum && u.PassWord == user.PassWord);
            if (item != null)
            {
                Session["User"] = item;
                return Content("success");
            }
            return Content("error");
        }
        public ActionResult Logoff()
        {
            Session.Clear();
            return RedirectToAction("Login2", "Users");
        }


        #region ---------------------------------------GetDepartmentList()取出部门列表
        private List<SelectListItem> GetDepartmentList()
        {
            var departmentlist = db.Users.Select(m => m.Department).Distinct().ToList();
            var items = new List<SelectListItem>();
            foreach (string department in departmentlist)
            {
                items.Add(new SelectListItem
                {
                    Text = department,
                    Value = department
                });
            }
            return items;
        }
        #endregion

        #region ---------------------------------------GetRoleList()取出角色列表
        private List<SelectListItem> GetRoleList()
        {
            var rolelist = db.Users.Select(m => m.Role).Distinct().ToList();
            var items = new List<SelectListItem>();
            foreach (string role in rolelist)
            {
                items.Add(new SelectListItem
                {
                    Text = role,
                    Value = role
                });
            }
            return items;
        }
        #endregion


        #region ---------------------------------------分页
        private static readonly int PAGE_SIZE = 10;

        private int GetPageCount(int recordCount)
        {
            int pageCount = recordCount / PAGE_SIZE;
            if (recordCount % PAGE_SIZE != 0)
            {
                pageCount += 1;
            }
            return pageCount;
        }
        #endregion


    }
}
