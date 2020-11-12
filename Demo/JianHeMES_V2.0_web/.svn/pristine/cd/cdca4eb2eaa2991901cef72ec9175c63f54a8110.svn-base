<!---  --->
<template>
  <div>
    <cHeader :active="active" ></cHeader>
    <!-- 操作栏 -->
    <div class="se_name">
      <el-select size="small" v-model="check_type" placeholder="请选择检查类型" @change="onChangeType" style="width:150px;">
          <el-option label="日检" value="日检"></el-option>
          <el-option label="周检" value="周检"></el-option>
          <el-option label="巡检" value="巡检"></el-option>
      </el-select>
      <cSelect :source="active" ref='sel'></cSelect>
      <el-select size="small" v-model="query_type" placeholder="请选择查询类型.." style="width:150px;">
          <template v-if="check_type!='周检'">
              <el-option label="按日查" value="日"></el-option>
              <el-option label="按月查" value="月"></el-option>
          </template>
          <template v-if="check_type=='周检'">
              <el-option label="按周查" value="周"></el-option>
              <el-option label="按月查" value="月"></el-option>
          </template>
      </el-select>
      <el-date-picker size="small" class="btn" v-if="query_type=='日'" v-model="select_date" value-format="yyyy-MM-dd"
                      type="date"
                      placeholder="选择日期" style="width:150px;" >
      </el-date-picker>
      <el-date-picker size="small" class="btn" v-if="query_type=='月'||query_type=='周'" v-model="select_month" value-format="yyyy-MM"
                      type="month"
                      placeholder="选择月份" style="width:150px;">
      </el-date-picker>
      <el-select size="small" v-if="query_type=='周'" v-model="select_week" placeholder="请选择周" style="width:150px;">
          <el-option label="第一周" value="1"></el-option>
          <el-option label="第二周" value="2"></el-option>
          <el-option label="第三周" value="3"></el-option>
          <el-option label="第四周" value="4"></el-option>
          <el-option label="第五周" value="5"></el-option>
          <el-option label="第六周" value="6"></el-option>
      </el-select>
      <el-button size="small" type="primary" v-on:click="onQueryData" class="btn">查询</el-button>
    </div>
     <!-- @*表格*@ -->
    <div class="table-height">
        <vxe-table border
                   ref="xTable"
                   size="mini"
                   height="600px"
                   align="center"
                   :data="tableData" stripe>
            <vxe-table-column type="seq" title="序号" width="50"></vxe-table-column>
            <vxe-table-column title="检查时间" field="Date" width="100"></vxe-table-column>
            <vxe-table-column title="部门" field="Department" width="100"></vxe-table-column>
            <vxe-table-column title="位置" field="Group" width="100"></vxe-table-column>
            <vxe-table-column title="区域号" field="District" width="60"></vxe-table-column>
            <vxe-table-column title="责任人" field="ResponsiblePerson" width="100"></vxe-table-column>
            <vxe-table-column title="7S扣分类型" field="PointsDeducted_Type" width="120"></vxe-table-column>
            <vxe-table-column title="扣分参考标准" field="PointsDeducted_Item" min-width="240">
                <template v-slot="{ row }">
                    <div v-for="(item,index) in row.PointsDeducted_Item" :key="index">{{index+1}}.{{item}}</div>
                </template>
            </vxe-table-column>
            <vxe-table-column title="问题描述" field="ProblemDescription" width="100"></vxe-table-column>
            <vxe-table-column title="7S扣分" field="PointsDeducted" width="80">
                <template v-slot="{ row }">
                    <div v-if="row.PointsDeducted!=''" class="row-pointsDeducted">{{-row.PointsDeducted}}</div>
                </template>
            </vxe-table-column>
            <vxe-table-column title="重复出现扣分" field="RepetitionPointsDeducted" width="80">
                <template v-slot="{ row }">
                    <div v-if="row.RepetitionPointsDeducted!=null" class="row-pointsDeducted">{{-row.RepetitionPointsDeducted}}</div>
                </template>
            </vxe-table-column>
            <vxe-table-column title="限期未整改扣分" field="RectificationPoints" width="80">
                <template v-slot="{ row }">
                    <div v-if="row.RectificationPoints!=null" class="row-pointsDeducted">{{-row.RectificationPoints}}</div>
                </template>
            </vxe-table-column>
            <vxe-table-column title="改善前" field="BeforeImprovement" width="240">
                <template v-slot="{ row,rowIndex }">
                    <div class="img-upload">                              
                        <lookImage :ImageList="row.BeforeImprovement" :row="row" :rowIndex="rowIndex" type="1" @fatherMethod="onDelete"></lookImage>
                    </div>
                </template>
            </vxe-table-column>
            <vxe-table-column title="改善后" field="AfterImprovement" width="240">
                <template v-slot="{ row,rowIndex  }">
                    <div class="img-upload" v-if="row.Rectification_Confim==null">
                        <lookImage :ImageList="row.AfterImprovement" :row="row" :rowIndex="rowIndex" type="2" @fatherMethod="onDelete"></lookImage>
                        <el-button v-if="check_type!='日检'&&$limit('整改后上传7S图片')" size="mini" type="primary" v-on:click="addImg(row,rowIndex)">添加照片</el-button>
                    </div>
                </template>
            </vxe-table-column>
            <vxe-table-column title="限期整改时间" field="RectificationTime" width="160">
                <template v-slot="{ row }">
                    <el-date-picker size="small" v-model="row.RectificationTime" value-format="yyyy-MM-dd"
                                    type="date"
                                    style="width:130px;margin-bottom:10px"
                                    placeholder="选择限期整改日期"
                                    :disabled="!$limit('修改限期整改时间')">
                    </el-date-picker>
                    <el-button size="mini" :disabled="!$limit('修改限期整改时间')" plain type="primary" v-on:click="onSaveTime(row)">修改</el-button>
                </template>

            </vxe-table-column>
            <vxe-table-column title="整改结果" field="Rectification_Confim" width="120">
                <template v-slot="{ row}">
                    <template v-if="row.Rectification_Confim==null&&$limit('审核7S整改结果')">
                        <el-select size="mini" v-model="select_rectification_confim" placeholder="请选择..." style="width:100px;margin-bottom:10px;">
                            <el-option label="通过" value="true"></el-option>
                            <el-option label="不通过" value="false"></el-option>
                        </el-select>
                        <el-button size="mini" plain type="primary" v-on:click="onRectificationConfim(row)">确认</el-button>
                    </template>
                    <template v-if="row.Rectification_Confim!=null">
                        {{row.Rectification_Confim?'通过':'不通过'}}
                    </template>
                </template>
            </vxe-table-column>
            <vxe-table-column title="操作" width="100" field="action">
                <template v-slot="{ row}">
                    <el-button size="mini" plain type="danger" v-on:click="onRemoveRow(row)">删除</el-button>
                </template>
            </vxe-table-column>
        </vxe-table>
    </div>
    <!--上传照片-->
    <el-dialog v-bind:visible.sync="dialogVisible" width="500px">
        <uploadImage :actionurl="'/KPI_Api/ImageUpload'" :fileList="fileList" @selectFile="selectFile" @handlePictureCardPreview="handlePictureCardPreview" 
        @handleRemove="handleRemove" @onCancel="onCancel" @uploadFile="uploadFile"></uploadImage>      
    </el-dialog>
    <el-dialog v-bind:visible.sync="showVisible">
        <img width="100%" :src="dialogImageUrl" alt="">
    </el-dialog>
  </div>
