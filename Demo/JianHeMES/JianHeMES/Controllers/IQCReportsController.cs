using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using Image = iTextSharp.text.Image;
using JianHeMES.Controllers;
using System.Drawing;
using System.Drawing.Imaging;
using Font = iTextSharp.text.Font;
using static JianHeMES.Controllers.PdfViewController;

namespace JianHeMES.Controllers 
{
    public class IQCReportsController : Controller
{
        private ApplicationDbContext db = new ApplicationDbContext();

        #region --------------------物料列表

        private List<SelectListItem> Material()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "请选择物料",
                    Value = ""
                },
                new SelectListItem
                {
                    Text = "三合一贴片灯",
                    Value = "三合一贴片灯"
                },
                new SelectListItem
                {
                    Text = "LED插件灯",
                    Value = "LED插件灯"
                }
            };
        }

        #endregion

        #region --------------------检验水平等级

        private List<SelectListItem> level()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "请选择",
                    Value = ""
                },
                new SelectListItem
                {
                    Text = "一般Ⅱ级",
                    Value = "一般Ⅱ级"
                },
                new SelectListItem
                {
                    Text = "一般Ⅲ级",
                    Value = "一般Ⅲ级"
                }
            };
        }

        #endregion

        #region --------------------首页
        // GET: IQCReports
        public async Task<ActionResult> Index()
        {
            return View(await db.IQCReports.ToListAsync());
        }

        #endregion

        #region --------------------Details页
        // GET: IQCReports/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IQCReport iQCReport = await db.IQCReports.FindAsync(id);
            if (iQCReport == null)
            {
                return HttpNotFound();
            }
            return View(iQCReport);
        }
        #endregion

        #region --------------------IQCReportCreate页
        // GET: IQCReports/Create
        public ActionResult IQCReportCreate()
        {
            ViewBag.Material = Material();
            ViewBag.Level = level();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "IQC检验员" || ((Users)Session["User"]).Role == "系统管理员")
            {
                return View();
            }
            return View();
        }

        // POST: IQCReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IQCReportCreate([Bind(Include = "Id,Material_SN,RoHS_REACH,OrderNumber,EquipmentNum,Provider,MaterialName,ModelNumber,Specification,MaterialQuantity,IncomingDate,ApplyPurchaseOrderNum,BatchNum,InspectionDate,SamplingPlan,MaterialVersion,C1,C2,C3,C4,C5,C6,C7,C8,C9,D1,D2,D3,D4,D5,D6,D7,D8,D9,E1,E2,E3,E4,E5,E6,E7,E8,E9,F1,F2,F3,F4,F5,F6,F7,F8,F9,S0,S1,S11,S12,S13,S14,S15,S2,S21,S22,S23,S24,S25,S3,S31,S32,S33,S34,S35,SR,SRJson,SG,SGJson,SB,SBJson,R0,R1,R11,R12,R13,R2,R21,R22,R23,R3,R31,R32,R33,P0,P1,P11,P12,P13,P2,P21,P22,P23,P3,P31,P32,P33,AM,AM0,AM1,AM11,AM12,AM13,AM2,AM21,AM22,AM23,AM3,AM31,AM32,AM33,AM4,AM41,AM42,AM43,NG1,NG2,NG3,NGD,NGHandle,ReportRemark,Inspector,Creator,CreatedDate,Assessor,AssessedDate,AssessorRemark,AssessorPass,Approve,ApprovedDate,ApproveRemark,ApprovePass")] IQCReport iQCReport)
        {
            ViewBag.Material = Material();
            ViewBag.Level = level();
            iQCReport.Creator = ((Users)Session["User"]).UserName;
            iQCReport.CreatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.IQCReports.Add(iQCReport);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(iQCReport);
        }
        #endregion

        #region --------------------IQCReportEdit页
        // GET: IQCReports/Edit/5
        public async Task<ActionResult> IQCReportEdit(int? id)
        {
            ViewBag.Material = Material();
            ViewBag.Level = level();
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "IQC检验员" || ((Users)Session["User"]).Role == "系统管理员")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                IQCReport iQCReport = await db.IQCReports.FindAsync(id);
                if (iQCReport == null)
                {
                    return HttpNotFound();
                }
                return View(iQCReport);
            }
            return RedirectToAction("Index", "IQCReports");
        }

        // POST: IQCReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IQCReportEdit([Bind(Include = "Id,Material_SN,RoHS_REACH,OrderNumber,EquipmentNum,Provider,MaterialName,ModelNumber,Specification,MaterialQuantity,IncomingDate,ApplyPurchaseOrderNum,BatchNum,InspectionDate,SamplingPlan,MaterialVersion,C1,C2,C3,C4,C5,C6,C7,C8,C9,D1,D2,D3,D4,D5,D6,D7,D8,D9,E1,E2,E3,E4,E5,E6,E7,E8,E9,F1,F2,F3,F4,F5,F6,F7,F8,F9,S0,S1,S11,S12,S13,S14,S15,S2,S21,S22,S23,S24,S25,S3,S31,S32,S33,S34,S35,SR,SRJson,SG,SGJson,SB,SBJson,R0,R1,R11,R12,R13,R2,R21,R22,R23,R3,R31,R32,R33,P0,P1,P11,P12,P13,P2,P21,P22,P23,P3,P31,P32,P33,AM,AM0,AM1,AM11,AM12,AM13,AM2,AM21,AM22,AM23,AM3,AM31,AM32,AM33,AM4,AM41,AM42,AM43,NG1,NG2,NG3,NGD,NGHandle,ReportRemark,Inspector,Creator,CreatedDate,Assessor,AssessedDate,AssessorRemark,AssessorPass,Approve,ApprovedDate,ApproveRemark,ApprovePass")] IQCReport iQCReport)
        {
            ViewBag.Material = Material();
            ViewBag.Level = level();
            if (ModelState.IsValid)
            {
                db.Entry(iQCReport).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(iQCReport);
        }
        #endregion

        #region --------------------IQCReportAssessor页
        public async Task<ActionResult> IQCReportAssessor(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "IQC组长" || ((Users)Session["User"]).Role == "系统管理员")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                IQCReport iQCReport = await db.IQCReports.FindAsync(id);
                if (iQCReport == null)
                {
                    return HttpNotFound();
                }
                return View(iQCReport);
            }
            return RedirectToAction("Index", "IQCReports");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IQCReportAssessor([Bind(Include = "Id,Material_SN,RoHS_REACH,OrderNumber,EquipmentNum,Provider,MaterialName,ModelNumber,Specification,MaterialQuantity,IncomingDate,ApplyPurchaseOrderNum,BatchNum,InspectionDate,SamplingPlan,MaterialVersion,C1,C2,C3,C4,C5,C6,C7,C8,C9,D1,D2,D3,D4,D5,D6,D7,D8,D9,E1,E2,E3,E4,E5,E6,E7,E8,E9,F1,F2,F3,F4,F5,F6,F7,F8,F9,S0,S1,S11,S12,S13,S14,S15,S2,S21,S22,S23,S24,S25,S3,S31,S32,S33,S34,S35,SR,SRJson,SG,SGJson,SB,SBJson,R0,R1,R11,R12,R13,R2,R21,R22,R23,R3,R31,R32,R33,P0,P1,P11,P12,P13,P2,P21,P22,P23,P3,P31,P32,P33,AM,AM0,AM1,AM11,AM12,AM13,AM2,AM21,AM22,AM23,AM3,AM31,AM32,AM33,AM4,AM41,AM42,AM43,NG1,NG2,NG3,NGD,NGHandle,ReportRemark,Inspector,Creator,CreatedDate,Assessor,AssessedDate,AssessorRemark,AssessorPass,Approve,ApprovedDate,ApproveRemark,ApprovePass")] IQCReport iQCReport)
        {
            iQCReport.Assessor = ((Users)Session["User"]).UserName;
            iQCReport.AssessedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(iQCReport).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(iQCReport);
        }
        #endregion

        #region --------------------IQCReportApprove页
        public async Task<ActionResult> IQCReportApprove(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "品质部经理" || ((Users)Session["User"]).Role == "系统管理员")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                IQCReport iQCReport = await db.IQCReports.FindAsync(id);
                if (iQCReport == null)
                {
                    return HttpNotFound();
                }
                return View(iQCReport);
            }
            return RedirectToAction("Index", "IQCReports");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IQCReportApprove([Bind(Include = "Id,Material_SN,RoHS_REACH,OrderNumber,EquipmentNum,Provider,MaterialName,ModelNumber,Specification,MaterialQuantity,IncomingDate,ApplyPurchaseOrderNum,BatchNum,InspectionDate,SamplingPlan,MaterialVersion,C1,C2,C3,C4,C5,C6,C7,C8,C9,D1,D2,D3,D4,D5,D6,D7,D8,D9,E1,E2,E3,E4,E5,E6,E7,E8,E9,F1,F2,F3,F4,F5,F6,F7,F8,F9,S0,S1,S11,S12,S13,S14,S15,S2,S21,S22,S23,S24,S25,S3,S31,S32,S33,S34,S35,SR,SRJson,SG,SGJson,SB,SBJson,R0,R1,R11,R12,R13,R2,R21,R22,R23,R3,R31,R32,R33,P0,P1,P11,P12,P13,P2,P21,P22,P23,P3,P31,P32,P33,AM,AM0,AM1,AM11,AM12,AM13,AM2,AM21,AM22,AM23,AM3,AM31,AM32,AM33,AM4,AM41,AM42,AM43,NG1,NG2,NG3,NGD,NGHandle,ReportRemark,Inspector,Creator,CreatedDate,Assessor,AssessedDate,AssessorRemark,AssessorPass,Approve,ApprovedDate,ApproveRemark,ApprovePass")] IQCReport iQCReport)
        {
            iQCReport.Approve = ((Users)Session["User"]).UserName;
            iQCReport.ApprovedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(iQCReport).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(iQCReport);
        }
        #endregion

        #region --------------------Delete页
        // GET: IQCReports/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (((Users)Session["User"]).Role == "系统管理员")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                IQCReport iQCReport = await db.IQCReports.FindAsync(id);
                if (iQCReport == null)
                {
                    return HttpNotFound();
                }
                return View(iQCReport);
            }
            return RedirectToAction("Index", "IQCReports");
        }

        // POST: IQCReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            IQCReport iQCReport = await db.IQCReports.FindAsync(id);
            db.IQCReports.Remove(iQCReport);
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
        #endregion









        #region----------------测试中

        //[HttpPost]
        //public FileResult DownloadPdf(string pdfData)
        //{
        //    //从网址下载Html字符串(方法一)
        //    //string inpath = System.Web.HttpContext.Current.Server.MapPath("~/PDFTemplate/test.html");
        //    //string htmlText = GetWebContent(inpath);//此处调用步骤4方法

        //    //获取MVC视图Html字符串(方法二)
        //    //string htmlText = GetViewHtml(ControllerContext, "Details");//此处调用步骤5方法
        //    string htmlText = pdfData.Replace("&lt", "<").Replace("&gt", ">");

        //    //string htmlText = "<table style=\"border: 1px solid red\"><colgroup><col width=\"80\" /><col width=\"80\" /></colgroup><thead><tr><th>序号</th><th>测试</th></tr></thead><tbody><tr><td>1</td><td>test</td></tr></tbody><tfoot>页脚</tfoot></table>";
        //    //htmlText = htmlText.Replace("\\\\", "");

        //    ////水印图片路径
        //    //string picPath = Server.MapPath("~/Images/LOGO.jpg");


        //    //html转pdf并加上水印
        //    //byte[] pdfFile = HtmlToPdfHelper.ConvertHtmlTextToPdf(htmlText, picPath, 100, 200, 100, 100);
        //    //byte[] pdfFile = HtmlToPdfHelper.ConvertHtmlTextToPdf(htmlText);
        //    byte[] pdfFile = ConvertHtmlTextToPDF(htmlText);
        //    //输出至客户端
        //    PdfDownload(pdfFile);//此处调用步骤6方法
        //    return File(pdfFile, "application/pdf", "test.pdf");
        //}


        private static bool IsMessyCode(string txt)
        {
            var bytes = Encoding.UTF8.GetBytes(txt);
            for (var i = 0; i < bytes.Length; i++)
            {
                if (i < bytes.Length - 3)
                    if (bytes[i] == 239 && bytes[i + 1] == 191 && bytes[i + 2] == 189)
                    {
                        return true;
                    }
            }
            return false;
        }

        /// <summary>
        /// Html字符串转PDF输出帮助类
        /// </summary>
        public class HtmlToPdfHelper
        {
            #region HtmlToPDF

            /// <summary>
            /// 判断是否有乱码
            /// </summary>
            /// <param name="txt"></param>
            /// <returns></returns>
            private static bool IsMessyCode(string txt)
            {
                var bytes = Encoding.UTF8.GetBytes(txt);
                for (var i = 0; i < bytes.Length; i++)
                {
                    if (i < bytes.Length - 3)
                        if (bytes[i] == 239 && bytes[i + 1] == 191 && bytes[i + 2] == 189)
                        {
                            return true;
                        }
                }
                return false;
            }

            /// <summary>
            /// 将Html字符串 输出到PDF档里
            /// </summary>
            /// <param name="htmlText"></param>
            /// <returns></returns>
            public static byte[] ConvertHtmlTextToPdf(string htmlText)
            {
                return ConvertHtmlTextToPdf(htmlText, "", 0, 0, 0, 0);
            }

            /// <summary>
            /// 将Html字符串 输出到PDF档里,并添加水印
            /// </summary>
            /// <param name="htmlText">网页代码</param>
            /// <param name="picPath">水印路径</param>
            /// <param name="left">距离左边距离</param>
            /// <param name="top">距顶部距离</param>
            /// <param name="width">水印宽度</param>
            /// <param name="height">水印高度</param>
            /// <returns></returns>
            public static byte[] ConvertHtmlTextToPdf(string htmlText, string picPath, int left, int top, int width, int height)
            {
                if (string.IsNullOrEmpty(htmlText))
                {
                    return null;
                }
                //避免当htmlText无任何html tag标签的纯文字时，转PDF时会挂掉，所以一律加上<p>标签
                htmlText = "<html><style>body{font-family:pingfang sc light;}</style><body><p>" + htmlText + "</p></body></html>";
                //string htmlString = "<html><style>body{font-family:pingfang sc light;}</style><body>第一页1p开始</body></html>";
                MemoryStream outputStream = new MemoryStream();//要把PDF写到哪个串流
                byte[] data = Encoding.UTF8.GetBytes(htmlText);//字串转成byte[]
                MemoryStream msInput = new MemoryStream(data);
                Document doc = new Document();//要写PDF的文件，建构子没填的话预设直式A4
                PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
                //指定文件预设开档时的缩放为100%
                PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
                //开启Document文件 
                doc.Open();
                writer.Open();

                //写入水印图片
                if (!string.IsNullOrEmpty(picPath))
                {
                    Image img = Image.GetInstance(picPath);
                    //设置图片的位置
                    img.SetAbsolutePosition(width + left, (doc.PageSize.Height - height) - top);
                    //设置图片的大小
                    img.ScaleAbsolute(width, height);
                    doc.Add(img);
                }
                try
                {
                    //使用XMLWorkerHelper把Html parse到PDF档里
                    //..TODO..问题在这里，writer和doc数据不可用。
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8);
                    //XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, new MemoryStream(Encoding.UTF8.GetBytes(htmlString)), null, Encoding.UTF8);
                    //将pdfDest设定的资料写到PDF档
                    PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
                    writer.SetOpenAction(action);
                }
                catch (Exception)
                {
                    return null;
                }
                doc.Close();
                msInput.Close();
                outputStream.Close();
                writer.Close();

                //回传PDF档案 
                return outputStream.ToArray();
            }

            #endregion

        }


        /// <summary>
        /// 获取网站内容，包含了 HTML+CSS+JS
        /// </summary>
        /// <returns>String返回网页信息</returns>
        public static string GetWebContent(string inpath)
        {
            try
            {
                WebClient myWebClient = new WebClient();
                //获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                myWebClient.Credentials = CredentialCache.DefaultCredentials;
                //从指定网站下载数据
                Byte[] pageData = myWebClient.DownloadData(inpath);
                //如果获取网站页面采用的是GB2312，则使用这句
                string pageHtml = Encoding.UTF8.GetString(pageData);
                bool isBool = IsMessyCode(pageHtml);//判断使用哪种编码 读取网页信息
                if (!isBool)
                {
                    string pageHtml1 = Encoding.UTF8.GetString(pageData);
                    pageHtml = pageHtml1;
                }
                else
                {
                    string pageHtml2 = Encoding.Default.GetString(pageData);
                    pageHtml = pageHtml2;
                }
                return pageHtml;
            }
            catch (WebException webEx)
            {
                return webEx.Message;
            }
        }


        /// <summary>
        /// 获取MVC视图Html
        /// </summary>
        /// <param name="context">控制器上下文</param>
        /// <param name="viewName">视图名称</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetViewHtml(ControllerContext context, string viewName)
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
                catch (Exception)
                {
                    throw;
                }

                return sw.GetStringBuilder().ToString();
            }
        }


        /// <summary>
        /// 将pdf文件流输出至浏览器下载
        /// </summary>
        /// <param name="pdfFile">PDF文件流</param>
        public static void PdfDownload(byte[] pdfFile)
        {
            byte[] buffer = pdfFile;
            Stream iStream = new MemoryStream(buffer);
            try
            {
                int length;
                long dataToRead;
                string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "IQC报告.pdf";//保存的文件名称
                dataToRead = iStream.Length;
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.ClearContent();
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf"; //文件类型 
                System.Web.HttpContext.Current.Response.AddHeader("Content-Length", dataToRead.ToString());//添加文件长度，进而显示进度 
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(filename, Encoding.UTF8));
                while (dataToRead > 0)
                {
                    if (System.Web.HttpContext.Current.Response.IsClientConnected)
                    {
                        length = buffer.Length;
                        System.Web.HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        System.Web.HttpContext.Current.Response.Flush();
                        buffer = new Byte[length];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception)
            {
                System.Web.HttpContext.Current.Response.Write("文件下载时出现错误!");
            }
            finally
            {
                if (iStream != null)
                {
                    iStream.Close();
                }
                //结束响应，否则将导致网页内容被输出到文件，进而文件无法打开  
                System.Web.HttpContext.Current.Response.Flush();
                System.Web.HttpContext.Current.Response.End();

            }
        }



        #endregion


        //-------------------------------------------------------------
        #region----------可以生成pdf，不能导入html内容


        //[HttpPost]
        //public FileResult DownloadPdf(string pdfData = "")
        //{
        //    pdfData = pdfData.Replace("&lt", "<").Replace("&gt", ">").Replace("\r\n", "");
        //    #region 针对IE浏览器js获取的html内容会丢失"的问题，添加转义字符\"
        //    //if (pdfData.IndexOf("color=red") > 0)
        //    //{
        //    //    pdfData = pdfData.Replace("color=red", "color=\"red\"");
        //    //}
        //    //if (pdfData.IndexOf("color=#000") > 0)
        //    //{
        //    //    pdfData = pdfData.Replace("color=#000", "color=\"#000\"");
        //    //}
        //    #endregion
        //    byte[] pdf = ConvertHtmlTextToPDF(pdfData);
        //    return File(pdf, "application/pdf", "test.pdf");
        //}




        public class UnicodeFontFactory : FontFactoryImp
        {
            private static readonly string arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                "arialuni.ttf");//arial unicode MS是完整的unicode字型。
            private static readonly string kaiuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                "KAIU.TTF");//标楷体
            private static readonly string simsunPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                "simsun.ttc,1");//新宋体
            private static readonly string msyhPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                "msyh.ttf");//微软雅黑

            public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color,
                bool cached)
            {
                //采用微软雅黑
                BaseFont baseFont = BaseFont.CreateFont(msyhPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                return new Font(baseFont, size, style, color);
            }
        }

        /// <summary>
        /// 将Html文字 输出到PDF档里
        /// </summary>
        /// <param name="htmlText"></param>
        /// <returns></returns>

        public byte[] ConvertHtmlTextToPDF(string htmlText)  //测试中
        {
            if (string.IsNullOrEmpty(htmlText))
            {
                return null;
            }
            //避免当htmlText无任何html tag标签的纯文字时，转PDF时会挂掉，所以一律加上<p>标签
            htmlText = "<html><p>" + htmlText + "</p></html>";
            //htmlText = "<html><style>body{font-family:pingfang sc light;}</style><body>第一页1p开始</body></html>";
            //htmlText = "<table><thead><tr><th>序号</th><th>测试</th></tr></thead><tbody><tr><td>1</td><td>test</td></tr></tbody><tfoot>页脚</tfoot></table>";
            //htmlText = "<table style=\"border: 1px solid red\"><colgroup><col width=\"80\" /><col width=\"80\" /></colgroup><thead><tr><th>序号</th><th>测试</th></tr></thead><tbody><tr><td>1</td><td>test</td></tr></tbody><tfoot>页脚</tfoot></table>";
            //htmlText = "<table>< colgroup >< col width = \"80\" />< col width = \"80\" /></ colgroup >< thead >< tr >< th > 序号 </ th >< th > 测试 </ th ></ tr ></ thead >< tbody >< tr >< td > 1 </ td >< td > test </ td ></ tr ></ tbody >< tfoot >页脚</ tfoot ></ table > ";
            MemoryStream outputStream = new MemoryStream();//要把PDF写到哪个串流
            byte[] data = Encoding.Default.GetBytes(htmlText);//字串转成byte[]
            //byte[] data = Encoding.UTF8.GetBytes(htmlText);//字串转成byte[]
            MemoryStream msInput = new MemoryStream(data);
            Document doc = new Document();//要写PDF的文件，建构子没填的话预设直式A4
            PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
            //指定文件预设开档时的缩放为100%
            PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
            //开启Document文件 
            doc.Open();
            writer.Open();

            //#region pdf文件添加LOGO
            //string logoPath = Server.MapPath("/Images/LOGO.jpg");
            //Image logo = Image.GetInstance(logoPath);
            //float scalePercent = 10f;       //图片缩放比例
            //float percentX = 60f;
            //float percentY = 250f;
            //logo.ScalePercent(scalePercent);
            //logo.SetAbsolutePosition(percentX, doc.PageSize.Height - percentY);
            //doc.Add(logo);
            //#endregion

            //使用XMLWorkerHelper把Html parse到PDF档里
            //msInput  MemoryStream出现ReadTimeOut和WriteTimeOut
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.Default, new UnicodeFontFactory());
            //将pdfDest设定的资料写到PDF档
            PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
            writer.SetOpenAction(action);
            doc.Close();
            writer.Close();
            //msInput.Close();
            outputStream.Close();
            //回传PDF文档 
            return outputStream.ToArray();
        }
        #endregion




        //--------------------------------------------------------------------------------------


        //public FileResult DownloadPdf()
        //{
        //    string pdfData = "";
        //    pdfData = pdfData.Replace("&lt", "<").Replace("&gt", ">").Replace("\r\n", "");
        //    #region 针对IE浏览器js获取的html内容会丢失"的问题，添加转义字符\"
        //    //if (pdfData.IndexOf("color=red") > 0)
        //    //{
        //    //    pdfData = pdfData.Replace("color=red", "color=\"red\"");
        //    //}
        //    //if (pdfData.IndexOf("color=#000") > 0)
        //    //{
        //    //    pdfData = pdfData.Replace("color=#000", "color=\"#000\"");
        //    //} 
        //    #endregion
        //    byte[] pdf = ConvertHtmlTextToPDF(pdfData);
        //    return File(pdf, "application/pdf", "test.pdf");
        //}


        [HttpPost]
        //public FileResult BaishiMianDan(string pdfData = "")
        public ActionResult DownloadPdf(string pdfData = "")
        {
            pdfData = pdfData.Replace("&lt", "<").Replace("&gt", ">").Replace("\r\n", "");
            #region 针对IE浏览器js获取的html内容会丢失"的问题，添加转义字符\"
            //if (pdfData.IndexOf("color=red") > 0)
            //{
            //    pdfData = pdfData.Replace("color=red", "color=\"red\"");
            //}
            //if (pdfData.IndexOf("color=#000") > 0)
            //{
            //    pdfData = pdfData.Replace("color=#000", "color=\"#000\"");
            //} 
            #endregion
            byte[] pdf = ConvertHtmlTextToPDF(pdfData);
            //return File(pdf, "application/pdf", "test.pdf");
            return new BinaryContentResult(pdf, "application/pdf");
        }


        ///// <summary>
        ///// 将Html文字 输出到PDF档里
        ///// </summary>
        ///// <param name="htmlText"></param>
        ///// <returns></returns>
        //public byte[] ConvertHtmlTextToPDF(string htmlText)
        //{
        //    if (string.IsNullOrEmpty(htmlText))
        //    {
        //        return null;
        //    }
        //        //避免当htmlText无任何html tag标签的纯文字时，转PDF时会挂掉，所以一律加上<p>标签
        //    htmlText = "<p>" + htmlText + "</p>";
        //    MemoryStream outputStream = new MemoryStream();//要把PDF写到哪个串流
        //    byte[] data = Encoding.Default.GetBytes(htmlText);//字串转成byte[]
        //    //    byte[] data = Encoding.UTF8.GetBytes(htmlText);//字串转成byte[]
        //    MemoryStream msInput = new MemoryStream(data);
        //    Document doc = new Document();
        //    doc.SetMargins(0, 0, 0, 0);     //设置内容边距
        //    PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
        //    //指定文件预设开档时的缩放为100%
        //    PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
        //    //开启Document文件 
        //    doc.Open();
        //    writer.Open();
        //    #region pdf文件添加LOGO
        //    string logoPath = Server.MapPath("/Images/LOGO.jpg");
        //    Image logo = Image.GetInstance(logoPath);
        //    float scalePercent = 10f;       //图片缩放比例
        //    float percentX = 60f;
        //    float percentY = 250f;
        //    logo.ScalePercent(scalePercent);
        //    logo.SetAbsolutePosition(percentX, doc.PageSize.Height - percentY);
        //    doc.Add(logo);
        //    #endregion
        //    //使用XMLWorkerHelper把Html parse到PDF档里
        //    XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.Default, new UnicodeFontFactory());
        //    //将pdfDest设定的资料写到PDF档
        //    PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
        //    writer.SetOpenAction(action);
        //    //doc.Close();
        //    //writer.Close();
        //    //msInput.Close();
        //    outputStream.Close();
        //    //回传PDF档案 
        //    return outputStream.ToArray();
        //}



        //--------------------------------------------------------------------------------------------

        //public class People
        //{
        //    public string Name;

        //    public People(string name)
        //    {
        //        Name = name;
        //    }
        //}



        private readonly HtmlViewRenderer htmlViewRenderer;

        //public PdfViewController()
        //{
        //    this.htmlViewRenderer = new HtmlViewRenderer();
        //}

        public string ViewPdf(string viewName, object model)
        {
            // Render the view html to a string.
            string htmlText = this.htmlViewRenderer.RenderViewToString(this, viewName, model);
            return htmlText;
        }


        ///// <summary>
        ///// 执行此Url，下載PDF文档
        ///// </summary>
        ///// <returns></returns>
        ///// 
        //[HttpPost]
        //public ActionResult DownloadPdf(string htmlText)
        //{
        //    //string htmlText = "中华人民共和国";
        //    //var person = new People(html);
        //    //PdfViewController pvc = new PdfViewController();
        //    string html = ViewPdf("IQCReport", htmlText);
        //    byte[] pdfFile = ConvertHtmlTextToPDF(htmlText);
        //    return new BinaryContentResult(pdfFile, "application/pdf");
        //}

        private static ViewContext CreateViewContext(TextWriter responseWriter, ControllerContext fakeControllerContext)
        {
            return new ViewContext(fakeControllerContext, new FakeView(), new ViewDataDictionary(), new TempDataDictionary(), responseWriter);
        }

    }






    /// <summary>
    /// HTML转PDF帮助类
    /// </summary>
    public class HtmlToPdfHelper
    {
        /// <summary>
        /// 准备好的html字符串
        /// </summary>
        private string m_HtmlString;

        /// <summary>
        /// PDF保存目录（绝对路径）
        /// </summary>
        private string m_PDFSaveFloder;

        /// <summary>
        /// 图片XY字典，例如 [{img1, 100,200},{img2,20,30}}
        /// </summary>
        private Dictionary<string, Tuple<float, float>> m_ImageXYDic = null;

        public HtmlToPdfHelper(string htmlString, string pdfSaveFloder, Dictionary<string, Tuple<float, float>> imageXYDic)
        {
            m_HtmlString = htmlString;
            m_PDFSaveFloder = pdfSaveFloder;
            m_ImageXYDic = imageXYDic;
        }

        //生成PDF
        public bool BuilderPDF()
        {
            try
            {
                string pdfSavePath = Path.Combine(m_PDFSaveFloder, Guid.NewGuid().ToString() + ".pdf");
                if (!Directory.Exists(m_PDFSaveFloder))
                {
                    Directory.CreateDirectory(m_PDFSaveFloder);
                }
                using (FileStream fs = new FileStream(pdfSavePath, FileMode.OpenOrCreate))
                {
                    byte[] htmlByte = ConvertHtmlTextToPDF(m_HtmlString);
                    fs.Write(htmlByte, 0, htmlByte.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("保存PDF到磁盘时异常", ex);
            }
        }

        //将html字符串转为字节数组（代码来自百度）
        public byte[] ConvertHtmlTextToPDF(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
            {
                return null;
            }
            //避免當htmlText無任何html tag標籤的純文字時，轉PDF時會掛掉，所以一律加上<p>標籤
            //htmlText = "<p>" + htmlText + "</p>";

            try
            {
                MemoryStream outputStream = new MemoryStream(); //要把PDF寫到哪個串流
                byte[] data = Encoding.UTF8.GetBytes(htmlText); //字串轉成byte[]
                MemoryStream msInput = new MemoryStream(data);
                Document doc = new Document(); //要寫PDF的文件，建構子沒填的話預設直式A4
                PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);

                //指定文件預設開檔時的縮放為100%
                PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
                //開啟Document文件 
                doc.Open();

                #region 图片的处理
                CssFilesImpl cssFiles = new CssFilesImpl();
                cssFiles.Add(XMLWorkerHelper.GetInstance().GetDefaultCSS());
                var cssResolver = new StyleAttrCSSResolver(cssFiles);

                var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
                tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor(m_ImageXYDic)); // use new processor

                var hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
                hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // inject the tagProcessors

                var charset = Encoding.UTF8;
                var htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(doc, writer));
                var pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                var worker = new XMLWorker(pipeline, true);
                var xmlParser = new XMLParser(true, worker, charset);
                xmlParser.Parse(new StringReader(htmlText));
                #endregion

                //使用XMLWorkerHelper把Html parse到PDF檔裡
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.Default, new UnicodeFontFactory());

                //將pdfDest設定的資料寫到PDF檔
                PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
                writer.SetOpenAction(action);

                doc.Close();
                msInput.Close();
                outputStream.Close();

                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("转PDF时异常", ex);
            }
        }

        //字体工厂（代码来自百度）
        public class UnicodeFontFactory : FontFactoryImp
        {
            private static readonly string arialFontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory
                , "Content/arialuni.ttf");//arial unicode MS是完整的unicode字型。

            public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color,
                bool cached)
            {
                BaseFont baseFont = BaseFont.CreateFont(arialFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                return new Font(baseFont, size, style, color);
            }
        }

        //自定义的图片处理类（代码来自百度）
        public class CustomImageTagProcessor : iTextSharp.tool.xml.html.Image
        {
            //个人加入的图片位置处理代码
            private float _offsetX = 0;
            private float _offsetY = 0;

            private Dictionary<string, Tuple<float, float>> _imageXYDict;//个人加入的图片位置处理代码
            public CustomImageTagProcessor(Dictionary<string, Tuple<float, float>> imageXYDict)//个人加入的图片位置处理代码
            {
                _imageXYDict = imageXYDict;
            }

            protected void SetImageXY(string imageId)//个人加入的图片位置处理代码
            {
                if (_imageXYDict == null)
                {
                    return;
                }
                Tuple<float, float> xyTuple = null;
                _imageXYDict.TryGetValue(imageId, out xyTuple);

                if (xyTuple != null)
                {
                    _offsetX = xyTuple.Item1;
                    _offsetY = xyTuple.Item2;
                }
            }

            public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
            {
                IDictionary<string, string> attributes = tag.Attributes;
                string src;
                if (!attributes.TryGetValue(HTML.Attribute.SRC, out src))
                    return new List<IElement>(1);

                if (string.IsNullOrEmpty(src))
                    return new List<IElement>(1);

                string imageId;//个人加入的图片位置处理代码
                if (!attributes.TryGetValue(HTML.Attribute.ID, out imageId))//个人加入的图片位置处理代码
                    return new List<IElement>(1);

                if (string.IsNullOrEmpty(imageId))
                    return new List<IElement>(1);

                SetImageXY(imageId);//个人加入的图片位置处理代码

                if (src.StartsWith("data:image/", StringComparison.InvariantCultureIgnoreCase))
                {
                    // data:[][;charset=][;base64],
                    var base64Data = src.Substring(src.IndexOf(",") + 1);
                    var imagedata = Convert.FromBase64String(base64Data);
                    var image = iTextSharp.text.Image.GetInstance(imagedata);

                    var list = new List<IElement>();
                    var htmlPipelineContext = GetHtmlPipelineContext(ctx);
                    list.Add(GetCssAppliers().Apply(new Chunk((iTextSharp.text.Image)GetCssAppliers().Apply(image, tag, htmlPipelineContext), _offsetX, _offsetY, true), tag, htmlPipelineContext));
                    return list;
                }
                else
                {
                    return base.End(ctx, tag, currentContent);
                }
            }
        }
    }

}

