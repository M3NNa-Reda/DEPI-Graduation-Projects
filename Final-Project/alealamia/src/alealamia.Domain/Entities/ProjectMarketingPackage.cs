using alealamia.Domain.Enums;

namespace alealamia.Domain.Entities;

public class ProjectMarketingPackage
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int MarketingPackageId { get; set; }
    public decimal PriceAtPurchase { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MarketingSubscriptionStatus Status { get; set; } = MarketingSubscriptionStatus.Active;
    public DateTime CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public MarketingPackage MarketingPackage { get; set; } = null!;
    public ICollection<ProjectMarketingPackageItem> Items { get; set; } = new List<ProjectMarketingPackageItem>();
}