import { OrderList, DisplayGroup } from "@/api/common";
export default {
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
            showPrint: false,  //打印剩余条码权限
        }
    },
    created: function () {
        this.getOrdernum();
        this.getGroup();
    },
    mounted: function () {
        //判断权限
        if (this.$store.state.user.userInfo.Admin) {
            this.showPrint = true;
        };
    },
    methods: {
        //获取订单号选择框下拉清单
        getOrdernum() {
            OrderList().then(res => {
                this.ordernumOptions = res.data.Data;
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
            DisplayGroup().then(res => {
                this.groupOptions = res.data.Data.Group;
                this.Department = res.data.Data.department;
                //查看班组列表里是否存在本地存储的值
                let localsg = localStorage.getItem('selectgroup');
                if (localsg != null) {
                    for (let i of this.groupOptions) {
                        if (i == localsg) {
                            this.Group = localsg;
                        };
                    };
                };
            }).catch(err => {
                console.warn(err);
            });
        },
    },
    watch: {
        //监听班组
        Group(v) {
            v != '' && localStorage.setItem('selectgroup', v);
        }
    },
};