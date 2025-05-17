// src/services/AuthService.js
import api from '@/utils/api/api.js';
import { ElMessage } from 'element-plus';

const AuthService = {
  async register({ username, password }) {
    try {
      const res = await api.post("/Auth/register", { username, password });

      const isSuccess = res.data.code;
      if(isSuccess === 0) // code===0表示失败
      {
        ElMessage.error('注册失败:请换一个用户名称');
        throw new Error('注册失败');
      }
    
      console.log(res.data)
      return res;
    } catch (error) {
      console.error("Register error:", error);
      throw error; // 向上传递错误信息
    }
  },

  async login({ username, password }) {
    try {
      const res = await api.post("/Auth/login", { username, password });

      const isSuccess = res.data.code;
      if(isSuccess === 0) // code===0表示失败
      {
        ElMessage.error('登录失败');
        throw new Error('登录失败');
      }

      const token = res.data.data;

      if (token) {
        localStorage.setItem('userToken', token); // 保存 token
      }
      return res;
    } catch (error) {
      console.error("Login error:", error);
      throw error; // 向上传递错误信息
    }
  },

  logout() {
    localStorage.removeItem("userToken");
  },

  getToken() {
    return localStorage.getItem("userToken");
  },
};

export default AuthService;
