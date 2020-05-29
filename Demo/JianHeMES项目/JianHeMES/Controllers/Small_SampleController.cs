﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using JianHeMES.Controllers.PDFHelper;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rotativa.Core.Options;
using Rotativa.MVC;

namespace JianHeMES.Controllers
{
    public class Small_SampleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public CommonController com = new CommonController();

        #region------首页查询和显示报告页
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Index(string customer, string ordernum, string mo1, string mo2, string led_factory, DateTime? date_s, DateTime? date_e, string approve)
        {
            IEnumerable<Small_Sample> recordlist = db.Small_Sample;
            if (!String.IsNullOrEmpty(customer))
            {
                recordlist = recordlist.Where(c => c.Customer.Contains(customer)).ToList();
            }
            if (!String.IsNullOrEmpty(ordernum))
            {
                recordlist = recordlist.Where(c => c.OrderNumber.Contains(ordernum)).ToList();
            }
            if (!String.IsNullOrEmpty(mo1))
            {
                recordlist = recordlist.Where(c => c.Mo1.Contains(mo1)).ToList();
            }
            if (!String.IsNullOrEmpty(mo2))
            {
                recordlist = recordlist.Where(c => c.Mo2.Contains(mo2)).ToList();
            }
            if (!String.IsNullOrEmpty(led_factory))
            {
                recordlist = recordlist.Where(c => c.LED_R1.Contains(led_factory)).ToList();
            }
            if (date_s != null && date_e != null)
            {
                if (date_s == date_e)
                {
                    recordlist = recordlist.Where(c => c.ApprovedDate == date_s).ToList();
                }
                else
                {
                    recordlist = recordlist.Where(c => c.ApprovedDate >= date_s && c.ApprovedDate <= date_e).ToList();
                }
            }
            if (!String.IsNullOrEmpty(approve))
            {
                switch (approve)
                {
                    case "未审核":
                        recordlist = recordlist.Where(c => c.Assessor == null).ToList();
                        break;
                    case "已审核已通过":
                        recordlist = recordlist.Where(c => c.Assessor != null && c.AssessorResult == true && c.Approved == null).ToList();
                        break;
                    case "已审核未通过":
                        recordlist = recordlist.Where(c => c.Assessor != null && c.AssessorResult == false && c.Approved == null).ToList();
                        break;
                    case "未核准":
                        recordlist = recordlist.Where(c => c.Assessor != null && c.Approved == null).ToList();
                        break;
                    case "已核准已通过":
                        recordlist = recordlist.Where(c => c.Approved != null && c.ApprovedResult == true).ToList();
                        break;
                    case "已核准未通过":
                        recordlist = recordlist.Where(c => c.Approved != null && c.ApprovedResult == false).ToList();
                        break;
                }
            }
            var res = recordlist.Select(c => new { c.Id, c.Customer, c.OrderNumber, c.Mo1, c.Mo2, c.ReportDate,c.CreatedDate, c.Assessor, c.AssessorResult, c.AssessedDate,c.AssessorResultFailureReason, c.Approved, c.ApprovedResult, c.ApprovedResultFailureReason,c.ApprovedDate, c.Creator }).ToList();
            return Content(JsonConvert.SerializeObject(res));
        }

        public ActionResult Display()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Display(int id)
        {
            var result = db.Small_Sample.FirstOrDefault(c => c.Id == id);
            return Content(JsonConvert.SerializeObject(result));
        }

        public ActionResult DisplayAsPdf(int? id,bool downloadfile =false)
        {
            ViewBag.ApprovedResult = null;
            if(id!=null)
            {
                var result = db.Small_Sample.FirstOrDefault(c => c.Id == id);
                ViewBag.Points_R_G_B_W = result.Points_R_G_B_W == null ? "" : result.Points_R_G_B_W;
                ViewBag.ApprovedResult = result.ApprovedResult == null?"": (result.ApprovedResult == true ? "通过" : "不通过");
                ViewBag.AssessorResult = result.AssessorResult == null ? "" : (result.AssessorResult == true ? "通过" : "不通过");
                string filename = result.OrderNumber + "_Report";
                ViewBag.json = JsonConvert.SerializeObject(result);
                if(downloadfile)
                {
                    return new ViewAsPdf("DisplayAsPdf", result)
                    {
                        RotativaOptions = {
                           PageOrientation = Orientation.Portrait,
                           PageMargins = { Left = 0, Bottom = 0, Right = 0, Top = 10 },
                           PageSize = Rotativa.Core.Options.Size.A4,
                           },
                        FileName = result.OrderNumber + "_小样报告.pdf",
                    };
                }
                else
                {
                    return new ViewAsPdf("DisplayAsPdf", result)
                    {
                        RotativaOptions = {
                           PageOrientation = Orientation.Portrait,
                           PageMargins = { Left = 0, Bottom = 0, Right = 0, Top = 10 },
                           PageSize = Rotativa.Core.Options.Size.A4,
                           },
                    };
                }
            }
            else
            {
                return new ViewAsPdf("DisplayAsPdf");
            }
        }



        //[HttpPost]
        //public ActionResult DisplayAsPdf(int id)
        //{
        //    var result = db.Small_Sample.FirstOrDefault(c => c.Id == id);
        //    ViewBag.json = JsonConvert.SerializeObject(result);
        //    return new ViewAsPdf("test");
        //}


        [HttpPost]
        public ActionResult Comfirm(int id, string name, bool result, string reason)
        {
            var record = db.Small_Sample.FirstOrDefault(c => c.Id == id);
            record.Assessor = name;
            record.AssessedDate = DateTime.Now;
            record.AssessorResult = result;
            record.AssessorResultFailureReason = reason;
            int count = db.SaveChanges();
            if (count > 0) return Content("审核成功");
            else return Content("审核失败");
        }

        //public void SendEmail(string sender,List<string> recipient,string content,string subject,List<string>ccList,string address)

        [HttpPost]
        public ActionResult Approve(int id, string name, bool result, string reason)
        {
            var record = db.Small_Sample.FirstOrDefault(c => c.Id == id);
            var ordernumber = record.OrderNumber;
            record.Approved = name;
            record.ApprovedDate = DateTime.Now;
            record.ApprovedResult = result;
            record.ApprovedResultFailureReason = reason;
            int count = db.SaveChanges();
            if (count > 0)
            {
                if (result)
                {
                    string[] orderstirng = ordernumber.Split(',');
                    foreach (var item in orderstirng)
                    {
                        var orderitem = db.OrderMgm.Where(c => c.OrderNum == item).FirstOrDefault();
                        if (orderitem != null)
                        {
                            orderitem.HandSampleScedule = "完成";
                            db.SaveChanges();
                        }
                    }
                    Small_Sample_Email_Sended send_record = new Small_Sample_Email_Sended
                    {
                        OrderNumber = record.OrderNumber,
                        RecordID = record.Id,
                        Sended = false
                    };
                    db.Small_Sample_Email_Sended.Add(send_record);
                    db.SaveChanges();
                    return Content("核准通过。");
                }
                else
                {
                    return Content("核准不通过。");
                }
            }
            else return Content("核准失败，请重新核准。");
        }

