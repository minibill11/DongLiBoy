/**
 * 工序产能录入页js功能
 **/

//存储混入实例
let mixin = {
    data: function () {
        return {
            loading: false,//loading状态开关
            tableList: [],//表格数据
            /** 选择框的值
                queryTable.protype 筛选型号
                queryTable.proplatform 筛选平台
                queryTable.statu 文件状态
                */
            queryTable: {
                protype: "",
                proplatform: "",
                statu: ""
            },
            /** 选择框的下拉列表
                options.protype 筛选型号下拉列表
                options.proplatform 筛选平台下拉列表
                options.statu 文件状态下拉列表
                */
            options: {
                protype: [],
                proplatform: [],
                statu: [{ value: '未审核' }, { value: '审核未通过' }, { value: '审核通过' }, { value: '未批准' }, { value: '批准未通过' }, { value: '批准通过' },]
            },
            limitsList: {},//用来存储用户所有的权限列表
            limitsRole: {},//用来存储用户与工序产能相关的权限列表
            pollingTimer: '',//定时器，控制页面定时刷新
            editNumTimer: 0,//监控打开的编辑框数量
        }
    },
    mounted: function () {
        this.beginTryTimer();
    },
    methods: {
        //总表数据轮询
        getTableData() {
            if (this.queryTable.protype != '' || this.queryTable.proplatform != '') {
                axios.post('/Process_Capacity/TotalProcess_Capacity', {
                    protype: this.queryTable.protype.toUpperCase(),
                    proplatform: this.queryTable.proplatform,
                    isCorrect: !this.limitsRole[1],
                }).then(res => {
                    //console.log(JSON.parse(JSON.stringify(res.data)));
                    this.editNumTimer == 0 && (this.tableList = res.data);
                }).catch(err => {
                    //console.warn("获取数据失败")
                });
            };
        },
        //总表条件查询
        onQuerySubmit() {
            this.loading = true;
            axios.post('/Process_Capacity/TotalProcess_Capacity', {
                protype: this.queryTable.protype.toUpperCase(),
                proplatform: this.queryTable.proplatform == "" ? [] : this.queryTable.proplatform,
                isCorrect: !this.limitsRole[1],
            }).then(res => {
                //console.log(JSON.parse(JSON.stringify(res.data)));
                this.tableList = res.data;
                this.loading = false;
                this.$message.success('查询成功！');
            }).catch(err => {
                console.warn("查询数据失败");
                this.loading = false;
            });
        },
        //型号下拉列表
        getProtype() {
            axios.post('/Process_Capacity/TypeList', { IsCorrect: !this.limitsRole[1] }).then(res => {
                //console.log(res.data);
                this.options.protype = res.data;
            }).catch(err => {
                console.warn("型号列表获取失败");
            });
        },
        //平台下拉列表
        getPlatfrom(v) {
            axios.post('/Process_Capacity/DisplayPlatfromFromType', { type: v, IsCorrect: !this.limitsRole[1] }).then(res => {
                //console.log(res.data);
                this.options.proplatform = res.data;
            }).catch(err => {
                console.warn("型号列表获取失败");
            });
        },
        /**
             * @@param roleName 参数为权限名称 例：‘新建平台’
             * 传入参数，在该用户权限列表中遍历查找该权限名称，若存在该权限，返回true，否则返回false
             */
        checkRoles(roleName) {   //检测权限
            let list = this.limitsList;
            if (list && roleName) {
                for (let item in list) {
                    if (list[item] == roleName) {
                        return true;
                    };
                };
            };
            return false;
        },
        //轮询计时方法，每10秒自动更新页面数据
        monitorTimer() {
            this.pollingTimer = setInterval(() => {
                this.editNumTimer == 0 && this.getTableData();
            }, 20000);
        },
        //表格行的颜色样式控制方法
        rowStyle({ row, rowIndex }) {
            let rtObj;
            row.editNum > 0 ? rtObj = { height: '100%', backgroundColor: '#d9f9d1' } : rtObj = { height: '100%' };
            return rtObj
        },
        //删除一行型号记录
        DeleteTotal(id, index) {
            this.$confirm('是否确认删除此行记录！', "请确认", {
                confirmButtonText: '确定',
                cancelButtonText: '取消',
                type: 'error',
            }).then(() => {

            }).catch(() => {

            });
        },
        //返回传入工段的 字段名称
        getSectionInfo(val) {
            return {
                'SMT': () => {
                    //SMT特殊，属于row的信息
                    return {
                        name: '贴装',
                        idName: 'id',
                        sectionName: 'row',
                        section: 'SMT'
                    }
                },
                '插件': () => {
                    return {
                        name: 'PluginDeviceName',
                        idName: 'PluginDeviceID',
                        sectionName: 'PluginDevice',
                        section: '插件'
                    }
                },
                '后焊': () => {
                    return {
                        name: 'AfterWeldProcessName',
                        idName: 'AfterWeldID',
                        sectionName: 'AfterWeld',
                        section: '后焊'
                    }
                },
                '三防': () => {
                    return {
                        name: 'ThreeProfProcessName',
                        idName: 'ThreeProfID',
                        sectionName: 'ThreeProf',
                        section: '三防'
                    }
                },
                '打底壳': () => {
                    return {
                        name: 'BottomCasProcessName',
                        idName: 'BottomCasID',
                        sectionName: 'BottomCas',
                        section: '打底壳'
                    }
                },
                '灌胶': () => {
                    return {
                        name: 'GlueProcessName',
                        idName: 'GlueID',
                        sectionName: 'Glue',
                        section: '灌胶'
                    }
                },
                '装磁吸安装板': () => {
                    return {
                        name: 'MagneticProcessName',
                        idName: 'MagneticID',
                        sectionName: 'Magnetic',
                        section: '装磁吸安装板'
                    }
                },
                '锁面罩': () => {
                    return {
                        name: 'LockTheMaskProcessName',
                        idName: 'LockTheMaskID',
                        sectionName: 'LockTheMask',
                        section: '锁面罩'
                    }
                },
                '气密': () => {
                    return {
                        name: 'AirtightProcessName',
                        idName: 'AirtightID',
                        sectionName: 'Airtight',
                        section: '气密'
                    }
                },
                '喷墨': () => {
                    return {
                        name: 'InkjetProcessName',
                        idName: 'InkjetID',
                        sectionName: 'Inkjet',
                        section: '喷墨'
                    }
                },
                '线材加工': () => {
                    return {
                        name: 'WireProcessName',
                        idName: 'WireID',
                        sectionName: 'Wire',
                        section: '线材加工'
                    }
                },
                '箱体加工': () => {
                    return {
                        name: 'BoxProcessName',
                        idName: 'BoxID',
                        sectionName: 'Box',
                        section: '箱体加工'
                    }
                },
                '模块线': () => {
                    return {
                        name: 'ModuleLineProcessName',
                        idName: 'ModuleLineID',
                        sectionName: 'ModuleLine',
                        section: '模块线'
                    }
                },
                '模组装配': () => {
                    return {
                        name: 'ModuleProcessName',
                        idName: 'ModuleID',
                        sectionName: 'Module',
                        section: '模组装配'
                    }
                },
                '老化': () => {
                    return {
                        name: 'BurinOneProcessName',
                        idName: 'BurinID',
                        sectionName: 'Burin',
                        section: '老化'
                    }
                },
                '包装': () => {
                    return {
                        name: 'PackingProcessName',
                        idName: 'PackingID',
                        sectionName: 'Packing',
                        section: '包装'
                    }
                },
            }[val].call(this);
        },
        //查找前工段是否存在多工艺工段
        checkBeforeContact(row, name) {
            //['包装', '老化', '模组装配', '模块线', '喷墨','线材加工','箱体加工', '气密', '锁面罩', '装磁吸安装板', '灌胶', '打底壳', '三防', '后焊', '插件']
            let sectionArr = ['包装', '老化', '模组装配', '灌胶', '三防'],
                thisIndex = 999;
            for (let i in sectionArr) {
                if (sectionArr[i] == name) {
                    thisIndex = i;
                };
                if (Number(i) > Number(thisIndex)) {
                    let sn = this.getSectionInfo(sectionArr[i]);
                    if (row[sn.sectionName] != null && sn.section != '老化' && (row[sn.sectionName].length > 1 || sn.section == "三防")) {
                        let rtd = {
                            name: sectionArr[i],
                            options: []
                        };
                        for (let rowitem of row[sn.sectionName]) {
                            rtd.options.push({
                                nextID: rowitem[sn.idName],
                                nextName: rowitem[sn.name]
                            });
                        };
                        return rtd;
                    };
                };
            };
            return {
                name: "",
                options: []
            };
        },
        //一行确认功能
        ChangCorrect(row, index) {
            axios.post('/Process_Capacity/ChangCorrect', { id: row.id, Correct: !row.isCorrect }).then(res => {
                this.$message.success(`${row.isCorrect ? '取消' : '确认'}成功`);
                row.isCorrect = !row.isCorrect;
            }).catch(err => {
                console.warn("型号列表获取失败");
            });
        },
        //页面加载轮询检测
        beginTryTimer() {
            this.limitsList = JSON.parse(localStorage.getItem("rigths"));
            if (this.limitsList == null) {
                setTimeout(() => {
                    this.beginTryTimer();
                }, 1000);
            } else {
                this.getMounted();
            };
        },
        //页面加载
        getMounted() {
            /** 获取与工序产能相关的权限对象列表，包括如下
                 *  1、新建平台 ，2、上传文件和编辑 ， 3、查看附表 ， 4、上传PDF和图片 ， 5、审核附表 ，6、批准附表 ，7、受控附表
                 *  对应的结果用函数checkRoles判断   例：存在“新建平台”权限，则权限1为true，不存在则为false
                 */
            this.limitsRole = {
                1: this.checkRoles("新建平台"),
                2: this.checkRoles("上传文件和编辑"),
                3: this.checkRoles("查看附表"),
                4: this.checkRoles("上传PDF和图片"),
                5: this.checkRoles("审核附表"),
                6: this.checkRoles("批准附表"),
                7: this.checkRoles("受控附表"),
            };
            //获取下拉型号列表
            this.getProtype();
            //获取下拉平台列表
            this.getPlatfrom("");
            //this.queryTable.statu = '未审核';//赋值文件状态默认值为未审核
            //this.onQuerySubmit();//页面加载完后 调用方法获取表格数据
            this.monitorTimer();//启动轮询计时器
        },
    },
    watch: {
        //监听平台选择，调用筛选方法，即时更新筛选后的表格
        "queryTable.protype"(v) {
            this.getPlatfrom(v.toUpperCase());
        }
    }
};
//通过mixins将代码注入vue实例
var app = new Vue({
    el: "#app",
    mixins: [mixin]
});