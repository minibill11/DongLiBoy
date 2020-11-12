import Main from '@/components/main'
import MainContent from '@/components/main-content'

export default [
  // 订单交期
  {
    path: '/order_delivery_date',
    name: 'order_delivery_date',
    component: Main,
    meta: {
      icon: 'logo-buffer',
      title: '订单交期',
      access: ['订单交期模块']
    },
    children: [
      {
        path: '/order_query',
        name: 'order_query',
        meta: {

          title: '查询'
        },
        component: MainContent,
        children: [{
          path: '/production_line_query',
          name: 'production_line_query',
          meta: {

            title: '产线查询'
          },
        },
        {
          path: '/production_controller',
          name: 'production_controller',
          meta: {

            title: '生产管控'
          },
        },
        {
          path: '/output_value_board',
          name: 'output_value_board',
          meta: {

            title: '产值看板'
          },
        },
        {
          path: '/smt_board_query',
          name: 'smt_board_query',
          meta: {

            title: 'SMT看板查询'
          },
        },
        {
          path: '/smt_controller_board',
          name: 'smt_controller_board',
          meta: {

            title: 'SMT产线管理看板'
          },
        },
        {
          path: '/ic_production_board',
          name: 'ic_production_board',
          meta: {

            title: 'IC面生产看板'
          },
        },
        {
          path: '/light_plane_query',
          name: 'light_plane_query',
          meta: {

            title: '灯面生产看板'
          },
        },
        {
          path: '/mc_solder_paste_board',
          name: 'mc_solder_paste_board',
          meta: {

            title: 'MC锡膏看板'
          },
        },
        {
          path: '/smt_solder_paste_board',
          name: 'smt_solder_paste_board',
          meta: {

            title: 'SMT锡膏看板'
          },
        },
        {
          path: '/rty',
          name: 'rty',
          meta: {

            title: '直通率'
          },
        },
        {
          path: '/spare_parts_materials',
          name: 'spare_parts_materials',
          meta: {

            title: '备品配件物料管理'
          },
        },
        {
          path: '/sop_query',
          name: 'sop_query',
          meta: {

            title: 'SOP'
          },
        }
        ]
      },
      {
        path: '/order',
        name: 'order',
        meta: {

          title: '订单管理'
        },
        component: MainContent,
        children: [{
          path: '/order_manage',
          name: 'order_manage',
          meta: {

            title: '订单管理'
          }
        },
        {
          path: '/bar_code',
          name: 'bar_code',
          meta: {

            title: '条码管理'
          }
        },
        {
          path: '/sample_report',
          name: 'sample_report',
          meta: {

            title: '小样报告'
          }
        },
        {
          path: '/customer_complaint',
          name: 'customer_complaint',
          meta: {

            title: '客诉'
          },
        },
        {
          path: '/customer_complaint_loss_list',
          name: 'customer_complaint_loss_list',
          meta: {

            title: '客诉损失明细表'
          },
        }
        ]
      },
      {
        path: '/module_group',
        name: 'module_group',
        meta: {

          title: '模组'
        },
        component: MainContent,
        children: [
          {
            path: '/pqc',
            name: 'pqc',
            meta: {
              title: 'PQC'
            },
            component: MainContent,
            children: [{
              path: '/pqc/pqc-index',
              name: 'pqc-index',
              meta: {
                title: '首页'
              },
              component: resolve => require(["@/view/order-delivery-date/pqc/index.vue"], resolve)
            }, {
              path: '/pqc/pqc-operate',
              name: 'pqc-operate',
              meta: {
                title: '操作页'
              },
              component: resolve => require(["@/view/order-delivery-date/pqc/operate.vue"], resolve)
            },]
          }, {
            path: '/fqc',
            name: 'fqc',
            meta: {
              title: 'FQC'
            },
            component: MainContent,
            children: [{
              path: '/fqc/fqc-index',
              name: 'fqc-index',
              meta: {
                title: '首页'
              },
              component: resolve => require(["@/view/order-delivery-date/fqc/index.vue"], resolve)
            }, {
              path: '/fqc/fqc-operate',
              name: 'fqc-operate',
              meta: {
                title: '操作页'
              },
              component: resolve => require(["@/view/order-delivery-date/fqc/operate.vue"], resolve)
            },]
          }, {
            path: '/calibrationRecord',
            name: 'calibrationRecord',
            meta: {
              title: '校正'
            },
            component: MainContent,
            children: [{
              path: '/calibrationRecord/calibrationRecord-index',
              name: 'calibrationRecord-index',
              meta: {
                title: '首页'
              },
              component: resolve => require(["@/view/order-delivery-date/calibrationRecord/index.vue"], resolve)
            }, {
              path: '/calibrationRecord/calibrationRecord-operate',
              name: 'calibrationRecord-operate',
              meta: {
                title: '操作页'
              },
              component: resolve => require(["@/view/order-delivery-date/calibrationRecord/operate.vue"], resolve)
            },]
          }, {
            path: '/burn-in',
            name: 'burn-in',
            meta: {
              title: '老化'
            },
            component: MainContent,
            children: [{
              path: '/burn-in/burn-in-index',
              name: 'burn-in-index',
              meta: {
                title: '首页'
              },
              component: resolve => require(["@/view/order-delivery-date/burn-in/index.vue"], resolve)
            }, {
              path: '/burn-in/burn-in-operate',
              name: 'burn-in-operate',
              meta: {
                title: '操作页'
              },
              component: resolve => require(["@/view/order-delivery-date/burn-in/operate.vue"], resolve)
            },]
          }, {
            path: '/appearance',
            name: 'appearance',
            meta: {
              title: '外观电检'
            },
            component: MainContent,
            children: [{
              path: '/appearance/appearance-index',
              name: 'appearance-index',
              meta: {
                title: '首页'
              },
              component: resolve => require(["@/view/order-delivery-date/appearance/index.vue"], resolve)
            }, {
              path: '/appearance/appearance-operate',
              name: 'appearance-operate',
              meta: {
                title: '操作页'
              },
              component: resolve => require(["@/view/order-delivery-date/appearance/operate.vue"], resolve)
            },]
          }, {
            path: '/production_line_scan_code',
            name: 'production_line_scan_code',
            meta: {

              title: '产线扫码'
            },
          },
          {
            path: '/rules_entry',
            name: 'rules_entry',
            meta: {

              title: '规则录入'
            },
          },
          {
            path: '/carton_packing',
            name: 'carton_packing',
            meta: {

              title: '外箱装箱'
            },
          },
          {
            path: '/inside_box_confirm',
            name: 'inside_box_confirm',
            meta: {

              title: '内箱确认'
            },
          },
          {
            path: '/stadium_displays',
            name: 'stadium_displays',
            meta: {

              title: '球场屏'
            },
          }
        ]
      },
      {
        path: '/module_manage',
        name: 'module_manage',
        meta: {
          title: '模块'
        },
        component: MainContent,
        children: [{
          path: '/module_manage/board',
          name: 'module-manage-board',
          meta: {
            title: '模块实时看板'
          },
          component: resolve => require(["@/view/order-delivery-date/ModuleManagement/board.vue"], resolve)
        }, {
          path: '/module_manage/board-history',
          name: 'module-manage-board-history',
          meta: {
            title: '模块历史看板'
          },
          component: resolve => require(["@/view/order-delivery-date/ModuleManagement/board-history.vue"], resolve)
        }, {
          path: '/module_manage/index',
          name: 'module-manage-index',
          meta: {
            title: '模块首页'
          },
          component: resolve => require(["@/view/order-delivery-date/ModuleManagement/index.vue"], resolve)
        }, {
          path: '/module_manage/normal-check',
          name: 'module-manage-normal-check',
          meta: {
            title: '模块产线'
          },
          component: resolve => require(["@/view/order-delivery-date/ModuleManagement/normal-check.vue"], resolve)
        }, {
          path: '/module_manage/spot-check',
          name: 'module-manage-spot-check',
          meta: {
            title: '模块抽检'
          },
          component: resolve => require(["@/view/order-delivery-date/ModuleManagement/spot-check.vue"], resolve)
        }, {
          path: '/module_manage/burn-in',
          name: 'module-manage-burn-in',
          meta: {
            title: '模块老化'
          },
          component: resolve => require(["@/view/order-delivery-date/ModuleManagement/burn-in.vue"], resolve)
        }, {
          path: '/module_manage/rule',
          name: 'module-manage-rule',
          meta: {
            title: '装箱规则'
          },
          component: resolve => require(["@/view/order-delivery-date/ModuleManagement/rule.vue"], resolve)
        }, {
          path: '/module_manage/in-side',
          name: 'module-manage-in-side',
          meta: {
            title: '内箱装箱'
          },
          component: resolve => require(["@/view/order-delivery-date/ModuleManagement/in-side.vue"], resolve)
        }, {
          path: '/module_manage/out-side',
          name: 'module-manage-out-side',
          meta: {
            title: '外箱装箱'
          },
          component: resolve => require(["@/view/order-delivery-date/ModuleManagement/out-side.vue"], resolve)
        }, {
          path: '/module_manage/again-print',
          name: 'module-manage-again-print',
          meta: {
            title: '查看标签'
          },
          component: resolve => require(["@/view/order-delivery-date/ModuleManagement/again-print.vue"], resolve)
        }, {
          path: '/module_manage/delete-tag',
          name: 'module-manage-delete-tag',
          meta: {
            title: '删除标签'
          },
          component: resolve => require(["@/view/order-delivery-date/ModuleManagement/delete-tag.vue"], resolve)
        }, {
          path: '/smt_plan_entry',
          name: 'smt_plan_entry',
          meta: {

            title: 'SMT计划录入'
          },
        },
        {
          path: '/smt_production_entry',
          name: 'smt_production_entry',
          meta: {

            title: 'SMT生产信息录入'
          },
        },
        {
          path: '/solder_paste_operation',
          name: 'solder_paste_operation',
          meta: {

            title: '锡膏操作'
          },
        },
        {
          path: '/module_page',
          name: 'module_page',
          meta: {

            title: '模块页'
          },
        }
        ]
      },

      {
        path: '/metal_plate',
        name: 'metal_plate',
        meta: {
          title: '钣金'
        },
        component: MainContent,
        children: [{
          path: '/metal-plate/mp_index',
          name: 'mp_index',
          meta: {
            title: '首页'
          },
          component: resolve => require(["@/view/order-delivery-date/metal-plate/mp-index.vue"], resolve)
        },
        {
          path: '/mp_basicinfo',
          name: 'mp_basicinfo',
          meta: {
            title: '录入基本信息'
          },
          component: resolve => require(["@/view/order-delivery-date/metal-plate/mp-basicinfo.vue"], resolve)
        },
        {
          path: '/mp_production',
          name: 'mp_production',
          meta: {
            title: '录入生产信息'
          },
          component: resolve => require(["@/view/order-delivery-date/metal-plate/mp-production.vue"], resolve)
        },
        {
          path: '/mp_modify',
          name: 'mp_modify',
          meta: {
            title: '修改录入信息'
          },
          component: resolve => require(["@/view/order-delivery-date/metal-plate/mp-modify.vue"], resolve)
        }
        ]                    
      },

      {
        path: '/spare_parts',
        name: 'spare_parts',
        meta: {
          title: '备品配件'
        }
      },
      {
        path: '/contract',
        name: 'contract',
        meta: {

          title: '合约'
        }
      },
      {
        path: '/sales',
        name: 'sales',
        meta: {

          title: '销售'
        }
      },
      {
        path: '/hand_sample',
        name: 'hand_sample',
        meta: {

          title: '小样'
        }
      },
      {
        path: '/warehouse',
        name: 'warehouse',
        meta: {

          title: '仓库'
        },
        component: MainContent,
        children: [
          {
            path: '/solder_paste_out_storage',
            name: 'solder_paste_out_storage',
            meta: {

              title: '锡膏出库'
            },
          },
          {
            path: '/solder_paste_put_storage',
            name: 'solder_paste_put_storage',
            meta: {

              title: '锡膏入库'
            },
          },
          {
            path: '/delete_put_storage',
            name: 'delete_put_storage',
            meta: {

              title: '删除入库信息'
            },
          },
          {
            path: '/edit_sorage_number',
            name: 'edit_sorage_number',
            meta: {

              title: '修改库位号'
            },
          },
          {
            path: '/warehouse_material_management',
            name: 'warehouse_material_management',
            meta: {

              title: '仓库物料管理'
            },
          },
          {
            path: '/product_out_storage',
            name: 'product_out_storage',
            meta: {

              title: '成品出库'
            },
          },
          {
            path: '/product_put_storage',
            name: 'product_put_storage',
            meta: {
              title: '成品入库'
            },
          },
          {
            path: '/stock_manage',
            name: 'stock_manage',
            meta: {
              title: '库存金额管理'
            },
            component: MainContent,
            children: [{
              path: '/stock_manage/stock_index',
              name: 'stock_index',
              meta: {
                title: '库存金额信息'
              },
              component: resolve => require(["@/view/order-delivery-date/warehouse/stock-manage/stock-index.vue"], resolve)
            },
            {
              path: '/stock_query',
              name: 'stock_query',
              meta: {
                title: '月度库存金额'
              },
              component: resolve => require(["@/view/order-delivery-date/warehouse/stock-manage/stock-query.vue"], resolve)
            },
            {
              path: '/stock_erpquery',
              name: 'stock_erpquery',
              meta: {
                title: '查询ERP月度库存金额'
              },
              component: resolve => require(["@/view/order-delivery-date/warehouse/stock-manage/stock-erpquery.vue"], resolve)
            }
            ]
          }
        ]
      }
    ]
  },
]