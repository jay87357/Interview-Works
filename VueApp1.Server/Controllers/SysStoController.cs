using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using MyDb.Models.EF;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using VueApp1.Server.Models;

namespace VueApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysStoController : Controller
    {
        private readonly AppDbContext db;

        // 建構子注入 AppDbContext
        public SysStoController(AppDbContext appDb)
        {
            db = appDb;
        }

        /// <summary>
        /// 上傳/更新資料
        /// 前端設計 要上傳新資料只有新增 需要修改需要刪除重新新增
        /// </summary>
        /// <returns></returns>
        [HttpPost("UpLoad")]
        public IActionResult UpLoad(MIS_StoList post)
        {
            ResultModel<string> result = new ResultModel<string>();
            // 沒有設定資料表名稱
            if (post == null || string.IsNullOrEmpty(post.Name))
            {
                result.isSuccess = false;
                result.ErrorMsg = "參數錯誤";
                return Json(result);
            }
            else
            {
                //有資料庫名稱 
                //查詢清單有沒有
                List<MIS_StoList> _list = db.MIS_StoLists.Where(x => x.Name == post.Name).ToList();
                if (_list.Count > 0)
                {
                    // 有資料，只更新備註和日期
                    db.MIS_StoLists.Update(post);
                    result.isSuccess = true;
                }
                else
                {
                    // 沒有資料表名稱，新增
                    // 檢查是否有檔案上傳
                    if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                    {
                        //只接受csv
                        var file = Request.Form.Files[0];
                        string extension = Path.GetExtension(file.FileName).ToLower();
                        if (extension != ".csv")
                        {
                            result.isSuccess = false;
                            result.ErrorMsg = "只接受CSV檔案格式";
                            return Json(result);
                        }
                        else
                        {
                            List<Dictionary<string, string>> csvData = new List<Dictionary<string, string>>();
                            using (var stream = file.OpenReadStream())
                            using (var reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                string headerLine = reader.ReadLine();
                                if (headerLine == null)
                                {
                                    result.isSuccess = false;
                                    result.ErrorMsg = "CSV 資料不能為空";
                                    return Json(result);
                                }
                                    

                                // 解析欄位名稱
                                string[] headers = headerLine.Split(',');

                                while (!reader.EndOfStream)
                                {
                                    string line = reader.ReadLine();
                                    if (string.IsNullOrWhiteSpace(line))
                                        continue;

                                    string[] values = line.Split(',');

                                    var dict = new Dictionary<string, string>();
                                    for (int i = 0; i < headers.Length; i++)
                                    {
                                        string value = i < values.Length ? values[i] : "";
                                        dict[headers[i]] = value;
                                    }
                                    csvData.Add(dict);
                                }
                            }
                            // 新增清單資料 後回傳結果
                            return Json(StoModel.Update(post.Name, csvData));
                        }
                    }
                    else
                    {
                        result.isSuccess = false;
                        result.ErrorMsg = "請上傳檔案";
                    }
                }
            }

            

            return Json(result);
        }

        /// <summary>
        /// 取得上傳清單
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetList")]
        public IActionResult GetList(MIS_StoList data)
        {
            ResultModel<List<MIS_StoList>> result = new ResultModel<List<MIS_StoList>>();
            try
            {
                List<MIS_StoList> list = db.MIS_StoLists.ToList();
                result.isSuccess = true;
                if (!string.IsNullOrEmpty(data.Name))
                {
                    list = list.Where(x => x.Name.Contains(data.Name)).ToList();
                }

                if (!string.IsNullOrEmpty(data.UploadDate))
                {
                    // 假設 UploadDate 格式為 "yyyy-MM-dd to yyyy-MM-dd"
                    // 或 "yyyy/MM/dd to yyyy/MM/dd"
                    list = list.Where(x =>
                        x.UploadDate.CompareTo(data.UploadDate) >= 0 &&
                        x.UploadDate.CompareTo(data.UploadDate) <= 0
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(data.Memo))
                { 
                    list = list.Where(x => x.Memo.Contains(data.Memo)).ToList();
                }

                result.Result = list;
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.ErrorMsg = ex.Message;
            }
            return Json(result);
        }

        /// <summary>
        /// 顯示上傳清單內容
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [HttpGet("GetDataList")]
        public IActionResult GetDataList(string Name)
        {
            ResultModel<List<Dictionary<string, string>>> result = new ResultModel<List<Dictionary<string, string>>>();
            // 檢查參數
            List<MIS_StoList> _list = db.MIS_StoLists.Where(x => x.Name == Name).ToList();
            if (_list.Count > 0)
            {
                result.Result = StoModel.GetList(Name);
                result.isSuccess = true;
            }
            else
            {
                result.isSuccess = false;
                result.ErrorMsg = "查無此資料";
            }
            return Json(result);
        }
    }
}
