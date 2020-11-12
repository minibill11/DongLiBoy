// 设备模块api
import axios from '@/libs/api.request'

//-------------运行状态--------------------
//设备稼动率汇总
export const Equipment_DeviceTime = (data) => {
  //年year,
  //月month
  return axios.request({
    url: 'Equipment_Api/Equipment_DeviceTime',
    data: data,
    method: 'post',
  })
}
//获取设备状态汇总
export const Equipment_Thecurrent = (data) => {
  //年year,
  //月month
  return axios.request({
    url: 'Equipment_Api/Equipment_Thecurrent',
    data: data,
    method: 'post',
  })
}
//获取设备保养汇总
export const Tally_Maintenance = (data) => {
  //年year,
  //月month
  return axios.request({
    url: 'Equipment_Api/Tally_Maintenance',
    data: data,
    method: 'post',
  })
}
//获取设备保养汇总
export const Equipment_EmployTime = (data) => {
  //年year,
  //月month
  //使用部门userDepartment
  return axios.request({
    url: 'Equipment_Api/Equipment_EmployTime',
    data: data,
    method: 'post',
  })
}

//-------------设备台账--------------------
//批量添加设备
export const BatchInputEquipment = (data) => {
  //批量添加表格数据inputList
  return axios.request({
    url: 'Equipment_Api/BatchInputEquipment',
    data: data,
    method: 'post',
  })
}
//设备台账查询
export const ParameterIndex = (data) => {
  // 设备编号equipmentnumber
  // 品牌brand
  // 资产编号assetnumber
  // 设备名称equipmentname
  // 型号规格modelspecification
  // 使用部门userdepartment
  // 存放地点storageplace
  // 车间workshop
  // 工段section
  // 使用状态status
  // 备注remark
  return axios.request({
    url: 'Equipment_Api/Index',
    data: data,
    method: 'post',
  })
}
//获取设备编号
export const EQNumberList = (data) => {
  return axios.request({
    url: 'Equipment_Api/EQNumberList',
    data: data,
    method: 'post',
  })
}
//设备台账查询
export const EquipmentNumberListPrint = (data) => {
  // 数组list
  // 打印数量pagecount
  // 浓度concentration
  // 打印机ip
  // 端口port
  return axios.request({
    url: 'Equipment_Api/EquipmentNumberListPrint',
    data: data,
    method: 'post',
  })
}

