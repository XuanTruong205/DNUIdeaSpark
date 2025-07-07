namespace DNU.IdeaSpark.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Department { get; set; }
        public string? AvatarUrl { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Points { get; set; }
        public int TotalPoints { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Idea> Ideas { get; set; } = new List<Idea>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    }
}