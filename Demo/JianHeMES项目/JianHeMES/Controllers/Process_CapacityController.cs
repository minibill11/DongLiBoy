using JianHeMES.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class Process_CapacityController : Controller
    {

        // GET: Process_Capacity
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult Upload_Pick_And_Place()
        //{
        //    try
        //    {
        //        HttpPostedFileBase uploadfile = Request.Files["fileup"];
        //        if (uploadfile == null)
        //        {
        //            return Content("no:非法上传");
        //        }
        //        if (uploadfile.FileName == "")
        //        {
        //            return Content("no:请选择文件");
        //        }

        //        string fileExt = Path.GetExtension(uploadfile.FileName);
        //        StringBuilder sbtime = new StringBuilder();
        //        sbtime.Append(DateTime.Now.Year).Append(DateTime.Now.Month).Append(DateTime.Now.Day).Append(DateTime.Now.Hour).Append(DateTime.Now.Minute).Append(DateTime.Now.Second);
        //        string dir = "/UploadFile/" + sbtime.ToString() + fileExt;
        //        string realfilepath = Request.MapPath(dir);
        //        string readDir = Path.GetDirectoryName(realfilepath);
        //        if (!Directory.Exists(readDir))
        //            Directory.CreateDirectory(readDir);
        //        uploadfile.SaveAs(realfilepath);
        //        //提取数据 
        //        var dt = ExcelTool.ExcelToDataTable(true, realfilepath);
        //        int j = 16;
        //        while (dt.Rows[j][0].ToString() == "修订")
        //        {
        //            j++;
        //        }
        //        var content = "";
        //        while (dt.Rows[j + 1][1].ToString() != null)
        //        {
        //            content = content + dt.Rows[j + 1][1].ToString() + "&&";
        //        }
        //        Pick_And_Place pick = new Pick_And_Place();
        //        for (int i = 1; i < 15; i++)
        //        {
        //            pick.SerialNumber = dt.Rows[0][1].ToString();//编号
        //            pick.Section = dt.Rows[0][3].ToString();//工段
        //            pick.FileName = dt.Rows[0][5].ToString();//文件名称
        //            pick.Line = i;//线别
        //            pick.MachineConfiguration = dt.Rows[i + 1][1].ToString();//机台配置
        //            pick.ProductType = dt.Rows[i + 1][2].ToString();//产品型号
        //            pick.PCBNumber = dt.Rows[i + 1][3].ToString();//pcb版号
        //            pick.ProcessDescription = dt.Rows[i + 1][4].ToString();//工序描述
        //            pick.Print = (decimal)dt.Rows[i + 1][5];//印刷
        //            pick.SolderPasteInspection = (decimal)dt.Rows[i + 1][6];//锡膏检测
        //            pick.PressedPatchNut = (decimal)dt.Rows[i + 1][7];//压贴片螺母
        //            pick.SMTMachineNetWork = (decimal)dt.Rows[i + 1][8];//贴片机净作业
        //            pick.PersonNum = (int)dt.Rows[i + 1][9];//人数
        //            pick.Bottleneck = (decimal)dt.Rows[i + 1][10];//瓶颈  
        //            pick.CapacityPerHour = (decimal)dt.Rows[i + 1][11];//每小时标准产能
        //            pick.PerCapitaCapacity = (decimal)dt.Rows[i + 1][12];//每小时人均产能
        //            pick.LatestUnitPrice = (decimal)dt.Rows[i + 1][13];//产品最新单价
        //            pick.Remark = dt.Rows[i + 1][14].ToString();//备注
        //            pick.MakingPeople = dt.Rows[j + 1][5].ToString();//制定人
        //            var time = dt.Rows[j + 2][5].ToString().Split('/');
        //            pick.MakingTime = new DateTime(Convert.ToInt32(time[2]), DataTypeChange.ChineseMonthChangeInt(time[1]), Convert.ToInt32(time[0]));
        //            pick.ExaminanPeople = dt.Rows[j + 1][8].ToString();//审批
        //            var time2 = dt.Rows[j + 2][8].ToString().Split('/');
        //            pick.ExaminanTime = new DateTime(Convert.ToInt32(time2[2]), DataTypeChange.ChineseMonthChangeInt(time2[1]), Convert.ToInt32(time2[0]));
        //            pick.ApproverPeople = dt.Rows[j + 1][11].ToString();//批准
        //            var time3 = dt.Rows[j + 2][11].ToString().Split('/');
        //            pick.ApproverTime = new DateTime(Convert.ToInt32(time3[2]), DataTypeChange.ChineseMonthChangeInt(time3[1]), Convert.ToInt32(time3[0]));
        //            pick.ControlledPeople = dt.Rows[j + 1][14].ToString();//受控
        //            var time4 = dt.Rows[j + 2][14].ToString().Split('/');
        //            pick.ControlledTime = new DateTime(Convert.ToInt32(time4[2]), DataTypeChange.ChineseMonthChangeInt(time4[1]), Convert.ToInt32(time4[0]));
        //            pick.RevisionContent = content;
        //        }

        //        return Content("返回内用");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(ex.Message);
        //    }
        //}

        //public ActionResult Upload_ProcessBalance()
        //{
        //    try
        //    {
        //        HttpPostedFileBase uploadfile = Request.Files["fileup"];
        //        if (uploadfile == null)
        //        {
        //            return Content("no:非法上传");
        //        }
        //        if (uploadfile.FileName == "")
        //        {
        //            return Content("no:请选择文件");
        //        }

        //        string fileExt = Path.GetExtension(uploadfile.FileName);
        //        StringBuilder sbtime = new StringBuilder();
        //        sbtime.Append(DateTime.Now.Year).Append(DateTime.Now.Month).Append(DateTime.Now.Day).Append(DateTime.Now.Hour).Append(DateTime.Now.Minute).Append(DateTime.Now.Second);
        //        string dir = "/UploadFile/" + sbtime.ToString() + fileExt;
        //        string realfilepath = Request.MapPath(dir);
        //        string readDir = Path.GetDirectoryName(realfilepath);
        //        if (!Directory.Exists(readDir))
        //            Directory.CreateDirectory(readDir);
        //        uploadfile.SaveAs(realfilepath);
        //        //提取数据 
        //        var dt = ExcelTool.ExcelToDataTable(true, realfilepath);
        //        int j = 16;
        //        while (dt.Rows[j][0].ToString() == "修订")
        //        {
        //            j++;
        //        }
        //        var content = "";
        //        while (dt.Rows[j + 1][1].ToString() != null)
        //        {
        //            content = content + dt.Rows[j + 1][1].ToString() + "&&";
        //        }
        //        ProcessBalance processBalance = new ProcessBalance();

        //        processBalance.SerialNumber = dt.Rows[0][1].ToString();//编号
        //        processBalance.Section = dt.Rows[0][3].ToString();//工段
        //        processBalance.Title = dt.Rows[1][2].ToString();//标题
        //        processBalance.StandardTotal = (int)dt.Rows[1][5];//标准总人数
        //        processBalance.BalanceRate = (decimal)dt.Rows[1][6];//平衡率
        //        processBalance.Bottleneck = (decimal)dt.Rows[1][7];//瓶颈
        //        processBalance.StandardOutput = (decimal)dt.Rows[1][8];//标准产量
        //        processBalance.StandardHourlyOutputPerCapita = (decimal)dt.Rows[1][10];//标准人均时产量
        //        processBalance.ProductWorkingHours = (decimal)dt.Rows[1][12];//产品工时

        //        string ProcessName = "";
        //        int pro = 3;
        //        while (dt.Rows[pro][1].ToString() != null)
        //        {
        //            ProcessName = ProcessName + dt.Rows[pro][1].ToString() + "&&";
        //            pro++;
        //        }
        //        processBalance.ProcessName = ProcessName.Substring(0,ProcessName.Length-2);//工序名称

        //        string StandarPersondNumber = "";
        //        for (int i = 3; i <= pro; i++)
        //        {
        //            StandarPersondNumber= StandarPersondNumber + dt.Rows[pro][1].ToString() + "&&";
        //        }

        //        processBalance.StandarPersondNumber = StandarPersondNumber.Substring(0, ProcessName.Length - 2);//标准人数


                
        //        processBalance.MakingPeople = dt.Rows[j + 1][5].ToString();//制定人
        //        var time = dt.Rows[j + 2][5].ToString().Split('/');
        //        processBalance.MakingTime = new DateTime(Convert.ToInt32(time[2]), DataTypeChange.ChineseMonthChangeInt(time[1]), Convert.ToInt32(time[0]));
        //        processBalance.ExaminanPeople = dt.Rows[j + 1][8].ToString();//审批
        //        var time2 = dt.Rows[j + 2][8].ToString().Split('/');
        //        processBalance.ExaminanTime = new DateTime(Convert.ToInt32(time2[2]), DataTypeChange.ChineseMonthChangeInt(time2[1]), Convert.ToInt32(time2[0]));
        //        processBalance.ApproverPeople = dt.Rows[j + 1][11].ToString();//批准
        //        var time3 = dt.Rows[j + 2][11].ToString().Split('/');
        //        processBalance.ApproverTime = new DateTime(Convert.ToInt32(time3[2]), DataTypeChange.ChineseMonthChangeInt(time3[1]), Convert.ToInt32(time3[0]));
        //        processBalance.ControlledPeople = dt.Rows[j + 1][14].ToString();//受控
        //        var time4 = dt.Rows[j + 2][14].ToString().Split('/');
        //        processBalance.ControlledTime = new DateTime(Convert.ToInt32(time4[2]), DataTypeChange.ChineseMonthChangeInt(time4[1]), Convert.ToInt32(time4[0]));
        //        processBalance.RevisionContent = content;


        //        return Content("返回内用");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(ex.Message);
        //    }
        //}
        //public void FindLast(int row)
        //{

        //}
    }
}