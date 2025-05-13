// src/services/FriendService.js
import api from '@/utils/api/api.js';

const FriendService = {
  async getFriendList() {
    try {
      const res = await api.get('/friend/list');
      return res.data;
    } catch (error) {
      console.error('Error fetching friend list:', error);
      throw error;
    }
  },

  async sendFriendRequest(receiverId) {
    try {
      const res = await api.post('/friend/send-request', { receiverId });
      return res;
    } catch (error) {
      console.error('Error sending friend request:', error);
      throw error;
    }
  },

  async respondFriendRequest({ senderId, accept }) {
    try {
      const res = await api.post('/friend/respond-request', { senderId, accept });
      return res;
    } catch (error) {
      console.error('Error responding to friend request:', error);
      throw error;
    }
  },

  async getReceivedRequests() {
    try {
      const res = await api.get('/friend/received-requests');
      return res;
    } catch (error) {
      console.error('Error fetching received requests:', error);
      throw error;
    }
  },

  async getSentRequests() {
    try {
      const res = await api.get('/friend/sent-requests');
      return res;
    } catch (error) {
      console.error('Error fetching sent requests:', error);
      throw error;
    }
  },

  async deleteFriend(friendId) {
    try {
      const res = await api.delete(`/friend/${friendId}`);
      return res;
    } catch (error) {
      console.error('Error deleting friend:', error);
      throw error;
    }
  },
};

export default FriendService;
