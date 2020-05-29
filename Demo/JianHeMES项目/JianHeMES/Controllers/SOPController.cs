﻿using iTextSharp.text.pdf;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rotativa.MVC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class SOPController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult SOP_Query()//查询PDF页
        {
            return View();
        }

        #region---客户端读取共享目录
        /// <summary>  
        /// 返回指定目录下文件夹信息  
        /// </summary>  
        /// <param name="folder">A 工艺流程/B 作业指导书\\盖受控章文件  目录字符串</param>  value=A/B
        /// <param name="subdirectory">目录字符串</param>  
        /// <param name="filename">文件名</param>  
        /// <returns></returns>  
        [HttpPost]
        public ActionResult Index(string folder, string subdirectory,string filename)   
        {
            //folder = folder == "B" ? "B 作业指导书\\盖受控章文件" : "A 工艺流程\\盖受控章文件";
            folder = folder == "B" ? "B 作业指导书" : "A 工艺流程";
            bool status = false;
            string ipaddress = "\\\\172.16.2.9\\";
            //string folderpath = "技术部\\01 工艺文件汇总\\00 技术部工艺文件电子档存放(对外)\\三阶文件\\";
            string folderpath = "质量管理体系文件\\20 工艺文件\\";

            //连接共享文件夹
            status = connectState(@"\\172.16.2.9\技术部", "15043", "h15043");
            if (status)
            {
                DirectoryInfo theFolder = new DirectoryInfo(ipaddress + folderpath + folder);
                //读取文件夹清单
                DirectoryInfo[] theFolderlist = GetFolderInDirectory(ipaddress + folderpath + folder);
                ////设选项为第一个文件夹(已排队不使用)
                //subdirectory = theFolderlist[0].Name;
                //读取文件清单
                var theFileslist = GetFilesInDirectory(ipaddress + folderpath + folder + "\\"+ subdirectory);

                if (filename == "") filename = theFileslist[0].Name;
                if (theFileslist.Count>0)
                {
                    string linkaddress = ConnectGetFile(ipaddress, folderpath, folder, subdirectory, filename, "15043", "h15043");
                    return Content(linkaddress);
                }
                else
                {
                    return Content("目录内没有PDF文件！");
                }
            }
            else
            {
                return Content("未能连接！");
            }
        }

        public static bool connectState(string path,string userName, string passWord)
        {
            bool Flag = false;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use " + path + " /user:" + userName + " " + passWord /*+ " /PERSISTENT:YES"*/;
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
                else
                {
                    throw new Exception(errormsg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }

        #region --------------------返回"A 工艺流程"目录下文件夹信息
        /// <summary>  
        /// 返回指定目录下文件夹信息  
        /// </summary>  
        /// <param name="folder">A 工艺流程/B 作业指导书\\盖受控章文件  目录字符串</param>  
        /// <returns></returns>  
        [HttpPost]
        public ActionResult GetShareFolderInDirectory(string folder = "")
        {
            //folder = folder == "B" ? "B 作业指导书\\盖受控章文件" : "A 工艺流程\\盖受控章文件";
            folder = folder == "B" ? "B 作业指导书" : "A 工艺流程";
            bool status = false;
            string ipaddress = "\\\\172.16.2.9\\";
            //string folderpath = "技术部\\01 工艺文件汇总\\00 技术部工艺文件电子档存放(对外)\\三阶文件\\";
            string folderpath = "质量管理体系文件\\20 工艺文件\\";
            status = connectState(@"\\172.16.2.9\技术部", "15043", "h15043");
            DirectoryInfo[] directoryArray = null;
            if (status)
            {
                DirectoryInfo directory = new DirectoryInfo(ipaddress + folderpath + folder);
                directoryArray = directory.GetDirectories();
            }
            return Content(JsonConvert.SerializeObject(directoryArray));
        }
        #endregion

        #region --------------------返回指定目录下文件信息
            /// <summary>  
            /// 返回指定目录下文件信息  
            /// </summary>  
            /// <param name="strDirectory">目录字符串</param>  
            /// <returns></returns>  
        [HttpPost]
        public ActionResult GetShareFilesInDirectory(string folder, string subdirectory)
        {
            //folder = folder == "B" ? "B 作业指导书\\盖受控章文件" : "A 工艺流程\\盖受控章文件";
            folder = folder == "B" ? "B 作业指导书" : "A 工艺流程";
            bool status = false;
            string ipaddress = "\\\\172.16.2.9\\";
            //string folderpath = "技术部\\01 工艺文件汇总\\00 技术部工艺文件电子档存放(对外)\\三阶文件\\" +folder+"\\"+ subdirectory;
            string folderpath = "质量管理体系文件\\20 工艺文件\\" + folder + "\\" + subdirectory;
            status = connectState(@"\\172.16.2.9\技术部", "15043", "h15043");
            FileInfo[] fileInfoArray = null;
            if (status)
            {
                DirectoryInfo directory = new DirectoryInfo(ipaddress + folderpath);
                fileInfoArray = directory.GetFiles().Where(c => c.Extension == ".pdf").ToArray();
                return Content(JsonConvert.SerializeObject(fileInfoArray));
            }
            else
            {
                return Content("未能连接！");
            }
        }
        #endregion

        public static string ConnectGetFile(string ipaddress, string folderpath, string folder, string subdirectory, string filename, string userName, string passWord)
        {
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use " + ipaddress + folderpath + "\\" + folder + " /user:" + userName + " " + passWord + " /PERSISTENT:YES";
                proc.StandardInput.WriteLine(dosLine);
                if(!Directory.Exists("D:\\MES_Data\\" + folderpath + "\\" +folder + "\\" + subdirectory))
                {
                    Directory.CreateDirectory("D:\\MES_Data\\" + folderpath + "\\" + folder + "\\" + subdirectory);
                }
                if(!System.IO.File.Exists("D:\\MES_Data\\" + folderpath + "\\" + folder + "\\" + subdirectory + "\\" + filename))
                {
                    System.IO.File.Copy(ipaddress + folderpath + "\\" + folder + "\\" + subdirectory + "\\" + filename, "D:\\MES_Data\\" + folderpath + folder + "\\" + subdirectory + "\\" + filename, true);
                }
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                //string errormsg = proc.StandardError.ReadToEnd();
                //proc.StandardError.Close();
                //if (string.IsNullOrEmpty(errormsg))
                //{
                //    Flag = true;
                //}
                //else
                //{
                //    throw new Exception(errormsg);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            string resultaddress = "/MES_Data/" + folderpath.Replace('\\','/') + folder + "/"+ subdirectory + "/"+filename;
            return resultaddress;
        }

        #endregion

        #region --------------------GetOrderNumList_SOP()检索订单号
        ///-------------------<GetOrderNumList_SOP_summary>
        /// 1.方法的作用：检索订单号
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到OrderMgm表里按照ID的排序顺序查询所有订单号并去重。
        /// 4.方法（可能）有结果：输出查询数据。
        ///-------------------</GetOrderNumList_SOP_summary>
        public ActionResult GetOrderNumList_SOP()
        {
            var ordernum = db.OrderMgm.OrderByDescending(m => m.ID).Select(m => m.OrderNum).Distinct();//按照ID的排序顺序查询所有订单号并去重
            return Content(JsonConvert.SerializeObject(ordernum));
        }
        #endregion

        #region----上传SOP PDF文档 A_工艺流程图
        ///-------------------<UploadFile_SOP_summary>
        /// 1.方法的作用：上传SOP PDF文档 A_工艺流程图
        /// 2.方法的参数和用法：platform平台，statss工段名，flowchart文件名，ordernumber订单号，用于保存数据
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断请求文件是否大于0。（2）获取上传内容和截取文件名的后缀。（3）判断订单号是否为空。（4）第三步为空时，判断该文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\工段名）是否等于false（有没有该文件夹）。
        /// （5）第四步等于false时，创建文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\工段名）。（6）判断文件类型是否等于PDF，类型相同时，把文件保存到对应的文件夹下。
        /// （7）第三步不为空时（订单号不为空），判断该文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\订单号\工段名）是否等于false（有没有该文件夹）。（8）第七步等于false时，创建文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\订单号\工段名）。
        /// （9）判断文件类型是否等于PDF，类型相同时，把文件保存到对应的文件夹下。（10）判断订单号，平台，工段名，文件名都不能为空，符合要求时，往soplist里添加订单号，平台，工段名，文件名数据，并把数据保存到对应的数据表和保存到数据库。
        /// （11）判断订单号为空；平台，工段名，文件名都不能为空，符合要求时，往soplist里添加订单号，平台，工段名，文件名数据，并把数据保存到对应的数据表和保存到数据库。
        /// 4.方法（可能）有结果：第一步等于0，上传失败；第一步大于0时，上传成功。
        ///-------------------</UploadFile_SOP_summary>
        public bool UploadFile_SOP(string platform, string statss, string flowchart, string ordernumber)
        {
            if (Request.Files.Count > 0)//判断请求文件是否大于0
            {
                HttpPostedFileBase file = Request.Files["UploadFile_SOP"];//获取上传内容
                var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();//截取文件名的后缀
                if (ordernumber == "null")//判断订单号是否为空
                {
                    //判断该文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\工段名）是否等于false（有没有该文件夹）
                    if (Directory.Exists(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + platform.ToString() + "\\" + statss + "\\") == false)//如果不存在就创建订单文件夹
                    {
                        //创建文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\工段名）
                        Directory.CreateDirectory(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + platform.ToString() + "\\" + statss + "\\");
                    }
                    if (fileType == ".pdf")//判断文件类型是否等于PDF
                    {
                        //把文件保存到对应的文件夹下
                        file.SaveAs(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + platform.ToString() + "\\" + statss + "\\" + flowchart);
                    }
                    else //这段代码可以不要（没有运行）

                    {
                        List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + platform.ToString() + "\\" + statss + "\\");
                        int pdf_count = fileInfos.Where(c => c.Name.StartsWith(platform.ToString()) && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + platform.ToString() + "\\" + statss + "\\" + (pdf_count + 1) + fileType);
                    }
                }
                else //订单号不为空
                {
                    //判断该文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\订单号\工段名）是否等于false（有没有该文件夹）
                    if (Directory.Exists(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + platform.ToString() + "\\" + ordernumber + "\\" + statss + "\\") == false)//如果不存在就创建订单文件夹
                    {
                        //创建文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\订单号\工段名）
                        Directory.CreateDirectory(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + platform.ToString() + "\\" + ordernumber + "\\" + statss + "\\");
                    }
                    if (fileType == ".pdf")//判断文件类型是否等于PDF
                    {
                        //把文件保存到对应的文件夹下
                        file.SaveAs(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + platform.ToString() + "\\" + ordernumber + "\\" + statss + "\\" + flowchart);
                    }
                    else//这段代码可以不要（没有运行）
                    {
                        List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + platform.ToString() + "\\" + ordernumber + "\\" + statss + "\\");
                        int pdf_count = fileInfos.Where(c => c.Name.StartsWith(platform.ToString()) && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + platform.ToString() + "\\" + ordernumber + "\\" + statss + "\\" + flowchart + (pdf_count + 1) + fileType);
                    }
                }
                //判断订单号，平台，工段名，文件名都不能为空
                if (ordernumber != "null" && !String.IsNullOrEmpty(platform) && !String.IsNullOrEmpty(flowchart) && !String.IsNullOrEmpty(statss))
                {
                    //往soplist里添加订单号，平台，工段名，文件名数据
                    SOPoperating soplist = new SOPoperating() { OrderNum = ordernumber, Platform = platform, Processflow_chart = flowchart, SectionName = statss };
                    db.SOPoperating.Add(soplist);//把数据保存到对应的数据表
                    db.SaveChanges();//保存到数据库
                }
                //判断订单号为空；平台，工段名，文件名都不能为空
                if (!String.IsNullOrEmpty(platform) && !String.IsNullOrEmpty(flowchart) && !String.IsNullOrEmpty(statss) && ordernumber == "null")
                {
                    //往soplist里添加订单号，平台，工段名，文件名数据
                    SOPoperating soplist1 = new SOPoperating() { Platform = platform, Processflow_chart = flowchart, SectionName = statss };
                    db.SOPoperating.Add(soplist1);//把数据保存到对应的数据表
                    db.SaveChanges();//保存到数据库
                }
                return true;
            }
            return false;
        }

        #endregion

        #region----上传SOP PDF文档 B_作业指导书
        ///-------------------<UploadFileSOP_B_summary>
        /// 1.方法的作用：上传SOP PDF文档 B_作业指导书
        /// 2.方法的参数和用法：platform平台，statss工段名，flowchart文件名，ordernumber订单号，用于保存数据
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断请求文件是否大于0。（2）获取上传内容和截取文件名的后缀。（3）判断订单号是否为空。（4）第三步为空时，判断该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\工段名）是否等于false（有没有该文件夹）。
        /// （5）第四步等于false时，创建文件夹（技术部\工艺文件汇总\B_作业指导书\平台\工段名）。（6）判断文件类型是否等于PDF，类型相同时，把文件保存到对应的文件夹下。
        /// （7）第三步不为空时（订单号不为空），判断该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\订单号\工段名）是否等于false（有没有该文件夹）。（8）第七步等于false时，创建文件夹（技术部\工艺文件汇总\B_作业指导书\平台\订单号\工段名）。
        /// （9）判断文件类型是否等于PDF，类型相同时，把文件保存到对应的文件夹下。（10）判断订单号，平台，工段名，文件名都不能为空，符合要求时，往soplist里添加订单号，平台，工段名，文件名数据，并把数据保存到对应的数据表和保存到数据库。
        /// （11）判断订单号为空；平台，工段名，文件名都不能为空，符合要求时，往soplist里添加订单号，平台，工段名，文件名数据，并把数据保存到对应的数据表和保存到数据库。
        /// 4.方法（可能）有结果：第一步等于0，上传失败；第一步大于0时，上传成功。
        ///-------------------</UploadFileSOP_B_summary>
        public bool UploadFileSOP_B(string platform, string statss, string flowchart, string ordernumber)
        {
            if (Request.Files.Count > 0)//判断请求文件是否大于0
            {
                HttpPostedFileBase file = Request.Files["UploadFileSOP_B"];//获取上传内容
                var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();//截取文件名的后缀
                if (ordernumber == "null")//判断订单号是否为空
                {
                    //判断该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\工段名）是否等于false（有没有该文件夹）
                    if (Directory.Exists(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + platform.ToString() + "\\" + statss + "\\") == false)//如果不存在就创建订单文件夹
                    {
                        //创建文件夹（技术部\工艺文件汇总\B_作业指导书\平台\工段名）
                        Directory.CreateDirectory(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + platform.ToString() + "\\" + statss + "\\");
                    }
                    if (fileType == ".pdf")//判断文件类型是否等于PDF
                    {
                        //把文件保存到对应的文件夹下
                        file.SaveAs(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + platform.ToString() + "\\" + statss + "\\" + flowchart);
                    }
                    else //这段代码可以不要（没有运行）
                    {
                        List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + platform.ToString() + "\\" + statss + "\\");
                        int pdf_count = fileInfos.Where(c => c.Name.StartsWith(platform.ToString()) && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + platform.ToString() + "\\" + statss + "\\" + (pdf_count + 1) + fileType);
                    }
                }
                else //订单号不为空
                {
                    //判断该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\订单号\工段名）是否等于false（有没有该文件夹）
                    if (Directory.Exists(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + platform.ToString() + "\\" + ordernumber + "\\" + statss + "\\") == false)//如果不存在就创建订单文件夹
                    {
                        //创建文件夹（技术部\工艺文件汇总\B_作业指导书\平台\订单号\工段名）
                        Directory.CreateDirectory(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + platform.ToString() + "\\" + ordernumber + "\\" + statss + "\\");
                    }
                    if (fileType == ".pdf")//判断文件类型是否等于PDF
                    {
                        //把文件保存到对应的文件夹下
                        file.SaveAs(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + platform.ToString() + "\\" + ordernumber + "\\" + statss + "\\" + flowchart);
                    }
                    else //这段代码可以不要（没有运行）
                    {
                        List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + platform.ToString() + "\\" + ordernumber + "\\" + statss + "\\");
                        int pdf_count = fileInfos.Where(c => c.Name.StartsWith(platform.ToString()) && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + platform.ToString() + "\\" + ordernumber + "\\" + statss + "\\" + flowchart + (pdf_count + 1) + fileType);
                    }
                }
                //判断订单号，平台，工段名，文件名都不能为空
                if (ordernumber != "null" && !String.IsNullOrEmpty(platform) && !String.IsNullOrEmpty(flowchart) && !String.IsNullOrEmpty(statss))
                {
                    //往soplist里添加订单号，平台，工段名，文件名数据
                    SOPoperating soplist = new SOPoperating() { OrderNum = ordernumber, Platform = platform, SOPreference_document = flowchart, SectionName = statss };
                    db.SOPoperating.Add(soplist);//把数据保存到对应的数据表
                    db.SaveChanges();//保存到数据库
                }
                //判断订单号等于空;平台，工段名，文件名都不能为空
                if (!String.IsNullOrEmpty(platform) && !String.IsNullOrEmpty(flowchart) && !String.IsNullOrEmpty(statss) && ordernumber == "null")
                {
                    //往soplist里添加订单号，平台，工段名，文件名数据
                    SOPoperating soplist1 = new SOPoperating() { Platform = platform, SOPreference_document = flowchart, SectionName = statss };
                    db.SOPoperating.Add(soplist1);//把数据保存到对应的数据表
                    db.SaveChanges();//保存到数据库
                }
                return true;
            }
            return false;
        }

        #endregion

        #region -----查看SOP PDF文档 
        ///-------------------<Previewpdf_SOP_summary>
        /// 1.方法的作用：查看SOP PDF文档 
        /// 2.方法的参数和用法：platform平台，statss工段名，flowchart文件名，ordernumber订单号，用于查询数据
        /// 3.方法的具体逻辑顺序，判断条件：（1）把数据表(SOPoperating)的数据全部拿出来。（2）判断pintai（查询数据）是否大于0。（3）第二步大于0时，判断平台是否为空，不为空时根据平台查询数据。
        /// （3.1）判断工段名是否为空，不为空时，根据工段名查询数据。（3.3）判断订单号是否为空，不为空时，根据订单号查询数据。（3.4）判断文件名是否为空，不为空时，根据文件名查询数据（文件名有：SOP参照文件、工艺流程图编号）。
        /// （4）循环pintai。（5）判断订单号和文件名（工艺流程图编号）是否为空，不为空时，调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\订单号\工段名）和查找文件名并截取文件名的后缀。
        /// （5.1）判断filesInfo是否为空，不为空时，把数据赋值到plat里（数据：文件地址），并把ID、平台、订单号、工段名、文件名、文件地址add到flow里，把flow add到result里，并把flow初始化。
        /// （6）判断订单号和文件名（SOP参照文件）是否为空，不为空时，调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\订单号\工段名）和查找文件名并截取文件名的后缀。
        /// （6.1）判断filesInfo是否为空，不为空时，把数据赋值到plat1里（数据：文件地址），并把并把ID、平台、订单号、工段名、文件名、文件地址add到flow里，把flow add到result里，并把flow初始化。
        /// （7）判断订单号是否等于空，文件名（工艺流程图编号）是否为空，订单号等于null和文件名不等于null时，调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\工段名）和查找文件名并截取文件名的后缀。
        /// （7.1）判断filesInfo是否为空，不为空时，把数据赋值到plat里（数据：文件地址），并把ID、平台、订单号、工段名、文件名、文件地址add到flow里，把flow add到result里，并把flow初始化。
        /// （8）判断订单号是否等于空，和文件名（SOP参照文件）是否为空，订单号等于null和文件名不等于null时，调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\工段名）和查找文件名并截取文件名的后缀。
        /// （8.1）判断filesInfo是否为空，不为空时，把数据赋值到plat1里（数据：文件地址），并把ID、平台、订单号、工段名、文件名、文件地址add到flow里，把flow add到result里，并把flow初始化。
        /// 4.方法（可能）有结果：查询失败；查询成功
        ///-------------------</Previewpdf_SOP_summary>
        public ActionResult Previewpdf_SOP(string platform, string flowchart, string ordernumber, string statss)
        {
            JArray result = new JArray();
            JObject flow = new JObject();
            JObject trform = new JObject();
            var pintai = db.SOPoperating.ToList();//把数据表的数据全部拿出来
            if (pintai.Count > 0)//判断pintai（查询数据）是否大于0
            {
                if (!String.IsNullOrEmpty(platform))//判断平台是否为空
                {
                    pintai = pintai.Where(c => c.Platform == platform).ToList();//根据平台查询数据
                }
                if (!String.IsNullOrEmpty(statss))//判断工段名是否为空
                {
                    pintai = pintai.Where(c => c.SectionName == statss).ToList();//根据工段名查询数据
                }
                if (!String.IsNullOrEmpty(ordernumber))//判断订单号是否为空
                {
                    pintai = pintai.Where(c => c.OrderNum == ordernumber).ToList();//根据订单号查询数据
                }
                if (!String.IsNullOrEmpty(flowchart))//判断文件名是否为空
                {
                    pintai = pintai.Where(c => c.Processflow_chart == flowchart || c.SOPreference_document == flowchart).ToList();//根据文件名查询数据（文件名有：SOP参照文件、工艺流程图编号）
                }
                foreach (var item in pintai)//循环
                {
                    if (item.OrderNum != null && item.Processflow_chart != null)//判断订单号和文件名（工艺流程图编号）是否为空
                    {
                        //调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\订单号\工段名）
                        List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + item.Platform + "\\" + item.OrderNum + "\\" + item.SectionName + "\\");
                        filesInfo = filesInfo.Where(c => c.Name.StartsWith(item.Processflow_chart) && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();//查找文件名并截取文件名的后缀
                        if (filesInfo.Count() > 0)//判断filesInfo是否为空
                        {
                            //把数据赋值到plat里（数据：文件地址）
                            string plat = @"/MES_Data/技术部/工艺文件汇总/A_工艺流程图/" + item.Platform + "/" + item.OrderNum + "/" + item.SectionName + "/" + item.Processflow_chart;
                            flow.Add("PDFType", "A_工艺流程图");
                            flow.Add("id", item.Id);//把ID add到flow里
                            flow.Add("Platform", item.Platform);//平台
                            flow.Add("OrderNum", item.OrderNum);//订单号
                            flow.Add("SectionName", item.SectionName);//工段名
                            flow.Add("Pilename", item.Processflow_chart);//文件名
                            flow.Add("Plat", plat);//文件地址
                            result.Add(flow);//把flow add到result里
                            flow = new JObject();//初始化
                        }
                    }
                    if (item.OrderNum != null && item.SOPreference_document != null)//判断订单号和文件名（SOP参照文件）是否为空
                    {
                        //调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\订单号\工段名）
                        List<FileInfo> filesInfo1 = comm.GetAllFilesInDirectory(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + item.Platform + "\\" + item.OrderNum + "\\" + item.SectionName + "\\");
                        filesInfo1 = filesInfo1.Where(c => c.Name.StartsWith(item.SOPreference_document) && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();//查找文件名并截取文件名的后缀
                        if (filesInfo1.Count() > 0) //判断filesInfo是否为空
                        {
                            //把数据赋值到plat1里（数据：文件地址）
                            string plat1 = @"/MES_Data/技术部/工艺文件汇总/B_作业指导书/" + item.Platform + "/" + item.OrderNum + "/" + item.SectionName + "/" + item.SOPreference_document;
                            trform.Add("PDFType", "B_作业指导书");
                            trform.Add("id", item.Id);//把ID add到flow里
                            trform.Add("Platform", item.Platform);//平台
                            trform.Add("OrderNum", item.OrderNum);//订单号
                            trform.Add("SectionName", item.SectionName);//工段名
                            trform.Add("Pilename", item.SOPreference_document);//文件名
                            trform.Add("Plat", plat1);//文件地址
                            result.Add(trform);//把flow add到result里
                            trform = new JObject();//初始化
                        }
                    }
                    if (item.OrderNum == null && item.Processflow_chart != null)//判断订单号是否等于空，文件名（工艺流程图编号）是否为空
                    {
                        //调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\工段名）
                        List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + item.Platform + "\\" + item.SectionName + "\\");
                        filesInfo = filesInfo.Where(c => c.Name.StartsWith(item.Processflow_chart) && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();//查找文件名并截取文件名的后缀
                        if (filesInfo.Count() > 0)//判断filesInfo是否为空
                        {
                            //把数据赋值到plat里（数据：文件地址）
                            string plat = @"/MES_Data/技术部/工艺文件汇总/A_工艺流程图/" + item.Platform + "/" + item.SectionName + "/" + item.Processflow_chart;
                            flow.Add("PDFType", "A_工艺流程图");
                            flow.Add("id", item.Id);//把ID add到flow里
                            flow.Add("Platform", item.Platform);//平台
                            flow.Add("SectionName", item.SectionName);//工段名
                            flow.Add("Pilename", item.Processflow_chart);//文件名
                            flow.Add("Plat", plat);//文件地址
                            result.Add(flow);//把flow add到result里
                            flow = new JObject();//初始化
                        }
                    }
                    if (item.OrderNum == null && item.SOPreference_document != null)//判断订单号是否等于空，和文件名（SOP参照文件）是否为空
                    {
                        //调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\工段名）
                        List<FileInfo> filesInfo1 = comm.GetAllFilesInDirectory(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + item.Platform + "\\" + item.SectionName + "\\");
                        filesInfo1 = filesInfo1.Where(c => c.Name.StartsWith(item.SOPreference_document) && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();//查找文件名并截取文件名的后缀
                        if (filesInfo1.Count() > 0)//判断filesInfo是否为空
                        {
                            //把数据赋值到plat1里（数据：文件地址）
                            string plat1 = @"/MES_Data/技术部/工艺文件汇总/B_作业指导书/" + item.Platform + "/" + item.SectionName + "/" + item.SOPreference_document;
                            trform.Add("PDFType", "B_作业指导书");
                            trform.Add("id", item.Id);//把ID add到flow里
                            trform.Add("Platform", item.Platform);//平台
                            trform.Add("SectionName", item.SectionName);//工段名
                            trform.Add("Pilename", item.SOPreference_document);//文件名
                            trform.Add("Plat", plat1);//文件地址
                            result.Add(trform);//把flow add到result里
                            trform = new JObject();//初始化
                        }
                    }
                }
                return Content(JsonConvert.SerializeObject(result));
            }
            return Content("false");
        }
        #endregion

        #region---更新SOP文档
        ///-------------------<UpdateSOP_document_summary>
        /// 1.方法的作用：更新SOP文档
        /// 2.方法的参数和用法：OrignFile（旧文件地址），folder（文件夹），platform平台，statss工段名，flowchart文件名，ordernumber订单号，ID
        /// 3.方法的具体逻辑顺序，判断条件：(1)以最后一个斜杠截取路径（只保留文件名）。(2)判断该文件夹（技术部\工艺文件汇总\folder(可能是：B_作业指导书或者是A_工艺流程图)\已替换文件）是否等于false（有没有该文件夹）。
        /// （2.1）第二步等于false时，创建文件夹（技术部\工艺文件汇总\folder(可能是：B_作业指导书或者是A_工艺流程图)\已替换文件）。（3）把路径（（技术部\工艺文件汇总\folder(可能是：B_作业指导书或者是A_工艺流程图)\已替换文件））赋值给NewFile。
        /// （4）把旧文件路径的斜杠（/）替换成这个斜杠（\）。(5)把旧文件移动到NewFile这个文件夹（技术部\工艺文件汇总\folder(可能是：B_作业指导书或者是A_工艺流程图)\已替换文件）下面。
        /// （6）判断文件夹是否等于B_作业指导书，等于时，判断调用的方法是否有这些参数（平台，工段名，文件名，订单号），有这些参数时，运行这个UploadFileSOP_B方法（上传SOP PDF文档 B_作业指导书）。
        /// （6.1）判断ID是否不等于0，不等于0时，根据ID到SOPoperating表里查询数据。（6.2）添加删除操作时间、添加删除操作人和添加操作记录（如：张三在2020年2月27日移除SOP名为文件名（如：组装.pdf）的文件记录）。
        /// （6.3）删除对应的数据（6.1步）。（6.4）添加删除操作日记数据（6.2步）。（6.5）保存到数据库。（7）判断文件夹是否等于A_工艺流程图，等于时，判断调用的方法是否有这些参数（平台，工段名，文件名，订单号），有这些参数时，运行这个UploadFile_SOP方法（上传SOP PDF文档 A_工艺流程图）。
        /// （7.1）判断ID是否不等于0，不等于0时，根据ID到SOPoperating表里查询数据。（7.2）添加删除操作时间、添加删除操作人和添加操作记录（如：张三在2020年2月27日移除SOP名为文件名（如：组装.pdf）的文件记录）。
        /// （7.3）删除对应的数据（7.1步）。（7.4）添加删除操作日记数据（7.2步）。（7.5）保存到数据库。
        /// 4.方法（可能）有结果：输出更新结果（有：true、false）。
        ///-------------------</UpdateSOP_document_summary>
        public bool UpdateSOP_document(string OrignFile, string folder, string platform, string statss, string flowchart, string ordernumber, int id)
        {
            bool result = true;
            var filename = OrignFile.Substring(OrignFile.LastIndexOf('/') + 1);//以最后一个斜杠截取路径（只保留文件名）
            //判断该文件夹（技术部\工艺文件汇总\folder(可能是：B_作业指导书或者是A_工艺流程图)\已替换文件）是否等于false（有没有该文件夹）
            if (Directory.Exists(@"D:\MES_Data\技术部\工艺文件汇总\" + folder + "\\已替换文件\\") == false)//如果不存在就创建订单文件夹
            {
                //创建文件夹（技术部\工艺文件汇总\folder(可能是：B_作业指导书或者是A_工艺流程图)\已替换文件）
                Directory.CreateDirectory(@"D:\MES_Data\技术部\工艺文件汇总\" + folder + "\\已替换文件\\");
            }
            var NewFile = @"D:\MES_Data\技术部\工艺文件汇总\" + folder + "\\已替换文件\\" + filename;//把路径（（技术部\工艺文件汇总\folder(可能是：B_作业指导书或者是A_工艺流程图)\已替换文件））赋值给NewFile
            try
            {
                var conversion = OrignFile.Replace("/", "\\");//把旧文件路径的斜杠（/）替换成这个斜杠（\）
                System.IO.File.Move("D:" + conversion, NewFile);//把旧文件移动到NewFile这个文件夹（技术部\工艺文件汇总\folder(可能是：B_作业指导书或者是A_工艺流程图)\已替换文件）下面
            }
            catch //捕捉异常
            {
                result = false;
            }
            if (folder == "B_作业指导书")//判断文件夹是否等于B_作业指导书
            {
                if (!UploadFileSOP_B(platform, statss, flowchart, ordernumber))//判断调用的方法是否有这些参数（平台，工段名，文件名，订单号）
                {
                    result = false;
                }
                if (id != 0)//判断ID是否不等于0
                {
                    var ite = db.SOPoperating.Where(c => c.Id == id).FirstOrDefault();//根据ID查询数据
                    UserOperateLog operaterecord = new UserOperateLog();
                    operaterecord.OperateDT = DateTime.Now; //添加删除操作时间
                    operaterecord.Operator = ((Users)Session["User"]).UserName;//添加删除操作人
                    //添加操作记录（如：张三在2020年2月27日移除SOP名为文件名（如：组装.pdf）的文件记录）
                    operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "移除SOP名为" + ite.SOPreference_document + "文件记录。";
                    db.SOPoperating.Remove(ite);//删除对应的数据
                    db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
                    db.SaveChanges();//保存到数据库
                }
            }
            if (folder == "A_工艺流程图")//判断文件夹是否等于A_工艺流程图
            {
                if (!UploadFile_SOP(platform, statss, flowchart, ordernumber))//判断调用的方法是否有这些参数（平台，工段名，文件名，订单号）
                {
                    result = false;
                }
                if (id != 0)//判断ID是否不等于0
                {
                    var ite = db.SOPoperating.Where(c => c.Id == id).FirstOrDefault();//根据ID查询数据
                    UserOperateLog operaterecord = new UserOperateLog();
                    operaterecord.OperateDT = DateTime.Now;//添加删除操作时间
                    operaterecord.Operator = ((Users)Session["User"]).UserName;//添加删除操作人
                    //添加操作记录（如：张三在2020年2月27日移除SOP名为文件名（如：组装.pdf）的文件记录）
                    operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "移除SOP名为" + ite.Processflow_chart + "文件记录。";
                    db.SOPoperating.Remove(ite);//删除对应的数据
                    db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据         
                    db.SaveChanges();//保存到数据库
                }
            }
            return result;
        }

        #endregion

        #region----删除SOP文档
        public bool DeleteSOP(string deletefile, int id)
        {
            bool result = true;
            var filename = deletefile.Substring(deletefile.LastIndexOf('/') + 1);//以最后一个斜杠截取路径（只保留文件名）
            //判断该文件夹（技术部\工艺文件汇总\已删除文件）是否等于false（有没有该文件夹）
            if (Directory.Exists(@"D:\MES_Data\技术部\工艺文件汇总\" + "\\已删除文件\\") == false)//如果不存在就创建文件夹
            {
                //创建文件夹（技术部\工艺文件汇总\已删除文件）
                Directory.CreateDirectory(@"D:\MES_Data\技术部\工艺文件汇总\" + "\\已删除文件\\");
            }
            //把路径（技术部\工艺文件汇总\已删除文件）赋值给NewFile
            var DeleteFile = @"D:\MES_Data\技术部\工艺文件汇总\" + "\\已删除文件\\" + filename;
            try
            {
                var conversion = deletefile.Replace("/", "\\");//把要删除文件路径的斜杠（/）替换成这个斜杠（\）
                System.IO.File.Move("D:" + conversion, DeleteFile);//把要删除文件移动到DeleteFile这个文件夹（技术部\工艺文件汇总\已删除文件）下面
            }
            catch //捕捉异常
            {
                result = false;
            }
            if (id != 0)//判断ID是否大于0
            {
                var ite = db.SOPoperating.Where(c => c.Id == id).FirstOrDefault();//根据ID查询数据
                UserOperateLog operaterecord = new UserOperateLog();
                operaterecord.OperateDT = DateTime.Now; //添加删除操作时间
                operaterecord.Operator = ((Users)Session["User"]).UserName;//添加删除操作人
                                                                           //添加操作记录（如：张三在2020年3月13日移除SOP名为文件名（如：组装.pdf）的文件记录）
                operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "移除SOP名为" + filename + "文件记录。";
                db.SOPoperating.Remove(ite);//删除对应的数据
                db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
                db.SaveChanges();//保存到数据库
            }
            return result;
        }
        #endregion

        #region---检索数据库有数据的平台，工段，文件名，订单号

        ///-------------------<Getplatform_SOP_summary>
        /// 1.方法的作用：检索平台（有数据）
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到SOPoperating表里按照ID的排序顺序查询所有平台并去重。
        /// 4.方法（可能）有结果：输出查询数据。
        ///-------------------<Getplatform_SOP_summary>
        public ActionResult Getplatform_SOP()//平台
        {
            var platf = db.SOPoperating.OrderByDescending(m => m.Id).Select(m => m.Platform).Distinct();
            return Content(JsonConvert.SerializeObject(platf));
        }

        ///-------------------<GetSectionname_SOP_summary>
        /// 1.方法的作用：检索工段（有数据）
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到SOPoperating表里按照ID的排序顺序查询所有工段并去重。
        /// 4.方法（可能）有结果：输出查询数据。
        ///------------------- </GetSectionname_SOP_summary>
        public ActionResult GetSectionname_SOP()//工段
        {
            var section = db.SOPoperating.OrderByDescending(m => m.Id).Select(m => m.SectionName).Distinct();
            return Content(JsonConvert.SerializeObject(section));
        }

        ///-------------------<GetProcess_SOP_summary>
        /// 1.方法的作用：检索文件名（有数据）
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到SOPoperating表里按照ID的排序顺序查询所有文件名并去重。
        /// 4.方法（可能）有结果：输出查询数据。
        ///-------------------</GetProcess_SOP_summary>
        public ActionResult GetProcess_SOP()//文件名
        {
            JArray soplist = new JArray();
            var flow = db.SOPoperating.OrderByDescending(m => m.Id).Select(m => m.Processflow_chart).Distinct();//工艺流程图
            var feren = db.SOPoperating.OrderByDescending(m => m.Id).Select(m => m.SOPreference_document).Distinct();//作业指导书
            soplist.Add(flow);
            soplist.Add(feren);
            return Content(JsonConvert.SerializeObject(soplist));
        }

        ///-------------------<Getorder_SOP_summary>
        /// 1.方法的作用：检索订单号(有数据)
        /// 2.方法的参数和用法：无
        /// 3.方法的具体逻辑顺序，判断条件：到SOPoperating表里按照ID的排序顺序查询所有订单号并去重。
        /// 4.方法（可能）有结果：输出查询数据。
        ///-------------------</Getorder_SOP_summary>
        public ActionResult Getorder_SOP()//订单号
        {
            var order = db.SOPoperating.OrderByDescending(m => m.Id).Select(m => m.OrderNum).Distinct();
            return Content(JsonConvert.SerializeObject(order));
        }
        #endregion

        #region-----是否有PDF文档(没有用到)
        ///-------------------<IsHavingPDF_summary>
        /// 1.方法的作用：是否有PDF文档
        /// 2.方法的参数和用法：platform平台，statss工段名，flowchart文件名，ordernumber订单号
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断订单号是否等于null，等于null时，调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\工段名）和查找文件名并截取文件名的后缀。
        /// （1.1）判断filesInfo是否大于0，大于0，输出true（有PDF文档）；等于0，输出false（无PDF文档）。（2）订单号不等于null时，调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\订单号\工段名）。
        /// （2.1）查找文件名并截取文件名的后缀。（2.2）判断filesInfo是否大于0，大于0，输出true（有PDF文档）；等于0，输出false（无PDF文档）。
        /// 4.方法（可能）有结果：输出true（有PDF文档）；输出false（无PDF文档）
        ///-------------------</IsHavingPDF_summary>
        public bool IsHavingPDF(string platform, string statss, string flowchart, string ordernumber)
        {
            if (ordernumber == null)//判断订单号是否等于null
            {
                //调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\工段名）
                List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + platform.ToString() + "\\" + statss + "\\");
                filesInfo = filesInfo.Where(c => c.Name.StartsWith(flowchart) && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();//查找文件名并截取文件名的后缀
                if (filesInfo.Count > 0)//判断filesInfo是否大于0
                    return true;
                else //等于0
                    return false;
            }
            else //订单号不等于null
            {
                //调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\订单号\工段名）
                List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\技术部\工艺文件汇总\B_作业指导书\" + platform.ToString() + "\\" + ordernumber + "\\" + statss + "\\");
                filesInfo = filesInfo.Where(c => c.Name.StartsWith(flowchart) && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();//查找文件名并截取文件名的后缀
                if (filesInfo.Count > 0)//判断filesInfo是否大于0
                    return true;
                else//等于0
                    return false;
            }
        }

        ///-------------------<IsBPDF_summary>
        /// 1.方法的作用：是否有PDF文档
        /// 2.方法的参数和用法：platform平台，statss工段名，flowchart文件名，ordernumber订单号
        /// 3.方法的具体逻辑顺序，判断条件：（1）判断订单号是否等于null，等于null时，调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\工段名）和查找文件名并截取文件名的后缀。
        /// （1.1）判断filesInfo是否大于0，大于0，输出true（有PDF文档）；等于0，输出false（无PDF文档）。（2）订单号不等于null时，调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\B_作业指导书\平台\订单号\工段名）。
        /// （2.1）查找文件名并截取文件名的后缀。（2.2）判断filesInfo是否大于0，大于0，输出true（有PDF文档）；等于0，输出false（无PDF文档）。
        /// 4.方法（可能）有结果：输出true（有PDF文档）；输出false（无PDF文档）
        ///-------------------</IsBPDF_summary>
        public bool IsBPDF(string platform, string statss, string flowchart, string ordernumber)
        {
            if (ordernumber == null)//判断订单号是否等于null
            {
                //调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\工段名）
                List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + platform.ToString() + "\\" + statss + "\\");
                filesInfo = filesInfo.Where(c => c.Name.StartsWith(flowchart) && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();//查找文件名并截取文件名的后缀
                if (filesInfo.Count > 0) //判断filesInfo是否大于0
                    return true;
                else //等于0
                    return false;
            }
            else //订单号不等于null
            {
                //调用公共方法遍历是否有该文件夹（技术部\工艺文件汇总\A_工艺流程图\平台\订单号\工段名）
                List<FileInfo> filesInfo = comm.GetAllFilesInDirectory(@"D:\MES_Data\技术部\工艺文件汇总\A_工艺流程图\" + platform.ToString() + "\\" + ordernumber + "\\" + statss + "\\");
                filesInfo = filesInfo.Where(c => c.Name.StartsWith(flowchart) && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();//查找文件名并截取文件名的后缀
                if (filesInfo.Count > 0)//判断filesInfo是否大于0
                    return true;
                else //等于0
                    return false;
            }
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
    }
}