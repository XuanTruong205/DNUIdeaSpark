using System;

namespace DNU.IdeaSpark.Models.ViewModels.Admin
{
    public class IdeaModerationViewModel
    {
        public int IdeaId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string SubmittedBy { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int VoteCount { get; set; }
    }

    public class IdeaModerationDetailViewModel
    {
        public int IdeaId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string SubmittedBy { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public List<StatusHistoryViewModel> StatusHistories { get; set; }
    }

    public class AttachmentViewModel
    {
        public string FileUrl { get; set; }
        public string FileType { get; set; }
    }

    public class StatusHistoryViewModel
    {
        public string Status { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; }
        public string Notes { get; set; }
    }
}