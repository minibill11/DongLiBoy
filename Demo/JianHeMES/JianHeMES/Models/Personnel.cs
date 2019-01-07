using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{        
    //人事日报表
    public class Personnel_daily
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门")]
        public string Department { get; set; }

        [Display(Name = "负责人")]
        public string Principal { get; set; }

        [Display(Name = "编制人数")]
        public int Aurhorized_personnel  { get; set; }

        [Display(Name = "刚需人数")]
        public int Need_personnel { get; set; }

        //缺口人数＝刚需人数－当日正式工人数-当日劳务工人数-当日入职正式工人数-当日入职劳务工人数

        [Display(Name = "当日正式工人数")]
        public int Employees_personnel { get; set; }

        [Display(Name = "当日劳务工人数")]
        public int Workers_personnel { get; set; }

        //当日人数＝当日正式工人数-当日劳务工人数

        [Display(Name = "当日入职正式工人数")]
        public int Today_on_board_employees { get; set; }

        [Display(Name = "当日入职劳务工人数")]
        public int Today_on_board_workers { get; set; }

        //当日入职人数＝当日入职正式工人数-当日入职劳务工人数

        [Display(Name = "预填表人数")]
        public int Interview { get; set; }

        [Display(Name = "当天离职正式工")]
        public int Todoy_dimission_employees { get; set; }

        [Display(Name = "当天离职劳务工")]
        public int Todoy_dimission_workers { get; set; }

        //当日离职人数＝当日离职正式工人数-当日离职劳务工人数

        [Display(Name = "当月待离职正式工人数")]
        public int Resigned_that_month { get; set; }

        [Display(Name = "当月待离职劳务工人数")]
        public int Resigned_workers_that_month { get; set; }

        [Display(Name = "日期")]
        public DateTime? Date { get; set; }

        [Display(Name = "日报提交人")]
        public string Reporter { get; set; }

    }

    //public class Personnel_daily_header
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    [Display(Name = "排序序号")]
    //    public int orderBy { get; set; }

    //    [Display(Name = "部门")]
    //    public string Department { get; set; }

    //    [Display(Name = "负责人")]
    //    public string Principal { get; set; }

    //    [Display(Name = "编制人数")]
    //    public int Aurhorized_personnel { get; set; }

    //    [Display(Name = "刚需人数")]
    //    public int Need_personnel { get; set; }

    //    [Display(Name = "日期")]
    //    public DateTime? Date { get; set; }

    //}

}