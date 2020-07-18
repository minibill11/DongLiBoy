﻿using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
            public string Oldbarocode { get; set; }
        }

        public class TempFinalQC
        {
            public string BarCodesNum { get; set; }
            public bool FQCCheckFinish { get; set; }
            public string Oldbarocode { get; set; }
        }

        public class TempBurn_in_MosaicScreen
        {
            public string BarCodesNum { get; set; }
            public string Oldbarocode { get; set; }
        }

        public class TempBurn_in
        {
            public string BarCodesNum { get; set; }
            public bool OQCCheckFinish { get; set; }
            public string Oldbarocode { get; set; }
        }

        public class TempCalibrationRecord
        {
            public string BarCodesNum { get; set; }
            public bool Normal { get; set; }
            public DateTime? FinishCalibration { get; set; }
            public string Oldbarocode { get; set; }
        }

        public class TempAppearance
        {
            public string BarCodesNum { get; set; }
            public bool OQCCheckFinish { get; set; }
            public DateTime? OQCCheckFT { get; set; }
            public string Oldbarocode { get; set; }
        }
        public class TempWarehouse
        {
            public string Barcode { get; set; }
            public bool IsOut { get; set; }
            public int id { get; set; }
        }
        public class TempAbnormal
        {
            public string Barcode { get; set; }
            public string Abnormal { get; set; }
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
            var barcodeList = db.BarCodes.OrderBy(c => c.BarCodesNum).Where(c => c.OrderNum == ordernum&&c.BarCodeType=="模组").Select(c => c.BarCodesNum).ToList();
            #region 将数据存到临时class中，减少访问数据库的数量，
            var assemble = (from c in db.Assemble select new TempAssemble { BoxBarCode = c.BoxBarCode, PQCCheckFinish = c.PQCCheckFinish, Oldbarocode = c.OldBarCodesNum }).ToList(); ;
            var finalqc = db.FinalQC.Select(c => new TempFinalQC { BarCodesNum = c.BarCodesNum, FQCCheckFinish = c.FQCCheckFinish, Oldbarocode = c.OldBarCodesNum }).ToList();
            var mosaicscreen = db.Burn_in_MosaicScreen.Select(c => new TempBurn_in_MosaicScreen { BarCodesNum = c.BarCodesNum, Oldbarocode = c.OldBarCodesNum }).ToList();
            var burnin = db.Burn_in.Select(c => new TempBurn_in { BarCodesNum = c.BarCodesNum, OQCCheckFinish = c.OQCCheckFinish, Oldbarocode = c.OldBarCodesNum }).ToList();
            var calibrationRecord = db.CalibrationRecord.Select(c => new TempCalibrationRecord { BarCodesNum = c.BarCodesNum, FinishCalibration = c.FinishCalibration, Normal = c.Normal, Oldbarocode = c.OldBarCodesNum }).ToList();
            var appearn = db.Appearance.Select(c => new TempAppearance { BarCodesNum = c.BarCodesNum, OQCCheckFinish = c.OQCCheckFinish, OQCCheckFT = c.OQCCheckFT, Oldbarocode = c.OldBarCodesNum }).ToList();
            var packingList = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == ordernum && c.QC_Operator == null).Select(c => c.BarCodeNum).ToList();
            var warehouse = db.Warehouse_Join.Where(c => c.CartonOrderNum == ordernum && c.State == "模组").Select(c => new TempWarehouse { Barcode = c.BarCodeNum, IsOut = c.IsOut, id = c.Id });
            #endregion
            foreach (var item in barcodeList)
            {
                //1是正常完成，2有开始未完成，3是未开始 
                JObject statu = new JObject();
                statu.Add("barcode", item);
                //组装
                var ass = assemble.Where(c => c.BoxBarCode == item && (c.Oldbarocode == item || c.Oldbarocode == null)).Select(c => c.PQCCheckFinish).ToList();
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
                var FQC = finalqc.Where(c => c.BarCodesNum == item && (c.Oldbarocode == item || c.Oldbarocode == null)).Select(c => c.FQCCheckFinish).ToList();
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
                var mosicBurnin = mosaicscreen.Count(c => c.BarCodesNum == item && (c.Oldbarocode == item || c.Oldbarocode == null));
                var burnIn = burnin.Where(c => c.BarCodesNum == item && (c.Oldbarocode == item || c.Oldbarocode == null)).Select(c => c.OQCCheckFinish).ToList();
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
                var calib = calibrationRecord.Where(c => c.BarCodesNum == item && (c.Oldbarocode == item || c.Oldbarocode == null)).Select(c => c.Normal).ToList();
                if (calib.Count == 0)
                {
                    statu.Add("CalibrationRecord", 3);
                }
                else
                {
                    if (calib.Contains(true))
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
                var Appearance = appearn.Where(c => c.BarCodesNum == item && (c.Oldbarocode == item || c.Oldbarocode == null)).Select(c => c.OQCCheckFinish).ToList();
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
                //包装

                if (packingList.Count == 0 || !packingList.Contains(item))
                {
                    statu.Add("Packing", "未包装");
                }
                else
                {
                    statu.Add("Packing", "已包装");
                }

                //入库
                if (warehouse.Count() == 0)
                {
                    statu.Add("Warehoujoin", "未入库");
                    statu.Add("Warehouout", "未出库");
                }
                else
                {
                    var warehoujoin = warehouse.Count(c => c.Barcode == item);
                    if (warehoujoin == 0)
                    {
                        statu.Add("Warehoujoin", "未入库");
                    }
                    else
                    {
                        statu.Add("Warehoujoin", "已入库");
                    }
                    //出库
                    var warehouout = warehouse.OrderByDescending(c => c.id).Where(c => c.Barcode == item).Select(c => c.IsOut).FirstOrDefault();
                    if (warehouout)
                    {
                        statu.Add("Warehouout", "已出库");
                    }
                    else
                    {
                        statu.Add("Warehouout", "未出库");
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
                var ass = db.Assemble.Where(c => c.BoxBarCode == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null)).Select(c => c.PQCCheckFinish).ToList();
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
                        var finish = db.Assemble.Where(c => c.BoxBarCode == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.PQCCheckFinish == true).FirstOrDefault();
                        if (ass.Count == 1)
                        {
                            result.Add("pqcStatu", 1);
                            result.Add("pqcCreator", finish.AssemblePQCPrincipal);
                            result.Add("pqcCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.PQCCheckFT));
                            result.Add("pqcAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.Assemble.Where(c => c.BoxBarCode == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.PQCCheckFinish == false && c.PQCCheckAbnormal != null).ToList();
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
                        var current = db.Assemble.Where(c => c.BoxBarCode == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null)).FirstOrDefault();
                        if (ass.Count == 1 && current.PQCCheckAbnormal == null)
                        {
                            result.Add("pqcStatu", 4);
                            result.Add("pqcCreator", current.AssemblePrincipal);
                            result.Add("pqcCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.PQCCheckBT));
                            result.Add("pqcAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.Assemble.Where(c => c.BoxBarCode == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.PQCCheckFinish == false && c.PQCCheckAbnormal != null).ToList();
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
                var fqc = db.FinalQC.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null)).Select(c => c.FQCCheckFinish).ToList();
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
                        var finish = db.FinalQC.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.FQCCheckFinish == true).FirstOrDefault();
                        if (fqc.Count == 1)
                        {
                            result.Add("fqcStatu", 1);
                            result.Add("fqcCreator", finish.FQCPrincipal);
                            result.Add("fqcCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.FQCCheckFT));
                            result.Add("fqcAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.FinalQC.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.FQCCheckFinish == false && c.FinalQC_FQCCheckAbnormal != null).ToList();
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
                        var current = db.FinalQC.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null)).FirstOrDefault();
                        if (fqc.Count == 1 && current.FinalQC_FQCCheckAbnormal == null)
                        {
                            result.Add("fqcStatu", 4);
                            result.Add("fqcCreator", current.FQCPrincipal);
                            result.Add("fqcCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.FQCCheckBT));
                            result.Add("fqcAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.FinalQC.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.FQCCheckFinish == false && c.FinalQC_FQCCheckAbnormal != null).ToList();
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
                var mosaicScreen = db.Burn_in_MosaicScreen.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null)).FirstOrDefault();
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
                var burn_in = db.Burn_in.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null)).Select(c => c.OQCCheckFinish).ToList();
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
                        var finish = db.Burn_in.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.OQCCheckFinish == true).FirstOrDefault();
                        if (burn_in.Count == 1)
                        {
                            result.Add("burnInStatu", 1);
                            result.Add("burnInCreator", finish.OQCPrincipal);
                            result.Add("burnInCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.OQCCheckFT));
                            result.Add("burnInAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.Burn_in.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal != null).ToList();
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
                        var current = db.Burn_in.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null)).FirstOrDefault();
                        if (burn_in.Count == 1 && current.Burn_in_OQCCheckAbnormal == null)
                        {
                            result.Add("burnInStatu", 4);
                            result.Add("burnInCreator", current.OQCPrincipal);
                            result.Add("burnInCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.OQCCheckBT));
                            result.Add("burnInAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.Burn_in.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.OQCCheckFinish == false && c.Burn_in_OQCCheckAbnormal != null).ToList();
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
                var calibrationRecord = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null)).Select(c => c.Normal).ToList();
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
                        var finish = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.Normal == true).FirstOrDefault();
                        if (calibrationRecord.Count == 1)
                        {
                            result.Add("calibrationRecordStatu", 1);
                            result.Add("calibrationRecordCreator", finish.Operator);
                            result.Add("calibrationRecordCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.FinishCalibration));
                            result.Add("calibrationRecordAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.Normal == false && c.AbnormalDescription != null).ToList();
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
                        var current = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null)).FirstOrDefault();
                        if (calibrationRecord.Count == 1 && current.AbnormalDescription == null)
                        {
                            result.Add("calibrationRecordStatu", 4);
                            result.Add("calibrationRecordCreator", current.Operator);
                            result.Add("calibrationRecordCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.BeginCalibration));
                            result.Add("calibrationRecordAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.Normal == false && c.AbnormalDescription != null).ToList();
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
                var appearance = db.Appearance.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null)).Select(c => c.OQCCheckFinish).ToList();
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
                        var finish = db.Appearance.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.OQCCheckFinish == true).FirstOrDefault();
                        if (appearance.Count == 1)
                        {
                            result.Add("appearanceStatu", 1);
                            result.Add("appearanceCreator", finish.OQCPrincipal);
                            result.Add("appearanceCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", finish.OQCCheckFT));
                            result.Add("appearanceAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.Appearance.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.OQCCheckFinish == false && c.Appearance_OQCCheckAbnormal != null).ToList();
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
                        var current = db.Appearance.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null)).FirstOrDefault();
                        if (appearance.Count == 1 && current.Appearance_OQCCheckAbnormal == null)
                        {
                            result.Add("appearanceStatu", 4);
                            result.Add("appearanceCreator", current.OQCPrincipal);
                            result.Add("appearanceCreatdate", string.Format("{0:yyyy-MM-dd hh:mm:ss}", current.OQCCheckBT));
                            result.Add("appearanceAbnormal", "");
                        }
                        else
                        {
                            var abnormal = db.Appearance.Where(c => c.BarCodesNum == barcode && (c.OldBarCodesNum == barcode || c.OldBarCodesNum == null) && c.OQCCheckFinish == false && c.Appearance_OQCCheckAbnormal != null).ToList();
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

                var outherBarcode = db.Packing_BarCodePrinting.Where(c => c.BarCodeNum == barcode && c.QC_Operator == null).ToList();
                var wahouser_join = db.Warehouse_Join.Where(c => c.BarCodeNum == barcode).ToList();
                if (outherBarcode.Count != 0)
                {
                    JObject result2 = new JObject();
                    result2.Add("ordernum", outherBarcode.FirstOrDefault().OrderNum);
                    result2.Add("otherBarcode", outherBarcode.FirstOrDefault().OuterBoxBarcode);
                    result2.Add("type", outherBarcode.FirstOrDefault().Type);
                    //var wahouser_join = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == barcode).ToList();
                    if (wahouser_join.Count != 0)
                    {

                        var num = wahouser_join.Max(c => c.WarehouseOutNum);
                        if (num != 0)
                        {
                            var isout = wahouser_join.OrderByDescending(c => c.Date).Select(c => c.IsOut).FirstOrDefault();
                            if (isout)
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
                    }
                    else
                    {
                        result2.Add("statu", "未入过库");
                        result2.Add("name", outherBarcode.FirstOrDefault().Operator);
                    }
                    JArray barcodejarray = new JArray();
                    barcodejarray.Add(barcode);
                    result2.Add("barcodelist", barcodejarray);
                    total.Add("two", result2);
                }
                else if (wahouser_join.Count != 0)
                {
                    JObject result2 = new JObject();
                    result2.Add("ordernum", wahouser_join.FirstOrDefault().OrderNum);
                    result2.Add("otherBarcode", outherBarcode.FirstOrDefault().OuterBoxBarcode);
                    result2.Add("type", outherBarcode.FirstOrDefault().Type);
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
            var outherBarcode = db.Packing_BarCodePrinting.Where(c => c.OuterBoxBarcode == barcode && c.QC_Operator == null).ToList();
            if (outherBarcode.Count != 0)
            {
                JObject result = new JObject();
                result.Add("ordernum", outherBarcode.FirstOrDefault().OrderNum);
                result.Add("otherBarcode", outherBarcode.FirstOrDefault().OuterBoxBarcode);
                result.Add("type", outherBarcode.FirstOrDefault().Type);
                var wahouser_join = db.Warehouse_Join.Where(c => c.OuterBoxBarcode == barcode).ToList();
                if (wahouser_join.Count != 0)
                {
                    var num = wahouser_join.Max(c => c.WarehouseOutNum);
                    if (num != 0)
                    {
                        var isout = wahouser_join.OrderByDescending(c => c.Date).Select(c => c.IsOut).FirstOrDefault();
                        if (isout)
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
                }
                else
                {
                    result.Add("statu", "未入过库");
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
                result.Add("materialNumber", sulderbarcode.MaterialNumber);
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

        public ActionResult OrderNumReport()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Query", act = "OrderNumReport" });
            }
            return View();
        }



        [HttpPost]
        //订单生产信息概况报表
        public ActionResult OrderNumReport(string ordernum)
        {
            JObject result = new JObject();
            #region 订单基本信息
            var orderinfo = db.OrderMgm.Where(c => c.OrderNum == ordernum).FirstOrDefault();
            var smtPlaninfo = db.SMT_ProductionPlan.Where(c => c.OrderNum == ordernum).ToList();
            //出库结束时间
            var warehouseOutendtime = db.Warehouse_Join.OrderByDescending(c => c.Id).Where(c => c.CartonOrderNum == ordernum && (c.NewBarcode == null || c.NewBarcode == ordernum) && c.IsOut == true && c.State == "模组").Select(c => c.WarehouseOutDate).FirstOrDefault();
            //订单号
            result.Add("OrderNum", orderinfo.OrderNum);
            //下单日期
            result.Add("ContractDate", orderinfo.ContractDate.ToString());
            //模组数量
            result.Add("Boxes", orderinfo.Boxes);
            //平台型号
            result.Add("PlatformType", orderinfo.PlatformType);
            //计划生产日期
            result.Add("PlanInputTime", orderinfo.PlanInputTime.ToString());
            //计划出货日期
            result.Add("PlanCompleteTime", orderinfo.PlanCompleteTime.ToString());
            //制程要求
            result.Add("ProcessingRequire", orderinfo.ProcessingRequire);
            //标准要求
            result.Add("StandardRequire", orderinfo.StandardRequire);
            //实际开始生产时间
            //result.Add("", orderinfo.StandardRequire);
            //最终出货时间
            result.Add("ultimatelyTime", warehouseOutendtime);
            //客户名称
            result.Add("CustomerName", orderinfo.CustomerName);
            //地区
            result.Add("Area", orderinfo.Area);
            //条码创建人
            result.Add("BarCodeCreator", orderinfo.BarCodeCreator);
            //条码创建时间
            result.Add("BarCodeCreateDate", orderinfo.BarCodeCreateDate.ToString());
            //SMT计划
            JArray smtplan = new JArray();
            foreach (var plan in smtPlaninfo)
            {
                JObject smtplanitem = new JObject();
                //SMT计划人
                smtplanitem.Add("smtPlanCreator", plan.Creator);
                //SMT计划时间
                smtplanitem.Add("smtPlanProductionDate", plan.PlanProductionDate.ToString());
                smtplan.Add(smtplanitem);
            }
            result.Add("Smtplan", smtplan);
            //是否为库存订单
            result.Add("IsRepertory", orderinfo.IsRepertory);
            //备注信息
            result.Add("Remark", orderinfo.Remark);
            #endregion

            #region 小样基本报告
            JObject small = new JObject();
            var smallinfo = db.Small_Sample.Where(c => c.OrderNumber.Contains(ordernum)).FirstOrDefault();
            if (smallinfo != null)
            {
                //上传人
                small.Add("SmallCreator", smallinfo.Creator);
                //上传时间
                small.Add("SmallCreatedDate", smallinfo.CreatedDate);
                //审核人
                small.Add("SmallAssessor", smallinfo.Assessor);
                //审核时间
                small.Add("SmallAssessedDate", smallinfo.AssessedDate);
                //核准人
                small.Add("SmallApproved", smallinfo.Approved);
                //核准时间
                small.Add("SmallApprovedDate", smallinfo.ApprovedDate);

                result.Add("Small", small);
            }
            else
            { result.Add("Small", null); }
            #endregion

            #region smt

            //模块数量
            var mianCount = db.SMT_ProductionBoardTable.Where(c => c.OrderNum == ordernum).Select(c => c.JobContent).Distinct().ToList();
            JArray smtjarray = new JArray();
            foreach (var name in mianCount)
            {
                JObject smtjobject = new JObject();
                var smtinfo = db.SMT_ProductionData.Where(c => c.OrderNum == ordernum && c.JobContent == name).ToList();
                var CompleteNum = 0;
                var PassNum = 0;
                if (smtinfo.Count != 0)
                {
                    CompleteNum = smtinfo.Sum(c => c.NormalCount) + smtinfo.Sum(c => c.AbnormalCount);
                    PassNum = smtinfo.Sum(c => c.NormalCount);
                }
                var TotalNum = (name == "IC面" || name == "灯面") ? orderinfo.Models : (name.Contains("电源") ? orderinfo.Powers : orderinfo.AdapterCard);
                smtjobject.Add("Jobcontent", name);
                smtjobject.Add("TotalNum", TotalNum);
                smtjobject.Add("CompleteNum", CompleteNum);
                smtjobject.Add("PassNum", PassNum);
                smtjobject.Add("CompleteRate", TotalNum == 0 ? "0%" : ((decimal)CompleteNum * 100 / TotalNum).ToString("F2") + "%");
                smtjobject.Add("PassRate", CompleteNum == 0 ? "0%" : ((decimal)PassNum * 100 / CompleteNum).ToString("F2") + "%");

                smtjarray.Add(smtjobject);

            }
            result.Add("Smt", smtjarray);
            #endregion

            #region 组装
            var assemble = db.Assemble.Where(c => c.OrderNum == ordernum && (c.OldOrderNum == ordernum || c.OldOrderNum == null) && c.PQCCheckFT != null).ToList();
            if (assemble.Count != 0)
            {
                var BTtime = assemble.OrderBy(c => c.Id).Select(c => c.PQCCheckBT).FirstOrDefault();

                var endtime = assemble.OrderByDescending(c => c.Id).Select(c => c.PQCCheckFT).FirstOrDefault();

                var complete = assemble.Where(c => c.PQCCheckFinish == true && c.RepetitionPQCCheck == false).Select(c => c.BoxBarCode).Distinct().ToList();

                var abnormal = assemble.Where(c => c.PQCCheckFinish == false).Select(c => c.BoxBarCode).Distinct().ToList();
                var passthrough = complete.Except(abnormal).ToList();

                var abnormalList = assemble.Where(c => c.PQCCheckFinish == false).Select(c => new TempAbnormal { Barcode = c.BoxBarCode, Abnormal = c.PQCCheckAbnormal }).ToList();

                var assmebleJobject = ProcessValue(orderinfo.AssembleFirstSample_Description, orderinfo.AssembleFirstSampleConverter, BTtime, endtime, complete.Count, orderinfo.Boxes, assemble.Count, passthrough.Count, abnormalList);

                result.Add("Assmeble", assmebleJobject);
            }
            else
            { result.Add("Assmeble", null); }
            #endregion

            #region FQC
            var FQC = db.FinalQC.Where(c => c.OrderNum == ordernum && (c.OldOrderNum == ordernum || c.OldOrderNum == null) && c.FQCCheckFT != null).ToList();
            if (FQC.Count != 0)
            {
                var fqcbeginTime = FQC.OrderBy(c => c.Id).Select(c => c.FQCCheckBT).FirstOrDefault();

                var fqcEndtime = FQC.OrderByDescending(c => c.Id).Select(c => c.FQCCheckFT).FirstOrDefault();

                var fqccomplete = FQC.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Select(c => c.BarCodesNum).Distinct().ToList();

                var fqcabnormal = FQC.Where(c => c.FQCCheckFinish == false).Select(c => c.BarCodesNum).Distinct().ToList();
                var fqcpassthrough = fqccomplete.Except(fqcabnormal).ToList();

                var fqcabnormalList = FQC.Where(c => c.FQCCheckFinish == false).Select(c => new TempAbnormal { Barcode = c.BarCodesNum, Abnormal = c.FinalQC_FQCCheckAbnormal }).ToList();

                var fqcjobject = ProcessValue(null, null, fqcbeginTime, fqcEndtime, fqccomplete.Count, orderinfo.Boxes, FQC.Count, fqcpassthrough.Count, fqcabnormalList);

                result.Add("FQC", fqcjobject);
            }
            else
            { result.Add("FQC", null); }
            #endregion

            #region 老化
            var burin = db.Burn_in.Where(c => c.OrderNum == ordernum && (c.OldOrderNum == ordernum || c.OldOrderNum == null) && c.OQCCheckFT != null).ToList();
            if (burin.Count != 0)
            {
                var burninBeginTime = burin.OrderBy(c => c.Id).Select(c => c.OQCCheckBT).FirstOrDefault();

                var burninEndtime = burin.OrderByDescending(c => c.Id).Select(c => c.OQCCheckFT).FirstOrDefault();

                var burninComplete = burin.Where(c => c.OQCCheckFinish == true).Select(c => c.BarCodesNum).Distinct().ToList();

                var burnAbnoramalBarcode = burin.Where(c => c.OQCCheckFinish == false).Select(c => c.BarCodesNum).Distinct().ToList();
                var burnPassThrough = burninComplete.Except(burnAbnoramalBarcode).ToList().Count();

                var burnabnormalList = burin.Where(c => c.OQCCheckFinish == false).Select(c => new TempAbnormal { Barcode = c.BarCodesNum, Abnormal = c.Burn_in_OQCCheckAbnormal }).ToList();

                var burnjobject = ProcessValue(orderinfo.BurnInFirstSample_Description, orderinfo.BurnInFirstSampleConverter, burninBeginTime, burninEndtime, burninComplete.Count, orderinfo.Boxes, burin.Count, burnPassThrough, burnabnormalList);

                result.Add("Burin", burnjobject);
            }
            else
            {
                result.Add("Burin", null);
            }
            #endregion

            #region 校正
            var calibration = db.CalibrationRecord.Where(c => c.OrderNum == ordernum && (c.OldOrderNum == ordernum || c.OldOrderNum == null) && c.FinishCalibration != null).ToList();
            if (calibration.Count != 0)
            {
                var calibrationBeginTime = calibration.OrderBy(c => c.ID).Select(c => c.BeginCalibration).FirstOrDefault();

                var calibrationEndtime = calibration.OrderByDescending(c => c.ID).Select(c => c.FinishCalibration).FirstOrDefault();

                var calibrationComplete = calibration.Where(c => c.Normal == true).Select(c => c.BarCodesNum).Distinct().ToList();

                var calibrationAbnoramalBarcode = calibration.Where(c => c.Normal == false).Select(c => c.BarCodesNum).Distinct().ToList();
                var calibrationPassThrough = calibrationComplete.Except(calibrationAbnoramalBarcode).ToList().Count();

                var calibrationabnormalList = calibration.Where(c => c.Normal == false).Select(c => new TempAbnormal { Barcode = c.BarCodesNum, Abnormal = c.AbnormalDescription }).ToList();

                var calibrationjobject = ProcessValue(null, null, calibrationBeginTime, calibrationEndtime, calibrationComplete.Count, orderinfo.Boxes, calibration.Count, calibrationPassThrough, calibrationabnormalList);

                result.Add("Calibration", calibrationjobject);
            }
            else
            {
                result.Add("Calibration", null);
            }
            #endregion

            #region 外观电检
            var appearance = db.Appearance.Where(c => c.OrderNum == ordernum && (c.OldOrderNum == ordernum || c.OldOrderNum == null) && c.OQCCheckFT != null).ToList();
            if (appearance.Count != 0)
            {
                var appearanceBeginTime = appearance.OrderBy(c => c.Id).Select(c => c.OQCCheckBT).FirstOrDefault();

                var appearanceEndtime = appearance.OrderByDescending(c => c.Id).Select(c => c.OQCCheckFT).FirstOrDefault();

                var appearanceComplete = appearance.Where(c => c.OQCCheckFinish == true).Select(c => c.BarCodesNum).Distinct().ToList();

                var appearanceAbnoramalBarcode = appearance.Where(c => c.OQCCheckFinish == false).Select(c => c.BarCodesNum).Distinct().ToList();
                var appearancePassThrough = appearanceComplete.Except(appearanceAbnoramalBarcode).ToList().Count();

                var appearanceabnormalList = appearance.Where(c => c.OQCCheckFinish == false).Select(c => new TempAbnormal { Barcode = c.BarCodesNum, Abnormal = c.Appearance_OQCCheckAbnormal }).ToList();

                var appearancejobject = ProcessValue(orderinfo.AppearanceFirstSample_Description, orderinfo.AppearanceFirstSampleConverter, appearanceBeginTime, appearanceEndtime, appearanceComplete.Count, orderinfo.Boxes, appearance.Count, appearancePassThrough, appearanceabnormalList);

                result.Add("Appearance", appearancejobject);
            }
            else
            {
                result.Add("Appearance", null);
            }

            #endregion

            #region 包装
            JObject package = new JObject();
            //开始时间
            var packageBeginTime = db.Packing_BarCodePrinting.OrderBy(c => c.Id).Where(c => c.OrderNum == ordernum && c.CartonOrderNum == ordernum && c.QC_Operator == null).Select(c => c.Date).FirstOrDefault();
            package.Add("packageBeginTime", packageBeginTime.ToString());
            //完成时间
            var packageEndTime = db.Packing_BarCodePrinting.OrderByDescending(c => c.Id).Where(c => c.OrderNum == ordernum && c.CartonOrderNum == ordernum && c.QC_Operator == null).Select(c => c.Date).FirstOrDefault();
            package.Add("packageEndTime", packageEndTime.ToString());
            result.Add("Package", package);
            #endregion

            #region 出入库
            JObject warehouse = new JObject();
            //入库开始时间
            var warehouseJoinbegtime = db.Warehouse_Join.OrderBy(c => c.Id).Where(c => c.CartonOrderNum == ordernum && (c.NewBarcode == null || c.NewBarcode == ordernum) && c.State == "模组").Select(c => c.Date).FirstOrDefault();
            if (warehouseJoinbegtime != null)
            {
                warehouse.Add("warehouseJoinbegtime", warehouseJoinbegtime);
                //入库结束时间
                var warehouseJoinendtime = db.Warehouse_Join.OrderByDescending(c => c.Id).Where(c => c.CartonOrderNum == ordernum && (c.NewBarcode == null || c.NewBarcode == ordernum) && c.State == "模组").Select(c => c.Date).FirstOrDefault();
                warehouse.Add("warehouseJoinendtime", warehouseJoinendtime);
                //出库开始时间
                var warehouseOutbegtime = db.Warehouse_Join.OrderBy(c => c.Id).Where(c => c.CartonOrderNum == ordernum && (c.NewBarcode == null || c.NewBarcode == ordernum) && c.IsOut == true && c.State == "模组").Select(c => c.WarehouseOutDate).FirstOrDefault();
                warehouse.Add("warehouseOutbegtime", warehouseOutbegtime);

                warehouse.Add("warehouseOutendtime", warehouseOutendtime);
                //库存时长
                var timespan = warehouseOutendtime - warehouseJoinbegtime;
                var WorkTimeSpan = "";
                if (timespan.Value.Days > 0)
                {
                    WorkTimeSpan = timespan.Value.Days.ToString() + "天" + timespan.Value.Minutes.ToString() + "分" + timespan.Value.Seconds.ToString() + "秒";
                }
                else
                {
                    WorkTimeSpan = timespan.Value.Minutes.ToString() + "分" + timespan.Value.Seconds.ToString() + "秒";
                }
                warehouse.Add("Timespan", WorkTimeSpan);

                result.Add("Warehouse", warehouse);
            }
            else
                result.Add("Warehouse", null);
            #endregion


            return Content(JsonConvert.SerializeObject(result));
        }


        public JObject ProcessValue(string FirstDescription, string FirstConverter, DateTime? begtime, DateTime? endtime, int completecount, int boxes, int totalcount, int passthrouhcount, List<TempAbnormal> tempAbnormals)
        {
            JObject result = new JObject();
            //首件信息
            result.Add("FirstDescription", FirstDescription);
            //首件操作人
            result.Add("FirstConverter", FirstConverter);
            //开始时间
            result.Add("BeginTime", begtime.ToString());
            //完成时间
            result.Add("EndTime", endtime.ToString());
            //完成率
            result.Add("CompletionRate", boxes == 0 ? "0%" : ((decimal)completecount * 100 / boxes).ToString("F2") + "%");
            //合格率
            result.Add("PassRate", totalcount == 0 ? "0%" : ((decimal)completecount * 100 / totalcount).ToString("F2") + "%");
            //直通率
            result.Add("PassthroughRate", totalcount == 0 ? "0%" : ((decimal)passthrouhcount * 100 / totalcount).ToString("F2") + "%");
            //作业时长
            var CT = endtime - begtime;
            string WorkTimeSpan = "";
            if (CT.Value.Days > 0)
            {
                WorkTimeSpan = CT.Value.Days.ToString() + "天" + CT.Value.Minutes.ToString() + "分" + CT.Value.Seconds.ToString() + "秒";
            }
            else
            {
                WorkTimeSpan = CT.Value.Minutes.ToString() + "分" + CT.Value.Seconds.ToString() + "秒";
            }
            result.Add("WorkTimeSpan", WorkTimeSpan);
            //异常记录清单
            JArray assembleabList = new JArray();
            foreach (var abnormalitem in tempAbnormals)
            {
                assembleabList.Add("条码:" + abnormalitem.Barcode + "异常信息为" + abnormalitem.Abnormal);
            }
            result.Add("AbnormalList", assembleabList);
            return result;
        }


        public ActionResult businessDepartment()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Query", act = "businessDepartment" });
            }
            return View();
        }
        public ActionResult contractDepartment()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Query", act = "contractDepartment" });
            }
            return View();
        }


        #region 销售部、合约部查询方法
        public class TEMP
        {
            //ordernum
            public string OrderNum { get; set; }
            //对应OldOrderNum，OuterBoxBarcode，JobContent
            public string TwoString { get; set; }
            public string BarCodesNum { get; set; }
            public bool Finish { get; set; }
            public bool Repetition { get; set; }
            public DateTime? EndTime { get; set; }
            public DateTime? WarehouseOutDate { get; set; }
            public int id { get; set; }
            //对应Quantity NormalCount
            public int Quantity { get; set; }
            //AbnormalCount
            public int Quantity2 { get; set; }
            public string Transportation { get; set; }
            public string WarehouseOutDocuments { get; set; }
            public string state { get; set; }
        }

        //销售部查看订单信息
        public ActionResult SelectOrderInfoFromBusiness(string ordernum,bool alling=false)
        {
            JArray array = new JArray();
            List<OrderMgm> orderinfo = null;
            if (alling)
            {
                orderinfo = db.OrderMgm.ToList();
                var jsonstring = "";
                if (System.IO.File.Exists(@"D:\MES_Data\TemDate\ProductionControllerExcept.json") == true)
                {
                    jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\ProductionControllerExcept.json");
                    string[] Exceptarray = jsonstring.Split(',');
                    orderinfo = orderinfo.Where(c => !Exceptarray.Contains(c.OrderNum)).OrderBy(m => m.OrderNum).ToList();
                }
            }
            else
            {
                orderinfo= db.OrderMgm.Where(c => c.OrderNum.Contains(ordernum)).ToList();
            }

            #region 原来程序
            //#region 取值
            //var fqc = db.FinalQC.Select(c => new TEMP { OrderNum = c.OrderNum, TwoString = c.OldOrderNum, Finish = c.FQCCheckFinish, Repetition = c.RepetitionFQCCheck, EndTime = c.FQCCheckFT, id = c.Id }).ToList();
            //var burn = db.Burn_in.Select(c => new TEMP { OrderNum = c.OrderNum, TwoString = c.OldOrderNum, Finish = c.OQCCheckFinish, EndTime = c.OQCCheckFT, id = c.Id }).ToList();
            //var package = db.Packing_BasicInfo.Select(c => new TEMP { OrderNum = c.OrderNum, Quantity = c.Quantity }).ToList();
            //var packageprint = db.Packing_BarCodePrinting.Select(c => new TEMP { OrderNum = c.OrderNum, TwoString = c.OuterBoxBarcode, EndTime = c.Date, id = c.Id }).ToList();
            //var warehouseinfo = db.Warehouse_Join.Select(c => new TEMP { OrderNum = c.OrderNum, TwoString = c.OuterBoxBarcode, Finish = c.IsOut, EndTime = c.Date, WarehouseOutDate = c.WarehouseOutDate, Transportation = c.Transportation, WarehouseOutDocuments = c.WarehouseOutDocuments, id = c.Id }).ToList();
            //var smtdate = db.SMT_ProductionData.Select(c => new TEMP { OrderNum = c.OrderNum, TwoString = c.JobContent, Quantity = c.NormalCount, Quantity2 = c.AbnormalCount, EndTime = c.EndTime, id = c.Id }).ToList();
            //#endregion


            //foreach (var item in orderinfo)
            //{
            //    JObject result = new JObject();
            //    JObject total = new JObject();
            //    result.Add("ordernum", item.OrderNum);
            //    total.Add("name", "预计投入时间");

            //    total.Add("value", item.PlanInputTime.ToString("yyyy-MM-dd"));
            //    result.Add("Estimated", total);
            //    total = new JObject();

            //    total.Add("name", "预计完成时间");

            //    total.Add("value", item.PlanCompleteTime.ToString("yyyy-MM-dd"));
            //    result.Add("CompleteTime", total);
            //    total = new JObject();
            //    //SMT
            //    //对应工作内容良品总数
            //    var NormalNumSum = smtdate.Where(c => c.OrderNum == item.OrderNum && c.TwoString == "灯面").Count() == 0 ? 0 : smtdate.Where(c => c.OrderNum == item.OrderNum && c.TwoString == "灯面").Sum(c => c.Quantity);

            //    //对应工作内容不良品总数
            //    var AbnormalNumSum = smtdate.Where(c => c.OrderNum == item.OrderNum && c.TwoString == "灯面").Count() == 0 ? 0 : smtdate.Where(c => c.OrderNum == item.OrderNum && c.TwoString == "灯面").Sum(c => c.Quantity2);

            //    //LASTTIME
            //    var lasttime = smtdate.OrderByDescending(c => c.id).Where(c => c.OrderNum == item.OrderNum && c.TwoString == "灯面").Select(c => c.EndTime).FirstOrDefault();
            //    result.Add("SMT", GetChildrenJobject("PCBA",item.Models, NormalNumSum+ AbnormalNumSum, lasttime,false));

            //    #region 模组
            //    var FinalQC_Record = fqc.Where(c => c.OrderNum == item.OrderNum && (c.TwoString == null || c.TwoString == item.OrderNum)).ToList();
            //    var fqclasttime = fqc.OrderByDescending(c => c.id).Where(c => c.OrderNum == item.OrderNum && (c.TwoString == null || c.TwoString == item.OrderNum)).Select(c => c.EndTime).FirstOrDefault();
            //    if (FinalQC_Record.Count > 0)
            //    {
            //        int FinalQC_Finish = FinalQC_Record.Where(c => c.Finish == true && c.Repetition == false).Count();
            //        result.Add("Model", GetChildrenJobject("模组", item.Boxes, FinalQC_Finish, fqclasttime, false));
            //    }
            //    else
            //    {
            //        result.Add("Model", GetChildrenJobject("模组", item.Boxes, 0, fqclasttime, false));
            //    }
            //    #endregion

            //    #region 老化调试
            //    var Burn_in_Record = (from m in burn where m.OrderNum == item.OrderNum && (m.TwoString == null || m.TwoString == item.OrderNum) select m).ToList();//查出OrderNum的所有老化记录
            //    var burnlasttime = burn.OrderByDescending(c => c.id).Where(c => c.OrderNum == item.OrderNum && (c.TwoString == null || c.TwoString == item.OrderNum)).Select(c => c.EndTime).FirstOrDefault();

            //    result.Add("Burnin", GetChildrenJobject("老化", item.Boxes, Burn_in_Record.Count, burnlasttime, false));

            //    #endregion

            //    #region 包装
            //    //总箱数
            //    var totalpackage = package.Where(c => c.OrderNum == item.OrderNum).ToList();
            //    var num = 0;
            //    if (totalpackage.Count != 0)
            //    {
            //        totalpackage.ForEach(c => num = num + c.Quantity );
            //    }

            //    var print = packageprint.Where(c => c.OrderNum == item.OrderNum).Select(c => c.TwoString).Distinct().Count();
            //    var printLasttime = packageprint.OrderByDescending(c=>c.id).Where(c => c.OrderNum == item.OrderNum).Select(c => c.EndTime).FirstOrDefault();

            //    result.Add("package", GetChildrenJobject("包装", num, print, printLasttime, false));
            //    #endregion

            //    #region 入库
            //    var warehouse = warehouseinfo.Where(c => c.OrderNum == item.OrderNum).Select(c => c.TwoString).Distinct().Count();
            //    var warehouseLasttime = warehouseinfo.OrderByDescending(c => c.id).Where(c => c.OrderNum == item.OrderNum).Select(c => c.EndTime).FirstOrDefault();
            //    result.Add("warehousejoin", GetChildrenJobject("入库", num, warehouse, warehouseLasttime, true));

            //    #endregion

            //    #region 出库
            //    var warehouseout = warehouseinfo.Where(c => c.OrderNum == item.OrderNum && c.Finish == true).Select(c => c.TwoString).Distinct().Count();
            //    var warehouseoutLasttime = warehouseinfo.OrderByDescending(c => c.WarehouseOutDate).Where(c => c.OrderNum == item.OrderNum).Select(c => c.WarehouseOutDate).FirstOrDefault();

            //    result.Add("warehouseout", GetChildrenJobject("出库", num, warehouseout, warehouseoutLasttime, false));

            //    //出库运输方式
            //    var Transportation = warehouseinfo.Where(c => c.OrderNum == item.OrderNum && c.Finish == true).Select(c => c.Transportation).FirstOrDefault();
            //    total.Add("name", "出库运输方式");
            //    total.Add("value", Transportation);

            //    result.Add("Transportation", total);
            //    total = new JObject();
            //    //出库单据
            //    var WarehouseOutDocuments = warehouseinfo.Where(c => c.OrderNum == item.OrderNum && c.Finish == true).Select(c => c.WarehouseOutDocuments).FirstOrDefault();
            //    total.Add("name", "出库单据");
            //    total.Add("value", WarehouseOutDocuments);

            //    result.Add("WarehouseOutDocuments", total);
            //    array.Add(result);
            //    #endregion
            //}
            #endregion

            #region 取值
            var fqc = db.FinalQC.Select(c => new TEMP { OrderNum = c.OrderNum, TwoString = c.OldOrderNum, Finish = c.FQCCheckFinish, Repetition = c.RepetitionFQCCheck, EndTime = c.FQCCheckFT, id = c.Id }).ToList();
            var burn = db.Burn_in.Select(c => new TEMP { OrderNum = c.OrderNum, TwoString = c.OldOrderNum, Finish = c.OQCCheckFinish, EndTime = c.OQCCheckFT, id = c.Id , BarCodesNum = c.BarCodesNum}).ToList();
            var package = db.Packing_BasicInfo.Select(c => new TEMP { OrderNum = c.OrderNum, Quantity = c.Quantity }).ToList();
            var packageprint = db.Packing_BarCodePrinting.Select(c => new TEMP { OrderNum = c.OrderNum, TwoString = c.OuterBoxBarcode, EndTime = c.Date, id = c.Id }).ToList();
            var warehouseinfo = db.Warehouse_Join.Select(c => new TEMP { OrderNum = c.OrderNum, TwoString = c.OuterBoxBarcode, Finish = c.IsOut, EndTime = c.Date, WarehouseOutDate = c.WarehouseOutDate, Transportation = c.Transportation, WarehouseOutDocuments = c.WarehouseOutDocuments, id = c.Id ,state=c.State}).ToList();
            var smtdata = db.SMT_ProductionData.Select(c => new TEMP{ OrderNum = c.OrderNum, TwoString = c.JobContent, Quantity = c.NormalCount, Quantity2 = c.AbnormalCount, EndTime = c.EndTime, id = c.Id }).ToList();
            #endregion


            foreach (var item in orderinfo)
            {
                JObject result = new JObject();
                JObject total = new JObject();
                result.Add("ordernum", item.OrderNum);
                total.Add("name", "预计投入时间");

                total.Add("value", item.PlanInputTime.ToString("yyyy-MM-dd"));
                result.Add("Estimated", total);
                total = new JObject();

                total.Add("name", "预计完成时间");

                total.Add("value", item.PlanCompleteTime.ToString("yyyy-MM-dd"));
                result.Add("CompleteTime", total);
                total = new JObject();
                //SMT
                var smtdatalist = smtdata.Where(c => c.OrderNum == item.OrderNum && c.TwoString == "灯面").ToList();
                if(smtdatalist.Count ==0)
                {
                    result.Add("SMT", GetChildrenJobject("PCBA", item.Models, 0, null, false));
                }
                else
                {
                    //对应工作内容良品总数
                    var NormalNumSum = smtdatalist.Sum(c => c.Quantity);

                    //对应工作内容不良品总数
                    var AbnormalNumSum = smtdatalist.Sum(c => c.Quantity2);

                    //LASTTIME
                    var lasttime = smtdatalist.OrderByDescending(c => c.id).FirstOrDefault().EndTime;
                    result.Add("SMT", GetChildrenJobject("PCBA", item.Models, NormalNumSum + AbnormalNumSum, lasttime, false));
                }

                #region 模组
                var FinalQC_Record = fqc.Where(c => c.OrderNum == item.OrderNum && (c.TwoString == null || c.TwoString == item.OrderNum)).ToList();
                var fqclasttime = FinalQC_Record.OrderByDescending(c => c.id).Select(c=>c.EndTime).FirstOrDefault();
                if (FinalQC_Record.Count > 0)
                {
                    int FinalQC_Finish = FinalQC_Record.Where(c => c.Finish == true && c.Repetition == false).Count();
                    result.Add("Model", GetChildrenJobject("模组", item.Boxes, FinalQC_Finish, fqclasttime, false));
                }
                else
                {
                    result.Add("Model", GetChildrenJobject("模组", item.Boxes, 0, fqclasttime, false));
                }
                #endregion

                #region 老化调试
                var Burn_in_Record = (from m in burn where m.OrderNum == item.OrderNum && (m.TwoString == null || m.TwoString == item.OrderNum)  select m).ToList();//查出OrderNum的所有老化记录
                var Burn_in_Record_finish = (from m in burn where m.OrderNum == item.OrderNum && (m.TwoString == null || m.TwoString == item.OrderNum) && m.Finish==true select m).ToList();//已完成
                var burnlasttime = Burn_in_Record_finish.OrderByDescending(c => c.id).Select(c=>c.EndTime).FirstOrDefault();
                if(Burn_in_Record.Count > 0 && Burn_in_Record_finish.Count < item.Boxes)
                {
                    JObject burn_in_ing = new JObject();
                    burn_in_ing.Add("name", "老化");
                    burn_in_ing.Add("valuename", "进行中");
                    burn_in_ing.Add("value", Burn_in_Record.Select(c=>c.BarCodesNum).Distinct().ToList().Count + "/" + item.Boxes);
                    result.Add("Burnin", burn_in_ing);
                }
                else
                {
                    result.Add("Burnin", GetChildrenJobject("老化", item.Boxes, Burn_in_Record_finish.Count, burnlasttime, false));
                }
                #endregion

                #region 包装
                //总箱数
                var totalpackage = package.Where(c => c.OrderNum == item.OrderNum).ToList();
                var num = 0;
                if (totalpackage.Count != 0)
                {
                    totalpackage.ForEach(c => num = num + c.Quantity);
                }
                var print = packageprint.Where(c => c.OrderNum == item.OrderNum).Select(c => c.TwoString).Distinct().Count();
                var printLasttime = packageprint.Where(c => c.OrderNum == item.OrderNum).OrderByDescending(c => c.id).Select(c => c.EndTime).FirstOrDefault();
                result.Add("package", GetChildrenJobject("包装", num, print, printLasttime, false));
                #endregion

                #region 入库
                var warehouselist = warehouseinfo.Where(c => c.OrderNum == item.OrderNum && c.state == "模组").ToList();
                var warehouse = warehouselist.Select(c => c.TwoString).Distinct().Count();
                var warehouseLasttime = warehouselist.OrderByDescending(c => c.id).Select(c => c.EndTime).FirstOrDefault();
                result.Add("warehousejoin", GetChildrenJobject("入库", num, warehouse, warehouseLasttime, true));
                #endregion

                #region 出库
                var warehouseoutlist = warehouseinfo.Where(c => c.OrderNum == item.OrderNum && c.Finish == true && c.state == "模组").ToList();
                var warehouseout = warehouseoutlist.Select(c => c.TwoString).Distinct().Count();
                var warehouseoutLasttime = warehouseinfo.Where(c => c.OrderNum == item.OrderNum && c.state == "模组").OrderByDescending(c => c.WarehouseOutDate).Select(c=>c.WarehouseOutDate).FirstOrDefault();
                result.Add("warehouseout", GetChildrenJobject("出库", num, warehouseout, warehouseoutLasttime, false));

                //出库运输方式
                var Transportation = warehouseoutlist.Select(c=>c.Transportation).FirstOrDefault();
                total.Add("name", "出库运输方式");
                total.Add("value", Transportation);
                result.Add("Transportation", total);
                total = new JObject();

                //出库单据
                var WarehouseOutDocuments = warehouseoutlist.Select(c=>c.WarehouseOutDocuments).FirstOrDefault();
                total.Add("name", "出库单据");
                total.Add("value", WarehouseOutDocuments);
                result.Add("WarehouseOutDocuments", total);
                array.Add(result);
                #endregion
            }
            UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]) == null ? "未获取登录信息" : ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = alling == true ? "在合约部或销售部选择了全部查询" : "在合约部或销售部查询了订单" + ordernum };
            db.UserOperateLog.Add(log);
            db.SaveChanges();
            return Content(JsonConvert.SerializeObject(array));
        }
        //合约部查看订单信息
        public ActionResult SelectOrderInfoFromContract(string ordernum)
        {
            JArray result = new JArray();
            #region 取值
            var fqc = db.FinalQC.Select(c => new { c.OrderNum, c.OldOrderNum, c.FQCCheckFinish, c.RepetitionFQCCheck }).ToList();
            var burn = db.Burn_in.Select(c => new { c.OrderNum, c.OldOrderNum, c.Burn_in_OQCCheckAbnormal, c.OQCCheckFinish }).ToList();
            var package = db.Packing_BasicInfo.Select(c => new { c.OrderNum, c.Quantity, c.OuterBoxCapacity }).ToList();
            var packageprint = db.Packing_BarCodePrinting.Select(c => new { c.OrderNum, c.OuterBoxBarcode }).ToList();
            var warehouseinfo = db.Warehouse_Join.Select(c => new { c.OrderNum, c.OuterBoxBarcode, c.IsOut,c.State }).ToList();
            var smtdate = db.SMT_ProductionData.Select(c => new { c.OrderNum, c.JobContent, c.NormalCount, c.AbnormalCount }).ToList();
            #endregion
            List<OrderMgm> OrderList_All = (from m in db.OrderMgm select m).ToList();
            OrderList_All = OrderList_All.OrderBy(m => m.OrderNum).Where(c => c.OrderNum.Contains(ordernum)).ToList();

            #region 剔除已经完成生产的订单号
            //
            //if (!string.IsNullOrEmpty(ordernum))
            //{
            //    OrderList_All = OrderList_All.Where(c => c.OrderNum == ordernum).ToList();
            //}
            //else
            //{
            //    List<OrderMgm> OutputOrderList = new List<OrderMgm>();
            //    List<OrderMgm> ExpectList = new List<OrderMgm>();
            //    if (Directory.Exists(@"D:\MES_Data\TemDate\") == false)//如果不存在就创建订单文件夹
            //    {
            //        Directory.CreateDirectory(@"D:\MES_Data\TemDate\");
            //    }
            //    var jsonstring = "";
            //    if (System.IO.File.Exists(@"D:\MES_Data\TemDate\ProductionControllerExcept.json") == true)
            //    {
            //        jsonstring = System.IO.File.ReadAllText(@"D:\MES_Data\TemDate\ProductionControllerExcept.json");
            //        string[] array = jsonstring.Split(',');
            //        OrderList_All = OrderList_All.Where(c => !array.Contains(c.OrderNum)).ToList();
            //    }
            //}
            #endregion

            foreach (var item in OrderList_All)
            {
                JObject orderinfo = new JObject();
                orderinfo.Add("ordernum", item.OrderNum);
                #region SMT

                //对应工作内容良品总数
                var NormalNumSum = smtdate.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "灯面").Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "灯面").Sum(c => c.NormalCount);

                //对应工作内容不良品总数
                var AbnormalNumSum = smtdate.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "灯面").Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == "灯面").Sum(c => c.AbnormalCount);
                
                //总完成率
                var SMTvalue = item.Models == 0 ? 0 : Math.Round(((decimal)(NormalNumSum + AbnormalNumSum)) / item.Models * 100, 2);
                
                orderinfo.Add("PCBA", SMTvalue);
                #endregion

                #region 模组
                var FinalQC_Record = fqc.Where(c => c.OrderNum == item.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == item.OrderNum)).ToList();
                if (FinalQC_Record.Count > 0)
                {
                    Decimal FinalQC_Finish = FinalQC_Record.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Count();
                    if (FinalQC_Finish == 0)

                        orderinfo.Add("FinalQC_Finish_Rate", 0);  //FQC完成率
                    else
                    {
                        var Modelvalue = item.Boxes == 0 ? 0 : Math.Round(FinalQC_Finish * 100 / item.Boxes, 2);
                        orderinfo.Add("FinalQC_Finish_Rate", Modelvalue > 100 ? 100 : Modelvalue);

                    }
                }
                else
                {
                    orderinfo.Add("FinalQC_Finish_Rate", 0);  //FQC完成率

                }
                #endregion

                #region 老化调试
                var Burn_in_Record = (from m in burn where m.OrderNum == item.OrderNum && (m.OldOrderNum == null || m.OldOrderNum == item.OrderNum) select m).ToList();//查出OrderNum的所有老化记录
                if (Burn_in_Record.Count() > 0)
                {
                    Decimal Burn_in_Normal = Burn_in_Record.Where(m => m.Burn_in_OQCCheckAbnormal == "正常").Count();
                    Decimal Burn_in_Finish = Burn_in_Record.Count(m => m.OQCCheckFinish == true);

                    var Burninvalue = item.Boxes == 0 ? 0 : Math.Round(Convert.ToDecimal(Burn_in_Record.Count()) / item.Boxes * 100, 2);
                    orderinfo.Add("Burn_in_Finish_Rate", Burninvalue > 100 ? 100 : Burninvalue);

                }
                else
                {
                    orderinfo.Add("Burn_in_Finish_Rate", 0);
                }
                #endregion

                #region 包装
                //总箱数
                var totalpackage = package.Where(c => c.OrderNum == item.OrderNum).ToList();
                var num = 0;
                if (totalpackage.Count == 0)
                {
                    totalpackage.ForEach(c => num = num + (c.Quantity * c.OuterBoxCapacity));
                }

                var print = packageprint.Where(c => c.OrderNum == item.OrderNum).Select(c => c.OuterBoxBarcode).Distinct().Count();
                var packagevalue = num == 0 ? 0 : Math.Round((decimal)print * 100 / num, 2);
                orderinfo.Add("Package_Finish_Rate", packagevalue > 100 ? 100 : packagevalue);


                #endregion

                #region 入库
                var warehouse = warehouseinfo.Where(c => c.OrderNum == item.OrderNum && c.State == "模组").Select(c => c.OuterBoxBarcode).Distinct().Count();
                var warehousevalue = num == 0 ? 0 : Math.Round((decimal)warehouse * 100 / num, 2);
                orderinfo.Add("Warehouse_Finish_Rate", warehousevalue > 100 ? 100 : warehousevalue);

                #endregion

                #region 出库
                var warehouseout = warehouseinfo.Where(c => c.OrderNum == item.OrderNum && c.IsOut == true && c.State == "模组").Select(c => c.OuterBoxBarcode).Distinct().Count();
                var warehouseoutvalue = num == 0 ? 0 : Math.Round((decimal)warehouseout * 100 / num, 2);
                orderinfo.Add("WarehouseOut_Finish_Rate", warehouseoutvalue > 100 ? 100 : warehouseoutvalue);
                #endregion
                result.Add(orderinfo);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //合约部查看订单信息历史
        public ActionResult SelectOrderInfoFromContractHistory(string ordernum = null)
        {
            JArray result = new JArray();
            #region 取值
            var fqc = db.FinalQC.Select(c => new { c.OrderNum, c.OldOrderNum, c.FQCCheckFinish, c.RepetitionFQCCheck }).ToList();
            var burn = db.Burn_in.Select(c => new { c.OrderNum, c.OldOrderNum, c.Burn_in_OQCCheckAbnormal, c.OQCCheckFinish }).ToList();
            var package = db.Packing_BasicInfo.Select(c => new { c.OrderNum, c.Quantity, c.OuterBoxCapacity }).ToList();
            var packageprint = db.Packing_BarCodePrinting.Select(c => new { c.OrderNum, c.OuterBoxBarcode }).ToList();
            var warehouseinfo = db.Warehouse_Join.Select(c => new { c.OrderNum, c.OuterBoxBarcode, c.IsOut,c.State }).ToList();
            var smtdate = db.SMT_ProductionData.Select(c => new { c.OrderNum, c.JobContent, c.NormalCount, c.AbnormalCount }).ToList();
            #endregion

            List<OrderMgm> OrderList_All = (from m in db.OrderMgm select m).ToList();
            if (!string.IsNullOrEmpty(ordernum))
            {
                OrderList_All = OrderList_All.Where(c => c.OrderNum == ordernum).ToList();
            }
            foreach (var item in OrderList_All)
            {
                JObject orderinfo = new JObject();
                orderinfo.Add("ordernum", item.OrderNum);
                #region SMT
                var jobcontenlist = smtdate.Where(c => c.OrderNum == item.OrderNum).Select(c => c.JobContent).Distinct().ToList();
                JArray smt = new JArray();
                foreach (var jobconten in jobcontenlist)
                {
                    JObject smtitem = new JObject();
                    var ModelNum = 0;
                    if (jobconten == "灯面" || jobconten == "IC面")
                    {
                        ModelNum = item.Models;
                    }
                    else if (jobconten.Contains("转接卡") == true)
                    {
                        ModelNum = item.AdapterCard;
                    }
                    else if (jobconten.Contains("电源") == true)
                    {
                        ModelNum = item.Powers;
                    }
                    //对应工作内容良品总数
                    var NormalNumSum = smtdate.Where(c => c.OrderNum == item.OrderNum && c.JobContent == jobconten).Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == jobconten).Sum(c => c.NormalCount);

                    //对应工作内容不良品总数
                    var AbnormalNumSum = smtdate.Where(c => c.OrderNum == item.OrderNum && c.JobContent == jobconten).Count() == 0 ? 0 : db.SMT_ProductionData.Where(c => c.OrderNum == item.OrderNum && c.JobContent == jobconten).Sum(c => c.AbnormalCount);

                    smtitem.Add("name", jobconten);
                    //总完成率
                    var SMTvalue = ModelNum == 0 ? 0 : Math.Round(((decimal)(NormalNumSum + AbnormalNumSum)) / ModelNum * 100, 2);
                    smtitem.Add("value", SMTvalue > 100 ? 100 : SMTvalue);

                    smt.Add(smtitem);
                }
                orderinfo.Add("SMT", smt);
                #endregion

                #region 模组
                var FinalQC_Record = fqc.Where(c => c.OrderNum == item.OrderNum && (c.OldOrderNum == null || c.OldOrderNum == item.OrderNum)).ToList();
                if (FinalQC_Record.Count > 0)
                {
                    Decimal FinalQC_Finish = FinalQC_Record.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Count();
                    if (FinalQC_Finish == 0)

                        orderinfo.Add("FinalQC_Finish_Rate", 0);  //FQC完成率
                    else
                    {
                        var Modelvalue = item.Boxes == 0 ? 0 : Math.Round(FinalQC_Finish * 100 / item.Boxes, 2);
                        orderinfo.Add("FinalQC_Finish_Rate", Modelvalue > 100 ? 100 : Modelvalue);

                    }
                }
                else
                {
                    orderinfo.Add("FinalQC_Finish_Rate", 0);  //FQC完成率

                }
                #endregion

                #region 老化调试
                var Burn_in_Record = (from m in burn where m.OrderNum == item.OrderNum && (m.OldOrderNum == null || m.OldOrderNum == item.OrderNum) select m).ToList();//查出OrderNum的所有老化记录
                if (Burn_in_Record.Count() > 0)
                {
                    Decimal Burn_in_Normal = Burn_in_Record.Where(m => m.Burn_in_OQCCheckAbnormal == "正常").Count();
                    Decimal Burn_in_Finish = Burn_in_Record.Count(m => m.OQCCheckFinish == true);

                    var Burninvalue = item.Boxes == 0 ? 0 : Math.Round(Convert.ToDecimal(Burn_in_Record.Count()) / item.Boxes * 100, 2);
                    orderinfo.Add("Burn_in_Finish_Rate", Burninvalue > 100 ? 100 : Burninvalue);

                }
                else
                {
                    orderinfo.Add("Burn_in_Finish_Rate", 0);
                }
                #endregion

                #region 包装
                //总箱数
                var totalpackage = package.Where(c => c.OrderNum == item.OrderNum).ToList();
                var num = 0;
                if (totalpackage.Count == 0)
                {
                    totalpackage.ForEach(c => num = num + (c.Quantity * c.OuterBoxCapacity));
                }

                var print = packageprint.Where(c => c.OrderNum == item.OrderNum).Select(c => c.OuterBoxBarcode).Distinct().Count();
                var packagevalue = num == 0 ? 0 : Math.Round((decimal)print * 100 / num, 2);
                orderinfo.Add("Package_Finish_Rate", packagevalue > 100 ? 100 : packagevalue);


                #endregion

                #region 入库
                var warehouse = warehouseinfo.Where(c => c.OrderNum == item.OrderNum && c.State == "模组").Select(c => c.OuterBoxBarcode).Distinct().Count();
                var warehousevalue = num == 0 ? 0 : Math.Round((decimal)warehouse * 100 / num, 2);
                orderinfo.Add("Warehouse_Finish_Rate", warehousevalue > 100 ? 100 : warehousevalue);

                #endregion

                #region 出库
                var warehouseout = warehouseinfo.Where(c => c.OrderNum == item.OrderNum && c.IsOut == true && c.State == "模组").Select(c => c.OuterBoxBarcode).Distinct().Count();
                var warehouseoutvalue = num == 0 ? 0 : Math.Round((decimal)warehouseout * 100 / num, 2);
                orderinfo.Add("WarehouseOut_Finish_Rate", warehouseoutvalue > 100 ? 100 : warehouseoutvalue);
                #endregion
                result.Add(orderinfo);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //子级模板
        public JObject GetChildrenJobject(string name, int total,int complete,DateTime? completetime,bool ispachage)
        {
            JObject result = new JObject();
            result.Add("name", name);
            if (complete == 0 || total == 0)
            {
                result.Add("valuename", "待投入");
                result.Add("value", "0/" + total);
            }
            else if (complete >= total)
            {
                result.Add("valuename", "已完成");
                result.Add("value", completetime.ToString());
            }
            else if (ispachage)
            {
                result.Add("valuename", "入库中");
                result.Add("value", complete + "/" + total);
            }
            else
            {
                result.Add("valuename", "进行中");
                result.Add("value", complete + "/" + total);
            }
            return result;
        }

        //根据订单的前10个字符给出完整的订单号
        public ActionResult GetOrdernumList(string prefix)
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.OrderNum).Where(c => c.OrderNum.Contains(prefix)).Select(c => c.OrderNum).ToList();
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

    }
}