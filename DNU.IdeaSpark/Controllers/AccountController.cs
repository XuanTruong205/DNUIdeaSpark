using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DNU.IdeaSpark.Data;
using DNU.IdeaSpark.Models.Entities;
using DNU.IdeaSpark.Models.ViewModels.Account;
using DNU.IdeaSpark.Services;
using DNU.IdeaSpark.Models.ViewModels.Ideas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace DNU.IdeaSpark.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly AuthService _authService;
        private readonly EmailService _emailService;

        public AccountController(ApplicationDbContext db, AuthService authService, EmailService emailService)
        {
            _db = db;
            _authService = authService;
            _emailService = emailService;
        }

        // ========== USER LOGIN ==========
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _db.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.Email == model.Email);

            if (user != null && _authService.VerifyPassword(model.Password, user.PasswordHash))
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetInt32("UserId", user.UserId);

                // Lưu vai trò đầu tiên tìm được (ưu tiên Admin nếu có nhiều vai trò)
                var roleName = user.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault() ?? "User";
                HttpContext.Session.SetString("UserRole", roleName);

                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Sai email hoặc mật khẩu.");
            return View(model);
        }

        // ========== USER REGISTER ==========
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.Email.EndsWith("@dnu.edu.vn", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.Email), "Chỉ dùng email trường @dnu.edu.vn");
                return View(model);
            }

            if (_db.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng");
                return View(model);
            }

            var passwordHash = _authService.HashPassword(model.Password);

            // Tạo user mới
            var user = new User
            {
                Email = model.Email,
                FullName = model.Name,
                Department = model.Department,
                PasswordHash = passwordHash,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            // Gán role "User" mặc định
            var defaultRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (defaultRole == null)
            {
                defaultRole = new Role { Name = "User" };
                _db.Roles.Add(defaultRole);
                await _db.SaveChangesAsync();
            }

            user.UserRoles = new System.Collections.Generic.List<UserRole>
            {
                new UserRole { RoleId = defaultRole.RoleId, User = user }
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Gửi email xác nhận
            var token = Guid.NewGuid().ToString();
            await _emailService.SendEmailAsync(
                model.Email,
                "Xác thực tài khoản DNU IdeaSpark",
                $"Click xác nhận: https://localhost:5070/Account/ConfirmEmail?email={model.Email}&token={token}");

            ViewBag.Message = "Vui lòng kiểm tra email để xác thực tài khoản!";
            return View("RegisterSuccess");
        }

        // ========== FORGOT PASSWORD ==========
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _db.Users.SingleOrDefault(u => u.Email == model.Email);
            if (user != null)
            {
                var resetToken = Guid.NewGuid().ToString();
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Đặt lại mật khẩu DNU IdeaSpark",
                    $"Bạn vừa yêu cầu đặt lại mật khẩu. Click để đặt lại: https://localhost:5070/Account/ResetPassword?email={user.Email}&token={resetToken}"
                );
                // Bạn nên lưu token này vào db (User hoặc bảng PasswordReset nếu có) để xác thực reset!
            }
            ViewBag.Message = "Nếu email hợp lệ, hướng dẫn đặt lại mật khẩu đã được gửi đến email của bạn.";
            return View();
        }


        // ========== USER PROFILE ==========
        [HttpGet]
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
                return RedirectToAction("Login");

            var contributedIdeas = _db.Ideas
                .Where(i => i.SubmitterUserId == user.UserId)
                .Select(i => new IdeaSummaryViewModel
                {
                    IdeaId = i.IdeaId,
                    Title = i.Title,
                    SubmittedAt = i.CreatedAt,
                    Votes = i.VoteCount
                }).ToList();

            var badges = _db.UserBadges
                .Where(ub => ub.UserId == user.UserId)
                .Select(ub => new BadgeViewModel
                {
                    Name = ub.Badge.Name,
                    IconUrl = ub.Badge.IconUrl,
                    Description = ub.Badge.Description
                }).ToList();

            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Department = user.Department,
                AvatarUrl = user.AvatarUrl,
                TotalPoints = user.TotalPoints,
                Badges = badges,
                ContributedIdeas = contributedIdeas
            };

            return View("Profile", model);
        }

        // ========== ADMIN LOGIN ==========
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdminLogin(LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                model.ErrorMessage = "Vui lòng nhập đầy đủ Email và Mật khẩu!";
                return View(model);
            }

            var user = _db.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.Email == model.Email);

            if (user == null || !_authService.VerifyPassword(model.Password, user.PasswordHash))
            {
                model.ErrorMessage = "Email hoặc mật khẩu không đúng.";
                return View(model);
            }

            var isAdmin = user.UserRoles.Any(r => r.Role.Name == "Admin");
            if (!isAdmin)
            {
                model.ErrorMessage = "Bạn không có quyền truy cập trang quản trị!";
                return View(model);
            }

            // Lưu session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserRole", "Admin");
            return RedirectToAction("Index", "UserManagement", new { area = "Admin" });
        }

        // ========== LOGOUT (DÙNG CHUNG) ==========
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }
    }
}
