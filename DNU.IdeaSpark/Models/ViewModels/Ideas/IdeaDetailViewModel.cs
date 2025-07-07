using DNU.IdeaSpark.Models.Entities;
namespace DNU.IdeaSpark.Models.ViewModels.Ideas
{
    public class IdeaDetailViewModel
    {
        public int IdeaId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string SubmitterName { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public int VoteCount { get; set; }
        public int CommentCount { get; set; }
        public Idea Idea { get; set; }
        public List<IdeaStatusHistory> StatusHistories { get; set; } = new();

        
        public List<AttachmentViewModel> Attachments { get; set; } = new();
        public List<StatusHistoryViewModel> StatusHistory { get; set; } = new();
        public List<CommentViewModel> Comments { get; set; } = new();
        public string SubmitterUserName { get; set; }
        public bool HasVoted { get; set; }

        
    }

    public class StatusHistoryViewModel
    {
        public string Status { get; set; }
        public DateTime StatusChangedAt { get; set; }
        public string Notes { get; set; }
    }

    public class AttachmentViewModel
    {
        public string FileUrl { get; set; }
        public string FileType { get; set; }
    }
    
}