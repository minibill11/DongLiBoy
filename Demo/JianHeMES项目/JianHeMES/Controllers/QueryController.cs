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
    public class QueryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public class TempAssemble
        {
            public string BoxBarCode { get; set; }
            public bool PQCCheckFinish { get; set; }
        }

        public class TempFinalQC
        {
            public string BarCodesNum { get; set; }
            public bool FQCCheckFinish { get; set; }
        }

        public class TempBurn_in_MosaicScreen
        {
            public string BarCodesNum { get; set; }
        }

        public class TempBurn_in
        {
            public string BarCodesNum { get; set; }
            public bool OQCCheckFinish { get; set; }
        }

        public class TempCalibrationRecord
        {
            public string BarCodesNum { get; set; }
            public bool Normal { get; set; }
            public DateTime? FinishCalibration { get; set; }
        }

        public class TempAppearance
        {
            public string BarCodesNum { get; set; }
            public bool OQCCheckFinish { get; set; }
            public DateTime? OQCCheckFT { get; set; }
        }
        //录入包装基本信息的订单列表
        public ActionResult GetOrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum).ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        // GET: Query
        public ActionResult barcodeInfo()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Query", act = "barcodeInfo" });
            }
            return View();
        }
        //据订单号显示模组工序信息
        public ActionResult ordernumModuleInfo()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Query", act = "ordernumModuleInfo" });
            }
            return View();
        }
        //据订单号显示模组工序信息//1是正常完成，2是有异常完成，3是未开始，4是正常进行中，5是异常进行中
        public ActionResult DisplayBarcodeListFromOrdernum(string ordernum)
        {
            JArray result = new JArray();
            var barcodeList = db.BarCodes.OrderBy(c => c.BarCodesNum).Where(c => c.OrderNum == ordernum).Select(c => c.BarCodesNum).ToList();
            #region 将数据存到临时class中，减少访问数据库的数量，
            var assemble = (from c in db.Assemble select new TempAssemble { BoxBarCode = c.BoxBarCode, PQCCheckFinish = c.PQCCheckFinish }).ToList(); ;
            var finalqc = db.FinalQC.Select(c => new TempFinalQC { BarCodesNum = c.BarCodesNum, FQCCheckFinish = c.FQCCheckFinish }).ToList();
            var mosaicscreen = db.Burn_in_MosaicScreen.Select(c => new TempBurn_in_MosaicScreen { BarCodesNum = c.BarCodesNum }).ToList();
            var burnin = db.Burn_in.Select(c => new TempBurn_in { BarCodesNum = c.BarCodesNum, OQCCheckFinish = c.OQCCheckFinish }).ToList();
            var calibrationRecord = db.CalibrationRecord.Select(c => new TempCalibrationRecord { BarCodesNum = c.BarCodesNum, FinishCalibration = c.FinishCalibration, Normal = c.Normal }).ToList();
            var appearn = db.Appearance.Select(c => new TempAppearance { BarCodesNum = c.BarCodesNum, OQCCheckFinish = c.OQCCheckFinish, OQCCheckFT = c.OQCCheckFT }).ToList();
            #endregion
            foreach (var item in barcodeList)
            {
                //1是正常完成，2有开始未完成，3是未开始 
                JObject statu = new JObject();
                statu.Add("barcode", item);
                //组装
                var ass = assemble.Where(c => c.BoxBarCode == item).Select(c => c.PQCCheckFinish).ToList();
                if (ass.Count == 0)
                {
                    statu.Add("Assemble", 3);
                }
                else
                {
                    if (ass.Contains(true))
                    {
                        if (ass.Count == 1)
                            statu.Add("Assemble", 1);
                        else
                            statu.Add("Assemble", 2);
                    }
                    else
                    {
                        if (ass.Count == 1)
                            statu.Add("Assemble", 4);
                        else
                            statu.Add("Assemble", 5);
                    }
                }
                //FQC
                var FQC = finalqc.Where(c => c.BarCodesNum == item).Select(c => c.FQCCheckFinish).ToList();
                if (FQC.Count == 0)
                {
                    statu.Add("FinalQC", 3);
                }
                else
                {
                    if (FQC.Contains(true))
                    {
                        if (FQC.Count == 1)
                            statu.Add("FinalQC", 1);
                        else
                            statu.Add("FinalQC", 2);
                    }
                    else
                    {
                        if (FQC.Count == 1)
                            statu.Add("FinalQC", 4);
                        else
                            statu.Add("FinalQC", 5);
                    }
                }
                //拼屏
                var mosicBurnin = mosaicscreen.Count(c => c.BarCodesNum == item);
                var burnIn = burnin.Where(c => c.BarCodesNum == item).Select(c => c.OQCCheckFinish).ToList();
                if (mosicBurnin == 0)
                {
                    statu.Add("Burn_in_MosaicScreen", 3);
                }
                else
                {
                    if (burnIn.Count == 0)
                        statu.Add("Burn_in_MosaicScreen", 4);
                    else
                        statu.Add("Burn_in_MosaicScreen", 1);
                }
                //老化
                if (burnIn.Count == 0)
                {
                    statu.Add("Burn_in", 3);
                }
                else
                {
                    if (burnIn.Contains(true))
                    {
                        if (burnIn.Count == 1)
                            statu.Add("Burn_in", 1);
                        else
                            statu.Add("Burn_in", 2);
                    }
                    else
                    {
                        if (burnIn.Count == 1)
                            statu.Add("Burn_in", 4);
                        else
                            statu.Add("Burn_in", 5);
                    }
                }
                //校正
                var calib = calibrationRecord.Where(c => c.BarCodesNum == item).Select(c=>c.Normal).ToList();
                if (calib.Count == 0)
                {
                    statu.Add("CalibrationRecord", 3);
                }
                else
                {
                    if (calib .Contains(true))
                    {
                        if (calib.Count == 1)
                            statu.Add("CalibrationRecord", 1);
                        else
                            statu.Add("CalibrationRecord", 2);
                    }
                    else
                    {
                        if (calib.Count == 1)
                            statu.Add("CalibrationRecord", 4);
                        else
                            statu.Add("CalibrationRecord", 5);
                    }
                }
                //电检
                var Appearance = appearn.Where(c => c.BarCodesNum == item).Select( c => c.OQCCheckFinish).ToList();
                if (Appearance.Count == 0)
                {
                    statu.Add("Appearance", 3);
                }
                else
                {
                    if (Appearance.Contains(true))
                    {
                        if (Appearance.Count == 1)
                            statu.Add("Appearance", 1);
                        else
                            statu.Add("Appearance", 2);
                    }
                    else
                    {
                        if (Appearance.Count == 1)
                            statu.Add("Appearance", 4);
                        else
                            statu.Add("Appearance", 5);
                    }
                }
                result.Add(statu);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //模组条码信息显示1是正常完成，2是有异常完成，3是未开始，4是正常进行中，5是异常进行中
        public ActionResult DisplayBarcodesModule(string barcode)
        {
            var barcodes = db.BarCodes.Where(c => c.BarCodesNum == barcode).FirstOrDefault();

            if (barcodes != null)
            {
                JObject total = new JObject();
                JObject result = new JObject();
                result.Add("ordernum", barcodes.OrderNum);
                result.Add("barcode", barcodes.BarCodesNum);
                result.Add("modulenum", barcodes.ModuleGroupNum == null ? "" : barcodes.ModuleGroupNum);
                result.Add("creator", barcodes.Creator);
                //组装
                var ass = db.Assemble.Where(c => c.BoxBarCode == barcodes.BarCodesNum).Select(c=>c.PQCCheckFinish).ToList();
                if (ass.Count() == 0)
                {
                    result.Add("pqcStatu", 3);
                    result.Add("pqcCreator", "");
                    result.Add("pqcCreatdate", "");
                    result.Add("pqcAbnormal", "");
                }
                else
                {
                    if (ass.Contains(true))
                    {
                        var finish = db.Assemble.Where(c => c.BoxBarCode == barcodes.BarCodesNum && c.PQCCheckFinish == true).FirstOrDefault();
                        if (ass.Count == 1)
                        {
                            result.Add("pqcStatu", 1);
                            result.Add("pqcCreator", finish.AssemblePQCPrincipal);
                            result.Add("pqcCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.PQCCheckFT));
                            result.Add("pqcAbnormal", "");
                        }
                        else
                        {
                            var abnormal= db.Assemble.Where(c => c.BoxBarCode == barcodes.BarCodesNum && c.PQCCheckFinish == false&&c.PQCCheckAbnormal!=null).ToList();
                            result.Add("pqcStatu", 2);
                            result.Add("pqcCreator", finish.AssemblePQCPrincipal);
                            result.Add("pqcCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.PQCCheckFT));
                            JArray jArrayAbnormal = new JArray();
                            abnormal.ForEach(c => jArrayAbnormal.Add(c.PQCCheckAbnormal));
                            result.Add("pqcAbnormal", jArrayAbnormal);
                        }
                           
                    }
                    else
                    {
                        var current = db.Assemble.Where(c => c.BoxBarCode == barcodes.BarCodesNum).FirstOrDefault();
                        if (ass.Count == 1&&current.PQCCheckAbnormal==null)
                        {
                            result.Add("pqcStatu", 4);
                            result.Add("pqcCreator", current.AssemblePrincipal);
                            result.Add("pqcCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.PQCCheckBT));
                            result.Add("pqcAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.Assemble.Where(c => c.BoxBarCode == barcodes.BarCodesNum && c.PQCCheckFinish == false&&c.PQCCheckAbnormal!=null).ToList();
                            result.Add("pqcStatu", 5);
                            result.Add("pqcCreator", current.AssemblePQCPrincipal);
                            result.Add("pqcCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.PQCCheckBT));
                            JArray jArrayAbnormal = new JArray();
                            abnormal.ForEach(c => jArrayAbnormal.Add(c.PQCCheckAbnormal));
                            result.Add("pqcAbnormal", jArrayAbnormal);
                        }
                    }
                }
                //FQC
                var fqc = db.FinalQC.Where(c => c.BarCodesNum == barcodes.BarCodesNum).Select(c=>c.FQCCheckFinish).ToList();
                if (fqc.Count() == 0)
                {
                    result.Add("fqcStatu", 3);
                    result.Add("fqcCreator", "");
                    result.Add("fqcCreatdate", "");
                    result.Add("fqcAbnormal", "");
                }
                else
                {
                    if (fqc.Contains(true))
                    {
                        var finish = db.FinalQC.Where(c => c.BarCodesNum == barcodes.BarCodesNum && c.FQCCheckFinish == true).FirstOrDefault();
                        if (fqc.Count == 1)
                        {
                            result.Add("fqcStatu", 1);
                            result.Add("fqcCreator", finish.FQCPrincipal);
                            result.Add("fqcCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.FQCCheckFT));
                            result.Add("fqcAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.FinalQC.Where(c => c.BarCodesNum == barcodes.BarCodesNum && c.FQCCheckFinish == false && c.FinalQC_FQCCheckAbnormal != null).ToList();
                            result.Add("fqcStatu", 2);
                            result.Add("fqcCreator", finish.FQCPrincipal);
                            result.Add("fqcCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.FQCCheckFT));
                            JArray jArrayAbnormal = new JArray();
                            abnormal.ForEach(c => jArrayAbnormal.Add(c.FinalQC_FQCCheckAbnormal));
                            result.Add("fqcAbnormal", jArrayAbnormal);
                        }

                    }
                    else
                    {
                        var current = db.FinalQC.Where(c => c.BarCodesNum == barcodes.BarCodesNum).FirstOrDefault();
                        if (fqc.Count == 1 && current.FinalQC_FQCCheckAbnormal == null)
                        {
                            result.Add("fqcStatu", 4);
                            result.Add("fqcCreator", current.FQCPrincipal);
                            result.Add("fqcCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.FQCCheckBT));
                            result.Add("fqcAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.FinalQC.Where(c => c.BarCodesNum == barcodes.BarCodesNum && c.FQCCheckFinish == false && c.FinalQC_FQCCheckAbnormal != null).ToList();
                            result.Add("fqcStatu", 5);
                            result.Add("fqcCreator", current.FQCPrincipal);
                            result.Add("fqcCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.FQCCheckBT));
                            JArray jArrayAbnormal = new JArray();
                            abnormal.ForEach(c => jArrayAbnormal.Add(c.FinalQC_FQCCheckAbnormal));
                            result.Add("fqcAbnormal", jArrayAbnormal);
                        }
                    }
                }
                //拼屏
                var mosaicScreen = db.Burn_in_MosaicScreen.Where(c => c.BarCodesNum == barcodes.BarCodesNum).FirstOrDefault();
                if (mosaicScreen == null)
                {
                    result.Add("mosaicScreenStatu", 3);
                    result.Add("mosaicScreenCreator", "");
                    result.Add("mosaicScreenCreatdate", "");
                }
                else
                {
                    var name = db.Personnel_Roster.Where(c => c.JobNum == mosaicScreen.OQCPrincipalNum).Select(c => c.Name).FirstOrDefault();
                    result.Add("mosaicScreenStatu", 1);
                    result.Add("mosaicScreenCreator", name);
                    result.Add("mosaicScreenCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", mosaicScreen.OQCMosaicStartTime));
                    //var count = db.Burn_in.Count(c => c.BarCodesNum == barcodes.BarCodesNum);
                    
                    //if (count == 1)
                    //{
                    //    result.Add("mosaicScreenStatu", 1);
                    //    result.Add("mosaicScreenCreator", name);
                    //    result.Add("mosaicScreenCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", mosaicScreen.OQCMosaicStartTime));
                    //}
                    //else if (count == 0)
                    //{
                    //    result.Add("mosaicScreenStatu", 4);
                    //    result.Add("mosaicScreenCreator", "");
                    //    result.Add("mosaicScreenCreatdate", "");
                    //}
                    //else
                    //{
                    //    result.Add("mosaicScreenStatu", 2);
                    //    result.Add("mosaicScreenCreator", name);
                    //    result.Add("mosaicScreenCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", mosaicScreen.OQCMosaicStartTime));
                    //}
                }
                //老化
                var burn_in = db.Burn_in.Where(c => c.BarCodesNum == barcodes.BarCodesNum).Select(c=>c.OQCCheckFinish).ToList();
                if (burn_in.Count() == 0)
                {
                    result.Add("burnInStatu", 3);
                    result.Add("burnInCreator", "");
                    result.Add("burnInCreatdate", "");
                    result.Add("burnInAbnormal", "");
                }
                else
                {
                    if (burn_in.Contains(true))
                    {
                        var finish = db.Burn_in.Where(c => c.BarCodesNum == barcodes.BarCodesNum && c.OQCCheckFinish == true).FirstOrDefault();
                        if (burn_in.Count == 1)
                        {
                            result.Add("burnInStatu", 1);
                            result.Add("burnInCreator", finish.OQCPrincipal);
                            result.Add("burnInCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.OQCCheckFT));
                            result.Add("burnInAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.Burn_in.Where(c => c.BarCodesNum == barcodes.BarCodesNum && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal != null).ToList();
                            result.Add("burnInStatu", 2);
                            result.Add("burnInCreator", finish.OQCPrincipal);
                            result.Add("burnInCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.OQCCheckFT));
                            JArray jArrayAbnormal = new JArray();
                            abnormal.ForEach(c => jArrayAbnormal.Add(c.Burn_in_OQCCheckAbnormal));
                            result.Add("burnInAbnormal", jArrayAbnormal);
                        }

                    }
                    else
                    {
                        var current = db.Burn_in.Where(c => c.BarCodesNum == barcodes.BarCodesNum).FirstOrDefault();
                        if (burn_in.Count == 1 && current.Burn_in_OQCCheckAbnormal == null)
                        {
                            result.Add("burnInStatu", 4);
                            result.Add("burnInCreator", current.OQCPrincipal);
                            result.Add("burnInCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.OQCCheckBT));
                            result.Add("burnInAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.Burn_in.Where(c => c.BarCodesNum == barcodes.BarCodesNum && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal != null).ToList();
                            result.Add("burnInStatu", 5);
                            result.Add("burnInCreator", current.OQCPrincipal);
                            result.Add("burnInCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.OQCCheckBT));
                            JArray jArrayAbnormal = new JArray();
                            abnormal.ForEach(c => jArrayAbnormal.Add(c.Burn_in_OQCCheckAbnormal));
                            result.Add("burnInAbnormal", jArrayAbnormal);
                        }
                    }
                }
                //校正
                var calibrationRecord = db.CalibrationRecord.Where(c => c.BarCodesNum == barcodes.BarCodesNum).Select(c=>c.Normal).ToList();
                if (calibrationRecord.Count() == 0)
                {
                    result.Add("calibrationRecordStatu", 3);
                    result.Add("calibrationRecordCreator", "");
                    result.Add("calibrationRecordCreatdate", "");
                    result.Add("calibrationRecordAbnormal", "");
                }
                else
                {
                    if (calibrationRecord.Contains(true))
                    {
                        var finish = db.CalibrationRecord.Where(c => c.BarCodesNum == barcodes.BarCodesNum && c.Normal == true).FirstOrDefault();
                        if (calibrationRecord.Count == 1)
                        {
                            result.Add("calibrationRecordStatu", 1);
                            result.Add("calibrationRecordCreator", finish.Operator);
                            result.Add("calibrationRecordCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.FinishCalibration));
                            result.Add("calibrationRecordAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.CalibrationRecord.Where(c => c.BarCodesNum == barcodes.BarCodesNum && c.Normal == false && c.AbnormalDescription != null).ToList();
                            result.Add("calibrationRecordStatu", 2);
                            result.Add("calibrationRecordCreator", finish.Operator);
                            result.Add("calibrationRecordCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.FinishCalibration));
                            JArray jArrayAbnormal = new JArray();
                            abnormal.ForEach(c => jArrayAbnormal.Add(c.AbnormalDescription));
                            result.Add("calibrationRecordAbnormal", jArrayAbnormal);
                        }

                    }
                    else
                    {
                        var current = db.CalibrationRecord.Where(c => c.BarCodesNum == barcodes.BarCodesNum).FirstOrDefault();
                        if (calibrationRecord.Count == 1 && current.AbnormalDescription == null)
                        {
                            result.Add("calibrationRecordStatu", 4);
                            result.Add("calibrationRecordCreator", current.Operator);
                            result.Add("calibrationRecordCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.BeginCalibration));
                            result.Add("calibrationRecordAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.CalibrationRecord.Where(c => c.BarCodesNum == barcodes.BarCodesNum && c.Normal == false && c.AbnormalDescription != null).ToList();
                            result.Add("calibrationRecordStatu", 5);
                            result.Add("calibrationRecordCreator", current.Operator);
                            result.Add("calibrationRecordCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.BeginCalibration));
                            JArray jArrayAbnormal = new JArray();
                            abnormal.ForEach(c => jArrayAbnormal.Add(c.AbnormalDescription));
                            result.Add("calibrationRecordAbnormal", jArrayAbnormal);
                        }
                    }
                }
                //电检
                var appearance = db.Appearance.Where(c => c.BarCodesNum == barcodes.BarCodesNum).Select(c=>c.OQCCheckFinish).ToList();
                if (appearance.Count() == 0)
                {
                    result.Add("appearanceStatu", 3);
                    result.Add("appearanceCreator", "");
                    result.Add("appearanceCreatdate", "");
                    result.Add("appearanceAbnormal", "");
                }
                else
                {
                    if (appearance.Contains(true))
                    {
                        var finish = db.Appearance.Where(c => c.BarCodesNum == barcodes.BarCodesNum && c.OQCCheckFinish == true).FirstOrDefault();
                        if (appearance.Count == 1)
                        {
                            result.Add("appearanceStatu", 1);
                            result.Add("appearanceCreator", finish.OQCPrincipal);
                            result.Add("appearanceCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.OQCCheckFT));
                            result.Add("appearanceAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.Appearance.Where(c => c.BarCodesNum == barcodes.BarCodesNum && c.OQCCheckFinish == false && c.Appearance_OQCCheckAbnormal != null).ToList();
                            result.Add("appearanceStatu", 2);
                            result.Add("appearanceCreator", finish.OQCPrincipal);
                            result.Add("appearanceCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.OQCCheckFT));
                            JArray jArrayAbnormal = new JArray();
                            abnormal.ForEach(c => jArrayAbnormal.Add(c.Appearance_OQCCheckAbnormal));
                            result.Add("appearanceAbnormal", jArrayAbnormal);
                        }

                    }
                    else
                    {
                        var current = db.Appearance.Where(c => c.BarCodesNum == barcodes.BarCodesNum).FirstOrDefault();
                        if (appearance.Count == 1 && current.Appearance_OQCCheckAbnormal == null)
                        {
                            result.Add("appearanceStatu", 4);
                            result.Add("appearanceCreator", current.OQCPrincipal);
                            result.Add("appearanceCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.OQCCheckBT));
                            result.Add("appearanceAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.Appearance.Where(c => c.BarCodesNum == barcodes.BarCodesNum && c.OQCCheckFinish == false && c.Appearance_OQCCheckAbnormal != null).ToList();
                            result.Add("appearanceStatu", 5);
                            result.Add("appearanceCreator", current.OQCPrincipal);
                            result.Add("appearanceCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.OQCCheckBT));
                            JArray jArrayAbnormal = new JArray();
                            abnormal.ForEach(c => jArrayAbnormal.Add(c.Appearance_OQCCheckAbnormal));
                            result.Add("appearanceAbnormal", jArrayAbnormal);
                        }
                    }
                }
                total.Add("first", result);

                var outherBarcode = db.Packing_BarCodePrinting.Where(c => c.BarCodeNum == barcode).ToList();
                if (outherBarcode.Count != 0)
                {
                    JObject result2 = new JObject();
                    result2.Add("ordernum", outherBarcode.FirstOrDefault().OrderNum);
                    result2.Add("otherBarcode", outherBarcode.FirstOrDefault().OuterBoxBarcode);
                    result2.Add("type", outherBarcode.FirstOrDefault().Type);
                    var wahouser_join = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == barcode).ToList();
                    if (wahouser_join.Count != 0)
                    {
                        var count = wahouser_join.Count(c => c.IsOut == true);
                        if (count != 0)
                        {
                            result2.Add("statu", "出库");
                            result2.Add("name", wahouser_join.FirstOrDefault().WarehouseOutOperator);
                        }
                        else
                        {
                            result2.Add("statu", "入库");
                            result2.Add("name", wahouser_join.FirstOrDefault().Operator);
                        }
                    }
                    else
                    {
                        result2.Add("statu", "未入库");
                        result2.Add("name", outherBarcode.FirstOrDefault().Operator);
                    }
                    JArray barcodejarray = new JArray();
                    barcodejarray.Add(barcode);
                    result2.Add("barcodelist", barcodejarray);
                    total.Add("two", result2);
                }
                else
                { total.Add("two", null); }

                    return Content(JsonConvert.SerializeObject(total));
            }
            return null;
        }

        //外箱条码信息显示
        public ActionResult DisplayBarcodesOuther(string barcode)
        {
            var outherBarcode = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == barcode).ToList();
            if (outherBarcode.Count != 0)
            {
                JObject result = new JObject();
                result.Add("ordernum", outherBarcode.FirstOrDefault().OrderNum);
                result.Add("otherBarcode", outherBarcode.FirstOrDefault().OuterBoxBarcode);
                result.Add("type", outherBarcode.FirstOrDefault().Type);
                var wahouser_join = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == barcode).ToList();
                if (wahouser_join.Count != 0)
                {
                    var count = wahouser_join.Count(c => c.IsOut == true);
                    if (count != 0)
                    {
                        result.Add("statu", "出库");
                        result.Add("name", wahouser_join.FirstOrDefault().WarehouseOutOperator);
                    }
                    else
                    {
                        result.Add("statu", "入库");
                        result.Add("name", wahouser_join.FirstOrDefault().Operator);
                    }
                }
                else
                {
                    result.Add("statu", "未入库");
                    result.Add("name", "");
                }
                JArray barcodejarray = new JArray();
                outherBarcode.ForEach(c => barcodejarray.Add(c.BarCodeNum));
                result.Add("barcodelist", barcodejarray);
                return Content(JsonConvert.SerializeObject(result));
            }
            return null;
        }

        //锡膏条码信息显示 1是有记录，3是没记录，没2
        public ActionResult DisplayBarcodesSulder(string barcode)
        {
            var sulderbarcode = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == barcode).FirstOrDefault();
            if (sulderbarcode != null)
            {
                JObject result = new JObject();
                result.Add("solderpasterBacrcode", sulderbarcode.SolderpasterBacrcode);
                //物料号
                result.Add("receivingNum", sulderbarcode.ReceivingNum);
                //批次
                result.Add("batch", sulderbarcode.Batch);
                //生产日期
                result.Add("leaveFactoryTime", string.Format("{0:yyyy-MM-dd hh:mm:ss}", sulderbarcode.LeaveFactoryTime));
                //供应商
                result.Add("supplier", sulderbarcode.Supplier);
                //有效期
                result.Add("effectiveDay", sulderbarcode.EffectiveDay);
                //入SMT冰柜
                var smtFreeze = db.SMT_Freezer.OrderByDescending(c => c.IntoTime).Where(c => c.SolderpasterBacrcode == barcode).Select(c => c.IntoTime).ToList();
                if (smtFreeze.Count() != 0)
                {
                    var lasttime = smtFreeze.FirstOrDefault();
                    result.Add("smtFreeze", 1);
                    var smtream = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == barcode && c.StartTime > lasttime).FirstOrDefault();
                    if (smtream != null)
                    {
                        result.Add("smtReam", 1);
                        var stir = db.SMT_Stir.Where(c => c.SolderpasterBacrcode == barcode && c.StartTime > smtream.StartTime).FirstOrDefault();
                        if (stir != null)
                        {
                            result.Add("smtStir", 1);
                            var empty = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == barcode && c.StartTime > stir.StartTime).FirstOrDefault();
                            if (empty != null)
                            {
                                result.Add("smtEmpty", 1);
                                var recly = db.SMT_Recycle.Count(c => c.SolderpasterBacrcode == barcode);
                                if (recly != 0)
                                    result.Add("smtrecly", 1);
                                else
                                    result.Add("smtrecly", 3);
                            }
                            else
                            {
                                result.Add("smtEmpty", 3);
                                result.Add("smtrecly", 3);
                            }
                        }
                        else
                        {
                            result.Add("smtStir", 3);
                            result.Add("smtEmpty", 3);
                            result.Add("smtrecly", 3);
                        }
                    }
                    else
                    {
                        result.Add("smtReam", 3);
                        result.Add("smtStir", 3);
                        result.Add("smtEmpty", 3);
                        result.Add("smtrecly", 3);
                    }
                }
                else
                {
                    result.Add("smtFreeze", 3);
                    result.Add("smtReam", 3);
                    result.Add("smtStir", 3);
                    result.Add("smtEmpty", 3);
                    result.Add("smtrecly", 3);
                }
                return Content(JsonConvert.SerializeObject(result));

            }
            return null;
        }




    }
}