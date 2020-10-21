import Main from '@/components/main'
export default [
   //设备
{
    path: '/Equipment',
    name: 'Equipment',
    meta: {
      icon: 'logo-buffer',
      title: '设备管理',
      access: ['设备模块']
    },
    component: Main,
    children: [{
      path: '/Equipment/Equipment_NewIndex',
      name: 'Equipment_NewIndex',
      meta: {
        title: '运行状态'
      },
      component: resolve => require(["@/view/equipment/equipment-index.vue"], resolve)
    },
    {
      path: '/Equipment_Parameter',
      name: 'Equipment_Parameter',
      meta: {
        title: '设备台账'
      },
      component: resolve => require(["@/view/equipment/parameter.vue"], resolve)
    },
    {
      path: '/Equipment_Production_line',
      name: 'Equipment_Production_line',
      meta: {
        title: '产线设备'
      },
      component: resolve => require(["@/view/equipment/production-line.vue"], resolve)
    },
    {
      path: '/Equipment_Tally_Query',
      name: 'Equipment_Tally_Query',
      meta: {
        title: '点检查询'
      },
      component: resolve => require(["@/view/equipment/tally-query.vue"], resolve)
    },
    {
      path: '/Equipment_Check_Record',
      name: 'Equipment_Check_Record',
      meta: {
        title: '点检记录'
      },
      component: resolve => require(["@/view/equipment/check-record.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_Fixbill_Query',
      name: 'Equipment_Fixbill_Query',
      meta: {
  
        title: '报修单查询'
      },
      component: resolve => require(["@/view/equipment/fixbill-query.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_Fixbill_Deatil',
      name: 'Equipment_Fixbill_Deatil',
      meta: {
        hideInMenu: true,
        title: '报修单详细页'
      },
      component: resolve => require(["@/view/equipment/fixbill-detail.vue"], resolve)
    },
    {
      path: '/Equipment_Safety_Bill',
      name: 'Equipment_Safety_Bill',
      meta: {
  
        title: '安全库存清单'
      }
    },
    {
      path: '/Equipment_Quality_Goal',
      name: 'Equipment_Quality_Goal',
      meta: {
  
        title: '指标达成率'
      }
    },
    {
      path: '/Equipment_ScanCode_Check',
      name: 'Equipment_ScanCode_Check',
      meta: {
  
        title: '扫码点检'
      }
    },
    {
      path: '/Equipment_Gas_Supply',
      name: 'Equipment_Gas_Supply',
      meta: {
  
        title: '供气系统'
      }
    }
    ]
  },
]