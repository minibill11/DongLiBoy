using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    //客诉订单表
    public class Customer_Complaints
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "客诉编号"), StringLength(100)]
        public string ComplaintNumber { get; set; }

        [Display(Name = "客户名称"), StringLength(100)]
        public string CustomerName { get; set; }

        [Display(Name = "订单号"), StringLength(200)]
        public string OrderNum { get; set; }

        [Display(Name = "模组数量/面积"), StringLength(200)]
        public string ModuleNumber { get; set; }

        [Display(Name = "所需区域"), StringLength(200)]
        public string RequiredArea { get; set; }

        [Display(Name = "产品型号"), StringLength(200)]
        public string ProductModel { get; set; }

        [Display(Name = "发货日期"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DeliveryDate { get; set; }

        [Display(Name = "投诉日期"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ComplaintsDate { get; set; }

        [Display(Name = "屏体面积"), StringLength(200)]
        public string ScreenArea { get; set; }

        [Display(Name = "控制系统及电源"), StringLength(200)]
        public string Control { get; set; }

        [Display(Name = "投诉内容"), StringLength(600)]
        public string Complaint_Content { get; set; }

        [Display(Name = "异常描述"), StringLength(600)]
        public string Abnormal_Describe { get; set; }

        [Display(Name = "原因分析"), StringLength(600)]
        public string Cause_Analysis { get; set; }

        [Display(Name = "临时处理措施"), StringLength(600)]
        public string Interim_Disposal { get; set; }

        [Display(Name = "长期处理措施"), StringLength(600)]
        public string Longterm_Treatment { get; set; }

        [Display(Name = "责任归属"), StringLength(100)]
        public string Liability { get; set; }

        [Display(Name = "最终处理结果"), StringLength(600)]
        public string Final_Result { get; set; }

        [Display(Name = "备注"), StringLength(300)]
        public string Remark { get; set; }

        [Display(Name = "结案日期"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? SettlementDate { get; set; }

        [Display(Name = "责任部门确认"), StringLength(50)]
        public string ResponDepartment { get; set; }

        [Display(Name = "责任部门确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ResponDate { get; set; }

        [Display(Name = "国内/外工程部确认"), StringLength(50)]
        public string EngineDepartment { get; set; }

        [Display(Name = "国内/外工程部确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? EngineDate { get; set; }

        [Display(Name = "品质部确认"), StringLength(50)]
        public string QualityDepartment { get; set; }

        [Display(Name = "品质部确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? QualityDate { get; set; }

        [Display(Name = "品技中心确认"), StringLength(50)]
        public string Quality_TechDepartment { get; set; }

        [Display(Name = "品技中心确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? QualityTechDate { get; set; }

        [Display(Name = "运营副总确认"), StringLength(50)]
        public string Operation { get; set; }

        [Display(Name = "运营副总确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? OperationDate { get; set; }

        [Display(Name = "副总经理确认"), StringLength(50)]
        public string VicePresident { get; set; }

        [Display(Name = "副总经理确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? VicePresDate { get; set; }

        [Display(Name = "总经理确认"), StringLength(50)]
        public string General_Manager { get; set; }

        [Display(Name = "总经理确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? GeneralDate { get; set; }

        [Display(Name = "创建人"), StringLength(50)]
        public string Creator { get; set; }

        [Display(Name = "创建时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "记录修改人"), StringLength(50)]
        public string Modifier { get; set; }

        [Display(Name = "修改时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }

    }

    //客诉订单表—投诉代码
    public class Customer_ComplaintsCode
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "外键")]
        public int OutsideKey { get; set; }

        [Display(Name = "投诉代码"), StringLength(50)]
        public string Complaintscode { get; set; }

        [Display(Name = "投诉代码-其他"), StringLength(50)]
        public string Complaintsother { get; set; }

        [Display(Name = "数量")]
        public int CodeNumber { get; set; }

    }

    //客诉损失明细表
    public class Customer_AttachmentLoss
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "客户名称"), StringLength(100)]
        public string CustomerName { get; set; }

        [Display(Name = "出货日期"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DeliveryDate { get; set; }

        [Display(Name = "投诉日期"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ComplaintsDate { get; set; }

        [Display(Name = "结案日期"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? SettlementDate { get; set; }

        [Display(Name = "产品型号/显示屏规格"), StringLength(100)]
        public string ProductModel { get; set; }

        [Display(Name = "订单号"), StringLength(300)]
        public string OrderNum { get; set; }

        [Display(Name = "合同号"), StringLength(100)]
        public string ContractNum { get; set; }

        [Display(Name = "平台"), StringLength(60)]
        public string Platform { get; set; }

        [Display(Name = "客诉现象/投诉内容"), StringLength(200)]
        public string Customer_phenomenon { get; set; }

        [Display(Name = "原因分析"), StringLength(600)]
        public string Cause_Analysis { get; set; }

        [Display(Name = "临时处理措施"), StringLength(500)]
        public string Interim_Disposal { get; set; }

        [Display(Name = "长期处理措施"), StringLength(500)]
        public string Longterm_Treatment { get; set; }

        [Display(Name = "索赔情况"), StringLength(100)]
        public string ClaimIs { get; set; }

        [Display(Name = "索赔确认"), StringLength(50)]
        public string ClaimConfirm { get; set; }

        [Display(Name = "合同金额(元)")]
        public decimal Contract_Amount { get; set; }

        [Display(Name = "损失金额(元)")]
        public decimal Losses_Amount { get; set; }

        [Display(Name = "损失率(%)")]
        public double LossRate { get; set; }

        [Display(Name = "客损归属-年")]
        public int Year { get; set; }

        [Display(Name = "客损归属-月")]
        public int Month { get; set; }

        [Display(Name = "不良类别"), StringLength(100)]
        public string BadType { get; set; }

        [Display(Name = "故障分类/产品型号"), StringLength(100)]
        public string Fault_Classification { get; set; }

        [Display(Name = "分类"), StringLength(100)]
        public string Classification { get; set; }

        [Display(Name = "备注"), StringLength(500)]
        public string Remark { get; set; }

        [Display(Name = "创建人"), StringLength(50)]
        public string Creator { get; set; }

        [Display(Name = "创建时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "记录修改人"), StringLength(50)]
        public string Modifier { get; set; }

        [Display(Name = "修改时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }

        [Display(Name = "审核人"), StringLength(50)]
        public string Audit { get; set; }

        [Display(Name = "审核时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? AuditDate { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }
    }

    //客损质量损失/责任判定
    public class Customer_Attachment_QualityLoss
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "外键")]
        public int OutsideKey { get; set; }

        [Display(Name = "质量损失(金额:元)")]
        public decimal QualityLoss { get; set; }

        [Display(Name = "责任判定"), StringLength(50)]
        public string Responsibility { get; set; }
    }


    //故障分类管理
    public class Customer_FaultTypes
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "大分类（故障类）"), StringLength(100)]
        public string Classification { get; set; }

        [Display(Name = "小分类（故障类）"), StringLength(100)]
        public string Fault_Classification { get; set; }

        [Display(Name = "备注"), StringLength(500)]
        public string Remark { get; set; }

        [Display(Name = "创建人"), StringLength(50)]
        public string Creator { get; set; }

        [Display(Name = "创建时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "记录修改人"), StringLength(50)]
        public string Modifier { get; set; }

        [Display(Name = "修改时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }

    }

}