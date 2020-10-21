<!--- 设备导航页面组件 --->
<template>
  <div>
    <SecondNav :active="active" :header_list="header_list"></SecondNav>
  </div>
</template>

<script>
import SecondNav from "_c/second-nav";
export default {
  name: "eq_header",
  props: ["active"],
  data() {
    return {
      header_list: [
        {
          title: "运行状态",
          name: "Equipment_NewIndex",
        },
        {
          title: "设备台账",
          name: "Equipment_Parameter",
          limit:"查看设备台账信息"
        },
        {
          title: "产线设备",
          name: "Equipment_Production_line",
          limit:"查看产线信息"
        },
        {
          title: "点检查询",
          name: "Equipment_Tally_Query",
          limit:"查看点检表"
        },
        {
          title: "报修单查询",
          name: "Equipment_Fixbill_Query",
          limit:"查看报修单"
        },
        {
          title: "安全库存清单",
          name: "Equipment_Safety_Bill",
          limit:"查看安全库存清单"
        },
        {
          title: "指标达成率",
          name: "Equipment_Quality_Goal",
          limit:"查看指标达成率"
        },
        {
          title: "扫码点检",
          name: "Equipment_ScanCode_Check",
          limit:"扫码点检"
        },
        {
          title: "供气系统",
          name: "Equipment_Gas_Supply",
          limit:"查看空压机数据"
        },
      ]
    };
  },
  components: {
    SecondNav,
  },
  computed: {},
  watch: {},
  methods: {},
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