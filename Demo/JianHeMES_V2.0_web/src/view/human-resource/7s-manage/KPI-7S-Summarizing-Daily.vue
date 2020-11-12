<!---  --->
<template>
  <div>
    <cHeader :active="active" ></cHeader>
    <!-- 操作栏 -->
    <div class="se_name">
      <el-select size="small" v-model="select_check_type" placeholder="请选择检查类型" style="width:150px;">
          <el-option label="日检" value="日检"></el-option>
          <el-option label="周检" value="周检"></el-option>
          <el-option label="巡检" value="巡检"></el-option>
      </el-select>
      <cSelect :source="'日/周/巡检汇总表'" ref="sel"></cSelect>
      <el-date-picker size="small" v-model="select_month"
                      type="month"
                      placeholder="请选择月份" style="width:150px;" class='btn'>
      </el-date-picker>
      <el-button size="small" type="primary" v-on:click="onQueryData" class='btn'>查询</el-button>
      <cExcel :excelData="allData"  :time="select_month" class="btn" :select_check_type="select_check_type"></cExcel>
    </div>
    <!-- 表格 -->
    <div class="table-height">
        <cTable :source="active" :select_month="select_month" ref='tab' :select_check_type="select_check_type" :show="true"></cTable>
    </div>
  </div>
</template>

<script>
import cHeader from "./page-components/_kpi_7s_header";
import cSelect from "./page-components/_select_component";
import cTable from "./page-components/_kpi_table";
import {QueryWeekData,QueryDailyData} from "@/api/hr/kpi-7s";
import cExcel from "./page-components/_export_daily";
export default {
  name: 'KPI_7S_Summarizing_Daily',
  props: {},
  data() {
    return {
      active:'日/周/巡检汇总表',
      select_check_type:null,
      select_month:null,
      check_name: '检查扣分',
      tableColumn: [],//表头
      allData: [],
      count: 0,
    };
  },
  components: {
    cHeader,
    cSelect,
    cTable,
    cExcel
  },
  computed: {},
  watch: {},
  methods: {
    //判断筛选
    onTip() {
        if (this.select_check_type == (null || undefined)) {
            this.$message.warning('请选择检查类型！');
            return false;
        } else if (this.$refs.sel.select_department == ('' || undefined)) {
            this.$message.warning('请选择部门！');
            return false;
        } else if (this.select_month == (null || undefined)) {
            this.$message.warning('请选择月份！');
            return false;
        } else {
            return true;
        }
    },
    //表头生成
    onCreatTableColumn() {
        let len;
        let list = [];
        if (this.select_check_type == '周检') {
            for (let i = 0; i < this.allData.length; i++) {
                len = this.allData[0].Week.length;
            }
            for (let index = 0; index < len; index++) {
                let i = index + 1;
                list.push({
                    title: '第' + i + '周',
                    field: 'data' + index
                })
            }
        } else {
            for (let i = 0; i < this.allData.length; i++) {
                len = this.allData[0].PointsDeducted.length;
            }
            for (let index = 0; index < len; index++) {
                list.push({
                    title: index + 1 + '日',
                    field: 'data' + index,
                    width: 50
                })
            }
        }
        this.$refs.tab.tableColumn = list;
    },
    //处理数据
    onProcessData() {
        let len;
        for (let i = 0; i < this.allData.length; i++) {
            if (this.select_check_type == '周检') {
                len = this.allData[0].Week.length;
                for (let j = 0; j < len; j++) {
                    var key = "data" + j;
                    this.allData[i][key] = this.allData[i].Week[j];
                }
            } else {
                len = this.allData[0].PointsDeducted.length;
                for (let j = 0; j < len; j++) {
                    var key = "data" + j;
                    this.allData[i][key] = this.allData[i].PointsDeducted[j];
                }
            }
        }
        this.$refs.tab.tableData = this.allData;
    },
    //查询数据
    onQueryData() {
        if (this.onTip()) {
            if (this.select_check_type == '周检') {
                let param = {
                    department: this.$refs.sel.select_department,
                    date: new Date(this.select_month),
                    position: this.$refs.sel.select_position,
                    district: this.$refs.sel.select_district,
                };
                QueryWeekData(param).then(res => {
                    this.allData = res.data.Data;
                    if (res.data.Data.length != 0) {
                        this.onCreatTableColumn();//处理表头
                        this.onProcessData();//处理数据
                        this.count = 0;
                    } else {
                        this.allData = [];
                        this.$refs.tab.tableData = [];
                    }
                    this.$message.success('查询成功！');
                })
            } else {
                let param = {
                    check_Type: this.select_check_type,
                    department: this.$refs.sel.select_department,
                    date: new Date(this.select_month),
                    position: this.$refs.sel.select_position,
                    district: this.$refs.sel.select_district,
                };
                QueryDailyData(param).then(res => {
                    this.allData = res.data.Data;
                    if (res.data.Data.length != 0) {
                        this.onCreatTableColumn();
                        this.onProcessData();
                        if (this.count == 0) {
                            this.onQueryData()
                            this.count += 1;
                        }
                    } else {
                        this.allData = [];
                        this.$refs.tab.tableData = [];
                    }
                    this.$message.success('查询成功！');
                })
            }
        }
    },
  },
  created() {},
  mounted() {},
  beforeCreate() {},
  beforeMount() {},
  beforeUpdate() {},
  updated() {},
  beforeDestroy() {},
  destroyed() {},
  activated() {
    //从汇总表页面进入
    if("check_type" in this.$route.params){
        this.select_check_type=this.$route.params.check_type
        this.$refs.sel.select_department= this.$route.params.department
        this.$refs.sel.select_position=this.$route.params.position
        this.$refs.sel.select_district=this.$route.params.district
        this.select_month=this.$route.params.date
        this.onQueryData()
    }
  },
}
</script>

<style lang='less' scoped>
@import url('./kpi7s.less');
</style>