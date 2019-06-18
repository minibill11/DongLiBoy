using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class BarCodes
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "订单号")]
        public string OrderNum { get; set; }

        [Display(Name = "新订单号")]
        public string ToOrderNum { get; set; }

        [Required]
        [Display(Name = "条码前缀")]
        public string BarCode_Prefix { get; set; }

        [Required]
        [Display(Name = "条码")]
        public string BarCodesNum { get; set; }

        [Display(Name = "模组箱体号")]
        public string ModuleGroupNum { get; set; }

        [Required]
        [Display(Name = "类型")]
        public string BarCodeType { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "创建人")]
        public string Creator { get; set; }

        [Display(Name = "是否为库存")]
        public Boolean IsRepertory { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }


        public virtual List<OrderMgm> OrderMgm { get; set; }
        public virtual List<Assemble> Assemble { get; set; }
        public virtual List<Users> Users { get; set; }

    }

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

        [Display(Name = "工序"),StringLength(20)]
        public string Procedure { get; set; }

        [Display(Name = "操作人工号")]
        public int UsserID { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreateDate { get; set; }

    }
}