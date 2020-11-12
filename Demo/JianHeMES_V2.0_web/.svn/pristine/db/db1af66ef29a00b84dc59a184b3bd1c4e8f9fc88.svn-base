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
  name: "daily-excel",
  props: ["excelData","time","select_check_type"],
  data() {
    return {
      bookType: "xlsx",
      filename: "export-demo",
      sheet: [      
        {
          title: "",
          multiHeader: [],
          table: [],
          keys: [],
          merges: [],
          colWidth: [],
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
                bold:null,
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
      this.sheet[0].title=str+"7S日/周/巡检汇总表"//标题
      this.sheet[0].sheetName=str+"7S汇总表"//工作簿名称
      let day= this.getMonthDay(dd.getFullYear(),dd.getMonth() + 1)
      this.sheet[0].multiHeader=[]
      this.sheet[0].keys=[]
      if (this.select_check_type == '周检') {  
            this.sheet[0].cellStyle[0].font.bold=false 
            alert(this.sheet[0].cellStyle[0].font.bold)
            let len;   
            let arr=["部门", "位置","区域号"]
           let keyarr =["Department","Position","District"]            
            for (let i = 0; i < this.excelData.length; i++) {
                len = this.excelData[0].Week.length;
            }
            for (let index = 0; index < len; index++) {
                let i = index + 1;
                arr.push('第' + i + '周')
                keyarr.push('data' + index)               
            }
            arr.push("扣分合计")
            keyarr.push("GradeSum")
            this.sheet[0].multiHeader.push(arr);
            this.sheet[0].keys=keyarr; 
        } else {
          this.sheet[0].cellStyle[0].font.bold=true
          alert(this.sheet[0].cellStyle[0].font.bold)
          let arr=["部门", "位置","区域号"]
          let keyarr =["Department","Position","District"]
          for(let i=1;i<=day;i++){
            arr.push(i+"日")
            keyarr.push('data'+i)
          } 
          arr.push("扣分合计")     
          keyarr.push("GradeSum")   
          this.sheet[0].multiHeader.push(arr);
          this.sheet[0].keys=keyarr;
      }    
    },
    getMonthDay(year, month) {
      let days = new Date(year, month + 1, 0).getDate()
      return days      
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