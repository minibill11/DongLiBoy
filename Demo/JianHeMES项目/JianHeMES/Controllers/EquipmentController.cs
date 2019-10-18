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

namespace JianHeMES.Controllers
{
    public class EquipmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        #region------查询首页------
        // GET: Equipment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index2()
        {
            return View();
        }

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
                            JOmechine.Add("assetNumber", mechine.AssetNumber);
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

        #region----根据资产编号获取设备详细信息的方法 Index2
        public ActionResult Particulars(string assetnumber)
        {
            if (assetnumber != null)
            {
                var partic = db.EquipmentSetStation.Where(c => c.AssetNumber == assetnumber).ToList();
                return Content(JsonConvert.SerializeObject(partic));
            }
            return Content("输入的资产编号不正确，请重新输入！");
        }

        #endregion

        #region-----产线添加设备的方法(单个)  Index2
        public ActionResult ADDEquipment(EquipmentSetStation EquipmentSetStation)
        {
            if (!String.IsNullOrEmpty(EquipmentSetStation.AssetNumber) && !String.IsNullOrEmpty(EquipmentSetStation.UserDepartment) && !String.IsNullOrEmpty(EquipmentSetStation.LineNum))
            {
                var eqlist = db.EquipmentSetStation.Where(c => c.UserDepartment == EquipmentSetStation.UserDepartment && c.LineNum == EquipmentSetStation.LineNum && c.StationNum >= EquipmentSetStation.StationNum).ToList();
                foreach (var item in eqlist)
                {
                    item.StationNum = item.StationNum + 1;
                    db.SaveChanges();
                }
                EquipmentSetStation.CreateTime = DateTime.Now;
                EquipmentSetStation.Creator = ((Users)Session["user"]).UserName;
                db.EquipmentSetStation.Add(EquipmentSetStation);
                db.SaveChanges();
                return Content("添加设备成功！");
            }
            return Content("添加设备失败！");
        }
        #endregion

        #region------产线删除设备 Index2
        [HttpPost]
        public ActionResult deleteEquipment(string assetNumber)
        {
            if (!String.IsNullOrEmpty(assetNumber))
            {
                var eq = db.EquipmentSetStation.Where(c => c.AssetNumber == assetNumber).FirstOrDefault();
                var eqlist = db.EquipmentSetStation.Where(c => c.UserDepartment == eq.UserDepartment && c.LineNum == eq.LineNum && c.StationNum >= eq.StationNum).ToList();
                foreach (var item in eqlist)
                {
                    item.StationNum = item.StationNum - 1;
                    db.SaveChanges();
                }
                db.EquipmentSetStation.Remove(eq);
                db.SaveChanges();
                return Content("删除设备成功！");
            }
            return Content("删除设备失败！");
        }


        #endregion

        #region-----产线一条添加  Index2

        public class equipment_station
        {
            public int Key { get; set; }
            public string Value { get; set; }
        }

