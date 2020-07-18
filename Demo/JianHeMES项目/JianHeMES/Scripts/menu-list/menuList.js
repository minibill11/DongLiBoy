/* 
 *  **统一设置菜单：
 *  *必需的值
    url                 --必需，链接地址
    title               --必需，菜单名字
    limits              --必需，需要允许查看的权限名 -以数组的形式为值，该值为空数组则不限制
    departmentLimits    --可选，保留或排除部门，以对象的形式，对象的属性不能为空，
                        例：departmentLimits:{  //KeepOrExclude为true，则表示只保留技术部和中心部可看,
                                                //若KeepOrExclude为false，则表示排除掉技术部和中心部，其余部门可看
                                                KeepOrExclude:true,
                                                JudgeArray:['技术部','中心部'] }
    specialLimits       --可选，个别人员特殊权限，不推荐使用
 *  子菜单
    children            --可选，此为子级菜单，格式和一级菜单一致


    --系统管理员显示所有菜单--
 */
const setMuneList = [
    {
        url: '/Query/businessDepartment',
        title: '销售部',
        limits: ['销售中心查询'],
        departmentLimits: {
            KeepOrExclude: true,
            JudgeArray: ['销售部']
        },
    },
    {
        url: '/Query/contractDepartment',
        title: '合约部',
        limits: ['合约中心查询'],
        departmentLimits: {
            KeepOrExclude: true,
            JudgeArray: ['合约部', '财务部', '采购部', '客户服务部']
        },
    },
    {
        url: '/allKongYa',
        title: '全厂温湿度',
        limits: [],
        departmentLimits: {
            KeepOrExclude: false,
            JudgeArray: ['合约部', '销售部', '财务部', '采购部', '客户服务部', '研发部']
        },
        specialLimits: {
            KeepOrExclude: false,
            JudgeArray: ['姜春艳', '刘秋如']
        },
        children: [
            {
                url: '/kongYaFloor',
                title: '楼层',
                limits: [],
                children: [
                    {
                        url: '/KongYaHT/KongYa/FirstFloor',
                        title: '一楼',
                        limits: [],
                    },
                    {
                        url: '/KongYaHT/KongYa/SecondFloor',
                        title: '二楼',
                        limits: [],
                    },
                    {
                        url: '/KongYaHT/KongYa/ThirdFloor',
                        title: '三楼',
                        limits: [],
                    },
                    {
                        url: '/KongYaHT/KongYa/FourthFloor',
                        title: '四楼',
                        limits: [],
                    },
                    {
                        url: '/KongYaHT/KongYa/FifthFloor',
                        title: '五楼',
                        limits: [],
                    },
                    {
                        url: '/KongYaHT/KongYa/Sixthfloor',
                        title: '六楼',
                        limits: [],
                    },]
            },

            {
                url: '/KongYaHT/KongYa/FloorDataShow',
                title: '汇总表',
                limits: [],
            },
            {
                url: '/KongYaHT/KongYa/OutputTHDataToExcel',
                title: '导出数据',
                limits: [],
            },
        ]
    },
    {
        url: '/kongYaroom',
        title: '空压机房',
        limits: [],
        departmentLimits: {
            KeepOrExclude: false,
            JudgeArray: ['合约部', '销售部', '财务部', '采购部', '客户服务部', '研发部']
        },
        specialLimits: {
            KeepOrExclude: false,
            JudgeArray: ['姜春艳', '刘秋如']
        },
        children: [
            {
                url: '/KongYaHT/KongYa/KongYaIndex',
                title: '空压机房',
                limits: [],
            },
            {
                url: '/KongYaHT/KongYa/KYDataShow',
                title: '汇总表',
                limits: [],
            },
        ]
    },
    {
        url: '/ProductionControl/Index',
        title: '生产管控',
        limits: [],
        departmentLimits: {
            KeepOrExclude: false,
            JudgeArray: ['合约部', '销售部', '财务部', '采购部', '客户服务部', '研发部']
        },
        specialLimits: {
            KeepOrExclude: false,
            JudgeArray: ['姜春艳', '刘秋如']
        },
    },
    {
        url: '/PMC',
        title: 'PMC部',
        limits: [],
        departmentLimits: {
            KeepOrExclude: false,
            JudgeArray: ['合约部', '销售部', '财务部', '采购部', '客户服务部', '研发部']
        },
        specialLimits: {
            KeepOrExclude: false,
            JudgeArray: ['姜春艳', '刘秋如']
        },
        children: [
            {
                url: '/OrderMgms/Index',
                title: '订单管理',
                limits: [],
            },
            {
                url: '/Query/ordernumModuleInfo',
                title: '订单模组概况',
                limits: [],
            },
            {
                url: '/BarCodes/DivertIndex',
                title: '挪用查询',
                limits: [],
            },
            {
                url: '/BarCodes/Index',
                title: '条码管理',
                limits: [],
            },
            {
                url: '/BarCodes/SetJsonFile',
                title: '模组编号录入',
                limits: [],
            },
            {
                url: '/Query/barcodeInfo',
                title: '条码内容查询',
                limits: [],
            },
            {
                url: '/SMT/SMT_ProductionPlan',
                title: 'SMT生产计划',
                limits: [],
            },
            {
                url: '/KPI/GetPassThrough',
                title: '直通率看板',
                limits: [],
            },
            {
                url: '/Packagings/board',
                title: '产值看板',
                limits: [],
            },
            {
                url: '/Packagings/HistoryBoard',
                title: '产值看板历史记录',
                limits: [],
            },
            {
                url: '/Packagings/inStockConfirm',
                title: '入库',
                limits: ['入库信息录入'],
            },
            {
                url: '/Packagings/DeleteConfirmAll',
                title: '删除入库信息',
                limits: ['系统管理员'],
            },
            {
                url: '/Warehouse_Material/QueryData',
                title: '仓库物料看板',
                limits: [],
            },
            {
                url: '/Packagings/stockNumEdit',
                title: '修改库位号',
                limits: ['库位号修改'],
            },
            {
                url: '/Packagings/outStockConfirm',
                title: '出库',
                limits: ['出库信息录入'],
            },
            {
                url: '/SMT_Sulderpaster/mcBoard',
                title: '锡膏看板',
                limits: [],
            },
            {
                url: '/SMT_Sulderpaster/AddWarehouseBaseInfo',
                title: '锡膏入库',
                limits: ['MC锡膏仓库操作'],
            },
            {
                url: '/Plans/Section_Enter',
                title: '工段工序录入',
                limits: [],
            },
        ]
    },
    {
        url: '/zhuanpei',
        title: '总装一/二部',
        limits: [],
        departmentLimits: {
            KeepOrExclude: false,
            JudgeArray: ['合约部', '销售部', '财务部', '采购部', '客户服务部', '研发部']
        },
        specialLimits: {
            KeepOrExclude: false,
            JudgeArray: ['姜春艳', '刘秋如']
        },
        children: [
            {
                url: '/ModuleManagement',
                title: '模块',
                limits: [],
                children: [
                    {
                        url: '/ModuleManagement/Index',
                        title: '看板首页',
                        limits: [],
                    },
                    {
                        url: '/ModuleManagement/normalCheck',
                        title: '模块产线',
                        limits: [],
                    }, {
                        url: '/ModuleManagement/spotCheck',
                        title: '模块抽检',
                        limits: [],
                    },
                    {
                        url: '/ModuleManagement/Burnin',
                        title: '模块老化',
                        limits: [],
                    }, {
                        url: '/ModuleManagement/Rule',
                        title: '装箱规则',
                        limits: [],
                    },
                    {
                        url: '/ModuleManagement/Inside',
                        title: '内箱装箱',
                        limits: [],
                    }, {
                        url: '/ModuleManagement/Outside',
                        title: '外箱装箱',
                        limits: [],
                    }]
            },
            {
                url: '/Assembles/AssembleIndex',
                title: '组装产线',
                limits: [],
            },
            {
                url: '/Burn_in_MosaicScreen/mosaicScreen_ShelfQuery',
                title: '拼屏查询',
                limits: [],
            },
            {
                url: '/Burn_in_MosaicScreen/mosaicScreen_B',
                title: '开始拼屏',
                limits: ['老化拼屏操作'],
            },
            {
                url: '/CalibrationRecords/Index',
                title: '校正信息',
                limits: [],
            },
            {
                url: '/Appearances/DisplayTotalModule',
                title: '模组条码打印',
                limits: [],
            },
            {
                url: '/Packagings/inputPackaging',
                title: '包装基本信息',
                limits: ['包装箱信息录入修改'],
            },
            {
                url: '/BarCodes/SetJsonFile',
                title: '模组编号录入',
                limits: ['包装箱信息录入修改'],
            },
            {
                url: '/Packagings/insideConfirm',
                title: '内箱确认',
                limits: ['内箱装箱确认'],
            },
            {
                url: '/CourtScreenModuleInfoes/Index',
                title: '球场屏',
                limits: [],
            },
        ]
    },
    {
        url: '/pingzhi',
        title: '品质部',
        limits: [],
        departmentLimits: {
            KeepOrExclude: false,
            JudgeArray: ['合约部', '销售部', '财务部', '采购部', '研发部']
        },
        specialLimits: {
            KeepOrExclude: false,
            JudgeArray: ['姜春艳', '刘秋如']
        },
        children: [
            {
                url: '/FinalQCs/Index',
                title: 'FQC',
                limits: [],
                departmentLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['客户服务部']
                },
            }, {
                url: '/Burn_in/Index',
                title: '老化OQC',
                limits: [],
                departmentLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['客户服务部']
                },
            }, {
                url: '/Appearances/Index',
                title: '外观电检',
                limits: [],
                departmentLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['客户服务部']
                },
            }, {
                url: '/Packagings/outsideBinningPrint',
                title: '外箱装箱',
                limits: ['查看外箱标签'],
                departmentLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['客户服务部']
                },
            }, {
                url: '/IQCReports/Index',
                title: 'IQC管理',
                limits: [],
                departmentLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['客户服务部']
                },
            }, {
                url: '/Packagings/SPC_QueryByOrderNumber',
                title: '备品配件物料管理',
                limits: [],
                departmentLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['客户服务部']
                },
            }, {
                url: '/Customer_Complaints/index',
                title: '客诉订单',
                limits: [],
                /*'创建订单客诉表','修改订单客诉表'*/
            }, {
                url: '/Customer_Complaints/Customer_loss',
                title: '客诉损失明细表',
                limits: [],
            },
        ]
    },

    {
        url: '/smt',
        title: 'SMT部',
        limits: [],
        departmentLimits: {
            KeepOrExclude: false,
            JudgeArray: ['合约部', '销售部', '财务部', '采购部', '客户服务部', '研发部']
        },
        specialLimits: {
            KeepOrExclude: false,
            JudgeArray: ['姜春艳', '刘秋如']
        },
        children: [
            {
                url: '/SMT/SMT_Manage',
                title: '看板管理',
                limits: [],
            }, {
                url: '/SMT/SMT_ProductionInfo',
                title: '生产信息',
                limits: [],
            }, {
                url: '/SMT/SMT_Operator',
                title: '产线操作',
                limits: ['产线信息录入'],
            }, {
                url: '/SMT/SMT_ProductionData',
                title: '历史记录',
                limits: [],
            }, {
                url: '/SMT_Sulderpaster/AddSMTFreezer',
                title: '锡膏入冰柜',
                limits: ['SMT锡膏入SMT冰柜'],
            }, {
                url: '/SMT_Sulderpaster/Stir',
                title: '锡膏搅拌使用',
                limits: ['SMT锡膏入SMT冰柜'],
            }, {
                url: '/SMT_Sulderpaster/smtBoard',
                title: '锡膏看板',
                limits: [],
            }, {
                url: '/SMT/DisplayICContenInfo',
                title: 'IC面生产看板',
                limits: [],
            }, {
                url: '/SMT/DisplayLightContenInfo',
                title: '灯面生产看板',
                limits: [],
            },
        ]
    },

    {
        url: '/jishu',
        title: '技术部',
        limits: [],
        departmentLimits: {
            KeepOrExclude: false,
            JudgeArray: ['合约部', '销售部', '财务部', '采购部', '客户服务部', '研发部']
        },
        specialLimits: {
            KeepOrExclude: false,
            JudgeArray: ['姜春艳', '刘秋如']
        },
        children: [{
            url: '/Equipment/index',
            title: '设备管理',
            limits: [],
        }, {
            url: '/Small_Sample/index',
            title: '小样报告',
            limits: [],
        }, {
            url: '/Process_Capacity/index',
            title: '工序产能首页',
            limits: [],
        }, {
            url: '/Process_Capacity/index4',
            title: '工序产能信息录入',
            limits: [],
        }, {
            url: '/SOP/index',
            title: 'SOP',
            limits: [],
        }, {
            url: '/Small_Sample/index',
            title: '小样报告',
            limits: [],
        },
        ]
    },
    {
        url: '/yanfa',
        title: '研发部',
        limits: [],
        departmentLimits: {
            KeepOrExclude: true,
            JudgeArray: ['研发部']
        },
        children: [{
            url: '/Small_Sample/index',
            title: '小样报告',
            limits: [],
        },
        ]
    },
    {
        url: '/banjin',
        title: '配套加工部',
        limits: [],
        departmentLimits: {
            KeepOrExclude: false,
            JudgeArray: ['合约部', '销售部', '财务部', '采购部', '客户服务部', '研发部']
        },
        specialLimits: {
            KeepOrExclude: false,
            JudgeArray: ['姜春艳', '刘秋如']
        },
        children: [
            {
                url: '/MetalPlate/Index',
                title: '生产进度管控',
                limits: [],
            }
        ]
    },

    {
        url: '/people',
        title: '人力资源部',
        limits: [],
        departmentLimits: {
            KeepOrExclude: false,
            JudgeArray: ['合约部', '销售部', '财务部', '采购部', '客户服务部', '研发部']
        },
        children: [
            {
                url: '/Personnel/Daily',
                title: '人事日报表',
                limits: ['显示人事日报表'],
                specialLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['姜春艳', '刘秋如']
                },
            }, {
                url: '/Personnel_of_Contrast/Contrast',
                title: '人工成本周对比',
                limits: ['显示人工成本对比表'],
                specialLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['姜春艳', '刘秋如']
                },
            }, {
                url: '/Personnel_Turnoverrate/Turnoverrate',
                title: '月流失率',
                limits: ['显示月流失率'],
                specialLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['姜春艳', '刘秋如']
                },
            }, {
                url: '/Personnel_Recruitment/Recruitment',
                title: '招聘周报',
                limits: ['显示招聘周报'],
                specialLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['姜春艳', '刘秋如']
                },
            }, {
                url: '/Personnel_Roster/LeavedPersonServiceLength',
                title: '离职员工工龄分布',
                limits: ['显示离职员工工龄表'],
                specialLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['姜春艳', '刘秋如']
                },
            }, {
                url: '/Personnel_Roster/Index',
                title: '花名册',
                limits: ['显示花名册'],
                specialLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['姜春艳', '刘秋如']
                },
            }, {
                url: '/Personnel_Framework/Index',
                title: '组织架构',
                limits: ['显示组织架构'],
                specialLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['姜春艳', '刘秋如']
                },
            }, {
                url: '/Personnel_Quality_objectives/Index',
                title: '月度优秀班组评选',
                limits: ['显示月度优秀班组评选'],
                specialLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['姜春艳', '刘秋如']
                },
            }, {
                url: '/Personnel_Reasons_for_leaving/Index',
                title: '离职原因调查',
                limits: ['显示离职员工调查'],
                specialLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['姜春艳', '刘秋如']
                },
            }, {
                url: '/Personnel_Leave/Index',
                title: '请假记录',
                limits: ['显示请假记录'],
                specialLimits: {
                    KeepOrExclude: false,
                    JudgeArray: ['姜春艳', '刘秋如']
                },
            }, {
                url: '/Users/Usersquery',
                title: '系统用户',
                limits: ['查看工厂用户清单', '查看总部用户清单'],
            },
        ]
    },
];