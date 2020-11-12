<template>
  <div>
    <div>
      <h2>Chat</h2>
      <div class="container">
        用户名： <input type="text" id="displayname" v-model="name" /> <br />
        信息：<input type="text" id="message" v-model="message" /><br />
        <input type="button" id="sendmessage" value="Send" @click="sendmsg" />
        <div
          id="discussion"
          v-for="(item, index) in discussionData"
          :key="index"
        >
          <li>{{ item }}</li>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: "th-fourfloor",
  components: {},
  data() {
    return {
      testvalue: "",
      proxy: {},
      proxy2: {},
      th_json1: {},
      user: {
        username: "",
        userNumber: "",
        connectionId: "",
      },
      name: "Gamon",
      message: "",
      discussionData: [],
    };
  },
  mounted() {
    //console.log(this.$userInfo);
    this.user.username = this.$userInfo.Name;
    this.user.userNumber = this.$userInfo.UserNumber;
    //console.log(this.user);
    //1.创建Hub:chathub连接
    var cn = $.hubConnection("http://localhost:18863/chathub");
    //2.创建chathub代理(为代理执行回调方法服务)
    this.proxy = cn.createHubProxy("chathub");
    //3.添加前端代理方法addNewMessageToPage
    this.proxy.on("addNewMessageToPage", (name, message) => {
      //console.log(this.discussionData);
      this.discussionData.push(name + ":" + message);
    });
    //3.添加前端代理方法addNewMessageToPage1
    this.proxy.on("addNewMessageToPage1", (name, message, connectionId) => {
      //console.log(this.discussionData);
      this.user.connectionId = connectionId;
      console.log(this.user.connectionId);
      this.discussionData.push(connectionId + "," + name + ":" + message);
    });
    //4.启动Hub连接
    cn.start({ jsonp: true }).done(function () {
      console.log("通讯正常");
    });

    console.log("下面是Hub1：");
    var cn2 = $.hubConnection("http://172.16.6.145/Hub4");
    this.proxy2 = cn2.createHubProxy("Hub4");
    this.proxy2.on("sendTH4", (TH4_json) => {
      this.th_json1 = TH4_json;
      console.log(TH4_json);
    });
    cn2.start({ jsonp: true }).done(console.log("Hub4已连通"));
  },

  methods: {
    sendmsg: function () {
      //5.执行服务器代理方法Send，产生回调方法和数据
      this.proxy.invoke("Send", this.name, this.message);
      this.proxy.invoke("Send1", this.name, this.message);

      // this.proxy
      //   .invoke("Send", this.name, this.message)
      //   .done(this.discussionData.push(this.name + ":" + this.message));
    },
  },
};
</script>