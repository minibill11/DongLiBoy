﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class Common
    {
        enum Weekly : int
        {
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7
        }



    }

    public class VersionInfo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Mes版本号"), StringLength(100)]
        public string MESVersion { get; set; }

        [Display(Name = "模块"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "更新内容"), StringLength(1000)]
        public string UpdateMes { get; set; }

        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        [Display(Name = "SVN版本号"), StringLength(100)]
        public string SNVVersion { get; set; }
    }
    public class Version_SectionList
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "模块"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "创建人"), StringLength(50)]
        public string Createor { get; set; }
    }
}