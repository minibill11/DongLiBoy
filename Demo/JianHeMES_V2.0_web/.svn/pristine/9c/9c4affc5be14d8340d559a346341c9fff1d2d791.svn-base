import Vue from 'vue'
import config from '@/config'

//用户信息
const userInfo = JSON.parse(localStorage.getItem("store"))?JSON.parse(localStorage.getItem("store")).user.userInfo:''

//用户权限
const limit = function (roleName) {
  let limitList = JSON.parse(localStorage.getItem("store"))?JSON.parse(localStorage.getItem("store")).user.access:''
  let flag = false
  if (limitList && roleName) {
    limitList.forEach(item => {
      if (item == roleName) {
        flag = true;
      }
    })
  }
  return flag
}

//获取调用路径
const loadPath = function(){
  var url;
  const host = 'http://' + window.location.host;
  // console.log('浏览器host地址：',host);
  // console.log(host.indexOf("172")!=-1,'true:跳内网api；false：跳外网api');
  if (process.env.NODE_ENV === 'development') {
    url = config.loadPath.dev;
  } else {
    if(host.indexOf("172")!=-1){
      url = config.loadPath.pro;
    } else{
      url = config.loadPath.proTwo;
    }
  }
  return url;
}

export default {
  userInfo,
  limit
}
// 注册全局变量及全局函数
Vue.prototype.$userInfo = userInfo
Vue.prototype.$limit = limit
Vue.prototype.$loadPath = loadPath()
