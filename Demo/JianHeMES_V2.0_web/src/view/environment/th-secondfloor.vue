<template>
  <div></div>
</template>

<script>
//import * as signalR from "@aspnet/signalr";
export default {
  name: "th-secondfloor",
  components: {},
  data() {
    return {
      //接收数据
      data: {},
      //代理器
      proxy: {},
      //用户
      user: {
        username: "",
        userNumber: "",
        connectionId: "",
      },
      messages: [], //返回消息
    };
  },
  mounted() {
    this.user.username = this.$userInfo.Name;
    this.user.userNumber = this.$userInfo.UserNumber;
    var cn = $.hubConnection("http://172.16.6.145/Hub2");
    this.proxy = cn.createHubProxy("Hub2");
    this.proxy.on("sendTH2", (TH2_json) => {
      this.data = TH2_json;
      console.log(TH2_json); //打印接收结果
    });
    cn.start({ jsonp: true }).done(console.log("Hub2已连通"));
  },
  methods: {},
  created() {
    // let thisVue = this;
    // this.connection = new signalR.HubConnectionBuilder()
    //   .withUrl("http://172.16.6.145:8080/chathub", {
    //     skipNegotiation: false,
    //     transport: signalR.HttpTransportType.WebSockets,
    //   })
    //   .configureLogging(signalR.LogLevel.Information)
    //   .build();
    // this.connection.on("ReceiveMessage", (user, message) => {
    //   thisVue.messages.push({ user, message });
    //   console.log({ user, message });
    // });
    // this.connection.on("ReceiveCaller", (message) => {
    //   let user = "自己"; //这里为了push不报错，我就弄了一个默认值。
    //   thisVue.messages.push({ user, message });
    //   console.log({ user, message });
    // });
    // console.log(this.connection);
    // this.connection.start({ jsonp: true }).done(function () {
    //   alert("连接成功");
    // });
  },
};
</script>