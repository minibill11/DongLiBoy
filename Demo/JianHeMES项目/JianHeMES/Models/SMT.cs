﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class SMT_OrderInfo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"),StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "数量")]
        public int Quantity { get; set; }

        [Display(Name = "平台类型"),StringLength(50)]

        public string PlatformType { get; set; }

        [Display(Name = "客户"), StringLength(50)]
        public string Customer { get; set; }

        [Display(Name = "交货时间"),DataType(DataType.DateTime)]
        public DateTime? DeliveryDate { get; set; }

        [Display(Name = "完成状态")]
        public Boolean Status { get; set; }

    }

    public class SMT_ProductionPlan
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="产线")]
        public int LineNum { get; set; }

        [Display(Name ="订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "计划产能")]
        public decimal Capacity { get; set; }

        [Display(Name = "计划数量")]
        public int Quantity { get; set; }

        [Display(Name = "工作内容"), StringLength(50)]
        public string JobContent { get; set; }

        [Display(Name ="创建生产计划日期"),DataType(DataType.DateTime)]  
        public DateTime? CreateDate { get; set; }

        [Display(Name = "计划生产日期"), DataType(DataType.DateTime)]
        public DateTime? PlanProductionDate { get; set; }

        [Display(Name = "计划创建人"), StringLength(50)]
        public string Creator { get; set; }

        [Display(Name = "备注"), StringLength(500)]
        public string Remark { get; set; }

    }
    public class SMT_ProductionData
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "产线")]
        public int LineNum { get; set; }

        [Display(Name = "订单"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "工作内容"), StringLength(50)]
        public string JobContent { get; set; }

        [Display(Name = "良品数量")]
        public int NormalCount { get; set; }

        [Display(Name = "不良品数量")]
        public int AbnormalCount { get; set; }

        [Display(Name = "开始时间"), DataType(DataType.DateTime)]
        public DateTime? BeginTime { get; set; }

        [Display(Name = "结束时间"), DataType(DataType.DateTime)]
        public DateTime? EndTime { get; set; }

        [Display(Name = "条码号"), StringLength(50)]
        public string BarcodeNum { get; set; }

        [Display(Name = "产品状态")]
        public bool Result { get; set; }

        [Display(Name = "日期"),DataType(DataType.DateTime)]
        public DateTime? ProductionDate { get; set; }

        [Display(Name ="操作员"), StringLength(50)]
        public string Operator { get; set; }

    }

    public class SMT_ProductionLineInfo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "产线")]
        public int LineNum { get; set; }

        [Display(Name = "正在生产的订单"), StringLength(50)]
        public string ProducingOrderNum { get; set; }

        [Display(Name = "创建记录日期"),DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Team { get; set; }

        [Display(Name = "组长"), StringLength(50)]
        public string GroupLeader { get; set; }

        [Display(Name = "产线状态"), StringLength(50)]
        public string Status { get; set; }

    }

    public class SMT_ProductionBoardTable
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "工作内容"), StringLength(50)]
        public string JobContent { get; set; }

        [Display(Name = "产线")]
        public int LineNum { get; set; }
    }

    public class SMT_Freezer
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "锡膏条码"), StringLength(100)]
        public string SolderpasterBacrcode { get; set; }

        [Display(Name = "入库时间")]
        public DateTime? IntoTime { get; set; }

        [Display(Name = "姓名"), StringLength(50)]
        public string UserName { get; set; }

        [Display(Name = "工号"), StringLength(50)]
        public string JobNum { get; set; }

        [Display(Name = "来源于"), StringLength(50)]
        public string Belogin { get; set; }
    }

    public class SMT_Rewarm
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "锡膏条码"), StringLength(100)]
        public string SolderpasterBacrcode { get; set; }

        [Display(Name = "回温时间")]
        public DateTime? StartTime { get; set; }

        [Display(Name = "姓名"), StringLength(50)]
        public string UserName { get; set; }

        [Display(Name = "工号"), StringLength(50)]
        public string JobNum { get; set; }
    }

    public class SMT_Stir
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "锡膏条码"), StringLength(100)]
        public string SolderpasterBacrcode { get; set; }

        [Display(Name = "姓名"), StringLength(50)]
        public string UserName { get; set; }

        [Display(Name = "工号"), StringLength(50)]
        public string JobNum { get; set; }

        [Display(Name = "开瓶搅拌时间")]
        public DateTime? StartTime { get; set; }

        [Display(Name = "二次搅拌姓名"), StringLength(50)]
        public string SecondName { get; set; }

        [Display(Name = "二次搅拌工号"), StringLength(50)]
        public string SecondJobNum { get; set; }

        [Display(Name = "二次搅拌时间")]
        public DateTime? SecondTime { get; set; }

        [Display(Name = "三次搅拌姓名"), StringLength(50)]
        public string ThreeJobName { get; set; }

        [Display(Name = "三次搅拌工号"), StringLength(50)]
        public string ThreeNum { get; set; }

        [Display(Name = "三次搅拌时间")]
        public DateTime? ThreeTime { get; set; }

        [Display(Name = "四次搅拌姓名"), StringLength(50)]
        public string FlorName { get; set; }

        [Display(Name = "四次搅拌工号"), StringLength(50)]
        public string FlorJobNum { get; set; }

        [Display(Name = "四次搅拌时间")]
        public DateTime? FlorTime { get; set; }
    }

    public class SMT_Employ
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "锡膏条码"), StringLength(100)]
        public string SolderpasterBacrcode { get; set; }

        [Display(Name = "姓名"), StringLength(50)]
        public string UserName { get; set; }

        [Display(Name = "工号"), StringLength(50)]
        public string JobNum { get; set; }

        [Display(Name = "使用时间")]
        public DateTime? StartTime { get; set; }

        [Display(Name = "产线")]
        public int LineNum { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

    }
    public class SMT_Recycle
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "锡膏条码"), StringLength(100)]
        public string SolderpasterBacrcode { get; set; }

        [Display(Name = "姓名"), StringLength(50)]
        public string UserName { get; set; }

        [Display(Name = "工号"), StringLength(50)]
        public string JobNum { get; set; }

        [Display(Name = "回收时间")]
        public DateTime? RecoveTime { get; set; }
    }
}