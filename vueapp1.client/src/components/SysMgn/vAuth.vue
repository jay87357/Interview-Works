<!-- 權限管理 -->
<template>


  <!-- 查詢表單 -->
  <div class="card">
    <div class="card-body">
      <button type="button"
              class="btn btn-warning me-3"
              @click="Create()"
              v-bind:disabled="SelectNode==null">
        新增權限項目
      </button>
      <button type="button"
              class="btn btn-danger"
              @click="DelRow()"
              v-bind:disabled="SelectNode==null">
        刪除權限項目
      </button>
    </div>
  </div>

  <!-- 列表 -->
  <div class="card">
    <div class="cust-load" v-if="isLoad">
      <div class="spinner-border">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <div class="card-body">
      <TreeView :nodes="nodes"
                :config="treeConfig"
                @node-focus="onNodeFocus"
                @node-blur="onNodeLeave" />
    </div>
  </div>
  


  <!-- 編輯視窗 -->
  <modal :title="title" ref="editmodal">
    <template #body>
      <form @submit.prevent="Update()">
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
              <span class="input-group-text">上一層代碼</span>
              <input type="text"
                     class="form-control"
                     v-model="editdata.pid"
                     disabled>
            </div>
          </div>
          <div class="col-12">
            <div class="input-group">
              <span class="input-group-text">權限項目名稱</span>
              <input type="text"
                     class="form-control"
                     autocomplete="off"
                     placeholder="請輸入權限項目名稱"
                     v-model="editdata.AuthName"
                     required>
            </div>
          </div>
          <div class="col-12">
            <div class="input-group">
              <span class="input-group-text">權限代碼</span>
              <input type="text"
                     class="form-control"
                     v-model="editdata.AuthCode"
                     disabled>
            </div>
          </div>
          <div class="col-12">
            <div class="input-group">
              <span class="input-group-text">權限類型</span>
              <select class="form-control" v-model="editdata.AuthType" @change="AuthTypeSelect">
                <option v-for="(item, i) in AuthType_option" :value="item.value">{{item.text}}</option>
              </select>
            </div>
          </div>
          <div class="col-12 mt-3">
            <button type="submit" class="btn btn-primary me-3">儲存</button>
            <button type="button" class="btn btn-danger" data-bs-dismiss="modal" aria-label="Close">取消</button>
          </div>
        </div>
      </form>
    </template>

  </modal>

</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import modal from '../../components/ModalModel.vue'
  import TreeView from 'vue3-treeview';

  // 範例資料
  const querydata = ref({});
  const editdata = ref({});
  const first = ref(0);
  const editmodal = ref({})
  const title = ref('');
  const AuthType_option = ref([
    { value: 'model', text: '分類' },
    { value: 'page', text: '頁面' },
    { value: 'fun', text: '功能' }
  ]);
  const SelectNode = ref(null);
  const nodes = ref({});
  const treeConfig = ref({
    multiple: true,
    roots: ["root"]
  });
  const isLoad = ref(false);

  onMounted(() => {
    Query();
  });


  async function Query() {
    isLoad.value = true;
    var d = await axios.post('/api/SysUser/GetAuthList', {});
    if (d.statusText == "OK") {
      if (d.data.isSuccess == true) {
        d = d.data.Result;
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
        console.log(JSON.stringify(obj))
      }
    } else {
      ShowAlert(d.data.ErrorMsg);
    }
    isLoad.value = false;
  }

  function EditRow(data) {
    editdata.value = data;
    title.value = '編輯'
    editmodal.value.show();
  }

  function Create() {
    editdata.value = {};

    if (SelectNode.value.id == 'root') {
      //根目錄
      editdata.value.pid = 0;
    } else {
      //非根目錄
      editdata.value.pid = SelectNode.value.id;
    }
    
    title.value = '新增 - ' + SelectNode.value.text + ' ↓';
    editmodal.value.show();
  }

  function DelRow() {
    const node = { ...SelectNode.value };
    const id = node.id;
    console.log(id)
    if (id == "root") {
      Swal.fire('根目錄禁止刪除');
    } else {
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
            const d = await axios.post('/api/SysUser/DeleteAuth', { id: id });
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

  }

  async function Update() {
    editmodal.value.hide();
    const data = { ...editdata.value };
    delete data.root;
    const d = await axios.post('/api/SysUser/UpdateAuth', data);
    if (d.statusText == "OK") {
      if (d.data.isSuccess == true) {
        const r = d.data.Result;
        if (r.length > 0) {
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

  function onNodeFocus(node) {
    SelectNode.value = node;
  }

  function onNodeLeave() {
    setTimeout(() => {
      SelectNode.value = null;
    }, 2000);
  }

  function AuthTypeSelect(e) {
    var v = e.target.value;
    if (v) {
      editdata.value.AuthCode = v.substring(0, 1).toUpperCase();
    }
  }

  function ShowAlert(msg, iconType = "error") {
    // 權限不足（例如要求 admin）
    Swal.fire({
      title: msg,
      icon: iconType
    });
  }
</script>

<style scoped>
  .p-m-4 {
    padding: 1rem;
  }
  .tree-block {
    position: relative;
    width: 100%;
    height: 100%;
    background-color: rgba(0,0,0,0.3);
  }
  .cust-load {
    position: absolute;
    z-index: 10000;
    top:50%;
    left: 50%;
    transform: translate(-50%,-50%);
  }
</style>
