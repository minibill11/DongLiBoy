<!---  --->
<template>
  <div>
     <div class="el-upload-list__item-thumbnail" v-for="(url,index) in ImageList" :key="index">
        <el-tooltip class="item" effect="dark" content="单击可预览图片，右击可删除图片" placement="top-start">
            <el-image style="width:70px;height:70px;margin:2px"
                      :preview-src-list="ImageList"
                      :src="url"
                      v-on:contextmenu.prevent="childMethod(url,row,rowIndex,type)" lazy>
            </el-image>
        </el-tooltip>
    </div>
  </div>
</template>

<script>
export default {
  name: '',
  props: ["ImageList","row","rowIndex","type"],
  data() {
    return {

    };
  },
  components: {},
  computed: {},
  watch: {},
  methods: {
      childMethod(url,row,rowIndex,type) {
        this.$emit('fatherMethod',url,row,rowIndex,type);
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

<style lang='less' scoped>
@import url('~@/assets/style/color.less');

</style>