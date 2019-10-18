﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class Test 
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"),StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "工作内容"),StringLength(50)]
        public string JobContent { get; set; }

        [Display(Name = "平台类型"), StringLength(50)]
        public string PlatformType { get; set; }

        [Display(Name = "产线号")]
        public int LineNum { get; set; }
    }

    public class TU
    {
        [Key]
        public int Id { get; set; }   
        [Display(Name ="号码")]
        public int No { get; set; }
        [Display(Name ="姓名"),StringLength(20)]
        public string Name { get; set; }
        [Display(Name ="年龄")]
        public int Age { get; set; }
        [Display(Name ="性别"),StringLength(6)]
        public string Sex { get; set; }
    }

}