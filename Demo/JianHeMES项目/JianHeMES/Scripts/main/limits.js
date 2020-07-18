﻿const limit = {
    data: function () {
        return {
            limitsList: [],//用来存储用户所有的权限列表
            menuList: [],//菜单
            isAdmin: false,//是否管理员
        }
    },
    //页面打开时执行
    created: function () {
        if (this.userName) {
            this.isAdmin = this.userRole == '系统管理员';
            if (localStorage.getItem('limits') == "[]" || localStorage.getItem('limits') == null) {
                axios.post("/Common/PermissionsArr", { id: this.userNum }).then(res => {
                    localStorage.setItem('limits', JSON.stringify([...new Set(res.data)]));
                    //获取存储在浏览器的权限列表
                    this.limitsList = JSON.parse(localStorage.getItem("limits"));
                    this.menuList = this.handleMune(setMuneList);
                }).catch(err => {
                    console.warn(err);
                });
            } else {
                //获取存储在浏览器的权限列表
                this.limitsList = JSON.parse(localStorage.getItem("limits"));
                this.menuList = this.handleMune(setMuneList);
            };
        } else {
            localStorage.removeItem('limits');
            localStorage.removeItem('menuIndexKey');
            localStorage.removeItem('menuIndexKeyPath');
        };
    },
    //函数方法
    methods: {
        //检测权限
        islimit(limitName) {
            return this.limitsList.includes(limitName);
        },
        //处理菜单
        handleMune(mune) {
            //开始递归
            for (let item of mune) {
                //处理 index指向地址
                item.url = `${item.title}→${item.url}`;
                //判断是否系统管理员
                if (this.isAdmin) {
                    item.show = true;
                } else {
                    //------------------start 非管理员权限筛选段start------------------
                    //默认为false
                    item.show = false;
                    let isok = false, department = this.userDepartment, name = this.userName;
                    //开始
                    if ('departmentLimits' in item) {
                        if (item.departmentLimits.KeepOrExclude) {
                            //保留
                            item.departmentLimits.JudgeArray.includes(department) ? isok = true : isok = false;
                        } else {
                            //排除
                            item.departmentLimits.JudgeArray.includes(department) ? isok = false : isok = true;
                        };
                    } else {
                        isok = true;
                    };

                    if ('limits' in item && item.limits.length != 0) {
                        for (let i of item.limits) {
                            if (this.islimit(i)) {
                                isok = true
                                break;
                            };
                        };
                    };

                    //特殊人员处理，不推荐此方式
                    if ('specialLimits' in item) {
                        if (item.specialLimits.KeepOrExclude) {
                            item.specialLimits.JudgeArray.includes(name) ? isok = true : isok = false;
                        } else {
                            if (item.specialLimits.JudgeArray.includes(name)) isok = false;
                        };
                    };

                    //结束
                    if (isok) {
                        item.show = true;
                    };
                    //  ------------------end 非管理员权限筛选段 end------------------
                };
                //检测子元素
                if ('children' in item) {
                    this.handleMune(item.children);
                };
            };
            //返回处理好的菜单
            return mune;
        },
    },
};