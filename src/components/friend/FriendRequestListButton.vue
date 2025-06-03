<!-- src/components/chat/FriendRequestList.vue -->
<template>
  <el-icon size="25">
    <Message @click="showRequestDialog= true" />
  </el-icon>

  <el-dialog title="好友申请" v-model="showRequestDialog">
    <div class="search-tab">

      <div class="request-list">
        <div v-if="requests.length === 0">暂无好友请求</div>
        <div v-for="req in requests" :key="req.id" class="request-card">
          <div style="display: flex; justify-content: space-between; align-items: center;">
            <p>来自用户：{{ req.senderUsername }}</p>
            <div class="actions">
              <el-button @click="respond(req.senderId, true)">接受</el-button>
              <el-button @click="respond(req.senderId, false)">拒绝</el-button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </el-dialog>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import FriendService from '@/services/FriendService'

const requests = ref([])
const showRequestDialog = ref(false)

const fetchRequests = async () => {
  const res = await FriendService.getReceivedRequests()
  requests.value = res.data
}

const respond = async (requestId, isAccepted) => {
  await FriendService.respondFriendRequest({
    senderId: requestId,
    accept: isAccepted,
  })
  await fetchRequests()
}

onMounted(() => {
  fetchRequests()
})
</script>

<style scoped>
.request-list {
  padding: 10px;
  border-left: 1px solid #ccc;
  background: #f9f9f9;
  overflow-y: auto;
}

.request-card {
  margin-bottom: 10px;
  padding: 8px;
  background: white;
  border-radius: 6px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.actions button {
  margin-right: 5px;
}
</style>
