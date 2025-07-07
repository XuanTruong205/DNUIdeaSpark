using System.ComponentModel.DataAnnotations;
namespace DNU.IdeaSpark.Models.Entities;

public class UserPointsLog
{
    [Key]
    public int LogId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int Points { get; set; }
    public string ActivityType { get; set; } = null!;
    public int? RelatedIdeaId { get; set; }
    public int? RelatedCommentId { get; set; }
    public DateTime LogTime { get; set; } = DateTime.UtcNow;
}
