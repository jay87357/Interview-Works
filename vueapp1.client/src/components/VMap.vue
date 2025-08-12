<template>
  <div>
    <iframe ref="dom_iframe" src="/arcMap.html" class="border-0 pt-5" frameborder="0"></iframe>

    <!-- 表單造型 -->
    <ThemeSwitcher />
    <modal :title="'KML資料'" ref="tablemodal" :isdrag="true" :isresize="true">
      <template #body>
        <div class="table-responsive">
          <!-- 列表 -->
          <DataTable :value="list"
                     :paginator="true"
                     :rows="10"
                     :rowsPerPageOptions="[5, 10, 20, 50]"
                     :first="first"
                     class="table table-bordered"
                     showGridlines>
            <Column header="編號">
              <template #body="slotProps" class="text-nowrap">
                {{ slotProps.index + 1 }}
              </template>
            </Column>
            <Column v-for="(item, i) in columns" class="text-nowrap" :field="item" :header="item" />
          </DataTable>
        </div>
      </template>
    </modal>

    <modal :title="'樞紐分析結果'" ref="editmodal">
      <template #body>
        <div ref="chartRef" class="chart"></div>
      </template>
    </modal>

  </div>
</template>

<style scoped>
  iframe {
    position: fixed; /* 或 absolute */
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    margin: 0;
    padding: 0;
    z-index: 0;
  }
  .chart {
    width: 100%;
    height: 300px;
    background: white;
    border-radius: 6px;
    box-shadow: 0 0 5px rgba(0,0,0,0.3);
    padding: 10px;
  }
</style>

<script setup>
  import { onMounted, ref } from "vue"
  import * as echarts from 'echarts'
  import * as togeojson from '@tmcw/togeojson'
  import modal from '../components/ModalModel.vue'
  import DataTable from 'primevue/datatable'
  import Column from 'primevue/column'


  const dom_iframe = ref(null);
  const chartRef = ref(null)
  const editmodal = ref({});
  const tablemodal = ref({});
  const columns = ref([]);
  const list = ref([]);
  const first = ref(0);
  let chartInstance = null



  onMounted(() => {
    const iframe = dom_iframe.value?.contentWindow;
    if (iframe) {
      iframe.addEventListener("load", () => {
        
        const iframeDoc = iframe.contentDocument || iframe.document
        const kmlInput = iframeDoc.querySelector("#kmlInput");
        
        kmlInput.addEventListener("change", function (evt) {
          const file = evt.target.files[0];
          if (!file) return;

          const reader = new FileReader();
          reader.onload = function () {
            const parser = new DOMParser();
            const _kml = parser.parseFromString(reader.result, 'text/xml');
            const geojson = togeojson.kml(_kml);

            // 轉成 Blob 給 GeoJSONLayer 使用
            const blob = new Blob([JSON.stringify(geojson)], { type: "application/json" });
            const url = URL.createObjectURL(blob);

            //傳進去iframe裡面
            iframe.AddKml(url, file.name);

            
          };
          reader.readAsText(file);
        });
      });
    }

    window.addEventListener("message", event => {
      if (event.data?.type === "pivotResult") {
        const { field, data } = event.data

        const labels = Object.keys(data)
        const values = Object.values(data)

        if (!chartInstance) {
          chartInstance = echarts.init(chartRef.value)
        }

        chartInstance.setOption({
          title: { text: `欄位 ${field} 統計圖`, left: 'center' },
          tooltip: {},
          xAxis: { type: 'category', data: labels },
          yAxis: { type: 'value' },
          series: [{
            type: 'bar',
            data: values
          }]
        });

        editmodal.value.show();
        chartInstance.resize();
      }

      if (event.data?.type === "columnsResult") {
        const { data } = event.data
        //console.log(event.data)
        console.log(data)
        columns.value = data;
        //for (let i in data) {
        //  columns.value.push(data[i]);
        //}
      }

      if (event.data?.type === "listResult") {
        const { data } = event.data
        console.log(data)
        list.value = data;
        //for (let i in data) {
        //  list.value.push(data[i]);
        //}
      }

      if (event.data?.type === "OpenTableModal") {
        if (tablemodal.value) tablemodal.value.show();
      }
    })
  })
</script>
