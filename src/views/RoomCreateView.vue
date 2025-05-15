<!-- src/views/RoomView.vue -->
<template>
  <div class="room-view">
    <CreateRoomForm @room-created="handleRoomCreated" />
    <InviteFriendList
      v-if="createdRoom"
      :roomId="createdRoom.id"
      @invited="handleFriendInvited"
    />
    <!-- <RoomChatWindow v-if="createdRoom" :room="createdRoom" /> -->
  </div>
</template>

<script setup>
import { ref } from "vue";
import CreateRoomForm from "@/components/room/CreateRoomForm.vue";
import InviteFriendList from "@/components/room/InviteFriendList.vue";
// import RoomChatWindow from "@/components/room/RoomChatWindow.vue";

const createdRoom = ref(null);

function handleRoomCreated(room) {
  createdRoom.value = room;
}

function handleFriendInvited(friendId) {
  console.log(`Friend ${friendId} invited.`);
}
</script>

<style scoped>
.room-view {
  padding: 20px;
}
</style>
