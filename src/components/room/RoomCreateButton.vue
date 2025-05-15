<!-- src/views/RoomView.vue -->
<template>
  <el-button class="button" type="primary" @click="showCreateRoomDialog = true">
    创建房间
  </el-button>

  <el-dialog title="创建房间" v-model="showCreateRoomDialog">
    <div class="room-view">
      <CreateRoomForm @room-created="handleRoomCreated" />
      <InviteFriendList v-if="createdRoom" :roomId="createdRoom.id" @invited="handleFriendInvited" />
      <!-- <RoomChatWindow v-if="createdRoom" :room="createdRoom" /> -->
    </div>
  </el-dialog>
</template>

<script setup>
import { ref } from "vue";
import CreateRoomForm from "@/components/room/CreateRoomForm.vue";
import InviteFriendList from "@/components/room/InviteFriendList.vue";
// import RoomChatWindow from "@/components/room/RoomChatWindow.vue";

const createdRoom = ref(null);
const showCreateRoomDialog = ref(false);

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

.button{
  display: block;
  width: 90%;
  margin: 0px 20px 10px 10px;
}
</style>
