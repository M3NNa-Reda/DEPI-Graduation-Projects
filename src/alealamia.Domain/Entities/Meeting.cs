using alealamia.Domain.Enums;

namespace alealamia.Domain.Entities;

public class Meeting
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int ClientProfileId { get; set; }
    public DateTime ProposedAt { get; set; }
    public MeetingStatus Status { get; set; } = MeetingStatus.Pending;
    public string? AdminNotes { get; set; }
    public string? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public ClientProfile ClientProfile { get; set; } = null!;
}