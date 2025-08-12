<template>
  <div class="container px-5">
    <div class="input-group mb-3">
      <div class="input-group-text">使用者名稱</div>
      <input class="form-control" v-model="editdata.UserName" disabled />
    </div>
    <div class="input-group mb-3">
      <div class="input-group-text">帳號</div>
      <input class="form-control" v-model="editdata.Acc" disabled />
    </div>
    <div class="input-group mb-3">
      <div class="input-group-text">群組名稱</div>
      <select class="form-control" v-model="editdata.id" disabled >
        <option v-for="(item,i) in group_option" :value="item.id" :key="i">{{item.GroupName}}</option>
      </select>
    </div>

    <div class="card mb-3">
      <div class="card-header">權限控制</div>
      <div class="card-body">
        <!-- 只有當 nodes 有值（或是長度）才渲染 Tree，避免套件在 data 為 undefined 時讀取 .roots -->
        <TreeView :nodes="nodes"
                  :config="treeConfig"
                  @node:clicked="onNodeClick"
                  />
      </div>
    </div>

      <router-link to="/" class="btn btn-primary">返回首頁</router-link>
    </div>
</template>

<script setup>
  import { reactive, ref, onMounted } from 'vue'
  import { globalState } from '@/globalState';
  // 匯入 vue3-treeview
  import TreeView from 'vue3-treeview';
  import 'vue3-treeview/dist/style.css';

  const editdata = reactive({});
  const group_option = reactive([]);

  // 樹狀資料格式
  const nodes = ref({});
  const treeConfig = ref({
    multiple: true,
    checkboxes: true,
    checkMode: 0,
    disabled: true,
    roots: ["root"],
  });

  onMounted(async () => {
    await init();
    await initAuthList();
    subQuery();
  });

  async function init() {
    var d = await axios.post('/api/SysUser/GetGroupList', { id: globalState.UserInfo.id});
    if (d.statusText == "OK") {
      if (d.data.isSuccess == true) {
        d = d.data.Result;
        for (let i in d) {
          group_option.push(d[i]);
        }
      }
    }

    for (let i in globalState.UserInfo) {
      editdata[i] = globalState.UserInfo[i];
    }
  }

  //初始化權限清單
  async function initAuthList() {
    let result = [];
    var AuthList = await axios.post('/api/SysUser/GetAuthList', {});
    if (AuthList.statusText == "OK") {
      if (AuthList.data.isSuccess == true) {
        const d = AuthList.data.Result;
        result = d;
        let obj = {
          root: {
            text: "根目錄",
            children: [],
            state: {
              opened: true // ← 預設展開
            }
          }
        };
        for (let i in d) {
          obj[d[i].id] = d[i];
          obj[d[i].id].text = d[i].AuthName;
          obj[d[i].id].state = { opened: true };// ← 預設展開
          obj[d[i].id].children = [];
          if (d[i].pid > 0) {
            d.forEach((item) => {
              if (item.id == d[i].pid) {
                item.children.push(d[i].id);
              }
            });
          } else {
            obj.root.children.push(d[i].id);
          }
        }
        nodes.value = obj;
      }
    } else {
      ShowAlert(d.data.ErrorMsg);
    }
    return result;
  }

  //查詢群組權限
  async function subQuery() {
    //查詢權限清單
    const AuthList = await initAuthList();
    //查詢群組權限
    if (AuthList.length > 0) {
      var Rel = await axios.post('/api/SysUser/GetRelGroupAuth', globalState.UserInfo);
      if (Rel.statusText == "OK") {
        if (Rel.data.isSuccess == true) {
          Rel = Rel.data.Result;
          for (let i in Rel) {
            for (let j in nodes.value) {
              if (Rel[i].AuthID == nodes.value[j].id) {
                nodes.value[j].state.checked = true;
                break;
              }
            }
          }
        }
      } else {
        ShowAlert(d.data.ErrorMsg);
      }
    }
  }
</script>

<style>
</style>
