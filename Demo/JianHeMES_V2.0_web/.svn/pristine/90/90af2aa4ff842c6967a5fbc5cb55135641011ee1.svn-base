<!---  --->
<template>
    <div>
        <el-header class="text-center">
            <h3>{{ state == false ? "PQC开始检查" : "PQC检查完成" }}</h3>
        </el-header>
        <el-row class="barcodeStateCard">
            <el-col :xs="24" :sm="6" :md="6" :lg="6" :xl="6">
                <div class="inputbox">
                    <el-checkbox v-model="checked">挪用库存 </el-checkbox>
                </div>
                <div class="inputbox" v-show="state == false">
                    <span class="title">班组</span>
                    <el-select
                        v-model="Group"
                        clearable
                        allow-create
                        filterable
                        placeholder="请选择班组"
                    >
                        <el-option
                            v-for="item in groupOptions"
                            :key="item"
                            :value="item"
                        >
                        </el-option>
                    </el-select>
                </div>
                <div class="inputbox" v-show="checked">
                    <span class="title">被挪用订单号</span>
                    <el-select
                        v-model="nuoOrder"
                        clearable
                        allow-create
                        filterable
                        placeholder="请选择挪用订单号"
                    >
                        <el-option
                            v-for="item in OrderNumOptions"
                            :key="item.value"
                            :value="item.value"
                        >
                        </el-option>
                    </el-select>
                </div>
                <div class="inputbox" v-show="checked">
                    <span class="title">被挪用条码号</span>
                    @*<input
                        type="text"
                        class="el-input__inner"
                        placeholder="请输入挪用条码号"
                        ref="selectCode"
                        v-on:keyup.enter="checkData($event)"
                        style="width: 225px"
                        v-bind:value="nuoBarCode"
                        v-on:input="nuoBarCode = $event.target.value"
                    />*@
                    <input
                        type="text"
                        placeholder="输入条码后回车"
                        v-model.lazy="nuoBarCode"
                        v-on:keyup.enter="checkData"
                        class="el-input__inner"
                        ref="selectNuoCode"
                        style="max-width: 225px"
                    />
                </div>
                <div class="inputbox">
                    <span class="title">订单号</span>
                    <el-select
                        v-model="OrderNum"
                        clearable
                        allow-create
                        filterable
                        placeholder="请选择订单号"
                        :disabled="state != false"
                    >
                        <el-option
                            v-for="item in OrderNumOptions"
                            :key="item.value"
                            :value="item.value"
                        >
                        </el-option>
                    </el-select>
                </div>
                <div class="inputbox" v-show="state == false">
                    <span class="title">箱体模组条码</span>
                    <input
                        type="text"
                        placeholder="输入条码后回车"
                        v-model.lazy="BoxBarCode"
                        v-on:keyup.enter="checkData"
                        class="el-input__inner"
                        ref="selectCode"
                        style="max-width: 225px"
                    />
                </div>
                <div class="inputbox" v-show="state == true">
                    <span class="title">箱体模组条码</span>
                    <el-input
                        v-model="input"
                        placeholder="请输入内容"
                        v-model.lazy="BoxBarCode"
                        v-on:keyup.enter="checkData"
                        ref="selectCode"
                        style="max-width: 225px"
                        :disabled="state != false"
                    ></el-input>
                </div>
                <div class="inputbox">
                    <span style="font-size: 12px; color: #fd6a6a"
                        >扫码需要保持英文输入状态</span
                    >
                </div>
                <div class="inputbox" v-show="state != false">
                    <span class="title">产线</span>
                    <el-input-number
                        v-model="num"
                        controls-position="right"
                        :min="1"
                        :max="200"
                        style="width: 225px"
                    ></el-input-number>
                </div>
                <div class="inputbox" v-show="state != false">
                    <span class="title">备注</span>
                    <el-input
                        type="textarea"
                        :rows="2"
                        placeholder="请输入内容"
                        v-model="Remark"
                        style="width: 225px"
                    >
                    </el-input>
                </div>
                <div class="inputbox" v-show="state != false">
                    <span class="title">维修情况</span>
                    <el-select
                        v-model="PQCRepairCondition"
                        placeholder="请选择"
                    >
                        <el-option
                            v-for="item in RepairOptions"
                            :key="item.value"
                            :label="item.label"
                            :value="item.value"
                        >
                        </el-option>
                    </el-select>
                </div>
                <div
                    class="inputbox"
                    v-show="PQCRepairCondition != '正常' && state != false"
                >
                    <span class="title">PQC异常</span>
                    <div class="abnormalFrame">
                        <ol>
                            <li
                                v-for="abnormal in abnormalarea"
                                :key="abnormal"
                            >
                                {{ abnormal }}
                            </li>
                        </ol>
                    </div>
                </div>
                <div class="inputbox">
                    <el-button type="primary" size="small" @click="checkData">{{
                        state == false ? "开始FQC检查工作" : "PQC检查完成"
                    }}</el-button>
                </div>
            </el-col>
            <div v-show="state == false">
                <el-col :xs="24" :sm="6" :md="6" :lg="6" :xl="5">
                    <el-card class="notDo">
                        <div slot="header">
                            <span>未开始 {{ notDoList.length }}个</span>
                        </div>
                        <ul>
                            <li v-for="item in notDoList" :key="item">
                                {{ item }}
                            </li>
                        </ul>
                        <ul style="color: #ff9900">
                            <li v-for="item in notList" :key="item">
                                {{ item }}
                            </li>
                        </ul>
                    </el-card>
                    <span style="font-size: 12px; color: #fd6a6a"
                        >红色表示未开始，橙色表示异常</span
                    >
                </el-col>
                <el-col :xs="24" :sm="6" :md="6" :lg="6" :xl="5">
                    <el-card class="notFinish">
                        <div slot="header">
                            <span>未完成{{ NeverFinish.length }}个</span>
                        </div>
                        <ul>
                            <li v-for="item in NeverFinish" :key="item">
                                {{ item }}
                            </li>
                        </ul>
                    </el-card>
                </el-col>
                <el-col :xs="24" :sm="6" :md="6" :lg="6" :xl="5">
                    <el-card class="finish">
                        <div slot="header">
                            <span> 已完成 {{ FinishList.length }}个</span>
                        </div>
                        <ul>
                            <li v-for="item in FinishList" :key="item">
                                {{ item }}
                            </li>
                        </ul>
                    </el-card>
                </el-col>
            </div>
            <div
                v-show="PQCRepairCondition != '正常' && state != false"
                class="pqcanomou"
            >
                <repairComponent></repairComponent>
            </div>
        </el-row>
    </div>
