﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class Users　　//1.用户表
    {
        [Key]
        public int ID { get; set; }

        //[Required]
        [Display(Name = "员工编号")]
        public int UserNum { get; set; }

        [Display(Name = "员工编号"), StringLength(50)]
        public string UserNumber { get; set; }

        //[Required]
        [Display(Name = "用户名"), StringLength(50)]
        public string UserName { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        [Display(Name = "密码"), StringLength(50)]
        public string PassWord { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "创建时间")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "创建人"), StringLength(50)]
        public string Creator { get; set; }

        [Display(Name = "权限")]
        public int UserAuthorize { get; set; }

        [Display(Name = "角色"), StringLength(50)]
        public string Role { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "产线")]
        public int LineNum { get; set; }

        [Display(Name = "删除人"), StringLength(50)]
        public string Deleter { get; set; }

        [Display(Name = "删除时间")]
        [DataType(DataType.DateTime)]
        public DateTime? DeleteDate { get; set; }

        [Display(Name = "描述"), StringLength(100)]
        public string Description { get; set; }

        public virtual List<OrderMgm> OrderMgm { get; set; }
        public virtual List<Assemble> Assemble { get; set; }
        public virtual List<BarCodes> BarCodes { get; set; }
    }

    public class Useroles  //2.使用权限表
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "姓名"), StringLength(50)]
        public string UserName { get; set; }

        [Display(Name = "工号")]
        public int UserID { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "职位"), StringLength(50)]
        public string Position { get; set; }

        [Display(Name = "权限名"), StringLength(50)]
        public string RolesName { get; set; }

        [Display(Name = "权限"), StringLength(500)]
        public string Roles { get; set; }

        [Display(Name = "模块"), StringLength(50)]
        public string FourModule { get; set; }

        [Display(Name = "新权限名"), StringLength(50)]
        public string New_RolesName { get; set; }

        [Display(Name = "新权限"), StringLength(500)]
        public string New_Roles { get; set; }
    }

    public class UserRolelistTable  //3.权限清单表
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "权限代码")]
        public int RolesCode { get; set; }

        [Display(Name = "权限组"), StringLength(50)]
        public string RolesName { get; set; }

        [Display(Name = "权限名"), StringLength(50)]
        public string Discription { get; set; }

        [Display(Name = "操作人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "操作时间")]
        public DateTime OperateDT { get; set; }
    }

    public class UserNewPermissions  //权限清单表(新的权限清单表)
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "模块"), StringLength(50)]
        public string FourModule { get; set; }

        [Display(Name = "权限代码")]
        public int RolesCode { get; set; }

        [Display(Name = "权限组"), StringLength(50)]
        public string RolesName { get; set; }

        [Display(Name = "权限名"), StringLength(50)]
        public string Discription { get; set; }

        [Display(Name = "创建人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime OperateDT { get; set; }
    }

    public class UserManageRoles  //4.管理权限表
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "管理者部门"), StringLength(20)]
        public string AdministratorDepartment { get; set; }

        [Display(Name = "管理者职位"), StringLength(20)]
        public string AdministratorPosition { get; set; }

        [Display(Name = "被管理者部门"), StringLength(20)]
        public string ToBeManageDepartment { get; set; }

        [Display(Name = "被管理者职位"), StringLength(20)]
        public string ToBeManagePosition { get; set; }

        [Display(Name = "标识")]
        public int Distribution { get; set; }

        [Display(Name = "操作人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "操作时间")]
        public DateTime OperateDT { get; set; }

        [Display(Name = "备注"), StringLength(100)]
        public string Remark { get; set; }
    }

    public class UserPositionList  //5.各部门职位清单
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "部门"), StringLength(20)]
        public string Department { get; set; }

        [Display(Name = "职位名称"), StringLength(20)]
        public string Position { get; set; }

        [Display(Name = "职位代码"), StringLength(30)]
        public string PositionCode { get; set; }

        [Display(Name = "说明"), StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "备注"), StringLength(100)]
        public string Remark { get; set; }

        [Display(Name = "操作人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "操作时间")]
        public DateTime OperateDT { get; set; }
    }

    public class UserCanManagePosition  //6.可管理下属
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "部门"), StringLength(20)]
        public string Department { get; set; }

        [Display(Name = "职位名称"), StringLength(20)]
        public string Position { get; set; }

        [Display(Name = "可管理下属"), StringLength(300)]
        public string CanManagePositionCodeList { get; set; }

        [Display(Name = "操作人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "操作时间")]
        public DateTime OperateDT { get; set; }
    }

    public class UserOperateLog  //7.操作日志
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "操作人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "操作时间")]
        public DateTime OperateDT { get; set; }

        [Display(Name = "操作记录"), StringLength(2000)]
        public string OperateRecord { get; set; }
    }

    public class UserInformationTips //8.信息提示事项表清单
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "提示事项"), StringLength(260)]
        public string Matters { get; set; }

        [Display(Name = "创建人"), StringLength(50)]
        public string Creator { get; set; }

        [Display(Name = "创建时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "修改人"), StringLength(50)]
        public string Modifier { get; set; }

        [Display(Name = "修改时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }

    }

    public class UserNeedsPrompt  //9.用户需要提示事项清单表
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "员工编号")]
        public int? UserNum { get; set; }

        [Display(Name = "用户名"), StringLength(50)]
        public string UserName { get; set; }

        [Display(Name = "提示事项"), StringLength(260)]
        public string Matters { get; set; }

        [Display(Name = "待完成事项")]
        public int? Pending { get; set; }

        [Display(Name = "完成时间")]
        public DateTime? CompleteTime { get; set; }

    }

    public class UserItemEmail
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "项目名称"),StringLength(50)]
        public string ProcesName { get; set; }

        [Display(Name = "姓名"), StringLength(50)]
        public string UserName { get; set; }

        [Display(Name = "邮箱地址"), StringLength(50)]
        public string EmailAddress { get; set; }

        [Display(Name = "创建人"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime OperateDT { get; set; }
    }

    public class UserTeam//班组信息
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        //[Display(Name = "效率指标目标值")]
        //public decimal EfficiencyTarget { get; set; }

        //[Display(Name = "效率指标来源"), StringLength(50)]
        //public string EfficiencySource { get; set; }

        //[Display(Name = "品质指标目标值")]
        //public decimal QualityTarget { get; set; }

        //[Display(Name = "品质指标来源"), StringLength(50)]
        //public string QualitySource { get; set; }

        //[Display(Name = "月流失指标目标值")]
        //public decimal TurnoverRateTarget { get; set; }

        //[Display(Name = "月流失率指标来源"), StringLength(50)]
        //public string TurnoverRateSource { get; set; }

        //[Display(Name = "7S指标目标值")]
        //public decimal SevenSTarget { get; set; }

        //[Display(Name = "7S指标来源"), StringLength(50)]
        //public string  SevenSSource { get; set; }

        [Display(Name = "创建人"), StringLength(50)]
        public string Createor { get; set; }

        [Display(Name = "创建时间")]
        public DateTime Createdate { get; set; }
    }
}