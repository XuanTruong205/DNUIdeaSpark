namespace DNU.IdeaSpark.Models.ViewModels.Ideas
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AuthorName { get; set; }
        public List<CommentViewModel> Replies { get; set; } = new();
    }
}