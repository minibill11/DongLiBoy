﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JianHeMES.Controllers
{
    public class Customer_ComplaintsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();

        // GET: Customer_Complaints
        public ActionResult Index()
        {
            return View(db.Customer_Complaints.ToList());
        }

        #region--客诉订单表

        #region --------------------GetOrderNumList()检索订单号(用于创建保存)
        ///-------------------<GetOrderNumList_summary>
        /// 1.方法的作用：检索订单号
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到OrderMgm表里按照ID的排序顺序查询所有订单号并去重。
        /// 4.方法（可能）有结果：输出查询数据。
        ///-------------------</GetOrderNumListP_summary>
        public ActionResult GetOrderNumList()
        {
            var ordernum = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum).Distinct();//按照ID的排序顺序查询所有订单号并去重
            return Content(JsonConvert.SerializeObject(ordernum));
        }
        #endregion

        #region --------------------GetOrderNumList_Customer()检索订单号(用于查询)
        ///-------------------<GetOrderNumList_Customer_summary>
        /// 1.方法的作用：检索订单号
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到OrderMgm表里按照ID的排序顺序查询所有订单号并去重。
        /// 4.方法（可能）有结果：输出查询数据。
        ///-------------------</GetOrderNumList_Customer_summary>
        public ActionResult GetOrderNumList_Customer()
        {
            var ordernumList = db.Customer_Complaints.OrderByDescending(m => m.Id).Select(m => m.OrderNum).Distinct();//按照ID的排序顺序查询所有订单号并去重
            return Content(JsonConvert.SerializeObject(ordernumList));
        }
        #endregion

        #region---查询订单客诉处理表

        //查询页  参数：orderNum（订单号），complaintsDate（投诉日期），complaintNumber（客诉编号），productModel（产品型号），deliveryDate（发货日期）
        public ActionResult IndexCustomer(string orderNum, DateTime? complaintsDate, string complaintNumber, string productModel, DateTime? deliveryDate)
        {
            JObject cutomer = new JObject();
            JArray plaints = new JArray();
            var dataList = db.Customer_Complaints.ToList();
            if (!String.IsNullOrEmpty(orderNum))
            {
                dataList = dataList.Where(c => c.OrderNum == orderNum).ToList();
            }
            else if (complaintsDate != null)
            {
                dataList = dataList.Where(c => c.ComplaintsDate == complaintsDate).ToList();
            }
            else if (!String.IsNullOrEmpty(complaintNumber))
            {
                dataList = dataList.Where(c => c.ComplaintNumber == complaintNumber).ToList();
            }
            else if (!String.IsNullOrEmpty(productModel))
            {
                dataList = dataList.Where(c => c.ProductModel == productModel).ToList();
            }
            else if (deliveryDate != null)
            {
                dataList = dataList.Where(c => c.DeliveryDate == deliveryDate).ToList();
            }
            if (dataList.Count > 0)
            {
                foreach (var item in dataList)
                {
                    //id
                    cutomer.Add("Id", dataList.Count == 0 ? 0 : item.Id);
                    //客诉编号
                    cutomer.Add("ComplaintNumber", dataList.Count == 0 ? null : item.ComplaintNumber);
                    //客户名称
                    cutomer.Add("CustomerName", dataList.Count == 0 ? null : item.CustomerName);
                    //订单号
                    cutomer.Add("OrderNum", dataList.Count == 0 ? null : item.OrderNum);
                    //模组数量/面积
                    cutomer.Add("ModuleNumber", dataList.Count == 0 ? null : item.ModuleNumber);
                    //所需区域
                    cutomer.Add("RequiredArea", dataList.Count == 0 ? null : item.RequiredArea);
                    //产品型号
                    cutomer.Add("ProductModel", dataList.Count == 0 ? null : item.ProductModel);
                    //发货日期
                    cutomer.Add("DeliveryDate", dataList.Count == 0 ? null : item.DeliveryDate);
                    //投诉日期
                    cutomer.Add("ComplaintsDate", dataList.Count == 0 ? null : item.ComplaintsDate);
                    //屏体面积
                    cutomer.Add("ScreenArea", dataList.Count == 0 ? null : item.ScreenArea);
                    //控制系统及电源
                    cutomer.Add("Control", dataList.Count == 0 ? null : item.Control);
                    //投诉内容
                    cutomer.Add("Complaint_Content", dataList.Count == 0 ? null : item.Complaint_Content);
                    //异常描述
                    cutomer.Add("Abnormal_Describe", dataList.Count == 0 ? null : item.Abnormal_Describe);
                    //备注
                    cutomer.Add("Remark", dataList.Count == 0 ? null : item.Remark);
                    //结案日期
                    cutomer.Add("SettlementDate", dataList.Count == 0 ? null : item.SettlementDate);
                    plaints.Add(cutomer);
                    cutomer = new JObject();
                }
            }
            return Content(JsonConvert.SerializeObject(plaints));
        }

        //详细页 参数：orderNum（订单号），complaintsDate（投诉日期）
        public ActionResult QueryCustomer(int id)
        {
            JObject cutomer = new JObject();
            JArray table = new JArray();
            JObject complaints = new JObject();
            JArray plaints = new JArray();
            var dataList = db.Customer_Complaints.Where(c => c.Id == id).ToList();

            if (dataList.Count > 0)
            {
                foreach (var item in dataList)
                {
                    //id
                    cutomer.Add("Id", dataList.Count == 0 ? 0 : item.Id);
                    //客诉编号
                    cutomer.Add("ComplaintNumber", dataList.Count == 0 ? null : item.ComplaintNumber);
                    //客户名称
                    cutomer.Add("CustomerName", dataList.Count == 0 ? null : item.CustomerName);
                    //订单号
                    cutomer.Add("OrderNum", dataList.Count == 0 ? null : item.OrderNum);
                    //模组数量/面积
                    cutomer.Add("ModuleNumber", dataList.Count == 0 ? null : item.ModuleNumber);
                    //所需区域
                    cutomer.Add("RequiredArea", dataList.Count == 0 ? null : item.RequiredArea);
                    //产品型号
                    cutomer.Add("ProductModel", dataList.Count == 0 ? null : item.ProductModel);
                    //发货日期
                    cutomer.Add("DeliveryDate", dataList.Count == 0 ? null : item.DeliveryDate);
                    //投诉日期
                    cutomer.Add("ComplaintsDate", dataList.Count == 0 ? null : item.ComplaintsDate);
                    //屏体面积
                    cutomer.Add("ScreenArea", dataList.Count == 0 ? null : item.ScreenArea);
                    //控制系统及电源
                    cutomer.Add("Control", dataList.Count == 0 ? null : item.Control);
                    //投诉内容
                    cutomer.Add("Complaint_Content", dataList.Count == 0 ? null : item.Complaint_Content);
                    //异常描述
                    cutomer.Add("Abnormal_Describe", dataList.Count == 0 ? null : item.Abnormal_Describe);
                    //投诉代码     
                    var ik = db.Customer_ComplaintsCode.Where(c => c.OutsideKey == item.Id).Select(c => new { c.OutsideKey, c.Complaintscode, c.CodeNumber }).ToList();//根据主表ID找到投诉代码表的数据
                    foreach (var it in ik)
                    {
                        //投诉代码
                        complaints.Add("Complaintscode", ik.Count == 0 ? null : it.Complaintscode);
                        //数量
                        complaints.Add("CodeNumber", ik.Count == 0 ? 0 : it.CodeNumber);
                        table.Add(complaints);
                        complaints = new JObject();
                    }
                    cutomer.Add("Table", table);//投诉代码和数量
                    table = new JArray();
                    var code = ik.Where(c => c.OutsideKey == item.Id).Select(c => c.Complaintscode).Distinct().ToList();
                    //异常图片
                    List<FileInfo> fileInfos = null;
                    JArray picture_jpg = new JArray();
                    JArray picture_pdf = new JArray();
                    JArray picture = new JArray();
                    string compDate = string.Format("{0:yyyyMMdd}", item.ComplaintsDate);
                    foreach (var ime in code)
                    {
                        if (Directory.Exists(@"D:\MES_Data\Customer_Complaints\" + item.OrderNum + "\\" + compDate + "\\" + ime + "\\") == false)
                        {
                            picture.Add(false); //异常图片
                        }
                        else
                        {
                            fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\Customer_Complaints\" + item.OrderNum + "\\" + compDate + "\\" + ime + "\\");
                            foreach (var it in fileInfos)
                            {
                                string path = @"/MES_Data/Customer_Complaints/" + item.OrderNum + "/" + compDate + "/" + ime + "/" + it;//组合出路径
                                var filetype = path.Split('.');//将组合出来的路径以点分隔，方便下一步判断后缀
                                if (filetype[1] == "jpg")//后缀为jpg
                                {
                                    picture_jpg.Add(path);
                                }
                                else //后缀为其他
                                {
                                    picture_pdf.Add(path);
                                }
                            }
                        }
                    }
                    //异常图片
                    cutomer.Add("Picture", picture);
                    picture = new JArray();
                    cutomer.Add("Picture_jpg", picture_jpg);
                    picture_jpg = new JArray();
                    cutomer.Add("Picture_pdf", picture_pdf);
                    picture_pdf = new JArray();
                    //原因分析
                    cutomer.Add("Cause_Analysis", dataList.Count == 0 ? null : item.Cause_Analysis);
                    //临时处理措施
                    cutomer.Add("Interim_Disposal", dataList.Count == 0 ? null : item.Interim_Disposal);
                    //长期处理措施
                    cutomer.Add("Longterm_Treatment", dataList.Count == 0 ? null : item.Longterm_Treatment);
                    //责任归属
                    cutomer.Add("Liability", dataList.Count == 0 ? null : item.Liability);
                    //最终处理结果
                    cutomer.Add("Final_Result", dataList.Count == 0 ? null : item.Final_Result);
                    //备注
                    cutomer.Add("Remark", dataList.Count == 0 ? null : item.Remark);
                    //结案日期
                    cutomer.Add("SettlementDate", dataList.Count == 0 ? null : item.SettlementDate);
                    //责任部门
                    cutomer.Add("ResponDepartment", dataList.Count == 0 ? null : item.ResponDepartment);
                    //国内/外工程部
                    cutomer.Add("EngineDepartment", dataList.Count == 0 ? null : item.EngineDepartment);
                    //品质部
                    cutomer.Add("QualityDepartment", dataList.Count == 0 ? null : item.QualityDepartment);
                    //品技中心
                    cutomer.Add("Quality_TechDepartment", dataList.Count == 0 ? null : item.Quality_TechDepartment);
                    //运营副总
                    cutomer.Add("Operation", dataList.Count == 0 ? null : item.Operation);
                    //副总经理
                    cutomer.Add("VicePresident", dataList.Count == 0 ? null : item.VicePresident);
                    //总经理
                    cutomer.Add("General_Manager", dataList.Count == 0 ? null : item.General_Manager);
                    //创建人
                    cutomer.Add("Creator", dataList.Count == 0 ? null : item.Creator);
                    //创建时间
                    cutomer.Add("CreateDate", dataList.Count == 0 ? null : item.CreateDate);
                    //部门
                    cutomer.Add("Department", dataList.Count == 0 ? null : item.Department);
                    //班组
                    cutomer.Add("Group", dataList.Count == 0 ? null : item.Group);
                    plaints.Add(cutomer);
                    cutomer = new JObject();
                }
            }
            return Content(JsonConvert.SerializeObject(plaints));
        }

        #endregion

        #region---多张上传图片(订单客诉处理表)
        [HttpPost]
        //参数：pictureFile（多张图片），orderNum（订单号），complaintsDate（投诉日期），complaintscode（投诉代码）
        public bool UploadFile_Abnormal(List<string> pictureFile, string orderNum, DateTime complaintsDate, string complaintscode)
        {
            //string complaints = complaintsDate.ToString("yyyyMMdd");
            string complaints = string.Format("{0:yyyyMMdd}", complaintsDate);
            if (Request.Files.Count > 0)
            {
                if (Directory.Exists(@"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "\\" + complaintscode + "\\") == false)//判断路径是否存在
                {
                    Directory.CreateDirectory(@"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "\\" + complaintscode + "\\");//创建路径
                }
                foreach (var item in pictureFile)
                {
                    HttpPostedFileBase file = Request.Files["UploadFile_Abnormal" + pictureFile.IndexOf(item)];
                    var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();
                    List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "\\" + complaintscode + "\\");//遍历文件夹中的个数
                    if (fileType == ".jpg")//判断文件后缀
                    {
                        int jpg_count = filesInfo.Where(c => c.Name.StartsWith(complaintscode + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                        file.SaveAs(@"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "\\" + complaintscode + "\\" + complaintscode + "_" + (jpg_count + 1) + fileType);//文件追加命名
                    }
                    else if (fileType == ".pdf")
                    {
                        int pdf_count = filesInfo.Where(c => c.Name.StartsWith(complaintscode + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").Count();
                        file.SaveAs(@"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "\\" + complaintscode + "\\" + complaintscode + "_" + (pdf_count + 1) + fileType);//文件追加命名
                    }
                }
                return true;
            }
            return false;
        }
        #endregion

        #region ---创建保存(订单客诉处理表)
        //订单号、投诉日期、用户名部门、用户名班组和投诉代码、数量都不能为空
        public ActionResult ADDCoustomer(Customer_Complaints customer_Complaints)
        {
            JObject customer = new JObject();
            string plan = null;
            if (customer_Complaints != null && customer_Complaints.OrderNum != null && customer_Complaints.ComplaintsDate != null)
            {
                if (db.Customer_Complaints.Count(c => c.OrderNum == customer_Complaints.OrderNum && c.ComplaintsDate == customer_Complaints.ComplaintsDate) > 0)
                {
                    plan = plan + customer_Complaints.OrderNum + customer_Complaints.ComplaintsDate + ",";
                    customer.Add("Meg", false);
                    customer.Add("Plan", plan + "已有相同数据");
                    return Content(JsonConvert.SerializeObject(customer));
                }
                else //判断等于0时
                {
                    customer_Complaints.Creator = ((Users)Session["user"]).UserName;//添加创建人
                    customer_Complaints.CreateDate = DateTime.Now;//添加创建时间
                    db.Customer_Complaints.Add(customer_Complaints);//把数据保存到对应的表里
                    var savecount = db.SaveChanges();
                    if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                    {
                        customer.Add("Meg", true);
                        customer.Add("Customer", JsonConvert.SerializeObject(customer_Complaints));
                        return Content(JsonConvert.SerializeObject(customer));
                    }
                    else //savecount等于0（没有把数据保存到数据库或者保存出错）
                    {
                        customer.Add("Meg", false);
                        return Content(JsonConvert.SerializeObject(customer));
                    }
                }
            }
            customer.Add("Meg", false);
            return Content(JsonConvert.SerializeObject(customer));
        }

        #endregion

        #region---修改数据（订单客诉处理表）

        //修改主表
        public ActionResult ModifyProcessing(Customer_Complaints complaints)
        {
            JObject Change = new JObject();
            if (complaints != null && complaints.OrderNum != null && complaints.ComplaintsDate != null && complaints.ResponDepartment == null)//判断订单号和投诉日期是否为空
            {
                complaints.Modifier = ((Users)Session["user"]).UserName;//添加修改人
                complaints.ModifyTime = DateTime.Now;//添加修改时间
                db.Entry(complaints).State = EntityState.Modified;//修改数据
                var savecount = db.SaveChanges();//保存数据库
                if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                {
                    Change.Add("Repairbill", true);
                    Change.Add("Change", JsonConvert.SerializeObject(complaints));
                    return Content(JsonConvert.SerializeObject(Change));
                }
                else //savecount等于0（没有把数据保存到数据库或者保存出错）
                {
                    Change.Add("Repairbill", false);
                    return Content(JsonConvert.SerializeObject(Change));
                }
            }
            Change.Add("Repairbill", false);
            return Content(JsonConvert.SerializeObject(Change));
        }

        //修改投诉代码或者增加投诉代码 参数:customer数组；orderNum订单号，complaintsDate投诉日期，responDepartment责任部门
        public ActionResult EditComplaintcode(List<Customer_ComplaintsCode> customer, int id, string responDepartment)
        {
            JObject table = new JObject();
            if (customer != null && id != 0 && responDepartment == null)//判断前端传进来的数据是否为空（customer数组，订单号，投诉日期）
            {
                foreach (var item in customer)
                {
                    if (db.Customer_ComplaintsCode.Count(c => c.OutsideKey == id && c.Complaintscode == item.Complaintscode) > 0)
                    {
                        table.Add("Mes", false);
                        table.Add("Change", "投诉代码相同");
                        return Content(JsonConvert.SerializeObject(table));
                    }
                    else
                    {
                        //根据订单号和投诉日期找对应的ID
                        var code = db.Customer_Complaints.Where(c => c.Id == id).Select(c => c.Id).FirstOrDefault();
                        item.OutsideKey = code;//把找出来的ID赋值给outsideKey
                        db.SaveChanges();//保存
                    }
                }
                db.Customer_ComplaintsCode.AddRange(customer);//把数据保存到相对应的表里
                int savecount = db.SaveChanges();//保存到数据库
                if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                {
                    table.Add("Mes", true);
                    table.Add("Change", JsonConvert.SerializeObject(customer));
                    return Content(JsonConvert.SerializeObject(table));
                }
                else
                {
                    //var code = db.Customer_Complaints.Where(c => c.Id == id).Select(c => c.Id).FirstOrDefault();
                    var record = db.Customer_Complaints.Where(c => c.Id == id).FirstOrDefault();//根据ID查询数据
                    db.Customer_Complaints.Remove(record);//删除对应的数据
                    db.SaveChanges();
                    table.Add("Mes", false);
                    table.Add("Change", "保存失败");
                    return Content(JsonConvert.SerializeObject(table));
                }
            }
            table.Add("Mes", false);
            table.Add("Change", "数据不完整");
            return Content(JsonConvert.SerializeObject(table));
        }

        #endregion

        #region---处理意见会签栏
        //id；responDepartment（责任部门确认），engineDepartment（国内/外工程部确认），qualityDepartment（品质部确认）,techDepartment（品技中心确认），operation（运营副总确认），vicePresident（副总经理确认），general_Manager（总经理确认）
        public ActionResult Signature(int id, string responDepartment, string engineDepartment, string qualityDepartment, string techDepartment, string operation, string vicePresident, string general_Manager)
        {
            JObject sign = new JObject();
            //根据ID找到相对应的数据
            var depae = db.Customer_Complaints.Where(c => c.Id == id && c.General_Manager == null).ToList();
            string table = null;
            if (depae.Count > 0)
            {
                foreach (var item in depae)
                {
                    if (!String.IsNullOrEmpty(responDepartment))//判断责任部门确认是否为空
                    {
                        item.ResponDepartment = responDepartment;//把数据保存到对应的字段里
                        item.ResponDate = DateTime.Now;//获取当前时间并保存到对应的字段里
                        db.SaveChanges();//保存到数据库
                        table = responDepartment;//把前端传进来的数据赋值到table
                    }
                    else if (!String.IsNullOrEmpty(engineDepartment))//判断国内/外工程部确认是否为空
                    {
                        item.EngineDepartment = engineDepartment;//把数据保存到对应的字段里
                        item.EngineDate = DateTime.Now;//获取当前时间并保存到对应的字段里
                        db.SaveChanges();//保存到数据库
                        table = engineDepartment;//把前端传进来的数据赋值到table
                    }
                    else if (!String.IsNullOrEmpty(qualityDepartment))//判断品质部确认是否为空
                    {
                        item.QualityDepartment = qualityDepartment;//把数据保存到对应的字段里
                        item.QualityDate = DateTime.Now;//获取当前时间并保存到对应的字段里
                        db.SaveChanges();//保存到数据库
                        table = qualityDepartment;//把前端传进来的数据赋值到table
                    }
                    else if (!String.IsNullOrEmpty(techDepartment))//判断品技中心确认是否为空
                    {
                        item.Quality_TechDepartment = techDepartment;//把数据保存到对应的字段里
                        item.QualityTechDate = DateTime.Now;//获取当前时间并保存到对应的字段里
                        db.SaveChanges();//保存到数据库
                        table = techDepartment;//把前端传进来的数据赋值到table
                    }
                    else if (!String.IsNullOrEmpty(operation))//判断运营副总确认是否为空
                    {
                        item.Operation = operation;//把数据保存到对应的字段里
                        item.OperationDate = DateTime.Now;//获取当前时间并保存到对应的字段里
                        db.SaveChanges();//保存到数据库
                        table = operation;//把前端传进来的数据赋值到table
                    }
                    else if (!String.IsNullOrEmpty(vicePresident))//判断副总经理确认是否为空
                    {
                        item.VicePresident = vicePresident;//把数据保存到对应的字段里
                        item.VicePresDate = DateTime.Now;//获取当前时间并保存到对应的字段里
                        db.SaveChanges();//保存到数据库
                        table = vicePresident;//把前端传进来的数据赋值到table
                    }
                    else if (!String.IsNullOrEmpty(general_Manager))//判断总经理确认是否为空
                    {
                        item.General_Manager = general_Manager;//把数据保存到对应的字段里
                        item.GeneralDate = DateTime.Now;//获取当前时间并保存到对应的字段里
                        db.SaveChanges();//保存到数据库
                        table = general_Manager;//把前端传进来的数据赋值到table
                    }
                }
                sign.Add("sign", true);
                sign.Add("table", table);
                return Content(JsonConvert.SerializeObject(sign));
            }
            sign.Add("sign", false);
            return Content(JsonConvert.SerializeObject(sign));
        }

        #endregion

        #region---删除客诉订单表和删除图片
        //删除数据
        public ActionResult DeleteCustomer(int id)
        {
            var record = db.Customer_Complaints.Where(c => c.Id == id).FirstOrDefault();
            var codelist = db.Customer_ComplaintsCode.Where(c => c.OutsideKey == id).ToList();
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;//添加删除操作时间
            operaterecord.Operator = ((Users)Session["User"]).UserName;//添加删除操作人
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除" + record.Creator + "创建的客诉订单表记录";
            db.Customer_Complaints.Remove(record);//删除对应的数据
            db.Customer_ComplaintsCode.RemoveRange(codelist);
            db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
            var recordList = db.Customer_Complaints.Where(c => c.Id == id).ToList();
            foreach (var item in recordList)
            {
                List<FileInfo> fileInfos = null;
                string compDate = string.Format("{0:yyyyMMdd}", item.ComplaintsDate);
                var code = codelist.Where(c => c.OutsideKey == id).Select(c => c.Complaintscode).Distinct().ToList();
                foreach (var ite in code)
                {
                    if (Directory.Exists(@"D:\MES_Data\Customer_Complaints\" + item.OrderNum + "\\" + compDate + "\\" + ite + "\\") == false)
                    {
                        int count1 = db.SaveChanges();//保存到数据库
                        if (count1 > 0)//判断count是否大于0（有没有把数据保存到数据库）
                            return Content("true");
                        else //等于0（没有把数据保存到数据库或者保存出错）
                            return Content("false");
                    }
                    else
                    {
                        fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\Customer_Complaints\" + item.OrderNum + "\\" + compDate + "\\" + ite + "\\");
                        foreach (var it in fileInfos)
                        {
                            System.IO.File.Delete(it.FullName);//删除图片
                            Console.WriteLine(it.FullName);
                        }
                    }
                }
                foreach (var ime in code)
                {
                    if (Directory.Exists(@"D:\MES_Data\Customer_Complaints\" + item.OrderNum + "\\" + compDate + "\\" + ime + "\\") == true)
                    {
                        string file_path = @"D:\MES_Data\Customer_Complaints\" + item.OrderNum + "\\" + compDate + "\\" + ime + "\\";//获取文件夹（以订单号/投诉日期命名）
                        Directory.Delete(file_path);//删除文件夹
                    }
                }
                if (Directory.Exists(@"D:\MES_Data\Customer_Complaints\" + item.OrderNum + "\\" + compDate + "\\") == true)
                {
                    string file_path = @"D:\MES_Data\Customer_Complaints\" + item.OrderNum + "\\" + compDate + "\\";//获取文件夹（以订单号命名）
                    Directory.Delete(file_path);//删除文件夹
                }
                if (Directory.Exists(@"D:\MES_Data\Customer_Complaints\" + item.OrderNum + "\\") == true)
                {
                    string file_path = @"D:\MES_Data\Customer_Complaints\" + item.OrderNum + "\\";//获取文件夹（以订单号命名）
                    Directory.Delete(file_path);//删除文件夹
                }
            }
            int count = db.SaveChanges();//保存到数据库
            if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
                return Content("true");
            else //等于0（没有把数据保存到数据库或者保存出错）
                return Content("false");
        }

        //删除图片path(路径)，orderNum（订单号），complaintsDate（投诉日期），complaintscode（投诉代码）
        public ActionResult DeleteFile(string path, string orderNum, DateTime complaintsDate, string complaintscode)
        {
            if (!String.IsNullOrEmpty(path) && !String.IsNullOrEmpty(orderNum) && complaintsDate != null && !String.IsNullOrEmpty(complaintscode))
            {
                string complaints = string.Format("{0:yyyyMMdd}", complaintsDate);

                string fileType = path.Substring(path.Length - 4, 4);//扩展名
                string Oldpath = @"D:" + path.Replace('/', '\\');//整个文件路径
                FileInfo pathFile = new FileInfo(Oldpath);//建立文件对象
                int code_N = Convert.ToInt16(pathFile.Name.Split('_')[1].Split('.')[0]);//文件序号
                string Newpath = @"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "_Delete_File\\";//新目录路径
                if (Directory.Exists(@"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "_Delete_File\\") == false)
                {
                    Directory.CreateDirectory(@"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "_Delete_File\\");
                    FileInfo Newfile = new FileInfo(Newpath + complaintscode + "_1" + fileType);
                    pathFile.CopyTo(Newfile.FullName);
                    pathFile.Delete();
                    List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "\\" + complaintscode + "\\");
                    int filecount = fileInfos.Where(c => c.Extension == fileType).Count();
                    for (int i = code_N; i < filecount + 1; i++)

                    {
                        string file_path = @"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "\\" + complaintscode + "\\" + complaintscode + "_" + (i + 1) + fileType;
                        string newfile_path = @"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "\\" + complaintscode + "\\" + complaintscode + "_" + i + fileType;
                        System.IO.File.Move(file_path, newfile_path);
                    }
                }
                else
                {
                    List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "_Delete_File\\");
                    int count = fileInfos.Where(c => c.Extension == fileType).Count();
                    FileInfo new_file = new FileInfo(Newpath + complaintscode + "_" + (count + 1) + fileType);
                    pathFile.CopyTo(new_file.FullName);
                    pathFile.Delete();
                    List<FileInfo> codefileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "\\" + complaintscode + "\\");
                    int filecount = codefileInfos.Where(c => c.Extension == fileType).Count();
                    for (int i = code_N; i < filecount + 1; i++)
                    {
                        string file_path = @"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "\\" + complaintscode + "\\" + complaintscode + "_" + (i + 1) + fileType;
                        string newfile_path = @"D:\MES_Data\Customer_Complaints\" + orderNum + "\\" + complaints + "\\" + complaintscode + "\\" + complaintscode + "_" + i + fileType;
                        System.IO.File.Move(file_path, newfile_path);
                    }
                }
                return Content("true");
            }
            return Content("false");
        }

        #endregion

        #region---删除（取消）投诉代码
        public ActionResult DeleteCode(int id, string code)
        {
            JObject table = new JObject();
            var complaints = db.Customer_ComplaintsCode.Where(c => c.OutsideKey == id && c.Complaintscode == code).FirstOrDefault();
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;//添加删除操作时间
            operaterecord.Operator = ((Users)Session["User"]).UserName;//添加删除操作人
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除客诉订单记录表ID名为" + id + "的投诉代码" + code;
            db.Customer_ComplaintsCode.Remove(complaints);//删除对应的数据
            db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
            int count = db.SaveChanges();//保存到数据库
            if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
            {
                table.Add("Mes", true);
                table.Add("Change", "取消投诉代码成功");
                return Content(JsonConvert.SerializeObject(table));
            }
            else //等于0（没有把数据保存到数据库或者保存出错）
            {
                table.Add("Mes", false);
                table.Add("Change", "取消投诉代码失败");
                return Content(JsonConvert.SerializeObject(table));
            }
        }
        #endregion

        #region---修改（添加）投诉代码数量
        public ActionResult ModifyCodeNumber(int id, string code, int newcodeNumber)
        {
            JObject table = new JObject();
            if (id != 0 && code != null)
            {
                var codenumber = db.Customer_ComplaintsCode.Where(c => c.OutsideKey == id && c.Complaintscode == code).ToList();
                foreach (var item in codenumber)
                {
                    item.CodeNumber = newcodeNumber;
                    db.SaveChanges();
                }
                table.Add("Mes", true);
                table.Add("Change", "修改成功");
                table.Add("Code", newcodeNumber);
                return Content(JsonConvert.SerializeObject(table));
            }
            table.Add("Mes", false);
            table.Add("Change", "修改失败");
            return Content(JsonConvert.SerializeObject(table));
        }
        #endregion

        #endregion

        #region--客诉损失明细表

        public ActionResult Customer_loss()//查询首页
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Customer_Complaints", act = "Customer_loss" });
            }
            return View();
        }

        public ActionResult BatchAddCustomer()//批量上传首页
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "Customer_Complaints", act = "BatchAddCustomer" });
            }
            return View();
        }

        #region--- 查询客诉损失明细表
        public ActionResult Query_AttachmentLoss(int? year, int? month)
        {
            JObject retul = new JObject();
            JArray attach = new JArray();
            JObject code = new JObject();
            JArray table = new JArray();
            JObject total = new JObject();
            int year_e = (int)year;
            int month_e = (int)month;
            var Attachment = db.Customer_AttachmentLoss.ToList();
            List<Customer_AttachmentLoss> losses = new List<Customer_AttachmentLoss>();
            decimal Huizhou = 0;//惠州工厂
            decimal Management = 0;//订单管理部
            decimal Purchasing = 0;//采购部
            decimal Research = 0;//研发中心
            decimal Engineering = 0;//工程售后部
            decimal Risk = 0;//风控中心
            if (year != 0 && month != 0)
            {
                losses = Attachment.Where(c => c.Year == year_e && c.Month == month_e).ToList();
            }
            else if (year != 0 && month == 0)
            {
                losses = Attachment.Where(c => c.Year == year_e).ToList();
            }
            if (losses.Count > 0)
            {
                foreach (var item in losses)
                {
                    //ID
                    retul.Add("Id", item.Id == 0 ? 0 : item.Id);
                    //客户名称
                    retul.Add("CustomerName", item.CustomerName == null ? null : item.CustomerName);
                    //出货日期
                    var deliveryDate = string.Format("{0:yyyy-MM-dd}", item.DeliveryDate);
                    retul.Add("DeliveryDate", deliveryDate == null ? null : deliveryDate);
                    //投诉日期
                    var complaintsDate = string.Format("{0:yyyy-MM-dd}", item.ComplaintsDate);
                    retul.Add("ComplaintsDate", complaintsDate == null ? null : complaintsDate);
                    //结案日期
                    var settlementDate = string.Format("{0:yyyy-MM-dd}", item.SettlementDate);
                    retul.Add("SettlementDate", settlementDate == null ? null : settlementDate);
                    //产品型号
                    retul.Add("ProductModel", item.ProductModel == null ? null : item.ProductModel);
                    //订单号
                    retul.Add("OrderNum", item.OrderNum == null ? null : item.OrderNum);
                    //客诉现象
                    retul.Add("Customer_phenomenon", item.Customer_phenomenon == null ? null : item.Customer_phenomenon);
                    //原因分析
                    retul.Add("Cause_Analysis", item.Cause_Analysis == null ? null : item.Cause_Analysis);
                    //临时处理措施
                    retul.Add("Interim_Disposal", item.Interim_Disposal == null ? null : item.Interim_Disposal);
                    //长期处理措施
                    retul.Add("Longterm_Treatment", item.Longterm_Treatment == null ? null : item.Longterm_Treatment);
                    //索赔情况
                    retul.Add("ClaimIs", item.ClaimIs == null ? null : item.ClaimIs);
                    //索赔确认
                    retul.Add("ClaimConfirm", item.ClaimConfirm == null ? null : item.ClaimConfirm);
                    //合同金额（元）
                    retul.Add("Contract_Amount", item.Contract_Amount == 0 ? 0 : item.Contract_Amount);
                    //损失金额（元）
                    retul.Add("Losses_Amount", item.Losses_Amount == 0 ? 0 : item.Losses_Amount);
                    if (item.LossRate == 0)
                    {
                        double lossrate = (double)(item.Losses_Amount / item.Contract_Amount) * 100;
                        //损失率
                        retul.Add("LossRate", lossrate == 0 ? 0 : lossrate);
                    }
                    else
                    {
                        //损失率
                        retul.Add("LossRate", item.LossRate == 0 ? 0 : item.LossRate);
                    }
                    //质量损失（金额/元）
                    var codelist = db.Customer_Attachment_QualityLoss.Where(c => c.OutsideKey == item.Id).Select(c => new { c.OutsideKey, c.QualityLoss, c.Responsibility }).ToList();
                    foreach (var it in codelist)
                    {
                        code.Add("QualityLoss", it.QualityLoss == 0 ? 0 : it.QualityLoss); //质量损失（金额/元）
                        code.Add("Responsibility", it.Responsibility == null ? null : it.Responsibility);//责任判定
                        table.Add(code);
                        code = new JObject();
                    }
                    var huizhou = codelist.Where(c => c.Responsibility == "惠州工厂").Select(c => c.QualityLoss).FirstOrDefault();
                    Huizhou = Huizhou + huizhou;
                    var guanlibu = codelist.Where(c => c.Responsibility == "订单管理部").Select(c => c.QualityLoss).FirstOrDefault();
                    Management = Management + guanlibu;
                    var caigoubu = codelist.Where(c => c.Responsibility == "采购部").Select(c => c.QualityLoss).FirstOrDefault();
                    Purchasing = Purchasing + caigoubu;
                    var yanfa = codelist.Where(c => c.Responsibility == "研发中心").Select(c => c.QualityLoss).FirstOrDefault();
                    Research = Research + yanfa;
                    var gongcheng = codelist.Where(c => c.Responsibility == "工程售后部").Select(c => c.QualityLoss).FirstOrDefault();
                    Engineering = Engineering + gongcheng;
                    var fengkong = codelist.Where(c => c.Responsibility == "风控中心").Select(c => c.QualityLoss).FirstOrDefault();
                    Risk = Risk + fengkong;
                    retul.Add("Table", table);//质量损失和责任判定
                    table = new JArray();
                    //客损归属-年
                    retul.Add("Year", item.Year == 0 ? 0 : item.Year);
                    //客损归属-月
                    retul.Add("Month", item.Month == 0 ? 0 : item.Month);
                    //备注
                    retul.Add("Remark", item.Remark == null ? null : item.Remark);
                    //审核人
                    retul.Add("Audit", item.Audit == null ? null : item.Audit);
                    //审核时间
                    retul.Add("AuditDate", item.AuditDate == null ? null : item.AuditDate);
                    attach.Add(retul);
                    retul = new JObject();
            
                }
                //惠州工厂总计
                total.Add("Huizhou", Huizhou);
                //订单管理部总计
                total.Add("Management", Management);
                //采购部总计
                total.Add("Purchasing", Purchasing);
                //研发中心总计
                total.Add("Research", Research);
                //工程售后部总计
                total.Add("Engineering", Engineering);
                //风控中心总计
                total.Add("Risk", Risk);
                //具体数据
                total.Add("Attach", attach);
            }
            return Content(JsonConvert.SerializeObject(total));
        }

        #endregion

        #region---添加数据
        [HttpPost]
        //批量增加
        public ActionResult BatchUpload(List<Customer_AttachmentLoss> attachment, int year, int month)
        {
            JObject loss = new JObject();
            JArray copy = new JArray();
            string rept = null;
            if (attachment.Count > 0 && year != 0 && month != 0)
            {
                foreach (var item in attachment)
                {
                    item.CreateDate = DateTime.Now;
                    item.Creator = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                    if (db.Customer_AttachmentLoss.Count(c => c.Year == year && c.Month == month && c.OrderNum == item.OrderNum && c.ComplaintsDate == item.ComplaintsDate) > 0)
                    {
                        rept = rept + ";" + year + "-" + month + "," + item.OrderNum + "," + item.ComplaintsDate + "," + "重复了";
                        copy.Add(rept);
                    }
                }
                if (rept != null)
                {
                    loss.Add("meg", false);
                    loss.Add("feg", copy);
                    return Content(JsonConvert.SerializeObject(loss));
                }

                foreach (var it in attachment)
                {
                    it.Year = year;
                    it.Month = month;
                    db.SaveChanges();
                }
                db.Customer_AttachmentLoss.AddRange(attachment);
                int count = db.SaveChanges();
                if (count > 0)
                {
                    loss.Add("meg", true);
                    loss.Add("feg", "保存成功！");
                    loss.Add("loss", JsonConvert.SerializeObject(attachment));
                    return Content(JsonConvert.SerializeObject(loss));
                }
                else
                {
                    loss.Add("meg", false);
                    loss.Add("feg", "保存出错！");
                    return Content(JsonConvert.SerializeObject(loss));
                }
            }
            loss.Add("meg", false);
            loss.Add("feg", "数据错误/年月为空");
            return Content(JsonConvert.SerializeObject(loss));
        }

        //单个添加数据
        public ActionResult ADDLoss(Customer_AttachmentLoss customerList, int year, int month, List<Customer_Attachment_QualityLoss> lossesList)
        {
            JObject table = new JObject();
            if (customerList != null && customerList.OrderNum != null && customerList.ComplaintsDate != null && year != 0 && month != 0 && lossesList != null)
            {
                if (db.Customer_AttachmentLoss.Count(c => c.OrderNum == customerList.OrderNum && c.ComplaintsDate == customerList.ComplaintsDate && c.Year == year && c.Month == month) > 0)
                {
                    table.Add("meg", false);
                    table.Add("feg", customerList.Year + "-" + customerList.Month + "," + customerList.OrderNum + "订单" + "," + "投诉日期" + customerList.ComplaintsDate + "重复了");
                    return Content(JsonConvert.SerializeObject(table));
                }
                else
                {
                    customerList.CreateDate = DateTime.Now;
                    customerList.Creator = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                    customerList.Year = year;
                    customerList.Month = month;
                    db.Customer_AttachmentLoss.Add(customerList);
                    int countt = db.SaveChanges();
                    if (countt > 0)
                    {
                        foreach (var item in lossesList)
                        {
                            var code = db.Customer_AttachmentLoss.Where(c => c.OrderNum == customerList.OrderNum && c.Year == year && c.Month == month).Select(c => c.Id).FirstOrDefault();
                            item.OutsideKey = code;
                            db.SaveChanges();
                        }
                        db.Customer_Attachment_QualityLoss.AddRange(lossesList);//把数据保存到相对应的表里
                        int savecount = db.SaveChanges();
                        if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                        {
                            table.Add("meg", true);
                            table.Add("feg", "保存成功！");
                            return Content(JsonConvert.SerializeObject(table));
                        }
                        else //savecount等于0（没有把数据保存到数据库或者保存出错）
                        {
                            var code = db.Customer_AttachmentLoss.Where(c => c.OrderNum == customerList.OrderNum && c.ComplaintsDate == customerList.ComplaintsDate && c.Year == year && c.Month == month).FirstOrDefault();
                            db.Customer_AttachmentLoss.Remove(code);//删除对应的数据
                            db.SaveChanges();//保存到数据库
                            table.Add("meg", false);
                            table.Add("feg", "保存出错！");
                            return Content(JsonConvert.SerializeObject(table));
                        }
                    }
                    else
                    {
                        table.Add("meg", false);
                        table.Add("feg", "保存出错！");
                        return Content(JsonConvert.SerializeObject(table));
                    }
                }
            }
            table.Add("meg", false);
            table.Add("feg", "数据出错！");
            return Content(JsonConvert.SerializeObject(table));
        }

        #endregion

        #region---修改数据

        //修改方法(修改主表)1
        public ActionResult ModifyGuestLoss(Customer_AttachmentLoss customer, List<Customer_Attachment_QualityLoss> qualityLoss)
        {
            JObject guest = new JObject();
            JArray table = new JArray();
            int count = 0;
            int count1 = 0;
            if (customer != null && customer.Year != 0 && customer.Month != 0 && qualityLoss.Count != 0)
            {
                customer.Modifier = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";//添加修改人
                customer.ModifyTime = DateTime.Now;//添加修改时间
                db.Entry(customer).State = EntityState.Modified;//修改数据
                count = db.SaveChanges();
            }
            if (qualityLoss.Count > 0)
            {
                foreach (var ite in qualityLoss)
                {
                    var code = db.Customer_Attachment_QualityLoss.Where(c => c.OutsideKey == customer.Id && c.Responsibility == ite.Responsibility).Select(c => c.QualityLoss).FirstOrDefault();
                    if (code != 0)
                    {
                        code = ite.QualityLoss;
                        count1 = db.SaveChanges();
                    }
                    else
                    {
                        ite.OutsideKey = customer.Id;
                        ite.QualityLoss = ite.QualityLoss;
                        ite.Responsibility = ite.Responsibility;
                        db.Customer_Attachment_QualityLoss.AddRange(qualityLoss);
                        count1 = db.SaveChanges();
                    }
                }
            }
            if (count > 0&&count1==0)
            {
                guest.Add("meg", true);
                guest.Add("feg", "修改成功！");
                guest.Add("guest", JsonConvert.SerializeObject(customer));
                guest.Add("quality", JsonConvert.SerializeObject(qualityLoss));
                return Content(JsonConvert.SerializeObject(guest));
            }
            if (count > 0 && count1 > 0)
            {
                guest.Add("meg", true);
                guest.Add("feg", "修改成功！");
                guest.Add("guest", JsonConvert.SerializeObject(customer));
                guest.Add("quality", JsonConvert.SerializeObject(qualityLoss));
                return Content(JsonConvert.SerializeObject(guest));
            }
            else
            {
                guest.Add("meg", false);
                guest.Add("feg", "修改失败！");
                return Content(JsonConvert.SerializeObject(guest));
            }
        }

        //修改方法（修改质量损失/责任判定里的金额）
        public ActionResult ModifyQualityLoss(int id, decimal qualityLoss, string responsibility)
        {
            JObject retul = new JObject();
            int count = 0;
            var code = db.Customer_Attachment_QualityLoss.Where(c => c.OutsideKey == id && c.Responsibility == responsibility).ToList();
            if (code != null)
            {
                foreach (var item in code)
                {
                    item.QualityLoss = qualityLoss;
                    count = db.SaveChanges();
                }
                if (count > 0)
                {
                    retul.Add("meg", true);
                    retul.Add("feg", "修改成功！");
                    retul.Add("qualityLoss", qualityLoss);
                    return Content(JsonConvert.SerializeObject(retul));
                }
                else
                {
                    retul.Add("meg", false);
                    retul.Add("feg", "修改失败！");
                    return Content(JsonConvert.SerializeObject(retul));
                }
            }
            retul.Add("meg", false);
            retul.Add("feg", "数据错误！");
            return Content(JsonConvert.SerializeObject(retul));
        }

        #endregion

        #endregion

        #region---其他

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer_Complaints customer_Complaints = db.Customer_Complaints.Find(id);
            if (customer_Complaints == null)
            {
                return HttpNotFound();
            }
            return View(customer_Complaints);
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ComplaintNumber,CustomerName,OrderNum,ModuleNumber,RequiredArea,ProductModel,DeliveryDate,ComplaintsDate,ScreenArea,Control,Complaint_Content,Abnormal_Describe,Cause_Analysis,Interim_Disposal,Longterm_Treatment,Liability,Final_Result,Remark,SettlementDate,ResponDepartment,ResponDate,EngineDepartment,EngineDate,QualityDepartment,QualityDate,Quality_TechDepartment,QualityTechDate,Operation,OperationDate,VicePresident,VicePresDate,General_Manager,GeneralDate,Creator,CreateDate,Modifier,ModifyTime")] Customer_Complaints customer_Complaints)
        {
            if (ModelState.IsValid)
            {
                db.Customer_Complaints.Add(customer_Complaints);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer_Complaints);
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer_Complaints customer_Complaints = db.Customer_Complaints.Find(id);
            if (customer_Complaints == null)
            {
                return HttpNotFound();
            }
            return View(customer_Complaints);
        }

        [HttpPost]
        public ActionResult EditCode(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<Customer_ComplaintsCode> customer_ComplaintsCodelist = db.Customer_ComplaintsCode.Where(c => c.OutsideKey == id).ToList();
            return Content(JsonConvert.SerializeObject(customer_ComplaintsCodelist));
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ComplaintNumber,CustomerName,OrderNum,ModuleNumber,RequiredArea,ProductModel,DeliveryDate,ComplaintsDate,ScreenArea,Control,Complaint_Content,Abnormal_Describe,Cause_Analysis,Interim_Disposal,Longterm_Treatment,Liability,Final_Result,Remark,SettlementDate,ResponDepartment,ResponDate,EngineDepartment,EngineDate,QualityDepartment,QualityDate,Quality_TechDepartment,QualityTechDate,Operation,OperationDate,VicePresident,VicePresDate,General_Manager,GeneralDate,Creator,CreateDate,Modifier,ModifyTime")] Customer_Complaints customer_Complaints)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer_Complaints).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer_Complaints);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer_Complaints customer_Complaints = db.Customer_Complaints.Find(id);
            if (customer_Complaints == null)
            {
                return HttpNotFound();
            }
            return View(customer_Complaints);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer_Complaints customer_Complaints = db.Customer_Complaints.Find(id);
            db.Customer_Complaints.Remove(customer_Complaints);
            db.SaveChanges();
            return RedirectToAction("Index");
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
