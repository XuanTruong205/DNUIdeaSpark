using System.ComponentModel.DataAnnotations;
using DNU.IdeaSpark.Models.Entities;

namespace DNU.IdeaSpark.Models.Entities;

public class IdeaCategory
{
    [Key]
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Idea> Ideas { get; set; } = new List<Idea>();
}

