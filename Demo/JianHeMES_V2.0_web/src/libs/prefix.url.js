import config from '@/config'
var baseUrl;
const host = 'http://' + window.location.host;
if (process.env.NODE_ENV === 'development') {
  baseUrl = config.baseUrl.dev;
} else {
  if(host.indexOf("172")!=-1){
    baseUrl = config.baseUrl.pro;
  } else{
    baseUrl = config.baseUrl.proTwo;
  }
}
export default baseUrl