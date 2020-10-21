﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JianHeMES.Controllers
{
    public class TipsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonController com = new CommonController();


        #region------提示用户待完成工作---

        // GET: Tips
        //public ActionResult Index()
        //{
        //    //return View(db.UserInformationTips.ToList());
        //    return View();
        //}

        //public ActionResult TipsArguments()
        //{
        //    return View();
        //}

        /// <summary>
        /// 根据提示项名查出所有提示项
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(int userId)
        {
            var userTipsItemList = db.UserNeedsPrompt.Where(c => c.UserNum == userId).ToList();
            JObject results = new JObject();
            foreach (var item in userTipsItemList)
            {
                //小样报告
                if (item.Matters == "小样报告确认审核") results.Add("小样报告确认审核", SmallSampleNotAssessQuery().Count());
                if (item.Matters == "小样报告确认核准") results.Add("小样报告确认核准", SmallSampleNotApprovedQuery().Count());
                //工序产能
                if (item.Matters == "工序产能确认审核") results.Add("工序产能确认审核", Process_CapacityNotExaminanQuery().Count());
                if (item.Matters == "工序产能确认批准") results.Add("工序产能确认批准", Process_CapacityNotApproverQuery().Count());
                if (item.Matters == "工序产能确认受控") results.Add("工序产能确认受控", Process_CapacityNotControlledQuery().Count());
                //设备管理
                if (item.Matters == "设备报修单审核") results.Add("设备报修单审核", Equipment_Repairbill_AssessorQuery().Count());

                if (item.Matters == "报修单采购批准") results.Add("报修单采购批准", Equipment_Repairbill_ApproveQuery().Count());
                if (item.Matters == "报修单技术部意见批准") results.Add("报修单技术部意见批准", Equipment_Repairbill_ApproveQuery1().Count());
                if (item.Matters == "故障报修单批准") results.Add("故障报修单批准", Equipment_Repairbill_ApproveQuery2().Count());
                if (item.Matters == "技术部故障报修单批准") results.Add("技术部故障报修单批准", Equipment_Repairbill_ApproveQuery3().Count());

                if (item.Matters == "设备报修单技术部意见") results.Add("设备报修单技术部意见", Equipment_RepairbillQuery_TecDepar().Count());
                if (item.Matters == "设备报修单采购部意见") results.Add("设备报修单采购部意见", Equipment_RepairbillQuery_Purchasing().Count());
                if (item.Matters == "设备报修单维修人/厂家") results.Add("设备报修单维修人/厂家", Equipment_RepairbillQuery_MainName().Count());
                if (item.Matters == "设备报修单维修后确认/技术部") results.Add("设备报修单维修后确认/技术部", Equipment_RepairbillQuery_TcConfirmName().Count());
                if (item.Matters == "设备报修单维修后确认/维修需要部门") results.Add("设备报修单维修后确认/维修需要部门", Equipment_RepairbillQuery_AfterMain().Count());

                if (item.Matters == "设备点检未确认点检保养") results.Add("设备点检未确认点检保养", Equipment_TallyQuery().Count());

                if (item.Matters == "未审核的质量指标达成状况统计表") results.Add("未审核的质量指标达成状况统计表", Equipment_QualitytargetQuery().Count());
                if (item.Matters == "未批准的质量指标达成状况统计表") results.Add("未批准的质量指标达成状况统计表", Equipment_ApprovetQuery().Count());
                if (item.Matters == "技术部未审核的安全库存清单") results.Add("技术部未审核的安全库存清单", Equipment_SafetyTec_AssessorQuery().Count());
                if (item.Matters == "MC部未审核的安全库存清单") results.Add("MC部未审核的安全库存清单", Equipment_SafetyAssessorQuery().Count());
                if (item.Matters == "工厂厂长未批准的安全库存清单") results.Add("工厂厂长未批准的安全库存清单", Equipment_SafetyApproveQuery().Count());
            }
            return Content(JsonConvert.SerializeObject(results));
        }

        //提示用户有多少个项目需要审批
        [HttpPost]
        public int Pending_Work(int userId)
        {
            int count = 0;
            var userTipsItemList = db.UserNeedsPrompt.Where(c => c.UserNum == userId).ToList();
            DateTime dt = DateTime.Now;
            DateTime d1 = new DateTime(dt.Year, dt.Month, 1);
            DateTime d2 = d1.AddMonths(1).AddDays(-1);
            foreach (var item in userTipsItemList)
            {
                //小样报告
                if (item.Matters == "小样报告确认审核") count = count + db.Small_Sample.Count(c => c.Assessor == null);
                if (item.Matters == "小样报告确认核准") count = count + db.Small_Sample.Count(c => c.Assessor != null && c.Approved == null);

                //工序产能
                if (item.Matters == "工序产能确认审核") count = count + Process_CapacityNotExaminanQuery().Count;
                if (item.Matters == "工序产能确认批准") count = count + Process_CapacityNotApproverQuery().Count;
                if (item.Matters == "工序产能确认受控") count = count + Process_CapacityNotControlledQuery().Count;

                //设备管理
                //报修单
                if (item.Matters == "设备报修单审核") count = count + Equipment_Repairbill_AssessorQuery().Count;

                if (item.Matters == "报修单采购批准") count = count + Equipment_Repairbill_ApproveQuery().Count;
                if (item.Matters == "报修单技术部意见批准") count = count + Equipment_Repairbill_ApproveQuery1().Count;
                if (item.Matters == "故障报修单批准") count = count + Equipment_Repairbill_ApproveQuery2().Count;
                if (item.Matters == "技术部故障报修单批准") count = count + Equipment_Repairbill_ApproveQuery3().Count;

                if (item.Matters == "设备报修单技术部意见") count = count + Equipment_RepairbillQuery_TecDepar().Count;
                if (item.Matters == "设备报修单采购部意见") count = count + Equipment_RepairbillQuery_Purchasing().Count;
                if (item.Matters == "设备报修单维修人/厂家") count = count + Equipment_RepairbillQuery_MainName().Count;
                if (item.Matters == "设备报修单维修后确认/技术部") count = count + Equipment_RepairbillQuery_TcConfirmName().Count;
                if (item.Matters == "设备报修单维修后确认/维修需要部门") count = count + Equipment_RepairbillQuery_AfterMain().Count;

                //点检保养              
                if (item.Matters == "设备点检未确认点检保养") count = count + Equipment_TallyQuery().Count;

                //质量目标达成状况统计表
                if (item.Matters == "未审核的质量指标达成状况统计表") count = count + Equipment_QualitytargetQuery().Count;
                if (item.Matters == "未批准的质量指标达成状况统计表") count = count + Equipment_ApprovetQuery().Count;
                //安全库存清单
                if (item.Matters == "技术部未审核的安全库存清单") count = count + Equipment_SafetyTec_AssessorQuery().Count;
                if (item.Matters == "MC部未审核的安全库存清单") count = count + Equipment_SafetyAssessorQuery().Count;
                if (item.Matters == "工厂厂长未批准的安全库存清单") count = count + Equipment_SafetyApproveQuery().Count;
            }
            return count;
        }
        #endregion



        #region------查询输出
        /// <summary>
        /// 输出所有提示项名清单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserTipsList()
        {
            JObject result = new JObject();
            var tipsList = db.UserInformationTips.Select(c => new { c.id, c.Matters });
            result.Add("result", JsonConvert.SerializeObject(tipsList));
            return Content(JsonConvert.SerializeObject(result));
        }
        /// <summary>
        /// 输出用户所有提示项清单
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserTipsList1(int userId)
        {
            JObject result = new JObject();
            var tipsList = db.UserNeedsPrompt.Where(c => c.UserNum == userId).Select(c => new { c.id, c.UserName, c.UserNum, c.Matters });

            result.Add("result", JsonConvert.SerializeObject(tipsList));
            return Content(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 输出提示项所有用户清单
        /// </summary>
        /// <param name="matter"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserTipsList2(string matter)
        {
            JObject result = new JObject();
            var tipsList = db.UserNeedsPrompt.Where(c => c.Matters == matter).Select(c => new { c.id, c.UserName, c.UserNum, c.Matters });
            result.Add("result", JsonConvert.SerializeObject(tipsList));
            return Content(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 找出所有未审核的小样报告确认审核项
        /// </summary>
        /// <returns></returns>
        public JArray SmallSampleNotAssessQuery()
        {
            JArray result = new JArray();
            JObject temp = new JObject();
            var recordList = db.Small_Sample.Where(c => c.Assessor == null).ToList();
            foreach (var item in recordList)
            {
                temp.Add("OrderNumber", item.OrderNumber);
                temp.Add("Id", item.Id);
                //temp.Add(item.OrderNumber, item.Id);
                result.Add(temp);
                temp = new JObject();

                //[   {"Small_Sample":[  {"OrderNumber":"AAA","Id":"123","Assessor":"false","Approved":"false"}  ,{},{}]},       {"Equipment":[{""}{""}{""}]}   {"IQC":[]}  ]
            }
            return result;
        }

        /// <summary>
        /// 找出所有未审核的工序产能表
        /// </summary>
        /// <returns></returns>
        public JArray Process_CapacityNotExaminanQuery()
        {
            JArray result = new JArray();
            #region 查找最后一个版本的集合,仅工序平衡表
            List<ProcessBalance> balances = com.GetNewNumberBalanceInfo();
            #endregion
            var recordList = balances.Where(c => c.ExaminanTime == null && string.IsNullOrEmpty(c.ExaminanPeople)).ToList();
            foreach (var balanceitem in recordList)
            {
                var id = db.Process_Capacity_Total.Where(c => c.Type == balanceitem.Type && c.Platform == balanceitem.Platform && c.ProductPCBnumber == balanceitem.ProductPCBnumber).Select(c => c.Id).FirstOrDefault();
                JObject info = new JObject();
                info.Add("id", id); //id
                info.Add("type", balanceitem.Type);//型号
                info.Add("platform", balanceitem.Platform);//平台
                info.Add("PCBNumber", balanceitem.ProductPCBnumber);//pcb编号
                info.Add("Section", balanceitem.Section);//工段
                info.Add("title", balanceitem.Title);//标题
                result.Add(info);

            }
            #region 查找最后一个版本的集合,仅贴片表 
            List<Pick_And_Place> pcik = com.GetNewNumberPickInfo();
            #endregion
            var pickRecordList = pcik.Where(c => c.ExaminanTime == null && string.IsNullOrEmpty(c.ExaminanPeople)).ToList();
            foreach (var pickitem in pickRecordList)
            {
                var id = db.Process_Capacity_Total.Where(c => c.Type == pickitem.Type && c.Platform == pickitem.Platform && c.ProductPCBnumber == pickitem.ProductPCBnumber).Select(c => c.Id).FirstOrDefault();
                JObject info = new JObject();
                info.Add("id", id);
                info.Add("type", pickitem.Type);
                info.Add("platform", pickitem.Platform);
                info.Add("PCBNumber", pickitem.ProductPCBnumber);
                info.Add("Section", "SMT");
                info.Add("title", "SMT");
                result.Add(info);
            }
            return result;
        }

        /// <summary>
        /// 找出所有未批准的工序产能表
        /// </summary>
        /// <returns></returns>
        public JArray Process_CapacityNotApproverQuery()
        {
            JArray result = new JArray();
            #region 查找最后一个版本的集合,仅工序平衡表
            List<ProcessBalance> balances = com.GetNewNumberBalanceInfo();
            #endregion
            var recordList = balances.Where(c => c.ApproverTime == null && string.IsNullOrEmpty(c.ApproverPeople)).ToList();
            foreach (var balanceitem in recordList)
            {
                var id = db.Process_Capacity_Total.Where(c => c.Type == balanceitem.Type && c.Platform == balanceitem.Platform && c.ProductPCBnumber == balanceitem.ProductPCBnumber).Select(c => c.Id).FirstOrDefault();
                JObject info = new JObject();
                info.Add("id", id); //id
                info.Add("type", balanceitem.Type);//型号
                info.Add("platform", balanceitem.Platform);//平台
                info.Add("PCBNumber", balanceitem.ProductPCBnumber);//pcb编号
                info.Add("Section", balanceitem.Section);//工段
                info.Add("title", balanceitem.Title);//标题
                result.Add(info);

            }
            #region 查找最后一个版本的集合,仅贴片表
            List<Pick_And_Place> pcik = com.GetNewNumberPickInfo();
            #endregion
            var pickRecordList = pcik.Where(c => c.ApproverTime == null && string.IsNullOrEmpty(c.ApproverPeople)).ToList();
            foreach (var pickitem in pickRecordList)
            {
                var id = db.Process_Capacity_Total.Where(c => c.Type == pickitem.Type && c.Platform == pickitem.Platform && c.ProductPCBnumber == pickitem.ProductPCBnumber).Select(c => c.Id).FirstOrDefault();
                JObject info = new JObject();
                info.Add("id", id);
                info.Add("type", pickitem.Type);
                info.Add("platform", pickitem.Platform);
                info.Add("PCBNumber", pickitem.ProductPCBnumber);
                info.Add("Section", "SMT");
                info.Add("title", "SMT");
                result.Add(info);
            }
            return result;
        }

        /// <summary>
        /// 找出所有未受控的工序产能表
        /// </summary>
        /// <returns></returns>
        public JArray Process_CapacityNotControlledQuery()
        {
            JArray result = new JArray();
            #region 查找最后一个版本的集合,仅工序平衡表
            List<ProcessBalance> balances = com.GetNewNumberBalanceInfo();
            #endregion
            var recordList = balances.Where(c => c.ControlledTime == null && string.IsNullOrEmpty(c.ControlledPeople)).ToList();
            foreach (var balanceitem in recordList)
            {
                var id = db.Process_Capacity_Total.Where(c => c.Type == balanceitem.Type && c.Platform == balanceitem.Platform && c.ProductPCBnumber == balanceitem.ProductPCBnumber).Select(c => c.Id).FirstOrDefault();
                JObject info = new JObject();
                info.Add("id", id); //id
                info.Add("type", balanceitem.Type);//型号
                info.Add("platform", balanceitem.Platform);//平台
                info.Add("PCBNumber", balanceitem.ProductPCBnumber);//pcb编号
                info.Add("Section", balanceitem.Section);//工段
                info.Add("title", balanceitem.Title);//标题
                result.Add(info);

            }
            #region 查找最后一个版本的集合,仅贴片表
            List<Pick_And_Place> pcik = com.GetNewNumberPickInfo();
            #endregion
            var pickRecordList = pcik.Where(c => c.ControlledTime == null && string.IsNullOrEmpty(c.ControlledPeople)).ToList();
            foreach (var pickitem in pickRecordList)
            {
                var id = db.Process_Capacity_Total.Where(c => c.Type == pickitem.Type && c.Platform == pickitem.Platform && c.ProductPCBnumber == pickitem.ProductPCBnumber).Select(c => c.Id).FirstOrDefault();
                JObject info = new JObject();
                info.Add("id", id);
                info.Add("type", pickitem.Type);
                info.Add("platform", pickitem.Platform);
                info.Add("PCBNumber", pickitem.ProductPCBnumber);
                info.Add("Section", "SMT");
                info.Add("title", "SMT");
                result.Add(info);
            }
            return result;
        }

        /// <summary>
        /// 找出所有未核准的小样报告确认核准项
        /// </summary>
        /// <returns></returns>
        public JArray SmallSampleNotApprovedQuery()
        {
            JArray result = new JArray();
            JObject temp = new JObject();
            var recordList = db.Small_Sample.Where(c => c.Assessor != null && c.Approved == null).ToList();
            foreach (var item in recordList)
            {
                temp.Add("OrderNumber", item.OrderNumber);
                temp.Add("Id", item.Id);
                //temp.Add(item.OrderNumber, item.Id);
                result.Add(temp);
                temp = new JObject();
            }
            return result;
        }

        #region-----设备管理

        #region----报修单
        /// <summary>
        /// 找出所有未审核的报修单
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_Repairbill_AssessorQuery()
        {
            JArray equipment_list = new JArray();
            JObject repair = new JObject();
            JObject repair1 = new JObject();
            JObject repair2 = new JObject();
            var dep = "";
            if (Session["User"] != null)
            {
                dep = ((Users)Session["User"]).Department == "智造部" ? "技术部" : ((Users)Session["User"]).Department;
            }
            var maintenance = db.EquipmentRepairbill.Where(c => c.RepairName != null && c.DeparAssessor == null && c.UserDepartment == dep).ToList();
            foreach (var item in maintenance)//故障审核
            {
                repair.Add("UserDepartment", item.UserDepartment);
                repair.Add("EquipmentNumber", item.EquipmentNumber);//设备编号
                repair.Add("EquipmentName", item.EquipmentName);//设备名称
                repair.Add("faulttime", item.FaultTime);//故障时间
                repair.Add("id", item.Id);
                equipment_list.Add(repair);
                repair = new JObject();
            }
            var repa = db.EquipmentRepairbill.Where(c => c.MEName != null && c.TecDeparAssessor == null && dep == "技术部").ToList();
            foreach (var item1 in repa)//技术部意见审核
            {
                repair1.Add("EquipmentNumber", item1.EquipmentNumber);//设备编号
                repair1.Add("EquipmentName", item1.EquipmentName);//设备名称
                repair1.Add("faulttime", item1.FaultTime);//故障时间
                repair1.Add("id", item1.Id);
                equipment_list.Add(repair1);
                repair1 = new JObject();
            }
            var purch = db.EquipmentRepairbill.Where(c => c.CeApprove != null && c.Needto == true && c.OpinionName != null && c.OpinAssessor == null && dep == "采购部").ToList();
            if (purch.Count > 0)
            {
                foreach (var item2 in purch)//联建采购审核
                {
                    repair2.Add("EquipmentNumber", item2.EquipmentNumber);
                    repair2.Add("EquipmentName", item2.EquipmentName);//设备名称
                    repair2.Add("faulttime", item2.FaultTime);
                    repair2.Add("id", item2.Id);
                    equipment_list.Add(repair2);
                    repair2 = new JObject();
                }
            }
            return equipment_list;
        }

        /// <summary>
        /// 找出所有未批准的报修单
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_Repairbill_ApproveQuery()//报修单采购批准
        {
            JArray equipmentRe = new JArray();
            JObject Repair2 = new JObject();
            var opinapp = db.EquipmentRepairbill.Where(c => c.OpinAssessor != null && c.OpinApprove == null).ToList();
            foreach (var item2 in opinapp)//联建采购批准
            {
                Repair2.Add("EquipmentNumber", item2.EquipmentNumber);
                Repair2.Add("EquipmentName", item2.EquipmentName);//设备名称
                Repair2.Add("faulttime", item2.FaultTime);
                Repair2.Add("id", item2.Id);
                equipmentRe.Add(Repair2);
                Repair2 = new JObject();
            }
            return equipmentRe;
        }
        public JArray Equipment_Repairbill_ApproveQuery1()//报修单技术部意见批准
        {
            JArray equipmentRe = new JArray();
            JObject Repair1 = new JObject();
            var repalist = db.EquipmentRepairbill.Where(c => c.TecDeparAssessor != null && c.CeApprove == null && c.UserDepartment == "技术部").ToList();
            foreach (var item1 in repalist)//技术部意见批准
            {
                Repair1.Add("EquipmentNumber", item1.EquipmentNumber);
                Repair1.Add("EquipmentName", item1.EquipmentName);//设备名称
                Repair1.Add("faulttime", item1.FaultTime);
                Repair1.Add("id", item1.Id);
                equipmentRe.Add(Repair1);
                Repair1 = new JObject();
            }
            return equipmentRe;
        }
        public JArray Equipment_Repairbill_ApproveQuery2()//故障报修单批准
        {
            JArray equipmentRe = new JArray();
            JObject Repair = new JObject();
            var mainten = db.EquipmentRepairbill.Where(c => c.DeparAssessor != null && c.CenterApprove == null && c.UserDepartment != "技术部").ToList();
            foreach (var item in mainten)//故障批准
            {
                Repair.Add("UserDepartment", item.UserDepartment);
                Repair.Add("EquipmentNumber", item.EquipmentNumber);
                Repair.Add("EquipmentName", item.EquipmentName);//设备名称
                Repair.Add("faulttime", item.FaultTime);
                Repair.Add("id", item.Id);
                equipmentRe.Add(Repair);
                Repair = new JObject();
            }
            return equipmentRe;
        }
        public JArray Equipment_Repairbill_ApproveQuery3()//技术部故障报修单批准
        {
            JArray equipmentRe = new JArray();
            JObject Repair = new JObject();
            var mainten = db.EquipmentRepairbill.Where(c => c.DeparAssessor != null && c.CenterApprove == null && c.UserDepartment == "技术部").ToList();
            foreach (var item in mainten)//故障批准
            {
                Repair.Add("UserDepartment", item.UserDepartment);
                Repair.Add("EquipmentNumber", item.EquipmentNumber);
                Repair.Add("EquipmentName", item.EquipmentName);//设备名称
                Repair.Add("faulttime", item.FaultTime);
                Repair.Add("id", item.Id);
                equipmentRe.Add(Repair);
                Repair = new JObject();
            }
            return equipmentRe;
        }

        /// <summary>
        /// 找出所有未填写数据的报修单（技术部意见)
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_RepairbillQuery_TecDepar()
        {
            JArray equipmentRepair = new JArray();
            JObject repairs = new JObject();
            var repairbill = db.EquipmentRepairbill.Where(c => c.CenterApprove != null && c.TecDepar_opinion == null).ToList();
            var repairbi = db.EquipmentRepairbill.Where(c => c.CenterApprove == null && c.OpinAssessor != null && c.TecDepar_opinion == null && c.Emergency == "非常紧急").ToList();
            if (repairbill.Count > 0)
            {
                foreach (var item in repairbill)//技术部意见
                {
                    repairs.Add("EquipmentNumber", item.EquipmentNumber);
                    repairs.Add("EquipmentName", item.EquipmentName);//设备名称
                    repairs.Add("faulttime", item.FaultTime);
                    repairs.Add("id", item.Id);
                    equipmentRepair.Add(repairs);
                    repairs = new JObject();
                }
            }
            if (repairbi.Count > 0)
            {
                foreach (var item in repairbi)//技术部意见
                {
                    repairs.Add("EquipmentNumber", item.EquipmentNumber);
                    repairs.Add("EquipmentName", item.EquipmentName);//设备名称
                    repairs.Add("faulttime", item.FaultTime);
                    repairs.Add("id", item.Id);
                    equipmentRepair.Add(repairs);
                    repairs = new JObject();
                }
            }
            return equipmentRepair;
        }
        /// <summary>
        /// 找出所有未填写数据的报修单(联建采购意见）
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_RepairbillQuery_Purchasing()
        {
            JArray equipmentRepair = new JArray();
            JObject repairs1 = new JObject();
            var pubrch = db.EquipmentRepairbill.Where(c => c.CeApprove != null && c.Needto == true && c.Purchasing_opinion == null);
            foreach (var item1 in pubrch)//联建采购
            {
                repairs1.Add("EquipmentNumber", item1.EquipmentNumber);
                repairs1.Add("EquipmentName", item1.EquipmentName);//设备名称
                repairs1.Add("faulttime", item1.FaultTime);
                repairs1.Add("id", item1.Id);
                equipmentRepair.Add(repairs1);
                repairs1 = new JObject();
            }
            return equipmentRepair;
        }
        /// <summary>
        /// 找出所有未填写数据的报修单(维修人/厂家）
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_RepairbillQuery_MainName()
        {
            JObject repairs2 = new JObject();
            JArray equipmentRepair = new JArray();
            var Maintenan = db.EquipmentRepairbill.Where(c => c.CeApprove != null && c.Needto == true && c.OpinApprove != null && c.MainName == null).ToList();
            foreach (var item2 in Maintenan)//维修人/厂家
            {
                repairs2.Add("EquipmentNumber", item2.EquipmentNumber);
                repairs2.Add("EquipmentName", item2.EquipmentName);//设备名称
                repairs2.Add("faulttime", item2.FaultTime);
                repairs2.Add("id", item2.Id);
                equipmentRepair.Add(repairs2);
                repairs2 = new JObject();
            }
            return equipmentRepair;
        }
        /// <summary>
        /// 找出所有未填写数据的报修单(维修后确认（技术部）
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_RepairbillQuery_TcConfirmName()
        {
            JObject repairs3 = new JObject();
            JObject repairs4 = new JObject();
            JArray equipmentRepair = new JArray();
            if (db.EquipmentRepairbill.Count(c => c.CeApprove != null && c.Needto == false) > 0)
            {
                var tcafter = db.EquipmentRepairbill.Where(c => c.CeApprove != null && c.TcConfirmName == null && c.Needto == false).ToList();
                foreach (var item5 in tcafter)//维修后确认（技术部）
                {
                    repairs3.Add("EquipmentNumber", item5.EquipmentNumber);
                    repairs3.Add("EquipmentName", item5.EquipmentName);//设备名称
                    repairs3.Add("faulttime", item5.FaultTime);
                    repairs3.Add("id", item5.Id);
                    equipmentRepair.Add(repairs3);
                    repairs3 = new JObject();
                }
            }
            if (db.EquipmentRepairbill.Count(c => c.CeApprove != null && c.Needto == true && c.OpinApprove != null && c.MainName != null) > 0)
            {
                var after = db.EquipmentRepairbill.Where(c => c.MainName != null && c.TcConfirmName == null && c.Needto == true).ToList();
                foreach (var item3 in after)//维修后确认（技术部）
                {
                    repairs4.Add("EquipmentNumber", item3.EquipmentNumber);
                    repairs4.Add("EquipmentName", item3.EquipmentName);//设备名称
                    repairs4.Add("faulttime", item3.FaultTime);
                    repairs4.Add("id", item3.Id);
                    equipmentRepair.Add(repairs4);
                    repairs4 = new JObject();
                }

            }
            return equipmentRepair;
        }
        /// <summary>
        /// 找出所有未填写数据的报修单(维修后确认（维修需要部门）
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_RepairbillQuery_AfterMain()
        {
            JObject repairs5 = new JObject();
            JObject repairs6 = new JObject();
            JArray equipmentRepair = new JArray();
            var dep = "";
            if (Session["User"] != null)
            {
                dep = ((Users)Session["User"]).Department == "智造部" ? "技术部" : ((Users)Session["User"]).Department;
            }
            if (db.EquipmentRepairbill.Count(c => c.CeApprove != null && c.Needto == true && c.OpinApprove != null && c.MainName != null) > 0)
            {
                var confir = db.EquipmentRepairbill.Where(c => c.MainName != null && c.Needto == true && c.AfterMain == null && c.UserDepartment == dep).ToList();
                foreach (var item4 in confir)//维修后确认（维修需要部门）
                {
                    repairs5.Add("UserDepartment", item4.UserDepartment);
                    repairs5.Add("EquipmentNumber", item4.EquipmentNumber);
                    repairs5.Add("EquipmentName", item4.EquipmentName);//设备名称
                    repairs5.Add("faulttime", item4.FaultTime);
                    repairs5.Add("id", item4.Id);
                    equipmentRepair.Add(repairs5);
                    repairs5 = new JObject();
                }
            }
            if (db.EquipmentRepairbill.Count(c => c.CeApprove != null && c.Needto == false) > 0)
            {
                var tcconfir = db.EquipmentRepairbill.Where(c => c.CeApprove != null && c.Needto == false && c.AfterMain == null && c.UserDepartment == dep).ToList();
                foreach (var item6 in tcconfir)//维修后确认（维修需要部门）
                {
                    repairs6.Add("UserDepartment", item6.UserDepartment);
                    repairs6.Add("EquipmentNumber", item6.EquipmentNumber);
                    repairs6.Add("EquipmentName", item6.EquipmentName);//设备名称
                    repairs6.Add("faulttime", item6.FaultTime);
                    repairs6.Add("id", item6.Id);
                    equipmentRepair.Add(repairs6);
                    repairs6 = new JObject();
                }
            }
            return equipmentRepair;
        }

        #endregion

        #region---点检保养记录
        /// <summary>
        /// 找出所有未确认点检保养的点检保养表（周保养，月保养，生产确认，部长确认）
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_TallyQuery()
        {
            JArray equipmenttally = new JArray();
            JObject record = new JObject();
            JObject record1 = new JObject();
            JObject record2 = new JObject();
            JObject record3 = new JObject();
            JObject record4 = new JObject();
            JObject record5 = new JObject();
            JObject record6 = new JObject();
            JObject record7 = new JObject();
            JObject record8 = new JObject();
            JObject record9 = new JObject();
            JObject record10 = new JObject();
            DateTime dt = DateTime.Now;
            var tally = db.Equipment_Tally_maintenance.Where(c => c.Day_group_8 != null && c.Week_Main_1 == null && c.Year == dt.Year && c.Month == dt.Month).ToList();
            foreach (var item in tally)//第一周保养
            {
                record.Add("EquipmentNumber", item.EquipmentNumber);//设备编号
                record.Add("year", item.Year);//年
                record.Add("month", item.Month);//月
                record.Add("Id", item.Id);
                equipmenttally.Add(record);
                record = new JObject();
            }
            var tally2 = db.Equipment_Tally_maintenance.Where(c => c.Day_group_14 != null && c.Week_Main_2 == null && c.Year == dt.Year && c.Month == dt.Month).ToList();
            foreach (var item1 in tally2)//第二周保养
            {
                record1.Add("EquipmentNumber", item1.EquipmentNumber);
                record1.Add("year", item1.Year);
                record1.Add("month", item1.Month);
                record1.Add("Id", item1.Id);
                equipmenttally.Add(record1);
                record1 = new JObject();
            }
            var tally3 = db.Equipment_Tally_maintenance.Where(c => c.Day_group_24 != null && c.Week_Main_3 == null && c.Year == dt.Year && c.Month == dt.Month).ToList();
            foreach (var item2 in tally3)//第三周保养
            {
                record2.Add("EquipmentNumber", item2.EquipmentNumber);
                record2.Add("year", item2.Year);
                record2.Add("month", item2.Month);
                record2.Add("Id", item2.Id);
                equipmenttally.Add(record2);
                record2 = new JObject();
            }
            var tally4 = db.Equipment_Tally_maintenance.Where(c => c.Day_group_31 != null && c.Week_Main_4 == null && c.Year == dt.Year && c.Month == dt.Month).ToList();
            foreach (var item3 in tally4)//第四周保养
            {
                record3.Add("EquipmentNumber", item3.EquipmentNumber);
                record3.Add("year", item3.Year);
                record3.Add("month", item3.Month);
                record3.Add("Id", item3.Id);
                equipmenttally.Add(record3);
                record3 = new JObject();
            }
            var engineer = db.Equipment_Tally_maintenance.Where(c => c.Week_Main_1 != null && c.Week_engineer_1 == null && c.Year == dt.Year && c.Month == dt.Month).ToList();
            foreach (var eng in engineer)//第一周工程师确认
            {
                record4.Add("EquipmentNumber", eng.EquipmentNumber);
                record4.Add("year", eng.Year);
                record4.Add("month", eng.Month);
                record4.Add("Id", eng.Id);
                equipmenttally.Add(record4);
                record4 = new JObject();
            }
            var engineer2 = db.Equipment_Tally_maintenance.Where(c => c.Week_Main_2 != null && c.Week_engineer_2 == null && c.Year == dt.Year && c.Month == dt.Month).ToList();
            foreach (var eng2 in engineer2)//第二周工程师确认
            {
                record5.Add("EquipmentNumber", eng2.EquipmentNumber);
                record5.Add("year", eng2.Year);
                record5.Add("month", eng2.Month);
                record5.Add("Id", eng2.Id);
                equipmenttally.Add(record5);
                record5 = new JObject();
            }
            var engineer3 = db.Equipment_Tally_maintenance.Where(c => c.Week_Main_3 != null && c.Week_engineer_3 == null && c.Year == dt.Year && c.Month == dt.Month).ToList();
            foreach (var eng3 in engineer3)//第三周工程师确认
            {
                record6.Add("EquipmentNumber", eng3.EquipmentNumber);
                record6.Add("year", eng3.Year);
                record6.Add("month", eng3.Month);
                record6.Add("Id", eng3.Id);
                equipmenttally.Add(record6);
                record6 = new JObject();
            }
            var engineer4 = db.Equipment_Tally_maintenance.Where(c => c.Week_Main_4 != null && c.Week_engineer_4 == null && c.Year == dt.Year && c.Month == dt.Month).ToList();
            foreach (var eng4 in engineer4)//第四周工程师确认
            {
                record7.Add("EquipmentNumber", eng4.EquipmentNumber);
                record7.Add("year", eng4.Year);
                record7.Add("month", eng4.Month);
                record7.Add("Id", eng4.Id);
                equipmenttally.Add(record7);
                record7 = new JObject();
            }
            var monthMain = db.Equipment_Tally_maintenance.Where(c => c.Week_engineer_4 != null && c.Month_main_1 == null && c.Year == dt.Year && c.Month == dt.Month).ToList();
            foreach (var main in monthMain)//月保养
            {
                record8.Add("EquipmentNumber", main.EquipmentNumber);
                record8.Add("year", main.Year);
                record8.Add("month", main.Month);
                record8.Add("Id", main.Id);
                equipmenttally.Add(record8);
                record8 = new JObject();
            }
            var monthMain2 = db.Equipment_Tally_maintenance.Where(c => c.Month_main_1 != null && c.Month_productin_2 == null && c.Year == dt.Year && c.Month == dt.Month).ToList();
            foreach (var main2 in monthMain2)//生产确认（月保养）
            {
                record9.Add("EquipmentNumber", main2.EquipmentNumber);
                record9.Add("year", main2.Year);
                record9.Add("month", main2.Month);
                record9.Add("Id", main2.Id);
                equipmenttally.Add(record9);
                record9 = new JObject();
            }
            var monthMain3 = db.Equipment_Tally_maintenance.Where(c => c.Month_productin_2 != null && c.Month_minister_3 == null && c.Year == dt.Year && c.Month == dt.Month).ToList();
            foreach (var main3 in monthMain3)//部长确认（月保养）
            {
                record10.Add("EquipmentNumber", main3.EquipmentNumber);
                record10.Add("year", main3.Year);
                record10.Add("month", main3.Month);
                record10.Add("Id", main3.Id);
                equipmenttally.Add(record10);
                record10 = new JObject();
            }
            return equipmenttally;
        }

        #endregion

        //#region---月保养时间计划表
        ///// <summary>
        ///// 找出所有技术部未确认的月保养时间计划表
        ///// </summary>
        ///// <returns></returns>
        //public JArray Equipment_Tec_NotarizeQuery()
        //{
        //    JArray Timeplan = new JArray();
        //    JObject plan = new JObject();
        //    DateTime dt = DateTime.Now;
        //    DateTime d1 = new DateTime(dt.Year, dt.Month, 1);
        //    DateTime d2 = d1.AddMonths(1).AddDays(-1);
        //    var tem2 = db.Equipment_MonthlyMaintenance.Where(c => c.Mainten_Lister != null && c.Tec_Notarize == null && dt <= d2).Select(c=>new { c.UserDepartment,c.Year ,c.Month}).Distinct().ToList();
        //    foreach (var main2 in tem2)//技术部确认
        //    {
        //        plan.Add("UserDepartment", main2.UserDepartment);
        //        plan.Add("year", main2.Year);
        //        plan.Add("month", main2.Month);
        //        Timeplan.Add(plan);
        //        plan = new JObject();
        //    }
        //    return Timeplan;
        //}

        ///// <summary>
        ///// 找出所有保养设备部门未确认的月保养时间计划表
        ///// </summary>
        ///// <returns></returns>
        //public JArray Equipment_MonthlyMainQuery()
        //{
        //    JArray Timeplan = new JArray();
        //    JObject plan = new JObject();

        //    var tem = db.Equipment_MonthlyMaintenance.Where(c => c.Tec_Notarize != null && c.AssortDepar == null ).Select(c => new { c.UserDepartment, c.Year, c.Month }).Distinct().ToList();
        //    foreach (var main in tem)//保养设备部门确认
        //    {
        //        plan.Add("UserDepartment", main.UserDepartment);//使用部门
        //        plan.Add("year", main.Year);//年
        //        plan.Add("month", main.Month);//月
        //        Timeplan.Add(plan);
        //        plan = new JObject();
        //    }
        //    return Timeplan;
        //}

        ///// <summary>
        ///// 找出所有PC部未确认的月保养时间计划表
        ///// </summary>
        ///// <returns></returns>
        //public JArray Equipment_PCDeparQuery()
        //{
        //    JArray Timeplan = new JArray();
        //    JObject plan = new JObject();
        //    var tem3 = db.Equipment_MonthlyMaintenance.Where(c => c.Tec_Notarize != null && c.PCDepar == null).Select(c => new { c.UserDepartment, c.Year, c.Month }).Distinct().ToList();
        //    foreach (var main3 in tem3)//PC部确认
        //    {
        //        plan.Add("UserDepartment", main3.UserDepartment);
        //        plan.Add("year", main3.Year);
        //        plan.Add("month", main3.Month);
        //        Timeplan.Add(plan);
        //        plan = new JObject();
        //    }
        //    return Timeplan;
        //}

        ///// <summary>
        ///// 找出所有未审核的月保养时间计划表
        ///// </summary>
        ///// <returns></returns>
        //public JArray Equipment_AssessorQuery()
        //{
        //    JArray Timeplan = new JArray();
        //    JObject plan = new JObject();
        //    var tem4 = db.Equipment_MonthlyMaintenance.Where(c => c.PCDepar != null && c.Assessor == null).Select(c => new { c.UserDepartment, c.Year, c.Month }).Distinct().ToList();
        //    foreach (var main4 in tem4)//审核
        //    {
        //        plan.Add("UserDepartment", main4.UserDepartment);
        //        plan.Add("year", main4.Year);
        //        plan.Add("month", main4.Month);
        //        Timeplan.Add(plan);
        //        plan = new JObject();
        //    }
        //    return Timeplan;
        //}

        //#endregion

        #region---质量指标达成状况统计表
        /// <summary>
        /// 找出所有未审核的质量指标达成状况统计表
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_QualitytargetQuery()
        {
            JArray quality = new JArray();
            JObject target = new JObject();
            DateTime dt = DateTime.Now;
            var quatar = db.Equipment_Quality_target.Where(c => c.Year == dt.Year && c.Month != dt.Month).Select(c => c.Month).Distinct().ToList();
            foreach (var item in quatar)//审核
            {
                var list = db.Equipment_Quality_target.Where(c => c.PrepareName != null && c.Assessor == null & c.Month == item).FirstOrDefault();
                if (list != null)
                {
                    target.Add("Name", "质量指标");
                    target.Add("Year", list.Year);
                    target.Add("Month", item);
                    quality.Add(target);
                    target = new JObject();
                }
            }
            return quality;
        }

        /// <summary>
        /// 找出所有未批准的质量指标达成状况统计表
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_ApprovetQuery()
        {
            JArray quality = new JArray();
            JObject target = new JObject();
            DateTime dt = DateTime.Now;
            var quatar = db.Equipment_Quality_target.Where(c => c.Year == dt.Year && c.Month != dt.Month).Select(c => c.Month).Distinct().ToList();
            foreach (var item2 in quatar)//批准
            {
                var list = db.Equipment_Quality_target.Where(c => c.Assessor != null && c.Approve == null && c.Year == dt.Year && c.Month == item2).FirstOrDefault();
                if (list != null)
                {
                    target.Add("Name", "质量指标");
                    target.Add("Year", list.Year);
                    target.Add("Month", item2);
                    quality.Add(target);
                    target = new JObject();
                }
            }
            return quality;
        }
        #endregion

        #region---安全库存清单
        /// <summary>
        /// 取出所有技术部未审核的安全库存清单
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_SafetyTec_AssessorQuery()
        {
            JArray safety = new JArray();
            JObject stock = new JObject();
            var depar = db.Equipment_Safetystock.Where(c => c.FinishingName != null && c.Tec_Assessor == null).Select(c => new { c.Year, c.Month }).Distinct().ToList();
            foreach (var item in depar)
            {
                stock.Add("Name", "安全库存清单");
                stock.Add("Year", item.Year);
                stock.Add("Month", item.Month);
                safety.Add(stock);
                stock = new JObject();
            }
            return safety;
        }

        /// <summary>
        /// 取出所有MC部未审核的安全库存清单
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_SafetyAssessorQuery()
        {
            JArray safety = new JArray();
            JObject stock = new JObject();
            var depar = db.Equipment_Safetystock.Where(c => c.Tec_Assessor != null && c.Assessor == null).Select(c => new { c.Year, c.Month }).Distinct().ToList();
            foreach (var item in depar)
            {
                stock.Add("Name", "安全库存清单");
                stock.Add("Year", item.Year);
                stock.Add("Month", item.Month);
                safety.Add(stock);
                stock = new JObject();
            }
            return safety;
        }

        /// <summary>
        /// 取出所有工厂厂长未批准的安全库存清单Equipment_SafetyApproveQuery
        /// </summary>
        /// <returns></returns>
        public JArray Equipment_SafetyApproveQuery()
        {
            JArray safety = new JArray();
            JObject stock = new JObject();
            var depar = db.Equipment_Safetystock.Where(c => c.Assessor != null && c.Approve == null).Select(c => new { c.Year, c.Month }).Distinct().ToList();
            foreach (var item in depar)
            {
                stock.Add("Name", "安全库存清单");
                stock.Add("Year", item.Year);
                stock.Add("Month", item.Month);
                safety.Add(stock);
                stock = new JObject();
            }
            return safety;
        }

        #endregion

        #endregion

        /// <summary>
        /// 取出所有用户和工号
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserNameUserId()
        {
            var result = db.Users.Select(c => new { c.UserName, c.UserNum });
            return Content(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 取出所有权限名
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRoleName()
        {
            var result = db.UserRolelistTable.Select(c => c.Discription).Distinct();
            return Content(JsonConvert.SerializeObject(result));
        }

        #endregion



        #region------添加修改删除提示项名
        //提示项名管理页面
        public ActionResult TipsItemManagement()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TipsItemManagement_add(UserInformationTips tipsItem)
        {
            //检查添加的提示项名称是否在权限名称列表中
            var count = db.UserRolelistTable.Count(c => c.RolesName == tipsItem.Matters);
            if (count > 0) return Content("提示项名称不权限名之中，请确认！");
            else
            {
                int tipsItemCount = db.UserInformationTips.Count(c => c.Matters == tipsItem.Matters);
                if (tipsItemCount > 0)
                {
                    return Content(tipsItem.Matters + "已经存在!");
                }
                else
                {
                    db.UserInformationTips.Add(tipsItem);
                    db.SaveChanges();
                    return Content("提示项“" + tipsItem.Matters + "”添加成功。");
                }
            }
        }

        [HttpPost]
        public ActionResult TipsItemManagement_modify(UserInformationTips tipsItem)
        {
            //检查修改后的提示项名称是否在权限名称列表中
            var originMatter = db.UserInformationTips.Find(tipsItem.id).Matters;
            var isExist = db.UserRolelistTable.Count(c => c.RolesName == tipsItem.Matters);
            if (isExist > 0)
            {
                DbEntityEntry<UserInformationTips> entry = db.Entry(tipsItem);
                entry.State = EntityState.Modified;
                //记录修改动作记录在UserOperateLog中
                UserOperateLog operaterecord = new UserOperateLog();
                operaterecord.OperateDT = DateTime.Now;
                operaterecord.Operator = ((Users)Session["User"]).UserName;
                operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "修改提示项名称：" + originMatter + "==>" + tipsItem.Matters;
                db.UserOperateLog.Add(operaterecord);
                int count = db.SaveChanges();
                if (count > 0) return Content("修改提示项名称：" + originMatter + "==>" + tipsItem.Matters + "修改成功。");
                return Content("");
            }
            else return Content("提示项名称不权限名之中，请确认！");
        }

        [HttpPost]
        public ActionResult TipsItemManagement_delete(int id)
        {
            //检查被删除项在UserNeedsPrompt表中是否有授权给用户，如果有则要求要先删除所有授权才能删除
            var tipsItem = db.UserInformationTips.Find(id);
            var userNeedsPromptRecordCount = db.UserNeedsPrompt.Count(c => c.Matters == tipsItem.Matters);
            if (userNeedsPromptRecordCount > 0) return Content(tipsItem.Matters + "已经授权了" + userNeedsPromptRecordCount + "个用户提示信息，如要删除" + tipsItem.Matters + "提示项，需要先解除相应的授权。");
            else
            {
                db.UserInformationTips.Remove(tipsItem);
                //记录删除动作记录在UserOperateLog中
                UserOperateLog operaterecord = new UserOperateLog();
                operaterecord.OperateDT = DateTime.Now;
                operaterecord.Operator = ((Users)Session["User"]).UserName;
                operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除提示项名称：" + tipsItem.Matters;
                db.UserOperateLog.Add(operaterecord);
                var count = db.SaveChanges();
                if (count > 0) return Content("删除" + tipsItem.Matters + "提示项成功。");
            }
            return Content("删除" + tipsItem.Matters + "提示项失败");
        }

        #endregion



        #region------用户添加删除需要的提示项

        [HttpPost]
        public ActionResult UserTipsItem_add(UserNeedsPrompt tipsItem)
        {
            var isexist = db.UserNeedsPrompt.Count(c => c.Matters == tipsItem.Matters && c.UserName == tipsItem.UserName);
            if (isexist > 0) return Content(tipsItem.UserName + "已有" + tipsItem.Matters + "权限");
            else
            {
                db.UserNeedsPrompt.Add(tipsItem);
                var count = db.SaveChanges();
                if (count > 0) return Content("保存成功");
                else return Content("保存失败");
            };
        }

        [HttpPost]
        public ActionResult UserTipsItem_delete(int id)
        {
            var record = db.UserNeedsPrompt.Find(id);
            db.UserNeedsPrompt.Remove(record);
            //记录删除动作记录在UserOperateLog中
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;
            operaterecord.Operator = ((Users)Session["User"]).UserName;
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除" + record.UserName + "的" + record.Matters + "提示项成功。";
            db.UserOperateLog.Add(operaterecord);
            var count = db.SaveChanges();
            if (count > 0) return Content("删除" + record.UserName + "的" + record.Matters + "提示项成功。");
            return Content("删除" + record.UserName + "的" + record.Matters + "提示项失败。");
        }
        #endregion

        #region---页面
        public ActionResult Tips_Arguments()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Tips", act = "Tips_Arguments" });
            }
            return View();
        }
        public ActionResult Tips_Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Tips", act = "Tips_Index" });
            }
            return View();
        }
        #endregion
        // GET: Tips/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserInformationTips userInformationTips = db.UserInformationTips.Find(id);
            if (userInformationTips == null)
            {
                return HttpNotFound();
            }
            return View(userInformationTips);
        }

        // GET: Tips/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tips/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Matters,Creator,CreateTime,Modifier,ModifyTime")] UserInformationTips userInformationTips)
        {
            if (ModelState.IsValid)
            {
                db.UserInformationTips.Add(userInformationTips);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userInformationTips);
        }

        // GET: Tips/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserInformationTips userInformationTips = db.UserInformationTips.Find(id);
            if (userInformationTips == null)
            {
                return HttpNotFound();
            }
            return View(userInformationTips);
        }

        // POST: Tips/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Matters,Creator,CreateTime,Modifier,ModifyTime")] UserInformationTips userInformationTips)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userInformationTips).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userInformationTips);
        }

        // GET: Tips/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserInformationTips userInformationTips = db.UserInformationTips.Find(id);
            if (userInformationTips == null)
            {
                return HttpNotFound();
            }
            return View(userInformationTips);
        }

        // POST: Tips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserInformationTips userInformationTips = db.UserInformationTips.Find(id);
            db.UserInformationTips.Remove(userInformationTips);
            db.SaveChanges();
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
    }
}
