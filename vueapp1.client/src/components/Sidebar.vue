<template>
  <div class="sidebar mx-5" v-if="menuItems.length > 0">
    <ul class="sidebar-menu">
      <SidebarItem v-for="item in menuItems"
                   :key="item.path"
                   :item="item" />
    </ul>
  </div>
</template>

<script>
  import menuConfig from '@/config/menuConfig.js'
  import { computed, watch, ref } from 'vue'
  import { useRoute, useRouter } from 'vue-router'
  import SidebarItem from './SidebarItem.vue'

  export default {
    name: 'Sidebar',
    components: { SidebarItem },
    setup() {
      const route = useRoute()
      const router = useRouter()
      const menuItems = ref([]);

      watch(route, (newVal) => {
        const bigModule = newVal.path.split('/')[1] // 取得大項目名稱
        menuItems.value = menuConfig[bigModule] || []
      })

      router.isReady().then(() => {
        const bigModule = route.path.split('/')[1] // 取得大項目名稱
        menuItems.value = menuConfig[bigModule] || []
      })
      
      return { menuItems }
    }
  }
</script>

<style scoped>
  .sidebar {
    width: 240px;
    background: #f8f9fa;
    overflow-y:auto;
  }
  .sidebar-menu {
    list-style:none;
  }
</style>
