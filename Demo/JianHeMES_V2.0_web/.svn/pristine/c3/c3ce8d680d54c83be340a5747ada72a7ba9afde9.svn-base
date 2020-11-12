<!---  --->
<template>
    <div>
        <moduleMenu active="模块老化"></moduleMenu>

        <el-row class="main">
            <el-col :xs="24" :sm="24" :md="10" :lg="10" :xl="10">
                <div class="flex-item">
                    <div>
                        <el-switch
                            v-model="inputAbnormalShow"
                            size="medium"
                            active-text="录入老化异常信息"
                            inactive-text=""
                        >
                        </el-switch>
                    </div>
                </div>
                <template v-if="inputAbnormalShow">
                    <div class="flex-item">
                        <label>订单号</label>
                        <div>
                            <el-autocomplete
                                v-model="Ordernum"
                                @select="selectOrdernum"
                                :fetch-suggestions="querySearch"
                                :debounce="0"
                                size="medium"
                                placeholder="输入内容可查询"
                                highlight-first-item
                                clearable
                            ></el-autocomplete>
                        </div>
                    </div>
                    <div
                        class="flex-item barcodeDiv"
                        v-bind:class="message != '' ? 'is-error' : ''"
                    >
                        <label>条码号</label>
                        <div>
                            <div
                                class="el-input el-input--medium el-input--suffix"
                            >
                                <input
                                    v-model.trim="ModuleBarcode"
                                    v-on:keyup.enter="postAbnormal('bar')"
                                    ref="barcodeRef"
                                    autocomplete="off"
                                    placeholder="输入条码后回车"
                                    class="el-input__inner"
                                />
                            </div>
                            <div
                                class="el-form-item__error"
                                v-show="message != ''"
                            >
                                {{ message }}
                            </div>
                        </div>
                    </div>
                    <div class="flex-item">
                        <label>异常原因</label>
                        <div>
                            <el-input
                                placeholder="请输入异常原因"
                                v-model.trim="AbnormalMessage"
                                v-on:keyup.native.enter="postAbnormal('ab')"
                                ref="abnormalRef"
                                size="medium"
                                clearable
                            >
                            </el-input>
                        </div>
                    </div>
                    <div class="flex-item">
                        <div>
                            <el-button
                                type="primary"
                                size="small"
                                @click="postAbnormal('btn')"
                                >确认</el-button
                            >
                        </div>
                    </div>
                </template>
                <template v-else>
                    <div class="flex-item">
                        <label>班组</label>
                        <div>
                            <el-select
                                v-model="Group"
                                placeholder="请选择班组"
                                size="medium"
                            >
                                <el-option
                                    v-for="item in groupOptions"
                                    :key="item"
                                    :label="item"
                                    :value="item"
                                >
                                </el-option>
                            </el-select>
                        </div>
                    </div>
                    <div class="flex-item">
                        <label>老化工序</label>
                        <div>
                            <el-select
                                v-model="statueFrom.value"
                                placeholder="请选择工序"
                                size="medium"
                            >
                                <el-option
                                    v-for="item in statueFrom.option"
                                    :key="item.value"
                                    :label="item.label"
                                    :value="item.value"
                                >
                                </el-option>
                            </el-select>
                        </div>
                    </div>
                    <div class="flex-item">
                        <label>订单号</label>
                        <div>
                            <el-autocomplete
                                v-model="Ordernum"
                                @select="selectOrdernum"
                                :fetch-suggestions="querySearch"
                                :debounce="0"
                                size="medium"
                                placeholder="输入内容可查询"
                                highlight-first-item
                                clearable
                            ></el-autocomplete>
                        </div>
                    </div>
                    <div class="flex-item">
                        <label>老化架号</label>
                        <div>
                            <el-input
                                placeholder="请输入老化架号"
                                v-model.trim="burninFrameNum"
                                ref="burninFrameNumRef"
                                v-on:keyup.native.enter="burninFrameNumEnter"
                                size="medium"
                                clearable
                            >
                            </el-input>
                        </div>
                    </div>
                    <div
                        class="flex-item barcodeDiv"
                        v-bind:class="message != '' ? 'is-error' : ''"
                    >
                        <label>条码号</label>
                        <div>
                            <div
                                class="el-input el-input--medium el-input--suffix"
                            >
                                <input
                                    v-model.trim="ModuleBarcode"
                                    v-on:keyup.enter="scanKeyup($event)"
                                    ref="barcodeRef"
                                    autocomplete="off"
                                    placeholder="输入条码后回车"
                                    class="el-input__inner"
                                />
                            </div>
                            <div
                                class="el-form-item__error"
                                v-show="message != ''"
                            >
                                {{ message }}
                            </div>
                        </div>
                    </div>
                    <div class="flex-item" v-show="period.length > 0">
                        <label>工段信息</label>
                        <div>
                            <el-table
                                :data="period"
                                size="mini"
                                align="center"
                                style="width: 220px"
                                max-height="300"
                                stripe
                                border
                            >
                                <el-table-column
                                    prop="Name"
                                    label="工段名"
                                    align="center"
                                >
                                </el-table-column>
                                <el-table-column
                                    prop="Have"
                                    label="状态"
                                    align="center"
                                >
                                    <template slot-scope="scope">
                                        <i
                                            :class="[
                                                scope.row.Have
                                                    ? 'el-icon-check green'
                                                    : 'el-icon-close red',
                                            ]"
                                        ></i>
                                    </template>
                                </el-table-column>
                            </el-table>
                        </div>
                    </div>
                    <div class="flex-item">
                        <div>
                            <el-button
                                type="primary"
                                size="small"
                                @click="checkBarcodeList"
                                v-if="statueFrom.value == '结束老化'"
                                :disabled="barcodeList.length == 0"
                                >检查条码</el-button
                            >
                            <el-button
                                type="primary"
                                size="small"
                                @click="postConfirm"
                                :disabled="!isok"
                                >确认</el-button
                            >
                        </div>
                    </div>
                </template>
            </el-col>
            <el-col :xs="24" :sm="24" :md="14" :lg="14" :xl="14">
                <barcodeStatusList
                    v-if="!inputAbnormalShow && statueFrom.value == '开始老化'"
                    v-model="barcodeListState"
                ></barcodeStatusList>
                <template
                    v-if="!inputAbnormalShow && statueFrom.value == '结束老化'"
                >
                    <div
                        style="
                            color: #888;
                            font-size: 13px;
                            margin: 0 0 5px 5px;
                        "
                    >
                        数量：{{ barcodeList.length }}
                    </div>
                    <el-table
                        :data="barcodeList"
                        max-height="400"
                        style="width: 500px"
                        size="small"
                        align="center"
                        stripe
                        border
                    >
                        <el-table-column
                            prop="barcode"
                            label="条码号"
                            align="center"
                        >
                        </el-table-column>
                        <el-table-column
                            prop="Message"
                            label="状态"
                            align="center"
                        >
                        </el-table-column>
                        <el-table-column
                            prop="Result"
                            label="是否可以完成"
                            align="center"
                        >
                            <template slot-scope="scope">
                                <i
                                    :class="[
                                        scope.row.Result
                                            ? 'el-icon-check green'
                                            : 'el-icon-close red',
                                    ]"
                                ></i>
                            </template>
                        </el-table-column>
                        <el-table-column width="50" align="center">
                            <template slot-scope="scope">
                                <el-button
                                    type="text"
                                    size="mini"
                                    @click="deleteRow(scope.$index)"
                                    >移除</el-button
                                >
                            </template>
                        </el-table-column>
                    </el-table>
                </template>
            </el-col>
        </el-row>
    </div>
