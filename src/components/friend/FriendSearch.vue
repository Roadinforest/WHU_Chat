<!-- src/components/friend/FriendSearch.vue -->
<template>
  <div class="search-tab">
    <input
      v-model="searchId"
      type="number"
      placeholder="输入好友ID"
    />
    <button @click="searchFriend">搜索</button>

    <div v-if="searchResult" class="search-result">
      <div class="friend-card">
        <p>好友ID: {{ searchResult.id }}</p>
        <p>昵称: {{ searchResult.username }}</p>
        <button @click="sendFriendRequest(searchResult.id)">添加好友</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import UserService from '@/services/UserService'
import FriendService from '@/services/FriendService'

const searchId = ref('')
// const searchResult = ref<{ id: number; name: string } | null>(null)
const searchResult = ref(null)

const searchFriend = async () => {
  try {
        if (searchId.value==='') {
            console.error("Id get null!!!")
        }
        const res= await UserService.getUserById(searchId.value)
        searchResult.value = res.data
        console.log("get frind info", res.data)
    } catch (err) {
        console.error('搜索失败', err)
        searchResult.value = null
    }
}

const sendFriendRequest = async (targetId) => {
    try {
        console.log("send friend request", targetId)
        await FriendService.sendFriendRequest(targetId)
        alert('好友请求已发送！')
    } catch (err) {
        console.error('发送失败', err)
        alert('发送好友请求失败')
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
