import Main from '@/components/main'
import MainContent from '@/components/main-content'

export default [
  // 环境监测
  {
    path: '/environment',
    name: 'environment',
    meta: {
      icon: 'logo-buffer',
      title: '环境监测'
    },
    component: Main,
    children: [{
      path: '/environment1',
      name: 'environment1',
      meta: {
        title: '全厂温湿度'
      },
      component: MainContent,
      children: [{
        path: '/environment/th_firstfloor',
        name: 'th_firstfloor',
        meta: {
          title: '一楼'
        },
        component: resolve => require(["@/view/environment/th-firstfloor.vue"], resolve)
      },
      {
        path: '/environment/th_secondfloor',
        name: 'th_secondfloor',
        meta: {
          title: '二楼'
        },
        component: resolve => require(["@/view/environment/th-secondfloor.vue"], resolve)
      }, {
        path: '/environment/th_threefloor',
        name: 'th_threefloor',
        meta: {
          title: '三楼'
        },
        component: resolve => require(["@/view/environment/th-threefloor.vue"], resolve)
      }, {
        path: '/environment/th_fourfloor',
        name: 'th_fourfloor',
        meta: {
          title: '四楼'
        },
        component: resolve => require(["@/view/environment/th-fourfloor.vue"], resolve)
      }, {
        path: '/environment/th_fivefloor',
        name: 'th_fivefloor',
        meta: {
          title: '五楼'
        },
        component: resolve => require(["@/view/environment/th-fivefloor.vue"], resolve)
      }, {
        path: '/environment/th_sixfloor',
        name: 'th_sixfloor',
        meta: {
          title: '六楼'
        },
        component: resolve => require(["@/view/environment/th-sixfloor.vue"], resolve)
      },]
    },
    {
      path: '/temperature_and_humidity_summary_list',
      name: 'temperature_and_humidity_summary_list',
      meta: {

        title: '温湿度汇总表'
      }
    }
    ]
  }
]