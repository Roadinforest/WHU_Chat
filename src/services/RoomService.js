// src/services/RoomService.js
import api from "@/utils/api/api"

const RoomService = {
  async createRoom({ name }) {
    try {
      const avatarUrl =
        "https://www.shutterstock.com/zh/image-photo/morrocan-turquoise-pottery-ceramics-small-traditional-2463718269";
      const response = await api.post("/room", { name, avatarUrl });
      // console.log("Room created successfully:", response.data);
      return response.data;
    } catch (error) {
      this.handleError(error);
    }
  },

  async getJoinedRoom() {
    try {
      const response = await api.get("/room/joined");
      // console.log("Joined rooms:", response.data);
      return response.data;
    } catch (error) {
      this.handleError(error);
    }
  },

  async joinRoom(roomId) {
    try {
      const response = await api.post(`/room/${roomId}/join`);
      // console.log(`Joined room ${roomId} successfully!`);
      return response.data;
    } catch (error) {
      this.handleError(error);
    }
  },

  async getRoomMember(roomId) {
    try {
      const response = await api.get(`/room/${roomId}/members`);
      // console.log(`Room members of ${roomId}:`, response.data);
      return response.data;
    } catch (error) {
      this.handleError(error);
    }
  },

  async inviteFriend(roomId, invitedUserId) {
    try {
      console.log(invitedUserId)
      const response = await api.post(`/room/${roomId}/invite`, { invitedUserId });
      console.log(`Friend ${invitedUserId} invited to room ${roomId} successfully!`);
      return response.data;
    } catch (error) {
      this.handleError(error);
    }
  },

  async leaveRoom(roomId) {
    try {
      const response = await api.delete(`/room/${roomId}/leave`);
      // console.log(`Left room ${roomId} successfully!`);
      return response.data;
    } catch (error) {
      this.handleError(error);
    }
  },

  async getHistory(roomId){
    try {
      const response = await api.post('/Chat/get_history',{roomId});
      // console.log(`History of room ${roomId}:`, response.data);
      return response.data;
    }catch(error){
      this.handleError(error);
    }
  }
};


export default RoomService;
