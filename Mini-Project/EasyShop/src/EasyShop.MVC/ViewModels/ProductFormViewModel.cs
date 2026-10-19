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

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
