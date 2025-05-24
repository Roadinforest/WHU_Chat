// src/services/RoomService.js
import api from "@/utils/api/api"

const FileService = {
  async deleteFile(url){
    try {
      const response = await api.post('/fileupload/delete',{url});
      console.log(`Delete file ${url}:`, response.data);
      return response.data;
    }catch(error){
      this.handleError(error);
    }
  }
};


export default FileService;


