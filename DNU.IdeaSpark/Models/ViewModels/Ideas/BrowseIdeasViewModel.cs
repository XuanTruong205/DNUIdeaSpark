using System;
using System.Collections.Generic;
using DNU.IdeaSpark.Models.Entities;

namespace DNU.IdeaSpark.Models.ViewModels.Ideas
{
    public class BrowseIdeasViewModel
    {
        public List<IdeaListItemViewModel> Ideas { get; set; } = new();
        public List<IdeaCategory> Categories { get; set; } = new();
        public string[] Statuses { get; set; } = Array.Empty<string>();
        public int? SelectedCategoryId { get; set; }
        public string? SelectedStatus { get; set; }
        public string? Search { get; set; }
        public string? Sort { get; set; }
    }

    public class IdeaListItemViewModel
    {
        public int IdeaId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SubmitterName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public int VoteCount { get; set; }
        public int CommentCount { get; set; }
    }
}