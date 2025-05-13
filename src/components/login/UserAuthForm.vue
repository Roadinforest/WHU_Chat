<template>
  <el-form :model="form" :rules="rules" ref="formRef" label-width="80px">
    <el-form-item label="用户名" prop="username">
      <el-input v-model="form.username" />
    </el-form-item>
    <el-form-item label="密码" prop="password">
      <el-input type="password" v-model="form.password" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="onSubmit">登录</el-button>
    </el-form-item>
  </el-form>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { ElMessage } from 'element-plus'
// import { login } from '@/api/auth'
import { useRouter } from 'vue-router'

const router = useRouter()
const formRef = ref()
const form = ref({ username: '', password: '' })

const rules = {
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
}

const onSubmit = async () => {
  // await formRef.value.validate()
  // try {
  //   const res = await login(form.value)
  //   localStorage.setItem('token', res.data.token)
  //   ElMessage.success('登录成功')
  //   router.push('/chat')
  // } catch (e) {
  //   ElMessage.error('登录失败')
  // }
}
</script>