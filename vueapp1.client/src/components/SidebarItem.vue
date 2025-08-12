<template>
  <li>
    <!-- 有子項目 -->
    <template v-if="item.child && item.child.length">
      <div class="menu-title"
           :class="{ active: isActive }">
        <i :class="item.icon" v-if="item.icon"></i> {{ item.name }}
      </div>
      <ul class="submenu">
        <SidebarItem v-for="sub in item.child"
                     :key="sub.path"
                     :item="sub" />
      </ul>
    </template>

    <!-- 無子項目 -->
    <template v-else>
      <!-- 當前路由項目用 span/p，其他用 router-link -->
      <component :is="isActive ? 'span' : 'router-link'"
                 :to="!isActive ? item.path : undefined"
                 class="menu-link"
                 :class="{ active: isActive }">
        <i :class="item.icon" v-if="item.icon"></i> {{ item.name }}
      </component>
    </template>
  </li>
</template>

<script>
  import { computed } from 'vue'
  import { useRoute } from 'vue-router'

  export default {
    name: 'SidebarItem',
    props: {
      item: { type: Object, required: true }
    },
    components: {
      SidebarItem: () => import('./SidebarItem.vue') // 自遞迴
    },
    setup(props) {
      const route = useRoute()

      // 判斷是否為當前路由
      const isActive = computed(() => {
        return route.path === props.item.path
      })

      return { isActive }
    }
  }
</script>

<style scoped>
  .submenu {
    list-style:none;
  }

  .menu-title {
    font-weight: bold;
    cursor: pointer;
  }

  .menu-link {
    display: block;
    padding: 4px 8px;
    text-decoration: none;
    color: #333;
  }

    .menu-link.active,
    .menu-title.active {
      background-color: #007bff;
      color: white;
      border-radius: 4px;
    }
</style>
