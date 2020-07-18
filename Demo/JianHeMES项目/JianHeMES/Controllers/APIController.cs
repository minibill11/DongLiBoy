﻿using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Http;

namespace JianHeMES.Controllers
{
    public class APIController : ApiController
    {
        // GET api/<controller>
        private ApplicationDbContext db = new ApplicationDbContext();
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }


        #region 校正API接口
        /// <summary>
        /// 校正API接口
        /// </summary>
        /// 根据传过来的parameter,查看parameter含有什么字段,根据字段的不同调用不同的逻辑
        /// 含有checking,先判断是否有用户信息,在判断有没有校正权限,如果过来的BarcodeNumber为空,代表只检测登录状态.如果BarcodeNumber不为空,则要继续检查条码与订单是否相符,条码是否重复校正.
        /// 含有Calibration_result,先判断用户信息是否正确,,再判断有没有校正权限,根据条码查找校正表,如果校正表有已开始未完成的记录,则修改记录,补全信息.如果校正表有完成的记录,则新增信息,记为重复信息.如果校正表没有信息,则新增信息.然后再修改条码和外观电检的模组号(如果外观电检有信息的话)
        /// <param name="parameter">json格式参数</param>
        /// <returns></returns>
        public string CalibrationApi(string parameter)
        {
            var prepare = JsonConvert.DeserializeObject<JObject>(parameter);//字符串转成json
            //var prepare = parameter;//字符串转成json
            UserOperateLog log = new UserOperateLog();
            log.Operator = "校正API";
            log.OperateDT = DateTime.Now;
            log.OperateRecord = "传入参数" + parameter;

            JObject result = new JObject();
            if (prepare.Property("checking") != null)//判断是否含有checking
            {
                try
                {
                    JObject item = new JObject();
                    var content = (JObject)prepare["checking"];//读取数据
                    var name = content["UserName"].ToString();//用户名
                    var pass = content["pwd"].ToString();///密码
                    var userid = Convert.ToInt32(content["UserID"].ToString());//工号
                    var barcode = content["BarcodeNumber"].ToString();//条码号
                    var duleGroupnum = content["ModuleGroupNumber"].ToString();//模组号
                    var ordernum = content["Ordernum"].ToString();//订单号
                    string msg = "成功";
                    if (db.Users.Count(c => c.UserName == name && c.UserNum == userid && c.PassWord == pass) == 0)//判断用户信息是否正确
                    {
                        item.Add("UserComfirm", false);
                        msg = "找不到此用户信息！";
                    }
                    else
                    {
                        if (db.Useroles.Count(c => c.UserName == name && c.UserID == userid && c.RolesName == "校正管理") != 0)//判断是否有校正权限
                        {
                            if (db.Useroles.Where(c => c.UserName == name && c.UserID == userid && c.RolesName == "校正管理").Select(c => c.Roles).FirstOrDefault().Split(',').Contains("1"))
                                item.Add("UserComfirm", true);
                            else
                            {
                                item.Add("UserComfirm", false);
                                msg = "此用户没有权限！";
                            }
                        }
                        else
                        {
                            item.Add("UserComfirm", false);
                            msg = "此用户没有权限！";
                        }
                    }
                    if (barcode == "" || ordernum == "")//判断条码号是否为空,为空表示只判断登录信息
                    {
                        item.Add("BarcodeNumberChecked", false);
                        item.Add("ReCalibration", false);
                    }
                    else
                    {
                        if (db.BarCodes.Count(c => c.BarCodesNum == barcode) == 0 || db.BarCodes.Where(c => c.BarCodesNum == barcode).Select(c => c.OrderNum).FirstOrDefault() != ordernum)//判断订单与条码是否相符
                        {
                            item.Add("BarcodeNumberChecked", false);
                            msg = "找不到条码，请确认条码与订单信息的准确";
                        }
                        else
                        {
                            item.Add("BarcodeNumberChecked", true);
                        }
                        if (db.CalibrationRecord.Count(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode) && c.Normal == true) == 0)//判断是否有重复信息
                        {
                            item.Add("ReCalibration", false);
                        }
                        else
                        {
                            item.Add("ReCalibration", true);
                        }
                    }
                    item.Add("msg", msg);
                    result.Add("checking_result", item);
                    log.Operator = "校正API";
                    log.OperateDT = DateTime.Now;
                    log.OperateRecord = "传入参数" + parameter + ",传出结果" + item;
                }
                catch (Exception e)
                {
                    return e.Message;
                }

            }

