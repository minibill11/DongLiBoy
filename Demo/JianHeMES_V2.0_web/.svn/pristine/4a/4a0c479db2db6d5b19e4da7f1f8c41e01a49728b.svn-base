<!--- 导出pdf --->
<template>
  <div class="component-btn">
    <el-button plain type="primary" size="mini" @click="print"
      >导出pdf</el-button
    >
    <div id="test">
         ok11
    </div>
  </div>
</template>

<script>
export default {
  name: "export_pdf",
  props: {},
  data() {
    return {
      title: "导出测试",
    };
  },
  components: {},
  computed: {},
  watch: {},
  methods: {
    print() {
    //   window.scrollTo(0, 0);
      this.getPdf(this.title,'#test');
    },
  },
  created() {},
  mounted() {},
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
</style>