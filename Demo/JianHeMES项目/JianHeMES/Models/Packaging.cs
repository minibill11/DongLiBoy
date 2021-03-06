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

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "条码号"), StringLength(50)]
        public string BarCodesNum { get; set; }

        [Display(Name = "开始时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? OQCCheckBT { get; set; }

        [Display(Name = "OQC检验员"), StringLength(50)]
        public string OQCPrincipal { get; set; }

        [Display(Name = "完成时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? OQCCheckFT { get; set; }

        [Display(Name = "OQC时长")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public TimeSpan? OQCCheckTime { get; set; }

        [Display(Name = "校正用时"), StringLength(50)]
        public string OQCCheckTimeSpan { get; set; }

        [Display(Name = "OQC异常")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]l
        public int Packaging_OQCCheckAbnormal { get; set; }

        [Display(Name = "维修情况"), StringLength(200)]
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
        [Display(Name = "描述"), StringLength(200)]
        public string DetialedDescription { get; set; }

    }

    public class Packing_BasicInfo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "包装箱类型"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "外箱容量(个)")]
        public int OuterBoxCapacity { get; set; }

        [Display(Name = "数量")]
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

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "出库完成时间")]
        public DateTime? WarehouseTime { get; set; }
    }

    public class Packing_BarCodePrinting
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "包装使用新订单号"), StringLength(50)]
        public string PackagingOrderNum { get; set; }

        [Display(Name = "外箱所属订单号"), StringLength(50)]
        public string CartonOrderNum { get; set; }

        [Display(Name = "外箱挪用订单号"), StringLength(50)]
        public string EmbezzleOrderNum { get; set; }

        [Display(Name = "外箱条码"), StringLength(50)]
        public string OuterBoxBarcode { get; set; }

        [Display(Name = "包装箱类型"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "模组条码"), StringLength(50)]
        public string BarCodeNum { get; set; }

        [Display(Name = "模组箱体号"), StringLength(50)]
        public string ModuleGroupNum { get; set; }

        [Display(Name = "品质QC人员"), StringLength(20)]
        public string QC_Operator { get; set; }

        [Display(Name = "品质QC确认时间")]
        public DateTime? QC_ComfirmDate { get; set; }

        [Display(Name = "包装操作人员"), StringLength(50)]
        public string Operator { get; set; }

        [Display(Name = "包装操作时间")]
        public DateTime? Date { get; set; }

        [Display(Name = "毛重量")]
        public double? G_Weight { get; set; }

        [Display(Name = "净重量")]
        public double? N_Weight { get; set; }

        [Display(Name = "SNTN"), StringLength(50)]
        public string SNTN { get; set; }

        [Display(Name = "批次")]
        public int Batch { get; set; }

        [Display(Name = "是否分批")]
        public bool IsBatch { get; set; }

        [Display(Name = "物料描述"), StringLength(50)]
        public string Materiel { get; set; }

        [Display(Name = "是否有标签")]
        public bool IsLogo { get; set; }

        [Display(Name = "屏序")]
        public int ScreenNum { get; set; }

        [Display(Name = "是否分套")]
        public bool IsSeparate { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }
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

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }
    }

    public class Packing_SPC_Records
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "外箱条码"), StringLength(50)]
        public string SPC_OuterBoxBarcode { get; set; }

        [Display(Name = "箱号"), StringLength(100)]
        public string Case_Number { get; set; }

        [Display(Name = "物品分类"), StringLength(60)]
        public string Item_Type { get; set; }

        [Display(Name = "物品名称"), StringLength(200)]
        public string Material_Name { get; set; }

        [Display(Name = "规格描述"), StringLength(600)]
        public string Specification_Description { get; set; }

        [Display(Name = "物料描述"), StringLength(60)]
        public string Material_Description { get; set; }

        [Display(Name = "物料编号"), StringLength(50)]
        public string MaterialNumber { get; set; }

        [Display(Name = "数量")]
        public Decimal Quantity { get; set; }

        [Display(Name = "物料标签显示类型"), StringLength(20)]
        public string SPC_Material_Type { get; set; }

        [Display(Name = "标签打印人员"), StringLength(20)]
        public string SPC_Label_Operator { get; set; }

        [Display(Name = "标签打印时间")]
        public DateTime? SPC_Label_Date { get; set; }

        [Display(Name = "备料确认")]
        public bool? SPC_Material_Confim { get; set; }

        [Display(Name = "备料确认人员"), StringLength(20)]
        public string SPC_Material_Confim_Operator { get; set; }

        [Display(Name = "备料确认时间")]
        public DateTime? SPC_Material_Confim_Date { get; set; }

        [Display(Name = "外箱标签显示类型"), StringLength(20)]
        public string SPC_OuterBox_Type { get; set; }

        [Display(Name = "屏序")]
        public int? ScreenNum { get; set; }

        [Display(Name = "批次")]
        public int? Batch { get; set; }

        [Display(Name = "LOGO")]
        public bool? IsLogo { get; set; }

        [Display(Name = "毛重量")]
        public double? G_Weight { get; set; }

        [Display(Name = "净重量")]
        public double? N_Weight { get; set; }

        [Display(Name = "SNTN"), StringLength(50)]
        public string SNTN { get; set; }

        [Display(Name = "包装人员"), StringLength(20)]
        public string SPC_Packaging_Operator { get; set; }

        [Display(Name = "包装时间")]
        public DateTime? SPC_Packaging_Date { get; set; }

        [Display(Name = "备注"), StringLength(200)]
        public string Remark { get; set; }

        [Display(Name = "单位"), StringLength(20)]
        public string Unit { get; set; }

        [Display(Name = "表序")]
        public int? TableNumber { get; set; }

        [Display(Name = "QC部门"), StringLength(50)]
        public string QC_Department { get; set; }

        [Display(Name = "QC班组"), StringLength(50)]
        public string QC_Group { get; set; }

        [Display(Name = "老化部门"), StringLength(50)]
        public string Pa_Department { get; set; }

        [Display(Name = "老化班组"), StringLength(50)]
        public string Pa_QCGroup { get; set; }

        [Display(Name = "清单导入人员"), StringLength(20)]
        public string ListingImport_Operator { get; set; }

        [Display(Name = "清单导入时间")]
        public DateTime? ListingImport_Date { get; set; }

        [Display(Name = "打印外箱标签人员"), StringLength(20)]
        public string Print_OutsideBox_Operator { get; set; }

        [Display(Name = "打印外箱标签时间")]
        public DateTime? Print_OutsideBox_Date { get; set; }

        [Display(Name = "拆分记录人"), StringLength(20)]
        public string Split_Operator { get; set; }

        [Display(Name = "拆分时间")]
        public DateTime? Split_Date { get; set; }

        [Display(Name = "外箱标签语言"), StringLength(20)]
        public string SPC_OutsideBoxLanguage { get; set; }

        [Display(Name = "项目名称"), StringLength(400)]
        public string SPC_projectName { get; set; }

        [Display(Name = "外箱标签打印次数")]
        public int SPC_OutsideBoxPrintCount { get; set; }

    }


}