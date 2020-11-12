<!--- 权限管理 --->
<template>
  <div class="main-box">
    <div class="main-header">权限管理</div>
    <el-main class="main-box">
      <el-row :gutter="20">
        <el-col :span="4" class="limit-left-box">
          <el-menu :unique-opened="true" @select="handleSelect">
            <el-submenu
              :index="item.title"
              v-for="(item, i) in PermissList"
              :key="i"
            >
              <template slot="title">
                <span>{{ item.title }}</span>
              </template>
              <el-menu-item
                :index="role"
                v-for="(role, j) in item.roles"
                :key="j"
                >{{ role }}</el-menu-item
              >
            </el-submenu>
          </el-menu>
        </el-col>
        <el-col :span="20">
          <div class="limit-box">
            <div class="nav-path">{{ rolesNav[0] }} / {{ rolesNav[1] }}</div>
            <div class="nav-more">
              <span>部门：</span>
              <el-select
                v-model="department"
                size="mini"
                clearable
                filterable
                placeholder="请选择部门"
                style="width: 140px"
              >
                <el-option
                  v-for="item in deparlist"
                  v-bind:key="item.value"
                  v-bind:label="item.label"
                  v-bind:value="item.value"
                >
                </el-option>
              </el-select>
              <span>姓名：</span>
              <el-input
                v-model="userName"
                size="mini"
                placeholder="请输入姓名"
                style="width: 140px"
                clearable
              ></el-input>
              <el-button type="primary" size="mini" @click="query"
                >查询</el-button
              >
              <el-button
                type="primary"
                plain
                size="mini"
                @click="onOpen('模块')"
                >操作模块</el-button
              >
              <el-button
                type="primary"
                plain
                size="mini"
                @click="onOpen('权限组')"
                >操作权限组</el-button
              >
              <el-button
                type="primary"
                plain
                size="mini"
                @click="onOpen('权限名')"
                >操作权限名</el-button
              >
            </div>
          </div>
          <el-table
            :data="tableData"
            border
            style="width: 100%"
            max-height="660"
          >
            <el-table-column
              prop="Department"
              label="部门"
              width="100"
            ></el-table-column>
            <el-table-column
              prop="Role"
              label="职位"
              width="100"
            ></el-table-column>
            <el-table-column
              prop="UserNum"
              label="工号"
              width="100"
            ></el-table-column>
            <el-table-column
              prop="UserName"
              label="姓名"
              width="100"
            ></el-table-column>
            <el-table-column label="权限配置">
              <template slot-scope="scope">
                <el-checkbox
                  style="pointer-events: none"
                  v-for="item in scope.row.Table"
                  :label="item.Discription"
                  :checked="item.Discr"
                  :key="item.Discription"
                  >{{ item.Discription }}</el-checkbox
                >
              </template>
            </el-table-column>
            <el-table-column label="操作" width="100">
              <template slot-scope="scope">
                <el-button
                  type="text"
                  size="mini"
                  @click="moreAction(scope.row)"
                  style="text-decoration: underline"
                  >操作</el-button
                >
              </template>
            </el-table-column>
          </el-table>
          <!-- @* 操作权限组/权限名/模块 *@ -->
          <el-dialog
            :title="'操作' + titleName"
            :visible.sync="isVisible"
            width="36%"
          >
            <div class="titleName">{{ rolesNav[0] }} / {{ rolesNav[1] }}</div>
            <div class="group-add">
              <span>新增{{ titleName }}：</span>
              <el-input
                v-model="groupName"
                style="width: 150px; margin-right: 10px"
              ></el-input>
              <el-button type="primary" @click="onAdd()">添加</el-button>
            </div>
            <div class="group-add" v-if="isModify">
              <span>修改{{ titleName }}：</span>
              <el-input
                v-model="new_groupName"
                style="width: 150px; margin-right: 10px"
              ></el-input>
              <el-button type="primary" @click="onModify()">修改</el-button>
              <el-button type="primary" plain @click="isModify = false"
                >取消</el-button
              >
            </div>
            <div class="group-add" style="margin-top: 10px">
              <span>已有{{ titleName }}：</span>
              <el-button
                style="margin-bottom: 10px"
                v-for="(groupList, index) in groupLists"
                :key="index"
                @click="onChooese(groupList)"
                >{{ groupList }}</el-button
              >
            </div>
          </el-dialog>
          <!-- @* 授权 *@ -->
          <el-dialog
            title="授权操作"
            :visible.sync="isAuthorization"
            width="50%"
          >
            <div class="titleName">
              <span>部门：{{ chooeseItem.Department }}</span>
              <span>职位：{{ chooeseItem.Role }} </span>
              <span>姓名：{{ chooeseItem.UserName }}</span>
            </div>
            <div class="authorization-box">
              <div
                class="authorization-item"
                style="border-right: 1px solid #e4ecf7"
              >
                <div class="limit-title">已有权限：</div>
                <div class="limit-item">
                <el-checkbox-group
                  v-model="haveLimit"
                  @change="changeHaveLimit"
                >
                  <el-checkbox
                    v-for="item in haveLimitList"
                    :label="item"
                    :key="item"
                    >{{ item }}</el-checkbox
                  >
                </el-checkbox-group></div>
              </div>
              <div class="authorization-item">
                <div class="limit-title">可添加权限：</div>
                     <div class="limit-item">
                <el-checkbox-group
                  v-model="neverLimit"
                  @change="changeNaverLimit"
                >
                  <el-checkbox
                    v-for="item in neverLimitList"
                    :label="item"
                    :key="item"
                    >{{ item }}</el-checkbox
                  >
                </el-checkbox-group>
                     </div>
              </div>
            </div>
            <!-- <el-row :gutter="20">
              <el-col :span="12">
                <div class="limit-title">已有权限：</div>
                <el-checkbox-group
                  v-model="haveLimit"
                  @change="changeHaveLimit"
                >
                  <el-checkbox
                    v-for="item in haveLimitList"
                    :label="item"
                    :key="item"
                    >{{ item }}</el-checkbox
                  >
                </el-checkbox-group>
              </el-col>
              <el-col :span="12">
                <div class="limit-title">可添加权限：</div>
                <el-checkbox-group
                  v-model="neverLimit"
                  @change="changeNaverLimit"
                >
                  <el-checkbox
                    v-for="item in neverLimitList"
                    :label="item"
                    :key="item"
                    >{{ item }}</el-checkbox
                  >
                </el-checkbox-group>
              </el-col>
            </el-row> -->
            <span slot="footer">
              <el-button v-on:click="isAuthorization = false">取 消</el-button>
              <el-button type="primary" @click="onChangeLimit">确 定</el-button>
            </span>
          </el-dialog>
        </el-col>
      </el-row>
    </el-main>
  </div>
