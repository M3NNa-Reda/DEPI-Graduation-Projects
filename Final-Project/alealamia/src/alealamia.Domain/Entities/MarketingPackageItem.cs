namespace alealamia.Domain.Entities;

public class MarketingPackageItem
{
    public int Id { get; set; }
    public int MarketingPackageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DefaultQuantity { get; set; }
    public bool IsActive { get; set; } = true;

    public MarketingPackage MarketingPackage { get; set; } = null!;
}