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
            var userrolelisttable = db.UserRolelistTable.ToList();
            foreach (var item in idlist)
            {
                var recordlist = userrolelisttable.Where(c => c.RolesName == item.RolesName).ToList();
                string[] strCharArr = item.Roles.Split(',');
                for (int i = 0; strCharArr.Count() > i; i++)
                {
                    int code = int.Parse(strCharArr[i]);
                    var Perm = recordlist.Where(c => c.RolesCode == code).FirstOrDefault().Discription;
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
        //挪用工序操作
        public void NuoOperation(string nuoBarCode, string nuoOrder, string newbarcode,string newOrdernum)
        {

            var barcode = db.BarCodes.Where(c => c.OrderNum == nuoOrder && c.BarCodesNum == nuoBarCode).FirstOrDefault();
            var appearancebarcode = db.BarCodes.Where(c => c.OrderNum == newOrdernum && c.BarCodesNum == newbarcode).FirstOrDefault();
            appearancebarcode.ModuleGroupNum = barcode.ModuleGroupNum;
            barcode.ModuleGroupNum = null;
            System.IO.File.Delete(@"D:\MES_Data\TemDate\OrderSequence\" + nuoOrder + ".json");
            var oldpqc = db.Assemble.Where(c => c.OrderNum == nuoOrder && c.BoxBarCode == nuoBarCode).ToList();
            foreach (var oldpqcitem in oldpqc)
            {
                if (oldpqcitem.OldOrderNum == null && oldpqcitem.OldBarCodesNum == null)
                {
                    oldpqcitem.OldOrderNum = oldpqcitem.OrderNum;
                    oldpqcitem.OldBarCodesNum = oldpqcitem.BoxBarCode;
                }
                oldpqcitem.OrderNum = newOrdernum; oldpqcitem.BoxBarCode = newbarcode;
            }

            var oldfqc = db.FinalQC.Where(c => c.OrderNum == nuoOrder && c.BarCodesNum == nuoBarCode).ToList();
            foreach (var oldpqcitem in oldfqc)
            {
                if (oldpqcitem.OldOrderNum == null && oldpqcitem.OldBarCodesNum == null)
                {
                    oldpqcitem.OldOrderNum = oldpqcitem.OrderNum;
                    oldpqcitem.OldBarCodesNum = oldpqcitem.BarCodesNum;
                }
                oldpqcitem.OrderNum = newOrdernum; oldpqcitem.BarCodesNum = newbarcode;
            }

            var oldburnM = db.Burn_in_MosaicScreen.Where(c => c.OrderNum == nuoOrder && c.BarCodesNum == nuoBarCode).ToList();
            foreach (var oldpqcitem in oldburnM)
            {
                if (oldpqcitem.OldOrderNum == null && oldpqcitem.OldBarCodesNum == null)
                {
                    oldpqcitem.OldOrderNum = oldpqcitem.OrderNum;
                    oldpqcitem.OldBarCodesNum = oldpqcitem.BarCodesNum;
                }
                oldpqcitem.OrderNum = newOrdernum; oldpqcitem.BarCodesNum = newbarcode;
            }

            var oldBurn = db.Burn_in.Where(c => c.OrderNum == nuoOrder && c.BarCodesNum == nuoBarCode).ToList();
            foreach (var oldpqcitem in oldBurn)
            {
                if (oldpqcitem.OldOrderNum == null && oldpqcitem.OldBarCodesNum == null)
                {
                    oldpqcitem.OldOrderNum = oldpqcitem.OrderNum;
                    oldpqcitem.OldBarCodesNum = oldpqcitem.BarCodesNum;
                }
                oldpqcitem.OrderNum = newOrdernum; oldpqcitem.BarCodesNum = newbarcode;
            }

            var oldcali = db.CalibrationRecord.Where(c => c.OrderNum == nuoOrder && c.BarCodesNum == nuoBarCode).ToList();
            foreach (var oldpqcitem in oldcali)
            {
                if (oldpqcitem.OldOrderNum == null && oldpqcitem.OldBarCodesNum == null)
                {
                    oldpqcitem.OldOrderNum = oldpqcitem.OrderNum;
                    oldpqcitem.OldBarCodesNum = oldpqcitem.BarCodesNum;
                }
                oldpqcitem.OrderNum = newOrdernum; oldpqcitem.BarCodesNum = newbarcode;
            }

            var oldapper = db.Appearance.Where(c => c.OrderNum == nuoOrder && c.BarCodesNum == nuoBarCode).ToList();
            foreach (var oldpqcitem in oldapper)
            {
                if (oldpqcitem.OldOrderNum == null && oldpqcitem.OldBarCodesNum == null)
                {
                    oldpqcitem.OldOrderNum = oldpqcitem.OrderNum;
                    oldpqcitem.OldBarCodesNum = oldpqcitem.BarCodesNum;
                }
                oldpqcitem.OrderNum = newOrdernum; oldpqcitem.BarCodesNum = newbarcode;
            }
            db.SaveChanges();
        }

        //查找当前订单号的出入库情况
        public List<Warehouse_Join> GetCurrentwarehousList(string ordernum)
        {
            var list = db.Warehouse_Join.Where(c => c.OrderNum == ordernum).ToList();

            var chongfu = list.Where(c => list.Count(a => a.BarCodeNum == c.BarCodeNum) > 1).Select(c => c.BarCodeNum).Distinct().ToList();
            var dange = list.Where(c => list.Count(a => a.BarCodeNum == c.BarCodeNum) == 1).Select(c => c.OuterBoxBarcode).Distinct().ToList();

            if (chongfu.Count == 0)
            {
                return list;
            }
            else
            {
                var time = list.Where(c => list.Count(a => a.BarCodeNum == c.BarCodeNum) > 1).Max(c => c.Date);
                DateTime updatetime = new DateTime(2019, 10, 17, 12, 00, 00);
                if (time < updatetime)
                {
                    return list;
                }
                List<Warehouse_Join> result = new List<Warehouse_Join>();
                foreach (var dan in dange)
                {
                    var info = list.Where(c => c.OuterBoxBarcode == dan).ToList();
                    if (info.FirstOrDefault().IsOut == false)
                    {
                        result.AddRange(info);
                    }
                    else
                    {
                        if (db.Packing_BarCodePrinting.Count(c => c.OrderNum == ordernum && c.OuterBoxBarcode == dan) == 0)
                        {
                            result.AddRange(info);
                        }
                    }
                }
                foreach (var item in chongfu)
                {
                    var current = list.Where(c => c.BarCodeNum == item).Select(c => c.IsOut).ToList();
                    if (current.Contains(false))
                    {
                        Warehouse_Join aa = list.Where(c => c.BarCodeNum == item && c.IsOut == false).FirstOrDefault();
                        result.Add(aa);
                    }
                    else
                    {
                        if (db.Packing_BarCodePrinting.Count(c => c.OrderNum == ordernum && c.BarCodeNum == item) != 0)
                        {
                            continue;
                        }
                        var max = list.Where(c => c.BarCodeNum == item).Max(c => c.WarehouseOutNum);
                        Warehouse_Join aa = list.Where(c => c.BarCodeNum == item && c.WarehouseOutNum == max).FirstOrDefault();
                        result.Add(aa);
                    }

                }
                return result;
            }
        }



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