using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JianHeMES.Models;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JianHeMES.AuthAttributes;

namespace JianHeMES.Controllers
{
    public class Personnel_LeaveController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        CommonalityController com = new CommonalityController();
        private static List<input> input = new List<input>();

        #region index 
        public ActionResult Index()
        {

            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Leave", act = "Index" });
            }
            return View();

        }
        [HttpPost]
        public async Task<ActionResult> index(string jobNum, string userName, string department, DateTime? startdate, DateTime? enddate, DateTime? modate)
        {
            JObject userItem = new JObject();
            JObject userJobject = new JObject();
            var personnel_leave = await db.Personnel_Leave.ToListAsync();

            if (jobNum != null)
            {
                personnel_leave = personnel_leave.Where(c => c.jobNum == jobNum).ToList();
            }
            if (userName != null)
            {
                personnel_leave = personnel_leave.Where(c => c.Name == userName).ToList();
            }
            if (department != null)
            {
                personnel_leave = personnel_leave.Where(c => c.department == department).ToList();
            }
            int i = 0;

            #region  按时间段查找数据
            if (startdate != null && enddate != null)
            {
                personnel_leave = personnel_leave.Where(c => c.leaveStartTime >= startdate && c.leaveStartTime <= enddate).ToList();
            }
            //按年月查找数据
            if (modate != null)
            {
                personnel_leave = personnel_leave.Where(c => c.leaveStartTime.Value.Year == modate.Value.Year && c.leaveStartTime.Value.Month == modate.Value.Month).ToList();
            }
            #endregion

            foreach (var item in personnel_leave)
            {
                //ID
                userItem.Add("Id", item.Id);
                //名字
                userItem.Add("Name", item.Name);
                //工号
                userItem.Add("jobNum", item.jobNum);
                //部门
                userItem.Add("department", item.department);
                //组
                userItem.Add("DP_group", item.DP_group);
                //岗位
                userItem.Add("position", item.position);
                //代理人
                userItem.Add("agent", item.agent);
                //请假类型
                userItem.Add("leaveType", item.leaveType);
                //申请日期
                userItem.Add("applydate", item.applydate);
                //请假开始时间
                userItem.Add("leaveStartTime", item.leaveStartTime);
                //请假结束时间
                userItem.Add("leaveEndTime", item.leaveEndTime);
                //请假时长
                userItem.Add("leaveTimeNum", item.leaveTimeNum);
                userItem.Add("leaveReason", item.leaveReason);
                userItem.Add("remark", item.remark);
                string leavetostring = string.Format("{0:yyyy-MM-dd--HH-mm-ss}", item.leaveStartTime);
                if (!checkfile(item.Name, leavetostring, "jpg"))
                {
                    userItem.Add("IsImg", "ture");
                }
                else
                    userItem.Add("IsImg", "false");

                if (!checkfile(item.Name, leavetostring, "pdf"))
                {
                    userItem.Add("pdf", "ture");
                }
                else
                    userItem.Add("pdf", "false");


                userJobject.Add(i.ToString(), userItem);
                i++;
                userItem = new JObject();
            }

            return Content(JsonConvert.SerializeObject(userJobject));
        }

        #endregion

        #region  -------默认时间值
        public ActionResult LeaveInfo()
        {
            var LeaveTime = db.Personnel_Leave.OrderByDescending(c => c.leaveStartTime).Select(c => c.leaveStartTime).FirstOrDefault();
            var year = LeaveTime.Value.Year;
            var mouth = LeaveTime.Value.Month;
            var leave = db.Personnel_Leave.Where(c => c.leaveStartTime.Value.Year == year && c.leaveStartTime.Value.Month == mouth).ToList();
            JObject userItem = new JObject();
            JObject userJobject = new JObject();
            int i = 0;
            foreach (var item in leave)
            {
                //ID
                userItem.Add("Id", item.Id);
                //名字
                userItem.Add("Name", item.Name);
                //工号
                userItem.Add("jobNum", item.jobNum);
                //部门
                userItem.Add("department", item.department);
                //组
                userItem.Add("DP_group", item.DP_group);
                //岗位
                userItem.Add("position", item.position);
                //代理人
                userItem.Add("agent", item.agent);
                //请假类型
                userItem.Add("leaveType", item.leaveType);
                //申请日期
                userItem.Add("applydate", item.applydate);
                //请假开始时间
                userItem.Add("leaveStartTime", item.leaveStartTime);
                //请假结束时间
                userItem.Add("leaveEndTime", item.leaveEndTime);
                //请假时长
                userItem.Add("leaveTimeNum", item.leaveTimeNum);
                userItem.Add("leaveReason", item.leaveReason);
                userItem.Add("remark", item.remark);
                string leavetostring = string.Format("{0:yyyy-MM-dd--HH-mm-ss}", item.leaveStartTime);
                if (!checkfile(item.Name, leavetostring, "jpg"))
                {
                    userItem.Add("IsImg", "ture");
                }
                else
                    userItem.Add("IsImg", "false");

                if (!checkfile(item.Name, leavetostring, "pdf"))
                {
                    userItem.Add("pdf", "ture");
                }
                else
                    userItem.Add("pdf", "false");

                userItem.Add("time", year.ToString() + "-" + mouth.ToString());

                userJobject.Add(i.ToString(), userItem);
                i++;
                userItem = new JObject();

            }

            return Content(JsonConvert.SerializeObject(userJobject));
        }

        #endregion

        #region 详细
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Leave personnel_Leave = db.Personnel_Leave.Find(id);
            if (personnel_Leave == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Leave);
        }
        #endregion

        #region 单个增加
        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Leave", act = "Create" });
            }


            return View();
        }

        [HttpPost]
        //,departmentApprover,centerApprover,personnelApprover,factoryApprover,managerApprover
        public ActionResult Create([Bind(Include = "Id,Name,jobNum,department,DP_group,position,agent,leaveType,applydate,leaveStartTime,leaveEndTime,leaveTimeNum,leaveReason,remark")] Personnel_Leave personnel_Leave)
        {
            if (ModelState.IsValid)
            {
                if (db.Personnel_Leave.Count(c => c.jobNum == personnel_Leave.jobNum && c.leaveStartTime == personnel_Leave.leaveStartTime) > 0)
                {
                    return Content("已有重复数据，请确认是否数据是否正确");
                }
                personnel_Leave.CreateDate = DateTime.Now;
                personnel_Leave.Creator = ((Users)Session["User"]).UserName;
                db.Personnel_Leave.Add(personnel_Leave);
                db.SaveChanges();
                return Content("true");
            }
            return Content("新增出错，请确认数据是否规范");
        }
        //根据给出的工号提供基本信息
        [HttpPost]
        public ActionResult getInfo(string jobnum)
        {
            var info = db.Personnel_Roster.Where(c => c.JobNum == jobnum).Select(c => new { c.Name, c.Department, c.DP_Group, c.Position }).FirstOrDefault();
            return Content(JsonConvert.SerializeObject(info));
        }
        #endregion

        #region 批量增加
        public ActionResult Batch_InputStaff()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Users", new { col = "Personnel_Leave", act = "Batch_InputStaff" });
            }


            return View();

        }

        [HttpPost]
        public ActionResult Batch_InputStaff(List<Personnel_Leave> inputList)
        {
            if (input != null)
            {
                input.Clear();
            }
            string repeat = null;
            foreach (var item in inputList)
            {
                if (db.Personnel_Leave.Count(c => c.jobNum == item.jobNum && c.leaveStartTime == item.leaveStartTime) != 0)
                    repeat = repeat + item.jobNum + ",";
            }
            if (!string.IsNullOrEmpty(repeat))
            {
                return Content(repeat + "已经有相同的数据，请重新输入");
            }

            foreach (var fi in inputList)
            {
                var bacth = new input() { userneme = fi.Name, datetime = string.Format("{0:yyyy-MM-dd--HH-mm-ss}", fi.leaveStartTime) };
                input.Add(bacth);
                fi.CreateDate = DateTime.Now;
                fi.Creator = ((Users)Session["User"]).UserName;
            }

            if (ModelState.IsValid)
            {
                db.Personnel_Leave.AddRange(inputList);
                db.SaveChangesAsync();
                return Content("true");
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

        #region 修改
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Leave personnel_Leave = db.Personnel_Leave.Find(id);
            if (personnel_Leave == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Leave);
        }

        [HttpPost]

        public ActionResult Edit([Bind(Include = "Id,Name,jobNum,department,DP_group,position,agent,leaveType,applydate,leaveStartTime,leaveEndTime,leaveTimeNum,leaveReason,remark")] Personnel_Leave personnel_Leave)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personnel_Leave).State = EntityState.Modified;
                db.SaveChangesAsync();
                return Content("true");
            }
            return Content("修改出错，请确认数据是否正确");
        }
        #endregion

        #region 删除
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Personnel_Leave personnel_Leave = db.Personnel_Leave.Find(id);
            if (personnel_Leave == null)
            {
                return HttpNotFound();
            }
            return View(personnel_Leave);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Personnel_Leave personnel_Leave = db.Personnel_Leave.Find(id);
            db.Personnel_Leave.Remove(personnel_Leave);
            db.SaveChanges();
            return Content("true");
        }

        #endregion

        #region 批量添加页面上传请假证明文件

        [HttpPost]
        public ActionResult BatchuploadLeaveFiles()
        {
            if (input != null)
            {

                foreach (var itme in input)
                {
                    string userneme = itme.userneme;
                    string leavestarttime = itme.datetime;
                    foreach (var file1 in Request.Files)
                    {

                        HttpPostedFileBase file = Request.Files[file1.ToString()];
                        var fileType = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                        var re = String.Equals(fileType, ".jpg") == true || String.Equals(fileType, ".pdf") == true ? false : true;
                        if (re)
                        {
                            return Content("<script>alert('您选择文件的文件类型不正确，请选择jpg或pdf类型文件！');history.go(-1);</script>");
                        }
                        string ReName = userneme + "_" + leavestarttime;
                        if (Directory.Exists(@"D:\\MES_Data\\LeaveFiles\\" + userneme + "\\") == false)//如果不存在就创建订单文件夹
                        {
                            Directory.CreateDirectory(@"D:\MES_Data\LeaveFiles\" + userneme + "\\");
                        }

                        List<FileInfo> fileInfos = com.GetAllFilesInDirectory(@"D:\MES_Data\LeaveFiles\" + userneme + "\\");
                        int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ReName) && c.Name.Substring(c.Name.Length - 4, 1) == ".").Count();

                        file.SaveAs(@"D:\MES_Data\LeaveFiles\" + userneme + "\\" + ReName + (jpg_count + 1) + fileType);
                    }
                }
                if (Request.Files.Count > 0)
                {
                    return Content("上传成功！");
                }
            }

            return Content("上传失败！");
        }
        #endregion

        #region 上传请假证明文件

        [HttpPost]
        public ActionResult uploadLeaveFiles(string userneme, string leavestarttime)
        {
            foreach (var file1 in Request.Files)
            {

                HttpPostedFileBase file = Request.Files[file1.ToString()];
                var fileType = file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                var re = String.Equals(fileType, ".jpg") == true || String.Equals(fileType, ".pdf") == true ? false : true;
                if (re)
                {
                    return Content("<script>alert('您选择文件的文件类型不正确，请选择jpg或pdf类型文件！');history.go(-1);</script>");
                }
                string ReName = userneme + "_" + leavestarttime;
                if (Directory.Exists(@"D:\\MES_Data\\LeaveFiles\\" + userneme + "\\") == false)//如果不存在就创建订单文件夹
                {
                    Directory.CreateDirectory(@"D:\MES_Data\LeaveFiles\" + userneme + "\\");
                }

                List<FileInfo> fileInfos = com.GetAllFilesInDirectory(@"D:\MES_Data\LeaveFiles\" + userneme + "\\");
                int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ReName) && c.Name.Substring(c.Name.Length - 4, 1) == ".").Count();

                file.SaveAs(@"D:\MES_Data\LeaveFiles\" + userneme + "\\" + ReName + (jpg_count + 1) + fileType);

            }
            if (Request.Files.Count > 0)
            {
                return Content("上传成功！");
            }
            return Content("上传失败！");
        }
        #endregion

        #region  判断是否有请假证明图片
        public bool checkfile(string userneme, string leavestarttime, string leaveFilepdf)
        {
            List<FileInfo> filesInfo = com.GetAllFilesInDirectory(@"D:\\MES_Data\\LeaveFiles" + "\\" + userneme + "\\");
            var filesInfojpg = filesInfo.Where(c => c.Name.StartsWith(userneme + "_" + leavestarttime) && c.Name.Substring(c.Name.Length - 4, 4) == "." + leaveFilepdf).ToList();
            return filesInfojpg.Count() == 0;
        }
        #endregion

        #region 查看请假证明图片JPG
        public ActionResult displayImg(string userneme, string leavestarttime)
        {
            List<FileInfo> filesInfo = com.GetAllFilesInDirectory(@"D:\\MES_Data\\LeaveFiles\\" + userneme + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(userneme + "_" + leavestarttime) && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").ToList();
            JObject json = new JObject();
            int i = 1;
            if (filesInfo.Count() > 0)
            {
                foreach (var item in filesInfo)
                {
                    json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.jpgjson = json;
                return Content(json.ToString());
            }
            else
            {
                return Content("图片文档未上传或不存在！");
            }
        }
        #endregion

        #region  查看请假证明图片PDF
        public ActionResult display_pdf(string userneme, string leavestarttime)
        {
            List<FileInfo> filesInfo = com.GetAllFilesInDirectory(@"D:\\MES_Data\\LeaveFiles\\" + userneme + "\\");
            filesInfo = filesInfo.Where(c => c.Name.StartsWith(userneme + "_" + leavestarttime) && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").ToList();
            JObject json = new JObject();
            int i = 1;
            if (filesInfo.Count() > 0)
            {
                foreach (var item in filesInfo)
                {
                    json.Add(i.ToString(), item.Name);
                    i++;
                }
                ViewBag.jpgjson = json;
                return Content(json.ToString());
            }
            else
            {
                return Content("图片文档未上传或不存在！");
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    internal class input
    {
        public string userneme { get; set; }
        public string datetime { get; set; }
    }


    //Api接口部分

    public class Personnel_Leave_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();
        private CommonController common = new CommonController();


        #region---首页查询页
        [HttpPost]
        [ApiAuthorize]
        public JObject index([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            JObject userItem = new JObject();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string jobNum = obj.jobNum == null ? null : obj.jobNum;//工号
            string userName = obj.userName == null ? null : obj.userName;//用户名
            string department = obj.department == null ? null : obj.department;//用户部门
            DateTime? startdate = obj.startdate;//开始时间
            DateTime? enddate = obj.enddate;//结束时间
            DateTime? modate = obj.modate;
            var personnel_leave = db.Personnel_Leave.ToList();

            if (jobNum != null)
            {
                personnel_leave = personnel_leave.Where(c => c.jobNum == jobNum).ToList();
            }
            if (userName != null)
            {
                personnel_leave = personnel_leave.Where(c => c.Name == userName).ToList();
            }
            if (department != null)
            {
                personnel_leave = personnel_leave.Where(c => c.department == department).ToList();
            }
            int i = 0;

            #region  按时间段查找数据
            if (startdate != null && enddate != null)
            {
                personnel_leave = personnel_leave.Where(c => c.leaveStartTime >= startdate && c.leaveStartTime <= enddate).ToList();
            }
            //按年月查找数据
            if (modate != null)
            {
                personnel_leave = personnel_leave.Where(c => c.leaveStartTime.Value.Year == modate.Value.Year && c.leaveStartTime.Value.Month == modate.Value.Month).ToList();
            }
            #endregion

            foreach (var item in personnel_leave)
            {
                //ID
                userItem.Add("Id", item.Id);
                //名字
                userItem.Add("Name", item.Name);
                //工号
                userItem.Add("jobNum", item.jobNum);
                //部门
                userItem.Add("department", item.department);
                //组
                userItem.Add("DP_group", item.DP_group);
                //岗位
                userItem.Add("position", item.position);
                //代理人
                userItem.Add("agent", item.agent);
                //请假类型
                userItem.Add("leaveType", item.leaveType);
                //申请日期
                userItem.Add("applydate", item.applydate);
                //请假开始时间
                userItem.Add("leaveStartTime", item.leaveStartTime);
                //请假结束时间
                userItem.Add("leaveEndTime", item.leaveEndTime);
                //请假时长
                userItem.Add("leaveTimeNum", item.leaveTimeNum);
                userItem.Add("leaveReason", item.leaveReason);
                string leavetostring = string.Format("{0:yyyy-MM-dd--HH-mm-ss}", item.leaveStartTime);
                string ReName = item.Name + "_" + leavetostring;
                //异常图片
                List<FileInfo> fileInfos = null;
                JArray picture_jpg = new JArray();
                JArray picture_pdf = new JArray();
                JArray picture = new JArray();
                if (Directory.Exists(@"D:\MES_Data\LeaveFiles\" + item.Name + "\\") == false)
                {
                    picture.Add(false);
                }
                else
                {
                    fileInfos = comm.GetAllFilesInDirectory(@"D:\\MES_Data\\LeaveFiles\\" + item.Name + "\\");
                    foreach (var it in fileInfos)
                    {
                        string path = @"/MES_Data/LeaveFiles/" + item.Name + "/" + ReName;//组合出路径
                        var filetype = path.Split('.');//将组合出来的路径以点分隔，方便下一步判断后缀
                        if (filetype[1] == "pdf")//后缀为jpg
                        {
                            picture_pdf.Add(path);
                        }
                        else //后缀为其他
                        {
                            picture_jpg.Add(path);
                        }
                    }
                }
                //异常图片
                userItem.Add("Picture", picture);
                picture = new JArray();
                userItem.Add("Picture_jpg", picture_jpg);
                picture_jpg = new JArray();
                userItem.Add("Picture_pdf", picture_pdf);
                picture_pdf = new JArray();
                userItem.Add("remark", item.remark);//备注
                result.Add(i.ToString(), userItem);
                i++;
                userItem = new JObject();
            }
            return common.GetModuleFromJobjet(result);
        }
        #endregion

        #region ----上传请假证明图片

        [HttpPost]
        [ApiAuthorize]
        public bool UploadLeaveFiles()
        {
            bool result = false;
            string userneme = HttpContext.Current.Request["userneme"];
            DateTime leavestarttime = Convert.ToDateTime(HttpContext.Current.Request["leavestarttime"]);
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//上传的问题件  这个“MS_HttpContext”参数名不需要改
            HttpRequestBase requests = context.Request;
            string complaints = string.Format("{0:yyyyMMdd}", leavestarttime);
            if (requests.Files.Count > 0)
            {
                if (Directory.Exists(@"D:\\MES_Data\\LeaveFiles\\" + userneme + "\\") == false)//判断路径是否存在
                {
                    Directory.CreateDirectory(@"D:\MES_Data\LeaveFiles\" + userneme + "\\");//创建路径
                }
                for (int i = 0; i < requests.Files.Count; i++)
                {
                    string ReName = userneme + "_" + complaints;
                    var file = requests.Files[i];
                    var fileType = file.FileName.Substring(file.FileName.Length - 4, 4).ToLower();
                    List<FileInfo> fileInfos = comm.GetAllFilesInDirectory(@"D:\MES_Data\LeaveFiles\" + userneme + "\\");
                    if (fileType == ".jpg")//判断文件后缀
                    {
                        int jpg_count = fileInfos.Where(c => c.Name.StartsWith(ReName + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                        file.SaveAs(@"D:\MES_Data\LeaveFiles\" + userneme + "\\" + ReName + (jpg_count + 1) + fileType); ;//文件追加命名
                        result = true;
                    }
                    else if (fileType == "jpeg")
                    {
                        int jpeg_count = fileInfos.Where(c => c.Name.StartsWith(ReName + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".jpg").Count();
                        file.SaveAs(@"D:\MES_Data\LeaveFiles\" + userneme + "\\" + ReName + (jpeg_count + 1) + fileType); ;//文件追加命名名
                        result = true;
                    }
                    else if (fileType == ".pdf")
                    {
                        int pdf_count = fileInfos.Where(c => c.Name.StartsWith(ReName + "_") && c.Name.Substring(c.Name.Length - 4, 4) == ".pdf").Count();
                        file.SaveAs(@"D:\MES_Data\LeaveFiles\" + userneme + "\\" + ReName + "_" + (pdf_count + 1) + fileType);//文件追加命名
                        result = true;
                    }
                    else if (fileType == "heic")
                    {
                        int heic_count = fileInfos.Where(c => c.Name.StartsWith(ReName + "_") && c.Name.Substring(c.Name.Length - 4, 4) == "heic").Count();
                        file.SaveAs(@"D:\MES_Data\LeaveFiles\" + userneme + "\\" + ReName + "_" + (heic_count + 1) + "." + fileType);//文件追加命名
                        result = true;
                    }
                    else
                    {
                        int else_count = fileInfos.Where(c => c.Name.StartsWith(ReName + "_") && c.Name.Substring(c.Name.Length - 4, 4) == fileType).Count();
                        file.SaveAs(@"D:\MES_Data\LeaveFiles\" + userneme + "\\" + ReName + "_" + (else_count + 1) + "." + fileType);//其他文件追加命名
                        result = true;
                    }
                }
                return result;
            }
            return result;
        }
        #endregion

        #region---单个上传请假记录
        [HttpPost]
        [ApiAuthorize]
        public JObject Create([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            Personnel_Leave personnel_Leave = JsonConvert.DeserializeObject<Personnel_Leave>(JsonConvert.SerializeObject(data));
            if (personnel_Leave != null && personnel_Leave.jobNum != null)
            {
                if (db.Personnel_Leave.Count(c => c.jobNum == personnel_Leave.jobNum && c.leaveStartTime == personnel_Leave.leaveStartTime) > 0)
                {
                    return common.GetModuleFromJobjet(result, false, "已有重复数据，请确认是否数据是否正确");
                }
                else
                {
                    personnel_Leave.CreateDate = DateTime.Now;
                    personnel_Leave.Creator = auth.UserName;
                    db.Personnel_Leave.Add(personnel_Leave);
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
        //根据给出的工号提供基本信息
        [HttpPost]
        [ApiAuthorize]
        public JObject getInfo([System.Web.Http.FromBody]JObject data)
        {
            JArray result = new JArray();
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string jobnum = obj.jobnum;//工号
            var info = db.Personnel_Roster.Where(c => c.JobNum == jobnum).Select(c => new { c.Name, c.Department, c.DP_Group, c.Position }).FirstOrDefault();
            result.Add(info);
            return common.GetModuleFromJarray(result);
        }

        #endregion

        #region----批量添加请假记录
        [HttpPost]
        [ApiAuthorize]
        public JObject Batch_InputStaff([System.Web.Http.FromBody]JObject data)
        {
            JArray result = new JArray();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            List<Personnel_Leave> inputList = obj.inputList;
            if (inputList.Count > 0)
            {
                string rep = null;
                foreach (var item in inputList)
                {
                    if (db.Personnel_Leave.Count(c => c.jobNum == item.jobNum && c.leaveStartTime == item.leaveStartTime) != 0)
                        rep = item.jobNum + "," + item.leaveStartTime;
                    result.Add(rep);
                }
                if (result.Count > 0)
                {
                    return common.GetModuleFromJarray(result, false, "已经有相同的数据，请重新输入");
                }
                foreach (var fi in inputList)
                {
                    fi.CreateDate = DateTime.Now;
                    fi.Creator = auth.UserName;
                    db.SaveChanges();
                }
                db.Personnel_Leave.AddRange(inputList);
                int count = db.SaveChanges();
                if (count > 0)
                {
                    return common.GetModuleFromJarray(result, true, "添加成功");
                }
                else
                {
                    return common.GetModuleFromJarray(result, false, "添加失败");
                }
            }
            return common.GetModuleFromJarray(result, false, "添加失败");
        }

        #endregion

        #region--- 修改记录

        [HttpPost]
        [ApiAuthorize]
        public JObject Edit([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            Personnel_Leave personnel_Leave = JsonConvert.DeserializeObject<Personnel_Leave>(JsonConvert.SerializeObject(data));
            if (personnel_Leave != null && personnel_Leave.Id != 0)
            {
                personnel_Leave.CreateDate = DateTime.Now;
                personnel_Leave.Creator = auth.UserName;
                db.Entry(personnel_Leave).State = EntityState.Modified;
                int count = db.SaveChanges();
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

        #region--- 删除请假记录
        [HttpPost]
        [ApiAuthorize]
        public JObject DeleteConfirmed([System.Web.Http.FromBody]JObject data)
        {
            JObject result = new JObject();
            AuthInfo auth = (AuthInfo)this.RequestContext.RouteData.Values["Authorization"];
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            int id = obj.id;//id
            if (id != 0)
            {
                var Leave = db.Personnel_Leave.Where(c => c.Id == id).FirstOrDefault();            
                UserOperateLog operaterecord = new UserOperateLog();
                operaterecord.OperateDT = DateTime.Now;//添加删除操作时间
                operaterecord.Operator = auth.UserName;//添加删除操作人
                operaterecord.OperateRecord = operaterecord.Operator + "在" + operaterecord.OperateDT + "删除" + Leave.Creator + "创建的请假记录信息";
                db.Personnel_Leave.Remove(Leave);//删除对应的数据
                db.UserOperateLog.Add(operaterecord);//添加删除操作日记数据
               int count = db.SaveChanges();
                if (count > 0)//判断count是否大于0（有没有把数据保存到数据库）
                {
                    return common.GetModuleFromJobjet(result, true, "删除成功");
                }
                else //等于0（没有把数据保存到数据库或者保存出错）
                {
                    return common.GetModuleFromJobjet(result, false, "删除失败！");
                }
            }
            return common.GetModuleFromJobjet(result, false, "删除失败！");          
        }
        #endregion



    }

}

