<!---  --->
<template>
  <div>
      <SecondNav :active="active" :header_list="header_list"></SecondNav>
  </div>
</template>

<script>
import SecondNav from "_c/second-nav";
export default {
  name: "kpi_7s_header", 
  props: ["active"], //字符串数组写法
  data() {
    return {
        header_list: [
        {
          title: "部门评比排名",
          name: "KPI_7S_SummarizingRanking",
        },
        {
          title: "扣分标准录入",
          name: "KPI_7S_GradeStandardInput",
          limit:"录入7S参考标准"
        },
        {
          title: "数据区域录入",
          name: "KPI_7S_RegionInput",
          limit:"录入7S区域数据"
        },
        {
          title: "部门班组扣分录入",
          name: "KPI_7S_RecordInput",
          limit:"录入7S日周巡检数据"
        },
        {
          title: "7S评比得分汇总",
          name: "KPI_7S_Summarizing",
          limit:"查看7S总表数据"
        },
        {
          title: "日/周/巡检汇总表",
          name: "KPI_7S_Summarizing_Daily",
          limit:"查看7S日/周/巡检汇总表"
        },
        {
          title: "日检记录查询",
          name: "KPI_7S_DailyRecord",
        },
      ],
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
}
</script>

<style lang='less' scoped>
@import url('~@/assets/style/color.less');

</style>