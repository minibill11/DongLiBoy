using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static JianHeMES.Controllers.PdfViewController;
using Image = iTextSharp.text.Image;

namespace JianHeMES.Controllers
{
    public class PDFHelperController : Controller
    {
        // GET: PDFHelper
        public ActionResult Index()
        {
            return View();
        }
    }


    namespace PDFHelper
    {
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
                htmlText = "<p>" + htmlText + "</p>";
                HTML ht = new HTML();
                MemoryStream outputStream = new MemoryStream();//要把PDF写到哪个串流
                byte[] data = Encoding.UTF8.GetBytes(htmlText);//字串转成byte[]
                MemoryStream msInput = new MemoryStream(data,0,data.Length);
                ////Rectangle rect = new Rectangle(11 * 72, 8.5f * 72);自定义纸张大小
                Document doc = new Document(PageSize._11X17, 20, 0, 20, 0);//要写PDF的文件，建构子没填的话预设直式A4
                PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
;               //指定文件预设开档时的缩放为100%
                PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
                //开启Document文件 
                doc.Open();
                //写入水印图片
                if (!string.IsNullOrEmpty(picPath))
                {
                    Image img = Image.GetInstance(picPath);
                    //设置图片的位置
                    img.SetAbsolutePosition(width + left, (doc.PageSize.Height - height) - top);
                    //设置图片的大小
                    img.ScaleAbsolute(width, height);
                    doc.Add(img);
                    //#region 图片的处理
                    //CssFilesImpl cssFiles = new CssFilesImpl();
                    //cssFiles.Add(XMLWorkerHelper.GetInstance().GetDefaultCSS());
                    //var cssResolver = new StyleAttrCSSResolver(cssFiles);

                    //var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                    //tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
                    //tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor(m_ImageXYDic)); // use new processor

                    //var hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
                    //hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // inject the tagProcessors

                    //var charset = Encoding.UTF8;
                    //var htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(doc, writer));
                    //var pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                    //var worker = new XMLWorker(pipeline, true);
                    //var xmlParser = new XMLParser(true, worker, charset);
                    //xmlParser.Parse(new StringReader(htmlText));
                    //#endregion
                }
                try
                {
                    //使用XMLWorkerHelper把Html parse到PDF档里
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8);
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
                //回传PDF档案 
                return outputStream.ToArray();
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


            #endregion     该注释掉的方法为获取某文件下的HTML代码来生成PDF，因为我是直接取的数据库中存的HTML代码，所以改成了下面的方法。
            /// <summary>
            /// 获取网站内容，包含了 HTML+CSS+JS
            /// </summary>
            /// <returns>String返回网页信息</returns>
            public static string GetWebContent2(string inpath)
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
            public static string GetWebContent(string monthContent)
            {
                try
                {
                    WebClient myWebClient = new WebClient();
                    //获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                    myWebClient.Credentials = CredentialCache.DefaultCredentials;
                    //从指定网站下载数据
                    //Byte[] pageData = myWebClient.DownloadData(inpath);
                    //如果获取网站页面采用的是GB2312，则使用这句
                    string pageHtml = monthContent;
                    //bool isBool = IsMessyCode(pageHtml);//判断使用哪种编码 读取网页信息
                    //if (!isBool)
                    //{
                    //    string pageHtml1 = Encoding.UTF8.GetString(pageData);
                    //    pageHtml = pageHtml1;
                    //}
                    //else
                    //{
                    //    string pageHtml2 = Encoding.Default.GetString(pageData);
                    //    pageHtml = pageHtml2;
                    //}
                    return pageHtml;
                }
                catch (WebException webEx)
                {
                    return webEx.Message;
                }
            }

            /// <summary>
            /// 将pdf文件流输出至浏览器下载
            /// </summary>
            /// <param name="pdfFile">PDF文件流</param>
            public static void PdfDownload(byte[] pdfFile, string title)
            {
                byte[] buffer = pdfFile;
                Stream iStream = new MemoryStream(buffer);
                try
                {
                    int length;
                    long dataToRead;
                    //string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";//保存的文件名称
                    string filename = title + ".pdf";
                    dataToRead = iStream.Length;
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ClearHeaders();
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.ContentType = "application/pdf"; //文件类型 
                    HttpContext.Current.Response.AddHeader("Content-Length", dataToRead.ToString());//添加文件长度，进而显示进度 
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(filename, Encoding.UTF8));
                    while (dataToRead > 0)
                    {
                        if (HttpContext.Current.Response.IsClientConnected)
                        {
                            length = buffer.Length;
                            HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                            HttpContext.Current.Response.Flush();
                            buffer = new Byte[length];
                            dataToRead = dataToRead - length;
                        }
                        else
                        {
                            dataToRead = -1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Response.Write("文件下载时出现错误!"+ex);
                }
                finally
                {
                    if (iStream != null)
                    {
                        iStream.Close();
                    }
                    //结束响应，否则将导致网页内容被输出到文件，进而文件无法打开  
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
        }
    }

}