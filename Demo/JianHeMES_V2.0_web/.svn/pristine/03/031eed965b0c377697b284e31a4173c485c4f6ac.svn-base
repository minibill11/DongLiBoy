<template>
  <div>
    <ExportExcel
      :bookType="bookType"
      :filename="filename"
      :sheet="sheet"
      :on-error="onError">
      <el-button plain type="success" size="small" @click="sendData">导出Excel</el-button>
    </ExportExcel>
  </div>
</template>
<script>
import ExportExcel from "_c/export-excel";
export default {
  name: "summarizing-excel",
  props: ["excelData","time"],
  data() {
    return {
      bookType: "xlsx",
      filename: "export-demo",
      sheet: [      
        {
          title: "",
          multiHeader: [
            ["部门", "班组","位置","区域号","日检扣分","周检扣分","","","","巡检扣分","","","","月末得分"],
            ["部门", "班组", "位置","区域号","日检扣分","周检正常扣分","周检未整改扣分","周检重复出现扣分","周检扣分合计","巡检正常扣分","巡检未整改扣分","巡检重复出现扣分","巡检扣分合计","月末得分"],
          ],
          table: [],
          keys: ["Department", "Group", "Position", "District","Grade_daily","Grade_week","Week_NotCorrected","Week_Repetition","Week_Sum","Grade_random","Random_NotCorrected","Random_Repetition","Random_Sum","TotalPoints"],
          merges: ["A2:A3","B2:B3","C2:C3","D2:D3","E2:E3","F2:I2","J2:M2","N2:N3"],
          colWidth: [16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,16],
          sheetName: "",
          globalStyle: {
            font: {
              color: { rgb: "000000" },
            },
          },
          alignment: {
              horizontal: "center",
              vertical: "left"
          },
          cellStyle: [
            {
              cell: "A1",
              font: {
                name: "宋体",
                sz: 16,
                color: { rgb: "000000" },
                bold: true,
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
    sendData(){       
      if(this.excelData.length>0){
              this.sheet[0].table=this.excelData
        }
        let dd =new Date(this.time)
        let str =dd.getFullYear()+"年"+(dd.getMonth() + 1)+"月"
        this.filename=str+"7S汇总表"//表名
        this.sheet[0].title=str+"7S汇总表"//标题
        this.sheet[0].sheetName=str+"7S汇总表"//工作簿名称
    }
  },
  created() {},
  mounted() {

  },
  watch:{
      
  }
};
</script>
<style lang='less' scoped>
 .el-button--success.is-disabled, .el-button--success.is-disabled:active, .el-button--success.is-disabled:focus, .el-button--success.is-disabled:hover {
    color: #FFF;
    background-color: #CCD3DE;
    border-color: #CCD3DE;
 }
</style>