</template>

<script>
import repairComponent from "./page-components/_repair_situation";
import {
    PQCCheckF,
    Assemblechecklist,
    CheckBarCodeNumIsRepertory,
    PQCCheckB,
} from "@/api/order/pqc";
import { OrderList, DisplayGroup } from "@/api/common";
export default {
    name: "pqc-operate",
    props: {},
    data() {
        return {
            checked: false,
            groupOptions: [],
            Group: "",
            nuoOrder: "", //挪用订单号
            nuoBarCode: "", //挪用条码号
            OrderNum: "",
            OrderNumOptions: [], //订单号
            BoxBarCode: "", //箱体模组条码
            Remark: "", //备注
            notDoList: [], //未开始
            NeverFinish: [], //未完成
            FinishList: [], //已完成
            num: undefined,
            PQCRepairCondition: "正常",
            RepairOptions: [
                { value: "正常" },
                { value: "现场维修" },
                { value: "转站维修" },
            ],
            abnormalarea: [],
            isnuonum: false,
            Department: "",
            state: false,
            id: "",
            notList: [],
        };
    },
    components: { repairComponent },
    computed: {},
    watch: {
        OrderNum(val) {
            if (val != "" && val != null) {
                localStorage.setItem("Order", val);
                this.getInfo(val);
            }
        },
        num(val) {
            if (val != "" || val != undefined)
                localStorage.setItem("LineId", val);
        },
        Group(val) {
            if (val != "" || val != undefined)
                localStorage.setItem("selectgroup", val);
        },
    },
    methods: {
        getGroup() {
            DisplayGroup().then((res) => {
                this.groupOptions = res.data.Data.Group;
                this.Department = res.data.Data.department;
                //查看班组列表里是否存在本地存储的值
                let localsg = localStorage.getItem("selectgroup");
                if (localsg != null) {
                    for (let i of this.groupOptions) {
                        if (i == localsg) {
                            this.Group = localsg;
                        }
                    }
                }
            });
        },
        //获取订单状态列表
        getInfo(oNum) {
            Assemblechecklist(oNum, "PQCCheck").then((res) => {
                let not = JSON.parse(res.data.Data.NotDoList);
                this.notDoList = not;
                let Never = JSON.parse(res.data.Data.NeverFinish);
                this.NeverFinish = Never;
                let NeverList = JSON.parse(res.data.Data.FinishList);
                this.FinishList = NeverList;
                let notAbnormalFinish = JSON.parse(
                    res.data.Data.AbnormalFinish
                );
                this.notList = notAbnormalFinish;
            });
        },
        //检查挪用条码是否可以挪用
        checkorderbar(v) {
            CheckBarCodeNumIsRepertory(this.nuoOrder, v)
                .then((res) => {
                    if (res.data != true) {
                        this.$message.error(res.data);
                        return;
                    }
                })
                .catch((err) => {
                    console.log("检查失败");
                });
        },
        //开始PQC检查
        startPQC() {
            if (this.Group == "") {
                this.$message.error("请选择班组！");
                return;
            }
            console.log(this.OrderNum, 11);
            console.log(this.BoxBarCode, 221);
            if (this.OrderNum != "" && this.BoxBarCode != "") {
                let code = this.BoxBarCode.replace(/\s+/g, "");
                let obj = {
                    deprment: this.Department,
                    group: this.Group,
                    ordernum: this.OrderNum,
                    barcode: code,
                    UserName: "@UserName",
                };
                PQCCheckB(obj)
                    .then((res) => {
                        if (res.data.Result == false) {
                            this.$refs.selectCode.select();
                            this.$message.error(res.data.Message);
                        } else {
                            this.id = res.data.Data.id;
                            this.$message.success(res.data.Message);
                            this.getInfo(this.OrderNum);
                            this.state = true;
                        }
                    })
                    .catch((err) => {
                        console.log("检查失败");
                    });
            } else {
                this.$message.error("订单号，模组条码不能为空！");
            }
        },
        checkData(val) {
            console.log(this.BoxBarCode, 2222);
            if (this.state != true) {
                //不等于true是开始
                this.startPQC();
            } else {
                //PQC结束需判断是否挪用
                if (this.checked != true) {
                    this.finishPQC();
                } else {
                    //挪用
                    if (this.nuoOrder != "" && this.nuoBarCode != "") {
                        this.checkorderbar(this.nuoBarCode);
                        this.finishPQC();
                    } else {
                        this.$message.error("挪用订单号与挪用条码号不能为空！");
                        return;
                    }
                }
            }
        },
        finishPQC() {
            let str = null;
            if (this.PQCRepairCondition != "正常") {
                if (this.abnormalarea.length == 0) {
                    this.$message.error("PQC异常不能为空！");
                    return;
                } else {
                    str = this.abnormalarea.toString();
                }
            }
            if (this.num == undefined) {
                this.$message.error("产线不能为空！");
                return;
            }
            if (this.id == "") {
                this.$message.error("id不能为空！");
                return;
            }
            let nuocode = this.nuoBarCode.replace(/\s+/g, "");
            let obj = {
                id: this.id,
                remark: this.Remark,
                line: this.num,
                PQCRepairCondition: this.PQCRepairCondition,
                UserId: "@UserIds",
                Messagr: str,
                nuoOrder: this.checked == false ? null : this.nuoOrder,
                nuoBarCode: this.checked == false ? null : nuocode,
                isnuo: this.checked.toString(),
            };
            PQCCheckF(obj)
                .then((res) => {
                    console.log(res.data);
                    if (res.data.Result == true) {
                        this.$message.success(res.data.Message);
                        this.state = false;
                        this.BoxBarCode = "";
                        this.PQCRepairCondition = "正常";
                        this.checked = false;
                        this.getInfo(this.OrderNum);
                        this.abnormalarea = [];
                    } else {
                        this.$refs.selectNuoCode.select();
                        this.$message.error(res.data.Message);
                    }
                })
                .catch((err) => {
                    console.log("检查失败");
                });
        },
    },
    created() {},
    mounted() {
        this.getGroup();
        OrderList().then((res) => {
            this.OrderNumOptions = res.data.Data;
        });
        var localOrder = localStorage.getItem("Order");
        if (localOrder != null) {
            this.OrderNum = localOrder;
            this.getInfo(localOrder);
        }
        var line = localStorage.getItem("LineId");
        if (line != null) {
            this.num = line;
        }
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
.el-row {
    margin-top: 30px;
}

.inputbox {
    padding-right: 30px;
    display: flex;
    flex-flow: row wrap;
    justify-content: flex-end;
    align-items: center;
    text-align: left;
    margin: 5px;
}
.inputbox .title {
    font-size: 14px;
    margin-right: 4px;
}
</style>