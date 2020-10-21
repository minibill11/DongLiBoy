import axios from '@/libs/api.request'

export const getList = () => {
        return axios.request({
            url: 'API/GetOrderNumGunter/?ordernum=2017-TEST-1',
            data: {},
            method: 'post'
        })
    }
    // export const getList = () => {
    //     return axios.request({
    //       url: 'api/CRM_Query/OrderInfo/?parameter=2017-TEST-1',
    //       params: {},
    //       method: 'post'
    //     })
    //   }
    // export const getList = () => {
    //     return axios.request({
    //       url: 'Personnel_Framework/Framework2',
    //       params: {},
    //       method: 'get'
    //     })
    //   }
    // export const getListTwo = (Calibration_result) => {
    //     return axios.request({
    //       url: '/api/API/CalibrationApi/?parameter='+Calibration_result,
    //       params: {
    //         Calibration_result:Calibration_result
    //       },
    //       method: 'post'
    //     })
    //   }

//库存金额首页查询
export const getRecord = (time) => {
    return axios.request({
        url: 'Warehouse_Material_Api/SA_Index',
        params: {
            year: time,
            queryType: 'original'
        },
        method: 'post'
    })
}

//月度库存金额页面查询
export const getMOnthData = (time) => {
    return axios.request({
        method: 'post',
        url: 'Warehouse_Material/StockAmount_Query',
        params: {
            date: time
        },
    })
}

//月度库存金额下载excel
export const getMOnthExcel = (row, time) => {
    return axios.request({
        method: 'post',
        url: 'Warehouse_Material/LookExcel',
        params: {
            category: row,
            date: time,
        },
    })
}