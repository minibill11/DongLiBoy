<template>
  <div ref="gantt" style="height:100%;"></div>
</template>

<script>
import "dhtmlx-gantt";
export default {
  name: "gantt",
  props: {
    tasks: {
      type: Object,
      default() {
        return { data: [] };
      }
    }
  },
  // watch: {
  //   tasks: {
  //     //深度监听，可监听到对象、数组的变化
  //     handler(val, oldVal) {
  //       console.log("新对象");
  //       console.log(val);
  //     },
  //     deep: true //true 深度监听
  //   }
  // },
  mounted: function() {
    gantt.config.readonly = true;
    gantt.config.min_column_width = 50;
    gantt.config.scale_height = 50;
    gantt.config.row_height = 24;
    gantt.config.scales = [
      { unit: "day", format: "%Y-%m-%d" },
      { unit: "hour", step: 3, format: "%H:%i" }
    ];
    gantt.templates.grid_file = function(item) {
      return "";
    };
    gantt.templates.grid_folder = function(item) {
      return "";
    };
    gantt.config.columns = [
      { name: "text", label: "车辆", tree: true, width: 200, resize: true },
      {
        name: "start_date",
        label: "开始时间",
        align: "center",
        width: 100,
        resize: true
      }
      // {
      //   name: "end_date",
      //   label: "结束时间",
      //   align: "center",
      //   width: 100,
      //   resize: true
      // }
    ];
    var secondGridColumns = {
      columns: [
        {
          name: "start_address",
          label: "起运地",
          width: 150,
          align: "center"
        },
        {
          name: "end_address",
          label: "送达地",
          width: 150,
          align: "center"
        }
      ]
    };
    gantt.config.layout = {
      css: "gantt_container",
      rows: [
        {
          cols: [
            { view: "grid", width: 300, scrollY: "scrollVer" },
            { resizer: true, width: 1 },
            { view: "timeline", scrollX: "scrollHor", scrollY: "scrollVer" },
            { resizer: true, width: 1 },
            {
              view: "grid",
              width: 300,
              bind: "task",
              scrollY: "scrollVer",
              config: secondGridColumns
            },
            { view: "scrollbar", id: "scrollVer" }
          ]
        },
        { view: "scrollbar", id: "scrollHor", height: 20 }
      ]
    };
    gantt.init(this.$refs.gantt);
    //gantt.parse(this.$props.tasks);
    gantt.clearAll();
    gantt.parse(this.tasks);
  }
};
</script>

<style lang='less'>
@import "~dhtmlx-gantt/codebase/dhtmlxgantt.css";
.gantt_grid_data .gantt_row.odd:hover,
.gantt_grid_data .gantt_row:hover {
  background-color: #d7d4fb;
}
.gantt_grid_data .gantt_row.gantt_selected,
.gantt_grid_data .gantt_row.odd.gantt_selected,
.gantt_task_row.gantt_selected {
  background-color: #d7d4fb;
}
// .gantt_tree_icon.gantt_open {
//   background-image: url("../../assets/images/icon/kai.svg");
// }

// .gantt_tree_icon.gantt_close {
//   background-image: url("../../assets/images/icon/guan.svg");
// }
</style>