import Main from '@/components/main'
import MainContent from '@/components/main-content'

export default [
   // 人力资源
   {
    path: '/human_resources',
    name: 'human_resources',
    meta: {
      icon: 'logo-buffer',
      title: '人力资源'
    },
    component: Main,
    children: [{
        path: '/personnel_daily_report',
        name: 'personnel_daily_report',
        meta: {

          title: '人事日报表'
        }
      },
      {
        path: '/labor_cost_week',
        name: 'labor_cost_week',
        meta: {

          title: '人工成本周对比'
        }
      },
      {
        path: '/time_table',
        name: 'time_table',
        meta: {

          title: '工时统计表'
        }
      },
      {
        path: '/wastage_rate_month',
        name: 'wastage_rate_month',
        meta: {

          title: '月流失率'
        }
      },
      {
        path: '/recruitment_weekly',
        name: 'recruitment_weekly',
        meta: {

          title: '招聘周报'
        }
      },
      {
        path: '/departure_distribution',
        name: 'departure_distribution',
        meta: {

          title: '离职员工工龄分布'
        }
      },
      {
        path: '/membership_roster',
        name: 'membership_roster',
        meta: {

          title: '花名册'
        }
      },
      {
        path: '/kpi',
        name: 'kpi',
        meta: {

          title: 'KPI'
        },
        component: MainContent,
        children: [{
            path: '/KPI_Index_History',
            name: 'KPI_Index_History',
            meta: {

              title: '核准记录查看'
            },
          },
          {
            path: '/KPI_Ranking',
            name: 'KPI_Ranking',
            meta: {

              title: '排名计算'
            },
          },
          {
            path: '/IntegralTable_Index',
            name: 'IntegralTable_Index',
            meta: {

              title: '班组积分查询'
            },
          },
          {
            path: '/Section_Enter',
            name: 'Section_Enter',
            meta: {

              title: '工段工序参数录入'
            },
          },
          {
            path: '/KPI_Daily',
            name: 'KPI_Daily',
            meta: {

              title: '效率指标日报表'
            },
          },
          {
            path: '/KPI_DeparturesNum',
            name: 'KPI_DeparturesNum',
            meta: {

              title: '班组人员流失汇总表'
            },
          }
        ]
      },
      {
        path: '/7s_manage',
        name: '7s_manage',
        meta: {

          title: '7s管理'
        },
        component: MainContent,
        children: [{
            path: '/KPI_7S_SummarizingRanking',
            name: 'KPI_7S_SummarizingRanking',
            meta: {
              title: '部门评比排名'
            },
            component: resolve => require(["@/view/human-resource/7s-manage/KPI-7S-SummarizingRanking.vue"], resolve)
          },
          {
            path: '/KPI_7S_GradeStandardInput',
            name: 'KPI_7S_GradeStandardInput',
            meta: {
              title: '扣分标准录入'
            },
            component: resolve => require(["@/view/human-resource/7s-manage/KPI-7S-GradeStandardInput.vue"], resolve)
          },
          {
            path: '/KPI_7S_RegionInput',
            name: 'KPI_7S_RegionInput',
            meta: {
              title: '数据区域录入'
            },
            component: resolve => require(["@/view/human-resource/7s-manage/KPI-7S-RegionInput.vue"], resolve)
          },
          {
            path: '/KPI_7S_RecordInput',
            name: 'KPI_7S_RecordInput',
            meta: {
              title: '部门班组扣分录入'
            },
            component: resolve => require(["@/view/human-resource/7s-manage/KPI-7S-RecordInput.vue"], resolve)
          },
          {
            path: '/KPI_7S_Summarizing',
            name: 'KPI_7S_Summarizing',
            meta: {
              title: '7S评比得分汇总'
            },
            component: resolve => require(["@/view/human-resource/7s-manage/KPI-7S-Summarizing.vue"], resolve)
          },
          {
            path: '/KPI_7S_Summarizing_Daily',
            name: 'KPI_7S_Summarizing_Daily',
            meta: {

              title: '日/周/巡检汇总表'
            },
            component: resolve => require(["@/view/human-resource/7s-manage/KPI-7S-Summarizing-Daily.vue"], resolve)
          },
          {
            path: '/KPI_7S_DailyRecord',
            name: 'KPI_7S_DailyRecord',
            meta: {

              title: '日检记录查询'
            },
            component: resolve => require(["@/view/human-resource/7s-manage/KPI-7S-DailyRecord.vue"], resolve)
          },
          {
            path: '/KPI_7S_Detail',
            name: 'KPI_7S_Detail',
            meta: {
              title: '7S班组扣分详细表'
            },
            component: resolve => require(["@/view/human-resource/7s-manage/KPI_7S_Detail.vue"], resolve)
          }
        ]
      },
      {
        path: '/organizational_structure',
        name: 'organizational_structure',
        meta: {

          title: '组织架构'
        }
      },
      {
        path: '/honor_roll_month',
        name: 'honor_roll_month',
        meta: {

          title: '月度光荣榜'
        }
      },
      {
        path: '/honor_roll_year',
        name: 'honor_roll_year',
        meta: {

          title: '年度光荣榜'
        }
      },
      {
        path: '/group_score_statistics',
        name: 'group_score_statistics',
        meta: {

          title: '班组积分统计表'
        }
      },
      {
        path: '/departure_reason_survey',
        name: 'departure_reason_survey',
        meta: {

          title: '离职原因调查'
        }
      },
      {
        path: '/leave_record',
        name: 'leave_record',
        meta: {

          title: '请假记录'
        }
      },
      {
        path: '/system_user',
        name: 'system_user',
        meta: {

          title: '系统用户'
        }
      }
    ]
  }
]