        public ActionResult Display2(int id)
        {
            var result = db.Small_Sample.FirstOrDefault(c => c.Id == id);
            ViewBag.result = JsonConvert.SerializeObject(result);
            return View(result);
        }
        #endregion


        #region------上传excel文件和创建报告页
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Small_Sample", act = "Create" });
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(Small_Sample small_Sample)
        {
            small_Sample.CreatedDate = DateTime.Now;//创建时间
            db.Small_Sample.Add(small_Sample);
            int result = db.SaveChanges();
            if (result > 0) return Content("保存成功");
            else return Content("保存失败");
        }

        [HttpPost]
        public ActionResult Upload_small_Sample_report()
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
                sbtime.Append(DateTime.Now.Year).Append(DateTime.Now.Month).Append(DateTime.Now.Day).Append(DateTime.Now.Hour).Append(DateTime.Now.Minute).Append(DateTime.Now.Second);
                string dir = "/UploadFile/" + sbtime.ToString() + fileExt;
                string realfilepath = Request.MapPath(dir);
                string readDir = Path.GetDirectoryName(realfilepath);
                if (!Directory.Exists(readDir))
                    Directory.CreateDirectory(readDir);
                uploadfile.SaveAs(realfilepath);
                //提取数据 
                var dt = ExcelTool.ExcelToDataTable(true, realfilepath);
                Small_Sample small_Sample_report = new Small_Sample();

                var trans = dt.Rows[0][9].ToString().Split('-');//报告日期
                small_Sample_report.ReportDate = new DateTime(Convert.ToInt32(trans[2]), DataTypeChange.ChineseMonthChangeInt(trans[1]), Convert.ToInt32(trans[0]));

                small_Sample_report.Customer = dt.Rows[2][2].ToString();//客户名称
                small_Sample_report.OrderNumber = dt.Rows[2][5].ToString();//生产单号
                small_Sample_report.Mo1 = dt.Rows[2][8].ToString();//规格型号
                small_Sample_report.Module_Number = DataTypeChange.IsNumberic(dt.Rows[2][11].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[2][11]); //模块数量PCS

