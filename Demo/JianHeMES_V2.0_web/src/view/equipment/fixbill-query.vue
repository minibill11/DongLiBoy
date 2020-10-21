<!--- 报修单查询 --->
<template>
  <div v-cloak>
    <eqHeader :active="active"></eqHeader>
    <el-main class="main-box">
      <div class="topContainer">
        <!-- @* 查询选择 *@ -->
        <div>
          <span class="selectText">设备编号：</span><br />
          <el-select
            v-model="ENunber"
            size="mini"
            clearable
            filterable
            placeholder="请选择设备编号"
            style="width: 150px"
          >
            <el-option
              v-for="item in options"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            >
            </el-option>
          </el-select>
        </div>
        <div>
          <span class="selectText">使用部门：</span><br />
          <el-select
            v-model="userdepart"
            size="mini"
            clearable
            filterable
            placeholder="请选择使用部门"
            style="width: 150px"
          >
            <el-option
              v-for="item in deparlist"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            >
            </el-option>
          </el-select>
        </div>
        <div>
          <span class="selectText">设备名称：</span><br />
          <el-input
            v-model="EName"
            size="mini"
            placeholder="设备名称"
            style="width: 150px"
          ></el-input>
        </div>
        <div>
          <span class="selectText">按故障日期查询：</span><br />
          <el-date-picker
            v-model="fualtTime"
            type="date"
            value-format="yyyy-MM-dd HH:mm:ss"
            :disabled="fualtTimeFlag"
            size="mini"
            style="width: 150px"
            placeholder="故障日期"
          >
          </el-date-picker>
        </div>
        <div>
          <span class="selectText">按时间段查询：</span><br />
          <el-date-picker
            v-model="TimeRange"
            type="daterange"
            size="mini"
            style="width: 360px"
            :disabled="fualtTimeRangeFlag"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
          >
          </el-date-picker>
        </div>
        <div>
          <span class="selectText">按年查询已维修：</span><br />
          <el-date-picker
            v-model="yearSamry"
            align="right"
            size="mini"
            type="year"
            style="width: 150px"
            :disabled="yearSamryFlag"
            placeholder="选择年"
          >
          </el-date-picker>
        </div>
        <div>
          <span class="selectText">按月查询已维修：</span><br />
          <el-select
            v-model="monthSamry"
            size="mini"
            :disabled="monthSamryFlag"
            clearable
            filterable
            placeholder="请选择月"
            style="width: 150px"
          >
            <el-option
              v-for="item in 12"
              :key="item"
              :label="item"
              :value="item"
            >
            </el-option>
          </el-select>
        </div>
        <div>
          <el-button
            type="primary"
            size="mini"
            @click="query"
            :disabled="showQuery"
            >查询</el-button
          >
        </div>
      </div>
      <div class="tip-box">
        <span class="tip"
          >提示：查询结果中部门（因组织架构变更）可能与当前部门不一致。</span
        >
      </div>
      <!-- @* 查询结果表格 *@ -->
      <div class="bottomContainer">
        <el-table :data="tableData" max-height="600" border style="width: 100%">
          <el-table-column prop="index" label="序号" width="50">
          </el-table-column>
          <el-table-column
            prop="EquipmentNumber"
            label="设备编号"
            sortable
            width="120"
          >
          </el-table-column>
          <el-table-column
            prop="EquipmentName"
            label="设备名称"
            sortable
            width="120"
          >
          </el-table-column>
          <el-table-column prop="FaultTime" label="故障时间" width="120">
          </el-table-column>
          <el-table-column prop="Emergency" label="紧急状态" width="120">
          </el-table-column>
          <el-table-column prop="FauDescription" label="报修内容">
          </el-table-column>
          <el-table-column label="报修状态">
            <template slot-scope="scope">
              <div style="text-align: left">
                <div>{{ scope.row.State ? scope.row.State : "" }}</div>
                <div>{{ scope.row.State1 ? scope.row.State1 : "" }}</div>
                <div>{{ scope.row.State2 ? scope.row.State2 : "" }}</div>
              </div>
            </template>
          </el-table-column>
          <el-table-column label="要求完成时间" width="150">
            <template slot-scope="scope">
              <span v-if="scope.row.RequirementsTime != null">{{
                scope.row.RequirementsTime | formatTime
              }}</span>
            </template>
          </el-table-column>
          <el-table-column prop="StateRepair" label="报修结果" width="100" sortable>
          </el-table-column>
          <el-table-column label="操作" width="100">
            <template slot-scope="scope">
              <el-button
                type="text"
                size="mini"
                @click="showDetials(scope.row)"
                style="text-decoration: underline"
                >详细</el-button
              >
            </template>
          </el-table-column>
        </el-table>
      </div>
    </el-main>
  </div>
</template>

