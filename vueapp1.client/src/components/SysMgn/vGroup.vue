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
              <span class="input-group-text">群組名稱</span>
              <input type="text"
                     class="form-control"
                     autocomplete="off"
                     v-model="querydata.GroupName">
            </div>
          </div>

          <div class="col-12">
            <button type="submit" class="btn btn-primary me-3">查詢</button>
            <button type="button" class="btn btn-warning" @click="Create()">新增群組</button>
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
        <Column field="GroupName" header="群組名稱" />
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
          <div class="card-header">
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
                  <span class="input-group-text">群組名稱</span>
                  <input type="text"
                         class="form-control"
                         autocomplete="off"
                         placeholder="請輸入群組名稱"
                         v-model="editdata.GroupName"
                         required>
                </div>
              </div>

              <div class="col-12 mt-3">
                <button type="submit" class="btn btn-primary me-3">儲存</button>
                <button type="button" class="btn btn-danger" data-bs-dismiss="modal" aria-label="Close">取消</button>
              </div>
            </div>
          </div>
          <div class="card-body">
            <TreeView :nodes="nodes"
                      :config="treeConfig" />
          </div>
        </div>
      </form>
    </template>

  </modal>

</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import DataTable from 'primevue/datatable'
  import Column from 'primevue/column'
  import modal from '../../components/ModalModel.vue'
  // 匯入 vue3-treeview
  import TreeView from 'vue3-treeview';
  import 'vue3-treeview/dist/style.css';
  import { node } from 'globals';

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
    roots: ["root"],
  });

  onMounted(() => {
    Query();
  });


  async function Query() {
    const data = { ...querydata.value };
    var d = await axios.post('/api/SysUser/GetGroupList', data);
    if (d.statusText == "OK") {
      if (d.data.isSuccess == true) {
        d = d.data.Result;
        list.value = d;
      }
    } else {
      ShowAlert(d.data.ErrorMsg);
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


  function ShowAlert(msg, iconType = "error") {
    // 權限不足（例如要求 admin）
    Swal.fire({
      title: msg,
      icon: iconType
    });
  }

  function EditRow(data) {
    editdata.value = data;
    title.value = '編輯'
    subQuery();
    editmodal.value.show();
  }


  //查詢群組權限
  async function subQuery() {
    //查詢權限清單
    const AuthList = await initAuthList();
    //查詢群組權限
    if (AuthList.length > 0) {
      var Rel = await axios.post('/api/SysUser/GetRelGroupAuth', editdata.value.GroupID);
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


  function Create() {
    editdata.value = {};
    title.value = '新增'
    initAuthList();
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
          const d = await axios.post('/api/SysUser/DeleteGroup', { id: id });
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
    const d = await axios.post('/api/SysUser/UpdateGroup', data);
    if (d.statusText == "OK") {
      if (d.data.isSuccess == true) {
        let r = d.data.Result;
        if (r.length > 0) {
          await subUpdate(r[0]);
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

  //更新群組權限
  async function subUpdate(GroupID) {
    const nodedata = { ...nodes.value };
    const datalist = [];
    delete nodedata.root;
    //資料重新組合
    for (let i in nodedata) {
      if (nodedata[i].state.checked == true) {
        datalist.push({ GroupID: GroupID, AuthID: i });
      }
    }

    const d = await axios.post('/api/SysUser/UpdateRelGroupAuth', datalist);
    if (d.statusText == "OK") {
      if (d.data.isSuccess == false) {
        ShowAlert('更新群組權限失敗:' + r.data.ErrorMsg);
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
