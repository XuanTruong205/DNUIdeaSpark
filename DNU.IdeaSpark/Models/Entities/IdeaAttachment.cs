using System.ComponentModel.DataAnnotations;

namespace DNU.IdeaSpark.Models.Entities
{
    public class IdeaAttachment
    {
        [Key]
        public int AttachmentId { get; set; } // Thêm dòng này làm khóa chính

        public int IdeaId { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }

        public Idea Idea { get; set; }
    }
}