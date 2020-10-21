﻿using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace JianHeMES.Controllers
{
    public class CommonERPDB
    {
        #region------ ERP表类

        public class rva_rvb_file  //按收货单号或采购单单号称查询接收类
        {
            [Display(Name = "收料员"), StringLength(40)]
            public string rvaud02 { get; set; }

            [Display(Name = "收货单号"), StringLength(20)]
            public string rva01 { get; set; }

            [Display(Name = "供应厂商"), StringLength(20)]
            public string rva05 { get; set; }

            [Display(Name = "ROHS"), StringLength(50)]
            public string MaterialType { get; set; }

            [Display(Name = "收货日期")]
            public DateTime? rva06 { get; set; }

            [Display(Name = "采购单单号"), StringLength(100)]
            public string rvb04 { get; set; }

            [Display(Name = "付款方式"), StringLength(100)]
            public string pay { get; set; }

            [Display(Name = "备注"), StringLength(1000)]
            public string rva08 { get; set; }

            [Display(Name = "项次行序"), StringLength(40)]
            public string rvb01 { get; set; }

            [Display(Name = "料件编号"), StringLength(40)]
            public string rvb05 { get; set; }

            [Display(Name = "品名规格"), StringLength(1000)]
            public string ima021 { get; set; }

            [Display(Name = "采购单号项次"), StringLength(1000)]
            public string rvb03 { get; set; }

            [Display(Name = "实收数量")] //储存该次的采购收货收货的实际数量。
            public double rvb07 { get; set; }

            [Display(Name = "单位"), StringLength(40)]
            public string pmn07 { get; set; }

            [Display(Name = "请购人"), StringLength(40)]
            public string pmk12 { get; set; }

            [Display(Name = "仓储批"), StringLength(100)]
            public string rvb36 { get; set; }

            [Display(Name = "请购单号项次"), StringLength(100)]
            public string pmn24 { get; set; }

            [Display(Name = "合同编号"), StringLength(100)]
            public string pmn122 { get; set; }

            [Display(Name = "规格"), StringLength(1000)]
            public string specification { get; set; }

            [Display(Name = "采购料件编号"), StringLength(40)]
            public string pmn041 { get; set; }

            [Display(Name = "采购料件品名"), StringLength(40)]
            public string pmn0411 { get; set; }

            [Display(Name = "采购料件规格"), StringLength(1000)]
            public string purchase_specification { get; set; }

            [Display(Name = "采购量")]
            public double pmn20 { get; set; }

            [Display(Name = "已收量")]
            public double pmn50_pmn55 { get; set; }

            [Display(Name = "入库量")]
            public double rvb30 { get; set; }

            [Display(Name = "可入库量")]
            public double rvb31 { get; set; }

            [Display(Name = "退货量")]
            public double rvb29 { get; set; }

            [Display(Name = "样品否")]
            public bool rvb35 { get; set; }

            [Display(Name = "仓库"),StringLength(40)]
            public string rvb366 { get; set; }

            [Display(Name = "库位"), StringLength(40)]
            public string rvb377 { get; set; }

            [Display(Name = "批号"), StringLength(60)]
            public string rvb388 { get; set; }

            //[Display(Name = "收货数量")] //储存该次的采购收货收货单上的注明数量。
            //public double rvb08 { get; set; }

            //[Display(Name = "发票编号"), StringLength(16)]
            //public string rvb22 { get; set; }

            //[Display(Name = "收货单项次"), StringLength(40)]
            //public string rvb02 { get; set; }
         }

        public class img_file
        {
            //[Key]
            //public int Id { get; set; }

            [Display(Name = "收料员"), StringLength(40)]
            public string rvaud02 { get; set; }

            [Display(Name = "供应厂商"), StringLength(20)]
            public string rva05 { get; set; }

            [Display(Name = "ROHS"), StringLength(50)]
            public string MaterialType { get; set; }

            [Display(Name = "料件编号"), StringLength(50)]
            public string img01 { get; set; }

            [Display(Name = "物料品名"), StringLength(100)]
            public string ima02 { get; set; }

            [Display(Name = "物料规格"), StringLength(1000)]
            public string ima021 { get; set; }

            [Display(Name = "仓库编号"), StringLength(50)]
            public string img02 { get; set; }

            [Display(Name = "储位"), StringLength(50)]
            public string img03 { get; set; }

            [Display(Name = "批号"), StringLength(50)]
            public string img04 { get; set; }

            [Display(Name = "参考号码"), StringLength(50)]
            public string img05 { get; set; }

            [Display(Name = "参考序号"), StringLength(50)]
            public int? img06 { get; set; }

            [Display(Name = "采购单位/生产单位"), StringLength(50)]
            public string img07 { get; set; }

            [Display(Name = "收货数量"), StringLength(50)]
            public double? img08 { get; set; }

            [Display(Name = "库存单位"), StringLength(50)]
            public string img09 { get; set; }

            [Display(Name = "库存数量"), StringLength(50)]
            public double? img10 { get; set; }

            [Display(Name = "No Use"), StringLength(50)]
            public double? img11 { get; set; }

            [Display(Name = "No Use"), StringLength(50)]
            public double? img12 { get; set; }

            [Display(Name = "制造日期"), StringLength(50)]
            public DateTime? img13 { get; set; }

            [Display(Name = "最近一次盘点日期"), StringLength(50)]
            public DateTime? img14 { get; set; }

            [Display(Name = "最近一次收料日期"), StringLength(50)]
            public DateTime? img15 { get; set; }

            [Display(Name = "最近一次发料日期"), StringLength(50)]
            public DateTime? img16 { get; set; }

            [Display(Name = "最近一次异动日期"), StringLength(50)]
            public DateTime? img17 { get; set; }

            [Display(Name = "有效日期"), StringLength(50)]
            public DateTime? img18 { get; set; }

            [Display(Name = "库存等级"), StringLength(50)]
            public string img19 { get; set; }

            [Display(Name = "单位数量换算率"), StringLength(50)]
            public double? img20 { get; set; }

            [Display(Name = "单位数量换算率-对料件库存单"), StringLength(50)]
            public double? img21 { get; set; }

            [Display(Name = "仓储类别"), StringLength(50)]
            public string img22 { get; set; }

            [Display(Name = "是否为可用仓储"), StringLength(50)]
            public string img23 { get; set; }

            [Display(Name = "是否为ＭＲＰ可用仓储"), StringLength(50)]
            public string img24 { get; set; }

            [Display(Name = "保税与否"), StringLength(50)]
            public string img25 { get; set; }

            [Display(Name = "仓储所属会计科目"), StringLength(50)]
            public string img26 { get; set; }

            [Display(Name = "工单发料优先顺序"), StringLength(50)]
            public int? img27 { get; set; }

            [Display(Name = "销售出货优先顺序"), StringLength(50)]
            public int? img28 { get; set; }

            //[Display(Name = "未知"), StringLength(50)]
            //public string img29 { get; set; }

            [Display(Name = "直接材料成本"), StringLength(50)]
            public double? img30 { get; set; }

            [Display(Name = "间接材料成本"), StringLength(50)]
            public double? img31 { get; set; }

            [Display(Name = "厂外加工材料成本"), StringLength(50)]
            public double? img32 { get; set; }

            [Display(Name = "厂外加工人工成本"), StringLength(50)]
            public double? img33 { get; set; }

            [Display(Name = "库存单位对成本单位的转换率"), StringLength(50)]
            public double? img34 { get; set; }

            [Display(Name = "专案号码"), StringLength(50)]
            public string img35 { get; set; }

            [Display(Name = "外观编号"), StringLength(50)]
            public string img36 { get; set; }

            [Display(Name = "呆滞日期"), StringLength(50)]
            public DateTime? img37 { get; set; }

            [Display(Name = "备注"), StringLength(50)]
            public string img38 { get; set; }

            //[Display(Name = "所属工厂"), StringLength(50)]
            //public string imgplant { get; set; }

            //[Display(Name = "所属法人"), StringLength(50)]
            //public string imglegal { get; set; }

        }

        public class ima_file
        {
            [Display(Name = "物料编号"), StringLength(50)]
            public string ima01 { get; set; }

            [Display(Name = "物料品名"), StringLength(100)]
            public string ima02 { get; set; }

            [Display(Name = "物料规格"), StringLength(1000)]
            public string ima021 { get; set; }

        }

        public class cxcr006  //tc_cxc_file  年：tc_cxc15、月：tc_cxc16
        {
            [Display(Name = "料号"),StringLength(50)]
            public string tc_cxc01 { get; set; }

            [Display(Name = "品名"), StringLength(120)]
            public string tc_cxc02 { get; set; }

            [Display(Name = "规格"), StringLength(1000)]
            public string tc_cxc03 { get; set; }

            [Display(Name = "面积")]
            public double area { get; set; }

            [Display(Name = "本月结存数")]
            public double tc_cxc04 { get; set; }

            [Display(Name = "本月结存单价")]
            public double tc_cxc05 { get; set; }

            [Display(Name = "本月结存金额")]
            public double tc_cxc06 { get; set; }

            [Display(Name = "30天")]
            public double tc_cxc07 { get; set; }

            [Display(Name = "90天")]
            public double tc_cxc08 { get; set; }

            [Display(Name = "180天")]
            public double tc_cxc09 { get; set; }

            [Display(Name = "365天")]
            public double tc_cxc10 { get; set; }

            [Display(Name = "2年")]
            public double tc_cxc11 { get; set; }

            [Display(Name = "3年")]
            public double tc_cxc12 { get; set; }

            [Display(Name = "3-5年")]
            public double tc_cxc13 { get; set; }

            [Display(Name = "5年以上")]
            public double tc_cxc14 { get; set; }

            [Display(Name = "年度")]
            public int tc_cxc15 { get; set; }

            [Display(Name = "月份")]
            public int tc_cxc16 { get; set; }

            [Display(Name = "会计科目")]
            public int AccountingSubject { get; set; }

            [Display(Name = "库位号")]
            public string WarehouseNumber { get; set; }

            [Display(Name = "分类明细")]
            public string Classification { get; set; }

        }

        public class cxcr006_raw_material
        {
            [Display(Name = "料号"), StringLength(50)]
            public string tc_cxc01 { get; set; }

            [Display(Name = "品名"), StringLength(120)]
            public string tc_cxc02 { get; set; }

            [Display(Name = "规格"), StringLength(1000)]
            public string tc_cxc03 { get; set; }

            [Display(Name = "面积")]
            public double area { get; set; }

            [Display(Name = "本月结存数")]
            public double tc_cxc04 { get; set; }

            [Display(Name = "本月结存单价")]
            public double tc_cxc05 { get; set; }

            [Display(Name = "本月结存金额")]
            public double tc_cxc06 { get; set; }

            [Display(Name = "30天")]
            public double tc_cxc07 { get; set; }

            [Display(Name = "90天")]
            public double tc_cxc08 { get; set; }

            [Display(Name = "180天")]
            public double tc_cxc09 { get; set; }

            [Display(Name = "365天")]
            public double tc_cxc10 { get; set; }

            [Display(Name = "2年")]
            public double tc_cxc11 { get; set; }

            [Display(Name = "3年")]
            public double tc_cxc12 { get; set; }

            [Display(Name = "3-5年")]
            public double tc_cxc13 { get; set; }

            [Display(Name = "5年以上")]
            public double tc_cxc14 { get; set; }

            [Display(Name = "会计科目")]
            public int AccountingSubject { get; set; }

            [Display(Name = "库位号")]
            public string WarehouseNumber { get; set; }

            [Display(Name = "分类明细")]
            public string Classification { get; set; }

            [Display(Name = "工单号"), StringLength(20)]
            public string sfb01 { get; set; }     //工单号

            [Display(Name = "工单备注"), StringLength(1000)]
            public string sfbud01 { get; set; }   //工单备注

            [Display(Name = "工单录入日期")]
            public DateTime sfb81 { get; set; }   //工单录入日期

            [Display(Name = "单位"), StringLength(4)]
            public string sfa12 { get; set; }     //单位

            [Display(Name = "应发数量")]
            public double sfa05 { get; set; }     //应发数量

            [Display(Name = "已发数量")]
            public double sfa06 { get; set; }     //已发数量

            [Display(Name = "未发数量")]
            public double sfa05_sfa06 { get; set; }     //未发数量

            [Display(Name = "年度")]
            public int tc_cxd15 { get; set; }

            [Display(Name = "月份")]
            public int tc_cxd16 { get; set; }

        }

        public class csfr008  //sfb_file,sfa_file,ima_file
        {
            [Display(Name = "工单号"), StringLength(20)]
            public string sfb01 { get; set; }     //工单号

            [Display(Name = "工单备注"), StringLength(1000)]
            public string sfbud01 { get; set; }   //工单备注
            
            [Display(Name = "工单录入日期")]
            public DateTime sfb81 { get; set; }   //工单录入日期

            [Display(Name = "生产数量")]      
            public double sfb08 { get; set; }    //生产数量

            [Display(Name = "入库数量")]
            public double sfb09 { get; set; }    //入库数量

            [Display(Name = "料号"), StringLength(40)]
            public string sfa03 { get; set; }    //料号

            [Display(Name = "品名"), StringLength(120)]
            public string ima02 { get; set; }     //品名

            [Display(Name = "规格"), StringLength(120)]
            public string ima021 { get; set; }    //规格

            [Display(Name = "单位"), StringLength(4)]
            public string sfa12 { get; set; }     //单位

            [Display(Name = "应发数量")]
            public double sfa05 { get; set; }     //应发数量

            [Display(Name = "已发数量")]
            public double sfa06 { get; set; }     //已发数量

            [Display(Name = "未发数量")]
            public double sfa05_sfa06  { get; set; }     //未发数量
    }

        public class tc_cxd_file  //固定表存储MC未发料月末记录  年：tc_cxd14、月：tc_cxd15
        {
            [Display(Name = "工单号"), StringLength(20)]
            public string tc_cxd01 { get; set; }    // varchar2(20)      /*工单号*/

            [Display(Name = "工单备注"), StringLength(1000)]
            public string tc_cxd02  { get; set; }  // varchar2(255)      /*工单备注*/

            [Display(Name = "日期")]
            public DateTime? tc_cxd03  { get; set; }  // date               /*日期*/

            [Display(Name = "生产数量")]
            public double tc_cxd04  { get; set; }  // number(15,3)       /*生产数量*/

            [Display(Name = "完工入库数量")]
            public double tc_cxd05  { get; set; }  // number(15,3)       /*完工入库数量*/

            [Display(Name = "料件编号"), StringLength(30)]
            public string tc_cxd06  { get; set; }  // varchar2(30)       /*料件编号*/

            [Display(Name = "品名"), StringLength(120)]
            public string tc_cxd07  { get; set; }  // varchar2(120)      /*品名*/

            [Display(Name = "规格"), StringLength(120)]
            public string tc_cxd08  { get; set; }  // varchar2(120)      /*规格*/

            [Display(Name = "发料单位"), StringLength(4)]
            public string tc_cxd09  { get; set; }  // varchar2(4)        /*发料单位*/

            [Display(Name = "应发数量")]
            public double tc_cxd10  { get; set; }  // number(15,3)       /*应发数量*/

            [Display(Name = "已发数量")]
            public double tc_cxd11  { get; set; }  // number(15,3)       /*已发数量*/

            [Display(Name = "未发数量")]
            public double tc_cxd10_tc_cxd11 { get; set; }  //未发料数量

            [Display(Name = "年度")]
            public int tc_cxd14 { get; set; }

            [Display(Name = "月份")]
            public int tc_cxd15 { get; set; }

            [Display(Name = "订单类型"), StringLength(30)]
            public string ordertype { get; set; }
        }

        public class imk_file
        {
            [Display(Name = "料件编号"), StringLength(40)]
            public string imk01    { get; set; }    //archar2(40) NOT NULL,   /*料件编号  */

            [Display(Name = "仓库编号"), StringLength(10)]
            public string imk02    { get; set; }    //archar2(10) NOT NULL,   /*仓库编号 */

            [Display(Name = "储位编号"), StringLength(10)]
            public string imk03    { get; set; }    //archar2(10) NOT NULL,   /*储位编号*/

            [Display(Name = "批号"), StringLength(24)]
            public string imk04    { get; set; }    //varchar2(24) NOT NULL,   /*批号*/

            [Display(Name = "年度")]
            public int imk05       { get; set; }    //number(5) NOT NULL,      /*年度*//*年度yyyy  */

            [Display(Name = "期别")]
            public int imk06       { get; set; }    //number(5) NOT NULL,      /*期别,月份*/

            [Display(Name = "本期入库统计量")]
            public double imk081   { get; set; }    //number(15,3),            /*本期入库统计量*/

            [Display(Name = "本期销货统计量")]
            public double imk082   { get; set; }    //number(15,3),            /*本期销货统计量*/

            [Display(Name = "本期领用统计量")]
            public double imk083   { get; set; }    //number(15,3),            /*本期领用统计量*/

            [Display(Name = "本期转拨统计量")]
            public double imk084   { get; set; }    //number(15,3),            /*本期转拨统计量*/

            [Display(Name = "本期调整统计量")]
            public double imk085   { get; set; }    //number(15,3),            /*本期调整统计量*/

            [Display(Name = "No Use")]
            public double imk086   { get; set; }    //number(15,3),            /*No Use*/

            [Display(Name = "No Use")]
            public int imk087      { get; set; }    //number(5),               /*No Use*/

            [Display(Name = "No Use")]
            public int imk088      { get; set; }    //number(5),               /*No Use*/

            [Display(Name = "No Use")]
            public int imk089      { get; set; }    //number(5),               /*No Use*/

            [Display(Name = "期末数量")]
            public double imk09    { get; set; }    //number(15,3),            /*期末数量*/

            [Display(Name = "所属工厂")]
            public string imkplant { get; set; }    //varchar2(10),            /*所属工厂*/

            [Display(Name = "所属法人")]
            public string imklegal { get; set; }    //varchar2(10)             /*所属法人*/

            [Display(Name = "期末余额")]
            public double sum { get; set; }    //             /*期末余额*/

        }


        //在制品查询表
        public class sfb_file
        {
            [Display(Name = "工单编号"), StringLength(20)]
            public string sfb01 { get; set; }    /*工单编号*/

            [Display(Name = "工单型态")]
            public int sfb02 { get; set; }       /*工单型态*/ /*储存该工单所属类别型态*/ /*正确值 1/2/5/7/11/12/13/15 */ /* 1: 一般工单  */  /* 5: 再加工工单*//* 7: 委外工单  */ /* 8: 重工委外工单   #Add By Snow */ /*11: 拆件式工单*/ /*13: 预测工单  *//*15: 试产工单  */

            [Display(Name = "在制品会计科目"), StringLength(24)]
            public string sfb03 { get; set; }    /*在制品会计科目*/
                                                 /*在制品会计科目  aag01 */
            [Display(Name = "工单状态"), StringLength(24)]
            public string sfb04 { get; set; }           /*工单状态 */
                                                        /*储存该工单目前处理阶段状况*/
                                                        /*正确值 1/2/3/4/5/6/7/8    */
                                                        /*1: 确认生产工单(firm plan)*/
                                                        /*2: 工单已发放,料表尚未列印*/
                                                        /*3: 工单已发放,料表已列印  */
                                                        /*4: 工单已发料             */
                                                        /*5: 在制过程中             */
                                                        /*6: 工单已完工,进入F.Q.C   */
                                                        /*7: 完工入库               */
                                                        /*8: 结案                   */

            [Display(Name = "料件编号"), StringLength(40)]
            public string sfb05 { get; set; }         /*料件编号                               */
                                                      /*料件编号   ima01                       */
                                                      /*储存该工单将投入生产料件               */

            [Display(Name = "使用制程编号"), StringLength(10)]
            public string sfb06 { get; set; }   // varchar2(10), /*使用制程编号*/
                                                /*使用制程编号  ecu01                    */
                                                /*储存该工单将投入生产料件时所用的制程编 */
                                                /*号                                     */
            [Display(Name = "版本"), StringLength(10)]
            public string sfb07 { get; set; } //varchar2(10), /*版本*/
                                              /*储存工单投入生产的料件版本             */

            [Display(Name = "产品结构指定有效日期")]
            public DateTime sfb071 { get; set; } //date, /*产品结构指定有效日期 */
                                                 /*储存工单投入生产的料件使用的产品结构指 */
                                                 /*定有效日期              
                                                  * */
            [Display(Name = "生产数量")]
            public double sfb08 { get; set; }   // number(15,3) NOT NULL,   /*生产数量                               */
                                                /*预计投入生产的数量，单位为料件生产单位 */

            [Display(Name = "已发料套数")]
            public double sfb081 { get; set; }     // number(15,3) NOT NULL,   /*已发料套数                             */


            [Display(Name = "完工入库数量")]
            public double sfb09 { get; set; } //      number(15,3) NOT NULL,   /*完工入库数量                           */
                                              /*储存该工单已完工入库数量               */

            [Display(Name = "再加工数量")]
            public double sfb10 { get; set; }     //number(15,3) NOT NULL,   /*再加工数量                             */
                                                  /*储存该工单再加工数量                   */

            [Display(Name = "F.Q.C 数量")]
            public double sfb11 { get; set; }      //number(15,3) NOT NULL,   /*F.Q.C 数量                             */
                                                   /*储存该工单在全检（检验）数量           */

            [Display(Name = "No Use")]
            public double sfb111 { get; set; }//number(15,3) NOT NULL,   /*No Use                                 */

            [Display(Name = "报废数量")]
            public double sfb12 { get; set; }      //number(15,3) NOT NULL,   /*报废数量                               */
                                                   /*储存该工单报废数量                     */

            [Display(Name = "在制盘盈亏量")]
            public double sfb121 { get; set; }     //number(15,3),            /*在制盘盈亏量                           */
                                                   /*在制盘盈亏量  add 99/04/27             */

            [Display(Name = "No Use")]
            public double sfb122 { get; set; } //number(15,3),            /*No Use                                 */

            [Display(Name = "预计起始生产日期")]
            public DateTime sfb13 { get; set; }  //date,                    /*预计起始生产日期                       */
                                                 /*储存该工单预计投入生产日期             */

            [Display(Name = "预计起始生产时间"),StringLength(5)]
            public string sfb14 { get; set; }     //varchar2(5),             /*预计起始生产时间                       */
                                                  /*预计起始生产时间(时:分)                */
                                                  /*储存该工单预计投入生产时间             */

            [Display(Name = "预计结束生产日期")]
            public DateTime sfb15 { get; set; } //date,                    /*预计结束生产日期                       */
                                                /*储存该工单预计完成生产日期             */

            [Display(Name = "预计结束生产时间")]
            public string sfb16 { get; set; }     //varchar2(5),             /*预计结束生产时间                       */
                                                  /*预计结束生产时间(时:分)                */
                                                  /*储存该工单预计完成生产时间             */

            [Display(Name = "已完工制程序号")]
            public int sfb17 { get; set; } //number(5),               /*已完工制程序号                         */
                                           /*储存该工单目前已完工的最大制程序号     */

            [Display(Name = "最近一次作业完工日期")]
            public DateTime sfb18 { get; set; } // date,                    /*最近一次作业完工日期                   */
                                                /*最近一次作业完工日期(天)               */
                                                /*储存该工单 [已完工制程序号] 之完工日期 */

            [Display(Name = "最近一次作业完工时间"),StringLength(5)]
            public string sfb19 { get; set; } //      varchar2(5),             /*最近一次作业完工时间                   */
                                              /*最近一次作业完工时间(时:分)            */
                                              /*储存该工单 [已完工制程序号] 之完工时间 */
                                              /*量产系统专用                           */

            [Display(Name = "MPS/MRP 需求日期")]
            public DateTime sfb20 { get; set; } //date,                    /*MPS/MRP 需求日期                       */
                                                /*储存该工单之预计 MPS/MPR 需求日期      */

            [Display(Name = "MPS/MRP 需求日期"),StringLength(5)]
            public string sfb21 { get; set; }      //varchar2(5),             /*MPS/MRP 需求时间                       */
                                                   /*MPS/MRP 需求时间(时:分)                */
                                                   /*储存该工单之预计 MPS/MPR 需求时间      */
                                                   /*量产系统专用                           */

            [Display(Name = "订单编号/预测工单编号"), StringLength(20)]
            public string sfb22 { get; set; }  //varchar2(20),            /*订单编号/预测工单编号                  */
                                               /*储存该工单之指定生产供给的订单单号     */

            [Display(Name = "订单项次/预测工单项次")]
            public int sfb221 { get; set; }  //number(5),               /*订单项次/预测工单项次                  */
                                             /*储存该工单之指定生产供给的订单项次     */

            [Display(Name = "APS 单据编号"), StringLength(20)]
            public string sfb222 { get; set; }  //varchar2(20),            /*APS 单据编号                           */
                                                /*APS 单据编号  no.4651 02/03/14(modify) */

            [Display(Name = "备料档产生否"), StringLength(1)]
            public string sfb23 { get; set; }  //varchar2(1),             /*备料档产生否                           */
                                               /*储存该工单是否已产生备料资料           */
                                               /*正确值 Y/N                             */
                                               /*Y: 该工单已产生备料资料                */
                                               /*N: 该工单尚未产生备料资料              */

            [Display(Name = "制程追踪档产生否"), StringLength(1)]
            public string sfb24 { get; set; }  //varchar2(1),             /*制程追踪档产生否                       */
                                               /*储存该工单是否已产生制程追踪资料       */
                                               /*正确值 Y/N                             */
                                               /*Y: 该工单已产生制程追踪资料            */
                                               /*N: 该工单尚未产生制程追踪资料          */

            [Display(Name = "实际开工日")]
            public DateTime sfb25 { get; set; }  //date,                    /*实际开工日                             */
                                                 /*储存该工单第一次发料日期, 于发料作业更新*/

            [Display(Name = "预计发放日期")]
            public DateTime sfb251 { get; set; }  //      date,                    /*预计发放日期                           */


            [Display(Name = "发料日期"), StringLength(5)]
            public string sfb26 { get; set; }  //varchar2(5),             /*发料日期  */

            [Display(Name = "专案号码"), StringLength(10)]
            public string sfb27 { get; set; }  //varchar2(10),            /*专案号码            */
                                               /*储存该工单所属专案号码                 */

            [Display(Name = "WBS编码"), StringLength(30)]
            public string sfb271 { get; set; } //varchar2(30),            /*WBS编码             */
                                               /*专案号码-顺序                          */
                                               /*储存该工单所属专案号码-顺序            */

            [Display(Name = "工单结案状态"), StringLength(1)]
            public string sfb28 { get; set; }//varchar2(1),             /*工单结案状态        */
                                             /*正确值 1/2/3                           */
                                             /*1: 发料已结束,不能再发料               */
                                             /*2: 生产工时已结束,不能再发料及输入工时 */
                                             /*3: 成本会计已结束,此工单不能再重新开启 */
                                             /*   ，可转入历史档                      */

            [Display(Name = "可用否"), StringLength(1)]
            public string sfb29 { get; set; } //varchar2(1),             /*可用否                                 */
                                              /*储存执行主排程计划或料需求计划时，是否 */
                                              /*可考虑将该工单完工数量列入需求之供给   */
                                              /*正确值 Y/N                             */
                                              /*Y: 可考虑为供给                        */
                                              /*N: 不可考虑为供给                      */

            [Display(Name = "预计工单完工入库仓库别"), StringLength(10)]
            public string sfb30 { get; set; } //varchar2(10),            /*预计工单完工入库仓库别                 */
                                              /*储存该工单完工后，预计存放的仓库别     */

            [Display(Name = "预计工单完工入库储位"), StringLength(10)]
            public string sfb31 { get; set; }  //varchar2(10),            /*预计工单完工入库储位                   */
                                               /*储存该工单完工后，预计存放的储位       */

            [Display(Name = "起始排程方法"), StringLength(2)]
            public string sfb32 { get; set; }      //varchar2(2),             /*起始排程方法                           */
                                                   /*正确值 1/2/3/4/5                       */
                                                   /*1: window scheduling forward           */
                                                   /*2: window scheduling backward          */
                                                   /*3: weighted window scheduling          */
                                                   /*4: tracking scheduling forward         */
                                                   /*5: tracking scheduling backward        */

            [Display(Name = "最近排程方法"), StringLength(2)]
            public string sfb33 { get; set; }   //varchar2(2),             /*最近排程方法                           */
                                                /*正确值 1/2/3/4/5                       */
                                                /*1: window scheduling forward           */
                                                /*2: window scheduling backward          */
                                                /*3: weighted window scheduling          */
                                                /*4: tracking scheduling forward         */
                                                /*5: tracking scheduling backward        */

            [Display(Name = "C/R 比率")]
            public double sfb34 { get; set; } //number(9,4),             /*C/R 比率                               */
                                              /*C/R 比率(%)                            */
                                              /*储存该工单紧急比率                     */
                                              /*开始时应设定为 '1'，可透过工单紧急比率 */
                                              /*重新计算作业重新设定                   */
                                              /*其值愈小者，赋予愈高的分派优先顺序     */

            [Display(Name = "最近排程方法"), StringLength(1)]
            public string sfb35 { get; set; }//  varchar2(1),             /*异动码                                 */
                                             /*异动码(Y/N)                            */
                                             /*储存该工单自上次料需求计划后，如有变动 */
                                             /*时，则需在此备注                       */
                                             /*提供 nettable change MRP 使用          */

            public DateTime sfb36 { get; set; }   //date,                    /*工单发料结束日期                       */
                                                  /*按工单结案状况，会有各阶段结束日期；而 */
                                                  /*本栏位记录料帐结束日期，即在工单经过此 */
                                                  /*阶段后，系统即便不可再发料给该工单     */
                                                  /*如需再发料，则需作工单的重新开启作业， */
                                                  /*才被允许可再发料给该工单               */

            public DateTime sfb37 { get; set; }//   date,                    /*工单发料及工时结束日                   */
                                               /*按工单结案状况，会有各阶段结束日期；而 */
                                               /*本栏位记录料帐及人工帐（工时）结束日期 */
                                               /*，简而言之即在工单经过此阶段后，系统即 */
                                               /*便不可再对该工单收集工时               */
                                               /*如需再收集工时，则需作工单的重新开启作 */
                                               /*业，才被允许可再收集工时给该工单       */

            public DateTime sfb38  { get; set; }//    date,                    /*工单成本会计结束日期                   */
                                                /*按工单结案状况，会有各阶段结束日期；而 */
                                                /*本栏位记录该工单已在最后确定的阶段日期 */
                                                /*，简而言之即在工单经过此阶段后，系统即 */
                                                /*便不可再对该工单发料及收集工时，而且亦 */
                                                /*无法再重新开启                         */

            public string sfb39 { get; set; }//  varchar2(1),             /*完工方式                               */
                                             /*储存该工单在料帐上，使用捡料及发料系统 */
                                             /*或使用领料及事后扣帐系统               */
                                             /*正确值 1/2                             */
                                             /*1: 捡料及发料系统                      */
                                             /*   Picking List 与 Push System         */
                                             /*2: 领料及事后扣帐系统                  */
                                             /*   Pull List 与 Backflush System       */

            public int sfb40  { get; set; }//number(5),               /*优先顺序                               */
                                             /*储存该工单派工的优先顺序               */
                                             /*正确值 应不小于零                      */
                                             /*其值愈小者顺序愈高                     */
            public string sfb41 { get; set; }  //varchar2(1),             /*冻结码                                 */
                                           /*冻结码(frozen flag)                    */
                                           /*储存该工单为需求之供给时，是否可更动其 */
                                           /*时程，以利需求之取得                   */
                                           /*正确值 Y/N                             */
                                           /*Y: 时程已被固定之工单                  */
                                           /*N: 时程尚未被固定之工单                */
            public int sfb42  { get; set; }//number(5),               /*工单展开阶数                           */
                                             /*储存该工单对下阶自动产生工单的往下阶数 */
            public DateTime sfb81  { get; set; }//date,                    /*输入日期                               */
                                       /*储存工单输入日期                       */
            public string sfb82 { get; set; }//varchar2(10),            /*制造部门/委外厂商                      */
            public string sfb85 { get; set; }//varchar2(20),            /*PBI NO(Picking Batch ID)               */
            public string sfb86 { get; set; }//varchar2(20),            /*母工单号码                             */
            public string sfb87 { get; set; }//varchar2(1),             /*确认否                                 */
                                             /*确认否(Y/N/X)                          */
            public string sfb88 { get; set; }//varchar2(20),            /*料表编号                               */
            public string sfb91 { get; set; } // varchar2(20),            /*制造通知单                             */
            public string sfb92 { get; set; } // number(5),               /*项次                                   */
            public string sfb93 { get; set; } // varchar2(1),             /*制程否                                 */
            public string sfb94 { get; set; } // varchar2(1),             /*FQC否                                  */
            public string sfb95 { get; set; } // varchar2(20),            /*特性代码                               */
            public string sfb96 { get; set; } // varchar2(255),           /*备注                                   */
            public string sfb97 { get; set; } // varchar2(20),            /*手册编号                               */
                                 //                  /*手册编号   (A050)                      */
            public string sfb98 { get; set; } // varchar2(10),            /*成本中心                               */
            public string sfb99 { get; set; } //varchar2(1),             /*重工否                                 */
                                             /*重工否(Y/N)                            */
            public string sfb100 { get; set; }//varchar2(1),             /*委外型态                               */
                                          /*委外型态                        养生计划*/
                                          /*1.委外工单对委外采购单型态为一对一     */
                                          /*2.委外工单对委外采购单型态为一对多     */
                                          /*预设 '1'                               */
                                          /*两者的差别在于,资料的一致性            */
            public int sfb101 { get; set; }//number(5),               /*变更序号                               */
                                             /*变更序号                        养生计划*/
            public string sfbacti { get; set; }// varchar2(1),             /*资料有效码                             */
                                               /*系统维护                               */
            public string sfbuser { get; set; }// varchar2(10),            /*资料所有者                             */
                                               //  /*系统维护                               */
            public string sfbgrup { get; set; }// varchar2(10),            /*资料所有群                             */
                                                  /*系统维护                               */
            public string sfbmodu { get; set; } //varchar2(10),            /*资料更改者                             */
                                                  /*系统维护                               */
            public DateTime sfbdate { get; set; }//date,                    /*最近修改日                             */
                                             /*系统维护                               */
            public string sfb1001 { get; set; }    // varchar2(40),            /*保税核准文号                           */
            public string sfb1002 { get; set; }//varchar2(1),             /*保税核销否                             */
            public string sfb1003 { get; set; }//varchar2(20),            /*保税放行单号                           */
            public string sfb102 { get; set; }//varchar2(10),            /*生产线                                 */
            public string sfb50 { get; set; }//varchar2(4),             /*活动代号                               */
            public string sfb51 { get; set; }//varchar2(10),            /*理由码                                 */
            public int sfb103 { get; set; }//number(5),               /*工单制程变更序号                       */
            public string sfbud01 { get; set; }//varchar2(255),           /*自订栏位-Textedit                      */
            public string sfbud02 { get; set; }//varchar2(40),            /*自订栏位-文字                          */
            public string sfbud03  { get; set; }//varchar2(40),            /*自订栏位-文字                          */
            public string sfbud04 { get; set; }// varchar2(40),            /*自订栏位-文字                          */
            public string sfbud05 { get; set; }// varchar2(40),            /*自订栏位-文字                          */
            public string sfbud06 { get; set; }//varchar2(40),            /*自订栏位-文字                          */
            public double sfbud07 { get; set; }//number(15,3),            /*自订栏位-数值                          */
            public double sfbud08 { get; set; }//number(15,3),            /*自订栏位-数值                          */
            public double sfbud09 { get; set; }//number(15,3),            /*自订栏位-数值                          */
            public int sfbud10 { get; set; }// number(10),              /*自订栏位-整数                          */
            public int sfbud11 { get; set; }//number(10),              /*自订栏位-整数                          */
            public int sfbud12 { get; set; }//number(10),              /*自订栏位-整数                          */
            public DateTime sfbud13 { get; set; }//date,                    /*自订栏位-日期                          */
            public DateTime sfbud14 { get; set; }//date,                    /*自订栏位-日期                          */
            public DateTime sfbud15 { get; set; }//date,                    /*自订栏位-日期                          */
            public string sfb43 { get; set; }//varchar2(1),             /*签核状况                               */
            public string sfb44 { get; set; }//varchar2(10),            /*申请人                                 */
            public string sfbmksg { get; set; }//varchar2(1),             /*是否签核                               */
            public string sfbplant { get; set; }//varchar2(10),            /*所属工厂                               */
            public string sfblegal { get; set; }//varchar2(10),            /*所属法人                               */
            public string sfboriu { get; set; }//varchar2(10),            /*资料建立者                             */
            public string sfborig { get; set; }//varchar2(10),            /*资料建立部门                           */
            public string sfb104 { get; set; }//varchar2(1)  /*备置否(Y/N)*/

        }

        public class ccg_file
        {
            [Display(Name = "工单单号重覆性生产产品成本"), StringLength(40)]
            public string ccg01 { get; set; }   //varchar2(40) NOT NULL, /*工单单号重覆性生产产品成本/*重覆性生产产品成本此栏是生产料号*/

            [Display(Name = "主件料号"), StringLength(40)]
            public string ccg04 { get; set; }   //         varchar2(40) NOT NULL,   /*主件料号*/
            //public double ccg11 { get; set; }   //         number(15,3) NOT NULL,   /*上月结存数量*/
            //public double ccg12   { get; set; } //   number(20,6) NOT NULL,   /*上月结存金额(a+b+c+d+e)*/
            public string durations { get; set; } //期间
            public int ccg01_yearmonth { get; set; }  //工单年月份
            public int ccg02 { get; set; }     //       number(5) NOT NULL,      /*年度*/
            public int ccg03 { get; set; }      //      number(5) NOT NULL,      /*月份*/

            //public double ccg12a  { get; set; } //   number(20,6) NOT NULL,   /*上月结存金额-材料(a)*/
            //public double ccg12b  { get; set; } //   number(20,6) NOT NULL,   /*上月结存金额-人工(b)*/
            //public double ccg12c  { get; set; } //   number(20,6) NOT NULL,   /*上月结存金额-制费(c)*/
            //public double ccg12d  { get; set; } //   number(20,6) NOT NULL,   /*上月结存金额-加工(d)*/
            //public double ccg12e  { get; set; } //   number(20,6) NOT NULL,   /*上月结存金额-其他(e)*/
            //public double ccg20   { get; set; } //   number(15,3) NOT NULL,   /*本月投入工时*/
            //public double ccg21   { get; set; } //   number(15,3) NOT NULL,   /*本月投入数量*/
            //public double ccg22   { get; set; } //   number(20,6) NOT NULL,   /*本月本阶投入金额(a+b+c+d+e)*/
            //public double ccg22a  { get; set; } //   number(20,6) NOT NULL,   /*本月本阶投入金额-材料(a)*/
            //public double ccg22b  { get; set; } //   number(20,6) NOT NULL,   /*本月本阶投入金额-人工(b)*/
            //public double ccg22c  { get; set; } //   number(20,6) NOT NULL,   /*本月本阶投入金额-制费(c)*/
            //public double ccg22d  { get; set; } //   number(20,6) NOT NULL,   /*本月本阶投入金额-加工(d)*/
            //public double ccg22e  { get; set; } //   number(20,6) NOT NULL,   /*本月本阶投入金额-其他(e)*/
            //public double ccg23   { get; set; } //   number(20,6) NOT NULL,   /*本月下阶投入金额(a+b+c+d+e)*/
            //public double ccg23a  { get; set; } //   number(20,6) NOT NULL,   /*本月下阶投入金额-材料(a)*/
            //public double ccg23b  { get; set; } //   number(20,6) NOT NULL,   /*本月下阶投入金额-人工(b)*/
            //public double ccg23c  { get; set; } //   number(20,6) NOT NULL,   /*本月下阶投入金额-制费(c)*/
            //public double ccg23d  { get; set; } //   number(20,6) NOT NULL,   /*本月下阶投入金额-加工(d)*/
            //public double ccg23e  { get; set; } //   number(20,6) NOT NULL,   /*本月下阶投入金额-其他(e)*/
            //public double ccg31   { get; set; } //   number(15,3) NOT NULL,   /*本月转出数量*/
            //public double ccg32   { get; set; } //   number(20,6) NOT NULL,   /*本月转出金额(a+b+c+d+e)*/
            //public double ccg32a  { get; set; } //   number(20,6) NOT NULL,   /*本月转出金额-材料(a)*/
            //public double ccg32b  { get; set; } //   number(20,6) NOT NULL,   /*本月转出金额-人工(b)*/
            //public double ccg32c  { get; set; } //   number(20,6) NOT NULL,   /*本月转出金额-制费(c)*/
            //public double ccg32d  { get; set; } //   number(20,6) NOT NULL,   /*本月转出金额-加工(d)*/
            //public double ccg32e { get; set; }  //   number(20,6) NOT NULL,   /*本月转出金额-其他(e)*/
            //public double ccg41 { get; set; }   //   number(15,3),            /*差异转出数量*/
            //public double ccg42 { get; set; }   //     number(20,6) NOT NULL,   /*差异转出金额*//*差异转出金额 (标准成本制下的标准差异)*/
            //public double ccg42a  { get; set; } //   number(20,6) NOT NULL,   /*差异转出金额-材料(a)*/
            //public double ccg42b  { get; set; } //   number(20,6) NOT NULL,   /*差异转出金额-人工(b)*/
            //public double ccg42c  { get; set; } //   number(20,6) NOT NULL,   /*差异转出金额-制费(c)*/
            //public double ccg42d  { get; set; } //   number(20,6) NOT NULL,   /*差异转出金额-加工(d)*/
            //public double ccg42e  { get; set; } //   number(20,6) NOT NULL,   /*差异转出金额-其他(e)*/
            public double ccg91 { get; set; } //   number(15,3) NOT NULL,   /*本月结存数量*/
            public double ccg92 { get; set; } //   number(20,6) NOT NULL,   /*本月结存金额(a+b+c+d+e)*/
            //public double ccg92a  { get; set; } //   number(20,6) NOT NULL,   /*本月结存金额-材料(a)*/
            //public double ccg92b  { get; set; } //   number(20,6) NOT NULL,   /*本月结存金额-人工(b)*/
            //public double ccg92c  { get; set; } //   number(20,6) NOT NULL,   /*本月结存金额-制费(c)*/
            //public double ccg92d  { get; set; } //   number(20,6) NOT NULL,   /*本月结存金额-加工(d)*/
            //public double ccg92e { get; set; }  //   number(20,6) NOT NULL,   /*本月结存金额-其他(e)*/

            //[Display(Name = "最近计算人员"), StringLength(10)]
            //public string ccguser { get; set; }  //  varchar2(10),            /*最近计算人员*/
            //public DateTime ccgdate { get; set; } // date,                    /*最近计算日期*/

            //[Display(Name = "最近计算时间"), StringLength(5)]
            //public string ccgtime { get; set; } //   varchar2(5),             /*最近计算时间*/

            //[Display(Name = "成本计算类别"), StringLength(1)]
            //public string ccg06 { get; set; }  //    varchar2(1) DEFAULT ' ' NOT NULL, /*成本计算类别*/

            //[Display(Name = "类别代号(批次号/专案号/利润"), StringLength(40)]
            //public string ccg07 { get; set; }   //   varchar2(40) DEFAULT ' ' NOT NULL, /*类别代号(批次号/专案号/利润 */
            //public double ccg12f { get; set; }  //   number(20,6) DEFAULT '0',/*上月结存金额-制费三*/
            //public double ccg12g { get; set; }  //   number(20,6) DEFAULT '0',/*上月结存金额-制费四*/
            //public double ccg12h { get; set; }  //   number(20,6) DEFAULT '0',/*上月结存金额-制费五*/
            //public double ccg22f { get; set; }  //   number(20,6) DEFAULT '0',/*本月本阶投入金额-制费三*/
            //public double ccg22g { get; set; }  //   number(20,6) DEFAULT '0',/*本月本阶投入金额-制费四*/
            //public double ccg22h { get; set; }  //   number(20,6) DEFAULT '0',/*本月本阶投入金额-制费五*/
            //public double ccg23f { get; set; }  //   number(20,6) DEFAULT '0',/*本月下阶投入金额-制费三*/
            //public double ccg23g { get; set; }  //   number(20,6) DEFAULT '0',/*本月下阶投入金额-制费四*/
            //public double ccg23h { get; set; }  //   number(20,6) DEFAULT '0',/*本月下阶投入金额-制费五*/
            //public double ccg32f { get; set; }  //   number(20,6) DEFAULT '0',/*本月转出金额-制费三*/
            //public double ccg32g { get; set; }  //   number(20,6) DEFAULT '0',/*本月转出金额-制费四*/
            //public double ccg32h { get; set; }  //   number(20,6) DEFAULT '0',/*本月转出金额-制费五*/
            //public double ccg42f { get; set; }  //   number(20,6) DEFAULT '0',/*差异输出金额-制费三*/
            //public double ccg42g { get; set; }  //   number(20,6) DEFAULT '0',/*差异输出金额-制费四*/
            //public double ccg42h { get; set; }  //   number(20,6) DEFAULT '0',/*差异输出金额-制费五*/
            //public double ccg92f { get; set; }  //   number(20,6) DEFAULT '0',/*本月结存金额-制费三*/
            //public double ccg92g { get; set; }  //   number(20,6) DEFAULT '0',/*本月结存金额-制费四*/
            //public double ccg92h { get; set; }  //   number(20,6) DEFAULT '0',/*本月结存金额-制费五*/

            //[Display(Name = "自订栏位-Textedit"), StringLength(255)]
            //public string ccgud01 { get; set; } //   varchar2(255),           /*自订栏位-Textedit  */

            //[Display(Name = "自订栏位-文字"), StringLength(40)]
            //public string ccgud02 { get; set; } //   varchar2(40),            /*自订栏位-文字*/

            //[Display(Name = "自订栏位-文字"), StringLength(40)]
            //public string ccgud03 { get; set; } //   varchar2(40),            /*自订栏位-文字*/

            //[Display(Name = "自订栏位-文字"), StringLength(40)]
            //public string ccgud04 { get; set; } //   varchar2(40),            /*自订栏位-文字*/

            //[Display(Name = "自订栏位-文字"), StringLength(40)]
            //public string ccgud05 { get; set; } //   varchar2(40),            /*自订栏位-文字*/

            //[Display(Name = "自订栏位-文字"), StringLength(40)]
            //public string ccgud06 { get; set; } //   varchar2(40),            /*自订栏位-文字*/
            //public double ccgud07 { get; set; } //   number(15,3),            /*自订栏位-数值*/
            //public double ccgud08 { get; set; } //  number(15,3),            /*自订栏位-数值 */
            //public double ccgud09 { get; set; } //  number(15,3),            /*自订栏位-数值 */
            //public int ccgud10 { get; set; }    //  number(10),              /*自订栏位-整数 */
            //public int ccgud11 { get; set; }    //  number(10),              /*自订栏位-整数 */
            //public int ccgud12 { get; set; }    //  number(10),              /*自订栏位-整数 */
            //public DateTime ccgud13 { get; set; }//  date,                    /*自订栏位-日期**/
            //public DateTime ccgud14 { get; set; }//  date,                    /*自订栏位-日期**/
            //public DateTime ccgud15 { get; set; }//  date,                    /*自订栏位-日期**/

            //[Display(Name = "所属工厂"), StringLength(10)]
            //public string ccgplant { get; set; } //  varchar2(10),            /*所属工厂     **/

            //[Display(Name = "所属法人"), StringLength(10)]
            //public string ccglegal { get; set; } //  varchar2(10),            /*所属法人     **/

            //[Display(Name = "资料建立部门"), StringLength(10)]
            //public string ccgorig { get; set; }  //  varchar2(10),            /*资料建立部门 */

            //[Display(Name = "资料建立者"), StringLength(10)]
            //public string ccgoriu { get; set; }  //  varchar2(10)             /*资料建立者   */

            //[Display(Name = "物料编号"), StringLength(50)]
            //public string ima01 { get; set; }

            [Display(Name = "物料品名"), StringLength(100)]
            public string ima02 { get; set; }

            [Display(Name = "其他分群码 四"), StringLength(10)]
            public string ima12 { get; set; }// varchar2(10),    /*其他分群码 四 */ /*成本分群码    */

            [Display(Name = "物料规格"), StringLength(1000)]
            public string ima021 { get; set; }

            [Display(Name = "是否为重覆性生产料件 (Y/N) "), StringLength(1)]
            public string ima911 { get; set; }// varchar2(1),             /*是否为重覆性生产料件 (Y/N)*/

            [Display(Name = "重工否(Y/N)"), StringLength(1)]
            public string sfb99 { get; set; } //varchar2(1),  /*重工否*/ /*重工否(Y/N)*/

            [Display(Name = "备注"), StringLength(1000)]
            public string sfbud01 { get; set; } ///varchar2(255),           /*自订栏位-Textedit                      */

            [Display(Name = "分类明细")]
            public string Classification { get; set; }

        }

        public class Financedetails
        {
            [Display(Name = "A 料号"), StringLength(40)]
            public string A { get; set; }

            [Display(Name = "B 品名"), StringLength(120)]
            public string B { get; set; }

            [Display(Name = "C 规格"), StringLength(1000)]
            public string C { get; set; }

            [Display(Name = "D 报废")]
            public double D { get; set; }

            [Display(Name = "E 本月结存数")]
            public double E { get; set; }

            [Display(Name = "F 结存单价")]
            public double F { get; set; }

            [Display(Name = "G 结存金额")]
            public double G { get; set; }

            [Display(Name = "H 超180天呆料")]
            public double H { get; set; }

            [Display(Name = "I 超呆料金额")]
            public double I { get; set; }

            [Display(Name = "J K未发料汇总")]
            public double J { get; set; }

            [Display(Name = "K K需求（备库订单+备库物料）")]
            public double K { get; set; }

            [Display(Name = "L K需求汇总金额")]
            public double L { get; set; }

            [Display(Name = "M 非K未发料")]
            public double M { get; set; }

            [Display(Name = "N 非K需求")]
            public double N { get; set; }

            [Display(Name = "O 超过180天库存")]
            public double O { get; set; }

            [Display(Name = "P 非K需求金额")]
            public double P { get; set; }

            [Display(Name = "Q 其它库存")]
            public double Q { get; set; }

            [Display(Name = "R 其它库存金额")]
            public double R { get; set; }

            [Display(Name = "S 超过180天")]
            public double S { get; set; }

            [Display(Name = "T 多")]
            public double T { get; set; }

            [Display(Name = "U K+非K")]
            public double U { get; set; }

            [Display(Name = "V 是否为签核备库")]
            public double V { get; set; }

            [Display(Name = "W 30天")]
            public double W { get; set; }

            [Display(Name = "X K需求")]
            public double X { get; set; }

            [Display(Name = "Y K金额")]
            public double Y { get; set; }

            [Display(Name = "Z 非K需求")]
            public double Z { get; set; }

            [Display(Name = "AA 非K金额")]
            public double AA { get; set; }

            [Display(Name = "AB 其它库存金额")]
            public double AB { get; set; }

            [Display(Name = "AC	30天金额")]
            public double AC { get; set; }

            [Display(Name = "AD	90天")]
            public double AD { get; set; }

            [Display(Name = "AE	90天金额")]
            public double AE { get; set; }

            [Display(Name = "AF K需求")]
            public double AF { get; set; }

            [Display(Name = "AG K金额")]
            public double AG { get; set; }

            [Display(Name = "AH 非K需求")]
            public double AH { get; set; }

            [Display(Name = "AI 非K金额")]
            public double AI { get; set; }

            [Display(Name = "AJ  其它库存金额")]
            public double AJ { get; set; }

            [Display(Name = "AK	180天")]
            public double AK { get; set; }

            [Display(Name = "AL 180天金额")]
            public double AL { get; set; }

            [Display(Name = "AM K需求")]
            public double AM { get; set; }

            [Display(Name = "AN K金额")]
            public double AN { get; set; }

            [Display(Name = "AO 非K需求")]
            public double AO { get; set; }

            [Display(Name = "AP 非K金额")]
            public double AP { get; set; }

            [Display(Name = "AQ 其它库存金额")]
            public double AQ { get; set; }

            [Display(Name = "AR	365天")]
            public double AR { get; set; }

            [Display(Name = "AS	365天金额")]
            public double AS { get; set; }

            [Display(Name = "AT K需求")]
            public double AT { get; set; }

            [Display(Name = "AU K金额")]
            public double AU { get; set; }

            [Display(Name = "AV 非K需求")]
            public double AV { get; set; }

            [Display(Name = "AW 非K金额")]
            public double AW { get; set; }

            [Display(Name = "AX 其它库存金额")]
            public double AX { get; set; }

            [Display(Name = "AY 2年")]
            public double AY { get; set; }

            [Display(Name = "AZ 2年金额")]
            public double AZ { get; set; }

            [Display(Name = "BA K需求")]
            public double BA { get; set; }

            [Display(Name = "BB K金额")]
            public double BB { get; set; }

            [Display(Name = "BC 非K需求")]
            public double BC { get; set; }

            [Display(Name = "BD 非K金额")]
            public double BD { get; set; }

            [Display(Name = "BE 其它库存金额")]
            public double BE { get; set; }

            [Display(Name = "BF 3年")]
            public double BF { get; set; }

            [Display(Name = "BG 3年金额")]
            public double BG { get; set; }

            [Display(Name = "BH 3-5年")]
            public double BH { get; set; }

            [Display(Name = "BI 3-5年金额")]
            public double BI { get; set; }

            [Display(Name = "BJ 5年以上")]
            public double BJ { get; set; }

            [Display(Name = "BK 5年以上金额")]
            public double BK { get; set; }

            [Display(Name = "BL 分类")]
            public double BL { get; set; }

            [Display(Name = "BM K需求")]
            public double BM { get; set; }

            [Display(Name = "BN K金额")]
            public double BN { get; set; }

            [Display(Name = "BO 非K需求")]
            public double BO { get; set; }

            [Display(Name = "BP 非K金额")]
            public double BP { get; set; }

            [Display(Name = "BQ 其它库存金额")]
            public double BQ { get; set; }

            [Display(Name = "BR 3年以上")]
            public double BR { get; set; }

            [Display(Name = "BS K需求")]
            public double BS { get; set; }

            [Display(Name = "BT K金额")]
            public double BT { get; set; }

            [Display(Name = "BU 非K需求")]
            public double BU { get; set; }

            [Display(Name = "BV 非K金额")]
            public double BV { get; set; }

            [Display(Name = "BW 其它库存金额")]
            public double BW { get; set; }

            [Display(Name = "BX 3年以上金额")]
            public double BX { get; set; }

            [Display(Name = "BY UKN")]
            public double BY { get; set; }

            [Display(Name = "BZ Q")]
            public double BZ { get; set; }
        }

        #endregion

        #region------ ERP查询方法


        //安全库存物料清单查询
        public static List<img_file> ERP_Query_SafetyStock(List<string> material_List)
        {
            string data = string.Empty;
            DataTable OutDataTable = new DataTable();
            OutDataTable.TableName = "data";
            List<img_file> result_list = new List<img_file>();
            if (material_List.Count == 0) return result_list;
            try
            {
                using (OracleConnection cn = GetOracleConnection())
                {
                    cn.Open();
                    string sql = "";
                    foreach (var item in material_List)
                    {
                        if (sql == "") sql = "Select img_file.*,ima_file.* from img_file inner join ima_file on img_file.img01=ima_file.ima01 where img01 ='" + item + "'";
                        else sql = sql + " or img01 = '" + item + "'";
                    }
                    OracleCommand cmd = new OracleCommand(sql, cn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(OutDataTable);
                    cmd.Dispose();
                    foreach (DataRow dataRow in OutDataTable.Rows)
                    {
                        img_file record = new img_file();
                        record.img01 = dataRow["img01"].ToString();
                        record.ima02 = dataRow["ima02"].ToString();
                        record.ima021 = dataRow["ima021"].ToString();
                        record.img02 = dataRow["img02"].ToString();
                        record.img03 = dataRow["img03"].ToString();
                        record.img04 = dataRow["img04"].ToString();
                        record.img05 = dataRow["img05"].ToString();
                        record.img06 = dataRow["img06"].ToString() == "" ? 0 : Convert.ToInt16(dataRow["img06"].ToString());
                        record.img07 = dataRow["img07"].ToString();
                        record.img08 = dataRow["img08"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["img08"].ToString());
                        record.img09 = dataRow["img09"].ToString();
                        record.img10 = dataRow["img10"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["img10"].ToString());
                        record.img11 = dataRow["img11"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["img11"].ToString());
                        record.img12 = dataRow["img12"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["img12"].ToString());
                        record.img13 = StringTODateTime(dataRow["img13"].ToString());
                        record.img14 = StringTODateTime(dataRow["img14"].ToString());
                        record.img15 = StringTODateTime(dataRow["img15"].ToString());
                        record.img16 = StringTODateTime(dataRow["img16"].ToString());
                        record.img17 = StringTODateTime(dataRow["img17"].ToString());
                        record.img18 = StringTODateTime(dataRow["img18"].ToString());
                        record.img19 = dataRow["img19"].ToString();
                        record.img20 = dataRow["img20"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["img20"].ToString());
                        record.img21 = dataRow["img21"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["img21"].ToString());
                        record.img22 = dataRow["img22"].ToString();
                        record.img23 = dataRow["img23"].ToString();
                        record.img24 = dataRow["img24"].ToString();
                        record.img25 = dataRow["img25"].ToString();
                        record.img26 = dataRow["img26"].ToString();
                        record.img27 = dataRow["img27"].ToString() == "" ? 0 : Convert.ToInt16(dataRow["img27"].ToString());
                        record.img28 = dataRow["img28"].ToString() == "" ? 0 : Convert.ToInt16(dataRow["img28"].ToString());
                        //record.img29 = dataRow["img29"].ToString();
                        record.img30 = dataRow["img30"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["img30"].ToString());
                        record.img31 = dataRow["img31"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["img31"].ToString());
                        record.img32 = dataRow["img32"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["img32"].ToString());
                        record.img33 = dataRow["img33"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["img33"].ToString());
                        record.img34 = dataRow["img34"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["img34"].ToString());
                        record.img35 = dataRow["img35"].ToString();
                        record.img36 = dataRow["img36"].ToString();
                        record.img37 = StringTODateTime(dataRow["img37"].ToString());
                        record.img38 = dataRow["img38"].ToString();
                        //record.imgplant = dataRow["imgplant"].ToString();
                        //record.imglegal = dataRow["imglegal"].ToString();
                        result_list.Add(record);
                    }
                    cn.Dispose();
                    cn.Close();
                    #region------ 修改列名
                    //OutDataTable.Columns["img01"].ColumnName = "料件编号";
                    //OutDataTable.Columns["img02"].ColumnName = "仓库编号";
                    //OutDataTable.Columns["img03"].ColumnName = "储位";
                    //OutDataTable.Columns["img04"].ColumnName = "批号";
                    //OutDataTable.Columns["img05"].ColumnName = "参考号码";
                    //OutDataTable.Columns["img06"].ColumnName = "参考序号";
                    //OutDataTable.Columns["img07"].ColumnName = "采购单位";
                    //OutDataTable.Columns["img08"].ColumnName = "收货数量";
                    //OutDataTable.Columns["img09"].ColumnName = "库存单位";
                    //OutDataTable.Columns["img10"].ColumnName = "库存数量";
                    //OutDataTable.Columns["img11"].ColumnName = "No Use";
                    //OutDataTable.Columns["img12"].ColumnName = "No Use";
                    //OutDataTable.Columns["img13"].ColumnName = "制造日期";
                    //OutDataTable.Columns["img14"].ColumnName = "最近一次盘点日期";
                    //OutDataTable.Columns["img15"].ColumnName = "最近一次收料日期";
                    //OutDataTable.Columns["img16"].ColumnName = "最近一次发料日期";
                    //OutDataTable.Columns["img17"].ColumnName = "最近一次异动日期";
                    //OutDataTable.Columns["img18"].ColumnName = "有效日期";
                    //OutDataTable.Columns["img19"].ColumnName = "库存等级";
                    //OutDataTable.Columns["img20"].ColumnName = "单位数量换算率";
                    //OutDataTable.Columns["img21"].ColumnName = "单位数量换算率-对料件库存单";
                    //OutDataTable.Columns["img22"].ColumnName = "仓储类别";
                    //OutDataTable.Columns["img23"].ColumnName = "是否为可用仓储";
                    //OutDataTable.Columns["img24"].ColumnName = "是否为ＭＲＰ可用仓储";
                    //OutDataTable.Columns["img25"].ColumnName = "保税与否";
                    //OutDataTable.Columns["img26"].ColumnName = "仓储所属会计科目";
                    //OutDataTable.Columns["img27"].ColumnName = "工单发料优先顺序";
                    //OutDataTable.Columns["img28"].ColumnName = "销售出货优先顺序";
                    ////OutDataTable.Columns["img29"].ColumnName = "未知列";
                    //OutDataTable.Columns["img30"].ColumnName = "直接材料成本";
                    //OutDataTable.Columns["img31"].ColumnName = "间接材料成本";
                    //OutDataTable.Columns["img32"].ColumnName = "厂外加工材料成本";
                    //OutDataTable.Columns["img33"].ColumnName = "厂外加工人工成本";
                    //OutDataTable.Columns["img34"].ColumnName = "库存单位对成本单位的转换率";
                    //OutDataTable.Columns["img35"].ColumnName = "专案号码";
                    //OutDataTable.Columns["img36"].ColumnName = "外观编号";
                    //OutDataTable.Columns["img37"].ColumnName = "呆滞日期";
                    //OutDataTable.Columns["img38"].ColumnName = "备注";
                    ////OutDataTable.Columns["imgplant"].ColumnName = "所属工厂";
                    ////OutDataTable.Columns["imglengal"].ColumnName = "所属法人";
                    #endregion

                }
                return result_list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //按物料号或物料名称查询
        public static List<img_file> ERP_MaterialQuery(string materialNumber, string productName)
        {
            string data = string.Empty;
            DataTable OutDataTable = new DataTable();
            OutDataTable.TableName = "data";
            List<img_file> result_list = new List<img_file>();
            try
            {
                using (OracleConnection cn = GetOracleConnection())
                {
                    cn.Open();
                    //string sql = "";
                    //string sql1 = "";  //增加收料员和供应商两个字段
                    string sql2 = "";  //增加物料类型字段ROHS

                    if (!string.IsNullOrEmpty(materialNumber) && !String.IsNullOrEmpty(productName))
                    {
                        //sql = "Select img_file.*,ima_file.* from img_file inner join ima_file on img_file.img01=ima_file.ima01 where img01 ='" + materialNumber + "' and ima02 like '%" + productName + "%'";
                        ////增加收料员和供应商两个字段
                        //sql1 = "Select img_file.*,ima_file.*, rvaud02||'/'||gen02 as 收料员, rva05||pmc03 as 供应厂商 from img_file inner join ima_file on img_file.img01 = ima_file.ima01 left join ((rva_file left join hzdb.pmc_file on pmc01 = rva05) left join gen_file on rvaud02 = gen01) on img05 = rva01 where img01 = '" + materialNumber + "' and ima02 like '%" + productName + "%'";
                        //增加收料员和供应商、物料类型ROHS三个字段
                        sql2 = "select img_file.*,ima_file.*, rvaud02||'/'||gen02 as 收料员, rva05||pmc03 as 供应厂商,case when instr(ima021,'ROHS')>0 then 'ROHS' end as ROHS from img_file inner join ima_file on img_file.img01 = ima_file.ima01 left join ((rva_file left join hzdb.pmc_file on pmc01 = rva05) left join gen_file on rvaud02 = gen01) on img05 = rva01 where img01 = '" + materialNumber + "' and ima02 like '%" + productName + "%'";
                    }
                    else if (!string.IsNullOrEmpty(materialNumber))
                    {
                        //sql = "Select img_file.*,ima_file.* from img_file inner join ima_file on img_file.img01=ima_file.ima01 where img01 ='" + materialNumber + "'";
                        ////增加收料员和供应商两个字段
                        //sql1 = "Select img_file.*,ima_file.*, rvaud02||'/'||gen02 as 收料员, rva05||pmc03 as 供应厂商 from img_file inner join ima_file on img_file.img01 = ima_file.ima01 left join ((rva_file left join hzdb.pmc_file on pmc01 = rva05) left join gen_file on rvaud02 = gen01) on img05 = rva01 where img01 = '" + materialNumber + "'";
                        //增加收料员和供应商、物料类型ROHS三个字段
                        sql2 = "select img_file.*,ima_file.*, rvaud02||'/'||gen02 as 收料员, rva05||pmc03 as 供应厂商,case when instr(ima021,'ROHS')>0 then 'ROHS' end as ROHS from img_file inner join ima_file on img_file.img01 = ima_file.ima01 left join ((rva_file left join hzdb.pmc_file on pmc01 = rva05) left join gen_file on rvaud02 = gen01) on img05 = rva01 where img01 = '" + materialNumber + "'";
                    }
                    else if (!String.IsNullOrEmpty(productName))
                    {
                        //sql = "Select img_file.*,ima_file.* from ima_file left join img_file on ima_file.ima01=img_file.img01 where ima02 like '%" + productName + "%'";
                        ////增加收料员和供应商两个字段
                        //sql1 = "Select img_file.*,ima_file.*, rvaud02||'/'||gen02 as 收料员, rva05||pmc03 as 供应厂商 from img_file inner join ima_file on img_file.img01 = ima_file.ima01 left join ((rva_file left join hzdb.pmc_file on pmc01 = rva05) left join gen_file on rvaud02 = gen01) on img05 = rva01 where ima02 like '%" + productName + "%'";
                        //增加收料员和供应商、物料类型ROHS三个字段
                        sql2 = "select img_file.*,ima_file.*, rvaud02||'/'||gen02 as 收料员, rva05||pmc03 as 供应厂商,case when instr(ima021,'ROHS')>0 then 'ROHS' end as ROHS from img_file inner join ima_file on img_file.img01 = ima_file.ima01 left join ((rva_file left join hzdb.pmc_file on pmc01 = rva05) left join gen_file on rvaud02 = gen01) on img05 = rva01 where ima02 like '%" + productName + "%'";
                    }
                    OracleCommand cmd = new OracleCommand(sql2, cn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(OutDataTable);
                    cmd.Dispose();
                    foreach (DataRow dataRow in OutDataTable.Rows)
                    {
                        img_file record = new img_file();
                        record.rvaud02 = dataRow["收料员"].ToString();
                        record.rva05 = dataRow["供应厂商"].ToString();
                        record.MaterialType = dataRow["ROHS"].ToString();
                        record.img01 = dataRow["IMG01"].ToString();
                        record.ima02 = dataRow["IMA02"].ToString();
                        record.ima021 = dataRow["IMA021"].ToString();
                        record.img02 = dataRow["IMG02"].ToString();
                        record.img03 = dataRow["IMG03"].ToString();
                        record.img04 = dataRow["IMG04"].ToString();
                        record.img05 = dataRow["IMG05"].ToString();
                        record.img06 = String.IsNullOrEmpty(dataRow["IMG06"].ToString()) == true ? 0 : Convert.ToInt16(dataRow["IMG06"].ToString());
                        record.img07 = dataRow["IMG07"].ToString();
                        record.img08 = String.IsNullOrEmpty(dataRow["IMG08"].ToString()) == true ? 0 : Convert.ToDouble(dataRow["IMG08"].ToString());
                        record.img09 = dataRow["IMG09"].ToString();
                        record.img10 = String.IsNullOrEmpty(dataRow["IMG10"].ToString()) == true ? 0 : Convert.ToDouble(dataRow["IMG10"].ToString());
                        record.img11 = String.IsNullOrEmpty(dataRow["IMG11"].ToString()) == true ? 0 : Convert.ToDouble(dataRow["IMG11"].ToString());
                        record.img12 = String.IsNullOrEmpty(dataRow["IMG12"].ToString()) == true ? 0 : Convert.ToDouble(dataRow["IMG12"].ToString());
                        record.img13 = StringTODateTime(dataRow["IMG13"].ToString());
                        record.img14 = StringTODateTime(dataRow["IMG14"].ToString());
                        record.img15 = StringTODateTime(dataRow["IMG15"].ToString());
                        record.img16 = StringTODateTime(dataRow["IMG16"].ToString());
                        record.img17 = StringTODateTime(dataRow["IMG17"].ToString());
                        record.img18 = StringTODateTime(dataRow["IMG18"].ToString());
                        record.img19 = dataRow["IMG19"].ToString();
                        record.img20 = String.IsNullOrEmpty(dataRow["IMG20"].ToString()) == true ? 0 : Convert.ToDouble(dataRow["IMG20"].ToString());
                        record.img21 = String.IsNullOrEmpty(dataRow["IMG21"].ToString()) == true ? 0 : Convert.ToDouble(dataRow["IMG21"].ToString());
                        record.img22 = dataRow["IMG22"].ToString();
                        record.img23 = dataRow["IMG23"].ToString();
                        record.img24 = dataRow["IMG24"].ToString();
                        record.img25 = dataRow["IMG25"].ToString();
                        record.img26 = dataRow["IMG26"].ToString();
                        record.img27 = String.IsNullOrEmpty(dataRow["IMG27"].ToString()) == true ? 0 : Convert.ToInt16(dataRow["IMG27"].ToString());
                        record.img28 = String.IsNullOrEmpty(dataRow["IMG28"].ToString()) == true ? 0 : Convert.ToInt16(dataRow["IMG28"].ToString());
                        record.img30 = String.IsNullOrEmpty(dataRow["IMG30"].ToString()) == true ? 0 : Convert.ToDouble(dataRow["IMG30"].ToString());
                        record.img31 = String.IsNullOrEmpty(dataRow["IMG31"].ToString()) == true ? 0 : Convert.ToDouble(dataRow["IMG31"].ToString());
                        record.img32 = String.IsNullOrEmpty(dataRow["IMG32"].ToString()) == true ? 0 : Convert.ToDouble(dataRow["IMG32"].ToString());
                        record.img33 = String.IsNullOrEmpty(dataRow["IMG33"].ToString()) == true ? 0 : Convert.ToDouble(dataRow["IMG33"].ToString());
                        record.img34 = String.IsNullOrEmpty(dataRow["IMG34"].ToString()) == true ? 0 : Convert.ToDouble(dataRow["IMG34"].ToString());
                        record.img35 = dataRow["IMG35"].ToString();
                        record.img36 = dataRow["IMG36"].ToString();
                        record.img37 = StringTODateTime(dataRow["IMG37"].ToString());
                        record.img38 = dataRow["IMG38"].ToString();
                        result_list.Add(record);
                    }
                    cn.Dispose();
                    cn.Close();
                }
                return result_list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //按物料号或物料名称查询(备品配件物料查询)
        public static List<ima_file> ERP_MaterialQuery_SPC(string materialNumber, string productName)
        {
            string data = string.Empty;
            DataTable OutDataTable = new DataTable();
            OutDataTable.TableName = "data";
            List<ima_file> result_list = new List<ima_file>();
            try
            {
                using (OracleConnection cn = GetOracleConnection())
                {
                    cn.Open();
                    string sql = "";
                    if (!string.IsNullOrEmpty(materialNumber) && !String.IsNullOrEmpty(productName))
                    {
                        sql = "Select ima_file.* from ima_file where ima01 ='" + materialNumber + "' and ima02 like '%" + productName + "%'";
                    }
                    else if (!string.IsNullOrEmpty(materialNumber))
                    {
                        sql = "Select ima_file.* from ima_file where ima01 ='" + materialNumber + "'";
                    }
                    else if (!String.IsNullOrEmpty(productName))
                    {
                        sql = "Select ima_file.* from ima_file where ima02 like '%" + productName + "%'";
                    }
                    OracleCommand cmd = new OracleCommand(sql, cn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(OutDataTable);
                    cmd.Dispose();
                    foreach (DataRow dataRow in OutDataTable.Rows)
                    {
                        ima_file record = new ima_file();
                        record.ima01 = dataRow["IMA01"].ToString();
                        record.ima02 = dataRow["IMA02"].ToString();
                        record.ima021 = dataRow["IMA021"].ToString();
                        result_list.Add(record);
                    }
                    cn.Dispose();
                    cn.Close();
                }
                return result_list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //按收货单号或采购单单号称查询
        public static List<rva_rvb_file> ERP_ContractQuery(string receivingNumber, string purchaseNumber) 
        {
            string data = string.Empty;
            DataTable OutDataTable = new DataTable();
            OutDataTable.TableName = "data";
            List<rva_rvb_file> result_list = new List<rva_rvb_file>();
            try
            {
                using (OracleConnection cn = GetOracleConnection())
                {
                    cn.Open();
                    string sql = "";
                    string sql1 = "";   //增加物料类型字段ROHS
                    if (!string.IsNullOrEmpty(receivingNumber) && !String.IsNullOrEmpty(purchaseNumber))
                    {
                        //rva01就是收货单号 receivingNumber，rva02是采购单号 purchaseNumber
                        //sql = "select rvaud02 || '/' || gen02 as 收料员, rva01 as 收货单号, rva05 || pmc03 as 供应厂商, rva06 as 收货日期, rva02 as 采购单单号, pmm20 || '/' || pma02 as 付款方式, rva08 as 备注, rvb01 as 项次行序, rvb05 as 料件编号, pmn041 || ',' || ima021 as 品名规格, rvb04 || ',' || rvb03 as 采购单号项次, rvb07 as 实收数量,pmn07 as 单位,(select pmk12 from pmk_file where pmk01 = pmn24)|| '-' || (select gen02 from gen_file where gen01 = (select pmk12 from pmk_file where pmk01 = pmn24)) as 请购人,rvb36 || '-' || rvb37 || '-' || rvb38 as 仓储批, pmn24 || '-' || pmn25 as 请购单号项次,pmn122 as 合同编号 from hzdb.rva_file left join hzdb.rvb_file on rva01 = rvb01 left join hzdb.gen_file on rvaud02 = gen01 left join hzdb.pmc_file on pmc01 = rva05 left join (hzdb.pmm_file left join hzdb.pma_file on pma01 = pmm20) on pmm01 = rva02 left join hzdb.pmn_file on(pmn01= rvb04 and pmn02 = rvb03) left join hzdb.ima_file on rvb05 = ima01 where rva01 = '"+ receivingNumber + "' or rva02 = '"+ purchaseNumber + "'";

                        sql = "select rvaud02||'/'||gen02 as 收料员, rva01 as 收货单号, rva05||pmc03 as 供应厂商, rva06 as 收货日期, rva02 as 采购单单号, pmm20||'/'|| pma02 as 付款方式, rva08 as 备注, rvb01 as 项次行序, rvb05 as 料件编号, pmn041 || ',' || ima021 as 品名规格, rvb04 || ',' || rvb03 as 采购单号项次, rvb07 as 实收数量,pmn07 as 单位,(select pmk12 from pmk_file where pmk01 = pmn24)|| '-' || (select gen02 from gen_file where gen01 = (select pmk12 from pmk_file where pmk01 = pmn24)) as 请购人,rvb36 || '-' || rvb37 || '-' || rvb38 as 仓储批, pmn24 || '-' || pmn25 as 请购单号项次,pmn122 as 合同编号, ima021 as 规格, pmn041 as 采购料件编号,pmn041 as 采购料件品名, ima021 as 采购料件规格,pmn20 as 采购量, (pmn50 - pmn55) as 已收量, rvb30 as 入库量, rvb31 as 可入库量, rvb29 as 退货量, rvb35 as 样品否, rvb36 as 仓库, rvb37 as 库位, rvb38 as 批号 from hzdb.rva_file left join hzdb.rvb_file on rva01 = rvb01 left join hzdb.gen_file on rvaud02 = gen01 left join hzdb.pmc_file on pmc01 = rva05 left join (hzdb.pmm_file left join hzdb.pma_file on pma01 = pmm20) on pmm01 = rva02 left join hzdb.pmn_file on(pmn01= rvb04 and pmn02 = rvb03) left join hzdb.ima_file on rvb05 = ima01 where rva01 = '" + receivingNumber + "' or rvb04 = '" + purchaseNumber + "'";
                        //增加物料类型字段ROHS
                        sql1 = "select rvaud02||'/'||gen02 as 收料员, rva01 as 收货单号, rva05||pmc03 as 供应厂商, rva06 as 收货日期, rva02 as 采购单单号, pmm20||'/'|| pma02 as 付款方式, rva08 as 备注, rvb01 as 项次行序, rvb05 as 料件编号, pmn041 || ',' || ima021 as 品名规格, rvb04 || ',' || rvb03 as 采购单号项次, rvb07 as 实收数量,pmn07 as 单位,(select pmk12 from pmk_file where pmk01 = pmn24)|| '-' || (select gen02 from gen_file where gen01 = (select pmk12 from pmk_file where pmk01 = pmn24)) as 请购人,rvb36 || '-' || rvb37 || '-' || rvb38 as 仓储批, pmn24 || '-' || pmn25 as 请购单号项次,pmn122 as 合同编号, ima021 as 规格, pmn041 as 采购料件编号,pmn041 as 采购料件品名, ima021 as 采购料件规格,pmn20 as 采购量, (pmn50 - pmn55) as 已收量, rvb30 as 入库量, rvb31 as 可入库量, rvb29 as 退货量, rvb35 as 样品否, rvb36 as 仓库, rvb37 as 库位, rvb38 as 批号,case when instr(ima021,'ROHS')>0 then 'ROHS' end as ROHS from hzdb.rva_file left join hzdb.rvb_file on rva01 = rvb01 left join hzdb.gen_file on rvaud02 = gen01 left join hzdb.pmc_file on pmc01 = rva05 left join (hzdb.pmm_file left join hzdb.pma_file on pma01 = pmm20) on pmm01 = rva02 left join hzdb.pmn_file on(pmn01= rvb04 and pmn02 = rvb03) left join hzdb.ima_file on rvb05 = ima01 where rva01 = '" + receivingNumber + "' or rvb04 = '" + purchaseNumber + "'";
                    }
                    else if (!string.IsNullOrEmpty(receivingNumber))
                    {
                        sql = "select rvaud02||'/'||gen02 as 收料员, rva01 as 收货单号, rva05||pmc03 as 供应厂商, rva06 as 收货日期, rva02 as 采购单单号, pmm20||'/'|| pma02 as 付款方式, rva08 as 备注, rvb01 as 项次行序, rvb05 as 料件编号, pmn041 || ',' || ima021 as 品名规格, rvb04 || ',' || rvb03 as 采购单号项次, rvb07 as 实收数量,pmn07 as 单位,(select pmk12 from pmk_file where pmk01 = pmn24)|| '-' || (select gen02 from gen_file where gen01 = (select pmk12 from pmk_file where pmk01 = pmn24)) as 请购人,rvb36 || '-' || rvb37 || '-' || rvb38 as 仓储批, pmn24 || '-' || pmn25 as 请购单号项次,pmn122 as 合同编号, ima021 as 规格, pmn041 as 采购料件编号,pmn041 as 采购料件品名, ima021 as 采购料件规格,pmn20 as 采购量, (pmn50 - pmn55) as 已收量, rvb30 as 入库量, rvb31 as 可入库量, rvb29 as 退货量, rvb35 as 样品否, rvb36 as 仓库, rvb37 as 库位, rvb38 as 批号 from hzdb.rva_file left join hzdb.rvb_file on rva01 = rvb01 left join hzdb.gen_file on rvaud02 = gen01 left join hzdb.pmc_file on pmc01 = rva05 left join (hzdb.pmm_file left join hzdb.pma_file on pma01 = pmm20) on pmm01 = rva02 left join hzdb.pmn_file on(pmn01= rvb04 and pmn02 = rvb03) left join hzdb.ima_file on rvb05 = ima01 where rva01 = '" + receivingNumber + "'";
                        //增加物料类型字段ROHS
                        sql1 = "select rvaud02||'/'||gen02 as 收料员, rva01 as 收货单号, rva05||pmc03 as 供应厂商, rva06 as 收货日期, rva02 as 采购单单号, pmm20||'/'|| pma02 as 付款方式, rva08 as 备注, rvb01 as 项次行序, rvb05 as 料件编号, pmn041 || ',' || ima021 as 品名规格, rvb04 || ',' || rvb03 as 采购单号项次, rvb07 as 实收数量,pmn07 as 单位,(select pmk12 from pmk_file where pmk01 = pmn24)|| '-' || (select gen02 from gen_file where gen01 = (select pmk12 from pmk_file where pmk01 = pmn24)) as 请购人,rvb36 || '-' || rvb37 || '-' || rvb38 as 仓储批, pmn24 || '-' || pmn25 as 请购单号项次,pmn122 as 合同编号, ima021 as 规格, pmn041 as 采购料件编号,pmn041 as 采购料件品名, ima021 as 采购料件规格,pmn20 as 采购量, (pmn50 - pmn55) as 已收量, rvb30 as 入库量, rvb31 as 可入库量, rvb29 as 退货量, rvb35 as 样品否, rvb36 as 仓库, rvb37 as 库位, rvb38 as 批号,case when instr(ima021,'ROHS')>0 then 'ROHS' end as ROHS from hzdb.rva_file left join hzdb.rvb_file on rva01 = rvb01 left join hzdb.gen_file on rvaud02 = gen01 left join hzdb.pmc_file on pmc01 = rva05 left join (hzdb.pmm_file left join hzdb.pma_file on pma01 = pmm20) on pmm01 = rva02 left join hzdb.pmn_file on(pmn01= rvb04 and pmn02 = rvb03) left join hzdb.ima_file on rvb05 = ima01 where rva01 = '" + receivingNumber + "'";
                    }
                    else if (!String.IsNullOrEmpty(purchaseNumber))
                    {
                        sql = "select rvaud02||'/'||gen02 as 收料员, rva01 as 收货单号, rva05||pmc03 as 供应厂商, rva06 as 收货日期, rva02 as 采购单单号, pmm20||'/'|| pma02 as 付款方式, rva08 as 备注, rvb01 as 项次行序, rvb05 as 料件编号, pmn041 || ',' || ima021 as 品名规格, rvb04 || ',' || rvb03 as 采购单号项次, rvb07 as 实收数量,pmn07 as 单位,(select pmk12 from pmk_file where pmk01 = pmn24)|| '-' || (select gen02 from gen_file where gen01 = (select pmk12 from pmk_file where pmk01 = pmn24)) as 请购人,rvb36 || '-' || rvb37 || '-' || rvb38 as 仓储批, pmn24 || '-' || pmn25 as 请购单号项次,pmn122 as 合同编号, ima021 as 规格, pmn041 as 采购料件编号,pmn041 as 采购料件品名, ima021 as 采购料件规格,pmn20 as 采购量, (pmn50 - pmn55) as 已收量, rvb30 as 入库量, rvb31 as 可入库量, rvb29 as 退货量, rvb35 as 样品否, rvb36 as 仓库, rvb37 as 库位, rvb38 as 批号 from hzdb.rva_file left join hzdb.rvb_file on rva01 = rvb01 left join hzdb.gen_file on rvaud02 = gen01 left join hzdb.pmc_file on pmc01 = rva05 left join (hzdb.pmm_file left join hzdb.pma_file on pma01 = pmm20) on pmm01 = rva02 left join hzdb.pmn_file on(pmn01= rvb04 and pmn02 = rvb03) left join hzdb.ima_file on rvb05 = ima01 where rvb04 = '" + purchaseNumber + "'";
                        //增加物料类型字段ROHS
                        sql1 = "select rvaud02||'/'||gen02 as 收料员, rva01 as 收货单号, rva05||pmc03 as 供应厂商, rva06 as 收货日期, rva02 as 采购单单号, pmm20||'/'|| pma02 as 付款方式, rva08 as 备注, rvb01 as 项次行序, rvb05 as 料件编号, pmn041 || ',' || ima021 as 品名规格, rvb04 || ',' || rvb03 as 采购单号项次, rvb07 as 实收数量,pmn07 as 单位,(select pmk12 from pmk_file where pmk01 = pmn24)|| '-' || (select gen02 from gen_file where gen01 = (select pmk12 from pmk_file where pmk01 = pmn24)) as 请购人,rvb36 || '-' || rvb37 || '-' || rvb38 as 仓储批, pmn24 || '-' || pmn25 as 请购单号项次,pmn122 as 合同编号, ima021 as 规格, pmn041 as 采购料件编号,pmn041 as 采购料件品名, ima021 as 采购料件规格,pmn20 as 采购量, (pmn50 - pmn55) as 已收量, rvb30 as 入库量, rvb31 as 可入库量, rvb29 as 退货量, rvb35 as 样品否, rvb36 as 仓库, rvb37 as 库位, rvb38 as 批号,case when instr(ima021,'ROHS')>0 then 'ROHS' end as ROHS from hzdb.rva_file left join hzdb.rvb_file on rva01 = rvb01 left join hzdb.gen_file on rvaud02 = gen01 left join hzdb.pmc_file on pmc01 = rva05 left join (hzdb.pmm_file left join hzdb.pma_file on pma01 = pmm20) on pmm01 = rva02 left join hzdb.pmn_file on(pmn01= rvb04 and pmn02 = rvb03) left join hzdb.ima_file on rvb05 = ima01 where rvb04 = '" + purchaseNumber + "'";
                    }
                    OracleCommand cmd = new OracleCommand(sql1, cn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(OutDataTable);
                    cmd.Dispose();
                    foreach (DataRow dataRow in OutDataTable.Rows)
                    {
                        rva_rvb_file record = new rva_rvb_file();
                        record.rvaud02 = dataRow["收料员"].ToString();   //收料员
                        record.rva01 = dataRow["收货单号"].ToString();   //收货单号
                        record.rva05 = dataRow["供应厂商"].ToString();   //供应厂商
                        record.MaterialType = dataRow["ROHS"].ToString();   //物料类型
                        record.rva06 = Convert.ToDateTime(dataRow["收货日期"].ToString());   //收货日期
                        record.rvb04 = dataRow["采购单单号"].ToString();   //采购单单号
                        record.pay = dataRow["付款方式"].ToString();   //付款方式
                        record.rva08 = dataRow["备注"].ToString();   //备注
                        record.rvb01 = dataRow["项次行序"].ToString();   //项次行序
                        record.rvb05 = dataRow["料件编号"].ToString();   //料件编号
                        record.ima021 = dataRow["品名规格"].ToString();   //品名规格
                        record.rvb03 = dataRow["采购单号项次"].ToString();   //采购单号项次
                        record.rvb07 = Convert.ToDouble(dataRow["实收数量"].ToString());   //实收数量
                        record.pmn07 = dataRow["单位"].ToString();   //单位
                        record.pmk12 = dataRow["请购人"].ToString();  //请购人
                        record.rvb36 = dataRow["仓储批"].ToString();   //仓储批
                        record.pmn24 = dataRow["请购单号项次"].ToString();   //请购单号项次
                        record.pmn122 = dataRow["合同编号"].ToString();   //合同编号
                        record.specification = dataRow["规格"].ToString();   //规格
                        record.pmn041 = dataRow["采购料件编号"].ToString();
                        record.pmn0411 = dataRow["采购料件品名"].ToString();
                        record.purchase_specification = dataRow["采购料件规格"].ToString();
                        record.pmn20 = Convert.ToDouble(dataRow["采购量"].ToString());
                        record.pmn50_pmn55 = Convert.ToDouble(dataRow["已收量"].ToString());
                        record.rvb30 = Convert.ToDouble(dataRow["入库量"].ToString());
                        record.rvb31 = Convert.ToDouble(dataRow["可入库量"].ToString());
                        record.rvb29 = Convert.ToDouble(dataRow["退货量"].ToString());
                        record.rvb35 = dataRow["样品否"].ToString() == "N" ? false : true;
                        record.rvb366 = dataRow["仓库"].ToString();
                        record.rvb377 = dataRow["库位"].ToString();
                        record.rvb388 = dataRow["批号"].ToString();
                        result_list.Add(record);
                    }
                    cn.Dispose();
                    cn.Close();
                }
                return result_list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        //财务明细表查询
        public static List<cxcr006> ERP_FinanceDetialsQuery(int year, int month)
        {
            string data = string.Empty;
            List<cxcr006> result_list = new List<cxcr006>();
            DataTable OutDataTable = new DataTable();
            OutDataTable.TableName = "data";
            try
            {
                using (OracleConnection cn = GetOracleConnection())
                {
                    cn.Open();
                    //加上会计科目  --ima39为料号对应会计科目--0为面积
                    string sql = "SELECT tc_cxc01,tc_cxc02,tc_cxc03,tc_cxc04,tc_cxc05,tc_cxc06,tc_cxc07,tc_cxc08,tc_cxc09,tc_cxc10,tc_cxc11, tc_cxc12,tc_cxc13,tc_cxc14,tc_cxc15,tc_cxc16,ima39,0 FROM tc_cxc_file,ima_file where tc_cxc01 = ima01 and tc_cxc15 ='"+year+"' and tc_cxc16 ='"+month+"' ORDER BY tc_cxc01";

                    OracleCommand cmd = new OracleCommand(sql, cn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(OutDataTable);
                    cmd.Dispose();
                    foreach (DataRow item in OutDataTable.Rows)
                    {
                        result_list.Add(new cxcr006
                        {
                            tc_cxc01 = item.ItemArray[0].ToString(),                            //料号
                            tc_cxc02 = item.ItemArray[1].ToString(),                            //品名
                            tc_cxc03 = item.ItemArray[2].ToString(),                            //规格
                            tc_cxc04 = Convert.ToDouble(item.ItemArray[3].ToString()),          //本月结存数
                            tc_cxc05 = Convert.ToDouble(item.ItemArray[4].ToString()),          //本月结存单价
                            tc_cxc06 = Convert.ToDouble(item.ItemArray[5].ToString()),          //本月结存金额
                            tc_cxc07 = Convert.ToDouble(item.ItemArray[6].ToString()),          //30天
                            tc_cxc08 = Convert.ToDouble(item.ItemArray[7].ToString()),          //90天
                            tc_cxc09 = Convert.ToDouble(item.ItemArray[8].ToString()),          //180天
                            tc_cxc10 = Convert.ToDouble(item.ItemArray[9].ToString()),          //365天
                            tc_cxc11 = Convert.ToDouble(item.ItemArray[10].ToString()),          //2年
                            tc_cxc12 = Convert.ToDouble(item.ItemArray[11].ToString()),          //3年
                            tc_cxc13 = Convert.ToDouble(item.ItemArray[12].ToString()),          //3-5年
                            tc_cxc14 = Convert.ToDouble(item.ItemArray[13].ToString()),          //5年以上
                            tc_cxc15 = Convert.ToInt32(item.ItemArray[14].ToString()),           //年份
                            tc_cxc16 = Convert.ToInt32(item.ItemArray[15].ToString()),           //年份
                            AccountingSubject = Convert.ToInt32(item[16].ToString()),            //会计科目
                            area = Convert.ToDouble(item.ItemArray[17].ToString()),              //面积
                            Classification = Classify(item.ItemArray[2].ToString()),             //定义分类
                        });
                    }
                    cn.Dispose();
                    cn.Close();
                }
                return result_list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //月底厂外仓记录查询
        public static List<imk_file> OutsideWarehouseQuery(int year, int month)
        {
            string data = string.Empty;
            List<imk_file> result_list = new List<imk_file>();
            DataTable OutDataTable = new DataTable();
            OutDataTable.TableName = "data";
            try
            {
                using (OracleConnection cn = GetOracleConnection())
                {
                    cn.Open();
                    //加入期末余额计算
                    string sql = "select distinct imk01, imk02, imk03, imk04, imk05, imk06, imk09, imk09*ccc23 from imk_file, ccc_file where imk02 = 'LCWC1' and imk05 = " + year + " and imk06 = " + month + " and imk09<>0 and imk01 = ccc01(+)  and imk05 = " + year + " and imk06 = " + month + " order by imk01";

                    OracleCommand cmd = new OracleCommand(sql, cn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(OutDataTable);
                    cmd.Dispose();
                    foreach (DataRow dataRow in OutDataTable.Rows)
                    {
                        imk_file record = new imk_file();
                        record.imk01 = dataRow["imk01"].ToString();
                        record.imk02 = dataRow["imk02"].ToString();
                        record.imk03 = dataRow["imk03"].ToString();
                        record.imk04 = dataRow["imk04"].ToString();
                        record.imk05 = dataRow["imk05"].ToString() == "" ? 0 : Convert.ToInt16(dataRow["imk05"].ToString());
                        record.imk06 = dataRow["imk06"].ToString() == "" ? 0 : Convert.ToInt16(dataRow["imk06"].ToString());
                        //record.imk081 = dataRow["imk081"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["imk081"].ToString());
                        //record.imk082 = dataRow["imk082"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["imk082"].ToString());
                        //record.imk083 = dataRow["imk083"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["imk083"].ToString());
                        //record.imk084 = dataRow["imk084"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["imk084"].ToString());
                        //record.imk085 = dataRow["imk085"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["imk085"].ToString());
                        //record.imk086 = dataRow["imk086"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["imk086"].ToString());
                        //record.imk087 = dataRow["imk087"].ToString() == "" ? 0 : Convert.ToInt16(dataRow["imk087"].ToString());
                        //record.imk088 = dataRow["imk088"].ToString() == "" ? 0 : Convert.ToInt16(dataRow["imk088"].ToString());
                        //record.imk089 = dataRow["imk089"].ToString() == "" ? 0 : Convert.ToInt16(dataRow["imk089"].ToString());
                        record.imk09 = dataRow["imk09"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["imk09"].ToString());
                        record.sum = dataRow["imk09*ccc23"].ToString() == "" ? 0 : Convert.ToDouble(dataRow["imk09*ccc23"].ToString());
                        //record.imkplant = dataRow["imkplant"].ToString();
                        //record.imklegal = dataRow["imklegal"].ToString();
                        result_list.Add(record);
                    }
                    cn.Dispose();
                    cn.Close();
                    return result_list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //MC未发料表查询
        public static List<csfr008> ERP_MC_NuIssueDetialsQuery(DateTime inputdate, DateTime enddate)
        {
            string data = string.Empty;
            List<csfr008> result_list = new List<csfr008>();
            DataTable OutDataTable = new DataTable();
            OutDataTable.TableName = "data";
            try
            {
                using (OracleConnection cn = GetOracleConnection())
                {
                    cn.Open();

                    //杨军提供
                    string inputdatestring = string.Format("{0:yy-MM-dd}", inputdate);
                    string enddatestring = string.Format("{0:yy-MM-dd}", enddate);
                    string sql = "select sfb01 ,sfbud01 ,sfb81 ,sfb08 ,sfb09 ,sfa03 ,ima02 ,ima021 ,sfa12 ,sfa05 ,sfa06 FROM  sfb_file,sfa_file,ima_file WHERE((sfb01 in (SELECT sfb01 FROM  sfb_file WHERE  sfb04 <> '8' and sfb87 <> 'X')) AND sfb81 <= to_date('" + inputdatestring + "', 'YY-MM-DD') OR(sfb01 in (SELECT sfb01 FROM  sfb_file WHERE  sfb04 = '8' and sfb38 > to_date('" + enddatestring + "', 'YY-MM-DD')))) AND  sfb01=sfa01 AND  sfa03=ima01 ORDER BY sfb81";
                    //  sfb81：订单录入日期
                    //  sfb38：订单结案日期

                    OracleCommand cmd = new OracleCommand(sql, cn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(OutDataTable);
                    cmd.Dispose();
                    foreach (DataRow dataRow in OutDataTable.Rows)
                    {
                        csfr008 record = new csfr008();
                        record.sfb01 = dataRow["sfb01"].ToString();                                  //工单号
                        record.sfbud01 = dataRow["sfbud01"].ToString();                              //工单备注
                        record.sfb81 = Convert.ToDateTime(dataRow["sfb81"].ToString());              //工单录入日期
                        record.sfb08 = Convert.ToDouble(dataRow["sfb08"].ToString());                //生产数量
                        record.sfb09 = Convert.ToDouble(dataRow["sfb09"].ToString());                //入库数量
                        record.sfa03 = dataRow["sfa03"].ToString();                                  //料号
                        record.ima02 = dataRow["ima02"].ToString();                                  //品名
                        record.ima021 = dataRow["ima021"].ToString();                               //规格
                        record.sfa12 = dataRow["sfa12"].ToString();                                  //单位
                        record.sfa05 = Convert.ToDouble(dataRow["sfa05"].ToString());               //应发数量
                        record.sfa06 = Convert.ToDouble(dataRow["sfa06"].ToString());                //已发数量
                        record.sfa05_sfa06 = record.sfa05 - record.sfa06;    //未发数量
                        result_list.Add(record);
                    }
                    cn.Dispose();
                    cn.Close();
                    return result_list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //MC未发料表(修改后方案)
        public static List<tc_cxd_file> ERP_MC_NuIssueDetialsQuery2(int year,int month)
        {
            string data = string.Empty;
            List<tc_cxd_file> result_list = new List<tc_cxd_file>();
            DataTable OutDataTable = new DataTable();
            OutDataTable.TableName = "data";
            try
            {
                using (OracleConnection cn = GetOracleConnection())
                {
                    cn.Open();
                    ////杨军提供
                    //string inputdatestring = string.Format("{0:yy-MM-dd}", inputdate);
                    //string enddatestring = string.Format("{0:yy-MM-dd}", enddate);
                    string sql = "select * FROM tc_cxd_file where tc_cxd14 = '" + year+"' and tc_cxd15 = '" + month +"'";
                    //  sfb81：订单录入日期
                    //  sfb38：订单结案日期
                    //tc_cxd14(年)、tc_cxd15（月）
                    OracleCommand cmd = new OracleCommand(sql, cn);
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(OutDataTable);
                    cmd.Dispose();
                    foreach (DataRow dataRow in OutDataTable.Rows)
                    {
                        tc_cxd_file record = new tc_cxd_file();
                        record.tc_cxd01 = dataRow["tc_cxd01"].ToString();                                  //工单号
                        record.tc_cxd02 = dataRow["tc_cxd02"].ToString();                                  //工单备注
                        record.tc_cxd03 = Convert.ToDateTime(dataRow["tc_cxd03"].ToString());              //工单录入日期
                        record.tc_cxd04 = Convert.ToDouble(dataRow["tc_cxd04"].ToString());                //生产数量
                        record.tc_cxd05 = Convert.ToDouble(dataRow["tc_cxd05"].ToString());                //入库数量
                        record.tc_cxd06 = dataRow["tc_cxd06"].ToString();                                  //料号
                        record.tc_cxd07 = dataRow["tc_cxd07"].ToString();                                  //品名
                        record.tc_cxd08 = dataRow["tc_cxd08"].ToString();                                  //规格
                        record.tc_cxd09 = dataRow["tc_cxd09"].ToString();                                  //单位
                        record.tc_cxd10 = Convert.ToDouble(dataRow["tc_cxd10"].ToString());                //应发数量
                        record.tc_cxd11 = Convert.ToDouble(dataRow["tc_cxd11"].ToString());                //已发数量
                        record.tc_cxd10_tc_cxd11 = record.tc_cxd10 - record.tc_cxd11;                      //未发数量
                        record.tc_cxd14 = Convert.ToInt32(dataRow["tc_cxd14"].ToString());                 //年份
                        record.tc_cxd15 = Convert.ToInt32(dataRow["tc_cxd15"].ToString());                 //月份
                        record.ordertype = Classify(record.tc_cxd02);                                      //定义订单类型
                        result_list.Add(record);
                    }
                    cn.Dispose();
                    cn.Close();
                    return result_list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //定义类型
        public static string Classify(string tc_cxd02)
        {
            tc_cxd02 = tc_cxd02.Length>30?tc_cxd02.Substring(0, 30):tc_cxd02;
            if (tc_cxd02.Contains("-K")|| tc_cxd02.Contains("备库")|| tc_cxd02.Contains("-RD")) return "1.公司常规备库单、2.投标备库单";  //1.-K或备库或-RD”
            if (tc_cxd02.Contains("-J"))  return "3.业务借样单";
            if (tc_cxd02.Contains("-Q"))  return "4.工厂品质单";
            if (tc_cxd02.Contains("-RM")) return "5.公司展厅单、6.各大区展厅单";
            if (tc_cxd02.Contains("-R"))  return "7.研发样品单";
            if (tc_cxd02.Contains("-C"))  return "8.公司参展单";
            if (tc_cxd02.Contains("P1-") || tc_cxd02.Contains("P2-") || tc_cxd02.Contains("P3-") || tc_cxd02.Contains("P4-") || tc_cxd02.Contains("P5-") || tc_cxd02.Contains("P6-") || tc_cxd02.Contains("P7-") || tc_cxd02.Contains("P8-") || tc_cxd02.Contains("P9-") || tc_cxd02.Contains("P10-")) return "9.品质客诉单";
            if (tc_cxd02.Contains("B1-") || tc_cxd02.Contains("B2-") || tc_cxd02.Contains("B3-") || tc_cxd02.Contains("B4-") || tc_cxd02.Contains("B5-") || tc_cxd02.Contains("B6-") || tc_cxd02.Contains("B7-") || tc_cxd02.Contains("B8-") || tc_cxd02.Contains("B9-") || tc_cxd02.Contains("B10-")) return "10.出货后增加物料单";
            if (tc_cxd02.Contains("-H")) return "11.易事达订单";
            if (tc_cxd02.Contains("-Y")|| tc_cxd02.Contains("-G") || tc_cxd02.Contains("-A") || tc_cxd02.Contains("-LA")) return "12.销售订单";
            if (tc_cxd02.Contains("-S")) return "13.费用挂大区单及赠送单";
            else return "14.其他订单";
        }

        //在制工单
        public static List<ccg_file> ERP_Work_in_process(int year_v, int month_v)
        {
            string sql = "SELECT ccg01,ccg02,ccg03,ccg04,ccg91,ccg92, ima12, ima02, ima021, ima911, sfb99, sfbud01 FROM ccg_file , ima_file , sfb_file WHERE ccg04= ima01  AND ccg01 = sfb01 AND ccg02 = " + year_v + " AND ccg03 = " + month_v;
            string data = string.Empty;
            DataTable OutDataTable = new DataTable();
            OutDataTable.TableName = "data";
            List<ccg_file> result_list = new List<ccg_file>();
            using (OracleConnection cn = GetOracleConnection())
            {
                cn.Open();
                OracleCommand cmd = new OracleCommand(sql, cn);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                da.Fill(OutDataTable);
                cmd.Dispose();
                foreach (DataRow dataRow in OutDataTable.Rows)
                {
                    ccg_file record = new ccg_file();
                    record.ccg01 = dataRow["ccg01"].ToString();                        //工单号
                    record.ccg04 = dataRow["ccg04"].ToString();                        //主件料号
                    //record.ccg11  = Convert.ToDouble(dataRow["ccg11"].ToString());     //上月结存数量
                    //record.ccg12  = Convert.ToDouble(dataRow["ccg12"].ToString());     //上月结存金额
                    record.ccg01_yearmonth = Convert.ToInt16(record.ccg01.Substring(5,4));              //工单年月份
                    int year = 2000 + Convert.ToInt16(record.ccg01.Substring(5, 2));
                    int month = Convert.ToInt16(record.ccg01.Substring(7, 2));
                    //上月年月值;
                    record.ccg02  = Convert.ToInt16(dataRow["ccg02"].ToString());      //年度 
                    record.ccg03  = Convert.ToInt16(dataRow["ccg03"].ToString());      //月份
                    int result_months = (record.ccg02 - year)*12+(record.ccg03 - month);
                    record.durations = result_months <1? "1个月以内":result_months < 3 ? "2-3个月" : result_months < 6 ? "4-6个月" : result_months < 12 ? "7-12个月" : result_months<24? "1-2年": result_months <36?"2-3年": result_months < 60 ?"3-5年": "5年以上";                                             //期间
                    //record.ccg12a = Convert.ToDouble(dataRow["ccg12a"].ToString());
                    //record.ccg12b = Convert.ToDouble(dataRow["ccg12b"].ToString());
                    //record.ccg12c = Convert.ToDouble(dataRow["ccg12c"].ToString());
                    //record.ccg12d = Convert.ToDouble(dataRow["ccg12d"].ToString());
                    //record.ccg12e = Convert.ToDouble(dataRow["ccg12e"].ToString());
                    //record.ccg20  = Convert.ToDouble(dataRow["ccg20"].ToString());
                    //record.ccg21  = Convert.ToDouble(dataRow["ccg21"].ToString());
                    //record.ccg22  = Convert.ToDouble(dataRow["ccg22"].ToString());
                    //record.ccg22a = Convert.ToDouble(dataRow["ccg22a"].ToString());
                    //record.ccg22b = Convert.ToDouble(dataRow["ccg22b"].ToString());
                    //record.ccg22c = Convert.ToDouble(dataRow["ccg22c"].ToString());
                    //record.ccg22d = Convert.ToDouble(dataRow["ccg22d"].ToString());
                    //record.ccg22e = Convert.ToDouble(dataRow["ccg22e"].ToString());
                    //record.ccg23  = Convert.ToDouble(dataRow["ccg23"].ToString());
                    //record.ccg23a = Convert.ToDouble(dataRow["ccg23a"].ToString());
                    //record.ccg23b = Convert.ToDouble(dataRow["ccg23b"].ToString());
                    //record.ccg23c = Convert.ToDouble(dataRow["ccg23c"].ToString());
                    //record.ccg23d = Convert.ToDouble(dataRow["ccg23d"].ToString());
                    //record.ccg23e = Convert.ToDouble(dataRow["ccg23e"].ToString());
                    //record.ccg31  = Convert.ToDouble(dataRow["ccg31"].ToString());
                    //record.ccg32  = Convert.ToDouble(dataRow["ccg32"].ToString());
                    //record.ccg32a = Convert.ToDouble(dataRow["ccg32a"].ToString());
                    //record.ccg32b = Convert.ToDouble(dataRow["ccg32b"].ToString());
                    //record.ccg32c = Convert.ToDouble(dataRow["ccg32c"].ToString());
                    //record.ccg32d = Convert.ToDouble(dataRow["ccg32d"].ToString());
                    //record.ccg32e = Convert.ToDouble(dataRow["ccg32e"].ToString());
                    //record.ccg41  = Convert.ToDouble(dataRow["ccg41"].ToString()); 
                    //record.ccg42  = Convert.ToDouble(dataRow["ccg42"].ToString());
                    //record.ccg42a = Convert.ToDouble(dataRow["ccg42a"].ToString());
                    //record.ccg42b = Convert.ToDouble(dataRow["ccg42b"].ToString());
                    //record.ccg42c = Convert.ToDouble(dataRow["ccg42c"].ToString());
                    //record.ccg42d = Convert.ToDouble(dataRow["ccg42d"].ToString());
                    //record.ccg42e = Convert.ToDouble(dataRow["ccg42e"].ToString());
                    record.ccg91 = Convert.ToDouble(dataRow["ccg91"].ToString());
                    record.ccg92 = Convert.ToDouble(dataRow["ccg92"].ToString());
                    //record.ccg92a = Convert.ToDouble(dataRow["ccg92a"].ToString());
                    //record.ccg92b = Convert.ToDouble(dataRow["ccg92b"].ToString());
                    //record.ccg92c = Convert.ToDouble(dataRow["ccg92c"].ToString());
                    //record.ccg92d = Convert.ToDouble(dataRow["ccg92d"].ToString());
                    //record.ccg92e = Convert.ToDouble(dataRow["ccg92e"].ToString());
                    //record.ccguser = dataRow["ccguser"].ToString();
                    //record.ccgdate = Convert.ToDateTime(dataRow["ccgdate"].ToString());
                    //record.ccgtime = dataRow["ccgtime"].ToString();
                    //record.ccg06 = dataRow["ccg06"].ToString();
                    //record.ccg07 = dataRow["ccg07"].ToString();
                    //record.ccg12f = Convert.ToDouble(dataRow["ccg12f"].ToString());
                    //record.ccg12g = Convert.ToDouble(dataRow["ccg12g"].ToString());
                    //record.ccg12h = Convert.ToDouble(dataRow["ccg12h"].ToString());
                    //record.ccg22f = Convert.ToDouble(dataRow["ccg22f"].ToString());
                    //record.ccg22g = Convert.ToDouble(dataRow["ccg22g"].ToString());
                    //record.ccg22h = Convert.ToDouble(dataRow["ccg22h"].ToString());
                    //record.ccg23f = Convert.ToDouble(dataRow["ccg23f"].ToString());
                    //record.ccg23g = Convert.ToDouble(dataRow["ccg23g"].ToString());
                    //record.ccg23h = Convert.ToDouble(dataRow["ccg23h"].ToString());
                    //record.ccg32f = Convert.ToDouble(dataRow["ccg32f"].ToString());
                    //record.ccg32g = Convert.ToDouble(dataRow["ccg32g"].ToString());
                    //record.ccg32h = Convert.ToDouble(dataRow["ccg32h"].ToString());
                    //record.ccg42f = Convert.ToDouble(dataRow["ccg42f"].ToString());
                    //record.ccg42g = Convert.ToDouble(dataRow["ccg42g"].ToString());
                    //record.ccg42h = Convert.ToDouble(dataRow["ccg42h"].ToString());
                    //record.ccg92f = Convert.ToDouble(dataRow["ccg92f"].ToString());
                    //record.ccg92g = Convert.ToDouble(dataRow["ccg92g"].ToString());
                    //record.ccg92h = Convert.ToDouble(dataRow["ccg92h"].ToString());
                    //record.ccgud01 = dataRow["ccgud01"].ToString();
                    //record.ccgud02 = dataRow["ccgud02"].ToString();
                    //record.ccgud03 = dataRow["ccgud03"].ToString();
                    //record.ccgud04 = dataRow["ccgud04"].ToString();
                    //record.ccgud05 = dataRow["ccgud05"].ToString();
                    //record.ccgud06 = dataRow["ccgud06"].ToString();
                    //record.ccgud07 = Convert.ToDouble(dataRow["ccgud07"].ToString());
                    //record.ccgud08 = Convert.ToDouble(dataRow["ccgud08"].ToString());
                    //record.ccgud09 = Convert.ToDouble(dataRow["ccgud09"].ToString());
                    //record.ccgud10 = Convert.ToInt16(dataRow["ccgud10"].ToString());
                    //record.ccgud11 = Convert.ToInt16(dataRow["ccgud11"].ToString());
                    //record.ccgud12 = Convert.ToInt16(dataRow["ccgud12"].ToString());
                    //record.ccgud13 = Convert.ToDateTime(dataRow["ccgud13"].ToString());
                    //record.ccgud14 = Convert.ToDateTime(dataRow["ccgud14"].ToString());
                    //record.ccgud15 = Convert.ToDateTime(dataRow["ccgud15"].ToString());
                    //record.ccgplant = dataRow["ccgplant"].ToString();
                    //record.ccglegal = dataRow["ccglegal"].ToString();
                    //record.ccgorig = dataRow["ccgorig"].ToString();
                    //record.ccgoriu = dataRow["ccgoriu"].ToString();
                    record.ima02 = dataRow["ima02"].ToString();                              //物料品名
                    record.ima12 = dataRow["ima12"].ToString();                              //其他分群码 四
                    record.ima021 = dataRow["ima021"].ToString();                            //物料规格
                    record.ima911 = dataRow["ima911"].ToString();                            //是否为重覆性生产料件 (Y/N)
                    record.sfb99 = dataRow["sfb99"].ToString();                              //重工否(Y/N)
                    record.sfbud01 = dataRow["sfbud01"].ToString();                          //备注
                    record.Classification = Classify(record.sfbud01);                      //定义分类
                    result_list.Add(record);
                }
                cn.Dispose();
                cn.Close();
            }
            return result_list;
        }

        #endregion

        #region------ 其他方法

        //TryConnect
        public static string TryConnectERP()
        {
            try
            {
                using (OracleConnection cn = GetOracleConnection())
                {
                    cn.Open();
                    cn.Dispose();
                    cn.Close();
                    return "连接正常";
                }
            }
            catch (Exception ex)
            {
                return ex.InnerException.InnerException.Message;
            }
        }
        private static OracleConnection GetOracleConnection()
        {
            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = "User Id=hzdb;Password=hzdb;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.5)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=topprod)))";
            return conn;
        }

        //String转换DateTime
        public static DateTime? StringTODateTime(string str)
        {
            DateTime? result = null;
            if (str == "")
            {
                return result;
            }
            else
            {
                result = Convert.ToDateTime(str);
                return result;
            }
        }
        #endregion

    }
}