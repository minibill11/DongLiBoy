import HttpRequest from '@/libs/axios'
import config from '@/config'
var baseUrl;
const host = 'http://' + window.location.host;
// console.log('浏览器host地址：',host);
// console.log(host.indexOf("172")!=-1,'true:跳内网api；false：跳外网api');
if (process.env.NODE_ENV === 'development') {
  baseUrl = config.baseUrl.dev;
} else {
  if(host.indexOf("172")!=-1){
    baseUrl = config.baseUrl.pro;
  } else{
    baseUrl = config.baseUrl.proTwo;
  }
}


const axios = new HttpRequest(baseUrl)
export default axios
