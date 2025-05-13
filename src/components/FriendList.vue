<!-- src/components/FriendList.vue -->
<template>
  <el-menu class="friend-list" :default-active="selectedId?.toString()" @select="handleSelect">
    <el-menu-item v-for="friend in friends" :key="friend.id" :index="friend.id.toString()">
      <el-avatar :src="friend.avatarUrl" class="mr-2" />
      {{ friend.username }}
    </el-menu-item>
  </el-menu>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const emit = defineEmits(['select-friend'])
const props = defineProps<{ selectedId: number | null }>()

const friends = ref<Array<{ id: number; username: string; avatarUrl?: string }>>([])

const handleSelect = (id: string) => {
  emit('select-friend', Number(id))
}

onMounted(() => {
  // 示例静态数据，可替换为 API 请求
  friends.value = [
    { id: 1, username: '小明', avatarUrl: '' },
    { id: 2, username: '小红', avatarUrl: '' }
  ]
})
</script>

<style scoped>
.friend-list {
  width: 250px;
  border-right: 1px solid #ddd;
  padding-top: 10px;
}
.mr-2 {
  margin-right: 10px;
}
</style>
