using Newtonsoft.Json;
using System.Data;
using System.Linq;

namespace VueApp1.Server.Models
{
    public class StoModel
    {
        public static List<Dictionary<string, string>> GetList(string name)
        {
            List<Dictionary<string, string>> _list = new List<Dictionary<string, string>>();
            using (clsDB db = new clsDB())
            {
                string sql = string.Format("SELECT * FROM {0} WHERE 1=1", name);
                DataTable tb = db.ToDataTable(sql);
                _list = clsDB.ToDictionary(tb);
            }
            return _list;
        }

        /// <summary>
        /// 建立資料表
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static ResultModel<string> Update(string Name, List<Dictionary<string, string>> list)
        {
            ResultModel<string> result = new ResultModel<string>();
            string sql = "";
            try
            {
                sql += "CREATE TABLE " + Name + " (";
                sql += "id INTEGER PRIMARY KEY AUTOINCREMENT, ";
                foreach (var key in list[0].Keys)
                {
                    sql +=  key + " TEXT, ";
                }
                sql += ")";

                using (clsDB db = new clsDB())
                {
                    if (db.ToExecute(sql))
                    {
                        //處裡欄位名稱
                        sql = "INSERT INTO " + Name + " (";
                        foreach (Dictionary<string, string> row in list)
                        {
                            List<string> temp = new List<string>();
                            foreach (var key in row.Keys)
                            {
                                temp.Add(key + " TEXT");
                            }
                            sql += string.Join(",", temp.ToArray());
                        }
                        sql += ") VALUES";
                        //處裡內容資料
                        foreach (Dictionary<string, string> row in list)
                        {
                            sql = "(";
                            List<string> temp = new List<string>();
                            foreach (var key in row.Keys)
                            {
                                temp.Add("'" + row[key].Replace("'", "''") + "'");
                            }
                            sql += string.Join(",", temp.ToArray()) + ")";
                        }
                        
                        result.isSuccess = db.ToExecute(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.ErrorMsg = ex.Message;
            }

            return result;
        }
    }
}
