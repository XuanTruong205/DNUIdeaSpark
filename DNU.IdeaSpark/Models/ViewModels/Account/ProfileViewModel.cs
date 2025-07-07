using DNU.IdeaSpark.Models.ViewModels.Ideas;
namespace DNU.IdeaSpark.Models.ViewModels.Account
{
    public class ProfileViewModel
    {
        public string FullName { get; set; }
        public string Department { get; set; }
        public string AvatarUrl { get; set; }
        public int TotalPoints { get; set; }
        public int TotalIdeas { get; set; }
        public List<BadgeViewModel> Badges { get; set; } = new();  // <--- CHỈ CẦN DÒNG NÀY
        public List<IdeaSummaryViewModel> ContributedIdeas { get; set; } = new();

    }
    
    public class IdeaSummaryViewModel
    {
        public int IdeaId { get; set; }
        public string Title { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int Votes { get; set; }
    }
}