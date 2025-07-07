using DNU.IdeaSpark.Data;
using DNU.IdeaSpark.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace DNU.IdeaSpark.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Moderator")]
    [Route("Admin/[controller]/[action]")]
    public class CategoryManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Admin/CategoryManagement/Index
        [HttpGet]
        public IActionResult Index()
        {
            var categories = _db.IdeaCategories.OrderBy(c => c.CategoryId).ToList();
            return View("~/Views/Admin/CategoryManagement/Index.cshtml", categories);
        }

        // POST: /Admin/CategoryManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["ErrorMessage"] = "Tên danh mục không được để trống!";
                return RedirectToAction("Index");
            }

            if (_db.IdeaCategories.Any(c => c.Name == name.Trim()))
            {
                TempData["ErrorMessage"] = "Tên danh mục đã tồn tại!";
                return RedirectToAction("Index");
            }

            _db.IdeaCategories.Add(new IdeaCategory { Name = name.Trim() });
            _db.SaveChanges();
            TempData["SuccessMessage"] = "Thêm danh mục thành công!";
            return RedirectToAction("Index");
        }

        // POST: /Admin/CategoryManagement/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int CategoryId, string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                TempData["ErrorMessage"] = "Tên danh mục không được để trống!";
                return RedirectToAction("Index");
            }

            var category = _db.IdeaCategories.Find(CategoryId);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy danh mục!";
                return RedirectToAction("Index");
            }

            if (_db.IdeaCategories.Any(c => c.Name == Name.Trim() && c.CategoryId != CategoryId))
            {
                TempData["ErrorMessage"] = "Tên danh mục đã tồn tại!";
                return RedirectToAction("Index");
            }

            category.Name = Name.Trim();
            _db.SaveChanges();
            TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
            return RedirectToAction("Index");
        }

        // POST: /Admin/CategoryManagement/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var category = _db.IdeaCategories.Find(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Danh mục không tồn tại!";
                return RedirectToAction("Index");
            }

            // Kiểm tra nếu có ý tưởng nào đang dùng danh mục này thì KHÔNG xóa
            bool isUsed = _db.Ideas.Any(i => i.CategoryId == id);
            if (isUsed)
            {
                TempData["ErrorMessage"] = "Không thể xóa: Danh mục này đang được sử dụng cho một hoặc nhiều ý tưởng!";
                return RedirectToAction("Index");
            }

            _db.IdeaCategories.Remove(category);
            _db.SaveChanges();
            TempData["SuccessMessage"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
        }
    }
}
