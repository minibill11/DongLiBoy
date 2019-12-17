let mixin = {
    data: function () {
        return {
            loading: false,
            tableList: [],
            queryTable: {
                protype: "",
                proplatform: "",
                statu: ""
            },
            options: {
                protype: [],
                proplatform: [],
                statu: [{ value: '未审核' }, { value: '审核未通过' }, { value: '审核通过' }, { value: '未批准' }, { value: '批准未通过' }, { value: '批准通过' },]
            },
            //权限列表 和 是否拥有权限
            limitsList: {},
            limitsRole: {},
            //定时器
            pollingTimer: "",
            editNumTimer: 0
        }
    },
    created: function () {
        this.limitsList = JSON.parse(localStorage.getItem("rigths"));
        this.onQuerySubmit();
        this.getProtype();
        this.getPlatfrom("");
        this.monitorTimer();

        //获取权限
        //1、新建平台 ，2、上传文件和编辑 ， 3、查看附表 ， 4、上传PDF和图片 ， 5、审核附表 ，6、批准附表 ，7、受控附表
        this.limitsRole = {
            1: this.checkRoles("新建平台"),
            2: this.checkRoles("上传文件和编辑"),
            3: this.checkRoles("查看附表"),
            4: this.checkRoles("上传PDF和图片"),
            5: this.checkRoles("审核附表"),
            6: this.checkRoles("批准附表"),
            7: this.checkRoles("受控附表"),
        };
    },
    methods: {
        //总表数据轮询
        getTableData() {
            axios.post('/Process_Capacity/TotalProcess_Capacity', {
                protype: this.queryTable.protype.toUpperCase(),
                proplatform: this.queryTable.proplatform
            }).then(res => {
                //console.log(JSON.parse(JSON.stringify(res.data)));
                this.editNumTimer == 0 && (this.tableList = res.data);
            }).catch(err => {
                //console.warn("获取数据失败")
            });
        },
        //总表条件查询
        onQuerySubmit() {
            this.loading = true;
            axios.post('/Process_Capacity/TotalProcess_Capacity', {
                protype: this.queryTable.protype.toUpperCase(),
                proplatform: this.queryTable.proplatform == "" ? [] : this.queryTable.proplatform
            }).then(res => {
                console.log(JSON.parse(JSON.stringify(res.data)));
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
            axios.post('/Process_Capacity/TypeList').then(res => {
                //console.log(res.data);
                this.options.protype = res.data;
            }).catch(err => {
                console.warn("型号列表获取失败");
            });
        },
        //平台下拉列表
        getPlatfrom(v) {
            axios.post('/Process_Capacity/DisplayPlatfromFromType', { type: v }).then(res => {
                //console.log(res.data);
                this.options.proplatform = res.data;
            }).catch(err => {
                console.warn("型号列表获取失败");
            });
        },
        //获取文件列表
        getFileStatu(v) {
            axios.post('/Process_Capacity/DisplayStatuMessage', { statu: v }).then(res => {
                console.log(res.data);
            }).catch(err => {
                console.warn("获取列表失败");
            });
        },
        //权限筛选
        checkRoles(roleName) {   //检测权限
            list = this.limitsList;
            if (list && roleName) {
                for (let item in list) {
                    if (list[item] == roleName) {
                        return true;
                    };
                };
            };
            return false;
        },
        //计时器
        monitorTimer() {
            this.pollingTimer = setInterval(() => {
                this.editNumTimer == 0 && this.getTableData();
            }, 3000);
        },
        //行style
        rowStyle({ row, rowIndex }) {
            let rtObj;
            row.editNum > 0 ? rtObj = { height: '100%', backgroundColor: '#f0ffec' } : rtObj = { height: '100%' };
            return rtObj
        }
    },
    watch: {
        "queryTable.protype"(v) {
            this.getPlatfrom(v.toUpperCase());
        },
        "queryTable.statu"(v) {
            this.getFileStatu(v)
        },
    }
};
var app = new Vue({
    el: "#app",
    mixins: [mixin]
});