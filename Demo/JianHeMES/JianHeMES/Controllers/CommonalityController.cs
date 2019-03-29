using JianHeMES.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace JianHeMES.Controllers
{
    public class CommonalityController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //[System.Web.Http.Route("Commonality/GetBarcode_Each_Section_Prompt")]
        [HttpPost]
        public string GetBarcode_Each_Section_Prompt(string barcode)
        {
            JObject result = new JObject();
            var Accemble_Record = db.Assemble.Where(c => c.BoxBarCode == barcode && c.PQCCheckFinish == true && c.RepetitionPQCCheck == false).Count();
            result.Add("Accemble_Record", Accemble_Record > 0 ? true : false);
            var FQC_Record = db.FinalQC.Where(c => c.BarCodesNum == barcode && c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Count();
            result.Add("FQC_Record", FQC_Record > 0 ? true : false);
            var Burn_in_Record = db.Burn_in.Where(c => c.BarCodesNum == barcode && c.OQCCheckFinish == true).Count();
            result.Add("Burn_in_Record", Burn_in_Record > 0 ? true : false);
            var Calibration_Record = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && c.Normal == true && c.RepetitionCalibration == false).Count();
            result.Add("Calibration_Record", Calibration_Record > 0 ? true : false);
            var Appearance_Record = db.Appearance.Where(c => c.BarCodesNum == barcode && c.OQCCheckFinish == true).Count();
            result.Add("Appearance_Record", Appearance_Record > 0 ? true : false);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost]
        public string GetBarcode_List_Each_Section_Prompt(List<string> barcodelist)
        {
            JObject result = new JObject();
            foreach (var barcode in barcodelist)
            {
                JObject barcode_result = new JObject();
                var Accemble_Record = db.Assemble.Where(c => c.BoxBarCode == barcode && c.PQCCheckFinish == true && c.RepetitionPQCCheck == false).Count();
                barcode_result.Add("Accemble_Record", Accemble_Record > 0 ? true : false);
                var FQC_Record = db.FinalQC.Where(c => c.BarCodesNum == barcode && c.FQCCheckFinish == true && c.RepetitionFQCCheck == false).Count();
                barcode_result.Add("FQC_Record", FQC_Record > 0 ? true : false);
                var Burn_in_Record = db.Burn_in.Where(c => c.BarCodesNum == barcode && c.OQCCheckFinish == true).Count();
                barcode_result.Add("Burn_in_Record", Burn_in_Record > 0 ? true : false);
                var Calibration_Record = db.CalibrationRecord.Where(c => c.BarCodesNum == barcode && c.Normal == true && c.RepetitionCalibration == false).Count();
                barcode_result.Add("Calibration_Record", Calibration_Record > 0 ? true : false);
                var Appearance_Record = db.Appearance.Where(c => c.BarCodesNum == barcode && c.OQCCheckFinish == true).Count();
                barcode_result.Add("Appearance_Record", Appearance_Record > 0 ? true : false);
                result.Add(barcode, barcode_result);
            }
            return JsonConvert.SerializeObject(result);
        }



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