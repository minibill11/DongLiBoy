﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Sockets;
using ZXing;
using ZXing.QrCode.Internal;
using ZXing.Aztec.Internal;

namespace JianHeMES.Controllers
{
    public class LablePrintController : Controller
    {
        // GET: LablePrint
        public ActionResult Index()
        {
            return View();
        }
    }


    #region 图像文件处理
    public class BarCodeLablePrint
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);


        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.


        [HttpPost]
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "LablePrint";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        [HttpPost]
        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }

        [HttpPost]
        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }


        #region 输出条码和二维码图片文件
        [HttpPost]
        public static Bitmap BarCodeToImg(string code, int width, int height)
        {
            BarcodeWriter barCodeWriter = new BarcodeWriter();
            barCodeWriter.Format = BarcodeFormat.CODE_128;    //条形码规格：EAN13规格：12（无校验位）或13位数字
            barCodeWriter.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            barCodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
            barCodeWriter.Options.Height = height;
            barCodeWriter.Options.Width = width;
            barCodeWriter.Options.Margin = 0;
            ZXing.Common.BitMatrix bm = barCodeWriter.Encode(code);
            Bitmap img = barCodeWriter.Write(bm);    // 生成条码图片
            return img;
        }

        
        public static Bitmap QRCodeImg(string code, int width, int height)
        {
            BarcodeWriter barCodeWriter = new BarcodeWriter();
            barCodeWriter.Format = BarcodeFormat.QR_CODE;
            barCodeWriter.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            barCodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
            barCodeWriter.Options.Height = height;
            barCodeWriter.Options.Width = width;
            barCodeWriter.Options.Margin = 0;
            ZXing.Common.BitMatrix bm = barCodeWriter.Encode(code);
            Bitmap img = barCodeWriter.Write(bm);
            //MemoryStream ms = new MemoryStream();
            //img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //img.Dispose();
            //return File(ms.ToArray(), "image/jpeg");
            return img;
        }


        #endregion

        #region 图像灰度、灰度反转、二值化处理
        /// <summary>
        /// 图像灰度化
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ToGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }


        /// <summary>
        /// 图像灰度反转
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap GrayReverse(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    Color newColor = Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }

        /// <summary>
        /// 图像二值化1：取图片的平均灰度作为阈值，低于该值的全都为0，高于该值的全都为255
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo1Bpp1(Bitmap bmp)
        {
            int average = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    average += color.B;
                }
            }
            average = (int)average / (bmp.Width * bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    int value = 255 - color.B;
                    //Color newColor = value > average ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);  //原判断值
                    Color newColor = value > average-100 ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }

        /// <summary>
        /// 图像二值化2
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo1Bpp2(Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
            for (int y = 0; y < h; y++)
            {
                byte[] scan = new byte[(w + 7) / 8];
                for (int x = 0; x < w; x++)
                {
                    Color c = img.GetPixel(x, y);
                    if (c.GetBrightness() >= 0.5) scan[x / 8] |= (byte)(0x80 >> (x % 8));
                }
                Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * y), scan.Length);
            }
            return bmp;
        }
        #endregion


        /// <summary>
        /// 将图片进行反色处理
        /// </summary>
        /// <param name="mybm">原始图片</param>
        /// <param name="width">原始图片的长度</param>
        /// <param name="height">原始图片的高度</param>
        /// <returns>被反色后的图片</returns>
        public static Bitmap RePic(Bitmap mybm, int width, int height)
        {
            Bitmap bm = new Bitmap(width, height);//初始化一个记录处理后的图片的对象
            int x, y, resultR, resultG, resultB;
            Color pixel;

            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = mybm.GetPixel(x, y);//获取当前坐标的像素值
                    resultR = 255 - pixel.R;//反红
                    resultG = 255 - pixel.G;//反绿
                    resultB = 255 - pixel.B;//反蓝
                    bm.SetPixel(x, y, Color.FromArgb(resultR, resultG, resultB));//绘图
                }
            }

            return bm;//返回经过反色处理后的图片
        }
    }
    #endregion


    #region 打印中文字符
    public class ZebraPrintHelper
    {
        [DllImport("Fnthex32.dll")]
        public static extern int GETFONTHEX(
        string BarcodeText,//转换的文本
        string FontName,//打印字体
        string FileName,//存储的变量名称
        int Orient,//方向
        int Height,//字体高度,点阵高度
        int Width,//点阵宽度
        int IsBold,//是否加粗0,1
        int IsItalic,//是否斜体0,1
        StringBuilder ReturnBarcodeCMD);//存储的内容

        /// <summary>
        /// 转换中文
        /// </summary>
        /// <param name="chStr">转换的字符</param>
        /// <param name="tempName">存储的变量名称</param>
        /// <param name="font">使用的字体</param>
        /// <returns></returns>
        public static string ConvertChineseToHex(string chStr, string tempName, string font = "Microsoft YaHei")
        {
            StringBuilder cBuf = new StringBuilder(chStr.Length * 1024);
            int nCount = GETFONTHEX(chStr, font, tempName, 0, 25, 15, 1, 0, cBuf);
            string temp = " " + cBuf.ToString();
            temp = temp.Substring(0, nCount);
            return temp;
        }
    }
    #endregion


    #region 条码处理主程序
    /// <summary>
    /// 斑马工具类,把图像转换成斑马打印机的命令
    /// </summary>
    /// 
    public class ZebraUnity
    {
        [HttpPost]
        public string Sendzebra(string ordernum)
        {
            //try
            //{
                //开始绘制图片
                int initialWidth = 780, initialHeight = 700;
                Bitmap theBitmap = new Bitmap(initialWidth, initialHeight);
                Graphics theGraphics = Graphics.FromImage(theBitmap);
                Brush bush = new SolidBrush(System.Drawing.Color.Black);//填充的颜色
                //呈现质量
                theGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                //背景色
                theGraphics.Clear(System.Drawing.Color.FromArgb(120, 240, 180));
                //double beishuhege = 0.37;
                Pen pen = new Pen(Color.Black,2);
                theGraphics.DrawLine(pen,10,10,10,600);
                theGraphics.DrawRectangle(pen, 20, 20, 500, 600);
                double beishulogo = 0.3;
                System.Drawing.Image img = new Bitmap("\\Images\\LOGO_black.png");
                theGraphics.DrawImage(img, 55, 0, (float)(img.Width * beishulogo), (float)(img.Height * beishulogo));
                //System.Drawing.Image img2 = new Bitmap(Application.StartupPath + "\\data\\系统数据\\合格.png");
                //theGraphics.DrawImage(img2, 500, -25, (float)(img2.Width * beishuhege), (float)(img2.Height * beishuhege));
                System.Drawing.Font myFont;
                myFont = new System.Drawing.Font("宋体", 32, FontStyle.Bold);
                StringFormat geshi = new StringFormat();
                geshi.Alignment = StringAlignment.Center; //居中
                //geshi.Alignment = StringAlignment.Far; //右对齐
                int starty = 135;
                int jianggey = 70;
                theGraphics.DrawString(ordernum, myFont, bush, 50, starty + jianggey * 5);
                //Rectangle rect = new Rectangle(0,starty , initialWidth, initialHeight);
                //theGraphics.DrawString("品名：" + pdnow.name, myFont, bush, 50, starty);
                //theGraphics.DrawString("规格：", myFont, bush, 50, starty + jianggey * 1);
                //theGraphics.DrawString("等级：" + cbhege.Text, myFont, bush, 50, starty + jianggey * 2);
                //theGraphics.DrawString("检验员：" + tbjyy.Text, myFont, bush, 50, starty + jianggey * 3);
                //theGraphics.DrawString("批号：20180722", myFont, bush, 50, starty + jianggey * 4);
                //theGraphics.DrawString("惠州市健和光电有限公司", myFont, bush, 50, starty + jianggey * 5);
                //System.Drawing.Image img3 = getqrcode("http://www.baidu.com");//传入URL返回二维码image
                System.Drawing.Image img3 = BarCodeLablePrint.QRCodeImg("http://172.16.6.145",100,100);//传入URL返回二维码image
                theGraphics.DrawImage(img3, 495, 180, (float)(240), (float)(240));
                //结束图片绘制以上都是绘制图片的代码


                //int totalbytes = 64800;
                //int rowbytes = 90;
                //string hex = ZebraUnity.BitmapToHex(theBitmap, out totalbytes, out rowbytes);//将图片转成ASCii码
                //string mubanstring = "~DGR:ZLOGO.GRF," + totalbytes.ToString() + "," + rowbytes.ToString() + "," + hex;//将图片生成模板指令
                //readerCamera2.CameraSendMessage(mubanstring);//利用TCP/IP发送模板指令到打印机


                //string otherstring = "^XA^FO0,0^XGR:ZLOGO.GRF,1,1^FS";//调用该模板指令
                //otherstring += "^XZ";
                //readerCamera2.CameraSendMessage(otherstring);//发送调用模板指令，利用TCP/ip协议发送指令
                Bitmap newbmp = new Bitmap(theGraphics.ToString());
                //MemoryStream ms = new MemoryStream();
                //img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            string ptrn = "InsideBoxLablePrinter";
            //Bitmap theBitmap = ZebraUnity.Base64ToBitmap(bitmap);//把base64字符串转换成Bitmap
            string sb = "^XA~DGR:ZONE.GRF,";
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(newbmp)));
            MemoryStream ms = new MemoryStream();
            theBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            int totalbytes = bm.ToString().Length;
            int rowbytes = 10;
            string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
            sb += totalbytes + "," + rowbytes + "," + hex;
            sb += "^LH0,0^F10,10^XGR:ZONE.GRF^FS^XZ";
            theBitmap.Dispose();
            //if (pagecount == 1)
            //{
            //    if (BarCodeLablePrint.SendStringToPrinter(ptrn, sb.ToString())) return Content("打印成功");
            //}
            //else if (pagecount == 2)
            //{
            //    bool resut = BarCodeLablePrint.SendStringToPrinter(ptrn, sb.ToString());
            //    resut = BarCodeLablePrint.SendStringToPrinter(ptrn, sb.ToString());
            //    if (resut) return Content("打印成功");
            //    else return Content("打印失败");
            //}
            return "打印成功";
                //img.Dispose();
                //return newbmp;
            //}
            //catch
            //{
            //    //ToolData.WriteLog(lrtxtLog, "发送打印机数据出错", 1);
            //}
        }

        //public System.Drawing.Image getqrcode(string content)
        //{
        //    var encoder = new Encoder(ErrorCorrectionLevel.M);
        //    QRCode qrCode = encoder.Encode(content);
        //    GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(12, QuietZoneModules.Two), Brushes.Black, Brushes.White);//如需改变二维码大小，调整12即可
        //    DrawingSize dSize = render.SizeCalculator.GetSize(qrCode.Matrix.Width);
        //    Bitmap map = new Bitmap(dSize.CodeWidth, dSize.CodeWidth);
        //    Graphics g = Graphics.FromImage(map);
        //    render.Draw(g, qrCode.Matrix);
        //    return map;
        //}


        #region 定义私有字段
        /// <summary>
        /// 线程锁，防止多线程调用。
        /// </summary>
        private static object SyncRoot = new object();
        /// <summary>
        /// ZPL压缩字典
        /// </summary>
        private static List<KeyValuePair<char, int>> compressDictionary = new List<KeyValuePair<char, int>>();
        #endregion

        #region 构造方法

        static ZebraUnity()
        {
            InitCompressCode();
        }

        #endregion

        #region 定义属性
        /// <summary>
        /// 图像的二进制数据
        /// </summary>
        public static byte[] GraphBuffer { get; set; }
        /// <summary>
        /// 图像的宽度
        /// </summary>
        private static int GraphWidth { get; set; }
        /// <summary>
        /// 图像的高度
        /// </summary>
        private static int GraphHeight { get; set; }
        private static int RowSize
        {
            get
            {
                return (((GraphWidth) + 31) >> 5) << 2;
            }
        }
        /// <summary>
        /// 每行的字节数
        /// </summary>
        private static int RowRealBytesCount
        {
            get
            {
                if ((GraphWidth % 8) > 0)
                {
                    return GraphWidth / 8 + 1;
                }
                else
                {
                    return GraphWidth / 8;
                }
            }
        }
        #endregion

        #region 位图转斑马指令字符串
        /// <summary>
        /// 位图转斑马指令字符串
        /// </summary>
        /// <param name="bitmap">位图数据</param>
        /// <param name="totalBytes">总共的字节数</param>
        /// <param name="rowBytes">每行的字节数</param>
        /// <returns>斑马ZPL 2命令</returns>
        public static string BmpToZpl(byte[] bitmap, out int totalBytes, out int rowBytes)
        {
            try
            {
                GraphBuffer = bitmap;
                byte[] bmpData = getBitmapData();
                string textHex = BitConverter.ToString(bmpData).Replace("-", string.Empty);
                string textBitmap = CompressLZ77(textHex);
                totalBytes = GraphHeight * RowRealBytesCount;
                rowBytes = RowRealBytesCount;
                return textBitmap;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 位图转ZPL指令
        /// </summary>
        /// <param name="bitmap">位图</param>
        /// <param name="totalBytes">返回参数总共字节数</param>
        /// <param name="rowBytes">返回参数每行的字节数</param>
        /// <returns>ZPL命令</returns>
        public static string BmpToZpl(System.Drawing.Image bitmap, out int totalBytes, out int rowBytes)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                return BmpToZpl(stream.ToArray(), out totalBytes, out rowBytes);
            }
        }

        /// <summary>
        /// 根据图片生成图片的ASCII 十六进制
        /// </summary>
        /// <param name="sourceBmp">原始图片</param>
        /// <param name="totalBytes">总共字节数</param>
        /// <param name="rowBytes">每行的字节数</param>
        /// <returns>ASCII 十六进制</returns>
        public static string BitmapToHex(System.Drawing.Image sourceBmp, out int totalBytes, out int rowBytes)
        {
            // 转成单色图
            Bitmap grayBmp = ConvertToGrayscale(sourceBmp as Bitmap);
            // 锁定位图数据    
            Rectangle rect = new Rectangle(0, 0, grayBmp.Width, grayBmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = grayBmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, grayBmp.PixelFormat);
            // 获取位图数据第一行的起始地址     
            IntPtr ptr = bmpData.Scan0;
            // 定义数组以存放位图的字节流数据      
            // 处理像素宽对应的字节数，如不为8的倍数，则对最后一个字节补0    
            int width = (int)Math.Ceiling(grayBmp.Width / 8.0);
            // 获取位图实际的字节宽，这个值因为要考虑4的倍数关系，可能大于width  
            int stride = Math.Abs(bmpData.Stride);
            // 计算位图数据实际所占的字节数，并定义数组      
            int bitmapDataLength = stride * grayBmp.Height;
            byte[] ImgData = new byte[bitmapDataLength];
            // 从位图文件复制图像数据到数组，从实际图像数据的第一行开始；因ptr指针而无需再考虑行倒序存储的处理          
            System.Runtime.InteropServices.Marshal.Copy(ptr, ImgData, 0, bitmapDataLength);
            // 计算异或操作数，以处理包含图像数据但又有补0操作的那个字节         
            byte mask = 0xFF;
            // 计算这个字节补0的个数       
            //int offset = 8 * width - grayBmp.Width;
            int offset = 8 - (grayBmp.Width % 8);
            //offset %= 8;
            offset = offset % 8;
            // 按补0个数对0xFF做相应位数的左移位操作           
            mask <<= (byte)offset;
            // 图像反色处理        
            for (int j = 0; j < grayBmp.Height; j++)
            {
                for (int i = 0; i < stride; i++)
                {
                    if (i < width - 1) //无补0的图像数据
                    {
                        ImgData[j * stride + i] ^= 0xFF;
                    }
                    else if (i == width - 1) //有像素的最后一个字节，可能有补0   
                    {
                        ImgData[j * stride + i] ^= mask;
                    }
                    else  //为满足行字节宽为4的倍数而最后补的字节        
                    {
                        //ImgData[j * stride + i] = 0x00;
                        ImgData[j * stride + i] ^= 0x00;
                    }
                }
            }
            // 将位图数据转换为16进制的ASCII字符          
            string zplString = BitConverter.ToString(ImgData);
            zplString = CompressLZ77(zplString.Replace("-", string.Empty));
            totalBytes = bitmapDataLength;
            rowBytes = stride;
            return zplString;
        }
        #endregion

        #region 获取单色位图数据
        /// <summary>
        /// 获取单色位图数据
        /// </summary>
        /// <param name="pimage"></param>
        /// <returns></returns>
        public static Bitmap ConvertToGrayscale(Bitmap pimage)
        {
            Bitmap source = null;

            // If original bitmap is not already in 32 BPP, ARGB format, then convert
            if (pimage.PixelFormat != PixelFormat.Format32bppArgb)
            {
                source = new Bitmap(pimage.Width, pimage.Height, PixelFormat.Format32bppArgb);
                source.SetResolution(pimage.HorizontalResolution, pimage.VerticalResolution);
                using (Graphics g = Graphics.FromImage(source))
                {
                    g.DrawImageUnscaled(pimage, 0, 0);
                }
            }
            else
            {
                source = pimage;
            }

            // Lock source bitmap in memory
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Copy image data to binary array
            int imageSize = sourceData.Stride * sourceData.Height;
            byte[] sourceBuffer = new byte[imageSize];
            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);

            // Unlock source bitmap
            source.UnlockBits(sourceData);

            // Create destination bitmap
            Bitmap destination = new Bitmap(source.Width, source.Height, PixelFormat.Format1bppIndexed);

            // Lock destination bitmap in memory
            BitmapData destinationData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

            // Create destination buffer
            imageSize = destinationData.Stride * destinationData.Height;
            byte[] destinationBuffer = new byte[imageSize];

            int sourceIndex = 0;
            int destinationIndex = 0;
            int pixelTotal = 0;
            byte destinationValue = 0;
            int pixelValue = 128;
            int height = source.Height;
            int width = source.Width;
            int threshold = 500;

            // Iterate lines
            for (int y = 0; y < height; y++)
            {
                sourceIndex = y * sourceData.Stride;
                destinationIndex = y * destinationData.Stride;
                destinationValue = 0;
                pixelValue = 128;

                // Iterate pixels
                for (int x = 0; x < width; x++)
                {
                    // Compute pixel brightness (i.e. total of Red, Green, and Blue values)
                    pixelTotal = sourceBuffer[sourceIndex + 1] + sourceBuffer[sourceIndex + 2] + sourceBuffer[sourceIndex + 3];
                    if (pixelTotal > threshold)
                    {
                        destinationValue += (byte)pixelValue;
                    }
                    if (pixelValue == 1)
                    {
                        destinationBuffer[destinationIndex] = destinationValue;
                        destinationIndex++;
                        destinationValue = 0;
                        pixelValue = 128;
                    }
                    else
                    {
                        pixelValue >>= 1;
                    }
                    sourceIndex += 4;
                }
                if (pixelValue != 128)
                {
                    destinationBuffer[destinationIndex] = destinationValue;
                }
            }

            // Copy binary image data to destination bitmap
            Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize);

            // Unlock destination bitmap
            destination.UnlockBits(destinationData);

            // Dispose of source if not originally supplied bitmap
            if (source != pimage)
            {
                source.Dispose();
            }

            // Return
            return destination;
        }
        /// <summary>
        /// 获取单色位图数据(1bpp)，不含文件头、信息头、调色板三类数据。
        /// </summary>
        /// <returns></returns>
        private static byte[] getBitmapData()
        {
            MemoryStream srcStream = new MemoryStream();
            MemoryStream dstStream = new MemoryStream();
            Bitmap srcBmp = null;
            Bitmap dstBmp = null;
            byte[] srcBuffer = null;
            byte[] dstBuffer = null;
            byte[] result = null;
            try
            {
                srcStream = new MemoryStream(GraphBuffer);
                srcBmp = Bitmap.FromStream(srcStream) as Bitmap;
                srcBuffer = srcStream.ToArray();
                GraphWidth = srcBmp.Width;
                GraphHeight = srcBmp.Height;
                //dstBmp = srcBmp.Clone(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), PixelFormat.Format1bppIndexed);
                dstBmp = ConvertToGrayscale(srcBmp);
                dstBmp.Save(dstStream, ImageFormat.Bmp);
                dstBuffer = dstStream.ToArray();

                result = dstBuffer;

                int bfOffBits = BitConverter.ToInt32(dstBuffer, 10);
                result = new byte[GraphHeight * RowRealBytesCount];

                ////读取时需要反向读取每行字节实现上下翻转的效果，打印机打印顺序需要这样读取。
                for (int i = 0; i < GraphHeight; i++)
                {
                    int sindex = bfOffBits + (GraphHeight - 1 - i) * RowSize;
                    int dindex = i * RowRealBytesCount;
                    Array.Copy(dstBuffer, sindex, result, dindex, RowRealBytesCount);
                }

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] ^= 0xFF;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (srcStream != null)
                {
                    srcStream.Dispose();
                    srcStream = null;
                }
                if (dstStream != null)
                {
                    dstStream.Dispose();
                    dstStream = null;
                }
                if (srcBmp != null)
                {
                    srcBmp.Dispose();
                    srcBmp = null;
                }
                if (dstBmp != null)
                {
                    dstBmp.Dispose();
                    dstBmp = null;
                }
            }
            return result;
        }
        #endregion

        #region LZ77图像字节流压缩方法
        private static string CompressLZ77(string text)
        {
            //将转成16进制的文本进行压缩
            string result = string.Empty;
            char[] arrChar = text.ToCharArray();
            int count = 1;
            for (int i = 1; i < text.Length; i++)
            {
                if (arrChar[i - 1] == arrChar[i])
                {
                    count++;
                }
                else
                {
                    result += convertNumber(count) + arrChar[i - 1];
                    count = 1;
                }
                if (i == text.Length - 1)
                {
                    result += convertNumber(count) + arrChar[i];
                }
            }
            return result;
        }

        private static string DecompressLZ77(string text)
        {
            string result = string.Empty;
            char[] arrChar = text.ToCharArray();
            int count = 0;
            for (int i = 0; i < arrChar.Length; i++)
            {
                if (isHexChar(arrChar[i]))
                {
                    //十六进制值
                    result += new string(arrChar[i], count == 0 ? 1 : count);
                    count = 0;
                }
                else
                {
                    //压缩码
                    int value = GetCompressValue(arrChar[i]);
                    count += value;
                }
            }
            return result;
        }

        private static int GetCompressValue(char c)
        {
            int result = 0;
            for (int i = 0; i < compressDictionary.Count; i++)
            {
                if (c == compressDictionary[i].Key)
                {
                    result = compressDictionary[i].Value;
                }
            }
            return result;
        }

        private static bool isHexChar(char c)
        {
            return c > 47 && c < 58 || c > 64 && c < 71 || c > 96 && c < 103;
        }

        private static string convertNumber(int count)
        {
            //将连续的数字转换成LZ77压缩代码，如000可用I0表示。
            string result = string.Empty;
            if (count > 1)
            {
                while (count > 0)
                {
                    for (int i = compressDictionary.Count - 1; i >= 0; i--)
                    {
                        if (count >= compressDictionary[i].Value)
                        {
                            result += compressDictionary[i].Key;
                            count -= compressDictionary[i].Value;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        private static void InitCompressCode()
        {
            //G H I J K L M N O P Q R S T U V W X Y        对应1,2,3,4……18,19。
            //g h i j k l m n o p q r s t u v w x y z      对应20,40,60,80……340,360,380,400。            
            for (int i = 0; i < 19; i++)
            {
                compressDictionary.Add(new KeyValuePair<char, int>(Convert.ToChar(71 + i), i + 1));
            }
            for (int i = 0; i < 20; i++)
            {
                compressDictionary.Add(new KeyValuePair<char, int>(Convert.ToChar(103 + i), (i + 1) * 20));
            }
        }
        #endregion

        private static char[] base64CodeArray = new char[]
{
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4',  '5', '6', '7', '8', '9', '+', '/', '='
};

        /// <summary>
        /// 是否base64字符串
        /// </summary>
        /// <param name="base64Str">要判断的字符串</param>
        /// <returns></returns>
        public static bool IsBase64(string base64Str)
        {
            byte[] bytes = null;
            return IsBase64(base64Str, out bytes);
        }


        #region C#判断字符串是否base64，及base64转换为Bitmap
        /// <summary>
        /// 是否base64字符串
        /// </summary>
        /// <param name="base64Str">要判断的字符串</param>
        /// <param name="bytes">字符串转换成的字节数组</param>
        /// <returns></returns>
        public static bool IsBase64(string base64Str, out byte[] bytes)
        {
            //string strRegex = "^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$";
            bytes = null;
            if (string.IsNullOrEmpty(base64Str))
                return false;
            else
            {
                if (base64Str.Contains(","))
                    base64Str = base64Str.Split(',')[1];
                if (base64Str.Length % 4 != 0)
                    return false;
                if (base64Str.Any(c => !base64CodeArray.Contains(c)))
                    return false;
            }
            try
            {
                bytes = Convert.FromBase64String(base64Str);
                return true;
            }
            catch (ZXing.FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// 把base64字符串转换成Bitmap
        /// </summary>
        /// <param name="base64Str">要转换的base64字符串</param>
        /// <returns></returns>
        public static Bitmap Base64ToBitmap(string base64Str)
        {
            Bitmap bitmap = null;
            byte[] bytes = null;
            try
            {
                if (IsBase64(base64Str, out bytes))
                {
                    using (MemoryStream stream = new MemoryStream(bytes))
                    {
                        stream.Seek(0, SeekOrigin.Begin);//为了避免有时候流指针定位错误，显式定义一下指针位置
                        bitmap = new Bitmap(stream);
                    }
                }
            }
            catch (Exception)
            {
                bitmap = null;
            }
            return bitmap;
        }
        #endregion



        #region 直连IP和端口打印内容
        /// <summary>
        /// IPPrint调用IP和端口链接打印机
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="modulenum"></param>
        /// <param name="barcode"></param>
        /// <param name="pagecount"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        //调用IP链接打印机
        public static string IPPrint(string data = "", int pagecount = 1, string ip = "", int port = 0)
        {
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(ip, port);
                StreamWriter writer = new StreamWriter(client.GetStream());
                for(int i =1;i<pagecount+1;i++)
                {
                    writer.Write(data);
                }
                writer.Flush();
                writer.Close();
                client.Close();
                return "打印成功！";
            }
            catch
            {
                return "打印连接失败,请检查打印机是否断网或未开机！";
            }
        }

        //测试
        public string IPPrint2(string bitmap="", string modulenum="", string barcode="", int pagecount = 1, string ip = "192.168.99.240", int port = 9101)
        {
            //if (String.IsNullOrEmpty(modulenum) && String.IsNullOrEmpty(barcode)) return "模组号或条码号没有值！";
            Bitmap theBitmap = ZebraUnity.Base64ToBitmap(bitmap);//把base64字符串转换成Bitmap
            Bitmap bm = new Bitmap(BarCodeLablePrint.ConvertTo1Bpp1(BarCodeLablePrint.ToGray(theBitmap)));
            int totalbytes = bm.ToString().Length;
            int rowbytes = 10;
            string hex = ZebraUnity.BmpToZpl(bm, out totalbytes, out rowbytes);
            string data = "";
            //data = "^XA^A@N,80,56,B:ST.FNT^BCN,0,Y,N,N^FO250,30BY20^FD" + modulenum + "^FS^FO210,115BY50^BCN,80,Y,N,N^A@N,30,21,B:Arial.FNT^FD" + barcode + "^FS^XZ";
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(ip, port);
                if (!client.Connected) return "打印机连接失败";
                StreamWriter writer = new StreamWriter(client.GetStream());
                writer.Write(data);
                writer.Flush();
                writer.Close();
                client.Close();
                return "打印成功";
            }
            catch
            {
                return "打印失败";
            }
        }
        #endregion



    }
    #endregion


}