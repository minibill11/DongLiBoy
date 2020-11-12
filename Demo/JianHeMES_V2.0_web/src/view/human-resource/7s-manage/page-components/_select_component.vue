<!---  --->
<template>
  <div class="select_name">
      <el-select size="small" v-model="select_department" placeholder="请选择部门" style="width:150px;"> 
          <el-option v-for="item in department_options"
                     :key="item.value"
                     :label="item.label"
                     :value="item.value">
          </el-option>
     </el-select>
     <el-select size="small" v-model="select_group" placeholder="请选择班组" style="width:150px;" v-show="source=='7S评比得分汇总表'">
                <el-option v-for="item in group_options"
                           :key="item.value"
                           :label="item.label"
                           :value="item.value">
                </el-option>
    </el-select>
     <el-select size="small" v-model="select_position" placeholder="请选择位置" style="width:150px;" v-show="source!='7S日检记录查询'">
          <el-option v-for="item in position_options"
                     :key="item.value"
                     :label="item.label"
                     :value="item.value">
          </el-option>
     </el-select>
     <el-select size="small" v-model="select_district" placeholder="请选择区域号" style="width:150px;" v-show="source!='7S日检记录查询'">
         <el-option v-for="item in district_options"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value">
         </el-option>
     </el-select>
  </div>
</template>

<script>
import {regionGetDep,regionGetDis,regionGetPosi,GetGroupList,GetDistrctList} from "@/api/hr/kpi-7s";
export default {
  name: '',
  props: ["source"],
  data() {
    return {
        select_department:null,
        department_options:[],
        select_position:null,
        position_options:[],
        select_district:null,
        district_options:[],
        select_group:null,
        group_options:[],
    };
  },
  components: {},
  computed: {},
  watch: {
      select_department(val){
          if(val!=null&&this.source!='7S评比得分汇总表'){
              this.onGetPosition();             
          }
          if(val!=null&&this.source=='7S评比得分汇总表'){
               this.onGetGroup()
          }
      },
      select_position(val){
          if(val!=null){
              this.onGetDistrictData();
          }
      },
      select_group(val){
          if(val!=null){
              this.onGetPositionList()
          }
      }
  },
  methods: {
      //获取初始化下拉数据
      onGetDepartmentData() {
          regionGetDep().then(res => {
              if(this.source=="区域数据"||this.source=="7S班组扣分详细表"){//去掉第一个选项“全部位置”
                  res.data.Data.shift();
              }             
              this.department_options = res.data.Data;
          })
      },
      onGetDistrictData() {
          let obj ={ department: this.select_department, position: this.select_position }
          regionGetDis(obj).then(res => {
              this.district_options = res.data.Data;
          })
      },
      onGetPosition() {
          let obj={ department: this.select_department }
          regionGetPosi(obj).then(res => {
              this.position_options = res.data.Data;
          })
      },
       //获取班组列表
       onGetGroup() {
           if(this.source=='7S评比得分汇总表'){
               GetGroupList(this.select_department).then(res => {
                    this.group_options = res.data.Data;
                })              
           }            
       },
       //根据部门 班组 获取位置
       onGetPositionList() {
           let obj= { department: this.select_department, group: this.select_group }
           GetDistrctList(obj).then(res => {
               this.position_options = res.data.Data;
           })
       },

  },
  created() {},
  mounted() {
        this.onGetDepartmentData()
        if(this.source=="7S日检记录查询"){
            this.select_department='全部部门'
        }
  },
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
.select_name .el-select{
    margin: 5px;
}

</style>