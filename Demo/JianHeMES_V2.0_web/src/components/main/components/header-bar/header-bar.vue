<template>
  <div class="header-bar">
   <sider-trigger :collapsed="collapsed" icon="md-menu" @on-change="handleCollpasedChange"></sider-trigger>
    <custom-bread-crumb v-if="flag" show-icon :list="breadCrumbList"></custom-bread-crumb>
    <div class="custom-content-con">
      <slot></slot>
    </div>
  </div>
</template>
<script>
import siderTrigger from './sider-trigger'
import customBreadCrumb from './custom-bread-crumb'
import './header-bar.less'
export default {
  name: 'HeaderBar',  
  data() {
    return {
      value:false,
      // flag: false
    };
  },
  components: {
    siderTrigger,
    customBreadCrumb
  },
  props: {
    flag: Boolean,
    collapsed: Boolean
  },

  computed: {
    breadCrumbList () {
      return this.$store.state.app.breadCrumbList
    }
  },
  methods: {
    handleCollpasedChange(state) {
      this.$emit("on-coll-change", state);
    }
  }
}
</script>
