<template>
  <div class="user-info">
    <img class="avatar" :src="userAvatarUrl" alt="user avatar" />
    <div class="user-meta">
      <p class="username">{{ userName }}</p>
      <p class="userid">ID: {{ userId }}</p>
    </div>
  </div>
</template>

<script setup>
import UserService from '@/services/UserService';
import { onMounted, ref } from 'vue';

const userName = ref('');
const userId = ref('');
const userAvatarUrl = ref('');

onMounted(async () => {
  const res = await UserService.getUserInfo();
  userName.value = res.data.username;
  userId.value = res.data.id;
  userAvatarUrl.value = res.data.avatarUrl;
});

</script>

<style scoped>
.user-info {
  display: flex;
  align-items: center;
  padding: 16px 12px;
  border-bottom: 1px solid #ddd;
  /* background-color: #f8f8f8; */
}

.avatar {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  object-fit: cover;
  margin-right: 12px;
}

.user-meta {
  display: flex;
  flex-direction: column;
}

.username {
  font-weight: bold;
  font-size: 16px;
  margin: 0;
}

.userid {
  font-size: 12px;
  color: #888;
  margin: 4px 0 0 0;
}
</style>
