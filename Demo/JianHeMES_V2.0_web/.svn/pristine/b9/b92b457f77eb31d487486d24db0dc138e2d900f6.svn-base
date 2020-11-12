<!--- 运行状态子表 --->
<template>
  <div>
    <!-- @*设备稼动率汇总表*@ -->
    <div v-if="selectedTitle == '设备稼动率汇总'">
      <el-table v-bind:data="currentTable" border stripe>
        <el-table-column type="index" width="50" label="序号"> </el-table-column>
        <el-table-column prop="UserDepartment" label="使用部门">
        </el-table-column>
        <el-table-column prop="RunnDevice" label="运行时长"> </el-table-column>
        <el-table-column prop="RunDepar_Device" label="运行占比">
        </el-table-column>
        <el-table-column prop="StopnDevice" label="停机时长"> </el-table-column>
        <el-table-column prop="StopDepar_Device" label="停机占比">
        </el-table-column>
        <el-table-column prop="MaintDevice" label="维修时长"> </el-table-column>
        <el-table-column prop="MaintDepar_Device" label="维修占比">
        </el-table-column>
        <el-table-column prop="TallyDevice" label="保养时长"> </el-table-column>
        <el-table-column prop="TallyDepar_Device" label="保养占比">
        </el-table-column>
        <el-table-column label="操作">
          <template slot-scope="scope">
            <el-button
              size="mini"
              type="text"
              style="text-decoration: underline"
              @@click="onToTurn(scope.row)"
              >详细</el-button
            >
          </template>
        </el-table-column>
      </el-table>
    </div>
    <!-- @*设备状态汇总表*@ -->
    <div v-else-if="selectedTitle == '设备状态汇总'">
      <el-table v-bind:data="factoryCurrentTable" border stripe>
        <el-table-column type="index" width="50" label="序号"> </el-table-column>
        <el-table-column prop="UserDepartment" label="使用部门">
        </el-table-column>
        <el-table-column prop="Runnumber" label="运行数量"> </el-table-column>
        <el-table-column prop="Run_Depar" label="运行占比"> </el-table-column>
        <el-table-column prop="Stopnumber" label="停机数量"> </el-table-column>
        <el-table-column prop="Stop_Depar" label="停机占比"> </el-table-column>
        <el-table-column prop="Maintnumber" label="维修数量"> </el-table-column>
        <el-table-column prop="Maint_Depar" label="维修占比"> </el-table-column>
        <el-table-column v-show="false" prop="Maintnumber" label="维修数量">
        </el-table-column>
        <el-table-column v-show="false" prop="Maint_Depar" label="维修占比">
        </el-table-column>
        <el-table-column label="操作">
          <template slot-scope="scope">
            <el-button
              size="mini"
              type="text"
              style="text-decoration: underline"
              @@click="onToTurn(scope.row)"
              >详细</el-button
            >
          </template>
        </el-table-column>
      </el-table>
    </div>
    <!-- @*设备保养汇总表*@ -->
    <div v-else-if="selectedTitle == '设备保养汇总'">
      <el-table v-bind:data="maintenanceTable" border stripe>
        <el-table-column type="index" width="50" label="序号"> </el-table-column>
        <el-table-column prop="UserDepartment" label="使用部门">
        </el-table-column>
        <el-table-column prop="Department_List" label="全厂所有设备台数">
        </el-table-column>
        <el-table-column prop="DepartmentNum" label="本月保养设备台数">
        </el-table-column>
        <el-table-column prop="Depart_HaveMaintain" label="已保养">
        </el-table-column>
        <el-table-column prop="Depart_HavePercent" label="已保养占比">
        </el-table-column>
        <el-table-column prop="Depart_ToMaintain" label="计划保养">
        </el-table-column>
        <el-table-column prop="Depart_ToPercent" label="计划保养占比">
        </el-table-column>
        <el-table-column
          v-show="false"
          prop="Depart_ToMaintain"
          label="计划保养"
        >
        </el-table-column>
        <el-table-column
          v-show="false"
          prop="Depart_ToPercent"
          label="计划保养占比"
        >
        </el-table-column>
        <el-table-column label="操作">
          <template slot-scope="scope">
            <el-button
              size="mini"
              type="text"
              style="text-decoration: underline"
              @@click="onToTurn(scope.row)"
              >详细</el-button
            >
          </template>
        </el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
export default {
  name: "",
  props: {
    show: false,
    selectedTitle: { type: String, default: "设备稼动率汇总" },
    table: Object,
  },
  data() {
    return {
      currentTable: [], //设备稼动率子表
      factoryCurrentTable: [], //设备状态子表
      maintenanceTable: [], //设备保养子表
    };
  },
  components: {},
  computed: {},
  watch: {
    table(val) {
      // console.log(val);
      this.initTable();
    },
  },
  methods: {
    //表数据
    initTable() {
      //console.log(this.selectedTitle, 000);
      //console.log(this.table, 888)
      this.currentTable = this.table.current;
      this.factoryCurrentTable = this.table.Factory_current;
      this.maintenanceTable = this.table.DepartmentList;
      //console.log(this.maintenanceTable)
    },
    //跳转
    onToTurn(row) {
      let date = new Date();
      //console.log(row);
      if (this.selectedTitle == "设备稼动率汇总") {
        //console.log('99');
        //console.log(row.UserDepartment, 888888)
        window.open("/Equipment/EmployTime?paramData=" + row.UserDepartment);
      }
      if (this.selectedTitle == "设备状态汇总") {
        //console.log('88');
        window.open(
          "/Equipment/Equipment_Production_line?paramData=" + row.UserDepartment
        );
      }
      if (this.selectedTitle == "设备保养汇总") {
        //console.log('77');
        window.open(
          "/Equipment/Equipment_Maintenance_Summary?paramData=" +
            row.UserDepartment +
            "&date=" +
            date
        );
      }
    },
  },
  created() {},
  mounted() {},
};
</script>

<style lang='less' scoped>
@import url("~@/assets/style/color.less");
</style>