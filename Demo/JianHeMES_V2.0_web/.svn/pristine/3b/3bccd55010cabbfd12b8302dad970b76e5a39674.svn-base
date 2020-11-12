<!---  --->
<template>
    <el-row class="barcodeStateCard">
        <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="8">
            <el-card class="notDo">
                <div slot="header">
                    <span>未开始 {{ notDoList.length }}个 </span>
                </div>
                <ul>
                    <li v-for="item in notDoList" :key="item">{{ item }}</li>
                </ul>
            </el-card>
        </el-col>
        <!-- <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
            <el-card class="notFinish">
                <div slot="header">
                    <span>进行中 {{ notFinishList.length }}个 </span>
                </div>
                <ul>
                    <li v-for="item in notFinishList" :key="item">
                        {{ item }}
                    </li>
                </ul>
            </el-card>
        </el-col> -->
        <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="8">
            <el-card class="finish">
                <div slot="header">
                    <span>已完成 {{ finishList.length }}个 </span>
                </div>
                <ul>
                    <li v-for="item in finishList" :key="item">{{ item }}</li>
                </ul>
            </el-card>
        </el-col>
    </el-row>
</template>

<script>
export default {
    name: "",
    props: ["value"],
    data() {
        return {
            notDoList: [],
            notFinishList: [],
            finishList: [],

        };
    },
    components: {},
    computed: {},
    watch: {
        value(v) {
            this.notDoList = v.NotDoList == null ? [] : v.NotDoList;
            this.notFinishList = v.NeverFinish == null ? [] : v.NeverFinish;
            this.finishList = v.FinishList == null ? [] : v.FinishList;
        },
    },
    methods: {},
    created() {},
    mounted() {},
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
/* 定义变量 */
@red: #f56c6c;
@blue: #409eff;
@green: #67c23a;

.borderColor(@bdc: #EBEEF5) {
    border-color: @bdc;
}

.headerBottomColor(@bbt: #EBEEF5) {
    color: @bbt;
    .borderColor(@bbt);

    /*box-shadow: 0 1px 6px 0 @bbt;*/
    .el-card__header {
        .borderColor(@bbt);
    }
}
.barcodeStateCard {
    text-align: center;

    .el-card {
        margin: 0 auto 10px;
        max-width: 250px;

        @media (min-width: 992px) {
            margin: 0 8px 8px;
        }
    }

    .el-card__header {
        padding: 8px 0;
    }

    /deep/.el-card__body {
        padding: 5px;
        overflow: auto;
        /* 屏幕小于992 */
        height: 200px;

        @media (min-width: 992px) {
            height: 400px;
        }
    }

    ul {
        list-style: none;
        padding: 0;
    }

    .notDo {
        .headerBottomColor(@red);
    }

    .notFinish {
        .headerBottomColor(@blue);
    }

    .finish {
        .headerBottomColor(@green);
    }
}
</style>