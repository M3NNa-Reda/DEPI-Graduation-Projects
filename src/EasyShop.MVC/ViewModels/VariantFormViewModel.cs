namespace EasyShop.MVC;

public class VariantFormViewModel
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "SKU")]
    public string SKU { get; set; } = string.Empty;

    [Range(0.01, 10_000_000, ErrorMessage = "Enter a price greater than zero.")]
    [Display(Name = "Price (EGP)")]
    public decimal Price { get; set; }

    [Range(0, 10_000_000, ErrorMessage = "Enter 0 or a positive amount.")]
    [Display(Name = "Original price (EGP)")]
    public decimal CompareAtPrice { get; set; }

    [Range(0, 1_000_000, ErrorMessage = "Stock cannot be negative.")]
    [Display(Name = "Stock")]
    public int StockQuantity { get; set; }

    [Display(Name = "On sale")]
    public bool IsActive { get; set; } = true;
}