</template>

<script>
import setModuleManagement from "./page-components/module";
import moduleMenu from "./page-components/_moduleMenu";
import barcodeStatusList from "./page-components/_barcodeStatusList";
import {
    Checklist,
    checkBarcode,
    BurninAbnormal,
    BurninCompleteCheck,
    BurninConfirm,
} from "@/api/order/ModuleManagement";
export default {
    name: "module-manage-burn-in",
    props: {},
    data() {
        return {
            barcodeListState: {},
            message: "",
            period: [],
            inputAbnormalShow: false,
            AbnormalMessage: "",
            isok: false,
            speak: "",
            statueFrom: {
                value: "",
                option: [
                    {
                        value: "开始老化",
                        label: "开始老化",
                    },
                    {
                        value: "结束老化",
                        label: "结束老化",
                    },
                ],
            },
            burninFrameNum: "",
            barcodeList: [],
        };
    },
    components: {
        moduleMenu,
        barcodeStatusList,
    },
    computed: {},
    watch: {
        ModuleBarcode(v) {
            if (this.inputAbnormalShow) {
                this.message = "";
                return;
            }
            this.disabledState = false;
            this.message = "";
            this.period = [];
            this.isok = false;
        },
        Ordernum(v) {
            this.reset();
        },
        "statueFrom.value"(v) {
            v != "" && localStorage.setItem("mokuailaohuastatue", v);
            if (v == "开始老化" && this.Ordernum != "") {
                this.getBarcodeStateList(this.Ordernum);
            } else if (v == "结束老化") {
                this.barcodeList = [];
                this.period = [];
            }
            if (this.burninFrameNum == "") {
                this.$refs.burninFrameNumRef.focus();
            } else {
                this.$refs.barcodeRef.focus();
            }
        },
        inputAbnormalShow(v) {
            this.reset();
            this.$nextTick(() => {
                this.ModuleBarcode = "";
                this.$refs.barcodeRef.focus();
            });
            if (!v && this.Ordernum != "") {
                this.getBarcodeStateList(this.Ordernum);
            }
        },
    },
    methods: {
        //选择订单号
        selectOrdernum(v) {
            if (v.value != "") {
                localStorage.setItem("Order", v.value);
                this.getBarcodeStateList(v.value);
            }
        },
        //确认完成
        postConfirm() {
            //验证不能为空，不打印的提示要正确
            if (this.checkPostInfo()) {
                this.speakInfo("请核对信息");
                this.$refs.barcodeRef.select();
                return;
            }
            this.loading = true;

            let postUrl = "",
                postData = {};
            if (this.statueFrom.value == "开始老化") {
                postUrl = "BurninCreate";
                postData = {
                    UserName: this.$userInfo.Name,
                    Department: this.Department,
                    Group: this.Group,
                    ModuleBarcode: this.ModuleBarcode,
                    Ordernum: this.Ordernum,
                    BurninFrame: this.burninFrameNum,
                    //BurninResult: false,
                    //BurninMessage: "正常",
                    Remark: "",
                };
            } else if (this.statueFrom.value == "结束老化") {
                postUrl = "BurninComplete";
                postData = {
                    UserName: this.$userInfo.Name,
                    ordernum: this.Ordernum,
                    modulbarcode: this.barcodeList.map((i) => i.barcode),
                };
            }
            BurninConfirm(postUrl, postData)
                .then((res) => {
                    if (res.data.Result == true) {
                        this.speakInfo("扫码成功");
                        this.$message.success(res.data.Message);
                        this.reset();
                        if (this.statueFrom.value == "开始老化") {
                            this.getBarcodeStateList(this.Ordernum);
                        } else if (this.statueFrom.value == "结束老化") {
                            this.barcodeList = [];
                        }
                        this.ModuleBarcode = "";
                        this.loading = false;
                        this.$refs.barcodeRef.focus();
                    } else {
                        this.message = res.data.Message;
                        this.$message.error(res.data.Message);
                        this.speakInfo(res.data.Message);
                        this.isok = false;
                        this.loading = false;
                        this.$refs.barcodeRef.select();
                    }
                })
                .catch((err) => {
                    this.speakInfo("记录失败");
                    console.warn(err);
                    this.loading = false;
                    this.$refs.barcodeRef.select();
                });
        },
        //扫码
        scanKeyup(v) {
            if (v.target.value == "") {
                this.$refs.barcodeRef.focus();
                return;
            }

            if (this.statueFrom.value == "开始老化") {
                this.loading = true;
                this.isok = false;
                this.disabledState = false;
                this.period = [];
                this.message = "";
                checkBarcode({
                    orderNum: this.Ordernum,
                    barcode: v.target.value,
                })
                    .then((res) => {
                        //console.log(res)
                        if (res.data.Data.result == true) {
                            this.isok = true;
                            this.period = res.data.Data.period;
                            this.postConfirm();
                        } else {
                            this.message = res.data.Data.message;
                            this.speakInfo(res.data.Data.message);
                            this.$refs.barcodeRef.select();
                        }
                        this.loading = false;
                    })
                    .catch((err) => {
                        this.speakInfo("记录失败");
                        console.warn(err);
                        this.loading = false;
                        this.$refs.barcodeRef.select();
                    });
            } else if (this.statueFrom.value == "结束老化") {
                if (this.barcodeList.length >= 1) {
                    for (let i of this.barcodeList) {
                        if (i.barcode == v.target.value) {
                            this.$message.warning("已经存在此条码");
                            return;
                        }
                    }
                }
                this.barcodeList.push({
                    barcode: v.target.value,
                });
                this.ModuleBarcode = "";
            }
        },
        checkBarcodeList() {
            let postlist = this.barcodeList.map((i) => i.barcode);
            BurninCompleteCheck({
                ordernum: this.Ordernum,
                modulbarcode: postlist,
            })
                .then((res) => {
                    this.barcodeList = res.data.Data;
                    this.isok = true;
                    for (let i of res.data.Data) {
                        if (!i.Result) {
                            this.isok = false;
                            break;
                        }
                    }
                    this.loading = false;
                })
                .catch((err) => {
                    this.loading = false;
                    this.$refs.barcodeRef.select();
                });
        },
        //语音播放
        speakInfo(text) {
            speechSynthesis.cancel(); //删除队列中所有的语音.如果正在播放, 则直接停止
            this.speak.text = text;
            speechSynthesis.speak(this.speak);
        },
        //获取条码状态清单
        getBarcodeStateList(v) {
            Checklist({
                orderNum: v,
                statue: "老化",
                IsSamping: false,
            })
                .then((res) => {
                    this.barcodeListState = res.data.Data;
                })
                .catch((err) => {
                    console.warn(err);
                });
        },
        deleteRow(index) {
            this.barcodeList.splice(index, 1);
        },
        //重置信息
        reset() {
            this.disabledState = false;
            this.message = "";
            this.period = [];
            this.isok = false;
            this.AbnormalMessage = "";
            this.barcodeList = [];
            this.loading = false;
        },
        //检查信息完整
        checkPostInfo() {
            let errormes = "",
                nooknum = 0;
            if (this.Department == "") {
                errormes = errormes + (nooknum + 1) + "、部门不能为空<br/>";
                nooknum++;
            }
            if (this.Group == "") {
                errormes = errormes + (nooknum + 1) + "、班组不能为空<br/>";
                nooknum++;
            }
            if (this.statueFrom.value == "") {
                errormes = errormes + (nooknum + 1) + "、老化工序不能为空<br/>";
                nooknum++;
            }
            if (this.Ordernum == "") {
                errormes = errormes + (nooknum + 1) + "、订单号不能为空<br/>";
                nooknum++;
            }
            if (this.burninFrameNum == "") {
                errormes = errormes + (nooknum + 1) + "、老化架号不能为空<br/>";
                nooknum++;
            }
            if (
                this.ModuleBarcode == "" &&
                this.statueFrom.value != "结束老化"
            ) {
                errormes = errormes + (nooknum + 1) + "、条码号不能为空<br/>";
                nooknum++;
            }
            if (nooknum > 0) {
                this.$alert(errormes, "数据不合格", {
                    confirmButtonText: "确定",
                    closeOnClickModal: true,
                    closeOnPressEscape: true,
                    dangerouslyUseHTMLString: true,
                    type: "error",
                });
                return true;
            } else {
                return false;
            }
        },
        //异常录入
        postAbnormal(from) {
            if (from == "bar") {
                if (this.AbnormalMessage == "") {
                    this.$refs.abnormalRef.focus();
                    return;
                }
            }
            if (from == "ab") {
                if (this.ModuleBarcode == "") {
                    this.$refs.barcodeRef.focus();
                    return;
                }
            }
            if (
                this.Ordernum == "" ||
                this.ModuleBarcode == "" ||
                this.AbnormalMessage == ""
            ) {
                this.$message.warning("请填写完整订单号，条码号，异常原因");
                return;
            }
            this.message = "";
            BurninAbnormal({
                ordernum: this.Ordernum,
                modulebarcode: this.ModuleBarcode,
                BurninResult: this.AbnormalMessage,
            })
                .then((res) => {
                    if (res.data.Result) {
                        this.$message.success(res.data.Message);
                        this.ModuleBarcode = "";
                        this.AbnormalMessage = "";
                        this.$refs.barcodeRef.focus();
                    } else {
                        this.message = res.data.Message;
                    }
                })
                .catch((err) => {
                    console.warn(err);
                });
        },
        burninFrameNumEnter() {
            this.$refs.barcodeRef.focus();
        },
    },
    mixins: [setModuleManagement],
    created() {
        let mokuailaohuastatue = localStorage.getItem("mokuailaohuastatue");
        if (mokuailaohuastatue != null) {
            this.statueFrom.value = mokuailaohuastatue;
        }
    },
    mounted() {
        let localOrder = localStorage.getItem("Order");
        if (localOrder != null) {
            this.Ordernum = localOrder;
            this.getBarcodeStateList(localOrder);
        }
        this.speak = new SpeechSynthesisUtterance();
        this.$refs.burninFrameNumRef.focus();
    },
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