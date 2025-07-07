using DNU.IdeaSpark.Data;
using DNU.IdeaSpark.Models.Entities;

public class BadgeService
{
    private readonly ApplicationDbContext _db;
    public BadgeService(ApplicationDbContext db)
    {
        _db = db;
    }

    public void CheckAndAwardBadges(User user)
    {
        // Người đóng góp tích cực
        if (user.Points >= 100 && !_db.UserBadges.Any(ub => ub.UserId == user.UserId && ub.BadgeId == 1))
        {
            _db.UserBadges.Add(new UserBadge { UserId = user.UserId, BadgeId = 1, AwardedAt = DateTime.UtcNow });
            _db.SaveChanges();
        }
        // Ý tưởng đột phá
        var popularIdea = _db.Ideas.Any(i => i.SubmitterUserId == user.UserId && i.VoteCount >= 20);
        if (popularIdea && !_db.UserBadges.Any(ub => ub.UserId == user.UserId && ub.BadgeId == 2))
        {
            _db.UserBadges.Add(new UserBadge { UserId = user.UserId, BadgeId = 2, AwardedAt = DateTime.UtcNow });
            _db.SaveChanges();
        }
        // ... kiểm tra các huy hiệu khác ...
    }
}