using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class CourtScreenModuleInfo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "模组号"),StringLength(50)]
        public string ModuleNum { get; set; }

        [Display(Name ="类型"),StringLength(50)]
        public string Type { get; set; }

        [Display(Name ="条码"),StringLength(50)]
        public string BarCode { get; set; }

        [Display(Name ="状态")]
        public int Status { get; set; }

        [Display(Name ="备注"),StringLength(200)]
        public string Remark { get; set; }

        [Display(Name ="录入人"),StringLength (50)]
        public string Creater { get; set; }

        [Display(Name ="录入时间")]
        public DateTime? CreateDate { get; set; }
    }
}