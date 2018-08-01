using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class OrderMgm
    {
        public int ID { get; set; }

        [Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "密码")]
        //[StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [Display(Name = "订单号")]
        public string OrderNum { get; set; }

        [Required]
        [Display(Name = "条码前缀")]
        public string BarCode_Prefix { get; set; }
        
        [Required]
        [Display(Name = "客户名称")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "订单日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ContractDate { get; set; }

        [Required]
        [Display(Name = "交货日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }

        [Required]
        [Display(Name = "地区")]
        public string Area { get; set; }

        [Display(Name = "模块类型")]
        public string ModelType { get; set; }

        [Display(Name = "模框类型")]
        public string BoxType { get; set; }

        [Display(Name = "电源类型")]
        public string PowerType { get; set; }

        [Display(Name = "转接卡类型")]
        public string AdapterCardType { get; set; }

        [Required]
        [Display(Name = "模组数量")]
        public int Boxes { get; set; }

        //[Required]
        [Display(Name = "模块数量")]
        public int Models { get; set; }

        //[Required]
        [Display(Name = "额外预留模块数量")]
        public int ModelsMore { get; set; }

        //[Required]
        [Display(Name = "电源数量")]
        public int Powers { get; set; }

        //[Required]
        [Display(Name = "额外预留电源数量")]
        public int PowersMore { get; set; }

        //[Required]
        [Display(Name = "转接卡")]
        public int AdapterCard { get; set; }

        //[Required]
        [Display(Name = "额外预留转接卡")]
        public int AdapterCardMore { get; set; }

        [Display(Name = "订单生成日期")]
        [DataType(DataType.DateTime)]
        public DateTime? OrderCreateDate { get; set; }

        [Display(Name = "条码是否已经生成")]
        public int? BarCodeCreated { get; set; }

        [Display(Name = "条码生成日期")]
        [DataType(DataType.DateTime)]
        public DateTime? BarCodeCreateDate { get; set; }

        [Display(Name = "条码生成者")]
        public string BarCodeCreator { get; set; }

        [Display(Name = "完工率")]
        public float? CompletedRate { get; set; }

        public virtual List<Assemble> Assemble { get; set; }
        public virtual List<Users> Users { get; set; }
        public virtual List<BarCodes> BarCodes { get; set; }

    }

}