<!---  --->
<template>
    <el-card class="acard">
        <div slot="header">
            <span>异常列表</span>
        </div>
        <div class="rightList" :gutter="20">
            <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12">
                <table class="table" style="width: 80%">
                    <tr>
                        <th colspan="4">箱体表面</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="2"
                                value="（箱体表面）-掉漆漏滴"
                                v-model="abnormalarea"
                            />
                            <label for="2">掉漆漏滴</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="3"
                                value="（箱体表面）-箱体颜色"
                                v-model="abnormalarea"
                            />
                            <label for="3">箱体颜色</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="4"
                                value="（箱体表面）-刮花"
                                v-model="abnormalarea"
                            />
                            <label for="4">刮花</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="5"
                                value="（箱体表面）-变形"
                                v-model="abnormalarea"
                            />
                            <label for="5">变形</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="4">螺丝/压铆</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="6"
                                value="（螺丝/压铆）-漏打螺丝"
                                v-model="abnormalarea"
                            />
                            <label for="6">漏打螺丝</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="7"
                                value="（螺丝/压铆）-不到位"
                                v-model="abnormalarea"
                            />
                            <label for="7">不到位</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="8"
                                value="（螺丝/压铆）-打滑"
                                v-model="abnormalarea"
                            />
                            <label for="8">打滑</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="9"
                                value="（螺丝/压铆）-打花"
                                v-model="abnormalarea"
                            />
                            <label for="9">打花</label>
                        </td>
                    </tr>
                </table>
                <table class="table" style="width: 80%">
                    <tr>
                        <th colspan="3">丝印LOGO</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="10"
                                value="（丝印LOGO）-刮花"
                                v-model="abnormalarea"
                            />
                            <label for="10">刮花</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="11"
                                value="（丝印LOGO）-错误"
                                v-model="abnormalarea"
                            />
                            <label for="11">错误</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="12"
                                value="（丝印LOGO）-颜色"
                                v-model="abnormalarea"
                            />
                            <label for="12">颜色</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="3">面罩</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="13"
                                value="（面罩）-墨色不一致"
                                v-model="abnormalarea"
                            />
                            <label for="13">墨色不一致</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="14"
                                value="（面罩）-方向不一致"
                                v-model="abnormalarea"
                            />
                            <label for="14">方向不一致</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="15"
                                value="（面罩）-破损"
                                v-model="abnormalarea"
                            />
                            <label for="15">破损</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="3">红胶/三防漆</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="16"
                                value="（红胶/三防漆）-没打到位"
                                v-model="abnormalarea"
                            />
                            <label for="16">没打到位</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="17"
                                value="（红胶/三防漆）-漏打"
                                v-model="abnormalarea"
                            />
                            <label for="17">漏打</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="18"
                                value="（红胶/三防漆）-污染"
                                v-model="abnormalarea"
                            />
                            <label for="18">污染</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="3">线材/结扎线方式</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="19"
                                value="（线材/结扎线方式）-线序错误"
                                v-model="abnormalarea"
                            />
                            <label for="19">线序错误</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="20"
                                value="（线材/结扎线方式）-破损"
                                v-model="abnormalarea"
                            />
                            <label for="20">破损</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="21"
                                value="（线材/结扎线方式）-漏铜"
                                v-model="abnormalarea"
                            />
                            <label for="21">漏铜</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="3">电源/保护盖接线座防尘网</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="22"
                                value="（电源/保护盖接线座防尘网）-漏打螺丝"
                                v-model="abnormalarea"
                            />
                            <label for="22">漏打螺丝</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="23"
                                value="（电源/保护盖接线座防尘网）-错材"
                                v-model="abnormalarea"
                            />
                            <label for="23">错材</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="24"
                                value="（电源/保护盖接线座防尘网）-松动"
                                v-model="abnormalarea"
                            />
                            <label for="24">松动</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="3">黑屏检验</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="25"
                                value="（黑屏检验）-亮点"
                                v-model="abnormalarea"
                            />
                            <label for="25">亮点</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="26"
                                value="（黑屏检验）-亮条"
                                v-model="abnormalarea"
                            />
                            <label for="26">亮条</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="27"
                                value="（黑屏检验）-闪电"
                                v-model="abnormalarea"
                            />
                            <label for="27">闪电</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="3">单灯显示效果</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="28"
                                value="（单灯显示效果）-瞎灯"
                                v-model="abnormalarea"
                            />
                            <label for="28">瞎灯</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="29"
                                value="（单灯显示效果）-高亮"
                                v-model="abnormalarea"
                            />
                            <label for="29">高亮</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="30"
                                value="（单灯显示效果）-暗灯"
                                v-model="abnormalarea"
                            />
                            <label for="30">暗灯</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="3">箱内外观</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="31"
                                value="（箱内外观）-走线不一致"
                                v-model="abnormalarea"
                            />
                            <label for="31">走线不一致</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="32"
                                value="（箱内外观）-有异物"
                                v-model="abnormalarea"
                            />
                            <label for="32">有异物</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="33"
                                value="（箱内外观）-刮花"
                                v-model="abnormalarea"
                            />
                            <label for="33">刮花</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="3">标识</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="34"
                                value="（标识）-不整齐"
                                v-model="abnormalarea"
                            />
                            <label for="34">不整齐</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="35"
                                value="（标识）-翘角"
                                v-model="abnormalarea"
                            />
                            <label for="35">翘角</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="36"
                                value="（标识）-错误"
                                v-model="abnormalarea"
                            />
                            <label for="36">错误</label>
                        </td>
                    </tr>
                </table>
                <table class="table" style="width: 80%">
                    <tr>
                        <th colspan="2">其他</th>
                    </tr>
                    <tr>
                        <th>
                            <input
                                type="checkbox"
                                id="37"
                                value="（单色和全彩色测试）"
                                v-model="abnormalarea"
                            />
                            <label for="37">单色和全彩色测试</label>
                        </th>
                        <th>
                            <input
                                type="checkbox"
                                id="38"
                                value="（产品能效功率测试）"
                                v-model="abnormalarea"
                            />
                            <label for="38">产品能效功率测试</label>
                        </th>
                    </tr>
                    <tr>
                        <th>
                            <input
                                type="checkbox"
                                id="39"
                                value="（防尘）"
                                v-model="abnormalarea"
                            />
                            <label for="39">防尘</label>
                        </th>
                        <th>
                            <input
                                type="checkbox"
                                id="40"
                                value="（防水）"
                                v-model="abnormalarea"
                            />
                            <label for="40">防水</label>
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="41"
                                value="（把手/提手）-毛刺"
                                v-model="abnormalarea"
                            />
                            <label for="41">把手/提手-毛刺</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="42"
                                value="（接地）-松动"
                                v-model="abnormalarea"
                            />
                            <label for="42">接地-松动</label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="43"
                                value="（模组对角平整缝隙）-超出公差"
                                v-model="abnormalarea"
                            />
                            <label for="43">模组对角平整缝隙-超出公差</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="44"
                                value="（指示灯/自检/LED模块）-自检失效"
                                v-model="abnormalarea"
                            />
                            <label for="44">指示灯/自检/LED模块-自检失效</label>
                        </td>
                    </tr>
                </table>
            </el-col>
            <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12">
                <table class="table" style="width: 80%; margin-left: 10px">
                    <tr>
                        <th colspan="2">箱门/门锁</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="45"
                                value="（箱门/门锁）-方向不一致"
                                v-model="abnormalarea"
                            />
                            <label for="45">方向不一致</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="46"
                                value="（箱门/门锁）-锁不牢"
                                v-model="abnormalarea"
                            />
                            <label for="46">锁不牢</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2">灯管</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="47"
                                value="（灯管）-外观破损"
                                v-model="abnormalarea"
                            />
                            <label for="47">外观破损</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="48"
                                value="（灯管）-错灯"
                                v-model="abnormalarea"
                            />
                            <label for="48">错灯</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2">指示灯/按钮/显示模块</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="49"
                                value="（指示灯/按钮/显示模块）-松动"
                                v-model="abnormalarea"
                            />
                            <label for="49">松动</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="50"
                                value="（指示灯/按钮/显示模块）-错亮"
                                v-model="abnormalarea"
                            />
                            <label for="50">错亮</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2">锁扣</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="51"
                                value="（锁扣）-方向不一致"
                                v-model="abnormalarea"
                            />
                            <label for="51">方向不一致</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="52"
                                value="（锁扣）-锁不牢"
                                v-model="abnormalarea"
                            />
                            <label for="52">锁不牢</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2">航插座/防水锁头</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="53"
                                value="（航插座/防水锁头）-螺丝没打紧"
                                v-model="abnormalarea"
                            />
                            <label for="53">螺丝没打紧</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="54"
                                value="（航插座/防水锁头）-方向装反"
                                v-model="abnormalarea"
                            />
                            <label for="54">方向装反</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2">风扇/温控断路器/滤波器</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="55"
                                value="（风扇/温控断路器/滤波器）-错材"
                                v-model="abnormalarea"
                            />
                            <label for="55">错材</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="56"
                                value="（风扇/温控断路器/滤波器）-松动"
                                v-model="abnormalarea"
                            />
                            <label for="56">松动</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2">系统/功能卡</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="57"
                                value="（系统/功能卡）-错材"
                                v-model="abnormalarea"
                            />
                            <label for="57">错材</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="58"
                                value="（系统/功能卡）-死卡"
                                v-model="abnormalarea"
                            />
                            <label for="58">死卡</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2">胶条</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="59"
                                value="（胶条）-没贴到位"
                                v-model="abnormalarea"
                            />
                            <label for="59">没贴到位</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="60"
                                value="（胶条）-错料"
                                v-model="abnormalarea"
                            />
                            <label for="60">错料</label>
                        </td>
                    </tr>

                    <tr>
                        <th colspan="2">间隙/错位/高低不平</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="61"
                                value="（间隙/错位/高低不平）-高低差超标"
                                v-model="abnormalarea"
                            />
                            <label for="61">高低差超标</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="62"
                                value="（间隙/错位/高低不平）-点距超标"
                                v-model="abnormalarea"
                            />
                            <label for="62">点距超标</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2">安装孔</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="63"
                                value="（安装孔）-漏装螺母"
                                v-model="abnormalarea"
                            />
                            <label for="63">漏装螺母</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="64"
                                value="（安装孔）-孔距不对"
                                v-model="abnormalarea"
                            />
                            <label for="64">孔距不对</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2">插销/弹柱/定位柱</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="65"
                                value="（插销/弹柱/定位柱）-安装不到位"
                                v-model="abnormalarea"
                            />
                            <label for="65">安装不到位</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="66"
                                value="（插销/弹柱/定位柱）-错位"
                                v-model="abnormalarea"
                            />
                            <label for="66">错位</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2">拼装测试</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="67"
                                value="（拼装测试）-高低差"
                                v-model="abnormalarea"
                            />
                            <label for="67">高低差</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="68"
                                value="（拼装测试）-错位"
                                v-model="abnormalarea"
                            />
                            <label for="68">错位</label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="2">扫描检验</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="69"
                                value="（扫描检验）-跟点"
                                v-model="abnormalarea"
                            />
                            <label for="69">跟点</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="70"
                                value="（扫描检验）-消稳"
                                v-model="abnormalarea"
                            />
                            <label for="70">消稳</label>
                        </td>
                    </tr>

                    <tr>
                        <th colspan="2">亮暗线</th>
                    </tr>
                    <tr>
                        <td>
                            <input
                                type="checkbox"
                                id="71"
                                value="（亮暗线）-高线超标"
                                v-model="abnormalarea"
                            />
                            <label for="71">高线超标</label>
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                id="72"
                                value="（亮暗线）-暗线超标"
                                v-model="abnormalarea"
                            />
                            <label for="72">暗线超标</label>
                        </td>
                    </tr>
                </table>
            </el-col>
        </div>
    </el-card>
