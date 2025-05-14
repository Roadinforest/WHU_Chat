<!-- src/components/ChatWindow.vue -->
<template>
  <div class="chat-window">
    <div class="chat-header">
      <h3>与 {{ friendName }} 聊天</h3>
    </div>
    <div class="chat-messages">
      <div v-for="msg in messages" :key="msg.id" class="chat-message">
        <strong>{{ msg.senderName }}：</strong> {{ msg.content }}
      </div>
    </div>
    <div class="chat-input">
      <el-input v-model="input" @keyup.enter="sendMessage" placeholder="输入消息..." />
      <el-button type="primary" @click="sendMessage">发送</el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'

const props = defineProps<{ friendId: number }>()
const input = ref('')
const messages = ref<Array<{ id: number; senderName: string; content: string }>>([])
const friendName = ref('')

watch(
  () => props.friendId,
  (id) => {
    // 加载消息记录和好友名称（可替换成 API）
    friendName.value = id === 1 ? '小明' : '小红'
    messages.value = [
      { id: 1, senderName: '我', content: '你好！' },
      { id: 2, senderName: friendName.value, content: '你好呀～' }
    ]
  },
  { immediate: true }
)

const sendMessage = () => {
  if (!input.value.trim()) return
  messages.value.push({
    id: Date.now(),
    senderName: '我',
    content: input.value
  })
  input.value = ''
}
</script>

<style scoped>
.chat-window {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.chat-header {
  padding: 10px;
  background-color: #f5f5f5;
  border-bottom: 1px solid #ddd;
}

.chat-messages {
  flex: 1;
  padding: 10px;
  overflow-y: auto;
  background-color: #fafafa;
}

.chat-message {
  margin-bottom: 8px;
}
.chat-input {
  padding: 10px;
  display: flex;
  gap: 10px;
}
</style>
