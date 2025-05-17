<!-- src/components/FriendList.vue -->
<template>
  <div class="friend-list">
    <div style="display: flex; justify-content: space-between; align-items: center;">
      <h3>好友列表</h3>
      <div style="display: flex; align-items: center; gap: 16px;">
        <FriendRequestListButton  />
        <FriendAddButton  />
      </div>
    </div>


    <el-menu :default-active="selectedId?.toString()" @select="handleSelect">
      <el-menu-item v-if="friends != null" v-for="friend in friends" :key="friend.id" :index="friend.id.toString()"
        class="menu-item">
        <div class="item-container">
          <el-avatar :src="friend.avatarUrl" size="25" />
          {{ friend.username }}
          <el-icon style="vertical-align: middle">
            <Delete @click="handleDelete(friend.id)" />
          </el-icon>
        </div>
      </el-menu-item>
    </el-menu>
  </div>

</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import FriendService from '@/services/FriendService';
import FriendAddButton from '../friend/FriendAddButton.vue';
import FriendRequestListButton from '../friend/FriendRequestListButton.vue';

const emit = defineEmits(['select-friend', 'select-room'])
const props = defineProps<{ selectedId: number | null }>()

// const friends = ref<Array<{ id: number; username: string; avatarUrl?: string }>>([])
const friends = ref(null)

const handleSelect = (id: string) => {
  const friend = friends.value?.find(f => f.id === Number(id))
  if (friend) {
    emit('select-friend', { id: friend.id, name: friend.username })
    emit('select-room', { id: friend.roomId })
  }
}

const handleDelete = async (id: number) => {
  await FriendService.deleteFriend(id)
  friends.value = friends.value?.filter(friend => friend.id !== id)
}

onMounted(async () => {
  const response = await FriendService.getFriendList()
  friends.value = response.data
  console.log(friends.value)
})
</script>

<style scoped>
.friend-list {
  /* width: 250px; */
  width: 20vw;
  border-right: 1px solid #ccc;
  padding: 1rem;
}

.item-container {
  width: 80%;
  display: flex;
  align-items: center;
  justify-content: space-between;
}
</style>
