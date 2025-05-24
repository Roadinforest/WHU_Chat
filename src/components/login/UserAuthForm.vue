<template>
  <el-form :model="form" :rules="rules" ref="formRef" label-width="80px">
    <el-form-item label="用户名" prop="username">
      <el-input v-model="form.username" />
    </el-form-item>
    <el-form-item label="密码" prop="password">
      <el-input type="password" v-model="form.password" />
    </el-form-item>

    <div style="display: flex;justify-content: center;margin: 0 !important;">
      <el-button @click="onSubmit" style="width: 50%;">
        {{ props.mode === 'login' ? '登录' : '注册' }}
      </el-button>
    </div>
    <!-- </el-form-item> -->

  </el-form>
</template>

<script setup lang="ts">
import { ref, defineEmits } from 'vue'
import { ElMessage } from 'element-plus'
import AuthService from '@/services/AuthService'
import { useRouter } from 'vue-router'

const router = useRouter()
const formRef = ref()
const form = ref({ username: '', password: '' })
const props = defineProps(["mode"])
const emit = defineEmits(['success'])

const rules = {
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
}

const onSubmit = async () => {
  const mode = props.mode;
  await formRef.value.validate()

  try {
    if (mode === 'login') {
      await AuthService.login(form.value);
      ElMessage.success('登录成功');
      emit('success')
    } else if (mode === 'register') {
      await AuthService.register(form.value);
      ElMessage.success('注册成功');
      router.push('/login');
    }
  } catch (e) {
    console.error(e); // 输出错误详情
  }
}
</script>