<!-- src/components/JoinedRoomList.vue -->
<template>
  <div class="room-list">
    <h3>我的聊天室</h3>
    <el-menu :default-active="selectedId?.toString()" @select="onSelect">
      <el-menu-item
        v-for="room in rooms"
        :key="room.id"
        :index="room.id.toString()"
      >

      <div class="room-container">
        <el-avatar :src="room.avatarUrl" :size="25" />

        {{ room.name }}

        <el-icon>
            <Delete @click="deleteRom(room.id)" />
        </el-icon>

      </div>
      </el-menu-item>
    </el-menu>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import RoomService from '@/services/RoomService'

const emit = defineEmits(["select-room"])

const selectedId = ref(null)
const rooms = ref([])

const fetchRooms = async () => {
  try {
    const res = await RoomService.getJoinedRoom()
    rooms.value = res.data
  } catch (e) {
    console.error('获取加入的房间失败', e)
  }
}

const onSelect = (idStr) => {
  const id = parseInt(idStr)
  const room = rooms.value.find((r) => r.id === id)
  if (room) {
    selectedId.value = id
    emit('select-room', { id: room.id, name: room.name })
  }
}

const deleteRom = async (id) => {
  try {
    await RoomService.leaveRoom(id)
    fetchRooms()
  } catch (e) {
    console.error('删除房间失败', e)
  }
}

onMounted(fetchRooms)
</script>

<style scoped>
.room-list {
  width: 20vw;
  border-right: 1px solid #ccc;
  padding: 1rem;
}

.room-container {
  width: 90%;
  display: flex;
  align-items: center;
  justify-content: space-between;
}
</style>
