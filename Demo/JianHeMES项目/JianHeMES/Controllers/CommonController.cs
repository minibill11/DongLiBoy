﻿using JianHeMES.Areas.KongYaHT.Models;
using JianHeMES.AuthAttributes;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class CommonController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();

        #region 权限+用户
        /// <summary>
        /// 检查权限,现在应该是没用的,部分代码还用着
        /// </summary>
        /// <param name="rolename">权限组</param>
        /// <param name="discription">权限名</param>
        /// <param name="name">姓名</param>
        /// <param name="id">工号</param>
        /// <returns></returns>
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
                if (item.RolesName != null && item.Roles != null)
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
            }
            return Content(JsonConvert.SerializeObject(limits));
        }

        //权限数组列表
        public ActionResult PermissionsArr(int id)
        {
            JArray limits = new JArray();
            var idlist = db.Useroles.Where(c => c.UserID == id).Select(c => new { c.RolesName, c.Roles }).ToList();
            if (idlist == null)
            {
                return Content(JsonConvert.SerializeObject(limits));
            }
            var userrolelisttable = db.UserRolelistTable.ToList();
            foreach (var item in idlist)
            {
                if (item.RolesName != null && item.Roles != null)
                {
                    var recordlist = userrolelisttable.Where(c => c.RolesName == item.RolesName).ToList();
                    string[] strCharArr = item.Roles.Split(',');
                    for (int i = 0; strCharArr.Count() > i; i++)
                    {
                        int code = int.Parse(strCharArr[i]);
                        var Perm = recordlist.Where(c => c.RolesCode == code).FirstOrDefault().Discription;
                        limits.Add(Perm);
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(limits));
        }

        //输入工号显示人名
        public string DisplayName(string jobNum)
        {
            var name = db.Personnel_Roster.Where(c => c.JobNum == jobNum).Select(c => c.Name).FirstOrDefault();
            if (name == null)
                return "false";
            else
                return name;
        }

        #endregion

        #region  组织架构/人事
        /// <summary>
        /// 根据人名传出组织架构的所属上两级
        /// </summary>
        /// 先根据花名册信息找到该姓名所属的组,再根据组在组织表找到此组上一个所属,判断上一级所属是车间还是部门,如果是车间,则根据车间再找到上一层所属部门,将组,车间,部门传给前端.如果是部门,则把组,部门返回给前面
        /// <param name="name">姓名</param>
        /// <returns></returns>
        public ActionResult GetUpTwoLeave(string name)
        {
            JObject result = new JObject();
            var message = db.Personnel_Roster.Where(c => c.Name == name).FirstOrDefault();//根据姓名查找花名册信息
            if (message == null)//如果信息为空返回null
            {
                return null;
            }
            result.Add("Group", message.DP_Group);//组
            var list = db.Personnel_Organization.ToList();//查找组织信息
            var Leavename = list.Where(c => c.Subordinate == message.DP_Group).Select(c => c.Superior).FirstOrDefault();//根据组的信息找到上一个所属级
            if (Leavename.Substring(Leavename.Length - 2, 2) == "车间")//如果所属级是车间
            {
                result.Add("Workshop", Leavename);//车间
                var depatment = list.Where(c => c.Subordinate.Split(',')[0] == Leavename).Select(c => c.Superior).FirstOrDefault();//根据车间找到上一个所属级,部门
                result.Add("Department", depatment);//部门
            }
            else//如果上一个所属级不是车间,那就是部门
            {
                result.Add("Workshop", null);
                result.Add("Department", Leavename);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 根据部门或车间找下层信息
        /// </summary>
        /// 根据穿过来的部门或车间再组织架构表中查找下一层信息,循环下一层信息,再判断下一层信息是不是组以上,是则往workshop,group添加信息.如果不是,表示下一层是职位级别,不返回信息
        /// <param name="upLeave">部门或车间</param>
        /// <returns></returns>
        public ActionResult GetDownOneLeave(string upLeave)
        {
            JArray workshop = new JArray();
            JArray group = new JArray();
            JObject list = new JObject();
            var message = db.Personnel_Organization.Where(c => c.Superior == upLeave).Select(c => c.Subordinate).ToList();//根据所传的值在组织架构找到下一层
            if (message == null)//如果找不到信息,则返回null
            {
                return null;
            }
            foreach (var item in message)//循环信息找到的下一层信息
            {
                var count = db.Personnel_Organization.Count(c => c.Superior == item);//查看这个下层信息还有没有下一层
                if (count != 0)//如果有,代表这层信息最低是组,不会是职位级别
                {
                    if (item.Substring(item.Length - 2, 2) == "车间")//判断是否是车间
                        workshop.Add(item);//添加入车间列表
                    else if (item.Substring(item.Length - 1, 1) == "组")//判断是否是组列表
                        group.Add(item);//添加入组列表
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
            var datelist = db.Personnel_Architecture.Where(c => c.ExecutionTime == exdate).ToList();

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
        #endregion

        #region 模组+模块公用方法
        /// <summary>
        /// 使用了模组号
        /// </summary>
        /// 根据订单号找到本地json文件,没找到直接返回true,本方法只为去掉已用掉的模组号.找到json文件,查看Normal里有没有对应模组号,有则移除,没有则查看Special里有没有对应的模组号,有则移除
        /// <param name="ordenum">订单</param>
        /// <param name="num">模组号</param>
        /// <returns></returns>
        public string DelteMudole(string ordenum, string num)
        {
            try
            {
                if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json") == true)//找到本地json文件
                {

                    var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json");
                    var json = JsonConvert.DeserializeObject<JObject>(jsonstring);//读取文件信息
                    var normal = json["Normal"].ToList();//读取Normal内容
                    if (normal.Contains(num))//查看模组号是否在Normal中,如果在
                    {
                        var index = normal.IndexOf(num);
                        normal[index].Remove();//移除模组号
                        string output2 = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
                        System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json", output2);//保存json文件
                    }
                    else//如果不在
                    {
                        if (json.Property("Special") != null)//查询json文件是否有Special
                        {
                            var special = json["Special"].ToList();//读取Special内容
                            if (special.Contains(num))//查看模组号是否在Special中
                            {
                                var index = special.IndexOf(num);
                                special[index].Remove();//移除模组号
                                string output2 = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
                                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence\" + ordenum + ".json", output2);//保存json文件

                            }
                        }
                    }
                    return "true";

                }
                return "true";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// 修改三个表的模组号
        /// </summary>
        /// 先判断是否有登录,没有登录返回false ,有则,记录旧的条码模组,检点模组,校正模组,将新的 模组号赋给他们,并保存,填写日志
        /// <param name="barcode">条码</param>
        /// <param name="module">模组号</param>
        /// <returns></returns>
        public JObject UpdateTotalModule(string barcode, string module, string name)
        {
            JObject result = new JObject();
            var barcodemodule = db.BarCodes.Where(c => c.BarCodesNum == barcode).FirstOrDefault();

            try
            {
                var oldcalibtationmodule = "";
                var oldappearanmodule = "";
                var modulelist = db.BarCodes.Where(c => c.OrderNum == barcodemodule.OrderNum && !string.IsNullOrEmpty(c.ModuleGroupNum) && c.BarCodesNum != barcode).Select(c => c.ModuleGroupNum).ToList();
                if (modulelist.Contains(module))//判断输入的模组号是否重复
                {
                    var barcodeitem = db.BarCodes.Where(c => c.ModuleGroupNum == module && c.OrderNum == barcodemodule.OrderNum).Select(c => c.BarCodesNum).FirstOrDefault();
                    result.Add("mes", "模组号与条码" + barcodeitem + "重复");
                    result.Add("pass", false);
                    return result;
                }
                //删除json 文件信息
                DelteMudole(barcodemodule.OrderNum, module);

                var oldbarcode = barcodemodule.ModuleGroupNum;//旧的条码模组
                barcodemodule.ModuleGroupNum = module;//将新的赋过去
                var calibtationmodule = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).ToList();//查找校正信息
                if (calibtationmodule.Count() != 0)//如果信息不为null
                {
                    oldcalibtationmodule = string.Join(",", calibtationmodule.Select(c => c.ModuleGroupNum).ToList());//旧的校正模组
                    calibtationmodule.ForEach(c => c.ModuleGroupNum = module);//将新的赋过去
                }
                var appearanmodule = db.Appearance.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).ToList();//查找电检信息
                if (appearanmodule.Count() != 0)//如果信息不为null
                {
                    oldappearanmodule = string.Join(",", appearanmodule.Select(c => c.ModuleGroupNum).ToList());//旧的电检信息
                    appearanmodule.ForEach(c => c.ModuleGroupNum = module);//将新的赋过去
                }
                //填写日志
                UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = name, OperateRecord = "条码是" + barcode + ",原条码模组为" + oldbarcode + ",校正条码模组为" + oldcalibtationmodule + ",电检条码为" + oldappearanmodule + ",修改后条码为" + module };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                result.Add("mes", "修改成功");
                result.Add("pass", true);
                return result;
            }
            catch
            {
                result.Add("mes", "保存失败");
                result.Add("pass", false);
                return result;
            }
        }

        /// <summary>
        /// 挪用工序操作
        /// </summary>
        /// 先把旧的模组号赋值给新的.删除旧订单号创建的json文件,找到生产管控剔除列表,删除其中的旧订单号.找到旧订单,旧模组的组装信息,将组装信息的条码,订单号改成新的条码,订单号,同理FQC,老化拼屏,老化,校正,电检也是如此
        /// <param name="nuoBarCode">被挪的条码</param>
        /// <param name="nuoOrder">被挪的订单</param>
        /// <param name="newbarcode">挪别人的条码</param>
        /// <param name="newOrdernum">挪别人的订单</param>
        public void NuoOperation(string nuoBarCode, string nuoOrder, string newbarcode, string newOrdernum)
        {

            var barcode = db.BarCodes.Where(c => c.OrderNum == nuoOrder && c.BarCodesNum == nuoBarCode).FirstOrDefault();//根据旧条码旧订单找到条码信息
            var appearancebarcode = db.BarCodes.Where(c => c.OrderNum == newOrdernum && c.BarCodesNum == newbarcode).FirstOrDefault();//根据新订单,新条码找到条码信息
            UserOperateLog LOG = new UserOperateLog() { OperateDT = DateTime.Now, Operator = "挪用操作", OperateRecord = "旧条码" + nuoBarCode + "的模组号是" + barcode.ModuleGroupNum + "新条码" + newbarcode + "的模组号是" + appearancebarcode.ModuleGroupNum };
            db.UserOperateLog.Add(LOG);
            appearancebarcode.ModuleGroupNum = barcode.ModuleGroupNum;//把旧的模组号赋给新的模组号
            barcode.ModuleGroupNum = null;//旧模组号定义为null
            if ((System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence")) == true)
            {
                System.IO.File.Delete(@"D:\MES_Data\TemDate\OrderSequence\" + nuoOrder + ".json");//找到旧订单号对应的本地json文件,并删除
            }
            var oldpqc = db.Assemble.Where(c => c.OrderNum == nuoOrder && c.BoxBarCode == nuoBarCode).ToList();//根据旧订单和旧条码找到组装信息
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\ProductionControllerExcept.json") == true)//查找生产管控剔除清单
            {
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\ProductionControllerExcept.json");//读取信息
                var list = jsonstring.Split(',').ToList();
                if (list.Contains(nuoOrder))//循环里面的订单号
                {
                    list.Remove(nuoOrder);//移除掉旧订单
                    string[] array = list.ToArray();
                    jsonstring = string.Join(",", array);
                }
                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\ProductionControllerExcept.json", jsonstring);//保存json文件
            }
            foreach (var oldpqcitem in oldpqc)//循环就订单找到的组装信息
            {
                if (oldpqcitem.OldOrderNum == null && oldpqcitem.OldBarCodesNum == null)//如果OldOrderNum和OldBarCodesNum为空,则把旧订单和旧条码赋值给他
                {
                    oldpqcitem.OldOrderNum = oldpqcitem.OrderNum;
                    oldpqcitem.OldBarCodesNum = oldpqcitem.BoxBarCode;
                }
                oldpqcitem.OrderNum = newOrdernum; oldpqcitem.BoxBarCode = newbarcode;//将新的订单号赋值给就旧的订单号,新放条码号赋值给旧的条码号
            }

            //FQC
            var oldfqc = db.FinalQC.Where(c => c.OrderNum == nuoOrder && c.BarCodesNum == nuoBarCode).ToList();
            foreach (var oldpqcitem in oldfqc)
            {
                if (oldpqcitem.OldOrderNum == null && oldpqcitem.OldBarCodesNum == null)//如果OldOrderNum和OldBarCodesNum为空,则把旧订单和旧条码赋值给他
                {
                    oldpqcitem.OldOrderNum = oldpqcitem.OrderNum;
                    oldpqcitem.OldBarCodesNum = oldpqcitem.BarCodesNum;
                }
                oldpqcitem.OrderNum = newOrdernum; oldpqcitem.BarCodesNum = newbarcode;//将新的订单号赋值给就旧的订单号,新放条码号赋值给旧的条码号
            }
            //老化拼屏
            var oldburnM = db.Burn_in_MosaicScreen.Where(c => c.OrderNum == nuoOrder && c.BarCodesNum == nuoBarCode).ToList();
            foreach (var oldpqcitem in oldburnM)
            {
                if (oldpqcitem.OldOrderNum == null && oldpqcitem.OldBarCodesNum == null)//如果OldOrderNum和OldBarCodesNum为空,则把旧订单和旧条码赋值给他
                {
                    oldpqcitem.OldOrderNum = oldpqcitem.OrderNum;
                    oldpqcitem.OldBarCodesNum = oldpqcitem.BarCodesNum;
                }
                oldpqcitem.OrderNum = newOrdernum; oldpqcitem.BarCodesNum = newbarcode;//将新的订单号赋值给就旧的订单号,新放条码号赋值给旧的条码号
            }
            //老化
            var oldBurn = db.Burn_in.Where(c => c.OrderNum == nuoOrder && c.BarCodesNum == nuoBarCode).ToList();
            foreach (var oldpqcitem in oldBurn)
            {
                if (oldpqcitem.OldOrderNum == null && oldpqcitem.OldBarCodesNum == null)//如果OldOrderNum和OldBarCodesNum为空,则把旧订单和旧条码赋值给他
                {
                    oldpqcitem.OldOrderNum = oldpqcitem.OrderNum;
                    oldpqcitem.OldBarCodesNum = oldpqcitem.BarCodesNum;
                }
                oldpqcitem.OrderNum = newOrdernum; oldpqcitem.BarCodesNum = newbarcode;//将新的订单号赋值给就旧的订单号,新放条码号赋值给旧的条码号
            }
            //校正
            var oldcali = db.CalibrationRecord.Where(c => c.OrderNum == nuoOrder && c.BarCodesNum == nuoBarCode).ToList();
            foreach (var oldpqcitem in oldcali)
            {
                if (oldpqcitem.OldOrderNum == null && oldpqcitem.OldBarCodesNum == null)//如果OldOrderNum和OldBarCodesNum为空,则把旧订单和旧条码赋值给他
                {
                    oldpqcitem.OldOrderNum = oldpqcitem.OrderNum;
                    oldpqcitem.OldBarCodesNum = oldpqcitem.BarCodesNum;
                }
                oldpqcitem.OrderNum = newOrdernum; oldpqcitem.BarCodesNum = newbarcode;//将新的订单号赋值给就旧的订单号,新放条码号赋值给旧的条码号
            }
            //电检
            var oldapper = db.Appearance.Where(c => c.OrderNum == nuoOrder && c.BarCodesNum == nuoBarCode).ToList();
            foreach (var oldpqcitem in oldapper)
            {
                if (oldpqcitem.OldOrderNum == null && oldpqcitem.OldBarCodesNum == null)//如果OldOrderNum和OldBarCodesNum为空,则把旧订单和旧条码赋值给他
                {
                    oldpqcitem.OldOrderNum = oldpqcitem.OrderNum;
                    oldpqcitem.OldBarCodesNum = oldpqcitem.BarCodesNum;
                }
                oldpqcitem.OrderNum = newOrdernum; oldpqcitem.BarCodesNum = newbarcode;//将新的订单号赋值给就旧的订单号,新放条码号赋值给旧的条码号
            }
            db.SaveChanges();
        }

        /// <summary>
        /// 查找当前订单号的出入库情况(包含混装和被混装的),纯模组
        /// </summary>
        /// 查找根据订单的所有出入库信息list,根据订单找到所有混装了别的订单的信息,将两个信息结合.
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
        public List<Warehouse_Join> GetCurrentwarehousList(string ordernum)
        {
            List<Warehouse_Join> result = new List<Warehouse_Join>();

            //查找订单所有的外箱条码列表
            var list = db.Warehouse_Join.Where(c => c.OrderNum == ordernum && c.State == "模组").ToList();//本订单出库
            var list2 = db.Warehouse_Join.Where(c => c.OrderNum != c.NewBarcode && c.NewBarcode == ordernum && c.State == "模组").ToList();//挪用别人的信息
            list.AddRange(list2);

            //查找订单所有未出库的列表
            var falselist = list.Where(c => c.IsOut == false).ToList();
            var faslebarcodeList = falselist.Select(c => c.OuterBoxBarcode).Distinct().ToList();
            //查找已出库，并且不是挪用出库，剔除混装在内的列表
            var truelist = list.Where(c => c.IsOut == true).ToList();

            if (falselist.Count() == 0 && truelist.Count() == 0)//  未出库列表表为0.出库列表为0 ,代表没有符合条件的出入库信息
            {
                return result;
            }

            result.AddRange(falselist);//添加入库信息,不管是几次出库,入库状态都不会重复

            if (truelist.Count() != 0)//出库数量不为零
            {

                foreach (var item in truelist)//循环出库列表
                {
                    if (truelist.Count(c => c.OuterBoxBarcode == item.OuterBoxBarcode) != 0)//选取每一个外箱条码最后一次出库记录
                    {
                        var itemMaxNUm = truelist.Where(c => c.OuterBoxBarcode == item.OuterBoxBarcode).Max(c => c.WarehouseOutNum);
                        var currentwarehouout = truelist.Where(c => c.BarCodeNum == item.BarCodeNum && c.WarehouseOutNum == itemMaxNUm).ToList();
                        result.AddRange(currentwarehouout);//添加进记录
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 查找当前订单号的出入库情况(包含混装和被混装的),模组+模块
        /// </summary>
        /// 查找根据订单的所有出入库信息list,根据订单找到所有混装了别的订单的信息,将两个信息结合.
        /// <param name="ordernum">订单号</param>
        /// <returns></returns>
        public List<Warehouse_Join> GetCurrentwarehousList2(string ordernum)
        {
            List<Warehouse_Join> result = new List<Warehouse_Join>();

            //查找订单所有的外箱条码列表
            var list = db.Warehouse_Join.Where(c => c.OrderNum == ordernum).ToList();//本订单出库
            var list2 = db.Warehouse_Join.Where(c => c.OrderNum != c.NewBarcode && c.NewBarcode == ordernum).ToList();//挪用别人的信息
            list.AddRange(list2);

            //查找订单所有未出库的列表
            var falselist = list.Where(c => c.IsOut == false).ToList();
            var faslebarcodeList = falselist.Select(c => c.OuterBoxBarcode).Distinct().ToList();
            //查找已出库，并且不是挪用出库，剔除混装在内的列表
            var truelist = list.Where(c => c.IsOut == true).ToList();

            if (falselist.Count() == 0 && truelist.Count() == 0)//  未出库列表表为0.出库列表为0 ,代表没有符合条件的出入库信息
            {
                return result;
            }

            result.AddRange(falselist);//添加入库信息,不管是几次出库,入库状态都不会重复

            if (truelist.Count() != 0)//出库数量不为零
            {

                foreach (var item in truelist)//循环出库列表
                {
                    if (truelist.Count(c => c.OuterBoxBarcode == item.OuterBoxBarcode) != 0)//选取每一个外箱条码最后一次出库记录
                    {
                        var itemMaxNUm = truelist.Where(c => c.OuterBoxBarcode == item.OuterBoxBarcode).Max(c => c.WarehouseOutNum);
                        var currentwarehouout = truelist.Where(c => c.BarCodeNum == item.BarCodeNum && c.WarehouseOutNum == itemMaxNUm).ToList();
                        result.AddRange(currentwarehouout);//添加进记录
                    }
                }
            }

            return result;
        }

        #region -----------------工段首页通用模板--------------------
        public class TempIndex
        {
            public int id { get; set; }
            public string OrderNum { get; set; }
            public string BarCodesNum { get; set; }
            public string ModuleGroupNum { get; set; }
            public string Principal { get; set; }
            public string Group { get; set; }
            public bool Finish { get; set; }
            public DateTime? EndTime { get; set; }
            public DateTime? StarTime { get; set; }
            public string BurnInShelfNum { get; set; }
            public int Line { get; set; }
            public bool Repetition { get; set; }
            public string RepetitionCause { get; set; }
            public string RepairCondition { get; set; }
            public string Abnormal { get; set; }

        }
        public JObject GeneralIndex(string OrderNum, List<TempIndex> totalInfo)
        {
            JObject result = new JObject();

            //var totalInfo = db.Appearance.Where(c => c.OrderNum == OrderNum && (c.OldOrderNum == null || c.OldOrderNum == OrderNum)).ToList();
            #region --------------------基础数据-----------------------
            JObject baseinfo = new JObject();
            var totalModuleNum = db.OrderMgm.Where(c => c.OrderNum == OrderNum).Select(c => c.Boxes).FirstOrDefault();
            //订单模组总数
            baseinfo.Add("Boxs", totalModuleNum);
            //总操作数量
            var totalCount = totalInfo.Count();
            baseinfo.Add("totalCount", totalCount);
            //正常数量
            var normalCount = totalInfo.Count(c => c.Finish == true && c.EndTime != null);
            baseinfo.Add("normalCount", normalCount);
            //异常数量
            var abnormalCount = totalInfo.Count(c => c.Finish == false && c.EndTime != null);
            baseinfo.Add("abnormalCount", abnormalCount);
            //完成率
            var completeRate = totalModuleNum == 0 ? 0 : Math.Round((double)normalCount * 100 / totalModuleNum, 2);
            baseinfo.Add("completeRate", completeRate);
            //总时长
            var totalTime = totalInfo.Max(c => c.EndTime) - totalInfo.Min(c => c.StarTime);
            if (totalTime == null)
            {
                baseinfo.Add("totalTime", null);
                baseinfo.Add("averageTime", null);
            }
            else
            {
                if (totalTime.Value.Days != 0)
                {
                    baseinfo.Add("totalTime", totalTime.Value.Days + "天" + totalTime.Value.Hours + "时" + totalTime.Value.Minutes + "分" /*+ totalTime.Value.Seconds + "秒"*/);
                }
                else
                {
                    baseinfo.Add("totalTime", totalTime.Value.Hours + "时" + totalTime.Value.Minutes + "分" /*+ totalTime.Value.Seconds + "秒"*/);
                }
                //平均时长
                var count = totalInfo.Count(c => c.EndTime != null);
                var averageTime = totalTime.Value.TotalSeconds / count;
                if (averageTime >= 86400)//一天=86400秒
                {
                    var day = Math.Floor(averageTime / 86400);
                    var hourse = Math.Floor((averageTime - day * 86400) / 3600);
                    var minute = Math.Floor((averageTime - day * 86400 - hourse * 3600) / 60);
                    //var seconds = averageTime - day * 86400 - hourse * 3600 - minute * 60;
                    baseinfo.Add("averageTime", day + "天" + hourse + "时" + minute + "分" /*+ seconds + "秒"*/);
                }
                else
                {
                    var hourse = Math.Floor(averageTime / 3600);
                    var minute = Math.Floor((averageTime - hourse * 3600) / 60);
                    //var seconds = averageTime - hourse * 3600 - minute * 60;
                    baseinfo.Add("averageTime", hourse + "时" + minute + "分" /*+ seconds + "秒"*/);
                }
            }
            //查询时间
            baseinfo.Add("selectTime", DateTime.Now.ToString());
            result.Add("baseinfo", baseinfo);
            #endregion

            #region --------------------未开始列表---------------------
            JArray ListArray = new JArray();
            var barcodelist = db.BarCodes.Where(c => c.OrderNum == OrderNum && c.BarCodeType == "模组").Select(c => c.BarCodesNum).ToList();
            var apperbarcodelist = totalInfo.Select(c => c.BarCodesNum).ToList();
            var notGoinglist = barcodelist.Except(apperbarcodelist).ToList();
            JArray notgoingAttay = new JArray();
            notGoinglist.ForEach(c => { notgoingAttay.Add(c); });
            JObject notgoingObj = new JObject();
            notgoingObj.Add("name", "未开始列表");
            notgoingObj.Add("List", notgoingAttay);
            ListArray.Add(notgoingObj);
            #endregion

            #region -------------------分组进行中列表------------------
            var group = totalInfo.Where(c => c.StarTime != null && c.EndTime == null).Select(c => c.Group).Distinct().ToList();
            foreach (var groupitem in group)
            {
                if (groupitem == "")
                {
                    continue;
                }
                if (string.IsNullOrEmpty(groupitem))
                {
                    var woriking = totalInfo.Where(c => c.StarTime != null && string.IsNullOrEmpty(c.Group) && c.EndTime == null).Select(c => c.BarCodesNum).ToList();
                    JObject item = new JObject
                    {
                        { "name", "没有分配班组进行中列表" },
                        { "List", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(woriking)) }
                    };
                    ListArray.Add(item);
                }
                else
                {
                    var woriking = totalInfo.Where(c => c.StarTime != null && c.Group == groupitem && c.EndTime == null).Select(c => c.BarCodesNum).ToList();
                    JObject item = new JObject
                    {
                        { "name", groupitem + "进行中列表" },
                        { "List", JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(woriking)) }
                    };
                    ListArray.Add(item);
                }
            }
            result.Add("statuList", ListArray);

            #endregion

            #region --------------------班组完成饼图-------------------
            JArray pieArray = new JArray();
            var group2 = totalInfo.Where(c => c.EndTime != null).Select(c => c.Group).Distinct().ToList();
            foreach (var groupitem in group2)
            {
                if (groupitem == "")
                {
                    continue;
                }
                if (string.IsNullOrEmpty(groupitem))
                {
                    var woriking = totalInfo.Where(c => c.EndTime != null && string.IsNullOrEmpty(c.Group)).Select(c => c.BarCodesNum).ToList();
                    JObject pieitem = new JObject
                    {
                    { "name",  "没有分配班组" },
                    { "value", woriking.Count }
                    };
                    pieArray.Add(pieitem);
                }
                else
                {
                    var woriking = totalInfo.Where(c => c.EndTime != null && c.Group == groupitem).Select(c => c.BarCodesNum).ToList();
                    JObject pieitem = new JObject
                    {
                    { "name",  groupitem },
                    { "value", woriking.Count }
                    };
                    pieArray.Add(pieitem);
                }
            }
            result.Add("groupPie", pieArray);
            #endregion

            #region --------------------异常正常饼图-------------------
            JArray Npie = new JArray();
            JObject nornal = new JObject
            {
                { "name", "正常"},
                { "value",normalCount}
            };
            Npie.Add(nornal);
            JObject abnormal = new JObject
            {
                { "name", "异常"},
                { "value",abnormalCount}
            };
            Npie.Add(abnormal);
            result.Add("statuePie", Npie);
            #endregion

            #region -----------------------Table-----------------------
            JArray table = new JArray();
            var group3 = totalInfo.Select(c => c.Group).Distinct().ToList();
            foreach (var groupitem in group3)
            {
                if (groupitem == "")
                {
                    continue;
                }
                if (string.IsNullOrEmpty(groupitem))
                {
                    var info = totalInfo.Where(c => string.IsNullOrEmpty(c.Group)).ToList();
                    JObject item = new JObject();
                    item.Add("group", "没有分配组信息");
                    JArray tableList = new JArray();
                    foreach (var infoitem in info)
                    {
                        JObject obj = new JObject();
                        obj.Add("id", infoitem.id);//订单
                        obj.Add("ordernum", infoitem.OrderNum);//订单
                        obj.Add("barcode", infoitem.BarCodesNum);//条码
                        obj.Add("moduleNum", infoitem.ModuleGroupNum);//模组号
                        obj.Add("BurnInShelfNum", infoitem.BurnInShelfNum);//老化架号
                        obj.Add("Line", infoitem.Line);//产线
                        obj.Add("principal", infoitem.Principal);//操作人
                        obj.Add("startTime", infoitem.StarTime.ToString());//开始时间
                        obj.Add("endTime", infoitem.EndTime.ToString());//结束时间
                        obj.Add("nornal", infoitem.Abnormal);//是否正常
                        obj.Add("RepairCondition", infoitem.RepairCondition);//维修情况
                        obj.Add("Repetition", infoitem.Repetition);//是否重复
                        obj.Add("RepetitionCause", infoitem.RepetitionCause);//重复原因
                        tableList.Add(obj);
                    }
                    item.Add("tableList", tableList);
                    table.Add(item);
                }
                else
                {
                    var info = totalInfo.Where(c => c.Group == groupitem).ToList();
                    JObject item = new JObject();
                    item.Add("group", groupitem);
                    JArray tableList = new JArray();
                    foreach (var infoitem in info)
                    {
                        JObject obj = new JObject();
                        obj.Add("id", infoitem.id);//订单
                        obj.Add("ordernum", infoitem.OrderNum);//订单
                        obj.Add("barcode", infoitem.BarCodesNum);//条码
                        obj.Add("moduleNum", infoitem.ModuleGroupNum);//模组号
                        obj.Add("BurnInShelfNum", infoitem.BurnInShelfNum);//老化架号
                        obj.Add("Line", infoitem.Line);//产线
                        obj.Add("principal", infoitem.Principal);//操作人
                        obj.Add("startTime", infoitem.StarTime.ToString());//开始时间
                        obj.Add("endTime", infoitem.EndTime.ToString());//结束时间
                        obj.Add("nornal", infoitem.Abnormal);//是否正常 
                        obj.Add("RepairCondition", infoitem.RepairCondition);//维修情况
                        obj.Add("Repetition", infoitem.Repetition);//是否重复
                        obj.Add("RepetitionCause", infoitem.RepetitionCause);//重复原因
                        tableList.Add(obj);
                    }
                    item.Add("tableList", tableList);
                    table.Add(item);
                }

            }
            result.Add("table", table);
            #endregion

            return result;
        }

        #endregion

       
        #endregion

        #region 工序产能
        /// <summary>
        /// 获取最后一个版本的工序平衡表集合
        /// </summary>
        /// 将平衡表里的信息根据类型,平台,pcb板进行分组,循环分组,对每一个分组查找他们的版本,找到最后的一个版本,存到balances中,最后返回balances
        /// <returns></returns>
        public List<ProcessBalance> GetNewNumberBalanceInfo()
        {
            var total = db.ProcessBalance.GroupBy(c => new { c.Type, c.ProductPCBnumber, c.Platform, c.Section, c.Title }).ToList();//将类型,平台,PCB板,工段 进行分组
            List<ProcessBalance> balances = new List<ProcessBalance>();
            foreach (var item in total)//循环分组信息
            {
                var balance = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == item.Key.Type && c.ProductPCBnumber == item.Key.ProductPCBnumber && c.Platform == item.Key.Platform && c.Section == item.Key.Section && c.Title == item.Key.Title).FirstOrDefault();//根据每组的分组信息,找到版本号,并读取版本号最高的数据
                balances.Add(balance);

            }
            return balances;
        }

        /// <summary>
        /// 获取最后一个版本的贴片表集合
        /// </summary>
        /// 将贴片表里的信息根据类型,平台,pcb板进行分组,循环分组,对每一个分组查找他们的版本,找到最后的一个版本,存到pcik中,最后返回pcik
        /// <returns></returns>
        public List<Pick_And_Place> GetNewNumberPickInfo()
        {
            var tota2 = db.Pick_And_Place.GroupBy(c => new { c.Type, c.ProductPCBnumber, c.Platform }).ToList();//将类型,平台,PCB板 进行分组
            List<Pick_And_Place> pcik = new List<Pick_And_Place>();
            foreach (var item in tota2)//循环分组信息
            {
                var pickitem = db.Pick_And_Place.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == item.Key.Type && c.ProductPCBnumber == item.Key.ProductPCBnumber && c.Platform == item.Key.Platform).FirstOrDefault();//根据每组的分组信息,找到版本号,并读取版本号最高的数据
                pcik.Add(pickitem);

            }
            return pcik;
        }
        #endregion

        #region SQL语句调用

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T">泛型集合的类型</typeparam>
        /// <param name="tableName">本地数据库表的表名</param>
        /// <param name="list">要插入集合</param>
        public string BulkInsert<T>(string tableName, IList<T> list)
        {
            var str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(str);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open(); //打开Connection连接
            }
            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                try
                {
                    bulkCopy.BatchSize = list.Count;
                    bulkCopy.DestinationTableName = tableName;

                    var table = new DataTable();
                    var props = TypeDescriptor.GetProperties(typeof(T))

                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                        .ToArray();

                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];
                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        table.Rows.Add(values);
                    }

                    bulkCopy.WriteToServer(table);

                }
                catch
                {
                    conn.Close();
                    return "false";
                }
            }
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close(); //关闭Connection连接
            }
            return "true";
        }


        /// <summary>
        /// 单个数据库语句操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public string SQLAloneExecute(string sql)
        {
            var str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(str);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open(); //打开Connection连接
            }

            SqlCommand command = new SqlCommand(sql, conn);
            try
            {
                command.ExecuteNonQuery();
                command.Cancel();
                conn.Close();
                return "true";
            }
            catch (Exception E)
            {
                command.Cancel();
                conn.Close();
                return E.Message;
            }

        }

        #endregion


        /// <summary>
        /// 发送邮箱
        /// </summary>
        /// <param name="sender">发送人</param>
        /// <param name="recipient">收件人</param>
        /// <param name="content">内容</param>
        /// <param name="subject">主题</param>
        /// <param name="ccList">抄送人列表</param>
        public bool SendEmail(string sender, List<string> recipient, string content, string subject, List<string> ccList, string address)
        {
            SmtpClient client = new SmtpClient("192.168.1.100", Convert.ToInt32("25"));//邮箱服务器地址和端口
            MailMessage msg = new MailMessage();
            try
            {
                msg.From = new MailAddress(sender);//发送人
                foreach (var item in recipient)//循环添加收件人
                {
                    msg.To.Add(item);
                }
                // msg.Subject = subject;
                msg.SubjectEncoding = System.Text.Encoding.GetEncoding("UTF-8");//文件主题的字符编码
                msg.IsBodyHtml = false;//是否有网址
                msg.Priority = MailPriority.High;//优先级,最高
                msg.Subject = subject;//主题
                foreach (var cc in ccList)//循环抄送人列表
                {
                    msg.CC.Add(cc);
                }
                msg.Body = content;//邮件主题

                msg.BodyEncoding = Encoding.GetEncoding("UTF-8");//文件内容的字符编码
                msg.IsBodyHtml = true;//是否有网址
                msg.Priority = MailPriority.Normal;//优先级 一般
                client.Send(msg);//发送邮件
                return true;
            }
            catch (Exception e)
            {
                //string dd = e.Message;
                //string bb = string.Join("-", recipient.ToArray());
                //string xx = string.Join("-", ccList.ToArray());
                //string aa = "sender:" + sender + ",content:" + content + ",subject:" + subject + ",address:" + address + ",recipient:" + bb + ",ccList:" + xx + ",message:" + dd;
                //System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\1233.txt", aa);
                return false;
            }
        }

        /// <summary>
        /// 找到每天的实际数据
        /// </summary>
        /// <param name="deparment">部门</param>
        /// <param name="group">班组</param>
        /// <param name="section">工段</param>
        /// <param name="process">工序</param>
        /// <param name="statu">正常或者异常</param>
        /// <param name="type">品质或者效率</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public int AcuteNum3(string deparment, string group, List<string> seactinList, string statu, string type, DateTime time, string Module = "")
        {
            DateTime starttime = time.Date.AddHours(8);
            DateTime endtime = starttime.AddDays(1);
            var total = 0;
            foreach (var item in seactinList)
            {
                var num = 0;
                switch (item)
                {
                    #region 模组
                    case "模组组装":
                        if (statu == "正常")
                            num = db.Assemble.Count(c => c.Department1 == deparment && c.Group == group && c.PQCCheckFinish == true && c.PQCCheckFT != null && c.PQCCheckFT >= starttime && c.PQCCheckFT < endtime);
                        else if (statu == "异常")
                            num = db.Assemble.Count(c => c.Department1 == deparment && c.Group == group && c.PQCCheckFinish == false && c.PQCCheckFT != null && c.PQCCheckFT >= starttime && c.PQCCheckFT < endtime);
                        break;
                    case "模组FQC":
                        if (statu == "正常")
                            num = db.FinalQC.Count(c => c.Department1 == deparment && c.Group == group && c.FQCCheckFinish == true && c.FQCCheckFT != null && c.FQCCheckFT >= starttime && c.FQCCheckFT < endtime);
                        else if (statu == "异常")
                            num = db.FinalQC.Count(c => c.Department1 == deparment && c.Group == group && c.FQCCheckFinish == false && c.FQCCheckFT != null && c.FQCCheckFT >= starttime && c.FQCCheckFT < endtime);
                        break;
                    case "模组拼屏":
                        if (statu == "正常")
                            num = db.Burn_in_MosaicScreen.Count(c => c.Department1 == deparment && c.Group == group && c.OQCMosaicStartTime >= starttime && c.OQCMosaicStartTime < endtime);
                        else if (statu == "异常")
                            num = db.Burn_in_MosaicScreen.Count(c => c.Department1 == deparment && c.Group == group && c.OQCMosaicStartTime >= starttime && c.OQCMosaicStartTime < endtime);
                        break;
                    case "模组老化":
                        if (statu == "正常")
                            num = db.Burn_in.Count(c => c.Department1 == deparment && c.Group == group && c.OQCCheckFinish == true && c.OQCCheckFT != null && c.OQCCheckFT >= starttime && c.OQCCheckFT < endtime);
                        else if (statu == "异常")
                            num = db.Burn_in.Count(c => c.Department1 == deparment && c.Group == group && c.OQCCheckFinish == false && c.OQCCheckFT != null && c.OQCCheckFT >= starttime && c.OQCCheckFT < endtime);
                        break;
                    case "模组校正":
                        if (statu == "正常")
                            num = db.CalibrationRecord.Count(c => c.Department1 == deparment && c.Group == group && c.Normal == true && c.FinishCalibration != null && c.FinishCalibration >= starttime && c.FinishCalibration < endtime);
                        else if (statu == "异常")
                            num = db.CalibrationRecord.Count(c => c.Department1 == deparment && c.Group == group && c.Normal == false && c.FinishCalibration != null && c.FinishCalibration >= starttime && c.FinishCalibration < endtime);
                        break;
                    case "模组外观":
                        if (statu == "正常")
                            num = db.Appearance.Count(c => c.Department1 == deparment && c.Group == group && c.OQCCheckFinish == true && c.OQCCheckFT != null && c.OQCCheckFT >= starttime && c.OQCCheckFT < endtime);
                        else if (statu == "异常")
                            num = db.Appearance.Count(c => c.Department1 == deparment && c.Group == group && c.OQCCheckFinish == false && c.OQCCheckFT != null && c.OQCCheckFT >= starttime && c.OQCCheckFT < endtime);
                        break;
                    case "模组包装":
                        num = db.Packing_BarCodePrinting.Count(c => c.Department == deparment && c.Group == group && c.Date != null && c.Date >= starttime && c.Date < endtime);
                        break;
                    #endregion

                    #region 模块
                    case "模块后焊":
                        num = db.AfterWelding.Count(c => c.Department == deparment && c.Group == group && c.AfterWeldingTime != null && c.AfterWeldingTime >= starttime && c.AfterWeldingTime < endtime);
                        break;
                    case "模块灌胶前电检":
                        num = db.ElectricInspection.Count(c => c.Department == deparment && c.Group == group && c.ElectricInspectionTime != null && c.ElectricInspectionTime >= starttime && c.ElectricInspectionTime < endtime && c.Section == "灌胶前电检");
                        break;
                    case "模块灌胶后电检":
                        num = db.ElectricInspection.Count(c => c.Department == deparment && c.Group == group && c.ElectricInspectionTime != null && c.ElectricInspectionTime >= starttime && c.ElectricInspectionTime < endtime && c.Section == "模块电检");
                        break;
                    case "模块老化":
                        num = db.ModuleBurnIn.Count(c => c.Department == deparment && c.Group == group && c.BurnInStartTime != null && c.BurnInStartTime >= starttime && c.BurnInStartTime < endtime);
                        break;
                    case "模块外观电检":
                        num = db.ElectricInspection.Count(c => c.Department == deparment && c.Group == group && c.ElectricInspectionTime != null && c.ElectricInspectionTime >= starttime && c.ElectricInspectionTime < endtime && c.Section == "外观电检");
                        break;
                    case "模块包装"://算外箱
                        num = db.ModuleOutsideTheBox.Count(c => c.Department == deparment && c.Group == group && c.InnerTime != null && c.InnerTime >= starttime && c.InnerTime < endtime);
                        break;

                    #endregion
                    default:
                        if (statu == "")
                        {
                            if (Module == "模块")
                            {
                                List<string> process = new List<string> { "AI", "后焊", "模块", "喷墨", "线材", "送检", "屏体包装", "老化" };
                                var num1 = db.KPI_ActualRecord.Count(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)) == 0 ? 0 : db.KPI_ActualRecord.Where(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)).Sum(c => c.ActualNormalNum);
                                var num2 = db.KPI_ActualRecord.Count(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)) == 0 ? 0 : db.KPI_ActualRecord.Where(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)).Sum(c => c.ActualAbnormalNum);
                                num = num1 + num2;
                            }
                            else if (Module == "模组")
                            {
                                List<string> process = new List<string> { "组装", "模组", "配件包装" };
                                var num1 = db.KPI_ActualRecord.Count(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)) == 0 ? 0 : db.KPI_ActualRecord.Where(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)).Sum(c => c.ActualNormalNum);
                                var num2 = db.KPI_ActualRecord.Count(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)) == 0 ? 0 : db.KPI_ActualRecord.Where(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)).Sum(c => c.ActualAbnormalNum);
                                num = num1 + num2;
                            }
                            else
                            {
                                var num1 = db.KPI_ActualRecord.Count(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type) == 0 ? 0 : db.KPI_ActualRecord.Where(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type).Sum(c => c.ActualNormalNum);
                                var num2 = db.KPI_ActualRecord.Count(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type) == 0 ? 0 : db.KPI_ActualRecord.Where(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type).Sum(c => c.ActualAbnormalNum);
                                num = num1 + num2;
                            }
                        }
                        else if (statu == "正常")
                        {
                            if (Module == "模块")
                            {
                                List<string> process = new List<string> { "AI", "后焊", "模块", "喷墨", "线材", "送检", "屏体包装", "老化" };
                                num = db.KPI_ActualRecord.Count(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)) == 0 ? 0 : db.KPI_ActualRecord.Where(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)).Sum(c => c.ActualNormalNum);

                            }
                            else if (Module == "模组")
                            {
                                List<string> process = new List<string> { "组装", "模组", "配件包装" };
                                num = db.KPI_ActualRecord.Count(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)) == 0 ? 0 : db.KPI_ActualRecord.Where(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)).Sum(c => c.ActualNormalNum);

                            }
                            else
                            {
                                num = db.KPI_ActualRecord.Count(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type) == 0 ? 0 : db.KPI_ActualRecord.Where(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type).Sum(c => c.ActualNormalNum);
                            }
                        }
                        else
                        {
                            if (Module == "模块")
                            {
                                List<string> process = new List<string> { "AI", "后焊", "模块", "喷墨", "线材", "送检", "屏体包装", "老化" };

                                num = db.KPI_ActualRecord.Count(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)) == 0 ? 0 : db.KPI_ActualRecord.Where(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)).Sum(c => c.ActualAbnormalNum);

                            }
                            else if (Module == "模组")
                            {
                                List<string> process = new List<string> { "组装", "模组", "配件包装" };

                                num = db.KPI_ActualRecord.Count(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)) == 0 ? 0 : db.KPI_ActualRecord.Where(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type && process.Contains(c.Process)).Sum(c => c.ActualAbnormalNum);

                            }
                            else
                            {
                                num = db.KPI_ActualRecord.Count(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type) == 0 ? 0 : db.KPI_ActualRecord.Where(c => c.Department == deparment && c.Group == group && c.ActualTime == time && c.IndicatorsType == type).Sum(c => c.ActualAbnormalNum);
                            }
                        }
                        break;

                }
                total = total + num;
            }
            return total;
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

        //返回前端,统一格式 jobject 用
        public JObject GetModuleFromJobjet(JObject value,bool? Result=null,string Message=null)
        {
            JObject result = new JObject();
            result.Add("PostResult", comm.ReturnApiPostStatus());
            result.Add("Data", value);
            result.Add("Result", Result);
            result.Add("Message", Message);
            return result;
        }

        //返回前端,统一格式 Jarray 用
        public JObject GetModuleFromJarray(JArray value, bool? Result = null, string Message = null)
        {
            JObject result = new JObject();
            result.Add("PostResult", comm.ReturnApiPostStatus());
            result.Add("Data", value);
            result.Add("Result", Result);
            result.Add("Message", Message);
            return result;
        }
    }

    public class Common_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController com = new CommonController();

        public class Temp
        {
            public int id { get; set; }
            public string  BarCodesNum { get; set; }
            public string ModuleGroupNum { get; set; }
            public bool Finsh { get; set; }
            public DateTime? FinshTime { get; set; }
        }

        #region ----------------列表---------------
        //拿到所有订单列表
        [HttpPost]
        [ApiAuthorize]
        public JObject OrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum);    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return GetModuleFromJarray(result);
        }

        //班组列表
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayGroup([System.Web.Http.FromBody]JObject data)
        {
            //参数
            string deparment = data["deparment"].ToString();//部门

            JObject result = new JObject();
           
            var grouplist = db.UserTemp.Where(c => c.Department == deparment).Select(c => c.Group).ToList();//根据部门找到班组列表
            JArray list = new JArray();
            grouplist.ForEach(c => list.Add(c));//将班组列表放到jarray 中

            result.Add("department", deparment);
            result.Add("Group", list);
            return GetModuleFromJobjet(result);

        }
        #endregion

        #region----------------模组---------------
        /// <summary>
        /// 检查条码是否在条码表,并且条码和订单是否相符
        /// </summary>
        /// <param name="Ordernum">订单</param>
        /// <param name="Barcode">条码</param>
        /// <returns></returns>
        public string CheckBarcode(string Ordernum, string Barcode)
        {
            JObject result = new JObject();
            if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == Barcode && u.BarCodeType == "模组") == null)
            {
                return "模组条码不存在，请检查条码输入是否正确！";
            }
            else if (db.BarCodes.FirstOrDefault(u => u.BarCodesNum == Barcode).OrderNum != Ordernum)
            {
                return "该模组条码不属于所选订单，请选择正确的订单号！";
            }

            return "true";
        }

        /// <summary>
        /// 检查生产表中是否有数据
        /// </summary>
        /// <param name="barcode">条码</param>
        /// <param name="Table">表名</param>
        /// <param name="BarcodeColumnName">条码列表名</param>
        /// <param name="FTColumnName">完成时间列表名</param>
        /// <param name="FinshColumnName">完成状态列表名</param>
        /// <returns></returns>
        public string CheckSectionStart(string barcode, string Table, string BarcodeColumnName, string FTColumnName, string FinshColumnName)
        {
            //1.检查是否有生产数据.没有则新增一条数据
            //2.检查时候有没有完成时间的数据,有则修改这条数据
            //3.检查是否正常完成的数据,如果没有,则新建一条数据
            //4.检查是否有正常完成的数据,如果有,则是重复数据
            string sql = "Select Count(*) From " + Table + " Where " + BarcodeColumnName + " ='" + barcode + "' AND OldBarCodesNum='" + barcode + "'";
            var count = SQLAloneExecute(sql);
            //1.检查是否有生产数据.没有则新增一条数据
            if (int.Parse(count) == 0)
            {
                return "新增";
            }
            else
            {
                string sql2 = "Select Count(*) From " + Table + " Where " + BarcodeColumnName + " ='" + barcode + "' AND OldBarCodesNum='" + barcode + "' AND " + FTColumnName + " is null";
                var doningcount = SQLAloneExecute(sql2);

                string sql3 = "Select Count(*) From " + Table + " Where " + BarcodeColumnName + " ='" + barcode + "' AND OldBarCodesNum='" + barcode + "' AND " + FTColumnName + " is not null and " + FinshColumnName + "='1'";
                var ABcount = SQLAloneExecute(sql3);
                //2.检查时候有没有完成时间的数据,有则修改这条数据
                if (int.Parse(doningcount) > 0)
                {
                    return "修改";
                }
                //3.检查是否正常完成的数据,如果没有,则新建一条数据
                else if (int.Parse(ABcount) == 0)
                {
                    return "新增";
                }
                //4.检查是否有正常完成的数据,如果有,则是重复数据
                else
                {
                    return "重复";
                }
            }

        }

        /// <summary>
        /// 单个数据库语句操作,selct第一条数据
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public string SQLAloneExecute(string sql)
        {
            var str = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(str);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open(); //打开Connection连接
            }

            SqlCommand command = new SqlCommand(sql, conn);
            try
            {
                SqlDataReader reader = command.ExecuteReader();
                string value = "";
                while (reader.Read())
                {
                    value = reader[0].ToString();
                }
                command.Cancel();
                conn.Close();
                return value;
            }
            catch (Exception E)
            {
                command.Cancel();
                conn.Close();
                return E.Message;
            }

        }

        /// <summary>
        /// 根据开始时间和结束时间,计算时间间隔
        /// </summary>
        /// <param name="Startime">开始时间</param>
        /// <param name="Endtime">结束时间</param>
        /// <returns></returns>
        public JObject CalculateTimespan(DateTime Startime, DateTime Endtime)
        {
            JObject result = new JObject();
            var CT = Endtime - Startime;
            if (CT.Days > 0)
            {
                result.Add("Date", CT.Days);
                result.Add("Time", new TimeSpan(CT.Hours, CT.Minutes, CT.Seconds));
                result.Add("TimeSpan", CT.Days.ToString() + "天" + CT.Hours.ToString() + "时" + CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒");
            }
            else
            {
                result.Add("Date", 0);
                result.Add("Time", CT);
                result.Add("TimeSpan", CT.Hours.ToString() + "时" + CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒");
            }
            return result;
        }


        /// <summary>
        /// 判断模组号是否重复
        /// </summary>
        /// <param name="ordernum">订单</param>
        /// <param name="barcode">条码</param>
        /// <param name="module">模组号</param>
        /// <param name="name">登录人员姓名</param>
        /// <returns></returns>
        public JObject UpdateTotalModule(string ordernum, string barcode, string module, string name)
        {
            JObject result = new JObject();
            var oldcalibtationmodule = "";
            var oldappearanmodule = "";
            #region 判断模组号是否重复
            //条码模组号列表
            var modulelist = db.BarCodes.Where(c => c.OrderNum == ordernum && !string.IsNullOrEmpty(c.ModuleGroupNum) && c.BarCodesNum != barcode).Select(c => c.ModuleGroupNum).ToList();
            //校正模组号列表
            var calibtationmoduleList = db.CalibrationRecord.Where(c => c.OrderNum == ordernum && c.Normal == true && !string.IsNullOrEmpty(c.ModuleGroupNum) && c.BarCodesNum != barcode && c.OldOrderNum == ordernum).Select(c => c.ModuleGroupNum).Distinct().ToList();
            //外观模组号列表
            var appearanmoduleList = db.Appearance.Where(c => c.OrderNum == ordernum && c.OQCCheckFinish == true && !string.IsNullOrEmpty(c.ModuleGroupNum) && c.BarCodesNum != barcode && c.OldOrderNum == ordernum).Select(c => c.ModuleGroupNum).Distinct().ToList();
            if (calibtationmoduleList.Contains(module) || appearanmoduleList.Contains(module) || modulelist.Contains(module))
            {
                var barcodeitem = db.BarCodes.Where(c => c.ModuleGroupNum == module && c.OrderNum == ordernum).Select(c => c.BarCodesNum).FirstOrDefault();
                result.Add("mes", "模组号与条码" + barcodeitem + "重复");
                result.Add("pass", false);
                return result;
            }
            #endregion

            #region 修改模组号
            try
            {
                //条码表修改
                var barcodemodule = db.BarCodes.Where(c => c.BarCodesNum == barcode).FirstOrDefault();
            var oldbarcode = barcodemodule.ModuleGroupNum;
            barcodemodule.ModuleGroupNum = module;
            //校正表修改
            var calibtationmodule = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).ToList();
            if (calibtationmodule.Count() != 0)//如果信息不为null
            {
                oldcalibtationmodule = string.Join(",", calibtationmodule.Select(c => c.ModuleGroupNum).ToList());//旧的校正模组
                calibtationmodule.ForEach(c => c.ModuleGroupNum = module);//将新的赋过去
            }
            //外观表修改
            var appearanmodule = db.Appearance.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).ToList();//查找电检信息
            if (appearanmodule.Count() != 0)//如果信息不为null
            {
                oldappearanmodule = string.Join(",", appearanmodule.Select(c => c.ModuleGroupNum).ToList());//旧的电检信息
                appearanmodule.ForEach(c => c.ModuleGroupNum = module);//将新的赋过去
            }
            #endregion

            #region 添加日志
            UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = name, OperateRecord = "条码是" + barcode + ",原条码模组为" + oldbarcode + ",校正条码模组为" + oldcalibtationmodule + ",电检条码为" + oldappearanmodule + ",修改后条码为" + module };
            db.UserOperateLog.Add(log);
            db.SaveChanges();
           
            #endregion

            result.Add("mes", "修改成功");
            result.Add("pass", true);
            
            return result;
            }
            catch
            {
                result.Add("mes", "保存失败");
                result.Add("pass", false);
                return result;
            }
        }

        /// <summary>
        /// 删除校正json 模组号
        /// </summary>
        /// <param name="OrderNum">订单</param>
        /// <param name="ModuleGroupNum">模组号</param>
        public void DeleteCaliJosn(string OrderNum,string ModuleGroupNum)
        {
            if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence2\" + OrderNum + ".json") == true)
            {
                var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + OrderNum + ".json");
                var json = JsonConvert.DeserializeObject<JArray>(jsonstring);//读取数据
                var index = json.Where(c => c.ToString() == ModuleGroupNum).FirstOrDefault();
                //var index = json.IndexOf(calibrationRecord.ModuleGroupNum);
                if (index != null)
                {
                    json.Remove(index);//移除模组号
                    string output2 = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + OrderNum + ".json", output2);//保存json文件
                }
            }
        }

        /// <summary>
        ///  查询订单已完成、未完成、未开始条码
        /// </summary>
        /// <param name="orderNum">订单</param>
        /// <param name="Value">需要查询的数据集</param>
        /// <returns></returns>
        public JObject SectionCehckList(string orderNum,List<Temp> Value)
        {
            List<string> NotDoList = new List<string>();//未开始做条码清单
            List<string> AbnormalFinish = new List<string>();//异常完成
            List<string> NeverFinish = new List<string>();//未完成条码清单
            List<string> FinishList = new List<string>();//已完成条码清单
            JObject stationResult = new JObject();//输出结果JObject

            List<string> barcodelist = db.BarCodes.Where(c => c.OrderNum == orderNum && c.BarCodeType == "模组").OrderBy(c => c.BarCodesNum).Select(c => c.BarCodesNum).ToList();
            List<string> recordlist = new List<string>();
            if (Value == null)
            {
                stationResult.Add("NotDoList", JsonConvert.SerializeObject(barcodelist));
                stationResult.Add("AbnormalFinish", JsonConvert.SerializeObject(AbnormalFinish));
                stationResult.Add("NeverFinish", JsonConvert.SerializeObject(NeverFinish));
                stationResult.Add("FinishList", JsonConvert.SerializeObject(FinishList));
            }
            else
            {
                var distinBarcodelist = (from s in Value group s by s.BarCodesNum into g select new { id = g.Max(x => x.id), BarCodesNum = g.Key, Normal = g.OrderByDescending(x => x.id).Select(c => c.Finsh).FirstOrDefault(), ModuleGroupNum = g.OrderByDescending(x => x.id).Select(x => x.ModuleGroupNum).FirstOrDefault(), FT = g.OrderByDescending(x => x.id).Select(x => x.FinshTime).FirstOrDefault() }).ToList();
                distinBarcodelist = distinBarcodelist.OrderBy(c => c.BarCodesNum).ToList();
                //拿到异常完成条码清单
                AbnormalFinish = distinBarcodelist.Where(c => c.FT != null && c.Normal == false).Select(c => c.BarCodesNum).ToList();
                //拿到表格中存在的条码清单
                var anoBarcode = distinBarcodelist.Select(c => c.BarCodesNum).ToList();
                //未开始做条码清单
                NotDoList = barcodelist.Except(anoBarcode).ToList();
                //未完成条码清单
                NeverFinish = distinBarcodelist.Where(c => c.Normal == false && c.FT == null).Select(c => c.BarCodesNum + "   " + c.ModuleGroupNum).ToList();
                //已完成条码清单
                FinishList = distinBarcodelist.Where(c => c.Normal == true).Select(c => c.BarCodesNum + "   " + c.ModuleGroupNum).ToList();


                stationResult.Add("NotDoList", JsonConvert.SerializeObject(NotDoList));
                stationResult.Add("AbnormalFinish", JsonConvert.SerializeObject(AbnormalFinish));
                stationResult.Add("NeverFinish", JsonConvert.SerializeObject(NeverFinish));
                stationResult.Add("FinishList", JsonConvert.SerializeObject(FinishList));
            }
            return stationResult;
        }

        /// <summary>
        /// 建立模组号规则前,删除模组号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject ModuleNumDeleteBeforeJson([System.Web.Http.FromBody]JObject data)
        {
            string ordernum = data["ordernum"].ToString();
            string UserName = data["UserName"].ToString();
            JObject result = new JObject();

            //查看订单是否挪用了
            var isnuo = db.BarCodeRelation.Count(c => c.NewOrderNum == ordernum);
            //查看订单是否已经包装了
            var ispachage = db.Packing_BarCodePrinting.Count(c => c.OrderNum == ordernum);
            if (isnuo == 0)
            {
                result.Add("mes", "此订单未曾挪用过别的订单,不能删除模组号");
                result.Add("pass", false);
                return GetModuleFromJobjet(result);
            }
            else if (ispachage != 0)
            {
                result.Add("mes", "此订单已经包装,不能删除模组号");
                result.Add("pass", false);
                return GetModuleFromJobjet(result);

            }
            else
            {
                //删除条码表的模组号
                string updatebarcodesql = "Update BarCodes SET ModuleGroupNum = NULL  where OrderNum='" + ordernum + "'";
                //删除校正表的模组号
                string updatecalisql = "Update CalibrationRecords SET ModuleGroupNum = NULL  where OrderNum='" + ordernum + "'";
                //删除外观表的模组号
                string updateappsql = "Update Appearances SET ModuleGroupNum = NULL  where OrderNum='" + ordernum + "'";

                var bar = com.SQLAloneExecute(updatebarcodesql);
                var cali = com.SQLAloneExecute(updatecalisql);
                var app = com.SQLAloneExecute(updateappsql);
                
                if (bar == "true" && cali == "true" && app == "true")
                {
                    UserOperateLog log = new UserOperateLog() { Operator = UserName, OperateDT = DateTime.Now, OperateRecord = "清除订单" + ordernum + "模组号" };
                    db.UserOperateLog.Add(log);
                    db.SaveChanges();
                    result.Add("mes", "删除成功");
                    result.Add("pass", true);
                    return GetModuleFromJobjet(result);
                }
                else
                {
                    UserOperateLog log = new UserOperateLog() { Operator = UserName, OperateDT = DateTime.Now, OperateRecord = "清除订单" + ordernum + "模组号失败.条码表:"+bar+",校正表: "+cali+",外观表: "+app };
                    db.UserOperateLog.Add(log);
                    db.SaveChanges();
                    result.Add("mes", "条码表:"+bar+",校正表:"+cali+",外观表:"+app);
                    result.Add("pass", false);
                    return GetModuleFromJobjet(result);
                }

            }
        }

        #endregion

        //返回前端,统一格式 JObject 用
        public JObject GetModuleFromJobjet(JObject value, bool? Result = null, string Message = null)
        {
            JObject result = new JObject();
            result.Add("PostResult", comm.ReturnApiPostStatus());
            result.Add("Data", value);
            result.Add("Result", Result);
            result.Add("Message", Message);
            return result;
        }

        //返回前端,统一格式 Jarray 用
        public JObject GetModuleFromJarray(JArray value, bool? Result = null, string Message = null)
        {
            JObject result = new JObject();
            result.Add("PostResult", comm.ReturnApiPostStatus());
            result.Add("Data", value);
            result.Add("Result", Result);
            result.Add("Message", Message);
            return result;
        }
    }

    public class CycleRuning
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public CommonController com = new CommonController();
        private kongyadbEntities kongyadb = new kongyadbEntities();
        private ApplicationDbContext equi_db = new ApplicationDbContext();
        private ApplicationDbContext safety_db = new ApplicationDbContext();


        static temp oldvalue;
        public class temp
        {
            public double? effectivecurrent_u { get; set; }
            public double? air1current_u { get; set; }
            public double? air1temperature { get; set; }
            public double? air1pressure { get; set; }
            public DateTime air1time { get; set; }

            public double? air2current_u { get; set; }
            public double? air2temperature { get; set; }
            public double? air2pressure { get; set; }
            public DateTime air2time { get; set; }

            public double? air3current_u { get; set; }
            public double? air3temperature { get; set; }
            public double? air3pressure { get; set; }
            public DateTime air3time { get; set; }

            //airbottle1[0].pressure, airbottle1[0].temperature, airbottle1[0].humidity
            public double? airbottle1pressure { get; set; }
            public double? airbottle1temperature { get; set; }
            public double? airbottle1humidity { get; set; }
            public DateTime airbottle1time { get; set; }

            public double? airbottle2pressure { get; set; }
            public double? airbottle2temperature { get; set; }
            public double? airbottle2humidity { get; set; }

            public double? airbottle3pressure { get; set; }
            public double? airbottle3temperature { get; set; }
            public double? airbottle3humidity { get; set; }

            public double? dery1pressure { get; set; }
            public double? dery1temperature { get; set; }
            public double? dery1humidity { get; set; }

            public double? dery2pressure { get; set; }
            public double? dery2temperature { get; set; }
            public double? dery2humidity { get; set; }

            public double? head3pressure { get; set; }
            public double? head3temperature { get; set; }
            public double? head3humidity { get; set; }

            public double? head4pressure { get; set; }
            public double? head4temperature { get; set; }
            public double? head4humidity { get; set; }
        }

        public class TempTemperatureAndHumidity
        {
            public int id { get; set; }
            public string Name { get; set; }
            public double? Temperature { get; set; }
            public double? Humidity { get; set; }

            public bool tempstatu { get; set; }
            public bool humistatu { get; set; }
        }
        public void Small_Sample_EmailSend()
        {
            var host_inside = "172.16.6.145";
            var host_outside = "hzjhgd.vicp.io";
            while (true)
            {
                var send_list = db.Small_Sample_Email_Sended.Where(c => c.Sended == false).ToList();
                if (send_list.Count > 0)
                {
                    var recipient_list = db.UserItemEmail.Where(c => c.ProcesName == "小样发送").ToList();
                    string recipient_list_string = "";
                    foreach (var item in recipient_list)
                    {
                        recipient_list_string = recipient_list_string == "" ? (item.UserName + "(" + item.EmailAddress + ")") : recipient_list_string + "," + item.UserName + "(" + item.EmailAddress + ")";
                    }
                    var ccList_list = db.UserItemEmail.Where(c => c.ProcesName == "小样抄送").ToList();
                    string ccList_list_string = "";
                    foreach (var item in ccList_list)
                    {
                        ccList_list_string = ccList_list_string == "" ? (item.UserName + "(" + item.EmailAddress + ")") : ccList_list_string + "," + item.UserName + "(" + item.EmailAddress + ")";
                    }
                    var ordernumber_list = send_list.Select(c => c.OrderNumber);
                    string content = "";
                    string address = "";
                    foreach (var record in send_list)
                    {
                        string content_ = @"<a href=""http://" + host_inside + "/Small_Sample/Display?id=" + record.RecordID + "\"> " + record.OrderNumber + "订单的小样报告已经核准。(内网)</a></br><a href=\"http://" + host_outside + "/Small_Sample/Display?id=" + record.RecordID + "\">" + record.OrderNumber + "订单的小样报告已经核准通过。(外网)</a></br>";
                        content = content + content_ + "</br>";
                    }
                    var result = com.SendEmail("xyts@lcjh.local", recipient_list.Select(c => c.EmailAddress).ToList(), content, String.Join(",", ordernumber_list.ToArray()) + "订单的小样报告已经核准通过。", ccList_list.Select(c => c.EmailAddress).ToList(), address);
                    if (result)
                    {
                        foreach (var record in send_list)
                        {
                            record.Sended = true;
                            record.SendDateTime = DateTime.Now;
                            record.SendSituation = "收件人：" + recipient_list_string + "抄送人：" + ccList_list_string;
                            db.SaveChanges();
                        }
                    }
                }
                Thread.CurrentThread.Join(new TimeSpan(0, 2, 0));//阻止设定时间
            }
        }

        #region---设备未按周期保养，系统需发邮件
        public void MonthMain_Email()
        {
            while (true)
            {
                var host_inside = "172.16.6.145";
                var host_outside = "hzjhgd.vicp.io";
                string content = "";
                string address = "";//地址
                string meg = null;
                bool aa = false;
                string theme = "设备未按周期保养情况";//主题  
                DateTime date = DateTime.Now;
                var meglist = equi_db.Equipment_MonthlyMaintenance.Select(c => c.EquipmentNumber).Distinct().ToList();
                var recipient_list = equi_db.UserItemEmail.Where(c => c.ProcesName == "设备未按周期保养发送").ToList();
                string recipient_list_string = "";
                foreach (var it in recipient_list)
                {
                    recipient_list_string = recipient_list_string == "" ? (it.UserName + "(" + it.EmailAddress + ")") : recipient_list_string + "," + it.UserName + "(" + it.EmailAddress + ")";
                }
                var ccList_list = equi_db.UserItemEmail.Where(c => c.ProcesName == "设备未按周期保养抄送");
                string ccList_list_string = "";
                foreach (var ite in ccList_list)
                {
                    ccList_list_string = ccList_list_string == "" ? (ite.UserName + "(" + ite.EmailAddress + ")") : ccList_list_string + "," + ite.UserName + "(" + ite.EmailAddress + ")";
                }
                foreach (var itm in meglist)
                {
                    var equi = equi_db.Equipment_MonthlyMaintenance.OrderByDescending(m => m.Id).Where(c => c.EquipmentNumber == itm).FirstOrDefault();
                    if (date > equi.Nextmainten_cycle)
                    {
                        string tiemer = equi.Year + "-" + equi.Month;
                        string content_list = @"<a href=""http://" + host_inside + "/Equipment/Equipment_MonthlyMaintenance_plan?" + "dates=" + tiemer + "&depar=" + equi.UserDepartment + "\"> "
                        + equi.EquipmentName + "," + "保养周期:" + equi.MaintenanceDate + "-" + equi.Nextmainten_cycle + "," + "保养时间超出有效期"
                        + "(内网)</a></br><a href=\"http://" + host_outside + "/Equipment/Equipment_MonthlyMaintenance_plan?" + "dates=" + tiemer + "&depar=" + equi.UserDepartment + "\"> "
                        + equi.EquipmentName + "," + "保养周期:" + equi.MaintenanceDate + "-" + equi.Nextmainten_cycle + "," + "保养时间超出有效期" + "(外网)</a></br>";
                        content = content + content_list + "</br>";//内容    
                        meg = equi_db.Equipment_Maintenmail.Count(c => c.Situation == "设备未按周期保养" && c.Year == date.Year && c.Month == date.Month && c.EquipmentNumber == equi.EquipmentNumber).ToString();
                        if (meg == "0")
                        {
                            Equipment_Maintenmail eq = new Equipment_Maintenmail();
                            eq.EquipmentNumber = equi.EquipmentNumber;
                            eq.EquipmentName = equi.EquipmentName;
                            eq.Mainten_equipment = equi.Mainten_equipment;
                            eq.MaintenanceDate = equi.MaintenanceDate;
                            eq.Nextmainten_cycle = equi.Nextmainten_cycle;
                            eq.Situation = "设备未按周期保养";
                            eq.Year = date.Year;
                            eq.Month = date.Month;
                            eq.Sended = true;
                            eq.SendDateTime = DateTime.Now;
                            eq.SendSituation = "收件人：" + recipient_list_string + "抄送人：" + ccList_list_string;
                            equi_db.Equipment_Maintenmail.Add(eq);
                            equi_db.SaveChanges();
                            aa = true;
                        }
                    }
                }
                if (!String.IsNullOrEmpty(content))
                {
                    if (aa == true)
                    {
                        com.SendEmail("gaohj@lcjh.local", recipient_list.Select(c => c.EmailAddress).ToList(), content, theme, ccList_list.Select(c => c.EmailAddress).ToList(), address);
                    }
                }
                Thread.CurrentThread.Join(new TimeSpan(24, 0, 0));//阻止设定时间
            }
        }
        #endregion

        #region ---安全库存清单,现有库存低于安全库存，系统须发邮件
        public void Safetystock_Email()
        {
            while (true)
            {
                var host_inside = "172.16.6.145";
                var host_outside = "hzjhgd.vicp.io";
                string content = "";
                string address = "";//地址
                string meg = null;
                string theme = "安全库存清单库存情况";//主题  
                bool aa = false;
                DateTime date = DateTime.Now;
                var security = safety_db.Equipment_Safetystock.ToList();
                var querylist = CommonERPDB.ERP_Query_SafetyStock(security.Select(c => c.Material).ToList());

                var recipient_list = safety_db.UserItemEmail.Where(c => c.ProcesName == "现有库存低于安全库存发送").ToList();
                string recipient_list_string = "";
                foreach (var it in recipient_list)
                {
                    recipient_list_string = recipient_list_string == "" ? (it.UserName + "(" + it.EmailAddress + ")") : recipient_list_string + "," + it.UserName + "(" + it.EmailAddress + ")";
                }
                var ccList_list = safety_db.UserItemEmail.Where(c => c.ProcesName == "现有库存低于安全库存抄送");
                string ccList_list_string = "";
                foreach (var ite in ccList_list)
                {
                    ccList_list_string = ccList_list_string == "" ? (ite.UserName + "(" + ite.EmailAddress + ")") : ccList_list_string + "," + ite.UserName + "(" + ite.EmailAddress + ")";
                }

                foreach (var item in security)
                {
                    var erp_record = querylist.Where(c => c.img01 == item.Material).FirstOrDefault();
                    string result = System.Text.RegularExpressions.Regex.Replace(item.Safety_stock, @"[^0-9]+", "");//使用正则表达式提取数字
                    if (result != "")
                    {
                        double shuzi = Convert.ToDouble(result);
                        double? img_stock = erp_record == null ? 0 : erp_record.img10;
                        if (shuzi > img_stock)
                        {
                            string url = "&depar=" + System.Web.HttpUtility.UrlEncode(item.EquipmentName);
                            string content_list = @"<a href=""http://" + host_inside + "/Equipment/Equipment_safety?" + url + "\"> "
                                + item.EquipmentName + ",品名:" + item.Descrip + ",物料料号" + item.Material + ",现有库存:" + img_stock + ",安全库存:" + item.Safety_stock + ",需要紧急备料使用"
                                + "(内网)</a></br><a href=\"http://" + host_outside + "/Equipment/Equipment_safety?" + url + "\"> "
                                + item.EquipmentName + ",品名:" + item.Descrip + ",物料料号" + item.Material + ",现有库存:" + img_stock + ",安全库存:" + item.Safety_stock + ",需要紧急备料使用" + "(外网)</a></br>";
                            content = content + content_list + "</br>";//内容     
                            meg = safety_db.Equipment_SafetyEmail.Count(c => c.Material == item.Material && c.Existing_inventory == img_stock && c.EquipmentName == item.EquipmentName).ToString();
                            if (meg == "0")
                            {
                                Equipment_SafetyEmail eq = new Equipment_SafetyEmail();
                                eq.EquipmentName = item.EquipmentName;
                                eq.Descrip = item.Descrip;
                                eq.Material = item.Material;
                                eq.Existing_inventory = img_stock;
                                eq.Safety_stock = item.Safety_stock;
                                eq.Sended = true;
                                eq.SendDateTime = DateTime.Now;
                                eq.SendSituation = "收件人：" + recipient_list_string + "抄送人：" + ccList_list_string;
                                safety_db.Equipment_SafetyEmail.Add(eq);
                                safety_db.SaveChanges();
                                aa = true;
                            }
                        }
                    }
                }
                if (!String.IsNullOrEmpty(content))
                {
                    if (aa == true)
                    {
                        com.SendEmail("gaohj@lcjh.local", recipient_list.Select(c => c.EmailAddress).ToList(), content, theme, ccList_list.Select(c => c.EmailAddress).ToList(), address);
                    }
                }
                Thread.CurrentThread.Join(new TimeSpan(0, 5, 0));//阻止设定时间
            }
        }
        #endregion

        #region 空压房
        /// <summary>
        /// 
        /// </summary>
        public void CheckHarvesterProgram()
        {
            //var host_inside = "172.16.6.145";
            //var host_outside = "hzjhgd.vicp.io";
            DateTime aa = DateTime.Now;
            DateTime lastrecore = DateTime.Now;
            List<bool> onestatu = new List<bool>() { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
            int count = 1;
            int notUpdate = 0;
            int collectcount = 1;
            int pccount = 0;
            while (true)
            {
                ApplicationDbContext db2 = new ApplicationDbContext();
                var recipient_list = db2.UserItemEmail.Where(c => c.ProcesName == "空压房运行情况").Select(c => c.EmailAddress).ToList();//邮件接收人
                                                                                                                                  //var recipient_list = new List<string(); //recipient_list.Add("zhangdy@lcjh.local");
                string content = "";
                string subject = "";//String.Join(",", ordernumber_list.ToArray()
                string address = "";
                List<string> cclist = new List<string>();
                //cclist.Add("hejw@lcjh.local");
                bool one = true;
                bool two = true;
                bool three = true;
                //List<bool> flow = new List<bool>() { true, true, true };
                //List<bool> fire = new List<bool>() { true, true, true };
                //List<bool> six = new List<bool>() { true, true, true };
                //List<bool> sevent = new List<bool>() { true, true, true };
                bool start = false;
                #region 取值
                //空压机1最后两个
                var lastTwoair1comp = kongyadb.aircomp1.OrderByDescending(c => c.id).Take(2).ToArray();
                //与当前时间差值
                var air1time = (DateTime.Now - lastTwoair1comp[0].recordingTime).TotalMinutes;
                air1time = Math.Floor(air1time);

                //空压机2最后两个
                var lastTwoair2comp = kongyadb.aircomp2.OrderByDescending(c => c.id).Take(2).ToArray();
                //与当前时间差值
                var air2time = (DateTime.Now - lastTwoair2comp[0].recordingTime).TotalMinutes;
                air2time = Math.Floor(air2time);

                //空压机3最后两个
                var lastTwoair3comp = kongyadb.aircomp3.OrderByDescending(c => c.id).Take(2).ToArray();
                //与当前时间差值
                var air3time = (DateTime.Now - lastTwoair3comp[0].recordingTime).TotalMinutes;
                air3time = Math.Floor(air3time);

                //气管1最后两个
                var airbottle1 = kongyadb.airbottle1.OrderByDescending(c => c.id).FirstOrDefault();
                var airbottle1time = (DateTime.Now - airbottle1.recordingTime).TotalMinutes;
                airbottle1time = Math.Floor(airbottle1time);

                //气管2最后两个
                var airbottle2 = kongyadb.airbottle2.OrderByDescending(c => c.id).FirstOrDefault();
                var airbottle2time = (DateTime.Now - airbottle2.recordingTime).TotalMinutes;
                airbottle2time = Math.Floor(airbottle2time);

                //气管3最后两个
                var airbottle3 = kongyadb.airbottle3.OrderByDescending(c => c.id).FirstOrDefault();
                var airbottle3time = (DateTime.Now - airbottle3.recordingTime).TotalMinutes;
                airbottle3time = Math.Floor(airbottle3time);

                //冷干1最后两个
                var drey1 = kongyadb.dryer1.OrderByDescending(c => c.id).FirstOrDefault();
                var drey1time = (DateTime.Now - drey1.recordingTime).TotalMinutes;
                drey1time = Math.Floor(drey1time);

                //冷干2最后两个
                var drey2 = kongyadb.dryer2.OrderByDescending(c => c.id).FirstOrDefault();
                var drey2time = (DateTime.Now - drey2.recordingTime).TotalMinutes;
                drey2time = Math.Floor(drey2time);

                //气管3出口最后两个
                var head3 = kongyadb.headerpipe3inch.OrderByDescending(c => c.id).FirstOrDefault();
                var head3time = (DateTime.Now - head3.recordingTime).TotalMinutes;
                head3time = Math.Floor(head3time);

                //气管4出口最后两个
                var head4 = kongyadb.headerpipe4inch.OrderByDescending(c => c.id).FirstOrDefault();
                var head4time = (DateTime.Now - head4.recordingTime).TotalMinutes;
                head4time = Math.Floor(head4time);
                #endregion

                if (oldvalue != null)
                {
                    var collect = Temptimespan(collectcount);//发送采集数据未更新邮箱的间隔时间
                    var pc = Temptimespan(pccount);//发送没有收到数据邮箱间隔时间
                                                   //采集电脑是否有问题
                    if (oldvalue.air1time == lastTwoair1comp[0].recordingTime && oldvalue.airbottle1time == airbottle1.recordingTime && Math.Floor((DateTime.Now - lastrecore).TotalMinutes) == pc)//判断数据库时间是否一致未更新,如果是的话,再与当前时间做对比,看间隔是否属实3,10,30...等分钟,是的话发送邮件
                    {
                        content = "采集没有收到数据";
                        subject = "采集没有收到数据";
                        pccount++;//发邮件次数,第一次3分钟,第二次10分钟....
                        var result = com.SendEmail("hejw@lcjh.local", recipient_list, content, subject, cclist, address);
                        continue;
                    }
                    else if (notUpdate >= (collect * 12))
                    {
                        collectcount++;//发邮件次数,第一次3分钟,第二次10分钟....
                        content = "采集数据一直未更新";
                        subject = "采集数据一直未更新";
                        var result = com.SendEmail("hejw@lcjh.local", recipient_list, content, subject, cclist, address);
                        continue;
                    }
                    else
                    {
                        var currenc = lastTwoair1comp[0].current_u > 40 ? lastTwoair1comp[0].current_u : (lastTwoair2comp[0].current_u > 40 ? lastTwoair2comp[0].current_u : lastTwoair3comp[0].current_u);//判断三个空压机那个在运行,读取那个的值
                        if (oldvalue.effectivecurrent_u == currenc && oldvalue.dery1humidity == drey1.humidity)//判断数据是否未更新
                        {
                            notUpdate++;//记录次数,邮箱检测是5秒一次,采集数据一直未更新第一次是3分钟发一次collect * 12=36; 5秒一次要有36次 ,刚还需要180秒 即3分钟
                        }
                        else
                        {
                            notUpdate = 0;
                            collectcount = 1;
                        }

                        //开关进行通知
                        #region 1.空压机开关机通知（只通知一次）
                        //空压机1
                        if (lastTwoair1comp[0].current_u != oldvalue.air1current_u && air1time == 0)
                        {
                            var value = KYswitch("1#螺杆空压机SA120A", lastTwoair1comp[0].current_u, lastTwoair1comp[0].temperature, lastTwoair1comp[0].pressure, oldvalue.air1current_u, oldvalue.air1temperature, oldvalue.air1pressure);//1#判断螺杆空压机SA120A与上一条数据的电流,压力,温度.判断是否有断电,停运,通电等情况的改变
                            if (value["content"] != "")//有改变
                            {
                                content = content + value["content"];
                                subject = subject + value["subject"];
                                lastrecore = lastTwoair1comp[0].recordingTime;
                            }
                        }
                        //空压机2
                        if (lastTwoair2comp[0].current_u != oldvalue.air2current_u && air2time == 0)
                        {
                            var value = KYswitch("2#空气压缩机90A", lastTwoair2comp[0].current_u, lastTwoair2comp[0].temperature, lastTwoair2comp[0].pressure, oldvalue.air2current_u, oldvalue.air2temperature, oldvalue.air2pressure);//2#空气压缩机90A与上一条数据的电流,压力,温度.判断是否有断电,停运,通电等情况的改变
                            if (value["content"] != "")//有改变
                            {
                                content = content + value["content"];
                                subject = subject + value["subject"];
                                lastrecore = lastTwoair2comp[0].recordingTime;
                            }
                        }
                        //空压机3 3#变频螺杆空压机SA120A
                        if (lastTwoair3comp[0].current_u != oldvalue.air3current_u && air3time == 0)
                        {
                            var value = KYswitch("3#变频螺杆空压机SA120A", lastTwoair3comp[0].current_u, lastTwoair3comp[0].temperature, lastTwoair3comp[0].pressure, oldvalue.air3current_u, oldvalue.air3temperature, oldvalue.air3pressure);//3#变频螺杆空压机SA120A与上一条数据的电流,压力,温度.判断是否有断电,停运,通电等情况的改变
                            if (value["content"] != "")//有改变
                            {
                                content = content + value["content"];
                                subject = subject + value["subject"];
                                lastrecore = lastTwoair3comp[0].recordingTime;
                            }
                        }
                        #endregion

                        #region  2.空压数据值没有更新时通知 3.空压数据值恢复更新时通知一次,与上面采集数据未更新是同样的判断,暂时隐藏
                        ////（第一次3分钟内没有数据更新通知，第二次10分钟，第三次30分钟，第四次1小时，第五次2小时）
                        ////空压机1
                        //if (lastTwoair1comp[0].current_u >= 40)
                        //{
                        //    var value = KYdate("1#螺杆空压机SA120A", air1time, lastTwoair1comp[0].recordingTime, oldvalue.air1time);
                        //    if (value["content"] != "")
                        //    {
                        //        content = content + value["content"];
                        //        subject = subject + value["subject"];
                        //        lastrecore = lastTwoair1comp[0].recordingTime;
                        //    }
                        //}
                        ////空压机2
                        //if (lastTwoair2comp[0].current_u >= 40)
                        //{
                        //    var value = KYdate("2#空气压缩机90A", air2time, lastTwoair2comp[0].recordingTime, oldvalue.air2time);
                        //    if (value["content"] != "")
                        //    {
                        //        content = content + value["content"];
                        //        subject = subject + value["subject"];
                        //        lastrecore = lastTwoair2comp[0].recordingTime;
                        //    }
                        //}
                        ////空压机3
                        //if (lastTwoair3comp[0].current_u >= 40)
                        //{
                        //    var value = KYdate("3#变频螺杆空压机SA120A", air3time, lastTwoair3comp[0].recordingTime, oldvalue.air3time);
                        //    if (value["content"] != "")
                        //    {
                        //        content = content + value["content"];
                        //        subject = subject + value["subject"];
                        //        lastrecore = lastTwoair3comp[0].recordingTime;
                        //    }
                        //}

                        #endregion

                        #region 4.压力偏低或恢复时通知 5.温度或湿度或压力值为0或回复通知  6.气管内湿度高于50%或恢复通知 
                        if (airbottle1time == 0)//与当前时间的间隔小于一分钟,符合条件
                        {
                            var value = pressureTooLow("1#气管出口", airbottle1.pressure, ref one);//查看压力值是否偏低,过低one=false,恢复one=true
                            if (one != onestatu[0])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[0] = one;
                                start = true;//为true则是需要发送邮件
                                lastrecore = airbottle1.recordingTime;//
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            else if (one == false)//为false 则是偏低异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            var value2 = IsZero("1#气管出口", airbottle1.pressure, airbottle1.temperature, ref two);//查看温度或湿度或压力值为0或回复通知
                            if (two != onestatu[1])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[1] = two;
                                start = true;//为true则是需要发送邮件
                                lastrecore = airbottle1.recordingTime;
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }
                            else if (two == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }

                        }

                        if (airbottle2time == 0)
                        {
                            one = true;
                            two = true;
                            var value = pressureTooLow("2#气管出口", airbottle2.pressure, ref one);
                            if (one != onestatu[2])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[2] = one;
                                start = true;//为true则是需要发送邮件
                                lastrecore = airbottle2.recordingTime;
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            else if (one == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            var value2 = IsZero("2#气管出口", airbottle2.pressure, airbottle2.temperature, ref two);
                            if (two != onestatu[3])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[3] = two;
                                start = true;//为true则是需要发送邮件
                                lastrecore = airbottle2.recordingTime;
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }
                            else if (two == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }

                        }

                        if (airbottle3time == 0)
                        {
                            one = true;
                            two = true;
                            var value = pressureTooLow("3#气管出口", airbottle3.pressure, ref one);
                            if (one != onestatu[4])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[4] = one;
                                start = true;//为true则是需要发送邮件
                                lastrecore = airbottle3.recordingTime;
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            else if (one == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            var value2 = IsZero("3#气管出口", airbottle3.pressure, airbottle3.temperature, ref two);
                            if (two != onestatu[5])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[5] = two;
                                start = true;//为true则是需要发送邮件
                                lastrecore = airbottle3.recordingTime;
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }
                            else if (two == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }

                        }

                        if (drey1time == 0)
                        {
                            one = true;
                            two = true;
                            three = true;
                            var value = pressureTooLow("1#冷干机", drey1.pressure, ref one);
                            if (one != onestatu[6])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[6] = one;
                                start = true;//为true则是需要发送邮件
                                lastrecore = drey1.recordingTime;
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            else if (one == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            var value2 = humidityTooHight("1#冷干机", drey1.humidity, ref two);
                            if (two != onestatu[7])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[7] = two;
                                start = true;//为true则是需要发送邮件
                                lastrecore = drey1.recordingTime;
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }
                            else if (two == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }

                            var value3 = IsZero("1#冷干机", drey1.pressure, drey1.temperature, ref three);
                            if (three != onestatu[8])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[8] = three;
                                start = true;//为true则是需要发送邮件
                                lastrecore = drey1.recordingTime;
                                content = content + value3["content"];
                                subject = subject + value3["subject"];
                            }
                            else if (three == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value3["content"];
                                subject = subject + value3["subject"];
                            }

                        }

                        if (drey2time == 0)
                        {
                            one = true;
                            two = true;
                            three = true;
                            var value = pressureTooLow("2#冷干机", drey2.pressure, ref one);
                            if (one != onestatu[9])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[9] = one;
                                start = true;//为true则是需要发送邮件
                                lastrecore = drey2.recordingTime;
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            else if (one == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            var value2 = humidityTooHight("2#冷干机", drey2.humidity, ref two);
                            if (two != onestatu[10])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[10] = two;
                                start = true;//为true则是需要发送邮件
                                lastrecore = drey2.recordingTime;
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }
                            else if (two == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }

                            var value3 = IsZero("2#冷干机", drey2.pressure, drey2.temperature, ref three);
                            if (three != onestatu[11])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[11] = three;
                                start = true;//为true则是需要发送邮件
                                lastrecore = drey2.recordingTime;
                                content = content + value3["content"];
                                subject = subject + value3["subject"];
                            }
                            else if (three == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value3["content"];
                                subject = subject + value3["subject"];
                            }


                        }

                        if (head3time == 0)
                        {
                            one = true;
                            two = true;
                            three = true;
                            var value = pressureTooLow("3寸管出口", head3.pressure, ref one);
                            if (one != onestatu[12])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[12] = one;
                                start = true;//为true则是需要发送邮件
                                lastrecore = head3.recordingTime;
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            else if (one == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            var value2 = humidityTooHight("3寸管出口", head3.humidity, ref two);
                            if (two != onestatu[13])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[13] = two;
                                start = true;//为true则是需要发送邮件
                                lastrecore = head3.recordingTime;
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }
                            else if (two == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }

                            var value3 = IsZero("3寸管出口", head3.pressure, head3.temperature, ref three);
                            if (three != onestatu[14])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[14] = three;
                                start = true;//为true则是需要发送邮件
                                lastrecore = head3.recordingTime;
                                content = content + value3["content"];
                                subject = subject + value3["subject"];
                            }
                            else if (three == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value3["content"];
                                subject = subject + value3["subject"];
                            }

                        }

                        if (head4time == 0)
                        {
                            one = true;
                            two = true;
                            three = true;
                            var value = pressureTooLow("4寸管出口", head4.pressure, ref one);
                            if (one != onestatu[15])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[15] = one;
                                start = true;//为true则是需要发送邮件
                                lastrecore = head4.recordingTime;
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            else if (one == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value["content"];
                                subject = subject + value["subject"];
                            }
                            var value2 = humidityTooHight("4寸管出口", head4.humidity, ref two);
                            if (two != onestatu[16])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[16] = two;
                                start = true;//为true则是需要发送邮件
                                lastrecore = head4.recordingTime;
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }
                            else if (two == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value2["content"];
                                subject = subject + value2["subject"];
                            }

                            var value3 = IsZero("4寸管出口", head4.pressure, head4.temperature, ref three);
                            if (three != onestatu[17])//判断上一次的状态与这次是否不同,如上一次是恢复,这次是偏低,则发邮箱,上一次是恢复,这次是恢复,则不发邮箱
                            {
                                onestatu[17] = three;
                                start = true;//为true则是需要发送邮件
                                lastrecore = head4.recordingTime;
                                content = content + value3["content"];
                                subject = subject + value3["subject"];
                            }
                            else if (three == false)//为false 则是异常,先记录在主题和内容中,如果有触发发邮件的条件,则一起发送出去
                            {
                                content = content + value3["content"];
                                subject = subject + value3["subject"];
                            }
                        }
                        #endregion
                    }
                }
                #region 将 最新的值赋给oldvalue
                oldvalue = new temp
                {
                    effectivecurrent_u = lastTwoair1comp[0].current_u > 40 ? lastTwoair1comp[0].current_u : (lastTwoair2comp[0].current_u > 40 ? lastTwoair2comp[0].current_u : lastTwoair3comp[0].current_u),//找到有运行的空压机电流值,用于判断采集数据是否有更新
                    air1current_u = lastTwoair1comp[0].current_u,//空压机1 电流
                    air1pressure = lastTwoair1comp[0].pressure,//空压机1 压力
                    air1temperature = lastTwoair1comp[0].temperature,//空压机1 温度
                    air1time = lastTwoair1comp[0].recordingTime,//空压机1 时间

                    air2current_u = lastTwoair2comp[0].current_u,//空压机2 电流
                    air2pressure = lastTwoair2comp[0].pressure,//空压机2 压力
                    air2temperature = lastTwoair2comp[0].temperature,//空压机2 温度
                    air2time = lastTwoair2comp[0].recordingTime,//空压机2 时间

                    air3current_u = lastTwoair3comp[0].current_u,//空压机3 电流
                    air3pressure = lastTwoair3comp[0].pressure,//空压机3 压力
                    air3temperature = lastTwoair3comp[0].temperature,//空压机3 温度
                    air3time = lastTwoair3comp[0].recordingTime,//空压机3 时间

                    airbottle1humidity = airbottle1.humidity,//气管1湿度
                    airbottle1pressure = airbottle1.pressure,//气管1 压力
                    airbottle1temperature = airbottle1.temperature,//气管1 温度
                    airbottle1time = airbottle1.recordingTime,//气管1 时间

                    airbottle2humidity = airbottle2.humidity,//气管2 湿度
                    airbottle2pressure = airbottle2.pressure,//气管2 压力
                    airbottle2temperature = airbottle2.temperature,//气管2 温度

                    airbottle3humidity = airbottle3.humidity,//气管3 湿度
                    airbottle3pressure = airbottle3.pressure,//气管3压力
                    airbottle3temperature = airbottle3.temperature,//气管3温度

                    dery1humidity = drey1.humidity,//冷干机1 湿度
                    dery1pressure = drey1.pressure,//冷干机1 压力
                    dery1temperature = drey1.temperature,//冷干机1 温度

                    dery2humidity = drey2.humidity,//冷干机2 湿度
                    dery2pressure = drey2.pressure,//冷干机2 压力
                    dery2temperature = drey2.temperature,//冷干机2 温度

                    head3humidity = head3.humidity,//出口3湿度
                    head3pressure = head3.pressure,//出口3 压力
                    head3temperature = head3.temperature,//出口3 温度

                    head4humidity = head4.humidity,//出口4湿度
                    head4pressure = head4.pressure,//出口4 压力
                    head4temperature = head4.temperature//出口4 温度
                };
                #endregion

                //Thread.CurrentThread.Join(new TimeSpan(0, 0, 5));//阻止设定时间

                if (!string.IsNullOrEmpty(content))
                {
                    var number = Temptimespan(count);
                    if (start == true)//状态发生变化发送邮件
                    {
                        count = 1;
                        var result = com.SendEmail("hejw@lcjh.local", recipient_list, content, subject, cclist, address);
                    }
                    else if (Math.Floor((DateTime.Now - lastrecore).TotalMinutes) == number)//lastrecore未上一次发送邮件的时间,查看时间间隔是否符合条码,符合则发送邮件
                    {
                        count++;
                        var result = com.SendEmail("hejw@lcjh.local", recipient_list, content, subject, cclist, address);
                    }

                }
                Thread.CurrentThread.Join(new TimeSpan(0, 0, 5));//阻止设定时间
            }

        }

        //时间间隔获取时间
        public int Temptimespan(int count)
        {
            switch (count)//次数
            {
                case 0:
                    return 0;//时间间隔
                case 1:
                    return 3;
                case 2:
                    return 10;
                case 3:
                    return 30;
                case 4:
                    return 60;
                default:
                    return (count - 4) * 120;
            }
        }
        /// <summary>
        /// 查看压力值是否正常或异常
        /// </summary>
        /// 判断压力值是否正常,正常siwtch返true,不正常返false
        /// <param name="name">检查内容</param>
        /// <param name="pressure1">压力值</param>
        /// <param name="siwtch">状态返回,正常为true,异常为false </param>
        /// <returns></returns>
        public Dictionary<string, string> pressureTooLow(string name, double? pressure1, ref bool siwtch)
        {
            string content = "";
            string shuject = "";
            if (pressure1 > 0.5)
            {
                //1#气管出口 压力恢复
                content = "</br>" + content + name + " 压力恢复,现在压力为" + pressure1 + "Mpa</br>";
                shuject = shuject + name + " 压力恢复,现在压力为" + pressure1 + "Mpa,";
                siwtch = true;
            }
            else if (pressure1 <= 0.5)
            {
                //1#气管出口 压力恢复
                content = "</br>" + content + name + "  压力过低,现在压力为" + pressure1 + "Mpa</br>";
                shuject = shuject + name + " 压力过低,现在压力为" + pressure1 + "Mpa,";
                siwtch = false;
            }

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("content", content);
            dictionary.Add("subject", shuject);
            return dictionary;
        }

        /// <summary>
        /// 判断湿度是否正常
        /// </summary>
        /// 判断湿度值是否正常,正常siwtch返true,不正常返false
        /// <param name="name">检查内容</param>
        /// <param name="humidity1">湿度</param>
        /// <param name="siwtch">状态返回,正常为true,异常为false</param>
        /// <returns></returns>
        public Dictionary<string, string> humidityTooHight(string name, double? humidity1, ref bool siwtch)
        {
            string content = "";
            string shuject = "";
            if (humidity1 >= 50)
            {
                //1#冷干机 气管内湿度高于50%
                content = "</br>" + content + name + "气管内湿度高于50 % ,现湿度值为" + humidity1 + "%RH</br>";
                shuject = shuject + name + "气管内湿度高于50 % ,现湿度值为" + humidity1 + "%RH,";
                siwtch = false;
            }
            else if (humidity1 < 50)
            {
                //1#冷干机 气管内湿度恢复低于50%
                content = "</br>" + content + name + " 气管内湿度恢复低于50 % ,现湿度值为" + humidity1 + "%RH</br>";
                shuject = shuject + name + " 气管内湿度恢复低于50 % ,现湿度值为" + humidity1 + "%RH,";
                siwtch = true;
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("content", content);
            dictionary.Add("subject", shuject);
            return dictionary;
        }

        /// <summary>
        /// 判断压力和温度是否为0
        /// </summary>
        /// <param name="name">检查内容</param>
        /// <param name="pressure1">压力</param>
        /// <param name="temperature1">温度</param>
        /// <param name="siwtch">状态返回,正常为true,异常为false</param>
        /// <returns></returns>
        public Dictionary<string, string> IsZero(string name, double? pressure1, double? temperature1, ref bool siwtch)
        {
            string content = "";
            string shuject = "";
            if (pressure1 == 0 || temperature1 == 0)
            {
                //1#气管出口 温度或湿度或压力值为0
                string aa = pressure1 == 0 ? "压力为0" : "";
                string bb = temperature1 == 0 ? "温度为0" : "";
                content = "</br>" + content + name + aa + bb + " </br>";
                shuject = shuject + name + aa + bb + ",";
                siwtch = false;
            }
            else if (pressure1 > 0 || temperature1 > 0)
            {
                //1#气管出口 温度或湿度或压力值恢复
                string aa = pressure1 > 0 ? "压力恢复为" + pressure1 + "Mpa" : "";
                string bb = temperature1 > 0 ? "温度恢复为" + temperature1 + "℃" : "";
                content = "</br>" + content + name + aa + bb + " </br>";
                shuject = shuject + name + aa + bb + ",";
                siwtch = true;
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("content", content);
            dictionary.Add("subject", shuject);
            return dictionary;
        }

        /// <summary>
        /// 空压机状态变更检查
        /// </summary>
        /// 如果电流从小于40 变成大于40,则代表从停机到运行.反之是从运行到停机
        /// 如果从温度为0,压力为0,电流为0,变成电流大于40,表示从断电到运行,反之是从运行到断电
        /// 如果从温度为0,压力为0,电流为0,变成电流小于40,表示从断电到停机,反之是从停机到断电
        /// <param name="name">检查内容</param>
        /// <param name="current_u1">最新电流</param>
        /// <param name="temperature1">最新温度</param>
        /// <param name="pressure1">最新压力</param>
        /// <param name="current_u2">上一条信息信息电流</param>
        /// <param name="temperature2">上一条信息信息温度</param>
        /// <param name="pressure2">上一条信息信息压力</param>
        /// <returns></returns>
        public Dictionary<string, string> KYswitch(string name, double? current_u1, double? temperature1, double? pressure1, double? current_u2, double? temperature2, double? pressure2)
        {
            string content = "";
            string subject = "";
            if (current_u1 > 40 && current_u2 < 40)
            {
                //从运行到停机
                content = "</br>" + content + name + "从停机到运行 </br>";
                subject = subject + name + "从停机到运行,";
            }
            else if (current_u1 > 40 && current_u2 == 0 && temperature2 == 0 && pressure2 == 0)
            {
                //从运行到断电
                content = "</br>" + content + name + " 从断电到运行 </br>";
                subject = subject + name + " 从断电到运行 ,";
            }
            else if (current_u1 < 40 && current_u2 > 40)
            {
                //从停机到运行
                content = "</br>" + content + name + "从运行到停机</br>";
                subject = subject + name + "从运行到停机,";
            }
            else if (current_u1 < 40 && current_u2 == 0 && temperature2 == 0 && pressure2 == 0)
            {
                //从停机到断电
                content = "</br>" + content + name + "从断电到停机</br>";
                subject = subject + name + "从断电到停机,";
            }
            else if (current_u1 == 0 && temperature1 == 0 && pressure1 == 0 && current_u2 > 40)
            {
                //从断电到运行
                content = "</br>" + content + name + " 从运行到断电</br>";
                subject = subject + name + " 从运行到断电,";
            }
            else if (current_u1 == 0 && temperature1 == 0 && pressure1 == 0 && current_u2 < 40)
            {
                //从断电到停机
                content = "</br>" + content + name + "从停机到断电</br>";
                subject = subject + name + "从停机到断电,";

            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("content", content);
            dictionary.Add("subject", subject);
            return dictionary;
        }

        ///// <summary>
        ///// 判断有没有数据(暂时没用)
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="time"></param>
        ///// <param name="date1"></param>
        ///// <param name="date2"></param>
        ///// <returns></returns>
        //public Dictionary<string, string> KYdate(string name, double time, DateTime date1, DateTime date2)
        //{
        //    string content = "";
        //    string subject = "";
        //    if (time == 3)
        //    {
        //        //没有数据第一次提示
        //        content = "</br>" + content + name + "已经3分钟内没有数据</br>";
        //        subject = subject + name + "已经3分钟内没有数据,";
        //    }
        //    else if (time == 10)
        //    {
        //        //没有数据第二次提示
        //        content = "</br>" + content + name + "已经10分钟内没有数据</br>";
        //        subject = subject + name + "已经10分钟内没有数据,";
        //    }
        //    else if (time == 30)
        //    {
        //        //没有数据第三次提示
        //        content = "</br>" + content + name + "已经30分钟内没有数据</br>";
        //        subject = subject + name + "已经30分钟内没有数据,";
        //    }
        //    else if (time == 60)
        //    {
        //        //没有数据第四次提示
        //        content = "</br>" + content + name + "已经60分钟内没有数据</br>";
        //        subject = subject + name + "已经60分钟内没有数据,";
        //    }
        //    else if (time >= 120 && (time % 120) == 0)
        //    {
        //        var num = time / 120;

        //        content = "</br>" + content + name + " 已经" + num * 2 + "小时内没有数据</br>";
        //        subject = subject + name + " 已经" + num * 2 + "小时内没有数据,";
        //        //没有数据第num+4次提示
        //    }
        //    else if ((date1 - date2).TotalMinutes > 3 && time == 0)
        //    {
        //        content = "</br>" + content + name + " 数值恢复</br>";
        //        subject = subject + name + " 数值恢复,";
        //    }

        //    Dictionary<string, string> dictionary = new Dictionary<string, string>();
        //    dictionary.Add("content", content);
        //    dictionary.Add("subject", subject);
        //    return dictionary;
        //}

        #endregion

        #region 全厂温湿度监控
        public void TemperatureAndHumidity()
        {
            #region 楼层全部清单
            List<string> NameList = new List<string>();
            NameList.Add("40004518#5");
            NameList.Add("40004518#6");
            NameList.Add("40004518#3");
            NameList.Add("40004518#4");
            NameList.Add("40004518#2");
            NameList.Add("40004557#1");
            NameList.Add("40004557#2");
            NameList.Add("40004557#3");
            NameList.Add("40004557#4");
            NameList.Add("40004557#5");
            NameList.Add("40004493#1");
            NameList.Add("40004518#1");
            NameList.Add("40001676#9");
            NameList.Add("40021209#1");
            NameList.Add("40004493#2");
            NameList.Add("40021216#5");
            NameList.Add("40021216#3");
            NameList.Add("40021216#4");
            NameList.Add("40021216#2");
            NameList.Add("40021216#1");
            NameList.Add("40001676#4");
            NameList.Add("40004493#5");
            NameList.Add("40004493#7");
            NameList.Add("40004493#8");
            NameList.Add("40004493#6");
            NameList.Add("40004493#4");
            NameList.Add("40004493#3");
            NameList.Add("40021210#1");
            NameList.Add("40021210#2");
            NameList.Add("40000938#6");
            NameList.Add("40000938#7");
            NameList.Add("40000938#8");
            NameList.Add("40000938#9");
            NameList.Add("40000938#10");
            NameList.Add("40000938#11");
            NameList.Add("40000938#12");
            #endregion
            #region 楼层在范围内清单
            List<string> ScopeOfNameList = new List<string>();
            ScopeOfNameList.Add("40004518#1");
            ScopeOfNameList.Add("40001676#9");
            ScopeOfNameList.Add("40021209#1");
            ScopeOfNameList.Add("40004493#2");
            ScopeOfNameList.Add("40021216#5");
            ScopeOfNameList.Add("40021216#3");
            ScopeOfNameList.Add("40021216#4");
            ScopeOfNameList.Add("40021216#2");
            ScopeOfNameList.Add("40021216#1");
            ScopeOfNameList.Add("40001676#4");
            ScopeOfNameList.Add("40004493#5");
            ScopeOfNameList.Add("40004493#7");
            ScopeOfNameList.Add("40004493#8");
            ScopeOfNameList.Add("40004493#6");
            ScopeOfNameList.Add("40004493#4");
            ScopeOfNameList.Add("40004493#3");
            ScopeOfNameList.Add("40021210#1");
            ScopeOfNameList.Add("40021210#2");
            ScopeOfNameList.Add("40000938#6");
            ScopeOfNameList.Add("40000938#7");
            ScopeOfNameList.Add("40000938#8");
            ScopeOfNameList.Add("40000938#9");
            ScopeOfNameList.Add("40000938#10");
            ScopeOfNameList.Add("40000938#11");
            ScopeOfNameList.Add("40000938#12");
            #endregion
            List<TempTemperatureAndHumidity> oldlist = new List<TempTemperatureAndHumidity>();
            var recipient_list = new List<string>();
            recipient_list.Add("zhangdy@lcjh.local");//收件人

            while (true)
            {
                string content = "";
                string subject = "";
                kongyadbEntities kongya = new kongyadbEntities();
                List<TempTemperatureAndHumidity> newlist = new List<TempTemperatureAndHumidity>();
                List<TempTemperatureAndHumidity> ScopeOfnewlist = new List<TempTemperatureAndHumidity>();
                #region 取值
                foreach (var item in NameList)
                {
                    string[] spit = item.Split('#');
                    string DeviceID = spit[0];
                    string NodeID = spit[1];
                    var newvalue = kongya.THhistory.OrderByDescending(c => c.id).Where(c => c.DeviceID == DeviceID && c.NodeID == NodeID).Select(c => new TempTemperatureAndHumidity { id = c.id, Name = c.DeviceName, Temperature = c.Tem, Humidity = c.Hum, humistatu = true, tempstatu = true }).FirstOrDefault();
                    newlist.Add(newvalue);//找到最新的值
                }
                foreach (var item in ScopeOfNameList)
                {
                    string[] spit = item.Split('#');
                    string DeviceID = spit[0];
                    string NodeID = spit[1];
                    var newvalue = kongya.THhistory.OrderByDescending(c => c.id).Where(c => c.DeviceID == DeviceID && c.NodeID == NodeID).Select(c => new TempTemperatureAndHumidity { id = c.id, Name = c.DeviceName, Temperature = c.Tem, Humidity = c.Hum, humistatu = true, tempstatu = true }).FirstOrDefault();
                    ScopeOfnewlist.Add(newvalue);//找到最新的值
                }
                #endregion

                if (oldlist.Count != 0)//如果olderlist 不为空
                {
                    #region 判断是否有数据
                    foreach (var item in newlist)
                    {
                        var oldstatu = oldlist.Where(c => c.Name == item.Name).FirstOrDefault();

                        if (oldstatu.id == item.id)//判断上一条信息的id与最新的id是否相同,如果相同说明没有数据更新
                        {
                            content = content + item.Name + "没有数据</br>";
                            subject = subject + item.Name + "没有数据";
                        }
                        oldstatu.id = item.id;
                    }
                    #endregion
                    #region 判断数据是否正常
                    foreach (var item in ScopeOfnewlist)
                    {

                        var oldstatu = oldlist.Where(c => c.Name == item.Name).FirstOrDefault();
                        bool statu = true;
                        bool statu2 = true;
                        string value = ContrastTemp(item.Temperature, ref statu);
                        string value2 = ContrasthHum(item.Humidity, ref statu2);

                        if (statu != oldstatu.tempstatu)
                        {
                            oldstatu.tempstatu = statu;
                            content = content + item.Name + value + "</br>";
                            subject = subject + item.Name + value;
                        }
                        if (statu2 != oldstatu.humistatu)
                        {
                            oldstatu.humistatu = statu2;
                            content = content + item.Name + value2 + "</br>";
                            subject = subject + item.Name + value2;
                        }

                    }
                    #endregion
                    if (!string.IsNullOrEmpty(content))
                    {
                        var result = com.SendEmail("hejw@lcjh.local", recipient_list, content, subject, recipient_list, "");
                    }
                }
                if (oldlist.Count == 0)
                {
                    oldlist = newlist;
                }

                Thread.CurrentThread.Join(new TimeSpan(0, 2, 0));//阻止设定时间
            }
        }
        public string ContrastTemp(double? newtemp, ref bool tempbool)
        {
            string result = "";
            if (newtemp > 28)
            {
                result = result + "温度超高，现温度为" + newtemp;
                tempbool = false;
            }
            if (newtemp < 16)
            {
                result = result + "温度超低，现温度为" + newtemp;
                tempbool = false;
            }
            if (newtemp > 16 && newtemp < 28)
            {
                result = result + "温度恢复，现温度为" + newtemp;
                tempbool = true;
            }
            return result;
        }
        public string ContrasthHum(double? himu, ref bool humubool)
        {
            string result = "";
            if (himu > 70)
            {
                result = result + "湿度超高，现湿度为" + himu;
                humubool = false;
            }
            if (himu < 45)
            {
                result = result + "湿度超低，现湿度为" + himu;
                humubool = false;
            }
            if (himu > 45 && himu < 70)
            {
                result = result + "湿度恢复，现湿度为" + himu;
                humubool = true;
            }

            return result;
        }
        #endregion
    }


    #region 暂时没用，不要删
    //    //空压机1
    //                if (lastTwoair1comp[0].current_u != lastTwoair1comp[1].current_u && air1time == 0)
    //                {
    //                    if (lastTwoair1comp[0].current_u > 40 && lastTwoair1comp[1].current_u< 40)
    //                    {
    //                        //从运行到停机
    //                        content = "1#螺杆空压机SA120A 从停机到运行";
    //                    }
    //                    else if (lastTwoair1comp[0].current_u > 40 && lastTwoair1comp[1].current_u == 0 && lastTwoair1comp[1].temperature == 0 && lastTwoair1comp[1].pressure == 0)
    //                    {
    //                        //从运行到断电
    //                        content = "1#螺杆空压机SA120A 从断电到运行";
    //                    }
    //                    else if (lastTwoair1comp[0].current_u< 40 && lastTwoair1comp[1].current_u> 40)
    //                    {
    //                        //从停机到运行
    //                        content = "1#螺杆空压机SA120A 从断电到运行";
    //                    }
    //                    else if (lastTwoair1comp[0].current_u< 40 && lastTwoair1comp[1].current_u == 0 && lastTwoair1comp[1].temperature == 0 && lastTwoair1comp[1].pressure == 0)
    //                    {
    //                        //从停机到断电
    //                        content = "1#螺杆空压机SA120A 从断电到停机";
    //                    }
    //                    else if (lastTwoair1comp[0].current_u == 0 && lastTwoair1comp[0].temperature == 0 && lastTwoair1comp[0].pressure == 0 && lastTwoair1comp[1].current_u > 40)
    //                    {
    //                        //从断电到运行
    //                        content = "1#螺杆空压机SA120A 从运行到断电";
    //                    }
    //                    else if (lastTwoair1comp[0].current_u == 0 && lastTwoair1comp[0].temperature == 0 && lastTwoair1comp[0].pressure == 0 && lastTwoair1comp[1].current_u > 40)
    //                    {
    //                        //从断电到停机
    //                        content = "1#螺杆空压机SA120A 从停机到断电";

    //                    }
    //                }
    //                //空压机2
    //                if (lastTwoair2comp[0].current_u != lastTwoair2comp[1].current_u && air2time == 0)
    //                {
    //                    if (lastTwoair2comp[0].current_u > 40 && lastTwoair2comp[1].current_u< 40)
    //                    {
    //                        //从运行到停机
    //                        content = content + " 2#空气压缩机90A 从停机到运行";
    //                        subject = subject + " 2#空气压缩机90A 从停机到运行";
    //                    }
    //                    else if (lastTwoair2comp[0].current_u > 40 && lastTwoair2comp[1].current_u == 0 && lastTwoair2comp[1].temperature == 0 && lastTwoair2comp[1].pressure == 0)
    //                    {
    //                        //从运行到断电
    //                        content = content + " 2#空气压缩机90A 从断电到运行";
    //                        subject = subject + " 2#空气压缩机90A 从断电到运行";
    //                    }
    //                    else if (lastTwoair2comp[0].current_u< 40 && lastTwoair2comp[1].current_u> 40)
    //                    {
    //                        //从停机到运行
    //                        content = content + " 2#空气压缩机90A 从运行到停机";
    //                        subject = subject + " 2#空气压缩机90A 从运行到停机";
    //                    }
    //                    else if (lastTwoair2comp[0].current_u< 40 && lastTwoair2comp[1].current_u == 0 && lastTwoair2comp[1].temperature == 0 && lastTwoair2comp[1].pressure == 0)
    //                    {
    //                        //从停机到断电
    //                        content = content + " 2#空气压缩机90A 从断电到停机";
    //                        subject = subject + " 2#空气压缩机90A 从断电到停机";
    //                    }
    //                    else if (lastTwoair2comp[0].current_u == 0 && lastTwoair2comp[0].temperature == 0 && lastTwoair2comp[0].pressure == 0 && lastTwoair2comp[1].current_u > 40)
    //                    {
    //                        //从断电到运行
    //                        content = content + " 2#空气压缩机90A 从运行到断电";
    //                        subject = subject + " 2#空气压缩机90A 从运行到断电";
    //                    }
    //                    else if (lastTwoair2comp[0].current_u == 0 && lastTwoair2comp[0].temperature == 0 && lastTwoair2comp[0].pressure == 0 && lastTwoair2comp[1].current_u > 40)
    //                    {
    //                        //从断电到停机
    //                        content = content + " 2#空气压缩机90A 从停机到断电";
    //                        subject = subject + " 2#空气压缩机90A 从停机到断电";
    //                    }
    //                }
    //                //空压机3
    //                if (lastTwoair3comp[0].current_u != lastTwoair3comp[1].current_u && air3time == 0)
    //                {
    //                    if (lastTwoair3comp[0].current_u > 40 && lastTwoair3comp[1].current_u< 40)
    //                    {
    //                        //从运行到停机
    //                        content = content + " 3#变频螺杆空压机SA120A 从停机到运行";
    //                        subject = subject + " 3#变频螺杆空压机SA120A 从停机到运行";
    //                    }
    //                    else if (lastTwoair3comp[0].current_u > 40 && lastTwoair3comp[1].current_u == 0 && lastTwoair3comp[1].temperature == 0 && lastTwoair3comp[1].pressure == 0)
    //                    {
    //                        //从运行到断电
    //                        content = content + " 3#变频螺杆空压机SA120A 从断电到运行";
    //                        subject = subject + " 3#变频螺杆空压机SA120A 从断电到运行";
    //                    }
    //                    else if (lastTwoair3comp[0].current_u< 40 && lastTwoair3comp[1].current_u> 40)
    //                    {
    //                        //从停机到运行
    //                        content = content + " 3#变频螺杆空压机SA120A 从运行到停机";
    //                        subject = subject + " 3#变频螺杆空压机SA120A 从运行到停机";
    //                    }
    //                    else if (lastTwoair3comp[0].current_u< 40 && lastTwoair3comp[1].current_u == 0 && lastTwoair3comp[1].temperature == 0 && lastTwoair3comp[1].pressure == 0)
    //                    {
    //                        //从停机到断电
    //                        content = content + " 3#变频螺杆空压机SA120A 从断电到停机";
    //                        subject = subject + " 3#变频螺杆空压机SA120A 从断电到停机";
    //                    }
    //                    else if (lastTwoair3comp[0].current_u == 0 && lastTwoair3comp[0].temperature == 0 && lastTwoair3comp[0].pressure == 0 && lastTwoair3comp[1].current_u > 40)
    //                    {
    //                        //从断电到运行
    //                        content = content + " 3#变频螺杆空压机SA120A 从运行到断电";
    //                        subject = subject + " 3#变频螺杆空压机SA120A 从运行到断电";
    //                    }
    //                    else if (lastTwoair3comp[0].current_u == 0 && lastTwoair3comp[0].temperature == 0 && lastTwoair3comp[0].pressure == 0 && lastTwoair3comp[1].current_u > 40)
    //                    {
    //                        //从断电到停机
    //                        content = content + " 3#变频螺杆空压机SA120A 从停机到断电";
    //                        subject = subject + " 3#变频螺杆空压机SA120A 从停机到断电";
    //                    }
    //                }
    //                #endregion

    //                #region  2.空压数据值没有更新时通知 3.空压数据值恢复更新时通知一次
    //                //（第一次3分钟内没有数据更新通知，第二次10分钟，第三次30分钟，第四次1小时，第五次2小时）
    //                //空压机1
    //                if (lastTwoair1comp[0].current_u >= 40)
    //                {
    //                    if (air1time == 3)
    //                    {
    //                        //没有数据第一次提示
    //                        content = content + " 1#螺杆空压机SA120A已经3分钟内没有数据";
    //                        subject = subject + " 1#螺杆空压机SA120A已经3分钟内没有数据";
    //                    }
    //                    else if (air1time == 10)
    //                    {
    //                        //没有数据第二次提示
    //                        content = content + " 1#螺杆空压机SA120A已经10分钟内没有数据";
    //                        subject = subject + " 1#螺杆空压机SA120A已经10分钟内没有数据";
    //                    }
    //                    else if (air1time == 30)
    //                    {
    //                        //没有数据第三次提示
    //                        content = content + " 1#螺杆空压机SA120A已经30分钟内没有数据";
    //                        subject = subject + " 1#螺杆空压机SA120A已经30分钟内没有数据";
    //                    }
    //                    else if (air1time == 60)
    //                    {
    //                        //没有数据第四次提示
    //                        content = content + " 1#螺杆空压机SA120A已经60分钟内没有数据";
    //                        subject = subject + " 1#螺杆空压机SA120A已经60分钟内没有数据";
    //                    }
    //                    else if (air1time >= 120 && (air1time % 120) == 0)
    //                    {
    //                        var num = air1time / 120;

    //content = content + " 1#螺杆空压机SA120A 已经" + num* 2 + "小时内没有数据";
    //                        subject = subject + " 1#螺杆空压机SA120A 已经" + num* 2 + "小时内没有数据";
    //                        //没有数据第num+4次提示
    //                    }
    //                    else if ((lastTwoair1comp[0].recordingTime - lastTwoair1comp[1].recordingTime).TotalMinutes > 3 && air1time == 0)
    //                    {
    //                        content = content + " 1#螺杆空压机SA120A 数值恢复";
    //                        subject = subject + " 1#螺杆空压机SA120A 数值恢复";
    //                    }

    //                }
    //                //空压机2
    //                if (lastTwoair2comp[0].current_u >= 40)
    //                {
    //                    if (air2time == 3)
    //                    {
    //                        //没有数据第一次提示
    //                        content = content + " 2#空气压缩机90A已经 3分钟内没有数据";
    //                        subject = subject + " 2#空气压缩机90A已经 3分钟内没有数据";
    //                    }
    //                    else if (air2time == 10)
    //                    {
    //                        //没有数据第二次提示
    //                        content = content + " 2#空气压缩机90A已经 10分钟内没有数据";
    //                        subject = subject + " 2#空气压缩机90A已经 10分钟内没有数据";
    //                    }
    //                    else if (air2time == 30)
    //                    {
    //                        //没有数据第三次提示
    //                        content = content + " 2#空气压缩机90A已经 30分钟内没有数据";
    //                        subject = subject + " 2#空气压缩机90A已经 30分钟内没有数据";
    //                    }
    //                    else if (air2time == 60)
    //                    {
    //                        //没有数据第四次提示
    //                        content = content + " 2#空气压缩机90A已经 60分钟内没有数据";
    //                        subject = subject + " 2#空气压缩机90A已经 60分钟内没有数据";
    //                    }
    //                    else if (air2time >= 120 && (air2time % 120) == 0)
    //                    {
    //                        var num = air2time / 120;
    //content = content + " 2#空气压缩机90A已经 " + num* 2 + "小时内没有数据";
    //                        subject = subject + " 2#空气压缩机90A已经 " + num* 2 + "小时内没有数据";
    //                        //没有数据第num+4次提示
    //                    }
    //                    else if ((lastTwoair2comp[0].recordingTime - lastTwoair2comp[1].recordingTime).TotalMinutes > 3 && air2time == 0)
    //                    {
    //                        content = content + " 2#空气压缩机90A 数值恢复";
    //                        subject = subject + " 2#空气压缩机90A 数值恢复";
    //                        //数值恢复
    //                    }
    //                }
    //                //空压机3
    //                if (lastTwoair3comp[0].current_u >= 40)
    //                {
    //                    if (air3time == 3)
    //                    {
    //                        //没有数据第一次提示
    //                        content = content + " 3#变频螺杆空压机SA120A已经 3分钟内没有数据";
    //                        subject = subject + " 3#变频螺杆空压机SA120A已经 3分钟内没有数据";
    //                    }
    //                    else if (air3time == 10)
    //                    {
    //                        //没有数据第二次提示
    //                        content = content + " 3#变频螺杆空压机SA120A已经 10分钟内没有数据";
    //                        subject = subject + " 3#变频螺杆空压机SA120A已经 10分钟内没有数据";
    //                    }
    //                    else if (air3time == 30)
    //                    {
    //                        //没有数据第三次提示
    //                        content = content + " 3#变频螺杆空压机SA120A已经 30分钟内没有数据";
    //                        subject = subject + " 3#变频螺杆空压机SA120A已经 30分钟内没有数据";
    //                    }
    //                    else if (air3time == 60)
    //                    {
    //                        //没有数据第四次提示
    //                        content = content + " 3#变频螺杆空压机SA120A已经 60分钟内没有数据";
    //                        subject = subject + " 3#变频螺杆空压机SA120A已经 60分钟内没有数据";
    //                    }
    //                    else if (air3time >= 120 && (air3time % 120) == 0)
    //                    {
    //                        var num = air3time / 120;
    //content = content + " 3#变频螺杆空压机SA120A已经 " + num* 2 + "小时内没有数据";
    //                        subject = subject + " 3#变频螺杆空压机SA120A已经 " + num* 2 + "小时内没有数据";

    //                        //没有数据第num+4次提示
    //                    }
    //                    else if ((lastTwoair3comp[0].recordingTime - lastTwoair3comp[1].recordingTime).TotalMinutes > 3 && air3time == 0)
    //                    {
    //                        //数值恢复
    //                        content = content + " 3#变频螺杆空压机SA120A 数值恢复";
    //                        subject = subject + " 3#变频螺杆空压机SA120A 数值恢复";
    //                    }
    //                }
    #region
    /*
    if (airbottle1[0].pressure > 0.5 && airbottle1[1].pressure <= 0.5)
    {
        //1#气管出口 压力过低
        content = content + " 1#气管出口 压力恢复,现在压力为" + airbottle1[0].pressure;
        subject = subject + " 1#气管出口 压力恢复";
    }
    else if (airbottle1[0].pressure <= 0.5 && airbottle1[1].pressure > 0.5)
    {
        //1#气管出口 压力恢复
        content = content + "1#气管出口  压力过低,现在压力为" + airbottle1[0].pressure;
        subject = subject + "1#气管出口  压力过低";
    }

    if ((airbottle1[0].pressure == 0 && airbottle1[1].pressure > 0) || (airbottle1[0].temperature == 0 && airbottle1[1].temperature > 0) || (airbottle1[0].humidity == 0 && airbottle1[1].humidity > 0))
    {
        //1#气管出口 温度或湿度或压力值为0
        string aa = airbottle1[0].pressure == 0 ? "压力为0" : "";
        string bb = airbottle1[0].temperature == 0 ? "温度为0" : "";
        string cc = airbottle1[0].humidity == 0 ? "湿度为0" : "";
        content = content + "1#气管出口 " + aa + bb + cc;
        subject = subject + "1#气管出口 温度或湿度或压力值为0 ";
    }
    else if ((airbottle1[0].pressure > 0 && airbottle1[1].pressure == 0) || (airbottle1[0].temperature > 0 && airbottle1[1].temperature == 0) || (airbottle1[0].humidity > 0 && airbottle1[1].humidity == 0))
    {
        //1#气管出口 温度或湿度或压力值恢复
        string aa = airbottle1[1].pressure == 0 ? "压力恢复为" + airbottle1[0].pressure : "";
        string bb = airbottle1[1].temperature == 0 ? "压力恢复为" + airbottle1[0].temperature : "";
        string cc = airbottle1[1].humidity == 0 ? "压力恢复为" + airbottle1[0].humidity : "";
        content = content + "1#气管出口 " + aa + bb + cc;
        subject = subject + "1#气管出口 温度或湿度或压力值恢复 ";
    }
    */
    #endregion
    #region
    //if (airbottle2[0].pressure > 0.5 && airbottle2[1].pressure <= 0.5)
    //{
    //    //2#气管出口 压力过低
    //    content = content + "2#气管出口 压力恢复 ，现在压力值为" + airbottle2[0].pressure;
    //    subject = subject + "2#气管出口 压力恢复 ";
    //}
    //else if (airbottle2[0].pressure <= 0.5 && airbottle2[1].pressure > 0.5)
    //{
    //    //2#气管出口 压力恢复
    //    content = content + "2#气管出口  压力过低 ，现在压力值为" + airbottle2[0].pressure; ;
    //    subject = subject + "2#气管出口  压力过低";
    //}

    //if ((airbottle2[0].pressure == 0 && airbottle2[1].pressure > 0) || (airbottle2[0].temperature == 0 && airbottle2[1].temperature > 0) || (airbottle2[0].humidity == 0 && airbottle2[1].humidity > 0))
    //{
    //    //1#气管出口 温度或湿度或压力值为0
    //    string aa = airbottle2[0].pressure == 0 ? "压力为0" : "";
    //    string bb = airbottle2[0].temperature == 0 ? "温度为0" : "";
    //    string cc = airbottle2[0].humidity == 0 ? "湿度为0" : "";
    //    content = content + "2#气管出口 " + aa + bb + cc;
    //    subject = subject + "2#气管出口 温度或湿度或压力值为0 ";
    //}
    //else if ((airbottle2[0].pressure > 0 && airbottle2[1].pressure == 0) || (airbottle2[0].temperature > 0 && airbottle2[1].temperature == 0) || (airbottle2[0].humidity > 0 && airbottle2[1].humidity == 0))
    //{
    //    //1#气管出口 温度或湿度或压力值恢复
    //    string aa = airbottle2[1].pressure == 0 ? "压力恢复为" + airbottle2[0].pressure : "";
    //    string bb = airbottle2[1].temperature == 0 ? "压力恢复为" + airbottle2[0].temperature : "";
    //    string cc = airbottle2[1].humidity == 0 ? "压力恢复为" + airbottle2[0].humidity : "";
    //    content = content + "2#气管出口 " + aa + bb + cc;
    //    subject = subject + "2#气管出口 温度或湿度或压力值恢复 ";
    //}
    #endregion
    #region
    //if (airbottle3[0].pressure > 0.5 && airbottle3[1].pressure <= 0.5)
    //{
    //    //3#气管出口 压力过低
    //    content = content + "3#气管出口 压力恢复，现在压力值为" + airbottle3[0].pressure; 
    //    subject = subject + "3#气管出口 压力恢复 ";
    //}
    //else if (airbottle3[0].pressure <= 0.5 && airbottle3[1].pressure > 0.5)
    //{
    //    //3#气管出口 压力恢复
    //    content = content + "3#气管出口 压力过低，现在压力值为" + airbottle3[0].pressure;
    //    subject = subject + "3#气管出口 压力过低";
    //}

    //if ((airbottle3[0].pressure == 0 && airbottle3[1].pressure > 0) || (airbottle3[0].temperature == 0 && airbottle3[1].temperature > 0) || (airbottle3[0].humidity == 0 && airbottle3[1].humidity > 0))
    //{
    //    //1#气管出口 温度或湿度或压力值为0
    //    string aa = airbottle3[0].pressure == 0 ? "压力为0" : "";
    //    string bb = airbottle3[0].temperature == 0 ? "温度为0" : "";
    //    string cc = airbottle3[0].humidity == 0 ? "湿度为0" : "";
    //    content = content + "3#气管出口 " + aa + bb + cc;
    //    subject = subject + "3#气管出口 温度或湿度或压力值为0 ";
    //}
    //else if ((airbottle3[0].pressure > 0 && airbottle3[1].pressure == 0) || (airbottle3[0].temperature > 0 && airbottle3[1].temperature == 0) || (airbottle3[0].humidity > 0 && airbottle3[1].humidity == 0))
    //{
    //    //1#气管出口 温度或湿度或压力值恢复
    //    string aa = airbottle3[1].pressure == 0 ? "压力恢复为" + airbottle3[0].pressure : "";
    //    string bb = airbottle3[1].temperature == 0 ? "压力恢复为" + airbottle3[0].temperature : "";
    //    string cc = airbottle3[1].humidity == 0 ? "压力恢复为" + airbottle3[0].humidity : "";
    //    content = content + "3#气管出口 " + aa + bb + cc;
    //    subject = subject + "3#气管出口 温度或湿度或压力值恢复 ";
    //}
    #endregion
    #region
    //if (drey1[0].pressure > 0.5 && drey1[1].pressure <= 0.5)
    //{
    //    //1#冷干机 压力过低
    //    content = content + "1#冷干机 压力恢复，现在压力值为" + airbottle3[0].pressure;
    //    subject = subject + "1#冷干机 压力恢复";
    //}
    //else if (drey1[0].pressure <= 0.5 && drey1[1].pressure > 0.5)
    //{
    //    //1#冷干机 压力恢复
    //    content = content + "1#冷干机 压力过低，现在压力值为" + airbottle3[0].pressure;
    //    subject = subject + "1#冷干机 压力过低";
    //}

    //if ((drey1[0].pressure == 0 && drey1[1].pressure > 0) || (drey1[0].temperature == 0 && drey1[1].temperature > 0) || (drey1[0].humidity == 0 && drey1[1].humidity > 0))
    //{
    //    //1#气管出口 温度或湿度或压力值为0
    //    string aa = airbottle3[0].pressure == 0 ? "压力为0" : "";
    //    string bb = airbottle3[0].temperature == 0 ? "温度为0" : "";
    //    string cc = airbottle3[0].humidity == 0 ? "湿度为0" : "";
    //    content = content + "1#冷干机 温度或湿度或压力值为0 ";
    //    subject = subject + "1#冷干机 温度或湿度或压力值为0 ";
    //}
    //else if ((drey1[0].pressure > 0 && drey1[1].pressure == 0) || (drey1[0].temperature > 0 && drey1[1].temperature == 0) || (drey1[0].humidity > 0 && drey1[1].humidity == 0))
    //{
    //    //1#气管出口 温度或湿度或压力值恢复
    //    string aa = airbottle3[1].pressure == 0 ? "压力恢复为" + airbottle3[0].pressure : "";
    //    string bb = airbottle3[1].temperature == 0 ? "压力恢复为" + airbottle3[0].temperature : "";
    //    string cc = airbottle3[1].humidity == 0 ? "压力恢复为" + airbottle3[0].humidity : "";
    //    content = content + "1#冷干机 温度或湿度或压力值恢复 ";
    //    subject = subject + "1#冷干机 温度或湿度或压力值恢复 ";
    //}

    //if (drey1[0].humidity >= 50 && drey1[1].humidity < 50)
    //{
    //    //1#冷干机 气管内湿度高于50%
    //    content = content + "1#冷干机 气管内湿度高于50% ";
    //    subject = subject + "1#冷干机 气管内湿度高于50% ";
    //}
    //else if (drey1[0].humidity < 50 && drey1[1].humidity >= 50)
    //{
    //    //1#冷干机 气管内湿度恢复低于50%
    //    content = content + " 1#冷干机 气管内湿度恢复低于50%";
    //    subject = subject + " 1#冷干机 气管内湿度恢复低于50%";
    //}
    #endregion
    #region
    //if (drey2[0].pressure > 0.5 && drey2[1].pressure <= 0.5)
    //{
    //    //2#冷干机 压力过低
    //    content = content + "2#冷干机 压力恢复 ，现在压力值为" + airbottle3[0].pressure;
    //    subject = subject + "2#冷干机 压力恢复 ";
    //}
    //else if (drey2[0].pressure <= 0.5 && drey2[1].pressure > 0.5)
    //{
    //    //2#冷干机 压力恢复
    //    content = content + "2#冷干机 压力过低，现在压力值为" + airbottle3[0].pressure;
    //    subject = subject + "2#冷干机 压力过低";
    //}

    //if ((drey2[0].pressure == 0 && drey2[1].pressure > 0) || (drey2[0].temperature == 0 && drey2[1].temperature > 0) || (drey2[0].humidity == 0 && drey2[1].humidity > 0))
    //{
    //    //1#气管出口 温度或湿度或压力值为0
    //    string aa = airbottle3[0].pressure == 0 ? "压力为0" : "";
    //    string bb = airbottle3[0].temperature == 0 ? "温度为0" : "";
    //    string cc = airbottle3[0].humidity == 0 ? "湿度为0" : "";
    //    content = content + "2#冷干机 温度或湿度或压力值为0 ";
    //    subject = subject + "2#冷干机 温度或湿度或压力值为0 ";
    //}
    //else if ((drey2[0].pressure > 0 && drey2[1].pressure == 0) || (drey2[0].temperature > 0 && drey2[1].temperature == 0) || (drey2[0].humidity > 0 && drey2[1].humidity == 0))
    //{
    //    //1#气管出口 温度或湿度或压力值恢复
    //    content = content + "2#冷干机 温度或湿度或压力值恢复 ";
    //    subject = subject + "2#冷干机 温度或湿度或压力值恢复 ";
    //}

    //if (drey2[0].humidity >= 50 && drey2[1].humidity < 50)
    //{
    //    //1#冷干机 气管内湿度高于50%
    //    content = content + "2#冷干机 气管内湿度高于50% ";
    //    subject = subject + "2#冷干机 气管内湿度高于50% ";
    //}
    //else if (drey2[0].humidity < 50 && drey2[1].humidity >= 50)
    //{
    //    //1#冷干机 气管内湿度恢复低于50%
    //    content = content + " 2#冷干机 气管内湿度恢复低于50%";
    //    subject = subject + " 2#冷干机 气管内湿度恢复低于50%";
    //}
    #endregion
    #region
    //if (head3[0].pressure > 0.5 && head3[1].pressure <= 0.5)
    //{
    //    //3#气管出口 压力过低
    //    content = content + "3存管出口 压力恢复，现在压力值为" + airbottle3[0].pressure;
    //    subject = subject + "3存管出口 压力恢复";
    //}
    //else if (head3[0].pressure <= 0.5 && head3[1].pressure > 0.5)
    //{
    //    //3#气管出口 压力恢复
    //    content = content + "3存管出口 压力过低，现在压力值为" + airbottle3[0].pressure;
    //    subject = subject + "3存管出口 压力过低";
    //}

    //if ((head3[0].pressure == 0 && head3[1].pressure > 0) || (head3[0].temperature == 0 && head3[1].temperature > 0) || (head3[0].humidity == 0 && head3[1].humidity > 0))
    //{
    //    //1#气管出口 温度或湿度或压力值为0
    //    string aa = airbottle3[0].pressure == 0 ? "压力为0" : "";
    //    string bb = airbottle3[0].temperature == 0 ? "温度为0" : "";
    //    string cc = airbottle3[0].humidity == 0 ? "湿度为0" : "";
    //    content = content + "3存管出口 温度或湿度或压力值为0 ";
    //    subject = subject + "3存管出口 温度或湿度或压力值为0 ";
    //}
    //else if ((head3[0].pressure > 0 && head3[1].pressure == 0) || (head3[0].temperature > 0 && head3[1].temperature == 0) || (head3[0].humidity > 0 && head3[1].humidity == 0))
    //{
    //    //1#气管出口 温度或湿度或压力值恢复
    //    content = content + "3存管出口 温度或湿度或压力值恢复 ";
    //    subject = subject + "3存管出口 温度或湿度或压力值恢复 ";
    //}

    //if (head3[0].humidity >= 50 && head3[1].humidity < 50)
    //{
    //    //1#冷干机 气管内湿度高于50%
    //    content = content + "3存管出口 气管内湿度高于50% ";
    //    subject = subject + "3存管出口 气管内湿度高于50% ";
    //}
    //else if (head3[0].humidity < 50 && head3[1].humidity >= 50)
    //{
    //    //1#冷干机 气管内湿度恢复低于50%
    //    content = content + " 3存管出口 气管内湿度恢复低于50%";
    //    subject = subject + " 3存管出口 气管内湿度恢复低于50%";
    //}
    #endregion
    #endregion

}