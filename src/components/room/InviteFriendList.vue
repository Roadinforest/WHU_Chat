<template>
  <div>
    <el-checkbox-group v-if="friends.length > 0" v-model="selected">

        <el-checkbox v-for="friend in friends" :key="friend.id" :label="friend.id">
          {{ friend.username }}
        </el-checkbox>
    </el-checkbox-group>

    <div v-else>
      <p>所有好友都在这个房间中哦！</p>
    </div>

    <El-Divider />
    <div class="button-container">
      <div></div>
      <el-button @click="invite" style="width: 15%;" type="primary">邀请好友</el-button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from "vue";
import FriendService from "@/services/FriendService";
import RoomService from "@/services/RoomService";
import { ElMessage } from "element-plus";

const props = defineProps({ roomId: Number });
const emit = defineEmits(["invited"]);

const friends = ref([]);
const selected = ref([]);
const memberList = ref([]);

onMounted(async () => {
  console.log("Hello!!!")
  const res = await FriendService.getFriendList();
  const res2 = await RoomService.getRoomMember(props.roomId)
  memberList.value = res2.data
  console.log("Hello", memberList.value)
  friends.value = res.data.filter(friend => !memberList.value.some(member => member.userId === friend.id));
});

watch(() => props.roomId, async () => {
  const res = await FriendService.getFriendList();
  const res2 = await RoomService.getRoomMember(props.roomId)
  memberList.value = res2.data
  console.log("Hello", memberList.value)
  friends.value = res.data.filter(friend => !memberList.value.some(member => member.userId === friend.id));
})


async function invite() {
  for (const id of selected.value) {
    try {
      await RoomService.inviteFriend(props.roomId, id);
    } catch (e) {
      ElMessage.error("邀请好友失败")
      console.error("邀请好友失败", e);
    }
    emit("invited", id);
  }
}
</script>


<style scoped>
.button-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>