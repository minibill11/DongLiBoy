﻿using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class Process_CapacityController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private CommonalityController comm = new CommonalityController();
        private CommonController com = new CommonController();

        #region 页面
        //工序产能首页
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "Index" });
            }
            return View();
        }
        //工序产能二级页
        public ActionResult Index2()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "Index2" });
            }
            return View();
        }

        //工序产能首页（旧）
        public ActionResult Index3()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "Index3" });
            }
            return View();
        }

        //工序产能首页（操作）
        public ActionResult Index4()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "Index4" });
            }
            return View();
        }
        //工序产能详细页
        public ActionResult Detail()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "Detail" });
            }
            return View();
        }
        //标准产能
        public ActionResult StandardCapacity()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "StandardCapacity" });
            }
            return View();
        }
        //工序平衡卡
        public ActionResult ProcessBalanceCard()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "ProcessBalanceCard" });
            }
            return View();
        }
        #endregion

        #region 临时类，用于总表JSON文件内容
        public class TempSmt
        {
            public int id { get; set; }
            public string Type { get; set; }
            public string PCB { get; set; }
            public string Plafrom { get; set; }
            public string PlafModule { get; set; }
            public string Maintenance { get; set; }
            public string Section { get; set; }
            public string ExaminamMessage { get; set; }
            public string ProcessDescription { get; set; }
            public int PersonNum { get; set; }
            public decimal CapacityPerHour { get; set; }
        }

        public class TempCapacity
        {
            // c.Id, c.Type, c.ProductPCBnumber, c.Platform, c.StandardCapacity , c.StandardNumber,c.Name,c.Name2,c.StandardNumber2,c.StandardCapacity2,  c.PlatformModul,c.Maintenance
            public int Id { get; set; }
            public string Type { get; set; }
            public string ProductPCBnumber { get; set; }
            public string Platform { get; set; }
            public string PlatformModul { get; set; }
            public string Maintenance { get; set; }
            public string Section { get; set; }
            public string Name { get; set; }
            public string Name2 { get; set; }
            public decimal StandardCapacity { get; set; }
            public decimal StandardCapacity2 { get; set; }
            public int StandardNumber { get; set; }
            public int StandardNumber2 { get; set; }
            public int ScrewMachineNum { get; set; }
            public int DispensMachineNum { get; set; }
            public double SingleLampWorkingHours { get; set; }
            public int PCBASingleLampNumber { get; set; }

        }
        public class TempBlance
        {
            public string Type { get; set; }
            public string PCB { get; set; }
            public string Plafrom { get; set; }
            public string PlafModule { get; set; }
            public string Maintenance { get; set; }
            public string Section { get; set; }
            public string SerialNumber { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public int StandardTotal { get; set; }
            public decimal StandardCapacity { get; set; }
            public decimal ModuleNeedNum { get; set; }
            public decimal StandardHourlyOutputPerCapita { get; set; }
            public bool Pdf { get; set; }
            public bool Jpg { get; set; }
            public int ScrewMachineNum { get; set; }
            public int DispensMachineNum { get; set; }
            public decimal BalanceRate { get; set; }
            public decimal Bottleneck { get; set; }


        }

        public class TempEquipment
        {
            public int SeactionID { get; set; }
            public string Seaction { get; set; }
            public string Name { get; set; }
            public decimal Capacity { get; set; }
            public int Number { get; set; }
            public string Statue { get; set; }
            public int PersonEquipmentNum { get; set; }

        }
        public class Relevance
        {
            public int id { get; set; }
            public string name { get; set; }
            public bool IsRelevannce { get; set; }

            //public string seaction { get; set; }
        }

        public class Temp
        {
            public int id { get; set; }
            public decimal StandardCapacity { get; set; }
            public int StandardNumber { get; set; }
            public string Name { get; set; }
        }
        #endregion

        #region 总表

        /// <summary>
        /// 总表,上传手输数据
        /// </summary>
        /// 根据条件找到符合的总表数据,循环,将前端需要的数据传出,根据平台,类型,PCB板找到贴片表的数据,平衡表的数据,及各工段的数据
        /// <param name="protype">类型</param>
        /// <param name="proplatform">平台</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TotalProcess_Capacity(string protype, string proplatform)
        {
            #region 参数筛选数值
            var total = db.Process_Capacity_Total.ToList();//没有筛选值
            if (!string.IsNullOrEmpty(proplatform))//如果有平台值
            {
                total = total.Where(c => c.Platform.Contains(proplatform)).ToList();
            }
            if (!string.IsNullOrEmpty(protype))//如果有类型值
            {
                total = total.Where(c => c.Type.Contains(protype)).ToList();
            }
            var idtolist = total.OrderByDescending(c => c.Id).Select(c => c.Id).ToList();//根据id排序，得到ID值
            #endregion
            #region 取值
            //SMT
            var SmtList = db.Pick_And_Place.Select(c => new TempSmt { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, PlafModule = c.PlatformModul, ProcessDescription = c.ProcessDescription, PersonNum = c.PersonNum, CapacityPerHour = c.CapacityPerHour, Maintenance = c.Maintenance }).ToList();
            //平衡卡
            var Balance = db.ProcessBalance.Select(c => new TempBlance { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, PlafModule = c.PlatformModul, StandardCapacity = c.StandardOutput, StandardTotal = c.StandardTotal, StandardHourlyOutputPerCapita = c.StandardHourlyOutputPerCapita, Id = c.Id, ModuleNeedNum = c.ModuleNeedNum2, Name = c.ProcessDescription, Title = c.Title, Section = c.Section, SerialNumber = c.SerialNumber, ScrewMachineNum = c.ScrewMachineNum, BalanceRate = c.BalanceRate, Bottleneck = c.Bottleneck, DispensMachineNum = c.DispensMachineNum, Maintenance = c.Maintenance });

            var Manual = db.Process_Capacity_Manual.Select(c => new TempCapacity
            {
                Type = c.Type,
                ProductPCBnumber = c.ProductPCBnumber,
                Platform = c.Platform,
                PlatformModul = c.PlatformModul,
                Name = c.Name,
                StandardCapacity = c.StandardCapacity,
                StandardNumber = c.StandardNumber,
                Id = c.Id,
                Name2
                = c.Name2,
                StandardCapacity2 = c.StandardCapacity2,
                StandardNumber2 = c.StandardNumber2,
                Maintenance = c.Maintenance,
                PCBASingleLampNumber = c.PCBASingleLampNumber,
                SingleLampWorkingHours = c.SingleLampWorkingHours,
                Section = c.Section,
                ScrewMachineNum = c.ScrewMachineNum,
                DispensMachineNum = c.DispensMachineNum
            }).ToList();
            ////插件
            //var PluginList = db.Process_Capacity_Plugin.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, PlafModule = c.PlatformModul, PCBASingleLampNumber = c.PCBASingleLampNumber, StandardCapacity = c.PluginStandardCapacity, Name = c.Name, ModuleNeedNum = c.ModuleNeedNum2, SingleLampWorkingHours = c.SingleLampWorkingHours, StandardNumber = c.PluginStandardNumber, Id = c.Id, Maintenance = c.Maintenance });
            ////喷墨
            //var InkjetList = db.Process_Capacity_Inkjet.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, PlafModule = c.PlatformModul, StandardCapacity = c.InkjetStabdardOutputPerHour, Name = c.InkjetProcessName, ModuleNeedNum = c.ModuleNeedNum2, StandardNumber = c.InkjetSuctionStandardTotal, Id = c.Id, Maintenance = c.Maintenance });
            ////灌胶
            //var GlueList = db.Process_Capacity_Glue.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, PlafModule = c.PlatformModul, StandardCapacity = c.GlueStabdardOutput, Name = c.GlueProcessName, ModuleNeedNum = c.ModuleNeedNum2, StandardNumber = c.GlueStandardTotal, Id = c.Id, Maintenance = c.Maintenance });
            ////气密
            //var AirtightList = db.Process_Capacity_Airtight.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, PlafModule = c.PlatformModul, StandardCapacity = c.AirtightStabdardOutput, Name = c.AirtightProcessName, ModuleNeedNum = c.ModuleNeedNum2, StandardNumber = c.AirtightStandardTotal, Id = c.Id, Maintenance = c.Maintenance });
            ////老化
            //var BurnList = db.Process_Capacity_Burin.Select(c => new TempBurn { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, PlafModule = c.PlatformModul, BurinOneProcessName = c.BurinOneProcessName, BurinOneStabdardOutputPerHour = c.BurinOneStabdardOutputPerHour, BurninOneSuctionStandardTotal = c.BurninOneSuctionStandardTotal, ModuleNeedNum = c.ModuleNeedNum2, Id = c.Id, BurinTwoProcessName = c.BurinTwoProcessName, BurinTwoStabdardOutputPerHour = c.BurinTwoStabdardOutputPerHour, BurinTwoSuctionStandardTotal = c.BurinTwoSuctionStandardTotal, Maintenance = c.Maintenance });

            #endregion
            JArray jarray = new JArray();

            foreach (var id in idtolist)
            {
                JObject result = new JObject();
                var info = db.Process_Capacity_Total.Find(id);
                var item = info.Type;
                result.Add("id", info.Id);//id
                result.Add("Type", info.Type);//类型
                var platform = info.Platform;
                var PCB = info.ProductPCBnumber;
                result.Add("Platform", platform);//平台
                result.Add("PCB", PCB);//PCB板
                result.Add("PlatformModul", info.PlatformModul);//平台模块
                result.Add("Maintenance", info.Maintenance);//维护方式
                result.Add("PCBDrying", info.PCBDrying);//pcb烘干时间
                result.Add("OutboundProcess", info.OutboundProcess);//外发工艺
                result.Add("DryTime", info.DryTime);//干燥时间
                result.Add("TotalEdit", false);//前端用
                result.Add("editNum", 0);//前端用

                #region ic面贴装 灯面贴装
                result.Add("SMTEidt", false);//前端用
                var TempSmtvalue = SmtList.Where(c => c.Type == item && c.PCB == PCB && c.Plafrom == platform && c.PlafModule == info.PlatformModul && c.Maintenance == info.Maintenance).ToList();//根据平台，类型，PCB板号找到贴片信息

                var aa = SMTvalue(TempSmtvalue);
                result.Add("SMTExaminanMessage", "审核通过");//默认审核都通过，这个判断暂时没用
                result.Add("icProductName", aa["Name"]);//IC面名字
                result.Add("icMaxStandardTotal", aa["icMaxStandardTotal"]);//ic面最大产能标配人数
                result.Add("icMaxStandardOutput", aa["icMaxStandardOutput"]);//ic面最大标配产能
                result.Add("icMinStandardTotal", aa["icMinStandardTotal"]);//IC面最小产能标配人数
                result.Add("icMinStandardOutput", aa["icMinStandardOutput"]);//ic面最小标配产能
                result.Add("LightProductName", aa["Name2"].ToString());//灯面名字
                result.Add("LightMaxStandardTotal", aa["LightMaxStandardTotal"]);//灯面最大标配人数
                result.Add("LightMaxStandardOutput", aa["LightMaxStandardOutput"]);//灯面最大标配产能
                result.Add("LightMinStandardTotal", aa["LightMinStandardTotal"]);//灯面最小标配人数
                result.Add("LightMinStandardOutput", aa["LightMinStandardOutput"]);//灯面最小标配产能
                //result.Add("SMTmoduleneed", aa.ModuleNeed);//贴片模组需求

                //SMT是否有图片或PDF
                if (IsHavingIMG(id, "SMT", "SMT"))//是否有图片
                    result.Add("SMTjpg", true);
                else
                    result.Add("SMTjpg", false);

                if (IsHavingPDF(id, "SMT", "SMT"))//是否有PDF文档
                    result.Add("SMTpdf", true);
                else
                    result.Add("SMTpdf", false);
                #endregion

                #region 插件
                var plugin = Manual.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.PlatformModul == info.PlatformModul && c.Maintenance == info.Maintenance && c.Section == "插件").ToList();//根据平台，PCB板,类型查找插件数据
                if (plugin.Count == 0)//如果找不到数据
                {
                    result.Add("PluginDevice", null);
                }
                else
                {
                    JArray array = new JArray();
                    foreach (var pluginitem in plugin)
                    {
                        JObject pluginjobject = new JObject();
                        pluginjobject.Add("PluginEidt", false);//前端用
                        pluginjobject.Add("PluginDeviceID", pluginitem.Id);//id
                        pluginjobject.Add("PluginDeviceName", pluginitem.Name); //插件工序名
                        pluginjobject.Add("SingleLampWorkingHours", pluginitem.SingleLampWorkingHours);//插件机台固定标准单灯工时
                        pluginjobject.Add("PCBASingleLampNumber", pluginitem.PCBASingleLampNumber);//插件PCBA单灯数
                        pluginjobject.Add("PluginStandardNumber", pluginitem.StandardNumber);//插件标配人数
                        pluginjobject.Add("PluginStandardCapacity", pluginitem.StandardCapacity);//插件产能标准
                        //pluginjobject.Add("PluginModuleNeed", pluginitem.ModuleNeedNum);//插件产能标准
                        if (IsHavingPDF(id, "插件", pluginitem.Name))//是否有pdf
                            pluginjobject.Add("PluginPdf", true);
                        else
                            pluginjobject.Add("PluginPdf", false);
                        if (IsHavingIMG(id, "插件", pluginitem.Name))//是否有jpg
                            pluginjobject.Add("PluginJpg", true);
                        else
                            pluginjobject.Add("PluginJpg", false);
                        array.Add(pluginjobject);//将单行的插件数据放到一个Jarray中
                    }
                    result.Add("PluginDevice", array);
                }
                #endregion

                #region 后焊
                var AfterWeldProcessName = Balance.Where(c => c.Type == item && c.PCB == PCB && c.Plafrom == platform && c.PlafModule == info.PlatformModul && c.Maintenance == info.Maintenance && c.Section == "后焊").ToList();//根据平台，类型，pcb板查找后焊数据
                if (AfterWeldProcessName.Count == 0)//如果数据为空
                {
                    result.Add("AfterWeld", null);
                }
                else
                {
                    var AfterWeldInfo = BlanceModule(AfterWeldProcessName);//调用通用json模板 
                    JArray AfterWeld = new JArray();
                    foreach (var AfterWelditem in AfterWeldInfo)
                    {
                        JObject jObject = new JObject();
                        jObject.Add("AfterWeldEidt", false);//前端用
                        jObject.Add("AfterWeldExaminanMessage", "审核通过");
                        jObject.Add("AfterWeldID", AfterWelditem.Id);//id
                        jObject.Add("AfterWeldProcessName", AfterWelditem.Title == "/" ? "/" : (AfterWelditem.Name == null ? AfterWelditem.Title : AfterWelditem.Name));//后焊工序描述
                        jObject.Add("AfterWeldStandardTotal", AfterWelditem.StandardTotal);//后焊标配人数
                        jObject.Add("AfterWeldStandardOutput", AfterWelditem.StandardCapacity);//后焊标配产能
                        jObject.Add("AfterModuleNeed", AfterWelditem.ModuleNeedNum);//后面模组需求
                        jObject.Add("AfterWeldStandardHourlyOutputPerCapita", AfterWelditem.StandardHourlyOutputPerCapita);//后焊标准人均时产能
                        jObject.Add("AfterWeldPdf", AfterWelditem.Pdf);//后焊pdf
                        jObject.Add("AfterWeldImg", AfterWelditem.Jpg);//后焊JPG

                        AfterWeld.Add(jObject);
                    }


                    result.Add("AfterWeld", AfterWeld);
                }
                #endregion

                #region 三防
                var threeprof = Manual.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.PlatformModul == info.PlatformModul && c.Maintenance == info.Maintenance && c.Section == "三防").ToList();//根据平台，PCB板，类型查找三防数据
                if (threeprof.Count == 0)//如果数据为空
                {
                    result.Add("ThreeProf", null);
                }
                else
                {
                    //var threeprofInfo = BlanceModule(threeprof);//调用通用json模板 
                    JArray Three = new JArray();
                    foreach (var threeprofitem in threeprof)
                    {
                        JObject jObject = new JObject();
                        jObject.Add("ThreeProfEidt", false);//前端用
                        jObject.Add("ThreeProfExaminanMessage", "审核通过");
                        jObject.Add("ThreeProfID", threeprofitem.Id);//id
                        jObject.Add("ThreeProfProcessName", threeprofitem.Name);//后焊工序描述
                        jObject.Add("ThreeProfStandardTotal", threeprofitem.StandardNumber);//后焊标配人数
                        jObject.Add("ThreeProfStabdardOutput", threeprofitem.StandardCapacity);//后焊标配产能
                        //jObject.Add("ThreeProfNeed", threeprofitem.ModuleNeedNum);//后面模组需求
                        //jObject.Add("AfterWeldStandardHourlyOutputPerCapita", threeprofitem.StandardHourlyOutputPerCapita);//后焊标准人均时产能
                        if (IsHavingPDF(id, "三防", threeprofitem.Name))//是否有pdf
                            jObject.Add("ThreeProfPdf", true);
                        else
                            jObject.Add("ThreeProfPdf", false);
                        if (IsHavingIMG(id, "三防", threeprofitem.Name))//是否有jpg
                            jObject.Add("ThreeProfJpg", true);
                        else
                            jObject.Add("ThreeProfJpg", false);

                        Three.Add(jObject);
                    }
                    result.Add("ThreeProf", Three);
                }
                #endregion

                #region 打底壳
                var BottomCasProcessName = Manual.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.PlatformModul == info.PlatformModul && c.Maintenance == info.Maintenance && c.Section == "打底壳").ToList();//根据平台，PCB板，平台查找打底壳数据
                if (BottomCasProcessName.Count == 0)//如果没有数据
                {
                    result.Add("BottomCas", null);
                }
                else
                {
                    JArray BottomCas = new JArray();
                    // var BottomCasinfo = BlanceModule(BottomCasProcessName);//调用通用模板
                    foreach (var message in BottomCasProcessName)
                    {
                        JObject jObject = new JObject();
                        jObject.Add("BottomCasEidt", false);//前端用
                        //jObject.Add("BottomCasModuleNeed", message.ModuleNeedNum);//打底壳模组需求
                        jObject.Add("BottomCasID", message.Id);//id
                        jObject.Add("BottomCasExaminanMessage", "审核通过");
                        jObject.Add("BottomCasProcessName", message.Name);//打底壳工序描述
                        jObject.Add("BottomCasStandardTotal", message.StandardNumber);//打底壳标配人数
                        jObject.Add("BottomCasStandardOutput", message.StandardCapacity);//打底壳标配产能
                        jObject.Add("BottomCasStandardHourlyOutputPerCapita", message.StandardNumber == 0 ? 0 : Math
                            .Round(message.StandardCapacity / message.StandardNumber, 2));//打底壳标配人均产能
                        jObject.Add("BottomCasDispensMachineNum", message.DispensMachineNum);//打底壳点胶机数量
                        jObject.Add("BottomCasScrewMachineNum", message.ScrewMachineNum);//打底壳螺丝机数量

                        if (IsHavingPDF(id, "打底壳", message.Name))//是否有pdf
                            jObject.Add("BottomCasPdf", true);
                        else
                            jObject.Add("BottomCasPdf", false);
                        if (IsHavingIMG(id, "打底壳", message.Name))//是否有jpg
                            jObject.Add("BottomCasImg", true);
                        else
                            jObject.Add("BottomCasImg", false);

                        BottomCas.Add(jObject);
                    }
                    result.Add("BottomCas", BottomCas);
                }
                #endregion

                #region 装磁吸
                var magnetic = Balance.Where(c => c.Type == item && c.PCB == PCB && c.Plafrom == platform && c.PlafModule == info.PlatformModul && c.Maintenance == info.Maintenance && c.Section == "装磁吸安装板").ToList();//根据类型，Pbc板，平台查找装磁吸信息
                if (magnetic.Count == 0)//如果没有数据
                {
                    result.Add("Magnetic", null);

                }
                else
                {
                    JArray magneticJarry = new JArray();
                    var mafneticinfo = BlanceModule(magnetic);//调用通用json文件
                    foreach (var ProcessName in mafneticinfo)
                    {
                        JObject jObject = new JObject();

                        jObject.Add("MagneticEidt", false);//前端用
                        jObject.Add("MagneticID", ProcessName.Id);//id
                        jObject.Add("MagneticExaminanMessage", "审核通过");
                        jObject.Add("MagneticModuleNeed", ProcessName.ModuleNeedNum);//装磁吸模组需求数量
                        jObject.Add("MagneticProcessName", ProcessName.Title == "/" ? "/" : (ProcessName.Name == null ? ProcessName.Title : ProcessName.Name));//装磁吸工序描述
                        jObject.Add("MagneticStandardTotal", ProcessName.StandardTotal);//装磁吸标准人数
                        jObject.Add("MagneticStabdardOutput", ProcessName.StandardCapacity);//装磁吸标配产能
                        jObject.Add("MagneticStandardHourlyOutputPerCapita", ProcessName.StandardHourlyOutputPerCapita);//装磁吸人均时产能
                        jObject.Add("MagneticPdf", ProcessName.Pdf);//装磁吸pdf
                        jObject.Add("MagneticImg", ProcessName.Jpg);//装磁吸jpg

                        magneticJarry.Add(jObject);

                    }
                    result.Add("Magnetic", magneticJarry);
                }

                #endregion

                #region 喷墨
                var inkjet = Manual.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.PlatformModul == info.PlatformModul && c.Maintenance == info.Maintenance && c.Section == "喷墨").ToList();//根据平台,PCB板,类型查找喷墨数据
                if (inkjet.Count == 0)//如果没有数据
                {
                    result.Add("Inkjet", null);

                }
                else
                {
                    JArray array = new JArray();
                    foreach (var inkjetitem in inkjet)
                    {
                        JObject jObject = new JObject();
                        jObject.Add("InkjetEidt", false);//前端用
                        jObject.Add("InkjetID", inkjetitem.Id);//id
                        jObject.Add("InkjetProcessName", inkjetitem.Name);//喷墨工序
                        jObject.Add("InkjetSuctionStandardTotal", inkjetitem.StandardNumber);//喷墨配置人数
                        jObject.Add("InkjetStabdardOutputPerHour", inkjetitem.StandardCapacity);//喷墨每小时产能
                        //jObject.Add("InkjetModuleNeed", inkjetitem.ModuleNeedNum);//模组需求数量
                        if (IsHavingPDF(id, "喷墨", inkjetitem.Name))//喷墨pdf
                            jObject.Add("InkjetPdf", true);
                        else
                            jObject.Add("InkjetPdf", false);
                        if (IsHavingIMG(id, "喷墨", inkjetitem.Name))//喷墨jpg
                            jObject.Add("InkjetJpg", true);
                        else
                            jObject.Add("InkjetJpg", false);
                        array.Add(jObject);
                    }
                    result.Add("Inkjet", array);
                }
                #endregion

                #region 灌胶
                var glue = Manual.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.PlatformModul == info.PlatformModul && c.Maintenance == info.Maintenance && c.Section == "灌胶").ToList();//根据平台,类型,PCB板查找灌胶数据
                if (glue.Count == 0)//如果没有数据
                {
                    result.Add("Glue", null);

                }
                else
                {
                    JArray array = new JArray();
                    foreach (var glueitem in glue)
                    {
                        JObject jObject = new JObject();
                        jObject.Add("GlueEidt", false);//前端用
                        jObject.Add("GlueID", glueitem.Id);//id
                        jObject.Add("GlueProcessName", glueitem.Name);//灌胶工序描述
                        jObject.Add("GlueStandardTotal", glueitem.StandardNumber);//灌胶标准总人数
                        jObject.Add("GlueStabdardOutput", glueitem.StandardCapacity);//灌胶标准产量
                        //jObject.Add("GlueModuleNeed", glueitem.ModuleNeedNum);//模组需求数量
                        if (IsHavingPDF(id, "灌胶", glueitem.Name))//灌胶pdf
                            jObject.Add("GluePdf", true);
                        else
                            jObject.Add("GluePdf", false);
                        if (IsHavingIMG(id, "灌胶", glueitem.Name))//灌胶jpg
                            jObject.Add("GlueJpg", true);
                        else
                            jObject.Add("GlueJpg", false);
                        array.Add(jObject);
                    }
                    result.Add("Glue", array);
                }
                #endregion

                #region 气密
                var airtight = Manual.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.PlatformModul == info.PlatformModul && c.Maintenance == info.Maintenance && c.Section == "气密").ToList();//根据平台,类型,PCB板查找气密数据
                if (airtight.Count == 0)//如果没有数据
                {
                    result.Add("Airtight", null);

                }
                else
                {
                    JArray array = new JArray();
                    foreach (var airtightitem in airtight)
                    {
                        JObject jObject = new JObject();
                        jObject.Add("AirtightEidt", false);//前端用
                        jObject.Add("AirtightID", airtightitem.Id);
                        jObject.Add("AirtightProcessName", airtightitem.Name);//气密工序描述
                        jObject.Add("AirtightStandardTotal", airtightitem.StandardNumber);//气密标准总人数
                        jObject.Add("AirtightStabdardOutput", airtightitem.StandardCapacity);//气密标准产量
                        //jObject.Add("AirtightModuleNeed", airtightitem.ModuleNeedNum);//模组需求数量
                        if (IsHavingPDF(id, "气密", airtightitem.Name))//气密pdf
                            jObject.Add("AirtightPdf", true);
                        else
                            jObject.Add("AirtightPdf", false);
                        if (IsHavingIMG(id, "气密", airtightitem.Name))//气密jpg
                            jObject.Add("AirtightJpg", true);
                        else
                            jObject.Add("AirtightJpg", false);
                        array.Add(jObject);
                    }
                    result.Add("Airtight", array);
                }

                #endregion

                #region 模块线
                var ModuleLineProcessName = Balance.Where(c => c.Type == item && c.PCB == PCB && c.Plafrom == platform && c.PlafModule == info.PlatformModul && c.Maintenance == info.Maintenance && c.Section == "模块线").ToList();//根据平台，PCB板，平台查找模块线数据
                if (ModuleLineProcessName.Count == 0)//如果没有数据
                {
                    result.Add("ModuleLine", null);
                }
                else
                {
                    JArray ModuleLine = new JArray();
                    var ModuleLineinfo = BlanceModule(ModuleLineProcessName);//调用通用模板
                    foreach (var message in ModuleLineinfo)
                    {
                        JObject jObject = new JObject();
                        jObject.Add("ModuleLineEidt", false);//前端用
                        jObject.Add("ModuleLineModuleNeed", message.ModuleNeedNum);//模块线模组需求
                        jObject.Add("ModuleLineID", message.Id);//id
                        jObject.Add("ModuleLineExaminanMessage", "审核通过");
                        jObject.Add("ModuleLineProcessName", message.Title == "/" ? "/" : (message.Name == null ? message.Title : message.Name));//模块线工序描述
                        jObject.Add("ModuleLineStandardTotal", message.StandardTotal);//模块线标配人数
                        jObject.Add("ModuleLineStandardOutput", message.StandardCapacity);//模块线标配产能

                        jObject.Add("ModuleLinePdf", message.Pdf);//模块线PDF

                        jObject.Add("ModuleLineImg", message.Jpg);//模块线JPG
                        // }
                        ModuleLine.Add(jObject);
                    }
                    result.Add("ModuleLine", ModuleLine);
                }
                #endregion

                #region 锁面罩
                var LockTheMaskProcessName = Manual.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.PlatformModul == info.PlatformModul && c.Maintenance == info.Maintenance && c.Section == "锁面罩").ToList();//根据平台,类型,PCB板查找锁面罩数据
                if (LockTheMaskProcessName.Count() == 0)//如果没有数据
                {
                    result.Add("LockTheMask", null);
                }
                else
                {

                    JArray LockTheMask = new JArray();
                    //var Lockinfo = BlanceModule(LockTheMaskProcessName);//调用通用模板
                    foreach (var ProcessName in LockTheMaskProcessName)
                    {
                        JObject jObject = new JObject();

                        jObject.Add("LockTheMaskEdit", false);//前端用
                        //jObject.Add("LockTheMaskModuleNeed", ProcessName.ModuleNeedNum);//模组需求数量
                        jObject.Add("LockTheMaskID", ProcessName.Id);//id
                        jObject.Add("LockTheMaskExaminanMessage", "审核通过");
                        jObject.Add("LockTheMaskProcessName", ProcessName.Name);//锁面罩工序描述
                        jObject.Add("LockTheMaskStandardTotal", ProcessName.StandardNumber);//锁面罩标配人数
                        jObject.Add("LockTheMaskStandardOutput", ProcessName.StandardCapacity);//锁面罩标配产能
                        jObject.Add("LockTheMaskStandardHourlyOutputPerCapita", ProcessName.StandardNumber == 0 ? 0 : Math
                            .Round(ProcessName.StandardCapacity / ProcessName.StandardNumber, 2));//锁面罩人均时产能
                        jObject.Add("LockTheMaskScrewMachineNum", ProcessName.ScrewMachineNum);//锁面罩螺丝机数量

                        if (IsHavingPDF(id, "锁面罩", ProcessName.Name))//是否有pdf
                            jObject.Add("LockTheMaskPdf", true);
                        else
                            jObject.Add("LockTheMaskPdf", false);
                        if (IsHavingIMG(id, "锁面罩", ProcessName.Name))//是否有jpg
                            jObject.Add("LockTheMaskImg", true);
                        else
                            jObject.Add("LockTheMaskImg", false);
                        //}
                        LockTheMask.Add(jObject);
                    }
                    result.Add("LockTheMask", LockTheMask);
                }
                #endregion

                #region 模组装配
                var ModuleProcessName = Balance.Where(c => c.Type == item && c.PCB == PCB && c.Plafrom == platform && c.PlafModule == info.PlatformModul && c.Maintenance == info.Maintenance && c.Section == "模组装配").ToList();//根据类型,平台,PCB板查找模组装配数量
                if (ModuleProcessName.Count == 0)//如果没有数据
                {
                    result.Add("Module", null);
                }
                else
                {
                    JArray Module = new JArray();
                    var Moduleinfo = BlanceModule(ModuleProcessName);//调用通用json模板
                    foreach (var message in Moduleinfo)
                    {
                        JObject jObject = new JObject();
                        jObject.Add("ModuleEidt", false);//前端用
                        jObject.Add("ModuleNeed", message.ModuleNeedNum);//模组需求数量
                        jObject.Add("ModuleID", message.Id);//id
                        jObject.Add("ModuleExaminanMessage", "审核通过");
                        jObject.Add("ModuleProcessName", message.Title == "/" ? "/" : (message.Name == null ? message.Title : message.Name));//模组装配工序描述
                        jObject.Add("ModuleStandardTotal", message.StandardTotal);//模组装配标配人数
                        jObject.Add("ModuleBalanceRate", message.BalanceRate + "%");//模组装配平衡率
                        jObject.Add("ModuleBottleneck", message.Bottleneck);//模组装配瓶颈
                        jObject.Add("ModuleStandardOutput", message.StandardCapacity);//模组装配标配产能
                        jObject.Add("ModuleStandardHourlyOutputPerCapita", message.StandardHourlyOutputPerCapita);//模组装配人均时产能

                        jObject.Add("ModulePdf", message.Pdf);//模组装配pdf

                        jObject.Add("ModuleImg", message.Jpg);//模组装配jpg

                        Module.Add(jObject);
                    }
                    result.Add("Module", Module);
                }
                #endregion

                #region 老化
                var burnin = Manual.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.PlatformModul == info.PlatformModul && c.Maintenance == info.Maintenance && c.Platform == platform && c.Section == "老化").ToList();//根据类型,PCB板,平台查找老化数据
                if (burnin.Count == 0)//如果没有数据
                {
                    result.Add("Burin", null);

                }
                else
                {
                    JArray array = new JArray();
                    foreach (var burninitem in burnin)
                    {
                        JObject jObject = new JObject();
                        jObject.Add("BurinEidt", false);//前端用
                        jObject.Add("BurinID", burninitem.Id);
                        jObject.Add("BurinOneProcessName", burninitem.Name);//老化工序描述1
                        jObject.Add("BurninOneSuctionStandardTotal", burninitem.StandardNumber);//老化1标配人数
                        jObject.Add("BurinOneStabdardOutputPerHour", burninitem.StandardCapacity);//老化1每小时产能
                        jObject.Add("BurinTwoProcessName", burninitem.Name2);//老化2工序描述
                        jObject.Add("BurinTwoSuctionStandardTotal", burninitem.StandardNumber2);//老化2标配人数
                        jObject.Add("BurinTwoStabdardOutputPerHour", burninitem.StandardCapacity2);//老化2每小时标准产能
                        //jObject.Add("BurinModuleNeed", burninitem.ModuleNeedNum);//模组需求数量
                        if (IsHavingPDF(id, burninitem.Name, burninitem.Name))//老化1是否有pdf
                            jObject.Add("BurinOnePdf", true);
                        else
                            jObject.Add("BurinOnePdf", false);
                        if (IsHavingIMG(id, burninitem.Name2, burninitem.Name2))//老化2是否有pdf
                            jObject.Add("BurinTwoPdf", true);
                        else
                            jObject.Add("BurinTwoPdf", false);
                        array.Add(jObject);
                    }
                    result.Add("Burin", array);
                }

                #endregion

                #region 包装
                var PackingProcessName = Balance.Where(c => c.Type == item && c.PCB == PCB && c.Plafrom == platform && c.PlafModule == info.PlatformModul && c.Maintenance == info.Maintenance && c.Section == "包装").ToList();//根据类型,PCB板,平台查找包装数据
                if (PackingProcessName.Count == 0)//如果没有数据
                {
                    result.Add("Packing", null);
                }
                else
                {
                    JArray Packing = new JArray();
                    var Packinginfo = BlanceModule(PackingProcessName);
                    foreach (var message in Packinginfo)
                    {
                        JObject jObject = new JObject();
                        jObject.Add("PackingEidt", false);//前端用
                        jObject.Add("PackingModuleNeed", message.ModuleNeedNum);//模组需求数量
                        jObject.Add("PackingID", message.Id);//id
                        jObject.Add("PackingExaminanMessage", "审核通过");
                        jObject.Add("PackingProcessName", message.Title == "/" ? "/" : (message.Name == null ? message.Title : message.Name));//包装工序描述
                        jObject.Add("PackingStandardTotal", message.StandardTotal);//包装标配人数
                        jObject.Add("PackingStandardOutput", message.StandardCapacity);//包装标配产能
                        jObject.Add("PackingPdf", message.Pdf);//包装pdf
                        jObject.Add("PackingImg", message.Jpg);//包装jpg
                        //}
                        Packing.Add(jObject);
                    }
                    result.Add("Packing", Packing);
                }
                #endregion
                jarray.Add(result);
            }
            return Content(JsonConvert.SerializeObject(jarray));
        }

        /// <summary>
        /// 贴片显示json模板
        /// </summary>
        /// 用于两个总表的贴片工段里,返传给前端的通用json模板
        /// <param name="iccount">数据</param>
        /// <returns></returns>
        public JObject SMTvalue(List<TempSmt> iccount)
        {
            JObject result = new JObject();
            if (iccount.Count != 0)//如果传过来的数据不为空
            {
                //已选择跳过,数据传回"/"
                if (iccount.FirstOrDefault().Section == "/")
                {
                    //result.Add("Seaction", "/");
                    result.Add("Name", "/");
                    result.Add("Name2", "/");
                    result.Add("icMaxStandardTotal", 0);
                    result.Add("icMaxStandardOutput", 0);
                    result.Add("icMinStandardTotal", 0);
                    result.Add("icMinStandardOutput", 0);
                    result.Add("LightMaxStandardTotal", 0);
                    result.Add("LightMaxStandardOutput", 0);
                    result.Add("LightMinStandardTotal", 0);
                    result.Add("LightMinStandardOutput", 0);
                }
                else
                {

                    //var moduleneed = iccount.FirstOrDefault().ModuleNeed;//查找贴片数据第一个模组需求数(一个贴片表有14条数据代表14条线,一个贴片表只有一个模组需求量)

                    //文件中没有IC面贴装
                    if (iccount.Where(c => c.ProcessDescription == "IC面贴装").Count() == 0)//查找14条数据中的工序描述有没有含有IC面贴装,如果没有,则其中关于IC面最大最小的产能和人数都不传值,默认是0
                    {
                        //result.ExaminamMessage = "审核通过";
                        // result.Seaction = iccount.FirstOrDefault().Section;
                        result.Add("Name", "IC面贴装");
                        result.Add("icMaxStandardTotal", 0);
                        result.Add("icMaxStandardOutput", 0);
                        result.Add("icMinStandardTotal", 0);
                        result.Add("icMinStandardOutput", 0);
                    }
                    else//如果含有IC面贴装
                    {
                        var icMaxStandardTotal = iccount.Where(c => c.ProcessDescription == "IC面贴装").Max(c => c.PersonNum);//在IC面贴装中找最大的标配人数
                        var icMaxStandardOutput = iccount.Where(c => c.ProcessDescription == "IC面贴装").Max(c => c.CapacityPerHour);//在IC面贴装中找最大的标配产能
                        var icMinStandardTotal = iccount.Where(c => c.ProcessDescription == "IC面贴装").Min(c => c.PersonNum);//在IC面贴装中找最小的标配人数
                        var icMinStandardOutput = iccount.Where(c => c.ProcessDescription == "IC面贴装").Min(c => c.CapacityPerHour);//在IC面贴装中找最小的标配产能

                        result.Add("Name", "IC面贴装");
                        result.Add("icMaxStandardTotal", icMaxStandardTotal);
                        result.Add("icMaxStandardOutput", icMaxStandardOutput);
                        result.Add("icMinStandardTotal", icMinStandardTotal);
                        result.Add("icMinStandardOutput", icMinStandardOutput);

                    }// result.Add("LimtProductName", true);
                    if (iccount.Where(c => c.ProcessDescription == "灯面贴装").Count() == 0)//查找14条数据中的工序描述有没有含有灯面面贴装,如果没有,则其中关于灯面最大最小的产能和人数都不传值,默认是0
                    {
                        result.Add("Name2", "灯面贴装");
                        result.Add("LightMaxStandardTotal", 0);
                        result.Add("LightMaxStandardOutput", 0);
                        result.Add("LightMinStandardTotal", 0);
                        result.Add("LightMinStandardOutput", 0);
                    }
                    else
                    {
                        var LightMaxStandardTotal = iccount.Where(c => c.ProcessDescription == "灯面贴装").Max(c => c.PersonNum);//在灯面贴装中招最大的标配人数
                        var LightMaxStandardOutput = iccount.Where(c => c.ProcessDescription == "灯面贴装").Max(c => c.CapacityPerHour);//在灯面贴装中招最大的标配产能
                        var LightMinStandardTotal = iccount.Where(c => c.ProcessDescription == "灯面贴装").Min(c => c.PersonNum);//在灯面贴装中招最小的标配人数
                        var LightMinStandardOutput = iccount.Where(c => c.ProcessDescription == "灯面贴装").Min(c => c.CapacityPerHour);//在灯面贴装中招最小的标配产能

                        result.Add("Name2", "灯面贴装");
                        result.Add("LightMaxStandardTotal", LightMaxStandardTotal);
                        result.Add("LightMaxStandardOutput", LightMaxStandardOutput);
                        result.Add("LightMinStandardTotal", LightMinStandardTotal);
                        result.Add("LightMinStandardOutput", LightMinStandardOutput);

                    }
                }
            }
            else
            {
                result.Add("Name", null);
                result.Add("Name2", null);
                result.Add("icMaxStandardTotal", 0);
                result.Add("icMaxStandardOutput", 0);
                result.Add("icMinStandardTotal", 0);
                result.Add("icMinStandardOutput", 0);
                result.Add("LightMaxStandardTotal", 0);
                result.Add("LightMaxStandardOutput", 0);
                result.Add("LightMinStandardTotal", 0);
                result.Add("LightMinStandardOutput", 0);
            }
            return result;
        }

        /// <summary>
        /// 平衡卡显示json模板
        /// </summary>
        /// <param name="blancevalue">传过来的平衡卡临时class 数据</param>
        /// <returns></returns>
        public List<TempBlance> BlanceModule(List<TempBlance> blancevalue)
        {
            List<TempBlance> result = new List<TempBlance>();
            var title = blancevalue.Select(c => c.Title).Distinct().ToList();//查找某个平台,型号,PCB板,工段中有几个工序描述,blancevalue已经是筛选好平台,型号,PCB板,工段

            foreach (var ProcessName in title)
            {
                TempBlance item = new TempBlance();
                var message = blancevalue.OrderByDescending(c => c.SerialNumber).Where(c => c.Title == ProcessName).FirstOrDefault();//查找某个平台,型号,PCB板,工段,工序描述中的版本号,按版本号排序,找到版本号最后的数据

                item.ModuleNeedNum = message.ModuleNeedNum;//模组需求量
                item.Id = message.Id;
                item.Title = message.Title;
                item.Name = message.Name;//工序描述
                item.StandardTotal = message.StandardTotal;//标配人数
                item.StandardCapacity = message.StandardCapacity;//标配产能
                item.StandardHourlyOutputPerCapita = message.StandardHourlyOutputPerCapita;//人均时产能
                item.ScrewMachineNum = message.ScrewMachineNum;//螺丝机数量
                item.Bottleneck = message.Bottleneck;//瓶颈
                item.BalanceRate = message.BalanceRate;//平衡数
                item.DispensMachineNum = message.DispensMachineNum;//点胶机数量

                if (IsHavingPDF(message.Id, message.Section, ProcessName))//是否有pdf
                    item.Pdf = true;
                else
                    item.Pdf = false;
                if (IsHavingIMG(message.Id, message.Section, ProcessName))//是否有jpg
                    item.Jpg = true;
                else
                    item.Jpg = false;
                result.Add(item);
            }
            return result;
        }


        /// <summary>
        /// 审核未审核列表(暂时没用)
        /// </summary>
        /// <param name="statu">状态选择(审核,或未审核)</param>
        /// <returns></returns>
        public ActionResult DisplayStatuMessage(string statu)
        {
            JArray result = new JArray();
            #region 查找最后一个版本的集合
            List<Pick_And_Place> pcik = com.GetNewNumberPickInfo().Where(c => c.FileName != "/").ToList();//获取最后一个版本的贴片表集合,剔除掉跳过的不分
            List<ProcessBalance> balances = com.GetNewNumberBalanceInfo().Where(c => c.Title != "/").ToList(); //获取最后一个版本的平衡表集合,剔除条跳过不分
            #endregion
            switch (statu)
            {
                case "未审核":
                    var pickNotExaminan = pcik.Where(c => c.ExaminanTime == null && string.IsNullOrEmpty(c.ExaminanPeople)).ToList();//查找未审核的贴片数据
                    var blanceNotExaminan = balances.Where(c => c.ExaminanTime == null && string.IsNullOrEmpty(c.ExaminanPeople)).ToList();//查找未审核的平衡表数据
                    result = DisplayStatuMessageResult(pickNotExaminan, blanceNotExaminan); //通用模板,将数据整理返回出去
                    break;
                case "审核未通过":
                    var pickExaminanNotPass = pcik.Where(c => c.ExaminanTime != null && c.IsPassExaminan == false).ToList();//查找审核未通过的贴片数据
                    var blanceExaminanNotPass = balances.Where(c => c.ExaminanTime != null && c.IsPassExaminan == false).ToList();//查找审核未通过的平衡表数据
                    result = DisplayStatuMessageResult(pickExaminanNotPass, blanceExaminanNotPass);
                    break;
                case "审核通过":
                    var pickExaminanPass = pcik.Where(c => c.ExaminanTime != null && c.IsPassExaminan == true).ToList();//查找审核通过的贴片数据
                    var blanceExaminanPass = balances.Where(c => c.ExaminanTime != null && c.IsPassExaminan == true).ToList();//查找审核通过的平衡表数据
                    result = DisplayStatuMessageResult(pickExaminanPass, blanceExaminanPass);
                    break;
                case "未批准":
                    var pickNotApprover = pcik.Where(c => c.ApproverTime == null && string.IsNullOrEmpty(c.ApproverPeople)).ToList();//查找未批准的贴片数据
                    var blanceNotApprover = balances.Where(c => c.ApproverTime == null && string.IsNullOrEmpty(c.ApproverPeople)).ToList();//查找未批准的平衡表数据
                    result = DisplayStatuMessageResult(pickNotApprover, blanceNotApprover);
                    break;
                case "批准未通过":
                    var pickApproverNotPass = pcik.Where(c => c.ApproverTime != null && c.IsPassApprover == false).ToList();//查找批准未通过的贴片数据
                    var blanceApproverNotPass = balances.Where(c => c.ApproverTime != null && c.IsPassApprover == false).ToList();//查找批准未通过的平衡表数据
                    result = DisplayStatuMessageResult(pickApproverNotPass, blanceApproverNotPass);
                    break;
                case "批准通过":
                    var pickApproverPass = pcik.Where(c => c.ApproverTime != null && c.IsPassApprover == true).ToList();//查找批准通过的贴片数据
                    var blanceApproverPass = balances.Where(c => c.ApproverTime != null && c.IsPassApprover == true).ToList();//查找未审核的平衡表数据
                    result = DisplayStatuMessageResult(pickApproverPass, blanceApproverPass);
                    break;

            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //审核列表 子级通用模板
        public JArray DisplayStatuMessageResult(List<Pick_And_Place> pickanplan, List<ProcessBalance> processes)
        {
            JArray result = new JArray();
            var groupby = pickanplan.GroupBy(c => new { c.Type, c.Platform, c.ProductPCBnumber }).ToList();//对贴片数据进行分组
            var grouby2 = processes.GroupBy(c => new { c.Type, c.Platform, c.ProductPCBnumber }).ToList();//对平衡表数据进行分组
            foreach (var item in groupby)//循环贴片分组
            {
                JObject info = new JObject();
                var id = db.Process_Capacity_Total.Where(c => c.Type == item.Key.Type && c.Platform == item.Key.Platform && c.ProductPCBnumber == item.Key.ProductPCBnumber).Select(c => c.Id).FirstOrDefault();//获得每个分组对应的id 
                info.Add("id", id);
                info.Add("Type", item.Key.Type);
                info.Add("Platform", item.Key.Platform);
                info.Add("ProductPCBnumber", item.Key.ProductPCBnumber);
                info.Add("Section", "SMT");
                info.Add("Title", "贴装产能一览表");
                result.Add(info);
            }
            foreach (var item in grouby2)//循环平衡表分组
            {
                var message = processes.Where(c => c.Type == item.Key.Type && c.ProductPCBnumber == item.Key.ProductPCBnumber && c.Platform == item.Key.Platform).ToList();//获得每个分组的信息
                var groubymessage = message.GroupBy(c => new { c.Section, c.Title }).ToList();//根据上一个得到的信息,在根据工段和工序描述分组

                foreach (var section in groubymessage)//循环分组
                {
                    JObject info = new JObject();
                    var id = db.Process_Capacity_Total.Where(c => c.Type == item.Key.Type && c.Platform == item.Key.Platform && c.ProductPCBnumber == item.Key.ProductPCBnumber).Select(c => c.Id).FirstOrDefault();//找到每个分组的id
                    info.Add("id", id);
                    info.Add("Type", item.Key.Type);
                    info.Add("Platform", item.Key.Platform);
                    info.Add("ProductPCBnumber", item.Key.ProductPCBnumber);
                    info.Add("Section", section.Key.Section);
                    info.Add("Title", section.Key.Title);
                    result.Add(info);
                }
            }
            return result;
        }

        /// <summary>
        /// 建立关联
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="plafrom">平台</param>
        /// <param name="platformModul">平台模块</param>
        /// <param name="faterSeaction">父系的工段名</param>
        /// <param name="faterID">父系的ID</param>
        /// <param name="ChildSeaction">子系的工段名</param>
        /// <param name="ChildID">子系id</param>
        public void CreateAssociated(string type, string plafrom, string platformModul, string Maintenance, string faterSeaction, List<Relevance> faterID, string ChildSeaction, int ChildID)
        {
            foreach (var item in faterID)
            {
                if (item.IsRelevannce == true)//勾上关联
                {
                    if (db.Process_Capacity_Relevance.Count(c => c.Type == type && c.PlatformModul == platformModul && c.Maintenance == Maintenance && c.Platform == plafrom && c.ChildID == ChildID && c.ChildSeaction == ChildSeaction && c.FatherID == item.id && c.FatherSeaction == faterSeaction) == 0)//查找有没有相同数据,没有则创建数据
                    {
                        Process_Capacity_Relevance relevance = new Process_Capacity_Relevance() { Type = type, Platform = plafrom, PlatformModul = platformModul, Maintenance = Maintenance, FatherSeaction = faterSeaction, FatherID = item.id, ChildID = ChildID, ChildSeaction = ChildSeaction, OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName };
                        db.Process_Capacity_Relevance.Add(relevance);
                        db.SaveChanges();
                    }
                }
                if (item.IsRelevannce == false)//取消关联
                {
                    var info = db.Process_Capacity_Relevance.Where(c => c.Type == type && c.PlatformModul == platformModul && c.Maintenance == Maintenance && c.Platform == plafrom && c.ChildID == ChildID && c.ChildSeaction == ChildSeaction && c.FatherID == item.id && c.FatherSeaction == faterSeaction).FirstOrDefault();//查找有没有需要删除的数据
                    if (info != null)
                    {
                        db.Process_Capacity_Relevance.Remove(info);//删除数据
                        db.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// 显示关联信息
        /// </summary>
        /// <param name="type">型号</param>
        /// <param name="plafrom">平台</param>
        /// <param name="platformModul">平台模块</param>
        /// <param name="ChildSeaction">子系工段</param>
        /// <param name="ChildID">子系id</param>
        /// <returns></returns>
        public ActionResult DisplayAssociated(string type, string plafrom, string platformModul, string Maintenance, string ChildSeaction, int ChildID)
        {
            JArray result = new JArray();
            var info = db.Process_Capacity_Relevance.Where(c => c.Type == type && c.PlatformModul == platformModul && c.Platform == plafrom && c.Maintenance == Maintenance && c.ChildID == ChildID && c.ChildSeaction == ChildSeaction).ToList();//查出符合条件的关联信息
            foreach (var item in info)//将上一级的关联信息显示出来
            {
                JObject itemjobject = new JObject();
                itemjobject.Add("id", item.FatherID);
                itemjobject.Add("seaction", item.FatherSeaction);
                result.Add(itemjobject);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 查找关联表的后端所有数据,
        /// </summary>
        /// <param name="value">关联表的第一个开端数据</param>
        /// <returns></returns>
        public List<Process_Capacity_Relevance> GetRelevanceList(Process_Capacity_Relevance value)
        {
            List<Process_Capacity_Relevance> result = new List<Process_Capacity_Relevance>();
            var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == value.Type && c.Platform == value.Platform && c.Maintenance == value.Maintenance && c.PlatformModul == value.PlatformModul && c.FatherSeaction == value.ChildSeaction && c.FatherID == value.ChildID).ToList();
            if (relevance.Count != 0)
            {
                foreach (var item in relevance)
                {
                    result.AddRange(GetRelevanceList(item));
                    result.AddRange(relevance);
                }
            }
            return result;
        }
        public JArray FindRelevanceList(Process_Capacity_Relevance value)
        {
            JArray result = new JArray();
            JObject result2 = new JObject();
            var black = db.Process_Capacity_Relevance.Where(c => c.ChildID == value.FatherID && c.ChildSeaction == value.FatherSeaction).ToList();//查找有没有前端关联,没有就直接返回本数据,有则再循环,直到没找到前端数据

            if (black.Count() != 0)//
            {
                if (black.Count > 1)
                {
                    JArray temp = new JArray();
                    foreach (var item in black)
                    {
                        JObject vv = new JObject();
                        JArray bb = new JArray();
                        bb = FindRelevanceList(item);
                        if (temp.Count != 0 && temp[0][0].GetType().Name == "JArray")
                        {
                            foreach (JArray itemtemp in temp)
                            {
                                itemtemp.Add(bb);
                                break;
                            }
                        }
                        else
                        {
                            temp.Add(bb);
                        }
                    }
                    result = temp;
                }
                else
                {
                    result = FindRelevanceList(black.FirstOrDefault());//循环
                }

            }
            result2.Add("id", value.FatherID);
            result2.Add("seaction", value.FatherSeaction);

            result = ChildeInArray(result, result2);
            return result;
        }

        /// <summary>
        /// 判断节点保存进数组的方式
        /// </summary>
        /// 两种保存方式 如数组[{a},{b}],节点{C},保存之后应该为[{a},{b},{c}],如果数组[[{a},{b}],[{c},{d}]],节点{e} 保存之后应该为[[{a},{b},{e}],[{c},{d},{e}]]
        /// <param name="Array">数组</param>
        /// <param name="Noode">节点</param>
        /// <returns></returns>
        public JArray ChildeInArray(JArray Array, JObject Noode)
        {
            if (Array.Count > 0 && Array[0].GetType().Name == "JArray")//判断是否是两层的数组
            {
                JArray temp = new JArray();
                foreach (JArray cc in Array)//如果是的话,读其中的子节点
                {
                    JArray bb = ChildeInArray(cc, Noode);//再次循环,直到直到只有一层数据的数据
                    temp.Add(bb);
                }
                Array = temp;
            }
            else//只有一层数组的数据,添加节点
            {
                Array.Add(Noode);
            }
            return Array;
        }

        /// <summary>
        /// 拿到一组关联数据后,去掉多余的[]
        /// </summary>
        /// 例如最后的结果是要拿到[[{a},{a}],[{b},{b}],[{c},{c}],[{d},{d}]]. Value是其中的一组数据[{a},{a}],此方法是验证其中一种数据是否只含一层数组的数据,如果value是[{a},{a}] 则传回 [{a},{a}],check为false,如果是两层数据以上的数据 如[[{a},{a}]]或[[[{a},{a}]]],则会传回[[{a},{a}]],check 为true,传回check为true,收到返回值的方法会foreach传回值,再ADD
        /// <param name="Value">关联数据</param>
        /// <param name="chcek">判断得到这个数据后,是否要循环</param>
        /// <returns></returns>
        public JArray CheckValueCleanBrackets(JArray Value, ref bool chcek)
        {
            if (Value.Count > 0 && Value[0].GetType().Name == "JArray")
            {
                chcek = true;
                if (Value[0][0].GetType().Name == "JArray")
                {
                    foreach (JArray item in Value)
                    {
                        Value = CheckValueCleanBrackets(item, ref chcek);
                    }
                    return Value;
                }
                else
                {
                    return Value;
                }
            }
            else
            {
                return Value;
            }

        }

        #region 最新方法,最新页面
        public string GetNameitem(string seaction, int id)
        {
            string result = "";
            switch (seaction)//switch工段
            {
                case "插件":
                case "喷墨":
                case "灌胶":
                case "气密":
                case "老化":
                case "三防":
                case "打底壳":
                case "锁面罩":
                    var name = db.Process_Capacity_Manual.Where(c => c.Id == id).Select(c => c.Name).FirstOrDefault();
                    result = name;
                    break;

                case "后焊":
                case "装磁吸安装板":
                case "模组装配":
                case "包装":
                case "模块线":
                    var name6 = db.ProcessBalance.Where(c => c.Id == id).Select(c => new { c.ProcessDescription, c.Title }).FirstOrDefault(); ;
                    var name7 = name6.ProcessDescription != null ? name6.ProcessDescription : name6.Title;
                    result = name7;
                    break;
            }

            return result;
        }
        //根据传过来的平台模块找三防个灌胶
        public ActionResult GetThreeList(string type, string Platform, string modulelist, string Maintenance)
        {
            JObject relevanceitem = new JObject();
            JArray relevanceArray = new JArray();
            var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == type && c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.FatherSeaction == "三防" && c.ChildSeaction == "灌胶").ToList();//找到符合条件的所以关联表
            var relevance2 = db.Process_Capacity_Relevance.Where(c => c.Type == type && c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.FatherSeaction == "三防" && (c.ChildSeaction == "模组装配" || c.ChildSeaction == "包装")).ToList();
            if (relevance.Count != 0)
            {
                foreach (var reitem in relevance)
                {
                    JObject relevanceitem2 = new JObject();
                    relevanceitem2.Add("threeid", reitem.FatherID);
                    relevanceitem2.Add("threeseaction", "三防");
                    relevanceitem2.Add("threename", GetNameitem("三防", reitem.FatherID));

                    relevanceitem2.Add("guleid", reitem.ChildID);
                    relevanceitem2.Add("guleseaction", "灌胶");
                    relevanceitem2.Add("gulename", GetNameitem("灌胶", reitem.ChildID));
                    relevanceArray.Add(relevanceitem2);
                }
            }
            else if (relevance2.Count != 0)
            {
                var each = relevance2.Where((x, y) => relevance2.FindIndex(z => z.FatherID == x.FatherID) == y).ToList();
                foreach (var reitem in each)
                {
                    JObject relevanceitem2 = new JObject();
                    relevanceitem2.Add("threeid", reitem.FatherID);
                    relevanceitem2.Add("threeseaction", "三防");
                    relevanceitem2.Add("threename", GetNameitem("三防", reitem.FatherID));

                    var glue = db.Process_Capacity_Glue.Where(c => c.Type == type && c.Platform == Platform && c.PlatformModul == modulelist && c.GlueProcessName != "/").Select(c => c.GlueProcessName).ToList();

                    relevanceitem2.Add("guleid", 0);
                    relevanceitem2.Add("guleseaction", "灌胶");
                    relevanceitem2.Add("gulename", glue.Count > 0 ? string.Join("➵", glue.ToArray()) : null);

                    relevanceArray.Add(relevanceitem2);
                }
            }
            else
            {
                //List<string> threeRelevance = new List<string>(); ;
                var three = db.Process_Capacity_Manual.Where(c => c.Type == type && c.Platform == Platform && c.PlatformModul == modulelist && c.Section == "三防" && c.Maintenance == Maintenance && c.Name != " / ").Select(c => c.Name).ToList();
                //three.ForEach(c => threeRelevance.Add(c.ProcessDescription == null ? c.Title : c.ProcessDescription));

                relevanceitem.Add("threeid", 0);
                relevanceitem.Add("threeseaction", "三防");
                relevanceitem.Add("threename", three.Count > 0 ? string.Join("➵", three.ToArray()) : null);


                var glue = db.Process_Capacity_Manual.Where(c => c.Type == type && c.Platform == Platform && c.Maintenance == Maintenance && c.PlatformModul == modulelist && c.Section == "灌胶" && c.Name != "/").Select(c => c.Name).ToList();

                relevanceitem.Add("guleid", 0);
                relevanceitem.Add("guleseaction", "灌胶");
                relevanceitem.Add("gulename", glue.Count > 0 ? string.Join("➵", glue.ToArray()) : null);
                relevanceArray.Add(relevanceitem);
            }
            return Content(JsonConvert.SerializeObject(relevanceArray));
        }


        //根据传过来的三方和灌胶,找模组列表
        public ActionResult GetMoudleList(int threeid, int guleid, string type, string Platform, string modulelist, string Maintenance)
        {
            JArray relevanceArray = new JArray();
            JObject result = new JObject();
            JObject itemjobject = new JObject();
            var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == type && c.Platform == Platform && c.Maintenance == Maintenance && c.PlatformModul == modulelist && (c.FatherID == threeid || c.FatherID == guleid) && c.ChildSeaction == "模组装配").ToList();//找到符合条件的所以关联表 
            var relevance2 = db.Process_Capacity_Relevance.Where(c => c.Type == type && c.Platform == Platform && c.Maintenance == Maintenance && c.PlatformModul == modulelist && c.FatherSeaction == "模组装配" && c.ChildSeaction == "包装").ToList();//找到符合条件的所以关联表 

            if (relevance.Count != 0)
            {
                var each = relevance.Where((x, y) => relevance.FindIndex(z => z.ChildID == x.ChildID) == y).ToList();
                JArray way = new JArray();
                JArray category = new JArray();
                JArray size = new JArray();
                foreach (var item in each)
                {
                    var name = GetNameitem("模组装配", item.ChildID);
                    var array = name.Split('➸');
                    if (array.Count() == 3)
                    {
                        if (!way.Children().Contains(array[0]))
                        {
                            way.Add(array[0]);
                        }
                        if (!category.Children().Contains(array[1]))
                        {
                            category.Add(array[1]);
                        }
                        if (!size.Children().Contains(array[2]))
                        {
                            size.Add(array[2]);
                        }
                    }
                    else
                    {
                        size.Add(name);
                    }
                    JObject relevanceitem = new JObject();
                    relevanceitem.Add("id", item.ChildID);
                    relevanceitem.Add("seaction", "模组装配");
                    relevanceitem.Add("name", GetNameitem("模组装配", item.ChildID));
                    relevanceArray.Add(relevanceitem);
                }
                itemjobject.Add("way", way);//出货方式
                itemjobject.Add("category", category);//模组类别
                itemjobject.Add("size", size);//箱体尺寸

            }
            else if (relevance2.Count != 0)
            {
                var each = relevance2.Where((x, y) => relevance2.FindIndex(z => z.FatherID == x.FatherID) == y).ToList();
                JArray way = new JArray();
                JArray category = new JArray();
                JArray size = new JArray();
                foreach (var item in each)
                {
                    var name = GetNameitem("模组装配", item.FatherID);
                    var array = name.Split('➸');
                    if (array.Count() == 3)
                    {
                        if (!way.Children().Contains(array[0]))
                        {
                            way.Add(array[0]);
                        }
                        if (!category.Children().Contains(array[1]))
                        {
                            category.Add(array[1]);
                        }
                        if (!size.Children().Contains(array[2]))
                        {
                            size.Add(array[2]);
                        }
                    }
                    else
                    {
                        size.Add(name);
                    }
                    JObject relevanceitem = new JObject();
                    relevanceitem.Add("id", item.FatherID);
                    relevanceitem.Add("seaction", "模组装配");
                    relevanceitem.Add("name", GetNameitem("模组装配", item.FatherID));
                    relevanceArray.Add(relevanceitem);
                }
                itemjobject.Add("way", way);//出货方式
                itemjobject.Add("category", category);//模组类别
                itemjobject.Add("size", size);//箱体尺寸
            }
            else
            {
                JObject relevanceitem = new JObject();
                List<string> moduleRelevance = new List<string>(); ;
                var module = db.ProcessBalance.Where(c => c.Type == type && c.Platform == Platform && c.Maintenance == Maintenance && c.PlatformModul == modulelist && c.Section == "模组装配").Select(c => new { c.Title, c.ProcessDescription }).ToList();
                List<string> way = new List<string>();
                List<string> category = new List<string>();
                List<string> size = new List<string>();
                foreach (var item in module)
                {
                    var name = item.ProcessDescription == null ? item.Title : item.ProcessDescription;
                    var array = name.Split('➸');
                    if (array.Count() == 3)
                    {
                        if (!way.Contains(array[0]))
                        {
                            way.Add(array[0]);
                        }
                        if (!category.Contains(array[1]))
                        {
                            category.Add(array[1]);
                        }
                        if (!size.Contains(array[2]))
                        {
                            size.Add(array[2]);
                        }
                    }
                    else
                    {
                        size.Add(name);
                    }
                }
                itemjobject.Add("way", string.Join("+", way.ToArray()));//出货方式
                itemjobject.Add("category", string.Join("+", category.ToArray()));//模组类别
                itemjobject.Add("size", string.Join("+", size.ToArray()));//箱体尺寸

                module.ForEach(c => moduleRelevance.Add(c.ProcessDescription == null ? c.Title : c.ProcessDescription));
                relevanceitem.Add("id", 0);
                relevanceitem.Add("seaction", "模组装配");
                relevanceitem.Add("name", string.Join("+", moduleRelevance.ToArray()));
                relevanceArray.Add(relevanceitem);
            }

            result.Add("selectOption", itemjobject);
            result.Add("idArry", relevanceArray);
            return Content(JsonConvert.SerializeObject(result));
        }
        //根据传过来的三方,灌胶,模组列表找 包装列表
        public ActionResult GetPackList(int threeid, int guleid, int moduleid, string type, string Platform, string modulelist, string Maintenance)
        {
            JArray relevanceArray = new JArray();
            var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == type && c.Platform == Platform && c.Maintenance == Maintenance && c.PlatformModul == modulelist && (c.FatherID == threeid || c.FatherID == guleid || c.FatherID == moduleid) && c.ChildSeaction == "包装").ToList();//找到符合条件的所以关联表 
            if (relevance.Count == 0)
            {
                JObject relevanceitem = new JObject();
                List<string> moduleRelevance = new List<string>();
                var module = db.ProcessBalance.Where(c => c.Type == type && c.Platform == Platform && c.Maintenance == Maintenance && c.PlatformModul == modulelist && c.Section == "包装").Select(c => new { c.Title, c.ProcessDescription }).ToList();
                module.ForEach(c => moduleRelevance.Add(c.ProcessDescription == null ? c.Title : c.ProcessDescription));
                relevanceitem.Add("id", 0);
                relevanceitem.Add("seaction", "包装");
                relevanceitem.Add("name", string.Join("+", moduleRelevance.ToArray()));
                relevanceArray.Add(relevanceitem);
            }
            else
            {
                foreach (var item in relevance)
                {
                    JObject relevanceitem = new JObject();
                    relevanceitem.Add("id", item.ChildID);
                    relevanceitem.Add("seaction", "包装");
                    relevanceitem.Add("name", GetNameitem("包装", item.ChildID));
                    relevanceArray.Add(relevanceitem);
                }
            }
            return Content(JsonConvert.SerializeObject(relevanceArray));
        }
        //根据 三方,灌胶,模组列表 包装 找每小时产能
        public ActionResult GetCapHouse(int threeid, int guleid, int moduleid, int packid, string type, string Platform, string modulelist, string Maintenance, bool isEquipment = true, decimal Referencecapacity = 0m, decimal ReferenceModuleNeed = 0m)
        {
            #region 之前
            #region 取值
            ////SMT
            //var SmtList = db.Pick_And_Place.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type).Select(c => new TempSmt { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, PersonNum = c.PersonNum, CapacityPerHour = c.CapacityPerHour, ProcessDescription = c.ProcessDescription, PlafModule = c.PlatformModul, Maintenance = c.Maintenance ,id=c.Id}).ToList();
            ////手输表集合
            //var ManualList = db.Process_Capacity_Manual.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type).Select(c => new TempCapacity { Id = c.Id, Type = c.Type, ProductPCBnumber = c.ProductPCBnumber, Platform = c.Platform, StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name, Name2 = c.Name2, StandardNumber2 = c.StandardNumber2, StandardCapacity2 = c.StandardCapacity2, PlatformModul = c.PlatformModul, Maintenance = c.Maintenance, Section = c.Section }).ToList();
            ////平衡卡
            //var Balance = db.ProcessBalance.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type).Select(c => new TempBlance { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.StandardOutput, StandardTotal = c.StandardTotal, Section = c.Section, SerialNumber = c.SerialNumber, Name = c.ProcessDescription, PlafModule = c.PlatformModul, Maintenance = c.Maintenance }).ToList();
            ////设备表
            //var Equipment = db.Process_Capacity_Equipment.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type).Select(c => new TempEquipment { Seaction = c.Seaction, Capacity = c.EquipmentCapacity, Name = c.EquipmentName, Number = c.EquipmentNum, Statue = c.Statue, PersonEquipmentNum = c.PersonEquipmentNum,SeactionID=c.SeactionID }).ToList();
            #endregion


            //JObject itemvalue = CalculateCapacityPerHour(SmtList, ManualList, Balance, Equipment, Platform, type, modulelist, threeid, guleid, moduleid, packid, Maintenance, isEquipment, Referencecapacity);//调用通用方法,得到值
            //JObject jobjectitem = new JObject();
            #endregion
            #region 修改
            JObject jobjectitem = new JObject();
            JArray listitem = new JArray();
            var seaction = new List<string>();
            //List<RelevanceList> relajaaray = new List<RelevanceList>();
            var Equipment = db.Process_Capacity_Equipment.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type).Select(c => new TempEquipment { Seaction = c.Seaction, Capacity = c.EquipmentCapacity, Name = c.EquipmentName, Number = c.EquipmentNum, Statue = c.Statue, PersonEquipmentNum = c.PersonEquipmentNum, SeactionID = c.SeactionID }).ToList();
            #region 取值所需人数计算,需要拿到改平台的所有工段的标准人数
            //模组装配
            var module = new ProcessBalance();
            if (moduleid != 0)
            {
                module = db.ProcessBalance.Where(c => c.Id == moduleid).FirstOrDefault();
            }
            else
            {
                module = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Platform == Platform && c.Maintenance == Maintenance && c.PlatformModul == modulelist && c.Type == type && c.Section == "模组装配").FirstOrDefault();

            }
            if (module == null)
            {
                jobjectitem.Add("person", 0);//所需人数
                jobjectitem.Add("processingFee", "");//加工费用
                jobjectitem.Add("capacityPerHour", 0);//每小时产能
                jobjectitem.Add("data", false);
                return Content(JsonConvert.SerializeObject(jobjectitem));
            }
            //smt
            var icsmt = db.Pick_And_Place.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.ProcessDescription == "IC面贴装").OrderBy(c => c.PersonNum).Select(c => new TempSmt { CapacityPerHour = c.CapacityPerHour, PersonNum = c.PersonNum, id = c.Id }).FirstOrDefault();

            var linesmt = db.Pick_And_Place.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.ProcessDescription == "灯面贴装").OrderBy(c => c.PersonNum).Select(c => new TempSmt { CapacityPerHour = c.CapacityPerHour, PersonNum = c.PersonNum, id = c.Id }).FirstOrDefault();

            //插件
            var plugin = db.Process_Capacity_Manual.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Section == "插件").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name, id = c.Id }).ToList();

            //后焊

            var aftertotal = db.ProcessBalance.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Section == "后焊").Select(c => new TempBlance { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.StandardOutput, StandardTotal = c.StandardTotal, Section = c.Section, SerialNumber = c.SerialNumber, Name = c.ProcessDescription, PlafModule = c.PlatformModul, Maintenance = c.Maintenance }).ToList();
            var after = GetNewSerinum(aftertotal);

            //三防
            var three = new List<Temp>();
            if (threeid != 0)
            {
                three = db.Process_Capacity_Manual.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Id == threeid).Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name, id = c.Id }).ToList();
            }
            else
            {
                three = db.Process_Capacity_Manual.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Section == "三防").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name, id = c.Id }).ToList();
                //three = GetNewSerinum(threetotal);
            }
            //打底壳
            var botton = db.Process_Capacity_Manual.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Section == "打底壳").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name, id = c.Id }).ToList(); ;
            //var botton = GetNewSerinum(bottontotal);

            //装磁吸

            var magnetitotal = db.ProcessBalance.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Section == "装磁吸安装板").Select(c => new TempBlance { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.StandardOutput, StandardTotal = c.StandardTotal, Section = c.Section, SerialNumber = c.SerialNumber, Name = c.ProcessDescription, PlafModule = c.PlatformModul, Maintenance = c.Maintenance }).ToList();
            var magneti = GetNewSerinum(magnetitotal);

            //喷墨
            var Inkjet = db.Process_Capacity_Manual.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Section == "喷墨").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name, id = c.Id }).ToList();

            //模块线

            var moduleLinetotal = db.ProcessBalance.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Section == "模块线").Select(c => new TempBlance { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.StandardOutput, StandardTotal = c.StandardTotal, Section = c.Section, SerialNumber = c.SerialNumber, Name = c.ProcessDescription, PlafModule = c.PlatformModul, Maintenance = c.Maintenance }).ToList();
            var moduleLine = GetNewSerinum(moduleLinetotal);

            //灌胶
            var glue = new List<Temp>();
            if (guleid != 0)
            {
                glue = db.Process_Capacity_Manual.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Id == guleid).Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name, id = c.Id }).ToList();
            }
            else
            {
                glue = db.Process_Capacity_Manual.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Section == "灌胶").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name, id = c.Id }).ToList();
            }
            //气密
            var airtight = db.Process_Capacity_Manual.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Section == "气密").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name, id = c.Id }).ToList();

            //锁面罩
            var lockmas = db.Process_Capacity_Manual.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Section == "锁面罩").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name, id = c.Id }).ToList();
            //var lockmas = GetNewSerinum(lockmastotal);


            //老化
            var burnin = db.Process_Capacity_Manual.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Section == "老化").ToList();

            //包装
            var packing = new List<TempBlance>();
            if (packid != 0)
            {
                packing = db.ProcessBalance.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Id == packid).Select(c => new TempBlance { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.StandardOutput, StandardTotal = c.StandardTotal, Section = c.Section, SerialNumber = c.SerialNumber, Name = c.ProcessDescription, PlafModule = c.PlatformModul, Maintenance = c.Maintenance }).ToList();
            }
            else
            {
                var packingtotal = db.ProcessBalance.Where(c => c.Platform == Platform && c.PlatformModul == modulelist && c.Maintenance == Maintenance && c.Type == type && c.Section == "包装").Select(c => new TempBlance { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.StandardOutput, StandardTotal = c.StandardTotal, Section = c.Section, SerialNumber = c.SerialNumber, Name = c.ProcessDescription, PlafModule = c.PlatformModul, Maintenance = c.Maintenance }).ToList();
                packing = GetNewSerinum(packingtotal);
            }
            #endregion

            #region 单个工段
            //var icsmtpersonNum = 0;//smt总人数
            //decimal icsmtWarkHours = 0m; //smt每小时产能
            //decimal icsmtCapacityPerHour = 0m;//smt标准产能

            //var linesmtpersonNum = 0;//smt总人数
            //decimal linesmtWarkHours = 0m; //smt每小时产能
            //decimal linesmtCapacityPerHour = 0m;//smt标准产能

            //var pluginpersonNum = 0;//插件总人数
            //decimal pluginWarkHours = 0m; //插件每小时产能
            //decimal pluginCapacityPerHour = 0m;//插件标准产能

            //var afterpersonNum = 0;//后焊总人数
            //decimal afterWarkHours = 0m; //后焊每小时产能
            //decimal afterCapacityPerHour = 0m;//后焊标准产能

            //var threePersonNum = 0;//三防总人数
            //var threeName = "";//三防名字
            //decimal threeWarkHours = 0m; //三防每小时产能
            //decimal threeCapacityPerHour = 0m;//三防标准产能

            //var bottonPersonNum = 0;//打底壳总人数
            //decimal bottonWarkHours = 0m; //打底壳每小时产能
            //decimal bottonCapacityPerHour = 0m;//打底壳标准产能

            //var magnetiPersonNum = 0;//装磁吸总人数
            //var magnetiName = "";//装磁吸
            //decimal magnetiWarkHours = 0m; //装磁吸每小时产能
            //decimal magnetiCapacityPerHour = 0m;//装磁吸标准产能

            //var InkjetPersonNum = 0;//喷墨总人数
            //decimal InkjetWarkHours = 0m; //喷墨每小时产能
            //decimal InkjetCapacityPerHour = 0m;//喷墨标准产能

            var glueid = 0;
            var gluePersonNum = 0;//灌胶总人数
            decimal glueWarkHours = 0m; //灌胶每小时产能
            decimal glueCapacityPerHour = 0m;//灌胶标准产能

            //var airtightPersonNum = 0;//气密总人数
            //decimal airtightWarkHours = 0m; //气密每小时产能
            //decimal airtightCapacityPerHour = 0m;//气密标准产能

            //var lockmasPersonNum = 0;//锁面罩总人数
            //decimal lockmasWarkHours = 0m; //锁面罩每小时产能
            //decimal lockmasCapacityPerHour = 0m;//锁面罩标准产能

            //var moduleLinePersonNum = 0;//模块线总人数
            //decimal moduleLineWarkHours = 0m; //模块线每小时产能
            //decimal moduleLineCapacityPerHour = 0m;//模块线标准产能

            var mudulePersonNum = 0;//总人数
            decimal moduleWarkHours = 0m;//每小时产能
            decimal moduleCapacityPerHour = 0m;//标准产能

            var burnPersonNum = 0;//总人数
            decimal burnWarkHours = 0m;//每小时产能
            decimal burnCapacityPerHour = 0m;//标准产能

            var burnPersonNum2 = 0;//总人数
            decimal burnWarkHours2 = 0m;//每小时产能
            decimal burnCapacityPerHour2 = 0m;//标准产能

            var packPersonNum = 0;//总人数
            decimal packWarkHours = 0m;//每小时产能
            decimal packCapacityPerHour = 0m;//标准产能
            #endregion

            #region 汇总

            var totalpersonNum = 0;//总人数
            decimal totalWarkHours = 0m;

            //var smttotalpersonNum = 0;//smt总人数
            //decimal smttotalWarkHours = 0m; //smt每小时产能

            //var mudulestotalpersonNum = 0;//总人数
            //decimal mudulestotalWarkHours = 0m;//模块每小时产能

            #endregion

            #region  新的
            JArray temparrray = new JArray();

            JObject array = new JObject();
            array.Add("name", temparrray);
            array.Add("person", temparrray);
            array.Add("controllerNum", temparrray);
            array.Add("moduleNeed", temparrray);
            //array.Add("capacityPerHour", temparrray);
            array.Add("equipmentName", temparrray);
            array.Add("equipmentNum", temparrray);
            array.Add("equipmentCapacity", temparrray);
            array.Add("balanceMultiple", temparrray);
            array.Add("balanceCapacity", temparrray);
            array.Add("balancePerson", temparrray);
            array.Add("balanceEquipmentNum", temparrray);
            array.Add("balanceRate", temparrray);
            array.Add("totalperson", temparrray);
            array.Add("totalWarkHours", temparrray);
            decimal reference;
            decimal referencemodule;
            #endregion

            #region 循环各个工段的数据,拿到人数集合,和各个工段的单人模组需求总时(1/(标准产能/人数/模组需求))
            //smt
            decimal workHoure = Math.Round(icsmt.CapacityPerHour == 0 ? 0 : module.SMTModuleNeedNum / icsmt.CapacityPerHour, 2);
            #region 取得平衡倍数
            var icmultiple = 0;
            int linemultiple = 0;
            //decimal multipleRate;
            #region 之前以平衡倍数都达到85以上
            //do
            //{
            //    if (icsmt.CapacityPerHour > linesmt.CapacityPerHour)
            //    {
            //        icmultiple++;
            //        var tempic = icsmt.CapacityPerHour * icmultiple;
            //        linemultiple = (int)Math.Floor(tempic / linesmt.CapacityPerHour);
            //        var temp2 = linesmt.CapacityPerHour * linemultiple;
            //        if (temp2 > tempic)
            //            multipleRate = tempic / temp2;
            //        else
            //            multipleRate = temp2 / tempic;
            //        if (multipleRate < (decimal)0.85)
            //        {
            //            linemultiple = (int)Math.Ceiling(tempic / linesmt.CapacityPerHour);
            //            var temp3 = linesmt.CapacityPerHour * linemultiple;
            //            if (temp3 > tempic)
            //                multipleRate = tempic / temp3;
            //            else
            //                multipleRate = temp3 / tempic;
            //        }

            //    }
            //    else
            //    {
            //        linemultiple++;
            //        var templine = linesmt.CapacityPerHour * linemultiple;
            //        icmultiple = (int)Math.Floor(templine / icsmt.CapacityPerHour);
            //        var temp2 = icsmt.CapacityPerHour * icmultiple;
            //        if (temp2 > templine)
            //            multipleRate = templine / temp2;
            //        else
            //            multipleRate = temp2 / templine;
            //        if (multipleRate < (decimal)0.85)
            //        {
            //            icmultiple = (int)Math.Ceiling(templine / icsmt.CapacityPerHour);
            //            var temp3 = icsmt.CapacityPerHour * icmultiple;
            //            if (temp3 > templine)
            //                multipleRate = templine / temp3;
            //            else
            //                multipleRate = temp3 / templine;
            //        }
            //    }
            //} while (multipleRate < (decimal)0.85);
            #endregion
            if (icsmt.CapacityPerHour > linesmt.CapacityPerHour)
            {
                icmultiple = 1;
                linemultiple = (int)Math.Truncate(icsmt.CapacityPerHour / linesmt.CapacityPerHour + (decimal)0.5);
            }
            if (icsmt.CapacityPerHour < linesmt.CapacityPerHour)
            {
                linemultiple = 1;
                icmultiple = (int)Math.Truncate(linesmt.CapacityPerHour / icsmt.CapacityPerHour + (decimal)0.5);
            }
            #endregion
            var maxcap = icsmt.CapacityPerHour * icmultiple > linesmt.CapacityPerHour * linemultiple ? icsmt.CapacityPerHour * icmultiple : linesmt.CapacityPerHour * linemultiple;
            if (icsmt.CapacityPerHour != 0m)
            {
                DetailObj(icsmt.id, "IC", "SMT", icsmt.PersonNum, icsmt.CapacityPerHour, workHoure, module.SMTModuleNeedNum, icmultiple, Equipment, maxcap, array);
            }

            decimal workHoure2 = Math.Round(linesmt.CapacityPerHour == 0 ? 0 : module.SMTModuleNeedNum / linesmt.CapacityPerHour, 2);
            if (linesmt.CapacityPerHour != 0m)
            {
                DetailObj(linesmt.id, "灯面", "SMT", linesmt.PersonNum, linesmt.CapacityPerHour, workHoure2, module.SMTModuleNeedNum, linemultiple, Equipment, maxcap, array);
            }
            //灌胶
            glue.ForEach(c =>
            {
                gluePersonNum = gluePersonNum + c.StandardNumber;
                glueCapacityPerHour = glueCapacityPerHour + c.StandardCapacity;
                glueWarkHours = glueWarkHours + (c.StandardCapacity == 0 ? 0 : module.GuleModuleNeedNum / c.StandardCapacity);
                glueid = c.id;
            });

            #region 拿到模块段的参考值
            if (Referencecapacity == 0M)
            {
                jobjectitem.Add("referenceName", "灌胶");//默认值的工段名字
                reference = Equipment.Where(c => c.Seaction == "灌胶").Select(c => c.Capacity).FirstOrDefault();
                if (reference == 0)
                    reference = glueCapacityPerHour;
            }
            else
            {
                reference = Referencecapacity;
            }
            if (ReferenceModuleNeed == 0M)
                referencemodule = module.GuleModuleNeedNum;
            else
                referencemodule = ReferenceModuleNeed;
            #endregion

                //插件
            plugin.ForEach(c =>
            {
                //pluginpersonNum = pluginpersonNum + c.StandardNumber;
                //pluginCapacityPerHour = pluginCapacityPerHour + c.StandardCapacity;
                //pluginWarkHours = pluginWarkHours + (c.StandardCapacity == 0 ? 0 : module.PluginModuleNeedNum / c.StandardCapacity);
                if (c.StandardCapacity != 0m)
                {
                    DetailObj(c.id, c.Name, "插件", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.PluginModuleNeedNum / c.StandardCapacity, module.PluginModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                }
            });
            //后焊
            after.ForEach(c =>
            {
                //afterpersonNum = afterpersonNum + c.StandardTotal;
                //afterCapacityPerHour = afterCapacityPerHour + c.StandardCapacity;
                //afterWarkHours = afterWarkHours + (c.StandardCapacity == 0 ? 0 : module.AfterModuleNeedNum / c.StandardCapacity);
                if (c.StandardCapacity != 0m)
                {
                    DetailObj(c.Id, c.Name, "后焊", c.StandardTotal, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.AfterModuleNeedNum / c.StandardCapacity, module.AfterModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                }
            });

            if (three.Count(c => c.Name.Contains("灌胶")) > 0)
            {
                // threeName = three.Where(c => c.Name.Contains("灌胶")).Select(c => c.Name).FirstOrDefault();
                if (magneti.Count(c => c.Name == "底壳装快锁") > 0)
                {
                    // 装磁吸
                    magneti.ForEach(c =>
                    {
                        //magnetiPersonNum = magnetiPersonNum + c.StandardTotal;
                        //magnetiCapacityPerHour = magnetiCapacityPerHour + c.StandardCapacity;
                        //magnetiWarkHours = magnetiWarkHours + (c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Id, c.Name, "装磁吸安装板", c.StandardTotal, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity, module.MagneticModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                        }
                    });
                    //打底壳
                    botton.ForEach(c =>
                    {
                        //bottonPersonNum = bottonPersonNum + c.StandardNumber;
                        //bottonCapacityPerHour = bottonCapacityPerHour + c.StandardCapacity;
                        //bottonWarkHours = bottonWarkHours + (c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.id, c.Name, "打底壳", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity, module.BottnModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                        }
                    });
                    //喷墨
                    Inkjet.ForEach(c =>
                    {
                        //InkjetPersonNum = InkjetPersonNum + c.StandardNumber;
                        //InkjetCapacityPerHour = InkjetCapacityPerHour + c.StandardCapacity;
                        //InkjetWarkHours = InkjetWarkHours + (c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.id, c.Name, "喷墨", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity, module.InjekModuleNeedNum, 0, Equipment, 0, array, c.StandardCapacity, module.InjekModuleNeedNum);
                        }
                    });
                    //三放
                    three.ForEach(c =>
                    {
                        //threePersonNum = threePersonNum + c.StandardNumber;
                        //threeCapacityPerHour = threeCapacityPerHour + c.StandardCapacity;
                        //threeWarkHours = threeWarkHours + (c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.id, c.Name, "三防", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity, module.ThreeModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                        }
                    });
                }
                else
                {
                    //打底壳
                    botton.ForEach(c =>
                    {
                        //bottonPersonNum = bottonPersonNum + c.StandardNumber;
                        //bottonCapacityPerHour = bottonCapacityPerHour + c.StandardCapacity;
                        //bottonWarkHours = bottonWarkHours + (c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.id, c.Name, "打底壳", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity, module.BottnModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                        }
                    });
                    // 装磁吸
                    magneti.ForEach(c =>
                    {
                        //magnetiPersonNum = magnetiPersonNum + c.StandardTotal;
                        //magnetiCapacityPerHour = magnetiCapacityPerHour + c.StandardCapacity;
                        //magnetiWarkHours = magnetiWarkHours + (c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Id, c.Name, "装磁吸安装板", c.StandardTotal, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity, module.MagneticModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                        }
                    });
                    //喷墨
                    Inkjet.ForEach(c =>
                    {
                        //InkjetPersonNum = InkjetPersonNum + c.StandardNumber;
                        //InkjetCapacityPerHour = InkjetCapacityPerHour + c.StandardCapacity;
                        //InkjetWarkHours = InkjetWarkHours + (c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.id, c.Name, "喷墨", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity, module.InjekModuleNeedNum, 0, Equipment, 0, array, c.StandardCapacity, module.InjekModuleNeedNum);
                        }
                    });
                    //三放
                    three.ForEach(c =>
                    {
                        //threePersonNum = threePersonNum + c.StandardNumber;
                        //threeCapacityPerHour = threeCapacityPerHour + c.StandardCapacity;
                        //threeWarkHours = threeWarkHours + (c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.id, c.Name, "三防", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity, module.ThreeModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                        }
                    });
                }
            }
            else
            {
                if (magneti.Count(c => c.Name == "底壳装快锁") > 0)
                {
                    //三放
                    three.ForEach(c =>
                    {
                        //threePersonNum = threePersonNum + c.StandardNumber;
                        //threeCapacityPerHour = threeCapacityPerHour + c.StandardCapacity;
                        //threeWarkHours = threeWarkHours + (c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.id, c.Name, "三防", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity, module.ThreeModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                        }
                    });
                    // 装磁吸
                    magneti.ForEach(c =>
                    {
                        //magnetiPersonNum = magnetiPersonNum + c.StandardTotal;
                        //magnetiCapacityPerHour = magnetiCapacityPerHour + c.StandardCapacity;
                        //magnetiWarkHours = magnetiWarkHours + (c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Id, c.Name, "装磁吸安装板", c.StandardTotal, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity, module.MagneticModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                        }
                    });
                    //打底壳
                    botton.ForEach(c =>
                    {
                        //bottonPersonNum = bottonPersonNum + c.StandardNumber;
                        //bottonCapacityPerHour = bottonCapacityPerHour + c.StandardCapacity;
                        //bottonWarkHours = bottonWarkHours + (c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.id, c.Name, "打底壳", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity, module.BottnModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                        }
                    });
                    //喷墨
                    Inkjet.ForEach(c =>
                    {
                        //InkjetPersonNum = InkjetPersonNum + c.StandardNumber;
                        //InkjetCapacityPerHour = InkjetCapacityPerHour + c.StandardCapacity;
                        //InkjetWarkHours = InkjetWarkHours + (c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.id, c.Name, "喷墨", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity, module.InjekModuleNeedNum, 0, Equipment, 0, array, c.StandardCapacity, module.InjekModuleNeedNum);
                        }
                    });
                }
                else
                {
                    //三放
                    three.ForEach(c =>
                    {
                        //threePersonNum = threePersonNum + c.StandardNumber;
                        //threeCapacityPerHour = threeCapacityPerHour + c.StandardCapacity;
                        //threeWarkHours = threeWarkHours + (c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.id, c.Name, "三防", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity, module.ThreeModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                        }
                    });
                    //打底壳
                    botton.ForEach(c =>
                    {
                        //bottonPersonNum = bottonPersonNum + c.StandardNumber;
                        //bottonCapacityPerHour = bottonCapacityPerHour + c.StandardCapacity;
                        //bottonWarkHours = bottonWarkHours + (c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.id, c.Name, "打底壳", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity, module.BottnModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                        }
                    });
                    // 装磁吸
                    magneti.ForEach(c =>
                    {
                        //magnetiPersonNum = magnetiPersonNum + c.StandardTotal;
                        //magnetiCapacityPerHour = magnetiCapacityPerHour + c.StandardCapacity;
                        //magnetiWarkHours = magnetiWarkHours + (c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Id, c.Name, "装磁吸安装板", c.StandardTotal, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity, module.MagneticModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                        }
                    });
                    //喷墨
                    Inkjet.ForEach(c =>
                    {
                        //InkjetPersonNum = InkjetPersonNum + c.StandardNumber;
                        //InkjetCapacityPerHour = InkjetCapacityPerHour + c.StandardCapacity;
                        //InkjetWarkHours = InkjetWarkHours + (c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.id, c.Name, "喷墨", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity, module.InjekModuleNeedNum, 0, Equipment, 0, array, c.StandardCapacity, module.InjekModuleNeedNum);
                        }
                    });
                }
            }
            if (glueCapacityPerHour != 0m)
            {
                DetailObj(glueid, "灌胶", "灌胶", gluePersonNum, glueCapacityPerHour, glueWarkHours, module.GuleModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
            }
            //气密
            airtight.ForEach(c =>
            {
                //airtightPersonNum = airtightPersonNum + c.StandardNumber;
                //airtightCapacityPerHour = airtightCapacityPerHour + c.StandardCapacity;
                //airtightWarkHours = airtightWarkHours + (c.StandardCapacity == 0 ? 0 : module.AirtightModuleNeedNum / c.StandardCapacity);
                if (c.StandardCapacity != 0m)
                {
                    DetailObj(c.id, c.Name, "气密", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.AirtightModuleNeedNum / c.StandardCapacity, module.AirtightModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                }
            });
            //锁面罩
            lockmas.ForEach(c =>
            {
                //lockmasPersonNum = lockmasPersonNum + c.StandardNumber;
                //lockmasCapacityPerHour = lockmasCapacityPerHour + c.StandardCapacity;
                //lockmasWarkHours = lockmasWarkHours + (c.StandardCapacity == 0 ? 0 : module.LockMasModuleNeedNum / c.StandardCapacity);
                if (c.StandardCapacity != 0m)
                {
                    DetailObj(c.id, c.Name, "锁面罩", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.LockMasModuleNeedNum / c.StandardCapacity, module.LockMasModuleNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                }
            });


            //模块线
            moduleLine.ForEach(c =>
            {
                //moduleLinePersonNum = moduleLinePersonNum + c.StandardTotal;
                //moduleLineCapacityPerHour = moduleLineCapacityPerHour + c.StandardCapacity;
                //moduleLineWarkHours = moduleLineWarkHours + (c.StandardCapacity == 0 ? 0 : module.ModuleLineNeedNum / c.StandardCapacity);
                if (c.StandardCapacity != 0m)
                {
                    DetailObj(c.Id, c.Name, "模块线", c.StandardTotal, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.ModuleLineNeedNum / c.StandardCapacity, module.ModuleLineNeedNum, 0, Equipment, 0, array, reference, referencemodule);
                }
            });
            //模组装配
            mudulePersonNum = mudulePersonNum + module.StandardTotal;
            moduleCapacityPerHour = moduleCapacityPerHour + module.StandardOutput;
            moduleWarkHours = moduleWarkHours + (module.StandardOutput == 0 ? 0 : module.ModuleNeedNum2 / module.StandardOutput);
            if (moduleCapacityPerHour != 0m)
            {
                DetailObj(0, "模组装配", "模组装配", mudulePersonNum, moduleCapacityPerHour, moduleWarkHours, module.ModuleNeedNum2, 1, Equipment, moduleCapacityPerHour, array);
            }
            //老化
            burnin.ForEach(c =>
            {
                burnPersonNum = burnPersonNum + c.StandardNumber;
                burnCapacityPerHour = burnCapacityPerHour + c.StandardCapacity;
                burnWarkHours = burnWarkHours + (c.StandardCapacity == 0 ? 0 : module.BuriInModuleNeedNum / c.StandardCapacity);
                burnPersonNum2 = burnPersonNum2 + c.StandardNumber2;
                burnCapacityPerHour2 = burnCapacityPerHour2 + c.StandardCapacity2;
                burnWarkHours2 = burnWarkHours2 + (c.StandardCapacity2 == 0 ? 0 : module.BuriInModuleNeedNum / c.StandardCapacity2);
            });
            if (burnCapacityPerHour != 0m)
            {
                DetailObj(0, "拼屏", "拼屏", burnPersonNum, burnCapacityPerHour, burnWarkHours, module.BuriInModuleNeedNum, 1, Equipment, burnCapacityPerHour, array);
            }
            if (burnCapacityPerHour2 != 0m)
            {
                DetailObj(0, "拆屏", "拆屏", burnPersonNum2, burnCapacityPerHour2, burnWarkHours2, module.BuriInModuleNeedNum, 1, Equipment, burnCapacityPerHour2, array);
            }
            //包装
            packing.ForEach(c =>
            {
                packPersonNum = packPersonNum + c.StandardTotal;
                packCapacityPerHour = packCapacityPerHour + c.StandardCapacity;
                packWarkHours = packWarkHours + (c.StandardCapacity == 0 ? 0 : module.PackModuleNeedNum / c.StandardCapacity);
            });
            if (packCapacityPerHour != 0m)
            {
                DetailObj(0, "包装", "包装", packPersonNum, packCapacityPerHour, packWarkHours, module.PackModuleNeedNum, 1, Equipment, packCapacityPerHour, array);
            }
            #endregion

            #region 汇总
            //smttotalWarkHours = workHoure + workHoure2 + pluginWarkHours + afterWarkHours;
            //smttotalpersonNum = icsmtpersonNum + linesmtpersonNum + pluginpersonNum + afterpersonNum;
            //mudulestotalWarkHours = threeWarkHours + bottonWarkHours + magnetiWarkHours + InkjetWarkHours + glueWarkHours + airtightWarkHours + lockmasWarkHours + moduleLineWarkHours;
            //mudulestotalpersonNum = threePersonNum + bottonPersonNum + magnetiPersonNum + InkjetPersonNum + gluePersonNum + airtightPersonNum + lockmasPersonNum + moduleLinePersonNum;
            //totalWarkHours = smttotalWarkHours + mudulestotalWarkHours + moduleWarkHours + burnWarkHours + burnWarkHours2 + packWarkHours;
            // totalpersonNum = smttotalpersonNum + mudulestotalpersonNum + mudulePersonNum + burnPersonNum + burnPersonNum2 + packPersonNum;
            foreach (var itemtotalWarkHours in array["totalWarkHours"])
            {
                totalWarkHours = totalWarkHours + (Math.Round( 1 /(decimal) itemtotalWarkHours["mincapacity"] * (decimal)itemtotalWarkHours["moduleneed"],2));
            }

            foreach (var itemperson in array["totalperson"])
            {
                totalpersonNum = totalpersonNum+(int)itemperson;
            }
            jobjectitem.Add("name", array["name"]); //详细表
            jobjectitem.Add("person", array["person"]);//标配人数
            jobjectitem.Add("controllerNum", array["controllerNum"]);//一个人能控制的台数
            jobjectitem.Add("moduleNeed", array["moduleNeed"]);//模组需求数
            //jobjectitem.Add("capacityPerHour", array["capacityPerHour"]);//每小时产能
            jobjectitem.Add("equipmentName", array["equipmentName"]);//设备名字
            jobjectitem.Add("equipmentNum", array["equipmentNum"]);//设备数量
            jobjectitem.Add("equipmentCapacity", array["equipmentCapacity"]);//设备产能
            jobjectitem.Add("balanceMultiple", array["balanceMultiple"]);//平衡后倍数
            jobjectitem.Add("balanceCapacity", array["balanceCapacity"]);//平衡后产能
            jobjectitem.Add("balancePerson", array["balancePerson"]);//平衡后人数
            jobjectitem.Add("balanceEquipmentNum", array["balanceEquipmentNum"]);//平衡后设备需求数
            jobjectitem.Add("balanceRate", array["balanceRate"]);//平衡率

            jobjectitem.Add("totalperson", totalpersonNum);//所需人数
            jobjectitem.Add("processingFee", "");//加工费用
            jobjectitem.Add("totalcapacityPerHour", Math.Round(totalWarkHours, 2));//每小时产能
            jobjectitem.Add("data", true);
            #endregion
            #endregion
            return Content(JsonConvert.SerializeObject(jobjectitem));
        }

        //之前代码,备用
        private JToken detailObj(string name, JToken person, JToken capacity, JToken capacityPerHour)
        {
            JObject jobjectitem = new JObject();
            jobjectitem.Add("name", name);
            jobjectitem.Add("person", person);
            jobjectitem.Add("capacity", capacity);
            jobjectitem.Add("capacityPerHour", capacityPerHour);
            return jobjectitem;
            throw new NotImplementedException();
        }


        private JObject DetailObj(int id, string name, string seaction, int person, decimal capacity, decimal capacityPerHour, decimal moduleneed, int blacnerate, List<TempEquipment> Equipment, decimal smtmax, JObject current, decimal reference = 0m, decimal referenceModule=0m)
        {
            double totalperson = 0;
            JObject jobjectitem = new JObject();
            JArray Aname = (JArray)current["name"]; //名字
            JArray Aperson = (JArray)current["person"];//标准人数
            JArray AcontrollerNum = (JArray)current["controllerNum"];//一个人能控制的台数
            JArray AmoduleNeed = (JArray)current["moduleNeed"];//模组需求
            //JArray AcapacityPerHour = (JArray)current["capacityPerHour"];//生产时间
            JArray AequipmentName = (JArray)current["equipmentName"];//设备名
            JArray AequipmentNum = (JArray)current["equipmentNum"];//设备数量
            JArray AequipmentCapacity = (JArray)current["equipmentCapacity"];//设备产能
            JArray AbalanceMultiple = (JArray)current["balanceMultiple"];//平衡后倍数
            JArray AbalanceCapacity = (JArray)current["balanceCapacity"];//平衡后产能
            JArray AbalancePerson = (JArray)current["balancePerson"];//平衡后人数
            JArray AbalanceRate = (JArray)current["balanceRate"];//平衡率
            JArray AbalanceEquipmentNum = (JArray)current["balanceEquipmentNum"];//平衡后设备台数
            JArray Atotalperson = (JArray)current["totalperson"];//总人数
            JArray AtotalWarkHours = (JArray)current["totalWarkHours"];//单人模组需求时长
            decimal mincapacity = 0m;
            Aname.Add(name);
            //Aperson.Add(person);
            //Acapacity.Add(capacity);
            AmoduleNeed.Add(moduleneed);
            //AcapacityPerHour.Add(Math.Round(capacityPerHour, 2));
            if (Equipment.Count(c => c.Seaction == seaction) != 0)//有设备
            {
                var info = Equipment.Where(c => c.Seaction == seaction && c.SeactionID == id).ToList();//得到设备列表
                JArray temoname = new JArray();
                JArray temonum = new JArray();
                JArray temoncapacity = new JArray();
                JArray balanceMultiple = new JArray();
                JArray balanceCapacity = new JArray();
                JArray balancePerson = new JArray();
                JArray balanceEquipmentNum = new JArray();
                JArray balanceRate = new JArray();
                JArray qeuimentperson = new JArray();
                JArray controllerNum = new JArray();
                info.ForEach(c =>
                {
                    temoname.Add(c.Name);
                    temonum.Add(c.Number);
                    temoncapacity.Add(c.Capacity);
                    var rate = GetBlanceRateFromMoudle(c.Capacity, reference, moduleneed, referenceModule);//计算得到倍数
                    balanceMultiple.Add(rate);
                    balanceCapacity.Add(Math.Round(c.Capacity * rate, 2));
                    if (mincapacity == 0m)
                        mincapacity = Math.Round(c.Capacity * rate, 2);
                    else if (mincapacity > Math.Round(c.Capacity * rate, 2))
                        mincapacity = Math.Round(c.Capacity * rate, 2);
                    balanceEquipmentNum.Add(c.Number * rate);
                    if (c.Statue == "设备")
                    {
                        var blacepersonitem = c.PersonEquipmentNum == 0 ? person * rate : Math.Ceiling((double)c.Number * rate / c.PersonEquipmentNum);
                        balancePerson.Add(blacepersonitem);
                        totalperson = totalperson + blacepersonitem;
                        controllerNum.Add(c.PersonEquipmentNum);
                        qeuimentperson.Add("/");
                    }
                    else
                    {
                        balancePerson.Add(c.PersonEquipmentNum * rate);
                        totalperson = totalperson + c.PersonEquipmentNum * rate;
                        qeuimentperson.Add(c.PersonEquipmentNum);
                        controllerNum.Add("/");
                        //controllerNum.Add(0);
                    }
                    if (c.Capacity * rate > reference)
                        balanceRate.Add(Math.Round(reference * 100 / (c.Capacity * rate), 2) + "%");
                    else
                        balanceRate.Add(Math.Round(c.Capacity * rate * 100 / reference, 2) + "%");
                });
                AequipmentName.Add(temoname);
                AequipmentNum.Add(temonum);
                AequipmentCapacity.Add(temoncapacity);
                AbalanceMultiple.Add(balanceMultiple);
                AbalanceCapacity.Add(balanceCapacity);
                AbalancePerson.Add(balancePerson);
                AbalanceRate.Add(balanceRate);
                Aperson.Add(qeuimentperson);
                AcontrollerNum.Add(controllerNum);
                AbalanceEquipmentNum.Add(balanceEquipmentNum);
            }
            else
            {
                JArray nulljarray = new JArray();
                AequipmentNum.Add(nulljarray);
                AcontrollerNum.Add(nulljarray);
                nulljarray = new JArray();
                nulljarray.Add(capacity);
                AequipmentCapacity.Add(nulljarray);
                nulljarray = new JArray();
                nulljarray.Add("人力");
                AequipmentName.Add(nulljarray);
                nulljarray = new JArray();
                nulljarray.Add(person);
                Aperson.Add(nulljarray);
                nulljarray = new JArray();

                if (blacnerate == 0)
                {
                    blacnerate = GetBlanceRateFromMoudle(capacity, reference, moduleneed, referenceModule);
                }
                nulljarray.Add(blacnerate);
                AbalanceMultiple.Add(nulljarray);
                nulljarray = new JArray();

                nulljarray.Add(Math.Round(capacity * blacnerate, 2));
                mincapacity = Math.Round(capacity * blacnerate, 2);
                AbalanceCapacity.Add(nulljarray);
                nulljarray = new JArray();

                nulljarray.Add(person * blacnerate);
                totalperson = totalperson + person * blacnerate;
                AbalancePerson.Add(nulljarray);
                nulljarray = new JArray();

                var temprate = "";
                if (smtmax == 0)
                {
                    if (capacity * blacnerate > reference)
                        temprate = capacity * blacnerate == 0 ? "0%" : Math.Round(reference * 100 / (capacity * blacnerate), 2) + "%";
                    else

                        temprate = reference == 0 ? "0%" : Math.Round(capacity * blacnerate * 100 / reference, 2) + "%";
                }
                else
                {
                    temprate = Math.Round(capacity * blacnerate * 100 / smtmax, 2) + "%";
                }
                nulljarray.Add(temprate);
                AbalanceRate.Add(nulljarray);
                nulljarray = new JArray();

                // nulljarray.Add(nulljarray1);
                AbalanceEquipmentNum.Add(nulljarray);
            }
            Atotalperson.Add(totalperson);
            JObject item = new JObject();
            if (seaction == "SMT")
            {
                var upate = AtotalWarkHours.Where(c => c["name"].ToString() == "SMT").FirstOrDefault();
                if (upate == null)
                {
                    item.Add("name", "SMT");
                    item.Add("mincapacity", mincapacity);
                    item.Add("moduleneed", moduleneed);
                    AtotalWarkHours.Add(item);
                }
                else
                {
                    var MINcurrent = (decimal)upate["mincapacity"];
                    if (mincapacity < MINcurrent)
                    {
                        upate["mincapacity"] = mincapacity;
                    }
                }
            }
            else if (seaction == "喷墨")
            {
                var upate = AtotalWarkHours.Where(c => c["name"].ToString() == seaction).FirstOrDefault();
                if (upate == null)
                {
                    item.Add("name", seaction);
                    item.Add("mincapacity", mincapacity);
                    item.Add("moduleneed", moduleneed);
                    AtotalWarkHours.Add(item);
                }
                else
                {
                    var MINcurrent = (decimal)upate["mincapacity"];
                    if (mincapacity < MINcurrent)
                    {
                        upate["mincapacity"] = mincapacity;
                    }
                }
            }
            else if (seaction == "模组装配" || seaction == "拆屏" || seaction == "拼屏" || seaction == "包装")
            {
                item.Add("name", seaction);
                item.Add("mincapacity", mincapacity);
                item.Add("moduleneed", moduleneed);
                AtotalWarkHours.Add(item);
            }
            else
            {
                var upate = AtotalWarkHours.Where(c => c["name"].ToString() == "模块").FirstOrDefault();
                if (upate == null)
                {
                    item.Add("name", "模块");
                    item.Add("mincapacity", mincapacity);
                    item.Add("moduleneed", moduleneed);
                    AtotalWarkHours.Add(item);
                }
                else
                {
                    var MINcurrent = (decimal)upate["mincapacity"];
                    if (mincapacity < MINcurrent)
                    {
                        upate["mincapacity"] = mincapacity;
                    }
                }
            }
            /*
             * jobjectitem.Add("balanceMultiple", array["equipmentCapacity"]);//平衡后倍数
            jobjectitem.Add("balanceCapacity", array["equipmentCapacity"]);//平衡后产能
            jobjectitem.Add("balancePerson", array["equipmentCapacity"]);//平衡后人数
            jobjectitem.Add("balanceEquipmentNum", array["equipmentCapacity"]);//平衡后设备需求数
            jobjectitem.Add("balanceRate", array["equipmentCapacity"]);//平衡率
             */

            return current;
        }

        private int GetBlanceRateFromMoudle(decimal capacityPerHour, decimal reference, decimal moduleneed,decimal referenceModule)
        {
            capacityPerHour = moduleneed==0?0: capacityPerHour / moduleneed;
            reference = referenceModule==0?0: reference / referenceModule;
            if (capacityPerHour > reference)
            {
                return 1;
            }
            else
            {
                #region 版本1
                //var temp3 = 0m;
                //var tempint1 = (int)Math.Floor(reference / capacityPerHour);//向下取整
                //var tempint2 = (int)Math.Ceiling(reference / capacityPerHour);//向上取整
                //if (capacityPerHour * tempint1 > reference)
                //    temp3 = reference / (capacityPerHour * tempint1);
                //else
                //    temp3 = (capacityPerHour * tempint1) / reference;
                //if (temp3 >= (decimal)0.85)
                //    return tempint1;
                //else
                //{
                //    var temp4 = 0m;
                //    if (capacityPerHour * tempint2 > reference)
                //        temp4 = reference / (capacityPerHour * tempint2);
                //    else
                //        temp4 = (capacityPerHour * tempint2) / reference;
                //    if (temp4 >= (decimal)0.85)
                //        return tempint2;
                //    else
                //        return temp4 > temp3 ? tempint2 : tempint1;
                //}
                #endregion

                #region 版本2
                int newrate = capacityPerHour==0?0:(int)Math.Truncate((reference / capacityPerHour )+ (decimal)0.5);
                return newrate;
                #endregion
            }


        }

        public JObject CalculateCapacityPerHour(List<TempSmt> SmtList, List<TempCapacity> ManualList, List<TempBlance> Balance, List<TempEquipment> Equipment, string Platform, string type, string platmodule, int threeid, int guleid, int moduleid, int packid, string Maintenance, bool isEquipment = true, decimal Referencecapacity = 0m)
        {
            JObject jobjectitem = new JObject();
            JArray listitem = new JArray();
            var seaction = new List<string>();
            //List<RelevanceList> relajaaray = new List<RelevanceList>();

            #region 取值所需人数计算,需要拿到改平台的所有工段的标准人数
            //模组装配
            var module = new ProcessBalance();
            if (moduleid != 0)
            {
                module = db.ProcessBalance.Where(c => c.Id == moduleid).FirstOrDefault();
            }
            else
            {
                module = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Platform == Platform && c.Maintenance == Maintenance && c.PlatformModul == platmodule && c.Type == type && c.Section == "模组装配").FirstOrDefault();

            }
            if (module == null)
            {
                jobjectitem.Add("person", 0);//所需人数
                jobjectitem.Add("processingFee", "");//加工费用
                jobjectitem.Add("capacityPerHour", 0);//每小时产能
                jobjectitem.Add("array", null);
                return jobjectitem;
            }
            //smt
            var icsmt = new TempSmt();
            var linesmt = new TempSmt();
            var smt1 = SmtList.Where(c => c.ProcessDescription == "IC面贴装").ToList();
            var smt2 = SmtList.Where(c => c.ProcessDescription == "灯面贴装").Select(c => new { c.CapacityPerHour, c.PersonNum }).ToList();
            if (smt1.Count != 0)
            {
                icsmt = smt1.OrderBy(c => c.PersonNum).Select(c => new TempSmt { CapacityPerHour = c.CapacityPerHour, PersonNum = c.PersonNum }).FirstOrDefault();
            }
            if (smt2.Count != 0)
            {
                linesmt = smt2.OrderBy(c => c.PersonNum).Select(c => new TempSmt { CapacityPerHour = c.CapacityPerHour, PersonNum = c.PersonNum }).FirstOrDefault();
            }
            //插件
            var plugin = ManualList.Where(c => c.Section == "插件").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name, id = c.Id }).ToList();

            //后焊

            var aftertotal = Balance.Where(c => c.Section == "后焊").ToList();
            var after = GetNewSerinum(aftertotal);

            //三防
            var three = new List<Temp>();
            if (threeid != 0)
            {
                three = ManualList.Where(c => c.Id == threeid).Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name }).ToList();
            }
            else
            {
                three = ManualList.Where(c => c.Section == "三防").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name }).ToList();
                //three = GetNewSerinum(threetotal);
            }
            //打底壳
            var botton = ManualList.Where(c => c.Section == "打底壳").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name }).ToList(); ;
            //var botton = GetNewSerinum(bottontotal);

            //装磁吸

            var magnetitotal = Balance.Where(c => c.Section == "装磁吸安装板").ToList();
            var magneti = GetNewSerinum(magnetitotal);

            //喷墨
            var Inkjet = ManualList.Where(c => c.Section == "喷墨").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name }).ToList();

            //模块线

            var moduleLinetotal = Balance.Where(c => c.Section == "模块线").ToList();
            var moduleLine = GetNewSerinum(moduleLinetotal);

            //灌胶
            var glue = new List<Temp>();
            if (guleid != 0)
            {
                glue = ManualList.Where(c => c.Id == guleid).Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name }).ToList();
            }
            else
            {
                glue = ManualList.Where(c => c.Section == "灌胶").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name }).ToList();
            }
            //气密
            var airtight = ManualList.Where(c => c.Section == "气密").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name }).ToList();

            //锁面罩
            var lockmas = ManualList.Where(c => c.Section == "锁面罩").Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber, Name = c.Name }).ToList();
            //var lockmas = GetNewSerinum(lockmastotal);


            //老化
            var burnin = ManualList.Where(c => c.Section == "老化").ToList();

            //包装
            var packing = new List<TempBlance>();
            if (packid != 0)
            {
                packing = Balance.Where(c => c.Id == packid).ToList();
            }
            else
            {
                var packingtotal = Balance.Where(c => c.Section == "包装").ToList();
                packing = GetNewSerinum(packingtotal);
            }
            #endregion

            #region 单个工段
            var icsmtpersonNum = 0;//smt总人数
            decimal icsmtWarkHours = 0m; //smt每小时产能
            decimal icsmtCapacityPerHour = 0m;//smt标准产能

            var linesmtpersonNum = 0;//smt总人数
            decimal linesmtWarkHours = 0m; //smt每小时产能
            decimal linesmtCapacityPerHour = 0m;//smt标准产能

            var pluginpersonNum = 0;//插件总人数
            decimal pluginWarkHours = 0m; //插件每小时产能
            decimal pluginCapacityPerHour = 0m;//插件标准产能

            var afterpersonNum = 0;//后焊总人数
            decimal afterWarkHours = 0m; //后焊每小时产能
            decimal afterCapacityPerHour = 0m;//后焊标准产能

            var threePersonNum = 0;//三防总人数
            var threeName = "";//三防名字
            decimal threeWarkHours = 0m; //三防每小时产能
            decimal threeCapacityPerHour = 0m;//三防标准产能

            var bottonPersonNum = 0;//打底壳总人数
            decimal bottonWarkHours = 0m; //打底壳每小时产能
            decimal bottonCapacityPerHour = 0m;//打底壳标准产能

            var magnetiPersonNum = 0;//装磁吸总人数
            var magnetiName = "";//装磁吸
            decimal magnetiWarkHours = 0m; //装磁吸每小时产能
            decimal magnetiCapacityPerHour = 0m;//装磁吸标准产能

            var InkjetPersonNum = 0;//喷墨总人数
            decimal InkjetWarkHours = 0m; //喷墨每小时产能
            decimal InkjetCapacityPerHour = 0m;//喷墨标准产能

            var gluePersonNum = 0;//灌胶总人数
            decimal glueWarkHours = 0m; //灌胶每小时产能
            decimal glueCapacityPerHour = 0m;//灌胶标准产能

            var airtightPersonNum = 0;//气密总人数
            decimal airtightWarkHours = 0m; //气密每小时产能
            decimal airtightCapacityPerHour = 0m;//气密标准产能

            var lockmasPersonNum = 0;//锁面罩总人数
            decimal lockmasWarkHours = 0m; //锁面罩每小时产能
            decimal lockmasCapacityPerHour = 0m;//锁面罩标准产能

            var moduleLinePersonNum = 0;//模块线总人数
            decimal moduleLineWarkHours = 0m; //模块线每小时产能
            decimal moduleLineCapacityPerHour = 0m;//模块线标准产能

            var mudulePersonNum = 0;//总人数
            decimal moduleWarkHours = 0m;//每小时产能
            decimal moduleCapacityPerHour = 0m;//标准产能

            var burnPersonNum = 0;//总人数
            decimal burnWarkHours = 0m;//每小时产能
            decimal burnCapacityPerHour = 0m;//标准产能

            var burnPersonNum2 = 0;//总人数
            decimal burnWarkHours2 = 0m;//每小时产能
            decimal burnCapacityPerHour2 = 0m;//标准产能

            var packPersonNum = 0;//总人数
            decimal packWarkHours = 0m;//每小时产能
            decimal packCapacityPerHour = 0m;//标准产能
            #endregion

            #region 汇总

            var totalpersonNum = 0;//总人数
            decimal totalWarkHours = 0m;

            var smttotalpersonNum = 0;//smt总人数
            decimal smttotalWarkHours = 0m; //smt每小时产能

            var mudulestotalpersonNum = 0;//总人数
            decimal mudulestotalWarkHours = 0m;//模块每小时产能

            #endregion

            #region  新的
            JArray temparrray = new JArray();

            JObject array = new JObject();
            array.Add("name", temparrray);
            array.Add("person", temparrray);
            array.Add("controllerNum", temparrray);
            array.Add("moduleNeed", temparrray);
            //array.Add("capacityPerHour", temparrray);
            array.Add("equipmentName", temparrray);
            array.Add("equipmentNum", temparrray);
            array.Add("equipmentCapacity", temparrray);
            array.Add("balanceMultiple", temparrray);
            array.Add("balanceCapacity", temparrray);
            array.Add("balancePerson", temparrray);
            array.Add("balanceEquipmentNum", temparrray);
            array.Add("balanceRate", temparrray);

            decimal reference;
            #endregion

            #region 循环各个工段的数据,拿到人数集合,和各个工段的单人模组需求总时(1/(标准产能/人数/模组需求))

            /*//smt
            decimal workHoure = Math.Round(icsmt.CapacityPerHour == 0 ? 0 : module.SMTModuleNeedNum / icsmt.CapacityPerHour, 2);
            #region 取得平衡倍数
            var icmultiple = 0;
            int linemultiple = 0;
            decimal multipleRate;
            do
            {
                if (icsmt.CapacityPerHour > linesmt.CapacityPerHour)
                {
                    icmultiple++;
                    var tempic = icsmt.CapacityPerHour * icmultiple;
                    linemultiple = (int)Math.Floor(tempic / linesmt.CapacityPerHour);
                    var temp2 = linesmt.CapacityPerHour * linemultiple;
                    if (temp2 > tempic)
                        multipleRate = tempic / temp2;
                    else
                        multipleRate = temp2 / tempic;
                    if (multipleRate < (decimal)0.85)
                    {
                        linemultiple = (int)Math.Ceiling(tempic / linesmt.CapacityPerHour);
                        var temp3 = linesmt.CapacityPerHour * linemultiple;
                        if (temp3 > tempic)
                            multipleRate = tempic / temp3;
                        else
                            multipleRate = temp3 / tempic;
                    }

                }
                else
                {
                    linemultiple++;
                    var templine = linesmt.CapacityPerHour * linemultiple;
                    icmultiple = (int)Math.Floor(templine / icsmt.CapacityPerHour);
                    var temp2 = icsmt.CapacityPerHour * icmultiple;
                    if (temp2 > templine)
                        multipleRate = templine / temp2;
                    else
                        multipleRate = temp2 / templine;
                    if (multipleRate < (decimal)0.85)
                    {
                        icmultiple = (int)Math.Ceiling(templine / icsmt.CapacityPerHour);
                        var temp3 = icsmt.CapacityPerHour * icmultiple;
                        if (temp3 > templine)
                            multipleRate = templine / temp3;
                        else
                            multipleRate = temp3 / templine;
                    }
                }
            } while (multipleRate < (decimal)0.85);

            #endregion
            var maxcap = icsmt.CapacityPerHour * icmultiple > linesmt.CapacityPerHour * linemultiple ? icsmt.CapacityPerHour * icmultiple : linesmt.CapacityPerHour * linemultiple;
            if (icsmt.CapacityPerHour != 0m)
            {
                DetailObj(icsmt.id, "IC", "SMT", icsmt.PersonNum, icsmt.CapacityPerHour, workHoure, module.SMTModuleNeedNum, icmultiple, Equipment, maxcap, array);
            }

            decimal workHoure2 = Math.Round(linesmt.CapacityPerHour == 0 ? 0 : module.SMTModuleNeedNum / linesmt.CapacityPerHour, 2);
            if (linesmt.CapacityPerHour != 0m)
            {
                DetailObj(linesmt.id, "灯面", "SMT", linesmt.PersonNum, linesmt.CapacityPerHour, workHoure2, module.SMTModuleNeedNum, linemultiple, Equipment, maxcap, array);
            }
            #region SMT段的平衡率


            #endregion
            //灌胶
            glue.ForEach(c =>
            {
                gluePersonNum = gluePersonNum + c.StandardNumber;
                glueCapacityPerHour = glueCapacityPerHour + c.StandardCapacity;
                glueWarkHours = glueWarkHours + (c.StandardCapacity == 0 ? 0 : module.GuleModuleNeedNum / c.StandardCapacity);
            });

            #region 拿到模块段的参考值
            if (Referencecapacity == 0M)
            {
                jobjectitem.Add("referenceName", "灌胶");//默认值的工段名字
                reference = Equipment.Where(c => c.Seaction == "灌胶").Select(c => c.Capacity).FirstOrDefault();
                if (reference == 0)
                    reference = glueCapacityPerHour;

            }
            else
            {
                reference = Referencecapacity;
            }
            #endregion

            //插件
            plugin.ForEach(c =>
            {
                pluginpersonNum = pluginpersonNum + c.StandardNumber;
                pluginCapacityPerHour = pluginCapacityPerHour + c.StandardCapacity;
                pluginWarkHours = pluginWarkHours + (c.StandardCapacity == 0 ? 0 : module.PluginModuleNeedNum / c.StandardCapacity);
                if (c.StandardCapacity != 0m)
                {
                    DetailObj(c.id, c.Name, "插件", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.PluginModuleNeedNum / c.StandardCapacity, module.PluginModuleNeedNum, 0, Equipment, 0, array, reference);
                }
            });
            //后焊
            after.ForEach(c =>
            {
                afterpersonNum = afterpersonNum + c.StandardTotal;
                afterCapacityPerHour = afterCapacityPerHour + c.StandardCapacity;
                afterWarkHours = afterWarkHours + (c.StandardCapacity == 0 ? 0 : module.AfterModuleNeedNum / c.StandardCapacity);
                if (c.StandardCapacity != 0m)
                {
                    DetailObj(c.Name, "后焊", c.StandardTotal, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.AfterModuleNeedNum / c.StandardCapacity, module.AfterModuleNeedNum, 0, Equipment, 0, array, reference);
                }
            });

            if (three.Count(c => c.Name.Contains("灌胶")) > 0)
            {
                // threeName = three.Where(c => c.Name.Contains("灌胶")).Select(c => c.Name).FirstOrDefault();
                if (magneti.Count(c => c.Name == "底壳装快锁") > 0)
                {
                    // 装磁吸
                    magneti.ForEach(c =>
                    {
                        magnetiPersonNum = magnetiPersonNum + c.StandardTotal;
                        magnetiCapacityPerHour = magnetiCapacityPerHour + c.StandardCapacity;
                        magnetiWarkHours = magnetiWarkHours + (c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "装磁吸安装板", c.StandardTotal, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity, module.MagneticModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                    //打底壳
                    botton.ForEach(c =>
                    {
                        bottonPersonNum = bottonPersonNum + c.StandardNumber;
                        bottonCapacityPerHour = bottonCapacityPerHour + c.StandardCapacity;
                        bottonWarkHours = bottonWarkHours + (c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "打底壳", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity, module.BottnModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                    //喷墨
                    Inkjet.ForEach(c =>
                    {
                        InkjetPersonNum = InkjetPersonNum + c.StandardNumber;
                        InkjetCapacityPerHour = InkjetCapacityPerHour + c.StandardCapacity;
                        InkjetWarkHours = InkjetWarkHours + (c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "喷墨", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity, module.InjekModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                    //三放
                    three.ForEach(c =>
                    {
                        threePersonNum = threePersonNum + c.StandardNumber;
                        threeCapacityPerHour = threeCapacityPerHour + c.StandardCapacity;
                        threeWarkHours = threeWarkHours + (c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "三防", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity, module.ThreeModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                }
                else
                {
                    //打底壳
                    botton.ForEach(c =>
                    {
                        bottonPersonNum = bottonPersonNum + c.StandardNumber;
                        bottonCapacityPerHour = bottonCapacityPerHour + c.StandardCapacity;
                        bottonWarkHours = bottonWarkHours + (c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "打底壳", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity, module.BottnModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                    // 装磁吸
                    magneti.ForEach(c =>
                    {
                        magnetiPersonNum = magnetiPersonNum + c.StandardTotal;
                        magnetiCapacityPerHour = magnetiCapacityPerHour + c.StandardCapacity;
                        magnetiWarkHours = magnetiWarkHours + (c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "装磁吸安装板", c.StandardTotal, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity, module.MagneticModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                    //喷墨
                    Inkjet.ForEach(c =>
                    {
                        InkjetPersonNum = InkjetPersonNum + c.StandardNumber;
                        InkjetCapacityPerHour = InkjetCapacityPerHour + c.StandardCapacity;
                        InkjetWarkHours = InkjetWarkHours + (c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "喷墨", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity, module.InjekModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                    //三放
                    three.ForEach(c =>
                    {
                        threePersonNum = threePersonNum + c.StandardNumber;
                        threeCapacityPerHour = threeCapacityPerHour + c.StandardCapacity;
                        threeWarkHours = threeWarkHours + (c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "三防", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity, module.ThreeModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                }
            }
            else
            {
                if (magneti.Count(c => c.Name == "底壳装快锁") > 0)
                {
                    //三放
                    three.ForEach(c =>
                    {
                        threePersonNum = threePersonNum + c.StandardNumber;
                        threeCapacityPerHour = threeCapacityPerHour + c.StandardCapacity;
                        threeWarkHours = threeWarkHours + (c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "三防", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity, module.ThreeModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                    // 装磁吸
                    magneti.ForEach(c =>
                    {
                        magnetiPersonNum = magnetiPersonNum + c.StandardTotal;
                        magnetiCapacityPerHour = magnetiCapacityPerHour + c.StandardCapacity;
                        magnetiWarkHours = magnetiWarkHours + (c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "装磁吸安装板", c.StandardTotal, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity, module.MagneticModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                    //打底壳
                    botton.ForEach(c =>
                    {
                        bottonPersonNum = bottonPersonNum + c.StandardNumber;
                        bottonCapacityPerHour = bottonCapacityPerHour + c.StandardCapacity;
                        bottonWarkHours = bottonWarkHours + (c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "打底壳", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity, module.BottnModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                    //喷墨
                    Inkjet.ForEach(c =>
                    {
                        InkjetPersonNum = InkjetPersonNum + c.StandardNumber;
                        InkjetCapacityPerHour = InkjetCapacityPerHour + c.StandardCapacity;
                        InkjetWarkHours = InkjetWarkHours + (c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "喷墨", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity, module.InjekModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                }
                else
                {
                    //三放
                    three.ForEach(c =>
                    {
                        threePersonNum = threePersonNum + c.StandardNumber;
                        threeCapacityPerHour = threeCapacityPerHour + c.StandardCapacity;
                        threeWarkHours = threeWarkHours + (c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "三防", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity, module.ThreeModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                    //打底壳
                    botton.ForEach(c =>
                    {
                        bottonPersonNum = bottonPersonNum + c.StandardNumber;
                        bottonCapacityPerHour = bottonCapacityPerHour + c.StandardCapacity;
                        bottonWarkHours = bottonWarkHours + (c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "打底壳", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity, module.BottnModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                    // 装磁吸
                    magneti.ForEach(c =>
                    {
                        magnetiPersonNum = magnetiPersonNum + c.StandardTotal;
                        magnetiCapacityPerHour = magnetiCapacityPerHour + c.StandardCapacity;
                        magnetiWarkHours = magnetiWarkHours + (c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "装磁吸安装板", c.StandardTotal, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity, module.MagneticModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                    //喷墨
                    Inkjet.ForEach(c =>
                    {
                        InkjetPersonNum = InkjetPersonNum + c.StandardNumber;
                        InkjetCapacityPerHour = InkjetCapacityPerHour + c.StandardCapacity;
                        InkjetWarkHours = InkjetWarkHours + (c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity);
                        if (c.StandardCapacity != 0m)
                        {
                            DetailObj(c.Name, "喷墨", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity, module.InjekModuleNeedNum, 0, Equipment, 0, array, reference);
                        }
                    });
                }
            }
            if (glueCapacityPerHour != 0m)
            {
                DetailObj("灌胶", "灌胶", gluePersonNum, glueCapacityPerHour, glueWarkHours, module.GuleModuleNeedNum, 0, Equipment, 0, array, reference);
            }
            //气密
            airtight.ForEach(c =>
            {
                airtightPersonNum = airtightPersonNum + c.StandardNumber;
                airtightCapacityPerHour = airtightCapacityPerHour + c.StandardCapacity;
                airtightWarkHours = airtightWarkHours + (c.StandardCapacity == 0 ? 0 : module.AirtightModuleNeedNum / c.StandardCapacity);
                if (c.StandardCapacity != 0m)
                {
                    DetailObj(c.Name, "气密", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.AirtightModuleNeedNum / c.StandardCapacity, module.AirtightModuleNeedNum, 0, Equipment, 0, array, reference);
                }
            });
            //锁面罩
            lockmas.ForEach(c =>
            {
                lockmasPersonNum = lockmasPersonNum + c.StandardNumber;
                lockmasCapacityPerHour = lockmasCapacityPerHour + c.StandardCapacity;
                lockmasWarkHours = lockmasWarkHours + (c.StandardCapacity == 0 ? 0 : module.LockMasModuleNeedNum / c.StandardCapacity);
                if (c.StandardCapacity != 0m)
                {
                    DetailObj(c.Name, "锁面罩", c.StandardNumber, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.LockMasModuleNeedNum / c.StandardCapacity, module.LockMasModuleNeedNum, 0, Equipment, 0, array, reference);
                }
            });


            //模块线
            moduleLine.ForEach(c =>
            {
                moduleLinePersonNum = moduleLinePersonNum + c.StandardTotal;
                moduleLineCapacityPerHour = moduleLineCapacityPerHour + c.StandardCapacity;
                moduleLineWarkHours = moduleLineWarkHours + (c.StandardCapacity == 0 ? 0 : module.ModuleLineNeedNum / c.StandardCapacity);
                if (c.StandardCapacity != 0m)
                {
                    DetailObj(c.Name, "模块线", c.StandardTotal, c.StandardCapacity, c.StandardCapacity == 0 ? 0 : module.ModuleLineNeedNum / c.StandardCapacity, module.ModuleLineNeedNum, 0, Equipment, 0, array, reference);
                }
            });
            //模组装配
            mudulePersonNum = mudulePersonNum + module.StandardTotal;
            moduleCapacityPerHour = moduleCapacityPerHour + module.StandardOutput;
            moduleWarkHours = moduleWarkHours + (module.StandardOutput == 0 ? 0 : module.ModuleNeedNum2 / module.StandardOutput);
            if (moduleCapacityPerHour != 0m)
            {
                DetailObj("模组装配", "模组装配", mudulePersonNum, moduleCapacityPerHour, moduleWarkHours, module.ModuleNeedNum2, 1, Equipment, moduleCapacityPerHour, array);
            }
            //老化
            burnin.ForEach(c =>
            {
                burnPersonNum = burnPersonNum + c.StandardNumber;
                burnCapacityPerHour = burnCapacityPerHour + c.StandardCapacity;
                burnWarkHours = burnWarkHours + (c.StandardCapacity == 0 ? 0 : module.BuriInModuleNeedNum / c.StandardCapacity);
                burnPersonNum2 = burnPersonNum2 + c.StandardNumber2;
                burnCapacityPerHour2 = burnCapacityPerHour2 + c.StandardCapacity2;
                burnWarkHours2 = burnWarkHours2 + (c.StandardCapacity2 == 0 ? 0 : module.BuriInModuleNeedNum / c.StandardCapacity2);
            });
            if (burnCapacityPerHour != 0m)
            {
                DetailObj("拼屏", "拼屏", burnPersonNum, burnCapacityPerHour, burnWarkHours, module.BuriInModuleNeedNum, 1, Equipment, burnCapacityPerHour, array);
            }
            if (burnCapacityPerHour2 != 0m)
            {
                DetailObj("拆屏", "拆屏", burnPersonNum2, burnCapacityPerHour2, burnWarkHours2, module.BuriInModuleNeedNum, 1, Equipment, burnCapacityPerHour2, array);
            }
            //包装
            packing.ForEach(c =>
            {
                packPersonNum = packPersonNum + c.StandardTotal;
                packCapacityPerHour = packCapacityPerHour + c.StandardCapacity;
                packWarkHours = packWarkHours + (c.StandardCapacity == 0 ? 0 : module.PackModuleNeedNum / c.StandardCapacity);
            });
            if (packCapacityPerHour != 0m)
            {
                DetailObj("包装", "包装", packPersonNum, packCapacityPerHour, packWarkHours, module.PackModuleNeedNum, 1, Equipment, packCapacityPerHour, array);
            }
            #region 之前的
            //smt
            //icsmtpersonNum = icsmtpersonNum + icsmt.PersonNum;
            //icsmtCapacityPerHour = icsmtCapacityPerHour + icsmt.CapacityPerHour;

            //icsmtWarkHours = icsmtWarkHours + (icsmt.CapacityPerHour == 0 ? 0 : module.SMTModuleNeedNum / icsmt.CapacityPerHour);

            //linesmtpersonNum = linesmtpersonNum + linesmt.PersonNum;
            //linesmtCapacityPerHour = linesmtCapacityPerHour + linesmt.CapacityPerHour;
            //linesmtWarkHours = linesmtWarkHours + (linesmt.CapacityPerHour == 0 ? 0 : module.SMTModuleNeedNum / linesmt.CapacityPerHour);


            ////插件
            //plugin.ForEach(c =>
            //{
            //    pluginpersonNum = pluginpersonNum + c.StandardNumber;
            //    pluginCapacityPerHour = pluginCapacityPerHour + c.StandardCapacity;
            //    // pluginWarkHours = pluginWarkHours + (c.StandardNumber == 0 || module.PluginModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardNumber / module.PluginModuleNeedNum))); });
            //    pluginWarkHours = pluginWarkHours + (c.StandardCapacity == 0 ? 0 : module.PluginModuleNeedNum / c.StandardCapacity);
            //});

            ////后焊
            //after.ForEach(c =>
            //{
            //    afterpersonNum = afterpersonNum + c.StandardTotal;
            //    afterCapacityPerHour = afterCapacityPerHour + c.StandardCapacity;
            //    //afterWarkHours = afterWarkHours + (c.StandardTotal == 0 || module.AfterModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardTotal / module.AfterModuleNeedNum)));
            //    afterWarkHours = afterWarkHours + (c.StandardCapacity == 0 ? 0 : module.AfterModuleNeedNum / c.StandardCapacity);

            //});
            ////三放
            //if (three.Count(c => c.Name.Contains("灌胶")) > 0)
            //{
            //    threeName = three.Where(c => c.Name.Contains("灌胶")).Select(c => c.Name).FirstOrDefault();
            //}
            //three.ForEach(c =>
            //{
            //    threePersonNum = threePersonNum + c.StandardNumber;
            //    threeCapacityPerHour = threeCapacityPerHour + c.StandardCapacity;
            //    //threeWarkHours = threeWarkHours + (c.StandardNumber == 0 || module.ThreeModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardNumber / module.ThreeModuleNeedNum)));
            //    threeWarkHours = threeWarkHours + (c.StandardCapacity == 0 ? 0 : module.ThreeModuleNeedNum / c.StandardCapacity);
            //});
            ////打底壳
            //botton.ForEach(c =>
            //{
            //    bottonPersonNum = bottonPersonNum + c.StandardNumber;
            //    bottonCapacityPerHour = bottonCapacityPerHour + c.StandardCapacity;
            //    //bottonWarkHours = bottonWarkHours + (c.StandardNumber == 0 || module.BottnModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardNumber / module.BottnModuleNeedNum)));
            //    bottonWarkHours = bottonWarkHours + (c.StandardCapacity == 0 ? 0 : module.BottnModuleNeedNum / c.StandardCapacity);
            //});
            ////装磁吸
            //if (magneti.Count(c => c.Name == "底壳装快锁") > 0)
            //{
            //    magnetiName = "底壳装快锁";
            //}
            //magneti.ForEach(c =>
            //{
            //    magnetiPersonNum = magnetiPersonNum + c.StandardTotal;
            //    magnetiCapacityPerHour = magnetiCapacityPerHour + c.StandardCapacity;
            //    //magnetiWarkHours = magnetiWarkHours + (c.StandardTotal == 0 || module.MagneticModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardTotal / module.MagneticModuleNeedNum))); });
            //    magnetiWarkHours = magnetiWarkHours + (c.StandardCapacity == 0 ? 0 : module.MagneticModuleNeedNum / c.StandardCapacity);
            //});
            ////喷墨
            //Inkjet.ForEach(c =>
            //{
            //    InkjetPersonNum = InkjetPersonNum + c.StandardNumber;
            //    InkjetCapacityPerHour = InkjetCapacityPerHour + c.StandardCapacity;
            //    //InkjetWarkHours = InkjetWarkHours + (c.StandardNumber == 0 || module.InjekModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardNumber / module.InjekModuleNeedNum)));
            //    InkjetWarkHours = InkjetWarkHours + (c.StandardCapacity == 0 ? 0 : module.InjekModuleNeedNum / c.StandardCapacity);
            //});
            ////灌胶
            //glue.ForEach(c =>
            //{
            //    gluePersonNum = gluePersonNum + c.StandardNumber;
            //    glueCapacityPerHour = glueCapacityPerHour + c.StandardCapacity;
            //    //glueWarkHours = glueWarkHours + (c.StandardNumber == 0 || module.GuleModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardNumber / module.GuleModuleNeedNum)));
            //    glueWarkHours = glueWarkHours + (c.StandardCapacity == 0 ? 0 : module.GuleModuleNeedNum / c.StandardCapacity);
            //});
            ////气密
            //airtight.ForEach(c =>
            //{
            //    airtightPersonNum = airtightPersonNum + c.StandardNumber;
            //    airtightCapacityPerHour = airtightCapacityPerHour + c.StandardCapacity;
            //    //airtightWarkHours = airtightWarkHours + (c.StandardNumber == 0 || module.AirtightModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardNumber / module.AirtightModuleNeedNum)));
            //    airtightWarkHours = airtightWarkHours + (c.StandardCapacity == 0 ? 0 : module.AirtightModuleNeedNum / c.StandardCapacity);
            //});
            ////锁面罩
            //lockmas.ForEach(c =>
            //{
            //    lockmasPersonNum = lockmasPersonNum + c.StandardNumber;
            //    lockmasCapacityPerHour = lockmasCapacityPerHour + c.StandardCapacity;
            //    //lockmasWarkHours = lockmasWarkHours + (c.StandardNumber == 0 || module.LockMasModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardNumber / module.LockMasModuleNeedNum)));
            //    lockmasWarkHours = lockmasWarkHours + (c.StandardCapacity == 0 ? 0 : module.LockMasModuleNeedNum / c.StandardCapacity);
            //});
            ////模块线
            //moduleLine.ForEach(c =>
            //{
            //    moduleLinePersonNum = moduleLinePersonNum + c.StandardTotal;
            //    moduleLineCapacityPerHour = moduleLineCapacityPerHour + c.StandardCapacity;
            //    // moduleLineWarkHours = moduleLineWarkHours + (c.StandardTotal == 0 || module.LockMasModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardTotal / module.LockMasModuleNeedNum)));
            //    moduleLineWarkHours = moduleLineWarkHours + (c.StandardCapacity == 0 ? 0 : module.LockMasModuleNeedNum / c.StandardCapacity);
            //});
            ////模组装配
            //mudulePersonNum = mudulePersonNum + module.StandardTotal;
            //moduleCapacityPerHour = moduleCapacityPerHour + module.StandardOutput;
            ////moduleWarkHours = moduleWarkHours + (module.StandardTotal == 0 || module.ModuleNeedNum2 == 0 ? 0 : (1 / (module.StandardOutput / module.StandardTotal / module.ModuleNeedNum2)));
            //moduleWarkHours = moduleWarkHours + (module.StandardOutput == 0 ? 0 : module.ModuleNeedNum2 / module.StandardOutput);
            ////老化
            //burnin.ForEach(c =>
            //{
            //    burnPersonNum = burnPersonNum + c.StandardNumber;
            //    burnCapacityPerHour = burnCapacityPerHour + c.StandardCapacity;
            //    //burnWarkHours = burnWarkHours + (c.StandardNumber == 0 || module.BuriInModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardNumber / module.BuriInModuleNeedNum)));
            //    burnWarkHours = burnWarkHours + (c.StandardCapacity == 0 ? 0 : module.BuriInModuleNeedNum / c.StandardCapacity);
            //    burnPersonNum2 = burnPersonNum2 + c.StandardNumber2;
            //    burnCapacityPerHour2 = burnCapacityPerHour2 + c.StandardCapacity2;
            //    //burnWarkHours2 = burnWarkHours2 + (c.StandardNumber2 == 0 || module.BuriInModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity2 / c.StandardNumber2 / module.BuriInModuleNeedNum)));
            //    burnWarkHours2 = burnWarkHours2 + (c.StandardCapacity2 == 0 ? 0 : module.BuriInModuleNeedNum / c.StandardCapacity2);
            //});
            //// burnCapacityPerHour = tempburncap == 0 ? 0 : Math.Round(3600 / tempburncap * burnPersonNum, 2);
            ////包装
            //packing.ForEach(c =>
            //{
            //    packPersonNum = packPersonNum + c.StandardTotal;
            //    packCapacityPerHour = packCapacityPerHour + c.StandardCapacity;
            //    // packWarkHours = packWarkHours + (c.StandardTotal == 0 || module.PackModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardTotal / module.PackModuleNeedNum))); });
            //    packWarkHours = packWarkHours + (c.StandardCapacity == 0 ? 0 : module.PackModuleNeedNum / c.StandardCapacity);
            //});
            #endregion
            */
            #endregion

            #region 详细
            ////ic
            //jobjectitem.Add("icPerson", icsmtpersonNum);//人数
            //jobjectitem.Add("icCapacity", icsmtCapacityPerHour);//标准产能
            //jobjectitem.Add("icCapacityPerHour", Math.Round(icsmtWarkHours, 2) /*== 0 ? 0 : Math.Round(smtpersonNum / smtWarkHours, 2)*/);//每小时产能
            ////灯面
            //jobjectitem.Add("linePerson", linesmtpersonNum);//人数
            //jobjectitem.Add("lineCapacity", linesmtCapacityPerHour);//标准产能
            //jobjectitem.Add("lineCapacityPerHour", Math.Round(linesmtWarkHours, 2) /*== 0 ? 0 : Math.Round(smtpersonNum / smtWarkHours, 2)*/);//每小时产能
            ////插件
            //jobjectitem.Add("pluginPerson", pluginpersonNum);//人数
            //jobjectitem.Add("pluginCapacity", pluginCapacityPerHour);//标准产能
            //jobjectitem.Add("pluginCapacityPerHour", Math.Round(pluginWarkHours, 2) /*== 0 ? 0 : Math.Round(pluginpersonNum / pluginWarkHours, 2)*/);//每小时产能
            ////后焊
            //jobjectitem.Add("afterPerson", afterpersonNum);//人数
            //jobjectitem.Add("afterCapacity", afterCapacityPerHour);//标准产能
            //jobjectitem.Add("afterCapacityPerHour", Math.Round(afterWarkHours, 2) /*== 0 ? 0 : Math.Round(afterpersonNum / afterWarkHours, 2)*/);//每小时产能
            ////三防
            //jobjectitem.Add("threePersonName", threeName);//三防名字
            //jobjectitem.Add("threePerson", threePersonNum);//人数
            //jobjectitem.Add("threeCapacity", threeCapacityPerHour);//标准产能
            //jobjectitem.Add("threeCapacityPerHour", Math.Round(threeWarkHours, 2) /*== 0 ? 0 : Math.Round(threePersonNum / threeWarkHours, 2)*/);//每小时产能
            ////打底壳
            //jobjectitem.Add("bottonPerson", bottonPersonNum);//人数
            //jobjectitem.Add("bottonCapacity", bottonCapacityPerHour);//标准产能
            //jobjectitem.Add("bottonCapacityPerHour", Math.Round(bottonWarkHours, 2)/* == 0 ? 0 : Math.Round(bottonPersonNum / bottonWarkHours, 2)*/);//每小时产能
            ////装磁吸
            //jobjectitem.Add("magnetiPerson", magnetiPersonNum);//人数
            //jobjectitem.Add("magnetiPersonName", magnetiName);//名字
            //jobjectitem.Add("magnetiCapacity", magnetiCapacityPerHour);//标准产能
            //jobjectitem.Add("magnetiCapacityPerHour", Math.Round(magnetiWarkHours, 2) /*== 0 ? 0 : Math.Round(magnetiPersonNum / magnetiWarkHours, 2)*/);//每小时产能
            ////喷墨
            //jobjectitem.Add("InkjetPerson", InkjetPersonNum);//人数
            //jobjectitem.Add("InkjetCapacity", InkjetCapacityPerHour);//标准产能
            //jobjectitem.Add("InkjetCapacityPerHour", Math.Round(InkjetWarkHours, 2) /*== 0 ? 0 : Math.Round(InkjetPersonNum / InkjetWarkHours, 2)*/);//每小时产能
            ////灌胶
            //jobjectitem.Add("gluePerson", gluePersonNum);//人数
            //jobjectitem.Add("glueCapacity", glueCapacityPerHour);//标准产能
            //jobjectitem.Add("glueCapacityPerHour", Math.Round(glueWarkHours, 2)/* == 0 ? 0 : Math.Round(gluePersonNum / glueWarkHours, 2)*/);//每小时产能
            ////气密
            //jobjectitem.Add("airtightPerson", airtightPersonNum);//人数
            //jobjectitem.Add("airtightCapacity", airtightCapacityPerHour);//标准产能
            //jobjectitem.Add("airtightCapacityPerHour", Math.Round(airtightWarkHours, 2) /*== 0 ? 0 : Math.Round(airtightPersonNum / airtightWarkHours, 2)*/);//每小时产能
            ////锁面罩
            //jobjectitem.Add("lockmasPerson", lockmasPersonNum);//人数
            //jobjectitem.Add("lockmasCapacity", lockmasCapacityPerHour);//标准产能
            //jobjectitem.Add("lockmasCapacityPerHour", Math.Round(lockmasWarkHours, 2) /*== 0 ? 0 : Math.Round(lockmasPersonNum / lockmasWarkHours, 2)*/);//每小时产能
            ////模块线
            //jobjectitem.Add("moduleLinePerson", moduleLinePersonNum);//人数
            //jobjectitem.Add("moduleLineCapacity", moduleLineCapacityPerHour);//标准产能
            //jobjectitem.Add("moduleLineCapacityPerHour", Math.Round(moduleLineWarkHours, 2)/* == 0 ? 0 : Math.Round(moduleLinePersonNum / moduleLineWarkHours, 2)*/);//每小时产能
            ////模组装配
            //jobjectitem.Add("mudulePerson", mudulePersonNum);//人数
            //jobjectitem.Add("muduleCapacity", moduleCapacityPerHour);//标准产能
            //jobjectitem.Add("moduleCapacityPerHour", Math.Round(moduleWarkHours, 2) /*== 0 ? 0 : Math.Round(mudulePersonNum / moduleWarkHours, 2)*/);//模组每小时产能
            ////老化1
            //jobjectitem.Add("burnPerson", burnPersonNum);//人数
            //jobjectitem.Add("burnCapacity", burnCapacityPerHour);//标准产能
            //jobjectitem.Add("burnCapacityPerHour", Math.Round(burnWarkHours, 2)/* == 0 ? 0 : Math.Round(burnPersonNum / burnWarkHours, 2)*/);//老化每小时产能
            //                                                                                                                                 //老化2
            //jobjectitem.Add("burnPerson2", burnPersonNum2);//人数
            //jobjectitem.Add("burnCapacity2", burnCapacityPerHour2);//标准产能
            //jobjectitem.Add("burnCapacityPerHour2", Math.Round(burnWarkHours2, 2)/* == 0 ? 0 : Math.Round(burnPersonNum / burnWarkHours, 2)*/);//老化每小时产能
            ////包装
            //jobjectitem.Add("packPerson", packPersonNum);//人数
            //jobjectitem.Add("packCapacity", packCapacityPerHour);//标准产能
            //jobjectitem.Add("packCapacityPerHour", Math.Round(packWarkHours, 2) /*== 0 ? 0 : Math.Round(packPersonNum / packWarkHours, 2)*/);//包装每小时产能
            #endregion

            #region 汇总
            // smttotalWarkHours = workHoure + workHoure2 + pluginWarkHours + afterWarkHours;
            smttotalpersonNum = icsmtpersonNum + linesmtpersonNum + pluginpersonNum + afterpersonNum;
            // jobjectitem.Add("smtCapacityPerHour", smttotalWarkHours == 0 ? 0 : Math.Round(smttotalpersonNum / smttotalWarkHours, 2));//smt每小时产能

            mudulestotalWarkHours = threeWarkHours + bottonWarkHours + magnetiWarkHours + InkjetWarkHours + glueWarkHours + airtightWarkHours + lockmasWarkHours + moduleLineWarkHours;
            mudulestotalpersonNum = threePersonNum + bottonPersonNum + magnetiPersonNum + InkjetPersonNum + gluePersonNum + airtightPersonNum + lockmasPersonNum + moduleLinePersonNum;
            //jobjectitem.Add("modulesCapacityPerHour", mudulestotalWarkHours == 0 ? 0 : Math.Round(mudulestotalpersonNum / mudulestotalWarkHours, 2));//模块每小时产能


            totalWarkHours = smttotalWarkHours + mudulestotalWarkHours + moduleWarkHours + burnWarkHours + burnWarkHours2 + packWarkHours;
            totalpersonNum = smttotalpersonNum + mudulestotalpersonNum + mudulePersonNum + burnPersonNum + burnPersonNum2 + packPersonNum;
            /* JArray name = new JArray();
            JArray person = new JArray();
            JArray capacity = new JArray();
            JArray moduleNeed = new JArray();
            JArray capacityPerHour = new JArray();
            JArray equipmentName = new JArray();
            JArray equipmentNum = new JArray();
            JArray equipmentCapacity = new JArray();
            */

            jobjectitem.Add("name", array["name"]); //详细表
            jobjectitem.Add("person", array["person"]);//标配人数
            jobjectitem.Add("controllerNum", array["controllerNum"]);//一个人能控制的台数
            jobjectitem.Add("moduleNeed", array["moduleNeed"]);//模组需求数
            //jobjectitem.Add("capacityPerHour", array["capacityPerHour"]);//每小时产能
            jobjectitem.Add("equipmentName", array["equipmentName"]);//设备名字
            jobjectitem.Add("equipmentNum", array["equipmentNum"]);//设备数量
            jobjectitem.Add("equipmentCapacity", array["equipmentCapacity"]);//设备产能
            jobjectitem.Add("balanceMultiple", array["balanceMultiple"]);//平衡后倍数
            jobjectitem.Add("balanceCapacity", array["balanceCapacity"]);//平衡后产能
            jobjectitem.Add("balancePerson", array["balancePerson"]);//平衡后人数
            jobjectitem.Add("balanceEquipmentNum", array["balanceEquipmentNum"]);//平衡后设备需求数
            jobjectitem.Add("balanceRate", array["balanceRate"]);//平衡率

            jobjectitem.Add("totalperson", totalpersonNum);//所需人数
            jobjectitem.Add("processingFee", "");//加工费用
            jobjectitem.Add("totalcapacityPerHour", Math.Round(totalWarkHours, 2));//每小时产能
            #endregion
            return jobjectitem;
        }
        public ActionResult GetInfoByModule1(string type, string Platform)
        {
            JArray totalresult = new JArray();
            var total = db.Process_Capacity_Total.ToList();//找到所有的平台,型号,PCB板 
            if (!string.IsNullOrEmpty(type))
            {
                total = total.Where(c => c.Type == type).ToList();
            }
            if (!string.IsNullOrEmpty(Platform))
            {
                total = total.Where(c => c.Platform == Platform).ToList();
            }
            var distintotal = total.Where((x, y) => total.FindIndex(z => z.Type == x.Type && z.Platform == x.Platform) == y);
            foreach (var item in distintotal)
            {
                JObject result = new JObject();

                JArray platformModule = new JArray();
                result.Add("platfrom", item.Platform);//平台
                result.Add("type", item.Type);//型号
                result.Add("PCB", item.ProductPCBnumber);//pcb板
                var platformmodulelist = total.Where(c => c.Type == item.Type && c.Platform == item.Platform).Select(c => c.PlatformModul).Distinct().ToList();

                foreach (var modulelist in platformmodulelist)
                {
                    //List<Relevance> threeRelevancetotal = new List<Relevance>();

                    JObject modulelistjobject = new JObject();
                    modulelistjobject.Add("PlatformModul", modulelist);//平台模块
                    modulelistjobject.Add("ModuleUnits", 1);//模组单位
                    var main = total.Where(c => c.Type == item.Type && c.Platform == item.Platform && c.PlatformModul == modulelist).Select(c => c.Maintenance).ToList();
                    modulelistjobject.Add("Maintenance", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(main)));
                    platformModule.Add(modulelistjobject);
                }
                result.Add("platformModule", platformModule);
                totalresult.Add(result);
            }

            return Content(JsonConvert.SerializeObject(totalresult));
        }


        //计算各个工序的时间
        //public ActionResult Num(string PlatformModule,string size,int area,string value)
        //{
        //    JObject result = new JObject();
        //    JArray itemarray = new JArray();
        //    decimal totalTime = 0m;
        //    var stringarry = PlatformModule.Split('*');//拆分平台模块
        //    var smallModuleArea = decimal.Parse(stringarry[0]) * decimal.Parse(stringarry[1]);//求得模块面积

        //    var stringarry2 = size.Split('*');//拆分尺寸
        //    var ModuleArea = decimal.Parse(stringarry2[0]) * decimal.Parse(stringarry2[1]);//求得模组面积

        //    var smallModuleNum = area / smallModuleArea;//求得模块数量
        //    var ModuleNum = area / ModuleArea;//求得模块数量

        //    JArray array = JsonConvert.DeserializeObject<JArray>(value);//拿到每个工序数据信息

        //    foreach (var item in array)
        //    {
        //        JObject info = new JObject();
        //        var time = 0m;
        //        info.Add("name", item["name"].ToString());
        //        //模组计算工时
        //        if (item["name"].ToString() == "模组装配" || item["name"].ToString() == "拼屏" || item["name"].ToString() == "拆屏" || item["name"].ToString() == "包装")
        //        {
        //             time = ModuleNum / decimal.Parse(item["capacity"].ToString());
        //            info.Add("Time", Math.Round(time, 2));
        //        }
        //        //模块计算工时
        //        else
        //        {
        //             time = smallModuleNum / decimal.Parse(item["capacity"].ToString());
        //            info.Add("Time", Math.Round(time, 2));
        //        }
        //        totalTime = totalTime + time;
        //        itemarray.Add(info);
        //    }
        //    result.Add("item", itemarray);
        //    result.Add("totalTime", totalTime);
        //    return Content(JsonConvert.SerializeObject(result));
        //}
        #endregion


        #region 添加，修改，删除

        //添加总表
        public ActionResult AddTotalProcess_Capacity(string type, string pcbnumber, string platform, string plafromodule, string Maintenance)
        {
            if (Session["User"] == null)//判断是否有登录,没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
            }
            JObject value = new JObject();
            var count = db.Process_Capacity_Total.Count(c => c.Type == type && c.ProductPCBnumber == pcbnumber && c.Maintenance == Maintenance && c.Platform == platform && c.PlatformModul == plafromodule);//根据前端传过来的平台,类型,PCB板查找总表中符合条件的数量
            if (count != 0)//如果查找的数量不为0,说明已有重复的数据,提示错误,返回前端
            {
                value.Add("message", "已有相同的型号平台和平台模块和维护方式");
                value.Add("content", null);
                return Content(JsonConvert.SerializeObject(value));
            }
            Process_Capacity_Total total = new Process_Capacity_Total() { Type = type, ProductPCBnumber = pcbnumber, Platform = platform, PlatformModul = plafromodule, Maintenance = Maintenance };//创建新的工序总表对象,并把前端传过来的平台,类型,PCB板赋值进去
            total.Operator = ((Users)Session["User"]).UserName;//登录姓名
            total.OperateDT = DateTime.Now;//创建时间
            db.Process_Capacity_Total.Add(total);//往数据库创建数据
            db.SaveChanges();//数据库保存

            value.Add("message", "添加成功");

            #region 返回新建总表jobject
            JObject result = new JObject();
            var info = db.Process_Capacity_Total.Where(c => c.Type == type && c.ProductPCBnumber == pcbnumber && c.Platform == platform && c.Maintenance == Maintenance).FirstOrDefault();//找到刚刚新增的数据
            result.Add("id", info.Id);
            result.Add("Type", type);
            result.Add("Platform", platform);
            result.Add("PCB", pcbnumber);
            result.Add("PlatformModul", plafromodule);
            result.Add("Maintenance", Maintenance);
            //ic面贴装 //灯面贴装
            result.Add("icProductName", null);
            result.Add("icMaxStandardTotal", null);
            result.Add("icMaxStandardOutput", null);
            result.Add("icMinStandardTotal", null);
            result.Add("icMinStandardOutput", null);
            result.Add("LightProductName", null);
            result.Add("LightMaxStandardTotal", null);
            result.Add("LightMaxStandardOutput", null);
            result.Add("LightMinStandardTotal", null);
            result.Add("LightMinStandardOutput", null);
            result.Add("SMTjpg", false);
            result.Add("SMTpdf", false);
            result.Add("TotalEdit", false);
            result.Add("editNum", 0);
            //PCB烘烤
            result.Add("PCBDrying", 0);
            //外发工艺
            result.Add("OutboundProcess", 0);
            //干燥时间
            result.Add("DryTime", 0);

            //插件
            result.Add("PluginDevice", null);

            //后焊
            result.Add("AfterWeld", null);

            //三防
            result.Add("ThreeProf", null);

            //打底壳
            result.Add("BottomCas", null);

            //装磁吸
            result.Add("Magnetic", null);

            //喷墨
            result.Add("Inkjet", null);

            //模块线
            result.Add("ModuleLine", null);

            //灌胶
            result.Add("Glue", null);

            //气密
            result.Add("Airtight", null);

            //锁面罩
            result.Add("LockTheMask", null);

            //模组装配
            result.Add("Module", null);

            //老化
            result.Add("Burin", null);

            //包装
            result.Add("Packing", null);
            #endregion
            value.Add("content", result);
            return Content(JsonConvert.SerializeObject(value));
        }

        //总表删除
        public ActionResult DeleteTotalProcess_Capacity(int id)
        {
            if (Session["User"] == null)//判断是否有登录,如果没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
            }
            var process = db.Process_Capacity_Total.Find(id);//根据id找到总表对应的数据,得到平台,型号,PCB板数据
            var type = process.Type;
            var platform = process.Platform;
            var PCBnumber = process.ProductPCBnumber;
            var plafromodule = process.PlatformModul;
            var Maintenance = process.Maintenance;

            //删除平衡卡
            var deleteblance = db.ProcessBalance.Where(c => c.Type == type && c.Platform == platform && c.ProductPCBnumber == PCBnumber && c.Maintenance == Maintenance && c.PlatformModul == plafromodule).ToList();
            db.ProcessBalance.RemoveRange(deleteblance);
            //删除贴片
            var pick = db.Pick_And_Place.Where(c => c.Type == type && c.Platform == platform && c.ProductPCBnumber == PCBnumber && c.Maintenance == Maintenance && c.PlatformModul == plafromodule).ToList();
            db.Pick_And_Place.RemoveRange(pick);
            //删除插件
            var plug = db.Process_Capacity_Plugin.Where(c => c.Type == type && c.Platform == platform && c.Maintenance == Maintenance && c.ProductPCBnumber == PCBnumber && c.PlatformModul == plafromodule).ToList();
            db.Process_Capacity_Plugin.RemoveRange(plug);
            ////删除三防
            //var three = db.Process_Capacity_ThreeProf.Where(c => c.Type == type && c.Platform == platform && c.ProductPCBnumber == PCBnumber).ToList();
            //db.Process_Capacity_ThreeProf.RemoveRange(three);
            ////删除喷墨
            //var inkjets = db.Process_Capacity_Inkjet.Where(c => c.Type == type && c.Platform == platform && c.Maintenance == Maintenance && c.ProductPCBnumber == PCBnumber && c.PlatformModul == plafromodule).ToList();
            //db.Process_Capacity_Inkjet.RemoveRange(inkjets);
            ////删除灌胶
            //var glue = db.Process_Capacity_Glue.Where(c => c.Type == type && c.Platform == platform && c.Maintenance == Maintenance && c.ProductPCBnumber == PCBnumber && c.PlatformModul == plafromodule).ToList();
            //db.Process_Capacity_Glue.RemoveRange(glue);
            ////删除气密
            //var airtights = db.Process_Capacity_Airtight.Where(c => c.Type == type && c.Platform == platform && c.Maintenance == Maintenance && c.ProductPCBnumber == PCBnumber && c.PlatformModul == plafromodule).ToList();
            //db.Process_Capacity_Airtight.RemoveRange(airtights);
            ////删除老化
            //var burn = db.Process_Capacity_Burin.Where(c => c.Type == type && c.Platform == platform && c.Maintenance == Maintenance && c.ProductPCBnumber == PCBnumber && c.PlatformModul == plafromodule).ToList();
            //db.Process_Capacity_Burin.RemoveRange(burn);

            //删除手术表
            var Manual = db.Process_Capacity_Manual.Where(c => c.Type == type && c.Maintenance == Maintenance && c.Platform == platform && c.PlatformModul == plafromodule && c.ProductPCBnumber == PCBnumber).ToList();
            db.Process_Capacity_Manual.RemoveRange(Manual);
            //删除关联
            var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == type && c.Maintenance == Maintenance && c.Platform == platform && c.PlatformModul == plafromodule).ToList();
            db.Process_Capacity_Relevance.RemoveRange(relevance);
            //删除设备表
            var equmment = db.Process_Capacity_Equipment.Where(c => c.Type == type && c.Maintenance == Maintenance && c.Platform == platform && c.PlatformModul == plafromodule).ToList();
            db.Process_Capacity_Equipment.RemoveRange(equmment);
            //删除总表
            db.Process_Capacity_Total.Remove(process);

            //填写日志
            UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "删除工序产能平台为" + platform + "型号为" + type + "PCB板号为" + PCBnumber + "平台模块" + plafromodule + "维护方式" + Maintenance };
            db.UserOperateLog.Add(log);
            db.SaveChanges();//保存数据
            JObject value = new JObject();
            value.Add("message", true);
            value.Add("content", "删除成功");
            return Content(JsonConvert.SerializeObject(value));
        }

        //总表修改
        public ActionResult UpdateTotalProcess_Capacity(int id, string type, string pcb, string platfrom, string plafromodule, decimal pcbdry, decimal outboundProcess, decimal drytime, string Maintenance)
        {
            if (Session["User"] == null)//判断是否有登录,如果没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
            }
            //修改总表
            var totals = db.Process_Capacity_Total.Where(c => c.Id == id).FirstOrDefault();//根据id找到信息,得到平台,类型,PCB板内容
            var oldtype = totals.Type;
            var oldplatform = totals.Platform;
            var oldPCBnumber = totals.ProductPCBnumber;
            var oldplatformModul = totals.PlatformModul;
            var oldpcbdry = totals.PCBDrying;
            var oldoutboundProcess = totals.OutboundProcess;
            var olddrytime = totals.DryTime;
            var oldMain = totals.Maintenance;

            totals.Type = type;//修改后的类型
            totals.Platform = platfrom;//修改后的平台
            totals.ProductPCBnumber = pcb;//修改后的PCB板
            totals.PlatformModul = plafromodule;//修改后的平台模块
            totals.PCBDrying = pcbdry;//pcb烘干时间
            totals.OutboundProcess = outboundProcess;//外发工艺
            totals.DryTime = drytime;//干燥时间
            totals.Maintenance = Maintenance;//维护方式


            //修改平衡卡 根据旧的平台,类型,PCB板内容,找到对应内容,再将新的赋值过去
            var deleteblance = db.ProcessBalance.Where(c => c.Type == oldtype && c.Platform == oldplatform && c.Maintenance == oldMain && c.ProductPCBnumber == oldPCBnumber && c.PlatformModul == oldplatformModul).ToList();
            //修改贴片根据旧的平台,类型,PCB板内容,找到对应内容,再将新的赋值过去
            var pick = db.Pick_And_Place.Where(c => c.Type == oldtype && c.Platform == oldplatform && c.Maintenance == oldMain && c.ProductPCBnumber == oldPCBnumber && c.PlatformModul == oldplatformModul).ToList();
            //修改手输 根据旧的平台,类型,PCB板内容,找到对应内容,再将新的赋值过去
            var Manual = db.Process_Capacity_Manual.Where(c => c.Type == oldtype && c.Platform == oldplatform && c.Maintenance == oldMain && c.ProductPCBnumber == oldPCBnumber && c.PlatformModul == oldplatformModul).ToList();
            ////修改插件 根据旧的平台,类型,PCB板内容,找到对应内容,再将新的赋值过去
            //var plug = db.Process_Capacity_Plugin.Where(c => c.Type == oldtype && c.Platform == oldplatform && c.Maintenance == oldMain && c.ProductPCBnumber == oldPCBnumber && c.PlatformModul == oldplatformModul).ToList();
            ////修改三防 根据旧的平台,类型,PCB板内容,找到对应内容,再将新的赋值过去
            ////var three = db.Process_Capacity_ThreeProf.Where(c => c.Type == oldtype && c.Platform == oldplatform && c.ProductPCBnumber == oldPCBnumber).ToList();
            ////three.ForEach(c => c.Type = type);
            ////three.ForEach(c => c.Platform = platfrom);
            ////three.ForEach(c => c.ProductPCBnumber = pcb);
            ////修改喷墨 根据旧的平台,类型,PCB板内容,找到对应内容,再将新的赋值过去
            //var inkjets = db.Process_Capacity_Inkjet.Where(c => c.Type == oldtype && c.Platform == oldplatform && c.Maintenance == oldMain && c.ProductPCBnumber == oldPCBnumber && c.PlatformModul == oldplatformModul).ToList();
            ////修改灌胶 根据旧的平台,类型,PCB板内容,找到对应内容,再将新的赋值过去
            //var glue = db.Process_Capacity_Glue.Where(c => c.Type == oldtype && c.Platform == oldplatform && c.Maintenance == oldMain && c.ProductPCBnumber == oldPCBnumber && c.PlatformModul == oldplatformModul).ToList();
            ////修改气密 根据旧的平台,类型,PCB板内容,找到对应内容,再将新的赋值过去
            //var airtights = db.Process_Capacity_Airtight.Where(c => c.Type == oldtype && c.Platform == oldplatform && c.Maintenance == oldMain && c.ProductPCBnumber == oldPCBnumber && c.PlatformModul == oldplatformModul).ToList();
            ////修改老化 根据旧的平台,类型,PCB板内容,找到对应内容,再将新的赋值过去
            //var burn = db.Process_Capacity_Burin.Where(c => c.Type == oldtype && c.Platform == oldplatform && c.Maintenance == oldMain && c.ProductPCBnumber == oldPCBnumber && c.PlatformModul == oldplatformModul).ToList();
            //修改关联
            var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == oldtype && c.Platform == oldplatform && c.Maintenance == oldMain && c.PlatformModul == oldplatformModul).ToList();
            //修改设备
            var equipment = db.Process_Capacity_Equipment.Where(c => c.Type == oldtype && c.Platform == oldplatform && c.Maintenance == oldMain && c.PlatformModul == oldplatformModul).ToList();

            if (type != oldtype)
            {
                deleteblance.ForEach(c => c.Type = type);
                pick.ForEach(c => c.Type = type);
                Manual.ForEach(c => c.Type = type);
                //inkjets.ForEach(c => c.Type = type);
                //glue.ForEach(c => c.Type = type);
                //airtights.ForEach(c => c.Type = type);
                //burn.ForEach(c => c.Type = type);
                relevance.ForEach(c => c.Type = type);
                equipment.ForEach(c => c.Type = type);
            }
            if (platfrom != oldplatform)
            {
                deleteblance.ForEach(c => c.Platform = platfrom);
                pick.ForEach(c => c.Platform = platfrom);
                Manual.ForEach(c => c.Platform = platfrom);
                //inkjets.ForEach(c => c.Platform = platfrom);
                //glue.ForEach(c => c.Platform = platfrom);
                //airtights.ForEach(c => c.Platform = platfrom);
                //burn.ForEach(c => c.Platform = platfrom);
                relevance.ForEach(c => c.Platform = platfrom);
                equipment.ForEach(c => c.Platform = platfrom);
            }
            if (pcb != oldPCBnumber)
            {
                deleteblance.ForEach(c => c.ProductPCBnumber = pcb);
                pick.ForEach(c => c.ProductPCBnumber = pcb);
                Manual.ForEach(c => c.ProductPCBnumber = pcb);
                equipment.ForEach(c => c.ProductPCBnumber = pcb);
                //inkjets.ForEach(c => c.ProductPCBnumber = pcb);
                //glue.ForEach(c => c.ProductPCBnumber = pcb);
                //airtights.ForEach(c => c.ProductPCBnumber = pcb);
                //burn.ForEach(c => c.ProductPCBnumber = pcb);
            }
            if (plafromodule != oldplatformModul)
            {
                deleteblance.ForEach(c => c.PlatformModul = plafromodule);
                pick.ForEach(c => c.PlatformModul = plafromodule);
                Manual.ForEach(c => c.PlatformModul = plafromodule);
                //inkjets.ForEach(c => c.PlatformModul = plafromodule);
                //glue.ForEach(c => c.PlatformModul = plafromodule);
                //airtights.ForEach(c => c.PlatformModul = plafromodule);
                //burn.ForEach(c => c.PlatformModul = plafromodule);
                relevance.ForEach(c => c.PlatformModul = plafromodule);
                equipment.ForEach(c => c.PlatformModul = plafromodule);
            }
            if (Maintenance != oldMain)
            {
                deleteblance.ForEach(c => c.Maintenance = Maintenance);
                pick.ForEach(c => c.Maintenance = Maintenance);
                Manual.ForEach(c => c.Maintenance = Maintenance);
                //inkjets.ForEach(c => c.Maintenance = Maintenance);
                //glue.ForEach(c => c.Maintenance = Maintenance);
                //airtights.ForEach(c => c.Maintenance = Maintenance);
                //burn.ForEach(c => c.Maintenance = Maintenance);
                relevance.ForEach(c => c.Maintenance = Maintenance);
                equipment.ForEach(c => c.Maintenance = Maintenance);
            }
            //填写日志
            UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "修改工序产能平台为" + oldplatform + "->" + platfrom + ",型号为" + oldtype + "->" + type + ",PCB板号为" + oldPCBnumber + "->" + pcb + ",平台模块" + oldplatformModul + "->" + plafromodule + ",pcn烘干时间" + oldpcbdry + "->" + pcbdry + ",外发工艺" + oldoutboundProcess + "->" + outboundProcess + ",干燥时间" + olddrytime + "->" + drytime + ",维护方式" + oldMain + "->" + Maintenance };
            db.UserOperateLog.Add(log);
            db.SaveChanges();//保存数据
            JObject value = new JObject();
            value.Add("message", true);
            value.Add("content", "修改成功");
            return Content(JsonConvert.SerializeObject(value));
        }

        //手输工段修改
        public ActionResult ManualChange(Process_Capacity_Manual newData, string statu)
        {
            if (Session["User"] == null)//判断是否有登录,如果没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
            }
            JObject value = new JObject();
            if (!ModelState.IsValid)//判断 Process_Capacity_Plugin 数据格式
            {
                value.Add("message", false);
                value.Add("content", "格式错误");
                return Content(JsonConvert.SerializeObject(value));
            }
            if (statu == "添加")
            {
                if (newData.Section == "插件")
                {
                    newData.SingleLampWorkingHours = 0.222;//写死的
                    newData.StandardCapacity = newData.PCBASingleLampNumber == 0 ? 0 : (decimal)(3600 / (newData.PCBASingleLampNumber * 0.222));//计算标准产能 3600/单灯数*0.2
                }
                newData.Operator = ((Users)Session["User"]).UserName;//登录姓名
                newData.OperateDT = DateTime.Now;//现在时间
                db.Process_Capacity_Manual.Add(newData);
                db.SaveChanges();//保存数据

                value.Add("message", true);
                var pluginitem = db.Process_Capacity_Manual.OrderByDescending(c => c.Id).Where(c => c.Type == newData.Type && c.ProductPCBnumber == newData.ProductPCBnumber && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.Section == newData.Section).FirstOrDefault();//查找刚刚新增的数据
                JObject pluginjobject = new JObject();
                pluginjobject.Add("Eidt", false);//前端用
                pluginjobject.Add("ID", pluginitem.Id);
                pluginjobject.Add("Name", pluginitem.Name); //工序名1
                pluginjobject.Add("Name2", pluginitem.Name2); //工序名2
                pluginjobject.Add("SingleLampWorkingHours", pluginitem.SingleLampWorkingHours);//插件机台固定标准单灯工时
                pluginjobject.Add("PCBASingleLampNumber", pluginitem.PCBASingleLampNumber);//插件PCBA单灯数
                pluginjobject.Add("StandardNumber", pluginitem.StandardNumber);//标配人数1
                pluginjobject.Add("StandardNumber2", pluginitem.StandardNumber2);//标配人数2
                pluginjobject.Add("StandardCapacity", pluginitem.StandardCapacity);//产能标准1
                pluginjobject.Add("StandardCapacity2", pluginitem.StandardCapacity2);//产能标准2
                pluginjobject.Add("DispensMachineNum", pluginitem.DispensMachineNum);//点胶机数量
                pluginjobject.Add("ScrewMachineNum", pluginitem.ScrewMachineNum);//螺丝机数量


                value.Add("content", pluginjobject);
                return Content(JsonConvert.SerializeObject(value));//返回给前面显示,实现不刷新就显示
            }
            else if (statu == "修改")
            {
                var old = db.Process_Capacity_Manual.Where(c => c.Id == newData.Id).FirstOrDefault();//查找要修改的数据
                                                                                                     //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "修改工序产能数据，工段" + old.Section + "，描述" + old.Name + "->" + newData.Name + "，人数" + old.StandardNumber + "->" + newData.StandardNumber + ",标准产能" + old.StandardCapacity + "->" + newData.StandardCapacity };


                db.UserOperateLog.Add(log);
                //修改值
                if (newData.Section == "插件")
                {
                    old.SingleLampWorkingHours = 0.222;
                    old.StandardCapacity = newData.PCBASingleLampNumber == 0 ? 0 : (decimal)(3600 / newData.PCBASingleLampNumber * 0.222);
                }
                else
                {
                    old.SingleLampWorkingHours = newData.SingleLampWorkingHours;
                    old.StandardCapacity = newData.StandardCapacity;
                }
                old.Name = newData.Name;
                old.Name2 = newData.Name2;
                old.PCBASingleLampNumber = newData.PCBASingleLampNumber;
                old.StandardNumber = newData.StandardNumber;
                old.StandardCapacity2 = newData.StandardCapacity2;
                old.StandardNumber2 = newData.StandardNumber2;
                old.DispensMachineNum = newData.DispensMachineNum;
                old.ScrewMachineNum = newData.ScrewMachineNum;
                //old.ModuleNeedNum2 = newData.ModuleNeedNum2;
                db.SaveChanges();//保存数据

                value.Add("message", true);
                value.Add("content", "修改成功");
                return Content(JsonConvert.SerializeObject(value));

            }
            else if (statu == "删除")
            {
                //查找要删除的数据
                var old = db.Process_Capacity_Manual.Find(newData.Id);
                //删除关联
                List<Process_Capacity_Relevance> result = new List<Process_Capacity_Relevance>();
                var relevance2 = db.Process_Capacity_Relevance.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.ChildSeaction == newData.Section && c.ChildID == newData.Id).ToList();
                result.AddRange(relevance2);
                var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.FatherSeaction == newData.Section && c.FatherID == newData.Id).ToList();
                if (relevance.Count != 0)
                {
                    foreach (var item in relevance)
                    {
                        result.AddRange(GetRelevanceList(item));
                        result.AddRange(relevance);
                    }
                }
                db.Process_Capacity_Relevance.RemoveRange(result);
                //删除设备
                var quiment = db.Process_Capacity_Equipment.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.Seaction == newData.Section && c.SeactionID == newData.Id).ToList();
                db.Process_Capacity_Equipment.RemoveRange(quiment);
                //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "删除工序产能数据，工段" + newData.Section + "，原数据,描述，单灯数，人数为" + old.Name + "，" + old.PCBASingleLampNumber + "，" + old.PCBASingleLampNumber };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                var delete = db.Process_Capacity_Manual.Find(newData.Id);
                db.Process_Capacity_Manual.Remove(delete);
                db.SaveChanges();//保存数据
                value.Add("message", true);
                value.Add("content", "删除成功");
                return Content(JsonConvert.SerializeObject(value));
            }

            return Content("true");
        }

        //添加设备
        public bool AddEquipment(Process_Capacity_Equipment Process_Capacity_Equipment, string statu)
        {
            if (statu == "添加")
            {
                db.Process_Capacity_Equipment.Add(Process_Capacity_Equipment);
                db.SaveChanges();
                return true;
            }
            if (statu == "修改")
            {
                db.Entry(Process_Capacity_Equipment).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            if (statu == "删除")
            {
                var temp = db.Process_Capacity_Equipment.Find(Process_Capacity_Equipment.Id);
                db.Process_Capacity_Equipment.Remove(temp);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        //显示设备信息
        public ActionResult DisplayEquipment(string type, string pcbnumber, string platform, string plafromodule, string Maintenance, string seaction, int id)
        {
            JArray result = new JArray();
            var info = db.Process_Capacity_Equipment.Where(c => c.Type == type && c.ProductPCBnumber == pcbnumber && c.Platform == platform && c.PlatformModul == plafromodule && c.Maintenance == Maintenance && c.Seaction == seaction && c.SeactionID == id).ToList();
            if (info.Count != 0)//如果找到设备数据信息
            {
                info.ForEach(c =>
                {
                    JObject item = new JObject();
                    item.Add("id", c.Id);
                    item.Add("EquipmentName", c.EquipmentName);//设备名称
                    item.Add("EquipmentCapacity", c.EquipmentCapacity);//设备产值
                    item.Add("EquipmentNum", c.EquipmentNum);//设备数量
                    item.Add("PersonEquipmentNum", c.PersonEquipmentNum);
                    item.Add("Statue", c.Statue);//类型
                    result.Add(item);
                });
            }
            return Content(JsonConvert.SerializeObject(result));
        }


        //工序平衡卡的输入和修改
        /// <summary>
        /// 修改点胶机何螺丝机，不再改模组需求数量
        /// </summary>
        public ActionResult Add_MachineNum(int id, int ScrewMachineNum, int DispensMachineNum)
        {
            var oldinfo = db.ProcessBalance.Find(id);//根据id查找到需要修改的信息,因为值默认是0,所以输入和修改的方法是一样的
                                                     //日志填写
            UserOperateLog log = new UserOperateLog()
            {
                Operator = ((Users)Session["User"]).UserName,
                OperateDT = DateTime.Now,
                OperateRecord =
            "修改工序产能数据，工段" + oldinfo.Section + "，描述" + oldinfo.Title + "，原数据螺丝机" + oldinfo.ScrewMachineNum + "->" + ScrewMachineNum + "，点胶机数量" + oldinfo.DispensMachineNum + "->" + DispensMachineNum
            };
            db.UserOperateLog.Add(log);//日志保存

            JObject value = new JObject();
            //修改数值
            oldinfo.DispensMachineNum = DispensMachineNum;
            oldinfo.ScrewMachineNum = ScrewMachineNum;
            db.SaveChanges();//数据保存
            value.Add("message", true);
            value.Add("content", "修改成功");
            return Content(JsonConvert.SerializeObject(value));
        }

        //已上传文件改文件名字
        public ActionResult Edit_ModuleName(int id, string name)
        {
            var blance = db.ProcessBalance.Find(id);//读取值
            JObject value = new JObject();
            //修改名称
            blance.ProcessDescription = name;
            db.SaveChanges();//数据保存
            value.Add("message", true);
            value.Add("content", "修改成功");
            return Content(JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// /// 上传平衡表后,调用此方法,或者是平衡卡修改调用此方法
        /// 填写工序描述
        /// 如果是模组装配,则还需要上传模组需求数
        /// </summary>
        /// <param name="id">数据id</param>
        /// <param name="smtmodeulneed">smt模组</param>
        /// <param name="plugmodeulneed">插件模组</param>
        /// <param name="aftermodeulneed">后焊模组</param>
        /// <param name="threemodeulneed">三防模组</param>
        /// <param name="bootnmodeulneed">打底壳模组</param>
        /// <param name="magneticmodeulneed">装磁吸模组</param>
        /// <param name="injekinmodeulneed">喷墨模组</param>
        /// <param name="gluemodeulneed">灌胶模组</param>
        /// <param name="airtightmodeulneed">气密模组</param>
        /// <param name="lockmodeulneed">锁面罩模组</param>
        /// <param name="modeulneed">模组装配模组</param>
        /// <param name="burnmodeulneed">老化模组</param>
        /// <param name="packmodeulneed">包装模组</param>
        /// </summary>
        public ActionResult Add_ModuleNeedNum(int id, decimal smtmodeulneed, decimal injekmodeulneed, decimal aftermodeulneed, decimal threemodeulneed, decimal bootnmodeulneed, decimal magneticmodeulneed, decimal pluginmodeulneed, decimal gluemodeulneed, decimal airtightmodeulneed, decimal lockmodeulneed, decimal modeulneed, decimal burnmodeulneed, decimal packmodeulneed, decimal moduleLinemoduleneed)
        {
            //var oldinfo = db.ProcessBalance.Find(id);//根据id查找到需要修改的信息,因为值默认是0,所以输入和修改的方法是一样的
            var blance = db.ProcessBalance.Find(id);//读取值

            //日志填写
            UserOperateLog log = new UserOperateLog()
            {
                Operator = ((Users)Session["User"]).UserName,
                OperateDT = DateTime.Now,
                OperateRecord =
            "修改工序产能数据，工段" + blance.Section + "，描述" + blance.Title + ",smt模组数量" + blance.SMTModuleNeedNum + "->" + smtmodeulneed + ",插件模组数量" + blance.PluginModuleNeedNum + "->" + pluginmodeulneed + ",后焊模组数量" + blance.AfterModuleNeedNum + "->" + aftermodeulneed + ",三防模组数量" + blance.ThreeModuleNeedNum + "->" + threemodeulneed + ",打底壳模组数量" + blance.BottnModuleNeedNum + "->" + bootnmodeulneed + ",装磁吸模组数量" + blance.MagneticModuleNeedNum + "->" + magneticmodeulneed + ",喷墨模组数量" + blance.InjekModuleNeedNum + "->" + injekmodeulneed + ",灌胶模组数量" + blance.GuleModuleNeedNum + "->" + gluemodeulneed + ",气密模组数量" + blance.AirtightModuleNeedNum + "->" + airtightmodeulneed + ",锁面罩模组数量" + blance.LockMasModuleNeedNum + "->" + lockmodeulneed + ",模组数量" + blance.ModuleNeedNum2 + "->" + modeulneed + ",老化模组数量" + blance.BuriInModuleNeedNum + "->" + burnmodeulneed + ",包装模组数量" + blance.PackModuleNeedNum + "->" + packmodeulneed + ",模块线模组数量" + blance.ModuleLineNeedNum + "->" + moduleLinemoduleneed
            };
            db.UserOperateLog.Add(log);//日志保存

            JObject value = new JObject();

            //填入模组号
            blance.SMTModuleNeedNum = smtmodeulneed;//贴片模组
            blance.PluginModuleNeedNum = pluginmodeulneed;//插件模组
            blance.AfterModuleNeedNum = aftermodeulneed;//后焊模组
            blance.ThreeModuleNeedNum = threemodeulneed;//三防模组
            blance.BottnModuleNeedNum = bootnmodeulneed;//打底壳模组
            blance.MagneticModuleNeedNum = magneticmodeulneed;//装磁吸模组
            blance.InjekModuleNeedNum = injekmodeulneed;//喷墨模组
            blance.GuleModuleNeedNum = gluemodeulneed;//灌胶模组
            blance.AirtightModuleNeedNum = airtightmodeulneed;//气密模组
            blance.LockMasModuleNeedNum = lockmodeulneed;//锁面罩模组
            blance.ModuleNeedNum2 = modeulneed;//模组装配模组
            blance.BuriInModuleNeedNum = burnmodeulneed;//老化模组
            blance.PackModuleNeedNum = packmodeulneed;//包装模组
            blance.ModuleLineNeedNum = moduleLinemoduleneed;


            //oldinfo.ModuleNeedNum2 = moduleNeed;
            db.SaveChanges();//数据保存
            value.Add("message", true);
            value.Add("content", "修改成功");
            return Content(JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// 显示所有模组需求
        /// </summary>
        /// <param name="id">模组装配数据的id </param>

        /// <returns></returns>
        public ActionResult GetModuleNeed(int id)
        {
            var info = db.ProcessBalance.Find(id);
            JObject result = new JObject();

            result.Add("SMTModuleNeedNum", info.SMTModuleNeedNum);
            result.Add("PluginModuleNeedNum", info.PluginModuleNeedNum);
            result.Add("AfterModuleNeedNum", info.AfterModuleNeedNum);
            result.Add("ThreeModuleNeedNum", info.ThreeModuleNeedNum);
            result.Add("BottnModuleNeedNum", info.BottnModuleNeedNum);
            result.Add("MagneticModuleNeedNum", info.MagneticModuleNeedNum);
            result.Add("InjekModuleNeedNum", info.InjekModuleNeedNum);
            result.Add("GuleModuleNeedNum", info.GuleModuleNeedNum);
            result.Add("AirtightModuleNeedNum", info.AirtightModuleNeedNum);
            result.Add("LockMasModuleNeedNum", info.LockMasModuleNeedNum);
            result.Add("ModuleNeedNum", info.ModuleNeedNum2);
            result.Add("BuriInModuleNeedNum", info.BuriInModuleNeedNum);
            result.Add("PackModuleNeedNum", info.PackModuleNeedNum);
            result.Add("ModuleLineNeedNum", info.ModuleLineNeedNum);
            return Content(JsonConvert.SerializeObject(result));
        }

        #endregion

        #endregion

        #region 贴片表操作

        /// <summary>
        /// 上传贴片表
        /// </summary>
        /// <param name="type"> 型号</param>
        /// <param name="PCBNumber"> PCB版号</param>
        /// <returns></returns>
        public ActionResult Upload_Pick_And_Place(string type, string PCBNumber, string platform, string plafmodule = null, string Maintenance = null)
        {
            JObject total = new JObject();
            try
            {
                JObject result = new JObject();
                HttpPostedFileBase uploadfile = Request.Files["fileup"];//查看是否有上传文件
                if (uploadfile == null)//如果没有文件上传,又调用此方法,意思为跳过
                {
                    Pick_And_Place pick_ = new Pick_And_Place();
                    pick_.Type = type;//类型
                    pick_.ProductPCBnumber = PCBNumber;//PCB板
                    pick_.Platform = platform;//平台
                    pick_.PlatformModul = plafmodule;//平台模块
                    pick_.Maintenance = Maintenance;//维护方式
                    pick_.Section = "/";//工段
                    db.Pick_And_Place.Add(pick_);
                    db.SaveChanges();//添加数据

                    //传回前端,实现不刷新显示
                    result.Add("SMTExaminanMessage", "未审核");
                    result.Add("icProductName", "/");
                    result.Add("icMaxStandardTotal", null);
                    result.Add("icMaxStandardOutput", null);
                    result.Add("icMinStandardTotal", null);
                    result.Add("icMinStandardOutput", null);
                    result.Add("LightProductName", "/");
                    result.Add("LightMaxStandardTotal", null);
                    result.Add("LightMaxStandardOutput", null);
                    result.Add("LightMinStandardTotal", null);
                    result.Add("LightMinStandardOutput", null);
                    result.Add("SMTmoduleneed", null);

                    total.Add("result", true);
                    total.Add("content", result);

                    return Content(JsonConvert.SerializeObject(total));
                }
                if (uploadfile.FileName == "")//如果选择空文件,提示错误
                {
                    total.Add("result", false);
                    total.Add("content", "请选择文件");
                    return Content(JsonConvert.SerializeObject(total));
                }

                string fileExt = Path.GetExtension(uploadfile.FileName);//返回扩展名
                StringBuilder sbtime = new StringBuilder();
                sbtime.Append(DateTime.Now.Year).Append(DateTime.Now.Month).Append(DateTime.Now.Day).Append(DateTime.Now.Hour).Append(DateTime.Now.Minute).Append(DateTime.Now.Second);//将当前时间转成字符串
                string dir = "/UploadFile/" + sbtime.ToString() + fileExt;//设置虚拟路径
                string realfilepath = Request.MapPath(dir);//将虚拟地址映射到物理地址上
                string readDir = Path.GetDirectoryName(realfilepath);//获取路径字符串
                if (!Directory.Exists(readDir))//判断路径是否正确,不正确创建新文件夹
                    Directory.CreateDirectory(readDir);
                uploadfile.SaveAs(realfilepath);
                // 提取数据
                var dt = ExcelTool.ExcelToDataTable(true, realfilepath);
                var totalCount = dt.Rows.Count;//文件总行数
                int j = 16;
                while (dt.Rows[j][0].ToString() != "修订")//找到表格中为"修订"的那一行
                {
                    j++;
                }
                var content = "";
                int k = j;
                while (k + 1 < totalCount && dt.Rows[k + 1][1].ToString() != "")//找到修订模块的内容,并将内容存到content中,以"&&"隔开,如修正内容1&&修正内容2
                {
                    content = content + dt.Rows[k + 1][1].ToString() + "&&";
                    k++;
                }
                var number = dt.Rows[1][1].ToString();//版本号
                var serialNumberList = db.Pick_And_Place.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Platform == platform && c.SerialNumber == number && c.Maintenance == Maintenance).ToList();//查找是否含有相同含本号的数据信息
                if (serialNumberList.Count != 0)//如果有,提示错误
                {
                    total.Add("result", false);
                    total.Add("content", "已有此版本，请修改版本");
                    return Content(JsonConvert.SerializeObject(total));
                }
                //将数据存入数据库
                List<Pick_And_Place> picklist = new List<Pick_And_Place>();
                for (int i = 2; i < 16; i++)
                {
                    Pick_And_Place pick = new Pick_And_Place();
                    pick.Type = type;
                    pick.ProductPCBnumber = PCBNumber;
                    pick.Platform = platform;
                    pick.PlatformModul = plafmodule;//平台模块
                    pick.Maintenance = Maintenance;//维护方式
                    pick.SerialNumber = dt.Rows[1][1].ToString();//编号
                    pick.Section = dt.Rows[1][3].ToString();//工段
                    pick.FileName = dt.Rows[1][6].ToString();//文件名称
                    pick.Line = i - 1;//线别
                    pick.MachineConfiguration = dt.Rows[i + 1][1].ToString();//机台配置
                    pick.ProductType = dt.Rows[i + 1][2].ToString();//产品型号
                    pick.PCBNumber = dt.Rows[i + 1][3].ToString();//pcb版号
                    pick.ProcessDescription = dt.Rows[i + 1][4].ToString();//工序描述
                    pick.Print = DataTypeChange.IsNumberic(dt.Rows[i + 1][5].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[i + 1][5]), 2);//印刷
                    pick.SolderPasteInspection = DataTypeChange.IsNumberic(dt.Rows[i + 1][6].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[i + 1][6]), 2);//锡膏检测
                    pick.PressedPatchNut = DataTypeChange.IsNumberic(dt.Rows[i + 1][7].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[i + 1][7]), 2);//压贴片螺母
                    pick.SMTMachineNetWork = DataTypeChange.IsNumberic(dt.Rows[i + 1][8].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[i + 1][8]), 2);//贴片机净作业
                    pick.PersonNum = DataTypeChange.IsNumberic(dt.Rows[i + 1][9].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[i + 1][9]);//人数
                    pick.Bottleneck = DataTypeChange.IsNumberic(dt.Rows[i + 1][10].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[i + 1][10]), 2);//瓶颈  
                    pick.CapacityPerHour = DataTypeChange.IsNumberic(dt.Rows[i + 1][11].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[i + 1][11]), 2);//每小时标准产能
                    pick.PerCapitaCapacity = DataTypeChange.IsNumberic(dt.Rows[i + 1][12].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[i + 1][12]), 2);//每小时人均产能
                    pick.LatestUnitPrice = DataTypeChange.IsNumberic(dt.Rows[i + 1][13].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[i + 1][13]), 2);//产品最新单价
                    pick.Remark = dt.Rows[i + 1][14].ToString();//备注


                    pick.MakingPeople = dt.Rows[j + 1][5].ToString();//制定人
                    if (dt.Rows[j + 2][5].ToString() == "")//如果制定人的模块内容为空,则时间传null,因为时间的类型是datetime?不能传""
                    {
                        pick.MakingTime = null;
                    }
                    else
                    {
                        var value = dt.Rows[j + 2][5].ToString();
                        pick.MakingTime = TimeChange(value);
                    }

                    pick.ExaminanPeople = dt.Rows[j + 1][8].ToString();//审批
                    if (dt.Rows[j + 2][8].ToString() == "")//如果审批人的模块内容为空,则时间传null,因为时间的类型是datetime?不能传""
                    {
                        pick.ExaminanTime = null;
                    }
                    else
                    {
                        var value = dt.Rows[j + 2][8].ToString();
                        pick.ExaminanTime = TimeChange(value);
                        pick.IsPassExaminan = true;//审核默认为通过
                    }

                    pick.ApproverPeople = dt.Rows[j + 1][11].ToString();//批准
                    if (dt.Rows[j + 2][11].ToString() == "")//如果批准人的模块内容为空,则时间传null,因为时间的类型是datetime?不能传""
                    {
                        pick.ApproverTime = null;
                    }
                    else
                    {
                        //时间转换
                        var time3 = dt.Rows[j + 2][11].ToString().Split('/');
                        var year = "20" + time3[2];
                        pick.ApproverTime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(time3[0]), Convert.ToInt32(time3[1]));

                        pick.IsPassApprover = true;//批准默认为通过
                    }

                    pick.ControlledPeople = dt.Rows[j + 1][14].ToString();//受控
                    if (dt.Rows[j + 2][14].ToString() == "")//如果受控人的模块内容为空,则时间传null,因为时间的类型是datetime?不能传""
                    {
                        pick.ControlledTime = null;
                    }
                    else
                    {
                        //时间转换
                        var time4 = dt.Rows[j + 2][14].ToString().Split('/');
                        var year = "20" + time4[2];
                        pick.ControlledTime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(time4[0]), Convert.ToInt32(time4[1]));

                        pick.IsPassControlled = true;//受控默认为通过
                    }
                    if (content != null)
                    {
                        pick.RevisionContent = content.Substring(0, content.Length - 2);//去掉最后的两个&&
                    }

                    picklist.Add(pick);
                }
                db.Pick_And_Place.AddRange(picklist);
                db.SaveChanges();//往数据库添加数据
                var iccount = db.Pick_And_Place.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Platform == platform).ToList();//查找刚才添加的数据
                                                                                                                                                 //返回数据给前面,实现不刷新显示
                if (iccount.Where(c => c.ProcessDescription == "IC面贴装").Count() == 0)//如果数据没有IC面贴装数据,则传回null
                {
                    result.Add("SMTExaminanMessage", "审核通过");
                    result.Add("icProductName", "IC面贴装");
                    result.Add("icMaxStandardTotal", null);
                    result.Add("icMaxStandardOutput", null);
                    result.Add("icMinStandardTotal", null);
                    result.Add("icMinStandardOutput", null);
                }
                else
                {
                    var icMaxStandardTotal = iccount.Where(c => c.ProcessDescription == "IC面贴装").Max(c => c.PersonNum);//IC面最大标准人数
                    var icMaxStandardOutput = iccount.Where(c => c.ProcessDescription == "IC面贴装").Max(c => c.CapacityPerHour);//IC面最大标配产能
                    var icMinStandardTotal = iccount.Where(c => c.ProcessDescription == "IC面贴装").Min(c => c.PersonNum);//IC面最小标准人数
                    var icMinStandardOutput = iccount.Where(c => c.ProcessDescription == "IC面贴装").Min(c => c.CapacityPerHour);//IC面最小标准产能
                    result.Add("SMTExaminanMessage", "审核通过");
                    result.Add("icProductName", "IC面贴装");
                    result.Add("icMaxStandardTotal", icMaxStandardTotal);
                    result.Add("icMaxStandardOutput", icMaxStandardOutput);
                    result.Add("icMinStandardTotal", icMinStandardTotal);
                    result.Add("icMinStandardOutput", icMinStandardOutput);
                }// result.Add("LimtProductName", true);
                if (iccount.Where(c => c.ProcessDescription == "灯面贴装").Count() == 0)//如果数据没有灯面贴装数据,则传回null
                {
                    result.Add("SMTExaminanMessage", "审核通过");
                    result.Add("icProductName", "灯面贴装");
                    result.Add("icMaxStandardTotal", null);
                    result.Add("icMaxStandardOutput", null);
                    result.Add("icMinStandardTotal", null);
                    result.Add("icMinStandardOutput", null);
                }
                else
                {
                    var LightMaxStandardTotal = iccount.Where(c => c.ProcessDescription == "灯面贴装").Max(c => c.PersonNum);//灯面最大标准人数
                    var LightMaxStandardOutput = iccount.Where(c => c.ProcessDescription == "灯面贴装").Max(c => c.CapacityPerHour);//灯面最大标配产能
                    var LightMinStandardTotal = iccount.Where(c => c.ProcessDescription == "灯面贴装").Min(c => c.PersonNum);//灯面最小标准人数
                    var LightMinStandardOutput = iccount.Where(c => c.ProcessDescription == "灯面贴装").Min(c => c.CapacityPerHour);//灯面最小标准产能
                    result.Add("LightProductName", "灯面贴装");
                    result.Add("LightMaxStandardTotal", LightMaxStandardTotal);
                    result.Add("LightMaxStandardOutput", LightMaxStandardOutput);
                    result.Add("LightMinStandardTotal", LightMinStandardTotal);
                    result.Add("LightMinStandardOutput", LightMinStandardOutput);
                }
                result.Add("SMTmoduleneed", null);
                total.Add("result", true);
                total.Add("content", result);
                return Content(JsonConvert.SerializeObject(total));
            }
            catch (Exception ex)
            {
                total.Add("result", false);
                total.Add("content", ex.Message);
                return Content(JsonConvert.SerializeObject(total));
            }
        }

        /// <summary>
        /// 显示全部版本贴片表
        /// </summary>
        /// <param name="type">型号</param>
        /// <param name="PCBNumber">PCB版号</param>
        /// <param name="platform">平台</param>
        /// <returns></returns>
        public ActionResult DisplayALL_Pick_And_Place(string type, string PCBNumber, string platform, string Maintenance)
        {
            var infoList = db.Pick_And_Place.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Maintenance == Maintenance && c.Platform == platform).Select(c => c.SerialNumber).Distinct().ToList();//查找符合条件的贴片信息,并提取其中的版本号
            JArray result = new JArray();
            foreach (var item in infoList)//循环版本号
            {
                var info = db.Pick_And_Place.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Maintenance == Maintenance && c.Platform == platform && c.SerialNumber == item).ToList();//找到不同版本号的信息
                var message = info.FirstOrDefault();
                JObject number = new JObject();
                number.Add("number", item);//版本号
                number.Add("Section", message.Section);//工段
                number.Add("FileName", message.FileName);//标题
                JArray line = new JArray();
                foreach (var infoitem in info)
                {
                    JObject itemjobject = new JObject();
                    itemjobject.Add("Line", infoitem.Line);//线别
                    itemjobject.Add("MachineConfiguration", infoitem.MachineConfiguration); //机台配置
                    itemjobject.Add("ProductType", infoitem.ProductType);//产品型号
                    itemjobject.Add("PCBNumber", infoitem.PCBNumber);//PCB版号
                    itemjobject.Add("ProcessDescription", infoitem.ProcessDescription);//工序描述
                    itemjobject.Add("Print", infoitem.Print);//印刷
                    itemjobject.Add("SolderPasteInspection", infoitem.SolderPasteInspection);//锡膏检测
                    itemjobject.Add("PressedPatchNut", infoitem.PressedPatchNut);//压贴片螺母
                    itemjobject.Add("SMTMachineNetWork", infoitem.SMTMachineNetWork);//贴片机净作业
                    itemjobject.Add("PersonNum", infoitem.PersonNum);//人数
                    itemjobject.Add("Bottleneck", infoitem.Bottleneck);//瓶颈
                    itemjobject.Add("CapacityPerHour", infoitem.CapacityPerHour);//每小时标准产能
                    itemjobject.Add("PerCapitaCapacity", infoitem.PerCapitaCapacity);//每小时人均产能
                    itemjobject.Add("LatestUnitPrice", infoitem.LatestUnitPrice);//产品最新单价
                    itemjobject.Add("Remark", infoitem.Remark);//备注
                    line.Add(itemjobject);
                }

                number.Add("Line", line);
                number.Add("RevisionContent", message.RevisionContent);//修订内容
                number.Add("MakingPeople", message.MakingPeople);//制定人
                number.Add("MakingTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", message.MakingTime));//制定时间
                number.Add("IsPassExaminan", message.IsPassExaminan);//审核是否通过
                number.Add("ExaminanPeople", message.ExaminanPeople);//审核人
                number.Add("ExaminanTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", message.ExaminanTime));//审核日期
                number.Add("IsPassApprover", message.IsPassApprover);//批准是否通过
                number.Add("ApproverPeople", message.ApproverPeople);//批准人
                number.Add("ApproverTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", message.ApproverTime));//批准日趋
                number.Add("IsPassControlled", message.IsPassControlled);//受控是否通过
                number.Add("ControlledPeople", message.ControlledPeople);//受控人
                number.Add("ControlledTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", message.ControlledTime));//受控日期

                result.Add(number);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 删除贴片表
        /// </summary>
        /// <param name="type">型号</param>
        /// <param name="PCBNumber">PCB版号</param>
        /// <param name="platform">平台</param>
        /// <returns></returns>
        public ActionResult Delete_Pick_And_Place(string type, string PCBNumber, string platform, string Maintenance)
        {
            if (Session["User"] == null)//判断是否有登录信息,没有就跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
            }
            var item = db.Pick_And_Place.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Maintenance == Maintenance && c.Platform == platform).Max(c => c.SerialNumber);//找到符合筛选条件的最大版本
            var info = db.Pick_And_Place.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Maintenance == Maintenance && c.Platform == platform && c.SerialNumber == item).ToList();//找到最大版本的数据信息
            var message = info.FirstOrDefault();
            //删除设备表
            var quiment = db.Process_Capacity_Equipment.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Maintenance == Maintenance && c.Platform == platform && c.Seaction == "SMT").ToList();
            db.Process_Capacity_Equipment.RemoveRange(quiment);
            //填写日志
            UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "删除工序产能数据，工段SMT，型号，PCB编号,平台为" + type + "，" + PCBNumber + "，" + platform };
            db.UserOperateLog.Add(log);

            JObject number = new JObject();
            if (message.ExaminanPeople != "" && message.ExaminanTime != null)//找到其中一条信息,查看其中审核信息是否为空,如果不为空,则查看删除人是否与审核人一致,一致能删,否则不能删
            {
                //if (((Users)Session["User"]).UserName == message.ExaminanPeople || ((Users)Session["User"]).Role == "系统管理员")//删除人是否与审核人一致
                //{
                db.Pick_And_Place.RemoveRange(info);
                db.SaveChanges();
                number.Add("message", true);
                number.Add("content", "删除成功");
                //}
                //else
                //{
                //    number.Add("message", false);
                //    number.Add("content", "删除失败，此表已审核，需审核人员删除");
                //}
            }
            else//没有审核则可以删除
            {
                db.Pick_And_Place.RemoveRange(info);
                db.SaveChanges();
                number.Add("message", true);
                number.Add("content", "删除成功");
            }
            return Content(JsonConvert.SerializeObject(number));
        }

        /// <summary>
        /// 审核人签名或批准人签名
        /// </summary>
        /// <param name="type">型号</param>
        /// <param name="PCBNumber">PCB版本号</param>
        /// <param name="Section">工段</param>
        /// <param name="number">编号</param>
        /// <param name="status">审核或批准</param>
        public bool Autograph_Pick_And_Place(string type, string PCBNumber, string Section, string number, string status, bool isPass)
        {
            if (Session["User"] == null)//判断是否有登录信息,没有则返回false
            {
                return false;
            }
            var info = db.Pick_And_Place.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Section == Section && c.SerialNumber == number).ToList();//查找符合条件 的数据信息
            if (status == "审核")
            {
                info.ForEach(c => { c.ExaminanPeople = ((Users)Session["User"]).UserName; c.ExaminanTime = DateTime.Now; c.IsPassExaminan = isPass; });//修改审核信息
                db.SaveChanges();
                return true;
            }
            if (status == "批准")
            {
                info.ForEach(c => { c.ApproverPeople = ((Users)Session["User"]).UserName; c.ApproverTime = DateTime.Now; c.IsPassApprover = isPass; });//修改批准信息
                db.SaveChanges();
                return true;
            }
            if (status == "受控")
            {
                info.ForEach(c => { c.ControlledPeople = ((Users)Session["User"]).UserName; c.ControlledTime = DateTime.Now; c.IsPassControlled = isPass; });//修改受控信息
                db.SaveChanges();
                return true;
            }
            return false;
        }

        #endregion


        #region 工序平衡卡表

        /// <summary>
        /// 上传工序平衡表
        /// </summary>
        /// <param name="type">型号</param>
        /// <param name="PCBNumber">PCB版号</param>
        /// <returns></returns>
        public ActionResult Upload_ProcessBalance(string type, string PCBNumber, string platform, string seaction, string Maintenance = null, string plafmodule = null, bool isnull = false, string name = "")
        {
            JObject total = new JObject();
            try
            {
                JObject jObject = new JObject();
                if (isnull)
                {
                    var same = db.ProcessBalance.Where(c => c.Platform == platform && c.Type == type && c.ProductPCBnumber == PCBNumber && c.Maintenance == Maintenance && c.Section == seaction && c.ProcessDescription == name).ToList();//查看上传的空模组是否有相同的
                    if (same.Count != 0)
                    {
                        total.Add("result", false);
                        total.Add("content", "已有相同名字的工序,请确认名字是否正确");
                        return Content(JsonConvert.SerializeObject(total));
                    }

                    ProcessBalance processBalance1 = new ProcessBalance();
                    processBalance1.Platform = platform;//平台
                    processBalance1.Type = type;//类型
                    processBalance1.ProductPCBnumber = PCBNumber;//PCB板
                    processBalance1.Section = seaction;//工段
                    processBalance1.Maintenance = Maintenance;
                    processBalance1.PlatformModul = plafmodule;//平台模块
                    processBalance1.ProcessDescription = name; //工序描述
                    processBalance1.Title = name; //工序描述
                    db.ProcessBalance.Add(processBalance1);
                    db.SaveChanges();//添加数据

                    total.Add("result", true);
                    var message1 = db.ProcessBalance.OrderByDescending(c => c.Id).Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Maintenance == Maintenance && c.Platform == platform && c.Section == seaction && c.ProcessDescription == name).FirstOrDefault();//查找刚刚添加的数据

                    jObject = BlanceItem(message1.Id, name, seaction);//调用通用模板,返回前面实现不刷新显示
                    total.Add("content", jObject);
                    return Content(JsonConvert.SerializeObject(total));
                }
                HttpPostedFileBase uploadfile = Request.Files["fileup"];//获得前端上传的文件
                if (uploadfile == null)//如果文件未null,又调用此方法,说明是跳过
                {
                    ProcessBalance processBalance1 = new ProcessBalance();
                    processBalance1.Platform = platform;//平台
                    processBalance1.Type = type;//类型
                    processBalance1.ProductPCBnumber = PCBNumber;//PCB板
                    processBalance1.Section = seaction;//工段
                    processBalance1.PlatformModul = plafmodule;//平台模块
                    processBalance1.Maintenance = Maintenance;
                    processBalance1.Title = "/";
                    db.ProcessBalance.Add(processBalance1);
                    db.SaveChanges();//添加数据
                    total.Add("result", true);
                    var message1 = db.ProcessBalance.OrderByDescending(c => c.Id).Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Maintenance == Maintenance && c.Platform == platform && c.Section == seaction && c.Title == "/").FirstOrDefault();//查找刚刚添加的数据

                    jObject = BlanceItem(message1.Id, "/", seaction);//调用通用模板,返回前面实现不刷新显示
                    total.Add("content", jObject);
                    return Content(JsonConvert.SerializeObject(total));
                }
                if (uploadfile.FileName == "")//如果选择空文件,提示错误
                {
                    total.Add("result", false);
                    total.Add("content", "请选择文件");
                    return Content(JsonConvert.SerializeObject(total));
                }

                string fileExt = Path.GetExtension(uploadfile.FileName);//获得文件扩展名
                StringBuilder sbtime = new StringBuilder();
                sbtime.Append(DateTime.Now.Year).Append(DateTime.Now.Month).Append(DateTime.Now.Day).Append(DateTime.Now.Hour).Append(DateTime.Now.Minute).Append(DateTime.Now.Second);//将时间装换字符
                string dir = "/UploadFile/" + sbtime.ToString() + fileExt;//等到虚拟路径
                string realfilepath = Request.MapPath(dir);//将虚拟路径映射到物理路径上
                string readDir = Path.GetDirectoryName(realfilepath);
                if (!Directory.Exists(readDir))//判断路径是否正确,错误,新建一个文件夹
                    Directory.CreateDirectory(readDir);
                uploadfile.SaveAs(realfilepath);
                //提取数据
                var dt = ExcelTool.ExcelToDataTable(true, realfilepath);
                var totalcount = dt.Rows.Count;
                int j = 16;
                while (dt.Rows[j][0].ToString() != "修订")//找到"修订"的那一行
                {
                    j++;
                }
                var content = "";
                int k = j;
                while (k + 1 < totalcount && dt.Rows[k + 1][1].ToString() != "")//将修订模块的数据赋值到content ,并以"☆"隔开
                {
                    content = content + dt.Rows[k + 1][1].ToString() + "☆";
                    k++;
                }
                var number = dt.Rows[1][1].ToString();//版本号
                var title = dt.Rows[2][1].ToString();//工序描述
                var isExit = db.ProcessBalance.Where(c => c.Platform == platform && c.Type == type && c.ProductPCBnumber == PCBNumber && c.SerialNumber == number && c.Section == seaction && c.Title == title && c.Maintenance == Maintenance).ToList();//查看上传的版本号是否重复
                if (isExit.Count != 0)
                {
                    total.Add("result", false);
                    total.Add("content", "已有此版本，请修改版本");
                    return Content(JsonConvert.SerializeObject(total));
                }
                ProcessBalance processBalance = new ProcessBalance();
                processBalance.Platform = platform;
                processBalance.Type = type;
                processBalance.ProductPCBnumber = PCBNumber;
                processBalance.SerialNumber = dt.Rows[1][1].ToString();//编号
                processBalance.Section = seaction;//工段
                processBalance.PlatformModul = plafmodule;//平台模块
                processBalance.Maintenance = Maintenance;
                processBalance.Title = dt.Rows[2][1].ToString();//标题
                processBalance.StandardTotal = DataTypeChange.IsNumberic(dt.Rows[2][5].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[2][5]);//标准总人数
                processBalance.BalanceRate = DataTypeChange.IsNumberic(dt.Rows[2][6].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[2][6]) * 100, 2);//平衡率
                processBalance.Bottleneck = DataTypeChange.IsNumberic(dt.Rows[2][7].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[2][7]), 2);//瓶颈
                processBalance.StandardOutput = DataTypeChange.IsNumberic(dt.Rows[2][8].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[2][8]), 2);//标准产量
                processBalance.StandardHourlyOutputPerCapita = DataTypeChange.IsNumberic(dt.Rows[2][10].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[2][10]), 2);//标准人均时产量
                processBalance.ProductWorkingHours = DataTypeChange.IsNumberic(dt.Rows[2][12].ToString()) == false ? 0 : Math.Round(Convert.ToDecimal(dt.Rows[2][12]), 2);//产品工时

                string ProcessName = "";
                int pro = 4;
                while (dt.Rows[pro][1].ToString() != "")
                {
                    ProcessName = ProcessName + dt.Rows[pro][1].ToString() + "☆";
                    pro++;
                }
                processBalance.ProcessName = ProcessName.Substring(0, ProcessName.Length - 1);//工序名称

                string StandarPersondNumber = "";
                for (int i = 4; i <= pro; i++)
                {
                    StandarPersondNumber = StandarPersondNumber + dt.Rows[i][5].ToString() + "☆";
                }

                processBalance.StandarPersondNumber = StandarPersondNumber.Substring(0, StandarPersondNumber.Length - 1);//标准人数

                string StandardNumber = "";
                for (int i = 4; i <= pro; i++)
                {
                    StandardNumber = StandardNumber + dt.Rows[i][6].ToString() + "☆";
                }

                processBalance.StandardNumber = StandardNumber.Substring(0, StandardNumber.Length - 1);//标准工时

                string UnioTime = "";
                for (int i = 4; i <= pro; i++)
                {
                    UnioTime = UnioTime + dt.Rows[i][7].ToString() + "☆";
                }

                processBalance.UnioTime = UnioTime.Substring(0, UnioTime.Length - 1);//单位工时

                string JigName = "";
                for (int i = 4; i <= pro; i++)
                {
                    JigName = JigName + dt.Rows[i][8].ToString() + "☆";
                }

                processBalance.JigName = JigName.Substring(0, JigName.Length - 1);//夹具名称

                string MachineTime = "";
                for (int i = 4; i <= pro; i++)
                {
                    MachineTime = MachineTime + dt.Rows[i][10].ToString() + "☆";
                }

                processBalance.MachineTime = MachineTime.Substring(0, MachineTime.Length - 1);//机器时间

                string MachineNumber = "";
                for (int i = 4; i <= pro; i++)
                {
                    MachineNumber = MachineNumber + dt.Rows[i][11].ToString() + "☆";
                }

                processBalance.MachineNumber = MachineNumber.Substring(0, MachineNumber.Length - 1);//机器数量

                string JigNum = "";
                for (int i = 4; i <= pro; i++)
                {
                    JigNum = JigNum + dt.Rows[i][12].ToString() + "☆";
                }

                processBalance.JigNum = JigNum.Substring(0, JigNum.Length - 1);//夹具数量

                string Remark = "";
                for (int i = 4; i <= pro; i++)
                {
                    Remark = Remark + dt.Rows[i][13].ToString() + "☆";
                }

                processBalance.Remark = Remark.Substring(0, Remark.Length - 1);//备注

                processBalance.MakingPeople = dt.Rows[j + 1][7].ToString();//制定人
                if (dt.Rows[j + 2][7].ToString() == "")//判断是否有制定人,没有制定时间为null 
                {
                    processBalance.MakingTime = null;
                }
                else
                {
                    var value = dt.Rows[j + 2][7].ToString();
                    processBalance.MakingTime = TimeChange(value);
                }

                processBalance.ExaminanPeople = dt.Rows[j + 1][9].ToString();//审批
                if (dt.Rows[j + 2][9].ToString() == "")//判断是否有审批人,没有审批时间为null 
                {
                    processBalance.ExaminanTime = null;
                }
                else
                {
                    //时间转换
                    var time2 = dt.Rows[j + 2][9].ToString().Split('/');
                    var year = "20" + time2[2];
                    processBalance.ExaminanTime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(time2[0]), Convert.ToInt32(time2[1]));
                    processBalance.IsPassExaminan = true;
                }

                processBalance.ApproverPeople = dt.Rows[j + 1][11].ToString();//批准
                if (dt.Rows[j + 2][11].ToString() == "")//判断是否有批准人,没有批准时间为null 
                {
                    processBalance.ApproverTime = null;
                }
                else
                {
                    //时间转换
                    var time3 = dt.Rows[j + 2][11].ToString().Split('/');
                    var year = "20" + time3[2];
                    processBalance.ApproverTime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(time3[0]), Convert.ToInt32(time3[1]));
                    processBalance.IsPassApprover = true;
                }

                processBalance.ControlledPeople = dt.Rows[j + 1][13].ToString();//受控
                if (dt.Rows[j + 2][13].ToString() == "")//判断是否有受控人,没有受控时间为null 
                {
                    processBalance.ControlledTime = null;
                }
                else
                {
                    //时间装换
                    var time4 = dt.Rows[j + 2][13].ToString().Split('/');
                    var year = "20" + time4[2];
                    processBalance.ControlledTime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(time4[0]), Convert.ToInt32(time4[1]));
                    processBalance.IsPassControlled = true;
                }
                if (content != "")
                {
                    processBalance.RevisionContent = content.Substring(0, content.Length - 1);//去除最后的符号"☆"
                }
                db.ProcessBalance.Add(processBalance);
                db.SaveChanges();//数据保存
                var message = db.ProcessBalance.OrderByDescending(c => c.Id).Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Maintenance == Maintenance && c.Platform == platform && c.PlatformModul == plafmodule && c.Section == seaction && c.Title == title).FirstOrDefault();//读取刚刚新增的数据
                jObject = BlanceItem(message.Id, message.Title, seaction);//调用通用模板,返回json给前端,实现不刷新显示
                total.Add("result", true);
                total.Add("content", jObject);
                return Content(JsonConvert.SerializeObject(total));
            }
            catch (Exception ex)
            {
                total.Add("result", false);
                total.Add("content", ex.Message);
                return Content(JsonConvert.SerializeObject(total));
            }
        }
        /// <summary>
        /// 上传平衡表通用模板,返回给前端
        /// </summary>
        /// <param name="Id">id</param>
        /// <param name="Title">工序描述</param>
        /// <param name="seaction">工段</param>
        /// <returns></returns>
        public JObject BlanceItem(int Id, string Title, string seaction)
        {
            JObject jObject = new JObject();
            switch (seaction)
            {
                case "后焊":
                    jObject.Add("AfterWeldExaminanMessage", "未审核");
                    jObject.Add("AfterWeldID", Id);
                    jObject.Add("AfterWeldProcessName", Title);
                    jObject.Add("AfterWeldStandardTotal", null);
                    jObject.Add("AfterWeldStandardOutput", null);
                    jObject.Add("AfterWeldStandardHourlyOutputPerCapita", null);
                    jObject.Add("AfterWeldPdf", false);
                    jObject.Add("AfterWeldImg", false);
                    break;
                case "三防":
                    jObject.Add("ThreeProfExaminanMessage", "未审核");
                    jObject.Add("ThreeProfID", Id);
                    jObject.Add("ThreeProfProcessName", Title);
                    jObject.Add("ThreeProfStandardTotal", null);
                    jObject.Add("ThreeProfStabdardOutput", null);
                    jObject.Add("ThreeProfPdf", false);
                    jObject.Add("ThreeProfJpg", false);
                    break;
                case "打底壳":
                    jObject.Add("BottomCasEidt", false);//前端用
                    jObject.Add("BottomCasExaminanMessage", "未审核");
                    jObject.Add("BottomCasID", Id);
                    jObject.Add("BottomCasProcessName", Title);
                    jObject.Add("BottomCasStandardTotal", null);
                    jObject.Add("BottomCasStandardOutput", null);
                    jObject.Add("BottomCasStandardHourlyOutputPerCapita", null);
                    jObject.Add("BottomCasDispensMachineNum", null);
                    jObject.Add("BottomCasScrewMachineNum", null);
                    jObject.Add("BottomCasPdf", false);
                    jObject.Add("BottomCasImg", false);
                    break;
                case "锁面罩":
                    jObject.Add("LockTheMaskEdit", false);//前端用
                    jObject.Add("LockTheMaskExaminanMessage", "未审核");
                    jObject.Add("LockTheMaskID", Id);
                    jObject.Add("LockTheMaskProcessName", Title);
                    jObject.Add("LockTheMaskStandardTotal", null);
                    jObject.Add("LockTheMaskStandardOutput", null);
                    jObject.Add("LockTheMaskStandardHourlyOutputPerCapita", null);
                    jObject.Add("LockTheMaskScrewMachineNum", null);
                    jObject.Add("LockTheMaskPdf", false);
                    jObject.Add("LockTheMaskImg", false);
                    break;
                case "模组装配":
                    jObject.Add("ModuleID", Id);
                    jObject.Add("ModuleExaminanMessage", "未审核");
                    jObject.Add("ModuleProcessName", Title);
                    jObject.Add("ModuleStandardTotal", null);
                    jObject.Add("ModuleBalanceRate", null);
                    jObject.Add("ModuleBottleneck", null);
                    jObject.Add("ModuleStandardOutput", null);
                    jObject.Add("ModuleStandardHourlyOutputPerCapita", null);
                    jObject.Add("ModulePdf", false);
                    jObject.Add("ModuleImg", false);
                    break;
                case "包装":
                    jObject.Add("PackingID", Id);
                    jObject.Add("PackingExaminanMessage", "未审核");
                    jObject.Add("PackingProcessName", Title);
                    jObject.Add("PackingStandardTotal", null);
                    jObject.Add("PackingStandardOutput", null);
                    jObject.Add("PackingPdf", false);
                    jObject.Add("PackingImg", false);
                    break;
                case "装磁吸安装板":
                    jObject.Add("MagneticExaminanMessage", "未审核");
                    jObject.Add("MagneticID", Id);
                    jObject.Add("MagneticProcessName", Title);
                    jObject.Add("MagneticStandardTotal", null);
                    jObject.Add("MagneticStabdardOutput", null);
                    jObject.Add("MagneticStandardHourlyOutputPerCapita", null);
                    jObject.Add("MagneticPdf", false);
                    jObject.Add("MagneticImg", false);
                    break;
                case "模块线":
                    jObject.Add("ModuleLineExaminanMessage", "未审核");
                    jObject.Add("ModuleLineID", Id);
                    jObject.Add("ModuleLineProcessName", Title);
                    jObject.Add("ModuleLineStandardTotal", null);
                    jObject.Add("ModuleLineStabdardOutput", null);
                    jObject.Add("ModuleLinePdf", false);
                    jObject.Add("ModuleLineImg", false);
                    break;
            }
            return jObject;
        }

        /// <summary>
        /// 显示全部版本工序平衡表
        /// </summary>
        /// <param name="type">型号</param>
        /// <param name="PCBNumber">PCB版号</param>
        /// <param name="Section">工段</param>
        /// <param name="platform">平台</param>
        /// <param name="title">标题</param>
        /// <returns></returns>
        public ActionResult DisplayALL_ProcessBalance(string type, string PCBNumber, string Section, string platform, string title, string Maintenance)
        {
            var infoList = db.ProcessBalance.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Section == Section && c.Platform == platform && c.Maintenance == Maintenance &&
            (c.Title == title || c.ProcessDescription == title)).Select(c => c.SerialNumber).Distinct().ToList();//查找所有符合筛选条件的版本号集合

            JArray result = new JArray();
            foreach (var item in infoList)//循环版本号
            {
                var info = db.ProcessBalance.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Section == Section && c.Maintenance == Maintenance && c.Platform == platform && (c.Title == title || c.ProcessDescription == title) && c.SerialNumber == item).FirstOrDefault();//找到对应版本号的内容

                JObject number = new JObject();
                number.Add("number", item);
                number.Add("Section", info.Section);//工段
                number.Add("StandardTotal", info.StandardTotal);//标准总人数
                number.Add("BalanceRate", info.BalanceRate);//平衡率
                number.Add("Bottleneck", info.Bottleneck);//瓶颈
                number.Add("StandardOutput", info.StandardOutput);//标准产量l
                number.Add("StandardHourlyOutputPerCapita", info.StandardHourlyOutputPerCapita);//标准人均时产量
                number.Add("ProductWorkingHours", info.ProductWorkingHours);//产品工时
                number.Add("Title", info.Title);//标题
                JArray line = new JArray();
                string[] name = info.ProcessName.Split('☆');//以☆进行分割
                string[] person = info.StandarPersondNumber.Split('☆');
                string[] workingTime = info.StandardNumber.Split('☆');
                string[] itemWarkTime = info.UnioTime.Split('☆');
                string[] jiaName = info.JigName.Split('☆');
                string[] machineTime = info.MachineTime.Split('☆');
                string[] machineNum = info.MachineNumber.Split('☆');
                string[] jiaNum = info.JigNum.Split('☆');
                string[] remark = info.Remark.Split('☆');
                for (int i = 0; i < name.Count(); i++)//循环分割的数据,将每行的数据读取出来
                {
                    JObject itemjobject = new JObject();
                    itemjobject.Add("ProcessName", name[i]); //工序名称
                    itemjobject.Add("StandarPersondNumber", person[i] == "" ? null : person[i]);//标注人数
                    itemjobject.Add("StandardNumber", workingTime[i] == "" ? null : workingTime[i]);//标准工时
                    itemjobject.Add("UnioTime", itemWarkTime[i] == "" ? null : itemWarkTime[i]);//单位工时
                    itemjobject.Add("JigName", jiaName[i] == "" ? null : jiaName[i]);//夹具名称
                    itemjobject.Add("MachineTime", machineTime[i] == "" ? null : machineTime[i]);//机器时间
                    itemjobject.Add("MachineNumber", machineNum[i] == "" ? null : machineNum[i]);//机器数量
                    itemjobject.Add("JigNum", jiaNum[i] == "" ? null : jiaNum[i]);//夹具个数
                    itemjobject.Add("Remark", remark[i] == "" ? null : remark[i]);//备注
                    line.Add(itemjobject);
                }

                number.Add("List", line);
                number.Add("RevisionContent", info.RevisionContent);//修订内容
                number.Add("MakingPeople", info.MakingPeople);//制定人
                number.Add("MakingTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", info.MakingTime));//制定时间
                number.Add("IsPassExaminan", info.IsPassExaminan);//审核是否通过
                number.Add("ExaminanPeople", info.ExaminanPeople);//审核人
                number.Add("ExaminanTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", info.ExaminanTime));//审核日期
                number.Add("IsPassApprover", info.IsPassApprover);//批准是否通过
                number.Add("ApproverPeople", info.ApproverPeople);//批准人
                number.Add("ApproverTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", info.ApproverTime));//批准日趋
                number.Add("IsPassControlled", info.IsPassControlled);//受控是否通过
                number.Add("ControlledPeople", info.ControlledPeople);//受控人
                number.Add("ControlledTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", info.ControlledTime));//受控日期

                result.Add(number);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 删除工序平衡表
        /// </summary>
        /// <param name="type">型号</param>
        /// <param name="PCBNumber">PCB版号</param>
        /// <param name="Section">工段</param>
        /// <param name="platform">平台</param>
        /// <param name="title">标题</param>
        /// <returns></returns>
        public ActionResult Delete_ProcessBalance(string type, string PCBNumber, string Section, string platform, string title, string Maintenance)
        {
            if (Session["User"] == null)//判断是否登录,没有登录跳到登录界面
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
            }
            //var item = db.ProcessBalance.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Section == Section && c.Platform == platform && c.Title == title).Max(c => c.SerialNumber);//查找符合条件的最大版本号

            var info = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Section == Section && c.Platform == platform && c.Title == title && c.Maintenance == Maintenance).FirstOrDefault();//找到最大版本号的平衡表信息

            if (info == null)
            {
                info = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Section == Section && c.Platform == platform && c.ProcessDescription == title && c.Maintenance == Maintenance).FirstOrDefault();//找到最大版本号的平衡表信息
            }
            //填写日志
            UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "删除工序产能数据，工段" + Section + "，型号，PCB编号,平台,标题为" + type + "，" + PCBNumber + "，" + platform + "," + title };
            db.UserOperateLog.Add(log);
            //删除设备表
            var quiment = db.Process_Capacity_Equipment.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Maintenance == Maintenance && c.Platform == platform && c.Seaction == Section && c.SeactionID == info.Id).ToList();
            db.Process_Capacity_Equipment.RemoveRange(quiment);
            //删除关联
            List<Process_Capacity_Relevance> result = new List<Process_Capacity_Relevance>();
            var relevance2 = db.Process_Capacity_Relevance.Where(c => c.Type == type && c.Platform == platform && c.Maintenance == Maintenance && c.PlatformModul == info.PlatformModul && c.ChildSeaction == Section && c.ChildID == info.Id).ToList();
            result.AddRange(relevance2);
            var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == type && c.Platform == platform && c.Maintenance == Maintenance && c.PlatformModul == info.PlatformModul && c.FatherSeaction == Section && c.FatherID == info.Id).ToList();
            if (relevance.Count != 0)
            {
                foreach (var item in relevance)
                {
                    result.AddRange(GetRelevanceList(item));
                    result.AddRange(relevance);
                }
            }
            db.Process_Capacity_Relevance.RemoveRange(result);

            JObject number = new JObject();
            if (info.ExaminanPeople != "" && info.ExaminanTime != null)//如果审核人员不为空,判断审核人员是否为删除人员,是能删,否则不能删
            {
                //if (((Users)Session["User"]).UserName == info.ExaminanPeople || ((Users)Session["User"]).Role == "系统管理员")
                //{
                db.ProcessBalance.Remove(info);
                db.SaveChanges();
                number.Add("message", true);
                number.Add("content", "删除成功");
                // }
                //else
                //{
                //    number.Add("message", false);
                //    number.Add("content", "删除失败，此表已审核，需审核人员删除");
                //}
            }
            else//没有审核人员,可以删
            {
                db.ProcessBalance.Remove(info);
                db.SaveChanges();
                number.Add("message", true);
                number.Add("content", "删除成功");
            }

            return Content(JsonConvert.SerializeObject(number));
        }

        /// <summary>
        /// 审核人签名或批准人签名
        /// </summary>
        /// <param name="type">型号</param>
        /// <param name="PCBNumber">PCB版本号</param>
        /// <param name="Section">工段</param>
        /// <param name="number">编号</param>
        /// <param name="status">审核或批准</param>
        public bool Autograph_ProcessBalance(string type, string PCBNumber, string Section, string number, string status, bool isPass)
        {
            if (Session["User"] == null)//判断是否有登录
            {
                return false;
            }
            var info = db.ProcessBalance.Where(c => c.Type == type && c.ProductPCBnumber == PCBNumber && c.Section == Section && c.SerialNumber == number).ToList();//查找符合条件的数据
            if (status == "审核")
            {
                info.ForEach(c => { c.ExaminanPeople = ((Users)Session["User"]).UserName; c.ExaminanTime = DateTime.Now; c.IsPassExaminan = isPass; });//修改内容
                db.SaveChanges();
                return true;
            }
            if (status == "批准")
            {
                info.ForEach(c => { c.ApproverPeople = ((Users)Session["User"]).UserName; c.ApproverTime = DateTime.Now; c.IsPassApprover = isPass; });//修改内容
                db.SaveChanges();
                return true;
            }
            if (status == "受控")
            {
                info.ForEach(c => { c.ControlledPeople = ((Users)Session["User"]).UserName; c.ControlledTime = DateTime.Now; c.IsPassControlled = isPass; });//修改内容
                db.SaveChanges();
                return true;
            }
            return false;
        }


        /// <summary>
        /// 复制平衡卡记录到新平台
        /// </summary>
        /// <param name="List<int> list">被复制平衡卡记录的Id</param>
        /// <param name="type">型号</param>
        /// <param name="platform">平台</param>
        /// <param name="productPCBnumber">产品PCB编号</param>
        /// <param name="platformModul">平台模块</param>
        /// <param name="maintenance">维护方式</param>
        [HttpPost]
        public ActionResult ProcessBalanceTableCopeToNewPlatform(List<int> list, string type, string platform, string productPCBnumber, string platformModul, string maintenance)
        {
            List<ProcessBalance> newrecordlist = new List<ProcessBalance>();
            JObject result = new JObject();
            if (db.Process_Capacity_Total.Count(c => c.Type == type && c.Platform == platform && c.ProductPCBnumber == productPCBnumber && c.PlatformModul == platformModul && c.Maintenance == maintenance) == 0)
            {
                result.Add("message", "没有找到平台类型");
                result.Add("isok", false);
                return Content(JsonConvert.SerializeObject(result));
            }
            List<ProcessBalance> recordlist = db.ProcessBalance.Where(c => list.Contains(c.Id)).ToList();
            newrecordlist.AddRange(recordlist);
            foreach (var item in newrecordlist)
            {
                ProcessBalance record = item;
                item.Type = type;
                item.Platform = platform;
                item.ProductPCBnumber = productPCBnumber;
                item.PlatformModul = platformModul;
                item.Maintenance = maintenance;
            }
            db.ProcessBalance.AddRange(newrecordlist);
            int count = db.SaveChanges();
            var resutstring = list.Count == count ? "选择了" + list.Count + "个记录，已全部复制到新平台。" : "选择了" + list.Count + "个记录，已复制了" + count + "个记录到新平台，有" + (list.Count - count) + "个记录未复制。";
            result.Add("message", resutstring);
            result.Add("isok", true);
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 复制工序记录到新平台
        /// <summary>
        /// 复制平衡卡记录到新平台
        /// </summary>
        /// <param name="sectionName">工序名(插件、喷墨、灌胶、气密、老化)</param>
        /// <param name="List<int> list">被复制平衡卡记录的Id</param>
        /// <param name="type">型号</param>
        /// <param name="platform">平台</param>
        /// <param name="productPCBnumber">产品PCB编号</param>
        /// <param name="platformModul">平台模块</param>
        /// <param name="maintenance">维护方式</param>
        [HttpPost]
        public ActionResult ProcessPartToNewPlatform(List<int> list, string type, string platform, string productPCBnumber, string platformModul, string maintenance)
        {
            int count = 0;
            //switch (sectionName)
            //{
            // case "插件":
            JObject result = new JObject();
            if (db.Process_Capacity_Total.Count(c => c.Type == type && c.Platform == platform && c.ProductPCBnumber == productPCBnumber && c.PlatformModul == platformModul && c.Maintenance == maintenance) == 0)
            {
                result.Add("message", "没有找到平台类型");
                result.Add("isok", false);
                return Content(JsonConvert.SerializeObject(result));
            }
            var recordlist1 = db.Process_Capacity_Manual.Where(c => list.Contains(c.Id)).ToList();
            var sectionName = recordlist1.FirstOrDefault().Section;
            foreach (var item in recordlist1)
            {
                item.Type = type;
                item.Platform = platform;
                item.ProductPCBnumber = productPCBnumber;
                item.PlatformModul = platformModul;
                item.Maintenance = maintenance;
            }
            db.Process_Capacity_Manual.AddRange(recordlist1);
            count = db.SaveChanges();
            //    break;
            //case "喷墨":
            //    var recordlist2 = db.Process_Capacity_Inkjet.Where(c => list.Contains(c.Id)).ToList();
            //    foreach (var item in recordlist2)
            //    {
            //        item.Type = type;
            //        item.Platform = platform;
            //        item.ProductPCBnumber = productPCBnumber;
            //        item.PlatformModul = platformModul;
            //        item.Maintenance = maintenance;
            //    }
            //    db.Process_Capacity_Inkjet.AddRange(recordlist2);
            //    result = db.SaveChanges();
            //    break;
            //case "灌胶":
            //    var recordlist3 = db.Process_Capacity_Glue.Where(c => list.Contains(c.Id)).ToList();
            //    foreach (var item in recordlist3)
            //    {
            //        item.Type = type;
            //        item.Platform = platform;
            //        item.ProductPCBnumber = productPCBnumber;
            //        item.PlatformModul = platformModul;
            //        item.Maintenance = maintenance;
            //    }
            //    db.Process_Capacity_Glue.AddRange(recordlist3);
            //    result = db.SaveChanges();
            //    break;
            //case "气密":
            //    var recordlist4 = db.Process_Capacity_Airtight.Where(c => list.Contains(c.Id)).ToList();
            //    foreach (var item in recordlist4)
            //    {
            //        item.Type = type;
            //        item.Platform = platform;
            //        item.ProductPCBnumber = productPCBnumber;
            //        item.PlatformModul = platformModul;
            //        item.Maintenance = maintenance;
            //    }
            //    db.Process_Capacity_Airtight.AddRange(recordlist4);
            //    result = db.SaveChanges();
            //    break;
            //case "老化":
            //    var recordlist5 = db.Process_Capacity_Burin.Where(c => list.Contains(c.Id)).ToList();
            //    foreach (var item in recordlist5)
            //    {
            //        item.Type = type;
            //        item.Platform = platform;
            //        item.ProductPCBnumber = productPCBnumber;
            //        item.PlatformModul = platformModul;
            //        item.Maintenance = maintenance;
            //    }
            //    db.Process_Capacity_Burin.AddRange(recordlist5);
            //    result = db.SaveChanges();
            //    break;
            //}
            string resutstring = "";
            resutstring = list.Count == count ? "选择了" + list.Count + "个" + sectionName + "记录，已全部复制到新平台。" : "选择了" + list.Count + "个记录，已复制了" + count + "个记录到新平台，有" + (list.Count - count) + "个记录未复制。";
            result.Add("message", resutstring);
            result.Add("isok", true);
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion


        #region 图片PDF相关操作

        //上传图片或PDF
        public bool UploadProcess_Capacity(int id, string seaction, string processName)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadProcess_Capacity"];//获取上传内容
                var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();
                //创建总目录
                if (Directory.Exists(@"D:\MES_Data\Process_Capacity\" + id.ToString()) == false)
                {
                    var info = db.Process_Capacity_Total.Find(id);
                    if (Directory.Exists(@"D:\MES_Data\Process_Capacity\") == false)//如果不存在就创建订单文件夹
                    {
                        Directory.CreateDirectory(@"D:\MES_Data\Process_Capacity\");
                    }
                    System.IO.File.AppendAllText(@"D:\MES_Data\Process_Capacity\directory.txt", "\n id:" + id + "对应的是--------型号:" + info.Type + ",平台:" + info.Platform + ",Pcb编号:" + info.ProductPCBnumber + "\n");
                }
                if (Directory.Exists(@"D:\MES_Data\Process_Capacity\" + id.ToString() + "\\" + seaction + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\Process_Capacity\" + id.ToString() + "\\" + seaction + "\\");
                }
                processName = processName.Replace("*", "X");//替换字符串,Windows系统文件夹命名不允许有*
                processName = processName.Replace("/", "々");//替换字符串,Windows系统文件夹命名不允许有/
                if (fileType == ".pdf")
                {
                    file.SaveAs(@"D:\MES_Data\Process_Capacity\" + id.ToString() + "\\" + seaction + "\\" + processName + ".pdf");
                }
                else
                {
                    List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\Process_Capacity\" + id.ToString() + "\\" + seaction + "\\");
                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(processName) && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                    file.SaveAs(@"D:\MES_Data\Process_Capacity\" + id.ToString() + "\\" + seaction + "\\" + processName + (jpg_count + 1) + fileType);
                }
                return true;
            }
            return false;
        }

        //删除图片或者PDF
        public ActionResult DeleteProcess_Capacity(int id, string seaction, string processName)
        {

            //if (Directory.Exists(@"D:\MES_Data\Process_Capacity\" + platform + "_" + type + "\\") == false)//如果不存在就创建订单文件夹
            //{
            //    Directory.CreateDirectory(@"D:\MES_Data\Process_Capacity\" + platform + "_" + type + "\\");
            //}
            //List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\Process_Capacity\" + platform + "_" + type + "\\");

            //int jpg_count = fileInfos.Where(c => c.Name.StartsWith(seaction) && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
            //file.SaveAs(@"D:\MES_Data\AssembleAbnormalOrder_Files\" + platform + "_" + type + "_" + seaction + (jpg_count + 1) + fileType);

            return View();
        }
        //查看图片 
        public ActionResult DisplayImg(int id, string seaction, string processName)
        {
            processName = processName.Replace("*", "X");//替换字符串,Windows系统文件夹命名不允许有*
            processName = processName.Replace("/", "々");//替换字符串,Windows系统文件夹命名不允许有/
            List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\Process_Capacity\" + id.ToString() + "\\" + seaction + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(processName) && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
            JArray json = new JArray();
            JObject result = new JObject();
            if (filesInfo.Count() > 0)
            {
                foreach (var item in filesInfo)
                {
                    string path = @"/MES_Data/Process_Capacity/" + id.ToString() + "/" + seaction + "/" + item.Name;
                    json.Add(path);
                }
                result.Add("have", true);
                result.Add("path", json);
                return Content(JsonConvert.SerializeObject(result));
            }
            else
            {
                result.Add("have", false);
                result.Add("path", null);
                return Content(JsonConvert.SerializeObject(result));
            }
        }

        //查看PDF
        public ActionResult DisplayPdf(int id, string seaction, string processName)
        {
            processName = processName.Replace("*", "X");
            processName = processName.Replace("/", "々");
            List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\Process_Capacity\" + id.ToString() + "\\" + seaction + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(processName) && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();
            JArray json = new JArray();
            JObject result = new JObject();
            if (filesInfo.Count() > 0)
            {
                string path = @"/MES_Data/Process_Capacity/" + id.ToString() + "/" + seaction + "/" + processName + ".pdf";
                result.Add("have", true);
                result.Add("path", path);
                return Content(JsonConvert.SerializeObject(result));
            }
            else
            {
                result.Add("have", false);
                result.Add("path", null);
                return Content(JsonConvert.SerializeObject(result));
            }
        }

        //是否有pdf文件
        public bool IsHavingPDF(int id, string seaction, string processName)
        {
            if (!string.IsNullOrEmpty(processName))
            {
                processName = processName.Replace("*", "X");
                processName = processName.Replace("/", "々");
            }
            List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\Process_Capacity\" + id.ToString() + "\\" + seaction + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(processName) && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();
            if (filesInfo.Count > 0)
                return true;
            else
                return false;
        }

        //是否有img文件
        public bool IsHavingIMG(int id, string seaction, string processName)
        {
            if (!string.IsNullOrEmpty(processName))
            {
                processName = processName.Replace("*", "X");
                processName = processName.Replace("/", "々");
            }
            List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\Process_Capacity\" + id.ToString() + "\\" + seaction + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(processName) && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
            if (filesInfo.Count > 0)
                return true;
            else
                return false;
        }
        #endregion

        //日期转换
        private DateTime TimeChange(string value)
        {
            if (value.Contains("/"))
            {
                var time = value.Split('/');
                if (time[0].Length == 4)
                {
                    return new DateTime(Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), Convert.ToInt32(time[2]));
                }
                else
                {
                    var year = "";
                    if (time[2].Length <= 2)
                    {
                        year = "20" + time[2];
                    }
                    else
                    {
                        year = time[2];
                    }
                    return new DateTime(Convert.ToInt32(year), Convert.ToInt32(time[0]), Convert.ToInt32(time[1]));
                }
            }
            else if (value.Contains("-") && !value.Contains("月"))
            {
                var time = value.Split('-');
                if (time[0].Length == 4)
                {
                    return new DateTime(Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), Convert.ToInt32(time[2]));
                }
                var year = "";
                if (time[2].Length <= 2)
                {
                    year = "20" + time[2];
                }
                else
                {
                    year = time[2];
                }
                return new DateTime(Convert.ToInt32(year), Convert.ToInt32(time[0]), Convert.ToInt32(time[1]));
            }
            else if (value.Contains("-") && value.Contains("月"))
            {
                var time = value.Split('-');
                return new DateTime(Convert.ToInt32(time[2]), DataTypeChange.ChineseMonthChangeInt(time[1]), Convert.ToInt32(time[0]));
            }
            else
            {
                DateTime startTime = new DateTime(1900, 1, 1);
                return startTime.AddDays(Convert.ToInt32(value) - 2);
            }
        }

        #region 以前显示页面


        /// <summary>
        ///  插件添加,修改,删除
        /// </summary>
        /// <param name="newData">数据</param>
        /// <param name="statu">要操作的状态</param>
        /// <returns></returns>
        public ActionResult Add_Plug_In(Process_Capacity_Plugin newData, string statu)
        {
            if (Session["User"] == null)//判断是否有登录,如果没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
            }
            JObject value = new JObject();
            if (!ModelState.IsValid)//判断 Process_Capacity_Plugin 数据格式
            {
                value.Add("message", false);
                value.Add("content", "格式错误");
                return Content(JsonConvert.SerializeObject(value));
            }
            if (statu == "添加")
            {
                newData.SingleLampWorkingHours = 0.222;//写死的
                newData.PluginStandardCapacity = newData.PCBASingleLampNumber == 0 ? 0 : (decimal)(3600 / (newData.PCBASingleLampNumber * 0.222));//计算标准产能 3600/单灯数*0.22
                newData.Operator = ((Users)Session["User"]).UserName;//登录姓名
                newData.OperateDT = DateTime.Now;//现在时间
                db.Process_Capacity_Plugin.Add(newData);
                db.SaveChanges();//保存数据

                value.Add("message", true);
                var pluginitem = db.Process_Capacity_Plugin.OrderByDescending(c => c.Id).Where(c => c.Type == newData.Type && c.ProductPCBnumber == newData.ProductPCBnumber && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance).FirstOrDefault();//查找刚刚新增的数据
                JObject pluginjobject = new JObject();
                pluginjobject.Add("PluginEidt", false);//前端用
                pluginjobject.Add("PluginDeviceID", pluginitem.Id);
                pluginjobject.Add("PluginDeviceName", pluginitem.Name); //插件工序名
                pluginjobject.Add("SingleLampWorkingHours", pluginitem.SingleLampWorkingHours);//插件机台固定标准单灯工时
                pluginjobject.Add("PCBASingleLampNumber", pluginitem.PCBASingleLampNumber);//插件PCBA单灯数
                pluginjobject.Add("PluginStandardNumber", pluginitem.PluginStandardNumber);//插件标配人数
                pluginjobject.Add("PluginStandardCapacity", pluginitem.PluginStandardCapacity);//插件产能标准
                                                                                               // pluginjobject.Add("PluginModuleNeed", pluginitem.ModuleNeedNum2);//插件产能标准
                value.Add("content", pluginjobject);
                return Content(JsonConvert.SerializeObject(value));//返回给前面显示,实现不刷新就显示
            }
            else if (statu == "修改")
            {
                var old = db.Process_Capacity_Plugin.Where(c => c.Id == newData.Id).FirstOrDefault();//查找要修改的数据
                                                                                                     //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "修改工序产能数据，工段插件，描述" + old.Name + "->" + newData.Name + "，单灯数" + old.PCBASingleLampNumber + "->" + newData.PCBASingleLampNumber + "，人数" + old.PluginStandardNumber + "->" + newData.PluginStandardNumber + ",模组需求数量" + old.ModuleNeedNum2 + "->" + newData.ModuleNeedNum2 };
                // RemoveHoldingEntityInContext(newData);
                // db.Process_Capacity_Plugin.Attach(newData);

                db.UserOperateLog.Add(log);
                //修改值
                old.SingleLampWorkingHours = 0.222;
                old.PluginStandardCapacity = newData.PCBASingleLampNumber == 0 ? 0 : (decimal)(3600 / newData.PCBASingleLampNumber * 0.222);
                old.Name = newData.Name;
                old.PCBASingleLampNumber = newData.PCBASingleLampNumber;
                old.PluginStandardNumber = newData.PluginStandardNumber;
                //old.ModuleNeedNum2 = newData.ModuleNeedNum2;
                db.SaveChanges();//保存数据

                value.Add("message", true);
                value.Add("content", "修改成功");
                return Content(JsonConvert.SerializeObject(value));

            }
            else if (statu == "删除")
            {
                //查找要删除的数据
                var old = db.Process_Capacity_Plugin.Find(newData.Id);
                //删除关联
                List<Process_Capacity_Relevance> result = new List<Process_Capacity_Relevance>();
                var relevance2 = db.Process_Capacity_Relevance.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.ChildSeaction == "插件" && c.ChildID == newData.Id).ToList();
                result.AddRange(relevance2);
                var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.FatherSeaction == "插件" && c.FatherID == newData.Id).ToList();
                if (relevance.Count != 0)
                {
                    foreach (var item in relevance)
                    {
                        result.AddRange(GetRelevanceList(item));
                        result.AddRange(relevance);
                    }
                }
                db.Process_Capacity_Relevance.RemoveRange(result);
                //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "删除工序产能数据，工段插件，原数据,描述，单灯数，人数为" + old.Name + "，" + old.PCBASingleLampNumber + "，" + old.PluginStandardNumber };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                var delete = db.Process_Capacity_Plugin.Find(newData.Id);
                db.Process_Capacity_Plugin.Remove(delete);
                db.SaveChanges();//保存数据
                value.Add("message", true);
                value.Add("content", "删除成功");
                return Content(JsonConvert.SerializeObject(value));
            }

            return Content("true");
        }

        /// <summary>
        /// 三防添加,修改删除
        /// </summary>
        /// <param name="newData">数据</param>
        /// <param name="statu">操作状态</param>
        /// <returns></returns>
        public ActionResult Add_ThreeProf(Process_Capacity_ThreeProf newData, string statu)
        {
            if (Session["User"] == null)//判断是否有登录,如果没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
            }

            JObject value = new JObject();
            if (!ModelState.IsValid)//判断格式
            {
                value.Add("message", false);
                value.Add("content", "格式错误");
                return Content(JsonConvert.SerializeObject(value));
            }
            if (statu == "添加")
            {
                newData.Operator = ((Users)Session["User"]).UserName;//登录名
                newData.OperateDT = DateTime.Now;
                db.Process_Capacity_ThreeProf.Add(newData);
                db.SaveChanges(); //新增数据
                value.Add("message", true);
                var threeprofitem = db.Process_Capacity_ThreeProf.OrderByDescending(c => c.Id).Where(c => c.Type == newData.Type && c.ProductPCBnumber == newData.ProductPCBnumber && c.Platform == newData.Platform).FirstOrDefault();//查找刚刚新增的数据,
                JObject jObject = new JObject();
                jObject.Add("ThreeProfEidt", false);//前端用
                jObject.Add("ThreeProfID", threeprofitem.Id);
                jObject.Add("ThreeProfProcessName", threeprofitem.ThreeProfProcessName);//三防工序描述
                jObject.Add("ThreeProfStandardTotal", threeprofitem.ThreeProfStandardTotal);//三防标准总人数
                jObject.Add("ThreeProfStabdardOutput", threeprofitem.ThreeProfStabdardOutput);//三防标准产量
                jObject.Add("ThreeProfModuleNeed", threeprofitem.ModuleNeedNum2);//三防模组需求数量
                value.Add("content", jObject);
                return Content(JsonConvert.SerializeObject(value));//数据返回前端,实现不刷新显示
            }
            else if (statu == "修改")
            {
                var old = db.Process_Capacity_ThreeProf.Where(c => c.Id == newData.Id).FirstOrDefault();//查找需要修改的数据
                                                                                                        //填写日志
                UserOperateLog log = new UserOperateLog()
                {
                    Operator = ((Users)Session["User"]).UserName,
                    OperateDT = DateTime.Now,
                    OperateRecord = "修改工序产能数据，工段三防，描述" + old.ThreeProfProcessName + "->" + newData.ThreeProfProcessName + "，总人数" + old.ThreeProfStandardTotal + "->" + newData.ThreeProfStandardTotal + "，标准产量" + old.ThreeProfStabdardOutput + "->" + newData.ThreeProfStabdardOutput + ",模组需求数量" + old.ModuleNeedNum2 + "->" + newData.ModuleNeedNum2
                };

                db.UserOperateLog.Add(log);
                //将新的值赋过去
                old.ThreeProfProcessName = newData.ThreeProfProcessName;
                old.ThreeProfStandardTotal = newData.ThreeProfStandardTotal;
                old.ThreeProfStabdardOutput = newData.ThreeProfStabdardOutput;
                old.ModuleNeedNum2 = newData.ModuleNeedNum2;

                db.SaveChanges();
                value.Add("message", true);
                value.Add("content", "修改成功");
                return Content(JsonConvert.SerializeObject(value));
            }
            else if (statu == "删除")
            {
                var delete = db.Process_Capacity_ThreeProf.Find(newData.Id);//查找需要删除的数据
                                                                            //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "删除工序产能数据，工段三防，原数据,描述，总人数，标准产量为" + delete.ThreeProfProcessName + "，" + delete.ThreeProfStandardTotal + "，" + delete.ThreeProfStabdardOutput };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                db.Process_Capacity_ThreeProf.Remove(delete);
                db.SaveChanges();
                value.Add("message", true);
                value.Add("content", "删除成功");
                return Content(JsonConvert.SerializeObject(value));
            }

            return Content("true");
        }
        /// <summary>
        /// 喷墨添加,修改,删除
        /// </summary>
        /// <param name="newData">数据</param>
        /// <param name="statu">操作状态</param>
        /// <returns></returns>
        public ActionResult Add_Inkjet(Process_Capacity_Inkjet newData, string statu)
        {
            if (Session["User"] == null)//判断是否有登录,如果没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
            }

            JObject value = new JObject();
            if (!ModelState.IsValid)
            {
                value.Add("message", false);
                value.Add("content", "格式错误");
                return Content(JsonConvert.SerializeObject(value));
            }
            if (statu == "添加")
            {
                newData.Operator = ((Users)Session["User"]).UserName;
                newData.OperateDT = DateTime.Now;
                db.Process_Capacity_Inkjet.Add(newData);
                db.SaveChanges();

                value.Add("message", true);
                var inkjetitem = db.Process_Capacity_Inkjet.OrderByDescending(c => c.Id).Where(c => c.Type == newData.Type && c.ProductPCBnumber == newData.ProductPCBnumber && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance).FirstOrDefault();//查找刚刚新增的数据,
                JObject jObject = new JObject();
                jObject.Add("InkjetEidt", false);//前端用
                jObject.Add("InkjetID", inkjetitem.Id);
                jObject.Add("InkjetProcessName", inkjetitem.InkjetProcessName);//喷墨工序
                jObject.Add("InkjetSuctionStandardTotal", inkjetitem.InkjetSuctionStandardTotal);//喷墨配置人数
                jObject.Add("InkjetStabdardOutputPerHour", inkjetitem.InkjetStabdardOutputPerHour);//喷墨每小时产能
                                                                                                   //jObject.Add("InkjetModuleNeed", inkjetitem.ModuleNeedNum2);//喷墨模组需求数量
                value.Add("content", jObject);
                return Content(JsonConvert.SerializeObject(value));//数据返回前端,实现不刷新显示
            }
            else if (statu == "修改")
            {
                var old = db.Process_Capacity_Inkjet.Where(c => c.Id == newData.Id).FirstOrDefault();//查找需要修改的数据
                                                                                                     //填写日志
                UserOperateLog log = new UserOperateLog()
                {
                    Operator = ((Users)Session["User"]).UserName,
                    OperateDT = DateTime.Now,
                    OperateRecord = "修改工序产能数据，工段喷墨，描述" + old.InkjetProcessName + "->" + newData.InkjetProcessName + "，总人数" + old.InkjetSuctionStandardTotal + "->" + newData.InkjetSuctionStandardTotal + "，标准产量" + old.InkjetStabdardOutputPerHour + "->" + newData.InkjetStabdardOutputPerHour + ",模组需求数量" + old.ModuleNeedNum2 + "->" + newData.ModuleNeedNum2
                };

                db.UserOperateLog.Add(log);
                //将新的值赋过去
                old.InkjetProcessName = newData.InkjetProcessName;
                old.InkjetSuctionStandardTotal = newData.InkjetSuctionStandardTotal;
                old.InkjetStabdardOutputPerHour = newData.InkjetStabdardOutputPerHour;
                // old.ModuleNeedNum2 = newData.ModuleNeedNum2;

                db.SaveChanges();
                value.Add("message", true);
                value.Add("content", "修改成功");
                return Content(JsonConvert.SerializeObject(value));
            }
            else if (statu == "删除")
            {
                var delete = db.Process_Capacity_Inkjet.Find(newData.Id);//查找需要删除的数据
                                                                         //删除关联
                List<Process_Capacity_Relevance> result = new List<Process_Capacity_Relevance>();
                var relevance2 = db.Process_Capacity_Relevance.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.ChildSeaction == "喷墨" && c.ChildID == newData.Id).ToList();
                result.AddRange(relevance2);
                var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.FatherSeaction == "喷墨" && c.FatherID == newData.Id).ToList();
                if (relevance.Count != 0)
                {
                    foreach (var item in relevance)
                    {
                        result.AddRange(GetRelevanceList(item));
                        result.AddRange(relevance);
                    }
                }
                db.Process_Capacity_Relevance.RemoveRange(result);

                //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "删除工序产能数据，工段喷墨，原数据,描述，总人数，标准产量为" + delete.InkjetProcessName + "，" + delete.InkjetSuctionStandardTotal + "，" + delete.InkjetStabdardOutputPerHour };
                db.UserOperateLog.Add(log);
                db.Process_Capacity_Inkjet.Remove(delete);
                db.SaveChanges();
                value.Add("message", true);
                value.Add("content", "删除成功");
                return Content(JsonConvert.SerializeObject(value));

            }

            return Content("true");
        }

        /// <summary>
        /// 灌胶添加,修改,删除
        /// </summary>
        /// <param name="newData">数据</param>
        /// <param name="statu">操作状态</param>
        /// <returns></returns>
        public ActionResult Add_Glue(Process_Capacity_Glue newData, string statu)
        {
            if (Session["User"] == null)//判断是否有登录,如果没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
            }

            JObject value = new JObject();
            if (!ModelState.IsValid)
            {
                value.Add("message", false);
                value.Add("content", "格式错误");
                return Content(JsonConvert.SerializeObject(value));
            }
            if (statu == "添加")
            {
                newData.Operator = ((Users)Session["User"]).UserName;//查找需要修改的数据
                newData.OperateDT = DateTime.Now;
                db.Process_Capacity_Glue.Add(newData);
                db.SaveChanges();
                value.Add("message", true);
                var glueitem = db.Process_Capacity_Glue.OrderByDescending(c => c.Id).Where(c => c.Type == newData.Type && c.ProductPCBnumber == newData.ProductPCBnumber && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance).FirstOrDefault();//查找刚刚新增的数据,
                JObject jObject = new JObject();
                jObject.Add("GlueEidt", false);//前端用
                jObject.Add("GlueID", glueitem.Id);
                jObject.Add("GlueProcessName", glueitem.GlueProcessName);//灌胶工序描述
                jObject.Add("GlueStandardTotal", glueitem.GlueStandardTotal);//灌胶标准总人数
                jObject.Add("GlueStabdardOutput", glueitem.GlueStabdardOutput);//灌胶标准产量
                                                                               //jObject.Add("GlueModuleNeed", glueitem.ModuleNeedNum2);//灌胶模组需求数量
                value.Add("content", jObject);
                return Content(JsonConvert.SerializeObject(value));//数据返回前端,实现不刷新显示
            }
            else if (statu == "修改")
            {
                var old = db.Process_Capacity_Glue.Where(c => c.Id == newData.Id).FirstOrDefault();//查找需要修改的数据
                                                                                                   //填写日志
                UserOperateLog log = new UserOperateLog()
                {
                    Operator = ((Users)Session["User"]).UserName,
                    OperateDT = DateTime.Now,
                    OperateRecord = "修改工序产能数据，工段灌胶，描述" + old.GlueProcessName + "->" + newData.GlueProcessName + "，总人数" + old.GlueStandardTotal + "->" + newData.GlueStandardTotal + "，标准产量" + old.GlueStabdardOutput + "->" + newData.GlueStabdardOutput + ",模组需求数量" + old.ModuleNeedNum2 + "->" + newData.ModuleNeedNum2
                };

                db.UserOperateLog.Add(log);
                //将新的值赋过去
                old.GlueProcessName = newData.GlueProcessName;
                old.GlueStandardTotal = newData.GlueStandardTotal;
                old.GlueStabdardOutput = newData.GlueStabdardOutput;
                //old.ModuleNeedNum2 = newData.ModuleNeedNum2;
                db.SaveChanges();
                value.Add("message", true);
                value.Add("content", "修改成功");
                return Content(JsonConvert.SerializeObject(value));
            }
            else if (statu == "删除")
            {
                var delete = db.Process_Capacity_Glue.Find(newData.Id);//查找需要删除的数据
                                                                       //删除关联
                List<Process_Capacity_Relevance> result = new List<Process_Capacity_Relevance>();
                var relevance2 = db.Process_Capacity_Relevance.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.ChildSeaction == "灌胶" && c.ChildID == newData.Id).ToList();
                result.AddRange(relevance2);
                var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.FatherSeaction == "灌胶" && c.FatherID == newData.Id).ToList();
                if (relevance.Count != 0)
                {
                    foreach (var item in relevance)
                    {
                        result.AddRange(GetRelevanceList(item));
                        result.AddRange(relevance);
                    }
                }
                db.Process_Capacity_Relevance.RemoveRange(result);
                //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "删除工序产能数据，工段灌胶，原数据,描述，总人数，标准产量为" + delete.GlueProcessName + "，" + delete.GlueStandardTotal + "，" + delete.GlueStabdardOutput };
                db.UserOperateLog.Add(log);
                db.Process_Capacity_Glue.Remove(delete);
                db.SaveChanges();
                value.Add("message", true);
                value.Add("content", "删除成功");
                return Content(JsonConvert.SerializeObject(value));
            }
            return Content("true");
        }
        /// <summary>
        /// 气密添加,修改,删除
        /// </summary>
        /// <param name="newData">数据</param>
        /// <param name="statu">操作状态</param>
        /// <returns></returns>
        public ActionResult Add_Airtight(Process_Capacity_Airtight newData, string statu)
        {
            if (Session["User"] == null)//判断是否有登录,如果没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
            }
            JObject value = new JObject();
            if (!ModelState.IsValid)
            {
                value.Add("message", false);
                value.Add("content", "格式错误");
                return Content(JsonConvert.SerializeObject(value));
            }
            if (statu == "添加")
            {
                newData.Operator = ((Users)Session["User"]).UserName;
                newData.OperateDT = DateTime.Now;
                db.Process_Capacity_Airtight.Add(newData);
                db.SaveChanges();
                value.Add("message", true);
                var airtightitem = db.Process_Capacity_Airtight.OrderByDescending(c => c.Id).Where(c => c.Type == newData.Type && c.ProductPCBnumber == newData.ProductPCBnumber && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance).FirstOrDefault();//查找刚刚新增的数据,
                JObject jObject = new JObject();
                jObject.Add("AirtightEidt", false);//前端用
                jObject.Add("AirtightID", airtightitem.Id);
                jObject.Add("AirtightProcessName", airtightitem.AirtightProcessName);//气密工序描述
                jObject.Add("AirtightStandardTotal", airtightitem.AirtightStandardTotal);//气密标准总人数
                jObject.Add("AirtightStabdardOutput", airtightitem.AirtightStabdardOutput);//气密标准产量
                                                                                           //jObject.Add("AirtightModuleNeed", airtightitem.ModuleNeedNum2);//气密模组需求数量
                value.Add("content", jObject);
                return Content(JsonConvert.SerializeObject(value));//数据返回前端,实现不刷新显示
            }
            else if (statu == "修改")
            {
                var old = db.Process_Capacity_Airtight.Where(c => c.Id == newData.Id).FirstOrDefault();//查找需要修改的数据
                                                                                                       //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "修改工序产能数据，工段气密，描述" + old.AirtightProcessName + "->" + newData.AirtightProcessName + "，总人数" + old.AirtightStandardTotal + "->" + newData.AirtightStandardTotal + "，标准产量" + old.AirtightStabdardOutput + "->" + newData.AirtightStabdardOutput + ",模组需求数量" + old.ModuleNeedNum2 + "->" + newData.ModuleNeedNum2 };
                db.UserOperateLog.Add(log);
                //将新的值赋过去
                old.AirtightProcessName = newData.AirtightProcessName;
                old.AirtightStandardTotal = newData.AirtightStandardTotal;
                old.AirtightStabdardOutput = newData.AirtightStabdardOutput;
                //old.ModuleNeedNum2 = newData.ModuleNeedNum2;
                db.SaveChanges();
                value.Add("message", true);
                value.Add("content", "修改成功");
                return Content(JsonConvert.SerializeObject(value));
            }
            else if (statu == "删除")
            {
                var delete = db.Process_Capacity_Airtight.Find(newData.Id);//查找需要删除的数据

                //删除关联
                List<Process_Capacity_Relevance> result = new List<Process_Capacity_Relevance>();
                var relevance2 = db.Process_Capacity_Relevance.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.ChildSeaction == "气密" && c.ChildID == newData.Id).ToList();
                result.AddRange(relevance2);
                var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.FatherSeaction == "气密" && c.FatherID == newData.Id).ToList();
                if (relevance.Count != 0)
                {
                    foreach (var item in relevance)
                    {
                        result.AddRange(GetRelevanceList(item));
                        result.AddRange(relevance);
                    }
                }
                db.Process_Capacity_Relevance.RemoveRange(result);

                //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "删除工序产能数据，工段气密，原数据,描述，总人数，标准产量为" + delete.AirtightProcessName + "，" + delete.AirtightStandardTotal + "，" + delete.AirtightStabdardOutput };
                db.UserOperateLog.Add(log);
                db.Process_Capacity_Airtight.Remove(delete);
                db.SaveChanges();
                value.Add("message", true);
                value.Add("content", "删除成功");
                return Content(JsonConvert.SerializeObject(value));
            }
            return Content("true");
        }

        ////装磁吸安装板输入
        //public ActionResult Add_MagneticSuction(Process_Capacity_Magnetic newData, string statu)
        //{
        //    if (Session["User"] == null)
        //    {
        //        return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
        //    }
        //    JObject value = new JObject();
        //    if (!ModelState.IsValid)
        //    {
        //        value.Add("message", false);
        //        value.Add("content", "格式错误");
        //        return Content(JsonConvert.SerializeObject(value));
        //    }
        //    if (statu == "添加")
        //    {
        //        newData.MagneticSuctionStandardHourlyOutputPerCapita = newData.MagneticSuctionStandardTotal == 0 ? 0 : newData.MagneticSuctionStabdardOutput / newData.MagneticSuctionStandardTotal;
        //        newData.Operator = ((Users)Session["User"]).UserName;
        //        newData.OperateDT = DateTime.Now;
        //        db.Process_Capacity_Magnetic.Add(newData);
        //        db.SaveChanges();
        //        value.Add("message", true);
        //        var magneticitem = db.Process_Capacity_Magnetic.OrderByDescending(c => c.Id).Where(c => c.Type == newData.Type && c.ProductPCBnumber == newData.ProductPCBnumber && c.Platform == newData.Platform).FirstOrDefault();
        //        JObject jObject = new JObject();
        //        jObject.Add("MagneticEidt", false);//前端用
        //        jObject.Add("MagneticID", magneticitem.Id);
        //        jObject.Add("MagneticProcessName", magneticitem.MagneticSuctionProcessName);//装磁吸工序描述
        //        jObject.Add("MagneticSuctionStandardTotal", magneticitem.MagneticSuctionStandardTotal);//装磁吸标准总人数
        //        jObject.Add("MagneticSuctionStabdardOutput", magneticitem.MagneticSuctionStabdardOutput);//装磁吸标准产量
        //        jObject.Add("MagneticSuctionStandardHourlyOutputPerCapita", magneticitem.MagneticSuctionStandardHourlyOutputPerCapita);//装磁吸人均时产量
        //        value.Add("content", jObject);
        //        return Content(JsonConvert.SerializeObject(value));

        //    }
        //    else if (statu == "修改")
        //    {
        //        var old = db.Process_Capacity_Magnetic.Where(c => c.Id == newData.Id).FirstOrDefault();
        //        UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "修改工序产能数据，工段装磁吸，原数据,描述，总人数，标准产量为" + old.MagneticProcessName + "，" + old.MagneticSuctionStandardTotal + "，" + old.MagneticSuctionStabdardOutput + "修改为" + newData.MagneticSuctionProcessName + "，" + newData.MagneticSuctionStandardTotal + "，" + newData.MagneticSuctionStabdardOutput };

        //        db.UserOperateLog.Add(log);
        //        old.MagneticSuctionStandardHourlyOutputPerCapita = newData.MagneticSuctionStandardTotal == 0 ? 0 : newData.MagneticSuctionStabdardOutput / newData.MagneticSuctionStandardTotal;
        //        old.MagneticProcessName = newData.MagneticProcessName;
        //        old.MagneticSuctionStandardTotal = newData.MagneticSuctionStandardTotal;
        //        old.MagneticSuctionStabdardOutput = newData.MagneticSuctionStabdardOutput;
        //        db.SaveChanges();
        //        value.Add("message", true);
        //        value.Add("content", "修改成功");
        //        return Content(JsonConvert.SerializeObject(value));

        //    }
        //    else if (statu == "删除")
        //    {
        //        var delete = db.Process_Capacity_Magnetic.Find(newData.Id);
        //        UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "删除工序产能数据，工段装磁吸，原数据,描述，总人数，标准产量为" + delete.MagneticProcessName + "，" + delete.MagneticSuctionStandardTotal + "，" + delete.MagneticSuctionStabdardOutput };
        //        db.UserOperateLog.Add(log);
        //        db.Process_Capacity_Magnetic.Remove(delete);
        //        db.SaveChanges();
        //        value.Add("message", true);
        //        value.Add("content", "删除成功");
        //        return Content(JsonConvert.SerializeObject(value));
        //    }
        //    return Content("true");
        //}
        //老化输入
        /// <summary>
        /// 老化添加,修改,删除
        /// </summary>
        /// <param name="newData">数据</param>
        /// <param name="statu">操作状态</param>
        /// <returns></returns>
        public ActionResult Add_Burin(Process_Capacity_Burin newData, string statu)
        {
            if (Session["User"] == null)//判断是否有登录,如果没有登录则跳到登录页面
            {
                return RedirectToAction("Login", "Users", new { col = "Process_Capacity", act = "TotalProcess_Capacity" });
            }
            JObject value = new JObject();
            if (!ModelState.IsValid)
            {
                value.Add("message", false);
                value.Add("content", "格式错误");
                return Content(JsonConvert.SerializeObject(value));
            }
            if (statu == "添加")
            {
                newData.Operator = ((Users)Session["User"]).UserName;
                newData.OperateDT = DateTime.Now;
                db.Process_Capacity_Burin.Add(newData);
                db.SaveChanges();
                value.Add("message", true);
                var burninitem = db.Process_Capacity_Burin.OrderByDescending(c => c.Id).Where(c => c.Type == newData.Type && c.ProductPCBnumber == newData.ProductPCBnumber && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance).FirstOrDefault();//查找刚刚新增的数据,
                JObject jObject = new JObject();
                jObject.Add("BurinEidt", false);//前端用
                jObject.Add("BurinID", burninitem.Id);
                jObject.Add("BurinOneProcessName", burninitem.BurinOneProcessName);//老化工序描述1
                jObject.Add("BurninOneSuctionStandardTotal", burninitem.BurninOneSuctionStandardTotal);//老化1标配人数
                jObject.Add("BurinOneStabdardOutputPerHour", burninitem.BurinOneStabdardOutputPerHour);//老化1每小时产能
                jObject.Add("BurinTwoProcessName", burninitem.BurinTwoProcessName);//老化2工序描述
                jObject.Add("BurinTwoSuctionStandardTotal", burninitem.BurinTwoSuctionStandardTotal);//老化2标配人数
                jObject.Add("BurinTwoStabdardOutputPerHour", burninitem.BurinTwoStabdardOutputPerHour);//老化2每小时标准产能
                                                                                                       //jObject.Add("BurinModuleNeed", burninitem.ModuleNeedNum2);//老化模组需求数量
                value.Add("content", jObject);
                return Content(JsonConvert.SerializeObject(value));//数据返回前端,实现不刷新显示

            }
            else if (statu == "修改")
            {
                var old = db.Process_Capacity_Burin.Where(c => c.Id == newData.Id).FirstOrDefault();//查找需要修改的数据
                                                                                                    //填写日志
                UserOperateLog log = new UserOperateLog()
                {
                    Operator = ((Users)Session["User"]).UserName,
                    OperateDT = DateTime.Now,
                    OperateRecord = "修改工序产能数据，工段老化，老化1描述" + old.BurinOneProcessName + "->" + newData.BurinOneProcessName + "，总人数" + old.BurninOneSuctionStandardTotal + "->" + newData.BurninOneSuctionStandardTotal + "，标准产量" + old.BurinOneStabdardOutputPerHour + "->" + newData.BurinOneStabdardOutputPerHour + "老化2描述" + old.BurinTwoProcessName + "->" + newData.BurinTwoProcessName + "，总人数" + old.BurinTwoSuctionStandardTotal + "->" + newData.BurinTwoSuctionStandardTotal + "，标准产量" + old.BurinTwoStabdardOutputPerHour + "->" + newData.BurinTwoStabdardOutputPerHour + ",模组需求数量" + old.ModuleNeedNum2 + "->" + newData.ModuleNeedNum2
                };

                db.UserOperateLog.Add(log);
                //将新的值赋过去
                old.BurinOneProcessName = newData.BurinOneProcessName;
                old.BurninOneSuctionStandardTotal = newData.BurninOneSuctionStandardTotal;
                old.BurinOneStabdardOutputPerHour = newData.BurinOneStabdardOutputPerHour;
                old.BurinTwoProcessName = newData.BurinTwoProcessName;
                old.BurinTwoSuctionStandardTotal = newData.BurinTwoSuctionStandardTotal;
                old.BurinTwoStabdardOutputPerHour = newData.BurinTwoStabdardOutputPerHour;
                //old.ModuleNeedNum2 = newData.ModuleNeedNum2;

                db.SaveChanges();
                value.Add("message", true);
                value.Add("content", "修改成功");
                return Content(JsonConvert.SerializeObject(value));
            }
            else if (statu == "删除")
            {
                var delete = db.Process_Capacity_Burin.Find(newData.Id);//查找需要删除的数据
                                                                        //删除关联
                List<Process_Capacity_Relevance> result = new List<Process_Capacity_Relevance>();
                var relevance2 = db.Process_Capacity_Relevance.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.ChildSeaction == "老化" && c.ChildID == newData.Id).ToList();
                result.AddRange(relevance2);
                var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == newData.Type && c.Platform == newData.Platform && c.Maintenance == newData.Maintenance && c.PlatformModul == newData.PlatformModul && c.FatherSeaction == "老化" && c.FatherID == newData.Id).ToList();
                if (relevance.Count != 0)
                {
                    foreach (var item in relevance)
                    {
                        result.AddRange(GetRelevanceList(item));
                        result.AddRange(relevance);
                    }
                }
                db.Process_Capacity_Relevance.RemoveRange(result);

                //填写日志
                UserOperateLog log = new UserOperateLog() { Operator = ((Users)Session["User"]).UserName, OperateDT = DateTime.Now, OperateRecord = "删除工序产能数据，工段老化，原数据,老化1描述，总人数，标准产量，老化2描述，总人数，标准产量为" + delete.BurinOneProcessName + "，" + delete.BurninOneSuctionStandardTotal + "，" + delete.BurinOneStabdardOutputPerHour + "，" + delete.BurinTwoProcessName + "，" + delete.BurinTwoSuctionStandardTotal + "，" + delete.BurinTwoStabdardOutputPerHour };
                db.UserOperateLog.Add(log);
                db.Process_Capacity_Burin.Remove(delete);
                db.SaveChanges();
                value.Add("message", true);
                value.Add("content", "删除成功");
                return Content(JsonConvert.SerializeObject(value));
            }
            return Content("true");
        }

        /// <summary>
        /// 总表,纯看
        /// </summary>
        /// <param name="type">类型(现在没有,筛选有前端筛选)</param>
        /// <returns></returns>
        public ActionResult TotalProcess_CapacityOnlyRead(string type)
        {
            var totalList = db.Process_Capacity_Total.ToList();//查找所有平台,PCB板,类型
            JArray result = new JArray();
            //#region 取值
            ////smt
            //var smttemp = db.Pick_And_Place.Select(c => new TempSmt { Section = c.Section, ModuleNeed = c.ModuleNeedNum2, PersonNum = c.PersonNum, CapacityPerHour = c.CapacityPerHour, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, ProcessDescription = c.ProcessDescription }).ToList();
            ////插件
            //var PluginList = db.Process_Capacity_Plugin.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, PCBASingleLampNumber = c.PCBASingleLampNumber, StandardCapacity = c.PluginStandardCapacity, Name = c.Name, ModuleNeedNum = c.ModuleNeedNum2, SingleLampWorkingHours = c.SingleLampWorkingHours, StandardNumber = c.PluginStandardNumber, Id = c.Id });
            ////平衡卡
            //var Balance = db.ProcessBalance.Select(c => new TempBlance { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.StandardOutput, StandardTotal = c.StandardTotal, StandardHourlyOutputPerCapita = c.StandardHourlyOutputPerCapita, Id = c.Id, ModuleNeedNum = c.ModuleNeedNum2, Name = c.Title, Section = c.Section, SerialNumber = c.SerialNumber, ScrewMachineNum = c.ScrewMachineNum, BalanceRate = c.BalanceRate, Bottleneck = c.Bottleneck, DispensMachineNum = c.DispensMachineNum });
            ////三防
            ////var ThreeProfList = db.Process_Capacity_ThreeProf.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.ThreeProfStabdardOutput, Name = c.ThreeProfProcessName, ModuleNeedNum = c.ModuleNeedNum2, StandardNumber = c.ThreeProfStandardTotal, Id = c.Id });
            ////喷墨
            //var InkjetList = db.Process_Capacity_Inkjet.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.InkjetStabdardOutputPerHour, Name = c.InkjetProcessName, ModuleNeedNum = c.ModuleNeedNum2, StandardNumber = c.InkjetSuctionStandardTotal, Id = c.Id });
            ////灌胶
            //var GlueList = db.Process_Capacity_Glue.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.GlueStabdardOutput, Name = c.GlueProcessName, ModuleNeedNum = c.ModuleNeedNum2, StandardNumber = c.GlueStandardTotal, Id = c.Id });
            ////气密
            //var AirtightList = db.Process_Capacity_Airtight.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.AirtightStabdardOutput, Name = c.AirtightProcessName, ModuleNeedNum = c.ModuleNeedNum2, StandardNumber = c.AirtightStandardTotal, Id = c.Id });
            ////老化
            //var BurnList = db.Process_Capacity_Burin.Select(c => new TempBurn { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, BurinOneProcessName = c.BurinOneProcessName, BurinOneStabdardOutputPerHour = c.BurinOneStabdardOutputPerHour, BurninOneSuctionStandardTotal = c.BurninOneSuctionStandardTotal, ModuleNeedNum = c.ModuleNeedNum2, Id = c.Id, BurinTwoProcessName = c.BurinTwoProcessName, BurinTwoStabdardOutputPerHour = c.BurinTwoStabdardOutputPerHour, BurinTwoSuctionStandardTotal = c.BurinTwoSuctionStandardTotal });
            //#endregion
            //foreach (var totalitem in totalList)
            //{
            //    JObject total = new JObject();
            //    var Platform = totalitem.Platform;
            //    var Type = totalitem.Type;
            //    var ProductPCBnumber = totalitem.ProductPCBnumber;
            //    decimal modeleNeedHoure = 0;
            //    int totalperson = 0;
            //    int smttotal = 0;
            //    decimal smtmodele = 0;
            //    int moduletotal = 0;
            //    decimal moduleneedhoure = 0;
            //    int packtotal = 0;
            //    decimal paackmodule = 0;
            //    JArray content = new JArray();
            //    //平台
            //    total.Add("Platform", Platform);
            //    //型号
            //    total.Add("Type", Type);
            //    //PCB
            //    total.Add("ProductPCBnumber", ProductPCBnumber);

            //    //箱体组装的标准产能,如果没有则传0
            //    var packStandardOutput = db.ProcessBalance.Count(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "模组装配") == 0 ? 0 : db.ProcessBalance.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "模组装配").Min(c => c.StandardOutput);


            //    #region SMT
            //    JArray SMT = new JArray();
            //    //贴片
            //    JObject pick = new JObject();
            //    var jobcontinfo = smttemp.Where(c => c.Type == Type && c.Plafrom == Platform && c.PCB == ProductPCBnumber).Select(c => new TempSmt { Section = c.Section, ModuleNeed = c.ModuleNeed, PersonNum = c.PersonNum, CapacityPerHour = c.CapacityPerHour, ProcessDescription = c.ProcessDescription }).ToList();//根据平台,类型,PCB板查找贴片数据
            //    var ss = SMTvalue(jobcontinfo);//调用通用模板数据

            //    if (ss.Seaction != "/")//是否跳过,跳过就不赋值
            //    {

            //        content.Add(Indexitemjobject("IC最大", ss.PersonNum, ss.StandardOutput, ss.ModuleNeed, packStandardOutput));//通用模板
            //        content.Add(Indexitemjobject("IC最小", ss.PersonNum2, ss.StandardOutput2, ss.ModuleNeed, packStandardOutput));//通用模板
            //        content.Add(Indexitemjobject("灯面最大", ss.PersonNum3, ss.StandardOutput3, ss.ModuleNeed, packStandardOutput));//通用模板
            //        content.Add(Indexitemjobject("灯面最小", ss.PersonNum4, ss.StandardOutput4, ss.ModuleNeed, packStandardOutput));//通用模板
            //        //总人数
            //        totalperson = totalperson + (ss.PersonNum > ss.PersonNum2 ? ss.PersonNum2 : ss.PersonNum);
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (ss.StandardOutput < ss.StandardOutput2 ? (ss.StandardOutput == 0 || ss.ModuleNeed == 0 ? 0 : 1 / (ss.StandardOutput / ss.PersonNum / ss.ModuleNeed)) : (ss.PersonNum2 == 0 || ss.ModuleNeed == 0 ? 0 : 1 / (ss.StandardOutput2 / ss.PersonNum2 / ss.ModuleNeed)));
            //        //总人数
            //        totalperson = totalperson + (ss.PersonNum3 > ss.PersonNum4 ? ss.PersonNum4 : ss.PersonNum3);
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (ss.StandardOutput3 < ss.StandardOutput4 ? (ss.StandardOutput3 == 0 || ss.ModuleNeed == 0 ? 0 : 1 / (ss.StandardOutput3 / ss.PersonNum3 / ss.ModuleNeed)) : (ss.PersonNum4 == 0 || ss.ModuleNeed == 0 ? 0 : 1 / (ss.StandardOutput4 / ss.PersonNum4 / ss.ModuleNeed)));
            //        //SMT总人数
            //        smttotal = smttotal + (ss.PersonNum > ss.PersonNum2 ? ss.PersonNum2 : ss.PersonNum);
            //        smttotal = smttotal + (ss.PersonNum3 > ss.PersonNum4 ? ss.PersonNum4 : ss.PersonNum3);
            //        //SMT单人生产模组需要总工时
            //        smtmodele = smtmodele + (ss.StandardOutput < ss.StandardOutput2 ? (ss.StandardOutput == 0 || ss.ModuleNeed == 0 ? 0 : 1 / (ss.StandardOutput / ss.PersonNum / ss.ModuleNeed)) : (ss.PersonNum2 == 0 || ss.ModuleNeed == 0 ? 0 : 1 / (ss.StandardOutput2 / ss.PersonNum2 / ss.ModuleNeed)));

            //        smtmodele = smtmodele + (ss.StandardOutput3 < ss.StandardOutput4 ? (ss.StandardOutput3 == 0 || ss.ModuleNeed == 0 ? 0 : 1 / (ss.StandardOutput3 / ss.PersonNum3 / ss.ModuleNeed)) : (ss.PersonNum4 == 0 || ss.ModuleNeed == 0 ? 0 : 1 / (ss.StandardOutput4 / ss.PersonNum4 / ss.ModuleNeed)));
            //    }
            //    pick.Add("name", "贴片");
            //    pick.Add("content", content);
            //    content = new JArray();
            //    //插件
            //    var plug = PluginList.Where(c => c.Type == Type && c.Plafrom == Platform && c.PCB == ProductPCBnumber).ToList();
            //    foreach (var plugitem in plug)
            //    {
            //        content.Add(Indexitemjobject(plugitem.Name, plugitem.StandardNumber, plugitem.StandardCapacity, plugitem.ModuleNeedNum, packStandardOutput));
            //        //总人数
            //        totalperson = totalperson + plugitem.StandardNumber;
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (plugitem.StandardNumber == 0 || plugitem.ModuleNeedNum == 0 ? 0 : 1 / (plugitem.StandardCapacity / plugitem.StandardNumber / plugitem.ModuleNeedNum));
            //        //SMT总人数
            //        smttotal = smttotal + plugitem.StandardNumber;
            //        //SMT单人生产模组需要总工时
            //        smtmodele = smtmodele + (plugitem.StandardNumber == 0 || plugitem.ModuleNeedNum == 0 ? 0 : 1 / (plugitem.StandardCapacity / plugitem.StandardNumber / plugitem.ModuleNeedNum));
            //    }
            //    JObject plugjobject = new JObject();
            //    plugjobject.Add("name", "插件");
            //    plugjobject.Add("content", content);
            //    content = new JArray();
            //    //后焊
            //    var weld = Balance.Where(c => c.Type == Type && c.Plafrom == Platform && c.PCB == ProductPCBnumber && c.Section == "后焊").ToList();
            //    var weldvalue = BlanceModule(weld);
            //    foreach (var newweld in weldvalue)
            //    {
            //        content.Add(Indexitemjobject(newweld.Name, newweld.StandardTotal, newweld.StandardCapacity, newweld.ModuleNeedNum, packStandardOutput));

            //        //总人数
            //        totalperson = totalperson + newweld.StandardTotal;
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (newweld.StandardTotal == 0 || newweld.ModuleNeedNum == 0 ? 0 : 1 / (newweld.StandardCapacity / newweld.StandardTotal / newweld.ModuleNeedNum));
            //        //SMT总人数
            //        smttotal = smttotal + newweld.StandardTotal;
            //        //SMT单人生产模组需要总工时
            //        smtmodele = smtmodele + (newweld.StandardTotal == 0 || newweld.ModuleNeedNum == 0 ? 0 : 1 / (newweld.StandardCapacity / newweld.StandardTotal / newweld.ModuleNeedNum));
            //    }
            //    JObject weldjobject = new JObject();
            //    weldjobject.Add("name", "后焊");
            //    weldjobject.Add("content", content);
            //    content = new JArray();
            //    SMT.Add(pick);//贴片
            //    SMT.Add(plugjobject);//插件
            //    SMT.Add(weldjobject);//后焊
            //    total.Add("SMT", SMT);//总表纯看大模块 SMT
            //    #endregion

            //    #region 模块
            //    JArray modlue = new JArray();
            //    //三防 
            //    var threeProf = Balance.Where(c => c.Type == Type && c.Plafrom == Platform && c.PCB == ProductPCBnumber && c.Section == "三防").ToList();
            //    foreach (var item in threeProf)
            //    {
            //        content.Add(Indexitemjobject(item.Name, item.StandardTotal, item.StandardCapacity, item.ModuleNeedNum, packStandardOutput));
            //        //总人数
            //        totalperson = totalperson + item.StandardTotal;
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (item.StandardTotal == 0 || item.ModuleNeedNum == 0 ? 0 : 1 / (item.StandardCapacity / item.StandardTotal / item.ModuleNeedNum));

            //        //模块总人数
            //        moduletotal = moduletotal + item.StandardTotal;
            //        //模块单人生产模组需要总工时
            //        moduleneedhoure = moduleneedhoure + (item.StandardTotal == 0 || item.ModuleNeedNum == 0 ? 0 : 1 / (item.StandardCapacity / item.StandardTotal / item.ModuleNeedNum));

            //    }
            //    JObject threeProfjobject = new JObject();
            //    threeProfjobject.Add("name", "三防");
            //    threeProfjobject.Add("content", content);
            //    content = new JArray();
            //    //打底壳
            //    var pan = Balance.Where(c => c.Type == Type && c.Plafrom == Platform && c.PCB == ProductPCBnumber && c.Section == "打底壳").ToList();
            //    var panvalue = BlanceModule(pan);
            //    foreach (var newSerialNumber in panvalue)
            //    {
            //        ////最新一版本
            //        //var newSerialNumber = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "打底壳" && c.Title == item).FirstOrDefault();

            //        content.Add(Indexitemjobject(newSerialNumber.Name, newSerialNumber.StandardTotal, newSerialNumber.StandardCapacity, newSerialNumber.ModuleNeedNum, packStandardOutput));

            //        //总人数
            //        totalperson = totalperson + newSerialNumber.StandardTotal;
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (newSerialNumber.StandardTotal == 0 || newSerialNumber.ModuleNeedNum == 0 ? 0 : 1 / (newSerialNumber.StandardCapacity / newSerialNumber.StandardTotal / newSerialNumber.ModuleNeedNum));

            //        //模块总人数
            //        moduletotal = moduletotal + newSerialNumber.StandardTotal;
            //        //模块单人生产模组需要总工时
            //        moduleneedhoure = moduleneedhoure + (newSerialNumber.StandardTotal == 0 || newSerialNumber.ModuleNeedNum == 0 ? 0 : 1 / (newSerialNumber.StandardCapacity / newSerialNumber.StandardTotal / newSerialNumber.ModuleNeedNum));
            //    }
            //    JObject panjobject = new JObject();
            //    panjobject.Add("name", "打底壳");
            //    panjobject.Add("content", content);
            //    content = new JArray();

            //    //装磁吸安装板
            //    var Magnetic = Balance.Where(c => c.Type == Type && c.Plafrom == Platform && c.PCB == ProductPCBnumber && c.Section == "装磁吸安装板").ToList();
            //    var Magneticvalue = BlanceModule(Magnetic);
            //    foreach (var newSerialNumber in Magnetic)
            //    {
            //        content.Add(Indexitemjobject(newSerialNumber.Name, newSerialNumber.StandardTotal, newSerialNumber.StandardCapacity, newSerialNumber.ModuleNeedNum, packStandardOutput));

            //        //总人数
            //        totalperson = totalperson + newSerialNumber.StandardTotal;
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (newSerialNumber.StandardTotal == 0 || newSerialNumber.ModuleNeedNum == 0 ? 0 : 1 / (newSerialNumber.StandardCapacity / newSerialNumber.StandardTotal / newSerialNumber.ModuleNeedNum));
            //        //模块总人数
            //        moduletotal = moduletotal + newSerialNumber.StandardTotal;
            //        //模块单人生产模组需要总工时
            //        moduleneedhoure = moduleneedhoure + (newSerialNumber.StandardTotal == 0 || newSerialNumber.ModuleNeedNum == 0 ? 0 : 1 / (newSerialNumber.StandardCapacity / newSerialNumber.StandardTotal / newSerialNumber.ModuleNeedNum));
            //    }
            //    JObject Magneticjobject = new JObject();
            //    Magneticjobject.Add("name", "装磁吸安装板");
            //    Magneticjobject.Add("content", content);
            //    content = new JArray();

            //    //喷墨
            //    var Inkjet = InkjetList.Where(c => c.Type == Type && c.Plafrom == Platform && c.PCB == ProductPCBnumber).ToList();
            //    foreach (var item in Inkjet)
            //    {
            //        content.Add(Indexitemjobject(item.Name, item.StandardNumber, item.StandardCapacity, item.ModuleNeedNum, packStandardOutput));
            //        //总人数
            //        totalperson = totalperson + item.StandardNumber;
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (item.StandardNumber == 0 || item.ModuleNeedNum == 0 ? 0 : 1 / (item.StandardCapacity / item.StandardNumber / item.ModuleNeedNum));
            //        //模块总人数
            //        moduletotal = moduletotal + item.StandardNumber;
            //        //模块单人生产模组需要总工时
            //        moduleneedhoure = moduleneedhoure + (item.StandardNumber == 0 || item.ModuleNeedNum == 0 ? 0 : 1 / (item.StandardCapacity / item.StandardNumber / item.ModuleNeedNum));
            //    }

            //    JObject Inkjetjobject = new JObject();
            //    Inkjetjobject.Add("name", "喷墨");
            //    Inkjetjobject.Add("content", content);
            //    content = new JArray();
            //    //灌胶
            //    var glue = GlueList.Where(c => c.Type == Type && c.Plafrom == Platform && c.PCB == ProductPCBnumber).ToList();
            //    foreach (var item in glue)
            //    {
            //        content.Add(Indexitemjobject(item.Name, item.StandardNumber, item.StandardCapacity, item.ModuleNeedNum, packStandardOutput));
            //        //总人数
            //        totalperson = totalperson + item.StandardNumber;
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (item.StandardNumber == 0 || item.ModuleNeedNum == 0 ? 0 : 1 / (item.StandardCapacity / item.StandardNumber / item.ModuleNeedNum));
            //        //模块总人数
            //        moduletotal = moduletotal + item.StandardNumber;
            //        //模块单人生产模组需要总工时
            //        moduleneedhoure = moduleneedhoure + (item.StandardNumber == 0 || item.ModuleNeedNum == 0 ? 0 : 1 / (item.StandardCapacity / item.StandardNumber / item.ModuleNeedNum));
            //    }

            //    JObject gluejobject = new JObject();
            //    gluejobject.Add("name", "灌胶");
            //    gluejobject.Add("content", content);
            //    content = new JArray();
            //    //气密
            //    var airtight = AirtightList.Where(c => c.Type == Type && c.Plafrom == Platform && c.PCB == ProductPCBnumber).ToList();
            //    foreach (var item in airtight)
            //    {
            //        content.Add(Indexitemjobject(item.Name, item.StandardNumber, item.StandardCapacity, item.ModuleNeedNum, packStandardOutput));
            //        //总人数
            //        totalperson = totalperson + item.StandardNumber;
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (item.StandardNumber == 0 || item.ModuleNeedNum == 0 ? 0 : 1 / (item.StandardCapacity / item.StandardNumber / item.ModuleNeedNum));
            //        //模块总人数
            //        moduletotal = moduletotal + item.StandardNumber;
            //        //模块单人生产模组需要总工时
            //        moduleneedhoure = moduleneedhoure + (item.StandardNumber == 0 || item.ModuleNeedNum == 0 ? 0 : 1 / (item.StandardCapacity / item.StandardNumber / item.ModuleNeedNum));
            //    }
            //    JObject airtightjobject = new JObject();
            //    airtightjobject.Add("name", "气密");
            //    airtightjobject.Add("content", content);
            //    content = new JArray();
            //    //锁面罩
            //    var mask = Balance.Where(c => c.Type == Type && c.Plafrom == Platform && c.PCB == ProductPCBnumber && c.Section == "锁面罩").ToList();
            //    var maskvalue = BlanceModule(mask);
            //    foreach (var newSerialNumber in maskvalue)
            //    {

            //        content.Add(Indexitemjobject(newSerialNumber.Name, newSerialNumber.StandardTotal, newSerialNumber.StandardCapacity, newSerialNumber.ModuleNeedNum, packStandardOutput));

            //        //总人数
            //        totalperson = totalperson + newSerialNumber.StandardTotal;
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (newSerialNumber.StandardTotal == 0 || newSerialNumber.ModuleNeedNum == 0 ? 0 : 1 / (newSerialNumber.StandardCapacity / newSerialNumber.StandardTotal / newSerialNumber.ModuleNeedNum));
            //        //模块总人数
            //        moduletotal = moduletotal + newSerialNumber.StandardTotal;
            //        //模块单人生产模组需要总工时
            //        moduleneedhoure = moduleneedhoure + (newSerialNumber.StandardTotal == 0 || newSerialNumber.ModuleNeedNum == 0 ? 0 : 1 / (newSerialNumber.StandardCapacity / newSerialNumber.StandardTotal / newSerialNumber.ModuleNeedNum));
            //    }
            //    JObject maskjobject = new JObject();
            //    maskjobject.Add("name", "锁面罩");
            //    maskjobject.Add("content", content);
            //    content = new JArray();

            //    modlue.Add(threeProfjobject);//三防
            //    modlue.Add(panjobject);//打底壳
            //    modlue.Add(Magneticjobject);//装磁吸
            //    modlue.Add(Inkjetjobject);//喷墨
            //    modlue.Add(gluejobject);//灌胶
            //    modlue.Add(airtightjobject);//气密
            //    modlue.Add(maskjobject);//锁面罩
            //    total.Add("module", modlue);//总表纯看 大模块 模块
            //    #endregion

            //    #region 组装
            //    JArray packing = new JArray();
            //    //模组装配
            //    var assembly = Balance.Where(c => c.Type == Type && c.Plafrom == Platform && c.PCB == ProductPCBnumber && c.Section == "模组装配").ToList();
            //    var assemblyvalue = BlanceModule(assembly);
            //    foreach (var newSerialNumber in assemblyvalue)
            //    {

            //        content.Add(Indexitemjobject(newSerialNumber.Name, newSerialNumber.StandardTotal, newSerialNumber.StandardCapacity, newSerialNumber.ModuleNeedNum, packStandardOutput));

            //        //总人数
            //        totalperson = totalperson + newSerialNumber.StandardTotal;
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (newSerialNumber.StandardTotal == 0 || newSerialNumber.ModuleNeedNum == 0 ? 0 : 1 / (newSerialNumber.StandardCapacity / newSerialNumber.StandardTotal / newSerialNumber.ModuleNeedNum));
            //        //组装总人数
            //        packtotal = packtotal + newSerialNumber.StandardTotal;
            //        //组装单人生产模组需要总工时
            //        paackmodule = paackmodule + (newSerialNumber.StandardTotal == 0 || newSerialNumber.ModuleNeedNum == 0 ? 0 : 1 / (newSerialNumber.StandardCapacity / newSerialNumber.StandardTotal / newSerialNumber.ModuleNeedNum));
            //    }
            //    JObject assemblyjobject = new JObject();
            //    assemblyjobject.Add("name", "模组装配");
            //    assemblyjobject.Add("content", content);
            //    content = new JArray();
            //    //老化
            //    var burn = db.Process_Capacity_Burin.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber).FirstOrDefault();
            //    if (burn != null)
            //    {
            //        content.Add(Indexitemjobject(burn.BurinOneProcessName, burn.BurninOneSuctionStandardTotal, burn.BurinOneStabdardOutputPerHour, burn.ModuleNeedNum2, packStandardOutput));
            //        content.Add(Indexitemjobject(burn.BurinTwoProcessName, burn.BurinTwoSuctionStandardTotal, burn.BurinTwoStabdardOutputPerHour, burn.ModuleNeedNum2, packStandardOutput));
            //        //总人数
            //        totalperson = totalperson + burn.BurninOneSuctionStandardTotal + burn.BurinTwoSuctionStandardTotal;
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (burn.BurninOneSuctionStandardTotal == 0 || burn.ModuleNeedNum2 == 0 ? 0 : 1 / (burn.BurinOneStabdardOutputPerHour / burn.BurninOneSuctionStandardTotal / burn.ModuleNeedNum2)) + (burn.BurinTwoSuctionStandardTotal == 0 || burn.ModuleNeedNum2 == 0 ? 0 : 1 / (burn.BurinTwoStabdardOutputPerHour / burn.BurinTwoSuctionStandardTotal / burn.ModuleNeedNum2));

            //        //组装总人数
            //        packtotal = packtotal + burn.BurninOneSuctionStandardTotal + burn.BurinTwoSuctionStandardTotal;
            //        //组装单人生产模组需要总工时
            //        paackmodule = paackmodule + (burn.BurninOneSuctionStandardTotal == 0 || burn.ModuleNeedNum2 == 0 ? 0 : 1 / (burn.BurinOneStabdardOutputPerHour / burn.BurninOneSuctionStandardTotal / burn.ModuleNeedNum2)) + (burn.BurinTwoSuctionStandardTotal == 0 || burn.ModuleNeedNum2 == 0 ? 0 : 1 / (burn.BurinTwoStabdardOutputPerHour / burn.BurinTwoSuctionStandardTotal / burn.ModuleNeedNum2));
            //    }
            //    JObject burnjobject = new JObject();
            //    burnjobject.Add("name", "老化");
            //    burnjobject.Add("content", content);
            //    content = new JArray();
            //    //包装
            //    var pack = Balance.Where(c => c.Type == Type && c.Plafrom == Platform && c.PCB == ProductPCBnumber && c.Section == "包装").ToList();
            //    var packvalue = BlanceModule(pack);
            //    foreach (var newSerialNumber in packvalue)
            //    {
            //        content.Add(Indexitemjobject(newSerialNumber.Name, newSerialNumber.StandardTotal, newSerialNumber.StandardCapacity, newSerialNumber.ModuleNeedNum, packStandardOutput));

            //        //总人数
            //        totalperson = totalperson + newSerialNumber.StandardTotal;
            //        //单人生产模组需要总工时
            //        modeleNeedHoure = modeleNeedHoure + (newSerialNumber.StandardTotal == 0 || newSerialNumber.ModuleNeedNum == 0 ? 0 : 1 / (newSerialNumber.StandardCapacity / newSerialNumber.StandardTotal / newSerialNumber.ModuleNeedNum));
            //        //组装总人数
            //        packtotal = packtotal + newSerialNumber.StandardTotal;
            //        //组装单人生产模组需要总工时
            //        paackmodule = paackmodule + (newSerialNumber.StandardTotal == 0 || newSerialNumber.ModuleNeedNum == 0 ? 0 : 1 / (newSerialNumber.StandardCapacity / newSerialNumber.StandardTotal / newSerialNumber.ModuleNeedNum));
            //    }
            //    JObject packjobject = new JObject();
            //    packjobject.Add("name", "包装");
            //    packjobject.Add("content", content);
            //    packing.Add(assemblyjobject);//模组装配
            //    packing.Add(burnjobject);//老化
            //    packing.Add(packjobject);//包装
            //    total.Add("packing", packing);//总表大块 模组
            //    #endregion

            //    #region 三表汇总
            //    //单人生产每模组需求总工时
            //    total.Add("modeleNeedHoure", Math.Round(modeleNeedHoure, 2));
            //    //SMT单人生产每模组需求总工时
            //    total.Add("SMTmodeleNeedHoure", Math.Round(smtmodele, 2));
            //    //模组单人生产每模组需求总工时
            //    total.Add("ModulemodeleNeedHoure", Math.Round(moduleneedhoure, 2));
            //    //组装单人生产每模组需求总工时
            //    total.Add("PackagemodeleNeedHoure", Math.Round(paackmodule, 2));
            //    //总人数
            //    total.Add("totalperson", totalperson);
            //    //SMT总人数
            //    total.Add("SMTtotalperson", smttotal);
            //    // 模组总人数
            //    total.Add("Moduletotalperson", moduletotal);
            //    //组装总人数
            //    total.Add("Packagetotalperson", packtotal);
            //    //每小时产能（模组/h）
            //    total.Add("capacityperhour", modeleNeedHoure == 0 ? 0 : Math.Round(totalperson / modeleNeedHoure, 2));
            //    //SMT每小时产能（模组/h）
            //    total.Add("SMTcapacityperhour", smtmodele == 0 ? 0 : Math.Round(smttotal / smtmodele, 2));
            //    //模组每小时产能（模组/h）
            //    total.Add("Modulecapacityperhour", moduleneedhoure == 0 ? 0 : Math.Round(moduletotal / moduleneedhoure, 2));
            //    //包装每小时产能（模组/h）
            //    total.Add("Packagecapacityperhour", paackmodule == 0 ? 0 : Math.Round(packtotal / paackmodule, 2));
            //    #endregion

            //    result.Add(total);
            //}
            return Content(JsonConvert.SerializeObject(result));
        }


        //index2 子级通用模板
        public JObject Indexitemjobject(string section, int StandardTotal, decimal StandardOutput, decimal modelNeedNum, decimal packStandardOutput)
        {
            JObject result = new JObject();
            if (section == "/")//是否跳过,跳过则传回/
            {
                result.Add("seaction", section);
                //标配人数
                result.Add("StandardTotal", "/");
                //标砖产能
                result.Add("StandardOutput", "/");
                //平衡
                result.Add("BalanceRate", "/");
                return result;
            }
            result.Add("seaction", section);
            //标配人数
            result.Add("StandardTotal", StandardTotal);
            //标砖产能
            result.Add("StandardOutput", StandardOutput);
            //平衡率计算 模组装配的标准产能/标准产能 * 模组需求
            decimal BalanceRate = StandardOutput == 0 ? 0 : Math.Round(packStandardOutput / StandardOutput * modelNeedNum, 2);
            result.Add("BalanceRate", BalanceRate);
            return result;
        }

        //详细分析表
        public ActionResult Detailed(string Type, string ProductPCBnumber, string Platform)
        {
            JArray result = new JArray();
            var smt = db.Pick_And_Place.Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform).ToList();
            //箱体组装的标准产能
            var packStandardOutput = db.ProcessBalance.Count(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "模组装配") == 0 ? 0 : db.ProcessBalance.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "模组装配").Min(c => c.StandardOutput);

            //SMT
            if (smt.Count != 0)//如果有贴片信息
            {
                if (smt.FirstOrDefault().Section == "/")//如果是跳过
                {
                    result.Add(DetailedItem("ic面贴装", 0, 0, 0, 0, true));//调用json通用模板
                    result.Add(DetailedItem("灯面贴装", 0, 0, 0, 0, true));//调用json通用模板
                }
                else
                {
                    var moduleNeed = smt.FirstOrDefault().ModuleNeedNum2;//贴片模组需求数
                    if (smt.Count(c => c.ProcessDescription == "IC面贴装") != 0)//如果贴片信息中含有IC面贴装数据,没有则不赋值
                    {
                        var icMaxStandardTotal = smt.Where(c => c.ProcessDescription == "IC面贴装").Max(c => c.PersonNum);//IC面贴装最大标配人数
                        var icMaxStandardOutput = smt.Where(c => c.ProcessDescription == "IC面贴装").Max(c => c.CapacityPerHour);//IC面贴装最大标准产能
                        var icMinStandardTotal = smt.Where(c => c.ProcessDescription == "IC面贴装").Min(c => c.PersonNum);//ic面最小标配人数
                        var icMinStandardOutput = smt.Where(c => c.ProcessDescription == "IC面贴装").Min(c => c.CapacityPerHour);//IC面最小标准产能
                        result.Add(DetailedItem("IC最大", icMaxStandardTotal, icMaxStandardOutput, moduleNeed, packStandardOutput, false));//调用通用模板
                        result.Add(DetailedItem("IC最小", icMinStandardTotal, icMinStandardOutput, moduleNeed, packStandardOutput, false));//调用通用模板
                    }
                    if (smt.Count(c => c.ProcessDescription == "灯面贴装") != 0)
                    {
                        var lineMaxStandardTotal = smt.Where(c => c.ProcessDescription == "灯面贴装").Max(c => c.PersonNum);//灯面最大标配人数
                        var lineMaxStandardOutput = smt.Where(c => c.ProcessDescription == "灯面贴装").Max(c => c.CapacityPerHour);//灯面最大标准产能
                        var lineMinStandardTotal = smt.Where(c => c.ProcessDescription == "灯面贴装").Min(c => c.PersonNum);//灯面最小标配人数
                        var lineMinStandardOutput = smt.Where(c => c.ProcessDescription == "灯面贴装").Min(c => c.CapacityPerHour);//灯面最小标准产能
                        result.Add(DetailedItem("灯面最大", lineMaxStandardTotal, lineMaxStandardOutput, moduleNeed, packStandardOutput, false));//调用通用模板
                        result.Add(DetailedItem("灯面最小", lineMinStandardTotal, lineMinStandardOutput, moduleNeed, packStandardOutput, false));//调用通用模板
                    }

                }
            }
            //插件 
            var plug = db.Process_Capacity_Plugin.Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform).ToList();//根据类型,PCB板,平台查找插件数据
            foreach (var item in plug)//循环得到的数据
            {
                if (item.Name == "/")//如果是跳过,传0
                {
                    result.Add(DetailedItem("插件", 0, 0, 0, 0, true));
                    break;
                }
                else//否则传值
                {
                    result.Add(DetailedItem(item.Name, item.PluginStandardNumber, item.PluginStandardCapacity, item.ModuleNeedNum2, packStandardOutput, false));
                }
            }
            //后焊
            var weld = db.ProcessBalance.Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "后焊").Select(c => c.Title).Distinct().ToList();//根据类型,PCB板,平台查找后焊数据
            foreach (var item in weld)
            {
                if (item == "/")//如果跳过传0
                {
                    result.Add(DetailedItem("后焊", 0, 0, 0, 0, true));
                    break;
                }
                //最新一版本,只有 上传excel 文件的工段才需要分版本
                var newweld = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "后焊" && c.Title == item).FirstOrDefault();
                //调用模板 
                result.Add(DetailedItem(newweld.Title, newweld.StandardTotal, newweld.StandardOutput, newweld.ModuleNeedNum2, packStandardOutput, false));
            }
            //三防
            var threeProf = db.Process_Capacity_ThreeProf.Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform).ToList();//根据类型,PCB板,平台查找三防数据
            foreach (var item in threeProf)
            {
                if (item.ThreeProfProcessName == "/")//如果跳过
                    result.Add(DetailedItem("三防", 0, 0, 0, 0, true));
                else//调用模板
                {
                    result.Add(DetailedItem(item.ThreeProfProcessName, item.ThreeProfStandardTotal, item.ThreeProfStabdardOutput, item.ModuleNeedNum2, packStandardOutput, false));
                }
            }
            //打底壳
            var pan = db.ProcessBalance.Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "打底壳").Select(c => c.Title).Distinct().ToList();//根据类型,PCB板,平台查找打底壳数据
            foreach (var item in pan)
            {
                if (item == "/")//如果跳过
                    result.Add(DetailedItem("打底壳", 0, 0, 0, 0, true));
                else
                {
                    //最新一版本只有 上传excel 文件的工段才需要分版本
                    var newSerialNumber = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "打底壳" && c.Title == item).FirstOrDefault();
                    //调用模板
                    result.Add(DetailedItem(newSerialNumber.Title, newSerialNumber.StandardTotal, newSerialNumber.StandardOutput, newSerialNumber.ModuleNeedNum2, packStandardOutput, false));
                }
            }
            //装磁吸安装板
            var Magnetic = db.ProcessBalance.Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "装磁吸安装板").Select(c => c.Title).Distinct().ToList();//根据类型,PCB板,平台查找装磁吸数据
            foreach (var item in Magnetic)
            {
                if (item == "/")//如果跳过 
                    result.Add(DetailedItem("装磁吸安装板", 0, 0, 0, 0, true));
                else
                {
                    //最新一版本 上传excel 文件的工段才需要分版本
                    var newSerialNumber = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "装磁吸安装板" && c.Title == item).FirstOrDefault();
                    //调用模板
                    result.Add(DetailedItem(newSerialNumber.Title, newSerialNumber.StandardTotal, newSerialNumber.StandardOutput, newSerialNumber.ModuleNeedNum2, packStandardOutput, false));
                }
            }
            //喷墨
            var Inkjet = db.Process_Capacity_Inkjet.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber).ToList();//根据类型,PCB板,平台查找喷墨数据
            foreach (var item in Inkjet)
            {
                if (item.InkjetProcessName == "/")//如果跳过
                    result.Add(DetailedItem("喷墨", 0, 0, 0, 0, true));
                else//调用模板
                {
                    result.Add(DetailedItem(item.InkjetProcessName, item.InkjetSuctionStandardTotal, item.InkjetStabdardOutputPerHour, item.ModuleNeedNum2, packStandardOutput, false));
                }
            }
            //灌胶
            var glue = db.Process_Capacity_Glue.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber).ToList();//根据类型,PCB板,平台查找灌胶数据
            foreach (var item in glue)
            {
                if (item.GlueProcessName == "/")//如果跳过 
                    result.Add(DetailedItem("灌胶", 0, 0, 0, 0, true));
                else//调用模板
                {
                    result.Add(DetailedItem(item.GlueProcessName, item.GlueStandardTotal, item.GlueStabdardOutput, item.ModuleNeedNum2, packStandardOutput, false));
                }
            }
            //气密
            var airtight = db.Process_Capacity_Airtight.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber).ToList();//根据类型,PCB板,平台查找气密数据
            foreach (var item in airtight)
            {
                if (item.AirtightProcessName == "/")//如果跳过 
                    result.Add(DetailedItem("气密", 0, 0, 0, 0, true));
                else//调用模板
                {
                    result.Add(DetailedItem(item.AirtightProcessName, item.AirtightStandardTotal, item.AirtightStabdardOutput, item.ModuleNeedNum2, packStandardOutput, false));
                }
            }
            //锁面罩
            var mask = db.ProcessBalance.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "锁面罩").Select(c => c.Title).Distinct().ToList();//根据类型,PCB板,平台查找锁面罩数据
            foreach (var item in mask)
            {
                if (item == "/")//如果跳过 
                    result.Add(DetailedItem("锁面罩", 0, 0, 0, 0, true));
                else
                {
                    //最新一版本 上传excel 文件的工段才需要分版本
                    var newSerialNumber = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "锁面罩" && c.Title == item).FirstOrDefault();
                    //调用模板
                    result.Add(DetailedItem(newSerialNumber.Title, newSerialNumber.StandardTotal, newSerialNumber.StandardOutput, newSerialNumber.ModuleNeedNum2, packStandardOutput, false));
                }
            }
            //模组装配
            var assembly = db.ProcessBalance.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "模组装配").Select(c => c.Title).Distinct().ToList();//根据类型,PCB板,平台查找模组装配数据
            foreach (var item in assembly)
            {
                if (item == "/")//如果跳过 
                    result.Add(DetailedItem("模组装配", 0, 0, 0, 0, true));
                else
                {
                    //最新一版本 上传excel 文件的工段才需要分版本
                    var newSerialNumber = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "模组装配" && c.Title == item).FirstOrDefault();
                    //调用模板
                    result.Add(DetailedItem(newSerialNumber.Title, newSerialNumber.StandardTotal, newSerialNumber.StandardOutput, newSerialNumber.ModuleNeedNum2, packStandardOutput, false));
                }
            }
            //老化
            var burn = db.Process_Capacity_Burin.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber).FirstOrDefault();//根据类型,PCB板,平台查找老化数据
            if (burn != null)
            {
                if (burn.BurinOneProcessName == "/")//老化1如果跳过 
                {
                    result.Add(DetailedItem("拼屏", 0, 0, 0, 0, true));
                }
                else//调用模板
                {
                    result.Add(DetailedItem(burn.BurinOneProcessName, burn.BurninOneSuctionStandardTotal, burn.BurinOneStabdardOutputPerHour, burn.ModuleNeedNum2, packStandardOutput, false));
                }
                if (burn.BurinTwoProcessName == "/")//老化2如果跳过 
                {
                    result.Add(DetailedItem("拆屏", 0, 0, 0, 0, true));
                }
                else//调用模板
                {
                    result.Add(DetailedItem(burn.BurinTwoProcessName, burn.BurinTwoSuctionStandardTotal, burn.BurinTwoStabdardOutputPerHour, burn.ModuleNeedNum2, packStandardOutput, false));
                }
            }
            //包装
            var pack = db.ProcessBalance.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "包装").Select(c => c.Title).Distinct().ToList();//根据类型,PCB板,平台查找包装数据
            foreach (var item in pack)
            {
                if (item == "/")//如果跳过 
                    result.Add(DetailedItem("包装", 0, 0, 0, 0, true));
                else
                {
                    //最新一版本 上传excel 文件的工段才需要分版本
                    var newSerialNumber = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "包装" && c.Title == item).FirstOrDefault();
                    //调用模板
                    result.Add(DetailedItem(newSerialNumber.Title, newSerialNumber.StandardTotal, newSerialNumber.StandardOutput, newSerialNumber.ModuleNeedNum2, packStandardOutput, false));
                }
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //详细分析表 子级通用模板
        public JObject DetailedItem(string name, int StandardTotal, decimal StandardOutput, decimal modelNeedNum, decimal packStandardOutput, bool skip)
        {
            JObject result = new JObject();
            if (skip)// 是跳过,传/
            {
                //描述
                result.Add("name", name);
                //标准人数
                result.Add("StandardTotal", "/");
                //标准产能
                result.Add("StandardOutput", "/");
                //人均产能
                result.Add("StandardOutputForOne", "/");
                //模组需求
                result.Add("ModelNeed", "/");
                //转换模组人均产能
                result.Add("ModelNeedForOne", "/");
                //单人每模组运算时间 
                result.Add("ModelNeedTimeForOne", "/");
                //平衡率
                result.Add("BalanceRate", "/");

                return result;
            }
            //描述
            result.Add("name", name);
            //标准人数
            result.Add("StandardTotal", StandardTotal);
            //标准产能
            result.Add("StandardOutput", StandardOutput);
            //人均产能=标准产能/标准人数
            var StandardOutputForOne = StandardTotal == 0 ? 0 : StandardOutput / StandardTotal;
            result.Add("StandardOutputForOne", Math.Round(StandardOutputForOne, 2));
            //模组需求
            result.Add("ModelNeed", modelNeedNum);
            //转换模组人均产能=人均产能/模组需求
            var ModelNeedForOne = modelNeedNum == 0 ? 0 : StandardOutputForOne / modelNeedNum;
            result.Add("ModelNeedForOne", Math.Round(ModelNeedForOne, 2));
            //单人每模组运算时间 =1/转换模组人均产能
            result.Add("ModelNeedTimeForOne", ModelNeedForOne == 0 ? 0 : 1 / ModelNeedForOne);
            //平衡率=箱体标准产能/标准产能*模组需求
            decimal BalanceRate = StandardOutput == 0 ? 0 : Math.Round(packStandardOutput / StandardOutput * modelNeedNum, 2);
            result.Add("BalanceRate", BalanceRate);

            return result;
        }

        /// <summary>
        /// 柱状图数据json
        /// </summary>
        /// <param name="Type">类型</param>
        /// <param name="ProductPCBnumber">PCB板</param>
        /// <param name="Platform">平台</param>
        /// <returns></returns>
        public ActionResult Icon(string Type, string ProductPCBnumber, string Platform)
        {
            JArray result = new JArray();
            JObject total = new JObject();
            var smt = db.Pick_And_Place.Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform).ToList();//查找符合筛选条件的贴片数据
                                                                                                                                                //箱体组装的标准产能
            var packStandardOutput = db.ProcessBalance.Count(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "模组装配") == 0 ? 0 : db.ProcessBalance.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "模组装配").Min(c => c.StandardOutput);

            //SMT
            if (smt.Count != 0)//贴片数据查找不为0,没有数据则跳过
            {
                if (smt.FirstOrDefault().Section == "/")//是否跳过,跳过平衡率显示0
                {
                    total.Add("name", "贴装");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    var moduleNeed = smt.FirstOrDefault().ModuleNeedNum2;//模组需求
                    if (smt.Count(c => c.ProcessDescription == "IC面贴装") != 0)//判断贴片数据是否含有IC面贴装数据,如果没有则不传回数据
                    {
                        var icMaxStandardOutput = smt.Where(c => c.ProcessDescription == "IC面贴装").Max(c => c.CapacityPerHour);//IC面贴装最大标准产能
                        var icMinStandardOutput = smt.Where(c => c.ProcessDescription == "IC面贴装").Min(c => c.CapacityPerHour);//IC面贴装最小标准产能
                        decimal icmaxBalanceRate = icMaxStandardOutput == 0 ? 0 : Math.Round(packStandardOutput / icMaxStandardOutput * moduleNeed, 2);//IC面贴装最大平衡率
                        decimal icminBalanceRate = icMinStandardOutput == 0 ? 0 : Math.Round(packStandardOutput / icMinStandardOutput * moduleNeed, 2);//IC面贴装最小平衡率
                        total.Add("name", "IC最大");
                        total.Add("BalanceRate", icmaxBalanceRate);
                        result.Add(total);
                        total = new JObject();

                        total.Add("name", "IC最小");
                        total.Add("BalanceRate", icminBalanceRate);
                        result.Add(total);
                        total = new JObject();
                    }
                    if (smt.Count(c => c.ProcessDescription == "灯面贴装") != 0)//判断贴片数据是否含有灯面贴装数据,如果没有则不传回数据
                    {
                        var lineMaxStandardOutput = smt.Where(c => c.ProcessDescription == "灯面贴装").Max(c => c.CapacityPerHour);//灯面贴装最大标准产能
                        var lineMinStandardOutput = smt.Where(c => c.ProcessDescription == "灯面贴装").Min(c => c.CapacityPerHour);//灯面贴装最小标准产能

                        decimal linemaxBalanceRate = lineMaxStandardOutput == 0 ? 0 : Math.Round(packStandardOutput / lineMaxStandardOutput * moduleNeed, 2);//灯面最大平衡率
                        decimal lineminBalanceRate = lineMinStandardOutput == 0 ? 0 : Math.Round(packStandardOutput / lineMinStandardOutput * moduleNeed, 2);//灯面最小平衡率
                        total.Add("name", "灯面最大");
                        total.Add("BalanceRate", linemaxBalanceRate);
                        result.Add(total);
                        total = new JObject();

                        total.Add("name", "灯面最小");
                        total.Add("BalanceRate", lineminBalanceRate);
                        result.Add(total);
                        total = new JObject();
                    }

                }
            }
            //插件 
            var plug = db.Process_Capacity_Plugin.Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform).ToList();//根据平台,类型PCB板查找插件数据
            foreach (var item in plug)
            {
                if (item.Name == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "插件");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    decimal plugBalanceRate = item.PluginStandardCapacity == 0 ? 0 : Math.Round(packStandardOutput / item.PluginStandardCapacity * item.ModuleNeedNum2, 2);//计算插件平衡率

                    total.Add("name", item.Name);
                    total.Add("BalanceRate", plugBalanceRate);
                    result.Add(total);
                    total = new JObject();
                }
            }
            //后焊
            var weld = db.ProcessBalance.Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "后焊").Select(c => c.Title).Distinct().ToList();//根据平台,类型PCB板查找后焊数据
            foreach (var item in weld)
            {
                if (item == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "后焊");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    //最新一版本 上传excel表格的工段都有版本之分
                    var newweld = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "后焊" && c.Title == item).FirstOrDefault();

                    decimal BalanceRate = newweld.StandardOutput == 0 ? 0 : Math.Round(packStandardOutput / newweld.StandardOutput * newweld.ModuleNeedNum2, 2);//计算后焊平衡率

                    total.Add("name", newweld.Title);
                    total.Add("BalanceRate", BalanceRate);
                    result.Add(total);
                    total = new JObject();
                }
            }
            //三防
            var threeProf = db.Process_Capacity_ThreeProf.Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform).ToList();//根据平台,类型PCB板查找三防数据
            foreach (var item in threeProf)
            {
                if (item.ThreeProfProcessName == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "三防");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    decimal BalanceRate = item.ThreeProfStabdardOutput == 0 ? 0 : Math.Round(packStandardOutput / item.ThreeProfStabdardOutput * item.ModuleNeedNum2, 2);//计算三防平衡率

                    total.Add("name", item.ThreeProfProcessName);
                    total.Add("BalanceRate", BalanceRate);
                    result.Add(total);
                    total = new JObject();
                }
            }
            //打底壳
            var pan = db.ProcessBalance.Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "打底壳").Select(c => c.Title).Distinct().ToList();//根据平台,类型PCB板查找打底壳数据
            foreach (var item in pan)
            {
                if (item == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "打底壳");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    //最新一版本 上传excel表格的工段都有版本之分
                    var newSerialNumber = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "打底壳" && c.Title == item).FirstOrDefault();

                    decimal BalanceRate = newSerialNumber.StandardOutput == 0 ? 0 : Math.Round(packStandardOutput / newSerialNumber.StandardOutput * newSerialNumber.ModuleNeedNum2, 2);//计算打底壳平衡率

                    total.Add("name", newSerialNumber.Title);
                    total.Add("BalanceRate", BalanceRate);
                    result.Add(total);
                    total = new JObject();
                }
            }
            //装磁吸安装板
            var Magnetic = db.ProcessBalance.Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "装磁吸安装板").Select(c => c.Title).Distinct().ToList();//根据平台,类型PCB板查找装磁吸数据
            foreach (var item in Magnetic)
            {
                if (item == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "装磁吸安装板");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    //最新一版本 上传excel表格的工段都有版本之分
                    var newSerialNumber = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "装磁吸安装板" && c.Title == item).FirstOrDefault();

                    decimal BalanceRate = newSerialNumber.StandardOutput == 0 ? 0 : Math.Round(packStandardOutput / newSerialNumber.StandardOutput * newSerialNumber.ModuleNeedNum2, 2);//计算平衡率

                    total.Add("name", newSerialNumber.Title);
                    total.Add("BalanceRate", BalanceRate);
                    result.Add(total);
                    total = new JObject();
                }
            }
            //喷墨
            var Inkjet = db.Process_Capacity_Inkjet.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber).ToList();//根据平台,类型PCB板查找喷墨数据
            foreach (var item in Inkjet)
            {
                if (item.InkjetProcessName == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "喷墨");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    decimal BalanceRate = item.InkjetStabdardOutputPerHour == 0 ? 0 : Math.Round(packStandardOutput / item.InkjetStabdardOutputPerHour * item.ModuleNeedNum2, 2);//计算平衡率

                    total.Add("name", item.InkjetProcessName);
                    total.Add("BalanceRate", BalanceRate);
                    result.Add(total);
                    total = new JObject();
                }
            }
            //灌胶
            var glue = db.Process_Capacity_Glue.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber).ToList();//根据平台,类型PCB板查找灌胶数据
            foreach (var item in glue)
            {
                if (item.GlueProcessName == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "灌胶");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    decimal BalanceRate = item.GlueStabdardOutput == 0 ? 0 : Math.Round(packStandardOutput / item.GlueStabdardOutput * item.ModuleNeedNum2, 2);//计算平衡率

                    total.Add("name", item.GlueProcessName);
                    total.Add("BalanceRate", BalanceRate);
                    result.Add(total);
                    total = new JObject();
                }
            }
            //气密
            var airtight = db.Process_Capacity_Airtight.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber).ToList();//根据平台,类型PCB板查找气密数据
            foreach (var item in airtight)
            {
                if (item.AirtightProcessName == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "气密");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    decimal BalanceRate = item.AirtightStabdardOutput == 0 ? 0 : Math.Round(packStandardOutput / item.AirtightStabdardOutput * item.ModuleNeedNum2, 2);//计算平衡率

                    total.Add("name", item.AirtightProcessName);
                    total.Add("BalanceRate", BalanceRate);
                    result.Add(total);
                    total = new JObject();
                }
            }
            //锁面罩
            var mask = db.ProcessBalance.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "锁面罩").Select(c => c.Title).Distinct().ToList();//根据平台,类型PCB板查找锁面罩数据
            foreach (var item in mask)
            {
                if (item == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "锁面罩");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    //最新一版本 上传excel表格的工段都有版本之分
                    var newSerialNumber = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "锁面罩" && c.Title == item).FirstOrDefault();

                    decimal BalanceRate = newSerialNumber.StandardOutput == 0 ? 0 : Math.Round(packStandardOutput / newSerialNumber.StandardOutput * newSerialNumber.ModuleNeedNum2, 2);//计算平衡率

                    total.Add("name", newSerialNumber.Title);
                    total.Add("BalanceRate", BalanceRate);
                    result.Add(total);
                    total = new JObject();
                }
            }
            //模组装配
            var assembly = db.ProcessBalance.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "模组装配").Select(c => c.Title).Distinct().ToList();//根据平台,类型PCB板查找模组装配数据
            foreach (var item in assembly)
            {
                if (item == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "模组装配");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    //最新一版本 上传excel表格的工段都有版本之分
                    var newSerialNumber = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "模组装配" && c.Title == item).FirstOrDefault();
                    decimal BalanceRate = newSerialNumber.StandardOutput == 0 ? 0 : Math.Round(packStandardOutput / newSerialNumber.StandardOutput * newSerialNumber.ModuleNeedNum2, 2);//计算平衡率

                    total.Add("name", newSerialNumber.Title);
                    total.Add("BalanceRate", BalanceRate);
                    result.Add(total);
                    total = new JObject();
                }
            }
            //老化
            var burn = db.Process_Capacity_Burin.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber).FirstOrDefault();//根据平台,类型PCB板查找老化数据
            if (burn != null)
            {
                if (burn.BurinOneProcessName == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "拼屏");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    decimal BalanceRate = burn.BurinOneStabdardOutputPerHour == 0 ? 0 : Math.Round(packStandardOutput / burn.BurinOneStabdardOutputPerHour * burn.ModuleNeedNum2, 2);//计算平衡率

                    total.Add("name", "拼屏");
                    total.Add("BalanceRate", BalanceRate);
                    result.Add(total);
                    total = new JObject();


                }
                if (burn.BurinTwoProcessName == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "拆屏");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    decimal BalanceRate = burn.BurinTwoStabdardOutputPerHour == 0 ? 0 : Math.Round(packStandardOutput / burn.BurinTwoStabdardOutputPerHour * burn.ModuleNeedNum2, 2);//计算平衡率

                    total.Add("name", "拆屏");
                    total.Add("BalanceRate", BalanceRate);
                    result.Add(total);
                    total = new JObject();

                }
            }
            //包装
            var pack = db.ProcessBalance.Where(c => c.Type == Type && c.Platform == Platform && c.ProductPCBnumber == ProductPCBnumber && c.Section == "包装").Select(c => c.Title).Distinct().ToList();//根据平台,类型PCB板查找包装数据
            foreach (var item in pack)
            {
                if (item == "/")//如果是跳过,平衡率为0
                {
                    total.Add("name", "包装");
                    total.Add("BalanceRate", 0);
                    result.Add(total);
                    total = new JObject();
                }
                else
                {
                    //最新一版本 上传excel表格的工段都有版本之分
                    var newSerialNumber = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Type == Type && c.ProductPCBnumber == ProductPCBnumber && c.Platform == Platform && c.Section == "包装" && c.Title == item).FirstOrDefault();
                    decimal BalanceRate = newSerialNumber.StandardOutput == 0 ? 0 : Math.Round(packStandardOutput / newSerialNumber.StandardOutput * newSerialNumber.ModuleNeedNum2, 2);//计算平衡率

                    total.Add("name", newSerialNumber.Title);
                    total.Add("BalanceRate", BalanceRate);
                    result.Add(total);
                    total = new JObject();
                }
            }
            return Content(JsonConvert.SerializeObject(result));
        }



        //贴片的输入和修改.现在 没用
        public ActionResult Add_PickmoduleNeed(string type, string productPCBnumber, string platform, decimal moduleNeed)
        {
            JObject value = new JObject();
            var oldinfo = db.Pick_And_Place.Where(c => c.Type == type && c.ProductPCBnumber == productPCBnumber && c.Platform == platform).ToList();//查找需要修改的贴片数据组,因为值默认是0,所以输入和修改的方法是一样的
            if (oldinfo.Count == 0)//如果找不到数据
            {
                value.Add("message", false);
                value.Add("content", "修改失败，找不到对应的文件");
                return Content(JsonConvert.SerializeObject(value));
            }
            //日志填写
            UserOperateLog log = new UserOperateLog()
            {
                Operator = ((Users)Session["User"]).UserName,
                OperateDT = DateTime.Now,
                OperateRecord =
            "修改工序产能数据，工段SMT贴片,原模组需求数量" + oldinfo.FirstOrDefault().ModuleNeedNum2 + "->" + moduleNeed
            };
            db.UserOperateLog.Add(log);

            //修改值
            // oldinfo.ForEach(c => c.ModuleNeedNum2 = moduleNeed);
            db.SaveChanges();
            value.Add("message", true);
            value.Add("content", "修改成功");
            return Content(JsonConvert.SerializeObject(value));
        }


        /// <summary>
        /// 显示不同模组的每小时产能
        /// </summary>
        /// <returns></returns>
        public ActionResult GetInfoByModule(string type, string plafrom)
        {
            JArray totalresult = new JArray();
            //var total = db.Process_Capacity_Total.ToList();//找到所有的平台,型号,PCB板 
            //if (!string.IsNullOrEmpty(type))
            //{
            //    total = total.Where(c => c.Type == type).ToList();
            //}
            //if (!string.IsNullOrEmpty(plafrom))
            //{
            //    total = total.Where(c => c.Platform == plafrom).ToList();
            //}
            //#region 取值
            ////SMT
            //var SmtList = db.Pick_And_Place.Select(c => new TempSmt { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, PersonNum = c.PersonNum, CapacityPerHour = c.CapacityPerHour, ProcessDescription = c.ProcessDescription }).ToList();
            ////插件
            //var PluginList = db.Process_Capacity_Plugin.Select(c => new TempCapacity { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.PluginStandardCapacity, StandardNumber = c.PluginStandardNumber }).ToList();
            ////平衡卡
            //var Balance = db.ProcessBalance.Select(c => new TempBlance { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.StandardOutput, StandardTotal = c.StandardTotal, Section = c.Section, SerialNumber = c.SerialNumber, Name = c.Title }).ToList();
            //////三防
            ////var ThreeProfList = db.Process_Capacity_ThreeProf.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.ThreeProfStabdardOutput, StandardNumber = c.ThreeProfStandardTotal });
            ////喷墨
            //var InkjetList = db.Process_Capacity_Inkjet.Select(c => new TempCapacity { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.InkjetStabdardOutputPerHour, StandardNumber = c.InkjetSuctionStandardTotal }).ToList();
            ////灌胶
            //var GlueList = db.Process_Capacity_Glue.Select(c => new TempCapacity { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.GlueStabdardOutput, StandardNumber = c.GlueStandardTotal }).ToList();
            ////气密
            //var AirtightList = db.Process_Capacity_Airtight.Select(c => new TempCapacity { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.AirtightStabdardOutput, StandardNumber = c.AirtightStandardTotal }).ToList();
            ////老化
            //var BurnList = db.Process_Capacity_Burin.Select(c => new TempBurn { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, BurinOneStabdardOutputPerHour = c.BurinOneStabdardOutputPerHour, BurninOneSuctionStandardTotal = c.BurninOneSuctionStandardTotal, BurinTwoStabdardOutputPerHour = c.BurinTwoStabdardOutputPerHour, BurinTwoSuctionStandardTotal = c.BurinTwoSuctionStandardTotal }).ToList();
            //#endregion
            //foreach (var item in total)
            //{
            //    JObject result = new JObject();
            //    JArray list = new JArray();
            //    JArray platformModule = new JArray();
            //    result.Add("platfrom", item.Platform);//平台
            //    result.Add("type", item.Type);//型号
            //    result.Add("PCB", item.ProductPCBnumber);//pcb板
            //    result.Add("PlatformModul", item.PlatformModul);//平台模块
            //    result.Add("ModuleUnits", 1);//模组单位

            //    #region 得出关联表的最后端数据列表
            //    var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == item.Type && c.Platform == item.Platform && c.PlatformModul == item.PlatformModul).ToList();//找到符合条件的所以关联表
            //    var relevancelast = new List<Process_Capacity_Relevance>();
            //    foreach (var relevanceitem in relevance)
            //    {
            //        var isfirst = relevance.Count(c => c.FatherID == relevanceitem.ChildID && c.FatherSeaction == relevanceitem.ChildSeaction);//查看当前id 是否是最开始有选项的
            //        if (isfirst == 0)//是最开始有选项的
            //        {
            //            relevancelast.Add(relevanceitem);
            //        }
            //    }
            //    #endregion

            //    JArray content = new JArray();
            //    if (relevancelast.Count == 0)//没找到关联关系,直接计算输出值
            //    {
            //        JObject itemvalue = CalculateCapacityPerHour(SmtList, PluginList, Balance, InkjetList, GlueList, AirtightList, BurnList, false, null, item.Platform, item.Type, item.ProductPCBnumber, item.PlatformModul);//调用通用方法,得到值

            //        JObject jobjectitem = new JObject();
            //        jobjectitem.Add("person", itemvalue["person"]);//所需人数
            //        jobjectitem.Add("processingFee", itemvalue["processingFee"]);//加工费用
            //        jobjectitem.Add("capacityPerHour", itemvalue["capacityPerHour"]);//每小时产能
            //        jobjectitem.Add("moduleName", itemvalue["moduleName"]);//关联名
            //        if (itemvalue["listitem"].Count() != 0)//总关联名
            //        {
            //            list.Add(itemvalue["listitem"]);
            //        }
            //        content.Add(jobjectitem);
            //    }
            //    else//找到关联关系,循环
            //    {
            //        JArray totalRelevance = new JArray();
            //        foreach (var lastitem in relevancelast)//循环关系表最后端的关联数据
            //        {
            //            JArray oneRelevance = new JArray();
            //            oneRelevance = FindRelevanceList(lastitem);//拿到完整的一条关系链
            //            JObject lastNoode = new JObject();
            //            lastNoode.Add("id", lastitem.ChildID);//当前关系链最后的一个节点
            //            lastNoode.Add("seaction", lastitem.ChildSeaction);
            //            var addNoode = ChildeInArray(oneRelevance, lastNoode);//将最后的节点往关系链放
            //            bool check = false;
            //            var relevanceFinsh = CheckValueCleanBrackets(addNoode, ref check);//处理好的关系链
            //            if (check)
            //            {
            //                foreach (var iii in relevanceFinsh)
            //                {
            //                    totalRelevance.Add(iii);
            //                }
            //            }
            //            else
            //            {
            //                totalRelevance.Add(relevanceFinsh);
            //            }
            //        }
            //        foreach (JArray moduleitem in totalRelevance)
            //        {
            //            #region 汇总内容
            //            JObject itemvalue = CalculateCapacityPerHour(SmtList, PluginList, Balance, InkjetList, GlueList, AirtightList, BurnList, true, moduleitem, item.Platform, item.Type, item.ProductPCBnumber, item.PlatformModul);

            //            JObject jobjectitem = new JObject();
            //            jobjectitem.Add("person", itemvalue["person"]);//所需人数
            //            jobjectitem.Add("processingFee", itemvalue["processingFee"]);//加工费用
            //            jobjectitem.Add("capacityPerHour", itemvalue["capacityPerHour"]);//每小时产能
            //            jobjectitem.Add("moduleName", itemvalue["moduleName"]);//关联名
            //            if (itemvalue["listitem"].Count() != 0)//总关联名
            //            {
            //                list.Add(itemvalue["listitem"]);
            //            }

            //            content.Add(jobjectitem);
            //            #endregion
            //            #region 详细内容

            //            #endregion
            //        }
            //    }
            //    result.Add("conten", content);
            //    result.Add("list", list);
            //    totalresult.Add(result);
            //}

            return Content(JsonConvert.SerializeObject(totalresult));
        }

        /// <summary>
        /// 找到平衡表的最后一个版本的集合
        /// </summary>
        /// <param name="value">平衡表集合</param>
        /// 根据传过来的平衡表数据,根据文件名进行分组,循环文件名,根据文件名找到最大的版本号,根据文件名和版本号,找到最后一个版本的数据信息,保存数据信息,最后传出去
        /// <returns></returns>
        public List<TempBlance> GetNewSerinum(List<TempBlance> value)
        {
            List<TempBlance> result = new List<TempBlance>();
            var serinum = value.Select(c => c.Name).Distinct();//查找数据里面excel文件的文件名,文件名相同归为是同一份文件的不同版本
            foreach (var item in serinum)//循环文件名
            {
                var temp = value.Where(c => c.Name == item).Max(c => c.SerialNumber);//根据文件名,找到该版本的最大版本号
                var resultitem = value.Where(c => c.Name == item && c.SerialNumber == temp).FirstOrDefault();//根据文件名和版本号,找到最后一个版本的数据信息
                result.Add(resultitem);//返回数据信息
            }
            return result;
        }

        /// <summary>
        /// 找到各个大块的每小时产能,SMT,模块,模组,老化,包装
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCapacity(string type, string plafrom)
        {
            JArray totalresult = new JArray();
            //var total = db.Process_Capacity_Total.ToList();//找到所有的平台,型号,PCB板 
            //if (!string.IsNullOrEmpty(type))
            //{
            //    total = total.Where(c => c.Type == type).ToList();
            //}
            //if (!string.IsNullOrEmpty(plafrom))
            //{
            //    total = total.Where(c => c.Platform == plafrom).ToList();
            //}
            //#region 取值
            ////SMT
            //var SmtList = db.Pick_And_Place.Select(c => new TempSmt { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, PersonNum = c.PersonNum, CapacityPerHour = c.CapacityPerHour }).ToList();
            ////插件
            //var PluginList = db.Process_Capacity_Plugin.Select(c => new TempCapacity { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.PluginStandardCapacity, StandardNumber = c.PluginStandardNumber }).ToList();
            ////平衡卡
            //var Balance = db.ProcessBalance.Select(c => new TempBlance { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.StandardOutput, StandardTotal = c.StandardTotal, Section = c.Section, SerialNumber = c.SerialNumber, Name = c.Title }).ToList();

            ////喷墨
            //var InkjetList = db.Process_Capacity_Inkjet.Select(c => new TempCapacity { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.InkjetStabdardOutputPerHour, StandardNumber = c.InkjetSuctionStandardTotal }).ToList();
            ////灌胶
            //var GlueList = db.Process_Capacity_Glue.Select(c => new TempCapacity { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.GlueStabdardOutput, StandardNumber = c.GlueStandardTotal }).ToList();
            ////气密
            //var AirtightList = db.Process_Capacity_Airtight.Select(c => new TempCapacity { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.AirtightStabdardOutput, StandardNumber = c.AirtightStandardTotal }).ToList();
            ////老化
            //var BurnList = db.Process_Capacity_Burin.Select(c => new TempBurn { Id = c.Id, Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, BurinOneStabdardOutputPerHour = c.BurinOneStabdardOutputPerHour, BurninOneSuctionStandardTotal = c.BurninOneSuctionStandardTotal, BurinTwoStabdardOutputPerHour = c.BurinTwoStabdardOutputPerHour, BurinTwoSuctionStandardTotal = c.BurinTwoSuctionStandardTotal }).ToList();
            //#endregion
            //foreach (var item in total)
            //{
            //    JObject result = new JObject();
            //    JArray list = new JArray();
            //    result.Add("platfrom", item.Platform);//平台
            //    result.Add("type", item.Type);//型号
            //    result.Add("PCB", item.ProductPCBnumber);//pcb板
            //    result.Add("PlatformModul", item.PlatformModul);//平台模块
            //    #region 得出关联表的最后端数据列表
            //    var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == item.Type && c.Platform == item.Platform && c.PlatformModul == item.PlatformModul).ToList();//找到符合条件的所以关联表
            //    var relevancelast = new List<Process_Capacity_Relevance>();
            //    foreach (var relevanceitem in relevance)
            //    {
            //        var isfirst = relevance.Count(c => c.FatherID == relevanceitem.ChildID && c.FatherSeaction == relevanceitem.ChildSeaction);//查看当前id 是否是最开始有选项的
            //        if (isfirst == 0)//是最开始有选项的
            //        {
            //            relevancelast.Add(relevanceitem);
            //        }
            //    }
            //    #endregion

            //    JArray content = new JArray();
            //    if (relevancelast.Count == 0)//没有找到关联
            //    {
            //        JObject itemvalue = CalculateCapacityPerHour(SmtList, PluginList, Balance, InkjetList, GlueList, AirtightList, BurnList, false, null, item.Platform, item.Type, item.ProductPCBnumber, item.PlatformModul);

            //        JObject jobjectitem = new JObject();
            //        jobjectitem.Add("smtcapacityPerHour", itemvalue["smtcapacityPerHour"]);//smt每小时产能
            //        jobjectitem.Add("modulescapacityPerHour", itemvalue["modulescapacityPerHour"]);//模块每小时产能
            //        jobjectitem.Add("modulecapacityPerHour", itemvalue["modulecapacityPerHour"]);//模组每小时产能
            //        jobjectitem.Add("burncapacityPerHour", itemvalue["burncapacityPerHour"]);//老化每小时产能
            //        jobjectitem.Add("包装capacityPerHour", itemvalue["包装capacityPerHour"]);//包装每小时产能

            //        jobjectitem.Add("moduleName", itemvalue["moduleName"]);//工艺名字
            //        if (itemvalue["listitem"].Count() != 0)//总关联名
            //        {
            //            list.Add(itemvalue["listitem"]);
            //        }
            //        content.Add(jobjectitem);
            //    }
            //    else//有关联,循环关联
            //    {
            //        JArray totalRelevance = new JArray();
            //        foreach (var lastitem in relevancelast)//循环关系表最后端的关联数据
            //        {
            //            JArray oneRelevance = new JArray();
            //            oneRelevance = FindRelevanceList(lastitem);//拿到完整的一条关系链
            //            JObject lastNoode = new JObject();
            //            lastNoode.Add("id", lastitem.ChildID);//当前关系链最后的一个节点
            //            lastNoode.Add("seaction", lastitem.ChildSeaction);
            //            var addNoode = ChildeInArray(oneRelevance, lastNoode);//将最后的节点往关系链放
            //            bool check = false;
            //            var relevanceFinsh = CheckValueCleanBrackets(addNoode, ref check);//处理好的关系链
            //            if (check)
            //            {
            //                foreach (var iii in relevanceFinsh)
            //                {
            //                    totalRelevance.Add(iii);
            //                }
            //            }
            //            else
            //            {
            //                totalRelevance.Add(relevanceFinsh);
            //            }
            //        }
            //        foreach (JArray moduleitem in totalRelevance)
            //        {
            //            JObject itemvalue = CalculateCapacityPerHour(SmtList, PluginList, Balance, InkjetList, GlueList, AirtightList, BurnList, true, moduleitem, item.Platform, item.Type, item.ProductPCBnumber, item.PlatformModul);

            //            JObject jobjectitem = new JObject();
            //            jobjectitem.Add("smtcapacityPerHour", itemvalue["smtcapacityPerHour"]);//smt每小时产能
            //            jobjectitem.Add("modulescapacityPerHour", itemvalue["modulescapacityPerHour"]);//模块每小时产能
            //            jobjectitem.Add("modulecapacityPerHour", itemvalue["modulecapacityPerHour"]);//模组每小时产能
            //            jobjectitem.Add("burncapacityPerHour", itemvalue["burncapacityPerHour"]);//老化每小时产能
            //            jobjectitem.Add("包装capacityPerHour", itemvalue["包装capacityPerHour"]);//包装每小时产能

            //            jobjectitem.Add("moduleName", itemvalue["moduleName"]);//工艺名字
            //            if (itemvalue["listitem"].Count() != 0)//总关联名
            //            {
            //                list.Add(itemvalue["listitem"]);
            //            }
            //            content.Add(jobjectitem);

            //        }
            //    }
            //    result.Add("conten", content);
            //    result.Add("list", list);//模组名称列表
            //    totalresult.Add(result);
            //}

            return Content(JsonConvert.SerializeObject(totalresult));
        }

        /// <summary>
        /// 计算每小时产能共用方法
        /// </summary>
        /// <param name="SmtList">SMT数据集</param>
        /// <param name="PluginList">插件数据集</param>
        /// <param name="Balance">平衡表数据及集</param>
        /// <param name="InkjetList">喷墨数据集</param>
        /// <param name="GlueList">灌胶数据集</param>
        /// <param name="AirtightList">气密数据集</param>
        /// <param name="BurnList">老化数据集</param>
        /// <param name="isChange">是否有关联</param>
        /// <param name="moduleitem">关联数据</param>
        /// <param name="Platform">平台</param>
        /// <param name="type">型号</param>
        /// <param name="ProductPCBnumber">PCB</param>
        /// <param name="platmodule">平台模块</param>
        /// <returns></returns>
        public JObject CalculateCapacityPerHour(List<TempSmt> SmtList, List<TempCapacity> PluginList, List<TempBlance> Balance, List<TempCapacity> InkjetList, List<TempCapacity> GlueList, List<TempCapacity> AirtightList, List<TempCapacity> BurnList, bool isChange, JArray moduleitem, string Platform, string type, string ProductPCBnumber, string platmodule)
        {
            JObject jobjectitem = new JObject();
            //JArray listitem = new JArray();
            //var seaction = new List<string>();
            //List<RelevanceList> relajaaray = new List<RelevanceList>();
            //if (isChange)
            //{
            //    relajaaray = moduleitem.ToObject<List<RelevanceList>>();
            //    var Relevancename = GetName(relajaaray);//得到所有的工序描述,前端显示
            //    seaction = relajaaray.Select(c => c.seaction).ToList();//得到所有的工段,后端判断读取
            //    listitem.Add(Relevancename);//前端显示
            //    jobjectitem.Add("listitem", listitem);                        //list.Add(listitem);
            //    jobjectitem.Add("moduleName", JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(Relevancename)));//模组名称
            //}
            //else
            //{
            //    jobjectitem.Add("listitem", listitem);                        //list.Add(listitem);
            //    jobjectitem.Add("moduleName", listitem);//模组名称
            //}
            //#region 取值所需人数计算,需要拿到改平台的所有工段的标准人数
            ////模组装配
            //var module = new ProcessBalance();
            //if (seaction.Contains("模组装配"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "模组装配").Select(c => c.id).FirstOrDefault();
            //    module = db.ProcessBalance.Where(c => c.Id == id).FirstOrDefault();
            //}
            //else
            //{
            //    module = db.ProcessBalance.OrderByDescending(c => c.SerialNumber).Where(c => c.Platform == Platform && c.Type == type && c.ProductPCBnumber == ProductPCBnumber && c.Section == "模组装配").FirstOrDefault();

            //}
            //if (module == null)
            //{
            //    jobjectitem.Add("person", 0);//所需人数
            //    jobjectitem.Add("processingFee", "");//加工费用
            //    jobjectitem.Add("capacityPerHour", 0);//每小时产能
            //    return jobjectitem;
            //}
            ////smt
            //var smt = new List<TempSmt>();
            //var smt1 = SmtList.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber && c.ProcessDescription == "IC面贴装").ToList();
            //var smt2 = SmtList.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber && c.ProcessDescription == "灯面贴装").Select(c => new { c.CapacityPerHour, c.PersonNum }).ToList();
            //if (smt1.Count != 0)
            //{
            //    smt.Add(smt1.OrderBy(c => c.PersonNum).Select(c => new TempSmt { CapacityPerHour = c.CapacityPerHour, PersonNum = c.PersonNum }).FirstOrDefault());
            //}
            //if (smt2.Count != 0)
            //{
            //    smt.Add(smt2.OrderBy(c => c.PersonNum).Select(c => new TempSmt { CapacityPerHour = c.CapacityPerHour, PersonNum = c.PersonNum }).FirstOrDefault());
            //}
            ////插件
            //var plugin = new List<Temp>();
            //if (seaction.Contains("插件"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "插件").Select(c => c.id).FirstOrDefault();
            //    plugin = PluginList.Where(c => c.Id == id).Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber }).ToList();
            //}
            //else
            //{
            //    plugin = PluginList.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber).Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber }).ToList();
            //}
            ////后焊
            //var after = new List<TempBlance>();
            //if (seaction.Contains("后焊"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "后焊").Select(c => c.id).FirstOrDefault();
            //    after = Balance.Where(c => c.Id == id).ToList();
            //}
            //else
            //{
            //    var aftertotal = Balance.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber && c.Section == "后焊").ToList();
            //    after = GetNewSerinum(aftertotal);
            //}
            ////三防
            //var three = new List<TempBlance>();
            //if (seaction.Contains("三防"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "三防").Select(c => c.id).FirstOrDefault();
            //    three = Balance.Where(c => c.Id == id).ToList();
            //}
            //else
            //{
            //    var threetotal = Balance.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber && c.Section == "三防").ToList();
            //    three = GetNewSerinum(threetotal);
            //}
            ////打底壳
            //var botton = new List<TempBlance>();
            //if (seaction.Contains("打底壳"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "打底壳").Select(c => c.id).FirstOrDefault();
            //    botton = Balance.Where(c => c.Id == id).ToList();
            //}
            //else
            //{
            //    var bottontotal = Balance.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber && c.Section == "打底壳").ToList();
            //    botton = GetNewSerinum(bottontotal);
            //}
            ////装磁吸
            //var magneti = new List<TempBlance>();
            //if (seaction.Contains("装磁吸板"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "装磁吸板").Select(c => c.id).FirstOrDefault();
            //    magneti = Balance.Where(c => c.Id == id).ToList();
            //}
            //else
            //{
            //    var magnetitotal = Balance.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber && c.Section == "装磁吸板").ToList();
            //    magneti = GetNewSerinum(magnetitotal);
            //}
            ////喷墨
            //var Inkjet = new List<Temp>();
            //if (seaction.Contains("喷墨"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "喷墨").Select(c => c.id).FirstOrDefault();
            //    Inkjet = InkjetList.Where(c => c.Id == id).Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber }).ToList();
            //}
            //else
            //{
            //    Inkjet = InkjetList.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber).Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber }).ToList();
            //}
            ////模块线
            //var moduleLine = new List<TempBlance>();
            //if (seaction.Contains("模块线"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "模块线").Select(c => c.id).FirstOrDefault();
            //    moduleLine = Balance.Where(c => c.Id == id).ToList();
            //}
            //else
            //{
            //    var moduleLinetotal = Balance.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber && c.Section == "模块线").ToList();
            //    moduleLine = GetNewSerinum(moduleLinetotal);
            //}
            ////灌胶
            //var glue = new List<Temp>();
            //if (seaction.Contains("灌胶"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "灌胶").Select(c => c.id).FirstOrDefault();
            //    glue = GlueList.Where(c => c.Id == id).Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber }).ToList();
            //}
            //else
            //{
            //    glue = GlueList.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber).Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber }).ToList();
            //}
            ////气密
            //var airtight = new List<Temp>();
            //if (seaction.Contains("气密"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "气密").Select(c => c.id).FirstOrDefault();
            //    airtight = AirtightList.Where(c => c.Id == id).Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber }).ToList();
            //}
            //else
            //{
            //    airtight = AirtightList.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber).Select(c => new Temp { StandardCapacity = c.StandardCapacity, StandardNumber = c.StandardNumber }).ToList();
            //}
            ////锁面罩
            //var lockmas = new List<TempBlance>();
            //if (seaction.Contains("锁面罩"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "锁面罩").Select(c => c.id).FirstOrDefault();
            //    lockmas = Balance.Where(c => c.Id == id).ToList();
            //}
            //else
            //{
            //    var lockmastotal = Balance.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber && c.Section == "锁面罩").ToList();
            //    lockmas = GetNewSerinum(lockmastotal);
            //}

            ////老化
            //var burnin = new List<TempBurn>();
            //if (seaction.Contains("气密"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "气密").Select(c => c.id).FirstOrDefault();
            //    burnin = BurnList.Where(c => c.Id == id).ToList();
            //}
            //else
            //{
            //    burnin = BurnList.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber).ToList();
            //}
            ////包装
            //var packing = new List<TempBlance>();
            //if (seaction.Contains("包装"))
            //{
            //    int id = relajaaray.Where(c => c.seaction == "包装").Select(c => c.id).FirstOrDefault();
            //    packing = Balance.Where(c => c.Id == id).ToList();
            //}
            //else
            //{
            //    var packingtotal = Balance.Where(c => c.Plafrom == Platform && c.Type == type && c.PCB == ProductPCBnumber && c.Section == "包装").ToList();
            //    packing = GetNewSerinum(packingtotal);
            //}
            //#endregion
            //var smtpersonNum = 0;//smt总人数
            //decimal smtWarkHours = 0m; //smt每小时产能
            //decimal smtCapacityPerHour = 0m;//smt标准产能

            //var pluginpersonNum = 0;//插件总人数
            //decimal pluginWarkHours = 0m; //插件每小时产能
            //decimal pluginCapacityPerHour = 0m;//插件标准产能

            //var afterpersonNum = 0;//后焊总人数
            //decimal afterWarkHours = 0m; //后焊每小时产能
            //decimal afterCapacityPerHour = 0m;//后焊标准产能

            //var threePersonNum = 0;//后焊总人数
            //decimal threeWarkHours = 0m; //后焊每小时产能
            //decimal threeCapacityPerHour = 0m;//后焊标准产能

            //var bottonPersonNum = 0;//后焊总人数
            //decimal bottonWarkHours = 0m; //后焊每小时产能
            //decimal bottonCapacityPerHour = 0m;//后焊标准产能

            //var magnetiPersonNum = 0;//后焊总人数
            //decimal magnetiWarkHours = 0m; //后焊每小时产能
            //decimal magnetiCapacityPerHour = 0m;//后焊标准产能

            //var totalpersonNum = 0;//总人数
            //decimal totalWarkHours = 0m;

            //var smttotalpersonNum = 0;//smt总人数
            //decimal smttotalWarkHours = 0m; //smt每小时产能

            //var mudulestotalpersonNum = 0;//总人数
            //decimal mudulestotalWarkHours = 0m;//模块每小时产能

            //var muduletotalpersonNum = 0;//总人数
            //decimal moduletotalWarkHours = 0m;//每小时产能

            //var burntotalpersonNum = 0;//总人数
            //decimal burntotalWarkHours = 0m;//每小时产能

            //var packtotalpersonNum = 0;//总人数
            //decimal packtotalWarkHours = 0m;//每小时产能
            //#region 循环各个工段的数据,拿到人数集合,和各个工段的单人模组需求总时(1/(标准产能/人数/模组需求))
            ////smt
            //smt.ForEach(c => { smtpersonNum = smtpersonNum + c.PersonNum; smtCapacityPerHour = smtCapacityPerHour + c.CapacityPerHour; smtWarkHours = smtWarkHours + (c.PersonNum == 0 || module.SMTModuleNeedNum == 0 ? 0 : (1 / (c.CapacityPerHour / c.PersonNum / module.SMTModuleNeedNum))); });
            ////插件
            //plugin.ForEach(c => { pluginpersonNum = pluginpersonNum + c.StandardNumber; pluginCapacityPerHour = pluginCapacityPerHour + c.StandardCapacity; pluginWarkHours = pluginWarkHours + (c.StandardNumber == 0 || module.PluginModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardNumber / module.PluginModuleNeedNum))); });
            ////后焊
            //after.ForEach(c => { afterpersonNum = afterpersonNum + c.StandardTotal; afterCapacityPerHour = afterCapacityPerHour + c.StandardCapacity; afterWarkHours = afterWarkHours + (c.StandardTotal == 0 || module.AfterModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardTotal / module.AfterModuleNeedNum))); });
            ////三放
            //three.ForEach(c => { mudulestotalpersonNum = mudulestotalpersonNum + c.StandardTotal; mudulestotalWarkHours = mudulestotalWarkHours + (c.StandardTotal == 0 || module.ThreeModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardTotal / module.ThreeModuleNeedNum))); });
            ////打底壳
            //botton.ForEach(c => { mudulestotalpersonNum = mudulestotalpersonNum + c.StandardTotal; mudulestotalWarkHours = mudulestotalWarkHours + (c.StandardTotal == 0 || module.BottnModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardTotal / module.BottnModuleNeedNum))); });
            ////装磁吸
            //magneti.ForEach(c => { mudulestotalpersonNum = mudulestotalpersonNum + c.StandardTotal; mudulestotalWarkHours = mudulestotalWarkHours + (c.StandardTotal == 0 || module.MagneticModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardTotal / module.MagneticModuleNeedNum))); });
            ////喷墨
            //Inkjet.ForEach(c => { mudulestotalpersonNum = mudulestotalpersonNum + c.StandardNumber; mudulestotalWarkHours = mudulestotalWarkHours + (c.StandardNumber == 0 || module.InjekModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardNumber / module.InjekModuleNeedNum))); });
            ////灌胶
            //glue.ForEach(c => { mudulestotalpersonNum = mudulestotalpersonNum + c.StandardNumber; mudulestotalWarkHours = mudulestotalWarkHours + (c.StandardNumber == 0 || module.GuleModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardNumber / module.GuleModuleNeedNum))); });
            ////气密
            //airtight.ForEach(c => { mudulestotalpersonNum = mudulestotalpersonNum + c.StandardNumber; mudulestotalWarkHours = mudulestotalWarkHours + (c.StandardNumber == 0 || module.AirtightModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardNumber / module.AirtightModuleNeedNum))); });
            ////锁面罩
            //lockmas.ForEach(c => { mudulestotalpersonNum = mudulestotalpersonNum + c.StandardTotal; mudulestotalWarkHours = mudulestotalWarkHours + (c.StandardTotal == 0 || module.LockMasModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardTotal / module.LockMasModuleNeedNum))); });
            ////模块线
            //moduleLine.ForEach(c => { mudulestotalpersonNum = mudulestotalpersonNum + c.StandardTotal; mudulestotalWarkHours = mudulestotalWarkHours + (c.StandardTotal == 0 || module.LockMasModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardTotal / module.LockMasModuleNeedNum))); });
            ////模组装配
            //muduletotalpersonNum = muduletotalpersonNum + module.StandardTotal;
            //moduletotalWarkHours = moduletotalWarkHours + (module.StandardTotal == 0 || module.ModuleNeedNum2 == 0 ? 0 : (1 / (module.StandardOutput / module.StandardTotal / module.ModuleNeedNum2)));
            ////老化
            //burnin.ForEach(c => { burntotalpersonNum = burntotalpersonNum + c.BurninOneSuctionStandardTotal; burntotalWarkHours = burntotalWarkHours + (c.BurninOneSuctionStandardTotal == 0 || module.BuriInModuleNeedNum == 0 ? 0 : (1 / (c.BurinOneStabdardOutputPerHour / c.BurninOneSuctionStandardTotal / module.BuriInModuleNeedNum))); burntotalpersonNum = burntotalpersonNum + c.BurinTwoSuctionStandardTotal; burntotalWarkHours = burntotalWarkHours + (c.BurinTwoSuctionStandardTotal == 0 || module.BuriInModuleNeedNum == 0 ? 0 : (1 / (c.BurinTwoStabdardOutputPerHour / c.BurinTwoSuctionStandardTotal / module.BuriInModuleNeedNum))); });
            ////包装
            //packing.ForEach(c => { packtotalpersonNum = packtotalpersonNum + c.StandardTotal; packtotalWarkHours = packtotalWarkHours + (c.StandardTotal == 0 || module.PackModuleNeedNum == 0 ? 0 : (1 / (c.StandardCapacity / c.StandardTotal / module.PackModuleNeedNum))); });
            //#endregion
            //smttotalWarkHours = smtWarkHours + pluginWarkHours + afterWarkHours;
            //smttotalpersonNum = smtpersonNum + pluginpersonNum + afterpersonNum;
            //jobjectitem.Add("smtcapacityPerHour", smttotalWarkHours == 0 ? 0 : Math.Round(smttotalpersonNum / smttotalWarkHours, 2));//smt每小时产能
            //jobjectitem.Add("modulescapacityPerHour", mudulestotalWarkHours == 0 ? 0 : Math.Round(mudulestotalpersonNum / mudulestotalWarkHours, 2));//模块每小时产能

            //jobjectitem.Add("modulecapacityPerHour", moduletotalWarkHours == 0 ? 0 : Math.Round(muduletotalpersonNum / moduletotalWarkHours, 2));//模组每小时产能
            //jobjectitem.Add("burncapacityPerHour", burntotalWarkHours == 0 ? 0 : Math.Round(burntotalpersonNum / burntotalWarkHours, 2));//老化每小时产能
            //jobjectitem.Add("包装capacityPerHour", packtotalWarkHours == 0 ? 0 : Math.Round(packtotalpersonNum / packtotalWarkHours, 2));//包装每小时产能
            //totalWarkHours = smttotalWarkHours + mudulestotalWarkHours + moduletotalWarkHours + burntotalWarkHours + packtotalWarkHours;
            //totalpersonNum = smttotalpersonNum + mudulestotalpersonNum + muduletotalpersonNum + burntotalpersonNum + packtotalpersonNum;
            //jobjectitem.Add("person", totalpersonNum);//所需人数
            //jobjectitem.Add("processingFee", "");//加工费用
            //jobjectitem.Add("capacityPerHour", totalWarkHours == 0 ? 0 : Math.Round(totalpersonNum / totalWarkHours, 2));//每小时产能
            return jobjectitem;
        }

        #endregion

        #region 不要的

        /// <summary>
        /// 根据传过来的id和工段组合,拿到工序描述
        /// </summary>
        /// <param name="value">id和工段组合</param>
        /// <returns></returns>
        //public List<string> GetName(List<RelevanceList> value)
        //{
        //    List<string> namelist = new List<string>();
        //    //string gulelname = "";
        //    //string threename = "";
        //    //var moduleName = "";
        //    //var packName = "";

        //    //三防
        //    //var threenameList = db.ProcessBalance.Where(c => c.Platform == plafrom && c.Type == type && c.PlatformModul == plafmodule && c.Section == "三防").Select(c => new { c.Title, c.ProcessDescription }).ToList();
        //    //threenameList.ForEach(c => threename = threename + (c.ProcessDescription != null ? c.ProcessDescription : c.Title) + "+");
        //    //灌胶
        //    //var glueNameList = db.Process_Capacity_Glue.Where(c => c.Platform == plafrom && c.Type == type && c.PlatformModul == plafmodule).Select(c => c.GlueProcessName).ToList();
        //    //glueNameList.ForEach(c => gulelname = gulelname + c + "+");

        //    //模组装配
        //    //var moduleNameList = db.ProcessBalance.Where(c => c.Platform == plafrom && c.Type == type && c.PlatformModul == plafmodule && c.Section == "模组装配").Select(c => new { c.Title, c.ProcessDescription }).ToList();
        //    //moduleNameList.ForEach(c => moduleName = moduleName + (c.ProcessDescription != null ? c.ProcessDescription : c.Title) + "+");
        //    //包装
        //    //var packNameList = db.ProcessBalance.Where(c => c.Platform == plafrom && c.Type == type && c.PlatformModul == plafmodule && c.Section == "包装").Select(c => new { c.Title, c.ProcessDescription }).ToList();
        //    //packNameList.ForEach(c => packName = packName + (c.ProcessDescription != null ? c.ProcessDescription : c.Title) + "+");


        //    foreach (var item in value)//循环组合
        //    {
        //        string result = "";
        //        switch (item.seaction)//switch工段
        //        {
        //            case "插件":
        //                var name = db.Process_Capacity_Plugin.Where(c => c.Id == item.id).Select(c => c.Name).FirstOrDefault();
        //                result = name;
        //                break;
        //            case "喷墨":
        //                var name2 = db.Process_Capacity_Inkjet.Where(c => c.Id == item.id).Select(c => c.InkjetProcessName).FirstOrDefault();
        //                result = name2;
        //                break;
        //            case "灌胶":
        //                var name3 = db.Process_Capacity_Glue.Where(c => c.Id == item.id).Select(c => c.GlueProcessName).FirstOrDefault();
        //                result = name3;
        //                break;
        //            case "气密":
        //                var name4 = db.Process_Capacity_Airtight.Where(c => c.Id == item.id).Select(c => c.AirtightProcessName).FirstOrDefault();
        //                result = name4;
        //                break;
        //            case "老化":
        //                var name5 = db.Process_Capacity_Burin.Where(c => c.Id == item.id).Select(c => c.BurinOneProcessName).FirstOrDefault();
        //                result = name5;
        //                break;
        //            case "后焊":
        //            case "三防":
        //            case "打底壳":
        //            case "装磁吸安装板":
        //            case "锁面罩":
        //            case "模组装配":
        //            case "包装":
        //            case "模块线":
        //                var name6 = db.ProcessBalance.Where(c => c.Id == item.id).Select(c => new { c.ProcessDescription, c.Title }).FirstOrDefault(); ;
        //                var name7 = name6.ProcessDescription != null ? name6.ProcessDescription : name6.Title;
        //                result = name7;
        //                break;
        //        }
        //        namelist.Add(result);
        //    }
        //    return namelist;
        //}
        //public List<RelevanceList> FindRelevanceList(Process_Capacity_Relevance value)
        //{
        //    List<RelevanceList> result = new List<RelevanceList>();
        //    var black = db.Process_Capacity_Relevance.Where(c => c.ChildID == value.FatherID && c.ChildSeaction == value.FatherSeaction).ToList();//查找有没有前端关联,没有就直接返回本数据,有则再循环,直到没找到前端数据

        //    RelevanceList relevance = new RelevanceList() { id = value.ChildID, seaction = value.ChildSeaction };
        //    if (black.Count() != 0)//
        //    {
        //        if (black.Count > 1)
        //        {
        //            foreach (var item in black)
        //            {
        //                result.AddRange(FindRelevanceList(black.FirstOrDefault()));

        //            }
        //        }
        //        else
        //        {
        //            result.AddRange(FindRelevanceList(black.FirstOrDefault()));//循环
        //        }

        //    }
        //    result.Add(relevance);//添加到list 返回
        //    return result;
        //}
        //public JArray test(string Type, string platfrom, string platfrommodule)
        //{
        //    JArray result = new JArray();
        //    var relevance = db.Process_Capacity_Relevance.Where(c => c.Type == Type && c.Platform == platfrom && c.PlatformModul == platfrommodule).ToList();//找到符合条件的所以关联表
        //    var relevancelast = new List<Process_Capacity_Relevance>();
        //    foreach (var relevanceitem in relevance)
        //    {
        //        var isfirst = relevance.Count(c => c.FatherID == relevanceitem.ChildID && c.FatherSeaction == relevanceitem.ChildSeaction);//查看当前id 是否是最开始有选项的
        //        if (isfirst == 0)//是最开始有选项的
        //        {
        //            relevancelast.Add(relevanceitem);
        //        }
        //    }
        //    foreach (var item in relevancelast)
        //    {
        //        JArray result1 = new JArray();
        //        result1 = FindRelevanceList2(item);
        //        JObject aa = new JObject();
        //        aa.Add("id", item.ChildID);
        //        aa.Add("seaction", item.ChildSeaction);
        //        var bb = test3(result1, aa);
        //        bool check = false;
        //        var ff = test4(bb, ref check);
        //        if (check)
        //        {
        //            foreach (var iii in ff)
        //            {
        //                result.Add(iii);
        //            }
        //        }
        //        else
        //        {
        //            result.Add(ff);
        //        }
        //        List<RelevanceList> vv = new List<RelevanceList>();
        //        vv = result1.ToObject<List<RelevanceList>>();
        //    }

        //    return result;
        //}
        //总表显示
        //public ActionResult TotalProcess_Capacity()
        //{

        //    var type = db.Process_Capacity_Total.Select(c => c.Type).Distinct().ToList();
        //    JArray totel = new JArray();
        //    foreach (var item in type)
        //    {
        //        JObject result = new JObject();
        //        var info = db.Process_Capacity_Total.Where(c => c.Type == item).ToList();//
        //        result.Add("id", info.FirstOrDefault().Id);
        //        result.Add("Type", item);
        //        var platform = info.FirstOrDefault().Platform;
        //        var PCB = info.FirstOrDefault().ProductPCBnumber;
        //        result.Add("Platform", platform);
        //        result.Add("PCB", PCB);
        //        //ic面贴装 //灯面贴装
        //        if (db.Pick_And_Place.Count(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform) == 0)//判断是否有对应的excel文件
        //        {
        //            result.Add("icProductName", null);
        //            result.Add("icMaxStandardTotal", null);
        //            result.Add("icMaxStandardOutput", null);
        //            result.Add("icMinStandardTotal", null);
        //            result.Add("icMinStandardOutput", null);

        //            result.Add("LightProductName", null);
        //            result.Add("LightMaxStandardTotal", null);
        //            result.Add("LightMaxStandardOutput", null);
        //            result.Add("LightMinStandardTotal", null);
        //            result.Add("LightMinStandardOutput", null);
        //        }
        //        else
        //        {
        //            var icMaxStandardTotal = db.Pick_And_Place.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.ProcessDescription == "IC面贴装").Max(c => c.PersonNum);
        //            var icMaxStandardOutput = db.Pick_And_Place.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.ProcessDescription == "IC面贴装").Max(c => c.CapacityPerHour);
        //            var icMinStandardTotal = db.Pick_And_Place.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.ProcessDescription == "IC面贴装").Min(c => c.PersonNum);
        //            var icMinStandardOutput = db.Pick_And_Place.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.ProcessDescription == "IC面贴装").Min(c => c.CapacityPerHour);
        //            result.Add("icProductName", "IC面贴装");
        //            result.Add("icMaxStandardTotal", icMaxStandardTotal);
        //            result.Add("icMaxStandardOutput", icMaxStandardOutput);
        //            result.Add("icMinStandardTotal", icMinStandardTotal);
        //            result.Add("icMinStandardOutput", icMinStandardOutput); // result.Add("LimtProductName", true);
        //            var LightMaxStandardTotal = db.Pick_And_Place.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.ProcessDescription == "灯面贴装").Max(c => c.PersonNum);
        //            var LightMaxStandardOutput = db.Pick_And_Place.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.ProcessDescription == "灯面贴装").Max(c => c.CapacityPerHour);
        //            var LightMinStandardTotal = db.Pick_And_Place.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.ProcessDescription == "灯面贴装").Min(c => c.PersonNum);
        //            var LightMinStandardOutput = db.Pick_And_Place.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.ProcessDescription == "灯面贴装").Min(c => c.CapacityPerHour);
        //            result.Add("LightProductName","灯面贴装");
        //            result.Add("LightMaxStandardTotal", LightMaxStandardTotal);
        //            result.Add("LightMaxStandardOutput", LightMaxStandardOutput);
        //            result.Add("LightMinStandardTotal", LightMinStandardTotal);
        //            result.Add("LightMinStandardOutput", LightMinStandardOutput);
        //        }

        //        //SMT是否有图片或PDF
        //        List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\Process_Capacity\" + platform + "_" + type + "\\");
        //        var jpginfo = filesInfo.Where(c => c.Name.StartsWith("SMT") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
        //        if (jpginfo.Count > 0)
        //            result.Add("SMTjpg", true);
        //        else
        //            result.Add("SMTjpg", false);
        //        var pdfinfo = filesInfo.Where(c => c.Name.StartsWith("SMT") && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();
        //        if (pdfinfo.Count > 0)
        //            result.Add("SMTpdf", true);
        //        else
        //            result.Add("SMTpdf", false);


        //        //插件
        //        var plugin = db.Process_Capacity_Plugin.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform).ToList();
        //        if (plugin.Count == 0)
        //        {
        //            result.Add("PluginDevice", null);

        //        }
        //        else
        //        {
        //            JArray array = new JArray();
        //            foreach (var pluginitem in plugin)
        //            {
        //                JObject pluginjobject = new JObject();
        //                pluginjobject.Add("PluginDeviceID", pluginitem.Id);
        //                pluginjobject.Add("PluginDeviceName", pluginitem.Name); //插件工序名
        //                pluginjobject.Add("SingleLampWorkingHours", pluginitem.SingleLampWorkingHours);//插件机台固定标准单灯工时
        //                pluginjobject.Add("PCBASingleLampNumber", pluginitem.PCBASingleLampNumber);//插件PCBA单灯数
        //                pluginjobject.Add("PluginStandardNumber", pluginitem.PluginStandardNumber);//插件标配人数
        //                pluginjobject.Add("PluginStandardCapacity", pluginitem.PluginStandardCapacity);//插件产能标准
        //                array.Add(pluginjobject);
        //            }
        //            result.Add("PluginDevice", array);
        //        }
        //        //后焊
        //        var AfterWeldProcessName = db.ProcessBalance.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.Section == "后焊").ToList();
        //        if (AfterWeldProcessName.Count == 0)
        //        {
        //            result.Add("AfterWeld", null);
        //        }
        //        else
        //        {
        //            var title = AfterWeldProcessName.Select(c => c.Title).ToList();
        //            JArray AfterWeld = new JArray();
        //            foreach (var ProcessName in title)
        //            {
        //                JObject jObject = new JObject();
        //                var message = AfterWeldProcessName.OrderByDescending(c => c.SerialNumber).Where(c => c.Title == ProcessName).FirstOrDefault();
        //                jObject.Add("AfterWeldProcessName", message.Title);
        //                jObject.Add("AfterWeldStandardTotal", message.StandardTotal);
        //                jObject.Add("AfterWeldStandardOutput", message.StandardOutput);
        //                jObject.Add("AfterWeldStandardHourlyOutputPerCapita", message.StandardHourlyOutputPerCapita);
        //                AfterWeld.Add(jObject);
        //            }
        //            result.Add("AfterWeld", AfterWeld);
        //        }
        //        if (IsHavingPDF(item, platform, "后焊"))
        //            result.Add("AfterWeldPdf", true);
        //        else
        //            result.Add("AfterWeldPdf", false);
        //        if (IsHavingIMG(item, platform, "后焊"))
        //            result.Add("AfterWeldImg", true);
        //        else
        //            result.Add("AfterWeldImg", false);

        //        //三防
        //        var threeprof = db.Process_Capacity_ThreeProf.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform).ToList();
        //        if (threeprof.Count == 0)
        //        {
        //            result.Add("ThreeProf", null);

        //        }
        //        else
        //        {
        //            JArray array = new JArray();
        //            foreach (var threeprofitem in threeprof)
        //            {
        //                JObject jObject = new JObject();
        //                jObject.Add("ThreeProfID", threeprofitem.Id);
        //                jObject.Add("ThreeProfProcessName", threeprofitem.ThreeProfProcessName);//三防工序描述
        //                jObject.Add("ThreeProfStandardTotal", threeprofitem.ThreeProfStandardTotal);//三防标准总人数
        //                jObject.Add("ThreeProfStabdardOutput", threeprofitem.ThreeProfStabdardOutput);//三防标准产量
        //                array.Add(jObject);
        //            }
        //            result.Add("ThreeProf", array);
        //        }

        //        //打底壳
        //        var BottomCasProcessName = db.ProcessBalance.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.Section == "打底壳").ToList();
        //        if (BottomCasProcessName.Count == 0)
        //        {
        //            result.Add("BottomCas", null);
        //        }
        //        else
        //        {
        //            var title = BottomCasProcessName.Select(c => c.Title).ToList();
        //            JArray BottomCas = new JArray();
        //            foreach (var ProcessName in title)
        //            {
        //                JObject jObject = new JObject();
        //                var message = BottomCasProcessName.OrderByDescending(c => c.SerialNumber).Where(c => c.Title == ProcessName).FirstOrDefault();
        //                jObject.Add("BottomCasProcessName", message.Title);
        //                jObject.Add("BottomCasStandardTotal", message.StandardTotal);
        //                jObject.Add("BottomCasStandardOutput", message.StandardOutput);
        //                jObject.Add("BottomCasStandardHourlyOutputPerCapita", message.StandardHourlyOutputPerCapita);
        //                jObject.Add("BottomCasDispensMachineNum", message.DispensMachineNum);
        //                jObject.Add("BottomCasScrewMachineNum", message.ScrewMachineNum);
        //                BottomCas.Add(jObject);
        //            }
        //            result.Add("BottomCas", BottomCas);
        //        }
        //        if (IsHavingPDF(item, platform, "打底壳"))
        //            result.Add("ABottomCasPdf", true);
        //        else
        //            result.Add("BottomCasPdf", false);
        //        if (IsHavingIMG(item, platform, "打底壳"))
        //            result.Add("BottomCasImg", true);
        //        else
        //            result.Add("BottomCasImg", false);

        //        //装磁吸
        //        var magnetic = db.Process_Capacity_Magnetic.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform).ToList();
        //        if (magnetic.Count == 0)
        //        {
        //            result.Add("Magnetic", null);

        //        }
        //        else
        //        {
        //            JArray array = new JArray();
        //            foreach (var magneticitem in magnetic)
        //            {
        //                JObject jObject = new JObject();
        //                jObject.Add("MagneticID", magneticitem.Id);
        //                jObject.Add("MagneticProcessName", magneticitem.MagneticSuctionProcessName);//装磁吸工序描述
        //                jObject.Add("MagneticSuctionStandardTotal", magneticitem.MagneticSuctionStandardTotal);//装磁吸标准总人数
        //                jObject.Add("MagneticSuctionStabdardOutput", magneticitem.MagneticSuctionStabdardOutput);//装磁吸标准产量
        //                jObject.Add("MagneticSuctionStandardHourlyOutputPerCapita", magneticitem.MagneticSuctionStabdardOutput);//装磁吸人均时产量
        //                array.Add(jObject);
        //            }
        //            result.Add("Magnetic", array);
        //        }


        //        //喷墨
        //        var inkjet = db.Process_Capacity_Inkjet.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform).ToList();
        //        if (inkjet.Count == 0)
        //        {
        //            result.Add("Inkjet", null);

        //        }
        //        else
        //        {
        //            JArray array = new JArray();
        //            foreach (var inkjetitem in inkjet)
        //            {
        //                JObject jObject = new JObject();
        //                jObject.Add("InkjetID", inkjetitem.Id);
        //                jObject.Add("InkjetProcessName", inkjetitem.InkjetProcessName);//喷墨工序
        //                jObject.Add("InkjetSuctionStandardTotal", inkjetitem.InkjetSuctionStandardTotal);//喷墨配置人数
        //                jObject.Add("InkjetStabdardOutputPerHour", inkjetitem.InkjetStabdardOutputPerHour);//喷墨每小时产能
        //                array.Add(jObject);
        //            }
        //            result.Add("Inkjet", array);
        //        }

        //        //灌胶
        //        var glue = db.Process_Capacity_Glue.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform).ToList();
        //        if (glue.Count == 0)
        //        {
        //            result.Add("Glue", null);

        //        }
        //        else
        //        {
        //            JArray array = new JArray();
        //            foreach (var glueitem in glue)
        //            {
        //                JObject jObject = new JObject();
        //                jObject.Add("GlueID", glueitem.Id);
        //                jObject.Add("GlueProcessName", glueitem.GlueProcessName);//灌胶工序描述
        //                jObject.Add("GlueStandardTotal", glueitem.GlueStandardTotal);//灌胶标准总人数
        //                jObject.Add("GlueStabdardOutput", glueitem.GlueStabdardOutput);//灌胶标准产量
        //                array.Add(jObject);
        //            }
        //            result.Add("Glue", array);
        //        }

        //        //气密
        //        var airtight = db.Process_Capacity_Airtight.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform).ToList();
        //        if (airtight.Count == 0)
        //        {
        //            result.Add("Airtight", null);

        //        }
        //        else
        //        {
        //            JArray array = new JArray();
        //            foreach (var airtightitem in airtight)
        //            {
        //                JObject jObject = new JObject();
        //                jObject.Add("AirtightID", airtightitem.Id);
        //                jObject.Add("AirtightProcessName", airtightitem.AirtightProcessName);//气密工序描述
        //                jObject.Add("AirtightStandardTotal", airtightitem.AirtightStandardTotal);//气密标准总人数
        //                jObject.Add("AirtightStabdardOutput", airtightitem.AirtightStabdardOutput);//气密标准产量
        //                array.Add(jObject);
        //            }
        //            result.Add("Airtight", array);
        //        }


        //        //锁面罩
        //        var LockTheMaskProcessName = db.ProcessBalance.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.Section == "锁面罩").ToList();
        //        if (LockTheMaskProcessName.Count() == 0)
        //        {
        //            result.Add("LockTheMask", null);
        //        }
        //        else
        //        {
        //            var title = LockTheMaskProcessName.Select(c => c.Title).ToList();
        //            JArray LockTheMask = new JArray();
        //            foreach (var ProcessName in title)
        //            {
        //                JObject jObject = new JObject();
        //                var message = LockTheMaskProcessName.OrderByDescending(c => c.SerialNumber).Where(c => c.Title == ProcessName).FirstOrDefault();
        //                jObject.Add("LockTheMaskProcessName", message.Title);
        //                jObject.Add("LockTheMaskStandardTotal", message.StandardTotal);
        //                jObject.Add("LockTheMaskStandardOutput", message.StandardOutput);
        //                jObject.Add("LockTheMaskStandardHourlyOutputPerCapita", message.StandardHourlyOutputPerCapita);
        //                jObject.Add("LockTheMaskScrewMachineNum", message.ScrewMachineNum);
        //                LockTheMask.Add(jObject);
        //            }
        //            result.Add("LockTheMask", LockTheMask);
        //        }
        //        if (IsHavingPDF(item, platform, "锁面罩"))
        //            result.Add("LockTheMaskPdf", true);
        //        else
        //            result.Add("LockTheMaskPdf", false);
        //        if (IsHavingIMG(item, platform, "锁面罩"))
        //            result.Add("LockTheMaskImg", true);
        //        else
        //            result.Add("LockTheMaskImg", false);

        //        //模组装配
        //        var ModuleProcessName = db.ProcessBalance.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.Section == "模组装配").ToList();
        //        if (ModuleProcessName.Count == 0)
        //        {
        //            result.Add("Module", null);
        //        }
        //        else
        //        {

        //            var title = ModuleProcessName.Select(c => c.Title).ToList();
        //            JArray Module = new JArray();
        //            foreach (var ProcessName in title)
        //            {
        //                JObject jObject = new JObject();
        //                var message = ModuleProcessName.OrderByDescending(c => c.SerialNumber).Where(c => c.Title == ProcessName).FirstOrDefault();
        //                jObject.Add("ModuleProcessName", message.Title);
        //                result.Add("ModuleStandardTotal", message.StandardTotal);
        //                result.Add("ModuleBalanceRate", message.BalanceRate + "%");
        //                result.Add("ModuleBottleneck", message.Bottleneck);
        //                result.Add("ModuleStandardOutput", message.StandardOutput);
        //                result.Add("ModuleStandardHourlyOutputPerCapita", message.StandardHourlyOutputPerCapita);
        //                Module.Add(jObject);
        //            }
        //            result.Add("Module", Module);
        //        }
        //        if (IsHavingPDF(item, platform, "模组装配"))
        //            result.Add("ModulePdf", true);
        //        else
        //            result.Add("ModulePdf", false);
        //        if (IsHavingIMG(item, platform, "模组装配"))
        //            result.Add("ModuleImg", true);
        //        else
        //            result.Add("ModuleImg", false);

        //        //老化
        //        var burnin = db.Process_Capacity_Burin.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform).ToList();
        //        if (burnin.Count == 0)
        //        {
        //            result.Add("Burin", null);

        //        }
        //        else
        //        {
        //            JArray array = new JArray();
        //            foreach (var burninitem in burnin)
        //            {
        //                JObject jObject = new JObject();
        //                jObject.Add("AirtightID", burninitem.Id);
        //                jObject.Add("BurinOneProcessName", burninitem.BurinOneProcessName);//老化工序描述1
        //                jObject.Add("BurninOneSuctionStandardTotal", burninitem.BurninOneSuctionStandardTotal);//老化1标配人数
        //                jObject.Add("BurinOneStabdardOutputPerHour", burninitem.BurinOneStabdardOutputPerHour);//老化1每小时产能
        //                jObject.Add("BurinTwoProcessName", burninitem.BurinTwoProcessName);//老化2工序描述
        //                jObject.Add("BurinTwoSuctionStandardTotal", burninitem.BurinTwoSuctionStandardTotal);//老化2标配人数
        //                jObject.Add("BurinTwoStabdardOutputPerHour", burninitem.BurinTwoStabdardOutputPerHour);//老化2每小时标准产能
        //                array.Add(jObject);
        //            }
        //            result.Add("Burin", array);
        //        }
        //        //包装
        //        var PackingProcessName = db.ProcessBalance.Where(c => c.Type == item && c.ProductPCBnumber == PCB && c.Platform == platform && c.Section == "包装").ToList();
        //        if (PackingProcessName.Count == 0)
        //        {
        //            result.Add("Packing", null);
        //        }
        //        else
        //        {

        //            var title = PackingProcessName.Select(c => c.Title).ToList();
        //            JArray Packing = new JArray();
        //            foreach (var ProcessName in title)
        //            {
        //                JObject jObject = new JObject();
        //                var message = PackingProcessName.OrderByDescending(c => c.SerialNumber).Where(c => c.Title == ProcessName).FirstOrDefault();
        //                jObject.Add("PackingProcessName", message.Title);
        //                result.Add("PackingStandardTotal", message.StandardTotal);
        //                result.Add("PackingStandardOutput", message.StandardOutput);
        //                Packing.Add(jObject);
        //            }
        //            result.Add("Packing", Packing);
        //        }
        //        if (IsHavingPDF(item, platform, "包装"))
        //            result.Add("PackingPdf", true);
        //        else
        //            result.Add("PackingPdf", false);
        //        if (IsHavingIMG(item, platform, "包装"))
        //            result.Add("PackingImg", true);
        //        else
        //            result.Add("PackingImg", false);
        //        //--启用编辑的字段，方便前端使用--
        //        result.Add("PluginEidt", false);
        //        result.Add("ThreeProfEidt", false);
        //        result.Add("BottomCasEidt", false);
        //        result.Add("MagneticEidt", false);
        //        result.Add("InkjetEidt", false);
        //        result.Add("GlueEidt", false);
        //        result.Add("AirtightEidt", false);
        //        result.Add("LockTheMaskEdit", false);
        //        result.Add("BurinEidt", false);
        //        //--edit end
        //        totel.Add(result);
        //    }

        //    return Content(JsonConvert.SerializeObject(totel));
        //}

        //public void SetModuleNeed(int id, string name, bool ismodule, decimal smtmodeulneed, decimal injekmodeulneed, decimal aftermodeulneed, decimal threemodeulneed, decimal bootnmodeulneed, decimal magneticmodeulneed, decimal pluginmodeulneed, decimal gluemodeulneed, decimal airtightmodeulneed, decimal lockmodeulneed, decimal modeulneed, decimal burnmodeulneed, decimal packmodeulneed)
        //{
        //    var blance = db.ProcessBalance.Find(id);//读取值
        //    //填写工序描述
        //    blance.ProcessDescription = name;
        //    if (ismodule)//如果是上传模组装配.则需要填写模组需求数
        //    {
        //        //填入模组号
        //        blance.SMTModuleNeedNum = smtmodeulneed;//贴片模组
        //        blance.PluginModuleNeedNum = pluginmodeulneed;//插件模组
        //        blance.AfterModuleNeedNum = aftermodeulneed;//后焊模组
        //        blance.ThreeModuleNeedNum = threemodeulneed;//三防模组
        //        blance.BottnModuleNeedNum = bootnmodeulneed;//打底壳模组
        //        blance.MagneticModuleNeedNum = magneticmodeulneed;//装磁吸模组
        //        blance.InjekModuleNeedNum = injekmodeulneed;//喷墨模组
        //        blance.GuleModuleNeedNum = gluemodeulneed;//灌胶模组
        //        blance.AfterModuleNeedNum = aftermodeulneed;//气密模组
        //        blance.LockMasModuleNeedNum = lockmodeulneed;//锁面罩模组
        //        blance.ModuleNeedNum2 = modeulneed;//模组装配模组
        //        blance.BuriInModuleNeedNum = burnmodeulneed;//老化模组
        //        blance.PackModuleNeedNum = packmodeulneed;//包装模组
        //    }
        //    db.Entry(blance).State = EntityState.Modified;
        //    db.SaveChanges();
        //}
        //public ActionResult GetSeactionList(string type, string plafrom)
        //{
        //    JObject result = new JObject();
        //    ////SMT
        //    //var SmtList = db.Pick_And_Place.Select(c => new TempSmt { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, PersonNum = c.PersonNum, CapacityPerHour = c.CapacityPerHour }).ToList();
        //    //插件
        //    var PluginList = db.Process_Capacity_Plugin.Where(c => c.Platform == plafrom && c.Type == type).Select(c => c.Name).Distinct().ToList();
        //    if(PluginList.Count<=1)
        //    //平衡卡
        //    var Balance = db.ProcessBalance.Select(c => new TempBlance { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.StandardOutput, StandardTotal = c.StandardTotal, Section = c.Section, SerialNumber = c.SerialNumber, Name = c.Title });
        //    //三防
        //    var ThreeProfList = db.Process_Capacity_ThreeProf.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.ThreeProfStabdardOutput, StandardNumber = c.ThreeProfStandardTotal });
        //    //喷墨
        //    var InkjetList = db.Process_Capacity_Inkjet.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.InkjetStabdardOutputPerHour, StandardNumber = c.InkjetSuctionStandardTotal });
        //    //灌胶
        //    var GlueList = db.Process_Capacity_Glue.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.GlueStabdardOutput, StandardNumber = c.GlueStandardTotal });
        //    //气密
        //    var AirtightList = db.Process_Capacity_Airtight.Select(c => new TempCapacity { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, StandardCapacity = c.AirtightStabdardOutput, StandardNumber = c.AirtightStandardTotal });
        //    //老化
        //    var BurnList = db.Process_Capacity_Burin.Select(c => new TempBurn { Type = c.Type, PCB = c.ProductPCBnumber, Plafrom = c.Platform, BurinOneStabdardOutputPerHour = c.BurinOneStabdardOutputPerHour, BurninOneSuctionStandardTotal = c.BurninOneSuctionStandardTotal, BurinTwoStabdardOutputPerHour = c.BurinTwoStabdardOutputPerHour, BurinTwoSuctionStandardTotal = c.BurinTwoSuctionStandardTotal });
        //}

        //平台下拉列表(待定不要)
        //public ActionResult PlatfromList()
        //{
        //    var orders = db.Process_Capacity_Total.Select(m => m.Platform).Distinct().ToList();    //获取平台清单
        //    JArray result = new JArray();
        //    foreach (var item in orders)
        //    {
        //        JObject List = new JObject();
        //        List.Add("value", item);

        //        result.Add(List);
        //    }
        //    return Content(JsonConvert.SerializeObject(result));
        //}
        //型号下拉列表
        public ActionResult TypeList()
        {
            var orders = db.Process_Capacity_Total.Select(m => m.Type).Distinct().ToList();    //获取型号清单
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //总表输入平台显示型号列表(待定不要)
        //public ActionResult DisplayTypeFromPlatfrom(string platfrom)
        //{
        //    var typeList = new List<string>();
        //    if (string.IsNullOrEmpty(platfrom))
        //    {
        //        typeList = db.Process_Capacity_Total.Select(m => m.Type).Distinct().ToList();
        //    }
        //    else
        //    {
        //        typeList = db.Process_Capacity_Total.Where(c => c.Platform == platfrom).Select(c => c.Type).Distinct().ToList();
        //    }
        //    JArray result = new JArray();
        //    foreach (var item in typeList)
        //    {
        //        JObject List = new JObject();
        //        List.Add("value", item);

        //        result.Add(List);
        //    }
        //    return Content(JsonConvert.SerializeObject(result));
        //}

        //总表输入型号显示平台列表
        public ActionResult DisplayPlatfromFromType(string type)
        {
            var platfromList = new List<string>();
            if (string.IsNullOrEmpty(type))//如果传过来的类型为空,则查找总表所有的平台
            {
                platfromList = db.Process_Capacity_Total.Select(m => m.Platform).Distinct().ToList();

            }
            else//根据选择的类型查找对应的平台
            {
                platfromList = db.Process_Capacity_Total.Where(c => c.Type.Contains(type)).Select(c => c.Platform).Distinct().ToList();
            }
            JArray result = new JArray();
            foreach (var item in platfromList)
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