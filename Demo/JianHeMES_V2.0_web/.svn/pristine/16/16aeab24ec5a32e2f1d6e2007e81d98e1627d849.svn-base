import axios from '@/libs/api.request'

//模组-PQC/FQC/校正/老化/电检 总表条件查询
export const query_NewIndex = (api,o) => {
    return axios.request({
        method: 'post',
        url: `${api}/NewIndex`,
        data: {
            ordernum: o,
        },
    })
}