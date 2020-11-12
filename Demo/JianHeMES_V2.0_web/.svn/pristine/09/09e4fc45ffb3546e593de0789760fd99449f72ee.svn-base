<!---  --->
<template>
  <div class="uploadImage_style">
      <el-upload :action="psotrr()"
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
          <el-button @click="onCancel">取 消</el-button>
          <el-button type="primary" @click="uploadFile">上传图片</el-button>
      </span>     
  </div>
</template>

<script>
export default {
  name: '',
  props: ["actionurl","fileList"],
  data() {
    return {

    };
  },
  components: {},
  computed: {},
  watch: {},
  methods: {
      psotrr(){
        return this.actionurl
      },
      selectFile(file){
          this.$emit('selectFile',file)
      },
      handlePictureCardPreview(file){
          this.$emit('handlePictureCardPreview',file)
      },
      handleRemove(file){
          this.$emit('handleRemove',file)         
      },
      onCancel(){
          this.$emit('onCancel')
      },
      uploadFile(){
          this.$emit('uploadFile')
      }
  },
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

<style  scoped>
 .dialog-footer .el-button{
    margin-top: 10px;
}
</style>