<!-- src/views/ChatView.vue -->
<template>
  <div class="chat-container">
    <FriendList @select-friend="handleSelectFriend" :selectedId="selectedFriendId" />
    <ChatWindow :friendId="selectedFriendId" v-if="selectedFriendId" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import FriendList from '@/components/chat/FriendList.vue'
import ChatWindow from '@/components/chat/ChatWindow.vue'

const selectedFriendId = ref<number | null>(null)

const handleSelectFriend = (id: number) => {
  selectedFriendId.value = id
}
</script>

<style scoped>
.chat-container {
  display: flex;
  height: 100vh;
}
</style>
