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


            return View(orderMgm);
        }
        #endregion

        #region --------------获取pdf文件

        //public ActionResult GetPDF(string ordernum)
        //{
        //    //ordernum = "特采订单.pdf";
        //    List<FileInfo> filesInfo = new List<FileInfo>();
        //    string directory = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\";
        //    filesInfo = GetAllFilesInDirectory(directory);
        //    List<string> pdf_address = new List<string>();
        //    foreach(var item in filesInfo)
        //    {
        //        if (item.FullName.Substring(item.FullName.Length-3,3)=="pdf")
        //        {
        //            pdf_address.Add(item.FullName);
        //        }
        //    }
        //    //var path = @"D:\MES_Data\AOD_Files\"+ordernum+"\\";
        //    //var file = Path.Combine(path,ordernum);
        //    //file = Path.GetFullPath(file);
        //    var path = @pdf_address.FirstOrDefault();
        //    var file = Path.Combine(path);
        //    file = Path.GetFullPath(file);
        //    if (!file.StartsWith(path))
        //    {
        //        // someone tried to be smart and sent 
        //        // ?filename=..\..\creditcard.pdf as parameter
        //        throw new HttpException(403, "Forbidden");
        //    }
        //    //return File(file, "application/pdf");
        //    return File(file, "application/pdf", ordernum+"_AOD.pdf");
        //    //return Json(pdf_address, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetPDF(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\";
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            foreach (var item in filesInfo)
            {
                if (item.FullName.Substring(item.FullName.Length - 3, 3) == "pdf")
                {
                    pdf_address.Add(item.FullName);
                }
            }
            var path = @pdf_address.FirstOrDefault();
            if(path==null)
            {
                return Content("<script>alert('此特采单pdf版文件尚未上传，无pdf文件可下载！');window.location.href='..';</script>");
            }
            return File(path, "application/pdf", ordernum + "_AOD.pdf");
        }


        [HttpPost]
        public static void PriviewPDF(System.Web.UI.Page p, string inFilePath)

        {


            p.Response.ContentType = "Application/pdf";

            string fileName = inFilePath.Substring(inFilePath.LastIndexOf('\\') + 1);

            p.Response.AddHeader("content-disposition", "filename=" + fileName);

            inFilePath = @"D:\MES_Data\AOD_Files\特采订单.pdf";

            p.Response.WriteFile(inFilePath);

            p.Response.End();

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


        #region --------------读取特采订单jpeg图片预览

        public ActionResult GetImg(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\";
            filesInfo = GetAllFilesInDirectory(directory);
            string strPath = "";
            if (filesInfo.Where(c => c.FullName.Substring(c.FullName.Length - 3, 3) == "png").Count() > 0)
            {
                strPath = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\" + ordernum + "_AOD.png";
            }
            if (filesInfo.Where(c => c.FullName.Substring(c.FullName.Length - 3, 3) == "bmp").Count() > 0)
            {
                strPath = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\" + ordernum + "_AOD.bmp";
            }
            if (filesInfo.Where(c => c.FullName.Substring(c.FullName.Length - 3, 3) == "jpg").Count() > 0)
            {
                strPath = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\" + ordernum + "_AOD.jpg";
            }
            Image img = Image.FromFile(@strPath);
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return File(ms.ToArray(), "image/jpeg");
        }

        #endregion


        #region --------------------上传文件方法

        [HttpPost]
        public ActionResult UploadFile(int id, string ordernum)
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["uploadfile"];
                var fileName = file.FileName;
                var fileType = fileName.Substring(fileName.Length - 4, 4);
                string ReName = ordernum + "_AOD";

                #region ----------各部门的文件
                //switch (((Users)Session["User"]).Department)
                //{
                //    case "PC部":
                //        ReName = ReName + "_PC";
                //        break;
                //    case "技术部":
                //        ReName = ReName + "_TN";
                //        break;
                //    case "装配一部":
                //        ReName = ReName + "_AS";
                //        break;
                //    case "品质部":
                //        ReName = ReName + "_QA";
                //        break;
                //}

                #endregion

                if (Directory.Exists(@"D:\MES_Data\AOD_Files\" + ordernum + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\AOD_Files\" + ordernum + "\\");
                }
                List<FileInfo> fileInfos = GetAllFilesInDirectory(@"D:\MES_Data\AOD_Files\" + ordernum + "\\");
                file.SaveAs(@"D:\MES_Data\AOD_Files\" + ordernum + "\\" + ReName + fileType);

                ////上传是检查是否有重复文件
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

                //List<string> types = new List<string>();
                //List<string> addreses = new List<string>();
                //types.Add(fileType);
                //orderMgm.AOD_FilesType = types;
                //string add = "D:\\MES_Data\\AOD_Files\\" + ordernum + "\\" + ReName + fileType;
                //addreses.Add(add);
                //orderMgm.AOD_FilesAddress = addreses;

                if (ModelState.IsValid)
                {
                    db.Entry(orderMgm).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "OrderMgms", new { id = orderMgm.ID });
                }
                return View(orderMgm);
            }
            return View();

            //string result = "success";
            //HttpPostedFileBase imageName = Request.Files["image"];// 从前台获取文件
            //string file = imageName.FileName;
            //string fileFormat = file.Split('.')[file.Split('.').Length - 1]; // 以“.”截取，获取“.”后面的文件后缀
            //Regex imageFormat = new Regex(@"^(bmp)|(png)|(gif)|(jpg)|(jpeg)"); // 验证文件后缀的表达式（自己写的，不规范别介意哈）
            //if (string.IsNullOrEmpty(file) || !imageFormat.IsMatch(fileFormat)) // 验证后缀，判断文件是否是所要上传的格式
            //{
            //    result = "error";
            //}
            //else
            //{
            //    string timeStamp = DateTime.Now.Ticks.ToString(); // 获取当前时间的string类型
            //    string firstFileName = timeStamp.Substring(0, timeStamp.Length - 4); // 通过截取获得文件名
            //    string imageStr = "Images/StudentImage/"; // 获取保存图片的项目文件夹
            //    string uploadPath = Server.MapPath("~/" + imageStr); // 将项目路径与文件夹合并
            //    string pictureFormat = file.Split('.')[file.Split('.').Length - 1];// 设置文件格式
            //    string fileName = firstFileName + "." + fileFormat;// 设置完整（文件名+文件格式） 
            //    string saveFile = uploadPath + fileName;//文件路径
            //    imageName.SaveAs(saveFile);// 保存文件
            //    // 如果单单是上传，不用保存路径的话，下面这行代码就不需要写了！
            //    string image = imageStr + fileName;// 设置数据库保存的路径

            //    result = "success";
            //}


            //Directory.Delete(Server.MapPath("~/upimg/hufu"), true);//删除文件夹以及文件夹中的子目录文件

            ////判断文件的存在
            //if (File.Exists(Server.MapPath("~/upimg/Data.html")))
            //{
            //    Response.Write("Yes");
            //    //存在文件
            //}
            //else
            //{
            //    Response.Write("No");
            //    //不存在文件
            //    File.Create(MapPath("~/upimg/Data.html"));//创建该文件
            //}
            //string name = GetFiles.FileName;//获取已上传文件的名字
            //string size = GetFiles.PostedFile.ContentLength.ToString();//获取已上传文件的大小
            //string type = GetFiles.PostedFile.ContentType;//获取已上传文件的MIME
            //string postfix = name.Substring(name.LastIndexOf(".") + 1);//获取已上传文件的后缀
            //string ipath = Server.MapPath("upimg") + "\\" + name;//获取文件的实际路径
            //string fpath = Server.MapPath("upfile") + "\\" + name;
            //string dpath = "upimg\\" + name;//判断写入数据库的虚拟路径
            //ShowPic.Visible = true;//激活
            //ShowText.Visible = true;//激活

            ////判断文件格式
            //if (name == "")
            //{
            //    Response.Write("<script>alert('上传文件不能为空')</script>");
            //}
            //else
            //{
            //    if (postfix == "jpg" || postfix == "gif" || postfix == "bmp" || postfix == "png")
            //    {
            //        GetFiles.SaveAs(ipath);
            //        ShowPic.ImageUrl = dpath;
            //        ShowText.Text = "你上传的图片名称是:" + name + "<br>" + "文件大小: " + size + "KB" + " < br > " + "文件类型: " + type + " < br > " + "存放的实际路径为: " + ipath;
            //    }
            //    else
            //    {
            //        ShowPic.Visible = false;//隐藏图片
            //        GetFiles.SaveAs(fpath);//由于不是图片文件,因此转存在upfile这个文件夹
            //        ShowText.Text = "你上传的文件名称是:" + name + "<br>" + "文件大小: " + size + "KB" + " < br > " + "文件类型: " + type + " < br > " + "存放的实际路径为: " + fpath;
            //    }
            //}
        }

        #endregion

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


    }
}

