﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static JianHeMES.Controllers.CommonalityController;
using static JianHeMES.Controllers.CommonERPDB;

namespace JianHeMES.Controllers
{
    public class Warehouse_MaterialController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();

        // GET: Warehouse_Material
        public ActionResult Index()
        {
            return View();
        }

        #region----MC部仓库物料管理
        public class tempWarehous
        {
            public string material { get; set; }
            public string barcode { get; set; }
            public DateTime? time { get; set; }
            public DateTime? intotime { get; set; }
            public int day { get; set; }

        }

        #region ---- 页面
        public ActionResult MaterialBasicInfo()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Warehouse_Material", act = "MaterialBasicInfo" });
            }
            return View();
        }
        public ActionResult QueryData()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Warehouse_Material", act = "QueryData" });
            }
            return View();
        }
        public ActionResult MaterialInput()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Warehouse_Material", act = "MaterialInput" });
            }
            return View();
        }
        public ActionResult Material_Outbound()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Warehouse_Material", act = "Material_Outbound" });
            }
            return View();
        }

        #endregion

        #region ---- 查询物料信息页
        [HttpPost]
        public ActionResult QueryMaterial(string material)
        {
            JObject table = new JObject();
            var querylist = CommonERPDB.ERP_MaterialQuery(material, "").ToList();
            return Content(JsonConvert.SerializeObject(querylist));
        }

        [HttpPost]
        public ActionResult QueryMaterialNumName(string materialNumber, string materialName)
        {
            var resultlist = CommonERPDB.ERP_MaterialQuery(materialNumber, materialName).Select(c => new { c.img01, c.ima02, c.ima021, c.img02, c.img03, c.img08, c.img09, c.img10, c.img18 }).ToList();//根据物料编号到ERP里查找数据
            return Content(JsonConvert.SerializeObject(resultlist));
        }

        #endregion

        #region ----修改库位号
        public ActionResult ModifyLocation(string material, string position)
        {
            JObject table = new JObject();
            var savecount = 0;
            var security = db.Equipment_Safetystock.Where(c => c.Material == material).ToList();//获取数据
            var querylist = CommonERPDB.ERP_Query_SafetyStock(security.Select(c => c.Material).ToList());//根据物料编号到ERP里查找数据
            foreach (var item in querylist)//循环querlist
            {
                if (db.Warehouse_Modify_WarehouseNum.Count(c => c.MaterialNumber == item.img01) == 0)
                {
                    var rede = new Warehouse_Modify_WarehouseNum() { MaterialNumber = material, MaterialBacrcode = null, OldWarehouseNum = item.img03, NewWarehouseNum = position, Modifier = ((Users)Session["user"]).UserName, ModifyTime = DateTime.Now };
                    db.Warehouse_Modify_WarehouseNum.Add(rede);
                    savecount = db.SaveChanges();//保存到数据库
                }
            }
            if (savecount > 0)//判断savecount是否大于0
            {
                table.Add("Position", position);//把库位号add到table里面
                table.Add("Site", true);
            }
            else//等于0 
            {
                table.Add("Site", false);
            }
            return Content(JsonConvert.SerializeObject(table));
        }
        #endregion

        #region ----显示修改库位号清单
        public ActionResult LocationQuery()
        {
            var locationlist = db.Warehouse_Modify_WarehouseNum.Select(c => new { c.ID, c.MaterialNumber, c.MaterialBacrcode, c.OldWarehouseNum, c.NewWarehouseNum }).ToList();
            return Content(JsonConvert.SerializeObject(locationlist));
        }
        #endregion

        #region ----如果ERP里的库位号已经修改过来，就把MES里的记录删除
        public ActionResult DeleteLocation(int id)
        {
            JObject table = new JObject();
            var locationlist = db.Warehouse_Modify_WarehouseNum.Select(c => new { c.MaterialNumber, c.OldWarehouseNum, c.NewWarehouseNum });
            var querylist = CommonERPDB.ERP_Query_SafetyStock(locationlist.Select(c => c.MaterialNumber).ToList());//根据物料编号到ERP里查找数据
            foreach (var item in querylist)
            {
                if (item.img03 == locationlist.Select(c => c.NewWarehouseNum).FirstOrDefault())
                {
                    var record = db.Warehouse_Modify_WarehouseNum.Where(c => c.ID == id).FirstOrDefault();//根据ID表里查询数据
                    UserOperateLog operaterecord = new UserOperateLog();
                    operaterecord.OperateDT = DateTime.Now;//添加删除操作时间
                    operaterecord.Operator = ((Users)Session["User"]).UserName;//添加删除操作人
                    //添加操作记录（如：张三在2020年2月27日删除设备管理邮件抄送人为李四的记录）
                    operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除库位号" + record.Modifier + "的记录。";
                    db.Warehouse_Modify_WarehouseNum.Remove(record);//删除对应的数据
                    db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
                    int count = db.SaveChanges();//保存到数据库
                    if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
                    {
                        table.Add("table", true);
                        table.Add("ware", "删除成功");
                        return Content(JsonConvert.SerializeObject(table));
                    }
                    else //等于0（没有把数据保存到数据库或者保存出错）
                    {
                        table.Add("table", false);
                        table.Add("ware", "删除失败");
                        return Content(JsonConvert.SerializeObject(table));
                    }
                }
                else
                {
                    table.Add("table", false);
                    table.Add("ware", "库位号不一致");
                    return Content(JsonConvert.SerializeObject(table));
                }
            }
            return Content(JsonConvert.SerializeObject(table));
        }
        #endregion

        #region---上传图片/图纸（多张上传）
        //pictureFile多张图片/图纸; pictureType文件类型（图片或者图纸）;materialNumber物料编号
        public bool UploadFile_Warehouse(List<string> pictureFile, string pictureType, string materialNumber)
        {
            var material = materialNumber.Split('-');//将物料号分隔
            if (Request.Files.Count > 0)
            {
                if (Directory.Exists(@"D:\MES_Data\Warehouse\" + material[0] + "\\") == false)//判断总路径是否存在
                {
                    Directory.CreateDirectory(@"D:\MES_Data\Warehouse\" + material[0] + "\\");//创建总路径
                }
                if (pictureType == "Picture")//判断文件类型是图片
                {
                    if (Directory.Exists(@"D:\MES_Data\Warehouse\" + material[0] + "\\" + materialNumber + "\\" + pictureType + "\\") == false)//判断图片路径是否存在
                    {
                        Directory.CreateDirectory(@"D:\MES_Data\Warehouse\" + material[0] + "\\" + materialNumber + "\\" + pictureType + "\\");//创建图片路径
                    }
                    foreach (var item in pictureFile)
                    {
                        HttpPostedFileBase fileBase = Request.Files["UploadFile_Warehouse" + pictureFile.IndexOf(item)];
                        var fileType = fileBase.FileName.Substring(fileBase.FileName.Length - 4, 4).ToLower();
                        List<FileInfo> fileInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\Warehouse\" + material[0] + "\\" + materialNumber + "\\" + pictureType + "\\");//遍历文件夹中的个数
                        if (fileType == ".jpg")
                        {
                            int JPGcount = fileInfo.Where(c => c.Name.StartsWith(materialNumber + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                            fileBase.SaveAs(@"D:\MES_Data\Warehouse\" + material[0] + "\\" + materialNumber + "\\" + pictureType + "\\" + materialNumber + "_" + (JPGcount + 1) + fileType);//文件追加命名JPG
                        }
                        else if (fileType == ".pdf")
                        {
                            int PDFcount = fileInfo.Where(c => c.Name.StartsWith(materialNumber + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").Count();
                            fileBase.SaveAs(@"D:\MES_Data\Warehouse\" + material[0] + "\\" + materialNumber + "\\" + pictureType + "\\" + materialNumber + "_" + (PDFcount + 1) + fileType);//文件追加命名PDF
                        }
                    }
                }
                else //判断文件类型是图纸
                {
                    if (Directory.Exists(@"D:\MES_Data\Warehouse\" + material[0] + "\\" + materialNumber + "\\" + pictureType + "\\") == false)//判断图纸路径是否存在
                    {
                        Directory.CreateDirectory(@"D:\MES_Data\Warehouse\" + material[0] + "\\" + materialNumber + "\\" + pictureType + "\\");//创建图纸路径
                    }
                    foreach (var ite in pictureFile)
                    {
                        HttpPostedFileBase fileBase = Request.Files["UploadFile_Warehouse" + pictureFile.IndexOf(ite)];
                        var fileType = fileBase.FileName.Substring(fileBase.FileName.Length - 4, 4).ToLower();
                        List<FileInfo> fileInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\Warehouse" + material[0] + "\\" + materialNumber + "\\" + pictureType + "\\");//遍历文件个数
                        if (fileType == ".jpg")
                        {
                            int JPGcount = fileInfo.Where(c => c.Name.StartsWith(materialNumber + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                            fileBase.SaveAs(@"D:\MES_Data\Warehouse\" + material[0] + "\\" + materialNumber + "\\" + pictureType + "\\" + materialNumber + "_" + (JPGcount + 1) + fileType);
                        }
                        else if (fileType == ".pdf")
                        {
                            int PDFcount = fileInfo.Where(c => c.Name.StartsWith(materialNumber + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                            fileBase.SaveAs(@"D:\MES_Data\Warehouse\" + material[0] + "\\" + materialNumber + "\\" + pictureType + "\\" + materialNumber + "_" + (PDFcount + 1) + fileType);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        #endregion

        #region---删除图片/图纸
        public ActionResult DeleteImg(string path, string materialNumber, string pictureType)//path(路径)、materialNumber（物料号）、pictureType（文件类型）
        {
            var materialList = materialNumber.Split('-');
            if (!String.IsNullOrEmpty(path))
            {
                string fileType = path.Substring(path.Length - 4, 4);//扩展名
                string OLDpath = @"D:" + path.Replace('/', '\\');//整个文件路径
                FileInfo pathFile = new FileInfo(OLDpath);//建立文件对象
                int warelist = Convert.ToInt16(pathFile.Name.Split('_')[1].Split('.')[0]);//文件序号
                string New_path = @"D:\MES_Data\Warehouse\" + materialList[0] + "\\" + materialNumber + "_DeleteFile\\";//新建目录
                if (Directory.Exists(@"D:\MES_Data\Warehouse\" + materialList[0] + "\\" + materialNumber + "_DeleteFile\\") == false)//目录是否存在
                {
                    Directory.CreateDirectory(@"D:\MES_Data\Warehouse\" + materialList[0] + "\\" + materialNumber + "_DeleteFile\\");
                    FileInfo Newfile = new FileInfo(New_path + materialNumber + "_1" + fileType);
                    pathFile.CopyTo(Newfile.FullName);//复制文件到新目录
                    pathFile.Delete();//删除原文件
                    List<FileInfo> fileInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\Warehouse" + materialList[0] + "\\" + materialNumber + "\\" + pictureType + "\\");
                    int filecount = fileInfo.Where(c => c.Extension == fileType).Count();//文件夹里的个数
                    for (int i = warelist; i < filecount + 1; i++)
                    {
                        string filepath = @"D:\MES_Data\Warehouse\" + materialList[0] + "\\" + materialNumber + "\\" + pictureType + "\\" + materialNumber + "_" + (i + 1) + fileType;
                        string NewFile_path = @"D:\MES_Data\Warehouse\" + materialList[0] + "\\" + materialNumber + "\\" + pictureType + "\\" + materialNumber + "_" + i + fileType;
                        System.IO.File.Move(filepath, NewFile_path);
                    }
                }
                else //已有删除目录时
                {
                    List<FileInfo> fileInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\Warehouse" + materialList[0] + "\\" + materialNumber + "_DeleteFile\\");
                    int count = fileInfo.Where(c => c.Extension == fileType).Count();
                    FileInfo Newfile = new FileInfo(New_path + materialNumber + "_" + (count + 1) + fileType);
                    pathFile.CopyTo(Newfile.FullName);//复制文件到新目录
                    pathFile.Delete();//删除原文件
                    List<FileInfo> pathfileInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\Warehouse" + materialList[0] + "\\" + materialNumber + "\\" + pictureType + "\\");
                    int filecount = pathfileInfo.Where(c => c.Extension == fileType).Count();//文件夹里的个数
                    for (int i = warelist; i < filecount + 1; i++)
                    {
                        string filepath = @"D:\MES_Data\Warehouse\" + materialList[0] + "\\" + materialNumber + "\\" + pictureType + "\\" + materialNumber + "_" + (i + 1) + fileType;
                        string NewFile_path = @"D:\MES_Data\Warehouse\" + materialList[0] + "\\" + materialNumber + "\\" + pictureType + "\\" + materialNumber + "_" + i + fileType;
                        System.IO.File.Move(filepath, NewFile_path);
                    }
                }
                return Content("true");//删除成功
            }
            else
            {
                return Content("false");//没有路径
            }
        }
        #endregion

        #region--仓库物料入库前操作

        //1.收料基本信息录入
        public ActionResult ReceivingInput(List<Warehouse_Receiving_Information> receiving_Information, string group, string receivingPart)
        {
            JObject retul = new JObject();
            if (receiving_Information != null)
            {
                foreach (var item in receiving_Information)
                {
                    if (db.Warehouse_Receiving_Information.Count(c => c.PurchaseOrder == item.PurchaseOrder && c.MaterialNumber == item.MaterialNumber && c.Batch == item.Batch) > 0)
                    {
                        retul.Add("meg", false);
                        retul.Add("repeat", item.PurchaseOrder + "采购合同里的" + item.MaterialNumber + "物料编号" + item.Batch + "批次重复了");
                        return Content(JsonConvert.SerializeObject(retul));
                    }
                }
                foreach (var ite in receiving_Information)
                {
                    ite.ReceivingPart = receivingPart;
                    ite.ReceivingDate = DateTime.Now;
                    ite.Group = group;
                    ite.Department = ((Users)Session["User"]).Department;
                }
                db.Warehouse_Receiving_Information.AddRange(receiving_Information);
                int count = db.SaveChanges();
                if (count > 0)
                {
                    retul.Add("meg", true);
                    retul.Add("repeat", "添加成功" + JsonConvert.SerializeObject(receiving_Information));
                    return Content(JsonConvert.SerializeObject(retul));
                }
                else
                {
                    retul.Add("meg", false);
                    retul.Add("repeat", "添加失败");
                    return Content(JsonConvert.SerializeObject(retul));
                }
            }
            retul.Add("meg", false);
            retul.Add("repeat", "数据为空");
            return Content(JsonConvert.SerializeObject(retul));
        }

        //2.显示采购合同基本信息
        public ActionResult AccordContract(string purchaseOrder)
        {
            JObject table = new JObject();
            var querylist = CommonERPDB.ERP_MaterialQuery(purchaseOrder, "").ToList();

            return View();
        }

        //3.显示送检单基本信息
        public ActionResult AccordInspection(string purchaseOrder)
        {
            JObject table = new JObject();
            var querylist = CommonERPDB.ERP_MaterialQuery(purchaseOrder, "").ToList();

            return View();
        }

        #endregion

        #region---仓库物料出入库

        //1.基本信息录入和生成条码,materialContainNum(物料条码所包含物料个数)
        [HttpPost]
        public ActionResult MaterialBasicInfo(Warehouse_MaterialBasicInfo_Collection materialBasicInfo_Collection, double materialContainNum)
        {
            JObject Table = new JObject();
            if (((Users)Session["User"]) != null)//判断是否登录
            {
                //判断数据、物料编号、批次、物料生产日、考核部门和班组是否为空
                if (materialBasicInfo_Collection != null && materialBasicInfo_Collection.MaterialNumber != null && materialBasicInfo_Collection.Batch != null && materialBasicInfo_Collection.LeaveFactoryTime != null && materialContainNum != 0 && materialBasicInfo_Collection.Department != null && materialBasicInfo_Collection.Group != null)
                {
                    //判断数据库是否有相同的物料号和批次
                    if (db.Warehouse_MaterialBasicInfo_Collection.Count(c => c.MaterialNumber == materialBasicInfo_Collection.MaterialNumber && c.Batch == materialBasicInfo_Collection.Batch) > 0)
                    {
                        Table.Add("Res", false);
                        Table.Add("Meg", "物料号和批次相同！");
                        return Content(JsonConvert.SerializeObject(Table));

                    }
                    materialBasicInfo_Collection.InvoiceDate = DateTime.Now;//来料日期
                    materialBasicInfo_Collection.ReceivingClerk = ((Users)Session["User"]).UserName;//收料人姓名
                    db.Warehouse_MaterialBasicInfo_Collection.Add(materialBasicInfo_Collection);//添加数据

                    #region--- 生成条码
                    if (!String.IsNullOrEmpty(materialBasicInfo_Collection.MaterialNumber) || materialBasicInfo_Collection.LeaveFactoryTime != null)
                    {
                        string year = materialBasicInfo_Collection.InvoiceDate.Value.Year.ToString().Substring(2);//获取物料生产日期的年后两位数
                        JArray barcodeList = new JArray();
                        int count = 0;
                        double bacrcodNum = materialBasicInfo_Collection.MaterialNum / materialContainNum;//物料总数‘除以’物料条码所包含物料个数
                        int bacrcod = (int)bacrcodNum;//把数值转换成int类型
                        for (int i = 1; i <= bacrcod; i++)
                        {
                            int num = 1;

                            //查找物料条码前缀一样的最后一个序列号
                            var materBacrcode = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialNumber == materialBasicInfo_Collection.MaterialNumber).OrderByDescending(c => c.MaterialBacrcode).FirstOrDefault(); // 找到相同物料编号,相同生产日期符合条件的最后一个物料条码
                            if (materBacrcode != null)
                            {
                                string materNum = materBacrcode.MaterialBacrcode.Substring(materBacrcode.MaterialBacrcode.Length - 5, 5);//找到条码的最后序列号
                                num = int.Parse(materNum) + 1;//序列号加一
                            }

                            //分割物料编号
                            var arr = materialBasicInfo_Collection.MaterialNumber.Split('-');
                            string materialNumber = arr[0] + arr[1];

                            //组成条码 物料编号+生产时间的年月日+序列编号
                            string barcode = materialNumber + "-" + year + materialBasicInfo_Collection.InvoiceDate.Value.Month.ToString().PadLeft(2, '0') + materialBasicInfo_Collection.InvoiceDate.Value.Day.ToString().PadLeft(2, '0') + "-" + num.ToString().PadLeft(5, '0');
                            //新建条码的数据信息
                            Warehouse_Material_Bacrcode material_Bacrcode = new Warehouse_Material_Bacrcode() { MaterialBacrcode = barcode, MaterialNumber = materialBasicInfo_Collection.MaterialNumber, MaterialContainNum = materialContainNum, PrintTime = DateTime.Now, PrintName = ((Users)Session["User"]).UserName };//新建条码的数据信息
                            db.Warehouse_Material_Bacrcode.Add(material_Bacrcode);
                            count = db.SaveChanges();
                            barcodeList.Add(JsonConvert.SerializeObject(material_Bacrcode));
                        }
                        if (count > 0)
                        {
                            Table.Add("Res", true);
                            Table.Add("Meg", "保存成功！");
                            Table.Add("BarcodeList", barcodeList);
                            Table.Add("bacrcodNum", bacrcodNum.ToString("0.00"));
                            Table.Add("MaterialDiscription", materialBasicInfo_Collection.MaterialDiscription);
                            return Content(JsonConvert.SerializeObject(Table));
                        }
                    }
                    #endregion
                }
                Table.Add("Res", false);
                Table.Add("Meg", "物料编号/批次/物料生产日期/考核部门和班组其中有为空");//判断数据、物料编号、批次、物料生产日、考核部门和班组是否为空
                return Content(JsonConvert.SerializeObject(Table));
            }
            Table.Add("Res", false);
            Table.Add("Meg", "没有登录！");
            return Content(JsonConvert.SerializeObject(Table));
        }

        //2.根据物料编号显示数据；materialNumber（物料编号）
        [HttpPost]
        public ActionResult QueryData(string materialNumber)
        {
            JObject Table = new JObject();
            var info = db.Warehouse_MaterialBasicInfo_Collection.Where(c => c.MaterialNumber == materialNumber).FirstOrDefault();
            var bac = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialNumber == info.MaterialNumber).Select(c => c.MaterialBacrcode).ToList();
            string newWarehouse = null;
            string newWarehouseNum = null;
            foreach (var item in bac)
            {
                var kuweuh = db.Warehouse_putIn.Where(c => c.CheckTime != null && c.MaterialBacrcode == item).Select(c => c.MaterialBacrcode).FirstOrDefault();
                if (kuweuh != null)
                {
                    var code = db.Warehouse_putIn.Where(c => c.MaterialBacrcode == kuweuh).Select(c => new { c.NewWarehouse, c.NewWarehouseNum }).FirstOrDefault();
                    newWarehouseNum = code.NewWarehouseNum;
                    newWarehouse = code.NewWarehouse;
                }
            }
            var querylist = CommonERPDB.ERP_MaterialQuery(materialNumber, "").FirstOrDefault();
            //ID
            Table.Add("ID", info.ID == 0 ? 0 : info.ID);
            //物料编号
            Table.Add("MaterialNumber", info == null ? null : info.MaterialNumber);
            //物料规格描述
            Table.Add("MaterialDiscription", info == null ? null : info.MaterialDiscription);
            //批次
            Table.Add("Batch", info == null ? null : info.Batch);
            //物料数量（收货数量MES）
            Table.Add("MaterialNum", info.MaterialNum == 0 ? 0 : info.MaterialNum);
            //库存量ERP
            Table.Add("img10", querylist == null ? null : querylist.img10);
            //有效期
            Table.Add("EffectiveDay", info.EffectiveDay == 0 ? 0 : info.EffectiveDay);
            //追加有效期
            Table.Add("AddEffectiveDay", info.AddEffectiveDay == 0 ? 0 : info.AddEffectiveDay);
            //物料品名
            Table.Add("ima02", querylist == null ? null : querylist.ima02);
            //MES仓库编号
            Table.Add("newWarehouse", newWarehouse == null ? null : newWarehouse);
            //MES储位（库位号）
            Table.Add("newWarehouseNum", newWarehouseNum == null ? null : newWarehouseNum);
            //ERP仓库编号
            Table.Add("Img02", querylist == null ? null : querylist.img02);
            //ERP储位（库位号）
            Table.Add("Img03", querylist == null ? null : querylist.img03);
            //物料单位
            Table.Add("Unit", info == null ? null : info.Unit);
            //供应商
            Table.Add("Supplier", info == null ? null : info.Supplier);
            //物料类型
            Table.Add("MaterialType", info == null ? null : info.MaterialType);
            //生产日期
            Table.Add("LeaveFactoryTime", info == null ? null : Convert.ToDateTime(info.LeaveFactoryTime).ToString("yyyy-MM-dd"));
            return Content(JsonConvert.SerializeObject(Table));
        }

        //3.物料入库；warehouse_putIn(条码列表)，newWarehouse（MES仓库编号），newWarehouseNum（MES库位号），group（班组）
        [HttpPost]
        public ActionResult MaterialInput(List<string> warehouse_putIn, string newWarehouse, string newWarehouseNum, string group)
        {
            JObject Ware = new JObject();
            if (((Users)Session["User"]) != null)//判断是否登录
            {
                int count = 0;
                foreach (var item in warehouse_putIn)
                {
                    Warehouse_putIn bacrcode = new Warehouse_putIn();
                    bacrcode.MaterialBacrcode = item;
                    bacrcode.NewWarehouse = newWarehouse;
                    bacrcode.NewWarehouseNum = newWarehouseNum;
                    bacrcode.CheckTime = DateTime.Now;
                    bacrcode.Username = ((Users)Session["User"]).UserName;
                    bacrcode.JobNum = ((Users)Session["User"]).UserName.ToString();
                    bacrcode.Group = group;
                    bacrcode.Department = ((Users)Session["User"]).Department;
                    db.Warehouse_putIn.Add(bacrcode);
                    count += db.SaveChanges();
                }
                if (count == warehouse_putIn.Count())
                {
                    Ware.Add("Res", true);
                    Ware.Add("Meg", warehouse_putIn.Count() + "条码入库成功");
                    return Content(JsonConvert.SerializeObject(Ware));
                }
                else
                {
                    Ware.Add("Res", false);
                    Ware.Add("Meg", "入库失败");
                    return Content(JsonConvert.SerializeObject(Ware));
                }
            }
            Ware.Add("Res", false);
            Ware.Add("Meg", "没有登录！");
            return Content(JsonConvert.SerializeObject(Ware));
        }

        //4.修改物料基本信息
        public ActionResult Modify_material(Warehouse_MaterialBasicInfo_Collection warehouse_Material)
        {
            JObject tally = new JObject();
            if (warehouse_Material != null && warehouse_Material.MaterialNumber != null)
            {
                db.Entry(warehouse_Material).State = EntityState.Modified;//修改数据
                var count = db.SaveChanges();
                if (count > 0)//判断result是否大于0（有没有把数据保存到数据库）
                {
                    tally.Add("tally", true);
                    tally.Add("warehouse_Material", JsonConvert.SerializeObject(warehouse_Material));
                    return Content(JsonConvert.SerializeObject(tally));
                }
                else //result等于0（没有把数据保存到数据库或者保存出错）
                {
                    tally.Add("tally", false);
                    tally.Add("warehouse_Material", null);
                    return Content(JsonConvert.SerializeObject(tally));
                }
            }
            tally.Add("tally", false);
            tally.Add("warehouse_Material", "数据有误");
            return Content(JsonConvert.SerializeObject(tally));
        }

        //5.物料出库：material_Outbound（条码列表），materiaTyper（出库类型如：工单出库，超领出库），name（领料人姓名），jobnum（领料人工号），department（领料部门），group（班组）
        [HttpPost]
        public ActionResult Material_Outbound(List<string> material_Outbound, string materiaTyper, string name, string jobnum, string department, string group)
        {
            JObject Table = new JObject();
            if (((Users)Session["User"]) != null)
            {
                int count = 0;
                int barcodeList = material_Outbound.Count();//拿到本次物料出库数量
                var warehous = db.Warehouse_Material_Outbound.Where(c => c.OutboundDate == null).ToList();//找没有出库的物料信息
                var tempwarehouse = new List<tempWarehous>();
                var tempwarehouse2 = new List<string>();
                foreach (var item in warehous)
                {
                    var data = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == item.MaterialBacrcode).FirstOrDefault();//找当前条码的条码信息
                                                                                                                                       //根据物料号和批次找到生产日期，有效器和追加有效期
                    var bate = db.Warehouse_MaterialBasicInfo_Collection.Where(c => c.MaterialNumber == item.MaterialNumber && c.Batch == item.Batch).Select(c => new { c.LeaveFactoryTime, c.EffectiveDay, c.AddEffectiveDay }).FirstOrDefault();
                    //根据当前条码号找条码入库时间
                    var infodate = db.Warehouse_putIn.Where(c => c.MaterialBacrcode == item.MaterialBacrcode).Select(c => c.CheckTime).FirstOrDefault();
                    int effective = bate.EffectiveDay + bate.AddEffectiveDay;
                    tempWarehous temp = new tempWarehous() { barcode = item.MaterialBacrcode, time = bate.LeaveFactoryTime, day = effective, intotime = infodate };//将条码,生产时间,有效期,入库时间保存起来

                    var factory = (int)(DateTime.Now - bate.LeaveFactoryTime).Value.TotalDays;//查看有效期是否为负数
                    var effctiveDay = effective - factory;
                    if (effctiveDay < 0)//有效期为负数的
                    {
                        tempwarehouse2.Add(item.MaterialBacrcode);
                    }
                    else//有效期为正数的
                    {
                        tempwarehouse.Add(temp);
                    }
                }
                var codeList = tempwarehouse.OrderBy(c => c.intotime).Select(c => c.barcode).ToList();//拿到去除有效期为负数的，根据入库时间排序的列表
                tempwarehouse2.AddRange(codeList);//将有效期为负数的放在最前面
                foreach (var ite in material_Outbound)//循环前端传过来的条码列表
                {
                    Warehouse_Material_Outbound outbounds = new Warehouse_Material_Outbound();
                    //根据条码号找MES库位号
                    var haouse = db.Warehouse_putIn.Where(c => c.MaterialBacrcode == ite).Select(c => c.NewWarehouseNum).FirstOrDefault();
                    //根据条码号找物料编号，条码数量
                    var contain = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == ite).Select(c => new { c.MaterialNumber, c.MaterialContainNum }).FirstOrDefault();
                    var arr = ite.Split('-')[1];//分割获取批次
                    outbounds.MaterialBacrcode = ite;//条码
                    outbounds.MaterialNumber = contain.MaterialNumber;//物料编号
                    outbounds.Batch = arr;//批次
                    outbounds.Material_OutNum = contain.MaterialContainNum;//条码数量（列如：1:10）
                    outbounds.NewWarehouseNum = haouse;//MES库位号
                    outbounds.MateriaOutboundTyper = materiaTyper;//物料出库类型（如：工单出库、超领出库等）
                    outbounds.Username = ((Users)Session["User"]).UserName;
                    outbounds.JobNum = ((Users)Session["User"]).UserNum.ToString();
                    outbounds.OutboundDate = DateTime.Now;
                    outbounds.OutUsername = name;
                    outbounds.OutJobNum = jobnum;
                    outbounds.OutDepartment = department;
                    outbounds.Department = ((Users)Session["User"]).Department;
                    outbounds.Group = group;
                    db.Warehouse_Material_Outbound.Add(outbounds);
                    count += db.SaveChanges();
                }
                if (count == material_Outbound.Count())//判断保存是否正确
                {
                    var materialList = tempwarehouse2.Take(barcodeList);
                    var notbarcode = materialList.Except(material_Outbound).ToList();
                    if (notbarcode.Count != 0)
                    {
                        string barcode = string.Join(",", notbarcode.ToArray());
                        UserOperateLog operateLog = new UserOperateLog() { OperateDT = DateTime.Now, Operator = ((Users)Session["User"]).UserName, OperateRecord = "物料条码" + barcode + "没有在本次的出库列表中，该条码应该是要出库的" };
                        db.UserOperateLog.Add(operateLog);
                        db.SaveChanges();
                    }
                    Table.Add("Res", true);
                    Table.Add("Meg", "出库成功");
                    return Content(JsonConvert.SerializeObject(Table));
                }
                else
                {
                    Table.Add("Res", false);
                    Table.Add("Meg", "出库失败");
                    return Content(JsonConvert.SerializeObject(Table));
                }
            }
            Table.Add("Res", false);
            Table.Add("Meg", "没有登录！");
            return Content(JsonConvert.SerializeObject(Table));
        }

        //6.物料出库列表显示提示先入先出
        public ActionResult MaterialsFirstIn(string materialBacrcode)
        {
            var mater = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == materialBacrcode).Select(c => c.MaterialNumber).FirstOrDefault();//根据条码找对应的物料编号
            var outbound = db.Warehouse_Material_Outbound.Where(c => c.OutboundDate != null && c.MaterialNumber == mater).Select(c => c.MaterialBacrcode).ToList();//根据物料编号和出库时间不为空找对应的条码清单
            var bacrcodeList = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialNumber == mater).Select(c => c.MaterialBacrcode).ToList();//根据物料编号找对应的条码清单
            var Inputbound = db.Warehouse_putIn.Where(c => c.CheckTime != null).Select(c => c.MaterialBacrcode).ToList();//把已入库的条码清单找出来
            var bacrcode = bacrcodeList.Intersect(Inputbound).ToList();//找到条码清单和入库条码清单里一样的条码清单（交集）
            var cha = bacrcode.Except(outbound).ToList();//找到没有出库的条码清单（差集）
            var tempwarehouse = new List<tempWarehous>();
            foreach (var item in cha)
            {
                var code = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == item).Select(c => c.MaterialNumber).FirstOrDefault();
                var batch = item.Split('-')[1];//分割获取批次
                //根据物料号和批次找生产日期和有效期
                var dataList = db.Warehouse_MaterialBasicInfo_Collection.Where(c => c.MaterialNumber == code && c.Batch == batch).FirstOrDefault();
                int effective = dataList.EffectiveDay + dataList.AddEffectiveDay;//有效期加追加有效期
                                                                                 //根据当前条码号找条码入库时间
                var infodate = db.Warehouse_putIn.Where(c => c.MaterialBacrcode == item).Select(c => c.CheckTime).FirstOrDefault();
                tempWarehous temp = new tempWarehous() { material = code, barcode = item, time = dataList.LeaveFactoryTime, day = effective, intotime = infodate };
                tempwarehouse.Add(temp);
            }
            tempwarehouse.OrderBy(c => c.intotime).ToList();//根据入库时间进行排序
            JArray table = new JArray();
            foreach (var ite in tempwarehouse)
            {
                JObject result = new JObject();
                //物料编号
                result.Add("MaterialNumber", ite.material);
                //条码
                result.Add("MaterialBacrcode", ite.barcode);
                //生产日期
                result.Add("LeaveFactoryTime", Convert.ToDateTime(ite.time).ToString("yyyy-MM-dd"));
                //有效天数
                var spem = (int)(DateTime.Now - ite.time).Value.TotalDays;
                var effectiveday = ite.day - spem;
                result.Add("EffectiveDay", effectiveday);
                if (effectiveday < 0)//有效期为负数,放在第一位
                {
                    table.AddFirst(result);
                }
                else
                {
                    table.Add(result);
                }
            }
            return Content(JsonConvert.SerializeObject(table));
        }

        //7.根据物料号和生产时间显示已出库和未出库、未入库的条码号
        public ActionResult BarcodeNotOut(string materialNumber, DateTime leaveFactoryTime)
        {
            JObject table = new JObject();
            JArray retul = new JArray();
            var date = db.Warehouse_MaterialBasicInfo_Collection.Where(c => c.MaterialNumber == materialNumber && c.LeaveFactoryTime == leaveFactoryTime).FirstOrDefault();
            var bacrcodeList = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialNumber == date.MaterialNumber).Select(c =>c.MaterialBacrcode).ToList();           
            var putinbacrcode = db.Warehouse_putIn.Select(c => c.MaterialBacrcode).Distinct().ToList();
            var code = bacrcodeList.Except(putinbacrcode).ToList();//未入库的条码清单
            var ecx = bacrcodeList.Intersect(putinbacrcode).ToList();
            var codeList = db.Warehouse_Material_Outbound.Select(c => c.MaterialBacrcode).Distinct().ToList();//查找物料出库表里的所有条码清单
            var except = ecx.Except(codeList).ToList();//拿到没有出库的条码清单
            var differe = ecx.Intersect(codeList).ToList();//拿到已出库的条码清单
            foreach(var item in code) //未入库的条码清单           
            {
                var codeNum = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == item).Select(c => c.MaterialContainNum).FirstOrDefault();
                table.Add("meg", "未入库");
                table.Add("MaterialBacrcode", item);
                table.Add("MaterialContainNum", codeNum);
                retul.Add(table);
                table = new JObject();
            }
            foreach(var ite in except)//拿到没有出库的条码清单
            {
                var codeNum = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == ite).Select(c => c.MaterialContainNum).FirstOrDefault();
                table.Add("meg", "已入库未出库");
                table.Add("MaterialBacrcode", ite);
                table.Add("MaterialContainNum", codeNum);
                retul.Add(table);
                table = new JObject();
            }
            foreach(var it in differe)//拿到已出库的条码清单
            {
                var codeNum = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == it).Select(c => c.MaterialContainNum).FirstOrDefault();
                table.Add("meg", "已出库");
                table.Add("MaterialBacrcode", it);
                table.Add("MaterialContainNum", codeNum);
                retul.Add(table);
                table = new JObject();
            }
            return Content(JsonConvert.SerializeObject(retul));
        }

        //8.修改标签条码所包含的个数
        public ActionResult ModifyBarcodeNum(string materialBacrcode, double materialContainNum)
        {
            JObject table = new JObject();
            if (materialBacrcode != null)
            {
                int count = 0;
                var barcode = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == materialBacrcode).ToList();
                foreach (var item in barcode)
                {
                    item.MaterialContainNum = materialContainNum;
                    count = db.SaveChanges();
                }
                if (count > 0)
                {
                    table.Add("Meg", true);
                    table.Add("Contain", materialContainNum);
                    return Content(JsonConvert.SerializeObject(table));
                }
                else
                {
                    table.Add("Meg", false);
                    table.Add("Contain", "修改失败");
                    return Content(JsonConvert.SerializeObject(table));
                }
            }
            table.Add("Meg", false);
            table.Add("Contain", "条码号为空");
            return Content(JsonConvert.SerializeObject(table));
        }

        //9.验证条码是否存在，或者已入库/出库
        public ActionResult ValidationBacrcode(string materialBacrcode, string type)
        {
            JObject table = new JObject();
            var bacrcode = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == materialBacrcode).FirstOrDefault();//根据条码去条码表找相对应的条码
            if (bacrcode != null)
            {
                if (materialBacrcode != null && type == "入库")
                {
                    var bacr = db.Warehouse_putIn.Where(c => c.MaterialBacrcode == materialBacrcode).FirstOrDefault();//根据条码去入库表找相对应的条码
                    if (bacr != null)
                    {
                        table.Add("res", false);
                        table.Add("meg", materialBacrcode + "此条码已生成，已入库");
                        return Content(JsonConvert.SerializeObject(table));
                    }
                    else
                    {
                        table.Add("res", true);
                        table.Add("meg", materialBacrcode + "此条码已生成，未入库");
                        return Content(JsonConvert.SerializeObject(table));
                    }
                }
                else if (materialBacrcode != null && type == "出库")
                {
                    var outbacrcode = db.Warehouse_Material_Outbound.Where(c => c.MaterialBacrcode == materialBacrcode).FirstOrDefault();//根据条码去出库找相对应的条码
                    var bacr = db.Warehouse_putIn.Where(c => c.MaterialBacrcode == materialBacrcode).FirstOrDefault();//根据条码去入库表找相对应的条码
                    if (bacr == null)
                    {
                        table.Add("res", false);
                        table.Add("meg", materialBacrcode + "此条码已生成,未入库");
                        return Content(JsonConvert.SerializeObject(table));
                    }
                    else if (outbacrcode != null)
                    {
                        table.Add("res", false);
                        table.Add("meg", materialBacrcode + "此条码已生成，已出库");
                        return Content(JsonConvert.SerializeObject(table));
                    }
                    else
                    {
                        table.Add("res", true);
                        table.Add("meg", materialBacrcode + "此条码已生成已入库，未出库");
                        return Content(JsonConvert.SerializeObject(table));
                    }
                }
            }
            table.Add("res", false);
            table.Add("meg", materialBacrcode + "此条码未生成");
            return Content(JsonConvert.SerializeObject(table));
        }

        #endregion

        #endregion

        #region--打印标签方法

        /// <summary>
        /// </summary>
        /// <param name="material_list">数组包括条码号、描述</param>
        /// <param name="pagecount">打印数量</param>
        /// <param name="ip">打印地址</param>
        /// <param name="port">打印端口</param>
        /// <param name="concentration">打印浓度</param>
        /// <param name="testswitch">输出图片或者打印</param>
        /// <returns></returns>
        public class MC_BarcodePrinting
        {
            public string MaterialNumber { get; set; }

            public string MaterialDiscription { get; set; }

        }
        //查看图片方法
        public ActionResult MaterialLableImages(string materialBarcode, string materialDescription)
        {
            //组织数据
            var num = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == materialBarcode).Select(c => c.MaterialContainNum);
            var AllBitmap = CreateImages(materialBarcode, materialDescription, num.ToString());
            MemoryStream ms = new MemoryStream();
            AllBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            AllBitmap.Dispose();
            return File(ms.ToArray(), "image/Png");

        }
        //打印图片方法
        public ActionResult MaterialLablePrint(List<MC_BarcodePrinting> material_list, int pagecount = 1, string ip = "", int port = 0, int concentration = 5)
        {
            int printcount = 0;
            foreach (var item in material_list)
            {
                var list = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == item.MaterialNumber).Select(c => c.MaterialContainNum).FirstOrDefault();
                var bm = CreateImages(item.MaterialNumber, item.MaterialDiscription, list.ToString());
                string data = "^XA^MD" + concentration + "~DGR:ZONE.GRF,";
                int totalbytes = bm.ToString().Length;
                int rowbytes = 10;
                string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
                data += totalbytes + "," + rowbytes + "," + hex;
                data += "^LH0,0^FO130,0^XGR:ZONE.GRF^FS^XZ";
                string result = ZebraUnity.IPPrint(data.ToString(), pagecount, ip, port);
                if (result == "打印成功！")
                {
                    printcount++;
                }
            }
            string res = (printcount > 0 ? printcount + "个打印成功!" : "") + ((material_list.Count - printcount) > 0 ? (material_list.Count - printcount) + "个打印失败！" : "");
            return Content(res);
        }
        //生成图片方法
        public Bitmap CreateImages(string materialNumber, string materialDiscription, string num)
        {
            int initialWidth = 550, initialHeight = 250;
            Bitmap AllBitmap = new Bitmap(initialWidth, initialHeight);
            Graphics theGraphics = Graphics.FromImage(AllBitmap);
            Brush bush = new SolidBrush(System.Drawing.Color.Black);
            theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
            Pen pen = new Pen(Color.Black, 3);//定义笔的大小

            //引入条码号
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center; //居中
            System.Drawing.Font myFont_materialNumber;
            myFont_materialNumber = new System.Drawing.Font("Microsoft YaHei UI", 18, FontStyle.Regular);
            theGraphics.DrawString(materialNumber, myFont_materialNumber, bush, 230, 16, format);
            //引入条形码
            Bitmap spc_materialnumberBarcode = BarCodeLablePrint.BarCodeToImg(materialNumber, 400, 32);
            theGraphics.DrawImage(spc_materialnumberBarcode, 30, 50, (float)(spc_materialnumberBarcode.Width), (float)(spc_materialnumberBarcode.Height));
            //引入数量
            theGraphics.DrawString("1:" + num, myFont_materialNumber, bush, 430, 16, format);


            //引入描述
            StringFormat format1 = new StringFormat();
            format1.Alignment = StringAlignment.Near; //居中
            if (materialDiscription.Length > 0 && materialDiscription.Length < 100)
            {

                Rectangle mc_materialDiscription = new Rectangle(35, 85, 500, 85);
                myFont_materialNumber = new System.Drawing.Font("Microsoft YaHei UI", 16, FontStyle.Regular);
                theGraphics.DrawString(materialDiscription, myFont_materialNumber, bush, mc_materialDiscription, format1);
            }
            else
            {
                Rectangle mc_materialDiscription = new Rectangle(60, 85, 455, 150);
                myFont_materialNumber = new System.Drawing.Font("Microsoft YaHei UI", 10, FontStyle.Regular);
                theGraphics.DrawString(materialDiscription, myFont_materialNumber, bush, mc_materialDiscription, format1);
            }

            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(AllBitmap)));//图形转二值
            return bm;
        }
        #endregion


        #region ---其他方法
        // GET: Warehouse_Material/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse_Material_BaseInfo warehouse_Material_BaseInfo = db.Warehouse_Material_BaseInfo.Find(id);
            if (warehouse_Material_BaseInfo == null)
            {
                return HttpNotFound();
            }
            return View(warehouse_Material_BaseInfo);
        }

        // GET: Warehouse_Material/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Warehouse_Material/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ManufactorNum,ManufactorName,MaterialNumber,ProductName,Specifications,Type,VarietyType")] Warehouse_Material_BaseInfo warehouse_Material_BaseInfo)
        {
            if (ModelState.IsValid)
            {
                db.Warehouse_Material_BaseInfo.Add(warehouse_Material_BaseInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(warehouse_Material_BaseInfo);
        }

        // GET: Warehouse_Material/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse_Material_BaseInfo warehouse_Material_BaseInfo = db.Warehouse_Material_BaseInfo.Find(id);
            if (warehouse_Material_BaseInfo == null)
            {
                return HttpNotFound();
            }
            return View(warehouse_Material_BaseInfo);
        }

        // POST: Warehouse_Material/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ManufactorNum,ManufactorName,MaterialNumber,ProductName,Specifications,Type,VarietyType")] Warehouse_Material_BaseInfo warehouse_Material_BaseInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(warehouse_Material_BaseInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(warehouse_Material_BaseInfo);
        }

        // GET: Warehouse_Material/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse_Material_BaseInfo warehouse_Material_BaseInfo = db.Warehouse_Material_BaseInfo.Find(id);
            if (warehouse_Material_BaseInfo == null)
            {
                return HttpNotFound();
            }
            return View(warehouse_Material_BaseInfo);
        }

        // POST: Warehouse_Material/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Warehouse_Material_BaseInfo warehouse_Material_BaseInfo = db.Warehouse_Material_BaseInfo.Find(id);
            db.Warehouse_Material_BaseInfo.Remove(warehouse_Material_BaseInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion


        #region------库存金额查询

        public ActionResult StockAmountCalculate()
        {
            ViewBag.financerecordcount = 0;
            ViewBag.mcunissuerecordcount = 0;
            ViewBag.queryfinancedatatimespan = null;
            ViewBag.querymc_unissuedatatimespan = null;
            return View();
        }

        [HttpPost]
        public ActionResult StockAmountCalculate(int year, int month, bool outputexcelfile = false)
        {
            //检查ERP
            var connectERPresult = CommonERPDB.TryConnectERP();
            if (connectERPresult != "连接正常")
            {
                JArray result = new JArray();//输出总结果
                JObject testResult = new JObject();
                testResult.Add("类别", "查询失败");
                testResult.Add("分类明细", connectERPresult);
                result.Add(testResult);
                return Content(JsonConvert.SerializeObject(result));
                //return Content("连接ERP数据库服务器失败！请检查网络或者ERP数据库服务器是否已启动。");//{"ORA-12541: TNS: 无监听程序"}
            }

            #region 成品部分
            //按年月查出仓库编号是LCWC1，且库位编号是CWC01的记录
            var outsiderecord = CommonERPDB.OutsideWarehouseQuery(year, month);
            //开始财务上月库存结存明细表查询时间
            DateTime queryfinancedatabegin = DateTime.Now;
            //财务上月库存结存明细表查询   单价tc_cxc05
            var financedata = CommonERPDB.ERP_FinanceDetialsQuery(year,month);
            //var financedata = financealldata;


            ////计算财务上月库存结存明细表查询时长
            //ViewBag.queryfinancedatatimespan = DateTime.Now - queryfinancedatabegin;
            ////var dt = DataTableTool.ToDataTable(financedata); //财务上月库存结存明细表转为DataTable类型
            //ViewBag.financerecordcount = financedata.Count;//统计财务上月库存结存明细表记录条数
            //取出财务上月库存结存明细表物料号清单
            //List<string> tc_cxc01_list = financedata.Select(c => c.tc_cxc01).Distinct().ToList();  
            //成品金额全部合计
            decimal total = (decimal)financedata.Sum(c => c.tc_cxc06);

            //在制品记录
            var work_in_process = CommonERPDB.ERP_Work_in_process(year, month);

            //输出成品全部明细记录
            if (outputexcelfile == true)
            {
                string[] columns = { "料号", "品名", "规格", "面积", "本月结存数", "本月结存单价", "本月结存金额", "30天", "90天", "180天", "365天", "2年", "3年", "3-5年", "5年以上", "会计科目", "库位号" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(financedata, "ERP导出财务月底库存结算表" + DateTime.Now.ToString("D") + "）", false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, "库存结算表（" + DateTime.Now.ToString("D") + "）.xlsx");
            }

            JArray results = new JArray();//输出总结果

            Dictionary<string, string> compare_dic = new Dictionary<string, string> {
                 { "-K-RD", "1.公司常规备库单、2.投标备库单" },
                 { "-J", "3.业务借样单" },
                 { "-Q", "4.工厂品质单" },
                 { "-RM", "5.公司展厅单、6.各大区展厅单" },
                 { "-R", "7.研发样品单" },
                 { "-C", "8.公司参展单" },
                 { "P-", "9.品质客诉单" },
                 { "B-", "10.出货后增加物料单" },
                 { "-S", "11.费用挂大区单及赠送单" },
                 { "-H", "12.易事达订单" },
                 { "-Y/-G/-A/-LA", "13.销售订单" },
                 { "Others", "14.其他订单"},
            };


            #region 财务科目类
            //科目清单
            var kemu_list = financedata.Select(c => c.AccountingSubject).Distinct().OrderBy(c => c).ToList();
            JObject resultrecord = new JObject();//单行记录
            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(140301, "原材料-基本材料");
            dict.Add(140302, "原材料-辅助材料");
            dict.Add(140501, "成品");
            dict.Add(140502, "半成品");
            foreach (var item in kemu_list)
            {
                resultrecord.Add("类别", "财务科目类");
                resultrecord.Add("分类明细", dict[item]);// item==140301? "原材料-基本材料": item == 140302 ? "原材料-辅助材料" : item == 140501? "成品" : item == 140502 ? "半成品":""
                var market1month = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
                resultrecord.Add("1个月以内", market1month.ToString("n4"));
                var market2_3month = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
                resultrecord.Add("2-3个月", market2_3month.ToString("n4"));
                var market4_6month = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
                resultrecord.Add("4-6个月", market4_6month.ToString("n4"));
                var market7_12month = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
                resultrecord.Add("7-12个月", market7_12month.ToString("n4"));
                var market1_2year = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
                resultrecord.Add("1-2年", market1_2year.ToString("n4"));
                var market2_3year = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
                resultrecord.Add("2-3年", market2_3year.ToString("n4"));
                var marketr3_5year = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
                resultrecord.Add("3-5年", marketr3_5year.ToString("n4"));
                var marketover5year = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
                resultrecord.Add("5年以上", marketover5year.ToString("n4"));
                var markettotal = market1month + market2_3month + market4_6month + market7_12month + market1_2year + market2_3year + marketr3_5year + marketover5year;
                resultrecord.Add("合计金额", markettotal.ToString("n4"));
                resultrecord.Add("记录条数", financedata.Count(c => c.AccountingSubject == item));
                resultrecord.Add("备注", item);
                results.Add(resultrecord);
                resultrecord = new JObject();
            }

            //增加在制品项
            resultrecord.Add("类别", "");
            resultrecord.Add("分类明细", "在制品类");
            var workinprocess1month_work = work_in_process.Where(c => c.durations == "1个月以内").Sum(c => c.ccg92);
            resultrecord.Add("1个月以内", workinprocess1month_work.ToString("n4"));
            var workinprocess2_3month_work = work_in_process.Where(c => c.durations == "2-3个月").Sum(c => c.ccg92);
            resultrecord.Add("2-3个月", workinprocess2_3month_work.ToString("n4"));
            var workinprocess4_6month_work = work_in_process.Where(c => c.durations == "4-6个月").Sum(c => c.ccg92);
            resultrecord.Add("4-6个月", workinprocess4_6month_work.ToString("n4"));
            var workinprocess7_12month_work = work_in_process.Where(c => c.durations == "7-12个月").Sum(c => c.ccg92);
            resultrecord.Add("7-12个月", workinprocess7_12month_work.ToString("n4"));
            var workinprocess1_2year_work = work_in_process.Where(c => c.durations == "1-2年").Sum(c => c.ccg92);
            resultrecord.Add("1-2年", workinprocess1_2year_work.ToString("n4"));
            var workinprocess2_3year_work = work_in_process.Where(c => c.durations == "2-3年").Sum(c => c.ccg92);
            resultrecord.Add("2-3年", workinprocess2_3year_work.ToString("n4"));
            var workinprocess3_5year_work = work_in_process.Where(c => c.durations == "3-5年").Sum(c => c.ccg92);
            resultrecord.Add("3-5年", workinprocess3_5year_work.ToString("n4"));
            var workinprocessover5year_work = work_in_process.Where(c => c.durations == "5年以上").Sum(c => c.ccg92);
            resultrecord.Add("5年以上", workinprocessover5year_work.ToString("n4"));
            var workinprocesstotal_work = workinprocess1month_work + workinprocess2_3month_work + workinprocess4_6month_work + workinprocess7_12month_work + workinprocess1_2year_work + workinprocess2_3year_work + workinprocess3_5year_work + workinprocessover5year_work;
            resultrecord.Add("合计金额", workinprocesstotal_work.ToString("n4"));
            resultrecord.Add("记录条数", work_in_process.Count());
            results.Add(resultrecord);
            resultrecord = new JObject();


            resultrecord.Add("5年以上", "全部合计");
            resultrecord.Add("合计金额", (Convert.ToDouble(total) + workinprocesstotal_work).ToString("n4"));
            resultrecord.Add("记录条数", financedata.Count() + work_in_process.Count());
            results.Add(resultrecord);
            resultrecord = new JObject();


            //空白行
            resultrecord.Add("类别", "");
            resultrecord.Add("分类明细", "---");
            results.Add(resultrecord);
            resultrecord = new JObject();

            #endregion


            #region 成品类


            #region 优先厂外仓
            //第一优先级应该是厂外仓，然后是备库和样品，再然后是配件订单，剩下的就是销售订单了和其他的
            var financefinish140501 = financedata.Where(c => c.AccountingSubject == 140501).ToList();
            double financefinish140501_sum = financefinish140501.Sum(c => c.tc_cxc06);
            int financefinish140501_count = financefinish140501.Count();
            //成品 4.厂外仓，财务发出的厂外仓数据为依据  //财务380条，程序208条
            var mn_list = outsiderecord.Select(c => c.imk01).ToList();
            var finished_product_warehouse = financefinish140501.Where(c => mn_list.Contains(c.tc_cxc01)).ToList();
            resultrecord.Add("类别", "成品类");
            resultrecord.Add("分类明细", "厂外仓");
            var market1month4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
            resultrecord.Add("1个月以内", market1month4.ToString("n4"));
            var market2_3month4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
            resultrecord.Add("2-3个月", market2_3month4.ToString("n4"));
            var market4_6month4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
            resultrecord.Add("4-6个月", market4_6month4.ToString("n4"));
            var market7_12month4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
            resultrecord.Add("7-12个月", market7_12month4.ToString("n4"));
            var market1_2year4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
            resultrecord.Add("1-2年", market1_2year4.ToString("n4"));
            var market2_3year4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
            resultrecord.Add("2-3年", market2_3year4.ToString("n4"));
            var market3_5year4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
            resultrecord.Add("3-5年", market3_5year4.ToString("n4"));
            var marketover5year4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
            resultrecord.Add("5年以上", marketover5year4.ToString("n4"));
            var markettotal4 = market1month4 + market2_3month4 + market4_6month4 + market7_12month4 + market1_2year4 + market2_3year4 + market3_5year4 + marketover5year4;
            resultrecord.Add("合计金额", markettotal4.ToString("n4"));
            resultrecord.Add("记录条数", finished_product_warehouse.Count);
            financefinish140501 = financefinish140501.Except(finished_product_warehouse).ToList(); //排除厂外仓记录
            results.Add(resultrecord);
            resultrecord = new JObject();
            #endregion

            #region 夹具模具
            //第二级，夹具模具
            //成品 夹具模具
            var financefinish_clamp = financefinish140501.Where(c => c.tc_cxc01.StartsWith("2")).ToList();
            resultrecord.Add("类别", "成品类");
            resultrecord.Add("分类明细", "夹具模具");
            var market1month2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
            resultrecord.Add("1个月以内", market1month2.ToString("n4"));
            var market2_3month2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
            resultrecord.Add("2-3个月", market2_3month2.ToString("n4"));
            var market4_6month2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
            resultrecord.Add("4-6个月", market4_6month2.ToString("n4"));
            var market7_12month2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
            resultrecord.Add("7-12个月", market7_12month2.ToString("n4"));
            var market1_2year2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
            resultrecord.Add("1-2年", market1_2year2.ToString("n4"));
            var market2_3year2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
            resultrecord.Add("2-3年", market2_3year2.ToString("n4"));
            var market3_5year2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
            resultrecord.Add("3-5年", market3_5year2.ToString("n4"));
            var marketover5year2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
            resultrecord.Add("5年以上", marketover5year2.ToString("n4"));
            var markettotal2 = market1month2 + market2_3month2 + market4_6month2 + market7_12month2 + market1_2year2 + market2_3year2 + market3_5year2 + marketover5year2;
            resultrecord.Add("合计金额", markettotal2.ToString("n4"));
            resultrecord.Add("记录条数", financefinish_clamp.Count);
            financefinish140501 = financefinish140501.Except(financefinish_clamp).ToList(); //排除夹具模组记录
            results.Add(resultrecord);
            resultrecord = new JObject();
            #endregion

            #region 单项计算
            foreach (var it in compare_dic.Keys.ToArray())
            {
                var temp = financefinish140501.Where(c => c.Classification == compare_dic[it]).ToList();
                resultrecord.Add("类别", "成品类");
                resultrecord.Add("分类明细", compare_dic[it] + "(" + it + ")");
                var market1month = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
                resultrecord.Add("1个月以内", market1month.ToString("n4"));
                var market2_3month = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
                resultrecord.Add("2-3个月", market2_3month.ToString("n4"));
                var market4_6month = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
                resultrecord.Add("4-6个月", market4_6month.ToString("n4"));
                var market7_12month = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
                resultrecord.Add("7-12个月", market7_12month.ToString("n4"));
                var market1_2year = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
                resultrecord.Add("1-2年", market1_2year.ToString("n4"));
                var market2_3year = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
                resultrecord.Add("2-3年", market2_3year.ToString("n4"));
                var market3_5year = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
                resultrecord.Add("3-5年", market3_5year.ToString("n4"));
                var marketover5year = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
                resultrecord.Add("5年以上", marketover5year.ToString("n4"));
                var markettotal = market1month + market2_3month + market4_6month + market7_12month + market1_2year + market2_3year + market3_5year + marketover5year;
                resultrecord.Add("合计金额", markettotal.ToString("n4"));
                resultrecord.Add("记录条数", temp.Count);
                financefinish140501 = financefinish140501.Except(temp).ToList();
                results.Add(resultrecord);
                resultrecord = new JObject();
            }
            #endregion

            resultrecord.Add("5年以上", "全部合计");
            resultrecord.Add("合计金额", financefinish140501_sum.ToString("n4"));
            resultrecord.Add("记录条数", financefinish140501_count);
            results.Add(resultrecord);
            resultrecord = new JObject();
            #endregion

            //空白行
            resultrecord.Add("类别", "");
            resultrecord.Add("分类明细", "---");
            results.Add(resultrecord);
            resultrecord = new JObject();

            #endregion

            #region 在制品部分

            #region 单项计算
            foreach (var it in compare_dic.Keys.ToArray())
            {
                var work_in_process_include = work_in_process.Where(c => c.Classification == compare_dic[it]).ToList();
                resultrecord.Add("类别", "在制品类");
                resultrecord.Add("分类明细", compare_dic[it] + "(" + it + ")");
                var workinprocess1month = work_in_process_include.Where(c => c.durations == "1个月以内").Sum(c => c.ccg92);
                resultrecord.Add("1个月以内", workinprocess1month.ToString("n4"));
                var workinprocess2_3month = work_in_process_include.Where(c => c.durations == "2-3个月").Sum(c => c.ccg92);
                resultrecord.Add("2-3个月", workinprocess2_3month.ToString("n4"));
                var workinprocess4_6month = work_in_process_include.Where(c => c.durations == "4-6个月").Sum(c => c.ccg92);
                resultrecord.Add("4-6个月", workinprocess4_6month.ToString("n4"));
                var workinprocess7_12month = work_in_process_include.Where(c => c.durations == "7-12个月").Sum(c => c.ccg92);
                resultrecord.Add("7-12个月", workinprocess7_12month.ToString("n4"));
                var workinprocess1_2year = work_in_process_include.Where(c => c.durations == "1-2年").Sum(c => c.ccg92);
                resultrecord.Add("1-2年", workinprocess1_2year.ToString("n4"));
                var workinprocess2_3year = work_in_process_include.Where(c => c.durations == "2-3年").Sum(c => c.ccg92);
                resultrecord.Add("2-3年", workinprocess2_3year.ToString("n4"));
                var workinprocess3_5year = work_in_process_include.Where(c => c.durations == "3-5年").Sum(c => c.ccg92);
                resultrecord.Add("3-5年", workinprocess3_5year.ToString("n4"));
                var workinprocessover5year = work_in_process_include.Where(c => c.durations == "5年以上").Sum(c => c.ccg92);
                resultrecord.Add("5年以上", workinprocessover5year.ToString("n4"));
                var workinprocesstotal = workinprocess1month + workinprocess2_3month + workinprocess4_6month + workinprocess7_12month + workinprocess1_2year + workinprocess2_3year + workinprocess3_5year + workinprocessover5year;
                resultrecord.Add("合计金额", workinprocesstotal.ToString("n4"));
                resultrecord.Add("记录条数", work_in_process_include.Count());
                results.Add(resultrecord);
                resultrecord = new JObject();
            }
            #endregion

            resultrecord.Add("类别", "");
            resultrecord.Add("分类明细", "");
            resultrecord.Add("合计金额", work_in_process.Sum(c => c.ccg92).ToString("n4"));
            resultrecord.Add("记录条数", work_in_process.Count());
            results.Add(resultrecord);
            resultrecord = new JObject();

            //空白行
            resultrecord.Add("类别", "");
            resultrecord.Add("分类明细", "---");
            results.Add(resultrecord);
            resultrecord = new JObject();
            #endregion

            #region----修改后MC未发料、原材料类
            //MC未发料表(修改后方案)
            var raw_material_nuissus = CommonERPDB.ERP_MC_NuIssueDetialsQuery2(year,month);
            List<cxcr006_raw_material> result7 = new List<cxcr006_raw_material>();
            //取到原材料表记录
            var financedata_Long2 = CXCR_ConvertType(financedata.Where(c => c.AccountingSubject != 140501).ToList());
            result7 = Calculate_raw(financedata_Long2, raw_material_nuissus);


            //MC表发料记录表(原材料)
            resultrecord.Add("类别", "MC表发料记录表(专用记录表)");
            resultrecord.Add("分类明细", "");
            results.Add(resultrecord);
            resultrecord = new JObject();

            compare_dic.Add("NoOrder", "无订单需求");

            #region 单项计算
            foreach (var it in compare_dic.Keys.ToArray())
            {
                var financedata_raw_material_result = result7.Where(c => c.Classification == compare_dic[it]).ToList();
                resultrecord.Add("类别", "原材料类");
                resultrecord.Add("分类明细", compare_dic[it] + "(" + it + ")");
                var raw_material1month = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
                resultrecord.Add("1个月以内", raw_material1month.ToString("n4"));
                var raw_material2_3month = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
                resultrecord.Add("2-3个月", raw_material2_3month.ToString("n4"));
                var raw_material4_6month = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
                resultrecord.Add("4-6个月", raw_material4_6month.ToString("n4"));
                var raw_material7_12month = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
                resultrecord.Add("7-12个月", raw_material7_12month.ToString("n4"));
                var raw_material1_2year = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
                resultrecord.Add("1-2年", raw_material1_2year.ToString("n4"));
                var raw_material2_3year = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
                resultrecord.Add("2-3年", raw_material2_3year.ToString("n4"));
                var raw_material3_5year = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
                resultrecord.Add("3-5年", raw_material3_5year.ToString("n4"));
                var raw_materialover5year = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
                resultrecord.Add("5年以上", raw_materialover5year.ToString("n4"));
                var raw_material_sum = raw_material1month + raw_material2_3month + raw_material4_6month + raw_material7_12month + raw_material1_2year + raw_material2_3year + raw_material3_5year + raw_materialover5year;
                resultrecord.Add("合计金额", raw_material_sum.ToString("n4"));
                resultrecord.Add("记录条数", financedata_raw_material_result.Count);
                results.Add(resultrecord);
                resultrecord = new JObject();
            }
            #endregion

            resultrecord.Add("5年以上", "汇总结果求和");
            resultrecord.Add("合计金额", result7.Where(c => c.Classification != "原始记录").Sum(c => c.tc_cxc06).ToString("n4"));
            resultrecord.Add("记录条数", result7.Where(c => c.Classification != "原始记录").Count());
            results.Add(resultrecord);
            resultrecord = new JObject();

            #endregion

            return Content(JsonConvert.SerializeObject(results));
        }


        [HttpPost]
        public ActionResult StockAmountCalculateGetExcel(string outputexcelfile, int year, int month)
        {
            //cxcr006 成品类表头
            string[] columns = { "料号", "品名", "规格", "面积", "本月结存数", "本月结存单价", "本月结存金额", "30天", "90天", "180天", "365天", "2年", "3年", "3-5年", "5年以上","年度","月份", "会计科目", "库位号", "分类明细" };
            //cxcr006_raw_material 原材料类表头
            string[] columns_raw_material = { "料号", "品名", "规格", "面积", "本月结存数", "本月结存单价", "本月结存金额", "30天", "90天", "180天", "365天", "2年", "3年", "3-5年", "5年以上", "会计科目", "库位号", "分类明细", "工单号", "工单备注", "工单录入日期", "单位", "应发数量", "已发数量", "未发数量", "年度", "月份" };
            //在制品类表头
            string[] columns_work_in_process = { "工单号", "料号", "期间", "工单年月份", "年度", "月份", "上月结存数量", "上月结存金额", "物料品名", "其他分群码四", "物料规格", "是否为重覆性生产料件 (Y/N)", "重工否(Y/N)", "备注", "分类明细" };
            //csfr008 MC表发料记录表表头
            string[] columns_raw_material_MC = { "工单号", "工单备注", "工单录入日期", "生产数量", "入库数量", "料号", "品名", "规格", "单位", "应发数量", "已发数量", "未发数量", "年度", "月份", "订单类型" };

            byte[] filecontent = null;
            string re = "";
            //财务整个月底结存表记录
            List<cxcr006> financealldata = CommonERPDB.ERP_FinanceDetialsQuery(year,month);
            //筛选出科目140501的记录
            var financedata = financealldata.Where(c => c.AccountingSubject == 140501).ToList();
            //财务上月库存结存明细表排除140501科目以外的记录：原材料-基本材料，原材料-辅助材料，半成品
            //var financedata_raw_material = financealldata.Where(c => c.AccountingSubject != 140501).ToList();
            //MC发料表
            // var raw_material = CommonERPDB.ERP_MC_NuIssueDetialsQuery(inputdate, enddate).Where(c => c.sfa05_sfa06 > 0).ToList();

            //MC未发料表(修改后方案)
            var raw_material_nuissus = CommonERPDB.ERP_MC_NuIssueDetialsQuery2(year,month);

            //第一优先级应该是厂外仓，然后是备库和样品，再然后是配件订单，剩下的就是销售订单了和其他的
            switch (outputexcelfile)
            {
                #region 财务分科
                case "财务科目类":
                    filecontent = ExcelExportHelper.ExportExcel(financealldata.ToList(), outputexcelfile + "(" + DateTime.Now.ToString("D") + "）", false, columns);
                    break;
                #endregion

                #region 成品类         
                case "成品类":
                    //按年月查出仓库编号是LCWC1，且库位编号是CWC01的记录
                    var outsiderecord = CommonERPDB.OutsideWarehouseQuery(year, month);
                    //取出厂外仓的所有物料号
                    var mn_list = outsiderecord.Select(c => c.imk01).ToList();
                    //根据厂外仓物料号统计厂外仓信息
                    var outsideWS = financedata.Where(c => mn_list.Contains(c.tc_cxc01)).ToList();
                    outsideWS.ForEach(c => { c.Classification = "厂外仓"; });
                    var clampWS = financedata.Where(c => c.tc_cxc01.StartsWith("2")).ToList();
                    clampWS.ForEach(c => { c.Classification = "夹具模具"; });
                    filecontent = ExcelExportHelper.ExportExcel(financedata.ToList(), outputexcelfile + "(" + DateTime.Now.ToString("D") + "）", false, columns);
                    break;
                #endregion

                #region 在制品类
                case "在制品类":
                    var result5 = CommonERPDB.ERP_Work_in_process(year, month);
                    filecontent = ExcelExportHelper.ExportExcel(result5, outputexcelfile + "(" + DateTime.Now.ToString("D") + "）", false, columns_work_in_process);
                    break;
                #endregion

                #region----修改后MC未发料方案
                case "MC表发料记录表(专用记录表)":
                    filecontent = ExcelExportHelper.ExportExcel(raw_material_nuissus, outputexcelfile + "(" + DateTime.Now.ToString("D") + "）", false, columns_raw_material_MC);
                    break;

                #region case "原材料类":
                case "原材料类":
                    List<cxcr006_raw_material> result7 = new List<cxcr006_raw_material>();
                    //取到原材料表记录
                    var financedata_Long2 = CXCR_ConvertType(financealldata.Where(c => c.AccountingSubject != 140501).ToList());
                    result7 = Calculate_raw(financedata_Long2, raw_material_nuissus);
                    filecontent = ExcelExportHelper.ExportExcel(result7, outputexcelfile + "(" + DateTime.Now.ToString("D") + "）", false, columns_raw_material);
                    break;
                #endregion
                #endregion

                default:
                    re = "整个财务结存表";
                    filecontent = ExcelExportHelper.ExportExcel(financealldata, re + "(" + DateTime.Now.ToString("D") + "）", false, columns);
                    break;
            }
            return File(filecontent, ExcelExportHelper.ExcelContentType, outputexcelfile + "(" + DateTime.Now.ToString("D") + "）.xlsx");
        }

        //计算并返回库存金额记录
        public static List<cxcr006_raw_material> Calculate_raw(List<cxcr006_raw_material> financedata, List<tc_cxd_file> raw_material_nuissus)
        {
            List<cxcr006_raw_material> result = new List<cxcr006_raw_material>();

            //计算拆分
            #region 拆分计算
            foreach (var item in financedata)
            {
                cxcr006_raw_material record0 = TransExpV2<cxcr006_raw_material, cxcr006_raw_material>.Trans(item);
                //保留原始记录
                record0.Classification = "原始记录";
                result.Add(record0);

                cxcr006_raw_material record = TransExpV2<cxcr006_raw_material, cxcr006_raw_material>.Trans(item);

                #region 按物料编号汇总后再按订单类别分类汇总扣减
                //取出物料号对应的未发料记录
                var raw_material_Number_list = raw_material_nuissus.Where(c => c.tc_cxd06 == item.tc_cxc01).ToList();
                if (raw_material_Number_list.Count > 0)
                {
                    //把排除物料号对应的未发料记录
                    raw_material_nuissus.Except(raw_material_Number_list);
                    Dictionary<string, string> compare_dic = new Dictionary<string, string> {
                        { "-K-RD", "1.公司常规备库单、2.投标备库单" },
                        { "-J", "3.业务借样单" },
                        { "-Q", "4.工厂品质单" },
                        { "-RM", "5.公司展厅单、6.各大区展厅单" },
                        { "-R", "7.研发样品单" },
                        { "-C", "8.公司参展单" },
                        { "P-", "9.品质客诉单" },
                        { "B-", "10.出货后增加物料单" },
                        { "-S", "11.费用挂大区单及赠送单" },
                        { "-H", "12.易事达订单" },
                        { "-Y/-G/-A/-LA", "13.销售订单" },
                        { "Others", "14.其他订单"} };
                    Dictionary<string, List<tc_cxd_file>> raw_material_Number_list_Classify = new Dictionary<string, List<tc_cxd_file>>();
                    foreach (var it in compare_dic.Keys.ToArray())
                    {
                        raw_material_Number_list_Classify.Add(it, raw_material_Number_list.Where(c => c.ordertype == compare_dic[it]).ToList());
                        raw_material_Number_list.Except(raw_material_Number_list_Classify[it]);
                    }
                    #endregion

                    foreach (var raw in raw_material_Number_list_Classify.Keys.ToArray())
                    {
                        List<tc_cxd_file> list = raw_material_Number_list_Classify[raw];
                        //此物料的应发料总数
                        var shuld_issue_sum = list.Sum(c => c.tc_cxd10);
                        //此物料的已发料总数
                        var issued_sum = list.Sum(c => c.tc_cxd11);
                        //此物料的未发料总数
                        var nuissue_sum = list.Sum(c => c.tc_cxd10_tc_cxd11);
                        Dictionary<string, double> issuedata = new Dictionary<string, double> { { "shuld_issue_sum", shuld_issue_sum }, { "issued_sum", issued_sum }, { "nuissue_sum", nuissue_sum } };
                        if (list.Count > 0 && nuissue_sum > 0)
                        {
                            var add_record = Abatement(record, issuedata, compare_dic[raw], out record);
                            result.Add(add_record);
                        }
                    }
                    if (record.tc_cxc04 > 0)
                    {
                        record.Classification = "无订单需求";
                        result.Add(record);
                    }
                }
                else
                {
                    record.Classification = "无订单需求";
                    result.Add(record);
                }
            }
            result = result.Where(c => c.tc_cxc04 > 0).ToList();
            #endregion
            return result;
        }

        public static class TransExpV2<TIn, TOut>
        {

            private static readonly Func<TIn, TOut> cache = GetFunc();
            private static Func<TIn, TOut> GetFunc()
            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
                List<MemberBinding> memberBindingList = new List<MemberBinding>();

                foreach (var item in typeof(TOut).GetProperties())
                {
                    if (!item.CanWrite)
                        continue;

                    MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                    MemberBinding memberBinding = Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }

                MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
                Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

                return lambda.Compile();
            }

            public static TOut Trans(TIn tIn)
            {
                return cache(tIn);
            }

        }

        //库存金额扣减记录
        public static cxcr006_raw_material Abatement(cxcr006_raw_material return_record, Dictionary<string, double> issuedata, string classify, out cxcr006_raw_material record)
        {
            cxcr006_raw_material record0 = TransExpV2<cxcr006_raw_material, cxcr006_raw_material>.Trans(return_record);

            return_record.sfa05 = issuedata["shuld_issue_sum"];
            return_record.sfa06 = issuedata["issued_sum"];
            return_record.sfa05_sfa06 = issuedata["nuissue_sum"];
            double price = return_record.tc_cxc06 / return_record.tc_cxc04;//计算单价
            //原始值
            Dictionary<int, double> orginal_dictionary = new Dictionary<int, double> { { 0, return_record.tc_cxc04 }, { 1, return_record.tc_cxc07 }, { 2, return_record.tc_cxc08 }, { 3, return_record.tc_cxc09 }, { 4, return_record.tc_cxc10 }, { 5, return_record.tc_cxc11 }, { 6, return_record.tc_cxc12 }, { 7, return_record.tc_cxc13 }, { 8, return_record.tc_cxc14 } };
            Dictionary<int, double> record_dic = new Dictionary<int, double>();//输出值，减完未发料后剩余的值
            Dictionary<int, double> return_dic = new Dictionary<int, double>();//返回值，需要要减的未发料对象值

            if (issuedata["nuissue_sum"] > 0)
            {
                var sum = issuedata["nuissue_sum"] > orginal_dictionary[0] ? orginal_dictionary[0] : issuedata["nuissue_sum"];//求和>结存?结存：求和
                return_dic.Add(0, sum);//返回值，需要要减的未发料对象值总和
                record_dic.Add(0, orginal_dictionary[0] - sum);//输出值，减完未发料后剩余的值总数
                for (int i = 8; i > 0; i--)
                {
                    if (orginal_dictionary[i] == 0)
                    {
                        return_dic.Add(i, 0);
                        record_dic.Add(i, 0);
                    }
                    else if (sum >= orginal_dictionary[i])
                    {
                        return_dic.Add(i, orginal_dictionary[i]);
                        record_dic.Add(i, 0);
                        sum = sum - orginal_dictionary[i];
                    }
                    else
                    {
                        return_dic.Add(i, sum);
                        record_dic.Add(i, orginal_dictionary[i] - sum);
                        sum = 0;
                    }
                }

                //修改返回对象值
                return_record.tc_cxc04 = return_dic[0];
                return_record.tc_cxc06 = return_dic[0] * price;
                return_record.tc_cxc07 = return_dic[1];
                return_record.tc_cxc08 = return_dic[2];
                return_record.tc_cxc09 = return_dic[3];
                return_record.tc_cxc10 = return_dic[4];
                return_record.tc_cxc11 = return_dic[5];
                return_record.tc_cxc12 = return_dic[6];
                return_record.tc_cxc13 = return_dic[7];
                return_record.tc_cxc14 = return_dic[8];
                return_record.Classification = classify;

                //修改输出对象值
                record0.tc_cxc04 = record_dic[0];
                record0.tc_cxc06 = record_dic[0] * price;
                record0.tc_cxc07 = record_dic[1];
                record0.tc_cxc08 = record_dic[2];
                record0.tc_cxc09 = record_dic[3];
                record0.tc_cxc10 = record_dic[4];
                record0.tc_cxc11 = record_dic[5];
                record0.tc_cxc12 = record_dic[6];
                record0.tc_cxc13 = record_dic[7];
                record0.tc_cxc14 = record_dic[8];
                //record0.Classification = classify;
            }
            else
            {
                foreach (var item in orginal_dictionary)
                {
                    record_dic.Add(item.Key, item.Value);
                }
            }

            record = record0;
            return return_record;
        }

        static List<cxcr006_raw_material> CXCR_ConvertType(List<cxcr006> inputlist)
        {
            List<cxcr006_raw_material> result_list = new List<cxcr006_raw_material>();
            foreach (var item in inputlist)
            {
                cxcr006_raw_material record = new cxcr006_raw_material();
                record.tc_cxc01 = item.tc_cxc01;
                record.tc_cxc02 = item.tc_cxc02;
                record.tc_cxc03 = item.tc_cxc03;
                record.area = item.area;
                record.tc_cxc04 = item.tc_cxc04;
                record.tc_cxc05 = item.tc_cxc05;
                record.tc_cxc06 = item.tc_cxc06;
                record.tc_cxc07 = item.tc_cxc07;
                record.tc_cxc08 = item.tc_cxc08;
                record.tc_cxc09 = item.tc_cxc09;
                record.tc_cxc10 = item.tc_cxc10;
                record.tc_cxc11 = item.tc_cxc11;
                record.tc_cxc12 = item.tc_cxc12;
                record.tc_cxc13 = item.tc_cxc13;
                record.tc_cxc14 = item.tc_cxc14;
                record.tc_cxd15 = item.tc_cxc15;
                record.tc_cxd16 = item.tc_cxc16;
                record.AccountingSubject = item.AccountingSubject;
                record.WarehouseNumber = item.WarehouseNumber;
                result_list.Add(record);
            }
            return result_list;
        }
        static cxcr006_raw_material CXCR_ConvertTypeSingal(cxcr006_raw_material inputlist)
        {
            cxcr006_raw_material record = new cxcr006_raw_material();
            record.tc_cxc01 = inputlist.tc_cxc01;
            record.tc_cxc02 = inputlist.tc_cxc02;
            record.tc_cxc03 = inputlist.tc_cxc03;
            record.area = inputlist.area;
            record.tc_cxc04 = inputlist.tc_cxc04;
            record.tc_cxc05 = inputlist.tc_cxc05;
            record.tc_cxc06 = inputlist.tc_cxc06;
            record.tc_cxc07 = inputlist.tc_cxc07;
            record.tc_cxc08 = inputlist.tc_cxc08;
            record.tc_cxc09 = inputlist.tc_cxc09;
            record.tc_cxc10 = inputlist.tc_cxc10;
            record.tc_cxc11 = inputlist.tc_cxc11;
            record.tc_cxc12 = inputlist.tc_cxc12;
            record.tc_cxc13 = inputlist.tc_cxc13;
            record.tc_cxc14 = inputlist.tc_cxc14;
            record.AccountingSubject = inputlist.AccountingSubject;
            record.WarehouseNumber = inputlist.WarehouseNumber;
            return record;
        }


        #endregion

        #region---导出excel
        [HttpPost]
        public FileContentResult ExportExcel(string tableData,int year, int month)
        {
            DataTable table = new DataTable();
            var array = JsonConvert.DeserializeObject(tableData) as JArray;
            if (array.Count > 0)
            {
                StringBuilder columns = new StringBuilder();
                JObject objColumns = array[0] as JObject;
                //构造表头
                foreach (JToken jkon in objColumns.AsEnumerable<JToken>())
                {
                    string name = ((JProperty)(jkon)).Name;
                    columns.Append(name + ",");
                    table.Columns.Add(name);
                }
                //向表中添加数据
                for (int i = 0; i < array.Count; i++)
                {
                    DataRow row = table.NewRow();
                    JObject obj = array[i] as JObject;
                    foreach (JToken jkon in obj.AsEnumerable<JToken>())
                    {

                        string name = ((JProperty)(jkon)).Name;
                        string value = ((JProperty)(jkon)).Value.ToString();
                        row[name] = value;
                    }
                    table.Rows.Add(row);
                }
            }
            byte[] filecontent = ExcelExportHelper.ExportExcel(table, "库存金额表（统计时间:" + year + "年" + month + "月", false);
            return File(filecontent, ExcelExportHelper.ExcelContentType, "金额表" + ".xlsx");
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



    }
}
