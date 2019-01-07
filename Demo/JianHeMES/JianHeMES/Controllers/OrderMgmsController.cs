using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json.Linq;

namespace JianHeMESEntities.Controllers
{
    public class OrderMgmsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: OrderMgms

        #region --------------------GetOrderNumList()检索订单号
        private List<SelectListItem> GetOrderNumList()
        {
            var ordernum = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum).Distinct();

            var ordernumitems = new List<SelectListItem>();
            foreach (string num in ordernum)
            {
                ordernumitems.Add(new SelectListItem
                {
                    Text = num,
                    Value = num
                });
            }
            return ordernumitems;
        }
        #endregion

        #region --------------------分页
        private static readonly int PAGE_SIZE = 10;

        private int GetPageCount(int recordCount)
        {
            int pageCount = recordCount / PAGE_SIZE;
            if (recordCount % PAGE_SIZE != 0)
            {
                pageCount += 1;
            }
            return pageCount;
        }
        #endregion

        #region --------------------首页
        // GET: OrderMgms
        public ActionResult Index()
        {
            ViewBag.OrderNumList = GetOrderNumList();//向View传递OrderNum订单号列表.
            //ViewBag.BarCodeTypeList = GetBarCodeTypeList();
            //return View(GetPagedDataSource(barcodes, 0, recordCount));

            ViewBag.Display = "display:none";//隐藏View基本情况信息
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string OrderNum, string searchString, string PlatformType, int PageIndex = 0)
        {
            var ordernums = db.OrderMgm as IQueryable<OrderMgm>;
            if (!String.IsNullOrEmpty(OrderNum))
            {
                ordernums = ordernums.Where(m => m.OrderNum == OrderNum);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                ordernums = ordernums.Where(m => m.OrderNum.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(PlatformType))
            {
                ordernums = ordernums.Where(m => m.PlatformType.Contains(PlatformType));
            }

            var recordCount = ordernums.Count();
            var pageCount = GetPageCount(recordCount);
            if (PageIndex >= pageCount && pageCount >= 1)
            {
                PageIndex = pageCount - 1;
            }

            ordernums = ordernums.OrderBy(m => m.OrderNum)
                 .Skip(PageIndex * PAGE_SIZE).Take(PAGE_SIZE);
            ViewBag.PageIndex = PageIndex;
            ViewBag.PageCount = pageCount;

            ViewBag.OrderNumList = GetOrderNumList();
            return View(ordernums.ToList());
        }
        #endregion

        #region --------------------Details页
        // GET: OrderMgms/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }

            #region----------订单各工段详细信息
            JObject ProductionDetailsJson = new JObject();
            int modelGroupQuantity = db.OrderMgm.Where(c => c.OrderNum == orderMgm.OrderNum).FirstOrDefault().Boxes;//订单模组数量

            #region----------订单在组装的统计数据
            var assembleRecord = db.Assemble.Where(c => c.OrderNum == orderMgm.OrderNum).ToList();//订单在组装的全部记录
            //开始时间 
            var assemble_begintime = assembleRecord.Min(c => c.PQCCheckBT);
            ProductionDetailsJson.Add("AssembleBeginTime", assemble_begintime.ToString());//开始时间

            //最后时间
            var assemble_endtime = assembleRecord.Max(c => c.PQCCheckFT);
            ProductionDetailsJson.Add("AssembleEndTime", assemble_endtime.ToString());//最后时间

            //完成时间
            DateTime? assemble_finishtime = new DateTime();
            if (assembleRecord.Count(c => c.PQCCheckFinish == true) == modelGroupQuantity)
            {
                assemble_finishtime = assemble_endtime;
            }
            else
            {
                assemble_finishtime = null;
            }
            ProductionDetailsJson.Add("AssembleFinishTime", assemble_finishtime.ToString());//完成时间

            //作业时长
            var assemble_newtime = assemble_endtime - assemble_begintime;//目前时长
            ProductionDetailsJson.Add("AssembleNewTime", assemble_newtime.ToString());//目前时长

            //直通个数
            var assemble_NotFirstPassYield = assembleRecord.Where(c=>c.PQCCheckFinish == false).Select(c=>c.BoxBarCode).Distinct().ToList();
            var assemble_PassYield = assembleRecord.Where(c=>c.PQCCheckFinish == true && c.RepetitionPQCCheck==false).Select(c => c.BoxBarCode).ToList();
            var assemble_FirstPassYield = assemble_PassYield.Except(assemble_NotFirstPassYield).ToList();//直通条码记录
            var assemble_FirstPassYieldCount = assemble_FirstPassYield.Count();//直通个数
            ProductionDetailsJson.Add("AssembleFirstPassYieldCount", assemble_FirstPassYieldCount.ToString());//直通个数
            if(assemble_FirstPassYieldCount==0)
            {
                ProductionDetailsJson.Add("AssembleFirstPassYield_Rate", "");//直通率
            }
            else
            {
                var assemble_FirstPassYield_Rate = (Convert.ToDouble(assemble_FirstPassYieldCount) / modelGroupQuantity * 100).ToString("F2");//直通率：直通数/模组数
                ProductionDetailsJson.Add("AssembleFirstPassYield_Rate", assemble_FirstPassYield_Rate);//直通率
            }

            //正常个数
            int PassYieldCount = assemble_PassYield.Count();
            ProductionDetailsJson.Add("AssemblePassYieldCount", PassYieldCount.ToString());//正常个数

            //有效工时
            //异常个数
            //异常工时

            //完成率
            var assemble_finished = assembleRecord.Count(m => m.PQCCheckFinish == true && m.RepetitionPQCCheck==false);//订单已完成PQC个数
            var assemble_finishedList = assembleRecord.Where(m => m.PQCCheckFinish == true).Select(m => m.BoxBarCode).ToList(); //订单已完成PQC的条码清单
            var assemble_assemblePQC_Count = assembleRecord.Count();//订单PQC全部记录条数
            if(assemble_finished==0)
            {
                ProductionDetailsJson.Add("AssembleFinisthRate", "");//完成率
                ProductionDetailsJson.Add("AssemblePassRate", "");//合格率
            }
            else
            {
                var assemble_finisthRate = (Convert.ToDouble(assemble_finished) / modelGroupQuantity * 100).ToString("F2");//完成率：完成数/订单的模组数
                ProductionDetailsJson.Add("AssembleFinisthRate", assemble_finisthRate);//完成率
                //合格率
                var passRate = (Convert.ToDouble(assemble_finished) / assemble_assemblePQC_Count * 100).ToString("F2");//合格率：完成数/记录数
                ProductionDetailsJson.Add("AssemblePassRate", passRate);//合格率
            }

            #endregion


            #region----------订单在FQC的统计数据
            var FinalQCRecord = db.FinalQC.Where(c => c.OrderNum == orderMgm.OrderNum).ToList();//订单在组装的全部记录
            //开始时间 
            var FinalQC_begintime = FinalQCRecord.Min(c => c.FQCCheckBT);
            ProductionDetailsJson.Add("FinalQCBeginTime", FinalQC_begintime.ToString());//开始时间

            //最后时间
            var FinalQC_endtime = FinalQCRecord.Max(c => c.FQCCheckFT);
            ProductionDetailsJson.Add("FinalQCEndTime", FinalQC_endtime.ToString());//最后时间

            //完成时间
            DateTime? FinalQC_finishtime = new DateTime();
            if (FinalQCRecord.Count(c => c.FQCCheckFinish == true) == modelGroupQuantity)
            {
                FinalQC_finishtime = FinalQC_endtime;
            }
            else
            {
                FinalQC_finishtime = null;
            }
            ProductionDetailsJson.Add("FinalQCFinishTime", FinalQC_finishtime.ToString());//完成时间

            //作业时长
            var FinalQC_newtime = FinalQC_endtime - FinalQC_begintime;//目前时长
            ProductionDetailsJson.Add("FinalQCNewTime", FinalQC_newtime.ToString());//目前时长

            //直通个数
            var FinalQC_NotFirstPassYield = FinalQCRecord.Where(c => c.FQCCheckFinish == false).Select(c => c.BarCodesNum).Distinct().ToList();
            var FinalQC_PassYield = FinalQCRecord.Where(c => c.FQCCheckFinish == true && c.RepetitionFQCCheck==false).Select(c => c.BarCodesNum).ToList();
            var FinalQC_FirstPassYield = FinalQC_PassYield.Except(FinalQC_NotFirstPassYield).ToList();//直通条码记录
            var FinalQC_FirstPassYieldCount = FinalQC_FirstPassYield.Count();//直通个数
            ProductionDetailsJson.Add("FinalQCFirstPassYieldCount", FinalQC_FirstPassYieldCount.ToString());//直通个数
            if (FinalQC_FirstPassYieldCount == 0)
            {
                ProductionDetailsJson.Add("FinalQCFirstPassYield_Rate", "");//直通率
            }
            else
            {
                var FinalQC_FirstPassYield_Rate = (Convert.ToDouble(FinalQC_FirstPassYieldCount) / modelGroupQuantity * 100).ToString("F2");//直通率：直通数/模组数
                ProductionDetailsJson.Add("FinalQCFirstPassYield_Rate", FinalQC_FirstPassYield_Rate);//直通率
            }

            //正常个数
            int FinalQC_PassYieldCount = FinalQC_PassYield.Count();
            ProductionDetailsJson.Add("FinalQCPassYieldCount", FinalQC_PassYieldCount.ToString());//正常个数

            //有效工时
            //异常个数
            //异常工时

            //完成率
            var FinalQC_finished = FinalQCRecord.Count(m => m.FQCCheckFinish == true && m.RepetitionFQCCheck==false);//订单已完成PQC个数
            var FinalQC_finishedList = FinalQCRecord.Where(m => m.FQCCheckFinish == true).Select(m => m.BarCodesNum).ToList(); //订单已完成PQC的条码清单
            var FinalQC_Count = FinalQCRecord.Count();//订单PQC全部记录条数
            if (FinalQC_finished == 0)
            {
                ProductionDetailsJson.Add("FinalQCFinisthRate", "");//完成率
                ProductionDetailsJson.Add("FinalQCPassRate", "");//合格率
            }
            else
            {
                var FinalQC_finisthRate = (Convert.ToDouble(FinalQC_finished) / modelGroupQuantity * 100).ToString("F2");//完成率：完成数/订单的模组数
                ProductionDetailsJson.Add("FinalQCFinisthRate", FinalQC_finisthRate);//完成率
                //合格率
                var passRate = (Convert.ToDouble(FinalQC_finished) / FinalQC_Count * 100).ToString("F2");//合格率：完成数/记录数
                ProductionDetailsJson.Add("FinalQCPassRate", passRate);//合格率
            }
            #endregion


            #region----------订单在老化的统计数据
            var burn_inRecord = db.Burn_in.Where(c => c.OrderNum == orderMgm.OrderNum).ToList();//订单在老化的全部记录
            //开始时间 
            var burn_in_begintime = burn_inRecord.Min(c => c.OQCCheckBT);
            ProductionDetailsJson.Add("Burn_inBeginTime", burn_in_begintime.ToString());//开始时间 

            //最后时间
            var burn_in_endtime = burn_inRecord.Max(c => c.OQCCheckFT);
            ProductionDetailsJson.Add("Burn_inEndTime", burn_in_endtime.ToString());//最后时间

            //完成时间
            DateTime? burn_infinishtime = new DateTime();
            if (burn_inRecord.Count(c => c.OQCCheckFinish == true) == modelGroupQuantity)
            {
                burn_infinishtime = burn_in_endtime;
            }
            else
            {
                burn_infinishtime = null;
            }
            ProductionDetailsJson.Add("Burn_inFinishTime", burn_infinishtime.ToString());//完成时间

            //作业时长
            var burn_innewtime = burn_in_endtime - burn_in_begintime;//目前时长
            ProductionDetailsJson.Add("Burn_inNewTime", burn_innewtime.ToString());//目前时长

            //直通个数
            var burn_in_NotFirstPassYield = burn_inRecord.Where(c => c.OQCCheckFinish == false).Select(c => c.BarCodesNum).Distinct().ToList();
            var burn_in_PassYield = burn_inRecord.Where(c => c.OQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
            var burn_in_FirstPassYield = burn_in_PassYield.Except(burn_in_NotFirstPassYield).ToList();//直通条码记录
            var burn_in_FirstPassYieldCount = burn_in_PassYield.Count();//直通个数
            ProductionDetailsJson.Add("Burn_inFirstPassYieldCount", burn_in_FirstPassYieldCount.ToString());//直通个数
            if(burn_in_FirstPassYieldCount==0)
            {
                ProductionDetailsJson.Add("Burn_inFirstPassYield_Rate", "");//直通率
            }
            else
            {
                var burn_in_FirstPassYield_Rate = (Convert.ToDouble(burn_in_FirstPassYieldCount) / burn_inRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2");//直通率：直通数/模组数
                ProductionDetailsJson.Add("Burn_inFirstPassYield_Rate", burn_in_FirstPassYield_Rate);//直通率
            }

            //正常个数
            int burn_inPassYieldCount = burn_in_PassYield.Count();
            ProductionDetailsJson.Add("Burn_inPassYieldCount", burn_inPassYieldCount.ToString());

            //有效工时
            //异常个数
            //异常工时

            //完成率
            var burn_in_finished = burn_inRecord.Count(m => m.OQCCheckFinish == true);//订单已完成PQC个数
            var burn_in_finishedList = burn_inRecord.Where(m => m.OQCCheckFinish == true).Select(m => m.BarCodesNum).ToList(); //订单已完成PQC的条码清单
            var burn_in_Count = burn_inRecord.Count();//订单PQC全部记录条数
            if(burn_in_Count==0)
            {
                ProductionDetailsJson.Add("Burn_inFinisthRate", "");
                ProductionDetailsJson.Add("Burn_inPassRate", "");

            }
            else
            {
                var burn_in_finisthRate = (Convert.ToDouble(burn_in_finished) / burn_inRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2");//完成率：完成数/订单的模组数
                ProductionDetailsJson.Add("Burn_inFinisthRate", burn_in_finisthRate);
                //合格率
                var burn_inPassRate = (Convert.ToDouble(burn_in_finished) / burn_in_Count * 100).ToString("F2");//合格率：完成数/记录数
                ProductionDetailsJson.Add("Burn_inPassRate", burn_inPassRate);
            }
            #endregion


            #region----------订单在校正的统计数据
            var calibrationRecord = db.CalibrationRecord.Where(c => c.OrderNum == orderMgm.OrderNum).ToList();//订单在校正的全部记录
            //开始时间 
            var calibration_begintime = calibrationRecord.Min(c => c.BeginCalibration);
            ProductionDetailsJson.Add("CalibrationBeginTime", calibration_begintime.ToString());//开始时间 

            //最后时间
            var calibration_endtime = calibrationRecord.Max(c => c.FinishCalibration);
            ProductionDetailsJson.Add("CalibrationEndTime", calibration_endtime.ToString());//最后时间

            //完成时间
            DateTime? calibration_finishtime = new DateTime();
            if (calibrationRecord.Count(c => c.Normal == true) == modelGroupQuantity)
            {
                calibration_finishtime = calibration_endtime;
            }
            else
            {
                calibration_finishtime = null;
            }
            ProductionDetailsJson.Add("CalibrationFinishTime", calibration_finishtime.ToString());//完成时间

            //作业时长
            var calibration_newtime = calibration_endtime - calibration_begintime;//目前时长
            ProductionDetailsJson.Add("CalibrationNewTime", calibration_newtime.ToString());//目前时长

            //直通个数
            var calibration_NotFirstPassYield = calibrationRecord.Where(c => c.Normal == false).Select(c => c.BarCodesNum).Distinct().ToList();
            var calibration_PassYield = calibrationRecord.Where(c => c.Normal == true).Select(c => c.BarCodesNum).ToList();
            var calibration_FirstPassYield = calibration_PassYield.Except(calibration_NotFirstPassYield).ToList();//直通条码记录
            var calibration_FirstPassYieldCount = calibration_PassYield.Count();//直通个数
            ProductionDetailsJson.Add("CalibrationFirstPassYieldCount", calibration_FirstPassYieldCount.ToString());//直通个数
            if(calibration_FirstPassYieldCount==0)
            {
                ProductionDetailsJson.Add("CalibrationFirstPassYield_Rate", "");//直通率
            }
            else
            {
                var calibration_FirstPassYield_Rate = (Convert.ToDouble(calibration_FirstPassYieldCount) / calibrationRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2");//直通率：直通数/模组数
                ProductionDetailsJson.Add("CalibrationFirstPassYield_Rate", calibration_FirstPassYield_Rate);//直通率
            }

            //正常个数
            int calibration_PassYieldCount = calibration_PassYield.Count();
            ProductionDetailsJson.Add("CalibrationPassYieldCount", calibration_PassYieldCount.ToString());

            //有效工时
            //异常个数
            //异常工时

            //完成率
            var calibration_finished = calibrationRecord.Count(m => m.Normal == true);//订单已完成PQC个数
            var calibration_finishedList = calibrationRecord.Where(m => m.Normal == true).Select(m => m.BarCodesNum).ToList(); //订单已完成PQC的条码清单
            var calibration_Count = calibrationRecord.Count();//订单PQC全部记录条数
            if(calibration_Count==0)
            {
                ProductionDetailsJson.Add("CalibrationFinisthRate", "");
                ProductionDetailsJson.Add("CalibrationPassRate", "");
            }
            else
            {
                var calibration_finisthRate = (Convert.ToDouble(calibration_finished) / calibrationRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2");//完成率：完成数/订单的模组数
                ProductionDetailsJson.Add("CalibrationFinisthRate", calibration_finisthRate);
                //合格率
                var calibration_PassRate = (Convert.ToDouble(calibration_finished) / calibration_Count * 100).ToString("F2");//合格率：完成数/记录数
                ProductionDetailsJson.Add("CalibrationPassRate", calibration_PassRate);
            }
            #endregion


            #region----------订单在外观包装的统计数据
            var appearanceRecord = db.Appearance.Where(c => c.OrderNum == orderMgm.OrderNum).ToList();//订单在包装的全部记录
            //开始时间 
            var appearance_begintime = appearanceRecord.Min(c => c.OQCCheckBT);
            ProductionDetailsJson.Add("AppearanceBeginTime", appearance_begintime.ToString());//开始时间 

            //最后时间
            var appearance_endtime = appearanceRecord.Max(c => c.OQCCheckFT);
            ProductionDetailsJson.Add("AppearanceEndTime", appearance_endtime.ToString());//最后时间

            //完成时间
            DateTime? appearance_finishtime = new DateTime();
            if (appearanceRecord.Count(c => c.OQCCheckFinish == true) == modelGroupQuantity)
            {
                appearance_finishtime = appearance_endtime;
            }
            else
            {
                appearance_finishtime = null;
            }
            ProductionDetailsJson.Add("AppearanceFinishTime", appearance_finishtime.ToString());//完成时间

            //作业时长
            var appearance_newtime = appearance_endtime - appearance_begintime;//目前时长
            ProductionDetailsJson.Add("AppearanceNewTime", appearance_newtime.ToString());//目前时长

            //直通个数
            var appearance_NotFirstPassYield = appearanceRecord.Where(c => c.OQCCheckFinish == false).Select(c => c.BarCodesNum).Distinct().ToList();
            var appearance_PassYield = appearanceRecord.Where(c => c.OQCCheckFinish == true).Select(c => c.BarCodesNum).ToList();
            var appearance_FirstPassYield = appearance_PassYield.Except(appearance_NotFirstPassYield).ToList();//直通条码记录
            var appearance_FirstPassYieldCount = appearance_PassYield.Count();//直通个数
            ProductionDetailsJson.Add("AppearanceFirstPassYieldCount", appearance_FirstPassYieldCount.ToString());//直通个数
            if(appearance_FirstPassYieldCount==0)
            {
                ProductionDetailsJson.Add("AppearanceFirstPassYield_Rate", "");//直通率
            }
            else
            {
                var appearance_FirstPassYield_Rate = (Convert.ToDouble(appearance_FirstPassYieldCount) / appearanceRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2");//直通率：直通数/模组数
                ProductionDetailsJson.Add("AppearanceFirstPassYield_Rate", appearance_FirstPassYield_Rate);//直通率
            }

            //正常个数
            int appearance_PassYieldCount = appearance_PassYield.Count();
            ProductionDetailsJson.Add("AppearancePassYieldCount", appearance_PassYieldCount.ToString());

            //有效工时
            //异常个数
            //异常工时

            //完成率
            var appearance_finished = appearanceRecord.Count(m => m.OQCCheckFinish == true);//订单已完成PQC个数
            var appearance_finishedList = appearanceRecord.Where(m => m.OQCCheckFinish == true).Select(m => m.BarCodesNum).ToList(); //订单已完成PQC的条码清单
            var appearance_Count = appearanceRecord.Count();//订单PQC全部记录条数
            if(appearance_Count==0)
            {
                ProductionDetailsJson.Add("AppearanceFinisthRate", "");
                ProductionDetailsJson.Add("AppearancePassRate", "");
            }
            else
            {
                var appearance_finisthRate = (Convert.ToDouble(appearance_finished) / appearanceRecord.Select(c => c.BarCodesNum).Distinct().Count() * 100).ToString("F2");//完成率：完成数/订单的模组数
                ProductionDetailsJson.Add("AppearanceFinisthRate", appearance_finisthRate);
                //合格率
                var appearance_PassRate = (Convert.ToDouble(appearance_finished) / appearance_Count * 100).ToString("F2");//合格率：完成数/记录数
                ProductionDetailsJson.Add("AppearancePassRate", appearance_PassRate);
            }
            #endregion
            ViewBag.ProductionDetailsJson = ProductionDetailsJson;
            #endregion

            //检查文件目录是否存在
            string directory = "D:\\MES_Data\\AOD_Files\\" + orderMgm.OrderNum;
            if (Directory.Exists(@directory) == true)
            {
                ViewBag.Directory = "Exists";
            }
            //检查pdf文件是否存在
            string pdfFile = "D:\\MES_Data\\AOD_Files\\" + orderMgm.OrderNum + "\\"+ orderMgm.OrderNum + "_AOD.pdf";
            if (System.IO.File.Exists(@pdfFile) == true)
            {
                ViewBag.PDf = "Exists";
            }
            List<FileInfo> filesInfo = new List<FileInfo>();
            JObject json = new JObject();
            if(ViewBag.Directory =="Exists")
            {
                filesInfo = GetAllFilesInDirectory(directory);
                filesInfo = filesInfo.Where(c => c.Name.StartsWith(orderMgm.OrderNum + "_AOD") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
                //检查jpg文件是否存在
                if (filesInfo.Count > 0)
                {            
                    ViewBag.Picture = "Exists";
                }
                int i = 1;
                foreach (var item in filesInfo)
                {
                    json.Add(i.ToString(), item.Name);
                    i++;
                }
            ViewBag.jpgjson = json;
            }
            return View(orderMgm);
        }
        #endregion
                    
        #region --------------------Create页
        // GET: OrderMgms/Create
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || ((Users)Session["User"]).Role == "系统管理员" || ((Users)Session["User"]).Role == "PC计划员")
            {
                return View();

            }
            return Content("<script>alert('对不起，您未授权管理订单，请联系PC部经理！');window.location.href='../OrderMgms/Index';</script>");
            //return RedirectToAction("Index");
        }

        // POST: OrderMgms/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OrderNum,BarCode_Prefix,CustomerName,ContractDate,DeliveryDate,PlanInputTime,PlanCompleteTime,PlatformType,Area,Boxes,Models,ModelsMore,Powers,PowersMore,AdapterCard,AdapterCardMore,BarCodeCreated,BarCodeCreateDate,BarCodeCreator,CompletedRate,IsRepertory,Remark")] OrderMgm orderMgm)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if(db.OrderMgm.Count(c=>c.OrderNum==orderMgm.OrderNum)>0)
            {
                ModelState.AddModelError("", "此订单号已存在，如订单信息有误，请进入订单详情修改信息！");
                return View(orderMgm);
            }
            //设置条码生成状态为0，表示未生成订单条码
            orderMgm.BarCodeCreated = 0;
                if (ModelState.IsValid)
                {
                    db.OrderMgm.Add(orderMgm);
                    db.SaveChanges();
                return Content("<script>alert('订单创建成功！');window.location.href='../OrderMgms/Index';</script>");
                //return RedirectToAction("Index");
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------Edit页
        // GET: OrderMgms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "经理" && ((Users)Session["User"]).Department == "PC部" || ((Users)Session["User"]).Role == "系统管理员" || ((Users)Session["User"]).Role == "PC计划员")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OrderMgm orderMgm = db.OrderMgm.Find(id);
                if (orderMgm == null)
                {
                    return HttpNotFound();
                }
                return View(orderMgm);
            }
            return RedirectToAction("Index", "OrderMgms");
        }

        // POST: OrderMgms/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OrderNum,BarCode_Prefix,CustomerName,ContractDate,DeliveryDate,PlanInputTime,PlanCompleteTime,PlatformType,Area,Boxes,Models,ModelsMore,Powers,PowersMore,AdapterCard,AdapterCardMore,BarCodeCreated,BarCodeCreateDate,BarCodeCreator,CompletedRate,IsRepertory,Remark")] OrderMgm orderMgm)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------Delete页
        // GET: OrderMgms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            return View(orderMgm);
        }

        // POST: OrderMgms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            db.OrderMgm.Remove(orderMgm);
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
        #endregion


        #region --------------------转为特采订单
        [HttpPost]
        public ActionResult AODConvert(int? id,string AOD_Description)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderMgm orderMgm = db.OrderMgm.Find(id);
            if (orderMgm == null)
            {
                return HttpNotFound();
            }
            orderMgm.IsAOD = true;
            orderMgm.AODConvertDate = DateTime.Now;
            orderMgm.AODConverter = ((Users)Session["User"]).UserName;
            orderMgm.AOD_Description = AOD_Description;
            if (ModelState.IsValid)
            {
                db.Entry(orderMgm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
            }
            return View(orderMgm);
        }
        #endregion

        #region --------------------上传文件(jpg、pdf)方法
        [HttpPost]
        public ActionResult UploadFile(int id, string ordernum)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadfile"];
                var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();
                string ReName = ordernum + "_AOD";
                if (Directory.Exists(@"D:\MES_Data\AOD_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\AOD_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\AOD_Files\" + ordernum + "\\");
                //List<FileInfo> test_temp = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AOD") && c.Name.Substring(c.Name.Length - 4, 4) ==".jpg").ToList();
                //文件为jpg类型
                if(fileType==".jpg")
                {
                    int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AOD") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                    file.SaveAs(@"D:\MES_Data\AOD_Files\" + ordernum + "\\" + ReName + (jpg_count + 1) + fileType);
                }
                //文件为pdf类型,直接存储或替换原文件
                else
                {
                    file.SaveAs(@"D:\MES_Data\AOD_Files\" + ordernum + "\\" + ReName + fileType);
                }
                OrderMgm orderMgm = db.OrderMgm.Find(id);
                if (orderMgm == null)
                {
                    return View(orderMgm);
                }
                if (ModelState.IsValid)
                {
                    db.Entry(orderMgm).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
                }
                return View(orderMgm);
            }
            return View();
        }
        #endregion

        #region --------------------获取特采订单pdf文件
        [HttpPost]
        public ActionResult GetPDF(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("<script>alert('此特采单pdf版文件尚未上传，无pdf文件可下载！');history.back(-1);</script>");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c=>c.Name == ordernum + "_AOD.pdf").Count()>0)
            {
                address = "/AOD_Files" + "/" + ordernum + "/" + ordernum + "_AOD.pdf";//filesInfo.Where(c => c.Name == ordernum + "_AOD.pdf").FirstOrDefault().Name;
            }
            else
            {
                return Content("<script>alert('此特采单pdf版文件尚未上传，无pdf文件可下载！');history.back(-1);</script>");
            }
            return Content(address);
        }
        #endregion

        #region --------------------获取特采订单图片预览
        [HttpPost]
        public ActionResult GetImg(string ordernum)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\AOD_Files\\" + ordernum + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum + "_AOD") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
            JObject json = new JObject();
            int i = 1;
            foreach (var item in filesInfo)
            {
                json.Add(i.ToString(), item.Name);
                i++;
            }
            ViewBag.jpgjson = json;
            //return Json(json, JsonRequestBehavior.AllowGet);
            return Content(json.ToString());
        }
        #endregion

        #region --------------------查看特采订单pdf文档页面
        public ActionResult preview_pdf(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("<script>alert('此特采单pdf版文件尚未上传，无pdf文件可下载！');history.back(-1);</script>");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_AOD.pdf").Count() > 0)
            {
                address = "~/Scripts/pdf.js/web/viewer.html?file=\\AOD_Files\\" + ordernum + "\\" + ordernum + "_AOD.pdf";
            }
            else
            {
                return Content("<script>alert('此特采单pdf版文件尚未上传，无pdf文件可下载！');history.back(-1);</script>");
            }
            ViewBag.address = address;
            ViewBag.ordernum = ordernum;
            return Redirect(address);
        }
        #endregion

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

