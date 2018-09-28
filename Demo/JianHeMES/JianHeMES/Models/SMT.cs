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
        [Display(Name = "交货时间")]
        public DateTime? DeliveryDate { get; set; }

        public virtual List<SMT_ProductionPlan> SMT_ProductionPlan { get; set; }
        public virtual List<SMT_ProcutionLineInfo> SMT_ProcutionLineInfo { get; set; }
        public virtual List<SMT_ProductionData> SMT_ProductionData { get; set; }

    }

    public class SMT_ProductionPlan
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="产线")]
        public int LineNum { get; set; }

        [Display(Name ="订单")]
        public string OrderNum { get; set; }

        [Display(Name ="创建记录日期")]
        public DateTime? CreateDate { get; set; }

        public virtual List<SMT_ProcutionLineInfo> SMT_ProcutionLineInfo { get; set; }
        public virtual List<SMT_OrderInfo> SMT_OrderInfo { get; set; }
        public virtual List<SMT_ProductionData> SMT_ProductionData { get; set; }


    }

    public class SMT_ProcutionLineInfo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "产线")]
        public int LineNum { get; set; }

        [Display(Name = "正在生产的订单")]
        public string ProducingOrderNum { get; set; }

        [Display(Name = "创建记录日期")]
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

    //public class SMT_User
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    [Display(Name = "帐号")]
    //    public string User { get; set; }

    //    [Display(Name = "姓名")]
    //    public string Name { get; set; }

    //    [Display(Name = "密码")]
    //    public string PassWord { get; set; }

    //    [Display(Name = "产线")]
    //    public int LineNum { get; set; }

    //    [Display(Name = "角色")]
    //    public string Role { get; set; }

    //    [Display(Name = "描述")]
    //    public string Description { get; set; }

    //    public virtual List<SMT_ProductionPlan> SMT_ProductionPlan { get; set; }
    //    public virtual List<SMT_ProcutionLineInfo> SMT_ProcutionLineInfo { get; set; }
    //    public virtual List<SMT_ProductionData> SMT_ProductionData { get; set; }

    //}

    public class SMT_ProductionData
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "产线")]
        public int LineNum { get; set; }

        [Display(Name = "订单")]
        public string OrderNum { get; set; }

        [Display(Name = "良品数量")]
        public int NormalCount { get; set; }

        [Display(Name = "良品数量")]
        public int AbnormalCount { get; set; }

        [Display(Name = "日期")]
        public DateTime? ProductionDate { get; set; }

        public virtual List<SMT_ProductionPlan> SMT_ProductionPlan { get; set; }
        public virtual List<SMT_ProcutionLineInfo> SMT_ProcutionLineInfo { get; set; }
        public virtual List<SMT_OrderInfo> SMT_OrderInfo { get; set; }
    }
}