<!-- src/components/JoinedRoomList.vue -->
<template>
  <el-dialog v-model="dialogVisible" title="邀请好友" width="30%">
    <InviteFriendList v-if="dialogVisible" :roomId="selectedId" />
  </el-dialog>


  <div class="room-list">
    <div style="display: flex; justify-content: space-between; align-items: center;">
      <h3>我的聊天室</h3>
      <RoomCreateButton @create-room="fetchRooms" />
    </div>
    <el-menu :default-active="selectedId?.toString()" @select="onSelect">
      <el-menu-item v-for="room in rooms" :key="room.id" :index="room.id.toString()">

        <div class="room-container">
          <el-avatar :src="room.avatarUrl" :size="25" />

          {{ room.name }}


          <div>
            <el-icon>
              <Delete @click="deleteRom(room.id)" />
            </el-icon>


            <el-icon>
              <Plus @click="handleAddMember(room.id)" />
            </el-icon>

          </div>

        </div>
      </el-menu-item>
    </el-menu>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import RoomService from '@/services/RoomService'
import RoomCreateButton from './RoomCreateButton.vue'
import InviteFriendList from './InviteFriendList.vue'

const emit = defineEmits(["select-room"])

const selectedId = ref(null)
const rooms = ref([])
const dialogVisible = ref(false)

const handleAddMember = async (roomId) => {
  dialogVisible.value = true
  selectedId.value = roomId
}

const fetchRooms = async () => {
  try {
    const res = await RoomService.getJoinedRoom()
    // rooms.value = res.data
    rooms.value = res.data.filter(room => !room.name.startsWith("Privateroom"));
  } catch (e) {
    console.error('获取加入的房间失败', e)
  }
}

const onSelect = (idStr) => {
  const id = parseInt(idStr);
  const roomIndex = rooms.value.findIndex((r) => r.id === id);

  if (roomIndex > -1) {
    const [selectedRoom] = rooms.value.splice(roomIndex, 1);
    rooms.value.unshift(selectedRoom);

    selectedId.value = id;
    emit('select-room', { id: selectedRoom.id, name: selectedRoom.name });
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
  padding: 1rem;
  border: 2px solid #ccc;
  border-radius: 20px;
  padding: 1rem;
  height: 95%;
  margin-right: 1%;
}

.room-container {
  width: 90%;
  display: flex;
  align-items: center;
  justify-content: space-between;
}
</style>
