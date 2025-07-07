using System.Collections.Generic;

namespace DNU.IdeaSpark.Models.ViewModels.Admin
{
    public class ReportDashboardViewModel
    {
        public int TotalIdeas { get; set; }
        public Dictionary<string, int> IdeaByStatus { get; set; }
        public Dictionary<string, int> IdeaByCategory { get; set; }
        public List<TopIdeaVm> TopIdeas { get; set; }
        public List<TopUserVm> TopUsers { get; set; }
        public int TotalUsers { get; set; }
        public int TotalComments { get; set; }
        public double AverageProcessTimeDays { get; set; }
    }
    public class TopIdeaVm
    {
        public int IdeaId { get; set; }
        public string Title { get; set; }
        public int VoteCount { get; set; }
        public int CommentCount { get; set; }
        public string Status { get; set; }
    }
    public class TopUserVm
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int Points { get; set; }
    }

}