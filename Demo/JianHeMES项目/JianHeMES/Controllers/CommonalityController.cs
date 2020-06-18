﻿using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace JianHeMES.Controllers
{
    public class CommonalityController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //[System.Web.Http.Route("Commonality/GetBarcode_Each_Section_Prompt")]
        [HttpPost]
        public string GetBarcode_Each_Section_Prompt(string barcode)
        {
            JObject result = new JObject();
            var Accemble_Record = db.Assemble.Where(c => c.BoxBarCode == barcode && c.PQCCheckFinish == true && c.RepetitionPQCCheck == false && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();
            result.Add("Accemble_Record", Accemble_Record > 0 ? true : false);
            var FQC_Record = db.FinalQC.Where(c => c.BarCodesNum == barcode && c.FQCCheckFinish == true && c.RepetitionFQCCheck == false && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();
            result.Add("FQC_Record", FQC_Record > 0 ? true : false);
            var MosaiScreen_Record = db.Burn_in_MosaicScreen.Where(c => c.BarCodesNum == barcode && c.OQCMosaicStartTime != null && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();
            result.Add("MosaiScreen_Record", MosaiScreen_Record > 0 ? true : false);
            var Burn_in_Record = db.Burn_in.Where(c => c.BarCodesNum == barcode && c.OQCCheckFinish == true && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();
            result.Add("Burn_in_Record", Burn_in_Record > 0 ? true : false);
            var Calibration_Record = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && c.Normal == true && c.RepetitionCalibration == false && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();
            result.Add("Calibration_Record", Calibration_Record > 0 ? true : false);
            var Appearance_Record = db.Appearance.Where(c => c.BarCodesNum == barcode && c.OQCCheckFinish == true && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();
            result.Add("Appearance_Record", Appearance_Record > 0 ? true : false);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string GetBarcode_List_Each_Section_Prompt(List<string> barcodelist)
        {
            JObject result = new JObject();
            foreach (var barcode in barcodelist)
            {
                JObject barcode_result = new JObject();
                var Accemble_Record = db.Assemble.Where(c => c.BoxBarCode == barcode && c.PQCCheckFinish == true && c.RepetitionPQCCheck == false && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();
                barcode_result.Add("Accemble_Record", Accemble_Record > 0 ? true : false);
                var FQC_Record = db.FinalQC.Where(c => c.BarCodesNum == barcode && c.FQCCheckFinish == true && c.RepetitionFQCCheck == false && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();
                barcode_result.Add("FQC_Record", FQC_Record > 0 ? true : false);
                var Burn_in_Record = db.Burn_in.Where(c => c.BarCodesNum == barcode && c.OQCCheckFinish == true && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();
                barcode_result.Add("Burn_in_Record", Burn_in_Record > 0 ? true : false);
                var Calibration_Record = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && c.Normal == true && c.RepetitionCalibration == false && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();
                barcode_result.Add("Calibration_Record", Calibration_Record > 0 ? true : false);
                var Appearance_Record = db.Appearance.Where(c => c.BarCodesNum == barcode && c.OQCCheckFinish == true && (c.OldBarCodesNum == null || c.OldBarCodesNum == barcode)).Count();
                barcode_result.Add("Appearance_Record", Appearance_Record > 0 ? true : false);
                result.Add(barcode, barcode_result);
            }
            return JsonConvert.SerializeObject(result);
        }



        /// <summary>
        /// 对象转换成json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObject">需要格式化的对象</param>
        /// <returns>Json字符串</returns>
        public static string DataContractJsonSerialize<T>(T jsonObject)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            string json = null;
            using (MemoryStream ms = new MemoryStream()) //定义一个stream用来存发序列化之后的内容
            {
                serializer.WriteObject(ms, jsonObject);
                json = Encoding.UTF8.GetString(ms.GetBuffer()); //将stream读取成一个字符串形式的数据，并且返回
                ms.Close();
            }
            return json;
        }

        /// <summary>
        /// json字符串转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">要转换成对象的json字符串</param>
        /// <returns></returns>
        public static T DataContractJsonDeserialize<T>(string json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            T obj = default(T);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
            }
            return obj;
        }



        #region  -------------Excel导出帮助类---------------
        /// <summary>
        /// Excel导出帮助类
        /// </summary>
        public class ExcelExportHelper
        {
            public static string ExcelContentType
            {
                get
                {
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                }
            }
            /// <summary>
            /// List转DataTable
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="data"></param>
            /// <returns></returns>
            public static DataTable ListToDataTable<T>(List<T> data)
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
                DataTable dataTable = new DataTable();
                for (int i = 0; i < properties.Count; i++)
                {
                    PropertyDescriptor property = properties[i];
                    dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
                object[] values = new object[properties.Count];
                foreach (T item in data)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = properties[i].GetValue(item);
                    }
                    dataTable.Rows.Add(values);
                }
                return dataTable;
            }
            /// <summary>
            /// 导出Excel
            /// </summary>
            /// <param name="dataTable">数据源</param>
            /// <param name="heading">工作簿Worksheet</param>
            /// <param name="showSrNo">//是否显示行编号</param>
            /// <param name="columnsToTake">要导出的列</param>
            /// <returns></returns>
            public static byte[] ExportExcel(DataTable dataTable, string heading = "", bool showSrNo = false, params string[] columnsToTake)
            {
                byte[] result = null;
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(string.Format("{0}Data", heading));
                    int startRowFrom = string.IsNullOrEmpty(heading) ? 1 : 3; //开始的行
                                                                              //是否显示行编号
                    if (showSrNo)
                    {
                        DataColumn dataColumn = dataTable.Columns.Add("序号", typeof(int));
                        dataColumn.SetOrdinal(0);
                        int index = 1;
                        foreach (DataRow item in dataTable.Rows)
                        {
                            item[0] = index;
                            index++;
                        }
                    }
                    //Add Content Into the Excel File
                    workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);
                    // autofit width of cells with small content 
                    int columnIndex = 1;
                    foreach (DataColumn item in dataTable.Columns)
                    {
                        ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
                        //int maxLength = columnCells.Where(cell => cell.Value != null).Max(cell => cell.Value.ToString().Count());
                        int maxLength = columnCells.Max(cell => cell.ToString().Count());
                        if (maxLength < 150)
                        {
                            workSheet.Column(columnIndex).AutoFit();
                        }
                        columnIndex++;
                    }
                    // format header - bold, yellow on black 
                    using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                    {
                        r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        r.Style.Font.Bold = true;
                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
                    }
                    // format cells - add borders 
                    using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                    {
                        r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                        r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    }
                    //// removed ignored columns 
                    //for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                    //{
                    //    if (i == 0 && showSrNo)
                    //    {
                    //        continue;
                    //    }
                    //    if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                    //    {
                    //        workSheet.DeleteColumn(i + 1);
                    //    }
                    //}
                    if (!String.IsNullOrEmpty(heading))
                    {
                        workSheet.Cells["A1"].Value = heading;

                        int c = 1;
                        foreach (var a in columnsToTake)
                        {
                            workSheet.Cells[3, c].Value = a;
                            workSheet.Cells[3, c].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                            workSheet.Cells[3, c].Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                            c++;
                        }

                        // workSheet.Column(dataTable.Columns.Count).Style.Numberformat.Format = "yyyy-MM-dd hh:mm";
                        //workSheet.Column(dataTable.Columns.Count).Style.ShrinkToFit = true;  //字体自适应大小
                        workSheet.Column(dataTable.Columns.Count).Width = 20;//设置列宽
                        for (int i = dataTable.Columns.Count - 1; i > 0; i--)  //设置前几列的列宽
                        {
                            workSheet.Column(i).Width = 12;//设置列宽
                        }

                        workSheet.InsertColumn(1, 1);
                        workSheet.InsertRow(1, 1);
                        workSheet.Column(1).Width = 5;

                        workSheet.Cells["B2"].Style.Font.Size = 18;
                    }
                    result = package.GetAsByteArray();
                }
                return result;
            }
            /// <summary>
            /// 导出Excel
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="data"></param>
            /// <param name="heading"></param>
            /// <param name="isShowSlNo"></param>
            /// <param name="ColumnsToTake"></param>
            /// <returns></returns>
            public static byte[] ExportExcel<T>(List<T> data, string heading = "", bool isShowSlNo = false, params string[] ColumnsToTake)
            {
                return ExportExcel(ListToDataTable<T>(data), heading, isShowSlNo, ColumnsToTake);
            }

            //JSON转DataTable
            public static DataTable JsonToDataTable(string json)
            {
                DataTable dataTable = new DataTable();  //实例化
                DataTable result;
                try
                {
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                    ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                    if (arrayList.Count > 0)
                    {
                        foreach (Dictionary<string, object> dictionary in arrayList)
                        {
                            if (dictionary.Keys.Count<string>() == 0)
                            {
                                result = dataTable;
                                return result;
                            }
                            if (dataTable.Columns.Count == 0)
                            {
                                foreach (string current in dictionary.Keys)
                                {
                                    dataTable.Columns.Add(current, dictionary[current].GetType());
                                }
                            }
                            DataRow dataRow = dataTable.NewRow();
                            foreach (string current in dictionary.Keys)
                            {
                                dataRow[current] = dictionary[current];
                            }

                            dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                        }
                    }
                }
                catch
                {
                }
                result = dataTable;
                return result;
            }
        }



        #endregion

        #region------------计算两个日期相差几年几个月

        /// <summary>
        /// 计算日期的间隔(静态类)
        /// </summary>
        public static class dateTimeDiff
        {
            /// <summary>
            /// 计算日期间隔
            /// </summary>
            /// <param name="d1">要参与计算的其中一个日期字符串</param>
            /// <param name="d2">要参与计算的另一个日期字符串</param>
            /// <returns>一个表示日期间隔的TimeSpan类型</returns>
            public static TimeSpan toResult(string d1, string d2)
            {
                try
                {
                    DateTime date1 = DateTime.Parse(d1);
                    DateTime date2 = DateTime.Parse(d2);
                    return toResult(date1, date2);
                }
                catch
                {
                    throw new Exception("字符串参数不正确!");
                }
            }
            /// <summary>
            /// 计算日期间隔
            /// </summary>
            /// <param name="d1">要参与计算的其中一个日期</param>
            /// <param name="d2">要参与计算的另一个日期</param>
            /// <returns>一个表示日期间隔的TimeSpan类型</returns>
            public static TimeSpan toResult(DateTime d1, DateTime d2)
            {
                TimeSpan ts;
                if (d1 > d2)
                {
                    ts = d1 - d2;
                }
                else
                {
                    ts = d2 - d1;
                }
                return ts;
            }

            /// <summary>
            /// 计算日期间隔
            /// </summary>
            /// <param name="d1">要参与计算的其中一个日期字符串</param>
            /// <param name="d2">要参与计算的另一个日期字符串</param>
            /// <param name="drf">决定返回值形式的枚举</param>
            /// <returns>一个代表年月日的int数组，具体数组长度与枚举参数drf有关</returns>
            public static int[] toResult(string d1, string d2, diffResultFormat drf)
            {
                try
                {
                    DateTime date1 = DateTime.Parse(d1);
                    DateTime date2 = DateTime.Parse(d2);
                    return toResult(date1, date2, drf);
                }
                catch
                {
                    throw new Exception("字符串参数不正确!");
                }
            }
            /// <summary>
            /// 计算日期间隔
            /// </summary>
            /// <param name="d1">要参与计算的其中一个日期</param>
            /// <param name="d2">要参与计算的另一个日期</param>
            /// <param name="drf">决定返回值形式的枚举</param>
            /// <returns>一个代表年月日的int数组，具体数组长度与枚举参数drf有关</returns>
            public static int[] toResult(DateTime d1, DateTime d2, diffResultFormat drf)
            {
                #region 数据初始化
                DateTime max;
                DateTime min;
                int year;
                int month;
                int tempYear, tempMonth;
                if (d1 > d2)
                {
                    max = d1;
                    min = d2;
                }
                else
                {
                    max = d2;
                    min = d1;
                }
                tempYear = max.Year;
                tempMonth = max.Month;
                if (max.Month < min.Month)
                {
                    tempYear--;
                    tempMonth = tempMonth + 12;
                }
                year = tempYear - min.Year;
                month = tempMonth - min.Month;
                #endregion
                #region 按条件计算
                if (drf == diffResultFormat.dd)
                {
                    TimeSpan ts = max - min;
                    return new int[] { ts.Days };
                }
                if (drf == diffResultFormat.mm)
                {
                    return new int[] { month + year * 12 };
                }
                if (drf == diffResultFormat.yy)
                {
                    return new int[] { year };
                }
                if (drf == diffResultFormat.hh)
                {
                    TimeSpan ts = max - min;
                    return new int[] { ts.Hours };
                }
                return new int[] { year, month };
                #endregion
            }
        }
        /// <summary>
        /// 关于返回值形式的枚举
        /// </summary>
        public enum diffResultFormat
        {
            /// <summary>
            /// 年数和月数
            /// </summary>
            yymm,
            /// <summary>
            /// 年数
            /// </summary>
            yy,
            /// <summary>
            /// 月数
            /// </summary>
            mm,
            /// <summary>
            /// 天数
            /// </summary>
            dd,
            /// <summary>
            /// 时长
            /// </summary>
            hh,
        }
        #endregion

        //将毫秒转换成时分秒
        public string changetimespan(double value)
        {
            string result = "";
            double day = value / 86400000;
            day = Math.Floor(day);
            double houres = Math.Floor((value - (86400000 * day)) / 3600000);
            double min = Math.Floor((value - (86400000 * day) - (3600000 * houres)) / 60000);
            double second = Math.Floor((value - (86400000 * day) - (3600000 * houres) - (60000 * min)) / 1000);
            return result = day + "天" + houres + "时" + min + "分" + second + "秒";

        }

        /// <summary>
        /// 计算两个时间的月数，
        /// </summary>
        /// 情况1 
        /// 传过来的两个时间刚好是月末的最后一天 如2020/4/30 和2020/7/31   
        /// 或者是后面的时间是月末,前面的时间不是, 如2020/4/5 和2020/7/31
        /// 或者是后面的时间的天大于前面的时间天 如2020/4/5 和2020/7/20
        /// 这几个的共同点都是 后面的时间的天是>=前面时间的,
        /// 计算方法是 
        /// 1.如果date2的月份大于date1 直接date2年-date1年*12+date2月-date1月    如2020/4/30 和2020/7/31   
        /// 2.如果date2的年月都等于date1的年月,则代表不满一月,date2日-date1日/date2月的最后一天  如2020/4/1 和2020/4/20   
        /// 3.如果date2的月份小于于date1 直接date2年-date1年-1*12+date2月+12-date1月  如  2019/4/1 和2020/3/20 
        /// 情况2
        /// date1是月末并且date2不是月末 如2020/4/30 和2020/7/20
        /// 或者date2的天小于date1的天 如2020/4/25 和2020/7/20
        /// 这几个的共同点是 date2的天是小于date1的天的
        /// 计算方法是
        /// 1.如果date2月>date1月并且date2-date1=1并且date2年=date1年 则(date1月的最后一天-date1日+date2日)/30  如2020/4/26 和2020/5/17
        /// 2.如果date2月>date1月并且date2-date1!=1或者date2年!=date1年 则date2年-date1年*12+date2月-date1月-1  如2020/4/26 和2020/7/17
        /// 3.如果date2月小于date1月 则date2年-date1年-1*12+date2月+12-date1月-1  如2019/4/26 和2020/2/17
        /// <param name="date2">LastDate</param>
        /// <param name="date1">FirstDate</param>
        /// <returns></returns>
        public decimal TwoDTforMonth_sub(DateTime? date2, DateTime? date1) //默认date2>date1
        {
            if (date1 != null && date2 != null)
            {
                //当月最后一天
                var d1_lastday = DateTime.DaysInMonth(Convert.ToDateTime(date1).Year, Convert.ToDateTime(date1).Month);
                //当月最后一天
                var d2_lastday = DateTime.DaysInMonth(Convert.ToDateTime(date2).Year, Convert.ToDateTime(date2).Month);

                //传过来的日期的天
                var d1_day = Convert.ToDateTime(date1).Day;
                var d2_day = Convert.ToDateTime(date2).Day;
                decimal monthsum = 0;
                //如果传过来的天数两个都刚好是最后一天,或者大的是最后一天,小的不是,或者两个都不是最后一天但大的天比小的大
                if ((d1_lastday == d1_day && d2_lastday == d2_day) || (d1_lastday != d1_day && d2_lastday == d2_day) || (d2_day >= d1_day))
                {
                    //如果大的月份比小的大,则直接 (大的年-小的年)*12+(大的月-小的月)
                    if (Convert.ToDateTime(date2).Month > Convert.ToDateTime(date1).Month)
                    {
                        monthsum = (Convert.ToDateTime(date2).Year - Convert.ToDateTime(date1).Year) * 12 + (Convert.ToDateTime(date2).Month - Convert.ToDateTime(date1).Month);
                    }
                    //如果两个时间的年相等并且两个时间的月份相等,则计算(大的天-小的天)/大的当月最后一天
                    else if (Convert.ToDateTime(date2).Year == Convert.ToDateTime(date1).Year && Convert.ToDateTime(date2).Month == Convert.ToDateTime(date1).Month)
                    {
                        monthsum = Decimal.Parse((Convert.ToDecimal(d2_day - d1_day) / d2_lastday).ToString("F2"));
                    }
                    //否则就是你(大的年-小的年-1)*12+(大的月-小的月)
                    else
                    {
                        monthsum = (Convert.ToDateTime(date2).Year - Convert.ToDateTime(date1).Year - 1) * 12 + (Convert.ToDateTime(date2).Month + 12 - Convert.ToDateTime(date1).Month);
                    }
                }
                //如果小的最后一天等于小的天数并且大的不是 或者大的天数小于小的天数
                else if ((d1_lastday == d1_day && d2_lastday != d2_day) || (d2_day <= d1_day))
                {
                    if (Convert.ToDateTime(date2).Month > Convert.ToDateTime(date1).Month)
                    {
                        if (Convert.ToDateTime(date2).Month - Convert.ToDateTime(date1).Month == 1 && date1.Value.Year == date2.Value.Year)
                        {
                            monthsum = Decimal.Parse(((Convert.ToDecimal(d1_lastday - d1_day) + d2_day) / 30).ToString("F2"));

                        }
                        else
                            monthsum = (Convert.ToDateTime(date2).Year - Convert.ToDateTime(date1).Year) * 12 + (Convert.ToDateTime(date2).Month - Convert.ToDateTime(date1).Month) - 1;
                    }
                    else
                    {
                        monthsum = (Convert.ToDateTime(date2).Year - Convert.ToDateTime(date1).Year - 1) * 12 + (Convert.ToDateTime(date2).Month + 12 - Convert.ToDateTime(date1).Month) - 1;
                        if (monthsum == 0)
                        {
                            monthsum = Decimal.Parse(((Convert.ToDecimal(d1_lastday - d1_day) + d2_day) / 30).ToString("F2"));
                        }
                    }
                }

                return monthsum;
            }
            return 0M;
        }

        //将花名册的部门和组移到组织架构
        public void MessageMove()
        {
            //var restorInfo = db.Personnel_Roster.Select(c => new { c.Department, c.DP_Group }).Distinct().ToList();
            //List<Personnel_Framework> framwork = new List<Personnel_Framework>();
            //foreach (var item in restorInfo)
            //{
            //    Personnel_Framework temp = new Personnel_Framework { Department = item.Department, Group = item.DP_Group };
            //    framwork.Add(temp);
            //}
            //if (framwork.Count != 0)
            //{
            //    db.Personnel_Framework.AddRange(framwork);
            //    db.SaveChangesAsync();
            //}
            var restorDpeartment = db.Personnel_Roster.Select(c => c.Department).Distinct().ToList();
            var restorDP_group = db.Personnel_Roster.Select(c => c.DP_Group).Distinct().ToList();
            List<Personnel_Framework> framwork = new List<Personnel_Framework>();
            foreach (var item in restorDpeartment)
            {
                foreach (var group in restorDP_group)
                {
                    if (item == "MC部" && group == "MC组")
                    {

                    }
                }
            }
        }
        //添加中心层
        public void AddMessage()
        {
            var depar = db.Personnel_Framework.Select(c => c.Department).Distinct().ToList();
            foreach (var item in depar)
            {
                switch (item)
                {
                    case "MC部":
                    case "PC部":
                    case "SMT部":
                    case "配套加工部":
                    case "装配一部":
                    case "装配二部":
                        var Manufacture = db.Personnel_Framework.Where(c => c.Department == item).ToList();
                        foreach (var update in Manufacture)
                        {
                            update.Central_layer = "制造中心";
                            db.SaveChangesAsync();
                        }
                        break;
                    case "品质部":
                    case "技术部":
                        var technology = db.Personnel_Framework.Where(c => c.Department == item).ToList();
                        foreach (var update in technology)
                        {
                            update.Central_layer = "品质技术中心";
                            db.SaveChangesAsync();
                        }
                        break;
                    case "行政后勤部":
                        var logistics = db.Personnel_Framework.Where(c => c.Department == item).ToList();
                        foreach (var update in logistics)
                        {
                            update.Central_layer = "厂务中心";
                            db.SaveChangesAsync();
                        }
                        break;
                    default:
                        var Other = db.Personnel_Framework.Where(c => c.Department == item).ToList();
                        foreach (var update in Other)
                        {
                            update.Central_layer = "其他";
                            db.SaveChangesAsync();
                        }
                        break;
                }

            }
        }

        public void BatchUpdate(T b, List<T> a, string updatevalue)
        {
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
            if (Directory.Exists(strDirectory))
            {
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
            }
            return listFiles;
        }

        #region --------------------返回指定目录下文件夹信息
        /// <summary>  
        /// 返回指定目录下文件夹信息  
        /// </summary>  
        /// <param name="strDirectory">目录字符串</param>  
        /// <returns></returns>  
        public DirectoryInfo[] GetFolderInDirectory(string strDirectory)
        {
            DirectoryInfo directory = new DirectoryInfo(strDirectory);
            DirectoryInfo[] directoryArray = directory.GetDirectories();
            return directoryArray;
        }
        #endregion
        #region --------------------返回指定目录下文件信息
        /// <summary>  
        /// 返回指定目录下文件信息  
        /// </summary>  
        /// <param name="strDirectory">目录字符串</param>  
        /// <returns></returns>  
        public List<FileInfo> GetFilesInDirectory(string strDirectory)
        {
            List<FileInfo> listFiles = new List<FileInfo>(); //保存所有的文件信息  
            DirectoryInfo directory = new DirectoryInfo(strDirectory);
            FileInfo[] fileInfoArray = directory.GetFiles();
            if (fileInfoArray.Length > 0) listFiles.AddRange(fileInfoArray);
            return listFiles;
        }
        #endregion


        //判断JPG文件是否存在
        public bool CheckJpgExit(string ordernum, string fileType)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\" + fileType + "\\" + ordernum + "\\");
            var filesInfojpg = filesInfo.Where(c => c.Name.StartsWith(ordernum) && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
            return filesInfojpg.Count() == 0;
        }
        //判断pdf文件是否存在
        public bool CheckpdfExit(string ordernum, string fileType)
        {
            List<FileInfo> filesInfo = GetAllFilesInDirectory(@"D:\\MES_Data\\" + fileType + "\\" + ordernum + "\\");
            var filesInfopdf = filesInfo.Where(c => c.Name.StartsWith(ordernum) && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();
            return filesInfopdf.Count == 0;
        }


        #endregion

        #region pdf文件跳转
        //pdf文件跳转
        public ActionResult PDFRedirectToAction(string address)
        {
            string path = "http://" + HttpContext.Request.Url.Host;
            string port = HttpContext.Request.Url.Port.ToString();
            if (port == "8080")
            {
                path = path + ":23525";
            }
            if (port == "18863")
            {
                path = path + ":18863";
            }
            path = path + address;
            return Redirect(path);


            //获取本地ip
            //string AddressIP = string.Empty;
            //foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            //{
            //    if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
            //    {
            //        AddressIP = _IPAddress.ToString();
            //    }
            //}
        }
        #endregion

        #region 获取访问的本地域名
        public ActionResult url()
        {
            string url = HttpContext.Request.Url.Host;
            return Content(url);
        }

        #endregion

        public ActionResult ischeckBarcode(string orderNum)
        {
            var count = db.BarCodes.Count(c => c.OrderNum == orderNum && c.IsRepertory == true);
            if (count == 0)
            { return Content("没有条码"); }
            else
                return Content("有条码");
        }


        #region  插入关联表
        public bool InsertRelation(BarCodeRelation barcoderelation)
        {
            //var ordernum = db.BarCodes.Where(c => c.BarCodesNum == oldBarcode).Select(c => c.OrderNum).FirstOrDefault();
            //BarCodeRelation barcoderelation = new BarCodeRelation() {OldOrderNum=ordernum, OldBarCodeNum = oldBarcode, NewBarCodesNum = newBarcode, NewOrderNum = newOrderNum, Procedure = procedure, UsserID = userid, CreateDate = DateTime.Now };
            var havesameinOld = db.BarCodeRelation.Count(c => c.OldOrderNum == barcoderelation.OldOrderNum && c.OldBarCodeNum == barcoderelation.OldBarCodeNum);
            var havesameinNew = db.BarCodeRelation.Count(c => c.NewOrderNum == barcoderelation.NewOrderNum && c.NewBarCodesNum == barcoderelation.NewBarCodesNum);
            if (havesameinNew == 0 && havesameinOld == 0)
            {
                //var relation = db.BarCodeRelation.Where(c => c.NewBarCodesNum == barcoderelation.OldBarCodeNum && c.NewOrderNum == barcoderelation.OldOrderNum).FirstOrDefault();
                //if (relation != null)
                //{
                //    barcoderelation.OldBarCodeNum = relation.OldBarCodeNum;
                //    barcoderelation.OldOrderNum = relation.OldOrderNum;
                //}
                db.BarCodeRelation.Add(barcoderelation);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        #region 判断库存条码
        public string CheckBarCodeNumIsRepertory(string barcodenum, string ordernum)
        {
            var isExict = db.BarCodes.Count(c => c.BarCodesNum == barcodenum);
            if (isExict == 0) { return "没有找到此条码"; }
            else
            {
                var isbelonge = db.BarCodes.Where(c => c.BarCodesNum == barcodenum).FirstOrDefault().OrderNum;
                if (isbelonge != ordernum)
                    return "此条码不属于此订单,属于订单" + isbelonge;

                else
                    return "true";
            }
        }
        #endregion

        //计算时长
        public double CalculationHours(DateTime d1, DateTime d2)
        {
            DateTime max;
            DateTime min;
            if (d1 > d2)
            {
                max = d1;
                min = d2;
            }
            else
            {
                max = d2;
                min = d1;
            }
            TimeSpan ts = max - min;
            return ts.TotalHours;
        }


        #region --------------------检索订单号------
        public List<SelectListItem> GetOrderNumList()
        {
            var ordernum = db.OrderMgm.OrderBy(m => m.OrderNum).Select(m => m.OrderNum).Distinct();
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

        public ActionResult GetOrderNumListJSON()
        {
            var ordernum = db.OrderMgm.OrderBy(m => m.OrderNum).Select(m => m.OrderNum).Distinct();
            JObject ordernumitems = new JObject();
            foreach (string num in ordernum)
            {
                ordernumitems.Add(num, num);
            }
            return Content(JsonConvert.SerializeObject(ordernumitems));
        }
        #endregion
    }


    //集合与DataTable间的相互转换创建类
    #region------集合与DataTable间的相互转换创建类
    public static class DataTableTool
    {
        /// <summary>    
        /// 转化一个DataTable    
        /// </summary>    
        /// <typeparam name="T"></typeparam>    
        /// <param name="list"></param>    
        /// <returns></returns>    
        public static DataTable ToDataTable<T>(this IEnumerable<T> list)
        {

            //创建属性的集合    
            List<PropertyInfo> pList = new List<PropertyInfo>();
            //获得反射的入口    

            Type type = typeof(T);
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列    
            Array.ForEach<PropertyInfo>(type.GetProperties(), p => { pList.Add(p); dt.Columns.Add(p.Name, p.PropertyType); });
            foreach (var item in list)
            {
                //创建一个DataRow实例    
                DataRow row = dt.NewRow();
                //给row 赋值    
                pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                //加入到DataTable    
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static DataTable ListToDataTable<T>(List<T> list)
        {
            Type type = typeof(T);
            PropertyInfo[] proInfo = type.GetProperties();
            DataTable dt = new DataTable();
            foreach (PropertyInfo p in proInfo)
            {
                //类型存在Nullable<Type>时，需要进行以下处理，否则异常
                Type t = p.PropertyType;
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    t = t.GetGenericArguments()[0];
                dt.Columns.Add(p.Name, t);
            }
            foreach (T t in list)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo p in proInfo)
                {
                    object obj = p.GetValue(t);
                    if (obj == null) continue;
                    if (p.PropertyType == typeof(DateTime) && Convert.ToDateTime(obj) < Convert.ToDateTime("1753-01-01"))
                        continue;
                    dr[p.Name] = obj;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }


        /// <summary>    
        /// DataTable 转换为List 集合    
        /// </summary>    
        /// <typeparam name="TResult">类型</typeparam>    
        /// <param name="dt">DataTable</param>    
        /// <returns></returns>    
        public static List<T> DataTableToList<T>(this DataTable dt) where T : class, new()
        {
            //创建一个属性的列表    
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口    

            Type t = typeof(T);

            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表     
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });

            //创建返回的集合    

            List<T> oblist = new List<T>();

            foreach (DataRow row in dt.Rows)
            {
                //创建TResult的实例    
                T ob = new T();
                //找到对应的数据  并赋值    
                prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                //放入到返回的集合中.    
                oblist.Add(ob);
            }
            return oblist;
        }


        /// <summary>    
        /// 将集合类转换成DataTable    
        /// </summary>    
        /// <param name="list">集合</param>    
        /// <returns></returns>    
        public static DataTable ToDataTableTow(IList list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();

                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }



        /// <summary>    
        /// 将泛型集合类转换成DataTable    

        /// </summary>    
        /// <typeparam name="T">集合项类型</typeparam>    

        /// <param name="list">集合</param>    
        /// <returns>数据集(表)</returns>    
        public static DataTable ToDataTable<T>(IList<T> list)
        {
            return ToDataTable<T>(list, null);
        }

        /// <summary>    
        /// 将泛型集合类转换成DataTable    
        /// </summary>    
        /// <typeparam name="T">集合项类型</typeparam>    
        /// <param name="list">集合</param>    
        /// <param name="propertyName">需要返回的列的列名</param>    
        /// <returns>数据集(表)</returns>    
        public static DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
    }
    #endregion


    //excel导入导出类
    #region------excel导入导出类
    public static class ExcelTool
    { /// <summary>
      /// 将excel中的数据导入到DataTable中
      /// </summary>
      /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
      /// <param name="fileName">文件路径</param>
      /// <param name="sheetName">excel工作薄sheet的名称</param>
      /// <returns>返回的DataTable</returns>
        public static DataTable ExcelToDataTable(bool isFirstRowColumn, string fileName, string sheetName = "")
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(fileName);
            }
            var data = new DataTable();
            IWorkbook workbook = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx", StringComparison.Ordinal) > 0)
                {
                    try
                    {
                        workbook = new XSSFWorkbook(fs);
                    }
                    catch
                    {
                        throw new Exception("上传失败，请尝试另为存上传文件，再次上传保存后的文件");
                    }
                }
                else if (fileName.IndexOf(".xls", StringComparison.Ordinal) > 0)
                {
                    try
                    {
                        workbook = new HSSFWorkbook(fs);
                    }
                    catch
                    {
                        throw new Exception("上传失败，请尝试另为存上传文件，再次上传保存后的文件");
                    }
                }

                ISheet sheet = null;
                if (workbook != null)
                {
                    //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    if (sheetName == "")
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    else
                    {
                        sheet = workbook.GetSheet(sheetName) ?? workbook.GetSheetAt(0);
                    }
                }
                if (sheet == null) return data;
                var firstRow = sheet.GetRow(0);
                //一行最后一个cell的编号 即总的列数
                int cellCount = firstRow.LastCellNum;
                int startRow;
                if (isFirstRowColumn)
                {
                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        var cell = firstRow.GetCell(i);
                        var cellValue = cell.StringCellValue;
                        if (cellValue == null) continue;
                        var column = new DataColumn(cellValue);
                        data.Columns.Add(column);
                    }
                    startRow = sheet.FirstRowNum + 1;
                }
                else
                {
                    startRow = sheet.FirstRowNum;
                }
                //最后一列的标号
                var rowCount = sheet.LastRowNum;
                for (var i = startRow; i <= rowCount; ++i)
                {
                    var row = sheet.GetRow(i);
                    //没有数据的行默认是null
                    if (row == null) continue;
                    var dataRow = data.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                    {
                        //同理，没有数据的单元格都默认是null
                        if (row.GetCell(j) != null)
                        {//dataRow[j] = row.GetCell(j).ToString();
                            if (row.GetCell(j).CellType == CellType.Boolean)
                            {
                                dataRow[j] = row.GetCell(j).NumericCellValue.ToString();
                            }
                            if (row.GetCell(j).CellType == CellType.Formula)
                            {
                                try
                                {
                                    var bb = row.GetCell(j).NumericCellValue;
                                    var aa = row.GetCell(j).NumericCellValue.ToString();
                                    dataRow[j] = row.GetCell(j).NumericCellValue.ToString();
                                }
                                catch
                                {
                                    dataRow[j] = "";
                                }
                            }
                            else
                            {
                                dataRow[j] = row.GetCell(j).ToString();
                            }
                        }

                    }
                    data.Rows.Add(dataRow);
                }

                return data;
            }
            catch (IOException ioex)
            {
                throw new IOException(ioex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <param name="fileName">文件夹路径</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public static int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten, string fileName)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (string.IsNullOrEmpty(sheetName))
            {
                throw new ArgumentNullException(sheetName);
            }
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(fileName);
            }
            IWorkbook workbook = null;
            if (fileName.IndexOf(".xlsx", StringComparison.Ordinal) > 0)
            {
                workbook = new XSSFWorkbook();
            }
            else if (fileName.IndexOf(".xls", StringComparison.Ordinal) > 0)
            {
                workbook = new HSSFWorkbook();
            }

            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                ISheet sheet;
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                int j;
                int count;
                //写入DataTable的列名，写入单元格中
                if (isColumnWritten)
                {
                    var row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }
                //遍历循环datatable具体数据项
                int i;
                for (i = 0; i < data.Rows.Count; ++i)
                {
                    var row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                //将文件流写入到excel
                workbook.Write(fs);
                return count;
            }
            catch (IOException ioex)
            {
                throw new IOException(ioex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
    }

    /// <summary>
    /// 提供将泛型集合数据导出Excel文档。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExcelResult<T> : ActionResult where T : new()
    {
        public ExcelResult(IList<T> entity, string fileName, bool showDisplayName = true)
        {
            this.Entity = entity;
            this.FileName = fileName;
            this.ShowDisplayName = showDisplayName;
        }

        public ExcelResult(IList<T> entity, bool showDisplayName = true)
        {
            this.Entity = entity;

            DateTime time = DateTime.Now;
            this.FileName = string.Format("{0}_{1}_{2}_{3}",
                time.Month, time.Day, time.Hour, time.Minute);
            this.ShowDisplayName = showDisplayName;
        }

        public IList<T> Entity
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public bool ShowDisplayName
        {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (Entity == null)
            {
                new EmptyResult().ExecuteResult(context);
                return;
            }

            SetResponse(context);
        }

        /// <summary>
        /// 设置并向客户端发送请求响应。
        /// </summary>
        /// <param name="context"></param>
        private void SetResponse(ControllerContext context)
        {
            StringBuilder sBuilder = ConvertEntity();
            byte[] bytestr = Encoding.Unicode.GetBytes(sBuilder.ToString());
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ClearContent();
            context.HttpContext.Response.Buffer = true;
            context.HttpContext.Response.Charset = "GB2312";
            //添加中文GB2312格式 
            context.HttpContext.Response.ContentEncoding = Encoding.GetEncoding("GB2312");
            context.HttpContext.Response.ContentType = "application/ms-excel";
            context.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + ".xls");
            context.HttpContext.Response.AddHeader("Content-Length", bytestr.Length.ToString());
            context.HttpContext.Response.Write(sBuilder);
            //添加Flush方法        
            context.HttpContext.Response.Flush();
            //添加Close方法           
            context.HttpContext.Response.Close();
            context.HttpContext.Response.End();
        }

        /// <summary>
        /// 把泛型集合转换成组合Excel表格的字符串。
        /// </summary>
        /// <returns></returns>
        private StringBuilder ConvertEntity()
        {
            StringBuilder sb = new StringBuilder();
            AddTableHead(sb);
            AddTableBody(sb);
            return sb;
        }

        /// <summary>
        /// 根据IList泛型集合中的每项的属性值来组合Excel表格。
        /// </summary>
        /// <param name="sb"></param>
        private void AddTableBody(StringBuilder sb)
        {
            if (Entity == null || Entity.Count <= 0)
            {
                return;
            }

            PropertyInfo[] properties = typeof(T).GetProperties();

            if (properties.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < Entity.Count; i++)
            {
                for (int j = 0; j < properties.Length; j++)
                {
                    string sign = j == properties.Length - 1 ? "\n" : "\t";
                    object obj = properties[j].GetValue(Entity[i], null);
                    sb.Append(obj ?? string.Empty).Append(sign);
                }
            }
        }

        /// <summary>
        /// 根据指定类型T的所有属性名称来组合Excel表头。
        /// </summary>
        /// <param name="sb"></param>
        private void AddTableHead(StringBuilder sb)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            if (properties.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < properties.Length; i++)
            {
                string headName = properties[i].Name;
                string sign = i == properties.Length - 1 ? "\n" : "\t";
                if (!ShowDisplayName)
                {
                    sb.Append(headName).Append(sign);
                    continue;
                }

                Attribute attribute = Attribute.GetCustomAttribute(properties[i], typeof(DisplayNameAttribute));
                if (attribute != null)
                {
                    DisplayNameAttribute displayNameAttribute = attribute as DisplayNameAttribute;
                    if (displayNameAttribute != null && !string.IsNullOrWhiteSpace(displayNameAttribute.DisplayName))
                    {
                        headName = displayNameAttribute.DisplayName;
                    }
                }

                sb.Append(headName).Append(sign);
            }
        }
    }

    #endregion

    #region------JSON.Hepler类
    public class JsonHelper
    {
        /// <summary>
        /// json反序列化
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object jsonDes<T>(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<T>(input);
        }
        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string json(object obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(obj);
        }
    }
    #endregion

    public class DirectoryFileOperate
    {
        /// <summary>
        /// 删除文件夹以及文件
        /// </summary>
        /// <param name="directoryPath"> 文件夹路径 </param>
        /// <param name="fileName"> 文件名称 </param>
        public static void DeleteDirectory(string directoryPath, string fileName)
        {
            //删除文件
            for (int i = 0; i < Directory.GetFiles(directoryPath).ToList().Count; i++)
            {
                if (Directory.GetFiles(directoryPath)[i] == fileName)
                {
                    File.Delete(fileName);
                }
            }

            //删除文件夹
            for (int i = 0; i < Directory.GetDirectories(directoryPath).ToList().Count; i++)
            {
                if (Directory.GetDirectories(directoryPath)[i] == fileName)
                {
                    Directory.Delete(fileName, true);
                }
            }
        }
    }


    //对象克隆
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

    public class DataTypeChange
    {
        /// <SUMMARY>
        /// 判断字符串是否可以转化为数字
        /// </SUMMARY>
        /// <PARAM name="str">要检查的字符串</PARAM>
        /// <RETURNS>true:可以转换为数字；false:不是数字</RETURNS>
        public static bool IsNumberic(string str)
        {
            double vsNum;
            bool isNum;
            isNum = double.TryParse(str, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out vsNum);
            return isNum;
        }

        ///<SUMMARY>
        ///中文月份转数字
        ///<PARAM name = "string">
        ///

        public static int ChineseMonthChangeInt(string month)
        {
            int result = 1;
            switch (month)
            {
                case "一月":
                    result = 1;
                    break;
                case "1月":
                    result = 1;
                    break;
                case "二月":
                    result = 2;
                    break;
                case "2月":
                    result = 2;
                    break;
                case "三月":
                    result = 3;
                    break;
                case "3月":
                    result = 3;
                    break;
                case "四月":
                    result = 4;
                    break;
                case "4月":
                    result = 4;
                    break;
                case "五月":
                    result = 5;
                    break;
                case "5月":
                    result = 5;
                    break;
                case "六月":
                    result = 6;
                    break;
                case "6月":
                    result = 6;
                    break;
                case "七月":
                    result = 7;
                    break;
                case "7月":
                    result = 7;
                    break;
                case "八月":
                    result = 8;
                    break;
                case "8月":
                    result = 8;
                    break;
                case "九月":
                    result = 9;
                    break;
                case "9月":
                    result = 9;
                    break;
                case "十月":
                    result = 10;
                    break;
                case "10月":
                    result = 10;
                    break;
                case "十一月":
                    result = 11;
                    break;
                case "11月":
                    result = 11;
                    break;
                case "十二月":
                    result = 12;
                    break;
                case "12月":
                    result = 12;
                    break;

                default:
                    break;
            }
            return result;

        }

    }

}