<!-- src/views/LoginRegisterView.vue -->
<template>
  <div class="auth-page">
    <el-card class="auth-card">
      <div class="auth-tabs">
        <el-tabs v-model="activeTab" stretch>
          <el-tab-pane label="登录" name="login">
            <UserAuthForm mode="login" @success="handleSuccess" />
          </el-tab-pane>
          <el-tab-pane label="注册" name="register">
            <UserAuthForm mode="register" @success="handleSuccess" />
          </el-tab-pane>
        </el-tabs>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import UserAuthForm from '@/components/login/UserAuthForm.vue'

const activeTab = ref('login')
const router = useRouter()

const handleSuccess = () => {
  router.push('/chat') // 登录注册成功跳转聊天页
}
</script>

<style scoped>
.auth-page {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background: #f2f2f2;
}
.auth-card {
  width: 400px;
  padding: 20px;
}
</style>
