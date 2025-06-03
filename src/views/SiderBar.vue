<template>
  <aside class="sidebar">
    <UserHeader />

    <ElDivider />
    <el-button class="button" type="primary" @click="select('friend-list')">好友</el-button>
    <el-button class="button" type="primary" @click="select('chat-room')">聊天室</el-button>
  </aside>
</template>

<script setup>
import UserHeader from '@/components/user/UserHeader.vue'
import { ElDivider } from 'element-plus'
import { defineEmits } from 'vue'

const emit = defineEmits(['page-change'])

const select = (page) => {
  emit('page-change', page)       // 通知父组件页面改变了
}
</script>

<style scoped>
.sidebar {
  margin-top: 10px;
  width: 15vw;
  padding: 10px;
  height: 95%;
  background-color: rgba(255, 255, 255, 0.3); 
  border: 2px solid #ddd;
  border-radius: 20px;
}

.button {
  display: block;
  width: 90%;
  margin: 0px 20px 10px 10px;
  background-color:rgba(255, 218, 53,0.4);
  border-color: grey;
}

.button:hover {
  background-color: rgba(255, 218, 53,0.8);
}
</style>
