var mixin = {
    data: function () {
        return {
            loading: false,
            tableList: [],
        }
    },
    created: function () {
        this.getTableData();
    },
    mounted: function () {

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
        }
    }
}