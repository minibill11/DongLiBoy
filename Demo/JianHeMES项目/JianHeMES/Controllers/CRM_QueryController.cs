﻿using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JianHeMES.Controllers
{
    public class CRM_QueryController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonController com = new CommonController();
        [HttpPost]
        public JArray OrderInfo(string parameter)
        {
            //var prepare = JsonConvert.DeserializeObject(parameter);//字符串转成json
            //建立本地存储表
            //1.按订单读本地存储记录，检查记录存储时间，如果订单号为空，则返回生产管控看板JSON文件的记录；
            //2.有订单号，如果存储时间到现在>5分钟，按订单查询（写一个独立的查询方法），本地存储一份查询结果，返回查询结果； 
            //3.有订单号，如果存储时间到现在 <5分钟，如果本地存储有记录，则返回取本地存储记录，如果没有，按订单查询，本地存储一份查询结果，返回查询结果；

            JArray total = new JArray();
            if (!String.IsNullOrEmpty(parameter))
            {
                JObject result = new JObject();

                var orderinfo = db.OrderMgm.Where(c => c.OrderNum == parameter).FirstOrDefault();
                if (orderinfo == null)
                {
                    result.Add("Ordernum", "订单号不存在(请核对订单号)或未创建(请联系PMC部)。");
                    total.Add(result);
                    return total;
                }
                var newbarcodelist = db.Warehouse_Join.Where(c => c.NewBarcode != c.CartonOrderNum && c.NewBarcode != null && c.State == "模组").Select(c => c.NewBarcode).ToList();
                var nuobarcodelist = new List<string>();
                if (newbarcodelist.Contains(parameter))
                {
                    nuobarcodelist = db.Warehouse_Join.Where(c => c.NewBarcode != c.CartonOrderNum && c.NewBarcode == parameter).Select(c => c.BarCodeNum).ToList();
                }
                var current = com.GetCurrentwarehousList(orderinfo.OrderNum);
                result.Add("Ordernum", parameter);//订单
                result.Add("Quantity", orderinfo.Boxes);//订单数量
                result.Add("PlatformType", orderinfo.PlatformType);//平台型号
                result.Add("PlanInputTime", orderinfo.PlanInputTime.ToString());//生成排期开始时间
                result.Add("PlanCompleteTime", orderinfo.PlanCompleteTime.ToString());//生产排期结束时间
                #region 实际生产开始时间

                DateTime? beginttime = new DateTime();
                beginttime = db.FinalQC.Where(c => c.OrderNum == parameter && (c.OldOrderNum == null || c.OldOrderNum == parameter)).Min(c => c.FQCCheckBT);//取出订单开始装配生产的PQCCheckBT值
                if (nuobarcodelist.Count != 0)//查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                {
                    var temp = db.FinalQC.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).Min(c => c.FQCCheckBT);//找到挪用出库的FinalQC最小开始时间
                    beginttime = beginttime == null ? temp : (temp == null ? beginttime : (beginttime > temp ? temp : beginttime));//如果beginttime和temp都不为null,谁小beginttime取谁的值,如果其中一个为null,取另一个不为null 的值,如果都为null,取null值
                }
                if (beginttime == null)
                {
                    beginttime = db.Burn_in.Where(c => c.OrderNum == parameter && (c.OldOrderNum == null || c.OldOrderNum == parameter)).Min(c => c.OQCCheckBT);//取出订单开始老化调试的OQCCheckBT值
                    if (nuobarcodelist.Count != 0)// 查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                    {
                        var temp = db.Burn_in.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).Min(c => c.OQCCheckBT);//找到挪用出库的Burn_in最小开始时间
                        beginttime = beginttime == null ? temp : (temp == null ? beginttime : (beginttime > temp ? temp : beginttime));//如果beginttime和temp都不为null,谁小beginttime取谁的值,如果其中一个为null,取另一个不为null 的值,如果都为null,取null值
                    }
                    if (beginttime == null)
                    {
                        beginttime = db.CalibrationRecord.Where(c => c.OrderNum == parameter && (c.OldOrderNum == null || c.OldOrderNum == parameter)).Min(c => c.BeginCalibration);//取出订单开始校正的BeginCalibration值
                        if (nuobarcodelist.Count != 0)// 查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                        {
                            var temp = db.CalibrationRecord.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).Min(c => c.BeginCalibration);//找到挪用出库的CalibrationRecord最小开始时间
                            beginttime = beginttime == null ? temp : (temp == null ? beginttime : (beginttime > temp ? temp : beginttime));//如果beginttime和temp都不为null,谁小beginttime取谁的值,如果其中一个为null,取另一个不为null 的值,如果都为null,取null值
                        }
                        if (beginttime == null)
                        {
                            beginttime = db.Appearance.Where(c => c.OrderNum == parameter && (c.OldOrderNum == null || c.OldOrderNum == parameter)).Min(c => c.OQCCheckBT);//取出订单开始包装电检检查的OQCCheckBT值
                            if (nuobarcodelist.Count != 0)// 查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                            {
                                var temp = db.Appearance.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).Min(c => c.OQCCheckBT);//找到挪用出库的Appearance最小开始时间
                                beginttime = beginttime == null ? temp : (temp == null ? beginttime : (beginttime > temp ? temp : beginttime));//如果beginttime和temp都不为null,谁小beginttime取谁的值,如果其中一个为null,取另一个不为null 的值,如果都为null,取null值
                            }
                        }
                    }
                }
                if (beginttime != null)
                {
                    result.Add("ActualProductionTime", beginttime.ToString());
                }
                else
                {
                    result.Add("ActualProductionTime", "未开始");
                }
                #endregion                                                                        //SMT完成率

                #region SMT完成率 和SMT 合格率

                //模块数
                var ModelNum = 0;
                var NormalNumSum = 0;
                var AbnormalNumSum = 0;
                JObject FinishRateitem = new JObject();
                JArray FinishRate = new JArray();
                JObject PassRateitem = new JObject();
                JArray PassRate = new JArray();
                var jobcontenlist = db.SMT_ProductionData.Where(c => c.OrderNum == parameter).Select(c => c.JobContent).Distinct().ToList();
                int j = 0;
                foreach (var jobconten in jobcontenlist)
                {
                    if (jobconten == "灯面" || jobconten == "IC面")
                    {
                        ModelNum = orderinfo.Models;
                    }
                    else if (jobconten.Contains("转接卡") == true)
                    {
                        ModelNum = orderinfo.AdapterCard;
                    }
                    else if (jobconten.Contains("电源") == true)
                    {
                        ModelNum = orderinfo.Powers;
                    }
                    //对应工作内容良品总数
                    NormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == orderinfo.OrderNum && c.JobContent == jobconten).Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == orderinfo.OrderNum && c.JobContent == jobconten).Sum(c => c.NormalCount);

                    //对应工作内容不良品总数
                    AbnormalNumSum = db.SMT_ProductionData.Where(c => c.OrderNum == orderinfo.OrderNum && c.JobContent == jobconten).Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == orderinfo.OrderNum && c.JobContent == jobconten).Sum(c => c.AbnormalCount);

                    //面
                    FinishRateitem.Add("Jobcontent", jobconten);
                    //总完成率分子
                    FinishRateitem.Add("FinishRateMolecule", NormalNumSum + AbnormalNumSum);
                    //总完成率分母
                    FinishRateitem.Add("FinishRateDenominator", ModelNum);
                    //总完成率
                    FinishRateitem.Add("FinishRate", ModelNum == 0 ? "" : (((decimal)(NormalNumSum + AbnormalNumSum)) / ModelNum * 100).ToString("F2") + "%");

                    //面
                    PassRateitem.Add("Jobcontent", jobconten);
                    //总合格率分子
                    PassRateitem.Add("PassRateMolecule", NormalNumSum);
                    //总合格率分母
                    PassRateitem.Add("PassRateDenominator", NormalNumSum + AbnormalNumSum);
                    //总合格率
                    PassRateitem.Add("PassRate", (AbnormalNumSum + NormalNumSum) == 0 ? "" : ((decimal)NormalNumSum / (NormalNumSum + AbnormalNumSum) * 100).ToString("F2") + "%");

                    FinishRate.Add(FinishRateitem);
                    PassRate.Add(PassRateitem);
                    FinishRateitem = new JObject();
                    PassRateitem = new JObject();
                    j++;
                }
                result.Add("SMT_Finish", FinishRate);
                result.Add("SMT_Pass", PassRate);

                #endregion

                #region FQC

                var FinalQC_Record = db.FinalQC.Where(c => c.OrderNum == orderinfo.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == orderinfo.OrderNum)).ToList();
                if (nuobarcodelist.Count != 0)//查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                {
                    var temp = db.FinalQC.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).ToList();//找到挪用出库的FinalQC的数据
                    FinalQC_Record.AddRange(temp);//将数据加入FinalQC_Record中
                }
                if (FinalQC_Record.Count > 0)
                {
                    Decimal FinalQC_Finish = FinalQC_Record.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Count();
                    if (FinalQC_Finish == 0)
                    {
                        result.Add("FinalQC_Finish_Rate", "0%");  //FQC完成率
                        result.Add("FinalQC_Pass_Rate", "0%");    //FQC合格率
                    }
                    else
                    {
                        //OrderNum.Add("FinalQC_Finish_Rate", (FinalQC_Finish / FinalQC_Record.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2") + "%");  //FQC完成率
                        result.Add("FinalQC_Finish_Rate", orderinfo.Boxes == 0 ? "--" : (FinalQC_Finish * 100 / orderinfo.Boxes).ToString("F2") + "%" + "(" + FinalQC_Finish + "/" + orderinfo.Boxes + ")");  //FQC比例
                        result.Add("FinalQC_Pass_Rate", (FinalQC_Finish / FinalQC_Record.Count() * 100).ToString("F2") + "%" + "(" + FinalQC_Finish + "/" + FinalQC_Record.Count() + ")");                                        //FQC合格率
                    }
                }
                else
                {
                    result.Add("FinalQC_Finish_Rate", "--%");  //FQC完成率
                    result.Add("FinalQC_Pass_Rate", "--%");    //FQC合格率
                }
                #endregion

                #region 校正
                var Calibration_Record = (from m in db.CalibrationRecord where m.OrderNum == orderinfo.OrderNum && m.RepetitionCalibration == false && (m.OldOrderNum == null || m.OldOrderNum == orderinfo.OrderNum) select m).ToList();//查出OrderNum的所有校正记录
                if (nuobarcodelist.Count != 0)//查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                {
                    var temp = db.CalibrationRecord.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).ToList();//找到挪用出库的CalibrationRecord的数据
                    Calibration_Record.AddRange(temp);//将数据加入Calibration_Record中
                }
                int Calibration_Record_Count = Calibration_Record.Select(m => m.BarCodesNum).Count();
                if (Calibration_Record.Count() > 0)
                {
                    //Decimal Calibration_Normal = Calibration_Record.Where(m => m.Normal == true).Count();//校正正常个数
                    Decimal Calibration_Normal = Calibration_Record.Where(m => m.Normal == true).Select(m => m.BarCodesNum).Distinct().Count();//校正正常个数
                    //计算校正完成率、合格率
                    if (Calibration_Normal == 0)
                    {
                        result.Add("Calibration_Finish_Rate", "0%");
                        result.Add("Calibration_Pass_Rate", "0%");
                    }
                    else
                    {
                        result.Add("Calibration_Finish_Rate", orderinfo.Boxes == 0 ? "--" : (Calibration_Normal / orderinfo.Boxes * 100).ToString("F2") + "%" + "(" + Calibration_Normal + "/" + orderinfo.Boxes + ")");
                        result.Add("Calibration_Pass_Rate", (Calibration_Normal / Calibration_Record_Count * 100).ToString("F2") + "%" + "(" + Calibration_Normal + "/" + Calibration_Record_Count + ")");
                    }
                }
                else
                {
                    result.Add("Calibration_Finish_Rate", "--%");
                    result.Add("Calibration_Pass_Rate", "--%");
                }
                #endregion

                #region 外观
                var Appearances_Record = (from m in db.Appearance where m.OrderNum == orderinfo.OrderNum && (m.OldOrderNum == null || m.OldOrderNum == orderinfo.OrderNum) select m).ToList();//查出OrderNum的所有外观包装记录
                if (nuobarcodelist.Count != 0)//查看是否有挪用出库,如果有,则把挪用出库的条码加入到判断中
                {
                    var temp = db.Appearance.Where(c => nuobarcodelist.Contains(c.BarCodesNum) && (c.OldBarCodesNum == null || nuobarcodelist.Contains(c.OldBarCodesNum))).ToList();//找到挪用出库的Appearance的数据
                    Appearances_Record.AddRange(temp);//将数据加入Appearances_Record中
                }
                int Appearances_Record_Count = Appearances_Record.Select(m => m.BarCodesNum).Distinct().Count();
                if (Appearances_Record.Count() > 0)
                {
                    //Decimal Appearances_Normal = Appearances_Record.Where(m => m.Appearance_OQCCheckAbnormal == "正常").Count();//外观包装正常个数
                    Decimal Appearances_Finish = Appearances_Record.Where(m => m.OQCCheckFinish == true).Count();//外观包装完成个数
                    //计算外观包装完成率、合格率
                    if (Appearances_Finish == 0)
                    {
                        result.Add("Appearances_Finish_Rate", "0%");
                        result.Add("Appearances_Pass_Rate", "0%");
                    }
                    else
                    {
                        //OrderNum.Add("Appearances_Finish_Rate", (Appearances_Finish / Appearances_Record_Count * 100).ToString("F2") + "%");
                        result.Add("Appearances_Finish_Rate", orderinfo.Boxes == 0 ? "--" : (Appearances_Finish / orderinfo.Boxes * 100).ToString("F2") + "%" + "(" + Appearances_Finish + " /" + orderinfo.Boxes + ")");
                        result.Add("Appearances_Pass_Rate", (Appearances_Finish / Appearances_Record.Count() * 100).ToString("F2") + "%" + "(" + Appearances_Finish + " /" + Appearances_Record.Count() + ")");
                    }
                }
                else
                {
                    //使用库存出库订单
                    Appearances_Record = db.Appearance.Where(c => c.ToOrderNum == orderinfo.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == orderinfo.OrderNum)).ToList();
                    if (Appearances_Record.Count() > 0)
                    {
                        Decimal Appearances_Finish = Appearances_Record.Where(m => m.OQCCheckFinish == true).Count();//外观包装完成个数
                        //计算外观包装完成率、合格率
                        if (Appearances_Finish == 0)
                        {
                            result.Add("Appearances_Finish_Rate", "0%");
                            result.Add("Appearances_Pass_Rate", "0%");
                        }
                        else
                        {
                            result.Add("Appearances_Finish_Rate", (Appearances_Finish / orderinfo.Boxes * 100).ToString("F2") + "%" + "(" + Appearances_Finish + " /" + orderinfo.Boxes + ")");
                            result.Add("Appearances_Pass_Rate", (Appearances_Finish / Appearances_Record.Count() * 100).ToString("F2") + "%" + "(" + Appearances_Finish + " /" + Appearances_Record.Count() + ")");
                        }
                    }
                    else
                    {
                        result.Add("Appearances_Finish_Rate", "--%");
                        result.Add("Appearances_Pass_Rate", "--%");
                    }
                }

                #endregion
                #region 出入库
                var joincount = current.Select(c => c.OuterBoxBarcode).Distinct().Count(); ;
                var outcount = current.Where(c => c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();

                result.Add("JoinAndOutDepot", (joincount == 0 ? "--" : joincount.ToString()) + "/" + (outcount == 0 ? "--" : outcount.ToString())); //入库/出库(件)
                #endregion
                DateTime? finishtime = new DateTime();
                if (current.Count(c => c.OrderNum == orderinfo.OrderNum && c.IsOut == true) >= orderinfo.Boxes)
                {
                    finishtime = current.Where(c => c.OrderNum == orderinfo.OrderNum && c.IsOut == true).Max(c => c.WarehouseOutDate);
                }
                else
                {
                    finishtime = null;
                }
                var totaltime = finishtime == null ? "" : (finishtime - beginttime).ToString();
                result.Add("ActualFinishTime", finishtime.ToString());
                result.Add("TotalTime", totaltime);


                total.Add(result);
            }
            else
            {
                var totaljson = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\ProductionController.json");
                var json = JsonConvert.DeserializeObject<JObject>(totaljson);
                var jsontotalitem = json.Children().Children().ToList();
                foreach (var jsonitem in jsontotalitem)
                {
                    JObject result = new JObject();

                    result.Add("Ordernum", jsonitem["OrderNum"].ToString());//订单
                    var box = jsonitem["Quantity"].ToString();
                    result.Add("Quantity", box);//订单数量
                    result.Add("PlatformType", jsonitem["PlatformType"].ToString());//平台型号
                    result.Add("PlanInputTime", jsonitem["PlanInputTime"].ToString());//生成排期开始时间
                    result.Add("PlanCompleteTime", jsonitem["PlanCompleteTime"].ToString());//生产排期结束时间
                    result.Add("ActualProductionTime", jsonitem["ActualProductionTime"].ToString());//实际生产开始时间
                                                                                                    //SMT完成率
                    var smtfinsh = ((JObject)jsonitem["extendFinishRate"]).Children().Children().ToList();
                    JArray smtfinshArray = new JArray();
                    foreach (var item in smtfinsh)
                    {
                        JObject obj = new JObject();
                        obj.Add("Jobcontent", item["jobconten"].ToString());
                        obj.Add("FinishRateMolecule", item["FinishRateMolecule"].ToString());
                        obj.Add("FinishRateDenominator", item["FinishRateDenominator"].ToString());
                        obj.Add("FinishRate", item["FinishRate"].ToString());
                        smtfinshArray.Add(obj);
                    }

                    result.Add("SMT_Finish", smtfinshArray);
                    //SNT合格率
                    var smtPass = ((JObject)jsonitem["dextendPassRate"]).Children().Children().ToList();
                    JArray smtPassArray = new JArray();
                    foreach (var item in smtPass)
                    {
                        JObject obj = new JObject();
                        obj.Add("Jobcontent", item["jobconten"].ToString());
                        obj.Add("PassRateMolecule", item["PassRateMolecule"].ToString());
                        obj.Add("PassRateDenominator", item["PassRateDenominator"].ToString());
                        obj.Add("PassRate", item["PassRate"].ToString());
                        smtPassArray.Add(obj);
                    }
                    result.Add("SMT_Pass", smtPassArray);

                    if (jsonitem["FinalQC_Finish"] != null)
                    {
                        result.Add("FinalQC_Finish_Rate", jsonitem["FinalQC_Finish_Rate"].ToString() + "(" + jsonitem["FinalQC_Finish"].ToString() + "/" + jsonitem["Quantity"].ToString() + ")");//FQC完成率

                        result.Add("FinalQC_Pass_Rate", jsonitem["FinalQC_Pass_Rate"].ToString() + "(" + jsonitem["FinalQC_Finish"].ToString() + "/" + jsonitem["FinalQC_Record_Count"].ToString() + ")");//FQC合格率
                    }
                    else
                    {
                        result.Add("FinalQC_Finish_Rate", "--%");//FQC完成率

                        result.Add("FinalQC_Pass_Rate", "--%");//FQC合格率
                    }
                    if (jsonitem["Calibration_Finish"] != null)
                    {
                        result.Add("Calibration_Finish_Rate", jsonitem["Calibration_Finish_Rate"].ToString() + "(" + jsonitem["Calibration_Finish"].ToString() + "/" + jsonitem["Quantity"].ToString() + ")");//校正完成率

                        result.Add("Calibration_Pass_Rate", jsonitem["Calibration_Pass_Rate"].ToString() + "(" + jsonitem["Calibration_Finish"].ToString() + "/" + jsonitem["Calibration_Record_Count"].ToString() + ")");//校正合格率
                    }
                    else
                    {
                        result.Add("Calibration_Finish_Rate", "--%");//校正完成率

                        result.Add("Calibration_Pass_Rate", "--%");//校正合格率
                    }
                    if (jsonitem["Appearances_Finish"] != null)
                    {
                        result.Add("Appearances_Finish_Rate", jsonitem["Appearances_Finish_Rate"].ToString() + "(" + jsonitem["Appearances_Finish"].ToString() + " /" + jsonitem["Quantity"].ToString() + ")");//外观电检完成率
                        result.Add("Appearances_Pass_Rate", jsonitem["Appearances_Pass_Rate"].ToString() + "(" + jsonitem["Appearances_Finish"].ToString() + "/" + jsonitem["Appearances_Record_Count"].ToString() + ")");//外观电检合格率
                    }
                    else
                    {
                        result.Add("Appearances_Finish_Rate", "--%");//外观电检完成率
                        result.Add("Appearances_Pass_Rate", "--%");//外观电检合格率
                    }
                    result.Add("JoinAndOutDepot", jsonitem["joinAndOutDepot"].ToString());//入库/出库(件)
                    result.Add("ActualFinishTime", jsonitem["ActualFinishTime"].ToString());//完成时间
                    result.Add("TotalTime", jsonitem["TotalTime"].ToString());//生产时间

                    total.Add(result);
                }
            }

            //string result = "MES接收到的内容是：" + parameter ;

            return total;
        }

        [HttpPost]
        [Authorize]
        public string OrderInfo2(string parameter)
        {
            //var prepare = JsonConvert.DeserializeObject(parameter);//字符串转成json

            string result = "MES接收到的内容是：" + parameter;

            return result;
        }

        // GET: api/CRM_Query
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CRM_Query/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CRM_Query
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/CRM_Query/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CRM_Query/5
        public void Delete(int id)
        {
        }
    }
}
