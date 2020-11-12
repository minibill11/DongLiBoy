<template>
  <!-- 导出安全库存清单 -->
  <div>
    <ExportExcel
      :bookType="bookType"
      :filename="filename"
      :sheet="sheet"
      :on-error="onError"
    >
      <el-button plain type="primary" size="mini" @click="onExcel"
        >导出Excel</el-button
      >
    </ExportExcel>
  </div>
</template>
<script>
import ExportExcel from "_c/export-excel";
export default {
  name: "",
  props: ["tableData"],
  watch: {
    tableData(val) {
      // console.log(val, "000");
    },
  },
  data() {
    return {
      bookType: "xlsx",
      filename: "安全库存清单",
      sheet: [
        {
          title: "",
          tHeader: [
            "序号",
            "设备名称",
            "品名",
            "规格/型号",
            "料号",
            "使用寿命",
            "月平均用量",
            "安全库存量",
            "仓库存量",
            "仓库号",
            "库位号",
            "批次号",
            "有效期",
            "采购周期",
            "用途",
            "备注",
          ],
          table: [],
          keys: [
            "Id",
            "EquipmentName",
            "Descrip",
            "Specifica",
            "Material",
            "Servicelife",
            "Amount",
            "Safety_stock",
            "Existing_inventory",
            "warehouse_number",
            "location",
            "batch_number",
            "validity",
            "Purchasing_cycle",
            "Materused",
            "Remark",
          ],
          sheetName: "安全库存清单",
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
    onExcel() {
      let arr = [];
      // console.log(this.tableData, "111");
      this.tableData.forEach((item) => {
        // let detail = item.Existing_inventory_Details == "[]"?"":item.Existing_inventory_Details;
        // console.log(detail, "666");
        let obj = {
          Id: item.Id,
          EquipmentName: item.EquipmentName,
          Descrip: item.Descrip,
          Specifica: item.Specifica,
          Material: item.Material,
          Servicelife: item.Servicelife,
          Amount: item.Amount,
          Safety_stock: item.Safety_stock,
          Existing_inventory: item.Existing_inventory,
          // warehouse_number: item.Existing_inventory.img02,
          // location: item.Existing_inventory.img03,
          // batch_number: item.Existing_inventory.img04,
          // validity: item.Existing_inventory.img18,
          warehouse_number: item.Existing_inventory_Details.img02
            ? item.Existing_inventory_Details.img02
            : "",
          location: item.Existing_inventory_Details.img03
            ? item.Existing_inventory_Details.img03
            : "",
          batch_number: item.Existing_inventory_Details.img04
            ? item.Existing_inventory_Details.img04
            : "",
          validity: item.Existing_inventory_Details.img18
            ? item.Existing_inventory_Details.img18
            : "",
          Purchasing_cycle: item.Purchasing_cycle,
          Materused: item.Materused,
          Remark: item.Remark,
        };
        arr.push(obj);
      });
      // console.log(arr);
      this.sheet[0].table = arr;
    },
  },
  created() {},
  mounted() {},
};
</script>
