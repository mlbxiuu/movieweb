using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using MovieWebsite.Models;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using MovieWebsite.Data;

var builder = WebApplication.CreateBuilder(args);

// Thiết lập Serilog để ghi log vào file
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine(builder.Environment.WebRootPath, "Logs/app-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

// Thêm dịch vụ logging với Serilog
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddSerilog();
});

// Thêm localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Thêm dịch vụ cho MVC với hỗ trợ localization
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Thêm DbContext với cấu hình SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình giới hạn tải lên file lớn
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 524_288_000; // 500MB
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
});

// Cấu hình FormOptions cho dữ liệu multipart
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524_288_000;
    options.MemoryBufferThreshold = 524_288;
    options.BufferBody = true;
});

// Đăng ký IWebHostEnvironment
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

// Thêm HttpClient, Session
builder.Services.AddHttpClient();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cấu hình xác thực cookie
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

var app = builder.Build();

// Cấu hình localization middleware
var supportedCultures = new[] { "en-US", "vi-VN" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Áp dụng migrations tự động trong môi trường phát triển
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.Migrate();
        app.Logger.LogInformation("Đã áp dụng migrations cho cơ sở dữ liệu SQLite.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Lỗi khi áp dụng migrations cho cơ sở dữ liệu SQLite.");
    }
}

// Cấu hình pipeline xử lý HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Movie/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Đảm bảo thư mục Uploads tồn tại
var uploadsPath = Path.Combine(app.Environment.WebRootPath, "Uploads");
try
{
    Directory.CreateDirectory(uploadsPath);
    app.Logger.LogInformation($"Thư mục Uploads đã được tạo hoặc xác minh tại: {uploadsPath}");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, $"Lỗi khi tạo thư mục Uploads tại: {uploadsPath}");
}

// Đảm bảo thư mục Logs tồn tại
var logsPath = Path.Combine(app.Environment.WebRootPath, "Logs");
try
{
    Directory.CreateDirectory(logsPath);
    app.Logger.LogInformation($"Thư mục Logs đã được tạo hoặc xác minh tại: {logsPath}");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, $"Lỗi khi tạo thư mục Logs tại: {logsPath}");
}

// Cấu hình route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

//Route Admin
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");    

// Đảm bảo đóng Serilog khi ứng dụng dừng
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
