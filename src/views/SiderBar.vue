<template>
  <aside class="sidebar">
    <UserHeader />

    <ElDivider />
    <button @click="select('friend-list')">好友</button>
    <button @click="select('chat-room')">聊天室</button>
    <button @click="select('friend-search')">朋友搜索</button>
    <button @click="select('friend-request')">好友请求</button>
  </aside>
</template>

<script setup>
import UserHeader from '@/components/user/userHeader.vue'
import { ElDivider } from 'element-plus'
import { defineEmits } from 'vue'

const emit = defineEmits(['page-change'])

const select = (page) => {
  emit('page-change', page)       // 通知父组件页面改变了
}
</script>

<style scoped>
.sidebar {
  width: 20vw;
  background-color: #f0f0f0;
  padding: 10px;
}

button {
  display: block;
  width: 100%;
  margin-bottom: 10px;
}
</style>
