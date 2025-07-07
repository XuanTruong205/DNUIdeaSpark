namespace DNU.IdeaSpark.Models.Entities;

public class Tag
{
    public int TagId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<IdeaTag> IdeaTags { get; set; } = new List<IdeaTag>();
}