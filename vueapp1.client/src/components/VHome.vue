
<template>
  <div>
    <!-- Icons Grid-->
    <section class="features-icons bg-light text-center">
      <div class="container">
        <div class="row">
          <div class="col-lg-4">
            <router-link to="/Map">
              <div class="features-icons-item mx-auto mb-5 mb-lg-0 mb-lg-3">
                <div class="features-icons-icon d-flex"><i class="bi-window m-auto text-primary"></i></div>
                <h3>圖台展示</h3>
                <p class="lead mb-0">地圖標點、資訊圖表展示</p>
              </div>
            </router-link>
          </div>
          <div class="col-lg-4">
            <router-link to="/SysMgn/Users">
              <div class="features-icons-item mx-auto mb-5 mb-lg-0 mb-lg-3">
                <div class="features-icons-icon d-flex"><i class="bi-gear m-auto text-primary"></i></div>
                <h3>後台管理</h3>
                <p class="lead mb-0">編輯使用者、群組、帳號權限</p>
              </div>
            </router-link>
          </div>
          <div class="col-lg-4">
            <router-link to="/SysSto/DataStore">
              <div class="features-icons-item mx-auto mb-0 mb-lg-3">
                <div class="features-icons-icon d-flex"><i class="bi-terminal m-auto text-primary"></i></div>
                <h3>資料匯入工具</h3>
                <p class="lead mb-0">把CSV、xls、xlsx匯入到資料庫且可以新增、編輯、刪除</p>
              </div>
            </router-link>
          </div>
        </div>
      </div>
    </section>
    <!-- Image Showcases-->
    <section class="showcase">
      <div class="container-fluid p-0">
        <div class="row g-0">
          <div class="col-lg-6 order-lg-2 text-white showcase-img" style="background-image: url('/img/TERIA.jpg');background-size:contain;background-repeat:no-repeat;"></div>
          <div class="col-lg-6 order-lg-1 my-auto showcase-text">
            <h2>地震衝擊資訊平台(TERIA)</h2>
            <p class="lead mb-0">這個專案主要是針對地震災害衝擊分析而設計的圖台系統。使用者在前端輸入斷層位置或座標、震源深度與規模等參數後，系統會透過伺服器端的排程程式進行分析運算，評估項目包含：地動、土壤液化、坡地崩塌、建物損害、人口影響、避難人數、短期收容需求、道路與橋梁、軌道橋梁、危險物質管線、供水與電力等。分析完成後，結果會在前端圖台上以圖形化方式呈現，方便使用者檢視與判讀。</p>
          </div>
        </div>
        <div class="row g-0">
          <div class="col-lg-6 text-white showcase-img" style="background-image: url('/img/TLAS.jpg'); background-size: contain; background-repeat: no-repeat;"></div>
          <div class="col-lg-6 my-auto showcase-text">
            <h2>天然災害經損服務網(TLAS)</h2>
            <p class="lead mb-0">這個專案主要針對天然災害（包含水災、風災、坡地災害與地震）造成的經濟損失進行統計與分析。系統可依使用者選定的災害範圍，快速提供損失相關資訊，並概算損失規模、災後復原所需的經費與人力，以及各類減免或補助措施，作為決策與規劃的重要參考依據。</p>
          </div>
        </div>
        <div class="row g-0">
          <div class="col-lg-6 order-lg-2 text-white showcase-img" style="background-image: url('/img/水利行政.jpg'); background-size: contain; background-repeat: no-repeat;"></div>
          <div class="col-lg-6 order-lg-1 my-auto showcase-text">
            <h2>水利資源資訊整合平台(水利行政系統)</h2>
            <p class="lead mb-0">這個專案主要負責管理水利系統，包括水利設備、水利工程、民眾報修案件以及政府公文書。系統首頁提供多種設備與工程位置的圖層展示，使用者可直接透過地圖圖層上的點位進行設備管理。此外，系統也支援多種報表輸出格式（XLSX、CSV、PDF），方便進行資料彙整與分析。</p>
          </div>
        </div>
        <div class="row g-0">
          <div class="col-lg-6 text-white showcase-img" style="background-image: url('/img/環保署.jpg'); background-size: contain; background-repeat: no-repeat;"></div>
          <div class="col-lg-6 my-auto showcase-text">
            <h2>環境地理資訊服務平臺</h2>
            <p class="lead mb-0">這個專案主要建置一個環境地理資訊服務平臺，提供多種環境相關圖資的查詢與視覺化功能。使用者可在前端選擇或篩選不同主題圖層（如空氣品質、水質、土壤污染、廢棄物處理等），並透過地圖工具進行測量、定位、標註與圖層樣式設定。系統後端採用統一的環境地理資料標準與 Open GIS 格式，確保跨單位資料的一致性與互操作性，並提供 API 與嵌入程式碼範例，方便其他系統串接。完成設定後，圖資可即時在前端展圖，並可輸出相關成果作為決策與分析的參考依據。</p>
          </div>
        </div>
      </div>
    </section>
    
    
  </div>
</template>
<script>
  export default {}
</script>
