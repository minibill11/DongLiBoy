<!--- 扫码点检 --->
<template>
  <div id="scan">
    <div class="eq-header">扫码点检</div>
    <!-- @*表单*@ -->
    <div class="form-box">
      <el-form
        :inline="true"
        :model="formInfo"
        label-position="left"
        label-width="90px"
      >
        <el-form-item label="点检周期：">
          <el-select
            size="small"
            v-model="formInfo.CheckType"
            style="width: 200px"
            placeholder="请选择.."
          >
            <el-option
              v-if="$limit('日点检保养人确认')"
              label="日点检"
              value="日点检"
            ></el-option>
            <el-option
              v-if="$limit('周保养人确认')"
              label="周点检"
              value="周点检"
            ></el-option>
            <el-option
              v-if="$limit('月保养人确认')"
              label="月点检"
              value="月点检"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="设备编号：">
          <div class="el-input el-input--medium el-input--suffix">
            <input
              v-model.lazy="formInfo.EquipmentNumber"
              style="width: 200px; height: 32px"
              v-on:keyup.enter="scanKeyup($event)"
              autocomplete="off"
              placeholder="请选择设备编号"
              class="el-input__inner"
            />
          </div>
        </el-form-item>
        <el-form-item label="设备名称：">
          <el-input
            size="small"
            v-model="formInfo.EquipmentName"
            placeholder="请选择设备名称"
            disabled
            style="width: 200px"
          >
          </el-input>
        </el-form-item>
        <el-form-item label="使用部门：">
          <el-input
            size="small"
            v-model="formInfo.UserDepartment"
            placeholder="请选择使用部门"
            disabled
            style="width: 200px"
          >
          </el-input>
        </el-form-item>
        <el-form-item label="线别：">
          <el-input
            size="small"
            v-model="formInfo.LineName"
            placeholder="请选择线别"
            disabled
            style="width: 200px"
          >
          </el-input>
        </el-form-item>
        <el-form-item label="日期：">
          <el-date-picker
            size="small"
            v-model="formInfo.Date"
            type="date"
            placeholder="请选择日期" value-format="yyyy-MM-dd HH:mm:ss"
            disabled
            style="width: 200px"
          >
          </el-date-picker>
        </el-form-item>

        <!-- @*TD-008-D版本表*@ -->
        <el-table
          v-if="
            !isCheck &&
            formInfo.EquipmentNumber != '' &&
            VersionNum == 'TD-008-D'
          "
          v-loading="loading"
          :data="tableData"
          style="width: 100%"
          border
          stripe
          @header-click="onCheck"
          :cell-style="cellStyle"
          :span-method="onSpanMethod"
        >
          <el-table-column prop="project" label="项目"> </el-table-column>
          <el-table-column prop="operation" label="操作方法"> </el-table-column>
          <el-table-column label="点检">
            <template slot-scope="scope">
              <div v-if="formInfo.CheckType == '日点检'">
                <el-select
                  size="mini"
                  v-model="scope.row.mainte"
                  placeholder="请选择..."
                >
                  <el-option
                    v-for="item in day_options"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                  >
                  </el-option>
                </el-select>
              </div>
              <div
                v-if="
                  formInfo.CheckType == '周点检' ||
                  formInfo.CheckType == '月点检'
                "
              >
                <el-input
                  size="mini"
                  type="textarea"
                  autosize
                  v-model="scope.row.mainte"
                  placeholder="请输入内容..."
                >
                </el-input>
              </div>
            </template>
          </el-table-column>
        </el-table>
        <!-- @*TD-008-E版本表*@ -->
        <el-table
          v-if="
            !isCheck &&
            formInfo.EquipmentNumber != '' &&
            VersionNum == 'TD-008-E'
          "
          v-loading="loading"
          :data="tableData"
          style="width: 100%"
          border
          stripe
          @header-click="onCheck"
          :cell-style="cellStyle"
        >
          <el-table-column prop="project" label="项目"> </el-table-column>
          <el-table-column prop="operation" label="操作方法"> </el-table-column>
          <el-table-column label="点检">
            <template slot-scope="scope">
              <div v-if="formInfo.CheckType == '日点检'">
                <el-select
                  size="mini"
                  v-model="scope.row.mainte"
                  placeholder="请选择..."
                >
                  <el-option
                    v-for="item in day_options"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                  >
                  </el-option>
                </el-select>
              </div>
              <div
                v-if="
                  formInfo.CheckType == '周点检' ||
                  formInfo.CheckType == '月点检'
                "
              >
                <el-radio-group
                  style="
                    display: flex;
                    flex-direction: column;
                    align-items: flex-start;
                  "
                  v-model="scope.row.status"
                  @change="isNormal(scope.row, scope.$index)"
                >
                  <el-radio label="正常">正常</el-radio>
                  <el-radio label="异常">异常</el-radio>
                </el-radio-group>
                <el-input
                  size="mini"
                  type="textarea"
                  v-model="scope.row.mainte"
                  placeholder="请输入内容..."
                >
                </el-input>
              </div>
            </template>
          </el-table-column>
        </el-table>
        <!-- @*提示*@ -->
        <div class="tip" v-if="isCheck && formInfo.EquipmentNumber != ''">
          {{ tip }}
        </div>
        <div
          style="margin-top: 10px"
          v-if="!isCheck && formInfo.EquipmentNumber != ''"
        >
          <el-form-item label="点检人：">
            <el-button size="small" class="success-btn" @click="onSave"
              >确认</el-button
            >
          </el-form-item>
        </div>
      </el-form>
    </div>
    <!-- @*显示点检*@ -->
    <el-card
      shadow="never"
      class="card-never"
      v-if="TallyNum != 0 && formInfo.CheckType == '日点检'"
    >
      <div slot="header" class="clearfix">
        <span
          >{{ formInfo.UserDepartment
          }}{{ formInfo.LineName }}今天未点检设备数：{{ TallyNum }}</span
        >
      </div>
      <div
        v-for="(item, index) in TallyList"
        :key="index"
        class="text item"
        @click="onToTurn(item)"
      >
        {{ item.EquipmentNumber }}&nbsp;&nbsp; {{ item.EquipmentName }}
      </div>
    </el-card>
    <el-card
      shadow="never"
      class="card-never"
      v-if="
        TallyAll != 0 &&
        formInfo.UserDepartment != 'SMT部' &&
        formInfo.CheckType == '日点检'
      "
    >
      <div slot="header" class="clearfix">
        <span
          >{{ formInfo.UserDepartment }}今天未点检设备数：{{ TallyAll }}</span
        >
      </div>
      <div
        v-for="(item, index) in TallyListAll"
        :key="index"
        class="text item"
        @click="onToTurn(item)"
      >
        {{ item.EquipmentNumber }}&nbsp;&nbsp; {{ item.EquipmentName }}
      </div>
    </el-card>
    <el-card
      shadow="never"
      class="card-have"
      v-if="HaveTallyNum != 0 && formInfo.CheckType == '日点检'"
    >
      <div slot="header" class="clearfix">
        <span
          >{{ formInfo.UserDepartment
          }}{{ formInfo.LineName }}今天已点检设备数：{{ HaveTallyNum }}</span
        >
      </div>
      <div
        v-for="(item, index) in HaveTallyList"
        :key="index"
        class="text item"
        @click="onToTurn(item)"
      >
        {{ item.EquipmentNumber }}&nbsp;&nbsp; {{ item.EquipmentName }}
      </div>
    </el-card>
  </div>
