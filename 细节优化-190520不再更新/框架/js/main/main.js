﻿const mixin = {
    data: function () {
        return {
            mainLoading: false,//loading状态开关
            screenSize: document.body.clientWidth,//屏幕尺寸
            urlPathName: [],//地址
            urlSearchParam: {},//地址参数
            activeIndex: '',//菜单选项
            openedsArr: [],//菜单选项数组
            leftMenuShow: false,//菜单的显示
        }
    },
    //页面打开时执行
    created: function () {
        //获取地址栏信息
        this.GetUrlParam();
        if (!this.userName) {
            let skipurl = this.urlPathName.split('/').filter(i => i);
            if (skipurl) { location.href = `/Users/Login2?col=${skipurl[0]}&act=${skipurl.slice(1).join('/')}`; } else { location.href = '/Users/Login2'; };
        };
        //处理菜单展开
        this.activeIndex = localStorage.getItem("menuIndexKey");
        this.openedsArr = JSON.parse(localStorage.getItem("menuIndexKeyPath"));
        if (this.activeIndex != '' && this.activeIndex != null && this.urlPathName != this.activeIndex.split('→')[1]) {
            this.activeIndex = '';
            this.openedsArr = [];
        };
    },
    //页面加载完后执行
    mounted: function () {
        window.onresize = () => { this.screenSize = document.body.clientWidth; this.screenSize > 992 ? this.leftMenuShow = true : this.leftMenuShow = false };
        if (this.screenSize > 992) this.leftMenuShow = true;
    },
    //函数方法
    methods: {
        //返回按钮
        goBack() {
            if (document.referrer && document.referrer.includes(location.host)) {
                location.href = document.referrer;
            } else {
                location.href = '/Home/Index3';
            };
        },
        //获取地址栏信息
        GetUrlParam() {
            //地址
            this.urlPathName = location.pathname;
            //参数
            let searchList = location.search;
            if (!searchList) return false;
            let params = {};
            //去掉?
            searchList = searchList.replace('?', '');
            //如果有多个键值对
            let lists = searchList.split('&');
            for (let i of lists) {
                let objArr = i.split("=");
                params[objArr[0]] = decodeURI(objArr[1]);
            };
            this.urlSearchParam = params;
        },
        //菜单hover进入
        enterHoverMenu() {
            if (this.screenSize <= 992) {
                this.leftMenuShow = true;
            };
        },
        //菜单点击
        changeMenuShow() {
            this.leftMenuShow = !this.leftMenuShow;
        }
    },
};