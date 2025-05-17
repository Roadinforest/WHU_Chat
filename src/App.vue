<template>
  <div class="app-layout">
    <main class="main-content">
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { onBeforeUnmount, onMounted } from 'vue';
import AuthService from './services/AuthService.js'

onMounted(async () => {
  const token=await AuthService.getToken()
  console.log("token(should be null)",token)
})

onBeforeUnmount(async () => {
  await AuthService.logout()
})
</script>


<style scoped>
.app-layout {
  display: flex;
  height: 100vh;
}
.main-content {
  flex: 1;
  padding: 20px;
}
</style>