        public ActionResult ADDLineNum(string usedepartment, string lineNum, string assetnumlist)
        {
            List<equipment_station> asnumlist = (List<equipment_station>)JsonHelper.jsonDes<List<equipment_station>>(assetnumlist);
            if (!String.IsNullOrEmpty(usedepartment) && !String.IsNullOrEmpty(lineNum) && asnumlist.Count > 0)
            {
                List<EquipmentSetStation> eqlist = new List<EquipmentSetStation>();
                foreach (var item in asnumlist)
                {
                    EquipmentSetStation eq = new EquipmentSetStation();
                    var eqdata = db.EquipmentBasicInfo.Where(c => c.AssetNumber == item.Value).FirstOrDefault();
                    eq.EquipmentNumber = eqdata.EquipmentNumber; //设备编号
                    eq.AssetNumber = item.Value;//资产编号
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
                return Content(usedepartment + "添加" + lineNum + "产线成功,该产线添加了" + asnumlist.Count + "台设备。");
            }
            return Content("添加产线失败！" + (String.IsNullOrEmpty(usedepartment) == true ? "未选择部门！" : "") + (String.IsNullOrEmpty(lineNum) == true ? "没有产线名！" : "") + (asnumlist.Count == 0 ? "产线至少要有一台设备！" : ""));
        }
        #endregion

        #region-----迁移设备的方法 Index2
        public ActionResult Migration(string assetNumber, string userdepar, string linenum, int stationnum)
        {
            var eq = db.EquipmentSetStation.FirstOrDefault(c => c.AssetNumber == assetNumber);
            //检查是否有迁移的产线，如果没有此产线，直接迁移。如果有此产线，检查此设备位置号是否有设备，如果此设备位置号没有设备，直接迁移，如果已有此设备号，先把原来的设备往后移，再迁移设备。
            var new_linenum_eqlist = db.EquipmentSetStation.Where(c => c.LineNum == linenum).ToList();
            //没有此产线号
            if (new_linenum_eqlist.Count == 0)
            {
                eq.UserDepartment = userdepar;
                eq.LineNum = linenum;
                eq.StationNum = stationnum;
                eq.Modifier = ((Users)Session["user"]).UserName;
                eq.ModifyTime = DateTime.Now;
                db.SaveChanges();
                return Content("迁移设备成功！");
            }
            //有此产线号
            if (new_linenum_eqlist.Count > 0)
            {
                //有此位置号
                if (stationnum == eq.StationNum)
                {
                    var asst = db.EquipmentSetStation.Where(c => c.UserDepartment == userdepar && c.LineNum == linenum).ToList();
                    foreach (var item in asst)
                    {
                        item.StationNum = item.StationNum + 1;
                        db.SaveChanges();
                    }
                    eq.UserDepartment = userdepar;
                    eq.LineNum = linenum;
                    eq.Modifier = ((Users)Session["user"]).UserName;
                    eq.ModifyTime = DateTime.Now;
                    db.SaveChanges();
                }
                //没有此位置号
                else
                {
                    eq.UserDepartment = userdepar;
                    eq.LineNum = linenum;
                    eq.StationNum = stationnum;
                    eq.Modifier = ((Users)Session["user"]).UserName;
                    eq.ModifyTime = DateTime.Now;
                    db.SaveChanges();
                }
                return Content("迁移设备成功！");
            }
            return Content("迁移设备失败，请检查数据是否正确！");
        }
        #endregion

        #region------修改设备状态的方法 Index2

        //public ActionResult ChangeStatus(int id, string statu)
        //{
        //    if (!String.IsNullOrEmpty(id.ToString()) && !String.IsNullOrEmpty(statu))
        //    {
        //        var record = db.EquipmentSetStation.Where(c => c.Id == id).FirstOrDefault();
        //        record.Status = statu;
        //        record.ModifyTime = DateTime.Now;
        //        record.Modifier = ((Users)Session["user"]).UserName;
        //        db.SaveChangesAsync();
        //        return Content("true");
        //    }
        //    return Content("false");
        //}

        #endregion

        #region---获取所有设备资产编号的方法  Index2
        [HttpPost]
        public ActionResult AssetNumberList()
        {
            var asset = db.EquipmentBasicInfo.OrderByDescending(m => m.Id).Select(c => c.AssetNumber).Distinct();
            return Content(JsonConvert.SerializeObject(asset));
        }
        #endregion

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
        #endregion

        #region------修改设备使用部门
        [HttpPost]
        public async Task<bool> ModifyEquipmentUseDepartment(int id, string newdepartment)
        {
            if (!String.IsNullOrEmpty(id.ToString()) && !String.IsNullOrEmpty(newdepartment))
            {
                var record = await db.EquipmentBasicInfo.Where(c => c.Id == id).FirstOrDefaultAsync();
                record.UserDepartment = newdepartment;
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        #endregion

        #region------批量添加设备------
        public ActionResult BatchInputEquipment()
        {
            //if (Session["User"] == null)
            //{
            //    return RedirectToAction("Login", "Users", new { col = "Equipment", act = "BatchInputEquipment" });
            //}
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> BatchInputEquipment(List<EquipmentBasicInfo> inputList)
        {
            string repeat = null;
            foreach (var item in inputList)
            {
                item.CreateTime = DateTime.Now;
                item.Creator = ((Users)Session["User"]) != null ? ((Users)Session["User"]).UserName : "";
                if (db.EquipmentBasicInfo.Count(c => c.AssetNumber == item.AssetNumber) != 0)
                    repeat = repeat + item.AssetNumber + ",";
            }
            JObject result = new JObject();
            if (!string.IsNullOrEmpty(repeat))
            {
                result.Add("repeat", repeat + "已经有相同的数据，请重新输入");
                return Content(JsonConvert.SerializeObject(result));
            }
            db.EquipmentBasicInfo.AddRange(inputList);
            int savecount = await db.SaveChangesAsync();
            if (savecount > 0) result.Add("success", "添加" + inputList.Count.ToString() + "台设备成功");
            else result.Add("failure", "添加失败");
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region------修改设备状态------
        //[HttpPost]
        //public async Task<bool> ModifyEquipmentStatusAsync(int id, string status)
        //{
        //    if (!String.IsNullOrEmpty(id.ToString()) && !String.IsNullOrEmpty(status))
        //    {
        //        var record = await db.EquipmentBasicInfo.Where(c => c.Id == id).FirstOrDefaultAsync();
        //        record.Status = status;
        //        await db.SaveChangesAsync();
        //        return true;
        //    }
        //    return false;
        //}
        #endregion

        #region------添加/修改状态记录------
        [HttpPost]
        public async Task<bool> AddEquipmentStatusRecordAsync(EquipmentStatusRecord record)
        {
            record.CreateTime = DateTime.Now;
            db.EquipmentStatusRecord.Add(record);
            if (record.StatusStarTime < DateTime.Now.AddMinutes(10) && record.StatusEndTime > DateTime.Now)
            {
                var eqm = db.EquipmentBasicInfo.Where(c => c.AssetNumber == record.AssetNumber).FirstOrDefault();
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
        public async Task<bool> EditEquipmentStatusRecordAsync(int id, EquipmentStatusRecord modifyrecord)
        {
            if (ModelState.IsValid)
            {
                var record = db.EquipmentStatusRecord.Where(c => c.Id == id).FirstOrDefault();
                record = modifyrecord;
                await db.SaveChangesAsync();
                return true; // Content("修改成功!");
            }
            return false;// Content("修改失败!");
        }
        #endregion

        #region---同时修改三个表的设备状态（Status）

        public ActionResult Equipment_state(List<string> assetnumber, string status, string linenum, string userdepar)
        {
            JObject message = new JObject();
            foreach (var item in assetnumber)
            {
                if (!String.IsNullOrEmpty(item) && !String.IsNullOrEmpty((status)))
                {
                    var station = db.EquipmentSetStation.Where(c => c.AssetNumber == item).FirstOrDefault();
                    if (station != null)
                    {
                        station.Status = status;
                        station.ModifyTime = DateTime.Now;
                        station.Modifier = ((Users)Session["user"]).UserName;
                    }
                    var infoe = db.EquipmentBasicInfo.Where(c => c.AssetNumber == item).FirstOrDefault();
                    if (infoe != null)
                    {
                        infoe.Status = status;
                        infoe.ModifyTime = DateTime.Now;
                        infoe.Modifier = ((Users)Session["user"]).UserName;
                    }
                    var record = db.EquipmentStatusRecord.OrderByDescending(c => c.StatusStarTime).Where(c => c.AssetNumber == item).FirstOrDefault();
                    if (record != null && record.Status != status)
                    {
                        record.StatusEndTime = DateTime.Now;
                        record.ModifyTime = DateTime.Now;
                        record.Modifier = ((Users)Session["user"]).UserName;
                        db.SaveChanges();
                        var add = record;
                        add.Status = status;
                        add.StatusStarTime = record.StatusEndTime;
                        add.StatusEndTime = null;
                        add.CreateTime = DateTime.Now;
                        add.Creator = ((Users)Session["user"]).UserName;
                        db.EquipmentStatusRecord.Add(add);
                        db.SaveChanges();
                    }
                    else if (record == null && infoe != null)
                    {
                        var rede = new EquipmentStatusRecord() { EquipmentNumber = infoe.EquipmentName, AssetNumber = infoe.AssetNumber, EquipmentName = infoe.EquipmentName, OrderNum = null, Status = status, StatusStarTime = DateTime.Now, StatusEndTime = null, ReportRepairMan = null, FailureDescription = null, Reason = null, RepairOrTestContent = null, GetJobTime = null, PlanFinishTime = null, RepairMan = null, SparePartsInfo = null, UserDepartment = userdepar, WorkShop = null, LineNum = linenum, Section = infoe.Section, Creator = ((Users)Session["user"]).UserName, CreateTime = DateTime.Now, Modifier = null, ModifyTime = null, Remark = null };
                        db.EquipmentStatusRecord.Add(rede);
                        db.SaveChanges();
                    }
                }
                message.Add("msg", "修改" + assetnumber.Count.ToString() + "台设备状态成功！");
                message.Add("result", true);
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
        public async Task<ActionResult> Details(string assetNumber)
        {
            if (assetNumber == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JObject result = new JObject();
            var basicinfo = await db.EquipmentBasicInfo.Where(c => c.AssetNumber == assetNumber).FirstOrDefaultAsync();
            result.Add("basicinfo", JsonConvert.SerializeObject(basicinfo));
            List<FileInfo> fileInfos = new List<FileInfo>();
            if (Directory.Exists(@"D:\MES_Data\Equipment\" + assetNumber + "\\") == false)
            {
                result.Add("picture", "未上传图片。");
            }
            else
            {
                fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\Equipment\" + assetNumber + "\\").Where(c => c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
                result.Add("picture", JsonConvert.SerializeObject(fileInfos));
            }
            var statusrecord = await db.EquipmentStatusRecord.Where(c => c.AssetNumber == assetNumber).OrderBy(c => c.StatusStarTime).ToListAsync();
            result.Add("statusrecord", JsonConvert.SerializeObject(statusrecord));
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region------上传设备照片(jpg)方法------
        [HttpPost]
        public ActionResult UploadEquipmentPicture(string assetNumber)
        {
            foreach (var file1 in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[file1.ToString()];
                var fileType = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                if (!String.Equals(fileType, ".jpg"))
                {
                    return Content("您选择文件的文件类型不正确，请选择jpg类型图片文件！");
                }
                if (Directory.Exists(@"D:\MES_Data\Equipment\" + assetNumber + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\Equipment\" + assetNumber + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\Equipment\" + assetNumber + "\\");
                //文件为jpg类型
                if (fileType == ".jpg")
                {
                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(assetNumber) && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                    file.SaveAs(@"D:\MES_Data\Equipment\" + assetNumber + "\\" + assetNumber + (jpg_count + 1) + fileType);
                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\Equipment\" + assetNumber + "\\" + assetNumber + fileType);
                }
                //return RedirectToAction("Details", "Equipment", new { assetNumber = assetNumber });
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
        public ActionResult EQNumberList()
        {
            var eqNumberlist = db.EquipmentBasicInfo.Select(c => c.EquipmentNumber).Distinct();
            return Content(JsonConvert.SerializeObject(eqNumberlist));
        }
        #endregion

        #region------根据设备编号获取设备信息
        [HttpPost]
        public ActionResult EquipmentInfo_getdata_by_eqnum(string eqnum)
        {
            return Content(JsonConvert.SerializeObject(db.EquipmentBasicInfo.Where(c=>c.EquipmentNumber==eqnum).FirstOrDefault()));
        }

        #endregion

        #region------设备点检保养记录表

        //点检查询首页
        public ActionResult Equipment_Tally()
        {
            return View();
        }

        //点检记录查询方法
        [HttpPost]
        public ActionResult Equipment_Tally(string equipmentName, string lineNum, string equipmentNumber, int year = 0, int month = 0)
        {
            List<Equipment_Tally_maintenance> recordlist = db.Equipment_Tally_maintenance.ToList();
            if (!String.IsNullOrEmpty(equipmentName))
            {
                recordlist = recordlist.Where(c => c.EquipmentName.Contains(equipmentName)).ToList();
            }
            if (!String.IsNullOrEmpty(lineNum))
            {
                recordlist = recordlist.Where(c => c.LineName.Contains(lineNum)).ToList();
            }
            if (!String.IsNullOrEmpty(equipmentNumber))
            {
                recordlist = recordlist.Where(c => c.EquipmentNumber.Contains(equipmentNumber)).ToList();
            }
            if (year!=0)
            {
                recordlist = recordlist.Where(c => c.Year==year).ToList();
            }
            if (month!=0)
            {
                recordlist = recordlist.Where(c => c.Month==month).ToList();
            }
            var result = recordlist.Select(c => new { c.Id, c.EquipmentName, c.EquipmentNumber, c.LineName, c.Year, c.Month }).ToList();
            return Content(JsonConvert.SerializeObject(result));
        }

        //打开点检记录
        public ActionResult Equipment_Tally_maintenance()
        {
            return View();
        }

        //打开点检记录取数据
        [HttpPost]
        public ActionResult Equipment_Tally_maintenance(int id)
        {
            var record = db.Equipment_Tally_maintenance.Where(c => c.Id == id).FirstOrDefault();
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

        #region------设备点检保养记录添加
        [HttpPost]
        public ActionResult AddEquipment_Tally_maintenance_Add(string eqname, string linename, string eqnum, int year, int month)
        {
            //检查是否存在
            int count = db.Equipment_Tally_maintenance.Count(c => c.EquipmentNumber == eqnum && c.Year == year && c.Month == month);
            if (count > 0)
            {
                return Content("记录已经存在");
            }
            else
            {
                Equipment_Tally_maintenance Equipment_Tally_maintenance = new Equipment_Tally_maintenance()
                {
                    EquipmentNumber = eqnum,
                    EquipmentName = eqname,
                    LineName = linename,
                    Year = year,
                    Month = month
                };
                db.Equipment_Tally_maintenance.Add(Equipment_Tally_maintenance);
                int result = db.SaveChanges();
                if (result > 0) return Content("保存成功！");
                else return Content("保存失败！");
            }
        }
        #endregion

        #region------设备点检保养记录修改
        [HttpPost]
        public ActionResult AddEquipment_Tally_maintenance_Edit(Equipment_Tally_maintenance equipment_Tally_maintenance)
        {
            if (equipment_Tally_maintenance != null)
            {
                var record = db.Equipment_Tally_maintenance.FirstOrDefault(c => c.EquipmentNumber == equipment_Tally_maintenance.EquipmentNumber && c.Year == equipment_Tally_maintenance.Year && c.Month == equipment_Tally_maintenance.Month);
                if (record != null)
                {
                    //打开记录
                    record = equipment_Tally_maintenance;
                    var result = db.SaveChanges();
                    if (result > 0) return Content("保存成功！");
                    else return Content("保存失败！");
                }
                else
                {
                    db.Equipment_Tally_maintenance.Add(equipment_Tally_maintenance);
                    var result = db.SaveChanges();
                    if (result > 0) return Content("保存成功！");
                    else return Content("保存失败！");
                }
            }
            else return Content("数据有误");
        }
        #endregion

        #region------仪器设备报修单
        public ActionResult EquipmentRepairbill()
        {
            return View();
        }
        #region---获取所有仪器设备报修单的设备编号的方法
        [HttpPost]
        public ActionResult InstrumentList()
        {
            var Instr_Number = db.EquipmentRepairbill.OrderByDescending(m => m.Id).Select(c => c.EquipmentNumber).Distinct();
            return Content(JsonConvert.SerializeObject(Instr_Number));
        }
        #endregion

        #region-----根据设备编号，设备名称，故障时间查询仪器设备报修单

        public ActionResult EquipmentRepairbill_Query()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EquipmentRepairbill_Query(string equnumber, string equipname, DateTime? date, DateTime? starttime, DateTime? endtime)
        {
            var equipment_list = db.EquipmentRepairbill.ToList();
            if (!String.IsNullOrEmpty(equnumber))
            {
                equipment_list = equipment_list.Where(c => c.EquipmentNumber == equnumber).ToList();
            }
            if (!String.IsNullOrEmpty(equipname))
            {
                equipment_list = equipment_list.Where(c => c.EquipmentName == equipname).ToList();
            }
            if (date != null)
            {
                equipment_list = equipment_list.Where(c => c.FaultTime == date).ToList();
            }
            if (starttime != null && endtime != null)
            {
                equipment_list = equipment_list.Where(c => c.FaultTime >= starttime && c.FaultTime <= endtime).ToList();
            }
            return Content(JsonConvert.SerializeObject(equipment_list));
        }
        #endregion

        #region------仪器设备报修单数据保存
        public ActionResult ADDmaintenance(EquipmentRepairbill EquipmentRepairbill)
        {
            if (EquipmentRepairbill != null)
            {
                if (db.EquipmentRepairbill.Count(c => c.EquipmentNumber == EquipmentRepairbill.EquipmentNumber && c.EquipmentName == EquipmentRepairbill.EquipmentName && c.FaultTime == EquipmentRepairbill.FaultTime) > 0)
                {
                    return Content("已有重复数据，请检查故障时间是否填写正确！");
                }
                EquipmentRepairbill.RepairDate = DateTime.Now;
                EquipmentRepairbill.RepairName = ((Users)Session["user"]).UserName;
                db.EquipmentRepairbill.Add(EquipmentRepairbill);
                db.SaveChanges();
                return Content("true");
            }
            return Content("保存出错，请确认是否书写规范！");
        }
        #endregion

        #region------仪器设备报修单修改
        public ActionResult Modify_repairs(EquipmentRepairbill equipmentRepairbill)
        {
            if(ModelState.IsValid)
            {
                db.Entry(equipmentRepairbill).State = EntityState.Modified;
                var savecount = db.SaveChanges();
                if(savecount>0) return Content("true");
                else return Content("false");
            }
            return Content("false");
        }
        #endregion

        #endregion

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

