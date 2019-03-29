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

        [Display(Name = "当天离职正式工_入职满七天")]
        public int Todoy_dimission_employees_over7days { get; set; }

        [Display(Name = "当天离职正式工_入职未满七天")]
        public int Todoy_dimission_employees_nvever_over7days { get; set; }


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

    public class Personnel_of_Contrast
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门")]
        public string Department { get; set; }

        [Display(Name = " 人数")]
        public int Number { get; set; }

        [Display(Name = "正班工时")]
        public decimal Zhang_WorkingHour { get; set; }

        [Display(Name = "正班工资")]
        public decimal Zhang_Pay { get; set; }

        [Display(Name = "人员加班总值")]
        public decimal Total_Staff { get; set; }

        [Display(Name = "加班工资合计")]
        public decimal Overtime_Total { get; set; }

        [Display(Name = "工资合计")]
        public decimal Pay_Total { get; set; }

        [Display(Name = " 日人均工资")]
        public decimal Daily_Pay { get; set; }

        [Display(Name = "日期")]
        public DateTime? Date { get; set; }

        [Display(Name = "记录创建时间")]
        public DateTime? CreateDate { get; set; }
        
        [Display(Name = "记录人")]
        public string Creator { get; set; }

        [Display(Name = "年份")]
        public int Year { get; set; }

        [Display(Name = "月份")]
        public int Month { get; set; }

        [Display(Name = "周")]
        public int Week { get; set; }

        //第几周
        [Display(Name = " 周数")]
        public int Week_number { get; set; }

        //星期一
        [Display(Name = "星期一")]
        public DateTime? Monday { get; set; }

        //星期日
        [Display(Name = "星期日")]
        public DateTime? Sunday { get; set; }

    }

    public class Personnel_Turnoverrate
    {

        [Key]
        public int Id { get; set; }

        [Display(Name = "部门")]
        public string Department { get; set; }

        [Display(Name = "月初人数")]
        public int Month_Beginning { get; set; }

        [Display(Name = "月末人数")]
        public int Month_End { get; set; }

        [Display(Name = "平均人数")]
        public int Average_Number { get; set; }

        [Display(Name = "离职人数")]
        public int Departure { get; set; }

        [Display(Name = "流失率")]
        public decimal Turnover_Rate { get; set; }

        [Display(Name = "平均数值")]
        public decimal Average_Value { get; set; }

        [Display(Name = "年份")]
        public int Year { get; set; }

        [Display(Name = "月份")]
        public int Month { get; set; }

        [Display(Name = "记录日期")]
        public DateTime? Date { get; set; }

        [Display(Name = "记录创建时间")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "记录人")]
        public string Creator { get; set; }

    }

    public class Personnel_Recruitment
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门")]
        public string Department_weekly { get; set; }

        [Display(Name = "需求岗位")]
        public string Demand_jobs { get; set; }

        [Display(Name = "编制")]
        public int Compile { get; set; }

        [Display(Name = "需求人数")]
        public int Demand_number { get; set; }

        [Display(Name = "录入人数")]
        public int Employed { get; set; }

        [Display(Name = "需求未完成人数")]
        public int Unfinished_nember { get; set; }

        [Display(Name = "申请日期")]
        public DateTime? Application_date { get; set; }

        [Display(Name = "到岗日期")]
        public DateTime? Work_date { get; set; }

        [Display(Name = "招聘计划周期")]
        public int Invite_Plan_Cycle { get; set; }

        [Display(Name = "招聘实际周期")]
        public int Invite_Actaul_Cycle { get; set; }

        [Display(Name = "日期")]
        public DateTime? Date { get; set; }

        [Display(Name = "记录创建时间")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "记录人")]
        public string Creator { get; set; }

        [Display(Name = "年份")]
        public int Year { get; set; }

        [Display(Name = "月份")]
        public int Month { get; set; }

        [Display(Name = "周")]
        public int Week { get; set; }

        //第几周
        [Display(Name = " 周数")]
        public int Week_number { get; set; }

        //星期一
        [Display(Name = "星期一")]
        public DateTime? Monday { get; set; }

        //星期日
        [Display(Name = "星期日")]
        public DateTime? Sunday { get; set; }

    }


}