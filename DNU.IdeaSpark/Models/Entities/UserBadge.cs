namespace DNU.IdeaSpark.Models.Entities;

public class UserBadge
{
    public int UserId { get; set; }
    public int UserBadgeId { get; set; }
    public User User { get; set; } = null!;
    public int BadgeId { get; set; }
    public DateTime AwardedAt { get; set; }
    public Badge Badge { get; set; } = null!;
    public DateTime AwardedDate { get; set; } = DateTime.UtcNow;
}
