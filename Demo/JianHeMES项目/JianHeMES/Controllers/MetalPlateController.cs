﻿using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class MetalPlateController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        #region---页面
        public ActionResult CreatBasicInfo()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "MetalPlate", act = "CreatBasicInfo" });
            }
            return View();
        }
        public ActionResult CreatProduction()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "MetalPlate", act = "CreatProduction" });
            }
            return View();
        }
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "MetalPlate", act = "Index" });
            }
            return View();
        }

        public ActionResult MetalPlate_Modify()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "MetalPlate", act = "MetalPlate_Modify" });
            }
            return View();
        }
        #endregion

        #region----基本信息录入
        [HttpPost]
        public ActionResult CreatBasicInfo(List<MetalPlate_BasicInfo> record)
        {
            int savecout = 0;
            string str = "";
            foreach (var item in record)
            {
                //检验记录是否存在
                if (db.MetalPlate_BasicInfo.Count(c => c.OrderNum == item.OrderNum && c.ProductionType == item.ProductionType) > 0)
                {
                    str = str == "" ? str += item.ProductionType : str += "、" + item.ProductionType;
                }
                else
                {
                    db.MetalPlate_BasicInfo.Add(item);//添加数据
                    savecout += db.SaveChanges();
                }
            }
            if (savecout == record.Count()) return Content("保存成功！");
            else return Content("生产类型为：" + str + "的记录已存在，保存失败！");
        }
        #endregion

        #region---生产信息录入
        [HttpPost]
        #region
        //public ActionResult CreatProductionInfo(string ordernum, string section, string productionType, DateTime? productionStartTime, string department, string group, Decimal normalQuantity, Decimal abnormaQuantity = 0)
        //{
        //    var basiclist = db.MetalPlate_BasicInfo.Where(k => k.OrderNum == ordernum && k.ProductionType == productionType).ToList().Count();
        //    if (basiclist > 0)//使用订单号、生产类型去基本信息表找，看看时候有此基本信息录入，没有则提示用户必须先创建基本信息
        //    {
        //        int count = 0;
        //        if (section == "图纸编程")//图纸编程第一次录入时，需保存生产开始时间    
        //        {
        //            var res = db.MetalPlate_BasicInfo.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType && c.ProductionStartTime == null).ToList();
        //            if (res.Count > 0)
        //            {
        //                res.FirstOrDefault().ProductionStartTime = productionStartTime;
        //                count = db.SaveChanges();
        //            }
        //            else count++;
        //        };
        //        //判断用户输入的数量是否大于订单的总数量
        //        var normalNum = db.MetalPlateProduction.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType&&c.Section==section).Select(c => c.NormalQuantity).ToArray().Sum();
        //        var abnormalNum = db.MetalPlateProduction.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType && c.Section == section).Select(c => c.AbnormaQuantity).ToArray().Sum();
        //        var totalnum = normalNum + abnormalNum + normalQuantity + abnormaQuantity;
        //        var record = db.MetalPlate_BasicInfo.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType).FirstOrDefault();
        //        if (totalnum > record.Quantity)
        //        {
        //            return Content("保存失败，您录入的数量大于订单总数量");
        //        }
        //        else
        //        {
        //            MetalPlateProduction list = new MetalPlateProduction();
        //            list.OrderNum = ordernum;
        //            list.Section = section;
        //            list.ProductionType = productionType;
        //            list.NormalQuantity = normalQuantity;
        //            list.AbnormaQuantity = abnormaQuantity;
        //            list.Department = department;
        //            list.Group = group;
        //            list.InputPerson = ((Users)Session["User"]).UserName;
        //            if (section == "入库")
        //            {
        //                list.InputTime = Convert.ToDateTime(productionStartTime);
        //            }
        //            else {
        //                list.InputTime = DateTime.Now;
        //            }                   
        //            db.MetalPlateProduction.Add(list);
        //            int savecout = db.SaveChanges();
        //            if (section == "入库")
        //            {
        //                //如果工段等于入库，就用订单号、生产类型去钣金生产表找出已经入库的数量之和，再判断这个总数量是否等于基本信息表的对应记录的数量，如果相等就修改基本信息表的完成状态为：True
        //                var total = db.MetalPlateProduction.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType && c.Section == section).Select(c => c.NormalQuantity).ToArray().Sum();
        //                var a = db.MetalPlateProduction.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType&& c.Section == section&& c.Section == "图纸编程").Select(c => c.AbnormaQuantity).ToArray().Sum();
        //                var b = db.MetalPlateProduction.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType && c.Section == section &&c.Section == "数冲冲压").Select(c => c.AbnormaQuantity).ToArray().Sum();
        //                var t = db.MetalPlateProduction.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType && c.Section == section && c.Section == "敲料去毛刺沉孔倒角").Select(c => c.AbnormaQuantity).ToArray().Sum();
        //                var d = db.MetalPlateProduction.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType && c.Section == section && c.Section == "焊接打磨").Select(c => c.AbnormaQuantity).ToArray().Sum();
        //                var e = db.MetalPlateProduction.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType && c.Section == section && c.Section == "外协喷涂").Select(c => c.AbnormaQuantity).ToArray().Sum();
        //                var f = db.MetalPlateProduction.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType && c.Section == section && c.Section == "装配丝印").Select(c => c.AbnormaQuantity).ToArray().Sum();
        //                var g = db.MetalPlateProduction.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType && c.Section == section && c.Section == "入库").Select(c => c.AbnormaQuantity).ToArray().Sum();

        //                //找出基本信息表的记录
        //                var rec = db.MetalPlate_BasicInfo.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType).FirstOrDefault();
        //                if ((total+a+b+d+e+f+g+t)>=rec.Quantity)
        //                {
        //                    rec.CompletionState = true;
        //                    db.SaveChanges();
        //                }
        //            };
        //            if (section == "图纸编程")
        //            {
        //                if (savecout > 0 && count > 0) return Content("保存成功！");
        //                else return Content("保存失败！");
        //            }
        //            else
        //            {
        //                if (savecout > 0) return Content("保存成功！");
        //                else return Content("保存失败！");
        //            }
        //        }
        //    }
        //    else return Content("此订单未创建基本信息记录，用户需先创建基本信息记录！");                
        //}
        #endregion

        public ActionResult CreatProductionInfo(List<MetalPlateProduction> record, string department, string group)
        {
            string order = "";
            foreach (var j in record)
            {
                var basicInfoQty = db.MetalPlate_BasicInfo.Where(c => c.OrderNum == j.OrderNum && c.ProductionType == j.ProductionType).ToList();
                //判断订单是否有基本信息录入
                if (basicInfoQty.Count() <= 0)
                {
                    if (!order.Contains(j.OrderNum))
                        order = order == "" ? order += j.OrderNum : order += "、" + j.OrderNum;
                }
            }
            //如果有订单未录入基本信息，那么传进来的所有数据都不进行保存的动作
            if (!String.IsNullOrEmpty(order))
                return Content("保存失败，订单号为：" + order + "未录入基本信息!");
            else
            {
                string str = "";
                foreach (var item in record)
                {
                    var recordList = db.MetalPlateProduction.Where(c => c.OrderNum == item.OrderNum && c.ProductionType == item.ProductionType && c.Section == item.Section).ToList();
                    var normalNum = recordList.Sum(c => c.NormalQuantity);
                    var abnormalNum = recordList.Sum(c => c.AbnormaQuantity);
                    var totalnum = normalNum + abnormalNum + item.NormalQuantity + item.AbnormaQuantity;
                    var basicInfoQty = db.MetalPlate_BasicInfo.Where(c => c.OrderNum == item.OrderNum && c.ProductionType == item.ProductionType).FirstOrDefault();
                    //判断用户输入的数量是否大于订单的总数量
                    if (totalnum > basicInfoQty.Quantity)
                    {
                        str = str == "" ? str += item.OrderNum + "，类型为:" + item.ProductionType + "，工段为:" + item.Section : str += "；" + item.OrderNum + "，类型为:" + item.ProductionType + "，工段为:" + item.Section;
                    }
                }
                int savecout = 0;
                //如果有一个工段的数量大于订单总数，那么传进来的所有数据都不进行保存的动作
                if (String.IsNullOrEmpty(str))
                {
                    foreach (var i in record)
                    {
                        MetalPlateProduction list = new MetalPlateProduction();
                        list.OrderNum = i.OrderNum;
                        list.Section = i.Section;
                        list.ProductionType = i.ProductionType;
                        list.NormalQuantity = i.NormalQuantity;
                        list.AbnormaQuantity = i.AbnormaQuantity;
                        list.Department = department;
                        list.Group = group;
                        list.InputPerson = ((Users)Session["User"]).UserName;
                        list.InputTime = DateTime.Now;
                        db.MetalPlateProduction.Add(list);
                        savecout += db.SaveChanges();
                        if (i.Section == "入库")
                        {
                            //如果工段等于入库，就用订单号、生产类型去钣金生产表找出已经入库的数量之和，再判断这个总数量是否等于基本信息表的对应记录的数量，如果相等就修改基本信息表的完成状态为：True
                            var total = db.MetalPlateProduction.Where(c => c.OrderNum == i.OrderNum && c.ProductionType == i.ProductionType && c.Section == i.Section).ToList();
                            var normalqty = total.Sum(c => c.NormalQuantity);
                            var abnormalqty = total.Sum(c => c.AbnormaQuantity);
                            //找出基本信息表的记录
                            var rec = db.MetalPlate_BasicInfo.Where(c => c.OrderNum == i.OrderNum && c.ProductionType == i.ProductionType).FirstOrDefault();
                            if ((normalqty + abnormalqty) == rec.Quantity)
                            {
                                rec.CompletionState = true;                               
                                db.SaveChanges();
                            }
                        };
                    }
                }
                else return Content("订单号为：" + str + "，录入的数量大于订单总数，记录保存失败！");
                if (savecout == record.Count()) return Content("保存成功！");
                else return Content("保存失败！");
            }
        }
        #endregion

        #region---查询
        public ActionResult getProductionInfo(string[] ordernum)
        {
            List<MetalPlate_BasicInfo> orderList = new List<MetalPlate_BasicInfo>();
            if (ordernum != null)
            {
                foreach (var item in ordernum)
                {
                    var newList = db.MetalPlate_BasicInfo.Where(c => c.OrderNum == item && c.CompletionState == true).ToList();//用户传订单号，直接查找全部订单状态未完成的
                    orderList = orderList.Concat(newList).ToList();
                }
            }
            else
            {
                orderList = db.MetalPlate_BasicInfo.Where(c => c.CompletionState == null || c.CompletionState == false).ToList();//用户不传订单号，直接查找全部订单状态未完成的
            }
            if (orderList.Count > 0)
            {
                JArray resultList = new JArray();
                foreach (var item in orderList)
                {
                    JObject basicinfo = new JObject();
                    basicinfo.Add("OrderNum", item.OrderNum);
                    basicinfo.Add("Quantity", item.Quantity);
                    basicinfo.Add("ProductionType", item.ProductionType);
                    basicinfo.Add("ProductScheduleStartTime", Convert.ToDateTime(item.ProductScheduleStartTime).ToString("yyyy-MM-dd HH:mm:ss"));
                    basicinfo.Add("DepartmentalDeliveryTime", Convert.ToDateTime(item.DepartmentalDeliveryTime).ToString("yyyy-MM-dd HH:mm:ss"));
                    basicinfo.Add("ProductionStartTime", item.ProductionStartTime == null ? null : Convert.ToDateTime(item.ProductionStartTime).ToString("yyyy-MM-dd HH:mm:ss"));

                    var normalList = db.MetalPlateProduction.Where(c => c.OrderNum == item.OrderNum && c.ProductionType == item.ProductionType);
                    var abnormalList = db.MetalPlateProduction.Where(c => c.OrderNum == item.OrderNum && c.ProductionType == item.ProductionType);

                    //工艺图纸
                    var drawNormal = normalList.Where(c => c.Section == "工艺图纸").Select(c => c.NormalQuantity).DefaultIfEmpty().Sum();
                    var drawAbnormal = abnormalList.Where(c => c.Section == "工艺图纸").Select(c => c.AbnormaQuantity).DefaultIfEmpty().Sum();
                    basicinfo.Add("DrawCompletion", Complete(item.Quantity, drawNormal + drawAbnormal) + "%" + "<br>" + "(" + (drawNormal + drawAbnormal).ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//完成率
                    basicinfo.Add("DrawPass", Complete(item.Quantity, drawNormal) + "%" + "<br>" + "(" + drawNormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//合格率
                    basicinfo.Add("DrawAbnormal", Complete(item.Quantity, drawAbnormal) + "%" + "<br>" + "(" + drawAbnormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//异常

                    //编程
                    var programmeNormal = normalList.Where(c => c.Section == "编程").Select(c => c.NormalQuantity).DefaultIfEmpty().Sum();
                    var programmeAbnormal = abnormalList.Where(c => c.Section == "编程").Select(c => c.AbnormaQuantity).DefaultIfEmpty().Sum();
                    basicinfo.Add("ProgramCompletion", Complete(item.Quantity, programmeNormal + programmeAbnormal) + "%" + "<br>" + "(" + (programmeNormal + programmeAbnormal).ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//完成率
                    basicinfo.Add("ProgramPass", Complete(item.Quantity, programmeNormal) + "%" + "<br>" + "(" + programmeNormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//合格率
                    basicinfo.Add("ProgramAbnormal", Complete(item.Quantity, programmeAbnormal) + "%" + "<br>" + "(" + programmeAbnormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//异常

                    //数冲冲压敲料去毛刺
                    var bluntNormal = normalList.Where(c => c.Section == "数冲冲压敲料去毛刺").Select(c => c.NormalQuantity).DefaultIfEmpty().Sum();
                    var bluntAbnormal = abnormalList.Where(c => c.Section == "数冲冲压敲料去毛刺").Select(c => c.AbnormaQuantity).DefaultIfEmpty().Sum();
                    basicinfo.Add("BluntCompletion", Complete(item.Quantity, bluntNormal + bluntAbnormal) + "%" + "<br>" + "(" + (bluntNormal + bluntAbnormal).ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//完成率
                    basicinfo.Add("BluntPass", Complete(item.Quantity, bluntNormal) + "%" + "<br>" + "(" + bluntNormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//合格率
                    basicinfo.Add("BluntAbnormal", Complete(item.Quantity, bluntAbnormal) + "%" + "<br>" + "(" + bluntAbnormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//异常


                    //沉孔倒角钳工
                    var chamferNormal = normalList.Where(c => c.Section == "沉孔倒角钳工").Select(c => c.NormalQuantity).DefaultIfEmpty().Sum();
                    var chamferAbnormal = abnormalList.Where(c => c.Section == "沉孔倒角钳工").Select(c => c.AbnormaQuantity).DefaultIfEmpty().Sum();
                    basicinfo.Add("ChamferCompletion", Complete(item.Quantity, chamferNormal + chamferAbnormal) + "%" + "<br>" + "(" + (chamferNormal + chamferAbnormal).ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//完成率
                    basicinfo.Add("ChamferPass", Complete(item.Quantity, chamferNormal) + "%" + "<br>" + "(" + chamferNormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//合格率
                    basicinfo.Add("ChamferAbnormal", Complete(item.Quantity, chamferAbnormal) + "%" + "<br>" + "(" + chamferAbnormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//异常


                    //折弯压铆
                    var bendNormal = normalList.Where(c => c.Section == "折弯压铆").Select(c => c.NormalQuantity).DefaultIfEmpty().Sum();
                    var bendAbnormal = abnormalList.Where(c => c.Section == "折弯压铆").Select(c => c.AbnormaQuantity).DefaultIfEmpty().Sum();
                    basicinfo.Add("BendCompletion", Complete(item.Quantity, bendNormal + bendAbnormal) + "%" + "<br>" + "(" + (bendNormal + bendAbnormal).ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//完成率
                    basicinfo.Add("BendPass", Complete(item.Quantity, bendNormal) + "%" + "<br>" + "(" + bendNormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//合格率
                    basicinfo.Add("BendAbnormal", Complete(item.Quantity, bendAbnormal) + "%" + "<br>" + "(" + bendAbnormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//异常

                    //焊接打磨
                    var weldNormal = normalList.Where(c => c.Section == "焊接打磨").Select(c => c.NormalQuantity).DefaultIfEmpty().Sum();
                    var weldAbnormal = abnormalList.Where(c => c.Section == "焊接打磨").Select(c => c.AbnormaQuantity).DefaultIfEmpty().Sum();
                    basicinfo.Add("WeldCompletion", Complete(item.Quantity, weldNormal + weldAbnormal) + "%" + "<br>" + "(" + (weldNormal + weldAbnormal).ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//完成率
                    basicinfo.Add("WeldPass", Complete(item.Quantity, weldNormal) + "%" + "<br>" + "(" + weldNormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//合格率
                    basicinfo.Add("WeldAbnormal", Complete(item.Quantity, weldAbnormal) + "%" + "<br>" + "(" + weldAbnormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//异常

                    //外协喷涂
                    var sprayNormal = normalList.Where(c => c.Section == "外协喷涂").Select(c => c.NormalQuantity).DefaultIfEmpty().Sum();
                    var sprayAbnormal = abnormalList.Where(c => c.Section == "外协喷涂").Select(c => c.AbnormaQuantity).DefaultIfEmpty().Sum();
                    basicinfo.Add("SprayCompletion", Complete(item.Quantity, sprayNormal + sprayAbnormal) + "%" + "<br>" + "(" + (sprayNormal + sprayAbnormal).ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//完成率
                    basicinfo.Add("SprayPass", Complete(item.Quantity, sprayNormal) + "%" + "<br>" + "(" + sprayNormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//合格率
                    basicinfo.Add("SprayAbnormal", Complete(item.Quantity, sprayAbnormal) + "%" + "<br>" + "(" + sprayAbnormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//异常

                    //装配丝印
                    var assembleNormal = normalList.Where(c => c.Section == "装配丝印").Select(c => c.NormalQuantity).DefaultIfEmpty().Sum();
                    var assembleAbnormal = abnormalList.Where(c => c.Section == "装配丝印").Select(c => c.AbnormaQuantity).DefaultIfEmpty().Sum();
                    basicinfo.Add("AssembleCompletion", Complete(item.Quantity, assembleNormal + assembleAbnormal) + "%" + "<br>" + "(" + (assembleNormal + assembleAbnormal).ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//完成率
                    basicinfo.Add("AssemblePass", Complete(item.Quantity, assembleNormal) + "%" + "<br>" + "(" + assembleNormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//合格率
                    basicinfo.Add("AssembleAbnormal", Complete(item.Quantity, assembleAbnormal) + "%" + "<br>" + "(" + assembleAbnormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//异常

                    //入库
                    var warehouseNum = normalList.Where(c => c.Section == "入库").ToList();
                    var warehouseNormal = warehouseNum.Select(c => c.NormalQuantity).DefaultIfEmpty().Sum();
                    var warehouseAbNormal = warehouseNum.Select(c => c.AbnormaQuantity).DefaultIfEmpty().Sum();
                    basicinfo.Add("WarehouseCompletion", Complete(item.Quantity, warehouseNormal + warehouseAbNormal) + "%" + "<br>" + "(" + (warehouseNormal + warehouseAbNormal).ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//完成率
                    basicinfo.Add("WarehousePass", Complete(item.Quantity, warehouseNormal) + "%" + "<br>" + "(" + warehouseNormal.ToString("0.##") + "/" + item.Quantity.ToString("0.##") + ")");//合格率

                    //完成时间    生产时长
                    //判断当前记录的入库总数量是否与基本信息表对应的订单数量相等，相等则输出完成时间，
                    if ((warehouseNormal + warehouseAbNormal) == orderList.Where(c => c.OrderNum == item.OrderNum && c.ProductionType == item.ProductionType).FirstOrDefault().Quantity)
                    {
                        var compleTime = normalList.Where(c => c.Section == "入库").Max(c => c.InputTime);
                        basicinfo.Add("CompletionTime", Convert.ToDateTime(compleTime).ToString("yyyy-MM-dd HH:mm:ss"));//完成时间
                        DateTime startTime = Convert.ToDateTime(orderList.Where(c => c.OrderNum == item.OrderNum && c.ProductionType == item.ProductionType).FirstOrDefault().ProductionStartTime);
                        TimeSpan span = compleTime.Subtract(startTime); //算法是compleTime(完成时间)减去 startTime(开始时间)
                        string s = span.Days + "天" + span.Hours + "小时" + span.Minutes + "分钟";
                        basicinfo.Add("ProductionTime", s.Replace("-", ""));//生产时长
                    }
                    else
                    {
                        basicinfo.Add("CompletionTime", "");
                        basicinfo.Add("ProductionTime", "");
                    }
                    resultList.Add(basicinfo);
                }
                //先找出订单号
                return Content(JsonConvert.SerializeObject(resultList));
            }
            else return Content("找不到已完成订单对应记录！");

        }
        //计算完成率  计算公式：（正常数+异常数*100）/订单总数
        //计算合格率  计算公式：（正常数*100）/订单总数
        //计算异常    计算公式：（异常数*100）/订单总数
        public string Complete(Decimal orderQuantity, Decimal totalQuantity)
        {
            var complete = ((totalQuantity * 100) / orderQuantity).ToString("0.##");
            return complete;
        }
        #endregion

        #region--获取已完成/未完成的订单列表
        public ActionResult GetOrderList()
        {
            var orders = db.MetalPlate_BasicInfo.Where(m => m.CompletionState == true).Select(m => m.OrderNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpPost]
        public ActionResult GetOrderNumList()
        {
            var orders = db.MetalPlate_BasicInfo.Select(m => m.OrderNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region--获取工段已录入的数据
        public ActionResult GetEnteredDatat(string ordernum, string productionType, string section)
        {
            var list = db.MetalPlateProduction.Where(c => c.OrderNum == ordernum && c.ProductionType == productionType && c.Section == section).Select(c => new { c.OrderNum, c.ProductionType, c.Section, c.NormalQuantity, c.AbnormaQuantity }).ToList();
            return Content(JsonConvert.SerializeObject(list));
        }
        #endregion

        #region--检验是否创建了基本信息记录
        public ActionResult CheckBasicInfo(string ordernum, string productionType)
        {
            var list = db.MetalPlate_BasicInfo.Where(k => k.OrderNum == ordernum && k.ProductionType == productionType).ToList().Count();
            if (list > 0) return Content("已创建订单基本信息！");
            else return Content("此订单未创建基本信息记录，用户需先创建基本信息记录！");
        }
        #endregion

        #region--找出基本信息表数据
        [HttpPost]
        public ActionResult getBasicInfo(string ordernum)
        {
            List<MetalPlate_BasicInfo > result = new List<MetalPlate_BasicInfo>();
            var resultList = db.MetalPlate_BasicInfo.Where(k => k.OrderNum == ordernum).ToList();
            foreach (var item in resultList) {
                var production = db.MetalPlateProduction.Where(c => c.OrderNum == item.OrderNum && c.ProductionType == item.ProductionType).ToList();
                var timerList = production.Where(c => c.Section == "入库").ToList();//查找入库记录
                if (timerList.Count<=0)//没有入库记录 返回remark是小于，前端根据小于、大于禁用修改操作
                {
                    item.Remark = "小于";
                    result.Add(item);
                }
                else {
                    DateTime timer = timerList.FirstOrDefault().InputTime;
                    if (timerList.Count() == 1)//只有一条入库记录，就不用找最大时间
                    {
                        timer = timerList.FirstOrDefault().InputTime;
                    }
                    else
                    {
                        timer = timerList.Max(c => c.InputTime);//找出入库的最大时间
                    }
                    DateTime time = timer.AddDays(1);//入库最大时间加一天
                    if (DateTime.Now > time)
                    {//使用加了一天后的日期 和现在时间相比，若是大于现在时间则直接返回大于，否则相反
                        item.Remark = "大于";
                        result.Add(item);
                    }
                    else
                    {
                        item.Remark = "小于";
                        result.Add(item);
                    }
                }              
            }
                return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region--找出生产信息表数据
        [HttpPost]
        public ActionResult getProduction(string ordernum)
        {          
            var recod = db.MetalPlate_BasicInfo.Where(c => c.OrderNum == ordernum).Select(c => new { c.OrderNum, c.ProductionType }).ToList();
            List<MetalPlateProduction> orderList = new List<MetalPlateProduction>();
            foreach (var item in recod)
            {               
               var resultList = db.MetalPlateProduction.Where(c => c.OrderNum == item.OrderNum && c.ProductionType == item.ProductionType).OrderBy(c => c.ProductionType).ThenBy(c => c.Section).ToList();
               var production = resultList.Where(k=>k.Section == "入库").ToList();//查找入库记录
                if (production.Count() <= 0)//没有入库记录 返回remark是小于，前端根据小于、大于禁用修改操作
                {
                    foreach (var j in resultList)
                    {
                        j.Remark = "小于";
                        orderList.Add(j);
                    }
                }
                else {
                    DateTime timer = production.FirstOrDefault().InputTime;
                    if (production.Count() == 1)//只有一条入库记录，就不用找最大时间
                    {
                        timer = production.FirstOrDefault().InputTime;
                    }
                    else
                    {
                        timer = production.Max(c => c.InputTime);//找出入库的最大时间
                    }
                    DateTime time = timer.AddDays(1);//入库最大时间加一天
                    foreach (var j in resultList)
                    {
                        if (DateTime.Now > time)
                        {//使用加了一天后的日期 和现在时间相比，若是大于现在时间则直接返回大于，否则相反
                            j.Remark = "大于";
                            orderList.Add(j);
                        }
                        else
                        {
                            j.Remark = "小于";
                            orderList.Add(j);
                        }
                    }
                }               
            }
            return Content(JsonConvert.SerializeObject(orderList));
        }
        #endregion

        #region--修改基本信息
        public ActionResult MetalPlateBasicInfo_modify(MetalPlate_BasicInfo newList)
        {
            DbEntityEntry<MetalPlate_BasicInfo> entry = db.Entry(newList);
            entry.State = System.Data.Entity.EntityState.Modified;
            int count = db.SaveChanges();
            if (count > 0) return Content("修改成功！");
            else return Content("修改失败！");
        }
        #endregion

        #region--修改生产数据信息
        public ActionResult MetalPlateProduction_modify(MetalPlateProduction newList)
        {
            //使用订单号，工段、生产类型找出已有数量
            var checkNumNo = db.MetalPlateProduction.Where(c => c.OrderNum == newList.OrderNum && c.Section == newList.Section && c.ProductionType == newList.ProductionType && c.Id != newList.Id).Select(c => c.NormalQuantity).ToArray().Sum();
            var checkNumabNo = db.MetalPlateProduction.Where(c => c.OrderNum == newList.OrderNum && c.Section == newList.Section && c.ProductionType == newList.ProductionType && c.Id != newList.Id).Select(c => c.AbnormaQuantity).ToArray().Sum();
            var record = db.MetalPlate_BasicInfo.Where(c => c.OrderNum == newList.OrderNum && c.ProductionType == newList.ProductionType).FirstOrDefault();
            var total = checkNumNo + checkNumabNo + newList.AbnormaQuantity + newList.NormalQuantity;
            if (total > record.Quantity)
            {
                return Content("修改失败,录入数量大于订单总数！");
            }
            else
            {
                DbEntityEntry<MetalPlateProduction> entry = db.Entry(newList);
                entry.State = System.Data.Entity.EntityState.Modified;
                int count = db.SaveChanges();
                if (count > 0) return Content("修改成功！");
                else return Content("修改失败！");
            }
        }
        #endregion

        #region--删除生产数据
        public ActionResult delteProduction(MetalPlateProduction record)
        {
            var list = db.MetalPlate_BasicInfo.Where(c => c.OrderNum == record.OrderNum && c.ProductionType == record.ProductionType && c.CompletionState == true).ToList().Count();
            if (list > 0)//大于零表示此订单已完成 不允许删除
            {
                return Content("此订单已完成入库，不允许删除数据");
            }
            else
            {
                var removeList = db.MetalPlateProduction.Where(c => c.Id == record.Id).ToList();
                db.MetalPlateProduction.RemoveRange(removeList);
                int count = db.SaveChanges();
                if (count > 0) return Content("删除成功！");
                else return Content("删除失败！");
            }
        }

        #endregion

        #region--删除基本信息
        public ActionResult delteBasicInfo(MetalPlate_BasicInfo record)
        {
            //1.根据id判断该记录的订单状态是否已完成  
            //2.已完成 不允许删除   
            //3.未完成：使用订单号，生产类型到数据表找记录 如果有此订单、类型对应的记录存在  提示用户先删除数据的数据 在删除基本信息表的数据
            var basicList = db.MetalPlate_BasicInfo.Where(c => c.OrderNum == record.OrderNum && c.ProductionType == record.ProductionType && c.CompletionState == true).ToList().Count();
            if (basicList > 0)
            {
                return Content("此订单已完成入库，不允许删除数据");
            }
            else
            {
                var productionList = db.MetalPlateProduction.Where(c => c.OrderNum == record.OrderNum && c.ProductionType == record.ProductionType).ToList().Count();
                if (productionList > 0)
                {
                    return Content("此订单的生产数据未删除,需先删除生产数据,在删除基本数据！");
                }
                else
                {
                    var removeList = db.MetalPlate_BasicInfo.Where(c => c.Id == record.Id).ToList();
                    db.MetalPlate_BasicInfo.RemoveRange(removeList);
                    int count = db.SaveChanges();
                    if (count > 0) return Content("删除成功！");
                    else return Content("删除失败！");
                }
            }
        }
        #endregion

        #region--工序名称
        public ActionResult SectionName()
        {
            JArray result = new JArray();
            string[] strArray = new string[] { "工艺图纸", "编程", "数冲冲压敲料去毛刺", "沉孔倒角钳工", "折弯压铆", "焊接打磨", "外协喷涂", "装配丝印", "入库" };
            foreach (var item in strArray)
            {
                JObject List = new JObject();
                List.Add("value", item);
                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion
    }
}