</template>

<script>
import {
  Tally_ScanCode,
  Checked_maintenance,
  Save_TallyData,
} from "@/api/equipment";
export default {
  name: "Equipment_ScanCode_Check",
  props: {},
  data() {
    return {
      loading: false,
      day_options: [
        {
          label: " ",
          value: 0,
        },
        {
          label: "√",
          value: 1,
        },
        {
          label: "×",
          value: 2,
        },
        {
          label: "/",
          value: 3,
        },
      ],
      //提交筛选
      formInfo: {
        CheckType: "",
        EquipmentNumber: "",
        EquipmentName: "",
        UserDepartment: "",
        LineName: "",
        Date: new Date(),
        //Date: '2020-12-16',
      },
      tableData: [],
      submitObj: {},
      isCheck: false,
      VersionNum: "", //版本号
      tip: "", //提示
      HaveTallyNum: 0, //已点检数
      HaveTallyList: [], //已点检列表
      TallyNum: 0, //未点检数-线别
      TallyList: [], //未点检列表-线别
      TallyAll: 0, //未点检数
      TallyListAll: [], //未点检列表
    };
  },
  components: {},
  computed: {},
  watch: {
    "formInfo.CheckType": {
      handler() {
        if (this.formInfo.EquipmentNumber.length >= 5) {
          this.onGetScanCheck();
        }
      },
      deep: true,
    },
    "formInfo.EquipmentNumber": {
      handler() {
        if (this.formInfo.CheckType != "") {
          this.onGetScanCheck();
        }
      },
      deep: true,
    },
  },
  methods: {
    scanKeyup(v) {
      console.log(v, 555);
      if (v.target.value == "") {
        v.target.focus();
      } else {
        v.target.select();
        this.onGetScanCheck();
      }
    },
    //合并行
    onSpanMethod({ columnIndex }) {
      //console.log(this.tableData.length);
      if (this.formInfo.CheckType == "周点检") {
        if (columnIndex == 2) {
          return {
            rowspan: this.tableData.length,
            colspan: 1,
          };
        }
      }
    },
    //点击日点检表头选择勾选全部
    onCheck(column) {
      if (this.formInfo.CheckType == "日点检" && column.label == "点检") {
        //console.log(this.tableData)
        this.tableData.forEach((item) => {
          item.mainte = 1;
        });
      }
    },
    //日点检背景色
    cellStyle({ columnIndex }) {
      if (this.formInfo.CheckType == "日点检") {
        if (columnIndex === 2) {
          // 指定列号
          return "background:#e6f8ea";
        }
      }
    },
    //根据设备编号获取扫码点检信息
    onGetScanCheck() {
      this.loading = true;
      Tally_ScanCode({
          equipmentNumber: this.formInfo.EquipmentNumber,
          time: this.formInfo.Date,
          type: this.formInfo.CheckType,
        })
        .then((res) => {
          //console.log(res.data.Data,888);
          //console.log(res.data.Data.Feg, '2222');
          if (res.data.Result) {
            this.isCheck = false;
            this.VersionNum = res.data.Data.VersionNum;
            this.formInfo.EquipmentName = res.data.Data.EquipmentName;
            this.formInfo.UserDepartment = res.data.Data.UserDepartment;
            this.formInfo.LineName = res.data.Data.LineName;
            this.onProcessData(res.data.Data);
            this.loading = false;
            this.onGetIsCheck();
          } else {
            this.isCheck = true;
            this.tip = res.data.Data;
            this.formInfo.EquipmentName = res.data.Data.EquipmentName;
            this.formInfo.UserDepartment = res.data.Data.UserDepartment;
            this.formInfo.LineName = res.data.Data.LineName;
            this.onGetIsCheck();
          }
        });
    },
    //根据部门查找当天是否已点检的设备
    onGetIsCheck() {
      //console.log(this.formInfo.UserDepartment,111);
      if (this.formInfo.UserDepartment == "SMT部") {
        Checked_maintenance({
            userdepartment: this.formInfo.UserDepartment,
            lineNum: this.formInfo.LineName,
            time: this.formInfo.Date,
          })
          .then((res) => {
            //console.log(res);
            this.TallyList = res.data.Data.Retul;
            this.HaveTallyList = res.data.Data.Have_retul;
            this.TallyNum = res.data.Data.Number ? res.data.Data.Number : 0;
            this.HaveTallyNum = res.data.Data.HaveTally ? res.data.Data.HaveTally : 0;
          });
      } else {
       Checked_maintenance({
            userdepartment: this.formInfo.UserDepartment,
            time: this.formInfo.Date,
          })
          .then((res) => {
            //console.log(res);
            let arr = [],
              num = 0;
            if (res.data.Data.Retul) {
              res.data.Data.Retul.forEach((item) => {
                if (item.LineName == this.formInfo.LineName) {
                  arr.push(item);
                  num += 1;
                }
              });
              this.TallyList = arr;
              this.TallyNum = num;
            }
            this.TallyListAll = res.data.Data.Retul;
            this.HaveTallyList = res.data.Data.Have_retul;
            this.TallyAll = res.data.Data.Number ? res.data.Data.Number : 0;
            this.HaveTallyNum = res.data.Data.HaveTally ? res.data.Data.HaveTally : 0;
          });
      }
    },
    //根据当前日期判断第几周
    onGetWeek() {
      let week;
      let dd = new Date(this.formInfo.Date).getDate();
      //判断当前日期为当月第几周
      if (dd > 0 && dd <= 8) {
        week = 1;
      } else if (dd > 8 && dd <= 16) {
        week = 2;
      } else if (dd > 16 && dd <= 24) {
        week = 3;
      } else if (dd > 24 && dd <= 32) {
        week = 4;
      }
      return week;
    },
    //处理数据
    onProcessData(data) {
      let arr = [];
      let letter_list = [
        { id: 1, letter: "A" },
        { id: 2, letter: "B" },
        { id: 3, letter: "C" },
        { id: 4, letter: "D" },
        { id: 5, letter: "E" },
        { id: 6, letter: "F" },
        { id: 7, letter: "G" },
        { id: 8, letter: "H" },
        { id: 9, letter: "I" },
        { id: 10, letter: "J" },
        { id: 11, letter: "K" },
      ];
      let obj;
      let dd = new Date(this.formInfo.Date).getDate();
      let week = this.onGetWeek();
      letter_list.forEach((item) => {
        if (this.formInfo.CheckType == "日点检") {
          //obj = {
          //    project: data[`Day_project${item.id}`],
          //    operation: data[`Day_opera${item.id}`],
          //    mainte: data[`Day_${item.letter}_${dd}`]
          //}
          key1 = `Day_project${item.id}`;
          key2 = `Day_opera${item.id}`;
          key3 = `Day_${item.letter}_${dd}`;
        }
        if (
          this.formInfo.CheckType == "周点检" &&
          this.VersionNum == "TD-008-D"
        ) {
          //obj = {
          //    project: data[`Week_Check${item.id}`],
          //    operation: data[`Week_Inspe${item.id}`],
          //    mainte: data[`Week_${week}`],
          //    status: '异常'
          //}
          key1 = `Week_Check${item.id}`;
          key2 = `Week_Inspe${item.id}`;
          key3 = `Week_${week}`;
        }
        if (
          this.formInfo.CheckType == "周点检" &&
          this.VersionNum == "TD-008-E"
        ) {
          //obj = {
          //    project: data[`Week_Check${item.id}`],
          //    operation: data[`Week_Inspe${item.id}`],
          //    mainte: data[`Week${week}_mainten${item.id}`],
          //    status: '异常'
          //}
          key1 = `Week_Check${item.id}`;
          key2 = `Week_Inspe${item.id}`;
          key3 = `Week${week}_mainten${item.id}`;
        }
        if (this.formInfo.CheckType == "月点检") {
          //obj = {
          //    project: data[`Month_Project${item.id}`],
          //    operation: data[`Month_Approach${item.id}`],
          //    mainte: data[`Month_${item.id}`]，
          //    status: '异常'
          //}
          key1 = `Month_Project${item.id}`;
          key2 = `Month_Approach${item.id}`;
          key3 = `Month_${item.id}`;
        }
        //console.log(key1,key2,key3);
        if (this.formInfo.CheckType == "日点检") {
          obj = {
            project: data[key1],
            operation: data[key2],
            mainte: data[key3],
          };
        } else {
          obj = {
            project: data[key1],
            operation: data[key2],
            mainte: data[key3],
            status: "异常",
          };
        }

        if (obj.project != null && obj.operation != null) {
          arr.push(obj);
        }
      });
      //console.log(arr);
      //console.log(arr.length);
      this.tableData = arr;
    },
    //自动填写
    isNormal(row, index) {
      //console.log(row, index);
      if (row.status == "正常") {
        this.tableData[index].mainte = row.operation;
      }
    },
    //点检人保存数据
    onSave() {
      this.$confirm("请确认！")
        .then((_) => {
          //console.log(this.formInfo);
          //console.log(this.tableData, 44);
          let flag = true;
          let submitObj = {};
          let dd = new Date(this.formInfo.Date).getDate();
          let letter_list = ["A", "B", "C", "D", "E", "F", "G", "H", "J", "K"];
          let param = {
            equipmentNumber: this.formInfo.EquipmentNumber,
            userDepartment: this.formInfo.UserDepartment,
            type: this.formInfo.CheckType,
            time: this.formInfo.Date,
          };
          if (this.formInfo.CheckType == "日点检") {
            submitObj[`Day_Mainte_${dd}`] = this.userName;
            this.tableData.forEach((item, index) => {
              if (item.mainte == null || item.mainte == "") {
                this.$message.warning("请补全信息！");
                flag = false;
              } else {
                submitObj[`Day_${letter_list[index]}_${dd}`] = item.mainte;
              }
            });
            //console.log(submitObj, 777);
          }
          if (
            this.formInfo.CheckType == "周点检" &&
            this.VersionNum == "TD-008-D"
          ) {
            let week = this.onGetWeek();
            if (
              this.tableData[0].mainte == null ||
              this.tableData[0].mainte == ""
            ) {
              this.$message.warning("请补全信息！");
              flag = false;
            } else {
              submitObj[`Week_Main_${week}`] = this.userName;
              submitObj[`Week_${week}`] = this.tableData[0].mainte;
              //console.log(submitObj, 777);
            }
          }
          if (
            this.formInfo.CheckType == "周点检" &&
            this.VersionNum == "TD-008-E"
          ) {
            let week = this.onGetWeek();
            submitObj[`Week_Main_${week}`] = this.userName;
            this.tableData.forEach((item, index) => {
              if (item.mainte == null || item.mainte == "") {
                this.$message.warning("请补全信息！");
                flag = false;
              } else {
                submitObj[`Week${week}_mainten${index + 1}`] =
                  this.tableData[index].status + "，" + item.mainte;
              }
              //console.log(flag);
              //console.log(submitObj, 777);
            });
          }
          if (this.formInfo.CheckType == "月点检") {
            submitObj[`Month_main_1`] = this.userName;
            this.tableData.forEach((item, index) => {
              if (item.mainte == null || item.mainte == "") {
                this.$message.warning("请补全信息！");
                flag = false;
              } else {
                if (this.VersionNum == "TD-008-E") {
                  submitObj[`Month_${index + 1}`] =
                    this.tableData[index].status + "，" + item.mainte;
                } else {
                  submitObj[`Month_${index + 1}`] = item.mainte;
                }
              }
            });
            //console.log(submitObj, 777);
          }
          if (flag) {
            submitObj["VersionNum"] = this.VersionNum;
            param["tally_Maintenances"] = submitObj;
            //console.log(param, 55555);
            Save_TallyData(param).then((res) => {
              //console.log(res.data.Data);
              if (res.data.Result) {
                this.$message.success(res.data.Message);
                this.onGetScanCheck();
              }
            });
          }
        })
        .catch((error) => {
          console.log(error);
        });
    },
    //跳转到对应的设备
    onToTurn(item) {
      //console.log(item);
      document.getElementById("scan").scrollIntoView();
      this.formInfo.CheckType = "日点检";
      this.formInfo.EquipmentNumber = item.EquipmentNumber;
    },
  },
  created() {},
  mounted() {},
};
</script>

