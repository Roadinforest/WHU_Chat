// src/services/ChatService.js
import api from "@/utils/api/api"

const ChatService = {

  async rmMessage(id){
    try {
      const response = await api.delete(`/Chat/delete_message/${id}`);
      return response.data;
    }catch(error){
      this.handleError(error);
    }
  }
};


export default ChatService;
