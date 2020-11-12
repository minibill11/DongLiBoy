<!---  --->
<template>
   <div class="sa-table">
        <el-table :data="tableDate" max-height="574" class="table" :row-class-name="tableRowClassName" :header-cell-style="{ 'text-align': 'center' }">
            <el-table-column prop="" label="类别" width="85" fixed align="center">
                <template slot-scope="scope">
                     <u style="color:#409eff" type="text" size="middle" v-on:click="LookExcelFile(scope.row.Category)" >{{ scope.row.Category }}</u>
                </template>
            </el-table-column>
            <el-table-column prop="Category_Detail" label="分类明细" width="120" fixed align="center"></el-table-column>
            <el-table-column prop="" label="库龄">
              <el-table-column prop="OneMonth" label="1个月以内" width="140" align="right"> 
                <template slot-scope="scope">
                        {{scope.row.OneMonth}}
                          <el-tooltip placement="top-start" effect="dark"	popper-class="atooltip"	>
                            <div slot="content">
　                          　<span style="text-align:left">上月数据</span><br/><span>{{lastMonth.length>0?lastMonth[scope.$index].OneMonth:''}}</span>　                          
                            </div>
                          	 <span  v-show="(scope.row.Category=='财务科目类'||scope.row.Category_Detail=='在制品类')&&lastMonth.length>0&&tableType=='月度库存金额'"><i :class="lastMonth.length>0?compare(scope.row.OneMonth,lastMonth[scope.$index].OneMonth):''"></i></span>
                          </el-tooltip>
                    </template>                 
              </el-table-column>
              <el-table-column prop="TwoMonths" label="2-3个月" width="140" align="right"> 
                <template slot-scope="scope">
                        {{scope.row.TwoMonths}}
                        <el-tooltip placement="top-start" effect="dark"	popper-class="atooltip"	>
                            <div slot="content">
　                          　<span style="text-align:left">上月数据</span><br/><span>{{lastMonth.length>0?lastMonth[scope.$index].TwoMonths:''}}</span>　                          
                            </div>
                          	 <span v-show="(scope.row.Category=='财务科目类'||scope.row.Category_Detail=='在制品类')&&lastMonth.length>0&&tableType=='月度库存金额'"><i :class="lastMonth.length>0?compare(scope.row.TwoMonths,lastMonth[scope.$index].TwoMonths):''"></i></span>
                        </el-tooltip>                                       
                  </template>               
              </el-table-column>
              <el-table-column prop="FourMonths" label="4-6个月" width="140" align="right">
                <template slot-scope="scope">
                        {{scope.row.FourMonths}}
                        <el-tooltip placement="top-start" effect="dark"	popper-class="atooltip"	>
                            <div slot="content">
　                          　<span style="text-align:left">上月数据</span><br/><span>{{lastMonth.length>0?lastMonth[scope.$index].FourMonths:''}}</span>　                          
                            </div>
                          	 <span v-show="(scope.row.Category=='财务科目类'||scope.row.Category_Detail=='在制品类')&&lastMonth.length>0&&tableType=='月度库存金额'"><i :class="lastMonth.length>0?compare(scope.row.FourMonths,lastMonth[scope.$index].FourMonths):''"></i></span>
                        </el-tooltip>                                             
                </template>
              </el-table-column>
              <el-table-column prop="SevenMonths" label="7-12个月" width="140" align="right">
                <template slot-scope="scope">
                        {{scope.row.SevenMonths}}
                         <el-tooltip placement="top-start" effect="dark"	popper-class="atooltip"	>
                            <div slot="content">
　                          　<span style="text-align:left">上月数据</span><br/><span>{{lastMonth.length>0?lastMonth[scope.$index].SevenMonths:''}}</span>　                          
                            </div>
                          	 <span v-show="(scope.row.Category=='财务科目类'||scope.row.Category_Detail=='在制品类')&&lastMonth.length>0&&tableType=='月度库存金额'"><i :class="lastMonth.length>0?compare(scope.row.SevenMonths,lastMonth[scope.$index].SevenMonths):''"></i></span>
                         </el-tooltip>
                </template>
              </el-table-column>
              <el-table-column prop="OneYear" label="1-2年" width="140" align="right">
                <template slot-scope="scope">
                        {{scope.row.OneYear}}
                        <el-tooltip placement="top-start" effect="dark"	popper-class="atooltip"	>
                            <div slot="content">
　                          　<span style="text-align:left">上月数据</span><br/><span>{{lastMonth.length>0?lastMonth[scope.$index].OneYear:''}}</span>　                          
                            </div>
                          	 <span v-show="(scope.row.Category=='财务科目类'||scope.row.Category_Detail=='在制品类')&&lastMonth.length>0&&tableType=='月度库存金额'"><i :class="lastMonth.length>0?compare(scope.row.OneYear,lastMonth[scope.$index].OneYear):''"></i></span>
                         </el-tooltip>
                </template>
              </el-table-column>
              <el-table-column prop="TwoYears" label="2-3年" width="140" align="right">
                <template slot-scope="scope">
                        {{scope.row.TwoYears}}
                        <el-tooltip placement="top-start" effect="dark"	popper-class="atooltip"	>
                            <div slot="content">
　                          　<span style="text-align:left">上月数据</span><br/><span>{{lastMonth.length>0?lastMonth[scope.$index].TwoYears:''}}</span>　                          
                            </div>
                          	 <span v-show="(scope.row.Category=='财务科目类'||scope.row.Category_Detail=='在制品类')&&lastMonth.length>0&&tableType=='月度库存金额'"><i :class="lastMonth.length>0?compare(scope.row.TwoYears,lastMonth[scope.$index].TwoYears):''"></i></span>
                         </el-tooltip>
                </template>
              </el-table-column>
              <el-table-column prop="ThreeYears" label="3-5年" width="140" align="right">
                <template slot-scope="scope">
                        {{scope.row.ThreeYears}}               
                        <el-tooltip placement="top-start" effect="dark"	popper-class="atooltip"	>
                            <div slot="content">
