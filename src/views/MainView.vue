<template>
  <div class="chat-container">
    <SiderBar @page-change="handlePageChange" />
    
    <!-- 主体内容容器：自动占满剩余空间 -->
    <div class="main-content">

      <div class="chat-room" v-if="currentPage === 'friend-list'">
      <FriendList
        @select-friend="handleSelectFriend"
        :selectedId="selectedFriendId"
      />
      <ChatWindow
        v-if="selectedFriendId"
        :friendId="selectedFriendId"
      />
      </div>

      <FriendSearch v-else-if="currentPage === 'friend-search'" />
      <FriendRequestList v-else-if="currentPage === 'friend-request'" />
    </div>
  </div>
</template>


<script setup lang="ts">
import { ref } from 'vue'
import FriendList from '@/components/chat/FriendList.vue'
import ChatWindow from '@/components/chat/ChatWindow.vue'
import FriendRequestList from '@/components/friend/FriendRequestList.vue'
import FriendSearch from '@/components/friend/FriendSearch.vue'
import SiderBar from './SiderBar.vue'

const selectedFriendId = ref(null)
const handleSelectFriend = (id) => {
  selectedFriendId.value = id
}

const currentPage = ref('friend-list') // 默认页面

const handlePageChange = (page) => {
  currentPage.value = page
}
</script>

<style scoped>
.chat-container {
  display: flex;
  height: 100vh;
}

.chat-room {
  display: flex;
  height: 100%;
}

.main-content {
  flex: 1; /* 占据剩余所有空间 */
  padding: 10px;
  overflow-y: auto;
}
</style>
