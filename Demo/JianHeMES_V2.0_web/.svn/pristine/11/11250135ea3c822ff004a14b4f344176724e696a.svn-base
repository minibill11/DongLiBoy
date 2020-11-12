// 用户信息api
import axios from '@/libs/api.request'
//登录
export const login = (UserNum,PassWord) => {
  const data = {
      UserNumber:UserNum,
      Password:PassWord
    }
  return axios.request({
    url: 'Token/Login',
    data: data,
    method: 'post',
  })
}
//获取用户信息以及权限
export const getUserInfo = () => {
  return axios.request({
    url: 'Users_Api/UserInfo',
    method: 'post'
  })
}
//检查token是否过期
export const CheckToken = () => {
  return axios.request({
    url: 'Users_Api/CheckToken',
    method: 'post'
  })
}

// export const logout = (token) => {
//   return axios.request({
//     url: 'logout',
//     method: 'post'
//   })
// }

// export const getUnreadCount = () => {
//   return axios.request({
//     url: 'message/count',
//     method: 'get'
//   })
// }

// export const getMessage = () => {
//   return axios.request({
//     url: 'message/init',
//     method: 'get'
//   })
// }

// export const getContentByMsgId = msg_id => {
//   return axios.request({
//     url: 'message/content',
//     method: 'get',
//     params: {
//       msg_id
//     }
//   })
// }

// export const hasRead = msg_id => {
//   return axios.request({
//     url: 'message/has_read',
//     method: 'post',
//     data: {
//       msg_id
//     }
//   })
// }

// export const removeReaded = msg_id => {
//   return axios.request({
//     url: 'message/remove_readed',
//     method: 'post',
//     data: {
//       msg_id
//     }
//   })
// }

// export const restoreTrash = msg_id => {
//   return axios.request({
//     url: 'message/restore',
//     method: 'post',
//     data: {
//       msg_id
//     }
//   })
// }
