// 公共api
import axios from '@/libs/api.request'
import store from '@/store'

 //获取账号班组列表
 export const DisplayGroup = () => {
    const data = {
      deparment:store.state.user.userInfo.Department,
    }
    return axios.request({
      url: 'Common_Api/DisplayGroup ',
      data: data,
      method: 'post',
    })
  }