</template>

<script>
import cHeader from "./page-components/_kpi_7s_header";
import cSelect from "./page-components/_select_component";
import {QueryDailyWeek,QueryDailyMonth,DeleteImage,DetailUpImage,DetailApprove,DetailModifyDate,DetailDeleteDate} from "@/api/hr/kpi-7s";
import lookImage from "./page-components/_lookimage";
import uploadImage from "./page-components/_upload_image";
export default {
  name: 'KPI_7S_Detail',
  props: {},
  data() {
    return {
      active:'7S班组扣分详细表',
      check_type:'',
      query_type:'',   //查询类型（按天/按月）
      select_date:'',
      select_month:'',
      select_week:'',
      tableData:[],
      fileList: [],
      dialogVisible: false, //控制图片上传弹框
      showVisible: false,
      dialogImageUrl: '',
      //整改结果选择
      select_rectification_confim: '',
    };
  },
  components: {
    cHeader,
    cSelect,
    lookImage,
    uploadImage,
  },
  computed: {},
  watch: {},
  methods: {
    //选取文件方法
    selectFile(file) {
        this.files.push(file.raw);
    },
    //查看临时图片
    handlePictureCardPreview(file) {
        this.dialogImageUrl = file.url;
        this.showVisible = true;
    },
    //移除临时图片
    handleRemove(file) {
        for (let i in this.files) {
            if (this.files[i].uid == file.uid) {
                this.files.splice(i, 1)
            }
        }
    },
    //取消上传
    onCancel() {
        this.fileList = [];
        this.dialogVisible = false;
    },
    //确认上传图片
    uploadFile() {
        this.$loading({
            lock: true,
            text: '上传ing...',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
        });
        let fd = new FormData();
        fd.append('department', this.uploadRow.Department)
        fd.append('position', this.uploadRow.Group)
        fd.append('check_date', this.uploadRow.Date)
        fd.append('check_Type', this.check_type)
        fd.append('district', this.uploadRow.District)
        fd.append('pointsDeducted_Type', this.uploadRow.PointsDeducted_Type)
        fd.append('uploadType', '改善后')
        let j = 0;
        this.files.forEach(item => { fd.append("UploadFile_Ingredients" + j, item); j++; })
        DetailUpImage(fd).then(res => {
            if (res.data == true) {
                this.$message.success('图片上传成功！');
                this.dialogVisible = false;
                this.onQueryData();
                this.files = this.fileList = [];
                this.$loading().close();
            } else {
                this.$message.error('图片上传失败');
                this.dialogVisible = false;
                this.$loading().close();
            }
        })
    },
    //添加图片
    addImg(row, rowIndex) {
        this.dialogVisible = true;
        this.files = []
        this.fileList = [];
        this.uploadRow = row;
        this.uploadIndex = rowIndex;
    },
    //选择
    onChangeType() {
        //this.$refs.xTable.resetColumn();
        //this.query_type = '';
    },
    //提示
    onTip() {
        if (this.check_type == '') {
            this.$message.warning('请选择检查类型!')
            return false;
        } else if (this.$refs.sel.select_department == null) {
            this.$message.warning('请选择部门！');
            return false;
        }
        else if (this.query_type == '') {
            this.$message.warning('请选择查询类型!')
            return false;
        } else if (this.select_date == '' && this.select_month == '' && this.select_week == '') {
            this.$message.warning('请选择日期!')
            return false;
        } else if (this.check_type == '周检' && this.query_type == '周' && this.select_week == '') {
            this.$message.warning('请选择第几周!')
            return false;
        }
        else {
            return true;
        }
    },
    //初始化表头
    onCreatTableColumn(val) {
        if (val == '日检') {
            this.$refs.xTable.hideColumn(this.$refs.xTable.getColumnByField('RectificationTime'));
            this.$refs.xTable.hideColumn(this.$refs.xTable.getColumnByField('Rectification_Confim'));
            this.$refs.xTable.hideColumn(this.$refs.xTable.getColumnByField('RepetitionPointsDeducted'));
            this.$refs.xTable.hideColumn(this.$refs.xTable.getColumnByField('RectificationPoints'));
        }
        if (!this.$limit('删除班组扣分数据')) {
            this.$refs.xTable.hideColumn(this.$refs.xTable.getColumnByField('action'));
        }
    },
    //查询数据
    onQueryData() {
        if (this.onTip()) {
            this.tableData=[]
            let date;
            if (this.check_type != '周检') {
                this.select_week = ''
            }
            if (this.query_type == '日') {
                date = this.select_date
            }
            if (this.query_type == '周' || this.query_type == '月') {
                date = this.select_month
            }
            if (this.query_type == '日' || this.query_type == '周') {
                let param = {
                    department: this.$refs.sel.select_department,
                    position: this.$refs.sel.select_position,
                    district: this.$refs.sel.select_district,
                    check_Type: this.check_type,
                    date: date,
                    week: this.select_week
                }
                QueryDailyWeek(param).then(res => {
                  res.data.Data.forEach(item => {
                    item.BeforeImprovement=this.ProcessingPath(item.BeforeImprovement)  
                    item.AfterImprovement=this.ProcessingPath(item.AfterImprovement)                 
                  });
                  if (res.data.Data.length == 0) {
                      this.$message.info('暂无记录！');
                  } else {
                      this.tableData = res.data.Data;
                      this.onCreatTableColumn(this.check_type);
                  }
                })
            }
            if (this.query_type == '月') {
                let param = {
                    department: this.$refs.sel.select_department,
                    position: this.$refs.sel.select_position,
                    district: this.$refs.sel.select_district,
                    check_Type: this.check_type,
                    date: date
                }
                QueryDailyMonth(param).then(res => {                
                  res.data.Data.forEach(item => {
                    item.BeforeImprovement=this.ProcessingPath(item.BeforeImprovement)  
                    item.AfterImprovement=this.ProcessingPath(item.AfterImprovement)                 
                  });
                    if (res.data.Data.length == 0) {
                        this.$message.info('该筛选范围暂无数据记录！');
                    } else {
                        this.tableData = res.data.Data;
                        this.onCreatTableColumn(this.check_type);
                    }
                })
            }
        }
    },
    //处理图片路径
    ProcessingPath(data){
      let urlArr =[]
      data.forEach(item=>{
          urlArr.push(this.$loadPath+item+"?"+Math.random())//添加随机数，防止删除后出现图片缓存的问题
      })
      return urlArr;
    },
    //删除图片
    onDelete(url,row,rowIndex,type) {
        if (this.$limit('删除7S图片')) {
            if (type == 1 && row.AfterImprovement.length > 0) {
                this.$message.error('改善后图片已上传，无法删除改善前图片！')
                return;
            }
            let imageUrl =url.split('//')
            let iUrl=imageUrl[2].split('?')
            let uploadType=''
            if(type==1){
                uploadType='改善前'
            }else{
                uploadType='改善后'
            }
            this.$confirm('确认删除该图片？')
                .then(_ => {
                    this.uploadIndex = rowIndex;
                    let param = {
                        'path': iUrl[0],
                        'department': row.Department,
                        'position': row.Group,
                        'check_date': row.Date,
                        'check_Type': this.check_type,
                        'district': row.District,
                        'pointsDeducted_Type': row.PointsDeducted_Type,
                        'uploadType':uploadType
                    }
                    this.$loading({
                        lock: true,
                        text: '删除ing...',
                        spinner: 'el-icon-loading',
                        background: 'rgba(0, 0, 0, 0.7)'
                    });
                    DeleteImage(param).then(res => {
                        if (res.data = "删除成功") {
                            this.onQueryData();
                            this.$loading().close();
                        }
                    })
                }).catch(_ => {
                    this.$loading().close();
                }
                );
        } else {
            this.$message.error('您暂无删除图片的权限！')
        }
    },
    //整改结果
    onRectificationConfim(row) {
        alert(123)
        let param = {
            id: row.Id,
            result: this.select_rectification_confim
        }
        DetailApprove(param).then(res => {
            if (res.data.Result == true) {
                this.onQueryData();
                this.$message.success('审核成功！');
            }else{
                this.$message.warning(res.data.Message);
            }
        })
    },
    //保存限期整改时间
    onSaveTime(row) {
        let obj ={ id: row.Id, time: row.RectificationTime }
        DetailModifyDate(obj).then(res => {
            if(res.data.Result==true){
                this.$message.success(res.data.Message);
            }else{
                this.$message.warning(res.data.Message);                
            }    
        })
    },
    //删除整行数据
    onRemoveRow(row) {
        this.$confirm('确认删除该条记录？').then(_ => {
            this.$loading({
                lock: true,
                text: '删除ing...',
                spinner: 'el-icon-loading',
                background: 'rgba(0, 0, 0, 0.7)'
            });
            DetailDeleteDate(row.Id).then(res => {
                if (res.data.Result ==true) {
                    this.onQueryData();
                    this.$loading().close();
                    this.$message.success(res.data.Message);
                }else{
                    this.$message.warning(res.data.Message); 
                }
            })
        }).catch(_ => {
            this.$loading().close();
        });
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
  activated() {
    if('date' in this.$route.params){
      let _this =this.$route.params
      this.check_type=_this.check_type,
      this.$refs.sel.select_department=_this.department
      this.$refs.sel.select_position=_this.position,
      this.$refs.sel.select_district=_this.district
      if (_this.query_type == '日') {
          this.select_date=_this.date
      };
      if (_this.query_type == '周' || _this.query_type == '月') {
          this.select_month=_this.date
      };
      this.query_type=_this.query_type
      this.select_week =_this.week
      this.onQueryData()
    }
  },
}
</script>

<style lang='less' scoped>
@import url('~@/assets/style/color.less');
@import url('./kpi7s.less');
@import url('~@/assets/style/vxe.css');
</style>