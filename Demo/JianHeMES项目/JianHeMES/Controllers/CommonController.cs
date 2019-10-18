﻿using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class CommonController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public bool isCheckRole(string rolename, string discription, string name, int id)
        {
            var position = db.Users.Where(c => c.UserNum == id).Select(c => c.Role).FirstOrDefault();
            if (position == "系统管理员")
            {
                return true;
            }
            var code = db.UserRolelistTable.Where(c => c.RolesName == rolename && c.Discription == discription).Select(c => c.RolesCode).FirstOrDefault().ToString();
            var codeList = db.Useroles.Where(c => c.UserName == name && c.UserID == id && c.RolesName == rolename).Select(c => c.Roles).FirstOrDefault();
            if (codeList == null)
                return false;

            return codeList.Contains(code);
        }



        //权限设置
        public ActionResult Permissions(int id)
        {
            var idlist = db.Useroles.Where(c => c.UserID == id).Select(c => new { c.RolesName, c.Roles }).ToList();

            if (idlist == null)
            {
                return Content("null");
            }
            JObject limits = new JObject();
            int k = 0;
            foreach (var item in idlist)
            {
                string[] strCharArr = item.Roles.Split(',');

                for (int i = 0; strCharArr.Count() > i; i++)
                {
                    int code = int.Parse(strCharArr[i]);
                    var Perm = db.UserRolelistTable.Where(c => c.RolesName == item.RolesName && c.RolesCode == code).Select(c => c.Discription).FirstOrDefault();
                    limits.Add(k.ToString(), Perm);
                    k++;
                }
            }
            return Content(JsonConvert.SerializeObject(limits));
        }


        //根据人名传出组织架构的所属上两级
        public ActionResult GetUpTwoLeave(string name)
        {
            JObject result = new JObject();
            var message = db.Personnel_Roster.Where(c => c.Name == name).FirstOrDefault();
            if (message == null)
            {
                return null;
            }
            result.Add("Group", message.DP_Group);
            var list = db.Personnel_Organization.ToList();
            var Leavename = list.Where(c => c.Subordinate.Split(',')[0] == message.DP_Group).Select(c => c.Superior).FirstOrDefault();
            if (Leavename.Substring(Leavename.Length - 2, 2) == "车间")
            {
                result.Add("Workshop", Leavename);
                var depatment = list.Where(c => c.Subordinate.Split(',')[0] == Leavename).Select(c => c.Superior).FirstOrDefault();
                result.Add("Department", depatment);
            }
            else
            {
                result.Add("Workshop", null);
                result.Add("Department", Leavename);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //根据部门或车间找下一级
        public ActionResult GetDownOneLeave(string upLeave)
        {
            JArray workshop = new JArray();
            JArray group = new JArray();
            JObject list = new JObject();
            var message = db.Personnel_Organization.Where(c => c.Superior == upLeave).Select(c => c.Subordinate).ToList();
            if (message == null)
            {
                return null;
            }
            foreach (var item in message)
            {
                var sub = item.Split(',')[0];
                var count = db.Personnel_Organization.Count(c => c.Superior == sub);
                if (count != 0)
                {
                    if (sub.Substring(sub.Length - 2, 2) == "车间")
                        workshop.Add(sub);
                    else if (sub.Substring(sub.Length - 1, 1) == "组")
                        group.Add(sub);
                }
            }
            list.Add("Workshop", workshop);
            list.Add("Group", group);
            return Content(JsonConvert.SerializeObject(list));
        }


        #region-----调用部门版本架构(只用于人事模块)
        public List<Personnel_Architecture> CompanyDatetime(DateTime executionTime)
        {
            var exdate = db.Personnel_Architecture.Where(c => c.ExecutionTime <= executionTime).Max(c => c.ExecutionTime);//前端传来的时间和数据库里部门版本的执行时间对比大小

            //从数据库找到所有的部门、部门负责人、编制人数、刚需人数   .Select(c => new { c.Department, c.Principal, c.Aurhorized_personnel, c.Need_personnel })
            var datelist = db.Personnel_Architecture.Where(c=>c.ExecutionTime==exdate).ToList();

            return datelist;
        }
        #endregion

        #region----部门版本（属于公共方法）
        public ActionResult comany(DateTime exce)
        {
            var exdate = db.Personnel_Architecture.Where(c => c.ExecutionTime <= exce).Max(c => c.ExecutionTime);//前端传来的时间和数据库里部门版本的执行时间对比大小
            //从数据库找到所有的部门、部门负责人、编制人数、刚需人数   
            var datelist = db.Personnel_Architecture.Where(c => c.ExecutionTime == exdate).Select(c => c.Department).ToList();
            return Content(JsonConvert.SerializeObject(datelist));
        }
        #endregion

        //public ActionResult Permissions(int id)
        //{

        //    var idlist = db.Useroles.Where(c => c.UserID == id && c.RolesName == "人事日报管理").Select(c => c.Roles).FirstOrDefault();
        //    if (string.IsNullOrEmpty(idlist))
        //    {
        //        return Content("null");
        //    }
        //    string[] strCharArr = idlist.Split(',');
        //    JObject limits = new JObject();
        //    int i = 0;
        //    foreach (var item in strCharArr)
        //    {
        //        int code = int.Parse(item);
        //        var Perm = db.UserRolelistTable.Where(c => c.RolesName == "人事日报管理" && c.RolesCode == code).Select(c => c.Discription).FirstOrDefault();
        //        limits.Add(i.ToString(), Perm);
        //        i++;
        //    }
        //    return Content(JsonConvert.SerializeObject(limits));
        //}
    }

}