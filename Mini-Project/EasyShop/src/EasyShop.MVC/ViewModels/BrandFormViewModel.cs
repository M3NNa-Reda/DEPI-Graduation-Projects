namespace EasyShop.MVC;

public class BrandFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(300)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Display(Name = "Logo URL")]
    [StringLength(500)]
    public string? LogoUrl { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
