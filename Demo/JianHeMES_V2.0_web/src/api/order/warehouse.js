import axios from '@/libs/api.request'
//库存金额首页查询
export const getRecord = (time, type) => {
    return axios.request({
        url: 'Warehouse_Material_Api/SA_Index',
        data: {
            year: time,
            queryType: type
        },
        method: 'post'
    })
}

//月度库存金额页面查询
export const getMOnthData = (time) => {
    return axios.request({
        method: 'post',
        url: 'Warehouse_Material_Api/StockAmount_Query',
        data: {
            date: time
        },
    })
}

//月度库存金额下载excel
export const getMOnthExcel = (row, time) => {
    return axios.request({
        method: 'post',
        url: 'Warehouse_Material_Api/LookExcel',
        data: {
            category: row,
            date: time,
        },
    })
}

//月度库存金额核对、审核报告
export const approveReport = (record) => {
    return axios.request({
        method: 'post',
        url: 'Warehouse_Material_Api/Approve',
        data: {
            id:record.id,
            reason:record.reason,
            approve_Type:record.approve_Type,
            approve_Result:record.approve_Result
        },
    })
}

//月度库存金额删除报告
export const deleteReport = (time) => {
    return axios.request({
        method: 'post',
        url: 'Warehouse_Material_Api/reportDelete',
        data: {
            date:time,          
        },
    })
}

//查询ERP库存月度库存金额页面——————查询
export const erpQuery = (date) => {
    console.log(date)
    return axios.request({
        method: 'post',
        url: 'Warehouse_Material_Api/StockAmountCalculate',
        data: {
            year:date.year,
            month:date.month
        },
    })
}

//查询ERP库存月度库存金额页面——————生成报表
export const createReport = (parameter) => {
    return axios.request({
        method: 'post',
        url: 'Warehouse_Material_Api/CreateTable',
        data: {
            record:parameter.record,
            date:parameter.date
        }       
    })
}

//查询ERP库存月度库存金额页面——————导出excel
export const exportReport = (parameter) => {
    return axios.request({
        method: 'post',
        url: 'Warehouse_Material_Api/exportExcel',
        data: {
            tableData:parameter.tableData,
            year:parameter.year,
            month:parameter.month
        },
        responseType: 'blob',
    })
}

//查询ERP库存月度库存金额页面——————分类导出excel文件
export const exportCategoryReport = (parameter) => {
    return axios.request({
        method: 'post',
        url: 'Warehouse_Material_Api/StockAmountCalculateExcel',
        data: {
            outputexcelfile:parameter.outputexcelfile,
            year:parameter.year,
            month:parameter.month
        },
        responseType: 'blob',
    })
}
