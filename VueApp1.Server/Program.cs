using Microsoft.EntityFrameworkCore;
using MyDb.Models.EF;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    //讓後端回傳JSON時不會以駝峰式命名
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache(); // 加入記憶體分散式快取
builder.Services.AddSession();


#region 初始化EF Core 
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
.Build();

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
string BasePath = AppDomain.CurrentDomain.BaseDirectory;
#if DEBUG
string? ConnStr = config.GetConnectionString("DebugDb");
#else
string ConnStr = config.GetConnectionString("MyDb");
#endif

optionsBuilder.UseSqlServer(ConnStr);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(ConnStr));
#endregion




#region Cookies驗證
//AddAuthentication 註冊驗證機制 括號中的 "" 是 驗證方案名稱
//AddCookie 指定使用 Cookie 當作驗證方式 驗證方式的名稱（需與 AddAuthentication(...) 呼應）。
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/";//未登入時導向哪個頁面
        options.AccessDeniedPath = "/";//登入後沒有權限時導向哪個頁面
    });

builder.Services.AddAuthorization();

#endregion




var app = builder.Build();

// Middleware 順序
app.UseSession();     // 要在 UseRouting 之前
app.Use(async (context, next) =>
{
    // 如果是 API 或靜態資源，就直接跳過
    if (context.Request.Path.StartsWithSegments("/api") ||
        context.Request.Path.Value.Contains("."))
    {
        await next();
    }
    else
    {
        context.Request.Path = "/index.html";
        await next();
    }
});
app.UseStaticFiles(); // 靜態檔案
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Controller 與預設路由
app.MapControllers();
app.MapDefaultControllerRoute();

// 最後 fallback，否則會攔截 API 路由
app.MapFallbackToFile("index.html");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.Run();

