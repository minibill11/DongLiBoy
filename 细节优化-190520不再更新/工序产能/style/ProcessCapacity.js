var mixin = {
    data: function () {
        return {
            loading: false,
            tableList: [],
            queryTable: {
                protype: "",
                proplatform: ""
            },
            options: {
                protype: [],
                proplatform: []
            }
        }
    },
    created: function () {
        //this.getTableData();
        this.getProtype();
        this.getPlatfrom();
    },
    methods: {
        //总表数据显示
        getTableData() {
            this.loading = true;
            this.tableList = [];
            axios.post('/Process_Capacity/TotalProcess_Capacity').then(res => {
                console.log(JSON.parse(JSON.stringify(res.data)));
                this.tableList = res.data;
                this.loading = false;
            }).catch(err => {
                console.warn("获取数据失败")
                this.loading = false;
            });
        },
        //总表条件查询
        onQuerySubmit() {
            this.loading = true;
            axios.post('/Process_Capacity/TotalProcess_Capacity', {
                protype: this.queryTable.protype,
                proplatform: this.queryTable.proplatform
            }).then(res => {
                console.log(JSON.parse(JSON.stringify(res.data)));
                this.tableList = res.data;
                this.loading = false;
                this.$message({
                    message: '查询成功！',
                    type: 'success'
                });
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
        getPlatfrom() {
            axios.post('/Process_Capacity/PlatfromList').then(res => {
                //console.log(res.data);
                this.options.proplatform = res.data;
            }).catch(err => {
                console.warn("型号列表获取失败");
            });
        },
    },
    //watch: {
    //    "queryTable.proplatform"(v) {
    //        this.onQuerySubmit(v);
    //    }
    //}
}