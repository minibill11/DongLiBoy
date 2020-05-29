﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.Data.Entity.Infrastructure;
using System.Text;
using JianHeMES.Areas.KongYaHT.Models;
using System.Threading;

namespace JianHeMES.Controllers
{
    public class EquipmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private kongyadbEntities db_KongYa = new kongyadbEntities();
        private CommonController meg = new CommonController();

        // GET: Equipment
        public ActionResult Index()//设备台账
        {
            return View();
        }

        public ActionResult Index2()//产线查看
        {
            return View();
        }

        public ActionResult First_equipment()//设备首页
        {
            return View();
        }

        public ActionResult Departmental_usage()//部门设备使用率首页
        {
            return View();
        }

        #region------Index查询首页------
        [HttpPost]
        public ActionResult Index(string equipmentnumber, string assetnumber, string equipmentname, string brand, string modelspecification, string userdepartment, string storageplace, string workshop, string section, string status, string remark = "")
        {

            IEnumerable<EquipmentBasicInfo> ebi = db.EquipmentBasicInfo;
            List<Exception> exprList = new List<Exception>();
            ParameterExpression paramExpr = Expression.Parameter(typeof(EquipmentStatusRecord), "o");
            MethodInfo containsMethod = typeof(string).GetMethod("Contains");//获取表示

            var resultlist = ebi;
            if (!String.IsNullOrEmpty(equipmentnumber))
            {
                resultlist = resultlist.Where(c => c.EquipmentNumber != null && c.EquipmentNumber.Contains(equipmentnumber));
            }
            if (!String.IsNullOrEmpty(assetnumber))
            {
                resultlist = resultlist.Where(c => c.AssetNumber != null && c.AssetNumber.Contains(assetnumber));
            }
            if (!String.IsNullOrEmpty(equipmentname))
            {
                resultlist = resultlist.Where(c => c.EquipmentName != null && c.EquipmentName.Contains(equipmentname));
            }
            if (!String.IsNullOrEmpty(brand))
            {
                resultlist = resultlist.Where(c => c.Brand != null && c.Brand.Contains(brand));
            }
            if (!String.IsNullOrEmpty(modelspecification))
            {
                resultlist = resultlist.Where(c => c.ModelSpecification != null && c.ModelSpecification.Contains(modelspecification));
            }
            if (!String.IsNullOrEmpty(userdepartment))
            {
                resultlist = resultlist.Where(c => c.UserDepartment != null && c.UserDepartment.Contains(userdepartment));
            }
            if (!String.IsNullOrEmpty(storageplace))
            {
                resultlist = resultlist.Where(c => c.StoragePlace != null && c.StoragePlace.Contains(storageplace));
            }
            if (!String.IsNullOrEmpty(workshop))
            {
                resultlist = resultlist.Where(c => c.WorkShop != null && c.WorkShop.Contains(workshop));
            }
            if (!String.IsNullOrEmpty(section))
            {
                resultlist = resultlist.Where(c => c.Section != null && c.Section.Contains(section));
            }
            if (!String.IsNullOrEmpty(status))
            {
                resultlist = resultlist.Where(c => c.Status != null && c.Status.Contains(status));
            }
            if (!String.IsNullOrEmpty(remark))
            {
                resultlist = resultlist.Where(c => c.Remark != null && c.Remark.Contains(remark));
            }
            JObject result = new JObject();
            int i = 1;
            foreach (var item in resultlist)
            {
                result.Add(i.ToString(), JsonConvert.SerializeObject(item));
                i++;
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        #region------获取设备的使用部门列表
        ///<summary>
        /// 1.方法的作用：获取设备的使用部门列表
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到EquipmentBasicInfo表里按照ID的排序顺序查询所有使用部门并去重。
        /// 4.方法（可能）有结果：输出查询数据；输出null。
        /// </summary>
        public ActionResult Userdepartment_list()
        {
            var depar_list = db.EquipmentBasicInfo.OrderByDescending(m => m.Id).Select(c => c.UserDepartment).Distinct();
            return Content(JsonConvert.SerializeObject(depar_list));
        }
        #endregion

        #region------修改设备使用部门
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：修改设备使用部门。
        /// 2.方法的参数和用法：ID，使用部门（Userdepartment），用法：属于查询条件和要修改的数据。
        /// 3.方法的具体逻辑顺序，判断条件：先判断前端是否有数据传送过来（id，newdepartment），然后根据前端传送过来的字段数据查找唯一的数据，后修改需要修改相对应的数据。
        /// 4.方法（可能）有结果：前端传送字段为空，修改失败。前端传送字段不为空，并找到对应的数据，修改成功。
        /// </summary>  
        public async Task<bool> ModifyEquipmentUseDepartment(int id, string newdepartment)
        {
            if (!String.IsNullOrEmpty(id.ToString()) && !String.IsNullOrEmpty(newdepartment))//判断ID，使用部门是否为空
            {
                var record = await db.EquipmentBasicInfo.Where(c => c.Id == id).FirstOrDefaultAsync();//根据ID查找数据
                record.UserDepartment = newdepartment;//修改保存使用部门数据
                await db.SaveChangesAsync();//保存到数据库
                return true;
            }
            return false;
        }
        #endregion

        #region------批量添加设备------
        public ActionResult BatchInputEquipment()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Equipment", act = "BatchInputEquipment" });
            }
            return View();
        }

