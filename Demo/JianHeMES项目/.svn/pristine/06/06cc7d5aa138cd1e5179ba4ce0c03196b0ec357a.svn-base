using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class Assemble
    {
        [Key]
        public int Id { get; set; }


        //模块组装部分
        [Display(Name = "订单号")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public string OrderNum { get; set; }      //订单号

        [Display(Name ="条码前缀")]
        public string BarCode_Prefix { get; set; }

        [Display(Name = "箱体模组条码")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "条码能为空")]
        public string BoxBarCode { get; set; }       //模组条码

        //[Key, Column(Order = 1)]
        [Display(Name = "模块条码清单")]
        public virtual List<string> ModelCollections { get; set; }    //模块条码数组   

        [Display(Name = "模块条码清单")]
        public string ModelList { get; set; }
        [Display(Name = "开始时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? AssembleBT { get; set; }   //组装开始时间

        [Display(Name = "模块组装负责人")]
        public string AssemblePrincipal { get; set; }   //模块组装负责人

        [Display(Name = "完成时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? AssembleFT { get; set; }   //组装完成时间

        [Display(Name = "组装时长")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public TimeSpan? AssembleTime { get; set; }  //组装时长

        [Display(Name = "是否已经完成组装")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public Boolean AssembleFinish { get; set; }  //是否已经完成组装

        //防水测试部分
        [Display(Name = "开始时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? WaterproofTestBT { get; set; }  //防水测试开始时间

        [Display(Name = "防水测试负责人")]
        public string WaterproofTestPrincipal { get; set; }   //防水测试负责人
        
        [Display(Name = "完成时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? WaterproofTestFT { get; set; }  //防水测试完成时间

        [Display(Name = "防水测试时长")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public TimeSpan? WaterproofTestTimeSpan { get; set; }  //防水测试时长

        [Display(Name = "异常代码")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public int WaterproofAbnormal { get; set; } //防水测试异常代码

        [Display(Name = "防水维修情况")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public string WaterproofMaintaince { get; set; }  //防水维修情况

        [Display(Name = "防水是否完成")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public Boolean WaterproofTestFinish { get; set; }  //防水是否完成


        //转接卡安装部分
        [Display(Name = "开始时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? AssembleAdapterCardBT { get; set; }   //安装转接卡开始时间

        [Display(Name = "安装转接卡负责人")]
        public string AssembleAdapterCardPrincipal { get; set; }//安装转接卡负责人

        [Display(Name = "完成时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? AssembleAdapterCardFT { get; set; }   //安装转接卡完成时间

        [Display(Name = "转接卡、电源清单")]
        public virtual List<string> AdapterCard_Power_Collection { get; set; }    //模块条码数组   

        [Display(Name = "转接卡、电源清单")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public string AdapterCard_Power_List { get; set; }   //安装转接卡完成时间

        [Display(Name = "安装时长")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public TimeSpan? AssembleAdapterTime { get; set; }   //转接卡安装时长

        [Display(Name = "转接卡是否安装完成")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public Boolean AssembleAdapterFinish { get; set; }  //转接卡是否安装完成


        //视检部分
        [Display(Name = "开始时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? ViewCheckBT { get; set; } //视检开始时间

        [Display(Name = "视检负责人")]
        public string AssembleViewCheckPrincipal { get; set; }  //视检负责人

        [Display(Name = "完成时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? ViewCheckFT { get; set; }  //视检完成时间

        [Display(Name = "视检时长")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public TimeSpan? ViewCheckTime { get; set; }  //视检时长

        [Display(Name = "视检异常")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public int ViewCheckAbnormal { get; set; } //视检异常
        [Display(Name = "视检是否完成")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public Boolean ViewCheckFinish { get; set; } //视检是否完成


        //电检部分
        [Display(Name = "开始时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? ElectricityCheckBT { get; set; } //电检开始时间
        
        [Display(Name = "电检负责人")]
        public string AssembleElectricityCheckPrincipal { get; set; }  //电检负责人
        
        [Display(Name = "完成时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? ElectricityCheckFT { get; set; }  //电检完成时间

        [Display(Name = "电检时长")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public TimeSpan? ElectricityCheckTime { get; set; }  //电检时长

        [Display(Name = "电检异常")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public int ElectricityCheckAbnormal { get; set; } //电检异常

        [Display(Name = "电检是否完成")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public Boolean ElectricityCheckFinish { get; set; } //电检是否完成



        //PQC部分
        [Display(Name = "开始时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? PQCCheckBT { get; set; } //PQC开始时间

        [Display(Name = "PQC负责人")]
        public string AssemblePQCPrincipal { get; set; }  //PQC负责人

        [Display(Name = "产线")]
        public int AssembleLineId { get; set; }    //产线ID号
        
        [Display(Name = "完成时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? PQCCheckFT { get; set; }  //PQC完成时间

        [Display(Name = "PQC时长(天)")]
        public int PQCCheckDate { get; set; }  //PQC时长

        [Display(Name = "PQC时长(时间)")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public TimeSpan? PQCCheckTime { get; set; }  //PQC时长

        [Display(Name = "PQC异常")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public string PQCCheckAbnormal { get; set; } //PQC异常

        [Display(Name ="维修情况")]
        public string PQCRepairCondition { get; set; }

        [Display(Name = "PQC是否完成")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public Boolean PQCCheckFinish { get; set; } //PQC是否完成

        [Display(Name = "是否重复PQC")]
        public Boolean RepetitionPQCCheck { get; set; }

        [Display(Name = "重复原因")]
        public string RepetitionPQCCheckCause { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; } //PQC是否完成


        public virtual List<OrderMgm> OrderMgm { get; set; }
        public virtual List<Users> Users { get; set; }
        public virtual List<BarCodes> BarCodes { get; set; }

    }

    public class AssembleLineId
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "产线ID号")]
        public string LineID { get; set; }

        [Display(Name = "产线名称")]
        public string AssembleLineName { get; set; }

        [Display(Name = "产线描述")]
        public string AssembleLineDiscription { get; set; }

    }

    public class ModelCollections
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "安装位置Id")]
        public string StationId { get; set; }

        [Display(Name = "订单号")]
        public string OrderNum { get; set; }      //订单号

        //[ForeignKey(BoxBarCode)]
        [Display(Name = "模组条码")]
        public string BoxBarCode { get; set; }       //模组条码

        [Display(Name = "模块条码")]
        public string BarCodesNum { get; set; }
    }

    public class Waterproofabnormal
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "防水测试异常代码")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public int Code { get; set; }
        [Display(Name = "描述")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public string DetialedDescription { get; set; }
    }


    public class AdapterCard_Power_Collection
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号")]
        public string OrderNum { get; set; }      //订单号

        //[ForeignKey(BoxBarCode)]
        [Display(Name = "模组条码")]
        public string BoxBarCode { get; set; }       //模组条码

        [Display(Name = "电源、转接卡条码")]
        public string BarCodesNum { get; set; }

    }
    public class ViewCheckabnormal
    {
        [Key]
        public int Id { get; set; }
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        [Display(Name = "频检异常代码")]
        public int Code { get; set; }
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        [Display(Name = "描述")]
        public string DetialedDescription { get; set; }
    }

    public class PQCCheckabnormal
    {
        [Key]
        public int Id { get; set; }
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        [Display(Name = "PQC异常代码")]
        public int Code { get; set; }
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        [Display(Name = "描述")]
        public string DetialedDescription { get; set; }
    }



}