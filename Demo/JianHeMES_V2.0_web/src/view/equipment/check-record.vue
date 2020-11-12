<!--- 设备点检保养记录表 --->
<template>
  <div>
    <div>
      <!-- <el-button
              v-if="!addOpera"
              size="mini"
              type="primary"
              plain
              @click="onToTurn"
              >查看对应部门月保养</el-button
            > -->
      <el-button
        v-if="!addOpera"
        size="mini"
        type="primary"
        plain
        @click="onPrint"
        >导出pdf</el-button
      >
    </div>
    <div id="record">
      <div class="eq-header">设备点检保养记录表</div>
      <el-main class="main-box">
        <!-- @* 筛选框s *@ -->
        <div class="check-box">
          <div class="check-item-box">
            <div class="check-box-item">
              <span>设备名称：</span>{{ EquipmentName }}
            </div>
            <div class="check-box-item">
              <span>部门：</span>{{ UserDepartment }}
            </div>
            <div class="check-box-item"><span>线别：</span>{{ LineName }}</div>
            <div class="check-box-item">
              <span>设备编号：</span>{{ EquipmentNumber }}
            </div>
            <div class="check-box-item">
              <span>时间：</span>{{ select_date | formatMonth }}
            </div>
          </div>
        </div>
        <!-- @*表格*@ -->
        <div class="bottomcontainer">
          <table style="width: 100%" border="1">
            <thead>
              <tr>
                <th style="width: 22px; padding: 8px" rowspan="2">周期</th>
                <th style="width: 248px" rowspan="2">项目</th>
                <th style="width: 300px" rowspan="2">操作方法</th>
                <th colspan="31">日期</th>
              </tr>
              <tr>
                <th
                  @click="checkAllDay(i)"
                  v-for="i in 31"
                  :key="i + 'a'"
                  class="qianlan"
                  style="width: 28px"
                >
                  {{ i }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td class="qianlan td-title" :rowspan="dailyPlan.length + 3">
                  日保养
                </td>
              </tr>
              <tr v-for="(item, index) in dailyPlan" :key="index + '1'">
                <td
                  @click.stop.prevent="
                    editText($event, item, index, 'dailyPlan', 'Dproject')
                  "
                  class="editItemStyle item-one"
                >
                  {{ item.Dproject }}
                </td>
                <td
                  @click.stop.prevent="
                    editText($event, item, index, 'dailyPlan', 'DOpera')
                  "
                  class="editItemStyle item-two"
                >
                  {{ item.DOpera }}
                </td>
                <td
                  v-for="i in 31"
                  :key="i + 'b'"
                  style="width: 45px; height: 45px"
                >
                  <select
                    v-model="item[`Day${i}`]"
                    class="testSelect"
                    :disabled="
                      todayNum == i &&
                      (totalOnfos[`Day_Mainte_${i}`] == '' ||
                        totalOnfos[`Day_Mainte_${i}`] == null)
                        ? false
                        : true
                    "
                    :class="
                      todayNum == i &&
                      (totalOnfos[`Day_Mainte_${i}`] == '' ||
                        totalOnfos[`Day_Mainte_${i}`] == null)
                        ? 'point'
                        : 'lvse'
                    "
                  >
                    <option>√</option>
                    <option>×</option>
                    <option>/</option>
                    <option></option>
                  </select>
                </td>
              </tr>
              <tr>
                <td class="td-small-title" colspan="2">保养人</td>
                <td
                  v-for="i in 31"
                  :key="i + 'c'"
                  @click="dayCheck(i)"
                  v-text="
                    totalOnfos[`Day_Mainte_${i}`] != ''
                      ? totalOnfos[`Day_Mainte_${i}`]
                      : '待审'
                  "
                  :class="totalOnfos[`Day_Mainte_${i}`] != '' ? '' : 'text-red'"
                ></td>
              </tr>
              <tr class="changeSize">
                <td class="td-small-title" colspan="2">组长确认</td>
                <td
                  v-for="i in 31"
                  :key="i + 'd'"
                  @click="dayGroupComfirm(i)"
                  v-text="
                    totalOnfos[`Day_group_${i}`] != ''
                      ? totalOnfos[`Day_group_${i}`]
                      : '待审'
                  "
                  :class="totalOnfos[`Day_group_${i}`] != '' ? '' : 'text-red'"
                ></td>
              </tr>
              <tr>
                <td class="qianlan td-title" :rowspan="weekPlan.length + 3">
                  周保养
                </td>
              </tr>
              <template v-if="VersionNum == 'TD-008-D'">
                <tr v-for="(item, index) in weekPlan" :key="index + '2'">
                  <td
                    @click.stop.prevent="
                      editText($event, item, index, 'weekPlan', 'weekObject')
                    "
                    class="editItemStyle item-one"
                  >
                    {{ item.weekObject }}
                  </td>
                  <td
                    @click.stop.prevent="
                      editText($event, item, index, 'weekPlan', 'weekOpera')
                    "
                    class="editItemStyle item-two"
                  >
                    {{ item.weekOpera }}
                  </td>
                  <td
                    v-for="i in 4"
                    :key="i + 'e'"
                    v-show="index == 0 ? true : false"
                    colspan="8"
                    style="text-align: center"
                    :rowspan="weekPlan.length"
                  >
                    <el-input
                      v-if="totalOnfos[`Week_Main_${i}`] == ''"
                      v-model="totalOnfos[`Week_${i}`]"
                      placeholder="请输入内容"
                      type="textarea"
                      :rows="weekPlan.length"
                      :disabled="week == i ? false : true"
                    ></el-input>
                    <div v-else class="changeText">
                      {{ totalOnfos[`Week_${i}`] }}
                    </div>
                  </td>
                </tr>
              </template>
              <template v-if="VersionNum == 'TD-008-E'">
                <tr v-for="(item, index) in weekPlan" :key="index + '3'">
                  <td
                    @click.stop.prevent="
                      editText($event, item, index, 'weekPlan', 'weekObject')
                    "
                    class="editItemStyle item-one"
                  >
                    {{ item.weekObject }}
                  </td>
                  <td
                    @click.stop.prevent="
                      editText($event, item, index, 'weekPlan', 'weekOpera')
                    "
                    class="editItemStyle item-two"
                  >
                    {{ item.weekOpera }}
                  </td>
                  <td
                    v-for="i in 4"
                    :key="i + 'f'"
                    colspan="8"
                    style="text-align: center"
                  >
                    <div
                      v-if="totalOnfos[`Week${i}_mainten${index + 1}`] == ''"
                    >
                      <el-radio-group
                        v-if="week == i"
                        v-model="weekCheck[`w${i}_mainten${index + 1}`]"
                        @change="
                          isNormalWeek(
                            i,
                            index + 1,
                            weekCheck[`w${i}_mainten${index + 1}`]
                          )
                        "
                      >
                        <el-radio label="正常">正常</el-radio>
                        <el-radio label="异常">异常</el-radio>
                      </el-radio-group>
                      <el-input
                        v-model="weekContent[`Week${i}_mainten${index + 1}`]"
                        placeholder="请输入内容"
                        :disabled="week == i ? false : true"
                      ></el-input>
                    </div>
                    <div v-else class="changeText">
                      {{
                        totalOnfos[`Week${i}_mainten${index + 1}`] == null
                          ? ""
                          : totalOnfos[
                              `Week${i}_mainten${index + 1}`
                            ].substring(3)
                      }}
                    </div>
                  </td>
                </tr></template
              >
              <tr>
                <td colspan="2" class="td-small-title">保养人</td>
                <td v-for="i in 4" :key="i + 'g'" :colspan="i == 4 ? 7 : 8">
                  <el-button
                    size="mini"
                    type="success"
                    v-if="totalOnfos[`Week_Main_${i}`] == ''"
                    @click="weekComfirms(i)"
                    :disabled="week == i ? false : true"
                    class="success-btn"
                    >确认</el-button
                  >
                  <span v-else style="font-weight: bold">{{
                    totalOnfos[`Week_Main_${i}`]
                  }}</span>
                </td>
              </tr>
              <tr>
                <td colspan="2" class="td-small-title">工程师确认</td>
                <td v-for="i in 4" :key="i + 'h'" :colspan="i == 4 ? 7 : 8">
                  <el-button
                    size="mini"
                    type="success"
                    v-if="totalOnfos[`Week_engineer_${i}`] == ''"
                    @click="weekMEComfirm(i)"
                    :disabled="
                      totalOnfos[`Week_Main_${i}`] != '' ? false : true
                    "
                    class="success-btn"
                    >确认</el-button
                  >
                  <span v-else style="font-weight: bold">{{
                    totalOnfos[`Week_engineer_${i}`]
                  }}</span>
                </td>
              </tr>

              <tr>
                <td
                  v-if="VersionNum != 'TD-008-E'"
                  class="qianlan td-title"
                  :rowspan="monthPlan.length + 4"
                >
                  月保养
                </td>
                <td
                  v-if="VersionNum == 'TD-008-E'"
                  class="qianlan td-title"
                  :rowspan="monthPlan.length + 3"
                >
                  月保养
                </td>
              </tr>
              <tr v-for="(item, index) in monthPlan" :key="index + '4'">
                <td
                  @click.stop.prevent="
                    editText($event, item, index, 'monthPlan', 'monthObject')
                  "
                  class="editItemStyle item-one"
                >
                  {{ item.monthObject }}
                </td>
                <td
                  @click.stop.prevent="
                    editText($event, item, index, 'monthPlan', 'monthOpera')
                  "
                  class="editItemStyle item-two"
                >
                  {{ item.monthOpera }}
                </td>
                <td colspan="31">
                  <div
                    v-if="totalOnfos.Month_main_1 == ''"
                    style="display: flex; align-items: center"
                  >
                    <el-radio-group
                      v-model="monthCheck[`m_mainten${index + 1}`]"
                      @change="
                        isNormalMonth(
                          index,
                          monthCheck[`m_mainten${index + 1}`]
                        )
                      "
                      style="width: 200px"
                    >
                      <el-radio label="正常">正常</el-radio>
                      <el-radio label="异常">异常</el-radio>
                    </el-radio-group>
                    <el-input
                      v-model="item.dataStr"
                      placeholder="请输入内容"
                      type="textarea"
                      autosize
                    ></el-input>
                  </div>
                  <div v-else class="changeText">
                    {{ item.dataStr == null ? "" : item.dataStr.substring(3) }}
                  </div>
                </td>
              </tr>
              <tr>
                <td colspan="2" class="td-small-title">保养人</td>
                <td colspan="31">
                  <el-button
                    size="mini"
                    type="success"
                    v-if="totalOnfos.Month_main_1 == ''"
                    @click="monthWorkerConfirm()"
                    class="success-btn"
                    >确认</el-button
                  >
                  <span v-else style="font-weight: bold">{{
                    totalOnfos.Month_main_1
                  }}</span>
                </td>
              </tr>
              <tr>
                <td colspan="2" class="td-small-title">
                  {{ VersionNum == "TD-008-E" ? "确认人" : "生产确认" }}
                </td>
                <td colspan="31">
                  <el-button
                    size="mini"
                    type="success"
                    v-if="totalOnfos.Month_productin_2 == ''"
                    @click="monthGroupConfirm()"
                    :disabled="totalOnfos.Month_main_1 != '' ? false : true"
                    class="success-btn"
                    >确认</el-button
                  >
                  <span v-else style="font-weight: bold">{{
                    totalOnfos.Month_productin_2
                  }}</span>
                </td>
              </tr>
              <tr v-if="VersionNum != 'TD-008-E'">
                <td colspan="2" class="td-small-title">部长确认</td>
                <td colspan="31">
                  <el-button
                    size="mini"
                    type="success"
                    v-if="totalOnfos.Month_minister_3 == ''"
                    @click="monthDeparConfirm()"
                    :disabled="
                      totalOnfos.Month_productin_2 != '' ? false : true
                    "
                    class="success-btn"
                    >确认</el-button
                  >
                  <span v-else style="font-weight: bold">{{
                    totalOnfos.Month_minister_3
                  }}</span>
                </td>
              </tr>

              <tr>
                <td class="qianlan td-title" rowspan="4">说明</td>
                <td style="text-align: left; padding: 5px" colspan="33">
                  1.完成情况标记符号：“√”正常已完成 “×”异常、待处理
                  “/”休息及计划停机中.以上保养项目必须切实执行并如实记录.
                </td>
              </tr>
              <tr>
                <td style="text-align: left; padding: 5px" colspan="33">
                  2.其中日保养由生产操作员执行,生产组长监督执行结果.周保养项目由设备技术员执行,设备工程师监督执行结果.月保养由设备工程师主导，设备技术员配合完成.
                </td>
              </tr>
              <tr>
                <td style="text-align: left; padding: 5px" colspan="33">
                  3.每天早上9点之前完成日保养，周六、日做周保养，月保养根据月保养计划表执行.
                </td>
              </tr>
              <tr>
                <td style="text-align: left; padding: 5px" colspan="33">
                  4.如有更换配件请注明配件名称和配件编号
                </td>
              </tr>
            </tbody>
          </table>
          <div class="note">
            <span style="color: #7a899f"
              >备注：根据体系表格文件样式规定，每月分成固定四个周(不按标准自然周划分)对设备进行周保养。</span
            >
            <span style="color: #000000">表单编号：{{ VersionNum }}</span>
          </div>
        </div>
        <!-- @* 操作按钮 *@ -->
        <div class="btn-box" v-if="addOpera">
          <el-button size="small" type="success" @click="addDailyPlan"
            >增加日计划项目</el-button
          >
          <el-button size="small" type="success" @click="addWeekPlan"
            >增加周计划项目</el-button
          >
          <el-button
            style="margin-right: 15px"
            size="small"
            type="success"
            @click="addMonthPlan"
            >增加月计划项目</el-button
          >
          <el-select
            size="small"
            v-model="Group"
            placeholder="请选择班组"
            style="margin-right: 10px; width: 150px"
          >
            <el-option
              v-for="item in group_options"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            >
            </el-option>
          </el-select>
          <ImportExcel :on-success="onSuccess">
            <el-button plain type="success" size="small">选取文件</el-button>
          </ImportExcel>
          <el-button
            v-show="showSaveInfosBtn"
            size="small"
            type="success"
            @click="addSave"
            >保存</el-button
          >
          <el-button
            v-show="showSaveInfosBtn"
            size="small"
            plain
            type="success"
            @click="resetForm"
            >重置</el-button
          >
        </div>
        <!-- @* 添加项目弹框 *@ -->
        <el-dialog
          :title="dialogVisibletitle"
          :visible.sync="dialogVisible"
          width="30%"
        >
          <div class="item-box">
            <el-input
              size="small"
              placeholder="请输入内容"
              v-model="addDailyProject"
            >
              <template slot="prepend">
                &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;项目:
              </template>
            </el-input>
            <el-input
              size="small"
              placeholder="请输入内容"
              v-model="addDailyOpera"
            >
              <template slot="prepend"> 操作方法: </template>
            </el-input>
          </div>
          <span slot="footer" class="dialog-footer">
            <el-button @click="dialogVisible = false">取 消</el-button>
            <el-button type="primary" @click="addDaily">确 定</el-button>
          </span>
        </el-dialog>
      </el-main>
    </div>
  </div>
</template>

<script>
import ImportExcel from "_c/import-excel";
import { DisplayGroup } from "@/api/common";
import {
  Equipment_Query_Tally,
  Equipment_Tally_maintenance_Add,
  Equipment_Tally_maintenance_Edit,
  Upload_Equipment_Tally,
} from "@/api/equipment";
import { formatDate } from "@/filters/index";
export default {
  name: "Equipment_Check_Record",
  inject: ["reload"],
  props: {},
  data() {
    return {
      userName: this.$userInfo.Name,
      Group: "",
      group_options: [], //获取班组
      VersionNum: "", //版本号
      select_date: "", //时间
      week: "", //当前周
      //是否创建点检表
      canchange: "", //true为创建表，false为修改表
      adddianjian: false, //点检权限
      addOpera: false,
      //设备信息
      EquipmentName: "", //设备名称
      UserDepartment: "", //部门
      EquipmentNumber: "", //设备编号
      LineName: "", //线别
      //表格
      todayNum: "", //判断是否当天显示日点检
      totalOnfos: "", //点检数据
      dailyPlan: [], //日点检数据
      weekPlan: [], //周点检数据
      monthPlan: [], //月点检数据

      //新版本是否异常及自动填写判断
      weekCheck: {}, //周判断保养内容，是否正常
      weekContent: {}, //周判断保养内容，是否正常
      monthCheck: {}, //月判断保养内容，是否正常

      //创建表添加各项计划
      addDailyProject: "",
      addDailyOpera: "",
      popoverFlag: "",
      //上传文件
      showuploadBtn: false,
      fileList: [],
      //底部按钮操作
      showSaveInfosBtn: false, //控制底部按钮显示
      dialogVisible: false, //控制添加日周月项目弹框显示
      dialogVisibletitle: "", //添加日周月项目弹框标题
    };
  },
  components: { ImportExcel },
  computed: {},
  watch: {
    // 监听日保养计划栏
    dailyPlan() {
      if (this.dailyPlan.length > 0) {
        this.showSaveInfosBtn = true;
      } else {
        this.showSaveInfosBtn = false;
      }
    },
  },
  methods: {
    // 查看对应部门月保养
    // onToTurn() {
    //   let dd = formatDate(this.select_date);
    //   this.$router.push({
    //     name: "Equipment_Maintenance_Summary",
    //     query: { paramData: this.UserDepartment, date: dd },
    //   });
    // window.open(
    //   "/Equipment/Equipment_Maintenance_Summary?paramData=" +
    //     this.UserDepartment +
    //     "&date=" +
    //     this.select_date
    // );
    // },
    //地址参数
    getAddress() {
      let urlSearchParam = this.$route.query;
      this.canchange = urlSearchParam.canchange;
      let param = JSON.parse(urlSearchParam.paramData);
      // console.log(param);
      // console.log(this.$route.query);
      this.EquipmentNumber = param.EquipmentNumber;
      this.select_date = new Date(param.time);
      // console.log(this.select_date);
      if (this.canchange == "true") {
        this.EquipmentName = param.EquipmentName;
        this.UserDepartment = param.UserDepartment;
        this.LineName = param.LineNum;
        //获取班组
        this.getGroupData();
      }
    },
    //获取账号班组列表
    getGroupData() {
      DisplayGroup()
        .then((res) => {
          //未登陆则返回null
          if (res.data.Data == "") {
            //this.$message.warning("登陆状态已丢失，请重试打开页面！")
          } else {
            this.Group = res.data.Data.Group[0];
            let arr = [];
            res.data.Data.Group.forEach((item) => {
              let obj = {
                label: item,
                value: item,
              };
              arr.push(obj);
            });
            this.group_options = arr;
            //console.log(this.group_options);
          }
        })
        .catch((err) => {
          //console.log(err);
        });
    },
    //判断当前第几周
    judgeWeek() {
      let dd = new Date();
      let day = dd.getDate();
      this.todayNum = day;
      if (day > 0 && day <= 8) {
        this.week = 1;
      } else if (day > 8 && day <= 16) {
        this.week = 2;
      } else if (day > 16 && day <= 24) {
        this.week = 3;
      } else if (day > 24 && day <= 31) {
        this.week = 4;
      }
    },
    //周，月判断加载保养内容字段，是否异常字段
    initDataCheck() {
      let weekobj = {};
      let monthobj = {};
      let contentobj = {};
      for (let i = 1; i <= 11; i++) {
        weekobj[`w${this.week}_mainten${i}`] = "异常";
        monthobj[`m_mainten${i}`] = "异常";
        contentobj[`Week${this.week}_mainten${i}`] = "";
      }
      this.weekCheck = weekobj;
      this.monthCheck = monthobj;
      this.weekContent = contentobj;
      //console.log(this.weekCheck, 99999);
    },
    //周保养自动填写
    isNormalWeek(i, index, val) {
      if (val == "正常") {
        //console.log(i, index, val);
        if (this.weekContent[`Week${i}_mainten${index}`] == "") {
          this.weekContent[`Week${i}_mainten${index}`] = "正常";
          //this.weekContent[`Week${i}_mainten${index}`] = this.weekPlan[index - 1].weekOpera;
        }
      } else {
        this.weekContent[`Week${i}_mainten${index}`] = "";
      }
    },
    //月保养自动填写
    isNormalMonth(index, val) {
      if (val == "正常") {
        //console.log(index, val);
        if (this.monthPlan[index].dataStr == "") {
          this.monthPlan[index].dataStr = "正常";
          //this.monthPlan[index].dataStr = this.monthPlan[index].monthOpera;
        }
      } else {
        this.monthPlan[index].dataStr = "";
      }
    },
    // 页面加载时获取点检表的数据
    getDefultInfos() {
      let dd = new Date(this.select_date);
      let year = dd.getFullYear();
      let month = dd.getMonth() + 1;
      Equipment_Query_Tally({
        equipmentNumber: this.EquipmentNumber,
        year: year,
        month: month,
      })
        .then((res) => {
          if (JSON.stringify(res.data.Data) != "{}") {
            //console.log(res.data.Data, 00);
            this.totalOnfos = res.data.Data;
            this.VersionNum = res.data.Data.VersionNum;
            this.UserDepartment = res.data.Data.UserDepartment;
            this.EquipmentName = res.data.Data.EquipmentName;
            this.LineName = res.data.Data.LineName;
            // 处理上数据
            this.makeDataCanUes(res.data.Data, "Day_project");
            this.changeSelectView(this.dailyPlan);
            // 处理中数据
            this.makeDataCanUes(res.data.Data, "Week_Check");
            // 处理下数据
            this.makeDataCanUes(res.data.Data, "Month_Project");
          } else {
            this.$message.warning("此时间段暂无点检记录!");
          }
        })
        .catch((err) => {
          console.log(err);
          this.$message.warning("获取点检表信息失败!");
        });
    },
    // 转换选择框的显示
    changeSelectView(dataobj) {
      dataobj.forEach((item) => {
        for (let i = 1; i <= 31; i++) {
          let kw = `Day${i}`;
          //console.log(item[kw])
          if (item[kw] == "0") {
            item[kw] = "";
          } else if (item[kw] == "1") {
            item[kw] = "√";
          } else if (item[kw] == "2") {
            item[kw] = "×";
          } else if (item[kw] == "3") {
            //console.log(item[kw])
            item[kw] = "/";
          }
        }
      });
    },
    //处理数据
    makeDataCanUes(dataObj, kw) {
      let arr = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K"];
      if (kw == "Day_project") {
        this.dailyPlan = [];
        for (let i = 1; i <= arr.length; i++) {
          let keyleft = `Day_project${i}`;
          let dailyOpera = `Day_opera${i}`;
          let baoyangTime = `Day_MainteTime_${i}`;
          let obj = {
            Dproject: dataObj[keyleft],
            DOpera: dataObj[dailyOpera],
            baoyangTime: dataObj[baoyangTime],
          }; // 添加操作时间
          let works = arr[i - 1];
          for (let j = 1; j <= 31; j++) {
            let ketright = `Day_${works}_${j}`;
            let Days = `Day${j}`;
            obj[Days] = dataObj[ketright];
          }
          if (dataObj[keyleft] == "" || dataObj[keyleft] == null) {
          } else {
            this.dailyPlan.push(obj);
          }
        }
        //console.log(this.dailyPlan);
      } else if (kw == "Week_Check") {
        this.weekPlan = [];
        for (let i = 1; i <= arr.length; i++) {
          let keyLeft = `Week_Check${i}`;
          let ketLeftTwo = `Week_Inspe${i}`;
          let obj = {
            weekObject: dataObj[keyLeft],
            weekOpera: dataObj[ketLeftTwo],
            weekProveOne: dataObj.Week_1,
            weekProveTwo: dataObj.Week_2,
            weekProveThree: dataObj.Week_3,
            weekProveFour: dataObj.Week_4,
          };
          if (dataObj[keyLeft] == "" || dataObj[keyLeft] == null) {
          } else {
            this.weekPlan.push(obj);
          }
        }
        //console.log(this.weekPlan);
        //console.log(this.totalOnfos);
      } else if (kw == "Month_Project") {
        this.monthPlan = [];
        for (let i = 1; i <= arr.length; i++) {
          let keyLeft = `Month_Project${i}`;
          let keyLeftTwo = `Month_Approach${i}`;
          let dataKey = `Month_${i}`;
          let obj = {
            monthObject: dataObj[keyLeft],
            monthOpera: dataObj[keyLeftTwo],
            dataStr: dataObj[dataKey],
          };
          if (dataObj[keyLeft] == "" || dataObj[keyLeft] == null) {
          } else {
            this.monthPlan.push(obj);
          }
        }
        //console.log(this.monthPlan, 5556);
      }
    },
    //编辑
    editText(e, item, index, name, text) {
      //console.log(e, item, index, name, text);
      let dd = new Date(this.select_date);
      let day = dd.getDate();
      //console.log(day);

      //每月5号前可以修改点检表的左侧信息
      if (day <= 5) {
        if (this.adddianjian && e.target.localName == "td") {
          e.target.innerHTML = `<textarea id='inputText' style="width:98%;" onkeyup="vm.keyupText(${index},'${name}','${text}')" onBlur="vm.blurText(${index},'${name}','${text}')">${item[text]}</textarea>`;
          let _thisinput = document.getElementById("inputText");
          _thisinput.focus();
          //console.log(_thisinput)
          _thisinput.selectionStart = _thisinput.selectionEnd =
            _thisinput.value.length;
        }
      }
    },
    //编辑失焦
    blurText(index, name, text) {
      //console.log(index, name, text, event);
      this[name][index][text] = event.target.value;
      event.target.outerHTML = event.target.value;
      this.addSave();
    },
    //编辑回车
    keyupText(index, name, text) {
      //console.log(index, name, text, event);
      if (event.keyCode == 13) {
        this[name][index][text] = event.target.value;
        event.target.outerHTML = event.target.value;
      }
    },
    // 保存新建或者修改的方法--- 此时需要反转重组数据
    addSave() {
      let postObj = {};
      postObj["UserDepartment"] = this.UserDepartment;
      postObj["EquipmentName"] = this.EquipmentName;
      postObj["EquipmentNumber"] = this.EquipmentNumber;
      postObj["LineName"] = this.LineName;
      postObj["VersionNum"] = this.VersionNum;
      // 还原上数据
      let arr = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L"];
      for (let i = 0; i < this.dailyPlan.length; i++) {
        postObj[`Day_project${i + 1}`] = this.dailyPlan[i].Dproject;
        postObj[`Day_opera${i + 1}`] = this.dailyPlan[i].DOpera;
        for (let j = 1; j <= 31; j++) {
          if (this.dailyPlan[i][`Day${j}`] == "") {
            postObj[`Day_${arr[i]}_${j}`] = 0;
          } else if (this.dailyPlan[i][`Day${j}`] == "√") {
            postObj[`Day_${arr[i]}_${j}`] = 1;
          } else if (this.dailyPlan[i][`Day${j}`] == "×") {
            postObj[`Day_${arr[i]}_${j}`] = 2;
          } else if (this.dailyPlan[i][`Day${j}`] == "/") {
            postObj[`Day_${arr[i]}_${j}`] = 3;
          }
        }
      }
      for (let i = 1; i <= 31; i++) {
        postObj[`Day_Mainte_${i}`] = this.totalOnfos[`Day_Mainte_${i}`];
        postObj[`Day_group_${i}`] = this.totalOnfos[`Day_group_${i}`];
        postObj[`Day_MainteTime_${i}`] = this.totalOnfos[`Day_MainteTime_${i}`]; //  保养人操作时间
        postObj[`Day_groupTime_${i}`] = this.totalOnfos[`Day_groupTime_${i}`]; // 保养组长确认年时间
        // 可在此处添加操作时间
      }

      // 还原中数据
      for (let i = 0; i < this.weekPlan.length; i++) {
        postObj[`Week_Check${i + 1}`] = this.weekPlan[i].weekObject;
        postObj[`Week_Inspe${i + 1}`] = this.weekPlan[i].weekOpera;
      }
      //TD-008-E版本保存保养内容数据
      for (let i = 1; i <= 11; i++) {
        postObj[`Week1_mainten${i}`] = this.totalOnfos[`Week1_mainten${i}`];
        postObj[`Week2_mainten${i}`] = this.totalOnfos[`Week2_mainten${i}`];
        postObj[`Week3_mainten${i}`] = this.totalOnfos[`Week3_mainten${i}`];
        postObj[`Week4_mainten${i}`] = this.totalOnfos[`Week4_mainten${i}`];
      }
      for (let i = 1; i <= 4; i++) {
        //旧版本保养内容数据
        postObj[`Week_${i}`] = this.totalOnfos[`Week_${i}`];
        // 周保养人
        postObj[`Week_Main_${i}`] = this.totalOnfos[`Week_Main_${i}`];
        // 周保养人操作时间
        postObj[`Week_MainTime_${i}`] = this.totalOnfos[`Week_MainTime_${1}`];
        // 周保养工程师
        postObj[`Week_engineer_${i}`] = this.totalOnfos[`Week_engineer_${i}`];
        // 周保养工程师操作时间
        postObj[`Week_engTime_${i}`] = this.totalOnfos[`Week_engTime_${i}`];
      }

      // 还原下数据
      for (let i = 0; i < this.monthPlan.length; i++) {
        postObj[`Month_Project${i + 1}`] = this.monthPlan[i].monthObject;
        postObj[`Month_Approach${i + 1}`] = this.monthPlan[i].monthOpera;
        postObj[`Month_${i + 1}`] = this.monthPlan[i].dataStr;
      }
      postObj["Month_mainTime_1"] = this.totalOnfos.Month_mainTime_1; // 月保养时间
      postObj["Month_produTime_2"] = this.totalOnfos.Month_produTime_2; // 生产确认时间
      postObj["Month_minisime_3"] = this.totalOnfos.Month_minisime_3; // 部长确认时间

      postObj["Month_main_1"] = this.totalOnfos.Month_main_1;
      postObj["Month_productin_2"] = this.totalOnfos.Month_productin_2;
      postObj["Month_minister_3"] = this.totalOnfos.Month_minister_3;

      //console.log(postObj, 88888);
      if (this.addOpera) {
        let dd = new Date(this.select_date);
        let y = dd.getFullYear();
        let m = dd.getMonth() + 1;
        postObj.Year = y;
        postObj.Month = m;
        // 创建保存
        Equipment_Tally_maintenance_Add(postObj)
          .then((res) => {
            if (res.data.Result) {
              this.$message.success(res.data.Message);
              let param = {
                EquipmentNumber: this.EquipmentNumber,
                time: this.select_date,
              };
              window.location.href =
                "/Equipment/Equipment_Check_Record?paramData=" +
                JSON.stringify(param) +
                "&canchange=false";
              this.dailyPlan = [];
              this.weekPlan = [];
              this.monthPlan = [];
              fileList = [];
            } else this.$message.warning(res.data.Message);
          })
          .catch((err) => {
            console.log(err);
          });
      } else {
        postObj.Id = this.totalOnfos.Id;
        postObj.Year = this.select_date.getFullYear();
        postObj.Month = this.select_date.getMonth() + 1;
        console.log(postObj);
        // 保存修改Equipment_Tally_maintenance_Edit
        Equipment_Tally_maintenance_Edit(postObj)
          .then((res) => {
            //console.log(res.data.Data);
            if (res.data.Result) {
              this.$message.success(res.data.Message);
            } else {
              if (res.data.Data.equipment_Tally_maintenance != null) {
                this.$message.warning(
                  res.data.Data.equipment_Tally_maintenance
                );
              } else {
                this.$message.success(res.data.Message);
              }
            }
          })
          .catch((err) => {
            console.log(err);
          });
      }
    },
    // 日保养确认
    checkAllDay(index) {
      if (this.todayNum != index) {
        return false;
      }
      //console.log(index)
      let dd = `Day${index}`;
      this.dailyPlan.forEach((item) => {
        if (item[dd] == "√") {
          item[dd] = " ";
        } else if ((item[dd] = " ")) {
          item[dd] = "√";
        }
      });
      //this.addSave();
    },
    // 日计划保养人审批---需要判断
    dayCheck(index) {
      //console.log(this.dailyPlan);
      let day = `Day${index}`;
      let flag;
      //判断当天是否勾选才能进行确认
      this.dailyPlan.forEach((item) => {
        if (item[day] == "") {
          flag = false;
        } else {
          flag = true;
        }
      });
      if (flag) {
        if (
          this.totalOnfos[`Day_Mainte_${index}`] == "" ||
          this.totalOnfos[`Day_Mainte_${index}`] == null
        ) {
          if (this.$limit("日点检保养人确认")) {
            this.$confirm("请确认！").then((_) => {
              this.totalOnfos[`Day_Mainte_${index}`] = this.userName;
              this.totalOnfos[`Day_MainteTime_${index}`] = new Date();
              this.addSave();
            });
          } else {
            this.$message.warning("无权限进行此操作");
          }
        } else {
          this.$message.warning("已审批，不可重复操作");
        }
      } else {
        this.$message.warning("有未确认保养设备，不可审批");
      }
    },
    // 日计划组长确认
    dayGroupComfirm(index) {
      if (
        this.totalOnfos[`Day_group_${index}`] == "" &&
        this.totalOnfos[`Day_Mainte_${index}`] != ""
      ) {
        if (this.$limit("日点检组长确认")) {
          this.$confirm("请确认！").then((_) => {
            this.totalOnfos[`Day_group_${index}`] = this.userName;
            this.totalOnfos[`Day_groupTime_${index}`] = new Date();
            this.addSave();
          });
        } else {
          this.$message.warning("无权限进行此操作");
        }
      } else {
        this.$message.warning("保养人未确认或组长已审核,不可操作");
      }
    },
    // 周计划保养人确认
    weekComfirms(i) {
      if (this.$limit("周保养人确认")) {
        if (this.VersionNum == "TD-008-E") {
          //判断TD-008-E版本填写保养内容是否为空
          let flag = true;
          for (let i = 1; i <= this.weekPlan.length; i++) {
            //console.log(this.week, i)
            if (this.weekContent[`Week${this.week}_mainten${i}`] == "") {
              flag = false;
              this.$message.warning("请先补全内容！");
              return flag;
            }
          }
          if (flag) {
            this.$confirm("请确认保养情况！").then((_) => {
              this.totalOnfos[`Week_Main_${i}`] = this.userName;
              this.totalOnfos[`Week_MainTime_${i}`] = new Date();
              for (let k = 1; k <= this.weekPlan.length; k++) {
                //为数据添加正常异常判断
                this.totalOnfos[`Week${this.week}_mainten${k}`] =
                  this.weekCheck[`w${this.week}_mainten${k}`] +
                  "，" +
                  this.weekContent[`Week${this.week}_mainten${k}`];
              }
              //console.log(this.totalOnfos,9999999)
              this.addSave();
            });
          }
        } else {
          if (this.totalOnfos[`Week_${i}`] != "") {
            this.$confirm("请确认保养情况！").then((_) => {
              this.totalOnfos[`Week_Main_${i}`] = this.userName;
              this.totalOnfos[`Week_MainTime_${i}`] = new Date();
              this.addSave();
            });
          } else {
            this.$message.warning("请先输入内容！");
          }
        }
      } else {
        this.$message.warning("暂无权限！");
      }
    },
    // 周保养工程师确认
    weekMEComfirm(i) {
      if (this.$limit("周保养工程师确认")) {
        this.$confirm("请确认保养情况！").then((_) => {
          this.totalOnfos[`Week_engineer_${i}`] = this.userName;
          this.totalOnfos[`Week_engTime_${i}`] = new Date();
          this.addSave();
        });
      } else {
        this.$message.warning("暂无权限！");
      }
    },
    // 月保养人确认
    monthWorkerConfirm() {
      let flag = true;
      //判断内容填写完整
      this.monthPlan.forEach((item) => {
        if (item.dataStr == "") {
          flag = false;
        }
      });
      if (flag) {
        if (this.$limit("月保养人确认")) {
          this.$confirm("请确认保养情况！").then((_) => {
            this.totalOnfos.Month_main_1 = this.userName;
            this.totalOnfos.Month_mainTime_1 = new Date();
            for (let k = 0; k < this.monthPlan.length; k++) {
              //为数据添加正常异常判断
              this.monthPlan[k].dataStr =
                this.monthCheck[`m_mainten${k + 1}`] +
                "，" +
                this.monthPlan[k].dataStr;
            }
            this.addSave();
          });
        } else {
          this.$message.warning("暂无权限！");
        }
      } else {
        this.$message.warning("月计划保养项目未填写完整,不可操作!");
      }
    },
    // 月生产确认
    monthGroupConfirm() {
      if (this.$limit("点检生产确认")) {
        this.$confirm("请确认保养情况！").then((_) => {
          this.totalOnfos.Month_productin_2 = this.userName;
          this.totalOnfos.Month_produTime_2 = new Date();
          this.addSave();
        });
      } else {
        this.$message.warning("暂无权限！");
      }
    },
    // 月部长确认
    monthDeparConfirm() {
      if (this.$limit("点检部长确认")) {
        this.$confirm("请确认保养情况！").then((_) => {
          this.totalOnfos.Month_minister_3 = this.userName;
          this.totalOnfos.Month_minisime_3 = new Date();
          this.addSave();
        });
      } else {
        this.$message.warning("暂无权限！");
      }
    },
    // 添加日计划项目--弹出弹框
    addDailyPlan() {
      this.dialogVisible = true;
      this.dialogVisibletitle = "添加日计划项目";
      this.popoverFlag = "daily";
    },
    // 添加周计划项目--弹出弹框
    addWeekPlan() {
      this.dialogVisible = true;
      this.dialogVisibletitle = "添加周计划项目";
      this.popoverFlag = "week";
    },
    // 添加月计划项目--弹出弹框
    addMonthPlan() {
      this.dialogVisible = true;
      this.dialogVisibletitle = "添加月计划项目";
      this.popoverFlag = "month";
    },
    // 添加计划项目--提交添加--目前未提交，只是渲染到视图
    addDaily() {
      if (this.addDailyProject !== "" && this.addDailyOpera !== "") {
        if (this.popoverFlag == "daily") {
          let obj = {
            Dproject: this.addDailyProject,
            DOpera: this.addDailyOpera,
          };
          this.dailyPlan.push(obj);
        } else if (this.popoverFlag == "week") {
          let obj = {
            weekObject: this.addDailyProject,
            weekOpera: this.addDailyOpera,
          };
          this.weekPlan.push(obj);
        } else if (this.popoverFlag == "month") {
          let obj = {
            monthObject: this.addDailyProject,
            monthOpera: this.addDailyOpera,
          };
          this.monthPlan.push(obj);
        }
        this.dialogVisible = false;
        this.popoverFlag = "";
        this.addDailyProject = "";
        this.addDailyOpera = "";
      }
    },
    // 重置按钮
    resetForm() {
      this.reload();
    },
    //导入excel
    onSuccess(response, file) {
      if (response.length != 0) {
        this.showuploadBtn = true;
        let json = response[0].data;
        // console.log(json);
        this.dailyPlan = [];
        this.weekPlan = [];
        this.monthPlan = [];
        json.forEach((item) => {
          if (item.__rowNum__ >= 5 && item.__rowNum__ <= 15) {
            let json_obj1 = JSON.parse(
              JSON.stringify(item).replace(/\ +/g, "")
            );
            let obj1 = {
              Dproject: json_obj1["惠州市健和光电有限公司"],
              DOpera: json_obj1["__EMPTY_2"],
            };
            this.dailyPlan.push(obj1);
          }
          if (item.__rowNum__ >= 18 && item.__rowNum__ <= 28) {
            let json_obj2 = JSON.parse(
              JSON.stringify(item).replace(/\ +/g, "")
            );
            let obj2 = {
              weekObject: json_obj2["惠州市健和光电有限公司"],
              weekOpera: json_obj2["__EMPTY_2"],
            };
            this.weekPlan.push(obj2);
          }
          if (item.__rowNum__ >= 31 && item.__rowNum__ <= 41) {
            let json_obj3 = JSON.parse(
              JSON.stringify(item).replace(/\ +/g, "")
            );
            let obj3 = {
              monthObject: json_obj3["惠州市健和光电有限公司"],
              monthOpera: json_obj3["__EMPTY_2"],
            };
            this.monthPlan.push(obj3);
          }
        });
      }
    },
    //导出pdf
    onPrint() {
      //导出pdf
      let printMsg = {
        direction: "p",
        unit: "pt",
        size: "a4",
      };
      this.getPdf("设备保养点检表", "#record", printMsg);
    },
  },
  created() {},
  mounted() {
    //获取第几周
    this.judgeWeek();
    this.initDataCheck();
    //获取地址传参
    this.getAddress();
    //获取权限
    this.adddianjian = this.$limit("新建点检表");
    //console.log(this.adddianjian)
    //console.log(this.canchange)
    if (this.canchange == "true" && this.adddianjian) {
      this.addOpera = true;
      this.VersionNum = "TD-008-E";
    }
    // 页面修改加载时获取点检表的数据
    if (!this.addOpera) {
      this.getDefultInfos();
    }
  },
};
</script>

<style lang='less' scoped>
@import url("~@/assets/style/color.less");
@import url("./page-components/equipment.less");

table {
  border: 1px solid #8ea0b8;
}

.testSelect {
  border: none;
  width: 100%;
  height: 100%;
  padding: 1px;
  /*            border: 1px solid #dcd8d8;*/
  -webkit-appearance: none;
  -moz-appearance: none;
  appearance: none; /*去掉下拉箭头*/
  /*在选择框的最右侧中间显示小箭头图片*/
  /*background: url("http://ourjs.github.io/static/2015/arrow.png") no-repeat scroll right center transparent;*/
}

.check-box {
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 14px;
  font-size: 16px;
}

.check-item-box {
  display: flex;
  align-items: center;
}

.check-box-item {
  margin-right: 35px;
  display: flex;
  align-items: center;
}

.check-box-item span {
  font-weight: bold;
}

/*        表格*/
table tr th,
table tr td {
  text-align: center;
  font-size: 16px;
}

.color {
  color: red;
}

.editItemStyle {
  text-align: left;
  padding: 5px;
}
.item-one {
  min-width: 180px;
}
.item-two {
  min-width: 360px;
}
.text-red {
  cursor: pointer;
  color: #fff;
  background-color: @error-color;
}

.el-upload__input {
  display: none !important;
}

.upload-demo {
  margin-left: 15px;
}

.td-title {
  padding: 8px;
}

.td-small-title {
  text-align: center;
  font-size: 16px;
  font-weight: bold;
  padding: 8px;
}

th,
.qianlan {
  background-color: #e4ecf7;
}

.lvse {
  background-color: #e6f8ea;
}

.point {
  text-align: center;
  cursor: pointer;
}

.btn-box {
  margin-top: 20px;
}

.item-box .el-input {
  margin-top: 10px;
}

.success-btn,
.success-btn:hover {
  color: #fff;
  padding: 7px 50px;
  margin-right: 0;
}
.note {
  width: 100%;
  font-size: 14px;
  margin: 10px 0;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.changeText {
  text-align: left;
  padding: 0 4px;
}
</style>