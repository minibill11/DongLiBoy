﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    //小样报告
    public class Small_Sample
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "报告日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReportDate { get; set; }

        [Display(Name = "客户名称"), StringLength(50)]
        public string Customer { get; set; }

        [Required]
        [Display(Name = "生产单号"), StringLength(50)]
        public string OrderNumber { get; set; }

        [Display(Name = "规格型号"), StringLength(50)]
        public string Mo1 { get; set; }

        [Display(Name = "模块数量（单位:PCS）")]
        public int Module_Number { get; set; }

        [Display(Name = "测试目的"), StringLength(200)]
        public string Te1 { get; set; }

        [Display(Name = "实际点间距")]
        public Double Actual_point { get; set; }

        [Display(Name = "测试工具"), StringLength(200)]
        public string Te2 { get; set; }

        [Display(Name = "订单接收卡"), StringLength(50)]
        public string ReceivingCard { get; set; }

        [Display(Name = "模块型号"), StringLength(50)]
        public string Mo2 { get; set; }

        [Display(Name = "点数")]
        public int Points { get; set; }

        [Display(Name = "扫描数")]
        public int Scanningway { get; set; }

        [Display(Name = "调式系统"), StringLength(50)]
        public string system { get; set; }

        [Display(Name = "测试条件"), StringLength(200)]
        public string Te3 { get; set; }

        [Display(Name = "固定值（温度）"), StringLength(50)]
        public string fixed_value { get; set; }

        [Display(Name = "驱动板电压")]
        public Decimal Driveplate { get; set; }

        [Display(Name = "电源型号"), StringLength(50)]
        public string Mo3 { get; set; }

        //样品参数实测值
        [Display(Name = "角度.颜色.R.亮度")]
        public Decimal Angle_R1 { get; set; }

        [Display(Name = "角度.颜色.R.坐标.min")]
        public Double Angle_R2 { get; set; }

        [Display(Name = "角度.颜色.R.坐标.max")]
        public Double Angle_R3 { get; set; }

        [Display(Name = "角度.颜色.G.亮度")]
        public Decimal Angle_G1 { get; set; }

        [Display(Name = "角度.颜色.G.坐标.min")]
        public Double Angle_G2 { get; set; }

        [Display(Name = "角度.颜色.G.坐标.max")]
        public Double Angle_G3 { get; set; }

        [Display(Name = "角度.颜色.B.亮度")]
        public Decimal Angle_B1 { get; set; }

        [Display(Name = "角度.颜色.B.坐标.min")]
        public Double Angle_B2 { get; set; }

        [Display(Name = "角度.颜色.B.坐标.max")]
        public Double Angle_B3 { get; set; }

        [Display(Name = "角度.颜色.W.亮度")]
        public Decimal Angle_W1 { get; set; }

        [Display(Name = "角度.颜色.W.坐标.min")]
        public Double Angle_W2 { get; set; }

        [Display(Name = "角度.颜色.W.坐标.max")]
        public Double Angle_W3 { get; set; }

        [Display(Name = "角度.客户要求.亮度.R")]
        public Decimal Client_R1 { get; set; }

        [Display(Name = "角度.客户要求.坐标.R.min")]
        public Decimal Client_R2 { get; set; }

        [Display(Name = "角度.客户要求.坐标.R.max")]
        public Decimal Client_R3 { get; set; }

        [Display(Name = "角度.客户要求.亮度.G")]
        public Decimal Client_G1 { get; set; }

        [Display(Name = "角度.客户要求.坐标.G.min")]
        public Decimal Client_G2 { get; set; }

        [Display(Name = "角度.客户要求.坐标.G.max")]
        public Decimal Client_G3 { get; set; }

        [Display(Name = "角度.客户要求.亮度.B")]
        public Decimal Client_B1 { get; set; }

        [Display(Name = "角度.客户要求.坐标.B.min")]
        public Decimal Client_B2 { get; set; }

        [Display(Name = "角度.客户要求.坐标.B.max")]
        public Decimal Client_B3 { get; set; }

        [Display(Name = "角度.客户要求.亮度.W.校正前后"), StringLength(50)]
        public string Client_W1 { get; set; }

        [Display(Name = "角度.客户要求.亮度.W.校正前后值")]
        public int Client_W1_V { get; set; }

        [Display(Name = "角度.客户要求.亮度.W.色温")]
        public int Client_W2 { get; set; }

        [Display(Name = "角度.客户要求.坐标.W.min")]
        public Double Client_W3 { get; set; }

        [Display(Name = "角度.客户要求.坐标.W.max")]
        public Double Client_W4 { get; set; }

        [Display(Name = "分压电/排阻阻值（欧）.R")]
        public int Points_R { get; set; }

        [Display(Name = "分压电/排阻阻值（欧）.G")]
        public int Points_G { get; set; }

        [Display(Name = "分压电/排阻阻值（欧）.B")]
        public int Points_B { get; set; }

        [Display(Name = "分压电/排阻阻值（欧）.W")]
        public int Points_W { get; set; }

        [Display(Name = "可调电阻阻值（欧）.R")]
        public int Adju_R { get; set; }

        [Display(Name = "可调电阻阻值（欧）.G")]
        public int Adju_G { get; set; }

        [Display(Name = "可调电阻阻值（欧）.B")]
        public int Adju_B { get; set; }

        [Display(Name = "可调电阻阻值（欧）.W")]
        public int Adju_W { get; set; }

        [Display(Name = "单IC引脚电流（mA）.R")]
        public Decimal IC_R1 { get; set; }

        [Display(Name = "单IC引脚电流（mA）.G")]
        public Decimal IC_G1 { get; set; }

        [Display(Name = "单IC引脚电流（mA）.B")]
        public Decimal IC_B1 { get; set; }

        [Display(Name = "单IC引脚电流（mA）.W")]
        public Decimal IC_W1 { get; set; }

        [Display(Name = "IC.VDS.R")]
        public Decimal IC_R2 { get; set; }

        [Display(Name = "IC.VDS.G")]
        public Decimal IC_G2 { get; set; }

        [Display(Name = "IC.VDS.B")]
        public Decimal IC_B2 { get; set; }

        [Display(Name = "IC.VDS.W")]
        public Decimal IC_W2 { get; set; }

        [Display(Name = "模块电流")]
        public Decimal Module_current { get; set; }

        [Display(Name = "IC型号"), StringLength(50)]
        public string IC3 { get; set; }

        [Display(Name = "刷新频率")]
        public int R1 { get; set; }

        [Display(Name = "接收卡Data包"), StringLength(50)]
        public string R2 { get; set; }

        [Display(Name = "接收卡带载点数1")]
        public int R3 { get; set; }

        [Display(Name = "接收卡带载点数2")]
        public int R4 { get; set; }

        [Display(Name = "LCT版本"), StringLength(50)]
        public string LCT { get; set; }

        [Display(Name = "亮度有效率（小数）")]
        public Decimal De1_dec { get; set; }

        [Display(Name = "灰度等级")]
        public int De2 { get; set; }

        [Display(Name = "DCLK")]
        public Decimal DCLK { get; set; }

        [Display(Name = "电源带载率")]
        public Double Load { get; set; }

        [Display(Name = "每平方最大功耗")]
        public int Per { get; set; }

        [Display(Name = "GCLK")]
        public Decimal GCLK { get; set; }

        [Display(Name = "MOS管型号"), StringLength(50)]
        public string MOS1 { get; set; }

        [Display(Name = "MOS压降(V)")]
        public Decimal MOS2 { get; set; }

        [Display(Name = "电流增益")]
        public Double Current { get; set; }

        //小样灯管及参数（供应商测试值)R
        [Display(Name = "LED厂家及型号.R"), StringLength(100)]
        public string LED_R1 { get; set; }

        [Display(Name = "LED厂家及型号.R.实际范围.IV.Min")]
        public Decimal LED_R2 { get; set; }

        [Display(Name = "LED厂家及型号.R.Min:Max.IV.Min")]
        public Decimal LED_R3 { get; set; }

        [Display(Name = "LED厂家及型号.R.实际范围.IV.Max")]
        public Decimal LED_R4 { get; set; }

        [Display(Name = "LED厂家及型号.R.Min:Max.IV.Max")]
        public Decimal LED_R5 { get; set; }

        [Display(Name = "LED厂家及型号.R.实际范围.WD.Min")]
        public Decimal LED_R6 { get; set; }

        [Display(Name = "LED厂家及型号.R.实际范围.WD.Max")]
        public Decimal LED_R7 { get; set; }

        [Display(Name = "LED厂家及型号.R.Min:Max.WD")]
        public Decimal LED_R8 { get; set; }

        [Display(Name = "LED厂家及型号.R.正向电压.Min")]
        public Decimal LED_R9 { get; set; }

        [Display(Name = "LED厂家及型号.R.正向电压.Max")]
        public Decimal LED_R10 { get; set; }

        [Display(Name = "LED厂家及型号.R.IV@IF")]
        public int LED_R11 { get; set; }

        //小样灯管及参数（供应商测试值)G
        [Display(Name = "LED厂家.G"), StringLength(100)]
        public string LED_G1 { get; set; }

        [Display(Name = "LED厂家.G.实际范围.IV.Min")]
        public Decimal LED_G2 { get; set; }

        [Display(Name = "LED厂家.G.Min:Max.IV.Min")]
        public Decimal LED_G3 { get; set; }

        [Display(Name = "LED厂家.G.实际范围.IV.Max")]
        public Decimal LED_G4 { get; set; }

        [Display(Name = "LED厂家.G.Min:Max.IV.Max")]
        public Decimal LED_G5 { get; set; }

        [Display(Name = "LED厂家.G.实际范围.WD.Min")]
        public Decimal LED_G6 { get; set; }

        [Display(Name = "LED厂家.G.实际范围.WD.Max")]
        public Decimal LED_G7 { get; set; }

        [Display(Name = "LED厂家.G.Min:Max.WD")]
        public Decimal LED_G8 { get; set; }

        [Display(Name = "LED厂家.G.正向电压.Min")]
        public Decimal LED_G9 { get; set; }

        [Display(Name = "LED厂家.G.正向电压.Max")]
        public Decimal LED_G10 { get; set; }

        [Display(Name = "LED厂家.G.IV@IF")]
        public int LED_G11 { get; set; }

        //小样灯管及参数（供应商测试值)B
        [Display(Name = "厂家及型号.B"), StringLength(100)]
        public string LED_B1 { get; set; }

        [Display(Name = "厂家及型号.B.实际范围.IV.Min")]
        public Decimal LED_B2 { get; set; }

        [Display(Name = "厂家及型号.B.Min:Max.IV.Min")]
        public Decimal LED_B3 { get; set; }

        [Display(Name = "厂家及型号.B.实际范围.IV.Max")]
        public Decimal LED_B4 { get; set; }

        [Display(Name = "厂家及型号.B.Min:Max.IV.Max")]
        public Decimal LED_B5 { get; set; }

        [Display(Name = "厂家及型号.B.实际范围.WD.Min")]
        public Decimal LED_B6 { get; set; }

        [Display(Name = "厂家及型号.B.实际范围.WD.Max")]
        public Decimal LED_B7 { get; set; }

        [Display(Name = "厂家及型号.B.Min:Max.WD")]
        public Decimal LED_B8 { get; set; }

        [Display(Name = "厂家及型号.B.正向电压.Min")]
        public Decimal LED_B9 { get; set; }

        [Display(Name = "厂家及型号.B.正向电压.Max")]
        public Decimal LED_B10 { get; set; }

        [Display(Name = "厂家及型号.B.IV@IF")]
        public int LED_B11 { get; set; }


        //小样灯管及参数（联建测试值）R
        [Display(Name = "LED厂家及型号.R"), StringLength(100)]
        public string Small_R1 { get; set; }

        [Display(Name = "LED厂家及型号.R.实际范围.IV.Min")]
        public Decimal Small_R2 { get; set; }

        [Display(Name = "LED厂家及型号.R.Min:Max.IV.Min")]
        public Decimal Small_R3 { get; set; }

        [Display(Name = "LED厂家及型号.R.实际范围.IV.Max")]
        public Decimal Small_R4 { get; set; }

        [Display(Name = "LED厂家及型号.R.Min:Max.IV.Max")]
        public Decimal Small_R5 { get; set; }

        [Display(Name = "LED厂家及型号.R.实际范围.WD.Min")]
        public Decimal Small_R6 { get; set; }

        [Display(Name = "LED厂家及型号.R.实际范围.WD.Max")]
        public Decimal Small_R7 { get; set; }

        [Display(Name = "LED厂家及型号.R.Min:Max.WD")]
        public Decimal Small_R8 { get; set; }

        [Display(Name = "LED厂家及型号.R.正向电压.Min")]
        public Decimal Small_R9 { get; set; }

        [Display(Name = "LED厂家及型号.R.正向电压.Max")]
        public Decimal Small_R10 { get; set; }

        [Display(Name = "LED厂家及型号.R.IV@IF")]
        public int Small_R11 { get; set; }

        //小样灯管及参数(联建测试值）G
        [Display(Name = "LED厂家.G"), StringLength(100)]
        public string Small_G1 { get; set; }

        [Display(Name = "LED厂家.G.实际范围.IV.Min")]
        public Decimal Small_G2 { get; set; }

        [Display(Name = "LED厂家.G.Min:Max.IV.Min")]
        public Decimal Small_G3 { get; set; }

        [Display(Name = "LED厂家.G.实际范围.IV.Max")]
        public Decimal Small_G4 { get; set; }

        [Display(Name = "LED厂家.G.Min:Max.IV.Max")]
        public Decimal Small_G5 { get; set; }

        [Display(Name = "LED厂家.G.实际范围.WD.Min")]
        public Decimal Small_G6 { get; set; }

        [Display(Name = "LED厂家.G.实际范围.WD.Max")]
        public Decimal Small_G7 { get; set; }

        [Display(Name = "LED厂家.G.Min:Max.WD")]
        public Decimal Small_G8 { get; set; }

        [Display(Name = "LED厂家.G.正向电压.Min")]
        public Decimal Small_G9 { get; set; }

        [Display(Name = "LED厂家.G.正向电压.Max")]
        public Decimal Small_G10 { get; set; }

        [Display(Name = "LED厂家.G.IV@IF")]
        public int Small_G11 { get; set; }

        // 小样灯管及参数（联建测试值）B
        [Display(Name = "厂家及型号.B"), StringLength(100)]
        public string Small_B1 { get; set; }

        [Display(Name = "厂家及型号.B.实际范围.IV.Min")]
        public Decimal Small_B2 { get; set; }

        [Display(Name = "厂家及型号.B.Min:Max.IV.Min")]
        public Decimal Small_B3 { get; set; }

        [Display(Name = "厂家及型号.B.实际范围.IV.Max")]
        public Decimal Small_B4 { get; set; }

        [Display(Name = "厂家及型号.B.Min:Max.IV.Max")]
        public Decimal Small_B5 { get; set; }

        [Display(Name = "厂家及型号.B.实际范围.WD.Min")]
        public Decimal Small_B6 { get; set; }

        [Display(Name = "厂家及型号.B.实际范围.WD.Max")]
        public Decimal Small_B7 { get; set; }

        [Display(Name = "厂家及型号.B.Min:Max.WD")]
        public Decimal Small_B8 { get; set; }

        [Display(Name = "厂家及型号.B.正向电压.Min")]
        public Decimal Small_B9 { get; set; }

        [Display(Name = "厂家及型号.B.正向电压.Max")]
        public Decimal Small_B10 { get; set; }

        [Display(Name = "厂家及型号.B.IV@IF")]
        public int Small_B11 { get; set; }

        [Display(Name = "报告创建人"), StringLength(50)]
        public string Creator { get; set; }

        [Display(Name = "报告创建时间")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "测试人"), StringLength(50)]
        public string Tester { get; set; }

        [Display(Name = "测试时间")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? TesterDate { get; set; }

        [Display(Name = "审核人"), StringLength(50)]
        public string Assessor { get; set; }

        [Display(Name = "报告审核时间")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? AssessedDate { get; set; }

        [Display(Name = "核准人"), StringLength(50)]
        public string Approved { get; set; }

        [Display(Name = "核准时间")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApprovedDate { get; set; }

        [Display(Name = "备注"), StringLength(300)]
        public string ReportRemark { get; set; }


        [Display(Name = "参考电压V-REXT")]
        public Decimal V_REXT { get; set; }

        [Display(Name = "IC常数N")]
        public Decimal N_Value { get; set; }

        [Display(Name = "单IC引脚电流Gain")]
        public Decimal Gain { get; set; }

    }

    public class Small_Sample_IC_Model_Arguments
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "IC型号"), StringLength(50)]
        public string IC_Model { get; set; }

        [Display(Name = "参考电压V-REXT")]
        public Decimal V_REXT { get; set; }

        [Display(Name = "IC常数N")]
        public Decimal N_Value { get; set; }

        [Display(Name = "单IC引脚电流Gain")]
        public Decimal Gain { get; set; }

        [Display(Name = "备注"), StringLength(300)]
        public string Remark { get; set; }
    }


    public class Small_Sample_ICmodel_Spacing  //模块型号和间距
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "模块型号"), StringLength(50)]
        public string Mo2 { get; set; }

        [Display(Name = "间距"), StringLength(100)]
        public string Spacing { get; set; }

        [Display(Name = "创建人"), StringLength(50)]
        public string Creator { get; set; }

        [Display(Name = "创建时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "修改人"), StringLength(50)]
        public string Modifier { get; set; }

        [Display(Name = "修改时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }

        [Display(Name = "备注"), StringLength(300)]
        public string Remark { get; set; }
    }

    public class Small_Sample_Spacing_Value //间距和最高/低值
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "间距"), StringLength(100)]
        public string Spacing { get; set; }

        [Display(Name = "最高值")]
        public Decimal Maximum { get; set; }

        [Display(Name = "最小值")]
        public Decimal Least { get; set; }

        [Display(Name = "创建人"), StringLength(50)]
        public string Creator { get; set; }

        [Display(Name = "创建时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "修改人"), StringLength(50)]
        public string Modifier { get; set; }

        [Display(Name = "修改时间"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyTime { get; set; }

        [Display(Name = "备注"), StringLength(300)]
        public string Remark { get; set; }
    }
}