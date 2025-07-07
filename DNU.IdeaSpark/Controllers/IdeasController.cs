using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DNU.IdeaSpark.Data;
using DNU.IdeaSpark.Models.Entities;
using DNU.IdeaSpark.Models.ViewModels.Ideas;
using Microsoft.AspNetCore.Http;

namespace DNU.IdeaSpark.Controllers
{
    public class IdeasController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly BadgeService _badgeService;

        // Constructor DUY NHẤT
        public IdeasController(ApplicationDbContext db, IWebHostEnvironment env, BadgeService badgeService)
        {
            _db = db;
            _env = env;
            _badgeService = badgeService;
        }

        // GET: Ideas/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_db.IdeaCategories.ToList(), "CategoryId", "Name");
            return View(new CreateIdeaViewModel());
        }

        // POST: Ideas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateIdeaViewModel model)
        {
            if (ModelState.IsValid)
            {
                int? userId = null;
                if (!model.IsAnonymous && HttpContext.Session.GetInt32("UserId") != null)
                    userId = HttpContext.Session.GetInt32("UserId");

                var idea = new Idea
                {
                    Title = model.Title,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Received", // Trạng thái chờ duyệt
                    IsAnonymous = model.IsAnonymous,
                    SubmitterUserId = userId
                };
                _db.Ideas.Add(idea);
                _db.SaveChanges();

                // Handle file attachment
                if (model.Attachment != null && model.Attachment.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Attachment.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Attachment.CopyTo(stream);
                    }
                    var attachment = new IdeaAttachment
                    {
                        IdeaId = idea.IdeaId,
                        FileUrl = "/uploads/" + fileName,
                        FileType = model.Attachment.ContentType
                    };
                    _db.IdeaAttachments.Add(attachment);
                    _db.SaveChanges();
                }

                // THÊM: Thông báo cho người dùng
                TempData["SuccessMessage"] = "Ý tưởng của bạn đã được gửi thành công và sẽ xuất hiện sau khi được phê duyệt!";

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Categories = new SelectList(_db.IdeaCategories.ToList(), "CategoryId", "Name", model.CategoryId);
            return View(model);
        }

        // GET: Ideas/Browse
        public IActionResult Browse(string search, int? categoryId, string status, string sort)
        {
            var categories = _db.IdeaCategories.ToList();
            var approvedStatus = new[] { "Approved", "Implemented", "Completed" };
            var statuses = new[] { "All", "Approved", "Implemented", "Completed" };

            // CHỈ LẤY Ý TƯỞNG ĐÃ ĐƯỢC DUYỆT
            var query = _db.Ideas
                .Include(i => i.Category)
                .Include(i => i.Comments)
                .Include(i => i.SubmitterUser)
                .Where(i => approvedStatus.Contains(i.Status));

            // Nếu filter theo status
            if (!string.IsNullOrEmpty(status) && status != "All" && approvedStatus.Contains(status))
                query = query.Where(i => i.Status == status);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(i => i.Title.Contains(search) || i.Description.Contains(search));
            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(i => i.CategoryId == categoryId.Value);

            query = sort switch
            {
                "votes" => query.OrderByDescending(i => i.VoteCount),
                "comments" => query.OrderByDescending(i => i.CommentCount),
                _ => query.OrderByDescending(i => i.CreatedAt),
            };

            var ideas = query
                .Select(i => new DNU.IdeaSpark.Models.ViewModels.Ideas.IdeaListItemViewModel
                {
                    IdeaId = i.IdeaId,
                    Title = i.Title,
                    CategoryName = i.Category.Name,
                    SubmitterName = i.IsAnonymous
                        ? "Ẩn danh"
                        : (i.SubmitterUserId == null ? "Ẩn danh" : i.SubmitterUser.FullName),
                    CreatedAt = i.CreatedAt,
                    Status = i.Status,
                    VoteCount = i.VoteCount,
                    CommentCount = i.CommentCount
                })
                .ToList();

            var vm = new DNU.IdeaSpark.Models.ViewModels.Ideas.BrowseIdeasViewModel
            {
                Ideas = ideas,
                Categories = categories,
                Statuses = statuses,
                SelectedCategoryId = categoryId,
                SelectedStatus = status,
                Search = search,
                Sort = sort
            };
            return View(vm);
        }

        // GET: Ideas/Details/{id}
        public IActionResult Details(int id)
        {
            var idea = _db.Ideas
                .Include(i => i.Category)
                .Include(i => i.Attachments)
                .Include(i => i.Votes)
                .Include(i => i.SubmitterUser)
                .Include(i => i.Comments)
                .ThenInclude(c => c.AuthorUser)
                .Include(i => i.StatusHistories)
                .ThenInclude(h => h.ChangedByUser)
                .FirstOrDefault(i => i.IdeaId == id);

            if (idea == null) return NotFound();

            var vm = new IdeaDetailViewModel
            {
                Idea = idea,
                SubmitterUserName = idea.IsAnonymous || idea.SubmitterUser == null
                    ? "Ẩn danh"
                    : idea.SubmitterUser.FullName,
                StatusHistories = idea.StatusHistories.OrderByDescending(h => h.StatusChangedAt).ToList(),
                Comments = idea.Comments.Where(c => c.ParentCommentId == null).OrderBy(c => c.CreatedAt)
                    .Select(c => new CommentViewModel
                    {
                        CommentId = c.CommentId,
                        AuthorName = c.AuthorUser == null ? "Ẩn danh" : c.AuthorUser.FullName,
                        CreatedAt = c.CreatedAt,
                        Content = c.Content
                    }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Upvote(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var existingVote = _db.Votes.FirstOrDefault(v => v.IdeaId == id && v.UserId == userId.Value);
            if (existingVote == null)
            {
                _db.Votes.Add(new Vote { IdeaId = id, UserId = userId.Value });
                var idea = _db.Ideas.Find(id);
                if (idea != null) idea.VoteCount += 1;
                _db.SaveChanges();
            }
            return RedirectToAction("Details", new { id });
        }

        // POST: /Ideas/AddComment
        [HttpPost]
        public IActionResult AddComment(int ideaId, string content, int? parentCommentId)
        {
            var comment = new Comment
            {
                IdeaId = ideaId,
                AuthorUserId = HttpContext.Session.GetInt32("UserId") ?? 0,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                ParentCommentId = parentCommentId
            };

            _db.Comments.Add(comment);

            // Tăng số lượng bình luận cho ý tưởng
            var idea = _db.Ideas.Find(ideaId);
            if (idea != null) idea.CommentCount++;

            // Cộng điểm cho user bình luận
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = _db.Users.Find(userId.Value);
                if (user != null)
                {
                    user.Points += 2;
                }
            }

            // Thưởng điểm cho tác giả ý tưởng nếu ý tưởng được bình luận
            if (idea != null && idea.SubmitterUserId != null)
            {
                var authorUser = _db.Users.Find(idea.SubmitterUserId.Value);
                if (authorUser != null)
                {
                    authorUser.Points += 1;
                }
            }

            _db.SaveChanges();
            return RedirectToAction("Details", new { id = ideaId });
        }

        public IActionResult Leaderboard()
        {
            var users = _db.Users
                .OrderByDescending(u => u.Points)
                .Take(20)
                .Select(u => new LeaderboardUserViewModel
                {
                    FullName = u.FullName,
                    Points = u.Points
                }).ToList();

            return View(users);
        }
    }
}
