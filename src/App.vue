<template>
  <div class="app-layout">
    <main class="main-content">
      <router-view />
    </main>
  </div>
</template>

<script setup>
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
