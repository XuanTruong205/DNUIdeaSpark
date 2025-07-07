namespace DNU.IdeaSpark.Models.Entities;

public class IdeaTag
{
    public int IdeaId { get; set; }
    public Idea Idea { get; set; } = null!;
    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}