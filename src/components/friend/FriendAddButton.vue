<!-- src/components/friend/FriendSearch.vue -->
<template>

  <el-icon size="25">
    <CirclePlus @click="showAddFriendDialog = true"/>
  </el-icon>
  
  <el-dialog title="添加好友" v-model="showAddFriendDialog">
  <div class="search-tab">
    <input
      v-model="searchId"
      type="number"
      placeholder="输入好友ID"
    />
    <el-button @click="searchFriend">搜索</el-button>

    <div v-if="searchResult" class="search-result">
      <div class="friend-card">
        <p>好友ID: {{ searchResult.id }}</p>
        <p>昵称: {{ searchResult.username }}</p>
        <el-button @click="sendFriendRequest(searchResult.id)">添加好友</el-button>
      </div>
    </div>
  </div>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import UserService from '@/services/UserService'
import FriendService from '@/services/FriendService'
import { ElMessage } from 'element-plus'

const searchId = ref('')
const showAddFriendDialog = ref(false)
const searchResult = ref(null)

const searchFriend = async () => {
  try {
        if (searchId.value==='') {
            console.error("Id get null!!!")
        }
        const res= await UserService.getUserById(searchId.value)
        searchResult.value = res.data
    } catch (err) {
        console.error('搜索失败', err)
        searchResult.value = null
    }
}

const sendFriendRequest = async (targetId) => {
  try {
    await FriendService.sendFriendRequest(targetId)
    ElMessage.success('好友请求已发送！')
  } catch (err) {
    console.error('发送失败', err)
    ElMessage.error(err)
  }
}
</script>

<style scoped>
.search-tab {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 16px;
}

.search-result {
  background-color: #f9f9f9;
  padding: 8px;
  border: 1px solid #ddd;
}

.friend-card {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
</style>