</template>

<script>
export default {
    name: "",
    props: {},
    data() {
        return {
            abnormalarea: [],
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
@import url("~@/assets/style/color.less");

.notFinish {
    border-color: blue;
}

.abnormalFrame {
    width: 260px;
    height: 200px;
    border: 1px solid #000000;
    overflow: auto;
}

.barcodeStateCard .pqcanomou .acard {
    max-width: 800px;
}

@media (min-width: 992px) {
    .barcodeStateCard .pqcanomou .acard {
        margin: 0 8px 8px;
    }
    .barcodeStateCard .notDo,
    .barcodeStateCard .finish,
    .barcodeStateCard .notFinish {
        margin: 0 auto;
        width: 200px;
    }
}
@media (max-width: 992px) {
    .inputbox {
        padding-right: 30px;
        display: flex;
        flex-flow: row wrap;
        justify-content: flex-start;
        align-items: center;
        text-align: left;
        margin: 5px;
    }

    .inputbox .title {
        font-size: 14px;
        margin: 5px;
    }

    .barcodeStateCard .notDo,
    .barcodeStateCard .finish,
    .barcodeStateCard .notFinish {
        margin-left: 0px;
    }
}

.barcodeStateCard .pqcanomou .el-card__body {
    padding: 5px;
    overflow: auto;
    height: 685px;
}

.rightList {
    margin-left: 50px;
}

.rightList .table > tr > th,
.rightList .table > tr > td {
    padding: 2px;
    line-height: 1.3;
    border-bottom: 1px solid #b2b0b0;
    font-size: 14px;
    text-align: left;
}
</style>