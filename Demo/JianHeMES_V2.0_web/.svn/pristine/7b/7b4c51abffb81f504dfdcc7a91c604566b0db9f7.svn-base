<!--- 产线设备 --->
<template>
  <div>
    <EqHeader :active="active"></EqHeader>
    <el-main class="main-box production-line-box">
      <!-- @* 查询选择框 *@ -->
      <div class="equipment-index-inputcontainer">
        <div class="equipment-index-inputcontainer-item">
          <div>使用部门：</div>
          <el-select
            v-model="selete_department"
            size="mini"
            allow-create
            filterable
            clearable
            placeholder="请选择使用部门"
            style="width: 130px"
          >
            <el-option
              v-for="item in department_options"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            >
            </el-option>
          </el-select>
        </div>
        <el-button type="primary" size="mini" @click="onAddLine"
          >增加产线</el-button
        >
      </div>
      <ProductionLineCard
        :selete_department="selete_department"
        :department_options="department_options"
        :assetNumsList="assetNumsList"
      ></ProductionLineCard>
    </el-main>
    <!-- @* 增加产线弹框 *@ -->
    <el-dialog title="添加产线" :visible.sync="showAddLine" width="30%">
      <el-form label-width="50px" v-model="addSelectedAssetnumsReturnData">
        <el-form-item label="产线">
          <el-input
            size="small"
            v-model="addSelectedAssetnumsReturnData.LineNum"
            style="width: 200px"
          ></el-input>
        </el-form-item>
        <el-form-item label="部门">
          <el-select
            clearable
            filterable
            v-model="addSelectedAssetnumsReturnData.UserDepartment"
            size="small"
            placeholder="请选择"
            style="width: 200px"
          >
            <el-option
              v-for="item in department_options"
              :key="item.value"
              :label="item.value"
              :value="item.value"
            >
            </el-option>
          </el-select>
        </el-form-item>
        <div style="margin-top: 10px">
          <el-button
            size="mini"
            type="success"
            @click="addMachineToCreatingLine"
            >添加设备</el-button
          >
          <el-button
            size="mini"
            type="danger"
            @click="cancelAddMachineToCreatingLine"
            >回退删除</el-button
          >
        </div>
        <div style="display: flex; flex-wrap: wrap">
          <div
            style="display: flex; align-items: center; margin-top: 5px"
            v-for="(item, index) in addlinemachinechioed"
            :key="index"
          >
            <span>序号{{ index + 1 }}&nbsp;&nbsp;&nbsp;</span>
            <el-select
              clearable
              filterable
              v-model="item.test"
              class="addlinemachines"
              size="small"
              placeholder="请选择"
              style="width: 200px"
            >
              <el-option
                v-for="item in assetNumsList"
                :key="item.value"
                :label="item.value"
                :value="item.value"
              >
              </el-option>
            </el-select>
          </div>
        </div>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button size="mini" @click="showAddLine = false">取 消</el-button>
        <el-button
          size="mini"
          type="primary"
          @click="asveAddMachineToCreatingLine"
          >保 存</el-button
        >
      </span>
    </el-dialog>
    
  </div>
</template>

<script>
import EqHeader from "./page-components/_eq_header";
import ProductionLineCard from "./page-components/_production_line_card";
import {
  EQNumberList,
  Userdepartment_list,
  ADDLineNum,
} from "@/api/equipment";
export default {
  name: "Equipment_Production_line",
  props: {},
  inject: ["reload"],
  data() {
    return {
      active: "产线设备",
      selete_department: "",
      department_options: [],
      assetNumsList: [], //资产编号
      // 添加产线
      showAddLine: false,
      addlinemachinechioed: [],
      addSelectedAssetnumsReturnData: {
        LineNum: null,
        EquipmentName: null,
        AssetNumber: null,
        UserDepartment: null,
        Status: null,
        EquipmentNumber: null,
        Section: null,
        StationNum: null,
        Remark: null,
      },
    };
  },
  components: { EqHeader, ProductionLineCard },
  computed: {},
  methods: {
    // 获取所有设备的资产编号
    getAllAssetNums() {
      EQNumberList().then((res) => {
        //console.log(res.data.Data)
        res.data.Data.forEach((item) => {
          let obj = { label: item, value: item };
          this.assetNumsList.push(obj);
        });
      });
    },
    //获取使用部门
    onGetDepartment() {
      Userdepartment_list().then((res) => {
        //console.log(res.data.Data);
        let option = [];
        res.data.Data.forEach((item) => {
          let obj = {
            label: item,
            value: item,
          };
          option.push(obj);
        });
        this.department_options = option;
      });
    },

    //增加产线
    onAddLine() {
      if (this.$limit("添加产线设备")) {
        this.showAddLine = true;
      } else {
        this.$message.warning("暂无权限！");
      }
    },
    //  添加新产线设备
    addMachineToCreatingLine() {
      let obj = { test: null };
      this.addlinemachinechioed.push(obj);
    },
    //  回退删除
    cancelAddMachineToCreatingLine() {
      this.addlinemachinechioed.pop();
    },
    // 新增产线保存提交
    asveAddMachineToCreatingLine() {
      if (this.addlinemachinechioed.length == 0) {
        this.$message.warning("新增产线必须至少存在一台或以上设备！");
      } else {
        let flag = false;
        this.addlinemachinechioed.forEach((item) => {
          if (item.test == null) {
          } else {
            flag = true;
          }
        });
        if (flag) {
          //alert("falg=true")
          let postdata = [];
          this.addlinemachinechioed.forEach((item, index) => {
            //let obj = [(index + 1), item.test]
            let obj2 = { Key: index + 1, Value: item.test };
            postdata.push(obj2);
          });
          //console.log(postdata,000);
          //console.log(JSON.stringify(postdata));
          //console.log(this.addSelectedAssetnumsReturnData)
          //console.log(this.addSelectedAssetnumsReturnData.UserDepartment)
          ADDLineNum({
            usedepartment: this.addSelectedAssetnumsReturnData.UserDepartment,
            lineNum: this.addSelectedAssetnumsReturnData.LineNum,
            equipmentNumberlist: JSON.stringify(postdata),
          })
            .then((res) => {
              //console.log(res.data.Data)
              if (res.data.Result) {
                this.$message.success(res.data.Message);
                this.showAddLine = false;
                this.reload();
              } else {
                this.$message.error(res.data.Message);
              }
            })
            .catch((err) => {
              console.log(err);
            });
        } else {
          this.$message.warning("请补全设备信息");
        }
      }
    },
    
  },
  created() {},
  mounted() {
    //获取地址传参
    let urlSearchParam = this.$route.query;
    if (urlSearchParam.paramData == undefined) {
      this.selete_department = this.$userInfo.Department;
    } else {
      this.selete_department = urlSearchParam.paramData;
    }
    this.onGetDepartment();
    this.getAllAssetNums();
  },
};
</script>

<style lang='less' scoped>
@import url("./page-components/equipment.less");
.production-line-box {
  min-width: 1200px;
  overflow: auto;
}
</style>