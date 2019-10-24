﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JianHeMES.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            //authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 此处添加自定义用户声明
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        #region---组装PQC
        //组装
        public DbSet<Assemble> Assemble { get; set; }
        public DbSet<Waterproofabnormal> Waterproofabnormal { get; set; }
        public DbSet<ViewCheckabnormal> ViewCheckabnormal { get; set; }
        public DbSet<PQCCheckabnormal> PQCCheckabnormal { get; set; }
        #endregion

        #region---FQC
        //FQC
        public DbSet<FinalQC> FinalQC { get; set; }
        #endregion

        #region---条码
        //条码
        public DbSet<BarCodes> BarCodes { get; set; }
        //挪用订单的条码关联
        public DbSet<BarCodeRelation> BarCodeRelation { get; set; }
        #endregion

        #region---订单
        //订单
        public DbSet<OrderMgm> OrderMgm { get; set; }
        public DbSet<OrderMgm_Delete> OrderMgm_Delete { get; set; }
        #endregion

        #region---系统用户
        //用户信息
        public DbSet<Users> Users { get; set; }
        public DbSet<Useroles> Useroles { get; set; }
        #endregion

        #region---权限
        //权限清单
        public DbSet<UserRolelistTable> UserRolelistTable { get; set; }
        public DbSet<UserManageRoles> UserManageRoles { get; set; }
        public DbSet<UserPositionList> UserPositionList { get; set; }
        public DbSet<UserCanManagePosition> UserCanManagePosition { get; set; }
        public DbSet<UserOperateLog> UserOperateLog { get; set; }
        #endregion

        #region---校正
        public DbSet<OrderInformation> OrderInformation { get; set; }
        //校正
        public DbSet<CalibrationRecord> CalibrationRecord { get; set; }
        public DbSet<ModelCollections> ModelCollections { get; set; }
        public DbSet<AdapterCard_Power_Collection> AdapterCard_Power_Collection { get; set; }
        public DbSet<AssembleLineId> AssembleLineId { get; set; }
        #endregion

        #region---拼屏老化
        //老化
        public DbSet<Burn_in> Burn_in { get; set; }
        //拼屏
        public DbSet<Burn_in_MosaicScreen> Burn_in_MosaicScreen { get; set; }
        public DbSet<Burn_in_OQCCheckAbnormal> Burn_in_OQCCheckAbnormal { get; set; }
        #endregion

        #region---外观电检
        //外观电检OQC
        public DbSet<Appearance> Appearance { get; set; }
        public DbSet<Appearance_OQCCheckAbnormal> Appearance_OQCCheckAbnormal { get; set; }
        #endregion

        #region---包装
        public DbSet<Packaging> Packaging { get; set; }
        //包装基本信息
        public DbSet<Packing_BasicInfo> Packing_BasicInfo { get; set; }

        //包装条码打印信息
        public DbSet<Packing_BarCodePrinting> Packing_BarCodePrinting { get; set; }

        //内箱打印确认
        public DbSet<Packing_InnerCheck> Packing_InnerCheck { get; set; }
        #endregion

        #region---出入库产值
        //入库基本信息
        public DbSet<Warehouse_Join> Warehouse_Join { get; set; }

        //出库基本信息
        public DbSet<Warehouse_Out> Warehouse_Out { get; set; }

        //产值表
        public DbSet<Production_Value> Production_Value { get; set; }
        #endregion

        #region---IQC
        //IQCReports
        public DbSet<IQCReport> IQCReports { get; set; }
        #endregion

        #region---SMT生产管理
        //SMT生产计划
        public DbSet<SMT_ProductionPlan> SMT_ProductionPlan { get; set; }
        //SMT线别信息
        public DbSet<SMT_ProductionLineInfo> SMT_ProductionLineInfo { get; set; }
        //SMT生产数据
        public DbSet<SMT_ProductionData> SMT_ProductionData { get; set; }

        public DbSet<SMT_OrderInfo> SMT_OrderInfo { get; set; }
        //SMT看板信息
        public DbSet<SMT_ProductionBoardTable> SMT_ProductionBoardTable { get; set; }
        #endregion

        #region---锡膏管理
        //锡膏入库基本表
        public DbSet<Warehouse_BaseInfo> Warehouse_BaseInfo { get; set; }

        //锡膏条码表
        public DbSet<Barcode_Solderpaste> Barcode_Solderpaste { get; set; }

        //仓库冰柜表
        public DbSet<Warehouse_Freezer> Warehouse_Freezer { get; set; }

        //SMT冰柜表
        public DbSet<SMT_Freezer> SMT_Freezer { get; set; }

        //SMT回温表
        public DbSet<SMT_Rewarm> SMT_Rewarm { get; set; }

        //SMT搅拌表
        public DbSet<SMT_Stir> SMT_Stir { get; set; }

        //SMT使用表
        public DbSet<SMT_Employ> SMT_Employ { get; set; }

        //锡膏回收
        public DbSet<SMT_Recycle> SMT_Recycle { get; set; }
        #endregion

        #region---人力资源部
        //人事日报表
        public DbSet<Personnel_daily> Personnel_daily { get; set; }
        //人工成本对照表
        public DbSet<Personnel_of_Contrast> Personnel_of_Contrast { get; set; }
        //月流失率表
        public DbSet<Personnel_Turnoverrate> Personnel_Turnoverrate { get; set; }
        //招聘周报
        public DbSet<Personnel_Recruitment> Personnel_Recruitment { get; set; }
        //花名册
        public DbSet<Personnel_Roster> Personnel_Roster { get; set; }
        //组织构架
        public DbSet<Personnel_Framework> Personnel_Framework { get; set; }
        public DbSet<Personnel_Organization> Personnel_Organization { get; set; }

        //部门架构版本
        public DbSet<Personnel_Architecture> Personnel_Architecture { get; set; }

        //请假信息
        public DbSet<Personnel_Leave> Personnel_Leave { get; set; }

        //离职信息
        public DbSet<Personnel_Reasons_for_leaving> Personnel_Reasons_for_leaving { get; set; }

        //非在岗人员信息
        public DbSet<Personnel_NotWorkingInfo> Personnel_NotWorkingInfo { get; set; }

        //实际完成值数据表
        public DbSet<Personnel_Quality_objectives> Personnel_Quality_objectives { get; set; }

        //考核指标版本
        public DbSet<Personnel_Assessment_indicators> Personnel_Assessment_indicators { get; set; }

        //出勤率，违纪记录表
        public DbSet<Personnel_Ranking_management> Personnel_Ranking_management { get; set; }

        //指标名称版本
        public DbSet<Personnel_IndexName> Personnel_IndexName { get; set; }

        //判断条件版本
        public DbSet<Personnel_Judge> Personnel_Judge { get; set; }

        #endregion

        #region---MC部仓库管理
        //物料表
        public DbSet<Warehouse_Material> Warehouse_Material { get; set; }
        #endregion

        #region---设备管理
        //设备基本信息表
        public DbSet<EquipmentBasicInfo> EquipmentBasicInfo { get; set; }

        //设备状态记录表
        public DbSet<EquipmentStatusRecord> EquipmentStatusRecord { get; set; }

        //设备产线位置记录表
        public DbSet<EquipmentSetStation> EquipmentSetStation { get; set; }

        //设备点检保养
        public DbSet<Equipment_Tally_maintenance> Equipment_Tally_maintenance { get; set; }

        //设备点检保养项目和操作方法
        public DbSet<Equipment_Checkthe_project> Equipment_Checkthe_project { get; set; }

        //仪器设备保修单
        public DbSet<EquipmentRepairbill> EquipmentRepairbill { get; set; }

        //设备月保养时间计划表
        public DbSet<Equipment_MonthlyMaintenance> Equipment_MonthlyMaintenance { get; set; }

        #endregion

        #region---SOP管理
        //SOP
        public DbSet<SOPoperating> SOPoperating { get; set; }
        #endregion

        #region---球场屏
        //球场屏
        public DbSet<CourtScreenModuleInfo> CourtScreenModuleInfoes { get; set; }
        #endregion

        #region ---模组号规则
        public DbSet<Barcode_Module> Barcode_Module { get; set; }
        #endregion

        #region ---小样报告
        public DbSet<Small_Sample> Small_Sample { get; set; }

        //IC电流算法Small_Sample_IC_Model_Arguments
        public DbSet<Small_Sample_IC_Model_Arguments> Small_Sample_IC_Model_Arguments { get; set; }

        //IC型号和间距
        public DbSet<Small_Sample_ICmodel_Spacing> Small_Sample_ICmodel_Spacing { get; set; }

        //间距和最高/低值
        public DbSet<Small_Sample_Spacing_Value> Small_Sample_Spacing_Value { get; set; }

        #endregion

        #region ---工序产能
        //public DbSet<Pick_And_Place> Pick_And_Place { get; set; }
        //public DbSet<ProcessBalance> ProcessBalance { get; set; }
        //public DbSet<Process_Capacity_Total> Process_Capacity_Total { get; set; }

        #endregion

        public DbSet<Test> Tests { get; set; }

        public object BarCode { get; internal set; }
        public Task TNews { get; internal set; }

        //创建数据库操作上下文，EF需要这个文件来创建和访问数据库
        //完成后需要重新编译项目（快捷键Ctrl+Shift+B），否则下面添加控制器时会出错。

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

    }
}