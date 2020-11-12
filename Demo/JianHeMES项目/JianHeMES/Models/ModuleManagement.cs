﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    //AI表
    public class ModuleAI
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "模块条码"), StringLength(50)]
        public string ModuleBarcode { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string Ordernum { get; set; }

        [Display(Name = "机台"), StringLength(50)]
        public string Machine { get; set; }

        [Display(Name = "AI时间")]
        public DateTime? AITime { get; set; }

        [Display(Name = "AI人"), StringLength(50)]
        public string AIor { get; set; }

        [Display(Name = "是否异常")]
        public bool IsAbnormal { get; set; }

        [Display(Name = "异常结果"), StringLength(500)]
        public string AbnormalResultMessage { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }
    }
    //后焊表
    public class AfterWelding
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "模块条码"), StringLength(50)]
        public string ModuleBarcode { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string Ordernum { get; set; }

        [Display(Name = "后焊时间")]
        public DateTime? AfterWeldingTime { get; set; }

        [Display(Name = "后焊人"), StringLength(50)]
        public string AfterWeldingor { get; set; }

        [Display(Name = "是否异常")]
        public bool IsAbnormal { get; set; }

        [Display(Name = "异常结果"), StringLength(500)]
        public string AbnormalResultMessage { get; set; }

        [Display(Name = "是否抽检")]
        public bool IsSampling { get; set; }

        [Display(Name = "抽检结果")]
        public bool SamplingResult { get; set; }

        [Display(Name = "抽检异常信息"),StringLength(500)]
        public string  SamplingResultMessage { get; set; }

        [Display(Name = "抽检时间")]
        public DateTime? SamplingTime { get; set; }

        [Display(Name = "抽检人"), StringLength(50)]
        public string Samplingor { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "产线")]
        public int Line { get; set; }

        [Display(Name = "备注"), StringLength(500)]
        public string Remark { get; set; }
    }

    //电检表
    public class ElectricInspection
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "模块条码"), StringLength(50)]
        public string ModuleBarcode { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string Ordernum { get; set; }

        [Display(Name = "电检时间")]
        public DateTime? ElectricInspectionTime { get; set; }

        [Display(Name = "电检人"), StringLength(50)]
        public string ElectricInspectionor { get; set; }

        [Display(Name = "电检工段"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "电检是否异常")]
        public bool ElectricInspectionResult { get; set; }

        [Display(Name = "电检异常信息"), StringLength(500)]
        public string ElectricInspectionMessage { get; set; }

        [Display(Name = "是否抽检")]
        public bool IsSampling { get; set; }

        [Display(Name = "抽检结果")]
        public bool SamplingResult { get; set; }

        [Display(Name = "抽检异常信息"), StringLength(500)]
        public string SamplingResultMessage { get; set; }

        [Display(Name = "抽检时间")]
        public DateTime? SamplingTime { get; set; }

        [Display(Name = "抽检人"), StringLength(50)]
        public string Samplingor { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "备注"), StringLength(500)]
        public string Remark { get; set; }
    }

    //老化表
    public class ModuleBurnIn
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "模块条码"), StringLength(50)]
        public string ModuleBarcode { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string Ordernum { get; set; }

        [Display(Name = "旧模块条码"), StringLength(50)]
        public string OldModuleBarcode { get; set; }

        [Display(Name = "旧订单号"), StringLength(50)]
        public string OldOrdernum { get; set; }

        [Display(Name = "老化开始时间")]
        public DateTime? BurnInStartTime { get; set; }

        [Display(Name = "老化人"), StringLength(50)]
        public string BurnInStartor { get; set; }

        [Display(Name = "老化架号"), StringLength(50)]
        public string BurninFrame { get; set; }

        [Display(Name = "老化是否异常")]
        public bool BurninResult { get; set; }

        [Display(Name = "老化异常信息"), StringLength(500)]
        public string BurninMessage { get; set; }

        [Display(Name = "是否抽检")]
        public bool IsSampling { get; set; }

        [Display(Name = "抽检结果")]
        public bool SamplingResult { get; set; }

        [Display(Name = "抽检异常信息"), StringLength(500)]
        public string SamplingResultMessage { get; set; }

        [Display(Name = "抽检时间")]
        public DateTime? SamplingTime { get; set; }

        [Display(Name = "抽检人"), StringLength(50)]
        public string Samplingor { get; set; }

        [Display(Name = "老化结束时间")]
        public DateTime? BurnInEndTime { get; set; }

        [Display(Name = "老化结束人"), StringLength(50)]
        public string BurnInEndor { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "备注"), StringLength(500)]
        public string Remark { get; set; }
    }

    //外观表
    public class ModuleAppearance
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "模块条码"), StringLength(50)]
        public string ModuleBarcode { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string Ordernum { get; set; }

        [Display(Name = "旧模块条码"), StringLength(50)]
        public string OldModuleBarcode { get; set; }

        [Display(Name = "旧订单号"), StringLength(50)]
        public string OldOrdernum { get; set; }

        [Display(Name = "外观时间")]
        public DateTime? AppearanceTime { get; set; }

        [Display(Name = "外观人"), StringLength(50)]
        public string Appearanceor { get; set; }

        [Display(Name = "外观是否异常")]
        public bool AppearanceResult { get; set; }

        [Display(Name = "外观异常信息"), StringLength(500)]
        public string AppearanceMessage { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "备注"), StringLength(500)]
        public string Remark { get; set; }
    }

    //抽检表
    public class ModuleSampling
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "模块条码"), StringLength(50)]
        public string ModuleBarcode { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string Ordernum { get; set; }

        [Display(Name = "抽检工段"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "抽检时间")]
        public DateTime? SamplingTime { get; set; }

        [Display(Name = "抽检人"), StringLength(50)]
        public string Samplingor { get; set; }

        [Display(Name = "抽检是否异常")]
        public bool SamplingResult { get; set; }

        [Display(Name = "抽检异常信息"), StringLength(500)]
        public string SamplingMessage { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "备注"), StringLength(500)]
        public string Remark { get; set; }
    }

    //内箱
    public class ModuleInsideTheBox
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "模块条码"), StringLength(50)]
        public string ModuleBarcode { get; set; }

        [Display(Name = "内箱条码"), StringLength(50)]
        public string InnerBarcode { get; set; }

        [Display(Name = "订单"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "装箱时间")]
        public DateTime? InnerTime { get; set; }

        [Display(Name = "装箱人"), StringLength(50)]
        public string Inneror { get; set; }

        [Display(Name = "SN/TN")]
        public int SN { get; set; }

        [Display(Name = "装箱类型"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "是否挪用")]
        public bool IsEmbezzle { get; set; }

        [Display(Name = "挪用订单"), StringLength(50)]
        public string EmbezzleOrdeNum { get; set; }

        [Display(Name = "是否混装")]
        public bool IsMixture { get; set; }

        [Display(Name = "混装订单"), StringLength(50)]
        public string MixtureOrdeNum { get; set; }

        [Display(Name = "状态"), StringLength(50)]
        public string Statue { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "备注"), StringLength(50)]
        public string Remark { get; set; }

    }

    //外箱
    public class ModuleOutsideTheBox
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "内箱条码"), StringLength(50)]
        public string InnerBarcode { get; set; }

        [Display(Name = "外箱条码"), StringLength(50)]
        public string OutsideBarcode { get; set; }

        [Display(Name = "订单"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "装箱时间")]
        public DateTime? InnerTime { get; set; }

        [Display(Name = "装箱人"), StringLength(50)]
        public string Inneror { get; set; }

        [Display(Name = "SN/TN")]
        public int SN { get; set; }

        [Display(Name = "装箱类型"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "是否有LOGO")]
        public bool IsLogo { get; set; }

        [Display(Name = "毛重")]
        public double G_Weight { get; set; }

        [Display(Name = "净重")]
        public int N_Weight { get; set; }

        [Display(Name = "物料描述"), StringLength(50)]
        public string Materiel { get; set; }

        [Display(Name = "屏序")]
        public int ScreenNum { get; set; }

        [Display(Name = "是否分套")]
        public bool IsSeparate { get; set; }

        [Display(Name = "批次")]
        public int Batch { get; set; }

        [Display(Name = "是否分批")]
        public bool IsBatch { get; set; }

        [Display(Name = "是否挪用")]
        public bool IsEmbezzle { get; set; }

        [Display(Name = "挪用订单"), StringLength(50)]
        public string EmbezzleOrdeNum { get; set; }

        [Display(Name = "是否混装")]
        public bool IsMixture { get; set; }

        [Display(Name = "混装订单"), StringLength(50)]
        public string MixtureOrdeNum { get; set; }

        [Display(Name = "包装使用新订单号"), StringLength(50)]
        public string PackagingOrderNum { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "备注"), StringLength(50)]
        public string Remark { get; set; }
    }
    //装箱规则
    public class ModulePackageRule
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "订单"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "外箱or内箱"), StringLength(50)]
        public string Statue { get; set; }

        [Display(Name = "装箱类型"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "外箱容量(个)")]
        public int OuterBoxCapacity { get; set; }

        [Display(Name = "箱体数")]
        public int Quantity { get; set; }

        [Display(Name = "屏序")]
        public int ScreenNum { get; set; }

        [Display(Name = "是否分套")]
        public bool IsSeparate { get; set; }

        [Display(Name = "批次")]
        public int Batch { get; set; }

        [Display(Name = "是否分批")]
        public bool IsBatch { get; set; }

        [Display(Name = "整箱毛重量")]
        public double? Full_G_Weight { get; set; }

        [Display(Name = "整箱净重量")]
        public double? Full_N_Weight { get; set; }

        [Display(Name = "尾箱毛重量")]
        public double? Tail_G_Weight { get; set; }

        [Display(Name = "尾箱净重量")]
        public double? Tail_N_Weight { get; set; }

        [Display(Name = "创建人"), StringLength(20)]
        public string Creator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "颜色"), StringLength(50)]
        public string COLOURS { get; set; }

        [Display(Name = "规格型号"), StringLength(50)]
        public string ITEMNO { get; set; }

        [Display(Name = "备注"), StringLength(200)]
        public string Remark { get; set; }
    }


    //模块实时看板
    public class ModuleBoard
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string Ordernum { get; set; }

        [Display(Name = "工段"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "更新时间")]
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "是否完成")]
        public bool IsComplete { get; set; }
    }
}