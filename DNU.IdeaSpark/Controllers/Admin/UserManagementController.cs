using DNU.IdeaSpark.Data;
using DNU.IdeaSpark.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace DNU.IdeaSpark.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Moderator")]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Admin/UserManagement
        [HttpGet]
        public IActionResult Index()
        {
            var users = _db.Users.Select(u => new UserManagementViewModel
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Department = u.Department,
                Roles = _db.UserRoles
                    .Where(ur => ur.UserId == u.UserId)
                    .Join(_db.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => r.Name)
                    .ToList()
            }).ToList();

            return View("~/Views/Admin/UserManagement/Index.cshtml", users);
        }

        // GET: /Admin/UserManagement/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // TODO: Load user and roles for editing, ví dụ trả về ViewModel chi tiết người dùng và roles
            return Content("Trang chỉnh sửa người dùng (đang phát triển)");
        }

        // POST: /Admin/UserManagement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
                return NotFound();

            // Xóa kèm các liên kết (userroles, badges...) nếu có, tránh lỗi khóa ngoại (Foreign Key)
            var userRoles = _db.UserRoles.Where(ur => ur.UserId == id).ToList();
            if (userRoles.Any()) _db.UserRoles.RemoveRange(userRoles);

            var userBadges = _db.UserBadges.Where(ub => ub.UserId == id).ToList();
            if (userBadges.Any()) _db.UserBadges.RemoveRange(userBadges);

            // (Tùy chọn) Xử lý thêm nếu có các bảng liên kết khác...

            _db.Users.Remove(user);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Đã xóa người dùng.";
            return RedirectToAction("Index", new { area = "Admin" });
        }
    }
}
