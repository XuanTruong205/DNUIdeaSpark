using System.ComponentModel.DataAnnotations;
namespace DNU.IdeaSpark.Models.Entities;

public class IdeaStatusHistory
{
    [Key]
    public int HistoryId { get; set; }
    public int IdeaId { get; set; }
    public Idea Idea { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime StatusChangedAt { get; set; } = DateTime.UtcNow;
    public int ChangedByUserId { get; set; }
    public User ChangedByUser { get; set; } = null!;
    public string? Notes { get; set; }
}

