import axios from '@/libs/api.request'

////扣分标准录入页面-----获取检查项目
export const getCheckItem = ()=> {
    return axios.request({
        url: 'KPI_Api/GainPointsType',    
        method: 'post'
    })
}

////扣分标准录入页面-----确认上传
export const uploadData = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/ReferenceStandard_Input', 
        data:{
            record:parameters
        },   
        method: 'post'
    })
}

////扣分标准录入页面-----删除记录
export const delData = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/ReferenceStandard_del', 
        data:{
            id:parameters
        },   
        method: 'post'
    })
}

////扣分标准录入页面-----查询
export const queryData = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/ReferenceStandard_query', 
        data:{
            pointsType:parameters
        },   
        method: 'post'
    })
}

////扣分标准录入页面-----修改
export const modifyData = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/ReferenceStandard_modify', 
        data:{
            id: parameters.id,
            pointsType: parameters.pointsType, 
            referenceStandard: parameters.referenceStandard
        },   
        method: 'post'
    })
}



//下拉选择框组件方法----获取部门
export const regionGetDep = ()=> {
    return axios.request({
        url: 'KPI_Api/GainDepartmentList',   
        method: 'post'
    })
}

//下拉选择框组件方法-----区域
export const regionGetDis = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/GainDistrictList',   
        data:{
            department: parameters.department,
            position: parameters.position
        },
        method: 'post'
    })
}

//下拉选择框组件方法-----获取位置
export const regionGetPosi = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/GainPositionList',   
        data:{
            department: parameters.department
        },
        method: 'post'
    })
}

//下拉选择框组件方法-----获取位置
export const regionGetVTime = ()=> {
    return axios.request({
        url: 'KPI_Api/GainVersions',          
        method: 'post'
    })
}

//数据区域录入页面-----查询数据
export const regionQuery = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/Region_query',   
        data:{
            department: parameters.department,
            position: parameters.position, 
            district: parameters.district,
            versionsTime: parameters.versionsTime
        },       
        method: 'post'
    })
}

//数据区域录入页面-----单条数据编辑保存
export const saveEditData = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/Region_modify',   
        data:{
            id: parameters.id,
            department: parameters.department,
            group: parameters.group,
            position: parameters.position,
            district: parameters.district,
            targetValue: parameters.targetValue
        },       
        method: 'post'
    })
}

//数据区域录入页面-----单条数据编辑删除
export const delEditData = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/Region_del',   
        data:{
            id: parameters,           
        },       
        method: 'post'
    })
}

//数据区域录入页面-----文件上传保存
export const saveFileData = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/Region_Input',   
        data:{
            record: parameters,           
        },       
        method: 'post'
    })
}




//部门班组扣分录入页面-----获取限期整改日期
export const getVersionT = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/GainTime', 
        data:{
            date: parameters,   
        },             
        method: 'post'
    })
}


//部门班组扣分录入页面-----根据版本时间找出全部区域号
export const getAllRegin = (parameters)=> {
    console.log(1111,parameters)
    return axios.request({
        url: 'KPI_Api/GainDistrict', 
        data:{
            date: parameters,   
        },            
        method: 'post'
    })
}


//部门班组扣分录入页面-----获取表格选项
export const getTableSelect = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/GainInputData', 
        data:{
            time: parameters,   
        },       
        method: 'post'
    })
}

//部门班组扣分录入页面------根据区域号找位置、部门
export const getPsionDep = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/GainPosition', 
        data:{
            district: parameters.district,  
            date: parameters.date
        },            
        method: 'post'
    }) 
}

//部门班组扣分录入页面------获取上传后的图片
export const gainImage = (parameters)=> {
    return axios.request({
        url: 'KPI_Api/LookImage', 
        data:{
            department:parameters.department,
            position:parameters.position,
            check_date:parameters.check_date,
            check_Type:parameters.check_Type,
            pointsDeducted_Type:parameters.pointsDeducted_Type,
            uploadType:parameters.uploadType,
            district:parameters.district,  
        },                 
        method: 'post'
    })
}

//部门班组扣分录入页面------删除上传后的图片
export const DeleteImage = (data)=> {
    return axios.request({
        url: 'KPI_Api/DelImg', 
        data,                 
        method: 'post'
    })
}

