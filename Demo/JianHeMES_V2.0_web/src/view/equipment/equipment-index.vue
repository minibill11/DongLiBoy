<!--- 设备首页-运行状态 --->
<template>
  <div>
    <eqHeader :active="active"></eqHeader>
  </div>
</template>

<script>
import eqHeader from "./page-components/_eq_header";
export default {
  name: "Equipment_NewIndex",
  props: {},
  data() {
    return {
      active: "运行状态",
    };
  },
  components: {
    eqHeader,
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
@import url("./page-components/equipment.less");
</style>