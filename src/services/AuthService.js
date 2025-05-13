// src/services/AuthService.js
import api from '@/utils/api';

const AuthService = {
  register({username, password}) {
    return api.post('/api/Auth/register', {  username, password });
  },

  login({username, password}) {
    return api.post('/api/Auth/login', { username, password })
      .then(res => {
        const token = res.data.token;
        if (token) {
          localStorage.setItem('token', token); // 保存 token
        }
        return res;
      });
  },

  logout() {
    localStorage.removeItem('token');
  },

  getToken() {
    return localStorage.getItem('token');
  }
};

export default AuthService;
