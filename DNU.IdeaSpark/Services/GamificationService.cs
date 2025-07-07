using DNU.IdeaSpark.Data;
using DNU.IdeaSpark.Models.Entities;

namespace DNU.IdeaSpark.Services;

public class GamificationService
{
    private readonly ApplicationDbContext _db;

    public GamificationService(ApplicationDbContext db)
    {
        _db = db;
    }

    public void AddPoints(int userId, int points, string activityType, int? relatedIdeaId = null, int? relatedCommentId = null)
    {
        var log = new UserPointsLog
        {
            UserId = userId,
            Points = points,
            ActivityType = activityType,
            RelatedIdeaId = relatedIdeaId,
            RelatedCommentId = relatedCommentId
        };

        var user = _db.Users.Find(userId);
        user.Points += points;

        _db.UserPointsLogs.Add(log);
        _db.SaveChanges();
    }
}