//设备履历---------------------------------
//设备状态使用率（停机、运行、维修）
export const Equipment_Timeusage = (data) => {
  // 设备编号equipmentNumber
  // 使用部门userDepartment
  // 年year
  // 月month
  return axios.request({
    url: 'Equipment_Api/Equipment_Timeusage',
    data: data,
    method: 'post',
  })
}
//设备信息详情页
export const Parameter_Details = (data) => {
  // 设备编号equipmentNumber
  return axios.request({
    url: 'Equipment_Api/Details',
    data: data,
    method: 'post',
  })
}
//设备信息详情页
export const Edit_Equipmentbasic = (data) => {
  // 设备编号equipmentNumber
  // 资产编号assetNumber
  // 设备名称equipmentName
  // 使用部门userdepartment
  // 状态status
  // 型号规格modelspeci
  // 出厂编号manufacturingNumber
  // 品牌brand
  // 供应商supplier
  // 购入日期purchaseDate
  // 启用日期actionDate
  // 备注remark
  // 存放地点storagePlace
  return axios.request({
    url: 'Equipment_Api/Edit_Equipmentbasic',
    data: data,
    method: 'post',
  })
}
//上传设备照片(jpg)方法
export const UploadEquipmentPicture = (data) => {
  // 设备编号equipmentNumber
  // 图片uploadfile
  return axios.request({
    url: 'Equipment_Api/UploadEquipmentPicture',
    data: data,
    method: 'post'
  })
}
//批量上传维修记录
export const BatchAdd_Rrecord = (data) => {
  //维修记录records
  //设备编号equipmentNumber
  //资产编号assetNumber
  //设备名称equipmentName
  //使用部门userDepsrtment
  //设备状态status
  return axios.request({
    url: 'Equipment_Api/BatchAdd_Rrecord',
    data: data,
    method: 'post',
  })
}
//添加维修履历汇总记录
export const AddEquipmentStatusRecordAsync = (data) => {
  // 设备编号EquipmentNumber
  // 资产编号AssetNumber
  // 设备名称EquipmentName
  // 订单号OrderNum
  // 设备状态Status
  // 状态开始时间StatusStarTime
  // 报修人ReportRepairMan
  // 故障现象描述FailureDescription
  // 原因分析Reason
  // 维修或检测内容RepairOrTestContent
  // 维修接单时间GetJobTime
  // 计划完成时间PlanFinishTime
  // 实际完成时间ActualFinishTime
  // 维修人RepairMan
  // 配件信息SparePartsInfo
  // 使用部门UserDepartment
  // 车间WorkShop
  // 产线号LineNum
  // 工段Section
  // 记录修改人Modifier
  // 备注Remark
  return axios.request({
    url: 'Equipment_Api/AddEquipmentStatusRecordAsync',
    data: data,
    method: 'post',
  })
}
//添加维修履历汇总记录
export const EditEquipmentStatusRecordAsync = (data) => {
  // 设备编号EquipmentNumber
  // 资产编号AssetNumber
  // 设备名称EquipmentName
  // 订单号OrderNum
  // 设备状态Status
  // 状态开始时间StatusStarTime
  // 报修人ReportRepairMan
  // 故障现象描述FailureDescription
  // 原因分析Reason
  // 维修或检测内容RepairOrTestContent
  // 维修接单时间GetJobTime
  // 计划完成时间PlanFinishTime
  // 实际完成时间ActualFinishTime
  // 维修人RepairMan
  // 配件信息SparePartsInfo
  // 使用部门UserDepartment
  // 车间WorkShop
  // 产线号LineNum
  // 工段Section
  // 记录修改人Modifier
  // 备注Remark
  return axios.request({
    url: 'Equipment_Api/EditEquipmentStatusRecordAsync',
    data: data,
    method: 'post',
  })
}

//关键元器件清单---------------------------------
//根据‘设备编号’查询关键元器件清单
export const Keyinquire = (data) => {
  //设备编号equipmentNumber
  return axios.request({
    url: 'Equipment_Api/Keyinquire',
    data: data,
    method: 'post',
  })
}
//修改关键元器件清单
export const Equipment_EditComponet = (data) => {
  // 设备编号equipmentNumber
  // 品名descrip
  // 规格型号specifica
  // 用途materused
  // 备足remark:
  // id
  return axios.request({
    url: 'Equipment_Api/Equipment_EditComponet',
    data: data,
    method: 'post',
  })
}
//批量上传关键元器件清单
export const Keycomponents_query = (data) => {
  // 上传数据inputList
  return axios.request({
    url: 'Equipment_Api/Keycomponents_query',
    data: data,
    method: 'post',
  })
}
//删除关键元器件
export const DeleteKeycom = (data) => {
  // id
  return axios.request({
    url: 'Equipment_Api/DeleteKeycom',
    data: data,
    method: 'post',
  })
}

