<template>
  <div class="user-dropdown">
    <el-dropdown placement="bottom-start">
      <div class="user-box">
        <span>用户管理</span>
        <span class="el-icon-caret-bottom"></span>
      </div>
      <el-dropdown-menu slot="dropdown">
        <el-dropdown-item>用户管理</el-dropdown-item>
        <el-dropdown-item>批量添加用户</el-dropdown-item>
        <el-dropdown-item>日志查询</el-dropdown-item>
      </el-dropdown-menu>
    </el-dropdown>
    <el-dropdown placement="bottom-start">
      <div class="user-box">
        <span class="el-icon-user"></span>
        <span>{{userInfo.Name}}</span>
        <span class="el-icon-caret-bottom"></span>
      </div>
      <el-dropdown-menu slot="dropdown">
        <el-dropdown-item>修改密码</el-dropdown-item>
        <el-dropdown-item>班组信息</el-dropdown-item>
        <el-dropdown-item>查看权限</el-dropdown-item>
        <el-dropdown-item>权限对比</el-dropdown-item>
        <el-dropdown-item @click.native="logout">退出登录</el-dropdown-item>
      </el-dropdown-menu>
    </el-dropdown>
  </div>
</template>

<script>
import "./user.less";
import { mapActions } from "vuex";
export default {
  name: "User",
  props: {
    userInfo: {
      type: Object
    },
    // messageUnreadCount: {
    //   type: Number,
    //   default: 0,
    // },
  },
  methods: {
    ...mapActions(["handleLogOut"]),
    logout() {
      this.handleLogOut().then(() => {
        this.$router.push({
          name: "login",
        });
      });
    }
  },
};
</script>
