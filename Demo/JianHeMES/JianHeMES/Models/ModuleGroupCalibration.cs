using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Models
{
    public class ModuleGroupCalibrationViewModel
    {
        public List<CalibrationRecord> AllCalibrationRecord;
        public SelectList OrderNumQueryList;
        public string orderNum { get; set; }

        public int Order_CR_Normal_Count { get; set; }
        public int Order_MG_Quantity { get; set; }
        public float FinishRate { get; set; }

        public IEnumerable<Users> Users;
    }
    public class OrderInformation
    {
        [Key]
        public int ID { get; set; }
        [Display(Name ="订单号")]
        public string OrderNum { get; set; }

        [Display(Name = "模组数量")]
        public int ModuleGroupQuantity { get; set; }

        [Display(Name = "订单日期")]
        [DataType(DataType.Date)]
        public DateTime? PlaceAnOrderDate { get; set; }

        [Display(Name = "交货日期")]
        [DataType(DataType.Date)]
        public DateTime? DateOfDelivery { get; set; }

        [Display(Name = "订单创建日期")]
        [DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }

    }

    public class CalibrationRecord
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "订单号")]
        public string OrderNum { get; set; }

        [Required]
        [Display(Name = "模组号")]
        public string ModuleGroupNum { get; set; }

        [Display(Name = "开始校正时间")]
        [DataType(DataType.DateTime)]
        public DateTime? BeginCalibration { get; set; }

        [Display(Name = "完成校正时间")]
        [DataType(DataType.DateTime)]
        public DateTime? FinishCalibration { get; set; }

        [Display(Name = "正常")]
        public Boolean Normal { get; set; }

        [Display(Name = "异常描述")]
        public string AbnormalDescription { get; set; }

        [Display(Name = "校正时长")]
        public TimeSpan? CalibrationTime { get; set; }
        [Display(Name = "校正用时")]
        public string CalibrationTimeSpan { get; set; }

        [Display(Name = "校正员")]
        public string Operator { get; set; }


    }
}