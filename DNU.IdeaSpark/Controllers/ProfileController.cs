using DNU.IdeaSpark.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DNU.IdeaSpark.Models.ViewModels.Ideas;
using DNU.IdeaSpark.Models.ViewModels.Account; // nếu ProfileViewModel ở đây
using Microsoft.AspNetCore.Http;

namespace DNU.IdeaSpark.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProfileController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            // 1. Lấy userId từ session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // 2. Lấy thông tin user, badges, ý tưởng đã gửi
            var user = _db.Users
                .Include(u => u.UserBadges)
                    .ThenInclude(ub => ub.Badge)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return NotFound();

            var userIdeas = _db.Ideas
                .Where(i => i.SubmitterUserId == userId)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

            var badges = user.UserBadges
                .Select(ub => new BadgeViewModel
                {
                    Name = ub.Badge.Name,
                    IconUrl = ub.Badge.IconUrl,
                    Description = ub.Badge.Description
                })
                .ToList();

            // 3. Build ViewModel truyền sang View
            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Department = user.Department,
                AvatarUrl = user.AvatarUrl,
                TotalPoints = user.TotalPoints, // hoặc user.Points
                Badges = badges,
                ContributedIdeas = userIdeas.Select(i => new IdeaSummaryViewModel
                {
                    IdeaId = i.IdeaId,
                    Title = i.Title,
                    SubmittedAt = i.CreatedAt,
                    Votes = i.VoteCount
                }).ToList()
            };

            return View(model);
        }
    }
}
