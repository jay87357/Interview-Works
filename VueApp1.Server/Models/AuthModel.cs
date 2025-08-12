using Microsoft.EntityFrameworkCore;
using MyDb.Models.EF;

namespace VueApp1.Server.Models
{
    public static class AuthModel
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="db"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<SysAuthList> GetAuthList(AppDbContext db, SysAuthList data)
        {
            List<SysAuthList> _list = new List<SysAuthList>();

            _list = db.SysAuthLists
                .Where(x => data.id <= 0 || x.id == data.id)
                .Where(x => string.IsNullOrEmpty(data.AuthName) || x.AuthName == data.AuthName)
                .Where(x => string.IsNullOrEmpty(data.AuthCode) || x.AuthCode == data.AuthCode)
                .Where(x => string.IsNullOrEmpty(data.AuthType) || x.AuthType == data.AuthType).ToList();

            return _list;
        }


        /// <summary>
        /// 新增/修改
        /// </summary>
        /// <param name="db"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<List<int>> UpdateAuthList(AppDbContext db, List<SysAuthList> data)
        {
            List<int> result = new List<int>();
            
            try
            {
                //更新
                result = await db.UpsertRangeAsync<SysAuthList, int>(data);
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.ToString());
            }
            return result;
        }


        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DelAuthList(AppDbContext db, int id)
        {
            bool result = false;
            if(id <= 0) return false;

            SysAuthList data = db.SysAuthLists.FirstOrDefault(u => u.id == id);
            if (data != null)
            {
                db.SysAuthLists.Remove(data);
                db.SaveChanges();
            }

            return result;
        }
    }
}
