import Main from '@/components/main'
import MainContent from '@/components/main-content'

export default [
   // 环境
   {
    path: '/environment',
    name: 'environment',
    meta: {
      icon: 'logo-buffer',
      title: '环境'
    },
    component: Main,
    children: [{
        path: '/temperature_and_humidity',
        name: 'temperature_and_humidity',
        meta: {

          title: '全厂温湿度'
        }
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