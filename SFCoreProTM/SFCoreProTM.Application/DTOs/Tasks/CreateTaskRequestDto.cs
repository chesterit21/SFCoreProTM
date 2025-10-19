using System;

namespace SFCoreProTM.Application.DTOs.Tasks;

public class CreateTaskRequestDto
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid ModuleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 1;
    public bool IsErd { get; set; } = false;
}