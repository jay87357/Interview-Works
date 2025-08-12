<template>
  <!-- Navigation-->
  <nav class="navbar navbar-nav navbar-light bg-light static-top fixed-top shadow">
    <div class="container">
      <!-- 漢堡按鈕（小螢幕才顯示） -->
      <button class="btn btn-primary d-md-none m-2"
              type="button"
              data-bs-toggle="collapse"
              data-bs-target="#sidebarMenu"
              aria-controls="sidebarMenu"
              aria-expanded="false"
              aria-label="Toggle navigation">
        <i class="bi bi-list"></i> <!-- Bootstrap Icons 漢堡圖示 -->
      </button>
      <router-link class="navbar-brand" to="/">展示網站</router-link>
      <button type="button" class="btn btn-primary" @click="btn_onclick" v-if="!state.isLogin">登入</button>
      <div class="d-flex" v-else>
        <div class="align-content-center px-3">歡迎&nbsp;<router-link to="/UserInfo">{{state.UserName}}</router-link></div>
        <button type="button" class="btn btn-primary" @click="btnLogout_onclick">登出</button>
      </div>
    </div>
  </nav>
</template>

<script setup>
  import { createApp, ref, getCurrentInstance } from 'vue'
  import { globalState } from '@/globalState'
  import VLogin from './VLogin.vue'//掛載別的頁面


  const onerr = e => console.warn('檢查權限錯誤:' + e);

  const state = globalState;//呼叫全域變數

  function btn_onclick() {
    Swal.fire({
      title: '登入',
      html: '<div id="login-placeholder"></div>', // 動態掛載點
      showConfirmButton: false, // 交由 Vue 控制按鈕
      allowEscapeKey: false,
      willOpen: () => {
        // 使用 Vue 渲染元件掛到 swal 的 div 上
        const container = document.getElementById('login-placeholder')
        // 避免多次掛載造成錯誤
        if (container.__vue_app__) {
          container.__vue_app__.unmount()
        }
        const app = createApp(VLogin)
        container.__vue_app__ = app
        app.mount(container)
      },
      willClose: () => {
        const container = document.getElementById('login-placeholder')
        if (container && container.__vue_app__) {
          container.__vue_app__.unmount()
          delete container.__vue_app__
        }
      }
    })
  }

  function btnLogout_onclick() {
    Swal.fire({
      title: "確定要登出嗎?",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "是",
      cancelButtonText: "否"
    }).then((result) => {
      if (result.isConfirmed) {
        axios.get('/api/SysUser/Logout')
          .then((r) => {
            if (r.statusText == "OK") {
              if (r.data.isSuccess == true) {
                state.isLogin = false;
                state.UserName = '';
                state.UserInfo = null;

                let timerInterval;
                Swal.fire({
                  icon: "success",
                  title: "登出成功!",
                  html: "在 <b></b> 秒後關閉視窗",
                  timer: 3000,
                  timerProgressBar: true,
                  didOpen: () => {
                    Swal.showLoading();
                    const timer = Swal.getPopup().querySelector("b");
                    timerInterval = setInterval(() => {
                      timer.textContent = `${Swal.getTimerLeft()}`;
                    }, 100);
                  },
                  willClose: () => {
                    clearInterval(timerInterval);
                  }
                }).then((result) => {
                  /* Read more about handling dismissals below */
                  if (result.dismiss === Swal.DismissReason.timer) {
                    
                  }
                });
              } else {
                Swal.fire({
                  icon: "error",
                  text: "登出失敗!",
                });
              }
            } else {
              Swal.fire({
                icon: "error",
                text: "登出失敗!",
              });
            }

          });
      }
    });
  }
</script>
