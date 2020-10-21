<template>
  <div>
    <ExportExcel
      :bookType="bookType"
      :filename="filename"
      :sheet="sheet"
      :on-error="onError">
      <el-button plain type="primary" size="small" @click="sendData">导出Excel</el-button>
    </ExportExcel>
  </div>
</template>
<script>
import ExportExcel from "_c/export-excel";
export default {
  name: "export_excel",
  props: ["excelData","time"],
  data() {
    return {
      bookType: "xlsx",
      filename: "export-demo",
      sheet: [      
        {
          title: "测试表2",
          multiHeader: [
            ["类别", "分类明细", "库龄","","","","","","","","","记录条数","备注"],
            ["类别", "分类明细", "1个月以内", "2-3个月","4-6个月","7-12个月","1-2年","2-3年","3-5年","5年以上","合计金额","记录条数","备注"],
          ],
          table: [],
          keys: ["Category", "Category_Detail", "OneMonth", "TwoMonths","FourMonths","SevenMonths","OneYear","TwoYears","ThreeYears","FiveYears","AmountTotal","RecordNum","Remark"],
          merges: ["A2:A3","B2:B3","C2:K2","L2:L3","M2:M3"],
          colWidth: [16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16],
          sheetName: "库存金额表",
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
        this.filename=str+"库存金额表"
        this.sheet[0].title="库存金额表（统计时间:"+str+"）"
        this.sheet[0].sheetName="库存金额表(统计时间"+str+")"
    }
  },
  created() {},
  mounted() {

  },
  watch:{
      excelData(){        
          if(this.excelData.length>0){
              this.sheet[0].table=this.excelData
          }
      }
  }
};
</script>
