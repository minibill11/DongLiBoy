using JianHeMES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JianHeMES.Controllers
{
    public class Common
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public  bool isCheckRole(string rolename, string discription, string name, int id)
        {
            
            var code = db.UserRolelistTable.Where(c => c.RolesName == rolename && c.Discription == discription).Select(c => c.RolesCode).FirstOrDefault().ToString();
            var codeList = db.Useroles.Where(c => c.UserName == name && c.UserID == id&&c.RolesName==rolename).Select(c => c.Roles).FirstOrDefault();
            if (codeList == null)
                return false;

            return codeList.Contains(code);
        }
    }
}