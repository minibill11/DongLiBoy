﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JianHeMES.Models;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net;
using System.Data.Entity;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using Microsoft.Owin.Security.Twitter.Messages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace JianHeMES.Controllers
{
    public class SMTController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();


        #region------------------管理界面
        // GET: SMT管理主页

        public ActionResult SMT_Mangage()
        {
            List<SMT_ProductionLineInfo> SMT_ProcutionLineInfos = new List<SMT_ProductionLineInfo>();
            SMT_ProcutionLineInfos = db.SMT_ProductionLineInfo.ToList();
            if (SMT_ProcutionLineInfos != null)
            {
                return View(SMT_ProcutionLineInfos);
            }
            else
                return View();
        }



        #region---------------------产线管理

        public ActionResult SMT_ProductionLineCreate()
        {
            ViewBag.Status = ProductionLineStatus();
            return View();
        }
        [HttpPost]
        public ActionResult SMT_ProductionLineCreate(FormCollection fc)
        {
            SMT_ProductionLineInfo newline = new SMT_ProductionLineInfo();
            newline.LineNum = Convert.ToInt32(fc["LineNum"]);
            newline.CreateDate = DateTime.Now;
            newline.Team = fc["Team"];
            newline.GroupLeader = fc["GroupLeader"];
            newline.Status = fc["Status"];
            ViewBag.Status = ProductionLineStatus();
            if (ModelState.IsValid)
            {
                db.SMT_ProductionLineInfo.Add(newline);
                db.SaveChanges();
                return RedirectToAction("SMT_Mangage");
            }
            else
            {
                return View(newline);
            }
        }


        public async Task<ActionResult> SMT_ProductionLineEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SMT_ProductionLineInfo record = await db.SMT_ProductionLineInfo.FindAsync(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = ProductionLineStatus();
            return View(record);
        }

        [HttpPost]
        public async Task<ActionResult> SMT_ProductionLineEdit([Bind(Include = "Id,LineNum,ProducingOrderNum,CreateDate,Team,GroupLeader,Status")]SMT_ProductionLineInfo record)
        {
            if (ModelState.IsValid)
            {
                db.Entry(record).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("SMT_Mangage");
            }
            return View(record);
        }

        #endregion



        #region---------------------订单管理

        // GET: SMT订单信息管理
        public ActionResult SMT_OrderMangage()
        {
            ViewBag.FinishStatus = FinishStatusList();
            return View();
        }

        [HttpPost]
        public PartialViewResult SMT_OrderMangage(string FinishStatus)
        {
            List<SMT_OrderInfo> QueryResult = new List<SMT_OrderInfo>();
            //筛选完成状态
            if (FinishStatus != null)
            {
                if (FinishStatus == "")
                {
                    QueryResult = db.SMT_OrderInfo.ToList();
                }
                else if (FinishStatus == "完成")
                {
                    QueryResult = db.SMT_OrderInfo.Where(c => c.Status == true).ToList();
                }
                else if (FinishStatus == "未完成")
                {
                    QueryResult = db.SMT_OrderInfo.Where(c => c.Status == false).ToList();
                }
            }
            //System.Threading.Thread.Sleep(2000);
            ViewBag.FinishStatus = FinishStatusList();
            return PartialView(QueryResult);

        }
        public ActionResult SMT_OrderInfoCreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SMT_OrderInfoCreate(string records)
        {
            List<SMT_OrderInfo> orders = new List<SMT_OrderInfo>();
           
            ////单个model传入
            //var order = JsonConvert.DeserializeObject<SMT_OrderInfo>(records);
            //if(order!=null)
            //{
            //    if (ModelState.IsValid)
            //    {
            //        db.SMT_OrderInfo.Add(order);
            //        db.SaveChanges();
            //    }
            //    return RedirectToAction("SMT_Mangage");
            //}
            orders = JsonConvert.DeserializeObject<List<SMT_OrderInfo>>(records);
            if (orders!=null)
            {
                foreach (var item in orders)
                {
                    if (ModelState.IsValid)
                    {
                        db.SMT_OrderInfo.Add(item);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("SMT_OrderMangage");
            }
            return View(records);
        }


        public async Task<ActionResult> SMT_OrderInfoEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SMT_OrderInfo record = await db.SMT_OrderInfo.FindAsync(id);
            if (record == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = ProductionLineStatus();
            return View(record);
        }

        [HttpPost]
        public async Task<ActionResult> SMT_OrderInfoEdit([Bind(Include = "Id,OrderNum,LineNum,Quantity,PlatformType,Customer,DeliveryDate,Status")]SMT_OrderInfo record)
        {
            if (ModelState.IsValid)
            {
                db.Entry(record).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("SMT_OrderMangage");
            }
            return View(record);
        }

        #endregion



        #region---------------------用户管理

        // GET: SMT用户管理
        public ActionResult SMT_UserMangage()
        {
            List<Users> SMT_User = db.Users.Where(c => c.Department == "SMT" && c.Role!="经理").ToList();
            return View(SMT_User);
        }


        // GET: SMT_User/Details/5
        public async Task<ActionResult> SMT_UserDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: SMT_User/Create
        public ActionResult SMT_UserCreate()
        {
            return View();
        }

        // POST: Packagings/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SMT_UserCreate([Bind(Include = "ID,UserNum,UserName,Password,CreateDate,Creator,UserAuthorize,Role,Department,LineNum,Deleter,DeleteDate,Description")] Users user)
        {
            user.Department = "SMT";
            user.CreateDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                //return RedirectToAction("SMT_UserMangage");
                return Content("<script>alert('用户"+ user.UserName +"添加成功！');window.location.href='../SMT/SMT_UserMangage';</script>");

            }
            return View(user);
        }

        // GET: SMT_User/Edit/5
        public async Task<ActionResult> SMT_UserEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: SMT_User/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SMT_UserEdit([Bind(Include = "ID,UserNum,UserName,Password,CreateDate,Creator,UserAuthorize,Role,Department,LineNum,Deleter,DeleteDate,Description")] Users user)
        {
            user.Department = "SMT";
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //return RedirectToAction("SMT_UserMangage");
                return Content("<script>alert('用户信息已成功修改！');window.location.href='../SMT_UserMangage';</script>");
            }
            return View(user);
        }

        // GET: SMT_User/Delete/5
        public async Task<ActionResult> SMT_UserDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: SMT_User/Delete/5
        [HttpPost, ActionName("SMT_UserDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Users user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            //return RedirectToAction("SMT_UserMangage");
            return Content("<script>alert('用户已经成功删除！');window.location.href='../SMT_UserMangage';</script>");
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


        #endregion


        #region------------------生产信息
        // GET: SMT总览表
        public ActionResult SMT_ProductionInfo()
        {
            return View();
        }

        //产线看板页面
        //[HttpPost]
        public ActionResult SMT_ProductionLineInfo(int LineNum)
        {
            //内容:痴线号，时间，班组，组长，正在生产的订单，良品数量，不良品数量，不良率，产线累计个数，订单完成率，今天计划订单，产线状态
            ViewBag.LineNum = LineNum;//获取产线号


            return View();
        }

        #endregion


        #region------------------生产操作(数据)
        // GET: SMT产线未段工位输入操作
        public ActionResult SMT_Operator()
        {
            //内容：用户名，产线号，正在生产的订单，时间，今天生产的订单及数量（良品、不良品）
            return View();
        }

        [HttpPost]
        //public ActionResult SMT_Operator(string OrderNum, int LineNum, string Result)
        public ActionResult SMT_Operator(FormCollection fc)
        {

            string ordernum = fc["OrderNum"];
            int linenum = Convert.ToInt32(fc["LineNum"]);
            string result = fc["Result"];

            return View();
        }
        #endregion



        #region------------------其他方法

        //// GET: SMT/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: SMT/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: SMT/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: SMT/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: SMT/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: SMT/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: SMT/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        #endregion

        #region ---------------------------------------FinishStatus列表
        private List<SelectListItem> FinishStatusList()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "完成",
                    Value = "完成"
                },
                new SelectListItem
                {
                    Text = "未完成",
                    Value = "未完成"
                }
            };
        }
        #endregion

        #region ---------------------------------------产线状态列表
        private List<SelectListItem> ProductionLineStatus()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "生产中",
                    Value = "生产中"
                },
                new SelectListItem
                {
                    Text = "待料",
                    Value = "待料"
                },
                new SelectListItem
                {
                    Text = "停产",
                    Value = "停产"
                }
            };
        }
        #endregion

        /// <summary>
        /// 对象转换成json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObject">需要格式化的对象</param>
        /// <returns>Json字符串</returns>
        public static string DataContractJsonSerialize<T>(T jsonObject)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            string json = null;
            using (MemoryStream ms = new MemoryStream()) //定义一个stream用来存发序列化之后的内容
            {
                serializer.WriteObject(ms, jsonObject);
                json = Encoding.UTF8.GetString(ms.GetBuffer()); //将stream读取成一个字符串形式的数据，并且返回
                ms.Close();
            }
            return json;
        }

        /// <summary>
        /// json字符串转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">要转换成对象的json字符串</param>
        /// <returns></returns>
        public static T DataContractJsonDeserialize<T>(string json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            T obj = default(T);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
            }
            return obj;
        }

    }
}