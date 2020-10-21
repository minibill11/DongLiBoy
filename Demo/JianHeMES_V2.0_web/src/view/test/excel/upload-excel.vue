<template>
  <div>
     <ImportExcel :on-success="onSuccess">
      <el-button plain type="primary" size="mini">点击导入</el-button>
    </ImportExcel>
    <div class="code" v-if="json">
      {{json}}
    </div>
  </div>
</template>
<script>
import ImportExcel from "_c/import-excel";
export default {
  name: "upload_excel",
  data() {
    return {
     json: ''
    };
  },
    components: { ImportExcel },
  methods: {
  onSuccess (response, file) {
      this.json = JSON.stringify(response)
      console.log(response)
    }
  },
  created() {},
  mounted() {},
};
</script>
