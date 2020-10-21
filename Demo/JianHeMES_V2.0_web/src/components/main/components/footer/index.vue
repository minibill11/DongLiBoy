<!--- 底部版本 --->
<template>
  <div>
    <div class="bottomFooter">
      &copy;@DateTime.Now.Year - 健和光电MES
      <span v-show="screenSize>992">
        &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;指定浏览器：
        <a
          :href="baseUrl+'MES_Data/ChromeCore_1277_3.0.1.6.exe'"
        >Chrome浏览器</a>，
        &nbsp;(火狐 for Windows)
        &nbsp;
        <a :href="baseUrl+'MES_Data/Firefox Setup x64.exe'">64位</a>/
        &nbsp;
        <a :href="baseUrl+'MES_Data/Firefox Setup x86.exe'">32位</a>/
        &nbsp;
        <a :href="baseUrl+'MES_Data/Firefox Setup XP.exe'">XP版</a>
      </span>
    </div>
  </div>
</template>

<script>
import config from '@/config';
export default {
  name: "Footer",
  props: {},
  data() {
    return {
      screenSize: document.body.clientWidth,//屏幕尺寸
      baseUrl:config.baseUrl.pro
    };
  },
  components: {},
  computed: {},
  watch: {},
  methods: {},
  created() {},
  mounted() {},
  beforeCreate() {},
  beforeMount() {},
  beforeUpdate() {},
  updated() {},
  beforeDestroy() {},
  destroyed() {},
  activated() {},
};
</script>

<style lang='less' scoped>
@bg-color-bottom: #eff2f7;
.bottomFooter {
      position: fixed;
    bottom: 0;
    width: 100%;
    text-align: center;
    background: @bg-color-bottom;
    height: 30px !important;
    line-height: 30px;
    font-size: 12px;
    color: #8f9fb9;
}
</style>