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
  return axios.request({
    url: 'Users_Api/RolesNameList',
    data: {
      FourModule: FourModule
    },
    method: 'post',
  })
}
//根据模块、权限组和根据模块、权限组、部门/姓名显示数据
export const DiscriptionList = (data) => {
  //模块FourModule
  //权限组RolesName
  //部门Department
  //姓名UserName
  return axios.request({
    url: 'Users_Api/DiscriptionList',
    data: data,
    method: 'post',
  })
}
//根据模块、权限组和根据模块、权限组、部门/姓名显示数据
export const Cancel_Authorization = (data) => {
  //模块FourModule
  //权限组RolesName
  //部门Department
  //姓名UserName
  //用户UserID
  //用户职位Position
  //删除数据Delete
  //添加数据Add
  return axios.request({
    url: 'Users_Api/Cancel_Authorization',
    data: data,
    method: 'post',
  })
}
//添加模块
export const AddModule = (FourModule) => {
  return axios.request({
    url: 'Users_Api/AddModule',
    data: {
      FourModule: FourModule
    },
    method: 'post',
  })
}
//修改模块
export const ModifyModule = (data) => {
  //模块名fourModule
  //新模块名new_fourModule
  //权限组rolesName
  return axios.request({
    url: 'Users_Api/ModifyModule',
    data: data,
    method: 'post',
  })
}
//添加权限组，权限名
export const AddPermission = (data) => {
  //模块名fourModule
  //权限名discription
  //添加权限组rolesName
  return axios.request({
    url: 'Users_Api/AddPermission',
    data: data,
    method: 'post',
  })
}
//修改权限组
export const ModifyRolesName = (data) => {
  //新权限组new_rolesName
  //权限组rolesName
  return axios.request({
    url: 'Users_Api/ModifyRolesName',
    data: data,
    method: 'post',
  })
}
//修改权限名
export const ModifyDiscription = (data) => {
  //权限组rolesName
  //权限名discription
  //新权限名new_discription
  return axios.request({
    url: 'Users_Api/ModifyDiscription',
    data: data,
    method: 'post',
  })
}
