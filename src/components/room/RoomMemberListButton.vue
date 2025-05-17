<template>
  <div>
    <el-icon size="25">
      <User @click="showFriendListDialog = true"/>
    </el-icon>

    <el-dialog title="聊天室成员" v-model="showFriendListDialog">
      <div class="friend-list-container">
        <div v-for="member in memberList" :key="member.id" class="friend-item">
          {{ member.username }}
        </div>
      </div>
    </el-dialog>

  </div>
</template>

<script setup>
import { ref, onMounted, watch } from "vue";
import RoomService from "@/services/RoomService";

const props = defineProps({ roomId: Number });
const showFriendListDialog = ref(false);

const memberList = ref([]);

onMounted(async () => {
  console.log("Hello!!!")
  const res = await RoomService.getRoomMember(props.roomId)
  console.log("Hello", memberList.value)
  memberList.value = res.data
});

watch(() => props.roomId, async () => {
  const res = await RoomService.getRoomMember(props.roomId)
  memberList.value = res.data
  console.log("Hello", memberList.value)
})

</script>


<style scoped>
.friend-list-container {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.friend-item {
  padding: 6px 12px;
  border-bottom: 1px solid #eee;
  transition: background-color 0.3s ease;
}

.friend-item:hover {
  background-color: #f5f7fa;
}
</style>