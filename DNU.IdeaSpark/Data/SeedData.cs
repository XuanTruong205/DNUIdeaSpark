using System;
using BCrypt.Net;
using System.Linq;
using DNU.IdeaSpark.Models.Entities;      // Đúng namespace của bạn
using DNU.IdeaSpark.Data;                // Nếu ApplicationDbContext nằm ở đây

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        // 1. Seed role Admin nếu chưa có
        if (!context.Roles.Any(r => r.Name == "Admin"))
        {
            context.Roles.Add(new Role { Name = "Admin", Description = "Quản trị hệ thống", CreatedAt = DateTime.UtcNow });
            context.SaveChanges();
        }

        // 2. Seed user Admin nếu chưa có
        var adminEmail = "admin@dnu.edu.vn";
        if (!context.Users.Any(u => u.Email == adminEmail))
        {
            var admin = new User
            {
                Email = adminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("YourStrongAdminPassword!"),
                FullName = "Quản trị viên hệ thống",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                Department = "Ban quản trị",
                AvatarUrl = null,
                Points = 0,
                TotalPoints = 0
            };
            context.Users.Add(admin);
            context.SaveChanges();

            var adminRole = context.Roles.First(r => r.Name == "Admin");
            context.UserRoles.Add(new UserRole
            {
                UserId = admin.UserId,
                RoleId = adminRole.RoleId
            });
            context.SaveChanges();
        }
    }
}