<script>
import eqHeader from "./page-components/_eq_header";
import {
  InstrumentList,
  Deparlist,
  EquipmentRepairbill_Query,
} from "@/api/equipment";
export default {
  name: "Equipment_Fixbill_Query",
  props: {},
  data() {
    return {
      active: "报修单查询",
      showQuery: true,
      EName: null,
      ENunber: null,
      fualtTime: null,
      TimeRange: [],
      userdepart: null,
      options: [],
      fualtTimeFlag: false,
      fualtTimeRangeFlag: false,
      yearSamryFlag: false,
      monthSamryFlag: false,
      tableData: [],
      deparlist: [],
      yearSamry: null,
      monthSamry: null,
    };
  },
  components: { eqHeader },
  computed: {},
  watch: {
    // 监听故障时间
    fualtTime() {
      if (this.fualtTime != null) {
        this.fualtTimeRangeFlag = true;
        this.yearSamryFlag = true;
        this.monthSamryFlag = true;
      } else {
        this.fualtTimeRangeFlag = false;
        this.yearSamryFlag = false;
        this.monthSamryFlag = false;
      }
    },
    //监听所选时间段
    TimeRange() {
      if (this.TimeRange != null) {
        this.fualtTimeFlag = true;
        this.yearSamryFlag = true;
        this.monthSamryFlag = true;
      } else {
        this.fualtTimeFlag = false;
        this.yearSamryFlag = false;
        this.monthSamryFlag = false;
      }
    },
    // 监听年
    yearSamry() {
      if (this.yearSamry != null) {
        this.fualtTimeRangeFlag = true;
        this.fualtTimeFlag = true;
        //this.monthSamryFlag = true
      } else {
        this.fualtTimeRangeFlag = false;
        this.fualtTimeFlag = false;
        //this.monthSamryFlag = false
      }
    },
    // 监听月
    monthSamry() {
      if (this.monthSamry != null && this.monthSamry != "") {
        this.fualtTimeRangeFlag = true;
        //this.yearSamryFlag = true
        this.fualtTimeFlag = true;
      } else {
        this.fualtTimeRangeFlag = false;
        //this.yearSamryFlag = false
        this.fualtTimeFlag = false;
      }
    },
  },
  methods: {
    // 页面加载时获取所有可选设备编号列表以及使用部门列表
    initData() {
      InstrumentList()
        .then((res) => {
          res.data.Data.forEach((item) => {
            let obj = { value: item, label: item };
            this.options.push(obj);
          });
        })
        .catch((err) => {
          this.$message.warning("获取设备编号列表失败");
        });
      Deparlist()
        .then((res) => {
          res.data.Data.forEach((item) => {
            let obj = { value: item, label: item };
            this.deparlist.push(obj);
          });
        })
        .catch((err) => {
          this.$message.warning("获取设备使用部门列表失败");
        });
    },
    // 查询
    query() {
      if (
        this.EName != null ||
        this.ENunber != null ||
        this.fualtTime != null ||
        this.TimeRange != null ||
        this.userdepart != null ||
        this.yearSamry != null ||
        (this.monthSamry != null && this.monthSamry != "")
      ) {
        let year = null;
        if (this.yearSamry != null) {
          let dd = new Date(this.yearSamry);
          year = dd.getFullYear();
        }
        let param = {
          month: this.monthSamry,
          year: year,
          userdepartment: this.userdepart,
          equnumber: this.ENunber,
          equipname: this.EName,
          date: this.fualtTime,
          starttime: this.TimeRange != null ? this.TimeRange[0] : null,
          endtime: this.TimeRange != null ? this.TimeRange[1] : null,
        };
        EquipmentRepairbill_Query(param)
          .then((res) => {
            if (res.data.Data.length > 0) {
              this.tableData = res.data.Data;
              // console.log(this.tableData, 999999999);
            } else {
              this.$message.warning("暂无数据");
            }
          })
          .catch((err) => {
            console.log(err);
            this.$message.error("查询失败");
          });
      } else {
        this.$message.warning("请补全需要查询的信息");
      }
    },
    // 详细按钮跳转传参
    showDetials(item) {
      //console.log(item);
      this.$router.push(
        {name: 'Equipment_Fixbill_Deatil',
        query:{ EquipmentNumber:item.EquipmentNumber,
        EquipmentName:item.EquipmentName,
        time:item.FaultTime,
        canchange:'false'
        }});
      // window.open(
      //   "/Equipment/Equipment_Fixbill_Detail?EquipmentNumber=" +
      //     item.EquipmentNumber +
      //     "&EquipmentName=" +
      //     item.EquipmentName +
      //     "&time=" +
      //     item.FaultTime +
      //     "&canchange=false"
      // );
    },
  },
  created() {},
  mounted() {
    if (this.$limit("查看报修单")) {
      this.showQuery = false;
    } else {
      this.showQuery = true;
    }
    this.initData();
    //默认时间段
    let dd = new Date();
    let y = dd.getFullYear();
    let startTime = new Date(`${y}-01-01`);
    this.TimeRange.push(startTime, dd);
    this.query();
  }
};
</script>

<style lang='less' scoped>
@import url("~@/assets/style/color.less");
@import url("./page-components/equipment.less");
[v-cloak] {
  display: none !important;
}
.topContainer {
  width: 100%;
  align-items: flex-end;
  display: flex;
  justify-content: center;
  flex-flow: row wrap;
  margin-bottom: 7px;
  font-size: 14px;
}

.topContainer div {
  margin-right: 5px;
}
</style>