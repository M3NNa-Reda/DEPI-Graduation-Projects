namespace EasyShop.MVC;

public class CouponFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [RegularExpression(@"^[A-Za-z0-9_-]+$", ErrorMessage = "Use letters, numbers, dashes and underscores only.")]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Discount type")]
    public DiscountType DiscountType { get; set; } = DiscountType.Percentage;

    [Display(Name = "Discount value")]
    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal DiscountValue { get; set; }

    [Display(Name = "Minimum order amount")]
    [Range(typeof(decimal), "0", "1000000")]
    public decimal? MinimumOrderAmount { get; set; }

    [Display(Name = "Maximum discount amount")]
    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal? MaximumDiscountAmount { get; set; }

    [Display(Name = "Usage limit")]
    [Range(1, int.MaxValue)]
    public int? UsageLimit { get; set; }

    [Display(Name = "Start date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;

    [Display(Name = "Expiration date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.Date.AddDays(30);

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
