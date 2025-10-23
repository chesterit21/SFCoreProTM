using System;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.DTOs.Projects;

public class ProjectDto
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProjectPath { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; }
}