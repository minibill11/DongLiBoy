let mixin = {
    data: function () {
        return {
            loading: false,//loading状态开关
            disabledState: false,//禁用状态
            Department: "",
            Group: "",
            groupOptions: [],
            Ordernum: "",
            ordernumOptions: [],
            ModuleBarcode: "",
            printInfo: {}
        }
    },
    created: function () {
        this.getGroup();
    },
    methods: {
        //获取订单号选择框下拉清单
        getOrdernumList(getState = false) {
            axios.post('/ModuleManagement/GetOrderNumList').then(res => {
                this.ordernumOptions = res.data;
                //本地订单值
                let localOrder = localStorage.getItem('Order');
                if (localOrder != null) {
                    this.Ordernum = localOrder;
                    getState && this.getBarcodeStateList(localOrder);
                };
            }).catch(err => {
                console.warn(err);
            });
        },
        //获取订单号选择框下拉清单
        getOrdernumList() {
            axios.post('/ModuleManagement/GetOrderNumList').then(res => {
                this.ordernumOptions = res.data;
                //本地订单值
                let localOrder = localStorage.getItem('Order');
                if (localOrder != null) {
                    this.Ordernum = localOrder;
                    this.getBarcodeStateList(localOrder);
                };
            }).catch(err => {
                console.warn(err);
            });
        },
        //获取账号班组列表
        getGroup() {
            axios.post("/Users/DisplayGroup").then(res => {
                //未登陆则返回null
                if (res.data == '') {
                    this.$message.warning("请登陆！")
                } else {
                    this.groupOptions = res.data.Group;
                    this.Department = res.data.department;
                    //查看班组列表里是否存在本地存储的值
                    let localsg = localStorage.getItem('selectgroup');
                    if (localsg != null) {
                        for (let i of this.groupOptions) {
                            if (i == localsg) {
                                this.Group = localsg;
                            };
                        };
                    };
                };
            }).catch(err => {
                console.warn(err);
            });
        },
        //订单号搜索方法
        querySearch(queryString, cb) {
            let restaurants = this.ordernumOptions;
            let results = queryString ? restaurants.filter(this.createFilter(queryString)) : restaurants;
            // 调用 callback 返回建议列表的数据
            cb(results);
        },
        createFilter(queryString) {
            return (restaurant) => {
                return (restaurant.value.toLowerCase().indexOf(queryString.toLowerCase()) > -1);
            };
        },
    },
    watch: {
        //监听班组
        Group(v) {
            v != '' && localStorage.setItem('selectgroup', v);
        }
    },
};