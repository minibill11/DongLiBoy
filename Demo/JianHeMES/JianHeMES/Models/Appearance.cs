using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class Appearance
    { //外观电检部分
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号")]
        public string OrderNum { get; set; }

        [Display(Name = "条码号")]
        public string BarCodesNum { get; set; }

        [Display(Name = "开始时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? OQCCheckBT { get; set; }

        [Display(Name = "OQC检验员")]
        public string OQCPrincipal { get; set; }

        [Display(Name = "完成时间")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public DateTime? OQCCheckFT { get; set; }

        [Display(Name = "OQC时长")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public TimeSpan? OQCCheckTime { get; set; }

        [Display(Name = "外观包装用时")]
        public string OQCCheckTimeSpan { get; set; }

        [Display(Name = "OQC异常")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public string Appearance_OQCCheckAbnormal { get; set; }

        [Display(Name = "维修情况")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public string RepairCondition { get; set; }

        [Display(Name = "OQC是否完成")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public Boolean OQCCheckFinish { get; set; }

        public virtual List<OrderMgm> OrderMgm { get; set; }
        public virtual List<Users> Users { get; set; }
        public virtual List<BarCodes> BarCodes { get; set; }
        public virtual List<Assemble> Assembles { get; set; }
        public virtual List<PQCCheckabnormal> PQCCheckabnormal { get; set; }

    }

    public class Appearance_OQCCheckAbnormal
    {
        [Key]
        public int Id { get; set; }
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        [Display(Name = "Appearance_OQC异常代码")]
        public int Code { get; set; }
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        [Display(Name = "描述")]
        public string DetialedDescription { get; set; }

    }
}
