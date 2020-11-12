using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Data.Entity;
using JianHeMES.AuthAttributes;

namespace JianHeMES.Controllers
{
    public class SMT_SulderpasterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController com = new CommonalityController();

        // GET: SMT_Sulderpaster
        //锡膏入库
        public class tempWarehous
        {
            public string barcode { get; set; }
            public DateTime? time { get; set; }
            public DateTime? intotime { get; set; }

            public int day { get; set; }

        }
        #region 页面
        public ActionResult AddWarehouseBaseInfo()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "AddWarehouseBaseInfo" });
            }
            return View();
        }

        //打印条码
        public ActionResult printBarcode()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "printBarcode" });
            }
            return View();
        }
        //锡膏入冰柜
        public ActionResult AddWarehouseFreezer()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "AddWarehouseFreezer" });
            }
            return View();
        }
        //出库
        public ActionResult outWarehouseFreezer()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "outWarehouseFreezer" });
            }
            return View();
        }
        //入SMT冰柜
        public ActionResult AddSMTFreezer()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "AddSMTFreezer" });
            }
            return View();
        }
        //回温
        public ActionResult Rewarming()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "Rewarming" });
            }
            return View();
        }

        //回温记录
        public ActionResult RewarmRecord()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "RewarmRecord" });
            }
            return View();
        }
        //搅拌
        public ActionResult Stir()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "Stir" });
            }
            return View();
        }
        //使用
        public ActionResult Use()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "Use" });
            }
            return View();
        }
        //回收
        public ActionResult recovery()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "recovery" });
            }
            return View();
        }
        //看板
        public ActionResult smtBoard()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "smtBoard" });
            }
            return View();
        }
        public ActionResult smtBoardHistory()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "smtBoard" });
            }
            return View();
        }
        public ActionResult mcBoard()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "mcBoard" });
            }
            return View();
        }
        public ActionResult Addwarehouse_Material()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "SMT_Sulderpaster", act = "Addwarehouse_Material" });
            }
            return View();
        }
        #endregion

        #region MC 物料表
        /// <summary>
        /// 录入物料信息
        /// </summary>
        /// 根据传过来的数据,判断是否有相同物料编号的数据,没有则添加数据,有则提示错误
        /// <param name="warehouse_Material">物料基本信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Addwarehouse_MaterialAsync(List<Warehouse_Material_BaseInfo> warehouse_Material)
        {
            int count = 0;
            JObject message = new JObject();
            if (ModelState.IsValid)//判断传过来的格式是否正确
            {
                foreach (var item in warehouse_Material)//循环传过来的数据
                {
                    var isExit = db.Warehouse_Material_BaseInfo.Count(c => c.MaterialNumber == item.MaterialNumber);//判断是否有相同物料编号的数据
                    if (isExit > 0)//如果有提示重复错误
                    {
                        message.Add("result", false);
                        message.Add("message", "已有重复的物料号");
                        return Content(JsonConvert.SerializeObject(message));
                    }
                    db.Warehouse_Material_BaseInfo.Add(item);//没有则往数据库添加数据
                    count += await db.SaveChangesAsync();
                }
                if (count == warehouse_Material.Count())//返回成功信息
                {
                    message.Add("result", true);
                    message.Add("message", "录入成功");
                    return Content(JsonConvert.SerializeObject(message));
                }
            }
            message.Add("result", false);
            message.Add("message", "输入的数据格式不对");
            return Content(JsonConvert.SerializeObject(message));
        }


        #endregion

        #region MC 看板
        /// <summary>
        /// MC 看板
        /// </summary>
        /// <param name="barcode">锡膏条码</param>
        /// <param name="materialNumber">物料编号</param>
        /// <param name="batch">批次</param>
        /// <param name="total">是否全部</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MCBoard(string barcode, string materialNumber, string batch, bool total = false)
        {
            var barcodeList = db.Barcode_Solderpaste.ToList();//得到所有锡膏条码表信息
            if (!string.IsNullOrEmpty(materialNumber))//判断物料编号是否为空
            {
                barcodeList = barcodeList.Where(c => c.MaterialNumber == materialNumber).ToList();
            }
            if (!string.IsNullOrEmpty(batch))//判断批次是否为空
            {
                barcodeList = barcodeList.Where(c => c.Batch == batch).ToList();
            }
            if (!string.IsNullOrEmpty(barcode))//判断条码是否为空
            {
                barcodeList = barcodeList.Where(c => c.SolderpasterBacrcode == barcode).ToList();
            }

            JArray result = new JArray();
            foreach (var item in barcodeList)//循环最后筛选好的锡膏数据
            {
                JObject jobjectitem = new JObject();

                var warehouse = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == item.SolderpasterBacrcode).FirstOrDefault();//查找冰柜表里是否有信息
                if (warehouse == null)//没有信息
                    jobjectitem.Add("statue", "未入冰柜");
                else
                {
                    if (warehouse.WarehouseOutTime != null)//有信息在判断是否已出库或入库
                    {
                        if (total)//是否是全部,如果是全部,有出库信息显示出库信息,不是勾选全部,有出库信息则跳过
                        {
                            if (warehouse.WarehouseOutUserName == "报废出库")
                            {
                                jobjectitem.Add("statue", "已报废");
                            }
                            else
                            {
                                jobjectitem.Add("statue", "已出库");
                            }
                        }
                        else
                            continue;
                    }
                    else
                        jobjectitem.Add("statue", "已入库");

                }
                //锡膏条码
                jobjectitem.Add("barcode", item.SolderpasterBacrcode);
                //物料号
                jobjectitem.Add("MaterialNumber", item.MaterialNumber);
                //物料号
                jobjectitem.Add("Batch", item.Batch);
                //供应商
                jobjectitem.Add("Supplier", item.Supplier);
                //生产时间
                jobjectitem.Add("LeaveFactoryTime", item.LeaveFactoryTime);
                //型号
                jobjectitem.Add("SolderpasteType", item.SolderpasteType);
                //剩余有效时间
                var timespan = item.EffectiveDay - (DateTime.Now - item.LeaveFactoryTime).Value.Days;
                jobjectitem.Add("effTime", timespan);
                result.Add(jobjectitem);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region MC 锡膏出入库 

        /// <summary>
        /// 仓库基本信息录入
        /// </summary>
        /// 判断有没有相同物料编号和批次的条码信息,有则返回错误,没有则录入信息,生产条码,条码要先判断是否有相同物料号相同出厂日期的条码,有的话在最后的条码序列号+1,生产新条码,没有则序列号从1开始
        /// <param name="warehouse_Material_InPut">仓库基本信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JArray> AddWarehouseBaseInfo(Warehouse_Material_InPut warehouse_Material_InPut, string Department1, string Group)
        {
            if (((Users)Session["User"]) != null)//判断是否登录
            {
                if (ModelState.IsValid)//判断数据格式是否正确
                {
                    if (db.Warehouse_Material_InPut.Count(c => c.MaterialNumber == warehouse_Material_InPut.MaterialNumber && c.Batch == warehouse_Material_InPut.Batch) != 0)//判断基本信息表里有没有相同的物料编号和批次,有则返回错误null
                    {
                        return null;
                    }
                    warehouse_Material_InPut.CheckTime = DateTime.Now;//录入时间
                    warehouse_Material_InPut.Username = ((Users)Session["User"]).UserName;//录入人姓名
                    warehouse_Material_InPut.JobNum = ((Users)Session["User"]).UserNum.ToString();//录入人工号
                    warehouse_Material_InPut.Department = Department1;
                    warehouse_Material_InPut.Group = Group;
                    db.Warehouse_Material_InPut.Add(warehouse_Material_InPut);//添加数据

                    #region 生产条码
                    if (!string.IsNullOrEmpty(warehouse_Material_InPut.MaterialNumber) || warehouse_Material_InPut.LeaveFactoryTime != null)//判断物料编号和出厂时间是否未空
                    {
                        string year = warehouse_Material_InPut.LeaveFactoryTime.Value.Year.ToString().Substring(2);//得到出厂时间的年后两位
                        JArray barcodeList = new JArray();
                        int count = 0;
                        for (int i = 1; i <= warehouse_Material_InPut.SolderpasteNum; i++)
                        {
                            int num = 1;
                            //查找锡膏条码前缀一样的最后一个序列号
                            var lastBarcode = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == warehouse_Material_InPut.MaterialNumber && c.LeaveFactoryTime == warehouse_Material_InPut.LeaveFactoryTime).OrderByDescending(c => c.SolderpasterBacrcode).FirstOrDefault();//找到相同物料编号,相同出厂时间符合条件的最后一个锡膏条码
                            if (lastBarcode != null)//符合条件的条码不为空
                            {

                                string lastNUm = lastBarcode.SolderpasterBacrcode.Substring(lastBarcode.SolderpasterBacrcode.Length - 3, 3);//找到条码的最后序列号
                                num = int.Parse(lastNUm) + 1;//序列号加1
                            }

                            string barcode = warehouse_Material_InPut.MaterialNumber + "-" + year + warehouse_Material_InPut.LeaveFactoryTime.Value.Month.ToString().PadLeft(2, '0') + warehouse_Material_InPut.LeaveFactoryTime.Value.Day.ToString().PadLeft(2, '0') + "-" + num.ToString().PadLeft(3, '0');//组成条码 物料编号+出产时间的年月日+序列编号

                            Barcode_Solderpaste barcode_Solderpaste = new Barcode_Solderpaste() { SolderpasterBacrcode = barcode, Batch = warehouse_Material_InPut.Batch, LeaveFactoryTime = warehouse_Material_InPut.LeaveFactoryTime, MaterialNumber = warehouse_Material_InPut.MaterialNumber, SolderpasteType = warehouse_Material_InPut.SolderpasteType, Supplier = warehouse_Material_InPut.Supplier, PrintTime = DateTime.Now, PrintName = ((Users)Session["User"]).UserName, EffectiveDay = (int)warehouse_Material_InPut.EffectiveDay };//新建条码的数据信息
                            db.Barcode_Solderpaste.Add(barcode_Solderpaste);//添加数据库
                            count += await db.SaveChangesAsync();
                            barcodeList.Add(barcode);
                        }
                        if (count == warehouse_Material_InPut.SolderpasteNum + 1)
                            return barcodeList;
                    }
                    #endregion
                    return null;


                }
            }
            return null;
        }

        /// <summary>
        /// 输入物料号，返回物料信息
        /// </summary>
        /// <param name="Material">物料号</param>
        /// <returns></returns>
        public ActionResult GetMaterialInfo(string Material)
        {
            var info = db.Warehouse_Material_BaseInfo.Where(c => c.MaterialNumber == Material).FirstOrDefault();//根据物料号找到物料基本信息
            JObject message = new JObject();
            if (info == null)
            {
                return null;
            }
            message.Add("type", info.Type);//类型
            //厂商简称
            message.Add("ManufactorName", info.ManufactorName);
            //厂商编号
            message.Add("ManufactorNum", info.ManufactorNum);
            return Content(JsonConvert.SerializeObject(message));
        }

        //查看物料信息
        public ActionResult SelctMaterial()
        {
            var meterial = db.Warehouse_Material_BaseInfo.Where(c => c.VarietyType == "锡膏").ToList();//查找锡膏的物料信息
            JArray result = new JArray();
            //result.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(meterial)));
            foreach (var item in meterial)
            {
                JObject jObject = new JObject();
                var num = item.MaterialNumber.ToString();
                var barcodelist = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == num).Select(c => c.SolderpasterBacrcode).ToList();
                var count = db.Warehouse_Freezer.Count(c => barcodelist.Contains(c.SolderpasterBacrcode) && c.WarehouseOutTime == null);
                jObject.Add("ID", item.ID);
                jObject.Add("ManufactorNum", item.ManufactorNum);
                jObject.Add("ManufactorName", item.ManufactorName);
                jObject.Add("MaterialNumber", item.MaterialNumber);
                jObject.Add("ProductName", item.ProductName);
                jObject.Add("Specifications", item.Specifications);
                jObject.Add("Type", item.Type);
                jObject.Add("VarietyType", item.VarietyType);
                jObject.Add("count", count);
                result.Add(jObject);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //修改物料信息
        public void UpdateMaterial(Warehouse_Material_BaseInfo warehouse_Material_BaseInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(warehouse_Material_BaseInfo).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 入仓库冰柜
        /// </summary>
        /// <param name="warehouse_FreezerList">条码列表</param>
        /// <param name="warehouseNum">库位号</param>
        /// <param name="group">班组</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> AddWarehouseFreezerAsync(List<string> warehouse_FreezerList, string warehouseNum, string group)
        {
            if (((Users)Session["User"]) == null)//判断是否登录
            {
                return false;
            }
            int count = 0;
            foreach (var item in warehouse_FreezerList)//循环条码列表
            {
                Warehouse_Freezer warehouse_Freezer = new Warehouse_Freezer();
                warehouse_Freezer.SolderpasterBacrcode = item;
                warehouse_Freezer.WarehouseNum = warehouseNum;
                warehouse_Freezer.IntoTime = DateTime.Now;
                warehouse_Freezer.UserName = ((Users)Session["User"]).UserName;
                warehouse_Freezer.JobNum = ((Users)Session["User"]).UserNum.ToString();
                warehouse_Freezer.Group = group;
                warehouse_Freezer.Department = ((Users)Session["User"]).Department;
                db.Warehouse_Freezer.Add(warehouse_Freezer);//新增信息
                count += await db.SaveChangesAsync();//保存数据
            }
            if (count == warehouse_FreezerList.Count())
                return true;
            else
                return false;
            //}
            //return Content("false");
        }

        /// <summary>
        /// 出仓库冰柜
        /// </summary>
        /// 找到根据入库时间排序的条码列表,将传过来的条码列表记录出库后,判断传过来的列表是否在根据入库时间排序的条码列表的前面,不是在前面的写出日志
        /// <param name="warehouse_FreezerList">条案列表</param>
        /// <param name="name">领料人姓名</param>
        /// <param name="jobnum">领料人工号</param>
        /// <param name="group">班组</param>
        /// <returns></returns>
        public async Task<bool> UpdateWarehouseFreezerAsync(List<string> warehouse_FreezerList, string name, string jobnum, string group)
        {
            if (((Users)Session["User"]) == null)//判断是否登录
            {
                return false;
            }
            int count = 0;
            #region 拿到应出库列表
            int a = warehouse_FreezerList.Count();//得到本次锡膏出库数量
            var warehous = db.Warehouse_Freezer.Where(c => c.WarehouseOutTime == null).ToList();//拿到没有出库的锡膏信息
            var tempwarehouse = new List<tempWarehous>();
            var tempwarehouse2 = new List<string>();
            foreach (var item in warehous)//查找条码列表
            {
                var time = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item.SolderpasterBacrcode).FirstOrDefault();//等到当前条码的条码信息
                tempWarehous temp = new tempWarehous() { barcode = item.SolderpasterBacrcode, time = time.LeaveFactoryTime, day = time.EffectiveDay, intotime = item.IntoTime };//将条码,锡膏出厂时间,有效期,入库时间保存起来

                var spam = (int)(DateTime.Now - time.LeaveFactoryTime).Value.TotalDays;//查看有效期是否未负数
                var effectiveday = time.EffectiveDay - spam;
                if (effectiveday < 0)//有效期为负数
                {
                    tempwarehouse2.Add(item.SolderpasterBacrcode);
                }
                else//有效期为正数
                {
                    tempwarehouse.Add(temp);
                }
            }
            var list = tempwarehouse.OrderBy(c => c.intotime).Select(c => c.barcode).ToList();//拿到除去有效期为负数的 根据入库时间排序的列表
            tempwarehouse2.AddRange(list);//将有效期为负数的放在最前面
            #endregion
            foreach (var item in warehouse_FreezerList)//循环传过来的条码列表
            {

                var warehouse = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();//修改数据
                warehouse.WarehouseOutTime = DateTime.Now;
                warehouse.WarehouseOutUserName = name;
                warehouse.WarehouseOutJobNum = jobnum;
                warehouse.WarehouseOutDep = ((Users)Session["User"]).Department;
                warehouse.WarehouseOutGroup = group;
                db.Entry(warehouse).State = System.Data.Entity.EntityState.Modified;
                count += await db.SaveChangesAsync();
            }
            if (count == warehouse_FreezerList.Count())//判断保存是否正确
            {
                //填写日志
                //找到应该要出库的列表
                var maybeList = tempwarehouse2.Take(a);
                //找不到没有在应出库列表的列表
                var not = maybeList.Except(warehouse_FreezerList).ToList();
                if (not.Count != 0)
                {
                    string barcode = string.Join(",", not.ToArray());
                    UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName, OperateRecord = "锡膏条码" + barcode + "没有在本次的出库列表中,它应该是要出库的" };
                    db.UserOperateLog.Add(log);
                    db.SaveChanges();
                }
                return true;
            }
            else
                return false;
            //}
            //return Content("false");
        }
        [HttpPost]
        //出库列表显示提示先入先出
        public ActionResult CheckWarehouse()
        {
            var warehous = db.Warehouse_Freezer.Where(c => c.WarehouseOutTime == null).ToList();//查找所有未出库的锡膏信息
            var warehousebarcodelist = warehous.Select(c => c.SolderpasterBacrcode).ToList();
            var recly = db.SMT_Recycle.Where(c => warehousebarcodelist.Contains(c.SolderpasterBacrcode)).Select(c => c.SolderpasterBacrcode).ToList();
            var a = warehousebarcodelist.Except(recly).ToList();
            var b = db.Warehouse_Freezer.Where(c => a.Contains(c.SolderpasterBacrcode)).Select(c => new { c.SolderpasterBacrcode, c.IntoTime }).ToList();
            var tempwarehouse = new List<tempWarehous>();
            foreach (var item in b)//循环信息
            {
                var time = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item.SolderpasterBacrcode).FirstOrDefault();
                tempWarehous temp = new tempWarehous() { barcode = item.SolderpasterBacrcode, time = time.LeaveFactoryTime, day = time.EffectiveDay, intotime = item.IntoTime };
                tempwarehouse.Add(temp);//记录信息
            }
            tempwarehouse.OrderBy(c => c.intotime).ToList();//根据入库时间进行排序

            JArray result = new JArray();
            foreach (var warehouseitem in tempwarehouse)//循环列表
            {
                JObject jObjectitem = new JObject();
                jObjectitem.Add("barcode", warehouseitem.barcode);//条码
                //生产日期
                jObjectitem.Add("time", warehouseitem.time);
                //有效天数
                var spam = (int)(DateTime.Now - warehouseitem.time).Value.TotalDays;
                var effectiveday = warehouseitem.day - spam;
                jObjectitem.Add("overdue", effectiveday);
                if (effectiveday < 0)//有效期为负数,放在第一位
                {
                    result.AddFirst(jObjectitem);
                }
                else
                {
                    result.Add(jObjectitem);
                }
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 根据料号和生产时间显示已出库和未出库的条码号
        /// </summary>
        /// <param name="materialNumber">物料编号</param>
        /// <param name="leaveFactoryTime">出厂时间</param>
        /// <returns></returns>
        public ActionResult DisplayBarcode(string materialNumber, DateTime leaveFactoryTime)
        {
            var barocdelList = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == materialNumber && c.LeaveFactoryTime == leaveFactoryTime).Select(c => c.SolderpasterBacrcode).ToList();//根据物料号.出厂时间找到条码列表
            var warehousrJoin = db.Warehouse_Freezer.Select(c => c.SolderpasterBacrcode).Distinct().ToList();//查找冰柜表里的所有条码清单
            var except = barocdelList.Except(warehousrJoin).ToList();//拿到没有入冰柜的条码清单
            var have = barocdelList.Intersect(warehousrJoin).ToList();//拿到在并冰柜的条码清单
            JObject result = new JObject();
            result.Add("except", except.Count);
            result.Add("exceptList", JsonConvert.SerializeObject(except));
            result.Add("having", have.Count);
            // result.Add("having", have.Count);
            return Content(JsonConvert.SerializeObject(result));
        }

        #region 锡膏再次打印
        /// <summary>
        /// 锡膏再次打印
        /// </summary>
        /// <param name="materialNumber">物料号</param>
        /// <param name="leaveFactoryTime">出厂时间</param>
        /// <param name="batch">批次</param>
        /// <returns></returns>
        public JArray DisplayPrintAgainBarcode(string materialNumber, DateTime leaveFactoryTime, string batch)
        {
            var barcodeList = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == materialNumber && c.LeaveFactoryTime == leaveFactoryTime && c.Batch == batch).Select(c => c.SolderpasterBacrcode).ToArray();//根据物料号,出厂时间,批次,找到条码信息
            JArray result = new JArray();
            foreach (var item in barcodeList)
            {
                result.Add(item);
            }
            return result;
        }
        #endregion

        //上传报废单
        public ActionResult UploadImg()
        {
            JObject result = new JObject();
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadfile"];

                if (Directory.Exists(@"D:\MES_Data\SMT_Sulderpaster\Scrap\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\SMT_Sulderpaster\Scrap\");
                }
                string time = string.Format("{0:yyyy-mm-dd-hh-MM-ss}", DateTime.Now);
                file.SaveAs(@"D:\MES_Data\SMT_Sulderpaster\Scrap\" + time + ".jpg");
                result.Add("mes", time);
                result.Add("pass", true);
                return Content(JsonConvert.SerializeObject(result));
            }
            result.Add("mes", "没有找到上传的文件");
            result.Add("pass", false);
            return Content(JsonConvert.SerializeObject(result));
        }

        //显示报废单
        public ActionResult DisplayImg(string suldepaster)
        {
            JObject result = new JObject();
            var time = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == suldepaster).Select(c => c.WarehouseOutJobNum).FirstOrDefault();
            List<FileInfo> filesInfo = com.GetAllFilesInDirectory(@"D:\MES_Data\SMT_Sulderpaster\Scrap\");
            var file = filesInfo.Where(c => c.Name == time + ".jpg").FirstOrDefault();
            if (file != null)
            {
                result.Add("have", true);
                result.Add("path", @"D:\MES_Data\SMT_Sulderpaster\Scrap\" + time + ".jpg");
            }
            else
            {
                result.Add("have", false);
                result.Add("path", null);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //更新报废单
        public void UpdateImg(string suldepaster)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadfile"];
                var time = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == suldepaster).Select(c => c.WarehouseOutJobNum).FirstOrDefault();
                file.SaveAs(@"D:\MES_Data\SMT_Sulderpaster\Scrap\" + time + ".jpg");
            }
        }
        #endregion


        #region SMT 入冰柜

        /// <summary>
        /// 条码检验
        /// </summary>
        /// <param name="smt_FreezerList">条码列表</param>
        /// <param name="statu">例：仓库，冰柜，回温，领用</param>
        /// <param name="belogin">冰柜专属 如：仓库，回温，领用</param>
        /// <returns></returns>
        public ActionResult CheckSMTFreezer(List<string> smt_FreezerList, string statu, string belogin = null)
        {
            JArray List = new JArray();
            foreach (var item in smt_FreezerList)//循环条码列表
            {
                JObject jObject = new JObject();
                jObject.Add("barcode", item);
                var startime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.PrintTime).FirstOrDefault();//根据条码,在条码表找信息
                if (startime == null)//如果没找到信息,提示错误
                {
                    jObject.Add("Tips", "没有找到此条码");
                }
                else
                {
                    var spantime = com.TwoDTforMonth_sub(DateTime.Now, startime);//计算锡膏有效期
                    var recyle = db.SMT_Recycle.Count(c => c.SolderpasterBacrcode == item);//在回收表是否有信息
                    if (statu == "回收")
                    {
                        if (recyle != 0)
                            jObject.Add("Tips", "此条码已回收");
                        else
                            jObject.Add("Tips", "正常");
                    }
                    else if (spantime >= 5 && spantime < 6)
                    {
                        jObject.Add("Tips", "将过期");
                    }
                    else if (spantime >= 6)
                    {
                        jObject.Add("Tips", "已过期");
                    }

                    else if (recyle != 0)
                    {
                        jObject.Add("Tips", "此条码已回收");
                    }
                    else
                    {
                        if (statu == "冰柜")
                        {
                            if (string.IsNullOrEmpty(belogin))
                            {
                                jObject.Add("Tips", "未选择来源工序");
                            }
                            else
                            {
                                var freezecount = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item && c.Belogin == belogin).OrderByDescending(c => c.IntoTime).Select(c => c.IntoTime).FirstOrDefault();
                                var emply = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                                if (belogin == "仓库")
                                {
                                    if (freezecount != null)
                                    { jObject.Add("Tips", "此条码已存在SMT冰柜中"); }
                                    else
                                        jObject.Add("Tips", "正常");
                                }
                                else if (belogin == "回温")
                                {
                                    var warmcount = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                                    if (warmcount <= freezecount)
                                    { jObject.Add("Tips", "此条码已存在SMT冰柜中"); }
                                    else if (warmcount == null)
                                    { jObject.Add("Tips", "此条码未经过回温"); }
                                    else if (warmcount < emply)
                                    { jObject.Add("Tips", "此条码已经领用过了"); }
                                    else
                                        jObject.Add("Tips", "正常");
                                }
                                else if (belogin == "领用")
                                {
                                    //var emply = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                                    if (emply <= freezecount)
                                    { jObject.Add("Tips", "此条码已存在SMT冰柜中"); }
                                    else if (emply == null)
                                    { jObject.Add("Tips", "此条码未使用过"); }
                                    else
                                        jObject.Add("Tips", "正常");
                                }
                            }
                        }
                        else if (statu == "回温")
                        {
                            var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.IntoTime).Select(c => c.IntoTime).FirstOrDefault();
                            var warm = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                            if (warm != null && warm >= freeze)
                            { jObject.Add("Tips", "此条码已在回温中"); }
                            else
                                jObject.Add("Tips", "正常");
                        }
                        else if (statu == "领用")
                        {
                            var warm = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                            var stri = db.SMT_Stir.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();

                            var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.IntoTime).Select(c => c.IntoTime).FirstOrDefault();
                            //var emply = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                            if (stri < warm || stri == null)
                            {
                                jObject.Add("Tips", "此条码未经过搅拌");
                            }
                            //else if (emply != null || emply >= freeze)
                            //{ jObject.Add("Tips", "此条码已经在使用中"); }
                            else
                                jObject.Add("Tips", "正常");
                        }
                        else if (statu == "出库")
                        {
                            var warehouse = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                            if (warehouse == null)
                            { jObject.Add("Tips", "此条码未入库"); }
                            else
                            {
                                if (warehouse.WarehouseOutTime != null)
                                    jObject.Add("Tips", "此条码已出库");
                                else
                                    jObject.Add("Tips", "正常");
                            }
                        }
                        else if (statu == "仓库")
                        {
                            var warehouse = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                            if (warehouse != null)
                            { jObject.Add("Tips", "此条码已入库"); }
                            else
                            {
                                jObject.Add("Tips", "正常");
                            }
                        }
                        else
                        {
                            jObject.Add("Tips", "正常");
                        }
                    }
                }
                List.Add(jObject);
            }
            return Content(JsonConvert.SerializeObject(List));
        }

        //入SMT冰柜下边列表
        public ActionResult CheckSMTFreezeList()
        {
            //DateTime lasrtime = new DateTime(2020, 3, 1);
            var smtfree = db.SMT_Freezer.Select(c => c.SolderpasterBacrcode).ToList();
            var recly = db.SMT_Recycle.Select(c => c.SolderpasterBacrcode).ToList();

            var mcfree = db.Warehouse_Freezer.Where(c => c.WarehouseOutTime != null && c.WarehouseOutUserName != "报废出库").Select(c => c.SolderpasterBacrcode).ToList();
            var exceptSMT = mcfree.Except(smtfree).ToList();
            var exceptRecly = exceptSMT.Except(recly).ToList();
            var waresfree = db.Warehouse_Freezer.Select(c => new tempWarehous { barcode = c.SolderpasterBacrcode, intotime = c.IntoTime }).ToList();
            var barcodesolder = db.Barcode_Solderpaste.Select(c => new tempWarehous { barcode = c.SolderpasterBacrcode, time = c.LeaveFactoryTime, day = c.EffectiveDay });
            List<string> distin = new List<string>();
            //排序
            //var orderbydate=rewarm
            JArray list = new JArray();
            foreach (var item in exceptRecly)
            {
                JObject rejobject = new JObject();

                rejobject.Add("barcode", item);
                //var time = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).Max(c => c.StartTime);
                var time = waresfree.Where(c => c.barcode == item).Max(c => c.intotime);
                var imespan = DateTime.Now - time;
                rejobject.Add("freezespan", imespan.Value.TotalSeconds);

                var productiontime = barcodesolder.Where(c => c.barcode == item).Select(c => c.time).FirstOrDefault();
                if (productiontime == null)
                {
                    continue;
                }
                var span = (DateTime.Now.Date - productiontime).Value.TotalDays;
                var effiday = barcodesolder.Where(c => c.barcode == item).Select(c => c.day).FirstOrDefault();
                rejobject.Add("overdue", effiday - span);

                list.Add(rejobject);

                distin.Add(item);
            }
            return Content(JsonConvert.SerializeObject(list));
        }


        //入SMT冰柜 数据存储操作
        [HttpPost]
        public async Task<bool> AddSMTFreezerAsync(List<string> smt_FreezerList, string belogin, string group)
        {
            if (((Users)Session["User"]) != null)
            {
                int count = 0;
                foreach (var item in smt_FreezerList)
                {
                    SMT_Freezer smt_Freezer = new SMT_Freezer();
                    smt_Freezer.SolderpasterBacrcode = item;
                    smt_Freezer.IntoTime = DateTime.Now;
                    smt_Freezer.UserName = ((Users)Session["User"]).UserName;
                    smt_Freezer.JobNum = ((Users)Session["User"]).UserNum.ToString();
                    smt_Freezer.Group = group;
                    smt_Freezer.Department = ((Users)Session["User"]).Department;
                    smt_Freezer.Belogin = belogin;

                    db.SMT_Freezer.Add(smt_Freezer);
                    count += await db.SaveChangesAsync();
                }
                if (count == smt_FreezerList.Count())
                    return true;
                else
                    return false;

            }
            return false;
        }

        #endregion

        #region SMT 回温
        //回温输入下边列表
        public ActionResult CheckSMTRewarm()
        {
            //找到回温 去掉重复并开始时间最后的一条
            var lastrewarm = (from g in db.SMT_Rewarm group g by g.SolderpasterBacrcode into s select new { time = s.Max(c => c.StartTime), barcode = s.Key }).ToList();
            //找到入SMT 去掉重复并开始时间最后的一条
            var lastfree = (from g in db.SMT_Freezer group g by g.SolderpasterBacrcode into s select new { time = s.Max(c => c.IntoTime), barcode = s.Key }).ToList();
            //拿到相同条码中,入SMT时间小于回温时间的条码列表(即已经在回温,或者回温结束)
            var finshrewarm = lastfree.Select(c => lastrewarm.Where(s => c.barcode == s.barcode && c.time <= s.time).Select(a => a.barcode).FirstOrDefault()).ToList();
            //入smt全部条码列表,按入库时间排序
            var rewarm = lastfree.OrderBy(c => c.time).Select(c => c.barcode).ToList();
            //回收的条码列表
            var recly = db.SMT_Recycle.Select(c => c.SolderpasterBacrcode).ToList();
            //var rewarm = (from s in db.SMT_Freezer group s by s.SolderpasterBacrcode into g select g.Key).ToList();
            //入库去掉回收条码列表
            var rewarmlist = rewarm.Except(recly).ToList();
            //入库去掉回温的条码类表
            var rewarmlist2 = rewarmlist.Except(finshrewarm).ToList();

            //排序
            //var orderbydate=rewarm
            JArray list = new JArray();
            foreach (var item in rewarmlist2)
            {
                JObject rejobject = new JObject();
                rejobject.Add("barcode", item);
                //var time = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).Max(c => c.StartTime);
                var time = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).Max(c => c.IntoTime);
                var imespan = DateTime.Now - time;
                rejobject.Add("freezespan", imespan.Value.TotalSeconds);

                var productiontime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.LeaveFactoryTime).FirstOrDefault();
                if (productiontime == null)
                {
                    continue;
                }
                var span = (DateTime.Now.Date - productiontime).Value.TotalDays;
                var effiday = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.EffectiveDay).FirstOrDefault();
                rejobject.Add("overdue", effiday - span);

                list.Add(rejobject);


            }
            return Content(JsonConvert.SerializeObject(list));
        }


        //开始回温 数据存储操作
        [HttpPost]
        public async Task<bool> AddSMTRewarmAsync(List<string> smt_RewarmList, string group)
        {
            if (((Users)Session["User"]) != null)
            {
                int count = 0;
                foreach (var item in smt_RewarmList)
                {
                    SMT_Rewarm smt_Rewarm = new SMT_Rewarm();
                    smt_Rewarm.SolderpasterBacrcode = item;
                    smt_Rewarm.StartTime = DateTime.Now;
                    smt_Rewarm.UserName = ((Users)Session["User"]).UserName;
                    smt_Rewarm.JobNum = ((Users)Session["User"]).UserNum.ToString();
                    smt_Rewarm.Department = ((Users)Session["User"]).Department;
                    smt_Rewarm.Group = group;
                    db.SMT_Rewarm.Add(smt_Rewarm);
                    count += await db.SaveChangesAsync();
                }
                if (count == smt_RewarmList.Count())
                    return true;

                else
                    return false;
            }
            return false;
        }


        //回温记录
        public ActionResult RewarmInfo()
        {
            //回温去掉重复,那最后回温数据
            var rewarm = (from g in db.SMT_Rewarm group g by g.SolderpasterBacrcode into x select new { time = x.Max(g => g.StartTime), barcode = x.Key }).ToList();
            //入冰柜去掉重复,拿最后的那条数据
            var smtFree = (from g in db.SMT_Freezer group g by g.SolderpasterBacrcode into x select new { time = x.Max(g => g.IntoTime), barcode = x.Key }).ToList();
            //搅拌去掉重复,拿最后搅拌那条数据
            var sit = (from g in db.SMT_Stir group g by g.SolderpasterBacrcode into x select new { time = x.Max(g => g.StartTime), barcode = x.Key }).ToList();
            //找到回收列表
            var recly = db.SMT_Recycle.Select(c => c.SolderpasterBacrcode).ToList();
            //找到 回温时间<=入冰柜时间的(不在回温中)
            var notworkRewarm = rewarm.Select(c => smtFree.Where(s => c.barcode == s.barcode && c.time <= s.time).Select(a => a.barcode).FirstOrDefault()).ToList();
            //找到 回温时间<=搅拌时间(不在回温中)
            var notworkRewarm2 = rewarm.Select(c => sit.Where(s => c.barcode == s.barcode && c.time <= s.time).Select(a => a.barcode).FirstOrDefault()).ToList();
            var tru = notworkRewarm.Count(c => c == "63000-1010-191103-001");
            // workRewarm2.RemoveAll(c => c == null);
            var rewarmList = rewarm.Select(c => c.barcode).ToList();
            var exceptFree = rewarmList.Except(notworkRewarm).ToList();
            var exceptSit = exceptFree.Except(notworkRewarm2).ToList();
            var exceptRecly = exceptSit.Except(recly).ToList();


            //List<string> distin = new List<string>();
            //排序
            //var orderbydate=rewarm
            JArray list = new JArray();
            foreach (var item in exceptRecly)
            {

                JObject rejobject = new JObject();
                //var warmcount = db.SMT_Rewarm.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == item).Select(c => c.StartTime).FirstOrDefault();
                //var sitcount = db.SMT_Stir.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == item).Select(c => c.StartTime).FirstOrDefault();
                //var free = db.SMT_Freezer.OrderByDescending(c => c.IntoTime).Where(c => c.SolderpasterBacrcode == item).Select(c => c.IntoTime).FirstOrDefault();
                //if ((warmcount != null && free != null && warmcount > free) && ((warmcount != null && sitcount == null) || (warmcount != null && sitcount != null && sitcount < warmcount)))
                //{
                rejobject.Add("barcode", item);
                var time = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).Max(c => c.StartTime);
                var imespan = DateTime.Now - time;
                rejobject.Add("retimespan", imespan.Value.TotalSeconds);

                var productiontime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.LeaveFactoryTime).FirstOrDefault();
                if (productiontime == null)
                {
                    continue;
                }
                var span = (DateTime.Now.Date - productiontime).Value.TotalDays;
                var effiday = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.EffectiveDay).FirstOrDefault();
                rejobject.Add("overdue", effiday - span);

                list.Add(rejobject);
                //}
            }
            return Content(JsonConvert.SerializeObject(list));
        }

        [HttpPost]
        //回温记录搜索
        public ActionResult RewarmInfo(string barocde)
        {
            var barcode = db.SMT_Rewarm.Count(c => c.SolderpasterBacrcode == barocde);
            if (barcode != 0)
            {
                JObject rejobject = new JObject();
                var warmcount = db.SMT_Rewarm.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == barocde).Select(c => c.StartTime).FirstOrDefault();
                var sitcount = db.SMT_Stir.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == barocde).Select(c => c.StartTime).FirstOrDefault(); ;
                if ((warmcount != null && sitcount == null) || (warmcount != null && sitcount != null && sitcount < warmcount))
                {
                    rejobject.Add("barcode", barocde);
                    var time = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == barocde).Max(c => c.StartTime);
                    var imespan = DateTime.Now - time;
                    rejobject.Add("retimespan", imespan.Value.TotalSeconds);

                    var productiontime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == barocde).Select(c => c.LeaveFactoryTime).FirstOrDefault();
                    if (productiontime == null)
                    {
                        return Content("false");
                    }
                    var span = (DateTime.Now.Date - productiontime).Value.TotalDays;
                    var effiday = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == barocde).Select(c => c.EffectiveDay).FirstOrDefault();
                    rejobject.Add("overdue", effiday - span);

                    return Content(JsonConvert.SerializeObject(rejobject));
                }
                ModelState.AddModelError("", "此条码没有正在的回温记录");
            }
            ModelState.AddModelError("", "没有找到此条码");
            return Content("false");
        }
        #endregion

        #region SMT 搅拌
        [HttpPost]
        public ActionResult DisplatStirInfo(string SolderpasterBacrcode)
        {
            JObject infojobject = new JObject();
            var free = db.SMT_Freezer.OrderByDescending(c => c.IntoTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).Select(c => c.IntoTime).FirstOrDefault();
            var warmtime = db.SMT_Rewarm.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).Select(c => c.StartTime).FirstOrDefault();
            //var user = db.SMT_Employ.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).Select(c => c.StartTime).FirstOrDefault();
            var info = db.SMT_Stir.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).FirstOrDefault();

            //没有回温记录,并且回温记录小于领用记录
            if (warmtime == null || (warmtime != null && warmtime < free))
            {
                infojobject.Add("first", null);
                infojobject.Add("second", null);
                infojobject.Add("three", null);
                infojobject.Add("flow", null);
                infojobject.Add("message", "此条码还未回温");
                infojobject.Add("canadd", false);
            }
            else
            {
                //有回温没有搅拌记录和 有回温有搅拌有领用，但回温记录大于搅拌记录
                if (info == null || (info != null && warmtime > info.StartTime))
                {
                    infojobject.Add("first", null);
                    infojobject.Add("second", null);
                    infojobject.Add("three", null);
                    infojobject.Add("flow", null);
                    infojobject.Add("message", "此条码还未开始搅拌");
                    infojobject.Add("canadd", true);
                }
                //有回温有搅拌,但回温小于搅拌
                else if (info != null && warmtime != null & warmtime < info.StartTime)
                {

                    infojobject.Add("first", info.StartTime);
                    infojobject.Add("second", info.SecondTime);
                    infojobject.Add("three", info.ThreeTime);
                    infojobject.Add("flow", info.FlorTime);
                    infojobject.Add("message", null);
                    infojobject.Add("canadd", true);

                }
            }
            return Content(JsonConvert.SerializeObject(infojobject));
        }

        //开始搅拌 数据存储操作
        public bool AddSMTStir(string SolderpasterBacrcode, int num, string group)
        {
            if (((Users)Session["User"]) != null)
            {

                var sMT_Stir = db.SMT_Stir.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).FirstOrDefault();
                switch (num)
                {
                    case 1:
                        SMT_Stir stir = new SMT_Stir() { SolderpasterBacrcode = SolderpasterBacrcode, UserName = ((Users)Session["User"]).UserName, JobNum = ((Users)Session["User"]).UserNum.ToString(), StartTime = DateTime.Now, Department = ((Users)Session["User"]).Department, Group = group };
                        db.SMT_Stir.Add(stir);

                        break;
                    case 2:
                        if (sMT_Stir.SecondTime == null)
                        {
                            sMT_Stir.SecondTime = DateTime.Now;
                            sMT_Stir.SecondName = ((Users)Session["User"]).UserName;
                            sMT_Stir.SecondJobNum = ((Users)Session["User"]).UserNum.ToString();
                        }
                        break;
                    case 3:
                        if (sMT_Stir.ThreeTime == null)
                        {
                            sMT_Stir.ThreeTime = DateTime.Now;
                            sMT_Stir.ThreeJobName = ((Users)Session["User"]).UserName;
                            sMT_Stir.ThreeNum = ((Users)Session["User"]).UserNum.ToString();
                        }
                        break;
                    case 4:
                        if (sMT_Stir.FlorTime == null)
                        {
                            sMT_Stir.FlorTime = DateTime.Now;
                            sMT_Stir.FlorName = ((Users)Session["User"]).UserName;
                            sMT_Stir.FlorJobNum = ((Users)Session["User"]).UserNum.ToString();
                        }
                        break;
                    default:

                        break;
                }

                db.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region SMT 使用
        [HttpPost]
        public async Task<bool> AddEmployAsync(List<string> smt_EmployList, string ordernum, int line, string group)
        {
            if (((Users)Session["User"]) == null)
            {
                return false;
            }
            int count = 0;
            foreach (var item in smt_EmployList)
            {
                SMT_Employ sMT_Employ = new SMT_Employ() { SolderpasterBacrcode = item, StartTime = DateTime.Now, JobNum = ((Users)Session["User"]).UserNum.ToString(), UserName = ((Users)Session["User"]).UserName, OrderNum = ordernum, LineNum = line, Department = ((Users)Session["User"]).Department, Group = group };
                db.SMT_Employ.Add(sMT_Employ);
                count += await db.SaveChangesAsync();
            }
            if (count == smt_EmployList.Count())
                return true;
            else
                return false;
        }

        #endregion

        #region SMT 回收
        [HttpPost]
        public async Task<bool> RecycleAsync(List<string> smt_RecycleList, string group)
        {
            if (((Users)Session["User"]) == null)
            {
                return false;
            }
            int count = 0;
            foreach (var item in smt_RecycleList)
            {
                SMT_Recycle sMT_Recycle = new SMT_Recycle() { SolderpasterBacrcode = item, RecoveTime = DateTime.Now, JobNum = ((Users)Session["User"]).UserNum.ToString(), UserName = ((Users)Session["User"]).UserName, Department = ((Users)Session["User"]).Department, Group = group };
                db.SMT_Recycle.Add(sMT_Recycle);
                count += await db.SaveChangesAsync();
            }
            if (count == smt_RecycleList.Count())
                return true;
            else
                return false;
        }
        #endregion

        #region SMT 看板
        public ActionResult Boadr()
        {
            JArray totle = new JArray();
            var freezebarcode = db.SMT_Freezer.Select(c => c.SolderpasterBacrcode).Distinct().ToList();
            var yesterday = DateTime.Now.AddDays(-1);
            var exceptbarcode = db.SMT_Recycle.Where(c => c.RecoveTime < yesterday).Select(c => c.SolderpasterBacrcode).ToList();
            var barcodes = freezebarcode.Except(exceptbarcode).ToList();
            #region 取值
            var freezelist = db.SMT_Freezer.Where(c => barcodes.Contains(c.SolderpasterBacrcode)).Select(c => new { c.SolderpasterBacrcode, c.IntoTime, c.UserName }).ToList();
            var rewarmlist = db.SMT_Rewarm.Where(c => barcodes.Contains(c.SolderpasterBacrcode)).Select(c => new { c.SolderpasterBacrcode, c.StartTime, c.UserName }).ToList();
            var stirlist = db.SMT_Stir.Where(c => barcodes.Contains(c.SolderpasterBacrcode)).Select(c => new { c.SolderpasterBacrcode, c.StartTime, c.UserName, c.SecondTime, c.ThreeTime, c.FlorTime, c.SecondName, c.ThreeJobName, c.FlorName }).ToList();
            var emptylist = db.SMT_Employ.Where(c => barcodes.Contains(c.SolderpasterBacrcode)).Select(c => new { c.SolderpasterBacrcode, c.StartTime, c.OrderNum, c.LineNum, c.UserName }).ToList();
            var recyllist = db.SMT_Recycle.Where(c => barcodes.Contains(c.SolderpasterBacrcode)).Select(c => new { c.SolderpasterBacrcode, c.UserName, c.RecoveTime }).ToList();
            var barcodelist = db.Barcode_Solderpaste.Where(c => barcodes.Contains(c.SolderpasterBacrcode)).Select(c => new { c.SolderpasterBacrcode, c.LeaveFactoryTime, c.EffectiveDay }).ToList();

            #endregion

            foreach (var item in barcodes)
            {

                JObject barcodejobject = new JObject();

                var freeze = freezelist.Where(c => c.SolderpasterBacrcode == item).OrderBy(c => c.IntoTime).ToList();
                for (int i = 0; i < freeze.Count(); i++)
                {
                    //SMT冰柜时间
                    JObject timeList = new JObject();
                    timeList.Add("barcode", item);
                    var freetime = freeze[i].IntoTime;
                    var freetiem2 = new DateTime?();
                    if (i + 1 == freeze.Count())
                    {
                        freetiem2 = DateTime.Now;
                    }
                    else
                    {
                        freetiem2 = freeze[i + 1].IntoTime;
                    }
                    timeList.Add("freezetime", freeze[i].UserName + ":" + freetime);
                    //回温时间
                    var warmtime = rewarmlist.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    timeList.Add("warmtime", warmtime == null ? "--" : warmtime.UserName + ":" + warmtime.StartTime.ToString());
                    //搅拌时间
                    var stirtime = stirlist.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    JArray stir = new JArray();
                    if (stirtime != null)
                    {
                        stir.Add(stirtime.UserName + ":" + stirtime.StartTime.ToString());
                        stir.Add(stirtime.SecondTime == null ? null : stirtime.SecondName + ":" + stirtime.SecondTime.ToString());
                        stir.Add(stirtime.ThreeTime == null ? null : stirtime.ThreeJobName + ":" + stirtime.ThreeTime.ToString());
                        stir.Add(stirtime.FlorTime == null ? null : stirtime.FlorName + ":" + stirtime.FlorTime.ToString());
                        timeList.Add("stirtime", stir);
                    }
                    else
                    { timeList.Add("stirtime", "--"); }
                    //领用时间
                    var employlist = emptylist.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).ToList();
                    JArray jarrayemply = new JArray();
                    foreach (var employ in employlist)
                    {
                        JObject jitem = new JObject();
                        jitem.Add("ordernum", employ.OrderNum);
                        jitem.Add("linnum", employ.LineNum);
                        jitem.Add("employtime", employ.UserName + ":" + employ.StartTime.ToString());

                        jarrayemply.Add(jitem);
                    }
                    timeList.Add("employ", jarrayemply);
                    //回收时间
                    var recycle = recyllist.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                    if (i + 1 == freeze.Count())
                    {
                        timeList.Add("recycletime", recycle == null ? "--" : recycle.UserName + ":" + recycle.RecoveTime.ToString());
                    }
                    else
                        timeList.Add("recycletime", "--");
                    //剩余有效期
                    var efftime = barcodelist.Where(c => c.SolderpasterBacrcode == item).Select(c => new { c.EffectiveDay, c.LeaveFactoryTime }).FirstOrDefault();
                    var timespan = 0;
                    if (recycle != null)
                    {
                        timespan = efftime.EffectiveDay - (recycle.RecoveTime - efftime.LeaveFactoryTime).Value.Days;
                    }
                    else
                    {
                        timespan = efftime.EffectiveDay - (DateTime.Now - efftime.LeaveFactoryTime).Value.Days;
                    }
                    timeList.Add("effTime", timespan);
                    barcodejobject.Add(i.ToString(), timeList);
                }
                totle.Add(barcodejobject);
            }
            return Content(JsonConvert.SerializeObject(totle));
        }

        [HttpPost]
        public ActionResult Boadr(string barcode, string bitch, string materialNumber, string Supplier, string ordernum)
        {
            var freezebarcode = db.SMT_Freezer.Select(c => c.SolderpasterBacrcode).Distinct();
            if (!string.IsNullOrEmpty(barcode))
            {
                freezebarcode = freezebarcode.Where(c => c == barcode);
            }
            if (!string.IsNullOrEmpty(bitch))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.Batch == bitch).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(materialNumber))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == materialNumber).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(Supplier))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.Supplier == Supplier).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(ordernum))
            {
                var barcodeList = db.SMT_Employ.Where(c => c.OrderNum == ordernum).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }

            JArray totle = new JArray();

            foreach (var item in freezebarcode)
            {
                JObject barcodejobject = new JObject();

                var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderBy(c => c.IntoTime).ToList();
                for (int i = 0; i < freeze.Count(); i++)
                {
                    //SMT冰柜时间
                    JObject timeList = new JObject();

                    //回收时间,回收超过一天移除
                    var recycle = db.SMT_Recycle.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                    if (recycle != null && (DateTime.Now - recycle.RecoveTime).Value.Days >= 1)
                    {
                        continue;
                    }
                    if (i + 1 == freeze.Count())
                    {
                        timeList.Add("recycletime", recycle == null ? "--" : recycle.UserName + ":" + recycle.RecoveTime.ToString());
                    }
                    else
                        timeList.Add("recycletime", "--");


                    timeList.Add("barcode", item);
                    var freetime = freeze[i].IntoTime;
                    var freetiem2 = new DateTime?();
                    if (i + 1 == freeze.Count())
                    {
                        freetiem2 = DateTime.Now;
                    }
                    else
                    {
                        freetiem2 = freeze[i + 1].IntoTime;
                    }
                    timeList.Add("freezetime", freeze[i].UserName + ":" + freetime);
                    //回温时间
                    var warmtime = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    timeList.Add("warmtime", warmtime == null ? "--" : warmtime.UserName + ":" + warmtime.StartTime.ToString());
                    //搅拌时间
                    var stirtime = db.SMT_Stir.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    JArray stir = new JArray();
                    if (stirtime != null)
                    {
                        stir.Add(stirtime.UserName + ":" + stirtime.StartTime.ToString());
                        stir.Add(stirtime.SecondTime == null ? null : stirtime.SecondName + ":" + stirtime.SecondTime.ToString());
                        stir.Add(stirtime.ThreeTime == null ? null : stirtime.ThreeJobName + ":" + stirtime.ThreeTime.ToString());
                        stir.Add(stirtime.FlorTime == null ? null : stirtime.FlorName + ":" + stirtime.FlorTime.ToString());
                        timeList.Add("stirtime", stir);
                    }
                    else
                    { timeList.Add("stirtime", "--"); }
                    //领用时间
                    var employlist = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).ToList();
                    JArray jarrayemply = new JArray();
                    foreach (var employ in employlist)
                    {
                        JObject jitem = new JObject();
                        jitem.Add("ordernum", employ.OrderNum);
                        jitem.Add("linnum", employ.LineNum);
                        jitem.Add("employtime", employ.UserName + ":" + employ.StartTime.ToString());

                        jarrayemply.Add(jitem);
                    }
                    timeList.Add("employ", jarrayemply);
                    //剩余有效期
                    var efftime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => new { c.EffectiveDay, c.LeaveFactoryTime }).FirstOrDefault();
                    var timespan = 0;
                    if (recycle != null)
                    {
                        timespan = efftime.EffectiveDay - (recycle.RecoveTime - efftime.LeaveFactoryTime).Value.Days;
                    }
                    else
                    {
                        timespan = efftime.EffectiveDay - (DateTime.Now - efftime.LeaveFactoryTime).Value.Days;
                    }
                    timeList.Add("effTime", timespan);

                    barcodejobject.Add(i.ToString(), timeList);

                }
                totle.Add(barcodejobject);
            }
            return Content(JsonConvert.SerializeObject(totle));
        }

        public ActionResult HistoryBoard()
        {
            JArray totle = new JArray();
            var freezebarcode = db.SMT_Freezer.Select(c => c.SolderpasterBacrcode).Distinct();

            foreach (var item in freezebarcode)
            {

                JObject barcodejobject = new JObject();

                var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderBy(c => c.IntoTime).ToList();
                for (int i = 0; i < freeze.Count(); i++)
                {
                    //SMT冰柜时间
                    JObject timeList = new JObject();
                    timeList.Add("barcode", item);
                    var freetime = freeze[i].IntoTime;
                    var freetiem2 = new DateTime?();
                    if (i + 1 == freeze.Count())
                    {
                        freetiem2 = DateTime.Now;
                    }
                    else
                    {
                        freetiem2 = freeze[i + 1].IntoTime;
                    }
                    timeList.Add("freezetime", freeze[i].UserName + ":" + freetime);
                    //回温时间
                    var warmtime = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    timeList.Add("warmtime", warmtime == null ? "--" : warmtime.UserName + ":" + warmtime.StartTime.ToString());
                    //搅拌时间
                    var stirtime = db.SMT_Stir.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    JArray stir = new JArray();
                    if (stirtime != null)
                    {
                        stir.Add(stirtime.UserName + ":" + stirtime.StartTime.ToString());
                        stir.Add(stirtime.SecondTime == null ? null : stirtime.SecondName + ":" + stirtime.SecondTime.ToString());
                        stir.Add(stirtime.ThreeTime == null ? null : stirtime.ThreeJobName + ":" + stirtime.ThreeTime.ToString());
                        stir.Add(stirtime.FlorTime == null ? null : stirtime.FlorName + ":" + stirtime.FlorTime.ToString());
                        timeList.Add("stirtime", stir);
                    }
                    else
                    { timeList.Add("stirtime", "--"); }
                    //领用时间
                    var employlist = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).ToList();
                    JArray jarrayemply = new JArray();
                    foreach (var employ in employlist)
                    {
                        JObject jitem = new JObject();
                        jitem.Add("ordernum", employ.OrderNum);
                        jitem.Add("linnum", employ.LineNum);
                        jitem.Add("employtime", employ.UserName + ":" + employ.StartTime.ToString());

                        jarrayemply.Add(jitem);
                    }
                    timeList.Add("employ", jarrayemply);
                    //回收时间
                    var recycle = db.SMT_Recycle.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                    if (i + 1 == freeze.Count())
                    {
                        timeList.Add("recycletime", recycle == null ? "--" : recycle.UserName + ":" + recycle.RecoveTime.ToString());
                    }
                    else
                        timeList.Add("recycletime", "--");
                    //剩余有效期
                    var efftime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => new { c.EffectiveDay, c.LeaveFactoryTime }).FirstOrDefault();
                    var timespan = 0;
                    if (recycle != null)
                    {
                        timespan = efftime.EffectiveDay - (recycle.RecoveTime - efftime.LeaveFactoryTime).Value.Days;
                    }
                    else
                    {
                        timespan = efftime.EffectiveDay - (DateTime.Now - efftime.LeaveFactoryTime).Value.Days;
                    }
                    timeList.Add("effTime", timespan);
                    barcodejobject.Add(i.ToString(), timeList);
                }
                totle.Add(barcodejobject);
            }
            return Content(JsonConvert.SerializeObject(totle));
        }

        public ActionResult HistoryBoadr(string barcode, string bitch, string materialNumber, string Supplier, string ordernum)
        {
            var freezebarcode = db.SMT_Freezer.Select(c => c.SolderpasterBacrcode).Distinct();
            if (!string.IsNullOrEmpty(barcode))
            {
                freezebarcode = freezebarcode.Where(c => c == barcode);
            }
            if (!string.IsNullOrEmpty(bitch))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.Batch == bitch).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(materialNumber))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == materialNumber).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(Supplier))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.Supplier == Supplier).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(ordernum))
            {
                var barcodeList = db.SMT_Employ.Where(c => c.OrderNum == ordernum).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }

            JArray totle = new JArray();

            foreach (var item in freezebarcode)
            {
                JObject barcodejobject = new JObject();

                var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderBy(c => c.IntoTime).ToList();
                for (int i = 0; i < freeze.Count(); i++)
                {
                    //SMT冰柜时间
                    JObject timeList = new JObject();
                    timeList.Add("barcode", item);
                    var freetime = freeze[i].IntoTime;
                    var freetiem2 = new DateTime?();
                    if (i + 1 == freeze.Count())
                    {
                        freetiem2 = DateTime.Now;
                    }
                    else
                    {
                        freetiem2 = freeze[i + 1].IntoTime;
                    }
                    timeList.Add("freezetime", freeze[i].UserName + ":" + freetime);
                    //回温时间
                    var warmtime = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    timeList.Add("warmtime", warmtime == null ? "--" : warmtime.UserName + ":" + warmtime.StartTime.ToString());
                    //搅拌时间
                    var stirtime = db.SMT_Stir.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    JArray stir = new JArray();
                    if (stirtime != null)
                    {
                        stir.Add(stirtime.UserName + ":" + stirtime.StartTime.ToString());
                        stir.Add(stirtime.SecondTime == null ? null : stirtime.SecondName + ":" + stirtime.SecondTime.ToString());
                        stir.Add(stirtime.ThreeTime == null ? null : stirtime.ThreeJobName + ":" + stirtime.ThreeTime.ToString());
                        stir.Add(stirtime.FlorTime == null ? null : stirtime.FlorName + ":" + stirtime.FlorTime.ToString());
                        timeList.Add("stirtime", stir);
                    }
                    else
                    { timeList.Add("stirtime", "--"); }
                    //领用时间
                    var employlist = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).ToList();
                    JArray jarrayemply = new JArray();
                    foreach (var employ in employlist)
                    {
                        JObject jitem = new JObject();
                        jitem.Add("ordernum", employ.OrderNum);
                        jitem.Add("linnum", employ.LineNum);
                        jitem.Add("employtime", employ.UserName + ":" + employ.StartTime.ToString());

                        jarrayemply.Add(jitem);
                    }
                    timeList.Add("employ", jarrayemply);
                    //回收时间
                    var recycle = db.SMT_Recycle.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                    if (i + 1 == freeze.Count())
                    {
                        timeList.Add("recycletime", recycle == null ? "--" : recycle.UserName + ":" + recycle.RecoveTime.ToString());
                    }
                    else
                        timeList.Add("recycletime", "--");
                    //剩余有效期
                    var efftime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => new { c.EffectiveDay, c.LeaveFactoryTime }).FirstOrDefault();
                    var timespan = 0;
                    if (recycle != null)
                    {
                        timespan = efftime.EffectiveDay - (recycle.RecoveTime - efftime.LeaveFactoryTime).Value.Days;
                    }
                    else
                    {
                        timespan = efftime.EffectiveDay - (DateTime.Now - efftime.LeaveFactoryTime).Value.Days;
                    }
                    timeList.Add("effTime", timespan);

                    barcodejobject.Add(i.ToString(), timeList);
                }

                totle.Add(barcodejobject);
            }
            return Content(JsonConvert.SerializeObject(totle));
        }
        #endregion

        #region 列表获取
        //领用订单列表获取
        public ActionResult GetOrderList()
        {
            var orders = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }


        //看板条码列表获取
        public ActionResult GetBoardBarcodeList()
        {
            var orders = db.SMT_Freezer.OrderByDescending(m => m.ID).Select(m => m.SolderpasterBacrcode).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //看板批次列表获取
        public ActionResult GetBoardBatchList()
        {
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Select(m => m.Batch).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //看板厂商简称列表获取
        public ActionResult GetBoardSupplierList()
        {
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Select(m => m.Supplier).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //看板物料编号列表获取
        public ActionResult GetBoardMaterialNumberList()
        {
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Select(m => m.MaterialNumber).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //看板订单列表获取
        public ActionResult GetBoardOrdernumList()
        {
            var orders = db.SMT_Employ.OrderByDescending(m => m.ID).Select(m => m.OrderNum).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //锡膏再打印物料号获取
        public ActionResult GetPrintAgainMaterialNumberList()
        {
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Select(m => m.MaterialNumber).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //锡膏再打印生产日期获取
        public ActionResult GetPrintAgainLeaveFactoryTimeList(string materialNumber)
        {
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Where(c => c.MaterialNumber == materialNumber).Select(m => m.LeaveFactoryTime).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                var vaule = string.Format("{0:yyyy-MM-dd}", item);
                List.Add("value", vaule);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //锡膏再打印批次获取
        public ActionResult GetPrintAgainBatchList(string materialNumber, DateTime time)
        {
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Where(c => c.MaterialNumber == materialNumber && c.LeaveFactoryTime == time).Select(m => m.Batch).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 暂时没用
        ////锡膏条码生成
        //public List<string> AddBarcode(string materialNumber, DateTime productionDate, int Num)
        //{
        //    if (!string.IsNullOrEmpty(materialNumber) || productionDate != null)
        //    {
        //        string year = productionDate.Year.ToString().Substring(2);
        //        List<string> barcodeList = new List<string>();
        //        for (int i = 1; i <= Num; i++)
        //        {
        //            var lastBarcode = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == materialNumber).OrderByDescending(c => c.SolderpasterBacrcode).FirstOrDefault();
        //            if (lastBarcode != null&&lastBarcode.LeaveFactoryTime==productionDate)
        //            {

        //                string lastNUm = lastBarcode.SolderpasterBacrcode.Substring(lastBarcode.SolderpasterBacrcode.Length-3,3);
        //                i = int.Parse(lastNUm)+1;
        //            }

        //            string barcode = materialNumber + "-" + year + productionDate.Month.ToString() + productionDate.Day.ToString() + "-" + i.ToString().PadLeft(3, '0');
        //            barcodeList.Add(barcode);
        //        }
        //        return barcodeList;
        //    }
        //    return null;
        //}

        /// <summary>
        /// 打印条码
        /// </summary>
        /// <param name="Batch"> 批次</param>
        /// <param name="MaterialNumber">料号</param>
        /// <param name="LeaveFactoryTime">生产时间</param>
        /// <param name="Supplier">供应商</param>
        /// <param name="SolderpasteType">型号</param>
        //public async Task<bool> AddBarcode_SolderpasteAsync(string Batch, string MaterialNumber, DateTime LeaveFactoryTime, string Supplier, string SolderpasteType, int EffectiveDay, int Num)
        //{
        //    if (((Users)Session["User"]) != null)
        //    {
        //        return false;
        //    }
        //    if (!string.IsNullOrEmpty(MaterialNumber) || LeaveFactoryTime != null)
        //    {
        //        string year = LeaveFactoryTime.Year.ToString().Substring(2);
        //        List<string> barcodeList = new List<string>();
        //        int count = 0;
        //        for (int i = 1; i <= Num; i++)
        //        {
        //            var lastBarcode = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == materialNumber).OrderByDescending(c => c.SolderpasterBacrcode).FirstOrDefault();
        //            if (lastBarcode != null && lastBarcode.LeaveFactoryTime == LeaveFactoryTime)
        //            {

        //                string lastNUm = lastBarcode.SolderpasterBacrcode.Substring(lastBarcode.SolderpasterBacrcode.Length - 3, 3);
        //                i = int.Parse(lastNUm) + 1;
        //            }

        //            string barcode = materialNumber + "-" + year + LeaveFactoryTime.Month.ToString() + LeaveFactoryTime.Day.ToString() + "-" + i.ToString().PadLeft(3, '0');

        //            Barcode_Solderpaste barcode_Solderpaste = new Barcode_Solderpaste() { SolderpasterBacrcode = barcode, Batch = Batch, LeaveFactoryTime = LeaveFactoryTime, MaterialNumber = materialNumber, SolderpasteType = SolderpasteType, Supplier = Supplier, PrintTime = DateTime.Now, PrintName = ((Users)Session["User"]).UserName };
        //            db.Barcode_Solderpaste.Add(barcode_Solderpaste);
        //            count += await db.SaveChangesAsync();
        //        }
        //        if (count == Num)
        //            return true;
        //    }
        //    return false;
        //}



        #endregion

        //输入工号显示人名
        public string DisplayName(string jobNum)
        {
            var name = db.Personnel_Roster.Where(c => c.JobNum == jobNum).Select(c => c.Name).FirstOrDefault();
            if (name == null)
            {
                UserOperateLog log = new UserOperateLog
                {
                    Operator = ((Users)Session["User"]).UserName,
                    OperateDT
                = DateTime.Now,
                    OperateRecord = "锡膏出库，出库给花名册以外的人，userid是" + jobNum
                };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                return "false";

            }
            else
                return name;
        }

        #region 打印锡膏标签
        [HttpPost]
        public ActionResult InsideBoxLable_Print(List<string> barcodelist, int pagecount = 1, string ip = "", int port = 0, int concentration = 30, int leftdissmenion = 100)
        {
            //string data = "^XA^MD30~DGR:ZONE.GRF,";
            JArray result = new JArray();

            foreach (var barcode in barcodelist)
            {
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                Bitmap bm = InsideBoxLable_DrawBitmap(barcode);
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO" + leftdissmenion + ",0^XGR:ZONE.GRF^FS^XZ";//FO.X,Y座标
                string message = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                JObject obj = new JObject();
                obj.Add("barcode", barcode);
                obj.Add("message", message);
                result.Add(obj);
            }

            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpPost]
        public ActionResult InsideBoxLable_ToImg(string barcode = "")
        {
            Bitmap bm = InsideBoxLable_DrawBitmap(barcode);
            MemoryStream ms = new MemoryStream();
            bm.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            bm.Dispose();
            return File(ms.ToArray(), "image/Png");
        }


        private Bitmap InsideBoxLable_DrawBitmap(string barcode = "")
        {
            //开始绘制图片
            int initialWidth = 400, initialHeight = 150;//宽8高3
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
            //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));


            //引入条码
            Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(barcode, 388, 60);
            theGraphics.DrawImage(bmp_barcode, 5, 25, bmp_barcode.Width, bmp_barcode.Height);

            //引入条码号
            System.Drawing.Font myFont_modulebarcodenum;
            myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 15, FontStyle.Regular);
            StringFormat geshi1 = new StringFormat
            {
                Alignment = StringAlignment.Center //居中
            };
            theGraphics.DrawString(barcode, myFont_modulebarcodenum, bush, 200, 80, geshi1);
            //结束图片绘制以上都是绘制图片的代码

            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值
            return bm;
        }

        #endregion
    }

    public class SMT_Sulderpaster_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController com = new CommonalityController();
        private CommonController comom = new CommonController();

        // GET: SMT_Sulderpaster
        //锡膏入库
        public class TempWarehous
        {
            public string Barcode { get; set; }
            public DateTime? Time { get; set; }
            public DateTime? Intotime { get; set; }

            public int Day { get; set; }

        }

        #region MC 物料表
        //查看物料信息
        [HttpPost]
        [ApiAuthorize]
        public JObject SelctMaterial()
        {
            var meterial = db.Warehouse_Material_BaseInfo.Where(c => c.VarietyType == "锡膏").ToList();//查找锡膏的物料信息
            JArray result = new JArray();
            //result.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(meterial)));
            foreach (var item in meterial)
            {
                JObject jObject = new JObject();
                var num = item.MaterialNumber.ToString();
                var barcodelist = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == num).Select(c => c.SolderpasterBacrcode).ToList();
                var count = db.Warehouse_Freezer.Count(c => barcodelist.Contains(c.SolderpasterBacrcode) && c.WarehouseOutTime == null);
                jObject.Add("ID", item.ID);
                jObject.Add("ManufactorNum", item.ManufactorNum);
                jObject.Add("ManufactorName", item.ManufactorName);
                jObject.Add("MaterialNumber", item.MaterialNumber);
                jObject.Add("ProductName", item.ProductName);
                jObject.Add("Specifications", item.Specifications);
                jObject.Add("Type", item.Type);
                jObject.Add("VarietyType", item.VarietyType);
                jObject.Add("count", count);
                result.Add(jObject);
            }
            return comom.GetModuleFromJarray(result);
        }

        //修改物料信息(暂时没用)
        [HttpPost]
        [ApiAuthorize]
        public void UpdateMaterial(Warehouse_Material_BaseInfo warehouse_Material_BaseInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(warehouse_Material_BaseInfo).State = EntityState.Modified;
                db.SaveChanges();
            }
        }


        /// <summary>
        /// 录入物料信息
        /// </summary>
        /// 根据传过来的数据,判断是否有相同物料编号的数据,没有则添加数据,有则提示错误
        /// <param name="warehouse_Material">物料基本信息</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject Addwarehouse_MaterialAsync([System.Web.Http.FromBody]JObject data)
        {
            List<Warehouse_Material_BaseInfo> warehouse_Material = JsonConvert.DeserializeObject<List<Warehouse_Material_BaseInfo>>(JsonConvert.SerializeObject(data));
            int count = 0;
            JObject message = new JObject();
            if (ModelState.IsValid)//判断传过来的格式是否正确
            {
                foreach (var item in warehouse_Material)//循环传过来的数据
                {
                    var isExit = db.Warehouse_Material_BaseInfo.Count(c => c.MaterialNumber == item.MaterialNumber);//判断是否有相同物料编号的数据
                    if (isExit > 0)//如果有提示重复错误
                    {
                        return comom.GetModuleFromJarray(null, false, "已有重复的物料号");
                    }
                    db.Warehouse_Material_BaseInfo.Add(item);//没有则往数据库添加数据
                    count += db.SaveChanges();
                }
                if (count == warehouse_Material.Count())//返回成功信息
                {
                    return comom.GetModuleFromJarray(null, true, "录入成功");
                }
            }
            return comom.GetModuleFromJarray(null, false, "输入的数据格式不对");
        }


        #endregion

        #region MC 看板
        /// <summary>
        /// MC 看板
        /// </summary>
        /// <param name="barcode">锡膏条码</param>
        /// <param name="materialNumber">物料编号</param>
        /// <param name="batch">批次</param>
        /// <param name="total">是否全部</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject MCBoard([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            string barcode = datavalue.barcode;
            string materialNumber = datavalue.materialNumber;
            string batch = datavalue.batch;
            bool total = datavalue.total ?? false;
            //string barcode, string materialNumber, string batch, bool total = false
            var barcodeList = db.Barcode_Solderpaste.ToList();//得到所有锡膏条码表信息
            if (!string.IsNullOrEmpty(materialNumber))//判断物料编号是否为空
            {
                barcodeList = barcodeList.Where(c => c.MaterialNumber == materialNumber).ToList();
            }
            if (!string.IsNullOrEmpty(batch))//判断批次是否为空
            {
                barcodeList = barcodeList.Where(c => c.Batch == batch).ToList();
            }
            if (!string.IsNullOrEmpty(barcode))//判断条码是否为空
            {
                barcodeList = barcodeList.Where(c => c.SolderpasterBacrcode == barcode).ToList();
            }
            var freeze = db.Warehouse_Freezer.Select(c => new { c.WarehouseOutTime, c.WarehouseOutUserName, c.SolderpasterBacrcode }).ToList();
            JArray result = new JArray();
            foreach (var item in barcodeList)//循环最后筛选好的锡膏数据
            {
                JObject jobjectitem = new JObject();

                var warehouse = freeze.Where(c => c.SolderpasterBacrcode == item.SolderpasterBacrcode).FirstOrDefault();//查找冰柜表里是否有信息
                if (warehouse == null)//没有信息
                    jobjectitem.Add("statue", "未入冰柜");
                else
                {
                    if (warehouse.WarehouseOutTime != null)//有信息在判断是否已出库或入库
                    {
                        if (total)//是否是全部,如果是全部,有出库信息显示出库信息,不是勾选全部,有出库信息则跳过
                        {
                            if (warehouse.WarehouseOutUserName == "报废出库")
                            {
                                jobjectitem.Add("statue", "已报废");
                            }
                            else
                            {
                                jobjectitem.Add("statue", "已出库");
                            }
                        }
                        else
                            continue;
                    }
                    else
                        jobjectitem.Add("statue", "已入库");

                }
                //锡膏条码
                jobjectitem.Add("barcode", item.SolderpasterBacrcode);
                //物料号
                jobjectitem.Add("MaterialNumber", item.MaterialNumber);
                //物料号
                jobjectitem.Add("Batch", item.Batch);
                //供应商
                jobjectitem.Add("Supplier", item.Supplier);
                //生产时间
                jobjectitem.Add("LeaveFactoryTime", item.LeaveFactoryTime);
                //型号
                jobjectitem.Add("SolderpasteType", item.SolderpasteType);
                //剩余有效时间
                var timespan = item.EffectiveDay - (DateTime.Now - item.LeaveFactoryTime).Value.Days;
                jobjectitem.Add("effTime", timespan);
                result.Add(jobjectitem);
            }
            return comom.GetModuleFromJarray(result);
        }
        #endregion

        #region MC 锡膏出入库 

        /// <summary>
        /// 仓库基本信息录入(部门和班组放在对象里面)
        /// </summary>
        /// 判断有没有相同物料编号和批次的条码信息,有则返回错误,没有则录入信息,生产条码,条码要先判断是否有相同物料号相同出厂日期的条码,有的话在最后的条码序列号+1,生产新条码,没有则序列号从1开始
        /// <param name="warehouse_Material_InPut">仓库基本信息</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public JObject AddWarehouseBaseInfo([System.Web.Http.FromBody]JObject data)
        {
            Warehouse_Material_InPut warehouse_Material_InPut = JsonConvert.DeserializeObject<Warehouse_Material_InPut>(JsonConvert.SerializeObject(data["warehouse_Material_InPut"]));
            string UserName = data["UserName"].ToString();
            int UserNum = int.Parse(data["UserName"].ToString());

            if (ModelState.IsValid)//判断数据格式是否正确
            {
                if (db.Warehouse_Material_InPut.Count(c => c.MaterialNumber == warehouse_Material_InPut.MaterialNumber && c.Batch == warehouse_Material_InPut.Batch) != 0)//判断基本信息表里有没有相同的物料编号和批次,有则返回错误null
                {
                    return comom.GetModuleFromJarray(null,false,"没有找到此料号和批次");
                }
                warehouse_Material_InPut.CheckTime = DateTime.Now;//录入时间
                warehouse_Material_InPut.Username = UserName;//录入人姓名
                warehouse_Material_InPut.JobNum = UserNum.ToString();//录入人工号
                db.Warehouse_Material_InPut.Add(warehouse_Material_InPut);//添加数据

                #region 生产条码
                if (!string.IsNullOrEmpty(warehouse_Material_InPut.MaterialNumber) || warehouse_Material_InPut.LeaveFactoryTime != null)//判断物料编号和出厂时间是否未空
                {
                    string year = warehouse_Material_InPut.LeaveFactoryTime.Value.Year.ToString().Substring(2);//得到出厂时间的年后两位
                    JArray result = new JArray();
                    int count = 0;
                    List<Barcode_Solderpaste> barcodelist = new List<Barcode_Solderpaste>();
                    //查找锡膏条码前缀一样的最后一个序列号
                    var lastBarcode = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == warehouse_Material_InPut.MaterialNumber && c.LeaveFactoryTime == warehouse_Material_InPut.LeaveFactoryTime).OrderByDescending(c => c.SolderpasterBacrcode).FirstOrDefault();//找到相同物料编号,相同出厂时间符合条件的最后一个锡膏条码
                    for (int i = 1; i <= warehouse_Material_InPut.SolderpasteNum; i++)
                    {
                        int num = 1;

                        if (lastBarcode != null)//符合条件的条码不为空
                        {
                            string lastNUm = lastBarcode.SolderpasterBacrcode.Substring(lastBarcode.SolderpasterBacrcode.Length - 3, 3);//找到条码的最后序列号
                            num = int.Parse(lastNUm) + 1;//序列号加1
                        }

                        string barcode = warehouse_Material_InPut.MaterialNumber + "-" + year + warehouse_Material_InPut.LeaveFactoryTime.Value.Month.ToString().PadLeft(2, '0') + warehouse_Material_InPut.LeaveFactoryTime.Value.Day.ToString().PadLeft(2, '0') + "-" + num.ToString().PadLeft(3, '0');//组成条码 物料编号+出产时间的年月日+序列编号

                        Barcode_Solderpaste barcode_Solderpaste = new Barcode_Solderpaste() { SolderpasterBacrcode = barcode, Batch = warehouse_Material_InPut.Batch, LeaveFactoryTime = warehouse_Material_InPut.LeaveFactoryTime, MaterialNumber = warehouse_Material_InPut.MaterialNumber, SolderpasteType = warehouse_Material_InPut.SolderpasteType, Supplier = warehouse_Material_InPut.Supplier, PrintTime = DateTime.Now, PrintName = UserName, EffectiveDay = (int)warehouse_Material_InPut.EffectiveDay };//新建条码的数据信息
                        barcodelist.Add(barcode_Solderpaste);
                        result.Add(barcode);
                    }
                    if (count == warehouse_Material_InPut.SolderpasteNum + 1)
                    {
                        db.Barcode_Solderpaste.AddRange(barcodelist);//添加数据库
                        count += db.SaveChanges();
                        return comom.GetModuleFromJarray(result,true,"成功");
                    }
                }
                #endregion

            }
            return comom.GetModuleFromJarray(null, false, "数据格式不对");

        }

        /// <summary>
        /// 输入物料号，返回物料信息
        /// </summary>
        /// <param name="Material">物料号</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [ApiAuthorize]
        public JObject MaterialInfo([System.Web.Http.FromBody]JObject data)
        {
            string Material = data["Material"].ToString();
            var info = db.Warehouse_Material_BaseInfo.Where(c => c.MaterialNumber == Material).FirstOrDefault();//根据物料号找到物料基本信息
            JObject result = new JObject();
            if (info == null)
            {
                return null;
            }
            result.Add("type", info.Type);//类型
            //厂商简称
            result.Add("ManufactorName", info.ManufactorName);
            //厂商编号
            result.Add("ManufactorNum", info.ManufactorNum);
            return comom.GetModuleFromJobjet(result);
        }


        /// <summary>
        /// 入仓库冰柜
        /// </summary>
        /// <param name="warehouse_FreezerList">条码列表</param>
        /// <param name="warehouseNum">库位号</param>
        /// <param name="group">班组</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public void AddWarehouseFreezerAsync([System.Web.Http.FromBody]JObject data)
        {
            List<string> warehouse_FreezerList = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(data["warehouse_FreezerList"]));
            string warehouseNum = data["warehouseNum"].ToString();
            string Department = data["Department"].ToString();
            string group = data["group"].ToString();
            string UserName = data["UserName"].ToString();
            int UserNum = int.Parse(data["UserNum"].ToString());

            int count = 0;
            List<Warehouse_Freezer> freezers = new List<Warehouse_Freezer>();
            foreach (var item in warehouse_FreezerList)//循环条码列表
            {
                Warehouse_Freezer warehouse_Freezer = new Warehouse_Freezer
                {
                    SolderpasterBacrcode = item,
                    WarehouseNum = warehouseNum,
                    IntoTime = DateTime.Now,
                    UserName = UserName,
                    JobNum = UserNum.ToString(),
                    Group = group,
                    Department = Department
                };
                freezers.Add(warehouse_Freezer);

            }
            db.Warehouse_Freezer.AddRange(freezers);//新增信息
             db.SaveChanges();//保存数据
        }

        /// <summary>
        /// 出仓库冰柜(参数 name->UserName;jobnum->UserNum;group->Group;增加参数ShouldBeBarcodelist(扫了10个条码出库,这个ShouldBeBarcodelist就是下方列表清单的前10个))
        /// </summary>
        /// 找到根据入库时间排序的条码列表,将传过来的条码列表记录出库后,判断传过来的列表是否在根据入库时间排序的条码列表的前面,不是在前面的写出日志
        /// <param name="warehouse_FreezerList">条案列表</param>
        /// <param name="name">领料人姓名</param>
        /// <param name="jobnum">领料人工号</param>
        /// <param name="group">班组</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize]
        public void UpdateWarehouseFreezerAsync([System.Web.Http.FromBody]JObject data)
        {
            List<string> warehouse_FreezerList = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(data["warehouse_FreezerList"]));
            List<string> ShouldBeBarcodelist = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(data["ShouldBeBarcodelist"]));
            string UserName = data["UserName"].ToString();
            string UserNum = data["UserNum"].ToString();
            string Department = data["Department"].ToString();
            string Group = data["Group"].ToString();

            int count = 0;
            foreach (var item in warehouse_FreezerList)//循环传过来的条码列表
            {
                var warehouse = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();//修改数据
                warehouse.WarehouseOutTime = DateTime.Now;
                warehouse.WarehouseOutUserName = UserName;
                warehouse.WarehouseOutJobNum = UserNum;
                warehouse.WarehouseOutDep = Department;
                warehouse.WarehouseOutGroup = Group;
                db.Entry(warehouse).State = System.Data.Entity.EntityState.Modified;
                count += db.SaveChanges();
            }
                //填写日志
                //找不到没有在应出库列表的列表
                var not = ShouldBeBarcodelist.Except(warehouse_FreezerList).ToList();
                if (not.Count != 0)
                {
                    string barcode = string.Join(",", not.ToArray());
                    UserOperateLog log = new UserOperateLog() { OperateDT = DateTime.Now, Operator = UserName, OperateRecord = "锡膏条码" + barcode + "没有在本次的出库列表中,它应该是要出库的" };
                    db.UserOperateLog.Add(log);
                    db.SaveChanges();
                }
            
        }

        //输入工号显示人名
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayName([System.Web.Http.FromBody]JObject data)
        {
            string jobNum = data["jobNum"].ToString();
            string UserName = data["UserName"].ToString();

            var name = db.Personnel_Roster.Where(c => c.JobNum == jobNum).Select(c => c.Name).FirstOrDefault();
            if (name == null)
            {
                UserOperateLog log = new UserOperateLog
                {
                    Operator = UserName,
                    OperateDT
                = DateTime.Now,
                    OperateRecord = "锡膏出库，出库给花名册以外的人，userid是" + jobNum
                };
                db.UserOperateLog.Add(log);
                db.SaveChanges();
                return comom.GetModuleFromJarray(null,false,"false");

            }
            else
                return comom.GetModuleFromJarray(null, true, name);
        }
        //出库列表显示提示先入先出
        [HttpPost]
        [ApiAuthorize]
        public JObject CheckWarehouse()
        {
            var warehous = db.Warehouse_Freezer.Where(c => c.WarehouseOutTime == null).ToList();//查找所有未出库的锡膏信息
            var warehousebarcodelist = warehous.Select(c => c.SolderpasterBacrcode).ToList();
            var recly = db.SMT_Recycle.Where(c => warehousebarcodelist.Contains(c.SolderpasterBacrcode)).Select(c => c.SolderpasterBacrcode).ToList();
            var a = warehousebarcodelist.Except(recly).ToList();
            var b = db.Warehouse_Freezer.Where(c => a.Contains(c.SolderpasterBacrcode)).Select(c => new { c.SolderpasterBacrcode, c.IntoTime }).ToList();
            var tempwarehouse = new List<TempWarehous>();
            foreach (var item in b)//循环信息
            {
                var time = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item.SolderpasterBacrcode).FirstOrDefault();
                TempWarehous temp = new TempWarehous() { Barcode = item.SolderpasterBacrcode, Time = time.LeaveFactoryTime, Day = time.EffectiveDay, Intotime = item.IntoTime };
                tempwarehouse.Add(temp);//记录信息
            }
            tempwarehouse.OrderBy(c => c.Intotime).ToList();//根据入库时间进行排序

            JArray result = new JArray();
            foreach (var warehouseitem in tempwarehouse)//循环列表
            {
                JObject jObjectitem = new JObject();
                jObjectitem.Add("barcode", warehouseitem.Barcode);//条码
                //生产日期
                jObjectitem.Add("time", warehouseitem.Time);
                //有效天数
                var spam = (int)(DateTime.Now - warehouseitem.Time).Value.TotalDays;
                var effectiveday = warehouseitem.Day - spam;
                jObjectitem.Add("overdue", effectiveday);
                if (effectiveday < 0)//有效期为负数,放在第一位
                {
                    result.AddFirst(jObjectitem);
                }
                else
                {
                    result.Add(jObjectitem);
                }
            }
            return comom.GetModuleFromJarray(result);
        }

        /// <summary>
        /// 根据料号和生产时间显示已出库和未出库的条码号
        /// </summary>
        /// <param name="materialNumber">物料编号</param>
        /// <param name="leaveFactoryTime">出厂时间</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayBarcode([System.Web.Http.FromBody]JObject data)
        {
            string materialNumber = data["materialNumber"].ToString();
            DateTime leaveFactoryTime = DateTime.Parse(data["leaveFactoryTime"].ToString());
            var barocdelList = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == materialNumber && c.LeaveFactoryTime == leaveFactoryTime).Select(c => c.SolderpasterBacrcode).ToList();//根据物料号.出厂时间找到条码列表
            var warehousrJoin = db.Warehouse_Freezer.Select(c => c.SolderpasterBacrcode).Distinct().ToList();//查找冰柜表里的所有条码清单
            var except = barocdelList.Except(warehousrJoin).ToList();//拿到没有入冰柜的条码清单
            var have = barocdelList.Intersect(warehousrJoin).ToList();//拿到在并冰柜的条码清单
            JObject result = new JObject();
            result.Add("except", except.Count);
            result.Add("exceptList", JsonConvert.SerializeObject(except));
            result.Add("having", have.Count);
            // result.Add("having", have.Count);
            return comom.GetModuleFromJobjet(result);
        }

        #region 锡膏再次打印
        /// <summary>
        /// 锡膏再次打印
        /// </summary>
        /// <param name="materialNumber">物料号</param>
        /// <param name="leaveFactoryTime">出厂时间</param>
        /// <param name="batch">批次</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [ApiAuthorize]
        public JArray DisplayPrintAgainBarcode([System.Web.Http.FromBody]JObject data)
        {
            string materialNumber = data["materialNumber"].ToString();
            string batch = data["batch"].ToString();
            DateTime leaveFactoryTime = DateTime.Parse(data["leaveFactoryTime"].ToString());
            var barcodeList = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == materialNumber && c.LeaveFactoryTime == leaveFactoryTime && c.Batch == batch).Select(c => c.SolderpasterBacrcode).ToArray();//根据物料号,出厂时间,批次,找到条码信息
            JArray result = new JArray();
            foreach (var item in barcodeList)
            {
                result.Add(item);
            }
            return result;
        }

        //锡膏再打印物料号获取
        [HttpPost]
        [ApiAuthorize]
        public JObject PrintAgainMaterialNumberList()
        {
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Select(m => m.MaterialNumber).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return comom.GetModuleFromJarray(result);
        }

        //锡膏再打印生产日期获取
        [HttpPost]
        [ApiAuthorize]
        public JObject PrintAgainLeaveFactoryTimeList([System.Web.Http.FromBody]JObject data)
        {
            string materialNumber = data["materialNumber"].ToString();
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Where(c => c.MaterialNumber == materialNumber).Select(m => m.LeaveFactoryTime).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                var vaule = string.Format("{0:yyyy-MM-dd}", item);
                List.Add("value", vaule);

                result.Add(List);
            }
            return comom.GetModuleFromJarray(result);
        }
        //锡膏再打印批次获取
        [HttpPost]
        [ApiAuthorize]
        public JObject PrintAgainBatchList([System.Web.Http.FromBody]JObject data)
        {
            string materialNumber = data["materialNumber"].ToString();
            DateTime time = DateTime.Parse(data["time"].ToString());
            var orders = db.Barcode_Solderpaste.OrderByDescending(m => m.ID).Where(c => c.MaterialNumber == materialNumber && c.LeaveFactoryTime == time).Select(m => m.Batch).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return comom.GetModuleFromJarray(result);
        }
        #endregion

        //上传报废单
        [HttpPost]
        [ApiAuthorize]
        public JObject UploadImg()
        {
            JObject result = new JObject();
            List<string> SolderpasterBacrcodeList =JsonConvert.DeserializeObject < List<string> >(JsonConvert.SerializeObject( HttpContext.Current.Request["SolderpasterBacrcodeList"]));//锡膏条码列表
            HttpContextBase context = (HttpContextBase)Request.Properties["uploadfile"];//上传的问题件  这个“MS_HttpContext”参数名不需要改

            HttpRequestBase Files = context.Request;
            if (Files.Files.Count > 0)
            {
                var file = Files.Files[1];

                if (Directory.Exists(@"D:\MES_Data\SMT_Sulderpaster\Scrap\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\SMT_Sulderpaster\Scrap\");
                }
                foreach (var item in SolderpasterBacrcodeList)
                {
                    file.SaveAs(@"D:\MES_Data\SMT_Sulderpaster\Scrap\" + item + ".jpg");
                }
                return comom.GetModuleFromJarray(null, true, "成功");
            }
            return comom.GetModuleFromJarray(null, false, "没有找到上传的文件");
        }

        //显示报废单
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplayImg([System.Web.Http.FromBody]JObject data)
        {
            string suldepaster = data["suldepaster"].ToString();
            JObject result = new JObject();
           // var time = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == suldepaster).Select(c => c.WarehouseOutJobNum).FirstOrDefault();
            List<FileInfo> filesInfo = com.GetAllFilesInDirectory(@"D:\MES_Data\SMT_Sulderpaster\Scrap\");
            var file = filesInfo.Where(c => c.Name == suldepaster + ".jpg").FirstOrDefault();
            if (file != null)
            {
                result.Add("have", true);
                result.Add("path", @"D:\MES_Data\SMT_Sulderpaster\Scrap\" + suldepaster + ".jpg");
            }
            else
            {
                result.Add("have", false);
                result.Add("path", null);
            }
            return comom.GetModuleFromJobjet(result);
        }

        //更新报废单
        [HttpPost]
        [ApiAuthorize]
        public void UpdateImg([System.Web.Http.FromBody]JObject data)
        {
            List<string> suldepaster =JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(data["suldepaster"]));
            HttpContextBase context = (HttpContextBase)Request.Properties["uploadfile"];//上传的问题件  这个“MS_HttpContext”参数名不需要改
            HttpRequestBase Files = context.Request;
            if (Files.Files.Count > 0)
            {
                var file = Files.Files[1];
                //var time = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == suldepaster).Select(c => c.WarehouseOutJobNum).FirstOrDefault();
                foreach (var item in suldepaster)
                {
                    file.SaveAs(@"D:\MES_Data\SMT_Sulderpaster\Scrap\" + item + ".jpg");
                }
            }
        }
        #endregion

        /// <summary>
        /// 条码检验
        /// </summary>
        /// <param name="smt_FreezerList">条码列表</param>
        /// <param name="statu">例：仓库，冰柜，回温，领用</param>
        /// <param name="belogin">冰柜专属 如：仓库，回温，领用</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [ApiAuthorize]
        public JObject CheckSMTFreezer([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            List<string> smt_FreezerList = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(datavalue.smt_FreezerList));
            string statu = datavalue.statu;
            string belogin = datavalue.belogin;
            //List<string> smt_FreezerList, string statu, string belogin = null
            JArray result = new JArray();
            foreach (var item in smt_FreezerList)//循环条码列表
            {
                JObject jObject = new JObject();
                jObject.Add("barcode", item);
                var startime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.PrintTime).FirstOrDefault();//根据条码,在条码表找信息
                if (startime == null)//如果没找到信息,提示错误
                {
                    jObject.Add("Tips", "没有找到此条码");
                }
                else
                {
                    var spantime = com.TwoDTforMonth_sub(DateTime.Now, startime);//计算锡膏有效期
                    var recyle = db.SMT_Recycle.Count(c => c.SolderpasterBacrcode == item);//在回收表是否有信息
                    if (statu == "回收")
                    {
                        if (recyle != 0)
                            jObject.Add("Tips", "此条码已回收");
                        else
                            jObject.Add("Tips", "正常");
                    }
                    else if (spantime >= 5 && spantime < 6)
                    {
                        jObject.Add("Tips", "将过期");
                    }
                    else if (spantime >= 6)
                    {
                        jObject.Add("Tips", "已过期");
                    }

                    else if (recyle != 0)
                    {
                        jObject.Add("Tips", "此条码已回收");
                    }
                    else
                    {
                        if (statu == "冰柜")
                        {
                            if (string.IsNullOrEmpty(belogin))
                            {
                                jObject.Add("Tips", "未选择来源工序");
                            }
                            else
                            {
                                var freezecount = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item && c.Belogin == belogin).OrderByDescending(c => c.IntoTime).Select(c => c.IntoTime).FirstOrDefault();
                                var emply = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                                if (belogin == "仓库")
                                {
                                    if (freezecount != null)
                                    { jObject.Add("Tips", "此条码已存在SMT冰柜中"); }
                                    else
                                        jObject.Add("Tips", "正常");
                                }
                                else if (belogin == "回温")
                                {
                                    var warmcount = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                                    if (warmcount <= freezecount)
                                    { jObject.Add("Tips", "此条码已存在SMT冰柜中"); }
                                    else if (warmcount == null)
                                    { jObject.Add("Tips", "此条码未经过回温"); }
                                    else if (warmcount < emply)
                                    { jObject.Add("Tips", "此条码已经领用过了"); }
                                    else
                                        jObject.Add("Tips", "正常");
                                }
                                else if (belogin == "领用")
                                {
                                    //var emply = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                                    if (emply <= freezecount)
                                    { jObject.Add("Tips", "此条码已存在SMT冰柜中"); }
                                    else if (emply == null)
                                    { jObject.Add("Tips", "此条码未使用过"); }
                                    else
                                        jObject.Add("Tips", "正常");
                                }
                            }
                        }
                        else if (statu == "回温")
                        {
                            var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.IntoTime).Select(c => c.IntoTime).FirstOrDefault();
                            var warm = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                            if (warm != null && warm >= freeze)
                            { jObject.Add("Tips", "此条码已在回温中"); }
                            else
                                jObject.Add("Tips", "正常");
                        }
                        else if (statu == "领用")
                        {
                            var warm = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                            var stri = db.SMT_Stir.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();

                            var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.IntoTime).Select(c => c.IntoTime).FirstOrDefault();
                            //var emply = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item).OrderByDescending(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault();
                            if (stri < warm || stri == null)
                            {
                                jObject.Add("Tips", "此条码未经过搅拌");
                            }
                            //else if (emply != null || emply >= freeze)
                            //{ jObject.Add("Tips", "此条码已经在使用中"); }
                            else
                                jObject.Add("Tips", "正常");
                        }
                        else if (statu == "出库")
                        {
                            var warehouse = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                            if (warehouse == null)
                            { jObject.Add("Tips", "此条码未入库"); }
                            else
                            {
                                if (warehouse.WarehouseOutTime != null)
                                    jObject.Add("Tips", "此条码已出库");
                                else
                                    jObject.Add("Tips", "正常");
                            }
                        }
                        else if (statu == "仓库")
                        {
                            var warehouse = db.Warehouse_Freezer.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                            if (warehouse != null)
                            { jObject.Add("Tips", "此条码已入库"); }
                            else
                            {
                                jObject.Add("Tips", "正常");
                            }
                        }
                        else
                        {
                            jObject.Add("Tips", "正常");
                        }
                    }
                }
                result.Add(jObject);
            }
            return comom.GetModuleFromJarray(result);
        }

        #region SMT 入冰柜
        
        //入SMT冰柜下边列表
        [HttpPost]
        [ApiAuthorize]
        public JObject CheckSMTFreezeList()
        {
            //DateTime lasrtime = new DateTime(2020, 3, 1);
            var smtfree = db.SMT_Freezer.Select(c => c.SolderpasterBacrcode).ToList();
            var recly = db.SMT_Recycle.Select(c => c.SolderpasterBacrcode).ToList();

            var mcfree = db.Warehouse_Freezer.Where(c => c.WarehouseOutTime != null && c.WarehouseOutUserName != "报废出库").Select(c => c.SolderpasterBacrcode).ToList();
            var exceptSMT = mcfree.Except(smtfree).ToList();
            var exceptRecly = exceptSMT.Except(recly).ToList();
            var waresfree = db.Warehouse_Freezer.Select(c => new TempWarehous { Barcode = c.SolderpasterBacrcode, Intotime = c.IntoTime }).ToList();
            var barcodesolder = db.Barcode_Solderpaste.Select(c => new TempWarehous { Barcode = c.SolderpasterBacrcode, Time = c.LeaveFactoryTime, Day = c.EffectiveDay });
            List<string> distin = new List<string>();
            //排序
            //var orderbydate=rewarm
            JArray result = new JArray();
            foreach (var item in exceptRecly)
            {
                JObject rejobject = new JObject();

                rejobject.Add("barcode", item);
                //var time = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).Max(c => c.StartTime);
                var time = waresfree.Where(c => c.Barcode == item).Max(c => c.Intotime);
                var imespan = DateTime.Now - time;
                rejobject.Add("freezespan", imespan.Value.TotalSeconds);

                var productiontime = barcodesolder.Where(c => c.Barcode == item).Select(c => c.Time).FirstOrDefault();
                if (productiontime == null)
                {
                    continue;
                }
                var span = (DateTime.Now.Date - productiontime).Value.TotalDays;
                var effiday = barcodesolder.Where(c => c.Barcode == item).Select(c => c.Day).FirstOrDefault();
                rejobject.Add("overdue", effiday - span);

                result.Add(rejobject);

                distin.Add(item);
            }
            return comom.GetModuleFromJarray(result);
        }


        //入SMT冰柜 数据存储操作
        [HttpPost]
        [ApiAuthorize]
        public void AddSMTFreezerAsync([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            List<string> smt_FreezerList = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(datavalue.smt_FreezerList));
            string group = datavalue.group;
            string Department = datavalue.Department;
            string belogin = datavalue.belogin;
            string UserName = datavalue.UserName;
            string UserNum = datavalue.UserNum;
            //List<string> smt_FreezerList, string belogin, string group
            List<SMT_Freezer> count = new List<SMT_Freezer>();
            foreach (var item in smt_FreezerList)
            {
                SMT_Freezer smt_Freezer = new SMT_Freezer();
                smt_Freezer.SolderpasterBacrcode = item;
                smt_Freezer.IntoTime = DateTime.Now;
                smt_Freezer.UserName = UserName;
                smt_Freezer.JobNum = UserNum.ToString();
                smt_Freezer.Group = group;
                smt_Freezer.Department = Department;
                smt_Freezer.Belogin = belogin;

                count.Add(smt_Freezer);

            }
            db.SMT_Freezer.AddRange(count);
            db.SaveChanges();
        }

        #endregion

        #region SMT 回温
        //回温输入下边列表
        [HttpPost]
        [ApiAuthorize]
        public JObject CheckSMTRewarm()
        {
            //找到回温 去掉重复并开始时间最后的一条
            var lastrewarm = (from g in db.SMT_Rewarm group g by g.SolderpasterBacrcode into s select new { time = s.Max(c => c.StartTime), barcode = s.Key }).ToList();
            //找到入SMT 去掉重复并开始时间最后的一条
            var lastfree = (from g in db.SMT_Freezer group g by g.SolderpasterBacrcode into s select new { time = s.Max(c => c.IntoTime), barcode = s.Key }).ToList();
            //拿到相同条码中,入SMT时间小于回温时间的条码列表(即已经在回温,或者回温结束)
            var finshrewarm = lastfree.Select(c => lastrewarm.Where(s => c.barcode == s.barcode && c.time <= s.time).Select(a => a.barcode).FirstOrDefault()).ToList();
            //入smt全部条码列表,按入库时间排序
            var rewarm = lastfree.OrderBy(c => c.time).Select(c => c.barcode).ToList();
            //回收的条码列表
            var recly = db.SMT_Recycle.Select(c => c.SolderpasterBacrcode).ToList();
            //var rewarm = (from s in db.SMT_Freezer group s by s.SolderpasterBacrcode into g select g.Key).ToList();
            //入库去掉回收条码列表
            var rewarmlist = rewarm.Except(recly).ToList();
            //入库去掉回温的条码类表
            var rewarmlist2 = rewarmlist.Except(finshrewarm).ToList();

            //排序
            //var orderbydate=rewarm
            JArray result = new JArray();
            foreach (var item in rewarmlist2)
            {
                JObject rejobject = new JObject();
                rejobject.Add("barcode", item);
                //var time = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).Max(c => c.StartTime);
                var time = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).Max(c => c.IntoTime);
                var imespan = DateTime.Now - time;
                rejobject.Add("freezespan", imespan.Value.TotalSeconds);

                var productiontime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.LeaveFactoryTime).FirstOrDefault();
                if (productiontime == null)
                {
                    continue;
                }
                var span = (DateTime.Now.Date - productiontime).Value.TotalDays;
                var effiday = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.EffectiveDay).FirstOrDefault();
                rejobject.Add("overdue", effiday - span);

                result.Add(rejobject);


            }
            return comom.GetModuleFromJarray(result);
        }


        //开始回温 数据存储操作
        [HttpPost]
        [ApiAuthorize]
        public void AddSMTRewarmAsync([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            List<string> smt_RewarmList = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(datavalue.smt_RewarmList));
            string group = datavalue.group;
            string Department = datavalue.Department;
            string UserName = datavalue.UserName;
            string UserNum = datavalue.UserNum;

            List<SMT_Rewarm> count = new List<SMT_Rewarm>();
            foreach (var item in smt_RewarmList)
            {
                SMT_Rewarm smt_Rewarm = new SMT_Rewarm();
                smt_Rewarm.SolderpasterBacrcode = item;
                smt_Rewarm.StartTime = DateTime.Now;
                smt_Rewarm.UserName = UserName;
                smt_Rewarm.JobNum = UserNum;
                smt_Rewarm.Department = Department;
                smt_Rewarm.Group = group;
                count.Add(smt_Rewarm);
            }
            db.SMT_Rewarm.AddRange(count);
            db.SaveChanges();
        }


        //回温记录
        [HttpPost]
        [ApiAuthorize]
        public JObject RewarmInfo()
        {
            //回温去掉重复,那最后回温数据
            var rewarm = (from g in db.SMT_Rewarm group g by g.SolderpasterBacrcode into x select new { time = x.Max(g => g.StartTime), barcode = x.Key }).ToList();
            //入冰柜去掉重复,拿最后的那条数据
            var smtFree = (from g in db.SMT_Freezer group g by g.SolderpasterBacrcode into x select new { time = x.Max(g => g.IntoTime), barcode = x.Key }).ToList();
            //搅拌去掉重复,拿最后搅拌那条数据
            var sit = (from g in db.SMT_Stir group g by g.SolderpasterBacrcode into x select new { time = x.Max(g => g.StartTime), barcode = x.Key }).ToList();
            //找到回收列表
            var recly = db.SMT_Recycle.Select(c => c.SolderpasterBacrcode).ToList();
            //找到 回温时间<=入冰柜时间的(不在回温中)
            var notworkRewarm = rewarm.Select(c => smtFree.Where(s => c.barcode == s.barcode && c.time <= s.time).Select(a => a.barcode).FirstOrDefault()).ToList();
            //找到 回温时间<=搅拌时间(不在回温中)
            var notworkRewarm2 = rewarm.Select(c => sit.Where(s => c.barcode == s.barcode && c.time <= s.time).Select(a => a.barcode).FirstOrDefault()).ToList();

            // workRewarm2.RemoveAll(c => c == null);
            var rewarmList = rewarm.Select(c => c.barcode).ToList();
            var exceptFree = rewarmList.Except(notworkRewarm).ToList();
            var exceptSit = exceptFree.Except(notworkRewarm2).ToList();
            var exceptRecly = exceptSit.Except(recly).ToList();


            //List<string> distin = new List<string>();
            //排序
            //var orderbydate=rewarm
            JArray result = new JArray();
            foreach (var item in exceptRecly)
            {

                JObject rejobject = new JObject();
                //var warmcount = db.SMT_Rewarm.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == item).Select(c => c.StartTime).FirstOrDefault();
                //var sitcount = db.SMT_Stir.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == item).Select(c => c.StartTime).FirstOrDefault();
                //var free = db.SMT_Freezer.OrderByDescending(c => c.IntoTime).Where(c => c.SolderpasterBacrcode == item).Select(c => c.IntoTime).FirstOrDefault();
                //if ((warmcount != null && free != null && warmcount > free) && ((warmcount != null && sitcount == null) || (warmcount != null && sitcount != null && sitcount < warmcount)))
                //{
                rejobject.Add("barcode", item);
                var time = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item).Max(c => c.StartTime);
                var imespan = DateTime.Now - time;
                rejobject.Add("retimespan", imespan.Value.TotalSeconds);

                var productiontime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.LeaveFactoryTime).FirstOrDefault();
                if (productiontime == null)
                {
                    continue;
                }
                var span = (DateTime.Now.Date - productiontime).Value.TotalDays;
                var effiday = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => c.EffectiveDay).FirstOrDefault();
                rejobject.Add("overdue", effiday - span);

                result.Add(rejobject);
                //}
            }
            return comom.GetModuleFromJarray(result);
        }


        //回温记录搜索
        [HttpPost]
        [ApiAuthorize]
        public JObject RewarmInfo([System.Web.Http.FromBody]JObject data)
        {
            string barocde = data["barocde"].ToString();
            var barcode = db.SMT_Rewarm.Count(c => c.SolderpasterBacrcode == barocde);
            if (barcode != 0)
            {
                JObject rejobject = new JObject();
                var warmcount = db.SMT_Rewarm.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == barocde).Select(c => c.StartTime).FirstOrDefault();
                var sitcount = db.SMT_Stir.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == barocde).Select(c => c.StartTime).FirstOrDefault(); ;
                if ((warmcount != null && sitcount == null) || (warmcount != null && sitcount != null && sitcount < warmcount))
                {
                    rejobject.Add("barcode", barocde);
                    var time = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == barocde).Max(c => c.StartTime);
                    var imespan = DateTime.Now - time;
                    rejobject.Add("retimespan", imespan.Value.TotalSeconds);

                    var productiontime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == barocde).Select(c => c.LeaveFactoryTime).FirstOrDefault();
                    if (productiontime == null)
                    {
                        return comom.GetModuleFromJobjet(null, false, "找不到此条码");
                    }
                    var span = (DateTime.Now.Date - productiontime).Value.TotalDays;
                    var effiday = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == barocde).Select(c => c.EffectiveDay).FirstOrDefault();
                    rejobject.Add("overdue", effiday - span);

                    return comom.GetModuleFromJobjet(rejobject);
                }
                return comom.GetModuleFromJobjet(null, false, "此条码没有正在的回温记录");
            }
            return comom.GetModuleFromJobjet(null, false, "找不到此条码");
        }
        #endregion

        #region SMT 搅拌
        [HttpPost]
        [ApiAuthorize]
        public JObject DisplatStirInfo([System.Web.Http.FromBody]JObject data)
        {
            string SolderpasterBacrcode = data["SolderpasterBacrcode"].ToString();
            JObject result = new JObject();
            var free = db.SMT_Freezer.OrderByDescending(c => c.IntoTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).Select(c => c.IntoTime).FirstOrDefault();
            var warmtime = db.SMT_Rewarm.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).Select(c => c.StartTime).FirstOrDefault();
            //var user = db.SMT_Employ.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).Select(c => c.StartTime).FirstOrDefault();
            var info = db.SMT_Stir.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).FirstOrDefault();

            //没有回温记录,并且回温记录小于领用记录
            if (warmtime == null || (warmtime != null && warmtime < free))
            {
                result.Add("first", null);
                result.Add("second", null);
                result.Add("three", null);
                result.Add("flow", null);
                result.Add("message", "此条码还未回温");
                result.Add("canadd", false);
            }
            else
            {
                //有回温没有搅拌记录和 有回温有搅拌有领用，但回温记录大于搅拌记录
                if (info == null || (info != null && warmtime > info.StartTime))
                {
                    result.Add("first", null);
                    result.Add("second", null);
                    result.Add("three", null);
                    result.Add("flow", null);
                    result.Add("message", "此条码还未开始搅拌");
                    result.Add("canadd", true);
                }
                //有回温有搅拌,但回温小于搅拌
                else if (info != null && warmtime != null & warmtime < info.StartTime)
                {

                    result.Add("first", info.StartTime);
                    result.Add("second", info.SecondTime);
                    result.Add("three", info.ThreeTime);
                    result.Add("flow", info.FlorTime);
                    result.Add("message", null);
                    result.Add("canadd", true);

                }
            }
            return comom.GetModuleFromJobjet(result);
        }

        //开始搅拌 数据存储操作
        [HttpPost]
        [ApiAuthorize]
        public void AddSMTStir([System.Web.Http.FromBody]JObject data)
        {

            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));

            string SolderpasterBacrcode = datavalue.SolderpasterBacrcode;
            int num = datavalue.num;
            string group = datavalue.group;
            string Department = datavalue.Department;
            string UserName = datavalue.UserName;
            string UserNum = datavalue.UserNum;

            var sMT_Stir = db.SMT_Stir.OrderByDescending(c => c.StartTime).Where(c => c.SolderpasterBacrcode == SolderpasterBacrcode).FirstOrDefault();
            switch (num)
            {
                case 1:
                    SMT_Stir stir = new SMT_Stir() { SolderpasterBacrcode = SolderpasterBacrcode, UserName = UserName, JobNum = UserNum.ToString(), StartTime = DateTime.Now, Department = Department, Group = group };
                    db.SMT_Stir.Add(stir);

                    break;
                case 2:
                    if (sMT_Stir.SecondTime == null)
                    {
                        sMT_Stir.SecondTime = DateTime.Now;
                        sMT_Stir.SecondName = UserName;
                        sMT_Stir.SecondJobNum = UserNum.ToString();
                    }
                    break;
                case 3:
                    if (sMT_Stir.ThreeTime == null)
                    {
                        sMT_Stir.ThreeTime = DateTime.Now;
                        sMT_Stir.ThreeJobName = UserName;
                        sMT_Stir.ThreeNum = UserNum.ToString();
                    }
                    break;
                case 4:
                    if (sMT_Stir.FlorTime == null)
                    {
                        sMT_Stir.FlorTime = DateTime.Now;
                        sMT_Stir.FlorName = UserName;
                        sMT_Stir.FlorJobNum = UserNum.ToString();
                    }
                    break;
                default:

                    break;
            }
            db.SaveChanges();

        }
        #endregion
        
        #region SMT 使用
        [HttpPost]
        [ApiAuthorize]
        public void AddEmployAsync([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            List<string> smt_EmployList = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(datavalue.smt_EmployList));
            string group = datavalue.group;
            string Department = datavalue.Department;
            string UserName = datavalue.UserName;
            string UserNum = datavalue.UserNum;
            string ordernum = datavalue.ordernum;
            int line = datavalue.line;

            foreach (var item in smt_EmployList)
            {
                SMT_Employ sMT_Employ = new SMT_Employ() { SolderpasterBacrcode = item, StartTime = DateTime.Now, JobNum = UserNum.ToString(), UserName = UserName, OrderNum = ordernum, LineNum = line, Department = Department, Group = group };
                db.SMT_Employ.Add(sMT_Employ);
                db.SaveChanges();
            }

        }

        #endregion

        #region SMT 回收
        [HttpPost]
        [ApiAuthorize]
        public void RecycleAsync([System.Web.Http.FromBody]JObject data)
        {
            var datavalue = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(data));
            List<string> smt_RecycleList = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(datavalue.smt_RecycleList));
            string group = datavalue.group;
            string Department = datavalue.Department;
            string UserName = datavalue.UserName;
            string UserNum = datavalue.UserNum;

            List<SMT_Recycle> count = new List<SMT_Recycle>(); ;
            foreach (var item in smt_RecycleList)
            {
                SMT_Recycle sMT_Recycle = new SMT_Recycle() { SolderpasterBacrcode = item, RecoveTime = DateTime.Now, JobNum = UserNum.ToString(), UserName = UserName, Department = Department, Group = group };
                count.Add(sMT_Recycle);
            }
            db.SMT_Recycle.AddRange(count);
            db.SaveChanges();
        }
        #endregion
        
        #region SMT 看板
        public JObject Boadr()
        {
            JArray result = new JArray();
            var freezebarcode = db.SMT_Freezer.Select(c => c.SolderpasterBacrcode).Distinct();
            var yesterday = DateTime.Now.AddDays(-1);
            var exceptbarcode = db.SMT_Recycle.Where(c => c.RecoveTime < yesterday).Select(c => c.SolderpasterBacrcode).ToList();
            var barcodes = freezebarcode.Except(exceptbarcode).ToList();
            foreach (var item in barcodes)
            {

                JObject barcodejobject = new JObject();

                var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderBy(c => c.IntoTime).ToList();
                for (int i = 0; i < freeze.Count(); i++)
                {
                    //SMT冰柜时间
                    JObject timeList = new JObject();
                    timeList.Add("barcode", item);
                    var freetime = freeze[i].IntoTime;
                    var freetiem2 = new DateTime?();
                    if (i + 1 == freeze.Count())
                    {
                        freetiem2 = DateTime.Now;
                    }
                    else
                    {
                        freetiem2 = freeze[i + 1].IntoTime;
                    }
                    timeList.Add("freezetime", freeze[i].UserName + ":" + freetime);
                    //回温时间
                    var warmtime = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    timeList.Add("warmtime", warmtime == null ? "--" : warmtime.UserName + ":" + warmtime.StartTime.ToString());
                    //搅拌时间
                    var stirtime = db.SMT_Stir.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    JArray stir = new JArray();
                    if (stirtime != null)
                    {
                        stir.Add(stirtime.UserName + ":" + stirtime.StartTime.ToString());
                        stir.Add(stirtime.SecondTime == null ? null : stirtime.SecondName + ":" + stirtime.SecondTime.ToString());
                        stir.Add(stirtime.ThreeTime == null ? null : stirtime.ThreeJobName + ":" + stirtime.ThreeTime.ToString());
                        stir.Add(stirtime.FlorTime == null ? null : stirtime.FlorName + ":" + stirtime.FlorTime.ToString());
                        timeList.Add("stirtime", stir);
                    }
                    else
                    { timeList.Add("stirtime", "--"); }
                    //领用时间
                    var employlist = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).ToList();
                    JArray jarrayemply = new JArray();
                    foreach (var employ in employlist)
                    {
                        JObject jitem = new JObject();
                        jitem.Add("ordernum", employ.OrderNum);
                        jitem.Add("linnum", employ.LineNum);
                        jitem.Add("employtime", employ.UserName + ":" + employ.StartTime.ToString());

                        jarrayemply.Add(jitem);
                    }
                    timeList.Add("employ", jarrayemply);
                    //回收时间
                    var recycle = db.SMT_Recycle.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                    if (i + 1 == freeze.Count())
                    {
                        timeList.Add("recycletime", recycle == null ? "--" : recycle.UserName + ":" + recycle.RecoveTime.ToString());
                    }
                    else
                        timeList.Add("recycletime", "--");
                    //剩余有效期
                    var efftime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => new { c.EffectiveDay, c.LeaveFactoryTime }).FirstOrDefault();
                    var timespan = 0;
                    if (recycle != null)
                    {
                        timespan = efftime.EffectiveDay - (recycle.RecoveTime - efftime.LeaveFactoryTime).Value.Days;
                    }
                    else
                    {
                        timespan = efftime.EffectiveDay - (DateTime.Now - efftime.LeaveFactoryTime).Value.Days;
                    }
                    timeList.Add("effTime", timespan);
                    barcodejobject.Add(i.ToString(), timeList);
                }
                result.Add(barcodejobject);
            }
            return comom.GetModuleFromJarray(result);
        }

        [HttpPost]
        public JObject Boadr([System.Web.Http.FromBody]JObject data)
        {
            string barcode = data["barcode"].ToString();
            string bitch = data["bitch"].ToString();
            string materialNumber = data["materialNumber"].ToString();
            string Supplier = data["Supplier"].ToString();
            string ordernum = data["ordernum"].ToString();

            var freezebarcode = db.SMT_Freezer.Select(c => c.SolderpasterBacrcode).Distinct();
            if (!string.IsNullOrEmpty(barcode))
            {
                freezebarcode = freezebarcode.Where(c => c == barcode);
            }
            if (!string.IsNullOrEmpty(bitch))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.Batch == bitch).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(materialNumber))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == materialNumber).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(Supplier))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.Supplier == Supplier).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(ordernum))
            {
                var barcodeList = db.SMT_Employ.Where(c => c.OrderNum == ordernum).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }

            JArray result = new JArray();

            foreach (var item in freezebarcode)
            {
                JObject barcodejobject = new JObject();

                var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderBy(c => c.IntoTime).ToList();
                for (int i = 0; i < freeze.Count(); i++)
                {
                    //SMT冰柜时间
                    JObject timeList = new JObject();

                    //回收时间,回收超过一天移除
                    var recycle = db.SMT_Recycle.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                    if (recycle != null && (DateTime.Now - recycle.RecoveTime).Value.Days >= 1)
                    {
                        continue;
                    }
                    if (i + 1 == freeze.Count())
                    {
                        timeList.Add("recycletime", recycle == null ? "--" : recycle.UserName + ":" + recycle.RecoveTime.ToString());
                    }
                    else
                        timeList.Add("recycletime", "--");


                    timeList.Add("barcode", item);
                    var freetime = freeze[i].IntoTime;
                    var freetiem2 = new DateTime?();
                    if (i + 1 == freeze.Count())
                    {
                        freetiem2 = DateTime.Now;
                    }
                    else
                    {
                        freetiem2 = freeze[i + 1].IntoTime;
                    }
                    timeList.Add("freezetime", freeze[i].UserName + ":" + freetime);
                    //回温时间
                    var warmtime = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    timeList.Add("warmtime", warmtime == null ? "--" : warmtime.UserName + ":" + warmtime.StartTime.ToString());
                    //搅拌时间
                    var stirtime = db.SMT_Stir.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    JArray stir = new JArray();
                    if (stirtime != null)
                    {
                        stir.Add(stirtime.UserName + ":" + stirtime.StartTime.ToString());
                        stir.Add(stirtime.SecondTime == null ? null : stirtime.SecondName + ":" + stirtime.SecondTime.ToString());
                        stir.Add(stirtime.ThreeTime == null ? null : stirtime.ThreeJobName + ":" + stirtime.ThreeTime.ToString());
                        stir.Add(stirtime.FlorTime == null ? null : stirtime.FlorName + ":" + stirtime.FlorTime.ToString());
                        timeList.Add("stirtime", stir);
                    }
                    else
                    { timeList.Add("stirtime", "--"); }
                    //领用时间
                    var employlist = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).ToList();
                    JArray jarrayemply = new JArray();
                    foreach (var employ in employlist)
                    {
                        JObject jitem = new JObject();
                        jitem.Add("ordernum", employ.OrderNum);
                        jitem.Add("linnum", employ.LineNum);
                        jitem.Add("employtime", employ.UserName + ":" + employ.StartTime.ToString());

                        jarrayemply.Add(jitem);
                    }
                    timeList.Add("employ", jarrayemply);
                    //剩余有效期
                    var efftime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => new { c.EffectiveDay, c.LeaveFactoryTime }).FirstOrDefault();
                    var timespan = 0;
                    if (recycle != null)
                    {
                        timespan = efftime.EffectiveDay - (recycle.RecoveTime - efftime.LeaveFactoryTime).Value.Days;
                    }
                    else
                    {
                        timespan = efftime.EffectiveDay - (DateTime.Now - efftime.LeaveFactoryTime).Value.Days;
                    }
                    timeList.Add("effTime", timespan);

                    barcodejobject.Add(i.ToString(), timeList);

                }
                result.Add(barcodejobject);
            }
            return comom.GetModuleFromJarray(result);
        }

        public JObject HistoryBoadr([System.Web.Http.FromBody]JObject data)
        {
            string barcode = data["barcode"].ToString();
            string bitch = data["bitch"].ToString();
            string materialNumber = data["materialNumber"].ToString();
            string Supplier = data["Supplier"].ToString();
            string ordernum = data["ordernum"].ToString();

            var freezebarcode = db.SMT_Freezer.Select(c => c.SolderpasterBacrcode).Distinct();
            if (!string.IsNullOrEmpty(barcode))
            {
                freezebarcode = freezebarcode.Where(c => c == barcode);
            }
            if (!string.IsNullOrEmpty(bitch))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.Batch == bitch).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(materialNumber))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.MaterialNumber == materialNumber).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(Supplier))
            {
                var barcodeList = db.Barcode_Solderpaste.Where(c => c.Supplier == Supplier).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }
            if (!string.IsNullOrEmpty(ordernum))
            {
                var barcodeList = db.SMT_Employ.Where(c => c.OrderNum == ordernum).Select(c => c.SolderpasterBacrcode).ToList();
                freezebarcode = freezebarcode.Where(c => barcodeList.Contains(c));
            }

            JArray result = new JArray();

            foreach (var item in freezebarcode)
            {
                JObject barcodejobject = new JObject();

                var freeze = db.SMT_Freezer.Where(c => c.SolderpasterBacrcode == item).OrderBy(c => c.IntoTime).ToList();
                for (int i = 0; i < freeze.Count(); i++)
                {
                    //SMT冰柜时间
                    JObject timeList = new JObject();
                    timeList.Add("barcode", item);
                    var freetime = freeze[i].IntoTime;
                    var freetiem2 = new DateTime?();
                    if (i + 1 == freeze.Count())
                    {
                        freetiem2 = DateTime.Now;
                    }
                    else
                    {
                        freetiem2 = freeze[i + 1].IntoTime;
                    }
                    timeList.Add("freezetime", freeze[i].UserName + ":" + freetime);
                    //回温时间
                    var warmtime = db.SMT_Rewarm.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    timeList.Add("warmtime", warmtime == null ? "--" : warmtime.UserName + ":" + warmtime.StartTime.ToString());
                    //搅拌时间
                    var stirtime = db.SMT_Stir.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).FirstOrDefault();
                    JArray stir = new JArray();
                    if (stirtime != null)
                    {
                        stir.Add(stirtime.UserName + ":" + stirtime.StartTime.ToString());
                        stir.Add(stirtime.SecondTime == null ? null : stirtime.SecondName + ":" + stirtime.SecondTime.ToString());
                        stir.Add(stirtime.ThreeTime == null ? null : stirtime.ThreeJobName + ":" + stirtime.ThreeTime.ToString());
                        stir.Add(stirtime.FlorTime == null ? null : stirtime.FlorName + ":" + stirtime.FlorTime.ToString());
                        timeList.Add("stirtime", stir);
                    }
                    else
                    { timeList.Add("stirtime", "--"); }
                    //领用时间
                    var employlist = db.SMT_Employ.Where(c => c.SolderpasterBacrcode == item && c.StartTime < freetiem2 && c.StartTime > freetime).OrderByDescending(c => c.StartTime).ToList();
                    JArray jarrayemply = new JArray();
                    foreach (var employ in employlist)
                    {
                        JObject jitem = new JObject();
                        jitem.Add("ordernum", employ.OrderNum);
                        jitem.Add("linnum", employ.LineNum);
                        jitem.Add("employtime", employ.UserName + ":" + employ.StartTime.ToString());

                        jarrayemply.Add(jitem);
                    }
                    timeList.Add("employ", jarrayemply);
                    //回收时间
                    var recycle = db.SMT_Recycle.Where(c => c.SolderpasterBacrcode == item).FirstOrDefault();
                    if (i + 1 == freeze.Count())
                    {
                        timeList.Add("recycletime", recycle == null ? "--" : recycle.UserName + ":" + recycle.RecoveTime.ToString());
                    }
                    else
                        timeList.Add("recycletime", "--");
                    //剩余有效期
                    var efftime = db.Barcode_Solderpaste.Where(c => c.SolderpasterBacrcode == item).Select(c => new { c.EffectiveDay, c.LeaveFactoryTime }).FirstOrDefault();
                    var timespan = 0;
                    if (recycle != null)
                    {
                        timespan = efftime.EffectiveDay - (recycle.RecoveTime - efftime.LeaveFactoryTime).Value.Days;
                    }
                    else
                    {
                        timespan = efftime.EffectiveDay - (DateTime.Now - efftime.LeaveFactoryTime).Value.Days;
                    }
                    timeList.Add("effTime", timespan);

                    barcodejobject.Add(i.ToString(), timeList);
                }

                result.Add(barcodejobject);
            }
            return comom.GetModuleFromJarray(result);
        }
        #endregion
        
        #region 打印锡膏标签
        [HttpPost]
        public JObject InsideBoxLable_Print(List<string> barcodelist, int pagecount = 1, string ip = "", int port = 0, int concentration = 30, int leftdissmenion = 100)
        {
            //string data = "^XA^MD30~DGR:ZONE.GRF,";
            JArray result = new JArray();

            foreach (var barcode in barcodelist)
            {
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                Bitmap bm = InsideBoxLable_DrawBitmap(barcode);
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO" + leftdissmenion + ",0^XGR:ZONE.GRF^FS^XZ";//FO.X,Y座标
                string message = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                JObject obj = new JObject();
                obj.Add("barcode", barcode);
                obj.Add("message", message);
                result.Add(obj);
            }

            return comom.GetModuleFromJarray(result);
        }
        

        private Bitmap InsideBoxLable_DrawBitmap(string barcode = "")
        {
            //开始绘制图片
            int initialWidth = 400, initialHeight = 150;//宽8高3
            Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(theBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
            //呈现质量
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //背景色
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));


            //引入条码
            Bitmap bmp_barcode = BarCodeLablePrint.BarCodeToImg(barcode, 388, 60);
            theGraphics.DrawImage(bmp_barcode, 5, 25, bmp_barcode.Width, bmp_barcode.Height);

            //引入条码号
            System.Drawing.Font myFont_modulebarcodenum;
            myFont_modulebarcodenum = new System.Drawing.Font("Malgun Gothic", 15, FontStyle.Regular);
            StringFormat geshi1 = new StringFormat();
            geshi1.Alignment = StringAlignment.Center; //居中
            theGraphics.DrawString(barcode, myFont_modulebarcodenum, bush, 200, 80, geshi1);
            //结束图片绘制以上都是绘制图片的代码

            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));//图形转二值
            return bm;
        }

        #endregion
        
    }
}