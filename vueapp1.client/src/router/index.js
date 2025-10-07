// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router';
import Home from '@/components/VHome.vue';
import About from '@/components/About.vue';
import VMap from '@/components/VMap.vue';
import VUserInfo from '@/components/VUserInfo.vue';
import VUsers from '@/components/SysMgn/vUsers.vue';
import VGroup from '@/components/SysMgn/vGroup.vue';
import VAuth from '@/components/SysMgn/vAuth.vue';
import VDataStore from '@/components/SysSto/vDataStore.vue';


import { globalState } from '@/globalState';

//把組件交給路由管理
const routes = [
  { path: '/', component: Home },
  { path: '/about', component: About },
  { path: '/Map', component: VMap },
  { path: '/UserInfo', component: VUserInfo, meta: { requiresAuth: true } },
  { path: '/SysMgn/Users', component: VUsers, meta: { requiresAuth: true, requiredRole: '系統管理員' } },
  { path: '/SysMgn/Group', component: VGroup, meta: { requiresAuth: true, requiredRole: '系統管理員' } },
  { path: '/SysMgn/Auth', component: VAuth, meta: { requiresAuth: true, requiredRole: '系統管理員' } },
  { path: '/SysSto/DataStore', component: VDataStore, meta: { requiresAuth: true, requiredRole: '系統管理員' } },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// 全域前置守衛
router.beforeEach(async (to, from, next) => {
  globalState.isLoading = true;//開啟loading特效
  let role = "";
  let isLogin = false;
  console.log('run router')
  if (to.meta.requiresAuth == undefined || globalState.UserInfo == null) {
    //檢查登入是否還有效
    try {
      if (!(globalState.isLogin && globalState.isLogin == true)) {
        //沒有儲存狀態
        var r = await axios.get('/api/SysUser/AuthCheck');
        if (r.statusText == "OK") {
          if (r.data.isSuccess == true) {
            globalState.isLogin = true;
            globalState.UserName = r.data.Result.UserName;
            globalState.UserInfo = r.data.Result;
          }
        }
      }
    }
    catch (e) {
      console.warn('權限檢查錯誤:' + e)
    };
  }

  if (globalState.UserInfo != null) {
    role = globalState.UserInfo.GroupName;
  }
  isLogin = globalState.isLogin;

  if (to.meta.requiresAuth && !isLogin) {
    ShowAlert();
    // 如果需要登入，但未登入
    next('/');
  } else if (to.meta.requiredRole && role !== to.meta.requiredRole) {
    ShowAlert();
    next('/');
  } else {
    // 通過
    next();
  }
  globalState.isLoading = false;//關閉loading特效
});

function ShowAlert() {
  // 權限不足（例如要求 admin）
  Swal.fire({
    title: "您沒有權限進入此頁面",
    icon: "error"
  });
}


export default router
