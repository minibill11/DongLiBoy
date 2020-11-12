import Vue from 'vue'
import App from './App'
import router from '@/router'
import store from '@/store'
import i18n from '@/locale'
import config from '@/config'
import importDirective from '@/directive'
import { directive as clickOutside } from 'v-click-outside-x'
import installPlugin from '@/plugin'

//加载全局过滤器
// if (process.env.NODE_ENV !== 'production') require('@/mock')
import * as filters from './filters/index'
Object.keys(filters).forEach(key => {
  Vue.filter(key, filters[key])
})

//引入
import '@/assets/icons/iconfont.css'
import '@/assets/theme/index.css';
import '@/assets/style/common.less';
import '@/libs/global'

//-----引入npm安装库配置-------
//引入ViewUI
import ViewUI from "view-design";
Vue.use(ViewUI, {
  i18n: (key, value) => i18n.t(key, value)
})

//引入ElementUI
import ElementUI from 'element-ui';
Vue.use(ElementUI, {
  size: 'small',
  i18n: (key, value) => i18n.t(key, value)
});

//引入html2pdf
import html2pdf from "@/libs/html2pdf";
Vue.use(html2pdf);

//引入vxe-table
import 'xe-utils'
import 'vxe-table/lib/style.css'
import VXETable from 'vxe-table'
Vue.use(VXETable)

//引入JQuery
//var jquery = require('jquery');
import $ from "jquery"
//注册SignalR插件
import 'signalr'

/**
 * @description 注册插件
 */
installPlugin(Vue)
/**
 * @description 生产环境关掉提示
 */
Vue.config.productionTip = false
/**
 * @description 全局注册应用配置
 */
Vue.prototype.$config = config
/**
 * @description 注册指令
 */
importDirective(Vue)
Vue.directive('clickOutside', clickOutside)
/* eslint-disable */

new Vue({
  el: '#app',
  router,
  i18n,
  store,
  render: h => h(App)
})
