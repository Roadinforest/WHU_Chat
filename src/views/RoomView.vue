<!-- src/views/RoomChatView.vue -->
<template>
  <div class="room-chat-container">
    <JoinedRoomList @select-room="handleSelectRoom" />
    <RoomChatWindow v-if="selectedRoomId" 
          :roomId="selectedRoomId" :roomName="selectedRoomName"/>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import JoinedRoomList from '@/components/room/JoinedRoomList.vue'
import RoomChatWindow from '@/components/room/RoomChatWindow.vue'

const selectedRoomId = ref(null)
const selectedRoomName = ref(null)
const handleSelectRoom = (room) => {
  // console.log('selected room:', room)
  selectedRoomId.value = room.id
  selectedRoomName.value = room.name
}
</script>

<style scoped>
.room-chat-container {
  display: flex;
  height: 100vh;
}
</style>
