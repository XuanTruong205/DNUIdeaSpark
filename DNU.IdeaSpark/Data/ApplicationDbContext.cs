using Microsoft.EntityFrameworkCore;
using DNU.IdeaSpark.Models.Entities;

namespace DNU.IdeaSpark.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Idea> Ideas { get; set; }
        public DbSet<IdeaCategory> IdeaCategories { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        
        public DbSet<IdeaAttachment> IdeaAttachments { get; set; }
        public DbSet<IdeaStatusHistory> IdeaStatusHistories { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<UserPointsLog> UserPointsLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Composite keys
            builder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
            builder.Entity<Vote>().HasKey(v => new { v.UserId, v.IdeaId });
            builder.Entity<UserBadge>().HasKey(ub => new { ub.UserId, ub.BadgeId });

            // Self-referencing Comment (reply)
            builder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Idea - IdeaStatusHistory (1-n)
            builder.Entity<IdeaStatusHistory>()
                .HasOne(h => h.Idea)
                .WithMany(i => i.StatusHistories)
                .HasForeignKey(h => h.IdeaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Idea - IdeaAttachment (1-n)
            builder.Entity<IdeaAttachment>()
                .HasOne(a => a.Idea)
                .WithMany(i => i.Attachments)
                .HasForeignKey(a => a.IdeaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Idea - SubmitterUser (n-1, có thể null nếu ẩn danh)
            builder.Entity<Idea>()
                .HasOne(i => i.SubmitterUser)
                .WithMany(u => u.Ideas)
                .HasForeignKey(i => i.SubmitterUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Idea - Category (n-1)
            builder.Entity<Idea>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Ideas)
                .HasForeignKey(i => i.CategoryId);

            // Seed danh mục ý tưởng mặc định
            builder.Entity<IdeaCategory>().HasData(
                new IdeaCategory { CategoryId = 1, Name = "Cơ sở vật chất" },
                new IdeaCategory { CategoryId = 2, Name = "Đào tạo" },
                new IdeaCategory { CategoryId = 3, Name = "Công tác Sinh viên" },
                new IdeaCategory { CategoryId = 4, Name = "IT & Hệ thống" },
                new IdeaCategory { CategoryId = 5, Name = "Ký túc xá" },
                new IdeaCategory { CategoryId = 6, Name = "Thư viện" }
            );

            base.OnModelCreating(builder);
        }
    }
}
