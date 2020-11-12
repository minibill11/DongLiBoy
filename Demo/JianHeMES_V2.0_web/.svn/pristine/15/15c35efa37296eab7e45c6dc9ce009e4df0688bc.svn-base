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
      path: '/Equipment/EmployTime',
      name: 'EmployTime',
      meta: {
        hideInMenu: true,
        title: '部门设备详细时长表'
      },
      component: resolve => require(["@/view/equipment/employ-time.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_Parameter',
      name: 'Equipment_Parameter',
      meta: {
        title: '设备台账'
      },
      component: resolve => require(["@/view/equipment/parameter.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_Parameter_Details',
      name: 'Equipment_Parameter_Details',
      meta: {
        hideInMenu: true,
        title: '设备履历'
      },
      component: resolve => require(["@/view/equipment/parameter-details.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_Keycomponents',
      name: 'Equipment_Keycomponents',
      meta: {
        hideInMenu: true,
        title: '关键元器件清单'
      },
      component: resolve => require(["@/view/equipment/key-components.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_Production_line',
      name: 'Equipment_Production_line',
      meta: {
        title: '产线设备'
      },
      component: resolve => require(["@/view/equipment/production-line.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_Tally_Query',
      name: 'Equipment_Tally_Query',
      meta: {
        title: '点检查询'
      },
      component: resolve => require(["@/view/equipment/tally-query.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_Check_Record',
      name: 'Equipment_Check_Record',
      meta: {
        hideInMenu: true,
        title: '设备点检保养记录表'
      },
      component: resolve => require(["@/view/equipment/check-record.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_Maintenance_Summary',
      name: 'Equipment_Maintenance_Summary',
      meta: {
        hideInMenu: true,
        title: '设备保养汇总表'
      },
      component: resolve => require(["@/view/equipment/maintenance-summary.vue"], resolve)
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
      path: '/Equipment/Equipment_Fixbill_Detail',
      name: 'Equipment_Fixbill_Detail',
      meta: {
        hideInMenu: true,
        title: '报修单详细页'
      },
      component: resolve => require(["@/view/equipment/fixbill-detail.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_Safety_Bill',
      name: 'Equipment_Safety_Bill',
      meta: {
        title: '安全库存清单'
      },
      component: resolve => require(["@/view/equipment/safety-bill.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_Quality_Goal',
      name: 'Equipment_Quality_Goal',
      meta: {
        title: '指标达成率'
      },
      component: resolve => require(["@/view/equipment/quality-goal.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_ScanCode_Check',
      name: 'Equipment_ScanCode_Check',
      meta: {
        title: '扫码点检'
      },
      component: resolve => require(["@/view/equipment/scancode-check.vue"], resolve)
    },
    {
      path: '/Equipment/Equipment_Gas_Supply',
      name: 'Equipment_Gas_Supply',
      meta: {
        title: '供气系统'
      },
      component: resolve => require(["@/view/equipment/gas-supply.vue"], resolve)
    }
    ]
  },
]