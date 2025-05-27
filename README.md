# WHU_Chat —— 基于 ASP.NET Core 与 Vue3 的即时聊天系统

## 🧑‍💻 项目简介

本项目为武汉大学《软件构造》课程的课程实践项目，由三人小组联合开发，旨在实现一个功能完善的 **即时聊天系统**，支持单聊、群聊、好友管理及多媒体传输功能。

整个系统采用 **前后端分离架构**，后端基于 `ASP.NET Core`，前端基于 `Vue 3` + `Element Plus`，通过 WebSocket 实现实时通信，并结合 MySQL 数据库进行数据存储。

---

## 技术栈

| 层 | 技术 | 描述 |
|----|------|------|
| 🖥️ 前端 | Vue 3 + Vite | 快速开发现代化 Web 应用 |
|      | Vue Router | 前端页面路由管理 |
|      | Axios | 前后端数据交互 |
|      | Element Plus | 组件库，美观高效 |
| 🌐 后端 | ASP.NET Core | 跨平台 Web 框架，支持 RESTful API 和 SignalR |
| 🔌 通信 | SignalR | 实时消息推送（基于 WebSocket） |
| 🗄️ 数据库 | MySQL | 存储用户、好友、房间、消息等数据 |

---

## ✨ 项目功能

- ✅ 用户注册与登录
- ✅ 好友添加与申请管理
- ✅ 单人聊天（WebSocket 实时通信）
- ✅ 群聊支持（房间制）
- ✅ 多媒体消息支持（图片/视频链接字段预留）
- ✅ 基本用户信息中心
- ✅ 实验文档、演示视频和最终报告整理


---


## 🚀 启动方式

### 后端(main分支)

#### 本地启动

```bash
cd WHUChat/WHUCHat.Server/
dotnet restore
dotnet run
```

#### 打包到服务器上

```bash
cd WHUChat/WHUCHat.Server/
dotnet publish -c Release -r linux-x64 --self-contained false -o ./publish-linux
```

将打包好的文件上传到服务器上

```bash
cd publish-linux
nohup dotnet WHUChat.Server.dll --urls "http://0.0.0.0:5000"   > output.log 2>&1 &
```

这时便将服务起起来了

### 前端(front分支)
```bash
npm install
npm run dev
```

## 课程与致谢
本项目为武汉大学计算机学院《软件构造》课程设计作业，感谢授课教师和助教提供的指导与建议！
