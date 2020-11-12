using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static JianHeMES.Controllers.CommonalityController;
using JianHeMES.AuthAttributes;

namespace JianHeMES.Controllers
{
    public class Personnel_RosterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController common = new CommonalityController();


        #region-----------------LeavedPersonServiceLength离职人员工龄统计表
        public ActionResult LeavedPersonServiceLength()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Roster", act = "LeavedPersonServiceLength" });
            }
            return View();
        }


        [HttpPost]
        public ActionResult LeavedPersonServiceLength(int? year, int? month)
        {
            JObject result = new JObject();
            JObject table1 = new JObject();
            JObject table2 = new JObject();
            int time = 0;
            if (month == 0)
            {
                DateTime dateTime = DateTime.Now;
                if (year == dateTime.Year)
                {
                    time = dateTime.Month;
                }
                else
                {
                    time = 12;
                }
            }

            var departmentlist = new List<Personnel_Architecture>();
            CommonController date1 = new CommonController();
            DateTime exdate = new DateTime((int)year, (int)month == 0 ? time : (int)month, 28, 0, 0, 0);
            departmentlist = date1.CompanyDatetime(exdate);
            DateTime inputDate = new DateTime((int)year, (int)month == 0 ? time : (int)month, 1);
            if ((inputDate.Year - DateTime.Now.Year) * 12 + (inputDate.Month - DateTime.Now.Month) > 0)
            {
                return Content("输入的年月份无效！");
            }
            List<Personnel_Roster> AllRecordList = new List<Personnel_Roster>();
            if (year != 0 && month != 0)
            {
                AllRecordList = db.Personnel_Roster.Where(c => c.LastDate != null && c.LastDate.Value.Year == year && c.LastDate.Value.Month == month).ToList();//按年月查找
            }
            else if (year != 0 && time != 0)
            {
                AllRecordList = db.Personnel_Roster.Where(c => c.LastDate != null && c.LastDate.Value.Year == year && c.LastDate.Value.Month <= time).ToList();//年汇总
            }
            //总人数
            var total = AllRecordList.Count();
            List<int> lessthan = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 24, 36, 48, 60, 72, 84, 96, 108, 120 };
            foreach (var item in departmentlist)
            {
                List<string> record1 = new List<string>();
                List<string> record2 = new List<string>();//操作族
                List<string> record3 = new List<string>();//技术族
                List<string> record4 = new List<string>();//专业族
                List<string> record5 = new List<string>();//服务族
                List<string> record6 = new List<string>();//管理族

                #region
                //record1.Add(item);

                ////<=1个月
                //var craftsman_DP_1month = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) < 30);
                //var not_craftsman_DP_1month = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) < 30);
                //record1.Add((craftsman_DP_1month+not_craftsman_DP_1month).ToString());
                ////record2.Add(item + "匠师傅");
                //record2.Add(craftsman_DP_1month.ToString());
                ////record3.Add(item + "非匠师傅");
                //record3.Add(not_craftsman_DP_1month.ToString());

                ////1-2月
                //var craftsman_DP_2month = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 30 && TwoDT_sub(c.LastDate, c.HireDate)<60);
                //var not_craftsman_DP_2month = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 30 && TwoDT_sub(c.LastDate, c.HireDate) < 60);
                //record1.Add((craftsman_DP_2month+not_craftsman_DP_2month).ToString());
                //record2.Add(craftsman_DP_2month.ToString());
                //record3.Add(not_craftsman_DP_2month.ToString());

                ////2-3月
                //var craftsman_DP_3month = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 60 && TwoDT_sub(c.LastDate, c.HireDate) < 90);
                //var not_craftsman_DP_3month = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 60 && TwoDT_sub(c.LastDate, c.HireDate) < 90);
                //record1.Add((craftsman_DP_3month + not_craftsman_DP_3month).ToString());
                //record2.Add(craftsman_DP_3month.ToString());
                //record3.Add(not_craftsman_DP_3month.ToString());

                ////3-4月
                //var craftsman_DP_4month = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 90 && TwoDT_sub(c.LastDate, c.HireDate) < 120);
                //var not_craftsman_DP_4month = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 90 && TwoDT_sub(c.LastDate, c.HireDate) < 120);
                //record1.Add((craftsman_DP_4month + not_craftsman_DP_4month).ToString());
                //record2.Add(craftsman_DP_4month.ToString());
                //record3.Add(not_craftsman_DP_4month.ToString());

                ////4-5月
                //var craftsman_DP_5month = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 120 && TwoDT_sub(c.LastDate, c.HireDate) < 150);
                //var not_craftsman_DP_5month = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 120 && TwoDT_sub(c.LastDate, c.HireDate) < 150);
                //record1.Add((craftsman_DP_5month + not_craftsman_DP_5month).ToString());
                //record2.Add(craftsman_DP_5month.ToString());
                //record3.Add(not_craftsman_DP_5month.ToString());

                ////5-6月
                //var craftsman_DP_6month = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 150 && TwoDT_sub(c.LastDate, c.HireDate) < 180);
                //var not_craftsman_DP_6month = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 150 && TwoDT_sub(c.LastDate, c.HireDate) < 180);
                //record1.Add((craftsman_DP_6month + not_craftsman_DP_6month).ToString());
                //record2.Add(craftsman_DP_6month.ToString());
                //record3.Add(not_craftsman_DP_6month.ToString());

                ////6-7月
                //var craftsman_DP_7month = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 180 && TwoDT_sub(c.LastDate, c.HireDate) < 210);
                //var not_craftsman_DP_7month = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 180 && TwoDT_sub(c.LastDate, c.HireDate) < 210);
                //record1.Add((craftsman_DP_7month + not_craftsman_DP_7month).ToString());
                //record2.Add(craftsman_DP_7month.ToString());
                //record3.Add(not_craftsman_DP_7month.ToString());

                ////7-8月
                //var craftsman_DP_8month = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 210 && TwoDT_sub(c.LastDate, c.HireDate) < 240);
                //var not_craftsman_DP_8month = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 210 && TwoDT_sub(c.LastDate, c.HireDate) < 240);
                //record1.Add((craftsman_DP_8month + not_craftsman_DP_8month).ToString());
                //record2.Add(craftsman_DP_8month.ToString());
                //record3.Add(not_craftsman_DP_8month.ToString());

                ////8-9月
                //var craftsman_DP_9month = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 240 && TwoDT_sub(c.LastDate, c.HireDate) < 270);
                //var not_craftsman_DP_9month = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 240 && TwoDT_sub(c.LastDate, c.HireDate) < 270);
                //record1.Add((craftsman_DP_9month + not_craftsman_DP_9month).ToString());
                //record2.Add(craftsman_DP_9month.ToString());
                //record3.Add(not_craftsman_DP_9month.ToString());

                ////9-10月
                //var craftsman_DP_10month = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 270 && TwoDT_sub(c.LastDate, c.HireDate) < 300);
                //var not_craftsman_DP_10month = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 270 && TwoDT_sub(c.LastDate, c.HireDate) < 300);
                //record1.Add((craftsman_DP_10month + not_craftsman_DP_10month).ToString());
                //record2.Add(craftsman_DP_10month.ToString());
                //record3.Add(not_craftsman_DP_10month.ToString());

                ////10-11月
                //var craftsman_DP_11month = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 300 && TwoDT_sub(c.LastDate, c.HireDate) < 330);
                //var not_craftsman_DP_11month = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 300 && TwoDT_sub(c.LastDate, c.HireDate) < 330);
                //record1.Add((craftsman_DP_11month + not_craftsman_DP_11month).ToString());
                //record2.Add(craftsman_DP_11month.ToString());
                //record3.Add(not_craftsman_DP_11month.ToString());

                ////11-12月
                //var craftsman_DP_12month = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 330 && TwoDT_sub(c.LastDate, c.HireDate) < 365);
                //var not_craftsman_DP_12month = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 330 && TwoDT_sub(c.LastDate, c.HireDate) < 365);
                //record1.Add((craftsman_DP_12month + not_craftsman_DP_12month).ToString());
                //record2.Add(craftsman_DP_12month.ToString());
                //record3.Add(not_craftsman_DP_12month.ToString());

                ////1-2年
                //var craftsman_DP_1year = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 365 && TwoDT_sub(c.LastDate, c.HireDate) < 730);
                //var not_craftsman_DP_1year = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 365 && TwoDT_sub(c.LastDate, c.HireDate) < 730);
                //record1.Add((craftsman_DP_1year + not_craftsman_DP_1year).ToString());
                //record2.Add(craftsman_DP_1year.ToString());
                //record3.Add(not_craftsman_DP_1year.ToString());

                ////2-3年
                //var craftsman_DP_2year = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 730 && TwoDT_sub(c.LastDate, c.HireDate) < 1095);
                //var not_craftsman_DP_2year = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 730 && TwoDT_sub(c.LastDate, c.HireDate) < 1095);
                //record1.Add((craftsman_DP_2year + not_craftsman_DP_2year).ToString());
                //record2.Add(craftsman_DP_2year.ToString());
                //record3.Add(not_craftsman_DP_2year.ToString());

                ////3-4年
                //var craftsman_DP_3year = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 1095 && TwoDT_sub(c.LastDate, c.HireDate) < 1460);
                //var not_craftsman_DP_3year = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 1095 && TwoDT_sub(c.LastDate, c.HireDate) < 1460);
                //record1.Add((craftsman_DP_3year + not_craftsman_DP_3year).ToString());
                //record2.Add(craftsman_DP_3year.ToString());
                //record3.Add(not_craftsman_DP_3year.ToString());

                ////4-5年
                //var craftsman_DP_4year = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 1460 && TwoDT_sub(c.LastDate, c.HireDate) < 1825);
                //var not_craftsman_DP_4year = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 1460 && TwoDT_sub(c.LastDate, c.HireDate) < 1825);
                //record1.Add((craftsman_DP_4year + not_craftsman_DP_4year).ToString());
                //record2.Add(craftsman_DP_4year.ToString());
                //record3.Add(not_craftsman_DP_1year.ToString());

                ////5-6年
                //var craftsman_DP_5year = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 1825 && TwoDT_sub(c.LastDate, c.HireDate) < 2190);
                //var not_craftsman_DP_5year = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 1825 && TwoDT_sub(c.LastDate, c.HireDate) < 2190);
                //record1.Add((craftsman_DP_5year + not_craftsman_DP_5year).ToString());
                //record2.Add(craftsman_DP_5year.ToString());
                //record3.Add(not_craftsman_DP_5year.ToString());

                ////6-7年
                //var craftsman_DP_6year = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 2190 && TwoDT_sub(c.LastDate, c.HireDate) < 2555);
                //var not_craftsman_DP_6year = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 2190 && TwoDT_sub(c.LastDate, c.HireDate) < 2555);
                //record1.Add((craftsman_DP_6year + not_craftsman_DP_6year).ToString());
                //record2.Add(craftsman_DP_6year.ToString());
                //record3.Add(not_craftsman_DP_6year.ToString());

                ////7-8年
                //var craftsman_DP_7year = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 2555 && TwoDT_sub(c.LastDate, c.HireDate) < 2920);
                //var not_craftsman_DP_7year = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 2555 && TwoDT_sub(c.LastDate, c.HireDate) < 2920);
                //record1.Add((craftsman_DP_7year + not_craftsman_DP_7year).ToString());
                //record2.Add(craftsman_DP_7year.ToString());
                //record3.Add(not_craftsman_DP_7year.ToString());

                ////8-9年
                //var craftsman_DP_8year = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 2920 && TwoDT_sub(c.LastDate, c.HireDate) < 3285);
                //var not_craftsman_DP_8year = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 2920 && TwoDT_sub(c.LastDate, c.HireDate) < 3285);
                //record1.Add((craftsman_DP_8year + not_craftsman_DP_8year).ToString());
                //record2.Add(craftsman_DP_8year.ToString());
                //record3.Add(not_craftsman_DP_8year.ToString());

                ////9-10年
                //var craftsman_DP_9year = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 3285 && TwoDT_sub(c.LastDate, c.HireDate) < 3650);
                //var not_craftsman_DP_9year = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 3285 && TwoDT_sub(c.LastDate, c.HireDate) < 3650);
                //record1.Add((craftsman_DP_9year + not_craftsman_DP_9year).ToString());
                //record2.Add(craftsman_DP_9year.ToString());
                //record3.Add(not_craftsman_DP_9year.ToString());

                ////10年以上
                //var craftsman_DP_10year = AllRecordList.Count(c => c.Department == item && c.levelType == "匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 3650);
                //var not_craftsman_DP_10year = AllRecordList.Count(c => c.Department == item && c.levelType == "非匠师傅" && TwoDT_sub(c.LastDate, c.HireDate) >= 3650);
                //record1.Add((craftsman_DP_10year + not_craftsman_DP_10year).ToString());
                //record2.Add(craftsman_DP_10year.ToString());
                //record3.Add(not_craftsman_DP_10year.ToString());

                //record2.Add(item + "匠师傅");
                //record3.Add(item + "非匠师傅");
                #endregion

                for (int i = 0; i < lessthan.Count; i++)
                {
                    if (lessthan[i] == 1)
                    {
                        //操作族小于一个月
                        var craftsman_DP_month = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]);
                        ////非操作组小于一个月
                        //var not_craftsman_DP_month = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]);
                        //技术族小于一个月
                        var technology_month = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "技术族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]);
                        //专业族小于一个月
                        var professional_month = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "专业族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]);
                        //服务族小于一个月
                        var service_month = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "服务族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]);
                        //管理族小于一个月
                        var management_month = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "管理族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]);
                        //部门小于一个月
                        record1.Add((craftsman_DP_month + technology_month + professional_month + service_month + management_month).ToString());
                        //record2.Add(item + "操作族");
                        record2.Add(craftsman_DP_month.ToString());
                        ////record3.Add(item + "非操作族");
                        //record3.Add(not_craftsman_DP_month.ToString());
                        //技术族
                        record3.Add(technology_month.ToString());
                        //专业族
                        record4.Add(professional_month.ToString());
                        //服务族
                        record5.Add(service_month.ToString());
                        //管理族
                        record6.Add(management_month.ToString());
                    }
                    else if (lessthan[i] == 120)
                    {
                        var craftsman_DP_month3 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]);
                        //var not_craftsman_DP_month3 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]);
                        //技术族
                        var technology_month3 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "技术族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]);
                        //专业族
                        var professional_month3 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "专业族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]);
                        //服务族
                        var service_month3 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "服务族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]);
                        //管理族
                        var management_month3 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "管理族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]);
                        record1.Add((craftsman_DP_month3 + technology_month3 + professional_month3 + service_month3 + management_month3).ToString());
                        //record2.Add(item + "操作族");
                        record2.Add(craftsman_DP_month3.ToString());
                        ////record3.Add(item + "非操作族");
                        //record3.Add(not_craftsman_DP_month3.ToString());
                        //技术族
                        record3.Add(technology_month3.ToString());
                        //专业族
                        record4.Add(professional_month3.ToString());
                        //服务族
                        record5.Add(service_month3.ToString());
                        //管理族
                        record6.Add(management_month3.ToString());

                        break;
                    }
                    var craftsman_DP_month2 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]);
                    //var not_craftsman_DP_month2 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]);
                    //技术族
                    var technology_month2 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "技术族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]);
                    //专业族
                    var professional_month2 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "专业族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]);
                    //服务族
                    var service_month2 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "服务族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]);
                    //管理族
                    var management_month2 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "管理族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]);
                    record1.Add((craftsman_DP_month2 + technology_month2 + professional_month2 + service_month2 + management_month2).ToString());
                    //record2.Add(item + "操作族");
                    record2.Add(craftsman_DP_month2.ToString());
                    //record3.Add(item + "非操作族");
                    //record3.Add(not_craftsman_DP_month2.ToString());
                    //技术族
                    record3.Add(technology_month2.ToString());
                    //专业族
                    record4.Add(professional_month2.ToString());
                    //服务族
                    record5.Add(service_month2.ToString());
                    //管理族
                    record6.Add(management_month2.ToString());

                }

                //表1
                //部门小计
                int sum1 = 0;
                foreach (var it in record1)
                {
                    sum1 = sum1 + Convert.ToInt32(it.ToString());
                }
                record1.Add(sum1.ToString());
                //占比
                record1.Add((Convert.ToDecimal(record1.LastOrDefault()) * 100 / total).ToString("F2") + "%");
                //人事日报表记录item部门的记录
                var Year_Month_Allrecord = db.Personnel_daily.Where(c => c.Date.Value.Year == year && c.Date.Value.Month == month).ToList();
                int countByDepartment = Year_Month_Allrecord.Count(c => c.Department == item.Department);
                //月初人数
                var begin_day_of_month = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == item.Department).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel + Year_Month_Allrecord.Where(c => c.Department == item.Department).OrderBy(c => c.Date).FirstOrDefault().Today_on_board_employees;
                //月末人数
                var end_day_of_month = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == item.Department).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel + Year_Month_Allrecord.Where(c => c.Department == item.Department).OrderByDescending(c => c.Date).FirstOrDefault().Today_on_board_employees;
                //部门实际人数(月初+月末平均人数)
                var actualPerson = (begin_day_of_month + end_day_of_month) / 2;
                record1.Add(actualPerson.ToString());
                //占比2
                record1.Add(actualPerson == 0 ? "0.00" : (Convert.ToDecimal(sum1) * 100 / actualPerson).ToString("F2") + "%");

                //表2
                //总计
                int sum2 = 0;
                foreach (var it in record2)//操作族
                {
                    sum2 = sum2 + Convert.ToInt32(it.ToString());
                }
                record2.Add(sum2.ToString());

                int sum3 = 0;
                foreach (var it in record3)//技术族
                {
                    sum3 = sum3 + Convert.ToInt32(it.ToString());
                }
                record3.Add(sum3.ToString());

                int sum4 = 0;
                foreach (var it in record4)//专业族
                {
                    sum4 = sum4 + Convert.ToInt32(it.ToString());
                }
                record4.Add(sum4.ToString());

                int sum5 = 0;
                foreach (var it in record5)//服务族
                {
                    sum5 = sum5 + Convert.ToInt32(it.ToString());
                }
                record5.Add(sum5.ToString());

                int sum6 = 0;
                foreach (var it in record6)//管理族
                {
                    sum6 = sum6 + Convert.ToInt32(it.ToString());
                }
                record6.Add(sum4.ToString());

                //占比
                record2.Add(actualPerson == 0 ? "0.00" : (Convert.ToDecimal(sum2) * 100 / total).ToString("F2") + "%");//操作族
                record3.Add(actualPerson == 0 ? "0.00" : (Convert.ToDecimal(sum3) * 100 / total).ToString("F2") + "%");//技术族
                record4.Add(actualPerson == 0 ? "0.00" : (Convert.ToDecimal(sum4) * 100 / total).ToString("F2") + "%");//专业族
                record5.Add(actualPerson == 0 ? "0.00" : (Convert.ToDecimal(sum5) * 100 / total).ToString("F2") + "%");//服务族
                record6.Add(actualPerson == 0 ? "0.00" : (Convert.ToDecimal(sum6) * 100 / total).ToString("F2") + "%");//管理族

                //表1
                table1.Add(item.Department, JsonConvert.SerializeObject(record1));
                //表2
                table2.Add(item.Department + "," + "操作族", JsonConvert.SerializeObject(record2));//操作族
                table2.Add(item.Department + "," + "技术族", JsonConvert.SerializeObject(record3));//技术族
                table2.Add(item.Department + "," + "专业族", JsonConvert.SerializeObject(record4));//专业族
                table2.Add(item.Department + "," + "服务族", JsonConvert.SerializeObject(record5));//服务族
                table2.Add(item.Department + "," + "管理族", JsonConvert.SerializeObject(record6));//管理族
                table2.Add(item.Department + "," + "小计", JsonConvert.SerializeObject(record1));

            }
            //表1：时段小计、占比 
            List<int> littleSum = new List<int>();
            List<string> percetnSum = new List<string>();

            //表2：总计、操作族总计、操作族占比、技术族总计、技术族占比、专业族总计、专业族占比、服务族总计、服务族占比、管理族总计、管理族占比
            List<int> craftsmanSum = new List<int>();//操作族总计
            //List<int> not_craftsmanSum = new List<int>();//非操作族总计
            List<string> craftsmanPercent = new List<string>();//操作族占比
            //List<string> not_craftsmanPercent = new List<string>();//非操作族占比
            List<string> zongbi = new List<string>();//总计
            List<int> technologySum = new List<int>();//技术族总计
            List<string> technologyPercent = new List<string>();//技术族占比
            List<int> professionalSum = new List<int>();//专业族总计
            List<string> professionalPercent = new List<string>();//专业族占比
            List<int> serviceSum = new List<int>();//服务族总计
            List<string> servicePercent = new List<string>();//服务族占比
            List<int> managementSum = new List<int>();//管理族总计
            List<string> managementPercent = new List<string>();//管理族占比

            for (int i = 0; i < lessthan.Count; i++)
            {
                if (lessthan[i] == 1)
                {
                    //表1
                    littleSum.Add(AllRecordList.Count(c => common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]));

                    //var percent = Convert.ToDecimal(sum) * 100 / total;
                    percetnSum.Add((Convert.ToDecimal(AllRecordList.Count(c => common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //表2                  
                    craftsmanSum.Add(AllRecordList.Count(c => c.levelType == "操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])); //操作族总计
                                                                                                                                                        //not_craftsmanSum.Add(AllRecordList.Count(c => c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]));                 
                    technologySum.Add(AllRecordList.Count(c => c.levelType == "技术族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]));//技术族总计                    
                    professionalSum.Add(AllRecordList.Count(c => c.levelType == "专业族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]));//专业族总计                   
                    serviceSum.Add(AllRecordList.Count(c => c.levelType == "服务族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])); //服务族总计                    
                    managementSum.Add(AllRecordList.Count(c => c.levelType == "管理族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]));//管理族总计

                    //操作族占比
                    craftsmanPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //not_craftsmanPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //技术族占比
                    technologyPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "技术族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //专业族占比
                    professionalPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "专业族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //服务族占比
                    servicePercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "服务族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //管理族占比
                    managementPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "管理族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //总计
                    zongbi.Add((Convert.ToDecimal(AllRecordList.Count(c => common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");

                }
                else if (lessthan[i] == 120)
                {
                    //表1
                    littleSum.Add(AllRecordList.Count(c => common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]));
                    //var percent = Convert.ToDecimal(sum) * 100 / total;
                    percetnSum.Add((Convert.ToDecimal(AllRecordList.Count(c => common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]) * 100 / total)).ToString("F2") + "%");
                    //表2
                    craftsmanSum.Add(AllRecordList.Count(c => c.levelType == "操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]));
                    //not_craftsmanSum.Add(AllRecordList.Count(c => c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]));
                    technologySum.Add(AllRecordList.Count(c => c.levelType == "技术族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]));//技术族总计                    
                    professionalSum.Add(AllRecordList.Count(c => c.levelType == "专业族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]));//专业族总计                   
                    serviceSum.Add(AllRecordList.Count(c => c.levelType == "服务族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])); //服务族总计                    
                    managementSum.Add(AllRecordList.Count(c => c.levelType == "管理族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]));//管理族总计

                    craftsmanPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //not_craftsmanPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //技术族占比
                    technologyPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "技术族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //专业族占比
                    professionalPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "专业族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //服务族占比
                    servicePercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "服务族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //管理族占比
                    managementPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "管理族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])) * 100 / total).ToString("#0.00") + "%");

                    zongbi.Add((Convert.ToDecimal(AllRecordList.Count(c => common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]) * 100 / total)).ToString("F2") + "%");
                    break;
                }
                //表1
                littleSum.Add(AllRecordList.Count(c => common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]));
                percetnSum.Add((Convert.ToDecimal(AllRecordList.Count(c => common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");
                //表2
                craftsmanSum.Add(AllRecordList.Count(c => c.levelType == "操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]));
                //not_craftsmanSum.Add(AllRecordList.Count(c => c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]));
                technologySum.Add(AllRecordList.Count(c => c.levelType == "技术族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]));//技术族总计                    
                professionalSum.Add(AllRecordList.Count(c => c.levelType == "专业族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]));//专业族总计                   
                serviceSum.Add(AllRecordList.Count(c => c.levelType == "服务族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])); //服务族总计                    
                managementSum.Add(AllRecordList.Count(c => c.levelType == "管理族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]));//管理族总计

                craftsmanPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");
                //not_craftsmanPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");
                //技术族占比
                technologyPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "技术族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");
                //专业族占比
                professionalPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "专业族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");
                //服务族占比
                servicePercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "服务族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");
                //管理族占比
                managementPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "管理族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");

                zongbi.Add((Convert.ToDecimal(AllRecordList.Count(c => common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");

            }
            //操作族总计
            var craftsmanTotalcount = AllRecordList.Count(c => c.levelType == "操作族");
            //非操作族总计
            //var not_craftsmanTotalcount = AllRecordList.Count(c => c.levelType == "非操作族");
            var technologyTotalcount = AllRecordList.Count(c => c.levelType == "技术族");//技术族总计
            var professionalTotalcount = AllRecordList.Count(c => c.levelType == "专业族");//专业族总计
            var serviceTotalcount = AllRecordList.Count(c => c.levelType == "服务族");//服务族总计
            var managementTotalcount = AllRecordList.Count(c => c.levelType == "管理族");//管理族总计

            //操作族总占比
            var craftsmanTotalPercent = (Convert.ToDecimal(craftsmanTotalcount) * 100 / total).ToString("#0.00") + "%";
            //非操作族总占比
            // var not_craftsmanTotalPercent = (Convert.ToDecimal(not_craftsmanTotalcount) * 100 / total).ToString("#0.00") + "%";
            var technologyTotalPercent = (Convert.ToDecimal(technologyTotalcount) * 100 / total).ToString("#0.00") + "%";//技术族总占比
            var professionalTotalPercent = (Convert.ToDecimal(professionalTotalcount) * 100 / total).ToString("#0.00") + "%";//专业族总占比
            var serviceTotalPercen = (Convert.ToDecimal(serviceTotalcount) * 100 / total).ToString("#0.00") + "%";//服务族总占比
            var managementTotalPercen = (Convert.ToDecimal(managementTotalcount) * 100 / total).ToString("#0.00") + "%";//管理族总占比
            //总占比
            var ZongBi = ((craftsmanTotalcount + technologyTotalcount + professionalTotalcount + serviceTotalcount + managementTotalcount) * 100 / total).ToString("#0.00") + "%";

            littleSum.Add(total);
            craftsmanSum.Add(craftsmanTotalcount);//操作族总计
            //not_craftsmanSum.Add(not_craftsmanTotalcount);           
            //not_craftsmanPercent.Add(not_craftsmanTotalPercent);
            technologySum.Add(technologyTotalcount);//技术族总计
            professionalSum.Add(professionalTotalcount);//专业族总计
            serviceSum.Add(serviceTotalcount);//服务族总计
            managementSum.Add(managementTotalcount);//管理族总计
            craftsmanPercent.Add(craftsmanTotalPercent); //操作族总占比
            technologyPercent.Add(technologyTotalPercent);//技术族总占比
            professionalPercent.Add(professionalTotalPercent);//专业族总占比
            servicePercent.Add(serviceTotalPercen);//服务族总占比
            managementPercent.Add(managementTotalPercen);//管理族总占比
            zongbi.Add(ZongBi); //总占比

            table1.Add("时段小计", JsonConvert.SerializeObject(littleSum));
            table1.Add("占比", JsonConvert.SerializeObject(percetnSum));

            //表2的总计和表1的时段小计是一样的

            table2.Add("操作族总计,操作族总计", JsonConvert.SerializeObject(craftsmanSum));//操作族总计
            //table2.Add("非操作族总计,非操作族总计", JsonConvert.SerializeObject(not_craftsmanSum));
            table2.Add("技术族总计,技术族总计", JsonConvert.SerializeObject(technologySum));//技术族总计
            table2.Add("专业族总计,专业族总计", JsonConvert.SerializeObject(professionalSum));//专业族总计
            table2.Add("服务族总计,服务族总计", JsonConvert.SerializeObject(serviceSum));//服务族总计
            table2.Add("管理族总计,管理族总计", JsonConvert.SerializeObject(managementSum));//管理族总计
            table2.Add("总计,总计", JsonConvert.SerializeObject(littleSum));

            table2.Add("操作族占比,操作族占比", JsonConvert.SerializeObject(craftsmanPercent));
            //table2.Add("非操作族占比,非操作族占比", JsonConvert.SerializeObject(not_craftsmanPercent));
            table2.Add("技术族占比,技术族占比", JsonConvert.SerializeObject(technologyPercent));//技术族占比
            table2.Add("专业族占比,专业族占比", JsonConvert.SerializeObject(professionalPercent));//专业族占比
            table2.Add("服务族占比,服务族占比", JsonConvert.SerializeObject(servicePercent));//服务族占比
            table2.Add("管理族占比,管理族占比", JsonConvert.SerializeObject(managementPercent));//管理族占比
            table2.Add("总占比,总占比", JsonConvert.SerializeObject(zongbi));

            result.Add("date", year + "-" + month);
            result.Add("表1", table1);
            result.Add("表2", table2);
            return Content(JsonConvert.SerializeObject(result));
        }


        public ActionResult DefaualInfo()
        {
            var lastTime = db.Personnel_Roster.OrderByDescending(c => c.LastDate).Select(c => c.LastDate).FirstOrDefault();
            int year = lastTime.Value.Year;
            int month = lastTime.Value.Month;

            var result = LeavedPersonServiceLength(year, month);
            return result;
        }
        public int TwoDTforMonth_sub(DateTime? date2, DateTime? date1) //默认date2>date1
        {
            if (date1 != null && date2 != null)
            {
                var d1_lastday = DateTime.DaysInMonth(Convert.ToDateTime(date1).Year, Convert.ToDateTime(date1).Month);
                var d2_lastday = DateTime.DaysInMonth(Convert.ToDateTime(date2).Year, Convert.ToDateTime(date2).Month);

                var d1_day = Convert.ToDateTime(date1).Day;
                var d2_day = Convert.ToDateTime(date2).Day;
                int monthsum = 0;
                if ((d1_lastday == d1_day && d2_lastday == d2_day) || (d1_lastday != d1_day && d2_lastday == d2_day) || (d2_day >= d1_day))
                {
                    if (Convert.ToDateTime(date2).Month > Convert.ToDateTime(date1).Month)
                    {
                        monthsum = (Convert.ToDateTime(date2).Year - Convert.ToDateTime(date1).Year) * 12 + (Convert.ToDateTime(date2).Month - Convert.ToDateTime(date1).Month);
                    }
                    else
                    {
                        monthsum = (Convert.ToDateTime(date2).Year - Convert.ToDateTime(date1).Year - 1) * 12 + (Convert.ToDateTime(date2).Month + 12 - Convert.ToDateTime(date1).Month);
                    }
                }
                else if ((d1_lastday == d1_day && d2_lastday != d2_day) || (d2_day <= d1_day))
                {
                    if (Convert.ToDateTime(date2).Month > Convert.ToDateTime(date1).Month)
                    {
                        monthsum = (Convert.ToDateTime(date2).Year - Convert.ToDateTime(date1).Year) * 12 + (Convert.ToDateTime(date2).Month - Convert.ToDateTime(date1).Month) - 1;
                    }
                    else
                    {
                        monthsum = (Convert.ToDateTime(date2).Year - Convert.ToDateTime(date1).Year - 1) * 12 + (Convert.ToDateTime(date2).Month + 12 - Convert.ToDateTime(date1).Month) - 1;
                    }
                }

                return monthsum;
            }
            return 0;
        }

        #endregion


        #region--------首页 Index
        public async Task<ActionResult> Index(string status, string department, string type, DateTime? date)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Roster", act = "Index" });
            }
            JObject result = new JObject();
            JObject count = new JObject();
            //ViewBag.departmentList = departmentList();
            //ViewBag.status = Sataus();
            var recordList = await db.Personnel_Roster.ToListAsync();
            var dplist = recordList.Select(c => c.Department).Distinct().ToList();
            foreach (var dp in dplist)
            {
                var dp_grouplist = recordList.Where(c => c.Department == dp).Select(c => c.DP_Group).Distinct().ToList();
                JObject detail = new JObject();
                detail.Add(dp + "在职总人数", recordList.Where(c => c.Department == dp && c.Status != "离职员工").Count());
                foreach (var dg in dp_grouplist)
                {
                    detail.Add(dg, recordList.Count(c => c.Department == dp && c.DP_Group == dg));
                }
                var educationlist = recordList.Where(c => c.Department == dp).Select(c => c.Education).Distinct().ToList();
                foreach (var ed in educationlist)
                {
                    detail.Add(ed, recordList.Count(c => c.Department == dp && c.Education == ed));
                }
                result.Add(dp, detail);
            }
            if (!String.IsNullOrEmpty(status))
            {
                recordList = recordList.Where(c => c.Status == status).ToList();
            }
            if (!String.IsNullOrEmpty(department))
            {
                recordList = recordList.Where(c => c.Department == department).ToList();
            }
            if (!String.IsNullOrEmpty(type))
            {
                recordList = recordList.Where(c => c.levelType == type).ToList();
            }
            if (date != null && date.Value.Year != 0 && date.Value.Month != 0)
            {
                var LastDateList = recordList.Where(c => c.LastDate != null).ToList();
                recordList = LastDateList.Where(c => c.LastDate.Value.Year == date.Value.Year && c.LastDate.Value.Month == date.Value.Month).ToList();
            }
            if (date != null && date.Value.Year != 0 && date.Value.Month == 0)
            {
                recordList = recordList.Where(c => c.LastDate.Value.Year == date.Value.Year).ToList();
            }
            count.Add("正式员工", db.Personnel_Roster.Count(c => c.Status == "正式员工"));
            count.Add("试用员工", db.Personnel_Roster.Count(c => c.Status == "试用员工"));
            count.Add("离职员工", db.Personnel_Roster.Count(c => c.Status == "离职员工"));
            count.Add("操作族", db.Personnel_Roster.Count(c => c.levelType == "操作族"));
            // count.Add("非操作族", db.Personnel_Roster.Count(c => c.levelType == "非操作族"));
            count.Add("技术族", db.Personnel_Roster.Count(c => c.levelType == "技术族"));
            count.Add("专业族", db.Personnel_Roster.Count(c => c.levelType == "专业族"));
            count.Add("服务族", db.Personnel_Roster.Count(c => c.levelType == "服务族"));
            count.Add("管理族", db.Personnel_Roster.Count(c => c.levelType == "管理族"));
            result.Add("总统计", count);
            ViewBag.statistics = result;
            ViewBag.jobNum = null;
            ViewBag.name = null;
            recordList.ForEach(c => { c.OnPostMonth = Convert.ToDecimal(c.LastDate == null ? common.TwoDTforMonth_sub(DateTime.Now, c.HireDate).ToString() : common.TwoDTforMonth_sub(c.LastDate, c.HireDate).ToString()); });
            return View(recordList);

            //if (Session["User"] == null)
            //{
            //    return RedirectToAction("Login", "Users", new { col = "Personnel_Roster", act = "Index" });
            //}
            ////ViewBag.departmentList = departmentList();
            ////ViewBag.status = Sataus();
            //JObject result = new JObject();
            //JObject count = new JObject();
            //count.Add("正式员工", await db.Personnel_Roster.CountAsync(c => c.Status == "正式员工"));
            //count.Add("试用员工", await db.Personnel_Roster.CountAsync(c => c.Status == "试用员工"));
            //count.Add("离职员工", await db.Personnel_Roster.CountAsync(c => c.Status == "离职员工"));
            //count.Add("操作族", await db.Personnel_Roster.CountAsync(c => c.levelType == "操作族"));
            //// count.Add("非操作族", await db.Personnel_Roster.CountAsync(c => c.levelType == "非操作族"));
            //count.Add("技术族", await db.Personnel_Roster.CountAsync(c => c.levelType == "技术族"));
            //count.Add("专业族", await db.Personnel_Roster.CountAsync(c => c.levelType == "专业族"));
            //count.Add("服务族", await db.Personnel_Roster.CountAsync(c => c.levelType == "服务族"));
            //count.Add("管理族", await db.Personnel_Roster.CountAsync(c => c.levelType == "管理族"));
            ////count.Add("目前在岗员工", await db.Personnel_Roster.CountAsync(c => c.Status == "正式员工") + await db.Personnel_Roster.CountAsync(c => c.Status == "试用员工"));
            //result.Add("总统计", count);
            //var dplist = await db.Personnel_Roster.Select(c => c.Department).Distinct().ToListAsync();
            //foreach (var dp in dplist)
            //{
            //    var dp_grouplist = await db.Personnel_Roster.Where(c => c.Department == dp).Select(c => c.DP_Group).Distinct().ToListAsync();
            //    JObject detail = new JObject();
            //    detail.Add(dp + "在职总人数", await db.Personnel_Roster.Where(c => c.Department == dp && c.Status != "离职员工").CountAsync());
            //    foreach (var dg in dp_grouplist)
            //    {
            //        detail.Add(dg == null ? "空组" : dg, await db.Personnel_Roster.CountAsync(c => c.Department == dp && c.DP_Group == dg && c.Status != "离职员工"));
            //    }
            //    var educationlist = await db.Personnel_Roster.Where(c => c.Department == dp).Select(c => c.Education).Distinct().ToListAsync();
            //    foreach (var ed in educationlist)
            //    {
            //        detail.Add(ed == null ? "空学历" : ed, await db.Personnel_Roster.CountAsync(c => c.Department == dp && c.Education == ed && c.Status != "离职员工"));
            //    }
            //    detail.Add("正式员工", await db.Personnel_Roster.CountAsync(c => c.Department == dp && c.Status == "正式员工"));
            //    detail.Add("试用员工", await db.Personnel_Roster.CountAsync(c => c.Department == dp && c.Status == "试用员工"));
            //    detail.Add("操作族", await db.Personnel_Roster.CountAsync(c => c.Department == dp && c.levelType == "操作族" && c.Status != "离职员工"));
            //    //detail.Add("非操作族", await db.Personnel_Roster.CountAsync(c => c.Department == dp && c.levelType == "非操作族" && c.Status != "离职员工"));
            //    detail.Add("技术族", await db.Personnel_Roster.CountAsync(c => c.Department == dp && c.levelType == "技术族" && c.Status != "离职员工"));
            //    detail.Add("专业族", await db.Personnel_Roster.CountAsync(c => c.Department == dp && c.levelType == "专业族" && c.Status != "离职员工"));
            //    detail.Add("服务族", await db.Personnel_Roster.CountAsync(c => c.Department == dp && c.levelType == "服务族" && c.Status != "离职员工"));
            //    detail.Add("管理族", await db.Personnel_Roster.CountAsync(c => c.Department == dp && c.levelType == "管理族" && c.Status != "离职员工"));
            //    result.Add(dp == null ? "没有数据列" : dp, detail);
            //}
            //ViewBag.statistics = result;
            //ViewBag.jobNum = null;
            //ViewBag.name = null;
            //var resultdata = await db.Personnel_Roster.ToListAsync();
            //resultdata.ForEach(c => { c.OnPostMonth = Convert.ToDecimal(c.LastDate == null ? common.TwoDTforMonth_sub(DateTime.Now, c.HireDate).ToString() : common.TwoDTforMonth_sub(c.LastDate, c.HireDate).ToString()); });
            //return View(resultdata);
        }

        [HttpPost]
        public async Task<ActionResult> Index(string jobNum, string name, string status, string department, string type, DateTime? date)
        {
            JObject result = new JObject();
            JObject count = new JObject();
            //ViewBag.departmentList = departmentList();
            //ViewBag.status = Sataus();
            var recordList = await db.Personnel_Roster.ToListAsync();
            var dplist = recordList.Select(c => c.Department).Distinct().ToList();
            foreach (var dp in dplist)
            {
                var dp_grouplist = recordList.Where(c => c.Department == dp).Select(c => c.DP_Group).Distinct().ToList();
                JObject detail = new JObject();
                detail.Add(dp + "在职总人数", recordList.Where(c => c.Department == dp && c.Status != "离职员工").Count());
                foreach (var dg in dp_grouplist)
                {
                    detail.Add(dg, recordList.Count(c => c.Department == dp && c.DP_Group == dg));
                }
                var educationlist = recordList.Where(c => c.Department == dp).Select(c => c.Education).Distinct().ToList();
                foreach (var ed in educationlist)
                {
                    detail.Add(ed, recordList.Count(c => c.Department == dp && c.Education == ed));
                }
                result.Add(dp, detail);
            }
            if (!String.IsNullOrEmpty(jobNum))
            {
                recordList = recordList.Where(c => c.JobNum == jobNum).ToList();
            }
            if (!String.IsNullOrEmpty(name))
            {
                recordList = recordList.Where(c => c.Name.Contains(name)).ToList();
            }
            if (!String.IsNullOrEmpty(status))
            {
                recordList = recordList.Where(c => c.Status == status).ToList();
            }
            if (!String.IsNullOrEmpty(department))
            {
                recordList = recordList.Where(c => c.Department == department).ToList();
            }
            if (!String.IsNullOrEmpty(type))
            {
                recordList = recordList.Where(c => c.levelType == type).ToList();
            }
            if (date != null && date.Value.Year != 0 && date.Value.Month != 0)
            {
                var LastDateList = recordList.Where(c => c.LastDate != null).ToList();
                recordList = LastDateList.Where(c => c.LastDate.Value.Year == date.Value.Year && c.LastDate.Value.Month == date.Value.Month).ToList();
            }
            if (date != null && date.Value.Year != 0 && date.Value.Month == 0)
            {
                recordList = recordList.Where(c => c.LastDate.Value.Year == date.Value.Year).ToList();
            }
            count.Add("正式员工", db.Personnel_Roster.Count(c => c.Status == "正式员工"));
            count.Add("试用员工", db.Personnel_Roster.Count(c => c.Status == "试用员工"));
            count.Add("离职员工", db.Personnel_Roster.Count(c => c.Status == "离职员工"));
            count.Add("操作族", db.Personnel_Roster.Count(c => c.levelType == "操作族"));
            // count.Add("非操作族", db.Personnel_Roster.Count(c => c.levelType == "非操作族"));
            count.Add("技术族", db.Personnel_Roster.Count(c => c.levelType == "技术族"));
            count.Add("专业族", db.Personnel_Roster.Count(c => c.levelType == "专业族"));
            count.Add("服务族", db.Personnel_Roster.Count(c => c.levelType == "服务族"));
            count.Add("管理族", db.Personnel_Roster.Count(c => c.levelType == "管理族"));
            result.Add("总统计", count);
            ViewBag.statistics = result;
            ViewBag.jobNum = jobNum;
            ViewBag.name = name;
            recordList.ForEach(c => { c.OnPostMonth = Convert.ToDecimal(c.LastDate == null ? common.TwoDTforMonth_sub(DateTime.Now, c.HireDate).ToString() : common.TwoDTforMonth_sub(c.LastDate, c.HireDate).ToString()); });
            return View(recordList);
        }
        #endregion


        #region  -------------导出花名册记录输出Excel表格方法------------------

        public class ResultToExcel
        {
            //工号
            public string JobNum { get; set; }
            //中文名
            public string Name { get; set; }
            //性别
            public string Sex { get; set; }
            //出生日期
            public string DateOfBirth { get; set; }
            //学历
            public string Education { get; set; }
            //组名
            public string DP_Group { get; set; }
            //职位名称
            public string Position { get; set; }
            //入司时间
            public string HireDate { get; set; }
            //最后工作日
            public string LastDate { get; set; }
            //一级部门
            public string Department { get; set; }
            //员工状态
            public string Status { get; set; }
            //族群
            public string levelType { get; set; }
            //在岗月数
            public string OnPostMonth { get; set; }
        }


        [HttpPost]
        public FileContentResult OutputExcel(string jobNum, string name, string status, string department)
        {
            var recordList = db.Personnel_Roster.ToList();
            if (!String.IsNullOrEmpty(jobNum))
            {
                recordList = recordList.Where(c => c.JobNum == jobNum).ToList();
            }
            if (!String.IsNullOrEmpty(name))
            {
                recordList = recordList.Where(c => c.Name == name).ToList();
            }
            if (!String.IsNullOrEmpty(status))
            {
                recordList = recordList.Where(c => c.Status == status).ToList();
            }
            if (!String.IsNullOrEmpty(department))
            {
                recordList = recordList.Where(c => c.Department == department).ToList();
            }
            List<ResultToExcel> Resultlist = new List<ResultToExcel>();
            if (recordList != null)
            {
                foreach (var item in recordList)
                {
                    ResultToExcel at = new ResultToExcel();
                    at.JobNum = item.JobNum;
                    at.Name = item.Name;
                    at.Sex = item.Sex == false ? "女" : "男";
                    at.DateOfBirth = item.DateOfBirth == null ? " " : string.Format("{0:yyyy-MM-dd}", item.DateOfBirth);
                    at.Education = item.Education;
                    at.DP_Group = item.DP_Group;
                    at.Position = item.Position;
                    at.HireDate = item.HireDate == null ? " " : string.Format("{0:yyyy-MM-dd HH:mm}", item.HireDate);
                    at.LastDate = item.LastDate == null ? " " : string.Format("{0:yyyy-MM-dd HH:mm}", item.LastDate);
                    at.Department = item.Department;
                    at.Status = item.Status;
                    at.levelType = item.levelType;
                    at.OnPostMonth = item.LastDate == null ? common.TwoDTforMonth_sub(DateTime.Now, item.HireDate).ToString() : common.TwoDTforMonth_sub(item.LastDate, item.HireDate).ToString();
                    Resultlist.Add(at);
                }
            }
            //导出表格名称
            string tableName = "花名册记录表";
            if (!String.IsNullOrEmpty(jobNum) || !String.IsNullOrEmpty(name) || !String.IsNullOrEmpty(status) || !String.IsNullOrEmpty(department))
            {
                tableName = tableName + "(按";
                tableName = tableName + (String.IsNullOrEmpty(jobNum) == true ? "" : "工号、");
                tableName = tableName + (String.IsNullOrEmpty(name) == true ? "" : "姓名、");
                tableName = tableName + (String.IsNullOrEmpty(status) == true ? "" : "员工状态、");
                tableName = tableName + (String.IsNullOrEmpty(department) == true ? "" : "部门、");
                tableName = tableName.Substring(0, tableName.Length - 1);
                tableName = tableName + "查找)";
            }
            if (Resultlist.Count() > 0)
            {
                string[] columns = { "工号", "姓名", "性别", "出生日期", "学历", "组名", "职位名称", "入司时间", "最后工作日期", "部门", "员工状态", "族群", "在岗月数" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
            }
            else
            {
                ResultToExcel at1 = new ResultToExcel();
                at1.JobNum = "没有找到相关记录！";
                Resultlist.Add(at1);
                string[] columns = { "工号", "姓名", "性别", "出生日期", "学历", "组名", "职位名称", "入司时间", "最后工作日期", "部门", "员工状态", "族群", "在岗月数" };
                byte[] filecontent = ExcelExportHelper.ExportExcel(Resultlist, tableName, false, columns);
                return File(filecontent, ExcelExportHelper.ExcelContentType, tableName + ".xlsx");
            }
        }
        #endregion

        #region--------批量添加员工
        public ActionResult Batch_InputStaff()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Roster", act = "Batch_InputStaff" });
            }
            return View();

        }

        [HttpPost]
        public ActionResult Batch_InputStaff(List<Personnel_Roster> inputList)
        {
            string repeat = null;
            foreach (var item in inputList)
            {
                item.CreateDate = DateTime.Now;
                item.Creator = ((Users)Session["User"]).UserName;
                if (db.Personnel_Roster.Count(c => c.JobNum == item.JobNum) != 0)
                    repeat = repeat + item.JobNum + ",";
            }
            if (!string.IsNullOrEmpty(repeat))
            {
                return Content(repeat + "已经有相同的数据，请重新输入");
            }
            if (ModelState.IsValid)
            {
                db.Personnel_Roster.AddRange(inputList);
                db.SaveChangesAsync();
                return Content("添加" + inputList.Count.ToString() + "员工成功");
            }
            #region 测试modelstate 为什么为false
            //else
            //{
            //    //获取所有错误的Key
            //    List<string> Keys = ModelState.Keys.ToList();
            //    //获取每一个key对应的ModelStateDictionary
            //    foreach (var key in Keys)
            //    {
            //        var errors = ModelState[key].Errors.ToList();
            //        //将错误描述输出到控制台
            //        foreach (var error in errors)
            //        {
            //            Console.WriteLine(error.ErrorMessage);
            //        }
            //    }
            //}
            #endregion
            return Content("添加失败");
        }

        #endregion


        #region--------其他方法
        // GET: Personnel_Roster


        // GET: Personnel_Roster/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Roster personnel_Roster = await db.Personnel_Roster.FindAsync(id);
            if (personnel_Roster == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Roster);
        }

        // GET: Personnel_Roster/Create
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Roster", act = "Create" });
            }
            return View();

        }

        // POST: Personnel_Roster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,JobNum,Name,Sex,DateOfBirth,Education,DP_Group,Position,HireDate,LastDate,Department,Status,levelType,OnPostMonth")] Personnel_Roster personnel_Roster)
        {
            if (db.Personnel_Roster.Count(c => c.JobNum == personnel_Roster.JobNum) != 0)
                return Content(personnel_Roster.JobNum + "已经有相同的数据，请重新输入");

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(personnel_Roster.JobNum) && !string.IsNullOrEmpty(personnel_Roster.Name) && !string.IsNullOrEmpty(personnel_Roster.Education) && !string.IsNullOrEmpty(personnel_Roster.DP_Group) && !string.IsNullOrEmpty(personnel_Roster.Position) && !string.IsNullOrEmpty(personnel_Roster.Department))
                {
                    personnel_Roster.CreateDate = DateTime.Now;
                    personnel_Roster.Creator = ((Users)Session["User"]).UserName;
                    db.Personnel_Roster.Add(personnel_Roster);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return Content("请输入正确的数据");
            }

            return View(personnel_Roster);
        }

        // GET: Personnel_Roster/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Roster personnel_Roster = await db.Personnel_Roster.FindAsync(id);
            if (personnel_Roster == null)
            {
                return HttpNotFound();
            }
            ViewBag.status = Sataus();
            ViewBag.statusV = personnel_Roster.Status;
            return View(personnel_Roster);
        }

        public async Task<ActionResult> currentPagrEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Roster personnel_Roster = await db.Personnel_Roster.FindAsync(id);
            if (personnel_Roster == null)
            {
                return HttpNotFound();
            }
            personnel_Roster.OnPostMonth = personnel_Roster.OnPostMonth == 0 ? common.TwoDTforMonth_sub(DateTime.Now, personnel_Roster.HireDate) : personnel_Roster.OnPostMonth;
            return Content(JsonConvert.SerializeObject(personnel_Roster));
        }

        //修改方法（有在使用）
        [HttpPost]
        public async Task<ActionResult> updateDate([Bind(Include = "Id,JobNum,Name,Sex,DateOfBirth,Education,DP_Group,Position,HireDate,LastDate,Department,Status,levelType,OnPostMonth")] Personnel_Roster personnel_Roster)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(personnel_Roster.JobNum) && !string.IsNullOrEmpty(personnel_Roster.Name) && !string.IsNullOrEmpty(personnel_Roster.Education) && !string.IsNullOrEmpty(personnel_Roster.DP_Group) && !string.IsNullOrEmpty(personnel_Roster.Position) && !string.IsNullOrEmpty(personnel_Roster.Department))
                {
                    db.Entry(personnel_Roster).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    int WorkNumber = int.Parse(personnel_Roster.JobNum);
                    var department = db.Users.Where(c => c.UserNum == WorkNumber && c.UserName == personnel_Roster.Name).ToList();
                    foreach (var it in department)
                    {
                        it.Department = personnel_Roster.Department;
                        db.Entry(it).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Content("ok");
                }
            }
            return Content("false");
        }

        // POST: Personnel_Roster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,JobNum,Name,Sex,DateOfBirth,Education,DP_Group,Position,HireDate,LastDate,Department,Status,levelType,OnPostMonth")] Personnel_Roster personnel_Roster)
        {
            ViewBag.status = Sataus();
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(personnel_Roster.JobNum) && !string.IsNullOrEmpty(personnel_Roster.Name) && !string.IsNullOrEmpty(personnel_Roster.Education) && !string.IsNullOrEmpty(personnel_Roster.DP_Group) && !string.IsNullOrEmpty(personnel_Roster.Position) && !string.IsNullOrEmpty(personnel_Roster.Department))
                {
                    db.Entry(personnel_Roster).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(personnel_Roster);
        }

        // GET: Personnel_Roster/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Roster personnel_Roster = await db.Personnel_Roster.FindAsync(id);
            if (personnel_Roster == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Roster);
        }

        // POST: Personnel_Roster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Personnel_Roster personnel_Roster = await db.Personnel_Roster.FindAsync(id);
            db.Personnel_Roster.Remove(personnel_Roster);
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

        #region------------计算两个日期相差几年几个月

        /// <summary>
        /// 计算日期的间隔(静态类)
        /// </summary>
        public static class dateTimeDiff
        {
            /// <summary>
            /// 计算日期间隔
            /// </summary>
            /// <param name="d1">要参与计算的其中一个日期字符串</param>
            /// <param name="d2">要参与计算的另一个日期字符串</param>
            /// <returns>一个表示日期间隔的TimeSpan类型</returns>
            public static TimeSpan toResult(string d1, string d2)
            {
                try
                {
                    DateTime date1 = DateTime.Parse(d1);
                    DateTime date2 = DateTime.Parse(d2);
                    return toResult(date1, date2);
                }
                catch
                {
                    throw new Exception("字符串参数不正确!");
                }
            }
            /// <summary>
            /// 计算日期间隔
            /// </summary>
            /// <param name="d1">要参与计算的其中一个日期</param>
            /// <param name="d2">要参与计算的另一个日期</param>
            /// <returns>一个表示日期间隔的TimeSpan类型</returns>
            public static TimeSpan toResult(DateTime d1, DateTime d2)
            {
                TimeSpan ts;
                if (d1 > d2)
                {
                    ts = d1 - d2;
                }
                else
                {
                    ts = d2 - d1;
                }
                return ts;
            }

            /// <summary>
            /// 计算日期间隔
            /// </summary>
            /// <param name="d1">要参与计算的其中一个日期字符串</param>
            /// <param name="d2">要参与计算的另一个日期字符串</param>
            /// <param name="drf">决定返回值形式的枚举</param>
            /// <returns>一个代表年月日的int数组，具体数组长度与枚举参数drf有关</returns>
            public static int[] toResult(string d1, string d2, diffResultFormat drf)
            {
                try
                {
                    DateTime date1 = DateTime.Parse(d1);
                    DateTime date2 = DateTime.Parse(d2);
                    return toResult(date1, date2, drf);
                }
                catch
                {
                    throw new Exception("字符串参数不正确!");
                }
            }
            /// <summary>
            /// 计算日期间隔
            /// </summary>
            /// <param name="d1">要参与计算的其中一个日期</param>
            /// <param name="d2">要参与计算的另一个日期</param>
            /// <param name="drf">决定返回值形式的枚举</param>
            /// <returns>一个代表年月日的int数组，具体数组长度与枚举参数drf有关</returns>
            public static int[] toResult(DateTime d1, DateTime d2, diffResultFormat drf)
            {
                #region 数据初始化
                DateTime max;
                DateTime min;
                int year;
                int month;
                int tempYear, tempMonth;
                if (d1 > d2)
                {
                    max = d1;
                    min = d2;
                }
                else
                {
                    max = d2;
                    min = d1;
                }
                tempYear = max.Year;
                tempMonth = max.Month;
                if (max.Month < min.Month)
                {
                    tempYear--;
                    tempMonth = tempMonth + 12;
                }
                year = tempYear - min.Year;
                month = tempMonth - min.Month;
                #endregion
                #region 按条件计算
                if (drf == diffResultFormat.dd)
                {
                    TimeSpan ts = max - min;
                    return new int[] { ts.Days };
                }
                if (drf == diffResultFormat.mm)
                {
                    return new int[] { month + year * 12 };
                }
                if (drf == diffResultFormat.yy)
                {
                    return new int[] { year };
                }
                return new int[] { year, month };
                #endregion
            }
        }
        /// <summary>
        /// 关于返回值形式的枚举
        /// </summary>
        public enum diffResultFormat
        {
            /// <summary>
            /// 年数和月数
            /// </summary>
            yymm,
            /// <summary>
            /// 年数
            /// </summary>
            yy,
            /// <summary>
            /// 月数
            /// </summary>
            mm,
            /// <summary>
            /// 天数
            /// </summary>
            dd,
        }
        #endregion

        #region ------------departmentList()取花名册部门列表
        private List<SelectListItem> departmentList()
        {
            var departmentlist = db.Personnel_Roster.Select(m => m.Department).Distinct();
            var items = new List<SelectListItem>();
            foreach (string department in departmentlist)
            {
                items.Add(new SelectListItem
                {
                    Text = department,
                    Value = department
                });
            }
            return items;
        }

        public ActionResult getDepartmentList()
        {
            var departmentlist = db.Personnel_Roster.Select(m => m.Department).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in departmentlist)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        #endregion

        #region --------------------正式/试用/离职
        private List<SelectListItem> Sataus()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "正式员工",
                    Value = "正式员工"
                },
                new SelectListItem
                {
                    Text = "试用员工",
                    Value = "试用员工"
                },
                new SelectListItem
                {
                    Text = "离职员工",
                    Value = "离职员工"
                }

            };
        }
        #endregion

    }

    //Api接口部分

    public class Personnel_Roster_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonController common = new CommonController();
        private CommonalityController commonality = new CommonalityController();


        #region-----------------离职员工工龄分布统计表

        [HttpPost]
        [ApiAuthorize]
        public JObject LeavedPersonServiceLength([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            JObject table1 = new JObject();
            JObject table2 = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int? year = obj.year;//年
            int? month = obj.month;//月
            int time = 0;
            if (month == 0)
            {
                DateTime dateTime = DateTime.Now;
                if (year == dateTime.Year)
                {
                    time = dateTime.Month;
                }
                else
                {
                    time = 12;
                }
            }
            var departmentlist = new List<Personnel_Architecture>();
            CommonController date1 = new CommonController();
            DateTime exdate = new DateTime((int)year, (int)month == 0 ? time : (int)month, 28, 0, 0, 0);
            departmentlist = date1.CompanyDatetime(exdate);
            DateTime inputDate = new DateTime((int)year, (int)month == 0 ? time : (int)month, 1);
            if ((inputDate.Year - DateTime.Now.Year) * 12 + (inputDate.Month - DateTime.Now.Month) > 0)
            {
                return common.GetModuleFromJobjet(result, false, "输入的年月份无效！");
            }
            List<Personnel_Roster> AllRecordList = new List<Personnel_Roster>();
            if (year != 0 && month != 0)
            {
                AllRecordList = db.Personnel_Roster.Where(c => c.LastDate != null && c.LastDate.Value.Year == year && c.LastDate.Value.Month == month).ToList();//按年月查找
            }
            else if (year != 0 && time != 0)
            {
                AllRecordList = db.Personnel_Roster.Where(c => c.LastDate != null && c.LastDate.Value.Year == year && c.LastDate.Value.Month <= time).ToList();//年汇总
            }
            //总人数
            var total = AllRecordList.Count();
            List<int> lessthan = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 24, 36, 48, 60, 72, 84, 96, 108, 120 };
            foreach (var item in departmentlist)
            {
                List<string> record1 = new List<string>();
                List<string> record2 = new List<string>();//操作族
                List<string> record3 = new List<string>();//技术族
                List<string> record4 = new List<string>();//专业族
                List<string> record5 = new List<string>();//服务族
                List<string> record6 = new List<string>();//管理族

                for (int i = 0; i < lessthan.Count; i++)
                {
                    if (lessthan[i] == 1)
                    {
                        //操作族小于一个月
                        var craftsman_DP_month = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "操作族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]);
                        ////非操作组小于一个月
                        //var not_craftsman_DP_month = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]);
                        //技术族小于一个月
                        var technology_month = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "技术族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]);
                        //专业族小于一个月
                        var professional_month = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "专业族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]);
                        //服务族小于一个月
                        var service_month = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "服务族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]);
                        //管理族小于一个月
                        var management_month = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "管理族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]);
                        //部门小于一个月
                        record1.Add((craftsman_DP_month + technology_month + professional_month + service_month + management_month).ToString());
                        //record2.Add(item + "操作族");
                        record2.Add(craftsman_DP_month.ToString());
                        ////record3.Add(item + "非操作族");
                        //record3.Add(not_craftsman_DP_month.ToString());
                        //技术族
                        record3.Add(technology_month.ToString());
                        //专业族
                        record4.Add(professional_month.ToString());
                        //服务族
                        record5.Add(service_month.ToString());
                        //管理族
                        record6.Add(management_month.ToString());
                    }
                    else if (lessthan[i] == 120)
                    {
                        var craftsman_DP_month3 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "操作族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]);
                        //var not_craftsman_DP_month3 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]);
                        //技术族
                        var technology_month3 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "技术族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]);
                        //专业族
                        var professional_month3 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "专业族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]);
                        //服务族
                        var service_month3 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "服务族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]);
                        //管理族
                        var management_month3 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "管理族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]);
                        record1.Add((craftsman_DP_month3 + technology_month3 + professional_month3 + service_month3 + management_month3).ToString());
                        //record2.Add(item + "操作族");
                        record2.Add(craftsman_DP_month3.ToString());
                        ////record3.Add(item + "非操作族");
                        //record3.Add(not_craftsman_DP_month3.ToString());
                        //技术族
                        record3.Add(technology_month3.ToString());
                        //专业族
                        record4.Add(professional_month3.ToString());
                        //服务族
                        record5.Add(service_month3.ToString());
                        //管理族
                        record6.Add(management_month3.ToString());

                        break;
                    }
                    var craftsman_DP_month2 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "操作族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]);
                    //var not_craftsman_DP_month2 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]);
                    //技术族
                    var technology_month2 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "技术族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]);
                    //专业族
                    var professional_month2 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "专业族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]);
                    //服务族
                    var service_month2 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "服务族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]);
                    //管理族
                    var management_month2 = AllRecordList.Count(c => c.Department == item.Department && c.levelType == "管理族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]);
                    record1.Add((craftsman_DP_month2 + technology_month2 + professional_month2 + service_month2 + management_month2).ToString());
                    //record2.Add(item + "操作族");
                    record2.Add(craftsman_DP_month2.ToString());
                    //record3.Add(item + "非操作族");
                    //record3.Add(not_craftsman_DP_month2.ToString());
                    //技术族
                    record3.Add(technology_month2.ToString());
                    //专业族
                    record4.Add(professional_month2.ToString());
                    //服务族
                    record5.Add(service_month2.ToString());
                    //管理族
                    record6.Add(management_month2.ToString());

                }

                //表1
                //部门小计
                int sum1 = 0;
                foreach (var it in record1)
                {
                    sum1 = sum1 + Convert.ToInt32(it.ToString());
                }
                record1.Add(sum1.ToString());
                //占比
                record1.Add((Convert.ToDecimal(record1.LastOrDefault()) * 100 / total).ToString("F2") + "%");
                //人事日报表记录item部门的记录
                var Year_Month_Allrecord = db.Personnel_daily.Where(c => c.Date.Value.Year == year && c.Date.Value.Month == month).ToList();
                int countByDepartment = Year_Month_Allrecord.Count(c => c.Department == item.Department);
                //月初人数
                var begin_day_of_month = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == item.Department).OrderBy(c => c.Date).FirstOrDefault().Employees_personnel + Year_Month_Allrecord.Where(c => c.Department == item.Department).OrderBy(c => c.Date).FirstOrDefault().Today_on_board_employees;
                //月末人数
                var end_day_of_month = countByDepartment == 0 ? 0 : Year_Month_Allrecord.Where(c => c.Department == item.Department).OrderByDescending(c => c.Date).FirstOrDefault().Employees_personnel + Year_Month_Allrecord.Where(c => c.Department == item.Department).OrderByDescending(c => c.Date).FirstOrDefault().Today_on_board_employees;
                //部门实际人数(月初+月末平均人数)
                var actualPerson = (begin_day_of_month + end_day_of_month) / 2;
                record1.Add(actualPerson.ToString());
                //占比2
                record1.Add(actualPerson == 0 ? "0.00" : (Convert.ToDecimal(sum1) * 100 / actualPerson).ToString("F2") + "%");

                //表2
                //总计
                int sum2 = 0;
                foreach (var it in record2)//操作族
                {
                    sum2 = sum2 + Convert.ToInt32(it.ToString());
                }
                record2.Add(sum2.ToString());

                int sum3 = 0;
                foreach (var it in record3)//技术族
                {
                    sum3 = sum3 + Convert.ToInt32(it.ToString());
                }
                record3.Add(sum3.ToString());

                int sum4 = 0;
                foreach (var it in record4)//专业族
                {
                    sum4 = sum4 + Convert.ToInt32(it.ToString());
                }
                record4.Add(sum4.ToString());

                int sum5 = 0;
                foreach (var it in record5)//服务族
                {
                    sum5 = sum5 + Convert.ToInt32(it.ToString());
                }
                record5.Add(sum5.ToString());

                int sum6 = 0;
                foreach (var it in record6)//管理族
                {
                    sum6 = sum6 + Convert.ToInt32(it.ToString());
                }
                record6.Add(sum4.ToString());

                //占比
                record2.Add(actualPerson == 0 ? "0.00" : (Convert.ToDecimal(sum2) * 100 / total).ToString("F2") + "%");//操作族
                record3.Add(actualPerson == 0 ? "0.00" : (Convert.ToDecimal(sum3) * 100 / total).ToString("F2") + "%");//技术族
                record4.Add(actualPerson == 0 ? "0.00" : (Convert.ToDecimal(sum4) * 100 / total).ToString("F2") + "%");//专业族
                record5.Add(actualPerson == 0 ? "0.00" : (Convert.ToDecimal(sum5) * 100 / total).ToString("F2") + "%");//服务族
                record6.Add(actualPerson == 0 ? "0.00" : (Convert.ToDecimal(sum6) * 100 / total).ToString("F2") + "%");//管理族

                //表1
                table1.Add(item.Department, JsonConvert.SerializeObject(record1));
                //表2
                table2.Add(item.Department + "," + "操作族", JsonConvert.SerializeObject(record2));//操作族
                table2.Add(item.Department + "," + "技术族", JsonConvert.SerializeObject(record3));//技术族
                table2.Add(item.Department + "," + "专业族", JsonConvert.SerializeObject(record4));//专业族
                table2.Add(item.Department + "," + "服务族", JsonConvert.SerializeObject(record5));//服务族
                table2.Add(item.Department + "," + "管理族", JsonConvert.SerializeObject(record6));//管理族
                table2.Add(item.Department + "," + "小计", JsonConvert.SerializeObject(record1));

            }
            //表1：时段小计、占比 
            List<int> littleSum = new List<int>();
            List<string> percetnSum = new List<string>();

            //表2：总计、操作族总计、操作族占比、技术族总计、技术族占比、专业族总计、专业族占比、服务族总计、服务族占比、管理族总计、管理族占比
            List<int> craftsmanSum = new List<int>();//操作族总计
            //List<int> not_craftsmanSum = new List<int>();//非操作族总计
            List<string> craftsmanPercent = new List<string>();//操作族占比
            //List<string> not_craftsmanPercent = new List<string>();//非操作族占比
            List<string> zongbi = new List<string>();//总计
            List<int> technologySum = new List<int>();//技术族总计
            List<string> technologyPercent = new List<string>();//技术族占比
            List<int> professionalSum = new List<int>();//专业族总计
            List<string> professionalPercent = new List<string>();//专业族占比
            List<int> serviceSum = new List<int>();//服务族总计
            List<string> servicePercent = new List<string>();//服务族占比
            List<int> managementSum = new List<int>();//管理族总计
            List<string> managementPercent = new List<string>();//管理族占比

            for (int i = 0; i < lessthan.Count; i++)
            {
                if (lessthan[i] == 1)
                {
                    //表1
                    littleSum.Add(AllRecordList.Count(c => commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]));

                    //var percent = Convert.ToDecimal(sum) * 100 / total;
                    percetnSum.Add((Convert.ToDecimal(AllRecordList.Count(c => commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //表2                  
                    craftsmanSum.Add(AllRecordList.Count(c => c.levelType == "操作族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])); //操作族总计
                                                                                                                                                             //not_craftsmanSum.Add(AllRecordList.Count(c => c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]));                 
                    technologySum.Add(AllRecordList.Count(c => c.levelType == "技术族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]));//技术族总计                    
                    professionalSum.Add(AllRecordList.Count(c => c.levelType == "专业族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]));//专业族总计                   
                    serviceSum.Add(AllRecordList.Count(c => c.levelType == "服务族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])); //服务族总计                    
                    managementSum.Add(AllRecordList.Count(c => c.levelType == "管理族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i]));//管理族总计

                    //操作族占比
                    craftsmanPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "操作族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //not_craftsmanPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //技术族占比
                    technologyPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "技术族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //专业族占比
                    professionalPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "专业族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //服务族占比
                    servicePercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "服务族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //管理族占比
                    managementPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "管理族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //总计
                    zongbi.Add((Convert.ToDecimal(AllRecordList.Count(c => commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i])) * 100 / total).ToString("#0.00") + "%");

                }
                else if (lessthan[i] == 120)
                {
                    //表1
                    littleSum.Add(AllRecordList.Count(c => commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]));
                    //var percent = Convert.ToDecimal(sum) * 100 / total;
                    percetnSum.Add((Convert.ToDecimal(AllRecordList.Count(c => commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]) * 100 / total)).ToString("F2") + "%");
                    //表2
                    craftsmanSum.Add(AllRecordList.Count(c => c.levelType == "操作族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]));
                    //not_craftsmanSum.Add(AllRecordList.Count(c => c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]));
                    technologySum.Add(AllRecordList.Count(c => c.levelType == "技术族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]));//技术族总计                    
                    professionalSum.Add(AllRecordList.Count(c => c.levelType == "专业族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]));//专业族总计                   
                    serviceSum.Add(AllRecordList.Count(c => c.levelType == "服务族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])); //服务族总计                    
                    managementSum.Add(AllRecordList.Count(c => c.levelType == "管理族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]));//管理族总计

                    craftsmanPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "操作族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //not_craftsmanPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //技术族占比
                    technologyPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "技术族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //专业族占比
                    professionalPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "专业族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //服务族占比
                    servicePercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "服务族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])) * 100 / total).ToString("#0.00") + "%");
                    //管理族占比
                    managementPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "管理族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i])) * 100 / total).ToString("#0.00") + "%");

                    zongbi.Add((Convert.ToDecimal(AllRecordList.Count(c => commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i]) * 100 / total)).ToString("F2") + "%");
                    break;
                }
                //表1
                littleSum.Add(AllRecordList.Count(c => commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]));
                percetnSum.Add((Convert.ToDecimal(AllRecordList.Count(c => commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");
                //表2
                craftsmanSum.Add(AllRecordList.Count(c => c.levelType == "操作族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]));
                //not_craftsmanSum.Add(AllRecordList.Count(c => c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]));
                technologySum.Add(AllRecordList.Count(c => c.levelType == "技术族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]));//技术族总计                    
                professionalSum.Add(AllRecordList.Count(c => c.levelType == "专业族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]));//专业族总计                   
                serviceSum.Add(AllRecordList.Count(c => c.levelType == "服务族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])); //服务族总计                    
                managementSum.Add(AllRecordList.Count(c => c.levelType == "管理族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1]));//管理族总计

                craftsmanPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "操作族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");
                //not_craftsmanPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "非操作族" && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && common.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");
                //技术族占比
                technologyPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "技术族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");
                //专业族占比
                professionalPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "专业族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");
                //服务族占比
                servicePercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "服务族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");
                //管理族占比
                managementPercent.Add((Convert.ToDecimal(AllRecordList.Count(c => c.levelType == "管理族" && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");

                zongbi.Add((Convert.ToDecimal(AllRecordList.Count(c => commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) >= lessthan[i] && commonality.TwoDTforMonth_sub(c.LastDate, c.HireDate) < lessthan[i + 1])) * 100 / total).ToString("#0.00") + "%");

            }
            //操作族总计
            var craftsmanTotalcount = AllRecordList.Count(c => c.levelType == "操作族");
            //非操作族总计
            //var not_craftsmanTotalcount = AllRecordList.Count(c => c.levelType == "非操作族");
            var technologyTotalcount = AllRecordList.Count(c => c.levelType == "技术族");//技术族总计
            var professionalTotalcount = AllRecordList.Count(c => c.levelType == "专业族");//专业族总计
            var serviceTotalcount = AllRecordList.Count(c => c.levelType == "服务族");//服务族总计
            var managementTotalcount = AllRecordList.Count(c => c.levelType == "管理族");//管理族总计

            //操作族总占比
            var craftsmanTotalPercent = (Convert.ToDecimal(craftsmanTotalcount) * 100 / total).ToString("#0.00") + "%";
            //非操作族总占比
            // var not_craftsmanTotalPercent = (Convert.ToDecimal(not_craftsmanTotalcount) * 100 / total).ToString("#0.00") + "%";
            var technologyTotalPercent = (Convert.ToDecimal(technologyTotalcount) * 100 / total).ToString("#0.00") + "%";//技术族总占比
            var professionalTotalPercent = (Convert.ToDecimal(professionalTotalcount) * 100 / total).ToString("#0.00") + "%";//专业族总占比
            var serviceTotalPercen = (Convert.ToDecimal(serviceTotalcount) * 100 / total).ToString("#0.00") + "%";//服务族总占比
            var managementTotalPercen = (Convert.ToDecimal(managementTotalcount) * 100 / total).ToString("#0.00") + "%";//管理族总占比
            //总占比
            var ZongBi = ((craftsmanTotalcount + technologyTotalcount + professionalTotalcount + serviceTotalcount + managementTotalcount) * 100 / total).ToString("#0.00") + "%";

            littleSum.Add(total);
            craftsmanSum.Add(craftsmanTotalcount);//操作族总计
            //not_craftsmanSum.Add(not_craftsmanTotalcount);           
            //not_craftsmanPercent.Add(not_craftsmanTotalPercent);
            technologySum.Add(technologyTotalcount);//技术族总计
            professionalSum.Add(professionalTotalcount);//专业族总计
            serviceSum.Add(serviceTotalcount);//服务族总计
            managementSum.Add(managementTotalcount);//管理族总计
            craftsmanPercent.Add(craftsmanTotalPercent); //操作族总占比
            technologyPercent.Add(technologyTotalPercent);//技术族总占比
            professionalPercent.Add(professionalTotalPercent);//专业族总占比
            servicePercent.Add(serviceTotalPercen);//服务族总占比
            managementPercent.Add(managementTotalPercen);//管理族总占比
            zongbi.Add(ZongBi); //总占比

            table1.Add("时段小计", JsonConvert.SerializeObject(littleSum));
            table1.Add("占比", JsonConvert.SerializeObject(percetnSum));

            //表2的总计和表1的时段小计是一样的

            table2.Add("操作族总计,操作族总计", JsonConvert.SerializeObject(craftsmanSum));//操作族总计
            //table2.Add("非操作族总计,非操作族总计", JsonConvert.SerializeObject(not_craftsmanSum));
            table2.Add("技术族总计,技术族总计", JsonConvert.SerializeObject(technologySum));//技术族总计
            table2.Add("专业族总计,专业族总计", JsonConvert.SerializeObject(professionalSum));//专业族总计
            table2.Add("服务族总计,服务族总计", JsonConvert.SerializeObject(serviceSum));//服务族总计
            table2.Add("管理族总计,管理族总计", JsonConvert.SerializeObject(managementSum));//管理族总计
            table2.Add("总计,总计", JsonConvert.SerializeObject(littleSum));

            table2.Add("操作族占比,操作族占比", JsonConvert.SerializeObject(craftsmanPercent));
            //table2.Add("非操作族占比,非操作族占比", JsonConvert.SerializeObject(not_craftsmanPercent));
            table2.Add("技术族占比,技术族占比", JsonConvert.SerializeObject(technologyPercent));//技术族占比
            table2.Add("专业族占比,专业族占比", JsonConvert.SerializeObject(professionalPercent));//专业族占比
            table2.Add("服务族占比,服务族占比", JsonConvert.SerializeObject(servicePercent));//服务族占比
            table2.Add("管理族占比,管理族占比", JsonConvert.SerializeObject(managementPercent));//管理族占比
            table2.Add("总占比,总占比", JsonConvert.SerializeObject(zongbi));

            result.Add("date", year + "-" + month);
            result.Add("表1", table1);
            result.Add("表2", table2);
            return common.GetModuleFromJobjet(result);
        }

        [HttpPost]
        [ApiAuthorize]
        public JObject DefaualInfo([System.Web.Http.FromBody]JObject data)
        {
            var lastTime = db.Personnel_Roster.OrderByDescending(c => c.LastDate).Select(c => c.LastDate).FirstOrDefault();
            int Year = lastTime.Value.Year;
            int Month = lastTime.Value.Month;
            var result = LeavedPersonServiceLength(data);
            return common.GetModuleFromJobjet(result);
        }

        [HttpPost]
        [ApiAuthorize]
        public int TwoDTforMonth_sub([System.Web.Http.FromBody]JObject data) //默认date2>date1
        {
            int result = 0;
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            DateTime? date2 = obj.date2;
            DateTime? date1 = obj.date1;
            if (date1 != null && date2 != null)
            {
                var d1_lastday = DateTime.DaysInMonth(Convert.ToDateTime(date1).Year, Convert.ToDateTime(date1).Month);
                var d2_lastday = DateTime.DaysInMonth(Convert.ToDateTime(date2).Year, Convert.ToDateTime(date2).Month);

                var d1_day = Convert.ToDateTime(date1).Day;
                var d2_day = Convert.ToDateTime(date2).Day;

                if ((d1_lastday == d1_day && d2_lastday == d2_day) || (d1_lastday != d1_day && d2_lastday == d2_day) || (d2_day >= d1_day))
                {
                    if (Convert.ToDateTime(date2).Month > Convert.ToDateTime(date1).Month)
                    {
                        result = (Convert.ToDateTime(date2).Year - Convert.ToDateTime(date1).Year) * 12 + (Convert.ToDateTime(date2).Month - Convert.ToDateTime(date1).Month);
                    }
                    else
                    {
                        result = (Convert.ToDateTime(date2).Year - Convert.ToDateTime(date1).Year - 1) * 12 + (Convert.ToDateTime(date2).Month + 12 - Convert.ToDateTime(date1).Month);
                    }
                }
                else if ((d1_lastday == d1_day && d2_lastday != d2_day) || (d2_day <= d1_day))
                {
                    if (Convert.ToDateTime(date2).Month > Convert.ToDateTime(date1).Month)
                    {
                        result = (Convert.ToDateTime(date2).Year - Convert.ToDateTime(date1).Year) * 12 + (Convert.ToDateTime(date2).Month - Convert.ToDateTime(date1).Month) - 1;
                    }
                    else
                    {
                        result = (Convert.ToDateTime(date2).Year - Convert.ToDateTime(date1).Year - 1) * 12 + (Convert.ToDateTime(date2).Month + 12 - Convert.ToDateTime(date1).Month) - 1;
                    }
                }

                return result;
            }
            return result;
        }

        #endregion

        #region--------花名册首页查询
        [HttpPost]
        [ApiAuthorize]
        public JObject Index([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            JArray depar = new JArray();
            JObject roster = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string jobNum = obj.jobNum;//工号
            string name = obj.name;//姓名
            string status = obj.status;//员工状态
            string department = obj.department;//部门
            string type = obj.type;//族群
            DateTime? dateTime = obj.dateTime;//离职年月
            var recordList = db.Personnel_Roster.ToList();
            var dplist = recordList.Select(c => c.Department).Distinct().ToList();
            foreach (var dp in dplist)
            {
                var dp_grouplist = recordList.Where(c => c.Department == dp).Select(c => c.DP_Group).Distinct().ToList();
                JObject detail = new JObject();
                detail.Add(dp + "在职总人数", recordList.Where(c => c.Department == dp && c.Status != "离职员工").Count());
                foreach (var dg in dp_grouplist)
                {
                    detail.Add(dg, recordList.Count(c => c.Department == dp && c.DP_Group == dg));
                }
                var educationlist = recordList.Where(c => c.Department == dp).Select(c => c.Education).Distinct().ToList();
                foreach (var ed in educationlist)
                {
                    detail.Add(ed, recordList.Count(c => c.Department == dp && c.Education == ed));
                }
                result.Add(dp, detail);
            }
            if (!String.IsNullOrEmpty(jobNum))
            {
                recordList = recordList.Where(c => c.JobNum == jobNum).ToList();
            }
            if (!String.IsNullOrEmpty(name))
            {
                recordList = recordList.Where(c => c.Name.Contains(name)).ToList();
            }
            if (!String.IsNullOrEmpty(status))
            {
                recordList = recordList.Where(c => c.Status == status).ToList();
            }
            if (!String.IsNullOrEmpty(department))
            {
                recordList = recordList.Where(c => c.Department == department).ToList();
            }
            if (!String.IsNullOrEmpty(type))
            {
                recordList = recordList.Where(c => c.levelType == type).ToList();
            }
            if (dateTime != null && dateTime.Value.Year != 0 && dateTime.Value.Month != 0)
            {
                var LastDateList = recordList.Where(c => c.LastDate != null).ToList();
                recordList = LastDateList.Where(c => c.LastDate.Value.Year == dateTime.Value.Year && c.LastDate.Value.Month == dateTime.Value.Month).ToList();
            }
            if (dateTime != null && dateTime.Value.Year != 0 && dateTime.Value.Month == 0)
            {
                recordList = recordList.Where(c => c.LastDate.Value.Year == dateTime.Value.Year).ToList();
            }
            if (recordList.Count > 0)
            {
                var userNumberList = recordList.Select(c => c.JobNum).Distinct().ToList();
                foreach (var item in userNumberList)
                {
                    var employees = recordList.Where(c => c.JobNum == item).FirstOrDefault();
                    if (employees != null)
                    {
                        roster.Add("Id", employees.Id);//id
                        roster.Add("JobNum", employees.JobNum);//工号
                        roster.Add("Name", employees.Name);//姓名
                        roster.Add("Sex", employees.Sex);//性别
                        roster.Add("DateOfBirth", employees.DateOfBirth);//出生年月
                        roster.Add("Education", employees.Education);//学历
                        roster.Add("DP_Group", employees.DP_Group);//班组名称
                        roster.Add("Position", employees.Position);//职位名称
                        roster.Add("HireDate", employees.HireDate);//入司时间
                        roster.Add("LastDate", employees.LastDate);//最后工作日期
                        roster.Add("Department", employees.Department);//部门
                        roster.Add("Status", employees.Status);//员工状态
                        roster.Add("LevelType", employees.levelType);//族群
                        if (employees.LastDate == null)
                        {
                            roster.Add("OnPostMonth", employees.OnPostMonth = Convert.ToDecimal(commonality.TwoDTforMonth_sub(DateTime.Now, employees.HireDate).ToString()));
                        }
                        else
                        {
                            roster.Add("OnPostMonth", Convert.ToDecimal(commonality.TwoDTforMonth_sub(employees.LastDate, employees.HireDate).ToString()));
                        }
                        depar.Add(roster);
                        roster = new JObject();
                    }
                }
                result.Add("Roster", depar);
            }
            return common.GetModuleFromJobjet(result);
        }
        #endregion

        #region--------批量添加员工
        [HttpPost]
        [ApiAuthorize]
        public JObject Batch_InputStaff([System.Web.Http.FromBody]JObject data)
        {
            JArray result = new JArray();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            List<Personnel_Roster> inputList = obj.inputList;
            if (inputList.Count > 0)
            {
                foreach (var item in inputList)
                {
                    item.CreateDate = DateTime.Now;
                    item.Creator = auth.UserName;
                    if (db.Personnel_Roster.Count(c => c.JobNum == item.JobNum) > 0)
                        result.Add(item.JobNum);
                }
                if (result.Count > 0)
                {
                    return common.GetModuleFromJarray(result, false, "已经有相同的数据，请重新输入");
                }
                db.Personnel_Roster.AddRange(inputList);
                int count = db.SaveChanges();
                if (count > 0)
                {
                    return common.GetModuleFromJarray(result, true, "添加" + inputList.Count.ToString() + "员工成功");
                }
                else
                {
                    return common.GetModuleFromJarray(result, false, "添加失败");
                }
            }
            return common.GetModuleFromJarray(result, false, "添加失败");
        }

        #endregion

        #region------单个添加员工
        [HttpPost]
        [ApiAuthorize]
        public JObject Create([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            Personnel_Roster personnel_Roster = JsonConvert.DeserializeObject<Personnel_Roster>(JsonConvert.SerializeObject(data));
            if (personnel_Roster != null && personnel_Roster.JobNum != null)
            {
                if (db.Personnel_Roster.Count(c => c.JobNum == personnel_Roster.JobNum) > 0)
                {
                    return common.GetModuleFromJobjet(result, false, personnel_Roster.JobNum + "已经有相同的数据，请重新输入");
                }
                if (!string.IsNullOrEmpty(personnel_Roster.JobNum) && !string.IsNullOrEmpty(personnel_Roster.Name) && !string.IsNullOrEmpty(personnel_Roster.Education) && !string.IsNullOrEmpty(personnel_Roster.DP_Group) && !string.IsNullOrEmpty(personnel_Roster.Position) && !string.IsNullOrEmpty(personnel_Roster.Department))
                {
                    personnel_Roster.CreateDate = DateTime.Now;
                    personnel_Roster.Creator = auth.UserName;
                    db.Personnel_Roster.Add(personnel_Roster);
                    int count = db.SaveChanges();
                    if (count > 0)
                    {
                        return common.GetModuleFromJobjet(result, true, "添加成功");
                    }
                    else
                    {
                        return common.GetModuleFromJobjet(result, false, "添加失败");
                    }
                }
            }
            return common.GetModuleFromJobjet(result, false, "添加失败");
        }

        #endregion

        #region------修改花名册记录
        [HttpPost]
        [ApiAuthorize]
        public JObject updateDate([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            Personnel_Roster personnel_Roster = JsonConvert.DeserializeObject<Personnel_Roster>(JsonConvert.SerializeObject(data));
            if (!string.IsNullOrEmpty(personnel_Roster.JobNum) && !string.IsNullOrEmpty(personnel_Roster.Name) && !string.IsNullOrEmpty(personnel_Roster.Education) && !string.IsNullOrEmpty(personnel_Roster.DP_Group) && !string.IsNullOrEmpty(personnel_Roster.Position) && !string.IsNullOrEmpty(personnel_Roster.Department))
            {
                int count = 0;
                db.Entry(personnel_Roster).State = EntityState.Modified;
                count = db.SaveChanges();
                int WorkNumber = int.Parse(personnel_Roster.JobNum);
                var department = db.Users.Where(c => c.UserNum == WorkNumber && c.UserName == personnel_Roster.Name).ToList();
                foreach (var it in department)
                {
                    it.Department = personnel_Roster.Department;
                    db.Entry(it).State = EntityState.Modified;
                    count += db.SaveChanges();
                }
                if (count > 0)
                {
                    return common.GetModuleFromJobjet(result, true, "修改成功");
                }
                else
                {
                    return common.GetModuleFromJobjet(result, false, "修改失败");
                }
            }
            return common.GetModuleFromJobjet(result, false, "修改失败");
        }

        #endregion

        #region ------------departmentList()取花名册部门列表
        [HttpPost]
        [ApiAuthorize]
        public JObject DepartmentList()
        {
            var departmentlist = db.Personnel_Roster.OrderByDescending(m => m.Id).Select(m => m.Department).Distinct().ToList();
            JArray result = new JArray
            {
                departmentlist
            };
            return common.GetModuleFromJarray(result);
        }

        #endregion

        #region ------------LevelTypeList()取族群列表
        [HttpPost]
        [ApiAuthorize]
        public JObject LevelTypeList()
        {
            var typelist = db.Personnel_Roster.OrderByDescending(m => m.Id).Select(m => m.levelType).Distinct().ToList();
            JArray result = new JArray
            {
                typelist
            };
            return common.GetModuleFromJarray(result);
        }

        #endregion



    }





}
