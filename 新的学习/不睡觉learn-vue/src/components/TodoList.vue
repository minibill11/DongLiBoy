<template>
  <div id="todolist" class="container">
    <div class="input-group mb-3">
      <div class="input-group-prepend">
        <button class="btn btn-outline-secondary" type="button" v-on:click="addTask">添加</button>
      </div>
      <input
        type="text"
        id="task_content"
        class="form-control"
        placeholder="请输入任务内容"
        aria-label
        aria-describedby="basic-addon1"
      />
    </div>

    <table class="table m-4">
      <thead>
        <th>任务</th>
      </thead>
      <tbody>
        <tr v-for="task in taskList">
          <td>
            <span>{{ task }}</span>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
 
<script>
import Store from "./store.js";
export default {
  name: "TodoList",
  data: function() {
    return {
      taskList: Store.fetch()
    };
  },
  methods: {
    addTask: function(event) {
      // 获取任务内容
      let task_content = document.querySelector("#task_content");

      // 添加任务内容到任务列表中
      this.taskList.push(task_content.value);

      // 清空任务内容输入框
      task_content.value = "";
    }
  },
  watch: {
    taskList: {
      handler: function(tasks) {
        Store.save(tasks);
      }
    }
  }
};
</script>