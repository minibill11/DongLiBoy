<!---  --->
<template>
    <div>
        <moduleMenu active="模块产线"></moduleMenu>
        <el-row class="main" v-loading="loading">
            <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
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
                    <label>工序</label>
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
                <div class="flex-item" v-if="statueFrom.value == 'AI'">
                    <label>机台号</label>
                    <div>
                        <el-input-number
                            size="medium"
                            :min="0"
                            v-model="machineNum"
                        ></el-input-number>
                    </div>
                </div>
                <div
                    class="flex-item barcodeDiv"
                    v-bind:class="message != '' ? 'is-error' : ''"
                >
                    <label>条码号</label>
                    <div>
                        <div class="el-input el-input--medium el-input--suffix">
                            <input
                                v-model.trim="ModuleBarcode"
                                v-on:keyup.enter="scanKeyup($event)"
                                ref="barcodeRef"
                                autocomplete="off"
                                placeholder="输入条码后回车"
                                class="el-input__inner"
                            />
                        </div>
                        <div class="el-form-item__error" v-show="message != ''">
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
                            @click="postConfirm"
                            :disabled="!isok"
                            >确认</el-button
                        >
                    </div>
                </div>
                <!-- <printSetting v-model="printInfo"></printSetting> -->
            </el-col>
            <el-col :xs="24" :sm="24" :md="16" :lg="16" :xl="16">
                <barcodeStatusList
                    v-model="barcodeListState"
                ></barcodeStatusList>
            </el-col>
        </el-row>
    </div>
</template>

