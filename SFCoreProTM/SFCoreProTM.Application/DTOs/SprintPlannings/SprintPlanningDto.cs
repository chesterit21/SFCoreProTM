using System;

namespace SFCoreProTM.Application.DTOs.SprintPlannings;

public class SprintPlanningDto
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid ModuleId { get; set; }
    public Guid TaskId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime TargetDate { get; set; }
    public int SortOrder { get; set; }
    public int SprintStatus { get; set; } // SprintStatus enum value
    public string Note { get; set; } = string.Empty;
}