<style lang='less' scoped>
@import url("~@/assets/style/color.less");
@import url("./page-components/equipment.less");

.title {
  font-weight: 400;
  width: 100%;
  font-size: 18px;
  text-align: center;
}

.form-box {
  width: 100%;
  padding: 10px 20px 0 20px;
  box-sizing: border-box;
  display: flex;
  align-items: center;
  justify-content: center;
}

.form_button_box {
  width: 100%;
  padding: 10px 0 0 0;
  align-items: center;
  justify-content: center;
}

.el-form-item {
  margin-bottom: 4px;
}

.el-form--inline .el-form-item {
  display: flex;
}

.el-form-item__label {
  padding: 0;
}

.success-btn,
.success-btn:active .success-btn:focus,
.success-btn:hover {
  width: 215px;
  background: @success-color;
  border-color: @success-color;
  color: #fff;
}

.info-btn.is-disabled,
.info-btn:active .info-btn:focus,
.info-btn:hover {
  width: 215px;
  background: #ccd3de;
  border-color: #ccd3de;
  color: #fff;
}

.tip {
  margin-top: 10px;
  background-color: #ecf5ff;
  color: @primary-color;
  font-size: 14px;
  margin-right: 10px;
  text-align: center;
  padding: 10px;
}

.el-card {
  font-size: 14px;
  margin-top: 14px;
  margin-right: 10px;
}

.card-have {
  border: 1px solid @success-color;
}

.card-have .el-card__header {
  color: @success-color;
  border-bottom: 1px solid @success-color;
}

.card-never {
  border: 1px solid @error-color;
}

.card-never .el-card__header {
  color: @error-color;
  border-bottom: 1px solid @error-color;
}

.el-card__header {
  padding: 8px 10px;
}

.el-card__body {
  padding: 10px;
}
</style>