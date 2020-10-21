// 设备模块api
import axios from '@/libs/api.request'

//-------------点检保养记录--------------------

//点检查询页面---------------------------------
//根据部门查找当天是否已点检的设备
export const Checked_maintenance = (data) => {
  //设备编号equnumber,
  //设备名称equipname,
  //使用部门userdepartment,
  //线别号lineNum
  return axios.request({
    url: 'Equipment_Api/Checked_maintenance',
    data: data,
    method: 'post',
  })
}
//获取所有点检保养的设备编号的方法
export const Tally_List = () => {
  return axios.request({
    url: 'Equipment_Api/TallyList',
    method: 'post',
  })
}
//获取设备点检有数据使用部门的方法
export const Tally_Deparlist = () => {
  return axios.request({
    url: 'Equipment_Api/Tally_Deparlist',
    method: 'post',
  })
}
//根据设备编号获取设备信息
export const EquipmentInfo_getdata_by_eqnum = (data) => {
  //设备编号equipmentNumber
  return axios.request({
    url: 'Equipment_Api/EquipmentInfo_getdata_by_eqnum',
    data: data,
    method: 'post',
  })
}
//修改使用部门
export const LineNameList = (data) => {
  //使用部门newdepartment
  return axios.request({
    url: 'Equipment_Api/LineNameList',
    data: data,
    method: 'post',
  })
}
//修改点检表的使用部门、线别号、设备名称
export const ModifyUseDepartment = (data) => {
  //id
  //使用部门newdepartment
  //产线newLineName
  //设备名称equipmentName
  return axios.request({
    url: 'Equipment_Api/ModifyUseDepartment',
    data: data,
    method: 'post',
  })
}
//可复制上月的点检项目和操作方法到下个月
export const TallySheet_Copy = (data) => {
  //目标年old_year
  //目标月old_month
  //复制年new_year
  //复制月new_month
  //版本号versionNum
  return axios.request({
    url: 'Equipment_Api/TallySheet_Copy',
    data: data,
    method: 'post',
  })
}

//点检保养表详细页---------------------------------
//获取设备管理:点检保养记录表
export const Equipment_Query_Tally = (data) => {
  //设备编号equipmentNumber,
  //年year,月month
  return axios.request({
    url: 'Equipment_Api/Equipment_Query_Tally',
    data: data,
    method: 'post',
  })
}
//设备点检保养记录创建保存
export const Equipment_Tally_maintenance_Add = (equipment_Tally_maintenance) => {
  //设备保养点检表所有数据
  const data = {
    equipment_Tally_maintenance: equipment_Tally_maintenance
  }
  return axios.request({
    url: 'Equipment_Api/Equipment_Tally_maintenance_Add',
    data: data,
    method: 'post',
  })
}
//设备点检保养记录修改
export const Equipment_Tally_maintenance_Edit = (equipment_Tally_maintenance) => {
  //设备保养点检表所有数据
  const data = {
    equipment_Tally_maintenance: equipment_Tally_maintenance
  }
  return axios.request({
    url: 'Equipment_Api/Equipment_Tally_maintenance_Edit',
    data: data,
    method: 'post',
  })
}

//获取点检记录查询方法
export const Equipment_Tally = (data) => {
  //使用部门userdepartment,
  //设备编号equnumber,
  //设备名称equipname,
  //线别号lineNum,
  //年year,月month
  return axios.request({
    url: 'Equipment_Api/Equipment_Tally',
    data: data,
    method: 'post',
  })
}
//获取点检记录数据：详细页
export const Equipment_Tally_maintenance = (data) => {
  //设备编号equnumber,
  //设备ID id
  return axios.request({
    url: 'Equipment_Api/Equipment_Tally_maintenance',
    data: data,
    method: 'post',
  })
}





//-------------报修单查询--------------------
//获取所有仪器设备报修单的设备编号的方法
export const InstrumentList = () => {
  return axios.request({
    url: 'Equipment_Api/InstrumentList',
    method: 'post',
  })
}
//获取设备报修单有数据使用部门的方法
export const Deparlist = () => {
  return axios.request({
    url: 'Equipment_Api/Deparlist',
    method: 'post',
  })
}
//根据部门，设备编号，设备名称，故障时间查询仪器设备报修单---查询页
export const EquipmentRepairbill_Query = (data) => {
  //使用部门userdepartment,
  //设备编号equnumber,
  //设备名称equipname,
  //故障时间date,
  //开始时间（时间段）starttime,
  //结束时间（时间段）endtime,
  //年year,月month
  return axios.request({
    url: 'Equipment_Api/EquipmentRepairbill_Query',
    data: data,
    method: 'post',
  })
}
//-------------报修单详细页--------------------
//根据部门，设备编号，设备名称，故障时间查询仪器设备报修单----详细页
export const EquipmentRepairbill_Detailed = (data) => {
  //设备编号equnumber,
  //故障时间date,
  return axios.request({
    url: 'Equipment_Api/EquipmentRepairbill_Detailed',
    data: data,
    method: 'post',
  })
}
//仪器设备报修单数据创建保存
export const Repairbill_maintenance = (data) => {
  //设备编号EquipmentNumber,
  // 设备名称EquipmentName,
  // 故障时间FaultTime,
  // 紧急状态Emergency,
  // 故障描述FauDescription,
  // 使用部门UserDepartment,
  // 部门Department,班组Group
  return axios.request({
    url: 'Equipment_Api/Repairbill_maintenance',
    data: data,
    method: 'post',
  })
}
//仪器设备报修单修改
export const Modify_repairs = (data) => {
  //设备编号EquipmentNumber,
  // 设备名称EquipmentName,
  // 故障时间FaultTime,
  // 紧急状态Emergency,
  // 故障描述FauDescription,
  // 使用部门UserDepartment,
  // 部门Department,班组Group
  return axios.request({
    url: 'Equipment_Api/Modify_repairs',
    data: data,
    method: 'post',
  })
}