namespace ClassLibrary1
{
    /// <summary>
    /// 测试类
    /// </summary>
    public class PersonEntity
    {
        /// <summary>
        /// html模版绝对路径
        /// </summary>
        public string m_HtmlTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts/person.html");

        /// <summary>
        /// PDF生成的目录（绝对路径）
        /// </summary>
        public string m_PdfSaveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PDFFolder");

        public PersonEntity()
        {
        }

        /// <summary>
        /// 生成PDF
        /// </summary>
        public void BuildPDF()
        {

            using (StreamReader reader = new StreamReader(m_HtmlTemplatePath))
            {
                string htmlStr = reader.ReadToEnd();//读取html模版

                string iamgeBase64Str1 = ImageToBase64String(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts/img1.jpg"));
                string iamgeBase64Str2 = ImageToBase64String(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts/img2.jpg"));

                htmlStr = htmlStr.Replace("@PersonName", "张三");
                htmlStr = htmlStr.Replace("@PersonImage1", iamgeBase64Str1);
                htmlStr = htmlStr.Replace("@PersonImage2", iamgeBase64Str2);

                Dictionary<string, Tuple<float, float>> imageXYDic = new Dictionary<string, Tuple<float, float>>();
                imageXYDic.Add("img1", new Tuple<float, float>(10, 20));
                imageXYDic.Add("img2", new Tuple<float, float>(200, 300));

                HtmlToPdfHelper pdfHelper = new HtmlToPdfHelper(htmlStr, m_PdfSaveFolder, imageXYDic);

                pdfHelper.BuilderPDF();//生成PDF
            }
        }

        //图片转为base64字符串
        public string ImageToBase64String(string imagePath)
        {
            try
            {
                Bitmap bitmap = new Bitmap(imagePath);

                MemoryStream ms = new MemoryStream();

                bitmap.Save(ms, ImageFormat.Jpeg);
                byte[] bytes = new byte[ms.Length];

                ms.Position = 0;
                ms.Read(bytes, 0, (int)ms.Length);
                ms.Close();

                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("图片转base64字符串时异常", ex);
            }
        }
    }
}

