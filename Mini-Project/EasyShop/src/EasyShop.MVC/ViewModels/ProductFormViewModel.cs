using Microsoft.AspNetCore.Http;

namespace EasyShop.MVC;

public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(300)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(250)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Brand")]
    public int BrandId { get; set; }

    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    // The current picture, or a link to one hosted elsewhere. Clearing it removes the picture.
    [Display(Name = "Image link")]
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    // A picture chosen from the admin's computer. It replaces the link above when present.
    [Display(Name = "Upload a picture")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
