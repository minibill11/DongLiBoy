// 系统管理模块api
import axios from '@/libs/api.request'

//-------------权限管理--------------------
//获取权限管理左侧大模块列表
export const Permiss_FourModuleList = () => {
  return axios.request({
    url: 'Users_Api/Permiss_FourModuleList',
    method: 'post',
  })
}
//获取权限管理左侧大模块子项列表
export const RolesNameList = (FourModule) => {
    const data = {
        FourModule:FourModule
    }
  return axios.request({
    url: 'Users_Api/RolesNameList',
    data:data,
    method: 'post',
  })
}
//根据模块、权限组和根据模块、权限组、部门/姓名显示数据
export const DiscriptionList = (FourModule,RolesName,Department,UserName) => {
    const data = {
        FourModule:FourModule,
        RolesName:RolesName,
        Department:Department,
        UserName:UserName
    }
  return axios.request({
    url: 'Users_Api/DiscriptionList',
    data:data,
    method: 'post',
  })
}

