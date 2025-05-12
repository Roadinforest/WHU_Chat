// src/api/user.ts
import http from './http'

export function register(data: { username: string; password: string }) {
  return http.post('/user/register', data)
}

export function login(data: { username: string; password: string }) {
  return http.post('/user/login', data)
}
