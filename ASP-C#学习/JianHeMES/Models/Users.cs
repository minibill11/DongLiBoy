using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class Users
    {
        [Key]
        public int ID { get; set; }

        //[Required]
        [Display(Name ="员工编号")]
        public int UserNum { get; set; }

        //[Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        //[Required]
        [Display(Name = "密码")]
        public string PassWord { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "创建时间")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "创建人")]
        public string Creator { get; set; }

        [Display(Name = "权限")]
        public int UserAuthorize { get; set; }

        [Display(Name = "角色")]
        public string Role { get; set; }

        [Display(Name ="部门")]
        public string Department { get; set; }

        [Display(Name = "产线")]
        public int LineNum { get; set; }

        [Display(Name = "删除人")]
        public string Deleter { get; set; }

        [Display(Name = "删除时间")]
        [DataType(DataType.DateTime)]
        public DateTime? DeleteDate { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }

        public virtual List<OrderMgm> OrderMgm { get; set; }
        public virtual List<Assemble> Assemble { get; set; }
        public virtual List<BarCodes> BarCodes { get; set; }
        public virtual List<UserRole> UserRole { get; set; }
        public virtual List<Departments> Departments { get; set; }
    }

    public class UserRole
    {
        public int ID { get; set; }

        public int RoleNum { get; set; }

        public string RoleName { get; set; }

        public string RoleScription { get; set; }
    }

    public class Departments
    {
        public int ID { get; set; }
        public int DepartmentNum { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentScription { get; set; }
    }
}