// src/services/AuthService.js
import api from '@/utils/api/api.js';

const AuthService = {
  async register({ username, password }) {
    try {
      const res = await api.post("/Auth/register", { username, password });
      return res;
    } catch (error) {
      console.error("Register error:", error);
      throw error; // 向上传递错误信息
    }
  },

  async login({ username, password }) {
    try {
      const res = await api.post("/Auth/login", { username, password });
      const token = res.data.data;
      console.log("Token saved", token);

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
