using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyDb.Models.EF;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VueApp1.Server.Models;

namespace VueApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysUserController : Controller
    {
        private readonly AppDbContext db;

        public SysUserController(AppDbContext appDb) 
        {
            db = appDb;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost("Auth")]
        public async Task<ResultModel<SysUser_View>> Auth(AuthData data)
        {
            ResultModel<SysUser_View> r = new ResultModel<SysUser_View>();
            string? Acc = data.Acc;
            string? Pwd = data.Pwd;
            string? Captcha = data.Captcha;
            if (string.IsNullOrEmpty(Acc))
            {
                r.ErrorMsg = "請填寫帳號!";
                return r;
            }
            if (string.IsNullOrEmpty(Pwd))
            {
                r.ErrorMsg = "請填寫密碼!";
                return r;
            }
            if (string.IsNullOrEmpty(Captcha))
            {
                r.ErrorMsg = "請填寫驗證碼!";
                return r;
            }

            var correctCaptcha = HttpContext.Session.GetString("CaptchaCode");
            if (string.IsNullOrEmpty(correctCaptcha) || Captcha != correctCaptcha)
            {
                r.ErrorMsg = "驗證碼錯誤";
                return r;
            }

            try
            {
                SysUser_View? dt = await db.SysUser_Views.FirstOrDefaultAsync(x => x.Acc == Acc && x.Pwd == Pwd);
                string JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                if (dt != null)
                {
                    r.Result = dt;
                    // 建立身份資訊（可加入角色）
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, dt.UserName),
                        new Claim(ClaimTypes.Role, dt.GroupName),
                        new Claim(ClaimTypes.UserData, JsonData)
                    };

                    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("MyCookieAuth", principal);
                    r.isSuccess = true;
                }
                else
                {
                    r.ErrorMsg = "登入失敗!";
                }
            }
            catch (Exception ex)
            {
                r.ErrorMsg = ex.Message;
            }
            
            return r;
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpGet("Logout")]
        public async Task<ResultModel<SysUser_View>> Logout()
        {
            ResultModel<SysUser_View> r = new ResultModel<SysUser_View>();
            await HttpContext.SignOutAsync("MyCookieAuth");
            r.isSuccess = true;
            return r;
        }

        /// <summary>
        /// 權限檢查
        /// </summary>
        /// <returns></returns>
        [HttpGet("AuthCheck")]
        public ResultModel<SysUser_View> AuthCheck()
        {
            ResultModel<SysUser_View> r = new ResultModel<SysUser_View>();
            var cookieData = User.Claims.Where(x => x.Type == ClaimTypes.UserData).FirstOrDefault();
            bool? IsAuthenticated = User.Identity?.IsAuthenticated;
            if (IsAuthenticated.Value == true && cookieData != null)
            {
                r.Result = Newtonsoft.Json.JsonConvert.DeserializeObject<SysUser_View>(cookieData.Value);
                r.isSuccess = true;
            }
            
            return r;
        }

        [HttpPost("GetGroupList")]
        public ResultModel<List<SysGroup>> GetGroupList(SysGroup post)
        {
            ResultModel<List<SysGroup>> r = new ResultModel<List<SysGroup>>();

            try
            {
                List<SysGroup> dt = db.SysGroups.Where(x=>string.IsNullOrEmpty(post.GroupName)||x.GroupName==post.GroupName).ToList();
                r.Result = dt;
                r.isSuccess = true;
            }
            catch (Exception ex)
            { 
                r.isSuccess = false;
                r.ErrorMsg = ex.Message;
            }
            return r;
        }

        [HttpPost("UpdateGroup")]
        public async Task<ResultModel<List<int>>> UpdateGroup(SysGroup post)
        {
            ResultModel<List<int>> r = new ResultModel<List<int>>();

            try
            {
                r.Result = await db.UpsertRangeAsync<SysGroup, int>(new List<SysGroup> { post });
                r.isSuccess = true;
            }
            catch (Exception ex)
            {
                r.isSuccess = false;
                r.ErrorMsg = ex.Message;
            }
            return r;
        }

        /// <summary>
        /// 刪除群組
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost("DeleteGroup")]
        public ResultModel<bool> DeleteGroup(SysGroup post)
        {
            ResultModel<bool> r = new ResultModel<bool>();
            try
            {
                if (post.id > 0) {
                    SysGroup data = db.SysGroups.FirstOrDefault(x => x.id == post.id);
                    if (data != null)
                    {
                        db.SysGroups.Remove(data);
                        db.SaveChanges();
                        r.Result = true;
                        r.isSuccess = true;
                    }
                } 
            }
            catch (Exception ex)
            {
                r.isSuccess = false;
                r.ErrorMsg = ex.Message;
            }
            return r;
        }

        /// <summary>
        /// 查詢使用者清單
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost("GetUserList")]
        public ResultModel<List<SysUser>> GetUserList(SysUser post)
        {
            ResultModel<List<SysUser>> r = new ResultModel<List<SysUser>>();
            try
            {
                r.Result = SysUserModel.GetSysUserList(db, post);
                r.isSuccess = true;
            }
            catch (Exception ex) 
            {
                r.isSuccess=false;
                r.ErrorMsg = ex.Message;
            }
            return r;
        }

        /// <summary>
        /// 編輯使用者
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost("UpdateUser")]
        public async Task<ResultModel<List<int>>> UpdateUser(SysUser post)
        {
            ResultModel<List<int>> r = new ResultModel<List<int>>();
            try
            {
                List<SysUser> _list = new List<SysUser>();
                _list.Add(post);
                r.Result = await SysUserModel.UpdateSysUserList(db, _list);
                r.isSuccess = true;
            }
            catch (Exception ex)
            {
                r.isSuccess = false;
                r.ErrorMsg = ex.Message;
            }
            return r;
        }

        /// <summary>
        /// 刪除使用者
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost("DeleteUser")]
        public ResultModel<bool> DeleteUser(SysUser post)
        {
            ResultModel<bool> r = new ResultModel<bool>();
            try
            {
                r.Result = SysUserModel.DelSysUser(db, post.id);
                r.isSuccess = true;
            }
            catch (Exception ex)
            {
                r.isSuccess = false;
                r.ErrorMsg = ex.Message;
            }
            return r;
        }

        /// <summary>
        /// 查詢權限清單
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost("GetAuthList")]
        public ResultModel<List<SysAuthList>> GetAuthList(SysAuthList post)
        {
            ResultModel<List<SysAuthList>> r = new ResultModel<List<SysAuthList>>();
            try
            {
                r.Result = AuthModel.GetAuthList(db, post);
                r.isSuccess = true;
            }
            catch (Exception ex)
            { 
                r.isSuccess = false;
                r.ErrorMsg = ex.Message;
            }
            return r;
        }

        [HttpPost("UpdateAuth")]
        public async Task<ResultModel<List<int>>> UpdateAuth(SysAuthList post)
        {
            ResultModel<List<int>> r = new ResultModel<List<int>>();
            try
            {
                r.Result = await db.UpsertRangeAsync<SysAuthList, int>(new List<SysAuthList> { post });
                r.isSuccess = true;
            }
            catch (Exception ex)
            {
                r.isSuccess = false;
                r.ErrorMsg = ex.Message;
            }
            return r;
        }

        [HttpPost("DeleteAuth")]
        public async Task<ResultModel<bool>> DeleteAuth(SysAuthList post)
        {
            ResultModel<bool> r = new ResultModel<bool>();
            try
            {
                //刪除非根目錄全部子資料
                if (post.pid > 0) 
                {
                    List<SysAuthList> _list = db.SysAuthLists.Where(x => x.pid == post.pid).ToList();
                    db.SysAuthLists.RemoveRange(_list);
                }
                //刪除主要資料
                SysAuthList item = await db.SysAuthLists.FirstOrDefaultAsync(x => x.id == post.id);
                if (item != null)
                {
                    db.SysAuthLists.Remove(item);
                }
                

                db.SaveChanges();
                r.Result = true;
                r.isSuccess = true;
            }
            catch (Exception ex)
            {
                r.isSuccess = false;
                r.ErrorMsg = ex.Message;
            }
            return r;
        }

        /// <summary>
        /// 查詢群組權限清單
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost("GetRelGroupAuth")]
        public ResultModel<List<RelGroupAuth>> GetRelGroupAuth(SysGroup post)
        {
            ResultModel<List<RelGroupAuth>> r = new ResultModel<List<RelGroupAuth>>();
            try
            {
                r.Result = db.RelGroupAuths.Where(x=>x.GroupID == post.id).ToList();
                r.isSuccess = true;
            }
            catch (Exception ex)
            {
                r.isSuccess = false;
                r.ErrorMsg = ex.Message;
            }
            return r;
        }

        [HttpPost("UpdateRelGroupAuth")]
        public async Task<ResultModel<List<int>>> UpdateRelGroupAuth(List<RelGroupAuth> post)
        {
            ResultModel<List<int>> r = new ResultModel<List<int>>();
            try
            {
                if (post.Count > 0)
                {
                    //先刪除資料
                    List<RelGroupAuth> data = db.RelGroupAuths.Where(x => x.GroupID == post[0].GroupID).ToList();
                    if (data.Count > 0)
                    {
                        db.RelGroupAuths.RemoveRange(data);
                        db.SaveChanges();
                    }
                    //在更新資料
                    r.Result = await db.UpsertRangeAsync<RelGroupAuth, int>(post);
                }
                r.isSuccess = true;
            }
            catch (Exception ex)
            {
                r.isSuccess = false;
                r.ErrorMsg = ex.Message;
            }
            return r;
        }

        /// <summary>
        /// 查詢群組權限清單
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [HttpPost("GetRelUserGroup")]
        public ResultModel<List<RelUserGroup>> GetRelUserGroup(SysUser post)
        {
            ResultModel<List<RelUserGroup>> r = new ResultModel<List<RelUserGroup>>();
            try
            {
                r.Result = db.RelUserGroups.Where(x => x.UserID == post.id).ToList();
                r.isSuccess = true;
            }
            catch (Exception ex)
            {
                r.isSuccess = false;
                r.ErrorMsg = ex.Message;
            }
            return r;
        }
    }
}
