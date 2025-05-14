<template>

<div class="chat-container">
      <FriendList
        @select-friend="handleSelectFriend"
        :selectedId="selectedFriendId"
      />
      <ChatWindow
        v-if="selectedFriendId"
        :friendId="selectedFriendId"
      />
</div>
</template>

<script setup>
import {ref} from 'vue'
import FriendList from '@/components/chat/FriendList.vue'
import ChatWindow from '@/components/chat/ChatWindow.vue'


const selectedFriendId = ref(null)
const selectedFriendName = ref(null)
const handleSelectFriend = (id,name) => {
  selectedFriendId.value = id
  selectedFriendName.value = name
}


</script>

<style scoped>
.chat-container {
  display: flex;
  height: 100vh;
}


</style>