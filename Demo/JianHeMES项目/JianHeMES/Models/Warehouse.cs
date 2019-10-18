﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class Warehouse_Join
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "外箱条码"), StringLength(50)]
        public string OuterBoxBarcode { get; set; }

        [Display(Name = "模组条码"), StringLength(50)]
        public string BarCodeNum { get; set; }

        [Display(Name = "模组箱体号"), StringLength(50)]
        public string ModuleGroupNum { get; set; }

        [Display(Name = "入库员"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "入库时间")]
        public DateTime? Date { get; set; }

        [Display(Name = "品质QC人员"), StringLength(20)]
        public string QC_Operator { get; set; }

        [Display(Name = "品质QC确认时间")]
        public DateTime? QC_ComfirmDate { get; set; }

        [Display(Name = "库位号"), StringLength(50)]
        public string WarehouseNum { get; set; }

        [Display(Name = "是否出库")]
        public bool IsOut { get; set; }

        [Display(Name = "出库员"), StringLength(20)]
        public string WarehouseOutOperator { get; set; }

        [Display(Name = "出库时间")]
        public DateTime? WarehouseOutDate { get; set; }

        [Display(Name = "备注"), StringLength(1000)]
        public string Remark { get; set; }

        [Display(Name = "出库次数")]
        public int WarehouseOutNum { get; set; }

        [Display(Name = "挪到订单"), StringLength(50)]
        public string NewBarcode { get; set; }
    }

    public class Production_Value
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "订单价值")]
        public decimal Worth { get; set; }

        [Display(Name = "创建时间")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "创建人"), StringLength(50)]
        public string Creator { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "修改人")]
        public DateTime? Updatetor { get; set; }

        [Display(Name = "备注"), StringLength(1000)]
        public string Remark { get; set; }
    }

    public class Warehouse_Out
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "订单号"), StringLength(50)]
        public string OrderNum { get; set; }

        [Display(Name = "外箱条码"), StringLength(50)]
        public string OuterBoxBarcode { get; set; }

        [Display(Name = "模组条码"), StringLength(50)]
        public string BarCodeNum { get; set; }

        [Display(Name = "模组箱体号"), StringLength(50)]
        public string ModuleGroupNum { get; set; }

        [Display(Name = "出库员"), StringLength(20)]
        public string Operator { get; set; }

        [Display(Name = "出库时间")]
        public DateTime? Date { get; set; }
        
        [Display(Name = "备注"), StringLength(1000)]
        public string Remark { get; set; }
    }

    public class Warehouse_BaseInfo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "仓库验收员姓名"), StringLength(50)]
        public string Username { get; set; }

        [Display(Name = "仓库验收员工号"), StringLength(50)]
        public string JobNum { get; set; }

        [Display(Name = "验收时间")]
        public DateTime? CheckTime { get; set; }

        [Display(Name = "锡膏类型"), StringLength(50)]
        public string SolderpasteType { get; set; }

        [Display(Name = "锡膏数量")]
        public int SolderpasteNum { get; set; }

        [Display(Name = "供应商"), StringLength(50)]
        public string Supplier { get; set; }

        [Display(Name = "厂家"), StringLength(50)]
        public string Manufactor { get; set; }

        [Display(Name = "生产日期")]
        public DateTime? LeaveFactoryTime { get; set; }

        [Display(Name = "出货日期")]
        public DateTime? ShipmenTime { get; set; }

        [Display(Name = "收料人姓名"), StringLength(50)]
        public string ReceivingClerk { get; set; }

        [Display(Name = "批次"), StringLength(50)]
        public string Batch { get; set; }

        [Display(Name = "料号"), StringLength(50)]
        public string ReceivingNum { get; set; }

        [Display(Name = "出库时间")]
        public DateTime? LeaveTime { get; set; }

        [Display(Name = "有效期")]
        public int EffectiveDay { get; set; }
    }

    public class Warehouse_Freezer
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "锡膏条码"), StringLength(100)]
        public string SolderpasterBacrcode { get; set; }

        [Display(Name = "入库时间")]
        public DateTime? IntoTime { get; set; }

        [Display(Name = "姓名"), StringLength(50)]
        public string UserName { get; set; }

        [Display(Name = "工号"), StringLength(50)]
        public string JobNum { get; set; }

        [Display(Name = "库位号"), StringLength(50)]
        public string WarehouseNum { get; set; }

        [Display(Name = "出库时间")]
        public DateTime? WarehouseOutTime { get; set; }

        [Display(Name = "出库人姓名"), StringLength(50)]
        public string WarehouseOutUserName { get; set; }

        [Display(Name = "出库人工号"), StringLength(50)]
        public string WarehouseOutJobNum { get; set; }

    }

    public class Warehouse_Material
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "厂商编号")]
        public int ManufactorNum { get; set; }

        [Display(Name = "厂商简称"), StringLength(50)]
        public string  ManufactorName { get; set; }

        [Display(Name = "收货单号"), StringLength(100)]
        public string InvoiceNum { get; set; }

        [Display(Name = "收货项次")]
        public int InvoiceTerm { get; set; }

        [Display(Name = "收货日期")]
        public DateTime? InvoiceDate { get; set; }

        [Display(Name = "料件编号"), StringLength(50)]
        public string MaterialNum { get; set; }

        [Display(Name = "品名"), StringLength(50)]
        public string ProductName { get; set; }

        [Display(Name = "规格"), StringLength(200)]
        public string Specifications { get; set; }

        [Display(Name = "采购单号"), StringLength(100)]
        public string PurchaseNum { get; set; }

        [Display(Name = "采购数量")]
        public decimal PurchaseCount { get; set; }

        [Display(Name = "请购单号/项次"), StringLength(100)]
        public string REQNum { get; set; }

        [Display(Name = "收货量")]
        public decimal ReceivingCount{ get; set; }

        [Display(Name = "入库单位"), StringLength(10)]
        public string WarehouseCompany { get; set; }

        [Display(Name = "入库量")]
        public decimal WarehouseCount { get; set; }

        [Display(Name = "收货金额")]
        public decimal ReceivingMoney { get; set; }

        [Display(Name = "型号"), StringLength(100)]
        public string Type { get; set; }
    }
}