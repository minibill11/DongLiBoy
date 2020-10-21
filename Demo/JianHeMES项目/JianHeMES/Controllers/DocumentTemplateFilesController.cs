using JianHeMES.AuthAttributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES.Controllers
{
    public class DocumentTemplateFilesController : Controller
    {
        // GET: BGManage
        public ActionResult Index()
        {
            return View();
        }

    }

    public class DocumentTemplateFiles_ApiController : System.Web.Http.ApiController
    {
        private CommonalityController comm = new CommonalityController();


        //读取模板文件夹的文档清单
        [HttpPost]
        [ApiAuthorize]
        public JObject DocumentTemplateFilesFolderInDirectory()
        {
            JObject result = new JObject();
            string folder = @"D:\MES_Data\DocumentTemplateFiles\";
            if (System.IO.Directory.Exists(folder))
            {
                var file_list = comm.GetFilesInDirectory(folder).Select(c=>c.Name).ToArray();
                result.Add("Data",JsonConvert.SerializeObject(file_list));
                result.Add("Result", true);
                result.Add("Message", "文件夹读取成功！");
                result.Add("PostResult", comm.ReturnApiPostStatus());
                return result;
            }
            else
            {
                result.Add("Data", "");
                result.Add("Result", false);
                result.Add("Message", "文件夹读取失败！");
                result.Add("PostResult", comm.ReturnApiPostStatus());
                return result;
            }
        }

        //读取模板文档(不带扩展名)
        [HttpPost]
        [ApiAuthorize]
        public JObject DocumentTemplateFile([System.Web.Http.FromBody]JObject data)
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            var obj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            string filename = obj.filename;
            JObject result = new JObject();
            string folder = @"D:\MES_Data\DocumentTemplateFiles\";
            if (System.IO.Directory.Exists(folder))
            {
                var file_list = comm.GetFilesInDirectory(folder);
                int count = file_list.Count(c => c.Name.Contains(filename));
                if(count>0)
                {
                    string name = file_list.Where(c => c.Name.Contains(filename)).FirstOrDefault().Name;
                    result.Add("Data", "/MES_Data/DocumentTemplateFiles/" + name);
                    result.Add("Result", true);
                    result.Add("Message", "模块文档读取成功！");
                    result.Add("PostResult", comm.ReturnApiPostStatus());
                }
                return result;
            }
            else
            {
                result.Add("Data", "");
                result.Add("Result", false);
                result.Add("Message", "模块文档读取失败，文档不存在！");
                result.Add("PostResult", comm.ReturnApiPostStatus());
                return result;
            }
        }


    }

}