let mixin = {
    data: function () {
        return {
            loading: false,//loading状态开关
            Department: "",
            Group: "",
            groupOptions: [],
            printInfo: {},
            limitsList: {},//用来存储用户所有的权限列表
            limitsRole: {},//用来存储用户与工序产能相关的权限列表
        }
    },
    created: function () {
        this.getGroup();
        //获取登陆时存储在浏览器的权限列表
        this.limitsList = JSON.parse(localStorage.getItem("rigths"));
        /** 获取与工序产能相关的权限对象列表，对应的结果用函数checkRoles判断 */
        this.limitsRole = {
            'Index': true,
            'MaterialBasicInfo': true,
            'QueryData': true,
            'MaterialInput': true,
            'Material_Outbound': true,
            //xxx: this.checkRoles("xxx"),
        };
    },
    methods: {
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