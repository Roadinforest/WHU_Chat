<!-- src/components/RoomChatWindow.vue -->
<template>
  <div class="chat-window">

    <div class="room-title">
      <h3>{{ props.roomName }}</h3>
      <RoomMemberListButton :roomId="props.roomId" />
    </div>

    <div class="message-box" ref="messageBox">

      <div v-if="messages.length > 0">

        <div v-for="(msg, index) in messages" :key="index" :class="{
          'message-item': true, 'my-message': msg.userName === userName,
          'other-message': msg.userName !== userName
        }">

          <div class="message-wrapper">
            <span class="username">{{ msg.userName }}</span>
            <div class="message-bubble">{{ msg.content }}</div>
          </div>

        </div>

      </div>

      <div v-else>
        <div class="message-item other-message">
            <span class="username">系统</span>
            <div class="message-bubble">你们暂时还没有开始聊天哦</div>
          </div>
      </div>

    </div>


    <div class="message-sender">
      <el-input type="textarea" v-model="inputMessage" placeholder="输入消息..." @keyup.enter="sendMsg" style="margin-left:5%" />
      <div style="width:5%"></div>
      <el-button style="margin-right:5%" @click="sendMsg" type="primary">发送</el-button>
    </div>


  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, watch ,nextTick} from 'vue'
import signalRService from '@/services/SignalRService'
import RoomService from '@/services/RoomService'
import RoomMemberListButton from './RoomMemberListButton.vue'
import UserService from '@/services/UserService'

const messageBox = ref(null)

const scrollToBottom = () => {
  nextTick(() => {
    if (messageBox.value) {
      messageBox.value.scrollTop = messageBox.value.scrollHeight
    }
  })
}


const props = defineProps(["roomId", "roomName"])
const messages = ref([])
const inputMessage = ref("")

const receiveHandler = (username, content, resUrl) => {
  messages.value.push({
    userName: username, // 映射字段名
    content,
    resUrl
  });
  scrollToBottom()
};

const userName = ref("");
const userId = ref("");
const userAvatarUrl = ref("");


onMounted(async () => {

  const res0 = await UserService.getUserInfo();
  userName.value = res0.data.username;
  userId.value = res0.data.id;
  userAvatarUrl.value = res0.data.avatarUrl;


  await signalRService.startConnection()
  signalRService.connection.on('ReceiveMessage', receiveHandler)
  await signalRService.entryRoom(props.roomId)
  const res = await RoomService.getHistory(props.roomId)
  messages.value = res.data
  scrollToBottom()

})

// 监听房间变化并进入对应房间
watch(
  () => props.roomId,
  async (newId, oldId) => {
    // console.log("roomId changed")
    if (oldId) await signalRService.quitRoom(oldId)
    if (newId) {
      await signalRService.entryRoom(newId)
      messages.value = [] // 清空旧房间记录

      const res = await RoomService.getHistory(newId)
      messages.value = res.data
      // console.log("messages: ", messages.value)
      console.log("messages: ", messages.value)

    }
    scrollToBottom()
  }
)

onBeforeUnmount(async () => {
  await signalRService.quitRoom(props.roomId)
  await signalRService.stopConnection()
})

const sendMsg = async () => {
  if (inputMessage.value.trim()) {
    await signalRService.sendMessage(props.roomId, inputMessage.value)
    inputMessage.value = ''
  }
  scrollToBottom()
}
</script>

<style scoped>
.room-title {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin: 2% 5% 0% 5%;
}

.message-box {
  height: 60vh;
  border: 1px solid #eee;
  border-radius: 25px;
  margin: 2% 5% 2% 5%;
  flex: 1;
  overflow-y: auto;
  padding: 10px;
  background-color: rgba(255, 255, 255, 0.3); /* You can adjust the color as needed */
}

.message-sender {
  display: flex;
  align-items: space-between;
  margin-bottom: 2%;
}

.chat-window {
  display: flex;
  flex-direction: column;
  flex: 1;
  height: 95%;
  padding: 10px;
  width: 30vw;

  border: 2px solid #ddd;
  border-radius: 20px;
  background-color: rgba(255, 255, 255, 0.3); /* You can adjust the color as needed */
}


/* 每条消息的容器 */
.message-item {
  display: flex;
  margin: 8px 0;
}

/* 自己的消息整体靠右显示 */
.my-message {
  justify-content: flex-end;
}

/* 别人的消息整体靠左显示 */
.other-message {
  justify-content: flex-start;
}

.message-bubble {
  padding: 10px 14px;
  border-radius: 18px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
  word-wrap: break-word;
  word-break: break-word;
  white-space: pre-wrap;
  max-width: 15vw;
  overflow-wrap: break-word; 
  overflow-x:hidden;
}


/* 自己的消息泡泡样式 */
.my-message .message-bubble {
  background-color: #cce5ff;
  color: #000;
  border-bottom-right-radius: 4px;
}

/* 别人的消息泡泡样式 */
.other-message .message-bubble {
  background-color: #ffffff;
  color: #000;
  border-bottom-left-radius: 4px;
}

.message-wrapper {
  display: flex;
  align-items: flex-end;
  gap: 6px;
}

/* 自己消息的用户名显示在右侧 */
.my-message .message-wrapper{
  flex-direction: row-reverse;
} 

/* 用户名样式 */
.username {
  font-size: 1.3em;
  color: #888;
  margin: 0 4px;
  white-space: nowrap;
}
</style>
