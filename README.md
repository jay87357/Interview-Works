# VueApp1
<p>為了面試而製作的成品</p>
<p>使用C#.net Core + vite + vue架構</p>
<p>後台驗證技術使用cookie驗證</p>
<p>資料庫連線使用entity framework core框架</p>
<p>資料庫系統為了方便攜帶使用SQLite</p>
<p>使用步驟如下</p>
<p>1. 下載完先對著方案點擊滑鼠右鍵 > 屬性 > 左方樹狀選單選擇 > 通用屬性 > 設定啟動專案 > 右方控制項中選擇多個啟動專案 > 右方控制項中的右下控制項中設定動作皆為啟動(預設為"無") > 點擊確定後關閉視窗</p>
<p>2. 對著方案點擊滑鼠右鍵 > 建置 (此時會自動安裝npm套件)</p>
<p>3. 用檔案總管開啟vueapp1.client.esproj所在位置 > 進入.vscode資料夾 > 對著資料夾內空白處點擊滑鼠右鍵>新增>文字文件 > 並命名為launch.json > 並貼上以下代碼後儲存:</p>
<code>
{
  "version": "0.2.0",
  "configurations": [
    {
      "type": "edge",
      "request": "launch",
      "name": "localhost (Edge)",
      "url": "https://localhost:60086",
      "webRoot": "${workspaceFolder}"
    },
    {
      "type": "chrome",
      "request": "launch",
      "name": "localhost (Chrome)",
      "url": "https://localhost:60086",
      "webRoot": "${workspaceFolder}"
    }
  ]
}
</code>
<p>按下F5即可啟動專案</p>