//产线设备---------------------------------
//从空压机汇总表获取实时设备状态
export const KongYa_Status = () => {
  return axios.request({
    url: 'Equipment_Api/KongYa_Status',
    method: 'post',
  })
}
//获取产线设备数据
export const Line_Index3 = (data) => {
  // 使用部门departmentlist
  return axios.request({
    url: 'Equipment_Api/Index3',
    data:data,
    method: 'post',
  })
}
//添加产线
export const ADDLineNum = (data) => {
  // 使用部门usedepartment
  // 产线lineNum
  // 设备编号列表equipmentNumberlist
  return axios.request({
    url: 'Equipment_Api/ADDLineNum',
    data:data,
    method: 'post',
  })
}
//添加设备
export const ADDEquipment = (data) => {
  // 设备信息EquipmentSetStation
  return axios.request({
    url: 'Equipment_Api/ADDEquipment',
    data:data,
    method: 'post',
  })
}
//删除设备
export const Equipment_Del = (data) => {
  // 设备编号equipmentNumber
  //id
  return axios.request({
    url: 'Equipment_Api/Equipment_Del',
    data:data,
    method: 'post',
  })
}
//同时修改两个表的设备状态（Status）
export const Equipment_state = (data) => {
  // 使用部门userdepar
  // 产线lineNum
  // 设备编号数组equipmentNumberlist
  // 设备状态status
  return axios.request({
    url: 'Equipment_Api/Equipment_state',
    data:data,
    method: 'post',
  })
}
//根据设备编号获取设备详细信息
export const Particulars = (data) => {
  // 设备编号equipmentNumber
  return axios.request({
    url: 'Equipment_Api/Particulars',
    data:data,
    method: 'post',
  })
}
//迁移设备
export const Migration = (data) => {
  // 使用部门userdepar
  // 产线lineNum
  // 设备编号equipmentNumber
  // 位置号stationnum
  // 车间workShop
  // 工段section
  // 备注remark
  return axios.request({
    url: 'Equipment_Api/Migration',
    data:data,
    method: 'post',
  })
}
//显示预览图片
export const Line_Details = (data) => {
  // 设备编号equipmentNumber
  return axios.request({
    url: 'Equipment_Api/Details',
    data:data,
    method: 'post',
  })
}


//-------------点检保养记录--------------------

