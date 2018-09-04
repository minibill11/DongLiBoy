using JianHeMES.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Data;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JianHeMES.Areas.KongYaHT.Models;
using JianHeMES.Controllers;
using System.Net.Mail;
using System.Text;
using System.Web.Helpers;

namespace JianHeMES.Hubs
{


    [HubName("myHubCheck")]
    public class MyHubCheck : Hub
    {
        // Is set via the constructor on each creation
        private BroadcasterCheck _broadcaster;
        public MyHubCheck()
            : this(BroadcasterCheck.Instance)
        {
        }
        public MyHubCheck(BroadcasterCheck broadcaster)
        {
            _broadcaster = broadcaster;
        }
    }

    /// <summary>
    /// 数据广播器
    /// </summary>
    public class BroadcasterCheck
    {
        private readonly static Lazy<BroadcasterCheck> _instance =
            new Lazy<BroadcasterCheck>(() => new BroadcasterCheck());
        private readonly IHubContext _hubContext;
        private Timer _broadcastLoop;

        public BroadcasterCheck()
        {
            // 获取所有连接的句柄，方便后面进行消息广播
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHubCheck>();
            // Start the broadcast loop
            _broadcastLoop = new Timer(
                BroadcastShape,
                null,
                0,
                1000);
        }

            int countsame = 0;
            JObject KY_json_old = new JObject();
        private void BroadcastShape(object state)
        {   // 定期执行的方法
            //空压房数据
            JObject KY_json = new JObject();
            using (var KY_db = new kongyadbEntities())
            {
            var KY_aircomp1 = (from m in KY_db.aircomp1 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_aircomp2 = (from m in KY_db.aircomp2 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_aircomp3 = (from m in KY_db.aircomp3 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_airbottle1 = (from m in KY_db.airbottle1 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_airbottle2 = (from m in KY_db.airbottle2 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_airbottle3 = (from m in KY_db.airbottle3 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_dryer1 = (from m in KY_db.dryer1 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_dryer2 = (from m in KY_db.dryer2 select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_headerpipe3inch = (from m in KY_db.headerpipe3inch select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_headerpipe4inch = (from m in KY_db.headerpipe4inch select m).OrderByDescending(p => p.id).FirstOrDefault();
            var KY_room = (from m in KY_db.room select m).OrderByDescending(p => p.id).FirstOrDefault();
            KY_json.Add("aircomp1", JsonConvert.SerializeObject(KY_aircomp1));
            KY_json.Add("aircomp2", JsonConvert.SerializeObject(KY_aircomp2));
            KY_json.Add("aircomp3", JsonConvert.SerializeObject(KY_aircomp3));
            KY_json.Add("airbottle1", JsonConvert.SerializeObject(KY_airbottle1));
            KY_json.Add("airbottle2", JsonConvert.SerializeObject(KY_airbottle2));
            KY_json.Add("airbottle3", JsonConvert.SerializeObject(KY_airbottle3));
            KY_json.Add("dryer1", JsonConvert.SerializeObject(KY_dryer1));
            KY_json.Add("dryer2", JsonConvert.SerializeObject(KY_dryer2));
            KY_json.Add("headerpipe3inch", JsonConvert.SerializeObject(KY_headerpipe3inch));
            KY_json.Add("headerpipe4inch", JsonConvert.SerializeObject(KY_headerpipe4inch));
            KY_json.Add("room", JsonConvert.SerializeObject(KY_room));
            }
            _hubContext.Clients.All.sendKYCheck(KY_json);

            if (KY_json_old != KY_json)
            {
                //countsame = 0;
                KY_json_old = KY_json;
            }
            if (KY_json_old == KY_json) countsame++;

            if (countsame==8|| countsame == 18 || countsame == 60 || countsame == 300 || countsame == 900 || countsame == 1800 || countsame == 3600 || countsame == 7200 || countsame == 14400 || countsame == 28800 || countsame == 43200 || countsame == 86400 || countsame == 172800 || countsame == 259200 || countsame == 345600)
            {
                switch (countsame)
                {
                    case 8:
                        SendEmail2("8秒");//8秒
                        break;
                    case 18:
                        SendEmail2("18秒");//8秒
                        break;
                    case 60:
                        SendEmail2("1分钟");//1分钟
                        break;
                    case 300:
                        SendEmail("5分钟");//5分钟
                        break;
                    case 900:
                        SendEmail("15分钟");//15分钟
                        break;
                    case 1800:
                        SendEmail("30分钟");//30分钟
                        break;
                    case 3600:
                        SendEmail("1小时");//1小时
                        break;
                    case 7200:
                        SendEmail("2小时");//2小时
                        break;
                    case 14400:
                        SendEmail("4小时");//4小时
                        break;
                    case 28800:
                        SendEmail("8小时");//8小时
                        break;
                    case 43200:
                        SendEmail("12小时");//12小时
                        break;
                    case 86400:
                        SendEmail("24小时");//24小时
                        break;
                    case 172800:
                        SendEmail("48小时");//48小时
                        break;
                    case 259200:
                        SendEmail("72小时");//72小时
                        break;
                    case 345600:
                        SendEmail("96小时");//96小时
                        break;
                }
            }


            #region ---------其他广播数据-----------
            ////三楼温湿度数据
            //JObject TH3_json = new JObject();   //创建JSON对象
            ////取出数据
            //using (var KY_db = new kongyadbEntities())
            //{
            //var TH_40001676_6 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "6") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40001676_9 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "9") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40001676_10 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "10") select m).OrderByDescending(p => p.id).FirstOrDefault();
            ////存入JSON对象
            //TH3_json.Add("TH_40001676_1", JsonConvert.SerializeObject(TH_40001676_6));
            //TH3_json.Add("TH_40001676_2", JsonConvert.SerializeObject(TH_40001676_9));
            //TH3_json.Add("TH_40001676_3", JsonConvert.SerializeObject(TH_40001676_10));
            //}
            ////广播发送JSON数据
            //_hubContext.Clients.All.sendTH3(TH3_json);

            ////四楼温湿度数据
            //JObject TH4_json = new JObject();   
            //using (var KY_db = new kongyadbEntities())
            //{
            //var TH_40001676_1 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "1") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40001676_2 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "2") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40001676_3 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "3") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40001676_4 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "4") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40001676_5 = (from m in db.THhistory where (m.DeviceID == "40001676" && m.NodeID == "5") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //TH4_json.Add("TH_40001676_1", JsonConvert.SerializeObject(TH_40001676_1));                                                                                                  
            //TH4_json.Add("TH_40001676_2", JsonConvert.SerializeObject(TH_40001676_2));
            //TH4_json.Add("TH_40001676_3", JsonConvert.SerializeObject(TH_40001676_3));
            //TH4_json.Add("TH_40001676_4", JsonConvert.SerializeObject(TH_40001676_4));
            //TH4_json.Add("TH_40001676_5", JsonConvert.SerializeObject(TH_40001676_5));
            //}
            //_hubContext.Clients.All.sendTH4(TH4_json);

            ////五楼温湿度数据
            //JObject TH5_json = new JObject();  
            //using (var KY_db = new kongyadbEntities())
            //{
            //var TH_40000938_1 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "1") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_2 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "2") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_3 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "3") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_4 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "4") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_5 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "5") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_6 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "6") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_7 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "7") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_8 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "8") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_9 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "9") select m).OrderByDescending(p => p.id).FirstOrDefault();    
            //var TH_40000938_10 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "10") select m).OrderByDescending(p => p.id).FirstOrDefault();  
            //var TH_40000938_11 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "11") select m).OrderByDescending(p => p.id).FirstOrDefault();  
            //var TH_40000938_12 = (from m in db.THhistory where (m.DeviceID == "40000938" && m.NodeID == "12") select m).OrderByDescending(p => p.id).FirstOrDefault();      
            //TH5_json.Add("TH_400938_1", JsonConvert.SerializeObject(TH_40000938_1));
            //TH5_json.Add("TH_400938_2", JsonConvert.SerializeObject(TH_40000938_2));
            //TH5_json.Add("TH_400938_3", JsonConvert.SerializeObject(TH_40000938_3));
            //TH5_json.Add("TH_400938_4", JsonConvert.SerializeObject(TH_40000938_4));
            //TH5_json.Add("TH_400938_5", JsonConvert.SerializeObject(TH_40000938_5));
            //TH5_json.Add("TH_400938_6", JsonConvert.SerializeObject(TH_40000938_6));
            //TH5_json.Add("TH_400938_7", JsonConvert.SerializeObject(TH_40000938_7));
            //TH5_json.Add("TH_400938_8", JsonConvert.SerializeObject(TH_40000938_8));
            //TH5_json.Add("TH_400938_9", JsonConvert.SerializeObject(TH_40000938_9));
            //TH5_json.Add("TH_400938_10", JsonConvert.SerializeObject(TH_40000938_10));
            //TH5_json.Add("TH_400938_11", JsonConvert.SerializeObject(TH_40000938_11));
            //TH5_json.Add("TH_400938_12", JsonConvert.SerializeObject(TH_40000938_12));
            //}
            //_hubContext.Clients.All.sendTH5(TH5_json);

            ////六楼温湿度数据
            //JObject TH6_json = new JObject();   
            //using (var KY_db = new kongyadbEntities())
            //{
            //var TH6_room = (from m in db.room select m).OrderByDescending(p => p.id).FirstOrDefault();
            //TH6_json.Add("room6", JsonConvert.SerializeObject(TH6_room));
            //}
            //_hubContext.Clients.All.sendTH6(TH6_json);
            #endregion
        }

        public static BroadcasterCheck Instance
        {
            get
            {
                return _instance.Value;
            }
        }


        #region    ---------------------空压房通信异常邮件通知---------------------------------------------
        public class EmailParameterSet
        {
            /// <summary>
            /// 收件人的邮件地址 
            /// </summary>
            public string ConsigneeAddress { get; set; }

            /// <summary>
            /// 收件人的名称
            /// </summary>
            public string ConsigneeName { get; set; }

            /// <summary>
            /// 收件人标题
            /// </summary>
            public string ConsigneeHand { get; set; }

            /// <summary>
            /// 收件人的主题
            /// </summary>
            public string ConsigneeTheme { get; set; }

            /// <summary>
            /// 发件邮件服务器的Smtp设置
            /// </summary>
            public string SendSetSmtp { get; set; }

            /// <summary>
            /// 发件人的邮件
            /// </summary>
            public string SendEmail { get; set; }

            /// <summary>
            /// 发件人的邮件密码
            /// </summary>
            public string SendPwd { get; set; }
            /// <summary>
            /// 发件内容
            /// </summary>
            public string SendContent { get; set; }
        }

        public bool MailSend(EmailParameterSet EPSModel)
        {
            try
            {
                //确定smtp服务器端的地址，实列化一个客户端smtp 
                SmtpClient sendSmtpClient = new SmtpClient(EPSModel.SendSetSmtp);//发件人的邮件服务器地址
                                                                                 //构造一个发件的人的地址
                MailAddress sendMailAddress = new MailAddress(EPSModel.SendEmail, EPSModel.ConsigneeHand, Encoding.UTF8);//发件人的邮件地址和收件人的标题、编码

                //构造一个收件的人的地址
                MailAddress consigneeMailAddress = new MailAddress(EPSModel.ConsigneeAddress, EPSModel.ConsigneeName, Encoding.UTF8);//收件人的邮件地址和收件人的名称 和编码

                //构造一个Email对象
                MailMessage mailMessage = new MailMessage(sendMailAddress, consigneeMailAddress);//发件地址和收件地址
                mailMessage.Subject = EPSModel.ConsigneeTheme;//邮件的主题
                mailMessage.BodyEncoding = Encoding.UTF8;//编码
                mailMessage.SubjectEncoding = Encoding.UTF8;//编码
                mailMessage.Body = EPSModel.SendContent;//发件内容
                mailMessage.IsBodyHtml = false;//获取或者设置指定邮件正文是否为html

                //设置邮件信息 (指定如何处理待发的电子邮件)
                sendSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定如何发邮件 是以网络来发
                sendSmtpClient.EnableSsl = false;//服务器支持安全接连，安全则为true

                sendSmtpClient.UseDefaultCredentials = false;//是否随着请求一起发

                //用户登录信息
                NetworkCredential myCredential = new NetworkCredential(EPSModel.SendEmail, EPSModel.SendPwd);
                sendSmtpClient.Credentials = myCredential;//登录
                sendSmtpClient.Send(mailMessage);//发邮件
                return true;//发送成功
            }
            catch (Exception)
            {
                return false;//发送失败
            }
        }


        private void SendEmail(object sender, EventArgs e)
        {
            EmailParameterSet model = new EmailParameterSet();
            model.SendEmail = "3490212659@qq.com";
            model.SendPwd = "ABC@123.";//密码
            model.SendSetSmtp = "smtp.qq.com";//发送的SMTP服务地址 ，每个邮箱的是不一样的。。根据发件人的邮箱来定
            model.ConsigneeAddress = "250389538@qq.com";
            model.ConsigneeTheme = "主题";
            model.ConsigneeHand = "标题";
            model.ConsigneeName = "xxx";
            model.SendContent = "htpp://www.baidu.com";
            //if (MailSend(model) == true)
            //{
            //    MessageBox.Show("邮件发送成功！");
            //}
            //else
            //{
            //    MessageBox.Show("邮件发送失败！");
            //}
        }

        private void SendEmail(int a)
        {
            EmailParameterSet model = new EmailParameterSet();
            model.SendEmail = "3490212659@qq.com";
            model.SendPwd = "ABC@123.";//密码
            model.SendSetSmtp = "smtp.qq.com";//发送的SMTP服务地址 ，每个邮箱的是不一样的。。根据发件人的邮箱来定
            model.ConsigneeAddress = "250389538@qq.com";
            model.ConsigneeTheme = "空压机已经" + a / 60 + "分钟没有新值，请检查采集程序及空压房弱电箱情况！";
            model.ConsigneeHand = "空压机已经" + a / 60 + "分钟没有新值，请检查采集程序及空压房弱电箱情况！";
            model.ConsigneeName = "管理员";
            model.SendContent = "htpp://www.baidu.com";
            if (MailSend(model) == true)
            {
                //MessageBox.Show("邮件发送成功！");
            }
            else
            {
                //MessageBox.Show("邮件发送失败！");
            }
        }

        private void SendEmail(string a)
        {
            EmailParameterSet model = new EmailParameterSet();
            model.SendEmail = "3490212659@qq.com";
            model.SendPwd = "ABC@123.";//密码
            model.SendSetSmtp = "smtp.qq.com";//发送的SMTP服务地址 ，每个邮箱的是不一样的。。根据发件人的邮箱来定
            model.ConsigneeAddress = "250389538@qq.com";
            model.ConsigneeTheme = "空压机已经连续" + a  + "没有新值，请检查采集程序及空压房弱电箱情况！";
            model.ConsigneeHand = "空压机已经连续" + a  + "没有新值，请检查采集程序及空压房弱电箱情况！";
            model.ConsigneeName = "管理员";
            model.SendContent = "空压机已经" + a + "没有新值，请检查采集程序及空压房弱电箱情况！";
            if (MailSend(model) == true)
            {
                //MessageBox.Show("邮件发送成功！");
            }
            else
            {
                //MessageBox.Show("邮件发送失败！");
            }
        }

        private void SendEmail2(string a)
        {
            var errorMessage = "发送失败";
            try
            {
                WebMail.SmtpServer = "smtp.qq.com";
                WebMail.SmtpPort = 25;//25;//端口号，不同SMTP服务器可能不同，可以查一下
                WebMail.EnableSsl = false;//禁用SSL
                WebMail.UserName = "3490212659";
                WebMail.Password = "ABC@123.";
                WebMail.From = "3490212659@qq.com";
                WebMail.Send("250389538@qq.com;3490212659@qq.com", "空压机房异常", "空压机已经连续" + a + "没有新值，请检查采集程序及空压房弱电箱情况！");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        #endregion

    }
}