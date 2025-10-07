<template>
  <div class="input-group pb-3">
    <div class="input-group-text">帳號</div>
    <input class="form-control" v-model="postdata.Acc" placeholder="請輸入帳號" v-bind:disabled="isLock" required />
  </div>
  <div class="input-group pb-3">
    <div class="input-group-text">密碼</div>
    <input class="form-control" v-model="postdata.Pwd" type="password" v-bind:disabled="isLock" placeholder="請輸入密碼" required />
  </div>


  <div class="spinner-border" role="status" v-if="captchaUrl==''" v-bind:disabled="isLock" style="cursor:wait">
    <span class="visually-hidden">Loading...</span>
  </div>
  <div class="input-group pb-3" v-else>
    <div class="input-group-text">驗證碼</div>
    <img @click="Img_click" class="border" :src="captchaUrl" alt="點擊更新驗證碼" style="cursor:pointer;" />
    <input class="form-control" v-model="postdata.Captcha" type="text" placeholder="請輸入驗證碼" required />
  </div>
  <button @click="doLogin" class="btn btn-primary mx-3" v-bind:disabled="isLock">登入</button>
  <button @click="btnCancel_click" class="btn btn-danger" v-bind:disabled="isLock">取消</button>
</template>

<script setup>
  import { ref, getCurrentInstance } from 'vue';
  
  import { globalState } from '@/globalState'
  
  const state = globalState;//呼叫全域變數
  
  //模型變數
  const captchaUrl = ref('');
  const postdata = ref({
    Acc: '',
    Pwd: '',
    Captcha: ''
  });
  const isLock = ref(false);

  function Img_click() {
    axios.get('/api/Captcha', { responseType: 'blob' })
      .then((response) => {
        if (captchaUrl.value) URL.revokeObjectURL(captchaUrl.value);
        const blob = new Blob([response.data], { type: 'image/png' });
        captchaUrl.value = URL.createObjectURL(blob);
      });
  }

  function doLogin() {
    isLock.value = true;
    const r = axios.post('/api/SysUser/Auth', postdata.value, {
      headers: {
        'Content-Type': 'application/json'
      }
    });
    r.then((response) => {
      if (response.statusText == "OK") {
        if (response.data.isSuccess == true) {
          isLock.value = false
          state.isLogin = true;
          state.UserName = response.data.Result.UserName;
          state.UserInfo = response.data.Result;
          let timerInterval;
          Swal.fire({
            icon: "success",
            title: "登入成功!",
            html: "在 <b></b> 秒後關閉視窗",
            timer: 3000,
            timerProgressBar: true,
            didOpen: () => {
              Swal.showLoading();
              const timer = Swal.getPopup().querySelector("b");
              timerInterval = setInterval(() => {
                let sec = Swal.getTimerLeft();
                sec = Math.round(sec / 1000);
                timer.textContent = `${sec}`;
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
            text: "登入失敗!",
          });
          isLock.value = false;
        }
      }
    });
    r.catch((error) => {
      Swal.fire({
        icon: "error",
        text: "登入失敗!",
      });
      isLock.value = false;
    });
  }

  function btnCancel_click() {
    Swal.close()
  }

  Img_click();
</script>
