
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class OrderMgm
    {
        #region----------订单基本信息
        [Key]
        public int ID { get; set; }

        [Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "信息")]
        //[StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Required]
        [Display(Name = "条码前缀"), StringLength(50)]
        public string BarCode_Prefix { get; set; }

        [Required]
        [Display(Name = "客户名称"), StringLength(50)]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "下单日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ContractDate { get; set; }

        [Required]
        [Display(Name = "出货日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }

        [Required]
        [Display(Name = "计划投入时间")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PlanInputTime { get; set; }

        [Required]
        [Display(Name = "计划完成时间")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PlanCompleteTime { get; set; }

        [Display(Name = "平台型号"), StringLength(50)]
        public string PlatformType { get; set; }

        [Required]
        [Display(Name = "地区"), StringLength(50)]
        public string Area { get; set; }

        [Display(Name = "模块类型"), StringLength(50)]
        public string ModelType { get; set; }

        [Display(Name = "模框类型"), StringLength(50)]
        public string BoxType { get; set; }

        [Display(Name = "电源类型"), StringLength(50)]
        public string PowerType { get; set; }

        [Display(Name = "转接卡类型"), StringLength(50)]
        public string AdapterCardType { get; set; }

        [Display(Name ="制程要求"), StringLength(50)]//有铅，无铅
        public string ProcessingRequire { get; set; }

        [Display(Name = "标准要求"), StringLength(50)] //商用，军用
        public string StandardRequire { get; set; }

        [Display(Name = "备用字段")]
        public int Capacity { get; set; }

        //[Required]
        [Display(Name = "SMT产能")]
        public decimal CapacityQ { get; set; }

        [Display(Name = "小样进度"), StringLength(50)]
        public string HandSampleScedule { get; set; }

        [Display(Name = "装配负责部门"), StringLength(20)]
        public string AssembleDepartment { get; set; }

        [Required]
        [Display(Name = "模组数量")]
        public int Boxes { get; set; }

        [Display(Name = "模块数量")]
        public int Models { get; set; }

        [Display(Name = "额外预留模块数量")]
        public int ModelsMore { get; set; }

        [Display(Name = "电源数量")]
        public int Powers { get; set; }

        [Display(Name = "额外预留电源数量")]
        public int PowersMore { get; set; }

        [Display(Name = "转接卡")]
        public int AdapterCard { get; set; }

        [Display(Name = "额外预留转接卡")]
        public int AdapterCardMore { get; set; }

        [Display(Name = "订单生成日期")]
        [DataType(DataType.DateTime)]
        public DateTime? OrderCreateDate { get; set; }


        //修改为Module模组条码
        [Display(Name = "模组条码是否已经生成")]
        public int? BarCodeCreated { get; set; }

        [Display(Name = "模组条码生成日期")]
        [DataType(DataType.DateTime)]
        public DateTime? BarCodeCreateDate { get; set; }

        [Display(Name = "模组条码生成者"), StringLength(50)]
        public string BarCodeCreator { get; set; }

        //模块条码
        [Display(Name = "模块条码是否已经生成")]
        public int? ModulePieceBarCodeCreated { get; set; }

        [Display(Name = "模块条码生成日期")]
        [DataType(DataType.DateTime)]
        public DateTime? ModulePieceBarCodeCreateDate { get; set; }

        [Display(Name = "模块条码生成者"), StringLength(50)]
        public string ModulePieceBarCodeCreator { get; set; }

        //电源条码
        [Display(Name = "电源条码是否已经生成")]
        public int? PowerBarCodeCreated { get; set; }

        [Display(Name = "电源条码生成日期")]
        [DataType(DataType.DateTime)]
        public DateTime? PowerBarCodeCreateDate { get; set; }

        [Display(Name = "电源条码生成者"), StringLength(50)]
        public string PowerBarCodeCreator { get; set; }

        //转接卡条码
        [Display(Name = "转接卡条码是否已经生成")]
        public int? AdapterCardBarCodeCreated { get; set; }

        [Display(Name = "转接卡条码生成日期")]
        [DataType(DataType.DateTime)]
        public DateTime? AdapterCardBarCodeCreateDate { get; set; }

        [Display(Name = "转接卡条码生成者"), StringLength(50)]
        public string AdapterCardBarCodeCreator { get; set; }

        [Display(Name = "完工率")]
        public float? CompletedRate { get; set; }

        [Display(Name = "是否为库存")]
        public Boolean IsRepertory { get; set; }

        [Display(Name = "备注"), StringLength(500)]
        public string Remark { get; set; }
        #endregion


        #region ------------特采单内容

        [Display(Name = "是否为特采订单")]
        public Boolean IsAOD { get; set; }

        [Display(Name = "转特采订单用户"), StringLength(50)]
        public string AODConverter { get; set; }

        [Display(Name = "转特采订单日期")]
        [DataType(DataType.DateTime)]
        public DateTime? AODConvertDate { get; set; }

        [Display(Name = "AOD文件类型")]
        public List<string> AOD_FilesType { get; set; }

        [Display(Name = "AOD文件路径")]
        public List<string> AOD_FilesAddress { get; set; }

        [Display(Name = "AOD描述"), StringLength(500)]
        public string AOD_Description { get; set; }



        //PC部
        [Display(Name = "AOD_PC文件类型")]
        public List<string> AOD_PC_FilesType { get; set; }

        [Display(Name = "AOD_PC文件路径")]
        public List<string> AOD_PC_FilesAddress { get; set; }

        [Display(Name = "AOD_PC描述"), StringLength(500)]
        public string AOD_PC_Description { get; set; }


        //技术部
        [Display(Name = "AOD_TNG文件类型")]
        public List<string> AOD_TNG_FilesType { get; set; }

        [Display(Name = "AOD_TNG文件路径")]
        public List<string> AOD_TNG_FilesAddress { get; set; }

        [Display(Name = "AOD_TNG描述"), StringLength(500)]
        public string AOD_TNG_Description { get; set; }


        //装配一部
        [Display(Name = "AOD_ASSEMB文件类型")]
        public List<string> AOD_ASSEMB_FilesType { get; set; }

        [Display(Name = "AOD_ASSEMB文件路径")]
        public List<string> AOD_ASSEMB_FilesAddress { get; set; }

        [Display(Name = "AOD_ASSEMB描述"), StringLength(500)]
        public string AOD_ASSEMB_Description { get; set; }


        //品质部
        [Display(Name = "AOD_QA文件类型")]
        public List<string> AOD_QA_FilesType { get; set; }

        [Display(Name = "AOD_QA文件路径")]
        public List<string> AOD_QA_FilesAddress { get; set; }

        [Display(Name = "AOD_QA描述"), StringLength(500)]
        public string AOD_QA_Description { get; set; }

        #endregion


        #region ------------异常订单内容
        [Display(Name = "是否为异常订单")]
        public Boolean IsAbnormalOrder { get; set; }

        [Display(Name = "转异常订单用户"), StringLength(50)]
        public string AbnormalOrderConverter { get; set; }

        [Display(Name = "转异常订单日期")]
        [DataType(DataType.DateTime)]
        public DateTime? AbnormalOrderConvertDate { get; set; }

        [Display(Name = "异常描述"), StringLength(500)]
        public string AbnormalOrder_Description { get; set; }


        //组装
        [Display(Name = "是否为异常订单")]
        public Boolean IsAssembleAbnormal { get; set; }

        [Display(Name = "转异常订单用户"), StringLength(50)]
        public string AssembleAbnormalConverter { get; set; }

        [Display(Name = "转异常订单日期")]
        [DataType(DataType.DateTime)]
        public DateTime? AssembleAbnormalConvertDate { get; set; }

        [Display(Name = "异常描述"), StringLength(500)]
        public string AssembleAbnormal_Description { get; set; }

        //老化
        [Display(Name = "是否为异常订单")]
        public Boolean IsBurninAbnormal { get; set; }

        [Display(Name = "转异常订单用户"), StringLength(50)]
        public string BurninAbnormalConverter { get; set; }

        [Display(Name = "转异常订单日期")]
        [DataType(DataType.DateTime)]
        public DateTime? BurninAbnormalConvertDate { get; set; }

        [Display(Name = "异常描述"), StringLength(500)]
        public string BurninAbnormal_Description { get; set; }

        //包装
        [Display(Name = "是否为异常订单")]
        public Boolean IsAppearanceAbnormal { get; set; }

        [Display(Name = "转异常订单用户"), StringLength(50)]
        public string AppearanceAbnormalConverter { get; set; }

        [Display(Name = "转异常订单日期")]
        [DataType(DataType.DateTime)]
        public DateTime? AppearanceAbnormalConvertDate { get; set; }

        [Display(Name = "异常描述"), StringLength(500)]
        public string AppearanceAbnormal_Description { get; set; }

        #endregion


        #region ------------组装首件内容
        [Display(Name = "是否有组装首件")]
        public Boolean IsAssembleFirstSample { get; set; }

        [Display(Name = "确定组装首件帐号"),StringLength(20)]
        public string AssembleFirstSampleConverter { get; set; }

        [Display(Name = "确定组装首件日期")]
        [DataType(DataType.DateTime)]
        public DateTime? AssembleFirstSampleDate { get; set; }

        [Display(Name = "组装首件描述"),StringLength(200)]
        public string AssembleFirstSample_Description { get; set; }
        #endregion

        #region ------------老化首件内容
        [Display(Name = "是否有老化首件")]
        public Boolean IsBurnInFirstSample { get; set; }

        [Display(Name = "确定老化首件帐号"), StringLength(20)]
        public string BurnInFirstSampleConverter { get; set; }

        [Display(Name = "确定老化首件日期")]
        [DataType(DataType.DateTime)]
        public DateTime? BurnInFirstSampleDate { get; set; }

        [Display(Name = "老化首件描述"), StringLength(200)]
        public string BurnInFirstSample_Description { get; set; }
        #endregion

        #region ------------包装首件内容
        [Display(Name = "是否有包装首件")]
        public Boolean IsAppearanceFirstSample { get; set; }

        [Display(Name = "确定包装首件帐号"), StringLength(20)]
        public string AppearanceFirstSampleConverter { get; set; }

        [Display(Name = "确定包装首件日期")]
        [DataType(DataType.DateTime)]
        public DateTime? AppearanceFirstSampleDate { get; set; }

        [Display(Name = "包装首件描述"), StringLength(200)]
        public string AppearanceFirstSample_Description { get; set; }
        #endregion


        public virtual List<Assemble> Assemble { get; set; }
        public virtual List<Users> Users { get; set; }
        public virtual List<BarCodes> BarCodes { get; set; }

    }


    public class OrderMgm_Delete
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name ="删除日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DeleteDate { get; set; }

        [Display(Name ="删除人"), StringLength(50)]
        public string Deleter { get; set; }
    }

}