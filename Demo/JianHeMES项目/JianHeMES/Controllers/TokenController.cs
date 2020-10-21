﻿using JianHeMES.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace JianHeMES.Controllers
{
    [RoutePrefix("api/Token")]
    public class TokenController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CommonalityController comm = new CommonalityController();

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public JObject Login([FromBody] JObject data)
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var loginRequest = JsonConvert.DeserializeObject<LoginRequest>(jsonStr);
            TokenInfo tokenInfo = new TokenInfo();//需要返回的口令信息
            int userNumber = Convert.ToInt32(loginRequest.UserNumber);
            var userExistList = db.Users.Where(u => u.UserNum == userNumber).ToList();//用户存在
            var user = userExistList.Where(u => u.PassWord == loginRequest.Password).FirstOrDefault();
            if (user != null)
            {

                //string userNum = loginRequest.UserNumber;
                string passWord = loginRequest.Password;
                bool isAdmin = user.Role == "系统管理员" ? true : false;

                //模拟数据库数据，真正的数据应该从数据库读取
                //身份验证信息
                AuthInfo authInfo = new AuthInfo { UserNumber = userNumber.ToString("D5"),UserName = user.UserName, Roles = new List<string> { "admin", "commonrole" }, IsAdmin = isAdmin, ExpiryDateTime = DateTime.Now.AddHours(5) };
                const string secretKey = "JianHeMES";//口令加密秘钥
                try
                {
                    byte[] key = Encoding.UTF8.GetBytes(secretKey);
                    IJwtAlgorithm algorithm = new HMACSHA256Algorithm();//加密方式
                    IJsonSerializer serializer = new JsonNetSerializer();//序列化Json
                    IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();//base64加解密
                    IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);//JWT编码
                    var token = encoder.Encode(authInfo, key);//生成令牌
                    //口令信息
                    tokenInfo.Success = true;
                    tokenInfo.Token = token;
                    tokenInfo.Message = "登录成功！";
                }
                catch (Exception ex)
                {
                    tokenInfo.Success = false;
                    tokenInfo.Message = ex.Message.ToString();
                }
                JObject result = new JObject();
                result.Add("PostResult", comm.ReturnApiPostStatus());
                result.Add("tokenInfo", (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(tokenInfo)));
                return result;
            }
            else
            {
                if (userExistList.Count > 0)
                {
                    tokenInfo.Success = false;
                    tokenInfo.Message = "密码不正确！";
                }
                else
                {
                    tokenInfo.Success = false;
                    tokenInfo.Message = "用户名不存在！";
                }
                JObject result = new JObject();
                var postresult = comm.ReturnApiPostStatus();
                postresult["PostSuccess"] = false;
                postresult["AuthorizationResult"] = false;
                postresult["Message"] = "未通过授权验证！Post请求失败！";
                result.Add("PostResult", postresult);
                result.Add("tokenInfo", (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(tokenInfo)));
                return result;

            }
        }


        // GET: api/Token
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Token/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Token
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Token/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Token/5
        public void Delete(int id)
        {
        }
    }
}
