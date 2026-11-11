namespace alealamia.Domain.Entities;

public class ProjectMarketingPackageItem
{
    public int Id { get; set; }
    public int ProjectMarketingPackageId { get; set; }
    public int? OriginalPackageItemId { get; set; }
    public string NameSnapshot { get; set; } = string.Empty;
    public string? DescriptionSnapshot { get; set; }
    public int Quantity { get; set; }

    public ProjectMarketingPackage ProjectMarketingPackage { get; set; } = null!;
    public MarketingPackageItem? OriginalPackageItem { get; set; }
}