<script>
import setModuleManagement from "./page-components/module";
import moduleMenu from "./page-components/_moduleMenu";
import barcodeStatusList from "./page-components/_barcodeStatusList";
// import printSetting from "@/components/print-setting/index";
import {
    Checklist,
    productionLine,
    checkBarcode,
    ModuleBarcodePrinting,
} from "@/api/order/ModuleManagement";
export default {
    name: "module-manage-normal-check",
    props: {},
    data() {
        return {
            barcodeListState: {},
            message: "",
            period: [],
            IsAbnormal: false,
            AbnormalResult: [],
            Remark: "",
            isok: false,
            speak: "",
            machineNum: 0,
            statueFrom: {
                value: "",
                option: [
                    {
                        value: "AI",
                        label: "AI工序",
                    },
                    {
                        value: "后焊",
                        label: "后焊工序",
                    },
                    {
                        value: "灌胶前电检",
                        label: "灌胶前电检",
                    },
                    {
                        value: "模块电检",
                        label: "模块电检",
                    },
                    {
                        value: "外观电检",
                        label: "外观电检",
                    },
                ],
            },
        };
    },
    components: {
        moduleMenu,
        barcodeStatusList,
        // printSetting,
    },
    computed: {},
    watch: {
        ModuleBarcode(v) {
            this.reset(false);
        },
        Ordernum(v) {
            this.reset(true);
        },
        IsAbnormal(v) {
            if (!v) {
                this.AbnormalResult = [];
            }
        },
        "statueFrom.value"(v) {
            if (v != "") {
                localStorage.setItem("mokuaistatue", v);
                this.Ordernum != "" && this.getBarcodeStateList(this.Ordernum);
            }
        },
    },
    methods: {
        //选择订单号
        selectOrdernum(v) {
            if (v.value != "") {
                localStorage.setItem("Order", v.value);
                this.statueFrom.value != "" &&
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

            if (this.statueFrom.value == "AI") {
                postUrl = "AICreate";
                postData = {
                    UserName: this.$userInfo.Name,
                    Department: this.Department,
                    Group: this.Group,
                    Machine: this.machineNum,
                    Ordernum: this.Ordernum,
                    ModuleBarcode: this.ModuleBarcode,
                    IsAbnormal: this.IsAbnormal,
                    AbnormalResultMessage: this.IsAbnormal
                        ? this.AbnormalResult.toString()
                        : "正常",
                };
            } else if (this.statueFrom.value == "后焊") {
                postUrl = "AfterWeldingCreate";
                postData = {
                    UserName: this.$userInfo.Name,
                    Department: this.Department,
                    Group: this.Group,
                    Ordernum: this.Ordernum,
                    ModuleBarcode: this.ModuleBarcode,
                    IsAbnormal: this.IsAbnormal,
                    AbnormalResultMessage: this.IsAbnormal
                        ? this.AbnormalResult.toString()
                        : "正常",
                    Remark: this.Remark,
                };
            } else {
                postUrl = "ElectricInspectionBeforeGlueFillingCreate";
                postData = {
                    UserName: this.$userInfo.Name,
                    Department: this.Department,
                    Group: this.Group,
                    Ordernum: this.Ordernum,
                    ModuleBarcode: this.ModuleBarcode,
                    Section: this.statueFrom.value,
                    ElectricInspectionResult: this.IsAbnormal,
                    ElectricInspectionMessage: this.IsAbnormal
                        ? this.AbnormalResult.toString()
                        : "正常",
                    Remark: this.Remark,
                };
            }
            productionLine(postUrl, postData)
                .then((res) => {
                    if (res.data.Result == true) {
                        this.speakInfo("扫码成功");
                        //this.printClick(true);
                        this.$message.success("录入成功！");
                        this.reset(true);
                        this.ModuleBarcode = "";
                        this.getBarcodeStateList(this.Ordernum);
                        this.$refs.barcodeRef.focus();
                    } else {
                        if (res.data.Message == "该条码已有后焊记录") {
                            //this.$message.info(res.data.Message + "，可重新打印");
                            this.message =
                                res.data.Message; /*+ "，可重新打印条码";*/
                            //this.disabledState = true;
                            this.isok = false;
                        } else {
                            //this.$alert(res.data.Message, '未完成', {
                            //    confirmButtonText: '确定',
                            //    type: 'error'
                            //});
                            this.$message.error(res.data.Message);
                        }
                        this.speakInfo(res.data.Message);
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

            this.loading = true;
            this.isok = false;
            this.disabledState = false;
            this.period = [];
            this.message = "";
            this.IsAbnormal = false;
            this.AbnormalResult = [];
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
                        //if (res.data.Data.message == "没有找到条码信息") {
                        //} else {
                        //};
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
        },
        //语音播放
        speakInfo(text) {
            //this.speak.lang = 'zh-CN';//中文
            //this.speak.pitch = 1;//设置话语的音调(值越大越尖锐, 越低越低沉)0-2
            //this.speak.rate = 1.2; //设置说话的速度(值越大语速越快, 越小语速越慢)0.1-10
            //this.speak.volume = 1;//获取并设置说话的音量0-1
            speechSynthesis.cancel(); //删除队列中所有的语音.如果正在播放, 则直接停止
            this.speak.text = text;
            speechSynthesis.speak(this.speak);
        },
        //打印
        printClick(fromresult) {
            let info = this.printInfo;
            if (info.ip == 0) {
                return;
            }
            this.loading = true;
            info["barcode"] = this.ModuleBarcode;
            info["modulenum"] = [this.ModuleBarcode];
            ModuleBarcodePrinting(info)
                .then((res) => {
                    if (fromresult) {
                        this.$message.success("录入成功！");
                        this.reset(true);
                        this.ModuleBarcode = "";
                    } else {
                        if (res.data.Data == "打印成功！") {
                            this.$message.success(res.data.Data);
                        } else {
                            this.$message.warning(res.data.Data);
                        }
                    }
                    this.loading = false;
                    this.$refs.barcodeRef.select();
                })
                .catch((err) => {
                    console.warn(err);
                    this.loading = false;
                });
        },
        //获取条码状态清单
        getBarcodeStateList(v) {
            Checklist({
                orderNum: v,
                statue: this.statueFrom.value,
                IsSamping: false,
            })
                .then((res) => {
                    this.barcodeListState = res.data.Data;
                })
                .catch((err) => {
                    console.warn(err);
                });
        },
        //重置信息
        reset(abnormalhave) {
            this.disabledState = false;
            this.message = "";
            this.period = [];
            this.isok = false;
            if (abnormalhave) {
                this.IsAbnormal = false;
                this.AbnormalResult = [];
                this.Remark = "";
            }
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
            if (this.Ordernum == "") {
                errormes = errormes + (nooknum + 1) + "、订单号不能为空<br/>";
                nooknum++;
            }
            if (this.statueFrom.value == "") {
                errormes = errormes + (nooknum + 1) + "、工序不能为空<br/>";
                nooknum++;
            }
            if (this.ModuleBarcode == "") {
                errormes = errormes + (nooknum + 1) + "、条码号不能为空<br/>";
                nooknum++;
            }

            if (this.IsAbnormal) {
                if (this.AbnormalResult.length == 0) {
                    errormes =
                        errormes +
                        (nooknum + 1) +
                        "、若为异常，异常列表不能为空<br/>";
                    nooknum++;
                }
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
    },
    mixins: [setModuleManagement],
    created() {
        let mokuaistatue = localStorage.getItem("mokuaistatue");
        if (mokuaistatue != null) {
            this.statueFrom.value = mokuaistatue;
        }
    },
    mounted() {
        let localOrder = localStorage.getItem("Order");
        if (localOrder != null) {
            this.Ordernum = localOrder;
            this.getBarcodeStateList(localOrder);
        }
        this.speak = new SpeechSynthesisUtterance();
        this.$refs.barcodeRef.focus();
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