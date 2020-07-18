﻿const warehouse = {
    data: function () {
        return {
            loading: false,//loading状态开关
            Department: "",
            Group: "",
            groupOptions: [],
            printInfo: {},
        }
    },
    created: function () {
        this.getGroup();
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
    },
    watch: {
        //监听班组
        Group(v) {
            v != '' && localStorage.setItem('selectgroup', v);
        }
    },
};