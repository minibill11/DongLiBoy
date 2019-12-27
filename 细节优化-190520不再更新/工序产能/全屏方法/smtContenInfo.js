﻿var chartMixin = {
    data: function () {
        return {
            loading: false,
            drawerShow: true,
            efficiencyChart: "",
            qualityChart: "",
            efficiencyChartbody: "",
            qualityChartbody: "",
            poll: 0,
            pickerOptions: {
                disabledDate(time) {
                    return time.getTime() > Date.now();
                },
                shortcuts: [{
                    text: '今天',
                    onClick(picker) {
                        picker.$emit('pick', new Date());
                    }
                }, {
                    text: '昨天',
                    onClick(picker) {
                        const date = new Date();
                        date.setTime(date.getTime() - 3600 * 1000 * 24);
                        picker.$emit('pick', date);
                    }
                }, {
                    text: '一周前',
                    onClick(picker) {
                        const date = new Date();
                        date.setTime(date.getTime() - 3600 * 1000 * 24 * 7);
                        picker.$emit('pick', date);
                    }
                }]
            },
        }
    },
    created: function () {
        this.postTime = new Date();
    },
    mounted: function () {
        //echart折线图配置
        var chartOption = {
            title: {
                left: 'center'
            },
            tooltip: {
                trigger: 'axis'
            },
            grid: {
                left: '3%',
                right: '5%',
                bottom: '7%',
                containLabel: true
            },
            xAxis: {
                type: 'category'
            },
            yAxis: {
                type: 'value',
                axisLabel: {
                    formatter: '{value} %'
                }
            },
            series: [{
                name: "达标率(%)",
                type: 'bar',
                label: {
                    normal: {
                        show: true,
                        position: 'top',
                        formatter: '{c} %',
                    }
                },
            }]
        };
        setTimeout(() => {
            //echart初始化
            this.efficiencyChart = echarts.init(document.getElementById('efficiencyBar'));
            this.efficiencyChart.setOption(chartOption);
            this.efficiencyChart.setOption({ title: { text: '效率未达标线别 - TOP3' } });
            this.qualityChart = echarts.init(document.getElementById('qualityBar'));
            this.qualityChart.setOption(chartOption);
            this.qualityChart.setOption({ title: { text: '品质未达标线别 - TOP3' } });
            //body
            this.efficiencyChartbody = echarts.init(document.getElementById('efficiencyBarbody'));
            this.efficiencyChartbody.setOption(chartOption);
            this.efficiencyChartbody.setOption({ title: { text: '效率未达标线别 - TOP3' } });
            this.qualityChartbody = echarts.init(document.getElementById('qualityBarbody'));
            this.qualityChartbody.setOption(chartOption);
            this.qualityChartbody.setOption({ title: { text: '品质未达标线别 - TOP3' } });
        }, 0);
    },
    methods: {
        getdata: function (address) {
            this.loading = true;
            axios.post("/SMT/" + address, {
                time: this.postTime,
            }).then(res => {
                //console.log(res.data);
                //表格数据
                this.dataList = res.data;
                //图表数据
                this.efficiencyChart.setOption({
                    series: [{ data: res.data.efficiency.value }],
                    xAxis: { data: res.data.efficiency.line }
                });
                this.qualityChart.setOption({
                    series: [{ data: res.data.quality.value }],
                    xAxis: { data: res.data.quality.line }
                });
                //body
                this.efficiencyChartbody.setOption({
                    series: [{ data: res.data.efficiency.value }],
                    xAxis: { data: res.data.efficiency.line }
                });
                this.qualityChartbody.setOption({
                    series: [{ data: res.data.quality.value }],
                    xAxis: { data: res.data.quality.line }
                });
                this.loading = false;
            }).catch(err => {
                console.warn(err);
                this.loading = false;
            });
        },
        isPoll(v) {
            let now = new Date(), select = new Date(v);
            this.poll && clearInterval(this.poll);
            return now.toLocaleDateString() == select.toLocaleDateString();
        },
        launchFullscreen() {
            //关闭
            if (document.exitFullscreen) {
                document.exitFullscreen();
            } else if (document.mozCancelFullScreen) {
                document.mozCancelFullScreen();
            } else if (document.webkitExitFullscreen) {
                document.webkitExitFullscreen();
            };
            //启动
            if (document.documentElement.requestFullscreen) {
                document.documentElement.requestFullscreen();
            } else if (document.documentElement.mozRequestFullScreen) {
                document.documentElement.mozRequestFullScreen();
            } else if (document.documentElement.webkitRequestFullscreen) {
                document.documentElement.webkitRequestFullscreen();
            } else if (document.documentElement.msRequestFullscreen) {
                document.documentElement.msRequestFullscreen();
            };
        }
    }
}