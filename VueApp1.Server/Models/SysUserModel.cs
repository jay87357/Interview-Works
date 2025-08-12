using MyDb.Models.EF;

namespace VueApp1.Server.Models
{
    public class SysUserModel
    {
        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="db"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<SysUser> GetSysUserList(AppDbContext db, SysUser data)
        {
            List<SysUser> _list = new List<SysUser>();

            _list = db.SysUsers
                .Where(x => data.id <= 0 || x.id == data.id)
                .Where(x => string.IsNullOrEmpty(data.Acc) || x.Acc.Contains(data.Acc))
                .Where(x => string.IsNullOrEmpty(data.Pwd) || x.Pwd == data.Pwd)
                .Where(x => string.IsNullOrEmpty(data.UserName) || x.UserName.Contains(data.UserName))
                .Where(x => string.IsNullOrEmpty(data.Email) || x.Email.Contains(data.Email))
                .ToList();

            return _list;
        }


        /// <summary>
        /// 新增/修改
        /// </summary>
        /// <param name="db"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<List<int>> UpdateSysUserList(AppDbContext db, List<SysUser> data)
        {
            //更新
            List<int> _list = await db.UpsertRangeAsync<SysUser, int>(data);
            return _list;
        }


        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DelSysUser(AppDbContext db, int id)
        {
            bool result = false;
            if (id <= 0) return false;

            SysUser data = db.SysUsers.FirstOrDefault(u => u.id == id);
            if (data != null)
            {
                db.SysUsers.Remove(data);
                db.SaveChanges();
            }

            return result;
        }
    }
}
