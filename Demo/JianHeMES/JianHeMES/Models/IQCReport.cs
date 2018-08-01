using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class IQCReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name ="物料编号")]
        public string Material_SN { get; set; }

        [Required]
        [Display(Name ="物料需要符合RoHS、REACH要求")]
        public Boolean? RoHS_REACH { get; set; }

        [Display(Name ="订单号")]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name ="测试设备编号")]
        public string EquipmentNum { get; set; }

        [Required]
        [Display(Name = "供应商")]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "物料名称")]
        public string MaterialName { get; set; }

        [Required]
        [Display(Name = "型号")]
        public string ModelNumber { get; set; }

        [Required]
        [Display(Name = "规格")]
        public string Specification { get; set; }

        [Required]
        [Display(Name = "来料数量")]
        public string MaterialQuantity { get; set; }

        [Required]
        [Display(Name = "来料日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode =true)]
        public DateTime? IncomingDate { get; set; }

        [Required]
        [Display(Name = "请购单号")]
        public string ApplyPurchaseOrderNum { get; set; }

        [Required]
        [Display(Name = "批号")]
        public string BatchNum { get; set; }

        [Required]
        [Display(Name = "检验日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode =true)]
        public DateTime? InspectionDate { get; set; }

        [Required]
        [Display(Name = "抽样方案")]
        public string SamplingPlan { get; set; }

        [Display(Name = "承认书或图纸号/版本")]
        public string MaterialVersion { get; set; }

        [Display(Name = "标识检验水平")]
        public string C1 { get; set; }

        [Display(Name = "标识抽样数量")]
        public string C2 { get; set; }

        [Display(Name = "标识判定标准严重AC")]
        public string C3 { get; set; }

        [Display(Name = "标识判定标准严重RE")]
        public string C4 { get; set; }

        [Display(Name = "标识判定标准一般AC")]
        public string C5 { get; set; }

        [Display(Name = "标识判定标准一般RE")]
        public string C6 { get; set; }

        [Display(Name = "标识判定标准轻微AC")]
        public string C7 { get; set; }

        [Display(Name = "标识判定标准轻微RE")]
        public string C8 { get; set; }

        [Display(Name = "标识检验判定")]
        public string C9 { get; set; }

        [Display(Name = "包装、外观检验水平")]
        public string D1 { get; set; }

        [Display(Name = "包装、外观抽样数量")]
        public string D2 { get; set; }

        [Display(Name = "包装、外观判定标准严重AC")]
        public string D3 { get; set; }

        [Display(Name = "包装、外观判定标准严重RE")]
        public string D4 { get; set; }

        [Display(Name = "包装、外观判定标准一般AC")]
        public string D5 { get; set; }

        [Display(Name = "包装、外观判定标准一般RE")]
        public string D6 { get; set; }

        [Display(Name = "包装、外观判定标准轻微AC")]
        public string D7 { get; set; }

        [Display(Name = "包装、外观判定标准轻微RE")]
        public string D8 { get; set; }

        [Display(Name = "包装、外观检验判定")]
        public string D9 { get; set; }

        [Display(Name = "尺寸检验水平")]
        public string E1 { get; set; }

        [Display(Name = "尺寸抽样数量")]
        public string E2 { get; set; }

        [Display(Name = "尺寸判定标准严重AC")]
        public string E3 { get; set; }

        [Display(Name = "尺寸判定标准严重RE")]
        public string E4 { get; set; }

        [Display(Name = "尺寸判定标准一般AC")]
        public string E5 { get; set; }

        [Display(Name = "尺寸判定标准一般RE")]
        public string E6 { get; set; }

        [Display(Name = "尺寸判定标准轻微AC")]
        public string E7 { get; set; }

        [Display(Name = "尺寸判定标准轻微RE")]
        public string E8 { get; set; }

        [Display(Name = "尺寸检验判定")]
        public string E9 { get; set; }

        [Display(Name = "性能/参数检验水平")]
        public string F1 { get; set; }

        [Display(Name = "性能/参数抽样数量")]
        public string F2 { get; set; }

        [Display(Name = "性能/参数判定标准严重AC")]
        public string F3 { get; set; }

        [Display(Name = "性能/参数判定标准严重RE")]
        public string F4 { get; set; }

        [Display(Name = "性能/参数判定标准一般AC")]
        public string F5 { get; set; }

        [Display(Name = "性能/参数判定标准一般RE")]
        public string F6 { get; set; }

        [Display(Name = "性能/参数判定标准轻微AC")]
        public string F7 { get; set; }

        [Display(Name = "性能/参数判定标准轻微RE")]
        public string F8 { get; set; }

        [Display(Name = "性能/参数检验判定")]
        public string F9 { get; set; }

        [Display(Name = "不良项目(检测项目)S0")]
        public string S0 { get; set; }

        [Display(Name = "规格要求S1")]
        public string S1 { get; set; }
        [Display(Name = "检验结果描述S11")]
        public string S11 { get; set; }
        [Display(Name = "检验结果描述S12")]
        public string S12 { get; set; }
        [Display(Name = "检验结果描述S13")]
        public string S13 { get; set; }
        [Display(Name = "检验结果描述S14")]
        public string S14 { get; set; }
        [Display(Name = "检验结果描述S15")]
        public string S15 { get; set; }

        [Display(Name = "规格要求S2")]
        public string S2 { get; set; }
        [Display(Name = "检验结果描述S21")]
        public string S21 { get; set; }
        [Display(Name = "检验结果描述S22")]
        public string S22 { get; set; }
        [Display(Name = "检验结果描述S23")]
        public string S23 { get; set; }
        [Display(Name = "检验结果描述S24")]
        public string S24 { get; set; }
        [Display(Name = "检验结果描述S25")]
        public string S25 { get; set; }

        [Display(Name = "规格要求S3")]
        public string S3 { get; set; }
        [Display(Name = "检验结果描述S31")]
        public string S31 { get; set; }
        [Display(Name = "检验结果描述S32")]
        public string S32 { get; set; }
        [Display(Name = "检验结果描述S33")]
        public string S33 { get; set; }
        [Display(Name = "检验结果描述S34")]
        public string S34 { get; set; }
        [Display(Name = "检验结果描述S35")]
        public string S35 { get; set; }


        [Display(Name = "不良项目(检测项目)SR")]
        public string SR { get; set; }

        [Display(Name = "规格要求SR1-SR5和检验结果描述SR101-SR510")]
        public string SRJson { get; set; }
        //[Display(Name = "规格要求SR1")]
        //public string SR1 { get; set; }
        //[Display(Name = "检验结果描述SR101")]
        //public float SR101 { get; set; }
        //[Display(Name = "检验结果描述SR102")]
        //public float SR102 { get; set; }
        //[Display(Name = "检验结果描述SR103")]
        //public float SR103 { get; set; }
        //[Display(Name = "检验结果描述SR104")]
        //public float SR104 { get; set; }
        //[Display(Name = "检验结果描述SR105")]
        //public float SR105 { get; set; }
        //[Display(Name = "检验结果描述SR106")]
        //public float SR106 { get; set; }
        //[Display(Name = "检验结果描述SR107")]
        //public float SR107 { get; set; }
        //[Display(Name = "检验结果描述SR108")]
        //public float SR108 { get; set; }
        //[Display(Name = "检验结果描述SR109")]
        //public float SR109 { get; set; }
        //[Display(Name = "检验结果描述SR110")]
        //public float SR110 { get; set; }

        //[Display(Name = "规格要求SR2")]
        //public string SR2 { get; set; }
        //[Display(Name = "检验结果描述SR201")]
        //public float SR201 { get; set; }
        //[Display(Name = "检验结果描述SR202")]
        //public float SR202 { get; set; }
        //[Display(Name = "检验结果描述SR203")]
        //public float SR203 { get; set; }
        //[Display(Name = "检验结果描述SR204")]
        //public float SR204 { get; set; }
        //[Display(Name = "检验结果描述SR205")]
        //public float SR205 { get; set; }
        //[Display(Name = "检验结果描述SR206")]
        //public float SR206 { get; set; }
        //[Display(Name = "检验结果描述SR207")]
        //public float SR207 { get; set; }
        //[Display(Name = "检验结果描述SR208")]
        //public float SR208 { get; set; }
        //[Display(Name = "检验结果描述SR209")]
        //public float SR209 { get; set; }
        //[Display(Name = "检验结果描述SR210")]
        //public float SR210 { get; set; }

        //[Display(Name = "规格要求SR3")]
        //public string SR3 { get; set; }
        //[Display(Name = "检验结果描述SR301")]
        //public float SR301 { get; set; }
        //[Display(Name = "检验结果描述SR302")]
        //public float SR302 { get; set; }
        //[Display(Name = "检验结果描述SR303")]
        //public float SR303 { get; set; }
        //[Display(Name = "检验结果描述SR304")]
        //public float SR304 { get; set; }
        //[Display(Name = "检验结果描述SR305")]
        //public float SR305 { get; set; }
        //[Display(Name = "检验结果描述SR306")]
        //public float SR306 { get; set; }
        //[Display(Name = "检验结果描述SR307")]
        //public float SR307 { get; set; }
        //[Display(Name = "检验结果描述SR308")]
        //public float SR308 { get; set; }
        //[Display(Name = "检验结果描述SR309")]
        //public float SR309 { get; set; }
        //[Display(Name = "检验结果描述SR310")]
        //public float SR310 { get; set; }

        //[Display(Name = "规格要求SR4")]
        //public string SR4 { get; set; }
        //[Display(Name = "检验结果描述SR401")]
        //public float SR401 { get; set; }
        //[Display(Name = "检验结果描述SR402")]
        //public float SR402 { get; set; }
        //[Display(Name = "检验结果描述SR403")]
        //public float SR403 { get; set; }
        //[Display(Name = "检验结果描述SR404")]
        //public float SR404 { get; set; }
        //[Display(Name = "检验结果描述SR405")]
        //public float SR405 { get; set; }
        //[Display(Name = "检验结果描述SR406")]
        //public float SR406 { get; set; }
        //[Display(Name = "检验结果描述SR407")]
        //public float SR407 { get; set; }
        //[Display(Name = "检验结果描述SR408")]
        //public float SR408 { get; set; }
        //[Display(Name = "检验结果描述SR409")]
        //public float SR409 { get; set; }
        //[Display(Name = "检验结果描述SR410")]
        //public float SR410 { get; set; }

        //[Display(Name = "规格要求SR5")]
        //public string SR5 { get; set; }
        //[Display(Name = "检验结果描述SR501")]
        //public float SR501 { get; set; }
        //[Display(Name = "检验结果描述SR502")]
        //public float SR502 { get; set; }
        //[Display(Name = "检验结果描述SR503")]
        //public float SR503 { get; set; }
        //[Display(Name = "检验结果描述SR504")]
        //public float SR504 { get; set; }
        //[Display(Name = "检验结果描述SR505")]
        //public float SR505 { get; set; }
        //[Display(Name = "检验结果描述SR506")]
        //public float SR506 { get; set; }
        //[Display(Name = "检验结果描述SR507")]
        //public float SR507 { get; set; }
        //[Display(Name = "检验结果描述SR508")]
        //public float SR508 { get; set; }
        //[Display(Name = "检验结果描述SR509")]
        //public float SR509 { get; set; }
        //[Display(Name = "检验结果描述SR510")]
        //public float SR510 { get; set; }
        

        [Display(Name = "不良项目(检测项目)R0")]
        public string R0 { get; set; }

        [Display(Name = "规格要求R1")]
        public string R1 { get; set; }
        [Display(Name = "检验结果描述R11")]
        public string R11 { get; set; }
        [Display(Name = "检验结果描述R12")]
        public string R12 { get; set; }
        [Display(Name = "检验结果描述R13")]
        public string R13 { get; set; }

        [Display(Name = "规格要求R2")]
        public string R2 { get; set; }
        [Display(Name = "检验结果描述R21")]
        public string R21 { get; set; }
        [Display(Name = "检验结果描述R22")]
        public string R22 { get; set; }
        [Display(Name = "检验结果描述R23")]
        public string R23 { get; set; }

        [Display(Name = "规格要求R3")]
        public string R3 { get; set; }
        [Display(Name = "检验结果描述R31")]
        public string R31 { get; set; }
        [Display(Name = "检验结果描述R32")]
        public string R32 { get; set; }
        [Display(Name = "检验结果描述R33")]
        public string R33 { get; set; }

        [Display(Name = "不良项目(检测项目)P0")]
        public string P0 { get; set; }

        [Display(Name = "规格要求P1")]
        public string P1 { get; set; }
        [Display(Name = "检验结果描述P11")]
        public string P11 { get; set; }
        [Display(Name = "检验结果描述P12")]
        public string P12 { get; set; }
        [Display(Name = "检验结果描述P13")]
        public string P13 { get; set; }

        [Display(Name = "规格要求P2")]
        public string P2 { get; set; }
        [Display(Name = "检验结果描述P21")]
        public string P21 { get; set; }
        [Display(Name = "检验结果描述P22")]
        public string P22 { get; set; }
        [Display(Name = "检验结果描述P23")]
        public string P23 { get; set; }

        [Display(Name = "规格要求P3")]
        public string P3 { get; set; }
        [Display(Name = "检验结果描述P31")]
        public string P31 { get; set; }

        [Display(Name = "检验结果描述P32")]
        public string P32 { get; set; }
        [Display(Name = "检验结果描述P33")]
        public string P33 { get; set; }

        [Display(Name = "我司测试条件R=G=B=8mA")]
        public string AM { get; set; }

        [Display(Name = "不良项目(检测项目)AM0")]
        public string AM0 { get; set; }

        //规格要求AM1
        [Display(Name = "规格要求AM1")]
        public string AM1 { get; set; }
        [Display(Name = "检验结果描述AM11")]
        public string AM11 { get; set; }
        [Display(Name = "检验结果描述AM12")]
        public string AM12 { get; set; }
        [Display(Name = "检验结果描述AM13")]
        public string AM13 { get; set; }

        [Display(Name = "规格要求AM2")]
        public string AM2 { get; set; }
        [Display(Name = "检验结果描述AM21")]
        public string AM21 { get; set; }
        [Display(Name = "检验结果描述AM22")]
        public string AM22 { get; set; }
        [Display(Name = "检验结果描述AM23")]
        public string AM23 { get; set; }

        [Display(Name = "规格要求AM3")]
        public string AM3 { get; set; }
        [Display(Name = "检验结果描述AM31")]
        public string AM31 { get; set; }
        [Display(Name = "检验结果描述AM32")]
        public string AM32 { get; set; }
        [Display(Name = "检验结果描述AM33")]
        public string AM33 { get; set; }

        [Display(Name = "规格要求AM4")]
        public string AM4 { get; set; }
        [Display(Name = "检验结果描述AM41")]
        public string AM41 { get; set; }
        [Display(Name = "检验结果描述AM42")]
        public string AM42 { get; set; }
        [Display(Name = "检验结果描述AM43")]
        public string AM43 { get; set; }

        [Display(Name = "总不良数(严重缺陷)")]
        public string NG1 { get; set; }

        [Display(Name = "总不良数(一般缺陷)")]
        public string NG2 { get; set; }

        [Display(Name = "总不良数(轻微缺陷)")]
        public string NG3 { get; set; }

        [Required]
        [Display(Name = "最终判定")]  //true=允收，false=拒收
        public Boolean? NGD { get; set; }

        [Display(Name = "不合格批处理")]
        public string NGHandle { get; set; }

        [Display(Name = "备注")]
        public string ReportRemark { get; set; }

        [Display(Name = "检验人")]
        public string Inspector { get; set; }

        [Display(Name = "报告创建人")]
        public string Creator { get; set; }

        [Display(Name = "报告创建时间")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString ="{0:yyyy/MM/dd}",ApplyFormatInEditMode =true)]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "审核人")]
        public string Assessor { get; set; }

        [Display(Name = "报告审核时间")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString ="{0:yyyy/MM/dd}",ApplyFormatInEditMode =true)]
        public DateTime? AssessedDate { get; set; }

        [Display(Name = "审核人评语")]
        public string AssessorRemark { get; set; }

        [Display(Name = "审核是否通过")]
        public Boolean? AssessorPass { get; set; }

        [Display(Name = "批准人：")]
        public string Approve { get; set; }

        [Display(Name = "报告批准时间")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString ="{0:yyyy/MM/dd}",ApplyFormatInEditMode =true)]
        public DateTime? ApprovedDate { get; set; }

        [Display(Name = "批准人评语")]
        public string ApproveRemark { get; set; }

        [Display(Name = "批准是否通过")]
        public Boolean? ApprovePass { get; set; }

    }

    public class IQCEquipment
    {
        public int Id { get; set; }
        public string EquipmentNum { get; set; }
        public string EquipmentName { get; set; }
        public string Manufacturer { get; set; }
        public string Discription { get; set; }
    }

    public class IQCProvider
    {
        public int Id { get; set; }
        public string ProviderNum { get; set; }
        public string ProviderName { get; set; }
        public string Discription { get; set; }

    }

}