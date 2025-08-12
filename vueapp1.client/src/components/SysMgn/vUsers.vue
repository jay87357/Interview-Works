<!-- 使用者管理 -->
<template>
  <!-- 表單造型 -->
  <ThemeSwitcher />

  <!-- 查詢表單 -->
  <div class="card">
    <div class="card-body">
      <form @submit.prevent="Query()">
        <div class="row g-3">
          <div class="col-md">
            <div class="input-group">
              <span class="input-group-text">帳號</span>
              <input type="text"
                     class="form-control"
                     autocomplete="off"
                     placeholder="請輸入帳號"
                     v-model="querydata.Acc">
            </div>
          </div>

          <div class="col-md">
            <div class="input-group">
              <span class="input-group-text">使用者名稱</span>
              <input type="text"
                     class="form-control"
                     autocomplete="off"
                     placeholder="請輸入使用者名稱"
                     v-model="querydata.UserName">
            </div>
          </div>

          <div class="col-md">
            <div class="input-group">
              <span class="input-group-text">電子郵件</span>
              <input type="text"
                     class="form-control"
                     autocomplete="off"
                     placeholder="請輸入電子郵件"
                     v-model="querydata.Email">
            </div>
          </div>

          <div class="col-12">
            <button type="submit" class="btn btn-primary me-3">查詢</button>
            <button type="button" class="btn btn-warning" @click="Create()">新增使用者</button>
          </div>
        </div>
      </form>
    </div>
  </div>

  <!-- 列表 -->
  <div class="card">
    <div class="card-body">
      <DataTable :value="list"
                 tableStyle="min-width: 50rem"
                 :paginator="true"
                 :rows="10"
                 :rowsPerPageOptions="[5, 10, 20, 50]"
                 :first="first"
                 showGridlines>
        <Column header="編號">
          <template #body="slotProps">
            {{ slotProps.index + 1 }}
          </template>
        </Column>
        <Column field="Acc" header="帳號" />
        <Column field="UserName" header="使用者名稱" />
        <Column field="Email" header="電子郵件" />
        <Column header="控制">
          <template #body="{ data }">
            <Button class="btn btn-primary me-3" @click="EditRow(data)">編輯</Button>
            <Button class="btn btn-danger" @click="DelRow(data.id)">刪除</Button>
          </template>
        </Column>
      </DataTable>
    </div>
  </div>


  <!-- 編輯視窗 -->
  <modal :title="title" ref="editmodal">
    <template #body>
      <form @submit.prevent="Update()">
        <div class="card">
          <div class="card-body">
            <div class="row g-3">
              <div class="col-12">
                <div class="input-group">
                  <span class="input-group-text">ID</span>
                  <input type="text"
                         class="form-control"
                         v-model="editdata.id"
                         disabled>
                </div>
              </div>
              <div class="col-12">
                <div class="input-group">
                  <span class="input-group-text">帳號</span>
                  <input type="text"
                         class="form-control"
                         autocomplete="off"
                         placeholder="請輸入帳號"
                         v-model="editdata.Acc"
                         required>
                </div>
              </div>

              <div class="col-12" v-if="title=='新增'">
                <div class="input-group">
                  <span class="input-group-text">密碼</span>
                  <input type="password"
                         class="form-control"
                         autocomplete="off"
                         placeholder="請輸入密碼"
                         v-model="editdata.Pwd"
                         required>
                </div>
              </div>

              <div class="col-12">
                <div class="input-group">
                  <span class="input-group-text">使用者名稱</span>
                  <input type="text"
                         class="form-control"
                         autocomplete="off"
                         placeholder="請輸入使用者名稱"
                         v-model="editdata.UserName"
                         required>
                </div>
              </div>

              <div class="col-12">
                <div class="input-group">
                  <span class="input-group-text">電子郵件</span>
                  <input type="text"
                         class="form-control"
                         autocomplete="off"
                         placeholder="請輸入電子郵件"
                         v-model="editdata.Email"
                         required>
                </div>
              </div>
              <div class="col-12">
                <div class="input-group">
                  <span class="input-group-text">群組</span>
                  <div class="form-control">
                    <TreeView :nodes="nodes" :config="treeConfig" />
                  </div>
                </div>
              </div>
              <div class="col-12 mt-3">
                <button type="submit" class="btn btn-primary me-3">儲存</button>
                <button type="button" class="btn btn-danger" data-bs-dismiss="modal" aria-label="Close">取消</button>
              </div>
            </div>
          </div>
        </div>
      </form>
    </template>

  </modal>

