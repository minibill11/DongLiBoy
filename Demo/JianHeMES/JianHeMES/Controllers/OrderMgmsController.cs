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
            //...TODO...
            #region----------订单在组装的统计数据


            //开始时间 

            //最后时间

            //完成时间

            //作业时长

            //直通个数

            //正常个数

            //有效工时

            //异常个数

            //异常工时

            #endregion

            #region----------订单在老化的统计数据


            //开始时间 

            //最后时间

            //完成时间

            //作业时长

            //直通个数

            //正常个数

            //有效工时

            //异常个数

            //异常工时

            #endregion

            #region----------订单在校正的统计数据


            //开始时间 

            //最后时间

            //完成时间

            //作业时长

            //直通个数

            //正常个数

            //有效工时

            //异常个数

            //异常工时

            #endregion

            #region----------订单在外观包装的统计数据


            //开始时间 

            //最后时间

            //完成时间

            //作业时长

            //直通个数

            //正常个数

            //有效工时

            //异常个数

            //异常工时

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

        #region --------------------上传文件方法
        [HttpPost]
        public ActionResult UploadFile(int id, string ordernum)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadfile"];
                //if (file.FileName.Substring(file.FileName.Length - 3, 3).ToLower() == "jpg" || file.FileName.Substring(file.FileName.Length - 3, 3).ToLower() == "png" || file.FileName.Substring(file.FileName.Length - 3, 3).ToLower() == "bmp")
                //{
                //    Image img = Image.FromStream(file.InputStream);
                //    img.RotateFlip(0);
                //    Bitmap bitmap = (Bitmap)Image.FromStream(file.InputStream);
                //}
                var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();

                string ReName = ordernum + "_AOD";

                if (Directory.Exists(@"D:\MES_Data\AOD_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\AOD_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\AOD_Files\" + ordernum + "\\");
                //file.SaveAs(@"D:\MES_Data\AOD_Files\" + ordernum + "\\" + ReName + fileType);

                ////上传是检查是否有重复文件
                //List<FileInfo> jpgfileInfos = fileInfos.Where(c => c.Name.Substring(file.FileName.Length - 4, 4) == ".jpg").ToList();
                List<FileInfo> temp = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AOD") && c.Name.Substring(c.Name.Length - 4, 4) ==".jpg").ToList();
                int count = fileInfos.Where(c => c.Name.StartsWith(ordernum + "_AOD") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                file.SaveAs(@"D:\MES_Data\AOD_Files\" + ordernum + "\\" + ReName + (count + 1) + fileType);
                //string lastfilename = ReName + fileType;
                //bool has = fileInfos.Where(c => c.Name == lastfilename).Count() > 0 ? true : false;
                //if (has)
                //{
                //    file.SaveAs(@"D:\MES_Data\AOD_Files\" + ordernum + "\\" + ReName + "-(1)" + fileType);
                //}
                //else
                //{
                //    file.SaveAs(@"D:\MES_Data\AOD_Files\" + ordernum + "\\" + ReName + fileType);
                //}
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

        #region --------------------获取pdf文件
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

        #region --------------------读取特采订单图片预览

        public ActionResult GetImg(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\";
            filesInfo = GetAllFilesInDirectory(directory);

            //string strPath = "";
            //if (filesInfo.Where(c => c.FullName.Substring(c.FullName.Length - 3, 3) == "png").Count() > 0)
            //{
            //    strPath = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\" + ordernum + "_AOD.png";
            //}
            //if (filesInfo.Where(c => c.FullName.Substring(c.FullName.Length - 3, 3) == "bmp").Count() > 0)
            //{
            //    strPath = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\" + ordernum + "_AOD.bmp";
            //}
            //if (filesInfo.Where(c => c.FullName.Substring(c.FullName.Length - 3, 3) == "jpg").Count() > 0)
            //{
            //    strPath = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\" + ordernum + "_AOD.jpg";
            //}
            //Image img = Image.FromFile(@strPath);
            //MemoryStream ms = new MemoryStream();
            //img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //return File(ms.ToArray(), "image/jpeg");

            filesInfo = filesInfo.Where(c => c.Name.StartsWith(ordernum + "_AOD") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
            JObject json = new JObject();

            int i = 1;
            foreach (var item in filesInfo)
            {
                json.Add(i.ToString(), item.Name);
                i++;
            }
            ViewBag.jpgjson = json;
            
            //foreach (var item in filesInfo)
            //{
            //    json.Add(item.Name, item.FullName);
            //}
            //ViewBag.jpgjson = json;
            return Content(json.ToString());
        }
        #endregion

        #region --------------------查看pdf文档页
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

