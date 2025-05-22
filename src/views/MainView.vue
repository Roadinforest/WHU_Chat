<template>
  <div class="main-container">
    <SiderBar @page-change="handlePageChange" />
    
    <!-- 主体内容容器：自动占满剩余空间 -->
    <div class="main-content">

      <ChatView  v-if="currentPage === 'friend-list'" />
      <RoomView  v-else-if="currentPage === 'chat-room'" />

    </div>
  </div>
</template>


<script setup lang="ts">
import { ref } from 'vue'
import ChatView from '@/views/ChatView.vue'
import SiderBar from './SiderBar.vue'
import RoomView from './RoomView.vue'

const currentPage = ref('friend-list') // 默认页面

const handlePageChange = (page) => {
  currentPage.value = page
}

</script>

<style scoped>
.main-container {
  display: flex;
  height: 95vh;
  border-radius: 25px;
  background-color: rgba(255, 255, 255, 0.3); 
}

.main-content {
  flex: 1; /* 占据剩余所有空间 */
  padding: 10px;
  overflow-y: auto;
}
</style>
