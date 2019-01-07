using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class FinalQC
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号")]
        public string OrderNum { get; set; }

        [Display(Name = "条码号")]
        public string BarCodesNum { get; set; }

        [Display(Name = "开始时间")]
        public DateTime? FQCCheckBT { get;set; }

        [Display(Name = "FQC检验员")]
        public string FQCPrincipal { get;set; }

        [Display(Name = "完成时间")]
        public DateTime? FQCCheckFT { get; set; }

        [Display(Name = "FQC时长(天)")]
        public int FQCCheckDate { get;set; }

        [Display(Name = "FQC时长")]
        public TimeSpan? FQCCheckTime { get; set; }

        [Display(Name = "FQC检验用时")]
        public string FQCCheckTimeSpan { get;set; }

        [Display(Name = "FQC异常")]
        public string FinalQC_FQCCheckAbnormal { get; set; }

        [Display(Name = "是否重复")]
        public bool RepetitionFQCCheck { get; set; }

        [Display(Name = "重复原因")]
        public string RepetitionFQCCheckCause { get; set; }

        [Display(Name = "FQC是否完成")]
        public Boolean FQCCheckFinish { get;set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }


    }
}