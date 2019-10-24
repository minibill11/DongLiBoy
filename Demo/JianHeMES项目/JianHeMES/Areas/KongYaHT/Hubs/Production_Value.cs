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
                10000);
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
            var temp_Warehouse = from c in db.Warehouse_Join select new Temp_Warehouse { OrderNum = c.OrderNum, Date = c.Date, OuterBoxBarcode = c.OuterBoxBarcode,IsOut=c.IsOut,WarehouseOutDate=c.WarehouseOutDate,NewBarcode=c.NewBarcode,WarehouseOutNum=c.WarehouseOutNum, BarCodeNum = c.BarCodeNum };

            var productionOrder = temp_BasicInfo.Select(c => c.OrderNum).Distinct().ToList();
            int i = 0;
            List<string> first = new List<string>();
            List<string> second = new List<string>();
            List<string> three = new List<string>();
            //排序
            foreach (var order in productionOrder)
            {
                if (temp_Warehouse.Count(c => c.OrderNum == order && c.IsOut == true) > 0)
                { first.Add(order); }
                else if (temp_Warehouse.Count(c => c.OrderNum == order && c.IsOut == false) > 0)
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
                var warehouse = temp_Warehouse.Where(c => c.IsOut == true && c.OrderNum == item).Select(c => c.OuterBoxBarcode).Distinct().Count() ;
                var baseinfo = temp_BasicInfo.Where(c => c.OrderNum == item).Sum(c => c.Quantity);
                if (warehouse==baseinfo)
                {
                    var lastDate = temp_Warehouse.Where(c => c.IsOut == true && c.OrderNum == item).Max(c => c.WarehouseOutDate);
                    if (lastDate <= yesterday)
                    {
                        continue;
                    }
                }
                var productionvalue = db.Production_Value.Where(c => c.OrderNum == item).FirstOrDefault();
                
                //订单号
                value.Add("OrderNum", item);

                //模组数量
                //var basicinfo = db.Packing_BasicInfo.Where(c => c.OrderNum == item).ToList();
                //int moduleCount = 0;
                //foreach (var basic in basicinfo)
                //{
                //    moduleCount = moduleCount + (basic.OuterBoxCapacity * basic.Quantity);
                //}
                var moduleCount = db.OrderMgm.Where(c => c.OrderNum == item).Select(c => c.Boxes).FirstOrDefault();
                value.Add("moduleCount", moduleCount);
                //包装件数
               // var quantity = db.Packing_BasicInfo.Where(c => c.OrderNum == item).Sum(c => c.Quantity);
                //已包装数量
                var packingCount = db.Packing_BarCodePrinting.Count(c => c.OrderNum == item).ToString()+"("+db.Packing_BarCodePrinting.Where(c => c.OrderNum == item).Select(c=>c.OuterBoxBarcode).Distinct().Count()+"/"+ baseinfo.ToString() + ")";
                value.Add("packingCount", packingCount);

                #region 查找当前出入库的情况
                var temresult = common.GetCurrentwarehousList(item);
                #endregion

                //已入库数量
                var warehousJoincount = temresult.Count(c => c.OrderNum == item);
                var warehousJoinCount = warehousJoincount.ToString()+"("+ temresult.Where(c => c.OrderNum == item && c.Date!=null).Select(c => c.OuterBoxBarcode).Distinct().Count() +"/"+ baseinfo.ToString() + ")";
                value.Add("warehousJoinCount", warehousJoinCount);
                //已出库数量
                var warehousOutcount = temresult.Count(c => c.OrderNum == item && c.IsOut == true);
                var warehousOutCount = warehousOutcount.ToString() + "(" + temresult.Where(c => c.OrderNum == item && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count() + "/" + baseinfo.ToString() + ")";
                value.Add("warehousOutCount", warehousOutCount);

                //库存数量
                var outheCount = temresult.Where(c => c.OrderNum == item && c.Date != null).Select(c => c.OuterBoxBarcode).Distinct().Count() - temresult.Where(c => c.OrderNum == item && c.IsOut == true).Select(c => c.OuterBoxBarcode).Distinct().Count();
                var stockCount = (warehousJoincount - warehousOutcount).ToString() +"(" +outheCount+ ")";
                value.Add("stockCount", stockCount);

                //挪用信息
                JArray nuoInfo = new JArray();
                var warehousenum = temp_Warehouse.Where(c => c.OrderNum == item && c.IsOut == true && c.NewBarcode != null).Select(c => c.WarehouseOutNum).Distinct().ToList();
                foreach (var num in warehousenum)
                {
                    var ordernum = temp_Warehouse.Where(c => c.OrderNum == item && c.IsOut == true && c.NewBarcode != null && c.WarehouseOutNum == num).Select(c => new { c.NewBarcode,c.OuterBoxBarcode}).ToList();
                   
                    nuoInfo.Add("第" + num + "次出库到" + ordernum.FirstOrDefault().NewBarcode + "订单"+ordernum.Select(c=>c.OuterBoxBarcode).Distinct().Count()+"箱("+ordernum.Count+"件);");
                }
                //var old = db.BarCodeRelation.Where(c => c.OldOrderNum == item).ToList();
                //if (old.Count() != 0)
                //{
                //    var oldselectnew=old.Select(c => c.NewOrderNum).Distinct();
                //    foreach (var newitem in oldselectnew)
                //    {
                //        var count = old.Count(c => c.NewOrderNum == newitem);
                //        nuoInfo.Add("挪到订单"+newitem + "(" + count + "条)");
                //    }
                //}
                //var newbarcode = db.BarCodeRelation.Where(c => c.NewOrderNum == item).ToList();
                //if (newbarcode.Count() != 0)
                //{
                //    var newselectold = newbarcode.Select(c => c.OldOrderNum).Distinct();
                //    foreach (var olditem in newselectold)
                //    {
                //        var count = newbarcode.Count(c => c.OldOrderNum == olditem);
                //        nuoInfo.Add("挪用订单" + olditem + "(" + count + "条)");
                //    }
                //}
                value.Add("nuoInfo", nuoInfo);
                //入库完成率
                var warehousJoinComplete = (moduleCount==0?0:(decimal)(warehousJoincount * 100) / moduleCount).ToString("F2")+"%";
                value.Add("warehousJoinComplete", warehousJoinComplete);
                //出库完成率
                var warehousOutComplete= (moduleCount == 0 ? 0 :(decimal)(warehousOutcount * 100) / moduleCount).ToString("F2") + "%";
                value.Add("warehousOutComplete",warehousOutComplete);

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