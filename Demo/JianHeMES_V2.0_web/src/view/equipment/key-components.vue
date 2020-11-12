<!--- 关键元器件 --->
<template>
  <div>
    <div class="eq-header">关键元器件清单</div>
    <el-main class="main-box">
      <div class="equipment-index-inputcontainer">
        <el-button
          type="primary"
          size="mini"
          @click="onShowAdd"
          style="margin-right: 10px"
          >批量添加</el-button
        >
        <span v-if="addFlag">
          <el-input
            size="mini"
            type="textarea"
            rows="1"
            v-model="inputInfo"
            class="textinput"
            placeholder="表格粘贴处..."
            style="width: 200px; margin-left: 10px"
          ></el-input>
          <DownloadTemplate
            :templateName="templateName"
            :url="url"
          ></DownloadTemplate>
          <ImportExcel :on-success="onSuccess">
            <el-button plain type="primary" size="mini">导入Excel</el-button>
          </ImportExcel>
          <el-button size="mini" type="primary" plain v-on:click="onUpload"
            >确认上传</el-button
          >
          <el-button size="mini" type="info" plain v-on:click="onCancel"
            >取消</el-button
          >
        </span>
      </div>
      <div class="tableContainer_keyComponent">
        <el-table
          v-bind:data="tableData"
          border
          style="width: 100%"
          max-height="660"
        >
          <el-table-column type="index" width="60" label="序号">
          </el-table-column>
          <el-table-column prop="EquipmentName" label="设备名称">
            <template slot-scope="scope">
              <div :class="scope.row.color == 1 ? 'textRed' : ''">
                {{ scope.row.EquipmentName }}
              </div>
            </template>
          </el-table-column>

          <el-table-column prop="Descrip" label="品名"> </el-table-column>
          <el-table-column prop="Specifica" label="规格/型号">
          </el-table-column>
          <el-table-column prop="Materused" label="用途"> </el-table-column>
          <el-table-column prop="Remark" label="备注"> </el-table-column>
          <el-table-column prop="Remark" width="100" label="操作">
            <template slot-scope="scope">
              <el-button
                v-if="!addFlag"
                v-on:click="changeRow(scope.row)"
                type="text"
                size="small"
                style="text-decoration: underline"
              >
                修改
              </el-button>
              <el-button
                v-on:click="removeRow(scope.row, scope.$index)"
                type="text"
                size="small"
                style="text-decoration: underline"
              >
                移除
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
      <div class="Equipment_KeycomponentsFooter">
        <p style="font-weight: 600">关键元器件清单说明：</p>
        <p>1）建立设备关键元器件清单有助于季度、年度保养确认</p>
        <p>2）设备维修配件申购相关型号查询</p>
      </div>

      <!-- @* 修改当行弹框 *@ -->
      <el-dialog title="修改" v-bind:visible.sync="dialogVisible" width="30%">
        <div class="dialogContent">
          <el-form v-bind:model="changeForm" label-width="100px">
            <el-form-item label="设备名称" prop="EquipmentName">
              <el-input
                disabled
                v-model="changeForm.EquipmentName"
                size="small"
              ></el-input>
            </el-form-item>
            <el-form-item label="品名" prop="Descrip">
              <el-input v-model="changeForm.Descrip" size="small"></el-input>
            </el-form-item>
            <el-form-item label="规格/型号" prop="Specifica">
              <el-input v-model="changeForm.Specifica" size="small"></el-input>
            </el-form-item>
            <el-form-item label="用途" prop="Materused">
              <el-input v-model="changeForm.Materused" size="small"></el-input>
            </el-form-item>
            <el-form-item label="备注" prop="Remark">
              <el-input v-model="changeForm.Remark" size="small"></el-input>
            </el-form-item>
          </el-form>
        </div>
        <span slot="footer" class="dialog-footer">
          <el-button size="mini" v-on:click="dialogVisible = false"
            >取 消</el-button
          >
          <el-button size="mini" type="primary" v-on:click="comfirmChanged"
            >确定</el-button
          >
        </span>
      </el-dialog>
    </el-main>
  </div>
</template>

