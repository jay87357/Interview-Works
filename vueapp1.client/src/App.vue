<template>
  <VHeader></VHeader>


  <div v-if="globalState.isLoading" key="loading" class="d-flex justify-content-center align-items-center vh-100 main-loading">
    <div class="spinner-border" style="width: 10rem; height: 10rem;" role="status">
      <span class="visually-hidden">Loading...</span>
    </div>
  </div>
  <div class="container-xxl mt-3 min-vh-100 d-flex">
    <Sidebar :id="'sidebarMenu'" class="collapse d-md-block bg-light border-end"></Sidebar>
    <main class="w-100">
      <router-view></router-view>
    </main>
  </div>
  

  <VFlooter></VFlooter>
</template>

<script setup>
  import VHeader from './components/VHeader.vue'
  import VFlooter from './components/VFooter.vue'
  import { globalState } from '@/globalState';
  import Sidebar from './components/Sidebar.vue'
</script>

<style scoped>
header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
  /* 淡入淡出動畫 */
  .fade-enter-active, .fade-leave-active {
    transition: opacity 0.3s ease;
  }

  .fade-enter-from, .fade-leave-to {
    opacity: 0;
  }
</style>
