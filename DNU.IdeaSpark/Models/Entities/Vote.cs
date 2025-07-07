namespace DNU.IdeaSpark.Models.Entities;

public class Vote
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int IdeaId { get; set; }
    public Idea Idea { get; set; } = null!;
}
