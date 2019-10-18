using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class SOPoperating   //查看共享目录文件
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "工艺流程图编号"), StringLength(50)]
        public string Processflow_chart { get; set; }

        [Display(Name = "工段名"), StringLength(50)]
        public string SectionName { get; set; }

        [Display(Name = "SOP参照文件"), StringLength(200)]
        public string SOPreference_document { get; set; }

        [Display(Name = "备注"), StringLength(1000)]
        public string Remark { get; set; }

    }
    
}