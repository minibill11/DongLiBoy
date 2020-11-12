<!--- 设备履历上传图片 --->
<template>
  <div class="upload-box">
    <el-button type="primary" size="mini" v-on:click="onShowAddImg"
      >上传图片</el-button
    >
    <!--上传照片-->
    <el-dialog
      title="上传图片"
      v-bind:visible.sync="dialogVisibleImg"
      width="40%"
      style="text-align:left;"
    >
      <el-upload
        action="/Equipment/UploadEquipmentPicture"
        list-type="picture-card"
        multiple
        :file-list="fileList"
        :auto-upload="false"
        :on-change="selectFile"
        :on-preview="handlePictureCardPreview"
        :on-remove="handleRemove"
      >
        <i class="el-icon-plus"></i>
      </el-upload>
      <span slot="footer" class="dialog-footer">
        <el-button size="small" v-on:click="onCancelImg">取 消</el-button>
        <el-button size="small" type="primary" v-on:click="uploadFile"
          >确定上传</el-button
        >
      </span>
    </el-dialog>
  </div>
</template>

<script>
import { UploadEquipmentPicture } from "@/api/equipment";
export default {
  name: "",
  props: ["equipmentNumber"],
  inject: ["reload"],
  data() {
    return {
      //打开上传图片
      dialogVisibleImg: false,
      fileList: [],
      files: [],
    };
  },
  components: {},
  computed: {},
  watch: {},
  methods: {
    //打开上传图片
    onShowAddImg() {
      if (this.$limit("上传设备图片")) {
        this.dialogVisibleImg = true;
      } else {
        this.$message.warning("暂无权限！");
      }
    },
    //取消上传图片
    onCancelImg() {
      this.dialogVisibleImg = false;
      this.fileList = [];
      this.files = [];
    },
    //选取文件方法
    selectFile(file) {
      //console.log(file);
      this.files.push(file.raw);
    },
    //移除临时图片
    handleRemove(file) {
      //console.log(file, fileList);
      for (let i in this.files) {
        if (this.files[i].uid == file.uid) {
          this.files.splice(i, 1);
        }
      }
    },
    //查看临时图片
    handlePictureCardPreview(file) {
      this.dialogImageUrl = file.url;
      this.showVisible = true;
    },
    //上传图片
    uploadFile() {
      let tt = this.files;
      if (tt.length != 0) {
        let fd = new FormData();
        fd.append("equipmentNumber", this.equipmentNumber);
        this.files.forEach((item, j) => {
          fd.append("uploadfile" + j, item);
          j++;
        });
        // console.log(fd);
        UploadEquipmentPicture(fd)
          .then((res) => {
            //console.log(res.data.Data)
            if (res.data.Result) {
              this.$message.success(res.data.Message);
              this.dialogVisibleImg = false;
              this.files = this.fileList = [];
              this.reload();
            } else {
              this.$message.warning(res.data.Message);
            }
          })
          .catch((err) => {
            console.log(err);
          });
      } else {
        this.$message.warning("请选择需要上传的图片！");
      }
    },
  },
  created() {},
  mounted() {},
};
</script>

<style lang='less' scoped>
.upload-box{
 display: inline-block;
 width: 90px;
}
</style>