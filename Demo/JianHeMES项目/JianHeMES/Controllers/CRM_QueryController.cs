using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JianHeMES.Controllers
{
    public class CRM_QueryController : ApiController
    {
        [HttpPost]
        public string OrderInfo(string parameter)
        {
            //var prepare = JsonConvert.DeserializeObject(parameter);//字符串转成json

            string result = "MES接收到的内容是：" + parameter ;

            return result;
        }

        [HttpPost]
        [Authorize]
        public string OrderInfo2(string parameter)
        {
            //var prepare = JsonConvert.DeserializeObject(parameter);//字符串转成json

            string result = "MES接收到的内容是：" + parameter;

            return result;
        }

        // GET: api/CRM_Query
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CRM_Query/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CRM_Query
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/CRM_Query/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CRM_Query/5
        public void Delete(int id)
        {
        }
    }
}
