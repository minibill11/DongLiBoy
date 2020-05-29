﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{

    //设备基本信息表
    public class EquipmentBasicInfo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        //序号 
        [Display(Name = "原序号"), StringLength(50)]
        public string SerialNumber { get; set; }

        //设备编号 
        [Display(Name = "设备编号"), StringLength(50)]
        public string EquipmentNumber { get; set; }

        //资产编号 
        [Display(Name = "资产编号"), StringLength(50)]
        public string AssetNumber { get; set; }

        //设备名称 
        [Display(Name = "设备名称"), StringLength(50)]
        public string EquipmentName { get; set; }

        //品牌
        [Display(Name = "品牌"), StringLength(50)]
        public string Brand { get; set; }

        //型号/规格 
        [Display(Name = "型号/规格"), StringLength(50)]
        public string ModelSpecification { get; set; }

        //设备铭牌 
        [Display(Name = "设备铭牌"), StringLength(1000)]
        public string InfoPlate { get; set; }

        //供应商
        [Display(Name = "供应商"), StringLength(50)]
        public string Supplier { get; set; }

        //购入日期
        [Display(Name = "购入日期")]
        public DateTime? PurchaseDate { get; set; }

        //出厂编号 
        [Display(Name = "出厂编号"), StringLength(50)]
        public string ManufacturingNumber { get; set; }

        //数量 
        [Display(Name = "数量")]
        public int Quantity { get; set; }

        //启用时间 
        [Display(Name = "启用时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ActionDate { get; set; }

        //折旧年限 
        [Display(Name = "折旧年限"), StringLength(50)]
        public string DepreciableLife { get; set; }

        //使用部门 
        [Display(Name = "使用部门"), StringLength(50)]
        public string UserDepartment { get; set; }

        //存放地点 
        [Display(Name = "存放地点"), StringLength(50)]
        public string StoragePlace { get; set; }

        //车间 
        [Display(Name = "车间"), StringLength(50)]
        public string WorkShop { get; set; }

        //产线号
        [Display(Name = "产线号"), StringLength(50)]
        public string LineNum { get; set; }

        //工段 
        [Display(Name = "工段"), StringLength(50)]
        public string Section { get; set; }

        //功能描述
        [Display(Name = "功能描述"), StringLength(500)]
        public string FunctionDiscription { get; set; }

        //设备状态 
        [Display(Name = "设备状态"), StringLength(50)]
        public string Status { get; set; }

        //记录创建人 
        [Display(Name = "记录创建人"), StringLength(50)]
        public string Creator { get; set; }

        //创建时间 
        [Display(Name = "创建时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CreateTime { get; set; }

        //记录修改人 
        [Display(Name = "记录修改人"), StringLength(50)]
        public string Modifier { get; set; }

        //创建时间 
        [Display(Name = "创建时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }

        [Display(Name = "报废人"), StringLength(50)]
        public string DiscardName { get; set; }

        [Display(Name = "报废时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DiscardTime { get; set; }

        //备注 
        [Display(Name = "备注"), StringLength(1000)]
        public string Remark { get; set; }

    }

    //设备维修履历汇总记录表
    public class EquipmentStatusRecord
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        //设备编号 
        [Display(Name = "设备编号"), StringLength(50)]
        public string EquipmentNumber { get; set; }

        //资产编号 
        [Display(Name = "资产编号"), StringLength(50)]
        public string AssetNumber { get; set; }

        //设备名称 
        [Display(Name = "设备名称"), StringLength(50)]
        public string EquipmentName { get; set; }

        //订单号
        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        //设备状态 
        [Display(Name = "设备状态"), StringLength(50)]
        public string Status { get; set; }

        //状态开始时间 
        [Display(Name = "状态开始时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? StatusStarTime { get; set; }

        //状态结束时间 
        [Display(Name = "状态结束时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? StatusEndTime { get; set; }

        [Display(Name = "订单"), StringLength(50)]
        public string OrderNumber { get; set; }

        //报修人 
        [Display(Name = "报修人"), StringLength(50)]
        public string ReportRepairMan { get; set; }

        //故障现象描述 
        [Display(Name = "故障现象描述"), StringLength(1000)]
        public string FailureDescription { get; set; }

        //原因 
        [Display(Name = "原因分析"), StringLength(1000)]
        public string Reason { get; set; }

        //维修或检测内容 
        [Display(Name = "维修或检测内容"), StringLength(1000)]
        public string RepairOrTestContent { get; set; }

        //维修接单时间
        [Display(Name = "维修接单时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? GetJobTime { get; set; }

        //计划完成时间
        [Display(Name = "计划完成时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? PlanFinishTime { get; set; }

        //实际完成时间
        [Display(Name = "实际完成时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ActualFinishTime { get; set; }

        //维修负责人 
        [Display(Name = "维修人"), StringLength(50)]
        public string RepairMan { get; set; }

        //配件信息 
        [Display(Name = "配件信息"), StringLength(1000)]
        public string SparePartsInfo { get; set; }

        //使用部门 
        [Display(Name = "使用部门"), StringLength(50)]
        public string UserDepartment { get; set; }

        //车间 
        [Display(Name = "车间"), StringLength(50)]
        public string WorkShop { get; set; }

        //产线号(名)
        [Display(Name = "产线号(名)"), StringLength(50)]
        public string LineNum { get; set; }

        //工段 
        [Display(Name = "工段"), StringLength(50)]
        public string Section { get; set; }

        //记录创建人 
        [Display(Name = "记录创建人"), StringLength(50)]
        public string Creator { get; set; }

        //创建时间 
        [Display(Name = "创建时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CreateTime { get; set; }

        //记录修改人 
        [Display(Name = "记录修改人"), StringLength(50)]
        public string Modifier { get; set; }

        //创建时间 
        [Display(Name = "创建时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }

        //备注 
        [Display(Name = "备注"), StringLength(1000)]
        public string Remark { get; set; }

    }

    //设备位置状态记录表
    public class EquipmentSetStation
    {
        [Key]

        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        //设备编号 
        [Display(Name = "设备编号"), StringLength(50)]
        public string EquipmentNumber { get; set; }

        //资产编号 
        [Display(Name = "资产编号"), StringLength(50)]
        public string AssetNumber { get; set; }

        //设备名称 
        [Display(Name = "设备名称"), StringLength(50)]
        public string EquipmentName { get; set; }

        //设备状态 
        [Display(Name = "设备状态"), StringLength(50)]
        public string Status { get; set; }

        //使用部门 
        [Display(Name = "使用部门"), StringLength(50)]
        public string UserDepartment { get; set; }

        //车间 
        [Display(Name = "车间"), StringLength(50)]
        public string WorkShop { get; set; }

        //产线号(名)
        [Display(Name = "产线号(名)"), StringLength(50)]
        public string LineNum { get; set; }

        //工段 
        [Display(Name = "工段"), StringLength(50)]
        public string Section { get; set; }

        //位置序号
        [Display(Name = "位置序号")]
        public int StationNum { get; set; }

        //记录创建人 
        [Display(Name = "记录创建人"), StringLength(50)]
        public string Creator { get; set; }

        //创建时间 
        [Display(Name = "创建时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CreateTime { get; set; }

        //记录修改人 
        [Display(Name = "记录修改人"), StringLength(50)]
        public string Modifier { get; set; }

        //创建时间 
        [Display(Name = "修改时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }

        //备注 
        [Display(Name = "备注"), StringLength(1000)]
        public string Remark { get; set; }

    }

    //修改设备状态记录表
    public class EquipmentStateTime
    {
        [Key]

        public int Id { get; set; }

        //设备编号 
        [Display(Name = "设备编号"), StringLength(50)]
        public string EquipmentNumber { get; set; }

        //资产编号 
        [Display(Name = "资产编号"), StringLength(50)]
        public string AssetNumber { get; set; }

        //设备名称 
        [Display(Name = "设备名称"), StringLength(50)]
        public string EquipmentName { get; set; }

        //使用部门 
        [Display(Name = "使用部门"), StringLength(50)]
        public string UserDepartment { get; set; }

        //产线号(名)
        [Display(Name = "产线号(名)"), StringLength(50)]
        public string LineNum { get; set; }

        //设备状态 
        [Display(Name = "设备状态"), StringLength(50)]
        public string Status { get; set; }

        //状态开始时间 
        [Display(Name = "状态开始时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? StatusStarTime { get; set; }

        //状态结束时间 
        [Display(Name = "状态结束时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? StatusEndTime { get; set; }

        //记录创建人 
        [Display(Name = "记录创建人"), StringLength(50)]
        public string Creator { get; set; }

        //创建时间 
        [Display(Name = "创建时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CreateTime { get; set; }
    }

    //设备点检保养记录表
    public class Equipment_Tally_maintenance
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "设备使用部门"), StringLength(50)]
        public string UserDepartment { get; set; }

        //设备名称 
        [Display(Name = "设备名称"), StringLength(50)]
        public string EquipmentName { get; set; }

        //设备编号 
        [Display(Name = "设备编号"), StringLength(50)]
        public string EquipmentNumber { get; set; }

        //产线号
        [Display(Name = "产线号"), StringLength(50)]
        public string LineName { get; set; }

        [Display(Name = "年")]
        public int Year { get; set; }

        [Display(Name = "月")]
        public int Month { get; set; }

        //A组日保养
        [Display(Name = "A日保养项目"), StringLength(200)]
        public string Day_project1 { get; set; }

        [Display(Name = "A操作方法（日保养）"), StringLength(600)]
        public string Day_opera1 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_1 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_2 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_3 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_4 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_5 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_6 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_7 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_8 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_9 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_10 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_11 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_12 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_13 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_14 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_15 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_16 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_17 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_18 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_19 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_20 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_21 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_22 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_23 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_24 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_25 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_26 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_27 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_28 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_29 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_30 { get; set; }

        [Display(Name = "A保养日期（天）")]
        public int Day_A_31 { get; set; }

        //B组日保养
        [Display(Name = "2日保养项目"), StringLength(200)]
        public string Day_project2 { get; set; }

        [Display(Name = "2操作方法（日保养）"), StringLength(600)]
        public string Day_opera2 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_1 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_2 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_3 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_4 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_5 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_6 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_7 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_8 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_9 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_10 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_11 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_12 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_13 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_14 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_15 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_16 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_17 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_18 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_19 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_20 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_21 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_22 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_23 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_24 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_25 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_26 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_27 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_28 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_29 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_30 { get; set; }

        [Display(Name = "B保养日期（天）")]
        public int Day_B_31 { get; set; }

        //C组日保养
        [Display(Name = "3日保养项目"), StringLength(200)]
        public string Day_project3 { get; set; }

        [Display(Name = "3操作方法（日保养）"), StringLength(600)]
        public string Day_opera3 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_1 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_2 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_3 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_4 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_5 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_6 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_7 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_8 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_9 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_10 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_11 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_12 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_13 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_14 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_15 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_16 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_17 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_18 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_19 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_20 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_21 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_22 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_23 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_24 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_25 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_26 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_27 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_28 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_29 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_30 { get; set; }

        [Display(Name = "C保养日期（天）")]
        public int Day_C_31 { get; set; }

        //D组日保养
        [Display(Name = "4日保养项目"), StringLength(200)]
        public string Day_project4 { get; set; }

        [Display(Name = "4操作方法（日保养）"), StringLength(600)]
        public string Day_opera4 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_1 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_2 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_3 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_4 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_5 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_6 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_7 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_8 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_9 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_10 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_11 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_12 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_13 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_14 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_15 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_16 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_17 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_18 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_19 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_20 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_21 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_22 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_23 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_24 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_25 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_26 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_27 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_28 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_29 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_30 { get; set; }

        [Display(Name = "D保养日期（天）")]
        public int Day_D_31 { get; set; }

        //E组日保养
        [Display(Name = "5日保养项目"), StringLength(200)]
        public string Day_project5 { get; set; }

        [Display(Name = "5操作方法（日保养）"), StringLength(600)]
        public string Day_opera5 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_1 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_2 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_3 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_4 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_5 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_6 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_7 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_8 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_9 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_10 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_11 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_12 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_13 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_14 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_15 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_16 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_17 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_18 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_19 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_20 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_21 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_22 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_23 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_24 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_25 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_26 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_27 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_28 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_29 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_30 { get; set; }

        [Display(Name = "E保养日期（天）")]
        public int Day_E_31 { get; set; }

        //F组日保养
        [Display(Name = "6日保养项目"), StringLength(200)]
        public string Day_project6 { get; set; }

        [Display(Name = "6操作方法（日保养）"), StringLength(600)]
        public string Day_opera6 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_1 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_2 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_3 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_4 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_5 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_6 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_7 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_8 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_9 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_10 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_11 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_12 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_13 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_14 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_15 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_16 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_17 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_18 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_19 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_20 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_21 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_22 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_23 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_24 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_25 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_26 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_27 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_28 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_29 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_30 { get; set; }

        [Display(Name = "F保养日期（天）")]
        public int Day_F_31 { get; set; }


        //G组日保养
        [Display(Name = "7日保养项目"), StringLength(200)]
        public string Day_project7 { get; set; }

        [Display(Name = "7操作方法（日保养）"), StringLength(600)]
        public string Day_opera7 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_1 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_2 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_3 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_4 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_5 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_6 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_7 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_8 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_9 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_10 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_11 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_12 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_13 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_14 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_15 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_16 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_17 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_18 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_19 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_20 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_21 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_22 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_23 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_24 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_25 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_26 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_27 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_28 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_29 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_30 { get; set; }

        [Display(Name = "G保养日期（天）")]
        public int Day_G_31 { get; set; }

        //H组日保养
        [Display(Name = "8日保养项目"), StringLength(200)]
        public string Day_project8 { get; set; }

        [Display(Name = "8操作方法（日保养）"), StringLength(600)]
        public string Day_opera8 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_1 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_2 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_3 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_4 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_5 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_6 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_7 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_8 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_9 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_10 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_11 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_12 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_13 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_14 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_15 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_16 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_17 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_18 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_19 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_20 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_21 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_22 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_23 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_24 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_25 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_26 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_27 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_28 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_29 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_30 { get; set; }

        [Display(Name = "H保养日期（天）")]
        public int Day_H_31 { get; set; }

        //I组日保养
        [Display(Name = "9日保养项目"), StringLength(200)]
        public string Day_project9 { get; set; }

        [Display(Name = "9操作方法（日保养）"), StringLength(600)]
        public string Day_opera9 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_1 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_2 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_3 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_4 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_5 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_6 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_7 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_8 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_9 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_10 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_11 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_12 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_13 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_14 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_15 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_16 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_17 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_18 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_19 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_20 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_21 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_22 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_23 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_24 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_25 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_26 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_27 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_28 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_29 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_30 { get; set; }

        [Display(Name = "I保养日期（天）")]
        public int Day_I_31 { get; set; }

        //J组日保养
        [Display(Name = "10日保养项目"), StringLength(200)]
        public string Day_project10 { get; set; }

        [Display(Name = "10操作方法（日保养）"), StringLength(600)]
        public string Day_opera10 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_1 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_2 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_3 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_4 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_5 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_6 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_7 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_8 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_9 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_10 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_11 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_12 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_13 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_14 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_15 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_16 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_17 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_18 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_19 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_20 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_21 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_22 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_23 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_24 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_25 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_26 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_27 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_28 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_29 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_30 { get; set; }

        [Display(Name = "J保养日期（天）")]
        public int Day_J_31 { get; set; }

        //K组日保养
        [Display(Name = "11日保养项目"), StringLength(200)]
        public string Day_project11 { get; set; }

        [Display(Name = "11操作方法（日保养）"), StringLength(600)]
        public string Day_opera11 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_1 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_2 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_3 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_4 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_5 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_6 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_7 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_8 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_9 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_10 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_11 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_12 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_13 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_14 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_15 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_16 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_17 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_18 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_19 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_20 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_21 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_22 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_23 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_24 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_25 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_26 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_27 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_28 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_29 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_30 { get; set; }

        [Display(Name = "K保养日期（天）")]
        public int Day_K_31 { get; set; }

        //保养人和保养时间
        [Display(Name = "1保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_1 { get; set; }

        [Display(Name = "1保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_1 { get; set; }

        [Display(Name = "2保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_2 { get; set; }

        [Display(Name = "2保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_2 { get; set; }

        [Display(Name = "3保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_3 { get; set; }

        [Display(Name = "3保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_3 { get; set; }

        [Display(Name = "4保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_4 { get; set; }

        [Display(Name = "4保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_4 { get; set; }

        [Display(Name = "5保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_5 { get; set; }

        [Display(Name = "5保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_5 { get; set; }

        [Display(Name = "6保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_6 { get; set; }

        [Display(Name = "6保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_6 { get; set; }

        [Display(Name = "7保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_7 { get; set; }

        [Display(Name = "7保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_7 { get; set; }

        [Display(Name = "8保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_8 { get; set; }

        [Display(Name = "8保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_8 { get; set; }

        [Display(Name = "9保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_9 { get; set; }

        [Display(Name = "9保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_9 { get; set; }

        [Display(Name = "10保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_10 { get; set; }

        [Display(Name = "10保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_10 { get; set; }

        [Display(Name = "11保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_11 { get; set; }

        [Display(Name = "11保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_11 { get; set; }

        [Display(Name = "12保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_12 { get; set; }

        [Display(Name = "12保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_12 { get; set; }

        [Display(Name = "13保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_13 { get; set; }

        [Display(Name = "13保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_13 { get; set; }

        [Display(Name = "14保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_14 { get; set; }

        [Display(Name = "14保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_14 { get; set; }

        [Display(Name = "15保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_15 { get; set; }

        [Display(Name = "15保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_15 { get; set; }

        [Display(Name = "16保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_16 { get; set; }

        [Display(Name = "16保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_16 { get; set; }

        [Display(Name = "17保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_17 { get; set; }

        [Display(Name = "17保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_17 { get; set; }

        [Display(Name = "18保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_18 { get; set; }

        [Display(Name = "18保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_18 { get; set; }

        [Display(Name = "19保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_19 { get; set; }

        [Display(Name = "19保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_19 { get; set; }

        [Display(Name = "20保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_20 { get; set; }

        [Display(Name = "20保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_20 { get; set; }

        [Display(Name = "21保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_21 { get; set; }

        [Display(Name = "21保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_21 { get; set; }

        [Display(Name = "22保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_22 { get; set; }

        [Display(Name = "22保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_22 { get; set; }

        [Display(Name = "23保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_23 { get; set; }

        [Display(Name = "23保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_23 { get; set; }

        [Display(Name = "24保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_24 { get; set; }

        [Display(Name = "24保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_24 { get; set; }

        [Display(Name = "25保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_25 { get; set; }

        [Display(Name = "25保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_25 { get; set; }

        [Display(Name = "26保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_26 { get; set; }

        [Display(Name = "26保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_26 { get; set; }

        [Display(Name = "27保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_27 { get; set; }

        [Display(Name = "27保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_27 { get; set; }

        [Display(Name = "28保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_28 { get; set; }

        [Display(Name = "28保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_28 { get; set; }

        [Display(Name = "29保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_29 { get; set; }

        [Display(Name = "29保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_29 { get; set; }

        [Display(Name = "30保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_30 { get; set; }

        [Display(Name = "30保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_30 { get; set; }

        [Display(Name = "31保养人（日保养）"), StringLength(50)]
        public string Day_Mainte_31 { get; set; }

        [Display(Name = "31保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_MainteTime_31 { get; set; }

        //组长确认和确认时间

        [Display(Name = "1组长确认（日保养）"), StringLength(50)]
        public string Day_group_1 { get; set; }

        [Display(Name = "1确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_1 { get; set; }

        [Display(Name = "2组长确认（日保养）"), StringLength(50)]
        public string Day_group_2 { get; set; }

        [Display(Name = "2确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_2 { get; set; }

        [Display(Name = "3组长确认（日保养）"), StringLength(50)]
        public string Day_group_3 { get; set; }

        [Display(Name = "3确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_3 { get; set; }

        [Display(Name = "4组长确认（日保养）"), StringLength(50)]
        public string Day_group_4 { get; set; }

        [Display(Name = "4确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_4 { get; set; }

        [Display(Name = "5组长确认（日保养）"), StringLength(50)]
        public string Day_group_5 { get; set; }

        [Display(Name = "5确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_5 { get; set; }

        [Display(Name = "6组长确认（日保养）"), StringLength(50)]
        public string Day_group_6 { get; set; }

        [Display(Name = "6确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_6 { get; set; }

        [Display(Name = "7组长确认（日保养）"), StringLength(50)]
        public string Day_group_7 { get; set; }

        [Display(Name = "7确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_7 { get; set; }

        [Display(Name = "8组长确认（日保养）"), StringLength(50)]
        public string Day_group_8 { get; set; }

        [Display(Name = "8确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_8 { get; set; }

        [Display(Name = "9组长确认（日保养）"), StringLength(50)]
        public string Day_group_9 { get; set; }

        [Display(Name = "9确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_9 { get; set; }

        [Display(Name = "10组长确认（日保养）"), StringLength(50)]
        public string Day_group_10 { get; set; }

        [Display(Name = "10确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_10 { get; set; }

        [Display(Name = "11组长确认（日保养）"), StringLength(50)]
        public string Day_group_11 { get; set; }

        [Display(Name = "11确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_11 { get; set; }

        [Display(Name = "12组长确认（日保养）"), StringLength(50)]
        public string Day_group_12 { get; set; }

        [Display(Name = "12确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_12 { get; set; }

        [Display(Name = "13组长确认（日保养）"), StringLength(50)]
        public string Day_group_13 { get; set; }

        [Display(Name = "13确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_13 { get; set; }

        [Display(Name = "14组长确认（日保养）"), StringLength(50)]
        public string Day_group_14 { get; set; }

        [Display(Name = "14确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_14 { get; set; }

        [Display(Name = "15组长确认（日保养）"), StringLength(50)]
        public string Day_group_15 { get; set; }

        [Display(Name = "15确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_15 { get; set; }

        [Display(Name = "16组长确认（日保养）"), StringLength(50)]
        public string Day_group_16 { get; set; }

        [Display(Name = "16确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_16 { get; set; }

        [Display(Name = "17组长确认（日保养）"), StringLength(50)]
        public string Day_group_17 { get; set; }

        [Display(Name = "17确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_17 { get; set; }

        [Display(Name = "18组长确认（日保养）"), StringLength(50)]
        public string Day_group_18 { get; set; }

        [Display(Name = "18确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_18 { get; set; }

        [Display(Name = "19组长确认（日保养）"), StringLength(50)]
        public string Day_group_19 { get; set; }

        [Display(Name = "19确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_19 { get; set; }

        [Display(Name = "20组长确认（日保养）"), StringLength(50)]
        public string Day_group_20 { get; set; }

        [Display(Name = "20确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_20 { get; set; }

        [Display(Name = "21组长确认（日保养）"), StringLength(50)]
        public string Day_group_21 { get; set; }

        [Display(Name = "21确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_21 { get; set; }

        [Display(Name = "22组长确认（日保养）"), StringLength(50)]
        public string Day_group_22 { get; set; }

        [Display(Name = "22确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_22 { get; set; }

        [Display(Name = "23组长确认（日保养）"), StringLength(50)]
        public string Day_group_23 { get; set; }

        [Display(Name = "23确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_23 { get; set; }

        [Display(Name = "24组长确认（日保养）"), StringLength(50)]
        public string Day_group_24 { get; set; }

        [Display(Name = "24确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_24 { get; set; }

        [Display(Name = "25组长确认（日保养）"), StringLength(50)]
        public string Day_group_25 { get; set; }

        [Display(Name = "25确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_25 { get; set; }

        [Display(Name = "26组长确认（日保养）"), StringLength(50)]
        public string Day_group_26 { get; set; }

        [Display(Name = "26确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_26 { get; set; }

        [Display(Name = "27组长确认（日保养）"), StringLength(50)]
        public string Day_group_27 { get; set; }

        [Display(Name = "27确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_27 { get; set; }

        [Display(Name = "28组长确认（日保养）"), StringLength(50)]
        public string Day_group_28 { get; set; }

        [Display(Name = "28确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_28 { get; set; }

        [Display(Name = "29组长确认（日保养）"), StringLength(50)]
        public string Day_group_29 { get; set; }

        [Display(Name = "29确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_29 { get; set; }

        [Display(Name = "30组长确认（日保养）"), StringLength(50)]
        public string Day_group_30 { get; set; }

        [Display(Name = "30确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_30 { get; set; }

        [Display(Name = "31组长确认（日保养）"), StringLength(50)]
        public string Day_group_31 { get; set; }

        [Display(Name = "31确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Day_groupTime_31 { get; set; }

        //周保养
        [Display(Name = "1周保养项目"), StringLength(200)]
        public string Week_Check1 { get; set; }

        [Display(Name = "1操作方法（周保养）"), StringLength(600)]
        public string Week_Inspe1 { get; set; }

        [Display(Name = "2周保养项目"), StringLength(200)]
        public string Week_Check2 { get; set; }

        [Display(Name = "2操作方法（周保养）"), StringLength(600)]
        public string Week_Inspe2 { get; set; }

        [Display(Name = "3周保养项目"), StringLength(200)]
        public string Week_Check3 { get; set; }

        [Display(Name = "3操作方法（周保养）"), StringLength(600)]
        public string Week_Inspe3 { get; set; }

        [Display(Name = "4周保养项目"), StringLength(200)]
        public string Week_Check4 { get; set; }

        [Display(Name = "4操作方法（周保养）"), StringLength(600)]
        public string Week_Inspe4 { get; set; }

        [Display(Name = "5周保养项目"), StringLength(200)]
        public string Week_Check5 { get; set; }

        [Display(Name = "5操作方法（周保养）"), StringLength(600)]
        public string Week_Inspe5 { get; set; }

        [Display(Name = "6周保养项目"), StringLength(200)]
        public string Week_Check6 { get; set; }

        [Display(Name = "6操作方法（周保养）"), StringLength(600)]
        public string Week_Inspe6 { get; set; }

        [Display(Name = "7周保养项目"), StringLength(200)]
        public string Week_Check7 { get; set; }

        [Display(Name = "7操作方法（周保养）"), StringLength(600)]
        public string Week_Inspe7 { get; set; }

        [Display(Name = "8周保养项目"), StringLength(200)]
        public string Week_Check8 { get; set; }

        [Display(Name = "8操作方法（周保养）"), StringLength(600)]
        public string Week_Inspe8 { get; set; }

        [Display(Name = "9周保养项目"), StringLength(200)]
        public string Week_Check9 { get; set; }

        [Display(Name = "9操作方法（周保养）"), StringLength(600)]
        public string Week_Inspe9 { get; set; }

        [Display(Name = "10周保养项目"), StringLength(200)]
        public string Week_Check10 { get; set; }

        [Display(Name = "10操作方法（周保养）"), StringLength(600)]
        public string Week_Inspe10 { get; set; }

        [Display(Name = "11周保养项目"), StringLength(200)]
        public string Week_Check11 { get; set; }

        [Display(Name = "11操作方法（周保养）"), StringLength(600)]
        public string Week_Inspe11 { get; set; }

        [Display(Name = "1保养日期（周）"), StringLength(800)]
        public string Week_1 { get; set; }

        [Display(Name = "2保养日期（周）"), StringLength(800)]
        public string Week_2 { get; set; }

        [Display(Name = "3保养日期（周）"), StringLength(800)]
        public string Week_3 { get; set; }

        [Display(Name = "4保养日期（周）"), StringLength(800)]
        public string Week_4 { get; set; }

        [Display(Name = "1保养人（周保养）"), StringLength(50)]
        public string Week_Main_1 { get; set; }

        [Display(Name = "1保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Week_MainTime_1 { get; set; }

        [Display(Name = "2保养人（周保养）"), StringLength(50)]
        public string Week_Main_2 { get; set; }

        [Display(Name = "2保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Week_MainTime_2 { get; set; }

        [Display(Name = "3保养人（周保养）"), StringLength(50)]
        public string Week_Main_3 { get; set; }

        [Display(Name = "3保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Week_MainTime_3 { get; set; }

        [Display(Name = "4保养人（周保养）"), StringLength(50)]
        public string Week_Main_4 { get; set; }

        [Display(Name = "4保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Week_MainTime_4 { get; set; }

        [Display(Name = "1工程师确认（周保养）"), StringLength(50)]
        public string Week_engineer_1 { get; set; }

        [Display(Name = "1确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Week_engTime_1 { get; set; }

        [Display(Name = "2工程师确认（周保养）"), StringLength(50)]
        public string Week_engineer_2 { get; set; }

        [Display(Name = "2确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Week_engTime_2 { get; set; }

        [Display(Name = "3工程师确认（周保养）"), StringLength(50)]
        public string Week_engineer_3 { get; set; }

        [Display(Name = "3确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Week_engTime_3 { get; set; }

        [Display(Name = "4工程师确认（周保养）"), StringLength(50)]
        public string Week_engineer_4 { get; set; }

        [Display(Name = "4确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Week_engTime_4 { get; set; }

        //月保养
        [Display(Name = "1月保养项目"), StringLength(200)]
        public string Month_Project1 { get; set; }

        [Display(Name = "1操作方法（月保养）"), StringLength(600)]
        public string Month_Approach1 { get; set; }

        [Display(Name = "1保养确认信息（月）"), StringLength(50)]
        public string Month_1 { get; set; }

        [Display(Name = "2月保养项目"), StringLength(200)]
        public string Month_Project2 { get; set; }

        [Display(Name = "2操作方法（月保养）"), StringLength(600)]
        public string Month_Approach2 { get; set; }

        [Display(Name = "2保养确认信息（月）"), StringLength(50)]
        public string Month_2 { get; set; }

        [Display(Name = "3月保养项目"), StringLength(200)]
        public string Month_Project3 { get; set; }

        [Display(Name = "3操作方法（月保养）"), StringLength(600)]
        public string Month_Approach3 { get; set; }

        [Display(Name = "3保养确认信息（月）"), StringLength(50)]
        public string Month_3 { get; set; }

        [Display(Name = "4月保养项目"), StringLength(200)]
        public string Month_Project4 { get; set; }

        [Display(Name = "4操作方法（月保养）"), StringLength(600)]
        public string Month_Approach4 { get; set; }

        [Display(Name = "4保养确认信息（月）"), StringLength(50)]
        public string Month_4 { get; set; }

        [Display(Name = "5月保养项目"), StringLength(200)]
        public string Month_Project5 { get; set; }

        [Display(Name = "5操作方法（月保养）"), StringLength(600)]
        public string Month_Approach5 { get; set; }

        [Display(Name = "5保养确认信息（月）"), StringLength(50)]
        public string Month_5 { get; set; }

        [Display(Name = "6月保养项目"), StringLength(200)]
        public string Month_Project6 { get; set; }

        [Display(Name = "6操作方法（月保养）"), StringLength(600)]
        public string Month_Approach6 { get; set; }

        [Display(Name = "6保养确认信息（月）"), StringLength(50)]
        public string Month_6 { get; set; }

        [Display(Name = "7月保养项目"), StringLength(200)]
        public string Month_Project7 { get; set; }

        [Display(Name = "7操作方法（月保养）"), StringLength(600)]
        public string Month_Approach7 { get; set; }

        [Display(Name = "7保养确认信息（月）"), StringLength(50)]
        public string Month_7 { get; set; }

        [Display(Name = "8月保养项目"), StringLength(200)]
        public string Month_Project8 { get; set; }

        [Display(Name = "8操作方法（月保养）"), StringLength(600)]
        public string Month_Approach8 { get; set; }

        [Display(Name = "8保养确认信息（月）"), StringLength(50)]
        public string Month_8 { get; set; }

        [Display(Name = "9月保养项目"), StringLength(200)]
        public string Month_Project9 { get; set; }

        [Display(Name = "9操作方法（月保养）"), StringLength(600)]
        public string Month_Approach9 { get; set; }

        [Display(Name = "9保养确认信息（月）"), StringLength(50)]
        public string Month_9 { get; set; }

        [Display(Name = "10月保养项目"), StringLength(200)]
        public string Month_Project10 { get; set; }

        [Display(Name = "10操作方法（月保养）"), StringLength(600)]
        public string Month_Approach10 { get; set; }

        [Display(Name = "10保养确认信息（月）"), StringLength(50)]
        public string Month_10 { get; set; }

        [Display(Name = "11月保养项目"), StringLength(200)]
        public string Month_Project11 { get; set; }

        [Display(Name = "11操作方法（月保养）"), StringLength(600)]
        public string Month_Approach11 { get; set; }

        [Display(Name = "11保养确认信息（月）"), StringLength(50)]
        public string Month_11 { get; set; }

        [Display(Name = "1保养人（月保养）"), StringLength(50)]
        public string Month_main_1 { get; set; }

        [Display(Name = "1保养时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Month_mainTime_1 { get; set; }

        [Display(Name = "2生产确认（月保养）"), StringLength(50)]
        public string Month_productin_2 { get; set; }

        [Display(Name = "2确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Month_produTime_2 { get; set; }

        [Display(Name = "3部长确认（月保养）"), StringLength(50)]
        public string Month_minister_3 { get; set; }

        [Display(Name = "3确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Month_minisime_3 { get; set; }

        [Display(Name = "说明"), StringLength(1500)]
        public string Remark { get; set; }

    }

    //设备报修单
    public class EquipmentRepairbill
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "设备使用部门"), StringLength(50)]
        public string UserDepartment { get; set; }

        [Display(Name = "设备编号"), StringLength(50)]
        public string EquipmentNumber { get; set; }

        [Display(Name = "设备名称"), StringLength(50)]
        public string EquipmentName { get; set; }

        [Display(Name = "故障时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FaultTime { get; set; }

        [Display(Name = "紧急状态"), StringLength(100)]
        public string Emergency { get; set; }

        //故障简述
        [Display(Name = "故障简述"), StringLength(1500)]
        public string FauDescription { get; set; }

        [Display(Name = "报修人员"), StringLength(50)]
        public string RepairName { get; set; }

        [Display(Name = "报修时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? RepairDate { get; set; }

        [Display(Name = "部门审核"), StringLength(50)]
        public string DeparAssessor { get; set; }

        [Display(Name = "审核时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? DeparAssessedDate { get; set; }

        [Display(Name = "中心总监批准"), StringLength(50)]
        public string CenterApprove { get; set; }

        [Display(Name = "批准时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? CenterApprovedDate { get; set; }

        //技术部意见
        [Display(Name = "技术部意见"), StringLength(1000)]
        public string TecDepar_opinion { get; set; }

        [Display(Name = "ME工程师"), StringLength(50)]
        public string MEName { get; set; }

        [Display(Name = "填写时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? MEDate { get; set; }

        [Display(Name = "部门审核"), StringLength(50)]
        public string TecDeparAssessor { get; set; }

        [Display(Name = "审核时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? TecDeparAssessedDate { get; set; }

        [Display(Name = "中心总监批准"), StringLength(50)]
        public string CeApprove { get; set; }

        [Display(Name = "批准时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? CeApprovedDate { get; set; }

        //联建采购意见
        [Display(Name = "联建采购意见"), StringLength(1000)]
        public string Purchasing_opinion { get; set; }

        [Display(Name = "意见人员"), StringLength(1000)]
        public string OpinionName { get; set; }

        [Display(Name = "意见人填写日期间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? OpinionDate { get; set; }

        [Display(Name = "审核人"), StringLength(50)]
        public string OpinAssessor { get; set; }

        [Display(Name = "审核时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? OpinAssessedDate { get; set; }

        [Display(Name = "批准人"), StringLength(50)]
        public string OpinApprove { get; set; }

        [Display(Name = "批准时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? OpinApprovedDate { get; set; }

        //维修后确认（维修需要部门）
        [Display(Name = "维修时间")]
        public DateTime? MaintenanceDate { get; set; }

        [Display(Name = "维修人（厂家）"), StringLength(150)]
        public string MainName { get; set; }

        [Display(Name = "维修后效果确认（维修需要部门）"), StringLength(1000)]
        public string AfterMain { get; set; }

        [Display(Name = "报修问题是否已解决")]
        public int RepairProblem { get; set; }

        [Display(Name = "确认人"), StringLength(50)]
        public string ConfirmName { get; set; }

        [Display(Name = "确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? ConfirmDate { get; set; }

        //维修后确认（技术部）
        [Display(Name = "维修后效果确认（技术部）"), StringLength(1000)]
        public string TcAfterMin { get; set; }

        [Display(Name = "报修问题是否已解决")]
        public int TcRepairProblem { get; set; }

        [Display(Name = "确认人"), StringLength(50)]
        public string TcConfirmName { get; set; }

        [Display(Name = "确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? TcConfirmDate { get; set; }

        [Display(Name = "是否需要联建采购意见")]
        public bool Needto { get; set; }

    }

    //设备月保养时间计划表
    public class Equipment_MonthlyMaintenance
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "使用部门"), StringLength(50)]
        public string UserDepartment { get; set; }

        [Display(Name = "设备名称"), StringLength(50)]
        public string EquipmentName { get; set; }

        [Display(Name = "设备编号"), StringLength(50)]
        public string EquipmentNumber { get; set; }

        [Display(Name = "计划保养日期")]
        public DateTime? Mainten_equipment { get; set; }

        [Display(Name = "实际保养日期")]
        public DateTime? MaintenanceDate { get; set; }

        [Display(Name = "下次保养周期（保养有效期）")]
        public DateTime? Nextmainten_cycle { get; set; }

        [Display(Name = "保养工时"), StringLength(50)]
        public string Maintenance_work { get; set; }

        [Display(Name = "保养负责人"), StringLength(100)]
        public string Mainten_supervisor { get; set; }

        [Display(Name = "月保养设备异常记录"), StringLength(200)]
        public string Abnormal_records { get; set; }

        [Display(Name = "备注"), StringLength(1500)]
        public string Remark { get; set; }

        [Display(Name = "制表人"), StringLength(50)]
        public string Mainten_Lister { get; set; }

        [Display(Name = "制表时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? TabulationTime { get; set; }

        [Display(Name = "技术部确认"), StringLength(50)]
        public string Tec_Notarize { get; set; }

        [Display(Name = "技术部确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Tec_NotarizeTime { get; set; }

        [Display(Name = "保养设备部门确认"), StringLength(50)]
        public string AssortDepar { get; set; }

        [Display(Name = "保养设备部门确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? AssortTime { get; set; }

        [Display(Name = "PC部确认"), StringLength(50)]
        public string PCDepar { get; set; }

        [Display(Name = "PC部确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? PCdeparTime { get; set; }

        [Display(Name = "审核"), StringLength(50)]
        public string Assessor { get; set; }

        [Display(Name = "审核时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? AssessedDate { get; set; }

        [Display(Name = "年")]
        public int Year { get; set; }

        [Display(Name = "月")]
        public int Month { get; set; }

        [Display(Name = "记录修改人"), StringLength(50)]
        public string Modifier { get; set; }

        [Display(Name = "修改时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }

    }

    //月保养质量目标达成状况统计表
    public class Equipment_Quality_target
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "担责部门"), StringLength(100)]
        public string LiaDepartment { get; set; }

        [Display(Name = "使用部门"), StringLength(50)]
        public string UserDepartment { get; set; }

        [Display(Name = "质量目标"), StringLength(300)]
        public string Quality_objec { get; set; }

        [Display(Name = "目标值"), StringLength(80)]
        public string Target_value { get; set; }

        [Display(Name = "计算公式"), StringLength(200)]
        public string Formulas { get; set; }

        [Display(Name = "统计周期"), StringLength(100)]
        public string Statistical { get; set; }

        [Display(Name = "项目：按规定要求保养台天次"), StringLength(150)]
        public string Required_maintain { get; set; }

        [Display(Name = "项目：按规定要求保养台天次实际值")]
        public int Required { get; set; }

        [Display(Name = "项目：计划保养总台天次"), StringLength(150)]
        public string Planned_maintenance { get; set; }

        [Display(Name = "项目：计划保养总台天次实际值")]
        public int Planned { get; set; }

        [Display(Name = "项目：有效率"), StringLength(80)]//格式100%
        public string With_efficiency { get; set; }

        [Display(Name = "项目：有效率")]//格式100%
        public Decimal efficiency { get; set; }

        [Display(Name = "年")]
        public int Year { get; set; }

        [Display(Name = "月")]
        public int Month { get; set; }

        [Display(Name = "备注"), StringLength(1500)]
        public string Remark { get; set; }

        [Display(Name = "编制人"), StringLength(50)]
        public string PrepareName { get; set; }

        [Display(Name = "编制日期"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? PrepareTime { get; set; }

        [Display(Name = "审核人"), StringLength(50)]
        public string Assessor { get; set; }

        [Display(Name = "审核时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? AssessedDate { get; set; }

        [Display(Name = "批准人"), StringLength(50)]
        public string Approve { get; set; }

        [Display(Name = "批准时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApprovedDate { get; set; }

        [Display(Name = "记录修改人"), StringLength(50)]
        public string Modifier { get; set; }

        [Display(Name = "修改时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }
    }

    //设备、设施、工具配件安全库存清单汇总表
    public class Equipment_Safetystock
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "使用部门"), StringLength(50)]
        public string UserDepartment { get; set; }

        [Display(Name = "设备名称"), StringLength(50)]
        public string EquipmentName { get; set; }

        [Display(Name = "品名"), StringLength(150)]
        public string Descrip { get; set; }

        [Display(Name = "规格/型号"), StringLength(150)]
        public string Specifica { get; set; }

        [Display(Name = "配料料号"), StringLength(150)]
        public string Material { get; set; }

        [Display(Name = "使用寿命"), StringLength(50)]
        public string Servicelife { get; set; }

        [Display(Name = "月平均用量"), StringLength(50)]
        public string Amount { get; set; }

        [Display(Name = "现有库存")]
        public double? Existing_inventory { get; set; }

        [Display(Name = "现有库存详情"), StringLength(1000)]
        public string Existing_inventory_Details { get; set; }

        [Display(Name = "安全库存量"), StringLength(50)]
        public string Safety_stock { get; set; }

        [Display(Name = "采购周期"), StringLength(50)]
        public string Purchasing_cycle { get; set; }

        [Display(Name = "用途"), StringLength(150)]
        public string Materused { get; set; }

        [Display(Name = "备注"), StringLength(150)]
        public string Remark { get; set; }

        [Display(Name = "年")]
        public int Year { get; set; }

        [Display(Name = "月")]
        public int Month { get; set; }

        [Display(Name = "配件安全库存说明"), StringLength(1500)]
        public string Explain { get; set; }

        [Display(Name = "资料整理人"), StringLength(50)]
        public string FinishingName { get; set; }

        [Display(Name = "资料整理时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FinishingDate { get; set; }

        [Display(Name = "技术部审核"), StringLength(50)]
        public string Tec_Assessor { get; set; }

        [Display(Name = "技术部审核时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Tec_AssessedDate { get; set; }

        [Display(Name = "装配1/2部确认"), StringLength(50)]
        public string Assembling_depar { get; set; }

        [Display(Name = "装配1/2部确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? Assembling_deparDate { get; set; }

        [Display(Name = "SMT1/2部确认"), StringLength(50)]
        public string SMTDepar { get; set; }

        [Display(Name = "SMT1/2部确认时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? SMTDeparDate { get; set; }

        [Display(Name = "MC部审核"), StringLength(50)]
        public string Assessor { get; set; }

        [Display(Name = "审核时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? AssessedDate { get; set; }

        [Display(Name = "工厂厂长批准"), StringLength(50)]
        public string Approve { get; set; }

        [Display(Name = "批准时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApprovedDate { get; set; }

        [Display(Name = "记录修改人"), StringLength(50)]
        public string Modifier { get; set; }

        [Display(Name = "修改时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }
    }

    //关键元器件清单汇总表
    public class Equipment_keycomponents
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "设备名称"), StringLength(50)]
        public string EquipmentName { get; set; }

        [Display(Name = "设备编号"), StringLength(50)]
        public string EquipmentNumber { get; set; }

        [Display(Name = "品名"), StringLength(150)]
        public string Descrip { get; set; }

        [Display(Name = "规格/型号"), StringLength(150)]
        public string Specifica { get; set; }

        [Display(Name = "用途"), StringLength(150)]
        public string Materused { get; set; }

        [Display(Name = "备注"), StringLength(150)]
        public string Remark { get; set; }

        [Display(Name = "关键元器清单说明"), StringLength(1500)]
        public string Explain { get; set; }

        [Display(Name = "制表人"), StringLength(50)]
        public string Mainten_Lister { get; set; }

        [Display(Name = "制表时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? TabulationTime { get; set; }

        [Display(Name = "记录修改人"), StringLength(50)]
        public string Modifier { get; set; }

        [Display(Name = "修改时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }
    }


    //设备月保养时间计划表邮件发送情况表
    public class Equipment_Maintenmail
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "设备名称"), StringLength(50)]
        public string EquipmentName { get; set; }

        [Display(Name = "设备编号"), StringLength(50)]
        public string EquipmentNumber { get; set; }

        [Display(Name = "计划保养日期")]
        public DateTime? Mainten_equipment { get; set; }

        [Display(Name = "实际保养日期")]
        public DateTime? MaintenanceDate { get; set; }

        [Display(Name = "下次保养周期（保养有效期）")]
        public DateTime? Nextmainten_cycle { get; set; }

        [Display(Name = "年")]
        public int Year { get; set; }

        [Display(Name = "月")]
        public int Month { get; set; }

        [Display(Name = "发送状态")]
        public bool Sended { get; set; }

        [Display(Name = "发送时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? SendDateTime { get; set; }

        [Display(Name = "发送情况"), StringLength(1000)]
        public string SendSituation { get; set; }

        [Display(Name = "保养情况"), StringLength(100)]
        public string Situation { get; set; }
    }

    //安全库存清单，现有库存小于安全安全库存邮件发送情况
    public class Equipment_SafetyEmail
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "设备名称"), StringLength(50)]
        public string EquipmentName { get; set; }

        [Display(Name = "品名(配件名称)"), StringLength(150)]
        public string Descrip { get; set; }

        [Display(Name = "配料料号"), StringLength(150)]
        public string Material { get; set; }

        [Display(Name = "现有库存")]
        public double? Existing_inventory { get; set; }

        [Display(Name = "安全库存量"), StringLength(50)]
        public string Safety_stock { get; set; }

        [Display(Name = "发送状态")]
        public bool Sended { get; set; }

        [Display(Name = "发送时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? SendDateTime { get; set; }

        [Display(Name = "发送情况"), StringLength(1000)]
        public string SendSituation { get; set; }
    }

}