        [HttpPost]
        ///<summary>
        /// 1.方法的作用：批量添加设备。
        /// 2.方法的参数和用法：List<EquipmentBasicInfo> inputList，用法：循环inputList（多条数据）并保存。
        /// 3.方法的具体逻辑顺序，判断条件：循环inputList并且判断它里面的数据是否有跟数据库已存在的数据相同（相同条件：设备编号），
        /// 然后判断repeat（存储有相同的数据的字段）是否为空，不为空，最后Add（inputList）保存到数据库。
        /// 4.方法（可能）有结果：有相同的数据存在,保存失败。没有相同的数据存在，保存成功。
        /// </summary> 
        public async Task<ActionResult> BatchInputEquipment(List<EquipmentBasicInfo> inputList)
        {
            string repeat = null;
            foreach (var item in inputList)//循环
            {
                item.CreateTime = DateTime.Now;
                item.Creator = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                if (db.EquipmentBasicInfo.Count(c => c.EquipmentNumber == item.EquipmentNumber) != 0)//根据设备编号到EquipmentBasicInfo表里查询数据，并判断数据是否大于0
                    repeat = repeat + item.EquipmentNumber + ",";
            }
            JObject result = new JObject();
            if (!string.IsNullOrEmpty(repeat))//判断repeat是否为空
            {
                result.Add("repeat", repeat);

                return Content(JsonConvert.SerializeObject(result));
            }
            db.EquipmentBasicInfo.AddRange(inputList);//add inputList
            int savecount = await db.SaveChangesAsync();
            if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                result.Add("success", "添加" + inputList.Count.ToString() + "台设备成功");
            else //savecount等于0（没有把数据保存到数据库或者保存出错）
                result.Add("failure", "添加失败");
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region------添加/修改维修履历汇总记录------
        [HttpPost]
        public async Task<bool> AddEquipmentStatusRecordAsync(EquipmentStatusRecord record)
        {
            record.Creator = ((Users)Session["user"]).UserName;
            record.CreateTime = DateTime.Now;
            db.EquipmentStatusRecord.Add(record);
            if (record.StatusStarTime < DateTime.Now.AddMinutes(10) && record.StatusEndTime > DateTime.Now)
            {
                var eqm = db.EquipmentBasicInfo.Where(c => c.EquipmentNumber == record.EquipmentNumber).FirstOrDefault();
                eqm.Status = record.Status;
            }
            int result = await db.SaveChangesAsync();
            if (result > 0) return true;// Content("添加成功!");
            else return false;// Content("添加失败!");
        }

        [HttpPost]
        public async Task<ActionResult> GetEquipmentStatusRecordAsyn(int id)
        {
            var record = await db.EquipmentStatusRecord.Where(c => c.Id == id).FirstOrDefaultAsync();
            return Content(JsonConvert.SerializeObject(record)); // Content("修改成功!");
        }

        [HttpPost]
        public ActionResult EditEquipmentStatusRecordAsync(EquipmentStatusRecord modifyrecord)
        {
            if (modifyrecord.Id != 0 && modifyrecord != null)
            {
                modifyrecord.ModifyTime = DateTime.Now;//添加修改时间
                modifyrecord.Modifier = ((Users)Session["user"]).UserName;//添加修改人
                db.Entry(modifyrecord).State = EntityState.Modified;//修改对应的数据数据
                db.SaveChanges();
                return Content("true"); // Content("修改成功!");
            }
            return Content("false");// Content("修改失败!");
        }

        //批量添加维修履历汇总记录Batch
        public ActionResult BatchAdd_Rrecord(List<EquipmentStatusRecord> records, string equipmentNumber, string assetNumber, string equipmentName, string userDepsrtment, string status)
        {
            JObject result = new JObject();
            if (records.Count > 0 && userDepsrtment != null)
            {
                string repeat = null;
                foreach (var item in records)//循环
                {
                    item.CreateTime = DateTime.Now;
                    item.Creator = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                    if (db.EquipmentStatusRecord.Count(c => c.EquipmentNumber == equipmentNumber && c.GetJobTime == item.GetJobTime && c.FailureDescription == item.FailureDescription) != 0)//根据设备编号和维修接单时间到EquipmentStatusRecord表里查询数据，并判断数据是否大于0
                        repeat = repeat + item.EquipmentNumber + item.GetJobTime + item.FailureDescription + ',';
                }
                if (!string.IsNullOrEmpty(repeat))//判断repeat是否为空
                {
                    result.Add("mes", false);
                    result.Add("repeat", repeat + "重复");
                    return Content(JsonConvert.SerializeObject(result));
                }
                foreach (var ite in records)
                {
                    ite.EquipmentNumber = equipmentNumber;
                    ite.AssetNumber = assetNumber;
                    ite.EquipmentName = equipmentName;
                    ite.UserDepartment = userDepsrtment;
                    ite.Status = status;
                    db.SaveChanges();
                }
                db.EquipmentStatusRecord.AddRange(records);//add inputList
                int savecount = db.SaveChanges();
                if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                {
                    result.Add("mes", true);
                    result.Add("repeat", "添加" + records.Count.ToString() + "记录成功");
                    return Content(JsonConvert.SerializeObject(result));
                }
                else //savecount等于0（没有把数据保存到数据库或者保存出错）
                {
                    result.Add("mes", false);
                    result.Add("repeat", "添加失败");
                    return Content(JsonConvert.SerializeObject(result));
                }
            }
            result.Add("mes", false);
            result.Add("repeat", "使用部门为空！/数据为空");
            return Content(JsonConvert.SerializeObject(result));
        }

        #endregion

        #region------同时修改三个表的设备状态（Status）
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：修改设备状态（同时修改三个相关联的数据表）。
        /// 2.方法的参数和用法：List<string> equipmentNumber（设备编号可以多传数据），status（设备状态）linenum（线别），userdepart（使用部门），用法：设备编号属于查询条件，其他字段可用于修改数据。
        /// 3.方法的具体逻辑顺序，判断条件：先判断设备编号和设备状态是否为空，其次循环设备编号并根据设备编号去EquipmentSetStation表、EquipmentBasicInfo表，EquipmentStatusRecord表里找相对应的数据，
        /// 然后判断找出来的数据是否为空，不为空就修改数据（修改设备状态）和添加修改人修改时间。最后就是判断EquipmentStatusRecord表是否已经有该设备状态的状态数据，
        /// 如果有就把设备状态的结束时间添加上并新建一条新数据，如果没有就直接添加数据（主要有：设备编号，资产编号，设备名称，设备状态，该设备状态的开始时间）。
        /// 4.方法（可能）有结果：第一个判断条件为空就修改失败，判断条件不为空就修改成功。
        /// </summary> 
        public ActionResult Equipment_state(List<string> equipmentNumber, string status, string linenum, string userdepar)
        {
            JObject message = new JObject();
            if (equipmentNumber.Count() > 0 && !String.IsNullOrEmpty((status)))//判断设备编号和设备状态是否为空
            {
                foreach (var item in equipmentNumber)//循环设备编号
                {
                    var station = db.EquipmentSetStation.Where(c => c.EquipmentNumber == item).FirstOrDefault();//根据设备编号查询数据
                    if (station != null)//判断查询出来的数据（station）是否为空
                    {
                        station.Status = status;
                        station.ModifyTime = DateTime.Now;
                        station.Modifier = ((Users)Session["user"]).UserName;
                        db.SaveChanges();
                    }
                    var infoe = db.EquipmentBasicInfo.Where(c => c.EquipmentNumber == item).FirstOrDefault();//根据设备编号查询数据
                    if (infoe != null)//判断查询出来的数据（infoe）是否为空
                    {
                        infoe.Status = status;
                        infoe.ModifyTime = DateTime.Now;
                        infoe.Modifier = ((Users)Session["user"]).UserName;
                        db.SaveChanges();
                    }
                    var record = db.EquipmentStateTime.OrderByDescending(c => c.StatusStarTime).Where(c => c.EquipmentNumber == item).FirstOrDefault();//按照设备状态的开始时间并根据设备编号查询数据
                    if (record != null && record.Status != status)//判断查询出来的数据（record）是否为空，查找出来的数据里的设备状态不能等于前端传送过来的设备状态
                    {
                        record.StatusEndTime = DateTime.Now;//添加状态结束时间
                        db.SaveChanges();//保存到数据库
                        var add = record;
                        add.Status = status;//修改保存设备状态
                        add.StatusStarTime = record.StatusEndTime;//保存状态时间
                        add.StatusEndTime = null;
                        add.CreateTime = DateTime.Now;//添加新建时间
                        add.Creator = ((Users)Session["user"]).UserName;//添加创建人
                        db.EquipmentStateTime.Add(add);//把数据保存到对应的表
                        db.SaveChanges();
                    }
                    else if (record == null && infoe != null)//判断record等于null，infoe不能等于null
                    {
                        //在EquipmentStateTime表里添加数据（数据有：设备编号，资产编号，设备名称，设备状态，状态的开始时间等）
                        var rede = new EquipmentStateTime() { EquipmentNumber = infoe.EquipmentNumber, AssetNumber = infoe.AssetNumber, EquipmentName = infoe.EquipmentName, Status = status, StatusStarTime = DateTime.Now, StatusEndTime = null, UserDepartment = userdepar, LineNum = linenum, Creator = ((Users)Session["user"]).UserName, CreateTime = DateTime.Now };
                        db.EquipmentStateTime.Add(rede);
                        db.SaveChanges();
                    }
                }
                message.Add("msg", "修改" + equipmentNumber.Count.ToString() + "台设备状态成功！");
                message.Add("result", true);
                message.Add("Status", status);
                return Content(JsonConvert.SerializeObject(message));
            }
            message.Add("msg", "修改设备状态失败！");
            message.Add("result", false);
            return Content(JsonConvert.SerializeObject(message));
        }

        #endregion

        #region ------添加设备维修保养记录（自用）------
        public ActionResult InputEquipmentRepairRecord()
        {
            //if (Session["User"] == null)
            //{
            //    return RedirectToAction("Login", "Users", new { col = "Equipment", act = "InputEquipmentRepairRecord" });
            //}
            return View();
        }

        [HttpPost]
        public ActionResult InputEquipmentRepairRecord(List<EquipmentStatusRecord> equipmentStatusRecordList)
        {
            if (ModelState.IsValid)
            {
                db.EquipmentStatusRecord.AddRange(equipmentStatusRecordList);
                db.SaveChangesAsync();
                return Content("保存成功!");
            }
            return Content("保存失败!");
        }
        public ActionResult InputEquipmentRepairRecordSingle()
        {
            //if (Session["User"] == null)
            //{
            //    return RedirectToAction("Login", "Users", new { col = "Equipment", act = "InputEquipmentRepairRecord" });
            //}
            return View();
        }

        [HttpPost]
        public ActionResult InputEquipmentRepairRecordSingle(EquipmentStatusRecord equipmentStatusRecord)
        {
            if (ModelState.IsValid)
            {
                db.EquipmentStatusRecord.Add(equipmentStatusRecord);
                db.SaveChangesAsync();
                return Content("保存成功!");
            }
            return Content("保存失败!");
        }

        #endregion

        #region------设备信息详情页------
        public ActionResult Details()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Details(string equipmentNumber)
        {
            if (equipmentNumber == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JObject result = new JObject();
            var basicinfo = await db.EquipmentBasicInfo.Where(c => c.EquipmentNumber == equipmentNumber).FirstOrDefaultAsync();
            result.Add("basicinfo", JsonConvert.SerializeObject(basicinfo));
            List<FileInfo> fileInfos = new List<FileInfo>();
            if (Directory.Exists(@"D:\MES_Data\Equipment\" + equipmentNumber + "\\") == false)
            {
                result.Add("picture", "未上传图片。");
            }
            else
            {
                fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\Equipment\" + equipmentNumber + "\\").Where(c => c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
                result.Add("picture", JsonConvert.SerializeObject(fileInfos));
            }
            var statusrecord = await db.EquipmentStatusRecord.Where(c => c.EquipmentNumber == equipmentNumber).OrderBy(c => c.GetJobTime).ToListAsync();
            result.Add("statusrecord", JsonConvert.SerializeObject(statusrecord));
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion、

        #region-----修改设备信息EquipmentBasicInfo表-----
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：修改设备信息EquipmentBasicInfo表（详情页的信息）。
        /// 2.方法的参数和用法：equipmentNumber（设备编号），assetNumber（资产编号），equipmentName（设备名称），userdepartment（使用部门）等，用法：设备编号属于查询条件，其他字段可用于修改数据。
        /// 3.方法的具体逻辑顺序，判断条件：（1）.根据设备编号到EquipmentBasicInfo表里查询相对应的数据。（2）.判断查询出来的数据是否为空，不为空时进入下一个子判断，如果子判断(判断资产编号、使用部门、设备名称等)为空，进入下一个子判断一直判断完为止。
        /// （3）.子判断不为空时就进入下一步，修改数据（修改资产编号、使用部门、设备名称等）和添加修改人修改时间。
        /// 4.方法（可能）有结果：判断条件为空就修改失败（数据有误），判断条件不为空就修改成功。
        /// </summary> 
        public ActionResult Edit_Equipmentbasic(string equipmentNumber, string assetNumber, string equipmentName, string userdepartment, string modelspeci, string manufacturingNumber, string brand, string supplier, DateTime? purchaseDate, DateTime? actionDate)
        {
            JObject messlist = new JObject();
            var recordlist = db.EquipmentBasicInfo.Where(c => c.EquipmentNumber == equipmentNumber).FirstOrDefault();//根据设备编号到EquipmentBasicInfo表里查询相对应的数据
            if (recordlist != null)//判断查询出来的数据是否为空
            {
                if (!String.IsNullOrEmpty(assetNumber))//判断前端传送过来的资产编号是否为空
                {
                    recordlist.Modifier = ((Users)Session["User"]).UserName;//添加修改人
                    recordlist.ModifyTime = DateTime.Now;//添加修改时间
                    recordlist.AssetNumber = assetNumber;//修改保存资产编号数据
                    db.SaveChanges();//保存到数据库
                    messlist.Add("messlist", "修改成功！");
                    messlist.Add("assetNumber", assetNumber);
                    return Content(JsonConvert.SerializeObject(messlist));
                }
                else if (!String.IsNullOrEmpty(equipmentName))//判断前端传送过来的设备名称是否为空（如果前一个判断为空，就会运行下一个判断）
                {
                    recordlist.Modifier = ((Users)Session["User"]).UserName;//添加修改人
                    recordlist.ModifyTime = DateTime.Now;//添加修改时间
                    recordlist.EquipmentName = equipmentName;//修改保存设备名称数据
                    db.SaveChanges();//保存到数据库
                    messlist.Add("messlist", "修改成功！");
                    messlist.Add("equipmentName", equipmentName);
                    return Content(JsonConvert.SerializeObject(messlist));
                }
                else if (!String.IsNullOrEmpty(userdepartment))//判断前端传送过来的使用部门是否为空（如果前一个判断为空，就会运行下一个判断）
                {
                    recordlist.Modifier = ((Users)Session["User"]).UserName;//添加修改人
                    recordlist.ModifyTime = DateTime.Now;//添加修改时间
                    recordlist.UserDepartment = userdepartment;//修改保存使用部门数据
                    db.SaveChanges();//保存到数据库
                    messlist.Add("messlist", "修改成功！");
                    messlist.Add("userdepartment", userdepartment);
                    return Content(JsonConvert.SerializeObject(messlist));
                }
                else if (!String.IsNullOrEmpty(modelspeci))//判断前端传送过来的型号/规格是否为空（如果前一个判断为空，就会运行下一个判断）
                {
                    recordlist.Modifier = ((Users)Session["User"]).UserName;//添加修改人
                    recordlist.ModifyTime = DateTime.Now;//添加修改时间
                    recordlist.ModelSpecification = modelspeci;//修改保存型号/规格数据
                    db.SaveChanges();//保存到数据库
                    messlist.Add("messlist", "修改成功！");
                    messlist.Add("modelspeci", modelspeci);
                    return Content(JsonConvert.SerializeObject(messlist));
                }
                else if (!String.IsNullOrEmpty(manufacturingNumber))//判断前端传送过来的出厂编号是否为空（如果前一个判断为空，就会运行下一个判断）
                {
                    recordlist.Modifier = ((Users)Session["User"]).UserName;//添加修改人
                    recordlist.ModifyTime = DateTime.Now;//添加修改时间
                    recordlist.ManufacturingNumber = manufacturingNumber;//修改保存出厂编号数据
                    db.SaveChanges();//保存到数据库
                    messlist.Add("messlist", "修改成功！");
                    messlist.Add("manufacturingNumber", manufacturingNumber);
                    return Content(JsonConvert.SerializeObject(messlist));
                }
                else if (!String.IsNullOrEmpty(brand))//判断前端传送过来的品牌（生产厂家）是否为空（如果前一个判断为空，就会运行下一个判断）
                {
                    recordlist.Modifier = ((Users)Session["User"]).UserName;//添加修改人
                    recordlist.ModifyTime = DateTime.Now;//添加修改时间
                    recordlist.Brand = brand;//修改保存品牌数据
                    db.SaveChanges();//保存到数据库
                    messlist.Add("messlist", "修改成功！");
                    messlist.Add("brand", brand);
                    return Content(JsonConvert.SerializeObject(messlist));
                }
                else if (!String.IsNullOrEmpty(supplier))//判断前端传送过来的供应商是否为空（如果前一个判断为空，就会运行下一个判断）
                {
                    recordlist.Modifier = ((Users)Session["User"]).UserName;//添加修改人
                    recordlist.ModifyTime = DateTime.Now;//添加修改时间
                    recordlist.Supplier = supplier;//修改保存供应商数据
                    db.SaveChanges();//保存到数据库
                    messlist.Add("messlist", "修改成功！");
                    messlist.Add("supplier", supplier);
                    return Content(JsonConvert.SerializeObject(messlist));
                }
                else if (purchaseDate != null)//判断前端传送过来的购入日期是否为空（如果前一个判断为空，就会运行下一个判断）
                {
                    recordlist.Modifier = ((Users)Session["User"]).UserName;//添加修改人
                    recordlist.ModifyTime = DateTime.Now;//添加修改时间
                    recordlist.PurchaseDate = purchaseDate;//修改保存购入日期数据
                    db.SaveChanges();//保存到数据库
                    messlist.Add("messlist", "修改成功！");
                    messlist.Add("purchaseDate", purchaseDate);
                    return Content(JsonConvert.SerializeObject(messlist));
                }
                else if (actionDate != null)//判断前端传送过来的启用时间是否为空（如果前一个判断为空，就会运行下一个判断）
                {
                    recordlist.Modifier = ((Users)Session["User"]).UserName;//添加修改人
                    recordlist.ModifyTime = DateTime.Now;//添加修改时间
                    recordlist.ActionDate = actionDate;//修改保存启用时间
                    db.SaveChanges();//保存到数据库
                    messlist.Add("messlist", "修改成功！");
                    messlist.Add("actionDate", actionDate);
                    return Content(JsonConvert.SerializeObject(messlist));
                }
            }
            return Content("数据有误！");
        }
        #endregion

        #region------上传设备照片(jpg)方法------
        [HttpPost]
        public ActionResult UploadEquipmentPicture(string equipmentNumber)
        {
            foreach (var file1 in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[file1.ToString()];
                var fileType = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                if (!String.Equals(fileType, ".jpg"))
                {
                    return Content("您选择文件的文件类型不正确，请选择jpg类型图片文件！");
                }
                if (Directory.Exists(@"D:\MES_Data\Equipment\" + equipmentNumber + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\Equipment\" + equipmentNumber + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\Equipment\" + equipmentNumber + "\\");
                //文件为jpg类型
                if (fileType == ".jpg")
                {
                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(equipmentNumber) && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                    file.SaveAs(@"D:\MES_Data\Equipment\" + equipmentNumber + "\\" + equipmentNumber + (jpg_count + 1) + fileType);
                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\Equipment\" + equipmentNumber + "\\" + equipmentNumber + fileType);
                }
                //return RedirectToAction("Details", "Equipment", new { equipmentNumber = equipmentNumber });
            }
            if (Request.Files.Count > 0)
            {
                return Content("上传成功！");
            }
            return Content("上传失败！");
        }
        #endregion  

        #region------获取所有设备编号的方法
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：获取所有设备编号的方法
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到EquipmentBasicInfo表里查询设备编号并去重
        /// 4.方法（可能）有结果：如果EquipmentBasicInfo表没有数据，查询数据就为空，EquipmentBasicInfo表里有数据，输出EquipmentBasicInfo表里去重过的所有设备编号
        /// </summary> 
        public ActionResult EQNumberList()
        {
            var eqNumberlist = db.EquipmentBasicInfo.Select(c => c.EquipmentNumber).Distinct();
            return Content(JsonConvert.SerializeObject(eqNumberlist));
        }
        #endregion

        #region------根据设备编号获取设备信息
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：获取设备详细信息
        /// 2.方法的参数和用法：equipmentNumber（设备编号）
        /// 3.方法的具体逻辑顺序，判断条件：根据设备编号到EquipmentBasicInfo表里查询跟设备编号相对应的所有数据（唯一的一条数据）
        /// 4.方法（可能）有结果：查询数据不为空，就输出该设备编号的所有数据，查询数据为空，输出null
        /// </summary> 
        public ActionResult EquipmentInfo_getdata_by_eqnum(string equipmentNumber)
        {
            return Content(JsonConvert.SerializeObject(db.EquipmentBasicInfo.Where(c => c.EquipmentNumber == equipmentNumber).FirstOrDefault()));
        }
        #endregion

        #region----根据使用部门获取设备编号
        public ActionResult Number_List(string userdepartment)
        {
            var equilist = db.EquipmentBasicInfo.Where(c => c.UserDepartment == userdepartment).Select(c => c.EquipmentNumber).Distinct();
            return Content(JsonConvert.SerializeObject(equilist));
        }
        #endregion

        #region-----设备状态使用率（停机、运行、维修）
        ///<summary>
        /// 1.方法的作用：
        /// 2.方法的参数和用法：
        /// 3.方法的具体逻辑顺序，判断条件：
        /// 4.方法（可能）有结果：
        /// </summary> 
        public ActionResult Equipment_Timeusage(List<string> equipmentNumber, string userDepartment, int? year, int? month)
        {
            JObject statu = new JObject();
            JArray usage = new JArray();
            JArray timeList = new JArray();
            double houses = 0;
            double halt = 0;//停机时间
            double rate_halt = 0;//停机的有效率
            double run = 0;//运行时间
            double rate_run = 0;//运行的有效率
            double main = 0;//维修时间
            double rate_main = 0;//维修的有效率
            double yuemo = 0;
            double yu = 0;
            double Thours = 0;
            double exce = 0;
            DateTime time = DateTime.Now;//获取当前时间
            List<EquipmentStateTime> dateList = new List<EquipmentStateTime>();
            if (equipmentNumber.Count > 0)//判断设备编号是否大于0
            {
                foreach (var item in equipmentNumber)//循环设备编号
                {
                    if (userDepartment != null && year != null && month != null)//判断使用部门和年月是否为空
                    {
                        //根据设备编号、使用部门、年月查找对应的数据
                        var p = db.EquipmentStateTime.Where(c => c.EquipmentNumber == item && c.UserDepartment == userDepartment && c.StatusStarTime.Value.Year == year && c.StatusStarTime.Value.Month <= month && c.StatusEndTime == null).ToList();
                        var k = db.EquipmentStateTime.Where(c => c.EquipmentNumber == item && c.UserDepartment == userDepartment && c.StatusStarTime.Value.Year == year && c.StatusStarTime.Value.Month <= month && c.StatusEndTime.Value.Month >= month).ToList();
                        dateList = p.Concat(k).ToList();
                        if (time.Year == year && time.Month == month)
                        {
                            houses = time.Day * 24;//月份的天数*24 
                        }
                        else
                        {
                            houses = DateTime.DaysInMonth(year.Value, month.Value) * 24;//月份的天数*24 
                        }
                    }
                    else if (userDepartment != null && year != null && month == null)//判断使用部门和年是否为空，判断月是否等于空
                    {
                        //根据设备编号、使用部门和年查找相对应的数据
                        dateList = db.EquipmentStateTime.Where(c => c.EquipmentNumber == item && c.UserDepartment == userDepartment && c.StatusStarTime.Value.Year == year).ToList();
                        if (time.Year == year)
                        {
                            houses = time.DayOfYear * 24;//月份的天数*24 
                        }
                        else
                        {
                            houses = (DateTime.IsLeapYear(year.Value) == true ? 366 : 365) * 24;//年份的天数*24
                        }
                    }
                    else if (year != null && month == null)//判断年是否为空，判断月是否等于空
                    {
                        //根据设备编号和年查询相对应的数据
                        dateList = db.EquipmentStateTime.Where(c => c.EquipmentNumber == item && c.StatusStarTime.Value.Year == year).ToList();
                        if (time.Year == year)
                        {
                            houses = time.DayOfYear * 24;//月份的天数*24 
                        }
                        else
                        {
                            houses = (DateTime.IsLeapYear(year.Value) == true ? 366 : 365) * 24;//年份的天数*24
                        }
                    }
                    else if (year != null && month != null)//判断年月是否为空
                    {
                        //根据设备编号和年月查询相对应的数据
                        var p = db.EquipmentStateTime.Where(c => c.EquipmentNumber == item && c.StatusStarTime.Value.Year == year && c.StatusStarTime.Value.Month <= month && c.StatusEndTime == null).ToList();
                        var k = db.EquipmentStateTime.Where(c => c.EquipmentNumber == item && c.StatusStarTime.Value.Year == year && c.StatusStarTime.Value.Month <= month && c.StatusEndTime.Value.Month >= month).ToList();
                        dateList = p.Concat(k).ToList();
                        if (time.Year == year && time.Month == month)
                        {
                            houses = time.Day * 24;//月份的天数*24 
                        }
                        else
                        {
                            houses = DateTime.DaysInMonth(year.Value, month.Value) * 24;//月份的天数*24 
                        }
                    }
                    if (dateList.Count > 0)//判断查询出来的数据是否大于0
                    {
                        foreach (var ite in dateList)//循环timelist
                        {
                            DateTime? dt1 = ite.StatusStarTime;//把DateTime?类型转换成DateTime类型(开始时间)
                            DateTime? dt2 = ite.StatusEndTime;//把DateTime?类型转换成DateTime类型（结束时间）
                            DateTime at1 = dateList.Where(c => c.StatusStarTime == ite.StatusStarTime).Min(c => c.StatusStarTime).Value;//最小的记录
                            if (dt2 == null)
                            {
                                yuemo = time.Hour - at1.Hour;
                            }
                            else
                            {
                                DateTime at2 = dateList.Where(c => c.StatusEndTime == ite.StatusEndTime).Max(c => c.StatusEndTime).Value;//最大的记录
                                yuemo = at2.Hour - at1.Hour;//月末减去月初的时间                               
                            }
                            yu = yu + yuemo;//把月末减去月初的时间转换成小时
                            DateTime begintime = default(DateTime);
                            if (year != null && month != null)
                            {
                                begintime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1, 0, 0, 0);
                            }
                            else if (year != null && month == null)
                            {
                                begintime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(1), 1, 0, 0, 0);
                            }
                            if (ite.Status == "停机")//设备状态等于“停机”
                            {
                                if (dt1 < begintime)
                                {
                                    if (ite.StatusEndTime == null)//设备状态的结束时间为空
                                    {
                                        Thours = (time - begintime).TotalHours;
                                    }
                                    else
                                    {
                                        Thours = (Convert.ToDateTime(dt2) - begintime).TotalHours;
                                    }
                                    halt = halt + Thours;//时间汇总
                                    exce = (yu <= 0 ? 0 : Thours / yu) / houses * 100;//使用总时长除以月末减去月初的时间在除以当月/年天数的时长     
                                }
                                else
                                {
                                    if (ite.StatusEndTime == null)
                                    {
                                        Thours = (time - Convert.ToDateTime(dt1)).TotalHours;
                                    }
                                    else
                                    {
                                        Thours = (Convert.ToDateTime(dt2) - Convert.ToDateTime(dt1)).TotalHours;
                                    }
                                    halt = halt + Thours;//时间汇总
                                    exce = (yu <= 0 ? 0 : Thours / yu) / houses * 100;//使用总时长除以月末减去月初的时间在除以当月/年天数的时长     
                                }                        
                                rate_halt = rate_halt + exce;//使用率汇总
                            }
                            else if (ite.Status == "运行")//判断设备状态是否等于“运行”
                            {
                                if (dt1 < begintime)
                                {
                                    if (ite.StatusEndTime == null)//设备状态的结束时间为空
                                    {
                                        Thours = (time - begintime).TotalHours;
                                    }
                                    else
                                    {
                                        Thours = (Convert.ToDateTime(dt2) - begintime).TotalHours;
                                    }
                                    run = run + Thours;//时间汇总
                                    exce = (yu <= 0 ? 0 : Thours / yu) / houses * 100;//使用总时长除以月末减去月初的时间在除以当月/年天数的时长  
                                }
                                else
                                {
                                    if (ite.StatusEndTime == null)
                                    {
                                        Thours = (time - Convert.ToDateTime(dt1)).TotalHours;
                                    }
                                    else
                                    {
                                        Thours = (Convert.ToDateTime(dt2) - Convert.ToDateTime(dt1)).TotalHours;
                                    }
                                    run = run + Thours;//时间汇总
                                    exce = (yu <= 0 ? 0 : Thours / yu) / houses * 100;//使用总时长除以月末减去月初的时间在除以当月/年天数的时长  
                                }                         
                                rate_run = rate_run + exce;//使用率汇总
                            }
                            else if (ite.Status == "维修")//判断设备状态是否等于“维修”
                            {
                                if (dt1 < begintime)
                                {
                                    if (ite.StatusEndTime == null)//设备状态的结束时间为空
                                    {
                                        Thours = (time - begintime).TotalHours;
                                    }
                                    else
                                    {
                                        Thours = (Convert.ToDateTime(dt2) - begintime).TotalHours;
                                    }
                                    main = main + Thours;//时间汇总
                                    exce = (yu <= 0 ? 0 : Thours / yu) / houses * 100;//使用总时长除以月末减去月初的时间在除以当月/年天数的时长  
                                }
                                else
                                {
                                    if (ite.StatusEndTime == null)
                                    {
                                        Thours = (time - Convert.ToDateTime(dt1)).TotalHours;
                                    }
                                    else
                                    {
                                        Thours = (Convert.ToDateTime(dt2) - Convert.ToDateTime(dt1)).TotalHours;
                                    }
                                    main = main + Thours;//时间汇总
                                    exce = (yu <= 0 ? 0 : Thours / yu) / houses * 100;//使用总时长除以月末减去月初的时间在除以当月/年天数的时长  
                                }                            
                                rate_main = rate_main + exce;//使用率汇总
                            }                            
                        }
                        //赋值为数组对象
                        JObject equi = new JObject();
                        equi.Add("name", "停机时间");
                        equi.Add("value", double.Parse(halt.ToString("0.00")) + "小时");
                        halt = new double();
                        timeList.Add(equi);
                        equi = new JObject();

                        equi.Add("name", "停机使用率");
                        equi.Add("value", double.Parse(rate_halt.ToString("0.00")) + "%");
                        rate_halt = new double();
                        timeList.Add(equi);
                        equi = new JObject();

                        equi.Add("name", "运行时间");
                        equi.Add("value", double.Parse(run.ToString("0.00")) + "小时");
                        run = new double();
                        timeList.Add(equi);
                        equi = new JObject();

                        equi.Add("name", "运行使用率");
                        equi.Add("value", double.Parse(rate_run.ToString("0.00")) + "%");
                        rate_run = new double();
                        timeList.Add(equi);
                        equi = new JObject();

                        equi.Add("name", "维修时间");
                        equi.Add("value", double.Parse(main.ToString("0.00")) + "小时");
                        main = new double();
                        timeList.Add(equi);
                        equi = new JObject();

                        equi.Add("name", "维修使用率");
                        equi.Add("value", double.Parse(rate_main.ToString("0.00")) + "%");
                        rate_main = new double();
                        timeList.Add(equi);
                        equi = new JObject();
                    }
                    statu.Add("Userdeparment", userDepartment);
                    statu.Add("EquipmentNumber", item);
                    var code = dateList.Where(c => c.EquipmentNumber == item).Select(c => c.EquipmentName).FirstOrDefault();
                    statu.Add("EquipmentName", code);
                    statu.Add("Year", year);
                    statu.Add("Month", month);
                    statu.Add("timeList", timeList);
                    timeList = new JArray();
                    usage.Add(statu);
                    statu = new JObject();
                }
            }
            return Content(JsonConvert.SerializeObject(usage));
        }

        #endregion

        #endregion

        #region------Index2产线查看------
        [HttpPost]
        public ActionResult Index2(List<string> departmentlist)
        {
            JArray result = new JArray();
            if (departmentlist != null)
            {
                foreach (var department in departmentlist)
                {
                    var linenumlist = db.EquipmentSetStation.Where(c => c.UserDepartment == department).Select(c => c.LineNum).Distinct().ToList();
                    JObject JOdepartmentDatas = new JObject();
                    JArray JOdepartment = new JArray();
                    foreach (var linenum in linenumlist)
                    {
                        JObject JOlineData = new JObject();
                        JOlineData.Add("linenum", linenum);
                        var equipmentlistbystationnum = db.EquipmentSetStation.Where(c => c.UserDepartment == department && c.LineNum == linenum).OrderBy(c => c.StationNum).ToList();
                        int eqcount = equipmentlistbystationnum.Count;
                        int stop_eqcount = equipmentlistbystationnum.Count(c => c.Status == "停机");
                        int start_eqcount = equipmentlistbystationnum.Count(c => c.Status == "运行");
                        int repair_eqcount = equipmentlistbystationnum.Count(c => c.Status == "维修" || c.Status == "保养");
                        if (eqcount == start_eqcount)
                        {
                            JOlineData.Add("productLineStatus", "运行");
                        }
                        else if (eqcount == stop_eqcount)
                        {
                            JOlineData.Add("productLineStatus", "停机");
                        }
                        else if (repair_eqcount > 0)
                        {
                            JOlineData.Add("productLineStatus", "维修/保养");
                        }
                        else
                        {
                            JOlineData.Add("productLineStatus", "未知状态");
                        }
                        JArray Jmechines = new JArray();
                        foreach (var mechine in equipmentlistbystationnum)
                        {
                            JObject JOmechine = new JObject();
                            JOmechine.Add("mechineindex", mechine.StationNum);
                            JOmechine.Add("equipmentNumber", mechine.EquipmentNumber);
                            JOmechine.Add("equipmentName", mechine.EquipmentName);
                            JOmechine.Add("status", mechine.Status);
                            Jmechines.Add(JOmechine);
                        }
                        JOlineData.Add("mechines", Jmechines);
                        JOdepartment.Add(JOlineData);
                    }
                    JOdepartmentDatas.Add(department, JOdepartment);
                    result.Add(JOdepartmentDatas);
                }
                return Content(JsonConvert.SerializeObject(result));
            }
            return Content("没有部门名！");
        }

        //修改设备基本信息方法
        [HttpPost]
        public ActionResult EquipmentBasicInfoModify(EquipmentBasicInfo equipmentBasicInfo)
        {
            DbEntityEntry<EquipmentBasicInfo> entry = db.Entry(equipmentBasicInfo);
            entry.State = System.Data.Entity.EntityState.Modified;
            int count = db.SaveChanges();
            if (count > 0) return Content("保存成功。");
            else return Content("保存失败。");
        }

        #region------获取所有设备编号的方法  Index2
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：获取所有设备编号的方法
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到EquipmentBasicInfo表里按照ID排序查询设备编号并去重
        /// 4.方法（可能）有结果：如果EquipmentBasicInfo表没有数据，查询数据就为空，EquipmentBasicInfo表里有数据，输出EquipmentBasicInfo表里去重过的所有设备编号
        /// </summary> 
        public ActionResult EquipmentNumberList()
        {
            var equi_list = db.EquipmentBasicInfo.OrderByDescending(m => m.Id).Select(c => c.EquipmentNumber).Distinct();
            return Content(JsonConvert.SerializeObject(equi_list));
        }
        #endregion

        #region------根据设备编号获取设备详细信息的方法 Index2
        ///<summary>
        /// 1.方法的作用：获取设备详细信息
        /// 2.方法的参数和用法：equipmentNumber（设备编号）
        /// 3.方法的具体逻辑顺序，判断条件：先判断设备编号是否为空，然后再根据设备编号到EquipmentSetStation表里查询跟设备编号相对应的所有数据
        /// 4.方法（可能）有结果：判断条件为空，输出“输入的设备编号不正确，请重新输入！”，查询数据不为空，就输出该设备编号的所有数据，查询数据为空，输出null
        /// </summary>
        public ActionResult Particulars(string equipmentNumber)
        {
            if (equipmentNumber != null)//判断设备编号是否为空
            {
                var partic = db.EquipmentSetStation.Where(c => c.EquipmentNumber == equipmentNumber).ToList();//根据设备编号查找数据
                return Content(JsonConvert.SerializeObject(partic));
            }
            return Content("输入的设备编号不正确，请重新输入！");
        }

        #endregion

        #region------产线添加设备的方法(单个)  Index2
        ///<summary>
        /// 1.方法的作用：产线添加设备（单个添加）
        /// 2.方法的参数和用法：EquipmentSetStation （所有字段），存储所有字段和数据
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断设备编号，使用部门，线别号是否为空。不为空进入下一步查询数据。（2）.根据使用部门和线别查询是否已经有该设备位置号，
        /// 有该位置号：循环查询数据，位置号加1，无该位置号，添加保存人和保存时间并把数据保存到数据库里的EquipmentSetStation表。
        /// 4.方法（可能）有结果：设备编号，使用部门，线别号为空，输出“添加设备数据有误！”。设备编号，使用部门，线别号不为空，输出“添加设备成功！”。
        /// </summary>
        public ActionResult ADDEquipment(EquipmentSetStation EquipmentSetStation)
        {
            JObject mess = new JObject();
            //判断设备编号，使用部门，线别号是否为空
            if (!String.IsNullOrEmpty(EquipmentSetStation.EquipmentNumber) && !String.IsNullOrEmpty(EquipmentSetStation.UserDepartment) && !String.IsNullOrEmpty(EquipmentSetStation.LineNum))
            {
                //根据使用部门、线别号、位置号查找数据
                var eqlist = db.EquipmentSetStation.Where(c => c.UserDepartment == EquipmentSetStation.UserDepartment && c.LineNum == EquipmentSetStation.LineNum && c.StationNum >= EquipmentSetStation.StationNum).ToList();
                foreach (var item in eqlist)//循环查询出来的数据
                {
                    item.StationNum = item.StationNum + 1;//位置号加一
                    db.SaveChanges();//保存到数据库
                }
                EquipmentSetStation.CreateTime = DateTime.Now;//添加创建时间
                EquipmentSetStation.Creator = ((Users)Session["user"]).UserName;//添加创建人
                db.EquipmentSetStation.Add(EquipmentSetStation);//把数据保存到对应的表
                var savecount = db.SaveChanges();//保存到数据库
                if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                {
                    mess.Add("messlist", "添加设备成功！");
                    mess.Add("equipmentSetStation", JsonConvert.SerializeObject(EquipmentSetStation));
                    return Content(JsonConvert.SerializeObject(mess));
                }
                else //savecount等于0（没有把数据保存到数据库或者保存出错）
                {
                    mess.Add("messlist", "添加设备失败！");
                    mess.Add("equipmentSetStation", null);
                    return Content(JsonConvert.SerializeObject(mess));
                }
            }

            return Content("添加设备数据有误！");
        }
        #endregion

        #region------产线删除设备 Index2
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：删除设备
        /// 2.方法的参数和用法：equipmentNumber（设备编号），删除条件。
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断设备编号是否为空。（2）根据设备编号查询该设备的数据（FirstOrDefault()）。
        /// （3）根据第二步查询出来的数据查询，根据使用部门，线别号查询位置号是否大于等于。（4）循环第三步查询出来的数据，位置号减1。（5）根据第二步查询出来的数据删除EquipmentSetStation表里的数据。
        /// 4.方法（可能）有结果：设备编号为空，删除失败。设备编号不为空，第二步查询出来的数据不为空，删除成功。
        /// </summary>
        public ActionResult deleteEquipment(string equipmentNumber)
        {
            if (!String.IsNullOrEmpty(equipmentNumber))//判断设备编号是否为空
            {
                var eq = db.EquipmentSetStation.Where(c => c.EquipmentNumber == equipmentNumber).FirstOrDefault();//根据设备编号查询该设备的数据
                //根据使用部门、线别号和位置号查询数据
                var eqlist = db.EquipmentSetStation.Where(c => c.UserDepartment == eq.UserDepartment && c.LineNum == eq.LineNum && c.StationNum >= eq.StationNum).ToList();
                foreach (var item in eqlist)//循环
                {
                    item.StationNum = item.StationNum - 1;//位置号减一
                    db.SaveChanges();//保存
                }
                db.EquipmentSetStation.Remove(eq);//删除对应的数据
                db.SaveChanges();//保存
                return Content("删除设备成功！");
            }
            return Content("删除设备失败！");
        }


        #endregion

        #region------产线一条添加  Index2

        public class equipment_station
        {
            public int Key { get; set; }
            public string Value { get; set; }
        }

        public ActionResult ADDLineNum(string usedepartment, string lineNum, string equipmentNumberlist)
        {
            List<equipment_station> eqnumlist = (List<equipment_station>)JsonHelper.jsonDes<List<equipment_station>>(equipmentNumberlist);
            List<equipment_station> eqnumlist1 = (List<equipment_station>)JsonHelper.jsonDes<List<equipment_station>>(equipmentNumberlist);
            if (!String.IsNullOrEmpty(usedepartment) && !String.IsNullOrEmpty(lineNum) && eqnumlist.Count > 0)
            {
                List<EquipmentSetStation> eqlist = new List<EquipmentSetStation>();
                foreach (var item in eqnumlist)
                {
                    EquipmentSetStation eq = new EquipmentSetStation();
                    var eqdata = db.EquipmentBasicInfo.Where(c => c.EquipmentNumber == item.Value).FirstOrDefault();
                    eq.EquipmentNumber = item.Value; //设备编号
                    eq.AssetNumber = eqdata.AssetNumber;//资产编号
                    eq.EquipmentName = eqdata.EquipmentName;//设备名称
                    eq.Status = "停机";//默认设备状态为停机
                    eq.UserDepartment = usedepartment;//使用部门
                    eq.WorkShop = eqdata.WorkShop;//车间
                    eq.LineNum = lineNum;//产线号（名）                
                    eq.Section = eqdata.Section;//工段
                    eq.StationNum = item.Key;//位置序号
                    eq.Creator = ((Users)Session["user"]).UserName;//创建记录人
                    eq.CreateTime = DateTime.Now;//创建时间
                    eqlist.Add(eq);
                }
                db.EquipmentSetStation.AddRange(eqlist);
                db.SaveChanges();
                foreach (var it in eqnumlist1)
                {
                    var deparlist = db.EquipmentBasicInfo.Where(c => c.EquipmentNumber == it.Value).FirstOrDefault();
                    deparlist.UserDepartment = usedepartment;
                    deparlist.LineNum = lineNum;
                    db.Entry(deparlist).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Content(usedepartment + "添加" + lineNum + "产线成功,该产线添加了" + eqnumlist.Count + "台设备。");
            }
            return Content("添加产线失败！" + (String.IsNullOrEmpty(usedepartment) == true ? "未选择部门！" : "") + (String.IsNullOrEmpty(lineNum) == true ? "没有产线名！" : "") + (eqnumlist.Count == 0 ? "产线至少要有一台设备！" : ""));
        }
        #endregion

        #region------迁移设备的方法 Index2
        ///<summary>
        /// 1.方法的作用：迁移设备
        /// 2.方法的参数和用法：equipmentNumber(设备编号)，userdepar（使用部门），linenum（线别号），stationnum（位置号），workShop（车间），section（工段），remark（备注）。用法：查询条件和迁移数据。
        /// 3.方法的具体逻辑顺序，判断条件：(1)根据设备编号到EquipmentSetStation表里查询数据。（2）根据线别号到EquipmentSetStation表里查询数据。（3）根据第二步查询出来的数据，判断要迁移的产线等于0（没有此产线），直接迁移数据。
        /// （4）根据第二步查询出来的数据，判断要迁移的产线大于0（有此产线），再判断此设备位置号是否有设备：如果已有此设备号，要根据使用部门和线别号查询数据，再先把原来的设备往后移（+1），再迁移设备；
        /// 如果此设备位置号没有设备，直接迁移；（4.1）根据设备编号到EquipmentBasicInfo表里查询数据（FirstOrDefault()）。（4.2）判断查询出来的数据是否为空，不为空时，修改里面的数据（修改：使用部门，线别号等数据）。
        /// 4.方法（可能）有结果：一二步查询数据为空，迁移失败。一二步查询数据不为空时，迁移成功。
        /// </summary>
        public ActionResult Migration(string equipmentNumber, string userdepar, string linenum, int stationnum, string workShop, string section, string remark)
        {
            var eq = db.EquipmentSetStation.FirstOrDefault(c => c.EquipmentNumber == equipmentNumber);
            //检查是否有迁移的产线，如果没有此产线，直接迁移。如果有此产线，检查此设备位置号是否有设备，如果此设备位置号没有设备，直接迁移，如果已有此设备号，先把原来的设备往后移，再迁移设备。
            var new_linenum_eqlist = db.EquipmentSetStation.Where(c => c.LineNum == linenum).ToList();
            if (new_linenum_eqlist.Count == 0) //没有此产线号
            {
                eq.UserDepartment = userdepar;//保存使用部门
                eq.LineNum = linenum;//线别号
                eq.StationNum = stationnum;//位置号
                eq.WorkShop = workShop;//车间
                eq.Section = section;//工段
                eq.Remark = remark;//备注
                eq.Modifier = ((Users)Session["user"]).UserName;//添加修改人
                eq.ModifyTime = DateTime.Now;//添加修改时间
                db.SaveChanges();//保存
                return Content("迁移设备成功！");
            }
            if (new_linenum_eqlist.Count > 0) //有此产线号
            {
                if (stationnum == eq.StationNum) //有此位置号
                {
                    //根据使用部门和线别号查询数据
                    var asst = db.EquipmentSetStation.Where(c => c.UserDepartment == userdepar && c.LineNum == linenum).ToList();
                    foreach (var item in asst)//循环
                    {
                        item.StationNum = item.StationNum + 1;//位置号+1
                        db.SaveChanges();//保存到数据库
                    }
                    eq.UserDepartment = userdepar;//使用部门
                    eq.LineNum = linenum;//线别号
                    eq.Remark = remark;//备注
                    eq.Modifier = ((Users)Session["user"]).UserName;//添加修改人
                    eq.ModifyTime = DateTime.Now;//添加修改时间
                    db.SaveChanges();//保存
                }
                //没有此位置号
                else
                {
                    eq.UserDepartment = userdepar;//保存使用部门
                    eq.LineNum = linenum;//线别号
                    eq.StationNum = stationnum;//位置号
                    eq.Remark = remark;//备注
                    eq.Modifier = ((Users)Session["user"]).UserName;//添加修改人
                    eq.ModifyTime = DateTime.Now;//添加修改时间
                    db.SaveChanges();//保存到数据库
                }
                var deparlist = db.EquipmentBasicInfo.Where(c => c.EquipmentNumber == equipmentNumber).FirstOrDefault();//根据设备编号查询数据
                if (deparlist != null)//判断查询出来的数据是否为空
                {
                    deparlist.UserDepartment = userdepar;//使用部门
                    deparlist.LineNum = linenum;//线别号
                    deparlist.WorkShop = workShop;//车间
                    deparlist.Section = section;//工段
                    db.Entry(deparlist).State = EntityState.Modified;//修改数据
                    db.SaveChanges();//把修改好的数据保存到数据库
                }
                return Content("迁移设备成功！");
            }
            return Content("迁移设备失败，请检查数据是否正确！");
        }
        #endregion

        #region---从空压机汇总表获取实时设备状态
        public ActionResult KongYa_Status()
        {
            JObject statu = new JObject();
            JObject table = new JObject();
            JObject rerult = new JObject();
            JArray stateList = new JArray();
            var it1 = db_KongYa.aircomp1.Max(c => c.id);//读取1#空压机的最大的ID
            var kongya1 = db_KongYa.aircomp1.Where(c => c.id == it1).Select(c => new { c.id, c.status, c.pressure, c.temperature, c.current_u, c.recordingTime }).ToList();//获取1#空压机表里的数据
            var it2 = db_KongYa.aircomp2.Max(c => c.id);//读取2#空压机的最大的ID
            var kongya2 = db_KongYa.aircomp2.Where(c => c.id == it2).Select(c => new { c.id, c.status, c.pressure, c.temperature, c.current_u, c.recordingTime }).ToList();//获取2#空压机表里的数据
            var it3 = db_KongYa.aircomp3.Max(c => c.id);//读取3#空压机的最大的ID
            var kongya3 = db_KongYa.aircomp3.Where(c => c.id == it3).Select(c => new { c.id, c.status, c.pressure, c.temperature, c.current_u, c.recordingTime }).ToList();//获取3#空压机表里的数据
            var status_list = db.EquipmentSetStation.Where(c => c.UserDepartment == "技术部").Select(c => new { c.EquipmentNumber, c.EquipmentName, c.Status, c.StationNum, c.LineNum }).ToList();//获取设备等于技术部的数据
            var tingji = "停机";
            var weix = "维修";
            var lixian = "离线";
            var duand = "断电";
            var yunx = "运行";
            if (kongya1.Count > 0)//判断1#空压机表里的数据是否为空
            {
                foreach (var item in kongya1)//循环1#空压机数据表
                {
                    if (item.current_u > 40)//判断电流是否大于40
                    {
                        foreach (var eq in status_list)//循环查找出来的数据（设备数据表）
                        {
                            if (eq.EquipmentName == "空压机" && eq.StationNum == 1)//判断设备名称是否等于空压机和判断设备位置号是否等于1号
                            {
                                statu.Add("equipmentNumber", eq.EquipmentNumber);
                                statu.Add("equipmentName", eq.EquipmentName);
                                statu.Add("mechineindex", eq.StationNum);
                                statu.Add("status", yunx);
                            }
                        }
                        table.Add("table", statu);
                        statu = new JObject();
                    }
                    else if (item.current_u < 40)//判断电流是否小于40
                    {
                        foreach (var eq in status_list)//循环查找出来的数据（设备数据表）
                        {
                            if (eq.EquipmentName == "空压机" && eq.StationNum == 1)//判断设备名称是否等于空压机和判断设备位置号是否等于1号
                            {
                                statu.Add("equipmentNumber", eq.EquipmentNumber);
                                statu.Add("equipmentName", eq.EquipmentName);
                                statu.Add("mechineindex", eq.StationNum);
                                statu.Add("status", tingji);
                                stateList.Add(weix);
                                stateList.Add(lixian);
                                stateList.Add(duand);
                            }
                        }
                        table.Add("table", statu);
                        statu = new JObject();
                        table.Add("stateList", stateList);
                        stateList = new JArray();
                    }
                }
            }
            if (kongya2.Count > 0)//判断2#空压机表里的数据是否为空
            {
                foreach (var item2 in kongya2)//循环2#空压机数据表的数据
                {
                    if (item2.current_u > 40)//判断电流是否大于40
                    {
                        foreach (var eq in status_list)//循环查找出来的数据（设备数据表）
                        {
                            if (eq.EquipmentName == "空压机" && eq.StationNum == 2)//判断设备名称是否等于空压机和设备位置号是否等于2号
                            {
                                statu.Add("equipmentNumber", eq.EquipmentNumber);
                                statu.Add("equipmentName", eq.EquipmentName);
                                statu.Add("mechineindex", eq.StationNum);
                                statu.Add("status", yunx);
                            }
                        }
                        table.Add("table1", statu);
                        statu = new JObject();
                    }
                    else if (item2.current_u < 40)//判断电流是否小于40
                    {
                        foreach (var eq in status_list)//循环查找出来的数据（设备数据表）
                        {
                            if (eq.EquipmentName == "空压机" && eq.StationNum == 2)//判断设备名称是否等于空压机和设备位置号是否等于2号
                            {
                                statu.Add("equipmentNumber", eq.EquipmentNumber);
                                statu.Add("equipmentName", eq.EquipmentName);
                                statu.Add("mechineindex", eq.StationNum);
                                statu.Add("status", tingji);
                                stateList.Add(weix);
                                stateList.Add(lixian);
                                stateList.Add(duand);
                            }
                        }
                        table.Add("table1", statu);
                        statu = new JObject();
                        table.Add("stateList1", stateList);
                        stateList = new JArray();
                    }
                }
            }
            if (kongya3.Count > 0)//判断3#空压机表里的数据是否为空
            {

                foreach (var item3 in kongya3)//循环3#空压机数据表的数据
                {
                    if (item3.current_u > 40)//判断电流是否大于40
                    {
                        foreach (var eq in status_list)//循环查找出来的数据（设备数据表）
                        {
                            if (eq.EquipmentName == "空压机" && eq.StationNum == 3)//判断设备名称是否等于空压机和设备位置号是否等于3
                            {
                                statu.Add("equipmentNumber", eq.EquipmentNumber);
                                statu.Add("equipmentName", eq.EquipmentName);
                                statu.Add("mechineindex", eq.StationNum);
                                statu.Add("status", yunx);
                            }
                        }
                        table.Add("table2", statu);
                        statu = new JObject();
                    }
                    else if (item3.current_u < 40)//判断电流是否小于40
                    {
                        foreach (var eq in status_list)//循环查找出来的数据（设备数据表）
                        {
                            if (eq.EquipmentName == "空压机" && eq.StationNum == 3)//判断设备名称是否等于空压机和判断设备位置号是否等于3
                            {
                                statu.Add("equipmentNumber", eq.EquipmentNumber);
                                statu.Add("equipmentName", eq.EquipmentName);
                                statu.Add("mechineindex", eq.StationNum);
                                statu.Add("status", tingji);
                                stateList.Add(weix);
                                stateList.Add(lixian);
                                stateList.Add(duand);
                            }
                        }
                        table.Add("table2", statu);
                        statu = new JObject();
                        table.Add("stateList2", stateList);
                        stateList = new JArray();
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(table));
        }

        #endregion

        #endregion

        #region------点检保养记录------

        #region------设备点检保养记录表

        //点检查询首页
        public ActionResult Equipment_Tally()
        {
            return View();
        }

        //点检记录查询方法
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：查询点检表记录
        /// 2.方法的参数和用法：userdepartment（使用部门），equipmentName（设备名称），lineNum（线别），equipmentNumber（设备编号），year（年），month（月），查询条件。
        /// 3.方法的具体逻辑顺序，判断条件：（1）先把Equipment_Tally_maintenance表里的数据全部封装到recordlist里面。（2）判断使用部门里是否为空，不为空时，根据使用部门到第一步查找出来的数据里查询相对应的数据。
        /// （3）判断设备名称里是否为空，不为空时，根据设备名称到第一步查找出来的数据里查询相对应的数据。（4）所有参数都是按照第一和第二步的步骤写。
        /// 4.方法（可能）有结果：查询条件不为空时并能查找出相对应的数据，输出查询数据；查询数据为空时，输出null。
        /// </summary>
        public ActionResult Equipment_Tally(string userdepartment, string equipmentName, string lineNum, string equipmentNumber, int year = 0, int month = 0)
        {
            List<Equipment_Tally_maintenance> recordlist = db.Equipment_Tally_maintenance.ToList();
            if (!String.IsNullOrEmpty(userdepartment))//判断使用部门里是否为空
            {
                recordlist = recordlist.Where(c => c.UserDepartment == userdepartment).ToList();//根据使用部门查询相对应的数据
            }
            if (!String.IsNullOrEmpty(equipmentName))//判断设备名称里是否为空
            {
                recordlist = recordlist.Where(c => c.EquipmentName == equipmentName).ToList();//根据设备名称查询相对应的数据
            }
            if (!String.IsNullOrEmpty(lineNum))//判断线别号是否为空
            {
                recordlist = recordlist.Where(c => c.LineName == lineNum).ToList();//根据线别号查询相对应的数据
            }
            if (!String.IsNullOrEmpty(equipmentNumber))//判断设备编号是否为空
            {
                recordlist = recordlist.Where(c => c.EquipmentNumber == equipmentNumber).ToList();//根据设备编号查询相对应的数据
            }
            if (year != 0)//判断年是否等于0
            {
                recordlist = recordlist.Where(c => c.Year == year).ToList();//根据设年查询相对应的数据
            }
            if (month != 0)//判断月是否等于0
            {
                recordlist = recordlist.Where(c => c.Month == month).ToList();//根据设月查询相对应的数据
            }
            var result = recordlist.Select(c => new { c.Id, c.EquipmentName, c.EquipmentNumber, c.LineName, c.Year, c.Month, c.UserDepartment }).ToList();
            return Content(JsonConvert.SerializeObject(result));
        }

        //打开点检记录
        public ActionResult Equipment_Tally_maintenance()
        {
            return View();
        }

        //打开点检记录取数据
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：打开点检记录取数据
        /// 2.方法的参数和用法：equipnumber（设备编号），查询条件。
        /// 3.方法的具体逻辑顺序，判断条件：根据设备编号到Equipment_Tally_maintenance表里查询数据。（FirstOrDefault()）
        /// 4.方法（可能）有结果：查询数据不为空时，输出查到的数据；查询数据为空时，输出null。
        /// </summary>
        public ActionResult Equipment_Tally_maintenance(string equipnumber)
        {
            var record = db.Equipment_Tally_maintenance.Where(c => c.EquipmentNumber == equipnumber).FirstOrDefault();
            return Content(JsonConvert.SerializeObject(record));
        }

        //[HttpPost]
        //public ActionResult Equipment_Tally_maintenance_Getdata(string equipmentNumber, int year = 0, int month = 0)
        //{
        //    if (year == 0 || month == 0)
        //    {
        //        DateTime today = DateTime.Now;
        //        var record = db.Equipment_Tally_maintenance.Where(c => c.Year == today.Year && c.Month == today.Month).FirstOrDefault();
        //        if (record == null)
        //        {
        //            var eq = db.EquipmentBasicInfo.Where(c => c.EquipmentNumber == equipmentNumber).FirstOrDefault();
        //            record.Year = DateTime.Now.Year;
        //            record.Month = DateTime.Now.Month;
        //            record.EquipmentNumber = equipmentNumber;
        //            record.LineName = eq.LineNum;
        //            record.EquipmentName = eq.EquipmentName;
        //            return Content(JsonConvert.SerializeObject(record));
        //        }
        //        else return Content(JsonConvert.SerializeObject(record));
        //    }
        //    else
        //    {
        //        var record = db.Equipment_Tally_maintenance.Where(c => c.Year == year && c.Month == month).FirstOrDefault();
        //        return Content("");
        //    }
        //}
        #endregion

        #region---获取所有点检保养的设备编号的方法
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：获取所有点检保养的设备编号
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到Equipment_Tally_maintenance表里按照ID的排序顺序查询所有设备编号并去重。
        /// 4.方法（可能）有结果：输出查询数据；输出null。
        /// </summary>
        public ActionResult TallyList()
        {
            var equipmentNumber = db.Equipment_Tally_maintenance.OrderByDescending(m => m.Id).Select(c => c.EquipmentNumber).Distinct();
            return Content(JsonConvert.SerializeObject(equipmentNumber));
        }
        #endregion

        #region---设备点检有数据使用部门的方法
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：获取点检表里有数据的使用部门
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到Equipment_Tally_maintenance表里按照ID的排序顺序查询所有使用部门并去重。
        /// 4.方法（可能）有结果：输出查询数据；输出null。
        /// </summary>
        public ActionResult Tally_Deparlist()
        {
            var deparlist = db.Equipment_Tally_maintenance.OrderByDescending(m => m.Id).Select(c => c.UserDepartment).Distinct();
            return Content(JsonConvert.SerializeObject(deparlist));
        }
        #endregion

        #region----设备管理:点检保养记录表(重组数据)
        ///<summary>
        /// 1.方法的作用：重组数据并输出
        /// 2.方法的参数和用法：equipmentNumber（设备编号），year（年），month（月）；查询条件。
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据查询条件到Equipment_Tally_maintenance表里查询数据。（2）判断第一步查询出来的数据是否为空，不为空时，获取右边的数据值。
        /// 4.方法（可能）有结果：null，输出查询出来的数据
        /// </summary>
        public ActionResult Equipment_Query_Tally(string equipmentNumber, int year, int month)
        {
            //根据设备编号和年月查找数据
            var tally_record = db.Equipment_Tally_maintenance.Where(c => c.EquipmentNumber == equipmentNumber && c.Year == year && c.Month == month).FirstOrDefault();
            JObject result2 = new JObject();//接收右边的值
            if (tally_record != null)//获取右边值
            {
                result2 = PrintProperties2(tally_record);
            }
            return Content(JsonConvert.SerializeObject(result2));
        }
        public JObject PrintProperties2(Equipment_Tally_maintenance eqcp)//右边表的对象
        {
            JObject temp = new JObject();
            foreach (PropertyInfo p in eqcp.GetType().GetProperties())
            {
                if (p.GetValue(eqcp) == null)//判断右边表的对象是否为空
                    temp.Add(p.Name, "");
                else temp.Add(p.Name, p.GetValue(eqcp).ToString());
            }
            return temp;
        }
        #endregion

        #region------设备点检保养记录创建保存
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：点检保养记录表创建保存。
        /// 2.方法的参数和用法：equipment_Tally_maintenance，用法：存储数据（所有字段）。
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断equipment_Tally_maintenance是否为空，和设备编号，使用部门，线别，年月也是否为空。（2）检查要保存的数据是否已经存在数据库（条件：设备编号，年月）。
        /// （3）判断第二步是否为空，不为空时（大于0就是该设备编号该年该月已经有数据存在了），直接输出“记录已经存在”，为空时，直接把数据保存到数据库。
        /// 4.方法（可能）有结果：第一步为空，第二步不为空时，添加失败；第一步不为空，第二步为空时，添加成功。
        /// </summary> 
        public ActionResult Equipment_Tally_maintenance_Add(Equipment_Tally_maintenance equipment_Tally_maintenance)
        {
            //判断equipment_Tally_maintenance是否为空，和设备编号，使用部门，线别，年月也是否为空
            if (equipment_Tally_maintenance != null && equipment_Tally_maintenance.EquipmentNumber != null && equipment_Tally_maintenance.UserDepartment != null && equipment_Tally_maintenance.LineName != null && equipment_Tally_maintenance.Year != 0 && equipment_Tally_maintenance.Month != 0)
            {
                //检查是否存在：检查要保存的数据是否已经存在数据库（条件：设备编号，年月）
                int count = db.Equipment_Tally_maintenance.Count(c => c.EquipmentNumber == equipment_Tally_maintenance.EquipmentNumber && c.Year == equipment_Tally_maintenance.Year && c.Month == equipment_Tally_maintenance.Month);
                if (count > 0)//判断检查是否存在（是否大于0，大于0就是该设备编号该年该月已经有数据存在了）
                {
                    return Content("记录已经存在");
                }
                db.Equipment_Tally_maintenance.Add(equipment_Tally_maintenance);//把数据保存到对应的表
                int result = db.SaveChanges();
                if (result > 0) //判断result是否大于0（有没有把数据保存到数据库）
                    return Content("true");
                else   //result等于0（没有把数据保存到数据库或者保存出错）
                    return Content("false");
            }
            return Content("false");
        }
        #endregion

        #region---点检保养记录（导入Excel表格）
        ///<summary>
        /// 1.方法的作用：导入Excel表格
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：
        /// 4.方法（可能）有结果：
        /// </summary>
        public ActionResult Upload_Equipment_Tally()
        {
            try
            {
                HttpPostedFileBase uploadfile = Request.Files["fileup"];
                if (uploadfile == null)
                {
                    return Content("no:非法上传");
                }
                if (uploadfile.FileName == "")
                {
                    return Content("no:请选择文件");
                }

                string fileExt = Path.GetExtension(uploadfile.FileName);
                StringBuilder sbtime = new StringBuilder();
                //equipment.Append(DateTime.Now.Year).Append(DateTime.Now.Month);
                string dir = "/UploadFile/" + sbtime.ToString() + fileExt;
                string realfilepath = Request.MapPath(dir);
                string readDir = Path.GetDirectoryName(realfilepath);
                if (!Directory.Exists(readDir))
                    Directory.CreateDirectory(readDir);
                uploadfile.SaveAs(realfilepath);
                //提取数据 
                var tally = ExcelTool.ExcelToDataTable(true, realfilepath);
                Equipment_Tally_maintenance equipment_Tally_report = new Equipment_Tally_maintenance();

                #region---A组日保养
                equipment_Tally_report.Day_project1 = String.IsNullOrEmpty(tally.Rows[4][1].ToString()) ? null : Convert.ToString(tally.Rows[4][1]);//日保养项目1       
                equipment_Tally_report.Day_opera1 = String.IsNullOrEmpty(tally.Rows[4][3].ToString()) ? null : Convert.ToString(tally.Rows[4][3]); //日保养操作方法1

                #endregion

                #region---B组日保养
                equipment_Tally_report.Day_project2 = String.IsNullOrEmpty(tally.Rows[5][1].ToString()) ? null : Convert.ToString(tally.Rows[5][1]);//日保养项目2
                equipment_Tally_report.Day_opera2 = String.IsNullOrEmpty(tally.Rows[5][3].ToString()) ? null : Convert.ToString(tally.Rows[5][3]);//日保养操作方法2

                #endregion

                #region---C组日保养
                equipment_Tally_report.Day_project3 = String.IsNullOrEmpty(tally.Rows[6][1].ToString()) ? null : Convert.ToString(tally.Rows[6][1]);//日保养项目3
                equipment_Tally_report.Day_opera3 = String.IsNullOrEmpty(tally.Rows[6][3].ToString()) ? null : Convert.ToString(tally.Rows[6][3]); //日保养操作方法3

                #endregion

                #region---D组日保养
                equipment_Tally_report.Day_project4 = String.IsNullOrEmpty(tally.Rows[7][1].ToString()) ? null : Convert.ToString(tally.Rows[7][1]);//日保养项目4
                equipment_Tally_report.Day_opera4 = String.IsNullOrEmpty(tally.Rows[7][3].ToString()) ? null : Convert.ToString(tally.Rows[7][3]);//日保养操作方法4
                #endregion

                #region---E组日保养
                equipment_Tally_report.Day_project5 = String.IsNullOrEmpty(tally.Rows[8][1].ToString()) ? null : Convert.ToString(tally.Rows[8][1]);//日保养项目5
                equipment_Tally_report.Day_opera5 = String.IsNullOrEmpty(tally.Rows[8][3].ToString()) ? null : Convert.ToString(tally.Rows[8][3]);//日保养操作方法5
                #endregion

                #region---F组日保养
                equipment_Tally_report.Day_project6 = String.IsNullOrEmpty(tally.Rows[9][1].ToString()) ? null : Convert.ToString(tally.Rows[9][1]);//日保养项目6
                equipment_Tally_report.Day_opera6 = String.IsNullOrEmpty(tally.Rows[9][3].ToString()) ? null : Convert.ToString(tally.Rows[9][3]);//日保养操作方法6
                #endregion

                #region---G组日保养
                equipment_Tally_report.Day_project7 = String.IsNullOrEmpty(tally.Rows[10][1].ToString()) ? null : Convert.ToString(tally.Rows[10][1]);//日保养项目7
                equipment_Tally_report.Day_opera7 = String.IsNullOrEmpty(tally.Rows[10][3].ToString()) ? null : Convert.ToString(tally.Rows[10][3]);//日保养操作方法7
                #endregion

                #region---H组日保养
                equipment_Tally_report.Day_project8 = String.IsNullOrEmpty(tally.Rows[11][1].ToString()) ? null : Convert.ToString(tally.Rows[11][1]);//日保养项目8
                equipment_Tally_report.Day_opera8 = String.IsNullOrEmpty(tally.Rows[11][3].ToString()) ? null : Convert.ToString(tally.Rows[11][3]);//日保养操作方法8
                #endregion

                #region---I组日保养
                equipment_Tally_report.Day_project9 = String.IsNullOrEmpty(tally.Rows[12][1].ToString()) ? null : Convert.ToString(tally.Rows[12][1]);//日保养项目9
                equipment_Tally_report.Day_opera9 = String.IsNullOrEmpty(tally.Rows[12][3].ToString()) ? null : Convert.ToString(tally.Rows[12][3]);//日保养操作方法9
                #endregion

                #region---J组日保养
                equipment_Tally_report.Day_project10 = String.IsNullOrEmpty(tally.Rows[13][1].ToString()) ? null : Convert.ToString(tally.Rows[13][1]);//日保养项目10
                equipment_Tally_report.Day_opera10 = String.IsNullOrEmpty(tally.Rows[13][3].ToString()) ? null : Convert.ToString(tally.Rows[13][3]);//日保养操作方法10
                #endregion

                #region---K组日保养
                equipment_Tally_report.Day_project11 = String.IsNullOrEmpty(tally.Rows[14][1].ToString()) ? null : Convert.ToString(tally.Rows[14][1]);//日保养项目11
                equipment_Tally_report.Day_opera11 = String.IsNullOrEmpty(tally.Rows[14][3].ToString()) ? null : Convert.ToString(tally.Rows[14][3]);//日保养操作方法11
                #endregion

                #region---周保养
                equipment_Tally_report.Week_Check1 = String.IsNullOrEmpty(tally.Rows[17][1].ToString()) ? null : Convert.ToString(tally.Rows[17][1]);//周保养项目1
                equipment_Tally_report.Week_Inspe1 = String.IsNullOrEmpty(tally.Rows[17][3].ToString()) ? null : Convert.ToString(tally.Rows[17][3]);//周保养操作方法1
                equipment_Tally_report.Week_Check2 = String.IsNullOrEmpty(tally.Rows[18][1].ToString()) ? null : Convert.ToString(tally.Rows[18][1]);//周保养项目2
                equipment_Tally_report.Week_Inspe2 = String.IsNullOrEmpty(tally.Rows[18][3].ToString()) ? null : Convert.ToString(tally.Rows[18][3]);//周保养操作方法2
                equipment_Tally_report.Week_Check3 = String.IsNullOrEmpty(tally.Rows[19][1].ToString()) ? null : Convert.ToString(tally.Rows[19][1]);//周保养项目3
                equipment_Tally_report.Week_Inspe3 = String.IsNullOrEmpty(tally.Rows[19][3].ToString()) ? null : Convert.ToString(tally.Rows[19][3]);//周保养操作方法3
                equipment_Tally_report.Week_Check4 = String.IsNullOrEmpty(tally.Rows[20][1].ToString()) ? null : Convert.ToString(tally.Rows[20][1]);//周保养项目4
                equipment_Tally_report.Week_Inspe4 = String.IsNullOrEmpty(tally.Rows[20][3].ToString()) ? null : Convert.ToString(tally.Rows[20][3]);//周保养操作方法4
                equipment_Tally_report.Week_Check5 = String.IsNullOrEmpty(tally.Rows[21][1].ToString()) ? null : Convert.ToString(tally.Rows[21][1]);//周保养项目5
                equipment_Tally_report.Week_Inspe5 = String.IsNullOrEmpty(tally.Rows[21][3].ToString()) ? null : Convert.ToString(tally.Rows[21][3]);//周保养操作方法5
                equipment_Tally_report.Week_Check6 = String.IsNullOrEmpty(tally.Rows[22][1].ToString()) ? null : Convert.ToString(tally.Rows[22][1]);//周保养项目6
                equipment_Tally_report.Week_Inspe6 = String.IsNullOrEmpty(tally.Rows[22][3].ToString()) ? null : Convert.ToString(tally.Rows[22][3]);//周保养操作方法6
                equipment_Tally_report.Week_Check7 = String.IsNullOrEmpty(tally.Rows[23][1].ToString()) ? null : Convert.ToString(tally.Rows[23][1]);//周保养项目7
                equipment_Tally_report.Week_Inspe7 = String.IsNullOrEmpty(tally.Rows[23][3].ToString()) ? null : Convert.ToString(tally.Rows[23][3]);//周保养操作方法7
                equipment_Tally_report.Week_Check8 = String.IsNullOrEmpty(tally.Rows[24][1].ToString()) ? null : Convert.ToString(tally.Rows[24][1]);//周保养项目8
                equipment_Tally_report.Week_Inspe8 = String.IsNullOrEmpty(tally.Rows[24][3].ToString()) ? null : Convert.ToString(tally.Rows[24][3]);//周保养操作方法8
                equipment_Tally_report.Week_Check9 = String.IsNullOrEmpty(tally.Rows[25][1].ToString()) ? null : Convert.ToString(tally.Rows[25][1]);//周保养项目9
                equipment_Tally_report.Week_Inspe9 = String.IsNullOrEmpty(tally.Rows[25][3].ToString()) ? null : Convert.ToString(tally.Rows[25][3]);//周保养操作方法9
                equipment_Tally_report.Week_Check10 = String.IsNullOrEmpty(tally.Rows[26][1].ToString()) ? null : Convert.ToString(tally.Rows[26][1]);//周保养项目10
                equipment_Tally_report.Week_Inspe10 = String.IsNullOrEmpty(tally.Rows[26][3].ToString()) ? null : Convert.ToString(tally.Rows[26][3]);//周保养操作方法10
                equipment_Tally_report.Week_Check11 = String.IsNullOrEmpty(tally.Rows[27][1].ToString()) ? null : Convert.ToString(tally.Rows[27][1]);//周保养项目11
                equipment_Tally_report.Week_Inspe11 = String.IsNullOrEmpty(tally.Rows[27][3].ToString()) ? null : Convert.ToString(tally.Rows[27][3]);//周保养操作方法11

                #endregion

                #region---月保养
                equipment_Tally_report.Month_Project1 = String.IsNullOrEmpty(tally.Rows[30][1].ToString()) ? null : Convert.ToString(tally.Rows[30][1]);//月保养项目1
                equipment_Tally_report.Month_Approach1 = String.IsNullOrEmpty(tally.Rows[30][3].ToString()) ? null : Convert.ToString(tally.Rows[30][3]);//月保养操作方法1
                equipment_Tally_report.Month_Project2 = String.IsNullOrEmpty(tally.Rows[31][1].ToString()) ? null : Convert.ToString(tally.Rows[31][1]);//月保养项目2
                equipment_Tally_report.Month_Approach2 = String.IsNullOrEmpty(tally.Rows[31][3].ToString()) ? null : Convert.ToString(tally.Rows[31][3]);//月保养操作方法2
                equipment_Tally_report.Month_Project3 = String.IsNullOrEmpty(tally.Rows[32][1].ToString()) ? null : Convert.ToString(tally.Rows[32][1]);//月保养项目3
                equipment_Tally_report.Month_Approach3 = String.IsNullOrEmpty(tally.Rows[32][3].ToString()) ? null : Convert.ToString(tally.Rows[32][3]);//月保养操作方法3
                equipment_Tally_report.Month_Project4 = String.IsNullOrEmpty(tally.Rows[33][1].ToString()) ? null : Convert.ToString(tally.Rows[33][1]);//月保养项目4
                equipment_Tally_report.Month_Approach4 = String.IsNullOrEmpty(tally.Rows[33][3].ToString()) ? null : Convert.ToString(tally.Rows[33][3]);//月保养操作方法4
                equipment_Tally_report.Month_Project5 = String.IsNullOrEmpty(tally.Rows[34][1].ToString()) ? null : Convert.ToString(tally.Rows[34][1]);//月保养项目5
                equipment_Tally_report.Month_Approach5 = String.IsNullOrEmpty(tally.Rows[34][3].ToString()) ? null : Convert.ToString(tally.Rows[34][3]);//月保养操作方法5
                equipment_Tally_report.Month_Project6 = String.IsNullOrEmpty(tally.Rows[35][1].ToString()) ? null : Convert.ToString(tally.Rows[35][1]);//月保养项目6
                equipment_Tally_report.Month_Approach6 = String.IsNullOrEmpty(tally.Rows[35][3].ToString()) ? null : Convert.ToString(tally.Rows[35][3]);//月保养操作方法6
                equipment_Tally_report.Month_Project7 = String.IsNullOrEmpty(tally.Rows[36][1].ToString()) ? null : Convert.ToString(tally.Rows[36][1]);//月保养项目7
                equipment_Tally_report.Month_Approach7 = String.IsNullOrEmpty(tally.Rows[36][3].ToString()) ? null : Convert.ToString(tally.Rows[36][3]);//月保养操作方法7
                equipment_Tally_report.Month_Project8 = String.IsNullOrEmpty(tally.Rows[37][1].ToString()) ? null : Convert.ToString(tally.Rows[37][1]);//月保养项目8
                equipment_Tally_report.Month_Approach8 = String.IsNullOrEmpty(tally.Rows[37][3].ToString()) ? null : Convert.ToString(tally.Rows[37][3]);//月保养操作方法8
                equipment_Tally_report.Month_Project9 = String.IsNullOrEmpty(tally.Rows[38][1].ToString()) ? null : Convert.ToString(tally.Rows[38][1]);//月保养项目9
                equipment_Tally_report.Month_Approach9 = String.IsNullOrEmpty(tally.Rows[38][3].ToString()) ? null : Convert.ToString(tally.Rows[38][3]);//月保养操作方法9
                equipment_Tally_report.Month_Project10 = String.IsNullOrEmpty(tally.Rows[39][1].ToString()) ? null : Convert.ToString(tally.Rows[39][1]);//月保养项目10
                equipment_Tally_report.Month_Approach10 = String.IsNullOrEmpty(tally.Rows[39][3].ToString()) ? null : Convert.ToString(tally.Rows[39][3]);//月保养操作方法10
                equipment_Tally_report.Month_Project11 = String.IsNullOrEmpty(tally.Rows[40][1].ToString()) ? null : Convert.ToString(tally.Rows[40][1]);//月保养项目11
                equipment_Tally_report.Month_Approach11 = String.IsNullOrEmpty(tally.Rows[40][3].ToString()) ? null : Convert.ToString(tally.Rows[40][3]);//月保养操作方法11

                #endregion

                JObject mes = new JObject();
                mes.Add("mes", true);
                mes.Add("equipment_Tally_report", JsonConvert.SerializeObject(equipment_Tally_report));
                return Content(JsonConvert.SerializeObject(mes));
            }
            catch
            {
                return Content("您输入的表格跟点检保养记录模板表格不一致，请使用标准模板表格。");
            }
        }

        #endregion

        #region------设备点检保养记录修改
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：修改点检保养记录
        /// 2.方法的参数和用法：equipment_Tally_maintenance（所有字段），更新数据。
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断equipment_Tally_maintenance是否为空，不为空时。（2）根据设备编号、年月和部长确认到Equipment_Tally_maintenance表查询数据，并判断查找出来的数据是否大于0，如果大于0，直接输出修改失败。
        /// （3）根据设备编号、年月到Equipment_Tally_maintenance表查询数据，并判断查找出来的数据是否大于0；大于0就可以进入下一步修改数据。
        /// 4.方法（可能）有结果：第二步大于0，修改失败；第二步等于0，第三步大于0，修改成功。
        /// </summary>
        public ActionResult Equipment_Tally_maintenance_Edit(Equipment_Tally_maintenance equipment_Tally_maintenance)
        {
            JObject tally = new JObject();
            if (equipment_Tally_maintenance != null)//判断equipment_Tally_maintenance是否为空
            {
                //1.根据设备编号、年月和部长确认到Equipment_Tally_maintenance表查询数据，并判断查找出来的数据是否大于0
                if (db.Equipment_Tally_maintenance.Count(c => c.EquipmentNumber == equipment_Tally_maintenance.EquipmentNumber && c.Year == equipment_Tally_maintenance.Year && c.Month == equipment_Tally_maintenance.Month && c.Month_minister_3 != null) > 0)
                {
                    tally.Add("tally", false);
                    tally.Add("equipment_Tally_maintenance", "部长确认有数据");
                    return Content(JsonConvert.SerializeObject(tally));
                }
                else //第1部查找出来的数据等于0
                {
                    //根据设备编号、年月到Equipment_Tally_maintenance表查询数据，并判断查找出来的数据是否大于0(需要大于0)
                    if (db.Equipment_Tally_maintenance.Count(c => c.EquipmentNumber == equipment_Tally_maintenance.EquipmentNumber && c.Year == equipment_Tally_maintenance.Year && c.Month == equipment_Tally_maintenance.Month) > 0)
                    {
                        db.Entry(equipment_Tally_maintenance).State = EntityState.Modified;//修改数据
                        var result = db.SaveChanges();
                        if (result > 0)//判断result是否大于0（有没有把数据保存到数据库）
                        {
                            tally.Add("tally", true);
                            tally.Add("equipment_Tally_maintenance", JsonConvert.SerializeObject(equipment_Tally_maintenance));
                            return Content(JsonConvert.SerializeObject(tally));
                        }
                        else //result等于0（没有把数据保存到数据库或者保存出错）
                        {
                            tally.Add("tally", false);
                            tally.Add("equipment_Tally_maintenance", null);
                            return Content(JsonConvert.SerializeObject(tally));
                        }
                    }
                }
            }
            return Content("数据有误");
        }
        #endregion

        #endregion

        #region------仪器设备报修单------
        public ActionResult EquipmentRepairbill()
        {
            return View();
        }
        #region---获取所有仪器设备报修单的设备编号的方法
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：获取所有仪器设备报修单的设备编号
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到EquipmentRepairbill表里按照ID的排序顺序查询所有设备编号并去重。
        /// 4.方法（可能）有结果：输出查询数据；输出null。
        /// </summary>
        public ActionResult InstrumentList()
        {
            var Instr_Number = db.EquipmentRepairbill.OrderByDescending(m => m.Id).Select(c => c.EquipmentNumber).Distinct();
            return Content(JsonConvert.SerializeObject(Instr_Number));
        }
        #endregion

        #region---设备报修单有数据使用部门的方法
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：获取报修单有数据使用部门的方法
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到EquipmentRepairbill表里按照ID的排序顺序查询所有使用部门并去重。
        /// 4.方法（可能）有结果：输出查询数据；输出null。
        /// </summary>
        public ActionResult Deparlist()
        {
            var depar = db.EquipmentRepairbill.OrderByDescending(m => m.Id).Select(c => c.UserDepartment).Distinct();
            return Content(JsonConvert.SerializeObject(depar));
        }
        #endregion

        #region-----根据部门，设备编号，设备名称，故障时间查询仪器设备报修单
        public ActionResult EquipmentRepairbill_Query()
        {
            return View();
        }

        [HttpPost]
        ///<summary>
        /// 1.方法的作用：查询仪器设备报修单
        /// 2.方法的参数和用法：userdepartment（使用部门），equnumber（设备编号），equipname（设备名称）date（故障时间），starttime（开始时间），endtime（结束时间），year（年），month（月），查询条件。
        /// 3.方法的具体逻辑顺序，判断条件：（1）先把EquipmentRepairbill表里的数据全部查找出来赋值到equipment_list。（2）判断使用部门是否为空，不为空时根据使用部门查询第一步查找出来的数据；为空时进入下一个判断。
        /// （3）判断设备编号是否为空，不为空时根据设备编号查询第一步查找出来的数据；为空时进入下一个判断。（4）判断设备名称是否为空，不为空时根据设备名称查询第一步查找出来的数据；为空时进入下一个判断。
        /// （5）判断故障时间是否为空，不为空时根据设备编号查询第一步查找出来的数据；为空时进入下一个判断。（6）判断故障开始时间和故障结束时间是否为空，不为空时根据故障开始时间和故障结束时间查询第一步查找出来的数据；为空时进入下一个判断。
        /// （7）判断年是否为空，不为空时根据故障时间的年查询第一步查找出来的数据；为空时进入下一个判断。（8）判断年月是否为空，不为空时根据故障时间的年月查询第一步查找出来的数据，为空时直接输出null。。
        /// 4.方法（可能）有结果：8个判断都为空或者查询出来的数据都为空，输出null；8个判断只要有一个不为空并能查询出对应的数据，输出想要的数据。
        /// </summary>
        public ActionResult EquipmentRepairbill_Query(string userdepartment, string equnumber, string equipname, DateTime? date, DateTime? starttime, DateTime? endtime, int? year, int? month)
        {
            var equipment_list = db.EquipmentRepairbill.ToList();
            if (!String.IsNullOrEmpty(userdepartment))//判断使用部门是否为空
            {
                equipment_list = equipment_list.Where(c => c.UserDepartment == userdepartment).ToList();//根据使用部门查询数据
            }
            if (!String.IsNullOrEmpty(equnumber))//判断设备编号是否为空
            {
                equipment_list = equipment_list.Where(c => c.EquipmentNumber == equnumber).ToList();//根据设备编号查找数据
            }
            if (!String.IsNullOrEmpty(equipname))//判断设备名称是否为空
            {
                equipment_list = equipment_list.Where(c => c.EquipmentName == equipname).ToList();//根据设备名称查询数据
            }
            if (date != null)//判断故障时间是否为空
            {
                equipment_list = equipment_list.Where(c => c.FaultTime == date).ToList();//根据故障时间查询数据
            }
            if (starttime != null && endtime != null)//判断故障开始时间和故障结束时间是否为空
            {
                equipment_list = equipment_list.Where(c => c.FaultTime >= starttime && c.FaultTime <= endtime).ToList();//根据故障数据查询数据，只是分时间段查
            }
            if (year != null && month == null)//判断年是否为空
            {
                equipment_list = equipment_list.Where(c => c.FaultTime.Value.Year == year && c.ConfirmName != null).ToList();//根据故障时间的年和确认人不等于null查询数据
            }
            if (year != null && month != null)//判断年月是否为空
            {
                //根据故障时间的年、故障时间的月和确认人不等于null查询数据
                equipment_list = equipment_list.Where(c => c.FaultTime.Value.Year == year && c.FaultTime.Value.Month == month && c.ConfirmName != null).ToList();
            }
            return Content(JsonConvert.SerializeObject(equipment_list));
        }
        #endregion

        #region------仪器设备报修单数据创建保存
        ///<summary>
        /// 1.方法的作用：设备报修单数据创建保存
        /// 2.方法的参数和用法：equipmentRepairbill，用法：存储数据（所有字段）。
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断equipmentRepairbill是否为空和使用部门，故障时间是否也为空。（2）检查要保存的数据是否已经存在数据库（条件：设备编号，故障时间）并判断是否大于0。
        /// （3）第二步大于0时（大于0就是该设备编号该故障时间已经有数据存在了），直接输出“已有重复数据，请重新填写故障时间！”，等于0时，直接把数据保存到数据库并添加创建人和创建时间。
        /// 4.方法（可能）有结果：第一步为空，第二步大于0时，添加失败；第一步不为空，第二步小于等于0时，添加成功。
        /// </summary> 
        public ActionResult Repairbill_maintenance(EquipmentRepairbill equipmentRepairbill)
        {
            JObject equipment = new JObject();
            if (equipmentRepairbill != null && equipmentRepairbill.UserDepartment != null && equipmentRepairbill.FaultTime != null)//判断equipmentRepairbill是否为空和使用部门，故障时间是否也为空
            {
                // 检查要保存的数据是否已经存在数据库（条件：设备编号，故障时间），并判断是否大于0（大于0就是该设备编号该故障时间已经有数据存在了）
                if (db.EquipmentRepairbill.Count(c => c.EquipmentNumber == equipmentRepairbill.EquipmentNumber && c.EquipmentName == equipmentRepairbill.EquipmentName && c.FaultTime == equipmentRepairbill.FaultTime) > 0)
                {
                    equipment.Add("addEquipment", false);
                    equipment.Add("equipmentRepairbill", "已有重复数据，请重新填写故障时间！");
                    return Content(JsonConvert.SerializeObject(equipment));

                }
                equipmentRepairbill.RepairDate = DateTime.Now;//添加报修时间
                equipmentRepairbill.RepairName = ((Users)Session["user"]).UserName;//添加报修人员
                db.EquipmentRepairbill.Add(equipmentRepairbill);//把保存到对应的数据表
                var savecount = db.SaveChanges();//保存到数据库
                if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                {
                    equipment.Add("addEquipment", true);
                    equipment.Add("equipmentRepairbill", "新建保存成功！");
                    return Content(JsonConvert.SerializeObject(equipment));
                }
                else //savecount等于0（没有把数据保存到数据库或者保存出错）
                {
                    equipment.Add("addEquipment", false);
                    equipment.Add("equipmentRepairbill", "新建保存失败！");
                    return Content(JsonConvert.SerializeObject(equipment));
                }
            }
            equipment.Add("addEquipment", false);
            equipment.Add("equipmentRepairbill", "数据有误！");
            return Content(JsonConvert.SerializeObject(equipment));
        }
        #endregion

        #region------仪器设备报修单修改
        ///<summary>
        /// 1.方法的作用：修改仪器设备报修单里的数据
        /// 2.方法的参数和用法：equipmentRepairbill（所有字段），用法：存储更新数据。
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断equipmentRepairbill是否为空，不为空时，直接修改数据。（2）判断savecount是否大于0（有没有把数据保存到数据库）。
        /// （3）savecount等于0（没有把数据保存到数据库或者保存出错）。
        /// 4.方法（可能）有结果：第一步判断为空和第三步等于0，输出修改失败；第一步判断不为空和第二步大于0，输出修改成功。
        /// </summary>
        public ActionResult Modify_repairs(EquipmentRepairbill equipmentRepairbill)
        {
            JObject repairbill = new JObject();
            if (equipmentRepairbill != null)//判断equipmentRepairbill是否为空
            {
                db.Entry(equipmentRepairbill).State = EntityState.Modified;//修改数据
                var savecount = db.SaveChanges();//保存数据库
                if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                {
                    repairbill.Add("repairbill", "修改成功！");
                    repairbill.Add("equipmentRepairbill", JsonConvert.SerializeObject(equipmentRepairbill));
                    return Content(JsonConvert.SerializeObject(repairbill));
                }
                else //savecount等于0（没有把数据保存到数据库或者保存出错）
                {
                    repairbill.Add("repairbill", "修改失败！");
                    repairbill.Add("equipmentRepairbill", null);
                    return Content(JsonConvert.SerializeObject(repairbill));
                }
            }
            return Content("false");
        }
        #endregion

        #endregion

        #region------设备月保养时间计划表------

        public ActionResult Equipment_MonthlyMaintenance_plan()//首页index
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Equipment", act = "Equipment_MonthlyMaintenance_plan" });
            }
            return View();
        }

        //月保养时间计划表图表查询首页
        public ActionResult Equipment_MonthTotalQuery()
        {
            return View();
        }
        #region----月保养时间计划表图表查询页
        public ActionResult Month_TotalQuery(int? year, int? month)
        {
            JObject table = new JObject();
            JObject equipment = new JObject();
            JObject shuju = new JObject();
            JObject zuizhong = new JObject();
            JArray maintenance = new JArray();
            JArray dayu = new JArray();//大于
            JArray xiaoyu = new JArray();//小于
            JArray dengyu = new JArray();//等于
            JArray weikong = new JArray();//为空
            DateTime dateTime = DateTime.Now;//获取当前时间
            JObject baoyang = new JObject();
            JArray dayubaoyang = new JArray();
            JArray xiaoyubaoyang = new JArray();
            JArray dengyubaoyang = new JArray();
            var departmentList = db.Equipment_MonthlyMaintenance.Select(c => c.UserDepartment).Distinct().ToList();//先把数据表有的部门都查找出来去重（部门列表）
            if (departmentList.Count > 0)
            {
                foreach (var item in departmentList)//循环部门列表
                {
                    var monthList = db.Equipment_MonthlyMaintenance.ToList();
                    if (year != null && month != null)
                    {
                        monthList = monthList.Where(c => c.UserDepartment == item && c.Year == year && c.Month == month).ToList();//根据部门和年月查询数据                      
                    }
                    else if (year != null && month == null)
                    {
                        monthList = monthList.Where(c => c.UserDepartment == item && c.Year == year).ToList();//根据部门和年查询数据           
                    }
                    if (monthList.Count > 0)
                    {
                        foreach (var maint in monthList)//循环根据部门和年月查询出来的数据
                        {
                            if (maint.MaintenanceDate > maint.Mainten_equipment)//判断实际保养日期是否大于计划保养日期
                            {
                                equipment.Add("Id", maint.Id);
                                equipment.Add("EquipmentNumber", maint.EquipmentNumber);
                                equipment.Add("EquipmentName", maint.EquipmentName);
                                equipment.Add("Year", maint.Year);
                                equipment.Add("Month", maint.Month);
                                dayu.Add(equipment);
                                equipment = new JObject();
                            }
                            else if (maint.MaintenanceDate == maint.Mainten_equipment)//判断实际保养日期是否等于计划保养日期
                            {
                                equipment.Add("Id", maint.Id);
                                equipment.Add("EquipmentNumber", maint.EquipmentNumber);
                                equipment.Add("EquipmentName", maint.EquipmentName);
                                equipment.Add("Year", maint.Year);
                                equipment.Add("Month", maint.Month);
                                dengyu.Add(equipment);
                                equipment = new JObject();
                            }
                            else if (maint.MaintenanceDate < maint.Mainten_equipment)//判断实际保养日期是否小于计划保养日期
                            {
                                equipment.Add("Id", maint.Id);
                                equipment.Add("EquipmentNumber", maint.EquipmentNumber);
                                equipment.Add("EquipmentName", maint.EquipmentName);
                                equipment.Add("Year", maint.Year);
                                equipment.Add("Month", maint.Month);
                                xiaoyu.Add(equipment);
                                equipment = new JObject();
                            }
                            else if (maint.MaintenanceDate == null)//判断实际保养日期是否等于null
                            {
                                if (dateTime > maint.Mainten_equipment)//判断当前时间是否大于计划保养日期
                                {
                                    equipment.Add("Id", maint.Id);
                                    equipment.Add("EquipmentNumber", maint.EquipmentNumber);
                                    equipment.Add("EquipmentName", maint.EquipmentName);
                                    equipment.Add("Year", maint.Year);
                                    equipment.Add("Month", maint.Month);
                                    dayubaoyang.Add(equipment);
                                    equipment = new JObject();
                                }
                                else if (dateTime < maint.Mainten_equipment)//判断当前时间是否小于计划保养日期
                                {

                                    equipment.Add("Id", maint.Id);
                                    equipment.Add("EquipmentNumber", maint.EquipmentNumber);
                                    equipment.Add("EquipmentName", maint.EquipmentName);
                                    equipment.Add("Year", maint.Year);
                                    equipment.Add("Month", maint.Month);
                                    xiaoyubaoyang.Add(equipment);
                                    equipment = new JObject();
                                }
                                else if (dateTime == maint.Mainten_equipment)//判断当前时间是否等于计划保养日期
                                {
                                    equipment.Add("Id", maint.Id);
                                    equipment.Add("EquipmentNumber", maint.EquipmentNumber);
                                    equipment.Add("EquipmentName", maint.EquipmentName);
                                    equipment.Add("Year", maint.Year);
                                    equipment.Add("Month", maint.Month);
                                    dengyubaoyang.Add(equipment);
                                    equipment = new JObject();
                                }
                            }
                        }
                        baoyang.Add("Late_late", dayubaoyang);//未按计划保养或者延迟保养
                        dayubaoyang = new JArray();
                        baoyang.Add("Before", xiaoyubaoyang);//未到计划保养时间
                        xiaoyubaoyang = new JArray();
                        baoyang.Add("Current", dengyubaoyang);//当前需保养设备
                        dengyubaoyang = new JArray();
                        weikong.Add(baoyang);
                        baoyang = new JObject();

                        table.Add("UserDepartment", item);//使用部门
                        shuju.Add("Late", dayu);//逾期保养
                        dayu = new JArray();
                        shuju.Add("Advance", xiaoyu);//提前保养
                        xiaoyu = new JArray();
                        shuju.Add("Ontime", dengyu);//按时保养
                        dengyu = new JArray();
                        shuju.Add("Nomaintenance", weikong);//未保养
                        weikong = new JArray();
                        table.Add("Datum", shuju);//数据
                        shuju = new JObject();
                        maintenance.Add(table);
                        table = new JObject();

                    }

                }
            }
            return Content(JsonConvert.SerializeObject(maintenance));
        }
        #endregion

        #region----查询页
        ///<summary>
        /// 1.方法的作用：查询设备月保养时间计划表
        /// 2.方法的参数和用法：userDepartment（设备使用部门），year（年），month（月）；用法：查询条件。
        /// 3.方法的具体逻辑顺序，判断条件：(1)先根据年月到Equipment_MonthlyMaintenance表里查询数据。（2）判断第一步查询出来的数据是否为空。（3）根据使用部门和年月到第一步查询出来的数据查询相对应的数据。
        /// （4）循环第三步查询出来的数据。（5）ADD页面想要显示的数据（如：ID，UserDepartment使用部门，Year年，Month月，EquipmentName设备名称等）。（6）判断查询出来的数据（mainplan）是否为空。
        /// （7）根据保养时间小于现在的时间（Datetime.now）和实际保养日期等于null到mainplan查询设备编号。（8）ADD第七步查询出来的数据。
        /// 4.方法（可能）有结果：第二步为空时和第三步查询为空时，输出null；：第二步不为空时和第三步查询不为空时，输出想要的数据。
        /// </summary>
        public ActionResult Query_Maintenance_plan(string userDepartment, int year, int month)
        {
            JArray Maintenance_plan_list = new JArray();
            JObject query_plan = new JObject();
            JObject query_plan1 = new JObject();
            var mainplan_list = db.Equipment_MonthlyMaintenance.Where(c => c.Year == year && c.Month == month).ToList();//根据年月查询数据
            if (mainplan_list != null)//判断查询出来的数据是否为空(mainplan_list)
            {
                var mainplan = mainplan_list.Where(c => c.UserDepartment == userDepartment && c.Year == year && c.Month == month).ToList();//根据使用部门和年月到mainplan_list查询相对应的数据。
                foreach (var item in mainplan)//循环查询出来的数据（mainplan）
                {
                    //id
                    query_plan.Add("Id", mainplan.Count == 0 ? 0 : item.Id);
                    //保养部门
                    query_plan.Add("UserDepartment", mainplan.Count == 0 ? null : item.UserDepartment);
                    //年
                    query_plan.Add("Year", mainplan.Count == 0 ? 0 : item.Year);
                    //月
                    query_plan.Add("Month", mainplan.Count == 0 ? 0 : item.Month);
                    //设备编号
                    query_plan.Add("EquipmentNumber", mainplan.Count == 0 ? null : item.EquipmentNumber);
                    //设备名称
                    query_plan.Add("EquipmentName", mainplan.Count == 0 ? null : item.EquipmentName);
                    //保养时间
                    query_plan.Add("Mainten_equipment", mainplan.Count == 0 ? null : item.Mainten_equipment);
                    //保养工时
                    query_plan.Add("Maintenance_work", mainplan.Count == 0 ? null : item.Maintenance_work);
                    //保养负责人
                    query_plan.Add("Mainten_supervisor", mainplan.Count == 0 ? null : item.Mainten_supervisor);
                    //实际保养日期
                    query_plan.Add("MaintenanceDate", mainplan.Count == 0 ? null : item.MaintenanceDate);
                    //下次保养日期
                    query_plan.Add("Nextmainten_cycle", mainplan.Count == 0 ? null : item.Nextmainten_cycle);
                    //备注
                    query_plan.Add("Remark", mainplan.Count == 0 ? null : item.Remark);
                    //月保养设备异常记录
                    query_plan.Add("Abnormal_records", mainplan.Count == 0 ? null : item.Abnormal_records);
                    Maintenance_plan_list.Add(query_plan);
                    query_plan = new JObject();
                }
                query_plan1.Add("mes", Maintenance_plan_list);
                Maintenance_plan_list = new JArray();
                //制表人
                query_plan1.Add("Mainten_Lister", mainplan.Count == 0 ? null : mainplan.FirstOrDefault().Mainten_Lister);
                //制表时间
                query_plan1.Add("TabulationTime", mainplan.Count == 0 ? null : mainplan.FirstOrDefault().TabulationTime);
                //技术部确认
                query_plan1.Add("Tec_Notarize", mainplan.Count == 0 ? null : mainplan.FirstOrDefault().Tec_Notarize);
                //技术部确认时间
                query_plan1.Add("Tec_NotarizeTime", mainplan.Count == 0 ? null : mainplan.FirstOrDefault().Tec_NotarizeTime);
                //保养设备部门确认
                query_plan1.Add("AssortDepar", mainplan.Count == 0 ? null : mainplan.FirstOrDefault().AssortDepar);
                //保养设备部门确认时间
                query_plan1.Add("AssortTime", mainplan.Count == 0 ? null : mainplan.FirstOrDefault().AssortTime);
                //PC部确认
                query_plan1.Add("PCDepar", mainplan.Count == 0 ? null : mainplan.FirstOrDefault().PCDepar);
                //PC部确认时间
                query_plan1.Add("PCdeparTime", mainplan.Count == 0 ? null : mainplan.FirstOrDefault().PCdeparTime);
                //审核
                query_plan1.Add("Assessor", mainplan.Count == 0 ? null : mainplan.FirstOrDefault().Assessor);
                //审核时间
                query_plan1.Add("AssessedDate", mainplan.Count == 0 ? null : mainplan.FirstOrDefault().AssessedDate);
                JObject query = new JObject();
                JArray plan = new JArray();
                if (mainplan != null)//判断查询出来的数据（mainplan）是否为空
                {
                    DateTime date = DateTime.Now;
                    //根据保养时间小于现在的时间（Datetime.now）和实际保养日期等于null到mainplan查询设备编号
                    var ment = mainplan.Where(c => c.Mainten_equipment < date && c.MaintenanceDate == null).Select(c => c.EquipmentNumber).ToList();
                    plan.Add(ment);
                    query.Add("ment", plan);
                    plan = new JArray();
                }
                query.Add("meg", "超出计划保养时间未保养设备");
                query_plan1.Add("mainten", query);
                query = new JObject();
            }

            return Content(JsonConvert.SerializeObject(query_plan1));
        }
        #endregion        

        #region-----检索时间计划表部门（有数据）
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：检索时间计划表部门（有数据）
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到Equipment_MonthlyMaintenance表里按照ID的排序顺序查询所有使用部门并去重。
        /// 4.方法（可能）有结果：输出查询数据；输出null。
        /// </summary>
        public ActionResult UserDepar_list()
        {
            var departmrnt_list = db.Equipment_MonthlyMaintenance.OrderByDescending(m => m.Id).Select(c => c.UserDepartment).Distinct();
            return Content(JsonConvert.SerializeObject(departmrnt_list));
        }

        #endregion  

        #region----创建保存页
        ///<summary>
        /// 1.方法的作用：创建保存月保养时间计划表
        /// 2.方法的参数和用法：equipment_MonthlyMaintenance（所有字段）；用法：保存数据。
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断equipment_MonthlyMaintenance对象是否为空，不为空时进入第二步。（2）根据使用部门、设备编号和年月查询数据，并判断是否大于0。等于0时，进入第三步。大于0直接输出“已有相同数据”，并退出程序运行。
        /// （3）根据使用部门、年月和审核要等于null查询数据，并判断是否大于0。等于0时，进入第四步。大于0时，输出“已经审核过”，并退出程序运行。（4）前两个判断条件都等于0时（第二、三步），添加创建人和创建时间并把数据保存到对应的表。
        /// （5）判断savecount是否大于0（有没有把数据保存到数据库），大于0时输出保存成功。等于0时，输出保存出错（保存失败）。
        /// 4.方法（可能）有结果：第一步判断为空时或者第二、三时判断大于0时，保存失败。第一步判断不为空时、第二、三时判断等于0和第五步大于0时，保存成功。
        /// </summary>
        public ActionResult ADDMonthlyMain_plan(Equipment_MonthlyMaintenance equipment_MonthlyMaintenance) //保存数据
        {
            JObject ADDequipment_Monthly = new JObject();
            string plan = null;
            if (equipment_MonthlyMaintenance != null)//判断equipment_MonthlyMaintenance对象是否为空
            {
                //根据使用部门、设备编号和年月查询数据，并判断是否大于0（大于0就是该部门该年该月该设备已经有数据存在了）。
                if (db.Equipment_MonthlyMaintenance.Count(c => c.UserDepartment == equipment_MonthlyMaintenance.UserDepartment && c.Year == equipment_MonthlyMaintenance.Year && c.Month == equipment_MonthlyMaintenance.Month && c.EquipmentNumber == equipment_MonthlyMaintenance.EquipmentNumber) > 0)
                {
                    plan = plan + equipment_MonthlyMaintenance.UserDepartment + equipment_MonthlyMaintenance.Year + equipment_MonthlyMaintenance.Month + equipment_MonthlyMaintenance.EquipmentNumber + ",";
                    ADDequipment_Monthly.Add("meg", "false");
                    ADDequipment_Monthly.Add("plan_main", plan + "已有相同数据");
                    return Content(JsonConvert.SerializeObject(ADDequipment_Monthly));
                }
                // 根据使用部门、年月和审核要等于null查询数据，并判断是否大于0
                if (db.Equipment_MonthlyMaintenance.Count(c => c.UserDepartment == equipment_MonthlyMaintenance.UserDepartment && c.Year == equipment_MonthlyMaintenance.Year && c.Month == equipment_MonthlyMaintenance.Month && c.AssortDepar != null) > 0)
                {
                    plan = plan + equipment_MonthlyMaintenance.UserDepartment + equipment_MonthlyMaintenance.Year + equipment_MonthlyMaintenance.Month + ",";
                    ADDequipment_Monthly.Add("meg", "false");
                    ADDequipment_Monthly.Add("plan_main", plan + "已经审核过");
                    return Content(JsonConvert.SerializeObject(ADDequipment_Monthly));
                }
                else //前两个判断都等于0时
                {
                    equipment_MonthlyMaintenance.Mainten_Lister = ((Users)Session["user"]).UserName;//添加创建人
                    equipment_MonthlyMaintenance.TabulationTime = DateTime.Now;//添加创建时间
                    db.Equipment_MonthlyMaintenance.Add(equipment_MonthlyMaintenance);//把数据保存到对应的表里
                    var savecount = db.SaveChanges();
                    if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                    {
                        ADDequipment_Monthly.Add("meg", "true");
                        return Content(JsonConvert.SerializeObject(ADDequipment_Monthly));
                    }
                    else //savecount等于0（没有把数据保存到数据库或者保存出错）
                    {
                        ADDequipment_Monthly.Add("meg", "false");
                        return Content(JsonConvert.SerializeObject(ADDequipment_Monthly));
                    }
                }
            }
            return Content("保存失败！");
        }
        #endregion

        #region---月保养计划批量添加
        ///<summary>
        /// 1.方法的作用：批量添加月保养计划
        /// 2.方法的参数和用法：inputList（数组），year（年），month（月），userdepartment（使用部门）。用法；保存数据。
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断年月和设备使用部门是否为空，不为空时进入第二步。（2）循环inputList并添加制表人和制表时间。
        /// （3）根据使用部门、年月和设备编号查询数据，并判断查询数据是否大于0（大于0就是该部门该年该月该设备已经有数据存在了），不大于0时进入第三步。
        /// （4）判断repat(存储相同数据的字段，也就是第三步查询出来的数据)是否为空，等于null时进入第五步，不为空时，直接输出相同的数据并停止运行程序。（5）循环inputList数据并保存使用部门和年月。
        /// （6）把数据保存到相对应的表里和判断savecount是否大于0（有没有把数据保存到数据库）。
        /// 4.方法（可能）有结果：第一步为空、第三步大于0和数据没有保存到数据库，批量添加失败。第一步不为空、第三步等于0和有把数据保存到数据库里，批量添加成功。
        /// </summary>
        public ActionResult Equip_ManintList(List<Equipment_MonthlyMaintenance> inputList, int year, int month, string userdepartment)
        {
            if (year != 0 && month != 0 && userdepartment != null && inputList.FirstOrDefault().EquipmentNumber != null)//判断年月和设备使用部门是否为空
            {
                JArray res = new JArray();
                string repat = "";
                foreach (var item in inputList)
                {
                    item.TabulationTime = DateTime.Now;
                    item.Mainten_Lister = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                    //根据使用部门、年月和设备编号查询数据，并判断查询数据是否大于0（大于0就是该部门该年该月该设备已经有数据存在了）
                    if (db.Equipment_MonthlyMaintenance.Count(c => c.UserDepartment == userdepartment && c.Year == year && c.Month == month && c.EquipmentNumber == item.EquipmentNumber) > 0)
                        repat = item.EquipmentNumber;
                    res.Add(repat);

                }
                JObject result = new JObject();
                if (!String.IsNullOrEmpty(repat))//判断repat(存储相同数据的字段)是否为空
                {
                    return Content(JsonConvert.SerializeObject(res));
                }
                foreach (var it in inputList)//循环inputList数据
                {
                    it.UserDepartment = userdepartment;//赋值使用部门
                    it.Year = year;//赋值年
                    it.Month = month;//赋值月
                    db.SaveChanges();//保存
                }
                db.Equipment_MonthlyMaintenance.AddRange(inputList);//把数据保存到相对应的表里
                int savecount = db.SaveChanges();
                if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                    return Content("true");
                else //savecount等于0（没有把数据保存到数据库或者保存出错）
                    return Content("false");
            }
            return Content("false");
        }

        #endregion

        #region----修改月保养时间计划表方法
        ///<summary>
        /// 1.方法的作用：修改月保养时间计划表
        /// 2.方法的参数和用法：equipment_MonthlyMaintenance（对象），用于更新数据。
        /// 3.方法的具体逻辑顺序，判断条件：判断equipment_MonthlyMaintenance是否为空，不为空时进入下一步。（2）判断实际保养日期和下次保养周期（保养有效期）是否为空，不为空时进入下一步。
        /// （3）下次保养周期（保养有效期）等于实际保养日期加30天；把年月按照时间模式显示如：2020-02；获取内网网址、外网网址、地址、主题等。（4）从UserItemEmail表里找项目名称等于“设备按计划保养发送”记录（人员名单）。
        /// （5）循环第五步找出来的数据，并把里面的某些数据赋值到recipient_list_string里（用户名，Email地址）。（6）从表里找项目名称等于“设备计划保养抄送”记录（人员名单）。（7）循环第六步，并把里面的某些数据赋值到ccList_list_string里面（用户名，Email地址）。
        /// （8）组合Email内容（网址，使用部门，年月，设备名称，计划保养时间，实际保养时间）。（9）add发送人，收件人，Email内容，主题，抄送人，地址和把数据保存到对应的表里（发送Email内容和收件人、抄送人、年月等数据）。
        /// （10）修改对应的数据数据，并把修改过的数据保存到数据库。（11）判断savecount是否大于0（有没有把数据保存到数据库），大于0修改成功；等于0（没有把数据保存到数据库或者保存出错），修改失败停止运行。
        /// 4.方法（可能）有结果：第一步为空和第十一步等于0，修改失败；第一步不为空和第十一步大于0，修改成功。
        /// </summary>
        public ActionResult Edit_MonthMain_plan(Equipment_MonthlyMaintenance equipment_MonthlyMaintenance)
        {
            JObject Editequipment_Monthly = new JObject();
            if (equipment_MonthlyMaintenance != null)//判断equipment_MonthlyMaintenance是否为空
            {
                equipment_MonthlyMaintenance.ModifyTime = DateTime.Now;//添加修改时间
                equipment_MonthlyMaintenance.Modifier = ((Users)Session["user"]).UserName;//添加修改人
                if (equipment_MonthlyMaintenance.MaintenanceDate != null && equipment_MonthlyMaintenance.Nextmainten_cycle == null)//判断实际保养日期和下次保养周期（保养有效期）是否为空
                {
                    equipment_MonthlyMaintenance.Nextmainten_cycle = equipment_MonthlyMaintenance.MaintenanceDate.Value.AddDays(30);//下次保养周期（保养有效期）等于实际保养日期加30天
                    string tiemer = equipment_MonthlyMaintenance.Year + "-" + equipment_MonthlyMaintenance.Month;//把年月按照时间模式显示如：2020-02
                    var host_inside = "172.16.6.145";//内网网址
                    var host_outside = "hzjhgd.vicp.io";//外网网址
                    string content = "";
                    string address = "";//地址
                    string theme = "设备按计划保养情况";//主题
                    var recipient_list = db.UserItemEmail.Where(c => c.ProcesName == "设备按计划保养发送").ToList();//从表里找项目名称等于“设备按计划保养发送”记录（人员名单）
                    string recipient_list_string = "";
                    foreach (var it in recipient_list)//循环
                    {
                        //把recipient_list里的某些数据赋值到recipient_list_string里面（用户名，Email地址）
                        recipient_list_string = recipient_list_string == "" ? (it.UserName + "(" + it.EmailAddress + ")") : recipient_list_string + "," + it.UserName + "(" + it.EmailAddress + ")";
                    }
                    var ccList_list = db.UserItemEmail.Where(c => c.ProcesName == "设备计划保养抄送").ToList();//从表里找项目名称等于“设备计划保养抄送”记录（人员名单）
                    string ccList_list_string = "";
                    foreach (var ite in ccList_list)
                    {
                        //把ccList_list里的某些数据赋值到ccList_list_string里面（用户名，Email地址）
                        ccList_list_string = ccList_list_string == "" ? (ite.UserName + "(" + ite.EmailAddress + ")") : ccList_list_string + "," + ite.UserName + "(" + ite.EmailAddress + ")";
                    }
                    //组合Email内容（网址，使用部门，年月，设备名称，计划保养时间，实际保养时间）
                    string content_list = @"<a href=""http://" + host_inside + "/Equipment/Equipment_MonthlyMaintenance_plan?" + "dates=" + tiemer + "&depar=" + equipment_MonthlyMaintenance.UserDepartment + "\"> "
                    + equipment_MonthlyMaintenance.EquipmentName + "," + "计划保养时间" + equipment_MonthlyMaintenance.Mainten_equipment + "," + "已完成保养" + "," + "实际保养时间:" + equipment_MonthlyMaintenance.MaintenanceDate
                    + "(内网)</a></br><a href=\"http://" + host_outside + "/Equipment/Equipment_MonthlyMaintenance_plan?" + "dates=" + tiemer + "&depar=" + equipment_MonthlyMaintenance.UserDepartment + "\"> "
                    + equipment_MonthlyMaintenance.EquipmentName + "," + "计划保养时间" + equipment_MonthlyMaintenance.Mainten_equipment + "," + "已完成保养" + "," + "实际保养时间:" + equipment_MonthlyMaintenance.MaintenanceDate + "(外网)</a></br>";

                    content = content + content_list + "</br>";//内容
                    //add发送人，收件人，Email内容，主题，抄送人，地址
                    meg.SendEmail("gaohj@lcjh.local", recipient_list.Select(c => c.EmailAddress).ToList(), content, theme, ccList_list.Select(c => c.EmailAddress).ToList(), address);
                    Equipment_Maintenmail eq = new Equipment_Maintenmail();
                    //下面的都是把数据保存到对应的表里（发送Email内容和收件人、抄送人、年月等数据）
                    eq.EquipmentNumber = equipment_MonthlyMaintenance.EquipmentNumber;
                    eq.EquipmentName = equipment_MonthlyMaintenance.EquipmentName;
                    eq.Mainten_equipment = equipment_MonthlyMaintenance.Mainten_equipment;
                    eq.MaintenanceDate = equipment_MonthlyMaintenance.MaintenanceDate;
                    eq.Nextmainten_cycle = equipment_MonthlyMaintenance.Nextmainten_cycle;
                    eq.Situation = "设备已完成保养";
                    eq.Year = equipment_MonthlyMaintenance.Year;
                    eq.Month = equipment_MonthlyMaintenance.Month;
                    eq.Sended = true;
                    eq.SendDateTime = DateTime.Now;
                    eq.SendSituation = "收件人：" + recipient_list_string + "抄送人：" + ccList_list_string;
                    db.Equipment_Maintenmail.Add(eq);
                    db.SaveChanges();
                }
                db.Entry(equipment_MonthlyMaintenance).State = EntityState.Modified;//修改对应的数据数据
                var savecount = db.SaveChanges();
                if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                {
                    Editequipment_Monthly.Add("Editequipment_Monthly", "修改成功！");
                    Editequipment_Monthly.Add("Editequipment_MonthlyMaintenance", JsonConvert.SerializeObject(equipment_MonthlyMaintenance));
                    return Content(JsonConvert.SerializeObject(Editequipment_Monthly));
                }
                else //savecount等于0（没有把数据保存到数据库或者保存出错）
                {
                    Editequipment_Monthly.Add("Editequipment_Monthly", "修改失败！");
                    Editequipment_Monthly.Add("Editequipment_MonthlyMaintenance", null);
                    return Content(JsonConvert.SerializeObject(Editequipment_Monthly));
                }
            }
            return Content("false");
        }
        #endregion

        #region---审核、确认时间计划表
        ///<summary>
        /// 1.方法的作用：审核、确认月保养时间计划表
        /// 2.方法的参数和用法：userdepartment（使用部门），year（年），month（月）属于查询条件；assortdepar（保养设备部门确认），tec_notarize（技术部确认），pcdepar（PC部确认），assessor（审核）用于更新数据。
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据使用部门和年月到Equipment_MonthlyMaintenance表里查询对应的数据。（2）判断查询出来的数据（plan：第一步）是否为空，不为空时进入下一步骤。
        /// （3）循环查询出来的数据（plan：第一步）。（4）判断保养设备部门确认是否为空，不为空时保存保养设备部门确认数据（用户名）和添加确认时间；为空时进入下一步。
        /// （5）判断技术部确认是否为空，不为空时保存技术部确认数据（用户名）和添加确认时间；为空时进入下一步。（6）判断PC部确认是否为空，不为空时保存PC部确认数据（用户名）和添加确认时间；为空时进入下一步。
        /// （7）判断审核是否为空，不为空时保存审核数据（用户名）和添加审核时间；为空时输出false。
        /// 4.方法（可能）有结果：第二步为空时，或者第四、五、六、七步都为空时，输出false；第二步不为空时和第四、五、六、七步有其中一个不为空时，输出true。
        /// </summary>
        public ActionResult EditAsserMonthMain_plan(string userdepartment, int year, int month, string assortdepar, string tec_notarize, string pcdepar, string assessor)
        {
            var plan = db.Equipment_MonthlyMaintenance.Where(c => c.UserDepartment == userdepartment && c.Year == year && c.Month == month).ToList();//根据使用部门和年月查询对应的数据
            if (plan != null)//判断查询出来的数据（plan）是否为空
            {
                foreach (var item in plan)//循环查询出来的数据（plan）
                {
                    if (!String.IsNullOrEmpty(assortdepar))//判断保养设备部门确认是否为空
                    {
                        item.AssortDepar = assortdepar;//赋值保养设备部门确认数据（用户名）
                        item.AssortTime = DateTime.Now;//确认时间
                        db.SaveChanges();//保存到数据库
                    }
                    else if (!String.IsNullOrEmpty(tec_notarize))//判断技术部确认是否为空
                    {
                        item.Tec_Notarize = tec_notarize;//赋值技术部确认数据（用户名）
                        item.Tec_NotarizeTime = DateTime.Now;//确认时间
                        db.SaveChanges();//保存到数据库
                    }
                    else if (!String.IsNullOrEmpty(pcdepar))//判断PC部确认是否为空
                    {
                        item.PCDepar = pcdepar;//赋值PC部确认数据（用户名）
                        item.PCdeparTime = DateTime.Now;//确认时间
                        db.SaveChanges();//保存到数据库
                    }
                    else if (!String.IsNullOrEmpty(assessor))//判断审核是否为空
                    {
                        item.Assessor = assessor;//赋值审核数据（用户名）
                        item.AssessedDate = DateTime.Now;//审核时间
                        db.SaveChanges();//保存到数据库
                    }
                }
                return Content("true");
            }
            return Content("false");
        }

        #endregion

        #endregion

        #region---设备月保养质量目标达成状况统计表---
        public ActionResult Equipment_Quality_target()
        {
            return View();
        }

        #region---查询设备月保养质量目标达成状况统计表
        ///<summary>
        /// 1.方法的作用：查询设备月保养质量目标达成状况统计表
        /// 2.方法的参数和用法：year年，month月，用于查询。
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据年月到Equipment_Quality_target表里查询对应的数据。（2）判断查询出来的数据（第一步）是否为空，不为空时进入下一步。（3）循环查询出来的数据（第一步）。
        /// （4）add页面想要显示的数据（如：ID，担责部门，使用部门质量目标，目标值...项目：实际保养台次，项目：有效率）。（5）判断第一步查询数据里的计划保养总台次实际值、实际保养台次、有效率是否为空。
        /// （6）第五步不为空时，直接add计划保养总台次实际值，实际保养台次，有效率百分比和add剩下不需要参与循环的数据（如：备注，编制人...批准，批准日期）。（7）第五为空时，根据使用部门，年月查询数据。
        /// （7.1）根据使用部门，年月查询和实际保养日期、下次保养周期（保养有效期）不为空的数据。（7.2）add查询出来的数据required；add查找出来的数据tally。（7.3）判断tally是否为空，为空是，有效率百分比直接为0。
        /// （7.4）7.3步不为空时，先7.2步的数据转换成小数点时，再计算有效率（实际保养台次/计划保养总台次实际值），最后add计算出来的数据。
        /// 4.方法（可能）有结果：第二步为空时，输出null，第二步不为空时，输出整理好的数据（查询成功）。
        /// </summary>
        public ActionResult Equipment_Quality_statistical(int year, int month)
        {
            JArray quality = new JArray();
            JObject quality_list = new JObject();
            JObject table = new JObject();
            int i = 0;
            var Quality_list = db.Equipment_Quality_target.Where(c => c.Year == year && c.Month == month).ToList();//根据年月查询数据
            if (Quality_list.Count > 0)//判断查询出来的数据是否为空
            {
                foreach (var dep in Quality_list)//循环查询出来的数据
                {
                    //ID
                    table.Add("Id", Quality_list.Count == 0 ? 0 : dep.Id);
                    //担责部门
                    table.Add("LiaDepartment", Quality_list.Count == 0 ? null : dep.LiaDepartment);
                    //使用部门
                    table.Add("UserDepartment", Quality_list.Count == 0 ? null : dep.UserDepartment);
                    //质量目标
                    table.Add("Quality_objec", Quality_list.Count == 0 ? null : dep.Quality_objec);
                    //目标值
                    table.Add("Target_value", Quality_list.Count == 0 ? null : dep.Target_value);
                    //计算公式
                    table.Add("Formulas", Quality_list.Count == 0 ? null : dep.Formulas);
                    //统计周期
                    table.Add("Statistical", Quality_list.Count == 0 ? null : dep.Statistical);
                    //项目：计划保养总台次实际值
                    table.Add("Required_maintain", Quality_list.Count == 0 ? null : dep.Required_maintain);
                    //项目：实际保养台次
                    table.Add("Planned_maintenance", Quality_list.Count == 0 ? null : dep.Planned_maintenance);
                    //项目：有效率
                    table.Add("With_efficiency", Quality_list.Count == 0 ? null : dep.With_efficiency);
                    if (dep.Planned == 0 && dep.Required == 0 && dep.efficiency == 0)//判断计划保养总台次实际值、实际保养台次、有效率是否为空
                    {
                        //根据使用部门，年月查询数据
                        var required = db.Equipment_MonthlyMaintenance.Where(c => c.UserDepartment == dep.UserDepartment && c.Year == year && c.Month == month).ToList();
                        //根据使用部门，年月查询和实际保养日期、下次保养周期（保养有效期）不为空的数据
                        var tally = db.Equipment_MonthlyMaintenance.Where(c => c.UserDepartment == dep.UserDepartment && c.Year == year && c.Month == month && c.MaintenanceDate != null && c.Nextmainten_cycle != null).ToList();
                        //项目：计划保养总台次实际值
                        table.Add("Required", required.Count == 0 ? 0 : required.Count);//add查询出来的数据required
                        //项目：实际保养台次
                        table.Add("Planned", tally.Count == 0 ? 0 : tally.Count);//add查找出来的数据tally
                        if (tally.Count > 0)//判断tally是否为空
                        {
                            decimal dt1 = tally.Count;//转换成小数点
                            decimal dt2 = required.Count;//转换成小数点
                            var efficiency = ((dt1 / dt2) * 100).ToString("F2");//计算有效率（实际保养台次/计划保养总台次实际值）
                            //项目：有效率百分比
                            table.Add("efficiency", efficiency);//add计算出来的数据
                        }
                        else //tally为空时
                        {
                            //项目：有效率百分比
                            table.Add("efficiency", 0);//有效率直接为0
                        }
                    }
                    else//计划保养总台次实际值、实际保养台次、有效率不为空
                    {
                        //项目：计划保养总台次实际值
                        table.Add("Required", dep.Required);
                        //项目：实际保养台次
                        table.Add("Planned", dep.Planned);
                        //项目：有效率百分比
                        table.Add("efficiency", dep.efficiency);
                    }
                    quality.Add(table);
                    table = new JObject();
                    i++;
                }
                quality_list.Add("quality_list", quality);
                quality = new JArray();
                //备注
                quality_list.Add("Remark", Quality_list.Count == 0 ? null : Quality_list.FirstOrDefault().Remark);
                //编制人
                quality_list.Add("PrepareName", Quality_list.Count == 0 ? null : Quality_list.FirstOrDefault().PrepareName);
                //编制日期
                quality_list.Add("PrepareTime", Quality_list.Count == 0 ? null : Quality_list.FirstOrDefault().PrepareTime);
                //审核
                quality_list.Add("Assessor", Quality_list.Count == 0 ? null : Quality_list.FirstOrDefault().Assessor);
                //审核日期
                quality_list.Add("AssessedDate", Quality_list.Count == 0 ? null : Quality_list.FirstOrDefault().AssessedDate);
                //批准
                quality_list.Add("Approve", Quality_list.Count == 0 ? null : Quality_list.FirstOrDefault().Approve);
                //批准日期
                quality_list.Add("ApprovedDate", Quality_list.Count == 0 ? null : Quality_list.FirstOrDefault().ApprovedDate);
            }
            return Content(JsonConvert.SerializeObject(quality_list));
        }
        #endregion

        #region---新增时，根据部门返回相对应的数据（待定）
        ///<summary>
        /// 1.方法的作用：新增时，根据部门返回相对应的数据
        /// 2.方法的参数和用法：
        /// 3.方法的具体逻辑顺序，判断条件：
        /// 4.方法（可能）有结果：
        /// </summary>
        public ActionResult AddQuality(string liaDepartment)
        {
            if (db.Equipment_Quality_target.Count(c => c.LiaDepartment == liaDepartment) > 0)
            {
                var partic = db.Equipment_Quality_target.Where(c => c.LiaDepartment == liaDepartment).Max(c => c.Year);
                var partic1 = db.Equipment_Quality_target.Where(c => c.LiaDepartment == liaDepartment).Max(c => c.Month);
                var depar = db.Equipment_Quality_target.Where(c => c.LiaDepartment == liaDepartment && c.Year == partic && c.Month == partic1).Select(c => new { c.LiaDepartment, c.Quality_objec, c.Target_value, c.Formulas, c.Statistical });
                return Content(JsonConvert.SerializeObject(depar));
            }
            return Content("false");
        }

        #endregion

        #region---保存当月数据周保养质量目标达成状况统计表（待定）
        ///<summary>
        /// 1.方法的作用：
        /// 2.方法的参数和用法：
        /// 3.方法的具体逻辑顺序，判断条件：
        /// 4.方法（可能）有结果：
        /// </summary>
        public ActionResult ADDequipment_quality(List<Equipment_Quality_target> Quality_target, string remark, int year, int month, string assessor)
        {
            if (assessor != null && year != 0 && month != 0)
            {
                foreach (var item in Quality_target)
                {
                    if (db.Equipment_Quality_target.Count(c => c.LiaDepartment == item.LiaDepartment && c.Year == year && c.Month == month) > 0)
                    {
                        var deparlist = db.Equipment_Quality_target.Where(c => c.LiaDepartment == item.LiaDepartment && c.Year == year && c.Month == month).FirstOrDefault();
                        deparlist.LiaDepartment = item.LiaDepartment;
                        deparlist.Required = item.Required;
                        deparlist.Planned = item.Planned;
                        deparlist.efficiency = item.efficiency;
                        deparlist.Year = year;
                        deparlist.Month = month;
                        db.Entry(deparlist).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return Content("true");
            }
            return Content("false");
        }


        #endregion

        #region---修改备注和审核、批准周保养质量目标达成状况统计表
        ///<summary>
        /// 1.方法的作用：修改备注和审核、批准
        /// 2.方法的参数和用法：year年，month月；用于查询，remark备注，preparName编制人，assessor审核，approve批准，用于修改。
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据年月到Equipment_Quality_target表里查询数据。（2）判断target_list（查询数据）是否为0。不为0时循环target_list（查询数据）。
        /// （3）判断备注是否为空，不为空时，保存备注数据、保存到数据库和把备注数据赋值到target里。（4）判断编制人是否为空，不为空时，保存编制人数据、添加时间、保存到数据库和把编制人数据赋值到target里。
        /// （5）判断审核是否为空，不为空时，保存审核数据、添加时间、保存到数据库和把审核数据赋值到target里。（6）判断批准是否为空，不为空时，保存批准数据、添加时间、保存到数据库和把批准数据赋值到target里。
        /// （7）把target 、现在时间（Datatime.now）ADD到quality里面,并输出quality。
        /// 4.方法（可能）有结果：第二步为空，输出false。第二步不为空，输出想要的数据。
        /// </summary>
        public ActionResult Editequipment_Quality(int year, int month, string remark, string preparName, string assessor, string approve)
        {
            JObject quality = new JObject();
            var target_list = db.Equipment_Quality_target.Where(c => c.Year == year && c.Month == month).ToList();//根据年月查询数据
            if (target_list.Count != 0)//判断target_list（查询数据）是否为0
            {
                string target = "";
                foreach (var item in target_list)//循环target_list（查询数据）
                {
                    if (!String.IsNullOrEmpty(remark))//判断备注是否为空
                    {
                        item.Remark = remark;//保存备注数据
                        db.SaveChanges();//保存到数据库
                        target = remark;//赋值
                    }
                    else if (!String.IsNullOrEmpty(preparName))//判断编制人是否为空
                    {
                        item.PrepareName = preparName;//保存编制人数据
                        item.PrepareTime = DateTime.Now;//添加时间
                        db.SaveChanges();//保存到数据库
                        target = preparName;//赋值
                    }
                    else if (!String.IsNullOrEmpty(assessor))//判断审核是否为空
                    {
                        item.Assessor = assessor;//保存审核数据
                        item.AssessedDate = DateTime.Now;//添加时间
                        db.SaveChanges();//保存到数据库
                        target = assessor;//赋值
                    }
                    else if (!String.IsNullOrEmpty(approve))//判断批准是否为空
                    {
                        item.Approve = approve;//保存批准数据
                        item.ApprovedDate = DateTime.Now;//添加时间
                        db.SaveChanges();//保存到数据库
                        target = approve;//赋值
                    }
                }
                quality.Add("quality", target);
                quality.Add("date", DateTime.Now);
                quality.Add("mes", true);
                return Content(JsonConvert.SerializeObject(quality));
            }
            quality.Add("mes", false);
            return Content(JsonConvert.SerializeObject(quality));
        }

        #endregion

        #region ---保存左边的数据（除去实际保养台次数值、计划保养总台次实际值数值、有效率百分比数值）
        ///<summary>
        /// 1.方法的作用：保存左边的数据
        /// 2.方法的参数和用法：inputList数组，remark备注，year年，month月，都用于保存数据。
        /// 3.方法的具体逻辑顺序，判断条件：(1)判断年月是否等于0,不等于0时，循环inputList数据，并添加编制时间和添加编制人。（2）根据使用部门、质量目标和年月查询数据，并判断是否大于0。
        /// （3）第二步大于0时add使用部门到date里、add质量目标到date里、add年到date里、add月到date里、把date里的数据add到depar、初始化date。（4）判断depar是否为空，不为空时，输出false，停止运行程序。
        /// （5）第二步为空时，第三步自然也就为空。（6）判断备注是否为空，不为空时，循环inputList，保存备注数据和保存到数据库。（7）判断年月是否为空，不为空时，循环inputList，保存年月和保存到数据库。
        /// （8）把数据保存到对应的表里，并把数据保存到数据库里。（9）判断savecount是否大于0（有没有把数据保存到数据库），大于0时，保存成功；等于0保存失败。
        /// 4.方法（可能）有结果：保存成功；保存失败。
        /// </summary>
        public ActionResult Equipment_QualityADD(List<Equipment_Quality_target> inputList, string remark, int year, int month)
        {
            if (year != 0 && month != 0)//判断年月是否等于0
            {
                JArray depar = new JArray();
                JObject date = new JObject();
                foreach (var item in inputList)//循环
                {
                    item.PrepareTime = DateTime.Now;//添加编制时间
                    item.PrepareName = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";//添加编制人
                    //根据使用部门、质量目标和年月查询数据，并判断是否大于0
                    if (db.Equipment_Quality_target.Count(c => c.UserDepartment == item.UserDepartment && c.Quality_objec == item.Quality_objec && c.Year == year && c.Month == month) > 0)
                    {
                        date.Add("UserDepartment", item.UserDepartment);//add使用部门到date里
                        date.Add("Quality_objec", item.Quality_objec);//add质量目标到date里
                        date.Add("Year", year);//add年到date里
                        date.Add("Month", month);//add月到date里
                        depar.Add(date);//把date里的数据add到depar
                        date = new JObject();//初始化date
                    }
                }
                JObject result = new JObject();
                if (depar.Count > 0)//判断depar是否为空
                {
                    result.Add("Date", false);
                    result.Add("Depar", depar);
                    return Content(JsonConvert.SerializeObject(result));
                }
                if (!String.IsNullOrEmpty(remark))//判断备注是否为空
                {
                    foreach (var it in inputList)//循环inputList
                    {
                        it.Remark = remark;//保存备注数据
                        db.SaveChanges();//保存到数据库
                    }
                }
                if (year != 0 && month != 0)//判断年月是否为空
                {
                    foreach (var it in inputList)//循环inputList
                    {
                        it.Year = year;//保存年
                        it.Month = month;//保存月
                        db.SaveChanges();//保存到数据库
                    }
                }
                db.Equipment_Quality_target.AddRange(inputList);//把数据保存到对应的表里
                int savecount = db.SaveChanges();//保存到数据库
                if (savecount > 0) //判断savecount是否大于0（有没有把数据保存到数据库）
                    return Content("true");
                else //savecount等于0（没有把数据保存到数据库或者保存出错）
                    return Content("false");
            }
            return Content("false");
        }
        #endregion

        #region ---修改左边的数据（除去实际保养台次数值、计划保养总台次实际值数值、有效率百分比数值）
        ///<summary>
        /// 1.方法的作用：修改左边的数据
        /// 2.方法的参数和用法：year年，month月，userdepartment使用部门，用于查询条件；quality_objec质量目标，target_value目标值，formulas计算公式，statistical统计周期，
        /// required_maintain项目：按规定要求保养台月次，planned_maintenance项目：计划保养总台月次，with_efficiency项目：有效率，用于修改。
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据年月、使用部门到Equipment_Quality_target表里查询数据。（2）判断quality（查询数据）是否大于0，大于0时，循环quality（查询数据）。
        /// （3）判断质量目标是否为空，不为空时，修改保存质量目标数据、保存到数据库和把质量目标数据赋值到equipment。（4）判断目标值是否为空，不为空时，修改保存目标值数据、保存到数据库和把目标值数据赋值到equipment。
        /// （5）判断计算公式是否为空，不为空时，修改保存计算公式数据、保存到数据库和把计算公式数据赋值到equipment。（6）统计周期、项目：按规定要求保养台月次、项目：计划保养总台月次、项目：有效率这几个数据也是跟第三、四、五步的逻辑顺序一样的。
        /// （7）添加修改时间、添加修改人，跳出循环quality（查询数据）。（8）把equipment add到table里、输出Json。
        /// 4.方法（可能）有结果：修改成功（第二步不为空），修改失败（第二步为空）。
        /// </summary>
        public ActionResult Modifi_Equipment_Quality(int year, int month, string userdepartment, string quality_objec, string target_value, string formulas, string statistical, string required_maintain, string planned_maintenance, string with_efficiency)
        {
            JObject table = new JObject();
            var quality = db.Equipment_Quality_target.Where(c => c.Year == year && c.Month == month && c.UserDepartment == userdepartment).ToList();//根据年月、使用部门查询数据
            if (quality.Count > 0)//判断quality（查询数据）是否大于0
            {
                string equipment = "";
                foreach (var item in quality)//循环quality（查询数据）
                {
                    if (!String.IsNullOrEmpty(quality_objec))//判断质量目标是否为空
                    {
                        item.Quality_objec = quality_objec;//修改保存质量目标数据
                        db.SaveChanges();//保存到数据库
                        equipment = quality_objec;//把质量目标数据赋值到equipment
                    }
                    if (!String.IsNullOrEmpty(target_value))//判断目标值是否为空
                    {
                        item.Target_value = target_value;//修改保存目标值数据
                        db.SaveChanges();//保存到数据库
                        equipment = target_value;//把目标值数据赋值到equipment
                    }
                    if (!String.IsNullOrEmpty(formulas))//判断计算公式是否为空
                    {
                        item.Formulas = formulas;//修改保存计算公式数据
                        db.SaveChanges();//保存到数据库
                        equipment = formulas;//把计算公式数据赋值到equipment
                    }
                    if (!String.IsNullOrEmpty(statistical))//判断统计周期是否为空
                    {
                        item.Statistical = statistical;//修改保存统计周期数据
                        db.SaveChanges();//保存到数据库
                        equipment = statistical;//把统计周期数据赋值到equipment
                    }
                    if (!String.IsNullOrEmpty(required_maintain))//判断项目：按规定要求保养台月次是否为空
                    {
                        item.Required_maintain = required_maintain;//修改保存项目：按规定要求保养台月次
                        db.SaveChanges();//保存到数据库
                        equipment = required_maintain;//把项目：按规定要求保养台月次数据赋值到equipment
                    }
                    if (!String.IsNullOrEmpty(planned_maintenance))//判断项目：计划保养总台月次是否为空
                    {
                        item.Planned_maintenance = planned_maintenance;//修改保存项目：计划保养总台月次
                        db.SaveChanges();//保存到数据库
                        equipment = planned_maintenance;//把项目：计划保养总台月次数据赋值到equipment
                    }
                    if (!String.IsNullOrEmpty(with_efficiency))//判断项目：有效率是否为空
                    {
                        item.With_efficiency = with_efficiency;//修改保存项目：有效率
                        db.SaveChanges();//保存到数据库
                        equipment = with_efficiency;//把项目：有效率数据赋值到equipment
                    }
                    item.ModifyTime = DateTime.Now;//添加修改时间
                    item.Modifier = ((Users)Session["user"]).UserName;//添加修改人
                }
                table.Add("quality", equipment);//把equipment add到table里
                table.Add("mes", true);
                return Content(JsonConvert.SerializeObject(table));//输出Json
            }
            table.Add("mes", false);
            return Content(JsonConvert.SerializeObject(table));
        }

        #endregion

        #endregion

        #region-----设备安全库存清单
        public ActionResult Equipment_safety()//安全库存清单查询页
        {

            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Equipment", act = "Equipment_safety" });
            }
            return View();
        }

        #region ---下拉框检索数据库有数据的使用部门，设备名称，配料料号，品名，规格/型号
        ///<summary>
        /// 1.方法的作用：下拉框检索数据库有数据的使用部门
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到Equipment_Safetystock表里按照ID的排序顺序查询所有使用部门并去重。
        /// 4.方法（可能）有结果：输出查询数据。
        /// </summary>
        public ActionResult Getsafety_department()//使用部门
        {
            var department = db.Equipment_Safetystock.OrderByDescending(m => m.Id).Select(m => m.UserDepartment).Distinct();
            return Content(JsonConvert.SerializeObject(department));
        }

        ///<summary>
        /// 1.方法的作用：下拉框检索数据库有数据的设备名称
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到Equipment_Safetystock表里按照ID的排序顺序查询所有设备名称并去重。
        /// 4.方法（可能）有结果：输出查询数据。
        /// </summary>
        public ActionResult Getsafety_equipmentName()//设备名称
        {
            var equipment = db.Equipment_Safetystock.OrderByDescending(m => m.Id).Select(m => m.EquipmentName).Distinct();
            return Content(JsonConvert.SerializeObject(equipment));
        }

        ///<summary>
        /// 1.方法的作用：下拉框检索数据库有数据的配料料号
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到Equipment_Safetystock表里按照ID的排序顺序查询所有‘配料料号’并去重。
        /// 4.方法（可能）有结果：输出查询数据。
        /// </summary>
        public ActionResult Getsafety_material()//配料料号
        {
            var material = db.Equipment_Safetystock.OrderByDescending(m => m.Id).Select(m => m.Material).Distinct();
            return Content(JsonConvert.SerializeObject(material));
        }

        ///<summary>
        /// 1.方法的作用：下拉框检索数据库有数据的品名
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到Equipment_Safetystock表里按照ID的排序顺序查询所有‘品名’并去重。
        /// 4.方法（可能）有结果：输出查询数据
        /// </summary>
        public ActionResult Getsafety_descrip()//品名
        {
            var descrip = db.Equipment_Safetystock.OrderByDescending(m => m.Id).Select(m => m.Descrip).Distinct();
            return Content(JsonConvert.SerializeObject(descrip));
        }

        ///<summary>
        /// 1.方法的作用：下拉框检索数据库有数据的规格/型号
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到Equipment_Safetystock表里按照ID的排序顺序查询所有‘规格/型号’并去重。
        /// 4.方法（可能）有结果：输出查询数据
        /// </summary>
        public ActionResult Getsafety_specifica()//规格/型号
        {
            var speci = db.Equipment_Safetystock.OrderByDescending(m => m.Id).Select(m => m.Specifica).Distinct();
            return Content(JsonConvert.SerializeObject(speci));
        }
        #endregion

        #region---按使用部门，设备名称，品名，规格/型号，配料料号查询数据
        ///<summary>
        /// 1.方法的作用：根据使用部门，设备名称，品名，规格/型号，配料料号查询数据
        /// 2.方法的参数和用法：userdepartment（使用部门），equipmentName（设备名称），descrip（品名），specifica（规格/型号），material（配料料号）；用于查询数据
        /// 3.方法的具体逻辑顺序，判断条件：（1）获取整个表的数据（security）。（2）判断使用部门是否为空,不为空时根据使用部门查询数据。（3）判断设备名称是否为空，不为空时根据设备名称查询数据。
        /// （4）判断品名是否为空,不为空时根据品名查询数据。（5）判断规格/型号是否为空,不为空时根据规格/型号查询数据。（6）判断配料料号是否为空，不为空时根据配料料号查询数据。
        /// （6）根据配料料号到ERP的数据库查询数据大于0的仓库存料（现有库存量）。（7）循环数据（security）：第六步查询出来的数据。（8）查找ERP的数据库里的配料料号等于本地数据（Equipment_Safetystock）里的配料料号的数据。
        /// （9）判断res(查找出来的数据：第八步)等于null，等于null时，仓库存料（现有库存量）直接等于0。（10）res(查找出来的数据)不等于null时，仓库存料（现有库存量）等于第八步查询出来的数据的仓库存料（现有库存量）和。
        /// （11）res(查找出来的数据)不等于null时，查找现有库存详情数据（物料号、仓库、库位、批号、现有库存量、有效期）。
        /// 4.方法（可能）有结果：输出查询出来的结果（可能会有null）
        /// </summary>
        public ActionResult Safetyquery(string userdepartment, string equipmentName, string descrip, string specifica, string material)
        {
            var security = db.Equipment_Safetystock.ToList();//获取数据
            if (!String.IsNullOrEmpty(userdepartment))//判断使用部门是否为空
            {
                security = security.Where(c => c.UserDepartment == userdepartment).ToList();//根据使用部门查询数据
            }
            if (!String.IsNullOrEmpty(equipmentName))//判断设备名称是否为空
            {
                security = security.Where(c => c.EquipmentName == equipmentName).ToList();//根据设备名称查询数据
            }
            if (!String.IsNullOrEmpty(descrip))//判断品名是否为空
            {
                security = security.Where(c => c.Descrip == descrip).ToList();//根据品名查询数据
            }
            if (!String.IsNullOrEmpty(specifica))//判断规格/型号是否为空
            {
                security = security.Where(c => c.Specifica == specifica).ToList();//根据规格/型号查询数据
            }
            if (!String.IsNullOrEmpty(material))//判断配料料号是否为空
            {
                security = security.Where(c => c.Material == material).ToList();//根据配料料号查询数据
            }
            var querylist = CommonERPDB.ERP_Query_SafetyStock(security.Select(c => c.Material).ToList()).Where(c => c.img10 > 0);//根据配料料号到ERP的数据库查询数据大于0的仓库存料（现有库存量）
            foreach (var item in security)//循环数据（security）
            {
                var res = querylist.Where(c => c.img01 == item.Material).ToList();//查找ERP的数据库里的配料料号等于本地数据（Equipment_Safetystock）里的配料料号的数据
                if (res == null)//res(查找出来的数据)等于null
                {
                    item.Existing_inventory = null;//仓库存料（现有库存量）等于0
                }
                else //res(查找出来的数据)不等于null
                {
                    item.Existing_inventory = querylist.Where(c => c.img01 == item.Material).Sum(c => c.img10);//仓库存料（现有库存量）等于ERP的数据库里的配料料号和本地数据库的配料料号的仓库存料（现有库存量）和
                    item.Existing_inventory_Details = JsonConvert.SerializeObject(res.Select(c => new { c.img01, c.ima02, c.ima021, c.img02, c.img03, c.img04, c.img10, c.img18 }).ToList());//查找现有库存详情数据（物料号、仓库、库位、批号、现有库存量、有效期）
                }
            }
            return Content(JsonConvert.SerializeObject(security));
        }

        #endregion

        #region ---批量上传安全库存清单
        ///<summary>
        /// 1.方法的作用：批量上传安全库存清单
        /// 2.方法的参数和用法：inputList（数组），year年，month月，userdepartment使用部门
        /// 3.方法的具体逻辑顺序，判断条件：(1)判断年月和使用部门是否为空。（2）循环数据inputList，并添加资料整理时间和添加资料整理人。（3）根据使用部门、设备名称、年月和配料料号查找数据，并判断查找出来的数据是否大于0。
        /// （4）第三步如果大于0，把配料料号和设备名称赋值到repat里面。（5）判断repat数据（第四步）是否为空，不为空时直接输出数据。（6）循环数据inputList，并把使用部门，年月保存到inputList里面。
        /// （7）把数据保存到相对应的表里，保存到数据库。（8）判断savecount是否大于0（有没有把数据保存到数据库），大于0就是添加成功，等于0（没有把数据保存到数据库或者保存出错）就是添加失败。
        /// 4.方法（可能）有结果：第一步为空时，第三步大于0和第八步等于0批量上传失败；第一步不为空时，第三步等于0和第八步大于0批量上传成功
        /// </summary>
        public ActionResult ADDsafestock(List<Equipment_Safetystock> inputList, int year, int month, string userdepartment)
        {
            if (year != 0 && month != 0 && userdepartment != null)//判断年月和使用部门是否为空
            {
                JArray res = new JArray();
                string repat = "";
                foreach (var item in inputList)//循环数据inputList
                {
                    item.FinishingDate = DateTime.Now;//添加资料整理时间
                    item.FinishingName = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";//添加资料整理人
                    //根据使用部门、设备名称、年月和配料料号查找数据，并判断查找出来的数据是否大于0
                    if (db.Equipment_Safetystock.Count(c => c.UserDepartment == userdepartment && c.Year == year && c.Month == month && c.EquipmentName == item.EquipmentName && c.Material == item.Material) > 0)
                    {
                        repat = item.Material;//赋值
                        repat = item.EquipmentName;//赋值
                        res.Add(repat);//add
                    }
                }
                JObject result = new JObject();
                if (!String.IsNullOrEmpty(repat))//判断repat数据是否为空
                {
                    return Content(JsonConvert.SerializeObject(res));//输出
                }
                foreach (var it in inputList)//循环数据inputList
                {
                    it.UserDepartment = userdepartment;//赋值
                    it.Year = year;//赋值
                    it.Month = month;//赋值
                    db.SaveChanges();//保存
                }
                db.Equipment_Safetystock.AddRange(inputList);//把数据保存到相对应的表里
                int savecount = db.SaveChanges();//保存到数据库
                if (savecount > 0) //判断savecount是否大于0（有没有把数据保存到数据库）
                    return Content("true");
                else //savecount等于0（没有把数据保存到数据库或者保存出错）
                    return Content("false");
            }
            return Content("false");
        }
        #endregion

        #region ---编辑修改安全库存清单
        ///<summary>
        /// 1.方法的作用：修改安全库存清单
        /// 2.方法的参数和用法：equipment_Safetystock（所有字段），用于修改数据。
        /// 3.方法的具体逻辑顺序，判断条件：(1)判断使用部门，年月，设备名称，配料料号是否为空。（2）添加修改时间、添加修改人、修改数据和把修改好的数据保存到数据库。
        /// （3）判断savecount是否大于0（有没有把数据保存到数据库），大于0时修改成功；等于0（没有把数据保存到数据库或者保存出错）时，修改失败。
        /// 4.方法（可能）有结果：第一步为空，第三步等于0，修改失败；第一步不为空，第三步大于0，修改成功。
        /// </summary>
        public ActionResult Modifi_safety(Equipment_Safetystock equipment_Safetystock)
        {
            JObject Safety = new JObject();
            //判断使用部门，年月，设备名称，配料料号是否为空
            if (equipment_Safetystock.UserDepartment != null && equipment_Safetystock.Year != 0 && equipment_Safetystock.Month != 0 && equipment_Safetystock.EquipmentName != null && equipment_Safetystock.Material != null)
            {
                equipment_Safetystock.ModifyTime = DateTime.Now;//添加修改时间
                equipment_Safetystock.Modifier = ((Users)Session["user"]).UserName;//添加修改人
                db.Entry(equipment_Safetystock).State = EntityState.Modified;//修改数据
                var savecount = db.SaveChanges();//把修改好的数据保存到数据库
                if (savecount > 0) //判断savecount是否大于0（有没有把数据保存到数据库）
                {
                    Safety.Add("Safety", true);
                    Safety.Add("equipment_Safetystock", JsonConvert.SerializeObject(equipment_Safetystock));
                    return Content(JsonConvert.SerializeObject(Safety));
                }
                else //savecount等于0（没有把数据保存到数据库或者保存出错）
                {
                    Safety.Add("Safety", false);
                    Safety.Add("equipment_Safetystock", null);
                    return Content(JsonConvert.SerializeObject(Safety));
                }
            }
            return Content("false");
        }
        #endregion

        #region ---审核确认安全库存清单
        ///<summary>
        /// 1.方法的作用：审核确认安全库存清单
        /// 2.方法的参数和用法：userdepartment（使用部门），year（年），month（月），用于查询；tec_asse（技术部审核），mcdepsr（MC部审核），approve（工厂厂长批准），用于修改数据。
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据年月到Equipment_Safetystock表里查询数据相对应的数据。（2）判断第一步查找的数据是否为空，不为空时循环stock（查询数据）。
        /// （3）判断技术部审核数据是否为空，不为空时保存技术部审核数据、添加时间、保存到数据库和把技术部审核数据赋值到depar。（4）判断MC部审核数据是否为空，不为空时保存MC部审核数据，添加时间，保存到数据库和把MC部审核数据赋值到depar。
        /// （5）判断工厂厂长批准数据是否为空，不为空时保存工厂厂长批准数据，添加时间，保存到数据库和把工厂厂长批准数据赋值到depar。（6）add depar和输出数据。
        /// 4.方法（可能）有结果：输出查询出来的数据
        /// </summary>
        public ActionResult Verification(string userdepartment, int year, int month, string tec_asse, string mcdepsr, string approve)
        {
            JObject Safety = new JObject();
            var stock = db.Equipment_Safetystock.Where(c => c.Year == year && c.Month == month).ToList();//根据年月查询对应数据
            if (stock != null)//判断stock（查询数据）是否为空
            {
                string depar = null;
                foreach (var item in stock)//循环stock（查询数据）
                {
                    if (!String.IsNullOrEmpty(tec_asse))//判断技术部审核数据是否为空
                    {
                        item.Tec_Assessor = tec_asse;//保存技术部审核数据
                        item.Tec_AssessedDate = DateTime.Now;//添加时间
                        db.SaveChanges();//保存到数据库

                        depar = tec_asse;//赋值
                    }
                    else if (!String.IsNullOrEmpty(mcdepsr))//判断MC部审核数据是否为空
                    {
                        item.Assessor = mcdepsr; //保存MC部审核数据
                        item.AssessedDate = DateTime.Now;//添加时间
                        db.SaveChanges();//保存到数据库
                        depar = mcdepsr;//赋值
                    }
                    else if (!String.IsNullOrEmpty(approve))//判断工厂厂长批准数据是否为空
                    {
                        item.Approve = approve;//保存工厂厂长批准数据
                        item.ApprovedDate = DateTime.Now;//添加时间
                        db.SaveChanges();//保存到数据库
                        depar = approve;//赋值
                    }
                }
                Safety.Add("Safety", true);
                Safety.Add("depar", depar);
                return Content(JsonConvert.SerializeObject(Safety));
            }
            return Content("false");
        }

        #endregion

        #endregion

        #region---设备关键元器件清单汇总
        public ActionResult Equipment_Keycomponents()
        {
            return View();
        }

        #region---根据‘设备编号’查询关键元器件清单
        ///<summary>
        /// 1.方法的作用：查询关键元器件清单
        /// 2.方法的参数和用法：equipmentNumber设备编号，用于查询
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据设备编号到Equipment_keycomponents表查询对应的数据。（2）判断component（查询出来的数据）是否大于0。
        /// （3）第二步大于时，循环component（第一步），页面需要显示的数据ADD到table里（数据有：ID，设备名称，设备编号，品名，规格/型号，用途，备注）。（4）table里的数据add到keycom里，并把table初始化。
        /// 4.方法（可能）有结果：输出查询结果（有可能为空）
        /// </summary>
        public ActionResult Keyinquire(string equipmentNumber)
        {
            JObject table = new JObject();
            JArray keycom = new JArray();
            var component = db.Equipment_keycomponents.Where(c => c.EquipmentNumber == equipmentNumber).ToList();//根据设备编号查询对应的数据
            if (component.Count > 0)//判断component（查询出来的数据）是否大于0
            {
                foreach (var item in component)//循环component（查询出来的数据）
                {
                    //ID
                    table.Add("Id", component.Count == 0 ? 0 : item.Id);
                    //设备名称
                    table.Add("EquipmentName", component.Count == 0 ? null : item.EquipmentName);
                    //设备编号
                    table.Add("EquipmentNumber", component.Count == 0 ? null : item.EquipmentNumber);
                    //品名
                    table.Add("Descrip", component.Count == 0 ? null : item.Descrip);
                    //规格/型号
                    table.Add("Specifica", component.Count == 0 ? null : item.Specifica);
                    //用途
                    table.Add("Materused", component.Count == 0 ? null : item.Materused);
                    //备注
                    table.Add("Remark", component.Count == 0 ? null : item.Remark);
                    keycom.Add(table);
                    table = new JObject();
                }
            }
            return Content(JsonConvert.SerializeObject(keycom));
        }
        #endregion

        #region---批量上传关键元器件清单
        ///<summary>
        /// 1.方法的作用：批量上传
        /// 2.方法的参数和用法：inputList
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断inputList和设备编号是否为空。（2）第一步不为空时，循环inputList，并添加制表时间和添加制表人。（3）根据设备编号，品名，规格/型号，用途查询数据，并判断是否大于0。
        /// （4）第三步不为空时，把设备编号、品名、规格/型号和用途add到repat里，把repat add到res里，并把repat初始化。（5）判断res是否为空，不为空，直接输出false，停止运行。（6）第三步等于0时，第五步自然而然的为空。
        /// （6）把数据保存到对应的数据表里和保存到数据库。（7）判断savecount是否大于0（有没有把数据保存到数据库），大于0，批量上传成功；等于0（没有把数据保存到数据库或者保存出错），批量上传失败。
        /// 4.方法（可能）有结果：批量上传成功，批量上传失败。
        /// </summary>
        public ActionResult Keycomponents_query(List<Equipment_keycomponents> inputList)
        {
            if (inputList != null && inputList.FirstOrDefault().EquipmentNumber != null)//判断inputList和设备编号是否为空
            {
                JArray res = new JArray();
                JObject repat = new JObject();
                foreach (var item in inputList)//循环inputList
                {
                    item.TabulationTime = DateTime.Now;//添加制表时间
                    item.Mainten_Lister = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";//添加制表人
                    //根据设备编号，品名，规格/型号，用途查询数据，并判断是否大于0
                    if (db.Equipment_keycomponents.Count(c => c.EquipmentNumber == item.EquipmentNumber && c.Descrip == item.Descrip && c.Specifica == item.Specifica && c.Materused == item.Materused) > 0)
                    {
                        repat.Add("EquipmentNumber", item.EquipmentNumber);//把设备编号add到repat里
                        repat.Add("Descrip", item.Descrip);//品名
                        repat.Add("Specifica", item.Specifica);//规格/型号
                        repat.Add("Materused", item.Materused);//用途
                        res.Add(repat);//把repat add到res里
                        repat = new JObject();//初始化repat
                    }
                }
                JObject result = new JObject();
                if (res.Count > 0)//判断res是否为空
                {
                    result.Add("repat", false);
                    result.Add("res", res);
                    return Content(JsonConvert.SerializeObject(result));
                }
                db.Equipment_keycomponents.AddRange(inputList);//把数据保存到对应的数据表里
                int savecount = db.SaveChanges();//保存到数据库
                if (savecount > 0)//判断savecount是否大于0（有没有把数据保存到数据库）
                    return Content("true");
                else  //savecount等于0（没有把数据保存到数据库或者保存出错）
                    return Content("false");
            }
            return Content("false");
        }
        #endregion

        #region---修改关键元器件清单
        ///<summary>
        /// 1.方法的作用：修改关键元器件清单
        /// 2.方法的参数和用法：equipmentNumber设备编号，id，用于查询，descrip品名，specifica规格/型号，materused用途，remark备注，用于修改
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据ID和设备编号到Equipment_keycomponents表查询相对应的数据。（2）判断componlist（查询数据）是否为空。（3）第二步不为空时，循环componlist（查询数据），并添加修改时间和添加修改人。
        /// （4）判断品名是否为空，不为空时，修改保存品名、保存到数据库和把品名赋值到componet。（5）判断规格/型号是否为空，不为空时，修改保存规格/型号、保存到数据库和把规格/型号赋值到componet。
        /// （6）判断用途是否为空，不为空时，修改保存用途、保存到数据库和把用途赋值到componet。（7）判断备注是否为空，不为空时，修改保存备注、保存到数据库和把备注赋值到componet。
        /// 4.方法（可能）有结果：修改成功（第二步不为空时，第四、五、六、七步有一步不为空）；修改失败（第二、四、五、六、七步都为空）。
        /// </summary>
        public ActionResult Equipment_EditComponet(string equipmentNumber, string descrip, string specifica, string materused, string remark, int id)
        {
            JObject keycompont = new JObject();
            //根据ID和设备编号查询相对应的数据
            var componlist = db.Equipment_keycomponents.Where(c => c.Id == id && c.EquipmentNumber == equipmentNumber).ToList();
            if (componlist.Count > 0)//判断componlist（查询数据）是否为空
            {
                string componet = null;
                foreach (var item in componlist)//循环componlist（查询数据）
                {
                    if (!String.IsNullOrEmpty(descrip))//判断品名是否为空
                    {
                        item.Descrip = descrip;//修改保存品名
                        db.SaveChanges();//保存到数据库
                        componet = descrip;//把品名赋值到componet
                    }
                    if (!String.IsNullOrEmpty(specifica))//判断规格/型号是否为空
                    {
                        item.Specifica = specifica;//修改保存规格/型号
                        db.SaveChanges();//保存到数据库
                        componet = specifica;//把规格/型号赋值到componet
                    }
                    if (!String.IsNullOrEmpty(materused))//判断用途是否为空
                    {
                        item.Materused = materused;//修改保存用途
                        db.SaveChanges();//保存到数据库
                        componet = materused;//把用途赋值到componet
                    }
                    if (!String.IsNullOrEmpty(remark))//判断备注是否为空
                    {
                        item.Remark = remark;//修改保存备注
                        db.SaveChanges();//保存到数据库
                        componet = remark;//把备注赋值到componet
                    }
                    item.ModifyTime = DateTime.Now;//添加修改时间
                    item.Modifier = ((Users)Session["user"]).UserName;//添加修改人
                }
                keycompont.Add("mes", true);
                keycompont.Add("componet", componet);
                return Content(JsonConvert.SerializeObject(keycompont));
            }
            return Content("false");
        }

        #endregion

        #region---删除关键元器件
        ///<summary>
        /// 1.方法的作用：删除关键元器件数据
        /// 2.方法的参数和用法：id，用于查询
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据ID到Equipment_keycomponents表里查询数据。（2）添加删除操作时间、添加删除操作人和添加操作记录（如：张三在2020年2月26日删除设备关键元器件为李四的记录）。
        /// （3）删除对应的数据（第一步查询出来的数据）。（4）添加删除操作日记数据（第二步的数据）。(5)保存到数据库，并判断count是否大于0（有没有把数据保存到数据库），大于0，删除成功；等于0（没有把数据保存到数据库或者保存出错），删除失败。
        /// 4.方法（可能）有结果：删除成功；删除失败。
        /// </summary>
        public ActionResult DeleteKeycom(int id)
        {
            var record = db.Equipment_keycomponents.Where(c => c.Id == id).FirstOrDefault();//根据ID查询数据
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;//添加删除操作时间
            operaterecord.Operator = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";//添加删除操作人
            //添加操作记录（如：张三在2020年2月26日删除设备关键元器件为李四的记录）
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除设备关键元器件为" + record.Mainten_Lister + "的记录。";
            db.Equipment_keycomponents.Remove(record);//删除对应的数据
            db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
            int count = db.SaveChanges();//保存
            if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
                return Content("true");
            else //countt等于0（没有把数据保存到数据库或者保存出错）
                return Content("false");
        }
        #endregion

        #endregion

        #region---设备稼动率（总设备运行状态图形页面）
        public ActionResult Equipment_Rateof_grain()
        {
            return View();
        }

        #region---首页汇总
        public ActionResult Equipment_Thecurrent()
        {
            JObject table = new JObject();
            JObject Equi_data = new JObject();
            JObject EquipmentList = new JObject();
            JObject current = new JObject();
            JArray Run = new JArray();
            JArray Downtime = new JArray();
            JArray Mainten = new JArray();
            JArray Factory_current = new JArray();
            double Runnumber = 0;//部门运行数量
            double Stopnumber = 0;//部门停机数量
            double Maintnumber = 0;//部门维修数量
            double Run_Depar = 0;//部门运行百分比
            double Stop_Depar = 0;//部门停机百分比
            double Maint_Depar = 0;//部门维修百分比
            double Runtotal = 0;//全厂运行数量
            double Stoptotal = 0;//全厂停机数量
            double Mainttotal = 0;//全厂维修数量
            double Runpercentage = 0;//全厂运行百分比
            double Stoppercentage = 0;//全厂停机百分比
            double Maintpercentage = 0;//全厂维修百分比
            var departmentList = db.EquipmentSetStation.Select(c => c.UserDepartment).Distinct().ToList();//把所有部门都查找出来并去重
            if (departmentList.Count > 0)
            {
                foreach (var item in departmentList)
                {
                    var statue = db.EquipmentSetStation.Where(c => c.UserDepartment == item).Select(c => new { c.Id, c.UserDepartment, c.LineNum, c.EquipmentNumber, c.AssetNumber, c.EquipmentName, c.Status }).ToList();
                    if (statue.Count > 0)
                    {
                        foreach (var ite in statue)
                        {
                            if (ite.Status == "运行")
                            {
                                Runnumber = statue.Count(c => c.UserDepartment == ite.UserDepartment && c.Status == ite.Status);
                                //ID
                                current.Add("Id", ite.Id);
                                //使用部门
                                current.Add("UserDepartment", ite.UserDepartment);
                                //线别
                                current.Add("LineNum", ite.LineNum);
                                //设备编号
                                current.Add("EquipmentNumber", ite.EquipmentNumber);
                                //资产编号
                                current.Add("AssetNumber", ite.AssetNumber);
                                //设备名称
                                current.Add("EquipmentName", ite.EquipmentName);
                                //设备状态
                                current.Add("Status", ite.Status);
                                Run.Add(current);
                                current = new JObject();
                            }
                            else if (ite.Status == "停机")
                            {
                                Stopnumber = statue.Count(c => c.UserDepartment == ite.UserDepartment && c.Status == ite.Status);
                                //ID
                                current.Add("Id", ite.Id);
                                //使用部门
                                current.Add("UserDepartment", ite.UserDepartment);
                                //线别
                                current.Add("LineNum", ite.LineNum);
                                //设备编号
                                current.Add("EquipmentNumber", ite.EquipmentNumber);
                                //资产编号
                                current.Add("AssetNumber", ite.AssetNumber);
                                //设备名称
                                current.Add("EquipmentName", ite.EquipmentName);
                                //设备状态
                                current.Add("Status", ite.Status);
                                Downtime.Add(current);
                                current = new JObject();
                            }
                            else if (ite.Status == "维修")
                            {
                                Maintnumber = statue.Count(c => c.UserDepartment == ite.UserDepartment && c.Status == ite.Status);
                                //ID
                                current.Add("Id", ite.Id);
                                //使用部门
                                current.Add("UserDepartment", ite.UserDepartment);
                                //线别
                                current.Add("LineNum", ite.LineNum);
                                //设备编号
                                current.Add("EquipmentNumber", ite.EquipmentNumber);
                                //资产编号
                                current.Add("AssetNumber", ite.AssetNumber);
                                //设备名称
                                current.Add("EquipmentName", ite.EquipmentName);
                                //设备状态
                                current.Add("Status", ite.Status);
                                Mainten.Add(current);
                                current = new JObject();
                            }
                        }
                        Run_Depar = (Runnumber * 100 / (Runnumber + Stopnumber + Maintnumber));//部门运行百分比
                        Stop_Depar = (Stopnumber * 100 / (Runnumber + Stopnumber + Maintnumber));//部门停机百分比
                        Maint_Depar = (Maintnumber * 100 / (Runnumber + Stopnumber + Maintnumber));//部门维修百分比
                        Runtotal = Runtotal + Runnumber;//计算运行总数量                             
                        Stoptotal = Stoptotal + Stopnumber;//计算停机总数量
                        Mainttotal = Mainttotal + Maintnumber;//计算维修总数量
                        Runpercentage = (Runtotal * 100 / (Runtotal + Stoptotal + Mainttotal));//运行总百分比
                        Stoppercentage = (Stoptotal * 100 / (Runtotal + Stoptotal + Mainttotal));//停机总百分比
                        Maintpercentage = ((Mainttotal * 100) / (Runtotal + Stoptotal + Mainttotal));//维修总百分比
                    }
                    Equi_data.Add("UserDepartment", item);//使用部门     
                    table.Add("Runnumber", Runnumber);//运行数量
                    Runnumber = new int();
                    table.Add("Run_Depar", Run_Depar.ToString("0.00") + "%");//部门运行百分比
                    table.Add("Run", Run);//运行
                    Run = new JArray();
                    table.Add("Stopnumber", Stopnumber);//停机数量
                    Stopnumber = new int();
                    table.Add("Stop_Depar", Stop_Depar.ToString("0.00") + "%");//部门停机百分比
                    table.Add("Downtime", Downtime);//停机
                    Downtime = new JArray();
                    table.Add("Maintnumber", Maintnumber);//维修数量
                    Maintnumber = new int();
                    table.Add("Maint_Depar", Maint_Depar.ToString("0.00") + "%");//部门维修百分比
                    table.Add("Mainten", Mainten);//维修
                    Mainten = new JArray();
                    Equi_data.Add("Shuju", table);//数据
                    table = new JObject();
                    Factory_current.Add(Equi_data);
                    Equi_data = new JObject();
                }
            }
            EquipmentList.Add("Runtotal", Runtotal);//运行总数量
            EquipmentList.Add("Runpercentage", Runpercentage.ToString("0.00") + "%");//运行总百分比
            EquipmentList.Add("Stoptotal", Stoptotal);//停机总数量
            EquipmentList.Add("Stoppercentage", Stoppercentage.ToString("0.00") + "%");//停机总百分比
            EquipmentList.Add("Mainttotal", Mainttotal);//维修总数量
            EquipmentList.Add("Maintpercentage", Maintpercentage.ToString("0.00") + "%");//维修总百分比
            EquipmentList.Add("Factory_current", Factory_current);
            Factory_current = new JArray();
            return Content(JsonConvert.SerializeObject(EquipmentList));
        }

        #endregion

        #region---设备的状态时长（运行/停机/维修）汇总
        public ActionResult Equipment_Duration()
        {
            return View();
        }
        public ActionResult Equipment_DeviceTime(int? year, int? month)
        {
            JObject equipmentList = new JObject();
            JObject table = new JObject();
            JObject pment = new JObject();
            JObject duration = new JObject();
            JArray devicetime = new JArray();
            JArray user = new JArray();
            JObject ainten = new JObject();
            JArray current = new JArray();
            double Downtime = 0;//个设备停机时长
            double Stop_percen = 0;//个设备停机时长百分比
            double Runtime = 0;//个设备运行时长
            double Runtime_percen = 0;//个设备运行时长百分比
            double Maintentime = 0;//个设备维修时长
            double Maintentime_percen = 0;//个设备维修时长百分比
            double Anonymous = 0;//个设备无记录时长
            double Anonymous_percen = 0;//个设备无记录时长百分比
            double RunnDevice = 0;//部门运行时长
            double StopnDevice = 0;//部门停机时长
            double MaintDevice = 0;//部门维修时长
            double AnonyDevice = 0;//部门无记录时长
            double RunDepar_Device = 0;//部门运行时长百分比
            double StopDepar_Device = 0;//部门停机时长百分比
            double MaintDepar_Device = 0;//部门维修时长百分比
            double AnonyDepar_Device = 0;//部门无记录时长百分比
            double RuntotalDevice = 0;//全厂运行时长
            double StoptotalDevice = 0;//全厂停机时长
            double MainttotalDevice = 0;//全厂维修时长
            double AnonytotalDevice = 0;//全厂无记录时长
            double Runpercentage_Device = 0;//全厂运行时长百分比
            double Stoppercentage_Device = 0;//全厂停机时长百分比
            double Maintpercentage_Device = 0;//全厂维修时长百分比 
            double Anonypercentage_Device = 0;//全厂无记录时长百分比
            double houses = 0;//天数*24
            double Deparequipment = 0;//部门设备数量
            double EquipmentList = 0;//全厂设备数量
            DateTime time = DateTime.Now;//获取当前时间
            List<EquipmentStateTime> dateList = new List<EquipmentStateTime>();
            var deparlist = db.EquipmentStateTime.Select(c => c.UserDepartment).Distinct().ToList();
            if (deparlist.Count > 0)
            {
                foreach (var ite in deparlist)
                {
                    Deparequipment = db.EquipmentSetStation.Count(c => c.UserDepartment == ite);//根据部门找设备台数
                    EquipmentList = db.EquipmentSetStation.Count();
                    if (ite != null && ite != "null")
                    {
                        if (ite != null && year != null && month != null)
                        {
                            if (time.Year == year && time.Month == month)
                            {
                                houses = time.Day * 24;//月份的天数*24 
                            }
                            else
                            {
                                houses = DateTime.DaysInMonth(year.Value, month.Value) * 24;//月份的天数*24 
                            }
                            var p = db.EquipmentStateTime.Where(c => c.UserDepartment == ite && c.StatusStarTime.Value.Year == year && c.StatusStarTime.Value.Month <= month && c.StatusEndTime == null).ToList();
                            var k = db.EquipmentStateTime.Where(c => c.UserDepartment == ite && c.StatusStarTime.Value.Year == year && c.StatusStarTime.Value.Month <= month && c.StatusEndTime.Value.Month >= month).ToList();
                            dateList = p.Concat(k).ToList();
                        }
                        else if (ite != null && year != null && month == null)
                        {
                            if (time.Year == year)
                            {
                                houses = time.DayOfYear * 24;//月份的天数*24 
                            }
                            else
                            {
                                houses = (DateTime.IsLeapYear(year.Value) == true ? 366 : 365) * 24;//年份的天数*24
                            }
                            dateList = db.EquipmentStateTime.Where(c => c.UserDepartment == ite && c.StatusStarTime.Value.Year == year && c.StatusEndTime.Value.Year == year).ToList();
                        }
                        if (dateList.Count > 0)
                        {
                            var stuta = dateList.Select(c => c.EquipmentNumber).Distinct().ToList();
                            foreach (var iu in stuta)  //foreach设备编号清单
                            {
                                var number = dateList.Where(c => c.EquipmentNumber == iu).ToList();
                                foreach (var item in number)  //foreach每个设备编号的记录，求设备的运行、停机、维修、无记录时长
                                {
                                    DateTime? dt1 = item.StatusStarTime;//把DateTime?类型转换成DateTime类型(开始时间)
                                    DateTime? dt2 = item.StatusEndTime;//把DateTime?类型转换成DateTime类型（结束时间）   
                                    DateTime begintime = default(DateTime);
                                    if (year != null && month != null)
                                    {
                                        begintime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1, 0, 0, 0);
                                    }
                                    else if (year != null && month == null)
                                    {
                                        begintime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(1), 1, 0, 0, 0);
                                    }
                                    if (item.Status == "停机")//设备状态等于“停机”
                                    {
                                        double Thours = 0;
                                        if (dt1 < begintime)
                                        {
                                            if (item.StatusEndTime == null)//设备状态的结束时间为空
                                            {
                                                Thours = (time - begintime).TotalHours;
                                            }
                                            else
                                            {
                                                Thours = (Convert.ToDateTime(dt2) - begintime).TotalHours;
                                            }
                                        }
                                        else
                                        {
                                            if (item.StatusEndTime == null)
                                            {
                                                Thours = (time - Convert.ToDateTime(dt1)).TotalHours;
                                            }
                                            else
                                            {
                                                Thours = (Convert.ToDateTime(dt2) - Convert.ToDateTime(dt1)).TotalHours;
                                            }
                                        }
                                        Downtime = Downtime + Thours;
                                    }
                                    if (item.Status == "运行")//判断设备状态是否等于“运行”
                                    {
                                        double Thours = 0;
                                        if (dt1 < begintime)
                                        {
                                            if (item.StatusEndTime == null)//设备状态的结束时间为空
                                            {
                                                Thours = (time - begintime).TotalHours;
                                            }
                                            else
                                            {
                                                Thours = (Convert.ToDateTime(dt2) - begintime).TotalHours;
                                            }
                                        }
                                        else
                                        {
                                            if (item.StatusEndTime == null)
                                            {
                                                Thours = (time - Convert.ToDateTime(dt1)).TotalHours;
                                            }
                                            else
                                            {
                                                Thours = (Convert.ToDateTime(dt2) - Convert.ToDateTime(dt1)).TotalHours;
                                            }
                                        }
                                        Runtime = Runtime + Thours;
                                    }
                                    if (item.Status == "维修")//判断设备状态是否等于“维修”
                                    {
                                        double Thours = 0;
                                        if (dt1 < begintime)
                                        {
                                            if (item.StatusEndTime == null)//设备状态的结束时间为空
                                            {
                                                Thours = (time - begintime).TotalHours;
                                            }
                                            else
                                            {
                                                Thours = (Convert.ToDateTime(dt2) - begintime).TotalHours;
                                            }
                                        }
                                        else
                                        {
                                            if (item.StatusEndTime == null)
                                            {
                                                Thours = (time - Convert.ToDateTime(dt1)).TotalHours;
                                            }
                                            else
                                            {
                                                Thours = (Convert.ToDateTime(dt2) - Convert.ToDateTime(dt1)).TotalHours;
                                            }
                                        }
                                        Maintentime = Maintentime + Thours;
                                    }
                                    if (Downtime != 0 || Runtime != 0 || Maintentime != 0)
                                    {
                                        if (Downtime > houses)
                                        {
                                            Downtime = houses;
                                        }
                                        else if (Runtime > houses)
                                        {
                                            Runtime = houses;
                                        }
                                        else if (Maintentime > houses)
                                        {
                                            Maintentime = houses;
                                        }
                                        //单个设备汇总计算
                                        Stop_percen = Downtime == 0 ? 0 : (Downtime / houses) * 100;//设备停机时长百分比
                                        Runtime_percen = Runtime == 0 ? 0 : (Runtime / houses) * 100;//设备运行时长百分比
                                        Maintentime_percen = Maintentime == 0 ? 0 : (Maintentime / houses) * 100;//设备维修时长百分比
                                        Anonymous = houses - Downtime - Runtime - Maintentime;//个设备无记录时长
                                        Anonymous_percen = Anonymous <= 0 ? 0 : (Anonymous / houses) * 100; //个设备无记录时长百分比                                        
                                    }
                                }
                                //部门汇总计算
                                RunnDevice = RunnDevice + Runtime;//部门运行时长
                                StopnDevice = StopnDevice + Downtime; //部门停机时长
                                MaintDevice = MaintDevice + Maintentime;//部门维修时长
                                AnonyDevice = (houses * Deparequipment) - RunnDevice - StopnDevice - MaintDevice;//部门无记录时长
                                RunDepar_Device = RunnDevice == 0 ? 0 : (RunnDevice / (houses * Deparequipment)) * 100;//部门运行时长百分比
                                StopDepar_Device = StopnDevice == 0 ? 0 : (StopnDevice / (houses * Deparequipment)) * 100;//部门停机时长百分比
                                MaintDepar_Device = MaintDevice == 0 ? 0 : (MaintDevice / (houses * Deparequipment)) * 100;//部门维修时长百分比
                                AnonyDepar_Device = AnonyDevice <= 0 ? 0 : (AnonyDevice / (houses * Deparequipment)) * 100;//部门无记录时长百分比                                                                

                                equipmentList.Add("Downtime", Downtime.ToString("0.00"));//停机时长
                                Downtime = new double();
                                equipmentList.Add("Stop_percen", Stop_percen.ToString("0.00") + "%");//停机时长百分比
                                Stop_percen = new double();

                                equipmentList.Add("Runtime", Runtime.ToString("0.00"));//运行时长
                                Runtime = new double();
                                equipmentList.Add("Runtime_percen", Runtime_percen.ToString("0.00") + "%");//运行时长百分比
                                Runtime_percen = new double();

                                equipmentList.Add("Maintentime", Maintentime.ToString("0.00"));//维修时长
                                Maintentime = new double();
                                equipmentList.Add("Maintentime_percen", Maintentime_percen.ToString("0.00") + "%");//维修时长百分比
                                Maintentime_percen = new double();

                                equipmentList.Add("Anonymous", Anonymous.ToString("0.00"));//无记录时长
                                Anonymous = new double();
                                equipmentList.Add("Anonymous_percen", Anonymous_percen.ToString("0.00") + "%");//无记录时长百分比
                                Anonymous_percen = new double();

                                devicetime.Add(equipmentList);
                                equipmentList = new JObject();
                                pment.Add("EquipmentNumber", iu);
                                var df = dateList.Where(c => c.EquipmentNumber == iu).Select(c => c.EquipmentName).FirstOrDefault();
                                pment.Add("EquipmentName", df);
                                pment.Add("Devicetime", devicetime);
                                devicetime = new JArray();
                                user.Add(pment);
                                pment = new JObject();
                            }
                        }
                        //全厂汇总计算
                        RuntotalDevice = RuntotalDevice + RunnDevice;//全厂运行时长
                        StoptotalDevice = StoptotalDevice + StopnDevice;//全厂停机时长
                        MainttotalDevice = MainttotalDevice + MaintDevice;//全厂维修时长
                        AnonytotalDevice = (houses * EquipmentList) - RuntotalDevice - StoptotalDevice - MainttotalDevice; //全厂无记录时长
                        Runpercentage_Device = RuntotalDevice == 0 ? 0 : (RuntotalDevice / (houses * EquipmentList)) * 100;//全厂运行时长百分比
                        Stoppercentage_Device = StoptotalDevice == 0 ? 0 : (StoptotalDevice / (houses * EquipmentList)) * 100;//全厂停机时长百分比
                        Maintpercentage_Device = MainttotalDevice == 0 ? 0 : (MainttotalDevice / (houses * EquipmentList)) * 100;//全厂维修时长百分比 
                        Anonypercentage_Device = AnonytotalDevice <= 0 ? 0 : (AnonytotalDevice / (houses * EquipmentList)) * 100;//全厂无记录时长百分比

                        table.Add("UserDepartment", ite);
                        table.Add("RunnDevice", RunnDevice.ToString("0.00"));//部门运行时长
                        RunnDevice = new double();
                        table.Add("RunDepar_Device", RunDepar_Device.ToString("0.00") + "%");//部门运行时长百分比
                        RunDepar_Device = new double();

                        table.Add("StopnDevice", StopnDevice.ToString("0.00"));//部门停机时长
                        StopnDevice = new double();
                        table.Add("StopDepar_Device", StopDepar_Device.ToString("0.00") + "%");//部门停机时长百分比
                        StopDepar_Device = new double();

                        table.Add("MaintDevice", MaintDevice.ToString("0.00"));//部门维修时长
                        MaintDevice = new double();
                        table.Add("MaintDepar_Device", MaintDepar_Device.ToString("0.00") + "%");//部门维修时长百分比
                        MaintDepar_Device = new double();

                        table.Add("AnonyDevice", AnonyDevice.ToString("0.00"));//部门无记录时长
                        AnonyDevice = new double();
                        table.Add("AnonyDepar_Device", AnonyDepar_Device.ToString("0.00") + "%");//部门无记录时长百分比
                        AnonyDepar_Device = new double();

                        table.Add("Table", user);
                        user = new JArray();
                        current.Add(table);
                        table = new JObject();
                    }
                }
                if (RuntotalDevice != 0 || StoptotalDevice != 0 || MainttotalDevice != 0)
                {
                    ainten.Add("RuntotalDevice", RuntotalDevice.ToString("0.00"));//全厂运行时长
                    ainten.Add("Runpercentage_Device", Runpercentage_Device.ToString("0.00") + "%");//全厂运行时长百分比               
                    ainten.Add("StoptotalDevice", StoptotalDevice.ToString("0.00"));//全厂停机时长                
                    ainten.Add("Stoppercentage_Device", Stoppercentage_Device.ToString("0.00") + "%");//全厂停机时长百分比               
                    ainten.Add("MainttotalDevice", MainttotalDevice.ToString("0.00"));//全厂维修时长              
                    ainten.Add("Maintpercentage_Device", Maintpercentage_Device.ToString("0.00") + "%");//全厂维修时长百分比     
                    ainten.Add("AnonytotalDevice", AnonytotalDevice.ToString("0.00"));//全厂无记录时长              
                    ainten.Add("Anonypercentage_Device", Anonypercentage_Device.ToString("0.00") + "%");//全厂无记录时长百分比    
                    ainten.Add("current", current);
                    current = new JArray();
                }
            }
            return Content(JsonConvert.SerializeObject(ainten));
        }

        #endregion

        #endregion

        #region------邮件发送人
        //邮件发送人添加
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：邮件发送人添加
        /// 2.方法的参数和用法：record（对象），用于添加数据
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据邮箱地址和项目名称到UserItemEmail表里查询数据。（2）判断isexist（查找出来的数据：第一步）是否为空，不为空时，直接输出邮件地址已存在，停止运行。
        /// （3）isexist（查找出来的数据：第一步）为空时，把数据保存到对应的数据表和保存到数据库。（4）判断count是否大于0（有没有把数据保存到数据库），大于0，保存成功；等于0（没有把数据保存到数据库或者保存出错），保存失败。
        /// 4.方法（可能）有结果：邮件地址已存在；保存失败；保存成功。
        /// </summary>
        public ActionResult EquipmentEmail_add(UserItemEmail record)
        {
            var isexist = db.UserItemEmail.Count(c => c.EmailAddress == record.EmailAddress && c.ProcesName == record.ProcesName);//根据邮箱地址和项目名称查询数据
            if (isexist > 0)//判断isexist（查找出来的数据）是否为空
                return Content(record.EmailAddress + "已经存在。");
            else //isexist（查找出来的数据）为空
            {
                db.UserItemEmail.Add(record);//把数据保存到对应的数据表
                int count = db.SaveChanges();//保存到数据库
                if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
                    return Content("保存成功");
                else   //count等于0（没有把数据保存到数据库或者保存出错）
                    return Content("保存失败");
            }
        }

        //邮件发送人修改
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：邮件发送人修改
        /// 2.方法的参数和用法：record（对象），用于修改数据
        /// 3.方法的具体逻辑顺序，判断条件：（1）把数据赋值到entry里。(2)修改数据（修改第一步的数据）和保存到数据库。
        /// （3）判断count是否大于0（有没有把数据保存到数据库），大于0，修改成功；等于0（没有把数据保存到数据库或者保存出错），修改失败。
        /// 4.方法（可能）有结果：修改成功；修改失败。
        /// </summary>
        public ActionResult EquipmentEmail_modify(UserItemEmail record)
        {
            DbEntityEntry<UserItemEmail> entry = db.Entry(record);//把数据赋值到entry里
            entry.State = System.Data.Entity.EntityState.Modified;//修改数据
            int count = db.SaveChanges();//保存到数据库
            if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
                return Content("保存成功");
            else //count等于0（没有把数据保存到数据库或者保存出错）
                return Content("保存失败");
        }

        //邮件发送人删除
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：邮件发送人删除
        /// 2.方法的参数和用法：id，用于查询
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据ID到UserItemEmail表里查询数据。（2）添加删除操作时间、添加删除操作人和添加操作记录（如：张三在2020年2月27日删除设备管理邮件发送人为李四的记录）。
        /// （3）删除对应的数据（第一步查询出来的数据）。（4）添加删除操作日记数据（第二步的数据）。(5)保存到数据库，并判断count是否大于0（有没有把数据保存到数据库），大于0，删除成功；等于0（没有把数据保存到数据库或者保存出错），删除失败。
        /// 4.方法（可能）有结果：删除成功；删除失败。
        /// </summary>
        public ActionResult EquipmentEmail_delete(int id)
        {
            var record = db.UserItemEmail.Where(c => c.id == id).FirstOrDefault();//根据ID查询数据
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;//添加删除操作时间
            operaterecord.Operator = ((Users)Session["User"]).UserName;//添加删除操作人
            //添加操作记录（如：张三在2020年2月27日删除设备管理邮件发送人为李四的记录）
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除设备管理邮件发送人为" + record.UserName + "的记录。";
            db.UserItemEmail.Remove(record);//删除对应的数据
            db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
            int count = db.SaveChanges();//保存到数据库
            if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
                return Content("删除成功");
            else //等于0（没有把数据保存到数据库或者保存出错）
                return Content("删除失败");
        }
        #endregion

        #region------邮件抄送人
        //邮件抄送人添加
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：邮件抄送人添加
        /// 2.方法的参数和用法：record（对象），用于添加数据
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据邮箱地址和项目名称到UserItemEmail表里查询数据。（2）判断isexist（查找出来的数据：第一步）是否为空，不为空时，直接输出邮件地址已存在，停止运行。
        /// （3）isexist（查找出来的数据：第一步）为空时，把数据保存到对应的数据表和保存到数据库。（4）判断count是否大于0（有没有把数据保存到数据库），大于0，保存成功；等于0（没有把数据保存到数据库或者保存出错），保存失败。
        /// 4.方法（可能）有结果：邮件地址已存在；保存失败；保存成功。
        /// </summary>
        public ActionResult EquipmentEmail_Send_add(UserItemEmail record)
        {
            var isexist = db.UserItemEmail.Count(c => c.EmailAddress == record.EmailAddress && c.ProcesName == record.ProcesName);//根据邮箱地址和项目名称查询数据
            if (isexist > 0)//判断isexist（查找出来的数据)是否为空
                return Content(record.EmailAddress + "已经存在。");
            else //isexist（查找出来的数据）为空
            {
                db.UserItemEmail.Add(record);//把数据保存到对应的数据表
                int count = db.SaveChanges();//保存到数据库
                if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
                    return Content("保存成功");
                else //count等于0（没有把数据保存到数据库或者保存出错）
                    return Content("保存失败");
            }
        }

        //邮件抄送人修改
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：邮件抄送人修改
        /// 2.方法的参数和用法：record（对象），用于修改数据
        /// 3.方法的具体逻辑顺序，判断条件：（1）把数据赋值到entry里。(2)修改数据（修改第一步的数据）和保存到数据库。
        /// （3）判断count是否大于0（有没有把数据保存到数据库），大于0，修改成功；等于0（没有把数据保存到数据库或者保存出错），修改失败。
        /// 4.方法（可能）有结果：修改成功；修改失败。
        /// </summary>
        public ActionResult EquipmentEmail_Send_modify(UserItemEmail record)
        {
            DbEntityEntry<UserItemEmail> entry = db.Entry(record);//把数据赋值到entry里
            entry.State = System.Data.Entity.EntityState.Modified;//修改数据
            int count = db.SaveChanges();//保存到数据库
            if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
                return Content("保存成功");
            else //count等于0（没有把数据保存到数据库或者保存出错）
                return Content("保存失败");
        }

        //邮件抄送人删除
        [HttpPost]
        ///<summary>
        /// 1.方法的作用：邮件抄送人删除
        /// 2.方法的参数和用法：id，用于查询
        /// 3.方法的具体逻辑顺序，判断条件：（1）根据ID到UserItemEmail表里查询数据。（2）添加删除操作时间、添加删除操作人和添加操作记录（如：张三在2020年2月27日删除设备管理邮件发送人为李四的记录）。
        /// （3）删除对应的数据（第一步查询出来的数据）。（4）添加删除操作日记数据（第二步的数据）。(5)保存到数据库，并判断count是否大于0（有没有把数据保存到数据库），大于0，删除成功；等于0（没有把数据保存到数据库或者保存出错），删除失败。
        /// 4.方法（可能）有结果：删除成功；删除失败。
        /// </summary>
        public ActionResult EquipmentEmail_Send_delete(int id)
        {
            var record = db.UserItemEmail.Where(c => c.id == id).FirstOrDefault();//根据ID表里查询数据
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;//添加删除操作时间
            operaterecord.Operator = ((Users)Session["User"]).UserName;//添加删除操作人
            //添加操作记录（如：张三在2020年2月27日删除设备管理邮件抄送人为李四的记录）
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除设备管理邮件抄送人为" + record.UserName + "的记录。";
            db.UserItemEmail.Remove(record);//删除对应的数据
            db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
            int count = db.SaveChanges();//保存到数据库
            if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
                return Content("删除成功");
            else //等于0（没有把数据保存到数据库或者保存出错）
                return Content("删除失败");
        }
        #endregion

        #region------其他方法

        // GET: Equipment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Equipment/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,SerialNumber,EquipmentNumber,AssetNumber,EquipmentName,Brand,ModelSpecification,InfoPlate,ManufacturingNumber,Quantity,ActionDate,DepreciableLife,UserDepartment,StoragePlace,WorkShop,LineNum,Section,FunctionDiscription,Status,Creator,CreateTime,Modifier,ModifyTime,Remark")] EquipmentBasicInfo equipmentBasicInfo)
        {
            if (ModelState.IsValid)
            {
                db.EquipmentBasicInfo.Add(equipmentBasicInfo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(equipmentBasicInfo);
        }

        // GET: Equipment/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentBasicInfo equipmentBasicInfo = await db.EquipmentBasicInfo.FindAsync(id);
            if (equipmentBasicInfo == null)
            {
                return HttpNotFound();
            }
            return View(equipmentBasicInfo);
        }

        // POST: Equipment/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,SerialNumber,EquipmentNumber,AssetNumber,EquipmentName,Brand,ModelSpecification,InfoPlate,ManufacturingNumber,Quantity,ActionDate,DepreciableLife,UserDepartment,StoragePlace,WorkShop,LineNum,Section,FunctionDiscription,Status,Creator,CreateTime,Modifier,ModifyTime,Remark")] EquipmentBasicInfo equipmentBasicInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipmentBasicInfo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(equipmentBasicInfo);
        }

        // GET: Equipment/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EquipmentBasicInfo equipmentBasicInfo = await db.EquipmentBasicInfo.FindAsync(id);
            if (equipmentBasicInfo == null)
            {
                return HttpNotFound();
            }
            return View(equipmentBasicInfo);
        }

        // POST: Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EquipmentBasicInfo equipmentBasicInfo = await db.EquipmentBasicInfo.FindAsync(id);
            db.EquipmentBasicInfo.Remove(equipmentBasicInfo);
            await db.SaveChangesAsync();
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

        #region --------------------返回指定目录下所有文件信息
        /// <summary>  
        /// 返回指定目录下所有文件信息  
        /// </summary>  
        /// <param name="strDirectory">目录字符串</param>  
        /// <returns></returns>  
        public List<FileInfo> GetAllFilesInDirectory(string strDirectory)
        {
            List<FileInfo> listFiles = new List<FileInfo>(); //保存所有的文件信息  
            DirectoryInfo directory = new DirectoryInfo(strDirectory);
            DirectoryInfo[] directoryArray = directory.GetDirectories();
            FileInfo[] fileInfoArray = directory.GetFiles();
            if (fileInfoArray.Length > 0) listFiles.AddRange(fileInfoArray);
            foreach (DirectoryInfo _directoryInfo in directoryArray)
            {
                DirectoryInfo directoryA = new DirectoryInfo(_directoryInfo.FullName);
                DirectoryInfo[] directoryArrayA = directoryA.GetDirectories();
                FileInfo[] fileInfoArrayA = directoryA.GetFiles();
                if (fileInfoArrayA.Length > 0) listFiles.AddRange(fileInfoArrayA);
                GetAllFilesInDirectory(_directoryInfo.FullName);//递归遍历  
            }
            return listFiles;
        }
        #endregion

    }
}

