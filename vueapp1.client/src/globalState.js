// src/globalState.js
import { reactive } from 'vue'

export const globalState = reactive({
  isLogin: false,
  isLoading: false,
  UserName: "",
  UserInfo: null
})
