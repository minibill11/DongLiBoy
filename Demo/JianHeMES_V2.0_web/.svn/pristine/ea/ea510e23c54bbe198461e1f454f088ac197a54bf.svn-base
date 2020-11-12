<!--- 供气系统 --->
<template>
  <div>
    <EqHeader :active="active"></EqHeader>
    <el-main class="main-box">
      <div style="display: flex">
        <div class="text-title">空压机房运行情况</div>
        <el-button
          size="mini"
          type="primary"
          @click="onChange"
          class="btn-show"
          >{{ isTable ? "空压机" : "汇总表" }}</el-button
        >
      </div>
      <iframe
        v-if="!isTable"
        class="content"
        :src="url_chart"
      ></iframe>
      <iframe
        v-if="isTable"
        class="content"
        :src="url_table"
      ></iframe>
    </el-main>
  </div>
</template>

<script>
import EqHeader from "./page-components/_eq_header";
export default {
  name: "Equipment_Gas_Supply",
  props: {},
  data() {
    return {
      active: "供气系统",
      isTable: false,
      url_chart:this.$loadPath+"/KongYaHT/KongYa/KongYaIndex2",
      url_table:this.$loadPath+"/KongYaHT/KongYa/KYDataShow2",
    };
  },
  components: { EqHeader },
  computed: {},
  watch: {},
  methods: {
    onChange() {
      if (this.isTable) {
        this.isTable = false;
      } else {
        this.isTable = true;
      }
    },
  },
  created() {},
  mounted() {},
};
</script>

<style lang='less' scoped>
@import url("~@/assets/style/color.less");
@import url("./page-components/equipment.less");
.content {
  width: 100%;
  height: 75vh;
  border: none;
}
.btn-show {
  flex: 1;
  position: absolute;
  right: 10%;
}
.text-title {
  flex: 9;
  margin: 2px;
  text-align: center;
  color: #8ea0b8;
  font-size: 24px;
  font-weight: bold;
}
</style>