using JianHeMES.Models;
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
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace JianHeMES.AuthAttributes
{
    /// <summary>
    /// 身份认证拦截器
    /// </summary>
    public class ApiAuthorizeAttribute: AuthorizeAttribute
    {
        /// <summary>
        /// 指示指定的控件是否已获得授权
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //前端请求api时会将token存放在名为"auth"的请求头中
            var content = actionContext.RequestContext;
            ////string actstring = JsonConvert.SerializeObject(actionContext);
            //var vvv = actionContext.Request.Headers;
            //var data = actionContext.ControllerContext.Request.RequestUri.Query;
            //var sdata = data.Split('=');
            var authHeader = from t in actionContext.Request.Headers where t.Key == "Authorization" select t.Value.FirstOrDefault();
            if (authHeader != null)
            {
                const string secretKey = "JianHeMES";//加密秘钥
                string token = authHeader.FirstOrDefault();//获取token
                //string token = sdata[sdata.Length - 1];//获取token
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        byte[] key = Encoding.UTF8.GetBytes(secretKey);
                        IJsonSerializer serializer = new JsonNetSerializer();
                        IDateTimeProvider provider = new UtcDateTimeProvider();
                        IJwtValidator validator = new JwtValidator(serializer, provider);
                        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                        IJwtAlgorithm algorithm = new HMACSHA256Algorithm();//加密方
                        IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder , algorithm);
                        //解密
                        var json = decoder.DecodeToObject<AuthInfo>(token, key, verify: true);
                        //string userNumber = json.UserName;//传入的token信息中取出用户名
                        if (json != null)
                        {
                            //判断口令过期时间
                            if (json.ExpiryDateTime < DateTime.Now)
                            {
                                return false;
                            }
                            actionContext.RequestContext.RouteData.Values.Add("Authorization", json);
                            return true;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 处理授权失败的请求
        /// </summary>
        /// <param name="actionContext"></param>
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var erModel = new
            {
                AuthorizationResult =false,
                PostSuccess = false,
                ErrorCode = 401,
                Message ="未通过授权验证！Post请求失败！"
            };
            JObject result = new JObject();
            result.Add("PostResult",(JObject)JsonConvert.DeserializeObject((JsonConvert.SerializeObject(erModel))));
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, result, "application/json");
        }

        /// <summary>
        ///  为操作授权时调用
        /// </summary>
        /// <param name="actionContext"></param>
        //public override void OnAuthorization(HttpActionContext actionContext)
        //{

        //}
    }
}