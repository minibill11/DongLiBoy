import axios from '@/libs/api.request'
//完成FQC
export const PQCCheckF = (obj) => {
    return axios.request({
        method: 'post',
        url: 'Assembles_Api/PQCCheckF',
        data: obj
    })
}
//获取状态列表
export const Assemblechecklist = (orderNum, station) => {
    return axios.request({
        method: 'post',
        url: 'Assembles_Api/Assemblechecklist',
        data: {
            orderNum: orderNum,
            station: station
        },
    })
}
//检查条码
export const CheckBarCodeNumIsRepertory = (ordernum, barcodenum) => {
    return axios.request({
        method: 'post',
        url: 'Commonality/CheckBarCodeNumIsRepertory',
        data: {
            ordernum: orderNum,
            barcodenum: barcodenum
        },
    })
}
//开始FQC
export const PQCCheckB = (obj) => {
    return axios.request({
        method: 'post',
        url: 'Assembles_Api/PQCCheckB',
        data: obj,
    })
}