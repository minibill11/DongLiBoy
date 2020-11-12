<!---  --->
<template>
  <div>
    <cHeader :active="active" ></cHeader>
     <!-- 操作栏-->
    <div class="se_name">
      <cSelect :source="'7S日检记录查询'" ref="sel" ></cSelect>
      <el-date-picker size="small" v-model="select_month"
                      type="month"
                      placeholder="请选择月份" style="width:150px;" class="btn">
      </el-date-picker>
      <el-button size="small" type="primary" v-on:click="onQueryData" class="btn">查询</el-button>
    </div>
    <!-- 表格 -->
    <div class="table-height">
        <cTable :source="active" :select_month="select_month" ref='tab' :show="false"></cTable>
    </div>
  </div>
</template>

<script>
import cHeader from "./page-components/_kpi_7s_header";
import cSelect from "./page-components/_select_component";
import cTable from "./page-components/_kpi_table";
import {GetDailyRecorData} from "@/api/hr/kpi-7s";
export default {
  name: 'KPI_7S_DailyRecord',
  props: {},
  data() {
    return {
        active:'日检记录查询',
        select_month: new Date(),
    };
  },
  components: {
      cHeader,
      cSelect,
      cTable
  },
  computed: {
  },
  watch: {},
  methods: {
    //查询数据
    onQueryData() {
        if (this.onTip()) {
            this.loading = true;
            let param = {
                department: this.$refs.sel.select_department,
                date: new Date(this.select_month)
            };
            GetDailyRecorData(param).then(res => {
                if (res.data.Data.length != 0) {
                    let tableData = res.data.Data;
                    let len = tableData[0].DailyRecord.length;
                    //处理表头
                    let list = [];
                    for (let index = 0; index < len; index++) {
                        list.push({
                            title: index + 1 + '日',
                            field: 'data' + index
                        })
                    }
                    this.$refs.tab.tableColumn = list;
                    //处理数据
                    for (let k = 0; k < tableData.length; k++) {
                        for (let j = 0; j < len; j++) {
                            let key = "data" + j;
                            tableData[k][key] = tableData[k].DailyRecord[j];
                        }
                    }
                    this.$refs.tab.tableData = tableData;
                    this.$message.success('查询成功！');
                } else {
                    this.$message.info('暂无记录！');
                    this.$refs.tab.tableData = [];
                }
            })
        }
    },
    //判断筛选
    onTip() {
        if (this.select_month == null) {
            this.$message.warning('请选择月份！');
            return false;
        } else {
            return true;
        }
    },

  },
  created() {

  },
  mounted() {
    this.onQueryData();
  },
  beforeCreate() {},
  beforeMount() {},
  beforeUpdate() {},
  updated() {},
  beforeDestroy() {},
  destroyed() {},
  activated() {},
}
</script>

<style lang='less' scoped>
@import url('~@/assets/style/color.less');
@import url('./kpi7s.less');
/*多级表头*/
.el-table thead.is-group th {
    background: #E4ECF7;
}
</style>