<!---  --->
<template>
    <el-row class="info">
        <el-col :span="14">
            <el-row
                ><b>订单 {{ ordernum }} 基本情况</b></el-row
            >
            <el-divider class="base-divider"></el-divider>
            <el-row class="base-info">
                <div>
                    订单的模组总量：<b>{{ baseinfo.Boxs }}</b>
                </div>
                <div>
                    总操作数量：<b>{{ baseinfo.totalCount }}</b>
                </div>
                <div>
                    正常数量：<b>{{ baseinfo.normalCount }}</b>
                </div>
                <div>
                    异常数量：<b>{{ baseinfo.abnormalCount }}</b>
                </div>
                <div>
                    完成率：<b>{{ baseinfo.completeRate }}</b>
                </div>
                <div>
                    总时长：<b>{{ baseinfo.totalTime }}</b>
                </div>
                <div>
                    平均时长：<b>{{ baseinfo.averageTime }}</b>
                </div>
                <div>
                    查询时间：<b>{{ baseinfo.selectTime }}</b>
                </div>
            </el-row>
            <el-divider class="base-divider"></el-divider>
            <el-row class="statu-row">
                <el-col
                    v-for="(item, index) in statuList"
                    :key="index"
                    class="statu-col"
                >
                    <div>{{ item.name }}：{{ item.List.length }}个</div>
                    <div class="statu-list">
                        <ul>
                            <li v-for="i in item.List" :key="i">{{ i }}</li>
                        </ul>
                    </div>
                </el-col>
            </el-row>
            <el-divider class="base-divider"></el-divider>
        </el-col>
        <el-col :span="5">
            <div id="groupPie" class="echart-div"></div>
        </el-col>
        <el-col :span="5">
            <div id="statuePie" class="echart-div"></div>
        </el-col>
    </el-row>
</template>

<script>
import echarts from "echarts";
export default {
    name: "",
    props: ["infodata"],
    data() {
        return {
            ordernum: null,
            baseinfo: {},
            statuList: [],
            groupPie: [],
            statuePie: [],
            echartOption: null,
        };
    },
    components: {},
    computed: {},
    watch: {
        infodata(v) {
            this.ordernum = v.ordernum;
            this.baseinfo = v.baseinfo;
            this.statuList = v.statuList;
            this.groupPie.setOption(v.groupPie);
            this.statuePie.setOption(v.statuePie);
        },
    },
    methods: {},
    created() {
        this.echartOption = {
            title: { left: "center" },
            tooltip: {
                trigger: "item",
                formatter: "{a} <br/>{b} : {c} ({d}%)",
            },
            color: ["#22c49e", "#f7c73a", "#ff8590"],
            legend: { bottom: 0, left: "center" },
            series: [
                {
                    name: "数量信息(占比)",
                    type: "pie",
                    radius: "50%",
                    center: ["50%", "50%"],
                    emphasis: {
                        itemStyle: {
                            shadowBlur: 10,
                            shadowOffsetX: 0,
                            shadowColor: "rgba(0, 0, 0, 0.5)",
                        },
                    },
                },
            ],
        };
    },
    mounted() {
        this.groupPie = echarts.init(document.getElementById("groupPie"));
        this.statuePie = echarts.init(document.getElementById("statuePie"));
        this.groupPie.setOption(this.echartOption);
        this.statuePie.setOption(this.echartOption);
    },
    beforeCreate() {},
    beforeMount() {},
    beforeUpdate() {},
    updated() {},
    beforeDestroy() {},
    destroyed() {},
    activated() {},
};
</script>

<style lang='less' scoped>
@import url("~@/assets/style/color.less");
.base-divider {
    margin: 10px 0;
}

.base-info,
.statu-row {
    display: flex;
    justify-content: flex-start;
    flex-flow: row wrap;
    font-size: 13px;
}

.base-info > div {
    min-width: 200px;
    margin: 0 5px 5px 0;
}

.echart-div {
    width: 200px;
    height: 200px;
    margin: 0 auto;
}

.statu-col {
    width: 200px;
    padding-right: 20px;

    @media (max-width: 992px) {
        margin-bottom: 10px;
    }
}

.statu-list {
    max-height: 100px;
    overflow: auto;

    ul {
        list-style: none;
        padding: 0;
        margin: 8px 0 0;

        li {
            font-size: 12px;
            height: 15px;
            line-height: 15px;
            cursor: pointer;
            margin: 1px auto;
            transition: all linear 0.1s;

            &:hover {
                font-size: 15px;
                height: 20px;
                line-height: 20px;
                border-radius: 8px;
                background: #f7c73a;
            }
        }
    }
}
</style>