﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    #region --- 订单总计划表
    public class Plan_FromOrder
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNumber { get; set; }

        [Display(Name = "计划工段"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "计划工序"), StringLength(50)]
        public string Process { get; set; }

        [Display(Name = "计划开始时间")]
        public DateTime? PlanStratTime { get; set; }

        [Display(Name = "计划结束时间")]
        public DateTime? PlanEndTime { get; set; }

        [Display(Name = "计划记录创建时间")]
        public DateTime? PlanCreateTime { get; set; }

        [Display(Name = "计划记录创建人"), StringLength(50)]
        public string PlanCreateor { get; set; }
    }

    #endregion

    #region --- 品质效率计划表
    public class Plan_FromKPI
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(100)]
        public string Group { get; set; }

        [Display(Name = "考核类型"), StringLength(50)]
        public string IndicatorsType { get; set; }

        [Display(Name = "检查类型"), StringLength(50)]
        public string CheckType { get; set; }

        [Display(Name = "检查部门"), StringLength(50)]
        public string CheckDepartment { get; set; }

        [Display(Name = "检查班组"), StringLength(50)]
        public string CheckGroup { get; set; }

        [Display(Name = "计划工段"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "计划工序"), StringLength(50)]
        public string Process { get; set; }

        [Display(Name = "计划时间")]
        public DateTime? PlanTime { get; set; }

        [Display(Name = "计划数量")]
        public int  PlanNum { get; set; }

        [Display(Name = "计划记录创建时间")]
        public DateTime? PlanCreateTime { get; set; }

        [Display(Name = "计划记录创建人"), StringLength(50)]
        public string  PlanCreateor { get; set; }
    }
    #endregion

    #region--- 工序工段编辑表
    public class Plan_SectionParameter
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "计划工序"), StringLength(50)]
        public string Process { get; set; }

        [Display(Name = "计划工段"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "计划工段对应表名"), StringLength(50)]
        public string Table { get; set; }

        [Display(Name = "记录创建时间")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "记录创建人"), StringLength(50)]
        public string Createor { get; set; }
    }
    #endregion

}