//部门班组扣分录入页面------检查记录是否重复上传
export const CheckRepeat = (data)=> {
    return axios.request({
        url: 'KPI_Api/Check_Record', 
        data:{
            record: data.record, formInfo: data.formInfo 
        },               
        method: 'post'
    })
}


//部门班组扣分录入页面------检查记录是否重复上传
export const  SaveRecord= (data)=> {
    return axios.request({
        url: 'KPI_Api/RecordInput', 
        data:{
            record: data.record, 
            formInfo: data.formInfo 
        },               
        method: 'post'
    })
}





//7S评比得分汇总表页面-------获取班组
export const  GetGroupList= (department)=> {
    return axios.request({
        url: 'KPI_Api/GroupList', 
        data:{
            department
        },              
        method: 'post'
    })
}


//7S评比得分汇总表页面-------获取班组
export const  GetDistrctList= (parameters)=> {
    return axios.request({
        url: 'KPI_Api/PositionSelect', 
        data:{
            department:parameters.department,
            group:parameters.group
        },              
        method: 'post'
    })
}

//7S评比得分汇总表页面-------查询表格数据
export const  GetTableList= (parameters)=> {
    return axios.request({
        url: 'KPI_Api/KPI_7S_SummarySheet', 
        data:{
            department: parameters.department,
            position: parameters.position,
            district: parameters.district,
            date: parameters.date,
            group: parameters.group,
        },              
        method: 'post'
    })
}


//7S评比得分排名页面-------查询表格数据
export const  getRankingData= (time)=> {
    return axios.request({
        url: 'KPI_Api/KPI_7S_Ranking', 
        data:{
            time
        },              
        method: 'post'
    })
}


//7S日检查询页面-------查询表格数据
export const  GetDailyRecorData= (data)=> {
    return axios.request({
        url: 'KPI_Api/DailyRecord_Query', 
        data:{
            department: data.department,
            date: data.date
        },              
        method: 'post'
    })
}




//日检查询页面-------查询表格数据
export const  QueryWeekData= (param)=> {
    return axios.request({
        url: 'KPI_Api/Week_SumQuery', 
        data:{
            department:param.department,
            date:param.date,
            position:param.position,
            district:param.district,
        },              
        method: 'post'
    })
}



//日检查询页面-------查询表格数据
export const  QueryDailyData= (param)=> {
    return axios.request({
        url: 'KPI_Api/Daily_SumQuery', 
        data:{
            check_Type:param.check_Type,
            department:param.department,
            date:param.date,
            position:param.position,
            district:param.district,
        },              
        method: 'post'
    })
}


//日检周检巡检详细页面-------日周查询
export const  QueryDailyWeek= (param)=> {
    return axios.request({
        url: 'KPI_Api/Detail_Query', 
        data:{
            department: param.department,
            position: param.position,
            district: param.district,
            check_Type: param.check_Type,
            date: param.date,
            week: param.week
        },              
        method: 'post'
    })
}

//日检周检巡检详细页面-------月查询
export const  QueryDailyMonth= (param)=> {
    return axios.request({
        url: 'KPI_Api/Month_Query', 
        data:{
            department: param.department,
            position: param.position,
            district: param.district,
            check_Type: param.check_Type,
            date: param.date,
        },              
        method: 'post'
    })
}



//详细页-------上传图片
export const  DetailUpImage= (data)=> {
    return axios.request({
        url: 'KPI_Api/ImageUpload', 
        data:data,              
        method: 'post'
    })
}

//详细页------限期整改确认
export const  DetailApprove= (data)=> {
    return axios.request({
        url: 'KPI_Api/Approve', 
        data:{
            id:data.id,
            result:data.result
        },              
        method: 'post'
    })
}


//详细页------修改整改时间
export const  DetailModifyDate= (data)=> {
    return axios.request({
        url: 'KPI_Api/RectificationTime_modify', 
        data:{
            id:data.id,
            time:data.time
        },              
        method: 'post'
    })
}


//详细页------删除整行数据
export const  DetailDeleteDate= (data)=> {
    return axios.request({
        url: 'KPI_Api/KPI_7S_Del', 
        data:{
            id:data
        },              
        method: 'post'
    })
}