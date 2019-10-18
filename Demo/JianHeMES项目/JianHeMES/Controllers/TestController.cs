﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json;
using JianHeMES.Areas.KongYaHT.Models;
using Newtonsoft.Json.Linq;
using ZXing;
using System.IO;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Web;
using System.Data;

namespace JianHeMES.Controllers
{
    public class TestController : Controller
    {
        private kongyadbEntities db = new kongyadbEntities();
        private ApplicationDbContext mesdb = new ApplicationDbContext();


        #region------测试上传下载Excel文件

        public ActionResult UploadExcelFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadExcelFile1()
        {
            try
            {
                HttpPostedFileBase uploadfile = Request.Files["fileup"];
                if (uploadfile == null)
                {
                    return Content("no:非法上传");
                }
                if (uploadfile.FileName == "")
                {
                    return Content("no:请选择文件");
                }

                string fileExt = Path.GetExtension(uploadfile.FileName);
                StringBuilder sbtime = new StringBuilder();
                sbtime.Append(DateTime.Now.Year).Append(DateTime.Now.Month).Append(DateTime.Now.Day).Append(DateTime.Now.Hour).Append(DateTime.Now.Minute).Append(DateTime.Now.Second);
                string dir = "/UploadFile/" + sbtime.ToString() + fileExt;
                string realfilepath = Request.MapPath(dir);
                string readDir = Path.GetDirectoryName(realfilepath);
                if (!Directory.Exists(readDir))
                Directory.CreateDirectory(readDir);
                uploadfile.SaveAs(realfilepath);
                //提取数据 
                var dt = ExcelTool.ExcelToDataTable(true, realfilepath);
                List<TU> list = new List<TU>();
                //foreach (DataRow item in dt.Rows)
                //{
                //    list.Add(new TU()
                //    {
                //        No = Convert.ToInt32(item[0]),
                //        Name = item[1].ToString(),
                //        Age = Convert.ToInt32(item[2]),
                //        Sex = item[3].ToString()
                //    });
                //}

                int rcount = dt.Rows.Count;
                for(int i=0;i<rcount;i++)
                {
                    list.Add(new TU()
                    {
                        No = Convert.ToInt32(dt.Rows[i][0]),
                        Name = dt.Rows[i][1].ToString(),
                        Age = Convert.ToInt32(dt.Rows[i][2]),
                        Sex = dt.Rows[i][3].ToString()
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult OutputExcelFile()
        {
            var path = Server.MapPath(@"/content/user.xlsx");

            var dt = ExcelTool.ExcelToDataTable(true, path);
            List<TU> list = new List<TU>();
            foreach (DataRow item in dt.Rows)
            {
                list.Add(new TU()
                {
                    No = Convert.ToInt32(item[0]),
                    Name = item[1].ToString(),
                    Age = Convert.ToInt32(item[2]),
                    Sex = item[3].ToString()
                });
            }

            //datatable
            //DataTable d = new DataTable();
            //d.Columns.Add("No");
            //d.Columns.Add("Name");
            //d.Columns.Add("Age");
            //d.Columns.Add("Sex");
            //foreach (var item in list)
            //{
            //    d.NewRow();
            //    d.Rows.Add(new object[]{item.No,item.Name,item.Age,item.Sex});
            //}
            //创建生成的excel的名称
            StringBuilder sbtime = new StringBuilder();
            sbtime.Append(DateTime.Now.Year).Append(DateTime.Now.Month).Append(DateTime.Now.Day).Append(DateTime.Now.Hour).Append(DateTime.Now.Minute).Append(DateTime.Now.Second);

            var d = DataTableTool.ToDataTableTow(list);
            var url = "/downfile/" + sbtime + ".xls";
            var newpath = Server.MapPath(url);
            ExcelTool.DataTableToExcel(d, "aa", true, newpath);
            //取到生成的名称
            var name = Path.GetFileName(newpath);
            return File(newpath, "application/vnd.ms-excel", name);
        }
        #endregion



        // GET: Test
        public ActionResult KPIstistatics()
        {
            return View();
        }
        [HttpPost]
        //KPI报表
        public ActionResult KPIstistatics(string department, string workshop, string DP_Group, string Name, DateTime? startdate, DateTime? enddate)
        {
            JArray kpi = new JArray();
            List<string> Namelist = new List<string>();
            var asse = mesdb.Personnel_Organization.Where(c => c.SubordinateDepartment == department);//从组织架构表里读取部门数据
            if (Name != null)
            {
                var sw = mesdb.Personnel_Roster.Where(c => c.Name == Name).FirstOrDefault();//从花名册里读取名字
                Namelist.Add(sw.Name);
            }
            else if (DP_Group != null)
            {
                var s = mesdb.Personnel_Roster.Where(c => c.DP_Group == DP_Group).ToList();//从花名册里读取组名
                foreach (var it in s)
                {
                    Namelist.Add(it.Name);
                }
            }
            else if (workshop != null)
            {
                asse = asse.Where(c => c.Superior == workshop);//从组织架构表里读取部门车间
                foreach (var it1 in asse)
                {
                    asse = asse.Where(c => c.Subordinate == it1.Subordinate);//从组织架构表里查找职位名称           
                    string[] strCharArr = it1.Subordinate.Split(',');//拆分it1.Subordinate
                    if (strCharArr.Count() > 1)
                    {
                        var mm = strCharArr[0];
                        var lo = mesdb.Personnel_Roster.Where(c => c.Department == it1.SubordinateDepartment && c.DP_Group == mm);//从花名册里读取跟组织架构一样的部门    
                        foreach (var it2 in lo)
                        {
                            Namelist.Add(it2.Name);
                        }
                    }
                    else
                    {
                        var ml = strCharArr[0];
                        var lo = mesdb.Personnel_Roster.Where(c => c.Department == it1.SubordinateDepartment && c.Position == ml);//从花名册里读取跟组织架构一样的部门
                        foreach (var it3 in lo)
                        {
                            Namelist.Add(it3.Name);
                        }
                    }
                }
            }
            else if (department != null)
            {
                var lp = mesdb.Personnel_Roster.Where(c => c.Department == department);//从花名册里读取部门数据
                foreach (var it4 in lp)
                {
                    Namelist.Add(it4.Name);
                }
            }

            #region-----数据

            #region-----数据表头

            JObject tableheader = new JObject();
            var depe = "部门";
            tableheader.Add("depe", depe);
            var work = "车间";
            if (!String.IsNullOrEmpty(workshop)) { tableheader.Add("work", work); }
            var Group = "小组";
            if (!String.IsNullOrEmpty(DP_Group)) { tableheader.Add("Group", Group); }
            var name = "姓名";
            tableheader.Add("name", name);
            var orde = "订单";
            tableheader.Add("ordernum", orde);
            foreach (var itr in Namelist)
            {
                if (mesdb.Assemble.Count(c => c.AssemblePQCPrincipal == itr && c.PQCCheckBT > startdate && c.PQCCheckFT < enddate) != 0)
                {
                    if (!tableheader.ContainsKey("PQC_assp"))
                    {
                        var PQC_assp = "组装PQC";
                        tableheader.Add("PQC_assp", PQC_assp);
                        var PQC_begin = "开始时间";
                        tableheader.Add("PQC_BeginTime", PQC_begin);
                        var PQC_end = "结束时间";
                        tableheader.Add("PQC_EndTime", PQC_end);
                    }
                }
                if (mesdb.Burn_in_MosaicScreen.Count(c => c.OQCPrincipalNum == itr && c.OQCMosaicStartTime > startdate && c.OQCMosaicEndTime < enddate) != 0)
                {
                    if (!tableheader.ContainsKey("brun_mosa"))
                    {
                        var brun_mosa = "老化拼屏";
                        tableheader.Add("brun_mosa", brun_mosa);
                        var brun_begin = "开始时间";
                        tableheader.Add("brun_begin", brun_begin);
                        var brun_end = "结束时间";
                        tableheader.Add("brun_end", brun_end);
                    }
                }
                else if (mesdb.CalibrationRecord.Count(c => c.CalibrationTimeSpan == itr && c.BeginCalibration > startdate && c.FinishCalibration < enddate) != 0)
                {
                    if (!tableheader.ContainsKey("calib"))
                    {
                        var calib = "校正";
                        tableheader.Add("calib", calib);
                        var cali_begin = "开始时间";
                        tableheader.Add("cali_begin", cali_begin);
                        var cali_end = "结束时间";
                        tableheader.Add("cali_end", cali_end);
                    }
                }
                else if (mesdb.CourtScreenModuleInfoes.Count(c => c.Creater == itr && c.CreateDate > startdate && c.CreateDate < enddate) != 0)
                {
                    if (!tableheader.ContainsKey("senn"))
                    {
                        var senn = "球场屏";
                        tableheader.Add("senn", senn);
                        var senn_begin = "录入时间";
                        tableheader.Add("senn_begin", senn_begin);
                    }
                }
                else if (mesdb.FinalQC.Count(c => c.FQCPrincipal == itr && c.FQCCheckBT > startdate && c.FQCCheckFT < enddate) != 0)
                {
                    if (!tableheader.ContainsKey("fqqc"))
                    {
                        var fqqc = "FQC抽检";
                        tableheader.Add("fqqc", fqqc);
                        var fqc_begin = "开始时间";
                        tableheader.Add("fqc_begin", fqc_begin);
                        var fqc_end = "结束时间";
                        tableheader.Add("fqc_end", fqc_end);
                    }
                }
                else if (mesdb.Burn_in.Count(c => c.OQCPrincipal == itr && c.OQCCheckBT > startdate && c.OQCCheckFT < enddate) != 0)
                {
                    if (!tableheader.ContainsKey("brun"))
                    {
                        var brun = "老化OQC";
                        tableheader.Add("brun", brun);
                        var in_begin = "开始时间";
                        tableheader.Add("in_begin", in_begin);
                        var in_end = "结束时间";
                        tableheader.Add("in_end", in_end);
                    }
                }
                else if (mesdb.Appearance.Count(c => c.OQCPrincipal == itr && c.OQCCheckBT > startdate && c.OQCCheckFT < enddate) != 0)
                {
                    if (!tableheader.ContainsKey("appere"))
                    {
                        var appere = "外观电检";
                        tableheader.Add("appere", appere);
                        var app_begin = "开始时间";
                        tableheader.Add("app_begin", app_begin);
                        var app_end = "结束时间";
                        tableheader.Add("app_end", app_end);
                    }
                }
            }
            kpi.Add(tableheader);
            var aa = JsonConvert.SerializeObject(kpi);
            #endregion

            #region-----组装PQC       

            if (aa.Contains("组装PQC"))
            {
                var ass = mesdb.Assemble.Where(c => Namelist.Contains(c.AssemblePQCPrincipal) && c.PQCCheckBT > startdate && c.PQCCheckFT < enddate).ToList();
                var ordernumlist = ass.Select(c => c.OrderNum).Distinct().ToList();//按照组装PQC的操作员读取订单表的数据          
                foreach (var te in Namelist)//Namelist是人名清单
                {
                    var count = ass.Count(c => c.AssemblePQCPrincipal == te);
                    if (count > 0)
                    {
                        //count_by_name.Add(te + "记录:" + count + ",订单数:" + ass.Where(c => c.AssemblePQCPrincipal == te).Select(c => c.OrderNum).Distinct().Count());
                        var record_list = ass.Where(c => c.AssemblePQCPrincipal == te);
                        var order_list = ass.Where(c => c.AssemblePQCPrincipal == te).Select(c => c.OrderNum).Distinct();
                        foreach (var order in order_list)
                        {
                            JObject order_record = new JObject();
                            order_record.Add("depe", department);
                            if (!String.IsNullOrEmpty(workshop)) { order_record.Add("work", workshop); }
                            if (!String.IsNullOrEmpty(DP_Group)) { order_record.Add("Group", DP_Group); }
                            order_record.Add("name", te);
                            order_record.Add("ordernum", order);
                            order_record.Add("PQC_BeginTime", record_list.Min(c => c.PQCCheckBT).ToString());
                            order_record.Add("PQC_EndTime", record_list.Max(c => c.PQCCheckFT).ToString());
                            var abnormalcount = record_list.Count(c => c.OrderNum == order & c.PQCCheckAbnormal != "正常" & c.PQCCheckAbnormal != null);
                            order_record.Add("PQC_assp", record_list.Count(c => c.OrderNum == order).ToString() + (abnormalcount > 0 ? ("/" + abnormalcount.ToString()) : ""));
                            //count_by_name.Add(order_record);
                            kpi.Add(order_record);
                            order_record = new JObject();
                        }
                    }
                }
            }
            #endregion

            #region-----老化拼屏
            if (aa.Contains("老化拼屏"))
            {
                var Mosaic = mesdb.Burn_in_MosaicScreen.Where(c => Namelist.Contains(c.OQCPrincipalNum) && c.OQCMosaicStartTime > startdate && c.OQCMosaicEndTime < enddate).ToList();//从老化拼屏表里读取负责拼屏的操作员
                var ordScreen = Mosaic.Select(c => c.OrderNum).Distinct().ToList();//按照老化拼屏的操作员读取订单表的数据
                foreach (var te1 in Namelist)
                {
                    var mosai = Mosaic.Count(c => c.OQCPrincipalNum == te1);
                    if (mosai > 0)
                    {
                        //mosa_name.Add(te1 + "记录:" + mosai + ",订单数:" + Mosaic.Where(c => c.OQCPrincipalNum == te1).Select(c => c.OrderNum).Distinct().Count());
                        var scre_brun = Mosaic.Where(c => c.OQCPrincipalNum == te1);
                        var brun_order = Mosaic.Where(c => c.OQCPrincipalNum == te1).Select(c => c.OrderNum).Distinct();
                        foreach (var reder in brun_order)
                        {
                            JObject brun_recod = new JObject();
                            brun_recod.Add("depe", department);
                            if (!String.IsNullOrEmpty(workshop)) { brun_recod.Add("work", workshop); }
                            if (!String.IsNullOrEmpty(DP_Group)) { brun_recod.Add("Group", DP_Group); }
                            brun_recod.Add("name", te1);
                            brun_recod.Add("ordernum", reder);
                            brun_recod.Add("brun_begin", scre_brun.Min(c => c.OQCPrincipalNum).ToString());
                            brun_recod.Add("brun_end", scre_brun.Max(c => c.OQCPrincipalNum).ToString());
                            brun_recod.Add("brun_mosa", scre_brun.Count(c => c.OrderNum == reder).ToString());
                            kpi.Add(brun_recod);
                            brun_recod = new JObject();
                        }
                    }
                }
            }
            #endregion

            #region-----校正
            if (aa.Contains("校正"))
            {
                var Calibra = mesdb.CalibrationRecord.Where(c => Namelist.Contains(c.Operator) && c.BeginCalibration > startdate && c.FinishCalibration < enddate).ToList();
                var ord_cali = Calibra.Select(c => c.OrderNum).Distinct().ToList();//按照校正的操作员读取订单表里的数据
                foreach (var te2 in Namelist)
                {
                    var record = Calibra.Count(c => c.Operator == te2);
                    if (record > 0)
                    {
                        //calib_count.Add(te2 + "记录:" + record + ",订单数:" + Calibra.Where(c => c.Operator == te2).Select(c => c.OrderNum).Distinct().Count());
                        var cortion = Calibra.Where(c => c.Operator == te2);
                        var cali_ordernum = Calibra.Where(c => c.Operator == te2).Select(c => c.OrderNum).Distinct();
                        foreach (var caliorder in cali_ordernum)
                        {
                            JObject record_cali = new JObject();
                            record_cali.Add("depe", department);
                            if (!String.IsNullOrEmpty(workshop)) { record_cali.Add("work", workshop); }
                            if (!String.IsNullOrEmpty(DP_Group)) { record_cali.Add("Group", DP_Group); }
                            record_cali.Add("name", te2);
                            record_cali.Add("ordernum", caliorder);
                            record_cali.Add("cali_begin", cortion.Min(c => c.BeginCalibration).ToString());
                            record_cali.Add("cali_end", cortion.Max(c => c.FinishCalibration).ToString());
                            var abnormalcount = cortion.Count(c => c.AbnormalDescription != "正常" & c.AbnormalDescription != null);
                            record_cali.Add("calib", cortion.Count(c => c.OrderNum == caliorder).ToString() + (abnormalcount > 0 ? "/" + abnormalcount.ToString() : ""));
                            kpi.Add(record_cali);
                            record_cali = new JObject();
                        }
                    }
                }
            }
            #endregion

            #region-----球场屏            
            if (aa.Contains("球场屏"))
            {
                var CourtScreen = mesdb.CourtScreenModuleInfoes.Where(c => Namelist.Contains(c.Creater) && c.CreateDate > startdate && c.CreateDate < enddate).ToList();
                var orde_Screen = CourtScreen.Select(c => c.OrderNum).Distinct().ToList();//按照球场屏表的录入人读取订单表里的数据
                foreach (var te3 in Namelist)
                {
                    var module = CourtScreen.Count(c => c.Creater == te3);
                    if (module > 0)
                    {
                        //Screen_count.Add(te3 + "记录:" + module + ",订单数:" + CourtScreen.Where(c => c.Creater == te3).Select(c => c.OrderNum).Distinct().Count());
                        var couyrscr = CourtScreen.Where(c => c.Creater == te3);
                        var screenn_order = CourtScreen.Where(c => c.Creater == te3).Select(c => c.OrderNum).Distinct();
                        foreach (var order_scree in screenn_order)
                        {
                            JObject court_record = new JObject();
                            court_record.Add("depe", department);
                            if (!String.IsNullOrEmpty(workshop)) { court_record.Add("work", workshop); }
                            if (!String.IsNullOrEmpty(DP_Group)) { court_record.Add("Group", DP_Group); }
                            court_record.Add("name", te3);
                            court_record.Add("ordernum", order_scree);
                            court_record.Add("senn_begin", CourtScreen.Select(c => c.CreateDate).FirstOrDefault().ToString());
                            court_record.Add("senn", CourtScreen.Count(c => c.OrderNum == order_scree));
                            kpi.Add(court_record);
                            court_record = new JObject();
                        }
                    }
                }
            }
            #endregion

            #region-----FQC抽检
            if (aa.Contains("FQC抽检"))
            {
                var finalqc = mesdb.FinalQC.Where(c => Namelist.Contains(c.FQCPrincipal) && c.FQCCheckBT > startdate && c.FQCCheckFT < enddate).ToList(); //从FQC表里读取负责FQC的检验员
                var ordeFQC = finalqc.Select(c => c.OrderNum).Distinct().ToList();//按照FQC表的检验员读取订单表里的数据
                foreach (var te4 in Namelist)
                {
                    var fqc = finalqc.Count(c => c.FQCPrincipal == te4);
                    if (fqc > 0)
                    {
                        //FQC_name.Add(te4 + "记录:" + fqc + ",订单数:" + finalqc.Where(c => c.FQCPrincipal == te4).Select(c => c.OrderNum).Distinct().Count());
                        var fqc_recod = finalqc.Where(c => c.FQCPrincipal == te4);
                        var fqc_numorder = finalqc.Where(c => c.FQCPrincipal == te4).Select(c => c.OrderNum).Distinct();
                        foreach (var order_FQC in fqc_numorder)
                        {
                            JObject fqc_list = new JObject();
                            fqc_list.Add("depe", department);
                            if (!String.IsNullOrEmpty(workshop)) { fqc_list.Add("work", workshop); }
                            if (!String.IsNullOrEmpty(DP_Group)) { fqc_list.Add("Group", DP_Group); }
                            fqc_list.Add("name", te4);
                            fqc_list.Add("ordernum", order_FQC);
                            fqc_list.Add("fqc_begin", finalqc.Min(c => c.FQCCheckBT).ToString());
                            fqc_list.Add("fqc_end", finalqc.Max(c => c.FQCCheckFT).ToString());
                            var abnormalcount = finalqc.Count(c => c.OrderNum == order_FQC & c.FinalQC_FQCCheckAbnormal != "正常" & c.FinalQC_FQCCheckAbnormal != null);
                            fqc_list.Add("fqqc", finalqc.Count(c => c.OrderNum == order_FQC).ToString() + (abnormalcount > 0 ? ("/" + abnormalcount.ToString()) : ""));
                            kpi.Add(fqc_list);
                            fqc_list = new JObject();
                        }
                    }
                }
            }
            #endregion

            #region-----老化OQC
            if (aa.Contains("老化OQC"))
            {
                var brun_recode = mesdb.Burn_in.Where(c => Namelist.Contains(c.OQCPrincipal) && c.OQCCheckBT > startdate && c.OQCCheckFT < enddate).ToList();//从老化OQC表里读取负责老化OQC的操作员
                var Bun_inorde = brun_recode.Select(c => c.OrderNum).Distinct().ToList();//按照老化OQC表的操作员读取订单表里的数据
                foreach (var te5 in Namelist)
                {
                    var brun_in = brun_recode.Count(c => c.OQCPrincipal == te5);
                    if (brun_in > 0)
                    {
                        //Brun_in_name.Add(te5 + "记录:" + brun_in + ",订单数:" + brun_recode.Where(c => c.OQCPrincipal == te5).Select(c => c.OrderNum).Distinct().Count());
                        var brun_order = brun_recode.Where(c => c.OQCPrincipal == te5);
                        var recou_brun = brun_recode.Where(c => c.OQCPrincipal == te5).Select(c => c.OrderNum).Distinct();
                        foreach (var order_brun in recou_brun)
                        {
                            JObject brun_list = new JObject();
                            brun_list.Add("depe", department);
                            if (!String.IsNullOrEmpty(workshop)) { brun_list.Add("work", workshop); }
                            if (!String.IsNullOrEmpty(DP_Group)) { brun_list.Add("Group", DP_Group); }
                            brun_list.Add("name", te5);
                            brun_list.Add("ordernum", order_brun);
                            brun_list.Add("in_begin", brun_recode.Min(c => c.OQCCheckBT).ToString());
                            brun_list.Add("in_end", brun_recode.Max(c => c.OQCCheckFT).ToString());
                            var abnormalcount = brun_recode.Count(c => c.OrderNum == order_brun & c.Burn_in_OQCCheckAbnormal != "正常" & c.Burn_in_OQCCheckAbnormal != null);
                            brun_list.Add("brun", brun_recode.Count(c => c.OrderNum == order_brun).ToString() + (abnormalcount > 0 ? ("/" + abnormalcount.ToString()) : ""));
                            kpi.Add(brun_list);
                            brun_list = new JObject();
                        }
                    }
                }
            }
            #endregion

            #region-----外观电检
            if (aa.Contains("外观电检"))
            {
                var appearance = mesdb.Appearance.Where(c => Namelist.Contains(c.OQCPrincipal) && c.OQCCheckBT > startdate && c.OQCCheckFT < enddate).ToList();//从外观点检表里读取负责外观点检的操作员
                var app_reoude = appearance.Select(c => c.OrderNum).Distinct().ToList();//按照从外观点检表的操作员读取订单表里的数据
                foreach (var te6 in Namelist)
                {
                    var appear = appearance.Count(c => c.OQCPrincipal == te6);
                    if (appear > 0)
                    {
                        //Appearance_count.Add(te6 + "记录:" + appear + ",订单数:" + appearance.Where(c => c.OQCPrincipal == te6).Select(c => c.OrderNum).Distinct().Count());
                        var apper_list = appearance.Where(c => c.OQCPrincipal == te6);
                        var appearan_roce = appearance.Where(c => c.OQCPrincipal == te6).Select(c => c.OrderNum).Distinct();
                        foreach (var apper_ordernum in appearan_roce)
                        {
                            JObject apper_recode = new JObject();
                            apper_recode.Add("depe", department);
                            if (!String.IsNullOrEmpty(workshop)) { apper_recode.Add("work", workshop); }
                            if (!String.IsNullOrEmpty(DP_Group)) { apper_recode.Add("Group", DP_Group); }
                            apper_recode.Add("name", te6);
                            apper_recode.Add("ordernum", apper_ordernum);
                            apper_recode.Add("app_begin", appearance.Min(c => c.OQCCheckBT).ToString());
                            apper_recode.Add("app_end", appearance.Max(c => c.OQCCheckFT).ToString());
                            var abnormalcount = appearance.Count(c => c.OrderNum == apper_ordernum & c.Appearance_OQCCheckAbnormal != "正常" & c.Appearance_OQCCheckAbnormal != null);
                            apper_recode.Add("appere", appearance.Count(c => c.OrderNum == apper_ordernum).ToString() + (abnormalcount > 0 ? ("/" + abnormalcount.ToString()) : ""));
                            kpi.Add(apper_recode);
                            apper_recode = new JObject();
                        }
                    }
                }
            }
            #endregion

            return Content(JsonConvert.SerializeObject(kpi));

        }
        #endregion


        //public ActionResult b_a_list()   //没老化就包装
        //{
        //    string ordernum = "2018-YA403-4";
        //    var list = mesdb.BarCodes.Where(c => c.OrderNum == ordernum).ToList();
        //    List<string> barcodeslist = new List<string>();
        //    var appearancefinistlist = mesdb.Appearance.Where(c => c.OrderNum == ordernum && c.OQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
        //    var burn_in = mesdb.Burn_in.Where(c => c.OrderNum == ordernum && c.OQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
        //    var result = appearancefinistlist.Except(burn_in).ToList();
        //    ViewBag.resultlist = result;
        //    return View(result);
        //}

        public ActionResult b_a_list()   //老化未完成　就校正
        {

            return View();
        }


        [HttpPost]
        public ActionResult b_a_list(string ordernum)
        {
            var list = mesdb.BarCodes.Where(c => c.OrderNum == ordernum).ToList();
            List<string> barcodeslist = new List<string>();
            var appearancefinistlist = mesdb.Appearance.Where(c => c.OrderNum == ordernum && c.OQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
            var burn_in = mesdb.Burn_in.Where(c => c.OrderNum == ordernum && c.OQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
            var result = appearancefinistlist.Except(burn_in).ToList();
            ViewBag.resultlist = result;
            return View(result);
        }


        public ActionResult CSVtest()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CSVtest(Array file)
        {

            return View();
        }


        [HttpPost]
        public ActionResult Index(string jsondata)   //接收jsondata
        {
            CalibrationRecord data = JsonConvert.DeserializeObject<CalibrationRecord>(jsondata);   //使用JsonConvert.DeserializeObject<类名>（字符串）来把传过来的字符串解析为对应的类
            return View();
        }

        [HttpPost]
        public ActionResult ElectVal()   //接收jsondata
        {
            var value = db.aircomp3.OrderByDescending(c => c.recordingTime).FirstOrDefault().current_u.ToString();
            return Content(value);
        }


        [HttpPost]
        public ActionResult HTChartsLeft(string point, DateTime left)
        {
            IQueryable<THhistory> queryRecords = null;

            queryRecords = (from m in db.THhistory
                            where (m.DeviceID == "40004493" && m.NodeID == "1" && m.RecordTime < left)
                            orderby m.id descending
                            select m).Take(100).OrderBy(m => m.RecordTime);
            if (queryRecords.Count() == 0)
            {
                return Content("无数据");
            }
            ViewBag.Station = queryRecords.FirstOrDefault().DeviceName;
            queryRecords.Select(m => new { m.id, m.DeviceID, m.NodeID, m.Tem, m.Hum, m.RecordTime, m.DeviceName });

            #region ---------------将对象转为列矩阵JSON
            List<Double> TemList = new List<double>();
            List<Double> HumList = new List<double>();
            List<DateTime> RecordTimeList = new List<DateTime>();
            foreach (var firstRecord in queryRecords)
            {
                TemList.Add(Convert.ToDouble(firstRecord.Tem));
                HumList.Add(Convert.ToDouble(firstRecord.Hum));
                RecordTimeList.Add(Convert.ToDateTime(firstRecord.RecordTime));
            }
            var iso = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            iso.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            JObject queryJsonObj = new JObject
            {
                { "Tem", JsonConvert.SerializeObject(TemList) },
                { "Hum", JsonConvert.SerializeObject(HumList) },
                { "RecordTime", JsonConvert.SerializeObject(RecordTimeList,iso).Replace("\"","")},
            };   //创建JSON对象
            #endregion

            ViewData["queryJsonObj"] = queryJsonObj;  //输出JSON
            return Content(JsonConvert.SerializeObject(queryJsonObj));
        }


        #region 组织架构框架输出
        [HttpPost]
        public ActionResult Framework()
        {
            var result = new JObject();
            JArray b = new JArray();
            result.Add("name", "All");
            result.Add("userid", b);
            result.Add("children", b);
            var All = Find_list("All");
            if (All.Count > 0)
            {
                result["children"] = Foreach_iterator(All);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //查询
        public List<string> Find_list(string str)
        {
            var result = mesdb.Tests.Where(c => c.OrderNum == str).Select(c => c.JobContent).ToList();
            return result;
        }
        //查找角色ID，需要角色+角色部门
        public JArray FindUserID(string str)
        {
            JArray aa = new JArray();
            var result = mesdb.Tests.Where(c => c.JobContent == str).Select(c => c.LineNum).ToList();
            aa.Add(result);
            return aa;
        }

        //迭代器
        public JArray Foreach_iterator(List<string> list_str)
        {
            var result = new JArray();
            if (list_str.Count > 0)
            {
                var obj_result = new JArray();
                var b = new JArray();
                var a = new JObject();
                foreach (var i in list_str)
                {
                    a.Add("name", i);
                    a.Add("userid", b);
                    a.Add("children", b);
                    obj_result.Add(a);
                    if (Find_list(i).Count > 0)
                    {
                        a["children"] = Foreach_iterator(Find_list(i));
                    }
                    else
                    {
                        a["userid"] = FindUserID(i);
                    }
                    a = new JObject();
                }
                return obj_result;
            }
            else return result;
        }
        #endregion


        #region 测试输出条码图形打印输出
        public ActionResult printTest()
        {
            return View();
        }
        [HttpPost]
        public ActionResult outputBitmap(string bitmap)
        {
            string ptrn = "InsideBoxLablePrinter";
            Bitmap theBitmap = ZebraUnity.Base64ToBitmap(bitmap);//把base64字符串转换成Bitmap
            string sb = "^XA~DGR:ZONE.GRF,";
            Bitmap bm = new Bitmap(ConvertTo1Bpp1(ToGray(theBitmap)));
            MemoryStream ms = new MemoryStream();
            theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            int totalbytes = bm.ToString().Length;
            int rowbytes = 10;
            string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
            sb += totalbytes + "," + rowbytes + "," + hex;
            sb += "^LH0,0^FO170,10^XGR:ZONE.GRF^FS^XZ";
            bool result = BarCodeLablePrint.SendStringToPrinter(ptrn, sb.ToString());            //图片转成8进制，直接生成相对应的ZPL
            theBitmap.Dispose();
            return File(ms.ToArray(), "image/Png");
        }

        #region 图像灰度、灰度反转、二值化处理

        /// <summary>
        /// 图像灰度化
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ToGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }


        /// <summary>
        /// 图像灰度反转
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap GrayReverse(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    Color newColor = Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }

        /// <summary>
        /// 图像二值化1：取图片的平均灰度作为阈值，低于该值的全都为0，高于该值的全都为255
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo1Bpp1(Bitmap bmp)
        {
            int average = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    average += color.B;
                }
            }
            average = (int)average / (bmp.Width * bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    int value = 255 - color.B;
                    Color newColor = value > average ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }

        /// <summary>
        /// 图像二值化2
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo1Bpp2(Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
            for (int y = 0; y < h; y++)
            {
                byte[] scan = new byte[(w + 7) / 8];
                for (int x = 0; x < w; x++)
                {
                    Color c = img.GetPixel(x, y);
                    if (c.GetBrightness() >= 0.5) scan[x / 8] |= (byte)(0x80 >> (x % 8));
                }
                Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);
            }
            return bmp;
        }


        #endregion


        #region 十六进制和十进制互转　ARGB转十六进制string
        ///
        ///ARGB转十六进制string
        /// <param value="ARGB"></param>
        /// <returns>string</returns> 
        /// 



        /// <summary>
        /// 十六进制转换到十进制
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static string Hex2Ten(string hex)
        {
            int ten = 0;
            for (int i = 0, j = hex.Length - 1; i < hex.Length; i++)
            {
                ten += HexChar2Value(hex.Substring(i, 1)) * ((int)Math.Pow(16, j));
                j--;
            }
            return ten.ToString();
        }

        public static int HexChar2Value(string hexChar)
        {
            switch (hexChar)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    return Convert.ToInt32(hexChar);
                case "a":
                case "A":
                    return 10;
                case "b":
                case "B":
                    return 11;
                case "c":
                case "C":
                    return 12;
                case "d":
                case "D":
                    return 13;
                case "e":
                case "E":
                    return 14;
                case "f":
                case "F":
                    return 15;
                default:
                    return 0;
            }
        }

        //使用
        //this.txtStartShi.Text = Hex2Ten(this.txtStartSN.Text.Trim().Substring(4, 8));
        //this.txtEndShi.Text = Hex2Ten(this.txtEndSN.Text.Trim().Substring(4, 8));


        /// <summary>
        /// 从十进制转换到十六进制
        /// </summary>
        /// <param name="ten"></param>
        /// <returns></returns>
        public static string Ten2Hex(string ten)
        {
            ulong tenValue = Convert.ToUInt64(ten);
            ulong divValue, resValue;
            string hex = "";
            do
            {
                //divValue = (ulong)Math.Floor(tenValue / 16);

                divValue = (ulong)Math.Floor((decimal)(tenValue / 16));

                resValue = tenValue % 16;
                hex = tenValue2Char(resValue) + hex;
                tenValue = divValue;
            }
            while (tenValue >= 16);
            if (tenValue != 0)
                hex = tenValue2Char(tenValue) + hex;
            return hex;
        }

        public static string tenValue2Char(ulong ten)
        {
            switch (ten)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return ten.ToString();
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
                default:
                    return "";
            }
        }

        //使用
        //int StartSN = Convert.ToInt32(this.txtStartShi.Text.Trim());
        //int EndSN = Convert.ToInt32(this.txtEndShi.Text.Trim());
        //for (int i = 0; i<EndSN - StartSN + 1; i++)
        //{
        //  listBox1.Items.Add("0014" + Ten2Hex((Convert.ToDouble(this.txtStartShi.Text) + i).ToString()));
        //}



        /// <summary>
        /// 10进制转34进制
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string Ten2ThirtyFour(int parameter)
        {
            string[] radix = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D",
                                 "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T",
                                 "U", "V", "W", "X", "Y", "Z" };
            string result = "";
            int len = 0;
            int remainder = 0;
            len = parameter / 34;
            remainder = parameter % 34;
            result = radix[remainder];
            while (len > 0)
            {
                remainder = len % 34;
                len = len / 34;
                result = radix[remainder] + result;
            }
            return result;
        }

        #endregion

        [HttpPost]
        public ActionResult printTest2(string bitmap)
        {
            string ptrn = "InsideBoxLablePrinter";
            ////int totalbytes = 64800;
            //int totalbytes = bitmap.Length;
            //int rowbytes = 10;
            //Bitmap theBitmap = ZebraUnity.Base64ToBitmap(bitmap);
            //string hex = ZebraUnity.BitmapToHex(theBitmap, out totalbytes, out rowbytes);//将图片转成ASCii码
            //string mubanstring = "~DGR:LABLE.GRF," + totalbytes.ToString() + "," + rowbytes.ToString() + "," + hex;//将图片生成模板指令
            //string otherstring = "^XA^FO220,50^XGR:LABLE.GRF,1.5,1.5^FS";//调用该模板指令
            //otherstring += "^XZ";
            //string data = mubanstring + otherstring;


            Bitmap theBitmap = ZebraUnity.Base64ToBitmap(bitmap);//把base64字符串转换成Bitmap
            Bitmap bitmap2 = ZebraUnity.ConvertToGrayscale(theBitmap);//获取单色位图数据
            int totalbytes = bitmap2.ToString().Length;
            int rowbytes = 10;
            string data = "^XA^FO220,50^XGR:LABLE.GRF,1.5,1.5^FS";
            string hex = ZebraUnity.BmpToZpl(bitmap2, out totalbytes, out rowbytes);
            //string cont = ZebraUnity.BitmapToHex(theBitmap, out int to, out int row);//将图片转成ASCii码
            //string zpl = string.Format("~DGR:Temp0.GRF,{0},{1},{2}", to, row, cont);
            //data = zpl;
            //data += "^FO220,5^XGR:Temp0.GRF,2,2^FS";
            data = data + hex;
            data += "^XZ";
            bool result = BarCodeLablePrint.SendStringToPrinter(ptrn, data);

            //NetPOSPrinter.PrintLine("112233");
            //string result = NetPOSPrinter.PrintInsideLablePic(bitmap2);
            //int a = 0;
            return View();
        }


        [HttpPost]
        public ActionResult printTest(string barcode, string modulenum)
        {
            string ptrn = "InsideBoxLablePrinter";

            //Bitmap bmp = ZebraUnity.Base64ToBitmap(data);
            string data_s = "^XA";//开始
            string modulenum_data = "^A@N,80,56,B:ST.FNT^BCN,0,Y,N,N^FO250,30BY20^FD" + modulenum + "^FS";//,B:ST.FNT
                                                                                                          ////string modulenum_data = "^A@N,80,56,B:ST.mmf^BCN,0,Y,N,N^FO270,30BY20^FD" + modulenum + "^FS";//,B:ST.FNT
                                                                                                          //string modulenum_data =ZebraPrintHelper.ConvertChineseToHex(modulenum,"tempName","宋体");
            string barcode_data = "^FO210,115BY50^BCN,80,Y,N,N^A@N,30,21,B:Arial.FNT^FD" + barcode + "^FS";//monospace,ARIALR.FNT
            string data_e = "^XZ";
            bool result = BarCodeLablePrint.SendStringToPrinter(ptrn, data_s + modulenum_data + barcode_data + data_e);
            //result = BarCode.SendStringToPrinter(ptrn, data_s + modulenum_data + barcode_data + data_e);
            return View();

        }

        public Bitmap BarCodeToImg(string code, int width, int height)
        {
            BarcodeWriter barCodeWriter = new BarcodeWriter();
            barCodeWriter.Format = BarcodeFormat.CODE_128;    //条形码规格：EAN13规格：12（无校验位）或13位数字
            barCodeWriter.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            barCodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
            barCodeWriter.Options.Height = height;
            barCodeWriter.Options.Width = width;
            barCodeWriter.Options.Margin = 0;
            ZXing.Common.BitMatrix bm = barCodeWriter.Encode(code);
            Bitmap img = barCodeWriter.Write(bm);    // 生成图片
                                                     //MemoryStream ms = new MemoryStream();
                                                     //img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                                     //img.Dispose();
                                                     //return File(ms.ToArray(), "image/jpeg");
            return img;
        }


        public ActionResult BarCode()
        {
            return View();
        }

        public ActionResult BarCodeImg(string code, int width, int height)
        {
            BarcodeWriter barCodeWriter = new BarcodeWriter();
            barCodeWriter.Format = BarcodeFormat.CODE_128;    //条形码规格：EAN13规格：12（无校验位）或13位数字
            barCodeWriter.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            barCodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
            barCodeWriter.Options.Height = height;
            barCodeWriter.Options.Width = width;
            barCodeWriter.Options.Margin = 0;
            ZXing.Common.BitMatrix bm = barCodeWriter.Encode(code);
            Bitmap img = barCodeWriter.Write(bm);    // 生成图片
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            img.Dispose();
            return File(ms.ToArray(), "image/jpeg");
        }
        public ActionResult QRCodeImg(string code, int width, int height)
        {
            BarcodeWriter barCodeWriter = new BarcodeWriter();
            barCodeWriter.Format = BarcodeFormat.QR_CODE;
            barCodeWriter.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            barCodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
            barCodeWriter.Options.Height = height;
            barCodeWriter.Options.Width = width;
            barCodeWriter.Options.Margin = 0;
            ZXing.Common.BitMatrix bm = barCodeWriter.Encode(code);
            Bitmap img = barCodeWriter.Write(bm);
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            img.Dispose();
            return File(ms.ToArray(), "image/jpeg");
        }
        public ActionResult QRCode()
        {
            return View();
        }
        #endregion


        //private Bitmap getSingleColorBitmap(Bitmap bitmap, @ColorInt int bgColor)
        //{
        //    int[] pix = new int[bitmap.Size.Width * bitmap.Size.Height];
        //    bool isBitmapEmpty = true;
        //    for (int y = 0; y < bitmap.Size.Height; y++)
        //    {
        //        for (int x = 0; x < bitmap.Size.Width ; x++)
        //        {
        //            int index = y * bitmap.Size.Width + x;
        //            pix[index] = Color.White.ToArgb();
        //            if (bitmap.GetPixel(x, y).ToArgb()!= bgColor)
        //            {
        //                isBitmapEmpty = false;
        //                pix[index] = Color.Black.ToArgb();
        //            }
        //        }
        //    }
        //    if (isBitmapEmpty)
        //    {
        //        return null;
        //    }
        //    bitmap.SetPixel(pix, 0, bitmap.Size.Width, 0, 0, bitmap.Size.Width, bitmap.Size.Height);
        //    return bitmap;
        //}

    }
}