</template>

<script>
import {
  Permiss_FourModuleList,
  RolesNameList,
  DiscriptionList,
  AddModule,
  ModifyModule,
  AddPermission,
  ModifyRolesName,
  ModifyDiscription,
  Cancel_Authorization,
} from "@/api/system";
export default {
  name: "permissions",
  props: {},
  inject: ["reload"],
  data() {
    return {
      showLimit: false,
      department: "人力资源部",
      deparlist: [
        { value: "人力资源部", lable: "人力资源部" },
        { value: "技术部", lable: "技术部" },
        { value: "SMT部", lable: "SMT部" },
        { value: "PMC部", lable: "PMC部" },
        { value: "品质部", lable: "品质部" },
        { value: "财务部", lable: "财务部" },
        { value: "采购部", lable: "采购部" },
        { value: "总经办", lable: "总经办" },
        { value: "客户服务部 ", lable: "客户服务部 " },
        { value: "配套加工部", lable: "配套加工部" },
        { value: "总装一部", lable: "总装一部" },
        { value: "总装二部", lable: "总装二部" },
      ],
      userName: "",
      rolesNav: [], //导航模块名字
      PermissList: [], //权限组
      tableData: [], //权限名
      //导航弹框名字
      titleName: "",
      isVisible: false,
      groupName: "", //新增模块，权限组，权限名
      groupLists: [], //已有模块，权限组，权限名
      isModify: false,
      chooese: "", //选中要修改的值
      new_groupName: "", //修改的值
      //授权弹框
      isAuthorization: false,
      chooeseItem: "", //选择要授权的行信息
      haveLimit: [], //选中已有权限
      haveLimitList: [], //已有权限列表
      neverLimit: [], //选中未有权限
      neverLimitList: [], //未有权限列表
    };
  },
  components: {},
  computed: {},
  watch: {},
  methods: {
    //权限组导航
    getPermissNav() {
      Permiss_FourModuleList().then((res) => {
        //   console.log(res.data, "5555");
        let arr = [],
          obj = {};
        res.data.Data.forEach((item) => {
          RolesNameList(item).then((res) => {
            obj = {
              title: item,
              roles: res.data.Data,
            };
            arr.push(obj);
          });
        });
        this.PermissList = arr;
      });
    },
    //选择权限组
    handleSelect(key, keyPath) {
      // console.log(keyPath[0], keyPath[1]);
      this.rolesNav = keyPath;
      this.query();
    },
    //查询数据
    query() {
      if (this.userName == "") {
        this.userName = null;
      }
      let param = {
        FourModule: this.rolesNav[0],
        RolesName: this.rolesNav[1],
        Department: this.department,
        UserName: this.userName,
      };
      DiscriptionList(param).then((res) => {
        // console.log(res,666);
        this.tableData = res.data.Data;
        this.showLimit = true;
      });
    },
    //更多操作
    moreAction(row) {
      // console.log(row);
      this.isAuthorization = true;
      this.chooeseItem = row;
      let arr1 = [],
        arr2 = [];
      row.Table.forEach((item) => {
        if (item.Discr) {
          arr1.push(item.Discription);
        } else {
          arr2.push(item.Discription);
        }
      });
      this.haveLimitList = arr1;
      this.haveLimit = arr1;
      this.neverLimitList = arr2;
    },
    changeHaveLimit(val) {
      // console.log(val, 111);
      this.haveLimit = val;
    },
    changeNaverLimit(val) {
      // console.log(val, 222);
      this.neverLimit = val;
    },
    //授权
    onChangeLimit() {
      let deleteList = [];
      deleteList = this.haveLimitList.filter((item) => {
        return this.haveLimit.indexOf(item) === -1;
      });
      // console.log(deleteList);
      let param = {
        FourModule: this.rolesNav[0],
        New_RolesName: this.rolesNav[1],
        Department: this.chooeseItem.Department,
        UserName: this.chooeseItem.UserName,
        UserID: this.chooeseItem.UserNum,
        Position: this.chooeseItem.Role,
        Delete: JSON.stringify(deleteList),
        Add: JSON.stringify(this.neverLimit),
      };
      Cancel_Authorization(param).then((res) => {
        // console.log(res.data);
        if (res.data.Data.Result) {
          this.isAuthorization = false;
          this.haveLimit = [];
          this.neverLimit = [];
          this.tableData = [];
          this.query();
        }
      });
    },
    //打开新增权限组/权限名/模块
    onOpen(val) {
      this.isVisible = true;
      this.titleName = val;
      this.groupName = "";
      this.chooese = "";
      this.new_groupName = "";
      if (val == "权限组") {
        this.PermissList.forEach((item) => {
          if (item.title == this.rolesNav[0]) {
            this.groupLists = item.roles;
          }
        });
      } else if (val == "权限名") {
        // console.log(this.tableData);
        let arr = [];
        this.tableData[0].Table.forEach((item) => {
          arr.push(item.Discription);
        });
        this.groupLists = arr;
      } else if (val == "模块") {
        // console.log(this.PermissList);
        let arr = [];
        this.PermissList.forEach((item) => {
          arr.push(item.title);
        });
        this.groupLists = arr;
      }
    },
    //添加权限组
    onAdd() {
      let param;
      if (this.titleName == "模块") {
        AddModule(this.groupName).then((res) => {
          this.$message.success(res.data.Data.Message);
          this.isVisible = false;
          this.isModify = false;
          this.reload();
        });
      } else {
        if (this.titleName == "权限组") {
          param = {
            fourModule: this.rolesNav[0],
            rolesName: this.groupName,
          };
        } else {
          param = {
            fourModule: this.rolesNav[0],
            rolesName: this.rolesNav[1],
            discription: this.groupName,
          };
        }
        // console.log(param);
        AddPermission(param).then((res) => {
          if (res.data.Data.Result) {
            this.$message.success(res.data.Data.Message);
            this.isVisible = false;
            this.isModify = false;
            this.query();
          } else {
            this.Message.warning(res.data.Data.Message);
          }
        });
      }
    },
    //选择修改的权限组、权限名
    onChooese(val) {
      this.isModify = true;
      this.chooese = val;
      this.new_groupName = val;
    },
    //添加权限名
    onModify() {
      this.isModify = true;
      if (this.titleName == "权限组") {
        let param1 = {
          rolesName: this.chooese,
          new_rolesName: this.new_groupName,
        };
        ModifyRolesName(param1).then((res) => {
          if (res.data.Data.Result) {
            this.$message.success(res.data.Data.Message);
            this.isVisible = false;
            this.isModify = false;
            this.query();
          } else {
            this.$message.warning(res.data.Data.Message);
          }
        });
      } else if (this.titleName == "权限名") {
        let param2 = {
          rolesName: this.rolesNav[1],
          discription: this.chooese,
          new_discription: this.new_groupName,
        };
        ModifyDiscription(param2).then((res) => {
          if (res.data.Data.Result) {
            this.$message.success(res.data.Data.Message);
            this.isVisible = false;
            this.isModify = false;
            this.query();
          } else {
            this.$message.warning(res.data.Data.Message);
          }
        });
      } else if (this.titleName == "模块") {
        let param3 = {
          fourModule: this.chooese,
          new_fourModule: this.new_groupName,
        };
        ModifyModule(param3).then((res) => {
          if (res.data.Data.Result) {
            this.$message.success(res.data.Data.Message);
            this.isVisible = false;
            this.isModify = false;
            this.reload();
          } else {
            this.$message.warning(res.data.Data.Message);
          }
        });
      }
    },
  },
  created() {},
  mounted() {
    this.getPermissNav();
  },
  beforeCreate() {},
  beforeMount() {},
  beforeUpdate() {},
  updated() {},
  beforeDestroy() {},
  destroyed() {},
  activated() {},
};
</script>

