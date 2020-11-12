<!--- 运行状态的饼图 --->
<template>
  <div>
    <div :class="size" :id="id" ref="dom"></div>
  </div>
</template>

<script>
import echarts from "echarts";
import { on, off } from "@/libs/tools";
export default {
  name: "",
  props: ["id", "size", "text", "value", "chart", "radius", "color"],
  data() {
    return {
      dom: null,
      option: [],
    };
  },
  components: {},
  computed: {},
  watch: {},
  methods: {
    resize() {
      this.dom.resize();
    },
  },
  mounted() {
    var chart = this.chart;
    this.option = {
      tooltip: {
        trigger: "item",
        formatter: function (a) {
          return a["name"] + "<br/>" + a["value"] + "/" + a["data"].per; //自定义百分比
        },
      },
      //color: ["#22c49e", "#f7c73a","#ff8590"],
      color: this.color,
      legend: {
        bottom: 10,
        left: "center",
        data: chart.map((item) => item.name),
      },
      graphic: {
        elements: [
          {
            type: "text",
            left: "center", // 相对父元素居中
            top: "40%",
            style: {
              fill: "#8EA0B8",
              text: [this.value],
              fontSize: 20,
              fontWeight: 800,
              //font: '18px Arial Normal',
            },
          },
          {
            type: "text",
            left: "center", // 相对父元素居中
            top: "48%", // 相对父元素上下的位置
            style: {
              fill: "#8EA0B8",
              text: [this.text],
              fontSize: 14,
              //fontWeight: bold
              //font: '12px Arial Normal',
            },
          },
        ],
      },
      series: [
        {
          type: "pie",
          radius: this.radius,
          center: ["50%", "45%"],
          minAngle: 20, //最小角度
          startAngle: 240, //起始角度
          emphasis: {
            itemStyle: {
              shadowBlur: 10,
              shadowOffsetX: 0,
              shadowColor: "rgba(0, 0, 0, 0.5)",
            },
          },
          label: {
            normal: {
              formatter: function (a) {
                let dd, str;
                chart.map((item) => {
                  if (item.name == a.name && item.value == a.value) {
                    dd = item.per;
                  }
                });
                if (a.name.length < 4) {
                  str =
                    "{b|" + a.name + ":" + a.value + "}  \n {d|" + dd + "}  \n";
                } else {
                  str =
                    "{b|" +
                    a.name +
                    ":} \n {b|" +
                    a.value +
                    "}  \n {d|" +
                    dd +
                    "}  \n";
                }
                //str = '{b|' + a.name + ':} \n {b|' + a.value + '}  \n {d|' + dd + '}  \n';
                return str;
              },
              borderWidth: 20,
              borderRadius: 4,
              padding: [0, -10],
              rich: {
                b: {
                  //name 文字样式
                  fontSize: 14,
                  lineHeight: 20,
                  color: "#293038",
                  fontWeight: 800,
                  display: "block",
                },
                d: {
                  //value 文字样式
                  fontSize: 14,
                  lineHeight: 20,
                  color: "#8EA0B8",
                  align: "center",
                  display: "block",
                },
              },
            },
          },
          data: chart,
        },
      ],
    };
    this.$nextTick(() => {
      this.dom = echarts.init(this.$refs.dom);
      this.dom.setOption(this.option);
      on(window, "resize", this.resize);
      //this.dom.hideLoading();
    });
  },
  beforeDestroy() {
    off(window, "resize", this.resize);
  },
};
</script>

<style lang='less' scoped>
@import url("~@/assets/style/color.less");
.big {
  width: 480px;
  height: 400px;
}

.small {
  width: 350px;
  height: 350px;
}
</style>