//点检查询页面---------------------------------
//根据部门查找当天是否已点检的设备
export const Checked_maintenance = (data) => {
  //设备编号equipmentNumber,
  //设备名称equipmentName,
  //使用部门userdepartment,
  //线别号lineNum
  //时间time
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
export const Equipment_Tally_maintenance_Add = (data) => {
  //设备保养点检表所有数据
  return axios.request({
    url: 'Equipment_Api/Equipment_Tally_maintenance_Add',
    data: data,
    method: 'post',
  })
}
//设备点检保养记录修改
export const Equipment_Tally_maintenance_Edit = (data) => {
  //设备保养点检表所有数据
  return axios.request({
    url: 'Equipment_Api/Equipment_Tally_maintenance_Edit',
    data: data,
    method: 'post',
  })
}
//点检保养表详细页---------------------------------
//获取设备管理:点检保养记
export const TallyEquipment_main = (data) => {
  //使用部门userDepartment,
  //年year,月month
  return axios.request({
    url: 'Equipment_Api/TallyEquipment_main',
    data: data,
    method: 'post',
  })
}

//-----------------------------------------------
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
//-----------------------------------------------




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

//----------------安全库存清单--------------------
//获取使用部门
export const Safety_department = () => {
  return axios.request({
    url: 'Equipment_Api/Safety_department',
    method: 'post',
  })
}
//获取规格型号
export const Safety_specifica = () => {
  return axios.request({
    url: 'Equipment_Api/Safety_specifica',
    method: 'post',
  })
}
//获取品名
export const Safety_descrip = () => {
  return axios.request({
    url: 'Equipment_Api/Safety_descrip',
    method: 'post',
  })
}
//获取配料料号
export const Safety_material = () => {
  return axios.request({
    url: 'Equipment_Api/Safety_material',
    method: 'post',
  })
}
//获取设备名称
export const Safety_equipmentName = () => {
  return axios.request({
    url: 'Equipment_Api/Safety_equipmentName',
    method: 'post',
  })
}
//按使用部门，设备名称，品名，规格/型号，配料料号查询数据
export const Safetyquery = (data) => {
  // 使用部门userdepartment
  // 设备名称equipmentName
  // 品名descrip
  // 规格/型号specifica
  // 配料料号material
  // 年year
  // 月month
  return axios.request({
    url: 'Equipment_Api/Safetyquery',
    data:data,
    method: 'post',
  })
}
//批量上传安全库存清单
export const ADDsafestock = (data) => {
  // 批量上传inputList
  // 年year
  // 月month
  // 部门userdepartment
  return axios.request({
    url: 'Equipment_Api/ADDsafestock',
    data: data,
    method: 'post',
  })
}
//编辑修改安全库存清单
export const Modifi_safety = (data) => {
  // 修改表格equipment_Safetystock
  return axios.request({
    url: 'Equipment_Api/Modifi_safety',
    data: data,
    method: 'post',
  })
}
//编辑修改安全库存清单
export const Verification = (data) => {
  // 使用部门userdepartment
  // 技术部审核tec_asse
  // PMC部审核mcdepsr
  // 工厂厂长批准approve
  // 年year
  // 月month
  return axios.request({
    url: 'Equipment_Api/Verification',
    data: data,
    method: 'post',
  })
}

//----------------指标达成率--------------------
//查询设备月保养质量目标达成状况统计表
export const Equipment_Quality_statistical = (data) => {
  // 年year
  // 月month
  return axios.request({
    url: 'Equipment_Api/Equipment_Quality_statistical',
    data: data,
    method: 'post',
  })
}
//获取设备的使用部门列表
export const Userdepartment_list = () => {
  // 年year
  // 月month
  return axios.request({
    url: 'Equipment_Api/Userdepartment_list',
    method: 'post',
  })
}
//保存左边的数据
export const Equipment_QualityADD = (data) => {
  // 批量上传inputList
  // 备注remark
  // 年year
  // 月month
  return axios.request({
    url: 'Equipment_Api/Equipment_QualityADD',
    data: data,
    method: 'post',
  })
}
//修改左边的数据
export const Modifi_Equipment_Quality = (data) => {
  // 使用部门userdepartment
  // 质量目标quality_objec
  // 目标值target_value
  // 计算公式formulas
  // 统计周期statistical
  // 按规定要求保养台月次required_maintain，
  // 计划保养总台月次planned_maintenance
  // 有效率with_efficiency
  // 年year
  // 月month
  return axios.request({
    url: 'Equipment_Api/Modifi_Equipment_Quality',
    data: data,
    method: 'post',
  })
}
//修改备注和审核、批准周保养质量目标达成状况统计表
export const Editequipment_Quality = (data) => {
  // 备注remark
  // 编制人preparName
  // 审核assessor
  // 批准approve
  // 年year
  // 月month
  return axios.request({
    url: 'Equipment_Api/Editequipment_Quality',
    data: data,
    method: 'post',
  })
}
//保存当月数据周保养质量目标达成状况统计表
export const ADDequipment_quality = (data) => {
  // 月保养质量目标达成状况统计表Quality_target
  // 备注remark
  // 审核人assessor
  // 年year
  // 月month
  return axios.request({
    url: 'Equipment_Api/ADDequipment_quality',
    data: data,
    method: 'post',
  })
}
//设备指标达成率（自动保存上个月的数据）
export const Equipment_Automatically = (data) => {
  //目标年old_year
  //目标月old_month
  //复制年new_year
  //复制月new_month
  return axios.request({
    url: 'Equipment_Api/Equipment_Automatically',
    data: data,
    method: 'post',
  })
}

//----------------扫码点检--------------------
//根据设备编号、时间，类型显示数据
export const Tally_ScanCode = (data) => {
  // 设备编号equipmentNumber
  // 时间time
  // 类型type
  return axios.request({
    url: 'Equipment_Api/Tally_ScanCode',
    data: data,
    method: 'post',
  })
}
//保存PDA点检数据
export const Save_TallyData = (data) => {
  // 点检保养表tally_Maintenances
  // 设备编号equipmentNumber
  // 使用部门userDepartment
  // 时间time
  // 类型type
  return axios.request({
    url: 'Equipment_Api/Save_TallyData',
    data: data,
    method: 'post',
  })
}
