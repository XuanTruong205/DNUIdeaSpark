using Microsoft.AspNetCore.Mvc;
using DNU.IdeaSpark.Data;
using DNU.IdeaSpark.Models.ViewModels.Admin;
using DNU.IdeaSpark.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore; // Để Include
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace DNU.IdeaSpark.Controllers.Admin
{
    [Area("Admin")]
    public class AdminLoginController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly AuthService _authService;

        public AdminLoginController(ApplicationDbContext db, AuthService authService)
        {
            _db = db;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/Admin/AdminLogin/Index.cshtml", new AdminLoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/AdminLogin/Index.cshtml", model);

            var user = _db.Users
                .Where(u => u.Email == model.Email)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefault();

            if (user == null || !_authService.VerifyPassword(model.Password, user.PasswordHash))
            {
                model.ErrorMessage = "Email hoặc mật khẩu không đúng.";
                return View("~/Views/Admin/AdminLogin/Index.cshtml", model);
            }

            if (!user.UserRoles.Any(r => r.Role.Name == "Admin"))
            {
                model.ErrorMessage = "Bạn không có quyền truy cập trang quản trị!";
                return View("~/Views/Admin/AdminLogin/Index.cshtml", model);
            }

            // Claims-based Authentication (dùng scheme "Cookies")
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("FullName", user.FullName ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            HttpContext.Session.SetInt32("UserId", user.UserId);

            // --- Redirect đúng Area về Admin/IdeaModeration
            return RedirectToAction("Index", "IdeaModeration", new { area = "Admin" });
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
