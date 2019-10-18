﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class Packaging
    {
        //外观电检部分
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号")]
        public string OrderNum { get; set; }

        [Display(Name = "条码号")]
        public string BarCodesNum { get; set; }

        [Display(Name = "开始时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? OQCCheckBT { get; set; }

        [Display(Name = "OQC检验员")]
        public string OQCPrincipal { get; set; }

        [Display(Name = "完成时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? OQCCheckFT { get; set; }

        [Display(Name = "OQC时长")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public TimeSpan? OQCCheckTime { get; set; }

        [Display(Name = "校正用时")]
        public string OQCCheckTimeSpan { get; set; }

        [Display(Name = "OQC异常")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]l
        public int Packaging_OQCCheckAbnormal { get; set; }

        [Display(Name = "维修情况")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public string RepairCondition { get; set; }

        [Display(Name = "OQC是否完成")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public Boolean OQCCheckFinish { get; set; }

        public virtual List<OrderMgm> OrderMgm { get; set; }
        public virtual List<Users> Users { get; set; }
        public virtual List<BarCodes> BarCodes { get; set; }
        public virtual List<Assemble> Assembles { get; set; }
        public virtual List<PQCCheckabnormal> PQCCheckabnormal { get; set; }

    }

    public class Packaging_OQCCheckAbnormal
    {
        [Key]
        public int Id { get; set; }
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        [Display(Name = "Packaging_OQC异常代码")]
        public int Code { get; set; }
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        [Display(Name = "描述")]
        public string DetialedDescription { get; set; }

    }

    public class Packing_BasicInfo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"),StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "包装箱类型"),StringLength(50)]
        public string Type { get; set; }

        [Display(Name ="外箱容量(个)")]
        public int OuterBoxCapacity { get; set; }

        [Display(Name ="数量")]
        public int Quantity { get; set; }

        [Display(Name = "创建人"), StringLength(20)]
        public string Creator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "备注"), StringLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "屏序")]
        public int ScreenNum { get; set; }

        [Display(Name = "是否分套")]
        public bool IsSeparate { get; set; }

    }

    public class Packing_BarCodePrinting
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "包装使用新订单号"), StringLength(50)]
        public string PackagingOrderNum { get; set; }

        [Display(Name = "外箱条码"), StringLength(50)]
        public string OuterBoxBarcode { get; set; }

        [Display(Name = "包装箱类型"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "模组条码"), StringLength(50)]
        public string  BarCodeNum { get; set; }

        [Display(Name = "模组箱体号"), StringLength(50)]
        public string  ModuleGroupNum { get; set; }

        [Display(Name = "品质QC人员"), StringLength(20)]
        public string QC_Operator { get; set; }

        [Display(Name = "品质QC确认时间")]
        public DateTime? QC_ComfirmDate { get; set; }

        [Display(Name = "包装操作人员"), StringLength(50)]
        public string Operator { get; set; }

        [Display(Name = "包装操作时间")]
        public DateTime? Date { get; set; }

        [Display(Name = "SNTN"), StringLength(50)]
        public string SNTN { get; set; }

        [Display(Name = "物料描述"), StringLength(50)]
        public string Materiel { get; set; }

        [Display(Name = "是否有标签")]
        public bool IsLogo { get; set; }
        
        [Display(Name = "屏序")]
        public int ScreenNum { get; set; }

        [Display(Name = "是否分套")]
        public bool IsSeparate { get; set; }
    }

    public class Packing_InnerCheck
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "内箱条码"), StringLength(50)]
        public string Barcode { get; set; }

        [Display(Name = "模组号"), StringLength(50)]
        public string ModuleNum { get; set; }

        [Display(Name = "品质QC人员"), StringLength(20)]
        public string QC_Operator { get; set; }

        [Display(Name = "品质QC确认时间")]
        public DateTime? QC_ComfirmDate { get; set; }
    }
}