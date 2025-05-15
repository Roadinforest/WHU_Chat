<template>
  <el-row @submit.prevent="create">
    <el-form-item style="width: 70%;" label="房间名">
      <el-input v-model="roomName" />
    </el-form-item>
    <el-button style="width:15%;margin-left:5%"  type="primary" @click="create">创建房间</el-button>
  </el-row>
</template>

<script setup>
import { ref } from "vue";
import RoomService from "@/services/RoomService";

const emit = defineEmits(["room-created"]);
const roomName = ref("");

async function create() {
  if (!roomName.value.trim()) return;
  const res = await RoomService.createRoom({ name: roomName.value });
  const room = res.data;
  console.log("create room", room)

  emit("room-created", room);
}
</script>
