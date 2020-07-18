﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    //贴片工序表
    public class Pick_And_Place
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "产品PCB编号"), StringLength(50)]
        public string ProductPCBnumber { get; set; }

        [Display(Name = "平台模块"), StringLength(50)]
        public string PlatformModul { get; set; }

        [Display(Name = "维护方式"), StringLength(50)]
        public string Maintenance { get; set; }

        [Display(Name = "工段"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "文件名称"), StringLength(50)]
        public string FileName { get; set; }

        [Display(Name = "编号"), StringLength(50)]
        public string SerialNumber { get; set; }

        [Display(Name = "线别")]
        public int Line { get; set; }

        [Display(Name = "机台配置"), StringLength(100)]
        public string MachineConfiguration { get; set; }

        [Display(Name = "产品型号"), StringLength(50)]
        public string ProductType { get; set; }

        [Display(Name = "PCB版号"), StringLength(50)]
        public string PCBNumber { get; set; }

        [Display(Name = "工序描述"), StringLength(50)]
        public string ProcessDescription { get; set; }

        [Display(Name = "印刷")]
        public decimal Print { get; set; }

        [Display(Name = "锡膏检测")]
        public decimal SolderPasteInspection { get; set; }

        [Display(Name = "压贴片螺母")]
        public decimal PressedPatchNut { get; set; }

        [Display(Name = "贴片机净作业")]
        public decimal SMTMachineNetWork { get; set; }

        [Display(Name = "人数")]
        public int PersonNum { get; set; }

        [Display(Name = "瓶颈")]
        public decimal Bottleneck { get; set; }

        [Display(Name = "每小时产能")]
        public decimal CapacityPerHour { get; set; }

        [Display(Name = "人均产能")]
        public decimal PerCapitaCapacity { get; set; }

        [Display(Name = "产品最新单价")]
        public decimal LatestUnitPrice { get; set; }

        [Display(Name = "制定人"), StringLength(50)]
        public string MakingPeople { get; set; }

        [Display(Name = "制定时间")]
        public DateTime? MakingTime { get; set; }

        [Display(Name = "审批人"), StringLength(50)]
        public string ExaminanPeople { get; set; }

        [Display(Name = "审批时间")]
        public DateTime? ExaminanTime { get; set; }

        [Display(Name = "批准人"), StringLength(50)]
        public string ApproverPeople { get; set; }

        [Display(Name = "批准时间")]
        public DateTime? ApproverTime { get; set; }

        [Display(Name = "受控人"), StringLength(50)]
        public string ControlledPeople { get; set; }

        [Display(Name = "受控时间")]
        public DateTime? ControlledTime { get; set; }

        [Display(Name = "修订"), StringLength(200)]
        public string Revision { get; set; }

        [Display(Name = "修订内容"), StringLength(1000)]
        public string RevisionContent { get; set; }

        [Display(Name = "备注"), StringLength(100)]
        public string Remark { get; set; }

        [Display(Name = "审核是否通过")]
        public bool IsPassExaminan { get; set; }

        [Display(Name = "批准是否通过")]
        public bool IsPassApprover { get; set; }

        [Display(Name = "受控是否通过")]
        public bool IsPassControlled { get; set; }

        [Display(Name = "文件路径地址"), StringLength(300)]
        public string FilePath { get; set; }

        [Display(Name = "模组需求数量")]
        public int ModuleNeedNum { get; set; }

        [Display(Name = "模组需求数量2")]
        public decimal ModuleNeedNum2 { get; set; }

    }

    //平衡卡表
    public class ProcessBalance
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "产品PCB编号"), StringLength(50)]
        public string ProductPCBnumber { get; set; }

        [Display(Name = "平台模块"), StringLength(50)]
        public string PlatformModul { get; set; }

        [Display(Name = "维护方式"), StringLength(50)]
        public string Maintenance { get; set; }

        [Display(Name = "工段"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "标题"), StringLength(50)]
        public string Title { get; set; }

        [Display(Name = "编号"), StringLength(50)]
        public string SerialNumber { get; set; }

        [Display(Name = "标准总人数和")]
        public int StandardTotal { get; set; }

        [Display(Name = "手工平衡率")]
        public decimal ManualBalanceRate { get; set; }

        [Display(Name = "设备平衡率")]
        public decimal EquipmentBalanceRate { get; set; }

        [Display(Name = "平衡率")]
        public decimal BalanceRate { get; set; }

        [Display(Name = "瓶颈")]
        public decimal Bottleneck { get; set; }

        [Display(Name = "标准产量")]
        public decimal StandardOutput { get; set; }

        [Display(Name = "标准人均时产量")]
        public decimal StandardHourlyOutputPerCapita { get; set; }

        [Display(Name = "产品工时")]
        public decimal ProductWorkingHours { get; set; }

        [Display(Name = "工序名称"), StringLength(1000)]
        public string ProcessName { get; set; }

        [Display(Name = "标准人数"), StringLength(1000)]
        public string StandarPersondNumber { get; set; }

        [Display(Name = "标准工时"), StringLength(1000)]
        public string StandardNumber { get; set; }

        [Display(Name = "单位工时"), StringLength(1000)]
        public string UnioTime { get; set; }

        [Display(Name = "夹具名称"), StringLength(1000)]
        public string JigName { get; set; }

        [Display(Name = "夹具个数"), StringLength(1000)]
        public string JigNum { get; set; }

        [Display(Name = "机器时间"), StringLength(1000)]
        public string MachineTime { get; set; }

        [Display(Name = "机器数量"), StringLength(1000)]
        public string MachineNumber { get; set; }

        [Display(Name = "制定人"), StringLength(50)]
        public string MakingPeople { get; set; }

        [Display(Name = "制定时间")]
        public DateTime? MakingTime { get; set; }

        [Display(Name = "审批人"), StringLength(50)]
        public string ExaminanPeople { get; set; }

        [Display(Name = "审批时间")]
        public DateTime? ExaminanTime { get; set; }

        [Display(Name = "批准人"), StringLength(50)]
        public string ApproverPeople { get; set; }

        [Display(Name = "批准时间")]
        public DateTime? ApproverTime { get; set; }

        [Display(Name = "受控人"), StringLength(50)]
        public string ControlledPeople { get; set; }

        [Display(Name = "受控时间")]
        public DateTime? ControlledTime { get; set; }

        [Display(Name = "修订"), StringLength(200)]
        public string Revision { get; set; }

        [Display(Name = "修订内容"), StringLength(1000)]
        public string RevisionContent { get; set; }

        [Display(Name = "备注"), StringLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "打底壳点胶机数量")]
        public int DispensMachineNum { get; set; }

        [Display(Name = "打底螺丝机数量")]
        public int ScrewMachineNum { get; set; }

        [Display(Name = "审核是否通过")]
        public bool IsPassExaminan { get; set; }

        [Display(Name = "批准是否通过")]
        public bool IsPassApprover { get; set; }

        [Display(Name = "受控是否通过")]
        public bool IsPassControlled { get; set; }

        [Display(Name = "文件路径地址"), StringLength(300)]
        public string FilePath { get; set; }



        [Display(Name = "模组需求数量2")]
        public decimal ModuleNeedNum2 { get; set; }


        [Display(Name = "模组需求数量")]
        public decimal ModuleNeedNum { get; set; }

        [Display(Name = "贴片模组需求数量")]
        public decimal SMTModuleNeedNum { get; set; }

        [Display(Name = "插件模组需求数量")]
        public decimal InjekModuleNeedNum { get; set; }

        [Display(Name = "后焊模组需求数量")]
        public decimal AfterModuleNeedNum { get; set; }

        [Display(Name = "三防模组需求数量")]
        public decimal ThreeModuleNeedNum { get; set; }

        [Display(Name = "打底壳模组需求数量")]
        public decimal BottnModuleNeedNum { get; set; }

        [Display(Name = "装磁吸模组需求数量")]
        public decimal MagneticModuleNeedNum { get; set; }

        [Display(Name = "喷墨模组需求数量")]
        public decimal PluginModuleNeedNum { get; set; }

        [Display(Name = "灌胶模组需求数量")]
        public decimal GuleModuleNeedNum { get; set; }

        [Display(Name = "气密模组需求数量")]
        public decimal AirtightModuleNeedNum { get; set; }

        [Display(Name = "锁面罩模组需求数量")]
        public decimal LockMasModuleNeedNum { get; set; }

        [Display(Name = "老化模组需求数量")]
        public decimal BuriInModuleNeedNum { get; set; }

        [Display(Name = "包装模组需求数量")]
        public decimal PackModuleNeedNum { get; set; }

        [Display(Name = "模块线需求数量")]
        public decimal ModuleLineNeedNum { get; set; }

        [Display(Name = "工序描述"),StringLength(100)]
        public string ProcessDescription { get; set; }

    }

    //产能工序总表
    public class Process_Capacity_Total
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "产品PCB编号"), StringLength(50)]
        public string ProductPCBnumber { get; set; }

        [Display(Name = "平台模块"), StringLength(50)]
        public string PlatformModul { get; set; }

        [Display(Name = "维护方式"), StringLength(50)]
        public string Maintenance { get; set; }

        [Display(Name = "PCB烘干时间")]
        public decimal PCBDrying { get; set; }

        [Display(Name = "外发工艺")]
        public decimal OutboundProcess { get; set; }

        [Display(Name = "干燥时间")]
        public decimal DryTime { get; set; }

        [Display(Name = "IC工序名称"), StringLength(50)]
        public string ICProcessName { get; set; }

        [Display(Name = "灯面工序名称"), StringLength(50)]
        public string LightProcessName { get; set; }

        [Display(Name = "创建人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? OperateDT { get; set; }

        //[Key]
        //public int Id { get; set; }

        //[Display(Name = "型号"), StringLength(50)]
        //public string Type { get; set; }

        //[Display(Name = "平台"), StringLength(50)]
        //public string Platform { get; set; }

        //[Display(Name = "产品PCB编号"), StringLength(50)]
        //public string ProductPCBnumber { get; set; }

        //[Display(Name = "IC工序名称"), StringLength(50)]
        //public string ICProcessName { get; set; }

        //[Display(Name = "灯面工序名称"), StringLength(50)]
        //public string LightProcessName { get; set; }

        //[Display(Name = "插件设备名"), StringLength(50)]
        //public string PluginDeviceName { get; set; }

        //[Display(Name = "插件机台固定标准单灯工时"), StringLength(100)]
        //public string SingleLampWorkingHours { get; set; }

        //[Display(Name = "PCBA插件单灯数"), StringLength(100)]
        //public string PCBASingleLampNumber { get; set; }

        //[Display(Name = "插件标配人数"), StringLength(100)]
        //public string PluginStandardNumber { get; set; }

        //[Display(Name = "插件标准产能"), StringLength(100)]
        //public string PluginStandardCapacity { get; set; }

        //[Display(Name = "后焊工序名称"), StringLength(50)]
        //public string AfterWeldProcessName { get; set; }

        //[Display(Name = "三防工序名称"), StringLength(50)]
        //public string ThreeProfProcessName { get; set; }

        //[Display(Name = "三防标准总人数"), StringLength(100)]
        //public string ThreeProfStandardTotal { get; set; }

        //[Display(Name = "三防标准产量"), StringLength(100)]
        //public string ThreeProfStabdardOutput { get; set; }

        //[Display(Name = "打底壳工序名称"), StringLength(50)]
        //public string BottomCasProcessName { get; set; }

        //[Display(Name = "打底壳点胶机数量"), StringLength(100)]
        //public string BottomCasDispensMachineNum { get; set; }

        //[Display(Name = "打底螺丝机数量"), StringLength(100)]
        //public string BottomCasScrewMachineNum { get; set; }

        //[Display(Name = "磁吸工序名称"), StringLength(50)]
        //public string MagneticSuctionProcessName { get; set; }

        //[Display(Name = "磁吸标准总人数"), StringLength(100)]
        //public string MagneticSuctionStandardTotal { get; set; }

        //[Display(Name = "磁吸标准产量"), StringLength(100)]
        //public string MagneticSuctionStabdardOutput { get; set; }

        //[Display(Name = "磁吸标准人均时产量"), StringLength(100)]
        //public string MagneticSuctionStandardHourlyOutputPerCapita { get; set; }

        //[Display(Name = "喷墨工序名称"), StringLength(50)]
        //public string InkjetProcessName { get; set; }

        //[Display(Name = "喷墨配置人数"), StringLength(100)]
        //public string InkjetSuctionStandardTotal { get; set; }

        //[Display(Name = "喷墨每小时产量"), StringLength(100)]
        //public string InkjetStabdardOutputPerHour { get; set; }

        //[Display(Name = "灌胶工序名称"), StringLength(50)]
        //public string GlueProcessName { get; set; }

        //[Display(Name = "灌胶标准总人数"), StringLength(100)]
        //public string GlueStandardTotal { get; set; }

        //[Display(Name = "灌胶标准产量"), StringLength(100)]
        //public string GlueStabdardOutput { get; set; }

        //[Display(Name = "气密工序名称"), StringLength(50)]
        //public string AirtightProcessName { get; set; }

        //[Display(Name = "气密标准总人数"), StringLength(100)]
        //public string AirtightStandardTotal { get; set; }

        //[Display(Name = "气密标准产量"), StringLength(100)]
        //public string AirtightStabdardOutput { get; set; }

        //[Display(Name = "锁面罩工序名称"), StringLength(50)]
        //public string LockTheMaskProcessName { get; set; }

        //[Display(Name = "锁面罩螺丝机数量"), StringLength(100)]
        //public string LockTheMaskScrewMachineNum { get; set; }

        //[Display(Name = "模组工序名称"), StringLength(50)]
        //public string ModuleProcessName { get; set; }

        //[Display(Name = "老化1工序名称"), StringLength(50)]
        //public string BurinOneProcessName { get; set; }

        //[Display(Name = "老化1标配人数"), StringLength(100)]
        //public string BurninOneSuctionStandardTotal { get; set; }

        //[Display(Name = "老化1每小时标准产量"), StringLength(100)]
        //public string BurinOneStabdardOutputPerHour { get; set; }

        //[Display(Name = "老化2工序名称"), StringLength(50)]
        //public string BurinTwoProcessName { get; set; }

        //[Display(Name = "老化2标配人数"), StringLength(100)]
        //public string BurinTwoSuctionStandardTotal { get; set; }

        //[Display(Name = "老化2每小时标准产量"), StringLength(100)]
        //public string BurinTwoStabdardOutputPerHour { get; set; }

        //[Display(Name = "包装工序名称"), StringLength(50)]
        //public string PackingProcessName { get; set; }
    }

    //手输入表
    public class Process_Capacity_Manual
    {

        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "产品PCB编号"), StringLength(50)]
        public string ProductPCBnumber { get; set; }

        [Display(Name = "平台模块"), StringLength(50)]
        public string PlatformModul { get; set; }

        [Display(Name = "维护方式"), StringLength(50)]
        public string Maintenance { get; set; }

        [Display(Name = "工段"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "工序描述"), StringLength(50)]
        public string Name { get; set; }
        
        [Display(Name = "标配人数")]
        public int StandardNumber { get; set; }

        [Display(Name = "标准产能")]
        public decimal StandardCapacity { get; set; }

        [Display(Name = "工序描述"), StringLength(50)]
        public string Name2 { get; set; }

        [Display(Name = "标配人数")]
        public int StandardNumber2 { get; set; }

        [Display(Name = "标准产能")]
        public decimal StandardCapacity2 { get; set; }
        
        [Display(Name = "打底壳点胶机数量")]
        public int DispensMachineNum { get; set; }

        [Display(Name = "打底螺丝机数量")]
        public int ScrewMachineNum { get; set; }

        [Display(Name = "插件机台固定标准单灯工时")]
        public double SingleLampWorkingHours { get; set; }

        [Display(Name = "PCBA插件单灯数")]
        public int PCBASingleLampNumber { get; set; }
        
        [Display(Name = "创建人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? OperateDT { get; set; }
    }

    //设备表
    public class Process_Capacity_Equipment
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "产品PCB编号"), StringLength(50)]
        public string ProductPCBnumber { get; set; }

        [Display(Name = "平台模块"), StringLength(50)]
        public string PlatformModul { get; set; }

        [Display(Name = "维护方式"), StringLength(50)]
        public string Maintenance { get; set; }

        [Display(Name = "工段"), StringLength(50)]
        public string Seaction { get; set; }

        [Display(Name = "工段ID")]
        public int SeactionID { get; set; }

        [Display(Name = "设备名字"), StringLength(50)]
        public string EquipmentName { get; set; }

        [Display(Name = "设备台数")]
        public int EquipmentNum { get; set; }

        [Display(Name = "设备产能")]
        public decimal EquipmentCapacity { get; set; }

        [Display(Name = "一人能看机台设备或者人力的人数")]
        public int PersonEquipmentNum { get; set; }

        [Display(Name = "人力或设备")]
        public string Statue { get; set; }
    }

    //插件 没用
    public class Process_Capacity_Plugin
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "产品PCB编号"), StringLength(50)]
        public string ProductPCBnumber { get; set; }

        [Display(Name = "平台模块"), StringLength(50)]
        public string PlatformModul { get; set; }

        [Display(Name = "维护方式"), StringLength(50)]
        public string Maintenance { get; set; }

        [Display(Name = "工序描述"), StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "插件机台固定标准单灯工时")]
        public double SingleLampWorkingHours { get; set; }

        [Display(Name = "PCBA插件单灯数")]
        public int PCBASingleLampNumber { get; set; }

        [Display(Name = "插件标配人数")]
        public int PluginStandardNumber { get; set; }

        [Display(Name = "插件标准产能")]
        public decimal PluginStandardCapacity { get; set; }

        [Display(Name = "创建人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? OperateDT { get; set; }

        [Display(Name = "模组需求数量")]
        public int ModuleNeedNum { get; set; }

        [Display(Name = "模组需求数量2")]
        public decimal ModuleNeedNum2 { get; set; }

    }
    //三防 没用
    public class Process_Capacity_ThreeProf
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "产品PCB编号"), StringLength(50)]
        public string ProductPCBnumber { get; set; }

        [Display(Name = "三防工序名称"), StringLength(50)]
        public string ThreeProfProcessName { get; set; }

        [Display(Name = "三防标准总人数")]
        public int ThreeProfStandardTotal { get; set; }

        [Display(Name = "三防标准产量")]
        public decimal ThreeProfStabdardOutput { get; set; }

        [Display(Name = "创建人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? OperateDT { get; set; }

        [Display(Name = "模组需求数量")]
        public int ModuleNeedNum { get; set; }

        [Display(Name = "模组需求数量2")]
        public decimal ModuleNeedNum2 { get; set; }

    }
    //装磁吸安装板 没用
    public class Process_Capacity_Magnetic
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "产品PCB编号"), StringLength(50)]
        public string ProductPCBnumber { get; set; }

        [Display(Name = "磁吸工序名称"), StringLength(50)]
        public string MagneticSuctionProcessName { get; set; }

        [Display(Name = "磁吸标准总人数")]
        public int MagneticSuctionStandardTotal { get; set; }

        [Display(Name = "磁吸标准产量")]
        public decimal MagneticSuctionStabdardOutput { get; set; }

        [Display(Name = "磁吸标准人均时产量")]
        public decimal MagneticSuctionStandardHourlyOutputPerCapita { get; set; }

        [Display(Name = "创建人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? OperateDT { get; set; }

        [Display(Name = "模组需求数量")]
        public int ModuleNeedNum { get; set; }

        [Display(Name = "模组需求数量2")]
        public decimal ModuleNeedNum2 { get; set; }

    }

    //喷墨 没用
    public class Process_Capacity_Inkjet
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "产品PCB编号"), StringLength(50)]
        public string ProductPCBnumber { get; set; }

        [Display(Name = "平台模块"), StringLength(50)]
        public string PlatformModul { get; set; }

        [Display(Name = "维护方式"), StringLength(50)]
        public string Maintenance { get; set; }

        [Display(Name = "喷墨工序名称"), StringLength(50)]
        public string InkjetProcessName { get; set; }

        [Display(Name = "喷墨配置人数")]
        public int InkjetSuctionStandardTotal { get; set; }

        [Display(Name = "喷墨每小时产量")]
        public decimal InkjetStabdardOutputPerHour { get; set; }

        [Display(Name = "创建人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? OperateDT { get; set; }

        [Display(Name = "模组需求数量")]
        public int ModuleNeedNum { get; set; }

        [Display(Name = "模组需求数量2")]
        public decimal ModuleNeedNum2 { get; set; }

    }

    //灌胶 没用
    public class Process_Capacity_Glue
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "产品PCB编号"), StringLength(50)]
        public string ProductPCBnumber { get; set; }

        [Display(Name = "平台模块"), StringLength(50)]
        public string PlatformModul { get; set; }

        [Display(Name = "维护方式"), StringLength(50)]
        public string Maintenance { get; set; }

        [Display(Name = "灌胶工序名称"), StringLength(50)]
        public string GlueProcessName { get; set; }

        [Display(Name = "灌胶标准总人数")]
        public int GlueStandardTotal { get; set; }

        [Display(Name = "灌胶标准产量")]
        public decimal GlueStabdardOutput { get; set; }

        [Display(Name = "创建人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? OperateDT { get; set; }

        [Display(Name = "模组需求数量")]
        public int ModuleNeedNum { get; set; }

        [Display(Name = "模组需求数量2")]
        public decimal ModuleNeedNum2 { get; set; }

    }
    //气密 没用
    public class Process_Capacity_Airtight
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "产品PCB编号"), StringLength(50)]
        public string ProductPCBnumber { get; set; }

        [Display(Name = "平台模块"), StringLength(50)]
        public string PlatformModul { get; set; }

        [Display(Name = "维护方式"), StringLength(50)]
        public string Maintenance { get; set; }

        [Display(Name = "气密工序名称"), StringLength(50)]
        public string AirtightProcessName { get; set; }

        [Display(Name = "气密标准总人数")]
        public int AirtightStandardTotal { get; set; }

        [Display(Name = "气密标准产量")]
        public decimal AirtightStabdardOutput { get; set; }

        [Display(Name = "创建人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? OperateDT { get; set; }

        [Display(Name = "模组需求数量")]
        public int ModuleNeedNum { get; set; }

        [Display(Name = "模组需求数量2")]
        public decimal ModuleNeedNum2 { get; set; }

    }

    //老化 没用
    public class Process_Capacity_Burin
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "产品PCB编号"), StringLength(50)]
        public string ProductPCBnumber { get; set; }

        [Display(Name = "平台模块"), StringLength(50)]
        public string PlatformModul { get; set; }

        [Display(Name = "维护方式"), StringLength(50)]
        public string Maintenance { get; set; }

        [Display(Name = "老化1工序名称"), StringLength(50)]
        public string BurinOneProcessName { get; set; }

        [Display(Name = "老化1标配人数")]
        public int BurninOneSuctionStandardTotal { get; set; }

        [Display(Name = "老化1每小时标准产量")]
        public decimal BurinOneStabdardOutputPerHour { get; set; }

        [Display(Name = "老化2工序名称"), StringLength(50)]
        public string BurinTwoProcessName { get; set; }

        [Display(Name = "老化2标配人数")]
        public int BurinTwoSuctionStandardTotal { get; set; }

        [Display(Name = "老化2每小时标准产量")]
        public decimal BurinTwoStabdardOutputPerHour { get; set; }
        
        [Display(Name = "创建人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? OperateDT { get; set; }

        [Display(Name = "模组需求数量")]
        public int ModuleNeedNum { get; set; }

        [Display(Name = "模组需求数量2")]
        public decimal ModuleNeedNum2 { get; set; }

    }

    //关联表

    public class Process_Capacity_Relevance
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "型号"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "平台"), StringLength(50)]
        public string Platform { get; set; }

        [Display(Name = "平台模块"), StringLength(50)]
        public string PlatformModul { get; set; }

        [Display(Name = "维护方式"), StringLength(50)]
        public string Maintenance { get; set; }

        [Display(Name = "父系联系工段"), StringLength(50)]
        public string FatherSeaction { get; set; }

        [Display(Name = "父系联系ID")]
        public int FatherID { get; set; }

        [Display(Name = "子系联系工段"), StringLength(50)]
        public string ChildSeaction { get; set; }

        [Display(Name = "子系联系ID")]
        public int ChildID { get; set; }

        [Display(Name = "创建人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? OperateDT { get; set; }

    }
}