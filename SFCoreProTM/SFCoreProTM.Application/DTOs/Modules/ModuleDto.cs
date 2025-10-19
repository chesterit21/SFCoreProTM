using System;

namespace SFCoreProTM.Application.DTOs.Modules;

public class ModuleDto
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int Status { get; set; } // ModuleStatus enum value
}