<!--- 打印机设置 --->
<template>
  <div class="print">
    <div class="flex-item">
      <label>选打印机</label>
      <div>
        <el-select
          v-model="printSelect"
          clearable
          placeholder="选择打印机"
          size="medium"
        >
          <el-option
            v-for="item in printOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          >
          </el-option>
        </el-select>
      </div>
    </div>
    <div class="flex-item" v-show="printSelect != '0' && printSelect != ''">
      <label>打印浓度</label>
      <div>
        <el-slider
          :max="30"
          :min="-30"
          size="medium"
          class="nongduSlider"
          v-model="nongDu"
        ></el-slider>
      </div>
    </div>
    <div class="flex-item" v-show="printSelect != '0' && printSelect != ''">
      <label>打印数量</label>
      <div>
        <el-input-number
          v-model.trim="pageCount"
          size="medium"
          :min="1"
          :max="5"
          clearable
        >
        </el-input-number>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "printSetting",
  props: [],
  data() {
    return {
      printSelect: "",
      printOptions: [
        {
          value: "0",
          label: "选择打印机",
        },
        {
          value: "172.16.99.230", //内部打印机
          label: "230",
        },
        {
          value: "172.16.99.231", //一楼仓库打印机
          label: "231",
        },
        {
          value: "172.16.99.239", //四楼备品备件
          label: "239",
        },
        {
          value: "172.16.99.240", //四楼外观电检/包装房
          label: "240",
        },
        {
          value: "172.16.99.241", //四楼外观电检/包装房
          label: "241",
        },
        {
          value: "172.16.99.242", //四楼外箱打包区域
          label: "242",
        },
        {
          value: "172.16.99.243", //五楼电子仓锡膏
          label: "243",
        },
        {
          value: "172.16.99.244", //二楼编带房
          label: "244",
        },
        {
          value: "172.16.99.245", //二楼办公室
          label: "245",
        },
      ],
      pageCount: 1,
      nongDu: 30,
      leftdissmenion: 100, // 左偏移量
    };
  },
  components: {},
  computed: {},
  watch: {
    printSelect(v) {
      localStorage.setItem("printIP", v);
    },
    nongDu(v) {
      localStorage.setItem("printNongDuCount", v);
    },
    pageCount(v) {
      localStorage.setItem("printPageCount", v);
    },
    leftdissmenion(v) {
      localStorage.setItem("cassoadd_leftdissmenion", v);
    },
  },
  methods: {},
  created() {},
  mounted() {
    let printIP = localStorage.getItem("printIP");
    if (printIP != null) {
      this.printSelect = printIP;
    }
    let localpage = localStorage.getItem("printPageCount");
    if (localpage != null) {
      this.pageCount = localpage;
    }
    let localnongdu = localStorage.getItem("printNongDuCount");
    if (localnongdu != null) {
      this.nongDu = +localnongdu;
    }

    let localleftyl = localStorage.getItem("cassoadd_leftdissmenion");
    if (localleftyl != null) {
      this.leftdissmenion = +localleftyl;
    }
  },
  updated() {
    this.$emit("input", {
      ip: this.printSelect,
      pagecount: this.pageCount,
      port: 9101,
      concentration: this.nongDu,
      leftdissmenion: this.leftdissmenion,
    });
  },
};
</script>

<style lang='less' scoped>
.flex-item {
  display: flex;
  align-items: center;
  margin: 10px 0;
  justify-content: center;
}

.flex-item > label {
  margin: 0 10px 0 0;
  height: 36px;
  line-height: 36px;
}

.flex-item > div {
  margin: 0 0 0 10px;
}

.flex-item .el-input__inner,
.nongduSlider {
  width: 200px;
}
</style>