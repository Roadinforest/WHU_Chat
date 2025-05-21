<template>
  <aside class="sidebar">
    <UserHeader />

    <ElDivider />
    <el-button class="button" type="primary" @click="select('friend-list')">好友</el-button>
    <el-button class="button" type="primary" @click="select('chat-room')">聊天室</el-button>
    <!-- <el-button class="button" type="primary" @click="select('create-chat-room')">聊天室新建</el-button> -->
    <!-- <el-button class="button" type="primary" @click="select('friend-search')">朋友搜索</el-button> -->
    <!-- <el-button class="button" type="primary" @click="select('friend-request')">好友请求</el-button> -->
    <!-- <RoomCreateButton class="button"/> -->
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
  width: 15vw;
  /* background-color: rgb(231, 238, 241); */
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
  background-color:grey;
  border-color: grey;
}
</style>
