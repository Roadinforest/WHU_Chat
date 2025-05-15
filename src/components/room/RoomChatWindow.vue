<!-- src/components/RoomChatWindow.vue -->
<template>
  <div class="chat-window">
    <h3>聊天室 #{{ props.roomName }}-{{ props.roomId }}</h3>
    <div class="message-box">
      <div v-for="(msg, index) in messages" :key="index">
        <strong>{{ msg.username }}：</strong> {{ msg.content }}
      </div>
    </div>

    <div class="message-container">
      <el-input v-model="inputMessage" placeholder="输入消息..." @keyup.enter="sendMsg" style="margin-left:5%" />
      <div style="width:5%"></div>
      <el-button style="margin-right:5%" @click="sendMsg" type="primary">发送</el-button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, watch } from 'vue'
import signalRService from '@/services/SignalRService'

const props = defineProps(["roomId", "roomName"])
const messages = ref([])
const inputMessage = ref("")

const receiveHandler = (username, content, resUrl) => {
  messages.value.push({ username, content, resUrl })
}

// 监听房间变化并进入对应房间
watch(
  () => props.roomId,
  async (newId, oldId) => {
    if (oldId) await signalRService.quitRoom(oldId)
    if (newId) {
      await signalRService.entryRoom(newId)
      messages.value = [] // 清空旧房间记录
    }
  }
)

onMounted(async () => {
  await signalRService.startConnection()
  signalRService.connection.on('ReceiveMessage', receiveHandler)
})

onBeforeUnmount(async () => {
  await signalRService.quitRoom(props.roomId)
  await signalRService.stopConnection()
})

const sendMsg = async () => {
  if (inputMessage.value.trim()) {
    await signalRService.sendMessage(props.roomId, inputMessage.value)
    inputMessage.value = ''
  }
}
</script>

<style scoped>
.chat-window {
  flex: 1;
  padding: 1rem;
}

.message-box {
  height: 60vh;
  overflow-y: auto;
  border: 1px solid #eee;
  margin-bottom: 1rem;
  padding: 0.5rem;
  margin: 5%;
}

.message-container {
  display: flex;
  align-items: space-between;
}
</style>
