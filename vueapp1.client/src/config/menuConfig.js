// src/config/menuConfig.js
export default {
  SysMgn: [
    {
      name: '後台管理',
      path: '/SysMgn',
      //icon: 'bi bi-person', // 可選：Bootstrap Icon
      child: [
        { name: '使用者管理', path: '/SysMgn/Users' },
        { name: '群組管理', path: '/SysMgn/Group' },
        { name: '權限管理', path: '/SysMgn/Auth' },
      ]
    }
  ],
  
}
