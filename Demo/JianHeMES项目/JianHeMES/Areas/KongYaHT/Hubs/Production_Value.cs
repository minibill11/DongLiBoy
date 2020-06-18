﻿using JianHeMES.Controllers;
using JianHeMES.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace JianHeMES.Areas.KongYaHT.Hubs
{ 
    #region ---------------------------------------------------------------------

    [HubName("Production_Value")]
    public class Production_Value : Hub
    {
        // Is set via the constructor on each creation
        private Production_ValueBroadcaster ProductionControlIndexBroadcaster_broadcaster;
        public Production_Value()
            : this(Production_ValueBroadcaster.ProductionControlIndexInstance)
        {
        }
        public Production_Value(Production_ValueBroadcaster ProductionControlIndexbroadcaster)
        {
            ProductionControlIndexBroadcaster_broadcaster = ProductionControlIndexbroadcaster;
        }
    }

    public class Temp_BasicInfo
    {
        public int Quantity { get; set; }

        public string OrderNum { get; set; }
    }

    public class Temp_Warehouse
    {
        public string OrderNum { get; set; }
        public string OuterBoxBarcode { get; set; }
        public string BarCodeNum { get; set; }
        public DateTime? Date { get; set; }
        public bool IsOut { get; set; }
        public DateTime? WarehouseOutDate { get; set; }
        public string NewBarcode { get; set; }
        public int WarehouseOutNum { get; set; }
        public string Cordernum { get; set; }
        public string state { get; set; }
    }
    /// <summary>
    /// 数据广播器
    /// </summary>
    public class Production_ValueBroadcaster
    {
        private readonly static Lazy<Production_ValueBroadcaster> ProductionControlIndex_instance =
            new Lazy<Production_ValueBroadcaster>(() => new Production_ValueBroadcaster());

        private readonly IHubContext ProductionControlIndex_hubContext;

        private Timer ProductionControlIndex_broadcastLoop;


        public Production_ValueBroadcaster()
        {
            // 获取所有连接的句柄，方便后面进行消息广播
            ProductionControlIndex_hubContext = GlobalHost.ConnectionManager.GetHubContext<Production_Value>();
            // Start the broadcast loop
            ProductionControlIndex_broadcastLoop = new Timer(
                ProductionControlIndexBroadcastShape,
                null,
                0,
                20000);
        }

        private void ProductionControlIndexBroadcastShape(object state)
        {

            JObject Production_Value = QueryExcution();
            //广播发送JSON数据
            ProductionControlIndex_hubContext.Clients.All.sendProductionControlIndex(Production_Value);
        }

        public static Production_ValueBroadcaster ProductionControlIndexInstance
        {
            get
            {
                return ProductionControlIndex_instance.Value;
            }
        }


        #region-----------查询数据方法

        public JObject QueryExcution()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            CommonalityController comm = new CommonalityController();
            CommonController common = new CommonController();
            JObject value =new  JObject();
            JObject total = new JObject();
            var yesterday = DateTime.Now.AddDays(-1);

            var temp_BasicInfo = from c in db.Packing_BasicInfo select new Temp_BasicInfo { OrderNum = c.OrderNum, Quantity = c.Quantity };
            var temp_Warehouse = from c in db.Warehouse_Join select new Temp_Warehouse { OrderNum = c.OrderNum, Date = c.Date, OuterBoxBarcode = c.OuterBoxBarcode,IsOut=c.IsOut,WarehouseOutDate=c.WarehouseOutDate,NewBarcode=c.NewBarcode,WarehouseOutNum=c.WarehouseOutNum, BarCodeNum = c.BarCodeNum,Cordernum=c.CartonOrderNum, state = c.State };

            var productionOrder = temp_BasicInfo.Select(c => c.OrderNum).Distinct().ToList();
            int i = 0;
            List<string> first = new List<string>();
            List<string> second = new List<string>();
            List<string> three = new List<string>();
            //排序
            foreach (var order in productionOrder)
            {
                if (temp_Warehouse.Count(c => c.OrderNum == order && c.IsOut == true && c.state == "模组") > 0)
                { first.Add(order); }
                else if (temp_Warehouse.Count(c => c.OrderNum == order && c.IsOut == false && c.state == "模组") > 0)
                { second.Add(order); }
                else
                { three.Add(order); }   
            }
            productionOrder.Clear();
            productionOrder.AddRange(first);
            productionOrder.AddRange(second);
            productionOrder.AddRange(three);

            foreach (var item in productionOrder)
            {
                #region 查找当前出入库的情况
                var temresult = common.GetCurrentwarehousList(item);
                
                var warehousJoincount = temresult.Where(c=>c.CartonOrderNum==item || c.NewBarcode == item).Select(c => c.BarCodeNum).Distinct().Count();
                var bigJoincount = temresult.Where(c => c.CartonOrderNum == item || c.NewBarcode == item).Select(c => c.OuterBoxBarcode).Distinct().Count();
                var warehousOutcount = temresult.Where(c => c.IsOut == true&& (c.CartonOrderNum == item || c.NewBarcode == item)).Select(c => c.BarCodeNum).Distinct().Count();
                var bigOutcount = temresult.Where(c => c.IsOut == true&& (c.CartonOrderNum == item || c.NewBarcode == item)).Select(c => c.OuterBoxBarcode).Distinct().Count();
                #endregion
                var baseinfo = temp_BasicInfo.Where(c => c.OrderNum == item).Sum(c => c.Quantity);
                if (bigOutcount == baseinfo)
                {
                    var lastDate = temp_Warehouse.Where(c => c.IsOut == true && c.OrderNum == item && c.state == "模组").Max(c => c.WarehouseOutDate);
                    if (lastDate <= yesterday)
                    {
                        continue;
                    }
                }
                var productionvalue = db.Production_Value.Where(c => c.OrderNum == item).Select(c=>new { c.Id,c.Worth,c.Remark }).FirstOrDefault();
                
                //订单号
                value.Add("OrderNum", item);
                var moduleCount = db.OrderMgm.Where(c => c.OrderNum == item).Select(c => c.Boxes).FirstOrDefault();
                value.Add("moduleCount", moduleCount);
                //包装件数
                // var quantity = db.Packing_BasicInfo.Where(c => c.OrderNum == item).Sum(c => c.Quantity);
                //已包装数量
                var modulecount = db.Packing_BarCodePrinting.Count(c => c.CartonOrderNum == item && c.QC_Operator == null);
                var outhercount = db.Packing_BarCodePrinting.Where(c => c.CartonOrderNum == item && c.QC_Operator == null).Select(c => c.OuterBoxBarcode).Distinct().Count();
                value.Add("packingCount", modulecount + "(" + outhercount + "/" + baseinfo.ToString() + ")");

                
                //已入库数量.Count(c => c.OrderNum == item)
                value.Add("warehousJoinCount", warehousJoincount + "(" + bigJoincount + "/" + baseinfo.ToString() + ")");
                //已出库数量
                //warehousOutcount = temresult.Where(c => ).Select(c => c.BarCodeNum).Distinct().Count();
                value.Add("warehousOutCount", warehousOutcount + "(" + bigOutcount + "/" + baseinfo.ToString() + ")");

                //库存数量
                //var outheCount = temresult.Where(c => c.OrderNum == item && c.Date != null).Select(c => c.OuterBoxBarcode).Distinct().Count() - temresult.Where(c => c.OrderNum == item && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();
                var stockCount = (warehousJoincount - warehousOutcount).ToString() + "(" + (bigJoincount - bigOutcount) + ")";
                value.Add("stockCount", stockCount);

                //挪用信息
                JArray nuoInfo = new JArray();
                var NewBarcode = temp_Warehouse.Where(c => c.OrderNum == item&&c.IsOut == true && c.NewBarcode != null&&c.NewBarcode!=" " && c.NewBarcode != item && c.state == "模组").Select(c => c.NewBarcode).Distinct().ToList();
                foreach (var Barcode in NewBarcode)
                {
                    var ordernum = temp_Warehouse.Where(c => c.OrderNum == item && c.IsOut == true && c.NewBarcode == Barcode && c.state == "模组").Select(c => new { c.NewBarcode, c.OuterBoxBarcode }).ToList();
                    nuoInfo.Add("出库到" + ordernum.FirstOrDefault().NewBarcode + "订单" + ordernum.Select(c => c.OuterBoxBarcode).Distinct().Count() + "箱(" + ordernum.Count + "件);");
                }
                var NewBarcode2 = temp_Warehouse.Where(c => c.NewBarcode == item && c.OrderNum!=c.NewBarcode&& c.IsOut == true && c.state == "模组").Select(c => c.OrderNum).Distinct().ToList();
                foreach (var Barcode in NewBarcode2)
                {
                    var ordernum = temp_Warehouse.Where(c => c.OrderNum == Barcode && c.IsOut == true && c.NewBarcode == item && c.state == "模组").Select(c => new { c.OrderNum, c.OuterBoxBarcode }).ToList();
                    nuoInfo.Add("从" + ordernum.FirstOrDefault().OrderNum + "订单挪了" + ordernum.Select(c => c.OuterBoxBarcode).Distinct().Count() + "箱(" + ordernum.Count + "件);");
                }
                

                value.Add("nuoInfo", nuoInfo);
                //入库完成率
                 var current = temresult.Where(c=>c.OrderNum==item || c.NewBarcode == item).Select(c => c.BarCodeNum).Distinct().Count();
                //warehousJoincount = warehousJoincount - current;
                var warehousJoinComplete = (moduleCount == 0 ? 0 : (decimal)(current * 100) / moduleCount).ToString("F2") + "%";
                value.Add("warehousJoinComplete", warehousJoinComplete);
                //出库完成率
                //warehousOutcount = warehousOutcount - current;
                var currentout = temresult.Where(c => c.IsOut == true&& (c.OrderNum == item || c.NewBarcode == item)).Select(c => c.BarCodeNum).Distinct().Count();
                var warehousOutComplete = (moduleCount == 0 ? 0 : (decimal)(currentout * 100) / moduleCount).ToString("F2") + "%";
                value.Add("warehousOutComplete", warehousOutComplete);

                if (productionvalue == null)
                {
                    value.Add("id", 0);
                    //总产值
                    value.Add("Worth","- -");
                    //目前入库产值
                    value.Add("warehouseJoinValue", "- -");
                    //未完成产值
                    value.Add("uncompleteValue", "- -");
                    //备注
                    value.Add("remark", "");
                }
                else
                {
                    value.Add("id", productionvalue.Id);
                    //总产值
                    value.Add("Worth", productionvalue.Worth);
                    //目前入库产值
                    var warehouseJoinValue = warehousJoincount * (moduleCount == 0 ? 0 : productionvalue.Worth / moduleCount);
                    value.Add("warehouseJoinValue", warehouseJoinValue.ToString("F1"));
                    //未完成产值
                    var uncompleteValue = productionvalue.Worth - warehouseJoinValue;
                    value.Add("uncompleteValue", uncompleteValue.ToString("F1"));
                    //备注
                    value.Add("remark", productionvalue.Remark);
                }
                total.Add(i.ToString(), value);
                i++;
                value = new JObject();
            }
            return total;
        }

    }

        #endregion
    }
#endregion