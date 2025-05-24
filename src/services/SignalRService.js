// src/services/SignalRService.js
import * as signalR from '@microsoft/signalr'
import baseURL from '@/utils/api/baseURL'

class SignalRService {
  constructor() {
    this.connection = null
    this.isConnected = false
  }

  async startConnection() {
    if (this.isConnected) return

    const token = localStorage.getItem('userToken')
    if (!token) {
      console.warn('Token missing: cannot connect to SignalR.')
      return
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${baseURL}/chatHub`, {
        accessTokenFactory: () => token,
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build()

    this.registerListeners()

    try {
      await this.connection.start()
      this.isConnected = true
      console.log(' SignalR connected.')
    } catch (err) {
      console.error(' SignalR connect failed:', err)
    }
  }

  /**
   * 注册后端消息事件
   */
  registerListeners() {
    this.connection.on('ReceiveMessage', (username, message, resUrl) => {
      // console.log(` [${username}] ${message}`)
      // 可触发事件或调用回调更新UI
    })
  }

  /**
   * 加入房间（聊天室）
   */
  async entryRoom(roomId) {
    if (!this.connection) return
    try {
      await this.connection.invoke('EntryRoom', String(roomId))
      // console.log(`Entered room ${roomId}`)
    } catch (err) {
      console.error(' Failed to enter room:', err)
      this.startConnection()
      this.entryRoom(roomId)
    }
  }

  /**
   * 退出房间
   */
  async quitRoom(roomId) {
    if (!this.connection) return
    try {
      await this.connection.invoke('QuitFromRoom',String(roomId))
      // console.log(`Left room ${roomId}`)
    } catch (err) {
      console.error('Failed to leave room:', err)
    }
  }

  /**
   * 发送消息
   */
  async sendMessage(roomId, message, resUrl = null) {
    if (!this.connection) return
    try {
      await this.connection.invoke('SendMessage', String(roomId), String(message)
      , String(resUrl))
    } catch (err) {
      console.error('SendMessage failed:', err)
      this.startConnection()
      this.entryRoom(roomId)
    }
  }

  async deleteMessage(id) {
    if (!this.connection) return
    try {
      await this.connection.invoke('DeleteMessage', Number(id))
    } catch (err) {
      console.error('deleteMessage failed:', err)
      this.startConnection()
    }
  }
  async stopConnection() {
    if (this.connection) {
      await this.connection.stop()
      this.isConnected = false
      // console.log('SignalR disconnected.')
    }
  }
}

const signalRService = new SignalRService()
export default signalRService
