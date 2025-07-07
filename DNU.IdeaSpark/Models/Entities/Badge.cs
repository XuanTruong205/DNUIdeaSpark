namespace DNU.IdeaSpark.Models.Entities;

public class Badge
{
    public int BadgeId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    
}
