// 公共api
import axios from '@/libs/api.request'
import store from '@/store'

//获取条码列表
export const OrderList = () => {
  return axios.request({
    url: 'Common_Api/OrderList ',
    method: 'post',
  })
}
//获取账号班组列表
export const DisplayGroup = () => {
  const data = {
    deparment: store.state.user.userInfo.Department,
  }
  return axios.request({
    url: 'Common_Api/DisplayGroup ',
    data: data,
    method: 'post',
  })
}
//获取当前MES版本号
export const GetLasrVersion = () => {
  return axios.request({
    url: 'VersionManagement_Api/LasrVersion',
    method: 'post',
  })
}
