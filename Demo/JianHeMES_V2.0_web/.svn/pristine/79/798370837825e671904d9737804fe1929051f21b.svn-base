<!---  --->
<template>
    <div>
        <moduleMenu active="模块看板"></moduleMenu>
        <el-form :inline="true" class="text-center">
            <el-form-item label="选择订单">
                <el-select
                    v-model="Ordernum"
                    multiple
                    collapse-tags
                    allow-create
                    default-first-option
                    filterable
                    clearable
                    placeholder="输入内容可查询"
                >
                    <el-option
                        v-for="item in ordernumOptions"
                        :key="item.value"
                        :label="item.value"
                        :value="item.value"
                    ></el-option>
                </el-select>
            </el-form-item>
            <el-form-item label="">
                <el-button type="primary" @click="onQuerySubmit"
                    >查询</el-button
                >
            </el-form-item>
        </el-form>

        <el-table
            :data="tableList"
            max-height="600"
            size="small"
            align="center"
            cell-class-name="cellParent"
            stripe
            border
            ><el-table-column
                type="index"
                label="序号"
                min-width="40"
                align="center"
                fixed
            >
            </el-table-column>
            <el-table-column
                prop="ordernum"
                label="订单号"
                min-width="100"
                align="center"
                fixed
            >
            </el-table-column>
            <el-table-column prop="type" label="平台型号" align="center" fixed>
            </el-table-column>
            <el-table-column
                prop="processingRequire"
                label="制程要求"
                align="center"
                fixed
            >
            </el-table-column>
            <el-table-column
                prop="standardRequire"
                label="标准要求"
                align="center"
                fixed
            >
            </el-table-column>
            <el-table-column
                prop="moduleNum"
                label="模块数"
                align="center"
                fixed
            >
            </el-table-column>
            <el-table-column
                prop="aiCount"
                label="AI机台完成数"
                align="center"
                fixed
            >
            </el-table-column>
            <el-table-column label="后焊" align="center">
                <el-table-column
                    prop="afterWeldingPass"
                    label="完成率"
                    align="center"
                >
                </el-table-column>
                <el-table-column
                    prop="samplingCount"
                    label="抽检率"
                    align="center"
                >
                </el-table-column>
                <el-table-column
                    prop="afterLine"
                    label="产线完成情况"
                    align="center"
                >
                </el-table-column>
            </el-table-column>
            <el-table-column label="灌胶电测" align="center">
                <el-table-column prop="gulePass" label="完成率" align="center">
                </el-table-column>
                <el-table-column
                    prop="gulePassThrough"
                    label="直通率"
                    align="center"
                >
                </el-table-column>
            </el-table-column>
            <el-table-column label="面罩电测" align="center">
                <el-table-column prop="maskPass" label="完成率" align="center">
                </el-table-column>
                <el-table-column
                    prop="maskPassThrough"
                    label="直通率"
                    align="center"
                >
                </el-table-column>
            </el-table-column>
            <el-table-column label="电测后" align="center">
                <el-table-column
                    prop="electricalSamplingCount"
                    label="抽检率"
                    align="center"
                >
                </el-table-column>
                <el-table-column
                    prop="electricalSamplingPass"
                    label="抽检合格率"
                    align="center"
                >
                </el-table-column>
            </el-table-column>
            <el-table-column label="老化" align="center">
                <el-table-column prop="burnPass" label="完成率" align="center">
                </el-table-column>
                <el-table-column
                    prop="burnPassThrough"
                    label="直通率"
                    align="center"
                >
                </el-table-column>
            </el-table-column>
            <el-table-column label="外观" align="center">
                <el-table-column
                    prop="appearancePass"
                    label="完成率"
                    align="center"
                >
                </el-table-column>
                <el-table-column
                    prop="appearancePassThrough"
                    label="直通率"
                    align="center"
                >
                </el-table-column>
            </el-table-column>
            <el-table-column label="包装数量" align="center">
                <el-table-column
                    prop="innerPackCount"
                    label="内箱"
                    align="center"
                >
                </el-table-column>
                <el-table-column
                    prop="outsidepackCount"
                    label="外箱"
                    align="center"
                >
                </el-table-column>
            </el-table-column>
            <el-table-column prop="warehousr" label="出入库情况" align="center">
            </el-table-column>
        </el-table>
    </div>
</template>

<script>
import setModuleManagement from "./page-components/module";
import moduleMenu from "./page-components/_moduleMenu";

import { module_index } from "@/api/order/ModuleManagement";
export default {
    name: "module-manage-index",
    props: {},
    data() {
        return {
            Ordernum: [],
            tableList: [], //存储表格总数据
        };
    },
    components: {
        moduleMenu,
    },
    computed: {},
    watch: {},
    methods: {
        //总表条件查询
        onQuerySubmit() {
            module_index(this.Ordernum)
                .then((res) => {
                    console.log(JSON.parse(JSON.stringify(res.data)));
                    this.tableList = res.data.Data;
                    this.$message.success("查询成功！");
                })
                .catch((err) => {
                    console.warn("查询数据失败");
                });
        },
    },
    mixins: [setModuleManagement],
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
@import url("./page-components/module.less");
.el-form {
    margin-top: 15px;
}
</style>