using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    #region--- 钣金基本信息表
    public class MetalPlate_BasicInfo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "订单数量")]
        public Decimal Quantity { get; set; }

        [Display(Name = "生产类型"), StringLength(50)]
        public string ProductionType { get; set; }

        [Display(Name = "生产排期起始时间")]
        public DateTime? ProductScheduleStartTime { get; set; }

        [Display(Name = "生产排期部门交货时间")]
        public DateTime? DepartmentalDeliveryTime { get; set; }

        [Display(Name = "生产开始时间")]
        public DateTime? ProductionStartTime { get; set; }

        [Display(Name = "完成状态")]
        public bool? CompletionState { get; set; }

        [Display(Name = "备注"), StringLength(200)]
        public string Remark { get; set; }

    }
    #endregion


    #region 钣金生产数据表
    public class MetalPlateProduction
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "工段"), StringLength(60)]
        public string Section { get; set; }

        [Display(Name = "正常数量")]
        public Decimal NormalQuantity { get; set; }

        [Display(Name = "异常数量")]
        public Decimal AbnormaQuantity { get; set; }

        [Display(Name = "录入时间")]
        public DateTime InputTime { get; set; }

        [Display(Name = "录入人"), StringLength(20)]
        public string InputPerson { get; set; }

        [Display(Name = "部门"), StringLength(50)]
        public string Department { get; set; }

        [Display(Name = "班组"), StringLength(50)]
        public string Group { get; set; }

        [Display(Name = "生产类型"), StringLength(50)]
        public string ProductionType { get; set; }

        [Display(Name = "备注"), StringLength(200)]
        public string Remark { get; set; }

    }
    #endregion
}