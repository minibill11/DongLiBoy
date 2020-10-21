import Main from '@/components/main'

export default [
   // 开发测试模块
   {
    path: '/text',
    name: 'text',
    meta: {
      // hideInMenu: true,
      icon: 'ios-bug',
      title: '测试'
    },
    component: Main,
    children: [{
        path: 'export_excel',
        name: 'export_excel',
        meta: {
          title: '导出excel'
        },
        component: resolve => require(['@/view/test/excel/export-excel-demo.vue'], resolve)
      },
      {
        path: 'upload_excel',
        name: 'upload_excel',
        meta: {
          title: '导入excel'
        },
        component: resolve => require(['@/view/test/excel/upload-excel.vue'], resolve)
      },
      {
        path: 'export_pdf',
        name: 'export_pdf',
        meta: {
          title: '导出pdf'
        },
        component: resolve => require(['@/view/test/export-pdf.vue'], resolve)
      }
    ]
  },
]