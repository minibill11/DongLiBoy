using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class SOPController : Controller
    {
        // GET: SOP
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string ordernum="", string section="")
        {
            bool status = false;
            string add = "";
            //连接共享文件夹
            status = connectState(@"\\172.16.2.9\技术部\01 工艺文件汇总\00 技术部工艺文件电子档存放(对外)\三阶文件\A 工艺流程\01 325x365平台 V系列", "15043", "h15043");
            if (status)
            {
                DirectoryInfo theFolder = new DirectoryInfo(@"\\172.16.2.9\技术部\01 工艺文件汇总\00 技术部工艺文件电子档存放(对外)\三阶文件\A 工艺流程\01 325x365平台 V系列");

                //先测试读文件，把目录路径与文件名连接
                string filename = theFolder.ToString() + "\\JA001E00G001A 325x365平台 V系列 前维护 线材组生产工艺流程图(MRV220&LRV220).pdf";
                add = "\\\\172.16.2.9\\技术部\\01 工艺文件汇总\\00 技术部工艺文件电子档存放(对外)\\三阶文件\\A 工艺流程\\01 325x365平台 V系列\\JA001E00G001A 325x365平台 V系列 前维护 线材组生产工艺流程图(MRV220&LRV220).pdf";
                ReadFiles(filename);

                ////测试写文件，拼出完整的路径
                //filename = theFolder.ToString() + "\\bad.txt";
                //WriteFiles(filename);

                ////遍历共享文件夹，把共享文件夹下的文件列表列到listbox
                //foreach (FileInfo nextFile in theFolder.GetFiles())
                //{

                //}
            }
            return Content(add);
        }


        public static bool connectState(string path, string userName, string passWord)
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


        //读文件
        public static void ReadFiles(string path)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(path))
                {
                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        //写入文件
        public static void WriteFiles(string path)
        {
            try
            {
                // Create an instance of StreamWriter to write text to a file.
                // The using statement also closes the StreamWriter.
                using (StreamWriter sw = new StreamWriter(path))
                {
                    // Add some text to the file.
                    sw.Write("This is the ");
                    sw.WriteLine("header for the file.");
                    sw.WriteLine("-------------------");
                    // Arbitrary objects can also be written to the file.
                    sw.Write("The date is: ");
                    sw.WriteLine(DateTime.Now);
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }


        #region --------------------查看SOP pdf文档页面
        public ActionResult preview_SOP_pdf(string ordernum)
        {
            List<FileInfo> filesInfo = new List<FileInfo>();
            string directory = "D:\\MES_Data\\AssembleSample_Files\\" + ordernum + "\\";
            if (Directory.Exists(@directory) == false)//如果不存在就创建订单文件夹
            {
                return Content("此组装首件pdf版文件尚未上传，无pdf文件可下载！");
            }
            filesInfo = GetAllFilesInDirectory(directory);
            List<string> pdf_address = new List<string>();
            string address = "";
            if (filesInfo.Where(c => c.Name == ordernum + "_AssembleSample.pdf").Count() > 0)
            {
                address = "/MES_Data/AssembleSample_Files" + "/" + ordernum + "/" + ordernum + "_AssembleSample.pdf";
            }
            else
            {
                return Content("此组装首件pdf版文件尚未上传，无pdf文件可下载！");
            }
            return Content(address);
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