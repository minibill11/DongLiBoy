﻿using JianHeMES.AuthAttributes;
using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class VersionManagementController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login2", "Users", new { col = "VersionManagement", act = "Index" });
            }
            return View();
        }
        //MES版本管理显示
        public ActionResult SelectVersion(string version, DateTime? startTime, DateTime? endTime)
        {
            
            List<string> versionList = new List<string>();
            if (!string.IsNullOrEmpty(version))
            {
                versionList.Add(version);
            }
            else if (startTime != null && endTime != null)
            {
                versionList = db.VersionInfo.Where(c => c.UpdateTime >= startTime && c.UpdateTime <= endTime).Select(c => c.MESVersion).Distinct().ToList();
            }
            else
            {
                versionList = db.VersionInfo.Select(c => c.MESVersion).Distinct().ToList();
            }
            JArray result = new JArray();
            foreach (var item in versionList)
            {
                JObject obj = new JObject();
                obj.Add("MESVersion", item);
                JArray sectionAry = new JArray();
                var info = db.VersionInfo.Where(c => c.MESVersion == item).ToList();
                if (item == "Next")
                {
                    obj.Add("CanChange", true);
                }
                else
                {
                    obj.Add("CanChange", false);
                }
                foreach (var section in info)
                {
                    JObject objitem = new JObject();
                    objitem.Add("Section", section.Section);
                    objitem.Add("UpdateMes", section.UpdateMes);
                    sectionAry.Add(objitem);
                }
                obj.Add("UpdateMesArray", sectionAry);
                obj.Add("UpdateTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", info.Max(c => c.UpdateTime)));
                result.Add(obj);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //变为新版本
        public ActionResult GetNewVersion(string newVersion)
        {
            var info = db.VersionInfo.Where(c => c.MESVersion == "Next").ToList();
            info.ForEach(c => c.MESVersion = newVersion);
            db.SaveChanges();
            JObject result = new JObject();
            result.Add("mes", "变更成功");
            result.Add("pass", true);
            return Content(JsonConvert.SerializeObject(result));
        }

        //显示
        public ActionResult DisplayVersion()
        {
            var info = db.VersionInfo.Where(c => c.MESVersion == "Next").ToList();
            JArray result = new JArray();
            foreach (var item in info)
            {
                JObject obj = new JObject();
                obj.Add("id", item.Id);
                obj.Add("MESVersion", item.MESVersion);
                obj.Add("Section", item.Section);
                obj.Add("UpdateMes", item.UpdateMes);
                obj.Add("UpdateTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.UpdateTime));
                obj.Add("SNVVersion", item.SNVVersion);
                result.Add(obj);
            }
            return Content(JsonConvert.SerializeObject(result));
        }
        //新增
        public ActionResult AddVersion(List<VersionInfo> version)
        {
            version.ForEach(c => { c.MESVersion = "Next"; c.UpdateTime = DateTime.Now; });
            db.VersionInfo.AddRange(version);
            db.SaveChanges();
            JObject result = new JObject();
            result.Add("mes", "新增成功");
            result.Add("pass", true);
            return Content(JsonConvert.SerializeObject(result));
        }

        //更新
        public ActionResult UpdateVersion(int id, string Section, string UpdateMes, string SNVVersion)
        {
            var info = db.VersionInfo.Find(id);
            info.Section = Section;
            info.UpdateMes = UpdateMes;
            info.SNVVersion = SNVVersion;
            info.UpdateTime = DateTime.Now;
            db.SaveChanges();
            JObject result = new JObject();
            result.Add("mes", "更新成功");
            result.Add("UpdateTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", info.UpdateTime));
            result.Add("pass", true);
            return Content(JsonConvert.SerializeObject(result));
        }

        //删除
        public ActionResult DeleteVersion(int id)
        {
            JObject result = new JObject();
            var info = db.VersionInfo.Find(id);
            db.VersionInfo.Remove(info);
            db.SaveChanges();
            result.Add("mes", "删除成功");
            result.Add("pass", true);
            return Content(JsonConvert.SerializeObject(result));
        }

        //拿到工段列表
        public ActionResult GetSectionList()
        {
            var orders = db.Version_SectionList.OrderByDescending(m => m.Id).Select(m => m.Section).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }

        //新增工段
        public ActionResult AddSection(string section)
        {
            Version_SectionList sectionList = new Version_SectionList() { Section = section, Createor = ((Users)Session["User"]).UserName, CreateTime = DateTime.Now };
            db.Version_SectionList.Add(sectionList);
            db.SaveChanges();
            JObject result = new JObject();
            result.Add("mes", "新增成功");
            result.Add("pass", true);
            return Content(JsonConvert.SerializeObject(result));
        }

        //版本列表
        public ActionResult GetVersion()
        {
            var orders = db.VersionInfo.OrderByDescending(m => m.Id).Select(m => m.MESVersion).Distinct().ToList();    //增加.Distinct()后会重新按OrderNum升序排序
            JArray result = new JArray();
            foreach (var item in orders)
            {
                JObject List = new JObject();
                List.Add("value", item);

                result.Add(List);
            }
            return Content(JsonConvert.SerializeObject(result));
        }


        //拿到最新版本号
        public string GetLasrVersion()
        {
            var verssion = db.VersionInfo.Where(c => c.MESVersion != "Next").OrderByDescending(c => c.Id).Select(c=>c.MESVersion).FirstOrDefault();
            return verssion;
        }
    }

    public class VersionManagement_ApiController : System.Web.Http.ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonController com = new CommonController();

        [HttpPost]
        [ApiAuthorize]
        public JArray SelectVersion(string version, DateTime? startTime, DateTime? endTime)
        {

            List<string> versionList = new List<string>();
            if (!string.IsNullOrEmpty(version))
            {
                versionList.Add(version);
            }
            else if (startTime != null && endTime != null)
            {
                versionList = db.VersionInfo.Where(c => c.UpdateTime >= startTime && c.UpdateTime <= endTime).Select(c => c.MESVersion).Distinct().ToList();
            }
            else
            {
                versionList = db.VersionInfo.Select(c => c.MESVersion).Distinct().ToList();
            }
            JArray result = new JArray();
            foreach (var item in versionList)
            {
                JObject obj = new JObject();
                obj.Add("MESVersion", item);
                JArray sectionAry = new JArray();
                var info = db.VersionInfo.Where(c => c.MESVersion == item).ToList();
                if (item == "Next")
                {
                    obj.Add("CanChange", true);
                }
                else
                {
                    obj.Add("CanChange", false);
                }
                foreach (var section in info)
                {
                    JObject objitem = new JObject();
                    objitem.Add("Section", section.Section);
                    objitem.Add("UpdateMes", section.UpdateMes);
                    sectionAry.Add(objitem);
                }
                obj.Add("UpdateMesArray", sectionAry);
                obj.Add("UpdateTime", string.Format("{0:yyyy-MM-dd HH:mm:ss}", info.Max(c => c.UpdateTime)));
                result.Add(obj);
            }
            return result;
        }

        [HttpPost]
        [ApiAuthorize]
        public JObject LasrVersion()
        {
            var verssion = db.VersionInfo.Where(c => c.MESVersion != "Next").OrderByDescending(c => c.Id).Select(c => c.MESVersion).FirstOrDefault();
            return com.GetModuleFromJobjet(null,null,verssion);
        }
    }


}