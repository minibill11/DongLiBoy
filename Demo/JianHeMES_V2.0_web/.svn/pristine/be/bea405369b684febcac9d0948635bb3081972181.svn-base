<!---  --->
<template>
  <div>
      <el-table   border
                  :data="tableData"
                  size="mini"
                  height="640px" stripe 
                  :header-cell-style="{'text-align':'center'}"
                  :cell-style="{'text-align':'center'}"
                  >
            <el-table-column type="index" 
                             width="50"
                             label="序号" 
                             fixed>
            </el-table-column>
            <el-table-column prop="Department"
                             label="部门"
                             width="100" 
                             fixed>
            </el-table-column>
            <el-table-column prop="Position"
                             show-overflow-tooltip
                             label="位置" 
                             fixed>
            </el-table-column>
            <el-table-column prop="District"
                             label="区域"
                             width="60" 
                             fixed>
            </el-table-column>
            <el-table-column :label="check_name" min-width="200" v-if="!show">
                <el-table-column v-for="(item, index) in tableColumn"
                                 :key="index+Math.random()"
                                 :prop="item.field"
                                 :label="item.title"
                                 width="42">
                    <template slot-scope="scope">
                        <div class="tickoff" @click="onToDetail(scope.row,item.title)">{{scope.row[item.field]=='true'?'√':''}}</div>
                    </template>
                </el-table-column>
            </el-table-column>
            <el-table-column :label="check_name" min-width="200" v-if="show">
                <el-table-column v-for="(item, index) in tableColumn"
                                 :key="index+Math.random()"
                                 :prop="item.field"
                                 :label="item.title"
                                 :width="item.width" fixed="center" sortable>
                    <template slot-scope="scope">
                        <div v-if="select_check_type!='周检'&&getRowData(scope.row,item.field)!=0" :class="[$limit('查看7S班组扣分详细表')?'col-red':'']" @click="onSelectData(scope.row,0,item.title)">{{getRowData(scope.row,item.field)}}</div>
                        <div v-if="select_check_type=='周检'&&getRowData(scope.row,item.field)!=0" :class="[$limit('查看7S班组扣分详细表')?'col-red':'']" @click="onSelectData(scope.row,2,item.title)">{{getRowData(scope.row,item.field)}}</div>
                    </template>
                </el-table-column>
            </el-table-column>
            <el-table-column prop="GradeSum"
                             label="扣分合计"
                             width="80" fixed="right" sortable v-if="show">
                <template slot-scope="scope">
                    <div :class="[$limit('查看7S班组扣分详细表')?'col-red':'']" v-if="scope.row.GradeSum!=''" @click="onSelectData(scope.row,1)">-{{scope.row.GradeSum}}</div>
                </template>
            </el-table-column>
        </el-table>
  </div>
</template>

<script>
export default {
  name: 'kpi_table',
  props: ["source","select_month","show","select_check_type"],
  data() {
    return {
        tableData:[],
        check_name: '检查扣分',
        tableColumn: [],//表头
    };
  },
  components: {},
  computed: {},
  watch: {},
  methods: {  
    //日期表头改变
    onChangeTitle() {
        let month = new Date(this.select_month).getMonth() + 1;
        this.check_name = '检查扣分（' + month + '月)'
    },
    getRowData(row, val) {
        for (let i in row) {
            if (i == val) {
                if (row[i] != '') {
                    return -row[i];
                }
            }
        }
    },
    //选择分数查询
    onSelectData(row, num, val) {
        if (this.$limit('查看7S班组扣分详细表')) {
            let date = new Date(this.select_month);
            let query_type;
            let week = '';
            if (num == 0) {
                query_type = '日';
                let dd = parseInt(val.replace("日", ""));
                date.setDate(dd);
            } else if (num == 1) {
                query_type = '月';
            } else if (num == 2) {
                query_type = '周';
                week = val.substring(1, 2);
            }
            this.$router.push({
                name:'KPI_7S_Detail',
                params:{
                    check_type:this.select_check_type,
                    department:row.Department,
                    position:row.Position,
                    district:row.District,
                    date:date,
                    query_type:query_type,
                    week:week                    
                }             
            })
            //window.location.href = "/KPI/KPI_7S_Detail?check_type=" + this.select_check_type + "&department=" + row.Department + "&position=" + row.Position + "&district=" +
            // row.District + "&date=" + date + "&query_type=" + query_type + "&week=" + week;
        }
    },
    //跳转详细
    onToDetail(row, val) {
        let dd = new Date(this.select_month);
        let num = parseInt(val);
        dd.setDate(num);
        this.$router.push({
            name:'KPI_7S_Detail',
            params:{
                check_type:'日检',
                department:row.Department,
                position:row.Position,
                district:row.District,
                date:dd,
                query_type:'日',
                week:''
            }
        })
        //console.log(dd);
        //window.open("/KPI/KPI_7S_Detail?check_type=日检&department=" + row.Department + "&position=" + row.Position + "&district=" + row.District + "&date=" + dd + "&query_type=日&week=");
    },
  },
  created() {},
  mounted() {
      if(this.select_month!=null){
          this.onChangeTitle()
      }
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

<style lang='less'>
.col-blue {
    background-color: #409eff !important;
    color: #fff;
}
.el-table thead.is-group th {
    background: #E4ECF7;
}
.tickoff{
    font-size: 20px;
    color: #2bcfaa;
    cursor: pointer;
}
.col-red {
    color: red;
    cursor: pointer;
}
    .col-red:hover {
        text-decoration: underline;
    }
</style>