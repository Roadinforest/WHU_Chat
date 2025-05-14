<!-- src/views/ChatView.vue -->
<template>
  <div class="chat-container">
    <div class="sidebar">
      <FriendSearch />
      <FriendList @select-friend="handleSelectFriend" :selectedId="selectedFriendId" />
    </div>
    <ChatWindow :friendId="selectedFriendId" v-if="selectedFriendId" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import FriendList from '@/components/chat/FriendList.vue'
import ChatWindow from '@/components/chat/ChatWindow.vue'
import FriendSearch from '@/components/friend/FriendSearch.vue'

// const selectedFriendId = ref<number | null>(null)
const selectedFriendId = ref(null)
const handleSelectFriend = (id) => {
  selectedFriendId.value = id
}
</script>

<style scoped>
.chat-container {
  display: flex;
  height: 100vh;
}

.sidebar {
  width: 300px;
  padding: 1rem;
  border-right: 1px solid #ccc;
  display: flex;
  flex-direction: column;
}
</style>
