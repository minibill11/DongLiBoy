
var app = new Vue({
    el: "#app",
    data: {
        loading: false,
        selectOptions: [],
        selectVal: '',
        screenSize: document.body.clientWidth,
        tableData: {}
    },
    methods: {
        //获取订单记录
        OrderNumReport(v) {
            axios.post('/Query/OrderNumReport', { ordernum: v }).then(res => {
                console.log(JSON.parse(JSON.stringify(res.data)));
                this.tableData = res.data;
            }).catch(err => {
                console.warn(err);
            });
        }
    },
    created: function () {
        //获取条码列表
        axios.post('/Packagings/GetOrderList').then(rer => {
            this.selectOptions = rer.data;
        }).catch(err => {
            console.warn("获取选择列表失败")
        });
        window.onresize = function () {
            app.screenSize = document.body.clientWidth;
        };
        this.selectVal = '2017-TEST-1'
    },
    mounted: function () {

    },
    watch: {
        selectVal: function (v) {
            v !== '' && this.OrderNumReport(v);
        },
    },
});