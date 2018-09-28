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

        public virtual List<OrderMgm> OrderMgm { get; set; }
        public virtual List<Assemble> Assemble { get; set; }
        public virtual List<Users> Users { get; set; }

    }
}