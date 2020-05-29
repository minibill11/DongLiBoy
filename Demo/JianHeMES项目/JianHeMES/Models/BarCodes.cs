﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    //订单条码表
    public class BarCodes
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "新订单号"), StringLength(50)]
        public string ToOrderNum { get; set; }

        [Required]
        [Display(Name = "条码前缀"), StringLength(50)]
        public string BarCode_Prefix { get; set; }

        [Required]
        [Display(Name = "条码"), StringLength(50)]
        public string BarCodesNum { get; set; }

        [Display(Name = "模组箱体号"), StringLength(50)]
        public string ModuleGroupNum { get; set; }

        [Required]
        [Display(Name = "类型"), StringLength(50)]
        public string BarCodeType { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "创建人"), StringLength(50)]
        public string Creator { get; set; }

        [Display(Name = "是否为库存")]
        public Boolean IsRepertory { get; set; }

        [Display(Name = "条码打印次数")]
        public int BarcodePrintCount { get; set; }

        [Display(Name = "模组打印次数")]
        public int ModuleNumPrintCount { get; set; }

        [Display(Name = "备注"), StringLength(1000)]
        public string Remark { get; set; }


        public virtual List<OrderMgm> OrderMgm { get; set; }
        public virtual List<Assemble> Assemble { get; set; }
        public virtual List<Users> Users { get; set; }

    }

    //挪用订单条码关联
    public class BarCodeRelation
    {
        [Key]
        public int ID { get; set; }
        
        [Display(Name = "库存订单号"),StringLength(50)]
        public string OldOrderNum { get; set; }

        [Display(Name = "库存条码"), StringLength(50)]
        public string OldBarCodeNum { get; set; }
        
        [Display(Name = "新订单号"), StringLength(50)]
        public string NewOrderNum { get; set; }
    
        [Display(Name = "新条码"), StringLength(50)]
        public string NewBarCodesNum { get; set; }

        [Display(Name = "工序"),StringLength(30)]
        public string Procedure { get; set; }

        [Display(Name = "操作人工号")]
        public int UsserID { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreateDate { get; set; }

    }

    //锡膏条码
    public class Barcode_Solderpaste
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "锡膏条码"), StringLength(100)]
        public string SolderpasterBacrcode { get; set; }

        [Display(Name = "打印时间")]
        public DateTime? PrintTime { get; set; }

        [Display(Name = "打印人姓名"), StringLength(50)]
        public string PrintName { get; set; }

        [Display(Name = "批次"), StringLength(50)]
        public string Batch { get; set; }

        [Display(Name = "是否打印")]
        public bool Print { get; set; }

        [Display(Name = "料件编号"), StringLength(30)]
        public string ReceivingNum { get; set; }  //(迁移完成后删除)

        [Display(Name = "物料编号"), StringLength(30)]
        public string MaterialNumber { get; set; }

        [Display(Name = "生产日期")]
        public DateTime? LeaveFactoryTime { get; set; }

        [Display(Name = "供应商"), StringLength(50)]
        public string Supplier { get; set; }

        [Display(Name = "锡膏类型"), StringLength(50)]
        public string SolderpasteType { get; set; }

        [Display(Name = "有效期")]
        public int EffectiveDay { get; set; }
    }

    //根据图纸模组规则创建模组号
    public class Barcode_Module
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "模组号"), StringLength(50)]
        public string ModuleNum { get; set; }

        [Display(Name = "条码"), StringLength(50)]
        public string Barcode { get; set; }

        [Display(Name = "屏序"), StringLength(20)]
        public string ScreenNum { get; set; }

        [Display(Name = "是否特殊")]
        public bool IsSpecial { get; set; }

        [Display(Name = "是否可用")]
        public bool IsUse { get; set; }

        [Display(Name = "备注")]
        public string Remak { get; set; }
    }
}