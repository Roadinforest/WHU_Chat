<template>
  <div>
    <el-checkbox-group v-model="selected">
      <el-checkbox
        v-for="friend in friends"
        :key="friend.id"
        :label="friend.id"
      >
        {{ friend.username }}
      </el-checkbox>
    </el-checkbox-group>
    <el-button @click="invite">邀请好友</el-button>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import FriendService from "@/services/FriendService";
import RoomService from "@/services/RoomService";

const props = defineProps({ roomId: Number });
const emit = defineEmits(["invited"]);

const friends = ref([]);
const selected = ref([]);

onMounted(async () => {
  const res = await FriendService.getFriendList();
  friends.value = res.data;
});

async function invite() {
  for (const id of selected.value) {
    console.log("select id",id);
    console.log("room id",props.roomId)
    await RoomService.inviteFriend(props.roomId, id);
    emit("invited", id);
  }
}
</script>
