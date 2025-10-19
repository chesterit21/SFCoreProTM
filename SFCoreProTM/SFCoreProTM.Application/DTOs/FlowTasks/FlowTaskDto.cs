using System;

namespace SFCoreProTM.Application.DTOs.FlowTasks;

public class FlowTaskDto
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid ModuleId { get; set; }
    public Guid TaskId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int FlowStatus { get; set; } // FlowStatus enum value
}