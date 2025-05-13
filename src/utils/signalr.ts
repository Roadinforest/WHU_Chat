import * as signalR from '@microsoft/signalr'

export const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5000/chatHub') // 后端 signalr 地址
  .withAutomaticReconnect()
  .build()
