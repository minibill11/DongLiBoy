<!---  --->
<template>
    <div>
        <moduleMenu active="查看标签"></moduleMenu>
    </div>
</template>

<script>
import setModuleManagement from "./page-components/module";
import moduleMenu from "./page-components/_moduleMenu";
export default {
    name: "module-manage-again-print",
    props: {},
    data() {
        return {};
    },
    components: {
        moduleMenu,
    },
    computed: {},
    watch: {},
    methods: {},
    mixins: [setModuleManagement],
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
@import url("~@/assets/style/color.less");
@import url("./page-components/module.less");
</style>