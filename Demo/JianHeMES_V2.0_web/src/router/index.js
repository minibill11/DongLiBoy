import Vue from 'vue'
import Router from 'vue-router'
import routes from './routers'
import store from '@/store'
import viewUI from 'view-design'
import {
  canTurnTo,
  setTitle
} from '@/libs/util'
import config from '@/config'
import {
  CheckToken
} from '@/api/user'
const {
  homeName
} = config

Vue.use(Router)
//解决菜单重复问题
const originalPush = Router.prototype.push
Router.prototype.push = function push(location) {
  return originalPush.call(this, location).catch(err => err)
}
const router = new Router({
  mode: 'history',
  routes
})
const LOGIN_PAGE_NAME = 'login'

const turnTo = (to, access, next) => {
  if (canTurnTo(to.name, access, routes)) next() // 有权限，可访问
  else next({
    replace: true,
    name: LOGIN_PAGE_NAME
  }) // 无权限，重定向到登录页面
}
router.beforeEach((to, from, next) => {
  viewUI.LoadingBar.start()
  var token = ''
  // console.log(localStorage.getItem("store"), 999)
  if (localStorage.getItem("store")) {
    store.replaceState(Object.assign({}, store.state, JSON.parse(localStorage.getItem("store"))))
    token = store.state.user.token
    // console.log(token, 'token')
    CheckToken().then(res => {
      // console.log(res.data.PostResult.AuthorizationResult, 898989)
      if (res.data.PostResult.AuthorizationResult) {
        // console.log(token, to.name)
        //有token且true
        if (token && to.name === LOGIN_PAGE_NAME) {
          console.log('分支33')
          // 已登录且要跳转的页面是登录页
          next({
            name: homeName // 跳转到homeName页
          })
        } else {
          console.log('分支44')
          if (store.state.user.hasGetInfo) {
            turnTo(to, store.state.user.access, next)
          } else {
            console.log('分支55')
            store.dispatch('getUserInfo').then(user => {
              // 拉取用户信息，通过用户权限和跳转的页面的name来判断是否有权限访问;access必须是一个数组，如：['super_admin'] ['super_admin', 'admin']
              turnTo(to, store.state.user.access, next)
            }).catch(() => {
              // setToken('')
              next({
                name: 'login'
              })
            })
          }
        }
      } else {
        //有token但失效
        console.log('分支66')
        localStorage.setItem("store","")
        next({
          name: LOGIN_PAGE_NAME // 跳转到登录页
        })
        return
      }
    })
  } else {
    
    if (to.name !== LOGIN_PAGE_NAME) {
      console.log('分支11')
      //没有token且要跳转的页面不是登录页
      next({
        name: LOGIN_PAGE_NAME // 跳转到登录页
      })
    } else if (to.name === LOGIN_PAGE_NAME) {
      console.log('分支22')
      //没有token且要跳转的页面是登录页
      next() // 跳转
    }
  }

})

router.afterEach(to => {
  setTitle(to, router.app)
  viewUI.LoadingBar.finish()
  window.scrollTo(0, 0)
})

export default router
