using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.IO;
using Microsoft.Office.Interop.Word;


namespace JianHeMES.Controllers
{
    public class ManualController : Controller
    {
        // GET: Manual
        public ActionResult Index(string url = "~/MES_Data/MES说明书/备品配件说明书/1.备品配件首页.pdf")
        {
            string physicalPath = Server.MapPath(Server.UrlDecode(url));
            string extension = Path.GetExtension(physicalPath);

            string htmlUrl = "";
            switch (extension.ToLower())
            {
                case ".xls":
                case ".xlsx":
                    htmlUrl = PreviewExcel(physicalPath, url);
                    break;
                case ".doc":
                case ".docx":
                    htmlUrl = PreviewWord(physicalPath, url);
                    break;
                case ".txt":
                    htmlUrl = PreviewTxt(physicalPath, url);
                    break;
                case ".pdf":
                    htmlUrl = PreviewPdf(physicalPath, url);
                    break;
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".gif":
                case ".png":
                    htmlUrl = PreviewImg(physicalPath, url);
                    break;
                default:
                    htmlUrl = PreviewOther(physicalPath, url);
                    break;
            }
            return View();
        }

        #region Index页面
        ///// <summary>
        ///// Index页面
        ///// </summary>
        ///// <param name="url">例：/uploads/......XXX.xls</param>
        ////[HttpPost]
        //public ActionResult Index(string url= "~/MES_Data/MES说明书/备品配件说明书/1.备品配件首页.pdf")
        //{
        //    string physicalPath = Server.MapPath(Server.UrlDecode(url));
        //    string extension = Path.GetExtension(physicalPath);

        //    string htmlUrl = "";
        //    switch (extension.ToLower())
        //    {
        //        case ".xls":
        //        case ".xlsx":
        //            htmlUrl = PreviewExcel(physicalPath, url);
        //            break;
        //        case ".doc":
        //        case ".docx":
        //            htmlUrl = PreviewWord(physicalPath, url);
        //            break;
        //        case ".txt":
        //            htmlUrl = PreviewTxt(physicalPath, url);
        //            break;
        //        case ".pdf":
        //            htmlUrl = PreviewPdf(physicalPath, url);
        //            break;
        //        case ".jpg":
        //        case ".jpeg":
        //        case ".bmp":
        //        case ".gif":
        //        case ".png":
        //            htmlUrl = PreviewImg(physicalPath, url);
        //            break;
        //        default:
        //            htmlUrl = PreviewOther(physicalPath, url);
        //            break;
        //    }

        //    return Redirect(Url.Content(htmlUrl));
        //}
        #endregion

        #region 预览Excel
        /// <summary>
        /// 预览Excel
        /// </summary>
        public string PreviewExcel(string physicalPath, string url)
        {
            Microsoft.Office.Interop.Excel.Application application = null;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            application = new Microsoft.Office.Interop.Excel.Application();
            object missing = Type.Missing;
            object trueObject = true;
            application.Visible = false;
            application.DisplayAlerts = false;
            workbook = application.Workbooks.Open(physicalPath, missing, trueObject, missing, missing, missing,
                missing, missing, missing, missing, missing, missing, missing, missing, missing);
            //Save Excel to Html
            object format = Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml;
            string htmlName = Path.GetFileNameWithoutExtension(physicalPath) + ".html";
            String outputFile = Path.GetDirectoryName(physicalPath) + "\\" + htmlName;
            workbook.SaveAs(outputFile, format, missing, missing, missing,
                              missing, XlSaveAsAccessMode.xlNoChange, missing,
                              missing, missing, missing, missing);
            workbook.Close();
            application.Quit();
            return Path.GetDirectoryName(Server.UrlDecode(url)) + "\\" + htmlName;
        }
        #endregion

        #region 预览Word
        /// <summary>
        /// 预览Word
        /// </summary>
        public string PreviewWord(string physicalPath, string url)
        {
            Microsoft.Office.Interop.Word._Application application = null;
            Microsoft.Office.Interop.Word._Document doc = null;
            application = new Microsoft.Office.Interop.Word.Application();
            object missing = Type.Missing;
            object trueObject = true;
            application.Visible = false;
            application.DisplayAlerts = WdAlertLevel.wdAlertsNone;
            doc = application.Documents.Open(physicalPath, missing, trueObject, missing, missing, missing,
                missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
            //Save Excel to Html
            object format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML;
            string htmlName = Path.GetFileNameWithoutExtension(physicalPath) + ".html";
            String outputFile = Path.GetDirectoryName(physicalPath) + "\\" + htmlName;
            doc.SaveAs(outputFile, format, missing, missing, missing,
                              missing, XlSaveAsAccessMode.xlNoChange, missing,
                              missing, missing, missing, missing);
            doc.Close();
            application.Quit();
            return Path.GetDirectoryName(Server.UrlDecode(url)) + "\\" + htmlName;
        }
        #endregion

        #region 预览Txt
        /// <summary>
        /// 预览Txt
        /// </summary>
        public string PreviewTxt(string physicalPath, string url)
        {
            return Server.UrlDecode(url);
        }
        #endregion

        #region 预览Pdf
        /// <summary>
        /// 预览Pdf
        /// </summary>
        public string PreviewPdf(string physicalPath, string url)
        {
            return Server.UrlDecode(url);
        }
        #endregion

        #region 预览图片
        /// <summary>
        /// 预览图片
        /// </summary>
        public string PreviewImg(string physicalPath, string url)
        {
            return Server.UrlDecode(url);
        }
        #endregion

        #region 预览其他文件
        /// <summary>
        /// 预览其他文件
        /// </summary>
        public string PreviewOther(string physicalPath, string url)
        {
            return Server.UrlDecode(url);
        }
        #endregion
    }
}



