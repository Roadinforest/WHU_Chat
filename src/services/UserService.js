import api from "@/utils/api/api.js";

const UserService = {
  async getUserInfo() {
    try {
      const res = await api.get(`/api/user/me`);
      console.log(res.data);
      return res.data;
    } catch (error) {
      console.log(error);
    }
  },

  async getUserById(userId) {
    try {
        const res = await api.get(`/api/user/${userId}`);
        console.log(res.data);
        return res.data;
    } catch (error) {
      console.log(error);
    }
  },
};

export default UserService;
