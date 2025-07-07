using DNU.IdeaSpark.Data;
using DNU.IdeaSpark.Services;
using DNU.IdeaSpark.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký DbContext với chuỗi kết nối
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Đăng ký các service tùy chỉnh (DI)
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<GamificationService>();
builder.Services.AddScoped<BadgeService, BadgeService>();

// 3. Đăng ký dịch vụ MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// 4. Đăng ký session
builder.Services.AddSession();

// 5. Đăng ký Cookie Authentication (chuẩn MVC)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/AdminLogin/Index";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// 6. Đăng ký IHttpContextAccessor để dùng trong Layout Razor, Service, Controller...
builder.Services.AddHttpContextAccessor();

// =========================
// BUILD & SEED DỮ LIỆU BAN ĐẦU
// =========================
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedData.Initialize(dbContext);
}

// =========================
// CẤU HÌNH MIDDLEWARE PIPELINE
// =========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();         // ⚠️ Phải đặt trước UseRouting
app.UseRouting();

app.UseAuthentication();  // ⚠️ Đặt trước Authorization
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // Định nghĩa route cho khu vực (areas) – rất quan trọng cho Admin
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    // Route mặc định cho user
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();