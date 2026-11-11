namespace alealamia.Domain.Entities;

public class MarketingPackage
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMonths { get; set; }
    public decimal DefaultPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<MarketingPackageItem> Items { get; set; } = new List<MarketingPackageItem>();
    public ICollection<ProjectMarketingPackage> ProjectPurchases { get; set; } = new List<ProjectMarketingPackage>();
}