<template>

<div class="chat-container">
      <FriendList
        @select-friend="handleSelectFriend"
        @select-room="handleSelectRoom"
        :selectedId="selectedFriendId"
      />
      <RoomChatWindow
        v-if="selectedRoomId"
        :roomId="selectedRoomId"
        :roomName="selectedFriendName"
      />
</div>
</template>

<script setup>
import {ref} from 'vue'
import FriendList from '@/components/chat/FriendList.vue'
import RoomChatWindow from '@/components/room/RoomChatWindow.vue'


const selectedFriendId = ref(null)
const selectedFriendName = ref(null)

const selectedRoomId = ref(null)
// const selectedRoomName = ref(null)
const handleSelectFriend = (friend) => {
  selectedFriendId.value = friend.id
  selectedFriendName.value = friend.name 
}

const handleSelectRoom = (room) => {
  // console.log('selected room:', room)
  selectedRoomId.value = room.id
  // selectedRoomName.value = room.name
}

</script>

<style scoped>
.chat-container {
  display: flex;
  height: 100vh;
}


</style>