<script>
import DownloadTemplate from "_c/download-template";
import ImportExcel from "_c/import-excel";
import {
  Keyinquire,
  Equipment_EditComponet,
  Keycomponents_query,
  DeleteKeycom,
} from "@/api/equipment";
export default {
  name: "",
  props: {},
  data() {
    return {
      addFlag: false,
      dialogVisible: false,
      tableData: [], // 页面加载的表格数据
      EquipmentNum: null, //设备编号
      inputInfo: null, // 粘贴框
      changeForm: {}, // 修改表单数据
      //上传excel
      templateName: "设备模块_关键元器件清单批量上传模板.xls",
      url:
        "MES_Data/DocumentTemplateFiles/设备模块_关键元器件清单批量上传模板.xls",
    };
  },
  components: { DownloadTemplate, ImportExcel },
  computed: {},
  watch: {
    //监听批量上传粘贴的数据
    inputInfo(val) {
      if (val != null) {
        let valOfPaste = val.split("\n");
        valOfPaste.pop();
        let initDatas = [];
        valOfPaste.forEach((item, i) => {
          var items = item.split("\t");
          initDatas.push(items);
        });
        let arr = [];
        initDatas.forEach((item) => {
          let obj = {
            EquipmentName: item[0],
            Descrip: item[1],
            Specifica: item[2],
            Materused: item[3],
            Remark: item[4],
            color: 0,
          };
          arr.push(obj);
        });
        this.tableData = arr;
        //console.log(this.tableData);
      }
    },
  },
  methods: {
    // 获取表格信息
    getTableData() {
      Keyinquire({ equipmentNumber: this.EquipmentNum }).then((res) => {
        //console.log(res.data.Data)
        this.tableData = res.data.Data;
      });
    },
    // 修改当行数据
    changeRow(row) {
      if (this.$limit("修改元器件信息")) {
        this.dialogVisible = true;
        this.changeForm = row;
      } else {
        this.$message.warning("暂无权限！");
      }
    },
    // 确认修改
    comfirmChanged() {
      let postData = {
        equipmentNumber: this.EquipmentNum,
        descrip: this.changeForm.Descrip,
        specifica: this.changeForm.Specifica,
        materused: this.changeForm.Materused,
        remark: this.changeForm.Remark,
        id: this.changeForm.Id,
      };
      Equipment_EditComponet(postData).then((res) => {
        //console.log(res.data.Data)
        if (res.data.Data.mes) {
          this.$message.success("修改成功！");
          this.dialogVisible = false;
        } else {
          this.$message.error("修改失败！");
        }
      });
    },
    // 打开批量上传
    onShowAdd() {
      if (this.$limit("批量上传元器件")) {
        this.addFlag = true;
        this.tableData = [];
      } else {
        this.$message.warning("暂无权限!");
      }
    },
    //导入Excel
    onSuccess(response, file) {
      if (response.length != 0) {
        let json = response[0].data;
        delete json[0];
        // console.log(json);
        let arr = [];
        json.forEach((item) => {
          let obj = {
            EquipmentName: item["__EMPTY"],
            Descrip: item["__EMPTY_1"],
            Specifica: item["__EMPTY_2"],
            Materused: item["__EMPTY_3"],
            Remark: item["__EMPTY_4"],
            color: 0,
          };
          arr.push(obj);
        });
        this.tableData = arr;
        // console.log(this.tableData);
      }
    },
    // 确定批量上传
    onUpload() {
      this.tableData.forEach((item) => {
        item.EquipmentNumber = this.EquipmentNum;
      });
      if (this.tableData.length > 0) {
        Keycomponents_query({ inputList: this.tableData }).then((res) => {
          //console.log(res.data.Data)
          if (res.data.Data == true) {
            this.$message.success("上传成功！");
            this.addFlag = false;
            this.inputInfo = null;
            this.getTableData();
          }
          if (res.data.Data.repat == false) {
            this.$message.warning("存在重复数据，请手动删除！");
            res.data.Data.res.forEach((item) => {
              this.tableData.forEach((items) => {
                if (
                  item.Descrip == items.Descrip &&
                  item.Specifica == items.Specifica &&
                  item.Materused == items.Materused
                ) {
                  items.color = 1;
                }
              });
            });
          }
        });
      } else {
        this.$notify({
          message: "请输入数据",
          type: "warning",
        });
      }
    },
    //清空批量上传
    onCancel() {
      this.addFlag = false;
      this.inputInfo = null;
      this.getTableData();
    },
    // 移除数据
    removeRow(row, index) {
      if (this.addFlag) {
        this.tableData.splice(index, 1);
      } else {
        if (this.$limit("删除元器件清单")) {
          this.$confirm("确认删除该条记录？")
            .then((_) => {
              DeleteKeycom({ id: row.Id })
                .then((res) => {
                  //console.log(res.data.Data)
                  if (res.data.Data == true) {
                    this.tableData.splice(index, 1);
                    this.$message({
                      message: "操作成功",
                      type: "success",
                    });
                  } else {
                    this.$message({
                      message: "操作失败",
                      type: "warning",
                    });
                  }
                })
                .catch((err) => {
                  this.$message.warning("删除失败！");
                });
            })
            .catch();
        } else {
          this.$message.warning("暂无权限！");
        }
      }
    },
  },
  created() {},
  mounted() {
    // 获取地址栏的参数并根据该参数获取原器件数据信息
    this.EquipmentNum = this.$route.query.equipmenNum;
    this.getTableData();
  },
};
</script>

<style lang='less' scoped>
@import url("~@/assets/style/color.less");
@import url("./page-components/equipment.less");
</style>