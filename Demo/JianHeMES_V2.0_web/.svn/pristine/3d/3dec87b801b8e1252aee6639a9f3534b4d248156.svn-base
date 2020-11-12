import axios from '@/libs/api.request'

//看板总表条件查询
export const module_index = (o) => {
    return axios.request({
        method: 'post',
        url: 'ModuleManagement_Api/Index',
        data: {
            orderunm: o,
        },
    })
}

//获取条码状态清单
export const Checklist = (obj) => {
    return axios.request({
        method: 'post',
        url: '/ModuleManagement_Api/Checklist',
        data: obj,
    })
}

//模块产线扫码确认
export const productionLine = (url, obj) => {
    return axios.request({
        method: 'post',
        url: `/ModuleManagement_Api/${url}`,
        data: obj,
    })
}

//模块产线、老化检查条码
export const checkBarcode = (obj) => {
    return axios.request({
        method: 'post',
        url: '/ModuleManagement_Api/AfterWeldingCheckBarcode',
        data: obj,
    })
}

//模块条码打印
export const ModuleBarcodePrinting = (obj) => {
    return axios.request({
        method: 'post',
        url: '/ModuleManagement_Api/ModuleBarcodePrinting',
        data: obj,
    })
}

//模块抽检检查条码
export const CheckSampling = (obj) => {
    return axios.request({
        method: 'post',
        url: '/ModuleManagement_Api/CheckSampling',
        data: obj,
    })
}

//模块抽检扫码确认
export const Sampling = (obj) => {
    return axios.request({
        method: 'post',
        url: '/ModuleManagement_Api/Sampling',
        data: obj,
    })
}

//老化异常录入
export const BurninAbnormal = (obj) => {
    return axios.request({
        method: 'post',
        url: '/ModuleManagement_Api/BurninAbnormal',
        data: obj,
    })
}

//老化条码扫码列表检查
export const BurninCompleteCheck = (obj) => {
    return axios.request({
        method: 'post',
        url: '/ModuleManagement_Api/BurninCompleteCheck',
        data: obj,
    })
}

//老化开始老化、结束老化确认
export const BurninConfirm = (url, obj) => {
    return axios.request({
        method: 'post',
        url: `/ModuleManagement_Api/${url}`,
        data: obj,
    })
}