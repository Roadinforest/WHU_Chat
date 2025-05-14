import api from "@/utils/api/api.js";

const UserService = {
  async getUserInfo() {
    try {
      const res = await api.get(`/user/me`);
      return res.data;
    } catch (error) {
      console.error(error);
    }
  },

  async getUserById(userId) {
    try {
        const res = await api.get(`/user/${userId}`);
        return res.data;
    } catch (error) {
      console.error(error);
    }
  },
};

export default UserService;
