using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DNU.IdeaSpark.Models.ViewModels.Ideas;

public class CreateIdeaViewModel
{
    [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Nội dung ý tưởng là bắt buộc")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Bạn phải chọn danh mục")]
    public int CategoryId { get; set; }

    public IFormFile? Attachment { get; set; }

    public bool IsAnonymous { get; set; }
    
}