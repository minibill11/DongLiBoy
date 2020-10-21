<!---  --->
<template>
  <div>
    <cHeader :active="active" ></cHeader>
    <div class="hang">
      <div class="select-style">
        <el-date-picker v-model="inputDate" type="year" placeholder="选择年" size="small" style="width:180px"></el-date-picker>
        <el-button type="primary" size="small" v-on:click="getDate" class="qty_btn" :disabled="!$limit('查看已审核发布')||!$limit('查看原始文件')">查询</el-button>
      </div>
      <el-table v-bind:data="tableData" max-height="623" border :header-cell-style="{'text-align':'center'}" :cell-style="{'text-align':'center'}" style="width:80%">  
         <el-table-column prop="Time" label="月份" width="200"></el-table-column>
         <el-table-column prop label="财务核对人" width="280">
             <template slot-scope="scope">
                 {{scope.row.Finance_Proofreader}}<span class="el-span">{{scope.row.Finance_Proofread_Date}}</span>
             </template>
          </el-table-column>
          <el-table-column prop label="PMC核对人" width="280">
             <template slot-scope="scope">
                 {{scope.row.Finance_Finance_Proofreader}}<span class="el-span">{{scope.row.PMC_Proofread_Date}}</span>
              </template>
          </el-table-column>
          <el-table-column prop label="财务审核" width="280">
                <template slot-scope="scope">
                  {{scope.row.Finance_Assessor}}<span class="el-span">{{scope.row.Finance_Assessed_Date}}</span>
                </template>
          </el-table-column>
          <el-table-column prop label="操作">
                <template slot-scope="scope">
                  <u style="color:#409eff" v-on:click="goDisplay(scope.row)">详细</u>
                </template>
          </el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
import cHeader from "./page-components/_stock_header";
import {getRecord} from "@/api/order/warehouse";
export default {
  name: "stock_index",
  props: [], //字符串数组写法
  data() {
    return {
      tableData: [],
      inputDate: "",
      active: "库存金额信息"
    };
  },
  components: {
    cHeader,
  },
  computed: {},
  watch: {},
  methods: {
        getDate() {
            let type = ''; //根据权限查询
            if (this.$limit('查看已审核发布') && !this.$limit('查看原始文件')) {
                type = 'approved';
            };
            if (this.$limit('查看已审核发布') && this.$limit('查看原始文件')) {
                type = 'original';
            };
            console.log(this.$limit('查看已审核发布') )
            getRecord(this.inputDate.getFullYear(), type).then((res) => {
              console.log(res.data)
                this.tableData = res.data.Data.Data;
                this.$message.success(res.data.Data.Message);
            });
        },
        goDisplay(row) {        
            this.$router.push({
                name: "stock_query",
                query: {
                time: row.Time,
                },
            });
        },
  },
  created() {},
  mounted() {
    let year = new Date();
    this.inputDate = year;
    if (this.$limit('查看已审核发布') && !this.$limit('查看原始文件')) {        
        this.getDate()
    };
    if (this.$limit('查看已审核发布') && this.$limit('查看原始文件')) {
        this.getDate()
    }; 
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
.el-table {
  font-size: 13px;
  font-family: "微软雅黑";
  color: black;
  margin: 0 auto;
}
.select-style {
    display: inline-block;
    margin: 10px 0 10px 0;
}
    
.qty_btn {
    margin: 5px;
}
    
.el-span {
    margin-left: 15px;
}
    
.hang {
    text-align: center;
}
</style>