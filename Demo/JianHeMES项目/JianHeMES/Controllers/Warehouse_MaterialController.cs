﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using JianHeMES.AuthAttributes;
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
                return RedirectToAction("Login2", "Users", new { col = "Warehouse_Material", act = "MaterialBasicInfo" });
            }
            return View();
        }
        public ActionResult QueryData()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Warehouse_Material", act = "QueryData" });
            }
            return View();
        }
        public ActionResult MaterialInput()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Warehouse_Material", act = "MaterialInput" });
            }
            return View();
        }
        public ActionResult Material_Outbound()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Warehouse_Material", act = "Material_Outbound" });
            }
            return View();
        }
        public ActionResult QueryPrint()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Warehouse_Material", act = "QueryPrint" });
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

        #region--仓库物料

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

        //2.根据采购合同单号显示物料基本信息
        public ActionResult AccordContract(string purchaseNumber)
        {
            JObject table = new JObject();
            JArray retul = new JArray();
            JArray deco = new JArray();
            JObject pmn = new JObject();
            if (!String.IsNullOrEmpty(purchaseNumber))
            {
                var querylist = CommonERPDB.ERP_ContractQuery("", purchaseNumber).ToList();
                var code = querylist.Select(c => c.rvb05).Distinct().ToList();
                if (code.Count != 0)
                {
                    retul.Add(code);
                    pmn.Add("retul", retul);
                    foreach (var item in code)
                    {
                        var it = querylist.Where(c => c.rvb05 == item).FirstOrDefault();
                        if (it != null)
                        {
                            //物料编号
                            table.Add("rvb05", it.rvb05 == null ? null : it.rvb05);
                            //物料类型
                            table.Add("type", it.MaterialType == null ? null : it.MaterialType);
                            //单位
                            table.Add("pmn07", it.pmn07 == null ? null : it.pmn07);
                            //供应商
                            table.Add("rva05", it.rva05 == null ? null : it.rva05);
                            //来料数量（应）
                            table.Add("pmn20", it.pmn20 == 0 ? 0 : it.pmn20);
                            //物料规格描述
                            table.Add("specification", it.specification == null ? null : it.specification);
                            deco.Add(table);
                            table = new JObject();
                        }
                    }
                    pmn.Add("deco", deco);
                }
            }
            return Content(JsonConvert.SerializeObject(pmn));
        }

        //3.根据物料编号显示物料基本信息
        public ActionResult AccordInspection(string materialNumber)
        {
            JObject retul = new JObject();
            JArray table = new JArray();
            JArray code = new JArray();
            var materialList = CommonERPDB.ERP_MaterialQuery(materialNumber, "").Distinct().ToList();
            var mat = materialList.Select(c => c.img01).Distinct().ToList();
            foreach (var item in mat)
            {
                var pat = materialList.Where(c => c.img01 == materialNumber).Distinct().FirstOrDefault();
                var supplier = materialList.Where(c => c.img01 == item && c.rva05 != "").Select(c => c.rva05).Distinct().ToList();
                code.Add(supplier);
                //物料编号
                retul.Add("img01", pat.img01 == null ? null : pat.img01);
                //物料类型
                retul.Add("type", pat.MaterialType == null ? null : pat.MaterialType);
                //单位
                retul.Add("img09", pat.img09 == null ? null : pat.img09);
                //供应商
                retul.Add("rva05", supplier.Count == 0 ? null : code);
                //来料数量（应）
                retul.Add("img08", pat.img08 == 0 ? 0 : pat.img08);
                //物料规格描述
                retul.Add("ima021", pat.ima021 == null ? null : pat.ima021);
                table.Add(retul);
                retul = new JObject();

            }
            return Content(JsonConvert.SerializeObject(table));
        }

        //4.采购合同信息数据和送货单信息数据对比(物料编号，物料规格，采购合同号)
        public ActionResult Compare_TheData(string materialNumber, string materialDiscription, string purchaseNumber)
        {
            JObject table = new JObject();
            if (!String.IsNullOrEmpty(materialNumber) && !String.IsNullOrEmpty(materialDiscription) && !String.IsNullOrEmpty(purchaseNumber))
            {
                var querylist = CommonERPDB.ERP_ContractQuery(purchaseNumber, "").ToList();
                foreach (var item in querylist)
                {
                    if (item.rvb04 == purchaseNumber)//判断采购合同单号是否一致
                    {
                        if (item.rvb05 == materialNumber)//判断物料编号是否一致
                        {
                            if (item.specification == materialDiscription)//判断物料规格是否相同
                            {
                                table.Add("meg", true);
                                table.Add("fag", "物料编号,物料规格，采购合同号这三者都一致！");
                                return Content(JsonConvert.SerializeObject(table));
                            }
                            else //物料规格不相同
                            {
                                table.Add("meg", false);
                                table.Add("fag", "物料编号和采购合同号都一致！但物料规格不一致");
                                return Content(JsonConvert.SerializeObject(table));
                            }
                        }
                        else //物料编号不一致
                        {
                            table.Add("meg", false);
                            table.Add("fag", "采购合同号一致!但物料编号不一致");
                            return Content(JsonConvert.SerializeObject(table));
                        }
                    }
                    else //采购合同单号不一致
                    {
                        table.Add("meg", false);
                        table.Add("fag", "采购合同不一致！");
                        return Content(JsonConvert.SerializeObject(table));
                    }
                }
            }
            table.Add("meg", false);
            table.Add("fag", "数据错误：物料编号/物料规格/采购合同号有一个为空！");
            return Content(JsonConvert.SerializeObject(table));
        }

        //5.显示收料送检单(没有写完)
        public ActionResult Inspection_Sheet(string receivingNumber)
        {
            JObject retul = new JObject();
            var querylist = CommonERPDB.ERP_ContractQuery(receivingNumber, "").ToList();
            if (querylist.Count > 0)
            {
                foreach (var item in querylist)
                {
                    //收料员
                    retul.Add("rvaud02", item.rvaud02 == null ? null : item.rvaud02);
                    //收货单号
                    retul.Add("rva01", item.rva01 == null ? null : item.rva01);
                    //收货日期
                    retul.Add("rva06", item.rva06 == null ? null : item.rva06);
                    //供应厂商
                    retul.Add("rva05", item.rva05 == null ? null : item.rva05);
                    //采购单号
                    retul.Add("rvb04", item.rvb04 == null ? null : item.rvb04);
                    //付款方式
                    retul.Add("pay", item.pay == null ? null : item.pay);
                    //备注
                    retul.Add("rva08", item.rva08 == null ? null : item.rva08);
                    //行序
                    retul.Add("rvb01", item.rvb01 == null ? null : item.rvb01);
                    //料件编号（物料编号）
                    retul.Add("rvb05", item.rvb05 == null ? null : item.rvb05);

                }
            }
            return View();
        }

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
                        //int bacrcod = (int)bacrcodNum;//把数值转换成int类型                        
                        double bacrcod = Math.Ceiling(materialBasicInfo_Collection.MaterialNum / materialContainNum);//余数有小数点，加一
                        int num = 0;
                        for (int i = 1; i <= bacrcod; i++)
                        {
                            //查找物料条码前缀一样的最后一个序列号
                            var materBacrcode = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialNumber == materialBasicInfo_Collection.MaterialNumber).OrderByDescending(c => c.MaterialBacrcode).FirstOrDefault(); // 找到相同物料编号,相同生产日期符合条件的最后一个物料条码 
                            if (materBacrcode != null)
                            {
                                var dd = materBacrcode.MaterialBacrcode.Split('-');
                                string ff = dd[1];
                                if (ff == materialBasicInfo_Collection.Batch)
                                {
                                    string materNum = materBacrcode.MaterialBacrcode.Substring(materBacrcode.MaterialBacrcode.Length - 5, 5);//找到条码的最后序列号
                                    num = int.Parse(materNum) + 1;//序列号加一
                                }
                                else
                                {
                                    num++;
                                }
                            }
                            else
                            {
                                num++;
                            }
                            //找最后一个条码尾数的个数                            
                            if (i == Math.Truncate(bacrcod))
                            {
                                var number = Math.Truncate(bacrcod - 1) * materialContainNum;
                                var weishu = materialBasicInfo_Collection.MaterialNum - number;
                                materialContainNum = weishu;
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
            var bacrcodeList = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialNumber == date.MaterialNumber).Select(c => c.MaterialBacrcode).ToList();
            var putinbacrcode = db.Warehouse_putIn.Select(c => c.MaterialBacrcode).Distinct().ToList();
            var code = bacrcodeList.Except(putinbacrcode).ToList();//未入库的条码清单
            var ecx = bacrcodeList.Intersect(putinbacrcode).ToList();
            var codeList = db.Warehouse_Material_Outbound.Select(c => c.MaterialBacrcode).Distinct().ToList();//查找物料出库表里的所有条码清单
            var except = ecx.Except(codeList).ToList();//拿到没有出库的条码清单
            var differe = ecx.Intersect(codeList).ToList();//拿到已出库的条码清单
            foreach (var item in code) //未入库的条码清单           
            {
                var codeNum = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == item).Select(c => c.MaterialContainNum).FirstOrDefault();
                table.Add("meg", "未入库");
                table.Add("MaterialBacrcode", item);
                table.Add("MaterialContainNum", codeNum);
                retul.Add(table);
                table = new JObject();
            }
            foreach (var ite in except)//拿到没有出库的条码清单
            {
                var codeNum = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == ite).Select(c => c.MaterialContainNum).FirstOrDefault();
                table.Add("meg", "已入库未出库");
                table.Add("MaterialBacrcode", ite);
                table.Add("MaterialContainNum", codeNum);
                retul.Add(table);
                table = new JObject();
            }
            foreach (var it in differe)//拿到已出库的条码清单
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

        //10.保存新增标签条码方法
        public ActionResult ADDBarcodeLabel(string materialNumber, int code, string batch, double materialContainNum)
        {
            JObject retul = new JObject();
            JArray barcodeList = new JArray();
            int count = 0;
            if (!String.IsNullOrEmpty(materialNumber) && code != 0 && !String.IsNullOrEmpty(batch) && materialContainNum != 0)
            {
                for (int i = 1; i <= code; i++)
                {
                    int num = 1;

                    //查找物料条码前缀一样的最后一个序列号
                    var materBacrcode = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialNumber == materialNumber).OrderByDescending(c => c.MaterialBacrcode).FirstOrDefault(); // 找到相同物料编号,相同生产日期符合条件的最后一个物料条码
                    if (materBacrcode != null)
                    {
                        string materNum = materBacrcode.MaterialBacrcode.Substring(materBacrcode.MaterialBacrcode.Length - 5, 5);//找到条码的最后序列号
                        num = int.Parse(materNum) + 1;//序列号加一
                    }

                    //分割物料编号
                    var arr = materialNumber.Split('-');
                    string bacr = arr[0] + arr[1];

                    //组成条码 物料编号+生产时间的年月日+序列编号
                    string barcode = bacr + "-" + batch + "-" + num.ToString().PadLeft(5, '0');
                    //新建条码的数据信息
                    Warehouse_Material_Bacrcode material_Bacrcode = new Warehouse_Material_Bacrcode() { MaterialBacrcode = barcode, MaterialNumber = materialNumber, MaterialContainNum = materialContainNum, PrintTime = DateTime.Now, PrintName = ((Users)Session["User"]).UserName };//新建条码的数据信息
                    db.Warehouse_Material_Bacrcode.Add(material_Bacrcode);
                    count = db.SaveChanges();
                    barcodeList.Add(JsonConvert.SerializeObject(material_Bacrcode));
                }
                if (count > 0)
                {
                    retul.Add("meg", true);
                    retul.Add("fag", "保存成功！");
                    retul.Add("barcodeList", barcodeList);
                    return Content(JsonConvert.SerializeObject(retul));
                }
                else
                {
                    retul.Add("meg", false);
                    retul.Add("fag", "保存失败");
                    return Content(JsonConvert.SerializeObject(retul));
                }
            }
            retul.Add("meg", false);
            retul.Add("fag", "数据为空");
            return Content(JsonConvert.SerializeObject(retul));
        }

        //11.根据物料编号显示该物料的库存数、当天来料数、当天已入库数、当天已出库数、未入库和未出库数
        public ActionResult According_Inventory(string materialNumber)
        {
            JObject accord = new JObject();
            DateTime time = DateTime.Now;
            if (!String.IsNullOrEmpty(materialNumber))
            {
                var materialList = CommonERPDB.ERP_MaterialQuery(materialNumber, "").ToList();
                var inventorySum = materialList.Where(c => c.img01 == materialNumber).Select(c => c.img10).DefaultIfEmpty().Sum();//从ERP里找该物料编号的库存数量
                var incomingNum = db.Warehouse_MaterialBasicInfo_Collection.Where(c => c.MaterialNumber == materialNumber && c.InvoiceDate == time).Select(c => c.MaterialNum).FirstOrDefault();//根据物料编号和时间找当日来料数量        
                var bacrcodeList = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialNumber == materialNumber).Select(c => c.MaterialBacrcode).ToList();//根据物料编号找条码
                var putinbacrcode = db.Warehouse_putIn.Select(c => c.MaterialBacrcode).Distinct().ToList();//找所有已入库条码
                var code = bacrcodeList.Except(putinbacrcode).ToList();//未入库的条码清单
                var ecx = bacrcodeList.Intersect(putinbacrcode).ToList();//已入库的条码清单
                var codeList = db.Warehouse_Material_Outbound.Where(c => c.MaterialNumber == materialNumber).Select(c => c.MaterialBacrcode).Distinct().ToList();//根据物料编号查找物料出库表里的所有条码清单
                var except = ecx.Except(codeList).ToList();//拿到没有出库的条码清单
                var differe = ecx.Intersect(codeList).ToList();//拿到已出库的条码清单
                if (inventorySum != null)
                {
                    //物料编号
                    accord.Add("materialNumber", materialNumber);
                    //库存量
                    accord.Add("inventorySum", inventorySum);
                    //当天来料数量
                    accord.Add("materialNum", incomingNum);
                    //当天已入库数
                    accord.Add("ecx", ecx.Count);
                    //当天已入库条码清单
                    accord.Add("ecxList", JsonConvert.SerializeObject(ecx));
                    //当前未入库库数量
                    accord.Add("code", code.Count);
                    //当前未入库条码清单
                    accord.Add("codeList", JsonConvert.SerializeObject(code));
                    //当天已出库数量
                    accord.Add("differe", differe.Count);
                    //当天已出库条码清单
                    accord.Add("differeList", JsonConvert.SerializeObject(differe));
                    //当前未出库数量
                    accord.Add("except", except.Count);
                    //当前未出库条码清单
                    accord.Add("exceptList", JsonConvert.SerializeObject(except));
                }
            }
            return Content(JsonConvert.SerializeObject(accord));
        }

        #endregion
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
            for (var j = 0; j < pagecount; j++)
            {
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
                    string result = ZebraUnity.IPPrint(data.ToString(), 1, ip, port);
                    if (result == "打印成功！")
                    {
                        printcount++;
                        var code = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialBacrcode == item.MaterialNumber).ToList();
                        foreach (var it in code)
                        {
                            int cd = it.PrintNum;
                            it.PrintNum = cd + 1;
                            db.SaveChanges();
                        }
                    }
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

                Rectangle mc_materialDiscription = new Rectangle(35, 85, 490, 85);
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

        #region---重打标签条码

        //重打标签条码：获取物料编号
        public ActionResult GetMaterialNumberList()
        {
            var materialNumberList = db.Warehouse_MaterialBasicInfo_Collection.OrderByDescending(m => m.ID).Select(m => m.MaterialNumber).Distinct().ToList();
            JArray code = new JArray();
            foreach (var item in materialNumberList)
            {
                JObject retul = new JObject();
                retul.Add("value", item);
                code.Add(retul);
            }
            return Content(JsonConvert.SerializeObject(code));
        }

        #region--- 重打标签条码：根据物料编号获取生产日期
        //public ActionResult GetProductionDateList(string materialNumber)
        //{
        //    JArray retul = new JArray();
        //    var dateList = db.Warehouse_MaterialBasicInfo_Collection.OrderByDescending(c => c.ID).Where(c => c.MaterialNumber == materialNumber).Select(m => m.LeaveFactoryTime).Distinct().ToList();
        //    foreach (var item in dateList)
        //    {
        //        JObject code = new JObject();
        //        var value = string.Format("{0:yyyy-MM-dd}", item);
        //        code.Add("value", value);
        //        retul.Add(code);
        //    }
        //    return Content(JsonConvert.SerializeObject(retul));
        //}
        #endregion

        //重打标签条码：根据物料编号，获取批次
        public ActionResult ForBatchList(string materialNumber)
        {
            JArray retul = new JArray();
            var batch = db.Warehouse_MaterialBasicInfo_Collection.OrderByDescending(m => m.ID).Where(c => c.MaterialNumber == materialNumber).Select(c => c.Batch).Distinct().ToList();
            foreach (var item in batch)
            {
                JObject List = new JObject();
                List.Add("value", item);

                retul.Add(List);
            }
            return Content(JsonConvert.SerializeObject(retul));
        }

        //根据物料编号和批次，获取条码
        public ActionResult ObtainLabelBarcode(string materialNumber, string batch)
        {
            JArray retul = new JArray();
            JObject code = new JObject();
            JObject mater = new JObject();
            var materList = db.Warehouse_Material_Bacrcode.Where(c => c.MaterialNumber == materialNumber).Select(c => new { c.MaterialBacrcode, c.MaterialContainNum, c.ID }).ToList();
            var materialDiscription = db.Warehouse_MaterialBasicInfo_Collection.Where(c => c.MaterialNumber == materialNumber && c.Batch == batch).Select(c => c.MaterialDiscription).FirstOrDefault();
            if (materList.Count > 0)
            {
                foreach (var item in materList)
                {
                    //var bacrcode = item.MaterialBacrcode.Split('-')[1];//分割获取批次
                    if (item.MaterialBacrcode.Split('-')[1] == batch)
                    {
                        code.Add("ID", item.ID);
                        code.Add("MaterialBacrcode", item.MaterialBacrcode);
                        code.Add("MaterialContainNum", item.MaterialContainNum);
                        retul.Add(code);
                        code = new JObject();
                    }
                }
                mater.Add("MaterBacrcodeList", retul);
                mater.Add("MaterialNumber", materialNumber);
                mater.Add("MaterialDiscription", materialDiscription);
            }
            return Content(JsonConvert.SerializeObject(mater));
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
            var financedata = CommonERPDB.ERP_FinanceDetialsQuery(year, month);
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
                 { "-H", "11.易事达订单" },
                 { "-Y/-G/-A/-LA", "12.销售订单" },
                 { "-S", "13.费用挂大区单及赠送单" },
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
                resultrecord.Add("Category", "财务科目类");
                resultrecord.Add("Category_Detail", dict[item]);// item==140301? "原材料-基本材料": item == 140302 ? "原材料-辅助材料" : item == 140501? "成品" : item == 140502 ? "半成品":""
                var market1month = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
                resultrecord.Add("OneMonth", market1month.ToString("n4"));
                var market2_3month = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
                resultrecord.Add("TwoMonths", market2_3month.ToString("n4"));
                var market4_6month = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
                resultrecord.Add("FourMonths", market4_6month.ToString("n4"));
                var market7_12month = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
                resultrecord.Add("SevenMonths", market7_12month.ToString("n4"));
                var market1_2year = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
                resultrecord.Add("OneYear", market1_2year.ToString("n4"));
                var market2_3year = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
                resultrecord.Add("TwoYears", market2_3year.ToString("n4"));
                var marketr3_5year = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
                resultrecord.Add("ThreeYears", marketr3_5year.ToString("n4"));
                var marketover5year = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
                resultrecord.Add("FiveYears", marketover5year.ToString("n4"));
                var markettotal = market1month + market2_3month + market4_6month + market7_12month + market1_2year + market2_3year + marketr3_5year + marketover5year;
                resultrecord.Add("AmountTotal", markettotal.ToString("n4"));
                resultrecord.Add("RecordNum", financedata.Count(c => c.AccountingSubject == item));
                resultrecord.Add("Remark", item);
                results.Add(resultrecord);
                resultrecord = new JObject();
            }

            //增加在制品项
            resultrecord.Add("Category", "");
            resultrecord.Add("Category_Detail", "在制品类");
            var workinprocess1month_work = work_in_process.Where(c => c.durations == "1个月以内").Sum(c => c.ccg92);
            resultrecord.Add("OneMonth", workinprocess1month_work.ToString("n4"));
            var workinprocess2_3month_work = work_in_process.Where(c => c.durations == "2-3个月").Sum(c => c.ccg92);
            resultrecord.Add("TwoMonths", workinprocess2_3month_work.ToString("n4"));
            var workinprocess4_6month_work = work_in_process.Where(c => c.durations == "4-6个月").Sum(c => c.ccg92);
            resultrecord.Add("FourMonths", workinprocess4_6month_work.ToString("n4"));
            var workinprocess7_12month_work = work_in_process.Where(c => c.durations == "7-12个月").Sum(c => c.ccg92);
            resultrecord.Add("SevenMonths", workinprocess7_12month_work.ToString("n4"));
            var workinprocess1_2year_work = work_in_process.Where(c => c.durations == "1-2年").Sum(c => c.ccg92);
            resultrecord.Add("OneYear", workinprocess1_2year_work.ToString("n4"));
            var workinprocess2_3year_work = work_in_process.Where(c => c.durations == "2-3年").Sum(c => c.ccg92);
            resultrecord.Add("TwoYears", workinprocess2_3year_work.ToString("n4"));
            var workinprocess3_5year_work = work_in_process.Where(c => c.durations == "3-5年").Sum(c => c.ccg92);
            resultrecord.Add("ThreeYears", workinprocess3_5year_work.ToString("n4"));
            var workinprocessover5year_work = work_in_process.Where(c => c.durations == "5年以上").Sum(c => c.ccg92);
            resultrecord.Add("FiveYears", workinprocessover5year_work.ToString("n4"));
            var workinprocesstotal_work = workinprocess1month_work + workinprocess2_3month_work + workinprocess4_6month_work + workinprocess7_12month_work + workinprocess1_2year_work + workinprocess2_3year_work + workinprocess3_5year_work + workinprocessover5year_work;
            resultrecord.Add("AmountTotal", workinprocesstotal_work.ToString("n4"));
            resultrecord.Add("RecordNum", work_in_process.Count());
            results.Add(resultrecord);
            resultrecord = new JObject();


            resultrecord.Add("FiveYears", "全部合计");
            resultrecord.Add("AmountTotal", (Convert.ToDouble(total) + workinprocesstotal_work).ToString("n4"));
            resultrecord.Add("RecordNum", financedata.Count() + work_in_process.Count());
            results.Add(resultrecord);
            resultrecord = new JObject();


            //空白行
            resultrecord.Add("Category", "");
            resultrecord.Add("Category_Detail", "---");
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
            resultrecord.Add("Category", "成品类");
            resultrecord.Add("Category_Detail", "厂外仓");
            var market1month4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
            resultrecord.Add("OneMonth", market1month4.ToString("n4"));
            var market2_3month4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
            resultrecord.Add("TwoMonths", market2_3month4.ToString("n4"));
            var market4_6month4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
            resultrecord.Add("FourMonths", market4_6month4.ToString("n4"));
            var market7_12month4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
            resultrecord.Add("SevenMonths", market7_12month4.ToString("n4"));
            var market1_2year4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
            resultrecord.Add("OneYear", market1_2year4.ToString("n4"));
            var market2_3year4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
            resultrecord.Add("TwoYears", market2_3year4.ToString("n4"));
            var market3_5year4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
            resultrecord.Add("ThreeYears", market3_5year4.ToString("n4"));
            var marketover5year4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
            resultrecord.Add("FiveYears", marketover5year4.ToString("n4"));
            var markettotal4 = market1month4 + market2_3month4 + market4_6month4 + market7_12month4 + market1_2year4 + market2_3year4 + market3_5year4 + marketover5year4;
            resultrecord.Add("AmountTotal", markettotal4.ToString("n4"));
            resultrecord.Add("RecordNum", finished_product_warehouse.Count);
            financefinish140501 = financefinish140501.Except(finished_product_warehouse).ToList(); //排除厂外仓记录
            results.Add(resultrecord);
            resultrecord = new JObject();
            #endregion

            #region 夹具模具
            //第二级，夹具模具
            //成品 夹具模具
            var financefinish_clamp = financefinish140501.Where(c => c.tc_cxc01.StartsWith("2")).ToList();
            resultrecord.Add("Category", "成品类");
            resultrecord.Add("Category_Detail", "夹具模具");
            var market1month2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
            resultrecord.Add("OneMonth", market1month2.ToString("n4"));
            var market2_3month2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
            resultrecord.Add("TwoMonths", market2_3month2.ToString("n4"));
            var market4_6month2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
            resultrecord.Add("FourMonths", market4_6month2.ToString("n4"));
            var market7_12month2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
            resultrecord.Add("SevenMonths", market7_12month2.ToString("n4"));
            var market1_2year2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
            resultrecord.Add("OneYear", market1_2year2.ToString("n4"));
            var market2_3year2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
            resultrecord.Add("TwoYears", market2_3year2.ToString("n4"));
            var market3_5year2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
            resultrecord.Add("ThreeYears", market3_5year2.ToString("n4"));
            var marketover5year2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
            resultrecord.Add("FiveYears", marketover5year2.ToString("n4"));
            var markettotal2 = market1month2 + market2_3month2 + market4_6month2 + market7_12month2 + market1_2year2 + market2_3year2 + market3_5year2 + marketover5year2;
            resultrecord.Add("AmountTotal", markettotal2.ToString("n4"));
            resultrecord.Add("RecordNum", financefinish_clamp.Count);
            financefinish140501 = financefinish140501.Except(financefinish_clamp).ToList(); //排除夹具模组记录
            results.Add(resultrecord);
            resultrecord = new JObject();
            #endregion

            #region 单项计算
            foreach (var it in compare_dic.Keys.ToArray())
            {
                var temp = financefinish140501.Where(c => c.Classification == compare_dic[it]).ToList();
                resultrecord.Add("Category", "成品类");
                resultrecord.Add("Category_Detail", compare_dic[it] + "(" + it + ")");
                var market1month = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
                resultrecord.Add("OneMonth", market1month.ToString("n4"));
                var market2_3month = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
                resultrecord.Add("TwoMonths", market2_3month.ToString("n4"));
                var market4_6month = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
                resultrecord.Add("FourMonths", market4_6month.ToString("n4"));
                var market7_12month = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
                resultrecord.Add("SevenMonths", market7_12month.ToString("n4"));
                var market1_2year = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
                resultrecord.Add("OneYear", market1_2year.ToString("n4"));
                var market2_3year = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
                resultrecord.Add("TwoYears", market2_3year.ToString("n4"));
                var market3_5year = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
                resultrecord.Add("ThreeYears", market3_5year.ToString("n4"));
                var marketover5year = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
                resultrecord.Add("FiveYears", marketover5year.ToString("n4"));
                var markettotal = market1month + market2_3month + market4_6month + market7_12month + market1_2year + market2_3year + market3_5year + marketover5year;
                resultrecord.Add("AmountTotal", markettotal.ToString("n4"));
                resultrecord.Add("RecordNum", temp.Count);
                financefinish140501 = financefinish140501.Except(temp).ToList();
                results.Add(resultrecord);
                resultrecord = new JObject();
            }
            #endregion

            resultrecord.Add("FiveYears", "全部合计");
            resultrecord.Add("AmountTotal", financefinish140501_sum.ToString("n4"));
            resultrecord.Add("RecordNum", financefinish140501_count);
            results.Add(resultrecord);
            resultrecord = new JObject();
            #endregion

            //空白行
            resultrecord.Add("Category", "");
            resultrecord.Add("Category_Detail", "---");
            results.Add(resultrecord);
            resultrecord = new JObject();

            #endregion

            #region 在制品部分

            #region 单项计算
            foreach (var it in compare_dic.Keys.ToArray())
            {
                var work_in_process_include = work_in_process.Where(c => c.Classification == compare_dic[it]).ToList();
                resultrecord.Add("Category", "在制品类");
                resultrecord.Add("Category_Detail", compare_dic[it] + "(" + it + ")");
                var workinprocess1month = work_in_process_include.Where(c => c.durations == "1个月以内").Sum(c => c.ccg92);
                resultrecord.Add("OneMonth", workinprocess1month.ToString("n4"));
                var workinprocess2_3month = work_in_process_include.Where(c => c.durations == "2-3个月").Sum(c => c.ccg92);
                resultrecord.Add("TwoMonths", workinprocess2_3month.ToString("n4"));
                var workinprocess4_6month = work_in_process_include.Where(c => c.durations == "4-6个月").Sum(c => c.ccg92);
                resultrecord.Add("FourMonths", workinprocess4_6month.ToString("n4"));
                var workinprocess7_12month = work_in_process_include.Where(c => c.durations == "7-12个月").Sum(c => c.ccg92);
                resultrecord.Add("SevenMonths", workinprocess7_12month.ToString("n4"));
                var workinprocess1_2year = work_in_process_include.Where(c => c.durations == "1-2年").Sum(c => c.ccg92);
                resultrecord.Add("OneYear", workinprocess1_2year.ToString("n4"));
                var workinprocess2_3year = work_in_process_include.Where(c => c.durations == "2-3年").Sum(c => c.ccg92);
                resultrecord.Add("TwoYears", workinprocess2_3year.ToString("n4"));
                var workinprocess3_5year = work_in_process_include.Where(c => c.durations == "3-5年").Sum(c => c.ccg92);
                resultrecord.Add("ThreeYears", workinprocess3_5year.ToString("n4"));
                var workinprocessover5year = work_in_process_include.Where(c => c.durations == "5年以上").Sum(c => c.ccg92);
                resultrecord.Add("FiveYears", workinprocessover5year.ToString("n4"));
                var workinprocesstotal = workinprocess1month + workinprocess2_3month + workinprocess4_6month + workinprocess7_12month + workinprocess1_2year + workinprocess2_3year + workinprocess3_5year + workinprocessover5year;
                resultrecord.Add("AmountTotal", workinprocesstotal.ToString("n4"));
                resultrecord.Add("RecordNum", work_in_process_include.Count());
                results.Add(resultrecord);
                resultrecord = new JObject();
            }
            #endregion

            resultrecord.Add("Category", "");
            resultrecord.Add("Category_Detail", "");
            resultrecord.Add("FiveYears", "全部合计");
            resultrecord.Add("AmountTotal", work_in_process.Sum(c => c.ccg92).ToString("n4"));
            resultrecord.Add("RecordNum", work_in_process.Count());
            results.Add(resultrecord);
            resultrecord = new JObject();

            //空白行
            resultrecord.Add("Category", "");
            resultrecord.Add("Category_Detail", "---");
            results.Add(resultrecord);
            resultrecord = new JObject();
            #endregion

            #region----修改后PMC未发料、原材料类
            //PMC未发料表(修改后方案)
            var raw_material_nuissus = CommonERPDB.ERP_MC_NuIssueDetialsQuery2(year, month);
            List<cxcr006_raw_material> result7 = new List<cxcr006_raw_material>();
            //取到原材料表记录
            var financedata_Long2 = CXCR_ConvertType(financedata.Where(c => c.AccountingSubject != 140501).ToList());
            result7 = Calculate_raw(financedata_Long2, raw_material_nuissus);


            //PMC未表发料明细表(原材料)
            resultrecord.Add("Category", "PMC未发料明细表");
            resultrecord.Add("Category_Detail", "");
            resultrecord.Add("RecordNum", raw_material_nuissus.Count());
            results.Add(resultrecord);
            resultrecord = new JObject();

            compare_dic.Add("NoOrder", "无订单需求");

            #region 单项计算
            foreach (var it in compare_dic.Keys.ToArray())
            {
                var financedata_raw_material_result = result7.Where(c => c.Classification == compare_dic[it]).ToList();
                resultrecord.Add("Category", "原材料类");
                resultrecord.Add("Category_Detail", compare_dic[it] + "(" + it + ")");
                var raw_material1month = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
                resultrecord.Add("OneMonth", raw_material1month.ToString("n4"));
                var raw_material2_3month = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
                resultrecord.Add("TwoMonths", raw_material2_3month.ToString("n4"));
                var raw_material4_6month = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
                resultrecord.Add("FourMonths", raw_material4_6month.ToString("n4"));
                var raw_material7_12month = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
                resultrecord.Add("SevenMonths", raw_material7_12month.ToString("n4"));
                var raw_material1_2year = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
                resultrecord.Add("OneYear", raw_material1_2year.ToString("n4"));
                var raw_material2_3year = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
                resultrecord.Add("TwoYears", raw_material2_3year.ToString("n4"));
                var raw_material3_5year = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
                resultrecord.Add("ThreeYears", raw_material3_5year.ToString("n4"));
                var raw_materialover5year = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
                resultrecord.Add("FiveYears", raw_materialover5year.ToString("n4"));
                var raw_material_sum = raw_material1month + raw_material2_3month + raw_material4_6month + raw_material7_12month + raw_material1_2year + raw_material2_3year + raw_material3_5year + raw_materialover5year;
                resultrecord.Add("AmountTotal", raw_material_sum.ToString("n4"));
                resultrecord.Add("RecordNum", financedata_raw_material_result.Count);
                results.Add(resultrecord);
                resultrecord = new JObject();
            }
            #endregion

            resultrecord.Add("FiveYears", "汇总结果求和");
            resultrecord.Add("AmountTotal", result7.Where(c => c.Classification != "原始记录").Sum(c => c.tc_cxc06).ToString("n4"));
            resultrecord.Add("RecordNum", result7.Where(c => c.Classification != "原始记录").Count());
            results.Add(resultrecord);
            resultrecord = new JObject();

            #endregion

            return Content(JsonConvert.SerializeObject(results));
        }

        //导出Excel表
        [HttpPost]
        public ActionResult StockAmountCalculateGetExcel(string outputexcelfile, int year, int month)
        {
            #region-----原程序
            ////cxcr006 成品类表头
            //string[] columns = { "料号", "品名", "规格", "面积", "本月结存数", "本月结存单价", "本月结存金额", "30天", "90天", "180天", "365天", "2年", "3年", "3-5年", "5年以上", "年度", "月份", "会计科目", "库位号", "分类明细" };
            ////cxcr006_raw_material 原材料类表头
            //string[] columns_raw_material = { "料号", "品名", "规格", "面积", "本月结存数", "本月结存单价", "本月结存金额", "30天", "90天", "180天", "365天", "2年", "3年", "3-5年", "5年以上", "会计科目", "库位号", "分类明细", "工单号", "工单备注", "工单录入日期", "单位", "应发数量", "已发数量", "未发数量", "年度", "月份" };
            ////在制品类表头
            //string[] columns_work_in_process = { "工单号", "料号", "期间", "工单年月份", "年度", "月份", "上月结存数量", "上月结存金额", "物料品名", "其他分群码四", "物料规格", "是否为重覆性生产料件 (Y/N)", "重工否(Y/N)", "备注", "分类明细" };
            ////csfr008 MC表发料记录表表头
            //string[] columns_raw_material_MC = { "工单号", "工单备注", "工单录入日期", "生产数量", "入库数量", "料号", "品名", "规格", "单位", "应发数量", "已发数量", "未发数量", "年度", "月份", "订单类型" };

            //byte[] filecontent = null;
            //string re = "";
            ////财务整个月底结存表记录
            //List<cxcr006> financealldata = CommonERPDB.ERP_FinanceDetialsQuery(year, month);
            ////筛选出科目140501的记录
            //var financedata = financealldata.Where(c => c.AccountingSubject == 140501).ToList();
            ////财务上月库存结存明细表排除140501科目以外的记录：原材料-基本材料，原材料-辅助材料，半成品
            ////var financedata_raw_material = financealldata.Where(c => c.AccountingSubject != 140501).ToList();
            ////MC发料表
            //// var raw_material = CommonERPDB.ERP_MC_NuIssueDetialsQuery(inputdate, enddate).Where(c => c.sfa05_sfa06 > 0).ToList();

            ////MC未发料表(修改后方案)
            //var raw_material_nuissus = CommonERPDB.ERP_MC_NuIssueDetialsQuery2(year, month);

            ////第一优先级应该是厂外仓，然后是备库和样品，再然后是配件订单，剩下的就是销售订单了和其他的
            //switch (outputexcelfile)
            //{
            //    #region 财务分科
            //    case "财务科目类":
            //        //filecontent = ExcelExportHelper.ExportExcel(financealldata.ToList(), outputexcelfile + "(" + DateTime.Now.ToString("D") + "）", false, columns);
            //        filecontent = ExcelExportHelper.ExportExcel(financealldata.ToList(), year+"年"+month+"月" + outputexcelfile , false, columns);
            //        break;
            //    #endregion

            //    #region 成品类         
            //    case "成品类":
            //        //按年月查出仓库编号是LCWC1，且库位编号是CWC01的记录
            //        var outsiderecord = CommonERPDB.OutsideWarehouseQuery(year, month);
            //        //取出厂外仓的所有物料号
            //        var mn_list = outsiderecord.Select(c => c.imk01).ToList();
            //        //根据厂外仓物料号统计厂外仓信息
            //        var outsideWS = financedata.Where(c => mn_list.Contains(c.tc_cxc01)).ToList();
            //        outsideWS.ForEach(c => { c.Classification = "厂外仓"; });
            //        var clampWS = financedata.Where(c => c.tc_cxc01.StartsWith("2")).ToList();
            //        clampWS.ForEach(c => { c.Classification = "夹具模具"; });
            //        filecontent = ExcelExportHelper.ExportExcel(financedata.ToList(), year + "年" + month + "月" +outputexcelfile , false, columns);
            //        break;
            //    #endregion

            //    #region 在制品类
            //    case "在制品类":
            //        var result5 = CommonERPDB.ERP_Work_in_process(year, month);
            //        filecontent = ExcelExportHelper.ExportExcel(result5, year + "年" + month + "月" +outputexcelfile , false, columns_work_in_process);
            //        break;
            //    #endregion

            //    #region----修改后PMC未发料方案
            //    case "PMC未发料明细表":
            //        filecontent = ExcelExportHelper.ExportExcel(raw_material_nuissus, year + "年" + month + "月" +outputexcelfile , false, columns_raw_material_MC);
            //        break;

            //    #region case "原材料类":
            //    case "原材料类":
            //        List<cxcr006_raw_material> result7 = new List<cxcr006_raw_material>();
            //        //取到原材料表记录
            //        var financedata_Long2 = CXCR_ConvertType(financealldata.Where(c => c.AccountingSubject != 140501).ToList());
            //        result7 = Calculate_raw(financedata_Long2, raw_material_nuissus);
            //        filecontent = ExcelExportHelper.ExportExcel(result7, year + "年" + month + "月" +outputexcelfile , false, columns_raw_material);
            //        break;
            //    #endregion
            //    #endregion

            //    default:
            //        re = "整个财务结存表";
            //        filecontent = ExcelExportHelper.ExportExcel(financealldata, year + "年" + month + "月" +re , false, columns);
            //        break;
            //}
            #endregion

            var filecontent = GetExcel(outputexcelfile, year, month);
            return File(filecontent, ExcelExportHelper.ExcelContentType, year + "年" + month + "月" + outputexcelfile + ".xlsx");
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
                        { "-H", "11.易事达订单" },
                        { "-Y/-G/-A/-LA", "12.销售订单" },
                        { "-S", "13.费用挂大区单及赠送单" },
                        { "Others", "14.其他订单"}
                    };
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
        public FileContentResult ExportExcel(string tableData, int year, int month)
        {
            string[] column = { "类别", "分类明细", "1个月以内", "2-3个月", "4-6个月", "7-12个月", "1-2年", "2-3年", "3-5年", "5年以上", "合计金额", "记录条数", "备注" };

            DataTable table = new DataTable();
            var array = JsonConvert.DeserializeObject(tableData) as JArray;
            if (array.Count > 0)
            {
                StringBuilder columns = new StringBuilder();
                JObject objColumns = array[0] as JObject;
                //构造表头
                foreach (var jkon in column)
                {
                    string name = jkon;
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
                        string name = "";
                        switch (((JProperty)(jkon)).Name)
                        {
                            case "Category":
                                name = "类别";
                                break;
                            case "Category_Detail":
                                name = "分类明细";
                                break;
                            case "OneMonth":
                                name = "1个月以内";
                                break;
                            case "TwoMonths":
                                name = "2-3个月";
                                break;
                            case "FourMonths":
                                name = "4-6个月";
                                break;
                            case "SevenMonths":
                                name = "7-12个月";
                                break;
                            case "OneYear":
                                name = "1-2年";
                                break;
                            case "TwoYears":
                                name = "2-3年";
                                break;
                            case "ThreeYears":
                                name = "3-5年";
                                break;
                            case "FiveYears":
                                name = "5年以上";
                                break;
                            case "AmountTotal":
                                name = "合计金额";
                                break;
                            case "RecordNum":
                                name = "记录条数";
                                break;
                            case "Remark":
                                name = "备注";
                                break;
                        }
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

        #region---库存金额生成

        #region---页面
        public ActionResult StockAmount_Query()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Warehouse_Material", act = "StockAmount_Query" });
            }
            return View();
        }
        public ActionResult StockAmount_Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Warehouse_Material", act = "StockAmount_Index" });
            }
            return View();
        }
        #endregion

        #region--- 查询
        [HttpPost]
        public ActionResult StockAmount_Query(DateTime date)
        {
            JObject record = new JObject();
            JObject result = new JObject();
            var theSameMonth = db.InventoryAmount_Record.Where(c => c.Year == date.Year && c.Month == date.Month).OrderBy(c => c.Month).ToList();//当月数据
            int year = date.Year;
            int month = (date.Month)-1;
            if (month == 0) {
                year = (date.Year)-1;
            }
            var lastMonth = db.InventoryAmount_Record.Where(c => c.Year == year && c.Month == month).OrderBy(c => c.Month).ToList();//上月数据
            var res = db.InventoryAmount_Assessed.Where(c => c.Year == date.Year && c.Month == date.Month).ToList();
            result.Add("theSameMonth", JsonConvert.SerializeObject(theSameMonth));
            result.Add("lastMonth", JsonConvert.SerializeObject(lastMonth));
            result.Add("res", JsonConvert.SerializeObject(res));
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region---生成报表，并保存类别的excel文件
        //返回字节流
        [HttpPost]
        public byte[] GetExcel(string outputexcelfile, int year, int month)
        {
            //cxcr006 成品类表头
            string[] columns = { "料号", "品名", "规格", "面积", "本月结存数", "本月结存单价", "本月结存金额", "30天", "90天", "180天", "365天", "2年", "3年", "3-5年", "5年以上", "年度", "月份", "会计科目", "库位号", "分类明细" };
            //cxcr006_raw_material 原材料类表头
            string[] columns_raw_material = { "料号", "品名", "规格", "面积", "本月结存数", "本月结存单价", "本月结存金额", "30天", "90天", "180天", "365天", "2年", "3年", "3-5年", "5年以上", "会计科目", "库位号", "分类明细", "工单号", "工单备注", "工单录入日期", "单位", "应发数量", "已发数量", "未发数量", "年度", "月份" };
            //在制品类表头
            string[] columns_work_in_process = { "工单号", "料号", "期间", "工单年月份", "年度", "月份", "上月结存数量", "上月结存金额", "物料品名", "其他分群码四", "物料规格", "是否为重覆性生产料件 (Y/N)", "重工否(Y/N)", "备注", "分类明细" };
            //csfr008 MC表发料记录表表头
            string[] columns_raw_material_MC = { "工单号", "工单备注", "工单录入日期", "生产数量", "入库数量", "料号", "品名", "规格", "单位", "应发数量", "已发数量", "未发数量", "年度", "月份", "订单类型" };

            byte[] filecontent = null;
            string re = "";
            //财务整个月底结存表记录
            List<cxcr006> financealldata = CommonERPDB.ERP_FinanceDetialsQuery(year, month);
            //筛选出科目140501的记录
            var financedata = financealldata.Where(c => c.AccountingSubject == 140501).ToList();

            //MC未发料表(修改后方案)
            var raw_material_nuissus = CommonERPDB.ERP_MC_NuIssueDetialsQuery2(year, month);

            //第一优先级应该是厂外仓，然后是备库和样品，再然后是配件订单，剩下的就是销售订单了和其他的
            switch (outputexcelfile)
            {
                #region 财务分科
                case "财务科目类":
                    filecontent = ExcelExportHelper.ExportExcel(financealldata.ToList(), year + "年" + month + "月" + outputexcelfile, false, columns);
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
                    filecontent = ExcelExportHelper.ExportExcel(financedata.ToList(), year + "年" + month + "月" + outputexcelfile, false, columns);
                    break;
                #endregion

                #region 在制品类
                case "在制品类":
                    var result5 = CommonERPDB.ERP_Work_in_process(year, month);
                    filecontent = ExcelExportHelper.ExportExcel(result5, year + "年" + month + "月" + outputexcelfile, false, columns_work_in_process);
                    break;
                #endregion

                #region----修改后PMC未发料方案
                case "PMC未发料明细表":
                    filecontent = ExcelExportHelper.ExportExcel(raw_material_nuissus, year + "年" + month + "月" + outputexcelfile, false, columns_raw_material_MC);
                    break;

                #region case "原材料类":
                case "原材料类":
                    List<cxcr006_raw_material> result7 = new List<cxcr006_raw_material>();
                    //取到原材料表记录
                    var financedata_Long2 = CXCR_ConvertType(financealldata.Where(c => c.AccountingSubject != 140501).ToList());
                    result7 = Calculate_raw(financedata_Long2, raw_material_nuissus);
                    filecontent = ExcelExportHelper.ExportExcel(result7, year + "年" + month + "月" + outputexcelfile, false, columns_raw_material);
                    break;
                #endregion
                #endregion

                default:
                    re = "整个财务结存表";
                    filecontent = ExcelExportHelper.ExportExcel(financealldata, year + "年" + month + "月" + re, false, columns);
                    break;
            }
            return filecontent;
        }

        ////保存
        public ActionResult CreateTable(List<InventoryAmount_Record> record, DateTime date)
        {
            if (db.InventoryAmount_Record.Count(c => c.Year == date.Year && c.Month == date.Month) == 0)//判断date.month这个月是否有报告生成
            {
                JArray category = new JArray();
                List<InventoryAmount_Record> result = new List<InventoryAmount_Record>();
                foreach (var item in record)
                {
                    item.Year = date.Year;//年
                    item.Month = date.Month;//月 
                    result.Add(item);
                    if (!string.IsNullOrEmpty(item.Category))
                    {
                        category.Add(item.Category);
                    }
                }
                db.InventoryAmount_Record.AddRange(result);
                int saveCount = db.SaveChanges();//记录表               
                InventoryAmount_Assessed newRecord = new InventoryAmount_Assessed();
                newRecord.Year = date.Year;
                newRecord.Month = date.Month;
                newRecord.Create_Person = ((Users)Session["User"]).UserName;//生成人
                newRecord.CreateTime = DateTime.Now;//生成时间
                db.InventoryAmount_Assessed.Add(newRecord);
                int count = db.SaveChanges();

                var re = category.Distinct();//类别去重
                int savenum = 0;
                foreach (var it in re)
                {
                    var filecontent = GetExcel(it.ToString(), date.Year, date.Month);
                    //获取配置文件中设置的保存
                    string f = System.Configuration.ConfigurationManager.AppSettings["SaveFilePath"];
                    //虚拟路径,保存地址
                    string savePath = @"D:\MES_Data\StockAmount\" + date.Year + "\\" + date.Month + "\\" + it + ".xlsx";

                    string FilePath = f + savePath;
                    if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                    }
                    //保存文件
                    FileStream fstream = System.IO.File.Create(FilePath, filecontent.Length);//创建文件流
                    try
                    {
                        fstream.Write(filecontent, 0, filecontent.Length);//把二进制流数据写入文件    
                        savenum++;
                    }
                    catch (Exception ex)
                    {
                        return Content("保存出错！");
                    }
                    finally
                    {
                        fstream.Close();
                    }
                }
                if (saveCount > 0 && count > 0 && savenum > 0) return Content("生成报表成功！");
                else return Content("生成报表失败！");
            }
            return Content("本月报表已存在，不允许重复生成！");
        }

        #endregion

        #region--- 审核
        public ActionResult Approve(int id, string reason, string approve_Type, bool approve_Result)
        {
            if (id != 0 && !string.IsNullOrEmpty(approve_Type))
            {
                var record = db.InventoryAmount_Assessed.Where(c => c.Id == id).FirstOrDefault();
                int count = 0;
                //1表示财务核对
                if (approve_Type == "1")
                {
                    record.Finance_Proofread = approve_Result;
                    record.Finance_Proofread_Date = DateTime.Now;
                    record.Finance_Proofread_Reason = reason;
                    record.Finance_Proofreader = ((Users)Session["User"]).UserName;
                    count = db.SaveChanges();
                }
                //2表示PMC核对
                else if (approve_Type == "2")
                {
                    record.PMC_Proofread = approve_Result;
                    record.PMC_Proofread_Date = DateTime.Now;
                    record.PMC_Proofread_Reason = reason;
                    record.Finance_Finance_Proofreader = ((Users)Session["User"]).UserName;
                    count = db.SaveChanges();
                }
                //财务审核
                else
                {
                    record.Finance_Assessed = approve_Result;
                    record.Finance_Assessed_Date = DateTime.Now;
                    record.Finance_Assessed_Reason = reason;
                    record.Finance_Assessor = ((Users)Session["User"]).UserName;
                    count = db.SaveChanges();
                }
                if (count > 0) return Content(approve_Type != "3" ? "核对成功！" : "审核成功！");
                else return Content(approve_Type != "3" ? "核对失败" : "审核失败！");
            }
            else return Content("传入参数为空！");
        }


        #endregion

        #region--- 删除
        [HttpPost]
        public ActionResult DeleteReport(DateTime date)
        {
            //删除记录表中的记录
            var recordlist = db.InventoryAmount_Record.Where(c => c.Year == date.Year && c.Month == date.Month).ToList();
            db.InventoryAmount_Record.RemoveRange(recordlist);
            int recordCount = db.SaveChanges();
            //删除审核表中记录
            var approveList = db.InventoryAmount_Assessed.Where(c => c.Year == date.Year && c.Month == date.Month).ToList();
            db.InventoryAmount_Assessed.RemoveRange(approveList);
            int approveCount = db.SaveChanges();
            //删除excel文件
            List<FileInfo> fileInfos = null;
            int deleteFileNum = 0;
            if (Directory.Exists(@"D:\MES_Data\StockAmount\" + date.Year + "\\" + date.Month + "\\"))//判断文件是否存在
            {
                fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\StockAmount\" + date.Year + "\\" + date.Month + "\\");//遍历文件夹
                foreach (var item in fileInfos)//foreach删除文件
                {
                    System.IO.File.Delete(item.FullName);
                    Console.WriteLine(item.FullName);
                    deleteFileNum++;
                }
            }
            if (recordCount > 0 && approveCount > 0 && deleteFileNum == fileInfos.Count()) return Content("删除成功！");
            else return Content("删除失败！");
        }
        #endregion

        #region--- 首页查询
        [HttpPost]
        public ActionResult SA_Index(DateTime year, string queryType)
        {
            JArray result = new JArray();
            List<InventoryAmount_Assessed> list = new List<InventoryAmount_Assessed>();
            if (queryType == "original")
            {
                list = db.InventoryAmount_Assessed.Where(c => c.Year == year.Year).OrderBy(c => c.Month).ToList();
            }
            else
            {
                list = db.InventoryAmount_Assessed.Where(c => c.Year == year.Year && c.Finance_Assessed != null && c.Finance_Proofread != null && c.PMC_Proofread != null).OrderBy(c => c.Month).ToList();
            }
            foreach (var item in list)
            {
                JObject res = new JObject();
                res.Add("Id", item.Id);
                res.Add("Time", item.Year + "-" + item.Month);

                res.Add("Finance_Proofreader", item.Finance_Proofreader == null ? "" : item.Finance_Proofreader);//财务核对人
                res.Add("Finance_Proofread_Date", item.Finance_Proofreader == null ? "" : Convert.ToDateTime(item.Finance_Proofread_Date).ToString("yyyy-MM-dd HH:mm:ss"));

                res.Add("Finance_Finance_Proofreader", item.Finance_Finance_Proofreader == null ? "" : item.Finance_Finance_Proofreader);//PMC核对
                res.Add("PMC_Proofread_Date", item.Finance_Finance_Proofreader == null ? "" : Convert.ToDateTime(item.PMC_Proofread_Date).ToString("yyyy-MM-dd HH:mm:ss"));

                res.Add("Finance_Assessor", item.Finance_Assessor == null ? "" : item.Finance_Assessor);
                res.Add("Finance_Assessed_Date", item.Finance_Assessor == null ? "" : Convert.ToDateTime(item.Finance_Assessed_Date).ToString("yyyy-MM-dd HH:mm:ss"));
                result.Add(res);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        #endregion

        #region---查询已保存excel文件
        public string LookExcel(string category, DateTime date)
        {
            List<FileInfo> draw_files = null;
            if (Directory.Exists(@"D:\MES_Data\StockAmount\" + date.Year + "\\" + date.Month + "\\"))//判断图纸的总路径是否存在
            {
                draw_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\StockAmount\" + date.Year + "\\" + date.Month + "\\");//遍历图纸文件夹的文件个数
            }
            if (draw_files != null)
            {
                JArray drawjpg_list = new JArray();//用于存放后缀为.jpg图纸的数组                   
                foreach (var i in draw_files)
                {
                    string name = category + ".xlsx";
                    if (i.ToString() == name)
                    {//判断是否是对应科目的文件
                        string path1 = @"/MES_Data/StockAmount/" + date.Year + "/" + date.Month + "/" + i;//组合出路径
                        return path1;
                    }
                }
                return "找不到对应文件";
            }
            else
            {
                return "找不到对应文件";
            }
        }
        #endregion

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


    //Api接口部分

    public class Warehouse_Material_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController common = new CommonController();

        #region---库存金额

        #region---方法内调用的方法
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
        //取到原材料表记录
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
                        { "-H", "11.易事达订单" },
                        { "-Y/-G/-A/-LA", "12.销售订单" },
                        { "-S", "13.费用挂大区单及赠送单" },
                        { "Others", "14.其他订单"}
                    };
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
        #endregion

        #region--实时查询ERP方法
        [HttpPost]
        [ApiAuthorize]
        public JObject StockAmountCalculate([System.Web.Http.FromBody]JObject data)//int year, int month, bool outputexcelfile = false
        {
            //检查ERP
            int year = int.Parse(data["year"].ToString());
            int month = int.Parse(data["month"].ToString());
            JObject result = new JObject();
            var connectERPresult = CommonERPDB.TryConnectERP();
            if (connectERPresult != "连接正常")
            {                
                result.Add("Data", connectERPresult);            
                return common.GetModuleFromJobjet(result, false, "查询失败");
                //return Content("连接ERP数据库服务器失败！请检查网络或者ERP数据库服务器是否已启动。");//{"ORA-12541: TNS: 无监听程序"}
            }

            #region 成品部分
            //按年月查出仓库编号是LCWC1，且库位编号是CWC01的记录
            var outsiderecord = CommonERPDB.OutsideWarehouseQuery(year, month);
            //开始财务上月库存结存明细表查询时间
            DateTime queryfinancedatabegin = DateTime.Now;
            //财务上月库存结存明细表查询   单价tc_cxc05
            var financedata = CommonERPDB.ERP_FinanceDetialsQuery(year, month);
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
            //if (outputexcelfile == true)
            //{
            //    string[] columns = { "料号", "品名", "规格", "面积", "本月结存数", "本月结存单价", "本月结存金额", "30天", "90天", "180天", "365天", "2年", "3年", "3-5年", "5年以上", "会计科目", "库位号" };
            //    byte[] filecontent = ExcelExportHelper.ExportExcel(financedata, "ERP导出财务月底库存结算表" + DateTime.Now.ToString("D") + "）", false, columns);
            //    return File(filecontent, ExcelExportHelper.ExcelContentType, "库存结算表（" + DateTime.Now.ToString("D") + "）.xlsx");
            //}

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
                 { "-H", "11.易事达订单" },
                 { "-Y/-G/-A/-LA", "12.销售订单" },
                 { "-S", "13.费用挂大区单及赠送单" },
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
                resultrecord.Add("Category", "财务科目类");
                resultrecord.Add("Category_Detail", dict[item]);// item==140301? "原材料-基本材料": item == 140302 ? "原材料-辅助材料" : item == 140501? "成品" : item == 140502 ? "半成品":""
                var market1month = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
                resultrecord.Add("OneMonth", market1month.ToString("n4"));
                var market2_3month = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
                resultrecord.Add("TwoMonths", market2_3month.ToString("n4"));
                var market4_6month = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
                resultrecord.Add("FourMonths", market4_6month.ToString("n4"));
                var market7_12month = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
                resultrecord.Add("SevenMonths", market7_12month.ToString("n4"));
                var market1_2year = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
                resultrecord.Add("OneYear", market1_2year.ToString("n4"));
                var market2_3year = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
                resultrecord.Add("TwoYears", market2_3year.ToString("n4"));
                var marketr3_5year = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
                resultrecord.Add("ThreeYears", marketr3_5year.ToString("n4"));
                var marketover5year = financedata.Where(c => c.AccountingSubject == item).Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
                resultrecord.Add("FiveYears", marketover5year.ToString("n4"));
                var markettotal = market1month + market2_3month + market4_6month + market7_12month + market1_2year + market2_3year + marketr3_5year + marketover5year;
                resultrecord.Add("AmountTotal", markettotal.ToString("n4"));
                resultrecord.Add("RecordNum", financedata.Count(c => c.AccountingSubject == item));
                resultrecord.Add("Remark", item);
                results.Add(resultrecord);
                resultrecord = new JObject();
            }

            //增加在制品项
            resultrecord.Add("Category", "");
            resultrecord.Add("Category_Detail", "在制品类");
            var workinprocess1month_work = work_in_process.Where(c => c.durations == "1个月以内").Sum(c => c.ccg92);
            resultrecord.Add("OneMonth", workinprocess1month_work.ToString("n4"));
            var workinprocess2_3month_work = work_in_process.Where(c => c.durations == "2-3个月").Sum(c => c.ccg92);
            resultrecord.Add("TwoMonths", workinprocess2_3month_work.ToString("n4"));
            var workinprocess4_6month_work = work_in_process.Where(c => c.durations == "4-6个月").Sum(c => c.ccg92);
            resultrecord.Add("FourMonths", workinprocess4_6month_work.ToString("n4"));
            var workinprocess7_12month_work = work_in_process.Where(c => c.durations == "7-12个月").Sum(c => c.ccg92);
            resultrecord.Add("SevenMonths", workinprocess7_12month_work.ToString("n4"));
            var workinprocess1_2year_work = work_in_process.Where(c => c.durations == "1-2年").Sum(c => c.ccg92);
            resultrecord.Add("OneYear", workinprocess1_2year_work.ToString("n4"));
            var workinprocess2_3year_work = work_in_process.Where(c => c.durations == "2-3年").Sum(c => c.ccg92);
            resultrecord.Add("TwoYears", workinprocess2_3year_work.ToString("n4"));
            var workinprocess3_5year_work = work_in_process.Where(c => c.durations == "3-5年").Sum(c => c.ccg92);
            resultrecord.Add("ThreeYears", workinprocess3_5year_work.ToString("n4"));
            var workinprocessover5year_work = work_in_process.Where(c => c.durations == "5年以上").Sum(c => c.ccg92);
            resultrecord.Add("FiveYears", workinprocessover5year_work.ToString("n4"));
            var workinprocesstotal_work = workinprocess1month_work + workinprocess2_3month_work + workinprocess4_6month_work + workinprocess7_12month_work + workinprocess1_2year_work + workinprocess2_3year_work + workinprocess3_5year_work + workinprocessover5year_work;
            resultrecord.Add("AmountTotal", workinprocesstotal_work.ToString("n4"));
            resultrecord.Add("RecordNum", work_in_process.Count());
            results.Add(resultrecord);
            resultrecord = new JObject();


            resultrecord.Add("FiveYears", "全部合计");
            resultrecord.Add("AmountTotal", (Convert.ToDouble(total) + workinprocesstotal_work).ToString("n4"));
            resultrecord.Add("RecordNum", financedata.Count() + work_in_process.Count());
            results.Add(resultrecord);
            resultrecord = new JObject();


            //空白行
            resultrecord.Add("Category", "");
            resultrecord.Add("Category_Detail", "---");
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
            resultrecord.Add("Category", "成品类");
            resultrecord.Add("Category_Detail", "厂外仓");
            var market1month4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
            resultrecord.Add("OneMonth", market1month4.ToString("n4"));
            var market2_3month4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
            resultrecord.Add("TwoMonths", market2_3month4.ToString("n4"));
            var market4_6month4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
            resultrecord.Add("FourMonths", market4_6month4.ToString("n4"));
            var market7_12month4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
            resultrecord.Add("SevenMonths", market7_12month4.ToString("n4"));
            var market1_2year4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
            resultrecord.Add("OneYear", market1_2year4.ToString("n4"));
            var market2_3year4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
            resultrecord.Add("TwoYears", market2_3year4.ToString("n4"));
            var market3_5year4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
            resultrecord.Add("ThreeYears", market3_5year4.ToString("n4"));
            var marketover5year4 = finished_product_warehouse.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
            resultrecord.Add("FiveYears", marketover5year4.ToString("n4"));
            var markettotal4 = market1month4 + market2_3month4 + market4_6month4 + market7_12month4 + market1_2year4 + market2_3year4 + market3_5year4 + marketover5year4;
            resultrecord.Add("AmountTotal", markettotal4.ToString("n4"));
            resultrecord.Add("RecordNum", finished_product_warehouse.Count);
            financefinish140501 = financefinish140501.Except(finished_product_warehouse).ToList(); //排除厂外仓记录
            results.Add(resultrecord);
            resultrecord = new JObject();
            #endregion

            #region 夹具模具
            //第二级，夹具模具
            //成品 夹具模具
            var financefinish_clamp = financefinish140501.Where(c => c.tc_cxc01.StartsWith("2")).ToList();
            resultrecord.Add("Category", "成品类");
            resultrecord.Add("Category_Detail", "夹具模具");
            var market1month2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
            resultrecord.Add("OneMonth", market1month2.ToString("n4"));
            var market2_3month2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
            resultrecord.Add("TwoMonths", market2_3month2.ToString("n4"));
            var market4_6month2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
            resultrecord.Add("FourMonths", market4_6month2.ToString("n4"));
            var market7_12month2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
            resultrecord.Add("SevenMonths", market7_12month2.ToString("n4"));
            var market1_2year2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
            resultrecord.Add("OneYear", market1_2year2.ToString("n4"));
            var market2_3year2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
            resultrecord.Add("TwoYears", market2_3year2.ToString("n4"));
            var market3_5year2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
            resultrecord.Add("ThreeYears", market3_5year2.ToString("n4"));
            var marketover5year2 = financefinish_clamp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
            resultrecord.Add("FiveYears", marketover5year2.ToString("n4"));
            var markettotal2 = market1month2 + market2_3month2 + market4_6month2 + market7_12month2 + market1_2year2 + market2_3year2 + market3_5year2 + marketover5year2;
            resultrecord.Add("AmountTotal", markettotal2.ToString("n4"));
            resultrecord.Add("RecordNum", financefinish_clamp.Count);
            financefinish140501 = financefinish140501.Except(financefinish_clamp).ToList(); //排除夹具模组记录
            results.Add(resultrecord);
            resultrecord = new JObject();
            #endregion

            #region 单项计算
            foreach (var it in compare_dic.Keys.ToArray())
            {
                var temp = financefinish140501.Where(c => c.Classification == compare_dic[it]).ToList();
                resultrecord.Add("Category", "成品类");
                resultrecord.Add("Category_Detail", compare_dic[it] + "(" + it + ")");
                var market1month = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
                resultrecord.Add("OneMonth", market1month.ToString("n4"));
                var market2_3month = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
                resultrecord.Add("TwoMonths", market2_3month.ToString("n4"));
                var market4_6month = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
                resultrecord.Add("FourMonths", market4_6month.ToString("n4"));
                var market7_12month = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
                resultrecord.Add("SevenMonths", market7_12month.ToString("n4"));
                var market1_2year = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
                resultrecord.Add("OneYear", market1_2year.ToString("n4"));
                var market2_3year = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
                resultrecord.Add("TwoYears", market2_3year.ToString("n4"));
                var market3_5year = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
                resultrecord.Add("ThreeYears", market3_5year.ToString("n4"));
                var marketover5year = temp.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
                resultrecord.Add("FiveYears", marketover5year.ToString("n4"));
                var markettotal = market1month + market2_3month + market4_6month + market7_12month + market1_2year + market2_3year + market3_5year + marketover5year;
                resultrecord.Add("AmountTotal", markettotal.ToString("n4"));
                resultrecord.Add("RecordNum", temp.Count);
                financefinish140501 = financefinish140501.Except(temp).ToList();
                results.Add(resultrecord);
                resultrecord = new JObject();
            }
            #endregion

            resultrecord.Add("FiveYears", "全部合计");
            resultrecord.Add("AmountTotal", financefinish140501_sum.ToString("n4"));
            resultrecord.Add("RecordNum", financefinish140501_count);
            results.Add(resultrecord);
            resultrecord = new JObject();
            #endregion

            //空白行
            resultrecord.Add("Category", "");
            resultrecord.Add("Category_Detail", "---");
            results.Add(resultrecord);
            resultrecord = new JObject();

            #endregion

            #region 在制品部分

            #region 单项计算
            foreach (var it in compare_dic.Keys.ToArray())
            {
                var work_in_process_include = work_in_process.Where(c => c.Classification == compare_dic[it]).ToList();
                resultrecord.Add("Category", "在制品类");
                resultrecord.Add("Category_Detail", compare_dic[it] + "(" + it + ")");
                var workinprocess1month = work_in_process_include.Where(c => c.durations == "1个月以内").Sum(c => c.ccg92);
                resultrecord.Add("OneMonth", workinprocess1month.ToString("n4"));
                var workinprocess2_3month = work_in_process_include.Where(c => c.durations == "2-3个月").Sum(c => c.ccg92);
                resultrecord.Add("TwoMonths", workinprocess2_3month.ToString("n4"));
                var workinprocess4_6month = work_in_process_include.Where(c => c.durations == "4-6个月").Sum(c => c.ccg92);
                resultrecord.Add("FourMonths", workinprocess4_6month.ToString("n4"));
                var workinprocess7_12month = work_in_process_include.Where(c => c.durations == "7-12个月").Sum(c => c.ccg92);
                resultrecord.Add("SevenMonths", workinprocess7_12month.ToString("n4"));
                var workinprocess1_2year = work_in_process_include.Where(c => c.durations == "1-2年").Sum(c => c.ccg92);
                resultrecord.Add("OneYear", workinprocess1_2year.ToString("n4"));
                var workinprocess2_3year = work_in_process_include.Where(c => c.durations == "2-3年").Sum(c => c.ccg92);
                resultrecord.Add("TwoYears", workinprocess2_3year.ToString("n4"));
                var workinprocess3_5year = work_in_process_include.Where(c => c.durations == "3-5年").Sum(c => c.ccg92);
                resultrecord.Add("ThreeYears", workinprocess3_5year.ToString("n4"));
                var workinprocessover5year = work_in_process_include.Where(c => c.durations == "5年以上").Sum(c => c.ccg92);
                resultrecord.Add("FiveYears", workinprocessover5year.ToString("n4"));
                var workinprocesstotal = workinprocess1month + workinprocess2_3month + workinprocess4_6month + workinprocess7_12month + workinprocess1_2year + workinprocess2_3year + workinprocess3_5year + workinprocessover5year;
                resultrecord.Add("AmountTotal", workinprocesstotal.ToString("n4"));
                resultrecord.Add("RecordNum", work_in_process_include.Count());
                results.Add(resultrecord);
                resultrecord = new JObject();
            }
            #endregion

            resultrecord.Add("Category", "");
            resultrecord.Add("Category_Detail", "");
            resultrecord.Add("FiveYears", "全部合计");
            resultrecord.Add("AmountTotal", work_in_process.Sum(c => c.ccg92).ToString("n4"));
            resultrecord.Add("RecordNum", work_in_process.Count());
            results.Add(resultrecord);
            resultrecord = new JObject();

            //空白行
            resultrecord.Add("Category", "");
            resultrecord.Add("Category_Detail", "---");
            results.Add(resultrecord);
            resultrecord = new JObject();
            #endregion

            #region----修改后PMC未发料、原材料类
            //PMC未发料表(修改后方案)
            var raw_material_nuissus = CommonERPDB.ERP_MC_NuIssueDetialsQuery2(year, month);
            List<cxcr006_raw_material> result7 = new List<cxcr006_raw_material>();
            //取到原材料表记录
            var financedata_Long2 = CXCR_ConvertType(financedata.Where(c => c.AccountingSubject != 140501).ToList());
            result7 = Calculate_raw(financedata_Long2, raw_material_nuissus);


            //PMC未表发料明细表(原材料)
            resultrecord.Add("Category", "PMC未发料明细表");
            resultrecord.Add("Category_Detail", "");
            resultrecord.Add("RecordNum", raw_material_nuissus.Count());
            results.Add(resultrecord);
            resultrecord = new JObject();

            compare_dic.Add("NoOrder", "无订单需求");

            #region 单项计算
            foreach (var it in compare_dic.Keys.ToArray())
            {
                var financedata_raw_material_result = result7.Where(c => c.Classification == compare_dic[it]).ToList();
                resultrecord.Add("Category", "原材料类");
                resultrecord.Add("Category_Detail", compare_dic[it] + "(" + it + ")");
                var raw_material1month = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc07);
                resultrecord.Add("OneMonth", raw_material1month.ToString("n4"));
                var raw_material2_3month = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc08);
                resultrecord.Add("TwoMonths", raw_material2_3month.ToString("n4"));
                var raw_material4_6month = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc09);
                resultrecord.Add("FourMonths", raw_material4_6month.ToString("n4"));
                var raw_material7_12month = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc10);
                resultrecord.Add("SevenMonths", raw_material7_12month.ToString("n4"));
                var raw_material1_2year = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc11);
                resultrecord.Add("OneYear", raw_material1_2year.ToString("n4"));
                var raw_material2_3year = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc12);
                resultrecord.Add("TwoYears", raw_material2_3year.ToString("n4"));
                var raw_material3_5year = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc13);
                resultrecord.Add("ThreeYears", raw_material3_5year.ToString("n4"));
                var raw_materialover5year = financedata_raw_material_result.Sum(c => (c.tc_cxc06 / c.tc_cxc04) * c.tc_cxc14);
                resultrecord.Add("FiveYears", raw_materialover5year.ToString("n4"));
                var raw_material_sum = raw_material1month + raw_material2_3month + raw_material4_6month + raw_material7_12month + raw_material1_2year + raw_material2_3year + raw_material3_5year + raw_materialover5year;
                resultrecord.Add("AmountTotal", raw_material_sum.ToString("n4"));
                resultrecord.Add("RecordNum", financedata_raw_material_result.Count);
                results.Add(resultrecord);
                resultrecord = new JObject();
            }
            #endregion

            resultrecord.Add("FiveYears", "汇总结果求和");
            resultrecord.Add("AmountTotal", result7.Where(c => c.Classification != "原始记录").Sum(c => c.tc_cxc06).ToString("n4"));
            resultrecord.Add("RecordNum", result7.Where(c => c.Classification != "原始记录").Count());
            results.Add(resultrecord);
            resultrecord = new JObject();

            #endregion

            result.Add("Data", results);
            return common.GetModuleFromJobjet(result, true, "查询成功！");
        }
        #endregion

        #region--导出excel文件
        [HttpPost]
        [ApiAuthorize]
        public HttpResponseMessage StockAmountCalculateExcel([System.Web.Http.FromBody]JObject data)
        {
            HttpResponseMessage result = null;
            string outputexcelfile = data["outputexcelfile"].ToString();
            int year = int.Parse(data["year"].ToString());
            int month = int.Parse(data["month"].ToString());
            var filecontent = GetExcel(outputexcelfile, year, month);
            try
            {               
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(filecontent);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = year + "年" + month + "月" + outputexcelfile + ".xlsx";
                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Gone);
            }
        }

        #endregion

        #region--- 库存金额首页查询
        [HttpPost]
        [ApiAuthorize]
        public JObject SA_Index([System.Web.Http.FromBody]JObject data)
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var data_Obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = data_Obj.year;
            JArray returnInfo = new JArray();
            List<InventoryAmount_Assessed> list = new List<InventoryAmount_Assessed>();
            JObject result = new JObject();
            if (data_Obj.queryType == "original")
            {
                list = db.InventoryAmount_Assessed.Where(c => c.Year == year).OrderBy(c => c.Month).ToList();
            }
            else if (data_Obj.queryType == "approved")
            {
                list = db.InventoryAmount_Assessed.Where(c => c.Year == year && c.Finance_Assessed != null && c.Finance_Proofread != null && c.PMC_Proofread != null).OrderBy(c => c.Month).ToList();
            }
            else {
                result.Add("Data", returnInfo);
                return common.GetModuleFromJobjet(result, true, "没有查询权限");
            }
            foreach (var item in list)
            {
                JObject res = new JObject();
                res.Add("Id", item.Id);
                res.Add("Time", item.Year + "-" + item.Month);

                res.Add("Finance_Proofreader", item.Finance_Proofreader == null ? "" : item.Finance_Proofreader);//财务核对人
                res.Add("Finance_Proofread_Date", item.Finance_Proofreader == null ? "" : Convert.ToDateTime(item.Finance_Proofread_Date).ToString("yyyy-MM-dd HH:mm:ss"));

                res.Add("Finance_Finance_Proofreader", item.Finance_Finance_Proofreader == null ? "" : item.Finance_Finance_Proofreader);//PMC核对
                res.Add("PMC_Proofread_Date", item.Finance_Finance_Proofreader == null ? "" : Convert.ToDateTime(item.PMC_Proofread_Date).ToString("yyyy-MM-dd HH:mm:ss"));

                res.Add("Finance_Assessor", item.Finance_Assessor == null ? "" : item.Finance_Assessor);
                res.Add("Finance_Assessed_Date", item.Finance_Assessor == null ? "" : Convert.ToDateTime(item.Finance_Assessed_Date).ToString("yyyy-MM-dd HH:mm:ss"));
                returnInfo.Add(res);
            }           
            result.Add("Data", returnInfo);
            return common.GetModuleFromJobjet(result, true, "查询成功！");
        }

        #endregion

        #region---查询已保存excel文件
        [HttpPost]
        [ApiAuthorize]
        public JObject LookExcel([System.Web.Http.FromBody]JObject data)//string category, DateTime date
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            DateTime date = obj.date;//时间
            var category = obj.category;//科目类别
            JObject result = new JObject();
            List<FileInfo> draw_files = null;
            if (Directory.Exists(@"D:\MES_Data\StockAmount\" + date.Year + "\\" + date.Month + "\\"))//判断图纸的总路径是否存在
            {
                draw_files = comm.GetAllFilesInDirectory(@"D:\MES_Data\StockAmount\" + date.Year + "\\" + date.Month + "\\");//遍历图纸文件夹的文件个数
            }
            if (draw_files != null)
            {
                foreach (var i in draw_files)
                {
                    string name = category + ".xlsx";
                    if (i.ToString() == name)
                    {//判断是否是对应科目的文件
                        string path1 = @"/MES_Data/StockAmount/" + date.Year + "/" + date.Month + "/" + i;//组合出路径
                        result.Add("Data", path1);                      
                        return common.GetModuleFromJobjet(result, true, "导出成功！");
                    }
                }
                return common.GetModuleFromJobjet(result, false, "找不到对应文件");
            }
            else
            {               
                return common.GetModuleFromJobjet(result, false, "找不到对应文件");
            }
        }
        #endregion

        #region--- 删除
        [HttpPost]
        [ApiAuthorize]
        public JObject reportDelete([System.Web.Http.FromBody]JObject data)//DateTime date
        {
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            DateTime date = obj.date;//时间           
            //删除记录表中的记录
            var recordlist = db.InventoryAmount_Record.Where(c => c.Year == date.Year && c.Month == date.Month).ToList();
            db.InventoryAmount_Record.RemoveRange(recordlist);
            int recordCount = db.SaveChanges();
            //删除审核表中记录
            var approveList = db.InventoryAmount_Assessed.Where(c => c.Year == date.Year && c.Month == date.Month).ToList();
            db.InventoryAmount_Assessed.RemoveRange(approveList);
            int approveCount = db.SaveChanges();
            //删除excel文件
            List<FileInfo> fileInfos = null;
            int deleteFileNum = 0;
            if (Directory.Exists(@"D:\MES_Data\StockAmount\" + date.Year + "\\" + date.Month + "\\"))//判断文件是否存在
            {
                fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\StockAmount\" + date.Year + "\\" + date.Month + "\\");//遍历文件夹
                foreach (var item in fileInfos)//foreach删除文件
                {
                    System.IO.File.Delete(item.FullName);
                    Console.WriteLine(item.FullName);
                    deleteFileNum++;
                }
            }
            if (recordCount > 0 && approveCount > 0 && deleteFileNum == fileInfos.Count())
            {            
                return common.GetModuleFromJobjet(result, true, "删除成功！");
            }
            else
            {
                return common.GetModuleFromJobjet(result, false, "删除失败！");
            }
        }
        #endregion

        #region--- 审核
        [HttpPost]
        [ApiAuthorize]
        public JObject Approve([System.Web.Http.FromBody]JObject data)//int id, string reason, string approve_Type, bool approve_Result
        {
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];         
            int id = int.Parse(data["id"].ToString());
            string reason = data["reason"].ToString();
            string approve_Type = data["approve_Type"].ToString();
            bool approve_Result =bool.Parse(data["approve_Result"].ToString());
            string UserName = auth.UserName;
            JObject result = new JObject();
            if (id != 0 && !string.IsNullOrEmpty(approve_Type))
            {
                var record = db.InventoryAmount_Assessed.Where(c => c.Id == id).FirstOrDefault();
                int count = 0;
                //1表示财务核对
                if (approve_Type == "1")
                {
                    record.Finance_Proofread = approve_Result;
                    record.Finance_Proofread_Date = DateTime.Now;
                    record.Finance_Proofread_Reason = reason;
                    record.Finance_Proofreader = UserName;
                    count = db.SaveChanges();
                }
                //2表示PMC核对
                else if (approve_Type == "2")
                {
                    record.PMC_Proofread = approve_Result;
                    record.PMC_Proofread_Date = DateTime.Now;
                    record.PMC_Proofread_Reason = reason;
                    record.Finance_Finance_Proofreader = UserName;
                    count = db.SaveChanges();
                }
                //财务审核
                else
                {
                    record.Finance_Assessed = approve_Result;
                    record.Finance_Assessed_Date = DateTime.Now;
                    record.Finance_Assessed_Reason = reason;
                    record.Finance_Assessor = UserName;
                    count = db.SaveChanges();
                }
                if (count > 0)
                {
                    if (approve_Type != "3")
                    {
                        return common.GetModuleFromJobjet(result, true, "核对成功！");
                    }
                    else
                    {
                        return common.GetModuleFromJobjet(result, true, "审核成功！");
                    }                   
                }
                else
                {
                    if (approve_Type != "3")
                    {
                        return common.GetModuleFromJobjet(result, false, "核对失败！");
                    }
                    else
                    {
                        return common.GetModuleFromJobjet(result, false, "审核失败！");
                    }
                }
            }
            else
            {
                return common.GetModuleFromJobjet(result, false, "传入参数为空！");
            }
        }
        #endregion

        #region--- 查询
        [HttpPost]
        [ApiAuthorize]
        public JObject StockAmount_Query([System.Web.Http.FromBody]JObject data)//DateTime date
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            DateTime date = obj.date;
            JObject record = new JObject();
            JObject result = new JObject();
            int year = date.Year;
            int month = (date.Month) - 1;
            if (month == 0)
            {
                year = (date.Year) - 1;
            }
            var lastMonth = db.InventoryAmount_Record.Where(c => c.Year == year && c.Month == month).OrderBy(c => c.Month).ToList();//上月数据
            var list = db.InventoryAmount_Record.Where(c => c.Year == date.Year && c.Month == date.Month).OrderBy(c => c.Month).ToList();//当月数据
            var res = db.InventoryAmount_Assessed.Where(c => c.Year == date.Year && c.Month == date.Month).ToList();
            result.Add("Data", JsonConvert.SerializeObject(list));//当月数据
            result.Add("lastMonth", JsonConvert.SerializeObject(lastMonth));//上月数据
            result.Add("DataOne", JsonConvert.SerializeObject(res));
            result.Add("PostResult", comm.ReturnApiPostStatus());
            return common.GetModuleFromJobjet(result, true, "查询成功！");
        }
        #endregion

        #region---生成报表，并保存类别的excel文件
        //返回字节流
        public byte[] GetExcel(string outputexcelfile, int year, int month)
        {
            //cxcr006 成品类表头
            string[] columns = { "料号", "品名", "规格", "面积", "本月结存数", "本月结存单价", "本月结存金额", "30天", "90天", "180天", "365天", "2年", "3年", "3-5年", "5年以上", "年度", "月份", "会计科目", "库位号", "分类明细" };
            //cxcr006_raw_material 原材料类表头
            string[] columns_raw_material = { "料号", "品名", "规格", "面积", "本月结存数", "本月结存单价", "本月结存金额", "30天", "90天", "180天", "365天", "2年", "3年", "3-5年", "5年以上", "会计科目", "库位号", "分类明细", "工单号", "工单备注", "工单录入日期", "单位", "应发数量", "已发数量", "未发数量", "年度", "月份" };
            //在制品类表头
            string[] columns_work_in_process = { "工单号", "料号", "期间", "工单年月份", "年度", "月份", "上月结存数量", "上月结存金额", "物料品名", "其他分群码四", "物料规格", "是否为重覆性生产料件 (Y/N)", "重工否(Y/N)", "备注", "分类明细" };
            //csfr008 MC表发料记录表表头
            string[] columns_raw_material_MC = { "工单号", "工单备注", "工单录入日期", "生产数量", "入库数量", "料号", "品名", "规格", "单位", "应发数量", "已发数量", "未发数量", "年度", "月份", "订单类型" };

            byte[] filecontent = null;
            string re = "";
            //财务整个月底结存表记录
            List<cxcr006> financealldata = CommonERPDB.ERP_FinanceDetialsQuery(year, month);
            //筛选出科目140501的记录
            var financedata = financealldata.Where(c => c.AccountingSubject == 140501).ToList();

            //MC未发料表(修改后方案)
            var raw_material_nuissus = CommonERPDB.ERP_MC_NuIssueDetialsQuery2(year, month);

            //第一优先级应该是厂外仓，然后是备库和样品，再然后是配件订单，剩下的就是销售订单了和其他的
            switch (outputexcelfile)
            {
                #region 财务分科
                case "财务科目类":
                    filecontent = ExcelExportHelper.ExportExcel(financealldata.ToList(), year + "年" + month + "月" + outputexcelfile, false, columns);
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
                    filecontent = ExcelExportHelper.ExportExcel(financedata.ToList(), year + "年" + month + "月" + outputexcelfile, false, columns);
                    break;
                #endregion

                #region 在制品类
                case "在制品类":
                    var result5 = CommonERPDB.ERP_Work_in_process(year, month);
                    filecontent = ExcelExportHelper.ExportExcel(result5, year + "年" + month + "月" + outputexcelfile, false, columns_work_in_process);
                    break;
                #endregion

                #region----修改后PMC未发料方案
                case "PMC未发料明细表":
                    filecontent = ExcelExportHelper.ExportExcel(raw_material_nuissus, year + "年" + month + "月" + outputexcelfile, false, columns_raw_material_MC);
                    break;

                #region case "原材料类":
                case "原材料类":
                    List<cxcr006_raw_material> result7 = new List<cxcr006_raw_material>();
                    //取到原材料表记录
                    var financedata_Long2 = CXCR_ConvertType(financealldata.Where(c => c.AccountingSubject != 140501).ToList());
                    result7 = Calculate_raw(financedata_Long2, raw_material_nuissus);
                    filecontent = ExcelExportHelper.ExportExcel(result7, year + "年" + month + "月" + outputexcelfile, false, columns_raw_material);
                    break;
                #endregion
                #endregion

                default:
                    re = "整个财务结存表";
                    filecontent = ExcelExportHelper.ExportExcel(financealldata, year + "年" + month + "月" + re, false, columns);
                    break;
            }
            return filecontent;
        }

        ////保存
        [HttpPost]
        [ApiAuthorize]
        public JObject CreateTable([System.Web.Http.FromBody]JObject data)//List<InventoryAmount_Record> record, DateTime date
        {
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            JObject result = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            List<InventoryAmount_Record> NewList = (List<InventoryAmount_Record>)JsonHelper.jsonDes<List<InventoryAmount_Record>>(data["record"].ToString());
            List<InventoryAmount_Record> record = NewList;//记录
            DateTime date = obj.date;//日期
            if (db.InventoryAmount_Record.Count(c => c.Year == date.Year && c.Month == date.Month) == 0)//判断date.month这个月是否有报告生成
            {
                JArray category = new JArray();
                List<InventoryAmount_Record> res = new List<InventoryAmount_Record>();
                foreach (var item in record)
                {
                    item.Year = date.Year;//年
                    item.Month = date.Month;//月 
                    res.Add(item);
                    if (!string.IsNullOrEmpty(item.Category))
                    {
                        category.Add(item.Category);
                    }
                }
                db.InventoryAmount_Record.AddRange(res);
                int saveCount = db.SaveChanges();//记录表               
                InventoryAmount_Assessed newRecord = new InventoryAmount_Assessed();
                newRecord.Year = date.Year;
                newRecord.Month = date.Month;
                newRecord.Create_Person = auth.UserName;//生成人
                newRecord.CreateTime = DateTime.Now;//生成时间
                db.InventoryAmount_Assessed.Add(newRecord);
                int count = db.SaveChanges();

                var re = category.Distinct();//类别去重
                int savenum = 0;
                foreach (var it in re)
                {
                    var filecontent = GetExcel(it.ToString(), date.Year, date.Month);//保存excel文件到服务器
                    //获取配置文件中设置的保存
                    string f = System.Configuration.ConfigurationManager.AppSettings["SaveFilePath"];
                    //虚拟路径,保存地址
                    string savePath = @"D:\MES_Data\StockAmount\" + date.Year + "\\" + date.Month + "\\" + it + ".xlsx";

                    string FilePath = f + savePath;
                    if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                    }
                    //保存文件
                    FileStream fstream = System.IO.File.Create(FilePath, filecontent.Length);//创建文件流
                    try
                    {
                        fstream.Write(filecontent, 0, filecontent.Length);//把二进制流数据写入文件    
                        savenum++;
                    }
                    catch (Exception ex)
                    {
                        return common.GetModuleFromJobjet(result,false,"生成报表失败！");
                    }
                    finally
                    {
                        fstream.Close();
                    }
                }
                if (saveCount > 0 && count > 0 && savenum > 0)
                {
                    return common.GetModuleFromJobjet(result,true,"生成报表成功！");
                }
                else {
                    return common.GetModuleFromJobjet(result, false, "生成报表失败！");
                }
            }         
            return common.GetModuleFromJobjet(result, false, "本月报表已存在，不允许重复生成！");
        }
        #endregion

        #region---导出excel
        #region---临时类
        public class InventoryAmount_NewList
        {
            public string Category { get; set; }
            public string Category_Detail { get; set; }
            public string OneMonth { get; set; }
            public string TwoMonths { get; set; }
            public string FourMonths { get; set; }
            public string SevenMonths { get; set; }
            public string OneYear { get; set; }
            public string TwoYears { get; set; }
            public string ThreeYears { get; set; }
            public string FiveYears { get; set; }
            public string AmountTotal { get; set; }
            public string RecordNum { get; set; }
            public string Remark { get; set; }
        }
        #endregion
        [HttpPost]
        [ApiAuthorize]
        public HttpResponseMessage exportExcel([System.Web.Http.FromBody]JObject data)//原方法参数：string tableData, int year, int month
        {
            HttpResponseMessage result = null;
            var jsonStr = JsonConvert.SerializeObject(data);
            var str = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int year = int.Parse(data["year"].ToString());
            int month = int.Parse(data["month"].ToString());
            string[] column = { "类别", "分类明细", "1个月以内", "2-3个月", "4-6个月", "7-12个月", "1-2年", "2-3年", "3-5年", "5年以上", "合计金额", "记录条数", "备注" };
            List<InventoryAmount_NewList> NewList = (List<InventoryAmount_NewList>)JsonHelper.jsonDes<List<InventoryAmount_NewList>>(data["tableData"].ToString());
            DataTable ttt = DataTableTool.ToDataTable(NewList);
            byte[] filecontent = ExcelExportHelper.ExportExcel(ttt, "库存金额表（统计时间:" + year + "年" + month + "月", false,column);
            try
            {
                result = Request.CreateResponse(HttpStatusCode.OK);
                var stream = new MemoryStream(filecontent);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType=new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = "库存金额表（统计时间:" + year + "年" + month + "月" + ".xlsx";
                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Gone);
            }
        }
        #endregion

        #endregion

        #region---仓库物料
        #region---有效期规则
        //有效期规则录入
        [HttpPost]
        [ApiAuthorize]
        public Object PeriodValidity_Input([System.Web.Http.FromBody]JObject data)//List<Warehouse_Material_ValidityPeriod> record
        {
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            List<Warehouse_Material_ValidityPeriod> record = obj;
            JObject result = new JObject();
            if (auth.UserName != null)
            {
                int count = 0;
                if (record.Count > 0)
                {
                    foreach (var item in record)
                    {
                        if (db.Warehouse_Material_ValidityPeriod.Count(c => c.MaterialNumber == item.MaterialNumber) < 1)
                        {//检验重复
                            item.CreateDate = DateTime.Now;
                            item.Creator =auth.UserName;
                            db.Warehouse_Material_ValidityPeriod.Add(item);
                            count += db.SaveChanges();
                        }
                    }
                };
                if (count > 0)
                {
                    result.Add("Result", true);
                    result.Add("Message", "保存成功！");
                    result.Add("PostResult", comm.ReturnApiPostStatus());
                    return common.GetModuleFromJobjet(result);
                }
                else
                {
                    result.Add("Result", false);
                    result.Add("Message", "保存失败！");
                    result.Add("PostResult", comm.ReturnApiPostStatus());
                    return common.GetModuleFromJobjet(result);                   
                }
            }
            else
            {
                result.Add("Result", false);
                result.Add("Message", "没有登录！");
                result.Add("PostResult", comm.ReturnApiPostStatus());
                return common.GetModuleFromJobjet(result);
            }
        }
        //有效期规则查询
        [HttpPost]
        [ApiAuthorize]
        public JObject PeriodValidity_Query([System.Web.Http.FromBody]JObject data)//string materialNum 物料号
        {
            List<Warehouse_Material_ValidityPeriod> record = new List<Warehouse_Material_ValidityPeriod>();
            string materialNum = data["materialNum"].ToString();
            if (!String.IsNullOrEmpty(materialNum))
            {
                record = db.Warehouse_Material_ValidityPeriod.Where(c => c.MaterialNumber == materialNum).ToList();
            }
            else
            {
                record = db.Warehouse_Material_ValidityPeriod.ToList();
            }
            JObject result = new JObject();
            JArray list = new JArray();
            list.Add(record);
            result.Add("Data", list);
            result.Add("Result", true);
            result.Add("Message", "查询成功！");
            result.Add("PostResult", comm.ReturnApiPostStatus());
            return common.GetModuleFromJobjet(result);
        }
        //有效期规则修改
        [HttpPost]
        [ApiAuthorize]
        public JObject PeriodValidity_Modify([System.Web.Http.FromBody]JObject data)//Warehouse_Material_ValidityPeriod record
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            Warehouse_Material_ValidityPeriod record = obj.record;
            if (record != null)
            {
                var list = db.Warehouse_Material_ValidityPeriod.Where(c => c.ID == record.ID).FirstOrDefault();
                list.MaterialNumber = record.MaterialNumber;//物料号
                list.ValidityPeriod = record.ValidityPeriod;//有效期
                list.Modifier = auth.UserName;//修改人
                list.ModifyDate = DateTime.Now;//修改时间
                int count = db.SaveChanges();
                if (count > 0)
                {
                    result.Add("Result", true);
                    result.Add("Message", "修改成功！");                  
                    result.Add("PostResult", comm.ReturnApiPostStatus());
                    return common.GetModuleFromJobjet(result);
                }
                else
                {
                    result.Add("Result", false);
                    result.Add("Message", "修改失败！");
                    result.Add("PostResult", comm.ReturnApiPostStatus());
                    return common.GetModuleFromJobjet(result);
                }
            }
            result.Add("Result", false);
            result.Add("Message", "传入数据为空！");
            result.Add("PostResult", comm.ReturnApiPostStatus());
            return common.GetModuleFromJobjet(result); ;
        }
        //有效期规则删除
        [HttpPost]
        [ApiAuthorize]
        public Object PeriodValidity_Delete([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            int id = int.Parse(data["ID"].ToString());
            if (id != 0) {
                var list = db.Warehouse_Material_ValidityPeriod.Where(c => c.ID == id).ToList();                
                UserOperateLog operaterecord = new UserOperateLog();
                operaterecord.OperateDT = DateTime.Now;
                operaterecord.Operator = auth.UserName;
                operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除仓库物料物料有效期记录,物料号为：" + list.FirstOrDefault().MaterialNumber + ",有效期为：" + list.FirstOrDefault().ValidityPeriod + "id为：" + id + ".";
                db.Warehouse_Material_ValidityPeriod.RemoveRange(list);
                db.UserOperateLog.Add(operaterecord);//保存删除日志
                int count = db.SaveChanges();
                if (count > 0)
                {
                    result.Add("Result", true);
                    result.Add("Message", "删除成功！");
                    result.Add("PostResult", comm.ReturnApiPostStatus());
                    return common.GetModuleFromJobjet(result);
                }
                else {
                    result.Add("Result", false);
                    result.Add("Message", "删除失败！");
                    result.Add("PostResult", comm.ReturnApiPostStatus());
                    return common.GetModuleFromJobjet(result);
                }
            }
            result.Add("Result", false);
            result.Add("Message", "传入参数为空！");
            result.Add("PostResult", comm.ReturnApiPostStatus());
            return common.GetModuleFromJobjet(result);
        }
        #endregion
        #endregion
    }

}
