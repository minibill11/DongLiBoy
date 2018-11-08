using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using static JianHeMES.Controllers.PdfViewController;

namespace JianHeMES.Controllers
{
    //public class PdfViewController : Controller
    //{
    //    // GET: PdfView
    //    public ActionResult Index()
    //    {
    //        return View();
    //    }
    //}

    public class PdfContentResult : ActionResult
    {
        public PdfContentResult() : this(null, null) { }

        public PdfContentResult(string viewName) : this(null, viewName) { }

        public PdfContentResult(object model) : this(model, null) { }

        public PdfContentResult(object model, string viewName)
        {
            this.ViewName = viewName;
            ViewData = null != model ? new ViewDataDictionary(model) : null;
        }

        public ViewDataDictionary ViewData { get; set; } = new ViewDataDictionary();

        public string ViewName { get; set; }

        public IView View { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (String.IsNullOrEmpty(ViewName))
            {
                ViewName = context.RouteData.GetRequiredString("action");
            }
            if (ViewData == null)
            {
                ViewData = context.Controller.ViewData;
            }
            ViewEngineResult result = ViewEngines.Engines.FindView(context, ViewName, null);
            View = result.View;

            StringBuilder sbHtml = new StringBuilder();
            TextWriter txtWriter = new StringWriter(sbHtml);
            ViewContext viewContext = new ViewContext(context, View, ViewData, context.Controller.TempData, txtWriter);
            result.View.Render(viewContext, txtWriter);

            HttpResponseBase httpResponse = context.HttpContext.Response;
            httpResponse.ContentType = System.Net.Mime.MediaTypeNames.Application.Pdf;

            //加入此头部文件会直接下载pdf文件，而不是在浏览器中预览呈现
            //context.HttpContext.Response.AppendHeader("Content-Disposition", string.Format("attachment;filename={0}.pdf", ViewName));

            HtmlToPdf(sbHtml, httpResponse);

            result.ViewEngine.ReleaseView(context, View);
        }

        private static void HtmlToPdf(StringBuilder sbHtml, HttpResponseBase httpResponse)
        {
            using (Document document = new Document(PageSize.A4, 4, 4, 4, 4))
            {
                using (PdfWriter pdfWriter = PdfWriter.GetInstance(document, httpResponse.OutputStream))
                {
                    document.Open();
                    FontFactory.RegisterDirectories();//注册系统中所支持的字体
                    XMLWorkerHelper worker = XMLWorkerHelper.GetInstance();
                    //UnicodeFontFactory 自定义实现解决itextsharp.xmlworker 不支持中文的问题
                    worker.ParseXHtml(pdfWriter, document, new MemoryStream(Encoding.UTF8.GetBytes(sbHtml.ToString())), null, Encoding.UTF8, new UnicodeFontFactory());
                    document.Close();
                }
            }
        }
    }


    //-----------------------------------------------------------------------------------------------------------------

    public class PdfViewController : Controller
    {
        private readonly HtmlViewRenderer htmlViewRenderer;

        public PdfViewController()
        {
            this.htmlViewRenderer = new HtmlViewRenderer();
        }

        //public string ViewPdf(string viewName, object model)
        //{
        //    // Render the view html to a string.
        //    string htmlText = this.htmlViewRenderer.RenderViewToString(this, viewName, model);
        //    return htmlText;
        //}


        public class UnicodeFontFactory : FontFactoryImp
        {
            private static readonly string arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                "arialuni.ttf");//arial unicode MS是完整的unicode字型。
            private static readonly string MarkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
              "KAIU.TTF");//標楷體


            public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color,
                bool cached)
            {
                //可用Arial或標楷體，自己選一個
                BaseFont baseFont = BaseFont.CreateFont(MarkPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                return new Font(baseFont, size, style, color);
            }
        }


        public class BinaryContentResult : ActionResult
        {
            private readonly string contentType;
            private readonly byte[] contentBytes;

            public BinaryContentResult(byte[] contentBytes, string contentType)
            {
                this.contentBytes = contentBytes;
                this.contentType = contentType;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                var response = context.HttpContext.Response;
                response.Clear();
                response.Cache.SetCacheability(HttpCacheability.Public);
                response.ContentType = this.contentType;
                //下面这段加上就是一个下载页面
                response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("文件名.pdf", System.Text.Encoding.UTF8));
                using (var stream = new MemoryStream(this.contentBytes))
                {
                    stream.WriteTo(response.OutputStream);
                    stream.Flush();
                }
            }
        }

    }


    public class HtmlViewRenderer
    {
        public string RenderViewToString(Controller controller, string viewName, object viewData)
        {
            var renderedView = new StringBuilder();
            using (var responseWriter = new StringWriter(renderedView))
            {
                var fakeResponse = new HttpResponse(responseWriter);
                var fakeContext = new HttpContext(HttpContext.Current.Request, fakeResponse);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(fakeContext), controller.RouteData, controller);
                //问题点
                var oldContext = HttpContext.Current;
                HttpContext.Current = fakeContext;

                using (var viewPage = new ViewPage())
                {
                    var html = new HtmlHelper(CreateViewContext(responseWriter, fakeControllerContext), viewPage);
                    html.RenderPartial(viewName, viewData);
                    HttpContext.Current = oldContext;
                }
            }

            return renderedView.ToString();
        }

        private static ViewContext CreateViewContext(TextWriter responseWriter, ControllerContext fakeControllerContext)
        {
            return new ViewContext(fakeControllerContext, new FakeView(), new ViewDataDictionary(), new TempDataDictionary(), responseWriter);
        }
    }

    public class FakeView : IView
    {

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            throw new NotImplementedException();
        }

    }




}