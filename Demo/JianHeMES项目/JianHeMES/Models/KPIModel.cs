﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    #region --- 7S区域数据表
    public class KPI_7S_DistrictPosition
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(100)]
        public string Group { get; set; }

        [Display(Name = "位置"), StringLength(200)]
        public string Position { get; set; }

        [Display(Name = "区域号")]
        public int District { get; set; }

        [Display(Name = "楼层")]
        public int Floor { get; set; }

        [Display(Name = "录入人"), StringLength(30)]
        public string InputPerson { get; set; }

        [Display(Name = "录入时间")]
        public DateTime InputTime { get; set; }

        [Display(Name = "修改人"), StringLength(30)]
        public string ModifyPerson { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? ModifyTime { get; set; }

        [Display(Name = "版本时间")]
        public DateTime? VersionsTime { get; set; }

        [Display(Name = "7S目标值")]
        public double TargetValue { get; set; }

        [Display(Name = "备注"), StringLength(600)]
        public string Remark { get; set; }

    }
    #endregion

    #region --- 7S记录表
    public class KPI_7S_Record
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(100)]
        public string Group { get; set; }

        [Display(Name = "位置"), StringLength(200)]
        public string Position { get; set; }

        [Display(Name = "区域号")]
        public int District { get; set; }

        [Display(Name = "日期")]
        public DateTime Date { get; set; }

        [Display(Name = "检查类型"), StringLength(50)]//日检、周检、随检
        public string Check_Type { get; set; }

        [Display(Name = "7S扣分类型")]//整理、整顿、清洁、清扫、安全、节约、素养
        public string PointsDeducted_Type { get; set; }

        [Display(Name = "7S扣分项"), StringLength(3000)]
        public string PointsDeducted_Item { get; set; }

        [Display(Name = "问题描述"), StringLength(1000)]
        public string ProblemDescription { get; set; }

        [Display(Name = "7S扣分")]
        public Decimal PointsDeducted { get; set; }

        [Display(Name = "责任人"), StringLength(30)]
        public string ResponsiblePerson { get; set; }

        [Display(Name = "录入人"), StringLength(30)]
        public string InputPerson { get; set; }

        [Display(Name = "检查人"), StringLength(100)]
        public string Check_Person { get; set; }

        [Display(Name = "录入时间")]
        public DateTime? InputTime { get; set; }

        [Display(Name = "限期整改时间")]
        public DateTime? RectificationTime { get; set; }

        [Display(Name = "整改结果描述"), StringLength(600)]

        public string RectificationResults { get; set; }

        [Display(Name = "整改结果")]
        public bool? Rectification_Confim { get; set; }

        [Display(Name = "整改结果判定人"), StringLength(30)]
        public string RectificationPerson { get; set; }

        [Display(Name = "判定结果时间")]
        public DateTime? ModifyTime { get; set; }

        [Display(Name = "限期未整改扣分")]
        public Decimal RectificationPoints { get; set; }

        [Display(Name = "重复出现问题扣分")]
        public Decimal RepetitionPointsDeducted { get; set; }

        [Display(Name = "第几周")]
        public int Week { get; set; }

        [Display(Name = "年份")]
        public int Year { get; set; }

        [Display(Name = "月份")]
        public int Month { get; set; }

        [Display(Name = "备注"), StringLength(600)]
        public string Remark { get; set; }
    }

    #endregion

    #region --- 7S扣分参照标准
    public class KPI_7S_ReferenceStandard
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "扣分类型"), StringLength(100)]
        public string PointsType { get; set; }

        [Display(Name = "扣分参考标准"), StringLength(2000)]
        public string ReferenceStandard { get; set; }

        [Display(Name = "录入人"), StringLength(30)]
        public string InputPerson { get; set; }

        [Display(Name = "录入时间")]
        public DateTime InputTime { get; set; }

        [Display(Name = "修改人"), StringLength(30)]
        public string ModifyPerson { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? ModifyTime { get; set; }

        [Display(Name = "备注"), StringLength(600)]
        public string Remark { get; set; }

    }
    #endregion

    #region --- 指标参数录入
    public class KPI_Indicators
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(100)]
        public string Group { get; set; }

        [Display(Name = "考核指标名"), StringLength(100)]
        public string IndicatorsName { get; set; }

        [Display(Name = "考核指标定义"), StringLength(100)]
        public string IndicatorsDefine { get; set; }

        [Display(Name = "计算公式"), StringLength(100)]
        public string ComputationalFormula { get; set; }

        [Display(Name = "权重")]
        public decimal Weight { get; set; }

        [Display(Name = "考核目标值")]
        public double IndicatorsValue { get; set; }

        [Display(Name = "考核目标值单位"), StringLength(20)]
        public string IndicatorsValueUnit { get; set; }

        [Display(Name = "数据名称"), StringLength(50)]
        public string DataName { get; set; }

        [Display(Name = "数据提供周期"), StringLength(20)]
        public string Cycle { get; set; }

        [Display(Name = "考核目标值来源部门"), StringLength(100)]
        public string SourceDepartment { get; set; }

        [Display(Name = "数据录入人员"), StringLength(100)]
        public string DataInputor { get; set; }

        [Display(Name = "录入时间"), StringLength(100)]
        public string DataInputTime { get; set; }

        [Display(Name = "考核目标值来源工段"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "考核目标值来源工序"), StringLength(50)]
        public string Process { get; set; }

        [Display(Name = "考核目标值来源记录时间段"), StringLength(50)]
        public string IndicatorsTimeSpan { get; set; }

        [Display(Name = "考核异常或者正常"), StringLength(50)]
        public string IndicatorsStatue { get; set; }

        [Display(Name = "考核类型"), StringLength(50)]
        public string IndicatorsType { get; set; }

        [Display(Name = "品质考核类型"), StringLength(50)]
        public string QualityStatue { get; set; }

        [Display(Name = "计划sql"), StringLength(500)]
        public string PlanSQL { get; set; }

        [Display(Name = "实际sql"), StringLength(500)]
        public string ActualSQL { get; set; }

        [Display(Name = "指标版本执行时间")]
        public DateTime? ExecutionTime { get; set; }
    }
    #endregion

    #region --- 品质效率实际记录表
    public class KPI_ActualRecord
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(100)]
        public string Group { get; set; }

        [Display(Name = "条码"), StringLength(100)]
        public string BarcodeNum { get; set; }

        [Display(Name = "考核类型"), StringLength(50)]  //品质/效率
        public string IndicatorsType { get; set; }

        [Display(Name = "实际工段"), StringLength(50)]
        public string Section { get; set; }

        [Display(Name = "实际工序"), StringLength(50)]
        public string Process { get; set; }

        [Display(Name = "记录时间")]
        public DateTime? ActualTime { get; set; }

        [Display(Name = "记录正常个数数量")]
        public int ActualNormalNum { get; set; }

        [Display(Name = "记录异常个数数量")]
        public int ActualAbnormalNum { get; set; }

        [Display(Name = "记录异常描述"), StringLength(600)]
        public string ActualAbnormalDescription { get; set; }

        [Display(Name = "记录是否正常")]
        public bool IsNormal { get; set; }

        [Display(Name = "记录创建时间")]
        public DateTime? ActualCreateTime { get; set; }

        [Display(Name = "记录创建人"), StringLength(50)]
        public string ActualCreateor { get; set; }

        [Display(Name = "记录完成时间")]
        public DateTime? ActualEndTime { get; set; }

        [Display(Name = "记录完成人"), StringLength(50)]
        public string ActualEndor { get; set; }
    }
    #endregion

    #region---班组流失率
    public class KPI_TurnoverRate
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "被考核部门"), StringLength(60)]
        public string Department { get; set; }//被考核部门

        [Display(Name = "班组名称"), StringLength(60)]
        public string Group { get; set; }//班组名称

        [Display(Name = "目标值")]
        public double IndicatorsValue { get; set; }//目标值

        [Display(Name = "月初人数")]
        public int BeginNumber { get; set; }//月初人数

        [Display(Name = "月末人数")]
        public int EndNumber { get; set; }//月末人数

        [Display(Name = "流失人数")]
        public int LossNumber { get; set; }//流失人数

        [Display(Name = "流失日期")]
        public DateTime DateLoss { get; set; }//流失日期

        [Display(Name = "确认人"), StringLength(60)]
        public string Createor { get; set; }//确认人

        [Display(Name = "确认时间")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "修改人"), StringLength(60)]
        public string ModifierName { get; set; }//修改人

        [Display(Name = "修改时间")]
        public DateTime? ModifierDate { get; set; }//修改时间

    }

    #endregion

    #region--- KPI显示总表
    public class KPI_TotalDisplay
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(100)]
        public string Group { get; set; }

        [Display(Name = "族群"), StringLength(50)]
        public string Ethnic_Group { get; set; }

        [Display(Name = "平均人数")]
        public decimal AvagePersonNum { get; set; }

        [Display(Name = "当月有无工伤事故")]
        public decimal InductrialAccident { get; set; }

        [Display(Name = "出勤率")]
        public decimal Attendance { get; set; }

        [Display(Name = "行政违规情况"), StringLength(200)]
        public string ViolationsMessage { get; set; }

        [Display(Name = "排名")]
        public int Ranking { get; set; }

        [Display(Name = "积分")]
        public int integral { get; set; }

        [Display(Name = "记录年月月份")]
        public DateTime? Time { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "创建人"), StringLength(50)]
        public string Createor { get; set; }
    }
    #endregion

    #region--班组评优月度指标汇总表
    public class KPI_MonthlyIndicators
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(100)]
        public string Group { get; set; }

        //效率指标
        [Display(Name = "效率指标"), StringLength(50)]
        public string Efficiency_Indicators { get; set; }

        [Display(Name = "指标名称(效率)"), StringLength(100)]
        public string Efficiency_IndexName { get; set; }

        [Display(Name = "目标值(效率)")]
        public double Efficiency_Target { get; set; }

        [Display(Name = "实际完成值(效率)")]
        public double Efficiency_Actual { get; set; }

        [Display(Name = "单项得分(效率)")]
        public double Efficiency_Single { get; set; }

        [Display(Name = "得分小计(效率)")]
        public double Efficiency_Score { get; set; }

        //品质指标
        [Display(Name = "品质指标"), StringLength(50)]
        public string Quality_Indicators { get; set; }

        [Display(Name = "指标名称(品质)"), StringLength(100)]
        public string Quality_IndexName { get; set; }

        [Display(Name = "目标值(品质)")]
        public double Quality_Target { get; set; }

        [Display(Name = "实际完成值(品质)")]
        public double Quality_Actual { get; set; }

        [Display(Name = "单项得分(品质)")]
        public double Quality_Single { get; set; }

        [Display(Name = "得分小计(品质)")]
        public double Quality_Score { get; set; }

        //月度流失率指标
        [Display(Name = "月度流失率指标"), StringLength(50)]
        public string Turnover_Indicators { get; set; }

        [Display(Name = "指标名称(流失率)"), StringLength(100)]
        public string Turnover_IndexName { get; set; }

        [Display(Name = "目标值(流失率)")]
        public double Turnover_Target { get; set; }

        [Display(Name = "实际完成值(流失率)")]
        public double Turnover_Actual { get; set; }

        [Display(Name = "单项得分(流失率)")]
        public double Turnover_Single { get; set; }

        [Display(Name = "得分小计(流失率)")]
        public double Turnover_Score { get; set; }

        //7S评比指标
        [Display(Name = "7S评比指标"), StringLength(50)]
        public string Comparison_Indicators { get; set; }

        [Display(Name = "指标名称(7S评比)"), StringLength(100)]
        public string Comparison_IndexName { get; set; }

        [Display(Name = "目标值(7S评比)")]
        public double Comparison_Target { get; set; }

        [Display(Name = "实际完成值(7S评比)")]
        public double Comparison_Actual { get; set; }

        [Display(Name = "单项得分(7S评比)")]
        public double Comparison_Single { get; set; }

        [Display(Name = "得分小计(7S评比)")]
        public double Comparison_Score { get; set; }


        [Display(Name = "合计总分）")]
        public decimal TotalScore { get; set; }

        [Display(Name = "平均人数（不少于5人）")]
        public decimal AvagePersonNum { get; set; }

        [Display(Name = "当月有无工伤事故")]
        public decimal InductrialAccident { get; set; }

        [Display(Name = "出勤率")]
        public decimal Attendance { get; set; }

        [Display(Name = "行政违规情况"), StringLength(200)]
        public string ViolationsMessage { get; set; }

        [Display(Name = "排名")]
        public int Ranking { get; set; }

        [Display(Name = "积分")]
        public int integral { get; set; }

        [Display(Name = "年份")]
        public int Year { get; set; }

        [Display(Name = "月份")]
        public int Month { get; set; }


    }

    //各部门负责人审核班组评优月度指标汇总
    public class KPI_ReviewSummary
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "年份")]
        public int Year { get; set; }

        [Display(Name = "月份")]
        public int Month { get; set; }

        [Display(Name = "记录审核类型"), StringLength(50)]
        public string Type { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "负责人审核"), StringLength(50)]
        public string HRAudit { get; set; }

        [Display(Name = "审核时间")]
        public DateTime? HRAuditDate { get; set; }

        [Display(Name = "是否通过")]
        public bool HRjudge { get; set; }

    }


    #endregion
}