                small_Sample_report.Te1 = dt.Rows[5][0].ToString();//测试目的
                small_Sample_report.Actual_point = DataTypeChange.IsNumberic(dt.Rows[5][8].ToString()) == false ? 0 : Convert.ToDouble(dt.Rows[5][8]);//实际点间距
                small_Sample_report.Te2 = dt.Rows[6][0].ToString();//测试工具
                small_Sample_report.ReceivingCard = dt.Rows[6][9].ToString();//订单接收卡
                small_Sample_report.Mo2 = dt.Rows[7][2].ToString();//模块型号
                small_Sample_report.Points = DataTypeChange.IsNumberic(dt.Rows[7][5].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[7][5]);//点数
                small_Sample_report.Scanningway = DataTypeChange.IsNumberic(dt.Rows[7][7].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[7][7]);//扫描数
                small_Sample_report.system = dt.Rows[7][9].ToString();//调式系统
                small_Sample_report.Te3 = dt.Rows[8][2].ToString();//测试条件
                small_Sample_report.fixed_value = dt.Rows[8][3].ToString();//固定值（温度）
                small_Sample_report.Driveplate = DataTypeChange.IsNumberic(dt.Rows[8][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[8][6]);//驱动板电压
                small_Sample_report.Mo3 = dt.Rows[8][8].ToString();//电源型号

                small_Sample_report.Angle_R1 = DataTypeChange.IsNumberic(dt.Rows[11][3].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[11][3]);//角度.颜色.R.亮度
                small_Sample_report.Angle_R2 = DataTypeChange.IsNumberic(dt.Rows[12][3].ToString()) == false ? 0 : Convert.ToDouble(dt.Rows[12][3]);//角度.颜色.R.坐标.min
                small_Sample_report.Angle_R3 = DataTypeChange.IsNumberic(dt.Rows[12][4].ToString()) == false ? 0 : Convert.ToDouble(dt.Rows[12][4]);//角度.颜色.R.坐标.max
                small_Sample_report.Angle_G1 = DataTypeChange.IsNumberic(dt.Rows[11][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[11][5]);//角度.颜色.G.亮度
                small_Sample_report.Angle_G2 = DataTypeChange.IsNumberic(dt.Rows[12][5].ToString()) == false ? 0 : Convert.ToDouble(dt.Rows[12][5]);//角度.颜色.G.坐标.min
                small_Sample_report.Angle_G3 = DataTypeChange.IsNumberic(dt.Rows[12][6].ToString()) == false ? 0 : Convert.ToDouble(dt.Rows[12][6]);//角度.颜色.G.坐标.max
                small_Sample_report.Angle_B1 = DataTypeChange.IsNumberic(dt.Rows[11][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[11][7]);//角度.颜色.B.亮度
                small_Sample_report.Angle_B2 = DataTypeChange.IsNumberic(dt.Rows[12][7].ToString()) == false ? 0 : Convert.ToDouble(dt.Rows[12][7]);//角度.颜色.B.坐标.min
                small_Sample_report.Angle_B3 = DataTypeChange.IsNumberic(dt.Rows[12][8].ToString()) == false ? 0 : Convert.ToDouble(dt.Rows[12][8]);//角度.颜色.B.坐标.max
                small_Sample_report.Angle_W1 = DataTypeChange.IsNumberic(dt.Rows[11][9].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[11][9]);//角度.颜色.W.亮度
                small_Sample_report.Angle_W2 = DataTypeChange.IsNumberic(dt.Rows[12][9].ToString()) == false ? 0 : Convert.ToDouble(dt.Rows[12][9]);//角度.颜色.W.坐标.min
                small_Sample_report.Angle_W3 = DataTypeChange.IsNumberic(dt.Rows[12][11].ToString()) == false ? 0 : Convert.ToDouble(dt.Rows[12][11]);//角度.颜色.W.坐标.max
                small_Sample_report.Client_R1 = DataTypeChange.IsNumberic(dt.Rows[13][3].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[13][3]);//角度.客户要求.亮度.R
                small_Sample_report.Client_R2 = DataTypeChange.IsNumberic(dt.Rows[14][3].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[14][3]);//角度.客户要求.坐标.R.min
                small_Sample_report.Client_R3 = DataTypeChange.IsNumberic(dt.Rows[14][4].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[14][4]);//角度.客户要求.坐标.R.max
                small_Sample_report.Client_G1 = DataTypeChange.IsNumberic(dt.Rows[13][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[13][5]);//角度.客户要求.亮度.G        
                small_Sample_report.Client_G2 = DataTypeChange.IsNumberic(dt.Rows[14][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[14][5]);//角度.客户要求.坐标.G.min
                small_Sample_report.Client_G3 = DataTypeChange.IsNumberic(dt.Rows[14][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[14][6]);//角度.客户要求.坐标.G.max
                small_Sample_report.Client_B1 = DataTypeChange.IsNumberic(dt.Rows[13][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[13][7]);//角度.客户要求.亮度.B
                small_Sample_report.Client_B2 = DataTypeChange.IsNumberic(dt.Rows[14][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[14][7]);//角度.客户要求.坐标.B.min
                small_Sample_report.Client_B3 = DataTypeChange.IsNumberic(dt.Rows[14][8].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[14][8]);//角度.客户要求.坐标.B.max
                small_Sample_report.Client_W1 = dt.Rows[13][9].ToString();//角度.客户要求.亮度.W.校正前后
                small_Sample_report.Client_W1_V = DataTypeChange.IsNumberic(dt.Rows[13][10].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[13][10]); ;//角度.客户要求.亮度.W.校正前后值
                small_Sample_report.Client_W2 = DataTypeChange.IsNumberic(dt.Rows[13][11].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[13][11]);//角度.客户要求.亮度.W.max
                small_Sample_report.Client_W3 = DataTypeChange.IsNumberic(dt.Rows[14][9].ToString()) == false ? 0 : Convert.ToDouble(dt.Rows[14][9]);//角度.客户要求.坐标.W.min
                small_Sample_report.Client_W4 = DataTypeChange.IsNumberic(dt.Rows[14][11].ToString()) == false ? 0 : Convert.ToDouble(dt.Rows[14][11]);//角度.客户要求.坐标.W.max
                small_Sample_report.Points_R_G_B_W = dt.Rows[15][0].ToString(); //分压电 / 排阻阻值（欧）
                small_Sample_report.Points_R = DataTypeChange.IsNumberic(dt.Rows[15][3].ToString()) == false? -1 : Convert.ToInt32(dt.Rows[15][3]);//分压电/排阻阻值（欧）.R
                //small_Sample_report.Points_G = dt.Rows[15][5]==""?0:Convert.ToInt32(dt.Rows[15][5]);//分压电/排阻阻值（欧）.G
                //small_Sample_report.Points_B = dt.Rows[15][7]==""?0:Convert.ToInt32(dt.Rows[15][7]);//分压电/排阻阻值（欧）.B
                //small_Sample_report.Points_W = dt.Rows[15][9]==""?0:Convert.ToInt32(dt.Rows[15][9]);//分压电/排阻阻值（欧）.W
                small_Sample_report.Adju_R = DataTypeChange.IsNumberic(dt.Rows[16][3].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[16][3]);//可调电阻阻值（欧）.R
                small_Sample_report.Adju_G = DataTypeChange.IsNumberic(dt.Rows[16][5].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[16][5]);//可调电阻阻值（欧）.G
                small_Sample_report.Adju_B = DataTypeChange.IsNumberic(dt.Rows[16][7].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[16][7]);//可调电阻阻值（欧）.B
                //small_Sample_report.Adju_W = dt.Rows[16][9];//可调电阻阻值（欧）.W
                small_Sample_report.IC_R1 = DataTypeChange.IsNumberic(dt.Rows[17][3].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[17][3]);//单IC引脚电流（mA）.R
                small_Sample_report.IC_G1 = DataTypeChange.IsNumberic(dt.Rows[17][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[17][5]);//单IC引脚电流（mA）.G
                small_Sample_report.IC_B1 = DataTypeChange.IsNumberic(dt.Rows[17][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[17][7]);//单IC引脚电流（mA）.B
                small_Sample_report.IC_W1 = DataTypeChange.IsNumberic(dt.Rows[17][9].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[17][9]);//单IC引脚电流（mA）.W

                small_Sample_report.IC_R2 = DataTypeChange.IsNumberic(dt.Rows[18][3].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[18][3]);//IC.VDS.R
                small_Sample_report.IC_G2 = DataTypeChange.IsNumberic(dt.Rows[18][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[18][5]);//IC.VDS.G
                small_Sample_report.IC_B2 = DataTypeChange.IsNumberic(dt.Rows[18][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[18][7]);//IC.VDS.B
                small_Sample_report.IC_W2 = DataTypeChange.IsNumberic(dt.Rows[18][9].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[18][9]);//IC.VDS.W
                small_Sample_report.Module_current = DataTypeChange.IsNumberic(dt.Rows[19][3].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[19][3]);//模块电流
                small_Sample_report.IC3 = dt.Rows[19][9].ToString();//IC型号
                small_Sample_report.R1 = dt.Rows[20][3].ToString();//刷新频率
                small_Sample_report.R2 = dt.Rows[20][7].ToString();//接收卡Data包
                small_Sample_report.R3 = DataTypeChange.IsNumberic(dt.Rows[21][3].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[21][3]);//接收卡带载点数1
                small_Sample_report.R4 = DataTypeChange.IsNumberic(dt.Rows[21][4].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[21][4]);//接收卡带载点数2
                small_Sample_report.LCT = dt.Rows[21][7].ToString();//LCT版本
                small_Sample_report.De1_dec = dt.Rows[22][3].ToString(); //亮度有效率（小数）
                small_Sample_report.De2 = dt.Rows[22][7].ToString();//灰度等级
                small_Sample_report.DCLK = DataTypeChange.IsNumberic(dt.Rows[22][10].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[22][10]);//DCLK
                small_Sample_report.Load = dt.Rows[23][3].ToString();//电源带载率
                small_Sample_report.Per = dt.Rows[23][7].ToString();//每平方最大功耗
                small_Sample_report.GCLK = DataTypeChange.IsNumberic(dt.Rows[23][10].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[23][10]);//GCLK
                small_Sample_report.MOS1 = dt.Rows[24][3].ToString();//MOS管型号
                small_Sample_report.MOS2 = DataTypeChange.IsNumberic(dt.Rows[24][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[24][7]);//MOS压降(V)
                small_Sample_report.Current = dt.Rows[24][10].ToString();//电流增益

                small_Sample_report.LED_R1 = dt.Rows[27][1].ToString();//LED厂家及型号.R
                small_Sample_report.LED_R2 = DataTypeChange.IsNumberic(dt.Rows[27][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[27][5]);//LED厂家及型号.R.实际范围.IV.Min
                small_Sample_report.LED_R3 = DataTypeChange.IsNumberic(dt.Rows[28][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[28][5]);//LED厂家及型号.R.Min:Max.IV.Min
                small_Sample_report.LED_R4 = DataTypeChange.IsNumberic(dt.Rows[27][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[27][6]);//LED厂家及型号.R.实际范围.IV.Max
                small_Sample_report.LED_R5 = DataTypeChange.IsNumberic(dt.Rows[28][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[28][6]);//LED厂家及型号.R.Min:Max.IV.Max
                small_Sample_report.LED_R6 = DataTypeChange.IsNumberic(dt.Rows[27][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[27][7]);//LED厂家及型号.R.实际范围.WD.Min
                small_Sample_report.LED_R7 = DataTypeChange.IsNumberic(dt.Rows[27][8].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[27][8]);//LED厂家及型号.R.实际范围.WD.Max
                small_Sample_report.LED_R8 = DataTypeChange.IsNumberic(dt.Rows[28][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[28][7]);//LED厂家及型号.R.Min:Max.WD
                small_Sample_report.LED_R9 = DataTypeChange.IsNumberic(dt.Rows[27][9].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[27][9]);//LED厂家及型号.R.正向电压.Min
                small_Sample_report.LED_R10 = DataTypeChange.IsNumberic(dt.Rows[27][10].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[27][10]);//LED厂家及型号.R.正向电压.Max
                small_Sample_report.LED_R11 = DataTypeChange.IsNumberic(dt.Rows[27][11].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[27][11]);//LED厂家及型号.R.IV@IF

                small_Sample_report.LED_G1 = dt.Rows[29][1].ToString();//LED厂家.G
                small_Sample_report.LED_G2 = DataTypeChange.IsNumberic(dt.Rows[29][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[29][5]);//LED厂家.G.实际范围.IV.Min
                small_Sample_report.LED_G3 = DataTypeChange.IsNumberic(dt.Rows[30][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[30][5]);//LED厂家.G.Min:Max.IV.Min
                small_Sample_report.LED_G4 = DataTypeChange.IsNumberic(dt.Rows[29][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[29][6]);//LED厂家.G.实际范围.IV.Max
                small_Sample_report.LED_G5 = DataTypeChange.IsNumberic(dt.Rows[30][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[30][6]);//LED厂家.G.Min:Max.IV.Max
                small_Sample_report.LED_G6 = DataTypeChange.IsNumberic(dt.Rows[29][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[29][7]);//LED厂家.G.实际范围.WD.Min
                small_Sample_report.LED_G7 = DataTypeChange.IsNumberic(dt.Rows[29][8].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[29][8]);//LED厂家.G.实际范围.WD.Max
                small_Sample_report.LED_G8 = DataTypeChange.IsNumberic(dt.Rows[30][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[30][7]);//LED厂家.G.Min:Max.WD
                small_Sample_report.LED_G9 = DataTypeChange.IsNumberic(dt.Rows[29][9].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[29][9]);//LED厂家.G.正向电压.Min
                small_Sample_report.LED_G10 = DataTypeChange.IsNumberic(dt.Rows[29][10].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[29][10]);//LED厂家.G.正向电压.Max
                small_Sample_report.LED_G11 = DataTypeChange.IsNumberic(dt.Rows[29][11].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[29][11]);//LED厂家.G.IV@IF

                small_Sample_report.LED_B1 = dt.Rows[31][1].ToString();//厂家及型号.B
                small_Sample_report.LED_B2 = DataTypeChange.IsNumberic(dt.Rows[31][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[31][5]);//厂家及型号.B.实际范围.IV.Min
                small_Sample_report.LED_B3 = DataTypeChange.IsNumberic(dt.Rows[32][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[32][5]);//厂家及型号.B.Min:Max.IV.Min
                small_Sample_report.LED_B4 = DataTypeChange.IsNumberic(dt.Rows[31][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[31][6]);//厂家及型号.B.实际范围.IV.Max
                small_Sample_report.LED_B5 = DataTypeChange.IsNumberic(dt.Rows[32][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[32][6]);//厂家及型号.B.Min:Max.IV.Max
                small_Sample_report.LED_B6 = DataTypeChange.IsNumberic(dt.Rows[31][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[31][7]);//厂家及型号.B.实际范围.WD.Min
                small_Sample_report.LED_B7 = DataTypeChange.IsNumberic(dt.Rows[31][8].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[31][8]);//厂家及型号.B.实际范围.WD.Max
                small_Sample_report.LED_B8 = DataTypeChange.IsNumberic(dt.Rows[32][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[32][7]);//厂家及型号.B.Min:Max.WD
                small_Sample_report.LED_B9 = DataTypeChange.IsNumberic(dt.Rows[31][9].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[31][9]);//厂家及型号.B.正向电压.Min
                small_Sample_report.LED_B10 = DataTypeChange.IsNumberic(dt.Rows[31][10].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[31][10]);//厂家及型号.B.正向电压.Max
                small_Sample_report.LED_B11 = DataTypeChange.IsNumberic(dt.Rows[31][11].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[31][11]);//厂家及型号.B.IV@IF

                small_Sample_report.Small_R1 = dt.Rows[35][1].ToString();//LED厂家及型号.R
                small_Sample_report.Small_R2 = DataTypeChange.IsNumberic(dt.Rows[35][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[35][5]);//LED厂家及型号.R.实际范围.IV.Min
                small_Sample_report.Small_R3 = DataTypeChange.IsNumberic(dt.Rows[36][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[36][5]);//LED厂家及型号.R.Min:Max.IV.Min
                small_Sample_report.Small_R4 = DataTypeChange.IsNumberic(dt.Rows[35][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[35][6]);//LED厂家及型号.R.实际范围.IV.Max
                small_Sample_report.Small_R5 = DataTypeChange.IsNumberic(dt.Rows[36][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[36][6]);//LED厂家及型号.R.Min:Max.IV.Max
                small_Sample_report.Small_R6 = DataTypeChange.IsNumberic(dt.Rows[35][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[35][7]);//LED厂家及型号.R.实际范围.WD.Min
                small_Sample_report.Small_R7 = DataTypeChange.IsNumberic(dt.Rows[35][8].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[35][8]);//LED厂家及型号.R.实际范围.WD.Max
                small_Sample_report.Small_R8 = DataTypeChange.IsNumberic(dt.Rows[36][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[36][7]);//LED厂家及型号.R.Min:Max.WD
                small_Sample_report.Small_R9 = DataTypeChange.IsNumberic(dt.Rows[35][9].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[35][9]);//LED厂家及型号.R.正向电压.Min
                small_Sample_report.Small_R10 = DataTypeChange.IsNumberic(dt.Rows[35][10].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[35][10]);//LED厂家及型号.R.正向电压.Max
                small_Sample_report.Small_R11 = DataTypeChange.IsNumberic(dt.Rows[35][11].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[35][11]);//LED厂家及型号.R.IV@IF

                small_Sample_report.Small_G1 = dt.Rows[37][1].ToString();//LED厂家.G
                small_Sample_report.Small_G2 = DataTypeChange.IsNumberic(dt.Rows[37][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[37][5]);//LED厂家.G.实际范围.IV.Min
                small_Sample_report.Small_G3 = DataTypeChange.IsNumberic(dt.Rows[38][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[38][5]);//LED厂家.G.Min:Max.IV.Min
                small_Sample_report.Small_G4 = DataTypeChange.IsNumberic(dt.Rows[37][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[37][6]);//LED厂家.G.实际范围.IV.Max
                small_Sample_report.Small_G5 = DataTypeChange.IsNumberic(dt.Rows[38][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[38][6]);//LED厂家.G.Min:Max.IV.Max
                small_Sample_report.Small_G6 = DataTypeChange.IsNumberic(dt.Rows[37][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[37][7]);//LED厂家.G.实际范围.WD.Min
                small_Sample_report.Small_G7 = DataTypeChange.IsNumberic(dt.Rows[37][8].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[37][8]);//LED厂家.G.实际范围.WD.Max
                small_Sample_report.Small_G8 = DataTypeChange.IsNumberic(dt.Rows[38][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[38][7]);//LED厂家.G.Min:Max.WD
                small_Sample_report.Small_G9 = DataTypeChange.IsNumberic(dt.Rows[37][9].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[37][9]);//LED厂家.G.正向电压.Min
                small_Sample_report.Small_G10 = DataTypeChange.IsNumberic(dt.Rows[37][10].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[37][10]);//LED厂家.G.正向电压.Max
                small_Sample_report.Small_G11 = DataTypeChange.IsNumberic(dt.Rows[37][11].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[37][11]);//LED厂家.G.IV@IF

                small_Sample_report.Small_B1 = dt.Rows[39][1].ToString();//厂家及型号.B
                small_Sample_report.Small_B2 = DataTypeChange.IsNumberic(dt.Rows[39][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[39][5]);//厂家及型号.B.实际范围.IV.Min
                small_Sample_report.Small_B3 = DataTypeChange.IsNumberic(dt.Rows[40][5].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[40][5]);//厂家及型号.B.Min:Max.IV.Min
                small_Sample_report.Small_B4 = DataTypeChange.IsNumberic(dt.Rows[39][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[39][6]);//厂家及型号.B.实际范围.IV.Max
                small_Sample_report.Small_B5 = DataTypeChange.IsNumberic(dt.Rows[40][6].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[40][6]);//厂家及型号.B.Min:Max.IV.Max
                small_Sample_report.Small_B6 = DataTypeChange.IsNumberic(dt.Rows[39][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[39][7]);//厂家及型号.B.实际范围.WD.Min
                small_Sample_report.Small_B7 = DataTypeChange.IsNumberic(dt.Rows[39][8].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[39][8]);//厂家及型号.B.实际范围.WD.Max
                small_Sample_report.Small_B8 = DataTypeChange.IsNumberic(dt.Rows[40][7].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[40][7]);//厂家及型号.B.Min:Max.WD
                small_Sample_report.Small_B9 = DataTypeChange.IsNumberic(dt.Rows[39][9].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[39][9]);//厂家及型号.B.正向电压.Min
                small_Sample_report.Small_B10 = DataTypeChange.IsNumberic(dt.Rows[39][10].ToString()) == false ? 0 : Convert.ToDecimal(dt.Rows[39][10]);//厂家及型号.B.正向电压.Max
                small_Sample_report.Small_B11 = DataTypeChange.IsNumberic(dt.Rows[39][11].ToString()) == false ? 0 : Convert.ToInt32(dt.Rows[39][11]);//厂家及型号.B.IV@IF

                small_Sample_report.ReportRemark = dt.Rows[41][0].ToString();//备注
                small_Sample_report.Approved = dt.Rows[42][3].ToString();//核准人
                small_Sample_report.Assessor = dt.Rows[42][6].ToString();//审核人
                small_Sample_report.Tester = dt.Rows[42][9].ToString();//测试人
                small_Sample_report.ReportVersionNumber = dt.Rows[43][0].ToString();//表单版本编号

                var ic_model = db.Small_Sample_IC_Model_Arguments.Where(c => c.IC_Model == small_Sample_report.IC3).FirstOrDefault();
                if (ic_model == null)
                {
                    small_Sample_report.V_REXT = 0;
                    small_Sample_report.Gain = 0;
                    small_Sample_report.N_Value = 0;
                }
                else
                {
                    small_Sample_report.V_REXT = ic_model.V_REXT;
                    small_Sample_report.Gain = ic_model.Gain;
                    small_Sample_report.N_Value = ic_model.N_Value;
                }

                //return Content(JsonConvert.SerializeObject(small_Sample_report));
                return Json(small_Sample_report, JsonRequestBehavior.AllowGet);
            }
            //catch (Exception ex)
            //{
            //    return Content(ex.Message);
            //}
            catch
            {
                return Content("您输入的表格跟小样模板表格不一致，请使用标准模板表格。");
            }

        }

        #endregion


        #region------小样参数页功能

        //参数首页
        public ActionResult Small_Sample_Arguments()
        {
            //if (Session["User"] == null)
            //{
            //    return RedirectToAction("Login", "Users", new { col = "Small_Sample", act = "Small_Sample_Arguments" });
            //}
            return View();
        }

        //取出参数
        [HttpPost]
        public ActionResult Small_Sample_Arguments_Getdata()
        {
            JObject result = new JObject();
            var Small_Sample_IC_Model_Arguments_list = db.Small_Sample_IC_Model_Arguments.ToList();
            var Small_Sample_ICmodel_Spacing_list = db.Small_Sample_ICmodel_Spacing.ToList();
            var Small_Sample_Spacing_Value_list = db.Small_Sample_Spacing_Value.ToList();
            var Small_Sample_EMail_SendTO_list = db.UserItemEmail.Where(c => c.ProcesName == "小样发送").ToList();
            var Small_Sample_EMail_SendCC_list = db.UserItemEmail.Where(c => c.ProcesName == "小样抄送").ToList();
            result.Add("Small_Sample_IC_Model_Arguments_list", JsonConvert.SerializeObject(Small_Sample_IC_Model_Arguments_list));
            result.Add("Small_Sample_ICmodel_Spacing_list", JsonConvert.SerializeObject(Small_Sample_ICmodel_Spacing_list));
            result.Add("Small_Sample_Spacing_Value_list", JsonConvert.SerializeObject(Small_Sample_Spacing_Value_list));
            result.Add("Small_Sample_EMail_SendTO_list", JsonConvert.SerializeObject(Small_Sample_EMail_SendTO_list));
            result.Add("Small_Sample_EMail_SendCC_list", JsonConvert.SerializeObject(Small_Sample_EMail_SendCC_list));
            return Content(JsonConvert.SerializeObject(result));
        }

        #region------IC型号计算参数
        //IC型号计算参数添加
        [HttpPost]
        public ActionResult Small_Sample_IC_Model_Arguments_add(Small_Sample_IC_Model_Arguments record)
        {
            var isexist = db.Small_Sample_IC_Model_Arguments.Count(c => c.IC_Model == record.IC_Model);
            if (isexist > 0) return Content(record.IC_Model + "已经存在。");
            else
            {
                db.Small_Sample_IC_Model_Arguments.Add(record);
                int count = db.SaveChanges();
                if (count > 0) return Content("保存成功");
                else return Content("保存失败");
            }
        }

        //IC型号计算参数修改
        [HttpPost]
        public ActionResult Small_Sample_IC_Model_Arguments_modify(Small_Sample_IC_Model_Arguments record_modify)
        {
            // 通过上下文获取对象相关信息
            DbEntityEntry<Small_Sample_IC_Model_Arguments> entry = db.Entry(record_modify);
            // 对象的状态是修改的
            entry.State = System.Data.Entity.EntityState.Modified;
            // 对象的属性值Gain修改(这一组状态修改可以不要)
            //entry.Property("Gain").IsModified = true;
            int count = db.SaveChanges();
            if (count > 0) return Content("保存成功");
            else return Content("保存失败");
        }

        //IC型号计算参数删除
        [HttpPost]
        public ActionResult Small_Sample_IC_Model_Arguments_delete(int id)
        {
            var re = db.Small_Sample_IC_Model_Arguments.Where(c => c.Id == id).FirstOrDefault();
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;
            operaterecord.Operator = ((Users)Session["User"]).UserName;
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除小样IC型号" + re.IC_Model + "的参数：V＝" + re.V_REXT + ",N=" + re.N_Value + ",G=" + re.Gain;
            db.Small_Sample_IC_Model_Arguments.Remove(re);
            db.UserOperateLog.Add(operaterecord);
            int count = db.SaveChanges();
            if (count > 0) return Content("删除成功");
            else return Content("删除失败");
        }
        #endregion

        #region------型号-间距对应
        //型号-间距对应添加
        [HttpPost]
        public ActionResult Small_Sample_ICmodel_Spacing_add(Small_Sample_ICmodel_Spacing record_add)
        {
            var isexist = db.Small_Sample_ICmodel_Spacing.Count(c => c.Mo2 == record_add.Mo2);
            if (isexist > 0) return Content(record_add.Mo2 + "已经存在。");
            else
            {
                db.Small_Sample_ICmodel_Spacing.Add(record_add);
                int count = db.SaveChanges();
                if (count > 0) return Content("保存成功");
                else return Content("保存失败");
            }
        }

        //型号-间距对应修改
        [HttpPost]
        public ActionResult Small_Sample_ICmodel_Spacing_modify(Small_Sample_ICmodel_Spacing record)
        {
            DbEntityEntry<Small_Sample_ICmodel_Spacing> entry = db.Entry(record);
            entry.State = System.Data.Entity.EntityState.Modified;
            int count = db.SaveChanges();
            if (count > 0) return Content("保存成功");
            else return Content("保存失败");
        }

        //型号-间距对应删除
        [HttpPost]
        public ActionResult Small_Sample_ICmodel_Spacing_delete(int id)
        {
            var re = db.Small_Sample_ICmodel_Spacing.Where(c => c.Id == id).FirstOrDefault();
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;
            operaterecord.Operator = ((Users)Session["User"]).UserName;
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除小样模块型号" + re.Mo2 + "的参数：间距＝" + re.Spacing;
            db.Small_Sample_ICmodel_Spacing.Remove(re);
            db.UserOperateLog.Add(operaterecord);
            int count = db.SaveChanges();
            if (count > 0) return Content("删除成功");
            else return Content("删除失败");
        }
        #endregion

        #region------间距对应参数范围
        //间距对应参数范围添加
        [HttpPost]
        public ActionResult Small_Sample_Spacing_Value_add(Small_Sample_Spacing_Value record)
        {
            var isexist = db.Small_Sample_Spacing_Value.Count(c => c.Spacing == record.Spacing);
            if (isexist > 0) return Content(record.Spacing + "已经存在。");
            else
            {
                db.Small_Sample_Spacing_Value.Add(record);
                int count = db.SaveChanges();
                if (count > 0) return Content("保存成功");
                else return Content("保存失败");
            }
        }

        //间距对应参数范围修改
        [HttpPost]
        public ActionResult Small_Sample_Spacing_Value_modify(Small_Sample_Spacing_Value record)
        {
            DbEntityEntry<Small_Sample_Spacing_Value> entry = db.Entry(record);
            entry.State = System.Data.Entity.EntityState.Modified;
            int count = db.SaveChanges();
            if (count > 0) return Content("保存成功");
            else return Content("保存失败");
        }

        //间距对应参数范围删除
        [HttpPost]
        public ActionResult Small_Sample_Spacing_Value_delete(int id)
        {
            var re = db.Small_Sample_Spacing_Value.Where(c => c.Id == id).FirstOrDefault();
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;
            operaterecord.Operator = ((Users)Session["User"]).UserName;
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除小样间距参数为" + re.Spacing + "的参数：Maximum＝" + re.Maximum + ",Least" + re.Least;
            db.Small_Sample_Spacing_Value.Remove(re);
            db.UserOperateLog.Add(operaterecord);
            int count = db.SaveChanges();
            if (count > 0) return Content("删除成功");
            else return Content("删除失败");
        }
        #endregion

        #region------邮件发送人
        //邮件发送人添加
        [HttpPost]
        public ActionResult Small_Sample_Report_SendTo_add(UserItemEmail record)
        {
            var isexist = db.UserItemEmail.Count(c => c.EmailAddress == record.EmailAddress && c.ProcesName == record.ProcesName);
            if (isexist > 0) return Content(record.EmailAddress + "已经存在。");
            else
            {
                db.UserItemEmail.Add(record);
                int count = db.SaveChanges();
                if (count > 0) return Content("保存成功");
                else return Content("保存失败");
            }
        }

        //邮件发送人修改
        [HttpPost]
        public ActionResult Small_Sample_Report_SendTo_modify(UserItemEmail record)
        {
            DbEntityEntry<UserItemEmail> entry = db.Entry(record);
            entry.State = System.Data.Entity.EntityState.Modified;
            int count = db.SaveChanges();
            if (count > 0) return Content("保存成功");
            else return Content("保存失败");
        }

        //邮件发送人删除
        [HttpPost]
        public ActionResult Small_Sample_Report_SendTo_delete(int id)
        {
            var record = db.UserItemEmail.Where(c => c.id == id).FirstOrDefault();
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;
            operaterecord.Operator = ((Users)Session["User"]).UserName;
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除小样报告邮件发送人为" + record.UserName + "的记录。";
            db.UserItemEmail.Remove(record);
            db.UserOperateLog.Add(operaterecord);
            int count = db.SaveChanges();
            if (count > 0) return Content("删除成功");
            else return Content("删除失败");
        }
        #endregion

        #region------邮件抄送人
        //邮件抄送人添加
        [HttpPost]
        public ActionResult Small_Sample_Report_SendCC_add(UserItemEmail record)
        {
            var isexist = db.UserItemEmail.Count(c => c.EmailAddress == record.EmailAddress && c.ProcesName == record.ProcesName);
            if (isexist > 0) return Content(record.EmailAddress + "已经存在。");
            else
            {
                db.UserItemEmail.Add(record);
                int count = db.SaveChanges();
                if (count > 0) return Content("保存成功");
                else return Content("保存失败");
            }
        }

        //邮件抄送人修改
        [HttpPost]
        public ActionResult Small_Sample_Report_SendCC_modify(UserItemEmail record)
        {
            DbEntityEntry<UserItemEmail> entry = db.Entry(record);
            entry.State = System.Data.Entity.EntityState.Modified;
            int count = db.SaveChanges();
            if (count > 0) return Content("保存成功");
            else return Content("保存失败");
        }

        //邮件抄送人删除
        [HttpPost]
        public ActionResult Small_Sample_Report_SendCC_delete(int id)
        {
            var record = db.UserItemEmail.Where(c => c.id == id).FirstOrDefault();
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;
            operaterecord.Operator = ((Users)Session["User"]).UserName;
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除小样报告邮件抄送人为" + record.UserName + "的记录。";
            db.UserItemEmail.Remove(record);
            db.UserOperateLog.Add(operaterecord);
            int count = db.SaveChanges();
            if (count > 0) return Content("删除成功");
            else return Content("删除失败");
        }
        #endregion
        #endregion


        #region------IC电流算法
        [HttpPost]
        public ActionResult Current(List<int> resistances, string ic_type = "", string de1_dec = "")
        {
            var type = db.Small_Sample_IC_Model_Arguments.Where(c => c.IC_Model == ic_type).FirstOrDefault();
            if (type == null) return Content("无");
            List<Double> ICpin = new List<Double>();
            foreach (var resistance in resistances)
            {
                ICpin.Add((type.V_REXT / resistance) * type.N_Value * type.Gain * 1000 * (Convert.ToDouble(de1_dec)));
            }
            return Content(JsonConvert.SerializeObject(ICpin));
        }
        #endregion


        #region------根据型号读取运算值
        public ActionResult IC_ModelRange(decimal IC_R1, decimal IC_G1, decimal IC_B1, decimal Points, decimal Scanningway, string Mo2)
        {
            JObject result = new JObject();
            var spacing_record = db.Small_Sample_ICmodel_Spacing.Where(c => c.Mo2 == Mo2).FirstOrDefault();
            if (spacing_record == null) return Content("找不到模块型号!");
            var Mo1_Spacing_Value_record = db.Small_Sample_Spacing_Value.Where(c => c.Spacing == spacing_record.Spacing).FirstOrDefault();
            //IC_R1:单IC引脚电流（mA）.R
            //IC_G1:单IC引脚电流（mA）.G
            //IC_B1:单IC引脚电流（mA）.B
            //Points:点数
            //record.Scanningway;//扫描数
            //在PH1.1 - PH3.1间距时、最低高于V + 0.3A、最高不能高于V + 1.5A。  
            //在PH3.2 - PH10 间距时、最低高于V + 0.3A、最高不能高于V + 1.1A。  
            //在PH12  - PH30 间距时、最低高于V + 0.1A、最高不能高于V + 0.8A
            Double base_v = 0;
            if (Scanningway == 0) base_v = Convert.ToDouble((IC_R1 + IC_G1 + IC_B1) * Points / 1000);
            else base_v = Convert.ToDouble((IC_R1 + IC_G1 + IC_B1) * Points / Scanningway / 1000);
            Double maximum = base_v + Convert.ToDouble(Mo1_Spacing_Value_record.Maximum);
            Double least = base_v + Convert.ToDouble(Mo1_Spacing_Value_record.Least);
            result.Add("最低值", least);
            result.Add("最高值", maximum);
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion


        #region------生成PDF功能
        /// <summary>
        /// 获取MVC视图Html
        /// </summary>
        /// <param name="context">控制器上下文</param>
        /// <param name="viewName">视图名称</param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public string GetViewHtml(ControllerContext context, string viewName)
        {

            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                try
                {
                    viewResult.View.Render(viewContext, sw);
                }
                catch (Exception ex)
                {
                    throw;
                }

                return sw.GetStringBuilder().ToString();
            }
        }
        public ActionResult GetPdf(string path)
        {
            path = "http:////172.16.6.145//Small_Sample//Display?id=69";
            string webcontent = HtmlToPdfHelper.GetWebContent2(path);
            byte[] contentbyte = HtmlToPdfHelper.ConvertHtmlTextToPdf(webcontent);
            HtmlToPdfHelper.PdfDownload(contentbyte, "小样报告");
            return new FilePathResult("~/content/The Garbage Collection Handbook.pdf", "application/pdf");
        }

        //HTML转PDF
        [HttpPost]
        public ActionResult ConvertToPDF(string html)
        {
            byte[] contentbyte = HtmlToPdfHelper.ConvertHtmlTextToPdf(html);
            HtmlToPdfHelper.PdfDownload(contentbyte, "小样报告");
            return new FilePathResult("~/UploadFile/小样报告.pdf", "application/pdf");
        }

        [HttpPost]
        public ActionResult ReoportToPDF(int reportid)
        {
            //string path = Server.MapPath("pdf");
            var record = db.Small_Sample.Find(reportid);
            MemoryStream memory = new MemoryStream();
            MemoryStream outputStream = new MemoryStream();
            Document document = new Document(PageSize.A4, 50, 50, 80, 50);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("D:\\MES_Data\\TemDate\\" + record.OrderNumber + "小样报告.pdf", FileMode.Create));//文档不存在时创建，存在时覆盖
            //PdfWriter writer = PdfWriter.GetInstance(document,memory);
            //设置字体
            BaseFont bfChinese = BaseFont.CreateFont("c://windows//fonts//simkai.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            //Font chFont = new Font(bfChinese, 12);
            //Font chFont_blue = new Font(bfChinese, 40, Font.NORMAL, new BaseColor(51, 0, 153));
            //Font chFont_msg = new Font(bfChinese, 14, Font.ITALIC, BaseColor.BLACK);

            document.Open();
            //添加一个表格
            PdfPTable pdfPTable = new PdfPTable(new float[] { 1, 1, 1 });
            pdfPTable.TotalWidth = document.Right - document.Left;
            float[] widths = { 100f, 100f, 100f };
            pdfPTable.SetWidths(widths);
            pdfPTable.LockedWidth = true;//必须锁定宽度，否则修改无效

            ////增加表头
            //PdfPCell student_name = new PdfPCell(new Phrase("姓名", chFont_msg));
            //PdfPCell course_name = new PdfPCell(new Phrase("课程", chFont_msg));
            //PdfPCell student_score = new PdfPCell(new Phrase("分数", chFont_msg));
            //pdfPTable.AddCell(student_name);
            //pdfPTable.AddCell(course_name);
            //pdfPTable.AddCell(student_score);
            ////增加详细数据
            //pdfPTable.AddCell(new PdfPCell(new Phrase("张三", chFont)));
            //pdfPTable.AddCell(new PdfPCell(new Phrase("语文", chFont)));
            //pdfPTable.AddCell(new PdfPCell(new Phrase("99", chFont)));

            //将一个元素添加到文档中
            document.Add(pdfPTable);
            document.Close();
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attchment:filename=small_sample_report.pdf");
            Response.ContentType = "applictaion/octet-stream";
            Response.OutputStream.Write(memory.GetBuffer(), 0, memory.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
            Response.Flush();
            Response.End();
            //return new FilePathResult("D:\\MES_Data\\TemDate\\" + record.OrderNumber+"小样报告.pdf", "application/pdf");
            string path = "~/MES_Data/TemDate/" + record.OrderNumber + "小样报告.pdf";
            var str_byte = FileContent(record.OrderNumber);
            //HtmlToPdfHelper.PdfDownload(str_byte, record.OrderNumber + "小样报告.pdf");
            //return File(path, "application/pdf");
            return Content(path);
        }

        //读取小样pdf文件为byte[]
        private byte[] FileContent(string ordernum)
        {
            using (FileStream fs = new FileStream("D:\\MES_Data\\TemDate\\" + ordernum + "小样报告.pdf", FileMode.Open, FileAccess.Read))
            {
                try
                {
                    byte[] buffur = new byte[fs.Length];
                    fs.Read(buffur, 0, (int)fs.Length);
                    return buffur;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        ////[NoAuthorization]
        //public ActionResult IndexPdf(string Year, string month)
        //{
        //    //获取值班内容
        //    string monthContent = helper.GetRotaMonth(Year, month);
        //    //值班标题
        //    string title = "";
        //    //从网址下载Html字符串(方法一)
        //    //string inpath = System.Web.HttpContext.Current.Server.MapPath("~/Upload/Rota/September.html");
        //    //string htmlText = HtmlToPdfHelper.GetWebContent(inpath)//如果读取文件中的HTML代码调用此方法
        //    string htmlText = HtmlToPdfHelper.GetWebContent(monthContent);//这是调用的我获取的数据库中的内容
        //    //获取MVC视图Html字符串(方法二)
        //    //string htmlText = GetViewHtml(ControllerContext, "EditIndex");            //如果直接调用视图中的HTML调调用此方法，不建议用此方法，因为视图中有数据会是动态生成的，这时会获取不到报错
        //    //水印图片路径
        //    //string picPath = Server.MapPath("~/PDFTemplate/TemplateImg/authentication-iocn.png");
        //    //string picPath = Server.MapPath("~/Upload/Rota/authentication-iocn.png");
        //    //html转pdf并加上水印
        //    //byte[] pdfFile = HtmlToPdfHelper.ConvertHtmlTextToPdf(htmlText, "", 100, 200, 100, 100);
        //    byte[] pdfFile = HtmlToPdfHelper.ConvertHtmlTextToPdf(htmlText);
        //    //输出至客户端
        //    HtmlToPdfHelper.PdfDownload(pdfFile, title);//此处调用步骤6方法
        //    return View();
        //}

        #endregion

        #region------删除小样报告
        [HttpPost]
        public ActionResult DeleteSmallSampleReport(int id)
        {
            var re = db.Small_Sample.Find(id);
            if (re == null) return Content("找不到小样报告记录！");
            string report_ordernum = re.OrderNumber;
            UserOperateLog operaterecord = new UserOperateLog();
            operaterecord.OperateDT = DateTime.Now;
            operaterecord.Operator = ((Users)Session["User"]).UserName;
            operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除" + re.OrderNumber + "小样报告,记录Id为：" + re.Id + ".";
            db.Small_Sample.Remove(re);
            db.UserOperateLog.Add(operaterecord);
            int count = db.SaveChanges();
            if (count > 0) return Content(report_ordernum + "小样报告删除成功！");
            else return Content(report_ordernum + "小样报告删除失败");
        }
        #endregion

        [HttpPost]
        public Bitmap PicToGrayPic(Bitmap pic)
        {
            var result = BarCodeLablePrint.ToGray(pic);
            result = BarCodeLablePrint.ConvertTo1Bpp2(result);
            return result;
        }

        public static bool IsNumberic(string str)
        {
            double vsNum;
            bool isNum;
            isNum = double.TryParse(str, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out vsNum);
            return isNum;
        }
    }
}