<style lang='less' scoped>
@import url("~@/assets/style/color.less");
.limit-left-box {
  max-height: 76vh;
  overflow: auto;
}
.limit-left-box::-webkit-scrollbar {
  display: none;
}
.title {
  font-size: 24px;
  padding: 10px 0;
}
.titleName {
  font-size: 16px;
  font-weight: bold;
  padding-bottom: 10px;
}
.titleName span {
  margin-right: 20px;
}
.main-box {
  padding: 10px;
}
.group-add {
  padding-top: 10px;
}
.group-add span {
  font-weight: bold;
}
//左侧权限导航
.el-menu,
.el-menu-item {
  background-color: rgba(239, 242, 247, 1);
}
.el-submenu .el-menu-item {
  min-width: 150px;
  padding: 0 8px;
}
.el-menu-item:hover {
  background-color: rgba(228, 236, 247, 1);
  border-right: 2px solid @primary-color;
}
.el-menu-item.is-active {
  color: @primary-color;
  background-color: rgba(228, 236, 247, 1);
  border-right: 2px solid @primary-color;
}
.el-menu-item,
.el-submenu_title {
  height: 40px;
  line-height: 40px;
}
//右边权限头部
.limit-box {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 10px;
  border: 1px solid #c9d3dd;
  border-bottom: none;
}
.nav-path {
  font-size: 16px;
}
.nav-more .el-input,
.nav-more .el-select {
  margin-right: 10px;
}
//授权弹框的样式
.authorization-box {
  display: flex;
  border: 1px solid #e4ecf7;
}
.authorization-item {
  width: 50%;
}
.limit-title {
  padding: 10px;
  border-bottom: 1px solid #e4ecf7;
}
.limit-item {
  padding: 10px;
}
</style>