　                          　<span style="text-align:left">上月数据</span><br/><span>{{lastMonth.length>0?lastMonth[scope.$index].ThreeYears:''}}</span>　                          
                            </div>
                          	 <span v-show="(scope.row.Category=='财务科目类'||scope.row.Category_Detail=='在制品类')&&lastMonth.length>0&&tableType=='月度库存金额'"><i :class="lastMonth.length>0?compare(scope.row.ThreeYears,lastMonth[scope.$index].ThreeYears):''"></i></span>
                         </el-tooltip>
                </template>
              </el-table-column>
              <el-table-column prop="FiveYears" label="5年以上" width="140" align="right">
                <template slot-scope="scope">
                        {{scope.row.FiveYears}}                       
                        <el-tooltip placement="top-start" effect="dark"	popper-class="atooltip"	>
                            <div slot="content">
　                          　<span style="text-align:left">上月数据</span><br/><span>{{lastMonth.length>0?lastMonth[scope.$index].FiveYears:''}}</span>　                          
                            </div>
                          	 <span v-show="(scope.row.Category=='财务科目类'||scope.row.Category_Detail=='在制品类')&&lastMonth.length>0&&tableType=='月度库存金额'"><i :class="lastMonth.length>0?compare(scope.row.FiveYears,lastMonth[scope.$index].FiveYears):''"></i></span>
                         </el-tooltip>
                </template>
              </el-table-column>
              <el-table-column prop="AmountTotal" label="合计金额" width="140" align="right">
                <template slot-scope="scope">
                        {{scope.row.AmountTotal}}
                        <el-tooltip placement="top-start" effect="dark"	popper-class="atooltip"	>
                            <div slot="content">
　                          　<span style="text-align:left">上月数据</span><br/><span>{{lastMonth.length>0?lastMonth[scope.$index].AmountTotal:''}}</span>　                          
                            </div>
                          	 <span v-show="(scope.row.Category=='财务科目类'||scope.row.Category_Detail=='在制品类')&&lastMonth.length>0&&tableType=='月度库存金额'"><i :class="lastMonth.length>0?compare(scope.row.AmountTotal,lastMonth[scope.$index].AmountTotal):''"></i></span>
                         </el-tooltip>
                </template>
              </el-table-column>
            </el-table-column>
            <el-table-column prop="RecordNum" label="记录条数" width="80" align="right">
            </el-table-column>
            <el-table-column prop="Remark" label="备注" align="center">
            </el-table-column>
        </el-table>
        <span style="font-size:small;">注：每月8号计算出上一个月份的库龄表数据(财务部),每月10号前完成财务核对、PMC核对及财务审核。</span>
    </div>
</template>

<script>
import {getMOnthExcel,exportCategoryReport } from "@/api/order/warehouse";
import config from "@/config/index";
export default {
  props: ["tableType", "time"],
  name: 'sa_table',
  data() {
    return {
      tableDate: [],//当月数据
      lastMonth:[],//上月数据
    };
  },
  components: {},
  computed: {},
  watch: {},
  methods: {
    //对比大小
    compare(month, lastmonth) {
        if (month != null) {
            let numOne = month.replace(/,/g, "");
            let numTwo = lastmonth.replace(/,/g, "")
            let active = '';
            if (Number(numOne) == Number(numTwo)) {
                active = 'el-icon-minus';
            } else if (Number(numOne) > Number(numTwo)) {
                active = 'el-icon-top'
            } else {
                active = 'el-icon-bottom'
            }
            return active;
        }
    },
    tableRowClassName({row,rowIndex}) {
      if (row.FiveYears == "全部合计" || row.FiveYears == "汇总结果求和") {
        return "warning-row";
      }else{
        return "";
      }        
    },
    //下载表格
    LookExcelFile(row) { 
      var dd = new Date(this.time);   
      if(this.tableType=="月度库存金额"){//导出本地文件
        let basurl = this.$loadPath.replace("api/", "");
        getMOnthExcel(row, this.time).then(function(res) {
        let fileName = dd.getFullYear() + '年' + (dd.getMonth() + 1) + '月' + row + '.xlsx'
        let url =basurl+ res.data.Data.Data
        let link = document.createElement('a')
        link.style.display = 'none'     
        link.href = url
        link.setAttribute('download', fileName)
        document.body.appendChild(link)
        link.click()
        });   
      }else{//后台生成文件流
        let obj={outputexcelfile:row,year:dd.getFullYear(),month:dd.getMonth() + 1}
        exportCategoryReport(obj).then(function(res) {
          let fileName = dd.getFullYear() + '年' + (dd.getMonth() + 1) + '月' + row + '.xlsx'
          let url = window.URL.createObjectURL(new Blob([res.data]))
          let link = document.createElement('a')
          link.style.display = 'none'
          link.href = url
          link.setAttribute('download', fileName)
          document.body.appendChild(link)
          link.click()
        });   
      }
    }
  },
  created() {

  },
  mounted() {

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
.el-table {
  font-size: 12px;
  font-family: "微软雅黑";
  color: black;
}
.el-icon-top {
  color: red;
  font-size: 16px;
  font-weight: bold;
}
.el-icon-bottom {
  color: deepskyblue;
  font-size: 16px;
  font-weight: bold;
}
.el-icon-minus {
  color: lightgreen;
  font-weight:800;
}   
.el-table thead.is-group th {
    background: #E4ECF7;
}

</style>