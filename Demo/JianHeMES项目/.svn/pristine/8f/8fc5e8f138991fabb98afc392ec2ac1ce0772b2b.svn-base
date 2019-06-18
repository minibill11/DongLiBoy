using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class Burn_in
    {
        //老化电检部分
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号")]
        public string OrderNum { get; set; }

        [Display(Name = "老化架号"), StringLength(20)]
        public string BurnInShelfNum { get; set; }

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

        [Display(Name = "OQC时长(天)")]
        public int OQCCheckDate { get; set; }

        [Display(Name = "OQC时长"),DataType(DataType.DateTime)]
        public TimeSpan? OQCCheckTime { get; set; }

        [Display(Name = "老化用时")]
        public string OQCCheckTimeSpan { get; set; }

        [Display(Name = "OQC异常")]
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        public string Burn_in_OQCCheckAbnormal { get; set; }

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
    
    /// <summary>
    /// 老化拼屏记录 
    /// </summary>
    public class Burn_in_MosaicScreen
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "条码号"), StringLength(50)]
        public string BarCodesNum { get; set; }

        [Display(Name = "老化架号"), StringLength(20)]
        public string BurnInShelfNum { get; set; }

        [Display(Name = "OQC检验员工号"), StringLength(20)]
        public string OQCPrincipalNum { get; set; }

        [Display(Name = "老化拼屏开始时间")]
        public DateTime? OQCMosaicStartTime { get; set; }

        [Display(Name = "老化拼屏结束时间")]
        public DateTime? OQCMosaicEndTime { get; set; }

        [Display(Name = "备注"), StringLength(200)]
        public string Remark { get; set; }

    }

    public class Burn_in_OQCCheckAbnormal
    {
        [Key]
        public int Id { get; set; }
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        [Display(Name = "Burn_in_OQC异常代码")]
        public int Code { get; set; }
        //[Required(AllowEmptyStrings = true, ErrorMessage = "能为空")]
        [Display(Name = "描述")]
        public string DetialedDescription { get; set; }

    }
}