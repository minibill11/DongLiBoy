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
            printInfo: {},
            limitsList: {},//用来存储用户所有的权限列表
            limitsRole: {},//用来存储用户与工序产能相关的权限列表
        }
    },
    created: function () {
        this.getOrdernumList();
        this.getGroup();
        //获取登陆时存储在浏览器的权限列表
        this.limitsList = JSON.parse(localStorage.getItem("rigths"));
        /** 获取与工序产能相关的权限对象列表，对应的结果用函数checkRoles判断 */
        this.limitsRole = {
            'Index': true,
            'AfterWeldingCreate': true,
            'AfterWeldingSampling': true,
            'Rule': true,
            'Inside': true,
            'Outside': true,
            '包装箱信息录入修改': this.checkRoles("包装箱信息录入修改"),
            //xxx: this.checkRoles("xxx"),
        };
    },
    methods: {
        //获取订单号选择框下拉清单
        getOrdernumList() {
            axios.post('/ModuleManagement/GetOrderNumList').then(res => {
                this.ordernumOptions = res.data;
                //本地订单值
                let localOrder = localStorage.getItem('Order');
                if (localOrder != null) {
                    this.Ordernum = localOrder;
                    try {
                        this.getBarcodeStateList(localOrder);
                    } catch (err) {
                        //console.log(err);
                    };
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
    },
    watch: {
        //监听班组
        Group(v) {
            v != '' && localStorage.setItem('selectgroup', v);
        }
    },
};