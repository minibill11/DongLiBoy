<!---  --->
<template>
  <div>
    <cHeader :active="active" ></cHeader>
    <!-- 操作栏 -->
    <div class="action-box">
        <div class="action-box-left">
            <el-date-picker size="small" v-model="select_month"
                            type="month"
                            placeholder="请选择月份" style="width:150px;">
            </el-date-picker>
            <el-button size="small" type="primary" @click="onQueryData" class='btn'>查询</el-button>
        </div>
    </div>

    <!-- 表格-->
    <div class="table-height">
        <el-table :data="tableData"
                  size="mini"
                  border
                  height='663px'
                  @row-click="onToTurn">
            <el-table-column prop="Ranking"
                             label="排名"
                             width="140">
                <template slot-scope="scope">
                    <div class="ranking-box"><span :class="['ranking',scope.row.Ranking<=3?'bg-red':'bg-blue']">{{scope.row.Ranking}}</span></div>
                </template>
            </el-table-column>
            <el-table-column label="部门"
                             width="160">
                <template slot-scope="scope">
                    <div class="department-style">{{scope.row.Department}}</div>
                </template>
            </el-table-column>
            <el-table-column label="7s得分">
                <template slot-scope="scope">
                    <div class="progress-box">
                        <div :class="['progress-line',scope.row.Ranking<=3?'bg-red':'bg-blue']" :style="{'width':`${scope.row.Score-20}%`}"></div>
                        <span>{{scope.row.Score}}</span>
                    </div>
                </template>
            </el-table-column>
        </el-table>
    </div>
  </div>
</template>

<script>
import cHeader from "./page-components/_kpi_7s_header";
import {getRankingData} from "@/api/hr/kpi-7s";
export default {
  name: 'KPI_7S_SummarizingRanking',
  props: {},
  data() {
    return {
      active:'部门评比排名',
      select_month:new Date(),
      tableData:[],
    };
  },
  components: {
    cHeader,
  },
  computed: {},
  watch: {},
  methods: {
    //查询数据
    onQueryData() {
        let dd = new Date(this.select_month);
        getRankingData(dd).then(res => {
          let arr=JSON.parse(res.data.Data.Data)
            arr.forEach(item => {
                item.Score = parseFloat(item.Score);
            })
            this.tableData = arr;
        })
    },
     //跳转
    onToTurn(row) {  
      this.$router.push({
                name: 'KPI_7S_Summarizing',
                params: {
                  department:row.Department,
                  time:this.select_month
                },
            });     

    }
  },
  created() {},
  mounted() {
    this.onQueryData()
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
.action-box {
    width: 100%;
    padding: 10px 0;
    display: flex;
    align-content: center;
    justify-content: space-between;
}

/*表格样式*/
td.el-table_1_column_3 {
    border: none;
}
.department-style {
    font-weight: bold;
    font-size: 16px;
    color: black;
}
.ranking-box {
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 8px;
}
.ranking {
    display: block;
    width: 36px;
    height: 34px;
    border-radius: 50%;
    line-height: 36px;
    text-align: center;
    line-height: 34px;
    text-align: center;
    font-size: 18px;
    color: #fff;
    font-weight: bold;
}
.bg-red {
    background-color: #ff8590;
}
.bg-blue {
    background-color: #b3d8ff;
}
.el-table td .cell {
    padding: 0;
}
.progress-box {
    width: 100%;
    display: flex;
    align-items: center;
}
    .progress-box span {
        display: inline-block;
        width: 65px;
        font-size: 24px;
        font-weight: bold;
        color: black;
    }
.progress-line {
    height: 20px;
}
</style>