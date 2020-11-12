﻿const layout = {
    data: function () {
        return {
            mainLoading: false,//loading状态开关
            screenSize: document.body.clientWidth,//屏幕尺寸
            urlPathName: [],//地址
            urlSearchParam: {},//地址参数
            activeIndex: null,//菜单选项
            openedsArr: [],//菜单选项数组
            leftMenuShow: false,//菜单的显示
            MESHOST: location.origin,//前后分离host
            currentVersion: '',//当前版本
        }
    },
    //页面打开时执行
    created: function () {
        //获取地址栏信息
        this.GetUrlParam();
        if (!this.userName) {
            let skipurl = this.urlPathName.split('/').filter(i => i);
            if (skipurl) { location.href = `/Users/Login2?col=${skipurl[0]}&act=${skipurl.slice(1).join('/')}` } else { location.href = '/Users/Login2' };
        };
        this.GetUserInfo();
        this.GetVersion();
        //处理菜单展开
        this.activeIndex = localStorage.getItem("menuIndexKey");
        this.openedsArr = JSON.parse(localStorage.getItem("menuIndexKeyPath"));
        if (this.activeIndex == null || (this.activeIndex != null && this.urlPathName != this.activeIndex.split('→')[1])) {
            this.activeIndex = '';
            this.openedsArr = [];
            if (this.urlPathName == '/Home/Index3') {
                this.openedsArr.push('首页→/Home');
            } else {
                this.getNavTitle(this.urlPathName, setMuneList);
            };
            if (this.openedsArr.length === 0) {
                this.openedsArr.push(`${document.title.split('-')[0]}→//`);
            };
        };
    },
    //页面加载完后执行
    mounted: function () {
        window.onresize = () => { this.screenSize = document.body.clientWidth; this.screenSize > 992 ? this.leftMenuShow = true : this.leftMenuShow = false };
        if (this.screenSize > 992) this.leftMenuShow = true;
    },
    //函数方法
    methods: {
        //递归获取导航标题
        getNavTitle(url, menulist, parent) {
            for (let m of menulist) {
                if ('children' in m) {
                    if (parent === undefined) {
                        this.getNavTitle(url, m.children, [m.url]);
                    } else {
                        parent.push(m.url);
                        this.getNavTitle(url, m.children, parent);
                    };
                } else {
                    if (url == m.url.split('→')[1]) {
                        this.activeIndex = m.url;
                        if (parent === undefined) {
                            this.openedsArr.push(m.url);
                        } else {
                            parent.push(m.url);
                            this.openedsArr = parent;
                        };
                    };
                };
            };
        },
        //返回按钮
        //goBack() {
        //    window.history.back();
        //    //if (document.referrer && document.referrer.includes(location.host)) {
        //    //    location.href = document.referrer;
        //    //} else {
        //    //    location.href = '/Home/Index3';
        //    //};
        //},
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
        //获取用户信息
        GetUserInfo() {
            //axios.post(`${this.MESHOST}/api/Users_Api/UserInfo`, {}, { headers: { Authorization: localStorage.getItem('authorization') } }).then(res => {
            //    if (res.data.PostResult.AuthorizationResult) { /*console.dir(res.data)*/ };
            //});
        },
        //获取版本信息
        GetVersion() {
            if (sessionStorage.getItem('mesVersion')) { this.currentVersion = sessionStorage.getItem('mesVersion') } else {
                axios.post('/VersionManagement/GetLasrVersion').then(res => { this.currentVersion = res.data; sessionStorage.setItem('mesVersion', res.data); });
            };
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
        },
    },
};