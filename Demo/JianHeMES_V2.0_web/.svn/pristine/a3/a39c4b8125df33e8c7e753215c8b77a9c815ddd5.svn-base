<!---  --->
<template>
    <div>
        <moduleMenu active="模块抽检"></moduleMenu>
        <el-row class="main">
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
            </el-col>
            <el-col :xs="24" :sm="24" :md="16" :lg="16" :xl="16">
                <el-row>
                    <el-col
                        :xs="24"
                        :sm="24"
                        :md="12"
                        :lg="8"
                        :xl="8"
                        :offset="8"
                        ><el-row
                            style="
                                text-align: center;
                                margin: 0 0 5px;
                                color: #409eff;
                                max-width: 250px;
                            "
                        >
                            抽检完成率：{{ SmaplingRate }}
                        </el-row></el-col
                    >
                </el-row>

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
import {
    Checklist,
    Sampling,
    CheckSampling,
} from "@/api/order/ModuleManagement";
export default {
    name: "module-manage-spot-check",
    props: {},
    data() {
        return {
            barcodeListState: {},
            SmaplingRate: "",
            message: "",
            period: [],
            IsAbnormal: false,
            AbnormalResult: [],
            Remark: "",
            isok: false,
            speak: "",
            statueFrom: {
                value: "",
                option: [
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

            let moduleSampling = {
                UserName: this.$userInfo.Name,
                Department: this.Department,
                Group: this.Group,
                Ordernum: this.Ordernum,
                ModuleBarcode: this.ModuleBarcode,
                Section: this.statueFrom.value,
                SamplingResult: this.IsAbnormal,
                SamplingMessage: this.IsAbnormal
                    ? this.AbnormalResult.toString()
                    : "正常",
                remak: this.Remark,
            };

            Sampling(moduleSampling)
                .then((res) => {
                    if (res.data.Result == true) {
                        this.speakInfo("扫码成功");
                        this.$message.success("录入成功！");
                        this.reset(true);
                        this.ModuleBarcode = "";
                        this.getBarcodeStateList(this.Ordernum);
                        this.loading = false;
                        this.$refs.barcodeRef.focus();
                    } else {
                        //this.$alert(res.data.Message, '未完成', {
                        //    confirmButtonText: '确定',
                        //    type: 'error'
                        //});
                        this.$message.error(res.data.Message);
                        this.message = res.data.Message;
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
            CheckSampling({
                ordernum: this.Ordernum,
                barcode: v.target.value,
                statue: this.statueFrom.value,
            })
                .then((res) => {
                    //console.log(res)
                    if (res.data.Result == true) {
                        this.isok = true;
                        this.period = res.data.Data.period;
                        this.postConfirm();
                    } else {
                        this.message = res.data.Message;
                        this.speakInfo(res.data.Message);
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
        //获取条码状态清单
        getBarcodeStateList(v) {
            Checklist({
                orderNum: v,
                statue: this.statueFrom.value,
                IsSamping: true,
            })
                .then((res) => {
                    this.barcodeListState = res.data.Data;
                    this.SmaplingRate = res.data.Data.SmaplingRate;
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
        if (mokuaistatue != null && mokuaistatue != "AI") {
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