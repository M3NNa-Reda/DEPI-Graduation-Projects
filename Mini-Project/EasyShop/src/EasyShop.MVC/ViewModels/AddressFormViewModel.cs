namespace EasyShop.MVC.ViewModels;

public class AddressFormViewModel
{
    [Required]
    [StringLength(100)]
    public string Country { get; set; } = "Egypt";

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Area { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Street { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "Building number")]
    public string? BuildingNumber { get; set; }

    [StringLength(20)]
    [Display(Name = "Apartment number")]
    public string? ApartmentNumber { get; set; }

    [StringLength(20)]
    [Display(Name = "Postal code")]
    public string? PostalCode { get; set; }
}