</template>

<script setup>
  import { ref, onMounted  } from 'vue'
  import DataTable from 'primevue/datatable'
  import Column from 'primevue/column'
  import modal from '../../components/ModalModel.vue'
  // 匯入 vue3-treeview
  import TreeView from 'vue3-treeview';
  import 'vue3-treeview/dist/style.css';

  // 範例資料
  const list = ref([]);
  const querydata = ref({});
  const editdata = ref({});
  const first = ref(0);
  const editmodal = ref({})
  const title = ref('');
  const nodes = ref({});
  const treeConfig = ref({
    multiple: true,
    checkboxes: true,
    checkMode: 0,
    roots: [],
  });


  onMounted(() => {
    Query();
  });


  async function Query() {
    const data = { ...querydata.value };
    var d = await axios.post('/api/SysUser/GetUserList', data);
    if (d.statusText == "OK") {
      if (d.data.isSuccess == true) {
        d = d.data.Result;
        list.value = d;
      }
    } else {
      ShowAlert(d.data.ErrorMsg);
    }
  }

  function ShowAlert(msg, iconType = "error") {
    // 權限不足（例如要求 admin）
    Swal.fire({
      title: msg,
      icon: iconType
    });
  }

  async function EditRow(data) {
    editdata.value = data;
    title.value = '編輯'
    await GetGroupList();
    await GetRelUserGroup(data.id);
    editmodal.value.show();
  }

  async function Create() {
    editdata.value = {};
    title.value = '新增'
    await GetGroupList();
    editmodal.value.show();
  }

  function DelRow(id) {
    Swal.fire({
      showCloseButton: true,
      showConfirmButton: false,
      html:
`<p>確定要刪除這筆資料嗎?</p>
<button type="button" class="btn btn-primary yes" >確定</button>
<button type="button" class="btn btn-secondary" onclick="Swal.close()">取消</button>`,
      didOpen: () => {
        const btn = Swal.getHtmlContainer().querySelector('.yes')
        btn.addEventListener('click', async () => {
          const d = await axios.post('/api/SysUser/DeleteUser', { id: id });
          if (d.statusText == "OK") {
            if (d.data.isSuccess == true) {
              Swal.close();
              ShowAlert('刪除成功', 'success');
              Query();
            } else {
              ShowAlert('失敗:' + d.data.ErrorMsg);
            }
          } else {
            ShowAlert("連線失敗");
          }
        });
      }
    });
  }

  async function Update() {
    editmodal.value.hide();
    const data = { ...editdata.value };
    const d = await axios.post('/api/SysUser/UpdateUser', data);
    if (d.statusText == "OK") {
      if (d.data.isSuccess == true) {
        const r = d.data.Result;
        if (r.length > 0) {
          await 
          ShowAlert('操作完成', 'success');
          Query();
        } else {
          ShowAlert('失敗，沒有資料被更新')
        }
      } else {
        ShowAlert('失敗:' + r.data.ErrorMsg);
      }
    } else {
      ShowAlert("連線失敗");
    }
  }

  async function GetGroupList() {
    const d = await axios.post('/api/SysUser/GetGroupList', {});
    if (d.statusText == "OK") {
      if (d.data.isSuccess == true) {
        const r = d.data.Result;
        let obj = {};
        treeConfig.value.roots = [];
        for (let i in r) {
          const item = r[i];
          treeConfig.value.roots.push(item.id);
          obj[item.id] = {
            text: item.GroupName,
            state: {
              opened: true // ← 預設展開
            }
          }
        }
        nodes.value = obj;
        console.log(obj);
      } else {
        ShowAlert('失敗:' + r.data.ErrorMsg);
      }
    } else {
      ShowAlert("連線失敗");
    }
  }

  async function GetRelUserGroup(UserID) {
    let d = await axios.post('/api/SysUser/GetRelUserGroup', { id: UserID });
    if (d.statusText == "OK") {
      if (d.data.isSuccess == true) {
        d = d.data.Result;
        for (let i in d) {
          for (let j in nodes.value) {
            if (d[i].GroupID == j) {
              nodes.value[j].state.checked = true
              break;
            }
          }
        }
      } else {
        ShowAlert('失敗:' + r.data.ErrorMsg);
      }
    } else {
      ShowAlert("連線失敗");
    }
  }
</script>

<style scoped>
  .p-m-4 {
    padding: 1rem;
  }
</style>
