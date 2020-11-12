<!---  --->
<template>
    <div>
        <el-header class="text-center">
            <h1>校正信息首页</h1>
        </el-header>
        <el-row class="text-center">
            <el-form :inline="true" size="small">
                <el-form-item label="选择订单">
                    <el-select
                        v-model="Ordernum"
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
                    <a
                        href="New_CalibrationRecord"
                        style="margin-left: 10px"
                        v-show="$limit('开始校正')"
                        ><el-button type="primary" size="small" plain
                            >开始校正工作</el-button
                        ></a
                    >
                    <a
                        href="Batch_CalibrationRecord"
                        style="margin-left: 10px"
                        v-show="$limit('开始校正')"
                        ><el-button type="primary" size="small" plain
                            >批量校正</el-button
                        ></a
                    >
                </el-form-item>
            </el-form>
        </el-row>
        <indexInfo
            ref="indexInfo"
            v-show="tableList.length > 0"
            :infodata="infodata"
        ></indexInfo>
        <el-row class="text-center">
            <el-table
                v-for="(table, index) in tableList"
                :key="index"
                :data="table.tableList"
                max-height="500"
                size="small"
                align="center"
                cell-class-name="cellParent"
                stripe
                border
            >
                <el-table-column
                    :label="`${table.group}(${table.tableList.length})`"
                    class-name="table-title"
                >
                    <el-table-column
                        type="index"
                        label="序号"
                        min-width="40"
                        align="center"
                    >
                    </el-table-column>
                    <el-table-column
                        prop="ordernum"
                        label="订单号"
                        min-width="100"
                        align="center"
                    >
                    </el-table-column>
                    <el-table-column
                        prop="barcode"
                        label="条码号"
                        min-width="130"
                        sortable
                        align="center"
                    >
                    </el-table-column>
                    <el-table-column
                        prop="moduleNum"
                        label="模组号"
                        min-width="80"
                        sortable
                        align="center"
                    >
                    </el-table-column>
                    <el-table-column
                        prop="principal"
                        label="操作人"
                        min-width="80"
                        align="center"
                    >
                    </el-table-column>
                    <el-table-column
                        prop="startTime"
                        label="开始时间"
                        min-width="120"
                        align="center"
                    >
                    </el-table-column>
                    <el-table-column
                        prop="endTime"
                        label="完成时间"
                        min-width="120"
                        align="center"
                    >
                    </el-table-column>
                    <el-table-column
                        prop="Repetition"
                        label="是否重复"
                        min-width="60"
                        align="center"
                    >
                    </el-table-column>
                    <el-table-column
                        prop="RepetitionCause"
                        label="重复原因"
                        min-width="100"
                        align="center"
                    >
                    </el-table-column>
                    <el-table-column
                        prop="RepairCondition"
                        label="维修情况"
                        sortable
                        min-width="100"
                        align="center"
                    >
                    </el-table-column>
                    <el-table-column
                        prop="nornal"
                        label="是否完成"
                        sortable
                        min-width="100"
                        align="center"
                    >
                    </el-table-column>
                    <el-table-column label="操作" min-width="50" align="center">
                        <template slot-scope="scope">
                            <a
                                v-if="$limit('修改校正记录')"
                                :href="`Edit/${scope.row.id}`"
                                target="_blank"
                                >编辑</a
                            >
                        </template>
                    </el-table-column>
                </el-table-column>
            </el-table>
        </el-row>
    </div>
</template>

<script>
import setIndex from "../page-components/index";
import { query_NewIndex } from "@/api/order/index";
import indexInfo from "../page-components/_index_info";
export default {
    name: "pqc-index",
    props: {},
    data() {
        return {
            tableList: [], //存储表格总数据
            infodata: {},
        };
    },
    components: {
        indexInfo,
    },
    computed: {},
    watch: {},
    methods: {
        //总表条件查询
        onQuerySubmit() {
            if (this.Ordernum == null || this.Ordernum == "") {
                this.$message("未选择订单");
                return;
            }
            query_NewIndex("CalibrationRecordsAPI", this.Ordernum).then(
                (res) => {
                    let rtd = res.data.Data,
                        info = {};
                    info.ordernum = this.Ordernum;
                    info.baseinfo = rtd.baseinfo;
                    info.statuList = rtd.statuList;
                    info.groupPie = {
                        title: { text: "班组完成饼图" },
                        series: [{ data: rtd.groupPie }],
                    };
                    info.statuePie = {
                        title: { text: "异常正常饼图" },
                        series: [{ data: rtd.statuePie }],
                    };
                    this.infodata = info;
                    this.tableList = rtd.table;
                }
            );
        },
    },
    mixins: [setIndex],
    created() {},
    mounted() {
        //this.Ordernum = "2017-TEST-1";
        //console.log(this.$refs.indexInfo.cc);
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
@import url("../page-components/index.less");
</style>