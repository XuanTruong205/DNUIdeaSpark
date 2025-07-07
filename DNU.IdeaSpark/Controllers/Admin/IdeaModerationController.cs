using Microsoft.AspNetCore.Mvc;
using DNU.IdeaSpark.Data;
using Microsoft.EntityFrameworkCore;
using DNU.IdeaSpark.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace DNU.IdeaSpark.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Moderator")]
    public class IdeaModerationController : Controller
    {
        private readonly ApplicationDbContext _db;

        public IdeaModerationController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Admin/IdeaModeration
        public IActionResult Index()
        {
            var ideas = _db.Ideas
                .Include(i => i.Category)
                .Include(i => i.SubmitterUser)
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new IdeaModerationViewModel
                {
                    IdeaId = i.IdeaId,
                    Title = i.Title,
                    Category = i.Category.Name,
                    SubmittedBy = i.IsAnonymous ? "Ẩn danh" : (i.SubmitterUser != null ? i.SubmitterUser.FullName : "Ẩn danh"),
                    Status = i.Status,
                    CreatedAt = i.CreatedAt,
                    VoteCount = i.VoteCount
                }).ToList();

            return View("~/Views/Admin/IdeaModeration/Index.cshtml", ideas);
        }

        // GET: /Admin/IdeaModeration/Details/5
        public IActionResult Details(int id)
        {
            var idea = _db.Ideas
                .Include(i => i.Category)
                .Include(i => i.SubmitterUser)
                .Include(i => i.StatusHistories).ThenInclude(h => h.ChangedByUser)
                .Include(i => i.Attachments)
                .FirstOrDefault(i => i.IdeaId == id);

            if (idea == null) return NotFound();

            var model = new IdeaModerationDetailViewModel
            {
                IdeaId = idea.IdeaId,
                Title = idea.Title,
                Description = idea.Description,
                Category = idea.Category.Name,
                SubmittedBy = idea.IsAnonymous ? "Ẩn danh" : (idea.SubmitterUser != null ? idea.SubmitterUser.FullName : "Ẩn danh"),
                Status = idea.Status,
                CreatedAt = idea.CreatedAt,
                Attachments = idea.Attachments?.Select(a => new AttachmentViewModel
                {
                    FileUrl = a.FileUrl,
                    FileType = a.FileType
                }).ToList() ?? new List<AttachmentViewModel>(),
                StatusHistories = idea.StatusHistories
                    .OrderByDescending(h => h.StatusChangedAt)
                    .Select(h => new StatusHistoryViewModel
                    {
                        Status = h.Status,
                        ChangedAt = h.StatusChangedAt,
                        ChangedBy = h.ChangedByUser != null ? h.ChangedByUser.FullName : "Hệ thống",
                        Notes = h.Notes
                    }).ToList()
            };

            // TempData thông báo
            ViewBag.Error = TempData["Error"];
            ViewBag.Success = TempData["Success"];

            return View("~/Views/Admin/IdeaModeration/Details.cshtml", model);
        }

        // POST: /Admin/IdeaModeration/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string newStatus, string note)
        {
            var idea = _db.Ideas
                .Include(i => i.StatusHistories)
                .FirstOrDefault(i => i.IdeaId == id);

            if (idea == null) return NotFound();

            var allowedStatus = new[] {
                "Received", "Under Review", "Approved", "Acknowledged",
                "In Progress", "Implemented", "Completed", "Rejected", "Not Feasible", "Duplicate"
            };
            if (!allowedStatus.Contains(newStatus))
            {
                TempData["Error"] = "Trạng thái không hợp lệ!";
                return RedirectToAction("Details", new { id = id });
            }

            idea.Status = newStatus;

            // Lưu lịch sử trạng thái
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            idea.StatusHistories.Add(new DNU.IdeaSpark.Models.Entities.IdeaStatusHistory
            {
                Status = newStatus,
                StatusChangedAt = DateTime.UtcNow,
                ChangedByUserId = userId,
                Notes = note
            });

            _db.SaveChanges();
            TempData["Success"] = "Cập nhật trạng thái thành công!";
            return RedirectToAction("Details", new { id = id });
        }
    }
}
