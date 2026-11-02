using alealamia.Domain.Enums;

namespace alealamia.Domain.Entities;

public class ProjectStatusHistory
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public ProjectStatus? OldStatus { get; set; }
    public ProjectStatus NewStatus { get; set; }
    public string ChangedByUserId { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime ChangedAt { get; set; }

    public Project Project { get; set; } = null!;
}