using JianHeMES.Models;
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

        public ActionResult Permissions(int id)
        {

            var idlist = db.Useroles.Where(c => c.UserID == id && c.RolesName == "人事日报管理").Select(c => c.Roles).FirstOrDefault();
            if (string.IsNullOrEmpty(idlist))
            {
                return Content("null");
            }
            string[] strCharArr = idlist.Split(',');
            JObject limits = new JObject();
            int i = 0;
            foreach (var item in strCharArr)
            {
                int code = int.Parse(item);
                var Perm = db.UserRolelistTable.Where(c => c.RolesName == "人事日报管理" && c.RolesCode == code).Select(c => c.Discription).FirstOrDefault();
                limits.Add(i.ToString(), Perm);
                i++;
            }
            return Content(JsonConvert.SerializeObject(limits));
        }
    }
}