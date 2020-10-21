import Main from '@/components/main'
import MainContent from '@/components/main-content'

export default [
   // 系统管理
   {
    path: '/system_manage',
    name: 'system_manage',
    meta: {
      icon: 'logo-buffer',
      title: '系统管理'
    },
    component: Main,
    children: [{
        path: '/user_manage',
        name: 'user_manage',
        meta: {

          title: '用户管理'
        }
      },
      {
        path: '/permissions',
        name: 'permissions',
        meta: {

          title: '权限管理'
        },
        component: resolve => require(["@/view/system/permissions.vue"], resolve)
      },
      {
        path: '/permissions_by_person',
        name: 'permissions_by_person',
        meta: {

          title: '按人员查看权限'
        }
      },
      {
        path: '/person_by_permissions',
        name: 'person_by_permissions',
        meta: {

          title: '按权限查看人员'
        }
      },
      {
        path: '/prompt_item_manage',
        name: 'prompt_item_manage',
        meta: {

          title: '提示项管理'
        }
      },
      {
        path: '/log_manage',
        name: 'log_manage',
        meta: {

          title: '日志管理'
        }
      },
      {
        path: '/version_manage',
        name: 'version_manage',
        meta: {

          title: 'MES版本管理'
        }
      },
      {
        path: '/instructions_manage',
        name: 'instructions_manage',
        meta: {

          title: '说明书管理'
        }
      },
    ]
  },
]