using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DNU.IdeaSpark.Data;
using DNU.IdeaSpark.Models.ViewModels.Admin;
using DNU.IdeaSpark.Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DNU.IdeaSpark.Controllers.Admin
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Tổng số ý tưởng
            var totalIdeas = await _context.Ideas.CountAsync();

            // Thống kê ý tưởng theo trạng thái
            var ideaByStatus = await _context.Ideas
                .GroupBy(i => i.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            // Thống kê ý tưởng theo danh mục
            var ideaByCategory = await _context.Ideas
                .GroupBy(i => i.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToListAsync();

            // Lấy tên danh mục
            var categories = await _context.IdeaCategories
                .ToDictionaryAsync(c => c.CategoryId, c => c.Name);

            // Top 5 ý tưởng nổi bật
            var topIdeas = await _context.Ideas
                .OrderByDescending(i => i.VoteCount)
                .ThenByDescending(i => i.CommentCount)
                .Take(5)
                .Select(i => new TopIdeaVm
                {
                    IdeaId = i.IdeaId,
                    Title = i.Title,
                    VoteCount = i.VoteCount,
                    CommentCount = i.CommentCount,
                    Status = i.Status
                })
                .ToListAsync();

            // Top 5 người dùng nhiều điểm nhất
            var topUsers = await _context.Users
                .OrderByDescending(u => u.Points)
                .Take(5)
                .Select(u => new TopUserVm
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Points = u.Points
                })
                .ToListAsync();

            // Tổng số user, tổng số bình luận
            var totalUsers = await _context.Users.CountAsync();
            var totalComments = await _context.Comments.CountAsync();

            // Thời gian xử lý trung bình (tính bằng ngày)
            var avgProcessTime = _context.IdeaStatusHistories
                .Where(h => h.Status == "Implemented" || h.Status == "Completed")
                .Join(_context.Ideas,
                    hist => hist.IdeaId,
                    idea => idea.IdeaId,
                    (hist, idea) => new { idea.CreatedAt, hist.StatusChangedAt })
                .AsEnumerable()
                .Select(x => (x.StatusChangedAt - x.CreatedAt).TotalDays)
                .DefaultIfEmpty(0)
                .Average();

            // Chuẩn bị model
            var model = new ReportDashboardViewModel
            {
                TotalIdeas = totalIdeas,
                IdeaByStatus = ideaByStatus.ToDictionary(x => x.Status, x => x.Count),
                IdeaByCategory = ideaByCategory.ToDictionary(
                    x => categories.ContainsKey(x.CategoryId) ? categories[x.CategoryId] : "Không rõ",
                    x => x.Count
                ),
                TopIdeas = topIdeas,
                TopUsers = topUsers,
                TotalUsers = totalUsers,
                TotalComments = totalComments,
                AverageProcessTimeDays = (double)avgProcessTime
            };

            // TRẢ VỀ VIEW VỚI ĐƯỜNG DẪN TUYỆT ĐỐI
            return View("~/Views/Admin/Reports/Index.cshtml", model);
        }
    }
}
