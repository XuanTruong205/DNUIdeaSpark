namespace DNU.IdeaSpark.Models.Entities;

public class Comment
{
    public int CommentId { get; set; }
    public int IdeaId { get; set; }
    public Idea Idea { get; set; } = null!;
    public int AuthorUserId { get; set; }
    public User AuthorUser { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}

