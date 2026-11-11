using alealamia.Domain.Enums;

namespace alealamia.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public int ClientProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool HasCustomization { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Pending;
    public decimal? FinalPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ClientProfile ClientProfile { get; set; } = null!;
    public ICollection<ProjectService> ProjectServices { get; set; } = new List<ProjectService>();
    public ICollection<ProjectStatusHistory> StatusHistory { get; set; } = new List<ProjectStatusHistory>();
    public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
    public ICollection<ProjectMarketingPackage> MarketingPackages { get; set; } = new List<ProjectMarketingPackage>();
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
    public ICollection<ProjectFile> ProjectFiles { get; set; } = new List<ProjectFile>();

}