<template>
  <!-- 表單造型 -->
  <ThemeSwitcher />
  <div class="card">
    <div class="card-header text-center">
      <h1>資料匯入工具</h1>
    </div>



    <div class="card-body">
      <div class="drop-zone ms-3"
           v-bind:class="{ 'is-dragover': isDragOver }"
           @dragover.prevent="onDragOver"
           @dragleave.prevent="onDragLeave"
           @drop.prevent="onDrop"
           @click="openFileDialog">
        <p>拖曳檔案到此處，或點擊此處選擇檔案</p>
        
        <input ref="fileInput"
               type="file"
               class="hidden-input"
               accept=".csv"
               @change="onFileInputChange" />
      </div>

      <div>
        <label class="form-label ms-3"><span class="text-danger">*</span>支援格式: CSV</label>
      </div>
      <div class="ms-3">
        <div class="input-group">
          <label class="input-group-text">檔案名稱:</label>
          <input type="text" v-model="txtFileName" class="form-control" disabled />
          <div class="ms-3">
            <button class="btn btn-primary" @click="uploadFile()">送出</button>
          </div>
        </div>
      </div>
      <div class="progress ms-3 mt-3" v-if="percent > 0">
        <div class="progress-bar"
             ref="domProgressBar"
             role="progressbar"
             :aria-valuenow="percent"
             aria-valuemin="0"
             aria-valuemax="100">
        {{percent}}%
        </div>
      </div>
    </div>

    <div class="card-footer">
      <h1 class="text-center">已上傳資料</h1>
      <div class="row">
        <div class="col">
          <div class="input-group">
            <div class="input-group-text">表單名稱</div>
            <input type="text"
                   class="form-control"
                   v-model="querydata.Name" />
          </div>
        </div>
        <div class="col">
          <div class="input-group">
            <div class="input-group-text">上傳日期</div>
            <input type="text"
                   class="form-control"
                   v-model="querydata.UploadDate" />
          </div>
        </div>
        <div class="col">
          <div class="input-group">
            <div class="input-group-text">備註</div>
            <input type="text"
                   class="form-control"
                   v-model="querydata.Memo" />
          </div>
        </div>
      </div>
      <div class="my-3">
        <button class="btn btn-primary" @click="QueryData">查詢</button>
      </div>
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
        <Column field="Name" header="表單名稱" />
        <Column field="UploadDate" header="上傳日期" />
        <Column field="Memo" header="備註" />
        <Column header="">
          <template #body="{ data }">
            <button class="btn btn-primary me-3" @click="EditRow(data)">編輯</button>
            <button class="btn btn-danger" @click="DelRow(data.id)">刪除</button>
          </template>
        </Column>
      </DataTable>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import modal from '../../components/ModalModel.vue'
  import DataTable from 'primevue/datatable'
  import Column from 'primevue/column'
  import { node } from 'globals';

  const isDragOver = ref(false);
  const fileInput = ref(null);
  const txtFileName = ref("無");
  const domProgressBar = ref(null);
  const percent = ref(0);
  const list = ref([]);
  const querydata = ref({});

  onMounted(() => {
    QueryData();
  });

  //當拖曳進來時改變樣式
  const onDragOver = () => {
    isDragOver.value = true;
  };
  //當拖曳離開時恢復樣式
  const onDragLeave = () => {
    isDragOver.value = false;
  };
  // 當拖曳進來放開後
  const onDrop = (event) => {
    isDragOver.value = false;
    const files = event.dataTransfer.files;

    if (files.length > 0) {
      handleFiles(files);
    }
  };
  //當選擇檔案後
  const onFileInputChange = (event) => {
    const files = event.dataTransfer.files;
    if (files.length > 0) {
      handleFiles(files);
    }
  };

  function handleFiles(files) {
    const file = files[0];
    const name = file.name;
    //更新顯示檔案名稱
    txtFileName.value = name;
  }

  //點擊上傳檔案
  const openFileDialog = () => {
    fileInput.value.click();
  }

  //當點擊上傳
  const uploadFile = async (file) => {
    const formData = new FormData();
    formData.append("file", file);

    try {
      const res = await axios.post("/api/SysSto/UpLoad", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
        onUploadProgress: (progressEvent) => {
          if (progressEvent.total) {
            const pst = Math.round(
              (progressEvent.loaded * 100) / progressEvent.total
            );
            dom.value.style.width = pst + "%";
          }
        },
      });

      ShowAlert('更新成功', 'success');
      fileInput.value.type = "text";
      fileInput.value.type = "file";
      percent.value = 0;
      QueryData();
    } catch (err) {
      console.error("上傳失敗", err);
    }
  };

  async function QueryData() {
    const d = await axios.post("/api/SysSto/GetList", querydata.value);
    if (d.statusText == "OK") {
      if (d.data.isSuccess == true) {
        d = d.data.Result;
        list.value = d;
      }
    } else {
      ShowAlert(d.data.ErrorMsg);
    }
  }

  function EditRow(row) {

  }

  function DelRow(id) {

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
  .drop-zone {
    border: 2px dashed #aaa;
    border-radius: 10px;
    padding: 40px;
    text-align: center;
    cursor: pointer;
    transition: 0.2s;
  }

  .is-dragover {
    border-color: #409eff;
    background: #f0faff;
  }

  .hidden-input {
    display: none;
  }
</style>
