<!---  --->
<template>
  <div>
    <cHeader :active="active"></cHeader>
    <div class="headerstyle">
      <span style="font-size: 14px" class="timelength">统计截止日期：</span>
      <el-date-picker value-format="yyyy-MM-dd HH:mm:ss" v-model="inputDate" type="month"  placeholder="选择录入日期"  size="small" style="width: 180px">
      </el-date-picker>
      <el-button type="primary"  size="small"  @click="selectInfo" v-show="$limit('查看原始文件')" class="qty">查询</el-button>
      <el-button type="success" plain size="small" v-on:click="exportExcel" v-show="tableLength.length > 0 && $limit('生成报表')" >导出结果</el-button>
      <el-button type="success" plain size="small" v-on:click="createDate" v-show="tableLength.length > 0 && $limit('生成报表')" class="qty" >生成报表</el-button>
      <span style="display: inline-block; font-size: 14px" class="timelength">查询时长:{{ count }}秒</span>
    </div>
    <cTable ref="table" class="home" :tableType="'查询ERP月度库存金额'" :time="inputDate"></cTable>
  </div>
</template>

<script>
import cHeader from "./page-components/_stock_header";
import cTable from "./page-components/_stock_table";
import { erpQuery, createReport, exportReport } from "@/api/order/warehouse";
export default {
  name: "stock_erpquery",
  props: {},
  data() {
    return {
      active: "查询ERP库存月度结存金额",
      inputDate: "",
      count: 0,
      intervalId: null,
      tableLength: [],
    };
  },
  components: {
    cHeader,
    cTable,
  },
  computed: {},
  watch: {},
  methods: {
    //查询
    selectInfo() {
      let dd = new Date(this.inputDate);
      let inputDate = dd.getFullYear();
      let endDate = dd.getMonth() + 1;
      this.count = 0;
      if (this.inputDate != "") {
        if (this.intervalId != null) {
          return;
        }
        this.intervalId = setInterval(() => {
          this.count += 1;
        }, 1000);
        let obj = { year: inputDate, month: endDate };
        erpQuery(obj).then((res) => {
            this.$refs.table.tableDate = res.data.Data.Data;
            this.tableLength = this.$refs.table.tableDate;
            console.log(this.tableLength);
            if (this.$refs.table.tableDate.length > 0) {
              clearInterval(this.intervalId);
              this.intervalId = null;
            }
          }).catch((err) => {
            console.warn("请求数据失败!");
          });
      } else {
        this.$message.error("请选择查询日期！");
      }
    },
    //导出excel方法
    exportExcel() {
      let dd = new Date(this.inputDate);
      let inputDate = dd.getFullYear();
      let endDate = dd.getMonth() + 1;
      let obj = {
        tableData: JSON.stringify(this.$refs.table.tableDate),
        year: inputDate,
        month: endDate,
      };
      exportReport(obj).then(function (res) {
        let fileName = inputDate + '年' + endDate + '月库存金额表.xlsx'
        let url = window.URL.createObjectURL(new Blob([res.data]))
        let link = document.createElement('a')
        link.style.display = 'none'
        link.href = url
        link.setAttribute('download', fileName)
        document.body.appendChild(link)
        link.click()
      });
    },
    //生成报表
    createDate() {
      if (this.$refs.table.tableDate.length > 0) {
        let obj = { record: this.$refs.table.tableDate, date: this.inputDate };
        createReport(obj).then((res) => {
          console.log(res.data);
          if (res.data.Result == true) {
            this.$message.success(res.data.Message);
          } else {
            this.$message.error(res.data.Message);
          }
        });
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
  activated() {},
};
</script>

<style scoped>
.headerstyle {
  text-align: center;
  margin-top: 5px;
  margin-bottom: 5px;
  display: inline-flex;
}
.qty {
  margin-left: 5px;
}

.home >>> .el-table thead.is-group th {
  background: #e4ecf7;
}
.timelength {
  margin: 5px;
}
</style>