<template>
  <div>
    <ExportExcel
      :bookType="bookType"
      :filename="filename"
      :sheet="sheet"
      :on-error="onError"
    >
      <el-button plain type="primary" size="mini">导出Excel</el-button>
    </ExportExcel>
  </div>
</template>
<script>
import ExportExcel from "_c/export-excel";
export default {
  name: "export_excel",
  data() {
    return {
      bookType: "xlsx",
      filename: "export-demo",
      sheet: [
        {
          title: "测试表1",
          tHeader: ["数据1", "数据2", "数据3", "数据4"],
          table: [
            {
              data1: "1111",
              data2: "2222",
              data3: "3333",
              data4: "4444",
            },
          ],
          keys: ["data1", "data2", "data3", "data4"],
          sheetName: "表头名称11",
          cellStyle: [
            {
              cell: "A1",
              font: {
                sz: 14,
                color: { rgb: "ffffff" },
                bold: true,
              },
              fill: {
                fgColor: { rgb: "ff7e00" },
              },
            },
          ],
        },
        {
          title: "测试表2",
          multiHeader: [
            ["二级表头", "", "", "详细信息"],
            ["数据1", "数据2", "数据3", "详细1"],
          ],
          table: [
            {
              data1: "1111111a",
              data2: "2222222a",
              data3: "3333333a",
              data4: "4444444a",
            },
          ],
          keys: ["data1", "data2", "data3", "data4"],
          merges: ["A2:C2"],
          colWidth: [16, 16, 16, 20],
          sheetName: "表头名称22",
          globalStyle: {
            font: {
              color: { rgb: "ff7e00" },
            },
          },
          cellStyle: [
            {
              cell: "A1",
              font: {
                name: "宋体",
                sz: 14,
                color: { rgb: "ffffff" },
                bold: true,
              },
              fill: {
                fgColor: { rgb: "ff7e00" },
              },
            },
          ],
        },
      ],
    };
  },
  components: { ExportExcel },
  methods: {
    onError(err) {
      console.log(err);
    },
    exportTable() {
      this.$refs.excelExport.pikaExportExcel();
    },
  },
  created() {},
  mounted() {},
};
</script>