            //{"Calibration_result":{"UserName":"suse","Ordernum":"123","BarcodeNumberChecked":"true","BeginTime":"","EndTime":"","Calibrationresult":"false","FailueReason":"",”BarcodeNumber”:””,"ReCalibration":"true",”RecalibrationReason”:””,”ModuleGroupNumber”:””}}
            if (prepare.Property("Calibration_result") != null)
            {
                try
                {
                    var content = (JObject)prepare["Calibration_result"];//读取信息
                    var username = content["UserName"].ToString();//用户姓名
                    var ordernum = content["Ordernum"].ToString();//订单号
                    var pass = content["pwd"].ToString();//密码
                    var userid = Convert.ToInt32(content["UserID"].ToString());//工号
                    var barcodenumbercheck = Convert.ToBoolean(content["BarcodeNumberChecked"].ToString());//条码确认是否正确,后面没用到
                    var begintime = Convert.ToDateTime(content["BeginTime"].ToString());//开始时间
                    var endtime = Convert.ToDateTime(content["EndTime"].ToString());//结束时间
                    var normal = Convert.ToBoolean(content["Calibrationresult"].ToString());//校正是否正常
                    var failue = normal == true ? "正常" : content["FailueReason"].ToString();//返回结果,正常显示正常,不正常显示失败原因
                    var barcode = content["BarcodeNumber"].ToString();//条码 
                    var reCalibration = Convert.ToBoolean(content["ReCalibration"].ToString());//是否重复
                    var recalibrationReason = reCalibration == false ? null : content["ReCalibrationReason"].ToString();//如果重复的话,重复原因
                    var moduleGroupNumber = content["ModuleGroupNumber"].ToString();//模组号
                    //查看权限和用户密码是否正确
                    if (db.Users.Count(c => c.UserName == username && c.UserNum == userid && c.PassWord == pass) == 0)
                    {
                        result.Add("Calibration_Recorded", false);
                        result.Add("msg", "用户名和密码不对");
                        return JsonConvert.SerializeObject(result);
                    }
                    else
                    {
                        if (db.Useroles.Count(c => c.UserName == username && c.UserID == userid && c.RolesName == "校正管理") == 0)
                        {
                            result.Add("Calibration_Recorded", false);
                            result.Add("msg", "用户没有权限");
                            return JsonConvert.SerializeObject(result);
                        }
                        else
                        {
                            if (!db.Useroles.Where(c => c.UserName == username && c.UserID == userid && c.RolesName == "校正管理").Select(c => c.Roles).FirstOrDefault().Split(',').Contains("1"))
                            {
                                result.Add("Calibration_Recorded", false);
                                result.Add("msg", "用户没有权限");
                                return JsonConvert.SerializeObject(result);
                            }
                        }
                    }
                    //计算校正的时间间隔
                    int ctDay = 0;
                    TimeSpan timespan;
                    string TimeSpantostring = "";
                    var CT = endtime - begintime;
                    if (CT.Days > 0)//含有天数
                    {
                        ctDay = CT.Days;
                        timespan = new TimeSpan(CT.Hours, CT.Minutes, CT.Seconds);
                        TimeSpantostring = CT.Days.ToString() + "天" + CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
                    }
                    else//不含天数
                    {
                        timespan = new TimeSpan();
                        timespan = CT;
                        TimeSpantostring = CT.Minutes.ToString() + "分" + CT.Seconds.ToString() + "秒";
                    }
                    var department = db.Users.Where(c => c.UserName == username && c.UserNum == userid && c.PassWord == pass).Select(c => c.Department).FirstOrDefault();
                    var modulelist = db.CalibrationRecord.Where(c => c.OrderNum == ordernum && c.Normal == true && !string.IsNullOrEmpty(c.ModuleGroupNum) && c.BarCodesNum!=barcode && c.OldOrderNum == ordernum).Select(c => c.ModuleGroupNum).ToList();
                    //判断校正记录是否能录入,有未完成记录
                    if (db.CalibrationRecord.Count(c => c.BarCodesNum == barcode && c.FinishCalibration == null) != 0)
                    {
                        var temp = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && c.FinishCalibration == null).FirstOrDefault();//找到记录,并修改,补全信息
                        temp.FinishCalibration = endtime;
                        temp.Normal = normal;
                        temp.AbnormalDescription = failue;
                        temp.CalibrationDate = ctDay;
                        temp.CalibrationTime = timespan;
                        temp.CalibrationTimeSpan = TimeSpantostring;
                        db.Entry(temp).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (modulelist.Contains(moduleGroupNumber))//判断输入的模组号是否重复
                    {
                        var barcodeitem = db.CalibrationRecord.Where(c => c.ModuleGroupNum == moduleGroupNumber && c.OrderNum == ordernum).Select(c => c.BarCodesNum).FirstOrDefault();
                        result.Add("Calibration_Recorded", false);
                        result.Add("msg", "校正失败,模组号与条码" + barcodeitem + "重复");
                        //添加日志
                        log.Operator = "校正API";
                        log.OperateDT = DateTime.Now;
                        log.OperateRecord = "传入参数" + parameter + ",传出结果" + false + "校正失败,模组号与条码" + barcodeitem + "重复";

                        db.UserOperateLog.Add(log);
                        db.SaveChanges();
                        return JsonConvert.SerializeObject(result);
                    }
                    //已经有完成记录,自动添加重复记录
                    else
                    {
                        if (db.CalibrationRecord.Count(c => c.BarCodesNum == barcode && c.Normal == true) != 0)
                        {
                            CalibrationRecord calibration = new CalibrationRecord() { BarCodesNum = barcode, OldBarCodesNum = barcode, OrderNum = ordernum, OldOrderNum = ordernum, ModuleGroupNum = moduleGroupNumber, BeginCalibration = begintime, FinishCalibration = endtime, Normal = normal, AbnormalDescription = failue, CalibrationDate = ctDay, CalibrationTime = timespan, CalibrationTimeSpan = TimeSpantostring, Operator = username, RepetitionCalibration = true, RepetitionCalibrationCause = recalibrationReason == null ? "系统检测已有重复，自动勾选重复" : recalibrationReason, Department1 = department, Group = "调试校正1组" };//添加重复记录

                            db.CalibrationRecord.Add(calibration);
                            db.SaveChanges();
                        }
                        else
                        {
                            //添加新的校正信息
                            CalibrationRecord calibration = new CalibrationRecord() { BarCodesNum = barcode, OldBarCodesNum = barcode, OrderNum = ordernum, OldOrderNum = ordernum, ModuleGroupNum = moduleGroupNumber, BeginCalibration = begintime, FinishCalibration = endtime, Normal = normal, AbnormalDescription = failue, CalibrationDate = ctDay, CalibrationTime = timespan, CalibrationTimeSpan = TimeSpantostring, Operator = username, RepetitionCalibration = false, RepetitionCalibrationCause = null, Department1 = department, Group = "调试校正1组" };

                            db.CalibrationRecord.Add(calibration);
                            db.SaveChanges();
                        }
                        //修改条码和外观电检的模组号
                        var barcodecount = db.BarCodes.Where(c => c.BarCodesNum == barcode).FirstOrDefault();
                        if (barcodecount != null)
                        {
                            barcodecount.ModuleGroupNum = moduleGroupNumber;
                            db.Entry(barcodecount).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        var appearancecount = db.Appearance.Where(c => c.BarCodesNum == barcode && (c.Appearance_OQCCheckAbnormal == "正常" || c.Appearance_OQCCheckAbnormal == null) && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).ToList();
                        if (appearancecount.Count > 0)
                        {
                            foreach (var item in appearancecount)
                            {
                                item.ModuleGroupNum = moduleGroupNumber;
                                db.Entry(item).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        //删除json文件中调用过的模组号
                        if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json") == true)
                        {
                            var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json");
                            var json = JsonConvert.DeserializeObject<JArray>(jsonstring);//读取数据
                            var index = json.Where(c => c.ToString() == moduleGroupNumber).FirstOrDefault();
                            //var index = json.IndexOf(calibrationRecord.ModuleGroupNum);
                            if (index != null)
                            {
                                json.Remove(index);//移除模组号
                                string output2 = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
                                System.IO.File.WriteAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json", output2);//保存json文件
                            }
                        }
                    }
                    result.Add("Calibration_Recorded", true);
                    result.Add("msg", normal == true ? "校正完成" : "校正异常记录完成");
                    //添加日志
                    log.Operator = "校正API";
                    log.OperateDT = DateTime.Now;
                    log.OperateRecord = "传入参数" + parameter + ",传出结果" + true;
                }
                catch (Exception E)
                {
                    result.Add("Calibration_Recorded", false);
                    result.Add("msg", "校正失败");
                    //添加日志
                    log.Operator = "校正API";
                    log.OperateDT = DateTime.Now;
                    log.OperateRecord = "传入参数" + parameter + ",传出结果" + false + E.Message;
                }
            }
            if (prepare.Property("GetModuleGroupNumber") != null)
            {
                try
                {
                    JObject item = new JObject();
                    var content = (JObject)prepare["GetModuleGroupNumber"];//读取数据
                    var barcode = content["BarcodeNumber"].ToString();//条码 
                    var ordernum = content["Ordernum"].ToString();//订单号
                    if (System.IO.File.Exists(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json") == true)
                    {
                        var jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\OrderSequence2\" + ordernum + ".json");
                        var json = JsonConvert.DeserializeObject<JArray>(jsonstring);//读取数据
                        var modeule = json[0].ToString();
                        if (modeule == "已打印")
                        {
                            return "";
                        }
                        else
                        {
                            return modeule;
                        }

                    }
                    var moduluenum = db.BarCodes.Where(c => c.OrderNum == ordernum && c.BarCodesNum == barcode).Select(c => c.ModuleGroupNum).FirstOrDefault();
                    return moduluenum;
                }
                catch
                {
                    return "";
                }
            }
            db.UserOperateLog.Add(log);
            db.SaveChanges();
            return JsonConvert.SerializeObject(result);
        }


        #endregion
        [System.Web.Http.HttpPost]
        public JObject GetOrderNumGunter(string ordernum)
        {
            JObject date = new JObject();
            JArray result = new JArray();
            var boxs = db.OrderMgm.Where(c => c.OrderNum == ordernum).Select(c => c.Boxes).FirstOrDefault();
            //组装
            JObject item = new JObject();
            var assBeginTime = db.Assemble.Where(c => c.OrderNum == ordernum).Min(c => c.PQCCheckBT);
            //var assEndTime = db.Assemble.Where(c => c.OrderNum == ordernum).Max(c => c.PQCCheckFT);
            var finsh = db.Assemble.Where(c => c.OrderNum == ordernum && c.PQCCheckFinish == true).Select(c => c.BoxBarCode).Distinct().Count();
            item.Add("id", 1);
            item.Add("start_date", string.Format("{0:yyyy-MM-dd hh:mm:ss}", assBeginTime));
            item.Add("duration", "5");
            item.Add("text", "组装");
            item.Add("progress",Math.Round((decimal)finsh/boxs,2));
            item.Add("parent",0);
            item.Add("deadline", string.Format("{0:yyyy-MM-dd hh:mm:ss}", assBeginTime.Value.AddDays(15)));
            item.Add("actual_start", string.Format("{0:yyyy-MM-dd hh:mm:ss}", assBeginTime.Value.AddDays(1)));
            item.Add("actual_end", string.Format("{0:yyyy-MM-dd hh:mm:ss}", assBeginTime.Value.AddDays(4)));
            item.Add("open", true);
            result.Add(item);
            item = new JObject();
            var fqcBeginTime = db.FinalQC.Where(c => c.OrderNum == ordernum).Min(c => c.FQCCheckBT);
            //var fqcEndTime = db.FinalQC.Where(c => c.OrderNum == ordernum).Max(c => c.FQCCheckFT);
            var fqcfinsh = db.FinalQC.Where(c => c.OrderNum == ordernum && c.FQCCheckFinish == true).Select(c => c.BarCodesNum).Distinct().Count();

            item.Add("id", 2);
            item.Add("start_date", string.Format("{0:yyyy-MM-dd hh:mm:ss}", assBeginTime.Value.AddDays(20)));
            item.Add("duration", "5");
            item.Add("text", "FQC");
            item.Add("progress", Math.Round((decimal)fqcfinsh / boxs, 2));
            item.Add("parent", 0);
            item.Add("deadline", string.Format("{0:yyyy-MM-dd hh:mm:ss}", assBeginTime.Value.AddDays(30)));
            item.Add("actual_start", string.Format("{0:yyyy-MM-dd hh:mm:ss}", assBeginTime.Value.AddDays(21)));
            item.Add("actual_end", string.Format("{0:yyyy-MM-dd hh:mm:ss}", assBeginTime.Value.AddDays(36)));
            item.Add("open", true);
            result.Add(item);
            item = new JObject();

            date.Add("data", result);
           
            result = new JArray();
            JObject bb = new JObject();
            item.Add("id", 1);
            item.Add("source", 1);
            item.Add("target", 2);
            item.Add("type", 3);
            result.Add(item);
            bb.Add("links", result);
            date.Add("collections", bb);
            return date;

            /*fqc
            var fqcBeginTime = db.FinalQC.Where(c => c.OrderNum == ordernum).Min(c => c.FQCCheckBT);
            var fqcEndTime = db.FinalQC.Where(c => c.OrderNum == ordernum).Max(c => c.FQCCheckFT);
            var fqcfinsh = db.FinalQC.Where(c => c.OrderNum == ordernum && c.FQCCheckFinish == true).Select(c => c.BarCodesNum).Distinct().Count();
           
            item.Add("id", 2);
            item.Add("strrt_date", fqcBeginTime);
            item.Add("duration", "5");
            item.Add("text", "FQC");
            item.Add("progress", Math.Round((decimal)fqcfinsh / boxs, 2));
            item.Add("parent", 0);
            item.Add("deadline", fqcEndTime);
            item.Add("planned_start", fqcBeginTime);
            item.Add("planned_end", fqcEndTime);
            item.Add("open", true);
            result.Add(item);
            item = new JObject();

            //老化
            var burnBeginTime = db.Burn_in.Where(c => c.OrderNum == ordernum).Min(c => c.OQCCheckBT);
            var burnEndTime = db.Burn_in.Where(c => c.OrderNum == ordernum).Max(c => c.OQCCheckFT);
            var burnfinsh = db.Burn_in.Where(c => c.OrderNum == ordernum && c.OQCCheckFinish == true).Select(c => c.BarCodesNum).Distinct().Count();
           
            item.Add("id", 3);
            item.Add("strrt_date", burnBeginTime);
            item.Add("duration", "5");
            item.Add("text", "老化");
            item.Add("progress", Math.Round((decimal)burnfinsh / boxs, 2));
            item.Add("parent", 0);
            item.Add("deadline", burnEndTime);
            item.Add("planned_start", burnBeginTime);
            item.Add("planned_end", burnEndTime);
            item.Add("open", true);
            result.Add(item);
            item = new JObject();

            //校正
            var cabBeginTime = db.CalibrationRecord.Where(c => c.OrderNum == ordernum).Min(c => c.BeginCalibration);
            var cabEndTime = db.CalibrationRecord.Where(c => c.OrderNum == ordernum).Max(c => c.FinishCalibration);
            var cabfinsh = db.CalibrationRecord.Where(c => c.OrderNum == ordernum && c.Normal == true).Select(c => c.BarCodesNum).Distinct().Count();

            item.Add("id", 4);
            item.Add("strrt_date", cabBeginTime);
            item.Add("duration", "5");
            item.Add("text", "老化");
            item.Add("progress", Math.Round((decimal)cabfinsh / boxs, 2));
            item.Add("parent", 0);
            item.Add("deadline", cabEndTime);
            item.Add("planned_start", cabBeginTime);
            item.Add("planned_end", cabEndTime);
            item.Add("open", true);
            result.Add(item);
            item = new JObject();
            //外观电检
            var apperanceBeginTime = db.Appearance.Where(c => c.OrderNum == ordernum).Min(c => c.OQCCheckBT);
            var apperanceEndTime = db.Appearance.Where(c => c.OrderNum == ordernum).Max(c => c.OQCCheckFT);
            var apperancefinsh = db.Appearance.Where(c => c.OrderNum == ordernum && c.OQCCheckFinish == true).Select(c=>c.BarCodesNum).Distinct().Count();
            item.Add("id", 5);
            item.Add("strrt_date", apperanceBeginTime);
            item.Add("duration", "5");
            item.Add("text", "外观电检");
            item.Add("progress", Math.Round((decimal)apperancefinsh / boxs, 2));
            item.Add("parent", 0);
            item.Add("deadline", apperanceEndTime);
            item.Add("planned_start", apperanceBeginTime);
            item.Add("planned_end", apperanceEndTime);
            item.Add("open", true);
            result.Add(item);
            result.Add(item);
            item = new JObject();
            */


        }
    }
}