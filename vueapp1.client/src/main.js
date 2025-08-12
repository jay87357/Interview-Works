import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router' // 加這行
import { globalState } from './globalState'
import PrimeVue from 'primevue/config'
import Aura from '@primeuix/themes/aura';


const app = createApp(App);

app.use(PrimeVue, {
  theme: {
    preset: Aura,
    options: {
      prefix: 'p',
      darkModeSelector: 'system',
      cssLayer: false
    }
  },
  ripple: true,
  inputStyle: 'outlined',
  locale: {
    startsWith: '開始於',
    contains: '包含',
    notContains: '不包含',
    endsWith: '結束於',
    equals: '等於',
    notEquals: '不等於',
    noFilter: '無過濾',
    lt: '小於',
    lte: '小於等於',
    gt: '大於',
    gte: '大於等於',
    dateIs: '日期是',
    dateIsNot: '日期不是',
    dateBefore: '日期在之前',
    dateAfter: '日期在之後',
    clear: '清除',
    apply: '套用',
    matchAll: '全部符合',
    matchAny: '符合任一',
    addRule: '新增規則',
    removeRule: '移除規則',
    accept: '接受',
    reject: '拒絕',
    // 其他國際化字串...
  }
});

// 設定全域變數
app.config.globalProperties.$global = globalState;

app.use(router) // 加這行
app.mount('#app')
