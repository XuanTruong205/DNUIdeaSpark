namespace DNU.IdeaSpark.Models.Entities
{
    public class Idea
    {
        public int IdeaId { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        // Danh mục ý tưởng
        public int CategoryId { get; set; }
        public IdeaCategory Category { get; set; } = null!;

        // Người gửi (null nếu ẩn danh)
        public int? SubmitterUserId { get; set; }
        public User? SubmitterUser { get; set; }

        public bool IsAnonymous { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Trạng thái hiện tại của ý tưởng ("Received", "Under Review", "Approved", "Rejected", ...)
        /// </summary>
        public string Status { get; set; } = "Received";

        public int VoteCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;

        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // Tệp đính kèm
        public ICollection<IdeaAttachment> Attachments { get; set; } = new List<IdeaAttachment>();

        // Lịch sử trạng thái
        public ICollection<IdeaStatusHistory> StatusHistories { get; set; } = new List<IdeaStatusHistory>();

        // Tag (gắn nhãn, chuỗi cách nhau bởi dấu ';', nếu muốn có thể chuyển sang bảng phụ IdeaTag nếu nhiều)
        /// <summary>
        /// Các tag (từ khóa), cách nhau bởi dấu ';' (VD: "Ký túc xá;Đào tạo")
        /// </summary>
        public string? Tags { get; set; }
    }
}