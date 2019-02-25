using System;
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

        [Display(Name = "订单号")]
        public string OrderNum { get; set; }

        [Display(Name = "数量")]
        public int Quantity { get; set; }

        [Display(Name = "平台类型")]

        public string PlatformType { get; set; }

        [Display(Name = "客户")]
        public string Customer { get; set; }

        [Display(Name = "交货时间"),DataType(DataType.DateTime)]
        public DateTime? DeliveryDate { get; set; }

        [Display(Name = "完成状态")]
        public Boolean Status { get; set; }

        public virtual List<SMT_ProductionPlan> SMT_ProductionPlan { get; set; }
        public virtual List<SMT_ProductionLineInfo> SMT_ProcutionLineInfo { get; set; }
        public virtual List<SMT_ProductionData> SMT_ProductionData { get; set; }

    }

    public class SMT_ProductionPlan
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="产线")]
        public int LineNum { get; set; }

        [Display(Name ="订单号")]
        public string OrderNum { get; set; }

        [Display(Name = "计划产能")]
        public decimal Capacity { get; set; }

        [Display(Name = "计划数量")]
        public int Quantity { get; set; }

        [Display(Name = "工作内容")]
        public string JobContent { get; set; }

        [Display(Name ="创建生产计划日期"),DataType(DataType.DateTime)]  
        public DateTime? CreateDate { get; set; }

        [Display(Name = "计划生产日期"), DataType(DataType.DateTime)]
        public DateTime? PlanProductionDate { get; set; }

        [Display(Name = "计划创建人")]
        public string Creator { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }


        public virtual List<SMT_ProductionLineInfo> SMT_ProcutionLineInfo { get; set; }
        public virtual List<SMT_OrderInfo> SMT_OrderInfo { get; set; }
        public virtual List<SMT_ProductionData> SMT_ProductionData { get; set; }


    }

    public class SMT_ProductionLineInfo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "产线")]
        public int LineNum { get; set; }

        [Display(Name = "正在生产的订单")]
        public string ProducingOrderNum { get; set; }

        [Display(Name = "创建记录日期"),DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "班组")]
        public string Team { get; set; }

        [Display(Name = "组长")]
        public string GroupLeader { get; set; }

        [Display(Name = "产线状态")]
        public string Status { get; set; }

        public virtual List<SMT_ProductionPlan> SMT_ProductionPlan { get; set; }
        public virtual List<SMT_OrderInfo> SMT_OrderInfo { get; set; }
        public virtual List<SMT_ProductionData> SMT_ProductionData { get; set; }

    }


    public class SMT_ProductionData
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "产线")]
        public int LineNum { get; set; }

        [Display(Name = "订单")]
        public string OrderNum { get; set; }

        [Display(Name = "工作内容")]
        public string JobContent { get; set; }

        [Display(Name = "良品数量")]
        public int NormalCount { get; set; }

        [Display(Name = "不良品数量")]
        public int AbnormalCount { get; set; }

        [Display(Name = "开始时间"), DataType(DataType.DateTime)]
        public DateTime? BeginTime { get; set; }

        [Display(Name = "结束时间"), DataType(DataType.DateTime)]
        public DateTime? EndTime { get; set; }

        [Display(Name = "条码号")]
        public string BarcodeNum { get; set; }

        [Display(Name = "产品状态")]
        public bool Result { get; set; }

        [Display(Name = "日期"),DataType(DataType.DateTime)]
        public DateTime? ProductionDate { get; set; }

        [Display(Name ="操作员")]
        public string Operator { get; set; }


        public virtual List<SMT_ProductionPlan> SMT_ProductionPlan { get; set; }
        public virtual List<SMT_ProductionLineInfo> SMT_ProcutionLineInfo { get; set; }
        public virtual List<SMT_OrderInfo> SMT_OrderInfo { get; set; }
    }
}