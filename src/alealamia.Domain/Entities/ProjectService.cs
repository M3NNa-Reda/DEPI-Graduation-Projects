namespace alealamia.Domain.Entities;

public class ProjectService
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int ServiceId { get; set; }
    public string? CustomizationDetails { get; set; }
    public DateTime CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public Service Service { get; set; } = null!;
}