<!-- src/views/RoomView.vue -->
<template>
  <!-- <el-button class="button" type="primary" @click="showCreateRoomDialog = true">
    创建房间
  </el-button> -->

  <el-icon size="25">
    <CirclePlus  @click="showCreateRoomDialog = true" />
  </el-icon>

  <el-dialog title="创建房间" v-model="showCreateRoomDialog" :before-close="handleClose">
    <div class="room-view" v-if="showCreateRoomDialog">
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
import { ElMessage } from "element-plus";
// import RoomChatWindow from "@/components/room/RoomChatWindow.vue";

const createdRoom = ref(null);
const showCreateRoomDialog = ref(false);
const emits = defineEmits(["createRoom"]);

function handleRoomCreated(room) {
  createdRoom.value = room;
}

function handleFriendInvited(friendId) {
  showCreateRoomDialog.value = false;
  ElMessage.success('好友已添加！')
  console.log(`Friend ${friendId} invited.`);
  emits("createRoom");
}

</script>

<style scoped>
.room-view {
  padding: 20px;
}

.button {
  display: block;
  width: 90%;
  margin: 0px 20px 10px 10px;
}
</style>
