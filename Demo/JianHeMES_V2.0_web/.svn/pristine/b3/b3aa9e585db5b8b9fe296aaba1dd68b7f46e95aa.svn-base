<!--- 权限管理 --->
<template>
  <div class="main-box">
    <div class="title">权限管理</div>
    <el-row :gutter="20">
      <el-col :span="4">
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
            ></el-input>
            <el-button type="primary" size="mini" @click="query">查询</el-button>
            <el-button type="primary" plain size="mini" @click="onOpenRolesList">新增权限组</el-button>
            <el-button type="primary" plain size="mini" @click="onOpenRolesName">新增权限名</el-button>
          </div>
        </div>
        <el-table :data="tableData" border style="width: 100%">
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
                >更多操作</el-button
              >
            </template>
          </el-table-column>
        </el-table>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import {
  Permiss_FourModuleList,
  RolesNameList,
  DiscriptionList,
} from "@/api/system";
export default {
  name: "permissions",
  props: {},
  data() {
    return {
      showLimit: false,
      department: "技术部",
      deparlist: [],
      userName: null,
      rolesNav: [],
      PermissList: [],
      tableData: [],
    };
  },
  components: {},
  computed: {},
  watch: {},
  methods: {
    //选择权限
    handleSelect(key, keyPath) {
      console.log(keyPath[0], keyPath[1]);
      this.rolesNav = keyPath;
      DiscriptionList(
        keyPath[0],
        keyPath[1],
        this.department,
        this.userName
      ).then((res) => {
        console.log(res);
        this.tableData = res.data.Data;
        this.showLimit = true;
      });
    },
    //查询数据
    query() {},
    //更多操作
    moreAction(row) {},
    //打开新增权限组
    onOpenRolesList(row) {},
    //打开新增权限名
    onOpenRolesName(row) {},
  },
  created() {},
  mounted() {
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
.title {
  font-size: 24px;
  padding: 10px 0;
}
.main-box {
  padding: 10px;
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
.nav-more .el-input,.nav-more .el-select{
  margin-right: 10px;
}
</style>