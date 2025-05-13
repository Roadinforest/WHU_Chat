// src/services/FriendService.js
import api from '@/utils/api';

const FriendService = {
  getFriendList() {
    return api.get('/api/friend/list');
  },

  sendFriendRequest(receiverId) {
    return api.post('/api/friend/send-request', { receiverId });
  },

  respondFriendRequest({senderId,accept}) {
    return api.post('/api/friend/respond-request', {senderId,accept});
  },

  getReceivedRequests() {
    return api.get('/api/friend/received-requests');
  },

  getSentRequests() {
    return api.get('/api/friend/sent-requests');
  },

  deleteFriend(friendId) {
    return api.delete(`/api/friend/${friendId}`);
  },
};

export default FriendService;
